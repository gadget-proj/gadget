using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Gadget.Auth.Domain
{
    public class User
    {
        public Guid Id { get; }
        public string UserName { get; }
        public string Password { get; }
        public string UserProvider { get; }

        private readonly ICollection<RefreshToken> _refreshTokens = new HashSet<RefreshToken>();
        public IEnumerable<RefreshToken> RefreshTokens => _refreshTokens.ToImmutableList();

        public User(string userName)
        {
            Id = Guid.NewGuid();
            UserName = userName;
        }

        public User(string userName, string password)
        {
            Id = Guid.NewGuid();
            UserName = userName;
            Password = password;
        }

        public void AddRefreshToken(RefreshToken refreshToken)
        {
            _refreshTokens.Add(refreshToken);
        }
    }
}