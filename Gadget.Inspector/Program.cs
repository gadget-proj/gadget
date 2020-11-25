using System.Reflection;
using Gadget.Inspector.Extensions;
using Gadget.Inspector.HandlerRegistration;
using Gadget.Messaging.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            //var tmp = new RegisterHandlers(null);
            //tmp.Register();
            var handler = new MainHandler();
            handler.ProcessEvent(new StartService { Agent = "Lucek", ServiceName = "LucekService" });
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMediatR(Assembly.GetExecutingAssembly());
                    services.AddLogging(options => options.AddConsole());
                    services.AddInspector();
                });
        }
    }
}