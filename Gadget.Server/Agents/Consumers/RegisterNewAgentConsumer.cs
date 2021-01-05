using Gadget.Messaging.Contracts.Commands;
using Gadget.Messaging.SignalR;
using Gadget.Server.Domain.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gadget.Server.Agents.Consumers
{
    /// <summary>
    /// Handles new agent(Inspector) registration
    /// </summary>
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
            _logger.LogInformation($"Trying to register new agent {context.Message.Agent}");
            var exists = _context.Agents
                .Any(a => a.Name == context.Message.Agent);
            if (exists)
            {
                _logger.LogInformation($"Agent {context.Message.Agent} is already registered, skipping");
                return;
            }

            var agent = new Agent(context.Message.Agent);
            agent.AddServices(context.Message.Services?.Select(s =>
            {
                //I dont like this, TODO check MassTransit serialization constraints
                var service = JsonConvert.DeserializeObject<ServiceDescriptor>(s.ToString());
                return new Service(service?.Name, service?.Status, agent);
            }));
            _agents.Add(agent);

            await _context.Agents.AddAsync(agent);
            await _context.SaveChangesAsync();
        }
    }
}