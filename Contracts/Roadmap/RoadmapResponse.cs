
namespace Career_Path.Contracts.Roadmap;

public record RoadmapResponse(
    int Id,
    DateTime CreatedAt,
    bool IsSaved = false
);