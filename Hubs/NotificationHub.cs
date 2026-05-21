using Career_Path.Contracts.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Career_Path.Hubs;

[Authorize]
// 1. التعديل الأول: الوراثة من Hub<T> بدلاً من Hub
public class NotificationHub : Hub<NotificationHub.INotificationClient>
{
    public interface INotificationClient
    {
        Task ReceiveNotification(NotificationResponse notification);
        Task UnreadCountUpdated(int count);

        // 2. التعديل الثاني: إضافة الدالة هنا لتتمكن من استخدامها
        Task UserOnline(string userId);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier!;

        // 3. التعديل الثالث: استدعاء الدالة مباشرة من الواجهة بدلاً من SendAsync
        await Clients.Others.UserOnline(userId);

        await base.OnConnectedAsync();
    }
}