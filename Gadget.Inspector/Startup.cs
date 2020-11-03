using System;
using System.Threading.Tasks;

namespace Gadget.Inspector
{
    class Startup
    {
        private readonly Inspector _inspector;

        public Startup(Inspector inspector)
        {
            _inspector = inspector;
        }

        internal async Task Run()
        {
            await _inspector.Start();
            Console.WriteLine("Inspector started");
            Console.ReadKey();
        }
    }
}
