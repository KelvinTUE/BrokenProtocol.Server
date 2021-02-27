using BrokenProtocol.Server.Data;
using LogicReinc.Asp.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server.Controllers
{
    [Route("[controller]/[action]")]
    public class SimulationController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public bool CreateVirtualGroup()
        {
            User user = HttpContext.GetAuthenticatedUser();

            user.CreateVirtualGroup(TimeSpan.FromMinutes(15));

            return true;
        }
        [HttpPost]
        [Authorize]
        public bool DeleteVirtualGroup()
        {
            User user = HttpContext.GetAuthenticatedUser();

            user.DeleteVirtualGroup();

            return true;
        }
    }
}
