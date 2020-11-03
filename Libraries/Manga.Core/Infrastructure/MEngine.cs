using Autofac;
using Manga.Core.Data;
using Manga.Core.Domain;
using Manga.Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Manga.Core.Infrastructure
{
    /// <summary>
    /// Engine
    /// </summary>
    public class MEngine : IEngine
    {
        #region Fields

        private ContainerManager _containerManager;

        #endregion

        #region Utilities

        /// <summary>
        /// Run startup tasks
        /// </summary>
        protected virtual void RunStartupTasks()
        {
            var typeFinder = _containerManager.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
                startUpTask.Execute();
        }

        /// <summary>
        /// Register dependencies
        /// </summary>
        /// <param name="config">Config</param>
        protected virtual void RegisterDependencies(MConfig config)
        {
            var builder = new ContainerBuilder();
            //var container = builder.Build();

            //we create new instance of ContainerBuilder
            //because Build() or Update() method can only be called once on a ContainerBuilder.


            //dependencies
            var typeFinder = new WPFAppTypeFinder(config);

            builder = new ContainerBuilder();
            builder.RegisterInstance(config).As<MConfig>().SingleInstance();
            builder.RegisterInstance(this).As<IEngine>().SingleInstance();
            builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();
            //builder.Update(container);

            //register dependencies provided by other assemblies
            //builder = new ContainerBuilder();
            var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            var drInstances = new List<IDependencyRegistrar>();
            foreach (var drType in drTypes)
                drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));
            //sort
            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            foreach (var dependencyRegistrar in drInstances)
                dependencyRegistrar.Register(builder, typeFinder);
            //builder.Update(container);
            var container = builder.Build();

            _containerManager = new ContainerManager(container);
            
            //set dependency resolver
            //DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        protected virtual void CreateSettings()
        {
            var settingsManager = new DataSettingsManager();
            try
            {
                string connectionString = null;

                //SQL CE
                string databaseFileName = "MangaSharp.Db.sdf";
                string databasePath = @"|DataDirectory|\" + databaseFileName;
                connectionString = "Data Source=" + databasePath + ";Persist Security Info=False";
                
                string databaseFullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, databaseFileName);

                //save settings
                var dataProvider = "sqlce";
                var settings = new DataSettings()
                {
                    DataProvider = dataProvider,
                    DataConnectionString = connectionString
                };
                settingsManager.SaveSettings(settings);

                //init data provider
                var dataProviderInstance = EngineContext.Current.Resolve<BaseDataProviderManager>().LoadDataProvider();
                dataProviderInstance.InitDatabase();

                //reset cache
                DataSettingsHelper.ResetCache();
            }
            catch(Exception exception)
            {
                //reset cache
                DataSettingsHelper.ResetCache();

                //clear provider settings if something got wrong
                settingsManager.SaveSettings(new DataSettings
                {
                    DataProvider = null,
                    DataConnectionString = null
                });
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize components and plugins in the environment.
        /// </summary>
        /// <param name="config">Config</param>
        public void Initialize(MConfig config)
        {
            //register dependencies
            RegisterDependencies(config);

            //check settings.txt
            if(!System.IO.File.Exists("Settings.txt"))
            {
                CreateSettings();
            }

            //startup tasks
            if (!config.IgnoreStartupTasks)
            {
                RunStartupTasks();
            }

        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T : class
        {
            return ContainerManager.Resolve<T>();
        }

        /// <summary>
        ///  Resolve dependency
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Container manager
        /// </summary>
        public ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion
    }
}
