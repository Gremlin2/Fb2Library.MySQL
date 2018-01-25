using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Devart.Data.MySql;
using Fb2Library.Data;

namespace Fb2Library.MySql.ConnectionProvider
{
    [XmlRoot("MySqlConnectionSettings")]
    public class MySqlConnectionSettings : ConnectionSettings
    {
        public MySqlConnectionSettings()
        {
            Username = "root";
            PlainTextPassword = "root";
        }
    }
}
