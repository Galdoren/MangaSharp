using Caliburn.Micro;
using Manga.Core.Infrastructure.DependencyManagement;
using MangaSharp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Manga.Core.Data;
using Manga.Data;
using Manga.Services.Chapters;
using Manga.Services.Mangas;
using Manga.Services.Publishers;
using Manga.Core.Plugins;
using MangaSharp.Services;
using Manga.Services.Net;

namespace MangaSharp.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(Autofac.ContainerBuilder builder, Manga.Core.Infrastructure.ITypeFinder typeFinder)
        {
            //data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();
            builder.Register(c => dataSettingsManager.LoadSettings()).As<DataSettings>();
            builder.Register(x => new EfDataProviderManager(x.Resolve<DataSettings>())).As<BaseDataProviderManager>().InstancePerDependency();

            builder.Register(x => x.Resolve<BaseDataProviderManager>().LoadDataProvider()).As<IDataProvider>().InstancePerDependency();

            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                var efDataProviderManager = new EfDataProviderManager(dataSettingsManager.LoadSettings());
                var dataProvider = efDataProviderManager.LoadDataProvider();
                dataProvider.InitConnectionFactory();

                builder.Register<IDbContext>(c => new MObjectContext(dataProviderSettings.DataConnectionString)).InstancePerLifetimeScope();
            }
            else
            {
                builder.Register<IDbContext>(c => new MObjectContext(dataSettingsManager.LoadSettings().DataConnectionString)).InstancePerLifetimeScope();
            }

            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

            //plugins
            builder.RegisterType<PluginFinder>().As<IPluginFinder>().InstancePerLifetimeScope();

            //services
            builder.RegisterType<ChapterService>().As<IChapterService>().InstancePerLifetimeScope();
            builder.RegisterType<MangaService>().As<IMangaService>().InstancePerLifetimeScope();
            builder.RegisterType<PublisherService>().As<IPublisherService>().InstancePerLifetimeScope();

            //download manager
            builder.RegisterType<AsyncDownloadManager>().As<IDownloadManager>().SingleInstance();
        }

        public int Order
        {
            get { return 2; }
        }
    }
}
