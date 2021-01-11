using System.Threading.Tasks;
using Gadget.Server.Agents.Dto;

namespace Gadget.Server.Agents.Services
{
    public class AgentsService : IAgentsService
    {
        public Task<AgentDto> GetAgents()
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IAgentsService
    {
        Task<AgentDto> GetAgents();
    }
}