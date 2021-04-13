using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Gadget.Auth;
using Gadget.Auth.Helpers;
using Gadget.Auth.Requests;
using Gadget.Auth.Services.Interfaces;

namespace Gadget.Server.Authorization
{
    [ApiController]
    [Route("auth")]
    public class AuthorizationController : ControllerBase
    {
        private readonly TokenManager _tokenManager;
        private readonly IUsersService _usersService;
        private readonly AuthorizationHelper _authorizationHelper;

        public AuthorizationController(TokenManager tokenManager, IUsersService usersService, AuthorizationHelper authorizationHelper)
        {
            _tokenManager = tokenManager;
            _usersService = usersService;
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
            if (!await _usersService.IsUserValid(request.UserName, request.Password))
            {
                return Unauthorized();
            }

            var token = _tokenManager.GenerateToken(request.UserName);
            var refreshToken = TokenManager.GenerateRefreshToken();
            await _usersService.SaveRefreshToken(request.UserName, refreshToken, _authorizationHelper.GetIp(HttpContext));
            _authorizationHelper.SetTokenCookie(refreshToken, Response);
            return Ok(token);
        }

        

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (await _usersService.RefreshTokenUnvalidated(refreshToken))
            {
                return Ok();
            }
            return NotFound();
        }


        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized("Refresh token not found");
            }
            var newToken = await  _usersService.RefreshToken(refreshToken, _authorizationHelper.GetIp(HttpContext));

            if (newToken is null)
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }

            _authorizationHelper.SetTokenCookie(newToken.RefreshToken, Response);
            return Ok(newToken.JwtToken);
        }
    }
}