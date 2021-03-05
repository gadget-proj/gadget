using System;
using System.Collections.Generic;

namespace Gadget.Server.Domain.Entities
{
    public class User
    {
        public Guid Id { get;}
        public string UserName { get;}
        public string UserProvider { get;}

        public List<RefreshToken> RefreshTokens { get; set; }

        public User(string userName)
        {
            Id = Guid.NewGuid();
            UserName = userName;
        }

        public void AddRefreshToken(RefreshToken refreshToken)
        {
            RefreshTokens.Add(refreshToken);
        }
    }
}
