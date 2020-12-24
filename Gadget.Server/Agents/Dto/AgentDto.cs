using System.Collections.Generic;

namespace Gadget.Server.Agents.Dto
{
    public class AgentDto
    {
        public string Name { get; set; }
        public IEnumerable<ServiceDto> Services { get; set; }
    }

    public class ServiceDto
    {
        public string Name { get; set; }
        public string Status { get; set; }
    }
}