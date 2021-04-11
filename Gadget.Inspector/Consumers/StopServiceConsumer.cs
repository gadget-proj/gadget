using System;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Commands;
using Gadget.Messaging.Contracts.Commands.v1;
using MassTransit;
using Microsoft.Extensions.Logging;
using Polly;

namespace Gadget.Inspector.Consumers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StopServiceConsumer : IConsumer<IStopService>
    {
        private readonly ILogger<StopServiceConsumer> _logger;

        public StopServiceConsumer(ILogger<StopServiceConsumer> logger)
        {
            _logger = logger;
        }


        public async Task Consume(ConsumeContext<IStopService> context)
        {
            _logger.LogInformation($"Trying to stop {context.Message.ServiceName}");
            var service = ServiceController
                .GetServices()
                .FirstOrDefault(s => s.ServiceName == context.Message.ServiceName);

            if (service is null)
            {
                throw new ApplicationException($"Service {context.Message.ServiceName} could not be found");
            }

            var serviceId = $"{context.Message.Agent}/{context.Message.ServiceName}";
            try
            {
                var timeout = TimeSpan.FromSeconds(3);
                await StopService(service, timeout);
            }
            catch (Exception e)
            {
                _logger.LogCritical($"Could not stop service {serviceId}, {e.Message}");
            }
        }

        private Task StopService(ServiceController serviceController, TimeSpan timeout, int retries = 3)
        {
            Policy
                .Handle<Win32Exception>()
                .Or<InvalidOperationException>()
                .WaitAndRetry(retries, _ => timeout)
                .Execute(
                    () =>
                    {
                        _logger.LogInformation(
                            $"Trying to execute action {nameof(StopServiceConsumer)}/{nameof(StopService)}");
                        serviceController.Refresh();
                        serviceController.Stop();
                        serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                    });
            return Task.CompletedTask;
        }
    }
}