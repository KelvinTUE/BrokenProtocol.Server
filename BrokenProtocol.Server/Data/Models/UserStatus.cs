using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server.Data.Models
{
    public class UserStatus
    {
        public bool Online { get; set; }
        public DateTime LastActivity { get; set; }
    }
}
