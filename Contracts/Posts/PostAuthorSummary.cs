namespace Career_Path.Contracts.Posts;

public sealed record PostAuthorSummary(
    string UserId,
    string? FullName,
    string? JobTitle,
    string? ProfilePictureUrl,
    string? Country
);