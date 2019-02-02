using Nop.Plugin.Payments.PayPalPlusBrasil.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Service
{
    public interface IPayPalPlusOrderService
    {
        IList<OrderPayPaltNote> GetOrderPayPalNote();

        void SaveOrderNoteControl(int orderId, ControlNoteStatus controlNoteStatus);
    }
}
