using System;
using System.Collections.Generic;
using System.Windows.Forms;
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
using XSqlDbType     = MySql.Data.MySqlClient.MySqlDbType;
#endif

public static class VvSkyLab
{

   public static /*bool*/List<VvSyncInfo> SynchronyzeWithSKY(XSqlConnection connLAN, XSqlConnection connSKY, ZXC.SkyOperation wantedOperation, bool isCheckOnly, bool isInitiatedExplicit, bool isADD_ONLY)
   {
      // TEXTHO_MATEJ: 
      //if(wantedOperation == ZXC.SkyOperation.RECEIVE) return null;

      #region Sync Locker Manager - On this server, only one SEND or RECEIVE at the time!

      if(wantedOperation == ZXC.SkyOperation.SEND   ) ZXC.SENDtoSKY_InProgress      = true;
      if(wantedOperation == ZXC.SkyOperation.RECEIVE) ZXC.RECEIVEfromSKY_InProgress = true;

      string syncLockTableName = "SKY_" + wantedOperation.ToString();
      uint   syncLockRecordID  = 0;
      VvSQL.VvLockerInfo theVvSyncLockerInfo = new VvSQL.VvLockerInfo(syncLockTableName, syncLockRecordID);

      string wantedDBname; bool lockOK;

      if(isCheckOnly == false)
      {
         wantedDBname = VvSQL.GetDbNameForThisTableName(Artikl.recordName);
         if(wantedDBname != connLAN.Database) connLAN.ChangeDatabase(wantedDBname);

         if(/*ArtiklDao da ode u PUG*/ ZXC.ArtiklDao.IsInLocker(connLAN, theVvSyncLockerInfo, false)) return null; // someone is already syncing 

         // lock does not exists. Let's lock it ! 

         theVvSyncLockerInfo.editorUID         = ZXC.vvDB_User          ;
         theVvSyncLockerInfo.clientMachineName = Environment.MachineName;
         theVvSyncLockerInfo.clientUserName    = Environment.UserName   ;

         lockOK = ZXC.ArtiklDao.InsertInLocker(connLAN, theVvSyncLockerInfo);

         if(!lockOK) ZXC.aim_emsg("Ne mogu LOCKIRATI Sinhronizaciju!");
      }

      #endregion Sync Locker Manager - On this server, only one SEND or RECEIVE at the time!

      #region Initializations

      Cursor.Current = Cursors.WaitCursor;

      bool OK = true;
      bool skyLogFound = false;
    //string wantedDBname;

      bool errOccured_ExecuteSynchronization = false;
      bool isInitiatedOnLoadOrExit = !isInitiatedExplicit;

      VvSQL.VvSkyLogEntry        skyLogEntry ;
      List<ZXC.DBactionForSrvRecID> recIDactions = null;

      DateTime prevSyncTS;
      DateTime startOfSyncTS = DateTime.MinValue;
      DateTime thisSyncTS    = DateTime.MinValue;
      DateTime endOfSyncTS   = DateTime.MinValue;

      if(wantedOperation == ZXC.SkyOperation.SEND   )  startOfSyncTS = VvSQL.GetServer_DateTime_Now(/*connSKY*/connLAN); // da li bi mozda ove trebalo izvuci izvan foreach petlje? razmisli: sta ako za neki tt traje dugo... i hoce li server biti lockiran for write ili ne?!
      if(wantedOperation == ZXC.SkyOperation.RECEIVE)  startOfSyncTS = VvSQL.GetServer_DateTime_Now(/*connLAN*/connSKY); // da li bi mozda ove trebalo izvuci izvan foreach petlje? razmisli: sta ako za neki tt traje dugo... i hoce li server biti lockiran for write ili ne?!

      ZXC.luiListaSkladista.LazyLoad();

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name );
      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name );
      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kplan> (null, VvSQL.SorterType.Konto);

      List<VvSQL.VvLanLogEntry> vvLanLogList = null;

      List<VvSyncInfo> syncInfoList = new List<VvSyncInfo>();

      #endregion Initializations

      var skyRulesList = ZXC.CURR_SkyRules.Where(sr => (sr.TheSkyOperation == wantedOperation || sr.TheSkyOperation == ZXC.SkyOperation.SendAndReceive)).OrderBy(sr => sr.DocumTTsort);

      if(skyRulesList == null) { ZXC.aim_emsg(MessageBoxIcon.Error, "Nema SkyRulesa"); return null; }

      foreach(SkyRule skyRule_rec in skyRulesList)
      {
          //if(ZXC.ThisLanServerRuleKind != skyRule_rec.RuleFor)                                                 continue; // skyRule_rec.RuleFor is NOT for me                        
          //if(wantedOperation == ZXC.SkyOperation.SEND    && skyRule_rec.Operation == ZXC.SkyOperation.RECEIVE) continue; // za trazeni SEND    prolazi samo SEND    i SendAndReceive 
          //if(wantedOperation == ZXC.SkyOperation.RECEIVE && skyRule_rec.Operation == ZXC.SkyOperation.SEND   ) continue; // za trazeni RECEIVE prolazi samo RECEIVE i SendAndReceive 

         if(isInitiatedOnLoadOrExit && wantedOperation == ZXC.SkyOperation.SEND    && skyRule_rec.NotSNDonExLd) continue; // Do not SEND    automatic On Load or Exit program 
         if(isInitiatedOnLoadOrExit && wantedOperation == ZXC.SkyOperation.RECEIVE && skyRule_rec.NotRCVonLoad) continue; // Do not RECEIVE automatic On Load         program 
          
          skyLogEntry = new VvSQL.VvSkyLogEntry();

/* === */ //                                                                                                                                                                                               
/* === */ if(wantedOperation == ZXC.SkyOperation.SEND   ) skyLogFound = VvDaoBase.GetLastSkyOrErrLogEntry(connLAN, false, ref skyLogEntry, skyRule_rec, wantedOperation/*, ZXC.ErrorStatus.NO_ERROR*/); // 
/* === */ if(wantedOperation == ZXC.SkyOperation.RECEIVE) skyLogFound = VvDaoBase.GetLastSkyOrErrLogEntry(connSKY, false, ref skyLogEntry, skyRule_rec, wantedOperation/*, ZXC.ErrorStatus.NO_ERROR*/); // 
/* === */ //                                                                                                                                                                                               

          prevSyncTS = skyLogFound ? skyLogEntry.thisSyncTS : /*DateTime.MinValue*/ ZXC.MySQL_MIN_timpestamp; //    TIMESTAMP has a range of '1970-01-01 00:00:01' UTC to '2038-01-19 03:14:07' UTC. 
          
          if(wantedOperation == ZXC.SkyOperation.SEND   )  thisSyncTS = VvSQL.GetServer_DateTime_Now(/*connSKY*/connLAN); // da li bi mozda ove trebalo izvuci izvan foreach petlje? razmisli: sta ako za neki tt traje dugo... i hoce li server biti lockiran for write ili ne?!
          if(wantedOperation == ZXC.SkyOperation.RECEIVE)  thisSyncTS = VvSQL.GetServer_DateTime_Now(/*connLAN*/connSKY); // da li bi mozda ove trebalo izvuci izvan foreach petlje? razmisli: sta ako za neki tt traje dugo... i hoce li server biti lockiran for write ili ne?!

          wantedDBname = VvSQL.GetDbNameForThisTableName(skyRule_rec.Record);
          if(wantedDBname != connLAN.Database) connLAN.ChangeDatabase(wantedDBname);
          if(wantedDBname != connSKY.Database) connSKY.ChangeDatabase(wantedDBname);

// === */                                                                                                                                                                       
/* === */ if(wantedOperation == ZXC.SkyOperation.SEND   ) recIDactions = VvSkyLab.SetupSynchronization(connLAN, skyRule_rec, ref vvLanLogList, wantedOperation, isADD_ONLY); // 
/* === */ if(wantedOperation == ZXC.SkyOperation.RECEIVE) recIDactions = VvSkyLab.SetupSynchronization(connSKY, skyRule_rec, ref vvLanLogList, wantedOperation, isADD_ONLY); // 
// === */                                                                                                                                                                       

          if(recIDactions == null || recIDactions.Count.IsZero()) continue; // there's NO NEWS by this skyRule 

          // 22.09.2017: ExecuteSynchronization opkoljen try - catch blokom. i dodana provjera ExecuteSynchronization-ovog OK-a 
          try 
          {
// === */                                                                                                                                                                                                                         
/* === */ if(wantedOperation == ZXC.SkyOperation.SEND   ) OK = VvSkyLab.ExecuteSynchronization(connLAN, connSKY, skyRule_rec, recIDactions, prevSyncTS, thisSyncTS, vvLanLogList, wantedOperation, syncInfoList, isCheckOnly); // 
/* === */ if(wantedOperation == ZXC.SkyOperation.RECEIVE) OK = VvSkyLab.ExecuteSynchronization(connSKY, connLAN, skyRule_rec, recIDactions, prevSyncTS, thisSyncTS, vvLanLogList, wantedOperation, syncInfoList, isCheckOnly); // 
// === */                                                                                                                                                                                                                         

             // 22.09.2017: 
             if(OK == false)
             {
                return ExecuteSynchronization_UNLOCK_OnFail(connLAN, isCheckOnly, syncLockTableName, syncLockRecordID, ref errOccured_ExecuteSynchronization, syncInfoList);
             }
          }
          catch 
          {
             return ExecuteSynchronization_UNLOCK_OnFail(connLAN, isCheckOnly, syncLockTableName, syncLockRecordID, ref errOccured_ExecuteSynchronization, syncInfoList);
          }

          if(skyRule_rec.Record == Artikl.recordName) ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name );
          if(skyRule_rec.Record == Kupdob.recordName) ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name );
          if(skyRule_rec.Record == Kplan .recordName) ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kplan> (null, VvSQL.SorterType.Konto);

      } // foreach(SkyRule skyRule_rec in skyRulesList)

      #region Finalizations

      // 30.08.2018: da mi ne ide nepotrebno u log ako nema promjene 
    //ZXC.SENDtoSKY_InProgress = ZXC.RECEIVEfromSKY_InProgress = false;
      if(ZXC.SENDtoSKY_InProgress      == true) ZXC.SENDtoSKY_InProgress      = false;
      if(ZXC.RECEIVEfromSKY_InProgress == true) ZXC.RECEIVEfromSKY_InProgress = false;

    //if(isCheckOnly == false)
      if(isCheckOnly == false && errOccured_ExecuteSynchronization == false)
      {
         lockOK = ZXC.ArtiklDao.DeleteFromLocker(connLAN, new VvSQL.VvLockerInfo(syncLockTableName, syncLockRecordID));
         if(!lockOK) ZXC.aim_emsg("Ne mogu ODLOCKIRATI Sinhronizaciju!");
      }

      #region syncInfoMessage

          if(wantedOperation == ZXC.SkyOperation.SEND   )  endOfSyncTS = VvSQL.GetServer_DateTime_Now(/*connSKY*/connLAN); // da li bi mozda ove trebalo izvuci izvan foreach petlje? razmisli: sta ako za neki tt traje dugo... i hoce li server biti lockiran for write ili ne?!
          if(wantedOperation == ZXC.SkyOperation.RECEIVE)  endOfSyncTS = VvSQL.GetServer_DateTime_Now(/*connLAN*/connSKY); // da li bi mozda ove trebalo izvuci izvan foreach petlje? razmisli: sta ako za neki tt traje dugo... i hoce li server biti lockiran for write ili ne?!

          ReportSynchronyzeWithSKYResults(syncInfoList, wantedOperation, isInitiatedExplicit, startOfSyncTS, endOfSyncTS, isCheckOnly, false);

      #endregion syncInfoMessage

      Cursor.Current = Cursors.Default;

      if(ZXC.ThisIsSkyLabProject == false) ZXC.ClearStatusText();

      return /*OK*/ syncInfoList;

      #endregion Finalizations
   }

   private static List<VvSyncInfo> ExecuteSynchronization_UNLOCK_OnFail(XSqlConnection connLAN, bool isCheckOnly, string syncLockTableName, uint syncLockRecordID, ref bool errOccured_ExecuteSynchronization, List<VvSyncInfo> syncInfoList)
   {
      errOccured_ExecuteSynchronization = true;

      ZXC.SENDtoSKY_InProgress = ZXC.RECEIVEfromSKY_InProgress = false;
      bool lockOK;

      if(isCheckOnly == false)
      {
         lockOK = ZXC.ArtiklDao.DeleteFromLocker(connLAN, new VvSQL.VvLockerInfo(syncLockTableName, syncLockRecordID));
         if(!lockOK) ZXC.aim_emsg("Ne mogu ODLOCKIRATI Sinhronizaciju!");
      }

      #region SendMail

      VvSkyLab.VvSyncInfo lastSyncInfo = (syncInfoList.NotEmpty() ? syncInfoList.Last() : new VvSkyLab.VvSyncInfo());

      VvMailClient mailClient = new VvMailClient();
    
      mailClient.EmailFromPasswd      = ZXC.SkyLabEmailPassword       ;
      mailClient.MailHost             = ZXC.ViperMailHost             ;
      mailClient.EmailFromAddress     = ZXC.SkyLabEmailAddress        ;
      mailClient.EmailFromDisplayName = ZXC.SkyLabEmailFromDisplayName;
      mailClient.MessageSubject       = ZXC.vvDB_ServerID + " SkyLab Syncing Exception"    ;
      mailClient.MessageBody          = "Started      : " + ZXC.programStartedOnDateTime.ToString(ZXC.VvDateAndTimeFormat) + "\n\n" +
                                        "CurrItemNo   : " + lastSyncInfo.CurrItemNo + "\n" +
                                        "TotalItems   : " + lastSyncInfo.TotalItems + "\n\n" +
                                        "Last syncInfo: " + lastSyncInfo.ToString() + "\n";

      mailClient.SendMail_Normal(false, ZXC.VektorEmailAddress);

      #endregion SendMail

      return syncInfoList; // !!! KOD POJAVE EXCEPTION-a, odi van ali prije toga pocisti locker. 
   }

   public static void ReportSynchronyzeWithSKYResults(List<VvSyncInfo> syncInfoList, ZXC.SkyOperation wantedOperation, bool isInitiatedExplicit, DateTime thisSyncTS, DateTime endOfSyncTS, bool isCheckOnly, bool isErrorLogInfo)
   {
      // za explicite javljaj uvijek, za implicite samo ako ima novosti 
      if(isInitiatedExplicit == false && syncInfoList.Count.IsZero()) return;

      // do 11.02.2015: je ovo bilo zremarckirano?! Ne kuzim zasto u scheduled VvSkyLab-u pustam ovaj dialog a ne bi smio? 
      if(ZXC.ThisIsSkyLabProject) return;

#if Nekad
      string syncInfoMessage = "Završena SKY " + wantedOperation + " operacija.\n\n";

    //syncInfoList.ForEach(sil => syncInfoMessage += 
    //   "Rule:\t"   + sil.SkyRule          + 
    //   "Zapis:\t"  + sil.VvDataRecordInfo + 
    //   "OK:\t"     + sil.IsOK             + 
    //   "Err:\t"    + sil.SqlErrNo + ":"   + sil.SqlErrMessage +
    //   "Action:\t" + sil.ResultingSrvRecIDaction.action + ":" + sil.ResultingSrvRecIDaction.lanSrvID + "-" + sil.ResultingSrvRecIDaction.lanRecID + 
    //   "\n");

      int skyRuleCount = syncInfoList                               .Select(sil => sil.SkyRule         ).Distinct().Count();
      int recordCount  = syncInfoList                               .Select(sil => sil.VvDataRecordInfo).Distinct().Count();
    //int okCount      = syncInfoList.Where(s=>s.SqlErrNo.IsZero ()).Select(sil => sil.SqlErrNo        )           .Count();
    //int errCount     = syncInfoList.Where(s=>s.SqlErrNo.NotZero()).Select(sil => sil.SqlErrNo        )           .Count();
      int okCount      = syncInfoList.Where(s=>s.IsOK == true      ).Select(sil => sil.SqlErrNo        )           .Count();
      int errCount     = syncInfoList.Where(s=>s.IsOK == false     ).Select(sil => sil.SqlErrNo        )           .Count();

      syncInfoMessage += String.Format("Za\t{0}\tSkyRulea\n\nZa\t{1}\tZapisa\n\nOK\t{2}\tOperacija\n\nERR\t{3}\tOperacija\n\n", skyRuleCount, recordCount, okCount, errCount);

      ZXC.aim_emsg(MessageBoxIcon.Information, syncInfoMessage);
#endif

      Sin_VvSyncInfoDLG syncInfoDlg = new Sin_VvSyncInfoDLG(syncInfoList, wantedOperation, isInitiatedExplicit, thisSyncTS, endOfSyncTS, isCheckOnly, isErrorLogInfo);

      syncInfoDlg.TheUC.PutDgvFields(syncInfoList);

      syncInfoDlg.ShowDialog();

      syncInfoDlg.Dispose();

   }

   public static List<ZXC.DBactionForSrvRecID> SetupSynchronization(XSqlConnection connLANorSKY, SkyRule skyRule_rec, ref List<VvSQL.VvLanLogEntry> vvLanLogList, ZXC.SkyOperation wantedOperation, bool isADD_ONLY)
   {

      // === */                                                                                                               
      /* === */ vvLanLogList = VvDaoBase.GetNonSynchronizedLANchanges_LogList(connLANorSKY, skyRule_rec, wantedOperation); // 
      // === */                                                                                                               

      if(vvLanLogList == null || vvLanLogList.Count.IsZero()) return null; // there's NO NEWS by this skyRule 

      #region preselio u VvSql.GetNonSynchronizedLANchanges_LogList_Command

    //
    //if(wantedOperation == ZXC.SkyOperation.SEND)    vvLanLogList.RemoveAll(lanLog => lanLog.isSkyTraffic); // ovo 'isSkyTraffic' je samo za probu. 100 puta razmisli sta ide ovdje 
    //                                                                                                       // pogotovo za 'SendAndReceive' SkyRules 
    //if(wantedOperation == ZXC.SkyOperation.RECEIVE) vvLanLogList.RemoveAll(lanLog => lanLog.lanServerID == ZXC.vvDB_ServerID); // izbaci logLAN novosti (sa Sky-a), a koje sam SAM izazvao 
    //
    //if(vvLanLogList.Count.IsZero()) return null; // there's NO NEWS by this skyRule 

      #endregion preselio u VvSql.GetNonSynchronizedLANchanges_LogList_Command

      var srvRecIDsDistinct = vvLanLogList.Select(entry => entry.SrvRecID).Distinct(); 

      List<ZXC.DBactionForSrvRecID> srvRecIDactions = new List<ZXC.DBactionForSrvRecID>();

      bool isADD, isRWT, isDEL;

      foreach(uint srvRecID in srvRecIDsDistinct)
      {
         var thisSrvRecIDLogEntries = vvLanLogList.Where(log => log.SrvRecID == srvRecID);

         isADD = thisSrvRecIDLogEntries.Any(log => log.action == VvSQL.DB_RW_ActionType.ADD);
         isRWT = thisSrvRecIDLogEntries.Any(log => log.action == VvSQL.DB_RW_ActionType.RWT);
         isDEL = thisSrvRecIDLogEntries.Any(log => log.action == VvSQL.DB_RW_ActionType.DEL);

         VvSQL.DB_RW_ActionType actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.NONE;

         if( isADD && !isRWT && !isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.ADD ;
         if( isADD &&  isRWT && !isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.ADD ;
         if(!isADD &&  isRWT && !isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.RWT ;
         if(!isADD &&  isRWT &&  isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.DEL ;
         if(!isADD && !isRWT &&  isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.DEL ;
         if( isADD && !isRWT &&  isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.NONE;
         if( isADD &&  isRWT &&  isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.NONE;

         // 01.12.2015: 
         if(isADD_ONLY && actionToTakeOnSyncTaker != VvSQL.DB_RW_ActionType.ADD) continue;

       //if(actionToTakeOnSyncTaker != VvSQL.DB_RW_ActionType.NONE) ... neka doda i za 'NONE' case da njegovi vvlanlog entry-i dobiju 'isSynchronised' flag 
         srvRecIDactions.Add
            (
               new ZXC.DBactionForSrvRecID
                  (
                     vvLanLogList.First(entry => entry.SrvRecID == srvRecID).recID    , 
                     vvLanLogList.First(entry => entry.SrvRecID == srvRecID).origSrvID, 
                     vvLanLogList.First(entry => entry.SrvRecID == srvRecID).origRecID, 
                     actionToTakeOnSyncTaker
                  )
            );

      } // foreach(uint recID in recIDsDistinct) 

      return srvRecIDactions;
   }

   public static List<ZXC.DBactionForSrvRecID> Setup_AfterInitNY_Synchronization(List<VvSQL.VvLanLogEntry> vvLanLogList)
   {

      if(vvLanLogList == null || vvLanLogList.Count.IsZero()) return null; // there's NO NEWS 

      var recIDsDistinct = vvLanLogList.Select(entry => entry.recID).Distinct(); 

      List<ZXC.DBactionForSrvRecID> recIDactions = new List<ZXC.DBactionForSrvRecID>();

      bool isADD, isRWT, isDEL;

      foreach(uint recID in recIDsDistinct)
      {
         var thisRecIDLogEntries = vvLanLogList.Where(log => log.recID == recID);

         isADD = thisRecIDLogEntries.Any(log => log.action == VvSQL.DB_RW_ActionType.ADD);
         isRWT = thisRecIDLogEntries.Any(log => log.action == VvSQL.DB_RW_ActionType.RWT);
         isDEL = thisRecIDLogEntries.Any(log => log.action == VvSQL.DB_RW_ActionType.DEL);

         VvSQL.DB_RW_ActionType actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.NONE;

         if( isADD && !isRWT && !isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.ADD ;
         if( isADD &&  isRWT && !isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.ADD ;
         if(!isADD &&  isRWT && !isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.RWT ;
         if(!isADD &&  isRWT &&  isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.DEL ;
         if(!isADD && !isRWT &&  isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.DEL ;
         if( isADD && !isRWT &&  isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.NONE;
         if( isADD &&  isRWT &&  isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.NONE;


         if(actionToTakeOnSyncTaker != VvSQL.DB_RW_ActionType.NONE)
         recIDactions.Add
            (
               new ZXC.DBactionForSrvRecID
                  (
                     vvLanLogList.First(entry => entry.recID == recID).recID    , 
                     vvLanLogList.First(entry => entry.recID == recID).origSrvID, 
                     vvLanLogList.First(entry => entry.recID == recID)./*origRecID*/recID, 
                     actionToTakeOnSyncTaker
                  )
            );

      } // foreach(uint recID in recIDsDistinct) 

      return recIDactions;
   }

   public static bool ExecuteSynchronization(XSqlConnection connSyncSENDER, XSqlConnection connSyncRECEIVER, SkyRule skyRule_rec, List<ZXC.DBactionForSrvRecID> recIDactions, DateTime prevSyncTS, DateTime thisSyncTS, List<VvSQL.VvLanLogEntry> vvLanLogList, ZXC.SkyOperation wantedOperation, List<VvSyncInfo> syncInfoList, bool isCheckOnly)
   {
      bool OK = true;
      bool unfinishedSyncFound = false;
    //bool ufsf_LanLogOK = false;

      ZXC.sqlErrNo = 0;
      ZXC.sqlErrMessage = "";

      uint soFarCount      = 0;
       int ofTotalCount    = 0;
      long elapsedTicks    = 0, remainTicks;
      decimal soFarKoef       ;
      TimeSpan elapsedTime = new TimeSpan(0);
      TimeSpan remainTime     ;

      VvDataRecord vvDataRecordSENDER = null;
      int nora;
      string statusText;
      VvSyncInfo currSync;
      VvSQL.VvLanLogEntry openSyncTranslanLogEntry;
      VvSQL.VvLanLogEntry ufsf_lanLogEntry;
      VvSQL.VvLanLogEntry badSender_lanLogEntry;
      VvSQL.VvSkyLogEntry skyLogEntryOfUnfinishedSync;

    //ZXC.stopWatch.Start();
      System.Diagnostics.Stopwatch syncStopWatch = System.Diagnostics.Stopwatch.StartNew();

      ofTotalCount = recIDactions.Count(ra => ra.action != VvSQL.DB_RW_ActionType.NONE);

      if(ZXC.Received_ZPC_List == null) ZXC.Received_ZPC_List = new List<ZXC.CdAndName_CommonStruct>();
      else                              ZXC.Received_ZPC_List.Clear();

      foreach(ZXC.DBactionForSrvRecID srvRecIDaction in recIDactions)
      {
         if(srvRecIDaction.action != VvSQL.DB_RW_ActionType.NONE)
         {

          //vvDataRecordSENDER = ZXC.GetNewVvDataRecordObject(skyRule_rec.Record, recIDaction.recID, recIDaction.lanSrvID, recIDaction.lanRecID);
          //OK = SetMeComplete_Record_byRecID(connSyncSENDER, vvDataRecordSENDER, skyRule_rec.Record, recIDaction);

            vvDataRecordSENDER = VvDaoBase.GetVvDataRecordByLanSrvIDAndLanRecID(connSyncSENDER, skyRule_rec.Record, srvRecIDaction.lanSrvID, srvRecIDaction.lanRecID, srvRecIDaction.action, true);

            OK = vvDataRecordSENDER != null;

            // === */ ADDREC_SKY, RWTREC_SKY, DELREC_SKY                                                                                                                              // 
            /* === */ if(OK && isCheckOnly == false)                                                                                                                                  // 
            /* === */ {                                                                                                                                                               // 
            /* === */    if(srvRecIDaction.action == VvSQL.DB_RW_ActionType.ADD)                                                                                                      // 
            /* === */    {                                                                                                                                                            // 
            /* === */       skyLogEntryOfUnfinishedSync = VvDaoBase.SetMe_SkyLogEntryByLanSrvRecID(connSyncSENDER, vvDataRecordSENDER, VvSQL.DB_RW_ActionType.UTIL, ZXC.vvDB_SKYlogTableName);                  // 
            /* === */       unfinishedSyncFound = skyLogEntryOfUnfinishedSync.operation == ZXC.SkyOperation.OpenSyncTran;                                                             // 
            /* === */       if(unfinishedSyncFound) // postoji vvLogSKY sa retkom 'OpenSyncTran'                                                                                      // 
            /* === */       {                                                                                                                                                         // 
            /* === */          ufsf_lanLogEntry = VvDaoBase.SetMe_LanLogEntryByLanSrvRecID(connSyncRECEIVER/*SENDER*/, vvDataRecordSENDER, VvSQL.DB_RW_ActionType.ADD);               // 
            /* === */          if(ufsf_lanLogEntry.logID.NotZero()) // nasao je ufsf_lanLogEntry za ADD i LanSrvRecID) - dovoljno je 'MarkAsSENDed'                                   // 
            /* === */          {                                                                                                                                                      // 
            /* === */             badSender_lanLogEntry = VvDaoBase.SetMe_LanLogEntryByLanSrvRecID(connSyncSENDER, vvDataRecordSENDER, VvSQL.DB_RW_ActionType.ADD);                   // 
            /* === */             OK = VvDaoBase.Insert_SKY_LogEntry(connSyncSENDER, vvDataRecordSENDER.SkyRule, VvSQL.DB_RW_ActionType.ADD, badSender_lanLogEntry,                   // 
            /* === */                                                VvSQL.GetServer_DateTime_Now(connSyncSENDER), ZXC.MySQL_MIN_timpestamp,                                          // 
            /* === */                                                0, "AUTO ufsf: forced 'SEND ADD action' Entry", false);                                                          // 
            /* === */                                                                                                                                                                 // 
            /* === */             CloseSyncTransaction(wantedOperation, vvDataRecordSENDER);                                                                                          // 
            /* === */             continue; /* !!! */                                                                                                                                 // 
            /* === */          }                                                                                                                                                      // 
            /* === */          else // NIJE nasao ufsf_lanLogEntry za ADD i LanSrvRecID) - UndoReceive_RollItBack                                                                     // 
            /* === */          {                                                                                                                                                      // 
            /* === */             OK = UndoReceive_RollItBack(connSyncRECEIVER, vvDataRecordSENDER, out nora);                                                                        // 
            /* === */          }                                                                                                                                                      // 
            /* === */       }                                                                                                                                                         // 
            /* === */                                                                                                                                                                 // 
            /* === */       if(OK)                                                                                                                                                    // 
            /* === */       {                                                                                                                                                         // 
            /* === */          openSyncTranslanLogEntry = vvLanLogList.Last(lanlog => lanlog.recID == srvRecIDaction.recID &&  srvRecIDaction.action != VvSQL.DB_RW_ActionType.NONE); // 
            /* === */          openSyncTranslanLogEntry.logID = 0; // da ne interferira sa JOINom kod buduceg zakljucivanja ima li sta za RECEIVEati                                  // 
            /* === */                                                                                                                                                                 // 
            /* === */          OpenSyncTransaction(connSyncSENDER, srvRecIDaction.lanSrvID, srvRecIDaction.lanRecID, openSyncTranslanLogEntry, thisSyncTS, prevSyncTS);               // 
            /* === */       }                                                                                                                                                         // 
            /* === */    }                                                                                                                                                            // 
            /* === */                                                                                                                                                                 // 
            /* === */    if(OK) OK = VvDaoBase.ExecuteSKY_DB_RW_Action(connSyncRECEIVER, vvDataRecordSENDER, srvRecIDaction.action, false);                                           // 
            /* === */ }                                                                                                                                                               // 
            // === */                                                                                                                                                                 // 

            soFarCount++;

            currSync = new VvSyncInfo() 
            { 
               WantedOperation         = wantedOperation   , 
               SkyRule                 = skyRule_rec       ,
               IsOK                    = OK                ,
               SqlErrNo                = ZXC.sqlErrNo      ,
               SqlErrMessage           = ZXC.sqlErrMessage ,
               ResultingSrvRecIDaction = srvRecIDaction    ,
               VvDataRecordInfo        = vvDataRecordSENDER == null ? "Nema zapisa lanSrvID " + srvRecIDaction.lanSrvID + " lanRecID " + srvRecIDaction.lanRecID : 
                                                                      vvDataRecordSENDER.ToString(),
               CurrItemNo              = soFarCount  ,
               TotalItems              = ofTotalCount,
            };

            syncInfoList.Add(currSync);

            #region soFar vs remaining calc

            soFarKoef     = ZXC.DivSafe(soFarCount, ofTotalCount);
            elapsedTicks += syncStopWatch.Elapsed.Ticks          ;
            elapsedTime  += syncStopWatch.Elapsed                ;
            remainTicks   = (long)(ZXC.DivSafe((decimal)elapsedTicks, soFarKoef) - elapsedTicks);
            remainTime    = new TimeSpan(remainTicks);

            #endregion soFar vs remaining calc

            statusText =
               syncStopWatch.Elapsed.TotalSeconds.ToString1Vv() + "s " +
               "(" + (elapsedTime.TotalSeconds / (double)soFarCount).ToString1Vv() + "s) " +
                (/*++*/soFarCount).ToString() +
                " of " + ofTotalCount +
               //" <"   + remainTime + "> "                              +
                string.Format(" <{0:00}:{1:00}:{2:00}> ", remainTime.Hours, remainTime.Minutes, remainTime.Seconds) +
                " " + wantedOperation +
                " ok " + currSync.IsOK +
                " " + currSync.ResultingSrvRecIDaction.action +
                " " + currSync.VvDataRecordInfo;

            syncStopWatch.Restart();

            if(ZXC.ThisIsSkyLabProject == false) { ZXC.SetStatusText(statusText); Cursor.Current = Cursors.WaitCursor; }
            else                                   ZXC.aim_log(statusText);

         } // if(srvRecIDaction.action != VvSQL.DB_RW_ActionType.NONE) 

         else { ZXC.sqlErrNo = 0; ZXC.sqlErrMessage = ""; }

         if(isCheckOnly || srvRecIDaction.action == VvSQL.DB_RW_ActionType.NONE) continue; // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 

         foreach(VvSQL.VvLanLogEntry lanLogEntry in vvLanLogList.Where(lanlog => lanlog.recID == srvRecIDaction.recID))
         {
            // === */                                                                                                                                                                           
            /* === */ if(OK) VvDaoBase.Insert_SKY_LogEntry(connSyncSENDER, skyRule_rec, srvRecIDaction.action, lanLogEntry, thisSyncTS, prevSyncTS, ZXC.sqlErrNo, ZXC.sqlErrMessage, false); // 
            /* === */ else   VvDaoBase.Insert_ERR_LogEntry(connSyncSENDER, skyRule_rec, srvRecIDaction.action, lanLogEntry, thisSyncTS, prevSyncTS, ZXC.sqlErrNo, ZXC.sqlErrMessage);        // 
            // === */                                                                                                                                                                           
         }

         // 10.11.2017: dodajemo CheckZPC provjeru kod RECEIVE-anja svakog ZPC-a 
       //if(OK && isCheckOnly == false && srvRecIDaction.action == VvSQL.DB_RW_ActionType.ADD) CloseSyncTransaction(wantedOperation, vvDataRecordSENDER);
         if(OK && isCheckOnly == false && srvRecIDaction.action == VvSQL.DB_RW_ActionType.ADD)
         {
                                                                                               CloseSyncTransaction(wantedOperation, vvDataRecordSENDER);
            #region ChkZPC News

            if(wantedOperation == ZXC.SkyOperation.RECEIVE && skyRule_rec.DocumTT == Faktur.TT_ZPC)
            {
               Faktur zpcFaktur_rec = vvDataRecordSENDER as Faktur;

               ZXC.Received_ZPC_List.Add(new ZXC.CdAndName_CommonStruct(zpcFaktur_rec.SkladCD, zpcFaktur_rec.TtNum, zpcFaktur_rec.DokDate));

               // Check ZPCova se odvija u VvForm_FormLoad_ExecuteSynchronisation_SEND_then_RECEIVE 
               // Jer u ovome trenutku nemomo Cache-a. Koristimo listu ZXC.Received_ZPC_List        
            }

            #endregion ChkZPC News
         }

      } // foreach(ZXC.DBactionForRecID recIDaction in recIDactions) 

      syncStopWatch.Stop();

      return OK;
   }

   public static int Execute_AfterInitNY_Synchronization(XSqlConnection prevYearDbConn, XSqlConnection thisYearDbConn, List<ZXC.DBactionForSrvRecID> recIDactions, string tableName)
   {
      bool OK = true;

      ZXC.sqlErrNo = 0;
      ZXC.sqlErrMessage = "";

      int debugCount = 0;

      VvDataRecord vvDataRecordPrevYear;

      foreach(ZXC.DBactionForSrvRecID srvRecIDaction in recIDactions)
      {
         if(srvRecIDaction.action == VvSQL.DB_RW_ActionType.ADD)
         {
            vvDataRecordPrevYear = ZXC.GetNewVvDataRecordObject(tableName, srvRecIDaction.recID, 0, 0);

            OK = vvDataRecordPrevYear.VvDao.SetMe_Record_byRecID(prevYearDbConn, vvDataRecordPrevYear, srvRecIDaction.recID, false);

            vvDataRecordPrevYear.VirtualLanRecID = srvRecIDaction.recID; /*ako kasnije u NY zatreba debug odakle je dosao*/

            if(OK) OK = vvDataRecordPrevYear.VvDao.ADDREC(thisYearDbConn, vvDataRecordPrevYear, false, false, false, false, true); // isSkyTraffic == true da zapamti origRecID u LanRecID 

            if(OK) debugCount++;
         }

         else { ZXC.sqlErrNo = 0; ZXC.sqlErrMessage = ""; }

      } // foreach(ZXC.DBactionForRecID recIDaction in recIDactions) 

      return debugCount;
   }

   internal static void OpenSyncTransaction(XSqlConnection connSyncSENDER, uint lanSrvID, uint lanRecID, VvSQL.VvLanLogEntry lanLogEntry, DateTime thisSyncTS, DateTime prevSyncTS) 
   { //return;

      SkyRule skyRule_rec = new SkyRule();

      skyRule_rec.CentOPS = skyRule_rec.ShopOPS = ZXC.SkyOperation.OpenSyncTran;

      VvDaoBase.Insert_SKY_LogEntry(connSyncSENDER, skyRule_rec, VvSQL.DB_RW_ActionType.UTIL, lanLogEntry, thisSyncTS, prevSyncTS, 0, "OpenSyncTransaction", true);
   }

   internal static void CloseSyncTransaction(ZXC.SkyOperation skyOperation, VvDataRecord theVvDataRecord) 
   { //return;

      bool OK = VvSkyLab.DeleteSKYlogEntry_ADD(ZXC.TheMainDbConnection, ZXC.TheSkyDbConnection, skyOperation, theVvDataRecord, true);
   }

   internal static bool DeleteSKYlogEntry_ADD(XSqlConnection connLAN, XSqlConnection connSKY, ZXC.SkyOperation skyOperation, VvDataRecord theVvDataRecord, bool isCloseSyncTransaction)
   {
      VvSQL.VvLanLogEntry lanLogEntry = new VvSQL.VvLanLogEntry();
      VvSQL.VvSkyLogEntry skyLogEntry = new VvSQL.VvSkyLogEntry();

      if(skyOperation == ZXC.SkyOperation.SEND   ) skyLogEntry = VvDaoBase.SetMe_SkyLogEntryByLanSrvRecID(connLAN, theVvDataRecord, VvSQL.DB_RW_ActionType.ADD, ZXC.vvDB_SKYlogTableName);
      if(skyOperation == ZXC.SkyOperation.RECEIVE) skyLogEntry = VvDaoBase.SetMe_SkyLogEntryByLanSrvRecID(connSKY, theVvDataRecord, VvSQL.DB_RW_ActionType.ADD, ZXC.vvDB_SKYlogTableName);

      if(skyLogEntry.skyLogID.IsZero()) // nema skyLogEntry za ADD i LanSrvRecID 
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "On {3} database\n\nIn '{2}'\n\nSkyLogEntry for ADD action does NOT exists!\n\nLanRecID: {0}\n\nLanSrvID: {1}", theVvDataRecord.VirtualLanRecID, theVvDataRecord.VirtualLanSrvID, ZXC.vvDB_SKYlogTableName, skyOperation == ZXC.SkyOperation.SEND ? "LOCAL" : "SKY in CLOUD");
         return false;
      }

      if(skyOperation == ZXC.SkyOperation.SEND   ) lanLogEntry = VvDaoBase.SetMe_LanLogEntryByLanSrvRecID(connLAN, theVvDataRecord, VvSQL.DB_RW_ActionType.ADD);
      if(skyOperation == ZXC.SkyOperation.RECEIVE) lanLogEntry = VvDaoBase.SetMe_LanLogEntryByLanSrvRecID(connSKY, theVvDataRecord, VvSQL.DB_RW_ActionType.ADD);

      if(lanLogEntry.logID.IsZero()) // nije nasao lanLogEntry za ADD i LanSrvRecID 
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "No lanLogEntry for ADD action!\n\nLanRecID: {0}\n\nLanSrvID: {1}", theVvDataRecord.VirtualLanRecID, theVvDataRecord.VirtualLanSrvID);
         return false;
      }

      string wantedDBname; 
      bool OK = true;

      wantedDBname = VvSQL.GetDbNameForThisTableName(theVvDataRecord.VirtualRecordName); 
      
      if(wantedDBname != connLAN.Database) connLAN.ChangeDatabase(wantedDBname); 
      if(wantedDBname != connSKY.Database) connSKY.ChangeDatabase(wantedDBname);

      if(skyOperation == ZXC.SkyOperation.SEND   ) OK = VvDaoBase.Delete_SKY_LogEntry(connLAN, lanLogEntry, isCloseSyncTransaction);
      if(skyOperation == ZXC.SkyOperation.RECEIVE) OK = VvDaoBase.Delete_SKY_LogEntry(connSKY, lanLogEntry, isCloseSyncTransaction);

      return OK;
   }

   internal static bool ForceSKYlogEntry_ADD(XSqlConnection connLAN, XSqlConnection connSKY, ZXC.SkyOperation skyOperation, VvDataRecord theVvDataRecord)
   {
      VvSQL.VvLanLogEntry lanLogEntry = new VvSQL.VvLanLogEntry();
      VvSQL.VvSkyLogEntry skyLogEntry = new VvSQL.VvSkyLogEntry();

      if(skyOperation == ZXC.SkyOperation.SEND   ) skyLogEntry = VvDaoBase.SetMe_SkyLogEntryByLanSrvRecID(connLAN, theVvDataRecord, VvSQL.DB_RW_ActionType.ADD, ZXC.vvDB_SKYlogTableName);
      if(skyOperation == ZXC.SkyOperation.RECEIVE) skyLogEntry = VvDaoBase.SetMe_SkyLogEntryByLanSrvRecID(connSKY, theVvDataRecord, VvSQL.DB_RW_ActionType.ADD, ZXC.vvDB_SKYlogTableName);

      if(skyLogEntry.skyLogID.NotZero()) // nasao da vec ima skyLogEntry za ADD i LanSrvRecID 
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "On {3} database\n\nIn '{2}'\n\nSkyLogEntry for ADD action already exists!\n\nLanRecID: {0}\n\nLanSrvID: {1}", theVvDataRecord.VirtualLanRecID, theVvDataRecord.VirtualLanSrvID, ZXC.vvDB_SKYlogTableName, skyOperation == ZXC.SkyOperation.SEND ? "LOCAL" : "SKY in CLOUD");
         return false;
      }

      if(skyOperation == ZXC.SkyOperation.SEND   ) lanLogEntry = VvDaoBase.SetMe_LanLogEntryByLanSrvRecID(connLAN, theVvDataRecord, VvSQL.DB_RW_ActionType.ADD);
      if(skyOperation == ZXC.SkyOperation.RECEIVE) lanLogEntry = VvDaoBase.SetMe_LanLogEntryByLanSrvRecID(connSKY, theVvDataRecord, VvSQL.DB_RW_ActionType.ADD);

      if(lanLogEntry.logID.IsZero()) // nije nasao lanLogEntry za ADD i LanSrvRecID 
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "No lanLogEntry for ADD action!\n\nLanRecID: {0}\n\nLanSrvID: {1}", theVvDataRecord.VirtualLanRecID, theVvDataRecord.VirtualLanSrvID);
         return false;
      }

      string wantedDBname; 
      bool OK = true;

      wantedDBname = VvSQL.GetDbNameForThisTableName(theVvDataRecord.VirtualRecordName); if(wantedDBname != connLAN.Database) connLAN.ChangeDatabase(wantedDBname); if(wantedDBname != connSKY.Database) connSKY.ChangeDatabase(wantedDBname);

      if(skyOperation == ZXC.SkyOperation.SEND) 
         OK = VvDaoBase.Insert_SKY_LogEntry(connLAN, theVvDataRecord.SkyRule, VvSQL.DB_RW_ActionType.ADD, lanLogEntry, VvSQL.GetServer_DateTime_Now(connLAN), ZXC.MySQL_MIN_timpestamp, /*ZXC.sqlErrNo*/0, /*ZXC.sqlErrMessage*/"Forced 'SEND ADD action' Entry", false);

      if(skyOperation == ZXC.SkyOperation.RECEIVE) 
         OK = VvDaoBase.Insert_SKY_LogEntry(connSKY, theVvDataRecord.SkyRule, VvSQL.DB_RW_ActionType.ADD, lanLogEntry, VvSQL.GetServer_DateTime_Now(connSKY), ZXC.MySQL_MIN_timpestamp, /*ZXC.sqlErrNo*/0, /*ZXC.sqlErrMessage*/"Forced 'RECEIVE ADD action' Entry", false);

      return OK;
   }

   internal static bool ForceLANlogEntry_ADD(XSqlConnection connLAN, /*XSqlConnection connSKY, ZXC.SkyOperation skyOperation,*/ VvDataRecord theVvDataRecord)
   {
      VvSQL.VvLanLogEntry lanLogEntry = new VvSQL.VvLanLogEntry();

      lanLogEntry = VvDaoBase.SetMe_LanLogEntryByLanSrvRecID(connLAN, theVvDataRecord, VvSQL.DB_RW_ActionType.ADD);

      if(lanLogEntry.logID.NotZero()) // nasao da vec ima lanLogEntry za ADD i LanSrvRecID 
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "In '{2}'\n\nLanLogEntry for ADD action already exists!\n\nLanRecID: {0}\n\nLanSrvID: {1}", theVvDataRecord.VirtualLanRecID, theVvDataRecord.VirtualLanSrvID, ZXC.vvDB_LANlogTableName);
         return false;
      }

      string wantedDBname = VvSQL.GetDbNameForThisTableName(theVvDataRecord.VirtualRecordName); if(wantedDBname != connLAN.Database) connLAN.ChangeDatabase(wantedDBname); if(wantedDBname != connLAN.Database) connLAN.ChangeDatabase(wantedDBname);

      bool isSkyTraffic = ZXC.vvDB_ServerID == ZXC.vvDB_ServerID_SkyCloud; // buduci da ovdje reguliramo missing lanLogEntry za ADD operaciju, a SkyCloud nemre ADDati,  

      bool OK = ZXC.FakturDao.Insert_LAN_LogEntry(connLAN, theVvDataRecord, VvSQL.DB_RW_ActionType.ADD, isSkyTraffic);

      return OK;
   }

   // Force RE SEND of vvDataRecord 
   internal static bool ForceLANlogEntry_RWT(XSqlConnection connLAN, /*XSqlConnection connSKY, ZXC.SkyOperation skyOperation,*/ VvDataRecord theVvDataRecord)
   {
      string wantedDBname = VvSQL.GetDbNameForThisTableName(theVvDataRecord.VirtualRecordName); if(wantedDBname != connLAN.Database) connLAN.ChangeDatabase(wantedDBname); if(wantedDBname != connLAN.Database) connLAN.ChangeDatabase(wantedDBname);

      bool OK = ZXC.FakturDao.Insert_LAN_LogEntry(connLAN, theVvDataRecord, VvSQL.DB_RW_ActionType.RWT, /*isSkyTraffic*/ false, /*isForce_RE_SEND*/ true);

      return OK;
   }

   internal static bool UndoReceive_RollItBack(XSqlConnection connLAN, VvDataRecord theVvDataRecord, out int nora)
   {
      string wantedDBname; 
      bool OK = true;

      wantedDBname = VvSQL.GetDbNameForThisTableName(theVvDataRecord.VirtualRecordName); if(wantedDBname != connLAN.Database) connLAN.ChangeDatabase(wantedDBname);

    //OK = VvDaoBase.UndoReceive_RollItBack(connLAN, theVvDataRecord.VirtualRecID, out nora);
      OK = VvDaoBase.UndoReceive_RollItBack(connLAN, theVvDataRecord.VirtualLanSrvID, theVvDataRecord.VirtualLanRecID, out nora);

      return OK;
   }

   internal static bool DeleteSKYlogEntry_ADD_AsDISPACHED_ToShop_OrCentr(XSqlConnection connLAN, string SKY_log_tableName, VvDataRecord theVvDataRecord)
   {
      VvSQL.VvLanLogEntry lanLogEntry = new VvSQL.VvLanLogEntry();
      VvSQL.VvSkyLogEntry skyLogEntry = new VvSQL.VvSkyLogEntry();

      skyLogEntry = VvDaoBase.SetMe_SkyLogEntryByLanSrvRecID(connLAN, theVvDataRecord, VvSQL.DB_RW_ActionType.ADD, SKY_log_tableName);

      if(skyLogEntry.skyLogID.IsZero()) // nema skyLogEntry za ADD i LanSrvRecID 
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "In '{2}'\n\nSkyLogEntry for ADD action does NOT exists!\n\nLanRecID: {0}\n\nLanSrvID: {1}", 
            theVvDataRecord.VirtualLanRecID, theVvDataRecord.VirtualLanSrvID, /*ZXC.vvDB_SKYlogTableName*/SKY_log_tableName);
         return false;
      }

      lanLogEntry = VvDaoBase.SetMe_LanLogEntryByLanSrvRecID(connLAN, theVvDataRecord, VvSQL.DB_RW_ActionType.ADD);

      if(lanLogEntry.logID.IsZero()) // nije nasao lanLogEntry za ADD i LanSrvRecID 
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "No lanLogEntry for ADD action!\n\nLanRecID: {0}\n\nLanSrvID: {1}", 
            theVvDataRecord.VirtualLanRecID, theVvDataRecord.VirtualLanSrvID);
         return false;
      }

      string wantedDBname; 
      bool OK = true;

      wantedDBname = VvSQL.GetDbNameForThisTableName(theVvDataRecord.VirtualRecordName); 
      
      if(wantedDBname != connLAN.Database) connLAN.ChangeDatabase(wantedDBname); 

    //OK = VvDaoBase.Delete_SKY_LogEntry                           (connLAN, lanLogEntry, false/*isCloseSyncTransaction*/);
      OK = VvDaoBase.Delete_SKY_LogEntry_AsDISPACHED_ToShop_OrCentr(connLAN, lanLogEntry, SKY_log_tableName);

      return OK;
   }


   public class VvSyncInfo
   {
      public ZXC.SkyOperation        WantedOperation         { get; set; }
      public SkyRule                 SkyRule                 { get; set; }
      public string                  VvDataRecordInfo        { get; set; }
      public bool                    IsOK                    { get; set; }
      public int                     SqlErrNo                { get; set; }
      public string                  SqlErrMessage           { get; set; }
      public ZXC.DBactionForSrvRecID ResultingSrvRecIDaction { get; set; }

      public uint CurrItemNo { get; set; }
      public  int TotalItems { get; set; }
      public bool CurrItemIsLastItem { get { return CurrItemNo == TotalItems; } }
    //public VvSQL.VvLanLogEntry TheVvLanLogEntry { get; set; }
    //public VvSQL.VvSkyLogEntry TheVvSkyLogEntry { get; set; }

      public override string ToString()
      {
         return

         "WantedOperation______\t: " + WantedOperation                + "\n" +
         "SkyRule______________\t: " + SkyRule                        + "\n" +
         "VvDataRecordInfo_____\t: " + VvDataRecordInfo               + "\n" +
         "IsOK_________________\t: " + IsOK                           + "\n" +
         "SqlErrNo_____________\t: " + SqlErrNo                       + "\n" +
         "SqlErrMessage________\t: " + SqlErrMessage                  + "\n" +
         "ResultingAction______\t\t: "+ResultingSrvRecIDaction.action + "\n" +
         "CurrItemNo___________\t: " + CurrItemNo                     + "\n" +
         "TotalItems___________\t: " + TotalItems                     + "\n" +
         "CurrItemIsLastItem___\t\t: " + CurrItemIsLastItem           + "\n" ;

      }
   }

   public class ChkSyncEntryComparer : IEqualityComparer<VvReportSourceUtil>
   {
      public bool Equals(VvReportSourceUtil x, VvReportSourceUtil y)
      {

         //Check whether the compared objects reference the same data.
         if(Object.ReferenceEquals(x, y)) return true;

         //Check whether any of the compared objects is null.
         if(Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
            return false;

         TtInfo tti = ZXC.TtInfo(x.DevName);
         bool isKolCijEqual;
       //if(x.DevName.NotEmpty() && tti.ProposeCijenaKind == ZXC.TtProposeCijenaKindEnum.Propose_PrNabCij)
         if(tti.TheTT != Faktur.TT_IRM && x.Kol.Ron2() == y.Kol.Ron2())
         {
          //isKolCijEqual = ZXC.AlmostEqualByPerc(x.TheMoney.Ron2(), y.TheMoney.Ron2(), 10.00M); // 10% tolerancije (od manje brojke 10%) 
            // 02.11.2015: zbog MSI i VMI koji se nakon CheckPrNabCij NE vracaju sa centrale na SKY i dalje na shop-ove
            // a CheckPrNabCij je u stanju proizvesti veelike razlike, ignorirati cemo TheMoney razliku za njih 
            // ubuduce mozda dialog sa pitanjem Os/Nes ili % tolerancije 
          //isKolCijEqual = ZXC.AlmostEqualByPerc(x.TheMoney.Ron2(), y.TheMoney.Ron2(), 12.00M); // 12% tolerancije (od manje brojke 12%) 
            if(tti.IsNotSyncChkPrNabCij_RtransTT) isKolCijEqual = true;
            else                                  isKolCijEqual = ZXC.AlmostEqualByPerc(x.TheMoney.Ron2(), y.TheMoney.Ron2(), 12.00M); // 12% tolerancije (od manje brojke 12%) 
         }
         else // IRM 
         {
            isKolCijEqual = ZXC.AlmostEqual(x.TheMoney.Ron2(), y.TheMoney.Ron2(), 0.05M);
         }

         //Check whether the ChkSyncEntrys' properties are equal.
         return 

            x.TheCD           == y.TheCD      && // t_skladCD        
            x.DevName         == y.DevName    && // t_tt             
            x.TheDate         == y.TheDate    && // t_skladDate      
            x.ArtiklGrCD      == y.ArtiklGrCD && // t_artiklCD       
            x.KupdobCD        == y.KupdobCD   && // t_ttNum          
            x.UtilUint        == y.UtilUint   && // t_serial         
            x.Count           == y.Count      && // COUNT(*)         
            x.Kol.Ron2()      == y.Kol.Ron2() && // SUM(t_kol)       
            isKolCijEqual                      ; // SUM(t_kol*t_cij) 

      }

      // If Equals() returns true for a pair of objects 
      // then GetHashCode() must return the same value for these objects.

      public int GetHashCode(VvReportSourceUtil ChkSyncEntry)
      {
         if(Object.ReferenceEquals(ChkSyncEntry, null)) return 0;

         int hashChkTheCD      = ChkSyncEntry.TheCD      == null ? 0 : ChkSyncEntry.TheCD     .GetHashCode();
         int hashChkDevName    = ChkSyncEntry.DevName    == null ? 0 : ChkSyncEntry.DevName   .GetHashCode();
         int hashChkTheDate    = ChkSyncEntry.TheDate    == null ? 0 : ChkSyncEntry.TheDate   .GetHashCode();
         int hashChkArtiklGrCD = ChkSyncEntry.ArtiklGrCD == null ? 0 : ChkSyncEntry.ArtiklGrCD.GetHashCode();
         int hashChkKupdobCD   = ChkSyncEntry.KupdobCD   == null ? 0 : ChkSyncEntry.KupdobCD  .GetHashCode();
         int hashChkUtilUint   = ChkSyncEntry.UtilUint   == null ? 0 : ChkSyncEntry.UtilUint  .GetHashCode();
         int hashChkCount      = ChkSyncEntry.Count      == null ? 0 : ChkSyncEntry.Count     .GetHashCode();
         int hashChkKol        = ChkSyncEntry.Kol        == null ? 0 : ChkSyncEntry.Kol       .GetHashCode();
       //int hashChkTheMoney   = ChkSyncEntry.TheMoney   == null ? 0 : ChkSyncEntry.TheMoney  .GetHashCode();


         //Calculate the hash code for the ChkSyncEntry.
         return 
            hashChkTheCD      ^
            hashChkDevName    ^
            hashChkTheDate    ^
            hashChkArtiklGrCD ^
            hashChkKupdobCD   ^
            hashChkUtilUint   ^
            hashChkCount      ^
            hashChkKol        /*^
            hashChkTheMoney   */; // moras ovdje izbaciti hash od 'TheMoney', jerbo Comparer prvo usporedjuje HASHova pa tek onda pita 'Equals' (googlaj 'C# IEqualityComparer')
      }

   }

   public class CheckSyncPairComparer : IEqualityComparer<CheckSyncPair>
   {
      public bool Equals(CheckSyncPair x, CheckSyncPair y)
      {

         //Check whether the compared objects reference the same data.
         if(Object.ReferenceEquals(x, y)) return true;

         //Check whether any of the compared objects is null.
         if(Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
            return false;

         //Check whether the ChkSyncEntrys' properties are equal.
         return 

            x.skladCD        == y.skladCD      && // t_skladCD        
            x.tt             == y.tt           && // t_tt             
            x.skladDate      == y.skladDate    && // t_skladDate      
            x.artiklCD       == y.artiklCD     && // t_artiklCD       
            x.ttNum          == y.ttNum        && // t_ttNum          
            x.tSerial        == y.tSerial       ; // t_serial         

      }

      // If Equals() returns true for a pair of objects 
      // then GetHashCode() must return the same value for these objects.

      public int GetHashCode(CheckSyncPair ChkSyncEntry)
      {
         if(Object.ReferenceEquals(ChkSyncEntry, null)) return 0;

         int hashChkskladCD   = ChkSyncEntry.skladCD   == null ? 0 : ChkSyncEntry.skladCD  .GetHashCode();
         int hashChktt        = ChkSyncEntry.tt        == null ? 0 : ChkSyncEntry.tt       .GetHashCode();
         int hashChkskladDate = ChkSyncEntry.skladDate == null ? 0 : ChkSyncEntry.skladDate.GetHashCode();
         int hashChkartiklCD  = ChkSyncEntry.artiklCD  == null ? 0 : ChkSyncEntry.artiklCD .GetHashCode();
         int hashChkttNum     = ChkSyncEntry.ttNum     == null ? 0 : ChkSyncEntry.ttNum    .GetHashCode();
         int hashChktSerial   = ChkSyncEntry.tSerial   == null ? 0 : ChkSyncEntry.tSerial  .GetHashCode();


         //Calculate the hash code for the ChkSyncEntry.
         return 
            hashChkskladCD   ^
            hashChktt        ^
            hashChkskladDate ^
            hashChkartiklCD  ^
            hashChkttNum     ^
            hashChktSerial   ;
      }

   }

   public class CheckSyncPair
   {
      //public VvReportSourceUtil LAN { get; set; }
      //public VvReportSourceUtil SKY { get; set; }

      public string   skladCD      { get; set; }
      public string   tt           { get; set; }
      public DateTime skladDate    { get; set; }
      public string   artiklCD     { get; set; }
      public int      ttNum        { get; set; }
      public int      tSerial      { get; set; }
      public int      lanCount     { get; set; }
      public decimal  lanSumKol    { get; set; }
      public decimal  lanSumKolCij { get; set; }
      public int      skyCount     { get; set; }
      public decimal  skySumKol    { get; set; }
      public decimal  skySumKolCij { get; set; }

      public override string ToString()
      {
         return skladCD+"/"+tt+"/"+skladDate.ToString(ZXC.VvDateFormat)+"/"+ttNum+"/"+tSerial;
      }
   }

   public static List<CheckSyncPair> Get_SYNC_WithSKY_Lists(XSqlConnection connLAN, XSqlConnection connSKY, uint grouppingLevel, string skladCD, string tt, DateTime dokDate)
   {
      string selectWhat = "t_skladCD, t_tt, t_skladDate, t_artiklCD, t_ttNum, t_serial, COUNT(*), ROUND(SUM(t_kol), 2), ROUND(SUM(t_kol * t_cij), 2)";
      if(grouppingLevel == 3) // no groupping at all 
      {
         selectWhat = "t_skladCD, t_tt, t_skladDate, t_artiklCD, t_ttNum, t_serial, 1, (t_kol), (t_kol * t_cij)";
      }

      string   groupBy1    = "GROUP BY t_skladCD, t_tt"                      ;
      // 02.12.2015:                                                          
    //string   groupBy2    = "GROUP BY t_skladCD, t_tt, t_skladDate"         ;
      string   groupBy2    = "GROUP BY t_skladCD, t_tt, t_skladDate, t_ttNum";
      string   orderBy     = "ORDER BY t_skladCD, t_tt, t_ttNum, t_serial"   ;
      string   grordBy     = grouppingLevel == 3 ? orderBy : grouppingLevel == 2 ? groupBy2 : groupBy1;

      string   onlySkladCD = grouppingLevel == 1 ? ""      : skladCD          ;
      string   onlyTT      = grouppingLevel == 1 ? ""      : tt               ;
      DateTime onlyDokDate = grouppingLevel == 3 ? dokDate : DateTime.MinValue;

      // 07.02.2015: sa Yesterday na Now        
      // 05.03.2021: vratio sa Now na Yesterday ALI SAMO u Vektor-u                . SKYLAB        i dalje po starom (Now) 
      // 08.03.2021: vratio sa Now na Yesterday ALI SAMO u Vektor-u i SAMO centrali. SKYLAB i SHOP i dalje po starom (Now) 
      DateTime dateDO   = ZXC.IsTEXTHOcentrala ? ZXC.Yesterday : DateTime.Now; 
    //DateTime dateDO   = DateTime.Now ;

      if(ZXC.luiListaSkladista.Count.IsZero()) ZXC.luiListaSkladista.LazyLoad();
      string[] skladCdList = ZXC.luiListaSkladista.Select(lui => lui.Cd).ToArray();

      List<VvReportSourceUtil> allLANlist = VvDaoBase.GetCheckSyncList(connLAN, dateDO, skladCdList, grordBy, selectWhat, grouppingLevel, onlySkladCD, onlyTT, onlyDokDate);
      List<VvReportSourceUtil> allSKYlist = VvDaoBase.GetCheckSyncList(connSKY, dateDO, skladCdList, grordBy, selectWhat, grouppingLevel, onlySkladCD, onlyTT, onlyDokDate);

      var diffLANlist = allLANlist.Except(allSKYlist, new ChkSyncEntryComparer());
      var diffSKYlist = allSKYlist.Except(allLANlist, new ChkSyncEntryComparer());

      List<CheckSyncPair> checkSyncPairList =
         diffLANlist                              // Source Collection          
         .FullOuterJoinJoin(diffSKYlist,          // Inner  Collection          
            lan => lan.ChkSyncGr(grouppingLevel), // PK                         
            sky => sky.ChkSyncGr(grouppingLevel), // FK                         
            new CheckSyncPairComparer(),          // comparer: Qukatz additions 
            (lan, sky) => new CheckSyncPair()     // Result Collection          
            { 
               skladCD      =       lan != null               ? lan.TheCD      : sky.TheCD     ,
               tt           =       lan != null               ? lan.DevName    : sky.DevName   ,
               skladDate    =       lan != null               ? lan.TheDate    : sky.TheDate   ,
               artiklCD     =       lan != null               ? lan.ArtiklGrCD : sky.ArtiklGrCD,
               ttNum        = (int)(lan != null               ? lan.KupdobCD   : sky.KupdobCD) ,
               tSerial      = (int)(lan != null               ? lan.UtilUint   : sky.UtilUint) ,
               lanCount     =       lan != null               ? lan.Count      : 0             ,
               lanSumKol    =       lan != null               ? lan.Kol        : 0M            ,
               lanSumKolCij =       lan != null               ? lan.TheMoney   : 0M            ,
               skyCount     =       sky != null               ? sky.Count      : 0             ,
               skySumKol    =       sky != null               ? sky.Kol        : 0M            ,
               skySumKolCij =       sky != null               ? sky.TheMoney   : 0M            ,
            }) 
            .ToList();

      return checkSyncPairList;
         
   }

   public static string ExtractErrorAndSyncDiffLinesFromLogFile(string logFileName)
   {
      List<string> lineList = /*string[] lines =*/ System.IO.File.ReadAllLines(logFileName, ZXC.VvUTF8Encoding_noBOM).ToList();

      string messageBody = "";

      for(int i = 0; i < lineList.Count; ++i)
      {
          //[13:05:20] B_Faktur_without_KupdobCD_OR_FaktEx
          //
          //Kontaktirajte SUPPORT.
          //
          //stavka: IRM-5833781 ---------------------------------------
          //
          //[13:06:22] F_Faktur_without_vvLogLAN
          //
          //Kontaktirajte SUPPORT.
          //
          //stavka: IRM-5833781 ---------------------------------------
          //
          //[13:07:00] SQL provjera gotova. Otkrivene su greĹˇke.
         
         if(lineList[i].Contains("Kontaktirajte SUPPORT"))
         {
            messageBody += lineList[i - 2] + "\n";
            
            if((i + 2) <= lineList.Count-1)
            {
               messageBody += lineList[i + 2] + "\n";
            }
         }
      }

      messageBody += "<---><---><---><---><---><---><---><---><--->\n";

      string syncDiffStartString = lineList.FirstOrDefault(line => line.Contains("Sync Diff Start:"));
      string syncDiffEndString   = lineList.FirstOrDefault(line => line.Contains("Sync Diff End."  ));

      if(syncDiffStartString.IsEmpty() || syncDiffEndString.IsEmpty()) return messageBody;

      // here we go... 

      int startIdx = lineList.IndexOf(syncDiffStartString);
      int endIdx   = lineList.IndexOf(syncDiffEndString  );

      for(int i = startIdx; i <= endIdx; ++i)
      {
         messageBody += lineList[i] + "\n";
      }

      return messageBody;
   }

   #region HRD_2023

   internal static bool TH_Addrec_EUR_Artikl(XSqlConnection conn, string rootName, decimal niceEuroMoney, string gr1cd_kategorija, string gr2cd_kind, string gr3cd_nabILIprod/*, string oldArtiklCD*/)
   {
      Artikl artikl_rec = new Artikl();

      artikl_rec.ArtiklCD   = rootName + niceEuroMoney.ToString("000.00");

      artikl_rec.Grupa1CD   = gr1cd_kategorija;
      artikl_rec.Grupa2CD   = gr2cd_kind      ;
      artikl_rec.Grupa3CD   = gr3cd_nabILIprod;

      artikl_rec.ArtiklName = artikl_rec.Grupa2Name + " " + artikl_rec.ArtiklCD;

      artikl_rec.TS         = "ROB";
      artikl_rec.PdvKat     = "25" ;
      artikl_rec.JedMj      = "kom";

    //artikl_rec.SkladCD    = "SVPS";

      artikl_rec.MadeIn     = VvForm.artMadeIn_EUR              ; // za lakse filtriranje u SQL recenicama 
      artikl_rec.Placement  = rootName                          ; // rootName                              
      artikl_rec.Starost    = ZXC.KuneIzEURa_HRD_(niceEuroMoney); // kune                                  
      artikl_rec.ImportCij  =                     niceEuroMoney ; // euri                                  
    //artikl_rec.ArtiklCD2  = oldArtiklCD                       ; // 'najslicniji' stari kunski artiklCD   

      bool OK = artikl_rec.VvDao.ADDREC(conn, artikl_rec);

      return OK;
   }

   internal static (bool, Artikl) TH_Addrec_EUR_Artikl_V2(XSqlConnection conn, string rootName, decimal niceEuroMoney, string gr1cd_kategorija, string gr2cd_kind, string gr3cd_nabILIprod/*, string oldArtiklCD*/)
   {
      Artikl artikl_rec = new Artikl();

      artikl_rec.ArtiklCD   = rootName + niceEuroMoney.ToString("000.00");

      artikl_rec.Grupa1CD   = gr1cd_kategorija;
      artikl_rec.Grupa2CD   = gr2cd_kind      ;
      artikl_rec.Grupa3CD   = gr3cd_nabILIprod;

      artikl_rec.ArtiklName = artikl_rec.Grupa2Name + " " + artikl_rec.ArtiklCD;

      artikl_rec.TS         = "ROB";
      artikl_rec.PdvKat     = "25" ;
      artikl_rec.JedMj      = "kom";

    //artikl_rec.SkladCD    = "SVPS";

      artikl_rec.MadeIn     = VvForm.artMadeIn_EUR              ; // za lakse filtriranje u SQL recenicama 
      artikl_rec.Placement  = rootName                          ; // rootName                              
      artikl_rec.Starost    = ZXC.KuneIzEURa_HRD_(niceEuroMoney); // kune                                  
      artikl_rec.ImportCij  =                     niceEuroMoney ; // euri                                  
    //artikl_rec.ArtiklCD2  = oldArtiklCD                       ; // 'najslicniji' stari kunski artiklCD   

      bool OK = artikl_rec.VvDao.ADDREC(conn, artikl_rec);

      return (OK, artikl_rec);
   }

   internal static int TH_Addrec_EUR_Artikl_OnNultiZPCs(XSqlConnection conn, Artikl artikl_rec, List<uint> nulti_ZPC_TtNumList, string rootName, decimal euroMoney, string gr1cd_kategorija, string gr2cd_kind, string gr3cd_nabILIprod)
   {
      List<string> malop_ALL_SkladCDlist = ZXC.luiListaSkladista.Where(lui => lui.Flag == true                              ).Select(l => l.Cd).ToList();
      List<string> malop5weekSkladCDlist = ZXC.luiListaSkladista.Where(lui => lui.Flag == true && ZXC.IsTH_5WeekShop(lui.Cd)).Select(l => l.Cd).ToList();

      bool isFor5weekOnly = false; // todo 

      List<uint> nulti_ZPC_TtNumList_filtered;

      if(isFor5weekOnly)
      {
         nulti_ZPC_TtNumList_filtered = nulti_ZPC_TtNumList/*.RemoveAll(kurac)*/;
      }
      else
      {
         nulti_ZPC_TtNumList_filtered = nulti_ZPC_TtNumList;
      }

      foreach(uint theTtNum in nulti_ZPC_TtNumList_filtered)
      {
         // daj mi zadnji rtrans od ovog theTtnum-a
      }

      throw new NotImplementedException();
   }

   internal static decimal Kune_From_KunskiArtiklName(string artiklCD)
   {
      string moneyStr = ZXC.SubstringSafe(artiklCD, 2);

      return ZXC.ValOrZero_Decimal(moneyStr, 0);
   }

   internal static decimal Euri_From_EuroArtiklName(string artiklCD)
   {
      string moneyStr = ZXC.SubstringSafe(artiklCD, 2);

      return ZXC.ValOrZero_Decimal(moneyStr, 2);
   }

   // Voila: 
   internal static Artikl Get_newNiceEuroArtikl(string oldNiceKunaArtiklCD, List<Artikl> artiklSifrar)
   {
      Artikl oldKunaArtikl_rec = artiklSifrar.Where(a => a.MadeIn == VvForm.artMadeIn_Kuna).SingleOrDefault(a => a.ArtiklCD == oldNiceKunaArtiklCD);

      if(oldKunaArtikl_rec == null) { ZXC.aim_emsg("nemoguce OLD_HRD"); return null; }

      Artikl newEuroArtikl_rec = null;

      try
      {
         newEuroArtikl_rec =
            artiklSifrar.Where(a => a.MadeIn == VvForm.artMadeIn_EUR &&
                                    a.Placement == oldKunaArtikl_rec.Placement &&
                                    a.ImportCij > ZXC.EURiIzKuna_HRD_(oldKunaArtikl_rec.Starost))
                                    .OrderBy(a => a.ImportCij).First();
      }
      catch
      {
         if(newEuroArtikl_rec == null) { /*ZXC.aim_emsg("{0} \n\rnenašo HRD2EUR", oldKunaArtikl_rec);*/ return null; }
      }

      return newEuroArtikl_rec;
   }

#if nepotrebno

   internal static string Get_newNiceEuroArtiklCD(string oldNiceKunaArtiklCD)
   {
      string  newNiceEuroArtiklCD;
      string  rootName    = ZXC.SubstringSafe(oldNiceKunaArtiklCD, 0, 2);

      decimal oldNiceKuna = Kune_From_KunskiArtiklName(oldNiceKunaArtiklCD);
      decimal newNiceEuro = Get_newNiceEuro(oldNiceKuna, rootName);

      newNiceEuroArtiklCD = rootName + newNiceEuro.ToString("000.00");

      return newNiceEuroArtiklCD;
   }

   private static decimal Get_newNiceEuro(decimal oldNiceKuna, string rootName)
   {
      decimal newNiceEuro;
      decimal newUglyEuro = ZXC.EURiIzKuna_HRD_(oldNiceKuna);

      decimal increment = GetIncrementForRootName(newUglyEuro, rootName);

      newNiceEuro = Get_nearestNiceEuro(newUglyEuro, increment);

      return newNiceEuro;
   }

   private static decimal GetIncrementForRootName(decimal newUglyEuro, string rootName)
   {
      decimal increment;

    //     if(newUglyEuro >=  0.25M && newUglyEuro <=   5.00M) increment = 0.25M;
    //else if(newUglyEuro >=  5.50M && newUglyEuro <=  20.00M) increment = 0.50M;
    //else if(newUglyEuro >= 21.00M && newUglyEuro <=  50.00M) increment = 1.00M;
    //else if(newUglyEuro >= 55.00M && newUglyEuro <= 135.00M) increment = 5.00M;
    //else                                                     increment = 0.00M;
           if(newUglyEuro >=  0.00M && newUglyEuro <=   5.00M) increment = 0.25M;
      else if(newUglyEuro >=  5.01M && newUglyEuro <=  20.00M) increment = 0.50M;
      else if(newUglyEuro >= 20.01M && newUglyEuro <=  50.00M) increment = 1.00M;
      else if(newUglyEuro >= 50.01M && newUglyEuro <= 999.99M) increment = 5.00M;
      else                                                     increment = 0.00M;

      return increment;
   }


   // TODO: 
   // 1. nemaju svi rootNames iste granice intervala za increment (npr 'else if(newUglyEuro >= 20.01M && newUglyEuro <=  50.00M) increment = 1.00M;') 
   // 2. fali increment 10.00 
   // 3. jos ne znamo da li se nice Euro iz ugly Eura racuna metodom, smaller, bigger ili nearest 
   private static decimal Get_nearestNiceEuro(decimal newUglyEuro, decimal increment)
   {
      decimal nearestNiceEuro;

      switch(increment)
      {
         case 0.25M: nearestNiceEuro = 0.00M; break;  
         case 0.50M: nearestNiceEuro = 0.00M; break;  
         case 1.00M: nearestNiceEuro = 0.00M; break;  
         case 5.00M: nearestNiceEuro = 0.00M; break;  

         default: nearestNiceEuro = 0.00M; break;
      }

      return nearestNiceEuro;
   }

#endif

   #endregion HRD_2023

   public static List<string> Get_TH_BagArtiklPreffixList(string bagArtiklCD)
   {
      List<string> bagArtiklPreffixList_ODJ_A = new List<string>() { "AO", "AM", "AD"       };
      List<string> bagArtiklPreffixList_ODJ_B = new List<string>() { "BO", "BM", "BD"       };
      List<string> bagArtiklPreffixList_ODJ_C = new List<string>() { "CO", "CM", "CD"       };
      List<string> bagArtiklPreffixList_CIP_A = new List<string>() { "AC"                   };
      List<string> bagArtiklPreffixList_OST_A = new List<string>() { "AT", "AR"             };
      List<string> bagArtiklPreffixList_CIP_B = new List<string>() { "BC"                   };
      List<string> bagArtiklPreffixList_OST_B = new List<string>() { "BT", "BR"             };
      List<string> bagArtiklPreffixList_TRV_B = new List<string>() { "BT"                   };
      List<string> bagArtiklPreffixList_OS_KT = new List<string>() { "KT"                   };

      switch(bagArtiklCD)
      {
         case "SQM W"  :
         case "SQM HW" :
         case "SQM S"  :
         case "SQM LS" :
         case "HATS"   :
         case "HATS S" :
         case "KOŽA  A": 
            return bagArtiklPreffixList_ODJ_A;
         case "KARNEVAL":
         case "SQ BRIC-A-BRAC":
         case "SQB S":
         case "SQB W":
            return bagArtiklPreffixList_ODJ_B;
         case "MIX S":
         case "MIX W":
            return bagArtiklPreffixList_ODJ_C;
         case "SHOES ALL":
         case "SHOES HW":
         case "SHOES LS A":
            return bagArtiklPreffixList_CIP_A;
         case "BAG A":
            return bagArtiklPreffixList_OST_A;
         case "SHOES B S":
         case "SHOES B W":
            return bagArtiklPreffixList_CIP_B;
         case "BAG B":
            return bagArtiklPreffixList_OST_B;
         case "BAG TRAVEL B":
            return bagArtiklPreffixList_TRV_B;
         case "SQ HHR":
         case "SQ HT":
         case "SQ TASKY":
            return bagArtiklPreffixList_OS_KT;
      }

      return null;
   }

}

public class TH_PriceRuleForCycleMoment
{

   #region Bussiness Propertiz

   public uint    CjenikKind  { get; set; } // C2, C3 ... C5, C6   
   public uint    WeekOfCycle { get; set; } // 1, 2, 3, 4, 5, 6    
   public uint    DayOfWeek   { get; set; } // 1, 2, 3, 4, 5, 6, 7 
   
   public decimal MaxPrice    { get; set; } // 'Sve cijene padaju na' 
   public decimal RbtSt1      { get; set; } // if(Fld_HappyHour.NotChecked) this is RbtSt1 
   public decimal HHpercent   { get; set; } // if(Fld_HappyHour.Checked   ) this is RbtSt1 
   public bool    LetakWanted { get; set; } // if(LetakWanted) RbtSt1
   public uint    ExclsvKind  { get; set; } // 0 - exclusiva NIJE posebna 1 - exc rezi na 50% 2 - exc zadrzi na 50% 
   public string  Opis        { get; set; } 

   #endregion Propertiz

   #region Other Propertiz

   public string SamePriceGroup { get { return this.CjenikKind.ToString() + "/" + this.WeekOfCycle.ToString() + "/" + this.MaxPrice.ToStringVv(); } }
   public uint   RuleID         { get { return this.CjenikKind * 100            + this.WeekOfCycle * 10             + this.DayOfWeek            ; } }
   public uint   WeekDayID      { get { return                                    this.WeekOfCycle * 10             + this.DayOfWeek            ; } }

   // RuleID dana od kada je ZPC za ovaj dan. ... Ako je danas nova cijena tada je ZPC_ID = RuleID, ako je stara onda ZPC_ID ukazuje na neki blizak prethodni dan. 'SamePriceGroup' ovdje nije primjeren ukoliko se ista cijena proteze na sljedeci tjedan.   
   public uint ZPC_ID   { get; set; }
   public uint ZPC_Age  { get; set; }

   //public uint ZPC_Age2 { get { return this.RuleID - this.ZPC_ID - (3 * (this.WeekOfCycle - 1)); } }

   public bool IsTimeForNultiZPCprice
   {
      get
      {
         return this.WeekDayID == 11; // 211, 311, 511, 611 
      }
   }

   //private Faktur nultiZpcFaktur_rec = null;

   public TimeSpan ZPC_Age_InDays { get { return new TimeSpan((int)this.ZPC_Age, 0, 0, 0); } }

   #endregion Other Propertiz

   #region Constructor

   public TH_PriceRuleForCycleMoment(
      uint    _CjenikKind  ,
      uint    _WeekOfCycle ,
      uint    _DayOfWeek   ,
      decimal _MaxPrice    ,
      decimal _RbtSt1      ,
      decimal _HHpercent   ,
      bool    _LetakWanted ,
      uint    _ExclsvKind  ,
      string  _Opis
      )
   {
      this.CjenikKind  = _CjenikKind  ;
      this.WeekOfCycle = _WeekOfCycle ;
      this.DayOfWeek   = _DayOfWeek   ;
      this.MaxPrice    = _MaxPrice    ;
      this.RbtSt1      = _RbtSt1      ;
      this.HHpercent   = _HHpercent   ;
      this.LetakWanted = _LetakWanted ;
      this.ExclsvKind  = _ExclsvKind  ;
      this.Opis        = _Opis        ;
   }

   public override string ToString()
   {
      return this.CjenikKind.ToString() + "." + this.WeekOfCycle.ToString() + "." + this.DayOfWeek.ToString() + ":" + this.MaxPrice.ToString0Vv() + "kn zpcID: " + this.ZPC_ID.ToString() + " zpcAge: " + this.ZPC_Age.ToString();
    //return this.RuleID    .ToString() + " "                                                                       + this.MaxPrice.ToString0Vv() + "kn zpcID: " + this.ZPC_ID.ToString() + " zpcAge: " + this.ZPC_Age.ToString();
   }

   #endregion Constructor

   #region Init_TH_Calendar 

   //- 'neke' 2 week poslovnice
   //- od srijede a ne od ponedjeljka, tj. MVI i IZM nije vise u subotu
   //- prvi dio dana ducan 'zatvoren', 
   //  radili bi MVI i IZM do 11h 
   //  a od 11h novi ZPC za zaduzivanje TRI
   //  a u 16h krece prodaja?! 
   //hahaha

   public enum TH_Cjenik_Kind // 2week mogu biti u 2W i 3W režimu cijena, 5week mogu biti u 5W i 6W režimu cijena ... i 4W od 08.2018. 
   {
      _NEDEFINIRANO_        =  0,
                               
      _2WShop_PON_2WCjenik_ =  2,
      _2WShop_PON_3WCjenik_ =  3,
                               
      _2WShop_SRI_2WCjenik_ =  7,
      _2WShop_SRI_3WCjenik_ =  8,
                               
      _5WShop_4WCjenik_     =  4,
      _5WShop_5WCjenik_     =  5,
      _5WShop_6WCjenik_     =  6,
      _5WShop_7WCjenik_     = 11,

      _3WShop_SRI_3WCjenik_ =  9,
      _3WShop_SRI_5WCjenik_ = 10,
      _3WShop_SRI_4WCjenik_ = 12

   }


   private static Dictionary<DateTime, TH_Cjenik_Kind> TH_Cjenik_Kind_BreakDates_2Week_PON = new Dictionary<DateTime, TH_Cjenik_Kind>
   {
      { new DateTime(2014, 12, 31), TH_Cjenik_Kind._2WShop_PON_3WCjenik_ },  // DUMMY START samo da da dummy break date 

      { new DateTime(2016, 12, 19), TH_Cjenik_Kind._2WShop_PON_3WCjenik_ },
      { new DateTime(2017, 01, 09), TH_Cjenik_Kind._2WShop_PON_2WCjenik_ },

      { new DateTime(2017, 10, 16), TH_Cjenik_Kind._2WShop_PON_3WCjenik_ },
      { new DateTime(2017, 11, 06), TH_Cjenik_Kind._2WShop_PON_2WCjenik_ },

      { new DateTime(2017, 12, 18), TH_Cjenik_Kind._2WShop_PON_3WCjenik_ },
      { new DateTime(2018, 01, 08), TH_Cjenik_Kind._2WShop_PON_2WCjenik_ },

      { new DateTime(2018, 03, 19), TH_Cjenik_Kind._2WShop_PON_3WCjenik_ },
      { new DateTime(2018, 04, 09), TH_Cjenik_Kind._2WShop_PON_2WCjenik_ },

      { new DateTime(2018, 06, 18), TH_Cjenik_Kind._2WShop_PON_3WCjenik_ },
      { new DateTime(2018, 07, 09), TH_Cjenik_Kind._2WShop_PON_2WCjenik_ },

      { new DateTime(2018, 10, 01), TH_Cjenik_Kind._2WShop_PON_3WCjenik_ },
      { new DateTime(2018, 10, 22), TH_Cjenik_Kind._2WShop_PON_2WCjenik_ },

      { new DateTime(2018, 12, 17), TH_Cjenik_Kind._2WShop_PON_3WCjenik_ },
      { new DateTime(2019, 01, 07), TH_Cjenik_Kind._2WShop_PON_2WCjenik_ },
   };

   private static Dictionary<DateTime, TH_Cjenik_Kind> TH_Cjenik_Kind_BreakDates_2Week_SRI = new Dictionary<DateTime, TH_Cjenik_Kind>
   {
    //{ new DateTime(2016, 12, 19), TH_Cjenik_Kind._2WShop_PON_3WCjenik_ },
    //{ new DateTime(2017, 01, 09), TH_Cjenik_Kind._2WShop_PON_2WCjenik_ },
    //
    //{ new DateTime(2017, 10, 16), TH_Cjenik_Kind._2WShop_PON_3WCjenik_ },
    //{ new DateTime(2017, 11, 06), TH_Cjenik_Kind._2WShop_PON_2WCjenik_ },
    //
    //{ new DateTime(2017, 12, 18), TH_Cjenik_Kind._2WShop_PON_3WCjenik_ },
    //{ new DateTime(2018, 01, 08), TH_Cjenik_Kind._2WShop_PON_2WCjenik_ },
    //
    //{ new DateTime(2018, 03, 19), TH_Cjenik_Kind._2WShop_PON_3WCjenik_ },
    //{ new DateTime(2018, 04, 09), TH_Cjenik_Kind._2WShop_PON_2WCjenik_ },
    //
    //{ new DateTime(2018, 06, 18), TH_Cjenik_Kind._2WShop_PON_3WCjenik_ },
    //{ new DateTime(2018, 07, 09), TH_Cjenik_Kind._2WShop_PON_2WCjenik_ },

      { new DateTime(2014, 12, 31), TH_Cjenik_Kind._2WShop_SRI_2WCjenik_ },  // DUMMY START samo da da dummy break date 

      { new DateTime(2018, 10, 03), TH_Cjenik_Kind._2WShop_SRI_2WCjenik_ }, // PRAPOCETAK 2WShop 'SRI' cjenika 

      { new DateTime(2018, 12, 19), TH_Cjenik_Kind._2WShop_SRI_3WCjenik_ },
      { new DateTime(2019, 01, 09), TH_Cjenik_Kind._2WShop_SRI_2WCjenik_ },

    //{ new DateTime(2018, 12, 19), TH_Cjenik_Kind._2WShop_SRI_3WCjenik_ },
      { new DateTime(2019, 07, 03), TH_Cjenik_Kind._2WShop_SRI_2WCjenik_ },

      { new DateTime(2019, 07, 31), TH_Cjenik_Kind._2WShop_SRI_3WCjenik_ },
      { new DateTime(2019, 08, 21), TH_Cjenik_Kind._2WShop_SRI_2WCjenik_ },

      { new DateTime(2019, 09, 18), TH_Cjenik_Kind._2WShop_SRI_3WCjenik_ },
      { new DateTime(2019, 10, 09), TH_Cjenik_Kind._2WShop_SRI_2WCjenik_ },

      { new DateTime(2019, 12, 18), TH_Cjenik_Kind._2WShop_SRI_3WCjenik_ },
      { new DateTime(2020, 01, 08), TH_Cjenik_Kind._2WShop_SRI_2WCjenik_ },

      { new DateTime(2020, 04, 01), TH_Cjenik_Kind._2WShop_SRI_3WCjenik_ }, //poslovnice nisu radile
      { new DateTime(2020, 04, 22), TH_Cjenik_Kind._2WShop_SRI_2WCjenik_ },

    //{ new DateTime(2020, 04, 29), TH_Cjenik_Kind._2WShop_SRI_3WCjenik_ }, iznimno, oce krenuti od cetvrtka a ne od srijede 
      { new DateTime(2020, 04, 30), TH_Cjenik_Kind._2WShop_SRI_3WCjenik_ },
      { new DateTime(2020, 05, 20), TH_Cjenik_Kind._2WShop_SRI_2WCjenik_ },

      { new DateTime(2020, 06, 03), TH_Cjenik_Kind._2WShop_SRI_3WCjenik_ },
      { new DateTime(2020, 06, 24), TH_Cjenik_Kind._2WShop_SRI_2WCjenik_ },

      { new DateTime(2020, 07, 22), TH_Cjenik_Kind._2WShop_SRI_3WCjenik_ },
      { new DateTime(2020, 08, 12), TH_Cjenik_Kind._2WShop_SRI_2WCjenik_ },

      { new DateTime(2020, 12, 16), TH_Cjenik_Kind._2WShop_SRI_3WCjenik_ },
      { new DateTime(2021, 01, 06), TH_Cjenik_Kind._2WShop_SRI_2WCjenik_ },


   };

   private static Dictionary<DateTime, TH_Cjenik_Kind> TH_Cjenik_Kind_BreakDates_5Week = new Dictionary<DateTime, TH_Cjenik_Kind>
   {
      { new DateTime(2014, 12, 31), TH_Cjenik_Kind._5WShop_5WCjenik_ },  // DUMMY START samo da da dummy break date 

      { new DateTime(2015, 07, 13), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2015, 08, 24), TH_Cjenik_Kind._5WShop_5WCjenik_ },

      { new DateTime(2017, 07, 17), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2017, 08, 28), TH_Cjenik_Kind._5WShop_5WCjenik_ },

      { new DateTime(2017, 12, 11), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2018, 01, 22), TH_Cjenik_Kind._5WShop_5WCjenik_ },

      { new DateTime(2018, 02, 26), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2018, 04, 09), TH_Cjenik_Kind._5WShop_5WCjenik_ },

      { new DateTime(2018, 07, 23), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2018, 09, 03), TH_Cjenik_Kind._5WShop_4WCjenik_ },
      { new DateTime(2018, 10, 01), TH_Cjenik_Kind._5WShop_5WCjenik_ },

      { new DateTime(2018, 12, 10), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2019, 01, 21), TH_Cjenik_Kind._5WShop_5WCjenik_ },

      { new DateTime(2019, 07, 15), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2019, 08, 26), TH_Cjenik_Kind._5WShop_5WCjenik_ },

      { new DateTime(2019, 12, 09), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2020, 01, 20), TH_Cjenik_Kind._5WShop_5WCjenik_ },

      { new DateTime(2020, 05, 04), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2020, 06, 15), TH_Cjenik_Kind._5WShop_5WCjenik_ },

      { new DateTime(2020, 12, 07), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2021, 01, 18), TH_Cjenik_Kind._5WShop_5WCjenik_ },

      { new DateTime(2021, 02, 22), TH_Cjenik_Kind._5WShop_7WCjenik_ },
    //{ new DateTime(2021, 04, 12), TH_Cjenik_Kind._5WShop_6WCjenik_ },21.04.2021. promjena cjenika
      { new DateTime(2021, 04, 12), TH_Cjenik_Kind._5WShop_5WCjenik_ },
      { new DateTime(2021, 05, 17), TH_Cjenik_Kind._5WShop_4WCjenik_ },
      { new DateTime(2021, 06, 14), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2021, 07, 26), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2021, 09, 06), TH_Cjenik_Kind._5WShop_4WCjenik_ },
      { new DateTime(2021, 10, 04), TH_Cjenik_Kind._5WShop_5WCjenik_ },
      { new DateTime(2021, 12, 13), TH_Cjenik_Kind._5WShop_6WCjenik_ },

      { new DateTime(2022, 01, 24), TH_Cjenik_Kind._5WShop_5WCjenik_ },
      { new DateTime(2022, 04, 04), TH_Cjenik_Kind._5WShop_4WCjenik_ },
      { new DateTime(2022, 05, 02), TH_Cjenik_Kind._5WShop_5WCjenik_ },
      { new DateTime(2022, 07, 11), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2022, 08, 22), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2022, 10, 03), TH_Cjenik_Kind._5WShop_5WCjenik_ },

      { new DateTime(2023, 01, 16), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2023, 02, 27), TH_Cjenik_Kind._5WShop_5WCjenik_ },
      { new DateTime(2023, 06, 12), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2023, 07, 24), TH_Cjenik_Kind._5WShop_5WCjenik_ },
      { new DateTime(2023, 12, 11), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      
      { new DateTime(2024, 01, 22), TH_Cjenik_Kind._5WShop_5WCjenik_ },
      { new DateTime(2024, 02, 26), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2024, 04, 08), TH_Cjenik_Kind._5WShop_5WCjenik_ },
      { new DateTime(2024, 05, 13), TH_Cjenik_Kind._5WShop_6WCjenik_ },
      { new DateTime(2024, 06, 24), TH_Cjenik_Kind._5WShop_5WCjenik_ },

   };

   private static Dictionary<DateTime, TH_Cjenik_Kind> TH_Cjenik_Kind_BreakDates_3Week_SRI = new Dictionary<DateTime, TH_Cjenik_Kind>
   {
      { new DateTime(2014, 12, 31), TH_Cjenik_Kind._3WShop_SRI_3WCjenik_ },  // DUMMY START samo da da dummy break date 

      { new DateTime(2020, 08, 26), TH_Cjenik_Kind._3WShop_SRI_3WCjenik_ },  // POCETAK 3WShop 'SRI' cjenika 
      
      { new DateTime(2020, 12, 09), TH_Cjenik_Kind._3WShop_SRI_5WCjenik_ }, 
      { new DateTime(2021, 01, 13), TH_Cjenik_Kind._3WShop_SRI_3WCjenik_ },   
      { new DateTime(2021, 12, 15), TH_Cjenik_Kind._3WShop_SRI_4WCjenik_ },  
      { new DateTime(2022, 01, 12), TH_Cjenik_Kind._3WShop_SRI_3WCjenik_ },
      { new DateTime(2022, 12, 14), TH_Cjenik_Kind._3WShop_SRI_4WCjenik_ },
      { new DateTime(2023, 01, 11), TH_Cjenik_Kind._3WShop_SRI_3WCjenik_ },

      { new DateTime(2024, 11, 13), TH_Cjenik_Kind._3WShop_SRI_4WCjenik_ },
      { new DateTime(2024, 12, 11), TH_Cjenik_Kind._3WShop_SRI_3WCjenik_ },

   };

