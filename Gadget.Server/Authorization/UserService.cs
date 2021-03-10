using Gadget.Server.Authorization.Providers;
using Gadget.Server.Domain.Entities;
using Gadget.Server.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Gadget.Server.Authorization
{
    public class UserService : IUserService
    {
        private readonly GadgetContext _context;
        private readonly ILoginProvider _loginProvider;

        public UserService(GadgetContext context, ILoginProvider loginProvider)
        {
            _context = context;
            _loginProvider = loginProvider;
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

        public async Task<bool> SaveRefreshToken(string userName, string token)
        {
            var user = await _context.Users.Include(x => x.RefreshTokens).FirstOrDefaultAsync(x=>x.UserName==userName);
            if (user is null)
            {
                return false;
            }
           // _context.RefreshToken.Add(new RefreshToken(user, token));
            user.AddRefreshToken(new RefreshToken(user, token));
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
