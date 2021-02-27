using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Shared.Models
{
    public class UserCreateModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string DeviceName { get; set; }
    }
}
