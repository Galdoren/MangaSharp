using Manga.Core.Data;
using Manga.Core.Infrastructure;
using Manga.Core.Plugins;
using Manga.Services.Publishers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace MangaSharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            // Load Plugins
            PluginManager.Initialize();

            // initialize Engine
            EngineContext.Initialize(false);

            var databaseInstalled = DataSettingsHelper.DatabaseIsInstalled();

            if(databaseInstalled)
            {
                //init data provider
                var dataProviderInstance = EngineContext.Current.Resolve<BaseDataProviderManager>().LoadDataProvider();
                dataProviderInstance.InitDatabase();
            }

            // install plugins
            var pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();
            var plugins = pluginFinder.GetPlugins<IPlugin>(false)
                .ToList()
                .OrderBy(x => x.PluginDescriptor.Group)
                .ThenBy(x => x.PluginDescriptor.DisplayOrder)
                .ToList();
            var pluginsIgnoredDuringInstallation = String.IsNullOrEmpty(ConfigurationManager.AppSettings["PluginsIgnoredDuringInstallation"]) ?
                    new List<string>() :
                    ConfigurationManager.AppSettings["PluginsIgnoredDuringInstallation"]
                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList();
            foreach (var plugin in plugins)
            {
                if (pluginsIgnoredDuringInstallation.Contains(plugin.PluginDescriptor.SystemName))
                    continue;
                if(!plugin.PluginDescriptor.Installed)
                    plugin.Install();
            }

            // a little hack to initialize database on start
            if(databaseInstalled)
            {
                var publisherService = EngineContext.Current.Resolve<IPublisherService>();
                publisherService.GetAllPublishers();
            }
        }
    }
}
