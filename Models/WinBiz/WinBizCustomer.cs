using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models.WinBiz
{
    [DataContract]
    public class WinBizCustomer
    {
        public WinBizCustomer()
        {
        }
        public WinBizCustomer(IDictionary<string, object> raw)
        {
            dos_numero = Convert.ToInt32(raw["dos_numero"]);
            dos_name = raw["dos_name"].ToString();
        }
        [Key]
        [DataMember]
        public int dos_numero { get; set; }
        [DataMember]
        public string dos_name { get; set; }
    }
}
