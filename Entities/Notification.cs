namespace Career_Path.Entities;

public enum NotificationType
{
    // Social
    NewFollower,
    PostLiked,
    PostCommented,
    CommentReplied,
    CommentReacted,

    // Jobs
    JobApplicationReceived,
    JobApplicationStatusChanged,

    // Chat
    NewMessage,

    // System
    SecurityAlert,
    GeneralInfo
}

public enum NotificationPriority
{
    Low,
    Normal,
    High
}

public class Notification
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string RecipientId { get; set; } = string.Empty;
    public ApplicationUser Recipient { get; set; } = default!;

    public string? ActorId { get; set; }
    public ApplicationUser? Actor { get; set; }

    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    // الـ Entity اللي الـ notification بتتكلم عنه (مثلاً PostId أو JobId)
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }

    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public bool EmailSent { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}