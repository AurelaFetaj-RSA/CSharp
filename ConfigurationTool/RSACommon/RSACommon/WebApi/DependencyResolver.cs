using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;

namespace WebApi
{

    public class DependencyResolver : IDependencyResolver, IDependencyScope
    {
        readonly IServiceProvider serviceProvider;

        public DependencyResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IDependencyScope BeginScope() => this;

        public void Dispose() { }

        public object GetService(Type serviceType) => serviceProvider.GetService(serviceType);

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var services = serviceProvider.GetService(serviceType);
            var sequence = services as IEnumerable;

            if (sequence != null)
            {
                foreach (object service in sequence)
                {
                    yield return service;
                }
            }
            else
            {
                yield return services;
            }
        }
    }
}
