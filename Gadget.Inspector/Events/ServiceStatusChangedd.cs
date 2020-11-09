using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Gadget.Inspector.Events
{
    public class ServiceStatusChanged : IRequest
    {
    }

    public class ServiceStatusChangedHandler : IRequestHandler<ServiceStatusChanged>
    {
        public async Task<Unit> Handle(ServiceStatusChanged request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}