using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models.Stripe
{
    public class CustomerHasStripeCustomer
    {
        public int customerId { get; set; }
        public string stripeCustomerId { get; set; }
    }
}
