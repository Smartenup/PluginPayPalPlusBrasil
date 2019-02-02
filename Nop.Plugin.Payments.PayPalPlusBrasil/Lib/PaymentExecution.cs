using Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Request;
using Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Response;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Lib
{
    public class PaymentExecution : APIResource
    {
        public PaymentExecution(bool sandBox) : base(sandBox)
        {
            BaseURI = "/payments/payment";
        }

        public async Task<PaymentExecutionResponse> CreateAsync(PaymentCreationMessage paymentCreationMessage, string PaymentIdPayPal, string token)
        {
            var retorno = await PostAsync<PaymentExecutionResponse>(paymentCreationMessage, $"{PaymentIdPayPal}/execute", token).ConfigureAwait(false);
            return retorno;
        }
    }
}
