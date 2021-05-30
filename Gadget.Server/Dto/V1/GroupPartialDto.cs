using System;
using System.Collections.Generic;

namespace Gadget.Server.Dto.V1
{
    public record GroupPartialDto(Guid Id, string Name);

    public record GroupDto(Guid Id, string Name, IEnumerable<ServiceDto> Services);
}