namespace Career_Path.Contracts.Posts;

public sealed record CreatePostRequest(
    string Content,
    IFormFile? File
);
