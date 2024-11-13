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
using Vektor.DataLayer.DS_Reports;
using System.Windows.Forms;
#endif

public sealed class RtransDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static RtransDao instance;

   private RtransDao(XSqlConnection conn, string dbName) : base(dbName, Rtrans.recordName, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static RtransDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new RtransDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableRtrans

   public static uint TableVersionStatic { get { return 12; } }

   public override uint TableVersion { get { return TableVersionStatic; } }

   public static string Create_table_definition()
   {
      return (
         /* 00 */  "recID        int(10)      unsigned NOT NULL auto_increment,\n" +
         /* 01 */  "t_parentID   int(10)      unsigned NOT NULL               ,\n" +
         /* 02 */  "t_dokNum     int(10)      unsigned NOT NULL               ,\n" +
         /* 03 */  "t_serial     smallint(5)  unsigned NOT NULL               ,\n" +
         /* 04 */  "t_skladDate  date                  NOT NULL default '0001-01-01',\n" +
         /* 05 */  "t_tt         char(3)               NOT NULL default ''    ,\n" +
         /* 06 */  "t_ttNum      int(10)      unsigned NOT NULL               ,\n" +
         /* 07 */  "t_ttSort     tinyint(4)            NOT NULL default '0'   ,\n" +
         /* 08 */  "t_artiklCD   varchar(32)           NOT NULL default ''    ,\n" +
         /* 09 */  "t_skladCD    varchar( 6)           NOT NULL default ''    ,\n" +
         /* 00 */  "t_artiklName varchar(80)           NOT NULL default ''    ,\n" +
         /* 11 */  "t_kupdob_cd  int(6)       unsigned NOT NULL default '0'   ,\n" +
         /* 12 */  "t_jedMj      varchar(12)           NOT NULL default ''    ,\n" +
         /* 13 */  "t_konto      varchar(16)           NOT NULL default ''    ,\n" +
         /* 14 */  "t_kol        decimal(12,4)         NOT NULL default '0.00',\n" +
         /* 15 */  "t_cij        decimal(16,8)         NOT NULL default '0.00',\n" +
         /* 16 */  "t_pdvSt      decimal( 5,2)         NOT NULL default '0.00',\n" +
         /* 17 */  "t_rbt1St     decimal( 7,4)         NOT NULL default '0.00',\n" +
         /* 18 */  "t_rbt2St     decimal( 5,2)         NOT NULL default '0.00',\n" +
         /* 19 */  "t_wanted     decimal(12,4)         NOT NULL default '0.00',\n" +
         /* 10 */  "t_doCijMal   decimal(12,4)         NOT NULL default '0.00',\n" +
         /* 21 */  "t_noCijMal   decimal(12,4)         NOT NULL default '0.00',\n" +
         /* 22 */  "t_twinID     int(10)      unsigned NOT NULL               ,\n" +
         /* 23 */  "t_pdvKolTip  tinyint(1)   unsigned NOT NULL default  0    ,\n" +
         /* 24 */  "t_ztr        decimal(12,4)         NOT NULL default '0.00',\n" +
         /* 25 */  "t_kol2       decimal(12,4)         NOT NULL default '0.00',\n" +
         /* 26 */  "t_mCalcKind  tinyint(1)   unsigned NOT NULL default  0    ,\n" +
         /* 27 */  "t_mtros_cd   int(6)       unsigned NOT NULL default '0'   ,\n" +
         /* 28 */  "t_isIrmUslug tinyint(1)   unsigned NOT NULL default  0    ,\n" +
         /* 29 */  "t_ppmvOsn    decimal(12,4)         NOT NULL default '0.00',\n" +
         /* 30 */  "t_ppmvSt1i2  decimal( 5,2)         NOT NULL default '0.00',\n" +
         /* 31 */  "t_pnpSt      decimal( 5,2)         NOT NULL default '0.00',\n" +
         /* 32 */  "t_serlot     varchar(32)           NOT NULL default ''    ,\n" +

          "PRIMARY KEY                   (recID)                                                   ,\n" +
          /*"UNIQUE*/" KEY BY_LINKER     (t_parentID, t_serial)                                    ,\n" +
          /*"UNIQUE*/" KEY BY_ARTIKL     (t_artiklCD, t_skladCD, t_skladDate, t_ttSort, t_ttNum, t_serial)\n"

         // TODO15: vot about index BY_SERLOT? Svima ili rucno samo tko treba? 
         );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Rtrans.recordNameArhiva;
      else tableName = Rtrans.recordName;

      switch(catchingVersion)
      {
         case 2: return ("MODIFY COLUMN t_cij        decimal(14,6)         NOT NULL default '0.00';");

         case 3: return ("ADD COLUMN t_mCalcKind  tinyint(1)   unsigned NOT NULL default  0     AFTER t_kol2;\n");

         case 4: return ("MODIFY COLUMN t_cij        decimal(16,6)         NOT NULL default '0.00';");

         case 5: return ("CHANGE COLUMN t_mrzSt t_wanted DECIMAL(12,4)   NOT NULL DEFAULT '0.00',\n" +
                         "ADD    COLUMN t_mtros_cd       INT(6) UNSIGNED NOT NULL DEFAULT 0 AFTER t_mCalcKind ;\n");

         case 6: return ("ADD COLUMN t_isIrmUslug  tinyint(1)   unsigned NOT NULL default 0 AFTER t_mtros_cd;\n");

         case 7: return ("ADD    COLUMN t_ppmvOsn    decimal(12,4)         NOT NULL default '0.00' AFTER t_isIrmUslug,\n" +
                         "ADD    COLUMN t_ppmvSt1i2  decimal( 5,2)         NOT NULL default '0.00' AFTER t_ppmvOsn   ;\n");

         case 8: return ("ADD    COLUMN t_pnpSt      decimal( 5,2)         NOT NULL default '0.00' AFTER t_ppmvSt1i2 ;\n");

         case 9: return ("MODIFY COLUMN t_rbt1St     decimal(7,4)          NOT NULL default '0.00';");

         case 10: return ("ADD    COLUMN t_serlot     varchar(32)          NOT NULL default '' AFTER t_pnpSt ;\n");

         case 11: return ("MODIFY COLUMN t_cij        decimal(16,8)        NOT NULL default '0.00';");
         case 12: return ("MODIFY COLUMN t_ttSort     tinyint(4)           NOT NULL default '0'   ;");

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableRtrans

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool dummy)
   {
      string preffix;
      Rtrans rtrans = (Rtrans)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.ID_Only ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_recID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {

         /* 01 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_parentID, TheSchemaTable.Rows[CI.t_parentID]);
         /* 02 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_dokNum, TheSchemaTable.Rows[CI.t_dokNum]);
         /* 03 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_serial, TheSchemaTable.Rows[CI.t_serial]);
         /* 04 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_skladDate, TheSchemaTable.Rows[CI.t_skladDate]);
         /* 05 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_TT, TheSchemaTable.Rows[CI.t_tt]);
         /* 06 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_ttNum, TheSchemaTable.Rows[CI.t_ttNum]);
         /* 07 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_ttSort, TheSchemaTable.Rows[CI.t_ttSort]);
         /* 08 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_artiklCD, TheSchemaTable.Rows[CI.t_artiklCD]);
         /* 09 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_skladCD, TheSchemaTable.Rows[CI.t_skladCD]);
         /* 00 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_artiklName, TheSchemaTable.Rows[CI.t_artiklName]);
         /* 11 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_kupdobCD, TheSchemaTable.Rows[CI.t_kupdobCD]);
         /* 12 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_jedMj, TheSchemaTable.Rows[CI.t_jedMj]);
         /* 13 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_konto, TheSchemaTable.Rows[CI.t_konto]);
         /* 14 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_kol, TheSchemaTable.Rows[CI.t_kol]);
         /* 15 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_cij, TheSchemaTable.Rows[CI.t_cij]);
         /* 16 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_pdvSt, TheSchemaTable.Rows[CI.t_pdvSt]);
         /* 17 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_rbt1St, TheSchemaTable.Rows[CI.t_rbt1St]);
         /* 18 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_rbt2St, TheSchemaTable.Rows[CI.t_rbt2St]);
         /* 19 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_wanted, TheSchemaTable.Rows[CI.t_wanted]);
         /* 10 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_doCijMal, TheSchemaTable.Rows[CI.t_doCijMal]);
         /* 21 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_noCijMal, TheSchemaTable.Rows[CI.t_noCijMal]);
         /* 22 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_twinID, TheSchemaTable.Rows[CI.t_twinID]);
         /* 23 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_pdvColTip, TheSchemaTable.Rows[CI.t_pdvKolTip]);
         /* 24 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_ztr, TheSchemaTable.Rows[CI.t_ztr]);
         /* 25 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_kol2, TheSchemaTable.Rows[CI.t_kol2]);
         /* 26 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_mCalcKind, TheSchemaTable.Rows[CI.t_mCalcKind]);
         /* 27 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_mtrosCD, TheSchemaTable.Rows[CI.t_mtrosCD]);
         /* 28 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_isIrmUsluga, TheSchemaTable.Rows[CI.t_isIrmUslug]);
         /* 29 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_ppmvOsn, TheSchemaTable.Rows[CI.t_ppmvOsn]);
         /* 30 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_ppmvSt1i2, TheSchemaTable.Rows[CI.t_ppmvSt1i2]);
         /* 31 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_pnpSt, TheSchemaTable.Rows[CI.t_pnpSt]);
         /* 32 */
         VvSQL.CreateCommandParameter(cmd, preffix, rtrans.T_serlot, TheSchemaTable.Rows[CI.t_serlot]);

      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva /* dummy */)
   {
      RtransStruct rdrData = new RtransStruct();

      rdrData._recID = reader.GetUInt32(CI.recID);

      //rdrData._addTS = reader.GetDateTime(1);
      //rdrData._modTS = reader.GetDateTime(2);
      //rdrData._addUID = reader.GetString(3);
      //rdrData._modUID = reader.GetString(4);


      /* 01 */
      rdrData._t_parentID = reader.GetUInt32(CI.t_parentID);
      /* 02 */
      rdrData._t_dokNum = reader.GetUInt32(CI.t_dokNum);
      /* 03 */
      rdrData._t_serial = reader.GetUInt16(CI.t_serial);
      /* 04 */
      rdrData._t_skladDate = reader.GetDateTime(CI.t_skladDate);
      /* 05 */
      rdrData._t_tt = reader.GetString(CI.t_tt);
      /* 06 */
      rdrData._t_ttNum = reader.GetUInt32(CI.t_ttNum);
      /* 07 */
      rdrData._t_ttSort = reader.GetInt16(CI.t_ttSort);
      /* 08 */
      rdrData._t_artiklCD = reader.GetString(CI.t_artiklCD);
      /* 09 */
      rdrData._t_skladCD = reader.GetString(CI.t_skladCD);
      /* 00 */
      rdrData._t_artiklName = reader.GetString(CI.t_artiklName);
      /* 11 */
      rdrData._t_kupdob_cd = reader.GetUInt32(CI.t_kupdobCD);
      /* 12 */
      rdrData._t_jedMj = reader.GetString(CI.t_jedMj);
      /* 13 */
      rdrData._t_konto = reader.GetString(CI.t_konto);
      /* 14 */
      rdrData._t_kol = reader.GetDecimal(CI.t_kol);
      /* 15 */
      rdrData._t_cij = reader.GetDecimal(CI.t_cij);
      /* 16 */
      rdrData._t_pdvSt = reader.GetDecimal(CI.t_pdvSt);
      /* 17 */
      rdrData._t_rbt1St = reader.GetDecimal(CI.t_rbt1St);
      /* 18 */
      rdrData._t_rbt2St = reader.GetDecimal(CI.t_rbt2St);
      /* 19 */
      rdrData._t_wanted = reader.GetDecimal(CI.t_wanted);
      /* 10 */
      rdrData._t_doCijMal = reader.GetDecimal(CI.t_doCijMal);
      /* 21 */
      rdrData._t_noCijMal = reader.GetDecimal(CI.t_noCijMal);
      /* 22 */
      rdrData._t_twinID = reader.GetUInt32(CI.t_twinID);
      /* 23 */
      rdrData._t_pdvKolTip = reader.GetUInt16(CI.t_pdvKolTip);
      /* 24 */
      rdrData._t_ztr = reader.GetDecimal(CI.t_ztr);
      /* 25 */
      rdrData._t_kol2 = reader.GetDecimal(CI.t_kol2);
      /* 26 */
      rdrData._t_mCalcKind = reader.GetUInt16(CI.t_mCalcKind);
      /* 27 */
      rdrData._t_mtros_cd = reader.GetUInt32(CI.t_mtrosCD);
      /* 28 */
      rdrData._t_isIrmUslug = reader.GetBoolean(CI.t_isIrmUslug);
      /* 29 */
      rdrData._t_ppmvOsn = reader.GetDecimal(CI.t_ppmvOsn);
      /* 30 */
      rdrData._t_ppmvSt1i2 = reader.GetDecimal(CI.t_ppmvSt1i2);
      /* 31 */
      rdrData._t_pnpSt = reader.GetDecimal(CI.t_pnpSt);
      /* 32 */
      rdrData._t_serlot = reader.GetString(CI.t_serlot);

      ((Rtrans)vvDataRecord).CurrentData = rdrData;

      // NE !: ((Rtrans)vvDataRecord).CalcTransResults();

      #region Kune Backup Values

      ((Rtrans)vvDataRecord).Tkn_cij = ((Rtrans)vvDataRecord).T_cij;

      #endregion Kune Backup Values
      return;
   }

   //public static int lastNarrowRtransCI = 21; // !!!!!! 
   public static string ColumnListFromNarrowDataReader(/*bool isForReportList,*/ string preffix)
   {
      //_PUSE_ if(isForReportList) return " * ";

      return

         /* 00 */ preffix + "recID, " +
         /* 01 */ preffix + "t_skladDate, " +
         /* 02 */ preffix + "t_tt, " +
         /* 03 */ preffix + "t_ttNum, " +
         /* 04 */ preffix + "t_ttSort, " +
         /* 05 */ preffix + "t_serial, " +
         /* 06 */ preffix + "t_kol, " +
         /* 07 */ preffix + "t_cij, " +
         /* 08 */ preffix + "t_pdvSt, " +
         /* 09 */ preffix + "t_rbt1St, " +
         /* 10 */ preffix + "t_rbt2St, " +
         /* 11 */ preffix + "t_wanted, " +
         /* 12 */ preffix + "t_doCijMal, " +
         /* 13 */ preffix + "t_noCijMal, " +
         /* 14 */ preffix + "t_ztr, " +
         /* 15 */ preffix + "t_kol2, " +
         /* 16 */ preffix + "t_parentID, " +
         /* 16a*/ preffix + "t_twinID, " +    // by Delf 25.05.2015.
                                              /* 17 */ preffix + "t_mCalcKind, " +
         /* 18 */ preffix + "t_isIrmUslug, " +
         /* 19 */ preffix + "t_pdvKolTip, " +
         /* 20 */ preffix + "t_ppmvSt1i2, " + // !!! ne zaboravi zarez na predzadnjega 
                                              /* 21 */ preffix + "t_pnpSt ";

      // !!! PAZI !!! na gore 'lastNarrowRtransCI' kada ovdje nesto dodajes 
   }

   // 25.05.2015: otkrio da 1. 'lastNarrowRtransCI' ne sluzi apsolutno nicemu                          
   //                       2. t_ppmvSt1i2 i t_pnpSt idu u sql recenicu ali ih reader ne iskoristava?! 

   public void FillFromNarrowDataReader(Rtrans rtrans_rec, XSqlDataReader reader)
   {
      RtransStruct rdrData = new RtransStruct();

      int colIdx = 0;

      /* 00 */
      rdrData._recID = reader.GetUInt32(colIdx++);
      /* 01 */
      rdrData._t_skladDate = reader.GetDateTime(colIdx++);
      /* 02 */
      rdrData._t_tt = reader.GetString(colIdx++);
      /* 03 */
      rdrData._t_ttNum = reader.GetUInt32(colIdx++);
      /* 04 */
      rdrData._t_ttSort = reader.GetInt16(colIdx++);
      /* 05 */
      rdrData._t_serial = reader.GetUInt16(colIdx++);
      /* 06 */
      rdrData._t_kol = reader.GetDecimal(colIdx++);
      /* 07 */
      rdrData._t_cij = reader.GetDecimal(colIdx++);
      /* 08 */
      rdrData._t_pdvSt = reader.GetDecimal(colIdx++);
      /* 19 */
      rdrData._t_rbt1St = reader.GetDecimal(colIdx++);
      /* 10 */
      rdrData._t_rbt2St = reader.GetDecimal(colIdx++);
      /* 11 */
      rdrData._t_wanted = reader.GetDecimal(colIdx++);
      /* 12 */
      rdrData._t_doCijMal = reader.GetDecimal(colIdx++);
      /* 13 */
      rdrData._t_noCijMal = reader.GetDecimal(colIdx++);
      /* 14 */
      rdrData._t_ztr = reader.GetDecimal(colIdx++);
      /* 15 */
      rdrData._t_kol2 = reader.GetDecimal(colIdx++);
      /* 16 */
      rdrData._t_parentID = reader.GetUInt32(colIdx++);
      /* 16a*/
      rdrData._t_twinID = reader.GetUInt32(colIdx++);    // by Delf 25.05.2015.
                                                         /* 17 */
      rdrData._t_mCalcKind = reader.GetUInt16(colIdx++);
      /* 18 */
      rdrData._t_isIrmUslug = reader.GetBoolean(colIdx++);
      /* 19 */
      rdrData._t_pdvKolTip = reader.GetUInt16(colIdx++);

      ((Rtrans)rtrans_rec).CurrentData = rdrData;

      //rtrans_rec.CalcTransResults(null);

      return;
   }

   #endregion FillFromDataReader

   #region FtrCI struct & InitializeSchemaColumnIndexes()

   public struct RtransCI
   {
      internal int recID;

      /* 01 */
      internal int t_parentID;
      /* 02 */
      internal int t_dokNum;
      /* 03 */
      internal int t_serial;
      /* 04 */
      internal int t_skladDate;
      /* 05 */
      internal int t_tt;
      /* 06 */
      internal int t_ttNum;
      /* 07 */
      internal int t_ttSort;
      /* 08 */
      internal int t_artiklCD;
      /* 09 */
      internal int t_skladCD;
      /* 00 */
      internal int t_artiklName;
      /* 11 */
      internal int t_kupdobCD;
      /* 02 */
      internal int t_jedMj;
      /* 03 */
      internal int t_konto;
      /* 14 */
      internal int t_kol;
      /* 15 */
      internal int t_cij;
      /* 16 */
      internal int t_pdvSt;
      /* 17 */
      internal int t_rbt1St;
      /* 18 */
      internal int t_rbt2St;
      /* 19 */
      internal int t_wanted;
      /* 10 */
      internal int t_doCijMal;
      /* 21 */
      internal int t_noCijMal;
      /* 22 */
      internal int t_twinID;
      /* 23 */
      internal int t_pdvKolTip;
      /* 24 */
      internal int t_ztr;
      /* 25 */
      internal int t_kol2;
      /* 26 */
      internal int t_mCalcKind;
      /* 27 */
      internal int t_mtrosCD;
      /* 28 */
      internal int t_isIrmUslug;
      /* 29 */
      internal int t_ppmvOsn;
      /* 30 */
      internal int t_ppmvSt1i2;
      /* 31 */
      internal int t_pnpSt;
      /* 32 */
      internal int t_serlot;

   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public RtransCI CI;
   public static int lastRtransCI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID = GetSchemaColumnIndex("recID");
      //FtrCI.addTS       = GetSchemaColumnIndex("addTS");
      //FtrCI.modTS       = GetSchemaColumnIndex("modTS");
      //FtrCI.addUID      = GetSchemaColumnIndex("addUID");
      //FtrCI.modUID      = GetSchemaColumnIndex("modUID");

      /* 01 */
      CI.t_parentID = GetSchemaColumnIndex("t_parentID");
      /* 02 */
      CI.t_dokNum = GetSchemaColumnIndex("t_dokNum");
      /* 03 */
      CI.t_serial = GetSchemaColumnIndex("t_serial");
      /* 04 */
      CI.t_skladDate = GetSchemaColumnIndex("t_skladDate");
      /* 05 */
      CI.t_tt = GetSchemaColumnIndex("t_tt");
      /* 06 */
      CI.t_ttNum = GetSchemaColumnIndex("t_ttNum");
      /* 07 */
      CI.t_ttSort = GetSchemaColumnIndex("t_ttSort");
      /* 08 */
      CI.t_artiklCD = GetSchemaColumnIndex("t_artiklCD");
      /* 09 */
      CI.t_skladCD = GetSchemaColumnIndex("t_skladCD");
      /* 00 */
      CI.t_artiklName = GetSchemaColumnIndex("t_artiklName");
      /* 11 */
      CI.t_kupdobCD = GetSchemaColumnIndex("t_kupdob_cd");
      /* 12 */
      CI.t_jedMj = GetSchemaColumnIndex("t_jedMj");
      /* 13 */
      CI.t_konto = GetSchemaColumnIndex("t_konto");
      /* 14 */
      CI.t_kol = GetSchemaColumnIndex("t_kol");
      /* 15 */
      CI.t_cij = GetSchemaColumnIndex("t_cij");
      /* 16 */
      CI.t_pdvSt = GetSchemaColumnIndex("t_pdvSt");
      /* 17 */
      CI.t_rbt1St = GetSchemaColumnIndex("t_rbt1St");
      /* 18 */
      CI.t_rbt2St = GetSchemaColumnIndex("t_rbt2St");
      /* 19 */
      CI.t_wanted = GetSchemaColumnIndex("t_wanted");
      /* 20 */
      CI.t_doCijMal = GetSchemaColumnIndex("t_doCijMal");
      /* 21 */
      CI.t_noCijMal = GetSchemaColumnIndex("t_noCijMal");
      /* 22 */
      CI.t_twinID = GetSchemaColumnIndex("t_twinID");
      /* 22 */
      CI.t_pdvKolTip = GetSchemaColumnIndex("t_pdvKolTip");
      /* 24 */
      CI.t_ztr = GetSchemaColumnIndex("t_ztr");
      /* 25 */
      CI.t_kol2 = GetSchemaColumnIndex("t_kol2");
      /* 26 */
      CI.t_mCalcKind = GetSchemaColumnIndex("t_mCalcKind");
      /* 27 */
      CI.t_mtrosCD = GetSchemaColumnIndex("t_mtros_cd");
      /* 28 */
      CI.t_isIrmUslug = GetSchemaColumnIndex("t_isIrmUslug");
      /* 29 */
      CI.t_ppmvOsn = GetSchemaColumnIndex("t_ppmvOsn");
      /* 30 */
      CI.t_ppmvSt1i2 = GetSchemaColumnIndex("t_ppmvSt1i2");
      /* 31 */
      CI.t_pnpSt = GetSchemaColumnIndex("t_pnpSt");
      /* 32 */
      CI.t_serlot = GetSchemaColumnIndex("t_serlot");

      lastRtransCI = CI.t_serlot; // !!!!!! 
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()

   #region DeleteFromCache

   public bool Should_Delete_Then_Renew_Cache(XSqlConnection conn)
   {
      bool goOn = true;

      if(ZXC.ShouldSupressRenewCache) goOn = false;  // RISK_CheckPrNabDokCij_inProgress || RISK_CheckZPCkol_inProgress. Do it later. 

      // 25.11.2014: ...zamisao: neka se CACHE generira samo n LAN serverima kod RECEIVANJA, kod SENDANJA neka SKY server bude ne generiran. Ako ga neko i zatreba na SKY-u, dici ce se generiranje implicitno
      //if(ZXC.SENDtoSKY_InProgress) goOn = false;

      // 28.09.2017: !!! HUGE NEWS !!! Glede cache-a na SKY serveru, prepoznavanje pokusaja te akcije se mijenja. 
      // Ne, na osnovu varijable ZXC.SENDtoSKY_InProgress nego na osnovi connection host-a                        
      if(conn.DataSource == ZXC.CURR_prjkt_rec.SkySrvrHostDecrypted) goOn = false;

      // 24.06.2015: 
      if(ZXC.IsTEXTHOshop && ZXC.RECEIVEfromSKY_InProgress) goOn = false;  // !!! 24.06.2015: za ono SENDanje 1212 ZPCa pa da kad poslovnice RECEIVEaju bude brze ... u oparu sa 'VvForm_FormLoad_ExecuteSynchronisation_SEND_then_RECEIVE' RISK_NewCache! 

      return (goOn);
   }

   public override bool Delete_Then_Renew_Cache_FromThisRtrans(XSqlConnection conn, VvDataRecord vvDataRecord, VvSQL.DB_RW_ActionType actionType)
   {
#if DEBUG
      CacheDebugList = new List<ZXC.CdAndName_CommonStruct>();
      ZXC.DebugCount = 0;
#endif
      if(!Should_Delete_Then_Renew_Cache(conn))
         return (true);

      ZXC.RtransDao.InitRecIDinfoList(/*false, (Rtrans)vvDataRecord*/);

      return Delete_Then_Renew_Cache_FromThisRtrans_JOB(conn, vvDataRecord, actionType, 0);
   }

   private static string fromThisRtrans_TtAndTtNum;
   public static string FromThisRtrans_TtAndTtNum
   {
      get { return RtransDao.fromThisRtrans_TtAndTtNum != null ? RtransDao.fromThisRtrans_TtAndTtNum : "isNull"; }
      set { RtransDao.fromThisRtrans_TtAndTtNum = value; }
   }

   public /*override*/ bool Delete_Then_Renew_Cache_FromThisRtrans_JOB(XSqlConnection conn, VvDataRecord vvDataRecord, VvSQL.DB_RW_ActionType actionType, uint thisRecursiveLevel)
   {
      if(!Should_Delete_Then_Renew_Cache(conn))
      {
         return true;
      }

      ZXC.DebugCount++;

      // tu bi trebao ALTER TABLE artstat, da ima informacija koji je rtrans inicijalno izazvao ovu lancanu reakciju za buduce debugiranje zas se digo new prnabcij 
      Rtrans rtrans_rec = ((Rtrans)vvDataRecord).MakeDeepCopy(); // ovo 'MakeDeepCopy' tek 14.12.2011 

      // 16.10.2015: 4 debug purposes TH-u stavljamo                                                     
      //             TtAndTtNumAndSerial rtransa koji je inicirao Delete_Then_Renew_Cache_FromThisRtrans 
      //             tj. podmecemo u artstat-ovu OrgPakJM verijablu                                      
      if(thisRecursiveLevel.IsZero())
      {
         FromThisRtrans_TtAndTtNum = rtrans_rec.TtAndTtNum;
      }

      // 10.07.2015: TEXTHO nulti ZPC - skip! (npr. 140000) 
      // 12.07.2015: ipak NE!                               
      //if(rtrans_rec.IsNultiZPC)
      //{
      //   return true;
      //}

      bool success = true;
      int nora = -1; // number of rows affected 

      ZXC.sqlErrNo = 0;

      // 08.09.2011: Ali kada u ispravi zamijenis artikl onda stari redak dobije actionType.DEL a u current data ti je sje'an artiklCD 
      bool delrecMarkedWithWrongArtiklCD = rtrans_rec.IsThisRtrans_DelrecMarkedWithWrongArtiklCD(actionType);

      #region Check If Ever in minus (06.12.2011)

      ArtStat artstat_rec = ArtiklDao.GetArtiklStatus(conn, rtrans_rec);

      // 06.12.2011: ako je ikada u minusu, neka renew-a cjeli cache za taj artikl! 
      //if(                                         artstat_rec != null && artstat_rec.FrsMinTt.NotEmpty() && artstat_rec.FrsMinTtNum.NotZero())
      if(ZXC.RISK_NewCache_InProgress == false && artstat_rec != null && artstat_rec.FrsMinTt.NotEmpty() && artstat_rec.FrsMinTtNum.NotZero())
      {
         // 08.07.2015: ciau, mama 
         if(!ZXC.IsTEXTHOany) // well, ovo bi trebalo RADIKALNO ubrzati ovu op kod sejvanja 
         {
            // 13.11.2015: UBIO ZA SVE!!! 
            //rtrans_rec.T_skladDate = DateTime.MinValue;
         }
      }

      #endregion Check If Ever in minus

      using(XSqlCommand cmd = VvSQL.DELREC_FromCacheCommand(conn, rtrans_rec, actionType))
      {
         try { nora = cmd.ExecuteNonQuery(); }
         catch(XSqlException ex) { success = false; ZXC.sqlErrNo = ex.Number; VvSQL.ReportSqlError("DELREC_FromCache " + vvDataRecord.VirtualRecordName, ex, System.Windows.Forms.MessageBoxButtons.OK); }
         catch(Exception ex)     { success = false; System.Windows.Forms.MessageBox.Show(ex.Message); }
      } //using

      // *=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*= 
      // *=*=*=*=*= RENEW CACHE - Start =*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*= 
      // *=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*= 

      string artiklCD, skladCD;

      bool isDELrtransActionFromRWTdocument =

         actionType == VvSQL.DB_RW_ActionType.DEL &&

         ((rtrans_rec.T_BCKPartiklCD.NotEmpty() && rtrans_rec.T_BCKPartiklCD != rtrans_rec.T_artiklCD) ||
          (rtrans_rec.T_BCKPskladCD .NotEmpty() && rtrans_rec.T_BCKPskladCD  != rtrans_rec.T_skladCD   ));

      // 09.05.2016: !!! BIG NEWS !!! ... ali je ovo kurac i ne radi! Ode u beskonacno rebuildanje cache-a za 36 rtransa beskonacno 
    //if(actionType == VvSQL.DB_RW_ActionType.RWT || delrecMarkedWithWrongArtiklCD   ) 
      if(actionType == VvSQL.DB_RW_ActionType.RWT || isDELrtransActionFromRWTdocument)
      {
         artiklCD = rtrans_rec.T_BCKPartiklCD;
         skladCD  = rtrans_rec.T_BCKPskladCD;
      }
      else
      {
         artiklCD = rtrans_rec.T_artiklCD;
         skladCD  = rtrans_rec.T_skladCD;
      }

      // RIDnews:                                                        
      //            ArtiklDao.SetArtiklStatus(conn, artiklCD, skladCD); 
      artstat_rec = ArtiklDao.SetArtiklStatus(conn, artiklCD, skladCD);

      // RIDnews:                                                          ___prNabCij_fromIzlazCache___ to be used as RtrUlazCijNBC for MSU, MMU, KUL, PUK, VMU, MVU 
      //if(rtrans_rec.TtInfo.HasTwinTT) ZXC.RecIDinfoDict[rtrans_rec.T_twinID] = artstat_rec.PrNabCij; // vidi malo, Delowsky, kak je ovo slick                         

      // *=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*= 
      // *=*=*=*=*= RENWE CACHE - End =*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*= 
      // *=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*= 

      #region DependentRtransesOnOtherSklad (Chain Reaction)

      List<Rtrans> dependent_RtransList;

      #region TwinTT pass (Madjuskladisnice)

      if(thisRecursiveLevel.IsZero() && rtrans_rec.T_TT.NotEmpty()) // a ako je Empty, zanci da smo tu dosli ne sas nekog konkretnog rtrans-a, vec iz nekog 'regeneriraj cache za artiklCD i skladCD - KOMPLETNO' 
      {
         dependent_RtransList = GetDependentTwinTtRtransesOnOtherSklad(conn, rtrans_rec);

         if(dependent_RtransList != null) // dakle, IMA ovisnih knjizenja po drugim skladistima 
         {
            // ZXC.BeSilentOnNotFoundError = true;

            // REKURZIJA 
            //recursiveLevel++;
            foreach(Rtrans dependentTwinTT_Rtrans_rec in dependent_RtransList)
            {
               Delete_Then_Renew_Cache_FromThisRtrans_JOB(conn, dependentTwinTT_Rtrans_rec, /*actionType*/ VvSQL.DB_RW_ActionType.UTIL /* actionType da bude bilo sta osim RWT jeer u rekurziju ne dolaze backup nego currentData */ , thisRecursiveLevel + 1);
            }

            // ZXC.BeSilentOnNotFoundError = false;
         }
      }

      #endregion TwinTT pass (Madjuskladisnice)

      #region SplitTT pass (Proizvodnja)

      // by Delf 30.04.15 - thisRecursiveLevel.IsZero() ispred svega! ......

      if(thisRecursiveLevel.IsZero() && rtrans_rec.T_TT.NotEmpty()) // a ako je Empty, zanci da smo tu dosli ne sas nekog konkretnog rtrans-a, vec iz nekog 'regeneriraj cache za artiklCD i skladCD - KOMPLETNO' 
      {
         // PUL pass 
         dependent_RtransList = GetDependentSplitTtRtransesOnProizvDocuments(conn, rtrans_rec, Faktur.TT_PUL, Faktur.TT_PIZ);

         if(dependent_RtransList != null) // dakle, IMA ovisnih knjizenja po drugim skladistima 
         {
            // REKURZIJA 
            //recursiveLevel++;
            foreach(Rtrans dependentSplitTT_Rtrans_rec in dependent_RtransList)
            {
               Delete_Then_Renew_Cache_FromThisRtrans_JOB(conn, dependentSplitTT_Rtrans_rec, /*actionType*/ VvSQL.DB_RW_ActionType.UTIL /* actionType da bude bilo sta osim RWT jeer u rekurziju ne dolaze backup nego currentData */ , thisRecursiveLevel + 1);
            }
         }

         // PUX pass 
         dependent_RtransList = GetDependentSplitTtRtransesOnProizvDocuments(conn, rtrans_rec, Faktur.TT_PUX, Faktur.TT_PIX);

         if(dependent_RtransList != null) // dakle, IMA ovisnih knjizenja po drugim skladistima 
         {
            // REKURZIJA 
            //recursiveLevel++;
            foreach(Rtrans dependentSplitTT_Rtrans_rec in dependent_RtransList)
            {
               Delete_Then_Renew_Cache_FromThisRtrans_JOB(conn, dependentSplitTT_Rtrans_rec, /*actionType*/ VvSQL.DB_RW_ActionType.UTIL /* actionType da bude bilo sta osim RWT jeer u rekurziju ne dolaze backup nego currentData */ , thisRecursiveLevel + 1);
            }
         }

         // TRM pass 
         dependent_RtransList = GetDependentSplitTtRtransesOnProizvDocuments(conn, rtrans_rec, Faktur.TT_TRM, Faktur.TT_TRI);

         if(dependent_RtransList != null) // dakle, IMA ovisnih knjizenja po drugim skladistima 
         {
            // REKURZIJA 
            //recursiveLevel++;
            foreach(Rtrans dependentSplitTT_Rtrans_rec in dependent_RtransList)
            {
               Delete_Then_Renew_Cache_FromThisRtrans_JOB(conn, dependentSplitTT_Rtrans_rec, /*actionType*/ VvSQL.DB_RW_ActionType.UTIL /* actionType da bude bilo sta osim RWT jeer u rekurziju ne dolaze backup nego currentData */ , thisRecursiveLevel + 1);
            }
         }

         // MOU pass 
         dependent_RtransList = GetDependentSplitTtRtransesOnProizvDocuments(conn, rtrans_rec, Faktur.TT_MOU, Faktur.TT_MOI);

         if(dependent_RtransList != null) // dakle, IMA ovisnih knjizenja po drugim skladistima 
         {
            // REKURZIJA 
            //recursiveLevel++;
            foreach(Rtrans dependentSplitTT_Rtrans_rec in dependent_RtransList)
            {
               Delete_Then_Renew_Cache_FromThisRtrans_JOB(conn, dependentSplitTT_Rtrans_rec, /*actionType*/ VvSQL.DB_RW_ActionType.UTIL /* actionType da bude bilo sta osim RWT jeer u rekurziju ne dolaze backup nego currentData */ , thisRecursiveLevel + 1);
            }
         }

      }

      #endregion SplitTT pass (Proizvodnja)

      #endregion DependentRtransesOnOtherSklad

      #region SkladCD is changed on RWT rtrans action

      // 04.05.2016: !!! BIG NEWS !!! preselio u VvDaoBase oko linije 804 
      //if(actionType == VvSQL.DB_RW_ActionType.RWT && rtrans_rec.IsSkladCdChanged_OnRwtAction           == true) 
      //{
      //   Delete_Then_Renew_Cache_FromThisRtrans_JOB(conn, rtrans_rec, VvSQL.DB_RW_ActionType.UTIL, thisRecursiveLevel + 1);
      //}

      #endregion SkladCD is changed on RWT rtrans action

      return (success);
   }

   private List<Rtrans> GetDependentSplitTtRtransesOnProizvDocuments(XSqlConnection conn, Rtrans fromRtrans_rec, string pulazTT, string pizlazTT)
   {
      #region Get PIZX list

      List<Rtrans> rtransList = new List<Rtrans>();
      Rtrans rtrans_rec;

      // vvDelf - 16.06.2015
      // provjera da li je prazan red.
      // Sad više ne pravi problem i kad nema ove provjere jer je ona druga proizvodnja ispravljana pa se nešto popravilo, ali svejedno

      if(fromRtrans_rec.T_artiklCD.IsEmpty()) return null;

      using(XSqlCommand cmd = VvSQL.GetThisAndRemainingRtransesForTT_Command(conn, pizlazTT, fromRtrans_rec))
      {
         using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult /*| CommandBehavior.SingleRow*/))
         {
            if(reader.HasRows == false) return null;

            while(reader.HasRows && reader.Read())
            {
               rtrans_rec = new Rtrans();
               ZXC.RtransDao.FillFromDataReader(rtrans_rec, reader, false);
               rtrans_rec.CalcTransResults(null);
               rtransList.Add(rtrans_rec);
            }

            reader.Close();

         } // using(XSqlDataReader reader 

      } // using(XSqlCommand cmd 

      #endregion Get PIZX list

      #region Get PULX list

      var parentIDs = rtransList.Select(rtr => rtr.T_parentID).Distinct();

      rtransList = new List<Rtrans>(); // one single document PULs 
      List<Rtrans> allPULsRtransList = new List<Rtrans>(); // cumulative documents PULs 

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt], "theTT", pulazTT, " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_parentID], "theParentID", 0 /* ! dummy */, " = "));

      foreach(uint parentID in parentIDs)
      {
         filterMembers.RemoveAt(1);
         filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_parentID], "theParentID", parentID, " = "));

         rtransList.Clear(); // !!! idijote 

         LoadGenericVvDataRecordList(conn, rtransList, filterMembers, "t_serial");

         allPULsRtransList.AddRange(rtransList);
      }

      allPULsRtransList = allPULsRtransList.OrderBy(rtr => rtr.T_ttNum).ThenBy(rtr => rtr.T_serial).ToList();

      #endregion Get PULX list

      var distinctArtikls = allPULsRtransList.Select(rtr => rtr.T_artiklCD).Distinct();

      List<Rtrans> distinct_FirstArtiklEntryOnly_RtransList = new List<Rtrans>(distinctArtikls.Count());

      foreach(string artiklCD in distinctArtikls)
      {
         distinct_FirstArtiklEntryOnly_RtransList.Add(allPULsRtransList.First(rtr => rtr.T_artiklCD == artiklCD));
      }

      return distinct_FirstArtiklEntryOnly_RtransList/*.Distinct().ToList()*/;
   }

   // vvDelf 16.06.2015 - START
   private List<Rtrans> GetDependentTwinTtRtransesOnOtherSklad(XSqlConnection conn, Rtrans rtrans_rec)
   {
      List<string> otherSkladCDsInUse = ArtiklDao.GetDistinctSkladCdListForArtikl(conn, rtrans_rec.T_artiklCD);

      otherSkladCDsInUse.Remove(rtrans_rec.T_skladCD); // ostaju, dakle, samo other skladCDs 

      if(otherSkladCDsInUse.Count().IsZero()) return null; // dakle, NEMA knjizenja po drugim skladistima 

      List<Rtrans> dependentRtransList = new List<Rtrans>(otherSkladCDsInUse.Count());


      // by Delf 24.05.2015: 
      //Rtrans izlaz_rtrans_rec, twin_rtrans_rec; bool OK;
      //foreach(string skladCD in otherSkladCDsInUse)
      //{
      //   izlaz_rtrans_rec = GetFirst_IzlazOrUlaz_OnToOtherSkladCD(conn, skladCD, rtrans_rec);
      //
      //   // 28.01.2012: 
      // //if(izlaz_rtrans_rec != null                                       ) // dakle postoji rtrans za ovaj artikl, za ovo skladiste, od ovog datuma -  koji ide na drugo skladiste 
      //   if(izlaz_rtrans_rec != null && izlaz_rtrans_rec.T_twinID.NotZero()) // dakle postoji rtrans za ovaj artikl, za ovo skladiste, od ovog datuma -  koji ide na drugo skladiste 
      //   {
      //      twin_rtrans_rec = new Rtrans();
      //      OK = twin_rtrans_rec.VvDao.SetMe_Record_byRecID(conn, twin_rtrans_rec, izlaz_rtrans_rec.T_twinID, false);
      //      if(OK) dependentRtransList.Add(twin_rtrans_rec);
      //   }
      //}

      // -----------------------------------

      Rtrans ulaz_rtrans_rec;
      foreach(string skladCD in otherSkladCDsInUse)
      {
         ulaz_rtrans_rec = GetFirst_IzlazOrUlaz_OnToOtherSkladCD(conn, skladCD, rtrans_rec, true);

         if(ulaz_rtrans_rec != null) dependentRtransList.Add(ulaz_rtrans_rec);
         //else ... todo ??? 
      }

      return dependentRtransList.OrderBy(raw => raw.T_skladDate).ThenBy(raw => raw.T_ttNum).ThenBy(raw => raw.T_serial).ToList();
   }
   // vvDelf 16.06.2015 - END

   private Rtrans GetFirst_IzlazOrUlaz_OnToOtherSkladCD(XSqlConnection conn, string otherSkladCD, Rtrans rtrans_rec, bool isULAZinstedOfIZLAZ)
   {
      Rtrans izlaz_rtrans_rec = new Rtrans();

      using(XSqlCommand cmd = VvSQL.GetFirst_IzlazOrUlaz_OnToOtherSkladCD_Command(conn, otherSkladCD, rtrans_rec, isULAZinstedOfIZLAZ))
      {
         if(ExecuteSingleFillFromDataReader(izlaz_rtrans_rec, false, cmd) == false)
         {
            izlaz_rtrans_rec = null;
         }
      }
      return izlaz_rtrans_rec;
   }


   #endregion DeleteFromCache

   #region CheckPrNabDokCij

