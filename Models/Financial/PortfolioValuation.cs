using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using ysriscorelibrary.Interfaces;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Models.Abstract;

namespace YsrisCoreLibrary.Models.Financial
{
    [DataContract]
    public class PortfolioValuation : AbstractEntity, IAbstractEntity
    {
        [Key]
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string fileName { get; set; }

        [DataMember]
        public DateTime valuationDate { get; set; }

        [DataMember]
        public decimal valuationAmount { get; set; }

        [DataMember]
        [NotMapped]
        public List<object> entityModel { get; set; }
    }
}