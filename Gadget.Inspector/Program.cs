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
            var inspector = new Inspector(new Uri("https://localhost:44347/gadget"), logger);
            await inspector.Start();
            Console.WriteLine("Inspector started");
            Console.ReadKey();
        }
    }
}