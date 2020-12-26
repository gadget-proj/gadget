using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Messaging.Commands;
using Gadget.Server.Domain.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Agents.Consumers
{
    public class RegisterNewAgentConsumer : IConsumer<IRegisterNewAgent>
    {
        private readonly ILogger<RegisterNewAgentConsumer> _logger;
        private readonly ICollection<Agent> _agents;
        private readonly GadgetContext _context;

        public RegisterNewAgentConsumer(ILogger<RegisterNewAgentConsumer> logger, ICollection<Agent> agents,
            GadgetContext context)
        {
            _logger = logger;
            _agents = agents;
            _context = context;
        }

        public async Task Consume(ConsumeContext<IRegisterNewAgent> context)
        {
            _logger.LogInformation("Regisgering new agent");

            var agent = new Agent(context.Message.Agent);
            //TODO Services are always empty why?
            agent.AddServices(context.Message.Services?.Select(s =>
            {
                var service = s as Messaging.Service;
                return new Service(service?.Name, service?.Status);
            }));
            _agents.Add(agent);

            await _context.AddAsync(new Agent(context.Message.Agent));
            await _context.SaveChangesAsync();
        }
    }
}