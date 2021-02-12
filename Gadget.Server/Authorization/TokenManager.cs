using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

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
            byte[] key = Encoding.ASCII.GetBytes(_secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            var handler = new JwtSecurityTokenHandler();

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
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

        public  ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtoken = (JwtSecurityToken)handler.ReadToken(token);
                if (jwtoken == null)
                {
                    return null;
                }

                byte[] key = Convert.FromBase64String(_secret);
                var parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                SecurityToken securityToken;
                var principal = handler.ValidateToken(token, parameters, out securityToken);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public  bool ValidateToken(string token, string userName)
        {

            var handler = new JwtSecurityTokenHandler();

            var principal = GetPrincipal(token);
            if (principal == null)
            {
                return false;
            }
            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (Exception)
            {
                return false;
            }

            var userNameClaim = identity.FindFirst(ClaimTypes.Name);
            return userName == userNameClaim.Value;
        }
    }
}
