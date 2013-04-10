using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Protocol.Http.Protocol;
using Griffin.Networking.Protocol.Http.Services.Authentication;

namespace Griffin.Networking.Protocol.Http.DemoServer.Basic
{
    class DummyAccountRepository : IRealmRepository 
    {
        public string GetRealm(IRequest request)
        {
            return "MyRealm";
        }
    }

    class DummyUserStorage : IAccountStorage
    {
        public IAuthenticationUser Lookup(string userName, Uri host)
        {
            return new User
                {
                    Password = "admin",
                    Username = "admin"
                };
        }

        class User : IAuthenticationUser
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string HA1 { get; set; }
        }
    }
}
