using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using Devart.Data.MySql;
using Fb2Library.Data;
using Fb2Library.Data.Migrations;
using Fb2Library.Entities;
using Fb2Library.Import;
using Fb2Library.Providers;
using Fb2Library.Services;

namespace Fb2Library.MySql.ConnectionProvider
{
    [Export(typeof(MySqlConnectionFactory))]
    [ExportConnectionFactory(ProviderName.MySql, ConnectionSettings = typeof(MySqlConnectionSettings), CreateDbOptions = typeof(CreateMySqlDbOptions))]
    internal class MySqlConnectionFactory : IConnectionFactory
    {
        private sealed class MySqlConnectionProvider : IConnectionProvider
        {
            private static readonly DataProviderBase provider = new MySqlDataProvider();

            private readonly CompositionContainer container;
            private readonly IEnumerable<ExportFactory<Migration, IMigrationMetadata>> migrations;
            private readonly string connectionString;

            private readonly Lazy<IStorageService> storageFactory;
            private readonly Lazy<ISchemaVersion> schemaVersion;

            public MySqlConnectionProvider(CompositionContainer container, IEnumerable<ExportFactory<Migration, IMigrationMetadata>> migrations, MySqlConnectionStringBuilder builder)
            {
                Contract.Requires(container != null);
                Contract.Requires(migrations != null);
                Contract.Requires(builder != null);

                this.container = container;
                this.migrations = migrations;

                this.connectionString = builder.ToString();

                this.storageFactory = new Lazy<IStorageService>(() =>
                {
                    using (DbManager db = new DbManager(provider, connectionString))
                    {
                        var query = from i in db.GetTable<DatabaseInfo>()
                                    select new DatabaseInfo { WorkMode = i.WorkMode };

                        DatabaseInfo databaseInfo = query.FirstOrDefault();
                        if (databaseInfo != null)
                        {
                            switch (databaseInfo.WorkMode)
                            {
                                case 0:
                                    return container.GetExportedValue<IDatabaseStorage>();

                                case 1:
                                    return container.GetExportedValue<IFileSystemStorage>();

                                case 2:
                                    return container.GetExportedValue<IExternalStorage>();

                                default:
                                    return null;
                            }
                        }
                    }

                    return null;
                });

                this.schemaVersion = new Lazy<ISchemaVersion>(() =>
                {
                    using (DbManager db = new DbManager(provider, connectionString))
                    {
                        DatabaseInfo info = db.GetTable<DatabaseInfo>().First();
                        if (!String.IsNullOrEmpty(info.Version))
                        {
                            Version version;
                            if (Version.TryParse(info.Version, out version))
                            {
                                return db.GetTable<SchemaInfo>().Where(x => x.Prefix == @"FBL").Select(x => new MySqlSchemaVersion(version, x.Version)).FirstOrDefault();
                            }
                        }
                    }

                    return null;
                });
            }

            public string ConnectionString
            {
                get
                {
                    return connectionString;
                }
            }

