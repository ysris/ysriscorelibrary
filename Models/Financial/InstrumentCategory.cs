using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ysriscorelibrary.Interfaces;

namespace YsrisCoreLibrary.Model
{
    public class InstrumentCategory : IAbstractEntity
    {
        [Key]
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string key { get; set; }

        [DataMember]
        public string description { get; set; }

        [DataMember]
        [NotMapped]
        public List<object> entityModel { get; set; }

        [DataMember]
        public DateTime? deletionDate { get; set; }


        public void SetFromValues(IAbstractEntity values)
        {
            throw new NotImplementedException();
        }
    }
}
