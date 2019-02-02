using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Response
{
    public partial class PaymentProfileExperienceResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("temporary")]
        public bool Temporary { get; set; }

        [JsonProperty("flow_config")]
        public FlowConfig FlowConfig { get; set; }

        [JsonProperty("input_fields")]
        public InputFields InputFields { get; set; }

        [JsonProperty("presentation")]
        public Presentation Presentation { get; set; }
    }

    public partial class FlowConfig
    {
    }

    public partial class InputFields
    {
        [JsonProperty("no_shipping")]
        public long NoShipping { get; set; }

        [JsonProperty("address_override")]
        public long AddressOverride { get; set; }
    }

    public partial class Presentation
    {
        [JsonProperty("brand_name")]
        public string BrandName { get; set; }

        [JsonProperty("locale_code")]
        public string LocaleCode { get; set; }
    }
}
