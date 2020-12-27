namespace Gadget.Messaging.Events
{
    public interface IServiceStatusChanged
    {
        string Agent { get; }
        string Name { get; }
        string Status { get; }
    }
}