#if _puse_

   public static void Init_TH_Calendar()
   {
      TH_Cjenik_Kind                   currCjenikKind, newCjenikKind;
      List<TH_PriceRuleForCycleMoment> TH_PriceRuleList_WX_CY;
      bool                             thisIsBreakDate;
      int                              ruleIDX;

      #region 2Week PON

      ZXC.TH_Cjenik_Calendar_2Week_PON = new Dictionary<DateTime, TH_PriceRuleForCycleMoment>();

      ruleIDX = 0; // offset od prvog dana ciklusa  tj. rowIdx u doticnoj THPR listi 

      currCjenikKind = TH_Cjenik_Kind._2WShop_PON_2WCjenik_;

    //for(DateTime theDate = ZXC.projectYearFirstDay                                                     ; theDate <= ZXC.projectYearLastDay; theDate += ZXC.OneDaySpan, ruleIDX++)
    //for(DateTime theDate = ZXC.projectYearFirstDay - ZXC.ThreeWeekSpan                                 ; theDate <= ZXC.projectYearLastDay; theDate += ZXC.OneDaySpan, ruleIDX++)
      for(DateTime theDate = ZXC.projectYearFirstDay - Get_TH_MaxNumOfWeeks_forCjenikKind(currCjenikKind); theDate <= ZXC.projectYearLastDay; theDate += ZXC.OneDaySpan, ruleIDX++)
      {
         thisIsBreakDate = TH_Cjenik_Kind_BreakDates_2Week_PON.TryGetValue(theDate, out newCjenikKind);

         if(thisIsBreakDate)
         {
            currCjenikKind = newCjenikKind;
            ruleIDX        =             0;
         }

       //if(ruleIDX == ((int)currCjenikKind) * 7                         ) ruleIDX = 0;
         if(ruleIDX == (Get_TH_DaysInCycle_forCjenikKind(currCjenikKind))) ruleIDX = 0; // vrijeme je za novu kolekciju, istekao je ciklus. 

         TH_PriceRuleList_WX_CY = Get_TH_PriceRuleList_forCjenikKind(currCjenikKind);

         ZXC.TH_Cjenik_Calendar_2Week_PON.Add(theDate, TH_PriceRuleList_WX_CY[ruleIDX]);
      }

      #endregion 2Week PON

      #region 2Week SRI

      ZXC.TH_Cjenik_Calendar_2Week_SRI = new Dictionary<DateTime, TH_PriceRuleForCycleMoment>();

      ruleIDX = 0; // offset od prvog dana ciklusa  tj. rowIdx u doticnoj THPR listi 

      currCjenikKind = TH_Cjenik_Kind._2WShop_SRI_2WCjenik_;

    //for(DateTime theDate = ZXC.projectYearFirstDay                                                     ; theDate <= ZXC.projectYearLastDay; theDate += ZXC.OneDaySpan, ruleIDX++)
    //for(DateTime theDate = ZXC.projectYearFirstDay - ZXC.ThreeWeekSpan                                 ; theDate <= ZXC.projectYearLastDay; theDate += ZXC.OneDaySpan, ruleIDX++)
      for(DateTime theDate = ZXC.projectYearFirstDay - Get_TH_MaxNumOfWeeks_forCjenikKind(currCjenikKind); theDate <= ZXC.projectYearLastDay; theDate += ZXC.OneDaySpan, ruleIDX++)
      {
         thisIsBreakDate = TH_Cjenik_Kind_BreakDates_2Week_SRI.TryGetValue(theDate, out newCjenikKind);

         if(thisIsBreakDate)
         {
            currCjenikKind = newCjenikKind;
            ruleIDX        =             0;
         }

       //if(ruleIDX == ((int)currCjenikKind - 5) * 7                     ) ruleIDX = 0; // NOTA BENE!!! Ovo '- 5' je tu jerbo koristimo Enum-ov index za iskazivanje WeekDurationa a moraju biti unikatni pa je ovo trik ...
         if(ruleIDX == (Get_TH_DaysInCycle_forCjenikKind(currCjenikKind))) ruleIDX = 0; // vrijeme je za novu kolekciju, istekao je ciklus. 

         TH_PriceRuleList_WX_CY = Get_TH_PriceRuleList_forCjenikKind(currCjenikKind);

         ZXC.TH_Cjenik_Calendar_2Week_SRI.Add(theDate, TH_PriceRuleList_WX_CY[ruleIDX]);
      }

      #endregion 2Week SRI

      #region 5Week 

      ZXC.TH_Cjenik_Calendar_5Week = new Dictionary<DateTime, TH_PriceRuleForCycleMoment>();

      ruleIDX = 0; // offset od prvog dana ciklusa  tj. rowIdx u doticnoj THPR listi 

      currCjenikKind = TH_Cjenik_Kind._5WShop_5WCjenik_;

    //for(DateTime theDate = ZXC.projectYearFirstDay                                                     ; theDate <= ZXC.projectYearLastDay; theDate += ZXC.OneDaySpan, ruleIDX++)
    //for(DateTime theDate = ZXC.projectYearFirstDay - ZXC.SixWeekSpan                                   ; theDate <= ZXC.projectYearLastDay; theDate += ZXC.OneDaySpan, ruleIDX++)
      for(DateTime theDate = ZXC.projectYearFirstDay - Get_TH_MaxNumOfWeeks_forCjenikKind(currCjenikKind); theDate <= ZXC.projectYearLastDay; theDate += ZXC.OneDaySpan, ruleIDX++)
      {
         thisIsBreakDate = TH_Cjenik_Kind_BreakDates_5Week.TryGetValue(theDate, out newCjenikKind);

         if(thisIsBreakDate)
         {
            currCjenikKind = newCjenikKind;
            ruleIDX        =             0;
         }

       //if(ruleIDX == ((int)currCjenikKind) * 7                         ) ruleIDX = 0;
         if(ruleIDX == (Get_TH_DaysInCycle_forCjenikKind(currCjenikKind))) ruleIDX = 0; // vrijeme je za novu kolekciju, istekao je ciklus. 

         TH_PriceRuleList_WX_CY = Get_TH_PriceRuleList_forCjenikKind(currCjenikKind);

         ZXC.TH_Cjenik_Calendar_5Week.Add(theDate, TH_PriceRuleList_WX_CY[ruleIDX]);
      }

      #endregion 5Week 

      #region 3Week SRI (od 31.08.2020)

      ZXC.TH_Cjenik_Calendar_3Week_SRI = new Dictionary<DateTime, TH_PriceRuleForCycleMoment>();

      ruleIDX = 0; // offset od prvog dana ciklusa  tj. rowIdx u doticnoj THPR listi 

      currCjenikKind = TH_Cjenik_Kind._3WShop_SRI_3WCjenik_;

    //for(DateTime theDate = ZXC.projectYearFirstDay                                                     ; theDate <= ZXC.projectYearLastDay; theDate += ZXC.OneDaySpan, ruleIDX++)
    //for(DateTime theDate = ZXC.projectYearFirstDay - ZXC.ThreeWeekSpan                                 ; theDate <= ZXC.projectYearLastDay; theDate += ZXC.OneDaySpan, ruleIDX++)
      for(DateTime theDate = ZXC.projectYearFirstDay - Get_TH_MaxNumOfWeeks_forCjenikKind(currCjenikKind); theDate <= ZXC.projectYearLastDay; theDate += ZXC.OneDaySpan, ruleIDX++)
      {
         thisIsBreakDate = TH_Cjenik_Kind_BreakDates_3Week_SRI.TryGetValue(theDate, out newCjenikKind);

         if(thisIsBreakDate)
         {
            currCjenikKind = newCjenikKind;
            ruleIDX        =             0;
         }

       //if(ruleIDX == ((int)currCjenikKind - 5) * 7                     ) ruleIDX = 0; // NOTA BENE!!! Ovo '- 5' je tu jerbo koristimo Enum-ov index za iskazivanje WeekDurationa a moraju biti unikatni pa je ovo trik ...
         if(ruleIDX == (Get_TH_DaysInCycle_forCjenikKind(currCjenikKind))) ruleIDX = 0; // vrijeme je za novu kolekciju, istekao je ciklus. 

         TH_PriceRuleList_WX_CY = Get_TH_PriceRuleList_forCjenikKind(currCjenikKind);

         ZXC.TH_Cjenik_Calendar_3Week_SRI.Add(theDate, TH_PriceRuleList_WX_CY[ruleIDX]);
      }

      #endregion 3Week SRI (od 31.08.2020)

   } // public static void Init_TH_Calendar() 

