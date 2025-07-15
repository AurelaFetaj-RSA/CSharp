using Owin;
using RSACommon.WebApiDefinitions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Owin.Builder;
using WebApi.Controllers;
using RSACommon;
using System.Reflection;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using SimpleInjector.Integration.WebApi;

namespace WebApi
{
    public class WebApiConfigurator
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            var container = new Container();            
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.RegisterInstance<ISharedList>(WebApiCore.SharedList);
            container.RegisterWebApiControllers(config);
            // 3. Verify the container's configuration.
            container.Verify();

            //container.AddService(typeof(ISharedList), WebApiCore.SharedList);
            //container.AddService(typeof(RecipeController), (sc, t) => new RecipeController((ISharedList)sc.GetService(typeof(ISharedList))));

            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
            config.MapHttpAttributeRoutes();           

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{userID}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        }
    }
}
