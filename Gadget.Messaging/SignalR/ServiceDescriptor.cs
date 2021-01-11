namespace Gadget.Messaging.SignalR
{
    /// <summary>
    /// ServiceDescriptor describes a windows service, its name and current status
    /// It is only meant to be used as a message
    /// </summary>
    public sealed class ServiceDescriptor
    {
        public string Agent { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string LogOnAs { get; set; }
        public string Description { get; set; }
    }
}