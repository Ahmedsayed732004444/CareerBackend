namespace Career_Path.Services;

using Career_Path.Contracts.Likes;
using Career_Path.Services.Abstraction;

public class LikeService(ApplicationDbContext context) : ILikeService
{
    private readonly ApplicationDbContext _context = context;
    public async Task<Result<LikePostResponse>> AddLikeAsync(string userId, string postId, CancellationToken ct = default)
    {
        try
        {
            var post = await _context.Posts.Where(p => p.Id == postId && !p.IsDeleted)
                .Select(p => new { LikesCount = p.Likes.Count, AlreadyLiked = p.Likes.Any(l => l.UserId == userId) })
                .FirstOrDefaultAsync(ct);

            if (post is null)
                return Result.Failure<LikePostResponse>(PostErrors.PostNotFound);

            if (post.AlreadyLiked)
                return Result.Failure<LikePostResponse>(PostErrors.AlreadyLiked);

            await _context.PostLikes.AddAsync(new PostLike { PostId = postId, UserId = userId }, ct);
            await _context.SaveChangesAsync(ct);
            return Result.Success(new LikePostResponse(postId, post.LikesCount + 1));
        }
        catch (Exception)
        {
            return Result.Failure<LikePostResponse>(PostErrors.Error);
        }
    }

    public async Task<Result<UnlikePostResponse>> RemoveLikeAsync(string userId, string postId, CancellationToken ct = default)
    {
        try
        {
            var postLike = await _context.PostLikes
                .Where(pl => pl.PostId == postId && pl.UserId == userId)
                .Select(pl => new
                { Like = pl, PostExists = pl.Post != null && !pl.Post.IsDeleted, LikesCount = pl.Post!.Likes.Count() })
                .FirstOrDefaultAsync(ct);

            if (postLike is null || !postLike.PostExists)
                return Result.Failure<UnlikePostResponse>(PostErrors.PostNotFound);
            if (postLike.Like is null)
                return Result.Failure<UnlikePostResponse>(PostErrors.LikeNotFound);
            _context.PostLikes.Remove(postLike.Like);
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            return Result.Success(new UnlikePostResponse(postId, postLike.LikesCount - 1));
        }
        catch (Exception)
        {
            return Result.Failure<UnlikePostResponse>(PostErrors.Error);
        }
    }

    public async Task<List<PostLikeResponse>> GetLikesAsync(string postId, CancellationToken ct = default)
    {
        return await _context.PostLikes
            .Where(p => p.PostId == postId)
            .Select(p => new PostLikeResponse(p.UserId, p.User.FullName!, p.User.UserProfile!.ProfilePictureUrl, p.LikedAt))
            .ToListAsync(ct);
    }
}
