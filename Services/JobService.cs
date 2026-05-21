using Career_Path.Contracts.Common;
using Career_Path.Contracts.Job;
using Career_Path.Contracts.JobApplication;
using Intelligent_Career_Advisor.Models;
using System.Data;
using System.Linq.Dynamic.Core;

namespace Career_Path.Services
{
    public class JobService(
        ApplicationDbContext context,
        IWebHostEnvironment env,
        IHttpContextAccessor accessor,
        IGenerateQuestionsService generateQuestionsService,
        ILogger<JobService> logger,
        IJobApplicationService jobApplicationService) : IJobService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IHttpContextAccessor _accessor = accessor;
        private readonly IWebHostEnvironment _env = env;
        private readonly IGenerateQuestionsService _generateQuestionsService = generateQuestionsService;
        private readonly ILogger<JobService> _logger = logger;
        private readonly IJobApplicationService _jobApplicationService = jobApplicationService;
        private IQueryable<Job> GetJobsWithCompany() =>
            _context.Jobs.AsNoTracking();

        public async Task<PaginatedList<JobResponse>> GetAllJobsAsync(string? userId, RequestFilters filters, CancellationToken ct = default)
        {
            try
            {
                var appliedJobIds = userId != null
                    ? await _context.JobSubmissions.Where(s => s.ApplicantId == userId).Select(s => s.JobId).ToHashSetAsync(ct)
                    : new HashSet<string>();

                var query = GetJobsWithCompany().Where(j => j.IsActive)
                    .ApplyFilters(filters, searchPredicate: x =>
                        (x.JobTitle ?? "").Contains(filters.SearchValue!) ||
                        (x.JobDescription ?? "").Contains(filters.SearchValue!))
                    .Select(j => new JobResponse(
                        j.Id,
                        j.JobTitle,
                        j.JobType,
                        j.JobDescription,
                        j.Location,
                        j.JobRequirements,
                        j.SalaryMin,
                        j.SalaryMax,
                        j.PostedDate,
                        j.DeadlineDate,
                        j.IsActive,
                        appliedJobIds.Contains(j.Id),
                        new CompanyDetails(j.CompanyId, j.Company.FullName, j.Company.UserProfile != null ? j.Company.UserProfile.ProfilePictureUrl ?? string.Empty : string.Empty)));

                return await query.ToPaginatedListAsync(filters, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all jobs for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Result<JobResponse>> GetJobAsync(string jobId, string? userId, CancellationToken ct)
        {
            try
            {
                var response = await GetJobsWithCompany().Where(j => j.Id == jobId)
                    .Select(j => new JobResponse(
                        j.Id, j.JobTitle, j.JobType, j.JobDescription, j.Location, j.JobRequirements,
                        j.SalaryMin, j.SalaryMax, j.PostedDate, j.DeadlineDate, j.IsActive,
                        j.JobSubmissions.Any(s => s.ApplicantId == userId),
                        new CompanyDetails(j.CompanyId, j.Company.FullName, j.Company.UserProfile!.ProfilePictureUrl ?? "")))
                    .FirstOrDefaultAsync(ct);

                if (response is null)
                    return Result.Failure<JobResponse>(JobErrors.JobNotFound);

                return Result.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving job {JobId}", jobId);
                return Result.Failure<JobResponse>(JobErrors.Error);
            }
        }

        public async Task<PaginatedList<JobResponse>> GetCompanyJobsAsync(string companyId, string? userId, RequestFilters filters, CancellationToken ct = default)
        {
            try
            {
                var query = GetJobsWithCompany()
                    .Where(j => j.CompanyId == companyId)
                    .ApplyFilters(filters, searchPredicate: x => (x.JobTitle ?? "").Contains(filters.SearchValue!))
                    .Select(j => new JobResponse(
                        j.Id, j.JobTitle, j.JobType, j.JobDescription, j.Location, j.JobRequirements,
                        j.SalaryMin, j.SalaryMax, j.PostedDate, j.DeadlineDate, j.IsActive,
                        j.JobSubmissions.Any(s => s.ApplicantId == userId),
                        new CompanyDetails(j.CompanyId, j.Company.FullName, j.Company.UserProfile!.ProfilePictureUrl ?? "")));

                return await query.ToPaginatedListAsync(filters, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving jobs for company {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<Result<PaginatedList<ApplyJobResponse>>> GetJobApplicantsAsync(string companyId, string jobId, RequestFilters filters, CancellationToken ct = default)
        {
            try
            {
                var jobExists = await _context.Jobs
                    .AnyAsync(j => j.CompanyId == companyId && j.Id == jobId, ct);

                if (!jobExists)
                    return Result.Failure<PaginatedList<ApplyJobResponse>>(JobErrors.JobNotFound);

                var query = _context.JobSubmissions
                    .Where(js => js.JobId == jobId)
                    .ApplyFilters(filters, searchPredicate: x => x.Notes != null && x.Notes.Contains(filters.SearchValue!))
                    .Select(js => new ApplyJobResponse(
                        js.Id,
                        js.ApplicantId,
                        js.CVPath,
                        js.Phone,
                        js.Notes,
                        js.AppliedAt,
                        js.ApplicationUser.FullName,
                        js.ApplicationUser.Email ?? string.Empty,
                        js.ApplicationUser.UserProfile != null ? js.ApplicationUser.UserProfile.ProfilePictureUrl ?? string.Empty : string.Empty))
                    .AsNoTracking();

                var submissions = await query.ToPaginatedListAsync(filters, ct);
                return Result.Success(submissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving applicants for job {JobId} in company {CompanyId}", jobId, companyId);
                return Result.Failure<PaginatedList<ApplyJobResponse>>(JobErrors.Error);
            }
        }

        public async Task<Result<JobResponse>> AddJobAsync(string companyId, JopRequest request, CancellationToken ct)
        {
            try
            {
                var job = request.Adapt<Job>();
                job.Id = Guid.CreateVersion7().ToString();
                job.CompanyId = companyId;

                await _context.Jobs.AddAsync(job, ct);
                await _context.SaveChangesAsync(ct);

                var response = await GetJobsWithCompany().Where(j => j.Id == job.Id)
                    .Select(j => new JobResponse(
                        j.Id, j.JobTitle, j.JobType, j.JobDescription, j.Location, j.JobRequirements,
                        j.SalaryMin, j.SalaryMax, j.PostedDate, j.DeadlineDate, j.IsActive, false,
                        new CompanyDetails(j.CompanyId, j.Company.FullName, j.Company.UserProfile != null ? j.Company.UserProfile.ProfilePictureUrl ?? string.Empty : string.Empty)))
                    .FirstOrDefaultAsync(ct);

                if (response is null)
                    return Result.Failure<JobResponse>(JobErrors.JobNotFound);

                BackgroundJob.Enqueue(() => _generateQuestionsService.GenerateQuestionsAsync(companyId, job.Id, ct));
                return Result.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding job for company {CompanyId}", companyId);
                return Result.Failure<JobResponse>(JobErrors.Error);
            }
        }

        public async Task<Result> UpdateJobAsync(string companyId, string jobId, JopRequest request, CancellationToken ct)
        {
            try
            {
                var job = await _context.Jobs
                    .FirstOrDefaultAsync(j => j.Id == jobId && j.CompanyId == companyId, ct);

                if (job is null)
                    return Result.Failure(JobErrors.JobNotFound);

                request.Adapt(job);
                await _context.SaveChangesAsync(ct);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating job {JobId} for company {CompanyId}", jobId, companyId);
                return Result.Failure(JobErrors.Error);
            }
        }

        public async Task<Result> DeleteJobAsync(string companyId, string jobId, CancellationToken ct)
        {
            try
            {
                var job = await _context.Jobs
                    .FirstOrDefaultAsync(j => j.Id == jobId && j.CompanyId == companyId, ct);

                if (job is null)
                    return Result.Failure(JobErrors.JobNotFound);

                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync(ct);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting job {JobId} for company {CompanyId}", jobId, companyId);
                return Result.Failure(JobErrors.Error);
            }
        }

        public async Task<Result> ToggleStatusAsync(string companyId, string jobId, CancellationToken ct)
        {
            try
            {
                var job = await _context.Jobs
                    .FirstOrDefaultAsync(j => j.Id == jobId && j.CompanyId == companyId, ct);

                if (job is null)
                    return Result.Failure(JobErrors.JobNotFound);

                job.IsActive = !job.IsActive;
                await _context.SaveChangesAsync(ct);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling status for job {JobId} in company {CompanyId}", jobId, companyId);
                return Result.Failure(JobErrors.Error);
            }
        }

        public async Task<Result> ApplyForJobAsync(string userId, string jobId, ApplyJobRequest request, CancellationToken ct = default)
        {
            try
            {
                var job = await _context.Jobs
                    .FirstOrDefaultAsync(j => j.Id == jobId, ct);

                if (job is null)
                    return Result.Failure(JobErrors.JobNotFound);

                if (!job.IsActive || (job.DeadlineDate.HasValue && job.DeadlineDate < DateTime.UtcNow))
                    return Result.Failure(JobErrors.JobClosed);

                var alreadyApplied = await _context.JobSubmissions
                    .AnyAsync(js => js.JobId == jobId && js.ApplicantId == userId, ct);

                if (alreadyApplied)
                    return Result.Failure(JobErrors.AlreadyApplied);

                var submission = request.Adapt<JobSubmission>();

                if (request.CV is not null)
                    submission.CVPath = await FileHelper.UploadeFileAsync(request.CV, "CvApllay", _env, _accessor);

                submission.ApplicantId = userId;
                submission.JobId = jobId;

                await _context.JobSubmissions.AddAsync(submission, ct);
                await _context.SaveChangesAsync(ct);
                BackgroundJob.Enqueue(() => _jobApplicationService.AddJobApplicationAsync(userId, new JobApplicationRequest(job.JobTitle ?? "New Job", job.Company.FullName, DateTime.UtcNow, ApplicationStatus.Applied, "Carrer Path", request.Notes), ct));
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while applying for job {JobId} by user {UserId}", jobId, userId);
                return Result.Failure(JobErrors.Error);
            }
        }

        public async Task<Result> GenerateJobQuestionsAsync(string companyId, string jobId, CancellationToken ct)
        {
            try
            {
                var job = await _context.Jobs
                    .FirstOrDefaultAsync(j => j.Id == jobId && j.CompanyId == companyId, ct);

                if (job is null)
                    return Result.Failure(JobErrors.JobNotFound);

                BackgroundJob.Enqueue(() => _generateQuestionsService.GenerateQuestionsAsync(companyId, job.Id, ct));
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating questions for job {JobId} in company {CompanyId}", jobId, companyId);
                return Result.Failure(JobErrors.Error);
            }
        }
    }
}