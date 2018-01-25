using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLToolkit.Data.DataProvider;
using Fb2Library.Data.Migrations;

namespace Fb2Library.MySql.ConnectionProvider
{
    [MigrationExport(ProviderName.MySql, 1, IsInitial = true)]
    internal class MySqlInitialMigration : Migration
    {
        public override void Up()
        {
            CreateTable("AUTOR").WithColumns
            (
                Column.Int("AUTORID").AsPrimaryKey().AutoIncrement(),
                Column.Int("SOURCEID"),
                Column.String("FIRSTNAME", 254),
                Column.String("MIDNAME", 254),
                Column.String("LASTNAME", 254),
                Column.String("NICKNAME", 254),
                Column.Text("DISPLAYNAME"),
                Column.Text("EMAIL"),
                Column.Text("HOMEPAGE"),
                Column.Binary("PHOTO"),
                Column.Text("INFO"),
                Column.Text("BIBLIOGRAPHI"),
                Column.String("LIBRARYID", 40)
            )
            .Index(IndexedColumn("FIRSTNAME"))
            .Index(IndexedColumn("LASTNAME"))
            .Index(IndexedColumn("NICKNAME"))
            .Unique("SOURCEID");

            CreateTable("AUTOR_SYNONIMS").WithColumns
            (
                Column.Int("ID").AsPrimaryKey().AutoIncrement(),
                Column.Int("AUTORID").NotNull(),
                Column.String("LASTNAME", 254),
                Column.String("FIRSTNAME", 254),
                Column.String("MIDNAME", 254),
                Column.String("NICKNAME", 254),
                Column.SmallInt("INLIST").NotNull().DefaultValue(0)
            )
            .ForeignKey("AUTORID").References("AUTOR").OnUpdate(ReferentialAction.Cascade).OnDelete(ReferentialAction.Cascade)
            .Index("AUTORID")
            .Index(IndexedColumn("LASTNAME"))
            .Index(IndexedColumn("FIRSTNAME"));

            CreateTable("SEQUENCES").WithColumns
            (
                Column.Int("SEQUENCEID").AsPrimaryKey().AutoIncrement(),
                Column.Int("SOURCEID"),
                Column.String("SEQUENCE", 125).NotNull(),
                Column.Int("PARENTID")
            )
            .Unique(IndexedColumn("SEQUENCE"))
            .Unique("SOURCEID")
            .ForeignKey("PARENTID").References("SEQUENCES").OnColumns("SEQUENCEID").OnUpdate(ReferentialAction.NoAction).OnDelete(ReferentialAction.NoAction)
            .Index("PARENTID");

            CreateTable("USERS").WithColumns
            (
                Column.Int("USERID").AsPrimaryKey().AutoIncrement(),
                Column.String("USERNAME", 40).NotNull(),
                Column.Int("OWNER"),
                Column.Int("CANEDIT"),
                Column.String("PASS", 12)
            )
            .Unique("USERNAME");

            CreateTable("BOOK").WithColumns
            (
                Column.Int("BOOKID").AsPrimaryKey().AutoIncrement(),
                Column.Int("SOURCEID"),
                Column.Text("GENRELIST"),
                Column.Text("ANNOTATION"),
                Column.Text("AUTORLIST"),
                Column.Text("KEYWORDS"),
                Column.String("BOOKNAME", 252),
                Column.String("SRCTITLE", 252),
                Column.Date("DATEVALUE"),
                Column.String("DATEVISIBLE", 25),
                Column.String("LANG", 10),
                Column.Binary("COVERPAGE"),
                Column.String("SRCLANG", 10),
                Column.String("SEQUENCE", 125),
                Column.String("SEQNUMBER", 4),
                Column.Int("SEQUENCEID"),
                Column.String("DI_PROGUSED", 254),
                Column.Date("DI_DATEVALUE"),
                Column.String("DI_DATEVISIBLE", 25),
                Column.String("DI_SRCURL", 254),
                Column.String("DI_SRCOCR", 254),
                Column.String("DI_VERSION", 10),
                Column.Text("DI_HISTORY"),
                Column.String("PI_BOOKNAME", 254),
                Column.String("PI_PUBLISHER", 254),
                Column.String("PI_CITY", 50),
                Column.String("PI_YEAR", 10),
                Column.String("PI_ISBN", 125),
                Column.DateTime("DATEIN"),
                Column.DateTime("DATEUPDATED"),
                Column.String("EXT", 5),
                Column.BigInt("FILESIZE"),
                Column.String("FILENAME", 1024),
                Column.String("FILEPATH", 1024),
                Column.DateTime("FILEDATE"),
                Column.Int("USERID"),
                Column.String("OLDID")
            )
            .ForeignKey("SEQUENCEID").References("SEQUENCES").OnUpdate(ReferentialAction.Cascade).OnDelete(ReferentialAction.SetNull)
            .ForeignKey("USERID").References("USERS").OnUpdate(ReferentialAction.Cascade).OnDelete(ReferentialAction.SetNull)
            .Index("SEQUENCEID")
            .Index("USERID")
            .Index("DATEIN")
            .Index("DATEUPDATED")
            .Index("OLDID").WithName("IX_BOOK_OLDID")
            .Unique("SOURCEID");

            CreateTable("CUSTOMINFO").WithColumns
            (
                Column.Int("ID").AsPrimaryKey().AutoIncrement(),
                Column.Int("BOOKID").NotNull(),
                Column.String("TYPE").NotNull(),
                Column.Text("VALUE")
            )
            .ForeignKey("BOOKID").References("BOOK").OnUpdate(ReferentialAction.Cascade).OnDelete(ReferentialAction.Cascade)
            .Index("BOOKID");

            CreateTable("DOCAUTHOR").WithColumns
            (
                Column.Int("DOCAUTORID").AsPrimaryKey().AutoIncrement(),
                Column.String("FIRSTNAME", 254),
                Column.String("MIDNAME", 254),
                Column.String("LASTNAME", 254),
                Column.String("NICKNAME", 254),
                Column.Text("EMAIL"),
                Column.Text("HOMEPAGE"),
                Column.Binary("PHOTO"),
                Column.Text("INFO")
            )
            .Index(IndexedColumn("FIRSTNAME"))
            .Index(IndexedColumn("LASTNAME"))
            .Index(IndexedColumn("NICKNAME"));

            CreateTable("TRANSLATE").WithColumns
            (
                Column.Int("TRANSLATEID").AsPrimaryKey().AutoIncrement(),
                Column.String("FIRSTNAME", 254),
                Column.String("MIDNAME", 254),
                Column.String("LASTNAME", 254),
                Column.String("NICKNAME", 254),
                Column.Text("EMAIL"),
                Column.Text("HOMEPAGE")
            )
            .Index(IndexedColumn("FIRSTNAME"))
            .Index(IndexedColumn("LASTNAME"))
            .Index(IndexedColumn("NICKNAME"));

            CreateTable("GRGENRE").WithColumns
            (
                Column.String("GRGENREID", 30).AsPrimaryKey(),
                Column.String("NAME", 30),
                Column.String("ENGNAME", 30)
            );

            CreateTable("GENRE").WithColumns
            (
                Column.String("GRGENREID", 30).NotNull(),
                Column.String("GENREID", 25).AsPrimaryKey(),
                Column.String("RUSNAME", 51),
                Column.String("ENGNAME", 50)
            )
            .ForeignKey("GRGENREID").References("GRGENRE").OnDelete(ReferentialAction.Cascade).OnUpdate(ReferentialAction.Cascade)
            .Index("GRGENREID");

            CreateTable("BOOK_AUTOR").WithColumns
            (
                Column.Int("ID").AsPrimaryKey().AutoIncrement(),
                Column.Int("BOOKID").NotNull(),
                Column.Int("AUTORID").NotNull(),
                Column.Int("SORTKEY").NotNull().DefaultValue(0)
            )
            .ForeignKey("BOOKID").References("BOOK").OnDelete(ReferentialAction.Cascade)
            .ForeignKey("AUTORID").References("AUTOR").OnDelete(ReferentialAction.Cascade)
            .Index("BOOKID")
            .Index("AUTORID");

            CreateTable("BOOK_DOCAUTOR").WithColumns
            (
                Column.Int("ID").AsPrimaryKey().AutoIncrement(),
                Column.Int("BOOKID").NotNull(),
                Column.Int("DOCAUTORID").NotNull(),
                Column.Int("SORTKEY").NotNull().DefaultValue(0)
            )
            .ForeignKey("BOOKID").References("BOOK").OnDelete(ReferentialAction.Cascade)
            .ForeignKey("DOCAUTORID").References("DOCAUTHOR").OnDelete(ReferentialAction.Cascade)
            .Index("BOOKID")
            .Index("DOCAUTORID");

            CreateTable("BOOK_GENRE").WithColumns
            (
                Column.Int("ID").AsPrimaryKey().AutoIncrement(),
                Column.Int("BOOKID").NotNull(),
                Column.String("GENREID", 25).NotNull(),
                Column.Int("SORTKEY").NotNull().DefaultValue(0)
            )
            .ForeignKey("BOOKID").References("BOOK").OnDelete(ReferentialAction.Cascade)
            .ForeignKey("GENREID").References("GENRE").OnDelete(ReferentialAction.Cascade)
            .Index("BOOKID")
            .Index("GENREID");

            CreateTable("BOOK_SEQUENCE").WithColumns
            (
                Column.Int("ID").AsPrimaryKey().AutoIncrement(),
                Column.Int("BOOKID").NotNull(),
                Column.Int("SEQUENCEID").NotNull(),
                Column.String("SEQNUMBER"),
                Column.Int("SORTKEY").NotNull().DefaultValue(0)
            )
            .ForeignKey("BOOKID").References("BOOK").OnDelete(ReferentialAction.Cascade)
            .ForeignKey("SEQUENCEID").References("SEQUENCES").OnDelete(ReferentialAction.Cascade)
            .Index("BOOKID")
            .Index("SEQUENCEID");

            CreateTable("BOOK_TRANSLATE").WithColumns
            (
                Column.Int("ID").AsPrimaryKey().AutoIncrement(),
                Column.Int("BOOKID").NotNull(),
                Column.Int("TRANSLATEID").NotNull(),
                Column.Int("SORTKEY").NotNull().DefaultValue(0)
            )
            .ForeignKey("BOOKID").References("BOOK").OnDelete(ReferentialAction.Cascade)
            .ForeignKey("TRANSLATEID").References("TRANSLATE").OnDelete(ReferentialAction.Cascade)
            .Index("BOOKID")
            .Index("TRANSLATEID");

            CreateTable("CITATE").WithColumns
            (
                Column.Int("ID").AsPrimaryKey().AutoIncrement(),
                Column.Int("BOOKID").NotNull(),
                Column.Int("USERID").NotNull(),
                Column.String("USERNAME", 40),
                Column.Text("NOTES"),
                Column.DateTime("DATENOTES")
            )
            .ForeignKey("BOOKID").References("BOOK").OnDelete(ReferentialAction.Cascade)
            .ForeignKey("USERID").References("USERS").OnDelete(ReferentialAction.Cascade)
            .Index("BOOKID")
            .Index("USERID");

            CreateTable("HISTORY").WithColumns
            (
                Column.Int("ID").AsPrimaryKey().AutoIncrement(),
                Column.Int("BOOKID").NotNull(),
                Column.Int("USERID").NotNull(),
                Column.Int("READED"),
                Column.Date("DATEREAD"),
                Column.Int("FAVORITS")
            )
            .ForeignKey("BOOKID").References("BOOK").OnDelete(ReferentialAction.Cascade)
            .ForeignKey("USERID").References("USERS").OnDelete(ReferentialAction.Cascade)
            .Index("BOOKID")
            .Index("USERID");

            CreateTable("NOTES").WithColumns
            (
                Column.Int("ID").AsPrimaryKey().AutoIncrement(),
                Column.Int("BOOKID").NotNull(),
                Column.Int("USERID").NotNull(),
                Column.String("USERNAME", 40),
                Column.Text("NOTES"),
                Column.DateTime("DATENOTES")
            )
            .ForeignKey("BOOKID").References("BOOK").OnDelete(ReferentialAction.Cascade)
            .ForeignKey("USERID").References("USERS").OnDelete(ReferentialAction.Cascade)
            .Index("BOOKID")
            .Index("USERID");

            CreateTable("TOREAD").WithColumns
            (
                Column.Int("ID").AsPrimaryKey().AutoIncrement(),
                Column.Int("BOOKID").NotNull(),
                Column.Int("USERID").NotNull(),
                Column.SmallInt("READED").NotNull().DefaultValue(0)
            )
            .ForeignKey("BOOKID").References("BOOK").OnDelete(ReferentialAction.Cascade)
            .ForeignKey("USERID").References("USERS").OnDelete(ReferentialAction.Cascade)
            .Index("BOOKID")
            .Index("USERID");

            CreateTable("GRADE").WithColumns
            (
                Column.Int("ID").AsPrimaryKey().AutoIncrement(),
                Column.Int("BOOKID").NotNull(),
                Column.Int("USERID"),
                Column.Int("POINT").NotNull()
            )
            .ForeignKey("BOOKID").References("BOOK").OnDelete(ReferentialAction.Cascade)
            .ForeignKey("USERID").References("USERS").OnDelete(ReferentialAction.Cascade)
            .Index("BOOKID")
            .Index("USERID");

            CreateTable("COLLECTIONINFO").WithColumns
            (
                Column.String("DB_ID", 40).AsPrimaryKey(),
                Column.String("COLLECTION_ID", 40),
                Column.Text("UPDATE_URL"),
                Column.String("VERSION", 8),
                Column.Int("LAST_BOOKID")
            );

            CreateTable("SCHEMAINFO").WithColumns
            (
                Column.String("PREFIX", 3).AsPrimaryKey(),
                Column.String("VERSION", 15).NotNull()
            );

            CreateTable("VERINFO").WithColumns
            (
                Column.String("VERSION"),
                Column.SmallInt("MODE"),
                Column.String("MOUNT"),
                Column.String("FILENAMINGPATTERN"),
                Column.String("DB_ID")
            );

            CreateTable("COLLECTION").WithColumns
            (
                Column.Int("COLLECTIONID").AsPrimaryKey().AutoIncrement(),
                Column.Int("PARENTID"),
                Column.String("NAME").NotNull(),
                Column.Int("TYPE").NotNull().DefaultValue(0),
                Column.Text("FILTER"),

                Column.Int("LFT").NotNull().DefaultValue(0),
                Column.Int("RGT").NotNull().DefaultValue(0),

                Column.Guid("UID"),
                Column.Boolean("LIMITED").NotNull().DefaultValue(0),
                Column.Int("MAX_LIMIT").NotNull().DefaultValue(500)
            )
            .ForeignKey("PARENTID").References("COLLECTION").OnColumns("COLLECTIONID").OnDelete(ReferentialAction.NoAction)
            .Index("PARENTID")
            .Index("LFT").WithName("IX_COLLECTION_LFT")
            .Index("RGT").WithName("IX_COLLECTION_RGT")
            .Unique("UID").WithName("IX_COLLECTION_UID");

            CreateTable("BOOK_COLLECTION").WithColumns
            (
                Column.Int("ID").AsPrimaryKey().AutoIncrement(),
                Column.Int("COLLECTIONID").NotNull(),
                Column.Int("BOOKID").NotNull(),
                Column.Int("USERID").NotNull()
            )
            .ForeignKey("COLLECTIONID").References("COLLECTION").OnDelete(ReferentialAction.Cascade)
            .ForeignKey("BOOKID").References("BOOK").OnDelete(ReferentialAction.Cascade)
            .ForeignKey("USERID").References("USERS").OnDelete(ReferentialAction.Cascade)
            .Index("COLLECTIONID")
            .Index("BOOKID")
            .Index("USERID")
            .Unique("COLLECTIONID", "BOOKID", "USERID");

            CreateTable("BOOK_LABEL").WithColumns
            (
                Column.Int("ID").AsPrimaryKey().AutoIncrement(),
                Column.Int("COLOR").NotNull(),
                Column.Int("BOOKID").NotNull(),
                Column.Int("USERID").NotNull()
            )
            .ForeignKey("BOOKID").References("BOOK").OnDelete(ReferentialAction.Cascade)
            .ForeignKey("USERID").References("USERS").OnDelete(ReferentialAction.Cascade)
            .Index("COLOR")
            .Index("BOOKID")
            .Index("USERID")
            .Unique("BOOKID", "USERID");

            CreateTable("BOOK_STORAGE").WithColumns
            (
                Column.Int("ID").AsPrimaryKey().AutoIncrement(),
                Column.Int("BOOKID").NotNull(),
                Column.String("EXT").NotNull(),
                Column.Binary("TEXT").NotNull()
            )
            .ForeignKey("BOOKID").References("BOOK").OnDelete(ReferentialAction.Cascade)
            .Unique("BOOKID", "EXT");

            CreateTable("IMPORT_HISTORY").WithColumns
            (
                Column.Int("IMPORTID").AsPrimaryKey().AutoIncrement(),
                Column.Int("USERID"),
                Column.DateTime("DATE")
            )
            .ForeignKey("USERID").References("USERS").OnDelete(ReferentialAction.Cascade)
            .Index("DATE");

            CreateTable("BOOK_IMPORT").WithColumns
            (
                Column.Int("IMPORTID").NotNull(),
                Column.Int("BOOKID").NotNull(),
                Column.Int("SEQUENCENO").NotNull().DefaultValue(0),
                Column.Int("ACTION").NotNull()
            )
            .PrimaryKey("IMPORTID", "BOOKID")
            .ForeignKey("IMPORTID").References("IMPORT_HISTORY").OnDelete(ReferentialAction.Cascade)
            .ForeignKey("BOOKID").References("BOOK").OnDelete(ReferentialAction.Cascade);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
