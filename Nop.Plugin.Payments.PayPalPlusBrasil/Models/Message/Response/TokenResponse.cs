using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Response
{
    public class TokenResponse
    {
        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("access_token")]
        public string AcessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("app_id")]
        public string AppID { get; set; }

        [JsonProperty("expires_in")]
        public int ExpeiredIn { get; set; }
    }
}
