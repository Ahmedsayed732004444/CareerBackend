namespace Career_Path.Contracts.Notifications;


public record NotificationResponse(
    string Id,
    string Type,
    string Priority,
    string Title,
    string Message,
    string? ActorId,
    string? ActorName,
    string? ActorPhotoUrl,
    string? EntityType,
    string? EntityId,
    bool IsRead,
    DateTime CreatedAt,
    DateTime? ReadAt
);

public record NotificationListResponse(
    List<NotificationResponse> Items,
    int TotalCount,
    int UnreadCount,
    int PageNumber,
    int PageSize,
    bool HasMore
);

public record NotificationPreferenceResponse(
    string Type,
    bool InAppEnabled,
    bool EmailEnabled
);

public record NotificationPreferencesListResponse(
    List<NotificationPreferenceResponse> Preferences
);

// ─── Requests ────────────────────────────────────────────────────────────────

public record BulkUpdateNotificationPreferencesRequest(
    List<NotificationPreferenceItem> Preferences
);

public record NotificationPreferenceItem(
    string Type,
    bool InAppEnabled,
    bool EmailEnabled
);
