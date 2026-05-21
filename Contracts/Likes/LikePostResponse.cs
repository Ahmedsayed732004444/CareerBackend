namespace Career_Path.Contracts.Likes;

public sealed record LikePostResponse(
  string PostId,
  int LikesCount
);
