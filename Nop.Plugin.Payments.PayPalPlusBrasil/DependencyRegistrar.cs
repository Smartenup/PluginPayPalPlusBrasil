using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Payments.PayPalPlusBrasil.Data;
using Nop.Plugin.Payments.PayPalPlusBrasil.Domain;
using Nop.Plugin.Payments.PayPalPlusBrasil.Service;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.PayPalPlusBrasil
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 0;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<PayPalPlusCustomerService>().As<IPayPalPlusCustomerService>().InstancePerLifetimeScope();
            builder.RegisterType< PayPalPlusOrderService>().As<IPayPalPlusOrderService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<PayPalPlusObjectContext>(builder, "nop_object_context_paypalplus");

            //override required repository with our custom context
            builder.RegisterType<EfRepository<CustomerPayPalPlus>>()
                .As<IRepository<CustomerPayPalPlus>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_paypalplus"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<OrderNotePayPalPlus>>()
                .As<IRepository<OrderNotePayPalPlus>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_paypalplus"))
                .InstancePerLifetimeScope();
        }
    }
}
