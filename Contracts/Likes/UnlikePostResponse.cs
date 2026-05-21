namespace Career_Path.Contracts.Likes;

public sealed record UnlikePostResponse(
    string PostId,
    int LikesCount
);
