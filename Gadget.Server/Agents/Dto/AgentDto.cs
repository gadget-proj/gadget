namespace Gadget.Server.Agents.Dto
{
    public class AgentDto
    {
        public string Name { get; set; }
    }

    public class ServiceDto
    {
        public ServiceDto(string name, string status)
        {
            Name = name;
            Status = status;
        }

        public string Name {get;}
        public string Status {get;}
    }
}