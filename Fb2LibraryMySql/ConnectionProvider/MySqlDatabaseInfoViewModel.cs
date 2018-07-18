using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using Devart.Data.MySql;
using Fb2Library.Entities;
using Fb2Library.Providers;
using Fb2Library.ViewModels;

namespace Fb2Library.MySql.ConnectionProvider
{
    [Export(ProviderName.MySql, typeof(DatabaseInfoViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class MySqlDatabaseInfoViewModel : DatabaseInfoViewModel, IPartImportsSatisfiedNotification
    {
        private readonly IConnectionProvider connectionProvider;

        [ImportingConstructor]
        public MySqlDatabaseInfoViewModel(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }

        public void OnImportsSatisfied()
        {
            if (!String.IsNullOrEmpty(connectionProvider.ConnectionString))
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(connectionProvider.ConnectionString);
                
                base.DatabaseName = builder.Database;

                using (DbManager db = connectionProvider.CreateDbManager(connectionProvider.ConnectionString))
                {
                    DatabaseInfo info = db.GetTable<DatabaseInfo>().First();

                    base.NamingPattern = info.NamingPattern;
                    base.DatabaseId = info.DatabaseId;
                    base.SchemaVersion = info.Version;
                    base.MountPoint = info.MountPoint;
                    base.WorkingMode = info.WorkMode;
                }
            }
        }
    }
}
