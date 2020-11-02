using System;
using System.Threading.Tasks;
using Autofac;
using Gadget.Inspector.Services;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector
{
    class Program
    {
        private static async Task Main()
        {
            var l = new LoggerFactory();
            var logger = l.CreateLogger<Inspector>();
            // container and registration
            //var builder = new ContainerBuilder();
            //builder.RegisterType<MachineHealthService>().AsImplementedInterfaces();
            //var container = builder.Build();


            // var inspector = new Inspector(new Uri("https://webscoket.noinputsignal.com/gadget/gadget"), logger);
            // var inspector = new Inspector(new Uri("https://unfold.azurewebsites.net/gadget"), logger);
            var inspector = new Inspector(new Uri("http://localhost:4448/gadget"), logger);
            await inspector.Start();
            Console.WriteLine("Inspector started");
            Console.ReadKey();
        }
    }
}