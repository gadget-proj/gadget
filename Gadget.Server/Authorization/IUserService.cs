using Gadget.Server.Domain.Entities;
using System.Threading.Tasks;

namespace Gadget.Server.Authorization
{
    public interface IUserService
    {
        Task<User> GetUser(string userName);

        Task<bool> AddUser(string userName);

    }
}
