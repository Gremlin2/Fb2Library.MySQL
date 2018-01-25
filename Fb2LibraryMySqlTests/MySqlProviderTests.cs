using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Fb2Library.MySql.ConnectionProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fb2Library.MySqlTests
{
    [TestClass]
    public class MySqlProviderTests
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            AggregateCatalog catalog = new AggregateCatalog();

            catalog.Catalogs.Add(new AssemblyCatalog(typeof(MySqlConnectionFactory).Assembly));

            CompositionContainer container = new CompositionContainer(catalog);

            CompositionBatch batch = new CompositionBatch();

            batch.AddExportedValue(container);

            //string typeIdentity = AttributedModelServices.GetTypeIdentity(typeof(IGenreTable));
            //IDictionary<string, object> metadata = new Dictionary<string, object>();
            //metadata.Add(CompositionConstants.ExportTypeIdentityMetadataName, typeIdentity);

            //batch.AddExport(new Export("Default", metadata, () => new GenreTable().Default));

            container.Compose(batch);

            testContext.Properties["Containter"] = container;
        }


        [TestMethod]
        public void CreateDatabase()
        {
            CompositionContainer container = (CompositionContainer)TestContext.Properties["Containter"];
            MySqlConnectionFactory factory = container.GetExportedValue<MySqlConnectionFactory>();
            CreateMySqlDbOptions options = new CreateMySqlDbOptions();

            MySqlConnectionSettings connectionSettings = new MySqlConnectionSettings();
            connectionSettings.ServerName = @"localhost";
            connectionSettings.Database = "testfb";
            connectionSettings.Username = "root";
            connectionSettings.PlainTextPassword = "root";

            factory.CreateDatabase(connectionSettings, options);
        }
    }
}
