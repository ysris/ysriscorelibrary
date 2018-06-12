using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models.WinBiz
{
    public class WinBizHistoricalRow
    {
        //[COMPOSED PK] See DbContext configuration
        public string winbizClientdos_numero { get; set; }

        [NotMapped]
        public WinBizCustomer winbizClient { get; set; }

        //[COMPOSED PK]
        public DateTime valuationDate { get; set; }

        public decimal salesRevenue { get; internal set; }

        public decimal charges { get; internal set; }

        public decimal preTaxIncome { get; internal set; }

        public decimal cashFlow { get; internal set; }

        public string customerCurrencyCode { get; internal set; }

        public decimal activityEvolution { get; internal set; }

        public decimal resultEvolution { get; internal set; }

        //[NotMapped]
        //public IEnumerable<WinBizHistoricalAccount> treasuryEvolution { get; internal set; }
    }
}
