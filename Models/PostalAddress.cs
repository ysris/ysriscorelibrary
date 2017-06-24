using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models
{
    [DataContract]
    public class PostalAddress
    {
        public PostalAddress()
        {
                
        }
        public PostalAddress(dynamic values)
        {
            if (values.id != null) id = values.id;
            if (values.adrLine1 != null) adrLine1 = values.adrLine1;
            if (values.adrLine2 != null) adrLine2 = values.adrLine2;
            if (values.adrPostalCode != null) adrPostalCode = values.adrPostalCode;
            if (values.adrCity != null) adrCity = values.adrCity;
            if (values.adrCountry != null) adrCountry = values.adrCountry;
        }

        [DataMember]
        [Key]
        public int id { get; set; }

        [DataMember]
        public string adrLine1 { get; set; }
        [DataMember]
        public string adrLine2 { get; set; }
        [DataMember]
        public string adrPostalCode { get; set; }
        [DataMember]
        public string adrCity { get; set; }
        [DataMember]
        public string adrCountry { get; set; }

        public DateTime? DeletionDate { get; set; }
    }
}
