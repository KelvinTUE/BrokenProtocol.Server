using BrokenProtocol.Server.Data;
using LogicReinc.Asp.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server
{
    public class AuthService : AuthenticationService<User>
    {
        public override TimeSpan Expires => TimeSpan.FromDays(7);

        public override IAuthUser AuthenticateAuthUser(string user, string pass)
        {
            return User.Login(user, pass);
        }

        public override IAuthUser GetAuthUser(string id)
        {
            return User.GetObject(id);
        }
    }
}
