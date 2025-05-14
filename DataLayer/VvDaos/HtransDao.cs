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

public sealed class HtransDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static HtransDao instance;

   private HtransDao(XSqlConnection conn, string dbName) : base(dbName, Htrans2.recordName, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static HtransDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new HtransDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableHtrans

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
         /* 05 */  "t_tt        char(6)              NOT NULL default '',\n" +
         /* 06 */  "t_ttNum     int(10)     unsigned NOT NULL               ,\n" +
         /* 07 */  "t_valName   char(3)              NOT NULL default ''          ,\n" +
         /* 08 */  "t_kupovni   decimal(10,6)        NOT NULL default '0.00'      ,\n" +
         /* 09 */  "t_srednji   decimal(10,6)        NOT NULL default '0.00'      ,\n" +
         /* 10 */  "t_prodajni  decimal(10,6)        NOT NULL default '0.00'      ,\n" +

          "PRIMARY KEY              (recID)                                  ,\n" +
         /*"UNIQUE*/" KEY BY_LINKER     (t_parentID, t_serial)                   ,\n" +
         /*"UNIQUE*/" KEY BY_TecForDate (t_valName,  t_dokDate)                   \n"

         // !!! NOTA BENE !!! tek si 28.0.2013 stavio da BY_LINKER i BY_TecForDate NISU UNIQUE!!!
         // Kod 'starih' korisnika je, dakle, chuspayz pa bi trebao svima rucno ubiti UNIQUE 
         // takodjer pazi kada prvi put nekome instaliras servera i kopiras staru-postojecu DevTec datoteku!!! 
         );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Htrans2.recordNameArhiva;
      else         tableName = Htrans2.recordName;

      switch(catchingVersion)
      {
         
         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableHtrans

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool dummy)
   {
      string preffix;
      Htrans2 htrans = (Htrans2)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, htrans.T_recID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {

      /* 01 */ VvSQL.CreateCommandParameter(cmd, preffix, htrans.T_parentID,  TheSchemaTable.Rows[CI.t_parentID]); 
      /* 02 */ VvSQL.CreateCommandParameter(cmd, preffix, htrans.T_dokNum,    TheSchemaTable.Rows[CI.t_dokNum]);
      /* 03 */ VvSQL.CreateCommandParameter(cmd, preffix, htrans.T_serial,    TheSchemaTable.Rows[CI.t_serial]);
      /* 04 */ VvSQL.CreateCommandParameter(cmd, preffix, htrans.T_dokDate,   TheSchemaTable.Rows[CI.t_dokDate]);
      /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, htrans.T_TT,        TheSchemaTable.Rows[CI.t_tt]);
      /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, htrans.T_ttNum,     TheSchemaTable.Rows[CI.t_ttNum]);
      /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, htrans.T_ValName ,  TheSchemaTable.Rows[CI.t_valName  ]);
      /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, htrans.T_Kupovni ,  TheSchemaTable.Rows[CI.t_kupovni  ]);
      /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, htrans.T_Srednji ,  TheSchemaTable.Rows[CI.t_srednji  ]);
      /* 00 */ VvSQL.CreateCommandParameter(cmd, preffix, htrans.T_Prodajni,  TheSchemaTable.Rows[CI.t_prodajni ]);

      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva /* dummy */)
   {
      HtransStruct rdrData = new HtransStruct();

      rdrData._recID = reader.GetUInt32(CI.recID);

      /* 01 */      rdrData._t_parentID   = reader.GetUInt32  (CI.t_parentID);
      /* 02 */      rdrData._t_dokNum     = reader.GetUInt32  (CI.t_dokNum);
      /* 03 */      rdrData._t_serial     = reader.GetUInt16  (CI.t_serial);
      /* 04 */      rdrData._t_dokDate    = reader.GetDateTime(CI.t_dokDate);
      /* 05 */      rdrData._t_tt         = reader.GetString  (CI.t_tt);
      /* 06 */      rdrData._t_ttNum      = reader.GetUInt32  (CI.t_ttNum);
      /* 07 */      rdrData._t_valName    = reader.GetString  (CI.t_valName );
      /* 08 */      rdrData._t_kupovni    = reader.GetDecimal (CI.t_kupovni );
      /* 09 */      rdrData._t_srednji    = reader.GetDecimal (CI.t_srednji );
      /* 10 */      rdrData._t_prodajni   = reader.GetDecimal (CI.t_prodajni);

      ((Htrans2)vvDataRecord).CurrentData = rdrData;

      return;
   }

   #endregion FillFromDataReader

   #region FtrCI struct & InitializeSchemaColumnIndexes()

   public struct HtransCI
   {
   internal int recID;

   /* 01 */   internal int  t_parentID;
   /* 02 */   internal int  t_dokNum;
   /* 03 */   internal int  t_serial;
   /* 04 */   internal int  t_dokDate;
   /* 05 */   internal int  t_tt;
   /* 06 */   internal int  t_ttNum;
   /* 07 */   internal int  t_valName  ;
   /* 08 */   internal int  t_kupovni  ;
   /* 09 */   internal int  t_srednji  ;
   /* 00 */   internal int  t_prodajni ;

   }

   /// <summary>
   /// HtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public HtransCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID       = GetSchemaColumnIndex("recID");

      CI.t_parentID  = GetSchemaColumnIndex("t_parentID");
      CI.t_dokNum    = GetSchemaColumnIndex("t_dokNum"  );
      CI.t_serial    = GetSchemaColumnIndex("t_serial"  );
      CI.t_dokDate   = GetSchemaColumnIndex("t_dokDate" );
      CI.t_tt        = GetSchemaColumnIndex("t_tt"      );
      CI.t_ttNum     = GetSchemaColumnIndex("t_ttNum"   );
      CI.t_valName   = GetSchemaColumnIndex("t_valName" );
      CI.t_kupovni   = GetSchemaColumnIndex("t_kupovni" );
      CI.t_srednji   = GetSchemaColumnIndex("t_srednji" );
      CI.t_prodajni  = GetSchemaColumnIndex("t_prodajni");
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()

}
