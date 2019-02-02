using Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Request;
using Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Response;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Lib
{
    public class PaymentCreation : APIResource
    {
        public PaymentCreation(bool sandBox) : base(sandBox)
        {
            BaseURI = "/payments/payment";
        }

        public async Task<PaymentResponse> CreateAsync(PaymentMessage paymentMessage, string token)
        {
            var retorno = await PostAsync<PaymentResponse>(paymentMessage, null, token).ConfigureAwait(false);
            return retorno;
        }
    }
}
