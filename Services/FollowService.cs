namespace Career_Path.Services;

using Career_Path.Contracts.Common;
using Career_Path.Contracts.UserProfile;
using Career_Path.Services.Abstraction;

public class FollowService(
    ApplicationDbContext context,
    ILogger<FollowService> logger) : IFollowService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<FollowService> _logger = logger;

    public async Task<Result> FollowAsync(string followerId, string followingId, CancellationToken ct = default)
    {
        try
        {
            if (followerId == followingId)
                return Result.Failure(FollowErrors.CannotFollowYourself);

            var targetUser = await _context.Users
                .AnyAsync(u => u.Id == followingId, ct);

            if (!targetUser)
                return Result.Failure(UserErrors.NotFound);

            var alreadyFollowing = await _context.UserFollows
                .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId, ct);

            if (alreadyFollowing)
                return Result.Failure(FollowErrors.AlreadyFollowing);

            await _context.UserFollows.AddAsync(new UserFollow
            {
                FollowerId = followerId,
                FollowingId = followingId
            }, ct);

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while user {FollowerId} trying to follow {FollowingId}", followerId, followingId);
            return Result.Failure(FollowErrors.Error);
        }
    }

    public async Task<Result> UnfollowAsync(string followerId, string followingId, CancellationToken ct = default)
    {
        try
        {
            var follow = await _context.UserFollows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId, ct);

            if (follow is null)
                return Result.Failure(FollowErrors.NotFollowing);

            _context.UserFollows.Remove(follow);
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while user {FollowerId} trying to unfollow {FollowingId}", followerId, followingId);
            return Result.Failure(FollowErrors.Error);
        }
    }
    public async Task<PaginatedList<FollowUserResponse>> GetMyFollowersAsync(string currentUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var query = _context.UserFollows
                .Where(f => f.FollowingId == currentUserId)
                .ApplyFilters(filters, searchPredicate: x =>
                    x.Follower.FullName.Contains(filters.SearchValue!))
                .Select(f => new FollowUserResponse(
                    f.FollowerId,
                    f.Follower.FullName,
                    f.Follower.UserProfile == null ? null : f.Follower.UserProfile.JobTitle,
                    f.Follower.UserProfile == null ? null : f.Follower.UserProfile.ProfilePictureUrl,
                    f.Follower.UserProfile == null ? null : f.Follower.UserProfile.Country,
                    f.Follower.Followers.Any(x => x.FollowerId == currentUserId)
                ))
                .AsNoTracking();

            return await query.ToPaginatedListAsync(filters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving my followers for user {UserId}", currentUserId);
            throw;
        }
    }

    public async Task<PaginatedList<FollowUserResponse>> GetMyFollowingAsync(string currentUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var query = _context.UserFollows
                .Where(f => f.FollowerId == currentUserId)
                .ApplyFilters(filters, searchPredicate: x =>
                    x.Following.FullName.Contains(filters.SearchValue!))
                .Select(f => new FollowUserResponse(
                    f.FollowingId,
                    f.Following.FullName,
                    f.Following.UserProfile == null ? null : f.Following.UserProfile.JobTitle,
                    f.Following.UserProfile == null ? null : f.Following.UserProfile.ProfilePictureUrl,
                    f.Following.UserProfile == null ? null : f.Following.UserProfile.Country,
                    f.Following.Followers.Any(x => x.FollowerId == currentUserId)
                ))
                .AsNoTracking();

            return await query.ToPaginatedListAsync(filters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving my following for user {UserId}", currentUserId);
            throw;
        }
    }
    public async Task<PaginatedList<FollowUserResponse>> GetFollowersAsync(string userId, string? currentUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var query = _context.UserFollows
                .Where(f => f.FollowingId == userId)
                .ApplyFilters(filters, searchPredicate: x =>
                    x.Follower.FullName.Contains(filters.SearchValue!))
                .Select(f => new FollowUserResponse(
                    f.FollowerId,
                    f.Follower.FullName,
                    f.Follower.UserProfile == null ? null : f.Follower.UserProfile.JobTitle,
                    f.Follower.UserProfile == null ? null : f.Follower.UserProfile.ProfilePictureUrl,
                    f.Follower.UserProfile == null ? null : f.Follower.UserProfile.Country,
                    f.Follower.Followers.Any(x => x.FollowerId == currentUserId)
                ))
                .AsNoTracking();

            return await query.ToPaginatedListAsync(filters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving followers for user {UserId}", userId);
            throw;
        }
    }

    public async Task<PaginatedList<FollowUserResponse>> GetFollowingAsync(string userId, string? currentUserId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var query = _context.UserFollows
                .Where(f => f.FollowerId == userId)
                .ApplyFilters(filters, searchPredicate: x =>
                    x.Following.FullName.Contains(filters.SearchValue!))
                .Select(f => new FollowUserResponse(
                    f.FollowingId,
                    f.Following.FullName,
                    f.Following.UserProfile == null ? null : f.Following.UserProfile.JobTitle,
                    f.Following.UserProfile == null ? null : f.Following.UserProfile.ProfilePictureUrl,
                    f.Following.UserProfile == null ? null : f.Following.UserProfile.Country,
                    f.Following.Followers.Any(x => x.FollowerId == currentUserId)
                ))
                .AsNoTracking();

            return await query.ToPaginatedListAsync(filters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving following for user {UserId}", userId);
            throw;
        }
    }
}