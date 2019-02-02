using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalPlusBrasil.Helpers;
using System;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Response
{
    public partial class PaymentResponse
    {
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
        public Payer Payer { get; set; }

        [JsonProperty("transactions")]
        public Transaction[] Transactions { get; set; }

        [JsonProperty("links")]
        public Link[] Links { get; set; }
    }

    public partial class Link
    {
        [JsonProperty("href")]
        public Uri Href { get; set; }

        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }
    }

    public partial class Payer
    {
        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }

        [JsonProperty("payer_info")]
        public PayerInfo PayerInfo { get; set; }
    }

    public partial class PayerInfo
    {
        [JsonProperty("shipping_address")]
        public PayerInfoShippingAddress ShippingAddress { get; set; }
    }

    public partial class PayerInfoShippingAddress
    {
    }

    public partial class Transaction
    {
        [JsonProperty("amount")]
        public Amount Amount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("item_list")]
        public ItemList ItemList { get; set; }

        [JsonProperty("related_resources")]
        public object[] RelatedResources { get; set; }
    }

    public partial class Amount
    {
        [JsonProperty("total")]
        public string Total { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("details")]
        public Details Details { get; set; }
    }

    public partial class Details
    {
        [JsonProperty("subtotal")]
        public string Subtotal { get; set; }

        [JsonProperty("shipping")]
        public string Shipping { get; set; }
    }

    public partial class ItemList
    {
        [JsonProperty("items")]
        public Item[] Items { get; set; }

        [JsonProperty("shipping_address")]
        public ItemListShippingAddress ShippingAddress { get; set; }
    }

    public partial class Item
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
    }

    public partial class ItemListShippingAddress
    {
        [JsonProperty("recipient_name")]
        public string RecipientName { get; set; }

        [JsonProperty("line1")]
        public string Line1 { get; set; }

        [JsonProperty("line2")]
        public string Line2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
    }
}
