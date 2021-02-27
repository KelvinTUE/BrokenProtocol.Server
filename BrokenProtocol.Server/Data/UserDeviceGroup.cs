using BrokenProtocol.Server.Rules;
using BrokenProtocol.Shared.Models;
using LogicReinc.Collections;
using LogicReinc.Data.Unified;
using LogicReinc.Data.Unified.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace BrokenProtocol.Server.Data
{
    [UnifiedCollection("DeviceGroups")]
    public class UserDeviceGroup : UnifiedIMObject<UserDeviceGroup>, IGroup
    {
        public string Name { get; set; }
        public List<Device> Devices => Users.Select(x => (Device)x.Device).ToList();
        public List<UserDevice> UserDevices => Users.Select(x => x.Device).ToList();

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [UnifiedIMReference(nameof(ObjectID), typeof(User), nameof(User.GroupID))]
        public TSList<User> Users { get; set; } = new TSList<User>();

        public bool IsVirtual => false;

        private IGroupRules _rules = new BasicRules();

        public bool CanPickup(Device device)
        {
            if (Users.Length == 0)
                return false;
            if (!Devices.Contains(device))
                return false;

            return _rules.CanPickup(Devices.Cast<Device>().ToList(), device);
        }

        public override bool Delete()
        {
            foreach(User user in Users)
            {
                user.GroupID = null;
                user.Update();
            }
            return base.Delete();
        }
    }
}
