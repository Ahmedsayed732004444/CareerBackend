using Career_Path.Contracts.UserProfile;
using Career_Path.Contracts.Users;

namespace Career_Path.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController(IUserProfileService userProfileService, IUserService userService) : ControllerBase
    {
        private readonly IUserProfileService _userProfileService = userProfileService;
        private readonly IUserService _userService = userService;

        [HttpGet("has-resumes")]
        public async Task<IActionResult> HasResumes(CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _userProfileService.HasResumesAsync(User.GetUserId()!, cancellationToken));
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserProfile(CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.GetUserId()!;
                var result = await _userProfileService.GetAsync(userId, userId); // 🆕 currentUserId = نفسه
                return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfileById(string userId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userProfileService.GetAsync(userId, User.GetUserId()); // 🆕 بعت currentUserId
                return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpPut("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpPut("basic-Info")]
        public async Task<IActionResult> UpdateBasicInfo([FromBody] BasicInfoRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userProfileService.UpdateBasicInfoAsync(User.GetUserId()!, request, cancellationToken);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpPut("cv")]
        public async Task<IActionResult> UpdateCv([FromForm] UpdateUserProfileCvRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userProfileService.UpdateCvAsync(User.GetUserId()!, request, cancellationToken);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpPut("picture")]
        public async Task<IActionResult> UpdatePicture([FromForm] UpdateUserProfilePictureRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userProfileService.UpdatePictureAsync(User.GetUserId()!, request, cancellationToken);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpPut("cover-picture")]
        public async Task<IActionResult> UpdateCoverPicture([FromForm] UpdateUserProfileCoverRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userProfileService.UpdateCoverPictureAsync(User.GetUserId()!, request, cancellationToken);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpDelete("cv")]
        public async Task<IActionResult> DeleteCv(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userProfileService.DeleteCvAsync(User.GetUserId()!, cancellationToken);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpDelete("picture")]
        public async Task<IActionResult> DeletePicture(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userProfileService.DeletePictureAsync(User.GetUserId()!, cancellationToken);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpDelete("cover-picture")]
        public async Task<IActionResult> DeleteCoverPicture(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userProfileService.DeleteCoverPictureAsync(User.GetUserId()!, cancellationToken);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpGet("profile-picture")]
        public async Task<IActionResult> GetProfilePicture(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userProfileService.GetProfilePictureUrlAsync(User.GetUserId()!);
                return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }
    }
}