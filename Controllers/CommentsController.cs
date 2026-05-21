using Career_Path.Contracts.Comments;
using Career_Path.Contracts.Common;
using Career_Path.Services.Abstraction;

namespace Career_Path.Controllers;

[Route("api/Comments")]
[ApiController]
[Authorize]
public class CommentsController(ICommentService _commentService) : ControllerBase
{
    // ──────────────────────────────────────────────
    //  Comment
    // ──────────────────────────────────────────────

    [HttpPost("{postId}")]
    public async Task<IActionResult> AddComment(string postId, [FromBody] AddCommentRequest request, CancellationToken ct)
    {
        var response = await _commentService.AddCommentAsync(User.GetUserId()!, postId, request, ct);
        return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
    }

    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(string commentId, CancellationToken ct)
    {
        var response = await _commentService.DeleteCommentAsync(User.GetUserId()!, commentId, ct);
        return response.IsSuccess ? NoContent() : response.ToProblem();
    }

    [HttpGet("{postId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostComments(string postId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        return Ok(await _commentService.GetPostCommentsAsync(postId, User.GetUserId(), filters, ct));
    }

    // ──────────────────────────────────────────────
    //  Reply
    // ──────────────────────────────────────────────

    [HttpPost("{commentId}/replies")]
    public async Task<IActionResult> AddReply(string commentId, [FromBody] AddReplyRequest request, CancellationToken ct)
    {
        var response = await _commentService.AddReplyAsync(User.GetUserId()!, commentId, request, ct);
        return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
    }

    [HttpDelete("replies/{replyId}")]
    public async Task<IActionResult> DeleteReply(string replyId, CancellationToken ct)
    {
        var response = await _commentService.DeleteReplyAsync(User.GetUserId()!, replyId, ct);
        return response.IsSuccess ? NoContent() : response.ToProblem();
    }

    // ──────────────────────────────────────────────
    //  Comment Reactions
    // ──────────────────────────────────────────────

    [HttpPost("{commentId}/like")]
    public async Task<IActionResult> LikeComment(string commentId, CancellationToken ct)
    {
        var response = await _commentService.LikeCommentAsync(User.GetUserId()!, commentId, ct);
        return response.IsSuccess ? NoContent() : response.ToProblem();
    }

    [HttpDelete("{commentId}/like")]
    public async Task<IActionResult> UnlikeComment(string commentId, CancellationToken ct)
    {
        var response = await _commentService.UnlikeCommentAsync(User.GetUserId()!, commentId, ct);
        return response.IsSuccess ? NoContent() : response.ToProblem();
    }

    // ──────────────────────────────────────────────
    //  Reply Reactions
    // ──────────────────────────────────────────────

    [HttpPost("replies/{replyId}/like")]
    public async Task<IActionResult> LikeReply(string replyId, CancellationToken ct)
    {
        var response = await _commentService.LikeReplyAsync(User.GetUserId()!, replyId, ct);
        return response.IsSuccess ? NoContent() : response.ToProblem();
    }
    [HttpGet("{commentId}/replies")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCommentReplies(string commentId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        return Ok(await _commentService.GetCommentRepliesAsync(commentId, User.GetUserId(), filters, ct));
    }
    [HttpDelete("replies/{replyId}/like")]
    public async Task<IActionResult> UnlikeReply(string replyId, CancellationToken ct)
    {
        var response = await _commentService.UnlikeReplyAsync(User.GetUserId()!, replyId, ct);
        return response.IsSuccess ? NoContent() : response.ToProblem();
    }
}