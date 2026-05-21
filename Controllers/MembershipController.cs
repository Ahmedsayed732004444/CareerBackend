using Career_Path.Authentication.Filters;
using Career_Path.Contracts.Common;
using Career_Path.Contracts.Users;

namespace Career_Path.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MembershipController(IMembershipUpgradeService membershipUpgradeService) : ControllerBase
    {
        private readonly IMembershipUpgradeService _membershipUpgradeService = membershipUpgradeService;

        [HttpPost("request")]
        public async Task<IActionResult> RequestUpgrade(MembershipUpgradeRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _membershipUpgradeService.RequestMembershipUpgradeAsync(User.GetUserId()!, request, cancellationToken);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpGet("requests")]
        [HasPermission(Permissions.GetMembershipUpgradeRequests)]
        public async Task<IActionResult> GetUpgradeRequests([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _membershipUpgradeService.GetMembershipUpgradeStatusAsync(filters, cancellationToken);
                return result.IsSuccess ? Ok(result) : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpPut("requests/{requestId}/approve")]
        [HasPermission(Permissions.ApproveMembershipUpgradeRequests)]
        public async Task<IActionResult> ApproveUpgradeRequest(string requestId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _membershipUpgradeService.ApproveRequestAsync(requestId, cancellationToken);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }

        [HttpPut("requests/{requestId}/reject")]
        [HasPermission(Permissions.RejectMembershipUpgradeRequests)]
        public async Task<IActionResult> RejectUpgradeRequest(string requestId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _membershipUpgradeService.RejectedRequestAsync(requestId, cancellationToken);
                return result.IsSuccess ? NoContent() : result.ToProblem();
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Request was cancelled by the client.");
            }
        }
    }
}