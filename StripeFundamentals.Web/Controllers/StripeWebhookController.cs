using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Stripe;
using StripeFundamentals.Web.Services;
using StripeFundamentals.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace StripeFundamentals.Web.Controllers
{
    public class StripeWebhookController : Controller
    {
        private ApplicationUserManager userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
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

        public StripeWebhookController() { }

        public StripeWebhookController(ApplicationUserManager userManager, StripeCustomerService customerService)
        {
            this.userManager = userManager;
            this.StripeCustomerService = customerService;
        }

        [HttpPost]
        public ActionResult Index()
        {
            Stream request = Request.InputStream;
            request.Seek(0, SeekOrigin.Begin);
            string json = new StreamReader(request).ReadToEnd();
            StripeEvent stripeEvent = null;
            try
            {
                stripeEvent = StripeEventUtility.ParseEvent(json);
                stripeEvent = VerifyEventSentFromStripe(stripeEvent);
                if (HasEventBeenProcessedPreviously(stripeEvent)) { return new HttpStatusCodeResult(HttpStatusCode.OK); };

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, string.Format("Unable to parse incoming event.  The following error occurred: {0}", ex.Message));
            }

            if (stripeEvent == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Incoming event empty");

            var emailService = new StripeFundamentals.Web.Services.EmailService();
            switch (stripeEvent.Type)
            {
                case StripeEvents.ChargeRefunded:
                    var charge = Mapper<StripeCharge>.MapFromJson(stripeEvent.Data.Object.ToString());
                    emailService.SendRefundEmail(charge); 
                    break;
                case StripeEvents.CustomerSubscriptionTrialWillEnd:
                    var subscription = Mapper<StripeSubscription>.MapFromJson(stripeEvent.Data.Object.ToString());
                    emailService.SendTrialEndEmail(subscription);
                    break;
                case StripeEvents.InvoicePaymentSucceeded:
                    StripeInvoice invoice = Mapper<StripeInvoice>.MapFromJson(stripeEvent.Data.Object.ToString());
                    var customer = StripeCustomerService.Get(invoice.CustomerId);
                    var user = UserManager.FindByEmail(customer.Email);

                    user.ActiveUntil = user.ActiveUntil.AddMonths(1);
	                UserManager.Update(user);

                    emailService.SendSubscriptionPaymentReceiptEmail(invoice, customer);
                    break;
                case StripeEvents.InvoicePaymentFailed:
                    var failedInvoice = Mapper<StripeInvoice>.MapFromJson(stripeEvent.Data.Object.ToString());
                    //TODO: implement sending email to customer and customer service
                    break;
                default:
                    break;
            }

            //TODO: log Stripe eventid to StripeEvent table in application database
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private bool HasEventBeenProcessedPreviously(StripeEvent stripeEvent)
        {
            //lookup in your database's StripeEventLog by  eventid
            //if eventid exists return true
            return false;

        }

        private static StripeEvent VerifyEventSentFromStripe(StripeEvent stripeEvent)
        {
            var eventService = new StripeEventService();
            stripeEvent = eventService.Get(stripeEvent.Id);
            return stripeEvent;
        }


       
    }
}