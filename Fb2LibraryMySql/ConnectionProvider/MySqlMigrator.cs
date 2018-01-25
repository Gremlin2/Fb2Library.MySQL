using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using Devart.Data.MySql;
using Fb2Library.Data.Migrations;
using Fb2Library.Providers;

namespace Fb2Library.MySql.ConnectionProvider
{
    internal class MySqlMigrator : AbstractMigrator
    {
        private readonly MySqlConnection connection;
        private readonly IConnectionProvider provider;

        public MySqlMigrator(IConnectionProvider provider, IEnumerable<ExportFactory<Migration, IMigrationMetadata>> migrations, IHistoryRepository historyRepository) : base(migrations, historyRepository)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            Contract.EndContractBlock();

            this.provider = provider;
        }

        public MySqlMigrator(MySqlConnection connection, IEnumerable<ExportFactory<Migration, IMigrationMetadata>> migrations, IHistoryRepository historyRepository)
            : base(migrations, historyRepository)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            Contract.EndContractBlock();

            this.connection = connection;
        }

        private DbManager CreateDbManager()
        {
            if (provider != null)
            {
                return provider.CreateDbManager(provider.ConnectionString);
            }

            return new DbManager(new MySqlDataProvider(), connection);
        }

        protected override MigrationGenerator CreateGenerator(IEnumerable<MigrationOperation> operations)
        {
            return new MySqlMigrationGenerator(operations);
        }

        protected override void ExecuteStatements(IEnumerable<MigrationStatement> statements)
        {
            using (DbManager db = CreateDbManager())
            {
                using (IDbTransaction transaction = db.BeginTransaction(IsolationLevel.Serializable))
                {
                    foreach (MigrationStatement statement in statements)
                    {
                        MigrationSqlStatement sqlStatement = statement as MigrationSqlStatement;
                        if (sqlStatement != null)
                        {
                            try
                            {
                                db.SetCommand(sqlStatement.Sql);
                                db.ExecuteNonQuery();
                            }
                            catch (Exception e)
                            {
                                throw;
                            }
                        }
                        else
                        {
                            statement.Execute();
                        }
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
