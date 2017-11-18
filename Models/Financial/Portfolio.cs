using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using ysriscorelibrary.Interfaces;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Models.Abstract;

namespace YsrisCoreLibrary.Models.Financial
{
    [DataContract]
    public class Portfolio : AbstractEntity, IAbstractEntity
    {
        [Key]
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public DateTime valuationDate { get; set; }

        //Value from the portfolios valuation csv file
        [DataMember]
        public decimal? valuationAmount { get; set; }

        [DataMember]
        public string name { get; set; }

        // [DataMember]
        // public decimal valuationPortfolioCcy { get; set; }

        // [DataMember]
        // public decimal valuationUsd { get; set; }

        // [DataMember]
        // public decimal performanceAs100Value { get; set; }

        // [DataMember]
        // public string description { get; set; }

        [DataMember]
        public string importFileUri { get; set; }

        /// related .equ file
        [DataMember]
        public string portfolioValuationfileName { get; set; }

        [DataMember]
        public DateTime creationDate { get; set; }

        [DataMember]
        public DateTime deletionDate { get; set; }

        [DataMember]
        [NotMapped]
        public bool isFavorite { get; set; }

        [DataMember]
        [NotMapped]
        public object echartOptions { get; set; }

        [DataMember]
        [NotMapped]
        //['2013/1/25', 2300, 2291.3, 2288.26, 2308.38],
        public object chartTimeSerie { get; set; }
        

        //[DataMember]
        [NotMapped]
        public IEnumerable<PortfolioValuation> portfolioValuation { get; set; }

        [DataMember]
        [NotMapped]
        public Dictionary<string, object> indicators { get; set; }

        //[DataMember]
        //public IEnumerable<object> portfolioValuationChart => portfolioValuation?.Select(a => new { value = new string[] { a.valuationDate.ToString("yyyy-MM-dd"), a.valuationAmount.ToString() } });

        [DataMember]
        [NotMapped]
        public List<PortfolioPosition> positions { get; set; }

        [DataMember]
        [NotMapped]
        public List<object> entityModel { get; set; }

        [DataMember]
        [NotMapped]
        public string prettyName => name;
    }
}