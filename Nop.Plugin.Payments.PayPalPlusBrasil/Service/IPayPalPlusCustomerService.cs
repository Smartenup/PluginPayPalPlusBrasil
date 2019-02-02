using Nop.Core.Domain.Customers;
using Nop.Plugin.Payments.PayPalPlusBrasil.Domain;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Service
{
    public interface IPayPalPlusCustomerService
    {
        void InsertCustomer(CustomerPayPalPlus customerPayPalPlus);

        void UpdateCustomer(CustomerPayPalPlus customerPayPalPlus);

        CustomerPayPalPlus GetCustomerPayPalPlus(Customer customer);
    }
}
