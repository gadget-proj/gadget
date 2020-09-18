using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gadget.Inspector
{

    class Program
    {
        private static void Main()
        {
            var services = new List<Service>();
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    foreach (var service in services)
                    {
                        service.Refresh();
                    }
                    await Task.Delay(5000);
                }
            });
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(">");
                Console.ForegroundColor = ConsoleColor.White;
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }

                var command = new Command(input);
                command.Execute(services);
            }
        }
    }
}