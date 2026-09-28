using LMSBackend.Application.Abstractions.Authentication;
using LMSBackend.Application.DTOs.Authentication;
using Microsoft.AspNetCore.Http;
using LMSBackend.Application.DTOs.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LMSBackend.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterRequest request,
            CancellationToken cancellationToken)
        {
            await _authService.RegisterUserAsync(
                request,
                cancellationToken);

            return StatusCode(StatusCodes.Status201Created);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserResponse>> Login(
            LoginRequest request,
            CancellationToken cancellationToken)
        {
            LoginResult result = await _authService.LoginUserAsync(
                request,
                cancellationToken);

            Response.Cookies.Append(
                "access_token",
                result.TokenResponse.AccessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = result.TokenResponse.AccessTokenExpiresAt
                });

            Response.Cookies.Append(
                "refresh_token",
                result.TokenResponse.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = result.TokenResponse.RefreshTokenExpiresAt
                });

            return Ok(result.User);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
            CancellationToken cancellationToken)
        {
            await _authService.LogoutUserAsync(
                cancellationToken);

            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("consent")]
        public async Task<IActionResult> Consent(
            UserConsentRequest request,
            CancellationToken cancellationToken)
        {
            await _authService.AddUserConsentAsync(
                request,
                cancellationToken);

            return NoContent();
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponse>> Refresh(
            CancellationToken cancellationToken)
        {
            string refreshToken =
                Request.Cookies["refresh_token"]
                ?? throw new UnauthorizedAccessException(
                    "Refresh token is missing.");

            TokenResponse result = await _authService.RenewRefreshTokenAsync(
                refreshToken,
                cancellationToken);

            Response.Cookies.Append(
                "access_token",
                result.AccessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = result.AccessTokenExpiresAt
                });

            Response.Cookies.Append(
                "refresh_token",
                result.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = result.RefreshTokenExpiresAt
                });

            return Ok();
        }
    }
}
