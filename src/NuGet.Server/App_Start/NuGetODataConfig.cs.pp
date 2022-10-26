using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;
using NuGet.Server;
using NuGet.Server.Infrastructure;
using NuGet.Server.V2;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.NuGetODataConfig), "OnStarting")]
[assembly: WebActivatorEx.PostApplicationStartMethod(typeof($rootnamespace$.App_Start.NuGetODataConfig), "OnStarted")]

namespace $rootnamespace$.App_Start 
{
    public static class NuGetODataConfig 
    {
        public static void OnStarting() 
        {
            ServiceResolver.SetServiceResolver(new DefaultServiceResolver());

            var config = GlobalConfiguration.Configuration;

                       NuGetV2WebApiEnabler.UseNuGetV2WebApiFeed(
                config,
                "NuGetDefault",
                "nuget",
                "PackagesOData",
                enableLegacyPushRoute: true);

            config.Services.Replace(typeof(IExceptionLogger), new TraceExceptionLogger());

            // Trace.Listeners.Add(new TextWriterTraceListener(HostingEnvironment.MapPath("~/NuGet.Server.log")));
            // Trace.AutoFlush = true;

            config.Routes.MapHttpRoute(
                name: "NuGetDefault_ClearCache",
                routeTemplate: "nuget/clear-cache",
                defaults: new { controller = "PackagesOData", action = "ClearCache" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

        }
        
        public static void OnStarted()
                {
                    var config = GlobalConfiguration.Configuration;
                    var container = new Container();
                    container.RegisterInstance(typeof(ISettingsProvider), ServiceResolver.Current.Resolve<ISettingsProvider>());
                    container.RegisterWebApiControllers(config);
                    container.Verify();
        
                    config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
                }

    }
}
