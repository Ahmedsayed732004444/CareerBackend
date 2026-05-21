namespace Career_Path.Contracts.Job
{
    public record JobResponse
    (
        string Id,
        string? JobTitle,
        string? JobType,
        string? JobDescription,
        string? Location,
        IEnumerable<string>? JobRequirements,
        decimal? SalaryMin,
        decimal? SalaryMax,
        DateTime? PostedDate,
        DateTime? DeadlineDate,
        bool IsActive,
        bool IApplied,
        CompanyDetails CompanyDetails

     );
    public record CompanyDetails
    (
        string CompanyId,
        string? Name,
        string? ProfilePictureUrl
    );
}