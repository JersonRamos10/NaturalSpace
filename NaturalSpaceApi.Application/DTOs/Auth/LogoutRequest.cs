using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalSpaceApi.Application.DTOs.Auth
{
    public sealed record LogoutRequest
    (
        string RefreshToken

    );
    
}
