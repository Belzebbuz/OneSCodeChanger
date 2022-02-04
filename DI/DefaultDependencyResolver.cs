using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace OneSCodeChanger.DI
{
    internal class DefaultDependencyResolver : IDependencyResolver
    {
        private IServiceScope serviceScope;
        protected IServiceProvider ServiceProvider { get; set; }

        public DefaultDependencyResolver(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            return this.ServiceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.ServiceProvider.GetServices(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            serviceScope = this.ServiceProvider.CreateScope();
            return new DefaultDependencyResolver(serviceScope.ServiceProvider);
        }

        public void Dispose()
        {
            // you can implement this interface just when you use .net core 2.0
            // this.ServiceProvider.Dispose();

            //need to dispose the scope otherwise
            //you'll get a memory leak
            serviceScope?.Dispose();
        }
    }
}