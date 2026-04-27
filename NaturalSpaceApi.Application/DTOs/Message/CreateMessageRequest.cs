using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace NaturalSpaceApi.Application.DTOs.Message
{
    public sealed record CreateMessageRequest
    (
        string? Content,
        List<IFormFile>? Attachments
    );
}
