using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StripeFundamentals.Models.Subscription
{
    public class IndexViewModel
    {
        public IList<Web.Data.Plan> Plans { get; set; }
    }

    public class BillingViewModel
    {
        public StripeFundamentals.Web.Data.Plan Plan { get; set; }
        public string StripePublishableKey { get; set; }

        public string StripeToken { get; set; }
    }
    
}