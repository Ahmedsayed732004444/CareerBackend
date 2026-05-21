using Career_Path.Contracts.Common;
using Career_Path.Contracts.Posts;

namespace Career_Path.Services.Abstraction;

public interface IPostService
{
    Task<Result<PostResponse>> CreatePostAsync(string userId, CreatePostRequest request, CancellationToken ct = default);
    Task<Result> SoftDeletePostAsync(string userId, string postId, CancellationToken ct = default);
    Task<Result> UpdatePostAsync(string userId, string postId, UpdatePostRequest request, CancellationToken ct = default);
    Task<Result<PostResponse>> GetPostAsync(string postId, string? userId = null, CancellationToken ct = default);
    Task<PaginatedList<PostResponse>> GetPostsByUserAsync(string userId, string? userSearchId, RequestFilters filters, CancellationToken ct = default);
    Task<PaginatedList<PostResponse>> GetPostsAsync(string? userId, RequestFilters filters, CancellationToken ct = default);
}
