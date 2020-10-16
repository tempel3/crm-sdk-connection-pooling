using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Kaziya.CRM.ConnectionPooling.Web.Properties;
using Microsoft.Xrm.Sdk;

namespace Kaziya.CRM.ConnectionPooling.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            
            var settings = LoadEnvSettings();
            BuildContainer(settings);
        }

        private void BuildContainer(EnvSettings settings)
        {
            // container
            var builder = new ContainerBuilder();
            
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.Register(i => new CrmConnectionPool(settings.ConnectionString) { MaxConnections = 4 }).SingleInstance();
            builder.RegisterType<CrmOrganizationServiceFromPool>().As<IOrganizationService>().InstancePerRequest();
            
            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static EnvSettings LoadEnvSettings()
        {
            // settings file
            DotNetEnv.Env.Load(Path.Combine(HttpContext.Current.Server.MapPath("~"), ".env"));
            
            return new EnvSettings()
            {
                ConnectionString = Environment.GetEnvironmentVariable("ConnectionString")
            };
        }
    }
}