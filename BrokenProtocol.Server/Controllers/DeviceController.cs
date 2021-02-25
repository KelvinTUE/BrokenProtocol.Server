﻿using BrokenProtocol.Server.Data;
using BrokenProtocol.Server.Data.Models;
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
            User user = HttpContext.GetAuthenticatedUser();
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
            User user = HttpContext.GetAuthenticatedUser();
            user.UpdateActivity();

            return user.Group?.CanPickup(user) ?? false;
        }

        /// <summary>
        /// Inform the server that the machine picked up an item
        /// </summary>
        [HttpPost]
        [Authorize]
        public bool PickedUpObject()
        {
            User user = HttpContext.GetAuthenticatedUser();

            if (string.IsNullOrEmpty(user.GroupID) || user.Group == null)
                return false;

            bool allowed = user.Group?.CanPickup(user) ?? false;

            user.Device.TotalCount++;
            user.UpdateActivity();

            return allowed;
        }

        /// <summary>
        /// Inform the server that the machine put back an item on the feeding belt.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public bool PutBackObject()
        {
            User user = HttpContext.GetAuthenticatedUser();

            if (string.IsNullOrEmpty(user.GroupID) || user.Group == null)
                return false;

            user.Device.TotalCount--;
            user.UpdateActivity();

            return user.Device.TotalCount >= 0;
        }


        /// <summary>
        /// Inform the server that it identified an item with a given color
        /// </summary>
        /// <param name="color">Color it found (0=black, 1=white)</param>
        [HttpPost]
        [Authorize]
        public void DeterminedObject([FromBody]ObjectData data)
        {
            User user = HttpContext.GetAuthenticatedUser();
            user.Device.IncrementColor(data.Color);
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

        /// <summary>
        /// Saves the provided dictionary as sensordata for this device
        /// </summary>
        /// <param name="sensorData">Data to save to this device</param>
        /// <returns>The currently saved sensordata</returns>
        [HttpPost]
        [Authorize]
        public bool Log([FromBody]Log log)
        {
            User user = HttpContext.GetAuthenticatedUser();
            user.Log(log);
            return true;
        }

        /// <summary>
        /// Saves the provided dictionary as sensordata for this device
        /// </summary>
        /// <param name="sensorData">Sensordata of device</param>
        /// <returns>true</returns>
        [HttpPost]
        [Authorize]
        public bool SensorData([FromBody] SensorData sensorData)
        {
            User user = HttpContext.GetAuthenticatedUser();
            user.SensorData(sensorData);
            user.UpdateActivity();
            return true;
        }
    }
}
