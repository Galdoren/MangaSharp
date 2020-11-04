using Manga.Core.Data;
using Manga.Data.Sql.Initializers;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlServerCe;

namespace Manga.Data.Sql
{
    public class SqlCeDataProvider : IDataProvider
    {
        #region Properties

        /// <summary>
        /// A value indicating whether this data provider supports stored procedures
        /// </summary>
        public bool StoredProceduredSupported
        {
            get { return false; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize connection factory
        /// </summary>
        public void InitConnectionFactory()
        {
            var connectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
            //TODO fix compilation warning (below)
#pragma warning disable 0618
            Database.DefaultConnectionFactory = connectionFactory;
        }

        /// <summary>
        /// Initialize database
        /// </summary>
        public void InitDatabase()
        {
            InitConnectionFactory();
            SetDatabaseInitializer();
        }

        /// <summary>
        /// Set database initializer
        /// </summary>
        public void SetDatabaseInitializer()
        {
            var initializer = new CreateCeDatabaseIfNotExists<MObjectContext>();
            //var initializer = new DropCreateCeDatabaseIfModelChanges<MObjectContext>();
            Database.SetInitializer(initializer);
        }

        /// <summary>
        /// Gets a support database parameter object (used by stored procedures)
        /// </summary>
        /// <returns>Parameter</returns>
        public DbParameter GetParameter()
        {
            return new SqlCeParameter();
        }

        public DbCommand GetCommand()
        {
            return new SqlCeCommand();
        }

        #endregion
    }
}
