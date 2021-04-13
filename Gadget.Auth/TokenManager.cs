using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Gadget.Auth
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
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }

        public static string GenerateRefreshToken()
        {
            var num = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(num);
            return Convert.ToBase64String(num);
        }
    }
}