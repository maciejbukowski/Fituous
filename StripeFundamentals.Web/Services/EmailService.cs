using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;


namespace StripeFundamentals.Web.Services
{
    public class EmailService
    {
        public void SendRefundEmail(StripeCharge stripeCharge)
        {
            var message = new MailMessage() { IsBodyHtml = true };
            message.To.Add(new MailAddress("customerservice@funnyant.com"));
            message.Subject = string.Format("refund requested on charge: {0}", stripeCharge.Id);
            message.Body = string.Format("<p>{0}</p>", string.Format("A customer at this email address {0} was issued a refund on their purchase: '{1}'.  Please follow up to determine a reason.", stripeCharge.ReceiptEmail, stripeCharge.Description));

            using (var smtp = new SmtpClient())
            {
                smtp.Send(message);
            }

        }





        public void SendTrialEndEmail(StripeSubscription subscription)
        {
            var message = new MailMessage() { IsBodyHtml = true };
            var customerService = new StripeCustomerService();
            var customer = customerService.Get(subscription.CustomerId); 
            message.To.Add(new MailAddress(customer.Email));
            message.Subject = "your trial is ending in a few days";
            message.Body = string.Format("<p>{0}</p>",
                  string.Format("I wanted to let you know that you have 3 days left on your software trial, so in 3 days your credit card will be charged your first monthly payment of ${0}.",
                  Math.Ceiling((decimal)subscription.StripePlan.Amount / 100)));

            using (var smtp = new SmtpClient())
            {
                smtp.Send(message);
            }
        }




        public void SendSubscriptionPaymentReceiptEmail(StripeInvoice invoice, StripeCustomer customer)
        {
            var message = new MailMessage() { IsBodyHtml = true };
            message.To.Add(new MailAddress(customer.Email));
            message.Subject = string.Format("Software Payment Receipt");
            message.Body = string.Format("<p>{0}</p>",
                                   string.Format("Thank you for using our software.  Your credit card has been charged ${0}",
                                   Math.Ceiling((decimal)invoice.AmountDue / 100)));

            using (var smtp = new SmtpClient())
            {
                smtp.Send(message);
            }
        }
    }
}