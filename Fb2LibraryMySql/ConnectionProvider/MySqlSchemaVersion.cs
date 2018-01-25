using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Fb2Library.Providers;

namespace Fb2Library.MySql.ConnectionProvider
{
    internal sealed class MySqlSchemaVersion : AbstractSchemaVersion
    {
        private readonly Version dbVersion;
        private readonly string version;

        public MySqlSchemaVersion(Version dbVersion, string version)
        {
            Contract.Requires(dbVersion != null);

            this.dbVersion = dbVersion;
            this.version = version;
        }

        public override Version DbVersion
        {
            get
            {
                return dbVersion;
            }
        }

        public override string Version
        {
            get
            {
                return version;
            }
        }

        public override bool HasLibraryId
        {
            get
            {
                return true;
            }
        }

        public override bool HasMappingTables
        {
            get
            {
                return true;
            }
        }

        public override bool HasCollectionInfoTable
        {
            get
            {
                return true;
            }
        }

        public override bool HasLastBookIdColumn
        {
            get
            {
                return false;
            }
        }

        public override bool HasCollectionTable
        {
            get
            {
                return true;
            }
        }

        public override bool HasBookLabelTable
        {
            get
            {
                return true;
            }
        }

        public override bool HasBookStorageTable
        {
            get
            {
                return true;
            }
        }

        public override bool HasSrcTitleColumn
        {
            get
            {
                return true;
            }
        }

        public override bool HasImportHistoryTable
        {
            get
            {
                return true;
            }
        }
    }
}
