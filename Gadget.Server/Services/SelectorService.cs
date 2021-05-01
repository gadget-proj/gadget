using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gadget.Server.Dto.V1;
using Gadget.Server.Persistence;
using Gadget.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gadget.Server.Services
{
    public class SelectorService : ISelectorService
    {
        private readonly GadgetContext _gadgetContext;
        private readonly ILogger<SelectorService> _logger;

        public SelectorService(GadgetContext gadgetContext, ILogger<SelectorService> logger)
        {
            _gadgetContext = gadgetContext;
            _logger = logger;
        }

        public async Task<IEnumerable<ServiceDto>> Match(string query)
        {
            var tokens = query.Split("/");
            if (!tokens.Any())
            {
                return null;
            }

            var agent = tokens.ElementAt(0);

            if (tokens.Length == 1)
            {
                var resource = await _gadgetContext.Services.FirstOrDefaultAsync(s => s.Name == query);
                if (resource is not null)
                    return new[]
                    {
                        new ServiceDto(resource.Name, resource.Status.ToString(), resource.LogOnAs,
                            resource.Description)
                    };
                _logger.LogInformation("could not find resource for {query}", query);
                return null;
            }

            var service = tokens.ElementAt(1).ToLower().Trim();

            var resources = await _gadgetContext.Services
                .Include(s => s.Agent)
                .Where(a => a.Name == service && a.Agent.Name == agent)
                .Select(s => new ServiceDto(s.Name, s.Status.ToString(), s.LogOnAs, s.Description))
                .ToListAsync();
            return resources;
        }
    }
}