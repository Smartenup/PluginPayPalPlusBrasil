using Nop.Data.Mapping;
using Nop.Plugin.Payments.PayPalPlusBrasil.Domain;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Data
{
    public class OrderNotePayPalPlusMap : NopEntityTypeConfiguration<OrderNotePayPalPlus>
    {
        public OrderNotePayPalPlusMap()
        {
            this.ToTable("Order_Note_PayPalPlus");
            this.HasKey(x => x.Id);
        }
    }
}
