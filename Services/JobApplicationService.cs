using Career_Path.Contracts.Common;
using Career_Path.Contracts.JobApplication;
using Intelligent_Career_Advisor.Models;

namespace Career_Path.Services;

public class JobApplicationService(
    ApplicationDbContext context,
    ILogger<JobApplicationService> logger) : IJobApplicationService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<JobApplicationService> _logger = logger;

    private IQueryable<JobApplication> GetUserApplicationsQuery(string userId) =>
        _context.JobApplications
            .AsNoTracking()
            .Where(ja => ja.UserId == userId);

    public async Task<PaginatedList<JobApplicationRespons>> GetJobApplicationsUserAsync(
        string userId,
        RequestFilters filters,
        CancellationToken ct = default)
    {
        try
        {
            var query = GetUserApplicationsQuery(userId)
                .ApplyFilters(filters, searchPredicate: x =>
                    (x.JobTitle ?? "").Contains(filters.SearchValue!) ||
                    (x.CompanyName ?? "").Contains(filters.SearchValue!))
                .Select(ja => new JobApplicationRespons(
                    ja.Id,
                    ja.JobTitle,
                    ja.CompanyName,
                    ja.ApplicationDate,
                    ja.Status,
                    ja.ApplicationSource,
                    ja.Notes
                ));

            return await query.ToPaginatedListAsync(filters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving job applications for user {UserId}", userId);
            throw;
        }
    }

    public async Task<Result<JobApplicationRespons>> GetJobApplicationAsync(
        string userId,
        string applicationId,
        CancellationToken ct = default)
    {
        try
        {
            var application = await GetUserApplicationsQuery(userId)
                .Where(ja => ja.Id == applicationId)
                .Select(ja => new JobApplicationRespons(
                    ja.Id,
                    ja.JobTitle,
                    ja.CompanyName,
                    ja.ApplicationDate,
                    ja.Status,
                    ja.ApplicationSource,
                    ja.Notes
                ))
                .FirstOrDefaultAsync(ct);

            if (application is null)
                return Result.Failure<JobApplicationRespons>(JobErrors.JobNotFound);

            return Result.Success(application);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving job application {ApplicationId} for user {UserId}", applicationId, userId);
            return Result.Failure<JobApplicationRespons>(JobErrors.Error);
        }
    }

    public async Task<Result<JobApplicationRespons>> AddJobApplicationAsync(
        string userId,
        JobApplicationRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var newApplication = request.Adapt<JobApplication>();
            newApplication.UserId = userId;

            await _context.JobApplications.AddAsync(newApplication, ct);
            await _context.SaveChangesAsync(ct);

            return Result.Success(newApplication.Adapt<JobApplicationRespons>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding job application for user {UserId}", userId);
            return Result.Failure<JobApplicationRespons>(JobErrors.Error);
        }
    }

    public async Task<Result> UpdateJobApplicationAsync(
        string userId,
        string applicationId,
        JobApplicationRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var existingApplication = await _context.JobApplications
                .FirstOrDefaultAsync(ja => ja.Id == applicationId && ja.UserId == userId, ct);

            if (existingApplication is null)
                return Result.Failure(JobErrors.JobNotFound);

            request.Adapt(existingApplication);
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating job application {ApplicationId} for user {UserId}", applicationId, userId);
            return Result.Failure(JobErrors.Error);
        }
    }

    public async Task<Result> DeleteJobApplicationAsync(
        string userId,
        string applicationId,
        CancellationToken ct = default)
    {
        try
        {
            var existingApplication = await _context.JobApplications
                .FirstOrDefaultAsync(ja => ja.Id == applicationId && ja.UserId == userId, ct);

            if (existingApplication is null)
                return Result.Failure(JobErrors.JobNotFound);

            _context.JobApplications.Remove(existingApplication);
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting job application {ApplicationId} for user {UserId}", applicationId, userId);
            return Result.Failure(JobErrors.Error);
        }
    }
}