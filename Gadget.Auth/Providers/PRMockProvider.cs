using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Gadget.Auth.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gadget.Auth.Providers
{
    public class PrMockProvider : ILoginProvider
    {
        private readonly AuthContext _context;

        public PrMockProvider(AuthContext context)
        {
            _context = context;
        }

        public async Task<bool> PasswordValid(string userName, string password)
        {
            var user = await _context.Users.SingleAsync(u => u.UserName == userName);

            var matches = BCrypt.Net.BCrypt.Verify(password, user.Password);
            return matches;
        }


        private string HashMd5(string value)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x =
                new System.Security.Cryptography.MD5CryptoServiceProvider();
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