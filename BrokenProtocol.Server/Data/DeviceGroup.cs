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
    public class DeviceGroup : UnifiedIMObject<DeviceGroup>
    {
        public string Name { get; set; }
        public List<Device> Devices => Users.Select(x => x.Device).ToList();

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [UnifiedIMReference(nameof(ObjectID), typeof(User), nameof(User.GroupID))]
        public TSList<User> Users { get; set; } = new TSList<User>();



        public bool CanPickup(User user)
        {
            if (Users.Length == 0)
                return false;

            int min = Users.Min(x => x.Device.TotalCount);

            return user.Device.TotalCount == min;
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
