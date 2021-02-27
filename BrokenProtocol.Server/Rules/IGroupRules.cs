using BrokenProtocol.Server.Data;
using BrokenProtocol.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server.Rules
{
    public interface IGroupRules
    {
        bool CanPickup(List<Device> devices, Device device);
    }
}
