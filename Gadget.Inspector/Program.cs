using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Gadget.Inspector
{
    class Program
    {
        private static async Task Main()
        {
            var l = new LoggerFactory();
            var logger = l.CreateLogger<Inspector>();
            var inspector = new Inspector(new Uri("https://webscoket.noinputsignal.com/gadget/gadget"), logger);
            // var inspector = new Inspector(new Uri("https://unfold.azurewebsites.net/gadget"), logger);
            // var inspector = new Inspector(new Uri("http://localhost:5000/gadget"), logger);
            await inspector.Start();
            Console.WriteLine("Inspector started");
            Console.ReadKey();    
        }
    }
}