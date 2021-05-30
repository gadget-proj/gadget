using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gadget.Server.Dto.V1;

namespace Gadget.Server.Services.Interfaces
{
    public interface IGroupsService
    {
        Task<Guid> CreateGroup(string name);
        Task AddResource(string groupName, string resource);
        Task<IEnumerable<GroupPartialDto>> GetGroups();
        Task<GroupDto> GetGroup(string groupName);
        Task StopResourcesAsync(string groupName);
        Task StartResourcesAsync(string groupName);
    }
}