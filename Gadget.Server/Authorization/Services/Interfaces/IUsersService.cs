using Gadget.Server.Authorization.Dto;
using Gadget.Server.Domain.Entities;
using System.Threading.Tasks;

namespace Gadget.Server.Authorization.Services.Interfaces
{
    public interface IUsersService
    {
        Task<User> GetUser(string userName);

        Task<bool> AddUser(string userName);

        Task<bool> IsUserValid(string userName, string password);

        Task<bool> SaveRefreshToken(string userName, string token, string ipAddress);

        Task<RefreshTokenResult> RefreshToken(string token, string ipAddress);

    }
}
