using Career_Path.Contracts.UserProfile;
using CareerPathFinal.Services;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ResumeController(
    IUserProfileService userProfileService,
    IParsingService parsingService) : ControllerBase
{
    [HttpPost("update/analayse")]
    public async Task<IActionResult> AnalayseResume([FromForm] UpdateUserProfileCvRequest request, CancellationToken ct)
    {
        try
        {
            var userId = User.GetUserId()!;
            var updateResult = await userProfileService.UpdateCvAsync(userId, request, ct);
            if (updateResult.IsFailure)
                return updateResult.ToProblem();
            var analysisResult = await parsingService.GetExtractionAsync(userId, request.CvFile, ct);
            return analysisResult.IsSuccess ? Ok(analysisResult.Value) : analysisResult.ToProblem();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}