using Career_Path.Contracts.Common;
using Career_Path.Contracts.Job;

namespace Career_Path.Services
{
    public interface IJobService
    {
        // Queries
        Task<PaginatedList<JobResponse>> GetAllJobsAsync(string? userId, RequestFilters filters, CancellationToken ct = default);
        Task<Result<JobResponse>> GetJobAsync(string jobId, string? userId, CancellationToken ct);
        Task<PaginatedList<JobResponse>> GetCompanyJobsAsync(string companyId, string? userId, RequestFilters filters, CancellationToken ct = default);
        Task<Result<PaginatedList<ApplyJobResponse>>> GetJobApplicantsAsync(string companyId, string jobId, RequestFilters filters, CancellationToken ct = default);

        // Commands
        Task<Result<JobResponse>> AddJobAsync(string companyId, JopRequest request, CancellationToken ct);
        Task<Result> UpdateJobAsync(string companyId, string jobId, JopRequest request, CancellationToken ct);
        Task<Result> DeleteJobAsync(string companyId, string jobId, CancellationToken ct);
        Task<Result> ToggleStatusAsync(string companyId, string jobId, CancellationToken ct);
        Task<Result> ApplyForJobAsync(string userId, string jobId, ApplyJobRequest request, CancellationToken ct = default);
        Task<Result> GenerateJobQuestionsAsync(string companyId, string jobId, CancellationToken ct);
    }
}
