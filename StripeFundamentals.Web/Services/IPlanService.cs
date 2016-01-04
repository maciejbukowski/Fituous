using System;
namespace StripeFundamentals.Web.Services
{
    public interface IPlanService
    {
        StripeFundamentals.Web.Data.Plan Find(int id);
        System.Collections.Generic.IList<StripeFundamentals.Web.Data.Plan> List();
    }
}
