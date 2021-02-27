using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server.Simulation
{
    public class VirtualGroupOptions
    {
        public bool AllowOfflinePickup { get; set; }
        public int SecondsObjects { get; set; } = 3;
        public int SecondsPickup { get; set; } = 1;
        public int SecondsDetermine { get; set; } = 2;
        public int SecondsVariance { get; set; } = 1;
    }
}
