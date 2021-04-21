using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gadget.Server.Dto.V1;
using MassTransit;

namespace Gadget.Server.Services
{
    public interface IGroupsService
    {
        Task<Guid> CreateGroup(string name);
        Task AddResource(Guid groupId, string resource);
        Task AddResource(string groupName, string resource);
        Task<IEnumerable<GroupPartialDto>> GetGroups();
        Task<GroupDto> GetGroup(string groupName);
        Task StopResourcesAsync(string groupName);
    }
}