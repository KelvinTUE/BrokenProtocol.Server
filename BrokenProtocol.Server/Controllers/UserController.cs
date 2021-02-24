using BrokenProtocol.Server.Data;
using LogicReinc.Asp;
using LogicReinc.Asp.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server.Controllers
{
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {

        /// <summary>
        /// Returns basic user information
        /// </summary>
        /// <returns>Basic user data</returns>
        [HttpGet]
        [Authorize]
        public UserData GetUserData()
        {
            User user = HttpContext.GetAuthenticatedUser();

            return new UserData()
            {
                Name = user.Username,
                DeviceName = user.Device.Name
            };
        }
    }

    public class UserData
    {
        public string Name { get; set; }
        public string DeviceName { get; set; }
    }
}
