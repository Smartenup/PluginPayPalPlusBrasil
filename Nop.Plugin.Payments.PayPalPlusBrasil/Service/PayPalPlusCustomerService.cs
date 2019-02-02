using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Payments.PayPalPlusBrasil.Domain;
using System;
using System.Linq;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Service
{
    public class PayPalPlusCustomerService : IPayPalPlusCustomerService
    {
        private readonly IRepository<CustomerPayPalPlus> _customerPayPalPlusRepository;

        public PayPalPlusCustomerService(IRepository<CustomerPayPalPlus> customerPayPalPlusRepository)
        {
            _customerPayPalPlusRepository = customerPayPalPlusRepository;
        }

        public CustomerPayPalPlus GetCustomerPayPalPlus(Customer customer)
        {
            var query = _customerPayPalPlusRepository.Table;

            query = query.Where(o => o.CustomerId == customer.Id);

            if (query.Count() != 0)
                return query.FirstOrDefault();

            return null;
        }

        public void InsertCustomer(CustomerPayPalPlus customerPayPalPlus)
        {
            if (customerPayPalPlus == null)
                throw new ArgumentNullException("customerPayPalPlus");

            _customerPayPalPlusRepository.Insert(customerPayPalPlus);
        }

        public void UpdateCustomer(CustomerPayPalPlus customerPayPalPlus)
        {
            if (customerPayPalPlus == null)
                throw new ArgumentNullException("customerPayPalPlus");

            _customerPayPalPlusRepository.Update(customerPayPalPlus);
        }
    }
}
