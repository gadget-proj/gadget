using System;

namespace Gadget.Inspector.Commands
{
    public static class CommandFactory
    {
        public static ICommand Create(CommandAction action)
        {
            switch (action)
            {
                case CommandAction.Create:
                    return new CreateCommand();
                case CommandAction.Delete:
                    return new DeleteCommand();
                case CommandAction.Display:
                    return new DisplayCommand();
                case CommandAction.Restart:
                    return new RestartCommand();
                default:
                    throw new ApplicationException("Uknown CommandAction command");
            }
        }
    }
}
