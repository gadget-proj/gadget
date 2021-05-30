using System;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Gadget.Messaging.Contracts.Commands.v1;
using Gadget.Messaging.Contracts.Events.v1;
using Gadget.Messaging.Contracts.Responses;
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
            var serviceNormalizedName = context.Message.ServiceName.Trim().ToLower();
            _logger.LogInformation($"Trying to start {serviceNormalizedName}");
            var service = ServiceController
                .GetServices()
                .FirstOrDefault(s => s.ServiceName.ToLower().Trim() == serviceNormalizedName);
            if (service is null)
            {
                throw new ApplicationException($"Service {serviceNormalizedName} could not be found");
            }

            if (service.Status == ServiceControllerStatus.Running)
            {
                await context.Publish<IActionResultResponse>(new
                {
                    context.CorrelationId, Success = false
                });
                return;
            }

            var timeout = TimeSpan.FromSeconds(5);
            try
            {
                var (success, error) = await StartService(service, timeout);
                await context.Publish<IActionResultResponse>(new
                {
                    context.CorrelationId, Success = success, Reason = error
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.GetType().ToString());
                _logger.LogError(e.Message);
                await context.Publish<IActionResultResponse>(new
                {
                    context.CorrelationId, Success = false, Reason = e.Message
                });
            }
        }

        private Task<Result> StartService(ServiceController serviceController, TimeSpan timeout, int retries = 3)
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
                                $"Trying to execute action {nameof(StartServiceConsumer)}/{nameof(StartService)}");
                            serviceController.Start();
                            serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
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