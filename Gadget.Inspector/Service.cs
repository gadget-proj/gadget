using System;
using System.Collections.Generic;
using System.ServiceProcess;

namespace Gadget.Inspector
{
    public class Service
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name => ServiceController.ServiceName;
        public ServiceController ServiceController { get; }
        public ServiceControllerStatus LastKnownStatus { get; private set; }

        private readonly IDictionary<ServiceControllerStatus, ICollection<Action<ServiceController>>> _actions =
            new Dictionary<ServiceControllerStatus, ICollection<Action<ServiceController>>>();

        public Service(string name)
        {
            ServiceController = new ServiceController(name);
        }

        public void AddStatusHandler(ServiceControllerStatus status, Action<ServiceController> action)
        {
            if (!_actions.TryGetValue(status, out var actions))
            {
                actions = new List<Action<ServiceController>>();
                _actions[status] = actions;
            }

            actions.Add(action);
        }
        public void Restart()
        {
            var status = ServiceController.Status;
            if (status == ServiceControllerStatus.Stopped)
            {
                ServiceController.Start();
            }
            else
            {
                ServiceController.Stop();
                ServiceController.Start();
            }
        }
        public void Refresh()
        {
            ServiceController.Refresh();
            var status = ServiceController.Status;
            if (status == LastKnownStatus) return;
            InvokeHandler(status);
            LastKnownStatus = status;
        }

        private void InvokeHandler(ServiceControllerStatus status)
        {
            if (!_actions.TryGetValue(status, out var actions)) return;
            foreach (var action in actions)
            {
                action.Invoke(ServiceController);
            }
        }

        public override string ToString()
        {
            return $"Id : [{Id}] Service : [{Name}]";
        }
    }
}