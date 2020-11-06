using Caliburn.Metro.Autofac;
using Caliburn.Micro;
using Manga.Core.Infrastructure;
using MangaSharp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extras.AttributeMetadata;
using Autofac.Features.AttributeFilters;
using System.Windows.Threading;
using Manga.Common.Enums;
using Manga.Common.Interfaces;

namespace MangaSharp
{
    public class AppBootstrapper : CaliburnMetroAutofacBootstrapper<AppViewModel>
    {
        protected new IContainer Container { get; set; }

        protected override void Configure()
        {
            //  allow base classes to change bootstrapper settings
            ConfigureBootstrapper();

            // initialize Engine
            //EngineContext.Initialize(false);

            //  validate settings
            if (CreateWindowManager == null)
                throw new ArgumentNullException(nameof(CreateWindowManager));
            if (CreateEventAggregator == null)
                throw new ArgumentNullException(nameof(CreateEventAggregator));

            //  configure container
            var builder = new ContainerBuilder();

            //  register main screen view models
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                //  must be a type with a name that ends with ViewModel
              .Where(type => type.Name.EndsWith("ViewModel"))
                //  must be in a namespace ending with ViewModels
              .Where(type => !EnforceNamespaceConvention || !string.IsNullOrWhiteSpace(type.Namespace) && type.Namespace.EndsWith("ViewModels"))
                //  must implement INotifyPropertyChanged (deriving from PropertyChangedBase will satisfy this)
              .Where(type => type.GetInterface(ViewModelBaseType.Name, false) != null)
                // must implement custom IMainScreenTabItem interface
              .Where(type => type.GetInterface(nameof(IViewModel)) != null && !type.IsAbstract && type.IsClass)                
                //  registered as view model
              .As<IViewModel>()
                //  allow metadata filter
              .WithAttributeFiltering()
                //  always create a new one
              .InstancePerDependency();

            //  register views
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                //  must be a type with a name that ends with View
              .Where(type => type.Name.EndsWith("View"))
                //  must be in a namespace that ends in Views
              .Where(type => !EnforceNamespaceConvention || !string.IsNullOrWhiteSpace(type.Namespace) && type.Namespace.EndsWith("Views"))
                //  registered as self
              .AsSelf()
                //  always create a new one
              .InstancePerDependency();

            //  register the single window manager for this container
            builder.Register(c => CreateWindowManager()).InstancePerLifetimeScope();
            //  register the single event aggregator for this container
            builder.Register(c => CreateEventAggregator()).InstancePerLifetimeScope();

            

            //  should we install the auto-subscribe event aggregation handler module?
            if (AutoSubscribeEventAggegatorHandlers)
                builder.RegisterModule<EventAggregationAutoSubscriptionModule>();

            //  allow derived classes to add to the container
            ConfigureContainer(builder);

            builder.RegisterModule<AttributedMetadataModule>();

            // TODO: Replace obsolete method
            builder.Update(EngineContext.Current.ContainerManager.Container);
            
            Container = EngineContext.Current.ContainerManager.Container;
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
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
                .WithAttributeFiltering()
                .InstancePerDependency();
            
            // register AppViewModel
            builder.RegisterType<AppViewModel>()
                // register as self
                .AsSelf()
                .WithAttributeFiltering()
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
            throw new Exception($"Could not locate any instances of contract {key ?? service.Name}.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;

            //return base.GetAllInstances(service);
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Container.Resolve<ILogger>().Error(e.Exception.ToString());
        }
    }
}
