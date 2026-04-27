using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaturalSpaceApi.Application.DTOs.Channel;
using NaturalSpaceApi.Application.Interfaces;
using System.Security.Claims;

namespace NaturalSpaceApi.Controllers
{
    [Route("api/workspaces/{workspaceId}/channels")]
    [ApiController]
    [Authorize]
    public class ChannelController : ControllerBase
    {
        private readonly IChannelService _channelService;

        public ChannelController(IChannelService channelService)
        {
            _channelService = channelService;
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
        public async Task<ActionResult<IEnumerable<ChannelResponse>>> GetChannelsByWorkspace(Guid workspaceId)
        {
            var userId = GetCurrentUserId();
            var channels = await _channelService.GetChannelsByWorkspaceAsync(workspaceId, userId);
            return Ok(channels);
        }

        [HttpGet("{channelId}")]
        public async Task<ActionResult<ChannelResponse>> GetChannelById(Guid channelId)
        {
            var userId = GetCurrentUserId();
            var channel = await _channelService.GetByIdAsync(channelId, userId);
            return Ok(channel);
        }

        [HttpPost]
        public async Task<ActionResult<ChannelResponse>> CreateChannel(
            Guid workspaceId,
            [FromBody] CreateChannelRequest request)
        {
            var userId = GetCurrentUserId();
            var channel = await _channelService.CreateChannelAsync(request, workspaceId, userId);
            return CreatedAtAction(nameof(GetChannelById),
                new { workspaceId, channelId = channel.Id },
                channel);
        }

        [HttpPut("{channelId}")]
        public async Task<ActionResult<ChannelResponse>> UpdateChannel(
            Guid channelId,
            [FromBody] UpdateChannelRequest request)
        {
            var userId = GetCurrentUserId();
            var channel = await _channelService.UpdateChannelAsync(channelId, request, userId);
            return Ok(channel);
        }

        [HttpDelete("{channelId}")]
        public async Task<ActionResult> DeleteChannel(Guid channelId)
        {
            var userId = GetCurrentUserId();
            await _channelService.DeleteChannelAsync(channelId, userId);
            return NoContent();
        }
    }
}
