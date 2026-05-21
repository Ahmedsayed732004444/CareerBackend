namespace Career_Path.Contracts.JobSubmission;

public record MySubmissionResponse(
 string Id,
 string JobId,
 string? JobTitle,
 string CompanyName,
 DateTime AppliedAt,
 string? Notes
);
