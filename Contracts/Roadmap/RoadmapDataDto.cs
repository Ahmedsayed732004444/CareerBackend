namespace Career_Path.Contracts.Roadmap;

public record RoadmapDataDto(
    string RoadmapTitle,
    string RoadmapType,
    int DurationWeeks,
    List<ModuleDto> Modules,
    bool GenerationFailed
);

public record ModuleDto(
    int Week,
    string Title,
    string Description,
    List<string> SkillsCovered,
    List<ResourceDto> Resources,
    bool Project
);

public record ResourceDto(
    string Type,
    string Url
);