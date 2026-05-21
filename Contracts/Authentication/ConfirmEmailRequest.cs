namespace Career_Path.Contracts.Authentication;

public record ConfirmEmailRequest(
    string UserId,
    string Code
);