using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandParser
{
    public class CmdResponsePacket
    {
        public bool success { get; set; }
        public int code { get; set; }
        public object data { get; set; }
        public string message { get; set; }
    }

    public enum CmdResponseMessage
    {
        Success = 200,
        Failed = 500,
    }
}
