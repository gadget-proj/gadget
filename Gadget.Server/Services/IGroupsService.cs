using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Server.Domain.Entities;
using Gadget.Server.Dto.V1;
using Gadget.Server.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Services
{
    public interface IGroupsService
    {
        Task<Guid> CreateGroup(string name);
        Task AddResource(Guid groupId, string resource);
        Task AddResource(string groupName, string resource);
        Task<IEnumerable<GroupDto>> GetGroups();
    }

    public class GroupsService : IGroupsService
    {
        private readonly ILogger<GroupsService> _logger;
        private readonly GadgetContext _context;

        public GroupsService(ILogger<GroupsService> logger, GadgetContext context)
        {
            _logger = logger;
            _context = context;
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

        public async Task AddResource(Guid groupId, string resource)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            if (group is null)
            {
                _logger.LogWarning($"Could not find group with id {groupId}");
                return;
            }
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

        public async Task<IEnumerable<GroupDto>> GetGroups()
        {
            return await _context.Groups.Select(g => new GroupDto(g.Id, g.Name)).ToListAsync();
        }
    }
}