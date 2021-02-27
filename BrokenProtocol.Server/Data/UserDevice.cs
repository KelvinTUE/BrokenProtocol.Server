using BrokenProtocol.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server.Data
{
    public class UserDevice : Device
    {
        public string UserID { get; set; }

        public UserDevice() { }
        public UserDevice(User user)
        {
            UserID = user.ObjectID;
        }
    }
}
