using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StripeFundamentals.Web.Data
{
    public partial class Plan
    {
        public string Name { get; set; }
        public int AmountInCents { get; set; }
        public string Currency { get; set; }
        public string Interval { get; set; }
        public int? TrialPeriodDays { get; set; }
        public int AmountInDollars
        {
            get
            {
                return (int)Math.Floor((decimal)this.AmountInCents / 100);
            }
        }
    }
}