using Career_Path.Contracts.Likes;

namespace Career_Path.Services.Abstraction;

public interface ILikeService
{
    Task<Result<LikePostResponse>> AddLikeAsync(string userId, string postId, CancellationToken ct = default);


    Task<Result<UnlikePostResponse>> RemoveLikeAsync(string userId, string postId, CancellationToken ct = default);


    Task<List<PostLikeResponse>> GetLikesAsync(string postId, CancellationToken ct = default);
}
