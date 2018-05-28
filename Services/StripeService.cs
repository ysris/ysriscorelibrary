using Microsoft.Extensions.Configuration;
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

        public async Task<IEnumerable<CustomerCompanyStripeSubscription>> GetSubscriptions(string stripeCustomerId)
        {
            var stripeSubscriptions = await _subscriptionService.ListAsync(new StripeSubscriptionListOptions { CustomerId = stripeCustomerId });
            return stripeSubscriptions.Select(a => new CustomerCompanyStripeSubscription
            {
                customerId = a.CustomerId,
                created = (DateTime)a.Created,
                period = new Tuple<DateTime?, DateTime?>(a.CurrentPeriodStart, a.CurrentPeriodEnd),
                subscriptions = a.Items.Data,
                billing = a.Billing,
                plan = a.StripePlan,
                quantity = (int)a.Quantity,
                start = a.Start,
                status = a.Status,
                trialPeriod = new Tuple<DateTime?, DateTime?>(a.TrialEnd, a.TrialStart),
                stripeSubId = a.Id
            });
        }

        public async Task<IEnumerable<StripePlan>> ListPlans()
        {
            return await _stripePlanService.ListAsync();
        }

        public async Task BuyLicences(string stripeCustomerId, int licenceCountToBuy)
        {
            var stripeSubscription = (await _subscriptionService.ListAsync(new StripeSubscriptionListOptions { CustomerId = stripeCustomerId })).Single();
            await _subscriptionService.UpdateAsync(
                stripeSubscription.Id,
                new StripeSubscriptionUpdateOptions { Quantity = stripeSubscription.Quantity + licenceCountToBuy }
            );
        }

        public async Task<StripeCustomer> GetStripeCustomer(Customer entity)
        {
            var set = await _stripeCustomerService.ListAsync();
            return set.SingleOrDefault(a => a.Email == entity.email);
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
