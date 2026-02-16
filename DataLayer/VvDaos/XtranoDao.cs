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
#endif

public sealed class XtranoDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static XtranoDao instance;

   private XtranoDao(XSqlConnection conn, string dbName) : base(dbName, Xtrano.recordName, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static XtranoDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new XtranoDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableXtrano

   public static   uint TableVersionStatic { get { return 7; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition()
   {
      return (
         /* 00 */  "recID        int(10)      unsigned NOT NULL auto_increment,\n" +
         /* 01 */  "t_parentID   int(10)      unsigned NOT NULL               ,\n" +
         /* 02 */  "t_dokNum     int(10)      unsigned NOT NULL               ,\n" +
         /* 03 */  "t_serial     smallint(5)  unsigned NOT NULL               ,\n" +
         /* 04 */  "t_dokDate    datetime              NOT NULL default '0001-01-01 00:00:00',\n" +
         /* 05 */  "t_tt         char(3)               NOT NULL default ''    ,\n" +
         /* 06 */  "t_ttNum      int(10)      unsigned NOT NULL               ,\n" +
         /* 07 */  "t_moneyA     decimal(12,4)         NOT NULL default '0.00',\n" +
       ///* 08 */  "t_opis_128   varchar(128)          NOT NULL default ''   ,\n"  +
         /* 08 */  "t_opis_128   varchar(4096)         NOT NULL default ''   ,\n"  +
         /* 09 */  "t_konto      varchar(64)           NOT NULL default ''   ,\n"  +
         /* 10 */  "t_devName    char(3)               NOT NULL default ''   ,\n"  +
         /* 11 */  "t_XmlZip     MEDIUMBLOB                                  ,\n"  +
         /* 12 */  "t_theString  varchar(64)           NOT NULL default ''   ,\n"  +
         /* 13 */  "t_theBool    tinyint(1)  unsigned  NOT NULL default 0    ,\n"  +
         /* 14 */  "t_dokDate2   date                  NOT NULL default '0001-01-01',\n" +


          "PRIMARY KEY                   (recID)                                                   ,\n" +
          /*"UNIQUE*/" KEY BY_LINKER     (t_parentID, t_serial)                                     \n"
         );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Xtrans.recordNameArhiva;
      else tableName = Xtrans.recordName;

      switch(catchingVersion)
      {

         case 2: return ("ADD COLUMN t_moneyA     decimal(12,4)         NOT NULL default '0.00' AFTER t_ttNum    ,  " +
                         "ADD COLUMN t_opis_128   varchar(128)          NOT NULL default ''     AFTER t_moneyA   ,  " +
                         "ADD COLUMN t_konto      varchar(8)            NOT NULL default ''     AFTER t_opis_128 ,  " +
                         "ADD COLUMN t_devName    char(3)               NOT NULL default ''     AFTER t_konto    ;\n");

         case 3: return ("MODIFY COLUMN t_opis_128  varchar(4096) NOT NULL default '';");

         case 4: return ("ADD COLUMN t_XmlZip MEDIUMBLOB AFTER t_devName;\n");

         case 5: return ("MODIFY COLUMN t_konto     varchar(64) NOT NULL default '', \n" +
                         "ADD    COLUMN t_theString varchar(64) NOT NULL default '' AFTER t_XmlZip;\n");

         case 6: return ("ADD    COLUMN t_theBool    tinyint(1)  unsigned  NOT NULL default 0 AFTER t_theString;\n");

         case 7: return ("MODIFY COLUMN t_dokDate    datetime NOT NULL default '0001-01-01 00:00:00', \n" +
                         "ADD    COLUMN t_dokDate2   date     NOT NULL default '0001-01-01' AFTER t_theBool;\n");

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }


   #endregion CreateTableXtrano

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool dummy)
   {
      string preffix;
      Xtrano xtrano = (Xtrano)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, xtrano.T_recID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {

      /* 01 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrano.T_parentID,     TheSchemaTable.Rows[CI.t_parentID ]); 
      /* 02 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrano.T_dokNum,       TheSchemaTable.Rows[CI.t_dokNum   ]);
      /* 03 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrano.T_serial,       TheSchemaTable.Rows[CI.t_serial   ]);
      /* 04 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrano.T_dokDate,      TheSchemaTable.Rows[CI.t_dokDate  ]);
      /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrano.T_TT,           TheSchemaTable.Rows[CI.t_tt       ]);
      /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrano.T_ttNum,        TheSchemaTable.Rows[CI.t_ttNum    ]);
      /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrano.T_moneyA  ,     TheSchemaTable.Rows[CI.t_moneyA   ]);
      /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrano.T_opis_128,     TheSchemaTable.Rows[CI.t_opis_128 ]);
      /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrano.T_konto   ,     TheSchemaTable.Rows[CI.t_konto    ]);
      /* 10 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrano.T_devName ,     TheSchemaTable.Rows[CI.t_devName  ]);
      /* 11 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrano.T_XmlZip  ,     TheSchemaTable.Rows[CI.t_XmlZip   ]);
      /* 12 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrano.T_theString,    TheSchemaTable.Rows[CI.t_theString]);
      /* 13 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrano.T_theBool  ,    TheSchemaTable.Rows[CI.t_theBool  ]);
      /* 14 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrano.T_dokDate2,     TheSchemaTable.Rows[CI.t_dokDate2 ]);
      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva /* dummy */)
   {
      XtranoStruct rdrData = new XtranoStruct();

      rdrData._recID = reader.GetUInt32(CI.recID);

      //rdrData._addTS = reader.GetDateTime(1);
      //rdrData._modTS = reader.GetDateTime(2);
      //rdrData._addUID = reader.GetString(3);
      //rdrData._modUID = reader.GetString(4);


      /* 01 */ rdrData._t_parentID   = reader.GetUInt32  (CI.t_parentID );
      /* 02 */ rdrData._t_dokNum     = reader.GetUInt32  (CI.t_dokNum   );
      /* 03 */ rdrData._t_serial     = reader.GetUInt16  (CI.t_serial   );
      /* 04 */ rdrData._t_dokDate    = reader.GetDateTime(CI.t_dokDate  );
      /* 05 */ rdrData._t_tt         = reader.GetString  (CI.t_tt       );
      /* 06 */ rdrData._t_ttNum      = reader.GetUInt32  (CI.t_ttNum    );
      /* 07 */ rdrData._t_moneyA     = reader.GetDecimal (CI.t_moneyA   );
      /* 08 */ rdrData._t_opis_128   = reader.GetString  (CI.t_opis_128 );
      /* 09 */ rdrData._t_konto      = reader.GetString  (CI.t_konto    );
      /* 10 */ rdrData._t_devName    = reader.GetString  (CI.t_devName  );

      /* 11 */ rdrData._t_XmlZip     = reader.IsDBNull(CI.t_XmlZip) ? null : (byte[])reader.GetValue(CI.t_XmlZip);
      /* 12 */ rdrData._t_theString  = reader.GetString  (CI.t_theString);
      /* 13 */ rdrData._t_theBool    = reader.GetBoolean (CI.t_theBool  );
      /* 14 */ rdrData._t_dokDate2   = reader.GetDateTime(CI.t_dokDate2 );

      ((Xtrano)vvDataRecord).CurrentData = rdrData;

      // NE !: ((Xtrano)vvDataRecord).CalcTransResults();
      
      return;
   }

   #endregion FillFromDataReader

   #region FtrCI struct & InitializeSchemaColumnIndexes()

   public struct XtranoCI
   {
      internal int recID;

      /* 01 */  internal int t_parentID;
      /* 02 */  internal int t_dokNum;
      /* 03 */  internal int t_serial;
      /* 04 */  internal int t_dokDate;
      /* 05 */  internal int t_tt;
      /* 06 */  internal int t_ttNum;
      /* 07 */  internal int t_moneyA;
      /* 08 */  internal int t_opis_128;
      /* 09 */  internal int t_konto;
      /* 10 */  internal int t_devName;
      /* 11 */  internal int t_XmlZip;
      /* 12 */  internal int t_theString;
      /* 13 */  internal int t_theBool;
      /* 14 */  internal int t_dokDate2;

   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public XtranoCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID       = GetSchemaColumnIndex("recID");
      //FtrCI.addTS       = GetSchemaColumnIndex("addTS");
      //FtrCI.modTS       = GetSchemaColumnIndex("modTS");
      //FtrCI.addUID      = GetSchemaColumnIndex("addUID");
      //FtrCI.modUID      = GetSchemaColumnIndex("modUID");

      /* 01 */ CI.t_parentID     = GetSchemaColumnIndex("t_parentID" );
      /* 02 */ CI.t_dokNum       = GetSchemaColumnIndex("t_dokNum"   );
      /* 03 */ CI.t_serial       = GetSchemaColumnIndex("t_serial"   );
      /* 04 */ CI.t_dokDate      = GetSchemaColumnIndex("t_dokDate"  );
      /* 05 */ CI.t_tt           = GetSchemaColumnIndex("t_tt"       );
      /* 06 */ CI.t_ttNum        = GetSchemaColumnIndex("t_ttNum"    );
      /* 07 */ CI.t_moneyA       = GetSchemaColumnIndex("t_moneyA"   );
      /* 08 */ CI.t_opis_128     = GetSchemaColumnIndex("t_opis_128" );
      /* 09 */ CI.t_konto        = GetSchemaColumnIndex("t_konto"    );
      /* 10 */ CI.t_devName      = GetSchemaColumnIndex("t_devName"  );
      /* 11 */ CI.t_XmlZip       = GetSchemaColumnIndex("t_XmlZip"   );
      /* 12 */ CI.t_theString    = GetSchemaColumnIndex("t_theString"); 
      /* 13 */ CI.t_theBool      = GetSchemaColumnIndex("t_theBool"  ); 
      /* 14 */ CI.t_dokDate2     = GetSchemaColumnIndex("t_dokDate2" );
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()

   public static List<Xtrano> Get_Prijavljeno_MAP_XtranoList_For_FakRecID(XSqlConnection conn, uint fakRecID)
   {
      bool success = true;
      Xtrano MAP_xtrano_rec = new Xtrano();
      List<Xtrano> MAP_XtranoList = new List<Xtrano>();

      ZXC.sqlErrNo = 0;

      if(fakRecID.IsZero()) return MAP_XtranoList;

      using(XSqlCommand cmd = (VvSQL.Get_Prijavljeno_MAP_XtranoList_For_FakRecID_Command(conn, fakRecID)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;

               while(success && reader.Read())
               {
                  MAP_xtrano_rec = new Xtrano();

                  ZXC.XtranoDao.FillFromDataReader(MAP_xtrano_rec, reader, false);

                  MAP_XtranoList.Add(MAP_xtrano_rec);

                  //string pero = VvStringCompressor.DecompressXml(MAP_xtrano_rec.T_XmlZip);

               }
               reader.Close();
            }
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("Get_Prijavljeno_MAP_XtranoList_For_FakRecID", ex, System.Windows.Forms.MessageBoxButtons.OK);

            ZXC.sqlErrNo = ex.Number;
         }
      } // using 

      return MAP_XtranoList;
   }

   public static /*bool*/Xtrano SetMe_MAP_XtranoForThis_FtransRecID(XSqlConnection conn, uint ftransRecID)
   {
      bool success = true;

      Xtrano MAPxtrano_rec = new Xtrano();

      using(XSqlCommand cmd = VvSQL.SetMe_MAP_XtranoForThis_FtransRecID_Command(conn, ftransRecID))
      {
         success = ZXC.XtranoDao.ExecuteSingleFillFromDataReader(MAPxtrano_rec, false, cmd, false);
      } 


      if(!success) return null;
      else         return MAPxtrano_rec;
   }

}
