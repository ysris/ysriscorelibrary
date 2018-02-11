using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models.Stripe
{
    public class CustomerHasStripeCustomer
    {
        [Key]
        public int customerId { get; set; }

        public string stripeCustomerId { get; set; }
    }
}
