namespace Gadget.Server.Dto.V1
{
    public class ServiceDto
    {
        public ServiceDto(string name, string status, string logOnAs, string description)
        {
            Name = name;
            Status = status;
            LogOnAs = logOnAs;
            Description = description;
        }

        public string Name { get; }
        public string Status { get; }
        public string LogOnAs { get;  }
        public string Description { get;  }
    }
}