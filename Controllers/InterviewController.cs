using Career_Path.Contracts.Interview;

namespace Career_Path.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewController(IInterviewService interviewService) : ControllerBase
    {
        private readonly IInterviewService _interviewService = interviewService;

        [HttpGet("{jobId}/questions")]
        public async Task<IActionResult> GetInterviewQuestions(string jobId, CancellationToken ct)
        {
            try
            {
                var result = await _interviewService.GetInterviewQuestionsAsync(jobId, ct);
                return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpPost("{jobId}/submit")]
        public async Task<IActionResult> SubmitInterview(string jobId, SubmitInterviewRequest request, CancellationToken ct)
        {
            try
            {
                var result = await _interviewService.SubmitInterviewAsync(jobId, request, ct);
                return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }
    }
}


