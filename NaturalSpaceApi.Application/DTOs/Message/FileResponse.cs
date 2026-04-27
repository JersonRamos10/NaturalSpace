using System;

namespace NaturalSpaceApi.Application.DTOs.Message
{
    public sealed record FileResponse
    (
        Guid Id,
        string FileName,
        string FileUrl,
        DateTime CreatedAt
    );
}
