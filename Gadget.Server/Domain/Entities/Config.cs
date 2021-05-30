using System;
using System.Collections.Generic;
using System.Linq;
using Gadget.Server.Domain.Enums;
using Gadget.Server.Dto.V1.Requests;
using Action = Gadget.Server.Domain.Enums.Action;

namespace Gadget.Server.Domain.Entities
{
    public record Config
    {
        public Config(IEnumerable<ActionRequest> actions)
        {
            _actions = actions.Select(a => a).ToHashSet();
        }

        private Config()
        {
        }

        public Guid Id { get; private set; }
        private readonly HashSet<ActionRequest> _actions = new();
        public IEnumerable<ActionRequest> Actions => _actions.ToList();

        public Action Action(ServiceStatus @event) =>
            Enum.Parse<Action>(_actions.First(a => a.Event == @event.ToString()).Command);
    }
}