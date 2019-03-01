using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.PayPalPlusBrasil.Controllers;
using Nop.Plugin.Payments.PayPalPlusBrasil.Lib;
using Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Request;
using Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Response;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using SmartenUP.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Nop.Plugin.Payments.PayPalPlusBrasil
{
    /// <summary>
    /// PayPalPlusBrasil payment processor
    /// </summary>
    public class PayPalPlusBrasilPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly PayPalPlusBrasilPaymentSettings _payPalPlusBrasilPaymentSettings;
        private readonly ISettingService _settingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly CurrencySettings _currencySettings;
        private readonly IWebHelper _webHelper;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IOrderNoteService _orderNoteService;
        #endregion

        #region Ctor

        public PayPalPlusBrasilPaymentProcessor(
            IWorkContext workContext,
            IStoreService storeService,
            PayPalPlusBrasilPaymentSettings payPalPlusBrasilPaymentSettings,
            ISettingService settingService, 
            IGenericAttributeService genericAttributeService,
            ICurrencyService currencyService, ICustomerService customerService,
            CurrencySettings currencySettings, IWebHelper webHelper, 
            IOrderTotalCalculationService orderTotalCalculationService,
            IOrderNoteService orderNoteService
            )
        {
            _workContext = workContext;
            _storeService = storeService;
            _payPalPlusBrasilPaymentSettings = payPalPlusBrasilPaymentSettings;
            _settingService = settingService;
            _genericAttributeService = genericAttributeService;
            _currencyService = currencyService;
            _customerService = customerService;
            _currencySettings = currencySettings;
            _webHelper = webHelper;
            _orderTotalCalculationService = orderTotalCalculationService;
            _orderNoteService = orderNoteService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            TokenResponse tokenResponse = null;
            PaymentExecutionResponse paymentExecutionResponse = null;

            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var payPalPlusBrasilPaymentSettings = _settingService.LoadSetting<PayPalPlusBrasilPaymentSettings>(storeScope);

            string username = payPalPlusBrasilPaymentSettings.RestAPIClientId;
            string password = payPalPlusBrasilPaymentSettings.RestAPISecrect;

            var payerIdPayPal = processPaymentRequest.CustomValues["PayerIdPayPal"].ToString();

            var paymentIdPayPal = processPaymentRequest.CustomValues["PaymentIdPayPal"].ToString();

            var paymentCreationMessage = new PaymentCreationMessage() { PayerId = payerIdPayPal };

            using (var token = new Token(payPalPlusBrasilPaymentSettings.UseSandbox))
            {
                tokenResponse = token.CreateAsync(username, password).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            using (var paymentExecution = new PaymentExecution(payPalPlusBrasilPaymentSettings.UseSandbox))
            {
                paymentExecutionResponse = paymentExecution.CreateAsync(paymentCreationMessage, paymentIdPayPal, tokenResponse.AcessToken).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            SalePaymentExecution SalePaymentExecution = GetSalePaymentExecution(paymentExecutionResponse);

            string stateExecution = SalePaymentExecution.State;

            if (stateExecution == "completed")
            {
                result.AuthorizationTransactionId = SalePaymentExecution.Id;

                result.NewPaymentStatus = PaymentStatus.Paid;
            }
            else
            {
                result.AuthorizationTransactionId = SalePaymentExecution.Id;

                result.NewPaymentStatus = PaymentStatus.Pending;
            }

            return result;
        }


        private SalePaymentExecution GetSalePaymentExecution(PaymentExecutionResponse executionResponse)
        {
            return executionResponse.Transactions[0].RelatedResources[0].Sale;
        }

        private int GetActiveStoreScopeConfiguration(IStoreService storeService, IWorkContext workContext)
        {
            //ensure that we have 2 (or more) stores
            if (storeService.GetAllStores().Count < 2)
                return 0;

            var storeId = workContext.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AdminAreaStoreScopeConfiguration);
            var store = storeService.GetStoreById(storeId);
            return store != null ? store.Id : 0;

        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            var result = 0;
            return result;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            
            return result;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();           

            return result;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();

            return result;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();

            
            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();


            return result;
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //it's not a redirection payment method. So we always return false
            return false;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentPayPalPlusBrasil";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.PayPalPlusBrasil.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentPayPalPlusBrasil";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.PayPalPlusBrasil.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentPayPalPlusBrasilController);
        }

        public override void Install()
        {
            //settings
            var settings = new PayPalPlusBrasilPaymentSettings
            {
                UseSandbox = true,
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.UseSandbox", "Use Sandbox");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.UseSandbox.Hint", "Check to enable Sandbox (testing environment).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.RestAPISandBoxAccount", "Rest API Account");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.RestAPISandBoxAccount.Hint", "Specify API account name.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.RestAPIClientId", "Rest API Client Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.RestAPIClientId.Hint", "Specify Rest API client Id.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.RestAPISecrect", "Rest API Client Secrect");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.RestAPISecrect.Hint", "Specify API Client Secrect.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.IdButtonConfirmOrFunction", "Id Buttom Confirm or Function Callback");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.IdButtonConfirmOrFunction.Hint", "Caso uma string seja fornecida, ela será interpretada como o ID de um formulário no documento que será submetido quando o botão de Continuar Externo for clicado (pode ser qualquer elemento). Caso uma função de callback seja fornecida, ela será executada apenas quando o botão de Continuar Externo for clicado.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.HabilitarParcelamento", "Habilitar o parcelamento");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.HabilitarParcelamento.Hint", "Habilita o parcelamento no pagamento");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.ParcelamentoMaximo", "Quantidade de Parcelas Máxima");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.Log", "Habilita o log");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.ProfileName", "Nome do ambiente de execução");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.ProfileName.Hint", "Nome do ambiente de execução ( ex: Nome da Máquina, dispositivo, ambiente DEV-HOM-PROD");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.ProfileBrandName", "Nome da marca");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.ProfileBrandName.Hint", "Nome da marca do ambiente de execução");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.ProfileLocaleCode", "Local Code do ambiente de execução (BR)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.ProfileLocaleCode.Hint", "Local Code do ambiente de execução (BR)");


            base.Install();
        }
        
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<PayPalPlusBrasilPaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.UseSandbox");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.UseSandbox.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.RestAPISandBoxAccount");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.RestAPISandBoxAccount.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.RestAPIClientId");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.RestAPIClientId.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.RestAPISecrect");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.RestAPISecrect.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.IdButtonConfirmOrFunction");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.IdButtonConfirmOrFunction.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.HabilitarParcelamento");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.HabilitarParcelamento.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.ParcelamentoMaximo");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.ParcelamentoMaximo.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.Log");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.ProfileName");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.ProfileName.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.ProfileBrandName");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.ProfileBrandName.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.ProfileLocaleCode");
            this.DeletePluginLocaleResource("Plugins.Payments.PayPalPlusBrasil.Fields.ProfileLocaleCode.Hint");

            base.Uninstall();
        }


        /// <summary>
        /// Gets Paypal URL
        /// </summary>
        /// <returns></returns>
        private string GetPaypalUrl()
        {
            return _payPalPlusBrasilPaymentSettings.UseSandbox ? "https://www.sandbox.paypal.com/us/cgi-bin/webscr" :
                "https://www.paypal.com/us/cgi-bin/webscr";
        }


        /// <summary>
        /// Verifies IPN
        /// </summary>
        /// <param name="formString">Form string</param>
        /// <param name="values">Values</param>
        /// <returns>Result</returns>
        public bool VerifyIpn(string formString, out Dictionary<string, string> values)
        {
            var req = (HttpWebRequest)WebRequest.Create(GetPaypalUrl());
            req.Method = WebRequestMethods.Http.Post;
            req.ContentType = MimeTypes.ApplicationXWwwFormUrlencoded;
            //now PayPal requires user-agent. otherwise, we can get 403 error
            req.UserAgent = HttpContext.Current.Request.UserAgent;

            string formContent = string.Format("{0}&cmd=_notify-validate", formString);
            req.ContentLength = formContent.Length;

            //PayPal requires TLS 1.2 since January 2016
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var sw = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
            {
                sw.Write(formContent);
            }

            string response;
            using (var sr = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                response = HttpUtility.UrlDecode(sr.ReadToEnd());
            }
            bool success = response.Trim().Equals("VERIFIED", StringComparison.OrdinalIgnoreCase);

            values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string l in formString.Split('&'))
            {
                string line = l.Trim();
                int equalPox = line.IndexOf('=');
                if (equalPox >= 0)
                    values.Add(line.Substring(0, equalPox), line.Substring(equalPox + 1));
            }

            return success;
        }


        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.Automatic;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Standard;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}