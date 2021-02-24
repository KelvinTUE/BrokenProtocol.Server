using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server.Simulation
{
    public class FakeDevice
    {
        public string Name { get; set; }

        public TimeSpan ObjectInterval { get; set; } = TimeSpan.FromSeconds(4);

        public TimeSpan DetermineTime { get; set; } = TimeSpan.FromSeconds(2);


        public void DoEvents()
        {

        }
    }
}
