using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class DataField:Attribute
    {
        public string[] Tag { get; set; }
        public DataField(string[] tag)
        {
            Tag = tag;
        }
    }
}
