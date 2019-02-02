using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.PayPalPlusBrasil.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Service
{
    public class PayPalPlusOrderService : IPayPalPlusOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderNotePayPalPlus> _orderNotePayPalPlusRepository;

        public PayPalPlusOrderService(IRepository<Order> orderRepository,
            IRepository<OrderNotePayPalPlus> orderNotePayPalPlusRepository)
        {
            _orderRepository = orderRepository;
            _orderNotePayPalPlusRepository = orderNotePayPalPlusRepository;
        }


        public IList<OrderPayPaltNote> GetOrderPayPalNote()
        {
            var ordersPayPalPlus = (from order in _orderRepository.Table
                                   where order.PaymentMethodSystemName == "Payments.PayPalPlusBrasil" &&
                                        (order.PaymentStatusId == (int)PaymentStatus.Paid || order.PaymentStatusId == (int)PaymentStatus.Authorized) &&
                                        order.OrderStatusId == (int)OrderStatus.Processing
                                   select new
                                   {
                                       OrderId = order.Id
                                   }).ToList();

            var orderNoteQuery = (from note in _orderNotePayPalPlusRepository.Table
                                  select new OrderPayPaltNote()
                                  {
                                      OrderId = note.OrderId,
                                      ControlNoteStatusId = note.ControlNoteStatusId
                                  }).ToList();

            var query = from order in ordersPayPalPlus
                        join note in orderNoteQuery  
                            on order.OrderId equals note.OrderId  
                            into orderNote
                            from orNo in orderNote.DefaultIfEmpty()
                        where (orNo != null && orNo.ControlNoteStatusId != (int)ControlNoteStatus.Concluido)
                        || (orNo == null)
                        select new OrderPayPaltNote()
                         {
                                OrderId = order.OrderId,
                                ControlNoteStatusId = orNo == null ? (int)ControlNoteStatus.Pendente : orNo.ControlNoteStatusId
                         };

            return query.ToList();
        }

        public void SaveOrderNoteControl(int orderId, ControlNoteStatus controlNoteStatus)
        {
            OrderNotePayPalPlus orderNotePayPalPlus = GetOrderNotePayPalPlus(orderId);

            if (orderNotePayPalPlus == null)
            {
                orderNotePayPalPlus = new OrderNotePayPalPlus();
                orderNotePayPalPlus.OrderId = orderId;
                orderNotePayPalPlus.ControlNoteStatusId = (int)controlNoteStatus;

                _orderNotePayPalPlusRepository.Insert(orderNotePayPalPlus);
            }
            else
            {
                orderNotePayPalPlus.ControlNoteStatusId = (int)controlNoteStatus;

                _orderNotePayPalPlusRepository.Update(orderNotePayPalPlus);
            }
            
        }

        private OrderNotePayPalPlus GetOrderNotePayPalPlus(int orderId)
        {
            var query = _orderNotePayPalPlusRepository.Table;

            query = query.Where(o => o.OrderId == orderId);

            if (query.Count() != 0)
                return query.FirstOrDefault();

            return null;
        }

    }
}
