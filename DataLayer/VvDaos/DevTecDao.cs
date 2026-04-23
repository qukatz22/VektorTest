using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

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
using System.IO;
#endif

public sealed class DevTecDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static DevTecDao instance;

   private DevTecDao(XSqlConnection conn, string dbName) : base(dbName, DevTec2.recordNameArhiva, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static DevTecDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new DevTecDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableDevTec

   public static   uint TableVersionStatic { get { return 3; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition(bool isArhiva)
   {
      return (
         /* 00 */  "recID        int(10)     unsigned NOT NULL auto_increment,\n" +
         /* 01 */  "addTS        timestamp                     NULL DEFAULT NULL,\n" +
         /* 02 */  "modTS        timestamp                     default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,\n" +
         /* 03 */  "addUID       varchar(16)          NOT NULL default 'XY',\n" +
         /* 04 */  "modUID       varchar(16)          NOT NULL default '',\n" +
        CreateTable_LanSrvID_And_LanRecID_Columns +
         /* 05 */  "dokNum       int(10)    unsigned  NOT NULL,\n" +
         /* 06 */  "dokDate      date                 NOT NULL default '0001-01-01',\n" +
         /* 07 */  "tt           char(3)              NOT NULL default '',\n" +
         /* 08 */  "ttNum        int(10)    unsigned  NOT NULL,\n" +
         /* 19 */  "napomena     varchar(1024)        NOT NULL default '',\n" +
         /* 10 */  "flagA        tinyint(1) unsigned  NOT NULL default 0,\n" +

         /* 11 */  "dateCreated  date                 NOT NULL default '0001-01-01',\n" +
         /* 12 */  "extDokNum    int(10)    unsigned  NOT NULL,\n" +


         CreateTable_ArhivaExtensionDefinition(isArhiva) +

                                "PRIMARY KEY            (recID             ),\n" +
          (isArhiva ? "" : "UNIQUE ") + "KEY BY_DOKNUM  (dokNum            ),\n" +
          (isArhiva ? "" : "UNIQUE ") + "KEY BY_DOKDATE (dokDate, dokNum   ),\n" +
          (isArhiva ? "" : "UNIQUE ") + "KEY BY_ttNum   (tt, ttNum)\n" +

          (isArhiva ? ", KEY BYqOrigRecID (origRecID)\n" : "")

         );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = DevTec2.recordNameArhiva;
      else         tableName = DevTec2.recordName;

      switch(catchingVersion)
      {
         case 2: return (isArhiva ? "ADD INDEX BYqOrigRecID (origRecID)\n" : "skip_me");

         case 3: return AlterTable_LanSrvID_And_LanRecID_Columns;
            
         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableDevTec

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      DevTec2 devTec = (DevTec2)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, devTec.RecID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, devTec.AddTS,  TheSchemaTable.Rows[CI.addTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, devTec.ModTS,  TheSchemaTable.Rows[CI.modTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, devTec.AddUID, TheSchemaTable.Rows[CI.addUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, devTec.ModUID, TheSchemaTable.Rows[CI.modUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, devTec.LanSrvID, TheSchemaTable.Rows[CI.lanSrvID]);
         VvSQL.CreateCommandParameter(cmd, preffix, devTec.LanRecID, TheSchemaTable.Rows[CI.lanRecID]);

         /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, devTec.DokNum,        TheSchemaTable.Rows[CI.dokNum]);
         /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, devTec.DokDate,       TheSchemaTable.Rows[CI.dokDate]);
         /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, devTec.TT,            TheSchemaTable.Rows[CI.tt]);
         /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, devTec.TtNum,         TheSchemaTable.Rows[CI.ttNum]);
         /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, devTec.Napomena,      TheSchemaTable.Rows[CI.napomena]);
         /* 10 */ VvSQL.CreateCommandParameter(cmd, preffix, devTec.FlagA,         TheSchemaTable.Rows[CI.flagA]);

         /* 11 */ VvSQL.CreateCommandParameter(cmd, preffix, devTec.DateCreated,   TheSchemaTable.Rows[CI.dateCreated]);
         /* 12 */ VvSQL.CreateCommandParameter(cmd, preffix, devTec.ExtDokNum  ,   TheSchemaTable.Rows[CI.extDokNum]);

         if(isArhiva)
         {
            VvSQL.CreateCommandParameter(cmd, preffix, devTec.OrigRecID, TheSchemaTable.Rows[CI.origRecID]);
            VvSQL.CreateCommandParameter(cmd, preffix, devTec.RecVer,    TheSchemaTable.Rows[CI.recVer]);
            VvSQL.CreateCommandParameter(cmd, preffix, devTec.ArAction,  TheSchemaTable.Rows[CI.arAction]);
            VvSQL.CreateCommandParameter(cmd, preffix, devTec.ArTS,      TheSchemaTable.Rows[CI.arTS]);
            VvSQL.CreateCommandParameter(cmd, preffix, devTec.ArUID,     TheSchemaTable.Rows[CI.arUID]);
         }
      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva)
   {
      DevTecStruct rdrData = new DevTecStruct();

      rdrData._recID  = reader.GetUInt32  (CI.recID);
      rdrData._addTS  = reader.GetDateTime(CI.addTS);
      rdrData._modTS  = reader.GetDateTime(CI.modTS);
      rdrData._addUID = reader.GetString  (CI.addUID);
      rdrData._modUID = reader.GetString  (CI.modUID);
      rdrData._lanSrvID= reader.GetUInt32  (CI.lanSrvID);
      rdrData._lanRecID= reader.GetUInt32  (CI.lanRecID);

      /* 05 */      rdrData._dokNum       = reader.GetUInt32   (CI.dokNum);
      /* 06 */      rdrData._dokDate      = reader.GetDateTime (CI.dokDate);
      /* 07 */      rdrData._tt           = reader.GetString   (CI.tt);
      /* 08 */      rdrData._ttNum        = reader.GetUInt32   (CI.ttNum);
      /* 19 */      rdrData._napomena     = reader.GetString   (CI.napomena);
      /* 10 */      rdrData._flagA        = reader.GetBoolean  (CI.flagA);

      /* 11 */      rdrData._dateCreated  = reader.GetDateTime (CI.dateCreated);
      /* 12 */      rdrData._extDokNum    = reader.GetUInt32   (CI.extDokNum);

      ((DevTec2)vvDataRecord).CurrentData = rdrData;

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

   #region LoadTranses

   public override void LoadTranses(XSqlConnection conn, VvDocumentRecord vvDocumentRecord, bool isArhiva)
   {
      DevTec2 devTec_rec = (DevTec2)vvDocumentRecord;

      if(devTec_rec.Transes == null) devTec_rec.Transes = new List<Htrans2>();
      else                           devTec_rec.Transes.Clear();

      LoadGenericTransesList<Htrans2>(conn, devTec_rec.Transes, devTec_rec.RecID, isArhiva);
   }

   #endregion LoadTranses


   #region DevTecCI struct & InitializeSchemaColumnIndexes()

   public struct DevTecCI
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
      internal int napomena;
      internal int flagA;
      internal int dateCreated;
      internal int extDokNum;

      internal int origRecID;
      internal int recVer;
      internal int arAction;
      internal int arTS;
      internal int arUID;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public DevTecCI CI;

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
      CI.napomena       = GetSchemaColumnIndex("napomena");
      CI.flagA          = GetSchemaColumnIndex("flagA");
      CI.dateCreated    = GetSchemaColumnIndex("dateCreated");
      CI.extDokNum      = GetSchemaColumnIndex("extDokNum");

      CI.origRecID      = GetSchemaColumnIndex("origRecID");
      CI.recVer         = GetSchemaColumnIndex("recVer");
      CI.arAction       = GetSchemaColumnIndex("arAction");
      CI.arTS           = GetSchemaColumnIndex("arTS");
      CI.arUID          = GetSchemaColumnIndex("arUID");
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()



   #region AutoAddDevTec

   private static uint currDokNum_ForAutoAddDevTec;

   private static void AutoAddDevTec(XSqlConnection conn, ref ushort line, DevTec2 devTec_rec, Htrans2 htrans_rec)
   {
      IVvDao devTecDao  = devTec_rec.VvDao;
      IVvDao htransDao  = htrans_rec.VvDao;

      ushort linesPerDocumentLIMIT = ushort.MaxValue;

      if(line < linesPerDocumentLIMIT) line++;
      else                             line=1;

      if(line == 1 || devTec_rec.DokDate != ZXC.DevTecRec.DokDate) // new zaglavlje needed bikoz: (1.st stavka ili novi curr_dokNum) 
      {
         line = 1; 

         //devTec_rec.VirtualTranses = new List<VvTransRecord>(); // ?? !! bez ovoga zajebava. 

         currDokNum_ForAutoAddDevTec = devTecDao.GetNextDokNum(conn, DevTec2.recordName);

         devTec_rec.DokNum = currDokNum_ForAutoAddDevTec;
         devTec_rec.TtNum  = devTecDao.GetNextTtNum(conn, devTec_rec.VirtualTT, null);


      // /* $$$ */ devTecDao.ADDREC(conn, devTec_rec, false, false, false, false);
         /* $$$ */ devTecDao.ADDREC(conn, devTec_rec); // idijote, ostavi ovo da je shouldRefresh TRUE! 

         
         // save new DevTec data for further transes 
         ZXC.DevTecRec = devTec_rec.MakeDeepCopy();

      } // new zaglavlje needed 


      htrans_rec.T_serial   = line;

      htrans_rec.T_parentID = ZXC.DevTecRec.RecID;
      htrans_rec.T_dokNum   = ZXC.DevTecRec.DokNum;
      htrans_rec.T_dokDate  = ZXC.DevTecRec.DokDate;
      htrans_rec.T_TT       = ZXC.DevTecRec.TT;
      htrans_rec.T_ttNum    = ZXC.DevTecRec.TtNum;

      /* $$$    htransDao.ADDREC(conn, htrans_rec);*/
      /* $$$ */
      htransDao.ADDREC(conn, htrans_rec, false, false, false, false);

   }

   public static void AutoSetDevTec(XSqlConnection conn, ref ushort line, DevTec2 devTec_rec, Htrans2 htrans_rec)
   {
      AutoAddDevTec(conn, ref line, devTec_rec, htrans_rec);
   }

   public static void AutoSetDevTec(
      
      XSqlConnection conn, 
      ref ushort     line, 

      DateTime dokDate,
      string   tt,
      string   napomena,
      DateTime dateCreated,
      uint     extDokNum,

      string   t_valName,
      decimal  t_kupovni, 
      decimal  t_srednji,
      decimal  t_prodajni)
   {

      DevTec2  devTec_rec = new DevTec2();
      Htrans2  htrans_rec = new Htrans2();

      devTec_rec.DokDate     = dokDate;
      devTec_rec.TT          = tt;
      devTec_rec.Napomena    = napomena;
      devTec_rec.DateCreated = dateCreated;
      devTec_rec.ExtDokNum   = extDokNum;

      htrans_rec.T_ValName  = t_valName ;
      htrans_rec.T_Kupovni  = t_kupovni ;
      htrans_rec.T_Srednji  = t_srednji ;
      htrans_rec.T_Prodajni = t_prodajni;

      AutoAddDevTec(conn, ref line, devTec_rec, htrans_rec);
   }

   #endregion AutoAddDevTec


   #region Get HNB TecajnaLista From WebSite

   private DateTime htransListLastLoaded = DateTime.MinValue;

 //public decimal GetHnbEurTecaj(DateTime forThisDate)
 //{
 //   return GetTecaj(ZXC.TheMainDbConnection, ZXC.BankaEnum.HNB, ZXC.ValutaNameEnum.EUR, ZXC.TipTecajaEnum.SREDNJI, forThisDate);
 //}
 //
 //public decimal GetHnbEurTecaj(XSqlConnection conn, DateTime forThisDate)
 //{
 //   return GetTecaj(conn, ZXC.BankaEnum.HNB, ZXC.ValutaNameEnum.EUR, ZXC.TipTecajaEnum.SREDNJI, forThisDate);
 //}

   public decimal GetHnbTecaj(ZXC.ValutaNameEnum valuta, DateTime forThisDate)
   {
      if(valuta == /*ZXC.ValutaNameEnum.HRK*/ZXC.EURorHRK_NameEnum || valuta == ZXC.ValutaNameEnum.EMPTY) return 1.00M;

      // 14.05.2025: 
      if(valuta == ZXC.ValutaNameEnum.HRK)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Ne postoji HNB HRK tečaj za traženi datum. Koristim '7.5345'");
         return 1M / ZXC.HRD_tecaj;
      }
      return GetTecaj(ZXC.TheMainDbConnection, ZXC.BankaEnum.HNB, valuta, ZXC.TipTecajaEnum.SREDNJI, forThisDate);
   }

   public decimal GetTecaj(XSqlConnection conn, ZXC.BankaEnum banka, ZXC.ValutaNameEnum valuta, ZXC.TipTecajaEnum tipTecaja, DateTime forThisDate)
   {
      if(valuta == /*ZXC.ValutaNameEnum.HRK*/ZXC.EURorHRK_NameEnum || valuta == ZXC.ValutaNameEnum.EMPTY) return 1.00M;

      // 12.05.2025. vidi opasku u ZXC.cs                                                                          
    //DateTime validHNBdate = GetValidHNBdate(forThisDate);
      DateTime validHNBdate =                (forThisDate.Date);

      //15.05.2023.
      if(forThisDate.Date == ZXC.Date01012023) validHNBdate = ZXC.Date01012023;

      string origDBname = conn.Database;
      Prepare_ZXC_HtransList(conn);
      ZXC.SetMainDbConnDatabaseName(origDBname);

      Htrans2 htrans_rec = 
         // 15.06.2015. 
       //ZXC.HtransList.SingleOrDefault
         ZXC.HtransList.LastOrDefault

         (htr =>  htr.T_TT      == banka .ToString() &&
                  htr.T_ValName == valuta.ToString() &&
                  htr.T_dokDate == validHNBdate        
          );

      if(htrans_rec == null)
      {
         DownloadAndAutoSet_HNB_DevTec(validHNBdate, true);

         if(ZXC.DevTecRec != null)
         {
            //06.02.2023: 
            if(ZXC.DevTecRec.Transes.IsEmpty())
            {
               ZXC.DevTecRec.VvDao.LoadTranses(/*conn*/ZXC.PrjConnection, ZXC.DevTecRec, false);
               ZXC.SetMainDbConnDatabaseName(origDBname);
            }

            htrans_rec = ZXC.DevTecRec.Transes.SingleOrDefault
               (htr => htr.T_TT == banka.ToString() &&
                        htr.T_ValName == valuta.ToString() &&
                        htr.T_dokDate == validHNBdate
                );
         }
         if(htrans_rec == null)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "NE MOGU SAZNATI TECAJ: " + banka.ToString() + " / " + valuta.ToString() + " / " + forThisDate.ToString(ZXC.VvDateDdMmYyyyFormat));
            return 1.00M;
         }
      }

      if(ZXC.IsEURoERA_projectYear)
      {
         switch(tipTecaja)
         {
            // 26.06.2023: 
          //case ZXC.TipTecajaEnum.KUPOVNI : return         1.00M / htrans_rec.T_Kupovni           ; // recipročno u odnosu na stare 'kunske' godine 
          //case ZXC.TipTecajaEnum.SREDNJI : return         1.00M / htrans_rec.T_Srednji           ; // recipročno u odnosu na stare 'kunske' godine 
          //case ZXC.TipTecajaEnum.PRODAJNI: return         1.00M / htrans_rec.T_Prodajni          ; // recipročno u odnosu na stare 'kunske' godine 
            case ZXC.TipTecajaEnum.KUPOVNI : return ZXC.Ron(1.00M / htrans_rec.T_Kupovni , 8); // recipročno u odnosu na stare 'kunske' godine 
            case ZXC.TipTecajaEnum.SREDNJI : return ZXC.Ron(1.00M / htrans_rec.T_Srednji , 8); // recipročno u odnosu na stare 'kunske' godine 
            case ZXC.TipTecajaEnum.PRODAJNI: return ZXC.Ron(1.00M / htrans_rec.T_Prodajni, 8); // recipročno u odnosu na stare 'kunske' godine 

            default: throw new Exception("DevTecDao.GetTecaj(): koji tecaj?");
         }
      }
      else // staro, 'KuneERA_projectYear' 
      { 
         switch(tipTecaja)
         {
            case ZXC.TipTecajaEnum.KUPOVNI : return htrans_rec.T_Kupovni ;
            case ZXC.TipTecajaEnum.SREDNJI : return htrans_rec.T_Srednji ;
            case ZXC.TipTecajaEnum.PRODAJNI: return htrans_rec.T_Prodajni;

            default: throw new Exception("DevTecDao.GetTecaj(): koji tecaj?");
         }
      }

   }

   private void Prepare_ZXC_HtransList(XSqlConnection conn)
   {
      DateTime htransListLastChanged;

      htransListLastChanged = VvDaoBase.GetTableLastChangeTimestamp(DevTec2.recordName);

      // 14.05.2025: 
    //if(htransListLastLoaded < htransListLastChanged)
      if(htransListLastLoaded < htransListLastChanged || htransListLastLoaded == DateTime.MinValue)
      {
         Load_ZXC_HtransList(conn);

         htransListLastLoaded = DateTime.Now;
      }
   }

   private void Load_ZXC_HtransList(XSqlConnection conn)
   {
      if(ZXC.HtransList == null)
      {
         ZXC.HtransList = new List<Htrans2>();
      }
      else
      {
         ZXC.HtransList.Clear();
      }

    //VvDaoBase.LoadGenericVvDataRecordList<Htrans2>(conn, ZXC.HtransList, null, "t_tt, t_valName, t_dokDate"          );
      VvDaoBase.LoadGenericVvDataRecordList<Htrans2>(conn, ZXC.HtransList, null, "t_tt,            t_dokDate, t_serial");
   }

   // 12.05.2025. vidi opasku u ZXC.cs                                                                          
   //private DateTime GetValidHNBdate(DateTime forThisDate)
   //{
   //   try
   //   {
   //      return ZXC.HNB_DevTecDays_Array.Last(devTecDay => devTecDay <= forThisDate);
   //   }
   //   catch(System.InvalidOperationException)
   //   {
   //      return ZXC.HNB_DevTecDays_Array.First(devTecDay => devTecDay >= forThisDate);
   //   }
   //}

   // 12.05.2025. vidi opasku u ZXC.cs                                                                          
   //public static void CheckAndDownloadMissingDevTec(XSqlConnection conn)
   //{
   //   List<DevTec> devTecList = GetExistingDevTecList(conn);
   //
   //   var existingDevTecDates = devTecList.Select(devTec_rec => devTec_rec.DokDate);
   //
   //   bool OK = true;
   //
   //   foreach(DateTime dateOnWhich_HNB_ShouldExists in ZXC.HNB_DevTecDays_Array)
   //   {
   //      if(dateOnWhich_HNB_ShouldExists > DateTime.Today) break;
   //
   //      if(existingDevTecDates.Contains(dateOnWhich_HNB_ShouldExists) == false)
   //      {
   //         OK = DownloadAndAutoSet_HNB_DevTec(dateOnWhich_HNB_ShouldExists, true);
   //      }
   //   }
   //   return /*OK*/;
   //}

   private static List<DevTec2> GetExistingDevTecList(XSqlConnection conn)
   {
      List<DevTec2> devTecList = new List<DevTec2>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);

      filterMembers.Add(new VvSqlFilterMember(ZXC.DevTecSchemaRows[ZXC.DTecCI.tt], "elTT", ZXC.BankaEnum.HNB.ToString(), " = "));

      VvDaoBase.LoadGenericVvDataRecordList<DevTec2>(conn, devTecList, filterMembers, "DokDate");

      return devTecList;
   }

   public static bool DownloadAndAutoSet_HNB_DevTec(DateTime dokDate, bool reportError)
   {
      bool OK;
      
      OK = DownloadHNBtecaj(dokDate, reportError);

      if(!OK) return false;

      OK = AutoAddDevTec_FromHNB_File(dokDate);

      return OK;
   }

   public static bool AutoAddDevTec_FromHNB_File(DateTime dokDate)
   {
      string localDirectory = GetLocalDirectoryForDownload();
      string localFileName  = GetLocalHNBfileName(dokDate);
      string localFilePath  = localDirectory + @"\" + localFileName;

      if(File.Exists(localFilePath) == false)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "NE POSTOJI datoteka: " + localFilePath);

         return false;
      }

      HNB_DevTecImportFile hnbDevTec = new HNB_DevTecImportFile(localFilePath);

      // 02.03.2016: 
      if(hnbDevTec.Transes == null || hnbDevTec.NumOfHnbFileLines == 0)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Datoteka: " + localFilePath + " je prazna!");

         return false;
      }

      ushort line = 0;

      foreach(HtransStruct htrans_rec in hnbDevTec.Transes)
      {
       //AutoSetDevTec(ZXC.PrjConnection, ref line,
         AutoSetDevTec(ZXC.TheSecondDbConn_OtherDB(ZXC.VvDB_prjktDB_Name), ref line,
            // 12.05.2025. vidi opasku u ZXC.cs                                                                          
          //hnbDevTec.HeadRecord._dokDate,
                                  dokDate,
            "HNB",
            hnbDevTec.HeadRecord._napomena,
            hnbDevTec.HeadRecord._dateCreated,
            hnbDevTec.HeadRecord._extDokNum,
            htrans_rec._t_valName,
            htrans_rec._t_kupovni,
            htrans_rec._t_srednji,  
            htrans_rec._t_prodajni);
      }

      // 09.02.2023: 
      ZXC.DevTecRec.VvDao.LoadTranses(/*conn*//*ZXC.PrjConnection*/ZXC.TheSecondDbConn_OtherDB(ZXC.VvDB_prjktDB_Name), ZXC.DevTecRec, false);

      return true;
   }

   public static bool DownloadHNBtecaj(DateTime dokDate, bool reportError)
   {
      Uri    theUri;
      string localDirectory  = GetLocalDirectoryForDownload();
      string localFileName   = GetLocalHNBfileName(dokDate);
      // 18.01.2023: 
    //string remoteDirectory = @"http://www.hnb.hr/tecajn"     ;
      string remoteDirectory = @"https://www.hnb.hr/tecajn-eur";
      string remoteFileName  = GetRemoteHNBfileName(dokDate)   ;
      string localFilePath;
      string remoteFilePath;

      localFilePath  = localDirectory  + @"\" + localFileName;
      remoteFilePath = remoteDirectory + @"/" + remoteFileName;

      theUri = new Uri(remoteFilePath);

      System.Net.WebClient wc = new System.Net.WebClient();

      try
      {
         wc.DownloadFile(theUri, localFilePath);
      }
      catch(System.Net.WebException webException)
      {
         if(reportError)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "DownloadHNBtecaj WebException. " + webException.Message);
         }
         return false;
      }
      catch(Exception ex)
      {
         if(reportError)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "DownloadHNBtecaj Exception. " + ex.Message);
         }
         return false;
      }

      return(File.Exists(localFilePath));
   }

   private static string GetLocalHNBfileName(DateTime dokDate)
   {
      return ("HNB_" + dokDate.ToString(ZXC.VvDateDdMmYyyyFormat) + ".txt");
   }

   private static string GetRemoteHNBfileName(DateTime dokDate)
   {
      return ("f" + dokDate.ToString(ZXC.VvDateDdMmYyFormat) + ".dat");
   }

   private static string GetLocalDirectoryForDownload()
   {
      string theDirectory = VvForm.GetMyDocumentsLocation(ZXC.vv_PRODUCT_Name, false) + @"\" + DevTec2.recordName;

      if(!System.IO.Directory.Exists(theDirectory))
      {
         VvForm.CreateDirectoryInMyDocuments(theDirectory);
      }

      return theDirectory;
   }

   #endregion Get HNB TecajnaLista From WebSite

}