#endif

   public static void Init_TH_Calendar()
   {
      // 30.12. 2022: komentiramo TH_Cjenik_Kind_ koije se vise ne upotrebljavaju 
    //ZXC.TH_Cjenik_Calendar_2Week_PON = Init_TH_Calendar_JOB(TH_Cjenik_Kind._2WShop_PON_2WCjenik_, TH_Cjenik_Kind_BreakDates_2Week_PON);
    //ZXC.TH_Cjenik_Calendar_2Week_SRI = Init_TH_Calendar_JOB(TH_Cjenik_Kind._2WShop_SRI_2WCjenik_, TH_Cjenik_Kind_BreakDates_2Week_SRI);
      ZXC.TH_Cjenik_Calendar_5Week     = Init_TH_Calendar_JOB(TH_Cjenik_Kind._5WShop_5WCjenik_    , TH_Cjenik_Kind_BreakDates_5Week    );
      ZXC.TH_Cjenik_Calendar_3Week_SRI = Init_TH_Calendar_JOB(TH_Cjenik_Kind._3WShop_SRI_3WCjenik_, TH_Cjenik_Kind_BreakDates_3Week_SRI);

   } // public static void Init_TH_Calendar() 

   private static Dictionary<DateTime, TH_PriceRuleForCycleMoment> Init_TH_Calendar_JOB (TH_Cjenik_Kind _cjenik_Kind, Dictionary<DateTime, TH_Cjenik_Kind> _TH_Cjenik_Kind_BreakDates_XY)
   {
      Dictionary<DateTime, TH_PriceRuleForCycleMoment> TH_Cjenik_Calendar_XY = new Dictionary<DateTime, TH_PriceRuleForCycleMoment>();

      int ruleIDX = 0; // offset od prvog dana ciklusa  tj. rowIdx u doticnoj THPR listi 

      TH_Cjenik_Kind currCjenikKind = _cjenik_Kind;
      TH_Cjenik_Kind newCjenikKind;

      List<TH_PriceRuleForCycleMoment> TH_PriceRuleList_WX_CY;

      // 30.12.2022: 
      List<DateTime> breakDateList = _TH_Cjenik_Kind_BreakDates_XY.Keys.ToList();
      DateTime       prevBreakDate = breakDateList.Where(date => date < ZXC.projectYearFirstDay).Max();

      bool thisIsBreakDate;

    //for(DateTime theDate = ZXC.projectYearFirstDay                                                     ; theDate <= ZXC.projectYearLastDay; theDate += ZXC.OneDaySpan, ruleIDX++)
    //for(DateTime theDate = ZXC.projectYearFirstDay - ZXC.ThreeWeekSpan                                 ; theDate <= ZXC.projectYearLastDay; theDate += ZXC.OneDaySpan, ruleIDX++)
    //for(DateTime theDate = ZXC.projectYearFirstDay - Get_TH_MaxNumOfWeeks_forCjenikKind(currCjenikKind); theDate <= ZXC.projectYearLastDay; theDate += ZXC.OneDaySpan, ruleIDX++)
      for(DateTime theDate = prevBreakDate                                                               ; theDate <= ZXC.projectYearLastDay; theDate += ZXC.OneDaySpan, ruleIDX++)
      {
         thisIsBreakDate = _TH_Cjenik_Kind_BreakDates_XY.TryGetValue(theDate, out newCjenikKind);

         if(thisIsBreakDate)
         {
            currCjenikKind = newCjenikKind;
            ruleIDX        =             0;
         }

       //if(ruleIDX == ((int)currCjenikKind - 5) * 7                     ) ruleIDX = 0; // NOTA BENE!!! Ovo '- 5' je tu jerbo koristimo Enum-ov index za iskazivanje WeekDurationa a moraju biti unikatni pa je ovo trik ...
         if(ruleIDX == (Get_TH_DaysInCycle_forCjenikKind(currCjenikKind))) ruleIDX = 0; // vrijeme je za novu kolekciju, istekao je ciklus. 

         TH_PriceRuleList_WX_CY = Get_TH_PriceRuleList_forCjenikKind(currCjenikKind);

         TH_Cjenik_Calendar_XY.Add(theDate, TH_PriceRuleList_WX_CY[ruleIDX]);
      }

      return TH_Cjenik_Calendar_XY;
   }

   private static List<TH_PriceRuleForCycleMoment> Get_TH_PriceRuleList_forCjenikKind(TH_Cjenik_Kind TH_Cjenik_Kind)
   {
      List<TH_PriceRuleForCycleMoment> TH_PriceRuleList_WX_CY = null;

      switch(TH_Cjenik_Kind)
      {
         case TH_Cjenik_Kind._2WShop_PON_2WCjenik_: TH_PriceRuleList_WX_CY = ZXC.TH_PriceRuleList_W2_PON_C2; break;
         case TH_Cjenik_Kind._2WShop_PON_3WCjenik_: TH_PriceRuleList_WX_CY = ZXC.TH_PriceRuleList_W2_PON_C3; break;
         case TH_Cjenik_Kind._2WShop_SRI_2WCjenik_: TH_PriceRuleList_WX_CY = ZXC.TH_PriceRuleList_W2_SRI_C2; break;
         case TH_Cjenik_Kind._2WShop_SRI_3WCjenik_: TH_PriceRuleList_WX_CY = ZXC.TH_PriceRuleList_W2_SRI_C3; break;
         case TH_Cjenik_Kind._5WShop_4WCjenik_    : TH_PriceRuleList_WX_CY = ZXC.TH_PriceRuleList_W5_C4    ; break;
         case TH_Cjenik_Kind._5WShop_5WCjenik_    : TH_PriceRuleList_WX_CY = ZXC.TH_PriceRuleList_W5_C5    ; break;
         case TH_Cjenik_Kind._5WShop_6WCjenik_    : TH_PriceRuleList_WX_CY = ZXC.TH_PriceRuleList_W5_C6    ; break;
         case TH_Cjenik_Kind._5WShop_7WCjenik_    : TH_PriceRuleList_WX_CY = ZXC.TH_PriceRuleList_W5_C7    ; break; //24.11.2020.
         case TH_Cjenik_Kind._3WShop_SRI_3WCjenik_: TH_PriceRuleList_WX_CY = ZXC.TH_PriceRuleList_W3_SRI_C3; break;
         case TH_Cjenik_Kind._3WShop_SRI_5WCjenik_: TH_PriceRuleList_WX_CY = ZXC.TH_PriceRuleList_W3_SRI_C5; break; //24.11.2020.
         case TH_Cjenik_Kind._3WShop_SRI_4WCjenik_: TH_PriceRuleList_WX_CY = ZXC.TH_PriceRuleList_W3_SRI_C4; break; //24.11.2020.

         default: ZXC.aim_emsg(MessageBoxIcon.Error, "Get_TH_PriceRuleList_forCjenikKind is null! TH_Cjenik_Kind[" + TH_Cjenik_Kind.ToString()); break;
      }

      return TH_PriceRuleList_WX_CY;
   }

   private static int Get_TH_DaysInCycle_forCjenikKind(TH_Cjenik_Kind TH_Cjenik_Kind)
   {
      int TH_DaysInCycle_WX_CY = -1;

      switch(TH_Cjenik_Kind)
      {
         case TH_Cjenik_Kind._2WShop_PON_2WCjenik_: TH_DaysInCycle_WX_CY = 2 /* num of weeks */ * 7; break;
         case TH_Cjenik_Kind._2WShop_PON_3WCjenik_: TH_DaysInCycle_WX_CY = 3 /* num of weeks */ * 7; break;
         case TH_Cjenik_Kind._2WShop_SRI_2WCjenik_: TH_DaysInCycle_WX_CY = 2 /* num of weeks */ * 7; break;
         case TH_Cjenik_Kind._2WShop_SRI_3WCjenik_: TH_DaysInCycle_WX_CY = 3 /* num of weeks */ * 7; break;
         case TH_Cjenik_Kind._5WShop_4WCjenik_    : TH_DaysInCycle_WX_CY = 4 /* num of weeks */ * 7; break;
         case TH_Cjenik_Kind._5WShop_5WCjenik_    : TH_DaysInCycle_WX_CY = 5 /* num of weeks */ * 7; break;
         case TH_Cjenik_Kind._5WShop_6WCjenik_    : TH_DaysInCycle_WX_CY = 6 /* num of weeks */ * 7; break;
         case TH_Cjenik_Kind._5WShop_7WCjenik_    : TH_DaysInCycle_WX_CY = 7 /* num of weeks */ * 7; break; //24.11.2020.
         case TH_Cjenik_Kind._3WShop_SRI_3WCjenik_: TH_DaysInCycle_WX_CY = 3 /* num of weeks */ * 7; break;
         case TH_Cjenik_Kind._3WShop_SRI_5WCjenik_: TH_DaysInCycle_WX_CY = 5 /* num of weeks */ * 7; break; //24.11.2020.
         case TH_Cjenik_Kind._3WShop_SRI_4WCjenik_: TH_DaysInCycle_WX_CY = 4 /* num of weeks */ * 7; break; //30.11.2021.

         default: ZXC.aim_emsg(MessageBoxIcon.Error, "Get_TH_DaysInCycle_forCjenikKind is null! TH_Cjenik_Kind[" + TH_Cjenik_Kind.ToString()); break;
      }

      return TH_DaysInCycle_WX_CY;
   }

   private static TimeSpan Get_TH_MaxNumOfWeeks_forCjenikKind(TH_Cjenik_Kind TH_Cjenik_Kind)
   {
      TimeSpan TH_MaxNumOfWeeks_WX_CY = TimeSpan.MinValue;

      switch(TH_Cjenik_Kind)
      {
         case TH_Cjenik_Kind._2WShop_PON_2WCjenik_: TH_MaxNumOfWeeks_WX_CY = ZXC.ThreeWeekSpan; break;
         case TH_Cjenik_Kind._2WShop_PON_3WCjenik_: TH_MaxNumOfWeeks_WX_CY = ZXC.ThreeWeekSpan; break;
         case TH_Cjenik_Kind._2WShop_SRI_2WCjenik_: TH_MaxNumOfWeeks_WX_CY = ZXC.ThreeWeekSpan; break;
         case TH_Cjenik_Kind._2WShop_SRI_3WCjenik_: TH_MaxNumOfWeeks_WX_CY = ZXC.ThreeWeekSpan; break;

       //24.11.2020. 5WShop_7WCjenik_ novost pa se rok trajanja ciklusa povecava?!
       //case TH_Cjenik_Kind._5WShop_4WCjenik_    : TH_MaxNumOfWeeks_WX_CY = ZXC.SixWeekSpan  ; break;
       //case TH_Cjenik_Kind._5WShop_5WCjenik_    : TH_MaxNumOfWeeks_WX_CY = ZXC.SixWeekSpan  ; break;
       //case TH_Cjenik_Kind._5WShop_6WCjenik_    : TH_MaxNumOfWeeks_WX_CY = ZXC.SixWeekSpan  ; break;
         case TH_Cjenik_Kind._5WShop_4WCjenik_    : TH_MaxNumOfWeeks_WX_CY = ZXC.SevenWeekSpan; break;
         case TH_Cjenik_Kind._5WShop_5WCjenik_    : TH_MaxNumOfWeeks_WX_CY = ZXC.SevenWeekSpan; break;
         case TH_Cjenik_Kind._5WShop_6WCjenik_    : TH_MaxNumOfWeeks_WX_CY = ZXC.SevenWeekSpan; break;
         case TH_Cjenik_Kind._5WShop_7WCjenik_    : TH_MaxNumOfWeeks_WX_CY = ZXC.SevenWeekSpan; break;

       //24.11.2020. _3WShop_SRI_5WCjenik_  novost pa se rok trajanja ciklusa povecava?!
       //case TH_Cjenik_Kind._3WShop_SRI_3WCjenik_: TH_MaxNumOfWeeks_WX_CY = ZXC.ThreeWeekSpan; break;
         case TH_Cjenik_Kind._3WShop_SRI_3WCjenik_: TH_MaxNumOfWeeks_WX_CY = ZXC.FiveWeekSpan; break;
         case TH_Cjenik_Kind._3WShop_SRI_5WCjenik_: TH_MaxNumOfWeeks_WX_CY = ZXC.FiveWeekSpan; break;
         case TH_Cjenik_Kind._3WShop_SRI_4WCjenik_: TH_MaxNumOfWeeks_WX_CY = ZXC.FiveWeekSpan; break;

         default: ZXC.aim_emsg(MessageBoxIcon.Error, "Get_TH_MaxNumOfWeeks_forCjenikKind is null! TH_Cjenik_Kind[" + TH_Cjenik_Kind.ToString()); break;
      }

      return TH_MaxNumOfWeeks_WX_CY;
   }

   #endregion Init_TH_Calendar 

   #region Init_SetAndCheck_TH_PriceRuleList

   public static void Init_TH_PriceRuleList()
   {

      #region TH_PriceRuleList_W2_PON_C2

      ZXC.TH_PriceRuleList_W2_PON_C2 = new List<TH_PriceRuleForCycleMoment>()
      {
                                     // CK Tjed Dan  Max Rbt HHprc  Letak  ExclsvKind   
         new TH_PriceRuleForCycleMoment(2,    1,  1,   0,  0,    0, false, 0, "W2 Tjedan 1 Dan 1 regular"),
         new TH_PriceRuleForCycleMoment(2,    1,  2, 30M,  0,    0, false, 0, "W2 Tjedan 1 Dan 2 do 30kn"),
         new TH_PriceRuleForCycleMoment(2,    1,  3, 25M,  0,    0, false, 0, "W2 Tjedan 1 Dan 3 do 25kn"),
         new TH_PriceRuleForCycleMoment(2,    1,  4, 25M,  0,    0, false, 0, "W2 Tjedan 1 Dan 4 do 25kn"),
         new TH_PriceRuleForCycleMoment(2,    1,  5, 20M,  0,    0, false, 0, "W2 Tjedan 1 Dan 5 do 20kn"),
         new TH_PriceRuleForCycleMoment(2,    1,  6, 20M,  0,    0, false, 0, "W2 Tjedan 1 Dan 6 do 20kn"),
         new TH_PriceRuleForCycleMoment(2,    1,  7, 20M,  0,    0, false, 0, "W2 Tjedan 1 Dan 7 do 20kn"),
         new TH_PriceRuleForCycleMoment(2,    2,  1, 15M,  0,    0, false, 0, "W2 Tjedan 2 Dan 1 do 15kn"),
         new TH_PriceRuleForCycleMoment(2,    2,  2, 15M,  0,    0, false, 0, "W2 Tjedan 2 Dan 2 do 15kn"),
         new TH_PriceRuleForCycleMoment(2,    2,  3, 10M,  0,    0, false, 0, "W2 Tjedan 2 Dan 3 do 10kn"),
         new TH_PriceRuleForCycleMoment(2,    2,  4,  7M,  0,    0, false, 0, "W2 Tjedan 2 Dan 4 do  7kn"),
         new TH_PriceRuleForCycleMoment(2,    2,  5,  5M,  0,    0, false, 0, "W2 Tjedan 2 Dan 5 do  5kn"),
         new TH_PriceRuleForCycleMoment(2,    2,  6,  3M,  0,    0, false, 0, "W2 Tjedan 2 Dan 6 do  3kn"),
         new TH_PriceRuleForCycleMoment(2,    2,  7,  3M,  0,    0, false, 0, "W2 Tjedan 2 Dan 7 do  3kn"),
      };

      SetAndCheck_TH_PriceRuleList(ZXC.TH_PriceRuleList_W2_PON_C2, TH_Cjenik_Kind._2WShop_PON_2WCjenik_);

      #endregion TH_PriceRuleList_W2_PON_C2

      #region TH_PriceRuleList_W2_PON_C3

      ZXC.TH_PriceRuleList_W2_PON_C3 = new List<TH_PriceRuleForCycleMoment>()
      {
                                     // CK Tjed Dan  Max Rbt HHprc  Letak  ExclsvKind   
         new TH_PriceRuleForCycleMoment(3,    1,  1,   0,  0,    0, false, 0, "W3 Tjedan 1 Dan 1 regular"),
         new TH_PriceRuleForCycleMoment(3,    1,  2, 30M,  0,    0, false, 0, "W3 Tjedan 1 Dan 2 do 30kn"),
         new TH_PriceRuleForCycleMoment(3,    1,  3, 30M,  0,    0, false, 0, "W3 Tjedan 1 Dan 3 do 30kn"),
         new TH_PriceRuleForCycleMoment(3,    1,  4, 25M,  0,    0, false, 0, "W3 Tjedan 1 Dan 4 do 25kn"),
         new TH_PriceRuleForCycleMoment(3,    1,  5, 25M,  0,    0, false, 0, "W3 Tjedan 1 Dan 5 do 25kn"),
         new TH_PriceRuleForCycleMoment(3,    1,  6, 25M,  0,    0, false, 0, "W3 Tjedan 1 Dan 6 do 25kn"),
         new TH_PriceRuleForCycleMoment(3,    1,  7, 25M,  0,    0, false, 0, "W3 Tjedan 1 Dan 7 do 25kn"),
         new TH_PriceRuleForCycleMoment(3,    2,  1, 20M,  0,    0, false, 0, "W3 Tjedan 2 Dan 1 do 20kn"),
         new TH_PriceRuleForCycleMoment(3,    2,  2, 20M,  0,    0, false, 0, "W3 Tjedan 2 Dan 2 do 20kn"),
         new TH_PriceRuleForCycleMoment(3,    2,  3, 20M,  0,    0, false, 0, "W3 Tjedan 2 Dan 3 do 20kn"),
         new TH_PriceRuleForCycleMoment(3,    2,  4, 20M,  0,    0, false, 0, "W3 Tjedan 2 Dan 4 do 20kn"),
         new TH_PriceRuleForCycleMoment(3,    2,  5, 20M,  0,    0, false, 0, "W3 Tjedan 2 Dan 5 do 20kn"),
         new TH_PriceRuleForCycleMoment(3,    2,  6, 15M,  0,    0, false, 0, "W3 Tjedan 2 Dan 6 do 15kn"),
         new TH_PriceRuleForCycleMoment(3,    2,  7, 15M,  0,    0, false, 0, "W3 Tjedan 2 Dan 7 do 15kn"),
         new TH_PriceRuleForCycleMoment(3,    3,  1, 15M,  0,    0, false, 0, "W3 Tjedan 3 Dan 1 do 15kn"),
         new TH_PriceRuleForCycleMoment(3,    3,  2, 15M,  0,    0, false, 0, "W3 Tjedan 3 Dan 2 do 15kn"),
         new TH_PriceRuleForCycleMoment(3,    3,  3, 10M,  0,    0, false, 0, "W3 Tjedan 3 Dan 3 do 10kn"),
         new TH_PriceRuleForCycleMoment(3,    3,  4, 10M,  0,    0, false, 0, "W3 Tjedan 3 Dan 4 do 10kn"),
         new TH_PriceRuleForCycleMoment(3,    3,  5, 10M,  0,    0, false, 0, "W3 Tjedan 3 Dan 5 do 10kn"),
         new TH_PriceRuleForCycleMoment(3,    3,  6,  7M,  0,    0, false, 0, "W3 Tjedan 3 Dan 6 do  7kn"),
         new TH_PriceRuleForCycleMoment(3,    3,  7,  7M,  0,    0, false, 0, "W3 Tjedan 3 Dan 7 do  7kn"),
      };

      SetAndCheck_TH_PriceRuleList(ZXC.TH_PriceRuleList_W2_PON_C3, TH_Cjenik_Kind._2WShop_PON_3WCjenik_);

      #endregion TH_PriceRuleList_W2_PON_C3

      #region TH_PriceRuleList_W2_SRI_C2

      ZXC.TH_PriceRuleList_W2_SRI_C2 = new List<TH_PriceRuleForCycleMoment>()
      {
                                     // CK Tjed Dan  Max Rbt HHprc  Letak  ExclsvKind   
         new TH_PriceRuleForCycleMoment(2,    1,  1,   0,  0,    0, false, 0, "W2/2 Tjedan 1 Dan 1 (SRI) regular"),
         new TH_PriceRuleForCycleMoment(2,    1,  2, 30M,  0,    0, false, 0, "W2/2 Tjedan 1 Dan 2 (ČET) do 30kn"),
         new TH_PriceRuleForCycleMoment(2,    1,  3, 30M,  0,    0, false, 0, "W2/2 Tjedan 1 Dan 3 (PET) do 30kn"),
         new TH_PriceRuleForCycleMoment(2,    1,  4, 25M,  0,    0, false, 0, "W2/2 Tjedan 1 Dan 4 (SUB) do 25kn"),
         new TH_PriceRuleForCycleMoment(2,    1,  5, 25M,  0,    0, false, 0, "W2/2 Tjedan 1 Dan 5 (NED) do 25kn"),
         new TH_PriceRuleForCycleMoment(2,    1,  6, 25M,  0,    0, false, 0, "W2/2 Tjedan 1 Dan 6 (PON) do 25kn"),
         new TH_PriceRuleForCycleMoment(2,    1,  7, 20M,  0,    0, false, 0, "W2/2 Tjedan 1 Dan 7 (UTO) do 20kn"),
         new TH_PriceRuleForCycleMoment(2,    2,  1, 15M,  0,    0, false, 0, "W2/2 Tjedan 2 Dan 1 (SRI) do 15kn"),
         new TH_PriceRuleForCycleMoment(2,    2,  2, 10M,  0,    0, false, 0, "W2/2 Tjedan 2 Dan 2 (ČET) do 10kn"),
         new TH_PriceRuleForCycleMoment(2,    2,  3, 10M,  0,    0, false, 0, "W2/2 Tjedan 2 Dan 3 (PET) do 10kn"),
         new TH_PriceRuleForCycleMoment(2,    2,  4,  7M,  0,    0, false, 0, "W2/2 Tjedan 2 Dan 4 (SUB) do  7kn"),
         new TH_PriceRuleForCycleMoment(2,    2,  5,  7M,  0,    0, false, 0, "W2/2 Tjedan 2 Dan 5 (NED) do  7kn"),
         new TH_PriceRuleForCycleMoment(2,    2,  6,  5M,  0,    0, false, 0, "W2/2 Tjedan 2 Dan 6 (PON) do  5kn"),
         new TH_PriceRuleForCycleMoment(2,    2,  7,  3M,  0,    0, false, 0, "W2/2 Tjedan 2 Dan 7 (UTO) do  3kn"),
      };

      SetAndCheck_TH_PriceRuleList(ZXC.TH_PriceRuleList_W2_SRI_C2, TH_Cjenik_Kind._2WShop_SRI_2WCjenik_);

      #endregion TH_PriceRuleList_W2_SRI_C2

      #region TH_PriceRuleList_W2_SRI_C3


      ZXC.TH_PriceRuleList_W2_SRI_C3 = new List<TH_PriceRuleForCycleMoment>()
      {
                                     // CK Tjed Dan  Max Rbt HHprc  Letak  ExclsvKind   
         new TH_PriceRuleForCycleMoment(3,    1,  1,   0,  0,    0, false, 0, "W2/3 Tjedan 1 Dan 1 (SRI) regular"),
         new TH_PriceRuleForCycleMoment(3,    1,  2, 30M,  0,    0, false, 0, "W2/3 Tjedan 1 Dan 2 (ČET) do 30kn"),
         new TH_PriceRuleForCycleMoment(3,    1,  3, 30M,  0,    0, false, 0, "W2/3 Tjedan 1 Dan 3 (PET) do 30kn"),
         new TH_PriceRuleForCycleMoment(3,    1,  4, 25M,  0,    0, false, 0, "W2/3 Tjedan 1 Dan 4 (SUB) do 25kn"),
         new TH_PriceRuleForCycleMoment(3,    1,  5, 25M,  0,    0, false, 0, "W2/3 Tjedan 1 Dan 5 (NED) do 25kn"),
         new TH_PriceRuleForCycleMoment(3,    1,  6, 25M,  0,    0, false, 0, "W2/3 Tjedan 1 Dan 6 (PON) do 25kn"),
         new TH_PriceRuleForCycleMoment(3,    1,  7, 20M,  0,    0, false, 0, "W2/3 Tjedan 1 Dan 7 (UTO) do 20kn"),
         new TH_PriceRuleForCycleMoment(3,    2,  1, 20M,  0,    0, false, 0, "W2/3 Tjedan 2 Dan 1 (SRI) do 20kn"),
         new TH_PriceRuleForCycleMoment(3,    2,  2, 15M,  0,    0, false, 0, "W2/3 Tjedan 2 Dan 2 (ČET) do 15kn"),
         new TH_PriceRuleForCycleMoment(3,    2,  3, 15M,  0,    0, false, 0, "W2/3 Tjedan 2 Dan 3 (PET) do 15kn"),
         new TH_PriceRuleForCycleMoment(3,    2,  4, 10M,  0,    0, false, 0, "W2/3 Tjedan 2 Dan 4 (SUB) do 10kn"),
         new TH_PriceRuleForCycleMoment(3,    2,  5, 10M,  0,    0, false, 0, "W2/3 Tjedan 2 Dan 5 (NED) do 10kn"),
         new TH_PriceRuleForCycleMoment(3,    2,  6, 10M,  0,    0, false, 0, "W2/3 Tjedan 2 Dan 6 (PON) do 10kn"),
         new TH_PriceRuleForCycleMoment(3,    2,  7,  7M,  0,    0, false, 0, "W2/3 Tjedan 2 Dan 7 (UTO) do  7kn"),
         new TH_PriceRuleForCycleMoment(3,    3,  1,  7M,  0,    0, false, 0, "W2/3 Tjedan 3 Dan 1 (SRI) do  7kn"),
         new TH_PriceRuleForCycleMoment(3,    3,  2,  7M,  0,    0, false, 0, "W2/3 Tjedan 3 Dan 2 (ČET) do  7kn"),
         new TH_PriceRuleForCycleMoment(3,    3,  3,  7M,  0,    0, false, 0, "W2/3 Tjedan 3 Dan 3 (PET) do  7kn"),
         new TH_PriceRuleForCycleMoment(3,    3,  4,  5M,  0,    0, false, 0, "W2/3 Tjedan 3 Dan 4 (SUB) do  5kn"),
         new TH_PriceRuleForCycleMoment(3,    3,  5,  5M,  0,    0, false, 0, "W2/3 Tjedan 3 Dan 5 (NED) do  5kn"),
         new TH_PriceRuleForCycleMoment(3,    3,  6,  5M,  0,    0, false, 0, "W2/3 Tjedan 3 Dan 6 (PON) do  5kn"),
         new TH_PriceRuleForCycleMoment(3,    3,  7,  3M,  0,    0, false, 0, "W2/3 Tjedan 3 Dan 7 (UTO) do  3kn"),
      };

      // 29.04.2020: iznimno, sljedeci 2 na 3 wuuek promjena pocinje u četvrtak a ne u srijedu i traje 20 dana do utorka 19.05.2020 
      // a povrat na standardni 2week pocinje od srijede 20.05.2020, pa ovo dole privremeno komentirano se moze vratiti             
      // 02.06.2020. ovo je primjerak kako je bilo kada su htjeli da ciklus pocne u cetvrtak a traje do srijede, iznimno 20 dana
      //ZXC.TH_PriceRuleList_W2_SRI_C3 = new List<TH_PriceRuleForCycleMoment>()
      //{
      //                               // CK Tjed Dan  Max Rbt HHprc  Letak  ExclsvKind   
      //   new TH_PriceRuleForCycleMoment(3,    1,  1,   0,  0,    0, false, 0, "W3 Tjedan 1 Dan 1 (ČET) regular"),
      //   new TH_PriceRuleForCycleMoment(3,    1,  2, 30M,  0,    0, false, 0, "W3 Tjedan 1 Dan 2 (PET) do 30kn"),
      //   new TH_PriceRuleForCycleMoment(3,    1,  3, 30M,  0,    0, false, 0, "W3 Tjedan 1 Dan 3 (SUB) do 30kn"),
      //   new TH_PriceRuleForCycleMoment(3,    1,  4, 30M,  0,    0, false, 0, "W3 Tjedan 1 Dan 4 (NED) do 30kn"),
      //   new TH_PriceRuleForCycleMoment(3,    1,  5, 30M,  0,    0, false, 0, "W3 Tjedan 1 Dan 5 (PON) do 30kn"),
      //   new TH_PriceRuleForCycleMoment(3,    1,  6, 25M,  0,    0, false, 0, "W3 Tjedan 1 Dan 6 (UTO) do 25kn"),
      //   new TH_PriceRuleForCycleMoment(3,    1,  7, 25M,  0,    0, false, 0, "W3 Tjedan 2 Dan 7 (SRI) do 25kn"),
      //   new TH_PriceRuleForCycleMoment(3,    2,  1, 20M,  0,    0, false, 0, "W3 Tjedan 2 Dan 1 (ČET) do 20kn"),
      //   new TH_PriceRuleForCycleMoment(3,    2,  2, 20M,  0,    0, false, 0, "W3 Tjedan 2 Dan 2 (PET) do 20kn"),
      //   new TH_PriceRuleForCycleMoment(3,    2,  3, 15M,  0,    0, false, 0, "W3 Tjedan 2 Dan 3 (SUB) do 15kn"),
      //   new TH_PriceRuleForCycleMoment(3,    2,  4, 15M,  0,    0, false, 0, "W3 Tjedan 2 Dan 4 (NED) do 15kn"),
      //   new TH_PriceRuleForCycleMoment(3,    2,  5, 15M,  0,    0, false, 0, "W3 Tjedan 2 Dan 5 (PON) do 15kn"),
      //   new TH_PriceRuleForCycleMoment(3,    2,  6, 10M,  0,    0, false, 0, "W3 Tjedan 2 Dan 6 (UTO) do 10kn"),
      //   new TH_PriceRuleForCycleMoment(3,    2,  7, 10M,  0,    0, false, 0, "W3 Tjedan 3 Dan 7 (SRI) do 10kn"),
      //   new TH_PriceRuleForCycleMoment(3,    3,  1,  7M,  0,    0, false, 0, "W3 Tjedan 3 Dan 1 (ČET) do  7kn"),
      //   new TH_PriceRuleForCycleMoment(3,    3,  2,  7M,  0,    0, false, 0, "W3 Tjedan 3 Dan 2 (PET) do  7kn"),
      //   new TH_PriceRuleForCycleMoment(3,    3,  3,  5M,  0,    0, false, 0, "W3 Tjedan 3 Dan 3 (SUB) do  5kn"),
      //   new TH_PriceRuleForCycleMoment(3,    3,  4,  5M,  0,    0, false, 0, "W3 Tjedan 3 Dan 4 (NED) do  5kn"),
      //   new TH_PriceRuleForCycleMoment(3,    3,  5,  5M,  0,    0, false, 0, "W3 Tjedan 3 Dan 5 (PON) do  5kn"),
      //   new TH_PriceRuleForCycleMoment(3,    3,  6,  3M,  0,    0, false, 0, "W3 Tjedan 3 Dan 6 (UTO) do  3kn"),
      //   new TH_PriceRuleForCycleMoment(3,    3,  7,  3M,  0,    0, false, 0, "W3 Tjedan 3 Dan 7 (SRI) do  3kn"),
      //};


      SetAndCheck_TH_PriceRuleList(ZXC.TH_PriceRuleList_W2_SRI_C3, TH_Cjenik_Kind._2WShop_SRI_3WCjenik_);

      #endregion TH_PriceRuleList_W2_SRI_C3

      #region TH_PriceRuleList_W5_C4

      ZXC.TH_PriceRuleList_W5_C4 = new List<TH_PriceRuleForCycleMoment>()
      {
                                      // CK Tjed Dan  Max  Rbt HHprc  Letak  ExclsvKind   
          new TH_PriceRuleForCycleMoment(4,    1,  1,   0,   0,    0,  true, 0, "W5/4 Tjedan 1 Dan 1"          ),
          new TH_PriceRuleForCycleMoment(4,    1,  2,   0,   0,    0,  true, 0, "W5/4 Tjedan 1 Dan 2"          ),
          new TH_PriceRuleForCycleMoment(4,    1,  3,   0,   0,    0,  true, 0, "W5/4 Tjedan 1 Dan 3"          ),
          new TH_PriceRuleForCycleMoment(4,    1,  4,   0,   0,    0,  true, 0, "W5/4 Tjedan 1 Dan 4"          ),
          new TH_PriceRuleForCycleMoment(4,    1,  5,   0,   0,    0,  true, 0, "W5/4 Tjedan 1 Dan 5"          ),
          new TH_PriceRuleForCycleMoment(4,    1,  6,   0, 10M,    0,  true, 0, "W5/4 Tjedan 1 Dan 6 10 posto" ),
          new TH_PriceRuleForCycleMoment(4,    1,  7,   0, 10M,    0,  true, 0, "W5/4 Tjedan 1 Dan 7 10 posto" ),
          new TH_PriceRuleForCycleMoment(4,    2,  1,   0, 30M,    0, false, 0, "W5/4 Tjedan 2 Dan 1 30 posto" ),
          new TH_PriceRuleForCycleMoment(4,    2,  2,   0, 30M,    0, false, 0, "W5/4 Tjedan 2 Dan 2 30 posto" ),
          new TH_PriceRuleForCycleMoment(4,    2,  3,   0, 30M,    0, false, 0, "W5/4 Tjedan 2 Dan 3 30 posto" ),
          new TH_PriceRuleForCycleMoment(4,    2,  4,   0, 50M,    0, false, 0, "W5/4 Tjedan 2 Dan 4 50 posto" ),
          new TH_PriceRuleForCycleMoment(4,    2,  5,   0, 50M,    0, false, 0, "W5/4 Tjedan 2 Dan 5 50 posto" ),
          new TH_PriceRuleForCycleMoment(4,    2,  6,   0, 50M,    0, false, 0, "W5/4 Tjedan 2 Dan 6 50 posto" ),
          new TH_PriceRuleForCycleMoment(4,    2,  7,   0, 50M,    0, false, 0, "W5/4 Tjedan 2 Dan 7 50 posto" ),
          new TH_PriceRuleForCycleMoment(4,    3,  1, 30M,   0,    0, false, 0, "W5/4 Tjedan 3 Dan 1 do 30kn"  ), 
          new TH_PriceRuleForCycleMoment(4,    3,  2, 30M,   0,    0, false, 0, "W5/4 Tjedan 3 Dan 2 do 30kn"  ), 
          new TH_PriceRuleForCycleMoment(4,    3,  3, 30M,   0,    0, false, 0, "W5/4 Tjedan 3 Dan 3 do 30kn"  ), 
          new TH_PriceRuleForCycleMoment(4,    3,  4, 25M,   0,    0, false, 0, "W5/4 Tjedan 3 Dan 4 do 25kn"  ), 
          new TH_PriceRuleForCycleMoment(4,    3,  5, 20M,   0,    0, false, 0, "W5/4 Tjedan 3 Dan 5 do 20kn"  ), 
          new TH_PriceRuleForCycleMoment(4,    3,  6, 20M,   0,    0, false, 0, "W5/4 Tjedan 3 Dan 6 do 20kn"  ), 
          new TH_PriceRuleForCycleMoment(4,    3,  7, 20M,   0,    0, false, 0, "W5/4 Tjedan 3 Dan 7 do 20kn"  ), 
          new TH_PriceRuleForCycleMoment(4,    4,  1, 15M,   0,    0, false, 0, "W5/4 Tjedan 4 Dan 1 do 15kn"  ), 
          new TH_PriceRuleForCycleMoment(4,    4,  2, 15M,   0,    0, false, 0, "W5/4 Tjedan 4 Dan 2 do 15kn"  ), 
          new TH_PriceRuleForCycleMoment(4,    4,  3, 10M,   0,    0, false, 0, "W5/4 Tjedan 4 Dan 3 do 10kn"  ), 
          new TH_PriceRuleForCycleMoment(4,    4,  4, 10M,   0,    0, false, 0, "W5/4 Tjedan 4 Dan 4 do 10kn"  ), 
          new TH_PriceRuleForCycleMoment(4,    4,  5,  7M,   0,    0, false, 0, "W5/4 Tjedan 4 Dan 5 do  7kn"  ), 
          new TH_PriceRuleForCycleMoment(4,    4,  6,  7M,   0,    0, false, 0, "W5/4 Tjedan 4 Dan 6 do  7kn"  ), 
          new TH_PriceRuleForCycleMoment(4,    4,  7,  7M,   0,    0, false, 0, "W5/4 Tjedan 4 Dan 7 do  7kn"  ), 
      };

      SetAndCheck_TH_PriceRuleList(ZXC.TH_PriceRuleList_W5_C4, TH_Cjenik_Kind._5WShop_4WCjenik_);

      #endregion TH_PriceRuleList_W5_C4

      #region TH_PriceRuleList_W5_C5

      // ZXC.TH_PriceRuleList_W5_C5 = new List<TH_PriceRuleForCycleMoment>()
      // {
      //                                 // CK Tjed Dan  Max  Rbt HHprc  Letak  ExclsvKind   
      //    new TH_PriceRuleForCycleMoment(5,    1,  1,   0,   0,    0,  true,     0, "W5/5 Tjedan 1 Dan 1"                     ),
      //    new TH_PriceRuleForCycleMoment(5,    1,  2,   0,   0,    0,  true,     0, "W5/5 Tjedan 1 Dan 2"                     ),
      //    new TH_PriceRuleForCycleMoment(5,    1,  3,   0,   0,    0,  true,     0, "W5/5 Tjedan 1 Dan 3"                     ),
      //    new TH_PriceRuleForCycleMoment(5,    1,  4,   0,   0,    0,  true,     0, "W5/5 Tjedan 1 Dan 4"                     ),
      //    new TH_PriceRuleForCycleMoment(5,    1,  5,   0,   0,    0,  true,     0, "W5/5 Tjedan 1 Dan 5"                     ),
      //    new TH_PriceRuleForCycleMoment(5,    1,  6,   0, 10M,    0,  true,     0, "W5/5 Tjedan 1 Dan 6 10 posto"            ),
      //    new TH_PriceRuleForCycleMoment(5,    1,  7,   0, 10M,    0,  true,     0, "W5/5 Tjedan 1 Dan 7 10 posto"            ),

      //    new TH_PriceRuleForCycleMoment(5,    2,  1,   0,   0,  20M, false,      0, "W5/5 Tjedan 2 Dan 1 happy hour 20 posto"),
      //    new TH_PriceRuleForCycleMoment(5,    2,  2,   0,   0,  20M, false,      0, "W5/5 Tjedan 2 Dan 2 happy hour 20 posto"),
      //    new TH_PriceRuleForCycleMoment(5,    2,  3,   0,   0,  20M, false,      0, "W5/5 Tjedan 2 Dan 3 happy hour 20 posto"),
      //    new TH_PriceRuleForCycleMoment(5,    2,  4,   0,   0,  20M, false,      0, "W5/5 Tjedan 2 Dan 4 happy hour 20 posto"),
      //    new TH_PriceRuleForCycleMoment(5,    2,  5,   0,   0,  20M, false,      0, "W5/5 Tjedan 2 Dan 5 happy hour 20 posto"),
      //    new TH_PriceRuleForCycleMoment(5,    2,  6,   0,   0,  20M, false,      0, "W5/5 Tjedan 2 Dan 6 happy hour 20 posto"),
      //    new TH_PriceRuleForCycleMoment(5,    2,  7,   0,   0,  20M, false,      0, "W5/5 Tjedan 2 Dan 7 happy hour 20 posto"),

      //    new TH_PriceRuleForCycleMoment(5,    3,  1,   0,   0,  30M, false,      0, "W5/5 Tjedan 3 Dan 1 happy hour 30 posto"      ),
      //    new TH_PriceRuleForCycleMoment(5,    3,  2,   0,   0,  50M, false,      0, "W5/5 Tjedan 3 Dan 2 happy hour 50 posto"      ),
      //    new TH_PriceRuleForCycleMoment(5,    3,  3,   0, 30M,    0, false,      0, "W5/5 Tjedan 3 Dan 3 30 posto"                 ),
      //    new TH_PriceRuleForCycleMoment(5,    3,  4,   0, 30M,    0, false,      0, "W5/5 Tjedan 3 Dan 4 30 posto"                 ),
      //    new TH_PriceRuleForCycleMoment(5,    3,  5,   0, 50M,    0, false,      0, "W5/5 Tjedan 3 Dan 5 50 posto"                 ),
      //    new TH_PriceRuleForCycleMoment(5,    3,  6,   0, 50M,    0, false,      0, "W5/5 Tjedan 3 Dan 6 50 posto"                 ),
      //    new TH_PriceRuleForCycleMoment(5,    3,  7,   0, 50M,    0, false,      0, "W5/5 Tjedan 3 Dan 7 50 posto"                 ),

      //    new TH_PriceRuleForCycleMoment(5,    4,  1, 30M,   0,    0, false, /*1*/0, "W5/5 Tjedan 4 Dan 1 do 30kn"/*excl 50 posto"*/), 
      //    new TH_PriceRuleForCycleMoment(5,    4,  2, 30M,   0,    0, false, /*2*/0, "W5/5 Tjedan 4 Dan 2 do 30kn"/*excl 50 posto"*/), 
      //    new TH_PriceRuleForCycleMoment(5,    4,  3, 25M,   0,    0, false, /*2*/0, "W5/5 Tjedan 4 Dan 3 do 25kn"/*excl 50 posto"*/), 
      //    new TH_PriceRuleForCycleMoment(5,    4,  4, 25M,   0,    0, false, /*2*/0, "W5/5 Tjedan 4 Dan 4 do 25kn"/*excl 50 posto"*/), 
      //    new TH_PriceRuleForCycleMoment(5,    4,  5, 20M,   0,    0, false, /*2*/0, "W5/5 Tjedan 4 Dan 5 do 20kn"/*excl 50 posto"*/), 
      //    new TH_PriceRuleForCycleMoment(5,    4,  6, 20M,   0,    0, false, /*2*/0, "W5/5 Tjedan 4 Dan 6 do 20kn"/*excl 50 posto"*/), 
      //    new TH_PriceRuleForCycleMoment(5,    4,  7, 20M,   0,    0, false, /*2*/0, "W5/5 Tjedan 4 Dan 7 do 20kn"/*excl 50 posto"*/),

      //    new TH_PriceRuleForCycleMoment(5,    5,  1, 15M,   0,    0, false, /*2*/0, "W5/5 Tjedan 5 Dan 1 do 15kn"/*excl 50 posto"*/), 
      //    new TH_PriceRuleForCycleMoment(5,    5,  2, 15M,   0,    0, false, /*2*/0, "W5/5 Tjedan 5 Dan 2 do 15kn"/*excl 50 posto"*/), 
      //    new TH_PriceRuleForCycleMoment(5,    5,  3, 10M,   0,    0, false,      0, "W5/5 Tjedan 5 Dan 3 do 10kn"                  ),
      //    new TH_PriceRuleForCycleMoment(5,    5,  4, 10M,   0,    0, false,      0, "W5/5 Tjedan 5 Dan 4 do 10kn"                  ), 
      //    new TH_PriceRuleForCycleMoment(5,    5,  5,  7M,   0,    0, false,      0, "W5/5 Tjedan 5 Dan 5 do  7kn"                  ), 
      //    new TH_PriceRuleForCycleMoment(5,    5,  6,  7M,   0,    0, false,      0, "W5/5 Tjedan 5 Dan 6 do  7kn"                  ), 
      //    new TH_PriceRuleForCycleMoment(5,    5,  7,  7M,   0,    0, false,      0, "W5/5 Tjedan 5 Dan 7 do  7kn"                  ),
      //};

      //23.12.2022.new 2023 EUR TH_PriceRuleList_W5_C5
      ZXC.TH_PriceRuleList_W5_C5 = new List<TH_PriceRuleForCycleMoment>()
      {
                                      // CK Tjed Dan   Max  Rbt HHprc  Letak  ExclsvKind   
         new TH_PriceRuleForCycleMoment(5,    1,  1,    0,   0,    0,  true,     0, "W5/5 Tjedan 1 Dan 1"                     ),
         new TH_PriceRuleForCycleMoment(5,    1,  2,    0,   0,    0,  true,     0, "W5/5 Tjedan 1 Dan 2"                     ),
         new TH_PriceRuleForCycleMoment(5,    1,  3,    0,   0,    0,  true,     0, "W5/5 Tjedan 1 Dan 3"                     ),
         new TH_PriceRuleForCycleMoment(5,    1,  4,    0,   0,    0,  true,     0, "W5/5 Tjedan 1 Dan 4"                     ),
         new TH_PriceRuleForCycleMoment(5,    1,  5,    0,   0,    0,  true,     0, "W5/5 Tjedan 1 Dan 5"                     ),
         new TH_PriceRuleForCycleMoment(5,    1,  6,    0, 10M,    0,  true,     0, "W5/5 Tjedan 1 Dan 6 10 posto"            ),
         new TH_PriceRuleForCycleMoment(5,    1,  7,    0, 10M,    0,  true,     0, "W5/5 Tjedan 1 Dan 7 10 posto"            ),

         new TH_PriceRuleForCycleMoment(5,    2,  1,    0,   0,  20M, false,      0, "W5/5 Tjedan 2 Dan 1 happy hour 20 posto"),
         new TH_PriceRuleForCycleMoment(5,    2,  2,    0,   0,  20M, false,      0, "W5/5 Tjedan 2 Dan 2 happy hour 20 posto"),
         new TH_PriceRuleForCycleMoment(5,    2,  3,    0,   0,  20M, false,      0, "W5/5 Tjedan 2 Dan 3 happy hour 20 posto"),
         new TH_PriceRuleForCycleMoment(5,    2,  4,    0,   0,  20M, false,      0, "W5/5 Tjedan 2 Dan 4 happy hour 20 posto"),
         new TH_PriceRuleForCycleMoment(5,    2,  5,    0,   0,  20M, false,      0, "W5/5 Tjedan 2 Dan 5 happy hour 20 posto"),
         new TH_PriceRuleForCycleMoment(5,    2,  6,    0,   0,  20M, false,      0, "W5/5 Tjedan 2 Dan 6 happy hour 20 posto"),
         new TH_PriceRuleForCycleMoment(5,    2,  7,    0,   0,  20M, false,      0, "W5/5 Tjedan 2 Dan 7 happy hour 20 posto"),

         new TH_PriceRuleForCycleMoment(5,    3,  1,    0,   0,  30M, false,      0, "W5/5 Tjedan 3 Dan 1 happy hour 30 posto"),
         new TH_PriceRuleForCycleMoment(5,    3,  2,    0,   0,  50M, false,      0, "W5/5 Tjedan 3 Dan 2 happy hour 50 posto"),
         new TH_PriceRuleForCycleMoment(5,    3,  3,    0, 30M,    0, false,      0, "W5/5 Tjedan 3 Dan 3 30 posto"           ),
         new TH_PriceRuleForCycleMoment(5,    3,  4,    0, 30M,    0, false,      0, "W5/5 Tjedan 3 Dan 4 30 posto"           ),
         new TH_PriceRuleForCycleMoment(5,    3,  5,    0, 50M,    0, false,      0, "W5/5 Tjedan 3 Dan 5 50 posto"           ),
         new TH_PriceRuleForCycleMoment(5,    3,  6,    0, 50M,    0, false,      0, "W5/5 Tjedan 3 Dan 6 50 posto"           ),
         new TH_PriceRuleForCycleMoment(5,    3,  7,    0, 50M,    0, false,      0, "W5/5 Tjedan 3 Dan 7 50 posto"           ),

         new TH_PriceRuleForCycleMoment(5,    4,  1,   4M,   0,    0, false,      0, "W5/5 Tjedan 4 Dan 1 do   4 EUR"         ),
         new TH_PriceRuleForCycleMoment(5,    4,  2,   4M,   0,    0, false,      0, "W5/5 Tjedan 4 Dan 2 do   4 EUR"         ),
         new TH_PriceRuleForCycleMoment(5,    4,  3, 3.5M,   0,    0, false,      0, "W5/5 Tjedan 4 Dan 3 do 3,5 EUR"         ),
         new TH_PriceRuleForCycleMoment(5,    4,  4, 3.5M,   0,    0, false,      0, "W5/5 Tjedan 4 Dan 4 do 3,5 EUR"         ),
         new TH_PriceRuleForCycleMoment(5,    4,  5,   3M,   0,    0, false,      0, "W5/5 Tjedan 4 Dan 5 do   3 EUR"         ),
         new TH_PriceRuleForCycleMoment(5,    4,  6,   3M,   0,    0, false,      0, "W5/5 Tjedan 4 Dan 6 do   3 EUR"         ),
         new TH_PriceRuleForCycleMoment(5,    4,  7,   3M,   0,    0, false,      0, "W5/5 Tjedan 4 Dan 7 do   3 EUR"         ),

         new TH_PriceRuleForCycleMoment(5,    5,  1, 2.5M,   0,    0, false,      0, "W5/5 Tjedan 5 Dan 1 do 2,5 EUR"         ),
         new TH_PriceRuleForCycleMoment(5,    5,  2,   2M,   0,    0, false,      0, "W5/5 Tjedan 5 Dan 2 do   2 EUR"         ),
         new TH_PriceRuleForCycleMoment(5,    5,  3,   2M,   0,    0, false,      0, "W5/5 Tjedan 5 Dan 3 do   2 EUR"         ),
         new TH_PriceRuleForCycleMoment(5,    5,  4, 1.5M,   0,    0, false,      0, "W5/5 Tjedan 5 Dan 4 do 1,5 EUR"         ),
         new TH_PriceRuleForCycleMoment(5,    5,  5,   1M,   0,    0, false,      0, "W5/5 Tjedan 5 Dan 5 do   1 EUR"         ),
         new TH_PriceRuleForCycleMoment(5,    5,  6, 0.5M,   0,    0, false,      0, "W5/5 Tjedan 5 Dan 6 do 0,5 EUR"         ),
         new TH_PriceRuleForCycleMoment(5,    5,  7, 0.5M,   0,    0, false,      0, "W5/5 Tjedan 5 Dan 7 do 0,5 EUR"         ),
     };


      SetAndCheck_TH_PriceRuleList(ZXC.TH_PriceRuleList_W5_C5, TH_Cjenik_Kind._5WShop_5WCjenik_);

      #endregion TH_PriceRuleList_W5_C5

      #region TH_PriceRuleList_W5_C6

#if Olddd
      ZXC.TH_PriceRuleList_W5_C6 = new List<TH_PriceRuleForCycleMoment>()
      {
                                      // W Tjed Dan  Max  Rbt HHprc  Letak  ExclsvKind   
          new TH_PriceRuleForCycleMoment(6,   1,  1,   0,   0,    0, false, 0, "W6 Tjedan 1 Dan 1"                               ),
          new TH_PriceRuleForCycleMoment(6,   1,  2,   0,   0,    0, false, 0, "W6 Tjedan 1 Dan 2"                               ),
          new TH_PriceRuleForCycleMoment(6,   1,  3,   0,   0,    0, false, 0, "W6 Tjedan 1 Dan 3"                               ),
          new TH_PriceRuleForCycleMoment(6,   1,  4,   0,   0,    0, false, 0, "W6 Tjedan 1 Dan 4"                               ),
          new TH_PriceRuleForCycleMoment(6,   1,  5,   0,   0,    0, false, 0, "W6 Tjedan 1 Dan 5"                               ),
          new TH_PriceRuleForCycleMoment(6,   1,  6,   0, 10M,    0, false, 0, "W6 Tjedan 1 Dan 6 10 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   1,  7,   0, 10M,    0, false, 0, "W6 Tjedan 1 Dan 7 10 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   2,  1,   0, 20M,    0, false, 0, "W6 Tjedan 2 Dan 1 20 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   2,  2,   0, 20M,    0, false, 0, "W6 Tjedan 2 Dan 2 20 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   2,  3,   0, 20M,    0, false, 0, "W6 Tjedan 2 Dan 3 20 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   2,  4,   0, 30M,    0, false, 0, "W6 Tjedan 2 Dan 4 30 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   2,  5,   0, 20M,    0, false, 0, "W6 Tjedan 2 Dan 5 20 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   2,  6,   0, 20M,    0, false, 0, "W6 Tjedan 2 Dan 6 20 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   2,  7,   0, 20M,    0, false, 0, "W6 Tjedan 2 Dan 7 20 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   3,  1,   0, 20M,  30M, false, 0, "W6 Tjedan 3 Dan 1 20 posto i happy hour 30 posto"),
          new TH_PriceRuleForCycleMoment(6,   3,  2,   0, 20M,  30M, false, 0, "W6 Tjedan 3 Dan 2 20 posto i happy hour 30 posto"),
          new TH_PriceRuleForCycleMoment(6,   3,  3,   0, 20M,  30M, false, 0, "W6 Tjedan 3 Dan 3 20 posto i happy hour 30 posto"),
          new TH_PriceRuleForCycleMoment(6,   3,  4,   0, 30M,    0, false, 0, "W6 Tjedan 3 Dan 4 30 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   3,  5,   0, 30M,    0, false, 0, "W6 Tjedan 3 Dan 5 30 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   3,  6,   0, 30M,    0, false, 0, "W6 Tjedan 3 Dan 6 30 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   3,  7,   0, 30M,    0, false, 0, "W6 Tjedan 3 Dan 7 30 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   4,  1,   0, 30M,  50M, false, 0, "W6 Tjedan 4 Dan 1 30 posto i happy hour 50 posto"),
          new TH_PriceRuleForCycleMoment(6,   4,  2,   0, 30M,  50M, false, 0, "W6 Tjedan 4 Dan 2 30 posto i happy hour 50 posto"),
          new TH_PriceRuleForCycleMoment(6,   4,  3,   0, 30M,  50M, false, 0, "W6 Tjedan 4 Dan 3 30 posto i happy hour 50 posto"),
          new TH_PriceRuleForCycleMoment(6,   4,  4,   0, 50M,    0, false, 0, "W6 Tjedan 4 Dan 4 50 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   4,  5,   0, 50M,    0, false, 0, "W6 Tjedan 4 Dan 5 50 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   4,  6,   0, 50M,    0, false, 0, "W6 Tjedan 4 Dan 6 50 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   4,  7,   0, 50M,    0, false, 0, "W6 Tjedan 4 Dan 7 50 posto"                      ),
          new TH_PriceRuleForCycleMoment(6,   5,  1, 30M,   0,    0, false, 0, "W6 Tjedan 5 Dan 1 do 30kn"                       ), 
          new TH_PriceRuleForCycleMoment(6,   5,  2, 30M,   0,    0, false, 0, "W6 Tjedan 5 Dan 2 do 30kn"                       ), 
          new TH_PriceRuleForCycleMoment(6,   5,  3, 25M,   0,    0, false, 0, "W6 Tjedan 5 Dan 3 do 25kn"                       ), 
          new TH_PriceRuleForCycleMoment(6,   5,  4, 25M,   0,    0, false, 0, "W6 Tjedan 5 Dan 4 do 25kn"                       ), 
          new TH_PriceRuleForCycleMoment(6,   5,  5, 20M,   0,    0, false, 0, "W6 Tjedan 5 Dan 5 do 20kn"                       ), 
          new TH_PriceRuleForCycleMoment(6,   5,  6, 20M,   0,    0, false, 0, "W6 Tjedan 5 Dan 6 do 20kn"                       ), 
          new TH_PriceRuleForCycleMoment(6,   5,  7, 20M,   0,    0, false, 0, "W6 Tjedan 5 Dan 7 do 20kn"                       ), 
          new TH_PriceRuleForCycleMoment(6,   6,  1, 15M,   0,    0, false, 0, "W6 Tjedan 6 Dan 1 do 15kn"                       ), 
          new TH_PriceRuleForCycleMoment(6,   6,  2, 15M,   0,    0, false, 0, "W6 Tjedan 6 Dan 2 do 15kn"                       ), 
          new TH_PriceRuleForCycleMoment(6,   6,  3, 10M,   0,    0, false, 0, "W6 Tjedan 6 Dan 3 do 10kn"                       ), 
          new TH_PriceRuleForCycleMoment(6,   6,  4,  7M,   0,    0, false, 0, "W6 Tjedan 6 Dan 4 do  7kn"                       ), 
          new TH_PriceRuleForCycleMoment(6,   6,  5,  5M,   0,    0, false, 0, "W6 Tjedan 6 Dan 5 do  5kn"                       ), 
          new TH_PriceRuleForCycleMoment(6,   6,  6,  3M,   0,    0, false, 0, "W6 Tjedan 6 Dan 6 do  3kn"                       ), 
          new TH_PriceRuleForCycleMoment(6,   6,  7,  3M,   0,    0, false, 0, "W6 Tjedan 6 Dan 7 do  3kn"                       ), 
      };

#endif

      //ZXC.TH_PriceRuleList_W5_C6 = new List<TH_PriceRuleForCycleMoment>()
      //{
      //                                // CK Tjed Dan  Max  Rbt HHprc  Letak  ExclsvKind   
      //   new TH_PriceRuleForCycleMoment(6,    1,  1,   0,   0,    0, false, 0, "W5/6 Tjedan 1 Dan 1"         ),
      //   new TH_PriceRuleForCycleMoment(6,    1,  2,   0,   0,    0, false, 0, "W5/6 Tjedan 1 Dan 2"         ),
      //   new TH_PriceRuleForCycleMoment(6,    1,  3,   0,   0,    0, false, 0, "W5/6 Tjedan 1 Dan 3"         ),
      //   new TH_PriceRuleForCycleMoment(6,    1,  4,   0,   0,    0, false, 0, "W5/6 Tjedan 1 Dan 4"         ),
      //   new TH_PriceRuleForCycleMoment(6,    1,  5,   0,   0,    0, false, 0, "W5/6 Tjedan 1 Dan 5"         ),
      //   new TH_PriceRuleForCycleMoment(6,    1,  6,   0, 10M,    0, false, 0, "W5/6 Tjedan 1 Dan 6 10 posto"),
      //   new TH_PriceRuleForCycleMoment(6,    1,  7,   0, 10M,    0, false, 0, "W5/6 Tjedan 1 Dan 7 10 posto"),

      //   new TH_PriceRuleForCycleMoment(6,    2,  1,   0,   0,  20M, false, 0, "W5/6 Tjedan 2 Dan 1 happy hour 20 posto"),
      //   new TH_PriceRuleForCycleMoment(6,    2,  2,   0,   0,  20M, false, 0, "W5/6 Tjedan 2 Dan 2 happy hour 20 posto"),
      //   new TH_PriceRuleForCycleMoment(6,    2,  3,   0,   0,  20M, false, 0, "W5/6 Tjedan 2 Dan 3 happy hour 20 posto"),
      //   new TH_PriceRuleForCycleMoment(6,    2,  4,   0,   0,  20M, false, 0, "W5/6 Tjedan 2 Dan 4 happy hour 20 posto"),
      //   new TH_PriceRuleForCycleMoment(6,    2,  5,   0,   0,  20M, false, 0, "W5/6 Tjedan 2 Dan 5 happy hour 20 posto"),
      //   new TH_PriceRuleForCycleMoment(6,    2,  6,   0,   0,  20M, false, 0, "W5/6 Tjedan 2 Dan 6 happy hour 20 posto"),
      //   new TH_PriceRuleForCycleMoment(6,    2,  7,   0,   0,  20M, false, 0, "W5/6 Tjedan 2 Dan 7 happy hour 20 posto"),

      //   new TH_PriceRuleForCycleMoment(6,    3,  1,   0, 30M,    0, false, 0, "W5/6 Tjedan 3 Dan 1 30 posto"),
      //   new TH_PriceRuleForCycleMoment(6,    3,  2,   0, 30M,    0, false, 0, "W5/6 Tjedan 3 Dan 2 30 posto"),
      //   new TH_PriceRuleForCycleMoment(6,    3,  3,   0, 30M,    0, false, 0, "W5/6 Tjedan 3 Dan 3 30 posto"),
      //   new TH_PriceRuleForCycleMoment(6,    3,  4,   0, 30M,    0, false, 0, "W5/6 Tjedan 3 Dan 4 30 posto"),
      //   new TH_PriceRuleForCycleMoment(6,    3,  5,   0, 30M,    0, false, 0, "W5/6 Tjedan 3 Dan 5 30 posto"),
      //   new TH_PriceRuleForCycleMoment(6,    3,  6,   0, 30M,    0, false, 0, "W5/6 Tjedan 3 Dan 6 30 posto"),
      //   new TH_PriceRuleForCycleMoment(6,    3,  7,   0, 30M,    0, false, 0, "W5/6 Tjedan 3 Dan 7 30 posto"),

      //   new TH_PriceRuleForCycleMoment(6,    4,  1,   0, 50M,    0, false, 0, "W5/6 Tjedan 4 Dan 1 50 posto"),
      //   new TH_PriceRuleForCycleMoment(6,    4,  2,   0, 50M,    0, false, 0, "W5/6 Tjedan 4 Dan 2 50 posto"),
      //   new TH_PriceRuleForCycleMoment(6,    4,  3,   0, 50M,    0, false, 0, "W5/6 Tjedan 4 Dan 3 50 posto" ),
      //   new TH_PriceRuleForCycleMoment(6,    4,  4,   0, 50M,    0, false, 0, "W5/6 Tjedan 4 Dan 4 50 posto" ),
      //   new TH_PriceRuleForCycleMoment(6,    4,  5,   0, 50M,    0, false, 0, "W5/6 Tjedan 4 Dan 5 50 posto" ),
      //   new TH_PriceRuleForCycleMoment(6,    4,  6,   0, 50M,    0, false, 0, "W5/6 Tjedan 4 Dan 6 50 posto" ),
      //   new TH_PriceRuleForCycleMoment(6,    4,  7,   0, 50M,    0, false, 0, "W5/6 Tjedan 4 Dan 7 50 posto" ),

      //   new TH_PriceRuleForCycleMoment(6,    5,  1, 30M,   0,    0, false, 0, "W5/6 Tjedan 5 Dan 1 do 30kn" ),
      //   new TH_PriceRuleForCycleMoment(6,    5,  2, 30M,   0,    0, false, 0, "W5/6 Tjedan 5 Dan 2 do 30kn" ),
      //   new TH_PriceRuleForCycleMoment(6,    5,  3, 25M,   0,    0, false, 0, "W5/6 Tjedan 5 Dan 3 do 25kn" ),
      //   new TH_PriceRuleForCycleMoment(6,    5,  4, 25M,   0,    0, false, 0, "W5/6 Tjedan 5 Dan 4 do 25kn" ),
      //   new TH_PriceRuleForCycleMoment(6,    5,  5, 20M,   0,    0, false, 0, "W5/6 Tjedan 5 Dan 5 do 20kn" ),
      //   new TH_PriceRuleForCycleMoment(6,    5,  6, 20M,   0,    0, false, 0, "W5/6 Tjedan 5 Dan 6 do 20kn" ),
      //   new TH_PriceRuleForCycleMoment(6,    5,  7, 20M,   0,    0, false, 0, "W5/6 Tjedan 5 Dan 7 do 20kn" ),

      //   new TH_PriceRuleForCycleMoment(6,    6,  1, 15M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 1 do 15kn" ),
      //   new TH_PriceRuleForCycleMoment(6,    6,  2, 15M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 2 do 15kn" ),
      //   new TH_PriceRuleForCycleMoment(6,    6,  3, 10M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 3 do 10kn" ),
      //   new TH_PriceRuleForCycleMoment(6,    6,  4, 10M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 4 do 10kn" ),
      //   new TH_PriceRuleForCycleMoment(6,    6,  5,  7M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 5 do  7kn" ),
      //   new TH_PriceRuleForCycleMoment(6,    6,  6,  7M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 6 do  7kn" ),
      //   new TH_PriceRuleForCycleMoment(6,    6,  7,  7M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 7 do  7kn" ),
      //};

      ZXC.TH_PriceRuleList_W5_C6 = new List<TH_PriceRuleForCycleMoment>()
      {
                                      // CK Tjed Dan  Max  Rbt HHprc  Letak  ExclsvKind   
         new TH_PriceRuleForCycleMoment(6,    1,  1,    0,   0,    0, false, 0, "W5/6 Tjedan 1 Dan 1"         ),
         new TH_PriceRuleForCycleMoment(6,    1,  2,    0,   0,    0, false, 0, "W5/6 Tjedan 1 Dan 2"         ),
         new TH_PriceRuleForCycleMoment(6,    1,  3,    0,   0,    0, false, 0, "W5/6 Tjedan 1 Dan 3"         ),
         new TH_PriceRuleForCycleMoment(6,    1,  4,    0,   0,    0, false, 0, "W5/6 Tjedan 1 Dan 4"         ),
         new TH_PriceRuleForCycleMoment(6,    1,  5,    0,   0,    0, false, 0, "W5/6 Tjedan 1 Dan 5"         ),
         new TH_PriceRuleForCycleMoment(6,    1,  6,    0, 10M,    0, false, 0, "W5/6 Tjedan 1 Dan 6 10 posto"),
         new TH_PriceRuleForCycleMoment(6,    1,  7,    0, 10M,    0, false, 0, "W5/6 Tjedan 1 Dan 7 10 posto"),

         new TH_PriceRuleForCycleMoment(6,    2,  1,    0,   0,  20M, false, 0, "W5/6 Tjedan 2 Dan 1 happy hour 20 posto"),
         new TH_PriceRuleForCycleMoment(6,    2,  2,    0,   0,  20M, false, 0, "W5/6 Tjedan 2 Dan 2 happy hour 20 posto"),
         new TH_PriceRuleForCycleMoment(6,    2,  3,    0,   0,  20M, false, 0, "W5/6 Tjedan 2 Dan 3 happy hour 20 posto"),
         new TH_PriceRuleForCycleMoment(6,    2,  4,    0,   0,  20M, false, 0, "W5/6 Tjedan 2 Dan 4 happy hour 20 posto"),
         new TH_PriceRuleForCycleMoment(6,    2,  5,    0,   0,  20M, false, 0, "W5/6 Tjedan 2 Dan 5 happy hour 20 posto"),
         new TH_PriceRuleForCycleMoment(6,    2,  6,    0,   0,  20M, false, 0, "W5/6 Tjedan 2 Dan 6 happy hour 20 posto"),
         new TH_PriceRuleForCycleMoment(6,    2,  7,    0,   0,  20M, false, 0, "W5/6 Tjedan 2 Dan 7 happy hour 20 posto"),

         new TH_PriceRuleForCycleMoment(6,    3,  1,    0, 30M,    0, false, 0, "W5/6 Tjedan 3 Dan 1 30 posto"),
         new TH_PriceRuleForCycleMoment(6,    3,  2,    0, 30M,    0, false, 0, "W5/6 Tjedan 3 Dan 2 30 posto"),
         new TH_PriceRuleForCycleMoment(6,    3,  3,    0, 30M,    0, false, 0, "W5/6 Tjedan 3 Dan 3 30 posto"),
         new TH_PriceRuleForCycleMoment(6,    3,  4,    0, 30M,    0, false, 0, "W5/6 Tjedan 3 Dan 4 30 posto"),
         new TH_PriceRuleForCycleMoment(6,    3,  5,    0, 30M,    0, false, 0, "W5/6 Tjedan 3 Dan 5 30 posto"),
         new TH_PriceRuleForCycleMoment(6,    3,  6,    0, 30M,    0, false, 0, "W5/6 Tjedan 3 Dan 6 30 posto"),
         new TH_PriceRuleForCycleMoment(6,    3,  7,    0, 30M,    0, false, 0, "W5/6 Tjedan 3 Dan 7 30 posto"),

         new TH_PriceRuleForCycleMoment(6,    4,  1,    0, 50M,    0, false, 0, "W5/6 Tjedan 4 Dan 1 50 posto"),
         new TH_PriceRuleForCycleMoment(6,    4,  2,    0, 50M,    0, false, 0, "W5/6 Tjedan 4 Dan 2 50 posto"),
         new TH_PriceRuleForCycleMoment(6,    4,  3,    0, 50M,    0, false, 0, "W5/6 Tjedan 4 Dan 3 50 posto" ),
         new TH_PriceRuleForCycleMoment(6,    4,  4,    0, 50M,    0, false, 0, "W5/6 Tjedan 4 Dan 4 50 posto" ),
         new TH_PriceRuleForCycleMoment(6,    4,  5,    0, 50M,    0, false, 0, "W5/6 Tjedan 4 Dan 5 50 posto" ),
         new TH_PriceRuleForCycleMoment(6,    4,  6,    0, 50M,    0, false, 0, "W5/6 Tjedan 4 Dan 6 50 posto" ),
         new TH_PriceRuleForCycleMoment(6,    4,  7,    0, 50M,    0, false, 0, "W5/6 Tjedan 4 Dan 7 50 posto" ),

         new TH_PriceRuleForCycleMoment(6,    5,  1,   4M,   0,    0, false, 0, "W5/6 Tjedan 5 Dan 1 do   4 EUR"),
         new TH_PriceRuleForCycleMoment(6,    5,  2, 3.5M,   0,    0, false, 0, "W5/6 Tjedan 5 Dan 2 do 3,5 EUR"),
         new TH_PriceRuleForCycleMoment(6,    5,  3,   3M,   0,    0, false, 0, "W5/6 Tjedan 5 Dan 3 do   3 EUR"),
         new TH_PriceRuleForCycleMoment(6,    5,  4,   3M,   0,    0, false, 0, "W5/6 Tjedan 5 Dan 4 do   3 EUR"),
         new TH_PriceRuleForCycleMoment(6,    5,  5, 2.5M,   0,    0, false, 0, "W5/6 Tjedan 5 Dan 5 do 2,5 EUR"),
         new TH_PriceRuleForCycleMoment(6,    5,  6, 2.5M,   0,    0, false, 0, "W5/6 Tjedan 5 Dan 6 do 2,5 EUR"),
         new TH_PriceRuleForCycleMoment(6,    5,  7, 2.5M,   0,    0, false, 0, "W5/6 Tjedan 5 Dan 7 do 2,5 EUR"),

         new TH_PriceRuleForCycleMoment(6,    6,  1,   2M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 1 do   2 EUR"),
         new TH_PriceRuleForCycleMoment(6,    6,  2,   2M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 2 do   2 EUR"),
         new TH_PriceRuleForCycleMoment(6,    6,  3,   2M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 3 do   2 EUR"),
         new TH_PriceRuleForCycleMoment(6,    6,  4, 1.5M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 4 do 1,5 EUR"),
         new TH_PriceRuleForCycleMoment(6,    6,  5,   1M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 5 do   1 EUR"),
         new TH_PriceRuleForCycleMoment(6,    6,  6, 0.5M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 6 do 0,5 EUR"),
         new TH_PriceRuleForCycleMoment(6,    6,  7, 0.5M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 7 do 0,5 EUR"),
      };

      SetAndCheck_TH_PriceRuleList(ZXC.TH_PriceRuleList_W5_C6, TH_Cjenik_Kind._5WShop_6WCjenik_);

      #endregion TH_PriceRuleList_W5_C6

      #region TH_PriceRuleList_W5_C7

      ZXC.TH_PriceRuleList_W5_C7 = new List<TH_PriceRuleForCycleMoment>()
      {
                                      // CK Tjed Dan  Max  Rbt HHprc  Letak  ExclsvKind   
         new TH_PriceRuleForCycleMoment(7,    1,  1,   0,   0,    0, false, 0, "W5/7 Tjedan 1 Dan 1"         ),
         new TH_PriceRuleForCycleMoment(7,    1,  2,   0,   0,    0, false, 0, "W5/7 Tjedan 1 Dan 2"         ),
         new TH_PriceRuleForCycleMoment(7,    1,  3,   0,   0,    0, false, 0, "W5/7 Tjedan 1 Dan 3"         ),
         new TH_PriceRuleForCycleMoment(7,    1,  4,   0,   0,    0, false, 0, "W5/7 Tjedan 1 Dan 4"         ),
         new TH_PriceRuleForCycleMoment(7,    1,  5,   0,   0,    0, false, 0, "W5/7 Tjedan 1 Dan 5"         ),
         new TH_PriceRuleForCycleMoment(7,    1,  6,   0, 10M,    0, false, 0, "W5/7 Tjedan 1 Dan 6 10 posto"),
         new TH_PriceRuleForCycleMoment(7,    1,  7,   0, 10M,    0, false, 0, "W5/7 Tjedan 1 Dan 7 10 posto"),
         new TH_PriceRuleForCycleMoment(7,    2,  1,   0, 20M,    0, false, 0, "W5/7 Tjedan 2 Dan 1 20 posto"),
         new TH_PriceRuleForCycleMoment(7,    2,  2,   0, 20M,    0, false, 0, "W5/7 Tjedan 2 Dan 2 20 posto"),
         new TH_PriceRuleForCycleMoment(7,    2,  3,   0, 20M,    0, false, 0, "W5/7 Tjedan 2 Dan 3 20 posto"),
         new TH_PriceRuleForCycleMoment(7,    2,  4,   0, 20M,    0, false, 0, "W5/7 Tjedan 2 Dan 4 20 posto"),
         new TH_PriceRuleForCycleMoment(7,    2,  5,   0, 20M,    0, false, 0, "W5/7 Tjedan 2 Dan 5 20 posto"),
         new TH_PriceRuleForCycleMoment(7,    2,  6,   0, 20M,    0, false, 0, "W5/7 Tjedan 2 Dan 6 20 posto"),
         new TH_PriceRuleForCycleMoment(7,    2,  7,   0, 20M,    0, false, 0, "W5/7 Tjedan 2 Dan 7 20 posto"),
         new TH_PriceRuleForCycleMoment(7,    3,  1,   0, 30M,    0, false, 0, "W5/7 Tjedan 3 Dan 1 30 posto"),
         new TH_PriceRuleForCycleMoment(7,    3,  2,   0, 30M,    0, false, 0, "W5/7 Tjedan 3 Dan 2 30 posto"),
         new TH_PriceRuleForCycleMoment(7,    3,  3,   0, 30M,    0, false, 0, "W5/7 Tjedan 3 Dan 3 30 posto"),
         new TH_PriceRuleForCycleMoment(7,    3,  4,   0, 30M,    0, false, 0, "W5/7 Tjedan 3 Dan 4 30 posto"),
         new TH_PriceRuleForCycleMoment(7,    3,  5,   0, 30M,    0, false, 0, "W5/7 Tjedan 3 Dan 5 30 posto"),
         new TH_PriceRuleForCycleMoment(7,    3,  6,   0, 30M,    0, false, 0, "W5/7 Tjedan 3 Dan 6 30 posto"),
         new TH_PriceRuleForCycleMoment(7,    3,  7,   0, 30M,    0, false, 0, "W5/7 Tjedan 3 Dan 7 30 posto"),
         new TH_PriceRuleForCycleMoment(7,    4,  1,   0, 30M,    0, false, 0, "W5/7 Tjedan 4 Dan 1 30 posto"),
         new TH_PriceRuleForCycleMoment(7,    4,  2,   0, 30M,    0, false, 0, "W5/7 Tjedan 4 Dan 2 30 posto"),
         new TH_PriceRuleForCycleMoment(7,    4,  3,   0, 30M,    0, false, 0, "W5/7 Tjedan 4 Dan 3 30 posto"),
         new TH_PriceRuleForCycleMoment(7,    4,  4,   0, 40M,    0, false, 0, "W5/7 Tjedan 4 Dan 4 40 posto"),
         new TH_PriceRuleForCycleMoment(7,    4,  5,   0, 40M,    0, false, 0, "W5/7 Tjedan 4 Dan 5 40 posto"),
         new TH_PriceRuleForCycleMoment(7,    4,  6,   0, 40M,    0, false, 0, "W5/7 Tjedan 4 Dan 6 40 posto"),
         new TH_PriceRuleForCycleMoment(7,    4,  7,   0, 40M,    0, false, 0, "W5/7 Tjedan 4 Dan 7 40 posto"),
         new TH_PriceRuleForCycleMoment(6,    5,  1,   0, 50M,    0, false, 0, "W5/6 Tjedan 5 Dan 1 50 posto"),
         new TH_PriceRuleForCycleMoment(6,    5,  2,   0, 50M,    0, false, 0, "W5/6 Tjedan 5 Dan 2 50 posto"),
         new TH_PriceRuleForCycleMoment(6,    5,  3,   0, 50M,    0, false, 0, "W5/6 Tjedan 5 Dan 3 50 posto"),
         new TH_PriceRuleForCycleMoment(6,    5,  4,   0, 50M,    0, false, 0, "W5/6 Tjedan 5 Dan 4 50 posto"),
         new TH_PriceRuleForCycleMoment(6,    5,  5,   0, 50M,    0, false, 0, "W5/6 Tjedan 5 Dan 5 50 posto"),
         new TH_PriceRuleForCycleMoment(6,    5,  6,   0, 50M,    0, false, 0, "W5/6 Tjedan 5 Dan 6 50 posto"),
         new TH_PriceRuleForCycleMoment(6,    5,  7,   0, 50M,    0, false, 0, "W5/6 Tjedan 5 Dan 7 50 posto"),
         new TH_PriceRuleForCycleMoment(6,    6,  1, 30M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 1 do 30kn" ),
         new TH_PriceRuleForCycleMoment(6,    6,  2, 30M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 2 do 30kn" ),
         new TH_PriceRuleForCycleMoment(6,    6,  3, 25M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 3 do 25kn" ),
         new TH_PriceRuleForCycleMoment(6,    6,  4, 25M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 4 do 25kn" ),
         new TH_PriceRuleForCycleMoment(6,    6,  5, 20M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 5 do 20kn" ),
         new TH_PriceRuleForCycleMoment(6,    6,  6, 20M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 6 do 20kn" ),
         new TH_PriceRuleForCycleMoment(6,    6,  7, 20M,   0,    0, false, 0, "W5/6 Tjedan 6 Dan 7 do 20kn" ),
         new TH_PriceRuleForCycleMoment(6,    7,  1, 20M,   0,    0, false, 0, "W5/6 Tjedan 7 Dan 1 do 20kn" ),
         new TH_PriceRuleForCycleMoment(6,    7,  2, 15M,   0,    0, false, 0, "W5/6 Tjedan 7 Dan 2 do 15kn" ),
         new TH_PriceRuleForCycleMoment(6,    7,  3, 15M,   0,    0, false, 0, "W5/6 Tjedan 7 Dan 3 do 15kn" ),
         new TH_PriceRuleForCycleMoment(6,    7,  4, 10M,   0,    0, false, 0, "W5/6 Tjedan 7 Dan 4 do 10kn" ),
         new TH_PriceRuleForCycleMoment(6,    7,  5,  7M,   0,    0, false, 0, "W5/6 Tjedan 7 Dan 5 do  7kn" ),
         new TH_PriceRuleForCycleMoment(6,    7,  6,  5M,   0,    0, false, 0, "W5/6 Tjedan 7 Dan 6 do  5kn" ),
         new TH_PriceRuleForCycleMoment(6,    7,  7,  5M,   0,    0, false, 0, "W5/6 Tjedan 7 Dan 7 do  5kn" ),
      };

      SetAndCheck_TH_PriceRuleList(ZXC.TH_PriceRuleList_W5_C7, TH_Cjenik_Kind._5WShop_7WCjenik_);

      #endregion TH_PriceRuleList_W5_C7

      #region TH_PriceRuleList_W3_SRI_C3


      //ZXC.TH_PriceRuleList_W3_SRI_C3 = new List<TH_PriceRuleForCycleMoment>()
      //{
      //                               // CK Tjed Dan  Max Rbt  HHprc  Letak  ExclsvKind   
      //   new TH_PriceRuleForCycleMoment(3,    1,  1,   0,  0,     0, false, 0, "W3/3 Tjedan 1 Dan 1 (SRI) regular" ),
      //   new TH_PriceRuleForCycleMoment(3,    1,  2,   0,  0,     0, false, 0, "W3/3 Tjedan 1 Dan 2 (ČET) regular" ),
      //   new TH_PriceRuleForCycleMoment(3,    1,  3,   0,10M,     0, false, 0, "W3/3 Tjedan 1 Dan 3 (PET) 10 posto"),
      //   new TH_PriceRuleForCycleMoment(3,    1,  4,   0,10M,     0, false, 0, "W3/3 Tjedan 1 Dan 4 (SUB) 10 posto"),
      //   new TH_PriceRuleForCycleMoment(3,    1,  5,   0,10M,     0, false, 0, "W3/3 Tjedan 1 Dan 5 (NED) 10 posto"),
      //   new TH_PriceRuleForCycleMoment(3,    1,  6, 30M,  0,     0, false, 0, "W3/3 Tjedan 1 Dan 6 (PON) do 30kn" ),
      //   new TH_PriceRuleForCycleMoment(3,    1,  7, 25M,  0,     0, false, 0, "W3/3 Tjedan 1 Dan 7 (UTO) do 25kn" ),
      //   new TH_PriceRuleForCycleMoment(3,    2,  1, 20M,  0,     0, false, 0, "W3/3 Tjedan 2 Dan 1 (SRI) do 20kn" ),
      //   new TH_PriceRuleForCycleMoment(3,    2,  2, 20M,  0,     0, false, 0, "W3/3 Tjedan 2 Dan 2 (ČET) do 20kn" ),
      //   new TH_PriceRuleForCycleMoment(3,    2,  3, 15M,  0,     0, false, 0, "W3/3 Tjedan 2 Dan 3 (PET) do 15kn" ),
      //   new TH_PriceRuleForCycleMoment(3,    2,  4, 15M,  0,     0, false, 0, "W3/3 Tjedan 2 Dan 4 (SUB) do 15kn" ),
      //   new TH_PriceRuleForCycleMoment(3,    2,  5, 15M,  0,     0, false, 0, "W3/3 Tjedan 2 Dan 5 (NED) do 15kn" ),
      //   new TH_PriceRuleForCycleMoment(3,    2,  6, 15M,  0,33.33M, false, 0, "W3/3 Tjedan 2 Dan 6 (PON) do 15kn happy hour 33,33 posto" ),
      //   new TH_PriceRuleForCycleMoment(3,    2,  7, 10M,  0,     0, false, 0, "W3/3 Tjedan 2 Dan 7 (UTO) do 10kn" ),
      //   new TH_PriceRuleForCycleMoment(3,    3,  1, 10M,  0,     0, false, 0, "W3/3 Tjedan 3 Dan 1 (SRI) do 10kn" ),
      //   new TH_PriceRuleForCycleMoment(3,    3,  2,  7M,  0,     0, false, 0, "W3/3 Tjedan 3 Dan 2 (ČET) do  7kn" ),
      //   new TH_PriceRuleForCycleMoment(3,    3,  3,  7M,  0,     0, false, 0, "W3/3 Tjedan 3 Dan 3 (PET) do  7kn" ),
      //   new TH_PriceRuleForCycleMoment(3,    3,  4,  7M,  0,     0, false, 0, "W3/3 Tjedan 3 Dan 4 (SUB) do  7kn" ),
      //   new TH_PriceRuleForCycleMoment(3,    3,  5,  7M,  0,     0, false, 0, "W3/3 Tjedan 3 Dan 5 (NED) do  7kn" ),
      //   new TH_PriceRuleForCycleMoment(3,    3,  6,  5M,  0,     0, false, 0, "W3/3 Tjedan 3 Dan 6 (PON) do  5kn" ),
      //   new TH_PriceRuleForCycleMoment(3,    3,  7,  3M,  0,     0, false, 0, "W3/3 Tjedan 3 Dan 7 (UTO) do  3kn" ),
      //};

      // 23.12.2022. new 2023 EUR TH_PriceRuleList_W3_SRI_C3
      ZXC.TH_PriceRuleList_W3_SRI_C3 = new List<TH_PriceRuleForCycleMoment>()
      {
                                     // CK Tjed Dan  Max Rbt  HHprc  Letak  ExclsvKind   
         new TH_PriceRuleForCycleMoment(3,    1,  1,    0,  0,     0, false, 0, "W3/3 Tjedan 1 Dan 1 (SRI) regular"   ),
         new TH_PriceRuleForCycleMoment(3,    1,  2,    0,  0,     0, false, 0, "W3/3 Tjedan 1 Dan 2 (ČET) regular"   ),
         new TH_PriceRuleForCycleMoment(3,    1,  3,    0,10M,     0, false, 0, "W3/3 Tjedan 1 Dan 3 (PET) 10 posto"  ),
         new TH_PriceRuleForCycleMoment(3,    1,  4,    0,10M,     0, false, 0, "W3/3 Tjedan 1 Dan 4 (SUB) 10 posto"  ),
         new TH_PriceRuleForCycleMoment(3,    1,  5,    0,10M,     0, false, 0, "W3/3 Tjedan 1 Dan 5 (NED) 10 posto"  ),
         new TH_PriceRuleForCycleMoment(3,    1,  6, 3.5M,  0,     0, false, 0, "W3/3 Tjedan 1 Dan 6 (PON) do 3,5 EUR"),
         new TH_PriceRuleForCycleMoment(3,    1,  7,   3M,  0,     0, false, 0, "W3/3 Tjedan 1 Dan 7 (UTO) do   3 EUR"),
         new TH_PriceRuleForCycleMoment(3,    2,  1,   3M,  0,     0, false, 0, "W3/3 Tjedan 2 Dan 1 (SRI) do   3 EUR"),
         new TH_PriceRuleForCycleMoment(3,    2,  2, 2.5M,  0,     0, false, 0, "W3/3 Tjedan 2 Dan 2 (ČET) do 2,5 EUR"),
         new TH_PriceRuleForCycleMoment(3,    2,  3, 2.5M,  0,     0, false, 0, "W3/3 Tjedan 2 Dan 3 (PET) do 2,5 EUR"),
         new TH_PriceRuleForCycleMoment(3,    2,  4,   2M,  0,     0, false, 0, "W3/3 Tjedan 2 Dan 4 (SUB) do   2 EUR"),
         new TH_PriceRuleForCycleMoment(3,    2,  5,   2M,  0,     0, false, 0, "W3/3 Tjedan 2 Dan 5 (NED) do   2 EUR"),
         new TH_PriceRuleForCycleMoment(3,    2,  6,   2M,  0,     0, false, 0, "W3/3 Tjedan 2 Dan 6 (PON) do   2 EUR"),
         new TH_PriceRuleForCycleMoment(3,    2,  7, 1.5M,  0,     0, false, 0, "W3/3 Tjedan 2 Dan 7 (UTO) do 1,5 EUR"),
         new TH_PriceRuleForCycleMoment(3,    3,  1, 1.5M,  0,     0, false, 0, "W3/3 Tjedan 3 Dan 1 (SRI) do 1,5 EUR"),
         new TH_PriceRuleForCycleMoment(3,    3,  2,   1M,  0,     0, false, 0, "W3/3 Tjedan 3 Dan 2 (ČET) do   1 EUR"),
         new TH_PriceRuleForCycleMoment(3,    3,  3,   1M,  0,     0, false, 0, "W3/3 Tjedan 3 Dan 3 (PET) do   1 EUR"),
         new TH_PriceRuleForCycleMoment(3,    3,  4,   1M,  0,     0, false, 0, "W3/3 Tjedan 3 Dan 4 (SUB) do   1 EUR"),
         new TH_PriceRuleForCycleMoment(3,    3,  5,   1M,  0,     0, false, 0, "W3/3 Tjedan 3 Dan 5 (NED) do   1 EUR"),
         new TH_PriceRuleForCycleMoment(3,    3,  6, 0.5M,  0,     0, false, 0, "W3/3 Tjedan 3 Dan 6 (PON) do 0,5 EUR"),
         new TH_PriceRuleForCycleMoment(3,    3,  7, 0.5M,  0,     0, false, 0, "W3/3 Tjedan 3 Dan 7 (UTO) do 0,5 EUR"),
      };

      SetAndCheck_TH_PriceRuleList(ZXC.TH_PriceRuleList_W3_SRI_C3, TH_Cjenik_Kind._3WShop_SRI_3WCjenik_);

      #endregion TH_PriceRuleList_W3_SRI_C3

      #region TH_PriceRuleList_W3_SRI_C5

      ZXC.TH_PriceRuleList_W3_SRI_C5 = new List<TH_PriceRuleForCycleMoment>()
      {
                                     // CK Tjed Dan  Max Rbt HHprc  Letak  ExclsvKind   
         new TH_PriceRuleForCycleMoment(5,    1,  1,   0,  0,    0, false, 0, "W3/5 Tjedan 1 Dan 1 (SRI) regular"),
         new TH_PriceRuleForCycleMoment(5,    1,  2,   0,  0,    0, false, 0, "W3/5 Tjedan 1 Dan 2 (ČET) regular"),
         new TH_PriceRuleForCycleMoment(5,    1,  3,   0,  0,    0, false, 0, "W3/5 Tjedan 1 Dan 3 (PET) regular"),
         new TH_PriceRuleForCycleMoment(5,    1,  4,   0,  0,    0, false, 0, "W3/5 Tjedan 1 Dan 4 (SUB) regular"),
         new TH_PriceRuleForCycleMoment(5,    1,  5,   0,  0,    0, false, 0, "W3/5 Tjedan 1 Dan 5 (NED) regular"),
         new TH_PriceRuleForCycleMoment(5,    1,  6,   0,  0,    0, false, 0, "W3/5 Tjedan 1 Dan 6 (PON) regular"),
         new TH_PriceRuleForCycleMoment(5,    1,  7,   0,  0,    0, false, 0, "W3/5 Tjedan 1 Dan 7 (UTO) regular"),

         new TH_PriceRuleForCycleMoment(5,    2,  1,   0,  0,    0, false, 0, "W3/5 Tjedan 2 Dan 1 (SRI) regular"),
         new TH_PriceRuleForCycleMoment(5,    2,  2,   0,  0,    0, false, 0, "W3/5 Tjedan 2 Dan 2 (ČET) regular"),
         new TH_PriceRuleForCycleMoment(5,    2,  3,   0,  0,    0, false, 0, "W3/5 Tjedan 2 Dan 3 (PET) regular"),
         new TH_PriceRuleForCycleMoment(5,    2,  4,   0,  0,    0, false, 0, "W3/5 Tjedan 2 Dan 4 (SUB) regular"),
         new TH_PriceRuleForCycleMoment(5,    2,  5,   0,  0,    0, false, 0, "W3/5 Tjedan 2 Dan 5 (NED) regular"),
         new TH_PriceRuleForCycleMoment(5,    2,  6, 30M,  0,    0, false, 0, "W3/5 Tjedan 2 Dan 6 (PON) do 30kn"),
         new TH_PriceRuleForCycleMoment(5,    2,  7, 30M,  0,    0, false, 0, "W3/5 Tjedan 2 Dan 7 (UTO) do 30kn"),

         new TH_PriceRuleForCycleMoment(5,    3,  1, 25M,  0,    0, false, 0, "W3/5 Tjedan 3 Dan 1 (SRI) do 25kn"),
         new TH_PriceRuleForCycleMoment(5,    3,  2, 25M,  0,    0, false, 0, "W3/5 Tjedan 3 Dan 2 (ČET) do 25kn"),
         new TH_PriceRuleForCycleMoment(5,    3,  3, 25M,  0,    0, false, 0, "W3/5 Tjedan 3 Dan 3 (PET) do 25kn"),
         new TH_PriceRuleForCycleMoment(5,    3,  4, 25M,  0,    0, false, 0, "W3/5 Tjedan 3 Dan 4 (SUB) do 25kn"),
         new TH_PriceRuleForCycleMoment(5,    3,  5, 25M,  0,    0, false, 0, "W3/5 Tjedan 3 Dan 5 (NED) do 25kn"),
         new TH_PriceRuleForCycleMoment(5,    3,  6, 20M,  0,    0, false, 0, "W3/5 Tjedan 3 Dan 6 (PON) do 20kn"),
         new TH_PriceRuleForCycleMoment(5,    3,  7, 20M,  0,    0, false, 0, "W3/5 Tjedan 3 Dan 7 (UTO) do 20kn"),

         new TH_PriceRuleForCycleMoment(5,    4,  1, 15M,  0,    0, false, 0, "W3/5 Tjedan 4 Dan 1 (SRI) do 15kn"),
         new TH_PriceRuleForCycleMoment(5,    4,  2, 15M,  0,    0, false, 0, "W3/5 Tjedan 4 Dan 2 (ČET) do 15kn"),
         new TH_PriceRuleForCycleMoment(5,    4,  3, 15M,  0,    0, false, 0, "W3/5 Tjedan 4 Dan 3 (PET) do 15kn"),
         new TH_PriceRuleForCycleMoment(5,    4,  4, 15M,  0,    0, false, 0, "W3/5 Tjedan 4 Dan 4 (SUB) do 15kn"),
         new TH_PriceRuleForCycleMoment(5,    4,  5, 15M,  0,    0, false, 0, "W3/5 Tjedan 4 Dan 5 (NED) do 15kn"),
         new TH_PriceRuleForCycleMoment(5,    4,  6, 10M,  0,    0, false, 0, "W3/5 Tjedan 4 Dan 6 (PON) do 10kn"),
         new TH_PriceRuleForCycleMoment(5,    4,  7, 10M,  0,    0, false, 0, "W3/5 Tjedan 4 Dan 7 (UTO) do 10kn"),

         new TH_PriceRuleForCycleMoment(5,    5,  1, 10M,  0,    0, false, 0, "W3/5 Tjedan 5 Dan 1 (SRI) do 10kn"),
         new TH_PriceRuleForCycleMoment(5,    5,  2,  7M,  0,    0, false, 0, "W3/5 Tjedan 5 Dan 2 (ČET) do  7kn"),
         new TH_PriceRuleForCycleMoment(5,    5,  3,  7M,  0,    0, false, 0, "W3/5 Tjedan 5 Dan 3 (PET) do  7kn"),
         new TH_PriceRuleForCycleMoment(5,    5,  4,  7M,  0,    0, false, 0, "W3/5 Tjedan 5 Dan 4 (SUB) do  7kn"),
         new TH_PriceRuleForCycleMoment(5,    5,  5,  7M,  0,    0, false, 0, "W3/5 Tjedan 5 Dan 5 (NED) do  7kn"),
         new TH_PriceRuleForCycleMoment(5,    5,  6,  5M,  0,    0, false, 0, "W3/5 Tjedan 5 Dan 6 (PON) do  5kn"),
         new TH_PriceRuleForCycleMoment(5,    5,  7,  5M,  0,    0, false, 0, "W3/5 Tjedan 5 Dan 7 (UTO) do  5kn"),
      };


      SetAndCheck_TH_PriceRuleList(ZXC.TH_PriceRuleList_W3_SRI_C5, TH_Cjenik_Kind._3WShop_SRI_5WCjenik_);

      #endregion TH_PriceRuleList_W3_SRI_C5

      #region TH_PriceRuleList_W3_SRI_C4

      ZXC.TH_PriceRuleList_W3_SRI_C4 = new List<TH_PriceRuleForCycleMoment>()
      {
                                     // CK Tjed Dan   Max  Rbt HHprc  Letak  ExclsvKind   
         new TH_PriceRuleForCycleMoment(4,    1,  1,    0,   0,    0, false, 0, "W3/4 Tjedan 1 Dan 1 (SRI) regular"   ),// 14.12.2022.
         new TH_PriceRuleForCycleMoment(4,    1,  2,    0,   0,    0, false, 0, "W3/4 Tjedan 1 Dan 2 (ČET) regular"   ),// 15.12.2022.
         new TH_PriceRuleForCycleMoment(4,    1,  3,    0, 10M,    0, false, 0, "W3/4 Tjedan 1 Dan 3 (PET) 10 posto"  ),// 16.12.2022.
         new TH_PriceRuleForCycleMoment(4,    1,  4,    0, 10M,    0, false, 0, "W3/4 Tjedan 1 Dan 4 (SUB) 10 posto"  ),// 17.12.2022.
         new TH_PriceRuleForCycleMoment(4,    1,  5,    0, 10M,    0, false, 0, "W3/4 Tjedan 1 Dan 5 (NED) 10 posto"  ),// 18.12.2022.
         new TH_PriceRuleForCycleMoment(4,    1,  6,  30M,   0,    0, false, 0, "W3/4 Tjedan 1 Dan 6 (PON) do 30kn"   ),// 19.12.2022.
         new TH_PriceRuleForCycleMoment(4,    1,  7,  30M,   0,    0, false, 0, "W3/4 Tjedan 1 Dan 7 (UTO) do 30kn"   ),// 20.12.2022.
                                                                                                                        
         new TH_PriceRuleForCycleMoment(4,    2,  1,  30M,   0,    0, false, 0, "W3/4 Tjedan 2 Dan 1 (SRI) do 30kn"   ),// 21.12.2022.
         new TH_PriceRuleForCycleMoment(4,    2,  2,  25M,   0,    0, false, 0, "W3/4 Tjedan 2 Dan 2 (ČET) do 25kn"   ),// 22.12.2022.
         new TH_PriceRuleForCycleMoment(4,    2,  3,  25M,   0,    0, false, 0, "W3/4 Tjedan 2 Dan 3 (PET) do 25kn"   ),// 23.12.2022.
         new TH_PriceRuleForCycleMoment(4,    2,  4,  20M,   0,    0, false, 0, "W3/4 Tjedan 2 Dan 4 (SUB) do 20kn"   ),// 24.12.2022.
         new TH_PriceRuleForCycleMoment(4,    2,  5,  20M,   0,    0, false, 0, "W3/4 Tjedan 2 Dan 5 (NED) do 20kn"   ),// 25.12.2022.
         new TH_PriceRuleForCycleMoment(4,    2,  6,  20M,   0,    0, false, 0, "W3/4 Tjedan 2 Dan 6 (PON) do 20kn"   ),// 26.12.2022.
         new TH_PriceRuleForCycleMoment(4,    2,  7,  20M,   0,    0, false, 0, "W3/4 Tjedan 2 Dan 7 (UTO) do 20kn"   ),// 27.12.2022.
                                                                                                                        
         new TH_PriceRuleForCycleMoment(4,    3,  1,  20M,   0,    0, false, 0, "W3/4 Tjedan 3 Dan 1 (SRI) do 20kn"   ),// 28.12.2022.
         new TH_PriceRuleForCycleMoment(4,    3,  2,  15M,   0,    0, false, 0, "W3/4 Tjedan 3 Dan 2 (ČET) do 15kn"   ),// 29.12.2022.
         new TH_PriceRuleForCycleMoment(4,    3,  3,  15M,   0,    0, false, 0, "W3/4 Tjedan 3 Dan 3 (PET) do 15kn"   ),// 30.12.2022.
         new TH_PriceRuleForCycleMoment(4,    3,  4,  15M,   0,    0, false, 0, "W3/4 Tjedan 3 Dan 4 (SUB) do 15kn"   ),// 31.12.2022.
         new TH_PriceRuleForCycleMoment(4,    3,  5,  15M,   0,    0, false, 0, "W3/4 Tjedan 3 Dan 5 (NED) do 15kn!!!"),// 01.01.2022. nulti zpc u EUR - treba napraviti novi sa 02.01.2023.
         new TH_PriceRuleForCycleMoment(4,    3,  6,   2M,   0,    0, false, 0, "W3/4 Tjedan 3 Dan 6 (PON) do 2 EUR"  ),// 02.01.2022.
         new TH_PriceRuleForCycleMoment(4,    3,  7,   2M,   0,    0, false, 0, "W3/4 Tjedan 3 Dan 7 (UTO) do 2 EUR"  ),// 03.01.2022.
                                                                                                                       
         new TH_PriceRuleForCycleMoment(4,    4,  1,   1M,   0,    0, false, 0, "W3/4 Tjedan 4 Dan 1 (SRI) do 1 EUR"  ),// 04.01.2023.
         new TH_PriceRuleForCycleMoment(4,    4,  2,   1M,   0,    0, false, 0, "W3/4 Tjedan 4 Dan 2 (ČET) do 1 EUR"  ),// 05.01.2023.
         new TH_PriceRuleForCycleMoment(4,    4,  3,   1M,   0,    0, false, 0, "W3/4 Tjedan 4 Dan 3 (PET) do 1 EUR"  ),// 06.01.2023.
         new TH_PriceRuleForCycleMoment(4,    4,  4,   1M,   0,    0, false, 0, "W3/4 Tjedan 4 Dan 4 (SUB) do 1 EUR"  ),// 07.01.2023.
         new TH_PriceRuleForCycleMoment(4,    4,  5,   1M,   0,    0, false, 0, "W3/4 Tjedan 4 Dan 5 (NED) do 1 EUR"  ),// 08.01.2023.
         new TH_PriceRuleForCycleMoment(4,    4,  6, 0.5M,   0,    0, false, 0, "W3/4 Tjedan 4 Dan 6 (PON) do 0,5 EUR"),// 09.01.2023.
         new TH_PriceRuleForCycleMoment(4,    4,  7, 0.5M,   0,    0, false, 0, "W3/4 Tjedan 4 Dan 7 (UTO) do 0,5 EUR"),// 10.01.2023.

      };


      SetAndCheck_TH_PriceRuleList(ZXC.TH_PriceRuleList_W3_SRI_C4, TH_Cjenik_Kind._3WShop_SRI_4WCjenik_);

      #endregion TH_PriceRuleList_W3_SRI_C4

   }

   public static bool SetAndCheck_TH_PriceRuleList(List<TH_PriceRuleForCycleMoment> TH_PriceRuleList_WX_CY, TH_Cjenik_Kind cjenikKind)
   {
      #region Set ZPC_ID, ZPC_Age

      decimal currCij  = TH_PriceRuleList_WX_CY.First().MaxPrice;
      uint currZPC_ID  = TH_PriceRuleList_WX_CY.First().RuleID  ;
      uint currZPC_Age = 0;

      foreach(TH_PriceRuleForCycleMoment rule in TH_PriceRuleList_WX_CY)
      {
         if(currCij != rule.MaxPrice)
         {
            currCij     = rule.MaxPrice;
            currZPC_ID  = rule.RuleID  ;
            currZPC_Age = 0            ;
         }

         rule.ZPC_ID  = currZPC_ID   ;
         rule.ZPC_Age = currZPC_Age++;
      }

      #endregion Set ZPC_ID, ZPC_Age

      #region Some Checks

      bool OK = true;

      int listCount = TH_PriceRuleList_WX_CY                                       .Count();
      int distCount = TH_PriceRuleList_WX_CY.Select(rule => rule.RuleID).Distinct().Count();

      // Check Bijekcija 
      if(listCount != distCount) throw new Exception(cjenikKind + " TH_PriceRuleForCycleMoment\n\nListCount != Distinct RuleID Count!");

      // Check Days count 
      if(listCount != TH_PriceRuleList_WX_CY.First().CjenikKind * 7) throw new Exception(cjenikKind + " TH_PriceRuleForCycleMoment\n\nListCount != CjenikKind * 7!");

      var samePriceGroups   = TH_PriceRuleList_WX_CY.GroupBy(rule => rule.SamePriceGroup);
      var sameZPC_ID_Groups = TH_PriceRuleList_WX_CY.GroupBy(rule => rule.ZPC_ID        );

      int ZPC_ID_DistinctPriceCount;

      // Check all rules with same ZPC_ID has same price 
      foreach(var sameZPC_ID_GR in sameZPC_ID_Groups)
      {
         ZPC_ID_DistinctPriceCount = sameZPC_ID_GR.Select(item => item.MaxPrice).Distinct().Count();

         if(ZPC_ID_DistinctPriceCount != 1) throw new Exception(cjenikKind + " TH_PriceRuleForCycleMoment\n\n ZPC_ID_DistinctPriceCount veće od 1!\n\nZPC_ID: " + sameZPC_ID_GR.Key.ToString() + "\n\nCount: " + ZPC_ID_DistinctPriceCount);
      }

      // ExclsvKind = 1 Only Once 
      int exclsvKindNa50postoCount = TH_PriceRuleList_WX_CY.Count(rule => rule.ExclsvKind == 1);
      if(exclsvKindNa50postoCount > 1) throw new Exception(cjenikKind + " TH_PriceRuleForCycleMoment\n\n exclsvKindNa50postoCount veće od 1!\n\nCount: " + exclsvKindNa50postoCount);

      // Check RuleID - ZPC_ID = ZPC_Age 
      //TH_PriceRuleList_WX_CY.ForEach(rule => { if(rule.ZPC_Age2 != rule.ZPC_Age)  throw new Exception(rule + "\n\nZPC_Age2 != ZPC_Age!\n\n" + rule.ZPC_Age2 + " != " + rule.ZPC_Age); });

      #endregion Some Checks

      return OK;
   }

   #endregion Init_SetAndCheck_TH_PriceRuleList

   #region NON STATIC Util Metodz

   public DateTime GetActiveZPCdate_ForThisDay(DateTime dokDate)
   {
      return GetActiveZPCdate_ForThisDay(this, dokDate);
   }

   #endregion NON STATIC Util Metodz

   #region STATIC Util Metodz

   // Voila: 
   internal static TH_PriceRuleForCycleMoment GetTHPR_ForThisDay /*_via_TH_Calendar*/(string skladCD, DateTime dokDate)
   {
      // !!! XXX !!! jbtpsmter!!!:
      if(skladCD.IsEmpty() || ZXC.IsSkladCD_THshop(skladCD) == false)
      {
         skladCD = ZXC.luiListaSkladista.FirstOrDefault(lui => lui.Cd.Length >= 2 && lui.Cd.Substring(0, 2) == ZXC.vvDB_ServerID.ToString()).Cd;
      }

      try
      { 
         // 25.09.2018: nestaje 'IsTH_2WeekShop' te se razlucije na IsTH_2Week_PON_Shop i IsTH_2Week_SRI_Shop 
       //if(ZXC.IsTH_2WeekShop(skladCD))      return ZXC.TH_Cjenik_Calendar_2Week    [dokDate.Date];  // Volia! Slick. 
         if(ZXC.IsTH_2Week_PON_Shop(skladCD)) return ZXC.TH_Cjenik_Calendar_2Week_PON[dokDate.Date];  // Volia! Slick. 
         if(ZXC.IsTH_2Week_SRI_Shop(skladCD)) return ZXC.TH_Cjenik_Calendar_2Week_SRI[dokDate.Date];  // Volia! Slick. 
         if(ZXC.IsTH_5WeekShop     (skladCD)) return ZXC.TH_Cjenik_Calendar_5Week    [dokDate.Date];  // Volia! Slick. 
         if(ZXC.IsTH_3Week_SRI_Shop(skladCD)) return ZXC.TH_Cjenik_Calendar_3Week_SRI[dokDate.Date];  // Volia! Slick. //31.08.2020.
      }
      catch { }

      ZXC.aim_emsg(MessageBoxIcon.Error, "GetTHPR_ForThisDay_via_TH_Calendar undefined!\r\n\r\nsklad [" + skladCD + "]\r\n\r\ndokDate [" + dokDate.ToString(ZXC.VvDateFormat) + "]");

      return null;
   }

#if GetTHPR_ForThisDay_OLD

   internal static TH_PriceRuleForCycleMoment GetTHPR_ForThisDay_OLD                 (string skladCD, DateTime dokDate)
   {
      // !!! XXX !!! jbtpsmter!!!:
      if(skladCD.IsEmpty() || ZXC.IsSkladCD_THshop(skladCD) == false)
      {
         skladCD = ZXC.luiListaSkladista.FirstOrDefault(lui => lui.Cd.Length >= 2 && lui.Cd.Substring(0, 2) == ZXC.vvDB_ServerID.ToString()).Cd;
      }

      ZXC.TH_Cjenik_Kind TH_Cjenik_Kind = ZXC.Get_TH_Cjenik_Kind(dokDate, skladCD);

      if(TH_Cjenik_Kind == ZXC.TH_Cjenik_Kind._NEDEFINIRANO_) return null;

      List<TH_PriceRuleForCycleMoment> TH_PriceRuleList_WX_CY = null;

      TimeSpan elapsedFrom_CurrCycleStart = TimeSpan.Zero;

      switch(TH_Cjenik_Kind)
      {
         case ZXC.TH_Cjenik_Kind._2WShop_2WCjenik_: 
            TH_PriceRuleList_WX_CY     = ZXC.TH_PriceRuleList_W2_C2; 
            elapsedFrom_CurrCycleStart = ZXC.Get_elapsedFrom_CurrCycleStart_2weekShop(dokDate, skladCD, false); 
            break;
         case ZXC.TH_Cjenik_Kind._2WShop_3WCjenik_: 
            TH_PriceRuleList_WX_CY     = ZXC.TH_PriceRuleList_W2_C3; 
            elapsedFrom_CurrCycleStart = ZXC.Get_elapsedFrom_CurrCycleStart_2weekShop(dokDate, skladCD, true ); 
            break;
         case ZXC.TH_Cjenik_Kind._5WShop_4WCjenik_: 
            TH_PriceRuleList_WX_CY     = ZXC.TH_PriceRuleList_W5_C4; 
            elapsedFrom_CurrCycleStart = ZXC.Get_elapsedFrom_CurrCycleStart_5weekShop(dokDate, skladCD, false); 
            break;
         case ZXC.TH_Cjenik_Kind._5WShop_5WCjenik_: 
            TH_PriceRuleList_WX_CY     = ZXC.TH_PriceRuleList_W5_C5; 
            elapsedFrom_CurrCycleStart = ZXC.Get_elapsedFrom_CurrCycleStart_5weekShop(dokDate, skladCD, false); 
            break;
         case ZXC.TH_Cjenik_Kind._5WShop_6WCjenik_: 
            TH_PriceRuleList_WX_CY     = ZXC.TH_PriceRuleList_W5_C6; 
            elapsedFrom_CurrCycleStart = ZXC.Get_elapsedFrom_CurrCycleStart_5weekShop(dokDate, skladCD, true ); 
            break;
      }

      int ruleIDX = (int)(Math.Floor(elapsedFrom_CurrCycleStart.TotalDays)); // offset od prvog dana ciklusa tj. rowIdx u listi 

      if(ruleIDX.IsZeroOrPositive()) return TH_PriceRuleList_WX_CY[ruleIDX];

      return null;
   }

#endif
   internal static decimal Get_ZPC_NewMalopCij(XSqlConnection conn, TH_PriceRuleForCycleMoment theTHPR, Artikl artWast_rec, Faktur nultiZpcFaktur_rec, bool isChkOnly)
   {
      string  artiklCD    = artWast_rec.ArtiklCD   ;
      string  skladCD     = artWast_rec.AS_SkladCD ;
      string  artGR2      = artWast_rec.AS_ArtGrCd2;
      decimal oldMalopCij = artWast_rec.AS_KnjigCij;
      decimal newMalopCij = oldMalopCij            ;

      string  artGR1      = artWast_rec.AS_ArtGrCd1;

      bool isODEandAkat   = artGR1 == "Akat" && artGR2 == "ODE"; // Odjeca Exclusive A kategorije (ono sto od 03.2021. hoce promjenjeni tretman cijena) 
                                                                 // ODEandAkat treba uvijek imati cijene sa nultog ZPC-a (najvece) 
      #region TimeForNultiZPCprice

      // 15.03.2021: 
    //if(theTHPR.IsTimeForNultiZPCprice                )
      // 11.06.2021: opet vracamo na staro, kak je bilo prije 15.03.2021: 
    //if(theTHPR.IsTimeForNultiZPCprice || isODEandAkat)
      if(theTHPR.IsTimeForNultiZPCprice                )
         {
         Rtrans nultiZpcRtrans_rec = nultiZpcFaktur_rec.Transes.FirstOrDefault(rtr => rtr.T_artiklCD == artiklCD);

         if(nultiZpcRtrans_rec != null) newMalopCij = nultiZpcRtrans_rec.T_noCijMal;
         else                           newMalopCij =                            0M;
      }

      #endregion TimeForNultiZPCprice

      #region No need for nulti ZPC. Analyse other days.

      else 
      {
         if(artGR2 == "ODE" && theTHPR.ExclsvKind != 0) // Artikl spada u exclusive artikl i treba poseban tretman 
         {
            if(theTHPR.ExclsvKind == 1) // exclusive artikl rezi na 50% 
            {
               if(isChkOnly) newMalopCij = oldMalopCij     ; // ne mijenjaj   
               else          newMalopCij = oldMalopCij / 2M; // dijeli na 50% 
            }
            else if(theTHPR.ExclsvKind == 2) // exclusive artikl zadrzi na 50% 
            {
               newMalopCij = oldMalopCij;
            }
            else
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "'ODE' artikl a ExclsvKind mu nije ni 0, ni 1 ni 2!");
               newMalopCij = oldMalopCij;
            }
         }

         // 04.6.2018: novoprepoznati NERIJESENI problem: neki put ZPC treba DIGNUTI a ne spustiti cijene; 
         else if(theTHPR.MaxPrice.NotZero() && oldMalopCij > theTHPR.MaxPrice) // obican not exclusive artikl 
         {
            newMalopCij = theTHPR.MaxPrice; // Šišaj cijene na MaxPrice 
         }
      }

      #endregion No need for nulti ZPC. Analyse other days.

      return newMalopCij;
   }

   public static DateTime GetActiveZPCdate_ForThisDay(TH_PriceRuleForCycleMoment rule, DateTime dokDate)
   {
      return dokDate - rule.ZPC_Age_InDays;
   }

   #endregion STATIC Util Metodz

}