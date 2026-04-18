using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaturalSpaceApi.Application.DTOs.Member;
using NaturalSpaceApi.Application.DTOs.Auth;
using NaturalSpaceApi.Application.Interfaces;
using System.Security.Claims;

namespace NaturalSpaceApi.Controllers
{
    [Route("api/workspaces/{workspaceId}/members")]
    [ApiController]
    [Authorize]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return Guid.Parse(userIdClaim);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberResponse>>> GetMembers(Guid workspaceId)
        {
            var currentUserId = GetCurrentUserId();
            var members = await _memberService.GetMembersAsync(workspaceId, currentUserId);
            return Ok(members);
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<UserResponse>> AddMember(Guid workspaceId, Guid userId)
        {
            var currentUserId = GetCurrentUserId();
            var user = await _memberService.AddMemberAsync(workspaceId, userId, currentUserId);
            return Ok(user);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> RemoveMember(Guid workspaceId, Guid userId)
        {
            var currentUserId = GetCurrentUserId();
            await _memberService.RemoveMemberAsync(workspaceId, userId, currentUserId);
            return NoContent();
        }

        [HttpDelete("me")]
        public async Task<ActionResult> LeaveWorkspace(Guid workspaceId)
        {
            var currentUserId = GetCurrentUserId();
            await _memberService.RemoveMemberAsync(workspaceId, currentUserId, currentUserId);
            return NoContent();
        }
    }
}
