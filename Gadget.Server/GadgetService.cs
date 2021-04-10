using System.Threading.Tasks;
using GadgetRPC;
using Grpc.Core;

namespace Gadget.Server
{
    public class GadgetService : GadgetRPC.Gadget.GadgetBase
    {
        public override Task<TestResponse> Test(TestRequest request, ServerCallContext context)
        {
            return Task.FromResult<TestResponse>(new TestResponse
            {
                Message = "hi there"
            });
        }
    }
}