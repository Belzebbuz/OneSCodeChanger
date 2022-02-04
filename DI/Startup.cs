using System;
using System.Web.Http;
using Microsoft.Extensions.DependencyInjection;

namespace OneSCodeChanger.DI
{
    public static class Startup
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static void Init(HttpConfiguration httpConfiguration)
        {
            IServiceProvider serviceProvider = new ServiceCollection()
                .ConfigureServices()
                .AddControllersAsServices()
                .BuildServiceProvider();
            ServiceProvider = serviceProvider;
            httpConfiguration.DependencyResolver = new DefaultDependencyResolver(ServiceProvider);

        }
    }
}
