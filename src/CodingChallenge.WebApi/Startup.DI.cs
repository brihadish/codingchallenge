using Autofac;
using Autofac.Integration.WebApi;
using System.Reflection;
using System.Web.Http;

namespace CodingChallenge.WebApi
{
    public partial class Startup
    {
        public void ConfigureDependencyInjection(HttpConfiguration configuration)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(_zipCodeIndexer);
            builder.RegisterInstance(_locationIndexer);
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            configuration.DependencyResolver = new AutofacWebApiDependencyResolver(builder.Build());
        }
    }
}
