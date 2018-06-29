using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models.Financial
{
    public class RsiTick
    {
        public DateTime DateTime { get; set; }
        public decimal K { get; set; }
        public decimal D { get; set; }
    }
}
