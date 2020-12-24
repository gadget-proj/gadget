using System.Collections.Generic;
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

        public RegisterNewAgentConsumer(ILogger<RegisterNewAgentConsumer> logger, ICollection<Agent> agents)
        {
            _logger = logger;
            _agents = agents;
        }

        public Task Consume(ConsumeContext<IRegisterNewAgent> context)
        {
            _logger.LogInformation($"{context.Message.GetType()}");
            _agents.Add(new Agent(context.Message.Agent));
            return Task.CompletedTask;
        }
    }
}