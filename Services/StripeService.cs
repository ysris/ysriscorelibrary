using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Models;

namespace YsrisCoreLibrary.Services
{
    /// <summary>
    /// Ysris compliant service to interact with stripe
    /// </summary>
    public class StripeService
    {
        private StripeCustomerService _stripeCustomerService;
        private StripeSubscriptionService _subscriptionService;
        private StripePlanService _stripePlanService;
        private IConfiguration _configuration;

        public StripeService(IConfiguration configuration)
        {
            _configuration = configuration;
            _stripeCustomerService = new StripeCustomerService();
            _subscriptionService = new StripeSubscriptionService();
            _stripePlanService = new StripePlanService();
        }

        public async Task<IEnumerable<StripeSubscription>> GetSubscriptions(string stripeCustomerId)
        {
            var stripeSubscriptions = _subscriptionService.ListAsync(new StripeSubscriptionListOptions { CustomerId = stripeCustomerId });
            return await stripeSubscriptions;
        }

        public async Task<IEnumerable<StripePlan>> ListPlans()
        {
            return await _stripePlanService.ListAsync();
        }


        public async Task<bool> IsCustomerSuscribedOnStripe(Customer entity)
        {
            var set = await _stripeCustomerService.ListAsync();
            return set.Any(a => a.Email == entity.email);
        }

        public async Task<StripeCustomer> CreateStripeCustomer(Customer user, string planId, string sourceToken)
        {
            var plan = await _stripePlanService.GetAsync(planId);

            var stripeCustomerOptions =
                new StripeCustomerCreateOptions
                {
                    Email = user.email,
                    Description = "",
                    SourceToken = sourceToken,
                    PlanId = plan.Id,
                };
            var stripeCustomer = await _stripeCustomerService.CreateAsync(stripeCustomerOptions);

            return stripeCustomer;
        }


    }
}
