using System;
using System.Threading.Tasks;

namespace Gadget.Inspector
{
    class Program
    {
        private static async Task Main()
        {
            var inspector = new Inspector(new Uri("http://localhost:5000/gadget"));
            await inspector.Start();
            Console.WriteLine("Inspector started");
            Console.ReadKey();
        }
    }
}