using Gadget.Server.Authorization.Requests;
using Gadget.Server.Authorization.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Gadget.Server.Authorization
{
    [ApiController]
    [Route("auth")]
    public class AuthorizationController : ControllerBase
    {
        private readonly TokenManager _tokenManager;
        private readonly IUserService _userService;

        public AuthorizationController(TokenManager tokenManager, IUserService userService)
        {
            _tokenManager = tokenManager;
            _userService = userService;
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("ja man");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!await _userService.IsUservalid(request.UserName, request.Password))
            {
                return Unauthorized();
            }

            var token = _tokenManager.GenerateToken(request.UserName);
            var refreshToken = _tokenManager.GenerateRefreshToken();
            await _userService.SaveRefreshToken(request.UserName, refreshToken, GetIp());
            SetTokenCookie(refreshToken);
            return Ok(token);
        }

        

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LoginRequest request)
        {
            return Ok();
        }


        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshRequest request)
        {
            //var refreshToken = Request.Cookies["refreshToken"];
            var refreshToken = request.RefreshToken;
            var newToken = await  _userService.RefreshToken(refreshToken, GetIp());

            if (newToken is null)
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }

            SetTokenCookie(newToken.RefreshToken);

            return Ok(newToken.JwtToken);
        }

        // TODO move to helper class
        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string GetIp()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}