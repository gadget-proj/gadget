using Gadget.Server.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Gadget.Server.Authorization
{
    public interface IUserService
    {
        Task<User> GetUser(string userName);

        Task<bool> AddUser(string userName);

        Task<bool> IsUservalid(string userName, string password);

        Task<bool> SaveRefreshToken(string userName, string token);

    }
}
