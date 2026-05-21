namespace Career_Path.Controllers;

using Career_Path.Contracts.Common;
using Career_Path.Services.Abstraction;

[Route("api/Follow")]
[ApiController]
[Authorize]
public class FollowController(IFollowService followService) : ControllerBase
{
    private readonly IFollowService _followService = followService;

    [HttpPost("{followingId}")]
    public async Task<IActionResult> Follow(string followingId, CancellationToken ct)
    {
        var result = await _followService.FollowAsync(User.GetUserId()!, followingId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{followingId}")]
    public async Task<IActionResult> Unfollow(string followingId, CancellationToken ct)
    {
        var result = await _followService.UnfollowAsync(User.GetUserId()!, followingId, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpGet("{userId}/followers")]
    public async Task<IActionResult> GetFollowers(string userId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        return Ok(await _followService.GetFollowersAsync(userId, User.GetUserId(), filters, ct));
    }

    [HttpGet("{userId}/following")]
    public async Task<IActionResult> GetFollowing(string userId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        return Ok(await _followService.GetFollowingAsync(userId, User.GetUserId(), filters, ct));
    }

    [HttpGet("my/followers")]
    public async Task<IActionResult> GetMyFollowers([FromQuery] RequestFilters filters, CancellationToken ct)
    {
        return Ok(await _followService.GetMyFollowersAsync(User.GetUserId()!, filters, ct));
    }

    [HttpGet("my/following")]
    public async Task<IActionResult> GetMyFollowing([FromQuery] RequestFilters filters, CancellationToken ct)
    {
        return Ok(await _followService.GetMyFollowingAsync(User.GetUserId()!, filters, ct));
    }
}