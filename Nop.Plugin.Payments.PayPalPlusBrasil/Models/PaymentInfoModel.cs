using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        public Uri ApprovalUrl { get; set; }
        public string Mode { get; set; }
        public string PayerFirstName { get; set; }
        public string PayerLastName { get; set; }
        public string PayerEmail { get; set; }
        public string PayerPhone { get; set; }
        public string PayerTaxId { get; set; }
        public string EnableContinue { get; set; }

        public string DisableContinue { get; set; }

        public string RememberedCards { get; set; }

        public string ReturnPaymentPayPal { get; set; }

        public string PaymentIdPayPal { get; set; }

        public string HabilitarParcelamento { get; set; }
        public string ParcelamentoMaximo { get; set; }

        public string CPFCNPJ { get; set; }

    }
}