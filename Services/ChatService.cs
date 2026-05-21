using Career_Path.Contracts.Chat;
using Career_Path.Services.Abstraction;

namespace Career_Path.Services
{
    public class ChatService(ApplicationDbContext context) : IChatService
    {
        public async Task<IEnumerable<MessageResponse>> GetConversationAsync(
            string userId, string otherUserId, CancellationToken ct = default)
        {
            return await context.Messages
                .Where(m =>
                    (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                    (m.SenderId == otherUserId && m.ReceiverId == userId))
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageResponse(
                    m.Id, m.SenderId, m.Sender.FullName, m.Content, m.SentAt, m.IsRead))
                .ToListAsync(ct);
        }

        public async Task MarkAsReadAsync(string senderId, string receiverId, CancellationToken ct = default)
        {
            await context.Messages
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsRead, true), ct);
        }

    }
}
