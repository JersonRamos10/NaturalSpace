using NaturalSpaceApi.Application.DTOs.Message;

namespace NaturalSpaceApi.Application.Interfaces
{
    public interface IMessageService
    {
        Task<MessageResponse> SendMessageAsync(Guid channelId, CreateMessageRequest request, Guid userId);
        Task<IEnumerable<MessageResponse>> GetMessagesByChannelAsync(Guid channelId, Guid userId, int pageNumber, int pageSize);
        Task<MessageResponse> UpdateMessageAsync(Guid messageId, UpdateMessageRequest request, Guid userId);
        Task DeleteMessageAsync(Guid messageId, Guid userId);
    }
}
