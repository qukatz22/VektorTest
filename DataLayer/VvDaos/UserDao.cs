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

public sealed class UserDao : VvDaoBase, IVvDao
{
   
   #region Singleton Constructor & instancer

   private static UserDao instance;

   private UserDao(XSqlConnection conn, string dbName) : base(dbName, User.recordNameArhiva, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static UserDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if (instance == null)
      {
         instance = new UserDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableUser

   public static   uint TableVersionStatic { get { return 5; } }

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

        "userName varchar(16) NOT NULL default '',\n" +
        "password varchar(41) NOT NULL default '' COMMENT 'len of 41 zbog MySql compat.',\n" +
        "ime      varchar(24) NOT NULL default '',\n" +
        "prezime  varchar(24) NOT NULL default '',\n" +
        "email    varchar(40) NOT NULL default '',\n" +
        "opis     varchar(80) NOT NULL default '',\n" +
        "isSuper  tinyint(1)   unsigned NOT NULL default 0,\n" +
         "oib     varchar(11)           NOT NULL default '',\n" +

         CreateTable_ArhivaExtensionDefinition(isArhiva) +

                              "PRIMARY KEY             (recID),\n" +
        (isArhiva ? "" : "UNIQUE ") + "KEY BY_USERNAME (userName),\n" +
                              "        KEY BY_PREZIME  (prezime)\n" +

          (isArhiva ? ", KEY BYqOrigRecID (origRecID)\n" : "")

      );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = User.recordNameArhiva;
      else         tableName = User.recordName;

      switch(catchingVersion)
      {
         case 2: return ("ADD COLUMN isSuper  tinyint(1)   unsigned NOT NULL default 0  AFTER opis;   \n");
         case 3: return ("ADD COLUMN oib     varchar(11)            NOT NULL default '' AFTER isSuper;\n");
         case 4: return (isArhiva ? "ADD INDEX BYqOrigRecID (origRecID)\n" : "skip_me");
         case 5: return AlterTable_LanSrvID_And_LanRecID_Columns;


         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableUser

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      User user = (User)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.ID_Only ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         //VvSQL.CreateCommandParameter(cmd, where_or_and, "prjktKupdobCD", user.RecID, XSqlDbType.Int32, 10);
         VvSQL.CreateCommandParameter(cmd, preffix, user.RecID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, user.AddTS,     TheSchemaTable.Rows[CI.addTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, user.ModTS,     TheSchemaTable.Rows[CI.modTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, user.AddUID,    TheSchemaTable.Rows[CI.addUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, user.ModUID,    TheSchemaTable.Rows[CI.modUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, user.LanSrvID,  TheSchemaTable.Rows[CI.lanSrvID]);
         VvSQL.CreateCommandParameter(cmd, preffix, user.LanRecID,  TheSchemaTable.Rows[CI.lanRecID]);

         VvSQL.CreateCommandParameter(cmd, preffix, user.UserName,              TheSchemaTable.Rows[CI.userName]);
         VvSQL.CreateCommandParameter(cmd, preffix, user.PasswdEncodedAsInFile, TheSchemaTable.Rows[CI.password]);
         VvSQL.CreateCommandParameter(cmd, preffix, user.Ime,                   TheSchemaTable.Rows[CI.ime]);
         VvSQL.CreateCommandParameter(cmd, preffix, user.Prezime,               TheSchemaTable.Rows[CI.prezime]);
         VvSQL.CreateCommandParameter(cmd, preffix, user.Email,                 TheSchemaTable.Rows[CI.email]);
         VvSQL.CreateCommandParameter(cmd, preffix, user.Opis,                  TheSchemaTable.Rows[CI.opis]);
         VvSQL.CreateCommandParameter(cmd, preffix, user.IsSuper,               TheSchemaTable.Rows[CI.isSuper]);
         VvSQL.CreateCommandParameter(cmd, preffix, user.Oib,                   TheSchemaTable.Rows[CI.oib]);

         if(isArhiva)
         {
            VvSQL.CreateCommandParameter(cmd, preffix, user.OrigRecID, TheSchemaTable.Rows[CI.origRecID]);
            VvSQL.CreateCommandParameter(cmd, preffix, user.RecVer,    TheSchemaTable.Rows[CI.recVer]);
            VvSQL.CreateCommandParameter(cmd, preffix, user.ArAction,  TheSchemaTable.Rows[CI.arAction]);
            VvSQL.CreateCommandParameter(cmd, preffix, user.ArTS,      TheSchemaTable.Rows[CI.arTS]);
            VvSQL.CreateCommandParameter(cmd, preffix, user.ArUID,     TheSchemaTable.Rows[CI.arUID]);
         }
      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva)
   {
      UserStruct rdrData = new UserStruct();

      rdrData._recID     = reader.GetUInt32  (CI.recID);
      rdrData._addTS     = reader.GetDateTime(CI.addTS);
      rdrData._modTS     = reader.GetDateTime(CI.modTS);
      rdrData._addUID    = reader.GetString  (CI.addUID);
      rdrData._modUID    = reader.GetString  (CI.modUID);
      rdrData._lanSrvID  = reader.GetUInt32  (CI.lanSrvID);
      rdrData._lanRecID  = reader.GetUInt32  (CI.lanRecID);

      rdrData._userName = reader.GetString (CI.userName);
      rdrData._password = reader.GetString (CI.password);
      rdrData._ime      = reader.GetString (CI.ime);
      rdrData._prezime  = reader.GetString (CI.prezime);
      rdrData._email    = reader.GetString (CI.email);
      rdrData._opis     = reader.GetString (CI.opis);
      rdrData._isSuper  = reader.GetBoolean(CI.isSuper);
      rdrData._oib      = reader.GetString (CI.oib);

      ((User)vvDataRecord).CurrentData = rdrData;

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

   public struct UserCI
   {
      internal int recID;
      internal int addTS;
      internal int modTS;
      internal int addUID;
      internal int modUID;
      internal int lanSrvID;
      internal int lanRecID;

      internal int userName;
      internal int password;
      internal int ime;
      internal int prezime;
      internal int email;
      internal int opis;
      internal int isSuper;
      internal int oib;

      internal int origRecID;
      internal int recVer;
      internal int arAction;
      internal int arTS;
      internal int arUID;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public UserCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID       = GetSchemaColumnIndex("recID");
      CI.addTS       = GetSchemaColumnIndex("addTS");
      CI.modTS       = GetSchemaColumnIndex("modTS");
      CI.addUID      = GetSchemaColumnIndex("addUID");
      CI.modUID      = GetSchemaColumnIndex("modUID");
      CI.lanSrvID    = GetSchemaColumnIndex("lanSrvID");
      CI.lanRecID    = GetSchemaColumnIndex("lanRecID");

      CI.userName    = GetSchemaColumnIndex("userName");
      CI.password    = GetSchemaColumnIndex("password");
      CI.ime         = GetSchemaColumnIndex("ime");
      CI.prezime     = GetSchemaColumnIndex("prezime");
      CI.email       = GetSchemaColumnIndex("email");
      CI.opis        = GetSchemaColumnIndex("opis");
      CI.isSuper     = GetSchemaColumnIndex("isSuper");
      CI.oib         = GetSchemaColumnIndex("oib");

      CI.origRecID      = GetSchemaColumnIndex("origRecID");
      CI.recVer         = GetSchemaColumnIndex("recVer");
      CI.arAction       = GetSchemaColumnIndex("arAction");
      CI.arTS           = GetSchemaColumnIndex("arTS");
      CI.arUID          = GetSchemaColumnIndex("arUID");
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()




   #region SetMe_Record specific

   public bool SetMe_Record_byUserName(XSqlConnection conn, User user_rec, string wanted_userName)
   {
      bool success = true;

      using(XSqlCommand cmd = EQLREC_byUserName_Command(conn, user_rec, wanted_userName))
      {
         using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow))
         {
            success = reader.HasRows;
            if(reader.Read()) FillFromDataReader((VvDataRecord)user_rec, reader, false);
            else success = false;
            reader.Close();
         } // using reader 
      }    // using cmd    

      if(!success)
      {
         VvSQL.ReportGeneric_DB_Error("", "UserName: [" + wanted_userName + "] u datoteci ne postoji.", System.Windows.Forms.MessageBoxButtons.OK);
      }

      return success;
   }

   private XSqlCommand EQLREC_byUserName_Command(XSqlConnection conn, User user_rec, string wanted_userName)
   {
      string preffix   = "prm_";
      string paramName = (string)TheSchemaTable.Rows[CI.userName]["ColumnName"];
      XSqlCommand cmd = VvSQL.InitCommand(conn);

      cmd.CommandText = "SELECT * FROM " + User.recordName + " WHERE " + paramName + " = ?prm_" + paramName;

      //arhivedDataRecord.SetCommandParamValues(cmd, arhivedDataRecord, ParamListType.ID_Only);
      //arhivedDataRecord.VvDao.SetCommandParamValues(cmd, arhivedDataRecord, ParamListType.ID_Only);
      VvSQL.CreateCommandParameter(cmd, preffix, wanted_userName, TheSchemaTable.Rows[CI.userName]);
      return (cmd);
   }

   #endregion SetMe_Record specific

   #region LoadPrivileges

   // PUSE

   //public void LoadUserPrivileges(XSqlConnection dbConnection, User user_rec)
   //{
   //   if(user_rec.Privileges == null) user_rec.Privileges = new List<Prvlg>();
   //   else                            user_rec.Privileges.Clear();

   //   string  prvlgUserName;
   //   DataRow drSchema;
   //   List<VvSqlFilterMember> filterMembers_1 = new List<VvSqlFilterMember>();


   //   // For this user only                                                                                                                                            
   //   drSchema      = ZXC.PrvlgDao.TheSchemaTable.Rows[ZXC.PrvlgDao.CI.userName];
   //   prvlgUserName = user_rec.UserName;

   //   filterMembers_1.Add(new VvSqlFilterMember(drSchema, "theUserName", prvlgUserName, " = "));

   //   LoadGenericVvDataRecordList<Prvlg>(dbConnection, user_rec.Privileges, filterMembers_1, "prjktTick, prjktKupdobCD");
   //}

   #endregion LoadPrivileges

   #region IsThisRecordInSomeRelation

   public override bool IsThisRecordInSomeRelation(ZXC.PrivilegedAction action, XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      bool inRelation;
      int? recCount;
      string elUser = (action == ZXC.PrivilegedAction.DELREC ? ((User)vvDataRecord).UserName : ((User)vvDataRecord).BackupedUserName);

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
      // For wanted user only                                                                                                                                            
      DataRow drSchema = ZXC.PrvlgDao.TheSchemaTable.Rows[ZXC.PrvlgDao.CI.userName];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elUser", elUser, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inRelation = true;
      else             inRelation = false;

      if(inRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));
         Issue_RecordIsInSomeRelation_Mesage(recordName, elUser, (int)recCount);
      }

      return inRelation;
   }

   #endregion IsThisRecordInSomeRelation

}

