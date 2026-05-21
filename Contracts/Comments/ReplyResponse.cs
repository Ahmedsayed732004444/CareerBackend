namespace Career_Path.Contracts.Comments;

public record ReplyResponse(
    string Id,
    string Content,
    DateTime CreatedAt,
    int LikesCount,
    bool IsLiked,
    CommentAuthorSummary Author
);