﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Shared.Models
{
    public class Log
    { 
        public string[] Tags { get; set; } = new string[0];
        public string Message { get; set; }
    }
}
