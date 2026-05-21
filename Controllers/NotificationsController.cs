using Career_Path.Contracts.Notifications;
using Career_Path.Services.Abstraction;

namespace Career_Path.Controllers;

[Route("api/notifications")]
[ApiController]
[Authorize]
public class NotificationsController(INotificationService _notificationService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? unreadOnly = null,
        CancellationToken ct = default)
    {
        var result = await _notificationService.GetUserNotificationsAsync(
            User.GetUserId()!, page, pageSize, unreadOnly, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken ct)
    {
        var count = await _notificationService.GetUnreadCountAsync(User.GetUserId()!, ct);
        return Ok(new { count });
    }

    [HttpPut("{notificationId}/read")]
    public async Task<IActionResult> MarkAsRead(string notificationId, CancellationToken ct)
    {
        var result = await _notificationService.MarkAsReadAsync(User.GetUserId()!, notificationId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken ct)
    {
        var result = await _notificationService.MarkAllAsReadAsync(User.GetUserId()!, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{notificationId}")]
    public async Task<IActionResult> DeleteNotification(string notificationId, CancellationToken ct)
    {
        var result = await _notificationService.DeleteNotificationAsync(User.GetUserId()!, notificationId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences(CancellationToken ct)
    {
        var result = await _notificationService.GetPreferencesAsync(User.GetUserId()!, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences(
        [FromBody] BulkUpdateNotificationPreferencesRequest request,
        CancellationToken ct)
    {
        var result = await _notificationService.UpdatePreferencesAsync(User.GetUserId()!, request, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}