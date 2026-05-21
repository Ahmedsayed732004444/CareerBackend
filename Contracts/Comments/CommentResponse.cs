namespace Career_Path.Contracts.Comments;

public record CommentResponse(
    string Id,
    string Content,
    DateTime CreatedAt,
    int LikesCount,
    bool IsLiked,
    CommentAuthorSummary Author
);
