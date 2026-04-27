using NaturalSpaceApi.Application.DTOs;
using NaturalSpaceApi.Application.DTOs.Auth;

namespace NaturalSpaceApi.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponse> RegisterAsync(RegisterRequest registerRequest);

        Task<AuthResponse> LoginAsync(LoginRequest loginRequest);

        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);

        Task LogoutAsync(LogoutRequest logoutRequest);
    }
}
