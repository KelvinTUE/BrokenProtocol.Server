using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Shared.Models
{
    public class UserLogModel
    {
        public const string TYPE_Request = "Request";

        public string UserID { get; set; }
        public Log Log { get; set; }

    }
}
