using System;
using Gadget.Server.Domain.Enums;

namespace Gadget.Server.Dto.V1
{
    public record ActionDto(Guid Id, Guid CorrelationId, DateTime Date, string Result, string Reason);

}