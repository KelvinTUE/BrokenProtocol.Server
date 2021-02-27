using BrokenProtocol.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server.Data
{
    public class UserDevice : Device
    {
        private static readonly TimeSpan ACTIVITY_TIMEOUT = TimeSpan.FromSeconds(5);

        public DateTime LastActivity { get; set; }
        public override bool IsOnline => DateTime.Now.Subtract(LastActivity) < ACTIVITY_TIMEOUT;

        public string UserID { get; set; }

        public UserDevice() { }
        public UserDevice(User user)
        {
            UserID = user.ObjectID;
        }
    }
}
