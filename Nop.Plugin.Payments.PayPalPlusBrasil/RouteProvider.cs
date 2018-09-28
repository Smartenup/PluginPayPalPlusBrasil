using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.PayPalPlusBrasil
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //IPN
            routes.MapRoute("Plugin.Payments.PayPalPlusBrasil.IPNHandler",
                 "Plugins/PaymentPayPalPlusBrasil/IPNHandler",
                 new { controller = "PaymentPayPalPlusBrasil", action = "IPNHandler" },
                 new[] { "Nop.Plugin.Payments.PayPalPlusBrasil.Controllers" }
            );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
