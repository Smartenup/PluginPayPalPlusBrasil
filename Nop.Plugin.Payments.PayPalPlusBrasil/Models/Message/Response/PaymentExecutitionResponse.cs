using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nop.Plugin.Payments.PayPalPlusBrasil.Helpers;
using System;
using System.Globalization;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Response
{
    public partial class PaymentExecutionResponse
    {
        public PaymentExecutionResponse()
        {
            Payer = new PayerPaymentExecution();
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("create_time")]
        public DateTimeOffset CreateTime { get; set; }

        [JsonProperty("update_time")]
        public DateTimeOffset UpdateTime { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("intent")]
        public string Intent { get; set; }

        [JsonProperty("payer")]
        public PayerPaymentExecution Payer { get; set; }

        [JsonProperty("transactions")]
        public TransactionPaymentExecution[] Transactions { get; set; }

        [JsonProperty("links")]
        public LinkPaymentExecution[] Links { get; set; }
    }

    public partial class LinkPaymentExecution
    {

        [JsonProperty("href")]
        public Uri Href { get; set; }

        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }
    }

    public partial class PayerPaymentExecution
    {
        public PayerPaymentExecution()
        {
            PayerInfo = new PayerInfoPaymentExecution();
        }

        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }

        [JsonProperty("payer_info")]
        public PayerInfoPaymentExecution PayerInfo { get; set; }
    }

    public partial class PayerInfoPaymentExecution
    {
        public PayerInfoPaymentExecution()
        {
            ShippingAddress = new ShippingAddressPaymentExecution();
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
        public ShippingAddressPaymentExecution ShippingAddress { get; set; }
    }

    public partial class ShippingAddressPaymentExecution
    {
        [JsonProperty("line1")]
        public string Line1 { get; set; }

        [JsonProperty("line2")]
        public string Line2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("recipient_name")]
        public string RecipientName { get; set; }
    }

    public partial class TransactionPaymentExecution
    {
        public TransactionPaymentExecution()
        {
            Amount = new TransactionAmountPaymentExecution();
        }

        [JsonProperty("amount")]
        public TransactionAmountPaymentExecution Amount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("item_list")]
        public ItemList ItemList { get; set; }

        [JsonProperty("related_resources")]
        public RelatedResourcePaymentExecution[] RelatedResources { get; set; }
    }

    public partial class TransactionAmountPaymentExecution
    {
        public TransactionAmountPaymentExecution()
        {
            Details = new DetailsPaymentExecution();
        }

        [JsonProperty("total")]
        public string Total { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("details")]
        public DetailsPaymentExecution Details { get; set; }
    }

    public partial class DetailsPaymentExecution
    {
        [JsonProperty("subtotal")]
        public string Subtotal { get; set; }

        [JsonProperty("tax")]
        public string Tax { get; set; }

        [JsonProperty("shipping")]
        public string Shipping { get; set; }

        [JsonProperty("handling_fee")]
        public string HandlingFee { get; set; }

        [JsonProperty("insurance")]
        public string Insurance { get; set; }

        [JsonProperty("shipping_discount")]
        public string ShippingDiscount { get; set; }
    }

    public partial class ItemListPaymentExecution
    {
        public ItemListPaymentExecution()
        {
            ShippingAddress = new ShippingAddressPaymentExecution();
        }

        [JsonProperty("items")]
        public Item[] Items { get; set; }

        [JsonProperty("shipping_address")]
        public ShippingAddressPaymentExecution ShippingAddress { get; set; }
    }

    public partial class ItemPaymentExecution
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("quantity")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Quantity { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("tax")]
        public string Tax { get; set; }
    }

    public partial class RelatedResourcePaymentExecution
    {
        public RelatedResourcePaymentExecution()
        {
            Sale = new SalePaymentExecution();
        }

        [JsonProperty("sale")]
        public SalePaymentExecution Sale { get; set; }
    }

    public partial class SalePaymentExecution
    {
        public SalePaymentExecution()
        {
            Amount = new SaleAmountPaymentExecution();
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("create_time")]
        public DateTimeOffset CreateTime { get; set; }

        [JsonProperty("update_time")]
        public DateTimeOffset UpdateTime { get; set; }

        [JsonProperty("amount")]
        public SaleAmountPaymentExecution Amount { get; set; }

        [JsonProperty("payment_mode")]
        public string PaymentMode { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("protection_eligibility")]
        public string ProtectionEligibility { get; set; }

        [JsonProperty("protection_eligibility_type")]
        public string ProtectionEligibilityType { get; set; }

        [JsonProperty("parent_payment")]
        public string ParentPayment { get; set; }

        [JsonProperty("links")]
        public Link[] Links { get; set; }
    }

    public partial class SaleAmountPaymentExecution
    {
        [JsonProperty("total")]
        public string Total { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public partial class PaymentExecutionResponse
    {
        public static PaymentExecutionResponse FromJson(string json) => JsonConvert.DeserializeObject<PaymentExecutionResponse>(json, ConverterPaymentExecution.Settings);
    }


    internal static class ConverterPaymentExecution
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
