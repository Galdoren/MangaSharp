using Autofac;
using Manga.Core.Infrastructure.DependencyManagement;
using Manga.Plugin.Publishers.Manganelo.Services;
using Manga.Services.Publishers;
using System;

namespace Manga.Plugin.Publishers.Manganelo
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, Core.Infrastructure.ITypeFinder typeFinder)
        {
            builder.RegisterType<ManganeloService>().Keyed<IPublisherWebService>("Manganelo").SingleInstance();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
