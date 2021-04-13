namespace Gadget.Auth.Providers
{
    public interface ILoginProvider
    {
        // TODO add options or sth to manage provider settings like hash algorithm, adresses etc.
        // TODO add enum provider type and factory to resolve multi providers by its type
        bool PasswordValid(string userName, string password);
    }
}
