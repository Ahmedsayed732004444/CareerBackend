namespace Career_Path.Contracts.JobSubmission;

public record JobSubmissionResponse(
  string Id,
  string ApplicantId,
  string FullName,
  string Email,
  string ProfilePictureUrl,
  string? CVPath,
  string? Phone,
  string? Notes,
  DateTime AppliedAt
);
