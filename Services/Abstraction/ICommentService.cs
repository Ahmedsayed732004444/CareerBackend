using Career_Path.Contracts.Comments;
using Career_Path.Contracts.Common;

namespace Career_Path.Services.Abstraction;

public interface ICommentService
{
    // Comment
    Task<Result<CommentResponse>> AddCommentAsync(string userId, string postId, AddCommentRequest request, CancellationToken ct = default);
    Task<Result> DeleteCommentAsync(string userId, string commentId, CancellationToken ct = default);
    Task<PaginatedList<CommentResponse>> GetPostCommentsAsync(string postId, string? userId, RequestFilters filters, CancellationToken ct = default);

    // Reply
    Task<Result<ReplyResponse>> AddReplyAsync(string userId, string commentId, AddReplyRequest request, CancellationToken ct = default);
    Task<Result> DeleteReplyAsync(string userId, string replyId, CancellationToken ct = default);
    Task<PaginatedList<ReplyResponse>> GetCommentRepliesAsync(string commentId, string? userId, RequestFilters filters, CancellationToken ct = default);

    // Reactions (Like)
    Task<Result> LikeCommentAsync(string userId, string commentId, CancellationToken ct = default);
    Task<Result> UnlikeCommentAsync(string userId, string commentId, CancellationToken ct = default);
    Task<Result> LikeReplyAsync(string userId, string replyId, CancellationToken ct = default);
    Task<Result> UnlikeReplyAsync(string userId, string replyId, CancellationToken ct = default);
}
