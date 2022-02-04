using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneSCodeChanger.Services;
using System;
using System.Linq;
using System.Web.Http.Controllers;

namespace OneSCodeChanger.DI
{
    public static class DIContainer
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddTransient<IModuleDownloadService,ModuleService>();
            services.AddTransient<IModuleUploadService,ModuleService>();
            services.AddTransient<IOneSConnectorService, OneSConnector>();
            services.AddLogging(logBuilder => logBuilder.AddFile("1Clogger.log").AddConsole());
            return services;
        }

        public static IServiceCollection AddControllersAsServices(this IServiceCollection services)
        {
            typeof(Startup).Assembly.GetExportedTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Where(t => typeof(IHttpController).IsAssignableFrom(t)
                    || t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)).ToList()
                    .ForEach(type => services.AddTransient(type));
            return services;
        }
    }

}
