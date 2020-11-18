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

                //MethodInfo methodInfo = t.GetMethod(methodName);
                //var inst = Activator.CreateInstance(t);
                //methodInfo.Invoke(inst, null);

                _controlPlane.RegisterHandler<IGadgetMessage>(methodName, _ =>
                {
                    MethodInfo methodInfo = t.GetMethod("Execute");
                    var inst = Activator.CreateInstance(t);
                    methodInfo.Invoke(inst, null);
                });
            }
        }
    }
}
