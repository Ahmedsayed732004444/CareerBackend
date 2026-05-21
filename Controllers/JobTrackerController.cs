using Career_Path.Contracts.Common;
using Career_Path.Contracts.JobApplication;

namespace Career_Path.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobTrackerController(IJobApplicationService jobApplicationService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetJobApplications([FromQuery] RequestFilters filters, CancellationToken ct)
        {
            try
            {
                var result = await jobApplicationService.GetJobApplicationsUserAsync(User.GetUserId()!, filters, ct);
                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobApplication(string id, CancellationToken ct)
        {
            try
            {
                var result = await jobApplicationService.GetJobApplicationAsync(User.GetUserId()!, id, ct);
                return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddJobApplication(JobApplicationRequest request, CancellationToken ct)
        {
            try
            {
                var result = await jobApplicationService.AddJobApplicationAsync(User.GetUserId()!, request, ct);
                return result.IsSuccess ? Created($"/api/JobTracker/{result.Value.Id}", result.Value) : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobApplication(string id, JobApplicationRequest request, CancellationToken ct)
        {
            try
            {
                var result = await jobApplicationService.UpdateJobApplicationAsync(User.GetUserId()!, id, request, ct);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobApplication(string id, CancellationToken ct)
        {
            try
            {
                var result = await jobApplicationService.DeleteJobApplicationAsync(User.GetUserId()!, id, ct);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }
    }
}