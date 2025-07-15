using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi
{
    public class WebApiServiceContaner : IServiceContainer
    {
        readonly Dictionary<Type, ServiceCreatorCallback> services = new Dictionary<Type, ServiceCreatorCallback>();
        public void AddService(Type serviceType, object serviceInstance) => AddService(serviceType, serviceInstance, true);
        public void AddService(Type serviceType, object serviceInstance, bool promote) => services[serviceType] = (sc, t) => serviceInstance;
        public void AddService(Type serviceType, ServiceCreatorCallback callback) => AddService(serviceType, callback, true);
        public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) => services[serviceType] = callback;

        public object GetService(Type serviceType)
        {
            ServiceCreatorCallback callback;

            if (services.TryGetValue(serviceType, out callback))
            {
                return callback(this, serviceType);
            }

            return null;
        }

        public void RemoveService(Type serviceType) => RemoveService(serviceType, true);

        public void RemoveService(Type serviceType, bool promote) => services.Remove(serviceType);
    }
}
