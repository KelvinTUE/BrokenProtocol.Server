using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenProtocol.Server.Data
{
    public class Device
    {
        public string UserID { get; set; }
        public string Name { get; set; }

        public int TotalCount = 0;
        public ConcurrentDictionary<ObjectColor, int> ObjectCounts { get; set; } = new ConcurrentDictionary<ObjectColor, int>();

        public Device() { }
        public Device(User user)
        {
            UserID = user.ObjectID;
        }

        public void Clear()
        {
            TotalCount = 0;
            ObjectCounts.Clear();
        }

        public void IncrementColor(ObjectColor color)
        {
            if (!ObjectCounts.ContainsKey(color))
                ObjectCounts.TryAdd(color, 0);
            ObjectCounts[color]++;
        }
    }
}
