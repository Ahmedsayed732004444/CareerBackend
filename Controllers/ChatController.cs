using Career_Path.Services.Abstraction;

namespace Career_Path.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController(IChatService chatService) : ControllerBase
    {
        [HttpGet("{otherUserId}")]
        public async Task<IActionResult> GetConversation(string otherUserId, CancellationToken ct)
        {
            var messages = await chatService.GetConversationAsync(User.GetUserId()!, otherUserId, ct);
            return Ok(messages);
        }

        [HttpPut("{senderId}/read")]
        public async Task<IActionResult> MarkAsRead(string senderId, CancellationToken ct)
        {
            await chatService.MarkAsReadAsync(senderId, User.GetUserId()!, ct);
            return NoContent();
        }
    }
}
