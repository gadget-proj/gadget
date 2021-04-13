using System.Collections.Generic;
using System.Text;

namespace Gadget.Auth.Providers
{
    public class PrMockProvider : ILoginProvider
    {
        private readonly Dictionary<string, string> _users;

        public PrMockProvider()
        {
            _users = new Dictionary<string, string> { 
                { "test", "5a105e8b9d40e1329780d62ea2265d8a" },
                {"lucek", "ff29a82b9ea498210089965fb5216806" } };
        }
        public bool PasswordValid(string userName, string password)
        {
            _users.TryGetValue(userName, out string  passwordHashed);
            return passwordHashed == HashMd5(password);
        }


        private string HashMd5(string value)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.ASCII.GetBytes(value);
            data = x.ComputeHash(data);

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
