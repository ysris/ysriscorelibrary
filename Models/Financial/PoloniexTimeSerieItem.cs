using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Classes;

namespace YsrisCoreLibrary.Models.Financial
{
    public class PoloniexTimeSerieItem : PoloniexTimeSerieItemCommon
    {
        public PoloniexTimeSerieItem(PoloniexTimeSerieItemRaw entity)
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

        public DateTime date { get; set; }
    }

    public class PoloniexTimeSerieItemRaw : PoloniexTimeSerieItemCommon
    {
        public string date { get; set; }
    }

    public class PoloniexTimeSerieItemCommon
    {
        public string high { get; set; }
        public string low { get; set; }
        public string open { get; set; }
        public string close { get; set; }
        public string volume { get; set; }
        public string quoteVolume { get; set; }
        public string weightedAverage { get; set; }
    }
}
