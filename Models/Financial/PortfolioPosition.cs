using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using ysriscorelibrary.Interfaces;

namespace YsrisCoreLibrary.Models.Financial
{
    [DataContract]
    public class PortfolioPosition : IAbstractEntity
    {
        public void SetFromValues(IAbstractEntity values)
        {
            throw new NotImplementedException();
        }

        [Key]
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string instrumentCode { get; set; }

        [DataMember]
        public string instrumentName { get; set; }

        [DataMember]
        public int? instrumentId { get; set; }

        [DataMember]
        public string portfolioName { get; set; }

        [DataMember]
        public decimal? position { get; set; }

        [DataMember]
        public int longshort { get; set; }

        [DataMember]
        public DateTime valuationDate { get; set; }

        //[DataMember]
        //public Portfolio portfolio { get; set; }

        [DataMember]
        public int? portfolioId { get; set; }

        [DataMember]
        public string importFileUri { get; set; }

        [DataMember]
        public DateTime creationDate { get; set; }

        [DataMember]
        [NotMapped]
        public List<object> entityModel { get; set; }

        [DataMember]
        [NotMapped]
        public Instrument instrument { get; set; }

        //[DataMember]
        //public Instrument instrument { get; set; }

        //[DataMember]
        //public string simulationName { get; set; }

        [DataMember]
        public DateTime? deletionDate { get; set; }

    }
}