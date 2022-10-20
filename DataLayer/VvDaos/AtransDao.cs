using System;
using System.Data;

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

public sealed class AtransDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static AtransDao instance;

   private AtransDao(XSqlConnection conn, string dbName) : base(dbName, Atrans.recordName, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static AtransDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new AtransDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableAtrans

   public static   uint TableVersionStatic { get { return 1; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition()
   {
      return (
         /* 00 */  "recID       int(10)     unsigned NOT NULL auto_increment,\n" +
         /* 01 */  "t_parentID  int(10)     unsigned NOT NULL,\n" +
         /* 02 */  "t_dokNum    int(10)     unsigned NOT NULL,\n" +
         /* 03 */  "t_serial    smallint(5) unsigned NOT NULL,\n" +
         /* 04 */  "t_dokDate   date                 NOT NULL default '0001-01-01',\n" +
         /* 05 */  "t_tt        char(3)              NOT NULL default '',\n" +
         /* 06 */  "t_osredCD   char(24)             NOT NULL,\n" +
         /* 07 */  "t_opis      char(32)             NOT NULL default '' COMMENT 'char(32) umjesto varchar(32) for speed',\n" +
         /* 08 */  "t_kol       decimal(8,2)         NOT NULL default '0.00',\n" +
         /* 09 */  "t_koef_am   char(3)              NOT NULL default '',\n" +
         /* 10 */  "t_amort_st  decimal(6,2)         NOT NULL default '0.00',\n" +
         /* 11 */  "t_normalAm  decimal(12,2)        NOT NULL default '0.00',\n" +
         /* 12 */  "t_dug       decimal(12,2)        NOT NULL default '0.00',\n" +
         /* 13 */  "t_pot       decimal(12,2)        NOT NULL default '0.00',\n" +

          "PRIMARY KEY                   (recID)                                                 ,\n" +
          /*"UNIQUE*/" KEY BY_LINKER     (t_parentID, t_serial)                                  ,\n" +
          /*"UNIQUE*/" KEY BY_DOKDATE    (t_dokDate,  t_dokNum,    t_serial)                     ,\n" +
          /*"UNIQUE*/" KEY BY_OSRED      (t_osredCD,  t_dokDate,   t_dokNum,  t_serial)           \n"
         );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Atrans.recordNameArhiva;
      else         tableName = Atrans.recordName;

      switch(catchingVersion)
      {
         
         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableAtrans

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool dummy)
   {
      string preffix;
      Atrans atrans = (Atrans)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, atrans.T_recID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {

      /* 01 */ VvSQL.CreateCommandParameter(cmd, preffix, atrans.T_parentID,  TheSchemaTable.Rows[CI.t_parentID]); 
      /* 02 */ VvSQL.CreateCommandParameter(cmd, preffix, atrans.T_dokNum,    TheSchemaTable.Rows[CI.t_dokNum]);
      /* 03 */ VvSQL.CreateCommandParameter(cmd, preffix, atrans.T_serial,    TheSchemaTable.Rows[CI.t_serial]);
      /* 04 */ VvSQL.CreateCommandParameter(cmd, preffix, atrans.T_dokDate,   TheSchemaTable.Rows[CI.t_dokDate]);
      /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, atrans.T_TT,        TheSchemaTable.Rows[CI.t_tt]);
      /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, atrans.T_osredCD,   TheSchemaTable.Rows[CI.t_osredCD]);
      /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, atrans.T_opis,      TheSchemaTable.Rows[CI.t_opis]);
      /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, atrans.T_kol,       TheSchemaTable.Rows[CI.t_kol]);
      /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, atrans.T_koefAm,    TheSchemaTable.Rows[CI.t_koef_am]);
      /* 10 */ VvSQL.CreateCommandParameter(cmd, preffix, atrans.T_amortSt,   TheSchemaTable.Rows[CI.t_amort_st]);
      /* 11 */ VvSQL.CreateCommandParameter(cmd, preffix, atrans.T_normalAm,  TheSchemaTable.Rows[CI.t_normalAm]);
      /* 12 */ VvSQL.CreateCommandParameter(cmd, preffix, atrans.T_dug,       TheSchemaTable.Rows[CI.t_dug]);
      /* 13 */ VvSQL.CreateCommandParameter(cmd, preffix, atrans.T_pot,       TheSchemaTable.Rows[CI.t_pot]);

      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataRow


   // OVO treba samo za find, a trans-ovi nemaju find...



//   //public void FillFromDataRow(VvDataRecord arhivedDataRecord, TypedDataSet_atrans.atransRow atransRow)
//   public void FillFromDataRow(VvDataRecord arhivedDataRecord, TypedDataSet_nalog.nalogRow atransRow)
//   {
//#if POPOPO
//      AtransStruct drData = new AtransStruct();
//      drData._recID = (uint)atransRow.prjktKupdobCD;
//      //drData._addTS = atransRow.addTS;
//      //drData._modTS = atransRow.modTS;
//      //drData._addUID = atransRow.addUID;
//      //drData._modUID = atransRow.modUID;

//      /* 01 */  drData._t_parentID  = atransRow.t_parentID    
//      /* 02 */  drData._t_dokNum    = atransRow.t_dokNum;    
//      /* 03 */  drData._t_serial    = atransRow.t_serial;     
//      /* 04 */  drData._t_dokDate   = atransRow.t_dokDate;    
//      /* 05 */  drData._t_konto     = atransRow.t_konto;      
//      /* 06 */  drData._t_kupdob_cd = atransRow.t_kupdob_cd;  
//      /* 07 */  drData._t_ticker    = atransRow.t_ticker;   
//      /* 08 */  drData._t_mtros_cd  = atransRow.t_mtros_cd;   
//      /* 09 */  drData._t_tipBr     = atransRow.t_tipBr;     
//      /* 00 */  drData._t_ime      = atransRow.t_opis;  
//      /* 11 */  drData._t_valuta    = atransRow.t_valuta;   
//      /* 12 */  drData._t_tip       = atransRow.t_tt;    
//      /* 13 */  drData._t_pdv       = atransRow.t_pdv;    
//      /* 14 */  drData._t_037       = atransRow.t_037;    
//      /* 15 */  drData._t_dokVer    = atransRow.t_dokVer; 
//      /* 16 */  drData._t_dug       = atransRow.t_dug;    
//      /* 17 */  drData._t_pot       = atransRow.t_pot;    

//      ((Atrans)arhivedDataRecord).CurrentData = drData;
//#endif
//      return;
//   }

   #endregion FillFromDataRow

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva /* dummy */)
   {
      AtransStruct rdrData = new AtransStruct();

      rdrData._recID = reader.GetUInt32(CI.recID);

      //rdrData._addTS = reader.GetDateTime(1);
      //rdrData._modTS = reader.GetDateTime(2);
      //rdrData._addUID = reader.GetString(3);
      //rdrData._modUID = reader.GetString(4);


      /* 01 */      rdrData._t_parentID   = reader.GetUInt32  (CI.t_parentID);
      /* 02 */      rdrData._t_dokNum     = reader.GetUInt32  (CI.t_dokNum);
      /* 03 */      rdrData._t_serial     = reader.GetUInt16  (CI.t_serial);
      /* 04 */      rdrData._t_skladDate    = reader.GetDateTime(CI.t_dokDate);
      /* 15 */      rdrData._t_tt         = reader.GetString  (CI.t_tt);
      /* 06 */      rdrData._t_osredCD    = reader.GetString  (CI.t_osredCD);
      /* 07 */      rdrData._t_opis       = reader.GetString  (CI.t_opis);
      /* 08 */      rdrData._t_kol        = reader.GetDecimal (CI.t_kol);
      /* 09 */      rdrData._t_koef_am    = reader.GetString  (CI.t_koef_am);
      /* 10 */      rdrData._t_amort_st   = reader.GetDecimal (CI.t_amort_st);
      /* 11 */      rdrData._t_normalAm   = reader.GetDecimal (CI.t_normalAm);
      /* 12 */      rdrData._t_dug        = reader.GetDecimal (CI.t_dug);
      /* 13 */      rdrData._t_pot        = reader.GetDecimal (CI.t_pot);

      ((Atrans)vvDataRecord).CurrentData = rdrData;

      return;
   }

   #endregion FillFromDataReader

   #region FtrCI struct & InitializeSchemaColumnIndexes()

   public struct AtransCI
   {
   internal int recID;

   /* 01 */   internal int  t_parentID;
   /* 02 */   internal int  t_dokNum;
   /* 03 */   internal int  t_serial;
   /* 04 */   internal int  t_dokDate;
   /* 05 */   internal int  t_tt;
   /* 06 */   internal int  t_osredCD;
   /* 07 */   internal int  t_opis;
   /* 08 */   internal int  t_kol;
   /* 09 */   internal int  t_koef_am;
   /* 10 */   internal int  t_amort_st;
   /* 11 */   internal int  t_normalAm;
   /* 12 */   internal int  t_dug;
   /* 13 */   internal int  t_pot;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public AtransCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID       = GetSchemaColumnIndex("recID");
      //FtrCI.addTS       = GetSchemaColumnIndex("addTS");
      //FtrCI.modTS       = GetSchemaColumnIndex("modTS");
      //FtrCI.addUID      = GetSchemaColumnIndex("addUID");
      //FtrCI.modUID      = GetSchemaColumnIndex("modUID");

      CI.t_parentID  = GetSchemaColumnIndex("t_parentID");
      CI.t_dokNum    = GetSchemaColumnIndex("t_dokNum");
      CI.t_serial    = GetSchemaColumnIndex("t_serial");
      CI.t_dokDate   = GetSchemaColumnIndex("t_dokDate");
      CI.t_tt        = GetSchemaColumnIndex("t_tt");
      CI.t_osredCD   = GetSchemaColumnIndex("t_osredCD");
      CI.t_opis      = GetSchemaColumnIndex("t_opis");
      CI.t_kol       = GetSchemaColumnIndex("t_kol");
      CI.t_koef_am   = GetSchemaColumnIndex("t_koef_am");
      CI.t_amort_st  = GetSchemaColumnIndex("t_amort_st");
      CI.t_normalAm  = GetSchemaColumnIndex("t_normalAm");
      CI.t_dug       = GetSchemaColumnIndex("t_dug");
      CI.t_pot       = GetSchemaColumnIndex("t_pot");
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()

}
