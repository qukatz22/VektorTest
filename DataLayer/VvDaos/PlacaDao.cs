using System;
using System.Collections.Generic;
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
using XSqlException  = MySql.Data.MySqlClient.MySqlException;
using XSqlDataReader = MySql.Data.MySqlClient.MySqlDataReader;
using Vektor.DataLayer.DS_Reports;
#endif

public sealed class PlacaDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static PlacaDao instance;

   private PlacaDao(XSqlConnection conn, string dbName) : base(dbName, Placa.recordNameArhiva, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static PlacaDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new PlacaDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTablePlaca

   public static   uint TableVersionStatic { get { return 11; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition(bool isArhiva)
   {
      return (
         /* 00 */  "recID        int(10)     unsigned NOT NULL auto_increment,\n" +
         /* 01 */  "addTS        timestamp                     default '0000-00-00 00:00:00',\n" +
         /* 02 */  "modTS        timestamp                     default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,\n" +
         /* 03 */  "addUID       varchar(16)          NOT NULL default 'XY',\n" +
         /* 04 */  "modUID       varchar(16)          NOT NULL default '',\n" +
        CreateTable_LanSrvID_And_LanRecID_Columns +
         /* 05 */  "dokNum       int(10)    unsigned  NOT NULL,\n" +
         /* 06 */  "dokDate      date                 NOT NULL default '0001-01-01',\n" +
         /* 07 */  "tt           char(3)              NOT NULL default '',\n" +
         /* 08 */  "ttNum        int(10)    unsigned  NOT NULL,\n" +
         /* 09 */  "vrstaObr     char(2)              NOT NULL default '',\n" +
         /* 10 */  "mmyyyy       char(6)              NOT NULL default '',\n" +
         /* 11 */  "fondSati     decimal(6,1)         NOT NULL default '0.0',\n" +
         /* 12 */  "mtros_cd     int(6)      unsigned NOT NULL default '0',\n" +
         /* 13 */  "mtros_tk     char(6)              NOT NULL default '',\n" +
         /* 14 */  "napomena     varchar(80)          NOT NULL default '',\n" +
         /* 15 */  "flagA        tinyint(1) unsigned  NOT NULL default 0,\n" + 
         /* 16 */  "rSm_ID       char(5)              NOT NULL default '',\n" +
         /* 17!*/  "isTrgFondSati tinyint(1) unsigned NOT NULL default 0 ,\n" +   // IsTrgFondSati! 
         /* 49 */  "vrstaJOPPD    char(2)             NOT NULL default '',\n" +
         /* 51 */  "isLocked      tinyint(1) unsigned NOT NULL default 0 ,\n" +  

         /* 17 */  "stpor1       decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 18 */  "stpor2       decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 19 */  "stpor3       decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 20 */  "stpor4       decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 21 */  "osnOdb       decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 22 */  "stMioIz      decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 23 */  "stMioIz2     decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 24 */  "stZdrNa      decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 25 */  "stZorNa      decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 26 */  "stZapNa      decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 27 */  "stZapII      decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 28 */  "minMioOsn    decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 29 */  "maxMioOsn    decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 30 */  "maxPorOsn1   decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 31 */  "maxPorOsn2   decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 32 */  "maxPorOsn3   decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 33 */  "stZpi        decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 34 */  "stOthOlak    decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 35 */  "stDodStaz    decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 36 */  "granBrRad    int(6)     unsigned  NOT NULL default '0'  ,\n" +
         /* 37 */  "stMioNaB1    decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 38 */  "stMioNa2B1   decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 39 */  "stMioNaB2    decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 40 */  "stMioNa2B2   decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 41 */  "stMioNaB3    decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 42 */  "stMioNa2B3   decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 43 */  "stMioNaB4    decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 44 */  "stMioNa2B4   decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 45 */  "prosPlaca    decimal(12,2)        NOT NULL default '0.0',\n" + // notFUSE 
         /* 46 */  "stMioNa2B5   decimal(12,2)        NOT NULL default '0.0',\n" + // FUSE 
         /* 47 */  "stKrizPor1   decimal(12,2)        NOT NULL default '0.0',\n" +
         /*48+2*/  "stKrizPor2   decimal(12,2)        NOT NULL default '0.0',\n" +
         /*50+2*/  "vrKoefBr1    decimal(12,2)        NOT NULL default '0.0',\n" +
         /* ???*/  "stZdrDD      decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 52 */  "mio1Granica1 decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 53 */  "mio1Granica2 decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 54 */  "mio1FiksOlk  decimal(12,2)        NOT NULL default '0.0',\n" +
         /* 55 */  "mio1KoefOlk  decimal(12,2)        NOT NULL default '0.0',\n" +

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

      if(isArhiva) tableName = Placa.recordNameArhiva;
      else         tableName = Placa.recordName;

      string dbName = VvSQL.GetDbNameForThisTableName(tableName);

      // Just few remarcks: 
      // 1. Ovo gore table name i dbName ti nece trebati za druge tablice. Ovdje je to specificno zbog Kupdob--->Prjkt inheritance-a 
      //    a i inace ti tableName i dbName trebaju samo zbog UPDATE clauzule a ne i za 'obicni'ADD COLUMN (UPDATE ti je isao da popunis novododanu kolonu kupdobCD sa unikatnim prjktKupdobCD vrijednostima) 
      // 2. Ovu pretumbaciju sa dodavanjem kupdobCD-a si morao u dvije faze (ver. 2, pa ver. 3) jer nemozes imati UNIQUE index dok je kupdobCD prazan.

      switch(catchingVersion)
      {
         case 2: return ("ADD COLUMN isTrgFondSati tinyint(1) unsigned  NOT NULL default 0   AFTER rSm_ID; \n");

         case 3: return (isArhiva ? "ADD INDEX BYqOrigRecID (origRecID)\n" : "skip_me");

         case 4: return ("MODIFY COLUMN rSm_ID       char(5)              NOT NULL default '';");

         case 5: return ("ADD COLUMN vrstaJOPPD     char(2)              NOT NULL default ''   AFTER isTrgFondSati; \n");

         case 6: return AlterTable_LanSrvID_And_LanRecID_Columns;

         case 7: return ("ADD COLUMN vrKoefBr1    decimal(12,2)        NOT NULL default '0.0'   AFTER stKrizPor2; \n");

         case 8: return ("ADD COLUMN islocked tinyint(1) unsigned  NOT NULL default 0   AFTER vrstaJOPPD; \n");

         case 9: return ("CHANGE COLUMN stMioNaB5 prosPlaca DECIMAL(12,2) NOT NULL DEFAULT '0.00'; \n");

         case 10: return ("ADD COLUMN stZdrDD    decimal(12,2)        NOT NULL default '0.0'   AFTER vrKoefBr1; \n");

         case 11: return ("ADD COLUMN mio1Granica1 decimal(12,2)        NOT NULL default '0.0' AFTER stZdrDD     ,  " +
                          "ADD COLUMN mio1Granica2 decimal(12,2)        NOT NULL default '0.0' AFTER mio1Granica1,  " +
                          "ADD COLUMN mio1FiksOlk  decimal(12,2)        NOT NULL default '0.0' AFTER mio1Granica2,  " +
                          "ADD COLUMN mio1KoefOlk  decimal(12,2)        NOT NULL default '0.0' AFTER mio1FiksOlk ;\n");

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTablePlaca

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      Placa placa = (Placa)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, placa.RecID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, placa.AddTS,    TheSchemaTable.Rows[CI.addTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, placa.ModTS,    TheSchemaTable.Rows[CI.modTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, placa.AddUID,   TheSchemaTable.Rows[CI.addUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, placa.ModUID,   TheSchemaTable.Rows[CI.modUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, placa.LanSrvID, TheSchemaTable.Rows[CI.lanSrvID]);
         VvSQL.CreateCommandParameter(cmd, preffix, placa.LanRecID, TheSchemaTable.Rows[CI.lanRecID]);

      /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.DokNum,        TheSchemaTable.Rows[CI.dokNum]);
      /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.DokDate,       TheSchemaTable.Rows[CI.dokDate]);
      /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.TT,            TheSchemaTable.Rows[CI.tt]);
      /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.TtNum,         TheSchemaTable.Rows[CI.ttNum]);
      /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.VrstaObr,      TheSchemaTable.Rows[CI.vrstaObr]);
      /* 10 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.MMYYYY,        TheSchemaTable.Rows[CI.mmyyyy]);
      /* 11 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.FondSati,      TheSchemaTable.Rows[CI.fondSati]);
      /* 12 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.MtrosCd,       TheSchemaTable.Rows[CI.mtros_cd]);
      /* 13 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.MtrosTk,       TheSchemaTable.Rows[CI.mtros_tk]);
      /* 14 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Napomena,      TheSchemaTable.Rows[CI.napomena]);
      /* 15 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.FlagA,         TheSchemaTable.Rows[CI.flagA]);
      /* 16 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.RSm_ID,        TheSchemaTable.Rows[CI.rSm_ID]);
      /* 17 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.IsTrgFondSati, TheSchemaTable.Rows[CI.isTrgFondSati]);
      /* 49 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.VrstaJOPPD   , TheSchemaTable.Rows[CI.vrstaJOPPD]);
      /* 51 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.IsLocked     , TheSchemaTable.Rows[CI.isLocked  ]);

      /* 17 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StPor1    ,    TheSchemaTable.Rows[CI.stpor1    ]);
      /* 18 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StPor2    ,    TheSchemaTable.Rows[CI.stpor2    ]);
      /* 19 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StPor3    ,    TheSchemaTable.Rows[CI.stpor3    ]);
      /* 20 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StPor4    ,    TheSchemaTable.Rows[CI.stpor4    ]);
      /* 21 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_OsnOdb    ,    TheSchemaTable.Rows[CI.osnOdb    ]);
      /* 22 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StMio1stup,    TheSchemaTable.Rows[CI.stMioIz   ]);
      /* 23 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StMio2stup,    TheSchemaTable.Rows[CI.stMioIz2  ]);
      /* 24 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StZdrNa   ,    TheSchemaTable.Rows[CI.stZdrNa   ]);
      /* 25 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StZorNa   ,    TheSchemaTable.Rows[CI.stZorNa   ]);
      /* 26 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StZapNa   ,    TheSchemaTable.Rows[CI.stZapNa   ]);
      /* 27 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StZapII   ,    TheSchemaTable.Rows[CI.stZapII   ]);
      /* 28 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_MinMioOsn ,    TheSchemaTable.Rows[CI.minMioOsn ]);
      /* 29 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_MaxMioOsn ,    TheSchemaTable.Rows[CI.maxMioOsn ]);
      /* 30 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_MaxPorOsn1,    TheSchemaTable.Rows[CI.maxPorOsn1]);
      /* 31 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_MaxPorOsn2,    TheSchemaTable.Rows[CI.maxPorOsn2]);
      /* 32 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_MaxPorOsn3,    TheSchemaTable.Rows[CI.maxPorOsn3]);
      /* 33 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StZpi     ,    TheSchemaTable.Rows[CI.stZpi     ]);
      /* 34 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StOthOlak ,    TheSchemaTable.Rows[CI.stOthOlak ]);
      /* 35 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StDodStaz ,    TheSchemaTable.Rows[CI.stDodStaz ]);
      /* 36 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_GranBrRad ,    TheSchemaTable.Rows[CI.granBrRad ]);
      /* 37 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StMioNaB1 ,    TheSchemaTable.Rows[CI.stMioNaB1 ]);
      /* 38 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StMioNa2B1,    TheSchemaTable.Rows[CI.stMioNa2B1]);
      /* 39 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StMioNaB2 ,    TheSchemaTable.Rows[CI.stMioNaB2 ]);
      /* 40 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StMioNa2B2,    TheSchemaTable.Rows[CI.stMioNa2B2]);
      /* 41 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StMioNaB3 ,    TheSchemaTable.Rows[CI.stMioNaB3 ]);
      /* 42 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StMioNa2B3,    TheSchemaTable.Rows[CI.stMioNa2B3]);
      /* 43 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StMioNaB4 ,    TheSchemaTable.Rows[CI.stMioNaB4 ]);
      /* 44 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StMioNa2B4,    TheSchemaTable.Rows[CI.stMioNa2B4]);
      /* 45 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_ProsPlaca ,    TheSchemaTable.Rows[CI.prosPlaca ]);
      /* 46 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_OsnDopClUp,    TheSchemaTable.Rows[CI.stMioNa2B5]);
      /* 47 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StKrizPor1,    TheSchemaTable.Rows[CI.stKrizPor1]);
      /* 48 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StKrizPor2,    TheSchemaTable.Rows[CI.stKrizPor2]);
      /* 50 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_VrKoefBr1 ,    TheSchemaTable.Rows[CI.vrKoefBr1 ]);
      /* 51 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_StZdrDD   ,    TheSchemaTable.Rows[CI.stZdrDD   ]);
      /* 52 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_Mio1Granica1,  TheSchemaTable.Rows[CI.mio1Granica1]);
      /* 53 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_Mio1Granica2,  TheSchemaTable.Rows[CI.mio1Granica2]);
      /* 54 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_Mio1FiksOlk,   TheSchemaTable.Rows[CI.mio1FiksOlk ]);
      /* 55 */ VvSQL.CreateCommandParameter(cmd, preffix, placa.Rule_Mio1KoefOlk,   TheSchemaTable.Rows[CI.mio1KoefOlk ]);

         if(isArhiva)
         {
            VvSQL.CreateCommandParameter(cmd, preffix, placa.OrigRecID, TheSchemaTable.Rows[CI.origRecID]);
            VvSQL.CreateCommandParameter(cmd, preffix, placa.RecVer,    TheSchemaTable.Rows[CI.recVer]);
            VvSQL.CreateCommandParameter(cmd, preffix, placa.ArAction,  TheSchemaTable.Rows[CI.arAction]);
            VvSQL.CreateCommandParameter(cmd, preffix, placa.ArTS,      TheSchemaTable.Rows[CI.arTS]);
            VvSQL.CreateCommandParameter(cmd, preffix, placa.ArUID,     TheSchemaTable.Rows[CI.arUID]);
         }
      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva)
   {
      PlacaStruct rdrData = new PlacaStruct();

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
      /* 09 */      rdrData._vrstaObr     = reader.GetString   (CI.vrstaObr);
      /* 10 */      rdrData._mmyyyy       = reader.GetString/*DateTime*/ (CI.mmyyyy);
      /* 11 */      rdrData._fondSati     = reader.GetDecimal  (CI.fondSati);
      /* 12 */      rdrData._mtros_cd     = reader.GetUInt32   (CI.mtros_cd);
      /* 13 */      rdrData._mtros_tk     = reader.GetString   (CI.mtros_tk);
      /* 14 */      rdrData._napomena     = reader.GetString   (CI.napomena);
      /* 15 */      rdrData._flagA        = reader.GetBoolean  (CI.flagA);
      /* 16 */      rdrData._rSm_ID       = reader.GetString   (CI.rSm_ID);
      /* 17 */      rdrData._isTrgFondSati= reader.GetBoolean  (CI.isTrgFondSati);
      /* 49 */      rdrData._vrstaJOPPD   = reader.GetString   (CI.vrstaJOPPD);
      /* 51 */      rdrData._isLocked     = reader.GetBoolean  (CI.isLocked  );

      /* 17 */      rdrData._pRules._stpor1       = reader.GetDecimal(CI.stpor1    );
      /* 18 */      rdrData._pRules._stpor2       = reader.GetDecimal(CI.stpor2    );
      /* 19 */      rdrData._pRules._stpor3       = reader.GetDecimal(CI.stpor3    );
      /* 20 */      rdrData._pRules._stpor4       = reader.GetDecimal(CI.stpor4    );
      /* 21 */      rdrData._pRules._osnOdb       = reader.GetDecimal(CI.osnOdb    );
      /* 22 */      rdrData._pRules._stMio1stup   = reader.GetDecimal(CI.stMioIz   );
      /* 23 */      rdrData._pRules._stMio2stup   = reader.GetDecimal(CI.stMioIz2  );
      /* 24 */      rdrData._pRules._stZdrNa      = reader.GetDecimal(CI.stZdrNa   );
      /* 25 */      rdrData._pRules._stZorNa      = reader.GetDecimal(CI.stZorNa   );
      /* 26 */      rdrData._pRules._stZapNa      = reader.GetDecimal(CI.stZapNa   );
      /* 27 */      rdrData._pRules._stZapII      = reader.GetDecimal(CI.stZapII   );
      /* 28 */      rdrData._pRules._minMioOsn    = reader.GetDecimal(CI.minMioOsn );
      /* 29 */      rdrData._pRules._maxMioOsn    = reader.GetDecimal(CI.maxMioOsn );
      /* 30 */      rdrData._pRules._maxPorOsn1   = reader.GetDecimal(CI.maxPorOsn1);
      /* 31 */      rdrData._pRules._maxPorOsn2   = reader.GetDecimal(CI.maxPorOsn2);
      /* 32 */      rdrData._pRules._maxPorOsn3   = reader.GetDecimal(CI.maxPorOsn3);
      /* 33 */      rdrData._pRules._stZpi        = reader.GetDecimal(CI.stZpi     );
      /* 34 */      rdrData._pRules._stOthOlak    = reader.GetDecimal(CI.stOthOlak );
      /* 35 */      rdrData._pRules._stDodStaz    = reader.GetDecimal(CI.stDodStaz );
      /* 36 */      rdrData._pRules._granBrRad    = reader.GetUInt32 (CI.granBrRad );
      /* 37 */      rdrData._pRules._stMioNaB1    = reader.GetDecimal(CI.stMioNaB1 );
      /* 38 */      rdrData._pRules._stMioNa2B1   = reader.GetDecimal(CI.stMioNa2B1);
      /* 39 */      rdrData._pRules._stMioNaB2    = reader.GetDecimal(CI.stMioNaB2 );
      /* 40 */      rdrData._pRules._stMioNa2B2   = reader.GetDecimal(CI.stMioNa2B2);
      /* 41 */      rdrData._pRules._stMioNaB3    = reader.GetDecimal(CI.stMioNaB3 );
      /* 42 */      rdrData._pRules._stMioNa2B3   = reader.GetDecimal(CI.stMioNa2B3);
      /* 43 */      rdrData._pRules._stMioNaB4    = reader.GetDecimal(CI.stMioNaB4 );
      /* 44 */      rdrData._pRules._stMioNa2B4   = reader.GetDecimal(CI.stMioNa2B4);
      /* 45 */      rdrData._pRules._prosPlaca    = reader.GetDecimal(CI.prosPlaca );
      /* 46 */      rdrData._pRules._stMioNa2B5   = reader.GetDecimal(CI.stMioNa2B5);
      /* 47 */      rdrData._pRules._stKrizPor1   = reader.GetDecimal(CI.stKrizPor1);
      /* 48 */      rdrData._pRules._stKrizPor2   = reader.GetDecimal(CI.stKrizPor2);
      /* 50 */      rdrData._pRules._vrKoefBr1    = reader.GetDecimal(CI.vrKoefBr1 );
      /* 51 */      rdrData._pRules._stZdrDD      = reader.GetDecimal(CI.stZdrDD   );
      /* 52 */      rdrData._pRules._mio1Granica1 = reader.GetDecimal(CI.mio1Granica1);
      /* 53 */      rdrData._pRules._mio1Granica2 = reader.GetDecimal(CI.mio1Granica2);
      /* 54 */      rdrData._pRules._mio1FiksOlk  = reader.GetDecimal(CI.mio1FiksOlk );
      /* 55 */      rdrData._pRules._mio1KoefOlk  = reader.GetDecimal(CI.mio1KoefOlk );

      ((Placa)vvDataRecord).CurrentData = rdrData;

      if(((Placa)vvDataRecord).Transes != null) ((Placa)vvDataRecord).Transes.Clear();
      // 13.06.2013: 
      if(((Placa)vvDataRecord).Transes2 != null) ((Placa)vvDataRecord).Transes2.Clear();
      if(((Placa)vvDataRecord).Transes3 != null) ((Placa)vvDataRecord).Transes3.Clear();

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

   #region FillFromTypedPlacaDataRow

   // TODO: vidi treba li mozda za ubuduce genericki pa overridano                   
   // FillFromTypedPlacaDataRow(VvDataRecord vvDataRecord, SomeTypedDataRow dataRow) 
   // fali li ti tu mozda i onih prvih 5 univerzalinih RecID, AddTS, ModTS, ...      

   public static void FillFromTypedPlacaDataRow(Placa placa_rec, Vektor.DataLayer.DS_Reports.DS_Placa.placaRow placaRow)
   {
      placa_rec.Memset0(0);

      /* 05 */ placa_rec.DokNum     = placaRow.dokNum  ;
      /* 06 */ placa_rec.DokDate    = placaRow.dokDate ; 
      /* 07 */ placa_rec.TT         = placaRow.tt      ;
      /* 08 */ placa_rec.TtNum      = placaRow.ttNum   ;
      /* 09 */ placa_rec.VrstaObr   = placaRow.vrstaObr;
      /* 10 */ placa_rec.MMYYYY     = placaRow.mmyyyy  ;
      /* 11 */ placa_rec.FondSati   = placaRow.fondSati;
      /* 12 */ placa_rec.MtrosCd    = placaRow.mtros_cd;
      /* 13 */ placa_rec.MtrosTk    = placaRow.mtros_tk;
      /* 14 */ placa_rec.Napomena   = placaRow.napomena;
      /* 15 */ placa_rec.FlagA      = Convert.ToBoolean(placaRow.flagA);
      /* 16 */ placa_rec.RSm_ID     = placaRow.rSm_ID  ;
      /* 17 */ placa_rec.IsTrgFondSati = Convert.ToBoolean(placaRow.isTrgFondSati);
      /* 49 */ placa_rec.VrstaJOPPD = placaRow.vrstaJOPPD;
      /* 51 */ placa_rec.IsLocked   = Convert.ToBoolean(placaRow.isLocked);
      
      /* 17 */ placa_rec.Rule_StPor1      = placaRow.stpor1;
      /* 18 */ placa_rec.Rule_StPor2      = placaRow.stpor2;
      /* 19 */ placa_rec.Rule_StPor3      = placaRow.stpor3;
      /* 20 */ placa_rec.Rule_StPor4      = placaRow.stpor4;
      /* 21 */ placa_rec.Rule_OsnOdb      = placaRow.osnOdb;
      /* 22 */ placa_rec.Rule_StMio1stup  = placaRow.stMioIz;
      /* 23 */ placa_rec.Rule_StMio2stup  = placaRow.stMioIz2;
      /* 24 */ placa_rec.Rule_StZdrNa     = placaRow.stZdrNa;
      /* 25 */ placa_rec.Rule_StZorNa     = placaRow.stZorNa;
      /* 26 */ placa_rec.Rule_StZapNa     = placaRow.stZapNa;
      /* 27 */ placa_rec.Rule_StZapII     = placaRow.stZapII;
      /* 28 */ placa_rec.Rule_MinMioOsn   = placaRow.minMioOsn;
      /* 29 */ placa_rec.Rule_MaxMioOsn   = placaRow.maxMioOsn;
      /* 30 */ placa_rec.Rule_MaxPorOsn1  = placaRow.maxPorOsn1;
      /* 31 */ placa_rec.Rule_MaxPorOsn2  = placaRow.maxPorOsn2;
      /* 32 */ placa_rec.Rule_MaxPorOsn3  = placaRow.maxPorOsn3;
      /* 33 */ placa_rec.Rule_StZpi       = placaRow.stZpi;
      /* 34 */ placa_rec.Rule_StOthOlak   = placaRow.stOthOlak;
      /* 35 */ placa_rec.Rule_StDodStaz   = placaRow.stDodStaz;
      /* 36 */ placa_rec.Rule_GranBrRad   = placaRow.granBrRad;
      /* 37 */ placa_rec.Rule_StMioNaB1   = placaRow.stMioNaB1;
      /* 38 */ placa_rec.Rule_StMioNa2B1  = placaRow.stMioNa2B1;
      /* 39 */ placa_rec.Rule_StMioNaB2   = placaRow.stMioNaB2;
      /* 40 */ placa_rec.Rule_StMioNa2B2  = placaRow.stMioNa2B2;
      /* 41 */ placa_rec.Rule_StMioNaB3   = placaRow.stMioNaB3;
      /* 42 */ placa_rec.Rule_StMioNa2B3  = placaRow.stMioNa2B3;
      /* 43 */ placa_rec.Rule_StMioNaB4   = placaRow.stMioNaB4;
      /* 44 */ placa_rec.Rule_StMioNa2B4  = placaRow.stMioNa2B4;
      /* 45 */ placa_rec.Rule_ProsPlaca   = placaRow.prosPlaca ;
      /* 46 */ placa_rec.Rule_OsnDopClUp  = placaRow.stMioNa2B5;
      /* 47 */ placa_rec.Rule_StKrizPor1  = placaRow.stKrizPor1;
      /* 48 */ placa_rec.Rule_StKrizPor2  = placaRow.stKrizPor2;
      /* 50 */ placa_rec.Rule_VrKoefBr1   = placaRow.vrKoefBr1 ;
      /* 51 */ placa_rec.Rule_StZdrDD     = placaRow.stZdrDD   ;
      /* 52 */ placa_rec.Rule_Mio1Granica1= placaRow.mio1Granica1;
      /* 53 */ placa_rec.Rule_Mio1Granica2= placaRow.mio1Granica2;
      /* 54 */ placa_rec.Rule_Mio1FiksOlk = placaRow.mio1FiksOlk ;
      /* 55 */ placa_rec.Rule_Mio1KoefOlk = placaRow.mio1KoefOlk ;
   }

   #endregion FillFromTypedPlacaDataRow

   #region FillTypedDataRowSumResults

// public static void FillTypedDataRowSumResults(Vektor.DataLayer.DS_Reports.DS_Placa.placaRow           placaRow, 
//                                               Vektor.DataLayer.DS_Reports.DS_Placa.IzvjTableDataTable ptransTable)
// public static void FillTypedDataRowSumResults(Vektor.DataLayer.DS_Reports.DS_Placa.placaRow       placaRow,
//                                               Vektor.DataLayer.DS_Reports.DS_Placa.IzvjTableRow[] ptransesOfThisPlaca)
   public static void FillTypedDataRowSumResults(DS_Placa.placaRow                              placaRow,
                                                 EnumerableRowCollection<DS_Placa.IzvjTableRow> ptransesOfThisPlaca)
   {
      /* 01 */   placaRow.S_tBrutoOsn   = ptransesOfThisPlaca.Sum(ptrn => ptrn.t_brutoOsn);
      /* 02 */   placaRow.S_tTopObrok   = ptransesOfThisPlaca.Sum(ptrn => ptrn.t_topObrok);  
      /* 03 */   placaRow.S_tDodBruto   = ptransesOfThisPlaca.Sum(ptrn => ptrn.t_dodBruto);  
      /* 04 */   placaRow.S_tZivotno    = ptransesOfThisPlaca.Sum(ptrn => ptrn.t_zivotno);
      /* 05 */   placaRow.S_tDopZdr     = ptransesOfThisPlaca.Sum(ptrn => ptrn.t_dopZdr);
      /* 06 */   placaRow.S_tDobMIO     = ptransesOfThisPlaca.Sum(ptrn => ptrn.t_dobMIO);
      /* 07 */   placaRow.S_tNetoAdd    = ptransesOfThisPlaca.Sum(ptrn => ptrn.t_netoAdd);
      /* 08 */   placaRow.S_tPrijevoz   = ptransesOfThisPlaca.Sum(ptrn => ptrn.t_prijevoz);
      /* 09 */   placaRow.S_rBruto100   = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Bruto100);
      /* 10 */   placaRow.S_rTheBruto   = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_TheBruto);
      /* 11 */   placaRow.S_rMioOsn     = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_MioOsn);
      /* 12 */   placaRow.S_rMio1stup   = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Mio1stup);
      /* 13 */   placaRow.S_rMio2stup   = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Mio2stup);
      /* 14 */   placaRow.S_rMioAll     = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_MioAll);
      /* 15 */   placaRow.S_rDoprIz     = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_DoprIz);
      /* 16 */   placaRow.S_rOdbitak    = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Odbitak);
      /* 17 */   placaRow.S_rPremije    = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Premije);
      /* 18 */   placaRow.S_rDohodak    = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Dohodak);
      /* 19 */   placaRow.S_rPorOsnAll  = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_PorOsnAll);
      /* 20 */   placaRow.S_rPorOsn1    = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_PorOsn1);
      /* 21 */   placaRow.S_rPorOsn2    = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_PorOsn2);
      /* 22 */   placaRow.S_rPorOsn3    = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_PorOsn3);
      /* 23 */   placaRow.S_rPorOsn4    = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_PorOsn4);
      /* 24 */   placaRow.S_rPor1uk     = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Por1Uk);
      /* 25 */   placaRow.S_rPor2uk     = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Por2Uk);
      /* 26 */   placaRow.S_rPor3uk     = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Por3Uk);
      /* 27 */   placaRow.S_rPor4uk     = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Por4Uk);
      /* 28 */   placaRow.S_rPorezAll   = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_PorezAll);
      /* 29 */   placaRow.S_rPrirez     = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Prirez);
      /* 30 */   placaRow.S_rPorPrirez  = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_PorPrirez);
      /* 31 */   placaRow.S_rNetto      = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Netto);
      /* 32 */   placaRow.S_rObustave   = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Obustave);
      /* 33 */   placaRow.S_r2Pay       = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_2Pay);
      /* 34 */   placaRow.S_rNaRuke     = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_NaRuke);
      /* 35 */   placaRow.S_rZdrNa      = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_ZdrNa);
      /* 36 */   placaRow.S_rZorNa      = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_ZorNa);
      /* 37 */   placaRow.S_rZapNa      = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_ZapNa);
      /* 38 */   placaRow.S_rZapII      = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_ZapII);
      /* 39 */   placaRow.S_rZapAll     = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_ZapAll);
      /* 40 */   placaRow.S_rDoprNa     = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_DoprNa);
      /* 41 */   placaRow.S_rDoprAll    = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_DoprAll);
      /* 42 */   placaRow.S_rMio1stupNa = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Mio1stupNa);
      /* 43 */   placaRow.S_rMio2stupNa = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Mio2stupNa);
      /* 44 */   placaRow.S_rMioAllNa   = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_MioAllNa);
      /* 45 */   placaRow.S_rKrizPorOsn = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_KrizPorOsn);
      /* 46 */   placaRow.S_rKrizPorUk  = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_KrizPorUk);
      /* 47 */   placaRow.S_rSatiR      = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_SatiR);
      /* 48 */   placaRow.S_rSatiB      = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_SatiB);
      /* 49 */   placaRow.S_rZpiUk      = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_ZpiUk);
      /* 50 */   placaRow.S_rDaniZpi    = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_DaniZpi);
      /* 51 */   placaRow.S_rNettoWoAdd = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_NettoWoAdd);
      /* 52 */   placaRow.S_rAHizdatak  = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_AHizdatak);
      /* 53 */   placaRow.S_rNettoAftKrp= ptransesOfThisPlaca.Sum(ptrn => ptrn.R_NettoAftKrp);
      /* 54 */   placaRow.S_rBrtDodNaStaz=ptransesOfThisPlaca.Sum(ptrn => ptrn.R_BrtDodNaStaz);

    //   55 */   placaRow.S_rTheBruto_WoNZ = ptransesOfThisPlaca.Where(ptrn =>  ptrn.t_spc != (byte)Ptrans.SpecEnum.NOVOZAPOSL                                                      ).Sum(ptrn => ptrn.R_TheBruto);
      /* 55 */   placaRow.S_rTheBruto_WoNZ = ptransesOfThisPlaca.Where(ptrn => (ptrn.t_spc != (byte)Ptrans.SpecEnum.NOVOZAPOSL && ptrn.t_spc != (byte)Ptrans.SpecEnum.NOVO_MINMIONE)).Sum(ptrn => ptrn.R_TheBruto);
      
      /* 56 */   placaRow.S_tBrDodPoloz       = ptransesOfThisPlaca.Sum(ptrn => ptrn.t_brDodPoloz      );  
      /* 57 */   placaRow.S_rSatiNeR          = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_SatiNeR         );  
      /* 58 */   placaRow.S_rSatiOnlyRad      = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_SatiOnlyRad     );  
      /* 59 */   placaRow.S_rSatiOnlyRadBruto = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_SatiOnlyRadBruto);  
      /* 60 */   placaRow.S_rPraznikHrsBruto  = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_PraznikHrsBruto );  
      
                 placaRow.S_PIVRnetto  = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_netto_PIVR);  
                 placaRow.S_PIVRbruto  = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_bruto_PIVR);  
                 placaRow.S_PIVRsatiR  = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_satiR_PIVR);  
                 placaRow.S_ONPNnetto  = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_netto_ONPN);  
                 placaRow.S_ONPNbruto  = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_bruto_ONPN);  
                 placaRow.S_ONPNsatiR  = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_satiR_ONPN);
                 placaRow.S_tDopZdr2020= ptransesOfThisPlaca.Sum(ptrn => ptrn.t_dopZdr2020);

                 placaRow.S_PIVRptrCount = ptransesOfThisPlaca.Count(ptrn => ptrn.R_isONPN == 0);
                 placaRow.S_ONPNptrCount = ptransesOfThisPlaca.Count(ptrn => ptrn.R_isONPN == 1);  

      /* 97 */   placaRow.S_rMio1Olk = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Mio1Olk);  
      /* 98 */   placaRow.S_rMio1Osn = ptransesOfThisPlaca.Sum(ptrn => ptrn.R_Mio1Osn);  

   }

   #endregion FillTypedDataRowResults

   #region LoadTranses

   public override void LoadTranses(XSqlConnection conn, VvDocumentRecord vvDocumentRecord, bool isArhiva)
   {
      Placa placa_rec = (Placa)vvDocumentRecord;

      // ==================================================================================== 

      if(placa_rec.Transes == null) placa_rec.Transes = new List<Ptrans>();
      else                          placa_rec.Transes.Clear();

      LoadGenericTransesList<Ptrans>(conn, placa_rec.Transes, placa_rec.RecID, isArhiva);

      // ==================================================================================== 

      if(placa_rec.Transes2 == null) placa_rec.Transes2 = new List<Ptrane>();
      else                           placa_rec.Transes2.Clear();

      LoadGenericTransesList<Ptrane>(conn, placa_rec.Transes2, placa_rec.RecID, isArhiva);

      // ==================================================================================== 

      if(placa_rec.Transes3 == null) placa_rec.Transes3 = new List<Ptrano>();
      else                           placa_rec.Transes3.Clear();

      LoadGenericTransesList<Ptrano>(conn, placa_rec.Transes3, placa_rec.RecID, isArhiva);
   }

   #endregion LoadTranses

   #region PlacaCI struct & InitializeSchemaColumnIndexes()

   public struct PlacaCI
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
      internal int vrstaObr;
      internal int mmyyyy;
      internal int fondSati;
      internal int mtros_cd;
      internal int mtros_tk;
      internal int napomena;
      internal int flagA;
      internal int rSm_ID;
      internal int isTrgFondSati;
      internal int vrstaJOPPD;
      internal int isLocked  ;

      internal int stpor1    ;
      internal int stpor2    ;
      internal int stpor3    ;
      internal int stpor4    ;
      internal int osnOdb    ;
      internal int stMioIz   ;
      internal int stMioIz2  ;
      internal int stZdrNa   ;
      internal int stZorNa   ;
      internal int stZapNa   ;
      internal int stZapII   ;
      internal int minMioOsn ;
      internal int maxMioOsn ;
      internal int maxPorOsn1;
      internal int maxPorOsn2;
      internal int maxPorOsn3;
      internal int stZpi     ;
      internal int stOthOlak ;
      internal int stDodStaz;
      internal int granBrRad;
      internal int stMioNaB1;
      internal int stMioNa2B1;
      internal int stMioNaB2;
      internal int stMioNa2B2;
      internal int stMioNaB3;
      internal int stMioNa2B3;
      internal int stMioNaB4;
      internal int stMioNa2B4;
      internal int prosPlaca;
      internal int stMioNa2B5;
      internal int stKrizPor1;
      internal int stKrizPor2;
      internal int vrKoefBr1 ;
      internal int stZdrDD   ;

      internal int mio1Granica1;
      internal int mio1Granica2;
      internal int mio1FiksOlk ;
      internal int mio1KoefOlk ;

      internal int origRecID;
      internal int recVer;
      internal int arAction;
      internal int arTS;
      internal int arUID;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public PlacaCI CI;

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
      CI.vrstaObr       = GetSchemaColumnIndex("vrstaObr");
      CI.mmyyyy         = GetSchemaColumnIndex("mmyyyy");
      CI.fondSati       = GetSchemaColumnIndex("fondSati");
      CI.mtros_cd       = GetSchemaColumnIndex("mtros_cd");
      CI.mtros_tk       = GetSchemaColumnIndex("mtros_tk");
      CI.napomena       = GetSchemaColumnIndex("napomena");
      CI.flagA          = GetSchemaColumnIndex("flagA");
      CI.rSm_ID         = GetSchemaColumnIndex("rSm_ID");
      CI.isTrgFondSati  = GetSchemaColumnIndex("isTrgFondSati");
      CI.vrstaJOPPD     = GetSchemaColumnIndex("vrstaJOPPD");
      CI.isLocked       = GetSchemaColumnIndex("isLocked");

      CI.stpor1         = GetSchemaColumnIndex("stpor1");
      CI.stpor2         = GetSchemaColumnIndex("stpor2");
      CI.stpor3         = GetSchemaColumnIndex("stpor3");
      CI.stpor4         = GetSchemaColumnIndex("stpor4");
      CI.osnOdb         = GetSchemaColumnIndex("osnOdb");
      CI.stMioIz        = GetSchemaColumnIndex("stMioIz");
      CI.stMioIz2       = GetSchemaColumnIndex("stMioIz2");
      CI.stZdrNa        = GetSchemaColumnIndex("stZdrNa");
      CI.stZorNa        = GetSchemaColumnIndex("stZorNa");
      CI.stZapNa        = GetSchemaColumnIndex("stZapNa");
      CI.stZapII        = GetSchemaColumnIndex("stZapII");
      CI.minMioOsn      = GetSchemaColumnIndex("minMioOsn");
      CI.maxMioOsn      = GetSchemaColumnIndex("maxMioOsn");
      CI.maxPorOsn1     = GetSchemaColumnIndex("maxPorOsn1");
      CI.maxPorOsn2     = GetSchemaColumnIndex("maxPorOsn2");
      CI.maxPorOsn3     = GetSchemaColumnIndex("maxPorOsn3");
      CI.stZpi          = GetSchemaColumnIndex("stZpi");
      CI.stOthOlak      = GetSchemaColumnIndex("stOthOlak");
      CI.stDodStaz      = GetSchemaColumnIndex("stDodStaz");
      CI.granBrRad      = GetSchemaColumnIndex("granBrRad");
      CI.stMioNaB1      = GetSchemaColumnIndex("stMioNaB1");
      CI.stMioNa2B1     = GetSchemaColumnIndex("stMioNa2B1");
      CI.stMioNaB2      = GetSchemaColumnIndex("stMioNaB2");
      CI.stMioNa2B2     = GetSchemaColumnIndex("stMioNa2B2");
      CI.stMioNaB3      = GetSchemaColumnIndex("stMioNaB3");
      CI.stMioNa2B3     = GetSchemaColumnIndex("stMioNa2B3");
      CI.stMioNaB4      = GetSchemaColumnIndex("stMioNaB4");
      CI.stMioNa2B4     = GetSchemaColumnIndex("stMioNa2B4");
      CI.prosPlaca      = GetSchemaColumnIndex("prosPlaca");
      CI.stMioNa2B5     = GetSchemaColumnIndex("stMioNa2B5");
      CI.stKrizPor1     = GetSchemaColumnIndex("stKrizPor1");
      CI.stKrizPor2     = GetSchemaColumnIndex("stKrizPor2");
      CI.vrKoefBr1      = GetSchemaColumnIndex("vrKoefBr1" );
      CI.stZdrDD        = GetSchemaColumnIndex("stZdrDD");

      CI.mio1Granica1   = GetSchemaColumnIndex("mio1Granica1"); 
      CI.mio1Granica2   = GetSchemaColumnIndex("mio1Granica2"); 
      CI.mio1FiksOlk    = GetSchemaColumnIndex("mio1FiksOlk");
      CI.mio1KoefOlk    = GetSchemaColumnIndex("mio1KoefOlk");

      CI.origRecID      = GetSchemaColumnIndex("origRecID");
      CI.recVer         = GetSchemaColumnIndex("recVer");
      CI.arAction       = GetSchemaColumnIndex("arAction");
      CI.arTS           = GetSchemaColumnIndex("arTS");
      CI.arUID          = GetSchemaColumnIndex("arUID");
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()



   #region GetNextRS_ID

   public uint GetNextRS_ID(XSqlConnection conn, string wantedTT, string wantedMMYYYY)
   {
      Placa placa_rec = new Placa();
      uint  biggestSoFar, nextRS_ID;
      bool  OK;

      ZXC.DbNavigationRestrictor ttRestrictor = new ZXC.DbNavigationRestrictor(Placa.tt_colName, new string[] { wantedTT });

      OK = FrsPrvNxtLst_REC(conn, placa_rec, VvSQL.DBNavigActionType.LST, Placa.sorterTtNum, false, ttRestrictor, ZXC.DbNavigationRestrictor.Empty, ZXC.DbNavigationRestrictor.Empty);

      if(!OK) // ovo je, dakle, prvi dokument u fajlu za wantedTT 
      {
         ZXC.VvDataBaseInfo dbInfo    = ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage;
         uint               yearDigit = uint.Parse(dbInfo.ProjectYear_LastDigit);

         uint yearBaseNum, yearDigitMultiplikator, ttBaseMultiplikator, ttBaseNum;

         //if(ZXC.CURR_prjkt_rec.IsObrt) yearDigitMultiplikator =  100; nema od 1.1.2010.
         yearDigitMultiplikator = 1000;

         ttBaseMultiplikator = yearDigitMultiplikator / 100; // 100 / 100 = 1 za obrte, 1000 / 100 = 10 za firme 

         yearBaseNum = yearDigit * yearDigitMultiplikator; // npr 9 * 1000 = 9000 - firme 
                                                           //     9 *  100 =  900 - obrti 
         switch(wantedTT)
         {
            case Placa.TT_REDOVANRAD  : ttBaseNum =  0; break;
            case Placa.TT_AUTORHONOR  : 
            case Placa.TT_AUTORHONUMJ : 
            case Placa.TT_AHSAMOSTUMJ : 
            case Placa.TT_SEZZAPPOLJOP: 
            case Placa.TT_IDD_KOLONA_4: ttBaseNum = 20; break;
            case Placa.TT_UGOVORODJELU: ttBaseNum = 40; break;
            case Placa.TT_PODUZETPLACA: ttBaseNum = 60; break;
            case Placa.TT_NADZORODBOR : ttBaseNum = 80; break;

            case Placa.TT_NEOPOREZPRIM: ttBaseNum = 00; break;

            default: throw new Exception("TT [" + wantedTT + "] nepoznat u GetNextRS_ID()!");
         }

         return yearBaseNum + (ttBaseNum * ttBaseMultiplikator); // npr 9000 + (20 * 10) = 9000 + 200 = 9200 (prvi AutHonorar u godini a za FIRMU 
      }                                                          // npr  900 + (20 *  1) =  900 +  20 =  920 (prvi AutHonorar u godini a za OBRT  

      // =============== here we go! Dakle nije prvi u godini nego samo treba inkrementirati (eventualno ako je razliciti MMYYYY) 

      biggestSoFar = ZXC.ValOrZero_UInt(placa_rec.RSm_ID);

      if(wantedMMYYYY == placa_rec.MMYYYY) // ovo je, dakle, konsekutivni dokument za ISTI RSm Obrazac (vise dokumenta cini jedan RSm Obrazac) 
      {
         nextRS_ID = biggestSoFar;
      }
      else
      {
         nextRS_ID = biggestSoFar + 1;
      }

      return (nextRS_ID);
   }

   #endregion GetNextRS_ID

   #region Set_IMPORT_OFFIX_Columns

   //  //____ Specifics 2 start ______________________________________________________

   //fprintf(device, "%s\t", placa_rec[0].p_br);
   //fprintf(device, "%s\t", GetMySqlDate(placa_rec[0].p_date));
   //fprintf(device, "%s\t", placa_rec[0].p_napomena);
   //fprintf(device, "%s\t", placa_rec[0].p_add_uid);
   //fprintf(device, "%s\t", GetMySqlDate(placa_rec[0].p_date_UnDok));
   //fprintf(device, "%s\t", placa_rec[0].p_edit_uid);
   //fprintf(device, "%s\t", placa_rec[0].p_date_edit[0] ? GetMySqlDate(placa_rec[0].p_date_edit) : GetMySqlDate(placa_rec[0].p_date_UnDok));
   //fprintf(device, "%s\t", placa_rec[0].p_mmyy);		
   //fprintf(device, "%s\t", placa_rec[0].p_evr);	// flagA	
   //fprintf(device, "%s\t", placa_rec[0].p_vu); // vrstaObr		
   //fprintf(device, "%.2lf\t", placa_rec[0].p_ukRSdaTbl);		// fondSati 
   //fprintf(device, "%.2lf\t", placa_rec[0].p_osnOdbitak);	// osnOdb 
   //fprintf(device, "%.2lf\t", placa_rec[0].p_stPor1);		// stpor1 
   //fprintf(device, "%.2lf\t", placa_rec[0].p_stPor2);		// stpor2 
   //fprintf(device, "%.2lf\t", placa_rec[0].p_stPor3);		// stpor3 
   //fprintf(device, "%.2lf\t", placa_rec[0].p_stPor4);		// stpor4 
   //fprintf(device, "%.2lf\t", placa_rec[0].p_stMioIz_I); // stMioIz 
   //fprintf(device, "%.2lf\t", placa_rec[0].p_stMioIz_II);// stMioIz2 
   //fprintf(device, "%.2lf\t", placa_rec[0].p_stZdrNa);	// stZdrNa 
   //fprintf(device, "%.2lf\t", placa_rec[0].p_stZorNa);	// stZorNa 
   //fprintf(device, "%.2lf\t", placa_rec[0].p_stZapNa);	// stZapNa 
   //fprintf(device, "%.2lf\t", placa_rec[0].p_stZapII);	// stZapII 
   //fprintf(device, "%s\t", getRS_ID(placa_rec[0].p_br, placa_rec[0].p_mmyy, placa_rec[0].p_napomena, 0));	// rSm_ID
   //fprintf(device, "\n");

   //  //____ Specifics 2 end   ______________________________________________________

   public override string Set_IMPORT_OFFIX_Columns()
   {
      return

         "(" + 

         "dokNum  , " + //fprintf(device, "%s\t", placa_rec[0].p_br);
         "dokDate , " + //fprintf(device, "%s\t", GetMySqlDate(placa_rec[0].p_date));
         "napomena, " + //fprintf(device, "%s\t", placa_rec[0].p_napomena);
         "addUID  , " + //fprintf(device, "%s\t", placa_rec[0].p_add_uid);
         "addTS   , " + //fprintf(device, "%s\t", GetMySqlDate(placa_rec[0].p_date_UnDok));
         "modUID  , " + //fprintf(device, "%s\t", placa_rec[0].p_edit_uid);
         "modTS   , " + //fprintf(device, "%s\t", placa_rec[0].p_date_edit[0] ? GetMySqlDate(placa_rec[0].p_date_edit) : GetMySqlDate(placa_rec[0].p_date_UnDok));
         "mmyyyy  , " + //fprintf(device, "%s\t", placa_rec[0].p_mmyy);		
         "flagA   , " + //fprintf(device, "%s\t", placa_rec[0].p_evr);	// flagA	
         "vrstaObr, " + //fprintf(device, "%s\t", placa_rec[0].p_vu); // vrstaObr		
         "fondSati, " + //fprintf(device, "%.2lf\t", placa_rec[0].p_ukRSdaTbl);		// fondSati 
         "osnOdb  , " + //fprintf(device, "%.2lf\t", placa_rec[0].p_osnOdbitak);	// osnOdb 
         "stpor1  , " + //fprintf(device, "%.2lf\t", placa_rec[0].p_stPor1);		// stpor1 
         "stpor2  , " + //fprintf(device, "%.2lf\t", placa_rec[0].p_stPor2);		// stpor2 
         "stpor3  , " + //fprintf(device, "%.2lf\t", placa_rec[0].p_stPor3);		// stpor3 
         "stpor4  , " + //fprintf(device, "%.2lf\t", placa_rec[0].p_stPor4);		// stpor4 
         "stMioIz , " + //fprintf(device, "%.2lf\t", placa_rec[0].p_stMioIz_I); // stMioIz 
         "stMioIz2, " + //fprintf(device, "%.2lf\t", placa_rec[0].p_stMioIz_II);// stMioIz2 
         "stZdrNa , " + //fprintf(device, "%.2lf\t", placa_rec[0].p_stZdrNa);	// stZdrNa 
         "stZorNa , " + //fprintf(device, "%.2lf\t", placa_rec[0].p_stZorNa);	// stZorNa 
         "stZapNa , " + //fprintf(device, "%.2lf\t", placa_rec[0].p_stZapNa);	// stZapNa 
         "stZapII , " + //fprintf(device, "%.2lf\t", placa_rec[0].p_stZapII);	// stZapII 
         "rSm_ID    " + //fprintf(device, "%.2lf\t", getRS_ID(placa_rec[0].p_br, placa_rec[0].p_mmyy, placa_rec[0].p_napomena, 0));	// rSm_ID
         
         ")"    + "\n" +

         "SET " + "\n" +

         "tt = 'RR', " + "\n" +

         "minMioOsn = IF(mmyyyy  = '1209',  2611.00,  2700.60), " + "\n" +
         "maxMioOsn = IF(mmyyyy  = '1209', 44760.00, 46296.00), " + "\n" +
         "maxPorOsn1=  3600.00, " + "\n" +
         "maxPorOsn2=  9000.00, " + "\n" +
         "maxPorOsn3= 25200.00, " + "\n" +
         "stZpi     =    20.00, " + "\n" +
         "stOthOlak =    30.00, " + "\n" +
         "stDodStaz =     0.50, " + "\n" +
         "granBrRad =    20   , " + "\n" +
         "stKrizPor1=     2   , " + "\n" +
         "stKrizPor2=     4   , " + "\n" +

         
         "recID = dokNum + 1, " + "\n" + // zbog obracuna br 0. a RecId nemre biti 0!
         "ttNum = dokNum  ";
   }

   #endregion Set_IMPORT_OFFIX_Columns

   #region COPY_DECEMBAR_PLACA_TABLE content to another table

   public bool COPY_DECEMBAR_PLACA_TABLE(XSqlConnection conn, string destDbName, string srcDbName, out int nora)
   {
      bool   success = true;

      nora = -1;

      ZXC.sqlErrNo = 0;

      using(XSqlCommand cmd = VvSQL.COPY_DECEMBAR_PLACA_TABLE_Command(conn, Placa.recordName, destDbName, srcDbName))
      {
         try                     { nora = cmd.ExecuteNonQuery(); }
         catch(XSqlException ex) { success = false; VvSQL.ReportSqlError("COPY_TABLE " + "Decembar Placa", ex, System.Windows.Forms.MessageBoxButtons.OK); ZXC.sqlErrNo = ex.Number; }
         catch(Exception ex)     { success = false; System.Windows.Forms.MessageBox.Show(ex.Message); }
      }
      if(!success) return false;

      using(XSqlCommand cmd = VvSQL.COPY_DECEMBAR_PLACA_TABLE_Command(conn, Ptrans.recordName, destDbName, srcDbName))
      {
         try                     {        cmd.ExecuteNonQuery(); }
         catch(XSqlException ex) { success = false; VvSQL.ReportSqlError("COPY_TABLE " + "Decembar Ptrans", ex, System.Windows.Forms.MessageBoxButtons.OK); ZXC.sqlErrNo = ex.Number; }
         catch(Exception ex)     { success = false; System.Windows.Forms.MessageBox.Show(ex.Message); }
      }
      using(XSqlCommand cmd = VvSQL.COPY_DECEMBAR_PLACA_TABLE_Command(conn, Ptrane.recordName, destDbName, srcDbName))
      {
         try                     {        cmd.ExecuteNonQuery(); }
         catch(XSqlException ex) { success = false; VvSQL.ReportSqlError("COPY_TABLE " + "Decembar Ptrane", ex, System.Windows.Forms.MessageBoxButtons.OK); ZXC.sqlErrNo = ex.Number; }
         catch(Exception ex)     { success = false; System.Windows.Forms.MessageBox.Show(ex.Message); }
      }
      using(XSqlCommand cmd = VvSQL.COPY_DECEMBAR_PLACA_TABLE_Command(conn, Ptrano.recordName, destDbName, srcDbName))
      {
         try                     {        cmd.ExecuteNonQuery(); }
         catch(XSqlException ex) { success = false; VvSQL.ReportSqlError("COPY_TABLE " + "Decembar Ptrano", ex, System.Windows.Forms.MessageBoxButtons.OK); ZXC.sqlErrNo = ex.Number; }
         catch(Exception ex)     { success = false; System.Windows.Forms.MessageBox.Show(ex.Message); }
      }
      return (success);
   }

   #endregion COPY_DECEMBAR_PLACA_TABLE

}
