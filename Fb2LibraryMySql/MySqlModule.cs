using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fb2Library.Modularity;

namespace Fb2Library.MySql
{
    [ModuleExport(typeof(MySqlModule), FriendlyName = "MySql Server Tools", Description = "The module adds support for MySql databases", InitializationMode = InitializationMode.WhenAvailable)]
    public class MySqlModule : IModule
    {
        public void Initialize()
        {
        }
    }
}
