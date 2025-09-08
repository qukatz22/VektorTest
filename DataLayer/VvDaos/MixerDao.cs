using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.OleDb;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using                  MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
using XSqlException  = MySql.Data.MySqlClient.MySqlException;
using XSqlDataReader = MySql.Data.MySqlClient.MySqlDataReader;
#endif

public sealed class MixerDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static MixerDao instance;

   private MixerDao(XSqlConnection conn, string dbName) : base(dbName, Mixer.recordNameArhiva, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static MixerDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new MixerDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableMixer

   public static   uint TableVersionStatic { get { return 6; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition(bool isArhiva)
   {
      return (
         /* 00 */  "recID        int(10)    unsigned NOT NULL auto_increment,\n" +
         /* 01 */  "addTS        timestamp                    NULL DEFAULT NULL,\n" +
         /* 02 */  "modTS        timestamp                    default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,\n" +
         /* 03 */  "addUID       varchar(16)         NOT NULL default 'XY',\n" +
         /* 04 */  "modUID       varchar(16)         NOT NULL default '',\n" +
                 CreateTable_LanSrvID_And_LanRecID_Columns +
         /* 07 */  "dokNum       int(10)    unsigned NOT NULL,\n" +
         /* 08 */  "dokDate      date                NOT NULL default '0001-01-01',\n" +
         /* 09 */  "tt           char(3)             NOT NULL default '',\n" +
         /* 10 */  "ttNum        int(10)    unsigned NOT NULL,\n" +
         /* 11 */  "napomena     varchar(256)        NOT NULL default '',\n" +
         /* 12 */  "intA         int(10)    unsigned NOT NULL default 0, \n" +
         /* 13 */  "dateA        date                NOT NULL default '0001-01-01',\n" +
         /* 14 */  "kupdobCD     int(6)     unsigned NOT NULL default 0, \n" +
         /* 15 */  "mtrosCD      int(6)     unsigned NOT NULL default 0, \n" +
         /* 16 */  "personCD     int(6)     unsigned NOT NULL default 0, \n" +
         /* 17 */  "intB         int(10)             NOT NULL default 0, \n" +
         /* 18 */  "isXxx        tinyint(1) unsigned NOT NULL default 0, \n" +
         /* 19 */  "konto        varchar(8)          NOT NULL default '',\n" +
         /* 20 */  "kupdobTK     char(6)             NOT NULL default '',\n" +
         /* 21 */  "kupdobName   varchar(50)         NOT NULL default '',\n" +
         /* 22 */  "mtrosTK      char(6)             NOT NULL default '',\n" +
         /* 23 */  "personIme    varchar(24)         NOT NULL default '',\n" +
         /* 24 */  "personPrezim varchar(32)         NOT NULL default '',\n" +
         /* 25 */  "devName      char(3)             NOT NULL default '',\n" +
         /* 26 */  "strA_40      varchar(40)         NOT NULL default '',\n" +
         /* 27 */  "strB_128     varchar(128)        NOT NULL default '',\n" +
         /* 28 */  "strC_32      varchar(32)         NOT NULL default '',\n" +
         /* 29 */  "strD_32      varchar(32)         NOT NULL default '',\n" +
         /* 30 */  "strE_256     varchar(256)        NOT NULL default '',\n" +
         /* 31 */  "strF_64      varchar(64)         NOT NULL default '',\n" +
         /* 32 */  "strG_40      varchar(40)         NOT NULL default '',\n" +
         /* 33 */  "dateTimeA    datetime            NOT NULL default '0001-01-01 00:00:00',\n" +
         /* 34 */  "dateTimeB    datetime            NOT NULL default '0001-01-01 00:00:00',\n" +
         /* 35 */  "dateB        date                NOT NULL default '0001-01-01',         \n" +
         /* 36 */  "moneyA       decimal(12,4)       NOT NULL default 0.00,\n" +
         /* 37 */  "moneyB       decimal(12,4)       NOT NULL default 0.00,\n" +
         /* 38 */  "moneyC       decimal(12,4)       NOT NULL default 0.00,\n" +
         /* 39 */  "strH_32      varchar(32)         NOT NULL default ''  ,\n" +
         /* 40 */  "projektCD    varchar(16)         NOT NULL default ''  ,\n" +
         /* 41 */  "v1_tt        char(3)             NOT NULL default ''  ,\n" +
         /* 42 */  "v1_ttNum     int(10)   unsigned  NOT NULL             ,\n" +
         /* 43 */  "v2_tt        char(3)             NOT NULL default ''  ,\n" +
         /* 44 */  "v2_ttNum     int(10)   unsigned  NOT NULL             ,\n" +
         /* 45 */  "konto2       varchar(8)          NOT NULL default ''  ,\n" +
         /* 46 */  "externLink1  varchar(256)        NOT NULL default ''  ,\n" +
         /*147 */  "externLink2  varchar(256)        NOT NULL default ''  ,\n" +

         CreateTable_ArhivaExtensionDefinition(isArhiva) +

                                "PRIMARY KEY            (recID),\n" +
          (isArhiva ? "" : "UNIQUE ") + "KEY BY_DOKNUM  (dokNum),\n" +
          (isArhiva ? "" : "UNIQUE ") + "KEY BY_DOKDATE (dokDate, dokNum),\n" +
          (isArhiva ? "" : "UNIQUE ") + "KEY BY_TTNUM   (tt, ttNum)\n" +

          (isArhiva ? ", KEY BYqOrigRecID (origRecID)\n" : "")


         );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Mixer.recordNameArhiva;
      else         tableName = Mixer.recordName;

      switch(catchingVersion)
      {
         case 2: return ("MODIFY COLUMN napomena     varchar(256)        NOT NULL default '';");

         case 3: return ("ADD COLUMN v1_tt        char(3)             NOT NULL default '' AFTER projektCD,   " +
                         "ADD COLUMN v1_ttNum     int(10)   unsigned  NOT NULL            AFTER v1_tt    ,   " +
                         "ADD COLUMN v2_tt        char(3)             NOT NULL default '' AFTER v1_ttNum ,   " +
                         "ADD COLUMN v2_ttNum     int(10)   unsigned  NOT NULL            AFTER v2_tt    ;   ");

         case 4: return ("ADD konto2       varchar(8)          NOT NULL default '' AFTER v2_ttNum   , " +
                         "ADD externLink1  varchar(256)        NOT NULL default '' AFTER konto2     , " +
                         "ADD externLink2  varchar(256)        NOT NULL            AFTER externLink1; ");

         case 5: return (isArhiva ? "ADD INDEX BYqOrigRecID (origRecID)\n" : "skip_me");

         case 6: return AlterTable_LanSrvID_And_LanRecID_Columns;

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableMixer

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      Mixer mixer = (Mixer)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, mixer.RecID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, mixer.AddTS,    TheSchemaTable.Rows[CI.addTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, mixer.ModTS,    TheSchemaTable.Rows[CI.modTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, mixer.AddUID,   TheSchemaTable.Rows[CI.addUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, mixer.ModUID,   TheSchemaTable.Rows[CI.modUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, mixer.LanSrvID, TheSchemaTable.Rows[CI.lanSrvID]);
         VvSQL.CreateCommandParameter(cmd, preffix, mixer.LanRecID, TheSchemaTable.Rows[CI.lanRecID]);

      /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.DokNum,        TheSchemaTable.Rows[CI.dokNum  ]);
      /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.DokDate,       TheSchemaTable.Rows[CI.dokDate ]);
      /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.TT,            TheSchemaTable.Rows[CI.tt      ]);
      /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.TtNum,         TheSchemaTable.Rows[CI.ttNum   ]);
      /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.Napomena,      TheSchemaTable.Rows[CI.napomena]);
      /* 10 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.IntA,          TheSchemaTable.Rows[CI.intA    ]);
      /* 11 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.DateA,         TheSchemaTable.Rows[CI.dateA   ]);

      /* 12 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.KupdobCD    ,  TheSchemaTable.Rows[CI.kupdobCD    ]);
      /* 13 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.MtrosCD     ,  TheSchemaTable.Rows[CI.mtrosCD     ]);
      /* 14 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.PersonCD    ,  TheSchemaTable.Rows[CI.personCD    ]);
      /* 15 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.IntB        ,  TheSchemaTable.Rows[CI.intB        ]);
      /* 16 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.IsXxx       ,  TheSchemaTable.Rows[CI.isXxx       ]);
      /* 17 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.Konto       ,  TheSchemaTable.Rows[CI.konto       ]);
      /* 18 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.KupdobTK    ,  TheSchemaTable.Rows[CI.kupdobTK    ]);
      /* 19 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.KupdobName  ,  TheSchemaTable.Rows[CI.kupdobName  ]);
      /* 20 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.MtrosTK     ,  TheSchemaTable.Rows[CI.mtrosTK     ]);
      /* 21 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.PersonIme   ,  TheSchemaTable.Rows[CI.personIme   ]);
      /* 22 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.PersonPrezim,  TheSchemaTable.Rows[CI.personPrezim]);
      /* 23 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.DevName     ,  TheSchemaTable.Rows[CI.devName     ]);
      /* 24 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.StrA_40     ,  TheSchemaTable.Rows[CI.strA_40     ]);
      /* 25 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.StrB_128    ,  TheSchemaTable.Rows[CI.strB_128    ]);
      /* 26 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.StrC_32     ,  TheSchemaTable.Rows[CI.strC_32     ]);
      /* 27 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.StrD_32     ,  TheSchemaTable.Rows[CI.strD_32     ]);
      /* 28 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.StrE_256    ,  TheSchemaTable.Rows[CI.strE_256    ]);
      /* 29 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.StrF_64     ,  TheSchemaTable.Rows[CI.strF_64     ]);
      /* 30 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.StrG_40     ,  TheSchemaTable.Rows[CI.strG_40     ]);
      /* 31 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.DateTimeA   ,  TheSchemaTable.Rows[CI.dateTimeA   ]);
      /* 32 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.DateTimeB   ,  TheSchemaTable.Rows[CI.dateTimeB   ]);
      /* 33 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.DateB       ,  TheSchemaTable.Rows[CI.dateB       ]);
      /* 34 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.MoneyA      ,  TheSchemaTable.Rows[CI.moneyA      ]);
      /* 35 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.MoneyB      ,  TheSchemaTable.Rows[CI.moneyB      ]);
      /* 36 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.MoneyC      ,  TheSchemaTable.Rows[CI.moneyC      ]);
      /* 37 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.StrH_32     ,  TheSchemaTable.Rows[CI.strH_32     ]);
      /* 38 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.ProjektCD   ,  TheSchemaTable.Rows[CI.projektCD   ]);
      /* 39 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.V1_tt       ,  TheSchemaTable.Rows[CI.v1_tt       ]);
      /* 40 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.V1_ttNum    ,  TheSchemaTable.Rows[CI.v1_ttNum    ]);
      /* 41 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.V2_tt       ,  TheSchemaTable.Rows[CI.v2_tt       ]);
      /* 42 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.V2_ttNum    ,  TheSchemaTable.Rows[CI.v2_ttNum    ]);
      /* 43 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.Konto2      ,  TheSchemaTable.Rows[CI.konto2      ]);
      /* 44 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.ExternLink1 ,  TheSchemaTable.Rows[CI.externLink1 ]);
      /* 45 */ VvSQL.CreateCommandParameter(cmd, preffix, mixer.ExternLink2 ,  TheSchemaTable.Rows[CI.externLink2 ]);

         if(isArhiva)
         {
            VvSQL.CreateCommandParameter(cmd, preffix, mixer.OrigRecID, TheSchemaTable.Rows[CI.origRecID]);
            VvSQL.CreateCommandParameter(cmd, preffix, mixer.RecVer,    TheSchemaTable.Rows[CI.recVer]);
            VvSQL.CreateCommandParameter(cmd, preffix, mixer.ArAction,  TheSchemaTable.Rows[CI.arAction]);
            VvSQL.CreateCommandParameter(cmd, preffix, mixer.ArTS,      TheSchemaTable.Rows[CI.arTS]);
            VvSQL.CreateCommandParameter(cmd, preffix, mixer.ArUID,     TheSchemaTable.Rows[CI.arUID]);
         }
      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva)
   {
      MixerStruct rdrData = new MixerStruct();

      rdrData._recID    = reader.GetUInt32  (CI.recID);
      rdrData._addTS    = reader.GetDateTime(CI.addTS);
      rdrData._modTS    = reader.GetDateTime(CI.modTS);
      rdrData._addUID   = reader.GetString  (CI.addUID);
      rdrData._modUID   = reader.GetString  (CI.modUID);
      rdrData._lanSrvID = reader.GetUInt32  (CI.lanSrvID);
      rdrData._lanRecID = reader.GetUInt32  (CI.lanRecID);

      /* 05 */      rdrData._dokNum       = reader.GetUInt32   (CI.dokNum);
      /* 06 */      rdrData._dokDate      = reader.GetDateTime (CI.dokDate);
      /* 07 */      rdrData._tt           = reader.GetString   (CI.tt);
      /* 08 */      rdrData._ttNum        = reader.GetUInt32   (CI.ttNum);
      /* 09 */      rdrData._napomena     = reader.GetString   (CI.napomena);
      /* 10 */      rdrData._intA         = reader.GetInt32    (CI.intA);
      /* 11 */      rdrData._dateA        = reader.GetDateTime (CI.dateA);

      /* 12 */      rdrData._kupdobCD     = reader.GetUInt32   (CI.kupdobCD    );
      /* 13 */      rdrData._mtrosCD      = reader.GetUInt32   (CI.mtrosCD     );
      /* 14 */      rdrData._personCD     = reader.GetUInt32   (CI.personCD    );
      /* 15 */      rdrData._intB         = reader.GetInt32    (CI.intB        );
      /* 16 */      rdrData._isXxx        = reader.GetBoolean  (CI.isXxx       );
      /* 17 */      rdrData._konto        = reader.GetString   (CI.konto       );
      /* 18 */      rdrData._kupdobTK     = reader.GetString   (CI.kupdobTK    );
      /* 19 */      rdrData._kupdobName   = reader.GetString   (CI.kupdobName  );
      /* 20 */      rdrData._mtrosTK      = reader.GetString   (CI.mtrosTK     );
      /* 21 */      rdrData._personIme    = reader.GetString   (CI.personIme   );
      /* 22 */      rdrData._personPrezim = reader.GetString   (CI.personPrezim);
      /* 23 */      rdrData._devName      = reader.GetString   (CI.devName     );
      /* 24 */      rdrData._strA_40      = reader.GetString   (CI.strA_40     );
      /* 25 */      rdrData._strB_128     = reader.GetString   (CI.strB_128    );
      /* 26 */      rdrData._strC_32      = reader.GetString   (CI.strC_32     );
      /* 27 */      rdrData._strD_32      = reader.GetString   (CI.strD_32     );
      /* 28 */      rdrData._strE_256     = reader.GetString   (CI.strE_256    );
      /* 29 */      rdrData._strF_64      = reader.GetString   (CI.strF_64     );
      /* 30 */      rdrData._strG_40      = reader.GetString   (CI.strG_40     );
      /* 31 */      rdrData._dateTimeA    = reader.GetDateTime (CI.dateTimeA   );
      /* 32 */      rdrData._dateTimeB    = reader.GetDateTime (CI.dateTimeB   );
      /* 33 */      rdrData._dateB        = reader.GetDateTime (CI.dateB       );
      /* 34 */      rdrData._moneyA       = reader.GetDecimal  (CI.moneyA      );
      /* 35 */      rdrData._moneyB       = reader.GetDecimal  (CI.moneyB      );
      /* 36 */      rdrData._moneyC       = reader.GetDecimal  (CI.moneyC      );
      /* 37 */      rdrData._strH_32      = reader.GetString   (CI.strH_32     );
      /* 38 */      rdrData._projektCD    = reader.GetString   (CI.projektCD   );
      /* 39 */      rdrData._v1_tt        = reader.GetString   (CI.v1_tt       );
      /* 40 */      rdrData._v1_ttNum     = reader.GetUInt32   (CI.v1_ttNum    );
      /* 41 */      rdrData._v2_tt        = reader.GetString   (CI.v2_tt       );
      /* 42 */      rdrData._v2_ttNum     = reader.GetUInt32   (CI.v2_ttNum    );
      /* 43 */      rdrData._konto2       = reader.GetString   (CI.konto2      );
      /* 44 */      rdrData._externLink1  = reader.GetString   (CI.externLink1 );
      /* 45 */      rdrData._externLink2  = reader.GetString   (CI.externLink2 );

      ((Mixer)vvDataRecord).CurrentData = rdrData;

      if(((Mixer)vvDataRecord).Transes != null) ((Mixer)vvDataRecord).Transes.Clear();
      // 13.06.2013: 
      if(((Mixer)vvDataRecord).Transes2 != null) ((Mixer)vvDataRecord).Transes2.Clear();
      if(((Mixer)vvDataRecord).Transes3 != null) ((Mixer)vvDataRecord).Transes3.Clear();

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

   #region FillFrom PNRraster Reader

   public static void FillPNRrasterFromExcelDataReader_transLine(XSqlConnection conn, ref PNRrasterStruct raster_rec, OleDbDataReader reader, uint lineNo)
   {
      PNRrasterStruct rdrData = new PNRrasterStruct();

      rdrData.PutDate    = ZXC.ValOr_01010001_DateTime(reader[0].ToString()   );
      if(rdrData.PutDate.IsEmpty()) // Excel vrati datum kao dayOffset od 1.1.1900 ... kada nema tocke na kraju 
      rdrData.PutDate    = ZXC.ValOr_01010001_DateTime_ExcelIdiot(reader[0].ToString());

      rdrData.Relacija   =                             reader[1].ToString()    ;
      rdrData.KmStart    = ZXC.ValOrZero_Int          (reader[2].ToString()   );
      rdrData.KmEnd      = ZXC.ValOrZero_Int          (reader[3].ToString()   );
      rdrData.KmDiff     = ZXC.ValOrZero_Int          (reader[4].ToString()   );
      rdrData.BenzPumpa  =                             reader[5].ToString()    ;
      rdrData.KmOnPumpa  = ZXC.ValOrZero_Int          (reader[6].ToString()   );
      rdrData.BenzLitara = ZXC.ValOrZero_Decimal      (reader[7].ToString(), 2);

      raster_rec = rdrData;
   }

   #endregion FillFrom PNRraster Reader

   #region LoadTranses

   public override void LoadTranses(XSqlConnection conn, VvDocumentRecord vvDocumentRecord, bool isArhiva)
   {
      Mixer mixer_rec = (Mixer)vvDocumentRecord;

      if(mixer_rec.Transes == null) mixer_rec.Transes = new List<Xtrans>();
      else                          mixer_rec.Transes.Clear();

      LoadGenericTransesList<Xtrans>(conn, mixer_rec.Transes, mixer_rec.RecID, isArhiva);
   
      // ==================================================================================== 

      if(mixer_rec.Transes2 == null) mixer_rec.Transes2 = new List<Xtrano>();
      else                           mixer_rec.Transes2.Clear();

      LoadGenericTransesList<Xtrano>(conn, mixer_rec.Transes2, mixer_rec.RecID, isArhiva);

      // ==================================================================================== 

      //if(mixer_rec.Transes3 == null) mixer_rec.Transes3 = new List<Xtrano>();
      //else                           mixer_rec.Transes3.Clear();

      //LoadGenericTransesList<Xtrano>(conn, mixer_rec.Transes3, mixer_rec.RecID, isArhiva);
   }

   #endregion LoadTranses

   #region MixerCI struct & InitializeSchemaColumnIndexes()

   public struct MixerCI
   {
      internal int recID;
      internal int addTS;
      internal int modTS;
      internal int addUID;
      internal int modUID;
      internal int lanSrvID;
      internal int lanRecID;

      internal int dokNum;
      internal int dokDate;
      internal int tt;
      internal int ttNum;
      internal int napomena;
      internal int intA;
      internal int dateA;

      internal int kupdobCD    ;
      internal int mtrosCD     ;
      internal int personCD    ;
      internal int intB        ;
      internal int isXxx       ;
      internal int konto       ;
      internal int kupdobTK    ;
      internal int kupdobName  ;
      internal int mtrosTK     ;
      internal int personIme   ;
      internal int personPrezim;
      internal int devName     ;
      internal int strA_40     ;
      internal int strB_128    ;
      internal int strC_32     ;
      internal int strD_32     ;
      internal int strE_256    ;
      internal int strF_64     ;
      internal int strG_40     ;
      internal int dateTimeA   ;
      internal int dateTimeB   ;
      internal int dateB       ;
      internal int moneyA      ;
      internal int moneyB      ;
      internal int moneyC      ;
      internal int strH_32     ;
      internal int projektCD   ;
      internal int v1_tt       ;
      internal int v1_ttNum    ;
      internal int v2_tt       ;
      internal int v2_ttNum    ;
      internal int konto2      ;
      internal int externLink1 ;
      internal int externLink2 ;

      internal int origRecID;
      internal int recVer;
      internal int arAction;
      internal int arTS;
      internal int arUID;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public MixerCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID          = GetSchemaColumnIndex("recID");
      CI.addTS          = GetSchemaColumnIndex("addTS");
      CI.modTS          = GetSchemaColumnIndex("modTS");
      CI.addUID         = GetSchemaColumnIndex("addUID");
      CI.modUID         = GetSchemaColumnIndex("modUID");
      CI.lanSrvID       = GetSchemaColumnIndex("lanSrvID");
      CI.lanRecID       = GetSchemaColumnIndex("lanRecID");
      CI.dokNum         = GetSchemaColumnIndex("dokNum");
      CI.dokDate        = GetSchemaColumnIndex("dokDate");
      CI.tt             = GetSchemaColumnIndex("tt");
      CI.ttNum          = GetSchemaColumnIndex("ttNum");
      CI.napomena       = GetSchemaColumnIndex("napomena");
      CI.intA           = GetSchemaColumnIndex("intA");
      CI.dateA          = GetSchemaColumnIndex("dateA");

      CI.kupdobCD     = GetSchemaColumnIndex("kupdobCD");
      CI.mtrosCD      = GetSchemaColumnIndex("mtrosCD");
      CI.personCD     = GetSchemaColumnIndex("personCD");
      CI.intB         = GetSchemaColumnIndex("intB");
      CI.isXxx        = GetSchemaColumnIndex("isXxx");
      CI.konto        = GetSchemaColumnIndex("konto");
      CI.kupdobTK     = GetSchemaColumnIndex("kupdobTK");
      CI.kupdobName   = GetSchemaColumnIndex("kupdobName");
      CI.mtrosTK      = GetSchemaColumnIndex("mtrosTK");
      CI.personIme    = GetSchemaColumnIndex("personIme");
      CI.personPrezim = GetSchemaColumnIndex("personPrezim");
      CI.devName      = GetSchemaColumnIndex("devName");
      CI.strA_40      = GetSchemaColumnIndex("strA_40");
      CI.strB_128     = GetSchemaColumnIndex("strB_128");
      CI.strC_32      = GetSchemaColumnIndex("strC_32");
      CI.strD_32      = GetSchemaColumnIndex("strD_32");
      CI.strE_256     = GetSchemaColumnIndex("strE_256");
      CI.strF_64      = GetSchemaColumnIndex("strF_64");
      CI.strG_40      = GetSchemaColumnIndex("strG_40");
      CI.dateTimeA    = GetSchemaColumnIndex("dateTimeA");
      CI.dateTimeB    = GetSchemaColumnIndex("dateTimeB");
      CI.dateB        = GetSchemaColumnIndex("dateB");
      CI.moneyA       = GetSchemaColumnIndex("moneyA");
      CI.moneyB       = GetSchemaColumnIndex("moneyB");
      CI.moneyC       = GetSchemaColumnIndex("moneyC");
      CI.strH_32      = GetSchemaColumnIndex("strH_32");
      CI.projektCD    = GetSchemaColumnIndex("projektCD");
      CI.v1_tt        = GetSchemaColumnIndex("v1_tt");
      CI.v1_ttNum     = GetSchemaColumnIndex("v1_ttNum");
      CI.v2_tt        = GetSchemaColumnIndex("v2_tt");
      CI.v2_ttNum     = GetSchemaColumnIndex("v2_ttNum");
      CI.konto2       = GetSchemaColumnIndex("konto2");
      CI.externLink1  = GetSchemaColumnIndex("externLink1");
      CI.externLink2  = GetSchemaColumnIndex("externLink2");

      CI.origRecID      = GetSchemaColumnIndex("origRecID");
      CI.recVer         = GetSchemaColumnIndex("recVer");
      CI.arAction       = GetSchemaColumnIndex("arAction");
      CI.arTS           = GetSchemaColumnIndex("arTS");
      CI.arUID          = GetSchemaColumnIndex("arUID");
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()


   #region AutoAddMixer


   private static void AutoAddMixer(XSqlConnection conn, ref ushort line, Mixer mixer_rec, Xtrans xtrans_rec)
   {
      IVvDao mixerDao  = mixer_rec.VvDao;
      IVvDao xtransDao = xtrans_rec.VvDao;

      ushort linesPerDocumentLIMIT = ushort.MaxValue;

      if(line < linesPerDocumentLIMIT) line++;
      else                             line=1;

      if(line == 1) // new zaglavlje needed 
      {
         line = 1;

         mixer_rec.DokNum = mixerDao.GetNextDokNum(conn, Mixer.recordName);

         if(mixer_rec.TtNum.IsZero()) // !!!: Kad je AutoAddMixer pozvan za neki import onda se preuzima ttNum iz importFile-a, a ako je neki Vektor-ov onda ga izracunaj 
         {
            mixer_rec.TtNum = mixerDao.GetNextTtNum(conn, mixer_rec.VirtualTT, null);
         }

         /* $$$ */ mixerDao.ADDREC(conn, mixer_rec);

         // save new Mixer data for further transes 
         ZXC.MixerRec = mixer_rec.MakeDeepCopy();

      } // new zaglavlje needed 

      xtrans_rec.T_serial   = line;
      xtrans_rec.T_parentID = ZXC.MixerRec.RecID  ;
      xtrans_rec.T_dokNum   = ZXC.MixerRec.DokNum ;
      xtrans_rec.T_dokDate  = ZXC.MixerRec.DokDate;
      xtrans_rec.T_TT       = ZXC.MixerRec.TT     ;
      xtrans_rec.T_ttNum    = ZXC.MixerRec.TtNum  ;

      xtransDao.ADDREC(conn, xtrans_rec, false, false, false, false);
   }

   public static void AutoSetMixer(XSqlConnection conn, ref ushort line, Mixer mixer_rec, Xtrans xtrans_rec)
   {
      AutoAddMixer(conn, ref line, mixer_rec, xtrans_rec);
   }


   #endregion AutoAddMixer


   #region SetMeMixer by tt + ttNum

   public static bool SetMeMixer(XSqlConnection conn, Mixer Mixer_rec, string _tt, uint _ttNum, bool _shouldBeSilent)
   {
      bool success = true;

      using(XSqlCommand cmd = VvSQL.SetMeMixer_Command(conn, _tt, _ttNum, Mixer_rec))
      {
         success = ZXC.MixerDao.ExecuteSingleFillFromDataReader(Mixer_rec, false, cmd, true);
      } // using cmd 

      if(!success && !_shouldBeSilent)
      {
         VvSQL.ReportGeneric_DB_Error("", "Podatak: [" + _tt + "-" + _ttNum.ToString("000000") + "] ne postoji u datoteci [" + Mixer.recordName + ".", System.Windows.Forms.MessageBoxButtons.OK);
      }

      return success;
   }

   public static bool SetMeLast_MixerUgovor_ByKupdobCD(XSqlConnection conn, Mixer mixer_rec, uint kupdobCD, bool _shouldBeSilent)
   {
      bool success = true;

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      DataRowCollection  MixSch = ZXC.MixerDao.TheSchemaTable.Rows;  
      MixerDao.MixerCI   MixCI  = ZXC.MixerDao.CI;

      filterMembers.Add(new VvSqlFilterMember(MixSch[MixCI.tt      ], "theTT" , Mixer.TT_RUG, " = "));
      filterMembers.Add(new VvSqlFilterMember(MixSch[MixCI.kupdobCD], "theKcd", kupdobCD    , " = "));

      success = ZXC.MixerDao.GetLastRecordBySomeOrder(conn, mixer_rec, filterMembers, "tt, ttNum ", _shouldBeSilent, false);

      return success;
   }

   public static bool SetMeLast_FakturRNZ_ByPersonCD(XSqlConnection conn, Faktur faktur_rec, uint personCD, bool _shouldBeSilent)
   {
      bool success = true;

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      DataRowCollection  FakSch = ZXC.FakturDao.TheSchemaTable.Rows;
      FakturDao.FakturCI FakCI  = ZXC.FakturDao.CI;
      DataRowCollection  FexSch = ZXC.FaktExDao.TheSchemaTable.Rows;
      FaktExDao.FaktExCI FexCI  = ZXC.FaktExDao.CI;

      filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.tt      ], "theTT" , Faktur.TT_RNZ, " = "));
      filterMembers.Add(new VvSqlFilterMember(FexSch[FexCI.personCD], "thePcd", personCD     , " = "));

      success = ZXC.FakturDao.GetLastRecordBySomeOrder(conn, faktur_rec, filterMembers, "tt, ttNum ", _shouldBeSilent, true);

      return success;
   }

   #endregion Find Mixer by tt + ttNum

   #region Set_UGO_TtNum_4XtransGroupping

   internal static void Set_UGO_TtNum_4XtransGroupping(XSqlConnection conn, List<Xtrans> xtranses)
   {
      Rtrans UGOrtrans_rec;
      bool UGOrtransFound;

      foreach(Xtrans xtrans_rec in xtranses)
      {
         UGOrtrans_rec = new Rtrans();

         UGOrtransFound = FakturDao.SetMeLastRtransForArtiklAndTT(conn, UGOrtrans_rec, Faktur.TT_UGO, xtrans_rec.T_artiklCD, /*false*/true);

         if(UGOrtransFound)
         {
          //xtrans_rec.T_personCD    = UGOrtrans_rec.T_ttNum   ;
          //xtrans_rec.T_kupdobCD    = UGOrtrans_rec.T_kupdobCD;
            xtrans_rec.R_externTtNum = UGOrtrans_rec.T_ttNum   ;
            xtrans_rec.R_externKCD   = UGOrtrans_rec.T_kupdobCD;
            xtrans_rec.R_externCij   = UGOrtrans_rec.T_cij     ;
         }
         else
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Za artikl [{0}] nema ugovora!", xtrans_rec.T_artiklCD);
         }
      }
   }

   #endregion Set_UGO_TtNum_4XtransGroupping

   public static bool SetMeLast_KDCxtrans_ByKupdobCD_And_KDCname(XSqlConnection conn, Xtrans xtrans_rec, uint kupdobCD, string KDCname)
   {
      bool success = true;

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      DataRowCollection  XtrSch = ZXC.XtransDao.TheSchemaTable.Rows;
      XtransDao.XtransCI XtrCI  = ZXC.XtransDao.CI;

      filterMembers.Add(new VvSqlFilterMember(XtrSch[XtrCI.t_tt          ], "theTT"    , Mixer.TT_KDC, " = "));
      filterMembers.Add(new VvSqlFilterMember(XtrSch[XtrCI.t_ttNum       ], "theTTnum" , kupdobCD    , " = "));
      filterMembers.Add(new VvSqlFilterMember(XtrSch[XtrCI.t_kpdbNameA_50], "kcdName"  , KDCname     , " = "));

      success = ZXC.XtransDao.GetLastRecordBySomeOrder(conn, xtrans_rec, filterMembers, "t_serial ", false, false);

      return success;
   }

}
