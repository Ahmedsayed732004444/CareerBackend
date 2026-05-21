// Controllers/JobSubmissionsController.cs
using Career_Path.Contracts.Common;
using Career_Path.Contracts.JobSubmission;
using Career_Path.Services.Abstraction;

namespace Career_Path.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class JobSubmissionsController(IJobSubmissionsService jobSubmissionsService) : ControllerBase
{
    private readonly IJobSubmissionsService _jobSubmissionsService = jobSubmissionsService;

    // Company: get all applicants for a job
    [HttpGet("companies/{companyId}/jobs/{jobId}")]
    //[HasPermission(Permissions.GetJobSubmissions)]
    public async Task<IActionResult> GetJobSubmissions(string companyId, string jobId, [FromQuery] RequestFilters filters, CancellationToken ct)
    {
        try
        {
            var result = await _jobSubmissionsService.GetJobSubmissionsAsync(companyId, jobId, filters, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, "Request was cancelled by the client.");
        }
    }

    // Company: get a single applicant submission
    [HttpGet("companies/{companyId}/jobs/{jobId}/submissions/{submissionId}")]
    // [HasPermission(Permissions.GetJobSubmissions)]
    public async Task<IActionResult> GetSubmission(string companyId, string jobId, string submissionId, CancellationToken ct)
    {
        try
        {
            var result = await _jobSubmissionsService.GetSubmissionAsync(companyId, jobId, submissionId, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, "Request was cancelled by the client.");
        }
    }

    // Company: send a note/feedback to an applicant via email
    [HttpPost("companies/{companyId}/jobs/{jobId}/submissions/{submissionId}/send-note")]
    //[HasPermission(Permissions.SendNoteToApplicant)]
    public async Task<IActionResult> SendNoteToApplicant(string companyId, string jobId, string submissionId, [FromBody] SendNoteRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _jobSubmissionsService.SendNoteToApplicantAsync(companyId, jobId, submissionId, request, ct);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, "Request was cancelled by the client.");
        }
    }

    // Applicant: view my submissions
    [HttpGet("my")]
    public async Task<IActionResult> GetMySubmissions([FromQuery] RequestFilters filters, CancellationToken ct)
    {
        try
        {
            var result = await _jobSubmissionsService.GetMySubmissionsAsync(User.GetUserId()!, filters, ct);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, "Request was cancelled by the client.");
        }
    }

    // Applicant: withdraw application
    [HttpDelete("{submissionId}")]
    public async Task<IActionResult> DeleteSubmission(string submissionId, CancellationToken ct)
    {
        try
        {
            var result = await _jobSubmissionsService.DeleteSubmissionAsync(User.GetUserId()!, submissionId, ct);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, "Request was cancelled by the client.");
        }
    }
}