using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.PayPalPlusBrasil.Domain;
using Nop.Plugin.Payments.PayPalPlusBrasil.Helper;
using Nop.Plugin.Payments.PayPalPlusBrasil.Lib;
using Nop.Plugin.Payments.PayPalPlusBrasil.Models;
using Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Request;
using Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Response;
using Nop.Plugin.Payments.PayPalPlusBrasil.Service;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using SmartenUP.Core.Util.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Controllers
{
    public class PaymentPayPalPlusBrasilController : BasePaymentController
    {

        private readonly JsonSerializerSettings _jsonSettings;
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILogger _logger;
        private readonly PaymentSettings _paymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerService _customerService;
        private readonly IStoreContext _storeContext;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly ITaxService _taxService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPayPalPlusCustomerService _payPalPlusCustomerService;

        public PaymentPayPalPlusBrasilController(IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            IPaymentService paymentService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            ILogger logger,
            PaymentSettings paymentSettings,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            ICustomerService customerService,
            IStoreContext storeContext,
            IOrderTotalCalculationService orderTotalCalculationService,
            IAddressAttributeParser addressAttributeParser,
            ITaxService taxService,
            IPriceCalculationService priceCalculationService,
            IPayPalPlusCustomerService payPalPlusCustomerService)
        {
            _workContext = workContext;
            _storeService = storeService;
            _settingService = settingService;
            _paymentService = paymentService;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _logger = logger;
            _paymentSettings = paymentSettings;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _customerService = customerService;
            _storeContext = storeContext;
            _orderTotalCalculationService = orderTotalCalculationService;
            _addressAttributeParser = addressAttributeParser;
            _taxService = taxService;
            _priceCalculationService = priceCalculationService;
            _payPalPlusCustomerService = payPalPlusCustomerService;

            _jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            /*
             * //load settings for a chosen store scope*/
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var settings = _settingService.LoadSetting<PayPalPlusBrasilPaymentSettings>(storeScope);
            
            var model = new ConfigurationModel();
                        model.UseSandbox = settings.UseSandbox;
            model.RestAPISandBoxAccount = settings.RestAPISandBoxAccount;
            model.RestAPIClientId = settings.RestAPIClientId;
            model.RestAPISecrect = settings.RestAPISecrect;
            model.IdButtonConfirmOrFunction = settings.IdButtonConfirmOrFunction;
            model.HabilitarParcelamento = settings.HabilitarParcelamento;
            model.ParcelamentoMaximo = settings.ParcelamentoMaximo;
            model.Log = settings.Log;
            model.ProfileName = settings.ProfileName;
            model.ProfileBrandName = settings.ProfileBrandName;
            model.ProfileLocaleCode = settings.ProfileLocaleCode;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.UseSandbox_OverrideForStore = _settingService.SettingExists(settings, x => x.UseSandbox, storeScope);
                model.RestAPISandBoxAccount_OverrideForStore = _settingService.SettingExists(settings, x => x.RestAPISandBoxAccount, storeScope);
                model.RestAPIClientId_OverrideForStore = _settingService.SettingExists(settings, x => x.RestAPIClientId, storeScope);
                model.RestAPISecrect_OverrideForStore = _settingService.SettingExists(settings, x => x.RestAPISecrect, storeScope);
                model.IdButtonConfirmOrFunction_OverrideForStore = _settingService.SettingExists(settings, x => x.IdButtonConfirmOrFunction, storeScope);
                model.HabilitarParcelamento_OverrideForStore = _settingService.SettingExists(settings, x => x.HabilitarParcelamento, storeScope);
                model.ParcelamentoMaximo_OverrideForStore = _settingService.SettingExists(settings, x => x.ParcelamentoMaximo, storeScope);

                model.Log_OverrideForStore = _settingService.SettingExists(settings, x => x.Log, storeScope);
                model.ProfileName_OverrideForStore = _settingService.SettingExists(settings, x => x.ProfileName, storeScope);
                model.ProfileBrandName_OverrideForStore = _settingService.SettingExists(settings, x => x.ProfileBrandName, storeScope);
                model.ProfileLocaleCode_OverrideForStore = _settingService.SettingExists(settings, x => x.ProfileLocaleCode, storeScope);
            }
            
            return View("~/Plugins/Payments.PayPalPlusBrasil/Views/PaymentPayPalPlusBrasil/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var settings = _settingService.LoadSetting<PayPalPlusBrasilPaymentSettings>(storeScope);

            //save settings
            settings.UseSandbox = model.UseSandbox;
            settings.RestAPISandBoxAccount = model.RestAPISandBoxAccount;
            settings.RestAPIClientId = model.RestAPIClientId;
            settings.RestAPISecrect = model.RestAPISecrect;
            settings.IdButtonConfirmOrFunction = model.IdButtonConfirmOrFunction;
            settings.HabilitarParcelamento = model.HabilitarParcelamento;
            settings.ParcelamentoMaximo = model.ParcelamentoMaximo;
            settings.Log = model.Log;
            settings.ProfileName = model.ProfileName;
            settings.ProfileBrandName = model.ProfileBrandName;
            settings.ProfileLocaleCode = model.ProfileLocaleCode;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            _settingService.SaveSettingOverridablePerStore(settings, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.RestAPISandBoxAccount, model.RestAPISandBoxAccount_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.RestAPIClientId, model.RestAPIClientId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.RestAPISecrect, model.RestAPISecrect_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.IdButtonConfirmOrFunction, model.IdButtonConfirmOrFunction_OverrideForStore, storeScope, false);

            _settingService.SaveSettingOverridablePerStore(settings, x => x.HabilitarParcelamento, model.HabilitarParcelamento_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.ParcelamentoMaximo, model.ParcelamentoMaximo_OverrideForStore, storeScope, false);

            _settingService.SaveSettingOverridablePerStore(settings, x => x.Log, model.HabilitarParcelamento_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.ProfileName, model.ProfileName_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.ProfileBrandName, model.ProfileBrandName_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(settings, x => x.ProfileLocaleCode, model.ProfileLocaleCode_OverrideForStore, storeScope, false);

            //now clear settings cache

            _settingService.ClearCache();
            
            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel();

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var payPalPlusBrasilPaymentSettings = _settingService.LoadSetting<PayPalPlusBrasilPaymentSettings>(storeScope);

            var customer = _customerService.GetCustomerById(_workContext.CurrentCustomer.Id);
            var customerPayPalPlus = _payPalPlusCustomerService.GetCustomerPayPalPlus(customer);

            string username = payPalPlusBrasilPaymentSettings.RestAPIClientId;
            string password = payPalPlusBrasilPaymentSettings.RestAPISecrect;

            TokenResponse tokenResponse = null;
            PaymentResponse paymentResponse = null;
            PaymentMessage paymentMessage = null;

            var number = string.Empty;
            var complement = string.Empty;
            var cpfCnpj = string.Empty;

            try
            {

                using (var token = new Token(payPalPlusBrasilPaymentSettings.UseSandbox))
                    tokenResponse = token.CreateAsync(username, password).ConfigureAwait(false).GetAwaiter().GetResult();

                VerifyProfileExperience(payPalPlusBrasilPaymentSettings, tokenResponse);

                paymentMessage = GetPaymentoMessage(payPalPlusBrasilPaymentSettings, customer);

                if (payPalPlusBrasilPaymentSettings.Log)
                    _logger.InsertLog(LogLevel.Information, "Nop.Plugin.Payments.PayPalPlusBrasil.PaymentMessage", JsonConvert.SerializeObject(paymentMessage, _jsonSettings), customer);

                using (var paymentCreation = new PaymentCreation(payPalPlusBrasilPaymentSettings.UseSandbox))
                    paymentResponse = paymentCreation.CreateAsync(paymentMessage, tokenResponse.AcessToken).ConfigureAwait(false).GetAwaiter().GetResult();

                if (payPalPlusBrasilPaymentSettings.Log)
                    _logger.InsertLog(LogLevel.Information, "Nop.Plugin.Payments.PayPalPlusBrasil.PaymentResponse", JsonConvert.SerializeObject(paymentResponse, _jsonSettings), customer);

                new AddressHelper(_addressAttributeParser, _workContext).GetCustomNumberAndComplement(customer.BillingAddress.CustomAttributes,
                    out number, out complement, out cpfCnpj);

                model.ApprovalUrl = GetAprovalUrl(paymentResponse.Links);
                model.Mode = payPalPlusBrasilPaymentSettings.UseSandbox ? "sandbox" : "live";
                model.PayerFirstName = customer.BillingAddress.FirstName;
                model.PayerLastName = customer.BillingAddress.LastName;
                model.PayerEmail = customer.Email;
                model.PayerPhone = AddressHelper.FormatarCelular(customer.ShippingAddress.PhoneNumber);
                model.PayerTaxId = cpfCnpj;
                model.DisableContinue = payPalPlusBrasilPaymentSettings.IdButtonConfirmOrFunction;
                model.EnableContinue = payPalPlusBrasilPaymentSettings.IdButtonConfirmOrFunction;
                model.HabilitarParcelamento = payPalPlusBrasilPaymentSettings.HabilitarParcelamento ? "1" : "0";
                model.ParcelamentoMaximo = payPalPlusBrasilPaymentSettings.ParcelamentoMaximo.ToString();
                model.CPFCNPJ = (cpfCnpj.Length <= 11) ? "BR_CPF" : "BR_CNPJ";
                model.PaymentIdPayPal = paymentResponse.Id;
                model.RememberedCards = (customerPayPalPlus != null) ? customerPayPalPlus.RememberedCards : string.Empty;

                return View("~/Plugins/Payments.PayPalPlusBrasil/Views/PaymentPayPalPlusBrasil/PaymentInfo.cshtml", model);

            }
            catch (Exception ex)
            {
                _logger.Error("PNop.Plugin.Payments.PayPalPlusBrasil.Controllers", ex, customer);
                throw;
            }
        }

        private Uri GetAprovalUrl(Link[] link)
        {
            foreach (var item in link)
            {
                if (string.Equals(item.Rel, "approval_url"))
                {
                    return item.Href;
                }
            }

            return null;
        }

        private PaymentMessage GetPaymentoMessage(PayPalPlusBrasilPaymentSettings payPalPlusBrasilPaymentSettings, Customer customer)
        {

            var cart = _workContext.CurrentCustomer.ShoppingCartItems.
                Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
               .LimitPerStore(_storeContext.CurrentStore.Id)
               .ToList();

            List<AppliedGiftCard> appliedGiftCards;
            List<Discount> orderAppliedDiscounts;
            decimal orderDiscountAmount;
            int redeemedRewardPoints;
            decimal redeemedRewardPointsAmount;
            var orderTotal = _orderTotalCalculationService.GetShoppingCartTotal(cart, out orderDiscountAmount,
                out orderAppliedDiscounts, out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount);

            decimal tax;
            List<Discount> shippingTotalDiscounts;
            var orderShippingTotalInclTax = _orderTotalCalculationService.GetShoppingCartShippingTotal(cart, true, out tax, out shippingTotalDiscounts);

            decimal orderSubTotalDiscountAmount;
            List<Discount> orderSubTotalAppliedDiscounts;
            decimal subTotalWithoutDiscountBase;
            decimal subTotalWithDiscountBase;
            _orderTotalCalculationService.GetShoppingCartSubTotal(cart, true, out orderSubTotalDiscountAmount,
                out orderSubTotalAppliedDiscounts, out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);

            var paymentMessage = new PaymentMessage();

            paymentMessage.Intent = "sale";
            paymentMessage.Payer.PaymentMethod = "paypal";

            paymentMessage.ExperienceProfileId = payPalPlusBrasilPaymentSettings.IdProfileExperience;

            var transactionList = new List<Models.Message.Request.Transaction>();
            var itemTransaction = new Models.Message.Request.Transaction();

            itemTransaction.Amount.Currency = "BRL";
            itemTransaction.Amount.Total = orderTotal.Value.ToString("N", new CultureInfo("en-US"));
            itemTransaction.Amount.Details.Shipping = orderShippingTotalInclTax.Value.ToString("N", new CultureInfo("en-US"));
            itemTransaction.Amount.Details.Subtotal = subTotalWithoutDiscountBase.ToString("N", new CultureInfo("en-US"));
            itemTransaction.Amount.Details.Discount = (orderDiscountAmount + orderSubTotalDiscountAmount).ToString("N", new CultureInfo("en-US"));

            itemTransaction.Description = "This is the payment transaction description";
            itemTransaction.PaymentOptions.AllowedPaymentMethod = "IMMEDIATE_PAY";
            itemTransaction.InvoiceNumber = string.Empty;


            var number = string.Empty;
            var complement = string.Empty;

            new AddressHelper(_addressAttributeParser, _workContext).GetCustomNumberAndComplement(customer.ShippingAddress.CustomAttributes,
                out number, out complement);

            itemTransaction.ItemList.ShippingAddress.RecipientName = AddressHelper.GetFullName(customer.ShippingAddress);
            itemTransaction.ItemList.ShippingAddress.Line1 = customer.ShippingAddress.Address1;
            itemTransaction.ItemList.ShippingAddress.Line2 = complement;
            itemTransaction.ItemList.ShippingAddress.City = customer.ShippingAddress.City;
            itemTransaction.ItemList.ShippingAddress.CountryCode = customer.ShippingAddress.Country.TwoLetterIsoCode;
            itemTransaction.ItemList.ShippingAddress.PostalCode = customer.ShippingAddress.ZipPostalCode;
            itemTransaction.ItemList.ShippingAddress.State = customer.ShippingAddress.StateProvince.Name;
            itemTransaction.ItemList.ShippingAddress.Phone = AddressHelper.FormatarCelular(customer.ShippingAddress.PhoneNumber);

            var itemList = new List<Models.Message.Request.Item>();

            foreach (var itemCart in cart)
            {
                var item = new Models.Message.Request.Item();

                item.Name = itemCart.Product.Name;
                item.Description = itemCart.Product.ShortDescription;
                item.Quantity = itemCart.Quantity;

                List<Discount> scDiscounts;
                decimal discountAmount;

                var scUnitPrice = _priceCalculationService.GetUnitPrice(itemCart, true, out discountAmount, out scDiscounts);

                item.Price = decimal.Round(scUnitPrice, 2).ToString("N2", new CultureInfo("en-US"));
                item.Sku = itemCart.Product.Sku;
                item.Currency = "BRL";

                itemList.Add(item);
            }

            itemTransaction.ItemList.Items = itemList.ToArray();

            transactionList.Add(itemTransaction);

            paymentMessage.Transactions = transactionList.ToArray();

            string returnUrl = _webHelper.GetStoreLocation(false) + "Plugins/PaymentPayPalPlusBrasil/IPNHandler";
            string cancelReturnUrl = _webHelper.GetStoreLocation(false) + "Plugins/PaymentPayPalPlusBrasil/IPNHandler";

            paymentMessage.RedirectUrls.ReturnUrl =  new Uri(returnUrl);
            paymentMessage.RedirectUrls.CancelUrl = new Uri(cancelReturnUrl);


            return paymentMessage;
        }

        private void VerifyProfileExperience(PayPalPlusBrasilPaymentSettings settings, TokenResponse tokenResponse)
        {
            if (string.IsNullOrWhiteSpace(settings.IdProfileExperience))
            {
                var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);

                var paymentProfileMessage = GetPaymentProfile(storeScope, settings);

                using (var experience = new PaymentExperience(settings.UseSandbox))
                {
                    var experienceResponse = experience.CreateAsync(paymentProfileMessage, tokenResponse.AcessToken).ConfigureAwait(false).GetAwaiter().GetResult();

                    settings.IdProfileExperience = experienceResponse.Id;

                    _settingService.SaveSettingOverridablePerStore(settings, x => x.IdProfileExperience, true, storeScope, false);

                    //now clear settings cache
                    _settingService.ClearCache();
                }
            }
        }

        private PaymentProfileMessage GetPaymentProfile(int storeScope, PayPalPlusBrasilPaymentSettings settings)
        {
            var paymentProfileMessage = new PaymentProfileMessage();

            paymentProfileMessage.Name = settings.ProfileName + " - " + storeScope.ToString();
            paymentProfileMessage.Presentation.BrandName = settings.ProfileBrandName + " - " + storeScope.ToString();
            paymentProfileMessage.Presentation.LocaleCode = settings.ProfileLocaleCode;

            paymentProfileMessage.InputFields.NoShipping = 0;
            paymentProfileMessage.InputFields.AddressOverride = 1;

            return paymentProfileMessage;
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var settings = _settingService.LoadSetting<PayPalPlusBrasilPaymentSettings>(storeScope);
            var customer = _customerService.GetCustomerById(_workContext.CurrentCustomer.Id);

            var paymentInfo = new ProcessPaymentRequest();
            paymentInfo.CustomValues = new Dictionary<string, object>();

            if (settings.Log)
                _logger.InsertLog(LogLevel.Information, "Nop.Plugin.Payments.PayPalPlusBrasil.PaymentListenerResponse", form["ReturnPaymentPayPal"], customer);

            var paymentListenerResponse = PaymentListenerResponse.FromJson(form["ReturnPaymentPayPal"]);

            if (!string.IsNullOrWhiteSpace(paymentListenerResponse.ResultListener.Term.MonthlyPayment.Value))
            {
                paymentInfo.CustomValues.Add("Parcelamento", paymentListenerResponse.ResultListener.Term.TermTerm.ToString() + "- X");
                paymentInfo.CustomValues.Add("Valor Parcela", paymentListenerResponse.ResultListener.Term.MonthlyPayment.Value);
                paymentInfo.CustomValues.Add("MOEDA", paymentListenerResponse.ResultListener.Term.MonthlyPayment.Currency);
            }

            paymentInfo.CustomValues.Add("PayerIdPayPal", paymentListenerResponse.ResultListener.Payer.PayerInfo.PayerId);
            paymentInfo.CustomValues.Add("PaymentIdPayPal", form["PaymentIdPayPal"]);

            AtualizarRememberedCard(paymentListenerResponse);

            return paymentInfo;

        }

        private void AtualizarRememberedCard(PaymentListenerResponse paymentListenerResponse)
        {
            var customer = _customerService.GetCustomerById(_workContext.CurrentCustomer.Id);

            var customerPayPalPlus = _payPalPlusCustomerService.GetCustomerPayPalPlus(customer);

            if (customerPayPalPlus == null){ customerPayPalPlus = new CustomerPayPalPlus(); }

            customerPayPalPlus.RememberedCards = paymentListenerResponse.ResultListener.RememberedCards;
            customerPayPalPlus.CustomerId = _workContext.CurrentCustomer.Id;

            if (customerPayPalPlus.Id > 0)
                _payPalPlusCustomerService.UpdateCustomer(customerPayPalPlus);
            else
                _payPalPlusCustomerService.InsertCustomer(customerPayPalPlus);
        }


        [ValidateInput(false)]
        public ActionResult IPNHandler()
        {
            byte[] param = Request.BinaryRead(Request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(param);
            Dictionary<string, string> values;

            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.PayPalPlusBrasil") as PayPalPlusBrasilPaymentProcessor;
            if (processor == null ||
                !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("PayPal Standard module cannot be loaded");

            if (processor.VerifyIpn(strRequest, out values))
            {
                #region values
                decimal mc_gross = decimal.Zero;
                try
                {
                    mc_gross = decimal.Parse(values["mc_gross"], new CultureInfo("en-US"));
                }
                catch { }

                string payer_status = string.Empty;
                values.TryGetValue("payer_status", out payer_status);
                string payment_status = string.Empty;
                values.TryGetValue("payment_status", out payment_status);
                string pending_reason = string.Empty;
                values.TryGetValue("pending_reason", out pending_reason);
                string mc_currency = string.Empty;
                values.TryGetValue("mc_currency", out mc_currency);
                string txn_id = string.Empty;
                values.TryGetValue("txn_id", out txn_id);
                string txn_type = string.Empty;
                values.TryGetValue("txn_type", out txn_type);
                string rp_invoice_id = string.Empty;
                values.TryGetValue("rp_invoice_id", out rp_invoice_id);
                string payment_type = string.Empty;
                values.TryGetValue("payment_type", out payment_type);
                string payer_id = string.Empty;
                values.TryGetValue("payer_id", out payer_id);
                string receiver_id = string.Empty;
                values.TryGetValue("receiver_id", out receiver_id);
                string invoice = string.Empty;
                values.TryGetValue("invoice", out invoice);
                string payment_fee = string.Empty;
                values.TryGetValue("payment_fee", out payment_fee);

                #endregion

                var sb = new StringBuilder();
                sb.AppendLine("Paypal IPN:");
                foreach (KeyValuePair<string, string> kvp in values)
                {
                    sb.AppendLine(kvp.Key + ": " + kvp.Value);
                }

                var newPaymentStatus = PaypalHelper.GetPaymentStatus(payment_status, pending_reason);
                sb.AppendLine("New payment status: " + newPaymentStatus);

                switch (txn_type)
                {
                    case "recurring_payment_profile_created":
                        //do nothing here
                        break;
                    case "recurring_payment":
                        #region Recurring payment
                        {
                            Guid orderNumberGuid = Guid.Empty;
                            try
                            {
                                orderNumberGuid = new Guid(rp_invoice_id);
                            }
                            catch
                            {
                            }

                            var initialOrder = _orderService.GetOrderByGuid(orderNumberGuid);
                            if (initialOrder != null)
                            {
                                var recurringPayments = _orderService.SearchRecurringPayments(initialOrderId: initialOrder.Id);
                                foreach (var rp in recurringPayments)
                                {
                                    switch (newPaymentStatus)
                                    {
                                        case PaymentStatus.Authorized:
                                        case PaymentStatus.Paid:
                                            {
                                                var recurringPaymentHistory = rp.RecurringPaymentHistory;
                                                if (!recurringPaymentHistory.Any())
                                                {
                                                    //first payment
                                                    var rph = new RecurringPaymentHistory
                                                    {
                                                        RecurringPaymentId = rp.Id,
                                                        OrderId = initialOrder.Id,
                                                        CreatedOnUtc = DateTime.UtcNow
                                                    };
                                                    rp.RecurringPaymentHistory.Add(rph);
                                                    _orderService.UpdateRecurringPayment(rp);
                                                }
                                                else
                                                {
                                                    //next payments
                                                    var processPaymentResult = new ProcessPaymentResult();
                                                    processPaymentResult.NewPaymentStatus = newPaymentStatus;
                                                    if (newPaymentStatus == PaymentStatus.Authorized)
                                                        processPaymentResult.AuthorizationTransactionId = txn_id;
                                                    else
                                                        processPaymentResult.CaptureTransactionId = txn_id;

                                                    _orderProcessingService.ProcessNextRecurringPayment(rp, processPaymentResult);
                                                }
                                            }
                                            break;
                                    }
                                }

                                //this.OrderService.InsertOrderNote(newOrder.OrderId, sb.ToString(), DateTime.UtcNow);
                                _logger.Information("PayPal IPN. Recurring info", new NopException(sb.ToString()));
                            }
                            else
                            {
                                _logger.Error("PayPal IPN. Order is not found", new NopException(sb.ToString()));
                            }
                        }
                        #endregion
                        break;
                    default:
                        #region Standard payment
                        {
                            string orderNumber = string.Empty;
                            values.TryGetValue("custom", out orderNumber);
                            Guid orderNumberGuid = Guid.Empty;
                            try
                            {
                                orderNumberGuid = new Guid(orderNumber);
                            }
                            catch
                            {
                            }

                            var order = _orderService.GetOrderByGuid(orderNumberGuid);
                            if (order != null)
                            {

                                //order note
                                order.OrderNotes.Add(new OrderNote
                                {
                                    Note = sb.ToString(),
                                    DisplayToCustomer = false,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                                _orderService.UpdateOrder(order);

                                switch (newPaymentStatus)
                                {
                                    case PaymentStatus.Pending:
                                        {
                                        }
                                        break;
                                    case PaymentStatus.Authorized:
                                        {
                                            //validate order total
                                            if (Math.Round(mc_gross, 2).Equals(Math.Round(order.OrderTotal, 2)))
                                            {
                                                //valid
                                                if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
                                                {
                                                    _orderProcessingService.MarkAsAuthorized(order);
                                                }
                                            }
                                            else
                                            {
                                                //not valid
                                                string errorStr = string.Format("PayPal IPN. Returned order total {0} doesn't equal order total {1}. Order# {2}.", mc_gross, order.OrderTotal, order.Id);
                                                //log
                                                _logger.Error(errorStr);
                                                //order note
                                                order.OrderNotes.Add(new OrderNote
                                                {
                                                    Note = errorStr,
                                                    DisplayToCustomer = false,
                                                    CreatedOnUtc = DateTime.UtcNow
                                                });
                                                _orderService.UpdateOrder(order);
                                            }
                                        }
                                        break;
                                    case PaymentStatus.Paid:
                                        {
                                            //validate order total
                                            if (Math.Round(mc_gross, 2).Equals(Math.Round(order.OrderTotal, 2)))
                                            {
                                                //valid
                                                if (_orderProcessingService.CanMarkOrderAsPaid(order))
                                                {
                                                    order.AuthorizationTransactionId = txn_id;
                                                    _orderService.UpdateOrder(order);

                                                    _orderProcessingService.MarkOrderAsPaid(order);

                                                }
                                            }
                                            else
                                            {
                                                //not valid
                                                string errorStr = string.Format("PayPal IPN. Returned order total {0} doesn't equal order total {1}. Order# {2}.", mc_gross, order.OrderTotal, order.Id);
                                                //log
                                                _logger.Error(errorStr);
                                                //order note
                                                order.OrderNotes.Add(new OrderNote
                                                {
                                                    Note = errorStr,
                                                    DisplayToCustomer = false,
                                                    CreatedOnUtc = DateTime.UtcNow
                                                });
                                                _orderService.UpdateOrder(order);
                                            }
                                        }
                                        break;
                                    case PaymentStatus.Refunded:
                                        {
                                            var totalToRefund = Math.Abs(mc_gross);
                                            if (totalToRefund > 0 && Math.Round(totalToRefund, 2).Equals(Math.Round(order.OrderTotal, 2)))
                                            {
                                                //refund
                                                if (_orderProcessingService.CanRefundOffline(order))
                                                {
                                                    _orderProcessingService.RefundOffline(order);
                                                }
                                            }
                                            else
                                            {
                                                //partial refund
                                                if (_orderProcessingService.CanPartiallyRefundOffline(order, totalToRefund))
                                                {
                                                    _orderProcessingService.PartiallyRefundOffline(order, totalToRefund);
                                                }
                                            }
                                        }
                                        break;
                                    case PaymentStatus.Voided:
                                        {
                                            if (_orderProcessingService.CanVoidOffline(order))
                                            {
                                                _orderProcessingService.VoidOffline(order);
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                _logger.Error("PayPal IPN. Order is not found", new NopException(sb.ToString()));
                            }
                        }
                        #endregion
                        break;
                }
            }
            else
            {
                _logger.Error("PayPal IPN failed.", new NopException(strRequest));
            }

            //nothing should be rendered to visitor
            return Content("");
        }

        [ValidateInput(false)]
        public ActionResult LogError(string ppplusError)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var settings = _settingService.LoadSetting<PayPalPlusBrasilPaymentSettings>(storeScope);
            var customer = _customerService.GetCustomerById(_workContext.CurrentCustomer.Id);

            if (settings.Log)
                _logger.InsertLog(LogLevel.Information, "Nop.Plugin.Payments.PayPalPlusBrasil.PaymentListenerJS", ppplusError, customer);

            return Content("");
        }
    }
}