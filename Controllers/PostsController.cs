using Career_Path.Contracts.Common;
using Career_Path.Contracts.Posts;
using Career_Path.Services.Abstraction;

namespace Career_Path.Controllers;

[Route("api/Posts")]
[ApiController]
[Authorize]
public class PostsController(IPostService _postService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request, CancellationToken ct)
    {
        var response = await _postService.CreatePostAsync(User.GetUserId()!, request, ct);
        return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
    }
    [HttpDelete("{postId}")]
    public async Task<IActionResult> SoftDeletePost(string postId, CancellationToken ct)
    {
        var response = await _postService.SoftDeletePostAsync(User.GetUserId()!, postId, ct);
        return response.IsSuccess ? NoContent() : response.ToProblem();
    }

    [HttpPut("{postId}")]
    public async Task<IActionResult> UpdatePost(string postId, [FromBody] UpdatePostRequest request, CancellationToken ct)
    {
        var response = await _postService.UpdatePostAsync(User.GetUserId()!, postId, request, ct);
        return response.IsSuccess ? Created() : response.ToProblem();
    }

    [HttpGet("{postId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPost(string postId, CancellationToken ct)
    {
        var response = await _postService.GetPostAsync(postId, User.GetUserId(), ct);
        return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
    }
    [HttpGet("user")]
    public async Task<IActionResult> GetPostsByUser([FromQuery] RequestFilters filters, CancellationToken ct)
    {
        return Ok(await _postService.GetPostsByUserAsync(User.GetUserId()!, null, filters, ct));
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPosts([FromQuery] RequestFilters filters, CancellationToken ct)
    {
        return Ok(await _postService.GetPostsAsync(User.GetUserId(), filters, ct));
    }
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetPostsByUserId(string userId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        return Ok(await _postService.GetPostsByUserAsync(userId, User.GetUserId()!, filters, ct));
    }
}
