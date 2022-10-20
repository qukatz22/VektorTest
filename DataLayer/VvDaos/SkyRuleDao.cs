using System;
using System.Data;
using System.Collections.Generic;
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
#endif

public sealed class SkyRuleDao : VvDaoBase, IVvDao
{
   
   #region Singleton Constructor & instancer

   private static SkyRuleDao instance;

   private SkyRuleDao(XSqlConnection conn, string dbName) : base(dbName, SkyRule.recordNameArhiva, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static SkyRuleDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if (instance == null)
      {
         instance = new SkyRuleDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableSkyRule

   public static   uint TableVersionStatic { get { return 9; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string OLD_Create_table_definition(bool isArhiva)
   {
      return 
      (
        "recID int(10) unsigned NOT NULL auto_increment,\n" +
        "addTS timestamp                 default '0000-00-00 00:00:00',\n" +
        "modTS timestamp                 default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,\n" +
        "addUID varchar(16)     NOT NULL default 'XY',\n" +
        "modUID varchar(16)     NOT NULL default '',\n" +
        CreateTable_LanSrvID_And_LanRecID_Columns +

        "record        varchar(16)          NOT NULL default '', \n" +
        "documTT       char(3)              NOT NULL default '', \n" +
        "ruleFor      enum ('NONE', 'POSL',     'CENT'    , 'SKY'                       ) NOT NULL, \n" +
        "birthLoc     enum ('NONE', 'POSL',     'CENT'    , 'SKY'                       ) NOT NULL, \n" +
        "frsSklKind   enum ('NONE', 'CENTvpsk', 'POVRvpsk', 'POSLvpsk'      , 'POSLmpsk') NOT NULL, \n" +
        "shareKind    enum ('NONE', 'WithALL' , 'WithCENT', 'WithSinglePOSL'            ) NOT NULL, \n" +
        "operation    enum ('NONE', 'RECEIVE' , 'SEND'    , 'SendAndReceive'            ) NOT NULL, \n" +
        "isOnly4LocSk tinyint(1) unsigned NOT NULL default '0',                                     \n" +

         CreateTable_ArhivaExtensionDefinition(isArhiva) +

        "PRIMARY KEY (recID),\n" +
        (isArhiva ? "" : "UNIQUE ") + "KEY BY_code (record, documTT, ruleFor, birthLoc, frsSklKind)\n" +

        (isArhiva ? ", KEY BYqOrigRecID (origRecID)\n" : "")

      );
   }

   public static string Create_table_definition(bool isArhiva)
   {
      return 
      (
        "recID int(10) unsigned NOT NULL auto_increment,\n" +
        "addTS timestamp                 default '0000-00-00 00:00:00',\n" +
        "modTS timestamp                 default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,\n" +
        "addUID varchar(16)     NOT NULL default 'XY',\n" +
        "modUID varchar(16)     NOT NULL default '',\n" +
        CreateTable_LanSrvID_And_LanRecID_Columns +

        "record       varchar(16)                                                         NOT NULL default '', \n" +
        "documTT      char(3)                                                             NOT NULL default '', \n" +
        "birthLoc     enum ('NONE', 'CENT',     'SHOP'    , 'SKY'                       ) NOT NULL,            \n" +
        "skl1kind     enum ('NONE', 'CentGLSK', 'CentPVSK', 'ShopVPSK', 'ShopMPSK'      ) NOT NULL,            \n" +
        "skl2kind     enum ('NONE', 'CentGLSK', 'CentPVSK', 'ShopVPSK', 'ShopMPSK'      ) NOT NULL,            \n" +
        "centOPS      enum ('NONE', 'RECEIVE' , 'SEND'    , 'SendAndReceive'            ) NOT NULL,            \n" +
        "shopOPS      enum ('NONE', 'RECEIVE' , 'SEND'    , 'SendAndReceive'            ) NOT NULL,            \n" +
        "shopRCVkind  enum ('NONE', 'EVERYTHING', 'OnlyLOCALskl1', 'OnlyOTHERskl1', 'OnlyLOCALskl2', 'OnlyOTHERskl2') NOT NULL,\n" +
        "centCanADD   tinyint(1) unsigned NOT NULL default '0',                                                \n" +
        "centCanRWT   tinyint(1) unsigned NOT NULL default '0',                                                \n" +
        "centCanDEL   tinyint(1) unsigned NOT NULL default '0',                                                \n" +
        "shopCanADD   tinyint(1) unsigned NOT NULL default '0',                                                \n" +
        "shopCanRWT   tinyint(1) unsigned NOT NULL default '0',                                                \n" +
        "shopCanDEL   tinyint(1) unsigned NOT NULL default '0',                                                \n" +
        "opis         varchar(128)        NOT NULL default '',                                                 \n" +
        "notBkgrndSND tinyint(1) unsigned NOT NULL default '0',                                                \n" +
        "notSNDonExLd tinyint(1) unsigned NOT NULL default '0',                                                \n" +
        "notRCVonLoad tinyint(1) unsigned NOT NULL default '0',                                                \n" +

         CreateTable_ArhivaExtensionDefinition(isArhiva) +

        "PRIMARY KEY (recID),\n" +
        (isArhiva ? "" : "") + "KEY BY_code (record, documTT, birthLoc, skl1kind, skl2kind)\n" +

        (isArhiva ? ", KEY BYqOrigRecID (origRecID)\n" : "")

      );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = SkyRule.recordNameArhiva;
      else         tableName = SkyRule.recordName;

      switch(catchingVersion)
      {
         case 2: return AlterTable_LanSrvID_And_LanRecID_Columns;
         case 3: return ("ADD COLUMN operation     tinyint(1)  unsigned NOT NULL default '0' AFTER timeOfDay;\n");

         case 4: return ("DROP INDEX BY_code;\n");

         case 5: return ("DROP   COLUMN skyServerID   ,\n" +
                         "DROP   COLUMN skyServerName ,\n" +
                         "DROP   COLUMN lanServerID   ,\n" +
                         "DROP   COLUMN lanServerName ,\n" +
                         "DROP   COLUMN skladCD       ,\n" +
                         "DROP   COLUMN prjktID       ,\n" +
                         "DROP   COLUMN prjktTick     ,\n" +
                         "DROP   COLUMN isExclusive   ,\n" +
                         "DROP   COLUMN frequency     ,\n" +
                         "DROP   COLUMN timeOfDay     ,\n" +
                         "MODIFY COLUMN operation   enum ('NONE', 'RECEIVE' , 'SEND'    , 'SendAndReceive'            ) NOT NULL                 , \n" +
                         "ADD    COLUMN ruleFor     enum ('NONE', 'POSL',     'CENT'    , 'SKY'                       ) NOT NULL AFTER operation , \n" +
                         "ADD    COLUMN birthLoc    enum ('NONE', 'POSL',     'CENT'    , 'SKY'                       ) NOT NULL AFTER ruleFor   , \n" +
                         "ADD    COLUMN frsSklKind  enum ('NONE', 'CENTvpsk', 'POVRvpsk', 'POSLvpsk'      , 'POSLmpsk') NOT NULL AFTER birthLoc  , \n" +
                         "ADD    COLUMN shareKind   enum ('NONE', 'WithALL' , 'WithCENT', 'WithSinglePOSL'            ) NOT NULL AFTER frsSklKind; \n");

         case 6: return ("ADD INDEX BY_code (record, documTT, ruleFor, birthLoc, frsSklKind);\n");

         case 7: return ("ADD COLUMN isOnly4LocSk tinyint(1) unsigned NOT NULL default '0' AFTER operation;\n");

         case 8: return ("DROP   COLUMN record      ,\n" +
                         "DROP   COLUMN documTT     ,\n" +
                         "DROP   COLUMN ruleFor     ,\n" +
                         "DROP   COLUMN birthLoc    ,\n" +
                         "DROP   COLUMN frsSklKind  ,\n" +
                         "DROP   COLUMN shareKind   ,\n" +
                         "DROP   COLUMN operation   ,\n" +
                         "DROP   COLUMN isOnly4LocSk,\n" +
                         "DROP   INDEX BY_code      ,\n" +
                         "ADD    COLUMN record       varchar(16)                                                         NOT NULL default '', \n" +
                         "ADD    COLUMN documTT      char(3)                                                             NOT NULL default '', \n" +
                         "ADD    COLUMN birthLoc     enum ('NONE', 'CENT',     'SHOP'    , 'SKY'                       ) NOT NULL,            \n" +
                         "ADD    COLUMN skl1kind     enum ('NONE', 'CentGLSK', 'CentPVSK', 'ShopVPSK', 'ShopMPSK'      ) NOT NULL,            \n" +
                         "ADD    COLUMN skl2kind     enum ('NONE', 'CentGLSK', 'CentPVSK', 'ShopVPSK', 'ShopMPSK'      ) NOT NULL,            \n" +
                         "ADD    COLUMN centOPS      enum ('NONE', 'RECEIVE' , 'SEND'    , 'SendAndReceive'            ) NOT NULL,            \n" +
                         "ADD    COLUMN shopOPS      enum ('NONE', 'RECEIVE' , 'SEND'    , 'SendAndReceive'            ) NOT NULL,            \n" +
                         "ADD    COLUMN shopRCVkind  enum ('NONE', 'EVERYTHING', 'OnlyLOCALskl1', 'OnlyOTHERskl1', 'OnlyLOCALskl2', 'OnlyOTHERskl2') NOT NULL,\n" +
                         "ADD    COLUMN centCanADD   tinyint(1) unsigned NOT NULL default '0',                                                \n" +
                         "ADD    COLUMN centCanRWT   tinyint(1) unsigned NOT NULL default '0',                                                \n" +
                         "ADD    COLUMN centCanDEL   tinyint(1) unsigned NOT NULL default '0',                                                \n" +
                         "ADD    COLUMN shopCanADD   tinyint(1) unsigned NOT NULL default '0',                                                \n" +
                         "ADD    COLUMN shopCanRWT   tinyint(1) unsigned NOT NULL default '0',                                                \n" +
                         "ADD    COLUMN shopCanDEL   tinyint(1) unsigned NOT NULL default '0',                                                \n" +
                         "ADD    COLUMN opis         varchar(128)        NOT NULL default '',                                                 \n" +
                         "ADD    INDEX BY_code (record, documTT, birthLoc, skl1kind, skl2kind);                                               \n");

         case 9: return ("ADD    COLUMN notBkgrndSND tinyint(1) unsigned NOT NULL default '0' AFTER opis        ,                             \n" +
                         "ADD    COLUMN notSNDonExLd tinyint(1) unsigned NOT NULL default '0' AFTER notBkgrndSND,                             \n" +
                         "ADD    COLUMN notRCVonLoad tinyint(1) unsigned NOT NULL default '0' AFTER notSNDonExLd;                             \n");

         // ALTERIRAJ KASNIJE OVAJ INDEX DA BUDE UNIQUE

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableSkyRule

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      SkyRule skyRule = (SkyRule)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.ID_Only ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         //VvSQL.CreateCommandParameter(cmd, where_or_and, "prjktKupdobCD", skyRule.RecID, XSqlDbType.Int32, 10);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.RecID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.AddTS,     TheSchemaTable.Rows[CI.addTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.ModTS,     TheSchemaTable.Rows[CI.modTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.AddUID,    TheSchemaTable.Rows[CI.addUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.ModUID,    TheSchemaTable.Rows[CI.modUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.LanSrvID,  TheSchemaTable.Rows[CI.lanSrvID]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.LanRecID,  TheSchemaTable.Rows[CI.lanRecID]);

       //VvSQL.CreateCommandParameter(cmd, preffix, skyRule.SkyServerID  , TheSchemaTable.Rows[CI.skyServerID  ]);
       //VvSQL.CreateCommandParameter(cmd, preffix, skyRule.SkyServerName, TheSchemaTable.Rows[CI.skyServerName]);
       //VvSQL.CreateCommandParameter(cmd, preffix, skyRule.LanServerID  , TheSchemaTable.Rows[CI.lanServerID  ]);
       //VvSQL.CreateCommandParameter(cmd, preffix, skyRule.LanServerName, TheSchemaTable.Rows[CI.lanServerName]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.Record       , TheSchemaTable.Rows[CI.record       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.DocumTT      , TheSchemaTable.Rows[CI.documTT      ]);
       //VvSQL.CreateCommandParameter(cmd, preffix, skyRule.SkladCD      , TheSchemaTable.Rows[CI.skladCD      ]);
       //VvSQL.CreateCommandParameter(cmd, preffix, skyRule.PrjktID      , TheSchemaTable.Rows[CI.prjktID      ]);
       //VvSQL.CreateCommandParameter(cmd, preffix, skyRule.PrjktTick    , TheSchemaTable.Rows[CI.prjktTick    ]);
       //VvSQL.CreateCommandParameter(cmd, preffix, skyRule.IsExclusive  , TheSchemaTable.Rows[CI.isExclusive  ]);
       //VvSQL.CreateCommandParameter(cmd, preffix, skyRule.Frequency    , TheSchemaTable.Rows[CI.frequency    ]);
       //VvSQL.CreateCommandParameter(cmd, preffix, skyRule.TimeOfDay    , TheSchemaTable.Rows[CI.timeOfDay    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.BirthLoc     , TheSchemaTable.Rows[CI.birthLoc    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.Skl1kind     , TheSchemaTable.Rows[CI.skl1kind    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.Skl2kind     , TheSchemaTable.Rows[CI.skl2kind    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.CentOPS      , TheSchemaTable.Rows[CI.centOPS     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.ShopOPS      , TheSchemaTable.Rows[CI.shopOPS     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.ShopRCVkind  , TheSchemaTable.Rows[CI.shopRCVkind ]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.CentCanADD   , TheSchemaTable.Rows[CI.centCanADD  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.CentCanRWT   , TheSchemaTable.Rows[CI.centCanRWT  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.CentCanDEL   , TheSchemaTable.Rows[CI.centCanDEL  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.ShopCanADD   , TheSchemaTable.Rows[CI.shopCanADD  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.ShopCanRWT   , TheSchemaTable.Rows[CI.shopCanRWT  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.ShopCanDEL   , TheSchemaTable.Rows[CI.shopCanDEL  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.Opis         , TheSchemaTable.Rows[CI.opis        ]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.NotBkgrndSND , TheSchemaTable.Rows[CI.notBkgrndSND]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.NotSNDonExLd , TheSchemaTable.Rows[CI.notSNDonExLd]);
         VvSQL.CreateCommandParameter(cmd, preffix, skyRule.NotRCVonLoad , TheSchemaTable.Rows[CI.notRCVonLoad]);

         if(isArhiva)
         {
            VvSQL.CreateCommandParameter(cmd, preffix, skyRule.OrigRecID, TheSchemaTable.Rows[CI.origRecID]);
            VvSQL.CreateCommandParameter(cmd, preffix, skyRule.RecVer,    TheSchemaTable.Rows[CI.recVer]);
            VvSQL.CreateCommandParameter(cmd, preffix, skyRule.ArAction,  TheSchemaTable.Rows[CI.arAction]);
            VvSQL.CreateCommandParameter(cmd, preffix, skyRule.ArTS  ,    TheSchemaTable.Rows[CI.arTS]);
            VvSQL.CreateCommandParameter(cmd, preffix, skyRule.ArUID,     TheSchemaTable.Rows[CI.arUID]);
         }
      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva)
   {
      SkyRuleStruct rdrData = new SkyRuleStruct();

      rdrData._recID     = reader.GetUInt32  (CI.recID);
      rdrData._addTS     = reader.GetDateTime(CI.addTS);
      rdrData._modTS     = reader.GetDateTime(CI.modTS);
      rdrData._addUID    = reader.GetString  (CI.addUID);
      rdrData._modUID    = reader.GetString  (CI.modUID);
      rdrData._lanSrvID  = reader.GetUInt32  (CI.lanSrvID);
      rdrData._lanRecID  = reader.GetUInt32  (CI.lanRecID);

    //rdrData._skyServerID   = reader.GetUInt32  (CI.skyServerID  );
    //rdrData._skyServerName = reader.GetString  (CI.skyServerName);
    //rdrData._lanServerID   = reader.GetUInt32  (CI.lanServerID  );
    //rdrData._lanServerName = reader.GetString  (CI.lanServerName);
      rdrData._record        = reader.GetString  (CI.record       );
      rdrData._documTT       = reader.GetString  (CI.documTT      );
    //rdrData._skladCD       = reader.GetString  (CI.skladCD      );
    //rdrData._prjktID       = reader.GetUInt32  (CI.prjktID      );
    //rdrData._prjktTick     = reader.GetString  (CI.prjktTick    );
    //rdrData._isExclusive   = reader.GetBoolean (CI.isExclusive  );
    //rdrData._frequency     = reader.GetUInt32  (CI.frequency    );
    //rdrData._timeOfDay     = reader.GetDateTime(CI.timeOfDay    );
      rdrData._birthLoc      = (ZXC.LanSrvKind)    Enum.Parse(typeof(ZXC.LanSrvKind    ), reader.GetString(CI.birthLoc    ), true);
      rdrData._skl1kind      = (ZXC.SkySklKind)    Enum.Parse(typeof(ZXC.SkySklKind    ), reader.GetString(CI.skl1kind    ), true);
      rdrData._skl2kind      = (ZXC.SkySklKind)    Enum.Parse(typeof(ZXC.SkySklKind    ), reader.GetString(CI.skl2kind    ), true);
      rdrData._centOPS       = (ZXC.SkyOperation)  Enum.Parse(typeof(ZXC.SkyOperation  ), reader.GetString(CI.centOPS     ), true);
      rdrData._shopOPS       = (ZXC.SkyOperation)  Enum.Parse(typeof(ZXC.SkyOperation  ), reader.GetString(CI.shopOPS     ), true);
      rdrData._shopRCVkind   = (ZXC.SkyReceiveKind)Enum.Parse(typeof(ZXC.SkyReceiveKind), reader.GetString(CI.shopRCVkind ), true);
      rdrData._centCanADD    = reader.GetBoolean                                                          (CI.centCanADD  );
      rdrData._centCanRWT    = reader.GetBoolean                                                          (CI.centCanRWT  );
      rdrData._centCanDEL    = reader.GetBoolean                                                          (CI.centCanDEL  );
      rdrData._shopCanADD    = reader.GetBoolean                                                          (CI.shopCanADD  );
      rdrData._shopCanRWT    = reader.GetBoolean                                                          (CI.shopCanRWT  );
      rdrData._shopCanDEL    = reader.GetBoolean                                                          (CI.shopCanDEL  );
      rdrData._opis          = reader.GetString                                                           (CI.opis        );
      rdrData._notBkgrndSND  = reader.GetBoolean                                                          (CI.notBkgrndSND);
      rdrData._notSNDonExLd  = reader.GetBoolean                                                          (CI.notSNDonExLd);
      rdrData._notRCVonLoad  = reader.GetBoolean                                                          (CI.notRCVonLoad);

      ((SkyRule)vvDataRecord).CurrentData = rdrData;

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

   public struct SkyRuleCI
   {
      internal int recID;
      internal int addTS;
      internal int modTS;
      internal int addUID;
      internal int modUID;
      internal int lanSrvID;
      internal int lanRecID;

    //internal int skyServerID  ;
    //internal int skyServerName;
    //internal int lanServerID  ;
    //internal int lanServerName;
      internal int record       ;
      internal int documTT      ;
    //internal int skladCD      ;
    //internal int prjktID      ;
    //internal int prjktTick    ;
    //internal int isExclusive  ;
    //internal int frequency    ;
    //internal int timeOfDay    ;
      internal int birthLoc     ;
      internal int skl1kind   ;
      internal int skl2kind   ;
      internal int centOPS    ;
      internal int shopOPS    ;
      internal int shopRCVkind;
      internal int centCanADD ;
      internal int centCanRWT ;
      internal int centCanDEL ;
      internal int shopCanADD ;
      internal int shopCanRWT ;
      internal int shopCanDEL ;
      internal int opis       ;
      internal int notBkgrndSND;
      internal int notSNDonExLd;
      internal int notRCVonLoad;

      internal int origRecID;
      internal int recVer;
      internal int arAction;
      internal int arTS;
      internal int arUID;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public SkyRuleCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID       = GetSchemaColumnIndex("recID");
      CI.addTS       = GetSchemaColumnIndex("addTS");
      CI.modTS       = GetSchemaColumnIndex("modTS");
      CI.addUID      = GetSchemaColumnIndex("addUID");
      CI.modUID      = GetSchemaColumnIndex("modUID");
      CI.lanSrvID    = GetSchemaColumnIndex("lanSrvID");
      CI.lanRecID    = GetSchemaColumnIndex("lanRecID");

    //CI.skyServerID   = GetSchemaColumnIndex("skyServerID"  );
    //CI.skyServerName = GetSchemaColumnIndex("skyServerName");
    //CI.lanServerID   = GetSchemaColumnIndex("lanServerID"  );
    //CI.lanServerName = GetSchemaColumnIndex("lanServerName");
      CI.record        = GetSchemaColumnIndex("record"       );
      CI.documTT       = GetSchemaColumnIndex("documTT"      );
    //CI.skladCD       = GetSchemaColumnIndex("skladCD"      );
    //CI.prjktID       = GetSchemaColumnIndex("prjktID"      );
    //CI.prjktTick     = GetSchemaColumnIndex("prjktTick"    );
    //CI.isExclusive   = GetSchemaColumnIndex("isExclusive"  );
    //CI.frequency     = GetSchemaColumnIndex("frequency"    );
    //CI.timeOfDay     = GetSchemaColumnIndex("timeOfDay"    );
    //CI.ruleFor       = GetSchemaColumnIndex("ruleFor"      );
      CI.birthLoc      = GetSchemaColumnIndex("birthLoc"     );
      CI.skl1kind      = GetSchemaColumnIndex("skl1kind"     );
      CI.skl2kind      = GetSchemaColumnIndex("skl2kind"     );
      CI.centOPS       = GetSchemaColumnIndex("centOPS"      );
      CI.shopOPS       = GetSchemaColumnIndex("shopOPS"      );
      CI.shopRCVkind   = GetSchemaColumnIndex("shopRCVkind"  );
      CI.centCanADD    = GetSchemaColumnIndex("centCanADD"   );
      CI.centCanRWT    = GetSchemaColumnIndex("centCanRWT"   );
      CI.centCanDEL    = GetSchemaColumnIndex("centCanDEL"   );
      CI.shopCanADD    = GetSchemaColumnIndex("shopCanADD"   );
      CI.shopCanRWT    = GetSchemaColumnIndex("shopCanRWT"   );
      CI.shopCanDEL    = GetSchemaColumnIndex("shopCanDEL"   );
      CI.opis          = GetSchemaColumnIndex("opis"         );
      CI.notBkgrndSND  = GetSchemaColumnIndex("notBkgrndSND" );
      CI.notSNDonExLd  = GetSchemaColumnIndex("notSNDonExLd" );
      CI.notRCVonLoad  = GetSchemaColumnIndex("notRCVonLoad" );

      CI.origRecID      = GetSchemaColumnIndex("origRecID");
      CI.recVer         = GetSchemaColumnIndex("recVer");
      CI.arAction       = GetSchemaColumnIndex("arAction");
      CI.arTS           = GetSchemaColumnIndex("arTS");
      CI.arUID          = GetSchemaColumnIndex("arUID");
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()



   #region Load_SkyRules

   public void Load_SkyRules(XSqlConnection conn) 
   {
      if(ZXC.CURR_SkyRules == null) ZXC.CURR_SkyRules = new List<SkyRule>();
      else                          ZXC.CURR_SkyRules.Clear();

      LoadGenericVvDataRecordList(conn, ZXC.CURR_SkyRules, /*filterMembers*/null, "record, documTT, birthLoc, skl1Kind, skl2Kind");
   }

   #endregion Load_SkyRules

}

