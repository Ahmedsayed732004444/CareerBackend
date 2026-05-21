using Career_Path.Contracts.Common;
using Career_Path.Contracts.JobSubmission;
using Career_Path.Services.Abstraction;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Career_Path.Services
{
    public class JobSubmissionsService(
        ApplicationDbContext context,
        ILogger<JobSubmissionsService> logger,
        IEmailSender emailSender) : IJobSubmissionsService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<JobSubmissionsService> _logger = logger;
        private readonly IEmailSender _emailSender = emailSender;

        // Get all submissions for a specific job (company view)
        public async Task<Result<PaginatedList<JobSubmissionResponse>>> GetJobSubmissionsAsync(
            string companyId,
            string jobId,
            RequestFilters filters,
            CancellationToken ct = default)
        {
            try
            {
                var jobExists = await _context.Jobs
                    .AnyAsync(j => j.Id == jobId && j.CompanyId == companyId, ct);

                if (!jobExists)
                    return Result.Failure<PaginatedList<JobSubmissionResponse>>(JobErrors.JobNotFound);

                var query = _context.JobSubmissions
                    .Where(js => js.JobId == jobId)
                    .ApplyFilters(filters, searchPredicate: x =>
                        (x.ApplicationUser.FirstName + " " + x.ApplicationUser.LastName).Contains(filters.SearchValue!) ||
                        (x.Notes ?? "").Contains(filters.SearchValue!))
                    .Select(js => new JobSubmissionResponse(
                        js.Id,
                        js.ApplicantId,
                        js.ApplicationUser.FullName,
                        js.ApplicationUser.Email ?? string.Empty,
                        js.ApplicationUser.UserProfile != null
                            ? js.ApplicationUser.UserProfile.ProfilePictureUrl ?? string.Empty
                            : string.Empty,
                        js.CVPath,
                        js.Phone,
                        js.Notes,
                        js.AppliedAt))
                    .AsNoTracking();

                var submissions = await query.ToPaginatedListAsync(filters, ct);
                return Result.Success(submissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving submissions for job {JobId} in company {CompanyId}", jobId, companyId);
                return Result.Failure<PaginatedList<JobSubmissionResponse>>(JobErrors.Error);
            }
        }

        // Get a single submission by ID (company view)
        public async Task<Result<JobSubmissionResponse>> GetSubmissionAsync(
            string companyId,
            string jobId,
            string submissionId,
            CancellationToken ct = default)
        {
            try
            {
                var submission = await _context.JobSubmissions
                    .Where(js => js.Id == submissionId && js.JobId == jobId && js.Job.CompanyId == companyId)
                    .Select(js => new JobSubmissionResponse(
                        js.Id,
                        js.ApplicantId,
                        js.ApplicationUser.FullName,
                        js.ApplicationUser.Email ?? string.Empty,
                        js.ApplicationUser.UserProfile != null
                            ? js.ApplicationUser.UserProfile.ProfilePictureUrl ?? string.Empty
                            : string.Empty,
                        js.CVPath,
                        js.Phone,
                        js.Notes,
                        js.AppliedAt))
                    .AsNoTracking()
                    .FirstOrDefaultAsync(ct);

                if (submission is null)
                    return Result.Failure<JobSubmissionResponse>(JobErrors.SubmissionNotFound);

                return Result.Success(submission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving submission {SubmissionId}", submissionId);
                return Result.Failure<JobSubmissionResponse>(JobErrors.Error);
            }
        }

        // Get all submissions for the authenticated user (applicant view)
        public async Task<Result<PaginatedList<MySubmissionResponse>>> GetMySubmissionsAsync(
            string userId,
            RequestFilters filters,
            CancellationToken ct = default)
        {
            try
            {
                var query = _context.JobSubmissions
                    .Where(js => js.ApplicantId == userId)
                    .ApplyFilters(filters, searchPredicate: x =>
                        (x.Job.JobTitle ?? "").Contains(filters.SearchValue!) ||
                        (x.Job.Company.FullName).Contains(filters.SearchValue!))
                    .Select(js => new MySubmissionResponse(
                        js.Id,
                        js.JobId,
                        js.Job.JobTitle,
                        js.Job.Company.FullName,
                        js.AppliedAt,
                        js.Notes))
                    .AsNoTracking();

                var submissions = await query.ToPaginatedListAsync(filters, ct);
                return Result.Success(submissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving submissions for user {UserId}", userId);
                return Result.Failure<PaginatedList<MySubmissionResponse>>(JobErrors.Error);
            }
        }

        // Company sends a note/feedback to the applicant via email
        public async Task<Result> SendNoteToApplicantAsync(
            string companyId,
            string jobId,
            string submissionId,
            SendNoteRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var submission = await _context.JobSubmissions
                    .Include(js => js.ApplicationUser)
                    .Include(js => js.Job)
                    .FirstOrDefaultAsync(js =>
                        js.Id == submissionId &&
                        js.JobId == jobId &&
                        js.Job.CompanyId == companyId, ct);

                if (submission is null)
                    return Result.Failure(JobErrors.SubmissionNotFound);

                var applicantEmail = submission.ApplicationUser.Email;
                if (string.IsNullOrWhiteSpace(applicantEmail))
                    return Result.Failure(UserErrors.EmailNotFound);

                var emailBody = BuildNoteEmailBody(
                    submission.ApplicationUser.FirstName,
                    submission.Job.JobTitle ?? "the position",
                    submission.Job.Company.FullName,
                    request.Note);

                BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(
                    applicantEmail,
                    $"📩 Career Path: Message from {submission.Job.Company.FullName}",
                    emailBody));

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending note to applicant for submission {SubmissionId}", submissionId);
                return Result.Failure(JobErrors.Error);
            }
        }

        // Delete a submission (applicant withdraws application)
        public async Task<Result> DeleteSubmissionAsync(
            string userId,
            string submissionId,
            CancellationToken ct = default)
        {
            try
            {
                var submission = await _context.JobSubmissions
                    .FirstOrDefaultAsync(js => js.Id == submissionId && js.ApplicantId == userId, ct);

                if (submission is null)
                    return Result.Failure(JobErrors.SubmissionNotFound);

                _context.JobSubmissions.Remove(submission);
                await _context.SaveChangesAsync(ct);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting submission {SubmissionId} for user {UserId}", submissionId, userId);
                return Result.Failure(JobErrors.Error);
            }
        }

        private static string BuildNoteEmailBody(
            string applicantName,
            string jobTitle,
            string companyName,
            string note) => $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 24px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                <h2 style='color: #2c3e50;'>📩 New Message from {companyName}</h2>
                <p>Dear <strong>{applicantName}</strong>,</p>
                <p>You have received a message from <strong>{companyName}</strong> regarding your application for the position of <strong>{jobTitle}</strong>.</p>
                <div style='background-color: #f9f9f9; border-left: 4px solid #3498db; padding: 16px; margin: 16px 0; border-radius: 4px;'>
                    <p style='margin: 0; color: #2c3e50;'>{note}</p>
                </div>
                <p>If you have any questions, feel free to reply to this email.</p>
                <p style='color: #7f8c8d; font-size: 13px;'>Best regards,<br/><strong>Career Path Team</strong></p>
            </div>";
    }
}