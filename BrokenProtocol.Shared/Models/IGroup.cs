using BrokenProtocol.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server.Data
{
    public interface IGroup
    {
        string ObjectID { get; }

        bool CanPickup(Device device);

        string Name { get; }

        bool IsVirtual { get; }

        List<Device> Devices { get; }
    }
}
