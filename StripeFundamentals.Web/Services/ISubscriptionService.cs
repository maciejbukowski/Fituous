using System;
namespace StripeFundamentals.Web.Services
{
    public interface ISubscriptionService
    {
        void Create(string userName, StripeFundamentals.Web.Data.Plan plan, string stripeToken);
        Stripe.StripeCustomerService StripeCustomerService { get; }
        Stripe.StripeSubscriptionService StripeSubscriptionService { get; }
        StripeFundamentals.Web.ApplicationUserManager UserManager { get; }
    }
}
