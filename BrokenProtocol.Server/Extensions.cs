using BrokenProtocol.Server.Data;
using LogicReinc.Asp;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server
{
    public static class Extensions
    {
        public static User GetAuthenticatedUser(this HttpContext context)
        {
            return (User)context.Request.GetAuthentication().UserObject;
        }
    }
}
