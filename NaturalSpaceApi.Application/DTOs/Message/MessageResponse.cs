using System;
using System.Collections.Generic;

namespace NaturalSpaceApi.Application.DTOs.Message
{
    public sealed record MessageResponse
    (
        Guid Id,
        string? Content,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        Guid UserId,
        string UserName,
        string? UserAvatarUrl,
        Guid ChannelId,
        List<FileResponse> Attachments
    );
}
