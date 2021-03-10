using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Gadget.Server.Authorization
{
    public class TokenManager
    {
        private readonly string _secret;

        public TokenManager(IConfiguration configuration)
        {
            _secret = configuration.GetValue<string>("SecurityKey");
        }

        public string GenerateToken(string userName)
        {
            var key = Encoding.ASCII.GetBytes(_secret);
            var handler = new JwtSecurityTokenHandler();

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userName)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var num = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(num);
                return Convert.ToBase64String(num);
            }
        }

        private ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = (JwtSecurityToken) handler.ReadToken(token);
                if (jwtSecurityToken == null)
                {
                    return null;
                }

                var key = Convert.FromBase64String(_secret);
                var parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var principal = handler.ValidateToken(token, parameters, out _);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool ValidateToken(string token, string userName)
        {
            var principal = GetPrincipal(token);
            if (principal == null)
            {
                return false;
            }

            ClaimsIdentity identity;
            try
            {
                identity = (ClaimsIdentity) principal.Identity;
            }
            catch (Exception)
            {
                return false;
            }

            var userNameClaim = identity?.FindFirst(ClaimTypes.Name);
            return userName == userNameClaim?.Value;
        }
    }
}