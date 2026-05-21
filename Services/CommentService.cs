namespace Career_Path.Services;

using Career_Path.Contracts.Comments;
using Career_Path.Contracts.Common;
using Career_Path.Services.Abstraction;

public class CommentService(
    ApplicationDbContext context,
    ILogger<CommentService> logger) : ICommentService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<CommentService> _logger = logger;

    // ──────────────────────────────────────────────
    //  Comment
    // ──────────────────────────────────────────────

    public async Task<Result<CommentResponse>> AddCommentAsync(string userId, string postId, AddCommentRequest request, CancellationToken ct = default)
    {
        try
        {
            var post = await _context.Posts
                .Where(p => p.Id == postId && !p.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (post is null)
                return Result.Failure<CommentResponse>(PostErrors.PostNotFound);

            var comment = new PostComment
            {
                PostId = postId,
                UserId = userId,
                Content = request.Content
            };

            await _context.PostComments.AddAsync(comment, ct);
            await _context.SaveChangesAsync(ct);

            var userProfile = await _context.UserProfiles
                .Where(p => p.UserId == userId)
                .Select(p => new
                {
                    p.ApplicationUser.FullName,
                    p.ProfilePictureUrl
                })
                .FirstOrDefaultAsync(ct);

            return Result.Success(new CommentResponse(
                comment.Id,
                comment.Content,
                comment.CreatedAt,
                0,
                false,
                new CommentAuthorSummary(
                    userId,
                    userProfile == null ? string.Empty : userProfile.FullName,
                    userProfile == null ? null : userProfile.ProfilePictureUrl)
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding comment to post {PostId} for user {UserId}", postId, userId);
            return Result.Failure<CommentResponse>(CommentErrors.Error);
        }
    }

    public async Task<Result> DeleteCommentAsync(string userId, string commentId, CancellationToken ct = default)
    {
        try
        {
            var comment = await _context.PostComments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId && !c.IsDeleted, ct);

            if (comment is null)
                return Result.Failure(CommentErrors.CommentNotFound);

            comment.IsDeleted = true;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting comment {CommentId} for user {UserId}", commentId, userId);
            return Result.Failure(CommentErrors.Error);
        }
    }

    public async Task<PaginatedList<CommentResponse>> GetPostCommentsAsync(string postId, string? userId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var query = _context.PostComments
                .Where(c => c.PostId == postId && !c.IsDeleted)
                .ApplyFilters(filters, searchPredicate: x =>
                    x.Content != null && x.Content.Contains(filters.SearchValue!))
                .Select(c => new CommentResponse(
                    c.Id,
                    c.Content,
                    c.CreatedAt,
                    c.Reactions.Count,
                    c.Reactions.Any(r => r.UserId == userId),
                    new CommentAuthorSummary(
                        c.UserId,
                        c.User.FullName,
                        c.User.UserProfile == null ? null : c.User.UserProfile.ProfilePictureUrl)
                ))
                .AsNoTracking();

            return await query.ToPaginatedListAsync(filters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving comments for post {PostId}", postId);
            throw;
        }
    }

    // ──────────────────────────────────────────────
    //  Reply
    // ──────────────────────────────────────────────

    public async Task<Result<ReplyResponse>> AddReplyAsync(string userId, string commentId, AddReplyRequest request, CancellationToken ct = default)
    {
        try
        {
            var comment = await _context.PostComments
                .Where(c => c.Id == commentId && !c.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (comment is null)
                return Result.Failure<ReplyResponse>(CommentErrors.CommentNotFound);

            var reply = new CommentReply
            {
                CommentId = commentId,
                UserId = userId,
                Content = request.Content
            };

            await _context.CommentReplies.AddAsync(reply, ct);
            await _context.SaveChangesAsync(ct);

            var userProfile = await _context.UserProfiles
                .Where(p => p.UserId == userId)
                .Select(p => new
                {
                    p.ApplicationUser.FullName,
                    p.ProfilePictureUrl
                })
                .FirstOrDefaultAsync(ct);

            return Result.Success(new ReplyResponse(
                reply.Id,
                reply.Content,
                reply.CreatedAt,
                0,
                false,
                new CommentAuthorSummary(
                    userId,
                    userProfile == null ? string.Empty : userProfile.FullName,
                    userProfile == null ? null : userProfile.ProfilePictureUrl)
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding reply to comment {CommentId} for user {UserId}", commentId, userId);
            return Result.Failure<ReplyResponse>(CommentErrors.Error);
        }
    }

    public async Task<Result> DeleteReplyAsync(string userId, string replyId, CancellationToken ct = default)
    {
        try
        {
            var reply = await _context.CommentReplies
                .FirstOrDefaultAsync(r => r.Id == replyId && r.UserId == userId && !r.IsDeleted, ct);

            if (reply is null)
                return Result.Failure(CommentErrors.ReplyNotFound);

            reply.IsDeleted = true;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting reply {ReplyId} for user {UserId}", replyId, userId);
            return Result.Failure(CommentErrors.Error);
        }
    }

    // ──────────────────────────────────────────────
    //  Comment Reactions
    // ──────────────────────────────────────────────

    public async Task<Result> LikeCommentAsync(string userId, string commentId, CancellationToken ct = default)
    {
        try
        {
            var comment = await _context.PostComments
                .Where(c => c.Id == commentId && !c.IsDeleted)
                .Select(c => new { AlreadyLiked = c.Reactions.Any(r => r.UserId == userId) })
                .FirstOrDefaultAsync(ct);

            if (comment is null)
                return Result.Failure(CommentErrors.CommentNotFound);

            if (comment.AlreadyLiked)
                return Result.Failure(CommentErrors.AlreadyLiked);

            await _context.CommentReactions.AddAsync(new CommentReaction { CommentId = commentId, UserId = userId }, ct);
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while liking comment {CommentId} for user {UserId}", commentId, userId);
            return Result.Failure(CommentErrors.Error);
        }
    }

    public async Task<Result> UnlikeCommentAsync(string userId, string commentId, CancellationToken ct = default)
    {
        try
        {
            var reaction = await _context.CommentReactions
                .FirstOrDefaultAsync(r => r.CommentId == commentId && r.UserId == userId, ct);

            if (reaction is null)
                return Result.Failure(CommentErrors.LikeNotFound);

            _context.CommentReactions.Remove(reaction);
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while unliking comment {CommentId} for user {UserId}", commentId, userId);
            return Result.Failure(CommentErrors.Error);
        }
    }

    // ──────────────────────────────────────────────
    //  Reply Reactions
    // ──────────────────────────────────────────────

    public async Task<Result> LikeReplyAsync(string userId, string replyId, CancellationToken ct = default)
    {
        try
        {
            var reply = await _context.CommentReplies
                .Where(r => r.Id == replyId && !r.IsDeleted)
                .Select(r => new { AlreadyLiked = r.Reactions.Any(re => re.UserId == userId) })
                .FirstOrDefaultAsync(ct);

            if (reply is null)
                return Result.Failure(CommentErrors.ReplyNotFound);

            if (reply.AlreadyLiked)
                return Result.Failure(CommentErrors.AlreadyLiked);

            await _context.ReplyReactions.AddAsync(new ReplyReaction { ReplyId = replyId, UserId = userId }, ct);
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while liking reply {ReplyId} for user {UserId}", replyId, userId);
            return Result.Failure(CommentErrors.Error);
        }
    }

    public async Task<Result> UnlikeReplyAsync(string userId, string replyId, CancellationToken ct = default)
    {
        try
        {
            var reaction = await _context.ReplyReactions
                .FirstOrDefaultAsync(r => r.ReplyId == replyId && r.UserId == userId, ct);

            if (reaction is null)
                return Result.Failure(CommentErrors.LikeNotFound);

            _context.ReplyReactions.Remove(reaction);
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while unliking reply {ReplyId} for user {UserId}", replyId, userId);
            return Result.Failure(CommentErrors.Error);
        }
    }
    public async Task<PaginatedList<ReplyResponse>> GetCommentRepliesAsync(string commentId, string? userId, RequestFilters filters, CancellationToken ct = default)
    {
        try
        {
            var query = _context.CommentReplies
                .Where(r => r.CommentId == commentId && !r.IsDeleted)
                .ApplyFilters(filters, searchPredicate: x =>
                    x.Content != null && x.Content.Contains(filters.SearchValue!))
                .Select(r => new ReplyResponse(
                    r.Id,
                    r.Content,
                    r.CreatedAt,
                    r.Reactions.Count,
                    r.Reactions.Any(re => re.UserId == userId),
                    new CommentAuthorSummary(
                        r.UserId,
                        r.User.FullName,
                        r.User.UserProfile == null ? null : r.User.UserProfile.ProfilePictureUrl)
                ))
                .AsNoTracking();

            return await query.ToPaginatedListAsync(filters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving replies for comment {CommentId}", commentId);
            throw;
        }
    }
}
