using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Gadget.Auth.Domain;
using Gadget.Auth.Dto;
using Gadget.Auth.Persistence;
using Gadget.Auth.Providers;
using Gadget.Auth.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gadget.Auth.Services
{
    public class UsersService : IUsersService
    {
        private readonly AuthContext _context;
        private readonly ILoginProvider _loginProvider;
        private readonly TokenManager _tokenManager;
        private readonly ILogger<UsersService> _logger;

        public UsersService(AuthContext context, ILoginProvider loginProvider, TokenManager tokenManager,
            ILogger<UsersService> logger)
        {
            _context = context;
            _loginProvider = loginProvider;
            _tokenManager = tokenManager;
            _logger = logger;
        }

        public async Task<bool> AddUser(string userName)
        {
            var user = new User(userName);
            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<User> GetUser(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task<bool> IsUserValid(string userName, string password)
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
            var user = await _context.Users.Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.UserName == userName);
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

            var user = await _context.Users.Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.RefreshTokens.Any(y => y.Token == decodedToken));
            if (user is null)
            {
                return null;
            }

            var refreshToken = user.RefreshTokens.Single(x => x.Token == decodedToken);
            if (!refreshToken.IsTokenIpValid(ipAddress) || !refreshToken.IsActive)
            {
                return null;
            }

            var newToken = _tokenManager.GenerateToken(user.UserName);
            if (refreshToken.IsActive)
            {
                refreshToken.Use();
                return new RefreshTokenResult(newToken, refreshToken.Token);
            }


            var newRefreshToken = TokenManager.GenerateRefreshToken();
            await SaveRefreshToken(user.UserName, newRefreshToken, ipAddress);
            return new RefreshTokenResult(newToken, newRefreshToken);
        }

        public async Task<bool> RefreshTokenUnvalidated(string token)
        {
            var decodedToken = HttpUtility.UrlDecode(token);

            var user = await _context.Users.Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.RefreshTokens.Any(y => y.Token == decodedToken));
            if (user is null)
            {
                return false;
            }

            var refreshToken = user.RefreshTokens.Single(x => x.Token == decodedToken);
            refreshToken.Unvalidate();
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task CreateUser(string username, string password)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User(username, hash);
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}