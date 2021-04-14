using System.Threading.Tasks;
using Gadget.Server.Dto.V1;
using Gadget.Server.Dto.V1.Requests;
using Gadget.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gadget.Server.Controllers
{
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

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            var groups = await _groupsService.GetGroups();
            return Ok(groups);
        }

        [HttpPost("{groupName}")]
        public async Task<IActionResult> AddToGroup(string groupName, AddToGroupRequest request)
        {
            await _groupsService.AddResource(groupName, request.ResourceName);
            return Ok();
        }
    }
}