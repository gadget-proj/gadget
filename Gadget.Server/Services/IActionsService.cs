using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Server.Dto.V1;
using Gadget.Server.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gadget.Server.Services
{
    public interface IActionsService
    {
        Task<IEnumerable<ActionDto>> GetActionsAsync(Guid correlationId);
    }


    public class ActionsService : IActionsService
    {
        private readonly GadgetContext _context;

        public ActionsService(GadgetContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActionDto>> GetActionsAsync(Guid correlationId)
        {
            if (correlationId == Guid.Empty)
            {
                throw new ApplicationException($"Invalid correlationId {correlationId} provided");
            }

            var actions = await _context.UserActions.Where(a => a.CorrelationId == correlationId).ToListAsync();
            return actions.Select(a => new ActionDto(a.Id, a.CorrelationId, a.Date, a.Result.ToString(), a.Reason));
        }
    }
}