#if DEBUG
   public static List<ZXC.CdAndName_CommonStruct> CacheDebugList;
#endif

   // Ovaj je onaj visoki, poziva se na otvaranje TabPage-a i SubModulAkcija 'Manual_CheckCache_CheckPrNabCij_CheckZPC' 
   public static bool CheckPrNabDokCij(XSqlConnection conn)
   {

      ZXC.SetStatusText("CheckPrNabDokCij");

#if DEBUG
      if(ZXC.RISK_CheckPrNabDokCij_inProgress == false &&
         ZXC.RISK_CheckZPCkol_inProgress == false)
      {
         CacheDebugList = new List<ZXC.CdAndName_CommonStruct>();
      }
#endif

      Cursor.Current = Cursors.WaitCursor;

      bool allOK = CheckPrNabDokCij(conn, 0, new List<Rtrans>());

      Cursor.Current = Cursors.Default;

#if DEBUG
      CacheDebugList = CacheDebugList.OrderBy(can => can.TheCd + can.TheName + can.TheUint).ToList();
      var cacheGR = CacheDebugList.GroupBy(can => can.TheCd + can.TheName + can.TheUint).ToList();
      cacheGR.RemoveAll(gr => gr.Count() < 2);
#endif

      ZXC.ClearStatusText();

      return allOK;

      // izlaz iz visokoga 
   }

   // Ovaj je onaj niski, poziva se iz visokog i rekurzijom iz samog sebe 
   public static bool CheckPrNabDokCij(XSqlConnection conn, uint thisRecursiveLevel, List<Rtrans> rtransInTroubleList_CUMULATIVE)
   {
      // 11.02.2015: 
      // 11.02.2015: lejter that day: ipak ne za sada. Neka SkyLab revalorizira, ali neka se ipak i njoj digne poruka tako da ako oce izvj RUC-a, da ga ima tocnog (ako odgovori sa YES) 
      //if(ZXC.IsTEXTHOcentrala && ZXC.ThisIsSkyLabProject == false) return true;
      bool allOK = true;
      string message = "";

      // 15.01.2015: 
      if(ZXC.IsTEXTHOshop) return allOK; // Revaloriziramo SAMO na centrali. Posljedica: financ. stanje veleprodajnog BO skadista, a ako se glada na poslovnici nece biti skroz tocno.
                                         // Na centrali bude tocno. Sta sve ovo zapravo znaci, isplivati ce u buducnosti...? 
                                         // 23.01.2015: 
      if(ZXC.IsTEXTHOsky) return allOK;

      if(ZXC.ThisIsSkyLabProject) ZXC.SetMainDbConnDatabaseName(VvSQL.GetDbNameForThisTableName(Faktur.recordName));

      uint descrepancyCount = CountDescrepanciesInPrNabDokCij(conn); // ('ZPC', 'INV', 'POV', 'ZKC', 'PIK', 'KIZ', 'MSI', 'VMI', 'PIZ', 'PIX', 'TRI', 'IMT', 'PPR', 'MVI', 'MOI', 'NOR') 
      // PAZI! Ovdje ides u PUL pass s obzirom na ove                // ('ZPC', 'INV', 'POV', 'ZKC', 'PIK', 'KIZ', 'MSI', 'VMI', 'PIZ', 'PIX', 'TRI', 'IMT', 'PPR', 'MVI', 'MOI', 'NOR') 
      // Podrazumijevas da ako ima 'klasicni' descrepancyCount da postoji i PUL descrepancy, ne provjeravas i ne trazis odgovor na pitanje zasebno za PUL 
      if(descrepancyCount.IsZero())
      {
         // 24.02.2016: dodan if(ZXC.RRD.Dsc_IsSerlotVisible), tako da ipak ode u check PPR/PIP odnosa 
         // 15.12.2017: dodano da i TEXTHO centrali a i meni na Ripley-u da ode u forced 'CheckPrNabDokCij_PULX_Additions()' 
       //if(ZXC.RRD.Dsc_IsSerlotVisible)
       //if(ZXC.RRD.Dsc_IsSerlotVisible || ((ZXC.IsTEXTHOany2 && !ZXC.IsTEXTHOshop))) // well, ili Mtflx ili TEXTHOcentrala/Ripley (to je ovaj (ZXC.IsTEXTHOany2 && !ZXC.IsTEXTHOshop))
         if(ZXC.RRD.Is_Serlot_Active    || ((ZXC.IsTEXTHOany2 && !ZXC.IsTEXTHOshop))) // well, ili Mtflx ili TEXTHOcentrala/Ripley (to je ovaj (ZXC.IsTEXTHOany2 && !ZXC.IsTEXTHOshop))
         {
            CheckPrNabDokCij_PULX_Additions(conn, /*false*/true, ref message); // !!! 
            allOK = message.Length.IsZero();
         }

         return allOK; // break whole checking operation 
      }

      allOK = false;

      if(ZXC.ThisIsSkyLabProject == false)
      {
         DialogResult result =
            MessageBox.Show
            ("Usljed promjene podataka koju su uvjetovali pros. nabavnu cijenu, na nekim dokumentima je ona kriva.\n\nDa li želite automatski revalorizirati cijenu na tim dokumentima?\n\nZa " + descrepancyCount + " stavaka.",
            "Promijenjena Prosječna Nabavna Cijena?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

         if(result != DialogResult.Yes) return allOK;
      }
      else // this IS SkyLabProject 
      {
         ZXC.aim_log("CheckPrNabDokCij thisRecursiveLevel: {0} rtransInTroubleList_CUMULATIVE.Count: {1}", thisRecursiveLevel, rtransInTroubleList_CUMULATIVE.Count);
      }

      Cursor.Current = Cursors.WaitCursor;

      ZXC.RISK_CheckPrNabDokCij_inProgress = true;

      // --- Here we go! --- 

      List<Rtrans> rtransInTroubleList = GetRtransesWithAsEx_HavingDescrepancies_List(conn);

      ZXC.aim_log("[=] Going to change: {0} Rtranses!", rtransInTroubleList.Count.ToString());

      List<Faktur> fakturForRwtrecList = GetFakturForRwtrecList_And_RwtNewValues(conn, rtransInTroubleList, false, RtransServiceKind.CheckPrNabCij);

      ZXC.aim_log("[=] Did change: {0} Fakturs!", fakturForRwtrecList.Count.ToString());

      message = StringOfFakturList(fakturForRwtrecList, false);

      #region PULX Additions (PUL-PIZ, PUX-PIX, TRM-TRI, PIP-PPR)

#if preselio_dole_zasebnu_funkciju
      //  - vvDelf 12.06.2015 - ubijena prva linija RwtNewValuesFor_PulxRtranses_HavingDescrepancies()

      // PUL-PIZ pass 
      rtransInTroubleList = GetPulRtranses_HavingDescrepancies_List(conn, Faktur.TT_PUL, Faktur.TT_PIZ, false);

      if(rtransInTroubleList != null)
      {
         // RwtNewValuesFor_PulxRtranses_HavingDescrepancies(conn, rtransInTroubleList);

         // 10.11.2014: dodano tek sada. Bijo BUG! 
         fakturForRwtrecList = GetFakturForRwtrecList_And_RwtNewValues(conn, rtransInTroubleList, true, RtransServiceKind.CheckPrNabCij);
         ZXC.aim_log("[PUL/PIZ] Faktur list: {0}", StringOfFakturList(fakturForRwtrecList));
      }

      // PUX-PIX pass 
      rtransInTroubleList = GetPulRtranses_HavingDescrepancies_List(conn, Faktur.TT_PUX, Faktur.TT_PIX, false);

      if(rtransInTroubleList != null)
      {
         // RwtNewValuesFor_PulxRtranses_HavingDescrepancies(conn, rtransInTroubleList);

         // 10.11.2014: dodano tek sada. Bijo BUG! 
         fakturForRwtrecList = GetFakturForRwtrecList_And_RwtNewValues(conn, rtransInTroubleList, true, RtransServiceKind.CheckPrNabCij);
         ZXC.aim_log("[PUX/PIX] Faktur list: {0}", StringOfFakturList(fakturForRwtrecList));
      }

      //// PUM pass 
      //rtransInTroubleList = GetPulRtranses_HavingDescrepancies_List(conn, Faktur.TT_PUM, Faktur.TT_PIM);

      //if(rtransInTroubleList != null)
      //{
      //   RwtNewValuesFor_PulxRtranses_HavingDescrepancies(conn, rtransInTroubleList);
      //}

      // TRM-TRI pass 
      rtransInTroubleList = GetPulRtranses_HavingDescrepancies_List(conn, Faktur.TT_TRM, Faktur.TT_TRI, true); // !!! ovaj radi GRUPIRANO po Artikl.grupa1CD !!! 

      if(rtransInTroubleList != null)
      {
         // RwtNewValuesFor_PulxRtranses_HavingDescrepancies(conn, rtransInTroubleList);

         // 10.11.2014: dodano tek sada. Bijo BUG! 
         fakturForRwtrecList = GetFakturForRwtrecList_And_RwtNewValues(conn, rtransInTroubleList, true, RtransServiceKind.CheckPrNabCij);
         ZXC.aim_log("[TRM/TRI] Faktur list: {0}", StringOfFakturList(fakturForRwtrecList));
      }

      // PIP-PPR pass 
      rtransInTroubleList = GetPIP_Rtranses_HavingDescrepancies_List(conn, Faktur.TT_PIP, Faktur.TT_PPR, false);

      if(rtransInTroubleList != null)
      {
         // RwtNewValuesFor_PulxRtranses_HavingDescrepancies(conn, rtransInTroubleList);

         // 10.11.2014: dodano tek sada. Bijo BUG! 
         fakturForRwtrecList = GetFakturForRwtrecList_And_RwtNewValues(conn, rtransInTroubleList, true, RtransServiceKind.CheckPrNabCij);
         ZXC.aim_log("[PIP/PPR] Faktur list: {0}", StringOfFakturList(fakturForRwtrecList));

         // !!!
         message += StringOfFakturList(fakturForRwtrecList);
      }
#endif

      // !!! ___________________________________________________ //     
      CheckPrNabDokCij_PULX_Additions(conn, false, ref message); // !!! 
      // !!! ___________________________________________________ //     

      #endregion PULX Additions

      Cursor.Current = Cursors.Default;

      // 26.02.2013: Go RECURSIVE in case of chain reaction needs 
      // Onaj gore 'if(descrepancyCount.IsZero()) return true;' prekida rekurziju 
      if(rtransInTroubleList != null) rtransInTroubleList_CUMULATIVE.AddRange(rtransInTroubleList);
      CheckPrNabDokCij(conn, thisRecursiveLevel + 1, rtransInTroubleList_CUMULATIVE); // REKURZIJA 

      if(ZXC.TheVvForm.TheVvUC is VvRecordUC) ZXC.TheVvForm.WhenRecordInDBHasChangedAction(); // ReRead_OnClick, Refresh record 

      /*if(ZXC.ThisIsSkyLabProject == false)*/
      ZXC.aim_emsg(MessageBoxIcon.Information, "Revalorizacija je gotova. Dokumenti:\n\n{0}", message);

      ZXC.RISK_CheckPrNabDokCij_inProgress = false;

      // OVO IDE U REMARK - vvDelf 16.06.2015
      /* !!! qweqwe!!! */ // if(thisRecursiveLevel.IsZero()) DeleteAndRenewCache_PreviouslySupressed(conn, rtransInTroubleList_CUMULATIVE);
                          // if(thisRecursiveLevel.IsZero()) DeleteAndRenewCache_PreviouslySupressed(conn, rtransInTroubleList_CUMULATIVE);

      ZXC.RtransDao.ResetRecIDinfoList();    // vvDelf 17.06.2015

      return allOK;

      // izlaz iz niskoga - rekurzivnoga 

   }

   public static void CheckPrNabDokCij_PULX_Additions(XSqlConnection conn, bool isExplicitCall, ref string message) // isExplicitCall: false if called from CheckPrNabDokCij, true if called as some SubmodulAction 
   {
      List<Rtrans> rtransInTroubleList;
      List<Faktur> fakturForRwtrecList;

      // PUL-PIZ pass 
      rtransInTroubleList = GetPulRtranses_HavingDescrepancies_List(conn, Faktur.TT_PUL, Faktur.TT_PIZ, false);

      if(rtransInTroubleList != null)
      {
         // RwtNewValuesFor_PulxRtranses_HavingDescrepancies(conn, rtransInTroubleList);

         // 10.11.2014: dodano tek sada. Bijo BUG! 
         fakturForRwtrecList = GetFakturForRwtrecList_And_RwtNewValues(conn, rtransInTroubleList, true, RtransServiceKind.CheckPrNabCij);
         ZXC.aim_log("[PUL/PIZ] Faktur list: {0}", StringOfFakturList(fakturForRwtrecList, true));

         // 28.10.2024: NEW ERA: dodatno popravljamo i CACHe zelenih PULX rtransa @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ 

         ZXC.RISK_CheckPrNabDokCij_inProgress = false; // privremeno, da ne zaustavi ..._JOB 

         foreach(Rtrans rtrans_rec in rtransInTroubleList) ZXC.RtransDao.Delete_Then_Renew_Cache_FromThisRtrans_JOB(conn, rtrans_rec, VvSQL.DB_RW_ActionType.UTIL, 0);

         ZXC.RISK_CheckPrNabDokCij_inProgress = true;

         // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ 

         if(isExplicitCall)
         {
            message += StringOfFakturList(fakturForRwtrecList, true);
            ZXC.aim_emsg(MessageBoxIcon.Information, "Revalorizacija PUL cijene je gotova. Dokumenti:\n\n{0}", message);
         }
      }

      // PUX-PIX pass 
      rtransInTroubleList = GetPulRtranses_HavingDescrepancies_List(conn, Faktur.TT_PUX, Faktur.TT_PIX, false);

      if(rtransInTroubleList != null)
      {
         // RwtNewValuesFor_PulxRtranses_HavingDescrepancies(conn, rtransInTroubleList);

         // 10.11.2014: dodano tek sada. Bijo BUG! 
         fakturForRwtrecList = GetFakturForRwtrecList_And_RwtNewValues(conn, rtransInTroubleList, true, RtransServiceKind.CheckPrNabCij);
         ZXC.aim_log("[PUX/PIX] Faktur list: {0}", StringOfFakturList(fakturForRwtrecList, true));

         // 28.10.2024: NEW ERA: dodatno popravljamo i CACHe zelenih PULX rtransa @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ 

         ZXC.RISK_CheckPrNabDokCij_inProgress = false; // privremeno, da ne zaustavi ..._JOB 

         foreach(Rtrans rtrans_rec in rtransInTroubleList) ZXC.RtransDao.Delete_Then_Renew_Cache_FromThisRtrans_JOB(conn, rtrans_rec, VvSQL.DB_RW_ActionType.UTIL, 0);

         ZXC.RISK_CheckPrNabDokCij_inProgress = true;

         // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ 

         if(isExplicitCall)
         {
            message += StringOfFakturList(fakturForRwtrecList, true);
            ZXC.aim_emsg(MessageBoxIcon.Information, "Revalorizacija PUX cijene je gotova. Dokumenti:\n\n{0}", message);
         }
      }

      //// PUM pass 
      //rtransInTroubleList = GetPulRtranses_HavingDescrepancies_List(conn, Faktur.TT_PUM, Faktur.TT_PIM);

      //if(rtransInTroubleList != null)
      //{
      //   RwtNewValuesFor_PulxRtranses_HavingDescrepancies(conn, rtransInTroubleList);
      //}

      // TRM-TRI pass 
      rtransInTroubleList = GetPulRtranses_HavingDescrepancies_List(conn, Faktur.TT_TRM, Faktur.TT_TRI, true); // !!! ovaj radi GRUPIRANO po Artikl.grupa1CD !!! 

      if(rtransInTroubleList != null)
      {
         // RwtNewValuesFor_PulxRtranses_HavingDescrepancies(conn, rtransInTroubleList);

         // 10.11.2014: dodano tek sada. Bijo BUG! 
         fakturForRwtrecList = GetFakturForRwtrecList_And_RwtNewValues(conn, rtransInTroubleList, true, RtransServiceKind.CheckPrNabCij);
         ZXC.aim_log("[TRM/TRI] Faktur list: {0}", StringOfFakturList(fakturForRwtrecList, true));

         // 28.10.2024: NEW ERA: dodatno popravljamo i CACHe zelenih PULX rtransa @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ 
         // NAMJERNO SUPRESSANA NOVOST ZA TEXTHO jer ne znamo konsekvence ... eventualne razlike ce njima VvJanitor popraviti 
         //ZXC.RISK_CheckPrNabDokCij_inProgress = false; // privremeno, da ne zaustavi ..._JOB 
         //
         //foreach(Rtrans rtrans_rec in rtransInTroubleList) ZXC.RtransDao.Delete_Then_Renew_Cache_FromThisRtrans_JOB(conn, rtrans_rec, VvSQL.DB_RW_ActionType.UTIL, 0);
         //
         //ZXC.RISK_CheckPrNabDokCij_inProgress = true;

         // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ 

         if(isExplicitCall)
         {
            message += StringOfFakturList(fakturForRwtrecList, true);
            ZXC.aim_emsg(MessageBoxIcon.Information, "Revalorizacija TRM cijene je gotova. Dokumenti:\n\n{0}", message);
         }
      }

      // 03.03.2017: bef PIP-PPR pass Check r.T_ppmvOsn & F.ProjektCD 
      if(ZXC.IsRNMnotRNP && ZXC.RRD.Dsc_IsSerlotVisible) // Dakle, samo za Mfx
      {
         List<ZXC.VvUtilDataPackage> theTtAndTtNumList;
         bool hasProblem = Get_RNU_PIP_Rtranses_MissingData(conn, out theTtAndTtNumList);
         if(hasProblem)
         {
            string errMessage = "";
            errMessage = VvForm.GetErrMessageList(theTtAndTtNumList);
            ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.\n\n{1}", ZXC.MySqlCheck_Kind.N_RnuPip_MissingData, errMessage);
         }
      }

      // PIP-PPR pass 
      rtransInTroubleList = GetPIP_Rtranses_HavingDescrepancies_List(conn, Faktur.TT_PIP, Faktur.TT_PPR, false);

      if(rtransInTroubleList != null)
      {
         // RwtNewValuesFor_PulxRtranses_HavingDescrepancies(conn, rtransInTroubleList);

         // 10.11.2014: dodano tek sada. Bijo BUG! 
         fakturForRwtrecList = GetFakturForRwtrecList_And_RwtNewValues(conn, rtransInTroubleList, true, RtransServiceKind.CheckPrNabCij);
         ZXC.aim_log("[PIP/PPR] Faktur list: {0}", StringOfFakturList(fakturForRwtrecList, true));

         // !!! uvijek, bezuvjetno cemo puniti message za PPR-PIP probleme 
         message += StringOfFakturList(fakturForRwtrecList, true);
         if(isExplicitCall) ZXC.aim_emsg(MessageBoxIcon.Information, "Revalorizacija PIP cijene je gotova. Dokumenti:\n\n{0}", message);
      }

      // MOU-MOI pass 
      rtransInTroubleList = GetPulRtranses_HavingDescrepancies_List(conn, Faktur.TT_MOU, Faktur.TT_MOI, false);

      if(rtransInTroubleList != null)
      {
         fakturForRwtrecList = GetFakturForRwtrecList_And_RwtNewValues(conn, rtransInTroubleList, true, RtransServiceKind.CheckPrNabCij);
         ZXC.aim_log("[MOU/MOI] Faktur list: {0}", StringOfFakturList(fakturForRwtrecList, true));

         // 28.10.2024: NEW ERA: dodatno popravljamo i CACHe zelenih PULX rtransa @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ 

         ZXC.RISK_CheckPrNabDokCij_inProgress = false; // privremeno, da ne zaustavi ..._JOB 

         foreach(Rtrans rtrans_rec in rtransInTroubleList) ZXC.RtransDao.Delete_Then_Renew_Cache_FromThisRtrans_JOB(conn, rtrans_rec, VvSQL.DB_RW_ActionType.UTIL, 0);

         ZXC.RISK_CheckPrNabDokCij_inProgress = true;

         // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ 

         if(isExplicitCall)
         {
            message += StringOfFakturList(fakturForRwtrecList, true);
            ZXC.aim_emsg(MessageBoxIcon.Information, "Revalorizacija MOU cijene je gotova. Dokumenti:\n\n{0}", message);
         }
      }
      
   }

   private static void DeleteAndRenewCache_PreviouslySupressed(XSqlConnection conn, List<Rtrans> rtransInTroubleList)
   {
      Cursor.Current = Cursors.WaitCursor;

      ZXC.RtransDao.InitRecIDinfoList();  // vv Delf 26.06.2015 - Ovo treba kad se zove iz CheckZPC - maybe

      var rtrByArtiklPerSkladGRPs = rtransInTroubleList.GroupBy(rtr => rtr.T_skladCD + rtr.T_artiklCD);

      /*if(ZXC.ThisIsSkyLabProject == false)*/
      if(rtransInTroubleList.Count.NotZero())
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Cache je potrebno regenerirati ({0} stavaka). Pričekajte, molim, obradu.", rtransInTroubleList.Count);

      foreach(var rtrGRP in rtrByArtiklPerSkladGRPs)
      {
         Rtrans rtrans_rec = rtrGRP.OrderBy(r => r.T_skladDate).ThenBy(r => r.T_ttSort).ThenBy(r => r.T_ttNum).ThenBy(r => r.T_serial).First();

         // vvDelf - TREBA li ovo imati JOB / 1 rekurzivni level ???
         ZXC.RtransDao.Delete_Then_Renew_Cache_FromThisRtrans_JOB(conn, rtrans_rec, VvSQL.DB_RW_ActionType./*RWT*/UTIL, 1);
      }

      Cursor.Current = Cursors.Default;

   }

   private static string StringOfFakturList(List<Faktur> fakturForRwtrecList, bool isPULXpass)
   {
      string message = ""; int i = 0;

      foreach(Faktur faktur_rec in fakturForRwtrecList)
      {
         message += ++i + (isPULXpass ? "x" : "") + ". " + faktur_rec.TipBr + " / " + faktur_rec.DokDate.ToString(ZXC.VvDateFormat) + ", \n";
      }

      return (message);
   }

   public static bool CheckAndRepare_ZPC_Kol_And_OldMpc(XSqlConnection conn)
   {
      bool allOK = true;

      if(ZXC.IsTEXTHOshop) return allOK;
      if(ZXC.IsTEXTHOsky) return allOK;

      if(ZXC.ThisIsSkyLabProject) ZXC.SetMainDbConnDatabaseName(VvSQL.GetDbNameForThisTableName(Faktur.recordName));

      ZXC.RISK_CheckZPCkol_inProgress = true;

      Cursor.Current = Cursors.WaitCursor;

      ZXC.SetStatusText("CheckAndRepare_ZPC_Kol_And_OldMpc");

      // --- Here we go! --- 

      List<Rtrans> rtransInTroubleList = GetRtransesWithAsEx_Having_ZPC_Descrepancies_List(conn);

      // 01.01.2022: 
      rtransInTroubleList.RemoveAll(rtr => rtr.IsNultiZPC);

      if(rtransInTroubleList == null || rtransInTroubleList.Count.IsZero())
      {
         Cursor.Current = Cursors.Default;
         ZXC.RISK_CheckZPCkol_inProgress = false;
         //ZXC.aim_emsg(MessageBoxIcon.Information, "Svi ZPC-ovi su OK.");
         return allOK;
      }

      allOK = false;

      List<Faktur> fakturForRwtrecList = GetFakturForRwtrecList_And_RwtNewValues(conn, rtransInTroubleList, false, RtransServiceKind.CheckZPCkol);

      // string message=""; int i=0;
      // foreach(Faktur faktur_rec in fakturForRwtrecList)
      // {
      //    message += ++i + ". " + faktur_rec.TipBr + "\n";
      // }

      string message = StringOfFakturList(fakturForRwtrecList, false);

      Cursor.Current = Cursors.Default;

      if(ZXC.ThisIsSkyLabProject == false) if(ZXC.TheVvForm.TheVvUC is VvRecordUC) ZXC.TheVvForm.WhenRecordInDBHasChangedAction(); // ReRead_OnClick, Refresh record 

      /*if(ZXC.ThisIsSkyLabProject == false)*/
      ZXC.aim_emsg(MessageBoxIcon.Information, "Revalorizacija ZPC količina je gotova. Dokumenti:\n\n{0}", message);

      ZXC.RISK_CheckZPCkol_inProgress = false;

      // !!! 
      // 22.06.2015: qukatz stavio i ovo out.
      // 24.06.2015: qukatz vratiJo 
      ZXC.SetStatusText("DeleteAndRenewCache_PreviouslySupressed");
      DeleteAndRenewCache_PreviouslySupressed(conn, rtransInTroubleList);
      ZXC.ClearStatusText();

      ZXC.RtransDao.FailedIzlazTwinsListManager(conn, VvSQL.DB_RW_ActionType.RWT);
      ZXC.RtransDao.ResetRecIDinfoList();

      return allOK;
   }

   public static bool Trim_ZPC(XSqlConnection conn)
   {
      bool allOK = true;

      if(ZXC.IsTEXTHOshop) return allOK;
      if(ZXC.IsTEXTHOsky ) return allOK;

      if(ZXC.ThisIsSkyLabProject) ZXC.SetMainDbConnDatabaseName(VvSQL.GetDbNameForThisTableName(Faktur.recordName));

      ZXC.RISK_CheckZPCkol_inProgress = true; // da ne uvodimo sad novu varijablu 

      Cursor.Current = Cursors.WaitCursor;

      ZXC.SetStatusText("Trim_ZPC");

      // --- Here we go! --- 

      // 29.09.2017: ako prede ponoc 
      DateTime dateDO = VvSQL.GetServer_DateTime_Now(conn);

      if(dateDO.Hour < 20) dateDO = dateDO - ZXC.OneDaySpan;

      List<Rtrans> unusefulZPC_rtransList = RtransDao.Get_UnusefulZPC_RtransList(conn, dateDO /*VvSQL.GetServer_DateTime_Now(conn)*/);

      if(unusefulZPC_rtransList == null || unusefulZPC_rtransList.Count.IsZero())
      {
         Cursor.Current = Cursors.Default;
         ZXC.RISK_CheckZPCkol_inProgress = false;
         //ZXC.aim_emsg(MessageBoxIcon.Information, "Svi ZPC-ovi su OK.");
         return allOK;
      }

      allOK = false;

      List<Faktur> fakturForRwtrecList = GetFakturForRwtrecList_And_RwtNewValues(conn, unusefulZPC_rtransList, false, RtransServiceKind.TrimZPC);

      string message = StringOfFakturList(fakturForRwtrecList, false);

      //string message=""; int i=0;
      //foreach(Faktur faktur_rec in fakturForRwtrecList)
      //{
      //   message += ++i + ". " + faktur_rec.TipBr + "\n";
      //}

      Cursor.Current = Cursors.Default;

      if(ZXC.ThisIsSkyLabProject == false) if(ZXC.TheVvForm.TheVvUC is VvRecordUC) ZXC.TheVvForm.WhenRecordInDBHasChangedAction(); // ReRead_OnClick, Refresh record 

      /*if(ZXC.ThisIsSkyLabProject == false)*/
      ZXC.aim_emsg(MessageBoxIcon.Information, "Trim ZPC operacija je gotova. Dokumenti:\n\n{0}", message);

      ZXC.RISK_CheckZPCkol_inProgress = false;

      ZXC.SetStatusText("DeleteAndRenewCache_PreviouslySupressed");
      DeleteAndRenewCache_PreviouslySupressed(conn, unusefulZPC_rtransList);
      ZXC.ClearStatusText();

      ZXC.RtransDao.FailedIzlazTwinsListManager(conn, VvSQL.DB_RW_ActionType.RWT);
      ZXC.RtransDao.ResetRecIDinfoList();

      return allOK;
   }

   private static List<Rtrans> GetPulRtranses_HavingDescrepancies_List(XSqlConnection conn, string pulxTT, string pizxTT, bool isByGR)
   {
      List<Rtrans> rtransList = new List<Rtrans>();
      Rtrans rtrans_rec;

      decimal correctCij;

      using(XSqlCommand cmd = VvSQL.GetPulRtranses_HavingDescrepancies_Command(conn, pulxTT, pizxTT, isByGR))
      {
         using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult /*| CommandBehavior.SingleRow*/))
         {
            if(reader.HasRows == false) return null;
            while(reader.HasRows && reader.Read())
            {
               rtrans_rec = new Rtrans();
               ZXC.RtransDao.FillFromDataReader(rtrans_rec, reader, false);
               //rtrans_rec.CalcTransResults(null);
               correctCij = reader.GetDecimal(lastRtransCI + 1); // trik, GetPulRtranses_HavingDescrepancies_Command() ti vrati cio Rtrans + jos jedan decimal gdje je 'correctCij' 
               rtrans_rec.TmpDecimal = correctCij; // trik, da gore kod RWTREC-a znas koja je korektna cijena 

               rtransList.Add(rtrans_rec);
            }
            reader.Close();
         } // using(XSqlDataReader reader 
      } // using(XSqlCommand cmd 

      return rtransList;
   }

   private static List<Rtrans> GetPIP_Rtranses_HavingDescrepancies_List(XSqlConnection conn, string pulxTT, string pizxTT, bool isByGR)
   {
      // 17.11.2016: !!! BIG NEWS !!!                   
      // Raskidamo vezu PPR/PIP cijene                  
      // TT_PIP vise nije 'arrayRadNalPUcijTT'          
      // Za nekog drugog u buducnosti treba onda uvesti 
      // novi set U/I TT-ova                            

      // 25.11.2016: vraćamo! ipak 
      //return null;

      // 25.11.2016: vraćamo! ipak 
      //#if PUSE_FUSE
      List<Rtrans> rtransList = new List<Rtrans>();
      Rtrans       rtrans_rec;

      decimal correctCij;

    //using(XSqlCommand cmd = VvSQL.GetPIP_Rtranses_HavingDescrepancies_Command(conn, pulxTT, pizxTT, isByGR))
      using(XSqlCommand cmd = VvSQL.GetPIP_Rtranses_HavingDescrepancies_Command(conn))
      {
         using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult /*| CommandBehavior.SingleRow*/))
         {
            if(reader.HasRows == false) return null;
            while(reader.HasRows && reader.Read())
            {
               rtrans_rec = new Rtrans();
               ZXC.RtransDao.FillFromDataReader(rtrans_rec, reader, false);
               //rtrans_rec.CalcTransResults(null);
               correctCij = reader.GetDecimal(lastRtransCI + 1); // trik, GetPulRtranses_HavingDescrepancies_Command() ti vrati cio Rtrans + jos jedan decimal gdje je 'correctCij' 
               rtrans_rec.TmpDecimal = correctCij; // trik, da gore kod RWTREC-a znas koja je korektna cijena 
               rtransList.Add(rtrans_rec);
            }
            reader.Close();
         } // using(XSqlDataReader reader 
      } // using(XSqlCommand cmd 

      return rtransList;
      //#endif
   }

   internal static bool Get_RNU_PIP_Rtranses_MissingData(XSqlConnection conn, out List<ZXC.VvUtilDataPackage> theTtAndTtNumList)
   {
      bool hasProblem = false;
      theTtAndTtNumList = new List<ZXC.VvUtilDataPackage>();
      ZXC.VvUtilDataPackage theTtAndTtNum;

      using(XSqlCommand cmd = VvSQL.Get_RNU_PIP_Rtranses_MissingData_Command(conn))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult /*| CommandBehavior.SingleRow*/))
            {
               hasProblem = reader.HasRows;

               while(reader.HasRows && reader.Read())
               {
                  theTtAndTtNum = new ZXC.VvUtilDataPackage();
                  Fill_TtAndTtNum_FromDataReader(ref theTtAndTtNum, reader, ZXC.MySqlCheck_Kind.N_RnuPip_MissingData); // 'ref' ... jer je ZXC.VvUtilDataPackage struct a ne class 
                  theTtAndTtNumList.Add(theTtAndTtNum);
               }

               reader.Close();

            } // using(XSqlDataReader reader 
         }

         catch(Exception ex) { ZXC.aim_emsg(MessageBoxIcon.Error, "SqlSomeCheckQuery [{0}] exception!:\n\n{1}", ZXC.MySqlCheck_Kind.N_RnuPip_MissingData, ex.Message); return (hasProblem); }

      } // using(XSqlCommand cmd = VvSQL.SqlSomeCheckQuery_Command(conn, mySqlCheck_Kind))  

      return hasProblem;
   }

   // PUSE 
   //private static void RwtNewValuesFor_PulxRtranses_HavingDescrepancies(XSqlConnection conn, List<Rtrans> pulxRtransInTroubleList)
   //{
   //   uint debugCount = 0;
   //   decimal oldVal;

   //   foreach(Rtrans rtrans_rec in pulxRtransInTroubleList)
   //   {
   //      oldVal = rtrans_rec.T_cij;

   //      rtrans_rec.BeginEdit();

   //      rtrans_rec.T_cij = rtrans_rec.TmpDecimal; // trik, GetPulRtranses_HavingDescrepancies_Command() ti vrati cio Rtrans + jos jedan decimal gdje je 'correctCij' a spremis ga u TmpDecimal 

   //      if(ZXC.AlmostEqual(oldVal, rtrans_rec.T_cij, 0.015M) == false)  // by Delf
   //      {
   //         ZXC.RtransDao.RWTREC(conn, rtrans_rec, false, false);

   //         ZXC.aim_log("_RwtNewValuesFor_PulxRtranses: done {0}. of {1} rtranses [{2}-{3}]",
   //            ++debugCount, pulxRtransInTroubleList.Count, rtrans_rec.T_TT, rtrans_rec.T_ttNum);
   //      }
   //      rtrans_rec.EndEdit();
   //   }
   //}

   public static decimal ConditionalValue(decimal oldValue, decimal newValue, decimal tolerance, ref bool didChange)
   {
      decimal diff = Math.Abs(oldValue - newValue);

      if(diff > tolerance)
      {
         oldValue = newValue;
         didChange = true;
      }

      return (oldValue);
   }

   private enum RtransServiceKind { CheckPrNabCij, CheckZPCkol, TrimZPC }

   private static List<Faktur> GetFakturForRwtrecList_And_RwtNewValues(XSqlConnection conn, List<Rtrans> rtransInDescrepancyList, bool isPULXpass, RtransServiceKind serviceKind)
   {
      List<Faktur> fakturList = new List<Faktur>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);

      uint[] parentIDs = rtransInDescrepancyList.Select(rtr => rtr.T_parentID).Distinct().ToArray();
      bool OK;
      Rtrans rtransInDescrepancy;
      VvDataRecord arhivedFaktur_rec;

      #region inClauseValue

      string inClauseValue = "(";

      bool firstPass = true;
      foreach(uint ID in parentIDs)
      {
         inClauseValue += (firstPass ? "" : ", ") + ID;
         if(firstPass) firstPass = false;
      }

      inClauseValue += ")";

      #endregion inClauseValue

      bool hasPossibleExtender = rtransInDescrepancyList.Any(rtr => rtr.TtInfo.IsExtendableTT);

      filterMembers.Add(new VvSqlFilterMember(hasPossibleExtender ? "L.recID" : "recID", inClauseValue, " IN "));

      LoadGenericVvDataRecordList(conn, fakturList, filterMembers, "", "tt, ttNum ", hasPossibleExtender);

      uint debugCount = 0; string arhivaLabel = "";

      foreach(Faktur faktur_rec in fakturList)
      {
         bool thisFakturIsTouched = false;

         faktur_rec.VvDao.LoadTranses(conn, faktur_rec, false);

         switch(serviceKind)
         {
            // 04.09.2015: 
            //case RtransServiceKind.CheckPrNabCij: arhivaLabel =                           "AUTO_NBC"; break;
            case RtransServiceKind.CheckPrNabCij: arhivaLabel = isPULXpass ? "AUT2_NBC" : "AUTO_NBC"; break;
            case RtransServiceKind.CheckZPCkol  : arhivaLabel = "CHCK_ZPC"; break;
            case RtransServiceKind.TrimZPC      : arhivaLabel = "TRIM_ZPC"; break;
         }

         arhivedFaktur_rec = faktur_rec.CreateArhivedDataRecord(conn, arhivaLabel);

         #region MOU

         bool hasMOU = rtransInDescrepancyList.Any(rtr => rtr.T_TT == Faktur.TT_MOU);

         Faktur MOD_faktur_rec = hasMOU ? (Faktur)faktur_rec.CreateNewRecordAndCloneItComplete() : null;

         #endregion MOU

         faktur_rec.BeginEdit();
         if(hasPossibleExtender) faktur_rec.VirtualExtenderRecord.BeginEdit();

         foreach(Rtrans rtrans_rec in faktur_rec.Transes)
         {
            rtransInDescrepancy = rtransInDescrepancyList.SingleOrDefault(rid => rid.T_recID == rtrans_rec.T_recID);

            if(rtransInDescrepancy != null)
            {
               rtrans_rec.BeginEdit();

               //if(isZPCkol == false) // old, classic 
               if(serviceKind == RtransServiceKind.CheckPrNabCij) // old, classic 
               {
                  //decimal tmpVal = rtrans_rec.T_cij;  // by Delf 15.06.2015
                  //                                    // 10.11.2014: 
                  //                                    //rtrans_rec.T_cij =                                               rtransInDescrepancy.TheAsEx.LastPrNabCij; // VOLIA ! 
                  //                                    // byDelf - 15.06.2015
                  //                                    //rtrans_rec.T_cij =                                    isPULXpass ? rtransInDescrepancy.TmpDecimal : rtransInDescrepancy.TheAsEx.LastPrNabCij; // VOLIA ! 

                  #region MOU

                  decimal MOU_cij = 0M;
                  bool isMOU = rtransInDescrepancy.T_TT == Faktur.TT_MOU;

                  if(isMOU) MOU_cij = MOD_PTG_DUC.Calc_AndOptional_ADDREC_MOD_Rtrans_From_MOD_Rtrano(conn, MOD_faktur_rec, rtrans_rec.T_serial);

                  #endregion MOU

                  decimal PULX_cij        = isMOU ? MOU_cij : rtransInDescrepancy.TmpDecimal;
                  decimal correctPrNabCij = rtransInDescrepancy.TheAsEx.LastPrNabCij;

                  rtrans_rec.T_cij = 
                     ConditionalValue(
                        /* old cij    */ rtrans_rec.T_cij, 
                        /* new cij    */ isPULXpass ? PULX_cij : correctPrNabCij,
                        /* tolerancy  */ ZXC.ValOrZero_Decimal(ChkPrNbC_diff_tolerancy.Replace(".", ","), 4) * 2 / 3, 
                        /* is touched */ ref thisFakturIsTouched)
                     ;

                  // 20.05.2015:  za svaki slucaj, a ne skodi: 
                  if(rtrans_rec.T_TT == Faktur.TT_ZPC && thisFakturIsTouched)
                  {
                     rtrans_rec.T_kol      = rtransInDescrepancy.TheAsEx.StanjeKol   ; // VOLIA ! 
                     rtrans_rec.T_doCijMal = rtransInDescrepancy.TheAsEx.PrevMalopCij; // VOLIA ! 
                                                                                       //rtrans_rec.T_doCijMal = rtransInDescrepancy.TheAsEx.LastUlazMPC ; 

                     // za svaki slucaj, a ne skodi: 
                     rtrans_rec.T_cij = rtransInDescrepancy.TheAsEx.LastPrNabCij; // VOLIA !? 
                  }
               }
               else if(serviceKind == RtransServiceKind.CheckZPCkol)// ZPC popravak kolicine i/ili oldMPC 
               {
                  //rtrans_rec.T_doCijMal.ToStringVv(), rtransInDescrepancy.TheAsEx.LastUlazMPC .ToStringVv());

                  rtrans_rec.T_kol = rtransInDescrepancy.TheAsEx.StanjeKol; // VOLIA ! 

                  rtrans_rec.T_doCijMal = rtransInDescrepancy.TheAsEx.PrevMalopCij; // VOLIA ! 
                                                                                    //rtrans_rec.T_doCijMal = rtransInDescrepancy.TheAsEx.LastUlazMPC ; 

                  // za svaki slucaj, a ne skodi: 
                  rtrans_rec.T_cij = rtransInDescrepancy.TheAsEx.LastPrNabCij; // VOLIA !? 

                  thisFakturIsTouched = true; // alwaus, ankondišnl 

               }

               if(serviceKind == RtransServiceKind.TrimZPC)
               {
                  rtrans_rec.SaveTransesWriteMode = ZXC.WriteMode.Delete;

                  thisFakturIsTouched = true;  // alwaus, ankondišnl 

               }
               else  // classic, ChkPrNabCij ili ChkZPC, NOT if(serviceKind == RtransServiceKind.TrimZPC) 
               {
                  rtrans_rec.SaveTransesWriteMode = ZXC.WriteMode.Edit;
               }

               // if(rtrans_rec.T_artiklCD == "SQ HHR")

            } // if(rtransInDescrepancy != null)

            rtrans_rec.CalcTransResults(faktur_rec);
         }

         if(thisFakturIsTouched/*faktur_rec.EditedHasChanges()*/)  // by Delf - 12.06.2015 - da ne snima nepotrebno
         {
            faktur_rec.TakeTransesSumToDokumentSum(/*31.01.2012: false*/ true);

            OK = ZXC.FakturDao.RWTREC(conn, faktur_rec, false, false);
            if(OK) arhivedFaktur_rec.VvDao.ADDREC(conn, arhivedFaktur_rec, true, true, false, false);
         }

         faktur_rec.EndEdit();
         if(hasPossibleExtender) faktur_rec.VirtualExtenderRecord.EndEdit();

      } // foreach(Faktur faktur_rec in fakturList) 

      return fakturList;
   }

   // 16.09.2015: 
   //internal static string ChkPrNbC_diff_tolerancy = "0.0015";
   internal static string ChkPrNbC_diff_tolerancy = "0.00015";

   private static List<Rtrans> GetRtransesWithAsEx_HavingDescrepancies_List(XSqlConnection conn)
   {
      List<Rtrans> rtransList = new List<Rtrans>();
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      #region SetFilterMembers4DescrepanciesList

      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD], false, "artCD", "", "", "", " != ", "", "R"));
      // filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt      ], false, "theTT", TtInfo.GetSql_IN_Clause(ZXC.TtInfoDokCijShouldBePrNabCij), "", "", " IN ", "", "R"));
      // NOTA BENE! TtInfo.GetSql_IN_Clause ne sljaka kada ide kao parametar, dakle, gornja linija javi syntax error!                                                                             
      filterMembers.Add(new VvSqlFilterMember("R.t_tt", TtInfo.GetSql_IN_Clause(ZXC.TtInfoDokCijShouldBePrNabCij), " IN "));
      // 14.11.2014: dizalo nepotrebno npr 6.4444 vs 6.4445 => 6.444 != 6.445
      //filterMembers.Add(new VvSqlFilterMember("ROUND(R.t_cij, 3)", "ROUND(A.lastPrNabCij, 3)", " != "));
      //filterMembers.Add(new VvSqlFilterMember("ROUND(R.t_cij, 2)", "ROUND(A.lastPrNabCij, 2)", " != "));
      filterMembers.Add(new VvSqlFilterMember("ABS(t_cij - lastPrNabCij)", ChkPrNbC_diff_tolerancy, " > "));

      #endregion SetFilterMembers4DescrepanciesList

      GetRtransWithArtstatList(conn, rtransList, "", filterMembers, "");

      return rtransList;
   }

   internal static void SvDUH_CheckPrNabCij_IZDsklDonacijeOnly(XSqlConnection conn, string skladCD)
   {
      List<Rtrans> rtransList = new List<Rtrans>();
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(4);

      #region SetFilterMembers4DescrepanciesList

      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD], false, "artCD",            "", "", "", " != ", "", "R"));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladCD ], false, "sklCD",       skladCD, "", "", "  = ", "", "R"));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt      ], false, "theTT", Faktur.TT_IZD, "", "", "  = ", "", "R"));
      filterMembers.Add(new VvSqlFilterMember("ABS(t_cij - lastPrNabCij)", ChkPrNbC_diff_tolerancy, " > "));

      #endregion SetFilterMembers4DescrepanciesList

      GetRtransWithArtstatList(conn, rtransList, "", filterMembers, "");

      Cursor.Current = Cursors.WaitCursor;

      ZXC.RISK_CheckPrNabDokCij_inProgress = true;

      // --- Here we go! --- 

      List<Rtrans> rtransInTroubleList = rtransList /*GetRtransesWithAsEx_HavingDescrepancies_List(conn)*/;

      ZXC.aim_log("[=] Going to change: {0} Rtranses!", rtransInTroubleList.Count.ToString());

      List<Faktur> fakturForRwtrecList;

      if(rtransInTroubleList.NotEmpty())
      {
         fakturForRwtrecList = GetFakturForRwtrecList_And_RwtNewValues(conn, rtransInTroubleList, false, RtransServiceKind.CheckPrNabCij);

         ZXC.aim_log("[=] Did change: {0} Fakturs!", fakturForRwtrecList.Count.ToString());
      }
      else
      {
         fakturForRwtrecList = new List<Faktur>();
      }

      string message = StringOfFakturList(fakturForRwtrecList, false);

      Cursor.Current = Cursors.Default;

      if(ZXC.TheVvForm.TheVvUC is VvRecordUC) ZXC.TheVvForm.WhenRecordInDBHasChangedAction(); // ReRead_OnClick, Refresh record 

      /*if(ZXC.ThisIsSkyLabProject == false)*/
      ZXC.aim_emsg(MessageBoxIcon.Information, "Revalorizacija je gotova. Dokumenti:\n\n{0}", message);

      ZXC.RISK_CheckPrNabDokCij_inProgress = false;

      ZXC.RtransDao.ResetRecIDinfoList();    // vvDelf 17.06.2015

      //return allOK;
   }

   private static List<Rtrans> GetRtransesWithAsEx_Having_ZPC_Descrepancies_List(XSqlConnection conn)
   {
      List<Rtrans> rtransList = new List<Rtrans>();
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(4);

      #region SetFilterMembers4DescrepanciesList

      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt], false, "elTT", Faktur.TT_ZPC, "", "", " = ", "", "R"));

      // 0712.2016: !!! BIG NEWS !!! zakljucismo da je ovo nuzno odraditi i na nultim ZPC-ovima! pa ubijamo ovaj FM koji izbauje nulte iz recenice 
      //filterMembers.Add(new VvSqlFilterMember("SUBSTRING(R.t_ttNUm, 4)", "000", " != ")); // skipaj 'nulte' ZPC-ove 

      filterMembers.Add(new VvSqlFilterMember("ROUND(R.t_doCijMal, 2)", "ROUND(A.prevMalopCij,                2)", " != ", ZXC.FM_OR_Enum.OPEN_OR));
    //filterMembers.Add(new VvSqlFilterMember("ROUND(R.t_doCijMal, 2)", "ROUND(A.lastMalopCij,                2)", " != ", ZXC.FM_OR_Enum.OPEN_OR ));
      filterMembers.Add(new VvSqlFilterMember("ROUND(R.t_kol,      2)", "ROUND(A.pstKol+A.ulazKol-A.izlazKol, 2)", " != ", ZXC.FM_OR_Enum.CLOSE_OR));

      #endregion SetFilterMembers4DescrepanciesList

      GetRtransWithArtstatList(conn, rtransList, "", filterMembers, "");

      return rtransList;
   }

   public static uint CountDescrepanciesInPrNabDokCij(XSqlConnection conn)
   {
      object obj;
      uint recCount;

      using(XSqlCommand cmd = VvSQL.CountDescrepanciesInPrNabDokCij_Command(conn, TtInfo.GetSql_IN_Clause(ZXC.TtInfoDokCijShouldBePrNabCij)))
      {
         try { obj = cmd.ExecuteScalar(); }
         catch(Exception) { return (0); }

         try { recCount = uint.Parse(obj.ToString()); }
         catch(Exception) { return (0); }
      }

      return (recCount);
   }

   public static List<Rtrans> Get_UnusefulZPC_RtransList(XSqlConnection conn, DateTime dateDO)
   {
      bool success = true;
      Rtrans rtrans_rec;
      Rtrans prev_rtrans_rec = null;
      //bool prevZpcMonth_isOldEra;
      //int prevZpcMonth, thisZpcMonth;
      //string currArtCD = "", currSklCD = "";
      ZXC.sqlErrNo = 0;

      List<Rtrans> unusefulZPC_rtransList = new List<Rtrans>();

      ZXC.aim_log("Get_UnusefulZPC_RtransList Process start!");

      #region SetFilterMembers

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(4);

      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladDate], "theDateDO", dateDO, " <= "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD], "theArtCD", "", " != "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladCD], "theSklCD", "__M_", " LIKE "));
      filterMembers.Add(new VvSqlFilterMember("SUBSTRING(t_ttNUm, 4)", "000", " != ")); // skipaj 'nulte' ZPC-ove 

      //filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD], "theArtCDod", "504", " >= "));
      //filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD], "theArtCDdo", "506", " <= "));

      #endregion SetFilterMembers

      using(XSqlCommand cmd = (VvSQL.Get_UnusefulZPC_RtransList_Command(conn, filterMembers, "t_skladCD, t_artiklCD, t_skladDate, t_ttSort, t_ttNum, t_serial")))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;

               for(; success && reader.Read(); prev_rtrans_rec = rtrans_rec.MakeDeepCopy())
               {
                  rtrans_rec = new Rtrans();

                  ZXC.RtransDao.FillFromDataReader(rtrans_rec, reader, false);

                  #region loop logic

                  if(prev_rtrans_rec == null) continue;

                  if(rtrans_rec.T_TT != Faktur.TT_ZPC || prev_rtrans_rec.T_TT != Faktur.TT_ZPC) continue;

                  // 16.07.2015: ubio ovo 
                  //prevZpcMonth = prev_rtrans_rec.T_skladDate.Month;
                  //thisZpcMonth = rtrans_rec     .T_skladDate.Month;
                  //prevZpcMonth_isOldEra = prevZpcMonth <= 6 && prev_rtrans_rec.T_skladDate < ZXC.TexthoNewArtiklsDate;
                  //if(thisZpcMonth != prevZpcMonth && prevZpcMonth_isOldEra) continue;

                  if((rtrans_rec.T_artiklCD != prev_rtrans_rec.T_artiklCD) ||
                     (rtrans_rec.T_skladCD  != prev_rtrans_rec.T_skladCD)) continue;

                  // 01.01.2022: 
                  if(prev_rtrans_rec.IsNultiZPC) continue;

                  unusefulZPC_rtransList.Add(prev_rtrans_rec);

                  ZXC.aim_log("{0} --> Dodan", prev_rtrans_rec);

                  #endregion loop logic

               } // while(success && reader.Read())

               reader.Close();
            }
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("Get_UnusefulZPC_RtransList", ex, System.Windows.Forms.MessageBoxButtons.OK);

            ZXC.sqlErrNo = ex.Number;
         }
      } // using 

      ZXC.aim_log("Exiting Get_UnusefulZPC_RtransList: ukupno unusefulZPC_rtransList: {0}", unusefulZPC_rtransList.Count.ToString());

      return unusefulZPC_rtransList;
   }

   #endregion CheckPrNabDokCij

   #region GetRtransWithArtstatList

   public static void GetRtransWithArtstatList(XSqlConnection conn, List<Rtrans> rtransList, string anotherJoinClause, List<VvSqlFilterMember> filterMembers, string orderBy)
   {
      bool success = true;
      Rtrans rtrans_rec;

      ZXC.sqlErrNo = 0;

      // makni '//Q' ako oces pagging 

      ZXC.ForceXtablePreffix = true;

      //Quint limitOffset     = 0;
      //Quint paketRowCount   = ZXC.BigDataRecCountLimit;
      //Quint recCountSoFar   = 0;
      //Quint readerPass      = 0;

      //Qwhile(success && (paketRowCount * readerPass) == recCountSoFar) // u jednom trenutku novi limitOffset ce biti veci od recCountSoFar, sto znaci da prosli pass nije napunjen do kraja pa treba prekinuti 
      //Q{
      //Q   limitOffset = paketRowCount * readerPass++;

      using(XSqlCommand cmd = (VvSQL.GetFaktursRtransWithArtstatList_Command(conn, anotherJoinClause, filterMembers, orderBy/*Q, limitOffset, paketRowCount*/)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;

               while(success && reader.Read())
               {
                  rtrans_rec = new Rtrans();

                  //ZXC.RtransDao .FillFromNarrowDataReader(rtrans_rec,         reader);
                  //ZXC.ArtStatDao.FillFromDataReader      (rtrans_rec.TheAsEx, reader, false, RtransDao.lastNarrowRtransCI + 1);
                  ZXC.RtransDao.FillFromDataReader(rtrans_rec, reader, false);
                  ZXC.ArtStatDao.FillFromDataReader(rtrans_rec.TheAsEx, reader, false, RtransDao.lastRtransCI + 1);

                  rtrans_rec.CalcTransResults(null);

                  rtransList.Add(rtrans_rec);

                  //QrecCountSoFar++;
               }
               reader.Close();
            }
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("GetFaktursRtransWithArtstatList", ex, System.Windows.Forms.MessageBoxButtons.OK);

            ZXC.sqlErrNo = ex.Number;
         }
      } // using 

      //Q} // while(...) 

      ZXC.ForceXtablePreffix = false;
   }

   #endregion GetRtransWithArtstatList

   #region Prodaja Po Dobavljacima

   internal static List<Rtrans> PairDobavByFIFO(XSqlConnection conn, List<Rtrans> izlazRtransList, string skladCD, bool shouldOverwriteKupdobCD)
   {
      if(izlazRtransList.Count.IsZero()) return izlazRtransList;

      izlazRtransList = izlazRtransList.OrderBy(iRtr => iRtr.T_ttNum).ToList();

      DateTime dateDO = izlazRtransList.Max(r => r.T_skladDate);
      string ulazTT_INclause = TtInfo.GetSql_IN_Clause(ZXC.TtUlazKolArray);

      List<Rtrans> ulazRtransList = new List<Rtrans>();
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladDate], false, "theDateDO", dateDO, "", "", " <= ", "", "R"));
      filterMembers.Add(new VvSqlFilterMember("R.t_tt", ulazTT_INclause, " IN "));
      if(skladCD.NotEmpty())
      {
         filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladCD], false, "theSkladCD", skladCD, "", "", " = ", "", "R"));
      }

      RtransDao.GetRtransWithArtstatList(conn, ulazRtransList, "", filterMembers, Rtrans.artiklOrderBy_ASC.Replace("t_", "R.t_"));

      ulazRtransList.ForEach(rtr => rtr.CalcTransResults(null)); // za onaj dole R_kol; 

      decimal prevIzlazKOL;
      decimal thisIzlazKOL;
      decimal reachedKol;
      uint thisUlazKCD;
      decimal thisUlazKOL;
      decimal thisUlaz_CIJ_KCR;
      int firstGoodIdx;

      Rtrans firstGoodUlaz;
      Rtrans shadowIzlazRtrans;

      List<Rtrans> artiklULAZrtransList;

      for(int i = 0; i < izlazRtransList.Count(); ++i)
      {
         artiklULAZrtransList = ulazRtransList.Where(rtr => rtr.T_artiklCD == izlazRtransList[i].T_artiklCD && rtr.T_skladCD == izlazRtransList[i].T_skladCD).ToList();

         thisIzlazKOL = izlazRtransList[i].R_kol;
         prevIzlazKOL = izlazRtransList[i].A_UkIzlazKol - thisIzlazKOL;

         firstGoodUlaz = artiklULAZrtransList.FirstOrDefault(uRtr => uRtr.A_UkUlazKol > prevIzlazKOL);
         firstGoodIdx = artiklULAZrtransList.IndexOf(firstGoodUlaz);

         if(artiklULAZrtransList == null || artiklULAZrtransList.Count().IsZero() || firstGoodUlaz == null)
         {
            izlazRtransList[i].T_twinID = 0;
            izlazRtransList[i].TmpDecimal = 0.00M;

            if(shouldOverwriteKupdobCD)
            {
               izlazRtransList[i].T_kupdobCD = izlazRtransList[i].T_twinID;
               izlazRtransList[i].T_cij = izlazRtransList[i].TmpDecimal;
            }
            continue;
         }

         reachedKol = firstGoodUlaz.A_UkUlazKol - prevIzlazKOL;

         izlazRtransList[i].T_twinID = firstGoodUlaz.T_kupdobCD;
         izlazRtransList[i].TmpDecimal = izlazRtransList[i].T_kol * firstGoodUlaz.R_CIJ_KCR;

         if(shouldOverwriteKupdobCD)
         {
            izlazRtransList[i].T_kupdobCD = izlazRtransList[i].T_twinID;
            izlazRtransList[i].T_cij = izlazRtransList[i].TmpDecimal;
         }

         int shadowCount = 0;

         while(reachedKol < thisIzlazKOL && firstGoodIdx < artiklULAZrtransList.Count() - 1)
         {
            ++firstGoodIdx;

            thisUlazKCD = artiklULAZrtransList[firstGoodIdx].T_kupdobCD;
            thisUlazKOL = artiklULAZrtransList[firstGoodIdx].R_kol;
            thisUlaz_CIJ_KCR = artiklULAZrtransList[firstGoodIdx].R_CIJ_KCR;

            reachedKol += thisUlazKOL;

            if(reachedKol > thisIzlazKOL) thisUlazKOL -= reachedKol - thisIzlazKOL;

            shadowIzlazRtrans = izlazRtransList[i].MakeDeepCopy();
            shadowIzlazRtrans.T_twinID = thisUlazKCD;
            shadowIzlazRtrans.T_kol = thisUlazKOL;
            shadowIzlazRtrans.TmpDecimal = shadowIzlazRtrans.T_kol * thisUlaz_CIJ_KCR;
            shadowIzlazRtrans.CalcTransResults(null);

            if(shouldOverwriteKupdobCD)
            {
               shadowIzlazRtrans.T_kupdobCD = shadowIzlazRtrans.T_twinID;
               shadowIzlazRtrans.T_cij = shadowIzlazRtrans.TmpDecimal;
            }

            izlazRtransList[i - shadowCount].T_kol -= thisUlazKOL;
            izlazRtransList[i - shadowCount].TmpDecimal = izlazRtransList[i - shadowCount].T_kol *
                                                          artiklULAZrtransList[firstGoodIdx - shadowCount - 1].R_CIJ_KCR;
            izlazRtransList[i - shadowCount].CalcTransResults(null);

            izlazRtransList.Insert(i++ + 1, shadowIzlazRtrans);
            shadowCount++;
         }
      }

      return izlazRtransList;
   }

   #endregion Prodaja Po Dobavljacima

   #region GlassOnIRM Methods

   internal static List<Rtrans> SetPimPumRtransList(XSqlConnection conn, VvDocumentRecord vvDocumentRecord, VvTransRecord trans_rec/*, ref ushort t_serial*/)
   {
      #region INIT

      Faktur faktur_rec = vvDocumentRecord as Faktur;
      Rtrans irmRtrans_rec = trans_rec as Rtrans;
      ArtStat irmArtstat_rec = ArtiklDao.GetArtiklStatus(conn, irmRtrans_rec);

      Artikl bottleArtikl_rec = null;
      Artikl glassArtikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == irmRtrans_rec.T_artiklCD);
      if(glassArtikl_rec != null)
      {
         bottleArtikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == glassArtikl_rec.LinkArtCD);
      }
      bool hasLinkedArtikl = bottleArtikl_rec != null;

      List<Rtrans> pimPumRtransList = new List<Rtrans>();
      List<Rtrans> bottleRtransList = new List<Rtrans>();

      uint t_parentID = irmRtrans_rec.T_parentID + 2000000000; // valjda nece t_parentID od IRMa doci do 2 milijarde! 
                                                               //uint   t_dokNum   = faktur_rec.VvDao.GetNextDokNum(conn, vvDocumentRecord.VirtualRecordName);
                                                               //uint   t_ttNum    = faktur_rec.VvDao.GetNextTtNum (conn, Faktur.TT_PIM, faktur_rec.SkladCD );
      uint t_dokNum = irmRtrans_rec.T_dokNum; // well, dokNum i ttNum PIMa ce biti isti kao i na IRMu, pa i tako mozes kasnije po potrebi povezivati 
      uint t_ttNum = irmRtrans_rec.T_ttNum; // well, dokNum i ttNum PIMa ce biti isti kao i na IRMu, pa i tako mozes kasnije po potrebi povezivati 

      decimal pumGlassNabCij = 0M;

      ushort t_serial = (ushort)(irmRtrans_rec.T_serial * 1000);

      #endregion INIT

      #region idemo via LinkedArtCD

      if(hasLinkedArtikl == true) // idemo via LinkedArtCD 
      {
         ArtStat PIM_bottleARTSTAT_rec = ArtiklDao.GetArtiklStatus(conn, bottleArtikl_rec.ArtiklCD, irmRtrans_rec.T_skladCD, irmRtrans_rec.T_skladDate);
         if(PIM_bottleARTSTAT_rec == null) PIM_bottleARTSTAT_rec = new ArtStat(); // nema jos nista u Rtrans-u za ovaj artikl na ovom skladistu 

         Rtrans PIM_bottleRtrans_rec = new Rtrans();

         PIM_bottleRtrans_rec.T_parentID = t_parentID;
         PIM_bottleRtrans_rec.T_dokNum = t_dokNum;
         PIM_bottleRtrans_rec.T_ttNum = t_ttNum;
         PIM_bottleRtrans_rec.T_serial = ++t_serial;
         PIM_bottleRtrans_rec.T_skladDate = irmRtrans_rec.T_skladDate;
         PIM_bottleRtrans_rec.T_TT = Faktur.TT_PIM;
         PIM_bottleRtrans_rec.T_ttSort = ZXC.TtInfo(Faktur.TT_PIM).TtSort;
         PIM_bottleRtrans_rec.T_artiklCD = bottleArtikl_rec.ArtiklCD;
         PIM_bottleRtrans_rec.T_skladCD = irmRtrans_rec.T_skladCD;
         PIM_bottleRtrans_rec.T_artiklName = bottleArtikl_rec.ArtiklName;
         PIM_bottleRtrans_rec.T_kupdobCD = irmRtrans_rec.T_kupdobCD;
         PIM_bottleRtrans_rec.T_jedMj = bottleArtikl_rec.JedMj;
         PIM_bottleRtrans_rec.T_konto = bottleArtikl_rec.Konto;
         PIM_bottleRtrans_rec.T_kol = ZXC.DivSafe(irmRtrans_rec.R_kolOP, bottleArtikl_rec.R_orgPak); // !!! 
         pumGlassNabCij = PIM_bottleARTSTAT_rec.PrNabCijOP;
         PIM_bottleRtrans_rec.T_cij = PIM_bottleARTSTAT_rec./*PrNabCij*/LastUlazMPC; // !!! 
         PIM_bottleRtrans_rec.T_pdvSt = irmRtrans_rec.T_pdvSt;
         //PIM_bottleRtrans_rec.T_rbt1St     = ;
         //PIM_bottleRtrans_rec.T_rbt2St     = ; TODO: a sta ako da rabat na cugu!? onda i T_wanted treba biti drugaciji. 
         PIM_bottleRtrans_rec.T_wanted = PIM_bottleARTSTAT_rec.LastUlazMPC; // !!! 
                                                                            //PIM_bottleRtrans_rec.T_doCijMal   = ;
                                                                            //PIM_bottleRtrans_rec.T_noCijMal   = ;
         PIM_bottleRtrans_rec.T_twinID = irmRtrans_rec.T_recID;
         //PIM_bottleRtrans_rec.T_pdvColTip  = ;
         //PIM_bottleRtrans_rec.T_ztr        = ;
         //PIM_bottleRtrans_rec.T_kol2       = ;
         PIM_bottleRtrans_rec.T_mCalcKind = ZXC.MalopCalcKind.By_MPC;
         //PIM_bottleRtrans_rec.T_mtrosCD    = ;
         //PIM_bottleRtrans_rec.T_isIrmUsluga= ;
         //PIM_bottleRtrans_rec.T_ppmvOsn    = ;
         //PIM_bottleRtrans_rec.T_ppmvSt1i2  = ;
         PIM_bottleRtrans_rec.T_pnpSt = ZXC.RRD.Dsc_PnpSt;

         PIM_bottleRtrans_rec.CalcTransResults(null);
         // force proizvodnja trans order: 'PUM' gshould go last. 
         pimPumRtransList.Add(PIM_bottleRtrans_rec);
      }

      #endregion idemo via LinkedArtCD

      #region idemo via normativ

      else if(hasLinkedArtikl == false) // idemo via normativ 
      {
         bottleRtransList = new List<Rtrans>();

         // TODO: 
      }

      #endregion idemo via normativ

      //                                                                                                   
      // Za prodanih 7.5 čaša Jackie 0.05:                                                                 
      //                                                                                                   
      // irmRtrans_rec.R_kolOP      (0.375 = IRM_t_kol(7.5) * čašaOrgPak(0.05))                            
      // bottleArtikl_rec.R_orgPak  (0.75)                                                                 
      // PIM_bottleRtrans_rec.T_kol (0.5 = irmRtrans_rec.R_kolOP (0.375) / bottleArtikl_rec.R_orgPak(0.75) 
      //                                                                                                   
      // pumGlassNabCij             = PIM_bottleARTSTAT_rec.PrNabCijOP  (170)                              
      // PIM_bottleRtrans_rec.T_cij = PIM_bottleARTSTAT_rec.LastUlazMPC (375)                              
      //                                                                                                   
      // PUM_glassRtrans_rec.T_wanted = irmRtrans_rec.T_cij (25)                                           
      // PUM_glassRtrans_rec.T_cij    (8.5 = pumGlassNabCij(170 * glassArtikl_rec.R_orgPak(0.05))          
      //                                                                                                   

      Rtrans PUM_glassRtrans_rec = irmRtrans_rec.MakeDeepCopy();

      PUM_glassRtrans_rec.T_parentID = t_parentID;
      PUM_glassRtrans_rec.T_dokNum = t_dokNum;
      PUM_glassRtrans_rec.T_ttNum = t_ttNum;
      PUM_glassRtrans_rec.T_serial = ++t_serial;
      PUM_glassRtrans_rec.T_TT = Faktur.TT_PUM;
      PUM_glassRtrans_rec.T_ttSort = ZXC.TtInfo(Faktur.TT_PUM).TtSort;
      PUM_glassRtrans_rec.T_twinID = irmRtrans_rec.T_recID;

      PUM_glassRtrans_rec.T_wanted = irmRtrans_rec.T_cij;
      PUM_glassRtrans_rec.T_cij = pumGlassNabCij * glassArtikl_rec.R_orgPak; // !!! 

      // force proizvodnja trans order: 'PUM' gshould go last. 
      pimPumRtransList.Add(PUM_glassRtrans_rec); // !!! 

      return pimPumRtransList;
   }

   internal static bool AddPimPumRtranses(XSqlConnection conn, VvDocumentRecord vvDocumentRecord, VvTransRecord trans_rec/*, ref ushort line*/)
   {
      bool success = true;

      List<Rtrans> linkedPimPum_rtransList = RtransDao.SetPimPumRtransList(conn, vvDocumentRecord, trans_rec/*, ref line*/);

      foreach(Rtrans pimPumRtrans in linkedPimPum_rtransList)
      {
         success = pimPumRtrans.VvDao.ADDREC(conn, pimPumRtrans, false, false, false, false);
      }
      return success;
   }

   internal static bool DeletePimPumRtranses(XSqlConnection conn, VvTransRecord trans_rec)
   {
      bool success = true;

      List<Rtrans> toDeletelinkedPimPum_rtransList = new List<Rtrans>();
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_twinID], "theTwinID", trans_rec.VirtualRecID, " = "));
      LoadGenericVvDataRecordList(conn, toDeletelinkedPimPum_rtransList, filterMembers, "");

      foreach(Rtrans pimPumRtrans in toDeletelinkedPimPum_rtransList)
      {
         success = pimPumRtrans.VvDao.DELREC(conn, pimPumRtrans, false);
      }

      return success;
   }

   #endregion GlassOnIRM Methods

   #region GetRtransList_ForProjektCD

   internal static List<Rtrans> GetRtransList_ForProjektCD(XSqlConnection conn, string projektTT_And_TtNum, string orderBy)
   {
      List<Rtrans> projektRtransList = new List<Rtrans>();

      // 01.03.2016: !!! 
      if(projektTT_And_TtNum.IsEmpty()) return projektRtransList;

      VvRptFilter rptFilter = new VvRptFilter();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.projektCD], "projektCD", projektTT_And_TtNum, " = "));

      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt], "tt1", Faktur.TT_RNM, " != "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt], "tt2", Faktur.TT_RNU, " != "));

      rptFilter.FilterMembers = filterMembers;

      // 15.02.2017: go in past (if needed) _start_ @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@  
      string prjTT;
      uint prjTtNum;

      Ftrans.ParseTipBr(projektTT_And_TtNum, out prjTT, out prjTtNum);

      int[] prevYears = FtransDao.GetPrevYearsList(prjTtNum, true);

      foreach(int year in prevYears)
      {
         ZXC.RtransDao.LoadManyDocumentsTtranses(ZXC.TheSecondDbConn_SameDB_OtherYear(year), projektRtransList, rptFilter, orderBy);
         projektRtransList.ForEach(rtr => rtr.R_utilBool = true); // flag da znamo da rtrans nije iz tekuce godine 
      }

      if(ZXC.theSecondDbConnection != null) ZXC.theSecondDbConnection.Close(); // nemoj tu pozivaty propertyy nego koristi varijablu (malo slovo)

      // 15.02.2017: go in past (if needed) _ end _ @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@  

      ZXC.RtransDao.LoadManyDocumentsTtranses(conn, projektRtransList, rptFilter, orderBy);

      return projektRtransList;
   }

   internal static List<Rtrans> GetRtransList_ForTT_And_TtNum(XSqlConnection conn, string theTT_And_TtNum, string orderBy)
   {
      return GetRtransList_ForTT_And_TtNum(conn, theTT_And_TtNum, orderBy, false);
   }
   internal static List<Rtrans> GetRtransList_ForTT_And_TtNum(XSqlConnection conn, string theTT_And_TtNum, string orderBy, bool needsSplitOrTwinTranses)
   {
      List<Rtrans> rtransList = new List<Rtrans>();

      string theTT;
      uint theTtNum;

      Ftrans.ParseTipBr(theTT_And_TtNum, out theTT, out theTtNum);

      if(theTT.IsEmpty() || theTtNum.IsZero()) return rtransList;

      if(needsSplitOrTwinTranses)
      {
         if(ZXC.TtInfo(theTT).HasSplitTT) theTT = ZXC.TtInfo(theTT).SplitTT;
         if(ZXC.TtInfo(theTT).HasTwinTT) theTT = ZXC.TtInfo(theTT).TwinTT;
      }

      VvRptFilter rptFilter = new VvRptFilter();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt], "theTT", theTT, " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_ttNum], "theTtNum", theTtNum, " = "));

      rptFilter.FilterMembers = filterMembers;

      ZXC.RtransDao.LoadManyDocumentsTtranses(conn, rtransList, rptFilter, orderBy);

      return rtransList;
   }

   private static decimal Get_KoefDovrsenosti_From_RNM_PPR_PIP_list_JOB(List<Rtrans> rtrans_RNM_RNU_list, List<Rtrans> rtrans_PPR_PIP_list, decimal ukKol_Kg_UlazPG, bool isOGonly)
   {
      if((rtrans_RNM_RNU_list == null || rtrans_RNM_RNU_list.Count.IsZero())) return 0.00M;
      if((rtrans_PPR_PIP_list == null || rtrans_PPR_PIP_list.Count.IsZero()) && ukKol_Kg_UlazPG.IsZero()) return 0.00M;

#if OldPIP_2016
      return ZXC.DivSafe
         (
            rtrans_PPR_PIP_list.Where(rtr => rtr.T_TT == Faktur.TT_PIP).Sum(rtr => rtr.T_kol), // PIP kol / 
            rtrans_RNM_list    .Where(rtr => rtr.T_TT == Faktur.TT_RNU).Sum(rtr => rtr.T_kol)  // RNU kol   
         );
#else
      // PIP 2017 
      // 25.01.2017: prelazimo na kilogramsko (a ne komadno) ponderiranje stupnja dovrsenosti 
      return ZXC.DivSafe
         (
            rtrans_PPR_PIP_list.Where(rtr => rtr.T_TT == Faktur.TT_PIP).Sum(rtr => rtr.R_kolOP) + (isOGonly ? 0 : ukKol_Kg_UlazPG), // PIP kg / - REALIZACIJA 
            rtrans_RNM_RNU_list.Where(rtr => rtr.T_TT == Faktur.TT_RNU).Sum(rtr => rtr.R_kolOP) - (isOGonly ? ukKol_Kg_UlazPG : 0)  // RNU kg   - PLAN        
         );
      // PIP 2017 
#endif
   }

   internal static decimal Get_KoefDovrsenosti_From_RNM_PPR_PIP_list_PG(List<Rtrans> rtrans_RNM_RNU_list, decimal ukKol_Kg_UlazPG)
   {
      if((rtrans_RNM_RNU_list == null || rtrans_RNM_RNU_list.Count.IsZero())) return 0.00M;

      return ZXC.DivSafe(ukKol_Kg_UlazPG,                                                                      // PIP kg / - REALIZACIJA iz PG 
                         rtrans_RNM_RNU_list.Where(rtr => rtr.T_TT == Faktur.TT_RNU).Sum(rtr => rtr.R_kolOP)); // RNU kg   - PLAN        
   }

   internal static decimal Get_KoefDovrsenosti_From_RNM_PPR_PIP_list(bool isOGonly, Faktur rnmFaktur_rec, List<Rtrans> rtrans_PPR_PIP_list, decimal ukKol_OP_UlazPG)
   {
      return Get_KoefDovrsenosti_From_RNM_PPR_PIP_list(isOGonly, rnmFaktur_rec, rtrans_PPR_PIP_list, ukKol_OP_UlazPG, false);
   }

   internal static decimal Get_KoefDovrsenosti_From_RNM_PPR_PIP_list(bool isOGonly, Faktur rnmFaktur_rec, List<Rtrans> rtrans_PPR_PIP_list, decimal ukKol_OP_UlazPG, bool ignoreR)
   {
      if(ignoreR == false && rnmFaktur_rec.StatusCD == "R") return 1M; // RNM ima oznaku StatusCD-a 'R' ... znaci 100% dovrsen. 

      return RtransDao.Get_KoefDovrsenosti_From_RNM_PPR_PIP_list_JOB(rnmFaktur_rec.Transes, rtrans_PPR_PIP_list, ukKol_OP_UlazPG, isOGonly);
   }

   #endregion GetRtransList_ForProjektCD

   #region GetFakturList_ForProjektCD

   internal static List<Faktur> GetFakturList_ForProjektCD(XSqlConnection conn, string projektTT_And_TtNum)
   {
      List<Faktur> projektFakturList = new List<Faktur>();

      //VvRptFilter rptFilter = new VvRptFilter();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.projektCD], "projektCD", projektTT_And_TtNum, " = "));

      //rptFilter.FilterMembers = filterMembers;

      VvDaoBase.LoadGenericVvDataRecordList<Faktur>(conn, projektFakturList, filterMembers, "", "dokDate, ttSort, ttNum", true);

      return projektFakturList;
   }

   // 09.02.2020: 
 //internal static List<Faktur> GetFakturList_VezniDok_KupdobCD_DokDate(XSqlConnection conn, string vezniDok, uint kupdobCD  , DateTime dokDate)
   internal static List<Faktur> GetFakturList_VezniDok_KupdobCD_DokDate(XSqlConnection conn, string vezniDok, uint kupdobCD/*, DateTime dokDate*/)
   {
      List<Faktur> theFakturList = new List<Faktur>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(4);

      // 09.02.2020: 
    //filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate], "dokDate", dokDate.Date, " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.vezniDok], "vezniDok", vezniDok, " = "));

      filterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.kupdobCD], "kupdobCD", kupdobCD, " = "));

      filterMembers.Add(new VvSqlFilterMember("tt", TtInfo.UlazniPdv_IN_Clause, " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!

      VvDaoBase.LoadGenericVvDataRecordList<Faktur>(conn, theFakturList, filterMembers, "", "dokDate, ttSort, ttNum", true);

      return theFakturList;
   }

   internal static List<Faktur> GetFakturList_TT_KupdobCD_DokDate_KCRP(XSqlConnection conn, string tt, uint kupdobCD, DateTime dokDate, decimal s_ukKCRP)
   {
      decimal tolerancy = 0.01M;

      decimal kcrpOD = s_ukKCRP - tolerancy;
      decimal kcrpDO = s_ukKCRP + tolerancy;

      List<Faktur> theFakturList = new List<Faktur>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(5);

      filterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.kupdobCD], "kupdobCD", kupdobCD    , " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt      ], "tt"      , tt          , " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate ], "dokDate" , dokDate.Date, " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.s_ukKCRP], "minKCRP" , kcrpOD      , " > "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.s_ukKCRP], "maxKCRP" , kcrpDO      , " < "));



      VvDaoBase.LoadGenericVvDataRecordList<Faktur>(conn, theFakturList, filterMembers, "", "dokDate, ttSort, ttNum", true);

      return theFakturList;
   }

   internal static bool GetProjektFaktur_ForProjektCD(XSqlConnection conn, Faktur projektFaktur_rec, string projektTT_And_TtNum)
   {
      string tt;
      uint ttNum;

      Ftrans.ParseTipBr(projektTT_And_TtNum, out tt, out ttNum);

      return FakturDao.SetMeFaktur(ZXC.TheVvForm.TheDbConnection, projektFaktur_rec, tt, ttNum, false);
   }

   internal static List<Faktur> Get_SVD_URA_FakturList_For_UGOVOR(XSqlConnection conn, string ugovorTT_And_TtNum)
   {
      List<Faktur> URA_FakturList = new List<Faktur>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt       ], "theTT"    , Faktur.TT_URA     , " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.projektCD], "projektCD", ugovorTT_And_TtNum, " = "));

      VvDaoBase.LoadGenericVvDataRecordList<Faktur>(conn, URA_FakturList, filterMembers, "", "dokDate, ttSort, ttNum", true);

      return URA_FakturList;
   }

   internal static List<Faktur> GetFakturList_TT_V1tt_V1ttNum_KupdobCD/*_DokDateOD*/(XSqlConnection conn, string tt, string v1tt, uint v1ttNum, uint kupdobCD/*, DateTime dokDateOD*/)
   {
      List<Faktur> theFakturList = new List<Faktur>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(/*5*/4);

      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt      ], "tt"      , tt            , " = " ));
    //filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate ], "dokDate" , dokDateOD.Date, " >= "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.v1_tt   ], "v1tt"    , v1tt          , " = " ));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.v1_ttNum], "v1ttNum" , v1ttNum       , " = " ));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.kupdobCD], "kupdobCD", kupdobCD      , " = " ));

      VvDaoBase.LoadGenericVvDataRecordList<Faktur>(conn, theFakturList, filterMembers, "", "dokDate, ttSort, ttNum", true);

      return theFakturList;
   }

   internal static List<Rtrans> GetRtransList_TT_V1tt_V1ttNum_KupdobCD/*_DokDateOD*/(XSqlConnection conn, string tt, string v1tt, uint v1ttNum, uint kupdobCD/*, DateTime dokDateOD*/)
   {
      List<Faktur> linkedFakturList = GetFakturList_TT_V1tt_V1ttNum_KupdobCD/*_DokDateOD*/(conn, tt, v1tt, v1ttNum, kupdobCD/*, dokDateOD*/);

      if(linkedFakturList.IsEmpty()) return null;

      List<Rtrans> linkedRtransList = new List<Rtrans>();

      foreach(Faktur linkedFaktur in linkedFakturList)
      {
         linkedFaktur.VvDao.LoadTranses(conn, linkedFaktur, false);

         linkedRtransList.AddRange(linkedFaktur.Transes);
      }

      return linkedRtransList;
   }

   #endregion GetFakturList_ForProjektCD

   #region GetFakturList_ByVezaTtAndTtNumOrProjektCD

   public static List<Faktur> GetFakturList_ByVezaTtAndTtNumOrProjektCD(XSqlConnection conn, Faktur faktur_rec)
   {
      bool success = true;
      Faktur linkedFaktur_rec = new Faktur();
      List<Faktur> linkedFakturList = new List<Faktur>();

      ZXC.sqlErrNo = 0;

      if(faktur_rec.TT.IsEmpty() && faktur_rec.TtNum.IsZero()) return linkedFakturList;

      using(XSqlCommand cmd = (VvSQL.GetFakturList_ByVezaTtAndTtNumOrProjektCD_Command(conn, faktur_rec)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;

               while(success && reader.Read())
               {
                  linkedFaktur_rec = new Faktur();

                  ZXC.FakturDao.FillFromDataReader(linkedFaktur_rec, reader, false);

                  if(linkedFaktur_rec.TtInfo.IsExtendableTT)
                  {
                     linkedFaktur_rec.TheEx = new FaktEx();
                     linkedFaktur_rec.TheEx.IsFillingFromJoinReader = true;
                     //linkedFaktur_rec.TheEx.IsFillingFrom_Fak2Nal_URM = true;
                     linkedFaktur_rec.TheEx.VvDao.FillFromDataReader(linkedFaktur_rec.TheEx, reader, false);
                     linkedFaktur_rec.TheEx.IsFillingFromJoinReader = false;
                  }

                  if(faktur_rec.RecID != linkedFaktur_rec.RecID) // nemoj samog sebe 
                  {
                     linkedFakturList.Add(linkedFaktur_rec);
                  }
               }
               reader.Close();
            }
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("GetFakturList_ByVezaTtAndTtNumOrProjektCD", ex, System.Windows.Forms.MessageBoxButtons.OK);

            ZXC.sqlErrNo = ex.Number;
         }
      } // using 

      return linkedFakturList;
   }

   //internal static List<Faktur> GetFakturList_ByVezaTtAndTtNumOrProjektCD(XSqlConnection conn, Faktur faktur_rec)
   //{
   //   List<Faktur> linkedtFakturList = new List<Faktur>();

   //   List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(9);

   //   #region filterMemberz

   //   filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.projektCD], ZXC.FM_OR_Enum.OPEN_OR, false, "projektCD", faktur_rec.ProjektCD, "", "", " = ", ""));

   //   filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.v1_tt]   , ZXC.FM_OR_Enum.OPEN_OR, false, "v1_tt"   , faktur_rec.V1_tt   , "", "", " = ", ""));
   //   filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.v1_ttNum], ZXC.FM_OR_Enum.NONE   , false, "v1_ttNum", faktur_rec.V1_ttNum, "", "", " = ", ""));

   //   filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.v2_tt]   , ZXC.FM_OR_Enum.OPEN_OR, false, "v2_tt"   , faktur_rec.V2_tt   , "", "", " = ", ""));
   //   filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.v2_ttNum], ZXC.FM_OR_Enum.NONE   , false, "v2_ttNum", faktur_rec.V2_ttNum, "", "", " = ", ""));

   //   filterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.v3_tt]   , ZXC.FM_OR_Enum.OPEN_OR, false, "v3_tt"   , faktur_rec.V3_tt   , "", "", " = ", ""));
   //   filterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.v3_ttNum], ZXC.FM_OR_Enum.NONE   , false, "v3_ttNum", faktur_rec.V3_ttNum, "", "", " = ", ""));

   //   filterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.v4_tt]   , ZXC.FM_OR_Enum.OPEN_OR, false, "v4_tt"   , faktur_rec.V4_tt   , "", "", " = ", ""));
   //   filterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.v4_ttNum], ZXC.FM_OR_Enum.NONE   , false, "v4_ttNum", faktur_rec.V4_ttNum, "", "", " = ", ""));

   //   #endregion filterMemberz

   //   VvDaoBase.LoadGenericVvDataRecordList<Faktur>(conn, linkedtFakturList, filterMembers, "", "dokDate, ttSort, ttNum", true);

   //   return linkedtFakturList;
   //}

   #endregion GetFakturList_ByVezaTtAndTtNumOrProjektCD

   #region T_serlot - SerialNumber, Atest, LOT, ...

   internal static List<ZXC.VvUtilDataPackage> GetFreeSerlotList_ForArtikl(XSqlConnection conn, string artiklCD, string skladCD)
   {
      return GetFreeSerlotList_ForArtikl(conn, artiklCD, skladCD, /*DateTime.MinValue*/ DateTime.Today); // ispravio u 2021. al i tak se ne koristi 
   }

   internal static List<ZXC.VvUtilDataPackage> GetFreeSerlotList_ForArtikl(XSqlConnection conn, string artiklCD, string skladCD, DateTime dateDO)
   {
      List<Rtrans> artiklRtransList = new List<Rtrans>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD], "artiklCD", artiklCD, " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladCD], "skladCD", skladCD, " = "));
      if(dateDO.NotEmpty())
         filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladDate], "dateDO", dateDO, " <= "));

      VvDaoBase.LoadGenericVvDataRecordList<Rtrans>(conn, artiklRtransList, filterMembers, Rtrans.artiklOrderBy_ASC);

      var rtrSerlotGroups = artiklRtransList.GroupBy(rtr => rtr.T_serlot);

      //List<string> availableSerlots = new List<string>();
      List<ZXC.VvUtilDataPackage> availableSerlots = new List<ZXC.VvUtilDataPackage>();
      decimal serlotKolSt;

      // --- VOILA --- 
      foreach(var rtrSerlotGR in rtrSerlotGroups)
      {
         serlotKolSt = GetSerlotKolSt(rtrSerlotGR.ToArray());

         if(serlotKolSt.IsPositive()) availableSerlots.Add(new ZXC.VvUtilDataPackage(rtrSerlotGR, serlotKolSt));
      }

      return availableSerlots;
   }

   private static decimal GetSerlotKolSt(Rtrans[] rtranses)
   {
      decimal kolSt = 0M;

      foreach(Rtrans rtrans in rtranses)
      {
         if(rtrans.TtInfo.IsFinKol_U) kolSt += rtrans.R_kol;
         if(rtrans.TtInfo.IsFinKol_I) kolSt -= rtrans.R_kol;
      }

      return kolSt;
   }

   /// <summary>
   /// serlotProizvodUlazTT: zadas ZELENI npr. "PUL" (proizvod) ... da nadje svoj prvi PIZ (sirovina)
   /// </summary>
   /// <param name="conn"></param>
   /// <param name="proizvodSerlot"></param>
   /// <param name="serlotProizvodUlazTT"></param>
   /// <returns></returns>
   internal static Rtrans GetFirstSirovinaIzlazRtrans_ForProizvodSerlot(XSqlConnection conn, string proizvodSerlot, /* PUL tt: */ string serlotProizvodUlazTT)
   {
      Rtrans firstSirovinaIzlazRtrans_ForProizvodUlazSerlot;

      List<Rtrans> rtransList_ForProizvodSerlot = GetRtransList_ForSerlot(conn, proizvodSerlot);

      string serlotSirovinaIzlazTT = ZXC.TtInfo(serlotProizvodUlazTT).LinkedIzlazTT;

      firstSirovinaIzlazRtrans_ForProizvodUlazSerlot = rtransList_ForProizvodSerlot.FirstOrDefault(rtr => rtr.T_TT == serlotSirovinaIzlazTT);

      return firstSirovinaIzlazRtrans_ForProizvodUlazSerlot;
   }

   internal static List<Rtrans> GetRtransList_ForSerlot(XSqlConnection conn, string serlot)
   {
      List<Rtrans> serlotRtransList = new List<Rtrans>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_serlot], "serlot", serlot, " = "));

      VvDaoBase.LoadGenericVvDataRecordList<Rtrans>(conn, serlotRtransList, filterMembers, Rtrans.artiklOrderBy_ASC);

      #region Check: duplicate ArtiklCD

      if(serlotRtransList.GroupBy(rtr => rtr.T_artiklCD).Count() > 1)
      {
         var rtrArtiklGroups = serlotRtransList.GroupBy(rtr => rtr.T_artiklCD);

         string artikls = "";
         foreach(var rtrArtiklGR in rtrArtiklGroups)
         {
            artikls += "[" + rtrArtiklGR.First().T_artiklCD + "] " + "[" + rtrArtiklGR.First().T_artiklName + "] " /*+ rtrans.ToString()*/ + "\n";
            foreach(Rtrans rtrans in rtrArtiklGR)
            {
               artikls += rtrans.T_TT + "-" + rtrans.T_ttNum + " redak " + rtrans.T_serial + "\n";
            }
            artikls += "\n";
         }

         ZXC.aim_emsg(MessageBoxIcon.Error, "Oznaka [{0}]\n\nje korištena na više od jednog artikla!\n\n{1}", serlot, artikls);
      }

      #endregion Check: duplicate ArtiklCD

      return serlotRtransList;
   }

   internal static List<Rtrans> GetRtransList_ForTT_And_Serlot(XSqlConnection conn, string tt, string serlot)
   {
      List<Rtrans> serlotRtransList = new List<Rtrans>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt    ], "theTT" , tt    , " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_serlot], "serlot", serlot, " = "));

      VvDaoBase.LoadGenericVvDataRecordList<Rtrans>(conn, serlotRtransList, filterMembers, Rtrans.artiklOrderBy_ASC);

      #region Check: duplicate ArtiklCD

      //if(serlotRtransList.GroupBy(rtr => rtr.T_artiklCD).Count() > 1)
      //{
      //   var rtrArtiklGroups = serlotRtransList.GroupBy(rtr => rtr.T_artiklCD);
      //
      //   string artikls = "";
      //   foreach(var rtrArtiklGR in rtrArtiklGroups)
      //   {
      //      artikls += "[" + rtrArtiklGR.First().T_artiklCD + "] " + "[" + rtrArtiklGR.First().T_artiklName + "] " /*+ rtrans.ToString()*/ + "\n";
      //      foreach(Rtrans rtrans in rtrArtiklGR)
      //      {
      //         artikls += rtrans.T_TT + "-" + rtrans.T_ttNum + " redak " + rtrans.T_serial + "\n";
      //      }
      //      artikls += "\n";
      //   }
      //
      //   ZXC.aim_emsg(MessageBoxIcon.Error, "Oznaka [{0}]\n\nje korištena na više od jednog artikla!\n\n{1}", serlot, artikls);
      //}

      #endregion Check: duplicate ArtiklCD

      return serlotRtransList;
   }

   #endregion T_serlot - SerialNumber, Atest, LOT, ...

   #region Last Used URA ORG ... AND ... Get_SVD_PotrosnjaInfo

   internal static decimal GetLastUsed_URA_ORG(XSqlConnection conn, string artiklCD, string theATK, bool shouldErrorReportToFile)
   {
      List<Rtrans> URAartiklsRtransList = new List<Rtrans>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD], "artiklCD", artiklCD, " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt], "theTT", Faktur.TT_URA, " = "));

      VvDaoBase.LoadGenericVvDataRecordList<Rtrans>(conn, URAartiklsRtransList, filterMembers, Rtrans.artiklOrderBy_ASC);

      if(URAartiklsRtransList.IsEmpty()) return 0;

      #region Check: ORG inkonsistency

      URAartiklsRtransList.ForEach(rtr => rtr.CalcTransResults(null));

    //if(                     URAartiklsRtransList.GroupBy(rtr => rtr.T_doCijMal).Count() > 1)
      if(theATK.NotEmpty() && URAartiklsRtransList.GroupBy(rtr => rtr.T_doCijMal).Count() > 1)
      {
         string errorMessage;

         var rtrArtikl_ORG_Groups = URAartiklsRtransList.GroupBy(rtr => rtr.T_doCijMal);

         string ORGs = "";
         foreach(var rtrArtikl_ORG_GR in rtrArtikl_ORG_Groups)
         {
            ORGs += "ORG [" + rtrArtikl_ORG_GR.First().T_doCijMal.ToString0Vv() + "] " + "za [" + rtrArtikl_ORG_GR.First().T_artiklName + "] " /*+ rtrans.ToString()*/ + "\n";
            foreach(Rtrans rtrans in rtrArtikl_ORG_GR)
            {
             //ORGs += rtrans.T_TT + "-" + rtrans.T_ttNum + " redak " + rtrans.T_serial + "\n";
               ORGs += rtrans.T_TT + "-" + rtrans.T_ttNum + " redak " + rtrans.T_serial + " cij " + rtrans.R_CIJ_KCRP.ToStringVv() + "\n";
            }
            ORGs += "\n";
         }

         errorMessage = String.Format("Artikl [{0}] ... ATK [{2}]\n\nima nekonzistentne oznake pakiranja (ORG)!\n\n{1}\n\n=========================================================================\n", 
                           artiklCD, ORGs, theATK);

         if(shouldErrorReportToFile)
         {
            ZXC.ErrorsList.Add(errorMessage);
         }
         else
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, errorMessage);
         }
      }

      #endregion Check: duplicate ORG

      return URAartiklsRtransList.Last().T_doCijMal; // T_doCijMal is ORG 
   }

   internal static ZXC.SVD_PotrosnjaInfo Get_SVD_PotrosnjaInfo(XSqlConnection conn, string theTT, Kupdob anaKupdob_rec, DateTime dateOD, DateTime dateDO, List<Kupdob> kupdobSifrar, bool isForPeriod)
   {
      #region Create new ZXC.SVD_PotrosnjaInfo

      ZXC.SVD_PotrosnjaInfo potrosnjaInfo = new ZXC.SVD_PotrosnjaInfo
      {
         TheDateOD = dateOD,
         TheDateDO = dateDO,
         SinName = anaKupdob_rec.Ulica2,
         AnaName = anaKupdob_rec.Naziv,
         AnaLimitMM = anaKupdob_rec.FinLimit,

         SinLimitMM = kupdobSifrar.Where(kpdb => kpdb.IsMtr && kpdb.Ulica1 == anaKupdob_rec.Ulica1).Sum(kpdb => kpdb.FinLimit), // Ulica1 je: "G", "I", K", ... 1. SLOVO Klinike/Zavoda 
         SVDLimitMM = kupdobSifrar.Where(kpdb => kpdb.IsMtr).Sum(kpdb => kpdb.FinLimit), // TOTAL mjesecni LIMIT BOLNICE 
      };

      #endregion Create new ZXC.SVD_PotrosnjaInfo

      #region AnaUtrosYY, AnaUtrosMM

      List<Faktur> anaTotalYYFakturList = new List<Faktur>(); // potroseno sveukupno       
      List<Faktur> anaUtrosYYFakturList = new List<Faktur>(); // potroseno od limitiranoga 

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(5);

      // 30.03.2018: limite 'jede' samo izlaz sa skladista '10'. 

      if(isForPeriod)
      {
         filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate], "dateOD", dateOD.Date, " >= "));
      }
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate], "dateDO", dateDO.Date, " <= "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.kupdobCD], "kupdobCD", anaKupdob_rec.KupdobCD, "  = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt], "tt", /*Faktur.TT_IZD*/theTT, "  = "));
    //filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.skladCD ], "skipSkl20", "20"                  , " != "));
    //filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.skladCD ], "skipSkl70", "70"                  , " != "));

      VvDaoBase.LoadGenericVvDataRecordList<Faktur>(conn, anaTotalYYFakturList, filterMembers, "", "dokDate, ttSort, ttNum", true); // potroseno sveukupno             

      // 14.04.2022: privremeno i skl 90 dok s skoscak ne rascistimo kako cemo ubuduce 
    //filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.skladCD], "skl10only", "10", "  = "));
    //filterMembers.Add(new VvSqlFilterMember("SkladCD", "10", "  = ", ZXC.FM_OR_Enum.OPEN_OR ));
    //filterMembers.Add(new VvSqlFilterMember("SkladCD", "90", "  = ", ZXC.FM_OR_Enum.CLOSE_OR));

      // 26.04.2022: vratili na staro, Skl_10 only! 
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.skladCD], "skl10only", "10", "  = "));
      VvDaoBase.LoadGenericVvDataRecordList<Faktur>(conn, anaUtrosYYFakturList, filterMembers, "", "dokDate, ttSort, ttNum", true); // potroseno od limitiranoga       

      List<Faktur> anaTotalMMFakturList = anaTotalYYFakturList.Where(fak => fak.DokDate.Month == dateDO.Month).ToList();
      List<Faktur> anaUtrosMMFakturList = anaUtrosYYFakturList.Where(fak => fak.DokDate.Month == dateDO.Month).ToList();

      potrosnjaInfo.AnaTotalYY = anaTotalYYFakturList.Sum(fak => fak./*S_ukKC*/S_ukKCRP);
      potrosnjaInfo.AnaTotalMM = anaTotalMMFakturList.Sum(fak => fak./*S_ukKC*/S_ukKCRP);

      potrosnjaInfo.AnaUtrosYY = anaUtrosYYFakturList.Sum(fak => fak./*S_ukKC*/S_ukKCRP);
      potrosnjaInfo.AnaUtrosMM = anaUtrosMMFakturList.Sum(fak => fak./*S_ukKC*/S_ukKCRP);

      #endregion AnaUtrosYY, AnaUtrosMM

      #region SinUtrosYY, SinUtrosMM

      if(isForPeriod == false) // dakle, samo za print izdatnice. Za reporte ne.
      {
         List<Faktur> sinTotalYYFakturList = new List<Faktur>();
         List<Faktur> sinUtrosYYFakturList = new List<Faktur>();

         filterMembers = new List<VvSqlFilterMember>(5);

         string sinCD = anaKupdob_rec.Ulica1; // Ulica1 je: "G", "I", K", ... 1. SLOVO Klinike/Zavoda 
         string sinCDregExp = sinCD + "__";

         if(isForPeriod)
         {
            filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate], "dateOD", dateOD.Date, " >= "));
         }
         filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate], "dateDO", dateDO.Date, " <= "));
         filterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.kupdobTK], "kupdobTK", sinCDregExp, " LIKE ")); // !!! jes' ziher?! 
         filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt], "tt", /*Faktur.TT_IZD*/theTT, "  = "));
         //filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.skladCD ], "skipSkl20", "20"                  , " != "  ));
         //filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.skladCD ], "skipSkl70", "70"                  , " != "  ));

         VvDaoBase.LoadGenericVvDataRecordList<Faktur>(conn, sinTotalYYFakturList, filterMembers, "", "dokDate, ttSort, ttNum", true);

         filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.skladCD], "skl10only", "10", "  = "));

         VvDaoBase.LoadGenericVvDataRecordList<Faktur>(conn, sinUtrosYYFakturList, filterMembers, "", "dokDate, ttSort, ttNum", true);

         List<Faktur> sinTotalMMFakturList = sinTotalYYFakturList.Where(fak => fak.DokDate.Month == dateDO.Month).ToList();
         List<Faktur> sinUtrosMMFakturList = sinUtrosYYFakturList.Where(fak => fak.DokDate.Month == dateDO.Month).ToList();

         potrosnjaInfo.SinTotalYY = sinTotalYYFakturList.Sum(fak => fak./*S_ukKC*/S_ukKCRP);
         potrosnjaInfo.SinTotalMM = sinTotalMMFakturList.Sum(fak => fak./*S_ukKC*/S_ukKCRP);

         potrosnjaInfo.SinUtrosYY = sinUtrosYYFakturList.Sum(fak => fak./*S_ukKC*/S_ukKCRP);
         potrosnjaInfo.SinUtrosMM = sinUtrosMMFakturList.Sum(fak => fak./*S_ukKC*/S_ukKCRP);

      }

      #endregion SinUtrosYY, SinUtrosMM

      return potrosnjaInfo;
   }

   // ... inOLDyears se razlikuje od inNEWyears po tome sto u OLDy NEMAMO a u NEWyears IMAMO ProjektCD ('UGO-12345678') u Faktur data layeru. 
   // U OLDy idemo po RTRANS listu: filterMembers URA-Artikl-Kupdob-DateOD-DateDO                                                             
   // U NEWy idemo po FAKTUR listu: filterMembers URA-ProjektCD                                                                               
   internal static decimal Get_UGO_Ostvareno_For_Artikl_inTHISyear(XSqlConnection conn, string artiklCD, uint kupdobCD, DateTime dateOD, DateTime dateDO)
   {
      if(dateOD < ZXC.projectYearFirstDay) ZXC.aim_emsg(MessageBoxIcon.Error, "DatumOD je iz neke 'stare' godine?!");

      List<Rtrans> URA_RtransList_For_UGO_Artikl = Get_RtransList_For_TT_Artikl_Kupdob_Dates(conn, Faktur.TT_URA, artiklCD, kupdobCD, dateOD, dateDO);

      return URA_RtransList_For_UGO_Artikl.Sum(rtr => rtr.R_KCRP); 
   }

   internal static decimal Get_UGO_Ostvareno_For_Artikl_inOLDyears(XSqlConnection conn, /*string theTT,*/ string artiklCD, uint kupdobCD, DateTime dateOD, DateTime dateDO)
   {
      if(dateOD < ZXC.projectYearFirstDay == false) return 0.00M; // Ugovor's DateOD iz from THIS year, no need for checking prev years 

      List<Rtrans> rtransList = Get_RtransList_For_TT_Artikl_Kupdob_Dates(conn, Faktur.TT_URA, artiklCD, kupdobCD, dateOD, dateDO);

      return rtransList.Sum(rtr => rtr.R_KCRP);
   }

   internal static List<Rtrans> Get_RtransList_For_TT_Artikl_Kupdob_Dates(XSqlConnection conn, string theTT, string artiklCD, uint kupdobCD, DateTime dateOD, DateTime dateDO)
   {
      List<Rtrans> rtransList = new List<Rtrans>();

      List<VvSqlFilterMember> filterMembers = GetRtransFilterMembers_TT_ArtCD_KupdobCD_DateOD_DateDO(theTT, artiklCD, kupdobCD, dateOD, dateDO);

#if oldvej
      bool needsPrevYears = dateOD < ZXC.projectYearFirstDay;

      if(needsPrevYears)
      { 
         for(int year = dateOD.Year; year < ZXC.projectYearFirstDay.Year; ++year)
         {
            VvDaoBase.LoadGenericVvDataRecordList<Rtrans>(ZXC.TheSecondDbConn_SameDB_OtherYear(year), rtransList, filterMembers, Rtrans.artiklOrderBy_ASC);
         }
      }
      else // this year 
      {
           VvDaoBase.LoadGenericVvDataRecordList<Rtrans>(conn                                       , rtransList, filterMembers, Rtrans.artiklOrderBy_ASC);
      }
#endif

      // qweqwetrlababalan 
      // kada se ugovor proteze u buducnost (nextYear), petlju treba ograniciti da stane u 'ovoj' godini (vidi UGO-1800063, artikl 10-3498) 
    //for(int year = dateOD.Year; year <= dateDO.Year                                ; ++year)
      for(int year = dateOD.Year; year <= dateDO.Year && year <= ZXC.projectYearAsInt; ++year)
      {
         if(year < ZXC.projectYearAsInt) // neka prosla godina 
            VvDaoBase.LoadGenericVvDataRecordList<Rtrans>(ZXC.TheSecondDbConn_SameDB_OtherYear(year), rtransList, filterMembers, Rtrans.artiklOrderBy_ASC);
         else // ova godina 
            VvDaoBase.LoadGenericVvDataRecordList<Rtrans>(conn                                      , rtransList, filterMembers, Rtrans.artiklOrderBy_ASC);

      }

      if(ZXC.projectYearAsInt >= 2023)
      {
         rtransList.Where(rtr => rtr.T_skladDate < ZXC.EURoERAstart).ToList().ForEach(rtr => rtr.Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB/*<Rtrans>*/(conn));
      }

      rtransList.ForEach(rtr => rtr.CalcTransResults(null));

      return rtransList;
   }

   internal static DateTime Get_URA_MaxDate_For_UGO_Artikl(XSqlConnection conn, string artiklCD, uint kupdobCD, DateTime dateOD, DateTime dateDO)
   {
      List<VvSqlFilterMember> filterMembers = GetRtransFilterMembers_TT_ArtCD_KupdobCD_DateOD_DateDO(Faktur.TT_URA, artiklCD, kupdobCD, dateOD, dateDO);

      DateTime maxDate = GetMaxUsedValueFromDateColumn(conn, filterMembers, "t_skladDate");

      return maxDate;
   }

   private static List<VvSqlFilterMember> GetRtransFilterMembers_TT_ArtCD_KupdobCD_DateOD_DateDO(string theTT, string artiklCD, uint kupdobCD, DateTime dateOD, DateTime dateDO)
   {
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(5);

      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt       ], "tt"      , theTT   , "  = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD ], "artiklCD", artiklCD, "  = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_kupdobCD ], "kupdobCD", kupdobCD, "  = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladDate], "dateOD"  , dateOD  , " >= "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladDate], "dateDO"  , dateDO  , " <= "));

      return filterMembers;
   }

   //Get_URA_RtransList_For_UGO_Artikl_NEWyears
   //
   //INPUT - FilterMembers
   //
   //- Rtrans T_tt        = "URA"
   //- Rtrans T_artiklCD  = paramA
   //- Rtrans T_skladDate = paramB
   //- UGO Tt + UGO TtNum (ProjektCD)

   internal static List<Rtrans> Get_RtransList_For_TT_Artikl_Kupdob(XSqlConnection conn, string theTT, string artiklCD, uint kupdobCD)
   {
      List<Rtrans> rtransList = new List<Rtrans>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3/*5*/);

      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt       ], "tt"      , theTT   , "  = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD ], "artiklCD", artiklCD, "  = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_kupdobCD ], "kupdobCD", kupdobCD, "  = "));
    //filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladDate], "dateOD"  , dateOD  , " >= "));
    //filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladDate], "dateDO"  , dateDO  , " <= "));

      VvDaoBase.LoadGenericVvDataRecordList<Rtrans>(conn, rtransList, filterMembers, Rtrans.artiklOrderBy_ASC);

      rtransList.ForEach(rtr => rtr.CalcTransResults(null));

      return rtransList;
   }

   internal static List<Rtrans> Get_RtransList_For_Sklad_And_TT_And_Date(XSqlConnection conn, string skladCD, string theTT, DateTime forDate)
   {
      List<Rtrans> rtransList = new List<Rtrans>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladCD  ], "skladCD", skladCD, "  = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt       ], "theTT"  , theTT  , "  = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladDate], "forDate", forDate, "  = "));

      VvDaoBase.LoadGenericVvDataRecordList<Rtrans>(conn, rtransList, filterMembers, Rtrans.artiklOrderBy_ASC);

      rtransList.ForEach(rtr => rtr.CalcTransResults(null));

      return rtransList;
   }

   #endregion Last Used URA ORG

   #region Some PTG stuff

   internal static List<Rtrans> GetRtransList_allDOD(XSqlConnection conn, Faktur faktur_rec)
   {
      List<PTG_Ugovor> DOD_PTG_Ugovor_List = new List<PTG_Ugovor>();

      uint wantedKUG = faktur_rec.V1_ttNum;
      uint wantedUoA = faktur_rec.V2_ttNum;

      List<VvSqlFilterMember> filterMembers = PTG_OtplatniPlan.GetFilterMembers_DODfakturList(Faktur.TT_DOD, wantedKUG, wantedUoA);

      VvDaoBase.LoadGenericVvDataRecordList<PTG_Ugovor>(conn, DOD_PTG_Ugovor_List, filterMembers, "", "dokDate, ttSort, ttNum", true);

      List<Rtrans> rtransList_allDOD  = new List<Rtrans>();
      List<Rtrans> rtransList_thisDOD = new List<Rtrans>();

      foreach(PTG_Ugovor thisDOD in DOD_PTG_Ugovor_List)
      {
         rtransList_thisDOD = RtransDao.GetRtransList_ForTT_And_TtNum(conn, thisDOD.TT_And_TtNum, "", false);
         rtransList_allDOD.AddRange(rtransList_thisDOD);
      }

      return rtransList_allDOD;
   }

   #endregion Some PTG stuff
}
