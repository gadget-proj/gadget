using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Gadget.Common
{
    public interface IDispatch
    {
    }

    public interface ICommand : IDispatch
    {
    }

    public interface IEvent : IDispatch
    {
    }


    public interface IHandler<in T> where T : IDispatch
    {
        Task Handle(T t);
    }

    public interface IDispatcher
    {
        Task Send<TCommand>(TCommand command) where TCommand : IDispatch;
    }

    public class Dispatcher : IDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public Dispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Send<TCommand>(TCommand command) where TCommand : IDispatch
        {
            var handler = _serviceProvider.GetService<IHandler<TCommand>>();
            if (handler is null)
            {
                throw new ApplicationException($"Cannot find handler for type {typeof(TCommand)}");
            }

            await handler.Handle(command);
        }
    }
}