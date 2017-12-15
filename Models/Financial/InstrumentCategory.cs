using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ysriscorelibrary.Interfaces;
using YsrisCoreLibrary.Models.Abstract;

namespace YsrisCoreLibrary.Model
{
    public class InstrumentCategory : AbstractEntity, IAbstractEntity
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
    }
}