public sealed class HNB_DevTecImportFile
{
   #region Fields Arrays

   VvImpExp.ImpExpField[] LeadFields = new VvImpExp.ImpExpField[]
   {
      new VvImpExp.ImpExpField("HD_BrDok"   ,  1,   0,   3),
      new VvImpExp.ImpExpField("HD_Created" ,  2,   0,   8), 
      new VvImpExp.ImpExpField("HD_DateDok" ,  3,   0,   8),
      new VvImpExp.ImpExpField("HD_TrnCount",  4,   0,   2),
   };

   VvImpExp.ImpExpField[] TransFields = new VvImpExp.ImpExpField[]
   {
      new VvImpExp.ImpExpField("TR_SifVal", 01,   0,   3), 
      new VvImpExp.ImpExpField("TR_OznVal", 02,   0,   3), 
      // u 2023: 
    //new VvImpExp.ImpExpField("TR_BrJed" , 03,   0,   3), 
      new VvImpExp.ImpExpField("TR_KupTec", 04,   0,  15), 
      new VvImpExp.ImpExpField("TR_SreTec", 05,   0,  15), 
      new VvImpExp.ImpExpField("TR_ProTec", 06,   0,  15), 
   };

   #endregion Fields Arrays

   #region Propertiz

   public Dictionary<string, VvImpExp.ImpExpField> LeadDict   { get; set; }
   public Dictionary<string, VvImpExp.ImpExpField> TransDict  { get; set; }

