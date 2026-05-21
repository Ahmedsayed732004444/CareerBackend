using Career_Path.Authentication.Filters;
using Career_Path.Contracts.Common;
using Career_Path.Contracts.Job;

namespace CareerPathFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobsController(IJobService _jobService) : ControllerBase
    {
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetJobAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _jobService.GetJobAsync(id, User.GetUserId(), cancellationToken);
                return result.IsSuccess ? Ok(result) : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllJobsAsync([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _jobService.GetAllJobsAsync(User.GetUserId(), filters, cancellationToken);
                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }

        [HttpGet("company/{companyId}")]
        [HasPermission(Permissions.GetJobs)]
        public async Task<IActionResult> GetCompanyJobsAsync(string companyId, [FromQuery] RequestFilters filters, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _jobService.GetCompanyJobsAsync(companyId, User.GetUserId(), filters, cancellationToken);
                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }

        [HttpPost("")]
        [HasPermission(Permissions.AddJobs)]
        public async Task<IActionResult> AddJobAsync([FromBody] JopRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _jobService.AddJobAsync(User.GetUserId()!, request, cancellationToken);
                return result.IsSuccess
                    ? Created($"/api/Jobs/{result.Value.Id}", result.Value)
                    : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }

        [HttpPut("{jobId}")]
        [HasPermission(Permissions.UpdateJobs)]
        public async Task<IActionResult> UpdateJobAsync(string jobId, [FromBody] JopRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _jobService.UpdateJobAsync(User.GetUserId()!, jobId, request, cancellationToken);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }

        [HttpDelete("{jobId}")]
        [HasPermission(Permissions.DeleteJobs)]
        public async Task<IActionResult> DeleteJobAsync(string jobId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _jobService.DeleteJobAsync(User.GetUserId()!, jobId, cancellationToken);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }

        [HttpPut("{jobId}/toggle-status")]
        [HasPermission(Permissions.UpdateJobs)]
        public async Task<IActionResult> ToggleStatusAsync(string jobId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _jobService.ToggleStatusAsync(User.GetUserId()!, jobId, cancellationToken);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }

        [HttpPost("{jobId}/apply")]
        [Authorize]
        public async Task<IActionResult> ApplyToJobAsync(string jobId, [FromForm] ApplyJobRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _jobService.ApplyForJobAsync(User.GetUserId()!, jobId, request, cancellationToken);
                return result.IsSuccess ? Ok() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }

        [HttpGet("{jobId}/applicants")]
        [HasPermission(Permissions.GetJobApplicants)]
        public async Task<IActionResult> GetJobApplicantsAsync(string jobId, [FromQuery] RequestFilters filters, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _jobService.GetJobApplicantsAsync(User.GetUserId()!, jobId, filters, cancellationToken);
                return result.IsSuccess ? Ok(result) : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }
        [HttpPost("{jobId}/generate-questions")]
        [HasPermission(Permissions.UpdateJobs)]
        public async Task<IActionResult> GenerateJobQuestionsAsync(string jobId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _jobService.GenerateJobQuestionsAsync(User.GetUserId()!, jobId, cancellationToken);
                return result.IsSuccess ? Ok() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }

    }
}