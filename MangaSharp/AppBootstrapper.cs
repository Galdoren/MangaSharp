using Caliburn.Metro.Autofac;
using Caliburn.Micro;
using Manga.Core.Data;
using Manga.Core.Infrastructure;
using Manga.Core.Infrastructure.DependencyManagement;
using MangaSharp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using System.Reflection;
using Autofac.Core;
using Autofac.Features.Metadata;
using Autofac.Extras.Attributed;
using MangaSharp.Infrastructure;

namespace MangaSharp
{
    public class AppBootstrapper : CaliburnMetroAutofacBootstrapper<AppViewModel>
    {
        new protected IContainer Container { get; set; }

        protected override void Configure()
        {
            //  allow base classes to change bootstrapper settings
            ConfigureBootstrapper();

            // initialize Engine
            //EngineContext.Initialize(false);

            //  validate settings
            if (CreateWindowManager == null)
                throw new ArgumentNullException("CreateWindowManager");
            if (CreateEventAggregator == null)
                throw new ArgumentNullException("CreateEventAggregator");

            //  configure container
            var builder = new ContainerBuilder();

            //  register main screen view models
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                //  must be a type with a name that ends with ViewModel
              .Where(type => type.Name.EndsWith("ViewModel"))
                //  must be in a namespace ending with ViewModels
              .Where(type => EnforceNamespaceConvention ? (!(string.IsNullOrWhiteSpace(type.Namespace)) && type.Namespace.EndsWith("ViewModels")) : true)
                //  must implement INotifyPropertyChanged (deriving from PropertyChangedBase will statisfy this)
              .Where(type => type.GetInterface(ViewModelBaseType.Name, false) != null)
                // must implement custom IMainScreenTabItem interface
              .Where(type => type.GetInterface(typeof(IViewModel).Name) != null && !type.IsAbstract && type.IsClass)                
                //  registered as view model
              .As<IViewModel>()
                //  allow metadata filter
              .WithAttributeFilter()
                //  always create a new one
              .InstancePerDependency();

            //  register views
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                //  must be a type with a name that ends with View
              .Where(type => type.Name.EndsWith("View"))
                //  must be in a namespace that ends in Views
              .Where(type => EnforceNamespaceConvention ? (!(string.IsNullOrWhiteSpace(type.Namespace)) && type.Namespace.EndsWith("Views")) : true)
                //  registered as self
              .AsSelf()
                //  always create a new one
              .InstancePerDependency();

            //  register the single window manager for this container
            builder.Register<IWindowManager>(c => CreateWindowManager()).InstancePerLifetimeScope();
            //  register the single event aggregator for this container
            builder.Register<IEventAggregator>(c => CreateEventAggregator()).InstancePerLifetimeScope();

            

            //  should we install the auto-subscribe event aggregation handler module?
            if (AutoSubscribeEventAggegatorHandlers)
                builder.RegisterModule<EventAggregationAutoSubscriptionModule>();

            //  allow derived classes to add to the container
            ConfigureContainer(builder);

            builder.RegisterModule<AttributedMetadataModule>();

            builder.Update(EngineContext.Current.ContainerManager.Container);
            
            this.Container = EngineContext.Current.ContainerManager.Container;
        }

        protected override void ConfigureContainer(Autofac.ContainerBuilder builder)
        {
            var config = new TypeMappingConfiguration
            {
                DefaultSubNamespaceForViewModels = "MangaSharp.ViewModels",
                DefaultSubNamespaceForViews = "MangaSharp.Views"
            };
            ViewLocator.ConfigureTypeMappings(config);
            ViewModelLocator.ConfigureTypeMappings(config);

            builder.RegisterType<AppWindowManager>().As<IWindowManager>().SingleInstance();

            
            // register manga window view model
            builder.RegisterType<MangaViewModel>()
                // register as self
                .AsSelf()
                .WithAttributeFilter()
                .InstancePerDependency();
            
            // register AppViewModel
            builder.RegisterType<AppViewModel>()
                // register as self
                .AsSelf()
                .WithAttributeFilter()
                // singleton
                .SingleInstance();
            
            /*
            var viewModels = Assembly.GetExecutingAssembly()
                .DefinedTypes.Where(x => x.GetInterface(typeof(IMainScreenTabItem).Name) != null && !x.IsAbstract && x.IsClass)
                .Select(x => x.Assembly);
            builder.RegisterAssemblyTypes(viewModels.ToArray())
                .As<IMainScreenTabItem>()
                .SingleInstance();
            */
            /*
            var assembly = typeof(AppViewModel).Assembly;
            
            builder.RegisterAssemblyTypes(assembly)
                .Where(item => item.Name.EndsWith("ViewModel") && !item.IsAbstract && item.IsClass)
                .AsSelf()
                .SingleInstance();*/
        }

        protected override object GetInstance(Type service, string key)
        {
            object instance;
            if (string.IsNullOrWhiteSpace(key))
            {
                if (EngineContext.Current.ContainerManager.TryResolve(service, null, out instance))
                    return instance;
            }
            else
            {
                if (Container.TryResolveNamed(key, service, out instance))
                    return instance;
            }
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name));
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;

            //return base.GetAllInstances(service);
        }
    }
}
