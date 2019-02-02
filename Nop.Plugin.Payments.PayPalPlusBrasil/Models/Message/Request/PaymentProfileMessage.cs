using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Request
{
    public class PaymentProfileMessage
    {
        public PaymentProfileMessage()
        {
            Presentation = new Presentation();
            InputFields = new InputFields();
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("presentation")]
        public Presentation Presentation { get; set; }

        [JsonProperty("input_fields")]
        public InputFields InputFields { get; set; }
    }

    public partial class InputFields
    {
        [JsonProperty("no_shipping")]
        public int NoShipping { get; set; }

        [JsonProperty("address_override")]
        public int AddressOverride { get; set; }
    }

    public partial class Presentation
    {
        [JsonProperty("brand_name")]
        public string BrandName { get; set; }

        [JsonProperty("locale_code")]
        public string LocaleCode { get; set; }
    }

}
