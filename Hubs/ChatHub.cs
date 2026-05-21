using Career_Path.Contracts.Chat;
using Microsoft.AspNetCore.SignalR;

namespace Career_Path.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly ApplicationDbContext _context;

    public ChatHub(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SendMessage(string receiverId, string content)
    {
        var senderId = Context.UserIdentifier!;

        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        var sender = await _context.Users.FindAsync(senderId);

        var response = new MessageResponse(
            message.Id,
            senderId,
            sender!.FullName,
            content,
            message.SentAt,
            false
        );

        // ابعت للمستقبل
        await Clients.User(receiverId).SendAsync("ReceiveMessage", response);
        // ابعت للمرسل تاني
        await Clients.Caller.SendAsync("ReceiveMessage", response);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier!;
        await Clients.Others.SendAsync("UserOnline", userId);
        await base.OnConnectedAsync();
    }
}
