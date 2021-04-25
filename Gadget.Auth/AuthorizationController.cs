using System.Threading.Tasks;
using Gadget.Auth.Domain;
using Gadget.Auth.Helpers;
using Gadget.Auth.Persistence;
using Gadget.Auth.Requests;
using Gadget.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gadget.Auth
{
    [ApiController]
    [Route("auth")]
    public class AuthorizationController : ControllerBase
    {
        private readonly TokenManager _tokenManager;
        private readonly IUsersService _usersService;
        private readonly AuthorizationHelper _authorizationHelper;
        private readonly AuthContext _context;

        public AuthorizationController(TokenManager tokenManager, IUsersService usersService,
            AuthorizationHelper authorizationHelper, AuthContext context)
        {
            _tokenManager = tokenManager;
            _usersService = usersService;
            _authorizationHelper = authorizationHelper;
            _context = context;
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("ja man");
        }

        [AllowAnonymous]
        [HttpPost("newnewnew")]
        public async Task<IActionResult> CreateNewUser()
        {
            await _context.AddAsync(new User("lucek"));
            await _context.SaveChangesAsync();
            return Ok();
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
            await _usersService.SaveRefreshToken(request.UserName, refreshToken,
                _authorizationHelper.GetIp(HttpContext));
            _authorizationHelper.SetTokenCookie(refreshToken, Response);
            return Ok(token);
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateAccount(CreateAccountRequest request)
        {
            await _usersService.CreateUser(request.Username, request.Password);
            return Ok();
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

            var newToken = await _usersService.RefreshToken(refreshToken, _authorizationHelper.GetIp(HttpContext));

            if (newToken is null)
            {
                return Unauthorized(new {message = "Invalid refresh token"});
            }

            _authorizationHelper.SetTokenCookie(newToken.RefreshToken, Response);
            return Ok(newToken.JwtToken);
        }
    }
}