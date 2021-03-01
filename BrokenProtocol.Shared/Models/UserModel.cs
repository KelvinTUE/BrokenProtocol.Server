using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Shared.Models
{
    public class UserModel
    {
        public string ObjectID { get; set; }
        public string Name { get; set; }
        public string DeviceName { get; set; }
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public int[] Activity { get; set; }
    }
}
