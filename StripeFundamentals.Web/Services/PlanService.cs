using Stripe;
using StripeFundamentals.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StripeFundamentals.Web.Services
{
    public class PlanService : StripeFundamentals.Web.Services.IPlanService
    {
        private IPaymentsModel db;
        private StripePlanService stripePlanService;

        public PlanService(IPaymentsModel paymentsModel, StripePlanService stripePlanService)
        {
            this.db = paymentsModel;
            this.stripePlanService = stripePlanService;
        }

        public PlanService()
            : this(new PaymentsModel(), new StripePlanService())
        {

        }

        public Plan Find(int id)
        {
            var plan = (from p in db.Plans.Include("Features")
                        where p.Id == id
                        select p).SingleOrDefault();

            var stripePlan = stripePlanService.Get(plan.ExternalId);
            StripePlanToPlan(stripePlan, plan);

            return plan;

        }


        public IList<Plan> List() {
            var plans = (from p in db.Plans.Include("Features")
                         orderby p.DisplayOrder
                         select p).ToList();
            
            var stripePlans = (from p in stripePlanService.List() select p).ToList();
            foreach (var plan in plans)
            {
                var stripePlan = stripePlans.Single(p => p.Id == plan.ExternalId);
                StripePlanToPlan(stripePlan, plan);
            }

            return plans;
        }

        private static void StripePlanToPlan(StripePlan stripePlan, Plan plan)
        {
            plan.Name = stripePlan.Name;
            plan.AmountInCents = stripePlan.Amount;
            plan.Currency = stripePlan.Currency;
            plan.Interval = stripePlan.Interval;
            plan.TrialPeriodDays = stripePlan.TrialPeriodDays;
        }


    }
}