using Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Request;
using Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Response;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Lib
{
    public class PaymentExperience : APIResource
    {
        public PaymentExperience(bool sandBox) : base(sandBox)
        {
            BaseURI = "/payment-experience/web-profiles";
        }

        public async Task<PaymentProfileExperienceResponse> CreateAsync(PaymentProfileMessage profileMessage, string token)
        {
            var retorno = await PostAsync<PaymentProfileExperienceResponse>(profileMessage, null, token).ConfigureAwait(false);
            return retorno;
        }
    }
}
