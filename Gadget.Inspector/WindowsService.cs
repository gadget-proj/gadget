using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using Gadget.Inspector.Events;

namespace Gadget.Inspector
{
    internal class WindowsService : IWindowsService
    {
        private readonly ServiceController _serviceController;
        private ServiceControllerStatus _lastKnownStatus;
        public EventHandler<WindowsServiceStatusChanged> StatusChanged;

        public WindowsService(ServiceController serviceController)
        {
            _serviceController = serviceController;
            StartWatcher();
        }

        public ServiceControllerStatus Status
        {
            get
            {
                _serviceController.Refresh();
                var currentStatus = _serviceController.Status;
                if (_lastKnownStatus != currentStatus)
                {
                    //Status has changed, notify the server
                }

                _lastKnownStatus = currentStatus;
                return currentStatus;
            }
        }

        public void Start()
        {
            _serviceController.Start();
        }

        public void Stop()
        {
            _serviceController.Stop();
        }

        private void StartWatcher()
        {
            //Possibly stealing thread from thread pool and never returning it
            var _ = Task.Run(async () =>
            {
                while (true)
                {
                    _serviceController.Refresh();
                    var currentStatus = _serviceController.Status;
                    if (currentStatus != _lastKnownStatus)
                    {
                        StatusChanged.Invoke(this, new WindowsServiceStatusChanged
                        {
                            ServiceName = _serviceController.ServiceName,
                            Status = currentStatus
                        });
                    }

                    _lastKnownStatus = currentStatus;
                    await Task.Delay(1000);
                }
            });
        }
    }
}