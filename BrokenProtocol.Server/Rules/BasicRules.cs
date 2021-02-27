using BrokenProtocol.Server.Data;
using BrokenProtocol.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenProtocol.Server.Rules
{
    public class BasicRules : IGroupRules
    {
        public bool CanPickup(List<Device> devices, Device device)
        {
            int min = devices.Min(x => x.TotalCount);

            return device.TotalCount == min;
        }
    }
}
