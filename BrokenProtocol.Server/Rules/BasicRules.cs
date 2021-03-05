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
            List<Device> applicable = devices.Where(x => x.IsOnline).ToList();
            if (applicable.Count == 0)
                return true;

            int min = applicable.Min(x => x.TotalCount);

            return device.TotalCount == min;
        }
    }
}
