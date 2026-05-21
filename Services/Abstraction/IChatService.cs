using Career_Path.Contracts.Chat;

namespace Career_Path.Services.Abstraction;

public interface IChatService
{
    Task<IEnumerable<MessageResponse>> GetConversationAsync(
        string userId, string otherUserId, CancellationToken ct = default);
    Task MarkAsReadAsync(string senderId, string receiverId, CancellationToken ct = default);
}
