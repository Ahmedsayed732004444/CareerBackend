namespace Career_Path.Contracts.Authentication;

public record LoginRequest(
    string Email,
    string Password
);