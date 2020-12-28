using Gadget.Inspector.Transport;
using Gadget.Messaging.Commands;
using System;
using System.Linq;
using System.Reflection;

namespace Gadget.Inspector.HandlerRegistration
{
    public class RegisterHandlers
    {
        private readonly IControlPlane _controlPlane;

        public RegisterHandlers(IControlPlane controlPlane)
        {
            _controlPlane = controlPlane;
        }

        public void Register()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterface(typeof(IHandler<>).Name) != null);

            foreach (var t in types)
            {
                var methodName = t.Name.Replace("Handler", "");

                _controlPlane.RegisterHandler<IGadgetMessage>(methodName, message =>
                {
                    MethodInfo methodInfo = t.GetMethod(methodName);
                    var inst = Activator.CreateInstance(t);
                    try
                    {
                        methodInfo.Invoke(inst, new object[] { message });
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e.Message);
                    }
                });
            }
        }
    }
}
