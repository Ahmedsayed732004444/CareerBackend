using CareerPathFinal.Services;
namespace Career_Path.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController(IMatchService _matchService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetMatchAsync(CancellationToken cancellationToken)
        {
            var result = await _matchService.GetMatchAsync(User.GetUserId()!, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
    }

}
