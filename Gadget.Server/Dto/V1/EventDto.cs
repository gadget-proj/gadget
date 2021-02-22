using System;

namespace Gadget.Server.Dto.V1
{
    public record EventDto (string Agent, string Service, DateTime CreatedAt, string Status);
}