using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models
{
    public class OsmEntity
    {
        public string place_id { get; set; }
        public object licence { get; set; }
        public object osm_type { get; set; }
        public object osm_id { get; set; }
        public object boundingbox { get; set; }
        public object lat { get; set; }
        public object lon { get; set; }
        public object display_name { get; set; }
        public object Class { get; set; }
        public object type { get; set; }
        public object importance { get; set; }
    }
}
