using NaturalSpaceApi.Application.DTOs.Channel;

namespace NaturalSpaceApi.Application.Interfaces
{
    public interface IChannelService
    {
        Task<ChannelResponse> CreateChannelAsync(CreateChannelRequest channelRequest, Guid workspaceId, Guid userId);

        Task DeleteChannelAsync(Guid channelId, Guid userId);

        Task<ChannelResponse> GetByIdAsync(Guid channelId, Guid userId);

        Task<IEnumerable<ChannelResponse>> GetChannelsByWorkspaceAsync(Guid workspaceId, Guid userId);

        Task<ChannelResponse> UpdateChannelAsync(Guid channelId, UpdateChannelRequest channelRequest, Guid userId);
    }
}
