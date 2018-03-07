using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models.Yooz
{
    [DataContract]
    public class YoozCustomer
    {
        [Key]
        [DataMember]
        public int index { get; set; }

        [DataMember]
        public string code { get; set; }

        [DataMember]
        public string name { get; set; }
    }
}
