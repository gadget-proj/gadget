using Gadget.Messaging.Commands;
using System;
using System.Linq;
using System.Reflection;

namespace Gadget.Inspector.HandlerRegistration
{
    public class MainHandler
    {
        public void ProcessEvent(IGadgetMessage message)
        {
            try
            {
                var type = Assembly.GetExecutingAssembly().GetTypes()
               .FirstOrDefault(x => x.Name == message.GetType().Name  + "Handler");

                if (type == null) return;

                MethodInfo methodInfo = type.GetMethod("Handle");
                var inst = Activator.CreateInstance(type);
                methodInfo.Invoke(inst, new object[] {message});
            }
            catch (System.Exception e)
            {
                Console.WriteLine("lipton");
                Console.WriteLine(e.Message);
                // logger.log ...
            }
        }
    }
}
