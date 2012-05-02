using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.JsonRpc
{
    public class SimpleHeader
    {
        public byte Version { get; set; }
        public int Length { get; set; }
    }
}
