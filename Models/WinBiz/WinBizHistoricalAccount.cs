using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models.WinBiz
{
    public class WinBizHistoricalAccount
    {
        public int classe { get; set; }
        public string numero { get; set; }
        public string compte { get; set; }
        public string pla_monnai { get; set; }
        public decimal solde { get; set; }
        public decimal débit { get; set; }
        public decimal crédit { get; set; }
        public decimal si_ml_d { get; set; }
        public decimal si_ml_c { get; set; }
        public decimal solde_me { get; set; }
        public decimal si_me_d { get; set; }
        public decimal si_me_c { get; set; }
        public decimal? budg_year { get; set; }
        public decimal? budg_month01 { get; set; }
        public decimal? budg_month02 { get; set; }
        public decimal? budg_month03 { get; set; }
        public decimal? budg_month04 { get; set; }
        public decimal? budg_month05 { get; set; }
        public decimal? budg_month06 { get; set; }
        public decimal? budg_month07 { get; set; }
        public decimal? budg_month08 { get; set; }
        public decimal? budg_month09 { get; set; }
        public decimal? budg_month10 { get; set; }
        public decimal? budg_month11 { get; set; }
        public decimal? budg_month12 { get; set; }

        // Composed PK : in context : TODO
        public string winbizCustomerId { get; set; }
        public DateTime? valuationDate { get; set; }
    }
}
