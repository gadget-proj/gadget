using System.Threading.Tasks;
using Gadget.Auth.Domain;
using Gadget.Auth.Dto;

namespace Gadget.Auth.Services.Interfaces
{
    public interface IUsersService
    {
        Task<bool> IsUserValid(string userName, string password);

        Task<bool> SaveRefreshToken(string userName, string token, string ipAddress);

        Task<RefreshTokenResult> RefreshToken(string token, string ipAddress);

        Task<bool> RefreshTokenUnvalidated(string token);

        Task CreateUser(string username, string password);
    }
}