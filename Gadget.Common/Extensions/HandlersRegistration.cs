using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Gadget.Common.Extensions
{
    public static class HandlersRegistration
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services, Assembly assembly)
        {
            services.Scan(x =>
            {
                x.FromAssemblies(assembly)
                    .AddClasses(classes => classes.AssignableTo(typeof(IHandler<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime();
            });

            return services;
        }
    }
}