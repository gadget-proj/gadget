namespace Gadget.Messaging.Events
{
    public interface IServiceStatusChanged
    {
        string ServiceName { get; }
        string Status { get; }
    }
}