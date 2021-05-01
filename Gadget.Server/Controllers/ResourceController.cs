using System;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Server.Domain.Entities;
using Gadget.Server.Domain.Enums;
using Gadget.Server.Dto.V1.Requests;
using Gadget.Server.Persistence;
using Gadget.Server.Services;
using Gadget.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ResourceController : ControllerBase
    {
        private readonly ISelectorService _selectorService;
        private readonly IAgentsService _agentsService;
        private readonly GadgetContext _gadgetContext;
        private readonly ILogger<ResourceController> _logger;

        public ResourceController(ISelectorService selectorService, IAgentsService agentsService,
            GadgetContext gadgetContext, ILogger<ResourceController> logger)
        {
            _selectorService = selectorService;
            _agentsService = agentsService;
            _gadgetContext = gadgetContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetResource(string selector)
        {
            var resources = await _selectorService.Match(selector);
            return resources is not null ? Ok(resources) : NotFound();
        }

        [HttpPost("config/apply")]
        public async Task<IActionResult> ApplyConfig(ApplyConfigRequest request)
        {
            var entry = Enum.Parse<ServiceStatus>(request.Rules.FirstOrDefault()?.Actions.FirstOrDefault()?.Event!);
            foreach (var configRequest in request.Rules)
            {
                var selector = configRequest.Selector;
                var svc = await _gadgetContext
                    .Services
                    .Include(s => s.Config)
                    .FirstOrDefaultAsync(s => s.Name == selector.ToLower().Trim());
                if (svc is null)
                {
                    continue;
                }

                var config = new Config(configRequest.Actions);
                svc.ApplyConfig(config);
                await _gadgetContext.SaveChangesAsync();
                _logger.LogInformation((false).ToString());
            }


            return Ok();
        }
    }
}