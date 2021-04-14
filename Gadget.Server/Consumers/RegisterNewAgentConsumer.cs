using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Commands;
using Gadget.Messaging.Contracts.Commands.v1;
using Gadget.Messaging.SignalR;
using Gadget.Messaging.SignalR.v1;
using Gadget.Server.Domain.Entities;
using Gadget.Server.Persistence;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Gadget.Server.Consumers
{
    /// <summary>
    /// Handles new agent(Inspector) registration
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RegisterNewAgentConsumer : IConsumer<IRegisterNewAgent>
    {
        private readonly ILogger<RegisterNewAgentConsumer> _logger;
        private readonly GadgetContext _context;

        public RegisterNewAgentConsumer(ILogger<RegisterNewAgentConsumer> logger, GadgetContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Consume(ConsumeContext<IRegisterNewAgent> context)
        {
            _logger.LogInformation($"Trying to register new agent {context.Message.Agent}");
            var exists = _context.Agents.Any(a => a.Name == context.Message.Agent);
            if (exists)
            {
                _logger.LogInformation($"Agent {context.Message.Agent} is already registered, skipping");
                return;
            }

            var agent = new Agent(context.Message.Agent, context.Message.Address);
            agent.AddServices(context.Message.Services?.Select(s =>
            {
                //I dont like this, TODO check MassTransit serialization constraints
                var service = JsonConvert.DeserializeObject<ServiceDescriptor>(s.ToString());
                return new Service(service?.Name.ToLower().Trim(), service?.Status, agent, service?.LogOnAs,
                    service?.Description);
            }));

            await _context.Agents.AddAsync(agent);
            await _context.SaveChangesAsync();
        }
    }
}