using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gadget.Server.Dto.V1;

namespace Gadget.Server.Services.Interfaces
{
    public interface IActionsService
    {
        Task<IEnumerable<ActionDto>> GetActionsAsync(Guid correlationId);
    }
}