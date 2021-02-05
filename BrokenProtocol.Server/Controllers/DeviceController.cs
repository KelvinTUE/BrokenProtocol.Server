using BrokenProtocol.Server.Data;
using LogicReinc.Asp.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenProtocol.Server.Controllers
{
    [Route("[controller]/[action]")]
    public class DeviceController : ControllerBase
    { 
        [HttpGet]
        [Authorize]
        public void Heartbeat()
        {
            User user = (User)HttpContext.Items["Authentication"];
            user.UpdateActivity();
        }

        [HttpGet]
        [Authorize]
        public bool CanPickup()
        {
            User user = (User)HttpContext.Items["Authentication"];
            user.UpdateActivity();

            return user.Group?.CanPickup(user) ?? false;
        }

        [HttpPost]
        [Authorize]
        public void PickedUpObject()
        {
            User user = (User)HttpContext.Items["Authentication"];
            user.Device.TotalCount++;
            user.UpdateActivity();
        }

        [HttpPost]
        [Authorize]
        public void DeterminedObjectColor(ObjectColor color)
        {
            User user = (User)HttpContext.Items["Authentication"];
            user.Device.IncrementColor(color);
            user.UpdateActivity();
        }


        [HttpGet]
        [Authorize]
        public List<UserDevice> GetOnlineDevices()
        {
            return Data.User.Database.Where(x => x.IsOnline).Select(x=>x.GetDevice()).ToList();
        }
    }
}
