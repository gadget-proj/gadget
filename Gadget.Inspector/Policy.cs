namespace Gadget.Inspector
{
    public class Policy
    {
        public PolicyAction Action { get; private set; }
        public void Execute (Service service)
        {
            switch (Action)
            {
                case PolicyAction.Ignore:
                    break;
                case PolicyAction.Restart:
                    service.Restart();
                    break;
                default:
                    break;
            }
        }
    }
}