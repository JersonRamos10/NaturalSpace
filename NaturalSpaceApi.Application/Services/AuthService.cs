using FluentValidation;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using NaturalSpaceApi.Application.DTOs;
using NaturalSpaceApi.Application.DTOs.Auth;
using NaturalSpaceApi.Application.Interfaces;
using NaturalSpaceApi.Domain.Entities;
using NaturalSpaceApi.Application.Exceptions;
using NaturalSpaceApi.Infrastructure.Data.Context;

using Mapster;

namespace NaturalSpaceApi.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly NaturalSpaceContext _context;
        private readonly IValidator<RegisterRequest> _validator;
        private readonly ITokenService _tokenService;

        public AuthService(
            NaturalSpaceContext context,
            IValidator<RegisterRequest> validator,
            ITokenService token)
        {
            _context = context;
            _validator = validator;
            _tokenService = token;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _context.Users
                .Where(u => u.Email == loginRequest.Email || u.UserName == loginRequest.UserName)
                .FirstOrDefaultAsync();

            if (user == null)
                throw new UnauthorizedException("User not found, email or username incorrect");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash);

            if (!isPasswordValid)
                throw new UnauthorizedException("Invalid credentials");

            var token = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshTokenAsync();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresDate = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponse(
                Token: token,
                Expiration: DateTime.UtcNow.AddHours(1),
                RefreshToken: refreshToken,
                RefreshTokenExpiration: DateTime.UtcNow.AddDays(7)
            );
        }

        public async Task<UserResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            var validationResult = _validator.Validate(registerRequest);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var emailExist = await _context.Users.AnyAsync(u => u.Email == registerRequest.Email);
            if (emailExist)
                throw new ConflictException("Email already in use");

            var userExist = await _context.Users.AnyAsync(u => u.UserName == registerRequest.UserName);
            if (userExist)
                throw new ConflictException($"Username [{registerRequest.UserName}] is already in use");

            var user = createUser(registerRequest);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user.Adapt<UserResponse>();
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var refreshTokenEntity = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (refreshTokenEntity == null || !refreshTokenEntity.IsActive)
            {
                throw new UnauthorizedException("Invalid or expired refresh token");
            }

            var user = refreshTokenEntity.User;

            var newToken = _tokenService.GenerateToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshTokenAsync();

            // Revocar el token anterior
            refreshTokenEntity.Revoked = DateTime.UtcNow;
            refreshTokenEntity.ReplacedByToken = newRefreshToken;

            // Crear nuevo refresh token
            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresDate = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(newRefreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponse(
                Token: newToken,
                Expiration: DateTime.UtcNow.AddHours(1),
                RefreshToken: newRefreshToken,
                RefreshTokenExpiration: DateTime.UtcNow.AddDays(7)
            );
        }

        public async Task LogoutAsync(LogoutRequest logoutRequest)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == logoutRequest.RefreshToken);

            if (refreshToken == null || refreshToken.IsRevoked)
                throw new BadRequestException("Invalid or already revoked token");

            refreshToken.Revoked = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private User createUser(RegisterRequest request)
        {
            return new User
            {
                Name = request.Name,
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                AvatarUrl = null,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
