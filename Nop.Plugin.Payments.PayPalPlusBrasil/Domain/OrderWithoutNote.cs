using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Domain
{
    public class OrderPayPaltNote
    {
        public int OrderId { get; set; }

        public int? ControlNoteStatusId { get; set; }
    }
}
