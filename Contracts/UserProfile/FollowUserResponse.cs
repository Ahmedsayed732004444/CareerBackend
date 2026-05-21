namespace Career_Path.Contracts.UserProfile;

public record FollowUserResponse(
 string UserId,
 string FullName,
 string? JobTitle,
 string? ProfilePictureUrl,
 string? Country,
 bool IsFollowedByMe
);
