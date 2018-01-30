using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Classes;

namespace YsrisCoreLibrary.Models.Financial
{
    [DataContract]
    public class TimeSerieItem : PoloniexTimeSerieItemCommon
    {
        public TimeSerieItem()
        {

        }

        public TimeSerieItem(PoloniexTimeSerieItemRaw entity)
        {
            high = entity.high;
            low = entity.low;
            open = entity.open;
            close = entity.close;
            volume = entity.volume;
            quoteVolume = entity.quoteVolume;
            weightedAverage = entity.weightedAverage;
            date = Tools.UnixTimeStampToDateTime(entity.date);
        }

        //[Key]
        //[DataMember]
        //public int id { get; set; }

        [DataMember]
        public string Instrument { get; set; }

        //[DataMember]
        //public string Market { get; set; }

        [DataMember]
        public DateTime date { get; set; }

        [DataMember]
        public int Period { get; set; }

        [DataMember]
        public new decimal? high { get; set; }

        [DataMember]
        public new decimal? low { get; set; }

        [DataMember]
        public new decimal? open { get; set; }

        [DataMember]
        public new decimal? close { get; set; }

        [DataMember]
        public new string volume { get; set; }

        [DataMember]
        public new string quoteVolume { get; set; }

        [DataMember]
        public new string weightedAverage { get; set; }
    }

    public class PoloniexTimeSerieItemRaw : PoloniexTimeSerieItemCommon
    {
        [DataMember]
        public string date { get; set; }
    }

    public class PoloniexTimeSerieItemCommon
    {
        [DataMember]
        public decimal? high { get; set; }

        [DataMember]
        public decimal? low { get; set; }

        [DataMember]
        public decimal? open { get; set; }

        [DataMember]
        public decimal? close { get; set; }

        [DataMember]
        public string volume { get; set; }

        [DataMember]
        public string quoteVolume { get; set; }

        [DataMember]
        public string weightedAverage { get; set; }
    }
}
