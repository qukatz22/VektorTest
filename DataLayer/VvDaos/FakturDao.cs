using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;

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
using System.Data.SqlTypes;
using System.Threading.Tasks;
using System.Windows.Forms;
#endif

public sealed class FakturDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static FakturDao instance;

   private FakturDao(XSqlConnection conn, string dbName) : base(dbName, Faktur.recordNameArhiva, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static FakturDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new FakturDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableFaktur

   public static   uint TableVersionStatic { get { return 14; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition(bool isArhiva)
   {
      return (
         /* 00 */  "recID        int(10)     unsigned NOT NULL auto_increment,\n" +
         /* 01 */  "addTS        timestamp                     default '0000-00-00 00:00:00',\n" +
         /* 02 */  "modTS        timestamp                     default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,\n" +
         /* 03 */  "addUID       varchar(16)          NOT NULL default 'XY'        ,\n" +
         /* 04 */  "modUID       varchar(16)          NOT NULL default ''          ,\n" +
        CreateTable_LanSrvID_And_LanRecID_Columns +
         /* 05 */  "dokNum       int(10)    unsigned  NOT NULL                     ,\n" +
         /* 06 */  "dokDate      datetime             NOT NULL default '0001-01-01 00:00:00',\n" +
         /* 07 */  "tt           char(3)              NOT NULL default ''          ,\n" +
         /* 18 */  "ttNum        int(10)    unsigned  NOT NULL                     ,\n" +
         /* 19 */  "ttSort       tinyint(4)           NOT NULL default '0'         ,\n" +
         /* 10 */  "skladCD      varchar(6)           NOT NULL default ''          ,\n" +
         /* 11 */  "skladCD2     varchar(6)           NOT NULL default ''          ,\n" +
         /* 12 */  "vezniDok     varchar(64)          NOT NULL default ''          ,\n" +
         /* 13 */  "napomena     varchar(256)         NOT NULL default ''          ,\n" +
         /* 14 */  "opis         varchar(1024)        NOT NULL default ''          ,\n" +
         /* 15 */  "konto        varchar(8)           NOT NULL default ''          ,\n" +
         /* 16 */  "projektCD    varchar(16)          NOT NULL default ''          ,\n" +
         /* 17 */  "v1_tt        char(3)              NOT NULL default ''          ,\n" +
         /* 18 */  "v1_ttNum     int(10)    unsigned  NOT NULL                     ,\n" +
         /* 19 */  "v2_tt        char(3)              NOT NULL default ''          ,\n" +
         /* 20 */  "v2_ttNum     int(10)    unsigned  NOT NULL                     ,\n" +
         /* 21 */  "s_ukKC       decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /* 22 */  "s_ukK        decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /* 23 */  "s_trnCount   int(6)     unsigned  NOT NULL default 0           ,\n" +
         /* 24 */  "osobaX       varchar(32)          NOT NULL default ''          ,\n" +
         /* 25 */  "dokDate2     datetime             NOT NULL default '0001-01-01 00:00:00',\n" +
         /* 26 */  "s_ukK2       decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /* 27 */  "decimal01    decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /* 28 */  "decimal02    decimal(12,4)        NOT NULL default '0.00'      ,\n" +

         CreateTable_ArhivaExtensionDefinition(isArhiva) +

 /* haodafakšudajnou kajćemiovo */      "PRIMARY KEY      (recID)                  ,\n" +
          (isArhiva ? "" : "UNIQUE ") + "KEY BY_TTNUM     (          ttSort, ttNum),\n" +
          (isArhiva ? "" : "UNIQUE ") + "KEY BY_TTDOKDATE (dokDate , ttSort, ttNum) \n" +

          // !!! faktur_ar jos ima ByOrigRecID INDEX koji radis rucno svake godine !!!

          (isArhiva ? ", KEY BYqOrigRecID (origRecID)\n" : "")

         );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Faktur.recordNameArhiva;
      else         tableName = Faktur.recordName;

      switch(catchingVersion)
      {
         case 2: return ("MODIFY COLUMN napomena     varchar(128)         NOT NULL default '', " +
                         "MODIFY COLUMN opis         varchar(400)         NOT NULL default ''  ");
         //case 3: return ("ADD INDEX BY_DOKNUM USING BTREE(dokNum);\n");

         case 3: return ("MODIFY COLUMN opis         varchar(1024)        NOT NULL default ''  ");

         case 4: return ("ADD COLUMN osobaX          varchar(32)          NOT NULL default '' AFTER s_trnCount;\n");

         case 5: return ("MODIFY COLUMN napomena     varchar(256)         NOT NULL default ''  ");

         case 6: return ("MODIFY COLUMN dokDate      datetime             NOT NULL default '0001-01-01 00:00:00'  ");

         case 7: return (isArhiva ? "ADD INDEX BYqOrigRecID (origRecID)\n" : "skip_me");

         case 8: return ("MODIFY COLUMN konto        varchar(8)           NOT NULL default ''  ");

         case 9: return AlterTable_LanSrvID_And_LanRecID_Columns;

         case 10: return ("ADD COLUMN dokDate2  datetime      NOT NULL default '0001-01-01 00:00:00' AFTER osobaX;  \n");
         case 11: return ("ADD COLUMN s_ukK2    decimal(12,4) NOT NULL default '0.00'                AFTER dokDate2;\n");
         case 12: return ("MODIFY COLUMN vezniDok     varchar(64)          NOT NULL default ''  ");
         case 13: return ("ADD COLUMN decimal01 decimal(12,4) NOT NULL default '0.00'                AFTER s_ukK2,   \n" +
                          "ADD COLUMN decimal02 decimal(12,4) NOT NULL default '0.00'                AFTER decimal01;\n");

         case 14: return ("MODIFY COLUMN ttSort       tinyint(4)           NOT NULL default '0'  ");

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableFaktur

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      Faktur faktur = (Faktur)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, faktur.RecID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, faktur.AddTS,    TheSchemaTable.Rows[CI.addTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, faktur.ModTS,    TheSchemaTable.Rows[CI.modTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, faktur.AddUID,   TheSchemaTable.Rows[CI.addUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, faktur.ModUID,   TheSchemaTable.Rows[CI.modUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, faktur.LanSrvID, TheSchemaTable.Rows[CI.lanSrvID]);
         VvSQL.CreateCommandParameter(cmd, preffix, faktur.LanRecID, TheSchemaTable.Rows[CI.lanRecID]);

      /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.DokNum      , TheSchemaTable.Rows[CI.dokNum      ]);
      /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.DokDate     , TheSchemaTable.Rows[CI.dokDate     ]);
      /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.TT          , TheSchemaTable.Rows[CI.tt          ]);
      /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.TtNum       , TheSchemaTable.Rows[CI.ttNum       ]);
      /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.TtSort      , TheSchemaTable.Rows[CI.ttSort      ]);
      /* 00 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.SkladCD     , TheSchemaTable.Rows[CI.skladCD     ]);
      /* 11 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.SkladCD2    , TheSchemaTable.Rows[CI.skladCD2    ]);
      /* 12 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.VezniDok    , TheSchemaTable.Rows[CI.vezniDok    ]);
      /* 13 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.Napomena    , TheSchemaTable.Rows[CI.napomena    ]);
      /* 14 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.Opis        , TheSchemaTable.Rows[CI.opis        ]);
      /* 15 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.Konto       , TheSchemaTable.Rows[CI.konto       ]);
      /* 16 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.ProjektCD   , TheSchemaTable.Rows[CI.projektCD   ]);
      /* 17 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.V1_tt       , TheSchemaTable.Rows[CI.v1_tt       ]);
      /* 18 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.V1_ttNum    , TheSchemaTable.Rows[CI.v1_ttNum    ]);
      /* 19 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.V2_tt       , TheSchemaTable.Rows[CI.v2_tt       ]);
      /* 20 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.V2_ttNum    , TheSchemaTable.Rows[CI.v2_ttNum    ]);
      /* 21 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.S_ukKC      , TheSchemaTable.Rows[CI.s_ukKC      ]);
      /* 22 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.S_ukK       , TheSchemaTable.Rows[CI.s_ukK       ]);
      /* 23 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.S_ukTrnCount, TheSchemaTable.Rows[CI.s_trnCount  ]);
      /* 24 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.OsobaX      , TheSchemaTable.Rows[CI.osobaX      ]);
      /* 25 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.DokDate2    , TheSchemaTable.Rows[CI.dokDate2    ]);
      /* 26 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.S_ukK2      , TheSchemaTable.Rows[CI.s_ukK2      ]);
      /* 27 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.Decimal01   , TheSchemaTable.Rows[CI.decimal01   ]);
      /* 28 */ VvSQL.CreateCommandParameter(cmd, preffix, faktur.Decimal02   , TheSchemaTable.Rows[CI.decimal02   ]);

         if(isArhiva)
         {
            VvSQL.CreateCommandParameter(cmd, preffix, faktur.OrigRecID, TheSchemaTable.Rows[CI.origRecID]);
            VvSQL.CreateCommandParameter(cmd, preffix, faktur.RecVer,    TheSchemaTable.Rows[CI.recVer]);
            VvSQL.CreateCommandParameter(cmd, preffix, faktur.ArAction,  TheSchemaTable.Rows[CI.arAction]);
            VvSQL.CreateCommandParameter(cmd, preffix, faktur.ArTS,      TheSchemaTable.Rows[CI.arTS]);
            VvSQL.CreateCommandParameter(cmd, preffix, faktur.ArUID,     TheSchemaTable.Rows[CI.arUID]);
         }
      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva)
   {
      FakturStruct rdrData = new FakturStruct();

      rdrData._recID     = reader.GetUInt32  (CI.recID);
      rdrData._addTS     = reader.GetDateTime(CI.addTS);
      rdrData._modTS     = reader.GetDateTime(CI.modTS);
      rdrData._addUID    = reader.GetString  (CI.addUID);
      rdrData._modUID    = reader.GetString  (CI.modUID);
      rdrData._lanSrvID  = reader.GetUInt32  (CI.lanSrvID);
      rdrData._lanRecID  = reader.GetUInt32  (CI.lanRecID);

      /* 05 */      rdrData._dokNum                = reader.GetUInt32   (CI.dokNum);
      /* 06 */      rdrData._dokDate               = reader.GetDateTime (CI.dokDate);
      /* 07 */      rdrData._tt                    = reader.GetString   (CI.tt);
      /* 08 */      rdrData._ttNum                 = reader.GetUInt32   (CI.ttNum);

      /* 09 */      rdrData._ttSort       = reader.GetInt16    (CI.ttSort      );
      /* 00 */      rdrData._skladCD      = reader.GetString   (CI.skladCD     );
      /* 11 */      rdrData._skladCD2     = reader.GetString   (CI.skladCD2    );
      /* 12 */      rdrData._vezniDok     = reader.GetString   (CI.vezniDok    );
      /* 13 */      rdrData._napomena     = reader.GetString   (CI.napomena    );
      /* 14 */      rdrData._opis         = reader.GetString   (CI.opis        );
      /* 15 */      rdrData._konto        = reader.GetString   (CI.konto       );
      /* 16 */      rdrData._projektCD    = reader.GetString   (CI.projektCD   );
      /* 17 */      rdrData._v1_tt        = reader.GetString   (CI.v1_tt       );
      /* 18 */      rdrData._v1_ttNum     = reader.GetUInt32   (CI.v1_ttNum    );
      /* 19 */      rdrData._v2_tt        = reader.GetString   (CI.v2_tt       );
      /* 20 */      rdrData._v2_ttNum     = reader.GetUInt32   (CI.v2_ttNum    );
      /* 21 */      rdrData._s_ukKC       = reader.GetDecimal  (CI.s_ukKC      );
      /* 22 */      rdrData._s_ukK        = reader.GetDecimal  (CI.s_ukK       );
      /* 23 */      rdrData._s_trnCount   = reader.GetUInt32   (CI.s_trnCount  );
      /* 24 */      rdrData._osobaX       = reader.GetString   (CI.osobaX      );
      /* 25 */      rdrData._dokDate2     = reader.GetDateTime (CI.dokDate2    );
      /* 26 */      rdrData._s_ukK2       = reader.GetDecimal  (CI.s_ukK2      );
      /* 27 */      rdrData._decimal01    = reader.GetDecimal  (CI.decimal01   );
      /* 28 */      rdrData._decimal02    = reader.GetDecimal  (CI.decimal02   );

      ((Faktur)vvDataRecord).CurrentData = rdrData;

      if(((Faktur)vvDataRecord).Transes != null) ((Faktur)vvDataRecord).Transes.Clear();
      // 13.06.2013: 
      if(((Faktur)vvDataRecord).Transes2 != null) ((Faktur)vvDataRecord).Transes2.Clear();
      if(((Faktur)vvDataRecord).Transes3 != null) ((Faktur)vvDataRecord).Transes3.Clear();

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

      #region Kune Backup Values

      ((Faktur)vvDataRecord).Skn_ukKC   = ((Faktur)vvDataRecord).S_ukKC  ;

      #endregion Kune Backup Values

      return;
   }

   #region FillFromSumOnlyDataReader

   // 26.05.2017:                                                                                                          
   // kada bi uplata bila R2 uplata, tada bi                                                                               
   // negativne uplate (storna, odobrenja i sl) dolazile u pozitivnom (tj. apsolutnom) iznosu, a trebaju biti negativne    
 //internal static string ftrUplata = " ABS(t_dug-t_pot) ";
   internal static string ftrUplata = " CASE WHEN t_dug<0 THEN t_dug WHEN t_pot<0 THEN t_pot ELSE ABS(t_dug-t_pot) END ";

   public static string ColumnListFromSumOnlyDataReader(bool isIRMgrouping, string preffix, bool isForPDV)
   {
      string columnList;

      columnList =

         (isIRMgrouping ?   "tt        , \n" : "")               +
         (isIRMgrouping ?   "ttSort    , \n" : "")               +
         (isIRMgrouping ?   "ttNum     , \n" : "")               +
         (isIRMgrouping ?   "dokDate   , \n" : "")               +
         (isIRMgrouping ?   "nacPlac   , \n" : "")               +
         (isIRMgrouping ?   "kupdobName, \n" : "")               +
         (isIRMgrouping ?   "kdOib     , \n" : "")               +
         (isIRMgrouping ?   "vezniDok  , \n" : "")               +
         (isIRMgrouping ?   "kdMjesto  , \n" : "")               +
         // 18.07.2019: 
       //(isIRMgrouping ?   "pdvDate   , \n" : "")               +
         (isIRMgrouping ? "IF(pdvR12 != 2, pdvDate, N.t_dokDate) as pdvDate, \n" : "")               +

         /* 00 */ preffix + "SUM(s_ukK      ) as s_ukK      , \n" +

         /* 01 */ preffix + "SUM(IF(pdvR12 != 2, s_ukKC     , s_ukKC      * " + ftrUplata + " / s_ukKCRMP)) as s_ukKC     , \n" +
         /* 02 */ preffix + "SUM(IF(pdvR12 != 2, s_ukKCRMP  , s_ukKCRMP   * " + ftrUplata + " / s_ukKCRMP)) as s_ukKCRMP  , \n" +
         /* 03 */ preffix + "SUM(IF(pdvR12 != 2, s_ukKCRM   , s_ukKCRM    * " + ftrUplata + " / s_ukKCRMP)) as s_ukKCRM   , \n" +
         /* 04 */ preffix + "SUM(IF(pdvR12 != 2, s_ukKCR    , s_ukKCR     * " + ftrUplata + " / s_ukKCRMP)) as s_ukKCR    , \n" +
         /* 05 */ preffix + "SUM(IF(pdvR12 != 2, s_ukRbt1   , s_ukRbt1    * " + ftrUplata + " / s_ukKCRMP)) as s_ukRbt1   , \n" +
         /* 06 */ preffix + "SUM(IF(pdvR12 != 2, s_ukRbt2   , s_ukRbt2    * " + ftrUplata + " / s_ukKCRMP)) as s_ukRbt2   , \n" +
         /* 07 */ preffix + "SUM(IF(pdvR12 != 2, s_ukZavisni, s_ukZavisni * " + ftrUplata + " / s_ukKCRMP)) as s_ukZavisn , \n" +
         /* 08 */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv23m , s_ukPdv23m  * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv23m , \n" +
         /* 09 */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv23n , s_ukPdv23n  * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv23n , \n" +
         /* 10 */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv22m , s_ukPdv22m  * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv22m , \n" +
         /* 11 */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv22n , s_ukPdv22n  * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv22n , \n" +
         /* 12 */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv10m , s_ukPdv10m  * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv10m , \n" +
         /* 13 */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv10n , s_ukPdv10n  * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv10n , \n" +
         /* 14 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn23m , s_ukOsn23m  * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn23m , \n" +
         /* 15 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn23n , s_ukOsn23n  * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn23n , \n" +
         /* 16 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn22m , s_ukOsn22m  * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn22m , \n" +
         /* 17 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn22n , s_ukOsn22n  * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn22n , \n" +
         /* 18 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn10m , s_ukOsn10m  * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn10m , \n" +
         /* 19 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn10n , s_ukOsn10n  * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn10n , \n" +
         /* 20 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn0   , s_ukOsn0    * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn0   , \n" +
         /* 21 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnPr  , s_ukOsnPr   * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnPr  , \n" +
         /* 22 */ preffix + "SUM(IF(pdvR12 != 2, s_ukMrz    , s_ukMrz     * " + ftrUplata + " / s_ukKCRMP)) as s_ukMrz    , \n" +
         /* 23 */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv    , s_ukPdv     * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv    , \n" +
         /* 24 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn07  , s_ukOsn07   * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn07  , \n" +
         /* 25 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn08  , s_ukOsn08   * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn08  , \n" +
         /* 26 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn09  , s_ukOsn09   * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn09  , \n" +
         /* 27 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn10  , s_ukOsn10   * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn10  , \n" +
         /* 28 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn11  , s_ukOsn11   * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn11  , \n" +
         /* 29 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnUr23, s_ukOsnUr23 * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnUr23, \n" +
         /* 30 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnUu10, s_ukOsnUu10 * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnUu10, \n" +
         /* 31 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnUu22, s_ukOsnUu22 * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnUu22, \n" +
         /* 32 */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnUu23, s_ukOsnUu23 * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnUu23, \n" +
         /* 33 */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvUr23, s_ukPdvUr23 * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvUr23, \n" +
         /* 34 */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvUu10, s_ukPdvUu10 * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvUu10, \n" +
         /* 35 */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvUu22, s_ukPdvUu22 * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvUu22, \n" +
         /* 36 */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvUu23, s_ukPdvUu23 * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvUu23, \n" +

         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv25m , s_ukPdv25m  * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv25m , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv25n , s_ukPdv25n  * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv25n , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn25m , s_ukOsn25m  * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn25m , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn25n , s_ukOsn25n  * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn25n , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnUr25, s_ukOsnUr25 * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnUr25, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnUu25, s_ukOsnUu25 * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnUu25, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvUr25, s_ukPdvUr25 * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvUr25, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvUu25, s_ukPdvUu25 * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvUu25, \n" +

         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv05m , s_ukPdv05m  * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv05m , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv05n , s_ukPdv05n  * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv05n , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn05m , s_ukOsn05m  * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn05m , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn05n , s_ukOsn05n  * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn05n , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnUr05, s_ukOsnUr05 * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnUr05, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvUr05, s_ukPdvUr05 * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvUr05, \n" +

         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnR25m_EU, s_ukOsnR25m_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnR25m_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnR25n_EU, s_ukOsnR25n_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnR25n_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnU25m_EU, s_ukOsnU25m_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnU25m_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnU25n_EU, s_ukOsnU25n_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnU25n_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnR10m_EU, s_ukOsnR10m_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnR10m_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnR10n_EU, s_ukOsnR10n_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnR10n_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnU10m_EU, s_ukOsnU10m_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnU10m_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnU10n_EU, s_ukOsnU10n_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnU10n_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnR05m_EU, s_ukOsnR05m_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnR05m_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnR05n_EU, s_ukOsnR05n_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnR05n_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnU05m_EU, s_ukOsnU05m_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnU05m_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnU05n_EU, s_ukOsnU05n_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnU05n_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn25m_BS , s_ukOsn25m_BS  * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn25m_BS , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn25n_BS , s_ukOsn25n_BS  * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn25n_BS , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn10m_BS , s_ukOsn10m_BS  * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn10m_BS , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn10n_BS , s_ukOsn10n_BS  * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn10n_BS , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn25m_TP , s_ukOsn25m_TP  * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn25m_TP , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn25n_TP , s_ukOsn25n_TP  * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn25n_TP , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvR25m_EU, s_ukPdvR25m_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvR25m_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvR25n_EU, s_ukPdvR25n_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvR25n_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvU25m_EU, s_ukPdvU25m_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvU25m_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvU25n_EU, s_ukPdvU25n_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvU25n_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvR10m_EU, s_ukPdvR10m_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvR10m_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvR10n_EU, s_ukPdvR10n_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvR10n_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvU10m_EU, s_ukPdvU10m_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvU10m_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvU10n_EU, s_ukPdvU10n_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvU10n_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvR05m_EU, s_ukPdvR05m_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvR05m_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvR05n_EU, s_ukPdvR05n_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvR05n_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvU05m_EU, s_ukPdvU05m_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvU05m_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdvU05n_EU, s_ukPdvU05n_EU * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdvU05n_EU, \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv25m_BS , s_ukPdv25m_BS  * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv25m_BS , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv25n_BS , s_ukPdv25n_BS  * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv25n_BS , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv10m_BS , s_ukPdv10m_BS  * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv10m_BS , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv10n_BS , s_ukPdv10n_BS  * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv10n_BS , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv25m_TP , s_ukPdv25m_TP  * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv25m_TP , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukPdv25n_TP , s_ukPdv25n_TP  * " + ftrUplata + " / s_ukKCRMP)) as s_ukPdv25n_TP , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnZP_11  , s_ukOsnZP_11   * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnZP_11  , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnZP_12  , s_ukOsnZP_12   * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnZP_12  , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsnZP_13  , s_ukOsnZP_13   * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsnZP_13  , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn12     , s_ukOsn12      * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn12     , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn13     , s_ukOsn13      * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn13     , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn14     , s_ukOsn14      * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn14     , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn15     , s_ukOsn15      * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn15     , \n" +
         /*    */ preffix + "SUM(IF(pdvR12 != 2, s_ukOsn16     , s_ukOsn16      * " + ftrUplata + " / s_ukKCRMP)) as s_ukOsn16     , \n" +

         // 29.11.2016: 
       ///* 37 */ preffix + "SUM(IF(tt = 'UPL'              , s_ukKC, 0)) as s_ukBlg_UPL, \n" +
         /* 37 */ preffix + "SUM(IF(tt = 'UPL' OR tt = 'BUP', s_ukKC, 0)) as s_ukBlg_UPL, \n" +

       ///* 38 */ preffix + "SUM(IF(tt = 'ISP'              , s_ukKC, 0)) as s_ukBlg_ISP, \n" +
         /* 38 */ preffix + "SUM(IF(tt = 'ISP' OR tt = 'BIS', s_ukKC, 0)) as s_ukBlg_ISP, \n" +

         /* 39 */ preffix + "COUNT(*)         as s_lineCount, \n" +
         ///* 38 */ preffix + "" + ftrUplata + " as r2_uplata  , \n" +
                  preffix + "L.recID          as recID        \n"; // samo za ORDER BY po UNION sintaksi...

      if(isForPDV == false) columnList = columnList.Replace(" * " + ftrUplata + " / s_ukKCRMP", "");

      return columnList;

   }

   public void FillFromSumOnlyDataReader(Faktur faktur_rec, XSqlDataReader reader, bool isIRMgrouping)
   {
      int colIdx=0;
      try
      {

if(isIRMgrouping) faktur_rec.TT          = reader.GetString  (colIdx++);
if(isIRMgrouping) faktur_rec.TtSort      = reader.GetInt16   (colIdx++);
if(isIRMgrouping) faktur_rec.TtNum       = reader.GetUInt32  (colIdx++);
if(isIRMgrouping) faktur_rec.DokDate     = reader.GetDateTime(colIdx++);
if(isIRMgrouping) faktur_rec.NacPlac     = reader.GetString  (colIdx++);
if(isIRMgrouping) faktur_rec.KupdobName  = reader.GetString  (colIdx++);
if(isIRMgrouping) faktur_rec.KdOib       = reader.GetString  (colIdx++);
if(isIRMgrouping) faktur_rec.VezniDok    = reader.GetString  (colIdx++);
if(isIRMgrouping) faktur_rec.KdMjesto    = reader.GetString  (colIdx++);
// 18.07.2019: 
if(isIRMgrouping) faktur_rec.PdvDate     = reader.GetDateTime(colIdx++);
                  
         /* 00 */ faktur_rec.S_ukK       = reader.GetDecimal (colIdx++);
         /* 01 */ faktur_rec.S_ukKC      = reader.GetDecimal (colIdx++);
         /* 02 */ faktur_rec.S_ukKCRP    = reader.GetDecimal (colIdx++);
         /* 03 */ faktur_rec.S_ukKCRM    = reader.GetDecimal (colIdx++);
         /* 04 */ faktur_rec.S_ukKCR     = reader.GetDecimal (colIdx++);
         /* 05 */ faktur_rec.S_ukRbt1    = reader.GetDecimal (colIdx++);
         /* 06 */ faktur_rec.S_ukRbt2    = reader.GetDecimal (colIdx++);
         /* 07 */ faktur_rec.S_ukZavisni = reader.GetDecimal (colIdx++);
         /* 08 */ faktur_rec.S_ukPdv23m  = reader.GetDecimal (colIdx++);
         /* 09 */ faktur_rec.S_ukPdv23n  = reader.GetDecimal (colIdx++);
         /* 10 */ faktur_rec.S_ukPdv22m  = reader.GetDecimal (colIdx++);
         /* 11 */ faktur_rec.S_ukPdv22n  = reader.GetDecimal (colIdx++);
         /* 12 */ faktur_rec.S_ukPdv10m  = reader.GetDecimal (colIdx++);
         /* 13 */ faktur_rec.S_ukPdv10n  = reader.GetDecimal (colIdx++);
         /* 14 */ faktur_rec.S_ukOsn23m  = reader.GetDecimal (colIdx++);
         /* 15 */ faktur_rec.S_ukOsn23n  = reader.GetDecimal (colIdx++);
         /* 16 */ faktur_rec.S_ukOsn22m  = reader.GetDecimal (colIdx++);
         /* 17 */ faktur_rec.S_ukOsn22n  = reader.GetDecimal (colIdx++);
         /* 18 */ faktur_rec.S_ukOsn10m  = reader.GetDecimal (colIdx++);
         /* 19 */ faktur_rec.S_ukOsn10n  = reader.GetDecimal (colIdx++);
         /* 20 */ faktur_rec.S_ukOsn0    = reader.GetDecimal (colIdx++);
         /* 21 */ faktur_rec.S_ukOsnPr   = reader.GetDecimal (colIdx++);
         /* 22 */ faktur_rec.S_ukMrz     = reader.GetDecimal (colIdx++);
         /* 23 */ faktur_rec.S_ukPdv     = reader.GetDecimal (colIdx++);
         /* 24 */ faktur_rec.S_ukOsn07   = reader.GetDecimal (colIdx++);
         /* 25 */ faktur_rec.S_ukOsn08   = reader.GetDecimal (colIdx++);
         /* 26 */ faktur_rec.S_ukOsn09   = reader.GetDecimal (colIdx++);
         /* 27 */ faktur_rec.S_ukOsn10   = reader.GetDecimal (colIdx++);
         /* 28 */ faktur_rec.S_ukOsn11   = reader.GetDecimal (colIdx++);
         /* 29 */ faktur_rec.S_ukOsnUr23 = reader.GetDecimal (colIdx++);
         /* 30 */ faktur_rec.S_ukOsnUu10 = reader.GetDecimal (colIdx++);
         /* 31 */ faktur_rec.S_ukOsnUu22 = reader.GetDecimal (colIdx++);
         /* 32 */ faktur_rec.S_ukOsnUu23 = reader.GetDecimal (colIdx++);
         /* 33 */ faktur_rec.S_ukPdvUr23 = reader.GetDecimal (colIdx++);
         /* 34 */ faktur_rec.S_ukPdvUu10 = reader.GetDecimal (colIdx++);
         /* 35 */ faktur_rec.S_ukPdvUu22 = reader.GetDecimal (colIdx++);
         /* 36 */ faktur_rec.S_ukPdvUu23 = reader.GetDecimal (colIdx++);

         /*    */ faktur_rec.S_ukPdv25m  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdv25n  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsn25m  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsn25n  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnUr25 = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnUu25 = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdvUr25 = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdvUu25 = reader.GetDecimal (colIdx++);

         /*    */ faktur_rec.S_ukPdv05m  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdv05n  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsn05m  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsn05n  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnUr05 = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdvUr05 = reader.GetDecimal (colIdx++);

         /*    */ faktur_rec.S_ukOsnR25m_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnR25n_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnU25m_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnU25n_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnR10m_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnR10n_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnU10m_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnU10n_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnR05m_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnR05n_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnU05m_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnU05n_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsn25m_BS  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsn25n_BS  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsn10m_BS  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsn10n_BS  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsn25m_TP  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsn25n_TP  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdvR25m_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdvR25n_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdvU25m_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdvU25n_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdvR10m_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdvR10n_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdvU10m_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdvU10n_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdvR05m_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdvR05n_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdvU05m_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdvU05n_EU = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdv25m_BS  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdv25n_BS  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdv10m_BS  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdv10n_BS  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdv25m_TP  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukPdv25n_TP  = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnZP_11   = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnZP_12   = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsnZP_13   = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsn12      = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsn13      = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsn14      = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsn15      = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukOsn16      = reader.GetDecimal (colIdx++);

         /* 37 */ faktur_rec.S_ukBlg_UPL = reader.GetDecimal (colIdx++);
         /* 38 */ faktur_rec.S_ukBlg_ISP = reader.GetDecimal (colIdx++);

   // 19.03.2018: data layer 'PdvNum' property oslobađamo od dosadasnje upotrebe da bi ga mogli koristiti za zaista data layer upotrebu 
       ///* 39 */ faktur_rec.PdvNum      = reader.GetUInt32  (colIdx++); // s_lineCount
         /* 39 */ faktur_rec.X_PdvNum    = reader.GetUInt32  (colIdx++); // s_lineCount

       ///* 38 */ faktur_rec.R2_uplata   = reader.GetUInt32  (colIdx++); // " + ftrUplata + " 
      }
      catch(SqlNullValueException) // kada prema filterMemberzima nema nicega  
      {
      }

      return;
   }

   #endregion FillFromSumOnlyDataReader

   #region FillFrom_KPM_Reader (Knjiga Popisa u Maloprodaji)

#if OldKPM
   public static string ColumnListFromKPM_Reader(string preffix, bool isULAZpass)
   {
      string columnList;

      columnList =

         // dodano TEK!!! 14.01.2015: 
/*00*/ preffix + (                                        isULAZpass ? "skladCD                                 " : "skladCD "                                                                   ) + "as skladCD      , \n" +
/*00*/ preffix + (                                        isULAZpass ? "dokDate                                 " : "dokDate "                                                                   ) + "as dokDate      , \n" +
/*01*/ preffix + (                                        isULAZpass ? "tt                                      " : "tt      "                                                                   ) + "as tt           , \n" +
/*02*/ preffix + (                                        isULAZpass ? "ttSort                                  " : "ttSort  "                                                                   ) + "as ttSort       , \n" +
/*03*/ preffix + (                                        isULAZpass ? "ttNum                                   " : "ttNum   "                                                                   ) + "as ttNum        , \n" +
/*04*/ preffix + (                                        isULAZpass ? "CONCAT(tt, '-', ttNum, ' ', kupdobName) " : "CONCAT(MIN(ttNum), '-', MAX(ttNum), ' Popusti i Dnevni Promet Maloprodaje')") + "as KupdobName   , \n" +
/*05*/ preffix + (                                        isULAZpass ? "X.s_ukKCRMP                             " : "SUM(X.s_ukKCRMP            ) "                                              ) + "as S_ukKCRP     , \n" +
/*06*/ preffix + (                                        isULAZpass ? "X.s_ukKCRP_usl                          " : "SUM(X.s_ukKCRP_usl         ) "                                              ) + "as S_ukKCRP_usl , \n" +
/*07*/ preffix + (                                        isULAZpass ? "X.s_ukMSK_00                            " : "SUM(X.s_ukMSK_00           ) "                                              ) + "as S_ukMSK_00   , \n" +
/*08*/ preffix + (                                        isULAZpass ? "X.s_ukMSK_10                            " : "SUM(X.s_ukMSK_10           ) "                                              ) + "as S_ukMSK_10   , \n" +
/*09*/ preffix + (                                        isULAZpass ? "X.s_ukMSK_23                            " : "SUM(X.s_ukMSK_23           ) "                                              ) + "as S_ukMSK_23   , \n" +
/*  */ preffix + (                                        isULAZpass ? "X.s_ukMSK_25                            " : "SUM(X.s_ukMSK_25           ) "                                              ) + "as S_ukMSK_25   , \n" +
/*  */ preffix + (                                        isULAZpass ? "X.s_ukMSK_05                            " : "SUM(X.s_ukMSK_05           ) "                                              ) + "as S_ukMSK_05   , \n" +
/*10*/ preffix + (ZXC.RRD.Dsc_IsSupressSHADOWing ? "0 " : isULAZpass ? "R.K_NivVrj00                            " : "SUM(R.K_NivVrj00           ) "                                              ) + "as K_NivVrj00   , \n" +
/*11*/ preffix + (ZXC.RRD.Dsc_IsSupressSHADOWing ? "0 " : isULAZpass ? "R.K_NivVrj10                            " : "SUM(R.K_NivVrj10           ) "                                              ) + "as K_NivVrj10   , \n" +
/*12*/ preffix + (ZXC.RRD.Dsc_IsSupressSHADOWing ? "0 " : isULAZpass ? "R.K_NivVrj23                            " : "SUM(R.K_NivVrj23           ) "                                              ) + "as K_NivVrj23   , \n" +
/*  */ preffix + (ZXC.RRD.Dsc_IsSupressSHADOWing ? "0 " : isULAZpass ? "R.K_NivVrj25                            " : "SUM(R.K_NivVrj25           ) "                                              ) + "as K_NivVrj25   , \n" +
/*  */ preffix + (ZXC.RRD.Dsc_IsSupressSHADOWing ? "0 " : isULAZpass ? "R.K_NivVrj05                            " : "SUM(R.K_NivVrj05           ) "                                              ) + "as K_NivVrj05   , \n" +
/*13*/ preffix + (                                        isULAZpass ? "1                                       " : "COUNT(DISTINCT fakturRecID)  "                                              ) + "as K_dokCount   , \n" +
/*14*/ preffix + (                                        isULAZpass ? "X.s_ukPpmvOsn                           " : "SUM(X.s_ukPpmvOsn          ) "                                              ) + "as S_ukPpmvOsn  , \n" +
/*15*/ preffix + (                                        isULAZpass ? "X.s_ukPpmvSt1i2                         " : "SUM(X.s_ukPpmvSt1i2        ) "                                              ) + "as S_ukPpmvSt1i2  \n" ;

      return columnList;

   }
#endif

   public static string ColumnListFromKPM_Reader(string preffix, int unionRbr) // 1 - malop ULAZ, 2 - IRM-ovi po grupirani danu, 3 - implicitna nivelacija IRM-ova 
   {
      string columnList = "";

      columnList =

         // dodano TEK!!! 14.01.2015: 
/*00*/ preffix + "skladCD as skladCD, \n" +
/*00*/ preffix + "dokDate as dokDate, \n" +
/*01*/ preffix + "tt      as tt     , \n" +
/*02*/ preffix + (unionRbr == 1 ? "ttSort " : unionRbr == 2 ? "ttSort " : /*unionRbr == 3*/"0 ") + "as ttSort,\n" +
/*03*/ preffix + "ttNum   as ttNum  , \n" +

/*04*/ preffix + (unionRbr == 1 ? "CONCAT(tt, '-', ttNum, ' ', kupdobName) " : unionRbr == 2 ? "CONCAT(MIN(ttNum), '-', MAX(ttNum), ' Popusti i Dnevni Promet Maloprodaje')" : /*unionRbr == 3*/"' NIVELACIJA po IRM'") + "as KupdobName   , \n" +

/*05*/ preffix + (unionRbr == 1 ? "X.s_ukKCRMP                             " : unionRbr == 2 ? "SUM(X.s_ukKCRMP            ) " : /*unionRbr == 3*/"0 "                     ) + "as S_ukKCRP     ,\n" +
/*06*/ preffix + (unionRbr == 1 ? "X.s_ukKCRP_usl                          " : unionRbr == 2 ? "SUM(X.s_ukKCRP_usl         ) " : /*unionRbr == 3*/"0 "                     ) + "as S_ukKCRP_usl ,\n" +
/*07*/ preffix + (unionRbr == 1 ? "X.s_ukMSK_00                            " : unionRbr == 2 ? "SUM(X.s_ukMSK_00           ) " : /*unionRbr == 3*/"0 "                     ) + "as S_ukMSK_00   ,\n" +
/*08*/ preffix + (unionRbr == 1 ? "X.s_ukMSK_10                            " : unionRbr == 2 ? "SUM(X.s_ukMSK_10           ) " : /*unionRbr == 3*/"0 "                     ) + "as S_ukMSK_10   ,\n" +
/*09*/ preffix + (unionRbr == 1 ? "X.s_ukMSK_23                            " : unionRbr == 2 ? "SUM(X.s_ukMSK_23           ) " : /*unionRbr == 3*/"0 "                     ) + "as S_ukMSK_23   ,\n" +
/*  */ preffix + (unionRbr == 1 ? "X.s_ukMSK_25                            " : unionRbr == 2 ? "SUM(X.s_ukMSK_25           ) " : /*unionRbr == 3*/"0 "                     ) + "as S_ukMSK_25   ,\n" +
/*  */ preffix + (unionRbr == 1 ? "X.s_ukMSK_05                            " : unionRbr == 2 ? "SUM(X.s_ukMSK_05           ) " : /*unionRbr == 3*/"0 "                     ) + "as S_ukMSK_05   ,\n" +
/*10*/ preffix + (unionRbr == 1 ? KPM_ulaz_SumClause("00")                   : unionRbr == 2 ? "0 "                            : /*unionRbr == 3*/KPM_izlaz_SumClause("00")) + "as K_NivVrj00   ,\n" +
/*11*/ preffix + (unionRbr == 1 ? KPM_ulaz_SumClause("10")                   : unionRbr == 2 ? "0 "                            : /*unionRbr == 3*/KPM_izlaz_SumClause("10")) + "as K_NivVrj10   ,\n" +
/*12*/ preffix + (unionRbr == 1 ? KPM_ulaz_SumClause("23")                   : unionRbr == 2 ? "0 "                            : /*unionRbr == 3*/KPM_izlaz_SumClause("23")) + "as K_NivVrj23   ,\n" +
/*  */ preffix + (unionRbr == 1 ? KPM_ulaz_SumClause("25")                   : unionRbr == 2 ? "0 "                            : /*unionRbr == 3*/KPM_izlaz_SumClause("25")) + "as K_NivVrj25   ,\n" +
/*  */ preffix + (unionRbr == 1 ? KPM_ulaz_SumClause("05")                   : unionRbr == 2 ? "0 "                            : /*unionRbr == 3*/KPM_izlaz_SumClause("05")) + "as K_NivVrj05   ,\n" +
/*13*/ preffix + (unionRbr == 1 ? "1 "                                       : unionRbr == 2 ? "COUNT(DISTINCT fakturRecID)  " : /*unionRbr == 3*/"0 "                     ) + "as K_dokCount   ,\n" +
// 17.05.2021: bijo BUG. kada je storno ppmv-a ... vidi logiku R_ukPpmvIzn ... a sada ovime izjednacismo 
//14*/ preffix + (unionRbr == 1 ? "X.s_ukPpmvOsn   "                         : unionRbr == 2 ? "SUM(X.s_ukPpmvOsn          ) " : /*unionRbr == 3*/"0 "                     ) + "as S_ukPpmvOsn  ,\n" +
/*14*/ preffix + (unionRbr == 1 ? "X.s_ukPpmvOsn   "                         : unionRbr == 2 ? "SUM(IF(F.S_ukK < 0.00 OR UPPER(F.Napomena) LIKE '%STORNO%', -X.S_ukPpmvOsn, X.S_ukPpmvOsn)          ) " : /*unionRbr == 3*/"0 "                     ) + "as S_ukPpmvOsn  ,\n" +
/*15*/ preffix + (unionRbr == 1 ? "X.s_ukPpmvSt1i2 "                         : unionRbr == 2 ? "SUM(X.s_ukPpmvSt1i2        ) " : /*unionRbr == 3*/"0 "                     ) + "as S_ukPpmvSt1i2 \n" ;

      return columnList;
   }

   public static string KPM_ulaz_SumClause(string pdvSt)
   {
      return "SUM(IF(rtrpdvSt = " + pdvSt + ", (IF(A.t_tt = 'IZM', rtrIzlazKol, (pstKol+ulazKol-izlazKol)-(rtrPstKol + rtrUlazKol-rtrIzlazKol)))*(rtrCijenaMPC-prevMalopCij), 0)) ";
   }
   public static string KPM_izlaz_SumClause(string pdvSt)
   {
      // 29.03.2016: !?!?!? obrni ova dva dole reda (remarckiraj prvi a odr drugi)
    //return "SUM(IF(rtrpdvSt = " + pdvSt + "                      , (rtrIzlazKol)*(rtrCijenaMPC-prevMalopCij), 0)) ";
      return "SUM(IF(rtrpdvSt = " + pdvSt + " AND rtrIsIrmUslug = 0, (rtrIzlazKol)*(rtrCijenaMPC-prevMalopCij), 0)) ";
   }

   //public static string rStr_StanjeKol  { get { return "(pstKol + ulazKol - izlazKol)"         ; } }
   //public static string rStr_thisRtrKol { get { return "(rtrPstKol + rtrUlazKol - rtrIzlazKol)"; } }
   //public static string rStr_prevStKol  { get { return rStr_StanjeKol + "-" + rStr_thisRtrKol  ; } }

   public void FillFrom_KPM_Reader(Faktur faktur_rec, XSqlDataReader reader, bool isULAZpass)
   {
      int colIdx=0;
      try
      {
         // dodano TEK!!! 14.01.2015: 

         /* 00 */ faktur_rec.SkladCD         = reader.GetString  (colIdx++);
         /* 00 */ faktur_rec.DokDate         = reader.GetDateTime(colIdx++);
         /* 01 */ faktur_rec.TT              = reader.GetString  (colIdx++);
         /* 02 */ faktur_rec.TtSort          = reader.GetInt16   (colIdx++);
         /* 03 */ faktur_rec.TtNum           = reader.GetUInt32  (colIdx++);
         /* 04 */ faktur_rec.KupdobName      = reader.GetString  (colIdx++);
         /* 05 */ faktur_rec.S_ukKCRP        = reader.GetDecimal (colIdx++);
         /* 06 */ faktur_rec.S_ukKCRP_usl    = reader.GetDecimal (colIdx++);
         /* 07 */ faktur_rec.S_ukMSK_00      = reader.GetDecimal (colIdx++);
         /* 08 */ faktur_rec.S_ukMSK_10      = reader.GetDecimal (colIdx++);
         /* 09 */ faktur_rec.S_ukMSK_23      = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukMSK_25      = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.S_ukMSK_05      = reader.GetDecimal (colIdx++);
         /* 10 */ faktur_rec.K_NivVrj00      = reader.GetDecimal (colIdx++);
         /* 11 */ faktur_rec.K_NivVrj10      = reader.GetDecimal (colIdx++);
         /* 12 */ faktur_rec.K_NivVrj23      = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.K_NivVrj25      = reader.GetDecimal (colIdx++);
         /*    */ faktur_rec.K_NivVrj05      = reader.GetDecimal (colIdx++);
         /* 13 */ faktur_rec.K_dokCount      = reader.GetUInt32  (colIdx++);
         /* 14 */ faktur_rec.S_ukPpmvOsn     = reader.GetDecimal (colIdx++);
         /* 15 */ faktur_rec.S_ukPpmvSt1i2   = reader.GetDecimal (colIdx++);

      }
      catch(SqlNullValueException) // kada prema filterMemberzima nema nicega  
      {
      }

      return;
   }

   #endregion FillFrom_KPM_Reader (Knjiga Popisa u Maloprodaji)

   #region FillFrom_URM_Fak2Nal_Reader (ULAZ Maloprodajni)

#if OldKPMversion
   public static string ColumnListFrom_URM_Fak2Nal_Reader(string preffix)
   {
      string columnList =

       preffix +  "F.*, X.*,                             \n" +
/*13*/ preffix + ("R.K_NivVrj00   ") + "as K_NivVrj00  , \n" +
/*14*/ preffix + ("R.K_NivVrj10   ") + "as K_NivVrj10  , \n" +
/*15*/ preffix + ("R.K_NivVrj23   ") + "as K_NivVrj23  , \n" +
/*  */ preffix + ("R.K_NivVrj25   ") + "as K_NivVrj25  , \n" +
/*  */ preffix + ("R.K_NivVrj05   ") + "as K_NivVrj05    \n" ;

      return columnList;
   }
#endif
   public static string ColumnListFrom_URM_Fak2Nal_Reader(string preffix)
   {
      string columnList =

       preffix +  "F.*, X.*,                             \n" +
/*13*/ preffix + KPM_ulaz_SumClause("00")  + "as K_NivVrj00  , \n" +
/*14*/ preffix + KPM_ulaz_SumClause("10")  + "as K_NivVrj10  , \n" +
/*15*/ preffix + KPM_ulaz_SumClause("23")  + "as K_NivVrj23  , \n" +
/*  */ preffix + KPM_ulaz_SumClause("25")  + "as K_NivVrj25  , \n" +
/*  */ preffix + KPM_ulaz_SumClause("05")  + "as K_NivVrj05    \n" ;

      return columnList;
   }

   #endregion FillFrom_URM_Fak2Nal_Reader (ULAZ Maloprodajni)

   #endregion FillFromDataReader

   #region LoadTranses

   public override void LoadTranses(XSqlConnection conn, VvDocumentRecord vvDocumentRecord, bool isArhiva)
   {
      Faktur faktur_rec = (Faktur)vvDocumentRecord;

      // ==================================================================================== 

      if(faktur_rec.Transes == null) faktur_rec.Transes = new List<Rtrans>();
      else                           faktur_rec.Transes.Clear();

      // 14.1.2011:
      if((faktur_rec.TtInfo.IsIRArucableTT || 
          faktur_rec.TtInfo.HasShadowTT       ) && isArhiva == false)
      {
         LoadGenericVvDataRecordList<Rtrans>(conn, faktur_rec.Transes, GetFilterMembers_LoadTranses(faktur_rec.RecID), isArhiva ? Rtrans.recordNameArhiva : Rtrans.recordName, "L.t_serial ", true);
      }
      else
      {
         LoadGenericTransesList<Rtrans>(conn, faktur_rec.Transes, faktur_rec.RecID, isArhiva);
      }
      
      // 21.03.2013: ___START___
      // ==================================================================================== 

      if(faktur_rec.Transes2 == null) faktur_rec.Transes2 = new List<Rtrano>();
      else                            faktur_rec.Transes2.Clear();

      LoadGenericTransesList<Rtrano>(conn, faktur_rec.Transes2, faktur_rec.RecID, isArhiva);
      // 21.03.2013: ___END___


      if(faktur_rec.IsTwinTT)
      {
         string origTT = faktur_rec.TT;
         string twinTT = ZXC.TtInfo(origTT).TwinTT;
         
         CheckTwinPairs(); // TODO!!! 

         RemoveTwins(faktur_rec.Transes, twinTT);
      }

      // 11.05.2022: dodan ForEach, a 
      // 19.05.2022: opkoljen if()-om jer kokoš/jaje kad ima ZTR-a ... nek to rjesavaju next generacije :-) 
      if(faktur_rec.IsZtrPresent == false)
      {
         faktur_rec.Transes.ForEach(rtr => rtr.CalcTransResults(faktur_rec));
      }

   }

   private List<VvSqlFilterMember> GetFilterMembers_LoadTranses(uint fakturRecID)
   {
      DataRow drSchema;
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      drSchema = ZXC.RtransDao.TheSchemaTable.Rows[ZXC.RtrCI.t_parentID];
      
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elFakturRecID", fakturRecID, " = "));
      
      return filterMembers;
   }

   private void RemoveTwins(List<Rtrans> transes, string twinTT_toBeRemoved)
   {
      List<Rtrans> rtransesToRemoveList = new List<Rtrans>(transes.Count / 2);

      foreach(Rtrans rtrans_rec in transes)
      {
         if(rtrans_rec.T_TT == twinTT_toBeRemoved) rtransesToRemoveList.Add(rtrans_rec);
      }

      foreach(Rtrans rtrans_rec in rtransesToRemoveList)
      {
         transes.Remove(rtrans_rec);
      }
   }

   private void CheckTwinPairs()
   {
      //throw new NotImplementedException();
   }

   #endregion LoadTranses

   #region LoadExtender

   public override void LoadExtender(XSqlConnection conn, VvDataRecord vvDataRecord, bool isArhiva)
   {
      Faktur faktur_rec = (Faktur)vvDataRecord;

      // ==================================================================================== 

      if(faktur_rec.TheEx == null) faktur_rec.TheEx = new FaktEx();
      else                         faktur_rec.TheEx.Memset0(0);

      bool found = faktur_rec.TheEx.VvDao.SetMe_Record_bySomeUniqueColumn(conn, faktur_rec.TheEx, faktur_rec.RecID, ZXC.FaktExSchemaRows[ZXC.FexCI.fakturRecID], isArhiva);

      // 31.10.2012: 
      if(found == false)
      {
         ZXC.RepairMissingFakturEx_InProgress = true;
         RepairMissingFakturEx(conn, faktur_rec);
      }
   }

   private void RepairMissingFakturEx(XSqlConnection conn, Faktur faktur_rec)
   {
      bool OK;

      faktur_rec.Transes.ForEach(rtr => rtr.CalcTransResults(null));
      faktur_rec.TakeTransesSumToDokumentSum(true);

      OK = faktur_rec.VvDao.AddExtenderRec(conn, faktur_rec.TheEx, faktur_rec.RecID, false, false);

      if(OK) ZXC.aim_emsg(MessageBoxIcon.Warning, "Ovome dokumentu nedostaju neki krucijalni podaci sa zaglavlja.\n\nMolim Vas da sa 'Ispravi' dopunite partnera i ostale podatke.");
   }

   #endregion LoadExtender

   #region FakturCI struct & InitializeSchemaColumnIndexes()

   public struct FakturCI
   {
      internal int recID;
      internal int addTS;
      internal int modTS;
      internal int addUID;
      internal int modUID;
      internal int lanSrvID;
      internal int lanRecID;

      internal int dokNum      ;
      internal int dokDate     ;
      internal int tt          ;
      internal int ttNum       ;
      internal int ttSort      ;
      internal int skladCD     ;
      internal int skladCD2    ;
      internal int vezniDok    ;
      internal int napomena    ;
      internal int opis        ;
      internal int konto       ;
      internal int projektCD   ;
      internal int v1_tt       ;
      internal int v1_ttNum    ;
      internal int v2_tt       ;
      internal int v2_ttNum    ;
      internal int s_ukKC      ;
      internal int s_ukK       ;
      internal int s_trnCount  ;
      internal int osobaX      ;
      internal int dokDate2    ;
      internal int s_ukK2      ;
      internal int decimal01   ;
      internal int decimal02   ;

      internal int origRecID;
      internal int recVer;
      internal int arAction;
      internal int arTS;
      internal int arUID;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public FakturCI CI;
   public static int lastFakturCI;

   protected override void InitializeSchemaColumnIndexes()
   {
      /* 00 */  CI.recID          = GetSchemaColumnIndex("recID");
      /* 01 */  CI.addTS          = GetSchemaColumnIndex("addTS");
      /* 02 */  CI.modTS          = GetSchemaColumnIndex("modTS");
      /* 03 */  CI.addUID         = GetSchemaColumnIndex("addUID");
      /* 04 */  CI.modUID         = GetSchemaColumnIndex("modUID");
      /* 04 */  CI.lanSrvID       = GetSchemaColumnIndex("lanSrvID");
      /* 04 */  CI.lanRecID       = GetSchemaColumnIndex("lanRecID");
      /* 05 */  CI.dokNum         = GetSchemaColumnIndex("dokNum");
      /* 06 */  CI.dokDate        = GetSchemaColumnIndex("dokDate");
      /* 07 */  CI.tt             = GetSchemaColumnIndex("tt");
      /* 08 */  CI.ttNum          = GetSchemaColumnIndex("ttNum");
      /* 09 */  CI.ttSort         = GetSchemaColumnIndex("ttSort");
      /* 10 */  CI.skladCD        = GetSchemaColumnIndex("skladCD");
      /* 11 */  CI.skladCD2       = GetSchemaColumnIndex("skladCD2");
      /* 12 */  CI.vezniDok       = GetSchemaColumnIndex("vezniDok");
      /* 13 */  CI.napomena       = GetSchemaColumnIndex("napomena");
      /* 14 */  CI.opis           = GetSchemaColumnIndex("opis");
      /* 15 */  CI.konto          = GetSchemaColumnIndex("konto");
      /* 16 */  CI.projektCD      = GetSchemaColumnIndex("projektCD");
      /* 17 */  CI.v1_tt          = GetSchemaColumnIndex("v1_tt");
      /* 18 */  CI.v1_ttNum       = GetSchemaColumnIndex("v1_ttNum");
      /* 19 */  CI.v2_tt          = GetSchemaColumnIndex("v2_tt");
      /* 20 */  CI.v2_ttNum       = GetSchemaColumnIndex("v2_ttNum");
      /* 21 */  CI.s_ukKC         = GetSchemaColumnIndex("s_ukKC");
      /* 22 */  CI.s_ukK          = GetSchemaColumnIndex("s_ukK");
      /* 23 */  CI.s_trnCount     = GetSchemaColumnIndex("s_trnCount");
      /* 24 */  CI.osobaX         = GetSchemaColumnIndex("osobaX");
      /*25+2*/  CI.dokDate2       = GetSchemaColumnIndex("dokDate2");
      /*26+2*/  CI.s_ukK2         = GetSchemaColumnIndex("s_ukK2");
      /*27+2*/  CI.decimal01      = GetSchemaColumnIndex("decimal01");
      /*28+2*/  CI.decimal02      = GetSchemaColumnIndex("decimal02");
 lastFakturCI = CI.decimal02; // !!!!!! 

      CI.origRecID      = GetSchemaColumnIndex("origRecID");
      CI.recVer         = GetSchemaColumnIndex("recVer");
      CI.arAction       = GetSchemaColumnIndex("arAction");
      CI.arTS           = GetSchemaColumnIndex("arTS");
      CI.arUID          = GetSchemaColumnIndex("arUID");
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()


   #region SetMeFaktur or SetMeRrans by tt + ttNum

   // PAZI! ovo NE ucitava i Transes 
   // ako ih trebas: someFaktur_rec.VvDao.LoadTranses(TheDbConnection, someFaktur_rec, false); 
   public static bool SetMeFaktur(XSqlConnection conn, Faktur faktur_rec, string _tt, uint _ttNum, bool _shouldBeSilent)
   {
      bool success = true;

      using(XSqlCommand cmd = VvSQL.SetMeFaktur_Command(conn, ZXC.TtInfo(_tt).TtSort, _ttNum, faktur_rec))
      {
         success = ZXC.FakturDao.ExecuteSingleFillFromDataReader(faktur_rec, false, cmd, true);
      } // using cmd 

      if(!success && !_shouldBeSilent)
      {
         VvSQL.ReportGeneric_DB_Error("", "Podatak: [" + _tt + "-" + _ttNum.ToString(/*"000000"*/) + "] ne postoji u datoteci [" + Faktur.recordName + ".", System.Windows.Forms.MessageBoxButtons.OK);
      }

      return success;
   }

   public static bool SetMeFaktur_ByVezniDok2(XSqlConnection conn, Faktur faktur_rec, string vezniDok2)
   {
      FaktEx theEx = new FaktEx();

      bool xOK = faktur_rec.TheEx.VvDao.SetMe_Record_bySomeUniqueColumn(conn, theEx, vezniDok2, ZXC.FaktExSchemaRows[ZXC.FexCI.vezniDok2], false, true);
      bool fOK = false;
      if(xOK) fOK = faktur_rec.VvDao.SetMe_Record_byRecID(conn, faktur_rec, theEx.FakturRecID, false, true);
      if(fOK && xOK) faktur_rec.TheEx = theEx;

      return (fOK && xOK);
   }

   public static bool SetMeFaktur_ByKupdobCdAndVezniDok2(XSqlConnection conn, Faktur faktur_rec, uint kupdobCD, string vezniDok2/*, bool _shouldBeSilent*/)
   {
      bool success = true;

      using(XSqlCommand cmd = VvSQL.SetMeFakturByKupdobCdAndVezniDok2_Command(conn, kupdobCD, vezniDok2))
      {
         success = ZXC.FakturDao.ExecuteSingleFillFromDataReader(faktur_rec, false, cmd, true);
      } // using cmd 

    //if(!success && !_shouldBeSilent)
    //{
    //   VvSQL.ReportGeneric_DB_Error("", "Podatak: [" + _tt + "-" + _ttNum.ToString(/*"000000"*/) + "] ne postoji u datoteci [" + Faktur.recordName + ".", System.Windows.Forms.MessageBoxButtons.OK);
    //}

      return success;
   }


   public static bool SetMeLastRtransForArtiklAndTtNum(XSqlConnection conn, Rtrans rtrans_rec, string _tt, uint _ttNum, string artiklCD, bool _shouldBeSilent)
   {
      bool success = true;

      using(XSqlCommand cmd = VvSQL.SetMeLastRtransForArtiklAndTtNum_Command(conn, ZXC.TtInfo(_tt).TtSort, _ttNum, artiklCD, rtrans_rec))
      {
         success = ZXC.RtransDao.ExecuteSingleFillFromDataReader(rtrans_rec, false, cmd, true);
      } // using cmd 

      if(!success && !_shouldBeSilent)
      {
         VvSQL.ReportGeneric_DB_Error("", "Podatak: [" + artiklCD + " - " + _tt + "-" + _ttNum.ToString("000000") + "] ne postoji u datoteci [" + Rtrans.recordName + ".", System.Windows.Forms.MessageBoxButtons.OK);
      }

      return success;
   }

   public static bool SetMeLastRtransForArtiklAndTT(XSqlConnection conn, Rtrans rtrans_rec, string _tt, string artiklCD, bool _shouldBeSilent)
   {
      bool success = true;

      using(XSqlCommand cmd = VvSQL.SetMeLastRtransForArtiklAndTT_Command(conn, ZXC.TtInfo(_tt).TtSort, artiklCD))
      {
         success = ZXC.RtransDao.ExecuteSingleFillFromDataReader(rtrans_rec, false, cmd, false);
      } // using cmd 

      if(!success && !_shouldBeSilent)
      {
         VvSQL.ReportGeneric_DB_Error("", "Podatak: [" + artiklCD + " - " + _tt + "] ne postoji u datoteci [" + Rtrans.recordName + ".", System.Windows.Forms.MessageBoxButtons.OK);
      }

      return success;
   }

   public static bool SetMeLastRtransForArtiklAnd_anyOfTheseTTs(XSqlConnection conn, Rtrans rtrans_rec, string[] _ttArray, string artiklCD, bool _shouldBeSilent)
   {
      bool success = true;

      using(XSqlCommand cmd = VvSQL.SetMeLastRtransForArtiklAnd_anyOfTheseTTs_Command(conn, _ttArray, artiklCD))
      {
         success = ZXC.RtransDao.ExecuteSingleFillFromDataReader(rtrans_rec, false, cmd, false);
      } // using cmd 

      if(!success && !_shouldBeSilent)
      {
         VvSQL.ReportGeneric_DB_Error("SetMeLastRtransForArtiklAnd_anyOfTheseTTs", "Podatak: [" + artiklCD + " ciljani tt-ovi  ne postoji u datoteci [" + Rtrans.recordName + ".", System.Windows.Forms.MessageBoxButtons.OK);
      }

      return success;
   }

   public static bool SetMeLastUGOrtransForURArtrans(XSqlConnection conn, Rtrans UGOrtrans_rec, Rtrans URArtrans_rec)
   {
      bool success = true;

      using(XSqlCommand cmd = VvSQL.SetMeLastUGOrtransForURArtrans_Command(conn, UGOrtrans_rec, URArtrans_rec))
      {
         success = ZXC.RtransDao.ExecuteSingleFillFromDataReader(UGOrtrans_rec, false, cmd, false);
      } // using cmd 

    //if(!success && !_shouldBeSilent)
    //{
    //   VvSQL.ReportGeneric_DB_Error("", "Podatak: [" + artiklCD + " - " + _tt + "] ne postoji u datoteci [" + Rtrans.recordName + ".", System.Windows.Forms.MessageBoxButtons.OK);
    //}

      return success;
   }

   public static bool FakturExistsFor_Sklad_And_TT_And_Date(XSqlConnection conn, string _skladCD, string _tt, DateTime _dokDate)
   {
      bool success = true;
      uint fakturCount;

      using(XSqlCommand cmd = VvSQL.FakturExistsFor_Sklad_And_TT_And_Date_Command(conn, _skladCD, ZXC.TtInfo(_tt).TtSort, _dokDate))
      {
         using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow))
         {
            success = reader.HasRows;

            if(success && reader.Read())
            {
               fakturCount = reader.GetUInt32(0);
               reader.Close();
            }
            else
            {
               fakturCount = 0;
            }
         } // using reader 
      } // using cmd 

      return fakturCount.IsPositive();
   }

   public static bool SetMeFaktur_BySklad_And_TT_And_Date(XSqlConnection conn, Faktur faktur_rec, string _skladCD, string _tt, DateTime _dokDate, bool _shouldBeSilent)
   {
      bool success = true;

      using(XSqlCommand cmd = VvSQL.SetMeFaktur_BySklad_And_TT_And_Date_Command(conn, _skladCD, ZXC.TtInfo(_tt).TtSort, _dokDate))
      {
         success = ZXC.FakturDao.ExecuteSingleFillFromDataReader(faktur_rec, false, cmd, /*true*/ false /* ??? !!! */);
      } // using cmd 

      if(!success && !_shouldBeSilent)
      {
         VvSQL.ReportGeneric_DB_Error("", "Podatak: [" + _skladCD + " " + _dokDate.ToString(ZXC.VvDateFormat) + " [" + _tt + "-" + "] ne postoji u datoteci [" + Faktur.recordName + ".", System.Windows.Forms.MessageBoxButtons.OK);
      }

      return success;
   }

   #endregion Find Faktur by tt + ttNum

   #region AutoAddFaktur


   private static void AutoAddFaktur(XSqlConnection conn, ref ushort line, Faktur faktur_rec, Rtrans rtrans_rec)
   {
      IVvDao fakturDao = faktur_rec.VvDao;
      IVvDao rtransDao = rtrans_rec.VvDao;

      ushort linesPerDocumentLIMIT = ushort.MaxValue;

      if(line < linesPerDocumentLIMIT) line++;
      else                             line=1;

      if(line == 1) // new zaglavlje needed 
      {
         line = 1;

         faktur_rec.DokNum = fakturDao.GetNextDokNum(conn, Faktur.recordName);

         if(faktur_rec.TtNum.IsZero()) // !!!: Kad je AutoAddFaktur pozvan za neki import onda se preuzima ttNum iz importFile-a, a ako je neki Vektor-ov onda ga izracunaj 
         {
            faktur_rec.TtNum = fakturDao.GetNextTtNum(conn, faktur_rec.VirtualTT, faktur_rec.SkladCD);
         }

         faktur_rec.TtSort = ZXC.TtInfo(faktur_rec.TT).TtSort;

         /* $$$ */ fakturDao.ADDREC(conn, faktur_rec);

         // save new Faktur data for further transes 
         ZXC.FakturRec = faktur_rec.MakeDeepCopy();

      } // new zaglavlje needed 


      rtrans_rec.T_serial   = line;

      rtrans_rec.T_parentID = ZXC.FakturRec.RecID;
      rtrans_rec.T_dokNum   = ZXC.FakturRec.DokNum;

      if(ZXC.FakturRec.TtInfo.IsSkladDateTT) rtrans_rec.T_skladDate = ZXC.FakturRec.SkladDate;
      else                                   rtrans_rec.T_skladDate = ZXC.FakturRec.DokDate;
      if(ZXC.FakturRec.IsExtendable)         rtrans_rec.T_kupdobCD  = ZXC.FakturRec.KupdobCD;

      rtrans_rec.T_TT      = ZXC.FakturRec.TT;
      rtrans_rec.T_ttNum   = ZXC.FakturRec.TtNum;
      rtrans_rec.T_ttSort  = ZXC.FakturRec.TtSort;
      rtrans_rec.T_skladCD = ZXC.FakturRec.SkladCD;

      rtransDao.ADDREC(conn, rtrans_rec, false, false, false, false);

   }

   public static void AutoSetFaktur(XSqlConnection conn, ref ushort line, Faktur faktur_rec, Rtrans rtrans_rec)
   {
      AutoAddFaktur(conn, ref line, faktur_rec, rtrans_rec);
   }

   public static /*void*/Rtrans AutoSetFaktur(
      
      XSqlConnection conn, 
      ref ushort     line, 

      string      f_tt             ,
      uint        f_ttNum          ,
      DateTime    f_dokDate        ,
      string      f_skladCD        ,
      string      f_vezniDok       ,
      string      f_projektCD      ,
      string      f_napomena       ,
      decimal     s_ukZavisni      ,
      decimal     s_ukKCRP         ,
      decimal     s_ukKCRM         ,
      decimal     s_ukKCR          ,
      decimal     s_ukKC           ,
      decimal     s_ukK            ,
      decimal     s_ukRbt1         ,
      decimal     s_ukOsn25m       ,
      decimal     s_ukPdv          ,
      decimal     s_ukPdv25m       ,
      decimal     s_ukMrz          ,
    //decimal     s_ukMSKpdv       ,
      decimal     s_ukMSKpdv_25    ,
    //decimal     s_ukMSK          ,
      decimal     s_ukMSK_25       ,
      uint        s_trnCount       ,
      uint        f_kupdobCD       ,
      string      f_kupdobName     ,
      string      f_kupdobTicker   ,
      string      f_kdOib          ,
      string      f_kdUlica        ,
      string      f_kdMjesto       ,
      string      f_kdZip          ,
      string      f_ZiroRn         ,
      string      f_Konto          ,
ZXC.PdvKnjigaEnum f_PdvKnjiga      ,
      int         f_rokPlac        ,
      DateTime    f_dospDate       ,
                                   
      string      t_artiklCD       ,
      string      t_artiklName     ,
      decimal     t_kol            ,
      decimal     t_cij            ,
      string      t_konto          ,
      decimal     t_pdvSt          ,
      decimal     t_rbt1St         ,
      decimal     t_rbt2St         ,
      decimal     t_wanted         ,
      ZXC.MalopCalcKind t_mCalcKind,
      ZXC.PdvKolTipEnum t_pdvKolTip,
      string      t_jedMj          ,
      decimal     t_ztr            ,
      decimal     t_doCijMal       ,
      decimal     t_noCijMal       ,
      decimal     t_pnpSt          )
   
   {

      Faktur  faktur_rec = new Faktur();
      Rtrans  rtrans_rec = new Rtrans();

      faktur_rec.DokDate      = f_dokDate     ;
      faktur_rec.TT           = f_tt          ;
      faktur_rec.TtNum        = f_ttNum       ;
      faktur_rec.Napomena     = f_napomena    ;
      faktur_rec.SkladCD      = f_skladCD     ;
      faktur_rec.VezniDok     = f_vezniDok    ;
      faktur_rec.ProjektCD    = f_projektCD   ;
      faktur_rec.S_ukZavisni  = s_ukZavisni   ;
      faktur_rec.S_ukKCRP     = s_ukKCRP      ;
      faktur_rec.S_ukKCRM     = s_ukKCRM      ;
      faktur_rec.S_ukKCR      = s_ukKCR       ;
      faktur_rec.S_ukKC       = s_ukKC        ;
      faktur_rec.S_ukK        = s_ukK         ;
      faktur_rec.S_ukRbt1     = s_ukRbt1      ;
      faktur_rec.S_ukOsn25m   = s_ukOsn25m    ;
      faktur_rec.S_ukPdv      = s_ukPdv       ;
      faktur_rec.S_ukPdv25m   = s_ukPdv25m    ;
      faktur_rec.S_ukMrz      = s_ukMrz       ;
    //faktur_rec.S_ukMSKpdv   = s_ukMSKpdv    ;
      faktur_rec.S_ukMskPdv25 = s_ukMSKpdv_25 ;
    //faktur_rec.S_ukMSK      = s_ukMSK       ;
      faktur_rec.S_ukMSK_25   = s_ukMSK_25    ;
      faktur_rec.S_ukTrnCount = s_trnCount    ;
      faktur_rec.KupdobCD     = f_kupdobCD    ;
      faktur_rec.KupdobName   = f_kupdobName  ;
      faktur_rec.KupdobTK     = f_kupdobTicker;
      faktur_rec.KdOib        = f_kdOib       ;
      faktur_rec.KdUlica      = f_kdUlica     ;
      faktur_rec.KdMjesto     = f_kdMjesto    ;
      faktur_rec.KdZip        = f_kdZip       ;
      faktur_rec.ZiroRn       = f_ZiroRn      ;
      faktur_rec.Konto        = f_Konto       ;
      faktur_rec.PdvKnjiga    = f_PdvKnjiga   ;

      faktur_rec.RokPlac      = f_rokPlac     ;
      faktur_rec.DospDate     = f_dospDate    ;

      if(faktur_rec.IsExtendable)
      {
         faktur_rec.PdvDate   = f_dokDate;
         faktur_rec.SkladDate = f_dokDate;
         faktur_rec.PdvR12    = ZXC.PdvR12Enum.R1;
      }

      rtrans_rec.T_artiklCD   = t_artiklCD  ;
      rtrans_rec.T_artiklName = t_artiklName;
      rtrans_rec.T_kol        = t_kol       ;
      rtrans_rec.T_cij        = t_cij       ;
      rtrans_rec.T_konto      = t_konto     ;
      rtrans_rec.T_pdvSt      = t_pdvSt     ;
      rtrans_rec.T_rbt1St     = t_rbt1St    ;
      rtrans_rec.T_rbt2St     = t_rbt2St    ;
      rtrans_rec.T_wanted     = t_wanted    ;
      rtrans_rec.T_mCalcKind  = t_mCalcKind ;
      rtrans_rec.T_pdvColTip  = t_pdvKolTip ;
      rtrans_rec.T_jedMj      = t_jedMj == null ? "" : t_jedMj; // 16.11.2013: ne kuzim zakaj samo ovdje rikne ako je string null? to se prvi put pojavilo kada sam dodao t_jedMj u ovu metodu. Kako npr kaad je f_dokumCD null ne rikne? 
      rtrans_rec.T_ztr        = t_ztr       ;
      rtrans_rec.T_doCijMal   = t_doCijMal  ;
      rtrans_rec.T_noCijMal   = t_noCijMal  ;
      rtrans_rec.T_pnpSt      = t_pnpSt     ;

      AutoAddFaktur(conn, ref line, faktur_rec, rtrans_rec);

      // 5.7.2011:
      return rtrans_rec;
   }

   #endregion AutoAddFaktur

   #region FRSSET or LSTSET Faktur

   // This is PUSE / FUSE sheat ?! 

   public  static Faktur FRSSET_Faktur(XSqlConnection conn, string wantedTT, DateTime dateOd, uint ttNum)
   {
      return FRSSET_LSTSET_Faktur(conn, wantedTT, dateOd, DateTime.MinValue, ttNum, VvSQL.DBNavigActionType.FRS);
   }

   public  static Faktur LSTSET_Faktur(XSqlConnection conn, string wantedTT, DateTime dateDo, uint ttNum)
   {
      return FRSSET_LSTSET_Faktur(conn, wantedTT, DateTime.MinValue, dateDo, ttNum, VvSQL.DBNavigActionType.LST);
   }

   private static Faktur FRSSET_LSTSET_Faktur(XSqlConnection conn, string wantedTT, DateTime dateOd, DateTime dateDo, uint ttNum, VvSQL.DBNavigActionType frsORlst)
   {
      bool OK;
      Faktur faktur_rec = new Faktur();

      uint fromTTnum = (ttNum.NotZero() ? ttNum : frsORlst == VvSQL.DBNavigActionType.FRS ? 0 : uint.MaxValue);

      DateTime fromDate = (frsORlst == VvSQL.DBNavigActionType.FRS && dateOd != DateTime.MinValue ? dateOd : 
                           frsORlst == VvSQL.DBNavigActionType.LST && dateDo != DateTime.MinValue ? dateDo : 
                           DateTime.MinValue);

      VvSQL.OrderDirectEnum direction = (frsORlst == VvSQL.DBNavigActionType.FRS ? VvSQL.OrderDirectEnum.ASC : VvSQL.OrderDirectEnum.DESC);

      using(XSqlCommand cmd = VvSQL.GTEREC_Command(conn, "*", new object[] { ZXC.TtInfo(wantedTT).TtSort, fromDate, fromTTnum }, Set_FRSSET_LSTSET_FilterMembers(wantedTT, dateOd, dateDo, fromTTnum, frsORlst), direction, Faktur.sorterDokDate, 0, 1, false, false, null))
      {
         OK = ZXC.FakturDao.ExecuteSingleFillFromDataReader(faktur_rec, false, cmd, faktur_rec.IsExtendable);

         if(!OK) return null;
      }

      return faktur_rec;
   }

   private static List<VvSqlFilterMember> Set_FRSSET_LSTSET_FilterMembers(string wantedTT, DateTime dateOd, DateTime dateDo, uint ttNum, VvSQL.DBNavigActionType frsORlst)
   {
      DataRow drSchema;
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      // filter for wantedTT                                                                                                                                             
      if(wantedTT.NotEmpty())
      {
         drSchema = ZXC.FakturDao.TheSchemaTable.Rows[ZXC.FakturDao.CI.tt];

         filterMembers.Add(new VvSqlFilterMember(drSchema, "elWantedTT", wantedTT, " = "));
      }

      // filter from dateOd                                                                                                                                             
      if(dateOd != DateTime.MinValue)
      {
         drSchema = ZXC.FakturDao.TheSchemaTable.Rows[ZXC.FakturDao.CI.dokDate];

         filterMembers.Add(new VvSqlFilterMember(drSchema, "elDateOd", dateOd, " >= "));
      }

      // filter from dateDo                                                                                                                                             
      if(dateDo != DateTime.MinValue)
      {
         drSchema = ZXC.FakturDao.TheSchemaTable.Rows[ZXC.FakturDao.CI.dokDate];

         filterMembers.Add(new VvSqlFilterMember(drSchema, "elDateDo", dateDo, " <= "));
      }

      // filter from ttNum                                                                                                                                             
      if(ttNum.NotZero())
      {
         drSchema = ZXC.FakturDao.TheSchemaTable.Rows[ZXC.FakturDao.CI.ttNum];

         filterMembers.Add(new VvSqlFilterMember(drSchema, "elTTnum", ttNum, frsORlst == VvSQL.DBNavigActionType.FRS ? " >= " : " <= "));
      }

      return filterMembers;
   }

   #endregion FRSSET or LSTSET Faktur

   #region PDV & KPM Metodz

   // LoadPdvSume(): 
   public static Faktur GetManyFakturSumAsOneSintFaktur(XSqlConnection conn, List<VvSqlFilterMember> filterMembers, bool isUra, bool needsLineCount, string anotherJoinClause)
   {
      bool success = true;
      bool isForPdv = anotherJoinClause.NotEmpty();
      Faktur faktur_rec = new Faktur();

      ZXC.sqlErrNo = 0;

      using(XSqlCommand cmd = (isUra ? (VvSQL.Get_SELECTzvjezdica_AndJOIN_FROM_Command(conn, faktur_rec, ColumnListFromSumOnlyDataReader(false, "", isForPdv), filterMembers, "", anotherJoinClause, "")) :
                                       (VvSQL.Get_PdvIraSUM_Command                   (conn,             ColumnListFromSumOnlyDataReader(false, "", isForPdv), filterMembers, needsLineCount))))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow))
            {
               success = reader.HasRows;

               while(success && reader.Read())
               {
                  ZXC.FakturDao.FillFromSumOnlyDataReader(faktur_rec, reader, false);
               }
               reader.Close();
            }
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("GetManyFakturSumAsOneSintFaktur", ex, System.Windows.Forms.MessageBoxButtons.OK);

            ZXC.sqlErrNo = ex.Number;
         }
      } // using 

      return faktur_rec;
   }

   public static bool LoadIraUnionIrmGroupedFakturList(XSqlConnection conn, List<Faktur> knjigaIraLines, List<VvSqlFilterMember> filterMembers)
   {
      bool success = true;
      Faktur fakturRptLine_rec = new Faktur();

      ZXC.sqlErrNo = 0;

      using(XSqlCommand cmd = (VvSQL.LoadIraUnionIrmGroupedFakturList_Command(conn, ColumnListFromSumOnlyDataReader(true, "", true), filterMembers)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;

               while(success && reader.Read())
               {
                  fakturRptLine_rec = new Faktur();

                  ZXC.FakturDao.FillFromSumOnlyDataReader(fakturRptLine_rec, reader, true);

                  // 06.09.2018: 
                //if(fakturRptLine_rec.TT == Faktur.TT_IRM                                                                     )
                  if(fakturRptLine_rec.TT == Faktur.TT_IRM && fakturRptLine_rec.NacPlac.ToLower().StartsWith("virman") == false)
                  {
                     // 19.03.2018: data layer 'PdvNum' property oslobađamo od dosadasnje upotrebe da bi ga mogli koristiti za zaista data layer upotrebu 
                   //fakturRptLine_rec.KupdobName = "DnevniPromet (" + fakturRptLine_rec.  PdvNum + " IRM)";
                     fakturRptLine_rec.KupdobName = "DnevniPromet (" + fakturRptLine_rec.X_PdvNum + " IRM)";
                  }
                  knjigaIraLines.Add(fakturRptLine_rec);
               }
               reader.Close();
            }
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("LoadIraUnionIrmGroupedFakturList", ex, System.Windows.Forms.MessageBoxButtons.OK);

            ZXC.sqlErrNo = ex.Number;
         }
      } // using 

      return success;
   }

   public static uint GetPdvKnjigaRbrForThisFaktur(XSqlConnection conn, Faktur faktur_rec)
   {
      // 06.08.2015: 
      if(ZXC.IsTEXTHOshop) return 0;

      bool success = true;
      uint theRbr  = 0;

      ZXC.sqlErrNo = 0;

      using(XSqlCommand cmd = VvSQL.GetPdvKnjigaRbrForThisFaktur_Command(conn, faktur_rec))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow))
            {
               success = reader.HasRows;

               if(success && reader.Read())
               {
                  theRbr = reader.GetUInt32(0);
               }
               reader.Close();
            }
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("GetPdvKnjigaRbrForThisFaktur", ex, System.Windows.Forms.MessageBoxButtons.OK);

            ZXC.sqlErrNo = ex.Number;
         }
      } // using 

      return theRbr;
   }

   public static bool LoadKPM_MalUlazUnionIrmGroupedFakturList(XSqlConnection conn, List<Faktur> kpmFakturLines, List<VvSqlFilterMember> filterMembers)
   {
      bool success = true;
      Faktur fakturRptLine_rec = new Faktur();

      ZXC.sqlErrNo = 0;

    //using(XSqlCommand cmd = (VvSQL.Load_KPM_MalUlazUnionIrmGroupedFakturList_Command(conn, ColumnListFromKPM_Reader("", true), ColumnListFromKPM_Reader("", false),                                  filterMembers)))
      using(XSqlCommand cmd = (VvSQL.Load_KPM_MalUlazUnionIrmGroupedFakturList_Command(conn, ColumnListFromKPM_Reader("",    1), ColumnListFromKPM_Reader("",     2), ColumnListFromKPM_Reader("", 3), filterMembers)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;

               while(success && reader.Read())
               {
                  fakturRptLine_rec = new Faktur();

                  ZXC.FakturDao.FillFrom_KPM_Reader(fakturRptLine_rec, reader, true);

                  kpmFakturLines.Add(fakturRptLine_rec);
               }
               reader.Close();
            }
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("LoadKPM_MalUlazUnionIrmGroupedFakturList", ex, System.Windows.Forms.MessageBoxButtons.OK);

            ZXC.sqlErrNo = ex.Number;
         }
      } // using 

      return success;
   }

   public static bool Load_Fak2Nal_URM_FakturList(XSqlConnection conn, List<Faktur> urmFakturLines, List<VvSqlFilterMember> filterMembers, string anotherJoinClause)
   {
      bool success = true;
      Faktur fakturRptLine_rec = new Faktur();

      ZXC.sqlErrNo = 0;

      using(XSqlCommand cmd = (VvSQL.Load_Fak2Nal_URM_FakturList_Command(conn, ColumnListFrom_URM_Fak2Nal_Reader(""), filterMembers, anotherJoinClause)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;

               while(success && reader.Read())
               {
                  fakturRptLine_rec = new Faktur();

                //ZXC.FakturDao.FillFrom_URM_Fak2Nal_Reader(fakturRptLine_rec, reader, true);
                  ZXC.FakturDao.FillFromDataReader         (fakturRptLine_rec, reader, false);

                  fakturRptLine_rec.TheEx = new FaktEx();
                  fakturRptLine_rec.TheEx.IsFillingFromJoinReader   = true;
                  fakturRptLine_rec.TheEx.IsFillingFrom_Fak2Nal_URM = true;
                  fakturRptLine_rec.TheEx.VvDao.FillFromDataReader(fakturRptLine_rec.TheEx, reader, false);
                  fakturRptLine_rec.TheEx.IsFillingFromJoinReader = false;

                  urmFakturLines.Add(fakturRptLine_rec);
               }
               reader.Close();
            }
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("Load_Fak2Nal_URM_FakturList", ex, System.Windows.Forms.MessageBoxButtons.OK);

            ZXC.sqlErrNo = ex.Number;
         }
      } // using 

      return success;
   }

   #endregion PDV & KPM Metodz

   #region Faktur2Nalog

   #region GetNeprebaceniFaktur2NalogList

   public static List<Faktur> GetNeprebaceniFakturAndRtrans2NalogLists(XSqlConnection conn, Faktur2NalogRulesAndData theRules, bool needsRtranses, out ZXC.VvUtilDataPackage[] nacPlacArray)
   {
      // 31.03.2022: 
      if(theRules.DateOd.IsEmpty()) theRules.DateOd = ZXC.projectYearFirstDay;
      if(theRules.DateDo.IsEmpty()) theRules.DateDo = DateTime.Today         ;

      #region Lokal varijablez

      List<Faktur> fakturList = new List<Faktur>();
      List<Rtrans> rtransList = new List<Rtrans>();

      string anotherJoinClause = "";

      List<VvSqlFilterMember> filterMembers = null;

      bool isIRM = false;
      bool isIZM = false;
      bool irmSUMisDaily = !theRules.KtoShemaDsc.Dsc_MirSumMonthly;

      nacPlacArray = new ZXC.VvUtilDataPackage[] { };

      #endregion Lokal varijablez

      #region IsIraRealizOnly

      // 14.02.2017: 
      if(theRules.IsIraRealizOnly)
      {
         string theTT = Faktur.TT_IRA;

         VvRpt_RiSk_Filter _rptFilter = new VvRpt_RiSk_Filter();

         _rptFilter.TT      = theTT          ;
         _rptFilter.DatumOd = theRules.DateOd;
         _rptFilter.DatumDo = theRules.DateDo;

         _rptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakturDao.CI.dokDate], "DateOD", theRules.DateOd, " >= "));
         _rptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakturDao.CI.dokDate], "DateDO", theRules.DateDo, " <= "));

         var RISKvsFtransRptR = new RptR_Rekap_IRMvsFtrans(true, "", _rptFilter, ZXC.RIZ_FilterStyle.Faktur, 
                                                              false,  // ArtiklWithArtstat 
                                                              false,  // ArtStat           
                                                              true ,  // Faktur            
                                                              false,  // Rtrans            
                                                              false,  // Kupdob            
                                                              false,  // Prjkt             
                                                              true ,  // Rtrans4ruc        
                                                              false); // Artikl            
         RISKvsFtransRptR.RptFilter = _rptFilter;
         RISKvsFtransRptR.FillRiskReportLists();

         foreach(VvManyDecimalsReportSourceRow mdItem in RISKvsFtransRptR.TheManyDecimalsList)
         {
               fakturList.Add(new Faktur()
               {
                  TT         = theTT          ,
                  SkladCD    = mdItem.TheStr  ,
                  DokDate    = mdItem.Date_1  ,
                  Ira_ROB_NV = mdItem.DecimA09, // DecimA09    = prNabVrij_Risk - prNabVrij, 
               }
            );
         }

         return fakturList;

      } // if(theRules.IsIraRealizOnly)

      #endregion IsIraRealizOnly

      filterMembers = GetFilterMembers4Fak2Nal(theRules);

      #region Get fakturList MALOPRODAJNI RACUNI Izlazni

    // 25.10.2021 za firme koje knjize samo radi IOS-a ZXC.KSD.Dsc_IsOnlyIOSknjizenje
    // if((theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP) ||
    //    (theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT && theRules.ThisTT_Only == Faktur.TT_IRM))
    //31.03.2022. razdvajamo IsOnlyIOSknjizenje i ForceIRMkaoIRA - ovdje ne treba ulayiti kada forsiramo IRM po IRA???????
    // if(((theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP) ||
    //   (theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT && theRules.ThisTT_Only == Faktur.TT_IRM)) && ZXC.KSD.Dsc_IsOnlyIOSknjizenje == false)
       if(((theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP) ||
           (theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT && theRules.ThisTT_Only == Faktur.TT_IRM)) && ZXC.KSD.Dsc_IsOnlyIOSknjizenje == false && ZXC.KSD.Dsc_ForceIRMkaoIRA == false)
        {
         needsRtranses = false;

         #region Get fakturList_IRM (analiticki IRM po IRM)

         List<Faktur> fakturList_IRM = new List<Faktur>();
         isIRM = true;

         #region NEW in 2015 4 TEXTHO: Do it by SkladCD 

         // 30.05.2014: 
       //if(ZXC.RRD.Dsc_IsSupressSHADOWing) // TEXTHO or something?... 
         // 18.05.2015: 
         if(true/*ZXC.IsTEXTHOany || ZXC.CURR_prjkt_rec.Ticker == "QQTEXT"-*/) // TEXTHO or something?... 
         {
          //List<string> malopSkladCDlist = VvDaoBase.GetDistinctFakturSkladCd(conn, ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, ZXC.TheVvForm.GetvvDB_prjktDB_name(), true);

          //foreach(string skladCD in malopSkladCDlist)
          //{
               VvRpt_RiSk_Filter theRiskFilter = new VvRpt_RiSk_Filter(false);
               theRiskFilter.FilterMembers = new List<VvSqlFilterMember>(/*4*/ 3);

               theRiskFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.tt     ], "elTT"     , Faktur.TT_IRM  , " = " ));
             //theRiskFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.skladCD], "elSkladCD", skladCD        , " = " ));
               theRiskFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.dokDate], "theDateOd", theRules.DateOd, " >= "));
               theRiskFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.dokDate], "theDateDo", theRules.DateDo, " <= "));

               theRiskFilter.DatumOd = theRules.DateOd;
               theRiskFilter.DatumDo = theRules.DateDo;
               theRiskFilter.TT      = Faktur.TT_IRM  ;
             //theRiskFilter.SkladCD = skladCD        ;

                /*RptR_Ira_Ruc*/RptR_RekapFaktur vvReport_Ira_Ruc = 
            new /*RptR_Ira_Ruc*/RptR_RekapFaktur(/*fakturGR*/ "", /*new Vektor.Reports.RIZ.CR_RUC_IRA_Light()*//*null*,*/ /*reportName*/"Fak2Nal_IRM", theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                      false, // ArtiklWithArtstat         
                                                      false, // ArtStat        
                                                      true , // Faktur         
                                                      false, // Rtrans         
                                             /*false*/true , // Kupdob         
                                                      false, // Prjkt          
                                                                                 /*true*/ false , // Rtrans4ruc     
                                                      false);// Artikl         

               vvReport_Ira_Ruc.FillRiskReportLists();

               fakturList_IRM.AddRange(vvReport_Ira_Ruc.TheFakturList);

          //} // foreach(string skladCD in malopSkladCDlist) 

         } // if(ZXC.RRD.Dsc_IsSupressSHADOWing) // TEXTHO or something?... 

         #endregion NEW in 2015 4 TEXTHO: Do it by SkladCD

         #region OLD Classic

         else // OLD, classic 
         {
            string selectWhat = "L.*, R.*, K_NivVrj00, K_NivVrj10, K_NivVrj23, K_NivVrj25, K_NivVrj05, Ira_ROB_NV \n";
            string orderBy    = "dokDate, ttSort, ttNum";

            if(ZXC.RRD.Dsc_IsSupressSHADOWing) selectWhat = "L.*, R.*, Ira_ROB_NV \n";

            anotherJoinClause = ZXC.RRD.Dsc_IsSupressSHADOWing ? "" :
               "LEFT JOIN(                                                                         \n" +
               "  SELECT T.t_parentID,                                                             \n" +
               "  SUM(IF(t_pdvSt =  0, rtrIzlazKol*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj00, \n" +
               "  SUM(IF(t_pdvSt = 10, rtrIzlazKol*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj10, \n" +
               "  SUM(IF(t_pdvSt = 23, rtrIzlazKol*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj23, \n" +
               "  SUM(IF(t_pdvSt = 25, rtrIzlazKol*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj25, \n" +
               "  SUM(IF(t_pdvSt = 05, rtrIzlazKol*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj05, \n" +
               "  SUM(t_kol * rtrCijenaNBC                                        ) as Ira_ROB_NV  \n" + 
               "       FROM rtrans  T                                                              \n" +
               "  LEFT JOIN artstat A ON T.RecID = A.rtransRecID                                   \n" +
               "  WHERE T.t_isIrmUslug = 0                                                         \n" +
               "  GROUP BY T.t_parentID                                                            \n" +
               ") AS RTR ON L.RecID = RTR.t_parentID                                               \n\n";

            anotherJoinClause += "LEFT JOIN ftrans Ft FORCE INDEX(BY_FakRecID) ON (MONTH(dokDate) = MONTH(Ft.t_dokDate) AND Ft.t_tt = 'IM') \n"; // da bi vidjeli nije li vec prebaceno 
            if(theRules.KtoShemaDsc.Dsc_MirSumMonthly == false)
            {
               //14.10.2013 :
             //anotherJoinClause = anotherJoinClause.Replace("MONTH", "");
               anotherJoinClause = anotherJoinClause.Replace("MONTH", "DATE");
            }

            VvDaoBase.LoadGenericVvDataRecordList(conn, fakturList_IRM, filterMembers, "", orderBy, true, selectWhat, anotherJoinClause, true); // ovaj zadnji 'true' means isIRMforFak2Nal 
         }

         #endregion OLD Classic

         #endregion Get fakturList_IRM (analiticki IRM po IRM)

         #region Set fakturList from fakturList_IRM GROUPed BY dokDate OR month

         // 03.10.2013: 
         foreach(Faktur fak in fakturList_IRM)
         {
            if(fak.IsVirmanIRM) // fak.NacPlac.ToLower().Contains("virman") || fak.NacPlac.ToLower().Contains("transakcijski") 
            {
               fak.NacPlac += " " + fak.TtNum.ToString("000000");

               fak.PrimPlatCD = fak.KupdobCD;
               fak.PrimPlatTK = fak.KupdobTK;
            }
         }

         #region nacPlacArray

         nacPlacArray = fakturList_IRM
          .GroupBy(fak => irmSUMisDaily ?
             fak.SKL_DokDate_Full__AsString_And_NacPlac :
             fak.SKL_DokDate_Month_AsString_And_NacPlac)
            .Select(grp => new ZXC.VvUtilDataPackage(
               irmSUMisDaily ? grp.First().SKL_DokDate_Full__AsString/*_And_NacPlac*/ :
                               grp.First().SKL_DokDate_Month_AsString/*_And_NacPlac*/,
               grp.First().NacPlac,
               grp.Sum(f => f.S_ukKCRP),
               grp.First().IsNpCash,
               grp.First/*OrDefault*/(/*f => f.PrimPlatTK.NotEmpty()*/).PrimPlatTK,
               grp.First/*OrDefault*/(/*f => f.PrimPlatCD.NotZero ()*/).PrimPlatCD,
               grp.First().TipBr,
               grp.First().RecID,
               grp.First().SkladCD,
               grp.First().DospDate
            ))
            .OrderByDescending(nacPlac => nacPlac.TheBool   /*IsNpCash*/)
            .ThenByDescending (nacPlac => nacPlac.TheDecimal/*S_ukKCRP*/)
            .ToArray();

         #endregion nacPlacArray

         #region fakturList

         fakturList = fakturList_IRM
          .GroupBy(fak => irmSUMisDaily ? 
           //new { fak.DokDate,       fak.NacPlac } : // ovo ti je primjer kako LINQ grupira po vise od jednog podatka, ali ti tu nece sljakati zbog conditional operatora '?' koji 'oce isti type prije i poslije ':' 
           //new { fak.DokDate.Month, fak.NacPlac })
             fak.SKL_DokDate_Full__AsString/*_And_NacPlac*/ :
             fak.SKL_DokDate_Month_AsString/*_And_NacPlac*/)
            .Select(grp => new Faktur(

                  /* DokDate      */ irmSUMisDaily ? grp.First().DokDate : 
                  /* zadnji dan u mj */ new DateTime(grp.First().DokDate.Year, grp.First().DokDate.Month, DateTime.DaysInMonth(grp.First().DokDate.Year, grp.First().DokDate.Month)),
                  /* RecID        */ 0,
                  /* TT           */ Faktur.TT_IRM,
                  /* TtNum        */ grp.First().TtNum,
                  /* DospDate     */ DateTime.MinValue,
                  /* MtrosCD      */ theRules.KtoShemaDsc.Dsc_IsVisibleColMtrosCD ? Kupdob.GetKupdobFromSkladCD(grp.First().SkladCD).KupdobCD :  0,
                  /* MtrosTK      */ theRules.KtoShemaDsc.Dsc_IsVisibleColMtrosCD ? Kupdob.GetKupdobFromSkladCD(grp.First().SkladCD).Ticker   : "",
                  /* ProjektCD    */ "",
                  /* PdvKnjiga    */ ZXC.PdvKnjigaEnum.REDOVNA, 
                  /* ShouldFak2Nal*/ ZXC.ShouldFak2NalEnum.Knjizi_NORMALNO, 
                  /* Napomena     */ "IRM " + grp.Min(f => f.TtNum) + " - " + grp.Max(f => f.TtNum), 
                  /* V1_tt        */ "", 
                  /* V1_ttNum     */ 0, 
                  /* V2_tt        */ "", 
                  /* V2_ttNum     */ 0, 
                  /* V3_tt        */ "", 
                  /* V3_ttNum     */ 0, 
                  /* V4_tt        */ "", 
                  /* V4_ttNum     */ 0, 
                  /* PrimPlatCD   */ grp.First().PrimPlatCD         , 
                  /* PrimPlatTK   */ grp.First().PrimPlatTK         , 
                  /* KupdobCD     */ 0                              , // uvijek prazan 
                  /* KupdobTK     */ ""                             , // uvijek prazan  
                  /* SkladCD      */ grp.First().SkladCD            , 
                  /* S_ukKCRP     */ grp.Sum  (f => f.S_ukKCRP     ), 
                  /* S_ukOsn23m   */ grp.Sum  (f => f.S_ukOsn23m   ), 
                  /* S_ukOsn23n   */ grp.Sum  (f => f.S_ukOsn23n   ), 
                  /* S_ukOsn22m   */ grp.Sum  (f => f.S_ukOsn22m   ), 
                  /* S_ukOsn22n   */ grp.Sum  (f => f.S_ukOsn22n   ), 
                  /* S_ukOsn10m   */ grp.Sum  (f => f.S_ukOsn10m   ), 
                  /* S_ukOsn10n   */ grp.Sum  (f => f.S_ukOsn10n   ), 
                  /* Konto        */ grp.First().Konto              , 
                  /* S_ukPdv23m   */ grp.Sum  (f => f.S_ukPdv23m   ), 
                  /* S_ukPdv23n   */ grp.Sum  (f => f.S_ukPdv23n   ), 
                  /* S_ukPdv22m   */ grp.Sum  (f => f.S_ukPdv22m   ), 
                  /* S_ukPdv22n   */ grp.Sum  (f => f.S_ukPdv22n   ), 
                  /* S_ukPdv10m   */ grp.Sum  (f => f.S_ukPdv10m   ), 
                  /* S_ukPdv10n   */ grp.Sum  (f => f.S_ukPdv10n   ), 
                  /* VezniDok     */ grp.First().VezniDok           , 
                  /*              */                               
                  /* IsNpCash     */ grp.First().IsNpCash           , 
                  /* NacPlac      */ grp.First().NacPlac            , 
                  /* S_ukKCR      */ grp.Sum  (f => f.S_ukKCR      ), 
                  /* S_ukKCR_usl  */ grp.Sum  (f => f.S_ukKCR_usl  ), 
                  /* S_ukKCRP_usl */ grp.Sum  (f => f.S_ukKCRP_usl ), 
                  /* S_ukMSK_00   */ grp.Sum  (f => f.S_ukMSK_00   ), 
                  /* S_ukMSK_10   */ grp.Sum  (f => f.S_ukMSK_10   ), 
                  /* S_ukMSK_23   */ grp.Sum  (f => f.S_ukMSK_23   ), 
                  /* S_ukMskPdv10 */ grp.Sum  (f => f.S_ukMskPdv10 ), 
                  /* S_ukMskPdv23 */ grp.Sum  (f => f.S_ukMskPdv23 ), 
                  /* K_NivVrj00   */ grp.Sum  (f => f./*K*/R_NivVrj00   ), 
                  /* K_NivVrj10   */ grp.Sum  (f => f./*K*/R_NivVrj10   ), 
                  /* K_NivVrj23   */ grp.Sum  (f => f./*K*/R_NivVrj23   ), 
                  /* Ira_ROB_NV   */ grp.Sum  (f => f.Ira_ROB_NV   ),
                                                                   
                  /* S_ukPdv25m   */ grp.Sum  (f => f.S_ukPdv25m   ), 
                  /* S_ukPdv25n   */ grp.Sum  (f => f.S_ukPdv25n   ), 
                  /* S_ukOsn25m   */ grp.Sum  (f => f.S_ukOsn25m   ), 
                  /* S_ukOsn25n   */ grp.Sum  (f => f.S_ukOsn25n   ), 
                  /* S_ukMskPdv25 */ grp.Sum  (f => f.S_ukMskPdv25 ), 
                  /* S_ukMSK_25   */ grp.Sum  (f => f.S_ukMSK_25   ), 
                  /* K_NivVrj25   */ grp.Sum  (f => f./*K*/R_NivVrj25   ), 
                                                                   
                  /* S_ukPdv05m   */ grp.Sum  (f => f.S_ukPdv05m   ), 
                  /* S_ukPdv05n   */ grp.Sum  (f => f.S_ukPdv05n   ), 
                  /* S_ukOsn05m   */ grp.Sum  (f => f.S_ukOsn05m   ), 
                  /* S_ukOsn05n   */ grp.Sum  (f => f.S_ukOsn05n   ), 
                  /* S_ukMskPdv05 */ grp.Sum  (f => f.S_ukMskPdv05 ), 
                  /* S_ukMSK_05   */ grp.Sum  (f => f.S_ukMSK_05   ), 
                  /* K_NivVrj05   */ grp.Sum  (f => f./*K*/R_NivVrj05   ), 
                  /* X_ukPpmvIzn  */ grp.Sum  (f => ZXC.VvGet_25_of_100(f.S_ukPpmvOsn, f.S_ukPpmvSt1i2)), // Od 2017 je u PpmvOsn vec izracunani iznosPpmv a  st1i2 je 100 
                  /* S_ukOsnPNP   */ grp.Sum  (f => f.S_ukOsnPNP   ), 
                  /* S_ukIznPNP   */ grp.Sum  (f => f.S_ukIznPNP   ), 
                  /* S_ukMskPNP   */ grp.Sum  (f => f.S_ukMskPNP   ),
                  /* Skiz_ukKC    */ grp.Sum  (f => f.Skiz_ukKC    ),
                  /* Skiz_ukKCR   */ grp.Sum  (f => f.Skiz_ukKCR   ),
                  /* Skiz_ukRbt1  */ grp.Sum  (f => f.Skiz_ukRbt1  )

               ))
            .OrderBy         (sumarniFaktur => sumarniFaktur.SkladCD )
            .ThenBy          (sumarniFaktur => sumarniFaktur.DokDate )
            .ThenByDescending(sumarniFaktur => sumarniFaktur.IsNpCash)
            .ThenByDescending(sumarniFaktur => sumarniFaktur.S_ukKCRP)
            .ToList();

         #endregion fakturList

         // 20.02.2014: 
         #region IRM Prihod should go analitically, like on IRA

         if(ZXC.RRD.Dsc_IsAnaPrihodIRM)
         {
            for(int i = 0; i < fakturList.Count; ++i) // fakturList su hibridni grupirani IRMovi (po danu ili mjesecu) 
            {
               // pridruzi svakom faktur_rec-u (koji je zapravo suma IRMova) grupu pripadajucih rtransova 

               VvRptFilter rptFilter = new VvRptFilter();

               List<VvSqlFilterMember> IRMfilterMembers = new List<VvSqlFilterMember>(3);

               IRMfilterMembers.Add(new VvSqlFilterMember(RtrSch[RtrCI.t_tt], "elT_tt", Faktur.TT_IRM, " = "));
               if(irmSUMisDaily)
               {
                  IRMfilterMembers.Add(new VvSqlFilterMember(RtrSch[RtrCI.t_skladDate], "elT_sklad_date", fakturList[i].DokDate, " = "));
               }
               else // sum is monthly! 
               {
                  DateTime dateOd = new DateTime(fakturList[i].DokDate.Year, fakturList[i].DokDate.Month, 1);
                  DateTime dateDo = fakturList[i].DokDate; // u slucaju monthly sume DokDate ti je gore nastao kao zadnji dan u mjesecu, sto ti tu i pase kao dateDO 
                  IRMfilterMembers.Add(new VvSqlFilterMember(RtrSch[RtrCI.t_skladDate], false, "theDateOd", dateOd, "", "", " >= ", ""));
                  IRMfilterMembers.Add(new VvSqlFilterMember(RtrSch[RtrCI.t_skladDate], false, "theDateDo", dateDo, "", "", " <= ", ""));
               }

               rptFilter.FilterMembers = IRMfilterMembers;

               ZXC.RtransDao.LoadManyDocumentsTtranses(conn, fakturList[i].Transes, rptFilter, "t_skladDate, t_ttSort, t_ttNum, t_serial");

            }
         }

         #endregion IRM Prihod should go analitically, like on IRA

         #endregion Set fakturList from fakturList_IRM GROUPed BY dokDate OR month
#if _PUSE_
         Faktur groupedIrmAsFaktur_rec = new Faktur();

         ZXC.sqlErrNo = 0;

         anotherJoinClause = "LEFT JOIN ftrans Ft FORCE INDEX(BY_FakRecID) ON (MONTH(dokDate) = MONTH(Ft.t_dokDate) AND Ft.t_tt = 'IM') \n"; // da bi vidjeli nije li vec prebaceno 

         if(theRules.KtoShemaDsc.Dsc_MirSumMonthly == false)
         {
            anotherJoinClause = anotherJoinClause.Replace("MONTH", "");
         }

         using(XSqlCommand cmd = (VvSQL.Load_GroupedIRM_4Fak2Nal_Command(conn, ColumnListFromSumOnlyDataReader(true, "", true), filterMembers, anotherJoinClause, theRules)))
         {
            try
            {
               using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
               {
                  while(reader.HasRows && reader.Read())
                  {
                     groupedIrmAsFaktur_rec = new Faktur();

                     ZXC.FakturDao.FillFromSumOnlyDataReader(groupedIrmAsFaktur_rec, reader, true);

                     if(groupedIrmAsFaktur_rec.TT == Faktur.TT_IRM)
                     {
                        groupedIrmAsFaktur_rec.TtNum      = 0; // groupedIrmAsFaktur_rec.PdvNum;
                        groupedIrmAsFaktur_rec.KupdobName = "DnevniPromet (" + groupedIrmAsFaktur_rec.PdvNum + " IRM)";
                     }
                     fakturList.Add(groupedIrmAsFaktur_rec);
                  }
                  reader.Close();
               }
            }
            catch(XSqlException ex) { VvSQL.ReportSqlError("GetNeprebaceniFaktur2NalogList", ex, System.Windows.Forms.MessageBoxButtons.OK); ZXC.sqlErrNo = ex.Number; }
         } // using 
#endif
      } // else if(fak2nalSet == ZXC.Faktur2NalogSetEnum.IZLAZNI_MP)

      #endregion MALOPRODAJNI RACUNI Izlazni

      #region MALOPRODAJNE IZDATNICE - IZM

      else if((theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.IZLAZ_SK_MP) ||
              (theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT && theRules.ThisTT_Only == Faktur.TT_IZM))
      {
         needsRtranses = false;

         #region Get fakturList_IZM (analiticki IZM po IZM)

         List<Faktur> fakturList_IZM = new List<Faktur>();
         isIZM = true;

         string selectWhat = "L.*, R.*, K_NivVrj00, K_NivVrj10, K_NivVrj23, K_NivVrj25, K_NivVrj05, Ira_ROB_NV \n";
         string orderBy    = "dokDate, ttSort, ttNum";

         anotherJoinClause =
            "LEFT JOIN(                                                                         \n" +
            "  SELECT T.t_parentID,                                                             \n" +
            "  SUM(IF(t_pdvSt =  0, rtrIzlazKol*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj00, \n" +
            "  SUM(IF(t_pdvSt = 10, rtrIzlazKol*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj10, \n" +
            "  SUM(IF(t_pdvSt = 23, rtrIzlazKol*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj23, \n" +
            "  SUM(IF(t_pdvSt = 25, rtrIzlazKol*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj25, \n" +
            "  SUM(IF(t_pdvSt = 05, rtrIzlazKol*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj05, \n" +
            "  SUM(t_kol * rtrCijenaNBC                                        ) as Ira_ROB_NV  \n" + 
            "       FROM rtrans  T                                                              \n" +
            "  LEFT JOIN artstat A ON T.RecID = A.rtransRecID                                   \n" +
            "  WHERE T.t_isIrmUslug = 0                                                         \n" +
            "  GROUP BY T.t_parentID                                                            \n" +
            ") AS RTR ON L.RecID = RTR.t_parentID                                               \n\n";

         // NOT FOR IZM
         //anotherJoinClause += "LEFT JOIN ftrans Ft FORCE INDEX(BY_FakRecID) ON (MONTH(dokDate) = MONTH(Ft.t_dokDate) AND Ft.t_tt = 'IM') \n"; // da bi vidjeli nije li vec prebaceno 
         if(theRules.KtoShemaDsc.Dsc_MirSumMonthly == false)
         {
            //14.10.2013 :
          //anotherJoinClause = anotherJoinClause.Replace("MONTH", "");
            anotherJoinClause = anotherJoinClause.Replace("MONTH", "DATE");
         }

         /* !!! */ filterMembers.RemoveAt(0); // remo

         VvDaoBase.LoadGenericVvDataRecordList(conn, fakturList_IZM, filterMembers, "", orderBy, true, selectWhat, anotherJoinClause, true); // ovaj zadnji 'true' means isIRMforFak2Nal 

         #endregion Get fakturList_IZM (analiticki IZM po IZM)

         #region Set fakturList from fakturList_IZM GROUPed BY dokDate OR month

         // 03.10.2013: 
         foreach(Faktur fak in fakturList_IZM)
         {
            if(fak.IsVirmanIRM) // fak.NacPlac.ToLower().Contains("virman") || fak.NacPlac.ToLower().Contains("transakcijski") 
            {
               fak.NacPlac += " " + fak.TtNum.ToString("000000");

               fak.PrimPlatCD = fak.KupdobCD;
               fak.PrimPlatTK = fak.KupdobTK;
            }
         }

         #region nacPlacArray

         nacPlacArray =  fakturList_IZM
          .GroupBy(fak => irmSUMisDaily ?
             fak.SKL_DokDate_Full__AsString_And_NacPlac :
             fak.SKL_DokDate_Month_AsString_And_NacPlac)
            .Select(grp => new ZXC.VvUtilDataPackage(
               irmSUMisDaily ? grp.First().SKL_DokDate_Full__AsString/*_And_NacPlac*/ :
                               grp.First().SKL_DokDate_Month_AsString/*_And_NacPlac*/,
               grp.First().NacPlac,
               grp.Sum(f => f.S_ukKCRP),
               grp.First().IsNpCash,
               grp.First/*OrDefault*/(/*f => f.PrimPlatTK.NotEmpty()*/).PrimPlatTK,
               grp.First/*OrDefault*/(/*f => f.PrimPlatCD.NotZero ()*/).PrimPlatCD,
               grp.First().TipBr,
               grp.First().RecID,
               grp.First().SkladCD,
               DateTime.MinValue // DospDate 
            ))
            .OrderByDescending(nacPlac => nacPlac.TheBool   /*IsNpCash*/)
            .ThenByDescending (nacPlac => nacPlac.TheDecimal/*S_ukKCRP*/)
            .ToArray();

         #endregion nacPlacArray

         #region fakturList

         fakturList = fakturList_IZM
          .GroupBy(fak => irmSUMisDaily ? 
           //new { fak.DokDate,       fak.NacPlac } : // ovo ti je primjer kako LINQ grupira po vise od jednog podatka, ali ti tu nece sljakati zbog conditional operatora '?' koji 'oce isti type prije i poslije ':' 
           //new { fak.DokDate.Month, fak.NacPlac })
             fak.SKL_DokDate_Full__AsString/*_And_NacPlac*/ :
             fak.SKL_DokDate_Month_AsString/*_And_NacPlac*/)
            .Select(grp => new Faktur(

                  /* DokDate      */ irmSUMisDaily ? grp.First().DokDate : 
                     /* zadnji dan u mj */ new DateTime(grp.First().DokDate.Year, grp.First().DokDate.Month, DateTime.DaysInMonth(grp.First().DokDate.Year, grp.First().DokDate.Month)),
                  /* RecID        */ 0,
                  /* TT           */ Faktur.TT_IZM,
                  /* TtNum        */ grp.First().TtNum,
                  /* DospDate     */ DateTime.MinValue,
                  /* MtrosCD      */ theRules.KtoShemaDsc.Dsc_IsVisibleColMtrosCD ? Kupdob.GetKupdobFromSkladCD(grp.First().SkladCD).KupdobCD :  0,
                  /* MtrosTK      */ theRules.KtoShemaDsc.Dsc_IsVisibleColMtrosCD ? Kupdob.GetKupdobFromSkladCD(grp.First().SkladCD).Ticker   : "",
                  /* ProjektCD    */ "",
                  /* PdvKnjiga    */ ZXC.PdvKnjigaEnum.REDOVNA, 
                  /* ShouldFak2Nal*/ ZXC.ShouldFak2NalEnum.Knjizi_NORMALNO, 
                  /* Napomena     */ "IZM " + grp.Min(f => f.TtNum) + " - " + grp.Max(f => f.TtNum), 
                  /* V1_tt        */ "", 
                  /* V1_ttNum     */ 0, 
                  /* V2_tt        */ "", 
                  /* V2_ttNum     */ 0, 
                  /* V3_tt        */ "", 
                  /* V3_ttNum     */ 0, 
                  /* V4_tt        */ "", 
                  /* V4_ttNum     */ 0, 
                  /* PrimPlatCD   */ grp.First().PrimPlatCD         , 
                  /* PrimPlatTK   */ grp.First().PrimPlatTK         , 
                  /* KupdobCD     */ 0                              , // uvijek prazan 
                  /* KupdobTK     */ ""                             , // uvijek prazan  
                  /* SkladCD      */ grp.First().SkladCD            , 
                  /* S_ukKCRP     */ grp.Sum  (f => f.S_ukKCRP     ), 
                  /* S_ukOsn23m   */ grp.Sum  (f => f.S_ukOsn23m   ), 
                  /* S_ukOsn23n   */ grp.Sum  (f => f.S_ukOsn23n   ), 
                  /* S_ukOsn22m   */ grp.Sum  (f => f.S_ukOsn22m   ), 
                  /* S_ukOsn22n   */ grp.Sum  (f => f.S_ukOsn22n   ), 
                  /* S_ukOsn10m   */ grp.Sum  (f => f.S_ukOsn10m   ), 
                  /* S_ukOsn10n   */ grp.Sum  (f => f.S_ukOsn10n   ), 
                  /* Konto        */ grp.First().Konto              , 
                  /* S_ukPdv23m   */ grp.Sum  (f => f.S_ukPdv23m   ), 
                  /* S_ukPdv23n   */ grp.Sum  (f => f.S_ukPdv23n   ), 
                  /* S_ukPdv22m   */ grp.Sum  (f => f.S_ukPdv22m   ), 
                  /* S_ukPdv22n   */ grp.Sum  (f => f.S_ukPdv22n   ), 
                  /* S_ukPdv10m   */ grp.Sum  (f => f.S_ukPdv10m   ), 
                  /* S_ukPdv10n   */ grp.Sum  (f => f.S_ukPdv10n   ), 
                  /* VezniDok     */ grp.First().VezniDok           , 
                  /*              */                               
                  /* IsNpCash     */ grp.First().IsNpCash           , 
                  /* NacPlac      */ grp.First().NacPlac            , 
                  /* S_ukKCR      */ grp.Sum  (f => f.S_ukKCR      ), 
                  /* S_ukKCR_usl  */ grp.Sum  (f => f.S_ukKCR_usl  ), 
                  /* S_ukKCRP_usl */ grp.Sum  (f => f.S_ukKCRP_usl ), 
                  /* S_ukMSK_00   */ grp.Sum  (f => f.S_ukMSK_00   ), 
                  /* S_ukMSK_10   */ grp.Sum  (f => f.S_ukMSK_10   ), 
                  /* S_ukMSK_23   */ grp.Sum  (f => f.S_ukMSK_23   ), 
                  /* S_ukMskPdv10 */ grp.Sum  (f => f.S_ukMskPdv10 ), 
                  /* S_ukMskPdv23 */ grp.Sum  (f => f.S_ukMskPdv23 ), 
                  /* K_NivVrj00   */ grp.Sum  (f => f.K_NivVrj00   ), 
                  /* K_NivVrj10   */ grp.Sum  (f => f.K_NivVrj10   ), 
                  /* K_NivVrj23   */ grp.Sum  (f => f.K_NivVrj23   ), 
                  /* Ira_ROB_NV   */ grp.Sum  (f => f.Ira_ROB_NV   ),
                                                                   
                  /* S_ukPdv25m   */ grp.Sum  (f => f.S_ukPdv25m   ), 
                  /* S_ukPdv25n   */ grp.Sum  (f => f.S_ukPdv25n   ), 
                  /* S_ukOsn25m   */ grp.Sum  (f => f.S_ukOsn25m   ), 
                  /* S_ukOsn25n   */ grp.Sum  (f => f.S_ukOsn25n   ), 
                  /* S_ukMskPdv25 */ grp.Sum  (f => f.S_ukMskPdv25 ), 
                  /* S_ukMSK_25   */ grp.Sum  (f => f.S_ukMSK_25   ), 
                  /* K_NivVrj25   */ grp.Sum  (f => f.K_NivVrj25   ), 
                                                                   
                  /* S_ukPdv05m   */ grp.Sum  (f => f.S_ukPdv05m   ), 
                  /* S_ukPdv05n   */ grp.Sum  (f => f.S_ukPdv05n   ), 
                  /* S_ukOsn05m   */ grp.Sum  (f => f.S_ukOsn05m   ), 
                  /* S_ukOsn05n   */ grp.Sum  (f => f.S_ukOsn05n   ), 
                  /* S_ukMskPdv05 */ grp.Sum  (f => f.S_ukMskPdv05 ), 
                  /* S_ukMSK_05   */ grp.Sum  (f => f.S_ukMSK_05   ), 
                  /* K_NivVrj05   */ grp.Sum  (f => f.K_NivVrj05   ), 
                  /* X_ukPpmvIzn  */ grp.Sum  (f => ZXC.VvGet_25_of_100(f.S_ukPpmvOsn, f.S_ukPpmvSt1i2)), // Od 2017 je u PpmvOsn vec izracunani iznosPpmv a  st1i2 je 100 
                  /* S_ukOsnPNP   */ grp.Sum  (f => f.S_ukOsnPNP   ), 
                  /* S_ukIznPNP   */ grp.Sum  (f => f.S_ukIznPNP   ), 
                  /* S_ukMskPNP   */ grp.Sum  (f => f.S_ukMskPNP   ),
                  /* Skiz_ukKC    */ grp.Sum  (f => f.Skiz_ukKC    ),
                  /* Skiz_ukKCR   */ grp.Sum  (f => f.Skiz_ukKCR   ),
                  /* Skiz_ukRbt1  */ grp.Sum  (f => f.Skiz_ukRbt1  )

               ))
            .OrderBy         (sumarniFaktur => sumarniFaktur.DokDate)
            .ThenByDescending(sumarniFaktur => sumarniFaktur.IsNpCash)
            .ThenByDescending(sumarniFaktur => sumarniFaktur.S_ukKCRP)
            .ToList();

         #endregion fakturList

         #endregion Set fakturList from fakturList_IZM GROUPed BY dokDate OR month

      } // else if(fak2nalSet == ZXC.Faktur2NalogSetEnum.IZLAZ_SK_MP ... IZM)

      #endregion MALOPRODAJNE IZDATNICE - IZM

      #region Get fakturList MALOPRODAJNI RACUNI Ulazni, URM-UFM-VMU-ZPC

      else if((theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.ULAZNI_MP) ||
              (theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT && theRules.ThisTT_Only == Faktur.TT_URM) ||
              (theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT && theRules.ThisTT_Only == Faktur.TT_UFM) ||
              (theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT && theRules.ThisTT_Only == Faktur.TT_VMI) || // VMI je u biti knjizenje VMUa 
              (theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT && theRules.ThisTT_Only == Faktur.TT_TRI) || // TRI je u biti knjizenje TRMa 
              (theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT && theRules.ThisTT_Only == Faktur.TT_ZPC))
      {
       //needsRtranses = false; // !!! 
       //needsRtranses = (theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.VMI); // !!! dakle, rtrans trebamo samo za VMI  
         needsRtranses = (theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.VMI || theRules.KtoShemaDsc.Dsc_KnjiziMSK_ulaz == false);

         anotherJoinClause = "LEFT JOIN ftrans Ft FORCE INDEX(BY_FakRecID) ON F.RecID = Ft.t_fakRecID \n"; // da bi vidjeli nije li vec prebaceno 

         Load_Fak2Nal_URM_FakturList(conn, fakturList, filterMembers, anotherJoinClause);

         // 02.10.2013: 
         anotherJoinClause = anotherJoinClause.Replace("F.RecID", "L.RecID"); 
      }

      #endregion Get fakturList MALOPRODAJNI RACUNI Ulazni, URM-UFM-VMU-NIV-ZPC

      #region Get fakturList VELEPRODAJNI RACUNI Ulazni / Izlazni, Blagajne, any 'OneExactTT' except 'IRM'

      else
      {
         // faktur_rec.TT == Faktur.TT_UPL || faktur_rec.TT == Faktur.TT_ISP || faktur_rec.TT == Faktur.TT_BUP || faktur_rec.TT == Faktur.TT_BIS;
         bool IsBLAGAJNA = theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.BLAGAJNA;
         
         string selectWhat = "L.*, R.*";
         string orderBy = "dokDate , ttSort, ttNum";

         // 09.09.2019: hztk oce sort na dan po unosu 
         if(IsBLAGAJNA && ZXC.RRD.Dsc_IsBlgOrderByDokNum)
         {
            orderBy = "dokDate, dokNum ";
         }

         anotherJoinClause = @"LEFT JOIN ftrans Ft FORCE INDEX(BY_FakRecID) ON L.RecID = Ft.t_fakRecID AND Ft.t_tt != 'IZ'" + "\n"; // da bi vidjeli nije li vec prebaceno 

         VvDaoBase.LoadGenericVvDataRecordList(conn, fakturList, filterMembers, "", orderBy, true, selectWhat, anotherJoinClause);
      }

      #endregion VELEPRODAJNI RACUNI Ulazni / Izlazni

      #region Rtranses & Artstat & Join

      if(needsRtranses)
      {
         #region Get rtransList with ArtstatEx

         RtransDao.GetRtransWithArtstatList(conn, rtransList, anotherJoinClause, filterMembers, "");

         #endregion Rtrans with ArtstatEx

         #region local JOIN faktur with rtranses

         Parallel.ForEach(fakturList, faktur =>
         {
            if(isIRM)
            {
               if(irmSUMisDaily)
               {
                  faktur.Transes = rtransList.Where(rtr => rtr.T_skladDate == faktur.DokDate).ToList();
               }
               else
               {
                  faktur.Transes = rtransList.Where(rtr => rtr.T_skladDate.Month == faktur.DokDate.Month).ToList();
               }
            }
            else
            {
               faktur.Transes = rtransList.Where(rtr => rtr.T_parentID == faktur.RecID).ToList();
            }
         }
         );

         #endregion local JOIN faktur with rtranses
      }

      #endregion Rtranses & Artstat & Join

      return fakturList;
   }

   private static List<VvSqlFilterMember> GetFilterMembers4Fak2Nal(Faktur2NalogRulesAndData theRules)
   {
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>();

      // 13.02.2017: dodan if 
      if(theRules.IsIraRealizOnly == false)
      {
         filterMembers.Add(new VvSqlFilterMember("Ft.t_parentID", "NULL", " IS ")); // ovaj, dakle, blokira vec prebacene 
      }

      #region Which TT

      if(theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.ULAZNI_VP   ||
         theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.ULAZNI_MP   ||
         theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP ||
         theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.PROIZ_IZLAZ ||
         theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.BLAGAJNA)
      {
         string IN_clause = "";
         switch(theRules.Fak2nalSet)
         {
            case ZXC.Faktur2NalogSetEnum.ULAZNI_VP  : IN_clause = TtInfo. UlazniPdv_IN_Clause.Replace("'URM', ", "")   .Replace("'UFM', ", ""); break; // UlazPDV: URA, URM, UFA, UFM, UOD, UPV,
            case ZXC.Faktur2NalogSetEnum.ULAZNI_MP  : IN_clause = TtInfo.UlazniMALOP_Fak2Nal_IN_Clause/*.Replace("VMU", "VMI")*/;               break; // URM-UFM-VMUne-NIV-ZPC                   
            case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP: IN_clause = TtInfo.IzlazniPdv_IN_Clause.Replace("'IRM', ", "")/*.Replace(", 'UPA'", "")*/;break;
            case ZXC.Faktur2NalogSetEnum.BLAGAJNA   : IN_clause = TtInfo.  Blagajna_IN_Clause  ;                                                break;
            case ZXC.Faktur2NalogSetEnum.PROIZ_IZLAZ: IN_clause = TtInfo.ProizvodnjaIzlaz_IN_Clause;                                            break;
         }
         // NOTA BENE! TtInfo.GetSql_IN_Clause ne sljaka kada ide kao parametar, javi syntax error!                                                                             
         filterMembers.Add(new VvSqlFilterMember("tt", IN_clause, " IN "));
      }
      else if(theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP)
      {
         filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.tt], false, "theTT", "IRM", "", "", "  = ", ""));
      }
      else if(theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.IZLAZ_SK_MP)
      {
         filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.tt], false, "theTT", "IZM", "", "", "  = ", ""));
      }
      else if(theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.VMI)
      {
       //filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.tt],                          false, "theTT" , "VMI", "", "", "  = ", ""));
         filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.tt], ZXC.FM_OR_Enum.OPEN_OR , false, "theTT" , "VMI", "", "", "  = ", ""));
         filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.tt], ZXC.FM_OR_Enum.CLOSE_OR, false, "theTT2", "TRI", "", "", "  = ", ""));
      }
      else if(theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT)
      {
         filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.tt], false, "theTT", theRules.ThisTT_Only, "", "", "  = ", ""));
      }
      else if(theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.IRA_RealizOnly)
      {
         filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.tt], false, "theTT", "IRA", "", "", "  = ", ""));
      }
      else throw new Exception("FakturDao.GetFilterMembers4Fak2Nal(): WhichTT undefined! [" + theRules.Fak2nalSet + "]");

      #endregion Which TT

      #region TimeRule OR DateOdDo

      if(theRules.PeriodDefinedVia_DokDate)
      {
         filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.dokDate], false, "theDateOd", theRules.DateOd, "", "", " >= ", ""));
         filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.dokDate], false, "theDateDo", theRules.DateDo, "", "", " <= ", ""));
      }
      else if(theRules.PeriodDefinedVia_AddTS)
      {
         DateTime tsOlderThen;

         if(theRules.Fak2nalTimeRule == ZXC.Faktur2NalogTimeRuleEnum.DoIt_For_AddTS_NotToday)
         {
            // ovo treba postati ponoc danasnjeg dana, pocetak ovog dana)
            tsOlderThen = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.addTS], false, "theAddTS", tsOlderThen, "", "", " < ", "", "L"));
         }
         else if(theRules.Fak2nalTimeRule == ZXC.Faktur2NalogTimeRuleEnum.DoIt_For_AddTS_OneHourOld)
         {
            tsOlderThen = DateTime.Now.AddHours(-1);

            filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.addTS], false, "theAddTS", tsOlderThen, "", "", " < ", "", "L"));
         }
      }
      else throw new Exception("FakturDao.GetFilterMembers4Fak2Nal(): PeriodDefinedVia_ undefined!");

      #endregion TimeRule OR DateOdDo

      return filterMembers;
   }

   #region Sam Lokal Propertiz

   private static DataRowCollection    ArtSch   { get { return ZXC.ArtiklDao.TheSchemaTable.Rows;  } }
   private static ArtiklDao.ArtiklCI   ArtCI    { get { return ZXC.ArtiklDao.CI;                   } }
   private static DataRowCollection    ArsSch   { get { return ZXC.ArtStatDao.TheSchemaTable.Rows; } }
   private static ArtStatDao.ArtStatCI ArsCI    { get { return ZXC.ArtStatDao.CI;                  } }
   private static DataRowCollection    FakSch   { get { return ZXC.FakturDao.TheSchemaTable.Rows;  } }
   private static FakturDao.FakturCI   FakCI    { get { return ZXC.FakturDao.CI;                   } }
   private static DataRowCollection    FakExSch { get { return ZXC.FaktExDao.TheSchemaTable.Rows;  } }
   private static FaktExDao.FaktExCI   FakExCI  { get { return ZXC.FaktExDao.CI;                   } }
   private static DataRowCollection    RtrSch   { get { return ZXC.RtransDao.TheSchemaTable.Rows;  } }
   private static RtransDao.RtransCI   RtrCI    { get { return ZXC.RtransDao.CI;                   } }
   private static DataRowCollection    FtrSch   { get { return ZXC.FtransDao.TheSchemaTable.Rows;  } }
   private static FtransDao.FtransCI   FtrCI    { get { return ZXC.FtransDao.CI;                   } }

   #endregion Sam Lokal Propertiz
   //private static List<VvSqlFilterMember> GetFilterMembers4Fak2Nal_ViaDateOdDo(ZXC.Faktur2NalogSetEnum fak2nalSet, DateTime dateOd, DateTime dateDo)
   //{
   //   List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(4);

   //   filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.tt     ], false, "theTT"    , "IRM" , "", "", "  = ", ""));

   //   filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.dokDate], false, "theDateOd", dateOd, "", "", " >= ", ""));
   //   filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.dokDate], false, "theDateDo", dateDo, "", "", " <= ", ""));

   //   filterMembers.Add(new VvSqlFilterMember("Ft.t_parentID", "NULL", " IS ")); // ovaj, dakle, blokira vec prebacene 

   //   return filterMembers;
   //}
   //private static List<VvSqlFilterMember> GetFilterMembers4Fak2Nal_ViaTimeRule(ZXC.Faktur2NalogSetEnum fak2nalSet, ZXC.Faktur2NalogTimeRuleEnum fak2nalTimeRule)
   //{
   //   List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>();
   //   string IN_clause = "";
   //   DateTime tsOlderThen;

   //   switch(fak2nalSet)
   //   {
   //      case ZXC.Faktur2NalogSetEnum.ULAZNI    : IN_clause = TtInfo. UlazniPdv_IN_Clause;                             break;
   //      case ZXC.Faktur2NalogSetEnum.IZLAZNI_VP: IN_clause = TtInfo.IzlazniPdv_IN_Clause.Replace("'IRM', ", "");      break;
   //      case ZXC.Faktur2NalogSetEnum.IZLAZNI_MP: IN_clause = TtInfo.GetSql_IN_Clause(new string[] { Faktur.TT_IRM }); break;
   //    //case ZXC.Faktur2NalogSetEnum.BLAGAJNA  : throw new Exception("FakturDao.GetNeprebaceniFaktur2NalogList(): Faktur2NalogSetEnum.BLAGAJNA UNDONE!");

   //      default: throw new Exception("FakturDao.GetFilterMembers4Fak2Nal_ViaTimeRule(): Faktur2NalogSetEnum: [" + fak2nalSet + "] is wrong!");

   //   }
   //   filterMembers.Add(new VvSqlFilterMember("tt", IN_clause, " IN "));

   //   if(fak2nalTimeRule == ZXC.Faktur2NalogTimeRuleEnum.DoIt_For_AddTS_NotToday)
   //   {
   //      // ovo treba postati ponoc danasnjeg dana, pocetak ovog dana)
   //      tsOlderThen = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

   //      filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.addTS], false, "theAddTS", tsOlderThen, "", "", " < ", "", "L"));
   //   }
   //   else if(fak2nalTimeRule == ZXC.Faktur2NalogTimeRuleEnum.DoIt_For_AddTS_OneHourOld)
   //   {
   //      tsOlderThen = DateTime.Now.AddHours(-1);

   //      filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.addTS], false, "theAddTS", tsOlderThen, "", "", " < ", "", "L"));
   //   }

   //   //filterMembers.Add(new VvSqlFilterMember(FtrSch[FtrCI.t_fakRecID], false, "thefakRecID", "NULL", "", "", " IS ", ""));
   //   filterMembers.Add(new VvSqlFilterMember("Ft.t_parentID", "NULL", " IS ")); // ovaj, dakle, blokira vec prebacene 

   //   return filterMembers;
   //}

   #endregion GetNeprebaceniFaktur2NalogList

   public static void ExecuteFaktur2Nalog(XSqlConnection conn, List<Faktur> fakturList, Faktur2NalogRulesAndData theRules, ZXC.VvUtilDataPackage[] nacPlacArray)
   {

      if(fakturList.Count.IsZero()) return;

      // 25.10.2021: if dodan tek sada 
      if(ZXC.TheVvForm.TheVvUC != null)
      {
         ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kplan>(null, VvSQL.SorterType.Code);
      }

      #region lokal variablez

      DateTime currDate = fakturList[0].DokDate.Date;

      int presumedNumOfNewNalog = fakturList./*Where(f => f.S_ukKCRP.NotZero()).*/Select(f => f.DokDate.Date).Distinct().Count();

      ushort line = 0;

      string origDatabase = conn.Database;
      ZXC.luiListaSkladista.LazyLoad(); // za sklad konto 
      conn.ChangeDatabase(origDatabase);

      #endregion lokal variablez

      theRules.NalogCount = 0;

      foreach(Faktur faktur_rec in fakturList)
      {
         // new nalog_rec: 
         if(currDate != faktur_rec.DokDate.Date) 
         {
            if(theRules.IsBLAGAJNA)
            {
/*E*/          Send2Nalog_BlagCurrDateSum(conn, ref line, theRules, fakturList.Where(fak => fak.DokDate.Date == currDate).ToList());
            }

            line = 0;
            currDate = faktur_rec.DokDate.Date; 
         }

         if(line == 0) theRules.NalogCount++; 

         theRules.SetCommonFtransPropertiz(conn, GetRasterID(fakturList), presumedNumOfNewNalog, faktur_rec);

         if(theRules.FtransCarinaKind == ZXC.ShouldFak2NalEnum.Knjizi_NISTA) { theRules.NalogCount--; continue; } // skip this faktur_rec 

         // VOILA --- this is it! 

// 13.02.2017: 
/* S0 */ if(theRules.IsIraRealizOnly)
         {
            Send2Nalog_Realizac(conn, ref line, theRules, faktur_rec);
            continue; // !!! 
         }
//--------------------------------------------------------------------------------------------------------------------------------------- 
/* A */  if(theRules.NeedsKupdobLine  && 
            theRules.IsIRM == false   &&
       faktur_rec.TT != Faktur.TT_UPA &&
       faktur_rec.TT != Faktur.TT_ZPC) Send2Nalog_Kupdob    (conn, ref line, theRules, faktur_rec);
//--------------------------------------------------------------------------------------------------------------------------------------- 
/* M */  if(theRules.IsIRM           ) Send2Nalog_Kupdob_IRM(conn, ref line, theRules, faktur_rec, nacPlacArray);
//--------------------------------------------------------------------------------------------------------------------------------------- 
/* B */  if(theRules.IsINTERN_UIorVMI) Send2Nalog_Sklad     (conn, ref line, theRules, faktur_rec); // ovo, dakle, NE ide za klasicne racune vec za, VMI, TRI, predatnice, izdatnice na mtr, medjuskladisnice, ... 
//--------------------------------------------------------------------------------------------------------------------------------------- 
/* C */  if(theRules.IsVMI == false &&
            theRules.IsIRM == false &&
            theRules.IsIZM == false &&
     faktur_rec.TT != Faktur.TT_UPA &&
            theRules.IsIZD == false  ) Send2Nalog_PrihTros  (conn, ref line, theRules, faktur_rec);
//--------------------------------------------------------------------------------------------------------------------------------------- 
/* M0*/  if(theRules.IsIRM /*|| 
            theRules.IsIZM*/)          Send2Nalog_Prihod_IRM(conn, ref line, theRules, faktur_rec);
//--------------------------------------------------------------------------------------------------------------------------------------- 
/* D */  if(theRules.IsVMI == false &&
            theRules.IsIZM == false &&
            theRules.IsMVI == false &&
ZXC.CURR_prjkt_rec.IsNeprofit == false)Send2Nalog_Pdv       (conn, ref line, theRules, faktur_rec);
//--------------------------------------------------------------------------------------------------------------------------------------- 
/* M1*/  if(theRules.IsMALOP_UIorVMI ) Send2Nalog_Malop     (conn, ref line, theRules, faktur_rec);
//--------------------------------------------------------------------------------------------------------------------------------------- 
/* S */  if(theRules.IsIRA ||
            theRules.IsIZD  )          Send2Nalog_Realizac  (conn, ref line, theRules, faktur_rec);
//--------------------------------------------------------------------------------------------------------------------------------------- 
/* M2*/  if(theRules.IsIZM && 
theRules.KtoShemaDsc.Dsc_KnjiziMSK_izlaz == false) 
                                       Send2Nalog_RealizcMal(conn, ref line, theRules, faktur_rec);
//--------------------------------------------------------------------------------------------------------------------------------------- 

      } // foreach(Faktur faktur_rec in fakturList) 

         // 'za zadnjega' 
/*E*/ if(theRules.IsBLAGAJNA) Send2Nalog_BlagCurrDateSum(conn, ref line, theRules, fakturList.Where(fak => fak.DokDate.Date == currDate).ToList());

   }

   private static void Send2Nalog_Malop          (XSqlConnection conn, ref ushort line, Faktur2NalogRulesAndData theRules, Faktur faktur_rec)
   {
      if(theRules.IsMALOP_UorVMI && theRules.KtoShemaDsc.Dsc_KnjiziMSK_ulaz  == false) return; 
      if(theRules.IsMALOP_I      && theRules.KtoShemaDsc.Dsc_KnjiziMSK_izlaz == false) return;

      string origTipBr = theRules.FtransTipBr;
      string nivTipBr  = "NIV-" + origTipBr;
      bool isUlaz      = theRules.IsMALOP_UorVMI;

      // eventualna NIVELACIJA - START                           

      if(ZXC.RRD.Dsc_IsSupressSHADOWing == false)
      {
         if(theRules.IsIRM || theRules.IsIZM) theRules.FtransOpis = "Nivelacija pri razduženju skladišta";

         // 04.11.2016: !!! BIG NEWS !!! : 
         // SVE faktur_rec.K_Niv dobile utjecajnika 'korekcKoef' kako bi eventualno promijenile predznak (*-1)
         // 
         decimal korekcKoef = isUlaz ? 1.00M : - 1.00M;

         theRules.FtransTipBr = nivTipBr;
         theRules.SetDugAndPot( isUlaz, /*isUlaz ? faktur_rec.R_NivVrj25 :*/ korekcKoef * faktur_rec.K_NivVrj25      ); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MSK_25   ; Send2Nalog(conn, ref line, theRules, false);
         theRules.SetDugAndPot( isUlaz, /*isUlaz ? faktur_rec.R_NivVrj23 :*/ korekcKoef * faktur_rec.K_NivVrj23      ); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MSK_23   ; Send2Nalog(conn, ref line, theRules, false);
         theRules.SetDugAndPot( isUlaz, /*isUlaz ? faktur_rec.R_NivVrj10 :*/ korekcKoef * faktur_rec.K_NivVrj10      ); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MSK_10   ; Send2Nalog(conn, ref line, theRules, false);
         theRules.SetDugAndPot( isUlaz, /*isUlaz ? faktur_rec.R_NivVrj05 :*/ korekcKoef * faktur_rec.K_NivVrj05      ); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MSK_05   ; Send2Nalog(conn, ref line, theRules, false);
         theRules.SetDugAndPot( isUlaz, /*isUlaz ? faktur_rec.R_NivVrj00 :*/ korekcKoef * faktur_rec.K_NivVrj00      ); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MSK_00   ; Send2Nalog(conn, ref line, theRules, false);

         if(theRules.IsIRM || theRules.IsIZM) theRules.FtransOpis = "Nivelacija uračunatog PDVa u razduženju skladišta";

         theRules.SetDugAndPot(!isUlaz, /*isUlaz ? faktur_rec.R_NivMskPdv25 :*/ korekcKoef * faktur_rec.K_NivMskPdv25); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MskPdv_25; Send2Nalog(conn, ref line, theRules, false);
         theRules.SetDugAndPot(!isUlaz, /*isUlaz ? faktur_rec.R_NivMskPdv23 :*/ korekcKoef * faktur_rec.K_NivMskPdv23); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MskPdv_23; Send2Nalog(conn, ref line, theRules, false);
         theRules.SetDugAndPot(!isUlaz, /*isUlaz ? faktur_rec.R_NivMskPdv10 :*/ korekcKoef * faktur_rec.K_NivMskPdv10); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MskPdv_10; Send2Nalog(conn, ref line, theRules, false);
         theRules.SetDugAndPot(!isUlaz, /*isUlaz ? faktur_rec.R_NivMskPdv05 :*/ korekcKoef * faktur_rec.K_NivMskPdv05); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MskPdv_05; Send2Nalog(conn, ref line, theRules, false);

         if(theRules.IsIRM || theRules.IsIZM) theRules.FtransOpis = "Nivelacija uračunatog poreza na potrošnju u razduženju skladišta";
         theRules.SetDugAndPot(!isUlaz, /*isUlaz ? faktur_rec.R_NivMrz      :*/ korekcKoef * faktur_rec.K_NivMskPNP); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_Msk_PNP; Send2Nalog(conn, ref line, theRules, false);

         if(theRules.IsIRM || theRules.IsIZM) theRules.FtransOpis = "Nivelacija uračunate marže u razduženju skladišta";
         theRules.SetDugAndPot(!isUlaz, /*isUlaz ? faktur_rec.R_NivMrz      :*/ korekcKoef * faktur_rec.K_NivMrz); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_Mrz; Send2Nalog(conn, ref line, theRules, false);
      }

      // eventualna NIVELACIJA - END                           

      // Classic - START 

      if(theRules.IsIRM || theRules.IsIZM) theRules.FtransOpis = "Razduženje skladišta";

      theRules.FtransTipBr = origTipBr;
      theRules.SetDugAndPot( isUlaz, faktur_rec.S_ukMSK_25   ); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MSK_25   ; Send2Nalog(conn, ref line, theRules, false);
      theRules.SetDugAndPot( isUlaz, faktur_rec.S_ukMSK_23   ); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MSK_23   ; Send2Nalog(conn, ref line, theRules, false);
      theRules.SetDugAndPot( isUlaz, faktur_rec.S_ukMSK_10   ); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MSK_10   ; Send2Nalog(conn, ref line, theRules, false);
      theRules.SetDugAndPot( isUlaz, faktur_rec.S_ukMSK_05   ); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MSK_05   ; Send2Nalog(conn, ref line, theRules, false);
      theRules.SetDugAndPot( isUlaz, faktur_rec.S_ukMSK_00   ); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MSK_00   ; Send2Nalog(conn, ref line, theRules, false);

      if(theRules.IsIRM || theRules.IsIZM) theRules.FtransOpis = "Uračunati PDV u razduženju skladišta";

      theRules.SetDugAndPot(!isUlaz, faktur_rec.S_ukMskPdv25 ); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MskPdv_25; Send2Nalog(conn, ref line, theRules, false);
      theRules.SetDugAndPot(!isUlaz, faktur_rec.S_ukMskPdv23 ); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MskPdv_23; Send2Nalog(conn, ref line, theRules, false);
      theRules.SetDugAndPot(!isUlaz, faktur_rec.S_ukMskPdv10 ); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MskPdv_10; Send2Nalog(conn, ref line, theRules, false);
      theRules.SetDugAndPot(!isUlaz, faktur_rec.S_ukMskPdv05 ); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_MskPdv_05; Send2Nalog(conn, ref line, theRules, false);
      theRules.SetDugAndPot(!isUlaz, faktur_rec.S_ukMrz      ); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_Mrz      ; Send2Nalog(conn, ref line, theRules, false);

      if(theRules.IsIRM || theRules.IsIZM) theRules.FtransOpis = "Uračunati porez na potrošnju u razduženju skladišta";
      theRules.SetDugAndPot(!isUlaz, faktur_rec.S_ukMskPNP); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_Msk_PNP; Send2Nalog(conn, ref line, theRules, false);

      // Classic - END 

      if(theRules.IsIRM || theRules.IsIZM)
      {
         theRules.FtransOpis = "Uračunata marža u razduženju skladišta";
         theRules.SetDugAndPot(true, faktur_rec.K_ukMskMrz); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_Mrz;             Send2Nalog(conn, ref line, theRules, false);

         theRules.FtransOpis = "Nabavna vrijednost prodane robe";
         theRules.SetDugAndPot(true, faktur_rec.Ira_ROB_NV); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_Kto_Realizacija; Send2Nalog(conn, ref line, theRules, false);
      }      
   }
   private static void Send2Nalog_Prihod_IRM     (XSqlConnection conn, ref ushort line, Faktur2NalogRulesAndData theRules, Faktur faktur_rec)
   {
      //25.10.2021.za firme koje knjize samo radi IOS-a
      if(ZXC.KSD.Dsc_IsOnlyIOSknjizenje) return;


         // 20.02.2014: IRM Prihod should go analitically, like on IRA 
         if(ZXC.RRD.Dsc_IsAnaPrihodIRM)
      {
         /*return*/ Send2Nalog_PrihTros(conn, ref line, theRules, faktur_rec);
         return;
      }

      theRules.FtransOpis = "Prihod od prodaje robe"  ; theRules.SetDugAndPot(false, faktur_rec.R_ukKCR_rob); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_KtoOsn_Mir  ; Send2Nalog(conn, ref line, theRules, false);
      theRules.FtransOpis = "Prihod od prodaje usluga"; theRules.SetDugAndPot(false, faktur_rec.S_ukKCR_usl); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_KtoIrmOsnUsl; Send2Nalog(conn, ref line, theRules, false);
      theRules.FtransOpis = "Poseban porez na MV"     ; theRules.SetDugAndPot(false, faktur_rec.X_ukPpmvIzn); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_KtoPpmv     ; Send2Nalog(conn, ref line, theRules, false);
   }
   private static void Send2Nalog_PrihTros       (XSqlConnection conn, ref ushort line, Faktur2NalogRulesAndData theRules, Faktur faktur_rec)
   {
      //25.10.2021.za firme koje knjize samo radi IOS-a
      if(ZXC.KSD.Dsc_IsOnlyIOSknjizenje) return;

      #region Fill T_konto if IsEmpty

      //foreach(Rtrans rtrans_rec in faktur_rec.Transes)
      foreach(Rtrans rtrans_rec in faktur_rec.TransesWOtwins)
      {
         if(rtrans_rec.T_konto.IsEmpty()) rtrans_rec.T_konto = theRules.GetKonto_PrihTros(faktur_rec, rtrans_rec);

         // 13.03.2014: 
         if(rtrans_rec.T_mtrosCD.IsZero()) rtrans_rec.T_mtrosCD = faktur_rec.MtrosCD;
      }

      #endregion Fill T_konto if IsEmpty

      // 05.03.2021: do sada je bilo da NE grupira samo blagajnu,  a sada dodajemo i 'po zelji' ali samo za ulazne 
    //bool shouldGroupByKonto = theRules.IsBLAGAJNA == false                                                    ; // NE grupiraj blagajnu 

      bool                                                                           shouldGroupByKonto = true ; // DA grupiraj po kontu prih/tros 
      if(theRules.IsBLAGAJNA)                                                        shouldGroupByKonto = false; // NE grupiraj po kontu prih/tros 
      if(theRules.KtoShemaDsc.Dsc_IsNeGrupTrosak && faktur_rec.TtInfo.IsUlazniPdvTT) shouldGroupByKonto = false; // NE grupiraj po kontu      tros 

      bool shouldGroupUFAbyPRD = theRules.KtoShemaDsc.Dsc_MirGroupByNacPlac;

    //List<Rtrans> rtransList4PrihodTrosak = faktur_rec.Transes
      List<Rtrans> rtransList4PrihodTrosak = faktur_rec.TransesWOtwins
         .Where(rtr => rtr.R_Kol_Puta_PrNabCij.NotZero() ||
                       rtr.R_KCR.NotZero())
         .ToList();

      // 28.05.2012: za theRules.IsSkip_NonMAT izbaci one koji nisu MAT
      // IsSKIP_NonMAT = faktur_rec.TT == Faktur.TT_PPR || faktur_rec.TT == Faktur.TT_POV;
      if(theRules.IsSKIP_NonMAT)
      {
       //rtransList4PrihodTrosak = faktur_rec.Transes
         rtransList4PrihodTrosak = faktur_rec.TransesWOtwins
         .Where(rtr => rtr.A_IsMaterijal)
         .ToList();
      }

      // 13.03.2014: 
    //var rtransGroupByKonto         = /*faktur_rec.Transes*/rtransList4PrihodTrosak.GroupBy(rtr => (shouldGroupByKonto ? rtr.T_konto                 : rtr.T_recID.ToString())).Select(grp => new
      var rtransGroupByKontoAndMtros = /*faktur_rec.Transes*/rtransList4PrihodTrosak.GroupBy(rtr => (shouldGroupByKonto ? rtr.T_konto + rtr.T_mtrosCD : rtr.T_recID.ToString())).Select(grp => new
         {
            // 13.03.2014: 
          //konto      = (shouldGroupByKonto ? grp.Key : grp.First().T_konto),
            konto      = (                               grp.First().T_konto  ),
            mtrosCD    = (                               grp.First().T_mtrosCD),
            
            rtrFrsRecID= grp.First().T_recID, // za FtransOpis kada je blagajna, gdje zapravo nisu grupirani 
            
            kontoMoney = grp.Sum
            (
               rtr => 
                  (faktur_rec.ShouldFak2Nal == ZXC.ShouldFak2NalEnum.Knjizi_SamoPDV_i_Carinu ? 

                     (rtr.R_pdv.NotZero() ? 0.00M : rtr.TtInfo.ProposeCijenaKind == ZXC.TtProposeCijenaKindEnum.Propose_PrNabCij ? rtr.R_Kol_Puta_PrNabCij :                                                rtr.R_KCR) : 
//                   (                              rtr.TtInfo.ProposeCijenaKind == ZXC.TtProposeCijenaKindEnum.Propose_PrNabCij ? rtr.R_Kol_Puta_PrNabCij :                                                rtr.R_KCR)
                     (                              rtr.TtInfo.ProposeCijenaKind == ZXC.TtProposeCijenaKindEnum.Propose_PrNabCij ? rtr.R_Kol_Puta_PrNabCij : (ZXC.CURR_prjkt_rec.IsNeprofit) ? rtr.R_KCRP : rtr.R_KCR)
                  )
            ), // ako je dirCarina i IMA pdv; NE sumiraj tu osnovicu, dirCarina koja NEMA pdv se tretira normalno 

            kontoCount = grp.Count(),

         });

      string origOpis = theRules.FtransOpis;


      // 13.03.2014: 
    //foreach(var ktoGrp       in rtransGroupByKonto        )
      foreach(var ktoAndMtrGrp in rtransGroupByKontoAndMtros)
      {
         //theRules.FtransOpis += theRules.AddStringAndSeparator(ktoGrp.kontoCount.ToString() + " red.");
         if(ktoAndMtrGrp.kontoCount > 1)
         {
            theRules.FtransOpis += (theRules.FtransOpis.NotEmpty() ? "/ suma " : "suma ") + ktoAndMtrGrp.kontoCount.ToString() + " red.";
         }
         else
         {
            // ovo Single pretvoreno u First nakon sto si skuzio da zbog onog gore '06.03.2012' moze biti vise transova ali nisu usli u rtransList4PrihodTrosak zbog KCR = 0 
            if(shouldGroupByKonto)
             //theRules.FtransOpis += " [" + faktur_rec.Transes       ./*Single*/First(rtr => rtr.T_konto == ktoAndMtrGrp.konto)      .T_artiklName + "]";
               theRules.FtransOpis += " [" + faktur_rec.TransesWOtwins./*Single*/First(rtr => rtr.T_konto == ktoAndMtrGrp.konto)      .T_artiklName + "]";
            else
             //theRules.FtransOpis += " [" + faktur_rec.Transes       ./*Single*/First(rtr => rtr.T_recID == ktoAndMtrGrp.rtrFrsRecID).T_artiklName + "]";
               theRules.FtransOpis += " [" + faktur_rec.TransesWOtwins./*Single*/First(rtr => rtr.T_recID == ktoAndMtrGrp.rtrFrsRecID).T_artiklName + "]";
         }

         if(theRules.IsBLAGAJNA)
         {
            theRules.FtransOpis  = faktur_rec.TipBr + ":" + theRules.FtransOpis;
            // daklem, ako ima stavaka i na prvoj stavci ima zadani broj racuna (u JedMj) onda FtransTipBr = broj racuna, odervajs FtransTipBr = Tt i TtNum same uplatnice/isplatnice 
          //if(faktur_rec.Transes       .Count  .NotZero() && faktur_rec.Transes[0].T_jedMj.NotEmpty())
            if(faktur_rec.TransesWOtwins.Count().NotZero() && faktur_rec.Transes[0].T_jedMj.NotEmpty())
            {
             //theRules.FtransTipBr = faktur_rec.Transes[0]                                                                    .T_jedMj;
             //theRules.FtransTipBr = faktur_rec.Transes       ./*Single*/First(rtr => rtr.T_recID == ktoAndMtrGrp.rtrFrsRecID).T_jedMj;
               theRules.FtransTipBr = faktur_rec.TransesWOtwins./*Single*/First(rtr => rtr.T_recID == ktoAndMtrGrp.rtrFrsRecID).T_jedMj;
            }
         }

         theRules.FtransKonto   = ktoAndMtrGrp.konto  ;
         theRules.FtransMtrosCD = ktoAndMtrGrp.mtrosCD;
         theRules.FtransMtrosTK = theRules.FtransMtrosCD.IsZero() ? "" : ZXC.TheVvForm.TheVvUC.Get_Kupdob_FromVvUcSifrar(theRules.FtransMtrosCD).Ticker;
         theRules.SetDugAndPot(!theRules.TT_OtvaranjeDUG, ktoAndMtrGrp.kontoMoney);

         // Za npr Intrade: na '6600' stavku u TipBr podmetni TT_TtNum od primke a ne od UFA-e 
         string origTipBr = theRules.FtransTipBr;
         if(faktur_rec.TT == Faktur.TT_UFA && shouldGroupUFAbyPRD)
         {
            // 22.06.2012: 
            if(faktur_rec.V1_tt.NotEmpty() && faktur_rec.V1_ttNum.NotZero())
            {
               theRules.FtransTipBr = Faktur.Set_TT_And_TtNum(faktur_rec.V1_tt, faktur_rec.V1_ttNum);
            }
         }

         // 10.04.2017: PPR from GPRO modification: redak 3. i 4. ---> umjesto konta troska treba konto proizvodnje 
         if(IsPPRfromRNM(theRules))
         {
            if(IsSkladGotProizvoda(theRules, faktur_rec.SkladCD))
            {
               theRules.FtransKonto = ZXC.KSD.Dsc_otp_ktoObrade; // na konto proizvodnje '60001' 
            }
         }

         Send2Nalog(conn, ref line, theRules, /* false */ theRules.IsBLAGAJNA || theRules.IsINTERN_UI); // da dobije fakturRecID 

         theRules.FtransOpis  = origOpis ;
         theRules.FtransTipBr = origTipBr;

         #region PPR sa RNM additions

         if(IsPPRfromRNM(theRules))
         {
            // 02.03.2017: 2 dodatna reda za hendlanje usluge (npr radnih sasti na PPR-u)

            // 3. redak USL u dug plus 
            theRules.SetDugAndPot(true, faktur_rec.Transes.Sum(rtr => rtr.R_byTheAsEx_uslOnly_KC));

            if(IsSkladGotProizvoda(theRules, faktur_rec.SkladCD))
            {
               theRules.FtransKonto = ZXC.KSD.Dsc_otp_ktoObrade; // na konto proizvodnje '60001' 
            }

            Send2Nalog(conn, ref line, theRules, false);

            // 4. redak USL u dug minus 

            theRules.SetDugAndPot(true, -1 * faktur_rec.Transes.Sum(rtr => rtr.R_byTheAsEx_uslOnly_KC));

            theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_KtoRnmPprUSL;

            // !!! inace se pojavljuje, a ne smije, u IATP-u 
            theRules.FtransProjektCD = "";

            Send2Nalog(conn, ref line, theRules, false);
         }

         #endregion PPR sa RNM additions
      }
   }

   private static bool IsPPRfromRNM(Faktur2NalogRulesAndData theRules)
   {
      return ZXC.IsRNMnotRNP && theRules.FtransFakTT == Faktur.TT_PPR;
   }

   private static bool IsSkladGotProizvoda(Faktur2NalogRulesAndData theRules, string skladCD)
   {
      string skladKonto = Faktur2NalogRulesAndData.GetSkladKontoForSkladCD(skladCD);

      return (skladKonto == ZXC.KSD.Dsc_otp_ktoGotProiz);
   }

   private static void Send2Nalog_Kupdob         (XSqlConnection conn, ref ushort line, Faktur2NalogRulesAndData theRules, Faktur faktur_rec)
   {
      theRules.SetKontoAndMoney_Kupdob(faktur_rec); 
      
      Send2Nalog(conn, ref line, theRules, true);

      if(theRules.KtoShemaDsc.Dsc_IsCheckAvanses)
      {
         Check4UnlinkedAvanses(conn, theRules, faktur_rec);
      }
   }
   private static void Send2Nalog_Kupdob_IRM     (XSqlConnection conn, ref ushort line, Faktur2NalogRulesAndData theRules, Faktur faktur_rec, ZXC.VvUtilDataPackage[] nacPlacArray)
   {
    //25.10.2021.za firme koje knjize samo radi IOS-a
    //if(ZXC.KSD.Dsc_IsOnlyIOSknjizenje)
    //31.03.2022. razdvajamo IsOnlyIOSknjizenje i ForceIRMkaoIRA - ovo su konta kada se knjize IRM pojedinacno pa onda to i sa time ima veze - pitanej je sto kad ce netko htjeti jedno i drugo????
      if(ZXC.KSD.Dsc_ForceIRMkaoIRA || ZXC.KSD.Dsc_IsOnlyIOSknjizenje)// da li ovdje staviti ILI????
      {
         if(faktur_rec.IsNpCash)
         {
            faktur_rec.Konto      = ZXC.KSD.Dsc_IrmKupciCash; // knjizenje gortovine koja ne ide u IOS   
            theRules.FtransValuta = faktur_rec.DokDate;
         }
         else if(faktur_rec.NacPlac.ToUpper() == "VIRMAN")
         {
            faktur_rec.Konto = ZXC.KSD.Dsc_RKto_Kupca; // klasicno virmansko kao i IRA            
         }
         else
         {
            faktur_rec.Konto = ZXC.KSD.Dsc_Mir_Kupci; // za kartice ako se hoce izuzeti iz IOS-a ili dati drugaciji konto od klasicnog virmanskog
            theRules.FtransValuta = faktur_rec.DokDate;
         }

         theRules.FtransTipBr = faktur_rec.TT_And_TtNum;

         theRules.SetKontoAndMoney_Kupdob(faktur_rec);

         Send2Nalog(conn, ref line, theRules, true);

         return;
      }

      bool mirSUMisDaily = !theRules.KtoShemaDsc.Dsc_MirSumMonthly;

      ZXC.VvUtilDataPackage[] nacPlacArray_thisPeriod;

      if(mirSUMisDaily) nacPlacArray_thisPeriod = nacPlacArray.Where(nacPlac => nacPlac.TheStr1 == faktur_rec.SKL_DokDate_Full__AsString).ToArray();
      else              nacPlacArray_thisPeriod = nacPlacArray.Where(nacPlac => nacPlac.TheStr1 == faktur_rec.SKL_DokDate_Month_AsString).ToArray();

      foreach(ZXC.VvUtilDataPackage nacPlacPackage in nacPlacArray_thisPeriod)
      {
         if(nacPlacPackage.TheBool/*IsNpCash*/) theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_IrmKupciCash; 
         else                                   theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_Mir_Kupci;

         theRules.FtransDug = nacPlacPackage.TheDecimal;

         theRules.FtransKupdobCD = nacPlacPackage.TheUint;
         theRules.FtransKupdobTK = nacPlacPackage.TheStr3;

         // 27.01.2016: 
         theRules.FtransFakRecID = nacPlacPackage.TheUint2;

       //theRules.FtransMtrosCD = Kupdob.GetKupdobFromSkladCD(nacPlacPackage.TheStr5).KupdobCD;
       //theRules.FtransMtrosTK = Kupdob.GetKupdobFromSkladCD(nacPlacPackage.TheStr5).Ticker  ;

         theRules.FtransTipBr = nacPlacPackage.TheStr4;

         // 19.01.2016: 
         theRules.FtransValuta = nacPlacPackage.TheDate.IsEmpty() ? DateTime.MinValue : nacPlacPackage.TheDate; // nevirmanski IRM tu dode kao 01.01.1753 tj. DateTimePicker.MinimumDateTime 

         theRules.FtransOpis     = mirSUMisDaily ? nacPlacPackage.TheStr2 : nacPlacPackage.TheStr2 + " / " + nacPlacPackage.TheStr1 + " mjesec";

         Send2Nalog(conn, ref line, theRules, true);
      }
   }
   private static void Send2Nalog_Pdv            (XSqlConnection conn, ref ushort line, Faktur2NalogRulesAndData theRules, Faktur faktur_rec)
   {
      //25.10.2021.za firme koje knjize samo radi IOS-a
      if(ZXC.KSD.Dsc_IsOnlyIOSknjizenje) return;

      if(theRules.IsIRM) theRules.FtransOpis = "PDV IRM-ova";

      if(faktur_rec.PdvGEOkind != ZXC.PdvGEOkindEnum.EU) // faktur_rec.PdvGEOkind je HR, WORLD, BS, TP. (klasik, kako je i bilo) 
      {
         bool is_BS     = (faktur_rec.PdvGEOkind == ZXC.PdvGEOkindEnum.BS);
         bool is_TP     = (                                                  faktur_rec.PdvGEOkind == ZXC.PdvGEOkindEnum.TP);
         bool is_BSorTP = (faktur_rec.PdvGEOkind == ZXC.PdvGEOkindEnum.BS || faktur_rec.PdvGEOkind == ZXC.PdvGEOkindEnum.TP);

         theRules.SetKontoAndMoney_Pdv_25m  (faktur_rec, faktur_rec.S_ukPdv25m  , faktur_rec.PdvGEOkind, false, false); Send2Nalog(conn, ref line, theRules, false);
         if(is_BSorTP) 
         { theRules.SetKontoAndMoney_Pdv_25m(faktur_rec, faktur_rec.S_ukPdv25m  , faktur_rec.PdvGEOkind, false, true ); Send2Nalog(conn, ref line, theRules, false); }
                                                                                
         theRules.SetKontoAndMoney_Pdv_25n  (faktur_rec, faktur_rec.S_ukPdv25n  , faktur_rec.PdvGEOkind, false, false); Send2Nalog(conn, ref line, theRules, false);
         if(is_BSorTP)
         { theRules.SetKontoAndMoney_Pdv_25n(faktur_rec, faktur_rec.S_ukPdv25n  , faktur_rec.PdvGEOkind, false, true ); Send2Nalog(conn, ref line, theRules, false);}                                                                    
         
         theRules.SetKontoAndMoney_Pdv_10m  (faktur_rec, faktur_rec.S_ukPdv10m  , faktur_rec.PdvGEOkind, false, false); Send2Nalog(conn, ref line, theRules, false);
         if(is_BS)
         { theRules.SetKontoAndMoney_Pdv_10m(faktur_rec, faktur_rec.S_ukPdv10m  , faktur_rec.PdvGEOkind, false, true ); Send2Nalog(conn, ref line, theRules, false); }
         
         theRules.SetKontoAndMoney_Pdv_10n  (faktur_rec, faktur_rec.S_ukPdv10n  , faktur_rec.PdvGEOkind, false, false); Send2Nalog(conn, ref line, theRules, false);
         if(is_BS)                                                              
         { theRules.SetKontoAndMoney_Pdv_10n(faktur_rec, faktur_rec.S_ukPdv10n  , faktur_rec.PdvGEOkind, false, true ); Send2Nalog(conn, ref line, theRules, false);}
         
         theRules.SetKontoAndMoney_Pdv_05m  (faktur_rec, faktur_rec.S_ukPdv05m  , faktur_rec.PdvGEOkind, false, false); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_05n  (faktur_rec, faktur_rec.S_ukPdv05n  , faktur_rec.PdvGEOkind, false, false); Send2Nalog(conn, ref line, theRules, false);
      }
      else // ZXC.PdvGEOkindEnum.EU 
      {
         theRules.SetKontoAndMoney_Pdv_25m(faktur_rec, faktur_rec.S_ukPdvR25m_EU, faktur_rec.PdvGEOkind, false, false); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_25m(faktur_rec, faktur_rec.S_ukPdvR25m_EU, faktur_rec.PdvGEOkind, false, true ); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_25n(faktur_rec, faktur_rec.S_ukPdvR25n_EU, faktur_rec.PdvGEOkind, false, false); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_25n(faktur_rec, faktur_rec.S_ukPdvR25n_EU, faktur_rec.PdvGEOkind, false, true ); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_10m(faktur_rec, faktur_rec.S_ukPdvR10m_EU, faktur_rec.PdvGEOkind, false, false); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_10m(faktur_rec, faktur_rec.S_ukPdvR10m_EU, faktur_rec.PdvGEOkind, false, true ); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_10n(faktur_rec, faktur_rec.S_ukPdvR10n_EU, faktur_rec.PdvGEOkind, false, false); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_10n(faktur_rec, faktur_rec.S_ukPdvR10n_EU, faktur_rec.PdvGEOkind, false, true ); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_05m(faktur_rec, faktur_rec.S_ukPdvR05m_EU, faktur_rec.PdvGEOkind, false, false); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_05m(faktur_rec, faktur_rec.S_ukPdvR05m_EU, faktur_rec.PdvGEOkind, false, true ); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_05n(faktur_rec, faktur_rec.S_ukPdvR05n_EU, faktur_rec.PdvGEOkind, false, false); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_05n(faktur_rec, faktur_rec.S_ukPdvR05n_EU, faktur_rec.PdvGEOkind, false, true ); Send2Nalog(conn, ref line, theRules, false);

         theRules.SetKontoAndMoney_Pdv_25m(faktur_rec, faktur_rec.S_ukPdvU25m_EU, faktur_rec.PdvGEOkind, true, false); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_25m(faktur_rec, faktur_rec.S_ukPdvU25m_EU, faktur_rec.PdvGEOkind, true, true ); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_25n(faktur_rec, faktur_rec.S_ukPdvU25n_EU, faktur_rec.PdvGEOkind, true, false); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_25n(faktur_rec, faktur_rec.S_ukPdvU25n_EU, faktur_rec.PdvGEOkind, true, true ); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_10m(faktur_rec, faktur_rec.S_ukPdvU10m_EU, faktur_rec.PdvGEOkind, true, false); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_10m(faktur_rec, faktur_rec.S_ukPdvU10m_EU, faktur_rec.PdvGEOkind, true, true ); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_10n(faktur_rec, faktur_rec.S_ukPdvU10n_EU, faktur_rec.PdvGEOkind, true, false); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_10n(faktur_rec, faktur_rec.S_ukPdvU10n_EU, faktur_rec.PdvGEOkind, true, true ); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_05m(faktur_rec, faktur_rec.S_ukPdvU05m_EU, faktur_rec.PdvGEOkind, true, false); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_05m(faktur_rec, faktur_rec.S_ukPdvU05m_EU, faktur_rec.PdvGEOkind, true, true ); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_05n(faktur_rec, faktur_rec.S_ukPdvU05n_EU, faktur_rec.PdvGEOkind, true, false); Send2Nalog(conn, ref line, theRules, false);
         theRules.SetKontoAndMoney_Pdv_05n(faktur_rec, faktur_rec.S_ukPdvU05n_EU, faktur_rec.PdvGEOkind, true, true ); Send2Nalog(conn, ref line, theRules, false);
      }

      theRules.SetKontoAndMoney_Pdv_23m(faktur_rec                       ); Send2Nalog(conn, ref line, theRules, false);
      theRules.SetKontoAndMoney_Pdv_23n(faktur_rec                       ); Send2Nalog(conn, ref line, theRules, false);
      theRules.SetKontoAndMoney_Pdv_22m(faktur_rec                       ); Send2Nalog(conn, ref line, theRules, false);
      theRules.SetKontoAndMoney_Pdv_22n(faktur_rec                       ); Send2Nalog(conn, ref line, theRules, false);

      if(theRules.IsIRM) theRules.FtransOpis = "Porez na potrošnju IRM-ova";
      theRules.SetDugAndPot(false, faktur_rec.S_ukIznPNP); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_ukIznPNP; Send2Nalog(conn, ref line, theRules, false);
   }
   private static void Send2Nalog_BlagCurrDateSum_OLDorig(XSqlConnection conn, ref ushort line, Faktur2NalogRulesAndData theRules, List<Faktur> onCurrDateFakturlist)
   {
      // 27.03.2018. ovdje bi trebalo sumirati ulatinice i isplatnice koje su sa istog skladista
      //             i onda za svako skladiste posebno izraziti sumu dug i pot na konta koja su upisana u 
      //             lui listi skladista
      //             ILI ako nam je lakse da u theRules-e stavimo odabir blagajne koja se knjizi pa neka korisnik odluci

      decimal ukUPL = onCurrDateFakturlist.Sum(fak => fak.S_Blg_UPL);
      decimal ukISP = onCurrDateFakturlist.Sum(fak => fak.S_Blg_ISP);

      theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_Blg_Promet;
      theRules.FtransOpis  = "Promet Blagajne";

      theRules.FtransDug = ukUPL;
      theRules.FtransPot = ukISP;

      theRules.FtransKupdobCD = theRules.FtransMtrosCD = 0;
      theRules.FtransKupdobTK = theRules.FtransMtrosTK = theRules.FtransTipBr = "";

      Send2Nalog(conn, ref line, theRules, false);
   }
   private static void Send2Nalog_BlagCurrDateSum(XSqlConnection conn, ref ushort line, Faktur2NalogRulesAndData theRules, List<Faktur> onCurrDateFakturlist)
   {
      decimal ukUPL;
      decimal ukISP;

      string currBlgKonto;
      string currBlgName ;

      VvLookUpItem lui;

      List<string> skladCDlist = onCurrDateFakturlist.Select(fak => fak.SkladCD).Distinct().ToList();

      foreach(string skladCD in skladCDlist)
      {
         lui = ZXC.luiListaSkladista.GetLuiForThisCd(skladCD);

         if(lui == null)
         {
            currBlgKonto = theRules.KtoShemaDsc.Dsc_Blg_Promet;
            currBlgName  = ""                                 ;
         }
         else
         {
            currBlgKonto = lui.Number.ToStringVv_NoDecimalNoGroup();
            currBlgName  = lui.Name;
         }

         ukUPL = onCurrDateFakturlist.Where(fak => fak.SkladCD == skladCD).Sum(fak => fak.S_Blg_UPL);
         ukISP = onCurrDateFakturlist.Where(fak => fak.SkladCD == skladCD).Sum(fak => fak.S_Blg_ISP);

       //theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_Blg_Promet;
         theRules.FtransKonto = currBlgKonto;
         theRules.FtransOpis = "Promet Blagajne " + currBlgName;

         theRules.FtransDug = ukUPL;
         theRules.FtransPot = ukISP;

         theRules.FtransKupdobCD = theRules.FtransMtrosCD = 0;
         theRules.FtransKupdobTK = theRules.FtransMtrosTK = theRules.FtransTipBr = "";

         Send2Nalog(conn, ref line, theRules, false);
      }
   }
   private static void Send2Nalog_Sklad          (XSqlConnection conn, ref ushort line, Faktur2NalogRulesAndData theRules, Faktur faktur_rec)
   {
      theRules.SetKontoAndMoney_Skl4IntUI(faktur_rec); 

      Send2Nalog(conn, ref line, theRules, true);
   }
   private static void Send2Nalog_Realizac       (XSqlConnection conn, ref ushort line, Faktur2NalogRulesAndData theRules, Faktur faktur_rec)
   {
      //25.10.2021.za firme koje knjize samo radi IOS-a
      if(ZXC.KSD.Dsc_IsOnlyIOSknjizenje) return;

      // 14.03.2014. 
      //if(theRules.KtoShemaDsc.Dsc_HocuRealizaciju == false) return;
      if(theRules.IsIZD == false &&  theRules.KtoShemaDsc.Dsc_HocuRealizaciju == false) return; // znaci da je ira koja nece realizaciju i IZD koji hoce iako nije oznacena

      // 16.09.2016: po osnovi Dsc_Kto_Skladiste dodatno specificirati overrajdanje pravila Dsc_HocuRealizaciju                                     
      // Dsc_Kto_Skladiste smo nekada koristili za nest' drugo                                                                                      
      // tako da neka sklad idu a neka ne idu u knjizenje Send2Nalog_Realizac (Metaflex OvoOno)                                                     
      bool isKontoNemojUrealizaciju = false; // isKontoNemojUrealizaciju = theRules.FtransKonto.StartsWith(theRules.KtoShemaDsc.Dsc_Kto_Skladiste); 

      // ===================== 4. redak 'Razduzenja Skladista' ============================= 
      // Konto 66xyz: POTRAZUJE (ali za IRU, za URU je DUG) - Nabavna vrijednost prodane robe IRA_NV      
      theRules.FtransOpis = "Razduženje skladišta";

      if(theRules.IsIraRealizOnly)
      {
         theRules.NalogNapomena = "KOREKCIJA razduženja skladišta i nabVr prodane robe po ProsNabCij";
         theRules.FtransTipBr   = "";
      }

      // 14.02.2017: 
    //theRules.SetDugAndPot(false,                                                     faktur_rec.R_Ira_NV);
      theRules.SetDugAndPot(false, theRules.IsIraRealizOnly ?  faktur_rec.Ira_ROB_NV : faktur_rec.R_Ira_NV);

      theRules.FtransKonto = /*theRules*/Faktur2NalogRulesAndData.GetSkladKontoForSkladCD(faktur_rec.SkladCD);

      // NE! prethodno popunjeni Dsc_Kto_Skladiste ce ovako izazvati kaos! Do dalnjega cemo fiksno 'nemoj 630...' a kao TODO iz rulsa  
    //isKontoNemojUrealizaciju = theRules.FtransKonto.StartsWith(theRules.KtoShemaDsc.Dsc_Kto_Skladiste);
      // 17.11.2016. vracamo natrag da ide u realizaciju nakon sto smo na PIP stavili plansku/skladisnu cijenu
    //isKontoNemojUrealizaciju = theRules.FtransKonto.StartsWith("630");
      // 24.11.2016. ne znamo sto ce biti sa PIP ciejnom pa vracamo na staro

      // 14.02.2017: ma knjizi, brate, uvijek! 
    //isKontoNemojUrealizaciju = theRules.FtransKonto.StartsWith("630");
      isKontoNemojUrealizaciju = false                                 ;

      if(isKontoNemojUrealizaciju == false) Send2Nalog(conn, ref line, theRules, false);
      else                                  return; // !!! dakle, nemoj niti 5. redak 


      // ===================== 4. redak 'Razduzenja Skladista' ============================= 

      // ===================== 5. redak 'Nabavna vrijednost prodane robe' ============================= 
      // Konto 7000: DUGUJE - Nabavna vrijednost prodane robe IRA_NV 
      theRules.FtransOpis = theRules.IsIZD ? "Nabavna vrijednost izdane robe" : "Nabavna vrijednost prodane robe";
      // 14.02.2017: 
    //theRules.SetDugAndPot(true,                                                    faktur_rec.R_Ira_NV);
      theRules.SetDugAndPot(true, theRules.IsIraRealizOnly ? faktur_rec.Ira_ROB_NV : faktur_rec.R_Ira_NV);

      theRules.FtransKonto = 
         theRules.IsIZD ? theRules.KtoShemaDsc.Dsc_KtoIZD : 
            theRules.KtoShemaDsc.Dsc_OcuPrihLikeSklad ? theRules.GetRealizacKontoForSkladCD(faktur_rec.SkladCD, theRules.KtoShemaDsc.Dsc_Kto_Realizacija) : 
               theRules.KtoShemaDsc.Dsc_Kto_Realizacija;

      Send2Nalog(conn, ref line, theRules, false);

      // ===================== 5. redak 'Nabavna vrijednost prodane robe' ============================= 
   }
   private static void Send2Nalog_RealizcMal     (XSqlConnection conn, ref ushort line, Faktur2NalogRulesAndData theRules, Faktur faktur_rec)
   {
      //25.10.2021.za firme koje knjize samo radi IOS-a
      if(ZXC.KSD.Dsc_IsOnlyIOSknjizenje) return;

      theRules.FtransOpis = "Razduženje skladišta";
      theRules.SetDugAndPot(false, faktur_rec.R_ukMSK); theRules.FtransKonto = /*theRules*/Faktur2NalogRulesAndData.GetSkladKontoForSkladCD(faktur_rec.SkladCD); 
      Send2Nalog(conn, ref line, theRules, false);

      theRules.FtransOpis = "Knjigovod. vrijednost izdane robe";
      theRules.SetDugAndPot(true, faktur_rec.R_ukMSK); theRules.FtransKonto = theRules.KtoShemaDsc.Dsc_KtoIZM; 
      Send2Nalog(conn, ref line, theRules, false);
   }

   private static void Send2Nalog                (XSqlConnection conn, ref ushort line, Faktur2NalogRulesAndData theRules, bool isKupdobLine)
   {
      if(isKupdobLine == false && (theRules.FtransDug + theRules.FtransPot).IsZero()) return; // za kupdobLiniju da doda iako je nula, da ne ponavlja... 

      // 13.03.2014. neprofitni ne zele mjesta troska na razlicitima od 3i4 i kupdobe, ttBr i valutu na 3i4

      bool mtrosIsUnWanted   = ( ZXC.CURR_prjkt_rec.IsNeprofit                           &&  theRules.FtransKonto.StartsWith("3") == false && theRules.FtransKonto.StartsWith("4") == false);
    //bool pozicijaIsUnWanted= ( ZXC.CURR_prjkt_rec.IsNeprofit                           &&  theRules.FtransKonto.StartsWith("3") == false && theRules.FtransKonto.StartsWith("4") == false);
      bool pozicijaIsUnWanted= ((ZXC.CURR_prjkt_rec.IsNeprofit                        ||
                                 ZXC.CURR_prjkt_rec.PlanKind == ZXC.PlanKindEnum.PlnBy_FOND) &&  theRules.FtransKonto.StartsWith("3") == false && theRules.FtransKonto.StartsWith("4") == false);
      bool fondIsUnWanted    = ( ZXC.CURR_prjkt_rec.PlanKind == ZXC.PlanKindEnum.PlnBy_FOND  &&  theRules.FtransKonto.StartsWith("3") == false && theRules.FtransKonto.StartsWith("4") == false);
      bool kupdobIsUnWanted  = ( ZXC.CURR_prjkt_rec.IsNeprofit                           && (theRules.FtransKonto.StartsWith("3") == true  || theRules.FtransKonto.StartsWith("4") == true ));
      bool tipBrValutaUnWant = ( ZXC.CURR_prjkt_rec.IsNeprofit                           && (theRules.FtransKonto.StartsWith("3") == true  || theRules.FtransKonto.StartsWith("4") == true ));

      uint   mtros_cd  = mtrosIsUnWanted   ?  0                : theRules.FtransMtrosCD ;
      string mtros_tk  = mtrosIsUnWanted   ? ""                : theRules.FtransMtrosTK ;
      uint   kupdob_cd = kupdobIsUnWanted  ?  0                : theRules.FtransKupdobCD;
      string kupdob_tk = kupdobIsUnWanted  ? ""                : theRules.FtransKupdobTK;
      string tipBr     = tipBrValutaUnWant ? ""                : theRules.FtransTipBr   ;
      string pozicija  = pozicijaIsUnWanted? ""                : theRules.FtransPozicija;
      string fond      = fondIsUnWanted    ? ""                : theRules.FtransFond    ;
      DateTime valuta  = tipBrValutaUnWant ? DateTime.MinValue : theRules.FtransValuta  ;
      
      // 01.04.2014:
      Kplan theKplan = VvUserControl.KplanSifrar.SingleOrDefault(kpl => kpl.Konto == theRules.FtransKonto);
      if(theKplan == null)
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Konto {0}\n\nne postoji!", theRules.FtransKonto);
      }

      theRules.NalogNapomena = theRules.LimitedStrNalog (theRules.NalogNapomena, ZXC.NalCI.napomena);
      theRules.FtransOpis    = theRules.LimitedStrFtrans(theRules.FtransOpis, ZXC.FtrCI.t_opis);
      //------------------------------------------------------------------------- 
      /* */
      /* */  NalogDao.AutoSetNalog(conn, ref line,
      /* */
      /* */ /*DateTime n_dokDate    */ theRules.NalogDokDate,
      /* */ /*string   n_tt         */ theRules.NalogTT,
      /* */ /*string   n_napomena   */ theRules.NalogNapomena,
      /* */
      /* */ /*string   t_konto      */ theRules.FtransKonto,
      /* */ /*uint     t_kupdob_cd  */ kupdob_cd /*theRules.FtransKupdobCD */,   
      /* */ /*string   t_ticker     */ kupdob_tk /*theRules.FtransKupdobTK */,
      /* */ /*uint     t_mtros_cd   */ mtros_cd  /*theRules.FtransMtrosCD  */,
      /* */ /*uint     t_mtros_tk   */ mtros_tk  /*theRules.FtransMtrosTK  */,
      /* */ /*string   t_tipBr      */ tipBr     /*theRules.FtransTipBr    */,
      /* */ /*string   t_opis       */ theRules.FtransOpis,
      /* */ /*DateTime t_valuta     */ valuta    /*theRules.FtransValuta   */,
      /* */ /*string   t_pdv        */ "",
      /* */ /*string   t_037        */ "",
      /* */ /*string   t_projektCD  */ theRules.FtransProjektCD,
      /* */ /*ushort   t_pdvKnjiga  */ theRules.FtransPdvKnjiga,
      /* */ /*uint     t_fakRecID   */ isKupdobLine ? theRules.FtransFakRecID   :                       0, // ! 
      /* */ /*OtsKindEnum t_otsKind */ isKupdobLine ? ZXC.OtsKindEnum.OTVARANJE : ZXC.OtsKindEnum.NIJEDNO, // ! 
      /* */ /* string  t_fond       */ fond    ,
      /* */ /* string  t_pozicija   */ pozicija,
      /* */ /*decimal  t_dug        */ theRules.FtransDug,
      /* */ /*decimal  t_pot        */ theRules.FtransPot);
      /* */
      //------------------------------------------------------------------------- 
   }

   public  static void Fak2Nal_CheckAndPrebaciIfNeeded(XSqlConnection conn)
   {
      if(ZXC.Fak2NalAutomationChecked == true) return; // Dakle, samo jednom u toku program sessije se ovo odvija, ako oce ponovo mora izaci pa uci u program 

      // Here we go! ----------------------------------   

      ZXC.Fak2NalAutomationChecked = true;
      
    //KtoShemaDsc              KSD = new KtoShemaDsc(ZXC.dscLuiLst_KtoShema);
      KtoShemaDsc              KSD = ZXC.KSD;
      Faktur2NalogRulesAndData theRules    = new Faktur2NalogRulesAndData();

      theRules.KtoShemaDsc              = KSD;
      theRules.IsAutomatic              = true;
      theRules.PeriodDefinedVia_DokDate = false;

      Cursor.Current = Cursors.WaitCursor;

      if(KSD.Fak2NalTime_Ulaz != ZXC.Faktur2NalogTimeRuleEnum.DoIt_NEVER) 
         LoadFaktur2Nalog_AUTOMATIC(conn, theRules, ZXC.Faktur2NalogSetEnum.ULAZNI_VP, KSD.Fak2NalTime_Ulaz, "ULAZNIH");

      if(KSD.Fak2NalTime_Ulaz != ZXC.Faktur2NalogTimeRuleEnum.DoIt_NEVER)
         LoadFaktur2Nalog_AUTOMATIC(conn, theRules, ZXC.Faktur2NalogSetEnum.ULAZNI_MP, KSD.Fak2NalTime_Ulaz, "MALOPRODAJNIH ULAZNIH");

      if(KSD.Fak2NalTime_IzlazVP != ZXC.Faktur2NalogTimeRuleEnum.DoIt_NEVER)
         LoadFaktur2Nalog_AUTOMATIC(conn, theRules, ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP, KSD.Fak2NalTime_IzlazVP, "IZLAZNIH");

      if(KSD.Fak2NalTime_IzlazMP != ZXC.Faktur2NalogTimeRuleEnum.DoIt_NEVER)
         LoadFaktur2Nalog_AUTOMATIC(conn, theRules, ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP, KSD.Fak2NalTime_IzlazMP, "MALOPRODAJNIH IZLAZNIH");

      if(KSD.Fak2NalTime_Blagajna != ZXC.Faktur2NalogTimeRuleEnum.DoIt_NEVER)
         LoadFaktur2Nalog_AUTOMATIC(conn, theRules, ZXC.Faktur2NalogSetEnum.BLAGAJNA, KSD.Fak2NalTime_Blagajna, "BLAGAJNIČKIH");

      Cursor.Current = Cursors.Default;

   }

   private static void LoadFaktur2Nalog_AUTOMATIC(XSqlConnection conn, Faktur2NalogRulesAndData theRules, ZXC.Faktur2NalogSetEnum faktur2NalogSetEnum, ZXC.Faktur2NalogTimeRuleEnum faktur2NalogTimeRuleEnum, string kakvihDokumenata)
   {
      theRules.Fak2nalSet      = faktur2NalogSetEnum;
      theRules.Fak2nalTimeRule = faktur2NalogTimeRuleEnum;

      ZXC.VvUtilDataPackage[] nacPlacArray;

      List<Faktur> fakturList = FakturDao.GetNeprebaceniFakturAndRtrans2NalogLists(conn, theRules, true, out nacPlacArray);

      FakturDao.ExecuteFaktur2Nalog(conn, fakturList, theRules, nacPlacArray);

      if(fakturList.Count.NotZero() || theRules.NalogCount.NotZero())
      {
         string massaze = String.Format("Gotovo.\n\nBroj obrađenih {2} RISK dokumenata/grupa: [{0}]\n\nBroj novih naloga: [{1}].\n\n{3}",
            fakturList.Count, theRules.NalogCount, kakvihDokumenata, GetRasterID(fakturList));

         MessageBox.Show(massaze, "AUTOMATSKI prijenos " + kakvihDokumenata + " RISK dokumenata na NZK", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }

      theRules.NalogCount = 0;
   }

   private static string GetRasterID(List<Faktur> fakturList)
   {
      return "od [" + fakturList[0].TT_And_TtNum + " " + fakturList[0].DDMM + "] do [" + fakturList.Last().TT_And_TtNum + " " + fakturList.Last().DDMM + "] (" + fakturList.Count + " dok.) ";
   }

   private static void Check4UnlinkedAvanses(XSqlConnection conn, Faktur2NalogRulesAndData theRules, Faktur faktur_rec)
   {
      System.Collections.Generic.List<Ftrans> unlinkedAvansesFtransList = new List<Ftrans>();

      DataRow drSchema;
      System.Collections.Generic.List<VvSqlFilterMember> filterMembers = new System.Collections.Generic.List<VvSqlFilterMember>(4);

      // filter for wantedTT                                                                                                                                             
      drSchema = ZXC.FtransSchemaRows[ZXC.FtrCI.t_tt];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elWantedTT", Nalog.IZ_TT, " = "));
      // filter from dateDO                                                                                                                                             
      drSchema = ZXC.FtransSchemaRows[ZXC.FtrCI.t_dokDate];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elDateDO", theRules.NalogDokDate, " <= "));
      // filter kupdobCD                                                                                                                                             
      drSchema = ZXC.FtransSchemaRows[ZXC.FtrCI.t_kupdob_cd];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elKupdob", theRules.FtransKupdobCD, " = "));
      // filter konto                                                                                                                                             
      drSchema = ZXC.FtransSchemaRows[ZXC.FtrCI.t_konto];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elKonto", theRules.FtransKonto, " = "));
      // filter tipbr                                                                                                                                             
      drSchema = ZXC.FtransSchemaRows[ZXC.FtrCI.t_tipBr];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elTipbr", "", " = "));

      LoadGenericVvDataRecordList(conn, unlinkedAvansesFtransList, filterMembers, " t_dokDate, t_dokNum, t_serial");

      int count = 0, maxCount = 10;
      if(unlinkedAvansesFtransList.Count.NotZero())
      {
         string errMessage = "Za partnera po računu iznosa " + faktur_rec.S_ukKCRP.ToStringVv() + "\n[" + faktur_rec + "]\n\npostoje NEVEZANE AVANSNE UPLATE:\n\n";

         foreach(Ftrans ftrans_rec in unlinkedAvansesFtransList)
         {
            if(++count <= maxCount)
            {
               errMessage += "stavka" + ftrans_rec + "---------------------------------------\n";
            }
            else
            {
               errMessage += "\n... i još [" + (unlinkedAvansesFtransList.Count - count + 1).ToString() + "] stavaka ...";
               break;
            }
         }

         //ZXC.aim_emsg(MessageBoxIcon.Error, errMessage);
         MessageBox.Show(errMessage, "OTKRIVENE SU NEVEZANE AVANSNE UPLATE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }

   }

   #endregion Faktur2Nalog

   #region Discover_LastUsedProjektTT
   
   internal static string Discover_LastUsedProjektTT_OldORIG(XSqlConnection dbConnection)
   {
      List<VvSqlFilterMember> filterMembers4_BOR = new List<VvSqlFilterMember>(1);
      filterMembers4_BOR.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt], false, "", Faktur.TT_BOR, "", "", " = ", ""));
      if(CountRecords(dbConnection, filterMembers4_BOR) > 0) return Faktur.TT_BOR;

      List<VvSqlFilterMember> filterMembers4_RNP = new List<VvSqlFilterMember>(1); 
      filterMembers4_RNP.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt], false, "", Faktur.TT_RNP, "", "", " = ", ""));
      if(CountRecords(dbConnection, filterMembers4_RNP) > 0) return Faktur.TT_RNP;

      List<VvSqlFilterMember> filterMembers4_RNM = new List<VvSqlFilterMember>(1);
      filterMembers4_RNM.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt], false, "", Faktur.TT_RNM, "", "", " = ", ""));
      if(CountRecords(dbConnection, filterMembers4_RNM) > 0) return Faktur.TT_RNM;

      List<VvSqlFilterMember> filterMembers4_RNS = new List<VvSqlFilterMember>(1);
      filterMembers4_RNS.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt], false, "", Faktur.TT_RNS, "", "", " = ", ""));
      if(CountRecords(dbConnection, filterMembers4_RNS) > 0) return Faktur.TT_RNS;

      List<VvSqlFilterMember> filterMembers4_RNZ = new List<VvSqlFilterMember>(1);
      filterMembers4_RNZ.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt], false, "", Faktur.TT_RNZ, "", "", " = ", ""));
      if(CountRecords(dbConnection, filterMembers4_RNZ) > 0) return Faktur.TT_RNZ;

      List<VvSqlFilterMember> filterMembers4_UGO = new List<VvSqlFilterMember>(1);
      filterMembers4_UGO.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt], false, "", Faktur.TT_UGO, "", "", " = ", ""));
      if(CountRecords(dbConnection, filterMembers4_RNZ) > 0) return Faktur.TT_RNZ;

      //List<VvSqlFilterMember> filterMembers4_PRJ = new List<VvSqlFilterMember>(1);
      //filterMembers4_PRJ.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt], false, "", Faktur.TT_PRJ, "", "", " = ", ""));
      //if(CountRecords(dbConnection, filterMembers4_PRJ) > 0) return Faktur.TT_PRJ;

      // default: 
      return Faktur.TT_PRJ;
   }

   internal static string Discover_LastUsedProjektTT(XSqlConnection dbConnection)
   {
      List<VvSqlFilterMember> filterMembers4_prjTT = new List<VvSqlFilterMember>(1);

      foreach(string projektTT in TtInfo.arrayProjektTT)
      {
         filterMembers4_prjTT = new List<VvSqlFilterMember>(1);
         filterMembers4_prjTT.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt], false, "", projektTT, "", "", " = ", ""));
         if(CountRecords(dbConnection, filterMembers4_prjTT) > 0) return projektTT;
      }

      // default: 
      return Faktur.TT_PRJ;
   }

   #endregion Discover_LastUsedProjektTT

   #region GetNarudzbaKolForArtikl

   internal static decimal GetNarudzbaKolForArtikl(XSqlConnection dbConnection, string artiklCD, string narTt, uint narTtNum)
   {
      List<Rtrans> rtransList = new List<Rtrans>();

      VvDaoBase.LoadGenericVvDataRecordList<Rtrans>(dbConnection, rtransList, GetFilterMembers_NarudzbaKolForArtikl(artiklCD, narTt, narTtNum), "t_serial ");

      return rtransList.Sum(rtr => rtr.T_kol);
   }

   private static List<VvSqlFilterMember> GetFilterMembers_NarudzbaKolForArtikl(string artiklCD, string narTt, uint narTtNum)
   {
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD], "elArtiklCD", artiklCD, " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt      ], "elTT"      , narTt   , " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_ttNum   ], "elTtNum"   , narTtNum, " = "));

      return filterMembers;
   }

   #endregion GetNarudzbaKolForArtikl

   #region PS_RISK

   internal static bool PS_RISK(XSqlConnection thisYconn, string pgDBname, string pgYear)
   {
      using(XSqlConnection prevYconn = VvSQL.CREATE_TEMP_XSqlConnection(pgDBname))
      {
         #region Init Stuff & GetArtiklWithArtstatList()

         List<Artikl> theArtiklWithArtstatList = new List<Artikl>();

         List<string> skladList = ArtiklDao.GetDistinctSkladCdListForArtikl(prevYconn, /*_artiklCD*/ "");

         var skladListW_sklNum = skladList.Join(ZXC.luiListaSkladista, sklList => sklList, lui => lui.Cd, (sklad, lui) => lui).OrderBy(lui => lui.Integer);

         VvRpt_RiSk_Filter rptFilter = new VvRpt_RiSk_Filter();

         DateTime pgYearFirstDay = new DateTime(int.Parse(pgYear), 1,   1            );
         DateTime pgYearLastDay  = new DateTime(int.Parse(pgYear), 12, 31, 23, 59, 59);

         rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.t_skladDate], true, "DateOD", pgYearFirstDay, pgYearFirstDay.ToString("dd.MM.yyyy."), "Od datuma:", "", ""));
         rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.t_skladDate], true, "DateDO", pgYearLastDay , pgYearLastDay .ToString("dd.MM.yyyy."), "Do datuma:", "", ""));

         #endregion Init Stuff

         foreach(VvLookUpItem lui in skladListW_sklNum)
         {
            ArtiklDao.GetArtiklWithArtstatList(prevYconn, theArtiklWithArtstatList, lui.Cd, rptFilter.DatumDo, rptFilter, "", "artiklName ");

            if(theArtiklWithArtstatList.Count.NotZero())
            {
               CreatePsFaktur(thisYconn /*!*/, theArtiklWithArtstatList, lui.Cd, lui.Flag, ZXC.projectYearFirstDay);

               theArtiklWithArtstatList.Clear();

               ZXC.SetStatusText(string.Format("Done PS for [{0}] [{1}]", lui.Cd, lui.Name));
            }
         }

      } // using(XSqlConnection pgConn = VvSQL.CREATE_TEMP_XSqlConnection(pgDBname))

      ZXC.ClearStatusText();

      return true;
   }

   private static void CreatePsFaktur(XSqlConnection conn, List<Artikl> theArtiklWithArtstatList, string _skladCD, bool _isMalopSkl, DateTime _dokDate)
   {
      #region Set RtransList

      Rtrans rtrans_rec;
      List<Rtrans> rtransList = new List<Rtrans>(theArtiklWithArtstatList.Count);
      VvLookUpItem pdvLui;
      decimal pdvSt;
      decimal klippingSum = 0.00M, finStSum = 0.00M;
      bool isVelepSkl = !_isMalopSkl;

      bool thisArtikl_hasNoKolHasCijenaOnly_ButNoPrometEither;

      // some check: 
      if(ZXC.IsTEXTHOany && ZXC.projectYearAsInt == 2023)
      {
         // provjeri ima li ijedan da je 
         // 1. artikl_rec.MadeIn == VvForm.artMadeIn_Kuna
         // 2. artikl_rec.ArtiklCD2.IsEmpty()
         // pa ako ima ... VAN! 
      }

      foreach(Artikl artikl_rec in theArtiklWithArtstatList)
      {
         // 02.01.2012:
         if(artikl_rec.TheAsEx.IsInKlipping)
         {
            klippingSum += artikl_rec.TheAsEx.StanjeFinKNJ;
            ZXC.aim_emsg(MessageBoxIcon.Warning, "Artikl [{0}] kolSt 0 a FinSt {1}", artikl_rec.ArtiklCD, artikl_rec.TheAsEx.StanjeFinKNJ.ToStringVv());
         }

         rtrans_rec = new Rtrans();

         // 14.12.2015: prilagodba za TH logiku (oni NE trebaju malopArtikl samo zbog cijena jer imaju nulte ZPC-ove)
       //if(artikl_rec.TheAsEx.IsWorthForPS)
         if(artikl_rec.TheAsEx.IsWorthForPS && ((!ZXC.IsTEXTHOany && ZXC.CURR_prjkt_rec.Ticker != "QQTEXT") || isVelepSkl || artikl_rec.TheAsEx.StanjeKol.NotZero()))
         {
            // 07.09.2022: BIG NEWS!                                                                                                        
            // NEMOJ više prebacivati bazuvjetno one koji NEMAJU StanjeKol a imaju 'cijenu' ... a za potrebe povrata kao prvog prometa u NG 
            // nego povjeri prija da li imaju ikakav promet u PG                                                                            
            // 20.12.2022: dodatno na ovo gasimo IsSvDUH uvjet te preskacemo PS takvih i za sve ostale, a ne samo SvDUH

            thisArtikl_hasNoKolHasCijenaOnly_ButNoPrometEither = 
             //ZXC.IsSvDUH &&
               artikl_rec.AS_StanjeKol .IsZero() &&
               artikl_rec.AS_UkPstKol  .IsZero() &&
               artikl_rec.AS_UkUlazKol .IsZero() && 
               artikl_rec.AS_UkIzlazKol.IsZero();

            if(thisArtikl_hasNoKolHasCijenaOnly_ButNoPrometEither) continue;

            finStSum += artikl_rec.TheAsEx.StanjeFinKNJ;

            rtrans_rec.T_artiklCD   = artikl_rec.ArtiklCD;
            rtrans_rec.T_artiklName = artikl_rec.ArtiklName;
            rtrans_rec.T_jedMj      = artikl_rec.JedMj;

            #region TH only HRD 2022 ---> 2023 

            if(ZXC.IsTEXTHOany && ZXC.projectYearAsInt == 2023 && artikl_rec.MadeIn == VvForm.artMadeIn_Kuna) // artikl_rec je iz prevYearConn! 
            {
               rtrans_rec.T_artiklCD   = artikl_rec.ArtiklCD2  ;
               rtrans_rec.T_artiklName = artikl_rec.ArtiklName2;
            }

            #endregion TH only HRD 2022 ---> 2023 

            rtrans_rec.T_kol  = artikl_rec.AS_StanjeKol ;
            rtrans_rec.T_kol2 = artikl_rec.AS_StanjeKol2;
            rtrans_rec.T_cij  = artikl_rec.AS_PrNabCij  ;

            // 14.12.2015: da bi CalcTransResults znao tko sam...
            rtrans_rec.T_TT = _isMalopSkl ? Faktur.TT_PSM : Faktur.TT_PST;

            #region Malop Additions

            if(_isMalopSkl)
            {
               if(artikl_rec.PdvKat.NotEmpty())
               {
                  pdvLui = ZXC.luiListaPdvKat.GetLuiForThisCd(artikl_rec.PdvKat);
                  pdvSt = pdvLui.Number;
               }
               else
               {
                  pdvSt = Faktur.CommonPdvStForThisDate(_dokDate);
               }

               rtrans_rec.T_pdvSt = pdvSt;

               rtrans_rec.T_wanted    = artikl_rec.AS_KnjigCij;
               rtrans_rec.T_mCalcKind = ZXC.MalopCalcKind.By_MPC; // !!! 
            }

            #endregion Malop Additions

            rtrans_rec.CalcTransResults(null);

            #region HRD 2022 ---> 2023

            if(ZXC.projectYearAsInt == 2023)
            {
             //decimal euroR_KC  = ZXC.EURiIzKuna_HRD_(rtrans_rec.R_KC)       ; // this IS     Ron2() 
               decimal euroR_KC  = ZXC.DivSafe(rtrans_rec.R_KC, ZXC.HRD_tecaj); // this IS NOT Ron2() 

               decimal euroT_cij = ZXC.DivSafe(euroR_KC, rtrans_rec.T_kol)/*.Ron2()*/;

               rtrans_rec.T_cij = euroT_cij/*.Ron2()*/;

               if(_isMalopSkl)
               {
                  decimal euroMP_cij = ZXC.DivSafe(rtrans_rec.R_CIJ_MSK, ZXC.HRD_tecaj); // this IS NOT Ron2() 

                //rtrans_rec.T_wanted = euroMP_cij/*.Ron2()*/;
                  rtrans_rec.T_wanted = euroMP_cij.Ron(4);
               }

               rtrans_rec.CalcTransResults(null);
            }

            #endregion HRD 2022 ---> 2023

            rtransList.Add(rtrans_rec);

         } // if(artikl_rec.TheAsEx.IsWorthForPS)
      }

      #endregion Set RtransList

      #region Set faktur_rec

      Faktur faktur_rec = new Faktur();
      ushort line = 0;

      faktur_rec.TT        = _isMalopSkl ? Faktur.TT_PSM : Faktur.TT_PST;

      faktur_rec.SkladDate = // !!! 
      faktur_rec.DokDate   = _dokDate;
      faktur_rec.SkladCD   = _skladCD;

      faktur_rec.S_ukK        =       rtransList.Sum(rtr => rtr.T_kol   );
      faktur_rec.S_ukK2       =       rtransList.Sum(rtr => rtr.T_kol2  );
      faktur_rec.S_ukKC       =       rtransList.Sum(rtr => rtr.R_KC    );
      faktur_rec.S_ukRbt1     =       rtransList.Sum(rtr => rtr.R_rbt1  );
      faktur_rec.S_ukKCR      =       rtransList.Sum(rtr => rtr.R_KCR   );
      faktur_rec.S_ukKCRM     =       rtransList.Sum(rtr => rtr.R_KCRM  );
      faktur_rec.S_ukKCRP     =       rtransList.Sum(rtr => rtr.R_KCRP  );
    //faktur_rec.S_ukMskPdv   =       rtransList.Sum(rtr => rtr.R_mskPdv);
    //faktur_rec.S_ukMSK      =       rtransList.Sum(rtr => rtr.R_MSK   );
      faktur_rec.S_ukTrnCount = (uint)rtransList.Count();

      // 14.12.2015: malop sume na drugi nacin 
      if(_isMalopSkl)
      {
         faktur_rec.Transes = rtransList;
         faktur_rec.TakeTransesSumToDokumentSum(true);
         faktur_rec.Transes = null;
      }

      #endregion Set faktur_rec

      #region foreach Rtrans AutoSetFaktur

      foreach(Rtrans rtrans in rtransList)
      {
         FakturDao.AutoSetFaktur(conn, ref line, faktur_rec, rtrans);
      }

      #endregion foreach Rtrans AutoSetFaktur

      if(klippingSum.NotZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nepreneseni financijski saldo {0}", klippingSum.ToStringVv());
      }

   }

   #endregion PS_RISK

   #region AutoAddInventuraDiff_RISK

   internal static bool AutoAddInventuraDiff_RISK(XSqlConnection conn, DateTime _dateInv, out uint PRIcount, out uint IZDcount, out uint KLKcount, out uint IZMcount)
   {
      #region Init Stuff 

      List<Artikl> theArtiklWithArtstatList           = new List<Artikl>();
      List<Artikl> theArtiklWithArtstatList_InvVISAK  = new List<Artikl>();
      List<Artikl> theArtiklWithArtstatList_InvMANJAK = new List<Artikl>();

      List<string> skladList = ArtiklDao.GetDistinctSkladCdListForArtikl(conn, /*_artiklCD*/ "");

      var skladListW_sklNum = skladList.Join(ZXC.luiListaSkladista, sklList => sklList, lui => lui.Cd, (sklad, lui) => lui).OrderBy(lui => lui.Integer);

      VvRpt_RiSk_Filter rptFilter = new VvRpt_RiSk_Filter();

      rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.t_skladDate], true, "DateOD", ZXC.projectYearFirstDay, "", "", "", ""));
    //rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.t_skladDate], true, "DateDO", ZXC.projectYearLastDay , "", "", "", ""));
      rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.t_skladDate], true, "DateDO", _dateInv               , "", "", "", ""));

      bool isMalop;
      uint THskladCDroot;

      PRIcount = IZDcount = KLKcount = IZMcount = 0;

      #endregion Init Stuff

      foreach(VvLookUpItem lui in skladListW_sklNum)
      {
         if(ZXC.IsTEXTHOany)
         {
            THskladCDroot = ZXC.ValOrZero_UInt(lui.Cd.SubstringSafe(0, 2));

            if(THskladCDroot != ZXC.vvDB_ServerID) continue; // Kruge rade 12BGS, 12BPS, 12BG2, 12BP2 a npr ILICA radi 14M5 i 14B5 
         }

         isMalop = lui.Flag;

       //ArtiklDao.GetArtiklWithArtstatList(conn, theArtiklWithArtstatList, lui.Cd, rptFilter.DatumDo, rptFilter, "", "artiklName ");
         ArtiklDao.GetArtiklWithArtstatList(conn, theArtiklWithArtstatList, lui.Cd, _dateInv         , rptFilter, "", "artiklName ");

         theArtiklWithArtstatList_InvVISAK  = theArtiklWithArtstatList.Where(artWars => artWars.AS_InvDiff.IsPositive()).ToList();
         theArtiklWithArtstatList_InvMANJAK = theArtiklWithArtstatList.Where(artWars => artWars.AS_InvDiff.IsNegative()).ToList();

         // 22.12.2015: start 
         bool invDokumentExists = FakturExistsFor_Sklad_And_TT_And_Date(conn, lui.Cd, isMalop ? Faktur.TT_INM : Faktur.TT_INV, _dateInv);
         if(invDokumentExists == false && theArtiklWithArtstatList.Count.NotZero())
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "Za skladiste: {0}\n\nna dan: {1}\n\n\nNE POSTOJI dokument inventure.\n\nZa ovo skladiste preskačem Viš/Manj proceduru.", lui.Cd, _dateInv);
            theArtiklWithArtstatList.Clear();
            continue;
         }

       //bool primkaDokumentExists = FakturExistsFor_Sklad_And_TT_And_Date(conn, lui.Cd, isMalop ? Faktur.TT_KLK : Faktur.TT_PRI, _dateInv);
       //if(primkaDokumentExists == true && theArtiklWithArtstatList.Count.NotZero())
       //{
       //   ZXC.aim_emsg(MessageBoxIcon.Warning, "Za skladiste: {0}\n\nna dan: {1}\n\n\nVEĆ POSTOJI dokument 'PRI' ili 'KLK'.\n\nZa ovo skladiste preskačem Viš/Manj proceduru.", lui.Cd, _dateInv);
       //   continue;
       //}
       //bool izdatDokumentExists = FakturExistsFor_Sklad_And_TT_And_Date(conn, lui.Cd, isMalop ? Faktur.TT_IZM : Faktur.TT_IZD, _dateInv);
       //if(izdatDokumentExists == true && theArtiklWithArtstatList.Count.NotZero())
       //{
       //   ZXC.aim_emsg(MessageBoxIcon.Warning, "Za skladiste: {0}\n\nna dan: {1}\n\n\nVEĆ POSTOJI dokument 'IZM' ili 'IZD'.\n\nZa ovo skladiste preskačem Viš/Manj proceduru.", lui.Cd, _dateInv);
       //   continue;
       //}

         // 22.12.2015: end 

         if(theArtiklWithArtstatList_InvVISAK.Count.NotZero())
         {
            uint fakturRecID = CreateInvDiffFaktur(conn, theArtiklWithArtstatList_InvVISAK, lui.Cd, isMalop, _dateInv, true, ref PRIcount, ref IZDcount, ref KLKcount, ref IZMcount);

            string tt = isMalop ? Faktur.TT_KLK : Faktur.TT_PRI;
            ZXC.TheVvForm.OpenNew_Record_TabPage(ZXC.TtInfo(tt).DefaultSubModulXY, fakturRecID);

            if(ZXC.IsTEXTHOany == false) // pretpostavka je da smo za TEXTHOany em saznali partnera, em ...
            {
               ZXC.TheVvForm.EditRecord_OnClick("INV VIŠAK", EventArgs.Empty);
               FakturExtDUC theDUC = ZXC.TheVvForm.TheVvUC as FakturExtDUC;
               theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
               //ZXC.TheVvForm.SaveRecord_OnClick("INV VIŠAK", EventArgs.Empty);
               ZXC.aim_emsg("Na kraju procesa zadajte partnera na ovu PRIMKU (skladište na koje se inventura odnosi)\n\nte usnimite dokument.");
            }
         }

         if(theArtiklWithArtstatList_InvMANJAK.Count.NotZero())
         {
            uint fakturRecID = CreateInvDiffFaktur(conn, theArtiklWithArtstatList_InvMANJAK, lui.Cd, isMalop, _dateInv, false, ref PRIcount, ref IZDcount, ref KLKcount, ref IZMcount);

            string tt = isMalop ? Faktur.TT_IZM : Faktur.TT_IZD;
            ZXC.TheVvForm.OpenNew_Record_TabPage(ZXC.TtInfo(tt).DefaultSubModulXY, fakturRecID);

            if(ZXC.IsTEXTHOany == false) // pretpostavka je da smo za TEXTHOany em saznali partnera, em ...
            {
               ZXC.TheVvForm.EditRecord_OnClick("INV MANJAK", EventArgs.Empty);
               FakturExtDUC theDUC = ZXC.TheVvForm.TheVvUC as FakturExtDUC;
               theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
               //ZXC.TheVvForm.SaveRecord_OnClick("INV MANJAK", EventArgs.Empty);
               ZXC.aim_emsg("Na kraju procesa zadajte partnera na ovu IZDATNICU (skladište na koje se inventura odnosi)\n\nte usnimite dokument.");
            }
         }

         theArtiklWithArtstatList.Clear();

      } // foreach(VvLookUpItem lui in skladListW_sklNum) 

      return true;
   }

   private static /*void*/ uint CreateInvDiffFaktur(XSqlConnection conn, List<Artikl> theArtiklWithArtstatList, string _skladCD, bool _isMalopSkl, DateTime _dokDate, bool isVISAK, ref uint PRIcount, ref uint IZDcount, ref uint KLKcount, ref uint IZMcount)
   {
      #region Set RtransList

      Rtrans rtrans_rec;
      List<Rtrans> rtransList = new List<Rtrans>(theArtiklWithArtstatList.Count);
      VvLookUpItem pdvLui;
      decimal pdvSt;
      bool isVelepSkl = !_isMalopSkl;
      bool isMANJAK   = !isVISAK;

      foreach(Artikl artikl_rec in theArtiklWithArtstatList)
      {
         rtrans_rec = new Rtrans();
         
         rtrans_rec.T_artiklCD   = artikl_rec.ArtiklCD;
         rtrans_rec.T_artiklName = artikl_rec.ArtiklName;
         rtrans_rec.T_jedMj      = artikl_rec.JedMj;
         
         rtrans_rec.T_kol  = Math.Abs(artikl_rec.AS_InvDiff );
         rtrans_rec.T_kol2 = Math.Abs(artikl_rec.AS_InvDiff2);
         rtrans_rec.T_cij  = artikl_rec.AS_PrNabCij; // ako je malop, ovo je dobro za VISAK a za manjak NE, nego ce ga preje'ati line 3180 
          
         // 14.12.2015: da bi CalcTransResults znao tko sam...
         if(isVISAK)       rtrans_rec.T_TT = _isMalopSkl ? Faktur.TT_KLK : Faktur.TT_PRI;
         else /* MANJAK */ rtrans_rec.T_TT = _isMalopSkl ? Faktur.TT_IZM : Faktur.TT_IZD;

         #region Malop Additions

         if(_isMalopSkl)
         {
            if(artikl_rec.PdvKat.NotEmpty())
            {
               pdvLui = ZXC.luiListaPdvKat.GetLuiForThisCd(artikl_rec.PdvKat);
               pdvSt = pdvLui.Number;
            }
            else
            {
               pdvSt = Faktur.CommonPdvStForThisDate(_dokDate);
            }

            rtrans_rec.T_pdvSt = pdvSt;

            rtrans_rec.T_mCalcKind = ZXC.MalopCalcKind.By_MPC; // !!! 

            // 31.12.2015: tek sada rastavili logiku za KLK/IZM (IZMu treba MPC doci u t_cij, a KLKu MPC treba doci u t_wanted) 
            if(isVISAK)
            {
               rtrans_rec.T_wanted    = artikl_rec.AS_KnjigCij;
            }
            else // isManjak 
            {
               rtrans_rec.T_cij       = artikl_rec.AS_KnjigCij;
            }
         }

         #endregion Malop Additions
         
         rtrans_rec.CalcTransResults(null);
         
         rtransList.Add(rtrans_rec);
      }

      #endregion Set RtransList

      #region Set faktur_rec

      // 30.12.2013: 
      // Nota Bene: za malop skl moras uci u ispravi pa dodati partnera, a i ispraviti naku kolicinu da se digne Calc dokumenta! 

      Faktur faktur_rec = new Faktur();
      ushort line = 0;

      #region counters

      if(isVISAK  &&  isVelepSkl) PRIcount++;
      if(isVISAK  && _isMalopSkl) KLKcount++;
      if(isMANJAK &&  isVelepSkl) IZDcount++;
      if(isMANJAK && _isMalopSkl) IZMcount++;

      #endregion counters

      if(isVISAK)
      {
         faktur_rec.TT = _isMalopSkl ? Faktur.TT_KLK : Faktur.TT_PRI;
         faktur_rec.Napomena = "INVENTURA - VIŠAK";
      }
      else // MANJAK 
      {
         faktur_rec.TT = _isMalopSkl ? Faktur.TT_IZM : Faktur.TT_IZD;
         faktur_rec.Napomena = "INVENTURA - MANJAK";
      }

      faktur_rec.SkladDate = // !!! 
      faktur_rec.DokDate   = _dokDate;
      faktur_rec.SkladCD   = _skladCD;

      // 14.12.2015: sve sume na drugi nacin 
    //faktur_rec.S_ukK        =       rtransList.Sum(rtr => rtr.T_kol   ); // tek 15.12.2015 mqjstore! 
    //faktur_rec.S_ukK2       =       rtransList.Sum(rtr => rtr.T_kol2  );
    //faktur_rec.S_ukKC       =       rtransList.Sum(rtr => rtr.R_KC    );
    //faktur_rec.S_ukRbt1     =       rtransList.Sum(rtr => rtr.R_rbt1  );
    //faktur_rec.S_ukKCR      =       rtransList.Sum(rtr => rtr.R_KCR   );
    //faktur_rec.S_ukKCRM     =       rtransList.Sum(rtr => rtr.R_KCRM  );
    //faktur_rec.S_ukKCRP     =       rtransList.Sum(rtr => rtr.R_KCRP  );
    //faktur_rec.S_ukTrnCount = (uint)rtransList.Count();

      faktur_rec.Transes = rtransList;
      faktur_rec.TakeTransesSumToDokumentSum(true);
      faktur_rec.Transes = null;

      // 15.12.2015: za TEXTHO se partner moze saznati ma osnovi dvoznam. korjena SkladCD-a 
      if(ZXC.IsTEXTHOany)
      {
         Kupdob kupdobTEXTHO_rec = Kupdob.GetKupdobFromSkladCD_TEXTHO(faktur_rec.SkladCD);
         if(kupdobTEXTHO_rec != null)
         {
            faktur_rec.KupdobCD   = kupdobTEXTHO_rec.KupdobCD;
            faktur_rec.KupdobTK   = kupdobTEXTHO_rec.Ticker  ;
            faktur_rec.KupdobName = kupdobTEXTHO_rec.Naziv   ;
         }
      }

      #endregion Set faktur_rec

      #region foreach Rtrans AutoSetFaktur

      foreach(Rtrans rtrans in rtransList)
      {
         FakturDao.AutoSetFaktur(conn, ref line, faktur_rec, rtrans);
      }

      #endregion foreach Rtrans AutoSetFaktur

      // 12.01.2017: epi brsdej, Tamara 
      if(ZXC.IsTEXTHOany)
      {
         if(faktur_rec.RecID.NotZero() && ZXC.IsSkyEnvironment) VvDaoBase.SendWriteOperationToSKY(ZXC.TheSecondDbConn_SameDB, faktur_rec, VvSQL.DB_RW_ActionType.ADD, /*true*/false);
      }

      return faktur_rec.RecID;
   }

   #endregion AutoAddInventuraDiff_RISK

   #region CheckTtNum_Slijednost

   internal static List<ZXC.VvUtilDataPackage> CheckTtNum_Slijednost(XSqlConnection conn, string skladCD, string tt, uint justAddedTtNum, DateTime justAddedDokDate, bool isDateXDateIzd)
   {
      List<ZXC.VvUtilDataPackage> theList = new List<ZXC.VvUtilDataPackage>();

      bool success = true;
      ZXC.VvUtilDataPackage the_rec;

      uint skladBR = (uint)ZXC.luiListaSkladista.GetIntegerForThisCd(skladCD);

      // 18.03.2014: Komisija News 
      VvLookUpItem baseLUI = ZXC.luiListaSkladista.GetBaseSkladLUI(skladCD);
      if(baseLUI != null)
      {
         skladCD = baseLUI.Cd;
         skladBR = (uint)baseLUI.Integer;
      }

      ZXC.sqlErrNo = 0;

      using(XSqlCommand cmd = (VvSQL.CheckTtNum_Slijednost_Command(conn, skladCD, tt, isDateXDateIzd)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;
               while(success && reader.Read())
               {
                  the_rec = new ZXC.VvUtilDataPackage(reader.GetDateTime(0),
                                                      reader.GetUInt32  (1),
                                                      /*(int)++okNum*/ 0); // dummy 
                  theList.Add(the_rec);

               }
               reader.Close();
            }
         }
         catch(XSqlException ex) { success = false; VvSQL.ReportSqlError("CheckTtNum_Slijednost", ex, System.Windows.Forms.MessageBoxButtons.OK); ZXC.sqlErrNo = ex.Number; return null; }
      } // using 

      // Za upravo dodanoga: 
      the_rec = new ZXC.VvUtilDataPackage(justAddedDokDate   ,
                                          justAddedTtNum     ,
                                          /*(int)++okNum*/ 0); // dummy 
      theList.Add(the_rec);


      //========== NOVO: treba presortirati za ovog novododanog ==================

      theList = theList.OrderBy(udp => udp.TheDate).ThenBy(udp => udp.TheUint).ToList();

      uint okNum = skladBR * /*100000*/ Faktur.BaseTtNum;

      // 13.11.204: 
      if(skladBR > 9 /*dvoznamenkasti*/ && tt != Faktur.TT_IRM) okNum /= 10; 

      for(int i = 0; i < theList.Count; ++i)
      {
         if(++okNum == theList[i].TheUint)
         {
            theList.RemoveAt(i--);
         }
         else
         {
            the_rec        = theList[i];
            the_rec.TheInt = (int)okNum;
            theList[i]     = the_rec;
         }
      }

      return theList;
   }

   internal static uint GetLastUsedTtNumFrom_SKY(XSqlConnection skyConn, string skladCD, string tt)
   {
      uint lastUsedTtNumFrom_SKY = 0;

      return lastUsedTtNumFrom_SKY;
   }

   #endregion CheckTtNum_Slijednost

   #region Check NOT fiskalized PRIHOD_TT Rns

   //public static bool CheckNOTfiskalizedRns()
   //{
   //   if(ZXC.CURR_prjkt_rec.IsFiskalOnline == false) return false;
   //
   //   string origDatabase = ZXC.TheMainDbConnection.Database;
   //   ZXC.SetMainDbConnDatabaseName(VvSQL.GetDbNameForThisTableName(Faktur.recordName));
   //
   //   uint descrepancyCount = CountNOTfiskalizedRns(ZXC.TheMainDbConnection);
   //
   //   ZXC.SetMainDbConnDatabaseName(origDatabase);
   //
   //   return false;
   //}

   public static uint CountNOTfiskalized_IRMs(XSqlConnection conn)
   {
      object obj;
      uint   recCount;

    //using(XSqlCommand cmd = VvSQL.CountNOTfiskalizedRns_Command(conn, TtInfo.GetSql_IN_Clause(ZXC.TtInfoPrihod)))
      using(XSqlCommand cmd = VvSQL.CountNOTfiskalizedRns_Command(conn, TtInfo.GetSql_IN_Clause(new string[] { Faktur.TT_IRM } )))
      {
         try              { obj = cmd.ExecuteScalar(); }
         catch(Exception) { return(0); }

         try              { recCount = uint.Parse(obj.ToString()); }
         catch(Exception) { return(0); }
      }

      return (recCount);
   }

   #endregion Check NOT fiskalized PRIHOD_TT Rns

   #region BarcodeFile

   internal static List<ZXC.VvUtilDataPackage> Get_BarcodeFile_Content_ForDokument(string tt, uint ttNum)
   {
      string expectedBcFilePreffix = "VvBCS_";
      string expectedFileName      = expectedBcFilePreffix + ZXC.TtInfo(tt).TtSort.ToString() + ttNum.ToString() + ".TXT";
      string expectedDirectory     = ZXC.TheVvForm.VvPref.vvMailData.DirectoryName;
      string fullPathFileName      = System.IO.Path.Combine(expectedDirectory, expectedFileName);

      #region FileDialog

      OpenFileDialog openFileDialog   = new OpenFileDialog();
      openFileDialog.InitialDirectory = ZXC.TheVvForm.VvPref.vvMailData.DirectoryName;
      openFileDialog.FileName         = expectedFileName;
      openFileDialog.Filter           = "VvBCS datoteke (barkod skenovi)|VvBCS_*.TXT|Sve Datoteke (*.*)|*.*";
    //openFileDialog.Filter           = "(*" + thisDocumentID + "*)|" + "*" + thisDocumentID + "*" + "|All files (*.*)|*.*";
      openFileDialog.FilterIndex      = 1;
      openFileDialog.RestoreDirectory = true;

      if(openFileDialog.ShowDialog() != DialogResult.OK)
      {
         openFileDialog.Dispose(); // !!! 
         return null;
      }

      fullPathFileName              = openFileDialog.FileName;
      System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(fullPathFileName);

      string fileName                                      = dInfo.Name;
      string directoryName                                 = fullPathFileName.Substring(0, fullPathFileName.Length - (fileName.Length + 1));
      ZXC.TheVvForm.VvPref.vvMailData.DirectoryName = directoryName;

      openFileDialog.Dispose(); // !!! 

      Cursor.Current = Cursors.WaitCursor;
      //SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}");

      #endregion FileDialog

      return Get_BarcodeFile_Content(fullPathFileName);
   }

   private static List<ZXC.VvUtilDataPackage> Get_BarcodeFile_Content(string fullPathFileName)
   {
      string columnSeparator = ";";

      List<string> lineList = null;

      try
      {
         lineList = System.IO.File.ReadAllLines(fullPathFileName, ZXC.VvUTF8Encoding_noBOM).ToList();
      }
      catch (System.IO.FileNotFoundException ex)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nema datoteke\n\n{0}\n\n{1}", fullPathFileName, ex.Message);
         return null;
      }
      
      string[] splitters;
      List<ZXC.VvUtilDataPackage> barcodeWkolList = new List<ZXC.VvUtilDataPackage>();

      // TtNum     ArtiklCD   Barcode        Kol 
      // 46105145; Znalgesin; 2200041457766; 3   

      foreach(string line in lineList)
      {
         splitters = line.Split(columnSeparator.ToCharArray());

         ZXC.VvUtilDataPackage barcodeWkol = new ZXC.VvUtilDataPackage();

         if(splitters.Length > 0) barcodeWkol.TheUint    = ZXC.ValOrZero_UInt   (splitters[0]   ); // TtNum    
         if(splitters.Length > 1) barcodeWkol.TheStr1    =                      (splitters[1]   ); // ArtiklCD 
         if(splitters.Length > 2) barcodeWkol.TheStr2    =                      (splitters[2]   ); // Barcode  
         if(splitters.Length > 3) barcodeWkol.TheDecimal = ZXC.ValOrZero_Decimal(splitters[3], 0); // Kol      

         barcodeWkol.TheStr3 = line;

         barcodeWkolList.Add(barcodeWkol);
      }

      return barcodeWkolList;
   }

   internal static void Set_BarcodeFile_VvNames()
   {
      string expectedBcFilePreffix = "VvBCS_";
      string expectedDirectory     = ZXC.TheVvForm.VvPref.vvMailData.DirectoryName;
      string columnSeparator       = ";";

      // Ođe želimo sve 'out_yyyymmddhhmmss.txt'      
      // rastaviti (grupirati) po TtNum-u na fajlove: 
      // VvBCS_46100115 (46 - TtSort, 100115 - TtNum) 
      // a 'out_' file oznaciti sa prefixom 'vv_'     

      string searchPattern = "out_" + ZXC.projectYear + "*.txt";

      DirectoryInfo diInfo = new DirectoryInfo(expectedDirectory); // D:\MyDocuments\Viper.NET\Vektor\_ PDF i Export Datoteke 

      FileInfo[] fiArray = diInfo.GetFiles(searchPattern);

      if(fiArray.Length.IsZero()) return; // der'z notin tu du 

      List<ZXC.VvUtilDataPackage> barcodeWkolList;
      string vvbcs_fileName;

      foreach(FileInfo fileInfo in fiArray)
      {
         barcodeWkolList = Get_BarcodeFile_Content(fileInfo.FullName);

         foreach(var ttNumGR in barcodeWkolList.GroupBy(bc => bc.TheUint))
         {
            vvbcs_fileName = Path.Combine(fileInfo.DirectoryName, expectedBcFilePreffix + ttNumGR.First().TheUint + ".txt");

            File.WriteAllLines(vvbcs_fileName, ttNumGR.Select(gr => gr.TheStr3)); // u TheStr3 si gore u 'Get_BarcodeFile_Content()' spremio orginal ne splittanu liniju 
         }

         #region Rename files; add 'vv_' preffix 

         File.Move(fileInfo.FullName, fileInfo.FullName.Replace("out_", "vv_out_"));

         #endregion Rename files; add 'vv_' preffix 

      }

   }

   #endregion BarcodeFile

   #region GetPrihodTT_Skladista_InUse

   internal static List<ZXC.VvUtilDataPackage> GetPrihodTT_Skladista_InUse(XSqlConnection conn)
   {
      List<ZXC.VvUtilDataPackage> theList = new List<ZXC.VvUtilDataPackage>();

      bool success = true;
      ZXC.VvUtilDataPackage the_rec;

      using(XSqlCommand cmd = (VvSQL.GetPrihodTT_Skladista_InUse_Command(conn)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;
               while(success && reader.Read())
               {
                  the_rec = new ZXC.VvUtilDataPackage()
                  {
                     TheStr1 = reader.GetString(0),
                     TheStr2 = reader.GetString(1)
                  };

                  theList.Add(the_rec);

               }
               reader.Close();
            }
         }
         catch(XSqlException ex) { success = false; VvSQL.ReportSqlError("GetPrihodTT_Skladista_InUse", ex, System.Windows.Forms.MessageBoxButtons.OK); ZXC.sqlErrNo = ex.Number; return null; }

      }

      return theList;
   }

   #endregion GetPrihodTT_Skladista_InUse
}

public class Faktur2NalogRulesAndData
{

   #region Rulez / GetFieldz UC propertiz

   public KtoShemaDsc KtoShemaDsc { get; set; }

   public bool TT_OtvaranjeDUG { get; set; }
   public bool TT_OtvaranjePOT { get { return !TT_OtvaranjeDUG; } }

   public bool IsAutomatic { get; set; } // prebacivanje obavljeno automatski, bez intervencije korisnika 
   public bool PeriodDefinedVia_DokDate { get; set; }
   public bool PeriodDefinedVia_AddTS   { get { return !PeriodDefinedVia_DokDate; } }

   public ZXC.Faktur2NalogSetEnum      Fak2nalSet      { get; set; }
   public ZXC.Faktur2NalogSetEnum      Fak2nalSetLocal { get; set; }
   public ZXC.Faktur2NalogTimeRuleEnum Fak2nalTimeRule { get; set; }

   public string ThisTT_Only { get; set; }

   public DateTime DateOd { get; set; }
   public DateTime DateDo { get; set; }

   //public bool GroupIrmByNacPlac { get; set; }
   //public bool GroupIrmByMonth   { get; set; }

   public bool NeedsKupdobLine
   {
      get
      {
         if(ThisTT_Only == Faktur.TT_IZD) return false; // jer je IZD 'IZLAZNI_VP' pa bi inace return true 

         if(Fak2nalSetLocal == ZXC.Faktur2NalogSetEnum.ULAZNI_VP  ||
            Fak2nalSetLocal == ZXC.Faktur2NalogSetEnum.ULAZNI_MP  ||
            Fak2nalSetLocal == ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP ||
            Fak2nalSetLocal == ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP)
         {
            return true;
         }

         // Znaci, defaultno necemo KupdobLine osim za eksplicitno navedene u ovom prethodnom if-u 
         return false;
      }
   }

   // 13.02.2017: 
   public bool IsIraRealizOnly 
   { 
      get 
      { 
         return Fak2nalSetLocal == ZXC.Faktur2NalogSetEnum.IRA_RealizOnly ||
                Fak2nalSet      == ZXC.Faktur2NalogSetEnum.IRA_RealizOnly;
      }
   }


   #endregion Rulez / GetFieldz UC propertiz

   #region Explicit nalog or ftrans propertiz and metodz

   #region Nalog Ftrans propertiz

   public uint     NalogCount       { get; set; } 
   public string   NalogTT          { get; set; } 
   public string   NalogNapomena    { get; set; } 
   public DateTime NalogDokDate     { get; set; }
   public string   FtransTipBr      { get; set; }
   public string   FtransFakTT      { get; set; }
   public string   FtransOpis       { get; set; }
   public DateTime FtransValuta     { get; set; }
   public uint     FtransFakRecID   { get; set; }
   public uint     FtransKupdobCD   { get; set; }
   public uint     FtransMtrosCD    { get; set; }
   public string   FtransKupdobTK   { get; set; }
   public string   FtransMtrosTK    { get; set; }
   public string   FtransProjektCD  { get; set; }
   public string   FtransPozicija   { get; set; }
   public string   FtransFond       { get; set; }
                                    
   public bool     IsBLAGAJNA       { get; set; }
   public bool     IsSKIP_NonMAT    { get; set; }
   public bool     IsINTERN_UI      { get; set; }
   public bool     IsINTERN_UIorVMI { get { return IsINTERN_UI || Fak2nalSet == ZXC.Faktur2NalogSetEnum.VMI;         } }
   public bool     IsVMI            { get { return                Fak2nalSet == ZXC.Faktur2NalogSetEnum.VMI;         } }
 //public bool     IsIRM            { get { return                Fak2nalSet == ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP; } } 25.10.2021. da se IRM-ovi ipk knjize pojedinacno
   public bool     IsIRM            { get { return                Fak2nalSet == ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP ||
                                                                  Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT && ThisTT_Only == Faktur.TT_IRM;} }
   public bool     IsIRA            { get { return                Fak2nalSet == ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP ||
                                                                  Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT && ThisTT_Only == Faktur.TT_IRA; } }
   public bool     IsIZD            { get { return                Fak2nalSet == ZXC.Faktur2NalogSetEnum.IZLAZ_SK_VP ||
                                                                  Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT && ThisTT_Only == Faktur.TT_IZD; } }
   public bool     IsIZM            { get { return                Fak2nalSet == ZXC.Faktur2NalogSetEnum.IZLAZ_SK_MP ||
                                                                  Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT && ThisTT_Only == Faktur.TT_IZM; } }
   public bool     IsMVI            { get { return                Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT && ThisTT_Only == Faktur.TT_MVI; } }

   public bool     IsMALOP_U        { get; set; }
   public bool     IsMALOP_UorVMI   { get { return IsMALOP_U              || IsVMI; } }
   public bool     IsMALOP_I        { get; set; }
   public bool     IsMALOP_UI       { get { return IsMALOP_U || IsMALOP_I         ; } }
   public bool     IsMALOP_UIorVMI  { get { return IsMALOP_U || IsMALOP_I || IsVMI; } }

   public ZXC.PdvKnjigaEnum     FtransPdvKnjiga  { get; set; }
   public ZXC.ShouldFak2NalEnum FtransCarinaKind { get; set; }

   public string  FtransKonto { get; set; }
   public decimal FtransDug   { get; set; }
   public decimal FtransPot   { get; set; }
   public decimal FtransMoney { get; set; }
   
   #endregion Nalog Ftrans propertiz

   internal void   SetCommonFtransPropertiz(XSqlConnection conn, string rasterID, int presumedNumOfNewNalog, Faktur faktur_rec)
   {
      SetNalogTT(faktur_rec.TT);
      SetNalogNapomena(rasterID, presumedNumOfNewNalog);
      SetFtransOpis(faktur_rec);

      NalogDokDate     = faktur_rec.DokDate.Date;
      FtransFakRecID   = faktur_rec.RecID;
      FtransTipBr      = faktur_rec.TipBr;
      FtransFakTT      = faktur_rec.TT;
      FtransValuta     = faktur_rec.DospDate.Date;
      FtransMtrosCD    = faktur_rec.MtrosCD;
      FtransMtrosTK    = faktur_rec.MtrosTK;
      FtransProjektCD  = faktur_rec.ProjektCD;
      FtransPdvKnjiga  = faktur_rec.PdvKnjiga;
      FtransCarinaKind = faktur_rec.ShouldFak2Nal;

      //############################################################################################# 
      // 14.10.2015: ovak je bilo
    //if(ZXC.CURR_prjkt_rec.PlanKind == ZXC.PlanKind.PlnBy_POZICIJA)
    //{
    //   FtransPozicija = VvRtransImporter.Limited_Ftrans(faktur_rec.VezniDok2, ZXC.FtrCI.t_pozicija);
    //}
    //else FtransPozicija = "";
    //
    //if(ZXC.CURR_prjkt_rec.PlanKind == ZXC.PlanKind.PlnBy_FOND)
    //{
    //   FtransFond = VvRtransImporter.Limited_Ftrans(faktur_rec.VezniDok2, ZXC.FtrCI.t_fond);
    //}
    //else FtransFond = "";
      // 14.10.2015: ovak je sada
      if(ZXC.CURR_prjkt_rec.PlanKind == ZXC.PlanKindEnum.PlnBy_POZICIJA) // TURZML
      {
         FtransPozicija = VvRtransImporter.Limited_Ftrans(faktur_rec.VezniDok2, ZXC.FtrCI.t_pozicija);
      }
      else FtransPozicija = "";

      if(ZXC.CURR_prjkt_rec.PlanKind == ZXC.PlanKindEnum.PlnBy_FOND) // keremp 
      {
         FtransPozicija = VvRtransImporter.Limited_Ftrans(faktur_rec.VezniDok2, ZXC.FtrCI.t_pozicija);
         FtransFond     = VvRtransImporter.Limited_Ftrans(faktur_rec.Fco      , ZXC.FtrCI.t_fond    );
      }
      else FtransFond = "";
      //############################################################################################# 

      IsBLAGAJNA  = faktur_rec.TT == Faktur.TT_UPL || faktur_rec.TT == Faktur.TT_ISP || faktur_rec.TT == Faktur.TT_BUP || faktur_rec.TT == Faktur.TT_BIS;

      // 02.03.2017: 
    //IsSKIP_NonMAT =                            faktur_rec.TT == Faktur.TT_PPR || faktur_rec.TT == Faktur.TT_POV;
      IsSKIP_NonMAT = (ZXC.IsRNMnotRNP ? false : faktur_rec.TT == Faktur.TT_PPR || faktur_rec.TT == Faktur.TT_POV); // Metaflexu idu svi 

      IsINTERN_UI = faktur_rec.TT == Faktur.TT_PPR || faktur_rec.TT == Faktur.TT_POV ||
                    faktur_rec.TT == Faktur.TT_MSI || faktur_rec.TT == Faktur.TT_MSU ||
                    faktur_rec.TT == Faktur.TT_VMI || faktur_rec.TT == Faktur.TT_VMU ||
                    faktur_rec.TT == Faktur.TT_TRI || faktur_rec.TT == Faktur.TT_TRM ||
                    faktur_rec.TT == Faktur.TT_PIZ || faktur_rec.TT == Faktur.TT_PUL ||
                    faktur_rec.TT == Faktur.TT_PIX || faktur_rec.TT == Faktur.TT_PUX ||
                    faktur_rec.TT == Faktur.TT_IMT || faktur_rec.TT == Faktur.TT_PIP || // TT_PIP dodan 24.03.2016.

                    Fak2nalSetLocal == ZXC.Faktur2NalogSetEnum.ULAZ_INT  ||
                    Fak2nalSetLocal == ZXC.Faktur2NalogSetEnum.PROIZ_IZLAZ;

      IsMALOP_U = faktur_rec.TtInfo.IsMalopFak2Nal_U;
      IsMALOP_I = faktur_rec.TtInfo.IsMalopFak2Nal_I;

      #region STORNO additions

      if(faktur_rec.Napomena.ToUpper().Contains("STORNO")) // faktur_rec je STORNO nekog drugog faktur_rec-a (malo ti je labavo pravilo, sta ako neko u napomeni makne rijec 'STORNO'!?) 
      {
         if(faktur_rec.V1_tt.NotEmpty() && faktur_rec.V1_ttNum.NotZero())
         {
            //bool OK;

            //Faktur faktur_rec_kojiJeBijoStorniran = new Faktur();
            //OK = FakturDao.SetMeFaktur(conn, faktur_rec_kojiJeBijoStorniran, faktur_rec.V1_tt, faktur_rec.V1_ttNum, false);

            //if(OK)
            //{
            //   FtransFakRecID = faktur_rec_kojiJeBijoStorniran.RecID;
                 FtransTipBr    = Faktur.Set_TT_And_TtNum(faktur_rec.V1_tt, faktur_rec.V1_ttNum);
                 FtransOpis     = "Ovo je " + faktur_rec.TipBr + " (STORNO od " + FtransTipBr + ")";
            //}
         }

         // NOTA BENE!!! Ovdje treba podmetnuti TT i TtNum dokumenta kojega je ovaj dok. stornirao, ALI ostaviti FtransFakRecID realan, inace bi svaki put nanovo Fak2Nal ponovno prebacivao ovaj storno. 
      }

      #endregion STORNO additions

      if(faktur_rec.PrimPlatCD.NotZero()) { FtransKupdobCD = faktur_rec.PrimPlatCD; FtransKupdobTK = faktur_rec.PrimPlatTK; }
      else                                { FtransKupdobCD = faktur_rec.KupdobCD;   FtransKupdobTK = faktur_rec.KupdobTK;   }

   }
   
   internal void   SetKontoAndMoney_Skl4IntUI(Faktur faktur_rec)
   {
    //decimal money = faktur_rec.Transes       .Sum(rtr => rtr.R_Kol_Puta_PrNabCij);
      decimal money;

      //IsSKIP_NonMAT = faktur_rec.TT == Faktur.TT_PPR || faktur_rec.TT == Faktur.TT_POV;
      if(this.IsSKIP_NonMAT)
      {
         money = faktur_rec.TransesWOtwins
            .Where(rtr => rtr.A_IsMaterijal)
            // 18.11.2013: 
          //.Sum(rtr => rtr.R_Kol_Puta_PrNabCij); // zasada PPR, POV; Knjizi samo materijal, preskaci POT i slicno... 
            .Sum(rtr => rtr.R_KC               ); // zasada PPR, POV; Knjizi samo materijal, preskaci POT i slicno... 
      }
      else
      {
         // 18.11.2013: 
       //money = faktur_rec.TransesWOtwins.Sum(rtr => rtr.R_Kol_Puta_PrNabCij); // odervajs, knjizi sve 
         // 02.03.2017: dodan if za razlucenje 'classic' ili nova 2017 PPR potreba 
         if(faktur_rec.TT == Faktur.TT_PPR)
         {
            money = faktur_rec.TransesWOtwins.Sum(rtr => rtr.R_byTheAsEx_bezUsl_KC); // odervajs, knjizi sve 
         }
         else // classic 
         {
            money = faktur_rec.TransesWOtwins.Sum(rtr => rtr.R_KC                 ); // odervajs, knjizi sve 
         }
      }

      SetDugAndPot(TT_OtvaranjeDUG, money);

      FtransKonto = GetSkladKontoForSkladCD(faktur_rec.SkladCD);

    //25.05.2015. kada nije zadan u luiListi onda je 0 a nije prazan
    //if(FtransKonto.IsEmpty())
      if(FtransKonto.IsEmpty() || FtransKonto == "0")
      {
         if(faktur_rec.Transes.Count.NotZero() && faktur_rec.Transes[0].TheAsEx.IsSitniInv) FtransKonto = KtoShemaDsc.Dsc_KtoSInvSklad; 
         else                                                                               FtransKonto = KtoShemaDsc.Dsc_KtoMatSklad ; // default for internUI is materijal
      }
   }
   internal void   SetKontoAndMoney_Kupdob   (Faktur faktur_rec)
   {
      decimal kupdobMoney = faktur_rec.S_ukKCRP;

      // 09.07.2013: 
      if(faktur_rec.PdvGEOkind != ZXC.PdvGEOkindEnum.HR)
      {
         kupdobMoney = faktur_rec.S_ukKCR;
      }

      if(faktur_rec.ShouldFak2Nal == ZXC.ShouldFak2NalEnum.Knjizi_SamoPDV_i_Carinu)
      {
         kupdobMoney -= faktur_rec.R_ukOsn25_23_22_10_05; // umanji S_ukKCRP za R_ukOsn23_22_10 
      }

      SetDugAndPot(TT_OtvaranjeDUG, kupdobMoney);

      if(faktur_rec.Konto.NotEmpty())
      {
         FtransKonto = faktur_rec.Konto;
      }
      else
      {
         switch(Fak2nalSetLocal)
         {
            #region Faktur2NalogSetEnum.ULAZNI (MALOP & VELEP)

            case ZXC.Faktur2NalogSetEnum.ULAZNI_VP:
            case ZXC.Faktur2NalogSetEnum.ULAZNI_MP: 

               switch(faktur_rec.PdvKnjiga)
               {
                  case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Dobav; break;
                  case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Dobav; break;
                  case ZXC.PdvKnjigaEnum.UVOZ_ROB: FtransKonto = KtoShemaDsc.Dsc_UKto_Dobav; break;
                  case ZXC.PdvKnjigaEnum.UVOZ_USL: FtransKonto = KtoShemaDsc.Dsc_SKto_Dobav; break;
                  
                  case ZXC.PdvKnjigaEnum.NIJEDNA : FtransKonto = KtoShemaDsc.Dsc_UKto_Dobav; // new 14.07.2013.
                     
                     switch(faktur_rec.PdvGEOkind)
                     {
                        case ZXC.PdvGEOkindEnum.EU: FtransKonto = KtoShemaDsc.Dsc_KupDobR_EU; break; 
                        case ZXC.PdvGEOkindEnum.BS: FtransKonto = KtoShemaDsc.Dsc_KupDob_BS ; break;
                        case ZXC.PdvGEOkindEnum.TP: FtransKonto = KtoShemaDsc.Dsc_KupDob_TP ; break; 
                        case ZXC.PdvGEOkindEnum.HR: FtransKonto = KtoShemaDsc.Dsc_RKto_Dobav; break; // 18.02.2019; od kada traže Xml PDV-URA, sa knjiga 'NIJEDNA' izbacujemo rn koji nisu u pdv-u pa je trebalo i ovo dodati jer je inace islo u uvoz 
                     }
                  break;
               }
               break;

            #endregion Faktur2NalogSetEnum.ULAZNI

            #region Faktur2NalogSetEnum.IZLAZNI_VP

            case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP: 

               switch(faktur_rec.PdvKnjiga)
               {
                  case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Kupca; break;
                  case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Kupca; break;
               }
               break;

            #endregion Faktur2NalogSetEnum.IZLAZNI_VP

            #region Faktur2NalogSetEnum.IZLAZNI_MP

            //case ZXC.Faktur2NalogSetEnum.IZLAZNI_MP:
            //   if(faktur_rec.IsNpCash) FtransKonto = KtoShemaDsc.Dsc_IrmKupciCash; 
            //   else                    FtransKonto = KtoShemaDsc.Dsc_Mir_Kupci; 
            //   break;
                     
            #endregion Faktur2NalogSetEnum.IZLAZNI_MP

            #region Faktur2NalogSetEnum.BLAGAJNA

            case ZXC.Faktur2NalogSetEnum.BLAGAJNA:

               switch(faktur_rec.TT)
               {
                  case Faktur.TT_UPL: 
                  case Faktur.TT_BUP: FtransKonto = KtoShemaDsc.Dsc_Blg_Promet; break;
                  case Faktur.TT_ISP: 
                  case Faktur.TT_BIS: FtransKonto = KtoShemaDsc.Dsc_Blg_Promet; break;
               }

               break;

            #endregion Faktur2NalogSetEnum.BLAGAJNA
         }
      }
   }
   internal void   SetKontoAndMoney_Pdv_25m  (Faktur faktur_rec, decimal money, ZXC.PdvGEOkindEnum geoKind, bool isUsluga, bool isKontra)
   {
      bool isOtvaranjePOT = !TT_OtvaranjeDUG;

      if(isKontra) isOtvaranjePOT = !isOtvaranjePOT;

      SetDugAndPot(isOtvaranjePOT, /*faktur_rec.S_ukPdv25m*/money);

      switch(Fak2nalSetLocal)
      {
         #region Faktur2NalogSetEnum.ULAZNI  (MALOP & VELEP)

         case ZXC.Faktur2NalogSetEnum.ULAZNI_VP:
         case ZXC.Faktur2NalogSetEnum.ULAZNI_MP:

            switch(faktur_rec.PdvKnjiga)
            {
               case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Pdv25m_Ura; break;
               case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Pdv25m_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_ROB: FtransKonto = KtoShemaDsc.Dsc_UKto_Pdv25m_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_USL: FtransKonto = KtoShemaDsc.Dsc_SKto_Pdv25m_Ura; break;
               
               case ZXC.PdvKnjigaEnum.NIJEDNA: // upaTODO: !!!!!! 

                  switch(faktur_rec.PdvGEOkind)
                  {
                     case ZXC.PdvGEOkindEnum.EU: FtransKonto = isUsluga ? (isKontra ? KtoShemaDsc.Dsc_ObrPdvU25_EU : KtoShemaDsc.Dsc_PdvU25m_EU): 
                                                                          (isKontra ? KtoShemaDsc.Dsc_ObrPdvR25_EU : KtoShemaDsc.Dsc_PdvR25m_EU); break;
                     case ZXC.PdvGEOkindEnum.BS:             FtransKonto = isKontra ? KtoShemaDsc.Dsc_ObrPdv25_BS  : KtoShemaDsc.Dsc_Pdv25m_BS  ; break;
                     case ZXC.PdvGEOkindEnum.TP:             FtransKonto = isKontra ? KtoShemaDsc.Dsc_ObrPdv25_TP  : KtoShemaDsc.Dsc_Pdv25m_TP  ; break; 
                  }
                  break;
            }

            break; // switch(Fak2nalSetLocal) 

         #endregion Faktur2NalogSetEnum.ULAZNI

         #region Faktur2NalogSetEnum.IZLAZNI_VP

         case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP:

            switch(faktur_rec.PdvKnjiga)
            {
               case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Pdv25_Ira; break;
               case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Pdv25_Ira; break;
            }
            break;

         #endregion Faktur2NalogSetEnum.IZLAZNI_VP

         #region Faktur2NalogSetEnum.IZLAZNI_MP

         case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP: FtransKonto = KtoShemaDsc.Dsc_KtoPdv25_Mir;
            break;

         #endregion Faktur2NalogSetEnum.IZLAZNI_MP

         #region Faktur2NalogSetEnum.BLAGAJNA

         #endregion Faktur2NalogSetEnum.BLAGAJNA
      }
   }
   internal void   SetKontoAndMoney_Pdv_25n  (Faktur faktur_rec, decimal money, ZXC.PdvGEOkindEnum geoKind, bool isUsluga, bool isKontra)
   {
      bool isOtvaranjePOT = !TT_OtvaranjeDUG;

      if(isKontra) isOtvaranjePOT = !isOtvaranjePOT;

      SetDugAndPot(isOtvaranjePOT, /*faktur_rec.S_ukPdv25n*/money);

      switch(Fak2nalSetLocal)
      {
         #region Faktur2NalogSetEnum.ULAZNI (MALOP & VELEP)

         case ZXC.Faktur2NalogSetEnum.ULAZNI_VP:
         case ZXC.Faktur2NalogSetEnum.ULAZNI_MP:

            switch(faktur_rec.PdvKnjiga)
            {
               case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Pdv25n_Ura; break;
               case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Pdv25n_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_ROB: FtransKonto = KtoShemaDsc.Dsc_UKto_Pdv25n_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_USL: FtransKonto = KtoShemaDsc.Dsc_SKto_Pdv25n_Ura; break;
               
               case ZXC.PdvKnjigaEnum.NIJEDNA: // upaTODO: !!!!!! 

                  switch(faktur_rec.PdvGEOkind)
                  {
                     case ZXC.PdvGEOkindEnum.EU: FtransKonto = isUsluga ? (isKontra ? KtoShemaDsc.Dsc_ObrPdvU25_EU : KtoShemaDsc.Dsc_PdvU25n_EU): 
                                                                          (isKontra ? KtoShemaDsc.Dsc_ObrPdvR25_EU : KtoShemaDsc.Dsc_PdvR25n_EU); break;
                     case ZXC.PdvGEOkindEnum.BS:             FtransKonto = isKontra ? KtoShemaDsc.Dsc_ObrPdv25_BS : KtoShemaDsc.Dsc_Pdv25n_BS   ; break;
                     case ZXC.PdvGEOkindEnum.TP:             FtransKonto = isKontra ? KtoShemaDsc.Dsc_ObrPdv25_TP : KtoShemaDsc.Dsc_Pdv25n_TP   ; break; 
                  }
                  break;
            }
            break;

         #endregion Faktur2NalogSetEnum.ULAZNI

         #region Faktur2NalogSetEnum.BLAGAJNA

         #endregion Faktur2NalogSetEnum.BLAGAJNA
      }
   }
   internal void   SetKontoAndMoney_Pdv_23m  (Faktur faktur_rec)
   {
      SetDugAndPot(!TT_OtvaranjeDUG, faktur_rec.S_ukPdv23m);

      switch(Fak2nalSetLocal)
      {
         #region Faktur2NalogSetEnum.ULAZNI  (MALOP & VELEP)

         case ZXC.Faktur2NalogSetEnum.ULAZNI_VP:
         case ZXC.Faktur2NalogSetEnum.ULAZNI_MP:

            switch(faktur_rec.PdvKnjiga)
            {
               case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Pdv23m_Ura; break;
               case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Pdv23m_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_ROB: FtransKonto = KtoShemaDsc.Dsc_UKto_Pdv23m_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_USL: FtransKonto = KtoShemaDsc.Dsc_SKto_Pdv23m_Ura; break;
            }
            break;

         #endregion Faktur2NalogSetEnum.ULAZNI

         #region Faktur2NalogSetEnum.IZLAZNI_VP

         case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP:

            switch(faktur_rec.PdvKnjiga)
            {
               case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Pdv23_Ira; break;
               case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Pdv23_Ira; break;
            }
            break;

         #endregion Faktur2NalogSetEnum.IZLAZNI_VP

         #region Faktur2NalogSetEnum.IZLAZNI_MP

         case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP: FtransKonto = KtoShemaDsc.Dsc_KtoPdv23_Mir;
            break;

         #endregion Faktur2NalogSetEnum.IZLAZNI_MP

         #region Faktur2NalogSetEnum.BLAGAJNA

         #endregion Faktur2NalogSetEnum.BLAGAJNA
      }
   }
   internal void   SetKontoAndMoney_Pdv_23n  (Faktur faktur_rec)
   {
      SetDugAndPot(!TT_OtvaranjeDUG, faktur_rec.S_ukPdv23n);

      switch(Fak2nalSetLocal)
      {
         #region Faktur2NalogSetEnum.ULAZNI (MALOP & VELEP)

         case ZXC.Faktur2NalogSetEnum.ULAZNI_VP:
         case ZXC.Faktur2NalogSetEnum.ULAZNI_MP:

            switch(faktur_rec.PdvKnjiga)
            {
               case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Pdv23n_Ura; break;
               case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Pdv23n_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_ROB: FtransKonto = KtoShemaDsc.Dsc_UKto_Pdv23n_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_USL: FtransKonto = KtoShemaDsc.Dsc_SKto_Pdv23n_Ura; break;
            }
            break;

         #endregion Faktur2NalogSetEnum.ULAZNI

         #region Faktur2NalogSetEnum.BLAGAJNA

         #endregion Faktur2NalogSetEnum.BLAGAJNA
      }
   }
   internal void   SetKontoAndMoney_Pdv_22m  (Faktur faktur_rec)
   {
      SetDugAndPot(!TT_OtvaranjeDUG, faktur_rec.S_ukPdv22m);

      switch(Fak2nalSetLocal)
      {
         #region Faktur2NalogSetEnum.ULAZNI (MALOP & VELEP)

         case ZXC.Faktur2NalogSetEnum.ULAZNI_VP:
         case ZXC.Faktur2NalogSetEnum.ULAZNI_MP:

            switch(faktur_rec.PdvKnjiga)
            {
               case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Pdv22m_Ura; break;
               case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Pdv22m_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_ROB: FtransKonto = KtoShemaDsc.Dsc_UKto_Pdv22m_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_USL: FtransKonto = KtoShemaDsc.Dsc_SKto_Pdv22m_Ura; break;
            }
            break;

         #endregion Faktur2NalogSetEnum.ULAZNI

         #region Faktur2NalogSetEnum.IZLAZNI_VP

         case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP:

            switch(faktur_rec.PdvKnjiga)
            {
               case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Pdv22_Ira; break;
               case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Pdv22_Ira; break;
            }
            break;

         #endregion Faktur2NalogSetEnum.IZLAZNI_VP

         #region Faktur2NalogSetEnum.IZLAZNI_MP

         case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP: FtransKonto = KtoShemaDsc.Dsc_KtoPdv22_Mir;
            break;

         #endregion Faktur2NalogSetEnum.IZLAZNI_MP

         #region Faktur2NalogSetEnum.BLAGAJNA

         #endregion Faktur2NalogSetEnum.BLAGAJNA
      }
   }
   internal void   SetKontoAndMoney_Pdv_22n  (Faktur faktur_rec)
   {
      SetDugAndPot(!TT_OtvaranjeDUG, faktur_rec.S_ukPdv22n);

      switch(Fak2nalSetLocal)
      {
         #region Faktur2NalogSetEnum.ULAZNI (MALOP & VELEP)

         case ZXC.Faktur2NalogSetEnum.ULAZNI_VP:
         case ZXC.Faktur2NalogSetEnum.ULAZNI_MP:

            switch(faktur_rec.PdvKnjiga)
            {
               case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Pdv22n_Ura; break;
               case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Pdv22n_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_ROB: FtransKonto = KtoShemaDsc.Dsc_UKto_Pdv22n_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_USL: FtransKonto = KtoShemaDsc.Dsc_SKto_Pdv22n_Ura; break;
            }
            break;

         #endregion Faktur2NalogSetEnum.ULAZNI

         #region Faktur2NalogSetEnum.BLAGAJNA

         #endregion Faktur2NalogSetEnum.BLAGAJNA
      }
   }
   internal void   SetKontoAndMoney_Pdv_10m  (Faktur faktur_rec, decimal money, ZXC.PdvGEOkindEnum geoKind, bool isUsluga, bool isKontra)
   {
      bool isOtvaranjePOT = !TT_OtvaranjeDUG;

      if(isKontra) isOtvaranjePOT = !isOtvaranjePOT;

      SetDugAndPot(isOtvaranjePOT, /*faktur_rec.S_ukPdv10m*/money);

      switch(Fak2nalSetLocal)
      {
         #region Faktur2NalogSetEnum.ULAZNI  (MALOP & VELEP)

         case ZXC.Faktur2NalogSetEnum.ULAZNI_VP:
         case ZXC.Faktur2NalogSetEnum.ULAZNI_MP:

            switch(faktur_rec.PdvKnjiga)
            {
               case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Pdv10m_Ura; break;
               case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Pdv10m_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_ROB: FtransKonto = KtoShemaDsc.Dsc_UKto_Pdv10m_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_USL: FtransKonto = KtoShemaDsc.Dsc_SKto_Pdv10m_Ura; break;
               
               case ZXC.PdvKnjigaEnum.NIJEDNA: // upaTODO: !!!!!! 

                  switch(faktur_rec.PdvGEOkind)
                  {
                     case ZXC.PdvGEOkindEnum.EU: FtransKonto = isUsluga ? (isKontra ? KtoShemaDsc.Dsc_ObrPdvU10_EU : KtoShemaDsc.Dsc_PdvU10m_EU): 
                                                                          (isKontra ? KtoShemaDsc.Dsc_ObrPdvR10_EU : KtoShemaDsc.Dsc_PdvR10m_EU); break;
                     case ZXC.PdvGEOkindEnum.BS:             FtransKonto = isKontra ? KtoShemaDsc.Dsc_ObrPdv10_BS  : KtoShemaDsc.Dsc_Pdv10m_BS  ; break;
                  }
                  break;
            }
            break;

         #endregion Faktur2NalogSetEnum.ULAZNI

         #region Faktur2NalogSetEnum.IZLAZNI_VP

         case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP:

            switch(faktur_rec.PdvKnjiga)
            {
               case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Pdv10_Ira; break;
               case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Pdv10_Ira; break;
            }
            break;

         #endregion Faktur2NalogSetEnum.IZLAZNI_VP

         #region Faktur2NalogSetEnum.IZLAZNI_MP

         case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP: FtransKonto = KtoShemaDsc.Dsc_KtoPdv10_Mir;
            break;

         #endregion Faktur2NalogSetEnum.IZLAZNI_MP

         #region Faktur2NalogSetEnum.BLAGAJNA

         #endregion Faktur2NalogSetEnum.BLAGAJNA
      }
   }
   internal void   SetKontoAndMoney_Pdv_10n  (Faktur faktur_rec, decimal money, ZXC.PdvGEOkindEnum geoKind, bool isUsluga, bool isKontra)
   {
      bool isOtvaranjePOT = !TT_OtvaranjeDUG;

      if(isKontra) isOtvaranjePOT = !isOtvaranjePOT;

      SetDugAndPot(isOtvaranjePOT, /*faktur_rec.S_ukPdv10n*/money);

      switch(Fak2nalSetLocal)
      {
         #region Faktur2NalogSetEnum.ULAZNI (MALOP & VELEP)

         case ZXC.Faktur2NalogSetEnum.ULAZNI_VP:
         case ZXC.Faktur2NalogSetEnum.ULAZNI_MP:

            switch(faktur_rec.PdvKnjiga)
            {
               case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Pdv10n_Ura; break;
               case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Pdv10n_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_ROB: FtransKonto = KtoShemaDsc.Dsc_UKto_Pdv10n_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_USL: FtransKonto = KtoShemaDsc.Dsc_SKto_Pdv10n_Ura; break;
               
               case ZXC.PdvKnjigaEnum.NIJEDNA: // upaTODO: !!!!!! 

                  switch(faktur_rec.PdvGEOkind)
                  {
                     case ZXC.PdvGEOkindEnum.EU: FtransKonto = isUsluga ? (isKontra ? KtoShemaDsc.Dsc_ObrPdvU10_EU : KtoShemaDsc.Dsc_PdvU10n_EU): 
                                                                          (isKontra ? KtoShemaDsc.Dsc_ObrPdvR10_EU : KtoShemaDsc.Dsc_PdvR10n_EU); break;
                     case ZXC.PdvGEOkindEnum.BS:             FtransKonto = isKontra ? KtoShemaDsc.Dsc_ObrPdv10_BS  : KtoShemaDsc.Dsc_Pdv10n_BS  ; break;
                  }
                  break;
            }
            break;

         #endregion Faktur2NalogSetEnum.ULAZNI

         #region Faktur2NalogSetEnum.BLAGAJNA

         #endregion Faktur2NalogSetEnum.BLAGAJNA
      }
   }
   internal void   SetKontoAndMoney_Pdv_05m  (Faktur faktur_rec, decimal money, ZXC.PdvGEOkindEnum geoKind, bool isUsluga, bool isKontra)
   {
      bool isOtvaranjePOT = !TT_OtvaranjeDUG;

      if(isKontra) isOtvaranjePOT = !isOtvaranjePOT;

      SetDugAndPot(isOtvaranjePOT, /*faktur_rec.S_ukPdv05m*/money);

      switch(Fak2nalSetLocal)
      {
         #region Faktur2NalogSetEnum.ULAZNI  (MALOP & VELEP)

         case ZXC.Faktur2NalogSetEnum.ULAZNI_VP:
         case ZXC.Faktur2NalogSetEnum.ULAZNI_MP:

            switch(faktur_rec.PdvKnjiga)
            {
               case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Pdv05m_Ura; break;
               case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Pdv05m_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_ROB: FtransKonto = KtoShemaDsc.Dsc_UKto_Pdv05m_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_USL: FtransKonto = KtoShemaDsc.Dsc_SKto_Pdv05m_Ura; break;
               
               case ZXC.PdvKnjigaEnum.NIJEDNA: // upaTODO: !!!!!! 

                  switch(faktur_rec.PdvGEOkind)
                  {
                     case ZXC.PdvGEOkindEnum.EU: FtransKonto = isUsluga ? (isKontra ? KtoShemaDsc.Dsc_ObrPdvU05_EU : KtoShemaDsc.Dsc_PdvU05m_EU) : 
                                                                          (isKontra ? KtoShemaDsc.Dsc_ObrPdvR05_EU : KtoShemaDsc.Dsc_PdvR05m_EU) ; break;
                  }
                  break;
            }
            break;

         #endregion Faktur2NalogSetEnum.ULAZNI

         #region Faktur2NalogSetEnum.IZLAZNI_VP

         case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP:

            switch(faktur_rec.PdvKnjiga)
            {
               case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Pdv05_Ira; break;
               case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Pdv05_Ira; break;
            }
            break;

         #endregion Faktur2NalogSetEnum.IZLAZNI_VP

         #region Faktur2NalogSetEnum.IZLAZNI_MP

         case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP: FtransKonto = KtoShemaDsc.Dsc_KtoPdv05_Mir;
            break;

         #endregion Faktur2NalogSetEnum.IZLAZNI_MP

         #region Faktur2NalogSetEnum.BLAGAJNA

         #endregion Faktur2NalogSetEnum.BLAGAJNA
      }
   }
   internal void   SetKontoAndMoney_Pdv_05n  (Faktur faktur_rec, decimal money, ZXC.PdvGEOkindEnum geoKind, bool isUsluga, bool isKontra)
   {
      bool isOtvaranjePOT = !TT_OtvaranjeDUG;

      if(isKontra) isOtvaranjePOT = !isOtvaranjePOT;

      SetDugAndPot(isOtvaranjePOT, /*faktur_rec.S_ukPdv05n*/money);

      switch(Fak2nalSetLocal)
      {
         #region Faktur2NalogSetEnum.ULAZNI (MALOP & VELEP)

         case ZXC.Faktur2NalogSetEnum.ULAZNI_VP:
         case ZXC.Faktur2NalogSetEnum.ULAZNI_MP:

            switch(faktur_rec.PdvKnjiga)
            {
               case ZXC.PdvKnjigaEnum.REDOVNA : FtransKonto = KtoShemaDsc.Dsc_RKto_Pdv05n_Ura; break;
               case ZXC.PdvKnjigaEnum.PREDUJAM: FtransKonto = KtoShemaDsc.Dsc_PKto_Pdv05n_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_ROB: FtransKonto = KtoShemaDsc.Dsc_UKto_Pdv05n_Ura; break;
               case ZXC.PdvKnjigaEnum.UVOZ_USL: FtransKonto = KtoShemaDsc.Dsc_SKto_Pdv05n_Ura; break;
               
               case ZXC.PdvKnjigaEnum.NIJEDNA: // upaTODO: !!!!!! 

                  switch(faktur_rec.PdvGEOkind)
                  {
                     case ZXC.PdvGEOkindEnum.EU: FtransKonto = isUsluga ? (isKontra ? KtoShemaDsc.Dsc_ObrPdvU05_EU : KtoShemaDsc.Dsc_PdvU05n_EU) :
                                                                          (isKontra ? KtoShemaDsc.Dsc_ObrPdvR05_EU : KtoShemaDsc.Dsc_PdvR05n_EU); break;
                  }
                  break;
            }
            break;

         #endregion Faktur2NalogSetEnum.ULAZNI

         #region Faktur2NalogSetEnum.BLAGAJNA

         #endregion Faktur2NalogSetEnum.BLAGAJNA
      }
   }

   // faktur_rec.Suk_KCR 
   internal string GetKonto_PrihTros         (Faktur faktur_rec, Rtrans rtrans_rec)
   {
      switch(Fak2nalSetLocal)
      {
         #region Faktur2NalogSetEnum.ULAZNI_VP (VELEP) --------------- 02.10.2013 dodan i ULAZNI_MP kada je na rulsima NEoznaceno 'Dsc_KnjiziMSK_ulaz'

         case ZXC.Faktur2NalogSetEnum.ULAZNI_VP:
         case ZXC.Faktur2NalogSetEnum.ULAZNI_MP:

            if(rtrans_rec.TheAsEx.IsKonto4Usluga == false && rtrans_rec.T_artiklCD.NotEmpty()) // klasicna roba 
            {
               return GetSkladKontoForSkladCD(faktur_rec.SkladCD);
            }
            else // usluga obicna i usluga za DP 
            {
               switch(faktur_rec.PdvKnjiga)
               {
                  case ZXC.PdvKnjigaEnum.REDOVNA : return KtoShemaDsc.Dsc_RKto_Osn_Ura;
                  case ZXC.PdvKnjigaEnum.PREDUJAM: return KtoShemaDsc.Dsc_PKto_Osn_Ura;
                  case ZXC.PdvKnjigaEnum.UVOZ_ROB: return KtoShemaDsc.Dsc_UKto_Osn_Ura;
                  case ZXC.PdvKnjigaEnum.UVOZ_USL: return KtoShemaDsc.Dsc_SKto_Osn_Ura;
                  
                  case ZXC.PdvKnjigaEnum.NIJEDNA : 
                     switch(faktur_rec.PdvGEOkind)
                     {
                        case ZXC.PdvGEOkindEnum.EU: return KtoShemaDsc.Dsc_TrosakR_EU;
                        case ZXC.PdvGEOkindEnum.BS: return KtoShemaDsc.Dsc_Trosak_BS;
                        case ZXC.PdvGEOkindEnum.TP: return KtoShemaDsc.Dsc_Trosak_TP;
                        default:                    return "";
                     }
                     
                  default: return "";
               }
            }

         #endregion Faktur2NalogSetEnum.ULAZNI

         #region Faktur2NalogSetEnum.ULAZNI_MP (MALOP)

         // 02.10.2013 (vidi case ZXC.Faktur2NalogSetEnum.ULAZNI_VP:) 
         // 
         //case ZXC.Faktur2NalogSetEnum.ULAZNI_MP:
         //   return "TODO: !!! [GetKonto_PrihTros()]";

         #endregion Faktur2NalogSetEnum.ULAZNI

         #region Faktur2NalogSetEnum.IZLAZNI_VP

         case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP:

            switch(faktur_rec.PdvKnjiga)
            {
               case ZXC.PdvKnjigaEnum.REDOVNA :
                  {
                          if(rtrans_rec.TheAsEx.IsRuc4Usluga     == true) return KtoShemaDsc.Dsc_RKto_Osn_Ira;
                     else if(rtrans_rec.TheAsEx.IsKonto4UslugaDP == true) return KtoShemaDsc.Dsc_KtoOsnIra_UslgDP;
                     else                                                 return GetPrihodKontoForSkladCD(faktur_rec.SkladCD, rtrans_rec.T_artiklCD, KtoShemaDsc.Dsc_RKto_Osn_Ira_Roba);
                  }               
               case ZXC.PdvKnjigaEnum.PREDUJAM:
                  {
                          if(rtrans_rec.TheAsEx.IsRuc4Usluga     == true) return KtoShemaDsc.Dsc_PKto_Osn_Ira;
                     else if(rtrans_rec.TheAsEx.IsKonto4UslugaDP == true) return KtoShemaDsc.Dsc_KtoOsnIra_UslgDP;
                     else                                                 return GetPrihodKontoForSkladCD(faktur_rec.SkladCD, rtrans_rec.T_artiklCD, KtoShemaDsc.Dsc_PKto_Osn_Ira_Roba);
                  }               
               
               default:                                                   return "";
            }
            
         #endregion Faktur2NalogSetEnum.IZLAZNI_VP

         #region Faktur2NalogSetEnum.IZLAZNI_MP

         case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP: return KtoShemaDsc.Dsc_KtoOsn_Mir;
            
         #endregion Faktur2NalogSetEnum.IZLAZNI_MP

         #region Faktur2NalogSetEnum.BLAGAJNA
         
         case ZXC.Faktur2NalogSetEnum.BLAGAJNA:

            if(faktur_rec.TT == Faktur.TT_UPL || faktur_rec.TT == Faktur.TT_BUP) return KtoShemaDsc.Dsc_Blg_Uplat;
            else                               return KtoShemaDsc.Dsc_Blg_Isplat;

         #endregion Faktur2NalogSetEnum.BLAGAJNA

         #region Faktur2NalogSetEnum.INTERNI_UI

         case ZXC.Faktur2NalogSetEnum.PROIZ_IZLAZ:

                 if(rtrans_rec.TheAsEx.IsMaterOrPotros == true) return KtoShemaDsc.Dsc_KtoMatTrsk;
            else if(rtrans_rec.TheAsEx.IsSitniInv      == true) return KtoShemaDsc.Dsc_KtoSInvTrsk;
            else                                                return KtoShemaDsc.Dsc_KtoMatTrsk; // ?! 

         #endregion Faktur2NalogSetEnum.INTERNI_UI

         // 24.03.2016.
         case ZXC.Faktur2NalogSetEnum.OneExactTT:
                 if(ThisTT_Only == Faktur.TT_PIP)  return KtoShemaDsc.Dsc_otp_ktoObrade;
                 else                              return "";

         default: return "";
      }
   }

   // faktur_rec.Suk_KCR 
   private string GetPrihodKontoForSkladCD(string skladCD, string artiklCD, string defaultPrihodKonto)
   {
      if(artiklCD.NotEmpty()) // klasicna roba 
      {
         // 22.02.2012 
         if(skladCD.IsEmpty()) return defaultPrihodKonto;

         string prihodKonto = GetSkladKontoForSkladCD(skladCD);

         // TODO: !!! vidi sta ako 'skladisni' konto nije samo 6600 nego npr. neka trica... 
       //if(defaultPrihodKonto.StartsWith("76")) return prihodKonto.Replace("66", "76"); 02.04.2014. korisnik sam odabire da li hoce ovakav nacin
         if(defaultPrihodKonto.StartsWith("76") && KtoShemaDsc.Dsc_OcuPrihLikeSklad == true) return prihodKonto.Replace("66", "76");
         else                                                                                return defaultPrihodKonto             ;
      }
      else return defaultPrihodKonto;
   }

   // Konto 7000: DUGUJE - Nabavna vrijednost prodane robe IRA_NV 
   /*private*/internal string  GetRealizacKontoForSkladCD(string skladCD, string defaultRealizacKonto)
   {
      // 22.02.2012 
      if(skladCD.IsEmpty()) return defaultRealizacKonto;

      string skladCdKonto = GetSkladKontoForSkladCD(skladCD);

      // TODO: !!! vidi sta ako 'skladisni' konto nije samo 6600 nego npr. neka trica... 
    //if(defaultPrihodKonto.StartsWith("76")) return prihodKonto.Replace("66", "76"); 02.04.2014. korisnik sam odabire da li hoce ovakav nacin
    //04.05.2016. samp privremeni ispravak da ne dojde glupost ako skladKto nepocinje sa 66
    //if(defaultRealizacKonto.StartsWith("70") && KtoShemaDsc.Dsc_OcuPrihLikeSklad == true) return skladCdKonto.Replace("66", "70");
      if(defaultRealizacKonto.StartsWith("70") && KtoShemaDsc.Dsc_OcuPrihLikeSklad == true && skladCdKonto.StartsWith("66")) return skladCdKonto.Replace("66", "70");
      else                                                                                                                   return defaultRealizacKonto;

      //04.05.2016. ovo je bio prijedlog ali to jos treba razraditi TODODO
      //string defaultRealizKtoRoot = defaultRealizacKonto.SubstringSafe(0, 2);
      //string defaultSkladKtoRoot  = skladCdKonto        .SubstringSafe(0, 2);
      //if(KtoShemaDsc.Dsc_OcuPrihLikeSklad == true) return skladCdKonto.Replace(defaultSkladKtoRoot, defaultRealizKtoRoot);
      //else                                         return defaultRealizacKonto;

   }

   // Konto 66xyz: POTRAZUJE (ali za IRU, za URU je DUG) - Nabavna vrijednost prodane robe IRA_NV                            
   // !!! VAZNO !!!: 16.09.2016. zakljucili da 'Dsc_Kto_Skladiste' sa rulsa labela 'Roba na sklad' ... se NE KORISTI NIGDJE! 
   // ... isti taj dan odlucili doticni 'Dsc_Kto_Skladiste' koristiti za overrajdanje pravila knjizi realizaciju ...         
   internal static string GetSkladKontoForSkladCD(string skladCD)
   {
      VvLookUpItem lui;

      if(skladCD.IsEmpty()) // UFA nastala iz PRI pri npr. uvozu 
      {
         lui = ZXC.luiListaSkladista.SingleOrDefault(l => l.Integer == 1); // probaj naci lui sa integerom '1' (integer 1 nam je kao intera sifra glvnog skladista) 
      }
      else
      {
         lui = ZXC.luiListaSkladista.SingleOrDefault(l => l.Cd == skladCD);
      }

      if(lui == null) { ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Ne znam skladiste {0} za skladKonto", skladCD); return ""; }
               
      return lui.Number.ToStringVv_NoDecimalNoGroup();
   }
   
   /*private*/public void SetNalogTT(string fakturTT)
   {
      switch(Fak2nalSet)
      {
         case ZXC.Faktur2NalogSetEnum.ULAZNI_VP     : NalogTT = "UR"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = Fak2nalSet;                               break;
         case ZXC.Faktur2NalogSetEnum.ULAZNI_MP     : NalogTT = "UM"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = Fak2nalSet;                               break;
         case ZXC.Faktur2NalogSetEnum.VMI           : NalogTT = "UM"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = Fak2nalSet;                               break;
         case ZXC.Faktur2NalogSetEnum.ULAZ_INT      : NalogTT = "TM"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = Fak2nalSet;                               break;
         case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP   : NalogTT = "IR"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = Fak2nalSet;                               break;
         case ZXC.Faktur2NalogSetEnum.IRA_RealizOnly: NalogTT = "IR"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = Fak2nalSet; ThisTT_Only = Faktur.TT_IRA;  break;
         case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP   : NalogTT = "IM"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = Fak2nalSet;                               break;
         case ZXC.Faktur2NalogSetEnum.PROIZ_IZLAZ   : NalogTT = "TM"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = Fak2nalSet;                               break;
         case ZXC.Faktur2NalogSetEnum.BLAGAJNA      :

            switch(fakturTT)
            {
               case Faktur.TT_UPL: NalogTT = "BL"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = Fak2nalSet; break;
               case Faktur.TT_BUP: NalogTT = "BL"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = Fak2nalSet; break;
               case Faktur.TT_ISP: NalogTT = "BL"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = Fak2nalSet; break;
               case Faktur.TT_BIS: NalogTT = "BL"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = Fak2nalSet; break;
            }
            break; 

         case ZXC.Faktur2NalogSetEnum.OneExactTT: 

            if(ThisTT_Only == Faktur.TT_UFA ||
               ThisTT_Only == Faktur.TT_URA ||
               ThisTT_Only == Faktur.TT_UOD ||
               ThisTT_Only == Faktur.TT_UPV)     { NalogTT = "UR"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.ULAZNI_VP; }

            else if(ThisTT_Only == Faktur.TT_UFM ||
                    ThisTT_Only == Faktur.TT_ZPC ||
                    ThisTT_Only == Faktur.TT_VMI ||
                    ThisTT_Only == Faktur.TT_TRI ||
                    ThisTT_Only == Faktur.TT_UPM ||
                    ThisTT_Only == Faktur.TT_URM) {NalogTT = "UM"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.ULAZNI_MP; }

            else if(ThisTT_Only == Faktur.TT_IFA ||
                    ThisTT_Only == Faktur.TT_IRA ||
                    ThisTT_Only == Faktur.TT_IOD ||
                    ThisTT_Only == Faktur.TT_IPV) {NalogTT = "IR"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP; }

            else if(ThisTT_Only == Faktur.TT_IZD) {NalogTT = "TM"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.IZLAZ_SK_VP; }
            else if(ThisTT_Only == Faktur.TT_IZM) {NalogTT = "TM"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.IZLAZ_SK_MP; }


            else if(ThisTT_Only == Faktur.TT_IRM) {NalogTT = "IM"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP; }

            else if(ThisTT_Only == Faktur.TT_UPL) {NalogTT = "BL"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.BLAGAJNA;    }
            else if(ThisTT_Only == Faktur.TT_BUP) {NalogTT = "BL"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.BLAGAJNA;    }
            else if(ThisTT_Only == Faktur.TT_ISP) {NalogTT = "BL"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.BLAGAJNA;    }
            else if(ThisTT_Only == Faktur.TT_BIS) {NalogTT = "BL"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.BLAGAJNA;    }

            else if(ThisTT_Only == Faktur.TT_IMT) {NalogTT = "TM"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.PROIZ_IZLAZ; }
            else if(ThisTT_Only == Faktur.TT_PPR) {NalogTT = "TM"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.PROIZ_IZLAZ; }
            else if(ThisTT_Only == Faktur.TT_POV) {NalogTT = "TM"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.PROIZ_IZLAZ; }
            else if(ThisTT_Only == Faktur.TT_PIZ) {NalogTT = "TM"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.PROIZ_IZLAZ; }
            else if(ThisTT_Only == Faktur.TT_PIX) {NalogTT = "TM"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.PROIZ_IZLAZ; }
            else if(ThisTT_Only == Faktur.TT_MSI) {NalogTT = "TM"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.PROIZ_IZLAZ; }
            else if(ThisTT_Only == Faktur.TT_VMI) {NalogTT = "TM"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.PROIZ_IZLAZ; }
            else if(ThisTT_Only == Faktur.TT_TRI) {NalogTT = "TM"; TT_OtvaranjeDUG = false; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.PROIZ_IZLAZ; }

            else if(ThisTT_Only == Faktur.TT_PUL) {NalogTT = "TM"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.ULAZ_INT;  }
            else if(ThisTT_Only == Faktur.TT_PUX) {NalogTT = "TM"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.ULAZ_INT;  }
            else if(ThisTT_Only == Faktur.TT_MSU) {NalogTT = "TM"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.ULAZ_INT;  }
            else if(ThisTT_Only == Faktur.TT_VMU) {NalogTT = "TM"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.ULAZ_INT;  }

            // 24.03.2016. dodan PIP
            else if(ThisTT_Only == Faktur.TT_PIP) { NalogTT = "TM"; TT_OtvaranjeDUG = true; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.OneExactTT; }

            else                                  {NalogTT = "TM"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.NONE;      } break;

         default                                 : NalogTT = "TM"; TT_OtvaranjeDUG = true ; Fak2nalSetLocal = ZXC.Faktur2NalogSetEnum.NONE;        break;

      } // switch(Fak2nalSet)

      //// !!! BIG NEWS !!! HUGE NEWS !!! start 
      bool nalogTToverrided = false;
      if(Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT  &&
         ThisTT_Only != Faktur.TT_UFA                      &&
         ThisTT_Only != Faktur.TT_URA                      &&
         ThisTT_Only != Faktur.TT_UOD                      &&
         ThisTT_Only != Faktur.TT_UPV                      &&
         ThisTT_Only != Faktur.TT_IFA                      &&
         ThisTT_Only != Faktur.TT_IRA                      &&
         ThisTT_Only != Faktur.TT_IOD                      &&
         ThisTT_Only != Faktur.TT_IRM                      &&
         ThisTT_Only != Faktur.TT_IPV                       )
      {
         NalogTT = ThisTT_Only;
         nalogTToverrided = true;
      }
      else if(ZXC.TtInfo(fakturTT).IsFinKol_TT &&
              fakturTT != Faktur.TT_UFA        &&
              fakturTT != Faktur.TT_URA        &&
              fakturTT != Faktur.TT_UOD        &&
              fakturTT != Faktur.TT_UPV        &&
              fakturTT != Faktur.TT_IRA        &&
              fakturTT != Faktur.TT_IFA        &&
              fakturTT != Faktur.TT_IOD        &&
              fakturTT != Faktur.TT_IPV        &&
              fakturTT != Faktur.TT_IRM         )
      {
         NalogTT = fakturTT;
         nalogTToverrided = true;
      }

      if(nalogTToverrided) CheckAndAddMissingNalogTT(NalogTT);

      //// !!! BIG NEWS !!! HUGE NEWS !!! end 

   }

   private void CheckAndAddMissingNalogTT(string nalogTT)
   {
      if(ZXC.luiListaNalogTT.GetLuiForThisCd(nalogTT) == null)
      {
         VvLookUpItem riskTTlui = ZXC.luiListaFakturType.GetLuiForThisCd(nalogTT);

         if(riskTTlui != null)
         {
            ZXC.luiListaNalogTT.Add(new VvLookUpItem(riskTTlui.Cd, riskTTlui.Name));

            VvDaoBase.SaveLookUpListToSqlTable(ZXC.luiListaNalogTT);
         }
      }
   }

   private void    SetNalogNapomena(string rasterID, int presumedNumOfNewNalog)
   {
      NalogNapomena  = "Raster ";
      NalogNapomena += (IsAutomatic ? "Autom. " : "");
      NalogNapomena += rasterID + "(filter: ";
      NalogNapomena += (PeriodDefinedVia_DokDate ? DateOd.Day.ToString() + "." + DateOd.Month.ToString() +  ".-" + DateDo.ToString(ZXC.VvDateFormat) : 
                                                   Fak2nalTimeRule.ToString());
      NalogNapomena += ") Nal " + NalogCount.ToString() + "/" + presumedNumOfNewNalog.ToString();

      NalogNapomena = LimitedStrNalog(NalogNapomena, ZXC.NalCI.napomena);
   }
   private void    SetFtransOpis(Faktur faktur_rec)
   {
      FtransOpis = "";

      FtransOpis += AddStringAndSeparator(faktur_rec.VezniDok.NotEmpty()  ? "or: " + faktur_rec.VezniDok : "");
      FtransOpis += AddStringAndSeparator(faktur_rec.PrimPlatCD.NotZero() ? faktur_rec.KupdobName : "");
      FtransOpis += AddStringAndSeparator(faktur_rec.V1_ttNum.NotZero() && faktur_rec.V1_tt != faktur_rec.TT ? "veza: " + faktur_rec.V1_tt + "-" + faktur_rec.V1_ttNum : ""); // znaci, NE zapisujemo V1tt i ttNum ako je u vezi istovrsni tti kao i faktur.tt 
      FtransOpis += AddStringAndSeparator(faktur_rec.V2_ttNum.NotZero() && faktur_rec.V2_tt != faktur_rec.TT ? "veza: " + faktur_rec.V2_tt + "-" + faktur_rec.V2_ttNum : "");
      FtransOpis += AddStringAndSeparator(faktur_rec.V3_ttNum.NotZero() && faktur_rec.V3_tt != faktur_rec.TT ? "veza: " + faktur_rec.V3_tt + "-" + faktur_rec.V3_ttNum : "");
      FtransOpis += AddStringAndSeparator(faktur_rec.V4_ttNum.NotZero() && faktur_rec.V4_tt != faktur_rec.TT ? "veza: " + faktur_rec.V4_tt + "-" + faktur_rec.V4_ttNum : "");

      FtransOpis = FtransOpis.TrimEnd('/');

      FtransOpis = LimitedStrFtrans(FtransOpis, ZXC.FtrCI.t_opis);

      if(FtransOpis.IsEmpty()) FtransOpis = faktur_rec.TT + " broj " + faktur_rec.TtNum;
   }

   internal string AddStringAndSeparator(string token)
   {
      if(token.IsEmpty()) return "";

      return token + "/";
   }
   internal string LimitedStrFtrans(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.FtransDao.GetSchemaColumnSize(cIdx));
   }
   internal string LimitedStrNalog (string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.NalogDao.GetSchemaColumnSize(cIdx));
   }

   internal void SetDugAndPot(bool isDug, decimal money)
   {
      if(isDug) { FtransDug = money; FtransPot = 0.00M; }
      else      { FtransDug = 0.00M; FtransPot = money; }
   }

   #endregion Explicit nalog or ftrans propertiz and metodz

}

public sealed class FaktExDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static FaktExDao instance;

   private FaktExDao(XSqlConnection conn, string dbName) : base(dbName, FaktEx.recordNameArhiva, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static FaktExDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new FaktExDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableFaktEx

   public static   uint TableVersionStatic { get { return 19; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition(bool isArhiva)
   {
      return (
         /* 00 */ "recID        int(10)      unsigned NOT NULL auto_increment      ,\n" +
         /* 01 */ "fakturRecID  int(10)      unsigned NOT NULL default '0'         ,\n" +
         /* 02 */ "pdvNum       int(10)      unsigned NOT NULL default '0'         ,\n" +
         /* 03 */ "pdvDate      date                  NOT NULL default '0001-01-01',\n" +
         /* 04 */ "kupdobCD     int(6)       unsigned NOT NULL default '0'         ,\n" +
         /* 05 */ "kupdobTK     char(6)               NOT NULL default ''          ,\n" +
         /* 06 */ "kupdobName   varchar(50)           NOT NULL default ''          ,\n" +
         /* 07 */ "kdUlica      varchar(32)           NOT NULL default ''          ,\n" +
         /* 08 */ "kdMjesto     varchar(32)           NOT NULL default ''          ,\n" +
         /* 09 */ "kdZip        varchar(16)           NOT NULL default ''          ,\n" +
         /* 10 */ "kdOib        varchar(12)           NOT NULL default ''          ,\n" +
         /* 11 */ "posJedCD     int(6)       unsigned NOT NULL default '0'         ,\n" +
         /* 12 */ "posJedTK     varchar(6)            NOT NULL default ''          ,\n" +
         /* 13 */ "posJedName   varchar(50)           NOT NULL default ''          ,\n" +
         /* 14 */ "posJedUlica  varchar(32)           NOT NULL default ''          ,\n" +
         /* 15 */ "posJedMjesto varchar(32)           NOT NULL default ''          ,\n" +
         /* 16 */ "posJedZip    varchar(11)           NOT NULL default ''          ,\n" +
         /* 17 */ "vezniDok2    varchar(40)           NOT NULL default ''          ,\n" +
         /* 18 */ "fco          varchar(32)           NOT NULL default ''          ,\n" +
         /* 19 */ "rokPlac      int(6)                NOT NULL default '0'         ,\n" +
         /* 20 */ "dospDate     date                  NOT NULL default '0001-01-01',\n" +
         /* 21 */ "skladDate    date                  NOT NULL default '0001-01-01',\n" +
         /* 22 */ "nacPlac      varchar(24)           NOT NULL default ''          ,\n" +
         /* 23 */ "ziroRn       varchar(24)           NOT NULL default ''          ,\n" +
         /* 24 */ "devName      char(3)               NOT NULL default ''          ,\n" +
         /* 25 */ "pnbM         varchar(24)           NOT NULL default ''          ,\n" +
         /* 26 */ "pnbV         varchar(24)           NOT NULL default ''          ,\n" +
         /* 27 */ "personCD     int(6)       unsigned NOT NULL default '0'         ,\n" +
         /* 28 */ "personName   varchar(32)           NOT NULL default ''          ,\n" +    
         /* 29 */ "napomena2    varchar(64)           NOT NULL default ''          ,\n" +
         /* 30 */ "cjenikTT     char(3)               NOT NULL default ''          ,\n" +
         /* 31 */ "statusCD     char(3)               NOT NULL default ''          ,\n" +
         /* 32 */ "rokPonude    int(6)                NOT NULL default '0'         ,\n" +
         /* 33 */ "ponudDate    date                  NOT NULL default '0001-01-01',\n" +
         /* 34 */ "mtrosCD      int(6)       unsigned NOT NULL default '0'         ,\n" +
         /* 35 */ "mtrosTK      char(6)               NOT NULL default ''          ,\n" +
         /* 36 */ "mtrosName    varchar(50)           NOT NULL default ''          ,\n" +
         /* 37 */ "primPlatCD   int(6)       unsigned NOT NULL default '0'         ,\n" +
         /* 38 */ "primPlatTK   char(6)               NOT NULL default ''          ,\n" +
         /* 39 */ "primPlatName varchar(50)           NOT NULL default ''          ,\n" +
         /* 40 */ "pdvKnjiga    tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
         /* 41 */ "isNpCash     tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
         /* 42 */ "pdvR12       tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
         /* 43 */ "pdvKolTip    tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
         /* 44 */ "s_ukKCRMP    decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 45 */ "s_ukKCRM     decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 46 */ "s_ukKCR      decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 47 */ "s_ukRbt1     decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 48 */ "s_ukRbt2     decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 49 */ "s_ukZavisni  decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 50 */ "s_ukProlazne decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 51 */ "s_ukPdv23m   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 52 */ "s_ukPdv23n   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 53 */ "s_ukPdv22m   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 54 */ "s_ukPdv22n   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 55 */ "s_ukPdv10m   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 56 */ "s_ukPdv10n   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 57 */ "s_ukOsn23m   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 58 */ "s_ukOsn23n   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 59 */ "s_ukOsn22m   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 50 */ "s_ukOsn22n   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 51 */ "s_ukOsn10m   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 52 */ "s_ukOsn10n   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 53 */ "s_ukOsn0     decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 64 */ "s_ukOsnPr    decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 65 */ "opciAlabel   varchar(32)           NOT NULL default ''          ,\n" +
         /* 66 */ "opciAvalue   varchar(32)           NOT NULL default ''          ,\n" +
         /* 67 */ "opciBlabel   varchar(32)           NOT NULL default ''          ,\n" +
         /* 68 */ "opciBvalue   varchar(32)           NOT NULL default ''          ,\n" +
         /* 69 */ "odgvPersCD   int(6)      unsigned  NOT NULL default '0'         ,\n" +
         /* 70 */ "odgvPersName varchar(32)           NOT NULL default ''          ,\n" +
         /* 71 */ "cjenTTNum    int(10)     unsigned  NOT NULL default '0'         ,\n" +
         /* 72 */ "v3_tt        char(3)               NOT NULL default ''          ,\n" +
         /* 73 */ "v3_ttNum     int(10)     unsigned  NOT NULL default '0'         ,\n" +
         /* 74 */ "v4_tt        char(3)               NOT NULL default ''          ,\n" +
         /* 75 */ "v4_ttNum     int(10)     unsigned  NOT NULL default '0'         ,\n" +
         /* 76 */ "s_ukMrz      decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 77 */ "s_ukPdv      decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 78 */ "tipOtpreme   varchar(32)           NOT NULL default ''          ,\n" +
         /* 79 */ "rokIsporuke  int(6)                NOT NULL default '0'         ,\n" +
         /* 80 */ "rokIspDate   date                  NOT NULL default '0001-01-01',\n" +
         /* 81 */ "dostName     varchar(50)           NOT NULL default ''          ,\n" +
         /* 82 */ "dostAddr     varchar(64)           NOT NULL default ''          ,\n" +
         /* 83 */ "s_ukOsn07    decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 84 */ "s_ukOsn08    decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 85 */ "s_ukOsn09    decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 86 */ "s_ukOsn10    decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 87 */ "s_ukOsn11    decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 88 */ "s_ukOsnUr23  decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 89 */ "s_ukOsnUu10  decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 90 */ "s_ukOsnUu22  decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 91 */ "s_ukOsnUu23  decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 92 */ "s_ukPdvUr23  decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 93 */ "s_ukPdvUu10  decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 94 */ "s_ukPdvUu22  decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 95 */ "s_ukPdvUu23  decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /* 96 */ "carinaKind   tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
         /* 97 */ "prjArtCD     varchar(32)           NOT NULL default ''          ,\n" +
         /* 98 */ "prjArtName   varchar(80)           NOT NULL default ''          ,\n" +
         /* 99 */ "externLink1  varchar(256)          NOT NULL default ''          ,\n" +
         /*100 */ "externLink2  varchar(256)          NOT NULL default ''          ,\n" +
         /*101 */ "someMoney    decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*102 */ "somePercent  decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*103 */ "s_ukMskPdv10 decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*104 */ "s_ukMskPdv23 decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*105 */ "s_ukMSK_00   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*106 */ "s_ukMSK_10   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*107 */ "s_ukMSK_23   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*108 */ "s_ukKCR_usl  decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*109 */ "s_ukKCRP_usl decimal(12,4)         NOT NULL default '0.00'      ,\n" +

         /*110 */ "s_ukPdv25m   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*111 */ "s_ukPdv25n   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*112 */ "s_ukOsn25m   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*113 */ "s_ukOsn25n   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*114 */ "s_ukOsnUr25  decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*115 */ "s_ukOsnUu25  decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*116 */ "s_ukPdvUr25  decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*117 */ "s_ukPdvUu25  decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*118 */ "s_ukMskPdv25 decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*119 */ "s_ukMSK_25   decimal(12,4)         NOT NULL default '0.00'      ,\n" +

         /*120 */ "fiskJIR      varchar(40)           NOT NULL default ''          ,\n" +
         /*121 */ "fiskZKI      varchar(40)           NOT NULL default ''          ,\n" +
         /*122 */ "fiskMsgID    varchar(40)           NOT NULL default ''          ,\n" +
         /*123 */ "fiskOibOp    varchar(11)           NOT NULL default ''          ,\n" +
         /*124 */ "fiskPrgBr    varchar(40)           NOT NULL default ''          ,\n" +

         /*125 */ "s_ukPdv05m   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*126 */ "s_ukPdv05n   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*127 */ "s_ukOsn05m   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*128 */ "s_ukOsn05n   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*129 */ "s_ukMskPdv05 decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*130 */ "s_ukMSK_05   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*131 */ "s_ukOsnUr05  decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*132 */ "s_ukPdvUr05  decimal(12,4)         NOT NULL default '0.00'      ,\n" +

         /*133 */ "s_pixK        decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*134 */ "s_puxK_P      decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*135 */ "s_puxK_All    decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*136 */ "s_pixKC       decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*137 */ "s_puxKC_P     decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*138 */ "s_puxKC_All   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*139 */ "s_ukPpmvOsn   decimal(12,4)         NOT NULL default '0.00'      ,\n" +
         /*140 */ "s_ukPpmvSt1i2 decimal( 5,2)         NOT NULL default '0.00'      ,\n" +
         /*141 */ "dateX         date                  NOT NULL default '0001-01-01',\n" +
         /*142 */ "vatCntryCode  char(2)               NOT NULL default ''          ,\n" +

         /*143 */ "pdvGEOkind    tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
         /*144 */ "pdvZPkind     tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
         /*145 */ "s_ukOsnR25m_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*146 */ "s_ukOsnR25n_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*147 */ "s_ukOsnU25m_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*148 */ "s_ukOsnU25n_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*149 */ "s_ukOsnR10m_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*150 */ "s_ukOsnR10n_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*151 */ "s_ukOsnU10m_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*152 */ "s_ukOsnU10n_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*153 */ "s_ukOsnR05m_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*154 */ "s_ukOsnR05n_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*155 */ "s_ukOsnU05m_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*156 */ "s_ukOsnU05n_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*157 */ "s_ukOsn25m_BS  decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*158 */ "s_ukOsn25n_BS  decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*159 */ "s_ukOsn10m_BS  decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*160 */ "s_ukOsn10n_BS  decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*161 */ "s_ukOsn25m_TP  decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*162 */ "s_ukOsn25n_TP  decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*163 */ "s_ukPdvR25m_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*164 */ "s_ukPdvR25n_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*165 */ "s_ukPdvU25m_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*166 */ "s_ukPdvU25n_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*167 */ "s_ukPdvR10m_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*168 */ "s_ukPdvR10n_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*169 */ "s_ukPdvU10m_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*170 */ "s_ukPdvU10n_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*171 */ "s_ukPdvR05m_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*172 */ "s_ukPdvR05n_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*173 */ "s_ukPdvU05m_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*174 */ "s_ukPdvU05n_EU decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*175 */ "s_ukPdv25m_BS  decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*176 */ "s_ukPdv25n_BS  decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*177 */ "s_ukPdv10m_BS  decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*178 */ "s_ukPdv10n_BS  decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*179 */ "s_ukPdv25m_TP  decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*180 */ "s_ukPdv25n_TP  decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*181 */ "s_ukOsnZP_11   decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*182 */ "s_ukOsnZP_12   decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*183 */ "s_ukOsnZP_13   decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*184 */ "s_ukOsn12      decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*185 */ "s_ukOsn13      decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*186 */ "s_ukOsn14      decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*187 */ "s_ukOsn15      decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*188 */ "s_ukOsn16      decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*189 */ "s_ukOsnPNP     decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*190 */ "s_ukIznPNP     decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*191 */ "s_ukMskPNP     decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*192 */ "skiz_ukKC      decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*193 */ "skiz_ukKCR     decimal(12,4)        NOT NULL default '0.00'      ,\n" +
         /*194 */ "skiz_ukRbt1    decimal(12,4)        NOT NULL default '0.00'      ,\n" +

                                              "PRIMARY KEY      (recID     ) ,\n" +
          (isArhiva ? "" : /*"UNIQUE "*/"") + "KEY BY_FakRecID  (fakturRecID),\n" +
          (isArhiva ? "" : /*"UNIQUE "*/"") + "KEY BY_KUPDOB    (kupdobName)  \n"
         );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = FaktEx.recordNameArhiva;
      else         tableName = FaktEx.recordName;

      switch(catchingVersion)
      {
         case 2: return ("ADD COLUMN tipOtpreme   varchar(32)           NOT NULL default ''           AFTER s_ukPdv    ,  " +
                         "ADD COLUMN rokIsporuke  int(6)                NOT NULL default '0'          AFTER tipOtpreme ,  " +
                         "ADD COLUMN rokIspDate   date                  NOT NULL default '0001-01-01' AFTER rokIsporuke;\n");

         case 3: return ("ADD COLUMN dostName     varchar(50)           NOT NULL default ''           AFTER rokIspDate    ,  " +
                         "ADD COLUMN dostAddr     varchar(64)           NOT NULL default ''           AFTER dostName      ;\n");

         case 4: return ("ADD INDEX BY_FakRecID USING BTREE(fakturRecID),\n" +
                         "ADD INDEX BY_KUPDOB   USING BTREE(kupdobName) ;\n");

         case 5: return ("ADD COLUMN s_ukOsn07     decimal(12,4)         NOT NULL default '0.00'          AFTER dostAddr    ,  " +
                         "ADD COLUMN s_ukOsn08     decimal(12,4)         NOT NULL default '0.00'          AFTER s_ukOsn07   ,  " +
                         "ADD COLUMN s_ukOsn09     decimal(12,4)         NOT NULL default '0.00'          AFTER s_ukOsn08   ,  " +
                         "ADD COLUMN s_ukOsn10     decimal(12,4)         NOT NULL default '0.00'          AFTER s_ukOsn09   ,  " +
                         "ADD COLUMN s_ukOsn11     decimal(12,4)         NOT NULL default '0.00'          AFTER s_ukOsn10   ,  " +
                         "ADD COLUMN s_ukOsnUr23   decimal(12,4)         NOT NULL default '0.00'          AFTER s_ukOsn11   ,  " +
                         "ADD COLUMN s_ukOsnUu10   decimal(12,4)         NOT NULL default '0.00'          AFTER s_ukOsnUr23 ,  " +
                         "ADD COLUMN s_ukOsnUu22   decimal(12,4)         NOT NULL default '0.00'          AFTER s_ukOsnUu10 ,  " +
                         "ADD COLUMN s_ukOsnUu23   decimal(12,4)         NOT NULL default '0.00'          AFTER s_ukOsnUu22 ,  " +
                         "ADD COLUMN s_ukPdvUr23   decimal(12,4)         NOT NULL default '0.00'          AFTER s_ukOsnUu23 ,  " +
                         "ADD COLUMN s_ukPdvUu10   decimal(12,4)         NOT NULL default '0.00'          AFTER s_ukPdvUr23 ,  " +
                         "ADD COLUMN s_ukPdvUu22   decimal(12,4)         NOT NULL default '0.00'          AFTER s_ukPdvUu10 ,  " +
                         "ADD COLUMN s_ukPdvUu23   decimal(12,4)         NOT NULL default '0.00'          AFTER s_ukPdvUu22 ;\n");

         //case 6: return ("DROP INDEX BY_FakRecID"); // jer si ovaj index zaboravio staviti i u create table
         //case 7: return ("ADD  INDEX BY_FakRecID USING BTREE(fakturRecID)");

         case 6: return ("ADD COLUMN carinaKind   tinyint(1)   unsigned NOT NULL default '0' AFTER s_ukPdvUu23    ;\n");

         case 7: return ("ADD COLUMN prjArtCD     varchar(32)           NOT NULL default '' AFTER carinaKind ,     " +
                         "ADD COLUMN prjArtName   varchar(80)           NOT NULL default '' AFTER prjArtCD   ,     " +
                         "ADD COLUMN externLink1  varchar(256)          NOT NULL default '' AFTER prjArtName ,     " +
                         "ADD COLUMN externLink2  varchar(256)          NOT NULL default '' AFTER externLink1,     " +
                         "ADD COLUMN someMoney    decimal(12,4)         NOT NULL default '0.00' AFTER externLink2, " +
                         "ADD COLUMN somePercent  decimal(12,4)         NOT NULL default '0.00' AFTER someMoney;   ");

         case 8: return ("CHANGE COLUMN pdvUvoz isNpCash TINYINT(1) UNSIGNED   NOT NULL DEFAULT 0,                          " +
                         "ADD    COLUMN s_ukMskPdv10     decimal(12,4)         NOT NULL default '0.00' AFTER somePercent ,  " +
                         "ADD    COLUMN s_ukMskPdv23     decimal(12,4)         NOT NULL default '0.00' AFTER s_ukMskPdv10,  " +
                         "ADD    COLUMN s_ukMSK_00       decimal(12,4)         NOT NULL default '0.00' AFTER s_ukMskPdv23,  " +
                         "ADD    COLUMN s_ukMSK_10       decimal(12,4)         NOT NULL default '0.00' AFTER s_ukMSK_00  ,  " +
                         "ADD    COLUMN s_ukMSK_23       decimal(12,4)         NOT NULL default '0.00' AFTER s_ukMSK_10  ,  " +
                         "ADD    COLUMN s_ukKCR_usl      decimal(12,4)         NOT NULL default '0.00' AFTER s_ukMSK_23  ,  " +
                         "ADD    COLUMN s_ukKCRP_usl     decimal(12,4)         NOT NULL default '0.00' AFTER s_ukKCR_usl ;  ");

         case 9: return ("ADD    COLUMN s_ukPdv25m       decimal(12,4)         NOT NULL default '0.00' AFTER s_ukKCRP_usl,  " +
                         "ADD    COLUMN s_ukPdv25n       decimal(12,4)         NOT NULL default '0.00' AFTER s_ukPdv25m  ,  " +
                         "ADD    COLUMN s_ukOsn25m       decimal(12,4)         NOT NULL default '0.00' AFTER s_ukPdv25n  ,  " +
                         "ADD    COLUMN s_ukOsn25n       decimal(12,4)         NOT NULL default '0.00' AFTER s_ukOsn25m  ,  " +
                         "ADD    COLUMN s_ukOsnUr25      decimal(12,4)         NOT NULL default '0.00' AFTER s_ukOsn25n  ,  " +
                         "ADD    COLUMN s_ukOsnUu25      decimal(12,4)         NOT NULL default '0.00' AFTER s_ukOsnUr25 ,  " +
                         "ADD    COLUMN s_ukPdvUr25      decimal(12,4)         NOT NULL default '0.00' AFTER s_ukOsnUu25 ,  " +
                         "ADD    COLUMN s_ukPdvUu25      decimal(12,4)         NOT NULL default '0.00' AFTER s_ukPdvUr25 ,  " +
                         "ADD    COLUMN s_ukMskPdv25     decimal(12,4)         NOT NULL default '0.00' AFTER s_ukPdvUu25 ,  " +
                         "ADD    COLUMN s_ukMSK_25       decimal(12,4)         NOT NULL default '0.00' AFTER s_ukMskPdv25;  ");

         case 10: return ("ADD COLUMN fiskJIR      varchar(40)           NOT NULL default '' AFTER s_ukMSK_25, " +
                          "ADD COLUMN fiskZKI      varchar(40)           NOT NULL default '' AFTER fiskJIR   , " +
                          "ADD COLUMN fiskMsgID    varchar(40)           NOT NULL default '' AFTER fiskZKI   , " +
                          "ADD COLUMN fiskOibOp    varchar(11)           NOT NULL default '' AFTER fiskMsgID , " +
                          "ADD COLUMN fiskPrgBr    varchar(40)           NOT NULL default '' AFTER fiskOibOp ; ");

        case 11: return ("ADD    COLUMN s_ukPdv05m       decimal(12,4)         NOT NULL default '0.00' AFTER fiskPrgBr   ,  " +
                         "ADD    COLUMN s_ukPdv05n       decimal(12,4)         NOT NULL default '0.00' AFTER s_ukPdv05m  ,  " +
                         "ADD    COLUMN s_ukOsn05m       decimal(12,4)         NOT NULL default '0.00' AFTER s_ukPdv05n  ,  " +
                         "ADD    COLUMN s_ukOsn05n       decimal(12,4)         NOT NULL default '0.00' AFTER s_ukOsn05m  ,  " +
                         "ADD    COLUMN s_ukMskPdv05     decimal(12,4)         NOT NULL default '0.00' AFTER s_ukOsn05n  ,  " +
                         "ADD    COLUMN s_ukMSK_05       decimal(12,4)         NOT NULL default '0.00' AFTER s_ukMskPdv05;  ");

        case 12: return ("ADD    COLUMN s_ukOsnUr05       decimal(12,4)         NOT NULL default '0.00' AFTER s_ukMSK_05 ,  " +
                         "ADD    COLUMN s_ukPdvUr05       decimal(12,4)         NOT NULL default '0.00' AFTER s_ukOsnUr05;  ");

        case 13: return ("ADD    COLUMN s_pixK            decimal(12,4)         NOT NULL default '0.00' AFTER s_ukPdvUr05 ,  " +
                         "ADD    COLUMN s_puxK_P          decimal(12,4)         NOT NULL default '0.00' AFTER s_pixK      ,  " +
                         "ADD    COLUMN s_puxK_All        decimal(12,4)         NOT NULL default '0.00' AFTER s_puxK_P    ,  " +
                         "ADD    COLUMN s_pixKC           decimal(12,4)         NOT NULL default '0.00' AFTER s_puxK_All  ,  " +
                         "ADD    COLUMN s_puxKC_P         decimal(12,4)         NOT NULL default '0.00' AFTER s_pixKC     ,  " +
                         "ADD    COLUMN s_puxKC_All       decimal(12,4)         NOT NULL default '0.00' AFTER s_puxKC_P   ;  ");

        case 14: return ("ADD    COLUMN s_ukPpmvOsn   decimal(12,4)         NOT NULL default '0.00' AFTER s_puxKC_All , " +
                         "ADD    COLUMN s_ukPpmvSt1i2 decimal( 5,2)         NOT NULL default '0.00' AFTER s_ukPpmvOsn;  ");

        case 15: return ("ADD    COLUMN dateX         date                  NOT NULL default '0001-01-01' AFTER s_ukPpmvSt1i2,  " +
                         "ADD    COLUMN vatCntryCode  char(2)               NOT NULL default ''           AFTER dateX        ,  " +
                         "MODIFY COLUMN kdOib         varchar(12)           NOT NULL default ''                              ;\n");

        case 16: return(
                        "ADD    COLUMN pdvGEOkind    tinyint(1)   unsigned NOT NULL default '0'    AFTER vatCntryCode  ,  " +
                        "ADD    COLUMN pdvZPkind     tinyint(1)   unsigned NOT NULL default '0'    AFTER pdvGEOkind    ,  " +
                        "ADD    COLUMN s_ukOsnR25m_EU decimal(12,4)        NOT NULL default '0.00' AFTER pdvZPkind     ,  " +
                        "ADD    COLUMN s_ukOsnR25n_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsnR25m_EU,  " +
                        "ADD    COLUMN s_ukOsnU25m_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsnR25n_EU,  " +
                        "ADD    COLUMN s_ukOsnU25n_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsnU25m_EU,  " +
                        "ADD    COLUMN s_ukOsnR10m_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsnU25n_EU,  " +
                        "ADD    COLUMN s_ukOsnR10n_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsnR10m_EU,  " +
                        "ADD    COLUMN s_ukOsnU10m_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsnR10n_EU,  " +
                        "ADD    COLUMN s_ukOsnU10n_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsnU10m_EU,  " +
                        "ADD    COLUMN s_ukOsnR05m_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsnU10n_EU,  " +
                        "ADD    COLUMN s_ukOsnR05n_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsnR05m_EU,  " +
                        "ADD    COLUMN s_ukOsnU05m_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsnR05n_EU,  " +
                        "ADD    COLUMN s_ukOsnU05n_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsnU05m_EU,  " +
                        "ADD    COLUMN s_ukOsn25m_BS  decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsnU05n_EU,  " +
                        "ADD    COLUMN s_ukOsn25n_BS  decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsn25m_BS ,  " +
                        "ADD    COLUMN s_ukOsn10m_BS  decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsn25n_BS ,  " +
                        "ADD    COLUMN s_ukOsn10n_BS  decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsn10m_BS ,  " +
                        "ADD    COLUMN s_ukOsn25m_TP  decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsn10n_BS ,  " +
                        "ADD    COLUMN s_ukOsn25n_TP  decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsn25m_TP ,  " +
                        "ADD    COLUMN s_ukPdvR25m_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsn25n_TP ,  " +
                        "ADD    COLUMN s_ukPdvR25n_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdvR25m_EU,  " +
                        "ADD    COLUMN s_ukPdvU25m_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdvR25n_EU,  " +
                        "ADD    COLUMN s_ukPdvU25n_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdvU25m_EU,  " +
                        "ADD    COLUMN s_ukPdvR10m_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdvU25n_EU,  " +
                        "ADD    COLUMN s_ukPdvR10n_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdvR10m_EU,  " +
                        "ADD    COLUMN s_ukPdvU10m_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdvR10n_EU,  " +
                        "ADD    COLUMN s_ukPdvU10n_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdvU10m_EU,  " +
                        "ADD    COLUMN s_ukPdvR05m_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdvU10n_EU,  " +
                        "ADD    COLUMN s_ukPdvR05n_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdvR05m_EU,  " +
                        "ADD    COLUMN s_ukPdvU05m_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdvR05n_EU,  " +
                        "ADD    COLUMN s_ukPdvU05n_EU decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdvU05m_EU,  " +
                        "ADD    COLUMN s_ukPdv25m_BS  decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdvU05n_EU,  " +
                        "ADD    COLUMN s_ukPdv25n_BS  decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdv25m_BS ,  " +
                        "ADD    COLUMN s_ukPdv10m_BS  decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdv25n_BS ,  " +
                        "ADD    COLUMN s_ukPdv10n_BS  decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdv10m_BS ,  " +
                        "ADD    COLUMN s_ukPdv25m_TP  decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdv10n_BS ,  " +
                        "ADD    COLUMN s_ukPdv25n_TP  decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdv25m_TP ,  " +
                        "ADD    COLUMN s_ukOsnZP_11   decimal(12,4)        NOT NULL default '0.00' AFTER s_ukPdv25n_TP ,  " +
                        "ADD    COLUMN s_ukOsnZP_12   decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsnZP_11  ,  " +
                        "ADD    COLUMN s_ukOsnZP_13   decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsnZP_12  ,  " +
                        "ADD    COLUMN s_ukOsn12      decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsnZP_13  ,  " +
                        "ADD    COLUMN s_ukOsn13      decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsn12     ,  " +
                        "ADD    COLUMN s_ukOsn14      decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsn13     ,  " +
                        "ADD    COLUMN s_ukOsn15      decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsn14     ,  " +
                        "ADD    COLUMN s_ukOsn16      decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsn15     ;  ");

        case 17: return ("ADD    COLUMN s_ukOsnPNP    decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsn16   ,  " +
                         "ADD    COLUMN s_ukIznPNP    decimal(12,4)        NOT NULL default '0.00' AFTER s_ukOsnPNP  ,  " +
                         "ADD    COLUMN s_ukMskPNP    decimal(12,4)        NOT NULL default '0.00' AFTER s_ukIznPNP  ;  ");

        case 18: return ("ADD    COLUMN skiz_ukKC     decimal(12,4)        NOT NULL default '0.00' AFTER s_ukMskPNP  ,  " +
                         "ADD    COLUMN skiz_ukKCR    decimal(12,4)        NOT NULL default '0.00' AFTER skiz_ukKC   ,  " +
                         "ADD    COLUMN skiz_ukRbt1   decimal(12,4)        NOT NULL default '0.00' AFTER skiz_ukKCR  ;  ");

        case 19: return ("MODIFY COLUMN vezniDok2    varchar(40)           NOT NULL default ''                       ;\n");

        default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableFaktEx

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      FaktEx faktEx = (FaktEx)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, faktEx.RecID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {

      /* 01 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.FakturRecID , TheSchemaTable.Rows[CI.fakturRecID ]);
      /* 02 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PdvNum      , TheSchemaTable.Rows[CI.pdvNum      ]);
      /* 03 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PdvDate     , TheSchemaTable.Rows[CI.pdvDate     ]);
      /* 04 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.KupdobCD    , TheSchemaTable.Rows[CI.kupdobCD    ]);
      /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.KupdobTK    , TheSchemaTable.Rows[CI.kupdobTK    ]);
      /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.KupdobName  , TheSchemaTable.Rows[CI.kupdobName  ]);
      /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.KdUlica     , TheSchemaTable.Rows[CI.kdUlica     ]);
      /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.KdMjesto    , TheSchemaTable.Rows[CI.kdMjesto    ]);
      /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.KdZip       , TheSchemaTable.Rows[CI.kdZip       ]);
      /* 10 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.KdOib       , TheSchemaTable.Rows[CI.kdOib       ]);
      /* 11 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PosJedCD    , TheSchemaTable.Rows[CI.posJedCD    ]);
      /* 12 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PosJedTK    , TheSchemaTable.Rows[CI.posJedTK    ]);
      /* 13 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PosJedName  , TheSchemaTable.Rows[CI.posJedName  ]);
      /* 14 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PosJedUlica , TheSchemaTable.Rows[CI.posJedUlica ]);
      /* 15 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PosJedMjesto, TheSchemaTable.Rows[CI.posJedMjesto]);
      /* 16 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PosJedZip   , TheSchemaTable.Rows[CI.posJedZip   ]);
      /* 17 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.VezniDok2   , TheSchemaTable.Rows[CI.vezniDok2   ]);
      /* 18 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.Fco         , TheSchemaTable.Rows[CI.fco         ]);
      /* 19 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.RokPlac     , TheSchemaTable.Rows[CI.rokPlac     ]);
      /* 20 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.DospDate    , TheSchemaTable.Rows[CI.dospDate    ]);
      /* 21 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.SkladDate   , TheSchemaTable.Rows[CI.skladDate   ]);
      /* 22 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.NacPlac     , TheSchemaTable.Rows[CI.nacPlac     ]);
      /* 23 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.ZiroRn      , TheSchemaTable.Rows[CI.ziroRn      ]);
      /* 24 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.DevName     , TheSchemaTable.Rows[CI.devName     ]);
      /* 25 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PnbM        , TheSchemaTable.Rows[CI.pnbM        ]);
      /* 26 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PnbV        , TheSchemaTable.Rows[CI.pnbV        ]);
      /* 27 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PersonCD    , TheSchemaTable.Rows[CI.personCD    ]);
      /* 28 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PersonName  , TheSchemaTable.Rows[CI.personName  ]);
      /* 29 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.Napomena2   , TheSchemaTable.Rows[CI.napomena2   ]);
      /* 30 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.CjenikTT    , TheSchemaTable.Rows[CI.cjenikTT    ]);
      /* 31 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.StatusCD    , TheSchemaTable.Rows[CI.statusCD    ]);
      /* 32 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.RokPonude   , TheSchemaTable.Rows[CI.rokPonude   ]);
      /* 33 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PonudDate   , TheSchemaTable.Rows[CI.ponudDate   ]);
      /* 34 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.MtrosCD     , TheSchemaTable.Rows[CI.mtrosCD     ]);
      /* 35 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.MtrosTK     , TheSchemaTable.Rows[CI.mtrosTK     ]);
      /* 36 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.MtrosName   , TheSchemaTable.Rows[CI.mtrosName   ]);
      /* 37 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PrimPlatCD  , TheSchemaTable.Rows[CI.primPlatCD  ]);
      /* 38 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PrimPlatTK  , TheSchemaTable.Rows[CI.primPlatTK  ]);
      /* 39 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PrimPlatName, TheSchemaTable.Rows[CI.primPlatName]);
      /* 40 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PdvKnjiga   , TheSchemaTable.Rows[CI.pdvKnjiga   ]);
      /* 41 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.IsNpCash    , TheSchemaTable.Rows[CI.isNpCash    ]);
      /* 42 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PdvR12      , TheSchemaTable.Rows[CI.pdvR12      ]);
      /* 43 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PdvKolTip   , TheSchemaTable.Rows[CI.pdvKolTip   ]);
      /* 44 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukKCRP    , TheSchemaTable.Rows[CI.s_ukKCRP    ]);
      /* 45 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukKCRM    , TheSchemaTable.Rows[CI.s_ukKCRM    ]);
      /* 46 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukKCR     , TheSchemaTable.Rows[CI.s_ukKCR     ]);
      /* 47 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukRbt1    , TheSchemaTable.Rows[CI.s_ukRbt1    ]);
      /* 48 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukRbt2    , TheSchemaTable.Rows[CI.s_ukRbt2    ]);
      /* 49 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukZavisni , TheSchemaTable.Rows[CI.s_ukZavisni ]);
      /* 50 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukProlazne, TheSchemaTable.Rows[CI.s_ukProlazne]);
      /* 51 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv23m  , TheSchemaTable.Rows[CI.s_ukPdv23m  ]);
      /* 52 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv23n  , TheSchemaTable.Rows[CI.s_ukPdv23n  ]);
      /* 53 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv22m  , TheSchemaTable.Rows[CI.s_ukPdv22m  ]);
      /* 54 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv22n  , TheSchemaTable.Rows[CI.s_ukPdv22n  ]);
      /* 55 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv10m  , TheSchemaTable.Rows[CI.s_ukPdv10m  ]);
      /* 56 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv10n  , TheSchemaTable.Rows[CI.s_ukPdv10n  ]);
      /* 57 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn23m  , TheSchemaTable.Rows[CI.s_ukOsn23m  ]);
      /* 58 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn23n  , TheSchemaTable.Rows[CI.s_ukOsn23n  ]);
      /* 59 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn22m  , TheSchemaTable.Rows[CI.s_ukOsn22m  ]);
      /* 50 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn22n  , TheSchemaTable.Rows[CI.s_ukOsn22n  ]);
      /* 51 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn10m  , TheSchemaTable.Rows[CI.s_ukOsn10m  ]);
      /* 62 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn10n  , TheSchemaTable.Rows[CI.s_ukOsn10n  ]);
      /* 63 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn0    , TheSchemaTable.Rows[CI.s_ukOsn0    ]);
      /* 64 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnPr   , TheSchemaTable.Rows[CI.s_ukOsnPr   ]);
      /* 65 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.OpciAlabel  , TheSchemaTable.Rows[CI.opciAlabel  ]);
      /* 66 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.OpciAvalue  , TheSchemaTable.Rows[CI.opciAvalue  ]);
      /* 67 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.OpciBlabel  , TheSchemaTable.Rows[CI.opciBlabel  ]);
      /* 68 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.OpciBvalue  , TheSchemaTable.Rows[CI.opciBvalue  ]);
      /* 69 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.OdgvPersCD  , TheSchemaTable.Rows[CI.odgvPersCD  ]);
      /* 70 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.OdgvPersName, TheSchemaTable.Rows[CI.odgvPersName]);
      /* 71 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.CjenTTnum   , TheSchemaTable.Rows[CI.cjenTTnum   ]);
      /* 72 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.V3_tt       , TheSchemaTable.Rows[CI.v3_tt       ]);
      /* 73 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.V3_ttNum    , TheSchemaTable.Rows[CI.v3_ttNum    ]);
      /* 74 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.V4_tt       , TheSchemaTable.Rows[CI.v4_tt       ]);
      /* 75 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.V4_ttNum    , TheSchemaTable.Rows[CI.v4_ttNum    ]);
      /* 76 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukMrz     , TheSchemaTable.Rows[CI.s_ukMrz     ]);
      /* 77 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv     , TheSchemaTable.Rows[CI.s_ukPdv     ]);
      /* 78 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.TipOtpreme  , TheSchemaTable.Rows[CI.tipOtpreme  ]);
      /* 79 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.RokIsporuke , TheSchemaTable.Rows[CI.rokIsporuke ]);
      /* 80 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.RokIspDate  , TheSchemaTable.Rows[CI.rokIspDate  ]);
      /* 81 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.DostName    , TheSchemaTable.Rows[CI.dostName    ]);
      /* 82 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.DostAddr    , TheSchemaTable.Rows[CI.dostAddr    ]);
      /* 83 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn07   , TheSchemaTable.Rows[CI.s_ukOsn07   ]);
      /* 84 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn08   , TheSchemaTable.Rows[CI.s_ukOsn08   ]);
      /* 85 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn09   , TheSchemaTable.Rows[CI.s_ukOsn09   ]);
      /* 86 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn10   , TheSchemaTable.Rows[CI.s_ukOsn10   ]);
      /* 87 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn11   , TheSchemaTable.Rows[CI.s_ukOsn11   ]);
      /* 88 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnUr23 , TheSchemaTable.Rows[CI.s_ukOsnUr23 ]);
      /* 89 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnUu10 , TheSchemaTable.Rows[CI.s_ukOsnUu10 ]);
      /* 90 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnUu22 , TheSchemaTable.Rows[CI.s_ukOsnUu22 ]);
      /* 91 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnUu23 , TheSchemaTable.Rows[CI.s_ukOsnUu23 ]);
      /* 92 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvUr23 , TheSchemaTable.Rows[CI.s_ukPdvUr23 ]);
      /* 93 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvUu10 , TheSchemaTable.Rows[CI.s_ukPdvUu10 ]);
      /* 94 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvUu22 , TheSchemaTable.Rows[CI.s_ukPdvUu22 ]);
      /* 95 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvUu23 , TheSchemaTable.Rows[CI.s_ukPdvUu23 ]);
      /* 96 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.CarinaKind_u, TheSchemaTable.Rows[CI.carinaKind  ]);
      /* 97 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PrjArtCD    , TheSchemaTable.Rows[CI.prjArtCD    ]);
      /* 98 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PrjArtName  , TheSchemaTable.Rows[CI.prjArtName  ]);
      /* 99 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.ExternLink1 , TheSchemaTable.Rows[CI.externLink1 ]);
      /*100 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.ExternLink2 , TheSchemaTable.Rows[CI.externLink2 ]);
      /*101 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.SomeMoney   , TheSchemaTable.Rows[CI.someMoney   ]);
      /*102 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.SomePercent , TheSchemaTable.Rows[CI.somePercent ]);
      /*103 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukMskPdv10, TheSchemaTable.Rows[CI.s_ukMskPdv10]);
      /*104 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukMskPdv23, TheSchemaTable.Rows[CI.s_ukMskPdv23]);
      /*105 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukMSK_00  , TheSchemaTable.Rows[CI.s_ukMSK_00  ]);
      /*106 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukMSK_10  , TheSchemaTable.Rows[CI.s_ukMSK_10  ]);
      /*107 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukMSK_23  , TheSchemaTable.Rows[CI.s_ukMSK_23  ]);
      /*108 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukKCR_usl , TheSchemaTable.Rows[CI.s_ukKCR_usl ]);
      /*109 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukKCRP_usl, TheSchemaTable.Rows[CI.s_ukKCRP_usl]);

      /*110 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv25m  , TheSchemaTable.Rows[CI.s_ukPdv25m  ]);
      /*111 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv25n  , TheSchemaTable.Rows[CI.s_ukPdv25n  ]);
      /*112 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn25m  , TheSchemaTable.Rows[CI.s_ukOsn25m  ]);
      /*113 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn25n  , TheSchemaTable.Rows[CI.s_ukOsn25n  ]);
      /*114 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnUr25 , TheSchemaTable.Rows[CI.s_ukOsnUr25 ]);
      /*115 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnUu25 , TheSchemaTable.Rows[CI.s_ukOsnUu25 ]);
      /*116 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvUr25 , TheSchemaTable.Rows[CI.s_ukPdvUr25 ]);
      /*117 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvUu25 , TheSchemaTable.Rows[CI.s_ukPdvUu25 ]);
      /*118 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukMskPdv25, TheSchemaTable.Rows[CI.s_ukMskPdv25]);
      /*119 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukMSK_25  , TheSchemaTable.Rows[CI.s_ukMSK_25  ]);

      /*120 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.FiskJIR     , TheSchemaTable.Rows[CI.fiskJIR     ]);
      /*121 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.FiskZKI     , TheSchemaTable.Rows[CI.fiskZKI     ]);
      /*122 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.FiskMsgID   , TheSchemaTable.Rows[CI.fiskMsgID   ]);
      /*123 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.FiskOibOp   , TheSchemaTable.Rows[CI.fiskOibOp   ]);
      /*124 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.FiskPrgBr   , TheSchemaTable.Rows[CI.fiskPrgBr   ]);

      /*125 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv05m  , TheSchemaTable.Rows[CI.s_ukPdv05m  ]);
      /*126 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv05n  , TheSchemaTable.Rows[CI.s_ukPdv05n  ]);
      /*127 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn05m  , TheSchemaTable.Rows[CI.s_ukOsn05m  ]);
      /*128 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn05n  , TheSchemaTable.Rows[CI.s_ukOsn05n  ]);
      /*129 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukMskPdv05, TheSchemaTable.Rows[CI.s_ukMskPdv05]);
      /*130 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukMSK_05  , TheSchemaTable.Rows[CI.s_ukMSK_05  ]);
      /*131 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnUr05 , TheSchemaTable.Rows[CI.s_ukOsnUr05 ]);
      /*132 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvUr05 , TheSchemaTable.Rows[CI.s_ukPdvUr05 ]);

      /*133 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_pixK      , TheSchemaTable.Rows[CI.s_pixK      ]);
      /*134 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_puxK_P    , TheSchemaTable.Rows[CI.s_puxK_P    ]);
      /*135 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_puxK_All  , TheSchemaTable.Rows[CI.s_puxK_All  ]);
      /*136 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_pixKC     , TheSchemaTable.Rows[CI.s_pixKC     ]);
      /*137 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_puxKC_P   , TheSchemaTable.Rows[CI.s_puxKC_P   ]);
      /*138 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_puxKC_All , TheSchemaTable.Rows[CI.s_puxKC_All ]);
      /*139 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPpmvOsn , TheSchemaTable.Rows[CI.s_ukPpmvOsn  ]);
      /*140 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPpmvSt1i2,TheSchemaTable.Rows[CI.s_ukPpmvSt1i2]);
      /*141 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.DateX        ,TheSchemaTable.Rows[CI.dateX        ]);
      /*142 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.VatCntryCode ,TheSchemaTable.Rows[CI.vatCntryCode ]);

      /*143 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PdvGEOkind    , TheSchemaTable.Rows[CI.pdvGEOkind    ]);
      /*144 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.PdvZPkind     , TheSchemaTable.Rows[CI.pdvZPkind     ]);
      /*145 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnR25m_EU, TheSchemaTable.Rows[CI.s_ukOsnR25m_EU]);
      /*146 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnR25n_EU, TheSchemaTable.Rows[CI.s_ukOsnR25n_EU]);
      /*147 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnU25m_EU, TheSchemaTable.Rows[CI.s_ukOsnU25m_EU]);
      /*148 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnU25n_EU, TheSchemaTable.Rows[CI.s_ukOsnU25n_EU]);
      /*149 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnR10m_EU, TheSchemaTable.Rows[CI.s_ukOsnR10m_EU]);
      /*150 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnR10n_EU, TheSchemaTable.Rows[CI.s_ukOsnR10n_EU]);
      /*151 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnU10m_EU, TheSchemaTable.Rows[CI.s_ukOsnU10m_EU]);
      /*152 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnU10n_EU, TheSchemaTable.Rows[CI.s_ukOsnU10n_EU]);
      /*153 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnR05m_EU, TheSchemaTable.Rows[CI.s_ukOsnR05m_EU]);
      /*154 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnR05n_EU, TheSchemaTable.Rows[CI.s_ukOsnR05n_EU]);
      /*155 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnU05m_EU, TheSchemaTable.Rows[CI.s_ukOsnU05m_EU]);
      /*156 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnU05n_EU, TheSchemaTable.Rows[CI.s_ukOsnU05n_EU]);
      /*157 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn25m_BS , TheSchemaTable.Rows[CI.s_ukOsn25m_BS ]);
      /*158 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn25n_BS , TheSchemaTable.Rows[CI.s_ukOsn25n_BS ]);
      /*159 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn10m_BS , TheSchemaTable.Rows[CI.s_ukOsn10m_BS ]);
      /*160 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn10n_BS , TheSchemaTable.Rows[CI.s_ukOsn10n_BS ]);
      /*161 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn25m_TP , TheSchemaTable.Rows[CI.s_ukOsn25m_TP ]);
      /*162 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn25n_TP , TheSchemaTable.Rows[CI.s_ukOsn25n_TP ]);
      /*163 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvR25m_EU, TheSchemaTable.Rows[CI.s_ukPdvR25m_EU]);
      /*164 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvR25n_EU, TheSchemaTable.Rows[CI.s_ukPdvR25n_EU]);
      /*165 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvU25m_EU, TheSchemaTable.Rows[CI.s_ukPdvU25m_EU]);
      /*166 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvU25n_EU, TheSchemaTable.Rows[CI.s_ukPdvU25n_EU]);
      /*167 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvR10m_EU, TheSchemaTable.Rows[CI.s_ukPdvR10m_EU]);
      /*168 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvR10n_EU, TheSchemaTable.Rows[CI.s_ukPdvR10n_EU]);
      /*169 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvU10m_EU, TheSchemaTable.Rows[CI.s_ukPdvU10m_EU]);
      /*170 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvU10n_EU, TheSchemaTable.Rows[CI.s_ukPdvU10n_EU]);
      /*171 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvR05m_EU, TheSchemaTable.Rows[CI.s_ukPdvR05m_EU]);
      /*172 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvR05n_EU, TheSchemaTable.Rows[CI.s_ukPdvR05n_EU]);
      /*173 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvU05m_EU, TheSchemaTable.Rows[CI.s_ukPdvU05m_EU]);
      /*174 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdvU05n_EU, TheSchemaTable.Rows[CI.s_ukPdvU05n_EU]);
      /*175 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv25m_BS , TheSchemaTable.Rows[CI.s_ukPdv25m_BS ]);
      /*176 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv25n_BS , TheSchemaTable.Rows[CI.s_ukPdv25n_BS ]);
      /*177 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv10m_BS , TheSchemaTable.Rows[CI.s_ukPdv10m_BS ]);
      /*178 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv10n_BS , TheSchemaTable.Rows[CI.s_ukPdv10n_BS ]);
      /*179 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv25m_TP , TheSchemaTable.Rows[CI.s_ukPdv25m_TP ]);
      /*180 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukPdv25n_TP , TheSchemaTable.Rows[CI.s_ukPdv25n_TP ]);
      /*181 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnZP_11  , TheSchemaTable.Rows[CI.s_ukOsnZP_11  ]);
      /*182 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnZP_12  , TheSchemaTable.Rows[CI.s_ukOsnZP_12  ]);
      /*183 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnZP_13  , TheSchemaTable.Rows[CI.s_ukOsnZP_13  ]);
      /*184 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn12     , TheSchemaTable.Rows[CI.s_ukOsn12     ]);
      /*185 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn13     , TheSchemaTable.Rows[CI.s_ukOsn13     ]);
      /*186 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn14     , TheSchemaTable.Rows[CI.s_ukOsn14     ]);
      /*187 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn15     , TheSchemaTable.Rows[CI.s_ukOsn15     ]);
      /*188 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsn16     , TheSchemaTable.Rows[CI.s_ukOsn16     ]);
      /*189 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukOsnPNP    , TheSchemaTable.Rows[CI.s_ukOsnPNP    ]);
      /*190 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukIznPNP    , TheSchemaTable.Rows[CI.s_ukIznPNP    ]);
      /*191 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.S_ukMskPNP    , TheSchemaTable.Rows[CI.s_ukMskPNP    ]);
      /*192 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.Skiz_ukKC     , TheSchemaTable.Rows[CI.skiz_ukKC     ]);
      /*193 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.Skiz_ukKCR    , TheSchemaTable.Rows[CI.skiz_ukKCR    ]);
      /*194 */ VvSQL.CreateCommandParameter(cmd, preffix, faktEx.Skiz_ukRbt1   , TheSchemaTable.Rows[CI.skiz_ukRbt1   ]);
      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva)
   {
      FaktExStruct rdrData = new FaktExStruct();

      // FaktEx additons
      int ciOffset;
      if(vvDataRecord.IsFillingFromJoinReader) ciOffset = FakturDao.lastFakturCI + 1;
      else                                     ciOffset = 0;

               rdrData._recID       = reader.GetUInt32   (CI.recID       + ciOffset);

      /* 01 */ rdrData._fakturRecID = reader.GetUInt32   (CI.fakturRecID + ciOffset);
      /* 02 */ rdrData._pdvNum      = reader.GetUInt32   (CI.pdvNum      + ciOffset);
      /* 03 */ rdrData._pdvDate     = reader.GetDateTime (CI.pdvDate     + ciOffset);
      /* 04 */ rdrData._kupdobCD    = reader.GetUInt32   (CI.kupdobCD    + ciOffset);
      /* 05 */ rdrData._kupdobTK    = reader.GetString   (CI.kupdobTK    + ciOffset);
      /* 06 */ rdrData._kupdobName  = reader.GetString   (CI.kupdobName  + ciOffset);
      /* 07 */ rdrData._kdUlica     = reader.GetString   (CI.kdUlica     + ciOffset);
      /* 08 */ rdrData._kdMjesto    = reader.GetString   (CI.kdMjesto    + ciOffset);
      /* 09 */ rdrData._kdZip       = reader.GetString   (CI.kdZip       + ciOffset);
      /* 10 */ rdrData._kdOib       = reader.GetString   (CI.kdOib       + ciOffset);
      /* 11 */ rdrData._posJedCD    = reader.GetUInt32   (CI.posJedCD    + ciOffset);
      /* 12 */ rdrData._posJedTK    = reader.GetString   (CI.posJedTK    + ciOffset);
      /* 13 */ rdrData._posJedName  = reader.GetString   (CI.posJedName  + ciOffset);
      /* 14 */ rdrData._posJedUlica = reader.GetString   (CI.posJedUlica + ciOffset);
      /* 15 */ rdrData._posJedMjesto= reader.GetString   (CI.posJedMjesto+ ciOffset);
      /* 16 */ rdrData._posJedZip   = reader.GetString   (CI.posJedZip   + ciOffset);
      /* 17 */ rdrData._vezniDok2   = reader.GetString   (CI.vezniDok2   + ciOffset);
      /* 18 */ rdrData._fco         = reader.GetString   (CI.fco         + ciOffset);
      /* 19 */ rdrData._rokPlac     = reader.GetInt32    (CI.rokPlac     + ciOffset);
      /* 20 */ rdrData._dospDate    = reader.GetDateTime (CI.dospDate    + ciOffset);
      /* 21 */ rdrData._skladDate   = reader.GetDateTime (CI.skladDate   + ciOffset);
      /* 22 */ rdrData._nacPlac     = reader.GetString   (CI.nacPlac     + ciOffset);
      /* 23 */ rdrData._ziroRn      = reader.GetString   (CI.ziroRn      + ciOffset);
      /* 24 */ rdrData._devName     = reader.GetString   (CI.devName     + ciOffset);
      /* 25 */ rdrData._pnbM        = reader.GetString   (CI.pnbM        + ciOffset);
      /* 26 */ rdrData._pnbV        = reader.GetString   (CI.pnbV        + ciOffset);
      /* 27 */ rdrData._personCD    = reader.GetUInt32   (CI.personCD    + ciOffset);
      /* 28 */ rdrData._personName  = reader.GetString   (CI.personName  + ciOffset);
      /* 29 */ rdrData._napomena2   = reader.GetString   (CI.napomena2   + ciOffset);
      /* 30 */ rdrData._cjenikTT    = reader.GetString   (CI.cjenikTT    + ciOffset);
      /* 31 */ rdrData._statusCD    = reader.GetString   (CI.statusCD    + ciOffset);
      /* 32 */ rdrData._rokPonude   = reader.GetInt32    (CI.rokPonude   + ciOffset);
      /* 33 */ rdrData._ponudDate   = reader.GetDateTime (CI.ponudDate   + ciOffset);
      /* 34 */ rdrData._mtrosCD     = reader.GetUInt32   (CI.mtrosCD     + ciOffset);
      /* 35 */ rdrData._mtrosTK     = reader.GetString   (CI.mtrosTK     + ciOffset);
      /* 36 */ rdrData._mtrosName   = reader.GetString   (CI.mtrosName   + ciOffset);
      /* 37 */ rdrData._primPlatCD  = reader.GetUInt32   (CI.primPlatCD  + ciOffset);
      /* 38 */ rdrData._primPlatTK  = reader.GetString   (CI.primPlatTK  + ciOffset);
      /* 39 */ rdrData._primPlatName= reader.GetString   (CI.primPlatName+ ciOffset);
      /* 40 */ rdrData._pdvKnjiga   = reader.GetUInt16   (CI.pdvKnjiga   + ciOffset);
      /* 41 */ rdrData._isNpCash    = reader.GetBoolean  (CI.isNpCash    + ciOffset);
      /* 42 */ rdrData._pdvR12      = reader.GetUInt16   (CI.pdvR12      + ciOffset);
      /* 43 */ rdrData._pdvKolTip   = reader.GetUInt16   (CI.pdvKolTip   + ciOffset);
      /* 44 */ rdrData._s_ukKCRP    = reader.GetDecimal  (CI.s_ukKCRP    + ciOffset);
      /* 45 */ rdrData._s_ukKCRM    = reader.GetDecimal  (CI.s_ukKCRM    + ciOffset);
      /* 46 */ rdrData._s_ukKCR     = reader.GetDecimal  (CI.s_ukKCR     + ciOffset);
      /* 47 */ rdrData._s_ukRbt1    = reader.GetDecimal  (CI.s_ukRbt1    + ciOffset);
      /* 48 */ rdrData._s_ukRbt2    = reader.GetDecimal  (CI.s_ukRbt2    + ciOffset);
      /* 49 */ rdrData._s_ukZavisni = reader.GetDecimal  (CI.s_ukZavisni + ciOffset);
      /* 50 */ rdrData._s_ukProlazne= reader.GetDecimal  (CI.s_ukProlazne+ ciOffset);
      /* 51 */ rdrData._s_ukPdv23m  = reader.GetDecimal  (CI.s_ukPdv23m  + ciOffset);
      /* 52 */ rdrData._s_ukPdv23n  = reader.GetDecimal  (CI.s_ukPdv23n  + ciOffset);
      /* 53 */ rdrData._s_ukPdv22m  = reader.GetDecimal  (CI.s_ukPdv22m  + ciOffset);
      /* 54 */ rdrData._s_ukPdv22n  = reader.GetDecimal  (CI.s_ukPdv22n  + ciOffset);
      /* 55 */ rdrData._s_ukPdv10m  = reader.GetDecimal  (CI.s_ukPdv10m  + ciOffset);
      /* 56 */ rdrData._s_ukPdv10n  = reader.GetDecimal  (CI.s_ukPdv10n  + ciOffset);
      /* 57 */ rdrData._s_ukOsn23m  = reader.GetDecimal  (CI.s_ukOsn23m  + ciOffset);
      /* 58 */ rdrData._s_ukOsn23n  = reader.GetDecimal  (CI.s_ukOsn23n  + ciOffset);
      /* 59 */ rdrData._s_ukOsn22m  = reader.GetDecimal  (CI.s_ukOsn22m  + ciOffset);
      /* 50 */ rdrData._s_ukOsn22n  = reader.GetDecimal  (CI.s_ukOsn22n  + ciOffset);
      /* 51 */ rdrData._s_ukOsn10m  = reader.GetDecimal  (CI.s_ukOsn10m  + ciOffset);
      /* 62 */ rdrData._s_ukOsn10n  = reader.GetDecimal  (CI.s_ukOsn10n  + ciOffset);
      /* 63 */ rdrData._s_ukOsn0    = reader.GetDecimal  (CI.s_ukOsn0    + ciOffset);
      /* 64 */ rdrData._s_ukOsnPr   = reader.GetDecimal  (CI.s_ukOsnPr   + ciOffset);
      /* 65 */ rdrData._opciAlabel  = reader.GetString   (CI.opciAlabel  + ciOffset);
      /* 66 */ rdrData._opciAvalue  = reader.GetString   (CI.opciAvalue  + ciOffset);
      /* 67 */ rdrData._opciBlabel  = reader.GetString   (CI.opciBlabel  + ciOffset);
      /* 68 */ rdrData._opciBvalue  = reader.GetString   (CI.opciBvalue  + ciOffset);
      /* 69 */ rdrData._odgvPersCD  = reader.GetUInt32   (CI.odgvPersCD  + ciOffset);
      /* 70 */ rdrData._odgvPersName= reader.GetString   (CI.odgvPersName+ ciOffset);
      /* 71 */ rdrData._cjenTTnum   = reader.GetUInt32   (CI.cjenTTnum   + ciOffset);
      /* 72 */ rdrData._v3_tt       = reader.GetString   (CI.v3_tt       + ciOffset);
      /* 73 */ rdrData._v3_ttNum    = reader.GetUInt32   (CI.v3_ttNum    + ciOffset);
      /* 74 */ rdrData._v4_tt       = reader.GetString   (CI.v4_tt       + ciOffset);
      /* 75 */ rdrData._v4_ttNum    = reader.GetUInt32   (CI.v4_ttNum    + ciOffset);
      /* 76 */ rdrData._s_ukMrz     = reader.GetDecimal  (CI.s_ukMrz     + ciOffset);
      /* 77 */ rdrData._s_ukPdv     = reader.GetDecimal  (CI.s_ukPdv     + ciOffset);
      /* 78 */ rdrData._tipOtpreme  = reader.GetString   (CI.tipOtpreme  + ciOffset);
      /* 79 */ rdrData._rokIsporuke = reader.GetInt32    (CI.rokIsporuke + ciOffset);
      /* 80 */ rdrData._rokIspDate  = reader.GetDateTime (CI.rokIspDate  + ciOffset);
      /* 81 */ rdrData._dostName    = reader.GetString   (CI.dostName    + ciOffset);
      /* 82 */ rdrData._dostAddr    = reader.GetString   (CI.dostAddr    + ciOffset);
      /* 83 */ rdrData._s_ukOsn07   = reader.GetDecimal  (CI.s_ukOsn07   + ciOffset);
      /* 84 */ rdrData._s_ukOsn08   = reader.GetDecimal  (CI.s_ukOsn08   + ciOffset);
      /* 85 */ rdrData._s_ukOsn09   = reader.GetDecimal  (CI.s_ukOsn09   + ciOffset);
      /* 86 */ rdrData._s_ukOsn10   = reader.GetDecimal  (CI.s_ukOsn10   + ciOffset);
      /* 87 */ rdrData._s_ukOsn11   = reader.GetDecimal  (CI.s_ukOsn11   + ciOffset);
      /* 88 */ rdrData._s_ukOsnUr23 = reader.GetDecimal  (CI.s_ukOsnUr23 + ciOffset);
      /* 89 */ rdrData._s_ukOsnUu10 = reader.GetDecimal  (CI.s_ukOsnUu10 + ciOffset);
      /* 90 */ rdrData._s_ukOsnUu22 = reader.GetDecimal  (CI.s_ukOsnUu22 + ciOffset);
      /* 91 */ rdrData._s_ukOsnUu23 = reader.GetDecimal  (CI.s_ukOsnUu23 + ciOffset);
      /* 92 */ rdrData._s_ukPdvUr23 = reader.GetDecimal  (CI.s_ukPdvUr23 + ciOffset);
      /* 93 */ rdrData._s_ukPdvUu10 = reader.GetDecimal  (CI.s_ukPdvUu10 + ciOffset);
      /* 94 */ rdrData._s_ukPdvUu22 = reader.GetDecimal  (CI.s_ukPdvUu22 + ciOffset);
      /* 95 */ rdrData._s_ukPdvUu23 = reader.GetDecimal  (CI.s_ukPdvUu23 + ciOffset);
      /* 96 */ rdrData._carinaKind  = reader.GetUInt16   (CI.carinaKind  + ciOffset);
      /* 97 */ rdrData._prjArtCD    = reader.GetString   (CI.prjArtCD    + ciOffset);
      /* 98 */ rdrData._prjArtName  = reader.GetString   (CI.prjArtName  + ciOffset);
      /* 99 */ rdrData._externLink1 = reader.GetString   (CI.externLink1 + ciOffset);
      /*100 */ rdrData._externLink2 = reader.GetString   (CI.externLink2 + ciOffset);
      /*101 */ rdrData._someMoney   = reader.GetDecimal  (CI.someMoney   + ciOffset);
      /*102 */ rdrData._somePercent = reader.GetDecimal  (CI.somePercent + ciOffset);
      /*103 */ rdrData._s_ukMskPdv10= reader.GetDecimal  (CI.s_ukMskPdv10+ ciOffset);
      /*104 */ rdrData._s_ukMskPdv23= reader.GetDecimal  (CI.s_ukMskPdv23+ ciOffset);
      /*105 */ rdrData._s_ukMSK_00  = reader.GetDecimal  (CI.s_ukMSK_00  + ciOffset);
      /*106 */ rdrData._s_ukMSK_10  = reader.GetDecimal  (CI.s_ukMSK_10  + ciOffset);
      /*107 */ rdrData._s_ukMSK_23  = reader.GetDecimal  (CI.s_ukMSK_23  + ciOffset);
      /*108 */ rdrData._s_ukKCR_usl = reader.GetDecimal  (CI.s_ukKCR_usl + ciOffset);
      /*109 */ rdrData._s_ukKCRP_usl= reader.GetDecimal  (CI.s_ukKCRP_usl+ ciOffset);

      /*110 */ rdrData._s_ukPdv25m  = reader.GetDecimal  (CI.s_ukPdv25m  + ciOffset);
      /*111 */ rdrData._s_ukPdv25n  = reader.GetDecimal  (CI.s_ukPdv25n  + ciOffset);
      /*112 */ rdrData._s_ukOsn25m  = reader.GetDecimal  (CI.s_ukOsn25m  + ciOffset);
      /*113 */ rdrData._s_ukOsn25n  = reader.GetDecimal  (CI.s_ukOsn25n  + ciOffset);
      /*114 */ rdrData._s_ukOsnUr25 = reader.GetDecimal  (CI.s_ukOsnUr25 + ciOffset);
      /*115 */ rdrData._s_ukOsnUu25 = reader.GetDecimal  (CI.s_ukOsnUu25 + ciOffset);
      /*116 */ rdrData._s_ukPdvUr25 = reader.GetDecimal  (CI.s_ukPdvUr25 + ciOffset);
      /*117 */ rdrData._s_ukPdvUu25 = reader.GetDecimal  (CI.s_ukPdvUu25 + ciOffset);
      /*118 */ rdrData._s_ukMskPdv25= reader.GetDecimal  (CI.s_ukMskPdv25+ ciOffset);
      /*119 */ rdrData._s_ukMSK_25  = reader.GetDecimal  (CI.s_ukMSK_25  + ciOffset);

      /*120 */ rdrData._fiskJIR     = reader.GetString   (CI.fiskJIR     + ciOffset);
      /*121 */ rdrData._fiskZKI     = reader.GetString   (CI.fiskZKI     + ciOffset);
      /*122 */ rdrData._fiskMsgID   = reader.GetString   (CI.fiskMsgID   + ciOffset);
      /*123 */ rdrData._fiskOibOp   = reader.GetString   (CI.fiskOibOp   + ciOffset);
      /*124 */ rdrData._fiskPrgBr   = reader.GetString   (CI.fiskPrgBr   + ciOffset);

      /*125 */ rdrData._s_ukPdv05m  = reader.GetDecimal  (CI.s_ukPdv05m  + ciOffset);
      /*126 */ rdrData._s_ukPdv05n  = reader.GetDecimal  (CI.s_ukPdv05n  + ciOffset);
      /*127 */ rdrData._s_ukOsn05m  = reader.GetDecimal  (CI.s_ukOsn05m  + ciOffset);
      /*128 */ rdrData._s_ukOsn05n  = reader.GetDecimal  (CI.s_ukOsn05n  + ciOffset);
      /*129 */ rdrData._s_ukMskPdv05= reader.GetDecimal  (CI.s_ukMskPdv05+ ciOffset);
      /*130 */ rdrData._s_ukMSK_05  = reader.GetDecimal  (CI.s_ukMSK_05  + ciOffset);
      /*131 */ rdrData._s_ukOsnUr05 = reader.GetDecimal  (CI.s_ukOsnUr05 + ciOffset);
      /*132 */ rdrData._s_ukPdvUr05 = reader.GetDecimal  (CI.s_ukPdvUr05 + ciOffset);

      /*133 */ rdrData._s_pixK      = reader.GetDecimal  (CI.s_pixK      + ciOffset);
      /*134 */ rdrData._s_puxK_P    = reader.GetDecimal  (CI.s_puxK_P    + ciOffset);
      /*135 */ rdrData._s_puxK_All  = reader.GetDecimal  (CI.s_puxK_All  + ciOffset);
      /*136 */ rdrData._s_pixKC     = reader.GetDecimal  (CI.s_pixKC     + ciOffset);
      /*137 */ rdrData._s_puxKC_P   = reader.GetDecimal  (CI.s_puxKC_P   + ciOffset);
      /*138 */ rdrData._s_puxKC_All = reader.GetDecimal  (CI.s_puxKC_All + ciOffset);
      /*139 */ rdrData._s_ukPpmvOsn = reader.GetDecimal  (CI.s_ukPpmvOsn  + ciOffset);
      /*140 */ rdrData._s_ukPpmvSt1i2=reader.GetDecimal  (CI.s_ukPpmvSt1i2+ ciOffset);
      /*141 */ rdrData._dateX        =reader.GetDateTime (CI.dateX        + ciOffset);
      /*142 */ rdrData._vatCntryCode =reader.GetString   (CI.vatCntryCode + ciOffset);

      /*143 */ rdrData._pdvGEOkind     = reader.GetUInt16   (CI.pdvGEOkind     + ciOffset);
      /*144 */ rdrData._pdvZPkind      = reader.GetUInt16   (CI.pdvZPkind      + ciOffset);
      /*145 */ rdrData._s_ukOsnR25m_EU = reader.GetDecimal  (CI.s_ukOsnR25m_EU + ciOffset);
      /*146 */ rdrData._s_ukOsnR25n_EU = reader.GetDecimal  (CI.s_ukOsnR25n_EU + ciOffset);
      /*147 */ rdrData._s_ukOsnU25m_EU = reader.GetDecimal  (CI.s_ukOsnU25m_EU + ciOffset);
      /*148 */ rdrData._s_ukOsnU25n_EU = reader.GetDecimal  (CI.s_ukOsnU25n_EU + ciOffset);
      /*149 */ rdrData._s_ukOsnR10m_EU = reader.GetDecimal  (CI.s_ukOsnR10m_EU + ciOffset);
      /*150 */ rdrData._s_ukOsnR10n_EU = reader.GetDecimal  (CI.s_ukOsnR10n_EU + ciOffset);
      /*151 */ rdrData._s_ukOsnU10m_EU = reader.GetDecimal  (CI.s_ukOsnU10m_EU + ciOffset);
      /*152 */ rdrData._s_ukOsnU10n_EU = reader.GetDecimal  (CI.s_ukOsnU10n_EU + ciOffset);
      /*153 */ rdrData._s_ukOsnR05m_EU = reader.GetDecimal  (CI.s_ukOsnR05m_EU + ciOffset);
      /*154 */ rdrData._s_ukOsnR05n_EU = reader.GetDecimal  (CI.s_ukOsnR05n_EU + ciOffset);
      /*155 */ rdrData._s_ukOsnU05m_EU = reader.GetDecimal  (CI.s_ukOsnU05m_EU + ciOffset);
      /*156 */ rdrData._s_ukOsnU05n_EU = reader.GetDecimal  (CI.s_ukOsnU05n_EU + ciOffset);
      /*157 */ rdrData._s_ukOsn25m_BS  = reader.GetDecimal  (CI.s_ukOsn25m_BS  + ciOffset);
      /*158 */ rdrData._s_ukOsn25n_BS  = reader.GetDecimal  (CI.s_ukOsn25n_BS  + ciOffset);
      /*159 */ rdrData._s_ukOsn10m_BS  = reader.GetDecimal  (CI.s_ukOsn10m_BS  + ciOffset);
      /*160 */ rdrData._s_ukOsn10n_BS  = reader.GetDecimal  (CI.s_ukOsn10n_BS  + ciOffset);
      /*161 */ rdrData._s_ukOsn25m_TP  = reader.GetDecimal  (CI.s_ukOsn25m_TP  + ciOffset);
      /*162 */ rdrData._s_ukOsn25n_TP  = reader.GetDecimal  (CI.s_ukOsn25n_TP  + ciOffset);
      /*163 */ rdrData._s_ukPdvR25m_EU = reader.GetDecimal  (CI.s_ukPdvR25m_EU + ciOffset);
      /*164 */ rdrData._s_ukPdvR25n_EU = reader.GetDecimal  (CI.s_ukPdvR25n_EU + ciOffset);
      /*165 */ rdrData._s_ukPdvU25m_EU = reader.GetDecimal  (CI.s_ukPdvU25m_EU + ciOffset);
      /*166 */ rdrData._s_ukPdvU25n_EU = reader.GetDecimal  (CI.s_ukPdvU25n_EU + ciOffset);
      /*167 */ rdrData._s_ukPdvR10m_EU = reader.GetDecimal  (CI.s_ukPdvR10m_EU + ciOffset);
      /*168 */ rdrData._s_ukPdvR10n_EU = reader.GetDecimal  (CI.s_ukPdvR10n_EU + ciOffset);
      /*169 */ rdrData._s_ukPdvU10m_EU = reader.GetDecimal  (CI.s_ukPdvU10m_EU + ciOffset);
      /*170 */ rdrData._s_ukPdvU10n_EU = reader.GetDecimal  (CI.s_ukPdvU10n_EU + ciOffset);
      /*171 */ rdrData._s_ukPdvR05m_EU = reader.GetDecimal  (CI.s_ukPdvR05m_EU + ciOffset);
      /*172 */ rdrData._s_ukPdvR05n_EU = reader.GetDecimal  (CI.s_ukPdvR05n_EU + ciOffset);
      /*173 */ rdrData._s_ukPdvU05m_EU = reader.GetDecimal  (CI.s_ukPdvU05m_EU + ciOffset);
      /*174 */ rdrData._s_ukPdvU05n_EU = reader.GetDecimal  (CI.s_ukPdvU05n_EU + ciOffset);
      /*175 */ rdrData._s_ukPdv25m_BS  = reader.GetDecimal  (CI.s_ukPdv25m_BS  + ciOffset);
      /*176 */ rdrData._s_ukPdv25n_BS  = reader.GetDecimal  (CI.s_ukPdv25n_BS  + ciOffset);
      /*177 */ rdrData._s_ukPdv10m_BS  = reader.GetDecimal  (CI.s_ukPdv10m_BS  + ciOffset);
      /*178 */ rdrData._s_ukPdv10n_BS  = reader.GetDecimal  (CI.s_ukPdv10n_BS  + ciOffset);
      /*179 */ rdrData._s_ukPdv25m_TP  = reader.GetDecimal  (CI.s_ukPdv25m_TP  + ciOffset);
      /*180 */ rdrData._s_ukPdv25n_TP  = reader.GetDecimal  (CI.s_ukPdv25n_TP  + ciOffset);
      /*181 */ rdrData._s_ukOsnZP_11   = reader.GetDecimal  (CI.s_ukOsnZP_11   + ciOffset);
      /*182 */ rdrData._s_ukOsnZP_12   = reader.GetDecimal  (CI.s_ukOsnZP_12   + ciOffset);
      /*183 */ rdrData._s_ukOsnZP_13   = reader.GetDecimal  (CI.s_ukOsnZP_13   + ciOffset);
      /*184 */ rdrData._s_ukOsn12      = reader.GetDecimal  (CI.s_ukOsn12      + ciOffset);
      /*185 */ rdrData._s_ukOsn13      = reader.GetDecimal  (CI.s_ukOsn13      + ciOffset);
      /*186 */ rdrData._s_ukOsn14      = reader.GetDecimal  (CI.s_ukOsn14      + ciOffset);
      /*187 */ rdrData._s_ukOsn15      = reader.GetDecimal  (CI.s_ukOsn15      + ciOffset);
      /*188 */ rdrData._s_ukOsn16      = reader.GetDecimal  (CI.s_ukOsn16      + ciOffset);
      /*189 */ rdrData._s_ukOsnPNP     = reader.GetDecimal  (CI.s_ukOsnPNP     + ciOffset);
      /*190 */ rdrData._s_ukIznPNP     = reader.GetDecimal  (CI.s_ukIznPNP     + ciOffset);
      /*191 */ rdrData._s_ukMskPNP     = reader.GetDecimal  (CI.s_ukMskPNP     + ciOffset);
      /*192 */ rdrData._skiz_ukKC      = reader.GetDecimal  (CI.skiz_ukKC      + ciOffset);
      /*193 */ rdrData._skiz_ukKCR     = reader.GetDecimal  (CI.skiz_ukKCR     + ciOffset);
      /*194 */ rdrData._skiz_ukRbt1    = reader.GetDecimal  (CI.skiz_ukRbt1    + ciOffset);

      int nextReaderIndex = lastFaktExCI + 1 + ciOffset;

      #region R2_uplata za PDV.TheFakturList

      if((reader.FieldCount - 1) == (nextReaderIndex)) // da ovdje uopce ulazi samo ako je u selectWhat navedeno " L.*, R.*, " + ftrUplata + " " (PDV Knjiga UR-A only)
      {
         try
         {
            ((FaktEx)vvDataRecord).R2_uplata = reader.GetDecimal(nextReaderIndex); // " + ftrUplata + " 
         }
         catch(SqlNullValueException /*Exception*/) { } // kada je po join-u null 
      }

      #endregion R2_uplata za PDV.TheFakturList

      #region FillFrom_ URM & IRM Fak2Nal_Reader ADDITIONS

      if((vvDataRecord as FaktEx).IsFillingFrom_Fak2Nal_URM == true)
      {
         try
         {
            /* 13 */ ((FaktEx)vvDataRecord).K_NivVrj00 = reader.GetDecimal(nextReaderIndex++);
            /* 14 */ ((FaktEx)vvDataRecord).K_NivVrj10 = reader.GetDecimal(nextReaderIndex++);
            /* 15 */ ((FaktEx)vvDataRecord).K_NivVrj23 = reader.GetDecimal(nextReaderIndex++);
            /*    */ ((FaktEx)vvDataRecord).K_NivVrj25 = reader.GetDecimal(nextReaderIndex++);
            /*    */ ((FaktEx)vvDataRecord).K_NivVrj05 = reader.GetDecimal(nextReaderIndex++);
         }
         catch(SqlNullValueException /*Exception*/) { } // kada je po join-u null 
      }

      if((vvDataRecord as FaktEx).IsFillingFrom_Fak2Nal_IRM == true)
      {
         try
         {
            /* 13 */ ((FaktEx)vvDataRecord).K_NivVrj00 = reader.GetDecimal(nextReaderIndex++);
            /* 14 */ ((FaktEx)vvDataRecord).K_NivVrj10 = reader.GetDecimal(nextReaderIndex++);
            /* 15 */ ((FaktEx)vvDataRecord).K_NivVrj23 = reader.GetDecimal(nextReaderIndex++);
            /*    */ ((FaktEx)vvDataRecord).K_NivVrj25 = reader.GetDecimal(nextReaderIndex++);
            /*    */ ((FaktEx)vvDataRecord).K_NivVrj05 = reader.GetDecimal(nextReaderIndex++);

            /*    */ ((FaktEx)vvDataRecord).Ira_ROB_NV = reader.GetDecimal(nextReaderIndex++);
         }
         catch(SqlNullValueException /*Exception*/) { } // kada je po join-u null 
      }

      #endregion FillFrom_ URM & IRM _Fak2Nal_Reader ADDITIONS

      ((FaktEx)vvDataRecord).CurrentData = rdrData;

      #region Kune Backup Values

      ((FaktEx)vvDataRecord).Skn_ukRbt1 = ((FaktEx)vvDataRecord).S_ukRbt1;
      ((FaktEx)vvDataRecord).Skn_ukKCR  = ((FaktEx)vvDataRecord).S_ukKCR ;
      ((FaktEx)vvDataRecord).Skn_ukKCRP = ((FaktEx)vvDataRecord).S_ukKCRP;

      #endregion Kune Backup Values

      return;
   }

   #endregion FillFromDataReader

   #region FaktExCI struct & InitializeSchemaColumnIndexes()

   public struct FaktExCI
   {
      internal int recID;

      /* 01 */ internal int fakturRecID ;
      /* 02 */ internal int pdvNum      ;
      /* 03 */ internal int pdvDate     ;
      /* 04 */ internal int kupdobCD    ;
      /* 05 */ internal int kupdobTK    ;
      /* 06 */ internal int kupdobName  ;
      /* 07 */ internal int kdUlica     ;
      /* 08 */ internal int kdMjesto    ;
      /* 09 */ internal int kdZip       ;
      /* 10 */ internal int kdOib       ;
      /* 11 */ internal int posJedCD    ;
      /* 12 */ internal int posJedTK    ;
      /* 13 */ internal int posJedName  ;
      /* 14 */ internal int posJedUlica ;
      /* 15 */ internal int posJedMjesto;
      /* 16 */ internal int posJedZip   ;
      /* 17 */ internal int vezniDok2   ;
      /* 18 */ internal int fco         ;
      /* 19 */ internal int rokPlac     ;
      /* 20 */ internal int dospDate    ;
      /* 21 */ internal int skladDate   ;
      /* 22 */ internal int nacPlac     ;
      /* 23 */ internal int ziroRn      ;
      /* 24 */ internal int devName     ;
      /* 25 */ internal int pnbM        ;
      /* 26 */ internal int pnbV        ;
      /* 27 */ internal int personCD    ;
      /* 28 */ internal int personName  ;
      /* 29 */ internal int napomena2   ;
      /* 30 */ internal int cjenikTT    ;
      /* 31 */ internal int statusCD    ;
      /* 32 */ internal int rokPonude   ;
      /* 33 */ internal int ponudDate   ;
      /* 34 */ internal int mtrosCD     ;
      /* 35 */ internal int mtrosTK     ;
      /* 36 */ internal int mtrosName   ;
      /* 37 */ internal int primPlatCD  ;
      /* 38 */ internal int primPlatTK  ;
      /* 39 */ internal int primPlatName;
      /* 40 */ internal int pdvKnjiga   ;
      /* 41 */ internal int isNpCash    ;
      /* 42 */ internal int pdvR12      ;
      /* 43 */ internal int pdvKolTip   ;
      /* 44 */ internal int s_ukKCRP    ;
      /* 45 */ internal int s_ukKCRM    ;
      /* 46 */ internal int s_ukKCR     ;
      /* 47 */ internal int s_ukRbt1    ;
      /* 48 */ internal int s_ukRbt2    ;
      /* 49 */ internal int s_ukZavisni ;
      /* 50 */ internal int s_ukProlazne;
      /* 51 */ internal int s_ukPdv23m  ;
      /* 52 */ internal int s_ukPdv23n  ;
      /* 53 */ internal int s_ukPdv22m  ;
      /* 54 */ internal int s_ukPdv22n  ;
      /* 55 */ internal int s_ukPdv10m  ;
      /* 56 */ internal int s_ukPdv10n  ;
      /* 57 */ internal int s_ukOsn23m  ;
      /* 58 */ internal int s_ukOsn23n  ;
      /* 59 */ internal int s_ukOsn22m  ;
      /* 60 */ internal int s_ukOsn22n  ;
      /* 61 */ internal int s_ukOsn10m  ;
      /* 62 */ internal int s_ukOsn10n  ;
      /* 63 */ internal int s_ukOsn0    ;
      /* 64 */ internal int s_ukOsnPr   ;
      /* 65 */ internal int opciAlabel  ;
      /* 66 */ internal int opciAvalue  ;
      /* 67 */ internal int opciBlabel  ;
      /* 68 */ internal int opciBvalue  ;
      /* 69 */ internal int odgvPersCD  ;
      /* 70 */ internal int odgvPersName;
      /* 71 */ internal int cjenTTnum   ;
      /* 72 */ internal int v3_tt       ;
      /* 73 */ internal int v3_ttNum    ;
      /* 74 */ internal int v4_tt       ;
      /* 75 */ internal int v4_ttNum    ;
      /* 76 */ internal int s_ukMrz     ;
      /* 77 */ internal int s_ukPdv     ;
      /* 78 */ internal int tipOtpreme  ;
      /* 79 */ internal int rokIsporuke ;
      /* 80 */ internal int rokIspDate  ;
      /* 81 */ internal int dostName    ;
      /* 82 */ internal int dostAddr    ;
      /* 83 */ internal int s_ukOsn07   ;
      /* 84 */ internal int s_ukOsn08   ;
      /* 85 */ internal int s_ukOsn09   ;
      /* 86 */ internal int s_ukOsn10   ;
      /* 87 */ internal int s_ukOsn11   ;
      /* 88 */ internal int s_ukOsnUr23 ;
      /* 89 */ internal int s_ukOsnUu10 ;
      /* 90 */ internal int s_ukOsnUu22 ;
      /* 91 */ internal int s_ukOsnUu23 ;
      /* 92 */ internal int s_ukPdvUr23 ;
      /* 93 */ internal int s_ukPdvUu10 ;
      /* 94 */ internal int s_ukPdvUu22 ;
      /* 95 */ internal int s_ukPdvUu23 ;
      /* 96 */ internal int carinaKind  ;
      /* 97 */ internal int prjArtCD    ;
      /* 98 */ internal int prjArtName  ;
      /* 99 */ internal int externLink1 ;
      /*100 */ internal int externLink2 ;
      /*101 */ internal int someMoney   ;
      /*102 */ internal int somePercent ;
      /*103 */ internal int s_ukMskPdv10;
      /*104 */ internal int s_ukMskPdv23;
      /*105 */ internal int s_ukMSK_00  ;
      /*106 */ internal int s_ukMSK_10  ;
      /*107 */ internal int s_ukMSK_23  ;
      /*108 */ internal int s_ukKCR_usl ;
      /*109 */ internal int s_ukKCRP_usl;
      /*110 */ internal int s_ukPdv25m  ;
      /*111 */ internal int s_ukPdv25n  ;
      /*112 */ internal int s_ukOsn25m  ;
      /*113 */ internal int s_ukOsn25n  ;
      /*114 */ internal int s_ukOsnUr25 ;
      /*115 */ internal int s_ukOsnUu25 ;
      /*116 */ internal int s_ukPdvUr25 ;
      /*117 */ internal int s_ukPdvUu25 ;
      /*118 */ internal int s_ukMskPdv25;
      /*119 */ internal int s_ukMSK_25  ;

      /*120 */ internal int fiskJIR     ;
      /*121 */ internal int fiskZKI     ;
      /*122 */ internal int fiskMsgID   ;
      /*123 */ internal int fiskOibOp   ;
      /*124 */ internal int fiskPrgBr   ;

      /*125 */ internal int s_ukPdv05m  ;
      /*126 */ internal int s_ukPdv05n  ;
      /*127 */ internal int s_ukOsn05m  ;
      /*128 */ internal int s_ukOsn05n  ;
      /*129 */ internal int s_ukMskPdv05;
      /*130 */ internal int s_ukMSK_05  ;
      /*131 */ internal int s_ukOsnUr05 ;
      /*132 */ internal int s_ukPdvUr05 ;

      /*133 */ internal int s_pixK      ;
      /*134 */ internal int s_puxK_P    ;
      /*135 */ internal int s_puxK_All  ;
      /*136 */ internal int s_pixKC     ;
      /*137 */ internal int s_puxKC_P   ;
      /*138 */ internal int s_puxKC_All ;
      /*139 */ internal int s_ukPpmvOsn  ;
      /*140 */ internal int s_ukPpmvSt1i2;
      /*141 */ internal int dateX        ;
      /*142 */ internal int vatCntryCode ;

      /* 52 */ internal int pdvGEOkind    ;
      /* 52 */ internal int pdvZPkind     ;
      /*145 */ internal int s_ukOsnR25m_EU;
      /*146 */ internal int s_ukOsnR25n_EU;
      /*147 */ internal int s_ukOsnU25m_EU;
      /*148 */ internal int s_ukOsnU25n_EU;
      /*149 */ internal int s_ukOsnR10m_EU;
      /*150 */ internal int s_ukOsnR10n_EU;
      /*151 */ internal int s_ukOsnU10m_EU;
      /*152 */ internal int s_ukOsnU10n_EU;
      /*153 */ internal int s_ukOsnR05m_EU;
      /*154 */ internal int s_ukOsnR05n_EU;
      /*155 */ internal int s_ukOsnU05m_EU;
      /*156 */ internal int s_ukOsnU05n_EU;
      /*157 */ internal int s_ukOsn25m_BS ;
      /*158 */ internal int s_ukOsn25n_BS ;
      /*159 */ internal int s_ukOsn10m_BS ;
      /*160 */ internal int s_ukOsn10n_BS ;
      /*161 */ internal int s_ukOsn25m_TP ;
      /*162 */ internal int s_ukOsn25n_TP ;
      /*163 */ internal int s_ukPdvR25m_EU;
      /*164 */ internal int s_ukPdvR25n_EU;
      /*165 */ internal int s_ukPdvU25m_EU;
      /*166 */ internal int s_ukPdvU25n_EU;
      /*167 */ internal int s_ukPdvR10m_EU;
      /*168 */ internal int s_ukPdvR10n_EU;
      /*169 */ internal int s_ukPdvU10m_EU;
      /*170 */ internal int s_ukPdvU10n_EU;
      /*171 */ internal int s_ukPdvR05m_EU;
      /*172 */ internal int s_ukPdvR05n_EU;
      /*173 */ internal int s_ukPdvU05m_EU;
      /*174 */ internal int s_ukPdvU05n_EU;
      /*175 */ internal int s_ukPdv25m_BS ;
      /*176 */ internal int s_ukPdv25n_BS ;
      /*177 */ internal int s_ukPdv10m_BS ;
      /*178 */ internal int s_ukPdv10n_BS ;
      /*179 */ internal int s_ukPdv25m_TP ;
      /*180 */ internal int s_ukPdv25n_TP ;
      /*181 */ internal int s_ukOsnZP_11  ;
      /*182 */ internal int s_ukOsnZP_12  ;
      /*183 */ internal int s_ukOsnZP_13  ;
      /*184 */ internal int s_ukOsn12     ;
      /*185 */ internal int s_ukOsn13     ;
      /*186 */ internal int s_ukOsn14     ;
      /*187 */ internal int s_ukOsn15     ;
      /*188 */ internal int s_ukOsn16     ;
      /*189 */ internal int s_ukOsnPNP    ;
      /*190 */ internal int s_ukIznPNP    ;
      /*191 */ internal int s_ukMskPNP    ;
      /*192 */ internal int skiz_ukKC     ;
      /*193 */ internal int skiz_ukKCR    ;
      /*194 */ internal int skiz_ukRbt1   ;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public FaktExCI CI;
   public static int lastFaktExCI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID          = GetSchemaColumnIndex("recID");

      /* 01 */ CI.fakturRecID  = GetSchemaColumnIndex("fakturRecID");
      /* 02 */ CI.pdvNum       = GetSchemaColumnIndex("pdvNum");
      /* 03 */ CI.pdvDate      = GetSchemaColumnIndex("pdvDate");
      /* 04 */ CI.kupdobCD     = GetSchemaColumnIndex("kupdobCD");
      /* 05 */ CI.kupdobTK     = GetSchemaColumnIndex("kupdobTK");
      /* 06 */ CI.kupdobName   = GetSchemaColumnIndex("kupdobName");
      /* 07 */ CI.kdUlica      = GetSchemaColumnIndex("kdUlica");
      /* 08 */ CI.kdMjesto     = GetSchemaColumnIndex("kdMjesto");
      /* 09 */ CI.kdZip        = GetSchemaColumnIndex("kdZip");
      /* 10 */ CI.kdOib        = GetSchemaColumnIndex("kdOib");
      /* 11 */ CI.posJedCD     = GetSchemaColumnIndex("posJedCD");
      /* 12 */ CI.posJedTK     = GetSchemaColumnIndex("posJedTK");
      /* 13 */ CI.posJedName   = GetSchemaColumnIndex("posJedName");
      /* 14 */ CI.posJedUlica  = GetSchemaColumnIndex("posJedUlica");
      /* 15 */ CI.posJedMjesto = GetSchemaColumnIndex("posJedMjesto");
      /* 16 */ CI.posJedZip    = GetSchemaColumnIndex("posJedZip");
      /* 17 */ CI.vezniDok2    = GetSchemaColumnIndex("vezniDok2");
      /* 18 */ CI.fco          = GetSchemaColumnIndex("fco");
      /* 19 */ CI.rokPlac      = GetSchemaColumnIndex("rokPlac");
      /* 20 */ CI.dospDate     = GetSchemaColumnIndex("dospDate");
      /* 21 */ CI.skladDate    = GetSchemaColumnIndex("skladDate");
      /* 22 */ CI.nacPlac      = GetSchemaColumnIndex("nacPlac");
      /* 23 */ CI.ziroRn       = GetSchemaColumnIndex("ziroRn");
      /* 24 */ CI.devName      = GetSchemaColumnIndex("devName");
      /* 25 */ CI.pnbM         = GetSchemaColumnIndex("pnbM");
      /* 26 */ CI.pnbV         = GetSchemaColumnIndex("pnbV");
      /* 27 */ CI.personCD     = GetSchemaColumnIndex("personCD");
      /* 28 */ CI.personName   = GetSchemaColumnIndex("personName");
      /* 29 */ CI.napomena2    = GetSchemaColumnIndex("napomena2");
      /* 30 */ CI.cjenikTT     = GetSchemaColumnIndex("cjenikTT");
      /* 31 */ CI.statusCD     = GetSchemaColumnIndex("statusCD");
      /* 32 */ CI.rokPonude    = GetSchemaColumnIndex("rokPonude");
      /* 33 */ CI.ponudDate    = GetSchemaColumnIndex("ponudDate");
      /* 34 */ CI.mtrosCD      = GetSchemaColumnIndex("mtrosCD");
      /* 35 */ CI.mtrosTK      = GetSchemaColumnIndex("mtrosTK");
      /* 36 */ CI.mtrosName    = GetSchemaColumnIndex("mtrosName");
      /* 37 */ CI.primPlatCD   = GetSchemaColumnIndex("primPlatCD");
      /* 38 */ CI.primPlatTK   = GetSchemaColumnIndex("primPlatTK");
      /* 39 */ CI.primPlatName = GetSchemaColumnIndex("primPlatName");
      /* 40 */ CI.pdvKnjiga    = GetSchemaColumnIndex("pdvKnjiga");
      /* 41 */ CI.isNpCash     = GetSchemaColumnIndex("isNpCash");
      /* 42 */ CI.pdvR12       = GetSchemaColumnIndex("pdvR12");
      /* 43 */ CI.pdvKolTip    = GetSchemaColumnIndex("pdvKolTip");
      /* 44 */ CI.s_ukKCRP     = GetSchemaColumnIndex("s_ukKCRMP");
      /* 45 */ CI.s_ukKCRM     = GetSchemaColumnIndex("s_ukKCRM");
      /* 46 */ CI.s_ukKCR      = GetSchemaColumnIndex("s_ukKCR");
      /* 47 */ CI.s_ukRbt1     = GetSchemaColumnIndex("s_ukRbt1");
      /* 48 */ CI.s_ukRbt2     = GetSchemaColumnIndex("s_ukRbt2");
      /* 49 */ CI.s_ukZavisni  = GetSchemaColumnIndex("s_ukZavisni");
      /* 50 */ CI.s_ukProlazne = GetSchemaColumnIndex("s_ukProlazne");
      /* 51 */ CI.s_ukPdv23m   = GetSchemaColumnIndex("s_ukPdv23m");
      /* 52 */ CI.s_ukPdv23n   = GetSchemaColumnIndex("s_ukPdv23n");
      /* 53 */ CI.s_ukPdv22m   = GetSchemaColumnIndex("s_ukPdv22m");
      /* 54 */ CI.s_ukPdv22n   = GetSchemaColumnIndex("s_ukPdv22n");
      /* 55 */ CI.s_ukPdv10m   = GetSchemaColumnIndex("s_ukPdv10m");
      /* 56 */ CI.s_ukPdv10n   = GetSchemaColumnIndex("s_ukPdv10n");
      /* 57 */ CI.s_ukOsn23m   = GetSchemaColumnIndex("s_ukOsn23m");
      /* 58 */ CI.s_ukOsn23n   = GetSchemaColumnIndex("s_ukOsn23n");
      /* 59 */ CI.s_ukOsn22m   = GetSchemaColumnIndex("s_ukOsn22m");
      /* 50 */ CI.s_ukOsn22n   = GetSchemaColumnIndex("s_ukOsn22n");
      /* 51 */ CI.s_ukOsn10m   = GetSchemaColumnIndex("s_ukOsn10m");
      /* 62 */ CI.s_ukOsn10n   = GetSchemaColumnIndex("s_ukOsn10n");
      /* 63 */ CI.s_ukOsn0     = GetSchemaColumnIndex("s_ukOsn0");
      /* 64 */ CI.s_ukOsnPr    = GetSchemaColumnIndex("s_ukOsnPr");   
      /* 65 */ CI.opciAlabel   = GetSchemaColumnIndex("opciAlabel");   
      /* 66 */ CI.opciAvalue   = GetSchemaColumnIndex("opciAvalue");   
      /* 67 */ CI.opciBlabel   = GetSchemaColumnIndex("opciBlabel");   
      /* 68 */ CI.opciBvalue   = GetSchemaColumnIndex("opciBvalue");
      /* 69 */ CI.odgvPersCD   = GetSchemaColumnIndex("odgvPersCD");
      /* 70 */ CI.odgvPersName = GetSchemaColumnIndex("odgvPersName");
      /* 71 */ CI.cjenTTnum    = GetSchemaColumnIndex("cjenTTnum");
      /* 72 */ CI.v3_tt        = GetSchemaColumnIndex("v3_tt");
      /* 73 */ CI.v3_ttNum     = GetSchemaColumnIndex("v3_ttNum");
      /* 74 */ CI.v4_tt        = GetSchemaColumnIndex("v4_tt");
      /* 75 */ CI.v4_ttNum     = GetSchemaColumnIndex("v4_ttNum");
      /* 76 */ CI.s_ukMrz      = GetSchemaColumnIndex("s_ukMrz");
      /* 77 */ CI.s_ukPdv      = GetSchemaColumnIndex("s_ukPdv");
      /* 78 */ CI.tipOtpreme   = GetSchemaColumnIndex("tipOtpreme");
      /* 79 */ CI.rokIsporuke  = GetSchemaColumnIndex("rokIsporuke");
      /* 80 */ CI.rokIspDate   = GetSchemaColumnIndex("rokIspDate");
      /* 81 */ CI.dostName     = GetSchemaColumnIndex("dostName");
      /* 82 */ CI.dostAddr     = GetSchemaColumnIndex("dostAddr");
      /* 83 */ CI.s_ukOsn07    = GetSchemaColumnIndex("s_ukOsn07");
      /* 84 */ CI.s_ukOsn08    = GetSchemaColumnIndex("s_ukOsn08");
      /* 85 */ CI.s_ukOsn09    = GetSchemaColumnIndex("s_ukOsn09");
      /* 86 */ CI.s_ukOsn10    = GetSchemaColumnIndex("s_ukOsn10");
      /* 87 */ CI.s_ukOsn11    = GetSchemaColumnIndex("s_ukOsn11");
      /* 88 */ CI.s_ukOsnUr23  = GetSchemaColumnIndex("s_ukOsnUr23");
      /* 89 */ CI.s_ukOsnUu10  = GetSchemaColumnIndex("s_ukOsnUu10");
      /* 80 */ CI.s_ukOsnUu22  = GetSchemaColumnIndex("s_ukOsnUu22");
      /* 81 */ CI.s_ukOsnUu23  = GetSchemaColumnIndex("s_ukOsnUu23");
      /* 82 */ CI.s_ukPdvUr23  = GetSchemaColumnIndex("s_ukPdvUr23");
      /* 83 */ CI.s_ukPdvUu10  = GetSchemaColumnIndex("s_ukPdvUu10");
      /* 84 */ CI.s_ukPdvUu22  = GetSchemaColumnIndex("s_ukPdvUu22");
      /* 85 */ CI.s_ukPdvUu23  = GetSchemaColumnIndex("s_ukPdvUu23");
      /* 96 */ CI.carinaKind   = GetSchemaColumnIndex("carinaKind");
      /* 97 */ CI.prjArtCD     = GetSchemaColumnIndex("prjArtCD");
      /* 98 */ CI.prjArtName   = GetSchemaColumnIndex("prjArtName");
      /* 99 */ CI.externLink1  = GetSchemaColumnIndex("externLink1");
      /*100 */ CI.externLink2  = GetSchemaColumnIndex("externLink2");
      /*101 */ CI.someMoney    = GetSchemaColumnIndex("someMoney");
      /*102 */ CI.somePercent  = GetSchemaColumnIndex("somePercent");
      /*103 */ CI.s_ukMskPdv10 = GetSchemaColumnIndex("s_ukMskPdv10");
      /*104 */ CI.s_ukMskPdv23 = GetSchemaColumnIndex("s_ukMskPdv23");
      /*105 */ CI.s_ukMSK_00   = GetSchemaColumnIndex("s_ukMSK_00");
      /*106 */ CI.s_ukMSK_10   = GetSchemaColumnIndex("s_ukMSK_10");
      /*107 */ CI.s_ukMSK_23   = GetSchemaColumnIndex("s_ukMSK_23");
      /*108 */ CI.s_ukKCR_usl  = GetSchemaColumnIndex("s_ukKCR_usl");
      /*109 */ CI.s_ukKCRP_usl = GetSchemaColumnIndex("s_ukKCRP_usl");

      /*110 */ CI.s_ukPdv25m   = GetSchemaColumnIndex("s_ukPdv25m");
      /*111 */ CI.s_ukPdv25n   = GetSchemaColumnIndex("s_ukPdv25n");
      /*112 */ CI.s_ukOsn25m   = GetSchemaColumnIndex("s_ukOsn25m");
      /*113 */ CI.s_ukOsn25n   = GetSchemaColumnIndex("s_ukOsn25n");
      /*114 */ CI.s_ukOsnUr25  = GetSchemaColumnIndex("s_ukOsnUr25");
      /*115 */ CI.s_ukOsnUu25  = GetSchemaColumnIndex("s_ukOsnUu25");
      /*116 */ CI.s_ukPdvUr25  = GetSchemaColumnIndex("s_ukPdvUr25");
      /*117 */ CI.s_ukPdvUu25  = GetSchemaColumnIndex("s_ukPdvUu25");
      /*118 */ CI.s_ukMskPdv25 = GetSchemaColumnIndex("s_ukMskPdv25");
      /*119 */ CI.s_ukMSK_25   = GetSchemaColumnIndex("s_ukMSK_25");

      /*120 */ CI.fiskJIR     = GetSchemaColumnIndex("fiskJIR")  ;
      /*121 */ CI.fiskZKI     = GetSchemaColumnIndex("fiskZKI")  ;
      /*122 */ CI.fiskMsgID   = GetSchemaColumnIndex("fiskMsgID");
      /*123 */ CI.fiskOibOp   = GetSchemaColumnIndex("fiskOibOp");
      /*124 */ CI.fiskPrgBr   = GetSchemaColumnIndex("fiskPrgBr");

      /*125 */ CI.s_ukPdv05m  = GetSchemaColumnIndex("s_ukPdv05m"  );
      /*126 */ CI.s_ukPdv05n  = GetSchemaColumnIndex("s_ukPdv05n"  );
      /*127 */ CI.s_ukOsn05m  = GetSchemaColumnIndex("s_ukOsn05m"  );
      /*128 */ CI.s_ukOsn05n  = GetSchemaColumnIndex("s_ukOsn05n"  );
      /*129 */ CI.s_ukMskPdv05= GetSchemaColumnIndex("s_ukMskPdv05");
      /*130 */ CI.s_ukMSK_05  = GetSchemaColumnIndex("s_ukMSK_05"  );
      /*131 */ CI.s_ukOsnUr05  = GetSchemaColumnIndex("s_ukOsnUr05");
      /*132 */ CI.s_ukPdvUr05  = GetSchemaColumnIndex("s_ukPdvUr05");

      /*133 */ CI.s_pixK       = GetSchemaColumnIndex("s_pixK")       ;
      /*134 */ CI.s_puxK_P     = GetSchemaColumnIndex("s_puxK_P")     ;
      /*135 */ CI.s_puxK_All   = GetSchemaColumnIndex("s_puxK_All")   ;
      /*136 */ CI.s_pixKC      = GetSchemaColumnIndex("s_pixKC")      ;
      /*137 */ CI.s_puxKC_P    = GetSchemaColumnIndex("s_puxKC_P")    ;
      /*138 */ CI.s_puxKC_All  = GetSchemaColumnIndex("s_puxKC_All")  ;
      /*139 */ CI.s_ukPpmvOsn  = GetSchemaColumnIndex("s_ukPpmvOsn")  ;
      /*140 */ CI.s_ukPpmvSt1i2= GetSchemaColumnIndex("s_ukPpmvSt1i2");
      /*141 */ CI.dateX        = GetSchemaColumnIndex("dateX")        ;
      /*142 */ CI.vatCntryCode = GetSchemaColumnIndex("vatCntryCode") ;

      /*143 */ CI.pdvGEOkind     = GetSchemaColumnIndex("pdvGEOkind");
      /*144 */ CI.pdvZPkind      = GetSchemaColumnIndex("pdvZPkind");
      /*145 */ CI.s_ukOsnR25m_EU = GetSchemaColumnIndex("s_ukOsnR25m_EU");
      /*146 */ CI.s_ukOsnR25n_EU = GetSchemaColumnIndex("s_ukOsnR25n_EU");
      /*147 */ CI.s_ukOsnU25m_EU = GetSchemaColumnIndex("s_ukOsnU25m_EU");
      /*148 */ CI.s_ukOsnU25n_EU = GetSchemaColumnIndex("s_ukOsnU25n_EU");
      /*149 */ CI.s_ukOsnR10m_EU = GetSchemaColumnIndex("s_ukOsnR10m_EU");
      /*150 */ CI.s_ukOsnR10n_EU = GetSchemaColumnIndex("s_ukOsnR10n_EU");
      /*151 */ CI.s_ukOsnU10m_EU = GetSchemaColumnIndex("s_ukOsnU10m_EU");
      /*152 */ CI.s_ukOsnU10n_EU = GetSchemaColumnIndex("s_ukOsnU10n_EU");
      /*153 */ CI.s_ukOsnR05m_EU = GetSchemaColumnIndex("s_ukOsnR05m_EU");
      /*154 */ CI.s_ukOsnR05n_EU = GetSchemaColumnIndex("s_ukOsnR05n_EU");
      /*155 */ CI.s_ukOsnU05m_EU = GetSchemaColumnIndex("s_ukOsnU05m_EU");
      /*156 */ CI.s_ukOsnU05n_EU = GetSchemaColumnIndex("s_ukOsnU05n_EU");
      /*157 */ CI.s_ukOsn25m_BS  = GetSchemaColumnIndex("s_ukOsn25m_BS");
      /*158 */ CI.s_ukOsn25n_BS  = GetSchemaColumnIndex("s_ukOsn25n_BS");
      /*159 */ CI.s_ukOsn10m_BS  = GetSchemaColumnIndex("s_ukOsn10m_BS");
      /*160 */ CI.s_ukOsn10n_BS  = GetSchemaColumnIndex("s_ukOsn10n_BS");
      /*161 */ CI.s_ukOsn25m_TP  = GetSchemaColumnIndex("s_ukOsn25m_TP");
      /*162 */ CI.s_ukOsn25n_TP  = GetSchemaColumnIndex("s_ukOsn25n_TP");
      /*163 */ CI.s_ukPdvR25m_EU = GetSchemaColumnIndex("s_ukPdvR25m_EU");
      /*164 */ CI.s_ukPdvR25n_EU = GetSchemaColumnIndex("s_ukPdvR25n_EU");
      /*165 */ CI.s_ukPdvU25m_EU = GetSchemaColumnIndex("s_ukPdvU25m_EU");
      /*166 */ CI.s_ukPdvU25n_EU = GetSchemaColumnIndex("s_ukPdvU25n_EU");
      /*167 */ CI.s_ukPdvR10m_EU = GetSchemaColumnIndex("s_ukPdvR10m_EU");
      /*168 */ CI.s_ukPdvR10n_EU = GetSchemaColumnIndex("s_ukPdvR10n_EU");
      /*169 */ CI.s_ukPdvU10m_EU = GetSchemaColumnIndex("s_ukPdvU10m_EU");
      /*170 */ CI.s_ukPdvU10n_EU = GetSchemaColumnIndex("s_ukPdvU10n_EU");
      /*171 */ CI.s_ukPdvR05m_EU = GetSchemaColumnIndex("s_ukPdvR05m_EU");
      /*172 */ CI.s_ukPdvR05n_EU = GetSchemaColumnIndex("s_ukPdvR05n_EU");
      /*173 */ CI.s_ukPdvU05m_EU = GetSchemaColumnIndex("s_ukPdvU05m_EU");
      /*174 */ CI.s_ukPdvU05n_EU = GetSchemaColumnIndex("s_ukPdvU05n_EU");
      /*175 */ CI.s_ukPdv25m_BS  = GetSchemaColumnIndex("s_ukPdv25m_BS");
      /*176 */ CI.s_ukPdv25n_BS  = GetSchemaColumnIndex("s_ukPdv25n_BS");
      /*177 */ CI.s_ukPdv10m_BS  = GetSchemaColumnIndex("s_ukPdv10m_BS");
      /*178 */ CI.s_ukPdv10n_BS  = GetSchemaColumnIndex("s_ukPdv10n_BS");
      /*179 */ CI.s_ukPdv25m_TP  = GetSchemaColumnIndex("s_ukPdv25m_TP");
      /*180 */ CI.s_ukPdv25n_TP  = GetSchemaColumnIndex("s_ukPdv25n_TP");
      /*181 */ CI.s_ukOsnZP_11   = GetSchemaColumnIndex("s_ukOsnZP_11");
      /*182 */ CI.s_ukOsnZP_12   = GetSchemaColumnIndex("s_ukOsnZP_12");
      /*183 */ CI.s_ukOsnZP_13   = GetSchemaColumnIndex("s_ukOsnZP_13");
      /*184 */ CI.s_ukOsn12      = GetSchemaColumnIndex("s_ukOsn12");
      /*185 */ CI.s_ukOsn13      = GetSchemaColumnIndex("s_ukOsn13");
      /*186 */ CI.s_ukOsn14      = GetSchemaColumnIndex("s_ukOsn14");
      /*187 */ CI.s_ukOsn15      = GetSchemaColumnIndex("s_ukOsn15");
      /*188 */ CI.s_ukOsn16      = GetSchemaColumnIndex("s_ukOsn16");
      /*189 */ CI.s_ukOsnPNP     = GetSchemaColumnIndex("s_ukOsnPNP");
      /*190 */ CI.s_ukIznPNP     = GetSchemaColumnIndex("s_ukIznPNP");
      /*191 */ CI.s_ukMskPNP     = GetSchemaColumnIndex("s_ukMskPNP");
      /*192 */ CI.skiz_ukKC      = GetSchemaColumnIndex("skiz_ukKC");
      /*193 */ CI.skiz_ukKCR     = GetSchemaColumnIndex("skiz_ukKCR");
      /*194 */ CI.skiz_ukRbt1    = GetSchemaColumnIndex("skiz_ukRbt1");

lastFaktExCI = CI.skiz_ukRbt1; // !!!!!! 

   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()

}
