using Autofac;
using Manga.Core.Infrastructure.DependencyManagement;
using Manga.Plugin.Publishers.Mangafox.Services;
using Manga.Services.Publishers;
using System;

namespace Manga.Plugin.Publishers.Mangafox
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, Core.Infrastructure.ITypeFinder typeFinder)
        {
            builder.RegisterType<MangafoxService>().Keyed<IPublisherWebService>("Mangafox").SingleInstance();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
