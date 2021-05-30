using System;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Responses;
using Gadget.Server.Domain.Entities;
using Gadget.Server.Domain.Enums;
using Gadget.Server.Persistence;
using MassTransit;

namespace Gadget.Server.Consumers
{
    public class ActionResultConsumer : IConsumer<IActionResultResponse>
    {
        private readonly GadgetContext _context;

        public ActionResultConsumer(GadgetContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<IActionResultResponse> context)
        {
            var actionId = context.CorrelationId;
            if (!actionId.HasValue)
            {
                return;
            }

            var result = context.Message.Success ? ActionResult.Succeeded : ActionResult.Failed;
            var action = new UserAction(Guid.NewGuid(), actionId.Value, DateTime.UtcNow, result,
                context.Message.Reason);
            await _context.UserActions.AddAsync(action);
            await _context.SaveChangesAsync();
        }
    }
}