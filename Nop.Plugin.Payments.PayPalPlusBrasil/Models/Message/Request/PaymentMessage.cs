using Newtonsoft.Json;
using Nop.Plugin.Payments.PayPalPlusBrasil.Helpers;
using System;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Request
{
    public partial class PaymentMessage
    {
        public PaymentMessage()
        {
            Payer = new Payer();
            RedirectUrls = new RedirectUrls();
        }

        [JsonProperty("intent")]
        public string Intent { get; set; }

        [JsonProperty("payer")]
        public Payer Payer { get; set; }

        [JsonProperty("experience_profile_id")]
        public string ExperienceProfileId { get; set; }

        [JsonProperty("transactions")]
        public Transaction[] Transactions { get; set; }

        [JsonProperty("redirect_urls")]
        public RedirectUrls RedirectUrls { get; set; }
    }

    public partial class Payer
    {
        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }
    }

    public partial class RedirectUrls
    {
        [JsonProperty("return_url")]
        public Uri ReturnUrl { get; set; }

        [JsonProperty("cancel_url")]
        public Uri CancelUrl { get; set; }
    }

    public partial class Transaction
    {
        public Transaction()
        {
            Amount = new Amount();
            PaymentOptions = new PaymentOptions();
            ItemList = new ItemList();
        }

        [JsonProperty("amount")]
        public Amount Amount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("payment_options")]
        public PaymentOptions PaymentOptions { get; set; }

        [JsonProperty("invoice_number")]        
        public string InvoiceNumber { get; set; }

        [JsonProperty("item_list")]
        public ItemList ItemList { get; set; }
    }

    public partial class Amount
    {
        public Amount()
        {
            Details = new Details();
        }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("total")]
        public string Total { get; set; }

        [JsonProperty("details")]
        public Details Details { get; set; }
    }

    public partial class Details
    {
        [JsonProperty("shipping")]
        public string Shipping { get; set; }

        [JsonProperty("subtotal")]
        public string Subtotal { get; set; }

        [JsonProperty("discount")]
        public string Discount { get; set; }
    }



    public partial class ItemList
    {
        public ItemList()
        {
            ShippingAddress = new ShippingAddress();
        }

        [JsonProperty("shipping_address")]
        public ShippingAddress ShippingAddress { get; set; }

        [JsonProperty("items")]
        public Item[] Items { get; set; }
    }

    public partial class Item
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("quantity")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Quantity { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public partial class ShippingAddress
    {
        [JsonProperty("recipient_name")]
        public string RecipientName { get; set; }

        [JsonProperty("line1")]
        public string Line1 { get; set; }

        [JsonProperty("line2")]
        public string Line2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }

    public partial class PaymentOptions
    {
        [JsonProperty("allowed_payment_method")]
        public string AllowedPaymentMethod { get; set; }
    }
}
