using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models
{
    [DataContract]
    public class PurpleColumn
    {

        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public bool isRequired { get; set; }
        [DataMember]
        public object entityColumnsModel { get; set; }
    }
}
