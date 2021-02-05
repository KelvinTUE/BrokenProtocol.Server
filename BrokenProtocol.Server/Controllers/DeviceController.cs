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

        /// <summary>
        /// Used solely to update online status.
        /// </summary>
        [HttpGet]
        [Authorize]
        public void Heartbeat()
        {
            User user = (User)HttpContext.Items["Authentication"];
            user.UpdateActivity();
        }


        /// <summary>
        /// Checks if the current user can pick up the next object it encounters
        /// </summary>
        /// <returns>If can encounter</returns>
        [HttpGet]
        [Authorize]
        public bool CanPickup()
        {
            User user = (User)HttpContext.Items["Authentication"];
            user.UpdateActivity();

            return user.Group?.CanPickup(user) ?? false;
        }

        /// <summary>
        /// Inform the server that the machine picked up an item
        /// </summary>
        [HttpPost]
        [Authorize]
        public void PickedUpObject()
        {
            User user = (User)HttpContext.Items["Authentication"];
            user.Device.TotalCount++;
            user.UpdateActivity();
        }

        /// <summary>
        /// Inform the server that it identified an item with a given color
        /// </summary>
        /// <param name="color">Color it found (0=black, 1=white)</param>
        [HttpPost]
        [Authorize]
        public void DeterminedObjectColor(ObjectColor color)
        {
            User user = (User)HttpContext.Items["Authentication"];
            user.Device.IncrementColor(color);
            user.UpdateActivity();
        }


        /// <summary>
        /// Get all online devices
        /// </summary>
        /// <returns>All online devices</returns>
        [HttpGet]
        [Authorize]
        public List<UserDevice> GetOnlineDevices()
        {
            return Data.User.Database.Where(x => x.IsOnline).Select(x=>x.GetDevice()).ToList();
        }
    }
}
