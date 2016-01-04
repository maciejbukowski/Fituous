using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Stripe;
using StripeFundamentals.Web;
using StripeFundamentals.Web.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace StripeFundamentals.Web.Services
{
    public class SubscriptionService : StripeFundamentals.Web.Services.ISubscriptionService
    {
        private ApplicationUserManager userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                userManager = value;
            }
        }

        private StripeCustomerService customerService;
        public StripeCustomerService StripeCustomerService
        {
            get
            {
                return customerService ?? new StripeCustomerService();
            }
            private set
            {
                customerService = value;
            }
        }

        private StripeSubscriptionService subscriptionService;
        public StripeSubscriptionService StripeSubscriptionService
        {
            get
            {
                return subscriptionService ?? new StripeSubscriptionService();
            }
            private set
            {
                subscriptionService = value;
            }
        }

        public SubscriptionService()
        {

        }

        public SubscriptionService(ApplicationUserManager userManager, StripeCustomerService customerService, StripeSubscriptionService subscriptionService)
        {
            this.userManager = userManager;
            this.customerService = customerService;
            this.subscriptionService = subscriptionService;
        }

        public void Create(string userName, Plan plan, string stripeToken)
        {
            var user = UserManager.FindByName(userName);
            
            if (String.IsNullOrEmpty(user.StripeCustomerId))  //first time customer
            {
                //create customer which will create subscription if plan is set and cc info via token is provided
                var customer = new StripeCustomerCreateOptions()
                {
                    Email = user.Email,
                    Source = new StripeSourceOptions() { TokenId = stripeToken },
                    PlanId = plan.ExternalId //externalid is stripe plan.id
                };

                StripeCustomer stripeCustomer = StripeCustomerService.Create(customer);

                user.StripeCustomerId = stripeCustomer.Id;
                user.ActiveUntil = DateTime.Now.AddDays((double)plan.TrialPeriodDays);
                UserManager.Update(user);
    
            }
            else
            {
                var stripeSubscription = StripeSubscriptionService.Create(user.StripeCustomerId, plan.ExternalId);
                user.ActiveUntil = DateTime.Now.AddDays((double)plan.TrialPeriodDays);
                UserManager.Update(user);
            }

        }

    }
}