namespace Gadget.Server.Authorization.Requests
{
    public class RefreshRequest
    {
        public string UserName { get; set; }

        public string Token { get; set; }
    }
}
