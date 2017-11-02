using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace YsrisCoreLibrary.Models
{
    [DataContract]
    public class TimeSerieItem
    {
        [Key]
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string Instrument { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public decimal? Open { get; set; }
        [DataMember]
        public decimal? High { get; set; }
        [DataMember]
        public decimal? Low { get; set; }
        [DataMember]
        public decimal? Close { get; set; }

        [DataMember]
        public decimal? typicalPrice => High != null && Low != null && Close != null ? (High + Low + Close) / 3 : null;
    }
}