using BrokenProtocol.Server.Data;
using BrokenProtocol.Server.Simulation;
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
        public bool CreateVirtualGroup([FromBody]VirtualGroupOptions options)
        {
            User user = HttpContext.GetAuthenticatedUser();

            if (options == null)
                options = new VirtualGroupOptions();

            user.CreateVirtualGroup(options, TimeSpan.FromMinutes(15));

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
