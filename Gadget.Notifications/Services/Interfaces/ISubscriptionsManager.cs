using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Gadget.Notifications.Services.Interfaces
{
    public interface ISubscriptionsManager
    {
        Task Add(string groupName);
        IAsyncEnumerable<string> Match(string selector);
    }

    public class SubscriptionsManager : ISubscriptionsManager
    {
        private readonly IList<string> _groups;
        private readonly ILogger<SubscriptionsManager> _logger;

        public SubscriptionsManager(IList<string> groups, ILogger<SubscriptionsManager> logger)
        {
            _groups = groups;
            _logger = logger;
        }

        public async Task Add(string groupName)
        {
            _logger.LogInformation($"Adding new group {groupName}");
            if (_groups.Contains(groupName))
            {
                _logger.LogInformation("Group already exists, skipping");
                return;
            }

            _groups.Add(groupName);
        }

        public async IAsyncEnumerable<string> Match(string selector)
        {
            _logger.LogInformation("Match");
            _logger.LogInformation($"{_groups.Count}");
            
            foreach (var group in _groups)
            {
                var regexp = new Regex(group);
                _logger.LogInformation($"{selector} is match for {group}");
                if (!regexp.IsMatch(selector))
                {
                    continue;
                }

                yield return group;
            }
        }
    }
}