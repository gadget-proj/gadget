using System.Collections.Generic;

namespace Gadget.Server.Dto.V1.Requests
{
    public record ActionRequest (string Event, string Command);
    public record ApplyConfigRequest(IEnumerable<ConfigRequest> Rules);
    public record ConfigRequest (string Selector, IEnumerable<ActionRequest> Actions);
}