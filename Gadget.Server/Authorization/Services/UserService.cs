using Gadget.Server.Authorization.Dto;
using Gadget.Server.Authorization.Providers;
using Gadget.Server.Authorization.Services.Interfaces;
using Gadget.Server.Domain.Entities;
using Gadget.Server.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Gadget.Server.Authorization.Services
{
    public class UserService : IUserService
    {
        private readonly GadgetContext _context;
        private readonly ILoginProvider _loginProvider;
        private readonly TokenManager _tokenManager;

        public UserService(GadgetContext context, ILoginProvider loginProvider, TokenManager tokenManager)
        {
            _context = context;
            _loginProvider = loginProvider;
            _tokenManager = tokenManager;
        }
        public async Task<bool> AddUser(string userName)
        {
            var user = new User(userName);
            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<User> GetUser(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(x=>x.UserName ==userName);
        }

        public async Task<bool> IsUservalid(string userName, string password)
        {
            var user = await GetUser(userName);
            if (user is null)
            {
                return false;
            }
            return _loginProvider.PasswordValid(userName, password);
        }

        public async Task<bool> SaveRefreshToken(string userName, string token, string ipAddress)
        {
            var user = await _context.Users.Include(x => x.RefreshTokens).FirstOrDefaultAsync(x=>x.UserName==userName);
            if (user is null)
            {
                return false;
            }
            user.AddRefreshToken(new RefreshToken(user, token, ipAddress));
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<RefreshTokenResult> RefreshToken(string token, string ipAddress)
        {
            var decodedToken = HttpUtility.UrlDecode(token);

            var user = await _context.Users.Include(x => x.RefreshTokens).FirstOrDefaultAsync(x => x.RefreshTokens.Any(y=>y.Token == decodedToken));
            if (user is null)
            {
                return null;
            }

            var refreshToken = user.RefreshTokens.Single(x => x.Token == decodedToken);
            if (!refreshToken.IsTokenIpValid(ipAddress) || !refreshToken.IsActive)
            {
                return null;
            }

            refreshToken.Use();
            var newRefreshToken = _tokenManager.GenerateRefreshToken();

           await SaveRefreshToken(user.UserName, newRefreshToken, ipAddress);
           var newJWT = _tokenManager.GenerateToken(user.UserName);

            return new RefreshTokenResult(newJWT, newRefreshToken);
        }
    }
}
