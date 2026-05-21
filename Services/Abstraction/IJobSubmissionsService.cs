using Career_Path.Contracts.Common;
using Career_Path.Contracts.JobSubmission;

namespace Career_Path.Services.Abstraction;

public interface IJobSubmissionsService
{
    Task<Result<PaginatedList<JobSubmissionResponse>>> GetJobSubmissionsAsync(string companyId, string jobId, RequestFilters filters, CancellationToken ct = default);
    Task<Result<JobSubmissionResponse>> GetSubmissionAsync(string companyId, string jobId, string submissionId, CancellationToken ct = default);
    Task<Result<PaginatedList<MySubmissionResponse>>> GetMySubmissionsAsync(string userId, RequestFilters filters, CancellationToken ct = default);
    Task<Result> SendNoteToApplicantAsync(string companyId, string jobId, string submissionId, SendNoteRequest request, CancellationToken ct = default);
    Task<Result> DeleteSubmissionAsync(string userId, string submissionId, CancellationToken ct = default);
}
