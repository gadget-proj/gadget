using System;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Server.Domain.Entities;
using Gadget.Server.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Services
{
    public interface IGroupsService
    {
        Task<Guid> CreateGroup(string name);
        Task AddResource(Guid groupId, string resource);
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
            _logger.LogInformation($"Creating new group {name}");
            var group = new Group(name);
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
    }
}