using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server.Websockets.Packets
{
    public class ManagementMessage
    {
        public string Type { get; set; }
        public object Data { get; set; }


        public static ManagementMessage FromObject(object obj)
        {
            return new ManagementMessage()
            {
                Type = obj.GetType().Name,
                Data = obj
            };
        }
    }
}
