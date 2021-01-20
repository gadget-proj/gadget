using Gadget.Messaging.Contracts.Commands;
using Gadget.Server.Agents.Dto;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gadget.Server.Agents
{
    public interface IAgentsService
    {
        Task<IEnumerable<AgentDto>> GetAgents();
        Task<IEnumerable<ServiceDto>> GetServices(string agentName);
        Task StartService(string agentName, string serviceName);
        Task StopService(string agentName, string serviceName);
        Task<IEnumerable<EventDto>> GetEvents(int number);
    }

    public class AgentsService : IAgentsService
    {
        private readonly GadgetContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<AgentsService> _logger;

        public AgentsService(GadgetContext context, IPublishEndpoint publishEndpoint, ILogger<AgentsService> logger)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<IEnumerable<AgentDto>> GetAgents()
        {
            var agents = await _context.Agents.ToListAsync();
            return await Task.FromResult(agents.Select(a => new AgentDto
            {
                Name = a.Name,
                Address = a.Address
            }));
        }

        public async Task<IEnumerable<EventDto>> GetEvents(int number)
        {
            var events = await _context.ServiceEvents
                        .OrderByDescending(x => x.CreatedAt)
                        .Take(number)
                        .ToListAsync();

            return await Task.FromResult(events.Select(e => new EventDto
            {
                CreatedAt = e.CreatedAt.ToString("hh:mm dd-MM-yyyy"),
                Status = e.Status,
                Agent = "Lorem",// to do 
                Service = "Ipsum"
            }));
        }

        public async Task<IEnumerable<ServiceDto>> GetServices(string agentName)
        {
            var machine = _context.Agents
                .Include(a => a.Services)
                .ThenInclude(s => s.Events.Take(20))
                .FirstOrDefault(x => x.Name == agentName);
            var services = machine?.Services;
            var dto = services?.Select(s => new ServiceDto(s.Name, s.Status, s.LogOnAs, s.Description, s.Events));
            return await Task.FromResult(dto);
        }

        public async Task StartService(string agentName, string serviceName)
        {
            try
            {
                await _publishEndpoint.Publish<IStartService>(new
                    {
                        ServiceName = serviceName,
                        Agent = agentName
                    },
                    context => { context.SetRoutingKey(serviceName); });
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }

        public async Task StopService(string agentName, string serviceName)
        {
            try
            {
                await _publishEndpoint.Publish<IStopService>(new
                    {
                        ServiceName = serviceName,
                        Agent = agentName
                    },
                    context => { context.SetRoutingKey(serviceName); });
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }
    }
}