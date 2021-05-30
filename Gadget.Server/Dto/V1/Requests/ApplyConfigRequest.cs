using System;
using System.Collections.Generic;

namespace Gadget.Server.Dto.V1.Requests
{
    public record ActionRequest (Guid Id, string Event, string Command);

    public record ApplyConfigRequest(IEnumerable<ConfigRequest> Rules);

    public record ConfigRequest (string Selector, IEnumerable<ActionRequest> Actions);
}