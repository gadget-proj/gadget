using System;
using System.Threading.Tasks;
using Gadget.Server.Services;
using Gadget.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gadget.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ActionsController : ControllerBase
    {
        private readonly IActionsService _actionsService;

        public ActionsController(IActionsService actionsService)
        {
            _actionsService = actionsService;
        }

        [HttpGet("{correlationId:guid}")]
        public async Task<IActionResult> GetActions(Guid correlationId)
        {
            return Ok(await _actionsService.GetActionsAsync(correlationId));
        }
    }
}