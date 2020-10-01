using System.ServiceProcess;

namespace Gadget.Inspector
{
        internal interface IWindowsService
        {
                ServiceControllerStatus Status { get; }
                void Start();
                void Stop();
        }
}