   // 2023: 
 //public const int lineFixedLength = 54; 
   public const int lineFixedLength = 51;

   string[] txtLines;

   public int? NumOfHnbFileLines { get; private set; }

   public int NumOfTransLinesSoFar { get; private set; }

   private decimal MoneySUM { get; set; }

   private bool leadOK = true, transOK = true, linesOK = true, fileOK = true;

   public  bool BadData
   {
      get
      {
         return (!fileOK || !leadOK || !transOK || !linesOK);
      }
   }

   private DevTecStruct headRecord;
   public  DevTecStruct HeadRecord
   {
      get { return headRecord; }
   }

   private HtransStruct[] transes;
   public  HtransStruct[] Transes
   {
      get { return transes; }
   }

   public string FullPathFileName { get; set; }
   public string FileName         { get; set; }
   public string DirectoryName    { get; set; }

   #endregion Propertiz

   #region Constructors

   public HNB_DevTecImportFile(string _fullPathFileName)
   {
      DirectoryInfo dInfo;

      NumOfHnbFileLines = LoadHnbFromFile(_fullPathFileName);

      // 02.03.2016: 
    //if(NumOfHnbFileLines == null)                           { fileOK = false; return; }
      if(NumOfHnbFileLines == null || NumOfHnbFileLines == 0) { fileOK = false; return; }

      FullPathFileName = _fullPathFileName;
      dInfo            = new DirectoryInfo(FullPathFileName);
      FileName         = dInfo.Name;
      DirectoryName    = FullPathFileName.Substring(0, FullPathFileName.Length - (FileName.Length+1));

      this.transes = new HtransStruct[(int)NumOfHnbFileLines - 1]; 

      NumOfTransLinesSoFar = 0;

      int[] LeadPartLengths   = LeadFields  .Select(el => el.FldLength).ToArray();
      int[] TransPartLengths  = TransFields .Select(el => el.FldLength).ToArray();

      bool thisIsFirtsLine = true;

      foreach(string line in txtLines)
      {
         if(BadData) break;

         if(thisIsFirtsLine == true)
         {
            thisIsFirtsLine = false;

            leadOK = Take_LEAD_Data(VvImpExp.SeparateStringsFromImportLine(line, LeadPartLengths));
         }
         else
         {
            transOK  = Take_TRANS_Data (VvImpExp.SeparateStringsFromImportLine(line, TransPartLengths), NumOfTransLinesSoFar++);
         }
      }

      if(NumOfHnbFileLines < 2 || NumOfHnbFileLines - NumOfTransLinesSoFar != 1) linesOK = false;
      else                                                                       linesOK = true;
   }

