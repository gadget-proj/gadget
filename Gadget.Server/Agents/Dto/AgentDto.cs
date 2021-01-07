namespace Gadget.Server.Agents.Dto
{
    public class AgentDto
    {
        public string Name { get; set; }
    }

    public class ServiceDto
    {
        public ServiceDto(string name, string status, string logOnAs, string description)
        {
            Name = name;
            Status = status;
            LogOnAs = logOnAs;
            Description = description;
        }

        public string Name {get;}
        public string Status {get;}
        public string LogOnAs { get; set; }
        public string Description { get; set; }
    }
}