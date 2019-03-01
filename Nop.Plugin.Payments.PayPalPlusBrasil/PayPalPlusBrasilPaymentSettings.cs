using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.PayPalPlusBrasil
{
    public class PayPalPlusBrasilPaymentSettings : ISettings
    {
        public string IdProfileExperience { get; set; }
        public bool UseSandbox { get; set; }

        public string RestAPISandBoxAccount { get; set; }
        public string RestAPIClientId { get; set; }
        public string RestAPISecrect { get; set; }    

        public string IdButtonConfirmOrFunction { get; set; }

        public bool HabilitarParcelamento { get; set; }

        public int ParcelamentoMaximo { get; set; }

        /// <summary>
        /// get os set a value indicating that will log everthig
        /// </summary>
        public bool Log { get; set; }
        
        /// <summary>
        /// Nome do ambiente de execução
        /// </summary>
        public string ProfileName { get; set; }

        public string ProfileBrandName { get; set; }

        public string ProfileLocaleCode { get; set; }

    }
}
