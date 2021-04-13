using System;
using Microsoft.AspNetCore.Http;

namespace Gadget.Auth.Helpers
{
    public class AuthorizationHelper
    {
        public void SetTokenCookie(string token, HttpResponse response)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        public string GetIp(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return context.Request.Headers["X-Forwarded-For"];
            }

            else
            {
                return context.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
                
        }
    }
}
