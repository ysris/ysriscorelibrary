using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using ysriscorelibrary.Interfaces;
using YsrisCoreLibrary.Model;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Models.Abstract;

namespace YsrisCoreLibrary.Models.Financial
{
    /// <summary>
    /// Abstract im
    /// </summary>    
    public class Instrument : AbstractEntity, IAbstractEntity
    {
        [Key]
        [DataMember]
        public int id { get; set; }


        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string code { get; set; }

        [DataMember]
        public DateTime creationDate { get; set; }

        [DataMember]
        [NotMapped]
        public List<object> entityModel { get; set; }

        [DataMember]
        [NotMapped]
        public string prettyName => $"{code} - {name}";

        [DataMember]
        [NotMapped]
        public List<TimeSerieItem> timeSerie { get; set; }

        [DataMember]
        [NotMapped]
        public bool isFavorite { get; set; }

        [DataMember]
        [NotMapped]
        //['2013/1/25', 2300, 2291.3, 2288.26, 2308.38],
        public object chartTimeSerie =>
            timeSerie == null ? null :
            new
            {
                categoryData = timeSerie.Select(a => a.date.ToString("yyyy/M/dd")),
                values = timeSerie.Select(a => new object[] { a.open, a.close, a.high, a.low, })
            };

        [DataMember]
        [NotMapped]
        public object echartOptions { get; set; }

        [DataMember]
        [NotMapped]
        public Dictionary<string, object> indicators { get; set; }

        [DataMember]
        public string importFileUri { get; set; }


        [DataMember]
        public string exchange { get; set; }

        [DataMember]
        public string category { get; set; }        

        [DataMember]
        public string producttype { get; set; }

        [DataMember]
        public string unitvalue { get; set; }

        [DataMember]
        public string currency { get; set; }



        // OHLC, date and period type needed for valuation fo current instrument


        [DataMember]
        [NotMapped]
        public decimal open { get; set; }

        [DataMember]
        [NotMapped]
        public decimal high { get; set; }

        [DataMember]
        [NotMapped]
        public decimal low { get; set; }

        [DataMember]
        [NotMapped]
        public decimal close { get; set; }

        [DataMember]
        [NotMapped]
        public InstrumentCategory instrumentCategory { get; set; }
    }
}