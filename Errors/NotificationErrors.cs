namespace Career_Path.Errors;


public record class NotificationErrors
{
    public static readonly Error NotFound =
        new("Notification.NotFound", "The specified notification was not found", StatusCodes.Status404NotFound);

    public static readonly Error Unauthorized =
        new("Notification.Unauthorized", "You are not authorized to access this notification", StatusCodes.Status403Forbidden);

    public static readonly Error InvalidType =
        new("Notification.InvalidType", "The specified notification type is invalid", StatusCodes.Status400BadRequest);

    public static readonly Error Error =
        new("Notification.Error", "An error occurred while processing the notification", StatusCodes.Status500InternalServerError);
}
