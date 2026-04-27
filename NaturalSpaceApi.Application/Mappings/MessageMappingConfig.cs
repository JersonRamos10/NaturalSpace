using Mapster;
using NaturalSpaceApi.Application.DTOs.Message;
using NaturalSpaceApi.Domain.Entities;

namespace NaturalSpaceApi.Application.Mappings
{
    public class MessageMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Message, MessageResponse>()
                .Map(dest => dest.UserName, src => src.User.Name)
                .Map(dest => dest.UserAvatarUrl, src => src.User.AvatarUrl);

            config.NewConfig<Domain.Entities.File, FileResponse>()
                .Map(dest => dest.FileUrl, src => src.FilePath);
        }
    }
}
