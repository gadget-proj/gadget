using System.Collections.Generic;
using System.Linq;
using Gadget.Server.Domain.Entities;

namespace Gadget.Server.Agents.Extensions
{
    public static class AgentExtensions
    {
        public static Agent WithConnectionId(this IEnumerable<Agent> agents, string connectionId)
        {
            return agents.FirstOrDefault(a => a.ConnectionId == connectionId);
        }
    }
}