   #endregion Constructors

   #region Methodz

   #region IMPORT

   private int? LoadHnbFromFile(string fName)
   {
      txtLines = VvImpExp.GetFileLinesAsStringArray(fName);

      if(txtLines == null) return null;
      else                 return txtLines.Length;
   }

   private char GetLineID(string line)
   {
      return ZXC.GetStringsLastChar(line);
   }

   private bool Take_LEAD_Data(string[] inFileData)
   {
      bool OK = true;
      int colNum = 1;

      if(inFileData == null) return false;

      headRecord._napomena = "Datoteka: " + FullPathFileName; 

      foreach(string field in inFileData)
      {
         switch(colNum)
         {
            case 1: headRecord._extDokNum   = ZXC.ValOrZero_UInt(field);                                 break;
            case 2: headRecord._dateCreated = ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(field); break;
            case 3: headRecord._dokDate     = ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(field); break;
         }

         colNum++;
      }

      return OK;
   }

   private bool Take_TRANS_Data(string[] inFileData, int serial)
   {
      bool OK = true;
      int colNum = 1;

      if(inFileData == null) return false;

      //transes[serial].t_br     = headRecord.iz_br;
      transes[serial]._t_serial = (ushort)serial;

      foreach(string field in inFileData)
      {
         switch(colNum)
         {
            case 2: transes[serial]._t_valName = field; break;

            case 3: transes[serial]._t_kupovni  = ValOrZero_Decimal_HNB(field); break;
            case 4: transes[serial]._t_srednji  = ValOrZero_Decimal_HNB(field); break;
            case 5: transes[serial]._t_prodajni = ValOrZero_Decimal_HNB(field); break;

         } // switch(colNum) 

         colNum++;
      }

      return OK;
   }

   public static decimal ValOrZero_Decimal_HNB(string textToParse)
   {
      decimal num = 0;

      if(textToParse != null && textToParse.Length > 0)
      {
         System.Globalization.CultureInfo current_ci = new System.Globalization.CultureInfo("hr-HR", true);

         current_ci.NumberFormat.NumberDecimalDigits = 6;

         IFormatProvider provider = System.Globalization.CultureInfo.CurrentCulture;
         
         Decimal.TryParse(textToParse, System.Globalization.NumberStyles.Number, current_ci, out num);
      }

      return num;
   }


   #endregion IMPORT

   #endregion Methodz

}

