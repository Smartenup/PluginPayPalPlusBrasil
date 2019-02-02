using Nop.Plugin.Payments.PayPalPlusBrasil.Domain;
using Nop.Plugin.Payments.PayPalPlusBrasil.Service;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Tasks;
using SmartenUP.Core.Services;
using System;

namespace Nop.Plugin.Payments.PayPalPlusBrasil
{
    public class PayPalPlusPrevisaoEnvioTask : ITask
    {
        private IPayPalPlusOrderService _payPalPlusOrderService;
        private IOrderService _orderService;
        private IOrderNoteService _orderNoteService;
        private readonly ILogger _logger;

        public PayPalPlusPrevisaoEnvioTask(IPayPalPlusOrderService payPalPlusOrderService,
            IOrderService orderService,
            IOrderNoteService orderNoteService,
            ILogger logger)
        {
            _payPalPlusOrderService = payPalPlusOrderService;
            _orderService = orderService;
            _orderNoteService = orderNoteService;
            _logger = logger;

        }

        public void Execute()
        {
            var orderPayPaltNote = _payPalPlusOrderService.GetOrderPayPalNote();

            foreach (var orderPayPal in orderPayPaltNote)
            {
                if (orderPayPal.ControlNoteStatusId == (int) ControlNoteStatus.Concluido )
                {
                    continue;
                }

                var order = _orderService.GetOrderById(orderPayPal.OrderId);

                try
                {
                    _payPalPlusOrderService.SaveOrderNoteControl(order.Id, ControlNoteStatus.Processamento);

                    _orderNoteService.AddOrderNote("Pagamento aprovado.", true, order);

                    _orderNoteService.AddOrderNote("Aguardando Impressão - Excluir esse comentário ao imprimir ", false, order);

                    _orderNoteService.AddOrderNote(_orderNoteService.GetOrdeNoteRecievedPayment(order, "PayPal Transparente"), true, order, true);

                    _payPalPlusOrderService.SaveOrderNoteControl(order.Id, ControlNoteStatus.Concluido);
                }
                catch (Exception ex)
                {
                    string logError = string.Format("Plugin.Payments.PayPalPlusBrasil: Erro criação nota ordem {0}", order.Id.ToString());

                    _logger.Error(logError, ex);

                    _payPalPlusOrderService.SaveOrderNoteControl(order.Id, ControlNoteStatus.Erro);
                }
                
            }
        }
    }
}
