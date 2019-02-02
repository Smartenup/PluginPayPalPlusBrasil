using Nop.Core;
using Nop.Core.Domain.Customers;
using System;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Domain
{
    public class CustomerPayPalPlus : BaseEntity
    {
        public int CustomerId { get; set; }

        public string RememberedCards { get; set; }
    }
}
