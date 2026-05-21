using Career_Path.Contracts.Common;
using Career_Path.Contracts.UserProfile;

namespace Career_Path.Services.Abstraction;

public interface IFollowService
{
    Task<Result> FollowAsync(string followerId, string followingId, CancellationToken ct = default);
    Task<Result> UnfollowAsync(string followerId, string followingId, CancellationToken ct = default);
    Task<PaginatedList<FollowUserResponse>> GetMyFollowersAsync(string currentUserId, RequestFilters filters, CancellationToken ct = default);
    Task<PaginatedList<FollowUserResponse>> GetMyFollowingAsync(string currentUserId, RequestFilters filters, CancellationToken ct = default);
    Task<PaginatedList<FollowUserResponse>> GetFollowersAsync(string userId, string? currentUserId, RequestFilters filters, CancellationToken ct = default);
    Task<PaginatedList<FollowUserResponse>> GetFollowingAsync(string userId, string? currentUserId, RequestFilters filters, CancellationToken ct = default);
}
