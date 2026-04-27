using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaturalSpaceApi.Application.DTOs.Workspace;
using NaturalSpaceApi.Application.Interfaces;
using System.Security.Claims;

namespace NaturalSpaceApi.Controllers
{
    [Route("api/workspaces")]
    [ApiController]
    [Authorize]
    public class WorkspaceController : ControllerBase
    {
        private readonly IWorkspaceService _workspaceService;

        public WorkspaceController(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
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
        public async Task<ActionResult<IEnumerable<WorkspaceResponse>>> GetMyWorkspaces()
        {
            var userId = GetCurrentUserId();
            var workspaces = await _workspaceService.GetWorkspacesByUserAsync(userId);
            return Ok(workspaces);
        }

        [HttpGet("{workspaceId}")]
        public async Task<ActionResult<WorkspaceResponse>> GetWorkspaceById(Guid workspaceId)
        {
            var workspace = await _workspaceService.GetByIdAsync(workspaceId);
            return Ok(workspace);
        }

        [HttpPost]
        public async Task<ActionResult<WorkspaceResponse>> CreateWorkspace([FromBody] CreateWorkSpaceRequest request)
        {
            var userId = GetCurrentUserId();
            var workspace = await _workspaceService.CreateWorkspaceAsync(request, userId);
            return CreatedAtAction(nameof(GetWorkspaceById), new { workspaceId = workspace.workspaceId }, workspace);
        }

        [HttpPut("{workspaceId}")]
        public async Task<ActionResult<WorkspaceResponse>> UpdateWorkspace(
            Guid workspaceId,
            [FromBody] UpdateWorkspaceRequest request)
        {
            var userId = GetCurrentUserId();
            var workspace = await _workspaceService.UpdateWorkspace(workspaceId, request, userId);
            return Ok(workspace);
        }

        [HttpDelete("{workspaceId}")]
        public async Task<ActionResult> DeleteWorkspace(Guid workspaceId)
        {
            var userId = GetCurrentUserId();
            await _workspaceService.DeleteWorkspaceAsync(workspaceId, userId);
            return NoContent();
        }
    }
}
