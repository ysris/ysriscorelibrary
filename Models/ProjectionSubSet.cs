using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models
{
    public class ProjectionSubSet
    {
        public int sourcecount { get; set; }
        public int destinationcount { get; set; }
        public object valuationDatePeriod { get; set; }
        public virtual bool status => sourcecount == destinationcount && sourcecount != 0;
        public string source { get; set; }
        public string destinationtable { get; set; }
        public string sourcetable { get; set; }
        public string destination { get; set; }
    }
}
