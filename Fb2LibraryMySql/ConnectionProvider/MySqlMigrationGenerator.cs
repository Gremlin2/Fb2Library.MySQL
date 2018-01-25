using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Fb2Library.Data.Migrations;

namespace Fb2Library.MySql.ConnectionProvider
{
    internal class MySqlMigrationGenerator : MigrationSqlGenerator
    {
        private const int DefaultMaxLength = 128;
        private const byte DefaultScale = 0;

        public MySqlMigrationGenerator(IEnumerable<MigrationOperation> operations)
            : base(operations)
        {
        }

        protected override string Name(string name)
        {
            return name;
        }

        protected override string Quote(string value)
        {
            return "`" + value + "`";
        }

        protected override void BuildColumnType(TextWriter writer, ColumnModel column)
        {
            switch (column.Type)
            {
                case SqlType.Guid:
                    writer.Write("CHAR(16) BINARY");
                    break;

                case SqlType.TinyInt:
                    writer.Write("TINYINT");
                    break;

                case SqlType.SmallInt:
                    writer.Write("SMALLINT");
                    break;

                case SqlType.Int:
                    writer.Write("INT");
                    break;

                case SqlType.BigInt:
                    writer.Write("BIGINT");
                    break;

                case SqlType.Single:
                    writer.Write("FLOAT");
                    break;

                case SqlType.Double:
                    writer.Write("DOUBLE");
                    break;

                case SqlType.Decimal:
                    writer.Write("DECIMAL");
                    if (column.Precision != null)
                    {
                        writer.Write("({0},{1})", column.Precision, column.Scale ?? DefaultScale);
                    }
                    break;

                case SqlType.Currency:
                    writer.Write("DECIMAL(18,4)");
                    break;

                case SqlType.Boolean:
                    writer.Write("BOOLEAN");
                    break;

                case SqlType.Char:
                    writer.Write("CHAR(");

                    if (column.MaxLength != null)
                    {
                        writer.Write(column.MaxLength);
                    }
                    else
                    {
                        writer.Write(DefaultMaxLength);
                    }

                    writer.Write(")");

                    if (!String.IsNullOrEmpty(column.CharacterSet))
                    {
                        writer.Write(" CHARSET " + column.CharacterSet);
                    }

                    if (!String.IsNullOrEmpty(column.Collate))
                    {
                        writer.Write(" COLLATE " + column.Collate);
                    }
                    break;

                case SqlType.VarChar:
                case SqlType.VarCharMax:
                    writer.Write("VAR");
                    goto case SqlType.Char;

                case SqlType.NChar:
                    goto case SqlType.Char;

                case SqlType.NVarChar:
                case SqlType.NVarCharMax:
                    writer.Write("VAR");
                    goto case SqlType.Char;

                case SqlType.Text:
                    writer.Write("MEDIUMTEXT");

                    if (!String.IsNullOrEmpty(column.CharacterSet))
                    {
                        writer.Write(" CHARSET " + column.CharacterSet);
                    }

                    if (!String.IsNullOrEmpty(column.Collate))
                    {
                        writer.Write(" COLLATE " + column.Collate);
                    }
                    break;

                case SqlType.NText:
                    goto case SqlType.Text;

                case SqlType.Xml:
                    goto case SqlType.Text;

                case SqlType.Date:
                    writer.Write("DATE");
                    break;

                case SqlType.Time:
                    writer.Write("TIME");
                    break;

                case SqlType.DateTime:
                    writer.Write("DATETIME");
                    break;

                case SqlType.TimeStamp:
                    goto case SqlType.DateTime;

                case SqlType.TimeSpan:
                    goto case SqlType.DateTime;

                case SqlType.Binary:
                    writer.Write("BINARY(");

                    if (column.MaxLength != null)
                    {
                        writer.Write(column.MaxLength);
                    }
                    else
                    {
                        writer.Write(DefaultMaxLength);
                    }

                    writer.Write(")");
                    break;

                case SqlType.VarBinary:
                    writer.Write("MEDIUMBLOB");
                    break;

                case SqlType.Custom:
                    writer.Write(column.Sql);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void Generate(CreateTableOperation operation, ColumnModel column, IndentedTextWriter writer)
        {
            writer.Write(Quote(column.Name));
            writer.Write(" ");

            BuildColumnType(writer, column);

            bool isNullable = column.IsNullable;

            if (operation.PrimaryKey != null)
            {
                AddPrimaryKeyOperation primaryKey = operation.PrimaryKey;

                if (primaryKey.Columns.Count == 1 && primaryKey.Columns.FirstOrDefault((x) => x == column.Name) != null)
                {
                    var createSequence = MigrationOperations.OfType<CreateSequenceOperation>().FirstOrDefault(x => x.TableName == operation.TableName && x.ColumnName == column.Name);
                    if (createSequence != null)
                    {
                        writer.Write(" AUTO_INCREMENT");

                        MigrationOperations.Remove(createSequence);
                    }

                    writer.Write(" PRIMARY KEY");

                    primaryKey.Columns.Remove(column.Name);
                    if (primaryKey.Columns.Count == 0)
                    {
                        operation.PrimaryKey = null;
                    }

                    isNullable = true;
                }
            }

            if (column.HasOptions)
            {
                foreach (string option in column.ColumnOptions)
                {
                    writer.Write(" ");
                    writer.Write(option);
                }
            }

            if (!isNullable)
            {
                writer.Write(" NOT NULL");
            }

            if (column.DefaultValue != null)
            {
                writer.Write(" DEFAULT ");
                writer.Write(Generate((dynamic)column.DefaultValue));
            }
        }

        protected override void Generate(UpdateVersionOperation operation)
        {
            using (var writer = Writer())
            {
                writer.WriteLine("INSERT INTO SCHEMAINFO (PREFIX, VERSION) VALUES ('FBL', {0})", operation.Version);
                writer.WriteLine("ON DUPLICATE KEY UPDATE VERSION = {0}", operation.Version);

                Statement(writer);
            }
        }
    }
}
