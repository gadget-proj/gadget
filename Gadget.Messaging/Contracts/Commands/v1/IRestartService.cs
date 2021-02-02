namespace Gadget.Messaging.Contracts.Commands.v1
{
    public interface IRestartService
    {
        string Agent { get; }
        string ServiceName { get; }
    }
}
