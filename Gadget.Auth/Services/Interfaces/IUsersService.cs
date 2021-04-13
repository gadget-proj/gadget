using System.Threading.Tasks;
using Gadget.Auth.Domain;
using Gadget.Auth.Dto;

namespace Gadget.Auth.Services.Interfaces
{
    public interface IUsersService
    {
        Task<User> GetUser(string userName);

        Task<bool> AddUser(string userName);

        Task<bool> IsUserValid(string userName, string password);

        Task<bool> SaveRefreshToken(string userName, string token, string ipAddress);

        Task<RefreshTokenResult> RefreshToken(string token, string ipAddress);

        Task<bool> RefreshTokenUnvalidated(string token);

    }
}
