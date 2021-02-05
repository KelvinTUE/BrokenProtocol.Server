using BrokenProtocol.Server.Data;
using LogicReinc.Asp.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server
{
    public class AuthService : AuthenticationService
    {
        public override TimeSpan Expires => TimeSpan.FromDays(7);

        public override object Authenticate(string user, string pass)
        {
            return User.Login(user, pass);
        }

        public override string[] GetRoles(object obj)
        {
            return ((User)obj).Roles;
        }

        public override object GetUser(string id)
        {
            return User.GetObject(id);
        }

        public override string GetUserID(object obj)
        {
            return ((User)obj).ObjectID;
        }
    }
}
