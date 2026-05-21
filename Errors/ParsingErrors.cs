namespace Career_Path.Errors;

public record ParsingErrors
{
    public static readonly Error ParsingFailed =
        new("Parsing.Failed", "Failed to parse the CV. Please try again later.", StatusCodes.Status500InternalServerError);
}
