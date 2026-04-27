using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaturalSpaceApi.Application.DTOs.Message;
using NaturalSpaceApi.Application.Interfaces;
using System.Security.Claims;

namespace NaturalSpaceApi.Controllers
{
    [Route("api/channels/{channelId}/messages")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
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
        public async Task<ActionResult<IEnumerable<MessageResponse>>> GetMessages(
            Guid channelId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = GetCurrentUserId();
            var messages = await _messageService.GetMessagesByChannelAsync(channelId, userId, pageNumber, pageSize);
            return Ok(messages);
        }

        [HttpPost]
        public async Task<ActionResult<MessageResponse>> SendMessage(
            Guid channelId,
            [FromForm] CreateMessageRequest request)
        {
            var userId = GetCurrentUserId();
            var message = await _messageService.SendMessageAsync(channelId, request, userId);
            return CreatedAtAction(nameof(GetMessages), new { channelId }, message);
        }

        [HttpPut("{messageId}")]
        public async Task<ActionResult<MessageResponse>> UpdateMessage(
            Guid messageId,
            [FromBody] UpdateMessageRequest request)
        {
            var userId = GetCurrentUserId();
            var message = await _messageService.UpdateMessageAsync(messageId, request, userId);
            return Ok(message);
        }

        [HttpDelete("{messageId}")]
        public async Task<ActionResult> DeleteMessage(Guid messageId)
        {
            var userId = GetCurrentUserId();
            await _messageService.DeleteMessageAsync(messageId, userId);
            return NoContent();
        }
    }
}
