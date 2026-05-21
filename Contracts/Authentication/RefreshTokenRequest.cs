namespace Career_Path.Contracts.Authentication;

public record RefreshTokenRequest(
    string Token,
    string RefreshToken
);