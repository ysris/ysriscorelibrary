using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Models.Stripe;

namespace YsrisCoreLibrary.Services
{
    public class StripeService
    {
        private StripeCustomerService stripeCustomerService = new StripeCustomerService();
        private StripeSubscriptionService subscriptionService = new StripeSubscriptionService();

        public StripeService()
        {
        }

        public async Task<IEnumerable<StripeSubscription>> GetSubscriptions(string stripeCustomerId)
        {
            
            var stripeSubscriptions = subscriptionService.ListAsync(new StripeSubscriptionListOptions { CustomerId = stripeCustomerId });


            return await stripeSubscriptions;
        }
    }
}
