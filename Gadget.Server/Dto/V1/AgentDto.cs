namespace Gadget.Server.Dto.V1
{
    public record AgentDto (string Name, string Address);

    public class AgentDtoClass
    {
        public string Name { get; set; }
        public string Agent { get; set; }

        private AgentDtoClass()
        {
            
        }

        public AgentDtoClass(string name, string agent)
        {
            Name = name;
            Agent = agent;
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}