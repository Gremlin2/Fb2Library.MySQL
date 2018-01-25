using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fb2Library.Commands;

namespace Fb2Library.MySql.ConnectionProvider
{
    [ExportCommand("Fb2Library.ConnectToDbMenu", LabelTitle = "MySQL Server")]
    public class ConnectToMySqlServerCommand : LibraryCommand, IBackstageTab
    {
        public ConnectToMySqlServerCommand()
            : base("ConnectToMySqlServerCommand", typeof(MySqlConnectionFactory))
        {
        }

        public object Content
        {
            get
            {
                return new MySqlConnectionSettings();
            }
        }
    }
}
