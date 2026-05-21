using Career_Path.Contracts.Common;
using CareerPathFinal.Services;

namespace Career_Path.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoadmapController(IRoadmapService roadmapService) : ControllerBase
    {
        private readonly IRoadmapService _roadmapService = roadmapService;

        [HttpGet]
        public async Task<IActionResult> GetUserRoadmaps([FromQuery] RequestFilters filters, CancellationToken ct)
        {
            try
            {
                return Ok(await _roadmapService.GetUserRoadmapsAsync(User.GetUserId()!, filters, ct));
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpGet("saved")]
        public async Task<IActionResult> GetUserSavedRoadmaps([FromQuery] RequestFilters filters, CancellationToken ct)
        {
            try
            {
                return Ok(await _roadmapService.GetUserSavedRoadmapsAsync(User.GetUserId()!, filters, ct));
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoadmap(int id, CancellationToken ct)
        {
            try
            {
                var result = await _roadmapService.GetRoadmapAsync(id, ct);
                return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpPost("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id, CancellationToken ct)
        {
            try
            {
                var result = await _roadmapService.ToggleStatusAsync(id, ct);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }
    }
}