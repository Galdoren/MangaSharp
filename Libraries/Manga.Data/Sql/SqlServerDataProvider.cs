using Manga.Core.Data;
using Manga.Data.Sql.Initializers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace Manga.Data.Sql
{
    public class SqlServerDataProvider : IDataProvider
    {
        #region Properties

        /// <summary>
        /// A value indicating whether this data provider supports stored procedures
        /// </summary>
        public bool StoredProceduredSupported
        {
            get { return true; }
        }

        #endregion

        #region Utilities

        protected virtual string[] ParseCommands(string filePath, bool throwExceptionIfNonExists)
        {
            if (!File.Exists(filePath))
            {
                if (throwExceptionIfNonExists)
                    throw new ArgumentException(string.Format("Specified file doesn't exist - {0}", filePath));
                else
                    return new string[0];
            }

            var statements = new List<string>();
            using (var stream = File.OpenRead(filePath))
            using (var reader = new StreamReader(stream))
            {
                var statement = "";
                while ((statement = ReadNextStatementFromStream(reader)) != null)
                    statements.Add(statement);
            }

            return statements.ToArray();
        }

        protected virtual string ReadNextStatementFromStream(StreamReader reader)
        {
            var builder = new StringBuilder();

            string line;

            while (true)
            {
                line = reader.ReadLine();
                if (line == null)
                {
                    if (builder.Length > 0)
                        return builder.ToString();
                    return null;
                }

                if (line.TrimEnd().ToUpper().Equals("GO"))
                    break;

                builder.AppendLine(line);
            }

            return builder.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize connection factory
        /// </summary>
        public void InitConnectionFactory()
        {
            var connectionFactory = new SqlConnectionFactory();
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
            //pass some tables to ensre that we have Where's the Part At installed
            var tablesToValidate = new[] { "User", "Circle", "Circle_Update" };

            //custom commands (stored procedures, indexes)

            var customCommands = new List<string>();
            //use webHelper.MapPath instead of HostingEnvironment.MapPath which is not available in unit tests

            customCommands.AddRange(ParseCommands(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SQL/SqlServer.Indexes.sql"), false));
            //customCommands.AddRange(ParseCommands(HostingEnvironment.MapPath("~/App_Data/SqlServer.Indexes.sql"), false));
            //use webHelper.MapPath instead of HostingEnvironment.MapPath which is not available in unit tests
            //customCommands.AddRange(ParseCommands(HostingEnvironment.MapPath("~/App_Data/SqlServer.StoredProcedures.sql"), false));
            customCommands.AddRange(ParseCommands(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SQL/SqlServer.StoredProcedures.sql"), false));

            var initializer = new CreateTablesIfNotExist<MObjectContext>(tablesToValidate, customCommands.ToArray());
            Database.SetInitializer(initializer);
        }

        /// <summary>
        /// Gets a support database parameter object (used by stored procedures)
        /// </summary>
        /// <returns>Parameter</returns>
        public DbParameter GetParameter()
        {
            return new SqlParameter();
        }

        public DbCommand GetCommand()
        {
            return new SqlCommand();
        }

        #endregion
    }
}
