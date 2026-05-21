using Career_Path.Contracts.Notifications;
using Career_Path.Hubs;
using Career_Path.Services.Abstraction;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;

namespace Career_Path.Services;


public class NotificationService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IHubContext<NotificationHub, NotificationHub.INotificationClient> hubContext,
    IEmailSender emailSender,
    ILogger<NotificationService> logger) : INotificationService
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IHubContext<NotificationHub, NotificationHub.INotificationClient> _hubContext = hubContext;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly ILogger<NotificationService> _logger = logger;

    // الأنواع اللي بتبعت Email بشكل افتراضي
    private static readonly HashSet<NotificationType> EmailDefaultTypes =
    [
        NotificationType.SecurityAlert,
        NotificationType.JobApplicationStatusChanged
    ];

    // ═══════════════════════════════════════════════════════════════
    //  إرسال
    // ═══════════════════════════════════════════════════════════════

    public async Task SendAsync(
        string recipientId,
        string? actorId,
        NotificationType type,
        string title,
        string message,
        string? entityType = null,
        string? entityId = null,
        NotificationPriority priority = NotificationPriority.Normal,
        CancellationToken ct = default)
    {
        try
        {
            // تحقق من الـ preferences للقناتين مع بعض دفعة واحدة
            var inAppEnabled = await IsInAppEnabledAsync(recipientId, type, ct);
            var emailEnabled = await IsEmailEnabledAsync(recipientId, type, ct);

            // لو القناتين معطّلتين ما فيش داعي نكمل خالص
            if (!inAppEnabled && !emailEnabled) return;

            // منع التكرار: لو نفس النوع ونفس الـ entity وما اتقراتش بعد
            if (entityId is not null && type == NotificationType.NewMessage)
            {
                var hasDuplicate = await _context.Notifications
                    .AnyAsync(n => n.RecipientId == recipientId
                                && n.Type == type
                                && n.EntityId == entityId
                                && !n.IsRead, ct);

                if (hasDuplicate) return;
            }

            var notification = new Notification
            {
                RecipientId = recipientId,
                ActorId = actorId,
                Type = type,
                Priority = priority,
                Title = title,
                Message = message,
                EntityType = entityType,
                EntityId = entityId
            };

            // بنحفظ الـ notification دايمًا في الـ DB (حتى لو in-app معطّل)
            // عشان الـ email channel مستقل
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(ct);

            // إرسال real-time عن طريق SignalR — بس لو in-app مفعّل
            if (inAppEnabled)
            {
                var actor = actorId is not null ? await _userManager.FindByIdAsync(actorId) : null;
                var response = MapToResponse(notification, actor);

                try
                {
                    await _hubContext.Clients.User(recipientId).ReceiveNotification(response);
                    var unreadCount = await GetUnreadCountAsync(recipientId, ct);
                    await _hubContext.Clients.User(recipientId).UnreadCountUpdated(unreadCount);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to push notification via SignalR to user {UserId}", recipientId);
                }
            }

            // إرسال email لو مفعّل — بنمرر الـ Id مباشرة بدون query جديدة
            if (emailEnabled)
                await TrySendEmailAsync(recipientId, notification.Id, title, message, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending notification of type {Type} to user {UserId}", type, recipientId);
        }
    }

    public async Task SendToManyAsync(
        IEnumerable<string> recipientIds,
        string? actorId,
        NotificationType type,
        string title,
        string message,
        string? entityType = null,
        string? entityId = null,
        NotificationPriority priority = NotificationPriority.Normal,
        CancellationToken ct = default)
    {
        foreach (var recipientId in recipientIds.Distinct())
        {
            if (ct.IsCancellationRequested) break;
            await SendAsync(recipientId, actorId, type, title, message, entityType, entityId, priority, ct);
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  استعلام
    // ═══════════════════════════════════════════════════════════════

    public async Task<Result<NotificationListResponse>> GetUserNotificationsAsync(
        string userId,
        int page = 1,
        int pageSize = 20,
        bool? unreadOnly = null,
        CancellationToken ct = default)
    {
        try
        {
            var query = _context.Notifications
                .Include(n => n.Actor)
                .Where(n => n.RecipientId == userId)
                .AsNoTracking();

            if (unreadOnly == true)
                query = query.Where(n => !n.IsRead);

            var totalCount = await query.CountAsync(ct);
            var unreadCount = await _context.Notifications
                .CountAsync(n => n.RecipientId == userId && !n.IsRead, ct);

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            var responses = notifications.Select(n => MapToResponse(n, n.Actor)).ToList();
            var hasMore = (page * pageSize) < totalCount;

            return Result.Success(new NotificationListResponse(
                responses, totalCount, unreadCount, page, pageSize, hasMore));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving notifications for user {UserId}", userId);
            return Result.Failure<NotificationListResponse>(NotificationErrors.Error);
        }
    }

    public async Task<int> GetUnreadCountAsync(string userId, CancellationToken ct = default)
    {
        return await _context.Notifications
            .CountAsync(n => n.RecipientId == userId && !n.IsRead, ct);
    }

    // ═══════════════════════════════════════════════════════════════
    //  قراءة وحذف
    // ═══════════════════════════════════════════════════════════════

    public async Task<Result> MarkAsReadAsync(string userId, string notificationId, CancellationToken ct = default)
    {
        try
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId, ct);

            if (notification is null)
                return Result.Failure(NotificationErrors.NotFound);

            if (notification.RecipientId != userId)
                return Result.Failure(NotificationErrors.Unauthorized);

            if (notification.IsRead)
                return Result.Success(); // Idempotent

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);

            var unreadCount = await GetUnreadCountAsync(userId, ct);
            await _hubContext.Clients.User(userId).UnreadCountUpdated(unreadCount);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while marking notification {NotificationId} as read", notificationId);
            return Result.Failure(NotificationErrors.Error);
        }
    }

    public async Task<Result> MarkAllAsReadAsync(string userId, CancellationToken ct = default)
    {
        try
        {
            var now = DateTime.UtcNow;

            await _context.Notifications
                .Where(n => n.RecipientId == userId && !n.IsRead)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(n => n.IsRead, true)
                    .SetProperty(n => n.ReadAt, now), ct);

            await _hubContext.Clients.User(userId).UnreadCountUpdated(0);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while marking all notifications as read for user {UserId}", userId);
            return Result.Failure(NotificationErrors.Error);
        }
    }

    public async Task<Result> DeleteNotificationAsync(string userId, string notificationId, CancellationToken ct = default)
    {
        try
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId, ct);

            if (notification is null)
                return Result.Failure(NotificationErrors.NotFound);

            if (notification.RecipientId != userId)
                return Result.Failure(NotificationErrors.Unauthorized);

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync(ct);

            // لو كانت unread نحدّث الـ count
            if (!notification.IsRead)
            {
                var unreadCount = await GetUnreadCountAsync(userId, ct);
                await _hubContext.Clients.User(userId).UnreadCountUpdated(unreadCount);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting notification {NotificationId}", notificationId);
            return Result.Failure(NotificationErrors.Error);
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  تفضيلات
    // ═══════════════════════════════════════════════════════════════

    public async Task<Result<NotificationPreferencesListResponse>> GetPreferencesAsync(
        string userId, CancellationToken ct = default)
    {
        try
        {
            var saved = await _context.NotificationPreferences
                .Where(np => np.UserId == userId)
                .AsNoTracking()
                .ToListAsync(ct);

            var allTypes = Enum.GetValues<NotificationType>();
            var savedDict = saved.ToDictionary(np => np.Type);

            var preferences = allTypes.Select(type =>
            {
                if (savedDict.TryGetValue(type, out var pref))
                    return new NotificationPreferenceResponse(type.ToString(), pref.InAppEnabled, pref.EmailEnabled);

                // Default: InApp دايمًا مفعّل، Email بس للأنواع المحددة
                return new NotificationPreferenceResponse(
                    type.ToString(),
                    InAppEnabled: true,
                    EmailEnabled: EmailDefaultTypes.Contains(type));
            }).ToList();

            return Result.Success(new NotificationPreferencesListResponse(preferences));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving notification preferences for user {UserId}", userId);
            return Result.Failure<NotificationPreferencesListResponse>(NotificationErrors.Error);
        }
    }

    public async Task<Result> UpdatePreferencesAsync(
        string userId,
        BulkUpdateNotificationPreferencesRequest request,
        CancellationToken ct = default)
    {
        try
        {
            foreach (var item in request.Preferences)
            {
                if (!Enum.TryParse<NotificationType>(item.Type, out var type))
                    return Result.Failure(NotificationErrors.InvalidType);

                var existing = await _context.NotificationPreferences
                    .FirstOrDefaultAsync(np => np.UserId == userId && np.Type == type, ct);

                if (existing is not null)
                {
                    existing.InAppEnabled = item.InAppEnabled;
                    existing.EmailEnabled = item.EmailEnabled;
                }
                else
                {
                    _context.NotificationPreferences.Add(new NotificationPreference
                    {
                        UserId = userId,
                        Type = type,
                        InAppEnabled = item.InAppEnabled,
                        EmailEnabled = item.EmailEnabled
                    });
                }
            }

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating notification preferences for user {UserId}", userId);
            return Result.Failure(NotificationErrors.Error);
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  Private Helpers
    // ═══════════════════════════════════════════════════════════════

    private async Task<bool> IsInAppEnabledAsync(string userId, NotificationType type, CancellationToken ct)
    {
        var pref = await _context.NotificationPreferences
            .AsNoTracking()
            .FirstOrDefaultAsync(np => np.UserId == userId && np.Type == type, ct);

        return pref?.InAppEnabled ?? true; // Default: مفعّل
    }

    private async Task<bool> IsEmailEnabledAsync(string userId, NotificationType type, CancellationToken ct)
    {
        var pref = await _context.NotificationPreferences
            .AsNoTracking()
            .FirstOrDefaultAsync(np => np.UserId == userId && np.Type == type, ct);

        return pref?.EmailEnabled ?? EmailDefaultTypes.Contains(type);
    }

    // بتاخد notificationId مباشرة — مش بتعمل query تانية تلاقيه
    private async Task TrySendEmailAsync(
        string recipientId,
        string notificationId,
        string title,
        string body,
        CancellationToken ct)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(recipientId);
            if (user?.Email is null) return;

            await _emailSender.SendEmailAsync(user.Email, title, body);

            // بنجيب الـ notification بالـ PK مباشرة — بدون query زيادة
            var notification = await _context.Notifications.FindAsync([notificationId], ct);
            if (notification is not null)
            {
                notification.EmailSent = true;
                await _context.SaveChangesAsync(ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send notification email to user {UserId}", recipientId);
        }
    }

    private static NotificationResponse MapToResponse(Notification n, ApplicationUser? actor)
    {
        return new NotificationResponse(
            Id: n.Id,
            Type: n.Type.ToString(),
            Priority: n.Priority.ToString(),
            Title: n.Title,
            Message: n.Message,
            ActorId: n.ActorId,
            ActorName: actor?.FullName,
            ActorPhotoUrl: actor?.UserProfile?.ProfilePictureUrl,
            EntityType: n.EntityType,
            EntityId: n.EntityId,
            IsRead: n.IsRead,
            CreatedAt: n.CreatedAt,
            ReadAt: n.ReadAt
        );
    }
}