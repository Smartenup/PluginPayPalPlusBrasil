using Nop.Data.Mapping;
using Nop.Plugin.Payments.PayPalPlusBrasil.Domain;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Data
{
    public class CustomerPayPalPlusMap : NopEntityTypeConfiguration<CustomerPayPalPlus>
    {
        public CustomerPayPalPlusMap()
        {
            this.ToTable("Customer_PayPalPlus");
            this.HasKey(x => x.Id);
        }
    }
}
