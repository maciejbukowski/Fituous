using System;
namespace StripeFundamentals.Web.Data
{
    public interface IPaymentsModel
    {
        System.Data.Entity.DbSet<Feature> Features { get; set; }
        System.Data.Entity.DbSet<Plan> Plans { get; set; }
    }
}
