using Gadget.Server.Authorization.Helpers;
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
        private readonly AuthorizationHelper _authorizationHelper;

        public AuthorizationController(TokenManager tokenManager, IUserService userService, AuthorizationHelper authorizationHelper)
        {
            _tokenManager = tokenManager;
            _userService = userService;
            _authorizationHelper = authorizationHelper;
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
            if (!await _userService.IsUserValid(request.UserName, request.Password))
            {
                return Unauthorized();
            }

            var token = _tokenManager.GenerateToken(request.UserName);
            var refreshToken = _tokenManager.GenerateRefreshToken();
            await _userService.SaveRefreshToken(request.UserName, refreshToken, _authorizationHelper.GetIp(HttpContext));
            _authorizationHelper.SetTokenCookie(refreshToken, Response);
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
            var refreshToken = request.RefreshToken;
            var newToken = await  _userService.RefreshToken(refreshToken, _authorizationHelper.GetIp(HttpContext));

            if (newToken is null)
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }

            _authorizationHelper.SetTokenCookie(newToken.RefreshToken, Response);
            return Ok(newToken.JwtToken);
        }
    }
}