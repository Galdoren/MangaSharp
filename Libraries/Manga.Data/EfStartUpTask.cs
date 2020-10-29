using Manga.Core.Data;
using Manga.Core.Infrastructure;
using System;

namespace Manga.Data
{
    public class EfStartUpTask : IStartupTask
    {
        public void Execute()
        {
            var settings = EngineContext.Current.Resolve<DataSettings>();
            if (settings != null && settings.IsValid())
            {
                var provider = EngineContext.Current.Resolve<IDataProvider>();
                if (provider == null)
                    throw new Exception("No IDataProvider found");
                provider.SetDatabaseInitializer();
            }
        }

        public int Delay
        {
            //ensure that this taks doesn't get delayed
            get { return 0; }
        }

        public int Order
        {
            //ensure that this task is run first 
            get { return -1000; }
        }
    }
}
