using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NaturalSpaceApi.Application.DTOs.Auth;
using NaturalSpaceApi.Application.Interfaces;

namespace NaturalSpaceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserResponse>> LoginAsync([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> RegisterAsync([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);

            return Ok(result);

        }

        [HttpPost("logout")]
        public async Task<ActionResult> LogoutAsync([FromBody] LogoutRequest request)
        {
            var result = await _authService.LogoutAsync(request);

            if (!result)
                return BadRequest(new { message = "Invalid or already revoked token" });

            return Ok(new { message = "Logout successful" });
        }
    }
}
