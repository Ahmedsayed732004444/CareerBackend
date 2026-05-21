namespace Career_Path.Errors;

public record RoadmapErrors
{
    public static readonly Error NotFound =
                new("RoadmapNotFound", "The requested roadmap was not found.", StatusCodes.Status404NotFound);
    public static readonly Error Error =
        new("RoadmapError", "An error occurred while processing the roadmap.", StatusCodes.Status500InternalServerError);
}
