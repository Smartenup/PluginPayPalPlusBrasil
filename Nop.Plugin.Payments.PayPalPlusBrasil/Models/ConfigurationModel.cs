using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalPlusBrasil.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalPlusBrasil.Fields.RestAPISandBoxAccount")]
        public string RestAPISandBoxAccount { get; set; }
        public bool RestAPISandBoxAccount_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalPlusBrasil.Fields.RestAPIClientId")]
        public string RestAPIClientId { get; set; }
        public bool RestAPIClientId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalPlusBrasil.Fields.RestAPISecrect")]
        public string RestAPISecrect { get; set; }
        public bool RestAPISecrect_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalPlusBrasil.Fields.IdButtonConfirmOrFunction")]
        public string IdButtonConfirmOrFunction { get; set; }
        public bool IdButtonConfirmOrFunction_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.PayPalPlusBrasil.Fields.HabilitarParcelamento")]
        public bool HabilitarParcelamento { get; set; }
        public bool HabilitarParcelamento_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalPlusBrasil.Fields.ParcelamentoMaximo")]
        public int ParcelamentoMaximo { get; set; }
        public bool ParcelamentoMaximo_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalPlusBrasil.Fields.Log")]
        /// <summary>
        /// get os set a value indicating that will log everthig
        /// </summary>
        public bool Log { get; set; }
        public bool Log_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalPlusBrasil.Fields.ProfileName")]
        /// <summary>
        /// Nome do ambiente de execução
        /// </summary>
        public string ProfileName { get; set; }
        public bool ProfileName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalPlusBrasil.Fields.ProfileBrandName")]
        public string ProfileBrandName { get; set; }
        public bool ProfileBrandName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalPlusBrasil.Fields.ProfileLocaleCode")]
        public string ProfileLocaleCode { get; set; }
        public bool ProfileLocaleCode_OverrideForStore { get; set; }

    }
}