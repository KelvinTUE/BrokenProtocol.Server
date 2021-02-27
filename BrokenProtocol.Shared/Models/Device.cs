using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrokenProtocol.Shared.Models;

namespace BrokenProtocol.Shared.Models
{
    public class Device
    {
        public string Name { get; set; }

        public int TotalCount { get; set; }
        public ConcurrentDictionary<string, int> ObjectCounts { get; set; } = new ConcurrentDictionary<string, int>();

        public Device() { }

        public void Clear()
        {
            TotalCount = 0;
            ObjectCounts.Clear();
        }

        public void IncrementColor(ObjectColor color)
        {
            string name = color.ToString();

            if (!ObjectCounts.ContainsKey(name))
                ObjectCounts.TryAdd(name, 0);
            ObjectCounts[name]++;
        }
    }
}
