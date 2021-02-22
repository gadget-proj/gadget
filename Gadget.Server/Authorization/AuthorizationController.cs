using Gadget.Server.Authorization.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Gadget.Server.Authorization
{
    [ApiController]
    [Route("auth")]
    public class AuthorizationController : ControllerBase
    {
        private readonly TokenManager _tokenManager;

        public AuthorizationController(TokenManager tokenManager)
        {
            _tokenManager = tokenManager;
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
            if (request.UserName != "lucek" || request.Password != "lucek")
            {
                return Unauthorized();
            }
            var token = _tokenManager.GenerateToken(request.UserName);
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
    }
}