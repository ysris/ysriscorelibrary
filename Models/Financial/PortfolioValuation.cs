using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using ysriscorelibrary.Interfaces;

namespace YsrisCoreLibrary.Models.Financial
{
    [DataContract]
    public class PortfolioValuation : IAbstractEntity
    {
        public void SetFromValues(IAbstractEntity values)
        {
            throw new NotImplementedException();
        }

        [DataMember]
        public string portfolioName { get; set; }

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