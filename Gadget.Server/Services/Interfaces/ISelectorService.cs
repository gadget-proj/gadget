using System.Collections.Generic;
using System.Threading.Tasks;
using Gadget.Server.Dto.V1;

namespace Gadget.Server.Services.Interfaces
{
    public interface ISelectorService
    {
        Task<IEnumerable<ServiceDto>> Match(string query);
    }
}