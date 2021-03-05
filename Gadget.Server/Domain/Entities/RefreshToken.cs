using System;

namespace Gadget.Server.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get;}
        public string Token { get;}
        public DateTime CreateDate { get;}
        public DateTime ExpireDate { get;}
        public User User { get;}
        public Guid UserId { get;}

        public bool Used { get; set; }
        public bool Unvalidated { get; set; }

        public RefreshToken(Guid userId, string token)
        {
            Id = Guid.NewGuid();
            Token = token;
            UserId = userId;
            CreateDate = DateTime.UtcNow;
            ExpireDate = DateTime.UtcNow.AddMinutes(10);
            Used = false;
            Unvalidated = false;
        }

    }
}
