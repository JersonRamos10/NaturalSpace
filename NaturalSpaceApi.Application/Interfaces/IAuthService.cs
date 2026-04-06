using NaturalSpaceApi.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalSpaceApi.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponse> RegisterAsync (CreateUserRequest userRequest);
    }
}
