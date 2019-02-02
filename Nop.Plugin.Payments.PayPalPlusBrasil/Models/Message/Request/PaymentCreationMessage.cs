using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Request
{
    public class PaymentCreationMessage
    {
        [JsonProperty("payer_id")]
        public string PayerId { get; set; }
    }
}
