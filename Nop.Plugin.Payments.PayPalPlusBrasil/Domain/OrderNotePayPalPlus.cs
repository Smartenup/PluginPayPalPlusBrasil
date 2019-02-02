using Nop.Core;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Domain
{
    public class OrderNotePayPalPlus : BaseEntity
    {
        public int OrderId { get; set; }
        public int ControlNoteStatusId { get; set; }
    }
}
