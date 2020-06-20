using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using TheCodeCamp.Data;

namespace TheCodeCamp
{
    public class AutofacConfig
    {
        public static void Register()
        {
            var builder = new ContainerBuilder();
            var config = GlobalConfiguration.Configuration;

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            RegisterServices(builder);
            builder.RegisterWebApiFilterProvider(config);
            builder.RegisterWebApiModelBinderProvider();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            var automapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CampMappingProfile());
            })
            .CreateMapper();

            builder.RegisterInstance(automapper)
                .As<IMapper>()
                .SingleInstance();

            builder.RegisterType<CampContext>()
                .InstancePerRequest();

            builder.RegisterType<CampRepository>()
                .As<ICampRepository>()
                .InstancePerRequest();
        }
    }
}
