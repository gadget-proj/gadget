using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Gadget.Inspector
{
    public interface IServicesProvider
    {
        IAsyncEnumerable<string> GetServices();
    }

    public class ServicesProvider : IServicesProvider
    {
        public async IAsyncEnumerable<string> GetServices()
        {
            yield return await Task.FromResult("AppReadiness");
        }
    }

    public static class ServiceLogger
    {
        private static void SetLogColor(ServiceControllerStatus status)
        {
            switch (status)
            {
                case ServiceControllerStatus.Stopped:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case ServiceControllerStatus.StartPending:
                    break;
                case ServiceControllerStatus.StopPending:
                    break;
                case ServiceControllerStatus.Running:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case ServiceControllerStatus.ContinuePending:
                    break;
                case ServiceControllerStatus.PausePending:
                    break;
                case ServiceControllerStatus.Paused:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        public static void Log(string value, ServiceControllerStatus status)
        {
            SetLogColor(status);
            Console.WriteLine(value);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public class Service
    {
        public string Name => ServiceController.DisplayName;
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

        public void UpdateStatus()
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
    }

    class Program
    {
        private static async Task Main()
        {
            var services = new List<Service>();
            var s = new Service("AppReadiness");
            s.AddStatusHandler(ServiceControllerStatus.Stopped,
                controller => Console.WriteLine($"{controller.DisplayName} is stopped"));
            services.Add(s);
            while (true)
            {
                foreach (var service in services)
                {
                    ServiceLogger.Log($"[SERVICE] : {service.ServiceController.ServiceName} " +
                                      $"[STATUS] : {service.ServiceController.Status}" +
                                      $"{Environment.NewLine}", service.ServiceController.Status);
                    service.UpdateStatus();
                }

                await Task.Delay(5000);
            }
        }
    }
}