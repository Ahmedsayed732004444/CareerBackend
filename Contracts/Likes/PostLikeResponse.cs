namespace Career_Path.Contracts.Likes;

public sealed record PostLikeResponse(
  string UserId,
  string FullName,
  string? ProfilePictureUrl,
  DateTime LikedAt
);
