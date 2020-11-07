using System.ServiceProcess;

namespace Gadget.Inspector.Services.Interfaces
{
    internal interface IWindowsService
    {
        ServiceControllerStatus Status { get; }
        void Start();
        void Stop();
    }
}