using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Win32;
using static ArtiklDao;
using System.Xml.Linq;


#if MICROSOFT
using XSqlConnection = System.Data.SqlClient.SqlConnection;
#else
using                   MySql.Data.MySqlClient;
using XSqlConnection  = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand     = MySql.Data.MySqlClient.MySqlCommand;
using XSqlDataReader  = MySql.Data.MySqlClient.MySqlDataReader;
using XSqlDbType      = MySql.Data.MySqlClient.MySqlDbType;
using XSqlParameter   = MySql.Data.MySqlClient.MySqlParameter;
using XSqlException   = MySql.Data.MySqlClient.MySqlException;
using XSqlDataAdapter = MySql.Data.MySqlClient.MySqlDataAdapter;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;
using System.IO;
#endif

public static class ZXC
{
   #region Global Variables

   //static int pero, mujo, tamara, nebojsa;

   public static string[] MainArgs;

   public static bool ThisIsVektorProject = true;
   public static bool ThisIsSurgerProject = false;
   public static bool ThisIsRemonsterProject = false;
   public static bool ThisIsSkyLabProject = false;
   public static bool ThisIsHektorProject = false;
   public static bool ThisIsJanitorProject = false;

   public static bool ThisIsQUKATZ = false;

   public static VvForm TheVvForm { get; set; }
   public static Form CurrentForm { get; set; }

   #region ActiveDocumentHost  (Phase 1a / C1)

   // Faza 1a / sub-korak C1 (DevExpress_Migration_V4.md §3.1a)
   // Apstrakcija "tko je trenutno aktivan host UI-a" — tip 'object' (IVvDocumentHost dolazi u 1b).
   // U Fazi 1a jedini registrirani host je ZXC.TheVvForm.

   private static readonly object              _documentHostsSync = new object();
   private static readonly List<object>        _documentHosts     = new List<object>();
   private static          object              _activeDocumentHost;

   public static object ActiveDocumentHost
   {
      get { lock(_documentHostsSync) { return _activeDocumentHost; } }
   }

   public static List<object> DocumentHosts
   {
      get { lock(_documentHostsSync) { return new List<object>(_documentHosts); } }
   }

   public static void RegisterDocumentHost(object host)
   {
      if(host == null) return;
      lock(_documentHostsSync)
      {
         if(_documentHosts.Contains(host)) return;
         _documentHosts.Add(host);
         if(_activeDocumentHost == null) _activeDocumentHost = host;
      }
   }

   public static void UnregisterDocumentHost(object host)
   {
      if(host == null) return;
      lock(_documentHostsSync)
      {
         _documentHosts.Remove(host);
         if(ReferenceEquals(_activeDocumentHost, host))
         {
            _activeDocumentHost = _documentHosts.Count > 0 ? _documentHosts[0] : null;
         }
      }
   }

   public static void SetActiveDocumentHost(object host)
   {
      if(host == null) return;
      lock(_documentHostsSync)
      {
         if(_documentHosts.Contains(host)) _activeDocumentHost = host;
      }
   }

   #endregion ActiveDocumentHost  (Phase 1a / C1)

   #region Path Providers  (Phase 1a / C4)

   // Faza 1a / sub-korak C4 (DevExpress_Migration_V4.md §3.1a)
   // Indirekcija ka VvForm-ovim helperima za putanje: ZXC metode (aim_log_file_name,
   // VvSerializedDR_DirectoryName) više NE zovu VvForm tipom direktno, nego kroz
   // delegate providere koje VvForm postavi u InitializeVvForm().
   //
   // Napomena: ne cache-amo rezultat jer ovisi o dinamickom stanju (PUG_ID, vvDB_User).
   // Fallback na VvForm putanju ostaje radi sigurnosti ako provider nije postavljen
   // (npr. dijagnostika prije InitializeVvForm()).

   public static Func<bool, string>   ProjectAndUserDocumentsLocationProvider;
   public static Func<string, string> LocalDirectoryForVvFileProvider;

   #endregion Path Providers  (Phase 1a / C4)

   #region Status Text Sink  (Phase 1a / C5)

   // Faza 1a / sub-korak C5 (DevExpress_Migration_V4.md §3.1a)
   // ZXC.SetStatusText / ClearStatusText vise ne diraju TheVvForm.TStripStatusLabel
   // direktno, nego idu kroz delegate sink koji VvForm postavi u InitializeVvForm().
   // U Fazi 3 (detach) sink ce rutirati kroz ActiveDocumentHost (svaka VvFloatingForm
   // ima vlastiti status label) — tada cemo ovdje samo zamijeniti implementaciju
   // postavljanja sink-a, a call siteovi (Mixer/Placa/Ptrane/Person/Htrans...) ostaju
   // nepromijenjeni.

   public static Action<string> StatusTextSetter;
   public static Action         StatusTextClearer;

   #endregion Status Text Sink  (Phase 1a / C5)

   public static VvFont vvFont;
   public static VvColorsAndStyles vvColors;

   public static bool ShouldForceSifrarRefreshing;

   public static string LastExportFileName;
   public static string LastExportFileHASHcode;

   public static bool LoadImportFile_HasErrors = false;
   public static bool LoadCrystalReports_HasErrors = false;
 //public static bool CRinstallationIsInitiated = false;

   public static ZXC.VektorSiteEnum VvDeploymentSite { get; set; }

   public static List<VvSQL.VvUcListMember> VvUserControlList;
   public static List<VvRiskMacro> VvRiskMacroList;
   public static List<CdAndName_CommonStruct> VvTableMaintenanceList;
   public static List<CdAndName_CommonStruct> Received_ZPC_List;

   public static List<Halmed_SVD.HALMEDartikl                  > hArtiklList               ; // ona dobivena iz Excel-a pa u TXT pa u MySQL table 'HALMED_artikl' 
   public static List<Halmed_SVD.OblikLijeka_tablicaOblikLijeka> hArtiklList_FromXML_wATK11; // ova dolazi iz XML-a gdje ima s_lio + ATK11 ali!!! nema <naziv>, <mj_ozn>, <obl_ozn>, <par_naziv>

   //02.10.2017: 
   //public static bool NO_Internet = false;

   // 11.06.2015: puse public static bool BeSilentOnNotFoundError = false;

   public static bool Fak2NalAutomationChecked = false;

   public static bool RISK_Cache_Checked = false;
   public static bool RISK_PrNabCij_Checked = false;
   public static bool RISK_BadMSU_Checked = false;
   public static bool RISK_NOTfisk_Checked = false;
   public static bool RISK_TtNum_Slijednost_Checked = false;
   public static bool DUC_UnlinkedTranses_Checked = false;
 //public static bool OldVvXmlDRfilesDeleted = false;
   public static bool VvXmlDR_LastDocumentMissing_AlertRaised = false;

   public static bool DumpChosenOtsList_OnNalogDUC_InProgress = false;
   public static bool Restore_FromVvXmlDR_InProgress = false;

   public static string LastUsedProjektTT = Faktur.TT_RNP;
   public static bool LastUsedProjektTT_IsDiscovered = false;
   public static bool OffixImport_InProgress = false;
   public static bool LoadPoprat_InProgress = false;
   public static bool RenewCache_InProgress = false;
   public static bool CopyOut_InProgress = false;
   public static bool RewriteAllDocuments_InProgress = false;
   public static bool PutRiskFilterFieldsInProgress = false;
   public static bool RepairMissingFakturEx_InProgress = false;
   public static bool RISK_ToggleKnDeviza_InProgress = false;
   public static bool RISK_InitZPCvalues_InProgress = false;
   public static bool RISK_PULXPIZX_Calc_InProgress = false;
   public static bool RESET_InitialLayout_InProgress = false;
   public static bool MenuReset_SvDUH_ZAHonly_InProgress = false;
   public static bool GOST_SOBA_BOR_SetOtherData_InProgress = false;

   public static bool   FakturList_To_PDF_InProgress = false;
   public static ushort FakturList_To_PDF_subDsc     =     0;

   public static bool ForceXtablePreffix = false;
   public static bool KOPfromFUG_InProgress = false;
   public static bool KOPfromUGAN_InProgress = false;
   public static int  KOPfromUGAN_TabIndex = 0;
   public static bool InitializeApplication_InProgress = false;
   private static bool ShouldPing { get { return InitializeApplication_InProgress == false; } } // za brzi ulazak u program, a kasnije 'ko ga 'ebe    
   internal static bool IsRipley7or22 { get { return Environment.MachineName == "RIPLEY7" || Environment.MachineName == "RIPLEY22"; } } // za brzi ulazak u program, 
   private static bool IsVvKristal { get { return Environment.MachineName.StartsWith("VVKRISTAL"); } } // za brzi ulazak u program, 
   public static bool IsRipleyOrKristal { get { return IsRipley7or22 || IsVvKristal; } }

   public static List<string> ErrorsList = null;
   public static bool RISK_NewCache_InProgress = false;
   public static bool RISK_CopyToOtherDUC_inProgress = false;
   public static bool RISK_CopyToMixerDUC_inProgress = false;
   public static bool MIXER_CopyToOtherDUC_inProgress = false;
   public static bool RISK_AutoAddInventuraDiff_inProgress = false;
   public static bool RISK_SaveVvDataRecord_inProgress = false;
   public static bool RISK_CheckPrNabDokCij_inProgress = false;
   public static bool RISK_CheckZPCkol_inProgress = false;
   public static bool RISK_PromjenaNacPlac_inProgress = false;
   public static bool RISK_FinalRn_inProgress = false;
   public static bool GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS_inProgress = false;
   public static bool DupCopyMenu_inProgress = false;

   public static bool ShouldSupressRenewCache { get { return RISK_CheckPrNabDokCij_inProgress || RISK_CheckZPCkol_inProgress || RISK_DisableCacheTemporarily; } }
   public static bool RISK_DisableCacheTemporarily = false;
   public static bool IsVELEFORM = false;

   public static bool RISK_DisableAutoFiskTemporarily = false;

   public static bool VerboseLOG = false/*true*/;

   public static bool RISK_FiskParagon_InProgress = false;
   public static bool RISK_Edit_RtranoOnly_InProgress = false;

   public static bool RISK_VvPDFreporter_InProgress = false;
   public static bool LoadIzvodDLG_isON = false;

   public static bool M2PAY_API_Initialized = false;
   public static bool M2PAY_Device_Connected = false;

   public static com.handpoint.api.FinancialStatus M2PAY_AuthorizationStatus = com.handpoint.api.FinancialStatus.UNDEFINED;

   public static string UmjetninaGrCD = "UMJ";
   public static string MotVoziloGrCD = "MOT";
   public static string RabMotVozGrCD = "RMV";

   public static string PTG_PENDING_SernoPreffix = "~~???~~ ";
   public static string PTG_OLFA_SernoPreffix    = "~~~!~~~ ";
   public static string PCK_TS  = "PCK";
   public static string KMP_TS  = "KMP";
   public static string ROB_TS  = "ROB";
   public static string USL_TS  = "USL";
   public static string OTH_TS  = "OST";
   public static string UDP_TS  = "UDP";
   public static string AVA_TS  = "AVA"; // Avansi 2026 

   public static string RAM_GR1 = "RAM";
   public static string HDD_GR1 = "HDD";
   public static string PTG_ZNJ = "ZNJ";
   public static string PTG_UNJ = "UNJ";

   public static string eRacuniDIR;

   public static uint DebugCount = 0;
   public static int ThisApplicationRunningInstancesCount;

   public static VvDataBaseInfo VvDataBaseInfoInUse;

   // TODO: ovdje sada treba resetirati sve potrebite. Koji su to, otkriti ces s vremenom kada budu javljali bug-ove. 
   internal static void ResetAll_GlobalStatusVariables()
   {
      Fak2NalAutomationChecked = false;
      LastUsedProjektTT_IsDiscovered = false;
   }

   // 30.03.2015: stavio init ZXC.RRD u InitializeVvDao() 
   public static RiskRulesDsc RRD { get; set; }
   public static KtoShemaDsc KSD { get; set; }
   //private static bool riskRulesLoaded = false;
   //private static RiskRulesDsc rrd;
   //public  static RiskRulesDsc RRD
   //{
   //   get 
   //   {
   //      if(riskRulesLoaded == false)
   //      {
   //         rrd = new RiskRulesDsc(ZXC.dscLuiLst_riskRules);
   //         riskRulesLoaded = true;
   //      }
   //      return rrd; 
   //   }
   //   set { ZXC.rrd = value; }
   //}

   public static bool IsRNMnotRNP { get { return KSD.Dsc_otp_obrProTT == Faktur.TT_RNM; } }

   public static UTF8Encoding VvUTF8Encoding_noBOM;

   public readonly static System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

   public static kamtbl kamtbl_rec;

   public static bool menuStripWasVisible = false;

   public static string FindART_NabOrProdKAT = Artikl.ProdRobaGrCD;

   // 26.03.2024: sa smo skuzili da tu fali GR - grcka i XI sj irska  
   // a tu je pak EL (grcka?) koje nema u excelici na intrastat sajtu 
   public static string[] EU_VatCodes_woHR = new string[]
 //{ "AT", "BE", "BG", "CY", "CZ", "DK", "EE", "FI", "FR", "DE", "EL", "HU", "IE", "IT", "LV", "LT", "LU", "MT", "NL", "PL", "PT", "RO", "SK", "SI", "ES", "SE" };
   { "AT", "BE", "BG", "CY", "CZ", "DK", "EE", "FI", "FR", "DE", "EL", "HU", "IE", "IT", "LV", "LT", "LU", "MT", "NL", "PL", "PT", "RO", "SK", "SI", "ES", "SE", "GR", "XI" };

   public static string PTG_lastUsedSerno = "";

#if !DEBUG
   public static F2_Provider_enum F2_TheProvider { get { return IsF2_2026_rules ? CURR_prjkt_rec.F2_Provider : F2_Provider_enum.MER; } }
#else
   public static F2_Provider_enum F2_TheProvider { get { return                   CURR_prjkt_rec.F2_Provider                       ; } }
#endif

   public static bool IsF2_2026_rules { get { return Vv_eRacun_HTTP.IsF2_2026_rules; } }

   #endregion Global Variables

   #region ZXC for MySQL

   //#region TheMainDbConnection Management ORIG

   //private static XSqlConnection theMainDbConnection;
   //public static XSqlConnection  TheMainDbConnection
   //{
   //   get 
   //   {
   //      // 3.6.2011: pokusaj da rijesis kada konekcija 'pukne' u radu, pa da se proba rekonektirati.
   //      // ...ali ovo rjesenje ima bug-ova i na radi uvijek, a i stvara neke side efekte. Cilj je rekonektirati kada si u npr ispravi, a pukne veza ali da kada se vratri mozes sejvati
   //      // ov, dakle, ostaje jedan TODO: ! 
   //      //if(theMainDbConnection.State != ConnectionState.Open)
   //      //{
   //      //   theMainDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, theMainDbConnection.Database);
   //      //}

   //      return theMainDbConnection;         
   //   }
   //   set { theMainDbConnection = value; }
   //}

   //public static XSqlConnection PrjConnection
   //{
   //   get 
   //   {
   //      string prjktDB_name = ZXC.TheVvForm.GetvvDB_prjktDB_name();

   //      ZXC.SetMainDbConnDatabaseName(prjktDB_name);

   //      return ZXC.TheMainDbConnection; 
   //   }
   //}

   //public static void SetMainDbConnDatabaseName(string dbName)
   //{
   //   if(ZXC.TheMainDbConnection.Database != dbName)
   //   {
   //      ZXC.TheMainDbConnection.ChangeDatabase(dbName);
   //   }
   //}

   //internal static XSqlConnection theSecondDbConnection;
   //public   static XSqlConnection TheSecondDbConn_SameDB
   //{
   //   get 
   //   {
   //      if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open)
   //      {
   //         theSecondDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, ZXC.TheMainDbConnection.Database);
   //      }
   //      if(theSecondDbConnection.Database != ZXC.TheMainDbConnection.Database)
   //      {
   //         theSecondDbConnection.ChangeDatabase(ZXC.TheMainDbConnection.Database);
   //      }

   //      return ZXC.theSecondDbConnection;
   //   }

   //}
   //public   static XSqlConnection TheSecondDbConn_SameDB_prevYear
   //{
   //   get 
   //   {
   //      string dbName = VvDB_NameConstructor((projectYearFirstDay.Year - 1).ToString(), CURR_prjkt_rec.Ticker, CURR_prjkt_rec.KupdobCD);

   //      if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open)
   //      {
   //         theSecondDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, dbName);
   //      }
   //      if(theSecondDbConnection.Database != dbName)
   //      {
   //         theSecondDbConnection.ChangeDatabase(dbName);
   //      }

   //      return ZXC.theSecondDbConnection;
   //   }

   //}
   //public   static XSqlConnection TheSecondDbConn_SameDB_OtherYear(int otherYear)
   //{
   //      string dbName = VvDB_NameConstructor(otherYear.ToString(), CURR_prjkt_rec.Ticker, CURR_prjkt_rec.KupdobCD);

   //      if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open)
   //      {
   //         theSecondDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, dbName);
   //      }
   //      if(theSecondDbConnection.Database != dbName)
   //      {
   //         theSecondDbConnection.ChangeDatabase(dbName);
   //      }

   //      return ZXC.theSecondDbConnection;
   //}
   //public   static XSqlConnection TheSecondDbConn_OtherDB(string dbName)
   //{
   //   if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open)
   //   {
   //      theSecondDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, dbName);
   //   }
   //   else if(theSecondDbConnection.Database != dbName)
   //   {
   //      theSecondDbConnection.ChangeDatabase(dbName);
   //   }

   //   return ZXC.theSecondDbConnection;
   //}

   //internal static XSqlConnection theThirdDbConnection;
   //public   static XSqlConnection TheThirdDbConn_SameDB
   //{
   //   get 
   //   {
   //      if(theThirdDbConnection == null || theThirdDbConnection.State != ConnectionState.Open)
   //      {
   //         theThirdDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, ZXC.TheMainDbConnection.Database);
   //      }
   //      else if(theThirdDbConnection.Database != ZXC.TheMainDbConnection.Database)
   //      {
   //         theThirdDbConnection.ChangeDatabase(ZXC.TheMainDbConnection.Database);
   //      }

   //      return ZXC.theThirdDbConnection;
   //   }
   //}
   //public   static XSqlConnection TheThirdDbConn_OtherDB(string dbName)
   //{
   //   if(theThirdDbConnection == null || theThirdDbConnection.State != ConnectionState.Open)
   //   {
   //      theThirdDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, dbName);
   //   }
   //   else if(theThirdDbConnection.Database != dbName)
   //   {
   //      theThirdDbConnection.ChangeDatabase(dbName);
   //   }
   //   return ZXC.theThirdDbConnection;
   //}

   //#endregion TheMainDbConnection Management ORIG

   #region TheMainDbConnection Management NEW

   private static XSqlConnection theMainDbConnection;

   public static XSqlConnection TheMainDbConnection
   {
      //get 
      //{
      //   // 3.6.2011: pokusaj da rijesis kada konekcija 'pukne' u radu, pa da se proba rekonektirati.
      //   // ...ali ovo rjesenje ima bug-ova i na radi uvijek, a i stvara neke side efekte. Cilj je rekonektirati kada si u npr ispravi, a pukne veza ali da kada se vratri mozes sejvati
      //   // ov, dakle, ostaje jedan TODO: ! 
      //   if(theMainDbConnection.State != ConnectionState.Open)
      //   {
      //    theMainDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, theMainDbConnection.Database);
      //      // 31.08.2012: 
      //      //theMainDbConnection.Open();
      //   }

      //   return theMainDbConnection;         
      //}

      // 01.09.2012: hvala Bogu, sada mi radi ako conn presisa wait_timeout servera, a Bogami i ako prilikom 'ispravi' server ode down pa onda opet up - mozes sejvati!        
      // caka je bila da ti nije dovoljno provjeravati ConnectionState nego si dodao i Ping(). Tada, ako nije ziva, moras napraviti NOVU conn. Nije dovoljno samo conn.Open()! 
      get
      {
         // 27.06.2014: ipak ubio ovaj 'theMainDbConnection.Ping()' jer hebeno usporava a izgleda da radi i baz toga. Provjeri!!! 
         //if(theMainDbConnection == null || theMainDbConnection.State != ConnectionState.Open   || theMainDbConnection.Ping() == false)
         //if(theMainDbConnection == null || theMainDbConnection.State != ConnectionState.Open /*|| theMainDbConnection.Ping() == false*/)
         // 01.12.2014: vratio Ping() provjeru jer cloud conn cesto javi timeout... 
         //if(theMainDbConnection == null || theMainDbConnection.State != ConnectionState.Open /*|| theMainDbConnection.Ping() == false*/)
         if(theMainDbConnection == null || theMainDbConnection.State != ConnectionState.Open || (ShouldPing && theMainDbConnection.Ping() == false))
         {
            if(theMainDbConnection != null) theMainDbConnection.Close();

            // 14.02.2013: 
            if(ZXC.vvDB_Server != null && ZXC.vvDB_User != null && ZXC.vvDB_Password != null)
            {
               theMainDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, theMainDbConnection.Database, false);
            }
         }
         return theMainDbConnection;
      }
      set { theMainDbConnection = value; }

   }

   public static XSqlConnection PrjConnection
   {
      get
      {
         string prjktDB_name = ZXC.VvDB_prjktDB_Name;

         ZXC.SetMainDbConnDatabaseName(prjktDB_name);

         return ZXC.TheMainDbConnection;
      }
   }

   public static void SetMainDbConnDatabaseName(string dbName)
   {
      if(theMainDbConnection.Database != dbName)
      {
         theMainDbConnection.ChangeDatabase(dbName);
      }
   }

   public static void SetSkyDbConnDatabaseName(string dbName)
   {
      if(theSkyDbConnection.Database != dbName)
      {
         theSkyDbConnection.ChangeDatabase(dbName);
      }
   }

   internal static XSqlConnection theSecondDbConnection;
   public static XSqlConnection TheSecondDbConn_SameDB
   {
      get
      {
         //if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open)
         //if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open   || theSecondDbConnection.Ping() == false)
         //if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open /*|| theSecondDbConnection.Ping() == false*/)
         if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open || (ShouldPing && theSecondDbConnection.Ping() == false))
         {
            if(theSecondDbConnection != null) theSecondDbConnection.Close();
            theSecondDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, theMainDbConnection.Database);
         }
         if(theSecondDbConnection.Database != theMainDbConnection.Database)
         {
            theSecondDbConnection.ChangeDatabase(theMainDbConnection.Database);
         }

         return ZXC.theSecondDbConnection;
      }

   }
   public static XSqlConnection TheSecondDbConn_SameDB_prevYear
   {
      get
      {
         string dbName = VvDB_NameConstructor((projectYearFirstDay.Year - 1).ToString(), CURR_prjkt_rec.Ticker, CURR_prjkt_rec.KupdobCD);

         //if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open)
         //if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open   || theSecondDbConnection.Ping() == false)
         //if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open /*|| theSecondDbConnection.Ping() == false*/)
         if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open || (ShouldPing && theSecondDbConnection.Ping() == false))
         {
            if(theSecondDbConnection != null) theSecondDbConnection.Close();
            theSecondDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, dbName);
         }
         if(theSecondDbConnection.Database != dbName)
         {
            theSecondDbConnection.ChangeDatabase(dbName);
         }

         return ZXC.theSecondDbConnection;
      }

   }
   public static XSqlConnection TheSecondDbConn_SameDB_OtherYear(int otherYear)
   {
      string dbName = VvDB_NameConstructor(otherYear.ToString(), CURR_prjkt_rec.Ticker, CURR_prjkt_rec.KupdobCD);

      //if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open)
      //if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open   || theSecondDbConnection.Ping() == false)
      //if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open /*|| theSecondDbConnection.Ping() == false*/)
      if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open || (ShouldPing && theSecondDbConnection.Ping() == false))
      {
         if(theSecondDbConnection != null) theSecondDbConnection.Close();
         theSecondDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, dbName);
      }
      if(theSecondDbConnection.Database != dbName)
      {
         theSecondDbConnection.ChangeDatabase(dbName);
      }

      return ZXC.theSecondDbConnection;
   }
   public static XSqlConnection TheSecondDbConn_OtherDB(string dbName)
   {
      //if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open)
      //if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open   || theSecondDbConnection.Ping() == false)
      //if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open /*|| theSecondDbConnection.Ping() == false*/)
      if(theSecondDbConnection == null || theSecondDbConnection.State != ConnectionState.Open || (ShouldPing && theSecondDbConnection.Ping() == false))
      {
         if(theSecondDbConnection != null) theSecondDbConnection.Close();
         theSecondDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, dbName);
      }
      else if(theSecondDbConnection.Database != dbName)
      {
         theSecondDbConnection.ChangeDatabase(dbName);
      }

      return ZXC.theSecondDbConnection;
   }

   internal static XSqlConnection theThirdDbConnection;
   public static XSqlConnection TheThirdDbConn_SameDB
   {
      get
      {
         //if(theThirdDbConnection == null || theThirdDbConnection.State != ConnectionState.Open)
         //if(theThirdDbConnection == null || theThirdDbConnection.State != ConnectionState.Open   || theThirdDbConnection.Ping() == false)
         //if(theThirdDbConnection == null || theThirdDbConnection.State != ConnectionState.Open /*|| theThirdDbConnection.Ping() == false*/)
         if(theThirdDbConnection == null || theThirdDbConnection.State != ConnectionState.Open || (ShouldPing && theThirdDbConnection.Ping() == false))
         {
            if(theThirdDbConnection != null) theThirdDbConnection.Close();
            theThirdDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, theMainDbConnection.Database);
         }
         else if(theThirdDbConnection.Database != theMainDbConnection.Database)
         {
            theThirdDbConnection.ChangeDatabase(theMainDbConnection.Database);
         }

         return ZXC.theThirdDbConnection;
      }
   }
   public static XSqlConnection TheThirdDbConn_OtherDB(string dbName)
   {
      //if(theThirdDbConnection == null || theThirdDbConnection.State != ConnectionState.Open)
      //if(theThirdDbConnection == null || theThirdDbConnection.State != ConnectionState.Open   || theThirdDbConnection.Ping() == false)
      //if(theThirdDbConnection == null || theThirdDbConnection.State != ConnectionState.Open /*|| theThirdDbConnection.Ping() == false*/)
      if(theThirdDbConnection == null || theThirdDbConnection.State != ConnectionState.Open || (ShouldPing && theThirdDbConnection.Ping() == false))
      {
         if(theThirdDbConnection != null) theThirdDbConnection.Close();
         theThirdDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, dbName);
      }
      else if(theThirdDbConnection.Database != dbName)
      {
         theThirdDbConnection.ChangeDatabase(dbName);
      }
      return ZXC.theThirdDbConnection;
   }

   internal static XSqlConnection theSkyDbConnection;
   //public static bool DelmeLaterDebug46 { get { return ZXC.IsTEXTHOany2 &&  ZXC.vvDB_ServerID == 46                                                       ; } }
   public static bool DelmeLaterDebug46 { get { return ZXC.IsTEXTHOany2 && (ZXC.vvDB_ServerID == 46 || ZXC.vvDB_ServerID == 14 || ZXC.vvDB_ServerID == 98); } }
   public static XSqlConnection TheSkyDbConnection
   {
      // vidi eventualno gore opaske za 'TheSkyDbConnection' 
      get
      {
         //ping -w 1000 -t 79.143.181.154 > skyping.txt 

         DateTime TS_ID = DateTime.Now;

         if(ZXC.DelmeLaterDebug46) { ZXC.aim_log("SkyDbConn_01 TS_ID {0} sp: {1}\n\r\t", TS_ID.ToString("HH:mm:ss"), ShouldPing); }

         // 06.09.2018: HUGE NEWS!:
         // MSqlConnection.Ping() se ponasa nekontrolirano. Za MS Sql Connection '.Ping()' uopce niti ne postoji 
         // MySql dokumentacija navodi da treba koristiti ' is_connected()' umjesto .Ping() jer ping vraca nekakav InterfaceError? a ne bool?! 
         // Do dalnjega prestajemo pingati ali samo kod Sky conn. TheMainDbConnection ostaje ista 
         // u 2018 TH 46 Đakovo je svakodnevno imalo probleme sa ovim TheSkyDbConnection.Ping(). Niti da vrati true niti da digne Exception i upise ga u aim_log! 
         //if(theSkyDbConnection == null || theSkyDbConnection.State != ConnectionState.Open || (ShouldPing && theSkyDbConnection.Ping() == false))
         if(theSkyDbConnection == null || theSkyDbConnection.State != ConnectionState.Open)
         {
            if(ZXC.DelmeLaterDebug46)
            {
               if(theSkyDbConnection == null) ZXC.aim_log("SkyDbConn_02 TS_ID {0} connIsNull\n\r\t", TS_ID.ToString("HH:mm:ss"));
               else ZXC.aim_log("SkyDbConn_02 TS_ID {0} state: {1} \n\r\t", TS_ID.ToString("HH:mm:ss"), theSkyDbConnection.State.ToString());
            }
            if(theSkyDbConnection != null) theSkyDbConnection.Close();

            if(ZXC.DelmeLaterDebug46) ZXC.aim_log("SkyDbConn_03 TS_ID {0} AFT eventual .Close() \n\r\t", TS_ID.ToString("HH:mm:ss"));

            if(ZXC.CURR_prjkt_rec.SkySrvrHostDecrypted != null && ZXC.vvDB_skyUserName != null && ZXC.CURR_prjkt_rec.SkyPasswordDecrypted != null)
            {
               string skyDbName = CURR_prjkt_rec.SkyVvDomena + "_" + ZXC.vvDB_prefix + projectYear + "_" + CURR_prjkt_rec.Ticker + "_" + CURR_prjkt_rec.KupdobCD.ToString("000000");

               if(ZXC.DelmeLaterDebug46) ZXC.aim_log("SkyDbConn_04 TS_ID {0} BEF CREATE_AND_OPEN_XSqlConnection \n\r\t", TS_ID.ToString("HH:mm:ss"));

               theSkyDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.CURR_prjkt_rec.SkySrvrHostDecrypted, ZXC.vvDB_skyUserName, ZXC.CURR_prjkt_rec.SkyPasswordDecrypted, skyDbName, /*false ?!? je kod MainConn */ true);

               if(ZXC.DelmeLaterDebug46) ZXC.aim_log("SkyDbConn_05 TS_ID {0} AFT CREATE_AND_OPEN_XSqlConnection state: {1}\n\r\t", TS_ID.ToString("HH:mm:ss"), theSkyDbConnection.State.ToString());

            }
         }

         if(ZXC.DelmeLaterDebug46) ZXC.aim_log("SkyDbConn_06 TS_ID {0} conn is OK\n\r\t", TS_ID.ToString("HH:mm:ss"));

         return theSkyDbConnection;
      }
      set { theSkyDbConnection = value; }
   }

   #region MBF connection

   internal static XSqlConnection theMbfDbConnection;
   public static XSqlConnection TheMbfDbConnection
   {
      get
      {
         if(theMbfDbConnection == null || theMbfDbConnection.State != ConnectionState.Open || (ShouldPing && theMbfDbConnection.Ping() == false))
         {
            if(theMbfDbConnection != null) theMbfDbConnection.Close();

            if(MbfSrvrHost != null && ZXC.vvDB_skyUserName != null && MbfPassword != null)
            {
               //string MbfDbName = CURR_prjkt_rec.SkyVvDomena + "_" + TheVvForm.GetvvDB_prefix() + projectYear + "_" + CURR_prjkt_rec.Ticker + "_" + CURR_prjkt_rec.KupdobCD.ToString("000000");

               theMbfDbConnection =
                  VvSQL.CREATE_AND_OPEN_XSqlConnection(MbfSrvrHost, ZXC.vvDB_mbfUserName, MbfPassword, ZXC.vv_MBF_DbName, /*false ?!? je kod MainConn */ true);
            }
         }
         return theMbfDbConnection;
      }
      set { theMbfDbConnection = value; }
   }

   // 13.11.2016: 
   //private static string MbfSrvrHost { get { 
   //   return ZXC.CURR_prjkt_rec.SkySrvrHostDecrypted.IsEmpty() ?
   //      "123.456.78.90".Replace(".90", ".87").Replace(".78", ".64").Replace(".456", ".136").Replace("123.", "213.") : // pokusavamo kamuflirati string anti reverse engeneering 
   //      ZXC.CURR_prjkt_rec.SkySrvrHostDecrypted; } } 
   //
   private static string MbfSrvrHost { get {
         return ZXC.CURR_prjkt_rec.SkySrvrHostDecrypted.IsEmpty() ?
            "123.456.78.90".Replace(".90", ".154").Replace(".78", ".181").Replace(".456", ".143").Replace("123.", "79.") : // pokusavamo kamuflirati string anti reverse engeneering 
            ZXC.CURR_prjkt_rec.SkySrvrHostDecrypted; } }

   // 05.12.2018: 
 //private static string MbfPassword { get {
 //      return ZXC.CURR_prjkt_rec.SkyPasswordDecrypted.IsEmpty() ?
 //         "qwe1qwe2qwe3".Replace("qwe3", "implicitnovirtualne").Replace("qwe2", "svesumetode").Replace("qwe1", "uapstraktnimklasama") : // pokusavamo kamuflirati string anti reverse engeneering 
 //         ZXC.CURR_prjkt_rec.SkyPasswordDecrypted; } }

   private static string MbfPassword { get {
         return 
            "qwe1qwe2qwe3".Replace("qwe3", "implicitnovirtualne").Replace("qwe2", "svesumetode").Replace("qwe1", "uapstraktnimklasama");
   } }

   #endregion MBF connection

   #endregion TheMainDbConnection Management NEW

   //public static string vvReport_TableName = "ReportTable";

   public const string rptFNameExtension = ".rpt";
   public const string rptDirectoryName = "rTree";
   public const string vvReport_TableName = "IzvjTable";
   public const string vvReport_SifrarWithStatusDataTableName = "StatusTable";

   public const string copyrightName = "VIPER";
   public const string copyrightOIB = "60042587515";

   public static string projectYear;
   public static string projectPrevYear;
   public static string ogYY;
   public static DateTime projectYearFirstDay;
   public static DateTime projectYearLastDay;
   public static DateTime nextYearFirstDay;
   public static DateTime prevYearFirstDay;
   public static DateTime prevYearLastDay;
   public static DateTime prevYearDecembar;
   public static DateTime programStartedOnDateTime;
   public static int projectYearAsInt;

   public static bool IsSkipSplash = false;
   public static bool TodayIsInFirst20YearDays
   {
      get
      {
         // 02.01.2018: 
         //return programStartedOnDateTime.Date < projectYearFirstDay.AddDays(20)     ;
         return programStartedOnDateTime.Date < projectYearLastDay.AddDays(21).Date;
      }
   }

   public static DateTime NowYearFirstDay = new DateTime(DateTime.Now.Year, 01, 01);

   public static DateTime utilTS;

   public static TimeSpan OneWeekSpan   = new TimeSpan(4, 12, 0, 0) + new TimeSpan(2, 12, 0, 0);
   public static TimeSpan TwoWeekSpan   = OneWeekSpan + OneWeekSpan;
   public static TimeSpan ThreeWeekSpan = OneWeekSpan + OneWeekSpan + OneWeekSpan;
   public static TimeSpan FourWeekSpan  = OneWeekSpan + OneWeekSpan + OneWeekSpan + OneWeekSpan;
   public static TimeSpan FiveWeekSpan  = OneWeekSpan + OneWeekSpan + OneWeekSpan + OneWeekSpan + OneWeekSpan;
   public static TimeSpan SixWeekSpan   = OneWeekSpan + OneWeekSpan + OneWeekSpan + OneWeekSpan + OneWeekSpan + OneWeekSpan;
   public static TimeSpan SevenWeekSpan = OneWeekSpan + OneWeekSpan + OneWeekSpan + OneWeekSpan + OneWeekSpan + OneWeekSpan + OneWeekSpan;

   public static TimeSpan OneHourSpan  = new TimeSpan(0, 1, 0, 0);
   public static TimeSpan OneDaySpan   = new TimeSpan(1, 0, 0, 0);
   public static TimeSpan TwoDaySpan   = OneDaySpan + OneDaySpan;
   public static TimeSpan ThreeDaySpan = OneDaySpan + OneDaySpan + OneDaySpan;
   public static TimeSpan FourDaySpan  = OneDaySpan + OneDaySpan + OneDaySpan + OneDaySpan;
   public static TimeSpan FiveDaySpan  = OneDaySpan + OneDaySpan + OneDaySpan + OneDaySpan + OneDaySpan;
   public static TimeSpan SixDaySpan   = OneDaySpan + OneDaySpan + OneDaySpan + OneDaySpan + OneDaySpan + OneDaySpan;

   public static TimeSpan OneSecondSpan = new TimeSpan(0, 0, 0, 1);
   public static TimeSpan OneMinuteSpan = new TimeSpan(0, 0, 1, 0);

   public static DateTime Yesterday                   { get { return (DateTime.Now.Date              - OneSecondSpan); } }
   public static DateTime YesterdayYesterday          { get { return (DateTime.Now.Date - OneDaySpan - OneSecondSpan); } }
   public static DateTime YesterdayYesterdayYesterday { get { return (DateTime.Now.Date - TwoDaySpan - OneSecondSpan); } }

   public static int      ThisYearSeconds             { get { return ((int)(DateTime.Now - projectYearFirstDay).TotalSeconds)/*.Ron(0)*/; } }

   public static DateTime DateFromThisYearSeconds(int thisYearSeconds)
   {
      return (projectYearFirstDay + new TimeSpan(0, 0, 0, thisYearSeconds)); 
   }

   public static DateTime ExcelZeroDate = new DateTime(1900, 01, 01);

   public static DateTime PdvEU_EraDate = new DateTime(2013, 07, 01);

   public static DateTime FiskalEraDate = new DateTime(2013, 01, 01);
   public static bool IsFikalEra
   {
      get
      {
         return (projectYearFirstDay >= FiskalEraDate);
      }
   }

   public static DateTime Date01012010 = new DateTime(2010, 01, 01);
   public static DateTime Date01072010 = new DateTime(2010, 07, 01);
   public static DateTime Date01012013 = new DateTime(2013, 01, 01);
   public static DateTime Date01072013 = new DateTime(2013, 07, 01);
   public static DateTime Date01012014 = new DateTime(2014, 01, 01);
   public static DateTime Date01072014 = new DateTime(2014, 07, 01);
   public static DateTime Date01012015 = new DateTime(2015, 01, 01);
   public static DateTime Date01032015 = new DateTime(2015, 03, 01);
   public static DateTime Date15032015 = new DateTime(2015, 03, 15);

   public static DateTime Date01042015 = new DateTime(2015, 04, 01);
   public static DateTime Date07042015 = new DateTime(2015, 04, 07);
   public static DateTime Date12052015 = new DateTime(2015, 05, 12);
   public static DateTime Date02102015 = new DateTime(2015, 10, 02);
   public static DateTime Date01122016 = new DateTime(2016, 12, 01);
   public static DateTime Date01012017 = new DateTime(2017, 01, 01);
   public static DateTime Date01012018 = new DateTime(2018, 01, 01);
   public static DateTime Date01012019 = new DateTime(2019, 01, 01);
   public static DateTime Date01092019 = new DateTime(2019, 09, 01);
   public static DateTime Date01012020 = new DateTime(2020, 01, 01);
   public static DateTime Date27042020 = new DateTime(2020, 04, 27);
   public static DateTime Date01012021 = new DateTime(2021, 01, 01);
   public static DateTime Date01022021 = new DateTime(2021, 02, 01);
   public static DateTime Date01042021 = new DateTime(2021, 04, 01);
   public static DateTime Date01012022 = new DateTime(2022, 01, 01);
   public static DateTime Date01012023 = new DateTime(2023, 01, 01);
   public static DateTime Date31012023 = new DateTime(2023, 01, 31);
   public static DateTime Date01042023 = new DateTime(2023, 04, 01);
   public static DateTime Date17042023 = new DateTime(2023, 04, 17);
   public static DateTime Date06062023 = new DateTime(2023, 06, 06);
   public static DateTime Date01122023 = new DateTime(2023, 12, 01);
   public static DateTime Date01012024 = new DateTime(2024, 01, 01);
   public static DateTime Date04012024 = new DateTime(2024, 01, 04);
   public static DateTime Date09042024 = new DateTime(2024, 04, 09);
   public static DateTime Date01082025 = new DateTime(2025, 08, 01);
   public static DateTime Date01012026 = new DateTime(2026, 01, 01);

   public static DateTime EURoERAstart { get { return Date01012023; } }

   public static DateTime TexthoNewArtiklsDate   = new DateTime(2015, 07, 13);
   public static DateTime TexthoInventuraDate    = new DateTime(2025, 12, 31); // TODO: !!!!!!!!!!!! ovo treba svake godine prilagodavati 
   public static DateTime TexthoOneDayBefInvDate = new DateTime(TexthoInventuraDate.Year,
                                                                TexthoInventuraDate.Month,
                                                                TexthoInventuraDate.Day - 1);

 //public static DateTime MySQL_MIN_timpestamp = new DateTime(1970, 01, 01, 00, 00, 01);
   public static DateTime MySQL_MIN_timpestamp = new DateTime(1970, 01, 02, 00, 00, 02);

   public static DateTime IFA_IRA_PrintDate_ERA = new DateTime(2019, 07, 11); // jer je Deployano 10.07.2019 

   public static string initialPrjktTicker /*= "TESTIS"*/;
   public static uint initialPrjktKCD    /* = 12     */;

   //public static string initialPrjktTicker = "VIPER";
   //public static string ProjectYear = "2005";

#if(DEBUG)
   public static int vvLockTimeoutMinutes = /*1*/ 0;
#else
   public static int vvLockTimeoutMinutes = 10;
#endif
   // 10.07.2015: 
   //public static int vvSyncLockTimeoutMinutes = 8     ;
   public static int vvSyncLockTimeoutMinutes = 4 * 60;

   public static int vvLockTimeoutSeconds = 60 * vvLockTimeoutMinutes;

   public static int vvSyncLockTimeoutSeconds = 60 * vvSyncLockTimeoutMinutes;

   public static /*const*/ string vvDB_prefix = /*vvDB_wwwDBname_preffix +*/ "vv";
   //public const  string vvDB_prjktDB_Name           = "vvprojekti"   ;
   public static string vv_PRODUCT_Name = "Vektor";
   public const string vvDB_luiPrefix = "zz_";
   public const string vvDB_lockerTableName = "vvlocker";
   public const string vvDB_riskMacroTableName = "vvRiskMacro";
   public const string vvDB_procTableName = "vvprocess";
   public const string vvDB_ucListTableName = "vvusercontrol";
   public const string vvDB_SKYlockerTableName = "vvSKYlocker";

   //public const  string vvDB_OLD_logTableName       = "vvlog"   ;
   public const string vvDB_OLD_logTableName = "vvlanlog";

   public const string vvDB_LANlogTableName = "vvlogLAN";
   public const string vvDB_SKYlogTableNameBase = "vvlogSKY_";
   public const string vvDB_ERRlogTableNameBase = "vvlogERR_";
   public const string vvDB_CpyERRlogTableNameBase = "vvlogCpyERR_";
   //public static  string vvDB_LANlogTableName    { get { return vvDB_LANlogTableNameBase    + vvDB_ServerID.ToString("00"); } }
   public static string vvDB_SKYlogTableName { get { return vvDB_SKYlogTableNameBase + vvDB_ServerID.ToString("00"); } }
   public static string vvDB_ERRlogTableName { get { return vvDB_ERRlogTableNameBase + vvDB_ServerID.ToString("00"); } }
   public static string vvDB_CpyERRlogTableName { get { return vvDB_CpyERRlogTableNameBase + vvDB_ServerID.ToString("00"); } }

   public const string vvDB_HALMEDartiklTableName = "HALMED_artikl";

   public static/*const*/ string vv_VEKTOR_PRODUCT_Name = "Vektor";
   public static/*const*/ string vv_SURGER_PRODUCT_Name = "Surger";
   public static/*const*/ string vv_REMONSTER_PRODUCT_Name = "Remonster";
   public static/*const*/ string vv_SKYLAB_PRODUCT_Name = "SkyLab";
   public static/*const*/ string vv_MBF_DbName = "vvMBF";
   public static/*const*/ string vv_HEKTOR_PRODUCT_Name = "Hektor"; //
   // Sve ove inicijalizacije su sada preseljene u VvForm.GetLoginData():

   public const string F2_Unprocessed = "NEOBRAĐENO!";

   public static uint BigDataRecCountLimit { get { return 32000; } } // 

   //public static string vvDB_User = System.Environment.UserName;
   public static uint vvDB_ServerID;

   public static uint vvDB_ServerID_CENTRALA = 12;
   public static uint vvDB_ServerID_SkyCloud = 10;

   public static string vvDB_Server; // !!! !!! !!! !!! 
   public static bool vvDB_IsLocalhost { get { return vvDB_Server == "localhost"; } }
#if(DEBUG)
   public static int vvDB_Port = 3306/*7*/;
#else
   public static int    vvDB_Port = 3306;
#endif
   public static string vvDB_User     /*= "root"*/;
   public static string vvDB_Password /*= VvAES.EncryptData("1q1q1q", ZXC.vv_AES_key)*/;
   public static string vvDB_systemSuperUserName = "root";
   public static string vvDB_programSuperUserName = "superuser";
   public static string vvDB_skyUserName = "skywalker";
   public static string vvDB_mbfUserName = "mbfwalker";

   public static string vvDB_VvDomena = "";
   //public static bool vvDB_is_www { get { return vvDB_Server.Contains("www"); } }
   public static bool vvDB_is_www { get { return vvDB_VvDomena.NotEmpty(); } }
   public static string vvDB_www_preffix
   {
      get
      {
         if(vvDB_is_www) return vvDB_VvDomena + "_";
         else return "";
      }
   }

   // C6 (Phase 1a): standalone property — `PrjConnection` i `VvDB_NameConstructor()` čitaju izravno iz ZXC-a.
   // Value je izvedena iz `vvDB_www_preffix` (koji se postavlja pri loginu preko `vvDB_VvDomena`);
   // dead backing field + dead setter uklonjeni jer ih nitko nije koristio.
   public static string VvDB_prjktDB_Name
   {
      get { return vvDB_www_preffix + "vvektor"; }
   }

   public static string GetUserNameWithWwwPreffix(string cleanUserName)
   {
      if(cleanUserName == vvDB_mbfUserName) return vvDB_mbfUserName;

      return cleanUserName == "root" ? cleanUserName : vvDB_www_preffix + cleanUserName;
   }

   public static string GetCleanUserNameWithoutWwwPreffix(string userNameWithPreffix)
   {
      return userNameWithPreffix == "root" ? userNameWithPreffix : userNameWithPreffix.Replace(vvDB_www_preffix, ""); ;
   }

   // PUSE: public static string vvDB_path;

   /// <summary>
   /// Cio Prjkt objekt nastao na osnovi 'TheVvDatabaseInfoIn_ComboBox4Projects'
   /// kod selected index changed ComboBox4Projects-a
   /// </summary>
   public static Prjkt CURR_prjkt_rec;
   public static User CURR_user_rec;

   public static Kupdob SvDUH_ZAHonlyKupdob_rec;

   public static string PUG_ID { get { return                          CURR_prjkt_rec.Ticker + projectYear               ; } }
 //public static string PUG_ID { get { return CURR_prjkt_rec != null ? CURR_prjkt_rec.Ticker + projectYear : "tmp_PUG_ID"; } }

   #region ROOT_prjkt_rec

   // 18.10.2017: 
   /*private*/
   internal static bool userSifrarLoaded = false;

   //public  static uint  rootPrjktKCD;
   public static ZXC.VvDataBaseInfo lowestPrjktCdDBI;
   /*private*/
   internal static bool root_prjkt_rec_loaded = false;
   private static Prjkt root_prjkt_rec;
   public static Prjkt ROOT_prjkt_rec
   {
      get
      {
         bool OK = true;

         if(root_prjkt_rec_loaded == false)
         {
            root_prjkt_rec = new Prjkt();

            // 19.12.2010: RefreshDataBasesInfo() ti vraca najnizu sifru u Prjkt tablici za zeljenu godinu i to ce ti biti Root_prjkt_rec!

            OK = root_prjkt_rec.VvDao.SetMe_Record_bySomeUniqueColumn(PrjConnection, root_prjkt_rec, lowestPrjktCdDBI.ProjectCode, ZXC.PrjktSchemaRows[ZXC.KpdbCI.kupdobCD], false);

            if(OK) root_prjkt_rec_loaded = true;
         }

         return (OK ? ZXC.root_prjkt_rec : null);
      }

   }

   public static string ROOT_Ticker { get { return ROOT_prjkt_rec.Ticker; } }
   public static uint ROOT_PrjktCd { get { return ROOT_prjkt_rec.KupdobCD; } }

   #endregion ROOT_prjkt_rec

   /// <summary>
   /// Zapravo ZXC.vvDB_User. Nastao pri logiranju u program via LoginForm-a.
   /// </summary>
   public static string CURR_userName { get { return vvDB_User; } }

   /// <summary>
   /// Lista svih Prvlg-ja po CURR_prjkt i CURR_user kod selected index changed ComboBox4Projects-a
   /// </summary>
   public static List<Prvlg> CURR_Privileges;
   public static List<SkyRule> CURR_SkyRules;

   public static ZXC.VvDataBaseInfo TheVvDatabaseInfoIn_ComboBox4Projects;

   public const string vv_User_AES_key = "iterasupoiberuvjoatreborratsenam";
   public const string vv_Login_AES_key = "qmwnebrvtcyxuzi'o;pl[k]jahsgdf";

   public static string EncryptThis_UserUC_Password(string humanPassword, string userName)
   {
      //if(userName == "root"   || (ZXC.vvDB_is_www /*&& userName.Contains(ZXC.vvDB_www_preffix)*/)) return humanPassword; // ostavi root-ov password unchanged non encrypted! 
      if(userName == "root" /*|| (ZXC.vvDB_is_www /*&& userName.Contains(ZXC.vvDB_www_preffix))*/) return humanPassword; // ostavi root-ov password unchanged non encrypted! 

      return humanPassword.NotEmpty() ? VvAES.EncryptData(humanPassword, ZXC.vv_User_AES_key) : "";
   }

   public static string EncryptThis_LoginForm_Data(string humanPassword)
   {
      return humanPassword.NotEmpty() ? VvAES.EncryptData(humanPassword, ZXC.vv_Login_AES_key) : "";
   }

   public static string VvDB_NameConstructor(string _year, string _ticker, uint _ID)
   {
      // 29.12.2016: 
    //return ZXC.vvDB_www_preffix + TheVvForm.GetvvDB_prefix() + _year + "_" + _ticker          + "_" + _ID.ToString("000000");
      return ZXC.vvDB_www_preffix + ZXC.vvDB_prefix + _year + "_" + _ticker.AsUTF8() + "_" + _ID.ToString("000000");
   }

   public static int sqlErrNo;
   public static string sqlErrMessage = "";

   public static string GetExcelConnectionString(string _fName, bool isNew)
   {
      return GetExcelConnectionString(_fName, isNew, false);
   }

   public static string GetExcelConnectionString(string _fName, bool isNew, bool noHeader)
   {
      string connString = "";

      if(isNew) connString = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + _fName + @"; Extended Properties=""Excel 12.0 Xml;HDR=YES;IMEX=1""";
      else connString = @"Provider=Microsoft.Jet.OLEDB.4.0;  Data Source=" + _fName + @"; Extended Properties=""Excel 8.0;HDR=Yes;IMEX=1""";

      if(noHeader) connString = connString.Replace("HDR=YES;", "HDR=NO;");

      return connString;
   }

   public static bool CurrUserHasSuperPrivileges
   {
      get
      {
         return (CURR_userName == vvDB_systemSuperUserName  ||
                 CURR_userName == vvDB_programSuperUserName ||
                 CURR_user_rec.IsSuper);
      }
   }

   public static bool CurrUserNameIs_Root_Or_Superuser
   {
      get
      {
         return (CURR_userName == vvDB_systemSuperUserName ||
                 CURR_userName == vvDB_programSuperUserName);
      }
   }

   public static bool Curr_TH_userName_Nema_ADD_privileges
   {
      get
      {
         bool isThisUserName_VoditeljRegije = ZXC.DoesThisStringEndsWithTwoDigits(CURR_userName) == false;

         if(isThisUserName_VoditeljRegije) return false; // voditeljima regija se dozvoljava ADD akcija a da je voditelj   
                                                         // regije znamo po tome stu mu userName NE zavrsava sa 2 znamenke 

         return (CURR_userName != vvDB_programSuperUserName && CURR_user_rec.IsSuper == true);
      }
   }

   public static string[] NoNeedForZaliha_ArtiklTSs;
   public static string[] IsMinusOK_or_UDP_ArtiklTS_array;

   #endregion ZXC for MySQL

   //#if OldVektor
   #region VvLookUps

   public static VvLookUpLista luiListaPorPla;
   public const string luiListaPorPla_Name = "PorPla";

   public static VvLookUpLista luiListaOpcina;
   public const string luiListaOpcina_Name = "Opcine";

   public static VvLookUpLista luiListaSkladista;
   public const string luiListaSkladista_Name = "Skladista";

   public static VvLookUpLista luiListaNalogTT;
   public const string luiListaNalogTT_Name = "NalogTT";

   public static VvLookUpLista luiListaZupanija;
   public const string luiListaZupanija_Name = "Zupanije";

   public static VvLookUpLista luiListaDjelat;
   public const string luiListaDjelat_Name = "Djelatnosti";

   public static VvLookUpLista luiListaPrvlgScope;
   public const string luiListaPrvlgScope_Name = "PrvlgScope";

   public static VvLookUpLista luiListaPrvlgType;
   public const string luiListaPrvlgType_Name = "PrvlgType";

   public static VvLookUpLista luiListaFakturType;
   public const string luiListaFakturType_Name = "FakturType";

   public static VvLookUpLista luiListaGrupaPartnera;
   public const string luiListaGrupaPartnera_Name = "GrupaPartnera";

   public static VvLookUpLista luiListaGodina;
   public const string luiListaGodina_Name = "Godine";

   public static VvLookUpLista luiListaNTR_BilancaMP;
   public const string luiListaNTR_BilancaMP_Name = "NTR_BilancaMP";

   public static VvLookUpLista luiListaNTR_RDiGMP;
   public const string luiListaNTR_RDiGMP_Name = "NTR_RDiGMP";

   public static VvLookUpLista luiListaNTR_ObrPD;
   public const string luiListaNTR_ObrPD_Name = "NTR_ObrPD";

   public static VvLookUpLista luiListaNTR_TSIPod;
   public const string luiListaNTR_TSIPod_Name = "NTR_TSIPod";

   public static VvLookUpLista luiListaNTR_PPI;
   public const string luiListaNTR_PPI_Name = "NTR_PPI";

   public static VvLookUpLista luiListaNTR_PPI2;
   public const string luiListaNTR_PPI2_Name = "NTR_PPI2";

   public static VvLookUpLista luiListaNTR_KPI;
   public const string luiListaNTR_KPI_Name = "KPI";

   public static VvLookUpLista luiListaNTR_KPI24;
   public const string luiListaNTR_KPI24_Name = "KPI24";

   public static VvLookUpLista luiListaAmortTT;
   public const string luiListaAmortTT_Name = "AmortTT";

   public static VvLookUpLista luiListaPlacaTT;
   public const string luiListaPlacaTT_Name = "PlacaTT";

   public static VvLookUpLista luiListaStrSprema;
   public const string luiListaStrSprema_Name = "StrucnaSprema";

   public static VvLookUpLista luiListaRadnoMjesto;
   public const string luiListaRadnoMjesto_Name = "RadnoMjesto";

   public static VvLookUpLista luiListaPersonTS;
   public const string luiListaPersonTS_Name = "PersonTS";

   public static VvLookUpLista luiListaFondSati_NOR;
   public const string luiListaFondSati_NOR_Name = "FondSatiNOR";

   public static VvLookUpLista luiListaFondSati_TRG;
   public const string luiListaFondSati_TRG_Name = "FondSatiTRG";

   public static VvLookUpLista luiListaPlacaVrstaObr;
   public const string luiListaPlacaVrstaObr_Name = "VrstaObracuna";

   public static VvLookUpLista luiListaPRules;
   public const string luiListaPRules_Name = "PRules";

   public static VvLookUpLista luiListaVrstaRadaEVR;
   public const string luiListaVrstaRadaEVR_Name = "VrstaRadaEVR";

   public static VvLookUpLista luiListaOsnovaOsigRsm;
   public const string luiListaOsnovaOsigRsm_Name = "OsnovaOsiguranja";

   public static VvLookUpLista luiListaStazBRsm;
   public const string luiListaStazBRsm_Name = "StazSPovecanimTrajanjem";

   public static VvLookUpLista luiListaDeviza;
   public const string luiListaDeviza_Name = "DevizneValute";

   public static VvLookUpLista luiListaGrupa1Artikla;
   public const string luiListaGrupa1Artikla_Name = "GrupaArtikla";

   public static VvLookUpLista luiListaGrupa2Artikla;
   public const string luiListaGrupa2Artikla_Name = "GrupaArtikla2";

   public static VvLookUpLista luiListaGrupa3Artikla;
   public const string luiListaGrupa3Artikla_Name = "GrupaArtikla3";

   public static VvLookUpLista luiListaPdvKat;
   public const string luiListaPdvKat_Name = "PdvKategorija";

   public static VvLookUpLista luiListaArtiklTS;
   public const string luiListaArtiklTS_Name = "TipArtikla";

   public static VvLookUpLista luiListaRiskZiroRn;
   public const string luiListaRiskZiroRn_Name = "ZiroRacun";

   public static VvLookUpLista luiListaRiskVrstaPl;
   public const string luiListaRiskVrstaPl_Name = "VrstaPlacanja";

   public static VvLookUpLista luiListaRiskCjenik;
   public const string luiListaRiskCjenik_Name = "Cjenik";

   public static VvLookUpLista luiListaRiskStatus;
   public const string luiListaRiskStatus_Name = "Status";

   public static VvLookUpLista luiListaRiskPredefs;
   public const string luiListaRiskPredefs_Name = "Predefs";

   public static VvLookUpLista luiListaRiskTipOtprem;
   public const string luiListaRiskTipOtprem_Name = "TipOtpreme";

   public static VvLookUpLista dscLuiLst_IRA_1;
   public const string dscLuiLst_IRA_1_Name = "Dsc_IRA_1";

   public static VvLookUpLista dscLuiLst_IRA_2;
   public const string dscLuiLst_IRA_2_Name = "Dsc_IRA_2";

   public static VvLookUpLista dscLuiLst_IRA_3;
   public const string dscLuiLst_IRA_3_Name = "Dsc_IRA_3";

   public static VvLookUpLista dscLuiLst_IRA_4;
   public const string dscLuiLst_IRA_4_Name = "Dsc_IRA_4";

   public static VvLookUpLista dscLuiLst_IRA_5;
   public const string dscLuiLst_IRA_5_Name = "Dsc_IRA_5";

   public static VvLookUpLista dscLuiLst_PON;
   public const string dscLuiLst_PON_Name = "Dsc_PON";

   public static VvLookUpLista dscLuiLst_PON_2;
   public const string dscLuiLst_PON_2_Name = "Dsc_PON_2";

   public static VvLookUpLista dscLuiLst_PON_3;
   public const string dscLuiLst_PON_3_Name = "Dsc_PON_3";

   public static VvLookUpLista dscLuiLst_PON_4;
   public const string dscLuiLst_PON_4_Name = "Dsc_PON_4";

   public static VvLookUpLista dscLuiLst_PNM;
   public const string dscLuiLst_PNM_Name = "Dsc_PNM";

   public static VvLookUpLista dscLuiLst_PNM_2;
   public const string dscLuiLst_PNM_2_Name = "Dsc_PNM_2";

   public static VvLookUpLista dscLuiLst_PNM_3;
   public const string dscLuiLst_PNM_3_Name = "Dsc_PNM_3";

   public static VvLookUpLista dscLuiLst_PNM_4;
   public const string dscLuiLst_PNM_4_Name = "Dsc_PNM_4";

   public static VvLookUpLista dscLuiLst_XYZ;
   public const string dscLuiLst_XYZ_Name = "Dsc_XYZ";

   public static VvLookUpLista dscLuiLst_URA;  // Ulazni Racun + Primka 
   public const string dscLuiLst_URA_Name = "Dsc_URA";

   public static VvLookUpLista dscLuiLst_UFA;
   public const string dscLuiLst_UFA_Name = "Dsc_UFA";

   public static VvLookUpLista dscLuiLst_UFM;
   public const string dscLuiLst_UFM_Name = "Dsc_UFM";

   public static VvLookUpLista dscLuiLst_PRI;  // Primka Veleprodaja
   public const string dscLuiLst_PRI_Name = "Dsc_PRI";

   public static VvLookUpLista dscLuiLst_KLK;  // Primka Maloprodaja
   public const string dscLuiLst_KLK_Name = "Dsc_KLK";

   public static VvLookUpLista dscLuiLst_KKM;  // KOMISIJSKA Primka Maloprodaja
   public const string dscLuiLst_KKM_Name = "Dsc_KKM";

   public static VvLookUpLista dscLuiLst_IFA;  // Izlazni racun 
   public const string dscLuiLst_IFA_Name = "Dsc_IFA";

   public static VvLookUpLista dscLuiLst_IFA_2;  // Izlazni racun 2 
   public const string dscLuiLst_IFA_2_Name = "Dsc_IFA_2";

   public static VvLookUpLista dscLuiLst_IFA_3;  // Izlazni racun 3 
   public const string dscLuiLst_IFA_3_Name = "Dsc_IFA_3";

   public static VvLookUpLista dscLuiLst_IFA_4;  // Izlazni racun 4 
   public const string dscLuiLst_IFA_4_Name = "Dsc_IFA_4";

   public static VvLookUpLista dscLuiLst_IZD;  // Izdatnica
   public const string dscLuiLst_IZD_Name = "Dsc_IZD";

   public static VvLookUpLista dscLuiLst_IRM;  // Izlazni racun Maloprodajni
   public const string dscLuiLst_IRM_Name = "Dsc_IRM";

   public static VvLookUpLista dscLuiLst_IRM_2;  // Izlazni racun Maloprodajni 2
   public const string dscLuiLst_IRM_2_Name = "Dsc_IRM_2";

   public static VvLookUpLista dscLuiLst_IRM_3;  // Izlazni racun Maloprodajni 3
   public const string dscLuiLst_IRM_3_Name = "Dsc_IRM_3";

   public static VvLookUpLista dscLuiLst_IRM_4;  // Izlazni racun Maloprodajni 4
   public const string dscLuiLst_IRM_4_Name = "Dsc_IRM_4";

   public static VvLookUpLista dscLuiLst_UOD;  // Ulazno Odobrenje
   public const string dscLuiLst_UOD_Name = "Dsc_UOD";

   public static VvLookUpLista dscLuiLst_UPV;  // Ulazno Povrat
   public const string dscLuiLst_UPV_Name = "Dsc_UPV";

   public static VvLookUpLista dscLuiLst_UPM;  // Ulazno Povrat malop 
   public const string dscLuiLst_UPM_Name = "Dsc_UPM";

   public static VvLookUpLista dscLuiLst_IOD;  // Izlazno Odobrenje
   public const string dscLuiLst_IOD_Name = "Dsc_IOD";

   public static VvLookUpLista dscLuiLst_IPV;  // Izlazno Povrat
   public const string dscLuiLst_IPV_Name = "Dsc_IPV";

   public static VvLookUpLista dscLuiLst_NRD;  // Narudzba dobavljacu domaca
   public const string dscLuiLst_NRD_Name = "Dsc_NRD";

   public static VvLookUpLista dscLuiLst_NRM;  // Narudzba dobavljacu maloprodajna
   public const string dscLuiLst_NRM_Name = "Dsc_NRM";

   public static VvLookUpLista dscLuiLst_NRU;  // Narudzba dobavljacu uvozna
   public const string dscLuiLst_NRU_Name = "Dsc_NRU";

   public static VvLookUpLista dscLuiLst_NRS;  // Narudzba dobavljacu za usluge
   public const string dscLuiLst_NRS_Name = "Dsc_NRS";

   public static VvLookUpLista dscLuiLst_NRK;  // Narudzba kupca
   public const string dscLuiLst_NRK_Name = "Dsc_NRK";

   public static VvLookUpLista dscLuiLst_STU;  // Storno ulaza
   public const string dscLuiLst_STU_Name = "Dsc_STU";

   public static VvLookUpLista dscLuiLst_STI;  // Storno izlaza
   public const string dscLuiLst_STI_Name = "Dsc_STI";

   public static VvLookUpLista dscLuiLst_RVI;  // Revers
   public const string dscLuiLst_RVI_Name = "Dsc_RVI";

   public static VvLookUpLista dscLuiLst_RVU;  // Revers povrat
   public const string dscLuiLst_RVU_Name = "Dsc_RVU";

   public static VvLookUpLista dscLuiLst_UPL;  // Blagajnicka uplatnica
   public const string dscLuiLst_UPL_Name = "Dsc_UPL";

   public static VvLookUpLista dscLuiLst_ISP;  // Blagajnicka isplatnica
   public const string dscLuiLst_ISP_Name = "Dsc_ISP";

   public static VvLookUpLista dscLuiLst_BUP;  // Blagajnicka uplatnica-Multi + Dev
   public const string dscLuiLst_BUP_Name = "Dsc_BUP";

   public static VvLookUpLista dscLuiLst_BIS;  // Blagajnicka isplatnica-Multi + Dev
   public const string dscLuiLst_BIS_Name = "Dsc_BIS";

   public static VvLookUpLista dscLuiLst_MSI;  // Medjuskladisnice
   public const string dscLuiLst_MSI_Name = "Dsc_MSI";

   //R_PIZ  // Proizvodnja
   //R_PIZ  // Proizvodnja
   public static VvLookUpLista dscLuiLst_CJE;  // Cjenik
   public const string dscLuiLst_CJE_Name = "Dsc_CJE";

   public static VvLookUpLista dscLuiLst_PST;  // Pocetno stanje
   public const string dscLuiLst_PST_Name = "Dsc_PST";

   public static VvLookUpLista dscLuiLst_INV;  // Inventura
   public const string dscLuiLst_INV_Name = "Dsc_INV";

   public static VvLookUpLista dscLuiLst_INM;  // Inventura Maloprodajna
   public const string dscLuiLst_INM_Name = "Dsc_INM";

   public static VvLookUpLista dscLuiLst_SKU;  // Skladisni ulaz Only - skladisna primka
   public const string dscLuiLst_SKU_Name = "Dsc_SKU";

   public static VvLookUpLista dscLuiLst_SKI;  // Skladisni izlaz Only - skladisna izdatnica
   public const string dscLuiLst_SKI_Name = "Dsc_SKU";

   public static VvLookUpLista dscLuiLst_PPR;  // Predatnica u proizvodnju
   public const string dscLuiLst_PPR_Name = "Dsc_PPR";

   public static VvLookUpLista dscLuiLst_PIP;  // Primka iz proizvodnje
   public const string dscLuiLst_PIP_Name = "Dsc_PIP";

   public static VvLookUpLista dscLuiLst_POV;  // Povratnica iz proizvodnje
   public const string dscLuiLst_POV_Name = "Dsc_POV";

   public static VvLookUpLista dscLuiLst_IZM;  // Izdatnica Maloprodajna
   public const string dscLuiLst_IZM_Name = "Dsc_IZM";

   public static VvLookUpLista dscLuiLst_IZM_2;  // Izdatnica Maloprodajna 2
   public const string dscLuiLst_IZM_2_Name = "Dsc_IZM_2";

   //R_TMK  // Korekturna temeljnica ulaza/izlaza
   //R_TMK  // Korekturna temeljnica ulaza/izlaza
   //R_ZPC  // Nivelacija - zapisnik o promjeni cijena
   //R_ZPC  // Nivelacija - zapisnik o promjeni cijena
   //R_IMT  // Izdatnica na mjesto troska
   //R_IMT  // Izdatnica na mjesto troska
   public static VvLookUpLista dscLuiLst_KIZ;  // Komisijska Izdatnica
   public const string dscLuiLst_KIZ_Name = "Dsc_KIZ";

   public static VvLookUpLista dscLuiLst_PIK;  // Povrat iz Komisije
   public const string dscLuiLst_PIK_Name = "Dsc_PIK";


   public static VvLookUpLista luiListaRiskJezikHrv;
   public const string luiListaRiskJezikHrv_Name = "Hrvatski";

   public static VvLookUpLista luiListaRiskJezikEng;
   public const string luiListaRiskJezikEng_Name = "Engleski";

   public static VvLookUpLista luiListaRiskJezikC;
   public const string luiListaRiskJezikC_Name = "JezikC";

   public static VvLookUpLista luiListaRiskJezikD;
   public const string luiListaRiskJezikD_Name = "JezikD";

   public static VvLookUpLista dscLuiLst_KtoShema;       // KtoShema
   public const string dscLuiLst_KtoShema_Name = "Dsc_KtoShema";

   public static VvLookUpLista dscLuiLst_KtoShemaPlaca;          // KtoShemaPlace
   public const string dscLuiLst_KtoShemaPlaca_Name = "Dsc_KtoShemaPlaca";


   public static VvLookUpLista luiListaRiskVodPrjkt;
   public const string luiListaRiskVodPrjkt_Name = "VoditeljProjekta";

   public static VvLookUpLista luiListaMixerType;
   public const string luiListaMixerType_Name = "OtherType";

   public static VvLookUpLista luiListaMixerZadatak;
   public const string luiListaMixerZadatak_Name = "Zadatak";

   public static VvLookUpLista luiListaMixerOdrediste;
   public const string luiListaMixerOdrediste_Name = "Odrediste";

   public static VvLookUpLista luiListaMixerVozilo;
   public const string luiListaMixerVozilo_Name = "Vozilo";

   public static VvLookUpLista luiListaMixerRelacija;
   public const string luiListaMixerRelacija_Name = "Relacija";

   public static VvLookUpLista luiListaMixTypePutNal;
   public const string luiListaMixTypePutNal_Name = "PutniNalog";

   public static VvLookUpLista luiListaMixTypeZahtjev;
   public const string luiListaMixTypeZahtjev_Name = "Zahtjev";

   public static VvLookUpLista luiListaMixTypeEvidencija;
   public const string luiListaMixTypeEvidencija_Name = "Evidencija";


   public static VvLookUpLista luiListaMixDnevnice;
   public const string luiListaMixDnevnice_Name = "Dnevnice";

   public static VvLookUpLista luiListaMixRadVrijemeRVR;
   public const string luiListaMixRadVrijemeRVR_Name = "VrstaRadnogVremenaRVR";

   public static VvLookUpLista luiListaMixRadVrijemRVR2;
   public const string luiListaMixRadVrijemRVR2_Name = "VrstaRadnogVremenaRVR1"; // od 28.03.2015. vrijede nova pravila

   public static VvLookUpLista dscLuiLst_riskRules;
   public const string dscLuiLst_riskRules_Name = "RiskRules";

   public static VvLookUpLista luiListaIspostava;
   public const string luiListaIspostava_Name = "Ispostave";

   public static VvLookUpLista luiListaKplanKlase;
   public const string luiListaKplanKlase_Name = "KplanKlase";

   public static VvLookUpLista luiListaMix_GFI_TSI;
   public const string luiListaMix_GFI_TSI_Name = "GFI_TSI";

   public static VvLookUpLista luiLista_PlanTT;
   public const string luiLista_PlanTT_Name = "PlanTT";

   public static VvLookUpLista luiListaMix_Statist_NPF;
   public const string luiListaMix_Statist_NPF_Name = "Statistika_NPF";

   // --------------------- 
   public static VvLookUpLista luiListaPozicijePlana;
   public const string luiListaPozicijePlana_Name = "PozicijePlana";
   public static VvLookUpLista luiListaPozicijePlanaPLN;
   public const string luiListaPozicijePlanaPLN_Name = "PozicijePlanaPLN";
   public static VvLookUpLista luiListaPozicijePlanaRLZ;
   public const string luiListaPozicijePlanaRLZ_Name = "PozicijePlanaRLZ";
   public static VvLookUpLista luiListaPozicijePlanaPBN;
   public const string luiListaPozicijePlanaPBN_Name = "PozicijePlanaPBN";
   // --------------------- 

   public static VvLookUpLista luiListaTSI_Podaci;
   public const string luiListaTSI_Podaci_Name = "TSI_Podaci";

   public static VvLookUpLista luiListaGFI_Bilanca;
   public const string luiListaGFI_Bilanca_Name = "GFI_Bilanca";

   public static VvLookUpLista luiListaGFI_RDG;
   public const string luiListaGFI_RDG_Name = "GFI_RDG";

   public static VvLookUpLista luiListaGFI_PodDop;
   public const string luiListaGFI_PodDop_Name = "GFI_PodDop";

   public static VvLookUpLista luiListaGFI_NT_I;
   public const string luiListaGFI_NT_I_Name = "GFI_NT_I";

   public static VvLookUpLista luiListaGFI_NT_D;
   public const string luiListaGFI_NT_D_Name = "GFI_NT_D";

   public static VvLookUpLista luiLista_PR_RAS_NPF;
   public const string luiLista_PR_RAS_NPF_Name = "PR_RAS_NPF";

   public static VvLookUpLista luiLista_S_PR_RAS_NPF;
   public const string luiLista_S_PR_RAS_NPF_Name = "S_PR_RAS_NPF";

   public static VvLookUpLista luiLista_BIL_NPF;
   public const string luiLista_BIL_NPF_Name = "BIL_NPF";

   public static VvLookUpLista luiLista_G_PR_IZ_NPF;
   public const string luiLista_G_PR_IZ_NPF_Name = "G_PR_IZ_NPF";

   //public static VvLookUpLista luiListaOPD;
   //public const  string        luiListaOPD_Name          = "OPD";

   public static VvLookUpLista luiListaPostaZupan;
   public const string luiListaPostaZupan_Name = "PostaZupan";

   public static VvLookUpLista luiListaRtranoGr;
   public const string luiListaRtranoGr_Name = "GrupaOpis";

   public static VvLookUpLista luiListaNacIspl;
   public const string luiListaNacIspl_Name = "NacinIsplate"; //Prilog 5 Joppd

   public static VvLookUpLista luiListaNeoporPrim;
   public const string luiListaNeoporPrim_Name = "NeoporeziviPrimici"; //Prilog 4 Joppd

   public static VvLookUpLista luiListaStjecatelj;
   public const string luiListaStjecatelj_Name = "StjecateljPrimitka"; //Prilog 2 Joppd

   public static VvLookUpLista luiListaPrimDoh;
   public const string luiListaPrimDoh_Name = "PrimiciObvezeDoprinosa"; //Prilog 3 Joppd

   public static VvLookUpLista luiListaPocKraj;
   public const string luiListaPocKraj_Name = "PrviZadnjiMjesecUOsiguranju"; //Prilog 3 Joppd

   public static VvLookUpLista luiListaVrstaIzvjJoppd;
   public const string luiListaVrstaIzvjJoppd_Name = "VrstaIzvjesca";

   public static VvLookUpLista luiListaBrojSobe;
   public const string luiListaBrojSobe_Name = "BrojSobe";

   public static VvLookUpLista luiListaMixRadVrijemeMVR;
   public const string luiListaMixRadVrijemeMVR_Name = "VrstaRadnogVremenaMVR";

   public static VvLookUpLista luiListaDrzave;
   public const string luiListaDrzave_Name = "Drzave";

   public static VvLookUpLista luiListaGranPrijelaz;
   public const string luiListaGranPrijelaz_Name = "GranPrijelaz";

   public static VvLookUpLista luiListaStatusGosta;
   public const string luiListaStatusGosta_Name = "StatusGosta";

   public static VvLookUpLista luiListaTipObjekta;
   public const string luiListaTipObjekta_Name = "TipObjekta";

   public static VvLookUpLista luiListaVrstaGosta;
   public const string luiListaVrstaGosta_Name = "VrstaGosta";

   public static VvLookUpLista luiListaVrstaPutIsprave;
   public const string luiListaVrstaPutIsprave_Name = "VrstaPutIsprave";

   public static VvLookUpLista luiListaFinFond;
   public const string luiListaFinFond_Name = "FinFond";

   public static VvLookUpLista luiListaKlasifikALL;
   public const string luiListaKlasifikALL_Name = "KlasifikALL";
   public static VvLookUpLista luiListaKlasifikJOB;
   public const string luiListaKlasifikJOB_Name = "KlasifikJOB";

   public static VvLookUpLista luiListaVrstaUgovora;
   public const string luiListaVrstaUgovora_Name = "VrstaUgovora";

   public static VvLookUpLista luiListaSerlot;
   public const string luiListaSerlot_Name = "RGC_LOT_Serno";

   public static VvLookUpLista luiListaVrstaRNM;
   public const string luiListaVrstaRNM_Name = "VrstaRNM";

   public static VvLookUpLista luiListaOutlookItems;
   public const string luiListaOutlookItems_Name = "MailAdresa";

   public static VvLookUpLista luiListaMixTypeZastitari;
   public const string luiListaMixTypeZastitari_Name = "Zastitari";

   public static VvLookUpLista luiListaeRacPoslProc;
   public const string luiListaeRacPoslProc_Name = "eRacPoslProc";

   public static VvLookUpLista luiListaFakRptUniFilter;
   public const string         luiListaFakRptUniFilter_Name = "FakRptUniFilter";

   public static VvLookUpLista luiListaPTG_NajamNaRok;
   public const string         luiListaPTG_NajamNaRok_Name = "NajamNaRok";

   public static VvLookUpLista luiListaPTG_VrstaNajma;
   public const string         luiListaPTG_VrstaNajma_Name = "VrstaNajma";

   public static VvLookUpLista luiListaPTG_DanZaFaktur;
   public const string         luiListaPTG_DanZaFaktur_Name = "DanZaFakturiranje";

   public static VvLookUpLista luiListaPTG_SlanjeRacuna;
   public const string         luiListaPTG_SlanjeRacuna_Name = "SlanjeRacuna";

   public static VvLookUpLista luiListaPTG_OsigPlacanja;
   public const string         luiListaPTG_OsigPlacanja_Name = "OsiguranjePlacanja";

   public static VvLookUpLista luiListaGeonomenklatura;
   public const string         luiListaGeonomenklatura_Name = "Geonomenklatura";

   public static VvLookUpLista luiListaIncoterms;
   public const string         luiListaIncoterms_Name = "Incoterms";

   public static VvLookUpLista luiListaIntrastVrPosla;
   public const string         luiListaIntrastVrPosla_Name = "IntrastVrPosla";

   public static VvLookUpLista luiListaIntrastVrProm;
   public const string         luiListaIntrastVrProm_Name = "IntrastVrProm";

   public static VvLookUpLista luiListaIntrastIsporuka;
   public const string         luiListaIntrastIsporuka_Name = "IntrastIsporuka";

   public static VvLookUpLista luiListaPCKpricesPerGB;
   public const string         luiListaPCKpricesPerGB_Name = "PCKpricesPerGB";

   public static VvLookUpLista luiListaZIZ_TT;
   public const string         luiListaZIZ_TT_Name = "ZIZ_TT";

   public static VvLookUpLista luiListaKPD2025;
   public const string         luiListaKPD2025_Name = "KPD2025";

   public static VvLookUpLista luiListaKodTipaEracuna;
   public const string         luiListaKodTipaEracuna_Name = "KodTipaEracuna";

   #endregion VvLookUps

   #region Global VvDao's

   //public static VvDataRecord[] PUG_RecordList = { new Kplan(), new Nalog(), new Ftrans(), new Kupdob() };
   public static string[] VvPUG_RecordNamesList =
      {
         Kplan .recordName,
         Kplan .recordNameArhiva,
         Nalog .recordName,
         Nalog .recordNameArhiva,
         Ftrans.recordName,
         Ftrans.recordNameArhiva,
         Kupdob.recordName,
         Kupdob.recordNameArhiva,
         Osred .recordName,
         Osred .recordNameArhiva,
         Amort .recordName,
         Amort .recordNameArhiva,
         Atrans.recordName,
         Atrans.recordNameArhiva,
         Person.recordName,
         Person.recordNameArhiva,
         Placa .recordName,
         Placa .recordNameArhiva,
         Ptrans.recordName,
         Ptrans.recordNameArhiva,
         Ptrane.recordName,
         Ptrane.recordNameArhiva,
         Ptrano.recordName,
         Ptrano.recordNameArhiva,

         ArtStat.recordName,

         Artikl.recordName,
         Artikl.recordNameArhiva,
         Faktur.recordName,
         Faktur.recordNameArhiva,
         FaktEx.recordName,
         FaktEx.recordNameArhiva,
         Rtrans.recordName,
         Rtrans.recordNameArhiva,
         Rtrano.recordName,
         Rtrano.recordNameArhiva,

         Mixer .recordName,
         Mixer .recordNameArhiva,
         Xtrans.recordName,
         Xtrans.recordNameArhiva,
         Xtrano.recordName,
         Xtrano.recordNameArhiva

      };

   public static KplanDao KplanDao { get; set; }
   public static DataRowCollection KplanSchemaRows { get { return ZXC.KplanDao.TheSchemaTable.Rows; } }
   public static KplanDao.KplanCI KplCI { get { return ZXC.KplanDao.CI; } }

   public static PrjktDao PrjktDao { get; set; }
   public static DataRowCollection PrjktSchemaRows { get { return ZXC.PrjktDao.TheSchemaTable.Rows; } }
   public static PrjktDao.PrjktCI PrjCI { get { return ZXC.PrjktDao.CI; } }

   public static UserDao UserDao { get; set; }
   public static DataRowCollection UserSchemaRows { get { return ZXC.UserDao.TheSchemaTable.Rows; } }
   public static UserDao.UserCI UsrCI { get { return ZXC.UserDao.CI; } }

   public static PrvlgDao PrvlgDao { get; set; }
   public static DataRowCollection PrvlgSchemaRows { get { return ZXC.PrvlgDao.TheSchemaTable.Rows; } }
   public static PrvlgDao.PrvlgCI PrvCI { get { return ZXC.PrvlgDao.CI; } }

   public static SkyRuleDao SkyRuleDao { get; set; }
   public static DataRowCollection SkyRuleSchemaRows { get { return ZXC.SkyRuleDao.TheSchemaTable.Rows; } }
   public static SkyRuleDao.SkyRuleCI SkyCI { get { return ZXC.SkyRuleDao.CI; } }

   public static DevTecDao DevTecDao { get; set; }
   public static DataRowCollection DevTecSchemaRows { get { return ZXC.DevTecDao.TheSchemaTable.Rows; } }
   public static DevTecDao.DevTecCI DTecCI { get { return ZXC.DevTecDao.CI; } }

   public static HtransDao HtransDao { get; set; }
   public static DataRowCollection HtransSchemaRows { get { return ZXC.HtransDao.TheSchemaTable.Rows; } }
   public static HtransDao.HtransCI HtrCI { get { return ZXC.HtransDao.CI; } }

   public static NalogDao NalogDao { get; set; }
   public static DataRowCollection NalogSchemaRows { get { return ZXC.NalogDao.TheSchemaTable.Rows; } }
   public static NalogDao.NalogCI NalCI { get { return ZXC.NalogDao.CI; } }

   public static FtransDao FtransDao { get; set; }
   public static DataRowCollection FtransSchemaRows { get { return ZXC.FtransDao.TheSchemaTable.Rows; } }
   public static FtransDao.FtransCI FtrCI { get { return ZXC.FtransDao.CI; } }

   public static KupdobDao KupdobDao { get; set; }
   public static DataRowCollection KupdobSchemaRows { get { return ZXC.KupdobDao.TheSchemaTable.Rows; } }
   public static KupdobDao.KupdobCI KpdbCI { get { return ZXC.KupdobDao.CI; } }

   public static OsredDao OsredDao { get; set; }
   public static DataRowCollection OsredSchemaRows { get { return ZXC.OsredDao.TheSchemaTable.Rows; } }
   public static OsredDao.OsredCI OsrCI { get { return ZXC.OsredDao.CI; } }

   public static AmortDao AmortDao { get; set; }
   public static DataRowCollection AmortSchemaRows { get { return ZXC.AmortDao.TheSchemaTable.Rows; } }
   public static AmortDao.AmortCI AmoCI { get { return ZXC.AmortDao.CI; } }

   public static AtransDao AtransDao { get; set; }
   public static DataRowCollection AtransSchemaRows { get { return ZXC.AtransDao.TheSchemaTable.Rows; } }
   public static AtransDao.AtransCI AtrCI { get { return ZXC.AtransDao.CI; } }

   public static PersonDao PersonDao { get; set; }
   public static DataRowCollection PersonSchemaRows { get { return ZXC.PersonDao.TheSchemaTable.Rows; } }
   public static PersonDao.PersonCI PerCI { get { return ZXC.PersonDao.CI; } }

   public static PlacaDao PlacaDao { get; set; }
   public static DataRowCollection PlacaSchemaRows { get { return ZXC.PlacaDao.TheSchemaTable.Rows; } }
   public static PlacaDao.PlacaCI PlaCI { get { return ZXC.PlacaDao.CI; } }

   public static PtransDao PtransDao { get; set; }
   public static DataRowCollection PtransSchemaRows { get { return ZXC.PtransDao.TheSchemaTable.Rows; } }
   public static PtransDao.PtransCI PtrCI { get { return ZXC.PtransDao.CI; } }
   public static PtraneDao PtraneDao { get; set; }
   public static DataRowCollection PtraneSchemaRows { get { return ZXC.PtraneDao.TheSchemaTable.Rows; } }
   public static PtraneDao.PtraneCI PteCI { get { return ZXC.PtraneDao.CI; } }
   public static PtranoDao PtranoDao { get; set; }
   public static DataRowCollection PtranoSchemaRows { get { return ZXC.PtranoDao.TheSchemaTable.Rows; } }
   public static PtranoDao.PtranoCI PtoCI { get { return ZXC.PtranoDao.CI; } }


   public static ArtiklDao ArtiklDao { get; set; }
   public static DataRowCollection ArtiklSchemaRows { get { return ZXC.ArtiklDao.TheSchemaTable.Rows; } }
   public static ArtiklDao.ArtiklCI ArtCI { get { return ZXC.ArtiklDao.CI; } }

   public static FakturDao FakturDao { get; set; }
   public static DataRowCollection FakturSchemaRows { get { return ZXC.FakturDao.TheSchemaTable.Rows; } }
   public static FakturDao.FakturCI FakCI { get { return ZXC.FakturDao.CI; } }

   public static FaktExDao FaktExDao { get; set; }
   public static DataRowCollection FaktExSchemaRows { get { return ZXC.FaktExDao.TheSchemaTable.Rows; } }
   public static FaktExDao.FaktExCI FexCI { get { return ZXC.FaktExDao.CI; } }

   public static RtransDao RtransDao { get; set; }
   public static DataRowCollection RtransSchemaRows { get { return ZXC.RtransDao.TheSchemaTable.Rows; } }
   public static RtransDao.RtransCI RtrCI { get { return ZXC.RtransDao.CI; } }

   public static RtranoDao RtranoDao { get; set; }
   public static DataRowCollection RtranoSchemaRows { get { return ZXC.RtranoDao.TheSchemaTable.Rows; } }
   public static RtranoDao.RtranoCI RtoCI { get { return ZXC.RtranoDao.CI; } }

   public static ArtStatDao ArtStatDao { get; set; }
   public static DataRowCollection ArtStatSchemaRows { get { return ZXC.ArtStatDao.TheSchemaTable.Rows; } }
   public static ArtStatDao.ArtStatCI AstCI { get { return ZXC.ArtStatDao.CI; } }

   public static MixerDao MixerDao { get; set; }
   public static DataRowCollection MixerSchemaRows { get { return ZXC.MixerDao.TheSchemaTable.Rows; } }
   public static MixerDao.MixerCI MixCI { get { return ZXC.MixerDao.CI; } }

   public static XtransDao XtransDao { get; set; }
   public static DataRowCollection XtransSchemaRows { get { return ZXC.XtransDao.TheSchemaTable.Rows; } }
   public static XtransDao.XtransCI XtrCI { get { return ZXC.XtransDao.CI; } }

   public static XtranoDao XtranoDao { get; set; }
   public static DataRowCollection XtranoSchemaRows { get { return ZXC.XtranoDao.TheSchemaTable.Rows; } }
   public static XtranoDao.XtranoCI XtoCI { get { return ZXC.XtranoDao.CI; } }



   //=============================================================================================================== 
   //=============================================================================================================== 
   //=============================================================================================================== 

   public static Type GetVvDataRecordType(string recordName)
   {
      Type vvDataRecordType = null;

      if(recordName.StartsWith(ZXC.vvDB_SKYlogTableName)) vvDataRecordType = typeof(VvSQL.VvSkyLogEntry);
      else if(recordName.StartsWith(ZXC.vvDB_ERRlogTableName)) vvDataRecordType = typeof(VvSQL.VvSkyLogEntry);
      else if(recordName.StartsWith(ZXC.vvDB_LANlogTableName)) vvDataRecordType = typeof(VvSQL.VvLanLogEntry);
      else switch(recordName)
         {
            case Kplan.recordName:
            case Kplan.recordNameArhiva: vvDataRecordType = typeof(Kplan); break;

            case Nalog.recordName:
            case Nalog.recordNameArhiva: vvDataRecordType = typeof(Nalog); break;

            case Ftrans.recordName:
            case Ftrans.recordNameArhiva: vvDataRecordType = typeof(Ftrans); break;

            case Kupdob.recordName:
            case Kupdob.recordNameArhiva: vvDataRecordType = typeof(Kupdob); break;

            case Osred.recordName:
            case Osred.recordNameArhiva: vvDataRecordType = typeof(Osred); break;

            case Amort.recordName:
            case Amort.recordNameArhiva: vvDataRecordType = typeof(Amort); break;

            case Atrans.recordName:
            case Atrans.recordNameArhiva: vvDataRecordType = typeof(Atrans); break;

            case Person.recordName:
            case Person.recordNameArhiva: vvDataRecordType = typeof(Person); break;

            case Placa.recordName:
            case Placa.recordNameArhiva: vvDataRecordType = typeof(Placa); break;

            case Ptrans.recordName:
            case Ptrans.recordNameArhiva: vvDataRecordType = typeof(Ptrans); break;

            case Ptrane.recordName:
            case Ptrane.recordNameArhiva: vvDataRecordType = typeof(Ptrane); break;

            case Ptrano.recordName:
            case Ptrano.recordNameArhiva: vvDataRecordType = typeof(Ptrano); break;

            // ========================================================================================== 

            case Prjkt.recordName:
            case Prjkt.recordNameArhiva: vvDataRecordType = typeof(Kupdob); break;

            case User.recordName:
            case User.recordNameArhiva: vvDataRecordType = typeof(User); break;

            case Prvlg.recordName:
            case Prvlg.recordNameArhiva: vvDataRecordType = typeof(Prvlg); break;

            case SkyRule.recordName:
            case SkyRule.recordNameArhiva: vvDataRecordType = typeof(SkyRule); break;

            case DevTec2.recordName:
            case DevTec2.recordNameArhiva: vvDataRecordType = typeof(DevTec2); break;

            case Htrans2.recordName:
            case Htrans2.recordNameArhiva: vvDataRecordType = typeof(Htrans2); break;

            case ArtStat.recordName: vvDataRecordType = typeof(ArtStat); break;

            case Artikl.recordName:
            case Artikl.recordNameArhiva: vvDataRecordType = typeof(Artikl); break;

            case Faktur.recordName:
            case Faktur.recordNameArhiva: vvDataRecordType = typeof(Faktur); break;

            case FaktEx.recordName:
            case FaktEx.recordNameArhiva: vvDataRecordType = typeof(FaktEx); break;

            case Rtrans.recordName:
            case Rtrans.recordNameArhiva: vvDataRecordType = typeof(Rtrans); break;

            case Rtrano.recordName:
            case Rtrano.recordNameArhiva: vvDataRecordType = typeof(Rtrano); break;

            case Mixer.recordName:
            case Mixer.recordNameArhiva: vvDataRecordType = typeof(Mixer); break;

            case Xtrans.recordName:
            case Xtrans.recordNameArhiva: vvDataRecordType = typeof(Xtrans); break;

            case Xtrano.recordName:
            case Xtrano.recordNameArhiva: vvDataRecordType = typeof(Xtrano); break;


            //case ZXC.vvDB_procTableName     : vvDataRecordType = typeof(VvSQL.Process_Ex     ); break;
            //case ZXC.vvDB_logTableName      : vvDataRecordType = typeof(VvSQL.VvLogEntry     ); break;
            //case ZXC.vvDB_SKYlogTableName   : vvDataRecordType = typeof(VvSQL.VvSkyLogEntry  ); break;
            case ZXC.vvDB_lockerTableName: vvDataRecordType = typeof(VvSQL.VvLockerInfo); break;
            //case ZXC.vvDB_SKYlockerTableName: vvDataRecordType = typeof(VvSQL.VvSKYlockerInfo); break; FUSE 
            case ZXC.vvDB_riskMacroTableName: vvDataRecordType = typeof(VvRiskMacro); break;
            case ZXC.vvDB_ucListTableName: vvDataRecordType = typeof(VvSQL.VvUcListMember); break;

            default:
               if(recordName.StartsWith(ZXC.vvDB_luiPrefix))
               {
                  vvDataRecordType = typeof(VvLookUpItem); break;
               }
               else
               {
                  ZXC.aim_emsg("recordName: [" + recordName + "] nedefiniran u ZXC.GetVvDataRecordType"); break;
                  //return null;
               }
         } // switch(recordName) 

      return vvDataRecordType;
   }

   public static VvDataRecord GetNewVvDataRecordObject(string recordName, uint recID, uint lanSrvID, uint lanRecID)
   {
      VvDataRecord vvDataRecord = null;

      switch(recordName)
      {
         case Kplan.recordName:
         case Kplan.recordNameArhiva: vvDataRecord = new Kplan(recID); break;

         case Nalog.recordName:
         case Nalog.recordNameArhiva: vvDataRecord = new Nalog(recID); break;

         case Ftrans.recordName:
         case Ftrans.recordNameArhiva: vvDataRecord = new Ftrans(recID); break;

         case Kupdob.recordName:
         case Kupdob.recordNameArhiva: vvDataRecord = new Kupdob(recID); break;

         case Osred.recordName:
         case Osred.recordNameArhiva: vvDataRecord = new Osred(recID); break;

         case Amort.recordName:
         case Amort.recordNameArhiva: vvDataRecord = new Amort(recID); break;

         case Atrans.recordName:
         case Atrans.recordNameArhiva: vvDataRecord = new Atrans(recID); break;

         case Person.recordName:
         case Person.recordNameArhiva: vvDataRecord = new Person(recID); break;

         case Placa.recordName:
         case Placa.recordNameArhiva: vvDataRecord = new Placa(recID); break;

         case Ptrans.recordName:
         case Ptrans.recordNameArhiva: vvDataRecord = new Ptrans(recID); break;

         case Ptrane.recordName:
         case Ptrane.recordNameArhiva: vvDataRecord = new Ptrane(recID); break;

         case Ptrano.recordName:
         case Ptrano.recordNameArhiva: vvDataRecord = new Ptrano(recID); break;

         // ========================================================================================== 

         case Prjkt.recordName:
         case Prjkt.recordNameArhiva: vvDataRecord = new Kupdob(recID); break;

         case User.recordName:
         case User.recordNameArhiva: vvDataRecord = new User(recID); break;

         case Prvlg.recordName:
         case Prvlg.recordNameArhiva: vvDataRecord = new Prvlg(recID); break;

         case SkyRule.recordName:
         case SkyRule.recordNameArhiva: vvDataRecord = new SkyRule(recID); break;

         case DevTec2.recordName:
         case DevTec2.recordNameArhiva: vvDataRecord = new DevTec2(recID); break;

         case Htrans2.recordName:
         case Htrans2.recordNameArhiva: vvDataRecord = new Htrans2(recID); break;

         case ArtStat.recordName: vvDataRecord = new ArtStat(recID); break;

         case Artikl.recordName:
         case Artikl.recordNameArhiva: vvDataRecord = new Artikl(recID); break;

         case Faktur.recordName:
         case Faktur.recordNameArhiva: vvDataRecord = new Faktur(recID); break;

         case FaktEx.recordName:
         case FaktEx.recordNameArhiva: vvDataRecord = new FaktEx(recID); break;

         case Rtrans.recordName:
         case Rtrans.recordNameArhiva: vvDataRecord = new Rtrans(recID); break;

         case Rtrano.recordName:
         case Rtrano.recordNameArhiva: vvDataRecord = new Rtrano(recID); break;

         case Mixer.recordName:
         case Mixer.recordNameArhiva: vvDataRecord = new Mixer(recID); break;

         case Xtrans.recordName:
         case Xtrans.recordNameArhiva: vvDataRecord = new Xtrans(recID); break;

         case Xtrano.recordName:
         case Xtrano.recordNameArhiva: vvDataRecord = new Xtrano(recID); break;


         //case ZXC.vvDB_procTableName     : vvDataRecordType = new VvSQL.Process_Ex    (); break;
         //case ZXC.vvDB_logTableName      : vvDataRecord = new VvSQL.VvLogEntry    (); break;
         //case ZXC.vvDB_SKYlogTableName   : vvDataRecord = new VvSQL.VvSkyLogEntry (); break;
         //case ZXC.vvDB_lockerTableName   : vvDataRecord = new VvSQL.VvLockerInfo  (); break;
         //case ZXC.vvDB_riskMacroTableName: vvDataRecord = new       VvRiskMacro   (); break;
         //case ZXC.vvDB_ucListTableName   : vvDataRecord = new VvSQL.VvUcListMember(); break;

         default:
            //if(recordName.StartsWith(ZXC.vvDB_luiPrefix))
            //{
            //   vvDataRecord = new VvLookUpItem(); break;
            //}
            //else
            //{
            ZXC.aim_emsg("recordName: [" + recordName + "] nedefiniran u ZXC.GetNewVvDataRecordObject"); break;
            //return null;
            //}
      } // switch(recordName) 

      vvDataRecord.VirtualLanSrvID = lanSrvID;
      vvDataRecord.VirtualLanRecID = lanRecID;

      return vvDataRecord;
   }

   #endregion Global VvDao's

   //#endif
#if NewVektor

   #region VvLookUps - UNIVERSAL

   public static VvLookUpLista luiListaOpcina;
   public const  string        luiListaOpcina_Name        = "Opcine";

   public static VvLookUpLista luiListaZupanija;
   public const  string        luiListaZupanija_Name      = "Zupanije";

   public static VvLookUpLista luiListaDjelat;
   public const  string        luiListaDjelat_Name        = "Djelatnosti";

   public static VvLookUpLista luiListaGrupaPartnera;
   public const  string        luiListaGrupaPartnera_Name = "GrupaPartnera";

   public static VvLookUpLista luiListaDeviza;
   public const string         luiListaDeviza_Name        = "DevizneValute";

   public static VvLookUpLista luiListaPrvlgScope;
   public const  string        luiListaPrvlgScope_Name    = "PrvlgScope";

   public static VvLookUpLista luiListaPrvlgType;
   public const  string        luiListaPrvlgType_Name     = "PrvlgType";

   public static VvLookUpLista luiListaIspostava;
   public const  string        luiListaIspostava_Name     = "Ispostave";

   public static VvLookUpLista luiListaPostaZupan;
   public const  string        luiListaPostaZupan_Name     = "PostaZupan";

   #endregion VvLookUps - UNIVERSAL
#endif

   #region Global VvDataRecords

   public static VvDataRecord TheGlobalVvDataRecord = null;

   public static Nalog NalogRec;
   public static Amort AmortRec;
   public static DevTec2 DevTecRec;
   public static Faktur FakturRec;
   public static Mixer MixerRec;

   public static VvSQL.VvLanLogEntry lastLAN_LogEntry = new VvSQL.VvLanLogEntry();

   #endregion Global VvDataRecords

   #region Vv QUN

   /// <summary>
   /// Jedinica Mjere za location i size of controls (obicno je: this.Font.Height+4;)
   /// </summary>
   private static int qun;
   public static int QUN
   {
      get { return (qun); }
      set { qun = value; }
   }
   /// <summary>
   /// QUN / 2 (polovina QUN-e)
   /// </summary>
   public static int Qun2 { get { return (ZXC.QUN / 2); } }
   /// <summary>
   /// QUN / 5 (petina QUN-e)
   /// </summary>
   public static int Qun5 { get { return (ZXC.QUN / 5); } }
   /// <summary>
   /// QUN / 4 (cetvrtina QUN-e)
   /// </summary>
   public static int Qun4 { get { return (ZXC.QUN / 4); } }
   /// <summary>
   /// QUN / 8 (osmina QUN-e)
   /// </summary>
   public static int Qun8 { get { return (ZXC.QUN / 8); } }
   /// <summary>
   /// QUN / 10 (desetina QUN-e)
   /// </summary>
   public static int Qun10 { get { return (ZXC.QUN / 10); } }
   /// <summary>
   /// QUN / 12 (dvanaestina QUN-e)
   /// </summary>
   public static int Qun12 { get { return (ZXC.QUN / 12); } }
   /// <summary>
   /// QUN * 2 (2 QUN-e)
   /// </summary>
   public static int Q2un { get { return (ZXC.QUN * 2); } }
   /// <summary>
   /// QUN * 3 (3 QUN-e)
   /// </summary>
   public static int Q3un { get { return (ZXC.QUN * 3); } }
   /// <summary>
   /// QUN * 4 (4 QUN-e)
   /// </summary>
   public static int Q4un { get { return (ZXC.QUN * 4); } }
   /// <summary>
   /// QUN * 5 (5 QUN-e)
   /// </summary>
   public static int Q5un { get { return (ZXC.QUN * 5); } }
   /// <summary>
   /// QUN * 6 (6 QUN-e)
   /// </summary>
   public static int Q6un { get { return (ZXC.QUN * 6); } }
   /// <summary>
   /// QUN * 7 (7 QUN-e)
   /// </summary>
   public static int Q7un { get { return (ZXC.QUN * 7); } }
   /// <summary>
   /// QUN * 8 (8 QUN-e)
   /// </summary>
   public static int Q8un { get { return (ZXC.QUN * 8); } }
   /// <summary>
   /// QUN * 9 (9 QUN-e)
   /// </summary>
   public static int Q9un { get { return (ZXC.QUN * 9); } }
   /// <summary>
   /// QUN * 10 (10 QUN-e)
   /// </summary>
   public static int Q10un { get { return (ZXC.QUN * 10); } }
   /// <summary>
   /// Sirina za Button (4 * ZXC.QUN)
   /// </summary>
   public static int QunBtnW { get { return (4 * ZXC.QUN); } }
   /// <summary>
   /// Visina za Button (11 * ZXC.Qun8)
   /// </summary>
   public static int QunBtnH { get { return (11 * ZXC.Qun8); } }
   /// <summary>
   /// Margina od ruba panela (7 * ZXC.Qun8)
   /// </summary>
   public static int QunMrgn { get { return (7 * ZXC.Qun8); } }
   /// <summary>
   /// Minimalna sirina OkCancel dialoga (ZXC.QunMrgn + ZXC.QunBtnW + ZXC.QunMrgn/2 + ZXC.QunBtnW + ZXC.QunMrgn)
   /// </summary>
   public static int MinOkCancelDlgWidth { get { return (ZXC.QunMrgn + ZXC.QunBtnW + ZXC.QunMrgn / 2 + ZXC.QunBtnW + ZXC.QunMrgn); } }
   /// <summary>
   /// Minimalna sirina OkResetCancel dialoga (ZXC.QunMrgn + ZXC.QunBtnW + ZXC.QunMrgn/2 + ZXC.QunBtnW + ZXC.QunMrgn/2 + ZXC.QunBtnW + ZXC.QunMrgn)
   /// </summary>
   public static int MinOkResetCancelDlgWidth { get { return (ZXC.QunMrgn + ZXC.QunBtnW + ZXC.QunMrgn / 2 + ZXC.QunBtnW + ZXC.QunMrgn / 2 + ZXC.QunBtnW + ZXC.QunMrgn); } }

   #endregion Vv QUN

   #region ZXC for Hamper

   public enum Redak
   {
      prvi, drugi, treći, četvrti, peti,
      šesti, sedmi, osmi, deveti, deseti,
      jedanajsti, dvanajsti, trinajsti, četrnajsti, petnajsti,
      šesnajsti, sedamnajsti, osamnajsti, devetnajsti, dvadeseti };

   public enum Kolona
   {
      prva, druga, treća, četvrta, peta,
      šesta, sedma, osma, deveta, deseta,
      jedanajsta, dvanajsta, trinajsta, četrnajsta, petnajsta,
      šesnajsta, sedamnajsta, osamnajsta, devetnajsta, dvadeseta,
      nijedna };

   public static Pen penBlack, penRed, penGreen;
   public static void DrawRectangleNonWeird(Graphics grfx, Pen pen, int upLeftX, int upLeftY, int width, int height)
   {
      grfx.DrawRectangle(pen, upLeftX, upLeftY, width - 1, height - 1);
   }
   /// <summary>
   /// public static string VvDateFormat = "dd.MM.yyyy";
   /// </summary>
   public static string VvDateFormat = "dd.MM.yyyy.";
   public static string VvDateDdMmYyFormat = "ddMMyy";
   public static string VvDateDdMmFormat = "dd.MM.";
   public static string VvDateDdMmYyyyFormat = "ddMMyyyy";
   public static string VvDateYyyyMmDdFormat = "yyyyMMdd";
   public static string VvDateYyyyMmDdMySQLFormat = "yyyy-MM-dd";
   public static string VvDateYyyyMmDdMySQL_TS_Format = "yyyy-MM-dd HH:mm:ss";
   public static string VvDateYyyyFormat = "yyyy";
   public static string VvDateYYFormat = "yy";
   public static string VvDateMmYyyyFormat = "MMyyyy";
   public static string VvDateYyyyMmFormat = "yyyy-MM";
   public static string VvEmptyDtpFormat = " ";
   // notaBene; kada ces pokusavati uljepsati format datuma pazi!: DateTime.ToString(someFormat) i DTP.CustomFormat(someFormat) NEMAJU ISTU SINTAKSU!!! 
   public static string VvDateAndTimeFormat = "dd.MM.yyyy  HH:mm:ss";
   public static string VvDateAndTimeFormat_NoSec = "dd.MM.yyyy HH:mm";
   public static string VvTimeStampFormat = "ddMMyy_HHmmss";
   public static string VvTimeStampFormat4FileName = "yyyyMMdd_HHmmss";
   public static string VvTimeOnlyFormat = "HH:mm";
   public static string VvTimeOnlyFormat2 = "HH:mm:ss";
   public static string VvDokDate4FiskQRcode = "yyyyMMdd_HHmm";

   public static System.Globalization.CultureInfo VvCultureInfo0;
   public static System.Globalization.CultureInfo VvCultureInfo1;
   public static System.Globalization.CultureInfo VvCultureInfo2;
   public static System.Globalization.CultureInfo VvCultureInfo2_ForceDot;
   public static System.Globalization.CultureInfo VvCultureInfo3;
   public static System.Globalization.CultureInfo VvCultureInfo4;
   public static System.Globalization.CultureInfo VvCultureInfo5;
   public static System.Globalization.CultureInfo VvCultureInfo6;
   public static System.Globalization.CultureInfo VvCultureInfo7;
   public static System.Globalization.CultureInfo VvCultureInfo8;
   public static System.Globalization.NumberFormatInfo VvNumberFormatInfo0;
   public static System.Globalization.NumberFormatInfo VvNumberFormatInfo1;
   public static System.Globalization.NumberFormatInfo VvNumberFormatInfo2;
   public static System.Globalization.NumberFormatInfo VvNumberFormatInfo2_ForceDot;
   public static System.Globalization.NumberFormatInfo VvNumberFormatInfo3;
   public static System.Globalization.NumberFormatInfo VvNumberFormatInfo4;
   public static System.Globalization.NumberFormatInfo VvNumberFormatInfo5;
   public static System.Globalization.NumberFormatInfo VvNumberFormatInfo6;
   public static System.Globalization.NumberFormatInfo VvNumberFormatInfo7;
   public static System.Globalization.NumberFormatInfo VvNumberFormatInfo8;

   #endregion ZXC for Hamper

   #region Global Types (some structs)

   public static Dictionary<uint, decimal> RecIDinfoDict;

   // 16.06.2015 - vvDelf - START

   public static List<Rtrans> FailedIzlazTwinsList;                // by Delf
   public static Rtrans ManagedFailedIzlazTwin;              // by Delf  -- currently being processed!
   public static uint InAddTwinIzlazParentId;              // by Delf  -- currently being ADDRECed!
   // public static List<Rtrans> FailedTwiceIzlazTwinsList;           // by Delf - not needed

   public static string FailedIzlazTwinsListArtiklCd;        // by Delf
   public static bool UseFailedIzlazTwinsListFlag;         // by Delf

   // 16.06.2015 - vvDelf - END

   public struct VvDataBaseInfo
   {
      //public string fullPath;
      private string[] splitters;
      public string prjktNaziv;

      public VvDataBaseInfo(string _year, string _ticker, uint _ID) : this(/*ZXC.vvDB_path + @"\" +*/ VvDB_NameConstructor(_year, _ticker, _ID))
      {
      }

      //public VvDataBaseInfo(string _fullPath)
      //{
      //   this.fullPath = _fullPath;
      //   this.dInfo = new System.IO.DirectoryInfo(fullPath);
      //   splitters = dInfo.Name.Split("_".ToCharArray());
      //   if(splitters.Length > 3) ZXC.aim_emsg("UPOZORENJE!: Database rptLabel ime vise od 2 znaka '_'. Vjerojatno u tickeru postoji nedozvoljeni znak '_' !!!");

      //   projectYearFirstDay = new DateTime(int.Parse(splitters[0].Remove(0, 2)),  1,  1);
      //   projectYearLastDay  = new DateTime(int.Parse(splitters[0].Remove(0, 2)), 12, 31);
      //}

      public VvDataBaseInfo(string _dbName)
      {
         dataBaseName = _dbName;

         if(ZXC.vvDB_is_www) _dbName = _dbName.Replace(ZXC.vvDB_www_preffix, "");
         //splitters = _dbName.Split("_".ToCharArray());
         splitters = _dbName.Split("_".ToCharArray());
         prjktNaziv = "";

         if(splitters.Length > 3) ZXC.aim_emsg("UPOZORENJE!: Database rptLabel ime vise od 2 znaka '_'. Vjerojatno u tickeru postoji nedozvoljeni znak '_' !!!");

         projectYearFirstDay = new DateTime(int.Parse(splitters[0].Remove(0, 2)), 1, 1);
         projectYearLastDay = new DateTime(int.Parse(splitters[0].Remove(0, 2)), 12, 31);

      }

      private string dataBaseName;
      public string DataBaseName
      {
         get { return ZXC.vvDB_www_preffix + dataBaseName; }
      }
      //public string DataBaseName { get; set; }
      //public string DataBaseName
      //{
      //   get { return dInfo.Name; }
      //}

      /// <summary>
      /// U stvari ovo je TICKER
      /// </summary>
      public string ProjectName
      {
         get
         {
            return splitters[1].ToUpper();
         }
      }

      public string ProjectCode
      {
         get
         {
            return splitters[2];
         }
      }

      public uint ProjectKcdAsUInt
      {
         get { return uint.Parse(ProjectCode); }
      }

      public string ProjectYear
      {
         get { return splitters[0].Remove(0, 2); }
      }

      public uint ProjectYearAsUInt
      {
         get { return ZXC.ValOrZero_UInt(this.ProjectYear); }
      }
      public string ProjectYear_LastDigit
      {
         get { return ProjectYear.Remove(0, 3); }
      }

      public string ProjectPreviousYear
      {
         get
         {
            int thisYear = int.Parse(splitters[0].Remove(0, 2));

            return ((int)(thisYear - 1)).ToString();
         }
      }

      public string ProjectPreviousYear_LastDigit
      {
         get { return ProjectPreviousYear.Remove(0, 3); }
      }

      public string Text4ComboBox
      {
         //get { return (ProjectName + " (" + ProjectCode + ") u godini " + ProjectYear); }
         //get { return (ProjectName + " (" + ProjectCode + ")"); }
         get { return (ProjectName + " (" + ProjectCode + ")" + (prjktNaziv.IsEmpty() ? "" : " " + prjktNaziv)); }

      }

      public override string ToString()
      {
         return Text4ComboBox;
      }

      private DateTime projectYearFirstDay;
      public DateTime ProjectYearFirstDay
      {
         get { return projectYearFirstDay; }
      }

      private DateTime projectYearLastDay;
      public DateTime ProjectYearLastDay
      {
         get { return projectYearLastDay; }
      }
   } // public struct VvDataBaseInfo 

   public struct Koordinata_3D
   {
      public readonly int X, Y, Z;

      public Koordinata_3D(int _x, int _y, int _z)
      {
         this.X = _x;
         this.Y = _y;
         this.Z = _z;
      }
   }

   /// <summary>
   /// Ovo neka ti bude ogledni primjer za kratke struct-ure 
   /// </summary>
   public struct DbNavigationRestrictor
   {
      public string[] RestrictedValues { get; set; }
      public string ColName { get; set; }

      public DbNavigationRestrictor(string _colName, string[] _colValues) : this()
      {
         ColName = _colName;
         RestrictedValues = _colValues;
      }

      public static DbNavigationRestrictor Empty = new ZXC.DbNavigationRestrictor(null, null);

      public bool IsEmpty  {  get { return (this.ColName == null || this.RestrictedValues == null); } }

      public bool NotEmpty {  get { return !IsEmpty; } }
   }

   public struct VvRptExternTblChooser_Placa
   {
      // dopuni listu bool-ova kako ce ti ubuduce trebati... 
      public bool Person { get; set; }
      public bool Kupdob { get; set; }
      public bool Kupdob2 { get; set; }
      public bool PtranE { get; set; }
      public bool PtranO { get; set; }

      public VvRptExternTblChooser_Placa(bool person, bool kupdob, bool kupdob2, bool ptranE, bool ptranO) : this()
      {
         this.Person = person;
         this.Kupdob = kupdob;
         this.Kupdob2 = kupdob2;
         this.PtranE = ptranE;
         this.PtranO = ptranO;
      }

      public static VvRptExternTblChooser_Placa Empty = new VvRptExternTblChooser_Placa(false, false, false, false, false);
   }

   public struct CdAndName_CommonStruct
   {
      public string TheCd { get; set; }
      public string TheName { get; set; }
      public uint TheUint { get; set; }
      public DateTime TheDate { get; set; }

      public CdAndName_CommonStruct(string _theCd, string _theName) : this()
      {
         TheCd = _theCd;
         TheName = _theName;
      }

      public CdAndName_CommonStruct(string _theCd, string _theName, uint _theUint) : this()
      {
         TheCd = _theCd;
         TheName = _theName;
         TheUint = _theUint;
      }

      public CdAndName_CommonStruct(string _theCd, uint _theUint, DateTime _theDate) : this()
      {
         TheCd = _theCd;
         TheUint = _theUint;
         TheDate = _theDate;
      }

      public override string ToString()
      {
         return TheName + "-" + TheCd + "-" + TheUint.ToString() + "-" + TheDate.ToString(ZXC.VvDateFormat);
      }
   }

   public struct NameAndDecimal_CommonStruct
   {
      public string TheName { get; set; }
      public decimal TheDecimal { get; set; }
      public uint TheUint { get; set; }

      public NameAndDecimal_CommonStruct(string _theName, decimal _theDecimal) : this()
      {
         TheName = _theName;
         TheDecimal = _theDecimal;
      }

      public NameAndDecimal_CommonStruct(string _theName, decimal _theDecimal, uint _theUint) : this()
      {
         TheName = _theName;
         TheDecimal = _theDecimal;
         TheUint = _theUint;
      }

      public override string ToString()
      {
         return TheName + " (" + TheDecimal.ToStringVv() + ")";
      }
   }

   public struct VvUtilDataPackage
   {
      public string TheStr1 { get; set; }
      public string TheStr2 { get; set; }
      public decimal TheDecimal { get; set; }
      public decimal TheDecimal2 { get; set; }
      public bool TheBool { get; set; }
      public string TheStr3 { get; set; }
      public uint TheUint { get; set; }
      public int TheInt { get; set; }
      public string TheStr4 { get; set; }
      public uint TheUint2 { get; set; }
      public string TheStr5 { get; set; }

      public decimal TheDecimal3 { get; set; }
      public decimal TheDecimal4 { get; set; }

      public DateTime TheDate { get; set; }

      public VvUtilDataPackage(string _theStr1, string _theStr2, decimal _theDecimal, bool _theBool) : this()
      {
         TheStr1 = _theStr1;
         TheStr2 = _theStr2;
         TheDecimal = _theDecimal;
         TheBool = _theBool;
      }

      public VvUtilDataPackage(string _theStr1, string _theStr2, decimal _theDecimal, decimal _theDecimal2) : this()
      {
         TheStr1 = _theStr1;
         TheStr2 = _theStr2;
         TheDecimal = _theDecimal;
         TheDecimal2 = _theDecimal2;
      }

      public VvUtilDataPackage(uint _theUint, string _theStr2, decimal _theDecimal, decimal _theDecimal2) : this()
      {
         TheUint = _theUint;
         TheStr2 = _theStr2;
         TheDecimal = _theDecimal;
         TheDecimal2 = _theDecimal2;
      }

      public VvUtilDataPackage(string _theStr1, string _theStr2, decimal _theDecimal) : this()
      {
         TheStr1 = _theStr1;
         TheStr2 = _theStr2;
         TheDecimal = _theDecimal;
         TheBool = false;
      }

      public VvUtilDataPackage(string _theStr1, decimal _theDecimal) : this()
      {
         TheStr1 = _theStr1;
         TheStr2 = "";
         TheDecimal = _theDecimal;
         TheBool = false;
      }

      public VvUtilDataPackage(string _theStr1, decimal _theDecimal, decimal _theDecimal2) : this()
      {
         TheStr1 = _theStr1;
         TheStr2 = "";
         TheDecimal = _theDecimal;
         TheDecimal2 = _theDecimal2;
         TheBool = false;
      }

      public VvUtilDataPackage(string _theStr1, decimal _theDecimal, bool _theBool) : this()
      {
         TheStr1 = _theStr1;
         TheStr2 = "";
         TheDecimal = _theDecimal;
         TheBool = _theBool;
      }

      //public VvUtilDataPackage(string _theStr1, string _theStr2, decimal _theDecimal, bool _theBool, string _theStr3, uint _theUint, string _theStr4/*, uint _theUint2*/, string _theStr5                   ) : this()
      public VvUtilDataPackage(string _theStr1, string _theStr2, decimal _theDecimal, bool _theBool, string _theStr3, uint _theUint, string _theStr4, uint _theUint2, string _theStr5, DateTime _theDate) : this()
      {
         TheStr1 = _theStr1;
         TheStr2 = _theStr2;
         TheDecimal = _theDecimal;
         TheBool = _theBool;
         TheStr3 = _theStr3;
         TheUint = _theUint;
         TheStr4 = _theStr4;
         TheUint2 = _theUint2;
         TheStr5 = _theStr5;
         TheDate = _theDate;
      }

      public VvUtilDataPackage(DateTime _theDate, decimal _theDecimal, decimal _theDecimal2) : this()
      {
         TheDate = _theDate;
         TheDecimal = _theDecimal;
         TheDecimal2 = _theDecimal2;
      }

      public VvUtilDataPackage(DateTime _theDate, uint _theUint, int _theInt) : this()
      {
         TheDate = _theDate;
         TheUint = _theUint;
         TheInt = _theInt;
      }

      public VvUtilDataPackage(string _theStr1, uint _theUint) : this()
      {
         TheStr1 = _theStr1;
         TheUint = _theUint;
      }

      /// <summary>
      /// Za SerlotInfo upotrebu
      /// </summary>
      /// <param name="rtrSerlotGR"></param>
      /// <param name="serlotKolSt"></param>
      public VvUtilDataPackage(IGrouping<string, Rtrans> rtrSerlotGR, decimal serlotKolSt) : this()
      {
         this.TheDecimal = serlotKolSt;
         this.TheStr1 = rtrSerlotGR.Key;

         Rtrans firstUlazRtrans = rtrSerlotGR.FirstOrDefault(rtrans => rtrans.TtInfo.IsFinKol_U);

         if(firstUlazRtrans != null)
         {
            TheDate = firstUlazRtrans.T_skladDate;

            string kupdobInfo = "";
            Kupdob kupdob = VvUserControl.KupdobSifrar.SingleOrDefault(k => k.KupdobCD == firstUlazRtrans.T_kupdobCD);
            if(kupdob != null) kupdobInfo = kupdob.Naziv;

            TheStr2 = firstUlazRtrans.TtAndTtNum + " od " + firstUlazRtrans.T_skladDate.ToString(ZXC.VvDateFormat) + " - " + kupdobInfo;
         }
      }

      public override string ToString()
      {
         return "(" + TheStr1 + ")" + "(" + TheStr2 + ")" + TheDecimal.ToStringVv() + "/" + TheBool + "/" + TheDate.ToString(ZXC.VvDateFormat);
      }
   }

   public struct FlagForKey
   {
      public bool flagState;
      public string keyName;

      public FlagForKey(string _key, bool _state)
      {
         this.keyName = _key;
         this.flagState = _state;
      }
   }

   public const uint LanRecIdBase = 10000000;
   public static uint GetSrvRecID(uint lanSrvID, uint lanRecID)
   {
      return lanSrvID * LanRecIdBase + lanRecID;
   }

   public struct DBactionForSrvRecID
   {
      public uint recID;
      public uint lanSrvID;
      public uint lanRecID;
      public VvSQL.DB_RW_ActionType action;

      public uint SrvRecID { get { return GetSrvRecID(this.lanSrvID, this.lanRecID); } }

      public DBactionForSrvRecID(uint _recID, uint _lanSrvID, uint _lanRecID, VvSQL.DB_RW_ActionType _action)
      {
         this.recID    = _recID   ;
         this.lanSrvID = _lanSrvID;
         this.lanRecID = _lanRecID;
         this.action   = _action  ;

         VvXmlDR_FullPathFileName = "";
      }

      // 26.09.2019: za potrebe 'Restore_FromVvXmlDR' 
      internal string VvXmlDR_FullPathFileName { get; set; }

      public override string ToString()
      {
         return recID.ToString() + "--->" + action.ToString();
      }
   }

   public static ZXC.ValutaNameEnum GetValutaNameEnumFromValutaName(string valutaName)
   {
      if(valutaName.IsEmpty()) return /*ZXC.ValutaNameEnum.HRK*/EURorHRK_NameEnum;

      return (ZXC.ValutaNameEnum)Enum.Parse(typeof(ZXC.ValutaNameEnum), valutaName, true);
   }

   public struct kamtbl
   {
      public DateTime[] kt_date;
      public decimal[] kt_stopaFiz;
      public decimal[] kt_stopaPra;

      public kamtbl(int size)
      {
         kt_date = new DateTime[size + 1];
         kt_stopaFiz = new decimal[size + 1]; // Fizicke osobe 
         kt_stopaPra = new decimal[size + 1]; // Pravne osobe  

         numOfDateBreaks = size;
      }

      // 11.05.2016: 
      public int numOfDateBreaks;

   }

   public struct SVD_PotrosnjaInfo
   {
      public string   SinName   { get; set; }
      public string   AnaName   { get; set; }
      public DateTime TheDateOD { get; set; }
      public DateTime TheDateDO { get; set; } // to je i DokDate 

      public decimal AnaLimitMM { get; set; }
      public decimal SinLimitMM { get; set; }
      public decimal SVDLimitMM { get; set; }
      public decimal AnaLimitPP { get { int forHowManyMonths = TheDateDO.Month - TheDateOD.Month + 1; return AnaLimitMM * forHowManyMonths; } } // period 
      public decimal SinLimitPP { get { int forHowManyMonths = TheDateDO.Month - TheDateOD.Month + 1; return SinLimitMM * forHowManyMonths; } } // period 
      public decimal SVDLimitPP { get { int forHowManyMonths = TheDateDO.Month - TheDateOD.Month + 1; return SVDLimitMM * forHowManyMonths; } } // period 
      public decimal AnaLimitYY { get { return AnaLimitMM * 12M; } }
      public decimal SinLimitYY { get { return SinLimitMM * 12M; } }
      public decimal SVDLimitYY { get { return SVDLimitMM * 12M; } }

      public decimal SinTotalYY { get; set; } // ovaj se koristi i za period               // totalna potrosnja 
      public decimal AnaTotalYY { get; set; } // ovaj se koristi i za period               // totalna potrosnja 
      public decimal SinTotalMM { get; set; }                                              // totalna potrosnja 
      public decimal AnaTotalMM { get; set; }                                              // totalna potrosnja 
      public decimal SinTotalPP { get { return SinTotalYY; } } // za lakse dole postotke   // totalna potrosnja 
      public decimal AnaTotalPP { get { return AnaTotalYY; } } // za lakse dole postotke   // totalna potrosnja 

      public decimal SinUtrosYY { get; set; } // ovaj se koristi i za period               // limitirana potrisnja - samo sa skl '10' 
      public decimal AnaUtrosYY { get; set; } // ovaj se koristi i za period               // limitirana potrisnja - samo sa skl '10' 
      public decimal SinUtrosMM { get; set; }                                              // limitirana potrisnja - samo sa skl '10' 
      public decimal AnaUtrosMM { get; set; }                                              // limitirana potrisnja - samo sa skl '10' 
      public decimal SinUtrosPP { get { return SinUtrosYY; } } // za lakse dole postotke   // limitirana potrisnja - samo sa skl '10' 
      public decimal AnaUtrosPP { get { return AnaUtrosYY; } } // za lakse dole postotke   // limitirana potrisnja - samo sa skl '10' 

      public decimal SinLeftoverYY { get { return SinLimitYY - SinUtrosYY; } }
      public decimal AnaLeftoverYY { get { return AnaLimitYY - AnaUtrosYY; } }
      public decimal SinLeftoverMM { get { return SinLimitMM - SinUtrosMM; } }
      public decimal AnaLeftoverMM { get { return AnaLimitMM - AnaUtrosMM; } }
      public decimal SinLeftoverPP { get { return SinLimitPP - SinUtrosPP; } } // period 
      public decimal AnaLeftoverPP { get { return AnaLimitPP - AnaUtrosPP; } } // period 

      public decimal SinPostoUtrosYY    { get { return ZXC.DivSafe(SinUtrosYY   , SinLimitYY) * 100M; } }
      public decimal AnaPostoUtrosYY    { get { return ZXC.DivSafe(AnaUtrosYY   , AnaLimitYY) * 100M; } }
      public decimal SinPostoLeftoverYY { get { return ZXC.DivSafe(SinLeftoverYY, SinLimitYY) * 100M; } }
      public decimal AnaPostoLeftoverYY { get { return ZXC.DivSafe(AnaLeftoverYY, AnaLimitYY) * 100M; } }

      public decimal SinPostoUtrosMM    { get { return ZXC.DivSafe(SinUtrosMM   , SinLimitMM) * 100M; } }
      public decimal AnaPostoUtrosMM    { get { return ZXC.DivSafe(AnaUtrosMM   , AnaLimitMM) * 100M; } }
      public decimal SinPostoLeftoverMM { get { return ZXC.DivSafe(SinLeftoverMM, SinLimitMM) * 100M; } }
      public decimal AnaPostoLeftoverMM { get { return ZXC.DivSafe(AnaLeftoverMM, AnaLimitMM) * 100M; } }

      public decimal SinPostoUtrosPP    { get { return ZXC.DivSafe(SinUtrosPP   , SinLimitPP) * 100M; } } // period 
      public decimal AnaPostoUtrosPP    { get { return ZXC.DivSafe(AnaUtrosPP   , AnaLimitPP) * 100M; } } // period 
      public decimal SinPostoLeftoverPP { get { return ZXC.DivSafe(SinLeftoverPP, SinLimitPP) * 100M; } } // period 
      public decimal AnaPostoLeftoverPP { get { return ZXC.DivSafe(AnaLeftoverPP, AnaLimitPP) * 100M; } } // period 

   }

   public struct VvXmlValidationData
   {
      public string targetNamespace;
      public string schemaUri;

      public VvXmlValidationData(string _targetNamespace, string _schemaUri) : this()
      {
         targetNamespace = _targetNamespace;
         schemaUri = _schemaUri;
      }

   }

   #endregion Global Types (some structs)

   #region Enums

   public enum VvModulEnum
   {
      MODUL_PRJKT = 0,

      MODUL_RAS = 1,
      MODUL_FIN = 2,
      MODUL_PLA = 3,
      MODUL_PADR = 4,
      MODUL_OSR = 5,

      MODUL_PRIJEM = 6,
      MODUL_OPRANA = 7,

      MODUL_DEBIT = 8,

      MODUL_UNDEF = 9,

      MODUL_OTHER = 10,

      MODUL_ANKETA = 11

   }

   public enum VvSubModulKindEnum
   {
      SIFRAR = 1,
      DOCUMENT = 2,
      REPORT_MENU = 3,
      RecLIST = 4,
      REPORT_ITEM = 5,
      DOCUMENT_LIKE = 6,
      OTHER = 7,
      UNDEF = 0
   }

   public enum VvTabPageKindEnum
   {
      RECORD_TabPage,
      REPORT_TabPage,
      RecLIST_TabPage,
      RECORD_TabPage_INTERACTIVE,
      OTHER_TabPage,
   }

   public enum VvInnerTabPageKindEnum
   {
      ReadWrite_TabPage,
      TransGrid_TabPage,
      ReportViewer_TabPage,
   }

   public enum VvSubModulEnum
   {
      /// <summary>
      /// Other
      /// </summary>
      UNDEF,

      /// <summary>
      /// Artikl-skladiste
      /// </summary>
      ART,

      /// <summary>
      /// Lista artikala
      /// </summary>
      LsART,

      /// <summary>
      /// Lista rtransa
      /// </summary>
      LsRTR,

      /// <summary>
      /// Ulazni Racun
      /// </summary>
      R_UFA,

      /// <summary>
      /// Ulazni Racun u MALOP (NE na skladiste, PDV only)
      /// </summary>
      R_UFM,

      /// <summary>
      /// Ulazni Racun Devizni
      /// </summary>
      R_UFAdev,

      /// <summary>
      /// Ulazni Racun + Primka 
      /// </summary>
      R_URA,

      /// <summary>
      /// PDV Temeljnica - EU virtualni porez, PDV pri uvozu iz NOT EU
      /// </summary>
      R_UPA,

      /// <summary>
      /// Ulazni Racun + rtrano+ Primka 
      /// </summary>
      R_URP,

      /// <summary>
      /// Ulazni Racun u MALOP + kalkulacija
      /// </summary>
      R_URM,

      /// <summary>
      /// Ulazni Racun u MALOP + kalkulacija prosireno
      /// </summary>
      R_URM_2,

      /// <summary>
      /// Ulazni Racun u MALOP + Devizni
      /// </summary>
      R_URM_D,

      /// <summary>
      /// Primka Veleprodaja
      /// </summary>
      R_PRI,

      /// <summary>
      /// Primka Veleprodaja Proizvodnja SerNo
      /// </summary>
      R_PRI_P,

      /// <summary>
      /// Primka Veleprodaja Devizna
      /// </summary>
      R_PRIdev,

      /// <summary>
      /// Primka Veleprodaja - Bez Cijena
      /// </summary>
      R_PRI_bc,

      /// <summary>
      /// Primka Maloprodaja
      /// </summary>
      R_KLK,

      /// <summary>
      /// Primka Maloprodaja prosirena
      /// </summary>
      R_KLK_2,

      /// <summary>
      /// KOMISIJSKA Primka Maloprodaja
      /// </summary>
      R_KKM,

      /// <summary>
      /// Primka Maloprodaja DEVIZNA
      /// </summary>
      R_KLD,

      /// <summary>
      /// Izlazni racun Veleprodajni
      /// </summary>
      R_IFA,


      /// <summary>
      /// Izlazni racun Veleprodajni Devizni
      /// </summary>
      R_IFAdev,

      /// <summary>
      /// Izlazni racun Veleprodajni + Izdatnica
      /// </summary>
      R_IRA,

      /// <summary>
      /// Izlazni racun +rtrano Veleprodajni + Izdatnica
      /// </summary>
      R_IRP,

      /// <summary>
      /// Izdatnica
      /// </summary>
      R_IZD,

      /// <summary>
      /// Izlazni racun Maloprodajni
      /// </summary>
      R_IRM,

      /// <summary>
      /// Izlazni racun Maloprodajni zaglavlje kao Veleprodajni
      /// </summary>
      R_IRM_2,

      /// <summary>
      /// Izdatnica Maloprodajna
      /// </summary>
      R_IZM,

      /// <summary>
      /// Izdatnica Maloprodajna inf za BOR
      /// </summary>
      R_IZM_2,

      /// <summary>
      /// Medjuskladisnice VELEP
      /// </summary>
      R_MSI,

      /// <summary>
      /// Medjuskladisnice MALOP
      /// </summary>
      R_MMI,

      /// <summary>
      /// Medjuskladisnice
      /// </summary>
      R_MSI_2,

      /// <summary>
      /// Medjuskladisnice - KOMISIJSKE, EKSTERNI dok.
      /// </summary>
      R_KIZ,

      /// <summary>
      /// Medjuskladisnice - KOMISIJSKE, EKSTERNI dok. POVRAT 
      /// </summary>
      R_PIK,

      /// <summary>
      /// Medjuskladisnice VELEP 2 MALOP
      /// </summary>
      R_VMI,

      /// <summary>
      /// Medjuskladisnice MALOP 2 VELEP
      /// </summary>
      R_MVI,

      /// <summary>
      /// Medjuskladisnice MALOP 2 VELEP
      /// </summary>
      R_MVI_2,

      /// <summary>
      /// Proizvodnja
      /// </summary>
      R_PIZ,

      /// <summary>
      /// Proizvodnja + rtrano
      /// </summary>
      R_PIZ_P,

      /// <summary>
      /// Cjenik
      /// </summary>
      R_CJ,

      /// <summary>
      /// Pocetno stanje
      /// </summary>
      R_PST,

      /// <summary>
      /// Pocetno stanje MALOPRODAJE
      /// </summary>
      R_PSM,

      /// <summary>
      /// Inventura MALOPRODAJE
      /// </summary>
      R_INM,

      /// <summary>
      /// Inventura
      /// </summary>
      R_INV,

      /// <summary>
      /// Skladisni ulaz/izlaz Only
      /// </summary>
      R_SKO,

      /// <summary>
      /// Skladisni izlaz Only
      /// </summary>
      R_SKU,

      /// <summary>
      /// Skladisni izlaz Only
      /// </summary>
      R_SKI,

      /// <summary>
      /// Obvezujuca ponuda
      /// </summary>
      R_OPN,

      /// <summary>
      /// Ulazno Odobrenje
      /// </summary>
      R_UOD,

      /// <summary>
      /// Ulazno Povrat
      /// </summary>
      R_UPV,

      /// <summary>
      /// Ulazno Povrat - MALOP
      /// </summary>
      R_UPM,

      /// <summary>
      /// Izlazno Odobrenje
      /// </summary>
      R_IOD,

      /// <summary>
      /// Izlazno Povrat
      /// </summary>
      R_IPV,

      /// <summary>
      /// Ponuda kupcu
      /// </summary>
      R_PON,

      /// <summary>
      /// Ponuda kupcu MALPRODAJNA (MPC)
      /// </summary>
      R_PNM,

      /// <summary>
      /// Narudzba dobavljacu
      /// </summary>
      R_NRD,

      /// <summary>
      /// Narudzba Uvoza dobavljacu
      /// </summary>
      R_NRU,

      /// <summary>
      /// Narudzba za USLUGU dobavljacu
      /// </summary>
      R_NRS,

      /// <summary>
      /// Narudzba kupca
      /// </summary>
      R_NRK,

      /// <summary>
      /// Narudzba dobavljacu MALOPRODAJNA
      /// </summary>
      R_NRM,

      /// <summary>
      /// Korekturna temeljnica ulaza/izlaza
      /// </summary>
      R_TMK,

      /// <summary>
      /// Nivelacija - zapisnik o promjeni cijena
      /// </summary>
      R_ZPC,

      /// <summary>
      /// Nivelacija - zapisnik o promjeni cijena
      /// </summary>
      R_ZKC,

      /// <summary>
      /// Izdatnica na mjesto troska
      /// </summary>
      R_IMT,

      /// <summary>
      /// Izdatnica na mjesto troska MALOP 
      /// </summary>
      //R_IMM, FUSE ?

      /// <summary>
      /// Izdatnica u Proizvodnju
      /// </summary>
      R_PPR,

      /// <summary>
      /// Povratnica (npr iz proizvodnje)
      /// </summary>
      R_POV,

      /// <summary>
      /// Primka iz proizvodnje
      /// </summary>
      R_PIP,

      /// <summary>
      /// Blagajnicka uplatnica
      /// </summary>
      R_UPL,

      /// <summary>
      /// Blagajnicka uplatnica - Multi + Dev
      /// </summary>
      R_BUP,

      /// <summary>
      /// Blagajnicka isplatnica
      /// </summary>
      R_ISP,

      /// <summary>
      /// Blagajnicka isplatnica - Multi + Dev
      /// </summary>
      R_BIS,

      /// <summary>
      /// Storno ulaza
      /// </summary>
      R_STU,

      /// <summary>
      /// Storno izlaza
      /// </summary>
      R_STI,

      /// <summary>
      /// Revers
      /// </summary>
      R_RVI,

      /// <summary>
      /// Revers povrat
      /// </summary>
      R_RVU,

      /// <summary>
      /// Radni Nalog PROIZVODNJE - Frigoterm
      /// </summary>
      R_RNP,

      /// <summary>
      /// Radni Nalog PROIZVODNJE - Metaflex
      /// </summary>
      R_RNM,

      /// <summary>
      /// Radni Nalog SERVISA 
      /// </summary>
      R_RNS,

      /// <summary>
      /// Projekt - opci Radni Nalog 
      /// </summary>
      R_PRJ,

      /// <summary>
      /// Boravak gosta
      /// </summary>
      R_BOR,

      /// <summary>
      /// Normativ - VELEPRODAJNI
      /// </summary>
      R_NOR,

      /// <summary>
      /// Normativ - MALOPRODAJNI
      /// </summary>
      R_NOM,

      /// <summary>
      /// Proizvodnja maloprodajna
      /// </summary>
      R_PIM,

      /// <summary>
      /// Proizvodnja velep 2 maloprodajna
      /// </summary>
      R_TRI,

      /// <summary>
      /// Otvoreni racuni proslih godina
      /// </summary>
      R_WYR,


      /// <summary>
      /// Lista faktura
      /// </summary>
      LsFAK,

      /// <summary>
      /// Lista Ulazni Racun
      /// </summary>
      LsR_UFA,

      /// <summary>
      /// Lista Ulazni Racun - MALOP
      /// </summary>
      LsR_UFM,

      /// <summary>
      /// Lista Ulazni Racun + Primka 
      /// </summary>
      LsR_URA,

      /// <summary>
      /// Lista Primka Veleprodaja
      /// </summary>
      LsR_PRI,

      /// <summary>
      /// Lista Primka Maloprodaja
      /// </summary>
      LsR_KLK,

      /// <summary>
      /// Lista Izlazni racun Veleprodajni
      /// </summary>
      LsR_IFA,

      /// <summary>
      /// Lista Izlazni racun Veleprodajni + Izdatnica
      /// </summary>
      LsR_IRA,

      /// <summary>
      /// Lista Izdatnica
      /// </summary>
      LsR_IZD,

      /// <summary>
      /// Lista Izlazni racun Maloprodajni
      /// </summary>
      LsR_IRM,

      /// <summary>
      /// Lista Medjuskladisnice
      /// </summary>
      LsR_MSI,

      /// <summary>
      /// Lista Proizvodnja
      /// </summary>
      LsR_PIZ,

      /// <summary>
      /// Lista Cjenik
      /// </summary>
      LsR_CJ,

      /// <summary>
      /// Lista Pocetno stanje
      /// </summary>
      LsR_PST,

      /// <summary>
      /// Lista Inventura
      /// </summary>
      LsR_INV,

      /// <summary>
      /// Lista Skladisni ulaz/izlaz Only
      /// </summary>
      LsR_SKO,

      /// <summary>
      /// Lista Obvezujuca ponuda
      /// </summary>
      LsR_OPN,

      /// <summary>
      /// Lista Ulazno Odobrenje
      /// </summary>
      LsR_UOD,

      /// <summary>
      /// Lista Ulazno Povrat
      /// </summary>
      LsR_UPV,

      /// <summary>
      /// Lista Izlazno Odobrenje
      /// </summary>
      LsR_IOD,

      /// <summary>
      /// Lista Izlazno Povrat
      /// </summary>
      LsR_IPV,

      /// <summary>
      /// Lista Ponuda kupcu
      /// </summary>
      LsR_PON,

      /// <summary>
      /// Lista Narudzba dobavljacu
      /// </summary>
      LsR_NRD,

      /// <summary>
      /// Lista Narudzba kupca
      /// </summary>
      LsR_NRK,

      /// <summary>
      /// Lista Korekturna temeljnica ulaza/izlaza
      /// </summary>
      LsR_TMK,

      /// <summary>
      /// Lista Nivelacija - zapisnik o promjeni cijena
      /// </summary>
      LsR_ZPC,

      /// <summary>
      /// Lista Izdatnica na mjesto troska
      /// </summary>
      LsR_IMT,

      /// <summary>
      /// Lista Blagajnicka uplatnica
      /// </summary>
      LsR_UPL,

      /// <summary>
      /// Lista Blagajnicka isplatnica
      /// </summary>
      LsR_ISP,

      /// <summary>
      /// Lista Storno ulaza
      /// </summary>
      LsR_STU,

      /// <summary>
      /// Lista Storno izlaza
      /// </summary>
      LsR_STI,

      /// <summary>
      /// Lista Revers
      /// </summary>
      LsR_RVI,

      /// <summary>
      /// Lista Revers povrat
      /// </summary>
      LsR_RVU,

      /// <summary>
      /// Materijalno izvjestaji
      /// </summary>
      RIZ,

      /// <summary>
      /// Kartica konta
      /// </summary>
      KPL,

      /// <summary>
      /// Lista konta
      /// </summary>
      LsKPL,

      /// <summary>
      /// Nalog firme
      /// </summary>
      NAL_F,

      /// <summary>
      /// Nalog Mtr
      /// </summary>
      NAL_M,

      /// <summary>
      /// Nalog obrt
      /// </summary>
      NAL_O,

      /// <summary>
      /// Nalog Projekt
      /// </summary>
      NAL_P,

      /// <summary>
      /// Lista Nalog 
      /// </summary>
      LsNAL,

      /// <summary>
      /// Financijski izjestaji
      /// </summary>
      FIZ,

      /// <summary>
      /// Financijski izjestaji
      /// </summary>
      GFI_TSI,

      /// <summary>
      /// Statisticki izvjestiji NPF
      /// </summary>
      STAT_NPF,

      /// <summary>
      /// Plan
      /// </summary>
      PLAN,

      /// <summary>
      /// Kartica radnika
      /// </summary>
      PER,

      /// <summary>
      /// Lista Radnika
      /// </summary>
      LsPER,

      /// <summary>
      /// Obracun palce do 2014
      /// </summary>
      PLA,

      /// <summary>
      /// Obracun palce od 1.1.2014.
      /// </summary>
      PLA_2014,

      /// <summary>
      /// Obracun neoporezivih primitaka primjena od 1.1.2014.
      /// </summary>
      PLA_NP,

      /// <summary>
      /// Lista obracuna placa
      /// </summary>
      LsPLA,

      /// <summary>
      /// Placa izvjestaji
      /// </summary>
      PIZ,

      /// <summary>
      /// Osnovna sredstva sifra
      /// </summary>
      OSR,

      /// <summary>
      /// Lista osnovnih sredstava
      /// </summary>
      LsOSR,

      /// <summary>
      /// Osnovna sredstva obracun
      /// </summary>
      AMO,

      /// <summary>
      /// Lista obracuna osnov.sredstava
      /// </summary>
      LsAMO,

      /// <summary>
      /// Osnov.sr izvjestaji
      /// </summary>
      OIZ,

      /// <summary>
      /// Karica partnera
      /// </summary>
      KID,

      /// <summary>
      /// Lista partnera
      /// </summary>
      LsKID,

      /// <summary>
      /// Partneri izvjestaj
      /// </summary>
      KIZ,

      /// <summary>
      /// Projekt
      /// </summary>
      PRJ,

      /// <summary>
      /// Lista Projekt
      /// </summary>
      LsPRJ,

      /// <summary>
      /// Projekt izvjestaji
      /// </summary>
      PRIZ,

      /// <summary>
      /// User
      /// </summary>
      USR,

      /// <summary>
      /// Lista usera
      /// </summary>
      LsUSR,

      /// <summary>
      /// Privilegije
      /// </summary>
      PRV,

      /// <summary>
      /// Lista privilegija
      /// </summary>
      LsPRV,

      /// <summary>
      /// SkyRule
      /// </summary>
      SKY,

      /// <summary>
      /// Lista SkyRule
      /// </summary>
      LsSKY,

      /// <summary>
      /// Devizni Tecaj
      /// </summary>
      DTEC,

      /// <summary>
      /// Lista deviznog tecaja
      /// </summary>
      LsDTEC,

      /// <summary>
      /// Prijem
      /// </summary>
      Vsr_PRM,

      /// <summary>
      /// Lista prijema
      /// </summary>
      Vsr_LsPRM,

      /// <summary>
      /// Prijem
      /// </summary>
      Vsr_JIL,

      /// <summary>
      /// Lista prijema
      /// </summary>
      Vsr_LsJIL,

      /// <summary>
      /// Prijem izvjestaji
      /// </summary>
      Vsr_PRZ,

      /// <summary>
      /// OperacijaA
      /// </summary>
      Vsr_OPRa,

      /// <summary>
      /// OperacijaB
      /// </summary>
      Vsr_OPRb,

      /// <summary>
      /// Anestezija
      /// </summary>
      Vsr_ANA,

      /// <summary>
      /// Lista operacijaA
      /// </summary>
      Vsr_LsOPa,

      /// <summary>
      /// Lista operacijaB
      /// </summary>
      Vsr_LsOPb,

      /// <summary>
      /// Lista anestezija
      /// </summary>
      Vsr_LsANA,

      /// <summary>
      /// Operacija/anestezija izvjestaji
      /// </summary>
      Vsr_OAI,

      /// <summary>
      /// Anketa
      /// </summary>
      Vsr_ANK,

      /// <summary>
      /// Lista anketa
      /// </summary>
      Vsr_LsANK,

      /// <summary>
      /// Anketa izvjestaji
      /// </summary>
      Vsr_ANKIz,

      /// <summary>
      /// Separator
      /// </summary>
      SEPARATOR,

      /// <summary>
      /// Debit
      /// </summary>
      Vrm_DBT,

      /// <summary>
      /// Lista debita
      /// </summary>
      Vrm_LsDBT,

      /// <summary>
      /// Zabranjeni, bez privilegije 
      /// </summary>

      /// <summary>
      /// Debit izvjestaji
      /// </summary>
      Vrm_DIZ,

      /// <summary>
      /// SubModulGrupa1
      /// </summary>
      GROUP1,

      /// <summary>
      /// SubModulGrupa2
      /// </summary>
      GROUP2,

      /// <summary>
      /// SubModulGrupa3
      /// </summary>
      GROUP3,

      /// <summary>
      /// SubModulGrupa4
      /// </summary>
      GROUP4,

      /// <summary>
      /// SubModulGrupa5
      /// </summary>
      GROUP5,

      /// <summary>
      /// SubModulGrupa6
      /// </summary>
      GROUP6,

      /// <summary>
      /// SubModulGrupa7
      /// </summary>
      GROUP7,

      /// <summary>
      /// SubModulGrupa8
      /// </summary>
      GROUP8,

      /// <summary>
      /// SubModulGrupa9
      /// </summary>
      GROUP9,

      /// <summary>
      /// Other - Virman
      /// </summary>
      X_VIR,

      /// <summary>
      /// Other - Putni nalog Tuzemni
      /// </summary>
      X_PNT,

      /// <summary>
      /// Other - Putni nalog Inozemni
      /// </summary>
      X_PNI,

      /// <summary>
      /// Other - Putni radni list
      /// </summary>
      X_PNR,

      /// <summary>
      /// Other - Loko voznja
      /// </summary>
      X_PNL,

      /// <summary>
      /// Other - Raster
      /// </summary>
      X_RAS,

      /// <summary>
      /// Other - RasterB
      /// </summary>
      X_RAS_B,

      /// <summary>
      /// Lista other
      /// </summary>
      LsMIX,

      /// <summary>
      /// Other izvjestaji - PutniNalog
      /// </summary>
      XIZ_P,

      /// <summary>
      /// Other - Zahtjev
      /// </summary>
      X_ZAH,

      /// <summary>
      /// Other - ZahtjevRNM
      /// </summary>
      X_ZAH_RNM,

      /// <summary>
      /// Other izvjestaji - Zahtjevi
      /// </summary>
      XIZ_Z,

      /// <summary>
      /// Other - SMD - servisno montazni dnevnik
      /// </summary>
      X_SMD,

      /// <summary>
      /// Other izvjestaji - SMD
      /// </summary>
      XIZ_S,

      /// <summary>
      /// Other - Evidencije
      /// </summary>
      X_EVD,

      /// <summary>
      /// Other - Urudzbeni zapisnik
      /// </summary>
      X_URZ,

      /// <summary>
      /// Other - Registar ugovora
      /// </summary>
      X_RUG,

      /// <summary>
      /// Other - Registar cijevi
      /// </summary>
      X_RGC,

      /// <summary>
      /// Other izvjestaji - Evidencije
      /// </summary>
      XIZ_E,

      /// <summary>
      /// Other - RVR - evidencija radnog vremena
      /// </summary>
      X_RVR,

      /// <summary>
      /// Other - RVR - evidencija radnog vremena od 04 2015
      /// </summary>
      X_RVR2,

      /// <summary>
      /// Other - RVI - evidencija radnog vremena interna
      /// </summary>
      X_IRV,

      /// <summary>
      /// Other - RVR - mjesecna evidencija radnog vremena
      /// </summary>
      X_MVR,

      /// <summary>
      /// Other izvjestaji - RVR
      /// </summary>
      XIZ_R,

      /// <summary>
      /// Other - ExtCjenici
      /// </summary>
      X_EXC,

      /// <summary>
      /// Other - Poseban porez na motorna vozila
      /// </summary>
      X_PMV,

      /// <summary>
      /// Other - GST - knjiga gostiju
      /// </summary>
      X_GST,

      /// <summary>
      /// Other izvjestaji - GST
      /// </summary>
      XIZ_G,

      /// <summary>
      /// Sinkronizacija
      /// </summary>
      SIN,

      /// <summary>
      /// SubModulGrupa10
      /// </summary>
      GROUP10,

      /// <summary>
      /// Radni Nalog ZASTITARA 
      /// </summary>
      R_RNZ,

      /// <summary>
      /// Other - ZLJ - lijecnicki pregledi
      /// </summary>
      X_ZLJ,

      /// <summary>
      /// Other - ZPG - provjera gadanja 
      /// </summary>
      X_ZPG,

      /// <summary>
      /// Other izvjestaji - zastitari
      /// </summary>
      XIZ_B,

      /// <summary>
      /// Other - AVR - mjesecni raspored radnog vremena
      /// </summary>
      X_AVR,

      /// <summary>
      /// Other - BMW
      /// </summary>
      X_BMW,

      /// <summary>
      /// Ulazni Racun + Primka SVDuh
      /// </summary>
      R_URA_SVD,

      /// <summary>
      /// Izdatnica SVDuh
      /// </summary>
      R_IZD_SVD,

      /// <summary>
      /// Narudzba Primka SVDuh
      /// </summary>
      R_NRD_SVD,

      /// <summary>
      /// Ugovor - Tender - SvDUH ...
      /// </summary>
      R_UGO,

      /// <summary>
      /// Prednarudzba - SvDUH ...
      /// </summary>
      X_PNA,

      /// <summary>
      /// Medjuskladisnice VELEP 2 MALOP
      /// </summary>
      R_VMI_2,


      /// <summary>
      /// Cjenik Kupca (2020 - Frag)
      /// </summary>
      R_CJK,

      /// <summary>
      /// Other - Nazivi artikla za kupca
      /// </summary>
      X_NAK,

      /// <summary>
      /// Ugovor PCTOGO
      /// </summary>
      R_UGO_PTG,

      /// <summary>
      /// Aneks PCTOGO
      /// </summary>
      R_ANU_PTG,
     
      /// <summary>
      /// SubModulGrupa11 Zastitari
      /// </summary>
      GROUP11,

      /// <summary>
      /// SubModulGrupa12 PCTOGO
      /// </summary>
      GROUP12,

      /// <summary>
      /// Krovni ugovor PCTOGO
      /// </summary>
      R_KUG_PTG,

      /// <summary>
      /// Dodatak PCTOGO
      /// </summary>
      R_DOD_PTG,

      /// <summary>
      /// Dodatak PCTOGO
      /// </summary>
      R_PR_DOD_PTG,

      /// <summary>
      /// Korekcija Otplatnog Plana PCTOGO - preseljeno u Mixer
      /// </summary>
      R_KOP_PTG, // !!! PUSE !!! NE KORISTITI 

      /// <summary>
      /// A1 Krovni ugovor PCTOGO
      /// </summary>
      R_A1_KUG_PTG,

      /// <summary>
      /// A1 Aneks PCTOGO
      /// </summary>
      R_A1_ANU_PTG,

      R_ZAH_SVD,

      /// <summary>
      /// Korekcija Otplatnog Plana PCTOGO
      /// /// </summary>
      X_KOP,

      /// <summary>
      /// Fakturiranje PCTOGO
      /// </summary>
      R_FUG_PTG,

      /// <summary>
      /// KUPDOB's contacts
      /// </summary>
      X_KDC,

      LsKDC,

      /// <summary>
      /// Povrat PCTOGO
      /// </summary>
      R_PVR_PTG,

      /// <summary>
      /// Modifikacije računala PCTOGO
      /// </summary>
      R_MOD_PTG,

      /// <summary>
      /// Lista rtrano - serNo
      /// </summary>
      LsRTO,

      /// <summary>
      /// Primka PCTOGO
      /// </summary>
      R_PRI_PTG,

      /// <summary>
      /// Izdatnica PCTOGO
      /// </summary>
      R_IZD_PTG,

      /// <summary>
      /// Medjuskladisnica PCTOGO
      /// </summary>
      R_MSI_PTG,

      /// <summary>
      /// Pocetno stanje PCTOGO
      /// </summary>
      R_PST_PTG,

      /// <summary>
      /// Obracun palce od 1.1.2024.
      /// </summary>
      PLA_2024,

      /// <summary>
      /// Ponuda proforma Veleprodajna by MPC
      /// </summary>
      R_PON_MPC,

      /// <summary>
      /// Izlazni racun Veleprodajni + Izdatnica by MPC
      /// </summary>
      R_IRA_MPC,

      /// <summary>
      /// SubModulGrupa13 TETRAGRAM
      /// </summary>
      GROUP13,

      /// <summary>
      /// Primka za otkup od fiz. osobe (Tetragram)
      /// </summary>
      R_PRI_POT,

      /// <summary>
      /// Primka u Veleprodaju NE Pdv - Posudba
      /// </summary>
      R_POU,

      /// <summary>
      /// Izdatnica iz Veleprodaje NE Pdv - ProdCij - Povrat Posudbe
      /// </summary>
      R_POI,

      /// <summary>
      /// Blagajnicka uplatnica - Multi + Automatska
      /// </summary>
      R_ABU,

      /// <summary>
      /// Blagajnicka isplatnica - Multi + Automatska
      /// </summary>
      R_ABI,

      /// <summary>
      /// Izlazni racun Veleprodajni + Izdatnica
      /// </summary>
      R_IRA_2,

      /// <summary>
      /// Izdatnica Veleprodajna by MPC
      /// </summary>
      R_IZD_MPC,

      /// <summary>
      /// Primka PCTOGO
      /// </summary>
      R_URA_PTG,

      /// <summary>
      /// PCK_info PCTOGO
      /// </summary>
      R_PCKinf_PTG,

      /// <summary>
      /// Izlazni racun Veleprodajni + Izdatnica - PCTOGO
      /// </summary>
      R_IRA_PTG,

      /// <summary>
      /// Zamjena - međuskladišnica - PCTOGO
      /// </summary>
      R_ZIZ_PTG,

      /// <summary>
      /// Povrat Djelomični PCTOGO
      /// </summary>
      R_PVD_PTG,

      /// <summary>
      /// Ponuda proforma Veleprodajna by MPC
      /// </summary>
      R_OPN_MPC,

      /// <summary>
      /// SubModulGrupa14 VIPER - TEST
      /// </summary>
      GROUP14,

      /// <summary>
      /// Fiskalizacja F2 - Izlaz
      /// </summary>
      R_F2I,

      /// <summary>
      /// Fiskalizacja F2 - Ulaz
      /// </summary>
      R_F2U,

      /// <summary>
      /// ZAR - Zastupnicki Racun Veleprodajna by MPC-RUC
      /// </summary>
      R_ZAR,

      /// <summary>
      /// Fiskalizacja F2 - Naplata externa
      /// </summary>
      R_F2N,

      /// <summary>
      /// Ledinek
      /// </summary>
      R_LED,

      FORBIDDEN
   };

   // Smjernice: de, ne kompliciraj nago ovo postavi jednostavno (malo sire nego u Offix-u) 
   //            pa ako se jednom pojavi potreba za razradjenijim (netko to plati :-))) ... 
   public enum VvPrivilegeScope
   {
      CIJELI_PROGRAM = 0,

      //================= 
      KOMPLET_RA_SK = 10,

      IZVJESTAJI_RA_SK = 11,
      SIF_ARTIKL_RA_SK = 12,
      SVI_DOKUM_RA_SK = 13,
      JEDAN_DOKUM_RA_SK = 14,

      //================= 
      KOMPLET_GK_SK = 20,

      IZVJESTAJI_GK_SK = 21,
      SIF_KONTA_GK_SK = 22,
      SVI_DOKUM_GK_SK = 23,
      JEDAN_DOKUM_GK_SK = 24,

   }

   // Smjernice: de, ne kompliciraj nago ovo postavi jednostavno (malo sire nego u Offix-u) 
   //            pa ako se jednom pojavi potreba za razradjenijim (netko to plati :-))) ... 
   public enum VvPrivilegeType
   {
      NEOGRANIČENO,
      SAMO_GLEDA,
      SAMO_DODAJE,
   }

   public enum FIZ_FilterStyle { Ftrans, Nalog, Kplan };
   public enum AIZ_FilterStyle { Atrans, Amort, Osred };
   public enum PIZ_FilterStyle { Ptrans, Placa, Person };
   public enum RIZ_FilterStyle { Rtrans, Faktur, Artikl, Ftrans };
   public enum MIX_FilterStyle { Virman, PutNal, Zahtjev };

   public enum VvReportEnum
   {
      FIZ_KPlan,
      FIZ_Dnevnik,
      FIZ_Dnevnik_Exp,
      FIZ_RekapNal,
      FIZ_BilancaS,
      FIZ_BilancaU,
      FIZ_BilancaSubKlas,
      FIZ_KKonta,
      FIZ_KKonta_ALL,
      FIZ_KKontaGroup_ALL,
      FIZ_KKontaGroup_ALL_Exp,
      FIZ_AnaSC,
      FIZ_AnaSCL,
      FIZ_SinSC,
      FIZ_OTS,
      FIZ_OTS_Kum,
      FIZ_AnaMT,
      FIZ_SinMT,
      FIZ_KPI,
      FIZ_KPI_orig,
      FIZ_PPI,
      FIZ_NTR_BilancaMP,
      FIZ_NTR_TSIPod,
      FIZ_NTR_RDiGMP,
      FIZ_NTR_ObrPD,
      FIZ_KnjigaURA,  // 18.02.2016. P-PPI tj. PPI2
      FIZ_KplanExport,
      FIZ_BilancaUNeprof,
      FIZ_BilancaSNeprof,
      FIZ_PlanPG_Plan1,
      FIZ_ThePln_Reali,
      FIZ_ThePln_Njv_Reali,
      FIZ_ThePln_Money,
      FIZ_Pl1_ThePln_Reali,
      FIZ_Plan_PBN,
      FIZ_APT_K1,

      KIZ_Lista,
      KIZ_Kontakti,
      KIZ_Faktur,
      KIZ_GrupTip,
      KIZ_Rabat,
      KIZ_Ziro,
      KIZ_Sifre,
      KIZ_VizitKarta,
      KIZ_VizitKaProsirena,
      KIZ_PartnerExport,

      PRIZ_Lista,
      PRIZ_Kontakti,
      PRIZ_Faktur,
      PRIZ_VizitKarta,
      PRIZ_VizitKaProsirena,

      ZNP_Pogled,

      AIZ_Dnevnik,
      AIZ_RekapDok,
      AIZ_RekapDok_DD,
      AIZ_Osred,
      AIZ_Amort,
      AIZ_PopisDI,
      AIZ_PopisDI_DD,
      AIZ_Old_PopisDI_DD,
      AIZ_InvList,
      AIZ_InvDiff,
      AIZ_ObrazacDI,


      PRZ_PrmODJ,
      PRZ_PrmODJ2,
      PRZ_PrmJIL,
      PRZ_PrmJIL2,

      OAI_OperacA,
      OAI_OperacB,
      OAI_Anestez,
      OAI_OperacA_D,
      OAI_OperacB_D,
      OAI_Anestez_D,
      OAI_Operant,
      OAI_Zahvat,

      DIZ_Opomene,
      DIZ_Opomena1,
      DIZ_Opomena2,
      DIZ_Opomena1VA,
      DIZ_Opomena2VA,
      DIZ_ForExport1,
      DIZ_ForExport2,
      DIZ_ForExportOvr,
      DIZ_CheckList1,
      DIZ_CheckList2,
      DIZ_CheckListOvr,
      DIZ_Rekapitulacija1,
      DIZ_Rekapitulacija2,
      DIZ_OtvoreneStavke1,
      DIZ_OvrhaFizicke,
      DIZ_OvrhaPravne,
      DIZ_PromjenaStatusa,
      DIZ_DebitList,
      DIZ_HZMO1,
      DIZ_ListaHZMO1,
      DIZ_RacunVip,
      DIZ_PozivNaPlacanje,
      DIZ_UvjerenjeMup,
      DIZ_DebitListOvr,
      DIZ_SpecifikacijaOvr,
      DIZ_ExportLista,
      DIZ_OtvoreneStavke2,
      DIZ_OtvoreneStavkeOvr,
      DIZ_OvrhaFizicke_New,
      DIZ_OvrhaPravne_New,
      DIZ_ExportBanke,
      DIZ_OvrhaFizicke3,
      DIZ_OvrhaPravne3,
      DIZ_SpecOvrMaleW,
      DIZ_ExcelAdActaObr,
      DIZ_ExportOvrProsireno,
      DIZ_ExportAll,
      DIZ_OvrhaOpca,
      DIZ_ExProvFina,
      DIZ_ExPrivPov,
      DIZ_ExPismNam,
      DIZ_ExIzvjVIP,
      DIZ_ExRedNapl,
      DIZ_OvrhaPravnaVA,
      DIZ_OvrhaFizickaVA,
      DIZ_ObracunKamata,
      DIZ_AaPredmetiVIP,
      DIZ_ObracunDuga_OTS,
      DIZ_SpecifikacijaUplVip,
      DIZ_OvrhaPravnaVA_JB,
      DIZ_OvrhaPravnaVA_1713,   // vrijedi od 01.07.2013. ili 30.06.2013.
      DIZ_OvrhaPravnaVA_32014, // vrijedi od 03 mj 2014
      DIZ_OvrhaFizickaVA_JB,
      DIZ_ExcelNoviPO,        // SpecOvrhaMaleVrijed2013
      DIZ_ExcelNasljed,       // Opomena3Hub
      DIZ_ExcelParnice,       // parnicna kartica 
      DIZ_ExcelOvrhe2013,     // jb
      DIZ_ExcelZatvoreniPred,
      DIZ_Backlog_Slucajevi,
      DIZ_Backlog_Troskovi,
      DIZ_Backlog_SlucajeviR,
      DIZ_Backlog_TroskoviR,
      DIZ_Backlog_SlucajeviO,
      DIZ_Backlog_TroskoviO,
      DIZ_OvrPravVA_01092014,  // vrijedi od 01092014
      DIZ_OvrFiziVA_01092014,  // vrijedi od 01092014

      DIZ_OvrPravVA_072016,
      DIZ_OvrFiziVA_072016,

      PIZ_RekapPlaca,
      PIZ_RekapPerson,
      PIZ_ObracunPlace,
      PIZ_ObracunDrDoh,
      PIZ_PotvrdaDrDoh,
      PIZ_IsplatnaLista,
      PIZ_IsplListaDrDoh,
      PIZ_ListaRadnika,
      PIZ_ObrazacRSm,
      PIZ_ObrazacDNR,
      PIZ_ObrazacID,
      PIZ_ObrazacID1,
      PIZ_ObrazacIDD,
      PIZ_ObrazacIDD1,
      PIZ_ObrazacIP,
      PIZ_KonacniObrPor,
      PIZ_ObrazacIPP,
      PIZ_VirmaniPlaca,
      PIZ_ZbrojniNalog,
      PIZ_ListaZaBanku,
      PIZ_RekapBruta,
      PIZ_ListaObustava,
      PIZ_JOPPD,
      PIZ_PotpisnaLista,
      PIZ_IsplatListOP,
      PIZ_ObrazacIP1,
      PIZ_ObrazacNP1,
      PIZ_RAD1,
      PIZ_RAD1_G,
      PIZ_RekapNeoporPri,
      PIZ_ObrazacIP1_v2,
      PIZ_APT_K2,
      PIZ_PersonMatPodaci,

      RIZ_RekapFaktur,
      RIZ_RekapRN,
      RIZ_Rekap_TH_DjelatRabat,
      RIZ_RekapIRMasBlagIzvj,
      RIZ_RUC_Ira,
      RIZ_RUC_Ira_Rtrans,
      RIZ_RekapPartnera,
      RIZ_RekapArtikla,
      RIZ_PrometArtikla,
      RIZ_LagerLista,
      RIZ_StanjeSklad_A,
      RIZ_StanjeSklad_AP,
      RIZ_StanjeSklad_B,
      RIZ_SkladBilten,
      RIZ_PDV_URA,
      RIZ_PDV_IRA,
      RIZ_PDV_PDV,
      RIZ_PDV_PDVk,

      RIZ_DUGOVANJA,
      RIZ_DUGOVANJA_S,
      RIZ_DUGOVANJA_Kum,
      RIZ_POTRAZIVANJA,
      RIZ_POTRAZIVANJA_S,
      RIZ_POTRAZIVANJA_Kum,
      RIZ_Kompenzacija,
      RIZ_DobavDospjeca,
      RIZ_KupciDospjeca,
      RIZ_TopDugovanja,
      RIZ_TopPotraziv,
      RIZ_OPZSTAT1,
      RIZ_OPZSTAT1_S,

      RIZ_BlgDnevnik,
      RIZ_StanjeSklad_Kol,
      RIZ_IFAraster,
      RIZ_IFArasterB,
      RIZ_KPM,
      RIZ_Knjizenja,
      RIZ_RekapFaktur_S,
      RIZ_Rekap_TH_DjelatRabat_S,
      RIZ_RekapRN_S,
      RIZ_BlgDnevnik_S,
      RIZ_RUC_Ira_S,
      RIZ_PDV_URA_2012,
      RIZ_PDV_IRA_2012,
      RIZ_PDV_PDV_2012,
      RIZ_PDV_PDVk_2012,
      RIZ_PrometArtikla_S,
      RIZ_LagerLista_S,
      RIZ_StanjeSklad_A_S,
      RIZ_StanjeSklad_AP_S,
      RIZ_StanjeSklad_B_S,
      RIZ_Komparacija,
      RIZ_PrometRazdoblja,
      RIZ_PrometRazdoblja_S,
      RIZ_KretanjeSklad_S,
      RIZ_StanjeSkladPoPRJ,
      RIZ_StanjeReversa,
      RIZ_InventurnaLista,
      RIZ_InventurneRazlike,
      RIZ_InventurnoStanje,
      RIZ_InventurneRazlike_S,
      RIZ_InventurnoStanje_S,
      RIZ_KarticaKupca,
      RIZ_KarticaDobav,
      RIZ_KarticaKupca_S,
      RIZ_KarticaDobav_S,
      RIZ_ObrKamataKupac,
      RIZ_ObrKamataDobav,
      RIZ_ObrKamataKupac_S,
      RIZ_ObrKamataDobav_S,
      RIZ_PPMV_Prilog9,
      RIZ_PDV_URA_EU,
      RIZ_PDV_IRA_EU,
      RIZ_PDV_PDV_EU,
      RIZ_ObrazacZP_EU,
      RIZ_ObrazacPDVS_EU,
      RIZ_ObrazacZP_EU_A,
      RIZ_ObrazacPDVS_EU_A,
      RIZ_ProdajaPoDobav_A,
      RIZ_ProdajaPoDobav_S,
      RIZ_Rekap_FISK_Faktur,
      RIZ_Rekap_MER_STATUS,
      RIZ_PromStSkladDobav_A,
      RIZ_PromStSkladDobav_S,
      RIZ_ProdajaPoDobav_B_A,
      RIZ_ProdajaPoDobav_B_S,
      RIZ_Rekap_PIX,
      RIZ_PrometArtikla_OP_A,
      RIZ_PrometArtikla_OP_S,
      RIZ_LagerLista_OP_A,
      RIZ_LagerLista_OP_S,
      RIZ_RUC_Provizija_A,
      RIZ_RUC_Provizija_S,
      RIZ_Rekap_PNP_Rtrans,
      RIZ_PDV_PDVk_2013,
      RIZ_LagerLista_OPSkl_A,
      RIZ_LagerLista_OPSkl_S,
      RIZ_ObrazacPPO,
      RIZ_StanjeSklad_AMB,
      RIZ_StanjeSklad_AMB_S,
      RIZ_StanjeSklad_OP_A,
      RIZ_StanjeSklad_OP_S,
      RIZ_PrometRazdoblja_OP_A,
      RIZ_PrometRazdoblja_OP_S,
      RIZ_Rekap_IRMvsFtrans,
      RIZ_Rekap_IRAvsFtrans,
      RIZ_Rekap_RNM,
      RIZ_Rekap_RNM_S,
      RIZ_RNM_Proizvodi,
      RIZ_RNM_Proizvodi_S,
      RIZ_Rekap_RNZ,
      RIZ_ObrazacPDV_MI,
      RIZ_ObrazacPDV_MU,
      RIZ_IntrastatUlaz,
      RIZ_IntrastatIzlaz,
      RIZ_LagerLista_Kol_A,
      RIZ_RekapFakturWKupdob,

      SVD_FinIzlazByAgr,
      SVD_FinIzlazByAgr_S,
      SVD_FinIzlazBySkl,
      SVD_FinIzlazBySkl_S,
      SVD_TopListaPartnera,
      SVD_PrmStSkl_ProsWeek,
      SVD_PlanRealizUGO,
      SVD_PlanRealizUGO_S,
      SVD_ArtikliKlinInv_Exp,
      SVD_URA_4Knjigovod,
      SVD_URA_4Knjigovod_S,
      SVD_NEW_4KNJ_S,

      RIZ_TemboWebShopExport,
      RIZ_Jeftinije_hr_Export,
      SVD_HALMED_Potrosnja,
      SVD_HALMED_Provjera ,
      SVD_PrmArt4Nabava,

      XIZ_RekapPutNalLoko,
      XIZ_RekapZahtjev,
      XIZ_RekapSMD,
      XIZ_RekapEVD,
      XIZ_RekapEVN,
      XIZ_RVR,
      XIZ_IRV,
      XIZ_PriBor,
      XIZ_MVR,
      XIZ_GST,
      XIZ_GST_STR,
      XIZ_TZNoc,
      XIZ_TZ_DolN,
      XIZ_UrdzZap,
      XIZ_RegUgv,
      XIZ_ZUG,
      XIZ_ZLJ,
      XIZ_ZPG,
      XIZ_ZOBJ,

      GROUP1,
      GROUP2,
      GROUP3,
      GROUP4,
      GROUP5,
      GROUP6,
      GROUP7,
      GROUP8,
      GROUP9,

      ZXC_UNDEFINED
   };

   public enum vvMenuStyleEnum
   {
      MenuItem_Only,                      // cisti ToolStripMenuItem
      MenuItemAndButton,                  //       ToolStripMenuItem + ToolStripButton
      Separator_onlyMenuItem,             // cisti ToolStripSeparator
      Separator_onlyButton,               // samo Button ToolStripSeparator
      Separator_MenuItemAndButton,        //       ToolStripSeparator + btnSeparator
      Checked_MenuItem,                   // cisti ToolStripMenuItem Checked
      DropDownMenuItem_VisibleToolStrip,  // cisti ToolStripMenuItem_DropDown za view Toolbars
      DropDown_IconSize,                  // icone
      MenuItem_SplitBtnModul,             //      ToolStripMenuItem + ToolStripSplitButton za modulPanel
      MenuItem_RightModulPanel,           // ToolStripMenuItem desni modulPanel
      MenuItem_LeftModulPanel,            // ToolStripMenuItem lijevi modulPanel
      MenuItem_UpDownTabPage,             // ToolStripMenuItem gore dolje TabPage
      MenuItem_RadnaOkolina,              // ToolStripMenuItem Radna Okolina
      DropDown_ZoomReport,
      MenuItemAndButton_Filter,
      MenuItemAndButtonReport,             // za toolstrip_report
      DropDown_ScalingFont
   };

   public enum vvMenuEnabled { openedTabPage, closedTabPage, nevazno };

   public enum WriteMode { None, Add, Edit, Delete };

   public enum ImportMode { None, ADDREC, RWTREC, ADDREC_4_DOPUNA, RWTREC_4_OPOM2, RWTREC_4_OVRHA, ANALYSE_Obrok_Only, RWTREC_BANKA_SENTDATE, RWTREC_BANKA_FEEDBACK, RWTREC_caseID };

   public enum PrivilegedAction { ADDREC, RWTREC, DELREC, ENTER_SUBMODUL };

   public enum ZaUpis { Zatvoreno, Otvoreno };

   public enum ReportMode { Fresh, Working, Done, Canceling };

   public enum ParentControlKind
   {
      VvRecordUC,
      VvReportUC,
      VvOtherUC,
      VvFindDialog,
      TamponPanel_HeaderLeft,
      TamponPanel_HeaderPrjkt,
      TamponPanel_Footer,
      Login_Form,
      VvDialog
   };

   public enum JAM_CharEdits { Unfiltered, DigitsOnly, LettersOnly, NumericOnly, AlphaNumericOnly, RegularExpression }

   public enum AutoCompleteRestrictor
   {
      No_Restrictions,

      KPL_Analitika_Only,

      KID_Centrala_Only,
      KID_NonCentrala_Only,
      KID_Mtros_Only,
      KID_NonMtros_Only,
      KID_Kupac_Only,
      KID_Dobav_Only,
      KID_Banka_Only,
      KID_Komisija_Only,

      FAK_ExactTT_Only,

      ART_MOT_VOZILO_Only,
      ART_NabOrProdKAT_Only,

      ART_NonRashod_Only,

      PER_RR_Only, // Placa 
      PER_PP_Only, // Poduzetnicka placa 
      PER_AHiAU_Only, // Autorski honorar 
      PER_NO_Only,  // Nadzorni 
      PER_UD_Only // Ugovor o djelu 
   }

   public enum SaldoOrDugOrPot
   {
      SALDO,
      DUG,
      POT,
      NULL
   }

   public enum AmortRazdoblje
   {
      GODINA,
      KVARTAL,
      MJESEC,
      POLUGOD,
      //NOW
   }

   public enum JeliJeTakav
   {
      JE_TAKAV,
      NIJE_TAKAV,
      NEBITNO
   }

   public enum UparenostKind
   {
      DA_uparen,
      NE_uparen,
      //Lose_uparen,
      NEBITNO
   }

   public enum GotMoneyKind
   {
      Kune,
      Euri,
      Kune_AND_Euri
   }

   public enum ErrorStatus
   {
      NEBITNO = 0,
      NO_ERROR = 1,
      IN_ERROR = 2
   }

   /// <summary>
   /// Ovo koristi kada trebas stanje RadioButton-a spremiti u DataLayer kao ushort
   /// </summary>
   public enum RadioButtonChecked
   {
      Prvi,
      Drugi,
      Treci,
      Cetvrti,
      Peti
   }

   public enum Spol
   {
      NEPOZNATO,
      MUSKO,
      ZENSKO,
   }

   public enum VvColSetVisible
   {
      AllVisible,
      RedVisible,
      BlueVisible
   }

   public enum MailStatus
   {
      INBOX,
      OUTBOX,
      SENT,
   }

   public enum VirmanEnum
   {
      POR, PRI, MIO1, MIO2, ZDR, ZOR, ZPI, ZAP, ZPP, NET, KRP, PRE, MIO1NA, MIO2NA,/*05.03.2020.*/ NP63, NP65, NP71, /*25.01.2022*/NP17, NP18, NP21, NP22, NP26, NP60, /*29.01.2024.*/ NP73
   }

   public enum VirmanBtchBookgKind
   {
      ALL, BtchBookg_ONLY, NON_BtchBookg_ONLY
   }

   public enum VektorSiteEnum
   {
      UNKNOWN, VIPER, ROZEL, ROZEL1, BOBESIC, KROVAL, LEUT, SENSO, ZAGRIA, INTRADE, MANDAR, FRIGOT, LAJNUS, LJUBEN, TEM95, DUCATI,
      VIPER1, ROZ001, PPUKAN, CIRCUL, VERIDI, LIKUM, TEMBO, JOSAVC, HZTK, JORDAN, VINAAA, AGSJAJ, ZARPTI, TEXTHO,
      TURZML, KEREMP, SPSIST, QQTEXT, METFLX, BRADA, SVDUH, PCTOGO, TGPROJ, TGPLEM, ARTEON
   }

   public enum BankaEnum
   {
      NULL, HNB, ZABA, PBZ, REIF, HYPO, HPB, ERSTE, SPLIT, VOLK
   }

   public enum TipTecajaEnum
   {
      KUPOVNI,
      SREDNJI,
      PRODAJNI
   }

   public enum ValutaNameEnum
   {
      EMPTY,
      AUD = 036,
      CAD = 124,
      CZK = 203,
      DKK = 208,
      HUF = 348,
      JPY = 392,
      NOK = 578,
      SEK = 752,
      CHF = 756,
      GBP = 826,
      USD = 840,
      EUR = 978,
      PLN = 985,
      BAM = 977,
      BYN = 933,
      HRK = 191
   }

   public enum ArtiklVpc1Policy
   {
      ZADANO,
      MARZA
   }

   public enum PdvKolTipEnum
   {
      NIJE, MOZE, NEMOZE, PROLAZ, KOL07, KOL08, KOL09, KOL10, KOL11, KOL12, KOL13, KOL14, KOL15, KOL16,
      UMJETN, // umjetnina, PDV na RUC only 
      AVANS_STORNO,
      GlassOnIRM
   }

   public enum PdvKnjigaEnum
   {
      /*0*/
      NIJEDNA, /*1*/ REDOVNA, /*2*/ PREDUJAM, /*3*/ UVOZ_ROB, /*4*/ UVOZ_USL, /*5*/ PDV_RUC
   }

   //public enum PdvUvozEnum
   //{
   //  NIJEDNO, ROBA, USLUGA
   //}

   public enum PdvR12Enum
   {
      NIJEDNO, R1, R2
   }

   public enum JezikEnum
   {
      HRV, ENG, NJE, TAL, SPA, FRA, C, D
   }

   public enum TtProposeCijenaKindEnum
   {
      Propose_NONE, Propose_CJENIK, Propose_PrNabCij, Propose_PrOfPrNabCij_SumeSastojaka
   }

   public enum OtsKindEnum
   {
      NIJEDNO, OTVARANJE, ZATVARANJE
   }

   public enum Faktur2NalogSetEnum
   {
      NONE, ULAZNI_VP, ULAZNI_MP, IZLAZ_RN_VP, IZLAZ_RN_MP, VMI, BLAGAJNA, PROIZ_IZLAZ, ULAZ_INT, OneExactTT, IZLAZ_SK_VP, IZLAZ_SK_MP, IRA_RealizOnly
   }

   public enum Faktur2NalogTimeRuleEnum
   {
      DoIt_NEVER, DoIt_ForDokDateInterval, DoIt_For_AddTS_NotToday, DoIt_For_AddTS_OneHourOld, DoIt_For_AnyUndone
   }

   public enum FM_OR_Enum
   {
      NONE, OPEN_OR, CLOSE_OR
   }

   public enum ShouldFak2NalEnum
   {
      Knjizi_NORMALNO, Knjizi_NISTA, Knjizi_SamoPDV_i_Carinu
   }

   public enum ProjektPaySetEnum
   {
      ToPayA, ToPayB, ToPayC, PidSume, PidSRent, PidTurst, PidDobit, PidKmDopr, PidKmClan, PidMO1, PidMO2, PidZdr, PidZor
   }

   /// <summary>
   /// Trouble Maker Status regarding MINUS
   /// </summary>
   public enum MinusTrouble
   {
      NO_TROUBLE, IN_TROUBLE, REPAIRED
   }

   public enum MalopCalcKind
   {
      By_MARZA, By_VPC, By_MPC
   }

   public enum PdvRTipEnum
   {
      PODUZECE_R1, OBRT_R1, OBRT_R2, OBRT_NOT_PDV, NOT_IN_PDV, POD_PO_NAPL, FIZICKA_OSOBA
   }

   public enum MinusPolicy
   {
      ALLOW_ALL, DENY_ALL, DENY_VEL_ALLOW_MAL, ALOW_ALL_NO_MSG
   }

   public enum KomisijaKindEnum
   {
      NIJE, VELEPRODAJNA, MALOPRODAJNA, TMB2SKL
   }

   public enum Rtrano_CalcVolumenEnum
   {
      TRUPAC, LETVA, FUSE
   }

   public enum EuroNormaEnum
   {
      NIJEDNA = 0, EuroI = 1, EuroII = 2, EuroIII = 3, EuroIV = 4, EuroV = 5, EuroVI = 6
   }

   public enum ShowArtiklInfo_CijenaKind_Enum
   {
      ulazProsCij,
      izlazProsCij,
      thisKupProsCij,
      ulazMinCij,
      izlazMinCij,
      thisKupMinCij,
      ulazMaxCij,
      izlazMaxCij,
      thisKupMaxCij,
      ulazLastCij,
      izlazLastCij,
      thisKupLastCij
   }

   public enum ZNP_Kind
   {
      Classic = 0,
      Placa_Specifikacija = 1,
      Placa_ZNP = 2,
      SEPA_Placa = 3
   }

   public enum MySqlCheck_Kind
   {
      A_FaktEx_without_Faktur = 0,
      B_Faktur_without_KupdobCD_OR_FaktEx = 1,
      C_Faktur_withMore_FaktEx = 2,
      D_Rtrans_without_Faktur = 3,
      E_Rtrans_duplicates = 4,
      F_Faktur_without_vvLogLAN = 5,
      G_Rtrans_without_TwinRtr = 6,
      H_vvLogERR_xy_exists = 7,
      I_Rtrans_with_wrong_TwinID = 8,
      J_FakturSUM_vs_RtransSUM = 9,
      K_TwinRtransVsArtstatCij = 10,
      L_NotFiskalizedIRMs = 11,
      M_MsiMsu_Roundtrip = 12,
      N_RnuPip_MissingData = 13,
   }

   public enum RptExportKind
   {
      PDF = 0,
      MSOffice = 1,
      XML = 2
   }

   #region 2013 EU PDV NEWS

   public enum PdvGEOkindEnum
   {
      HR, EU, WORLD, TP /*Tuzemni Prijenos porezne obveze*/ , BS /*BezSjedista*/
   }

   public enum PdvZPkindEnum
   {
      NoZP, KOL_11, KOL_12, KOL_13,
      SVD_LJEK, SVD_POTR
   }

   #endregion 2013 EU PDV NEWS

   //   public enum POSprinterKind
   //   {
   //      CLASSIC,
   //      BIXOLON01,
   //      BIXOLON02,
   //      EPSON01,
   //      EPSON02,
   //      POS80
   //   }

   public enum PlanKindEnum
   {
      NO_PLAN,
      PlnBy_POZICIJA,
      PlnBy_MTROS,
      PlnBy_FOND
   }

   //public enum PeriodLockKind
   //{
   //   NO_LOCK,
   //   On20LockFromPrevMonth,
   // //LK_fromPrevKvart
   //}

   public enum PdfSignatureKind
   {
      NO_SIGNATURE = 0,
      SIGN_UNVISIBLE = 1,
      SIGN_VISIBLE = 2
   }

   public enum MailClientKind
   {
      NO_Outlook_VektorDialog = 0,
      OutlookAccount_OutlookDialog = 1,
      OutlookAccount_VektorDialog = 2
   }

   public enum VvUBL_PolsProcEnum
   {
      P00 = 0, // Nedefinirano 

      P01 =  1, // P1  – Fakturiranje isporuka dobara i usluga preko narudžbi na temelju ugovora
      P02 =  2, // P2  - Periodično fakturiranje isporuka na temelju ugovora
      P03 =  3, // P3  - Fakturiranje isporuka preko nepredviđene narudžbe
      P04 =  4, // P4  - Plaćanje predujma (avansno plaćanje)
      P05 =  5, // P5  - Plaćanje na licu mjesta
      P06 =  6, // P6  - Plaćanje prije isporuke na temelju narudžbe
      P07 =  7, // P7  - Računi s referencom na otpremnicu
      P08 =  8, // P8  - Računi s referencom na otpremnicu i primku
      P09 =  9, // P9  - Odobrenje ili negativno fakturiranje
      P10 = 10, // P10 - Korektivno fakturiranje
      P11 = 11, // P11 - Parcijalno i završno fakturiranje
      P12 = 12, // P12 - Samoizdavanje računa
      P99 = 99, // P99:Oznaka kupca - Proces definiran od strane kupca

   }

   public enum VvmBoxKind
   {
      aim_emsg_List,
      BarCodeInfo  ,
      RobnaKartica ,
      F2_SEND_candidates,
      F2_IMPORT_candidates,
      F2_MAP_candidates,
      F2_webApiResults
   }

   #region PTG Enums

   public enum PCK_Info_Kind
   {
      OvaBazaOnly        = 0,
      SveBazeOnly        = 1,
      SveBazeIkomponente = 2,
      KomponenteOnly     = 3,
   }

   public enum PTG_DanFakturiranjaEnum
   {
      PrviDanMjeseca  ,
      ZadnjiDanMjeseca,
      NaDanUgovora
   }

   public enum PTG_MjestoIsporukeEnum
   {
      Korisnik,
      KorisnikOverseas,
      PcToGo
   }

   public enum PTG_VrstaNajmaEnum
   {
      Mjesecni,
      Dnevni
   }

   public enum PTG_NajamNaRokEnum
   {
      DN1 /*dana1do7    */,
      DN2 /*dana8do14   */,
      DN3 /*dana15do31  */,
      MN1 /*mjesec1do3  */,
      MN2 /*mjesec4do6  */,
      MN3 /*mjesec7do12 */,
      MN4 /*mjesec13do24*/, 
      MN5 /*mjesec36    */,
      MN6 /*mjesec48    */,
      EMPTY
    }

   #endregion PTG Enums

   public enum MoneyConversionKind
   {
      automatski = 0,
      mnozenje = 1,
      dijeljenje = 2,
   }

   public enum F2_Provider_enum
   {
      UNKNOWN,
      MER,
      PND
   }

   public enum F2_RolaKind
   {
      NEMA_F2,                           // TZGML, HZTK                                                         
      VlastitoKnjigovodstvo_F2_ALL,      // CLASSIC                                                             
      VlastitoKnjigovodstvo_F2_FUR_Only, // TEXTILEHOUSE, ŽAR PTICA                                             
      KlijentServisa_TipA,               // NEMA importa Izlaznih racuna ... Veleform  .                        
      KlijentServisa_TipB,               // IMA  importa Izlaznih racuna ... Tetragram .                        
      KlijentServisa_TipC,               // IMA F2 ali NEMA uopce veze na PT - fin. posrednika ... 'SensoMicro' 
      IMA_B2B_Virman_NEMA_F2             // Vlatko                                                              
   }

   //public enum AMSstatus
   //{
   //   NEPOZNAT    = 0,
   //   U_AMSu_JE   = 1,
   //   NIJE_U_AMSu = 2
   //}

   public enum F2_WebApi
   {
      UnknownApi,
      SEND,
      eIzvj,
      OutboxTRNstatus,
      OutboxDPSstatus,
      OutboxTRNstatusList,
      OutboxTRNstatusListAsKnjigServis,
      OutboxDPSstatusList,
      InboxTRNstatusList,
      InboxDPSstatusList,
      FISK_singleStatus,
      FISKstatusOutbox,
      FISKstatusInbox,
      REJECTstatus,
      MAPstatus,
      MAPaction,
      MAPaction_WO_eID,
      RECEIVEdocument,
      PING,
      CheckAMS,

      HDD_OutboxListAsKnjigServis,
      HDD_InboxListAsKnjigServis,
   }

   public enum F012kind
   {
      Nepoznato,
      F1     ,
      F2     ,
    //F2send ,
    //F2eIzvj,
      F0 // NoFX
   }

   public enum F2_R1enum
   {
      Nepoznato,
      B2B,
      B2C
   }

   public enum F2_StatusInAndOutBoxEnum
   {
      //Uspjeh     = 0,
      //Neuspjeh   = 1,
      //Na_cekanju = 2

      Nepoznato  = 0,
      DA_JE      = 1,
      Na_cekanju = 2,
      NE_NIJE    = 3

   }

   public enum F2_CreateFakturKind
   {
      From_WS_MyOwn,
      From_WS_XXX  ,
      From_HDD_XXX ,

   }

   #endregion Enums

   #region aim_emsg

   public static void aim_emsg(string text)
   {
      ZXC.aim_emsg_JOB(MessageBoxIcon.None, text);
   }

   public static void aim_emsg(string format, params object[] args)
   {
      ZXC.aim_emsg_JOB(MessageBoxIcon.None, string.Format(format, args));
   }

   public static void aim_emsg(MessageBoxIcon mbIcon, string format, params object[] args)
   {
      ZXC.aim_emsg_JOB(mbIcon, string.Format(format, args));

   }

   public static void aim_emsg(MessageBoxIcon mbIcon, string text)
   {
      ZXC.aim_emsg_JOB(mbIcon, text);
   }

   public static void aim_emsg_JOB(MessageBoxIcon mbIcon, string _text)
   {
      string text = SanitizeForMessageBox(_text);

      ZXC.aim_log(text);

      if(ZXC.ThisIsSkyLabProject) return;

      MessageBox.Show(text, "", MessageBoxButtons.OK, mbIcon);

    //Aim_emsg_DLG aim_emsg_DLG = new Aim_emsg_DLG(mbIcon, text);
   }

   private static string SanitizeForMessageBox(string s, int max = 8000)
   {
      if(s == null) return string.Empty;
      s = s.Replace("\0", string.Empty)
           .Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n"); // normalize
      s = s.Replace("{", "").Replace("}", ""); // escape for formatters
      if(s.Length > max) s = s.Substring(0, max) + "...";
      return s;
   }
   public static string GetMethodNameDaStack()
   {
      int skipFrames = 0;

      System.Diagnostics.StackFrame sf1 = new System.Diagnostics.StackTrace(++skipFrames, true).GetFrame(0);
      System.Diagnostics.StackFrame sf2 = new System.Diagnostics.StackTrace(++skipFrames, true).GetFrame(0);
      System.Diagnostics.StackFrame sf3 = new System.Diagnostics.StackTrace(++skipFrames, true).GetFrame(0);
      System.Diagnostics.StackFrame sf4 = new System.Diagnostics.StackTrace(++skipFrames, true).GetFrame(0);

      return
      "\tSource location:\t" + sf1.GetFileName() +
      "\n\tLine:\t\t" + sf1.GetFileLineNumber() +
      "\n\tType:\t\t" + sf1.GetMethod().DeclaringType +
      "\n\tMethod:\t\t" + sf1.GetMethod().Name + "\n\n" +

      "\tSource location:\t" + sf2.GetFileName() +
      "\n\tLine:\t\t" + sf2.GetFileLineNumber() +
      "\n\tType:\t\t" + sf2.GetMethod().DeclaringType +
      "\n\tMethod:\t\t" + sf2.GetMethod().Name + "\n\n" +

      "\tSource location:\t" + sf3.GetFileName() +
      "\n\tLine:\t\t" + sf3.GetFileLineNumber() +
      "\n\tType:\t\t" + sf3.GetMethod().DeclaringType +
      "\n\tMethod:\t\t" + sf3.GetMethod().Name + "\n\n" +

      "\tSource location:\t" + sf4.GetFileName() +
      "\n\tLine:\t\t" + sf4.GetFileLineNumber() +
      "\n\tType:\t\t" + sf4.GetMethod().DeclaringType +
      "\n\tMethod:\t\t" + sf4.GetMethod().Name;
   }

   public static string GetMethodNameDaStack_Short()
   {
      int skipFrames = 0;

      System.Diagnostics.StackFrame sf1 = new System.Diagnostics.StackTrace(++skipFrames, true).GetFrame(0);
      System.Diagnostics.StackFrame sf2 = new System.Diagnostics.StackTrace(++skipFrames, true).GetFrame(0);
      System.Diagnostics.StackFrame sf3 = new System.Diagnostics.StackTrace(++skipFrames, true).GetFrame(0);
      System.Diagnostics.StackFrame sf4 = new System.Diagnostics.StackTrace(++skipFrames, true).GetFrame(0);

      return
      "\tMethod:\t" + sf1.GetMethod().Name + "\n" +
      "\tMethod:\t" + sf2.GetMethod().Name + "\n" +
      "\tMethod:\t" + sf3.GetMethod().Name + "\n" +
      "\tMethod:\t" + sf4.GetMethod().Name + "\n";
   }

   public static void aim_emsg_List(string messageListNaslov, List<string> messageList)
   {
      aim_emsg_List(messageListNaslov, messageList, false);
   }

   public static void aim_emsg_List(string messageListNaslov, List<string> messageList, bool smallFont)
   {
      // 01.06.2020: pucamo i u log 
      int count = 0;
      foreach(string msg in messageList) { ZXC.aim_log("{0}. {1}", ++count, msg); }

      // ovo je dodadno tek 03.04.2023.!?                     
      // ali jos nije deployano a tice se Skylaba i Janitora! 
      if(ZXC.ThisIsSkyLabProject) return;

      VvMessageBoxDLG vvMsgDLG = new VvMessageBoxDLG(smallFont, VvmBoxKind.aim_emsg_List);

      // 15.12.2025: 
      for(int i = 0; i < messageList.Count; ++i)
      {
         vvMsgDLG.TextForSupportMailBody += messageList[i] + Environment.NewLine;
      }

      vvMsgDLG.Text = messageListNaslov;

      vvMsgDLG.TheUC.PutDgvFields(messageList);

      vvMsgDLG.ShowDialog();

      // 17.11.2025: 
    //vvMsgDLG.Close();
      vvMsgDLG.Dispose();
   }
   public static void aim_emsg_List(string messageListNaslov, List<string> messageList, bool smallFont, string xmlContentForAttachment)
   {
      aim_emsg_List(messageListNaslov, messageList, smallFont, xmlContentForAttachment, null);
   }
   public static void aim_emsg_List(string messageListNaslov, List<string> messageList, bool smallFont, string xmlContentForAttachment, string fileName)
   {
      // 01.06.2020: pucamo i u log 
      int count = 0;
      foreach(string msg in messageList) { ZXC.aim_log("{0}. {1}", ++count, msg); }

      // ovo je dodadno tek 03.04.2023.!?                     
      // ali jos nije deployano a tice se Skylaba i Janitora! 
      if(ZXC.ThisIsSkyLabProject) return;

      VvMessageBoxDLG vvMsgDLG = new VvMessageBoxDLG(smallFont, VvmBoxKind.aim_emsg_List);

      // 15.12.2025: 
      for(int i = 0; i < messageList.Count; ++i)
      {
         vvMsgDLG.TextForSupportMailBody += messageList[i] + Environment.NewLine;
      }

      // 25.12.2025: Spremi XML kao temp file za attachment
      if(xmlContentForAttachment.NotEmpty())
      {
         try
         {
            // Koristi fileName ako postoji, inače generiraj novi
            string xmlFileName = fileName.NotEmpty()
               ? fileName
               : $"eRacun_ValidationError_{DateTime.Now:yyyyMMdd_HHmmss}.xml";

            string tempXmlPath = System.IO.Path.Combine(
               System.IO.Path.GetTempPath(),
               xmlFileName
            );

            System.IO.File.WriteAllText(tempXmlPath, xmlContentForAttachment, System.Text.Encoding.UTF8);
            vvMsgDLG.AttachmentFilePath = tempXmlPath;
         }
         catch(Exception ex)
         {
            aim_log("Greška kod spremanja XML attachmenta: {0}", ex.Message);
         }
      }

      vvMsgDLG.Text = messageListNaslov;

      vvMsgDLG.TheUC.PutDgvFields(messageList);

      vvMsgDLG.ShowDialog();

      // 17.11.2025: 
      vvMsgDLG.Dispose();
   }
   public static void aim_emsg_VvException(Exception e)
   { 
      VvExceptionDlg exDlg = new VvExceptionDlg(e.Message, e.StackTrace,
         e.InnerException != null ? e.InnerException.Message : "no inner ex", e.InnerException != null ? e.InnerException.StackTrace : "");

      DialogResult dlgResult = exDlg.ShowDialog();

      // 17.11.2025: 
    //exDlg.Close();
      exDlg.Dispose();
   }

   public static string PrettyPrintResponse(string body)
   {
      // Try JSON first
      var pretty = PrettyPrintJson(body);
      if(pretty != body) return pretty;

      // Try XML
      pretty = PrettyPrintXml(body);
      if(pretty != body) return pretty;

      // Fallback: return as-is
      return body;
   }

   public static string PrettyPrintJson(string json)
   {
      try
      {
         var parsedJson = Newtonsoft.Json.Linq.JToken.Parse(json);
         return parsedJson.ToString(Newtonsoft.Json.Formatting.Indented);
      }
      catch
      {
         // Not valid JSON, return as-is
         return json;
      }
   }
   public static string PrettyPrintXml(string xml)
   {
      try
      {
         var doc = new System.Xml.XmlDocument();
         doc.LoadXml(xml);
         using(var stringWriter = new StringWriter())
         using(var xmlTextWriter = new System.Xml.XmlTextWriter(stringWriter) { Formatting = System.Xml.Formatting.Indented })
         {
            doc.WriteContentTo(xmlTextWriter);
            xmlTextWriter.Flush();
            return stringWriter.GetStringBuilder().ToString();
         }
      }
      catch
      {
         // Not valid XML, return as-is
         return xml;
      }
   }

   #endregion aim_emsg

   #region aim_log

   // by Delf 04.05.2015 - aim_log

   public static string aim_log_file_name()
   {
      if(ZXC.utilTS.IsEmpty()) ZXC.utilTS = DateTime.Now;

      // Faza 1a / C4: kroz ZXC.ProjectAndUserDocumentsLocationProvider; fallback na VvForm.
      string baseDir = ZXC.ProjectAndUserDocumentsLocationProvider != null
                     ? ZXC.ProjectAndUserDocumentsLocationProvider(false)
                     : ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false);

      string fName = baseDir + @"\" + ZXC.vv_PRODUCT_Name + "Log_"  + ZXC.utilTS.ToString(ZXC.VvTimeStampFormat4FileName) + @".TXT";

      return (fName);
   }

   private static System.Object lockThis = new System.Object();

   public static void aim_log_core(bool addTimeStamp, bool addNewLine, string format, params object[] args)
   {
      string fName = aim_log_file_name();

      // 03.06.2019: 
      // 04.07.2019: ugasio jer nez zasto sam i palio a radi BUG jer {0} mora ostati {0} 
    //format = format.Replace("\"", "^").Replace("{", "(").Replace("}", ")"); ;
      lock(ZXC.lockThis)
      {
         // System.IO.File.AppendAllText(@"d:\delflog.txt", string.Format(format, args) + (addNewLine ? "" : "\r\n"));  // by Delf 04.05.2015
         System.IO.File.AppendAllText(fName, (addTimeStamp ? DateTime.Now.ToString(@"[HH:mm:ss] ") : "") + string.Format(format, args) + (addNewLine ? Environment.NewLine : ""));  // by Delf 04.05.2015
      }
   }

   public static void aim_log(string format, params object[] args)  // Cijeli red, timestamp + newline
   {
      ZXC.aim_log_core(true, true, format, args);  // by Delf 04.05.2015
   }

   public static void aim_logstart(string format, params object[] args)  // Početak reda - timestamp
   {
      ZXC.aim_log_core(true, false, format, args);  // by Delf 04.05.2015
   }

   public static void aim_logadd(string format, params object[] args)  // dodaj unutar istog reda
   {
      ZXC.aim_log_core(false, false, format, args);  // by Delf 04.05.2015
   }

   public static void aim_logln(string format, params object[] args)  // Završi red, dodaj newline
   {
      ZXC.aim_log_core(false, true, format, args);  // by Delf 04.05.2015
   }

   #endregion aim_log

   #region Util Methods

   public static void SendMultipleTabKey(int tabCount)
   {
      for(int i = 0; i < tabCount; ++i) SendKeys.Send("{TAB}");
   }

   public static void IssueAccessDeniedMessage(ZXC.PrivilegedAction privilegedAction, VvSubModul subModul, string documType)
   {
#if(DEBUG)
      ZXC.aim_emsg("NEODOBRENA PRIVILEGIJA za akciju: [{0}] modul: [{1}] subModul: [{2}] smKind: [{3}] documType: [{4}].",
         privilegedAction.ToString(), subModul.modulEnum.ToString(), subModul.subModulEnum.ToString(), subModul.subModulKindEnum.ToString(), documType);
#else
      ZXC.aim_emsg("NEODOBRENA PRIVILEGIJA.");
#endif
   }

   #region Parse Text then Validate or Empty-Null-Zero

   public static string ValOrEmpty_CheckBoxChecked_AsText(VvCheckBox vvcb)
   {
      if(vvcb.Checked == false) return vvcb.TheFalseText;
      else                      return vvcb.TheTrueText;
   }

   public static DateTime ValOr_01010001_DtpDateTime(DateTime dtpDateTime)
   {
      if(dtpDateTime != DateTimePicker.MinimumDateTime)
      {
         return dtpDateTime;
      }
      else // dtpDateTime IS DateTimePicker.MinimumDateTime, convert it to DataBase 'no time' "01.01.0001" 
      {
         return DateTime.MinValue;
      }
   }

   public static TimeSpan ValOr_0000_DtpForTimeOnly(DateTime dtpDateTime)
   {
      if(dtpDateTime.Hour.IsZero() && dtpDateTime.Minute.IsZero())
      {
         return TimeSpan.Zero;
      }
      else 
      {
         return new TimeSpan(dtpDateTime.Hour, dtpDateTime.Minute, 00);
      }
   }

   public static DateTime ValOr_01011753_DateTime(DateTime dateTime)
   {
      //if(dateTime != DateTime.MinValue)
      if(dateTime >= DateTimePicker.MinimumDateTime)
      {
         return dateTime;
      }
      else if((dateTime - DateTime.MinValue) > TimeSpan.Zero) // ovaj else if dodan 04.12.2017. za situacije kada DateTime ima samo HH:mm korisnu komponentu a DDMMYYY je MnValue (01.01.0001) 
      {
         return new DateTime(DateTimePicker.MinimumDateTime.Year, DateTimePicker.MinimumDateTime.Month, DateTimePicker.MinimumDateTime.Day, 
                             dateTime.Hour, dateTime.Minute, 00);
      }
      else // dateTime IS DateTime.MinValue, convert it to DateTimePicker.MinimumDateTime 'no time' "01.01.1753" 
      {
         return DateTimePicker.MinimumDateTime;
      }
   }

   public static DateTime ValOr_01011753_DateTime(TimeSpan timeSpan)
   {
      if(timeSpan == TimeSpan.Zero || (timeSpan.Hours.IsZero() && timeSpan.Minutes.IsZero()))
      {
         return DateTimePicker.MinimumDateTime;
      }
      else
      {
         return new DateTime(DateTimePicker.MinimumDateTime.Year, DateTimePicker.MinimumDateTime.Month, DateTimePicker.MinimumDateTime.Day,
                             timeSpan.Hours, timeSpan.Minutes, 00);
      }
   }

   public static string ValOrEmpty_DtpDateTime_AsText(VvDateTimePicker dtp)
   {
      if(dtp.Value == DateTimePicker.MinimumDateTime) return "";
      else                                            return dtp.Value.ToString(dtp.CustomFormat);
      //else
      //{
      //   if(vvcb.Text.NotEmpty()) return vvcb.Text;
      //   else                    return vvcb.Value.ToString(vvcb.CustomFormat);
      //}
   }

   public static string ValOrEmpty_YyyyDateTime_AsText(DateTime dateTime)
   {
      if(dateTime == DateTime.MinValue) return "";
      else                              return dateTime.ToString(ZXC.VvDateYyyyFormat);
   }

   public static string ValOrEmpty_MmYyyyDateTime_AsText(DateTime dateTime)
   {
      if(dateTime == DateTime.MinValue) return "";
      else                              return dateTime.ToString(ZXC.VvDateMmYyyyFormat);
   }

   public static string ValOrEmpty_ddMMyyDateTime_AsText(DateTime dateTime)
   {
      if(dateTime == DateTime.MinValue) return "";
      else                              return dateTime.ToString(ZXC.VvDateDdMmYyFormat);
   }

   public static string ValOrEmpty_YyyyMMddDateTime_AsText(DateTime dateTime)
   {
      if(dateTime == DateTime.MinValue) return "";
      else                              return dateTime.ToString(ZXC.VvDateYyyyMmDdFormat);
   }

   public static DateTime ValOr_01010001_DateTime(string theText)
   {
      // 29.11.2017: 
      string cleanText = theText.TrimEnd('.').TrimEnd(' ');

      DateTime dateTime = DateTime.MinValue;

    //if(theText   != null && theText  .Length > 0)
      if(cleanText != null && cleanText.Length > 0)
      {
         try
         {
          //dateTime = DateTime.ParseExact(theText, "dd.MM.yyyy.", null); ... ne sljaka kada je npr. mjesec bez vodece nule! 
          //dateTime = DateTime.Parse     (theText.TrimEnd('.')        ); // ovo TrimEnd jerbo mu smeta zadnja tocka poslije godine u ShortDateFormatu 
            dateTime = DateTime.Parse     (cleanText                   ); 
         }
         catch (System.FormatException)
         {
            //return (DateTime.MinValue);
         }
      }

      return dateTime;
   }

   public static DateTime ValOr_01010001_DateTime_ExcelIdiot(string dayOffsetAsText) // dauyOffset od 1.1.1900 (npr "41671" = 01.02.2014.) 
   {
      DateTime dateTime = DateTime.MinValue;

      if(dayOffsetAsText != null && dayOffsetAsText.Length > 0)
      {
         int dayOffset = ZXC.ValOrZero_Int(dayOffsetAsText);
         if(dayOffset.IsZero()) return dateTime;

         // !!! 
         int errorCorrector = 2;

         TimeSpan timeSpan = new TimeSpan(dayOffset - errorCorrector, 0, 0, 0);
         dateTime = ZXC.ExcelZeroDate + timeSpan;
      }

      return dateTime;
   }

   public static DateTime ValOr_01010001_DateTime_Import_ddMMyyyy_Format(string theText)
   {
      DateTime dateTime = DateTime.MinValue;

      if(theText != null && theText.Length > 0)
      {
         try
         {
            dateTime = DateTime.ParseExact(theText, "ddMMyyyy", null);
         }
         catch(System.FormatException)
         {
            //return (DateTime.MinValue);
         }
      }

      return dateTime;
   }

   public static DateTime ValOr_01010001_DateTime_Import_yyyyMMdd_Format(string theText)
   {
      DateTime dateTime = DateTime.MinValue;

      if(theText != null && theText.Length > 0)
      {
         try
         {
            dateTime = DateTime.ParseExact(theText, "yyyyMMdd", null);
         }
         catch(System.FormatException)
         {
            //return (DateTime.MinValue);
         }
      }

      return dateTime;
   }

   public static DateTime ValOr_01010001_DateTime_Import_yyyyMMdd_HHmmss_Format(string theText)
   {
      DateTime dateTime = DateTime.MinValue;

      if(theText != null && theText.Length > 0)
      {
         try
         {
            dateTime = DateTime.ParseExact(theText, VvTimeStampFormat4FileName, null);
         }
         catch(System.FormatException)
         {
            //return (DateTime.MinValue);
         }
      }

      return dateTime;
   }

   public static DateTime ValOr_01010001_DateTime_Import_yyyy_MM_dd_Format(string theText)
   {
      DateTime dateTime = DateTime.MinValue;

      if(theText != null && theText.Length > 0)
      {
         try
         {
            dateTime = DateTime.ParseExact(theText, "yyyy-MM-dd", null);
         }
         catch(System.FormatException)
         {
            //return (DateTime.MinValue);
         }
      }

      return dateTime;
   }

   public static DateTime ValOr_01010001_DateTime_Import_yyyy_Format(string theText)
   {
      DateTime dateTime = DateTime.MinValue;

      if(theText != null && theText.Length > 0)
      {
         try
         {
            dateTime = DateTime.ParseExact(theText, "yyyy", null);
         }
         catch(System.FormatException)
         {
            //return (DateTime.MinValue);
         }
      }

      return dateTime;
   }

   public static DateTime ValOr_01010001_DateTime_Import_MM_YYYY_Format(string theText)
   {
      DateTime dateTime = DateTime.MinValue;

      if(theText != null && theText.Length > 0)
      {
         try
         {
            dateTime = DateTime.ParseExact(theText, "MM yyyy", null);
         }
         catch(System.FormatException)
         {
            //return (DateTime.MinValue);
         }
      }

      return dateTime;
   }

   public static DateTime ValOr_01010001_DateTime_Import_ddMMyy_Format(string theText)
   {
      DateTime dateTime = DateTime.MinValue;

    //if(theText != null && theText.Length > 0)
      if(theText != null && theText.Length > 0 && theText != "000000")
      {
         try
         {
            dateTime = DateTime.ParseExact(theText, "ddMMyy", null);
         }
         catch(System.FormatException)
         {
            //return (DateTime.MinValue);
         }
      }

      return dateTime;
   }

   public static DateTime ValOr_01010001_DateTime_Import_HHmm_Format(string theText)
   {
      DateTime dateTime = DateTime.MinValue;

      if(theText != null && theText.Length > 0)
      {
         try
         {
            dateTime = DateTime.ParseExact(theText, VvTimeOnlyFormat, null);
         }
         catch(System.FormatException)
         {
            //return (DateTime.MinValue);
         }
      }

      return dateTime;
   }

   public static DateTime ValOrDefault_DateTime(string theText, DateTime defaultDateTime)
   {
      DateTime dateTime = defaultDateTime;

      if(theText != null && theText.Length > 0)
      {
         try
         {
          //dateTime = DateTime.ParseExact(theText, "dd.MM.yyyy.", null); ... ne sljaka kada je npr. mjesec bez vodece nule! 
            dateTime = DateTime.Parse     (theText.TrimEnd('.')); // ovo TrimEnd jerbo mu smeta zadnja tocka poslije godine u ShortDateFormatu 
         }
         catch (System.FormatException)
         {
            //return (DateTime.MinValue);
         }
      }

      return dateTime;
   }

   public static uint ValOrZero_UInt(string theText)
   {
      uint num = 0;

      if(theText != null && theText.Length > 0)
      {
         UInt32.TryParse(theText, out num);
      }

      return num;
   }

   public static int ValOrZero_Int(string theText)
   {
      int num = 0;

      if(theText != null && theText.Length > 0)
      {
         Int32.TryParse(theText, out num);
      }

      return num;
   }

   public static int ValOrZero_Int_wDot(string theText_wDot)
   {
      int num = 0;
      string theText;

      if(theText_wDot != null && theText_wDot.EndsWith(".")) theText = theText_wDot.Remove(theText_wDot.Length - 1, 1);
      else                                                   theText = theText_wDot;

      if(theText != null && theText.Length > 0)
      {
         Int32.TryParse(theText, out num);
      }

      return num;
   }

   public static short ValOrZero_Short(string theText)
   {
      short num = 0;

      if (theText != null && theText.Length > 0)
      {
        short.TryParse(theText, out num);
      }

      return num;
   }

   public static ushort ValOrZero_Ushort(string theText)
   {
      ushort num = 0;

      if(theText != null && theText.Length > 0)
      {
         ushort.TryParse(theText, out num);
      }

      return num;
   }

   public static decimal ValOrZero_Decimal(string theText, int numberOfDecimalPLaces)
   {
      return ValOrZero_Decimal(theText, numberOfDecimalPLaces, false);
   }

   public static double ValOrZero_Double(string theText, int numberOfDecimalPLaces)
   {
      return ValOrZero_Double(theText, numberOfDecimalPLaces, false);
   }

   public static decimal ValOrZero_Decimal(string theText, int numberOfDecimalPLaces, bool isForPercent)
   {
      decimal num = 0;

      string textToParse;

      if(isForPercent) textToParse = theText.TrimEnd('%');
      else             textToParse = theText;

      if(textToParse != null && textToParse.Length > 0)
      {
         //Decimal.TryParse(theText, out num);
         Decimal.TryParse(textToParse, System.Globalization.NumberStyles.Number, ZXC.GetNumberFormatInfo(numberOfDecimalPLaces), out num);
      }

      return num;
   }

   public static double ValOrZero_Double(string theText, int numberOfDecimalPLaces, bool isForPercent)
   {
      double num = 0;

      string textToParse;

      if(isForPercent) textToParse = theText.TrimEnd('%');
      else             textToParse = theText;

      if(textToParse != null && textToParse.Length > 0)
      {
         Double.TryParse(textToParse, System.Globalization.NumberStyles.Number, ZXC.GetNumberFormatInfo(numberOfDecimalPLaces), out num);
      }

      return num;
   }

   #endregion Parse Text then Validate or Empty-Null-Zero

   public static decimal DivSafe(decimal djeljenik, decimal djelitelj)
   {
      //decimal a = djeljenik;
      //decimal b = djelitelj;
      //decimal c = b.NotZero() ? a / b : 0M;
      
      /*decimal result =*/ return djelitelj.NotZero() ? djeljenik / djelitelj : 0M;

      //return result;
   }

   public static decimal DivSafeOLD(decimal djeljenik, decimal djelitelj) // CHECK THIS !!! !!! !!! vrijednost 'djeljenik' se promijeni pri return-u ?! 
   {
      //decimal a = djeljenik;
      //decimal b = djelitelj;
      //decimal c = b.NotZero() ? a / b : 0M;

      if(NotZero(djelitelj)) return djeljenik / djelitelj;
      else                   return decimal.Zero;
   }

   public static int CompareStrings(string x, string y)
   {
      if(x == null)
      {
         if(y == null) // If x is null and y is null, they're equal. 
         {
            return 0;
         }
         else // If x is null and y is not null, y is greater. 
         {
            return -1;
         }
      }
      else
      {
         // If x is not null...

         if(y == null) // ...and y is null, x is greater.
         {
            return 1;
         }
         else // ...and y is not null, compare two strings.
         {
            return x.CompareTo(y);
         }
      }
   }

   public static char GetStringsLastChar(string str)
   {
      if(str.Length.IsPositive()) return str[str.Length - 1];
      else                        return '\0';
   }

   public static char VvLastChar(this string str)
   {
      if(str.Length.IsPositive()) return str[str.Length - 1];
      else                        return '\0';
   }

   /// <summary>
   /// money mora biti na 2 decimale!
   /// </summary>
   /// <param name="money"></param>
   /// <returns></returns>
   public static char GetLastCharOfMoney(decimal money)
   {
      int kcrp_x_100_as_int = (int)(Math.Floor(money * 100.00M));

      string kcrp_x_100_as_str = kcrp_x_100_as_int.ToString();

      char lastChar = ZXC.GetStringsLastChar(kcrp_x_100_as_str);

      return lastChar;
   }

   public static bool IsThisMoneyPdvRon2Problematical(decimal money)
   {
      if(GetLastCharOfMoney(money) == '0' ||
         GetLastCharOfMoney(money) == '5'  )
           return false;
      else return true ;
   }
   public static string GetStringsLast3Char(string str)
   {
      if(str.Length < 3) return "";
      
      return str.Substring(str.Length - 3, 3);
   }

   public static string GetStringsLastNchars(string str, int n)
   {
      if(str.Length < n) return "";

      return str.SubstringSafe(str.Length - n, n);
   }

   public static string GetOrovaniCharPattern(string text)
   {
      System.Text.StringBuilder sb = new System.Text.StringBuilder();

      foreach(char ch in text)
      {
         sb.Append(ch.ToString() + "|");
      }

      //sb.Remove(sb.Length - suffix.Length, suffix.Length); // makni suffix sa zadnjega 
      sb.Remove(sb.Length - 1, 1);

      return sb.ToString();
   }

   public static string VvTranslate437ToLatin2(this string text437)
   {
      return Translate437ToLatin2(text437);
   }

   private static string Translate437ToLatin2(string text437)
   {
      char[] chars437 = text437.ToCharArray();
      char translatedChar;

      StringBuilder sb = new StringBuilder(text437.Length);

      foreach(char char437 in chars437)
      {
         switch(char437)
         {
            case '}' : translatedChar = 'ć'; break;
            case ']' : translatedChar = 'Ć'; break;
            case '~' : translatedChar = 'č'; break;
            case '^' : translatedChar = 'Č'; break;
            case '|' : translatedChar = 'đ'; break;
            case '\\': translatedChar = 'Đ'; break;
            case '`' : translatedChar = 'ž'; break;
            case '@' : translatedChar = 'Ž'; break;
            case '{' : translatedChar = 'š'; break;
            case '[' : translatedChar = 'Š'; break;

            default: translatedChar = char437; break;
         }

         sb.Append(translatedChar);
      }

      return sb.ToString();
   }

   public static System.Globalization.NumberFormatInfo GetNumberFormatInfo(int numberOfDecimalPlaces)
   {
      switch(numberOfDecimalPlaces)
      {
         case 0: return ZXC.VvNumberFormatInfo0;
         case 1: return ZXC.VvNumberFormatInfo1;
         case 2: return ZXC.VvNumberFormatInfo2;
         case 3: return ZXC.VvNumberFormatInfo3;
         case 4: return ZXC.VvNumberFormatInfo4;
         case 5: return ZXC.VvNumberFormatInfo5;
         case 6: return ZXC.VvNumberFormatInfo6;
         case 7: return ZXC.VvNumberFormatInfo7;
         case 8: return ZXC.VvNumberFormatInfo8;

         default: 
            ZXC.aim_emsg("Broj decimala " + numberOfDecimalPlaces.ToString() + " NEPODRZAN u ZXC.GetNumberFormatInfo()!");
            return new System.Globalization.NumberFormatInfo();
      }
   }

   // Extension Method! 
   public static string ToStringVv(this decimal dNum)
   {
      return dNum.ToString("N", ZXC.GetNumberFormatInfo(2));
   }
   
   public static string ToStringVv_NoGroup(this decimal dNum)
   {
      return dNum.ToString("N", ZXC.GetNumberFormatInfo(2)).Replace(".", ""); // 1.234,567 ---> 1234,57 
   }

   public static string ToString0Vv_NoGroup(this decimal dNum)
   {
      return dNum.ToString("N", ZXC.GetNumberFormatInfo(0)).Replace(".", ""); // 1.234,567 ---> 1235 
   }

   public static string ToStringVvKolDecimalPlaces(this decimal dNum, int numDecPlaces)
   {
      return dNum.ToString("N", ZXC.GetNumberFormatInfo(numDecPlaces));
   }

   public static string ToStringVv_ForceDot(this decimal dNum)
   {
      return dNum.ToString("N", ZXC.GetNumberFormatInfo(2)).Replace(',', '.');
   }

   public static string ToString0Vv(this decimal dNum)
   {
      return dNum.ToString("N", ZXC.GetNumberFormatInfo(0));
   }

   public static string ToString0Vv(this double dNum)
   {
      return dNum.ToString("N", ZXC.GetNumberFormatInfo(0));
   }

   public static string ToString1Vv(this double dNum)
   {
      return dNum.ToString("N", ZXC.GetNumberFormatInfo(1));
   }

   public static string ToStringVv_NoDecimalNoGroup(this decimal dNum) // 6.600,00 ---> 6600
   {
      return dNum.ToString("F0");
   }

   public static string ToStringVv_NoGroup_ForceDot(this decimal dNum) // 6.600,00 ---> 6600.00
   {
      // 15.01.2015: 
    //return dNum       .ToString("G").Replace(',', '.');
      return dNum.Ron2().ToString("G").Replace(',', '.');
   }

   public static string ToStringVv_NoGroup_ForceDot_Fisk(this decimal dNum) // 6.600,00 ---> 6600,00
   {
      return dNum.ToString("F2").Replace(',', '.');
   }

   // Extension Method! 
   public static string ToStringVv_RSm(this decimal dNum)
   {
      if(dNum.NotZero()) return ToStringVv(dNum);
      else               return "---";
   }

   public static string ToStringVv_JPD(this decimal dNum)
   {
      if(dNum.NotZero()) return ToStringVv(dNum);
      else               return "0,00";
   }

   public static string ToStringVv_HUB3_PDF417(decimal num, int fldLength) // 123.55 --- 000000000000000012355
   {
      string formatter = string.Format("{{0:D{0}}}", fldLength);

      return string.Format(formatter, (int)(num * 100.00M));
   }

   public static string ToStringVv_fieldLength(decimal num, int fldLength) 
   {
      string formatter = string.Format("{{0,{0}}}", fldLength);
      
      return string.Format(formatter, ZXC.ToStringVv(num));
   }

   public static void RaiseErrorProvider(Control control, string errorText)
   {
      ErrorProvider errorProvider = new ErrorProvider();

      errorProvider.SetIconAlignment(control, ErrorIconAlignment.TopLeft);
      errorProvider.SetIconPadding(control, 2);
      errorProvider.BlinkRate = 250;
      errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.AlwaysBlink;
      errorProvider.SetError(control, errorText);
      ZXC.aim_emsg(MessageBoxIcon.Error, errorText);
      errorProvider.Clear();
   }

   public static bool SplitImePrezime(string komplet, ref string imePart, ref string prezimePart)
   {
      int kommaLocation;

      if(komplet.Contains(',') == false) return false;

      kommaLocation = komplet.IndexOf(',');

      prezimePart = komplet.Substring(0, kommaLocation);

      imePart     = komplet.Substring(kommaLocation + 1, komplet.Length - (kommaLocation + 1));

      prezimePart = prezimePart.Trim();
      imePart     = imePart    .Trim();

      if(imePart.Length < 1) return false;

      return true;
   }

   public static float GetPixeliVodoravniZaMilimetre(int milimeters)
   {
      float DpiX;

      using(Graphics grfx = TheVvForm.CreateGraphics())
      {
         DpiX = grfx.DpiX;
      }

      return (milimeters * (DpiX / 25.4f));
   }

   public static float GetPixeliOkomitiZaMilimetre(int milimeters)
   {
      float DpiY;

      using(Graphics grfx = TheVvForm.CreateGraphics())
      {
         DpiY = grfx.DpiY;
      }

      return (milimeters * (DpiY / 25.4f));
   }

   /// <summary>
   /// [assembly: AssemblyVersion("1.0.*")]
   /// Asterisk sign instructs Visual Studio to assign on each build a version 1.0.d.s, 
   /// where d is the number of days since February 1, 2000, and s is the number of seconds since midnight/2.
   /// </summary>
   public static string VvProgramVersion
   {
      get
      {
         return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
      }
   }

   public static DateTime GetBirthDateFromJmbg(string MatBr)
   {
      if(MatBr.Length != 13) return DateTime.MinValue;

      int yyyy, mm, dd;

      //0306967330118

      dd   = ValOrZero_Int(MatBr.Substring(0, 2));
      mm   = ValOrZero_Int(MatBr.Substring(2, 2));
      yyyy = ValOrZero_Int(MatBr.Substring(4, 3));
      yyyy += (yyyy > 500 ? 1000 : 2000);

      //return new DateTime(yyyy, mm, dd);
      return ValOr_01010001_DateTime_Import_ddMMyyyy_Format(dd.ToString("00") + mm.ToString("00") + yyyy.ToString());
   }

   public static string LenLimitedStr(string theString, int maxLength)
   {
      if(theString.IsEmpty()) return "";

      if(theString.Length <= maxLength) return theString;
      else                              return theString.Substring(0, maxLength);
   }

   public static string LenLimitedStrWithAddition(string data, string uniqueAddition, int maxLength)
   {
      if(data.IsEmpty()) return "";

      string theString = data + uniqueAddition;

      if(theString.Length <= maxLength) return theString;
      else
      {
       //return theString.Substring    (0, maxLength - uniqueAddition.Length) + uniqueAddition;
         return theString.SubstringSafe(0, maxLength - uniqueAddition.Length) + uniqueAddition;
      }
   }

   private static decimal VvRon2(decimal dNum)
   {
      return VvRon(dNum, 2);
   }

   private static decimal VvRon(decimal dNum, int numOfDecPlaces)
   {
      return Math.Round(dNum, numOfDecPlaces, MidpointRounding.AwayFromZero);
   }

   private static decimal ron2offix(this decimal value)
   {
      decimal power    = 100M;
      decimal addValue = 0.005M;
      decimal tmpValue;

      tmpValue = Math.Floor((value + addValue + 0.0000000001M) * power);

      return (tmpValue / power);
   }

   // Extension Method! 
   public static decimal Ron2(this decimal value)
   {
      return VvRon2(value);
   }

   public static decimal Ron(this decimal value, int numOfDecPlaces)
   {
      return VvRon(value, numOfDecPlaces);
   }

   #region About Pdv OvoOno

   /// <summary>
   /// Izracunaj UKUPNO s PDV-om iz podataka: osnovica i pdvStopa
   /// --->
   /// return (100 * (1 + (25 / 100.00M))) = 100 * 1.25 = 125;
   /// </summary>
   /// <param name="osnovica"></param>
   /// <param name="pdvStopa"></param>
   /// <returns></returns>
   public static decimal VvGet_125_on_100(this decimal osnovica, decimal pdvStopa)
   {
      return (osnovica * (1.00M + (pdvStopa / 100.00M))); // return (100 * (1 + (25 / 100.00M))) = 100 * 1.25 = 125;
   }

   /// <summary>
   /// Izracunaj IZNOS PDV-a iz podataka: osnovica i pdvStopa
   /// --->
   /// return (100 * (25 / 100)) = 100 * 0.25 = 25;
   /// </summary>
   /// <param name="osnovica"></param>
   /// <param name="pdvStopa"></param>
   /// <returns></returns>
   public static decimal VvGet_25_of_100(this decimal osnovica, decimal pdvStopa)
   {
      return (osnovica * (pdvStopa / 100.00M)); // return (100 * (25 / 100)) = 100 * 0.25 = 25;
   }

   /// <summary>
   /// Izracunaj osnovicu (iznos bez PDVa) iz podataka: ukupnoSaPDVom i pdvStope
   /// --->
   /// return (125 / (1 + (25 / 100))) = (125 / (1 + 0.25)) = 125 / 1.25 = 100;
   /// </summary>
   /// <param name="iznosPDVa"></param>
   /// <param name="pdvStopa"></param>
   /// <returns></returns>
   public static decimal VvGet_100_from_125(this decimal ukupnoSaPDVom, decimal pdvStopa)
   {
      return (ukupnoSaPDVom / (1.00M + (pdvStopa / 100.00M))); // return (125 / (1 + (25 / 100))) = (125 / (1 + 0.25)) = 125 / 1.25 = 100;
   }

   /// <summary>
   /// Izracunaj osnovicu (iznos bez PDVa) iz podataka: ukupnoSaPDVom i pdvStope
   /// --->
   /// return (iznosPDVa * 100M / pdvStopa)
   /// </summary>
   /// <param name="ukupnoSaPDVom"></param>
   /// <param name="pdvStopa"></param>
   /// <returns></returns>
   public static decimal VvGet_100_from_25and25(this decimal iznosPDVa, decimal pdvStopa)
   {
      return (iznosPDVa * 100M / pdvStopa);
   }

   /// <summary>
   /// Izracunaj osnovicu (iznos bez PDVa i PNPa) iz podataka: ukupnoSaPDVom I PNPom te pdvStope i pnpStope
   /// --->
   /// return (128 * 100 / (100 + 25 + 3)) = (12800 / (128)) = 100;
   /// </summary>
   /// <param name="ukupnoSaPDVom_I_PNPom"></param>
   /// <param name="pdvStopa"></param>
   /// <returns></returns>
   public static decimal VvGet_100_from_128(this decimal ukupnoSaPDVom_I_PNPom, decimal pdvStopa, decimal pnpStopa)
   {
      return ZXC.DivSafe(ukupnoSaPDVom_I_PNPom * 100M, (pdvStopa + pnpStopa + 100M)); // return (128 * 100 / (100 + 25 + 3)) = (12800 / (128)) = 100; 
   }

   /// <summary>
   /// Izbij iznos PDVa iz podataka: ukupnoSaPDVom i pdvStope
   /// --->
   /// return (125 - 100) = 25;
   /// </summary>
   /// <param name="ukupnoSaPDVom"></param>
   /// <param name="pdvStopa"></param>
   /// <returns></returns>
   public static decimal VvGet_25_from_125(this decimal ukupnoSaPDVom, decimal pdvStopa)
   {
      return (ukupnoSaPDVom - ukupnoSaPDVom.VvGet_100_from_125(pdvStopa)); // return (125 - 100) = 25;
   }

   /// <summary>
   /// Izbij iznos PDVa iz podataka: ukupnoSaPDVom_I_PNPom te pdvStope i pnpStope
   /// --->
   /// return (128 - 3 - 100) = 25;
   /// </summary>
   /// <param name="ukupnoSaPDVom_I_PNPom"></param>
   /// <param name="pdvStopa"></param>
   /// <returns></returns>
   public static decimal VvGet_25or3_from_128(this decimal ukupnoSaPDVom_I_PNPom, decimal wantedStopa, decimal otherStopa)
   {
      decimal netto = VvGet_100_from_128(ukupnoSaPDVom_I_PNPom, wantedStopa, otherStopa);

      return netto.VvGet_25_of_100(wantedStopa); // return (128 - 3 - 100) = 25;
   }

   /// <summary>
   /// Izracunaj cijenu ili slicno nako djelovanja RABATA 
   /// </summary>
   /// <param name="osnovica"></param>
   /// <param name="rabatStopa"></param>
   /// <returns></returns>
   public static decimal VvGet_90_from_100(this decimal osnovica, decimal rabatStopa)
   {
      return (osnovica * (1M - (rabatStopa / 100M)));
   }

   public static decimal VvGet_rbtSt_100to90(this decimal befRabat, decimal afterRabat)
   {
      if(befRabat.IsZero()) return 0.00M;

      return (1M - afterRabat / befRabat) * 100M;
   }

   public static void Synchronise_A_and_B(ref decimal a, ref decimal b, decimal pdvSt, decimal c)
   {
      int i = 0;
      decimal bi;

      for(decimal ai = a; i < 10; i++, ai -= 0.01M)
      {
         bi = ZXC.VvGet_25_of_100(ai, pdvSt).Ron2();
         
         if(ai + bi == c) 
         {
            a = ai;
            b = bi;
            return;
         } 
            
      }
   }


   #endregion About Pdv OvoOno

   #region SetWorkingDaysList_ForYear

//01.01. Nova Godina
//06.01. Sveta tri kralja
//       Uskrs *(Calc)
//       Uskrsni ponedjeljak *(Calc)
//01.05. Praznik rada
//       Tijelovo *(Calc)
//30.05. Dan državnosti                !!!           NOVO od 2020
//22.06. Dan antifašističke borbe
//25.06. Dan državnosti                !!! samo do zakljucno 2019
//05.08. Dan pobjede i domovinske zahvalnosti
//15.08. Velika Gospa
//08.10. Dan neovisnosti               !!! samo do zakljucno 2019
//01.11. Svi sveti
//18.11. Dan sjecanja                  !!!           NOVO od 2020
//25.12. Božić
//26.12. Sveti Stjepan

   public static DateTime[]           WorkingDaysArray;
   public static DateTime[]        NonWorkingDaysArray;
   public static DateTime[]           WeekendDaysArray;
   public static DateTime[]           PraznikDaysArray;
   public static DateTime[] PraznikDaysBezWeekendArray; // FUSE 

   // 12.05.2025. vidi opasku u ZXC.cs                                                                          
   // spoznajemo da se kod tecaja rabe 3 datuma.                                                                
   // A datum - datum utvrđenja                                                                                 
   // B datum - datum od kada vrijedi                                                                           
   // c datum - datum ya koji trebamo tecaj                                                                     
   // ocito nekada nisu na www.hnb.hr postojali fajlovi po C datumu pa smo morali kalkulirati A i B datume      
   // po novome, izgleda da HNB svakodnevno izdaje fajlove koji se zovu po  datumu pa nam vise A i B ne trebaju 
   // nego mozemo od sad nadalje direktno traziti HNB fajl po C datumu                                          
   // te gasimo ZXC.HNB_DevTecDays_Array                                                                        
   // a kako bi popravili i mozebitno krive stare podatke (slucaj "HZTK 22.04.2025"),                           
   // preimenovati cemo DevTec i Htrans u DevTec2 i Htrans2 pa stari DevTec i Htrans postaju obsolete           
 //public static DateTime[]       HNB_DevTecDays_Array;

   public static List<Htrans2> HtransList;

   public static bool IsThisDanVikend(DateTime date)
   {
      if(date.DayOfWeek == DayOfWeek.Saturday ||
         date.DayOfWeek == DayOfWeek.Sunday)
         return true;
      else
         return false;

   }

   public static bool IsThisDanPraznik(DateTime date, int inYear)
   {
      TimeSpan oneDaySpan = new TimeSpan(1, 0, 0, 0);

      DateTime novaGodina         = new DateTime  (inYear,  1,  1);
      DateTime svetaTriKralja     = new DateTime  (inYear,  1,  6);
      DateTime praznikRada        = new DateTime  (inYear,  5,  1);
      DateTime danAFborbe         = new DateTime  (inYear,  6, 22);
      DateTime danDomZahval       = new DateTime  (inYear,  8,  5);
      DateTime velikaGospa        = new DateTime  (inYear,  8, 15);
      DateTime sviSveti           = new DateTime  (inYear, 11,  1);
      DateTime bozic              = new DateTime  (inYear, 12, 25);
      DateTime svetiStjepan       = new DateTime  (inYear, 12, 26);

      DateTime danDrzavnosti      = inYear < 2020 ? new DateTime(inYear,  6, 25) : new DateTime(inYear,  5, 30);
      DateTime danNeovisnoati     = inYear < 2020 ? new DateTime(inYear, 10,  8) : DateTime.MinValue;           
      DateTime danSjecanja        = inYear < 2020 ? DateTime.MinValue            : new DateTime(inYear, 11, 18);

      DateTime uskrsnaNedjelja    = CalcEasterDate(inYear);
      DateTime uskrsniPonedjeljak = uskrsnaNedjelja + oneDaySpan;

      TimeSpan tijelovoSpan = new TimeSpan(4 + 8*7, 0, 0, 0); // (jednostavno računanje datuma: deveti četvrtak nakon Uskrsa).
      DateTime tjelovo      = uskrsnaNedjelja + tijelovoSpan;

      if(
            date           == novaGodina         ||
            date           == svetaTriKralja     ||
            date           == uskrsnaNedjelja    ||
            date           == uskrsniPonedjeljak ||
            date           == praznikRada        ||
            date           == tjelovo            ||
            date           == danAFborbe         ||
            date           == danDrzavnosti      ||
            date           == danDomZahval       ||
            date           == velikaGospa        ||
            date           == danNeovisnoati     ||
            date           == danSjecanja        || // novo 2020 
            date           == sviSveti           ||
            date           == bozic              ||
            date           == svetiStjepan        )
         return true;
      else
         return false;
   }

   public static bool IsThisDanNeradniDan(DateTime date, int inYear)
   {
      return (IsThisDanVikend(date) || IsThisDanPraznik(date, inYear));
   }
   public static bool IsThisDanRadniDan(DateTime date, int inYear)
   {
      return !IsThisDanNeradniDan(date, inYear);
   }

 //public static void SetWorkingDaysList_ForYear(int year)
   public static void SetWorkingDaysList_ForYear(        )
   {
      List<DateTime> hnb_DevTecDays_list       = new List<DateTime>();
      List<DateTime>     workingDaysList       = new List<DateTime>();
      List<DateTime>  nonWorkingDaysList       = new List<DateTime>();
      List<DateTime>     weekendDaysList       = new List<DateTime>();
      List<DateTime>     praznikDaysList       = new List<DateTime>();
      List<DateTime> praznikDaysBezWeekendList = new List<DateTime>();

      if(IsThisDanRadniDan(prevYearLastDay, prevYearLastDay.Year))
      {
         hnb_DevTecDays_list.Add(projectYearFirstDay); // samo ako je 31.12. prosle godine radni dan 
      }

    //for(DateTime date = projectYearFirstDay; date <= projectYearLastDay; date += ZXC.OneDaySpan)
      for(DateTime date = prevYearDecembar   ; date <= projectYearLastDay; date += ZXC.OneDaySpan)
      {
         if(IsThisDanNeradniDan(date, date.Year))
         {
            nonWorkingDaysList.Add(date);

            if(IsThisDanPraznik(date, date.Year)) praznikDaysList.Add(date);
            if(IsThisDanVikend (date           )) weekendDaysList.Add(date);

            if(IsThisDanPraznik(date, date.Year) == true &&
               IsThisDanVikend (date           ) == false) praznikDaysBezWeekendList.Add(date);
         }
         else 
         {
            workingDaysList.Add(date);

            hnb_DevTecDays_list.Add(date + ZXC.OneDaySpan);
         }

      }
                   WorkingDaysArray =           workingDaysList.ToArray();
                NonWorkingDaysArray =        nonWorkingDaysList.ToArray();
                   WeekendDaysArray =           weekendDaysList.ToArray();
                   PraznikDaysArray =           praznikDaysList.ToArray();
         PraznikDaysBezWeekendArray = praznikDaysBezWeekendList.ToArray();

   // 12.05.2025. vidi opasku u ZXC.cs                                                                          
             //HNB_DevTecDays_Array =       hnb_DevTecDays_list.ToArray();
   }

   public static decimal GetSumaBlagdanskihRadnihSatiZaMjesec(string MMYYYY, bool isTrgovac, bool isPolaVremena, decimal dnevniFondSati)
   {
      DateTime theDate = Placa.GetDateTimeFromMMYYYY(MMYYYY, false);

    //var blagdanDaniZaMjesec = PraznikDaysArray.Where(day => day.Month == theDate.Month);
      var blagdanDaniZaMjesec = PraznikDaysArray.Where(day => day.Month == theDate.Month && day.Year == theDate.Year);

      decimal sumaBlagdanskihRadnihSati = 0.00M;
      foreach(DateTime dan in blagdanDaniZaMjesec)
      {
         sumaBlagdanskihRadnihSati += GetWorkHoursCount(isTrgovac, isPolaVremena, dan, dan, dnevniFondSati);
      }

      return sumaBlagdanskihRadnihSati;
   }

   public static DateTime CalcEasterDate(int piYear)
   {
      DateTime dEaster = new DateTime();

      double iA;
      double iB;
      double iC;
      double iD;
      double iE;
      double iFA;
      double iG;
      double iH;
      double iK;
      double iI;
      double iL;
      double iM;
      double iNA;
      double iP;

      iA = (piYear % 19);
      iB = Math.Floor(piYear / 100.0);
      iC = (piYear % 100);
      iD = Math.Floor(iB / 4.0);
      iE = (iB % 4);
      iFA = Math.Floor((iB + 8) / 25.0);
      iG = Math.Floor((iB - iFA + 1) / 3);
      iH = ((19 * iA) + iB - iD - iG + 15) % 30;
      iI = Math.Floor(iC / 4.0);
      iK = (iC % 4);
      iL = (32 + (2 * iE) + (2 * iI) - iH - iK) % 7;
      iM = Math.Floor((iA + (11 * iH) + (22 * iL)) / 451.0);
      iNA = Math.Floor((iH + iL - (7 * iM) + 114) / 31.0);
      iP = (iH + iL - (7 * iM) + 114) % 31;

      dEaster = new DateTime(piYear, Convert.ToInt16(iNA), Convert.ToInt16(iP + 1));
      return dEaster;
   }

   public static decimal GetWorkHoursCount(bool isTrgovac, bool isPolaVremena, string MMYYYY, uint startDay, uint endDay, decimal dnevniFondSati)
   {
      DateTime startDate = ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(startDay.ToString("00") + MMYYYY);
      DateTime endDate   = ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(endDay  .ToString("00") + MMYYYY);

      if(/* BUG! startDate == endDate ||*/ startDate == DateTime.MinValue || endDate == DateTime.MinValue) return 0;

      return(GetWorkHoursCount(isTrgovac, isPolaVremena, startDate, endDate, dnevniFondSati));
   }

   public static decimal GetWorkHoursCount(bool isTrgovac, bool isPolaVremena, DateTime startDate, DateTime endDate, decimal dnevniFondSati)
   {
    //uint    workHoursCount = 0;
      decimal workHoursCount = 0;
      
      if(dnevniFondSati.IsZero()) dnevniFondSati = Placa.SluzbeniDnevniFondSati;

      for(DateTime currDate = startDate; currDate <= endDate; currDate = currDate.AddDays(1))
      {
         if(isTrgovac == true)
         {
            if(currDate.DayOfWeek      == DayOfWeek.Sunday)   continue;
            else if(currDate.DayOfWeek == DayOfWeek.Saturday) workHoursCount += 5;
            else                                              workHoursCount += 7;
         }
         else
         {
            if(currDate.DayOfWeek == DayOfWeek.Saturday ||
               currDate.DayOfWeek == DayOfWeek.Sunday)     continue;
          //else                                           workHoursCount += (uint)dnevniFondSati;
            else                                           workHoursCount +=       dnevniFondSati;
         }
      }

      if(isPolaVremena == true) workHoursCount /= 2;

      return workHoursCount;
   }

   public static uint GetMFD(decimal SMFS, decimal SDFS/*, bool isTrgovac*/) // mjesecni fond radnih dana 
   {
      return (uint)(ZXC.DivSafe(SMFS, SDFS));
   }

   // sluzbeniMjesecniFondRadniSati je 0 ako nije trgovac jer je nebitan podatak za ovu metodu 
   public static decimal GetSluzbeniDnevniFondRadniSati(bool isTrgovac, string zaMMYYYY, decimal sluzbeniMjesecniFondRadniSati) 
   {
      if(isTrgovac)
      {
         int trgovackiMjesecniFondDana = GetTrgovackiMjesecniFondDana(zaMMYYYY); // fond dana u mjesecu bez Nedjelja 
         return ZXC.DivSafe(sluzbeniMjesecniFondRadniSati, (decimal)trgovackiMjesecniFondDana);
      }
      else // obican, NE trgovac 
      {
         return Placa.SluzbeniDnevniFondSati;
      }
   }

   private static int GetTrgovackiMjesecniFondDana(string zaMMYYYY)
   {
      DateTime startDate = Placa.GetDateTimeFromMMYYYY(zaMMYYYY, false);
      DateTime endDate   = Placa.GetDateTimeFromMMYYYY(zaMMYYYY, true );

      int sundayCount = 0;

      for(DateTime currDate = startDate; currDate <= endDate; currDate = currDate.AddDays(1))
      {
         if(currDate.DayOfWeek == DayOfWeek.Sunday) sundayCount++;
      }

      return endDate.Day - sundayCount;
   }

   #endregion SetWorkingDaysList_ForYear

   public static decimal StopaPromjene(decimal startValue, decimal changedValue)
   {
    //if(startValue.IsZero()) return 100.00M;
      if(startValue.IsZero()) return changedValue.IsZero() ? 0M : 100.00M;

      return (changedValue - startValue) / startValue * 100.00M;
   }

   public static decimal StopaRealizacije(decimal plan, decimal realizacija)
   {
      return (ZXC.DivSafe(realizacija, plan) * 100M);
   }

   public static decimal UdioOdUkupnog(decimal udio, decimal ukupno)
   {
      return StopaRealizacije(ukupno, udio);
   }

   public static decimal StopaNajvecegOdstupanjaOdProsjeka(IEnumerable<decimal> decimalList, /*string GR_key,*/ out decimal out_avgValue, out decimal out_valueWithMaxOdstupanje)
   {
      out_avgValue = out_valueWithMaxOdstupanje = 0M;

      if(decimalList == null || decimalList.Count().IsZero()) return 0;

      var nonZeroDecimalList = decimalList.Where(num => num.NotZero());

      if(nonZeroDecimalList == null || nonZeroDecimalList.Count().IsZero()) return 0;

    //decimal  minValue = decimalList.Min    ();
    //decimal  maxValue = decimalList.Max    ();
      decimal  avgValue = nonZeroDecimalList.Average();

      decimal  maxOdstupanje = nonZeroDecimalList.Max(num => Math.Abs(num - avgValue));
    //decimal  valueWithMaxOdstupanje = Math.Abs(avgValue - maxOdstupanje);
      decimal  valueWithMaxOdstupanje = nonZeroDecimalList.First(num => Math.Abs(num - avgValue) == maxOdstupanje);

      out_avgValue               = avgValue              ;
      out_valueWithMaxOdstupanje = valueWithMaxOdstupanje;

      return StopaPromjene(avgValue, valueWithMaxOdstupanje);
   }

   public static bool AlmostEqual(decimal firstValue, decimal secondValue, decimal tolerancy)
   {
      return Math.Abs(firstValue - secondValue) <= tolerancy;
   }

   public static bool AlmostEqualByPerc(decimal firstValue, decimal secondValue, decimal percTolerancy)
   {
      decimal baseValue = Math.Min(Math.Abs(firstValue), Math.Abs(secondValue));
      decimal tolerancy = baseValue * (percTolerancy / 100.00M);

      bool almostEqualByPerc = AlmostEqual(firstValue, secondValue, tolerancy);

      return almostEqualByPerc;
   }

   public static string GetStr4ISOcheck(string opcCD, string virSpec)
   {
      if(opcCD.Length < 3)
      {
         // 30.06.2015: 
       //ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Na obracunu nije zadana sifra opcine!");
         opcCD = "000";
      }
      return "17" + opcCD.Substring(0, 3) + virSpec;
   }

   // ako ovo ne bude dobro, pogledaj na 
   // E:\0_DOWNLOAD\Code Project Stuff\verhoeff_check_digit_demo\VerhoeffCheckDigit solution...
   public static int GetISO7064(string str4ISOcheck)
   {
      int ckDigit;
      int a, x, b, y, z; 
      
      b = 10; // inicijalni ostatak od '% 10'
      
      //for(i=0; str4ISOcheck[i]; ++i) {
      foreach(char zChar in str4ISOcheck)
      {
         z = ZXC.ValOrZero_Int(zChar.ToString());

         x = b + z;
         a = x % 10;
         if(a.IsZero()) a = 10;
         y = 2 * a;
         b = y % 11;
      }
      
      ckDigit = 11 - b;

      if(ckDigit == 10)
         ckDigit = 0;
      
      return(ckDigit);
   }

   public static string GetIBAN_KBR(string oldZiro) // oldZiro npr: '2360000-1234567890' 
   {
      // http://www.hnb.hr/propisi/devizni-poslovi/h-o%20konstrukciji%20i%20upotrebi%20medunarodnog%20broja%20bankovnog%20racuna-iban.pdf 

      string IBAN_KBR = "";

      oldZiro = oldZiro.Replace("-", "") + "172700";

      decimal ziroNum = ZXC.ValOrZero_Decimal(oldZiro, 0);

      int ostatakNN = (int)(ziroNum % 97);

      int ccNum = (97 + 1) - ostatakNN;

      IBAN_KBR = ccNum.ToString("00");

      return IBAN_KBR;
   }

   public static string GetIBANfromOldZiro(string oldZiro)
   {
      if(oldZiro.Contains("HR") || oldZiro.Contains('-') == false) return oldZiro;

      string IBAN_KBR = ZXC.GetIBAN_KBR(oldZiro);

      return "HR" + IBAN_KBR + oldZiro.Replace("-", "");

   }
   public static string GetPnbModfromOldPnbMod(string oldPnb) //16.01.2013.
   {
      if(oldPnb.Contains("HR") || oldPnb.Length == 4) return oldPnb;

      return (oldPnb.NotEmpty() ? "HR" + oldPnb : "" );
   }

   public static string GetNameForThisCdFromManyLuiLists(string _wantedCd, params VvLookUpLista[] listOfLists)
   {
      string theName;

      foreach(VvLookUpLista list in listOfLists) 
      { 
         theName = list.GetNameForThisCd(_wantedCd);

         if(theName.NotEmpty()) return theName;
      }

      return "";
   }

   internal static int GetNumOfUpisanihDecimala(decimal dNum)
   {
      int decimalPlaces;

      string dNumAsString = Convert.ToString(dNum);
      
      int idxOfFirstAfterDecimaSign = (dNumAsString.IndexOf(ZXC.VvNumberFormatInfo2.NumberDecimalSeparator) + 1);

      if(idxOfFirstAfterDecimaSign.IsZero()) return 0; // nema uopce decimalnog zareza 

      decimalPlaces = ((dNumAsString.Length) - idxOfFirstAfterDecimaSign);           

      return decimalPlaces;
   }

   internal static bool IsStillOldPdv23_Today { get { return IsStillOldPdv23_ForThisDate(DateTime.Now); } }

   internal static bool IsStillOldPdv23_ForThisDate(DateTime theDate)
   {
      return theDate < Faktur.NewPdvStopaDate;
   }

   internal static int GetWeekOfYear(this DateTime theDate)
   {
      string ccName = System.Globalization.CultureInfo.CurrentCulture.Name;
      System.Globalization.CultureInfo cInfo = new System.Globalization.CultureInfo(/*"hr-HR"*/ ccName, true);

      System.Globalization.Calendar cal = cInfo.Calendar;

      return cal.GetWeekOfYear(theDate, ZXC.VvCultureInfo0.DateTimeFormat.CalendarWeekRule, ZXC.VvCultureInfo0.DateTimeFormat.FirstDayOfWeek);
   }

   public static string ExcelColName(int intCol)
   {
      // qukatz:
      intCol--;

      string sColName = "";
      if(intCol < 26)
         sColName = Convert.ToString(Convert.ToChar((Convert.ToByte((char)'A') + intCol)));
      else
      {
         int intFirst = ((int)intCol / 26);
         int intSecond = ((int)intCol % 26);
         sColName = Convert.ToString(Convert.ToByte((char)'A') + intFirst);
         sColName += Convert.ToString(Convert.ToByte((char)'A') + intSecond);
      }
      return sColName;
   }

   public static int ExcelColNumber(string strCol)
   {
      strCol = strCol.ToUpper();
      int intColNumber = 0;
      if(strCol.Length > 1)
      {
         intColNumber = Convert.ToInt16(Convert.ToByte(strCol[1]) - 65);
         intColNumber += Convert.ToInt16(Convert.ToByte(strCol[1]) - 64) * 26;
      }
      else
         intColNumber = Convert.ToInt16(Convert.ToByte(strCol[0]) - 65);

      // qukatz:
      intColNumber++;

      return intColNumber;
   }

   public static string VvXmlElementValue(string xmlString, string elementName, string prefix)
   {
      string elementValue, openTag, closeTag, emptyTag;

      openTag  = "<"  + (prefix.NotEmpty() ? prefix + ":" : "" )+ elementName + ">"  ; // <VvMigrator>   
      closeTag = "</" + (prefix.NotEmpty() ? prefix + ":" : "" )+ elementName + ">"  ; // </VvMigrator>  
      emptyTag = "<"  + (prefix.NotEmpty() ? prefix + ":" : "" )+ elementName + " />"; // <VvMigrator /> 

      if(xmlString.Contains(openTag)  == false) return null;
      if(xmlString.Contains(emptyTag) == true) return null;

      int startIdx, endIdx, valueLength;

      startIdx = xmlString.IndexOf(openTag) + openTag.Length;
      endIdx   = xmlString.IndexOf(closeTag);

      valueLength = endIdx - startIdx;

      elementValue = xmlString.Substring(startIdx, valueLength);

      return elementValue;
   }

   public static void RemoveEmptyNodes(System.Xml.XmlDocument doc)
   {
      System.Xml.XmlNodeList nodes = doc.SelectNodes("//node()");

      foreach(System.Xml.XmlNode node in nodes)
         if((node.Attributes != null && node.Attributes.Count == 0) && (node.ChildNodes.Count == 0))
            node.ParentNode.RemoveChild(node);
   }

   public static void RemoveEmptyNodesNEW(System.Xml.XmlDocument doc)
   {
      System.Xml.XmlNodeList nodes = doc.SelectNodes("//node()");

      foreach(System.Xml.XmlNode node in nodes)
      {
         // Provjeri ima li samo whitespace text nodes
         bool hasOnlyWhitespace = node.ChildNodes.Count > 0 &&
                                  node.ChildNodes.Cast<System.Xml.XmlNode>()
                                      .All(n => n.NodeType == System.Xml.XmlNodeType.Text &&
                                               string.IsNullOrWhiteSpace(n.Value));

         if((node.Attributes == null || node.Attributes.Count == 0) &&
            (node.ChildNodes.Count == 0 || hasOnlyWhitespace))
         {
            node.ParentNode.RemoveChild(node);
         }
      }
   }
   public static /*string*/System.Xml.XmlDocument RemoveEmptyNodes(string xmlString)
   {
      System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();

      xmlDocument.PreserveWhitespace = true;
      xmlDocument.LoadXml(xmlString);

      ZXC.RemoveEmptyNodes(xmlDocument);

      return xmlDocument/*.OuterXml*/;
   }

   public static System.Xml.XmlDocument RemoveEmptyNodesNEW(string xmlString)
   {
      var xDoc = System.Xml.Linq.XDocument.Parse(xmlString);

      // Ukloni sve prazne elemente (bez atributa i bez sadržaja)
      xDoc.Descendants()
          .Where(e => !e.HasAttributes &&
                      !e.HasElements &&
                      string.IsNullOrWhiteSpace(e.Value))
          .Remove();

      System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
      xmlDocument.PreserveWhitespace = true;
      xmlDocument.LoadXml(xDoc.ToString());

      return xmlDocument;
   }

   /// <summary>
   /// Uklanja sve prazne XML elemente iz XML stringa.
   /// Element je prazan ako nema atribute, nema child elemente i nema sadržaj (ili ima samo whitespace).
   /// </summary>
   public static string RemoveEmptyNodes_v2(string xmlString)
   {
      if(string.IsNullOrWhiteSpace(xmlString))
         return xmlString;

      try
      {
         var xDoc = System.Xml.Linq.XDocument.Parse(xmlString);

         // Iterativno uklanjaj prazne elemente
         bool hasChanges;
         do
         {
            hasChanges = false;

            var emptyElements = xDoc.Descendants()
               .Where(e => !e.HasAttributes &&
                          !e.HasElements &&
                          string.IsNullOrWhiteSpace(e.Value))
               .ToList();

            if(emptyElements.Any())
            {
               hasChanges = true;
               emptyElements.ForEach(e => e.Remove());
            }
         }
         while(hasChanges);

         // Vrati kao string
         return xDoc.ToString();
      }
      catch(System.Xml.XmlException ex)
      {
         ZXC.aim_log("RemoveEmptyNodes greška: {0}", ex.Message);
         return xmlString;
      }
   }
   public static string BeautifyXml(string xmlString, System.Text.Encoding encoding)
   {
      StringBuilder stringBuilder = new StringBuilder();

      System.Xml.Linq.XElement xElement = System.Xml.Linq.XElement.Parse(xmlString);

      System.Xml.XmlWriterSettings xmlWriterSettings = new System.Xml.XmlWriterSettings()
      {
         Encoding    = encoding,
         Indent      = true    , // byQ 
         IndentChars = "   "   , // byQ 

         OmitXmlDeclaration  = true,
       //NewLineOnAttributes = true,
      };

      using(var xmlWriter = System.Xml.XmlWriter.Create(stringBuilder, xmlWriterSettings))
      {
         xElement.Save(xmlWriter);
      }

    //return stringBuilder.ToString();
      return @"<?xml version=""1.0"" encoding=""UTF-8""?>" + "\n" +
             stringBuilder.ToString();
   }


   public static int numOfDateBreaks = 15; // ovo moras inkrementirati kada dodajes novu kamatu 
   public static int set_kamtbl_idx(DateTime dateStart/*, kamtbl kamtbl_rec*/)
   {
      int i=0;

      do 
      {
         if(ZXC.kamtbl_rec.kt_date[i] == DateTime.MinValue)
         {
            if(i.NotZero() && ZXC.kamtbl_rec.kt_date[i - 1] <= dateStart) return (i - 1);
            else                                                          return( -1);
         }

         if(ZXC.kamtbl_rec.kt_date[i] >= dateStart) return (i - 1);

      // 01.08.2016: 
    //} while(++i < 12);
      } while(++i < numOfDateBreaks + 2);

      return(-1);
   }

   public static decimal PopapajUplatuNa(this decimal iznosUplate, ref decimal iznosDuga, bool mozeUminus)
   {
      if(iznosUplate.IsZero()) return 0.00M;

      decimal popapano = 0;

      if(iznosUplate > iznosDuga)
      {
         popapano  = iznosDuga;
         if(mozeUminus == false) iznosDuga = 0.00M;
         else                    iznosDuga -= iznosUplate;
      }
      else 
      {
         iznosDuga -= iznosUplate;
         popapano   = iznosUplate;
      }

      iznosUplate -= popapano;

      // vrati preostalo uplate 
      return iznosUplate;
   }

   public static decimal CalcDekurzivStopa(int idx, int dana, bool isPravna/*, kamtbl kamtbl_rec*/)
   {
      // 29.01.2018: 
    //if(idx < 0 || idx >= 12                 ) return (0.00M);
      if(idx < 0 || idx >= ZXC.numOfDateBreaks) return (0.00M);

      int     danaUgodini;
      decimal dio, kamStopa;

      if(isPravna == true)
         kamStopa = ZXC.kamtbl_rec.kt_stopaPra[idx];
      else
         kamStopa = ZXC.kamtbl_rec.kt_stopaFiz[idx];

      danaUgodini = (DateTime.IsLeapYear(ZXC.kamtbl_rec.kt_date[idx].Year) ? 366 : 365) * 100;

      dio = (kamStopa * dana) / danaUgodini;

      return(dio);
   }

   public static int GetOtsZakas(DateTime dateOD, DateTime dateDO)
   {
      return (int)((dateDO.Date - dateOD.Date).TotalDays);
   }

   public static bool IsOPPsljednost(string theTT, uint theLUIuinteger)
   {
    //if( TtInfo(theTT).IsPrihodTT                                   && theLUIuinteger.NotZero()) return true;
    //if((TtInfo(theTT).IsPrihodTT      || TtInfo(theTT).IsPonudaTT) && theLUIuinteger.NotZero()) return true;
      if((TtInfo(theTT).IsPrihodTTorABx || TtInfo(theTT).IsPonudaTT) && theLUIuinteger.NotZero()) return true;
      else                                                                                        return false;
   }

   public static void SetStatusText(string statusText)
   {
      // Faza 1a / C5: kroz ZXC.StatusTextSetter; fallback na TheVvForm.TStripStatusLabel.
      if(ZXC.StatusTextSetter != null)
      {
         ZXC.StatusTextSetter(statusText);
         return;
      }

      if(ZXC.TheVvForm != null && ZXC.TheVvForm.TStripStatusLabel != null)
      {
         ZXC.TheVvForm.TStripStatusLabel.Text = statusText;
         ZXC.TheVvForm.TStripStatusLabel.Invalidate();
         ZXC.TheVvForm.Update();
       //ZXC.TheVvForm.Refresh();
      }
   }

   public static void ClearStatusText()
   {
      // Faza 1a / C5: kroz ZXC.StatusTextClearer; fallback na TheVvForm.TStripStatusLabel.
      if(ZXC.StatusTextClearer != null)
      {
         ZXC.StatusTextClearer();
         return;
      }

      if(ZXC.TheVvForm != null && ZXC.TheVvForm.TStripStatusLabel != null)
      {
         ZXC.TheVvForm.TStripStatusLabel.Text = "";
         ZXC.TheVvForm.Refresh();
      }
   }

   internal static string[] GetStringArrayFromCommaSeparatedTokens(string commaSeparatedTokens)
   {
      // 13.01.2016: 
    //return commaSeparatedTokens.                 Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
      return commaSeparatedTokens.Replace(" ", "").Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
   }

   internal static decimal RefreshedKunskaCijena(decimal oldKunskaCij, decimal oldDevTec, decimal newDevTec)
   {
      return DivSafe(oldKunskaCij, oldDevTec) * newDevTec;
   }

   internal static int MonthDifference(this DateTime dateA, DateTime dateB)
   {
      //return Math.Abs((dateA.Month - dateB.Month) + 12 * (dateA.Year - dateB.Year));
      //return Math.Abs((dateA - dateB).tt);
        return ((dateA.Year - dateB.Year) * 12) + dateA.Month - dateB.Month;
   }

   internal static int YearDifference(DateTime dateOfBirth, DateTime dateToday) // CalculateAge 
   {  
       int age = dateToday.Year - dateOfBirth.Year;

       if(dateToday.DayOfYear < dateOfBirth.DayOfYear)  
           age = age - 1;  
     
       return age;  
   } 

   internal static string VvExceptionDetails(Exception ex)
   {
      string exInfo =
      /*return*/
      "[" + DateTime.Now.ToString(ZXC.VvDateAndTimeFormat) + "] ExMessage:          \r\n" +
      ex.Message + "\r\n" +
      "ExStackTrace:                                                                \r\n" +
      ex.StackTrace + "\r\n" +
      "=============================\r\n" +
      "=============================\r\n" +
      "=============================\r\n";

      string innerExInfo =

      ex.InnerException == null ? "" :

     ("InnerExMessage:                                                              \r\n" +
      ex.InnerException.Message + "\r\n" +
      "InnerExStackTrace:                                                           \r\n" +
      ex.InnerException.StackTrace);

      return exInfo + innerExInfo;
   }

   internal static string GetShortFiskTipDokumenta(Raverus.FiskalizacijaDEV.PopratneFunkcije.TipDokumentaEnum tipDokumenta)
   {
      switch(tipDokumenta)
      { 
         case Raverus.FiskalizacijaDEV.PopratneFunkcije.TipDokumentaEnum.PoslovniProstorZahtjev: return "PpZ";
         case Raverus.FiskalizacijaDEV.PopratneFunkcije.TipDokumentaEnum.PoslovniProstorOdgovor: return "PpO";
         case Raverus.FiskalizacijaDEV.PopratneFunkcije.TipDokumentaEnum.RacunZahtjev          : return "RnZ";
         case Raverus.FiskalizacijaDEV.PopratneFunkcije.TipDokumentaEnum.RacunOdgovor          : return "RnO";
      }

      return tipDokumenta.ToString();
   }

   internal static bool IsBadOib(string oib, bool allowEmpty)
   {
      // 22.10.2021: Qingfu 
      if(ZXC.IsSvDUH) return false;

      if(allowEmpty && oib.IsEmpty()) return false;

      if(oib.Length != 11) return true;

      if(oib.Any(ch => Char.IsDigit(ch) == false)) return true;

      return !Is_OIB_Ok(oib); // Is_OIB_Ok() vraca true ako je ok, a nama treba IsBadOib pa vracamo obrnuto. 
   }

 //private static bool CheckOIB(string oib)
   private static bool Is_OIB_Ok(string oib)
   {
      if(oib.Length != 11) return false;

      long b;
      if(!long.TryParse(oib, out b)) return false;

      int a = 10;
      for(int i = 0; i < 10; i++)
      {
         a = a + Convert.ToInt32(oib.Substring(i, 1));
         a = a % 10;
         if(a == 0) a = 10;
         a *= 2;
         a = a % 11;
      }
      int kontrolni = 11 - a;
      if(kontrolni == 10) kontrolni = 0;

      return kontrolni == Convert.ToInt32(oib.Substring(10, 1));
   }

   internal static DateTime GetDateTimeFromDecimal_HHMMonly(decimal number) // number MORA biti pozitivan! 
   {
      int hh = (int)Math.Truncate(number);
      int mm = (int)((number - (decimal)hh) * 100M);

    //return new DateTime(ZXC.projectYearFirstDay.Year, ZXC.projectYearFirstDay.Month, ZXC.projectYearFirstDay.Day,
    //                    hh, mm, 00);
      return new DateTime(DateTime.MinValue      .Year, DateTime.MinValue      .Month, DateTime.MinValue      .Day,
                          hh, mm, 00);
   }

   internal static DateTime GetDateTimeFromDecimal_fullDateTime(DateTime date, decimal number) // number MORA biti pozitivan! 
   {
      int hh = (int)Math.Truncate(number);
      int mm = (int)((number - (decimal)hh) * 100M);

      return new DateTime(date.Year, date.Month, date.Day, hh, mm, 00);
   }

   internal static decimal GetDecimalFromDateTime(DateTime timeDataOnly) // say, 8:47 
   {
      int hh = timeDataOnly.Hour  ; //  8
      int mm = timeDataOnly.Minute; // 47

      return hh + mm / 100M;
   }

   internal static decimal GetDecimalFromTimeSpan(TimeSpan theTimeSpan) // say, 8:47 
   {
      int hh = theTimeSpan.Hours  ; //  8
      int mm = theTimeSpan.Minutes; // 47

      return hh + mm / 100M;
   }

   internal static string SystemValidFileName(string fileName)
   {
    //fileName = fileName.Replace('\'', '’'); // U+2019 right single quotation mark
    //fileName = fileName.Replace('"' , '”'); // U+201D right double quotation mark
    //fileName = fileName.Replace('/' , '⁄'); // U+2044 fraction slash
      fileName = fileName.Replace('\'', '_'); // by Q                              
      fileName = fileName.Replace('"' , '_'); // by Q                              
      fileName = fileName.Replace('/' , '_'); // by Q                              
      foreach(char c in System.IO.Path.GetInvalidFileNameChars())
      {
         fileName = fileName.Replace(c, '_');
      }
      return fileName;
   }

   internal static Image Get_Faktur_EAN8_Image(Faktur faktur_rec)
   {
      string bcStr = faktur_rec.TtSort_And_TtNum;

      BarcodeLib.Barcode barcode = new BarcodeLib.Barcode(/*"30583306", BarcodeLib.TYPE.EAN8*/);
      barcode.IncludeLabel = true;
      barcode.AlternateLabel = bcStr;
      Image barcodeImage = barcode.Encode(BarcodeLib.TYPE.EAN8/*13*/, bcStr, Color.DarkBlue, Color.LightBlue, 100, 100);
      return barcodeImage;
   }

   #region VvXmlDR OvoOno

   internal static string VvSerializedDR_DirectoryName
   {
      get
      {
         // Faza 1a / C4: kroz ZXC.LocalDirectoryForVvFileProvider; fallback na VvForm.
         string subDir = @"VvSerializedDR_" + ZXC.PUG_ID;
         return ZXC.LocalDirectoryForVvFileProvider != null
              ? ZXC.LocalDirectoryForVvFileProvider(subDir)
              : VvForm.GetLocalDirectoryForVvFile(subDir);
      }
   }

   //internal/*private*/ static List<string> GetFileNames_AutoCreated_VvXmlDR_ToDelete(string virtualRecordName2, DateTime dateDO)
   //{
   //   string searchPattern  = "?_" + "*vv" + virtualRecordName2 + "*" + ZXC.PUG_ID + "*.xml";
   //
   //   DirectoryInfo diInfo = new DirectoryInfo(ZXC.VvSerializedDR_DirectoryName);
   //
   //   FileInfo[] fiArray  = diInfo.GetFiles(searchPattern );
   //
   //   // dateDO je prekPrekJucer, comparer je '<' a ne '<=' pa je rezultat da recimo u ponedeljak jos uvijek ne brise fajlove od petka 
   //   List<string> fileNames = fiArray.Where(fi => fi.CreationTime.Date < dateDO.Date).Select(fi => fi.FullName).ToList();
   //
   //   return fileNames ;
   //}

   internal static List<string> GetFileNames_AutoCreated_VvXmlDR_ToAdd_ToTargzip(string directoryName)
   {
      string searchPattern  = "?_*vv*_" + ZXC.PUG_ID + "_*.xml"; // ne zaboravi vratiti iz komentara ticker i projectYear! 

      DirectoryInfo diInfo = new DirectoryInfo(directoryName);

      FileInfo[] fiArray  = diInfo.GetFiles(searchPattern ); // za '?_*' 

      List<string> fileNames = fiArray.Select(fi => fi.FullName).ToList();

    //return fileNames.OrderBy(q => q).ToList();
      return fileNames                         ;
   }

   internal static List<string> GetFileNames_AutoCreated_VektorLog_ToAdd_ToTargzip(string gZipPreffix, string directoryName)
   {
      string searchPattern  = gZipPreffix + "*.TXT";

      DirectoryInfo diInfo = new DirectoryInfo(directoryName);

      FileInfo[] fiArray  = diInfo.GetFiles(searchPattern ); 

      List<string> fileNames = fiArray.Select(fi => fi.FullName).ToList();

    //return fileNames.OrderBy(q => q).ToList();
      return fileNames                         ;
   }

   internal static List<string> GetFileNames_AutoCreated_VvXml_RnX_IRM_ToAdd_ToTargzip(string directoryName, string gZipPreffix, int dayOfMonthLimit)
   {
      string searchPattern = gZipPreffix + "IRM-*_20????" + dayOfMonthLimit.ToString("00") + "_*.xml";

      DirectoryInfo diInfo = new DirectoryInfo(directoryName);

      FileInfo[] fiArray = diInfo.GetFiles(searchPattern); 

      List<string> fileNames = fiArray.Select(fi => fi.FullName).ToList();

      return fileNames;
   }

   //private  static List<string> oldVvXmlDRfilesCheckedAndDeletedList;
   //internal static List<string> OldVvXmlDRfilesCheckedAndDeletedList
   //{
   //   get
   //   {
   //      if(oldVvXmlDRfilesCheckedAndDeletedList.IsEmpty()) oldVvXmlDRfilesCheckedAndDeletedList = new List<string>();
   //      return oldVvXmlDRfilesCheckedAndDeletedList;
   //   }
   //   set => oldVvXmlDRfilesCheckedAndDeletedList = value;
   //}

   internal static bool Delete_OldNamingConvention_AutoCreated_VvXmlDR_Directory()
   {
      bool hasError = false;

      //string searchPattern = "?_" + "*vv*" + "_" + ZXC.CURR_prjkt_rec.Ticker + "_" + "*.xml";
      //
      //DirectoryInfo diInfo = new DirectoryInfo(ZXC.VvSerializedDR_DirectoryName);
      //
      //FileInfo[] fiArray = diInfo.GetFiles(searchPattern);
      //
      //List<string> fileNames = fiArray.Select(fi => fi.FullName).ToList();
      //
      //foreach(string fileName in fileNames)
      //{
      //   try { System.IO.File.Delete(fileName); }
      //   catch(Exception ex) { ZXC.aim_emsg("Delete File Error:\n\n{0}", ex.Message); hasError = true; }
      //}

      string oldDirectory = VvForm.GetLocalDirectoryForVvFile(@"VvSerializedDR");

      try { System.IO.Directory.Delete(oldDirectory, true); }
      catch(Exception ex) { ZXC.aim_emsg("Delete Directory Error:\n\n{0}", ex.Message); hasError = true; }

      return hasError;
   }

   //internal static bool Delete_AutoCreated_VvXmlDR_Files(string virtualRecordName2, DateTime dateDO)
   //{
   //   bool hasError = false;
   //
   //   List<string> fileNames = GetFileNames_AutoCreated_VvXmlDR_ToDelete(virtualRecordName2, dateDO);
   //
   //   foreach(string fileName in fileNames)
   //   {
   //      try { System.IO.File.Delete(fileName); }
   //      catch(Exception ex) { ZXC.aim_emsg("Delete File Error:\n\n{0}", ex.Message); hasError = true; }
   //   }
   //
   //   return hasError;
   //}

   internal static VvDataRecord AutoCreated_VvXmlDocumentDR_Exists(VvDataRecord theVvDataRecord, out bool xmlReadOK, uint ttNum)
   {
      FileInfo[] fiArray = GetAutoCreated_VvXmlDocumentDR_FileInfoArray_ForThisRecord(theVvDataRecord, ttNum);

      VvDataRecord existingVvDataRecord;

      xmlReadOK = true;

      if(fiArray.Length.NotZero()) // Should ALERT!!! 
      {
         string newestFileName = fiArray.OrderBy(fi => fi.Name).Last().FullName;

         existingVvDataRecord = theVvDataRecord.VvDataRecordFactory();

         existingVvDataRecord = existingVvDataRecord.Deserialize_VvDataRecord_FromXmlFile(newestFileName);

         // 13.10.2022: start !!!                                                        
         // ovo je, (pokusaj?), da onaj di se ovo vraca zna da je zapravo SHIT HAPPEND   
         // ali je existingVvDataRecord null zbog sjebanog XML-a a ne zato kaj je sve ok 
         if(existingVvDataRecord == null)
         {
            xmlReadOK = false; 
         }
         // 13.10.2022: end !!!                                                          
      }
      else // it's ok 
      {
         existingVvDataRecord = null;
      }

      return existingVvDataRecord;
   }

   internal static FileInfo[] GetAutoCreated_VvXmlDocumentDR_FileInfoArray_ForThisRecord(VvDataRecord theVvDataRecord, uint ttNum)
   {
      string virtualDocumentRecordName = theVvDataRecord.VirtualRecordName             ;
      string tt                        = ((VvDocumLikeRecord)theVvDataRecord).VirtualTT;

    //string searchPattern = VvDataRecord.Auto_vvXmlDR_preffix + "*vv" + theVvDataRecord.VirtualRecordName2 + "_" + ttNum.ToString() + "_" + ZXC.CURR_prjkt_rec.Ticker + "_*.xml";
      string searchPattern = VvDataRecord.Auto_vvXmlDR_preffix + "*vv" + theVvDataRecord.VirtualRecordName2 + "_" + ttNum.ToString() + "_" + ZXC.PUG_ID                + "_*.xml";

      DirectoryInfo diInfo = new DirectoryInfo(ZXC.VvSerializedDR_DirectoryName);

      FileInfo[] fiArray = diInfo.GetFiles(searchPattern);

      return fiArray;
   }

   internal static int Vv_TarAndGZip_ByDate(string gZipPreffix, string directoryName, int dayOfMonthLimit)
   {
      int packedFilesCount = 0;

      string filename        = gZipPreffix + ZXC.PUG_ID + ".tar.gz";                         
      string targzFileName   = Path.Combine(directoryName, filename);
      List<string> fileNames = ZXC.GetFileNames_AutoCreated_VvXmlDR_ToAdd_ToTargzip(directoryName);
      
      // Kao, ako Exists, vec si ovo obavio, nije prvi ulazak u danu pa nemoj nist' raditi. (A pakira se sve sto nije danasnje) 
      if(!File.Exists(targzFileName)) ZXC.CreateAndAddFilesToTarThenDeleteFiles(targzFileName, fileNames);
    //else                            ZXC.AddFilesToTarThenDeleteFiles         (targzFileName, fileNames);

      //ZXC.ExtractTGZ(targzFileName); primjer za Unzip-anje! 


      return packedFilesCount;
   }

   internal static void CreateAndAddFilesToTarThenDeleteFiles(string outputTarFilename, List<string> filenames)
   {
      using(FileStream fs = new FileStream(outputTarFilename, FileMode./*OpenOr*/Create, FileAccess.Write, FileShare.None)) // byQ 
      using(Stream gzipStream = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(fs))
      using(ICSharpCode.SharpZipLib.Tar.TarArchive tarArchive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateOutputTarArchive(gzipStream))
      {
       //tarArchive.RootPath    = Path.GetDirectoryName(outputTarFilename); // da ide relative pathname 
         string sourceDirectory = Path.GetDirectoryName(outputTarFilename); // da ide relative pathname 

         // Note that the RootPath is currently case sensitive and must be forward slashes e.g. "c:/temp"
         // and must not end with a slash, otherwise cuts off first char of filename
         // This is scheduled for fix in next release
         tarArchive.RootPath = sourceDirectory.Replace('\\', '/');
         if(tarArchive.RootPath.EndsWith("/")) tarArchive.RootPath = tarArchive.RootPath.Remove(tarArchive.RootPath.Length - 1);

         // tarArchive.SetKeepOldFiles(false); ovo ne radi nist?

         foreach(string filename in filenames)
         {
            ICSharpCode.SharpZipLib.Tar.TarEntry tarEntry = ICSharpCode.SharpZipLib.Tar.TarEntry.CreateEntryFromFile(filename);
            tarArchive.WriteEntry(tarEntry, /*true*/false);

            try { System.IO.File.Delete(filename); }
            catch(Exception ex) { ZXC.aim_log("Delete File Error:\n\n{0}", ex.Message); /*hasError = true;*/ }

         }

         tarArchive.Close();
      }
   }

   public static void ExtractTGZ(String gzArchiveName/*, String destFolder*/)
   {
      string destFolder = Path.GetDirectoryName(gzArchiveName); // byQ 

      Stream inStream   = File.OpenRead(gzArchiveName);
      Stream gzipStream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(inStream);

      ICSharpCode.SharpZipLib.Tar.TarArchive tarArchive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(gzipStream);
      tarArchive.ExtractContents(destFolder);
      tarArchive.Close();

      gzipStream.Close();
      inStream  .Close();
   }

   public static T DeserializeFromXmlData<T>(string xmlData)
   {
      System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

      using(StringReader reader = new StringReader(xmlData))
      {
         return (T)serializer.Deserialize(reader);
      }
   }

   #endregion VvXmlDR OvoOno

   internal static uint GetDesnoOdZareza_asUint(uint ttNum, int formater)
   {
      decimal prelomljeniTtNum = (decimal)ttNum / (decimal)formater;
      decimal samoLijevo = Math.Floor(prelomljeniTtNum);

      decimal samoDesno = prelomljeniTtNum - samoLijevo;

      return (uint)(samoDesno * formater);
   }

   private static uint GetLijevoOdZareza_asUint(decimal num, int formater)
   {
      return (uint)Math.Floor(num);
   }

   public static T DeepCopy<T>(T obj)
   {
      if(obj == null)
         throw new ArgumentNullException("Object cannot be null");
      return (T)Process(obj);
   }

   static object Process(object obj)
   {
      if(obj == null)
         return null;
      Type type = obj.GetType();
      if(type.IsValueType || type == typeof(string))
      {
         return obj;
      }
      else if(type.IsArray)
      {
         Type elementType = Type.GetType(
              type.FullName.Replace("[]", string.Empty));
         var array = obj as Array;
         Array copied = Array.CreateInstance(elementType, array.Length);
         for(int i = 0; i < array.Length; i++)
         {
            copied.SetValue(Process(array.GetValue(i)), i);
         }
         return Convert.ChangeType(copied, obj.GetType());
      }
      else if(type.IsClass)
      {
         object toret = Activator.CreateInstance(obj.GetType());
         FieldInfo[] fields = type.GetFields(BindingFlags.Public |
                     BindingFlags.NonPublic | BindingFlags.Instance);
         foreach(FieldInfo field in fields)
         {
            object fieldValue = field.GetValue(obj);
            if(fieldValue == null)
               continue;
            field.SetValue(toret, Process(fieldValue));
         }
         return toret;
      }
      else
         throw new ArgumentException("Unknown type");
   }

   internal static uint GetUintFrom9bools(bool b1, bool b2, bool b3, bool b4, bool b5, bool b6, bool b7, bool b8, bool b9)
   {
      uint boolNum = 1000000000;

      boolNum += (uint)(b9 == true ? 1 : 0) * 100000000;
      boolNum += (uint)(b8 == true ? 1 : 0) *  10000000;
      boolNum += (uint)(b7 == true ? 1 : 0) *   1000000;
      boolNum += (uint)(b6 == true ? 1 : 0) *    100000;
      boolNum += (uint)(b5 == true ? 1 : 0) *     10000;
      boolNum += (uint)(b4 == true ? 1 : 0) *      1000;
      boolNum += (uint)(b3 == true ? 1 : 0) *       100;
      boolNum += (uint)(b2 == true ? 1 : 0) *        10;
      boolNum += (uint)(b1 == true ? 1 : 0) *         1;

      return boolNum;
   }

   #region 2022 ---> 2023 ... Kuna 2 EUR prelazak 

   //12.07.2022. Poznat je i službeni tečaj konverzije koji će biti 7,53450 kuna za 1 euro.
   public const decimal HRD_tecaj = 7.5345M;

   public static decimal EURiIzKuna_HRD_(this decimal kuneMoney)
   {
      return ZXC.DivSafe(kuneMoney, ZXC.HRD_tecaj).Ron2();
   }

   public static decimal KuneIzEURa_HRD_(this decimal eurMoney)
   {  
      return (eurMoney * ZXC.HRD_tecaj).Ron2();
   }

   /// <summary>
   /// Ovisno o ZXC.projectYear
   /// </summary>
   /// <param name="nekiMoney"></param>
   /// <returns></returns>
   public static decimal EURiIzKuna_ILI_KuneIzEURa_HRD_(decimal nekiMoney)
   {
      if(ZXC.projectYearAsInt >= 2023) return KuneIzEURa_HRD_(nekiMoney);
      else                             return EURiIzKuna_HRD_(nekiMoney);
   }

   // 25.08.2022: tu jos fale: NiceEURo_1_00 i NiceEURo_5_00 
   public static decimal NiceEURo(decimal eurMoney, decimal increment) { return Math.Round(eurMoney * ZXC.DivSafe(1.00M, increment), MidpointRounding.AwayFromZero) / ZXC.DivSafe(1.00M, increment); }

   public static decimal NiceEURo_0_25(decimal eurMoney) { return Math.Round(eurMoney * 4, MidpointRounding.AwayFromZero) / 4; }
   public static decimal NiceEURo_0_50(decimal eurMoney) { return Math.Round(eurMoney * 2, MidpointRounding.AwayFromZero) / 2; }
   public static decimal NiceEURo_0_05(decimal eurMoney)
   {
      // 1.23 ---> 1.25 
      // 1.25 ---> 1.25 
      // 1.27 ---> 1.30 
      // 1.30 ---> 1.30 

      decimal centIncrement;

      if(eurMoney > eurMoney.Ron2())
      {
         eurMoney += 0.01M;
         eurMoney = eurMoney.Ron2();
      }

      int centDigit = ith_digit(Decimal.ToInt32(eurMoney * 100M), 0);

      switch(centDigit)
      {
         case 1: 
         case 6: centIncrement = 0.04M; break;
         case 2:
         case 7: centIncrement = 0.03M; break;
         case 3:
         case 8: centIncrement = 0.02M; break;
         case 4:
         case 9: centIncrement = 0.01M; break;

         default: centIncrement = 0.00M; break;
      }

      return (eurMoney + centIncrement).Ron2();
   }

   // zero based index gledano s desna na lijevo 
   private static int ith_digit(int n, int i)
   {
      return (int)(n / Math.Pow(10, i)) % 10;
   }

   public static decimal NiceEURo_ReadyKuna(decimal kuneMoney)
   {
      decimal uglyEURo = ZXC.DivSafe(kuneMoney, ZXC.HRD_tecaj);
    //decimal niceEURo = NiceEURo_0_05(uglyEURo );
    //decimal niceEURo = NiceEURo_0_25(uglyEURo );
      decimal niceEURo = NiceEURo_0_50(uglyEURo );

      return KuneIzEURa_HRD_(niceEURo);
   }

   #region Round Up OR Down ToNearest

   public static decimal RoundUpToNearest(decimal passednumber, decimal roundto)
   {
      // 105.5 up to nearest 1 = 106
      // 105.5 up to nearest 10 = 110
      // 105.5 up to nearest 7 = 112
      // 105.5 up to nearest 100 = 200
      // 105.5 up to nearest 0.2 = 105.6
      // 105.5 up to nearest 0.3 = 105.6

      //if no rounto then just pass original number back
      if(roundto == 0)
      {
         return passednumber;
      }
      else
      {
         return Math.Ceiling(passednumber / roundto) * roundto;
      }
   }

   public static decimal RoundDownToNearest(decimal passednumber, decimal roundto)
   {
      // 105.5 down to nearest 1 = 105
      // 105.5 down to nearest 10 = 100
      // 105.5 down to nearest 7 = 105
      // 105.5 down to nearest 100 = 100
      // 105.5 down to nearest 0.2 = 105.4
      // 105.5 down to nearest 0.3 = 105.3

      //if no rounto then just pass original number back
      if(roundto == 0)
      {
         return passednumber;
      }
      else
      {
         return Math.Floor(passednumber / roundto) * roundto;
      }
   }

   #endregion Round Up OR Down ToNearest

   public static (decimal retMoney_KN, decimal retMoney_EUR) RetCalc_KN_OR_EUR(bool isEURinput, decimal fakMoney_EUR, decimal gotMoney)
   {
      decimal gotMoney_KN  = isEURinput ? KuneIzEURa_HRD_(gotMoney) : gotMoney                 ;
      decimal gotMoney_EUR = isEURinput ? gotMoney                  : EURiIzKuna_HRD_(gotMoney);

      decimal fakMoney_KN = KuneIzEURa_HRD_(fakMoney_EUR);

      decimal retMoney_KN  = gotMoney_KN  - fakMoney_KN ;
      decimal retMoney_EUR = gotMoney_EUR - fakMoney_EUR; 

      return (retMoney_KN.Ron2(), retMoney_EUR.Ron2());
   }

   public static (decimal retMoney_KN, decimal retMoney_EUR) RetCalc_KN_PLUS_EUR(bool isEURinput, decimal fakMoney_EUR, decimal gotMoney, bool isNewEUR, decimal NEWretMoney)
   {
      decimal gotMoney_KN  = isEURinput ? KuneIzEURa_HRD_(gotMoney) : gotMoney                 ;
      decimal gotMoney_EUR = isEURinput ? gotMoney                  : EURiIzKuna_HRD_(gotMoney);

      decimal fakMoney_KN = KuneIzEURa_HRD_(fakMoney_EUR);

      decimal ORIGretMoney_KN  = gotMoney_KN  - fakMoney_KN ;
      decimal ORIGretMoney_EUR = gotMoney_EUR - fakMoney_EUR; 

      decimal NEWretMoney_KN ;
      decimal NEWretMoney_EUR; 

      if(isNewEUR) // interveniramo u EUR iznos povrata 
      {
         NEWretMoney_EUR = NEWretMoney;
         NEWretMoney_KN  = KuneIzEURa_HRD_(ORIGretMoney_EUR - NEWretMoney_EUR);
      }
      else         // interveniramo u KN iznos povrata 
      {
         NEWretMoney_KN  = NEWretMoney;
         NEWretMoney_EUR = EURiIzKuna_HRD_(ORIGretMoney_KN - NEWretMoney_KN);
      }

      // HRD_Ron2()_Foobar: 
      if(NEWretMoney_EUR.IsZero()) NEWretMoney_KN  = ORIGretMoney_KN ;
      if(NEWretMoney_KN .IsZero()) NEWretMoney_EUR = ORIGretMoney_EUR;

      if(NEWretMoney_KN.IsNegative() || NEWretMoney_EUR.IsNegative())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Jedan od iznosa koji treba uzvratiti je negativan.\n\nUnijeli ste krivu kombinaciju plaćanja!");
       //return (ORIGretMoney_KN, ORIGretMoney_EUR);
         return (              0, ORIGretMoney_EUR);
      }

      return (NEWretMoney_KN.Ron2(), NEWretMoney_EUR.Ron2());
   }

   public static string EURorHRKstr 
   { 
      get 
      {
         if(projectYearAsInt <= 2022) return "HRK";
         else                         return "EUR";
      } 
   }

   public static ValutaNameEnum EURorHRK_NameEnum
   { 
      get 
      {
         if(projectYearAsInt <= 2022) return ValutaNameEnum.HRK;
         else                         return ValutaNameEnum.EUR;
      } 
   }

   public static bool IsEURoERA_projectYear
   {
      get
      {
         if(IsManyYearDB) return true;

         if(projectYearAsInt <= 2022) return false;
         else                         return true ;
      }
   }

   public static bool IsKuneERA_projectYear
   {
      get
      {
         if(IsManyYearDB) return false;

         if(projectYearAsInt <= 2022) return true ;
         else                         return false;
      }
   }

   #endregion 2022 ---> 2023 ... Kuna 2 EUR prelazak 

   public static bool DoesThisStringEndsWithTwoDigits(string theString)
   {
      if(theString.IsEmpty())  return false;
      if(theString.Length < 3) return false;

      int idxOfLastCharacter    = theString.Length - 1  ;
      int idxOfBefLastCharacter = idxOfLastCharacter - 1;

      bool isLastChDigit    = Char.IsDigit(theString[idxOfLastCharacter   ]);
      bool isBefLastChDigit = Char.IsDigit(theString[idxOfBefLastCharacter]);

      return isLastChDigit && isBefLastChDigit;
   }

   // NOTE BENE!!! nece se moci mjesati GB i TB jer MOD DUC matematika bude radila samo sa GB-ovima. 
   internal static string ModifyPCK_ArtiklName(string origName, decimal newRAM, decimal newHDD, string RAMjm, string HDDjm)
   {
      string newName = "";

      if(RAMjm.IsEmpty()) RAMjm = "GB";
      if(HDDjm.IsEmpty()) HDDjm = "GB";

      string[] tokens = origName.Split(new string[] { "GB", "gb", "Gb", "gB", "TB", "tb", "Tb", "tB" }, 3, StringSplitOptions.RemoveEmptyEntries);

    //if(tokens.Length < 3) return origName;
      if(tokens.Length < 2)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Orig naziv artikla [{0}] nema navedene i RAM i HDD GB kapacitete!", origName);
         return origName;
      }
      string oldRAMstr = "";
      string oldHDDstr = "";

      string token;

      token = tokens[0].TrimEnd(' ');
      for(int i = token.Length - 1; i >= 0; --i)
      {
         if(!Char.IsDigit(token[i])) break;
         oldRAMstr = token[i].ToString() + oldRAMstr;
      }
      newName += token.Replace(oldRAMstr, newRAM.ToString0Vv_NoGroup()) + /*"GB"*/RAMjm;

      token = tokens[1].TrimEnd(' ');
      for(int i = token.Length - 1; i >= 0; --i)
      {
         if(!Char.IsDigit(token[i])) break;
         oldHDDstr = token[i].ToString() + oldHDDstr;
      }
      newName += token.Replace(oldHDDstr, newHDD.ToString0Vv_NoGroup()) + /*"GB"*/HDDjm;

      if(tokens.Length > 2) newName += tokens[2];

      return newName;
   }

   public static bool IsThisUintDvoznamenkast(uint theUint)
   {
      return theUint > 9 && theUint < 100;
   }

   public static VvLookUpItem GetTetragram_PreferredSkladCD_LookUpItem(/*string userName*/)
   {
      if(ZXC.IsTETRAGRAM_ANY == false) return null;

      string userName = ZXC.CURR_userName;

      if(userName.Length < 3) return null;

      string[] tokens = userName.Split(new char[] { '-' });

      if(tokens.Length != 2) return null;

      string city_token = tokens[1];

      VvLookUpItem skladLUI = null;

      switch(city_token)
      {
         case "zg": skladLUI = ZXC.luiListaSkladista.FirstOrDefault(lui => lui.Cd.StartsWith("Z") && lui.Name.ToLower().Contains("glavno")); break;
         case "st": skladLUI = ZXC.luiListaSkladista.FirstOrDefault(lui => lui.Cd.StartsWith("S") && lui.Name.ToLower().Contains("glavno")); break;
         case "os": skladLUI = ZXC.luiListaSkladista.FirstOrDefault(lui => lui.Cd.StartsWith("O") && lui.Name.ToLower().Contains("glavno")); break;
         case "ri": skladLUI = ZXC.luiListaSkladista.FirstOrDefault(lui => lui.Cd.StartsWith("R") && lui.Name.ToLower().Contains("glavno")); break;

         default: return null;
      }

      return skladLUI;
   }

   public static bool IsSifrar_And_WeAreInPGyear(VvDataRecord vvDataRecord)
   {
      return vvDataRecord.IsSifrar && ZXC.projectYearFirstDay.Year != DateTime.Now.Year && vvDataRecord.IsPUG_NonPrjkt_DataRecord;
   }

   public static bool Does_NY_dbExists(XSqlConnection thisProjectYear_conn)
   {
      int theNY = DateTime.Now.Year;

      string NY_dbName = ZXC.VvDB_NameConstructor(theNY.ToString(), ZXC.CURR_prjkt_rec.Ticker, ZXC.CURR_prjkt_rec.KupdobCD);

      bool NY_dbExists = VvSQL.CHECK_DATABASE_EXISTS(thisProjectYear_conn, NY_dbName);

      return NY_dbExists;
   }

   public static bool Does_NY_tableExists(XSqlConnection thisProjectYear_conn, string recordName)
   {
      int theNY = DateTime.Now.Year;

      string NY_dbName = ZXC.VvDB_NameConstructor(theNY.ToString(), ZXC.CURR_prjkt_rec.Ticker, ZXC.CURR_prjkt_rec.KupdobCD);

      bool NY_dbExists = VvSQL.CHECK_TABLE_EXISTS(thisProjectYear_conn, NY_dbName, recordName);

      return NY_dbExists;
   }

   public static uint Base10TtNumBuffer(uint numOfDigitsAvailable)
   {
      // za numOfDigitsAvailable 2 ---> return     100 
      // za numOfDigitsAvailable 3 ---> return    1000 
      // za numOfDigitsAvailable 4 ---> return   10000 
      // za numOfDigitsAvailable 5 ---> return  100000 
      // za numOfDigitsAvailable 6 ---> return 1000000 

      return (uint)Math.Pow(10, numOfDigitsAvailable);
   }

   public static uint Get_YYandRecID(int year, uint recID)
   {
      // Input parameters are some DateTime.Year and some sql recID uint.
      // Return value, YYandRecID, should be composite uint where 2 digit year is farmost left and the remaining digits stores recID,
      // but using 9 digits placeholder
      // Example: Get_YYandRecID(2023, 123) yields 230000123

      // Get last two digits of year
      int yy = year % 100;
      // Compose the result: YY as leftmost 2 digits, recID as rightmost 7 digits
      uint YYandRecID = (uint)(yy * Base10TtNumBuffer(7) + (recID % Base10TtNumBuffer(7)));
      return YYandRecID;
   }

   public static (int year, uint recID) GetYearAndRecIDFrom_YYandRecID(uint YYandRecID)
   {
      // Extract the 2-digit year (YY) from the leftmost 2 digits
      int yy = (int)(YYandRecID / Base10TtNumBuffer(7));
    
      // Extract the recID from the rightmost 7 digits
      uint recID = YYandRecID % Base10TtNumBuffer(7);
      
      // Compose the full year in 20YY format
      int year = 2000 + yy;
      return (year, recID);
   }

   #endregion Util Methods

   #region Extension Methods (Came with C# 3.0)

   #region IsZero, IsPozitive, ... (decimal, uint, int, ushort)

   public static int NotNeg(this int value)
   {
      return value > 0 ? value : 0;
   }

   //puse: 
   //public static bool IsZero(decimal value)
   //{
   //   return value == decimal.Zero;
   //}

   //puse: 
   //public static bool NotZero(decimal value)
   //{
   //   return !IsZero(value);
   //}

   public static bool IsZero(this double value)
   {
      return value == /*double.Zero*/ 0;
   }

   public static bool IsZero(this decimal value)
   {
      return value == decimal.Zero;
   }

   public static bool AlmostZero(this decimal value)
   {
      //return value == 0;
      return Math.Abs(value) <= 0.01M;
   }

   public static bool AlmostZero(this decimal value, decimal tolerancy)
   {
      //return value == 0;
      return Math.Abs(value) <= tolerancy;
   }

   public static decimal ZeroIfAlmostZero(this decimal value)
   {
      if(value.AlmostZero()) return 0.00M;
      else                   return value;
   }

   public static bool IsNegative(this decimal value)
   {
      return value < decimal.Zero;
   }

   public static bool IsZeroOrPositive(this decimal value)
   {
      return !IsNegative(value);
   }

   public static bool IsZeroOrNegative(this decimal value)
   {
      return !IsPositive(value);
   }

   public static bool IsPositive(this decimal value)
   {
      return value > decimal.Zero;
   }

   public static bool NotZero(this decimal value)
   {
      return value != decimal.Zero;
   }

   public static bool IsZero(this uint value)
   {
      return value == 0;
   }

   public static bool AlmostZero(this uint value)
   {
      //return value == 0;
      return Math.Abs(value) <= 0.01M;
   }

   public static bool IsNegative(this uint value)
   {
      return value < 0;
   }

   public static bool IsZeroOrPositive(this uint value)
   {
      return !IsNegative(value);
   }

   public static bool IsZeroOrNegative(this uint value)
   {
      return !IsPositive(value);
   }

   public static bool IsPositive(this uint value)
   {
      return value > 0;
   }

   public static bool NotZero(this uint value)
   {
      return value != 0;
   }

   public static bool IsZero(this int value)
   {
      return value == 0;
   }

   public static bool NotZero(this long value)
   {
      return value != 0;
   }
   public static bool IsZero(this long value)
   {
      return value == 0;
   }

   public static bool NotZero(this long? value)
   {
      return value != null && value != 0;
   }
   public static bool IsZero(this long? value)
   {
      return value == null || value == 0;
   }

   public static bool IsNegative(this int value)
   {
      return value < 0;
   }

   public static bool IsZeroOrPositive(this int value)
   {
      return !IsNegative(value);
   }

   public static bool IsZeroOrNegative(this int value)
   {
      return !IsPositive(value);
   }

   public static bool IsPositive(this int value)
   {
      return value > 0;
   }

   public static bool MoreThenOne(this int value)
   {
      return value > 1;
   }

   public static bool NotZero(this int value)
   {
      return value != 0;
   }

   public static bool IsZero(this ushort value)
   {
      return value == 0;
   }

   public static bool IsNegative(this ushort value)
   {
      return value < 0;
   }

   public static bool IsZeroOrPositive(this ushort value)
   {
      return !IsNegative(value);
   }

   public static bool IsZeroOrNegative(this ushort value)
   {
      return !IsPositive(value);
   }

   public static bool IsPositive(this ushort value)
   {
      return value > 0;
   }

   public static bool NotZero(this ushort value)
   {
      return value != 0;
   }

   public static bool IsZero(this short value)
   {
      return value == 0;
   }

   public static bool IsNegative(this short value)
   {
      return value < 0;
   }

   public static bool IsZeroOrPositive(this short value)
   {
      return !IsNegative(value);
   }

   public static bool IsZeroOrNegative(this short value)
   {
      return !IsPositive(value);
   }

   public static bool IsPositive(this short value)
   {
      return value > 0;
   }

   public static bool NotZero(this short value)
   {
      return value != 0;
   }

   //puse: 
   //public static bool NotEmpty(string theString)
   //{
   //   return !String.IsNullOrEmpty(theString);
   //}

   #endregion IsZero, IsPozitive, ... (decimal, uint, int, ushort)

   //It is a common practice to store passwords in databases using a hash. MD5 (defined in RFC 1321) is a common hash algorithm, and using it from C# is easy.
   //Here’s an implementation of a method that converts a string to an MD5 hash, which is a 32-character string of hexadecimal numbers.
   public static string VvCalculateMD5(this string theString)
   {
      // step 1, calculate MD5 hash from input
      MD5 md5 = System.Security.Cryptography.MD5.Create();
      byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(theString);
      byte[] hash = md5.ComputeHash(inputBytes);

      // step 2, convert byte array to hex string
      StringBuilder sb = new StringBuilder();
      for(int i = 0; i < hash.Length; i++)
      {
         sb.Append(hash[i].ToString("x2"));
      }
      return sb.ToString();

      //To make the hex string use lower-case letters instead of upper-case, replace the single line inside the for loop with this line:
      //sb.Append(hash[i].ToString("x2"));   
   }

   public static bool IsEmpty(this string theString)
   {
      // 17.03.2017: !!! BIG NEWS !!! BIG BIG NEWS !!! 
    //return String.IsNullOrEmpty     (theString);     
      return String.IsNullOrWhiteSpace(theString);
   }

   public static bool NotEmpty(this string theString)
   {
      //return !String.IsNullOrEmpty(theString);
      return !IsEmpty(theString);
   }

   public static bool IsValidOIB(this string theString)
   {
      //return !String.IsNullOrEmpty(theString);
      
      return !IsEmpty(theString) && theString.Length > 4 && !theString.Contains('.');
   }

   public static bool IsEmpty(this DateTime theDate)
   {
      return !NotEmpty(theDate);
   }

   public static bool NotEmpty(this DateTime theDate)
   {
      if(theDate == null || theDate == DateTime.MinValue || theDate == DateTimePicker.MinimumDateTime)
         return false;
      else
         return true;
   }

   public static int VvDokDate_YY(this DateTime theDate)
   {
      return theDate.IsEmpty() ? ZXC.NowYearFirstDay.Year - 2000 : theDate.Year - 2000;
   }

   public static DateTime DayBefore(this DateTime theDate)
   {
      return theDate.Date - ZXC.OneDaySpan;
   }

   public static DateTime PrevMonthFirstDay(this DateTime theDate)
   {
      if(theDate == null || theDate == DateTime.MinValue || theDate == DateTimePicker.MinimumDateTime)
         return DateTime.MinValue;

      int thisMonth = theDate.Month;

      // 05.12.2016: 
    //if(thisMonth == 1) return DateTime.MinValue;
      if(thisMonth == 1) return new DateTime(theDate.Year - 1, 12, 01); 

      return new DateTime(theDate.Year, thisMonth - 1, 1);
   }

   public static DateTime PrevMonthLastDay(this DateTime theDate)
   {
      if(theDate == null || theDate == DateTime.MinValue || theDate == DateTimePicker.MinimumDateTime)
         return DateTime.MinValue;

      int thisMonth = theDate.Month;

      // 05.12.2016: 
    //if(thisMonth == 1) return DateTime.MinValue;
      if(thisMonth == 1) return new DateTime(theDate.Year - 1, 12, 31); 

      return new DateTime(theDate.Year, thisMonth - 1, DateTime.DaysInMonth(theDate.Year, theDate.Month -1));
   }

   public static DateTime ThisMonthFirstDay(this DateTime theDate)
   {
      if(theDate == null || theDate == DateTime.MinValue || theDate == DateTimePicker.MinimumDateTime)
         return DateTime.MinValue;

      return new DateTime(theDate.Year, theDate.Month, 1);
   }

   public static DateTime ThisMonthLastDay(this DateTime theDate)
   {
      if(theDate == null || theDate == DateTime.MinValue || theDate == DateTimePicker.MinimumDateTime)
         return DateTime.MinValue;

      return new DateTime(theDate.Year, theDate.Month, DateTime.DaysInMonth(theDate.Year, theDate.Month));
   }

   public static TimeSpan The_23_59_ts = new TimeSpan(23, 59, 59);

   public static DateTime EndOfDay(this DateTime theDate)
   {
      return theDate.Date + The_23_59_ts;
   }

   public static bool IsSunday(this DateTime theDate)
   {
      return theDate.DayOfWeek == DayOfWeek.Sunday;
   }

   public static bool IsFromTodayMMYYYY(this DateTime theDate)
   {
      return theDate.Year == DateTime.Today.Year && theDate.Month == DateTime.Today.Month;
   }

   public static VvLookUpItem GetRandomItem(this List<VvLookUpItem> theList, Random random)
   {
      return theList[random.Next(theList.Count)];
   }

   public static IEnumerable<TResult> Rollup<TSource, TResult>(
       this IEnumerable<TSource> source,
       TResult seed,
       Func<TSource, TResult, TResult> projection)
   {
      TResult nextSeed = seed;
      foreach(TSource src in source)
      {
         TResult projectedValue = projection(src, nextSeed);
         nextSeed = projectedValue;
         yield return projectedValue;
      }
   }

   public static bool IsEmpty<T>(this List<T> theList)
   {
      if(theList == null) return true;

      return theList.Count < 1;
   }

   public static bool NotEmpty<T>(this List<T> theList)
   {
      return theList != null && theList.Count > 0;
   }

   public static string SubstringSafe(this string text, int start, int length)
   {
      if (start >= text.Length)
         return "";            
      if (start + length > text.Length)
         length = text.Length - start;         
      if (length.IsNegative())
         length = 0;         
      return text.Substring(start, length);
   }

   public static string SubstringSafe(this string text, int start)
   {
      if (start >= text.Length)
         return "";            
    //if (start + length > text.Length)
    //   length = text.Length - start;         
    //if (length.IsNegative())
    //   length = 0;         
      return text.Substring(start/*, length*/);
   }

   public static string RbrMjUkvartalu(this DateTime theDate)
   {
      if(theDate.Month == 1 || theDate.Month == 4 || theDate.Month == 7 || theDate.Month == 10) return "Prvi";
      if(theDate.Month == 2 || theDate.Month == 5 || theDate.Month == 8 || theDate.Month == 11) return "Drugi";
      if(theDate.Month == 3 || theDate.Month == 6 || theDate.Month == 9 || theDate.Month == 12) return "Treći";

      return "QAZ mj u kvartalu";
   }

   public static string AsUTF8(this string text)
   {
      byte[] bytes = Encoding.Default.GetBytes(text);

      return Encoding.UTF8.GetString(bytes); 
   }

   public static int CountOfSignificantDecimalPlaces(this decimal n)
   {
      n = Math.Abs(n); //make sure it is positive.
      n -= (int)n;     //remove the integer part of the number.
      var decimalPlaces = 0;
      while(n > 0)
      {
         decimalPlaces++;
         n *= 10;
         n -= (int)n;
      }
      return decimalPlaces;
   }

   #region GetSafe XSqlDataReader reader data

   public static string GetSafeString(this XSqlDataReader reader, int colIdx)
   {
      if(reader.IsDBNull(colIdx)) return "";

      return reader.GetString(colIdx);
   }

   public static decimal GetSafeDecimal(this XSqlDataReader reader, int colIdx)
   {
      if(reader.IsDBNull(colIdx)) return 0M;

      return reader.GetDecimal(colIdx);
   }

   public static int GetSafeInt32(this XSqlDataReader reader, int colIdx)
   {
      if(reader.IsDBNull(colIdx)) return 0;

      return reader.GetInt32(colIdx);
   }

   public static uint GetSafeUInt32(this XSqlDataReader reader, int colIdx)
   {
      if(reader.IsDBNull(colIdx)) return 0;

      return reader.GetUInt32(colIdx);
   }

   public static DateTime GetSafeDateTime(this XSqlDataReader reader, int colIdx)
   {
      if(reader.IsDBNull(colIdx)) return DateTime.MinValue;

      return reader.GetDateTime(colIdx);
   }

   #endregion GetSafe XSqlDataReader reader data

   public static string FirstLetterToUpperCase(this string str)
   {
      if(str.IsEmpty()) return str;

      return char.ToUpper(str[0]) + str.Substring(1);
   }

   public static string NullSafe(this string str)
   {
      if(str.IsEmpty()) return "";

      return str;
   }

   public static VvUserControl GetParent_VvUserControl(this Control _control)
   {
      Control parentControl = _control.Parent;

      if(parentControl == null) return null;

      if(parentControl is VvUserControl) return parentControl as VvUserControl;

      return GetParent_VvUserControl(parentControl);
   }

   public static bool In<T>(this T @object, params T[] values)
   {
      // this is LINQ expression. If you don't want to use LINQ,
      // you can use a simple foreach and return true 
      // if object is found in the array
      return values.Contains(@object);
   }

   public static bool In<T>(this T @object, IEnumerable<T> valueList)
   {
      // this is LINQ expression. If you don't want to use LINQ,
      // you can use a simple foreach and return true if object 
      // is found in the array
      return valueList.Contains(@object);
   }

   public static bool IsFaktur_IRA(this VvDataRecord theVvDataRecord)
   {
      if(theVvDataRecord is Faktur) return false;

      Faktur faktur_rec = (theVvDataRecord as Faktur);

      if(faktur_rec.TT != Faktur.TT_IRA) return false;

      return true;
   }

   public static bool IsOdd(this int number)
   {
      return (number % 2).NotZero();
   }

   public static bool IsEven(this int number)
   {
      return (number % 2).IsZero();
   }

   #endregion Extension Methods (Came with C# 3.0)

   #region TtInfo

   public static Dictionary<string, TtInfo> RiskTT;
   public static TtInfo[]                   TtInfoArray;

   public static string[] TtInfoArtstatInfluencers      { get { return ZXC.TtInfoArray.Where(tti => tti.IsArtiklStatusInfluencer       ).Select(tti => tti.TheTT).ToArray(); } }
   public static string[] TtInfoDokCijShouldBePrNabCij  { get { return ZXC.TtInfoArray.Where(tti => tti.IsDokCijShouldBePrNabCij       ).Select(tti => tti.TheTT).ToArray(); } }
   public static string[] TtInfoDokKolShouldBePrevKolSt { get { return ZXC.TtInfoArray.Where(tti => tti.IsDokKolShouldBePrevKolStanjeTT).Select(tti => tti.TheTT).ToArray(); } }
   public static string[] TtInfoPrihod                  { get { return ZXC.TtInfoArray.Where(tti => tti.IsPrihodTT                     ).Select(tti => tti.TheTT).ToArray(); } }

   public static string[] TtUlazKolArray      { get { return ZXC.TtInfoArray.Where(tti => tti.IsFinKol_PS || tti.IsFinKol_U).Select(tti => tti.TheTT).ToArray(); } }
   public static string[] TtIzlazKolArray     { get { return ZXC.TtInfoArray.Where(tti =>                    tti.IsFinKol_I).Select(tti => tti.TheTT).ToArray(); } }
   public static string[] TtUlazIzlazKolArray { get { return ZXC.TtInfoArray.Where(tti => tti.IsFinKol_PS || tti.IsFinKol_U
                                                                                                          || tti.IsFinKol_I).Select(tti => tti.TheTT).ToArray(); } }

   public static TtInfo TtInfo(string theTT)
   {
      if(theTT.IsEmpty() || theTT == " ") return new TtInfo();

      try
      {
         TtInfo theTtInfo = ZXC.RiskTT[theTT];

         if(ZXC.IsSvDUH)
         {
            if(theTT == Faktur.TT_URA) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_URA_SVD;
            if(theTT == Faktur.TT_NRD) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_NRD_SVD;
            if(theTT == Faktur.TT_IZD) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_IZD_SVD;
         }

         if(ZXC.IsPCTOGO)
         {
            if(theTT == Faktur.TT_PRI) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_PRI_PTG;
            if(theTT == Faktur.TT_IZD) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_IZD_PTG;
            if(theTT == Faktur.TT_MSI) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_MSI_PTG;
            if(theTT == Faktur.TT_PST) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_PST_PTG;
            if(theTT == Faktur.TT_URA) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_URA_PTG;
            if(theTT == Faktur.TT_IRA) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_IRA_PTG;
            
            if(theTT == Faktur.TT_DI2) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_DOD_PTG;
            if(theTT == Faktur.TT_PV2) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_PVR_PTG;
            if(theTT == Faktur.TT_ZI2) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_ZIZ_PTG;
            if(theTT == Faktur.TT_UG2) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_UGO_PTG;
            if(theTT == Faktur.TT_AU2) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_ANU_PTG;
         }

         if(ZXC.IsTETRAGRAM_ANY)
         {
            if(theTT == Faktur.TT_PON) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_PON_MPC;
            if(theTT == Faktur.TT_OPN) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_OPN_MPC;
            if(theTT == Faktur.TT_IRA) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_IRA_MPC;
            if(theTT == Faktur.TT_IZD) theTtInfo.DefaultSubModulEnum = VvSubModulEnum.R_IZD_MPC;
         }

         return theTtInfo;
      }
      catch(Exception)
      {
         return new TtInfo();
      }
   }

   public static TtInfo TtInfo(short theTtSort)
   {
      if(theTtSort.IsZero()) return new TtInfo();

      try
      {
         // 18.12.2015: uvodimo po prvi put da vise od 1 TT-a moze imati isti ttSort (npr. INV, INM) 
       //return ZXC.RiskTT.SingleOrDefault(rtt => rtt.Value.TtSort == theTtSort).Value;
         return ZXC.RiskTT.FirstOrDefault(rtt => rtt.Value.TtSort == theTtSort).Value;
      }
      catch(Exception)
      {
         return new TtInfo();
      }
   }

   #endregion TtInfo

   #region MoneyToText and IntToText

   /****************************************************************************/
   /************************ NUM2TEXT ... **************************************/
   /****************************************************************************/

   internal static string KuneIlipe(decimal money)
   {
      string kuneTxt, lipeTxt;
      decimal lipe = (money.Ron2() - Math.Floor(money.Ron2())) * 100;

      kuneTxt = num2text(money);
      lipeTxt = num2text(lipe);

      string knOrEur  = projectYearAsInt < 2023 ? "kuna i " : "eura i ";
      string lpOrCent = projectYearAsInt < 2023 ? "lipa"    : "centa"  ;

    //return kuneTxt + "kuna i " + lipeTxt + "lipa"  ;
      return kuneTxt + knOrEur   + lipeTxt + lpOrCent;
   }

internal static string num2text(decimal num)
//char *text;
//double num;
{
   if(num.IsZero()) return "nula ";

   string theText="";
   string numx;
   char[] trice;
   string[] ended_trice = new string[5];
   int i;
   int j;
   int tri = 0;
   int len;
   numx = string.Format("{0:f0}", Math.Floor(num));
   len = numx.Length;
   trice = new char[3];
   for(i = len - 1, j = 0; i >= 0; --i)
   {
      trice[2 - j] = numx[i];
	  if(((j+1)%3==0) || i == 0)
	  {
		 set3(ref ended_trice[tri], trice, tri);
		 tri++;
       trice = new char[3];
		 j = 0;
	  }
	  else
		  j++;
   }
   for(i = tri-1; i>=0; i--)
      theText += ended_trice[i];

   return(theText);
}

private static int set3(ref string ended_komplet, char[] trice, int komplet)
//char *ended_komplet, *trice;
//int komplet; /* 0-jedinice, 1-tisucice, 2-mil., ...*/
{
   string deset = "";
   string sto = "";
   string naz = "";

   ended_komplet = "";

   switch(komplet)
   {
	  /* jedinice */
 case 0: desetice(ref deset, trice.Skip(1).ToArray(), 0);	 break;
	  /* tisucice */
 case 1: desetice(ref deset, trice.Skip(1).ToArray(), 2);	 break;
	  /* milijuni */
 case 2: desetice(ref deset, trice.Skip(1).ToArray(), 1);	 break;
	  /* milijarde*/
 case 3: desetice(ref deset, trice.Skip(1).ToArray(), 2);	 break;

 default:		  break;

   }
   naziv(ref naz, trice, komplet);
   stotice(ref sto, trice[0]);
   if(sto.Length.NotZero())
   {
	  ended_komplet += sto;
	  ended_komplet += " ";
   }
   if(deset.Length.NotZero())
   {
	  ended_komplet += deset;
	  ended_komplet += " ";
   }
   if(naz.Length.NotZero())
   {
	  ended_komplet += naz;
	  ended_komplet += " ";
   }
   return (0);
}

private static void stotice(ref string text, char num)
//char *text, num; /* iznos stotica */
{

   text="";
   switch(num)
   {
	  case '1' :		  text = "sto";		   break;
	  case '2' :		  text = "dvijesto";	break;
	  case '3' :		  text = "tristo";		break;
	  case '4' :		  text = "četristo";	break;
	  case '5' :		  text = "petsto";		break;
	  case '6' :		  text = "šesto";		break;
	  case '7' :		  text = "sedamsto";	break;
	  case '8' :		  text = "osamsto";	break;
	  case '9' :		  text = "devetsto"; break;
	  case '\0':
     case '0' : 	  text = "";			  break;
   }
   return;
}
private static int naziv(ref string naz, char[] trice, int komplet)
//char *naz, *trice;
//int komplet; /* 0-jedinice, 1-tisucice, 2-mil., ...*/
{
   int i;
   int err = 0;

   naz = "";
   for(i = 0;i<3;i++)
	   if(/*trice[i] &&*/ trice[i]!='0')
		   err = 1;
   if(err == 0 || komplet == 0)
   {
	   naz = "";
	   return(0);
   }
   switch(komplet)
   {
	  /* tisucice */
 case 1:
	 if(trice[1] == '1')
		 naz = "tisuća";
							 else
							 {
								switch(trice[2])
								{
								   case '0':
									   case '1':
										   case '5':
											   case '6':
								   case '7':
									   case '8':
										   case '9':
									naz = "tisuća";
									break;
								   case '2':
									   case '3':
										   case '4':
									naz = "tisuće";
									break;
								}
							 }
							 break;
	  /* milijuni */
 case 2:
	 if(trice[1] == '1')
		 naz = "milijuna";
							 else
							 {
								switch(trice[2])
								{
								   case '1':
									naz = "milijun";
									break;
								   case '0':
									   case '2':
										   case '3':
											   case '4':
								   case '5':
									   case '6':
										   case '7':
											   case '8':
								   case '9':
									naz = "milijuna";
									break;
								}
							 }
							 break;
	  /* milijarde*/
 case 3:
	 if(trice[1] == '1')
		 naz = "milijardi";
							 else
							 {
								switch(trice[2])
								{
								   case '1':
									naz = "milijarda";
									break;
								   case '0':
									   case '5':
										   case '6':
											   case '7':
								   case '8':
									   case '9':
									naz = "milijardi";
									break;
								   case '2':
									   case '3':
										   case '4':
									naz = "milijarde";
									break;
								}
							 }
							 break;
	  default:
		  break;
   }
   return (0);
}

private static string broj(ref string text, char num, int spol)
//char *text, num;
//int spol; /* 0-jedinice, 1-muski, 2-zenski */
{
   switch(num)
   {
		 case '1':
			 if(spol == 1)
				 text = "jedan";
				   else
					   text = "jedna";
					   break;
		 case '2':
			 if(spol == 1)
				 text = "dva";
				   else
					   text = "dvije";
					   break;
		 case '3':			 text = "tri";			 break;
		 case '4':			 text = "četiri";			 break;
		 case '5':			 text = "pet";			 break;
		 case '6':			 text = "šest";			 break;
		 case '7':			 text = "sedam";			 break;
		 case '8':			 text = "osam";			 break;
		 case '9':			 text = "devet";			 break;
		 case '0':			 text = "";			 break;
   }
   return("");
}

private static int desetice(ref string deset, char[] trica, int spol)
//char *deset, *trica; /* zadnje dvije znamenke */
//int spol; /* 0-jedinice, 1-muski, 2-zenski */
{
   int num;
   //double atof = ;
   string text="";
   string dtext="";

   deset="";

   if(trica.Length == 2)
	   num = ZXC.ValOrZero_Int(new string(trica));
   else // nema desetica samo su jedinice
      num = ZXC.ValOrZero_Int(trica[1].ToString());
   if(num < 10)
   {
	  broj(ref deset, trica[1], spol);
   } 
   else if(num > 9 && num < 20)
   {
	  switch((int)num)
	  {
		 case 10:			 deset = "deset";			 break;
		 case 11:			 deset = "jedanaest";			 break;
		 case 12:			 deset = "dvanaest";			 break;
		 case 13:			 deset = "trinaest";			 break;
		 case 14:			 deset = "četrnaest";			 break;
		 case 15:			 deset = "petnaest";			 break;
		 case 16:			 deset = "šesnaest";			 break;
		 case 17:			 deset = "sedamnaest";			 break;
		 case 18:			 deset = "osamnaest";			 break;
		 case 19:			 deset = "devetnaest";			 break;
	  }
   } 
   else
   {
	  switch(trica[0])
	  {
		 case '2':			 dtext = "dva";			 break;
		 case '3':			 dtext = "tri";			 break;
		 case '4':			 dtext = "četr";			 break;
		 case '5':			 dtext = "pe";			 break;
		 case '6':			 dtext = "šez";			 break;
		 case '7':			 dtext = "sedam";			 break;
		 case '8':			 dtext = "osam";			 break;
		 case '9':			 dtext = "deve";			 break;
	  }
	  broj(ref text, trica[1], spol);
	  deset = string.Format("{0}deset{1}", dtext, text);
   }
   return(0);
}

/****************************************************************************/
// IntAsText trenutno za danaPutNl a moyda i za nesto drugo muskog spola
   internal static string IntAsText(int _num) // dana na slPutu
   {
      string  intAsText;
      string dana = "dan";
      decimal num  = (decimal)_num;

      intAsText = num2text4int(num);
      if(_num == 1 || (_num != 11 && (_num.ToString()).EndsWith("1"))) dana = "dan";
      else                                                             dana = "dana";

      return intAsText + dana;
   }
 
   internal static string num2text4int(decimal num)
   //char *text;
   //double num;
   {
      if(num.IsZero()) return "nula ";

      string theText="";
      string numx;
      char[] trice;
      string[] ended_trice = new string[5];
      int i;
      int j;
      int tri = 0;
      int len;
      numx = string.Format("{0:f0}", Math.Floor(num));
      len = numx.Length;
      trice = new char[3];
      for(i = len - 1, j = 0; i >= 0; --i)
      {
         trice[2 - j] = numx[i];
	     if(((j+1)%3==0) || i == 0)
	     {
		    set3forInt(ref ended_trice[tri], trice, tri);
		    tri++;
          trice = new char[3];
		    j = 0;
	     }
	     else
		     j++;
      }
      for(i = tri-1; i>=0; i--)
         theText += ended_trice[i];

      return(theText);
   }

   private static int set3forInt(ref string ended_komplet, char[] trice, int komplet)
   //char *ended_komplet, *trice;
   //int komplet; /* 0-jedinice, 1-tisucice, 2-mil., ...*/
   {
      string deset = "";
      string sto = "";
      string naz = "";

      ended_komplet = "";

      switch(komplet)
      {
	     /* jedinice */
    case 0: desetice(ref deset, trice.Skip(1).ToArray(), 1);	 break;
	     /* tisucice */
    case 1: desetice(ref deset, trice.Skip(1).ToArray(), 2);	 break;
	     /* milijuni */
    case 2: desetice(ref deset, trice.Skip(1).ToArray(), 1);	 break;
	     /* milijarde*/
    case 3: desetice(ref deset, trice.Skip(1).ToArray(), 2);	 break;

    default:		  break;

      }
      naziv(ref naz, trice, komplet);
      stotice(ref sto, trice[0]);
      if(sto.Length.NotZero())
      {
	     ended_komplet += sto;
	     ended_komplet += " ";
      }
      if(deset.Length.NotZero())
      {
	     ended_komplet += deset;
	     ended_komplet += " ";
      }
      if(naz.Length.NotZero())
      {
	     ended_komplet += naz;
	     ended_komplet += " ";
      }
      return (0);
   }

   /****************************************************************************/
   /*********************** END OF NUM2TEXT ... ********************************/
   /****************************************************************************/

   #endregion MoneyToText

   #region TEXTHO - TEXTILEHOUSE 2 & 5 WEEK CYCLES

   // 27.11.2020: poslovnica(e) se seli iz 5 u 2 (tj. 3)
   public static string[] TH_skl_5u2 = new string[] { "34M5"/*, "98M5", "xyMz"*/ };
   // 07.05.2025: poslovnica(e) se seli iz 2 u 5
   // 15.01.2026: dodajemo i 68M2 (Kreso happy bDay!) 
 //public static string[] TH_skl_2u5 = new string[] { "54M2"        /*, "98M5", "xyMz"*/ };
   public static string[] TH_skl_2u5 = new string[] { "54M2", "68M2"/*, "98M5", "xyMz"*/ };

   public static bool IsSkladCD_THshop(string skladCD) // 
   {
      if(!IsTEXTHOany && !IsTEXTHOany2) return false;
      if(skladCD.Length != 4          ) return false;

      if(skladCD.VvLastChar() != '5' && skladCD.VvLastChar() != '2') return false;

      return true;
   }

   public static bool IsTH_DEAD_Shop(string skladCD)
   {
      return 
             (
                skladCD.StartsWith("42") || // Zaprešić                             
                skladCD.StartsWith("40") || // Križevci                             
                skladCD.StartsWith("44") || // Vukovar                              
                skladCD.StartsWith("46") || // Đakovo ... ?! mozda prerano?!        
                skladCD.StartsWith("50") || // Virovitica - STARA (nova je na '80') 
                skladCD.StartsWith("80") || // Virovitica - ode i ona               
                skladCD.StartsWith("62") || // Kutina                               
                skladCD.StartsWith("82") || // Sesvete                              
                skladCD.StartsWith("78") || // Split                                
                skladCD.StartsWith("76") || // Beli Manastir                        
                skladCD.StartsWith("34") || // Požega                               
                skladCD.StartsWith("48") || // Našice                               
                skladCD.StartsWith("86") || // Vlaška                               
                skladCD.StartsWith("18") || // Šubićeva 1                           
                skladCD.StartsWith("66")    // Nova Gradiška                        
             );
   }

   // 25.09.2018: nestaje 'IsTH_2WeekShop' te se razlucije na IsTH_2Week_PON_Shop i IsTH_2Week_SRI_Shop 
 //public static bool IsTH_2WeekShop(string skladCD) { return skladCD.EndsWith("2") == true; }
   public static bool IsTH_5WeekShop(string skladCD)
   {
      // 27.11.2020: poslovnica(e) se seli iz 5 u 2 (tj. 3)
      if(TH_skl_5u2.Contains(skladCD)) return false;

      if(TH_skl_2u5.Contains(skladCD)) return true;

      return skladCD.EndsWith("5") == true;
   }

   public static bool IsTH_2Week_PON_Shop(string skladCD)
   {
      // 07.01.2019: 
    //return skladCD.EndsWith("2") && !IsTH_2Week_SRI_Shop(skladCD);
      return false;
   }

   public static bool IsTH_2Week_SRI_Shop(string skladCD)
   {
      if(skladCD.EndsWith("5")) return false; // ziheraski 

      if(IsTH_5WeekShop     (skladCD) ||
         IsTH_3Week_SRI_Shop(skladCD)) return false;

      return true;
   }

   public static bool IsTH_3Week_SRI_Shop(string skladCD)  // 31.08.2020 
   {
      // 27.11.2020: poslovnica(e) se seli iz 5 u 2 (tj. 3)
      if(TH_skl_5u2.Contains(skladCD)) return true;
      
      if(TH_skl_2u5.Contains(skladCD)) return false;

      if(skladCD.EndsWith("5")) return false; // ziheraski 

      return skladCD.EndsWith("2") &&
             (
                skladCD.StartsWith("48") || // Nasice         
                skladCD.StartsWith("52") || // Sisak          
                skladCD.StartsWith("54") || // Karlovac       
                skladCD.StartsWith("56") || // Slavonski Brod 
                skladCD.StartsWith("66") || // Nova Gradiska  
                skladCD.StartsWith("38") || // Vinkovci        // od 07.10.2020.
                skladCD.StartsWith("68") || // Cakovec         // od 27.11.2020.
                skladCD.StartsWith("64") || // Bjelovar       
                skladCD.StartsWith("42") || // Zapresic       
                skladCD.StartsWith("36")    // Zabok  
             );
   }

   internal static bool IsTH_specialPeriod(DateTime dokDate)
   {
      DateTime AweekStart = new DateTime(2024, 04, 15);
      DateTime Aweek__End = new DateTime(2024, 04, 21, 23, 59, 59);

      DateTime BweekStart = new DateTime(2024, 05, 20);
      DateTime Bweek__End = new DateTime(2024, 05, 26, 23, 59, 59);

      DateTime CweekStart = new DateTime(2024, 07, 01);
      DateTime Cweek__End = new DateTime(2024, 07, 07, 23, 59, 59);

      DateTime DweekStart = new DateTime(2024, 08, 05);
      DateTime Dweek__End = new DateTime(2024, 08, 11, 23, 59, 59);

      return (dokDate >= AweekStart && dokDate <= Aweek__End) ||
             (dokDate >= BweekStart && dokDate <= Bweek__End) ||
             (dokDate >= CweekStart && dokDate <= Cweek__End) ||
             (dokDate >= DweekStart && dokDate <= Dweek__End)  ;
   }


   // 31.08.2020: 
   //public enum TH_ShopWeekKind { _5W = 0, _2W_PON = 1, _2W_SRI = 2 }
   //public enum TH_ShopWeekKind { _5W = 0, _2W_PON = 1, _2W_SRI = 2, _3W_SRI = 3 }
   public enum TH_ShopWeekKind { _5W = 0, _2W_PON = 1, _2W_SRI = 2, _3W_SRI = 3, _NOT_TH_SHOP_ = 4 }

   public static Dictionary<DateTime, TH_PriceRuleForCycleMoment> TH_Cjenik_Calendar_2Week_PON;
   public static Dictionary<DateTime, TH_PriceRuleForCycleMoment> TH_Cjenik_Calendar_2Week_SRI;
   public static Dictionary<DateTime, TH_PriceRuleForCycleMoment> TH_Cjenik_Calendar_5Week    ;
   public static Dictionary<DateTime, TH_PriceRuleForCycleMoment> TH_Cjenik_Calendar_3Week_SRI; // 31.08.2020 

   public static List<TH_PriceRuleForCycleMoment> TH_PriceRuleList_W2_PON_C2;
   public static List<TH_PriceRuleForCycleMoment> TH_PriceRuleList_W2_PON_C3;
   public static List<TH_PriceRuleForCycleMoment> TH_PriceRuleList_W2_SRI_C2;
   public static List<TH_PriceRuleForCycleMoment> TH_PriceRuleList_W2_SRI_C3;
   public static List<TH_PriceRuleForCycleMoment> TH_PriceRuleList_W5_C4    ;
   public static List<TH_PriceRuleForCycleMoment> TH_PriceRuleList_W5_C5    ;
   public static List<TH_PriceRuleForCycleMoment> TH_PriceRuleList_W5_C6    ;
   public static List<TH_PriceRuleForCycleMoment> TH_PriceRuleList_W5_C7    ;  // 24.11.2020 prvi puta od 22.02.2021.
   public static List<TH_PriceRuleForCycleMoment> TH_PriceRuleList_W3_SRI_C3;  // 31.08.2020 
   public static List<TH_PriceRuleForCycleMoment> TH_PriceRuleList_W3_SRI_C5;  // 24.11.2020 prvi puta od 09.12.2020.
   public static List<TH_PriceRuleForCycleMoment> TH_PriceRuleList_W3_SRI_C4;  // 30.11.2021 prvi puta od 15.12.2021.

   #region Novootvorene Poslovnice Irregular Cycle Moment

   // Kada predstoji otvaranje poslovnice tu joj zadati ova 2 podatka    

   // Kada nova poslovnica postane 'obicna' tj. uhvati ritam sa ostalima 
   // tada treba 'NewShop_ServerIDstr' staviti na ""                     

   // 08.01.2018: Koprivnica pretala biti 'NewShop' kada se pojavi nova, opet ovo aktivirati. 
 //public static string   NewShop_ServerIDstr = "74"                      ; // Server ID nove poslovnice as string 
   public static string   NewShop_ServerIDstr = ""                        ; // Server ID nove poslovnice as string 
   public static DateTime NewShop_BirthDate   = new DateTime(2017, 06, 19); // Datum pocetka rada nove poslovnice. '(2017, 06, 19)' bijase Koprivnica 

   public static bool ThisIsNovootvoreniShop(string skladCD)
   {
      if(skladCD.IsEmpty()) // znaci tu smo dosli iz mass ZPC production 
      {
         return false;
      }

      if(ZXC.NewShop_ServerIDstr.IsEmpty()) // znaci momentalno NEMAMO nijednu novootvorenu poslovnicu... a koja ima svoj pomaknuti CycleMoment 
      {
         return false;
      }

      return skladCD.StartsWith(ZXC.NewShop_ServerIDstr);
   }

   #endregion Novootvorene Poslovnice Irregular Cycle Moment

#if GetTHPR_ForThisDay_OLD

   #region TH Cycle Zero Dates 

   // 21.12.2017: BIG NEWS! Uvodimo List<DokDate> koja sadrzi datume pocetka 3week ciklusa;
   public static List<DateTime> ZeroDate3weekList = new List<DateTime> ( )
      {
         // !!! Date ASCENDING !!! Dodajes nove na kraj liste 

         new DateTime(2016, 12, 19),
         new DateTime(2017, 10, 16),
         new DateTime(2017, 12, 18),
         new DateTime(2018, 03, 19),
         new DateTime(2018, 06, 18),
         new DateTime(2018, 10, 01),
         new DateTime(2018, 12, 17),
      };

   public static DateTime THcycle_ZeroDate3week_ForThisDokDate(DateTime dokDate)
   {
      DateTime last3weekStartForThisDokDate = DateTime.MinValue;

      foreach(DateTime _3weekStart in ZeroDate3weekList)
      {
         if(dokDate >= _3weekStart) last3weekStartForThisDokDate = _3weekStart;
      }

      if(last3weekStartForThisDokDate == DateTime.MinValue) ZXC.aim_emsg(MessageBoxIcon.Error, "NEUSPJEH!\n\nTHcycle_ZeroDate3week_ForThisDokDate\n\nFor date {0}", dokDate.ToString(ZXC.VvDateFormat));

      return last3weekStartForThisDokDate;
   }

   public static DateTime THcycle_ZeroDate2week_ForThisDokDate(DateTime dokDate)
   {
      return THcycle_ZeroDate3week_ForThisDokDate(dokDate) + ThreeWeekSpan;
   }

   public static List<DateTime> ZeroDate6weekList = new List<DateTime> ( )
      {
         // !!! Date ASCENDING !!! Dodajes nove na kraj liste 

         new DateTime(2015, 07, 13),
         new DateTime(2017, 07, 17),
         new DateTime(2017, 12, 11),
         new DateTime(2018, 02, 26),
         new DateTime(2018, 07, 23),
      };

   public static DateTime THcycle_ZeroDate6week_ForThisDokDate(DateTime dokDate)
   {
      DateTime last6weekStartForThisDokDate = DateTime.MinValue;

      foreach(DateTime _6weekStart in ZeroDate6weekList)
      {
         if(dokDate >= _6weekStart) last6weekStartForThisDokDate = _6weekStart;
      }

      if(last6weekStartForThisDokDate == DateTime.MinValue) ZXC.aim_emsg(MessageBoxIcon.Error, "NEUSPJEH!\n\nTHcycle_ZeroDate6week_ForThisDokDate\n\nFor date {0}", dokDate.ToString(ZXC.VvDateFormat));

      return last6weekStartForThisDokDate;
   }

   public static DateTime THcycle_ZeroDate5week_ForThisDokDate(DateTime dokDate)
   {
      return THcycle_ZeroDate6week_ForThisDokDate(dokDate) + SixWeekSpan;
   }
   
   #endregion TH Cycle Zero Dates

   #region Get_TH_Cjenik_Kind from 20.12.2017 - 08.2018 (PUSE)

   // 20.12.2017: 
   public static TH_Cjenik_Kind Get_TH_Cjenik_Kind(DateTime dokDate, string skladCD)
   {
      if(IsTH_2WeekShop(skladCD)) return Get_TH_Cjenik_Kind_2W(dokDate);
      if(IsTH_5WeekShop(skladCD)) return Get_TH_Cjenik_Kind_5W(dokDate);

      return TH_Cjenik_Kind._NEDEFINIRANO_;
   }

   private static TH_Cjenik_Kind Get_TH_Cjenik_Kind_2W(DateTime dokDate)
   {
      bool is3week = Get_isThisUsual2week_temp3week(dokDate);

      if(is3week) return TH_Cjenik_Kind._2WShop_3WCjenik_;
      else        return TH_Cjenik_Kind._2WShop_2WCjenik_;
   }
   private static TH_Cjenik_Kind Get_TH_Cjenik_Kind_5W(DateTime dokDate)
   {
      bool is6week = Get_isThisUsual5week_temp6week(dokDate);

      if(is6week) return TH_Cjenik_Kind._5WShop_6WCjenik_;
      else        return TH_Cjenik_Kind._5WShop_5WCjenik_;
   }

   #endregion Get_TH_Cjenik_Kind from 20.12.2017 - 08.2018 (PUSE)

   private static bool Get_isThisUsual2week_temp3week(DateTime dokDate)
   {
      DateTime last_ZeroDate3week_ForThisDokDate = ZXC.THcycle_ZeroDate3week_ForThisDokDate(dokDate);

    //bool is3week = (dokDate >= ZXC.THcycle_ZeroDate3week_E      ) && (dokDate - ZXC.THcycle_ZeroDate3week_E      ) </*=*/ ThreeWeekSpan;
      bool is3week = (dokDate >= last_ZeroDate3week_ForThisDokDate) && (dokDate - last_ZeroDate3week_ForThisDokDate) </*=*/ ThreeWeekSpan;

      return is3week;
   }

   private static bool Get_isThisUsual5week_temp6week(DateTime dokDate)
   {
      DateTime last_ZeroDate6week_ForThisDokDate = ZXC.THcycle_ZeroDate6week_ForThisDokDate(dokDate);

      bool is6week = (dokDate >= last_ZeroDate6week_ForThisDokDate) && (dokDate - last_ZeroDate6week_ForThisDokDate) </*=*/ SixWeekSpan;

      return is6week;
   }

   internal static TimeSpan Get_elapsedFrom_CurrCycleStart_2weekShop(DateTime dokDate, string skladCD, bool is3week) 
   {
      DateTime currCycleStart;

      if(is3week == false) currCycleStart = THcycle_ZeroDate2week_ForThisDokDate(dokDate); // 2week 
      else                 currCycleStart = THcycle_ZeroDate3week_ForThisDokDate(dokDate); // 3week 

      // 12.06.2017: prejebavanje 'currCycleStart' logike, prilagodjeno novootvorenim poslovnicama ___ START ___ 

      if(ThisIsNovootvoreniShop(skladCD)) // ova poslovnica je novootvorena, izvan zatecenog ritma postojecih poslovnica 
      {
         currCycleStart = ZXC.NewShop_BirthDate;
      }

      // 12.06.2017: prejebavanje 'currCycleStart' logike, prilagodjeno novootvorenim poslovnicama ___  END  ___ 

      while(currCycleStart <= dokDate)
      {
       //currCycleStart +=                              (TwoWeekSpan) ;
         currCycleStart += (is3week ? (ThreeWeekSpan) : (TwoWeekSpan));
      }

    //currCycleStart -=                              (TwoWeekSpan) ;
      currCycleStart -= (is3week ? (ThreeWeekSpan) : (TwoWeekSpan));

      TimeSpan elapsedFrom_CurrCycleStart = dokDate - currCycleStart;

      return elapsedFrom_CurrCycleStart;
   }

   internal static TimeSpan Get_elapsedFrom_CurrCycleStart_5weekShop(DateTime dokDate, string skladCD, bool is6week)
   {
      // 09.11.2017: ukidamo THcycle_ZeroDate3week_ABCD & THcycle_ZeroDate5weekClassicNEW23 i ostavljamo samo zadnje 
    //DateTime currCycleStart = dokDate >= THcycle_ZeroDate5weekClassicNEW4 ? THcycle_ZeroDate5weekClassicNEW4 : THcycle_ZeroDate5weekClassicNEW3; // za 23.01.2017 pa nadalje 
      // 21.12.2017: 
    //DateTime currCycleStart =                                               THcycle_ZeroDate5weekClassicNEW4                                   ; // za 23.01.2017 pa nadalje 
      DateTime currCycleStart;

      if(is6week == false) currCycleStart = THcycle_ZeroDate5week_ForThisDokDate(dokDate); // 5week 
      else                 currCycleStart = THcycle_ZeroDate6week_ForThisDokDate(dokDate); // 6week 

      // 12.06.2017: prejebavanje 'currCycleStart' logike, prilagodjeno novootvorenim poslovnicama ___ START ___ 

      if(ThisIsNovootvoreniShop(skladCD)) // ova poslovnica je novootvorena, izvan zatecenog ritma postojecih poslovnica 
      {
         currCycleStart = ZXC.NewShop_BirthDate;
      }

      // 12.06.2017: prejebavanje 'currCycleStart' logike, prilagodjeno novootvorenim poslovnicama ___  END  ___ 

      while(currCycleStart <= dokDate)
      {
       //currCycleStart +=                            (FiveWeekSpan);
         currCycleStart += (is6week ? (SixWeekSpan) : (FiveWeekSpan));
      }

    //currCycleStart -=                            (FiveWeekSpan);
      currCycleStart -= (is6week ? (SixWeekSpan) : (FiveWeekSpan));

      TimeSpan elapsedFrom_CurrCycleStart = dokDate - currCycleStart;

      return elapsedFrom_CurrCycleStart;
   }

#endif

   public static bool IsTHshopDuringJanitorTime 
   {
      get
      {
         return ZXC.IsTEXTHOshop                                                                      &&
                (DateTime.Now.TimeOfDay >= (CURR_prjkt_rec.RvrDo.TimeOfDay + new TimeSpan(0, 20, 0))) &&
                (DateTime.Now.TimeOfDay <= (CURR_prjkt_rec.RvrDo.TimeOfDay + new TimeSpan(0, 30, 0)))  ;
      }
   }

   public static void IssueTHshopDuringJanitorTimeMessage()
   {
      string janOD = (ZXC.CURR_prjkt_rec.RvrDo.TimeOfDay + new TimeSpan(0, 20, 0)).ToString();
      string janDO = (ZXC.CURR_prjkt_rec.RvrDo.TimeOfDay + new TimeSpan(0, 30, 0)).ToString();

      ZXC.aim_emsg(MessageBoxIcon.Stop, "Nedozvoljen rad za vrijeme servisnih pozadinskih operacija.\n\n[{0}] - [{1}]", janOD, janDO);
   }

   // 07.11.2017: ne damo Valentini da radi ZPCove navecer od 21 do 6 
   public static bool IsTHcentralaDuringSkylabCacheTime { get { return ZXC.IsTEXTHOcentrala && (DateTime.Now.Hour >= 21 || DateTime.Now.Hour <= 05); } }
   public static void IssueTHcentralaDuringSkylabCacheTimeMessage()
   {
      ZXC.aim_emsg(MessageBoxIcon.Stop, "Nedozvoljena izrada ZPC-a za vrijeme servisnih pozadinskih operacija.\n\n[{0}] - [{1}]", "21.00", "06.00");
   }

   public static bool TH_Should_ESC_DRW_Log // Escape or delete row(s) 
   {
      get
      {
         if(IsTEXTHOshop                     == false        ) return false; 
         if(TheVvForm.TheVvUC is IRMDUC      == false        ) return false;
         if(TheVvForm.TheVvTabPage.WriteMode != WriteMode.Add) return false;

         return true;
      }
   }

   // 27.02.2018: TH 2018 News 'Zakljucaj SVE Prethodne Godine'

   public static int TH_FirstOkYear = 2024; // Svake godine kada Gabrielle kaze da je gotova s 'proslom' godinom 
                                            // ovu brojku treba inkrementirati... zapravo na 'tekucu' godinu     

   public static bool IsTH_vvTQ_VvKristal { get { return vvDB_VvDomena == "vvTQ" && (vvDB_Server.ToLower() == "vvkristal" || vvDB_Server.ToLower() == "localhost"); } }

   public static string GetNiceEnumStr(Enum _enum)
   {
      // Primjerice:                                                   
      //                                                               
      // public enum TH_CycleMoment                                    
      // {                                                             
      //    NEDEFINIRAN_CycleMoment,                                   
      //                                                               
      //    [NiceEnumStr("W2 Tjedan 1 Dan 1   regular")]               
      //                  W2_Tjedan_1_Dan_1___regular               ,  
      //    [NiceEnumStr("W2 Tjedan 1 Dan 2   do 30kn")]               
      //                  W2_Tjedan_1_Dan_2___do_30kn               ,  
      //    [NiceEnumStr("W2 Tjedan 1 Dan 3i4 do 25kn")]               
      //                  W2_Tjedan_1_Dan_3_4_do_25kn               ,  
      // 
      Type type = _enum.GetType();

      System.Reflection.MemberInfo[] memInfo = type.GetMember(_enum.ToString());

      if(memInfo != null && memInfo.Length > 0)
      {

         object[] attrs = memInfo[0].GetCustomAttributes(typeof(NiceEnumStr),
                                                         false);

         if(attrs != null && attrs.Length > 0)

            return ((NiceEnumStr)attrs[0]).Text;

      }

      return _enum.ToString();

   }

   public static string GetEnumDescription(Enum _enum)
   {
      // Primjerice:                                                   
      //                                                               
      // public enum QuantityCode
      // {
      // [Description("")]
      // Null,
      // [Description("H87")]
      // Piece,
      // [Description("KGM")]
      // Kilogram,
      // [Description("KMT")]
      // Kilometre,
      // [Description("GRM")]
      // Gram,

      Type type = _enum.GetType();

      System.Reflection.MemberInfo[] memInfo = type.GetMember(_enum.ToString());

      if(memInfo != null && memInfo.Length > 0)
      {

         object[] attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute),
                                                         false);

         if(attrs != null && attrs.Length > 0)

            return ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description;

      }

      return _enum.ToString();

   }

   public static TH_ShopWeekKind Get_TH_ShopWeekKind(string skladCD)
   {
      if(IsSkladCD_THshop(skladCD) == false) return TH_ShopWeekKind._NOT_TH_SHOP_;

           if(IsTH_2Week_PON_Shop(skladCD)) return TH_ShopWeekKind._2W_PON;
      else if(IsTH_2Week_SRI_Shop(skladCD)) return TH_ShopWeekKind._2W_SRI;
      else if(IsTH_3Week_SRI_Shop(skladCD)) return TH_ShopWeekKind._3W_SRI;
      else if(IsTH_5WeekShop     (skladCD)) return TH_ShopWeekKind._5W    ;

    //throw new Exception(string.Format("Skladiste [{0}] je nepoznat TH_ShopWeekKind.", skladCD));
      aim_emsg(string.Format("Skladiste [{0}] je nepoznat TH_ShopWeekKind.", skladCD));
      return TH_ShopWeekKind._NOT_TH_SHOP_;
   }

   public static void Check_TH_SKL_ShopWeekKind()
   {
      var malopSkladCDlist = ZXC.luiListaSkladista.Where(lui => lui.Flag == true && ZXC.IsSkladCD_THshop(lui.Cd)).Select(l => l.Cd).ToList();

      List<string> skladInfoList = new List<string>();

      foreach(string skladCD in malopSkladCDlist)
      {
         skladInfoList.Add(skladCD + " ---> " + Get_TH_ShopWeekKind(skladCD).ToString());
      }
   }

   #endregion TEXTHO - TEXTILEHOUSE 2 & 5 WEEK CYCLES

   #region All About SKY

   public static bool IsSkyEnvironment
   {
      get
      {
       //return vvDB_ServerID.NotZero() &&  ThisIsVektorProject                                                                                             /* && jos nesto?! */;
       //return vvDB_ServerID.NotZero() &&  ThisIsVektorProject                                                                              && IsTEXTHOany /* && jos nesto?! */;
         return vvDB_ServerID.NotZero() && (ThisIsVektorProject || ThisIsSkyLabProject || ZXC.vv_PRODUCT_Name == ZXC.vv_SKYLAB_PRODUCT_Name) && IsTEXTHOany /* && jos nesto?! */;
      }
   }

   private static bool ppero = true;
   public static bool PPERO
   {
      get { return ppero; }
      set { ppero = value; }
   }

   // 30.08.2018: SENDtoSKY_InProgress & RECEIVEfromSKY_InProgress fieldz encapsulated az propertiz 
 //public static bool  SENDtoSKY_InProgress      = false;
 //public static bool  RECEIVEfromSKY_InProgress = false;
   public static bool _SENDtoSKY_InProgress      = false;
   public static bool  SENDtoSKY_InProgress
   {
      get { return _SENDtoSKY_InProgress; }
      set
      {
         // /*!!! delme later*/ if(ZXC.IsTEXTHOany2 && ZXC.vvDB_ServerID == 46/*98*/) // log Đakovo only 
         //{
         //   string vvDRinfo = "";
         //   if(ZXC.TheVvForm.TheVvUC is IRMDUC)
         //   {
         //      Faktur faktur_rec = (Faktur)ZXC.TheVvForm.TheVvDataRecord;
         //      vvDRinfo = faktur_rec.ToString();
         //   }
         //   ZXC.aim_log("SEND: {0} ---> {1}\n\r{2}[{3}]\n\r\t", _SENDtoSKY_InProgress.ToString(), value.ToString(), ZXC.GetMethodNameDaStack_Short(), vvDRinfo);
         //}
         _SENDtoSKY_InProgress = value;
      }
   }
   public static bool _RECEIVEfromSKY_InProgress = false;
   public static bool  RECEIVEfromSKY_InProgress
   {
      get { return _RECEIVEfromSKY_InProgress; }
      set
      {
         // /*!!! delme later*/ if(ZXC.IsTEXTHOany2 && ZXC.vvDB_ServerID == 46/*98*/) // log Đakovo only 
         //{
         //   string vvDRinfo = "";
         //   if(ZXC.TheVvForm.TheVvUC is IRMDUC)
         //   {
         //      Faktur faktur_rec = (Faktur)ZXC.TheVvForm.TheVvDataRecord;
         //      vvDRinfo = faktur_rec.ToString();
         //   }
         //   ZXC.aim_log("RECEIVE: {0} ---> {1}\n\r{2}[{3}]\n\r\t", _RECEIVEfromSKY_InProgress.ToString(), value.ToString(), ZXC.GetMethodNameDaStack_Short(), vvDRinfo);
         //}
         _RECEIVEfromSKY_InProgress = value;
      }
   }

   public static bool SENDorRECEIVE_SKY_InProgress { get { return SENDtoSKY_InProgress || RECEIVEfromSKY_InProgress; } }

   public enum SkySklKind       { NONE = 0, CentGLSK = 1, CentPVSK = 2, ShopVPSK       = 3, ShopMPSK = 4 }
 //public enum SkyOperation     { NONE = 0, RECEIVE  = 1, SEND     = 2, SendAndReceive = 3               } // u odnosu na SkyServer      
   public enum SkyOperation     { NONE = 0, RECEIVE  = 1, SEND     = 2, SendAndReceive = 3, OpenSyncTran } // u odnosu na SkyServer      
   public enum LanSrvKind       { NONE = 0, CENT     = 1, SHOP     = 2, SKY            = 3               } // ovo je takodjer i BirthLoc 
   public enum SkyReceiveKind      
   { 
      NONE          = 0, 
      EVERYTHING    = 1, 
      OnlyLOCALskl1 = 2, 
      OnlyOTHERskl1 = 3, 
      OnlyLOCALskl2 = 4, 
      OnlyOTHERskl2 = 5,
   }

   public static bool IsEmpty (this SkySklKind     skl1kind   ) { return skl1kind    == SkySklKind    .NONE; }
   public static bool IsEmpty (this SkyReceiveKind shopRCVkind) { return shopRCVkind == SkyReceiveKind.NONE; }
   public static bool IsEmpty (this SkyOperation   operation  ) { return operation   == SkyOperation  .NONE; }
   public static bool IsEmpty (this LanSrvKind     srvKind    ) { return srvKind     == LanSrvKind    .NONE; }
   public static bool NotEmpty(this SkySklKind     frsSklKind ) { return frsSklKind  != SkySklKind    .NONE; }
   public static bool NotEmpty(this SkyReceiveKind shopRCVkind) { return shopRCVkind != SkyReceiveKind.NONE; }
   public static bool NotEmpty(this SkyOperation   operation  ) { return operation   != SkyOperation  .NONE; }
   public static bool NotEmpty(this LanSrvKind     srvKind    ) { return srvKind     != LanSrvKind    .NONE; }

 //public static bool IsSvDUHdomena    { get { return vvDB_VvDomena == /*"vvSD"*/SVD_vvDomena; } }
   public static bool IsSvDUHdomena    { get { return vvDB_VvDomena == /*"vvSD"*/SVD_vvDomena || vvDB_VvDomena == /*"vvSD"*/SVQ_vvDomena; } }
   public static bool IsSvDUH          { get { return                            ThisIsVektorProject && (VvDeploymentSite == ZXC.VektorSiteEnum.SVDUH || (IsSvDUHdomena && CURR_prjkt_rec.Ticker.StartsWith("TESTIS")) ||
                                                                                                         CURR_prjkt_rec.Ticker.StartsWith     ("SVDUH")) ; } }
   public static bool IsSvDUH_ZAHonly  { get { return IsSvDUHdomena && IsSVD_ZAH_UserName(CURR_userName); } }

   public static string[] SvDUH_donSkl = new string[]
   { "20", "22", "24", "26" };

   public static bool IsSvDUH_donSkl(string sklCD)
   {
      return IsSvDUH && SvDUH_donSkl.Contains(sklCD);
   } 

   public const string SVD_serverName = "APOTEKA1";
   public const string SVD_vvDomena   = "vvSD";
   public const string SVQ_vvDomena   = "vvSQ";

   public static bool IsSVD_ZAH_UserName(string userName)
   {
      if(userName.Length != 3)                return false;
      if(Char.IsLetter(userName[0]) == false) return false;
      if(Char.IsDigit (userName[1]) == false) return false;
      if(Char.IsDigit (userName[2]) == false) return false;

      return true;
   }

   public static bool IsSVD_ZAH_ODOBRAVATELJ_UserName(string userName)
   {
      if(userName.Length < 4)                 return false;
      if(Char.IsUpper(userName[0]) == false)  return false;
      if(userName[0] != userName[1])          return false;
      if(userName[0] != userName[2])          return false;

      return true;
   }

   public static bool IsVektor_SVD_Deployment
   {
      get
      {
         if(ZXC.ThisIsSkyLabProject) return false;
#if DEBUG
         return true;
#else
         if(Environment.MachineName == "RIPLEY7"       ||
            Environment.MachineName == "RIPLEY22"      ||
            Environment.MachineName == "VVKRISTAL"     ||
            Environment.MachineName == "VVKRISTAL-NEW" ||       
            Environment.MachineName == "QWHICHKEY"      ) return true; // 'kod nas' uvijk true 

         // 01.12.2022: koji je ovo krc?! ... dodali OR ZXC.IsSvDUHdomena
       //return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateLocation.ToString() == @"http://www.viper.hr/vektorSVD/Vektor.application";
         return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateLocation.ToString() == @"http://www.viper.hr/vektorSVD/Vektor.application" ||
                System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateLocation.ToString() == @"file://apoteka1/VektorClientDeploy/Vektor.application" ||
                ZXC.IsSvDUHdomena;
#endif
      }
   }


 //public static bool Should_SynchronizeArtiklSifrar { get { return IsTETRAGRAM_ANY; } } // TODO in future i za neke druge 
   public static bool Should_SynchronizeArtiklSifrar { get { return IsTETRAGRAM_T1_OR_T2; } } // TODO in future i za neke druge 
   public static bool IsTETRAGRAMdomena { get { return vvDB_VvDomena == "vvT1" || vvDB_VvDomena == "vvT2" || vvDB_VvDomena == "vvT3"; } }
   public static bool IsTETRAGRAM_PROJ 
   { 
      get 
      {
         if(CURR_prjkt_rec == null) return false;

         return ThisIsVektorProject && (VvDeploymentSite == ZXC.VektorSiteEnum.TGPROJ || CURR_prjkt_rec.Ticker.StartsWith("TGPROJ")) ; 
      } 
   }
   public static bool IsTETRAGRAM_PLEM 
   { 
      get 
      {
         if(CURR_prjkt_rec == null) return false;

         return ThisIsVektorProject && (VvDeploymentSite == ZXC.VektorSiteEnum.TGPLEM || CURR_prjkt_rec.Ticker.StartsWith("TGPLEM")) ; 
      } 
   }
   public static bool IsTETRAGRAM_ARTEON
   {
      get
      {
         if(CURR_prjkt_rec == null) return false;

         return ThisIsVektorProject && (VvDeploymentSite == ZXC.VektorSiteEnum.ARTEON || CURR_prjkt_rec.Ticker.StartsWith("ARTEON"));
      }
   }
   public static bool IsTETRAGRAM_ANY       { get { return IsTETRAGRAM_PROJ || IsTETRAGRAM_PLEM || IsTETRAGRAM_ARTEON; } }
   public static bool IsTETRAGRAM_T1_OR_T2  { get { return IsTETRAGRAM_PROJ || IsTETRAGRAM_PLEM; } }
   public static bool IsManyYearDB     { get { return IsPCTOGOdomena /*|| IsPCTOGO*/; } }
   public static bool IsPCTOGOdomena   { get { return vvDB_VvDomena == "vvPC"; } }
   public static bool IsPCTOGO         { get { return ThisIsVektorProject && (VvDeploymentSite == ZXC.VektorSiteEnum.PCTOGO ) ||
                                                                            //CURR_prjkt_rec.Ticker.StartsWith     ("PCTOGO") ; } }
                                                                              CURR_prjkt_rec.Ticker.StartsWith     ("PCTOG" ) ; } } // da imamo mogucnost i drugih prj
   public static bool IsTEMBO          { get { return                            ThisIsVektorProject && (VvDeploymentSite == ZXC.VektorSiteEnum.TEMBO || 
                                                                                                         CURR_prjkt_rec.Ticker.StartsWith     ("TEMBO")) ; } }
   public static bool IsKEREMP         { get { return                            ThisIsVektorProject && (VvDeploymentSite == ZXC.VektorSiteEnum.KEREMP || 
                                                                                                         CURR_prjkt_rec.Ticker.StartsWith     ("KEREMP")) ; } }
   public static bool IsZASTITARI      { get { return                            ThisIsVektorProject &&  VvDeploymentSite == VektorSiteEnum.BRADA        ; } }
   public static bool IsSPSISTdemo     { get { return                            ThisIsVektorProject &&  VvDeploymentSite == VektorSiteEnum.SPSIST       ; } }

   public static bool IsTEXTHOany      { get { return vvDB_ServerID.NotZero() && ThisIsVektorProject &&  VvDeploymentSite == VektorSiteEnum.TEXTHO;      } }
   public static bool IsTEXTHOsky      { get { return IsTEXTHOany && vvDB_ServerID == vvDB_ServerID_SkyCloud;                                            } }
   public static bool IsTEXTHOcentrala { get { return IsTEXTHOany && vvDB_ServerID == vvDB_ServerID_CENTRALA;                                            } }
   public static bool IsTEXTHOshop     { get { return IsTEXTHOany && vvDB_ServerID != vvDB_ServerID_CENTRALA && vvDB_ServerID != vvDB_ServerID_SkyCloud; } }

   // 29.02.2016: ZXC.IsTEXTHOany je ovdje ponekad false jer jos nije bio InitializeVvDao pa je ovo alternativni nacin 
   public static bool IsTEXTHOany2     { get { return IsTEXTHOany || vvDB_VvDomena == "vvTH" || vvDB_VvDomena == "vvTQ"; } }

   public static bool IsTEXTHOatypicShop(string skladCD)
   {
      // 19.09.2018: ugaseno tek danas?! 
    //return IsTEXTHOshop == true && skladCD == "60M5"; // zasada samo Rijeka, a kasnije niti ona 
      return                         false            ; // always false                           
   }

   public static bool IsPoslovnicaServerID(this uint thisServerID)
   {
      return thisServerID != vvDB_ServerID_CENTRALA && thisServerID != vvDB_ServerID_SkyCloud;
   }

   public static LanSrvKind ThisLanServerKind
   {
      get
      {
         //if(IsTEXTHOsky     ) return LanSrvKind.SKY ;
         //if(IsTEXTHOcentrala) return LanSrvKind.CENT;
         //if(IsTEXTHOshop    ) return LanSrvKind.SHOP;
         //
         //return LanSrvKind.NONE;

         return GetLanSrvKind(vvDB_ServerID);
      }
   }

   public static ZXC.LanSrvKind GetLanSrvKind(uint serverID)
   {
      // 26.12.2015: !!! 
      if(serverID.IsZero())                      return ZXC.LanSrvKind.NONE;

      if(serverID == ZXC.vvDB_ServerID_SkyCloud) return ZXC.LanSrvKind.SKY ; // TEORETSKI NEMOGUCE!!! 
      if(serverID == ZXC.vvDB_ServerID_CENTRALA) return ZXC.LanSrvKind.CENT;
      if(serverID.IsPoslovnicaServerID()       ) return ZXC.LanSrvKind.SHOP;

      return ZXC.LanSrvKind.NONE;
   }

   public static bool IsPoslovnicaSklad(string skladCD)
   {
      uint skladServerID = ZXC.ValOrZero_UInt(skladCD.Substring(0, 2));

      if(skladServerID.IsZero()) return false;

      return skladServerID.IsPoslovnicaServerID();
   }

 //public static uint GetTEXTHO_Regija_puse(string skladCD)
 //{
 //   if(skladCD.IsEmpty()) return 0;
 //
 //   uint serverID = ZXC.ValOrZero_UInt(skladCD.Substring(0, 2));
 //
 //   return GetTEXTHO_Regija(serverID);
 //}

   public static DateTime Slavonija_24_26_EndDate10072018 = new DateTime(2018, 07, 10);
   public static DateTime Slavonija_38_48_EndDate12072018 = new DateTime(2018, 07, 12);
   public static DateTime Slavonija_46_76_EndDate18072018 = new DateTime(2018, 07, 18);

   public static uint GetTEXTHO_Regija(uint serverID)
   {
      if(serverID.IsZero()) return 0;

      if(IsTEXTHOshop == false) { /*ZXC.aim_emsg(MessageBoxIcon.Error, "SkladCD [{0}] NIJE TEXTHO Shop!", skladCD);*/ return 0; }

      // 10.07.2018: Slavonija se u sljedecih par dana totalno gasi. Oni kazu:
      // od 10-16 24 i 26 
      // od 12-18 38 i 48 
      // od 18-25 46 i 76 

      if(serverID == 24 || // 24 Osijek 1       
         serverID == 26  ) // 26 Osijek 2       
      {
         if(ZXC.programStartedOnDateTime.Date < Slavonija_24_26_EndDate10072018) return 2; // Do 09.07. su na 'Slavonija' 
         else                                                                    return 1; // Od 10.07. su na 'Zagreb'    
      }
      if(serverID == 38 || // 38 Vinkovci       
         serverID == 48  ) // 48 Nasice         
      {
         if(ZXC.programStartedOnDateTime.Date < Slavonija_38_48_EndDate12072018) return 2; // Do 11.07. su na 'Slavonija' 
         else                                                                    return 1; // Od 12.07. su na 'Zagreb'    
      }
      if(serverID == 46 || // 46 Djakovo         
         serverID == 76  ) // 76 Beli Manastir   
      {
         if(ZXC.programStartedOnDateTime.Date < Slavonija_46_76_EndDate18072018) return 2; // Do 17.07. su na 'Slavonija' 
         else                                                                    return 1; // Od 18.07. su na 'Zagreb'    
      }
      if(serverID == 98) // 98 Viper Test        
      {
         if(ZXC.programStartedOnDateTime.Date < Slavonija_24_26_EndDate10072018) return 2; // Do 09.07. su na 'Slavonija' 
         else                                                                    return 1; // Od 10.07. su na 'Zagreb'    
      }

      // 'SLAVONIJA' skladista ... PUSE !!! 
      if(serverID == 24 || // 24 Osijek 1       
         serverID == 26 || // 26 Osijek 2       
       //serverID == 34 || // 34 Pozega         //09.05.2018: preseljeno na zagreb 
         serverID == 38 || // 38 Vinkovci       
         serverID == 44 || // 44 Vukovar        
         serverID == 46 || // 46 Djakovo        
         serverID == 48 || // 48 Nasice         
       //serverID == 50 || // 50 Virovitica     // 07.11.2015: preseljeno na zagreb 
         serverID == 98 || // 98 Viper Test     
         serverID == 76  ) // 76 Beli Manastir  
       //serverID == 62 || // 62 Kutina         //08.06.2015., // 01.09.2015
       //serverID == 66 || // 66 Nova Gradiska  //04.08.2017: preseljeno na zagreb 
       //serverID == 56  ) // 56 Slavonski Brod //09.05.2018: preseljeno na zagreb 
       {
         ZXC.aim_emsg("Nemoguce!");
         return 2;
       }

      // 'ZAGREB' skladista 
      if(serverID == 14 || // 14 Ilica          
         serverID == 16 || // 16 Dubrava        
         serverID == 18 || // 18 Subiceva       
         serverID == 20 || // 20 Velika Gorica  
         serverID == 22 || // 22 Savska         
         serverID == 28 || // 28 Maksimirska    
         serverID == 30 || // 30 Racki          
         serverID == 32 || // 32 Ozaljska       
         serverID == 34 || // 34 Pozega         //09.05.2018: preseljeno sa slavonije
         serverID == 36 || // 36 Zabok          
         serverID == 40 || // 40 Krizevci       
         serverID == 42 || // 42 Zapresic       
         serverID == 50 || // 50 Virovitica     // 07.11.2015: preseljeno sa slavonije 
         serverID == 52 || // 52 Sisak          
         serverID == 54 || // 54 Karlovac       
         serverID == 56 || // 56 Slavonski Brod //09.05.2018: preseljeno sa slavonije
         serverID == 58 || // 58 Varazdin       
         serverID == 62 || // 62 Kutina         // 01.09.2015: opet vraceno na ZGB 
         serverID == 64 || // 64 Bjelovar       
         serverID == 66 || // 66 Nova Gradiska  // od 04.08.2017 
         serverID == 68 || // 68 Čakovec        
         serverID == 70 || // 70 Maksimir 2     
         serverID == 72 || // 72 Savica         
         serverID == 74 || // 74 Koprivnica     
         serverID == 78 || // 78 Split          // od 11.04.2018 
         serverID == 80 || // 80 Virovitica nova // 27.08.2018: preseljena lokacija u Virovitici pa je nova poslovnica, 50 je puse 
         serverID == 82 || // 82 Sesvete        
         serverID == 84 || // 84 Rijeka 2       
         serverID == 88 || // 88 Tratinska      
         serverID == 86 || // 86 Vlaška         
         serverID == 60  ) // 60 Rijeka         
         return 1;

      ZXC.aim_emsg(MessageBoxIcon.Error, "Za serverID [{0}] ne mogu saznati regiju!", serverID); return 0; 

   }

   //public struct SkySyncInfo
   //{
   //   #region Propertiez & Constructor

   //   public string           TheTT         { get; set; }
   //   public LanSrvKind       BirthLoc      { get; set; }
   //   public SkyFrsSklKind    FirstSklad    { get; set; }
   //   public SkyShareKind     ShareKind     { get; set; }
   //   public SkyOperation     Operation     { get; set; }

   //   public SkySyncInfo(string           _TheTT     ,
   //                      LanSrvKind       _BirthLoc  ,
   //                      SkyFrsSklKind    _FirstSklad,
   //                      SkyShareKind     _ShareKind ,
   //                      SkyOperation     _Operation ) : this()
   //   {
   //      this.TheTT         = _TheTT     ;
   //      this.BirthLoc      = _BirthLoc  ;
   //      this.FirstSklad    = _FirstSklad;
   //      this.ShareKind     = _ShareKind ;
   //      this.Operation     = _Operation ;
   //   }

   //   public static SkySyncInfo Empty = new ZXC.SkySyncInfo("", LanSrvKind   .NONE,
   //                                                             SkyFrsSklKind.NONE,
   //                                                             SkyShareKind .NONE,
   //                                                             SkyOperation .NONE);
   //   #endregion Propertiez & Constructor

   //}

   public static string[] CURR_SkyRules_RtransTT_Array 
   { 
      get 
      {
         var TT_List = CURR_SkyRules.Where(sr => sr.DocumTT.NotEmpty()).Select(sr => sr.DocumTT).Distinct().ToList();

         int count = TT_List.Count;

         for(int i=0; i < count; ++i)
         {
            if(ZXC.TtInfo(TT_List[i]).HasSplitTT) TT_List.Add(ZXC.TtInfo(TT_List[i]).SplitTT);
            if(ZXC.TtInfo(TT_List[i]).HasTwinTT ) TT_List.Add(ZXC.TtInfo(TT_List[i]).TwinTT );
         }

         return TT_List.ToArray(); 
      } 
   } 

   public static bool IsVvDataRecord_MyBorn_InSkyEnvironment(VvDataRecord theVvDataRecord)
   {
      return ZXC.IsSkyEnvironment && theVvDataRecord.VirtualLanSrvID == ZXC.vvDB_ServerID;
   }

   #endregion All About SKY

   #region VvMailClientStuff

   public static string ViperMailHost              = "mail.viper.hr"  ;
   public static string VektorEmailAddress         = "vektor@viper.hr";
   public static string SkyLabEmailAddress         = "skylab@viper.hr";
   public static string SkyLabEmailFromDisplayName = "VvSkyLab"       ;
   public static string RobertEmailAddress         = "robert@viper.hr";

   public static string SkyLabEmailPassword = "qwe1qwe2qwe3".Replace("qwe3", "1q").Replace("qwe2", "era").Replace("qwe1", "maw"); // pokusavamo kamuflirati string anti reverse engeneering 


   public static string Vv_GZip_ThisFile(string fullFileNamePath)
   {
      //string full_ZIP_FileNamePath = "";

      FileInfo fi = new FileInfo(fullFileNamePath);

      using (FileStream inFile = fi.OpenRead())
      {
          // Prevent compressing hidden and 
  
          // already compressed files.
  
          if ((File.GetAttributes(fi.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & fi.Extension != ".gz")
          {
            // Create the compressed file.

            using (FileStream full_ZIP_FileNamePath = File.Create(fi.FullName + ".gz"))
            {
                using (System.IO.Compression.GZipStream Compress = new System.IO.Compression.GZipStream(full_ZIP_FileNamePath, System.IO.Compression.CompressionMode.Compress))
                {
                  // Copy the source file into 
                  // the compression stream.
                  inFile.CopyTo(Compress);
            
                  //Console.WriteLine("Compressed {0} from {1} to {2} bytes.", fi.Name, fi.Length.ToString(), full_ZIP_FileNamePath.Length.ToString());
            
                }
            
                return full_ZIP_FileNamePath.Name;
            }

            // DotNet 4.7.2: 
            //using(FileStream zipToOpen = new FileStream(@"c:\users\exampleuser\release.zip", FileMode.Open))
            //{
            //   using(ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
            //   {
            //      ZipArchiveEntry readmeEntry = archive.CreateEntry("Readme.txt");
            //      using(StreamWriter writer = new StreamWriter(readmeEntry.Open()))
            //      {
            //         writer.WriteLine("Information about this package.");
            //         writer.WriteLine("========================");
            //      }
            //   }
            //}

         }
      }

      //using(FileStream fs = new FileStream(@"C:\Temp\output.zip", FileMode.Create))
      //using(ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
      //{
      //   arch.CreateEntryFromFile(@"C:\Temp\data.xml", "data.xml");
      //}
      return "";
   }

#if mozda_nikad

   #region ICSharpCode.SharpZipLib.dll

   // http://www.csharpest.net/?p=110                                      
   // https://www.codepool.biz/create-extract-update-tar-gzip-windows.html 
   // https://github.com/yushulx/dotnet-tar-gzip/blob/master/Program.cs    

   // Name                                                                                 
   //     targzip                                                                          
   // Synopsis                                                                             
   //     targzip [c file.tar.gz directory] [e file.tar.gz directory] [u file.tar.gz file] 
   // Description                                                                          
   //     c create a tar.gz file                                                           
   //     e extract a tar.gz file                                                          
   //     u update a file                                                                  
   //                                 ";                                                   
   //                                                                                      
   //                 Console.WriteLine(usage);                                            
   //                 return;                                                              
   //             }                                                                        
   //                                                                                      
   //             if (args.Length != 3)                                                    
   //             {                                                                        
   //                 Console.WriteLine("Invalid arguments.");                             
   //                 return;                                                              
   //             }                                                                        
   //                                                                                      
   //             if (!args[1].EndsWith(".tar.gz"))                                        
   //             {                                                                        
   //                 Console.WriteLine("Please input a gzip file.");                      
   //                 return;                                                              
   //             }                                                                        
   //                                                                                      
   //             string opt = args[0];                                                    
   //             Program app = new Program();                                             
   //             try                                                                      
   //             {                                                                        
   //                 switch (opt)                                                         
   //                 {                                                                    
   //                     case "c":                                                        
   //                         {                                                            
   //                             app.CreateTarGZ(args[1], args[2]);                       
   //                         }                                                            
   //                         break;                                                       
   //                     case "e":                                                        
   //                         {                                                            
   //                             app.ExtractTGZ(args[1], args[2]);                        
   //                         }                                                            
   //                         break;                                                       
   //                     case "u":                                                        
   //                         {                                                            
   //                             app.UpdateGZipFile(args[1], args[2], true);              
   //                         }                                                            
   //                         break;                                                       
   //                     default:                                                         
   //                         Console.WriteLine("Invalid options.");                       
   //                         break;                                                       
   //                 }                                                                    
   //                                                                                      
   //                 Console.WriteLine("Done.");                                          
   //             }                                                                        
   //             catch (Exception e)                                                      
   //             {                                                                        
   //                 Console.WriteLine(e);                                                
   //             }                                                                        
   //                                                                                      
   //         }                                                                            
   //     }                                                                                
   // }                                                                                    

   internal static void AddFilesToTarThenDeleteFiles         (string outputTarFilename, List<string> filenames)
   {
      UpdateGZipFileS(outputTarFilename, filenames, /*bool asciiTranslate ?!? */ true);
   }

   public static void ExtractTGZ(String gzArchiveName/*, String destFolder*/)
   {
      string destFolder = Path.GetDirectoryName(gzArchiveName); // byQ 

      Stream inStream   = File.OpenRead(gzArchiveName);
      Stream gzipStream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(inStream);

      ICSharpCode.SharpZipLib.Tar.TarArchive tarArchive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(gzipStream);
      tarArchive.ExtractContents(destFolder);
      tarArchive.Close();

      gzipStream.Close();
      inStream.Close();
   }

   public static string ExtractGZipFile(string gzipFileName/*, string targetDir*/)
   {
      string targetDir = Path.GetDirectoryName(gzipFileName); // byQ 

      // Use a 4K buffer. Any larger is a waste.    
      byte[] dataBuffer = new byte[4096];

      using(System.IO.Stream fs = new FileStream(gzipFileName, FileMode.Open, FileAccess.Read))
      {
         using(ICSharpCode.SharpZipLib.GZip.GZipInputStream gzipStream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(fs))
         {

            // Change this to your needs
            string fnOut = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(gzipFileName));

            using(FileStream fsOut = File.Create(fnOut))
            {
               ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(gzipStream, fsOut, dataBuffer);
            }

            return fnOut;
         }
      }
   }

   private static void CopyWithAsciiTranslate(ICSharpCode.SharpZipLib.Tar.TarInputStream tarIn, Stream outStream)
   {
      byte[] buffer = new byte[4096];
      bool isAscii = true;
      bool cr = false;

      int numRead = tarIn.Read(buffer, 0, buffer.Length);
      int maxCheck = Math.Min(200, numRead);
      for(int i = 0; i < maxCheck; i++)
      {
         byte b = buffer[i];
         if(b < 8 || (b > 13 && b < 32) || b == 255)
         {
            isAscii = false;
            break;
         }
      }
      while(numRead > 0)
      {
         if(isAscii)
         {
            // Convert LF without CR to CRLF. Handle CRLF split over buffers.
            for(int i = 0; i < numRead; i++)
            {
               byte b = buffer[i]; // assuming plain Ascii and not UTF-16
               if(b == 10 && !cr)     // LF without CR
                  outStream.WriteByte(13);
               cr = (b == 13);

               outStream.WriteByte(b);
            }
         }
         else
         {
            outStream.Write(buffer, 0, numRead);
         }
         numRead = tarIn.Read(buffer, 0, buffer.Length);
      }
   }

   public static void ExtractTar(String tarFileName, String destFolder)
   {

      Stream inStream = File.OpenRead(tarFileName);
      ICSharpCode.SharpZipLib.Tar.TarArchive tarArchive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(inStream);
      tarArchive.ExtractContents(destFolder);
      tarArchive.Close();
      inStream.Close();
   }

   public static void UpdateTar(string tarFileName, string targetFile, bool asciiTranslate)
   {
      using(FileStream fsIn = new FileStream(tarFileName, FileMode.Open, FileAccess.Read))
      {
         string tmpTar = Path.Combine(Path.GetDirectoryName(tarFileName), "tmp.tar");
         using(FileStream fsOut = new FileStream(tmpTar, FileMode.OpenOrCreate, FileAccess.Write))
         {
            ICSharpCode.SharpZipLib.Tar.TarOutputStream tarOutputStream = new ICSharpCode.SharpZipLib.Tar.TarOutputStream(fsOut);
            ICSharpCode.SharpZipLib.Tar.TarInputStream  tarIn           = new ICSharpCode.SharpZipLib.Tar.TarInputStream (fsIn );
            ICSharpCode.SharpZipLib.Tar.TarEntry tarEntry;

            while((tarEntry = tarIn.GetNextEntry()) != null)
            {

               if(tarEntry.IsDirectory)
               {
                  continue;
               }
               // Converts the unix forward slashes in the filenames to windows backslashes
               //
               string name = tarEntry.Name.Replace('/', Path.DirectorySeparatorChar);
               string sourceFileName = Path.GetFileName(targetFile);
               string targetFileName = Path.GetFileName(tarEntry.Name);

               if(sourceFileName.Equals(targetFileName))
               {
                  using(Stream inputStream = File.OpenRead(targetFile))
                  {

                     long fileSize = inputStream.Length;
                     ICSharpCode.SharpZipLib.Tar.TarEntry entry = ICSharpCode.SharpZipLib.Tar.TarEntry.CreateTarEntry(tarEntry.Name);

                     // Must set size, otherwise TarOutputStream will fail when output exceeds.
                     entry.Size = fileSize;

                     // Add the entry to the tar stream, before writing the data.
                     tarOutputStream.PutNextEntry(entry);

                     // this is copied from TarArchive.WriteEntryCore
                     byte[] localBuffer = new byte[32 * 1024];
                     while(true)
                     {
                        int numRead = inputStream.Read(localBuffer, 0, localBuffer.Length);
                        if(numRead <= 0)
                        {
                           break;
                        }
                        tarOutputStream.Write(localBuffer, 0, numRead);
                     }
                  }
                  tarOutputStream.CloseEntry();
               }
               else
               {
                  tarOutputStream.PutNextEntry(tarEntry);

                  if(asciiTranslate)
                  {
                     CopyWithAsciiTranslate(tarIn, tarOutputStream);
                  }
                  else
                  {
                     tarIn.CopyEntryContents(tarOutputStream);
                  }

                  tarOutputStream.CloseEntry();
               }
            }
            tarIn.Close();
            tarOutputStream.Close();
         }

         File.Delete(tarFileName);
         File.Move(tmpTar, tarFileName);
      }
   }

   private static void UpdateTarGZ(string tgzFilename, string tarFileName)
   {
      Stream gzoStream = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(File.Create(tgzFilename));

      using(FileStream source = File.Open(tarFileName, FileMode.Open))
      {
         byte[] localBuffer = new byte[32 * 1024];
         while(true)
         {
            int numRead = source.Read(localBuffer, 0, localBuffer.Length);
            if(numRead <= 0)
            {
               break;
            }
            gzoStream.Write(localBuffer, 0, numRead);
         }
      }

      gzoStream.Close();

      File.Delete(tarFileName);
   }

   // ovo update-a postojeci file tarFile-a, NIJE dodavanje novog fajla! 
   public static void UpdateGZipFileS(string targzFileName, List<string> fileList, bool asciiTranslate)
   {
      if(!File.Exists(targzFileName))
      {
         //Console.WriteLine("Please input valid file");
         ZXC.aim_log("Please input valid gzipFileName");
         return;
      }

      if(fileList.IsEmpty())
      {
         return;
      }

      // Extract gzip to tar
      string tarFileName = ExtractGZipFile(targzFileName/*, Path.GetDirectoryName(gzipFileName)*/);
      // Update tar
      foreach(string targetFile in fileList)
      {
         UpdateTar(tarFileName, targetFile, asciiTranslate);
      }

      // Create a new tar.gz
      UpdateTarGZ(targzFileName, tarFileName);
   }

   #endregion ICSharpCode.SharpZipLib.dll

#endif

   #endregion VvMailClientStuff

   #region Check Connectivity

   #region From Internet

   //Method 1: WebRequest
   //We may send a web request to a website which assumed to be online always, for example google.com. 
   //If we can get a response, then obviously the device that runs our application is connected to the internet.
   private static bool VvCC_WebRequestTest() // sporo? 
   {
      string url = "http://www.google.com";
      try
      {
         System.Net.WebRequest myRequest = System.Net.WebRequest.Create(url);
         System.Net.WebResponse myResponse = myRequest.GetResponse();
      }
      catch(System.Net.WebException)
      {
         return false;
      }
      return true;
   }

   //Method 2: TCP Socket
   //There can be some delay in response of web request therefore this method may not be fast enough for some applications. 
   //A better way is to check whether port 80, default port for http traffic, of an always online website. 
   private static bool VvCC_TcpSocketTest() // brze? 
   {
      try
      {
         System.Net.Sockets.TcpClient client =
             new System.Net.Sockets.TcpClient("www.google.com", 80);
         client.Close();
         return true;
      }
      catch(System.Exception ex)
      {
         return false;
      }
   }

   //Method 3: Ping

   private static bool VvCC_PingTest(string IPaddressAsString)
   {
      System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();

      //set options ttl=128 and no fragmentation
      System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions(128, true);

      //32 empty bytes buffer
      byte[] data = new byte[32];

      System.Net.NetworkInformation.PingReply pingReply =
        //ping.Send(System.Net.IPAddress.Parse("208.69.34.231"  ), 1000               );
        //ping.Send(System.Net.IPAddress.Parse(IPaddressAsString), 8000               ); // 8.00 sec timeout
          ping.Send(System.Net.IPAddress.Parse(IPaddressAsString), 8000, data, options); // 8.00 sec timeout

      bool success = (pingReply.Status == System.Net.NetworkInformation.IPStatus.Success);

      // 27.08.2018: dodan log
      if(success == false) ZXC.aim_log("VvCC_PingTest({0}) failed: pingStatus.Status = {1}.", IPaddressAsString, pingReply.Status.ToString());

      return success;

      //if(pingStatus.Status == System.Net.NetworkInformation.IPStatus.Success)
      //{
      //   return true;
      //}
      //else
      //{
      //   return false;
      //}
   }

   //Method 4: DNS Lookup 
   //Alternatively you can use DNS lookup to check internet connectivity. This method is faster than Ping method.
   /// <summary>
   /// Vv CheckConnectivity by DNS test
   /// </summary>
   /// <param name="targetHost"></param>
   /// <returns></returns>
   private static bool VvCC_DnsTest_ForHost(string targethostNameOrAddress)
   {
      if(targethostNameOrAddress.IsEmpty()) { ZXC.aim_emsg(MessageBoxIcon.Error, "TargethostNameOrAddress is empty!"); return false; }

      try
      {
         // 2.3. 2018 OVO NE VALJA 'ipHE' nije null a ja nemrem pingat SkyLaba!
         System.Net.IPHostEntry ipHe = System.Net.Dns.GetHostEntry(targethostNameOrAddress);

         return true;
      }
      catch
      {
         ZXC.aim_log("VvCC_DnsTest_ForHost [{0}] failed!", targethostNameOrAddress); 
         return false;
      }
   }

   #endregion From Internet

   // PUSE - moze to i bolje 
 //public static bool IsSkyDbConnectionOK
 //{
 //   get
 //   {
 //      try
 //      {
 //         string dummy = ZXC.TheSkyDbConnection.Database; // ovo treba pokusati otvoriti konekciju
 //
 //         if(theSkyDbConnection == null || theSkyDbConnection.State != ConnectionState.Open || (ShouldPing && theSkyDbConnection.Ping() == false)) return /*null*/false;
 //      }
 //      catch(Exception ex) { return /*null*/false; }
 //
 //      return true;
 //   }
 //}
 //public static bool IsSkyDbConnectionNotOK { get { return !IsSkyDbConnectionOK; } }

   //02.10.2017: 
 //public static bool NO_SKY_Connection
 //{
 //   get
 //   {
 //      if(ZXC.IsTEXTHOshop == false) return false;
 //
 //      bool isDown = (ZXC.theSkyDbConnection == null || ZXC.theSkyDbConnection.State != ConnectionState.Open);
 //
 //      if(isDown) ZXC.NO_Internet = true ;
 //      else       ZXC.NO_Internet = false;
 //
 //      return isDown;
 //
 //   }
 //}

   public static bool Vv_ThisClientHasInternetConnection { get { string theHost = "www.google.com"                   ; return VvCC_DnsTest_ForHost(theHost); } }
   // 02.03.2018: otkrili [kada je SKY zaista bio down] da VvCC_DnsTest_ForHost radi krivo. ipHe NIJE null a SKY-a nema... pa prelazimo na ping 
 //public static bool Vv_ThisClientHasSkyLabConnection   { get { string theHost = CURR_prjkt_rec.SkySrvrHostDecrypted; return VvCC_DnsTest_ForHost(theHost); } }
   public static bool Vv_ThisClientHasSkyLabConnection   { get { string theHost = CURR_prjkt_rec.SkySrvrHostDecrypted; return VvCC_PingTest       (theHost); } }

 //public static bool Vv_ThisClientHasFiskalConnection   { get { string theHost = Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav.cisUrl; return VvCC_DnsTest_ForHost(theHost); } }
   public static bool Vv_ThisClientHasFiskalConnection   { get { string theHost = "cis.porezna-uprava.hr"                                     ; return VvCC_DnsTest_ForHost(theHost); } }
 //public static bool Vv_ThisClientHasFiskalConnection   { get { /* TODO: !!!!! */                                     return Vv_ThisClientHasInternetConnection; } }

   public static bool Vv_ThisClientHas_NO_InternetConnection { get { return !Vv_ThisClientHasInternetConnection; } }
   public static bool Vv_ThisClientHas_NO_SkyLabConnection   { get { return !Vv_ThisClientHasSkyLabConnection  ; } }
   public static bool Vv_ThisClientHas_NO_FiskalConnection   { get { return !Vv_ThisClientHasFiskalConnection  ; } }

   #endregion Check Connectivity

   #region List of installed PROGRAMS 

   internal static List<VvUtilDataPackage> VvGetInstalledPrograms()
   {
      List<VvUtilDataPackage> installs = new List<VvUtilDataPackage>();
      List<string> keys = new List<string>() {
        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
        @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
      };

      // The RegistryView.Registry64 forces the application to open the registry as x64 even if the application is compiled as x86 
      FindInstalls(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64), keys, installs);
      FindInstalls(RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64), keys, installs);

      installs = installs.Where(progInf => progInf.TheStr1.NotEmpty())./*Distinct().*/OrderBy(progInf => progInf.TheStr3).ToList();
      //installs.Sort(); // The list of ALL installed applications

      return installs;
   }

   private static void FindInstalls(RegistryKey regKey, List<string> keys, List<VvUtilDataPackage> installed)
   {
      VvUtilDataPackage progInfo;

      foreach(string key in keys)
      {
         using(RegistryKey rk = regKey.OpenSubKey(key))
         {
            if(rk == null)
            {
               continue;
            }
            foreach(string skName in rk.GetSubKeyNames())
            {
               using(RegistryKey sk = rk.OpenSubKey(skName))
               {
                  try
                  {
                     progInfo = new VvUtilDataPackage()
                     {
                        TheStr1 = skName                                         ,
                        TheStr2 = Convert.ToString(sk.GetValue("DisplayVersion")),
                        TheStr3 = Convert.ToString(sk.GetValue("DisplayName"))   ,
                        TheDate = ZXC.ValOr_01010001_DateTime_Import_yyyyMMdd_Format(Convert.ToString(sk.GetValue("InstallDate")))
                     };

                     installed.Add(progInfo);
                  }
                  catch(Exception ex)
                  { }
               }
            }
         }
      }
   }

   public static bool HasAppropriateCRruntime()
   {
      List<VvUtilDataPackage> installs = ZXC.VvGetInstalledPrograms();

      // gore si u using(RegistryKey sk = rk.OpenSubKey(skName))
      // saznao (skName) GUID od CR13SP32MSI64_0-80007712.MSI   
      // ("CR for Visual Studio SP32 CR Runtime 64-bit MSI") ||   
      // ("CR for Visual Studio SP32 CR Runtime 32-bit MSI")
      bool hasAppropriateCRruntime = installs.Any(prg => prg.TheStr1 == @"{36B0EEF1-E0B2-40FC-BEE5-F036E51D7540}" ||
                                                         prg.TheStr1 == @"{4D5EEA90-E0C2-4130-8FC2-F468998AADA3}"); // u TheStr1 je GUID od instaliranog CRruntime-a 

      return hasAppropriateCRruntime;
   }

   #endregion List of installed PROGRAMS 

}

#if SVD_LicenceCount
SELECT 10 AS Mj, 2021 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2021-10-31' UNION
SELECT 11 AS Mj, 2021 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2021-11-30' UNION
SELECT 12 AS Mj, 2021 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2021-12-31' UNION
SELECT 01 AS Mj, 2022 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2022-01-31' UNION
SELECT 02 AS Mj, 2022 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2022-02-28' UNION
SELECT 03 AS Mj, 2022 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2022-03-31' UNION
SELECT 04 AS Mj, 2022 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2022-04-30' UNION
SELECT 05 AS Mj, 2022 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2022-05-31' UNION
SELECT 06 AS Mj, 2022 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2022-06-30' UNION
SELECT 07 AS Mj, 2022 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2022-07-31' UNION
SELECT 08 AS Mj, 2022 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2022-08-31' UNION
SELECT 09 AS Mj, 2022 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2022-09-30' UNION
SELECT 10 AS Mj, 2022 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2022-10-31' UNION
SELECT 11 AS Mj, 2022 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2022-11-30' UNION
SELECT 12 AS Mj, 2022 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2022-12-31' UNION
SELECT 01 AS Mj, 2023 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2023-01-31' UNION
SELECT 02 AS Mj, 2023 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2023-02-28' UNION
SELECT 03 AS Mj, 2023 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2023-03-31' UNION
SELECT 04 AS Mj, 2023 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2023-04-30' UNION
SELECT 05 AS Mj, 2023 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2023-05-31' UNION
SELECT 06 AS Mj, 2023 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2023-06-30' UNION
SELECT 07 AS Mj, 2023 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2023-07-31' UNION
SELECT 08 AS Mj, 2023 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2023-08-31' UNION
SELECT 09 AS Mj, 2023 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2023-09-30' UNION
SELECT 10 AS Mj, 2023 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2023-10-31' UNION
SELECT 11 AS Mj, 2023 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2023-11-30' UNION
SELECT 12 AS Mj, 2023 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2023-12-31' UNION
SELECT 01 AS Mj, 2024 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2024-01-31' UNION
SELECT 02 AS Mj, 2024 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 75  AS Legalno, COUNT(DISTINCT clientName) - 3 - 75  AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2024-02-14' UNION
SELECT 03 AS Mj, 2024 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 230 AS Legalno, COUNT(DISTINCT clientName) - 3 - 230 AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2024-03-15' UNION
SELECT 04 AS Mj, 2024 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 230 AS Legalno, COUNT(DISTINCT clientName) - 3 - 230 AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2024-04-15' UNION
SELECT 05 AS Mj, 2024 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 230 AS Legalno, COUNT(DISTINCT clientName) - 3 - 230 AS NE_legalno FROM vvmbf_svd.svd_21_22_23_24 mbf WHERE logTS <= '2024-05-15' 

SELECT 06 AS Mj, 2024 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 1   AS Legalno, COUNT(DISTINCT clientName) - 0 - 1   AS NE_legalno FROM svd_21_22_23_24 mbf WHERE logTS <= '2024-06-15' UNION
SELECT 07 AS Mj, 2024 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 1   AS Legalno, COUNT(DISTINCT clientName) - 0 - 1   AS NE_legalno FROM svd_21_22_23_24 mbf WHERE logTS <= '2024-07-15' UNION
SELECT 08 AS Mj, 2024 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 1   AS Legalno, COUNT(DISTINCT clientName) - 0 - 1   AS NE_legalno FROM svd_21_22_23_24 mbf WHERE logTS <= '2024-08-15' UNION
SELECT 09 AS Mj, 2024 AS God, COUNT(DISTINCT clientName) - 3 AS Stanje, 1   AS Legalno, COUNT(DISTINCT clientName) - 0 - 1   AS NE_legalno FROM svd_21_22_23_24 mbf WHERE logTS <= '2024-09-15' 
#endif

