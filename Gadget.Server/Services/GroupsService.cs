using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Server.Domain.Entities;
using Gadget.Server.Dto.V1;
using Gadget.Server.Persistence;
using Gadget.Server.Services.Interfaces;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Services
{
    public class GroupsService : IGroupsService
    {
        private readonly ILogger<GroupsService> _logger;
        private readonly GadgetContext _context;
        private readonly IAgentsService _agentsService;

        public GroupsService(ILogger<GroupsService> logger, GadgetContext context, IAgentsService agentsService)
        {
            _logger = logger;
            _context = context;
            _agentsService = agentsService;
        }

        public async Task<Guid> CreateGroup(string name)
        {
            var normalizedName = name.Trim().ToLower();
            _logger.LogInformation($"Creating new group {name}");
            var exists = await _context.Groups.AnyAsync(g => g.Name == normalizedName);
            if (exists)
            {
                _logger.LogWarning("There is group already present with this name");
                return Guid.Empty;
            }

            var group = new Group(normalizedName);
            await _context.AddAsync(group);
            await _context.SaveChangesAsync();
            return group.Id;
        }


        public async Task AddResource(string groupName, string resource)
        {
            var normalizedName = groupName.Trim().ToLower();
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == normalizedName);
            if (group is null)
            {
                _logger.LogWarning($"Could not find group with name {normalizedName}");
                return;
            }

            var normalizedServiceName = resource.Trim().ToLower();
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Name == normalizedServiceName);

            if (service is null)
            {
                _logger.LogWarning($"Could not find resource with name {normalizedServiceName}");
                return;
            }

            group.AddResource(service);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added resource to group");
        }

        public async Task RemoveResource(string groupName, string resource)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Name.ToLower() == groupName.ToLower());
            if (group is null)
            {
                throw new Exception("Could not find requested resource group");
            }
            _logger.LogInformation($"Trying to remove {resource} from resource group {group.Name}");
            group.RemoveResource(resource);
        }
        
        public async Task<IEnumerable<GroupPartialDto>> GetGroups()
        {
            return await _context.Groups.Select(g => new GroupPartialDto(g.Id, g.Name)).ToListAsync();
        }

        public async Task<GroupDto> GetGroup(string groupName)
        {
            var group = await _context.Groups
                .Include(g => g.Resources)
                .FirstOrDefaultAsync(g => g.Name == groupName)
                .Select(g => new GroupDto(g.Id, g.Name,
                    Enumerable.Select<Service, ServiceDto>(g.Resources,
                        s => new ServiceDto(s.Name, s.Status.ToString(), s.LogOnAs, s.Description))));
            return group;
        }

        public async Task StopResourcesAsync(string groupName)
        {
            try
            {
                var normalizedName = groupName.Trim().ToLower();
                var resources = await _context.Groups
                    .Include(g => g.Resources)
                    .ThenInclude(r => r.Agent)
                    .FirstOrDefaultAsync(r => r.Name == groupName);
                var stopRequests = resources.Resources.Select(r => _agentsService.StopService(r.Agent.Name, r.Name));
                await Task.WhenAll(stopRequests);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }
        
        public async Task StartResourcesAsync(string groupName)
        {
            try
            {
                var normalizedName = groupName.Trim().ToLower();
                var resources = await _context.Groups
                    .Include(g => g.Resources)
                    .ThenInclude(r => r.Agent)
                    .FirstOrDefaultAsync(r => r.Name == groupName);
                var stopRequests = resources.Resources.Select(r => _agentsService.StopService(r.Agent.Name, r.Name));
                await Task.WhenAll(stopRequests);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }
    }
}