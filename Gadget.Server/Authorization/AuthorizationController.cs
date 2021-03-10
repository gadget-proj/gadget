using Gadget.Server.Authorization.Providers;
using Gadget.Server.Authorization.Requests;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!await _userService.IsUservalid(request.UserName, request.Password))
            {
                return Unauthorized();
            }

            var token = _tokenManager.GenerateToken(request.UserName);
            var refreshToken = _tokenManager.GenerateRefreshToken();
            await _userService.SaveRefreshToken(request.UserName, refreshToken);
            SetTokenCookie(refreshToken);
            return Ok(token);
        }

        

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LoginRequest request)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Refresh(RefreshRequest request)
        {
            if (_tokenManager.ValidateToken(request.Token, request.UserName))
            {
                return Unauthorized();
            }

            return Ok(_tokenManager.GenerateToken(request.UserName));
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}