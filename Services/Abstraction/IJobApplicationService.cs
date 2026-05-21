using Career_Path.Contracts.Common;
using Career_Path.Contracts.JobApplication;

namespace Career_Path.Services;

public interface IJobApplicationService
{
    Task<PaginatedList<JobApplicationRespons>> GetJobApplicationsUserAsync(string userId, RequestFilters filters, CancellationToken ct = default);
    Task<Result<JobApplicationRespons>> GetJobApplicationAsync(string userId, string applicationId, CancellationToken ct = default);
    Task<Result<JobApplicationRespons>> AddJobApplicationAsync(string userId, JobApplicationRequest request, CancellationToken ct = default);
    Task<Result> UpdateJobApplicationAsync(string userId, string applicationId, JobApplicationRequest request, CancellationToken ct = default);
    Task<Result> DeleteJobApplicationAsync(string userId, string applicationId, CancellationToken ct = default);
}