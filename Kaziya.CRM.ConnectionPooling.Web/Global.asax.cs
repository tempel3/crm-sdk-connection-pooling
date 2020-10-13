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
            
            BuildContainer();
        }
        
        protected void BuildContainer()
        {
            // settings file
            DotNetEnv.Env.Load(Path.Combine(HttpContext.Current.Server.MapPath("~"),".env"));
            var connectionString = System.Environment.GetEnvironmentVariable("ConnectionString");
            // container
            var builder = new ContainerBuilder();
            
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.Register(i => new CrmConnectionPool(connectionString) { MaxConnections = 4 }).SingleInstance();
            builder.RegisterType<CrmOrganizationServiceFromPool>().As<IOrganizationService>().InstancePerRequest();
            
            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}