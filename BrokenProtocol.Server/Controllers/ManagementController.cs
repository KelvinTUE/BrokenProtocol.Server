using BrokenProtocol.Server.Data;
using BrokenProtocol.Shared.Models;
using LogicReinc.Asp.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server.Controllers
{
    [Route("[controller]/[action]")]
    public class ManagementController : ControllerBase
    {
        #region Users
        [HttpGet]
        [Authorize("admin")]
        public List<UserModel> Users()
        {
            return Data.User.Database.Select(x => x.ToModel());
        }

        [HttpPost]
        [Authorize("admin")]
        public bool CreateUser([FromBody] UserCreateModel model)
        {
            User user = new User()
            {
                Username = model.Username,
                Device = new UserDevice()
                {
                    Name = model.DeviceName,
                },
                Password = model.Password
            };
            user.Insert();
            user.Device.UserID = user.ObjectID;
            user.Update();
            return true;
        }

        [HttpPost]
        public bool DeleteUser(string id)
        {
            User user = Data.User.GetObject(id);
            if (user == null)
            {
                base.StatusCode(404);
                return false;
            }
            user.Delete();
            return true;
        }
        #endregion

        #region Groups
        [HttpGet]
        [Authorize("admin")]
        public List<UserDeviceGroup> Groups()
        {
            return UserDeviceGroup.Database.ToList();
        }

        [HttpPost]
        [Authorize("admin")]
        public bool CreateGroup([FromBody]GroupCreateModel model)
        {
            UserDeviceGroup group = new UserDeviceGroup();
            group.Name = model.Name;
            group.Insert();
            return true;
        }
        [HttpPost]
        [Authorize("admin")]
        public bool DeleteGroup(string id)
        {
            UserDeviceGroup group = UserDeviceGroup.GetObject(id);
            if(group == null)
            {
                StatusCode(404);
                return false;
            }
            group.Delete();
            return true;
        }

        [HttpPost]
        [Authorize("admin")]
        public UserDeviceGroup AddUserToGroup(string userid, string groupid)
        {
            Data.User user = Data.User.GetObject(userid);
            Data.UserDeviceGroup group = Data.UserDeviceGroup.GetObject(groupid);

            if (user == null || group == null)
            {
                StatusCode(400);
                throw new ArgumentException("User or group is null");
            }

            if (group.Devices.Count >= 4)
            {
                StatusCode(400);
                throw new InvalidOperationException("Not allowed to have more than 4 users");
            }

            user.GroupID = group.ObjectID;
            user.Update();

            return group;
        }

        [HttpPost]
        [Authorize("admin")]
        public UserDeviceGroup RemoveUserFromGroup(string userid, string groupid)
        {
            Data.User user = Data.User.GetObject(userid);
            Data.UserDeviceGroup group = Data.UserDeviceGroup.GetObject(groupid);

            if (user == null || group == null)
            {
                StatusCode(400);
                throw new ArgumentException("User or group is null");
            }

            if (group.Devices.Count >= 4)
            {
                StatusCode(400);
                throw new InvalidOperationException("Not allowed to have more than 4 users");
            }

            if (group.Users.FirstOrDefault(x => x.ObjectID == userid) == null)
            {
                StatusCode(400);
                throw new ArgumentException("User not part of group");
            }

            user.GroupID = null;
            user.Update();

            return group;
        }
        #endregion
    }
}
