using System;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Commands.v1;
using Gadget.Messaging.Contracts.Events.v1;
using MassTransit;
using Microsoft.Extensions.Logging;
using Polly;

namespace Gadget.Inspector.Consumers
{
    public class StartServiceConsumer : IConsumer<IStartService>
    {
        private readonly ILogger<StartServiceConsumer> _logger;

        public StartServiceConsumer(ILogger<StartServiceConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IStartService> context)
        {
            _logger.LogInformation($"Trying to start {context.Message.ServiceName}");
            var service = ServiceController.GetServices()
                .FirstOrDefault(s => s.ServiceName == context.Message.ServiceName);
            if (service is null)
            {
                throw new ApplicationException($"Service {context.Message.ServiceName} could not be found");
            }

            var timeout = TimeSpan.FromSeconds(5);
            try
            {
                await StartService(service, timeout);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                await context.Publish<IActionFailed>(new
                {
                    context.CorrelationId,
                    Agent = Environment.MachineName,
                    Service = service.ServiceName,
                    Action = nameof(StartServiceConsumer),
                    Reason = e.Message,
                    Date = DateTime.UtcNow
                });
            }
        }

        private static Task StartService(ServiceController serviceController, TimeSpan timeout, int retries = 3)
        {
            Policy.Handle<Win32Exception>().WaitAndRetry(retries, _ => timeout).Execute(() =>
            {
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
            });
            return Task.CompletedTask;
        }
    }
}