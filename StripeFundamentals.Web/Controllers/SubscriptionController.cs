using StripeFundamentals.Models.Subscription;
using StripeFundamentals.Web.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StripeFundamentals.Web.Controllers;
using Stripe;


namespace StripeFundamentals.Controllers
{
    public class SubscriptionController : Controller
    {
        private IPlanService planService;

        private ISubscriptionService subscriptionService;
        public ISubscriptionService SubscriptionService
        {
            get
            {
                return subscriptionService ?? new SubscriptionService();
            }
            private set
            {
                subscriptionService = value;
            }
        }

        public SubscriptionController(IPlanService planService, ISubscriptionService subscriptionService)
        {
            this.planService = planService;
            this.subscriptionService = subscriptionService;
        }


        public SubscriptionController()
        {

        }

        public IPlanService PlanService
        {
            get
            {
                return planService ?? new PlanService();
            }
            private set
            {
                planService = value;
            }
        }


        // GET: Subscription
        public ActionResult Index()
        {
            var viewModel = new IndexViewModel() { Plans = PlanService.List() };
            return View(viewModel);
        }

        public ActionResult Billing(int planId)
        {
            string stripePublishableKey = ConfigurationManager.AppSettings["stripePublishableKey"];
            var viewModel = new BillingViewModel() { Plan = PlanService.Find(planId), StripePublishableKey = stripePublishableKey };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Billing(BillingViewModel billingViewModel)
        {
            billingViewModel.Plan = PlanService.Find(billingViewModel.Plan.Id);
            try
            {
                SubscriptionService.Create(User.Identity.Name, billingViewModel.Plan, billingViewModel.StripeToken);
            }
            catch (StripeException stripeException)
            {
                ModelState.AddModelError(string.Empty, stripeException.Message);
                return View(billingViewModel);
            }
            return RedirectToAction("Index", "Dashboard");
        }
       


        
    }
}