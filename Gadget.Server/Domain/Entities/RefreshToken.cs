using System;
using System.Net;

namespace Gadget.Server.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get;}
        public string Token { get;}
        public DateTime CreateDate { get;}
        public DateTime ExpireDate { get;}
        public User User { get;}
        //public Guid UserId { get;}
        public bool Used { get;  private set; }
        public string IpAddress { get; }
        public bool Unvalidated { get; private set; }

        private RefreshToken()
        {
        }

        public RefreshToken(User user, string token, string ipAddress)
        {
            Token = token;
            User = user;
            CreateDate = DateTime.UtcNow;
            ExpireDate = DateTime.UtcNow.AddMinutes(10);
            Used = false;
            Unvalidated = false;
            IpAddress = ipAddress;
        }

        public void Use()
        {
            Used = true;
        }

        public void Unvalidate()
        {
            Unvalidated = true;
        }


        public bool IsExpired => DateTime.UtcNow >= ExpireDate;

        public bool IsActive => !Unvalidated && !IsExpired;

        public bool IsTokenIpValid(string ipAddress)
        {
            IPAddress tokenIp = IPAddress.Parse(IpAddress);
            IPAddress refreshIp = IPAddress.Parse(ipAddress);
            return tokenIp.Equals(refreshIp);
        }
    }
}
