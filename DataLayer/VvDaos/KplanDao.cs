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

public sealed class KplanDao : VvDaoBase, IVvDao
{
   
   #region Singleton Constructor & instancer

   private static KplanDao instance;

   private KplanDao(XSqlConnection conn, string dbName) : base(dbName, Kplan.recordNameArhiva, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static KplanDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if (instance == null)
      {
         instance = new KplanDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region Sceppa's generated data adapter code

   //public static MySqlDataAdapter CreateSceppaDataAdapter()
   //{
   //   MySqlCommand selectCommand = new MySqlCommand(@"SELECT * FROM KPLAN ", ZXC.TheVvForm.conn);
   //   return CreateSceppaDataAdapter(selectCommand);
   //}

   //public static MySqlDataAdapter CreateSceppaDataAdapter(string selectCommandText)
   //{
   //   MySqlCommand selectCommand = new MySqlCommand(selectCommandText, ZXC.TheVvForm.conn);
   //   return CreateSceppaDataAdapter(selectCommand);
   //}

   //public static MySqlDataAdapter CreateSceppaDataAdapter(MySqlCommand selectCommand)
   //{
   //   MySqlDataAdapter da = new MySqlDataAdapter(selectCommand);

   //   System.Data.Common.DataTableMapping dtm;
   //   dtm = da.TableMappings.Add("Table", "KPLAN");
   //   dtm.ColumnMappings.Add("kontoID", "kontoID");
   //   dtm.ColumnMappings.Add("konto", "konto");
   //   dtm.ColumnMappings.Add("ts", "ts");
   //   dtm.ColumnMappings.Add("naziv", "naziv");

   //   MySqlCommand cmd;
   //   da.UpdateCommand = da.SelectCommand.Connection.CreateCommand();
   //   cmd = da.UpdateCommand;
   //   cmd.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
   //   cmd.CommandText = @"UPDATE KPLAN SET konto = ?prm_konto_Curr, ts = ?prm_tip_Curr, naziv = ?prm_naziv_Curr WHERE kontoID = ?prm_kontoID_Orig AND konto = ?prm_konto_Orig AND ts = ?prm_tip_Orig AND naziv = ?prm_naziv_Orig; SELECT * FROM KPLAN WHERE kontoID = ?prm_kontoID_Refresh";
   //   cmd.Parameters.Add(new MySqlParameter("konto_Curr", MySqlDbType.VarChar, 6, ParameterDirection.Input, false, 0, 0, "konto", DataRowVersion.Current, null));
   //   cmd.Parameters.Add(new MySqlParameter("tip_Curr", MySqlDbType.String, 1, ParameterDirection.Input, false, 0, 0, "ts", DataRowVersion.Current, null));
   //   cmd.Parameters.Add(new MySqlParameter("naziv_Curr", MySqlDbType.VarChar, 30, ParameterDirection.Input, false, 0, 0, "naziv", DataRowVersion.Current, null));
   //   cmd.Parameters.Add(new MySqlParameter("kontoID_Orig", MySqlDbType.Int32, 10, ParameterDirection.Input, false, 0, 0, "kontoID", DataRowVersion.Original, null));
   //   cmd.Parameters.Add(new MySqlParameter("konto_Orig", MySqlDbType.VarChar, 6, ParameterDirection.Input, false, 0, 0, "konto", DataRowVersion.Original, null));
   //   cmd.Parameters.Add(new MySqlParameter("tip_Orig", MySqlDbType.String, 1, ParameterDirection.Input, false, 0, 0, "ts", DataRowVersion.Original, null));
   //   cmd.Parameters.Add(new MySqlParameter("naziv_Orig", MySqlDbType.VarChar, 30, ParameterDirection.Input, false, 0, 0, "naziv", DataRowVersion.Original, null));
   //   cmd.Parameters.Add(new MySqlParameter("kontoID_Refresh", MySqlDbType.Int32, 10, ParameterDirection.Input, false, 0, 0, "kontoID", DataRowVersion.Current, null));

   //   da.InsertCommand = da.SelectCommand.Connection.CreateCommand();
   //   cmd = da.InsertCommand;
   //   cmd.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
   //   cmd.CommandText = @"INSERT INTO KPLAN (konto, ts, naziv) VALUES (?prm_konto, ?prm_tip, ?prm_naziv); SELECT * FROM KPLAN WHERE kontoID = @@IDENTITY";
   //   cmd.Parameters.Add(new MySqlParameter("konto", MySqlDbType.VarChar, 6, ParameterDirection.Input, false, 0, 0, "konto", DataRowVersion.Current, null));
   //   cmd.Parameters.Add(new MySqlParameter("ts", MySqlDbType.String, 1, ParameterDirection.Input, false, 0, 0, "ts", DataRowVersion.Current, null));
   //   cmd.Parameters.Add(new MySqlParameter("naziv", MySqlDbType.VarChar, 30, ParameterDirection.Input, false, 0, 0, "naziv", DataRowVersion.Current, null));

   //   da.DeleteCommand = da.SelectCommand.Connection.CreateCommand();
   //   cmd = da.DeleteCommand;
   //   cmd.UpdatedRowSource = UpdateRowSource.None;
   //   cmd.CommandText = @"DELETE FROM KPLAN WHERE kontoID = ?prm_kontoID AND konto = ?prm_konto AND ts = ?prm_tip AND naziv = ?prm_naziv";
   //   cmd.Parameters.Add(new MySqlParameter("kontoID", MySqlDbType.Int32, 10, ParameterDirection.Input, false, 0, 0, "kontoID", DataRowVersion.Original, null));
   //   cmd.Parameters.Add(new MySqlParameter("konto", MySqlDbType.VarChar, 6, ParameterDirection.Input, false, 0, 0, "konto", DataRowVersion.Original, null));
   //   cmd.Parameters.Add(new MySqlParameter("ts", MySqlDbType.String, 1, ParameterDirection.Input, false, 0, 0, "ts", DataRowVersion.Original, null));
   //   cmd.Parameters.Add(new MySqlParameter("naziv", MySqlDbType.VarChar, 30, ParameterDirection.Input, false, 0, 0, "naziv", DataRowVersion.Original, null));

   //   return da;
   //}

   #endregion Sceppa's generated data adapter code

   #region CreateTableKplan

   public static   uint TableVersionStatic { get { return 7; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition(bool isArhiva)
   {
      return 
      (
        "recID int(10) unsigned NOT NULL auto_increment,\n" +
        "addTS timestamp                 NULL DEFAULT NULL,\n" +
        "modTS timestamp                 default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,\n" +
        "addUID varchar(16)     NOT NULL default 'XY',\n" +
        "modUID varchar(16)     NOT NULL default '',\n"   +
        CreateTable_LanSrvID_And_LanRecID_Columns         +

        "konto     varchar(9)      NOT NULL default '',\n" +
        "tip       char(1)         NOT NULL default '',\n" +
        "naziv     varchar(64)     NOT NULL default '',\n" +
        "psRule    tinyint(1)      unsigned NOT NULL default 0,\n" +
        "anaGr     varchar(30)     NOT NULL default '',\n" +
        "opis      varchar(64)     NOT NULL default '',\n" +
        "naziv2    varchar(64)     NOT NULL default '',\n" +
        "naziv3    varchar(64)     NOT NULL default '',\n" +
        "fond      char(1)         NOT NULL default '',\n" +
         CreateTable_ArhivaExtensionDefinition(isArhiva) +

                              "PRIMARY KEY  (recID),\n" +
        (isArhiva ? "" : "UNIQUE ") + "KEY BY_KONTO (konto),\n" +
                                      "KEY BY_NAZIV (naziv)\n"  +
          (isArhiva ? ", KEY BYqOrigRecID (origRecID)\n" : "")
      );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Kplan.recordNameArhiva;
      else         tableName = Kplan.recordName;

      string dbName = VvSQL.GetDbNameForThisTableName(tableName);


      switch(catchingVersion)
      {  
         case 2: return ("MODIFY COLUMN naziv  VARCHAR(64) NOT NULL default '',            " +
                         "MODIFY COLUMN opis   VARCHAR(64) NOT NULL default '',            " + 
                         "ADD    COLUMN naziv2 VARCHAR(64) NOT NULL DEFAULT '' AFTER opis, " +
                         "ADD    COLUMN naziv3 VARCHAR(64) NOT NULL DEFAULT '' AFTER naziv2");

         case 3: return ("MODIFY COLUMN konto  varchar(8) NOT NULL default '';             ");

         case 4: return (isArhiva ? "ADD INDEX BYqOrigRecID (origRecID)\n" : "skip_me");

         case 5: return AlterTable_LanSrvID_And_LanRecID_Columns;

         case 6: return ("ADD COLUMN fond      char(1)         NOT NULL default '' AFTER naziv3;");

         case 7: return ("MODIFY COLUMN konto  varchar(9) NOT NULL default '';             ");

         default: throw new Exception("For table " + Kupdob.recordName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableKplan

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      Kplan kplan = (Kplan)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.ID_Only ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         //VvSQL.CreateCommandParameter(cmd, where_or_and, "prjktKupdobCD", kplan.RecID, XSqlDbType.Int32, 10);
         VvSQL.CreateCommandParameter(cmd, preffix, kplan.RecID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         //DataRow theRow = TheSchemaTable.Rows.Find("modUID");

         VvSQL.CreateCommandParameter(cmd, preffix, kplan.AddTS,     TheSchemaTable.Rows[CI.addTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, kplan.ModTS,     TheSchemaTable.Rows[CI.modTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, kplan.AddUID,    TheSchemaTable.Rows[CI.addUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, kplan.ModUID,    TheSchemaTable.Rows[CI.modUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, kplan.LanSrvID,  TheSchemaTable.Rows[CI.lanSrvID]);
         VvSQL.CreateCommandParameter(cmd, preffix, kplan.LanRecID,  TheSchemaTable.Rows[CI.lanRecID]);

         VvSQL.CreateCommandParameter(cmd, preffix, kplan.Konto,     TheSchemaTable.Rows[CI.konto]);
         VvSQL.CreateCommandParameter(cmd, preffix, kplan.Tip,       TheSchemaTable.Rows[CI.tip]);
         VvSQL.CreateCommandParameter(cmd, preffix, kplan.Naziv,     TheSchemaTable.Rows[CI.naziv]);
         VvSQL.CreateCommandParameter(cmd, preffix, kplan.PsRule,    TheSchemaTable.Rows[CI.psRule]);
         VvSQL.CreateCommandParameter(cmd, preffix, kplan.AnaGr,     TheSchemaTable.Rows[CI.anaGr]);
         VvSQL.CreateCommandParameter(cmd, preffix, kplan.Opis,      TheSchemaTable.Rows[CI.opis]);
         VvSQL.CreateCommandParameter(cmd, preffix, kplan.Naziv2,    TheSchemaTable.Rows[CI.naziv2]);
         VvSQL.CreateCommandParameter(cmd, preffix, kplan.Naziv3,    TheSchemaTable.Rows[CI.naziv3]);
         VvSQL.CreateCommandParameter(cmd, preffix, kplan.Fond  ,    TheSchemaTable.Rows[CI.fond  ]);

         if(isArhiva)
         {
            VvSQL.CreateCommandParameter(cmd, preffix, kplan.OrigRecID, TheSchemaTable.Rows[CI.origRecID]);
            VvSQL.CreateCommandParameter(cmd, preffix, kplan.RecVer,    TheSchemaTable.Rows[CI.recVer]);
            VvSQL.CreateCommandParameter(cmd, preffix, kplan.ArAction,  TheSchemaTable.Rows[CI.arAction]);
            VvSQL.CreateCommandParameter(cmd, preffix, kplan.ArTS,      TheSchemaTable.Rows[CI.arTS]);
            VvSQL.CreateCommandParameter(cmd, preffix, kplan.ArUID,     TheSchemaTable.Rows[CI.arUID]);
         }
      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataRow

   //public void FillFromDataRow(VvDataRecord arhivedDataRecord,  Vektor.DataLayer.DS_kplan.kplanRow kplanRow)
   //{
   //   KplanStruct drData = new KplanStruct();

   //   drData._recID     = (uint)kplanRow.prjktKupdobCD;
   //   drData._addTS     = kplanRow.addTS;
   //   drData._modTS     = kplanRow.modTS;
   //   drData._addUID    = kplanRow.addUID;
   //   drData._modUID    = kplanRow.modUID;
   //   drData._konto     = kplanRow.konto;
   //   drData._ts       = kplanRow.ts;
   //   drData._naziv     = kplanRow.naziv;

   //   ((Kplan)arhivedDataRecord).CurrentData = drData;

   //   return;
   //}

   #endregion FillFromDataRow

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva)
   {
      KplanStruct rdrData = new KplanStruct();

      rdrData._recID     = reader.GetUInt32  (CI.recID   );
      rdrData._addTS     = reader.GetDateTime(CI.addTS   );
      rdrData._modTS     = reader.GetDateTime(CI.modTS   );
      rdrData._addUID    = reader.GetString  (CI.addUID  );
      rdrData._modUID    = reader.GetString  (CI.modUID  );
      rdrData._lanSrvID  = reader.GetUInt32  (CI.lanSrvID);
      rdrData._lanRecID  = reader.GetUInt32  (CI.lanRecID);

      rdrData._konto     = reader.GetString  (CI.konto );
      rdrData._tip       = reader.GetString  (CI.tip   );
      rdrData._naziv     = reader.GetString  (CI.naziv );
      rdrData._psRule    = reader.GetUInt16  (CI.psRule);
      rdrData._anaGr     = reader.GetString  (CI.anaGr );
      rdrData._opis      = reader.GetString  (CI.opis  );
      rdrData._naziv2    = reader.GetString  (CI.naziv2);
      rdrData._naziv3    = reader.GetString  (CI.naziv3);
      rdrData._fond      = reader.GetString  (CI.fond  );

      ((Kplan)vvDataRecord).CurrentData = rdrData;

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

   public struct KplanCI
   {
      internal int recID;
      internal int addTS;
      internal int modTS;
      internal int addUID;
      internal int modUID;
      internal int lanSrvID;
      internal int lanRecID;

      internal int konto;
      internal int tip;
      internal int naziv;
      internal int psRule;
      internal int anaGr;
      internal int opis;
      internal int naziv2;
      internal int naziv3;
      internal int fond  ;

      internal int origRecID;
      internal int recVer;
      internal int arAction;
      internal int arTS;
      internal int arUID;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public KplanCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID       = GetSchemaColumnIndex("recID");
      CI.addTS       = GetSchemaColumnIndex("addTS");
      CI.modTS       = GetSchemaColumnIndex("modTS");
      CI.addUID      = GetSchemaColumnIndex("addUID");
      CI.modUID      = GetSchemaColumnIndex("modUID");
      CI.lanSrvID    = GetSchemaColumnIndex("lanSrvID");
      CI.lanRecID    = GetSchemaColumnIndex("lanRecID");
      CI.konto       = GetSchemaColumnIndex("konto");
      CI.tip         = GetSchemaColumnIndex("tip");
      CI.naziv       = GetSchemaColumnIndex("naziv");
      CI.psRule      = GetSchemaColumnIndex("psRule");
      CI.anaGr       = GetSchemaColumnIndex("anaGr");
      CI.opis        = GetSchemaColumnIndex("opis");
      CI.naziv2      = GetSchemaColumnIndex("naziv2");
      CI.naziv3      = GetSchemaColumnIndex("naziv3");
      CI.fond        = GetSchemaColumnIndex("fond");

      CI.origRecID      = GetSchemaColumnIndex("origRecID");
      CI.recVer         = GetSchemaColumnIndex("recVer");
      CI.arAction       = GetSchemaColumnIndex("arAction");
      CI.arTS           = GetSchemaColumnIndex("arTS");
      CI.arUID          = GetSchemaColumnIndex("arUID");
}

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()




   #region LoadFtranses


   // Postao 'PUSE'

   //public void LoadKplanFtranses(XSqlConnection dbConnection, Kplan kplan_rec)
   //{
   //   if(kplan_rec.Ftranses == null) kplan_rec.Ftranses = new List<Ftrans>();
   //   else                           kplan_rec.Ftranses.Clear();

   //   string  kplanKonto;
   //   DataRow drSchema;

   //   List<VvSqlFilterMember> filterMembers_1 = new System.Collections.Generic.List<VvSqlFilterMember>(1);
      
   //   // For this Kplan only                                                                                                                                            
   //   drSchema   = ZXC.FtransDao.TheSchemaTable.Rows[ZXC.FtransDao.CI.t_konto];
   //   kplanKonto = kplan_rec.Konto;

   //   filterMembers_1.Add(new VvSqlFilterMember(drSchema, "theKonto", kplanKonto, " = "));

   //   LoadGenericVvDataRecordList<Ftrans>(dbConnection, kplan_rec.Ftranses, filterMembers_1, "t_dokDate DESC, t_dokNum DESC, t_serial DESC");
   //}

   #endregion LoadFtranses

   #region GetKplanList_ForKlasa

   // !!! 
   // Ovo bi ti zapravo trebalo biti Obsolete/Sepsolete. Bolja fora je:           
   // List<Kplan> kplanList;                                                      
   // TheVvRecordUC.SetSifrarAndAutocomplete<Kplan>(null, VvSQL.SorterType_Dokument.None); 
   // kplanList = TheVvUC.KplanSifrar;                                            
   // !!! 

   public static List<Kplan> GetKplanList_ForKlasa(XSqlConnection conn, string klasa)
   {
      List<Kplan> listaKonta = new List<Kplan>();
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);
      DataRow drSchema;

      // For konta klase '4' only                                                                                                                                            
      drSchema = ZXC.KplanDao.TheSchemaTable.Rows[ZXC.KplanDao.CI.konto];

      filterMembers.Add(new VvSqlFilterMember(drSchema, "kontoKlase", klasa + "%", " LIKE "));

      // For analitic konta only                                                                                                                                            
      drSchema = ZXC.KplanDao.TheSchemaTable.Rows[ZXC.KplanDao.CI.tip];

      filterMembers.Add(new VvSqlFilterMember(drSchema, "tipKonta", "A", " = "));

      LoadGenericVvDataRecordList<Kplan>(conn, listaKonta, filterMembers, "konto");

      return listaKonta;
   }

   #endregion GetKplanList_ForKlasa

   #region IsThisRecordInSomeRelation

   public override bool IsThisRecordInSomeRelation(ZXC.PrivilegedAction action, XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      bool inRelation;
      int? recCount;
      string elKonto = (action == ZXC.PrivilegedAction.DELREC ? ((Kplan)vvDataRecord).Konto : ((Kplan)vvDataRecord).BackupedKonto);

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
      // For wanted konto only                                                                                                                                            
      DataRow drSchema = ZXC.FtransDao.TheSchemaTable.Rows[ZXC.FtransDao.CI.t_konto];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elKonto", elKonto, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inRelation = true;
      else             inRelation = false;

      if(inRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));
         Issue_RecordIsInSomeRelation_Mesage(recordName, elKonto, (int)recCount);
      }

      return inRelation;
   }

   #endregion IsThisRecordInSomeRelation

   #region GetKplan Calculations

   public static decimal GetKplan_SaldoOrDugOrPot_SUM(XSqlConnection conn, ZXC.SaldoOrDugOrPot sdp, string konto, string tt, DateTime dateOd, DateTime dateDo)
   {
      decimal dNum = decimal.Zero; if(konto.IsEmpty()) { ZXC.aim_emsg("Zadajet KONTO!"); return dNum; }

      List<VvSqlFilterMember> filterMembers = Set_GetKplan_FilterMembers(konto, tt, dateOd, dateDo);

      using(XSqlCommand cmd = VvSQL.GetKplan_SUM_Command(conn, sdp, filterMembers))
      {
         object obj = cmd.ExecuteScalar();

         // ovo dole kak radi ne kuzim! Za sve ostalo bi ti javio 'invalid cast' kada bi probao diraktno cast-ati u int ili sl. 
         // inace se mora dNum = decimal.Parse(sender.ToString()); !!!                                                             
         try              { dNum = (decimal)obj; }
         catch(Exception) { dNum = decimal.Zero; }
      }

      return dNum;
   }

   private static List<VvSqlFilterMember> Set_GetKplan_FilterMembers(string konto, string tt, DateTime dateOd, DateTime dateDo)
   {
      DataRow drSchema;
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(4);

      // For wanted konto only                                                                                                                                            
      drSchema = ZXC.FtransDao.TheSchemaTable.Rows[ZXC.FtransDao.CI.t_konto];

      filterMembers.Add(new VvSqlFilterMember(drSchema, "elKonto", konto, " = "));

      if(tt.NotEmpty())
      {
         // For wanted vrsta knjizenja (t_tt) only                                                                                                                                            
         drSchema = ZXC.FtransDao.TheSchemaTable.Rows[ZXC.FtransDao.CI.t_tt];

         filterMembers.Add(new VvSqlFilterMember(drSchema, "elT_tip", tt, " = "));
      }

      if(dateOd != DateTime.MinValue)
      {
         // t_dokDate from dateOd only                                                                                                                                            
         drSchema = ZXC.FtransDao.TheSchemaTable.Rows[ZXC.FtransDao.CI.t_dokDate];

         filterMembers.Add(new VvSqlFilterMember(drSchema, "elDateOD", dateOd, " >= "));
      }

      if(dateDo != DateTime.MaxValue)
      {
         // t_dokDate untill dokDateDo only                                                                                                                                            
         drSchema = ZXC.FtransDao.TheSchemaTable.Rows[ZXC.FtransDao.CI.t_dokDate];

         filterMembers.Add(new VvSqlFilterMember(drSchema, "elDateDO", dateDo, " <= "));
      }

      return filterMembers;
   }

   #endregion GetKplan Calculations

   #region Set_IMPORT_OFFIX_Columns

   //  //____ Specifics 2 start ______________________________________________________

   //fprintf(device, "%s\t", fsklad_rec[0].s_konto    );
   //fprintf(device, "%s\t", fsklad_rec[0].s_tip      );
   //fprintf(device, "%s\t", fsklad_rec[0].s_naziv    );
   //fprintf(device, "%s\t", fsklad_rec[0].s_opis     );
   //fprintf(device, "%s\t", fsklad_rec[0].s_ana_gr   );
	
   //  //____ Specifics 2 end   ______________________________________________________

   public override string Set_IMPORT_OFFIX_Columns()
   {
      return
         "(konto, tip, naziv, opis, anaGr)"          + "\n" +
         "SET "                                      + "\n" +
         "addTS  = CURRENT_TIMESTAMP, modTS = addTS," + "\n" +
         "naziv2 = opis,                            " + "\n" +
         "addUID = '" + ZXC.vvDB_User + "'";
   }

   internal static void ImportFromOffix_Translate437(XSqlConnection conn)
   {
      int debugCount;

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Kplan>(conn, Translate437, null, "", ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out debugCount);
   }

   static bool Translate437(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      Kplan kplan_rec = vvDataRecord as Kplan;

      kplan_rec.Opis  = kplan_rec.Opis .VvTranslate437ToLatin2();
      kplan_rec.Naziv = kplan_rec.Naziv.VvTranslate437ToLatin2();
      
      return kplan_rec.EditedHasChanges();
   }

   #endregion Set_IMPORT_OFFIX_Columns

}

