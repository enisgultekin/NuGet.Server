// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;
using NuGet.Server.Core.Infrastructure;
using NuGet.Server.DataServices;
using NuGet.Server.Infrastructure;
using NuGet.Server.V2;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;

// The consuming project executes this logic with its own copy of this class. This is done with a .pp file that is
// added and transformed upon package install.
#if DEBUG
[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NuGet.Server.App_Start.NuGetODataConfig), "OnStarting")]
[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(NuGet.Server.App_Start.NuGetODataConfig), "OnStarted")]
#endif

namespace NuGet.Server.App_Start
{
    public static class NuGetODataConfig
    {
        public static void OnStarting()
        {
            ServiceResolver.SetServiceResolver(new DefaultServiceResolver());
            Initialize(GlobalConfiguration.Configuration, "PackagesOData");
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

        public static void Initialize(HttpConfiguration config, string controllerName)
        {
            NuGetV2WebApiEnabler.UseNuGetV2WebApiFeed(
                config,
                "NuGetDefault",
                "nuget",
                controllerName,
                enableLegacyPushRoute: true);

            config.Services.Replace(typeof(IExceptionLogger), new TraceExceptionLogger());

            // Trace.Listeners.Add(new TextWriterTraceListener(HostingEnvironment.MapPath("~/NuGet.Server.log")));
            // Trace.AutoFlush = true;

            config.Routes.MapHttpRoute(
                name: "NuGetDefault_ClearCache",
                routeTemplate: "nuget/clear-cache",
                defaults: new { controller = controllerName, action = nameof(PackagesODataController.ClearCache) },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );
        }
    }
}