using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class DataPacket
    {
        public PagePacket PagePacket { get; set; }
        public object Data { get; set; }
    }

    public class PagePacket
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int PageTotal { get; set; }
        public int DataTotal { get; set; }

        public PagePacket(int i, int s, int pt, int dt) 
        {
            this.PageIndex = i;
            this.PageSize = s;
            this.PageTotal = pt;
            this.DataTotal = dt;
        }
    }
}
