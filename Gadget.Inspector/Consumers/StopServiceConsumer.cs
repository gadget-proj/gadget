using System;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Commands.v1;
using Gadget.Messaging.Contracts.Responses;
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
            var serviceNormalizedName = context.Message.ServiceName.Trim().ToLower();
            _logger.LogInformation($"Trying to stop {serviceNormalizedName}");
            var service = ServiceController
                .GetServices()
                .FirstOrDefault(s => s.ServiceName.ToLower().Trim() == serviceNormalizedName);

            if (service is null)
            {
                throw new ApplicationException($"Service {serviceNormalizedName} could not be found");
            }

            if (service.Status == ServiceControllerStatus.Stopped)
            {
                await context.Publish<IActionResultResponse>(new
                {
                    context.CorrelationId, Success = false, Reason = "Service is not running"
                });
                return;
            }

            var serviceId = $"{context.Message.Agent}/{serviceNormalizedName}";
            try
            {
                var timeout = TimeSpan.FromSeconds(3);
                var (success, error) = await StopService(service, timeout);
                await context.Publish<IActionResultResponse>(new
                {
                    context.CorrelationId, Success = success, Reason = error
                });
            }
            catch (Exception e)
            {
                _logger.LogCritical($"Could not stop service {serviceId}, {e.Message}");
                await context.Publish<IActionResultResponse>(new
                {
                    context.CorrelationId, Success = false, Reason = e.Message
                });
            }
        }

        private Task<Result> StopService(ServiceController serviceController, TimeSpan timeout, int retries = 3)
        {
            try
            {
                Policy
                    .Handle<Win32Exception>()
                    .Or<InvalidOperationException>()
                    .WaitAndRetry(retries, _ => timeout)
                    .Execute(
                        () =>
                        {
                            _logger.LogInformation(
                                $"Trying to execute action {nameof(StartServiceConsumer)}/{nameof(StopService)}");
                            serviceController.Refresh();
                            serviceController.Stop();
                            serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                        });
                return Task.FromResult(new Result(true, ""));
            }
            catch (Exception e)
            {
                return Task.FromResult(new Result(false, e.Message));
            }
        }
    }
}