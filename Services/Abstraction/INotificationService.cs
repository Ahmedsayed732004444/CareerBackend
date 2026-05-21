using Career_Path.Contracts.Notifications;

namespace Career_Path.Services.Abstraction;


public interface INotificationService
{
    // ─── إرسال ──────────────────────────────────────────────────────────────

    Task SendAsync(
        string recipientId,
        string? actorId,
        NotificationType type,
        string title,
        string message,
        string? entityType = null,
        string? entityId = null,
        NotificationPriority priority = NotificationPriority.Normal,
        CancellationToken ct = default);

    Task SendToManyAsync(
        IEnumerable<string> recipientIds,
        string? actorId,
        NotificationType type,
        string title,
        string message,
        string? entityType = null,
        string? entityId = null,
        NotificationPriority priority = NotificationPriority.Normal,
        CancellationToken ct = default);

    // ─── استعلام ────────────────────────────────────────────────────────────

    Task<Result<NotificationListResponse>> GetUserNotificationsAsync(
        string userId,
        int page = 1,
        int pageSize = 20,
        bool? unreadOnly = null,
        CancellationToken ct = default);

    Task<int> GetUnreadCountAsync(string userId, CancellationToken ct = default);

    // ─── قراءة وحذف ─────────────────────────────────────────────────────────

    Task<Result> MarkAsReadAsync(string userId, string notificationId, CancellationToken ct = default);
    Task<Result> MarkAllAsReadAsync(string userId, CancellationToken ct = default);
    Task<Result> DeleteNotificationAsync(string userId, string notificationId, CancellationToken ct = default);

    // ─── تفضيلات ────────────────────────────────────────────────────────────

    Task<Result<NotificationPreferencesListResponse>> GetPreferencesAsync(string userId, CancellationToken ct = default);

    Task<Result> UpdatePreferencesAsync(
        string userId,
        BulkUpdateNotificationPreferencesRequest request,
        CancellationToken ct = default);
}