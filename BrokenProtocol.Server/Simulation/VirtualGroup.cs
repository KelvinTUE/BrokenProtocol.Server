using BrokenProtocol.Server.Data;
using BrokenProtocol.Server.Rules;
using BrokenProtocol.Server.Simulation;
using BrokenProtocol.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenProtocol.Server.Simulation
{
    public class VirtualGroup : IGroup
    {
        public string ObjectID => "Virtual";

        private User _user = null;
        public DateTime Expiration { get; set; }

        public string Name => "Virtual";

        public VirtualGroupOptions Options { get; set; }

        public Device Device => _user?.Device;
        public List<FakeDevice> FakeDevices { get; set; } = new List<FakeDevice>();

        public List<Device> Devices => FakeDevices.Cast<Device>().Concat(new Device[] { Device }).ToList();

        public bool IsVirtual => true;

        private IGroupRules Rules = new BasicRules();

        public VirtualGroup(User user, VirtualGroupOptions options, TimeSpan validity)
        {
            _user = user;
            Options = options;
            FakeDevices.Add(new FakeDevice("Virtual1", this));
            FakeDevices.Add(new FakeDevice("Virtual2", this));
            FakeDevices.Add(new FakeDevice("Virtual3", this));
            Expiration = DateTime.Now.Add(validity);
        }

        public void DoEvents()
        {
            foreach(FakeDevice device in FakeDevices)
                device.DoEvents();
        }

        public bool CanPickup(Device device)
        {
            List<Device> devices = FakeDevices.Cast<Device>().ToList();
            devices.Add(Device);
            return Rules.CanPickup(devices, device);
        }
    }
}
