using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandParser
{
    public class CmdHandler
    {
        public static string CmdExecute(string packet, CmdHandleMethod method)
        {
            CmdResponsePacket response;
            method(packet, out response);
            
            return JsonConvert.SerializeObject(response);
        }

        public delegate void CmdHandleMethod(string data, out CmdResponsePacket response);
    }
}
