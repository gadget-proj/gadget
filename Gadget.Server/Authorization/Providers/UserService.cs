using Gadget.Server.Domain.Entities;
using Gadget.Server.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Gadget.Server.Authorization.Providers
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
    }
}
