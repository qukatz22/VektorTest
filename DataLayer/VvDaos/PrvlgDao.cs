using System;
using System.Data;
using System.Collections.Generic;

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

public sealed class PrvlgDao : VvDaoBase, IVvDao
{
   
   #region Singleton Constructor & instancer

   private static PrvlgDao instance;

   private PrvlgDao(XSqlConnection conn, string dbName) : base(dbName, Prvlg.recordNameArhiva, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static PrvlgDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if (instance == null)
      {
         instance = new PrvlgDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTablePrvlg

   public static   uint TableVersionStatic { get { return 2; } } 

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition(bool isArhiva)
   {
      return 
      (
        "recID int(10) unsigned NOT NULL auto_increment,\n" +
        "addTS timestamp                 NULL DEFAULT NULL,\n" +
        "modTS timestamp                 default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,\n" +
        "addUID varchar(16)     NOT NULL default 'XY',\n" +
        "modUID varchar(16)     NOT NULL default '',\n" +
        CreateTable_LanSrvID_And_LanRecID_Columns +

        "prjktID    int(10)     NOT NULL default 0,\n" +
        "prjktTick  char(6)     NOT NULL default '',\n" +
        "userName   varchar(16) NOT NULL default '',\n" +
        "prvlgScope varchar(50) NOT NULL default '',\n" +
        "prvlgType  varchar(30) NOT NULL default '',\n" +
        "documType  varchar(6)  NOT NULL default '',\n" +

         CreateTable_ArhivaExtensionDefinition(isArhiva) +

                              "PRIMARY KEY               (recID),\n" +
        (isArhiva ? "" : "UNIQUE ") + "KEY BY_PRJKT_TICK (prjktTick, recID),\n" +
        (isArhiva ? "" : "UNIQUE ") + "KEY BY_USER_NAME  (userName,  recID)\n" +

          (isArhiva ? ", KEY BYqOrigRecID (origRecID)\n" : "")

      );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Prvlg.recordNameArhiva;
      else         tableName = Prvlg.recordName;

      switch(catchingVersion)
      {
         case 2: return AlterTable_LanSrvID_And_LanRecID_Columns;

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTablePrvlg

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      Prvlg prvlg = (Prvlg)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.ID_Only ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         //VvSQL.CreateCommandParameter(cmd, where_or_and, "prjktKupdobCD", prvlg.RecID, XSqlDbType.Int32, 10);
         VvSQL.CreateCommandParameter(cmd, preffix, prvlg.RecID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, prvlg.AddTS,     TheSchemaTable.Rows[CI.addTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, prvlg.ModTS,     TheSchemaTable.Rows[CI.modTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, prvlg.AddUID,    TheSchemaTable.Rows[CI.addUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, prvlg.ModUID,    TheSchemaTable.Rows[CI.modUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, prvlg.LanSrvID,  TheSchemaTable.Rows[CI.lanSrvID]);
         VvSQL.CreateCommandParameter(cmd, preffix, prvlg.LanRecID,  TheSchemaTable.Rows[CI.lanRecID]);

         VvSQL.CreateCommandParameter(cmd, preffix, prvlg.PrjktID,    TheSchemaTable.Rows[CI.prjktID]);
         VvSQL.CreateCommandParameter(cmd, preffix, prvlg.PrjktTick,  TheSchemaTable.Rows[CI.prjktTick]);
         VvSQL.CreateCommandParameter(cmd, preffix, prvlg.UserName,   TheSchemaTable.Rows[CI.userName]);
         VvSQL.CreateCommandParameter(cmd, preffix, prvlg.PrvlgScope, TheSchemaTable.Rows[CI.prvlgScope]);
         VvSQL.CreateCommandParameter(cmd, preffix, prvlg.PrvlgType,  TheSchemaTable.Rows[CI.prvlgType]);
         VvSQL.CreateCommandParameter(cmd, preffix, prvlg.DocumType,  TheSchemaTable.Rows[CI.documType]);

         if(isArhiva)
         {
            VvSQL.CreateCommandParameter(cmd, preffix, prvlg.OrigRecID, TheSchemaTable.Rows[CI.origRecID]);
            VvSQL.CreateCommandParameter(cmd, preffix, prvlg.RecVer,    TheSchemaTable.Rows[CI.recVer]);
            VvSQL.CreateCommandParameter(cmd, preffix, prvlg.ArAction,  TheSchemaTable.Rows[CI.arAction]);
            VvSQL.CreateCommandParameter(cmd, preffix, prvlg.ArTS,    TheSchemaTable.Rows[CI.arTS]);
            VvSQL.CreateCommandParameter(cmd, preffix, prvlg.ArUID,     TheSchemaTable.Rows[CI.arUID]);
         }
      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva)
   {
      PrvlgStruct rdrData = new PrvlgStruct();

      rdrData._recID     = reader.GetUInt32  (CI.recID);
      rdrData._addTS     = reader.GetDateTime(CI.addTS);
      rdrData._modTS     = reader.GetDateTime(CI.modTS);
      rdrData._addUID    = reader.GetString  (CI.addUID);
      rdrData._modUID    = reader.GetString  (CI.modUID);
      rdrData._lanSrvID  = reader.GetUInt32  (CI.lanSrvID);
      rdrData._lanRecID  = reader.GetUInt32  (CI.lanRecID);

      rdrData._prjktID    = reader.GetUInt32(CI.prjktID);
      rdrData._prjktTick  = reader.GetString(CI.prjktTick);
      rdrData._userName   = reader.GetString(CI.userName);
      rdrData._prvlgScope = reader.GetString(CI.prvlgScope);
      rdrData._prvlgType  = reader.GetString(CI.prvlgType);
      rdrData._documType  = reader.GetString(CI.documType);

      ((Prvlg)vvDataRecord).CurrentData = rdrData;

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

   public struct PrvlgCI
   {
      internal int recID;
      internal int addTS;
      internal int modTS;
      internal int addUID;
      internal int modUID;
      internal int lanSrvID;
      internal int lanRecID;

      internal int prjktID;
      internal int prjktTick;
      internal int userName;
      internal int prvlgScope;
      internal int prvlgType;
      internal int documType;

      internal int origRecID;
      internal int recVer;
      internal int arAction;
      internal int arTS;
      internal int arUID;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public PrvlgCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID       = GetSchemaColumnIndex("recID");
      CI.addTS       = GetSchemaColumnIndex("addTS");
      CI.modTS       = GetSchemaColumnIndex("modTS");
      CI.addUID      = GetSchemaColumnIndex("addUID");
      CI.modUID      = GetSchemaColumnIndex("modUID");
      CI.lanSrvID    = GetSchemaColumnIndex("lanSrvID");
      CI.lanRecID    = GetSchemaColumnIndex("lanRecID");

      CI.prjktID    = GetSchemaColumnIndex("prjktID");
      CI.prjktTick  = GetSchemaColumnIndex("prjktTick");
      CI.userName   = GetSchemaColumnIndex("userName");
      CI.prvlgScope = GetSchemaColumnIndex("prvlgScope");
      CI.prvlgType  = GetSchemaColumnIndex("prvlgType");
      CI.documType  = GetSchemaColumnIndex("documType");

      CI.origRecID      = GetSchemaColumnIndex("origRecID");
      CI.recVer         = GetSchemaColumnIndex("recVer");
      CI.arAction       = GetSchemaColumnIndex("arAction");
      CI.arTS           = GetSchemaColumnIndex("arTS");
      CI.arUID          = GetSchemaColumnIndex("arUID");
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()






   #region Load_ZXC_Privileges

   public void Load_ZXC_Privileges(XSqlConnection conn, uint prjktID, string userName)
   {
      if(ZXC.CURR_Privileges == null) ZXC.CURR_Privileges = new List<Prvlg>();
      else                            ZXC.CURR_Privileges.Clear();

      uint    theKupdobCD;
      string  text;

      DataRow drSchema;
      System.Collections.Generic.List<VvSqlFilterMember> filterMembers = new System.Collections.Generic.List<VvSqlFilterMember>(2);


      // For CURR_Prjkt only                                                                                                                                          
      drSchema    = this.TheSchemaTable.Rows[CI.prjktID];
      theKupdobCD = ZXC.CURR_prjkt_rec.KupdobCD/*RecID*/;

      filterMembers.Add(new VvSqlFilterMember(drSchema, "CURR_prjkt_rec.KupdobCD", theKupdobCD, " = "));

      // For CURR_user only                                                                                                                                            
      drSchema = this.TheSchemaTable.Rows[CI.userName];
      text     = ZXC.CURR_userName;

      filterMembers.Add(new VvSqlFilterMember(drSchema, "CURR_userName", text, " = "));

      LoadGenericVvDataRecordList<Prvlg>(conn, ZXC.CURR_Privileges, filterMembers, "prvlgType, prvlgScope"); 
      
   }

   #endregion Load_ZXC_Privileges
}