            public string DataProviderName
            {
                get
                {
                    return provider.Name;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public bool IsEmbedded
            {
                get
                {
                    return false;
                }
            }

            public bool IsUnicode
            {
                get
                {
                    return true;
                }
            }

            public ISchemaVersion SchemaVersion
            {
                get
                {
                    return schemaVersion.Value;
                }
            }

            public void Reconnect()
            {
                throw new NotImplementedException();
            }

            public void ClearAllPools()
            {
                MySqlConnection.ClearAllPools();
            }

            public AbstractMigrator CreateMigrator()
            {
                SqlHistoryRepository repository = new SqlHistoryRepository(this);
                return new MySqlMigrator(this, migrations, repository);
            }

            public IDatabaseAccessProvider CreateAccessProvider()
            {
                return container.GetExportedValue<IStandardDataAccessProvider>();
            }

            public IStorageService CreateStorage()
            {
                return storageFactory.Value;
            }

            public IImportCommandFactory CreateCommandFactory()
            {
                throw new NotImplementedException();
            }

            public DbManager CreateDbManager(string connectionString)
            {
                return new DbManager(provider, connectionString);
            }

            public DbManager CreateDbManager(IDbConnection connection)
            {
                return new DbManager(provider, connection);
            }
        }

        private sealed class MySqlConnectionProviderFactory : IConnectionProviderFactory
        {
            private readonly CompositionContainer container;
            private readonly IEnumerable<ExportFactory<Migration, IMigrationMetadata>> migrations;
            private readonly MySqlConnectionStringBuilder builder;

            public MySqlConnectionProviderFactory(CompositionContainer container, IEnumerable<ExportFactory<Migration, IMigrationMetadata>> migrations, MySqlConnectionStringBuilder builder)
            {
                this.container = container;
                this.migrations = migrations;
                this.builder = builder;
            }

            public IConnectionProvider CreateProvider()
            {
                return new MySqlConnectionProvider(container, migrations, builder);
            }
        }

        private readonly CompositionContainer container;
        private readonly IEnumerable<ExportFactory<Migration, IMigrationMetadata>> migrations;
        private readonly IGenreTable defaultTable;

        [ImportingConstructor]
        public MySqlConnectionFactory(CompositionContainer container, [ImportMany(ProviderName.MySql)] IEnumerable<ExportFactory<Migration, IMigrationMetadata>> migrations, [Import("Default")] IGenreTable defaultTable)
        {
            Contract.Assume(container != null);
            Contract.Assume(migrations != null);
            Contract.Assume(defaultTable != null);

            this.container = container;
            this.migrations = migrations;
            this.defaultTable = defaultTable;
        }

        public IConnectionProviderFactory CreateFactory(string filename)
        {
            throw new NotSupportedException();
        }

        public IConnectionProviderFactory CreateFactory(ConnectionSettings connectionSettings)
        {
            if (connectionSettings == null)
            {
                throw new ArgumentNullException("connectionSettings");
            }

            if (!(connectionSettings is MySqlConnectionSettings))
            {
                throw new ArgumentException("", "connectionSettings");
            }

            Contract.EndContractBlock();

            MySqlConnectionSettings settings = (MySqlConnectionSettings) connectionSettings;

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();

            builder.Host = settings.ServerName;
            builder.UserId = settings.Username;
            builder.Password = settings.PlainTextPassword;
            builder.Database = settings.Database;
            builder.Unicode = true;
            builder.Pooling = true;

            return new MySqlConnectionProviderFactory(container, migrations, builder);
        }

        public bool IsSameConnection(IConnectionProvider provider, string filename)
        {
            return false;
        }

        public bool IsSameConnection(IConnectionProvider provider, ConnectionSettings connectionSettings)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (connectionSettings == null)
            {
                throw new ArgumentNullException("connectionSettings");
            }

            Contract.EndContractBlock();

            if (provider is MySqlConnectionProvider)
            {
                MySqlConnectionSettings settings = connectionSettings as MySqlConnectionSettings;
                if (settings == null)
                {
                    return false;
                }

                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(provider.ConnectionString);

                if (String.Compare(settings.ServerName, builder.Host, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    return false;
                }

                if (String.Compare(settings.Database, builder.Database, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    return false;
                }

                if (String.Compare(settings.Username ?? "", builder.UserId, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public void CreateDatabase(string filename, ICreateDbOptions options)
        {
            throw new NotSupportedException();
        }

        private void CreateTables(MySqlConnection connection, ICreateDbOptions options)
        {
            Contract.Requires(connection != null);
            Contract.Requires(options != null);

            SqlHistoryRepository repository = new SqlHistoryRepository(new MySqlDataProvider(), connection);
            AbstractMigrator migrator = new MySqlMigrator(connection, migrations, repository);

            migrator.Upgrade();

            string commandText = "INSERT INTO VERINFO VALUES (@version, @mode, @mountpoint, @pattern, @id)";
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.CommandType = CommandType.Text;

                
                command.Parameters.Add("@version", MySqlType.VarChar, 15).Value = "2.0.0.0";
                command.Parameters.Add("@mode", MySqlType.SmallInt).Value = options.WorkMode;
                command.Parameters.Add("@mountpoint", MySqlType.VarChar, 4096).Value = options.MountPoint;
                command.Parameters.Add("@pattern", MySqlType.VarChar, 252).Value = options.NamingPattern;
                command.Parameters.Add("@id", MySqlType.VarChar, 40).Value = options.DatabaseId;

                command.ExecuteNonQuery();
            }

            commandText = "INSERT INTO USERS (USERNAME, OWNER, PASS, CANEDIT) VALUES ('owner', 1, '', 1)";
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }

            commandText = "INSERT INTO COLLECTION (NAME, TYPE, LFT, RGT) VALUES ('root', -1, 1, 2)";
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.CommandType = CommandType.Text;

                command.ExecuteNonQuery();
            }

            using (MySqlTransaction transaction = connection.BeginTransaction())
            {
                commandText = "INSERT INTO GRGENRE (GRGENREID, NAME, ENGNAME) VALUES (@genreid, @runame, @enname)";

                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = commandText;

                    command.Transaction = transaction;

                    command.Parameters.Add("@genreid", MySqlType.VarChar, 30);
                    command.Parameters.Add("@runame", MySqlType.VarChar, 30);
                    command.Parameters.Add("@enname", MySqlType.VarChar, 30);

                    command.Prepare();

                    foreach (IGenreInfo genre in defaultTable.Groups)
                    {
                        command.Parameters["@genreid"].Value = genre.Name;
                        command.Parameters["@runame"].Value = genre.GetDescription("ru");
                        command.Parameters["@enname"].Value = genre.GetDescription("en");

                        command.ExecuteNonQuery();
                    }
                }

                commandText = "INSERT INTO GENRE (GRGENREID, GENREID, RUSNAME, ENGNAME) VALUES (@groupid, @genreid, @runame, @enname)";
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = commandText;

                    command.Transaction = transaction;

                    command.Parameters.Add("@groupid", MySqlType.VarChar, 30);
                    command.Parameters.Add("@genreid", MySqlType.VarChar, 25);
                    command.Parameters.Add("@runame", MySqlType.VarChar, 51);
                    command.Parameters.Add("@enname", MySqlType.VarChar, 50);

                    command.Prepare();

                    foreach (IGenreInfo genre in defaultTable)
                    {
                        command.Parameters["@groupid"].Value = genre.Group.Name;
                        command.Parameters["@genreid"].Value = genre.Name;
                        command.Parameters["@runame"].Value = genre.GetDescription("ru");
                        command.Parameters["@enname"].Value = genre.GetDescription("en");

                        command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }

        public void CreateDatabase(ConnectionSettings connectionSettings, ICreateDbOptions options)
        {
            if (connectionSettings == null)
            {
                throw new ArgumentNullException("connectionSettings");
            }

            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            if (!(connectionSettings is MySqlConnectionSettings))
            {
                throw new ArgumentException("", "connectionSettings");
            }

            Contract.EndContractBlock();

            MySqlConnectionSettings settings = (MySqlConnectionSettings) connectionSettings;

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();

            builder.Host = settings.ServerName;
            builder.UserId = settings.Username;
            builder.Password = settings.PlainTextPassword;
            builder.Unicode = true;
            builder.Pooling = false;

            using (MySqlConnection connection = new MySqlConnection(builder.ToString()))
            {
                connection.Open();

                string commandText = String.Format(@"USE `{0}`;", settings.Database);

                try
                {
                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = commandText;

                        command.ExecuteNonQuery();
                    }
                }
                catch (MySqlException e)
                {
                    if (e.Code != 1049)
                    {
                        throw;
                    }

                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = String.Format(@"CREATE DATABASE `{0}`;", settings.Database);

                        command.ExecuteNonQuery();
                    }
                }
            }

            builder.Database = settings.Database;

            using (MySqlConnection connection = new MySqlConnection(builder.ToString()))
            {
                connection.Open();

                CreateTables(connection, options);
            }
        }
    }
}
