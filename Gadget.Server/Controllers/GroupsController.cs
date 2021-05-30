using System.Threading.Tasks;
using Gadget.Server.Dto.V1;
using Gadget.Server.Dto.V1.Requests;
using Gadget.Server.Services;
using Gadget.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gadget.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupsService _groupsService;


        public GroupsController(IGroupsService groupsService)
        {
            _groupsService = groupsService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewGroup(CreateNewGroupDto request)
        {
            var id = await _groupsService.CreateGroup(request.Name);
            return Ok(id);
        }

        [HttpPost("{groupName}")]
        public async Task<IActionResult> AddToGroup(string groupName, AddToGroupRequest request)
        {
            await _groupsService.AddResource(groupName, request.ResourceName);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            var group = await _groupsService.GetGroups();
            return Ok(group);
        }

        [HttpGet("{groupName}")]
        public async Task<IActionResult> GetGroup(string groupName)
        {
            var group = await _groupsService.GetGroup(groupName);
            return Ok(group);
        }

        [HttpPost("{groupName}/stop")]
        public async Task<IActionResult> StopGroupResources(string groupName)
        {
            await _groupsService.StopResourcesAsync(groupName);
            return Ok();
        }

        [HttpPost("{groupName}/start")]
        public async Task<IActionResult> StartGroupResources(string groupName)
        {
            await _groupsService.StartResourcesAsync(groupName);
            return Ok();
        }
    }
}