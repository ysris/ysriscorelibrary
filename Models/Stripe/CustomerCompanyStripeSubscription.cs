using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stripe;

namespace YsrisCoreLibrary.Models.Stripe
{
    public class CustomerCompanyStripeSubscription
    {
        public int id { get; set; }
        public object plan { get; set; }
        public int quantity { get; set; }
        public DateTime created { get; set; }
        public object stripeResponse { get; set; }
        public string customerId { get; internal set; }
        public Tuple<DateTime?, DateTime?> period { get; internal set; }
        public List<StripeSubscriptionItem> subscriptions { get; internal set; }
        public StripeBilling? billing { get; internal set; }
        public StripePlan stripePlan { get; internal set; }
        public DateTime? start { get; internal set; }
        public string status { get; internal set; }
        public Tuple<DateTime?, DateTime?> trialPeriod { get; internal set; }
        public string stripeSubId { get; internal set; }
    }
}
