using System;
using System.Data;
using System.Linq;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using                  MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
using XSqlDataReader = MySql.Data.MySqlClient.MySqlDataReader;
using XSqlException  = MySql.Data.MySqlClient.MySqlException;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
#endif

public sealed class ArtiklDao : VvDaoBase, IVvDao
{
   
   #region Singleton Constructor & instancer

   private static ArtiklDao instance;

   private ArtiklDao(XSqlConnection conn, string dbName) : base(dbName, Artikl.recordNameArhiva, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static ArtiklDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if (instance == null)
      {
         instance = new ArtiklDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableArtikl

   public static   uint TableVersionStatic { get { return 11; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition(bool isArhiva)
   {
      return 
      (
        "recID int(10) unsigned NOT NULL auto_increment                                       ,\n" +
        "addTS timestamp                 NULL DEFAULT NULL                        ,\n" +
        "modTS timestamp                 default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,\n" +
        "addUID varchar(16)     NOT NULL default 'XY'                                         ,\n" +
        "modUID varchar(16)     NOT NULL default ''                                           ,\n" +
                CreateTable_LanSrvID_And_LanRecID_Columns +

/* 05 */" artiklCD      varchar(32)             NOT NULL default '',\n" +   
/* 06 */" artiklName    varchar(80)             NOT NULL default '',\n" +   
/* 07 */" barCode1      varchar(16)             NOT NULL default '',\n" +     
/* 08 */" skladCD       varchar(6)              NOT NULL default '',\n" +  
/* 09 */" grupa1CD      varchar(6)              NOT NULL default '',\n" +     
/* 10 */" jedMj         varchar(12)             NOT NULL default '',\n" +      
/* 11 */" konto         varchar(16)             NOT NULL default '',\n" +
/* 12 */" artiklCD2     varchar(32)             NOT NULL default '',\n" +
/* 13 */" artiklName2   varchar(80)             NOT NULL default '',\n" +
/* 14 */" ts            varchar(6)              NOT NULL default '',\n" +
/* 15 */" barCode2      varchar(16)             NOT NULL default '',\n" +
/* 16 */" serNo         varchar(32)             NOT NULL default '',\n" +
/* 17 */" grupa2CD      varchar(6)              NOT NULL default '',\n" +
/* 18 */" grupa3CD      varchar(6)              NOT NULL default '',\n" +
/* 19 */" placement     varchar(16)             NOT NULL default '',\n" +
/* 20 */" linkArtCD     varchar(32)             NOT NULL default '',\n" +
/* 21 */" dateProizv    date                    NOT NULL default '0001-01-01',\n" +
/* 22 */" pdvKat        varchar(8)              NOT NULL default '',\n" +
/* 23 */" longOpis      varchar(2048)           NOT NULL default '',\n" +
/* 24 */" prefValName   char   (3)              NOT NULL default '',\n" +
/* 25 */" orgPak        varchar(8)              NOT NULL default '',\n" +
/* 26 */" isRashod      tinyint(1)     unsigned NOT NULL default '0',\n" +
/* 27 */" isAkcija      tinyint(1)     unsigned NOT NULL default '0',\n" +
/* 28 */" isMaster      tinyint(1)     unsigned NOT NULL default '0',\n" +
/* 29 */" masaNetto     decimal(10,3)           NOT NULL default '0.00',\n" +   
/* 30 */" masaBruto     decimal(10,3)           NOT NULL default '0.00',\n" +   
/* 31 */" promjer       decimal(10,3)           NOT NULL default '0.00',\n" +   
/* 32 */" povrsina      decimal(10,3)           NOT NULL default '0.00',\n" +   
/* 33 */" zapremina     decimal(10,3)           NOT NULL default '0.00',\n" +   
/* 34 */" duljina       decimal(10,3)           NOT NULL default '0.00',\n" +   
/* 35 */" sirina        decimal(10,3)           NOT NULL default '0.00',\n" +   
/* 36 */" visina        decimal(10,3)           NOT NULL default '0.00',\n" +   
/* 37 */" starost       decimal(10,2)           NOT NULL default '0.00',\n" +   
/* 38 */" boja          varchar(8)              NOT NULL default ''    ,\n" +   
/* 39 */" spol          tinyint(1)     unsigned NOT NULL default '0'   ,\n" +
/* 40 */" garancija     tinyint(2)     unsigned NOT NULL default '0'   ,\n" +
/* 41 */" dobavCD       int(6)         unsigned NOT NULL default '0'   ,\n" +
/* 42 */" proizCD       int(6)         unsigned NOT NULL default '0'   ,\n" +
/* 43 */" isAllowMinus  tinyint(1)     unsigned NOT NULL default '0',\n" +
/* 44 */" isSerNo       tinyint(1)     unsigned NOT NULL default '0',\n" +
/* 45 */" masaNettoJM   char   (3)              NOT NULL default '',\n" +
/* 46 */" masaBrutoJM   char   (3)              NOT NULL default '',\n" +
/* 47 */" promjerJM     char   (3)              NOT NULL default '',\n" +
/* 48 */" povrsinaJM    char   (3)              NOT NULL default '',\n" +
/* 59 */" zapreminaJM   char   (3)              NOT NULL default '',\n" +
/* 50 */" duljinaJM     char   (3)              NOT NULL default '',\n" +
/* 51 */" sirinaJM      char   (3)              NOT NULL default '',\n" +
/* 52 */" visinaJM      char   (3)              NOT NULL default '',\n" +
/* 53 */" velicina      varchar(16)             NOT NULL default ''    ,\n" +   
/* 54 */" madeIn        varchar(32)             NOT NULL default ''    ,\n" +   
/* 55 */" url           varchar(128)            NOT NULL default ''    ,\n" +   
/* 56 */" atestBr       varchar(32)             NOT NULL default ''    ,\n" +   
/* 57 */" atestDate     date                    NOT NULL default '0001-01-01',\n" +
/* 58 */" vpc1Policy    tinyint(1)     unsigned NOT NULL default '0'   ,\n" +
/* 59 */" isPrnOpis     tinyint(1)     unsigned NOT NULL default '0'   ,\n" +
/* 60 */" carTarifa     varchar(16)             NOT NULL default ''    ,\n" +   
/* 61 */" partNo        varchar(32)             NOT NULL default ''    ,\n" +   
/* 62 */" napomena      varchar(98)             NOT NULL default ''    ,\n" +   
/* 63 */" importCij     decimal(10,4)           NOT NULL default '0.00',\n" +   
/* 64 */" snaga         decimal(10,2)           NOT NULL default '0.00',\n" +   
/* 65 */" snagaJM       char   (3)              NOT NULL default ''    ,\n" +
/* 66 */" emisCO2       smallint(3)    unsigned NOT NULL default '0'   ,\n" +
/*67+2*/" euroNorma     tinyint(1)     unsigned NOT NULL default '0'   ,\n" +

         CreateTable_ArhivaExtensionDefinition(isArhiva) +

                                      "PRIMARY KEY    (recID     ),\n" +
        (isArhiva ? "" : "UNIQUE ") + "KEY BY_artCD   (artiklCD  )\n," +
        (isArhiva ? "" : "UNIQUE ") + "KEY BY_artName (artiklName)\n," +
                                      "KEY BY_barCode1(barCode1  )\n" +

          (isArhiva ? ", KEY BYqOrigRecID (origRecID)\n" : "")

      );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Artikl.recordNameArhiva;
      else         tableName = Artikl.recordName;

      switch(catchingVersion)
      {
         case 2: return ("ADD COLUMN napomena  varchar(98)           NOT NULL default '' AFTER partNo;\n");
         case 3: return ("DROP INDEX BY_artName, ADD UNIQUE INDEX BY_artName USING BTREE(artiklName);");
         case 4: return ("ADD COLUMN importCij     decimal(10,4)     NOT NULL default '0.00' AFTER napomena;\n");
         case 5: return ("DROP INDEX BY_artName, DROP INDEX BY_artCD, ADD " + (isArhiva ? "" : "UNIQUE ") + " INDEX BY_artCD (artiklCD), ADD " + (isArhiva ? "" : "UNIQUE ") + " INDEX BY_artName (artiklName);");
         case 6: return ("MODIFY COLUMN longOpis     varchar(2048)        NOT NULL default '';");

         case 7: return ("ADD COLUMN snaga         decimal(10,2)           NOT NULL default '0.00' AFTER importCij,    " +
                         "ADD COLUMN snagaJM       char   (3)              NOT NULL default ''     AFTER snaga;\n");

         case 8: return (isArhiva ? "ADD INDEX BYqOrigRecID (origRecID)\n" : "skip_me");

         case 9: return ("ADD COLUMN emisCO2       smallint(3)    unsigned NOT NULL default '0' AFTER snagaJM,  " +
                         "ADD COLUMN euroNorma     tinyint(1)     unsigned NOT NULL default '0' AFTER emisCO2;\n");

         case 10: return AlterTable_LanSrvID_And_LanRecID_Columns;

         case 11: return ("MODIFY COLUMN masaNetto     decimal(10,3)           NOT NULL default '0.00', " +
                          "MODIFY COLUMN masaBruto     decimal(10,3)           NOT NULL default '0.00', " +
                          "MODIFY COLUMN promjer       decimal(10,3)           NOT NULL default '0.00', " +
                          "MODIFY COLUMN povrsina      decimal(10,3)           NOT NULL default '0.00', " +
                          "MODIFY COLUMN zapremina     decimal(10,3)           NOT NULL default '0.00', " +
                          "MODIFY COLUMN duljina       decimal(10,3)           NOT NULL default '0.00', " +
                          "MODIFY COLUMN sirina        decimal(10,3)           NOT NULL default '0.00', " +
                          "MODIFY COLUMN visina        decimal(10,3)           NOT NULL default '0.00';\n");

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableArtikl

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      Artikl artikl = (Artikl)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.ID_Only ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         //VvSQL.CreateCommandParameter(cmd, where_or_and, "prjktKupdobCD", artikl.RecID, XSqlDbType.Int32, 10);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.RecID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         //DataRow theRow = TheSchemaTable.Rows.Find("modUID");

         VvSQL.CreateCommandParameter(cmd, preffix, artikl.AddTS,      TheSchemaTable.Rows[CI.addTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.ModTS,      TheSchemaTable.Rows[CI.modTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.AddUID,     TheSchemaTable.Rows[CI.addUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.ModUID,     TheSchemaTable.Rows[CI.modUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.LanSrvID,   TheSchemaTable.Rows[CI.lanSrvID]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.LanRecID,   TheSchemaTable.Rows[CI.lanRecID]);
   
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.ArtiklCD   , TheSchemaTable.Rows[CI.artiklCD   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.ArtiklName , TheSchemaTable.Rows[CI.artiklName ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.BarCode1   , TheSchemaTable.Rows[CI.barCode1   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.SkladCD    , TheSchemaTable.Rows[CI.skladCD    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Grupa1CD   , TheSchemaTable.Rows[CI.grupa1CD   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.JedMj      , TheSchemaTable.Rows[CI.jedMj      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Konto      , TheSchemaTable.Rows[CI.konto      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.ArtiklCD2  , TheSchemaTable.Rows[CI.artiklCD2  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.ArtiklName2, TheSchemaTable.Rows[CI.artiklName2]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.TS         , TheSchemaTable.Rows[CI.ts         ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.BarCode2   , TheSchemaTable.Rows[CI.barCode2   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.SerNo      , TheSchemaTable.Rows[CI.serNo      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Grupa2CD   , TheSchemaTable.Rows[CI.grupa2CD   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Grupa3CD   , TheSchemaTable.Rows[CI.grupa3CD   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Placement  , TheSchemaTable.Rows[CI.placement  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.LinkArtCD  , TheSchemaTable.Rows[CI.linkArtCD  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.DateProizv , TheSchemaTable.Rows[CI.dateProizv ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.PdvKat     , TheSchemaTable.Rows[CI.pdvKat     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.LongOpis   , TheSchemaTable.Rows[CI.longOpis   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.PrefValName, TheSchemaTable.Rows[CI.prefValName]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.OrgPak     , TheSchemaTable.Rows[CI.orgPak     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.IsRashod   , TheSchemaTable.Rows[CI.isRashod   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.IsAkcija   , TheSchemaTable.Rows[CI.isAkcija   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.IsMaster   , TheSchemaTable.Rows[CI.isMaster   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.MasaNetto  , TheSchemaTable.Rows[CI.masaNetto  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.MasaBruto  , TheSchemaTable.Rows[CI.masaBruto  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Promjer    , TheSchemaTable.Rows[CI.promjer    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Povrsina   , TheSchemaTable.Rows[CI.povrsina   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Zapremina  , TheSchemaTable.Rows[CI.zapremina  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Duljina    , TheSchemaTable.Rows[CI.duljina    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Sirina     , TheSchemaTable.Rows[CI.sirina     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Visina     , TheSchemaTable.Rows[CI.visina     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Starost    , TheSchemaTable.Rows[CI.starost    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Boja       , TheSchemaTable.Rows[CI.boja       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Spol       , TheSchemaTable.Rows[CI.spol       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Garancija   , TheSchemaTable.Rows[CI.garancija   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.DobavCD     , TheSchemaTable.Rows[CI.dobavCD     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.ProizCD     , TheSchemaTable.Rows[CI.proizCD     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.IsAllowMinus, TheSchemaTable.Rows[CI.isAllowMinus]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.IsSerNo     , TheSchemaTable.Rows[CI.isSerNo     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.MasaNettoJM , TheSchemaTable.Rows[CI.masaNettoJM ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.MasaBrutoJM , TheSchemaTable.Rows[CI.masaBrutoJM ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.PromjerJM   , TheSchemaTable.Rows[CI.promjerJM   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.PovrsinaJM  , TheSchemaTable.Rows[CI.povrsinaJM  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.ZapreminaJM , TheSchemaTable.Rows[CI.zapreminaJM ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.DuljinaJM   , TheSchemaTable.Rows[CI.duljinaJM   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.SirinaJM    , TheSchemaTable.Rows[CI.sirinaJM    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.VisinaJM    , TheSchemaTable.Rows[CI.visinaJM    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Velicina    , TheSchemaTable.Rows[CI.velicina    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.MadeIn      , TheSchemaTable.Rows[CI.madeIn      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Url         , TheSchemaTable.Rows[CI.url         ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.AtestBr     , TheSchemaTable.Rows[CI.atestBr     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.AtestDate   , TheSchemaTable.Rows[CI.atestDate   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Vpc1Policy  , TheSchemaTable.Rows[CI.vpc1Policy  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.IsPrnOpis   , TheSchemaTable.Rows[CI.isPrnOpis   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.CarTarifa   , TheSchemaTable.Rows[CI.carTarifa   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.PartNo      , TheSchemaTable.Rows[CI.partNo      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Napomena    , TheSchemaTable.Rows[CI.napomena    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.ImportCij   , TheSchemaTable.Rows[CI.importCij   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.Snaga       , TheSchemaTable.Rows[CI.snaga       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.SnagaJM     , TheSchemaTable.Rows[CI.snagaJM     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.EmisCO2     , TheSchemaTable.Rows[CI.emisCO2     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artikl.EuroNorma   , TheSchemaTable.Rows[CI.euroNorma   ]);

         if(isArhiva)
         {
            VvSQL.CreateCommandParameter(cmd, preffix, artikl.OrigRecID, TheSchemaTable.Rows[CI.origRecID]);
            VvSQL.CreateCommandParameter(cmd, preffix, artikl.RecVer,    TheSchemaTable.Rows[CI.recVer]);
            VvSQL.CreateCommandParameter(cmd, preffix, artikl.ArAction,  TheSchemaTable.Rows[CI.arAction]);
            VvSQL.CreateCommandParameter(cmd, preffix, artikl.ArTS,      TheSchemaTable.Rows[CI.arTS]);
            VvSQL.CreateCommandParameter(cmd, preffix, artikl.ArUID,     TheSchemaTable.Rows[CI.arUID]);
         }

      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva)
   {
      ArtiklStruct rdrData = new ArtiklStruct();

      rdrData._recID       = reader.GetUInt32  (CI.recID    );
      rdrData._addTS       = reader.GetDateTime(CI.addTS    );
      rdrData._modTS       = reader.GetDateTime(CI.modTS    );
      rdrData._addUID      = reader.GetString  (CI.addUID   );
      rdrData._modUID      = reader.GetString  (CI.modUID   );
      rdrData._lanSrvID    = reader.GetUInt32  (CI.lanSrvID );
      rdrData._lanRecID    = reader.GetUInt32  (CI.lanRecID );

      rdrData._artiklCD    = reader.GetString   (CI.artiklCD   );
      rdrData._artiklName  = reader.GetString   (CI.artiklName );
      rdrData._barCode1    = reader.GetString   (CI.barCode1   );
      rdrData._skladCD     = reader.GetString   (CI.skladCD    );
      rdrData._grupa1CD    = reader.GetString   (CI.grupa1CD   );
      rdrData._jedMj       = reader.GetString   (CI.jedMj      );
      rdrData._konto       = reader.GetString   (CI.konto      );
      rdrData._artiklCD2   = reader.GetString   (CI.artiklCD2  );
      rdrData._artiklName2 = reader.GetString   (CI.artiklName2);
      rdrData._ts          = reader.GetString   (CI.ts         );
      rdrData._barCode2    = reader.GetString   (CI.barCode2   );
      rdrData._serNo       = reader.GetString   (CI.serNo      );
      rdrData._grupa2CD    = reader.GetString   (CI.grupa2CD   );
      rdrData._grupa3CD    = reader.GetString   (CI.grupa3CD   );
      rdrData._placement   = reader.GetString   (CI.placement  );
      rdrData._linkArtCD   = reader.GetString   (CI.linkArtCD  );
      rdrData._dateProizv  = reader.GetDateTime (CI.dateProizv );
      rdrData._pdvKat      = reader.GetString   (CI.pdvKat     );
      rdrData._longOpis    = reader.GetString   (CI.longOpis   );
      rdrData._prefValName = reader.GetString   (CI.prefValName);
      rdrData._orgPak      = reader.GetString   (CI.orgPak     );
      rdrData._isRashod    = reader.GetBoolean  (CI.isRashod   );
      rdrData._isAkcija    = reader.GetBoolean  (CI.isAkcija   );
      rdrData._isMaster    = reader.GetBoolean  (CI.isMaster   );
      rdrData._masaNetto   = reader.GetDecimal  (CI.masaNetto  );
      rdrData._masaBruto   = reader.GetDecimal  (CI.masaBruto  );
      rdrData._promjer     = reader.GetDecimal  (CI.promjer    );
      rdrData._povrsina    = reader.GetDecimal  (CI.povrsina   );
      rdrData._zapremina   = reader.GetDecimal  (CI.zapremina  );
      rdrData._duljina     = reader.GetDecimal  (CI.duljina    );
      rdrData._sirina      = reader.GetDecimal  (CI.sirina     );
      rdrData._visina      = reader.GetDecimal  (CI.visina     );
      rdrData._starost     = reader.GetDecimal  (CI.starost    );
      rdrData._boja        = reader.GetString   (CI.boja       );
      rdrData._spol        = reader.GetUInt16   (CI.spol       );
      rdrData._garancija   = reader.GetUInt16   (CI.garancija   );
      rdrData._dobavCD     = reader.GetUInt32   (CI.dobavCD     );
      rdrData._proizCD     = reader.GetUInt32   (CI.proizCD     );
      rdrData._isAllowMinus= reader.GetBoolean  (CI.isAllowMinus);
      rdrData._isSerNo     = reader.GetBoolean  (CI.isSerNo     );
      rdrData._masaNettoJM = reader.GetString   (CI.masaNettoJM );
      rdrData._masaBrutoJM = reader.GetString   (CI.masaBrutoJM );
      rdrData._promjerJM   = reader.GetString   (CI.promjerJM   );
      rdrData._povrsinaJM  = reader.GetString   (CI.povrsinaJM  );
      rdrData._zapreminaJM = reader.GetString   (CI.zapreminaJM );
      rdrData._duljinaJM   = reader.GetString   (CI.duljinaJM   );
      rdrData._sirinaJM    = reader.GetString   (CI.sirinaJM    );
      rdrData._visinaJM    = reader.GetString   (CI.visinaJM    );
      rdrData._velicina    = reader.GetString   (CI.velicina    );
      rdrData._madeIn      = reader.GetString   (CI.madeIn      );
      rdrData._url         = reader.GetString   (CI.url         );
      rdrData._atestBr     = reader.GetString   (CI.atestBr     );
      rdrData._atestDate   = reader.GetDateTime (CI.atestDate   );
      rdrData._vpc1Policy  = reader.GetUInt16   (CI.vpc1Policy  );
      rdrData._isPrnOpis   = reader.GetBoolean  (CI.isPrnOpis   );
      rdrData._carTarifa   = reader.GetString   (CI.carTarifa   );
      rdrData._partNo      = reader.GetString   (CI.partNo      );
      rdrData._napomena    = reader.GetString   (CI.napomena    );
      rdrData._importCij   = reader.GetDecimal  (CI.importCij   );
      rdrData._snaga       = reader.GetDecimal  (CI.snaga       );
      rdrData._snagaJM     = reader.GetString   (CI.snagaJM     );
      rdrData._emisCO2     = reader.GetUInt16   (CI.emisCO2     );
      rdrData._euroNorma   = reader.GetUInt16   (CI.euroNorma   );
      
      ((Artikl)vvDataRecord).CurrentData = rdrData;

      if(isArhiva)
      {
         VvArhivaStruct rdrDataArhiva = new VvArhivaStruct();

         rdrDataArhiva._origRecID = reader.GetUInt32  (CI.origRecID);
         rdrDataArhiva._recVer    = reader.GetUInt32  (CI.recVer);
         rdrDataArhiva._arAction  = reader.GetString  (CI.arAction);
         rdrDataArhiva._arTS      = reader.GetDateTime(CI.arTS);
         rdrDataArhiva._arUID     = reader.GetString  (CI.arUID);

         vvDataRecord.TheArhivaData = rdrDataArhiva;
      }

      return;
   }

   #endregion FillFromDataReader

   #region FtrCI struct & InitializeSchemaColumnIndexes()

   public struct ArtiklCI
   {
      internal int recID;
      internal int addTS;
      internal int modTS;
      internal int addUID;
      internal int modUID;
      internal int lanSrvID;
      internal int lanRecID;

      internal int artiklCD   ;
      internal int artiklName ;
      internal int barCode1   ;
      internal int skladCD    ;
      internal int grupa1CD   ;
      internal int jedMj      ;
      internal int konto      ;
      internal int artiklCD2  ;
      internal int artiklName2;
      internal int ts         ;
      internal int barCode2   ;
      internal int serNo      ;
      internal int grupa2CD   ;
      internal int grupa3CD   ;
      internal int placement  ;
      internal int linkArtCD  ;
      internal int dateProizv ;
      internal int pdvKat     ;
      internal int longOpis   ;
      internal int prefValName;
      internal int orgPak     ;
      internal int isRashod   ;
      internal int isAkcija   ;
      internal int isMaster   ;
      internal int masaNetto;
      internal int masaBruto;
      internal int promjer  ;
      internal int povrsina ;
      internal int zapremina;
      internal int duljina  ;
      internal int sirina   ;
      internal int visina   ;
      internal int starost  ;
      internal int boja     ;
      internal int spol     ;
      internal int garancija   ;
      internal int dobavCD     ;
      internal int proizCD     ;
      internal int isAllowMinus;
      internal int isSerNo     ;
      internal int masaNettoJM ;
      internal int masaBrutoJM ;
      internal int promjerJM   ;
      internal int povrsinaJM  ;
      internal int zapreminaJM ;
      internal int duljinaJM   ;
      internal int sirinaJM    ;
      internal int visinaJM    ;
      internal int velicina    ;
      internal int madeIn      ;
      internal int url         ;
      internal int atestBr     ;
      internal int atestDate   ;
      internal int vpc1Policy  ;
      internal int isPrnOpis   ;
      internal int carTarifa   ;
      internal int partNo      ;
      internal int napomena    ;
      internal int importCij   ;
      internal int snaga       ;
      internal int snagaJM     ;
      internal int emisCO2     ;
      internal int euroNorma   ;

      internal int origRecID;
      internal int recVer;
      internal int arAction;
      internal int arTS;
      internal int arUID;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public ArtiklCI CI;
   public static int lastArtiklCI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID       = GetSchemaColumnIndex("recID");
      CI.addTS       = GetSchemaColumnIndex("addTS");
      CI.modTS       = GetSchemaColumnIndex("modTS");
      CI.addUID      = GetSchemaColumnIndex("addUID");
      CI.modUID      = GetSchemaColumnIndex("modUID");
      CI.lanSrvID    = GetSchemaColumnIndex("lanSrvID");
      CI.lanRecID    = GetSchemaColumnIndex("lanRecID");

      CI.artiklCD    = GetSchemaColumnIndex("artiklCD");
      CI.artiklName  = GetSchemaColumnIndex("artiklName");
      CI.barCode1    = GetSchemaColumnIndex("barCode1");
      CI.skladCD     = GetSchemaColumnIndex("skladCD");
      CI.grupa1CD    = GetSchemaColumnIndex("grupa1CD");
      CI.jedMj       = GetSchemaColumnIndex("jedMj");
      CI.konto       = GetSchemaColumnIndex("konto");
      CI.artiklCD2   = GetSchemaColumnIndex("artiklCD2");
      CI.artiklName2 = GetSchemaColumnIndex("artiklName2");
      CI.ts          = GetSchemaColumnIndex("ts");
      CI.barCode2    = GetSchemaColumnIndex("barCode2");
      CI.serNo       = GetSchemaColumnIndex("serNo");
      CI.grupa2CD    = GetSchemaColumnIndex("grupa2CD");
      CI.grupa3CD    = GetSchemaColumnIndex("grupa3CD");
      CI.placement   = GetSchemaColumnIndex("placement");
      CI.linkArtCD   = GetSchemaColumnIndex("linkArtCD");
      CI.dateProizv  = GetSchemaColumnIndex("dateProizv");
      CI.pdvKat      = GetSchemaColumnIndex("pdvKat");
      CI.longOpis    = GetSchemaColumnIndex("longOpis");
      CI.prefValName = GetSchemaColumnIndex("prefValName");
      CI.orgPak      = GetSchemaColumnIndex("orgPak");
      CI.isRashod    = GetSchemaColumnIndex("isRashod");
      CI.isAkcija    = GetSchemaColumnIndex("isAkcija");
      CI.isMaster    = GetSchemaColumnIndex("isMaster");
      CI.masaNetto   = GetSchemaColumnIndex("masaNetto");
      CI.masaBruto   = GetSchemaColumnIndex("masaBruto");
      CI.promjer     = GetSchemaColumnIndex("promjer  ");
      CI.povrsina    = GetSchemaColumnIndex("povrsina ");
      CI.zapremina   = GetSchemaColumnIndex("zapremina");
      CI.duljina     = GetSchemaColumnIndex("duljina");
      CI.sirina      = GetSchemaColumnIndex("sirina");
      CI.visina      = GetSchemaColumnIndex("visina");
      CI.starost     = GetSchemaColumnIndex("starost");
      CI.boja        = GetSchemaColumnIndex("boja");
      CI.spol        = GetSchemaColumnIndex("spol");

      CI.garancija    = GetSchemaColumnIndex("garancija");
      CI.dobavCD      = GetSchemaColumnIndex("dobavCD");
      CI.proizCD      = GetSchemaColumnIndex("proizCD");
      CI.isAllowMinus = GetSchemaColumnIndex("isAllowMinus");
      CI.isSerNo      = GetSchemaColumnIndex("isSerNo");
      CI.masaNettoJM  = GetSchemaColumnIndex("masaNettoJM");
      CI.masaBrutoJM  = GetSchemaColumnIndex("masaBrutoJM");
      CI.promjerJM    = GetSchemaColumnIndex("promjerJM");
      CI.povrsinaJM   = GetSchemaColumnIndex("povrsinaJM");  
      CI.zapreminaJM  = GetSchemaColumnIndex("zapreminaJM");
      CI.duljinaJM    = GetSchemaColumnIndex("duljinaJM");
      CI.sirinaJM     = GetSchemaColumnIndex("sirinaJM");
      CI.visinaJM     = GetSchemaColumnIndex("visinaJM");
      CI.velicina     = GetSchemaColumnIndex("velicina");
      CI.madeIn       = GetSchemaColumnIndex("madeIn");
      CI.url          = GetSchemaColumnIndex("url");
      CI.atestBr      = GetSchemaColumnIndex("atestBr");
      CI.atestDate    = GetSchemaColumnIndex("atestDate");
      CI.vpc1Policy   = GetSchemaColumnIndex("vpc1Policy");
      CI.isPrnOpis    = GetSchemaColumnIndex("isPrnOpis");
      CI.carTarifa    = GetSchemaColumnIndex("carTarifa");
      CI.partNo       = GetSchemaColumnIndex("partNo");
      CI.napomena     = GetSchemaColumnIndex("napomena");
      CI.importCij    = GetSchemaColumnIndex("importCij");
      CI.snaga        = GetSchemaColumnIndex("snaga");
      CI.snagaJM      = GetSchemaColumnIndex("snagaJM");
      CI.emisCO2      = GetSchemaColumnIndex("emisCO2");
      CI.euroNorma    = GetSchemaColumnIndex("euroNorma");

      lastArtiklCI = CI.euroNorma; // !!!!!! 

      CI.origRecID   = GetSchemaColumnIndex("origRecID");
      CI.recVer      = GetSchemaColumnIndex("recVer");
      CI.arAction    = GetSchemaColumnIndex("arAction");
      CI.arTS        = GetSchemaColumnIndex("arTS");
      CI.arUID       = GetSchemaColumnIndex("arUID");
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()



   #region IsThisRecordInSomeRelation

   public override bool IsThisRecordInSomeRelation(ZXC.PrivilegedAction action, XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      bool   inRtransRelation;
      int?   recCount;
      string artiklCD = (action == ZXC.PrivilegedAction.DELREC ? ((Artikl)vvDataRecord).ArtiklCD : ((Artikl)vvDataRecord).BackupedArtiklCD);

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
      // For wanted artiklCD only                                                                                                                                            

      // Rtrans ______________________________________________________________________________________________
      DataRow drSchema = ZXC.RtransDao.TheSchemaTable.Rows[ZXC.RtransDao.CI.t_artiklCD];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elArtiklCD", artiklCD, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inRtransRelation = true;
      else             inRtransRelation = false;

      if(inRtransRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));

         Issue_RecordIsInSomeRelation_Mesage(recordName, artiklCD, (int)recCount);
      }

      return (inRtransRelation);
   }

   #endregion IsThisRecordInSomeRelation

   #region FRSSET or LSTSET Artikl's Rtrans

   // This is PUSE / FUSE sheat ?! 
#if votafakisdid

   public  static Rtrans FRSSET_ArtiklsRtrans(XSqlConnection conn, string artiklCD, string skladCD, string wantedTT)
   {
      return FRSSET_LSTSET_ArtiklsRtrans(conn, artiklCD, skladCD, wantedTT, VvSQL.DBNavigActionType.FRS);
   }

   public  static Rtrans LSTSET_ArtiklsRtrans(XSqlConnection conn, string artiklCD, string skladCD, string wantedTT)
   {
      return FRSSET_LSTSET_ArtiklsRtrans(conn, artiklCD, skladCD, wantedTT, VvSQL.DBNavigActionType.LST);
   }

   private static Rtrans FRSSET_LSTSET_ArtiklsRtrans(XSqlConnection conn, string artiklCD, string skladCD, string wantedTT, VvSQL.DBNavigActionType frsORlst)
   {
      bool OK;
      Rtrans rtrans_rec = new Rtrans();

      uint                  fromTTnum = (frsORlst == VvSQL.DBNavigActionType.FRS ? 0                         : uint.MaxValue);
      VvSQL.OrderDirectEnum direction = (frsORlst == VvSQL.DBNavigActionType.FRS ? VvSQL.OrderDirectEnum.ASC : VvSQL.OrderDirectEnum.DESC);

      using(XSqlCommand cmd = VvSQL.GTEREC_Command(conn, "*", new object[] { artiklCD, skladCD, ZXC.TtInfo(wantedTT).TtSort, fromTTnum }, Set_FilterMembers_FRSSET_LSTSET_ArtiklsRtrans(artiklCD, skladCD, wantedTT), direction, Rtrans.sorter_Artikl_TTnum, 0, 1, false, false, null))
      {
         OK = ZXC.RtransDao.ExecuteSingleFillFromDataReader(rtrans_rec, false, cmd);

         if(!OK) return null;
      }

      return rtrans_rec;
   }

   private static List<VvSqlFilterMember> Set_FilterMembers_FRSSET_LSTSET_ArtiklsRtrans(string artiklCD, string skladCD, string wantedTT)
   {
      DataRow drSchema;
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      // one t_artiklCD only                                                                                                                                            
      if(artiklCD.NotEmpty())
      {
         drSchema = ZXC.RtransDao.TheSchemaTable.Rows[ZXC.RtransDao.CI.t_artiklCD];

         filterMembers.Add(new VvSqlFilterMember(drSchema, "elArtiklCD", artiklCD, " = "));
      }

      // skladCD only                                                                                                                                            
      if(skladCD.NotEmpty())
      {
         drSchema = ZXC.RtransDao.TheSchemaTable.Rows[ZXC.RtransDao.CI.t_skladCD];

         filterMembers.Add(new VvSqlFilterMember(drSchema, "elSkladCD", skladCD, " = "));
      }

      // filter for wantedTT                                                                                                                                             
      if(wantedTT.NotEmpty())
      {
         drSchema = ZXC.RtransDao.TheSchemaTable.Rows[ZXC.RtransDao.CI.t_tt];

         filterMembers.Add(new VvSqlFilterMember(drSchema, "elWantedTT", wantedTT, " = "));
      }

      return filterMembers;
   }

#endif

   #endregion FRSSET or LSTSET Artikl's Rtrans




   #region :-) CACHE - GetArtiklStatus

   #region OverLoaders

   /// <summary>
   /// DON'T USE CACHE! - For control/test purposiz!
   /// GetArtiklStatus za kompletni fajl - with supress cache capabilityes!
   /// </summary>
   /// <param name="conn"></param>
   /// <param name="_artiklCD"></param>
   /// <param name="_skladCD"></param>
   /// <param name="_dateDo"></param>
   /// <returns></returns>
   public static ArtStat GetArtiklStatus(XSqlConnection conn, string _artiklCD, string _skladCD, bool dontUseCache)
   {
      return GetAndSetArtiklStatus(/*nisam bas skroz siguran sta tu ide s obzirom na 'dontUseCache'*/dontUseCache, conn, _artiklCD, _skladCD, DateTime.MinValue, null, null, null, dontUseCache, null);
   }
   /// <summary>
   /// GetArtiklStatus za kompletni fajl
   /// </summary>
   /// <param name="conn"></param>
   /// <param name="_artiklCD"></param>
   /// <param name="_skladCD"></param>
   /// <param name="_dateDo"></param>
   /// <returns></returns>
   public static ArtStat GetArtiklStatus(XSqlConnection conn, string _artiklCD, string _skladCD)
   {
      return GetAndSetArtiklStatus(false, conn, _artiklCD, _skladCD, DateTime.MinValue, null, null, null, false, null);
   }
   /// <summary>
   /// GetArtiklStatus na neki datum
   /// </summary>
   /// <param name="conn"></param>
   /// <param name="_artiklCD"></param>
   /// <param name="_skladCD"></param>
   /// <param name="_dateDo"></param>
   /// <returns></returns>
   public static ArtStat GetArtiklStatus(XSqlConnection conn, string _artiklCD, string _skladCD, DateTime _dateDo)
   {
      return GetAndSetArtiklStatus(false, conn, _artiklCD, _skladCD, _dateDo, null, null, null, false, null);
   }
   /// <summary>
   /// GetArtiklStatus na nekom egzaktnom Rtrans-u (tj. trenutak prije, NE ukljucujuci njega samoga (serial - 1)
   /// </summary>
   /// <param name="conn"></param>
   /// <param name="rtrans_rec"></param>
   /// <returns></returns>
   public static ArtStat GetArtiklStatus(XSqlConnection conn, Rtrans rtrans_rec)
   {
      return GetAndSetArtiklStatus(false, conn, rtrans_rec.T_artiklCD, rtrans_rec.T_skladCD, rtrans_rec.T_skladDate, rtrans_rec.T_ttSort, rtrans_rec.T_ttNum, rtrans_rec.T_serial, false, null);
   }

   /// <summary>
   /// SetArtiklStatus za kompletni fajl
   /// </summary>
   /// <param name="conn"></param>
   /// <param name="_artiklCD"></param>
   /// <param name="_skladCD"></param>
   /// <returns></returns>
   public static ArtStat SetArtiklStatus(XSqlConnection conn, string _artiklCD, string _skladCD)
   {
      //22.2.2011:
      if(_artiklCD.IsEmpty()) return null; // Dakle, ne racunamo cache za stavke koje nemaju zadanu sifru artikla ('qwe' pattern) 

      return GetAndSetArtiklStatus(true /* !!! */, conn, _artiklCD, _skladCD, DateTime.MinValue, null, null, null, false, null);
   }

   #endregion OverLoaders

   private static ArtStat GetAndSetArtiklStatus(bool shouldCreateMissingCacheRecords, XSqlConnection conn, string _artiklCD, string _skladCD, DateTime _dateDo, short? _ttSort, uint? _ttNum, ushort? _serial, bool dontUseCache, List<Rtrans> rtrArtstReportList)
   {
      #region Local variables

      ArtStat cumulativeArtStat_rec = new ArtStat();
      ArtStat utilArtstat_rec       = null;
      Rtrans  rtrans_rec            = new Rtrans();

      List<Rtrans> rtransesFromMinusMoment = new List<Rtrans>();

      string orderBy_ASC  = Rtrans.artiklOrderBy_ASC;
      string orderBy_DESC = Rtrans.artiklOrderBy_DESC;

      bool isFor_All_SkladCD = _skladCD.IsEmpty();
      
      bool isFor_One_SkladCD = _skladCD.NotEmpty();

      bool isThereSomethingInCache, isInMinus = false, isThereSomethingInRtrans = false;

      bool YES_UseCache = !dontUseCache;

      bool NOT_InMinus = !isInMinus;

      bool thisIsJustRead_NonCreateCacheMode = !shouldCreateMissingCacheRecords;

      #endregion Local variables

      #region Should We Enter Recursion?!

      if(isFor_All_SkladCD)
      {
         var skladCDlist = GetDistinctSkladCdListForArtikl(conn, _artiklCD);

         foreach(string currSkladCD in skladCDlist)
         {
            SumInSintArtStat_rec(cumulativeArtStat_rec, GetAndSetArtiklStatus(shouldCreateMissingCacheRecords, conn, _artiklCD, currSkladCD, _dateDo, _ttSort, _ttNum, _serial, dontUseCache, rtrArtstReportList)); // Recursion 
         }

         return cumulativeArtStat_rec;
      }
      
      #endregion Should We Enter Recursion?!

      #region GetLast FromCache

      if(YES_UseCache)
      {
         using(XSqlCommand cmd = VvSQL.GetLastFromCache_Command(conn, _artiklCD, _skladCD, _dateDo, _ttSort, _ttNum, _serial, orderBy_DESC))
         {
            isThereSomethingInCache = ZXC.ArtStatDao.ExecuteSingleFillFromDataReader(cumulativeArtStat_rec, false, cmd);
         }
      }
      else
      {
         isThereSomethingInCache = false;
      }

      if(thisIsJustRead_NonCreateCacheMode && YES_UseCache) // We came here in good faith, counting that complete cache was previously created 
      {
         goto ENDlabel;
      }

      #endregion GetLast FromCache

      #region Rtrans Loop

      using(XSqlCommand cmd = VvSQL.GetRemainingRtransesForCalcArtiklStatus_Command(conn, (isThereSomethingInCache ? cumulativeArtStat_rec : null), RtransDao.ColumnListFromNarrowDataReader(/*_PUSE_ isForReportList,*/ ""), _artiklCD, _skladCD, _dateDo, _ttSort, _ttNum, _serial, orderBy_ASC))
      {
         using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult /*| CommandBehavior.SingleRow*/))
         {
            isThereSomethingInRtrans = reader.HasRows;

            while(reader.HasRows && reader.Read())
            {
               #region FillFromNarrowDataReader And decide is it worth something 

               ZXC.RtransDao.FillFromNarrowDataReader(rtrans_rec, reader);

               // vvDelf - 16.06.2015 - START
               // 13.04.2015 - by Delf

               // RIDnews:                                                          
               if(rtrans_rec.TtInfo.IsInternUlaz_FromOneIzlaz && rtrans_rec.T_parentID != ZXC.InAddTwinIzlazParentId) // MSU, MMU, KUL, PUK, VMU, MVU 
               {
                  decimal prNabCij_fromIzlazCache;
                  bool twinIzlazFound;

                  rtrans_rec.T_artiklCD = _artiklCD;
                  rtrans_rec.T_skladCD = _skladCD;

                  // STORY:
                  // - koristi cijenu iz Dict Tabele ako postoji
                  // - ak ne postoji, stavi u Failed Listu za kasnije
                  // edit 23.06.15 - ipak ne iz baze, ostavi kak je u rtransu

                  if(ZXC.RecIDinfoDict != null)
                  {
                     if(twinIzlazFound = ZXC.RecIDinfoDict.TryGetValue(rtrans_rec.T_recID, out prNabCij_fromIzlazCache))
                     {
                        rtrans_rec.T_cij = prNabCij_fromIzlazCache; // !!! ___ !!! 
                        if(ZXC.VerboseLOG) ZXC.aim_log("Cached Cijena for ID: {0} / {1}", rtrans_rec.T_recID, prNabCij_fromIzlazCache.ToStringVv());
                     }
                     else
                     {
                        // if(rtrans_rec.T_artiklCD == "SQ HHR")
                        if(ZXC.VerboseLOG) ZXC.aim_log("twinIzlaz NOT found - id: {0} / {1}", rtrans_rec.T_recID, rtrans_rec);
                        if(!ZXC.UseFailedIzlazTwinsListFlag || ZXC.RecIDinfoDict.Count().IsZero())
                        {
                           if(ZXC.UseFailedIzlazTwinsListFlag)
                           {
                              ZXC.FailedIzlazTwinsList.Add(rtrans_rec.MakeDeepCopy());
                              break;
                           }
                           // NO MORE - vvDelf 23.06.2015 - rtrans_rec.T_cij = GetPrNabCijFrom_One_LinkedIzlazRtrans(rtrans_rec);
                           if(ZXC.VerboseLOG) ZXC.aim_log("Cached Cijena FROM FILE for ID: {0} / {1}", rtrans_rec.T_recID, rtrans_rec.T_cij.ToStringVv());
                        }
                        else
                        {
                           if(ZXC.FailedIzlazTwinsList.Contains(rtrans_rec))
                           {
                              ZXC.FailedIzlazTwinsList.Remove(rtrans_rec);
                              ZXC.ManagedFailedIzlazTwin = null;  // This signal to Manager not to remove this rtrans
                              // NO MORE - vvDelf 23.06.2015 - rtrans_rec.T_cij = GetPrNabCijFrom_One_LinkedIzlazRtrans(rtrans_rec);
                              if(ZXC.VerboseLOG) ZXC.aim_log("twinIzlaz NOT found - TWICE or MORE - id: {0} / {1}", rtrans_rec.T_recID, rtrans_rec);
                           }
                           ZXC.FailedIzlazTwinsList.Add(rtrans_rec.MakeDeepCopy());
                           break;
                        }
                     }
                  }
                  else
                  {
                     if(ZXC.VerboseLOG) ZXC.aim_log("Doing nothing - id: {0} / {1}", rtrans_rec.T_recID, rtrans_rec);
                  }
               }
               // vvDelf - 16.06.2015 - END
               
               rtrans_rec.CalcTransResults(null);

               // Dakle, ovaj TT nije niti 'Robni UI', niti 'Skladisni UI', niti neki cijenik tj, totalni je dummy za SumRtrans() 
               if(rtrans_rec.TtInfo.IsArtiklStatusInfluencer == false) continue; 

               #endregion FillFromNarrowDataReader And decide is it worth something

               #region if(NOT_InMinus) SumFromRtrans(), else just add to 'rtransesStartingFromMinusMomentList'

               if(NOT_InMinus) // classic process 
               {
                  isInMinus = cumulativeArtStat_rec.SumFromRtrans(rtrans_rec, _artiklCD, _skladCD, false); // VOILA!                     
                  
                  NOT_InMinus = !isInMinus;

                  if(isInMinus) // prestani puniti CACHE ako se pojavi minus, te dole pokreni 'ABRAKAKOBREDABRA' manager 
                  {
                     rtransesFromMinusMoment.Add(rtrans_rec.MakeDeepCopy()); // ovaj 'incompleteRtrans_rec' je, dakle, proizveo minus 

                     // 03.09.2011: on first minus occurence, save cumulativeArtStat_rec
                     if(rtransesFromMinusMoment.Count == 1)
                     {
                        utilArtstat_rec = cumulativeArtStat_rec.MakeDeepCopy();
                     }
                  }
               } 
               else // classic process stopped, just add rtrans to list for later processing 
               {
                  rtransesFromMinusMoment.Add(rtrans_rec.MakeDeepCopy());
               }

               #endregion if(NOT_InMinus) SumRtrans(), else just add to 'rtransesStartingFromMinusMomentList'

               #region if(YES_UseCache && NOT_InMinus) ADDREC TO CACHE

               if(YES_UseCache && NOT_InMinus)
               {
                  // vvDelf - 16.06.2015 - START
                  if(rtrans_rec.TtInfo.HasTwinTT && ZXC.UseFailedIzlazTwinsListFlag)
                  {
                     rtrans_rec.T_artiklCD = _artiklCD;
                     rtrans_rec.T_skladCD = _skladCD;
                     // if(rtrans_rec.T_artiklCD == "SQ HHR")
                     if(ZXC.VerboseLOG) ZXC.aim_log("twinUlaz Ab SET: id: {0} / {1}", rtrans_rec.T_twinID, rtrans_rec);
                     ZXC.RecIDinfoDict[rtrans_rec.T_twinID] = cumulativeArtStat_rec.PrNabCij; // vidi malo, Delowsky, kak je ovo slick                         
                  }
                  // vvDelf - 16.06.2015 - END

                  ZXC.UseThirdDbConnection(
                     () => ZXC.TheThirdDbConn_SameDB,
                     thirdDbConn => cumulativeArtStat_rec.VvDao.ADDREC(thirdDbConn, cumulativeArtStat_rec, false, false, false, false));
               }

               #endregion if(YES_UseCache && NOT_InMinus) Add it to List
            }

            reader.Close();

         } // using(XSqlDataReader reader 

      } // using(XSqlCommand cmd 

      #endregion Rtrans LOOP

      #region TIME MACHINE - ABRAKAKOBREDABRA  manager

      if(isInMinus == true)
      {
         #region Try To Repare Troublemakers

         List<Rtrans> troubleMakers = new List<Rtrans>();

         for(int ri = 0; ri < rtransesFromMinusMoment.Count; ++ri)
         {
            rtransesFromMinusMoment[ri].TmpDecimal = 0.0M;

            if((rtransesFromMinusMoment[ri].ThisRtransAddrecSmanjujeStanje && troubleMakers.Count.NotZero()) || 
               cumulativeArtStat_rec.SumFromRtrans(rtransesFromMinusMoment[ri], _artiklCD, _skladCD, true))
            {
               rtransesFromMinusMoment[ri].MinusStatus = ZXC.MinusTrouble.IN_TROUBLE;

               troubleMakers.Add(rtransesFromMinusMoment[ri]);
            }
            else
            {
               for(int ti = 0; ti < troubleMakers.Count && cumulativeArtStat_rec.SumFromRtrans(troubleMakers[ti], _artiklCD, _skladCD, true) == false; ++ti)
               {
                  rtransesFromMinusMoment.Single(rtr => rtr.T_recID == troubleMakers[ti].T_recID).TmpDecimal  = cumulativeArtStat_rec.PrNabCij; // 03.09.2011: Save ABRAKAKOBREDABRA calculated PrNabCij for later use 
                // 01.02.2012" 
                //rtransesFromMinusMoment.Single(rtr => rtr.T_recID == troubleMakers[ti].T_recID).TmpDecimal2 = cumulativeArtStat_rec.MalopCij;
                  rtransesFromMinusMoment.Single(rtr => rtr.T_recID == troubleMakers[ti].T_recID).TmpDecimal2 = cumulativeArtStat_rec.RtrCijenaMPC; 
                  rtransesFromMinusMoment.Single(rtr => rtr.T_recID == troubleMakers[ti].T_recID).MinusStatus = ZXC.MinusTrouble.REPAIRED;

                  troubleMakers.Remove(troubleMakers[ti--]);
               }
            }
         } // foreach 

         #endregion Try To Repare Troublemakers

         #region Finally, ADDREC To Cache for all rtranses in 'rtransesStartingFromMinusMomentList'

         if(YES_UseCache)
         {
            cumulativeArtStat_rec = utilArtstat_rec.MakeDeepCopy(); // go back in cumulation chronology 

            foreach(Rtrans rtrans in rtransesFromMinusMoment)
            {
               cumulativeArtStat_rec.SumFromRtrans(rtrans, _artiklCD, _skladCD, true, true); // VOILA!                     

               // vvDelf - 16.06.2015 - START
               if(rtrans.TtInfo.HasTwinTT && ZXC.UseFailedIzlazTwinsListFlag)
               {
                  rtrans.T_artiklCD = _artiklCD;
                  rtrans.T_skladCD = _skladCD;
                  // if(rtrans_rec.T_artiklCD == "SQ HHR")
                  if(ZXC.VerboseLOG) ZXC.aim_log("twinUlaz A- SET: id: {0} / {1}", rtrans.T_twinID, rtrans);
                  if (rtrans.TmpDecimal.NotZero())
                     ZXC.RecIDinfoDict[rtrans.T_twinID] = rtrans.TmpDecimal; // vidi malo, Delowsky, kak je ovo slick                         
                  else
                     ZXC.RecIDinfoDict[rtrans.T_twinID] = cumulativeArtStat_rec.PrNabCij; // vidi malo, Delowsky, kak je ovo slick                         
               }
               // vvDelf - 16.06.2015 - END
                ZXC.UseThirdDbConnection(
                   () => ZXC.TheThirdDbConn_SameDB,
                   thirdDbConn => cumulativeArtStat_rec.VvDao.ADDREC(thirdDbConn, cumulativeArtStat_rec, false, false, false, false));
            }
         }

         #endregion Finally, ADDREC To Cache for all rtranses in 'rtransesStartingFromMinusMomentList'

      } // if(isInMinus == true) 

      #endregion TIME MACHINE - ABRAKAKOBREDABRA  manager

      ENDlabel:

      if(!isThereSomethingInCache && !isThereSomethingInRtrans) return null;
      else                                                      return cumulativeArtStat_rec;

   } // GetArtiklStatus END 

   #region if(isFor_All_SkladCD) Utils

   public static List<string> GetDistinctSkladCdListForArtikl(XSqlConnection conn, string _artiklCD)
   {
      return GetDistinctSkladCdListForArtikl(conn, _artiklCD, false);
   }
   public static List<string> GetDistinctSkladCdListForArtikl(XSqlConnection conn, string _artiklCD, bool isRootCD)
   {
      string skladCD;
      List<string> skladCDlist = new List<string>();

      // Ovo si mogao i efikasnije, tako da postavis kao eventualni uvjet jos i '_dateDo, _ttSort, _ttNum, _serial' ali neka...
      using(XSqlCommand cmd = VvSQL.GetDistinctSkladCdForArtikl_Command(conn, _artiklCD, isRootCD))
      {
         using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult /*| CommandBehavior.SingleRow*/))
         {
            while(reader.HasRows && reader.Read())
            {
               skladCD = reader.GetString(0);
               
               if(skladCD.NotEmpty())
               {
                  skladCDlist.Add(skladCD);
               }
            }
            reader.Close();
         }
      }

      return skladCDlist;
   }

   private static void SumInSintArtStat_rec(ArtStat SINT_rec, ArtStat curr_rec)
   {
      if(curr_rec == null) return;

      #region Classic sums

      // !!! PAZI !!! za Fisycal je ovo krivo! Da bi i Fisycal radilo na ovaj nacin trebao bi dodati u DataLayer UkUlazFirmaKolFisycal, UkIzlazFirmaKolFisycal, ... a ne da ti se... 
      SINT_rec.UkPstKol          += curr_rec.UkPstKol         ;
      SINT_rec.UkPstFinNBC       += curr_rec.UkPstFinNBC      ;
      SINT_rec.UkPstFinMPC       += curr_rec.UkPstFinMPC      ;
      SINT_rec.InvKol            += curr_rec.InvKol           ;
      SINT_rec.InvFinNBC            += curr_rec.InvFinNBC           ;
      SINT_rec.UkUlazKolFisycal  += curr_rec.UkUlazKolFisycal ;
    //SINT_rec.UkUlazKol         += curr_rec.UkUlazKol        ;
      SINT_rec.UkUlazKol         += curr_rec.UkUlazFirmaKol   ;
    //SINT_rec.UkUlazFinNBC      += curr_rec.UkUlazFinNBC     ;
      SINT_rec.UkUlazFinNBC      += curr_rec.UkUlazFirmaFinNBC;
      SINT_rec.UkUlazFinMPC      += curr_rec.UkUlazFirmaFinMPC;
      SINT_rec.UkIzlazKolFisycal += curr_rec.UkIzlazKolFisycal;
    //SINT_rec.UkIzlazKol        += curr_rec.UkIzlazKol       ;
      SINT_rec.UkIzlazKol        += curr_rec.UkIzlazFirmaKol  ;
      SINT_rec.UkStanjeKolRezerv += curr_rec.UkStanjeKolRezerv;
    //SINT_rec.UkIzlazFinNBC     += curr_rec.UkIzlazFinNBC    ;
      SINT_rec.UkIzlazFinNBC     += curr_rec.UkIzlazFirmaFinNBC;
      SINT_rec.UkIzlazFinMPC     += curr_rec.UkIzlazFirmaFinMPC;
      SINT_rec.UkIzlFinProdKCR   += curr_rec.UkIzlFinProdKCR  ;

      #endregion classic sums

      #region PreDefs ... in fact, nonsens information

      if(curr_rec.PreDefVpc1  .NotZero()) SINT_rec.PreDefVpc1      /*+*/= curr_rec.PreDefVpc1;
      if(curr_rec.PreDefVpc2  .NotZero()) SINT_rec.PreDefVpc2      /*+*/= curr_rec.PreDefVpc2;
      if(curr_rec.PreDefMpc1  .NotZero()) SINT_rec.PreDefMpc1      /*+*/= curr_rec.PreDefMpc1;
      if(curr_rec.PreDefDevc  .NotZero()) SINT_rec.PreDefDevc      /*+*/= curr_rec.PreDefDevc;
      if(curr_rec.PreDefRbt1  .NotZero()) SINT_rec.PreDefRbt1      /*+*/= curr_rec.PreDefRbt1;
      if(curr_rec.PreDefRbt2  .NotZero()) SINT_rec.PreDefRbt2      /*+*/= curr_rec.PreDefRbt2;
      if(curr_rec.PreDefMinKol.NotZero()) SINT_rec.PreDefMinKol    /*+*/= curr_rec.PreDefMinKol;
      if(curr_rec.PreDefMarza .NotZero()) SINT_rec.PreDefMarza     /*+*/= curr_rec.PreDefMarza;

      #endregion PreDefs ... in fact, nonsens information

      #region Ulaz/Izlaz Min/Max Cijena

      if(curr_rec.UlazCijMin.NotZero() && (curr_rec.UlazCijMin < SINT_rec.UlazCijMin || SINT_rec.UlazCijMin.IsZero()))
      {
         SINT_rec.UlazCijMin = curr_rec.UlazCijMin;
      }
      if(curr_rec.UlazCijMax.NotZero() && (curr_rec.UlazCijMax > SINT_rec.UlazCijMax || SINT_rec.UlazCijMax.IsZero()))
      {
         SINT_rec.UlazCijMax = curr_rec.UlazCijMax;
      }

      if(curr_rec.IzlazCijMin.NotZero() && (curr_rec.IzlazCijMin < SINT_rec.IzlazCijMin || SINT_rec.IzlazCijMin.IsZero()))
      {
         SINT_rec.IzlazCijMin = curr_rec.IzlazCijMin;
      }
      if(curr_rec.IzlazCijMax.NotZero() && (curr_rec.IzlazCijMax > SINT_rec.IzlazCijMax || SINT_rec.IzlazCijMax.IsZero()))
      {
         SINT_rec.IzlazCijMax = curr_rec.IzlazCijMax;
      }

      #endregion Ulaz/Izlaz Min/Max Cijena

      #region Ulaz/Izlaz Zadnji Date/Cijena

      if(SINT_rec.DateZadPst == DateTime.MinValue ||
         SINT_rec.DateZadPst < curr_rec.DateZadPst)
      {
         SINT_rec.DateZadPst = curr_rec.DateZadPst;
      }

      if(SINT_rec.DateZadInv == DateTime.MinValue ||
         SINT_rec.DateZadInv < curr_rec.DateZadInv)
      {
         SINT_rec.DateZadInv = curr_rec.DateZadInv;
      }

      if(SINT_rec.DateZadUlaz == DateTime.MinValue ||
         SINT_rec.DateZadUlaz < curr_rec.DateZadUlaz)
      {
         SINT_rec.DateZadUlaz = curr_rec.DateZadUlaz;
         SINT_rec.UlazCijLast = curr_rec.UlazCijLast;
      }

      if(SINT_rec.DateZadIzlaz == DateTime.MinValue ||
         SINT_rec.DateZadIzlaz < curr_rec.DateZadIzlaz)
      {
         SINT_rec.DateZadIzlaz = curr_rec.DateZadIzlaz;
         SINT_rec.IzlazCijLast = curr_rec.IzlazCijLast;
      }

      #endregion Ulaz/Izlaz Zadnji Date/Cijena

   }
   
   #endregion isFor_All_SkladCD utils

   #region GetLinkedIzlazDokPrNabCij

   // 24.10.2014: otkriveno da je ova metoda APSOLUTNO NEPOTREBNA - PRIVREMENO SE KORISTI
   // TO DO - ODLUKA DA LI JE IPAK ELIMINIRATI

   public static decimal GetPrNabCijFrom_One_LinkedIzlazRtrans(Rtrans ulazRtrans_rec, out Rtrans izlaz_rec, out bool foundArtstat)
   {
      izlaz_rec = null;
      foundArtstat = false;

      Rtrans izlazRtrans_rec = new Rtrans();

      bool localFoundArtstat = false;

      decimal prNabCij = ZXC.UseSecondDbConnection(
         () => ZXC.TheSecondDbConn_SameDB,
         conn =>
         {
            if(!ZXC.RtransDao.SetMe_Record_bySomeUniqueColumn(conn, izlazRtrans_rec, ulazRtrans_rec.T_recID, ZXC.RtransSchemaRows[ZXC.RtrCI.t_twinID], false, false))
            {
               if(ZXC.VerboseLOG) ZXC.aim_log("ulazRtrans_rec Can't find izlazRtrans_rec: {0}", ulazRtrans_rec.T_recID);

               return 0.00M;
            }

            ArtStat artStat_rec = ArtiklDao.GetArtiklStatus(conn, izlazRtrans_rec);

            if(ZXC.VerboseLOG) ZXC.aim_log("ulazRtrans_rec GetFromDisk: id: {0} twin id: {1} / {2}", izlazRtrans_rec.T_recID, izlazRtrans_rec.T_twinID, izlazRtrans_rec);

            if(artStat_rec == null)
            {
               artStat_rec = RepareMissingArtstat(conn, izlazRtrans_rec);

               if(artStat_rec == null) return 0.00M;
            }

            if(ZXC.VerboseLOG) ZXC.aim_log("artStat_rec Found On Disk: {0}", artStat_rec);

            localFoundArtstat = true;

            return artStat_rec.PrNabCij;
         });

      if(localFoundArtstat)
      {
         izlaz_rec = izlazRtrans_rec;
         foundArtstat = true;
      }

      return prNabCij;
   }

   // 24.10.2014: otkriveno da je ova metoda APSOLUTNO NEPOTREBNA 
   //public static decimal GetPrNabCijFrom_Many_LinkedIzlazRtranses(Rtrans ulazRtrans_rec)
   //{
   //   XSqlConnection conn = ZXC.TheSecondDbConn_SameDB;
      
   //   List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
   //   filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_parentID], "elParentID", ulazRtrans_rec.T_parentID, " = "));

   //   List<Rtrans> rtransList = new List<Rtrans>();

   //   RtransDao.GetRtransWithArtstatList(conn, rtransList, "", filterMembers, "R.t_serial");

   //   string ulazTT  = ulazRtrans_rec.T_TT;
   //   string izlazTT = ZXC.TtInfoArray.Single(tti => tti.SplitTT == ulazTT).TheTT;

   //   // 24.10.2014: 
   // //var izlazRtranses = rtransList.Where(rtr => rtr.T_TT == izlazTT                                                                                         );
   // //var ulazRtranses  = rtransList.Where(rtr => rtr.T_TT == ulazTT                                                                                          );
   //   if(/*ZXC.RRD.Dsc_IsProizvCijByArtGr*/ Faktur.IsProizvCijByArtGr(izlazTT)) RepareMissingArtstat(conn, ulazRtrans_rec);
   //   var izlazRtranses = rtransList.Where(rtr => rtr.T_TT == izlazTT && (Faktur.IsProizvCijByArtGr(izlazTT) && ulazRtrans_rec.TheAsEx != null ? ulazRtrans_rec.A_ArtGrCd1 == rtr.A_ArtGrCd1 : true));
   //   var ulazRtranses  = rtransList.Where(rtr => rtr.T_TT == ulazTT  && (Faktur.IsProizvCijByArtGr(izlazTT) && ulazRtrans_rec.TheAsEx != null ? ulazRtrans_rec.A_ArtGrCd1 == rtr.A_ArtGrCd1 : true));

   //   #region Bad(Missing)Cache Manager

   //   int badCacheCount = izlazRtranses.Count(rtr => rtr.TheAsEx.IsNullFromReader);

   //   if(badCacheCount.IsPositive())
   //   {
   //      var handicapedIzlazRtranses = izlazRtranses.Where(rtr => rtr.TheAsEx.IsNullFromReader);

   //      foreach(Rtrans rtrans_rec in handicapedIzlazRtranses)
   //      {
   //         RepareMissingArtstat(conn, rtrans_rec);
   //      }

   //   }

   //   #endregion Bad(Missing)Cache Manager

   //   decimal ukFinIzlaz = izlazRtranses.Sum(rtr => rtr.R_Kol_Puta_PrNabCij);
   //   decimal ukKolUlaz  = ulazRtranses .Sum(rtr => rtr.T_kol              );
   //   decimal ncPerUlKol = ZXC.DivSafe(ukFinIzlaz, ukKolUlaz);

   //   // 28.04.2013: 
   // //return ncPerUlKol;
   //   return ncPerUlKol.Ron(4);
   //}

   /*private*/ public static ArtStat RepareMissingArtstat(XSqlConnection conn, Rtrans rtrans_rec)
   {
      ArtStat artstat_rec = ArtiklDao.SetArtiklStatus(conn, rtrans_rec.T_artiklCD, rtrans_rec.T_skladCD);

      rtrans_rec.TheAsEx = artstat_rec;

      return artstat_rec;
   }

   #endregion GetLinkedIzlazDokPrNabCij

   #region ArtStatColumnsForArtiklGterecCommand

   internal static string ArtStatColumnsForArtiklGterecCommand(ArtiklListUC artiklListUC)
   {
      // 22.12.2012: 
      bool isMalopSklad = ZXC.luiListaSkladista.GetFlagForThisCd(artiklListUC.Fld_SituacijaZaSkladCD);

      if(artiklListUC.Fld_IsShowSomeOfStatusData == false && 
         artiklListUC.TheFilterMembers.Count(m => m.relatedTable == ArtStat.recordName).IsZero()) return "";

      return "preDefVpc1     as ext_preDefVpc1, preDefMpc1 as ext_preDefMpc1, " + "  stanjeKolRezerv as ext_Kolreserve, " + "\n" +
             ResCol_StanjeKol                               + " as ext_kolSt,      \n" +
             ResCol_StanjKolFisycal                         + " as ext_kolFisycal, \n" +
             ResCol_StanjeKolFree                           + " as ext_kolFree,    \n" + 
             (isMalopSklad ? ResCol_RucMAL : ResCol_RucVEL) + " as ext_RucVpc1     \n" +
             ", lastMalopCij as ext_lastMalopCij \n" +
             ", lastPrNabCij as ext_lastPrNabCij";
   }

   public static string NoNeedForZaliha_ArtiklTSs_IN_clause { get { return TtInfo.GetSql_IN_Clause(ZXC.NoNeedForZaliha_ArtiklTSs); } }

   public static string ResCol_StanjeKol        { get { return "IF(artiklTS IN" + NoNeedForZaliha_ArtiklTSs_IN_clause + ", 0, (pstKol+ulazKol-izlazKol))"                                  ; } }
   public static string ResCol_StanjKolFisycal  { get { return "IF(artiklTS IN" + NoNeedForZaliha_ArtiklTSs_IN_clause + ", 0, (pstKol+ulazKolFisycal-izlazKolFisycal))"                    ; } }
   public static string ResCol_StanjeKolFree    { get { return "IF(artiklTS IN" + NoNeedForZaliha_ArtiklTSs_IN_clause + ", 0, (pstKol+ulazKolFisycal-izlazKolFisycal-stanjeKolRezerv))"    ; } }
   public static string ResCol_StanjeFinNBC     { get { return "IF(artiklTS IN" + NoNeedForZaliha_ArtiklTSs_IN_clause + ", 0, (pstFinNBC + ulazFinNBC - izlazFinNBC))"                     ; } }
   public static string ResCol_StanjeFinMPC     { get { return "IF(artiklTS IN" + NoNeedForZaliha_ArtiklTSs_IN_clause + ", 0, (pstFinMPC + ulazFinMPC - izlazFinMPC))"                     ; } }
   public static string ResCol_PrNabCij         { get { return "(" + ResCol_StanjeFinNBC +"/" + ResCol_StanjeKol + ")"                                               ; } } // i tu bi trebao lastPrNabCij ako nema stanja...
   public static string ResCol_RucVEL           { get { return "IF(preDefVpc1=0,   0, ((preDefVpc1-"   + ResCol_PrNabCij + ")/" + ResCol_PrNabCij + "*100))"             ; } }
   public static string ResCol_IzlazKol         { get { return "IF(artiklTS IN" + NoNeedForZaliha_ArtiklTSs_IN_clause + ", 0, (izlazKol))"                                  ; } }

   public static string ResCol_RucMAL           { get { return "IF(lastMalopCij=0, 0, (((lastMalopCij / 1.25) - lastPrNabCij) / lastPrNabCij * 100))"             ; } }

   public static string ResCol_UlazFinNBC          { get { return "IF(artiklTS IN" + NoNeedForZaliha_ArtiklTSs_IN_clause + ", 0, (rtrPstVrjNBC + rtrUlazVrjNBC) + ((pstKol+ulazKol-izlazKol)-(rtrPstKol + rtrUlazKol-rtrIzlazKol))*(rtrCijenaMPC-prevMalopCij))"      ; } }
   public static string ResCol_IzlazFinNBC         { get { return "IF(artiklTS IN" + NoNeedForZaliha_ArtiklTSs_IN_clause + ", 0, (rtrIzlazVrjNBC))"                    ; } }
   public static string ResCol_UlazFinMPC          { get { return "IF(artiklTS IN" + NoNeedForZaliha_ArtiklTSs_IN_clause + ", 0, (rtrPstVrjMPC + rtrUlazVrjMPC))"      ; } }
   public static string ResCol_IzlazFinMPC         { get { return "IF(artiklTS IN" + NoNeedForZaliha_ArtiklTSs_IN_clause + ", 0, (rtrIzlazVrjMPC)-(rtrIzlazKol)*(rtrCijenaMPC-prevMalopCij))"; } }

   // some Rtrans result columns
   public static string Rtr_Ulaz_Kol      { get { return TtInfo.GetSql_IN_Clause(ZXC.TtUlazKolArray     ); } }
   public static string Rtr_Izlaz_Kol     { get { return TtInfo.GetSql_IN_Clause(ZXC.TtIzlazKolArray    ); } }
   public static string Rtr_UlazIzlaz_Kol { get { return TtInfo.GetSql_IN_Clause(ZXC.TtUlazIzlazKolArray); } }

   #endregion ArtStatColumnsForArtiklGterecCommand

   #region Check Cache

   public static uint CountMissingArtstat(XSqlConnection conn)
   {
      object obj;
      uint   recCount;

      using(XSqlCommand cmd = VvSQL.CountMissingArtstat_Command(conn, TtInfo.GetSql_IN_Clause(ZXC.TtInfoArtstatInfluencers)))
      {
         try              { obj = cmd.ExecuteScalar(); }
         catch(Exception) { return(0); }

         try              { recCount = uint.Parse(obj.ToString()); }
         catch(Exception) { return(0); }
      }

      return (recCount);
   }

   public static uint Count_BadMSU_ArtstatNabCij(XSqlConnection conn)
   {
      object obj;
      uint   recCount;

      using(XSqlCommand cmd = VvSQL.Count_BadMSU_ArtstatNabCij_Command(conn, TtInfo.GetSql_IN_Clause(new string[] { "MSU", "VMU", "MVU", "MMU", "KUL", "PUK" })))
      {
         try              { obj = cmd.ExecuteScalar(); }
         catch(Exception) { return(0); }

         try              { recCount = uint.Parse(obj.ToString()); }
         catch(Exception) { return(0); }
      }

      return (recCount);
   }

   public static List<ZXC.CdAndName_CommonStruct> DistinctListOfMissingArtstat(XSqlConnection conn)
   {
      bool success = true;
      List<ZXC.CdAndName_CommonStruct> theList = new List<ZXC.CdAndName_CommonStruct>();

      using(XSqlCommand cmd = VvSQL.DistinctListOfMissingArtstat_Command(conn, TtInfo.GetSql_IN_Clause(ZXC.TtInfoArtstatInfluencers)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult /*| CommandBehavior.SingleRow*/))
            {
               success = reader.HasRows;

               while(success && reader.Read())
               {
                  theList.Add(new ZXC.CdAndName_CommonStruct(reader.GetString(0), reader.GetString(1)));
               }

               reader.Close();
            } // using reader 
         }
         catch(XSqlException ex) { success = false; VvSQL.ReportSqlError("DistinctListOfMissingArtstat", ex, System.Windows.Forms.MessageBoxButtons.OK); ZXC.sqlErrNo = ex.Number; }
      }

      return (theList.OrderBy(mis => mis.TheCd).ToList());
   }

 //public static List<ZXC.CdAndName_CommonStruct> DistinctListOf_BadMSU_ArtstatNabCij(XSqlConnection conn)
   public static List<Rtrans>                     DistinctListOf_BadMSU_ArtstatNabCij(XSqlConnection conn)
   {
      bool success = true;
      List<ZXC.CdAndName_CommonStruct> theList    = new List<ZXC.CdAndName_CommonStruct>();
      List<Rtrans>                     rtransList = new List<Rtrans>();
      Rtrans rtrans_rec;

      using(XSqlCommand cmd = VvSQL.DistinctListOfBadMSU_ArtstatNabCij_Command(conn, TtInfo.GetSql_IN_Clause(new string[] { "MSU", "VMU", "MVU", "MMU", "KUL", "PUK" })))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult /*| CommandBehavior.SingleRow*/))
            {
               success = reader.HasRows;

               while(success && reader.Read())
               {
                //theList.Add(new ZXC.CdAndName_CommonStruct(reader.GetString(0), reader.GetString(1)));
                  rtrans_rec = new Rtrans();
                  rtrans_rec.VvDao.FillFromDataReader(rtrans_rec, reader, false);
                  rtransList.Add(rtrans_rec);
               }

               reader.Close();
            } // using reader 
         }
         catch(XSqlException ex) { success = false; VvSQL.ReportSqlError("DistinctListOfBadMSU_ArtstatNabCij", ex, System.Windows.Forms.MessageBoxButtons.OK); ZXC.sqlErrNo = ex.Number; }
      }

    //return (theList.OrderBy(mis => mis.TheCd).ToList());
      return rtransList;
   }

   public static bool CheckCache(XSqlConnection conn)
   {
      return CheckCache(conn, false);
   }

   public static bool CheckCache(XSqlConnection conn, bool beSilent)
   {
      // 30.076.2015: 
      if(ZXC.IsTEXTHOsky) return true;

      bool allOK = true;

      ZXC.SetStatusText("CheckCache");

    //uint missing_or_badMSU_Count = CountMissingArtstat(conn);
      uint missing_Count           = CountMissingArtstat(conn);

    //// 21.12.2016: !!! BIG BIG NEWS !!!  ... kasnije ipak tu ugasio 
    //missing_or_badMSU_Count += Count_BadMSU_ArtstatNabCij(conn);    

      if(missing_Count.IsZero()) return allOK;

      allOK = false;

      ZXC.ErrorsList = null;

      if(beSilent != true)
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Cache je potrebno regenerirati ({0} stavaka). Pričekajte, molim, obradu.", missing_Count);

         // 25.05.2017: TH Shop debug purposes: prijavi na mail, jer ovdje, valjda, ne bi nikada trebalo doci sa beSilent == false 
         if(ZXC.IsTEXTHOshop)
         {
            // 18.09. ipak ugasio

            // // 17.09.2017: TEXTHOshopu neka sada napravi cio novi cache .......................................... start 
            // /* */ ZXC.SENDtoSKY_InProgress      = 
            // /* */ ZXC.RECEIVEfromSKY_InProgress = false;
            // /* */ string recordName = ArtStat.recordName; bool ncOK;
            // /* */ string dbName = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects.DataBaseName;
            // /* */ ncOK = VvSQL.DROP_TABLE(conn, dbName, recordName);
            // /* */ //if(!ncOK) return;
            // /* */ VvSQL.CheckTableVersion_AndCatchUpIfNeeded(conn, dbName, recordName);
            // // 17.09.2017: TEXTHOshopu neka sada napravi cio novi cache ...........................................  end  

            string errMsg = "CheckCache entered, NOT silent."  + 
                            "\nSENDtoSKY_InProgress is: "      + ZXC.SENDtoSKY_InProgress +
                            "\nRECEIVEfromSKY_InProgress is: " + ZXC.RECEIVEfromSKY_InProgress /*+ 
                            "\nRENEW CACHE is: "               + ncOK*/;

            VvMailClient mailClient = new VvMailClient();
            mailClient.SendMail_SUPPORT(false, "Cache! w auto rebuild", errMsg, ZXC.VvDeploymentSite + "_" + ZXC.vvDB_ServerID.ToString());

            ZXC.aim_log(errMsg);
            

            // 21.08.2019: ako neki fale, neka rebuilda sve. Anti minus pokusaj ... START ... 
            ZXC.TheVvForm.RISK_NewCache("SKY", EventArgs.Empty);
            return true;
            // 21.08.2019: ako neki fale, neka rebuilda sve. Anti minus pokusaj ...  END  ... 


         } // if(ZXC.IsTEXTHOshop)
      }

      ZXC.RenewCache_InProgress = true;

      Cursor.Current = Cursors.WaitCursor;

      // --- Here we go! --- 

      // CdAndName_CommonStruct ti je utiliti struct-ura opce namjene u ZXC-u 
      //03.06.2015: 
    //List<ZXC.CdAndName_CommonStruct> theMissersList = DistinctListOfMissingArtstat(conn);
      List<ZXC.CdAndName_CommonStruct> theMissersList = DistinctListOfMissingArtstat(conn).OrderBy(can => can.TheCd).ThenBy(can => can.TheName).ToList();
      List<ZXC.CdAndName_CommonStruct> theMissersListEx = null;

    //// 21.12.2016: !!! BIG BIG NEWS !!! ... kasnije ipak tu ugasio  
    //AddTo_DistinctListOf_BadMSU_ArtstatNabCij(conn, theMissersList);

      Rtrans rtrans_rec = new Rtrans();

      int debugCount = 0, currIntPerc = 0;
      decimal currDecimaPerc;
      string statusText;

      foreach(var misser in theMissersList)
      {
         rtrans_rec.T_artiklCD = misser.TheCd;
         rtrans_rec.T_skladCD  = misser.TheName;

         // --- VOILA!!! ------------------------- 

         try
         {
            ZXC.RtransDao.Delete_Then_Renew_Cache_FromThisRtrans(conn, rtrans_rec, VvSQL.DB_RW_ActionType.DEL);
         }
         catch(MySqlException mysqlEx)
         {
            if(mysqlEx.Message.Contains("There is already an open DataReader"))
            {
               ZXC.aim_log("CheckCache problem:\n\n" + ZXC.VvExceptionDetails(mysqlEx));
               if(theMissersListEx == null) theMissersListEx = new List<ZXC.CdAndName_CommonStruct>();

               theMissersListEx.Add(misser);
            }
            else ZXC.aim_emsg(MessageBoxIcon.Error, mysqlEx.Message);
         }
         catch(Exception ex)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "CheckCache problem:\n\n" + ZXC.VvExceptionDetails(ex));
         }

         ++debugCount;
         currDecimaPerc = ZXC.DivSafe(debugCount, theMissersList.Count) * 100M;
         if(currIntPerc != (int)Math.Floor(currDecimaPerc) || currIntPerc.IsZero())
         {
            currIntPerc = (int)Math.Floor(currDecimaPerc);
            statusText = String.Format("Done {0} / {1} = {2}%. [{3}-{4}]", debugCount, theMissersList.Count, currIntPerc, misser.TheCd, misser.TheName);
            if(ZXC.ThisIsSkyLabProject == false) { ZXC.SetStatusText(statusText); if(Cursor.Current != Cursors.WaitCursor) Cursor.Current = Cursors.WaitCursor; }
            else                                   ZXC.aim_log(statusText);
         }
      }

      // 3.2.2011: Ok, ne kuzim kako ali ovo rijesava problem cilklickog GetPrNabCij za LinkedIzlaz...
      // nije mi jasno kako je dovoljno samo jos jedamput ponoviti za neuspjele pa da bude sve dobro!?
      // well, for now, shuty-kenyaj-brzinemenjaj 

      if(theMissersListEx != null && theMissersListEx.Count.IsPositive())
      {
         foreach(var misser in theMissersListEx)
         {
            rtrans_rec.T_artiklCD = misser.TheCd;
            rtrans_rec.T_skladCD = misser.TheName;

            // --- VOILA!!! Part 2 ------------------------- 

            ZXC.RtransDao.Delete_Then_Renew_Cache_FromThisRtrans(conn, rtrans_rec, VvSQL.DB_RW_ActionType.DEL);
         }
      }

      // vvDelf - 16.06.2015 - START
      
      ZXC.RtransDao.FailedIzlazTwinsListManager(conn, VvSQL.DB_RW_ActionType.DEL);  // true=reset, by Delf 01.06.2015

      ZXC.RtransDao.ResetRecIDinfoList();
      // vvDelf - 16.06.2015 - end

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg(MessageBoxIcon.Information, "Regeneriranje je gotovo.");

      ZXC.RenewCache_InProgress = false;

      #region ErrorList Report (Missing ArtiklCDs)

      // 04.10.2011:
      if(ZXC.ErrorsList != null && ZXC.ErrorsList.Count().NotZero())
      {
         List<string> distinctSortedList = ZXC.ErrorsList.Distinct().OrderBy(err => err).ToList();

         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvCheckCache_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, System.Text.Encoding.GetEncoding(1250)))
         {
            foreach(string error in distinctSortedList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nNedostaje {2} artikala. ", ZXC.ErrorsList.Count(), fName, distinctSortedList.Count());
      }

      #endregion ErrorList Report

      ZXC.ClearStatusText();

      return allOK;
   }

   public static bool Check_MSU_Cache(XSqlConnection conn, bool beSilent)
   {
      // 30.076.2015: 
      if(ZXC.IsTEXTHOsky) return true;

      bool allOK = true;

      ZXC.SetStatusText("Check_MSU_Cache");

      uint badMSU_Count = Count_BadMSU_ArtstatNabCij(conn);

      if(badMSU_Count.IsZero()) return allOK;

      allOK = false;

      ZXC.ErrorsList = null;

      if(beSilent != true)
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "MSU Cache je potrebno regenerirati ({0} stavaka). Pričekajte, molim, obradu.", badMSU_Count);
      }

      ZXC.RenewCache_InProgress = true;

      Cursor.Current = Cursors.WaitCursor;

      // --- Here we go! --- 

    //List<ZXC.CdAndName_CommonStruct> theMissersList = DistinctListOf_BadMSU_ArtstatNabCij(conn).OrderBy(can => can.TheCd).ThenBy(can => can.TheName).ToList();
      List<Rtrans>                     theMissersList = DistinctListOf_BadMSU_ArtstatNabCij(conn);//.OrderBy(can => can.TheCd).ThenBy(can => can.TheName).ToList();

    //Rtrans rtrans_rec = new Rtrans();

      int debugCount = 0, currIntPerc = 0;
      decimal currDecimaPerc;
      string statusText;

    //foreach(var badMSU_artiklOnSklad in theMissersList)
      foreach(Rtrans badMSU_rtrans in theMissersList)
      {
       //rtrans_rec.T_artiklCD = badMSU_rtrans.TheCd;
       //rtrans_rec.T_skladCD  = badMSU_rtrans.TheName;

         // --- VOILA!!! ------------------------- 

         try
         {
          //ZXC.RtransDao.Delete_Then_Renew_Cache_FromThisRtrans(conn, badMSU_rtrans, VvSQL.DB_RW_ActionType.DEL);
            ZXC.RtransDao.Delete_Then_Renew_Cache_FromThisRtrans_JOB(conn, badMSU_rtrans, VvSQL.DB_RW_ActionType.UTIL, 0);
         }
         catch(MySqlException mysqlEx)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Check_MSU_Cache problem:\n\n" + ZXC.VvExceptionDetails(mysqlEx));
         }
         catch(Exception ex)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Check_MSU_Cache problem:\n\n" + ZXC.VvExceptionDetails(ex));
         }

         ++debugCount;
         currDecimaPerc = ZXC.DivSafe(debugCount, theMissersList.Count) * 100M;
         if(currIntPerc != (int)Math.Floor(currDecimaPerc) || currIntPerc.IsZero())
         {
            currIntPerc = (int)Math.Floor(currDecimaPerc);
            statusText = String.Format("MSU Done {0} / {1} = {2}%. [{3}-{4}]", debugCount, theMissersList.Count, currIntPerc, badMSU_rtrans.T_artiklCD, badMSU_rtrans.T_skladCD);
            if(ZXC.ThisIsSkyLabProject == false) { ZXC.SetStatusText(statusText); if(Cursor.Current != Cursors.WaitCursor) Cursor.Current = Cursors.WaitCursor; }
            else                                   ZXC.aim_log(statusText);
         }
      }

      // vvDelf - 16.06.2015 - START
      
      ZXC.RtransDao.FailedIzlazTwinsListManager(conn, VvSQL.DB_RW_ActionType.DEL);  // true=reset, by Delf 01.06.2015

      ZXC.RtransDao.ResetRecIDinfoList();
      // vvDelf - 16.06.2015 - end

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg(MessageBoxIcon.Information, "MSU Regeneriranje je gotovo.");

      ZXC.RenewCache_InProgress = false;

      #region ErrorList Report (Missing ArtiklCDs)

      // 04.10.2011:
      if(ZXC.ErrorsList != null && ZXC.ErrorsList.Count().NotZero())
      {
         List<string> distinctSortedList = ZXC.ErrorsList.Distinct().OrderBy(err => err).ToList();

         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\Vv_MSU_CheckCache_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, System.Text.Encoding.GetEncoding(1250)))
         {
            foreach(string error in distinctSortedList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "MSU Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nNedostaje {2} artikala. ", ZXC.ErrorsList.Count(), fName, distinctSortedList.Count());
      }

      #endregion ErrorList Report

      ZXC.ClearStatusText();

      return allOK;
   }

   #endregion Check Cache

   #region RwtrecAditionalAction - On TS Changed invalidate cache

   public override bool RwtrecAditionalAction(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      bool   OK         = true;
      Artikl artikl_rec = vvDataRecord as Artikl;

      string oldTS = artikl_rec.BackupData._ts;
      string newTS = artikl_rec           .TS ;

      decimal oldR_OrgPak = artikl_rec.R_BackupDataOrgPak;
      decimal newR_OrgPak = artikl_rec.R_orgPak          ;

      string oldPDV = artikl_rec.BackupData._pdvKat;
      string newPDV = artikl_rec           .PdvKat ;

    //if(oldTS == newTS) return true;
      if(oldTS == newTS && oldR_OrgPak == newR_OrgPak && oldPDV == newPDV) return true;

      // 12.11.2012: 
    //bool oldTSwasUsluga = (oldTS == "USL" || oldTS == "PRS" || oldTS == "TAK");
    //bool newTSisUsluga  = (newTS == "USL" || newTS == "PRS" || newTS == "TAK");
      bool oldTSwasUsluga = ZXC.NoNeedForZaliha_ArtiklTSs.Contains(oldTS);
      bool newTSisUsluga  = ZXC.NoNeedForZaliha_ArtiklTSs.Contains(newTS);

      bool oldTSwasntUsluga = !oldTSwasUsluga;
      bool newTSisntUsluga  = !newTSisUsluga;

      if(oldTSwasUsluga   && newTSisUsluga   && oldR_OrgPak == newR_OrgPak && oldPDV == newPDV) return true;
      if(oldTSwasntUsluga && newTSisntUsluga && oldR_OrgPak == newR_OrgPak && oldPDV == newPDV) return true;

      //______________________________   
      //______________________________   
      // If we are still here, meaning: usluga/notUsluga changed... OR OrgPak changed... OR PdvSt changed... should invalidate cache! 
      //______________________________   
      //______________________________   


      ZXC.aim_emsg(MessageBoxIcon.Warning, "Cache je potrebno regenerirati (promjena TS-a ili OrgPak-a Artikla ili PdvSt).\n\r\n\rPričekajte, molim, obradu.");

      Cursor.Current = Cursors.WaitCursor;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);
      
      // --- Here we go! --- 

      Rtrans rtrans_rec = new Rtrans();
      
      var skladCDlist = GetDistinctSkladCdListForArtikl(conn, artikl_rec.ArtiklCD).Where(sklCD => sklCD.NotEmpty());

      foreach(string currSkladCD in skladCDlist)
      {
         rtrans_rec.T_artiklCD = artikl_rec.ArtiklCD;
         rtrans_rec.T_skladCD  = currSkladCD;

         // --- VOILA!!! ------------------------- 

         ZXC.RtransDao.Delete_Then_Renew_Cache_FromThisRtrans(conn, rtrans_rec, VvSQL.DB_RW_ActionType.DEL);
      }

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg(MessageBoxIcon.Information, "Regeneriranje je gotovo.");
      
      return OK;
   }

   #endregion RwtrecAditionalAction - On TS Changed invalidate cache

   #endregion :-) CACHE - GetArtiklStatus

   #region GetArtiklWithArtstatList for RiskReport

   // 23.12.2022: 
 //internal static void         GetArtiklWithArtstatList(XSqlConnection conn, List<Artikl> artiklList, string _skladCD, DateTime _dateDo, VvRpt_RiSk_Filter RptFilter, /*fuse*/ string artiklColumns, string orderBy)
   internal static List<Artikl> GetArtiklWithArtstatList(XSqlConnection conn, List<Artikl> artiklList, string _skladCD, DateTime _dateDo, VvRpt_RiSk_Filter RptFilter, /*fuse*/ string artiklColumns, string orderBy)
   {
      bool success = true;
      Artikl artikl_rec;

      ZXC.sqlErrNo = 0;

      List<Artikl> thisPassArtiklList = new List<Artikl>();

      using(XSqlCommand cmd = (VvSQL.GetArtiklWithArtstatList_Command(conn, _skladCD, _dateDo, RptFilter, artiklColumns, orderBy)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;

               while(success && reader.Read())
               {
                  artikl_rec = new Artikl();

                  ZXC.ArtiklDao .FillFromDataReader(artikl_rec,         reader, false);
                  ZXC.ArtStatDao.FillFromDataReader(artikl_rec.TheAsEx, reader, false, ArtiklDao.lastArtiklCI + 1);

                  artiklList        .Add(artikl_rec); // ova je kumulativna 
                  thisPassArtiklList.Add(artikl_rec); // ova je this pass   
               }
               reader.Close();
            }
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("GetArtiklWithArtstatList", ex, System.Windows.Forms.MessageBoxButtons.OK);

            ZXC.sqlErrNo = ex.Number;
         }
      } // using 

      return thisPassArtiklList;
   }

   internal static void GetKretanjeSkladList(XSqlConnection conn, List<ZXC.VvUtilDataPackage> theList, /*string _skladCD, DateTime _dateOd, DateTime _dateDo,*/ VvRpt_RiSk_Filter RptFilter, string ulazClause, string izlazClause)
   {
      // TODO: da ovo trza i na grupiranje po grupi artikla, tipu, ... kao i std skladReports 

      bool success = true;
      //VvReportSourceUtil rsu_rec;
      ZXC.VvUtilDataPackage the_rec;

      ZXC.sqlErrNo = 0;

      using(XSqlCommand cmd = (VvSQL.GetKretanjeSkladList_Command(conn, RptFilter, ulazClause, izlazClause)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;
               while(success && reader.Read())
               {
                  //rsu_rec = new VvReportSourceUtil();

                  //rsu_rec.TheDate   = reader.GetDateTime(0);
                  //rsu_rec.TheMoney  = reader.GetDecimal (1);
                  //rsu_rec.TheMoney2 = reader.GetDecimal (2);

                  //rsuList.Add(rsu_rec);

                  the_rec = new ZXC.VvUtilDataPackage(reader.GetDateTime(0),
                                                      reader.GetDecimal (1),
                                                      reader.GetDecimal (2));

                  theList.Add(the_rec);

               }
               reader.Close();
            }
         }
         catch(XSqlException ex) { success = false; VvSQL.ReportSqlError("GetKretanjeSkladList", ex, System.Windows.Forms.MessageBoxButtons.OK); ZXC.sqlErrNo = ex.Number; }
      } // using 

   }

   internal static void GetStanjeSklPoRNPList(XSqlConnection conn, List<ZXC.VvUtilDataPackage> theList, /*string _skladCD, DateTime _dateOd, DateTime _dateDo,*/ VvRpt_RiSk_Filter RptFilter, string ulazClause, string izlazClause, string ulazIzlazClause)
   {
      // TODO: da ovo trza i na grupiranje po grupi artikla, tipu, ... kao i std skladReports 

      bool success = true;
      //VvReportSourceUtil rsu_rec;
      ZXC.VvUtilDataPackage the_rec;

      ZXC.sqlErrNo = 0;

      using(XSqlCommand cmd = (VvSQL.GetStanjeSklPoRNPList_Command(conn, RptFilter, ulazClause, izlazClause, ulazIzlazClause)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;
               while(success && reader.Read())
               {
                  the_rec = new ZXC.VvUtilDataPackage(reader.GetString (0),
                                                      reader.GetString (1),
                                                      reader.GetDecimal(2),
                                                      reader.GetDecimal(3));
                  theList.Add(the_rec);

               }
               reader.Close();
            }
         }
         catch(XSqlException ex) { success = false; VvSQL.ReportSqlError("GetStanjeSklPoRNPList", ex, System.Windows.Forms.MessageBoxButtons.OK); ZXC.sqlErrNo = ex.Number; }
      } // using 

   }

   internal static void GetStanjeReversaList(XSqlConnection conn, List<ZXC.VvUtilDataPackage> theList, /*string _skladCD, DateTime _dateOd, DateTime _dateDo,*/ VvRpt_RiSk_Filter RptFilter, string ulazClause, string izlazClause, string ulazIzlazClause)
   {
      // TODO: da ovo trza i na grupiranje po grupi artikla, tipu, ... kao i std skladReports 

      bool success = true;
      //VvReportSourceUtil rsu_rec;
      ZXC.VvUtilDataPackage the_rec;

      ZXC.sqlErrNo = 0;

      using(XSqlCommand cmd = (VvSQL.GetStanjeReversaList_Command(conn, RptFilter, ulazClause, izlazClause, ulazIzlazClause)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;
               while(success && reader.Read())
               {
                  the_rec = new ZXC.VvUtilDataPackage(reader.GetUInt32 (0),
                                                      reader.GetString (1),
                                                      reader.GetDecimal(2),
                                                      reader.GetDecimal(3));
                  theList.Add(the_rec);

               }
               reader.Close();
            }
         }
         catch(XSqlException ex) { success = false; VvSQL.ReportSqlError("GetStanjeReversaList", ex, System.Windows.Forms.MessageBoxButtons.OK); ZXC.sqlErrNo = ex.Number; }
      } // using 

   }

   internal static void Get_HasKolStOnly_ArtiklWithArtstatList(XSqlConnection conn, List<Artikl> artiklList, string _skladCD, DateTime _dateDo, /*fuse*/ string artiklColumns, string orderBy)
   {
      VvRpt_RiSk_Filter rptFilter = new VvRpt_RiSk_Filter();

      // 06.08.2015: !!! 
      rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArtiklDao.ResCol_StanjeKol, 0M, " <> ", ""));
      ArtiklDao.GetArtiklWithArtstatList(conn, artiklList, _skladCD, _dateDo, rptFilter, artiklColumns, orderBy);
   }

   internal static List<string> Get_NoInventura_YesKolSt_ArtiklWithArtstatList(List<Artikl> yesKolStArtiklList, List<Rtrans> yesInventuraRtransList)
   {
      var naLageruArtiklCDlist = yesKolStArtiklList    .Select(a => a.ArtiklCD  );
      var inventArtiklCDlist   = yesInventuraRtransList.Select(r => r.T_artiklCD);

      var noInventura_YesManjak_ArtiklCdList =

      naLageruArtiklCDlist.Except(inventArtiklCDlist).ToList();

      return noInventura_YesManjak_ArtiklCdList;
   }

   internal static List<string> Get_NoInventura_YesManjak_RtransList(List<Rtrans> yesManjakRtransList, List<Rtrans> yesInventuraRtransList)
   {
      var manjakArtiklCDlist = yesManjakRtransList   .Select(r => r.T_artiklCD);
      var inventArtiklCDlist = yesInventuraRtransList.Select(r => r.T_artiklCD);
      var noInventura_YesManjak_ArtiklCdList =

    //yesManjakRtransList.LeftExcludingJoin(yesInventuraRtransList, mRtr => mRtr.T_artiklCD, invRtr => invRtr.T_artiklCD, (mRtr, invRtr) => mRtr).ToList();
    //yesManjakRtransList.Except(yesInventuraRtransList).ToList();
      manjakArtiklCDlist.Except(inventArtiklCDlist).ToList();

      return noInventura_YesManjak_ArtiklCdList;
   }

   #endregion GetArtiklWithArtstatList for RiskReport


   #region Set_IMPORT_OFFIX_Columns

   //  //____ Specifics 2 start ______________________________________________________

   //fprintf(device, "%s\t", sklad_rec[0].s_artikl_cd);
   //fprintf(device, "%s\t", sklad_rec[0].s_artikl);
   //fprintf(device, "%s\t", sklad_rec[0].s_bcode);
   //fprintf(device, "%s\t", sklad_rec[0].s_grupa1_cd);
   //fprintf(device, "%s\t", sklad_rec[0].s_grupa2_cd);
   //fprintf(device, "%s\t", sklad_rec[0].s_jm);
   //fprintf(device, "%s\t", sklad_rec[0].s_skl_cd);
   //fprintf(device, "%s\t", sklad_rec[0].s_skonto);
   //fprintf(device, "%s\t", sklad_rec[0].s_deviza);
   //fprintf(device, "%.0lf\t", sklad_rec[0].s_marza   * 100.00); // tezina u kg - Intrade 
   //fprintf(device, "%.0lf\t", sklad_rec[0].s_cij_vpc * 100.00);
	
   //fprintf(device, "\n");
	
   //  //____ Specifics 2 end   ______________________________________________________

   public /*override*/ string Set_IMPORT_OFFIX_Columns_ORIG()
   {
      return

         "(" +

         "artiklCD   , " + //fprintf(device, "%s\t", sklad_rec[0].s_artikl_cd);
         "artiklName , " + //fprintf(device, "%s\t", sklad_rec[0].s_artikl);
         "barCode1   , " + //fprintf(device, "%s\t", sklad_rec[0].s_bcode);
         "grupa1CD   , " + //fprintf(device, "%s\t", sklad_rec[0].s_grupa1_cd);
         "grupa2CD   , " + //fprintf(device, "%s\t", sklad_rec[0].s_grupa2_cd);
         "jedMj      , " + //fprintf(device, "%s\t", sklad_rec[0].s_jm);
         "@skladCD   , " + //fprintf(device, "%s\t", sklad_rec[0].s_skl_cd);
         "konto      , " + //fprintf(device, "%s\t", sklad_rec[0].s_skonto);
         "prefValName, " + //fprintf(device, "%s\t", sklad_rec[0].s_deviza);
         "@debljina  , " + //fprintf(device, "%.0lf\t", sklad_rec[0].s_dimZ * 100.00); // debljina / promjer u mm - PPUK
         "@masaNetto , " + //fprintf(device, "%.0lf\t", sklad_rec[0].s_marza   * 100.00); // tezina u kg - Intrade 
         "@cij_vpc     " + //fprintf(device, "%.0lf\t", sklad_rec[0].s_cij_vpc * 100.00);
         ")"    + "\n" +

         "SET " + "\n" +

         //"kupdobCD = @KCD, " + "\n" +

         //"ticker = CONCAT(SUBSTRING(naziv, 1, 3), SUBSTRING(@KCD, 4, 3)), " + "\n" +

         "skladCD  = IF(@skladCD  = '01', 'VPSK', ''), " + "\n" +
         "importCij = @cij_vpc / 100.00, " + "\n" +
         //"isObrt = IF(@isObrt = 'X', 1, 0), " + "\n" +

         "promjer     = @debljina / 100.00, " + "\n" +
         "promjerJM   = 'mm', " + "\n" +
         "masaNetto   = @masaNetto / 100.00, " + "\n" +
         "masaNettoJM = 'kg', " + "\n" +

         //"komentar = CONCAT(@napomena3, ' ', @napomena4), " + "\n" +

         "addTS = CURRENT_TIMESTAMP, modTS = addTS," + "\n" +
         "addUID = '" + ZXC.vvDB_User + "'";
   }

   public override string Set_IMPORT_OFFIX_Columns()
   {
      return

         "(" +

         //"artiklCD   , " + //fprintf(device, "%s\t", sklad_rec[0].s_artikl_cd);
         //"artiklName , " + //fprintf(device, "%s\t", sklad_rec[0].s_artikl);
         //"barCode1   , " + //fprintf(device, "%s\t", sklad_rec[0].s_bcode);
         //"grupa1CD   , " + //fprintf(device, "%s\t", sklad_rec[0].s_grupa1_cd);
         //"grupa2CD   , " + //fprintf(device, "%s\t", sklad_rec[0].s_grupa2_cd);
         //"jedMj      , " + //fprintf(device, "%s\t", sklad_rec[0].s_jm);
         //"@skladCD   , " + //fprintf(device, "%s\t", sklad_rec[0].s_skl_cd);
         //"konto      , " + //fprintf(device, "%s\t", sklad_rec[0].s_skonto);
         //"prefValName, " + //fprintf(device, "%s\t", sklad_rec[0].s_deviza);
         //"@debljina  , " + //fprintf(device, "%.0lf\t", sklad_rec[0].s_dimZ * 100.00); // debljina / promjer u mm - PPUK
         //"@masaNetto , " + //fprintf(device, "%.0lf\t", sklad_rec[0].s_marza   * 100.00); // tezina u kg - Intrade 
         //"@cij_vpc     " + //fprintf(device, "%.0lf\t", sklad_rec[0].s_cij_vpc * 100.00);

         "artiklCD   , " + // fprintf(device, "%s\t", sklad_rec[0].s_artikl_cd );
         "@artiklName, " + // fprintf(device, "%s\t", sklad_rec[0].s_artikl    );
	      "barCode1   , " + // fprintf(device, "%s\t", sklad_rec[0].s_bcode     );
	      "grupa1CD   , " + // fprintf(device, "%s\t", sklad_rec[0].s_grupa1_cd );
	      "grupa2CD   , " + // fprintf(device, "%s\t", sklad_rec[0].s_grupa2_cd );
	      "jedMj      , " + // fprintf(device, "%s\t", sklad_rec[0].s_jm        );
	      "skladCD    , " + // fprintf(device, "%s\t", sklad_rec[0].s_skl_cd    );
	      "konto      , " + // fprintf(device, "%s\t", sklad_rec[0].s_skonto    );
	      "@atestBr   , " + // fprintf(device, "%s\t", sklad_rec[0].s_name      );
	      "artiklCD2  , " + // fprintf(device, "%s\t", sklad_rec[0].s_artikl_cd2);
	      "artiklName2, " + // fprintf(device, "%s\t", sklad_rec[0].s_proizv    );
	      "grupa3CD   , " + // fprintf(device, "%s\t", sklad_rec[0].s_tipB      );
         "pdvKat     , " + // fprintf(device, "%.0lf\t", sklad_rec[0].s_alfa   );
         "orgPak       " + // fprintf(device, "%.0lf\t", ORG                   );


         ")"    + "\n" +

         "SET " + "\n" +

         //"kupdobCD = @KCD, " + "\n" +

         //"ticker = CONCAT(SUBSTRING(naziv, 1, 3), SUBSTRING(@KCD, 4, 3)), " + "\n" +

         //"skladCD  = IF(@skladCD  = '01', 'VPSK', ''), " + "\n" +
         //"importCij = @cij_vpc / 100.00, " + "\n" +
         //"isObrt = IF(@isObrt = 'X', 1, 0), " + "\n" +

         //"promjer     = @debljina / 100.00, " + "\n" +
         //"promjerJM   = 'mm', " + "\n" +
         //"masaNetto   = @masaNetto / 100.00, " + "\n" +
         //"masaNettoJM = 'kg', " + "\n" +

         "artiklName = CONCAT(@artiklName, @atestBr), " + "\n" +

         "addTS = CURRENT_TIMESTAMP, modTS = addTS," + "\n" +
         "addUID = '" + ZXC.vvDB_User + "'";
   }

   internal static void ImportFromOffix_Translate437(XSqlConnection conn)
   {
      //int debugCount;

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Artikl>(conn, Translate437, null, "", ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out int debugCount);
   }

   static bool Translate437(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      Artikl artikl_rec = vvDataRecord as Artikl;

      artikl_rec.ArtiklName  = artikl_rec.ArtiklName.VvTranslate437ToLatin2();
      
      return artikl_rec.EditedHasChanges();
   }

   internal bool SynchronizeArtiklSifrar(XSqlConnection conn, ZXC.WriteMode writeMode, Artikl orig_artikl_rec)
   {
      bool OK = true;

      // vvT1_vv2024_TGPROJ_000001 
      // vvT2_vv2024_TGPLEM_000001 

      string kontra_dbName = 
         ZXC.vvDB_VvDomena == "vvT1" ? conn.Database.Replace("vvT1", "vvT2").Replace("TGPROJ", "TGPLEM") : 
         ZXC.vvDB_VvDomena == "vvT2" ? conn.Database.Replace("vvT2", "vvT1").Replace("TGPLEM", "TGPROJ") : 
         throw new Exception("vvDomena nije niti T1 niuti T2");

      string orig_dbName = conn.Database;

      conn.ChangeDatabase(kontra_dbName);

      Artikl kontra_artikl_rec = null;

      if(orig_artikl_rec is Artikl) { orig_artikl_rec = (orig_artikl_rec as Artikl); kontra_artikl_rec = (orig_artikl_rec as Artikl).MakeDeepCopy();                             }

      bool isArtikl = kontra_artikl_rec     != null;

      if(isArtikl) kontra_artikl_rec.SkladCD = "";

      // ADDREC 
      if(writeMode == ZXC.WriteMode.Add) 
      {
         try
         {
            OK = ADDREC(conn, kontra_artikl_rec, false, false, false, false);
         }
         catch(Exception ex2)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, ex2.Message);
            OK = false;
         }
      } // ADDREC 

      // RWTREC 
      else if(writeMode == ZXC.WriteMode.Edit) 
      {

// jel bi tu sad ako je Faktur trebalo zapravo pobrisati URA-u pa rekonstruirati ADDREC_          
// sta je s cacheom u T2?!                                                                        
// zapravo ga treba pitati (sambunjaka), treba li on zaista ovo i za EDIT, DEL ili samo za ADD    
// ovog jos nikad nije bilo u programu. ADDREC zbog kopiraj, kopi aut, init NY, ... itd ali       
// NE i sinhronizacija kada se izvorni record ispravlja ili briše !!! !!! !!!                     
// I to ne kao clone-irana varijanta (npr. Artikl) nego jos treba modificirati (set URA from IRA) 
         bool foundOK = SetMe_Record_bySomeUniqueColumn(conn, kontra_artikl_rec, orig_artikl_rec.ArtiklCD, ZXC.ArtiklSchemaRows[ZXC.ArtCI.artiklCD], false, false);

         if(foundOK == false)
         {
            ZXC.aim_emsg("SynchronizeArtiklSifrar: nema takvog artikla u kontra database-u!?");
            conn.ChangeDatabase(orig_dbName);
            return false;
         }
         try
         {
            kontra_artikl_rec.BeginEdit();

            kontra_artikl_rec.CurrentData = orig_artikl_rec.CurrentData;

            OK = RWTREC(conn, kontra_artikl_rec, false, false, false, false);

            kontra_artikl_rec.EndEdit();
         }
         catch(Exception ex2)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, ex2.Message);
            OK = false;
         }
      } // RWTREC 

      // DELREC 
      else if(writeMode == ZXC.WriteMode.Delete)
      {
         bool foundOK = SetMe_Record_bySomeUniqueColumn(conn, kontra_artikl_rec, orig_artikl_rec.ArtiklCD, ZXC.ArtiklSchemaRows[ZXC.ArtCI.artiklCD], false, false);

         if(foundOK == false)
         {
            ZXC.aim_emsg("SynchronizeArtiklSifrar: nema takvog artikla u kontra database-u!?");
            conn.ChangeDatabase(orig_dbName);
            return false;
         }
         try
         {
            DELREC(conn, kontra_artikl_rec, false);
         }
         catch(Exception ex2)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, ex2.Message);
            OK = false;
         }

      } // DELREC

      conn.ChangeDatabase(orig_dbName);

      return OK;
   }

   #endregion Set_IMPORT_OFFIX_Columns


}

// Some SQL SELECT Examples
//*daj artikl-e i njihove zadnje artstat-e:*/
//    SELECT artiklCD, artiklName,
//           (pstKol + ulazKol - izlazKol) AS stanjeKol,
//           AST.*, artikl.*
//    FROM      artikl
//    LEFT JOIN artstat AST ON
      
//    AST.recID = (
      
//      SELECT RecID FROM artstat WHERE t_artiklCD = artiklCD
      
//      AND t_skladCD  = 'VPSK' AND t_skladDate <= '2011-01-02'
      
//      ORDER BY t_skladDate DESC, t_ttSort DESC, t_ttNum DESC, t_serial DESC
      
//      LIMIT 1
//    )
      
//    WHERE pstKol + ulazKol - izlazKol > 0
      
//    ORDER BY artiklCD

//=============================================================================== 
//*daj artstat-e koji fale:*/
//SELECT COUNT(*)
//FROM rtrans  R

//LEFT JOIN     artstat A

//ON R.recID =
//   A.rtransRecID

//WHERE A.recID IS NULL
//AND R.t_artiklCD != 'qwe'
//AND R.t_tt IN ('URA', 'IRA')

//=============================================================================== 
// daj artstat-e za sve rtrans-e neke fakture
//SELECT A.*

//FROM      rtrans  R
//LEFT JOIN artstat A

//ON R.recID = A.rtransRecID

//WHERE R.t_parentID = 21
