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
using XSqlException  = MySql.Data.MySqlClient.MySqlException;
using System.Collections.Generic;
#endif

public sealed class OsredDao : VvDaoBase, IVvDao
{
   
   #region Singleton Constructor & instancer

   private static OsredDao instance;

   private OsredDao(XSqlConnection conn, string dbName) : base(dbName, Osred.recordNameArhiva, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static OsredDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if (instance == null)
      {
         instance = new OsredDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableOsred

   public static   uint TableVersionStatic { get { return 4; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition(bool isArhiva)
   {
      return 
      (
        "recID int(10) unsigned NOT NULL auto_increment,\n" +
        "addTS timestamp                 default '0000-00-00 00:00:00',\n" +
        "modTS timestamp                 default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,\n" +
        "addUID varchar(16)     NOT NULL default 'XY',\n" +
        "modUID varchar(16)     NOT NULL default '',\n" +
                CreateTable_LanSrvID_And_LanRecID_Columns +

        "naziv       varchar(64)      NOT NULL default '',\n" +
        "osredCD     varchar(24)      NOT NULL default '',\n" +
        "konto       varchar(16)      NOT NULL default '',\n" +
        "konto_iv    varchar(16)      NOT NULL default '',\n" +
        "grupa       varchar(8)       NOT NULL default '',\n" +
        "strana_k    varchar(8)       NOT NULL default '',\n" +
        "ser_br      varchar(64)      NOT NULL default '',\n" +
        "mtros_cd    int(6)  unsigned NOT NULL default '0',\n" +
        "mtros_tk    varchar(6)       NOT NULL default '',\n" +
        "kupdob_cd   int(6)  unsigned NOT NULL default '0',\n" +
        "kupdob_tk   varchar(6)       NOT NULL default '',\n" +
        "dokum_cd    varchar(64)      NOT NULL default '',\n" +
        "invest      varchar(16)      NOT NULL default '',\n" +
        "invbr_od    varchar(64)      NOT NULL default '',\n" +
        "invbr_do    varchar(16)      NOT NULL default '',\n" +
        "koef_am     char(1)          NOT NULL default '',\n" +
        "amort_st    DECIMAL(6,2)     NOT NULL DEFAULT 0.00,\n" +
        "vijek       DECIMAL(6,2)     NOT NULL DEFAULT 0.00,\n" +
        "isRashod    tinyint(1)   unsigned NOT NULL default 0,\n" +

         CreateTable_ArhivaExtensionDefinition(isArhiva) +

                              "PRIMARY KEY  (recID),\n" +
        (isArhiva ? "" : "UNIQUE ") + "KEY BY_NAZIV (naziv, recID)\n," +
        (isArhiva ? "" : "UNIQUE ") + "KEY BY_CODE  (osredCD)\n" +

          (isArhiva ? ", KEY BYqOrigRecID (origRecID)\n" : "")

      );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Osred.recordNameArhiva;
      else         tableName = Osred.recordName;

      switch(catchingVersion)
      {
         case 2: return (isArhiva ? "ADD INDEX BYqOrigRecID (origRecID)\n" : "skip_me");

         case 3: return AlterTable_LanSrvID_And_LanRecID_Columns;

         case 4: return ("MODIFY COLUMN dokum_cd    varchar(64)         NOT NULL default '', " +
                         "MODIFY COLUMN invbr_od    varchar(64)         NOT NULL default ''  ");

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableOsred

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      Osred osred = (Osred)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.ID_Only ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         //VvSQL.CreateCommandParameter(cmd, where_or_and, "prjktKupdobCD", osred.RecID, XSqlDbType.Int32, 10);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.RecID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         //DataRow theRow = TheSchemaTable.Rows.Find("modUID");

         VvSQL.CreateCommandParameter(cmd, preffix, osred.AddTS,      TheSchemaTable.Rows[CI.addTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.ModTS,      TheSchemaTable.Rows[CI.modTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.AddUID,     TheSchemaTable.Rows[CI.addUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.ModUID,     TheSchemaTable.Rows[CI.modUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.LanSrvID,   TheSchemaTable.Rows[CI.lanSrvID]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.LanRecID,   TheSchemaTable.Rows[CI.lanRecID]);
   
         VvSQL.CreateCommandParameter(cmd, preffix, osred.Naziv,      TheSchemaTable.Rows[CI.naziv]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.OsredCD,    TheSchemaTable.Rows[CI.osredCD]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.Konto,      TheSchemaTable.Rows[CI.konto]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.KontoIv,    TheSchemaTable.Rows[CI.konto_iv]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.Grupa,      TheSchemaTable.Rows[CI.grupa]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.StranaK,    TheSchemaTable.Rows[CI.strana_k]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.SerBr,      TheSchemaTable.Rows[CI.ser_br]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.MtrosCd,    TheSchemaTable.Rows[CI.mtros_cd]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.MtrosTk,    TheSchemaTable.Rows[CI.mtros_tk]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.KupdobCd,   TheSchemaTable.Rows[CI.kupdob_cd]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.KupdobTk,   TheSchemaTable.Rows[CI.kupdob_tk]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.DokumCd,    TheSchemaTable.Rows[CI.dokum_cd]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.Invest,     TheSchemaTable.Rows[CI.invest]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.InvbrOd,    TheSchemaTable.Rows[CI.invbr_od]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.InvbrDo,    TheSchemaTable.Rows[CI.invbr_do]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.KoefAm,     TheSchemaTable.Rows[CI.koef_am]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.AmortSt,    TheSchemaTable.Rows[CI.amort_st]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.Vijek,      TheSchemaTable.Rows[CI.vijek]);
         VvSQL.CreateCommandParameter(cmd, preffix, osred.IsRashod,   TheSchemaTable.Rows[CI.isRashod]);
      

         if(isArhiva)
         {
            VvSQL.CreateCommandParameter(cmd, preffix, osred.OrigRecID, TheSchemaTable.Rows[CI.origRecID]);
            VvSQL.CreateCommandParameter(cmd, preffix, osred.RecVer,    TheSchemaTable.Rows[CI.recVer]);
            VvSQL.CreateCommandParameter(cmd, preffix, osred.ArAction,  TheSchemaTable.Rows[CI.arAction]);
            VvSQL.CreateCommandParameter(cmd, preffix, osred.ArTS,    TheSchemaTable.Rows[CI.arTS]);
            VvSQL.CreateCommandParameter(cmd, preffix, osred.ArUID,     TheSchemaTable.Rows[CI.arUID]);
         }

      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva)
   {
      OsredStruct rdrData = new OsredStruct();

      rdrData._recID       = reader.GetUInt32  (CI.recID    );
      rdrData._addTS       = reader.GetDateTime(CI.addTS    );
      rdrData._modTS       = reader.GetDateTime(CI.modTS    );
      rdrData._addUID      = reader.GetString  (CI.addUID   );
      rdrData._modUID      = reader.GetString  (CI.modUID   );
      rdrData._lanSrvID    = reader.GetUInt32  (CI.lanSrvID );
      rdrData._lanRecID    = reader.GetUInt32  (CI.lanRecID );

      rdrData._naziv       = reader.GetString  (CI.naziv    );
      rdrData._osredCD     = reader.GetString  (CI.osredCD  );
      rdrData._konto       = reader.GetString  (CI.konto    );
      rdrData._konto_iv    = reader.GetString  (CI.konto_iv );
      rdrData._grupa       = reader.GetString  (CI.grupa    );
      rdrData._strana_k    = reader.GetString  (CI.strana_k );
      rdrData._ser_br      = reader.GetString  (CI.ser_br   );
      rdrData._mtros_cd    = reader.GetUInt32  (CI.mtros_cd );
      rdrData._mtros_tk    = reader.GetString  (CI.mtros_tk );
      rdrData._kupdob_cd   = reader.GetUInt32  (CI.kupdob_cd);
      rdrData._kupdob_tk   = reader.GetString  (CI.kupdob_tk);
      rdrData._dokum_cd    = reader.GetString  (CI.dokum_cd );
      rdrData._invest      = reader.GetString  (CI.invest   );
      rdrData._invbr_od    = reader.GetString  (CI.invbr_od );
      rdrData._invbr_do    = reader.GetString  (CI.invbr_do );
      rdrData._koef_am     = reader.GetString  (CI.koef_am  );
      rdrData._amort_st    = reader.GetDecimal (CI.amort_st );
      rdrData._vijek       = reader.GetDecimal (CI.vijek    );
      rdrData._isRashod    = reader.GetBoolean (CI.isRashod );

      ((Osred)vvDataRecord).CurrentData = rdrData;

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

   private static OsredStatus FillOsredStatusFromDataReader(XSqlDataReader reader, int ciOffset, bool needsDates)
   {
      OsredStatus osredStatus = new OsredStatus();

      try
      {
         osredStatus.OsredCD      = reader.GetString ( 0 + ciOffset);
         osredStatus.IsRashodovan = reader.GetBoolean( 1 + ciOffset);
         osredStatus.InvSt        = reader.GetDecimal( 2 + ciOffset);
         osredStatus.KolSt        = reader.GetDecimal( 3 + ciOffset);
         osredStatus.UkNabDug     = reader.GetDecimal( 4 + ciOffset);
         osredStatus.UkNabPot     = reader.GetDecimal( 5 + ciOffset);
         osredStatus.UkRasDug     = reader.GetDecimal( 6 + ciOffset);
         osredStatus.UkRasPot     = reader.GetDecimal( 7 + ciOffset);
         osredStatus.OldAmDug     = reader.GetDecimal( 8 + ciOffset);
         osredStatus.OldAmPot     = reader.GetDecimal( 9 + ciOffset);
         osredStatus.NewAmDug     = reader.GetDecimal(10 + ciOffset);
         osredStatus.NewAmPot     = reader.GetDecimal(11 + ciOffset);

         if(needsDates)
         {
            osredStatus.DateNabava   = reader.GetDateTime(12 + ciOffset); 
            osredStatus.DateRashod   = reader.GetDateTime(13 + ciOffset); 

            /* '2999-12-31' vraca querry ako nema 'NB' */ if(osredStatus.DateNabava.Year > ZXC.projectYearFirstDay.Year) osredStatus.DateNabava = DateTime.MinValue;
            /* '2999-12-31' vraca querry ako nema 'RS' */ if(osredStatus.DateRashod.Year > ZXC.projectYearFirstDay.Year) osredStatus.DateRashod = DateTime.MinValue;
         }
      }
      catch(System.Data.SqlTypes.SqlNullValueException) // kada je null from reader 
      {
      }

      return osredStatus;
   }

   #endregion FillFromDataReader

   #region FtrCI struct & InitializeSchemaColumnIndexes()

   public struct OsredCI
   {
      internal int recID;
      internal int addTS;
      internal int modTS;
      internal int addUID;
      internal int modUID;
      internal int lanSrvID;
      internal int lanRecID;

      internal int naziv;
      internal int osredCD;
      internal int konto;
      internal int konto_iv;
      internal int grupa;
      internal int strana_k;
      internal int ser_br;
      internal int mtros_cd;
      internal int mtros_tk;
      internal int kupdob_cd;
      internal int kupdob_tk;
      internal int dokum_cd;
      internal int invest;
      internal int invbr_od;
      internal int invbr_do;
      internal int koef_am;
      internal int amort_st;
      internal int vijek;
      internal int isRashod;

      internal int origRecID;
      internal int recVer;
      internal int arAction;
      internal int arTS;
      internal int arUID;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public OsredCI CI;

   public static int lastOsredCI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID       = GetSchemaColumnIndex("recID");
      CI.addTS       = GetSchemaColumnIndex("addTS");
      CI.modTS       = GetSchemaColumnIndex("modTS");
      CI.addUID      = GetSchemaColumnIndex("addUID");
      CI.modUID      = GetSchemaColumnIndex("modUID");
      CI.lanSrvID    = GetSchemaColumnIndex("lanSrvID");
      CI.lanRecID    = GetSchemaColumnIndex("lanRecID");
      CI.naziv       = GetSchemaColumnIndex("naziv");
      CI.osredCD     = GetSchemaColumnIndex("osredCD");
      CI.konto       = GetSchemaColumnIndex("konto");
      CI.konto_iv    = GetSchemaColumnIndex("konto_iv");
      CI.grupa       = GetSchemaColumnIndex("grupa");
      CI.strana_k    = GetSchemaColumnIndex("strana_k");
      CI.ser_br      = GetSchemaColumnIndex("ser_br");
      CI.mtros_cd    = GetSchemaColumnIndex("mtros_cd");
      CI.mtros_tk    = GetSchemaColumnIndex("mtros_tk");
      CI.kupdob_cd   = GetSchemaColumnIndex("kupdob_cd");
      CI.kupdob_tk   = GetSchemaColumnIndex("kupdob_tk");
      CI.dokum_cd    = GetSchemaColumnIndex("dokum_cd");
      CI.invest      = GetSchemaColumnIndex("invest");
      CI.invbr_od    = GetSchemaColumnIndex("invbr_od");
      CI.invbr_do    = GetSchemaColumnIndex("invbr_do");
      CI.koef_am     = GetSchemaColumnIndex("koef_am");
      CI.amort_st    = GetSchemaColumnIndex("amort_st");
      CI.vijek       = GetSchemaColumnIndex("vijek");
      CI.isRashod    = GetSchemaColumnIndex("isRashod");

      lastOsredCI    = CI.isRashod; // !!!!!! 

      CI.origRecID      = GetSchemaColumnIndex("origRecID");
      CI.recVer         = GetSchemaColumnIndex("recVer");
      CI.arAction       = GetSchemaColumnIndex("arAction");
      CI.arTS           = GetSchemaColumnIndex("arTS");
      CI.arUID          = GetSchemaColumnIndex("arUID");
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()



   #region IsThisRecordInSomeRelation

   public override bool IsThisRecordInSomeRelation(ZXC.PrivilegedAction action, XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      bool inRelation;
      int? recCount;
      string osredCD = (action == ZXC.PrivilegedAction.DELREC ? ((Osred)vvDataRecord).OsredCD : ((Osred)vvDataRecord).BackupedOsredCD);

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
      // For wanted osredCD only                                                                                                                                            
      DataRow drSchema = ZXC.AtransDao.TheSchemaTable.Rows[ZXC.AtransDao.CI.t_osredCD];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elOsredeCD", osredCD, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inRelation = true;
      else             inRelation = false;

      if(inRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));
         Issue_RecordIsInSomeRelation_Mesage(recordName, osredCD, (int)recCount);
      }

      return inRelation;
   }

   #endregion IsThisRecordInSomeRelation
   
   #region CalcOsredStatus

   public static OsredStatus GetOsredStatus(XSqlConnection conn, string wantedOsredCD, ZXC.AmortRazdoblje amRazdoblje, DateTime dateOfStatus)
   {
      return GetOsredStatusList(conn, wantedOsredCD, amRazdoblje, dateOfStatus)[0];
   }

   public static List<OsredStatus> GetOsredStatusList(XSqlConnection conn, string wantedOsredCD, ZXC.AmortRazdoblje amRazdoblje, DateTime dateOfStatus)
   {
      DateTime          dateAmRazdobljeStart;
      OsredStatus       osredSTAT_rec;
      List<OsredStatus> list = new List<OsredStatus>();

      List<VvSqlFilterMember> filterMembers = Set_CalcOsredStatus_FilterMembers(wantedOsredCD, amRazdoblje, dateOfStatus, out dateAmRazdobljeStart);

      using(XSqlCommand cmd = VvSQL.GetOsredStatusList_SUM_Command(conn, filterMembers, dateAmRazdobljeStart))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               while(reader.HasRows && reader.Read())
               {
                  osredSTAT_rec = OsredDao.FillOsredStatusFromDataReader(reader, 0, false);

                  osredSTAT_rec.AmRazdoblje  = amRazdoblje;

                  osredSTAT_rec.DateOfStatus = dateOfStatus;
                  
                  list.Add(osredSTAT_rec);
               }
               reader.Close();
            }
         }
         catch(XSqlException ex) { VvSQL.ReportSqlError("GetOsredStatusList", ex, System.Windows.Forms.MessageBoxButtons.OK); return null; }

      } // using(XSqlCommand cmd = VvSQL.GetOsredStatusList_SUM_Command(conn, filterMembers_1, dateAmRazdobljeStart))

      return list;
   }

   private static List<VvSqlFilterMember> Set_CalcOsredStatus_FilterMembers(string wantedOsredCD, ZXC.AmortRazdoblje amRazdoblje, DateTime dateDo, out DateTime dateAmRazdobljeStart)
   {
      DataRow drSchema;
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(wantedOsredCD.IsEmpty() ? 1 : 2);

      // one t_personCD only                                                                                                                                            
      if(wantedOsredCD.NotEmpty())
      {
         drSchema = ZXC.AtransDao.TheSchemaTable.Rows[ZXC.AtransDao.CI.t_osredCD];

         filterMembers.Add(new VvSqlFilterMember(drSchema, "elOsredCD", wantedOsredCD, " = "));
      }

      // t_dokDate untill dokDateDo only                                                                                                                                            
      drSchema = ZXC.AtransDao.TheSchemaTable.Rows[ZXC.AtransDao.CI.t_dokDate];

      filterMembers.Add(new VvSqlFilterMember(drSchema, "dateDO", dateDo, " <= "));

      dateAmRazdobljeStart = OsredStatus.GetDateAmRazdobljeStart(amRazdoblje, dateDo);

      return filterMembers;
   }
   
   /// <summary>
   /// NEW in 2016 for ObrazacDI
   /// </summary>
   /// <param name="dbConnection"></param>
   /// <param name="wantedOsredCD"></param>
   /// <param name="dateDo"></param>
   /// <returns></returns>
   internal static void GetOsredWithOsrstatList(XSqlConnection conn, List<Osred> osredList, DateTime _dateDo, VvRpt_Osred_Filter RptFilter, DateTime _dateAmRazdobljeStart, string orderBy)
   {
      bool success = true;
      Osred osred_rec;

      ZXC.sqlErrNo = 0;

      using(XSqlCommand cmd = (VvSQL.GetOsredWithOsrstatList_Command(conn, _dateDo, RptFilter, _dateAmRazdobljeStart, orderBy)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;

               while(success && reader.Read())
               {
                  osred_rec = new Osred();

                  ZXC.OsredDao.FillFromDataReader(osred_rec, reader, false);

                  osred_rec.TheOsEx = OsredDao.FillOsredStatusFromDataReader(reader, OsredDao.lastOsredCI + 1, true);

                  osredList.Add(osred_rec);
               }
               reader.Close();
            }
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("GetOsredWithArtstatList", ex, System.Windows.Forms.MessageBoxButtons.OK);

            ZXC.sqlErrNo = ex.Number;
         }
      } // using 
   }

   
   #endregion CalcOsredStatus

   public static List<Atrans> GetAtransListForOsred(XSqlConnection dbConnection, string wantedOsredCD, DateTime dateDo)
   {
      #region SetFilterMemberz

      DataRow drSchema;
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      //                                                                                                                               
      drSchema = ZXC.AtransDao.TheSchemaTable.Rows[ZXC.AtransDao.CI.t_osredCD];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elOsredCD", wantedOsredCD, " = "));

      //                                                                                                                               
      drSchema = ZXC.AtransDao.TheSchemaTable.Rows[ZXC.AtransDao.CI.t_dokDate];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elDateDO", dateDo, " <= "));

      #endregion SetFilterMemberz

      List<Atrans> atransList = new List<Atrans>();

      VvDaoBase.LoadGenericVvDataRecordList<Atrans>(dbConnection, atransList, filterMembers, "t_dokDate, t_tt, t_dokNum, t_serial");

      return atransList;
   }

   #region Set_IMPORT_OFFIX_Columns

   //  //____ Specifics 2 start ______________________________________________________

   //fprintf(device, "%s\t", osred_rec[0].os_osred_cd);
   //fprintf(device, "%s\t", osred_rec[0].os_osred);
   //fprintf(device, "%s\t", osred_rec[0].os_konto);
   //fprintf(device, "%s\t", osred_rec[0].os_ivt_cd);  // invbr_od
   //fprintf(device, "%s\t", osred_rec[0].os_ivt_span); // invbr_od
   //fprintf(device, "%s\t", osred_rec[0].os_part_no); // serBr 
   //fprintf(device, "%s\t", osred_rec[0].os_invest); // invest
   //fprintf(device, "%c\t", osred_rec[0].os_some_flag[0]); // koef_am
   //fprintf(device, "%c\t", osred_rec[0].os_deadFlag[0]); // isRashod 
   //fprintf(device, "%s\t", osred_rec[0].os_grupa);
   //fprintf(device, "%s\t", osred_rec[0].os_strana_k);
   //fprintf(device, "%s\t", osred_rec[0].os_konto_iv);
   //fprintf(device, "%s\t", osred_rec[0].os_mtros_cd);
   //fprintf(device, "%s\t", osred_rec[0].os_mtros);
   //fprintf(device, "%s\t", osred_rec[0].os_kupdob_cd);
   //fprintf(device, "%s\t", osred_rec[0].os_kupdob);
   //fprintf(device, "%s\t", osred_rec[0].os_dokum_cd);
   //fprintf(device, "%.2lf\t", osred_rec[0].os_vijek);
   //fprintf(device, "%.2lf\t", osred_rec[0].os_amort_p);

   //fprintf(device, "\n");
	
   //  //____ Specifics 2 end   ______________________________________________________

   public override string Set_IMPORT_OFFIX_Columns()
   {
      return

         "(" +

         "osredCD  , " + //fprintf(device, "%s\t", osred_rec[0].os_osred_cd);
         "naziv    , " + //fprintf(device, "%s\t", osred_rec[0].os_osred);
         "konto    , " + //fprintf(device, "%s\t", osred_rec[0].os_konto);
         "invbr_od , " + //fprintf(device, "%s\t", osred_rec[0].os_ivt_cd);  // invbr_od
         "invbr_do , " + //fprintf(device, "%s\t", osred_rec[0].os_ivt_span); // invbr_od
         "ser_br   , " + //fprintf(device, "%s\t", osred_rec[0].os_part_no); // serBr 
         "invest   , " + //fprintf(device, "%s\t", osred_rec[0].os_invest); // invest
         "koef_am  , " + //fprintf(device, "%c\t", osred_rec[0].os_some_flag[0]); // koef_am
         "isRashod , " + //fprintf(device, "%c\t", osred_rec[0].os_deadFlag[0]); // isRashod 
         "grupa    , " + //fprintf(device, "%s\t", osred_rec[0].os_grupa);
         "strana_k , " + //fprintf(device, "%s\t", osred_rec[0].os_strana_k);
         "konto_iv , " + //fprintf(device, "%s\t", osred_rec[0].os_konto_iv);
         "mtros_cd , " + //fprintf(device, "%s\t", osred_rec[0].os_mtros_cd);
         "@puse1   , " + //fprintf(device, "%s\t", osred_rec[0].os_mtros);
         "kupdob_cd, " + //fprintf(device, "%s\t", osred_rec[0].os_kupdob_cd);
         "@puse2   , " + //fprintf(device, "%s\t", osred_rec[0].os_kupdob);
         "dokum_cd , " + //fprintf(device, "%s\t", osred_rec[0].os_dokum_cd);
         "vijek    , " + //fprintf(device, "%.2lf\t", osred_rec[0].os_vijek);
         "amort_st   " + //fprintf(device, "%.2lf\t", osred_rec[0].os_amort_p);

         ")"    + "\n" +

         "SET " + "\n" +

         "addTS = CURRENT_TIMESTAMP, modTS = addTS," + "\n" +
         "addUID = '" + ZXC.vvDB_User + "'";
   }

   internal static void ImportFromOffix_Translate437_SetTickers(XSqlConnection conn)
   {
      int debugCount;

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Osred>(conn, Translate437, null, "", ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out debugCount);
   }

   static bool Translate437(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      Osred osred_rec = vvDataRecord as Osred;

      osred_rec.Naziv = osred_rec.Naziv.VvTranslate437ToLatin2();

      if(osred_rec.KupdobCd.NotZero())
      {
         Kupdob kupdob_rec = new Kupdob(osred_rec.KupdobCd);

         bool OK = kupdob_rec.VvDao.SetMe_Record_bySomeUniqueColumn(conn, kupdob_rec, osred_rec.KupdobCd, ZXC.KupdobSchemaRows[ZXC.KpdbCI.kupdobCD], false, false);

         if(OK)
         {
            osred_rec.KupdobTk = kupdob_rec.Ticker;
         }
      }

      if(osred_rec.MtrosCd.NotZero())
      {
         Kupdob kupdob_rec = new Kupdob(osred_rec.MtrosCd);

         bool OK = kupdob_rec.VvDao.SetMe_Record_bySomeUniqueColumn(conn, kupdob_rec, osred_rec.MtrosCd, ZXC.KupdobSchemaRows[ZXC.KpdbCI.kupdobCD], false, false);

         if(OK)
         {
            osred_rec.MtrosTk = kupdob_rec.Ticker;
         }
      }
      
      return osred_rec.EditedHasChanges();
   }

   #endregion Set_IMPORT_OFFIX_Columns

}

