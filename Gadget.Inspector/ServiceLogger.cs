using System;
using System.ServiceProcess;

namespace Gadget.Inspector
{
    public static class ServiceLogger
    {
        private static void SetLogColor(ServiceControllerStatus status)
        {
            switch (status)
            {
                case ServiceControllerStatus.Stopped:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case ServiceControllerStatus.StartPending:
                    break;
                case ServiceControllerStatus.StopPending:
                    break;
                case ServiceControllerStatus.Running:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case ServiceControllerStatus.ContinuePending:
                    break;
                case ServiceControllerStatus.PausePending:
                    break;
                case ServiceControllerStatus.Paused:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        public static void Log(string value, ServiceControllerStatus status)
        {
            SetLogColor(status);
            Console.WriteLine(value);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}