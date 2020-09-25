﻿using System;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Gadget.Inspector
{
    /// <summary>
    /// TODO Handler on status changed
    /// </summary>
    internal class WindowsService
    {
        private readonly ServiceController _serviceController;
        private ServiceControllerStatus _lastKnownStatus;
        public EventHandler<WindowsServiceStatusChanged> StatusChanged;
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

        public WindowsService(ServiceController serviceController)
        {
            _serviceController = serviceController;
            StartWatcher();
        }
        private void StartWatcher()
        {
            var _ = Task.Run(async () =>
            {
                while (true)
                {
                    _serviceController.Refresh();
                    var currentStatus = _serviceController.Status;
                    if (currentStatus != _lastKnownStatus)
                    {
                        Console.WriteLine("[ws] status has changed");
                        StatusChanged.Invoke(null, new WindowsServiceStatusChanged
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
        public void Start() => _serviceController.Start();
        public void Stop() => _serviceController.Stop();

    }
}