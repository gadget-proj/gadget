using System;
using Gadget.Server.Domain.Enums;

namespace Gadget.Server.Domain.Entities
{
    public record UserAction (Guid Id, Guid CorrelationId, DateTime Date, ActionResult Result, string Reason);
}