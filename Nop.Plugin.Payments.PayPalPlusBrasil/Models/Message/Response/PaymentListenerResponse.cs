using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nop.Plugin.Payments.PayPalPlusBrasil.Helpers;
using System.Globalization;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Response
{
    public partial class PaymentListenerResponse
    {
        public PaymentListenerResponse()
        {
            ResultListener = new ResultListener();
        }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("result")]
        public ResultListener ResultListener { get; set; }
    }

    public partial class ResultListener
    {
        public ResultListener()
        {
            ShippingAddressListener = new ShippingAddressListener();
            Term = new TermListener();
            Payer = new PayerListener();
        }


        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("cart_id")]
        public string CartId { get; set; }

        [JsonProperty("shipping_address")]
        public ShippingAddressListener ShippingAddressListener { get; set; }

        [JsonProperty("payer")]
        public PayerListener Payer { get; set; }

        [JsonProperty("payment_approved")]
        public bool PaymentApproved { get; set; }

        [JsonProperty("declined_instruments")]
        public DeclinedInstrumentListener[] DeclinedInstruments { get; set; }

        [JsonProperty("rememberedCards")]
        public string RememberedCards { get; set; }

        [JsonProperty("term")]
        public TermListener Term { get; set; }
    }

    public partial class DeclinedInstrumentListener
    {
        [JsonProperty("funding_instrument_type")]
        public string FundingInstrumentType { get; set; }

        [JsonProperty("decline_type")]
        public string DeclineType { get; set; }

        [JsonProperty("reason_code")]
        public string ReasonCode { get; set; }
    }

    public partial class PayerListener
    {
        public PayerListener()
        {
            PayerInfo = new PayerInfoListener();
            FundingOption = new FundingOptionListener();
        }

        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("payer_info")]
        public PayerInfoListener PayerInfo { get; set; }

        [JsonProperty("funding_option")]
        public FundingOptionListener FundingOption { get; set; }
    }

    public partial class FundingOptionListener
    {       

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("funding_sources")]
        public FundingSourceListener[] FundingSources { get; set; }
    }

    public partial class FundingSourceListener
    {
        public FundingSourceListener()
        {
            AmountListener = new MonthlyPaymentListener();
        }

        [JsonProperty("funding_mode")]
        public string FundingMode { get; set; }

        [JsonProperty("funding_instrument_type")]
        public string FundingInstrumentType { get; set; }

        [JsonProperty("amount")]
        public MonthlyPaymentListener AmountListener { get; set; }

        [JsonProperty("soft_descriptor")]
        public string SoftDescriptor { get; set; }
    }

    public partial class MonthlyPaymentListener
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public partial class PayerInfoListener
    {
        public PayerInfoListener()
        {
            ShippingAddressListener = new ShippingAddressListener();
        }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("payer_id")]
        public string PayerId { get; set; }

        [JsonProperty("shipping_address")]
        public ShippingAddressListener ShippingAddressListener { get; set; }

        [JsonProperty("phone")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Phone { get; set; }

        [JsonProperty("tax_id_type")]
        public string TaxIdType { get; set; }

        [JsonProperty("tax_id")]
        public string TaxId { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
    }

    public partial class ShippingAddressListener
    {
        [JsonProperty("recipient_name")]
        public string RecipientName { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("line1")]
        public string Line1 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("postal_code")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long PostalCode { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("normalization_status")]
        public string NormalizationStatus { get; set; }

        [JsonProperty("default_address")]
        public bool DefaultAddress { get; set; }

        [JsonProperty("preferred_address")]
        public bool PreferredAddress { get; set; }

        [JsonProperty("disable_for_transaction")]
        public bool DisableForTransaction { get; set; }
    }

    public partial class TermListener
    {
        public TermListener()
        {
            MonthlyPayment = new MonthlyPaymentListener();
        }

        [JsonProperty("term")]
        public long TermTerm { get; set; }

        [JsonProperty("monthly_payment")]
        public MonthlyPaymentListener MonthlyPayment { get; set; }
    }

    public partial class PaymentListenerResponse
    {
        public static PaymentListenerResponse FromJson(string json) => JsonConvert.DeserializeObject<PaymentListenerResponse>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this PaymentListenerResponse self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    
}
