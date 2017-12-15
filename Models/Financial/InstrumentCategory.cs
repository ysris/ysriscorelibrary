using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Model
{
    public class InstrumentCategory
    {
        [Key]
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string key { get; set; }

        [DataMember]
        public string description { get; set; }

    }
}
