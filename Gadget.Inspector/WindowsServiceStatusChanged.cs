using System.ServiceProcess;

namespace Gadget.Inspector
{
    internal class WindowsServiceStatusChanged
    {
        public string ServiceName { get; set; }
        public ServiceControllerStatus Status { get; set; }
    }
}