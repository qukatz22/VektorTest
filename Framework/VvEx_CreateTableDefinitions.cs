using System;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand = MySql.Data.MySqlClient.MySqlCommand;
using XSqlDataReader = MySql.Data.MySqlClient.MySqlDataReader;
using XSqlDbType = MySql.Data.MySqlClient.MySqlDbType;
using XSqlParameter = MySql.Data.MySqlClient.MySqlParameter;
using XSqlException = MySql.Data.MySqlClient.MySqlException;
using XSqlDataAdapter = MySql.Data.MySqlClient.MySqlDataAdapter;
#endif

public static class VvSkyLog_Ex
{
   public static string Create_table_definition()
   {
      return(
         "syncErrNo     int(8)  unsigned        NOT NULL DEFAULT 0                     COMMENT 'Eventualni error number. 0 if no error',               \n" +
         "syncMessage   varchar(1024)           NOT NULL DEFAULT ''                    COMMENT 'Error ili neki drugi message',                         \n" +
         "operation     enum ('NONE', 'RECEIVE', 'SEND', 'SendAndReceive', 'OpenSyncTran') NOT NULL,                                                   \n" +
         "resultAction  enum('ADD','RWT','DEL', 'NONE', 'UTIL') NOT NULL COMMENT 'resulting action za eventualni paket pojava recID-a',                \n" +
         "thisSyncTS    timestamp               NOT NULL DEFAULT '0000-00-00 00:00:00' COMMENT 'TS ove       sinhronizacije ',                         \n" +
         "prevSyncTS    timestamp               NOT NULL DEFAULT '0000-00-00 00:00:00' COMMENT 'TS prethodne sinhronizacije ',                         \n" +
         "skyLogTS      timestamp               NOT NULL DEFAULT CURRENT_TIMESTAMP     COMMENT 'TS nastanka ovog LogEntry-a',                          \n" +
         "skyLogID      int(10) unsigned        NOT NULL AUTO_INCREMENT                COMMENT 'vlastiti recID - PrimaryKey',                          \n" +
         "ruleRecID     int(10) unsigned        NOT NULL DEFAULT 0                     COMMENT 'Veza na SkyRule record koji je stvorio ovaj log entry',\n" +

         "record        varchar(16)             NOT NULL DEFAULT '',                                                                 \n" +
         "isSkyTraffic  tinyint(1) unsigned     NOT NULL DEFAULT '0',                                                                \n" +
         "origSrvID     int(4)  unsigned        NOT NULL DEFAULT 0,                                                                  \n" +
         "origRecID     int(10) unsigned        NOT NULL DEFAULT 0,                                                                  \n" +
         "tt            varchar(6)              NOT NULL DEFAULT '',                                                                 \n" +
         "skladCD       varchar(6)              NOT NULL DEFAULT '',                                                                 \n" +
         "skladCD2      varchar(6)              NOT NULL DEFAULT '',                                                                 \n" +
         "action        enum('ADD','RWT','DEL') NOT NULL,                                                                            \n" +
         "lanLogTS      timestamp               NOT NULL DEFAULT '0000-00-00 00:00:00',                                              \n" +
         "lanLogUID     varchar(16)             NOT NULL DEFAULT '',                                                                 \n" +
         "addTS         timestamp               NOT NULL DEFAULT '0000-00-00 00:00:00',                                              \n" +
         "modTS         timestamp               NOT NULL DEFAULT '0000-00-00 00:00:00',                                              \n" +
         "addUID        varchar(16)             NOT NULL DEFAULT '',                                                                 \n" +
         "modUID        varchar(16)             NOT NULL DEFAULT '',                                                                 \n" +
         "lanServerID   int(4) unsigned         NOT NULL DEFAULT 0,                                                                  \n" +
         "lanServerName varchar(32)             NOT NULL DEFAULT 'nepoznat' COMMENT 'IP adresa ili hostName odakle dolazi akcija',   \n" +
         "lanClientName varchar(32)             NOT NULL DEFAULT 'nepoznat',                                                         \n" +
         "currRecID     int(10) unsigned        NOT NULL COMMENT 'ID recorda na koji se entry odnosi',                               \n" +
         "lanLogID      int(10) unsigned        NOT NULL DEFAULT 0 COMMENT 'za JOIN na vvLanLog, daj i INDEX',                       \n" +

         "PRIMARY KEY        (skyLogID),\n" +
         "INDEX   byLanLogID (lanLogID),\n" +
         "INDEX   bySkyLogTS (skyLogTS) " 
      );
   }
}

public static class VvLog_Ex
{
   public static string Create_table_definition()
   {
      return(
         "record        varchar(16)             NOT NULL DEFAULT '',                                                                 \n" +
         "isSkyTraffic  tinyint(1) unsigned     NOT NULL DEFAULT '0',                                                                \n" +
         "origSrvID     int(4)  unsigned        NOT NULL DEFAULT 0,                                                                  \n" +
         "origRecID     int(10) unsigned        NOT NULL DEFAULT 0,                                                                  \n" +
         "tt            varchar(6)              NOT NULL DEFAULT '',                                                                 \n" +
         "skladCD       varchar(6)              NOT NULL DEFAULT '',                                                                 \n" +
         "skladCD2      varchar(6)              NOT NULL DEFAULT '',                                                                 \n" +
         "action        enum('ADD','RWT','DEL') NOT NULL,                                                                            \n" +
         "logTS         timestamp               NOT NULL DEFAULT CURRENT_TIMESTAMP,                                                  \n" +
         "logUID        varchar(16)             NOT NULL DEFAULT '',                                                                 \n" +
         "addTS         timestamp               NOT NULL DEFAULT '0000-00-00 00:00:00',                                              \n" +
         "modTS         timestamp               NOT NULL DEFAULT '0000-00-00 00:00:00',                                              \n" +
         "addUID        varchar(16)             NOT NULL DEFAULT '',                                                                 \n" +
         "modUID        varchar(16)             NOT NULL DEFAULT '',                                                                 \n" +
         "lanServerID   int(4) unsigned         NOT NULL DEFAULT 0,                                                                  \n" +
         "lanServerName varchar(32)             NOT NULL DEFAULT 'nepoznat' COMMENT 'IP adresa ili hostName odakle dolazi akcija',   \n" +
         "clientName    varchar(32)             NOT NULL DEFAULT 'nepoznat',                                                         \n" +
         "recID         int(10) unsigned        NOT NULL COMMENT 'ID recorda na koji se entry odnosi',                               \n" +
         "logID         int(10) unsigned        NOT NULL AUTO_INCREMENT COMMENT 'log entry ID-a',                                    \n" +

         "PRIMARY KEY         (logID)        ,      \n" +
         "INDEX byLogTS       (logTS)        ,      \n" + 
         "INDEX byRecord      (record, logTS),      \n" +
         "INDEX byLanSrvRecID (origSrvID, origRecID)\n"
      );
   }

   // prervious NEW 
   //public static string AlterTo_NEW_lanlog_table_definitionText(string dbName)
   //{
   //   return (
   //      "ALTER TABLE " + dbName + "." + ZXC.vvDB_OLD_logTableName + " RENAME TO " + ZXC.vvDB_logTableName + ", " + "\n" +
   //      " CHANGE COLUMN logHost clientName VARCHAR(32) NOT NULL DEFAULT 'nepoznat', "                             + "\n" +
   //      " ADD COLUMN origSrvID     int(4)  unsigned    NOT NULL DEFAULT 0                     AFTER record     ," + "\n" +
   //      " ADD COLUMN origRecID     int(10) unsigned    NOT NULL DEFAULT 0                     AFTER origSrvID  ," + "\n" +
   //      " ADD COLUMN tt VARCHAR(6)                     NOT NULL DEFAULT ''                    AFTER origRecID  ," + "\n" +
   //      " ADD COLUMN skladCD         VARCHAR(6)        NOT NULL DEFAULT ''                    AFTER tt         ," + "\n" +
   //      " ADD COLUMN skladCD2        VARCHAR(6)        NOT NULL DEFAULT ''                    AFTER skladCD    ," + "\n" +
   //      " ADD COLUMN addTS           TIMESTAMP         NOT NULL DEFAULT '0000-00-00 00:00:00' AFTER logUID     ," + "\n" +
   //      " ADD COLUMN modTS           TIMESTAMP         NOT NULL DEFAULT '0000-00-00 00:00:00' AFTER addTS      ," + "\n" +
   //      " ADD COLUMN addUID          VARCHAR(16)       NOT NULL DEFAULT ''                    AFTER modTS      ," + "\n" +
   //      " ADD COLUMN modUID          VARCHAR(16)       NOT NULL DEFAULT ''                    AFTER addUID     ," + "\n" +
   //      " ADD COLUMN lanServerID INT(4) UNSIGNED       NOT NULL DEFAULT 0                     AFTER modUID     ," + "\n" +
   //      " ADD COLUMN lanServerName VARCHAR(32)         NOT NULL DEFAULT 'nepoznat'            AFTER lanServerID," + "\n" +
   //      " ADD COLUMN isSkyTraffic  TINYINT(1) UNSIGNED NOT NULL DEFAULT 0                     AFTER record     ," + "\n" +
   //      " ADD INDEX byLogTS(logTS),                                    "                                          + "\n" +
   //      " ADD INDEX byRecord(record, logTS)                            "                                          + "\n"
   //   );
   //}

   public static string AlterTo_NEW_lanlog_table_definitionText(string dbName)
   {
      return (
         "ALTER TABLE " + dbName + "." + ZXC.vvDB_OLD_logTableName + " RENAME TO " + ZXC.vvDB_LANlogTableName + ", " + "\n" +
         " ADD COLUMN skladCD2        VARCHAR(6)        NOT NULL DEFAULT ''                    AFTER skladCD    " + "\n"
      );
   }

}

public static class VvMBF_Ex
{
   public static string Create_table_definition()
   {
      return(
         "logTS         timestamp               NOT NULL DEFAULT CURRENT_TIMESTAMP,                                                  \n" +
         "rtrCount      int(10) unsigned        NOT NULL DEFAULT 0,                                                                  \n" +
         "ftrCount      int(10) unsigned        NOT NULL DEFAULT 0,                                                                  \n" +
         "ptrCount      int(10) unsigned        NOT NULL DEFAULT 0,                                                                  \n" +
         "atrCount      int(10) unsigned        NOT NULL DEFAULT 0,                                                                  \n" +
         "xtrCount      int(10) unsigned        NOT NULL DEFAULT 0,                                                                  \n" + 
         "serverName    varchar(32)             NOT NULL DEFAULT 'nepoznat' COMMENT 'IP adresa ili hostName odakle dolazi akcija',   \n" + // ZXC.vvDB_Server                
         "serverID      int(4) unsigned         NOT NULL DEFAULT 0,                                                                  \n" + // ZXC.vvDB_ServerID              
         "vvDomena      char(6)                 NOT NULL default '' ,                                                                \n" + // ZXC.vvDB_VvDomena              
         "year          char(6)                 NOT NULL default '' ,                                                                \n" + // ZXC.projectYear                
         "userName      varchar(16)             NOT NULL DEFAULT '',                                                                 \n" + // ZXC.vvDB_User                  
         "clientName    varchar(32)             NOT NULL DEFAULT 'nepoznat',                                                         \n" + // System.Environment.MachineName 
         "prjktCD       int(6)         unsigned NOT NULL default '0',                                                                \n" + // ZXC.CURR_prjkt_rec.KupdobCD    
         "prjktTK       char(6)                 NOT NULL default '' ,                                                                \n" + // ZXC.CURR_prjkt_rec.Ticker      
         "prjktName     varchar(50)             NOT NULL default '' ,                                                                \n" + // ZXC.CURR_prjkt_rec.Naziv       
         "appVersion    varchar(32)             NOT NULL DEFAULT '',                                                                 \n" + // VvProgramVersion               
         "clientIP      varchar(32)             NOT NULL DEFAULT '',                                                                 \n" + // GetClientIPaddress             

         "logID         int(10) unsigned        NOT NULL AUTO_INCREMENT COMMENT 'log entry ID-a',                                    \n" +

         "PRIMARY KEY         (logID)        ,      \n" +
         "INDEX byLogTS       (logTS)               \n"
      );
   }

}

public static class Process_Ex
{
   #region CreateTableProcess

   public static string Create_table_definition()
   {
      return (
        "recID    int(10)              unsigned          NOT NULL auto_increment,\n" +
        "procTS   timestamp                              NOT NULL default CURRENT_TIMESTAMP,\n" +
        "procUID  varchar(16) collate latin2_croatian_ci NOT NULL default 'XY',\n" +
        "procHost varchar(32) collate latin2_croatian_ci NOT NULL default '' COMMENT 'IP adresa ili hostName odakle dolazi akcija',\n" +
        "forma    varchar(32) collate latin2_croatian_ci NOT NULL default '' COMMENT 'Otvoreni TabPage (forma)',\n" +

        "PRIMARY KEY  (recID)\n"
      );
   }

   #endregion CreateTableProcess

}

public static class VvLocker_Ex
{
   public static string Create_table_definition()
   {
      return (
        "lockID             int(10) unsigned NOT NULL auto_increment     COMMENT 'log entry ID-a',\n" +
        "tableName          varchar(16)      NOT NULL default 'Vv',\n" +
        "recID              int(10) unsigned NOT NULL                    COMMENT 'ID recorda na koji se entry odnosi',\n" +
        "editorUID          varchar(16)      NOT NULL default 'XY',\n" +
        "inEditTS           timestamp        NOT NULL default CURRENT_TIMESTAMP,\n" +
        "environMachineName varchar(32)      NOT NULL default 'nepoznat' COMMENT 'IP adresa ili hostName odakle dolazi akcija',\n" +
        "environUserName    varchar(32)      NOT NULL default 'nepoznat' COMMENT 'IP adresa ili hostName odakle dolazi akcija',\n" +

        "PRIMARY KEY  (lockID), \n" + 
        "INDEX recordAndRecID (tableName, recID)");  
   }

}

public static class VvSKYlocker_Ex
{
   public static string Create_table_definition()
   {
      return (
        "lockID             int(10) unsigned NOT NULL auto_increment,                                 \n" + // PRIMARY KEY
        "lockTS             timestamp        NOT NULL default CURRENT_TIMESTAMP,                      \n" + // CURRENT_TIMESTAMP
        "record             varchar(16)      NOT NULL default '',                                     \n" + // from SkyRule 
        "documTT            char(3)          NOT NULL default '',                                     \n" + // from SkyRule 
        "birthLoc           enum ('NONE', 'CENT', 'SHOP', 'SKY') NOT NULL,                            \n" + // from SkyRule 
        "skl1kind           enum ('NONE', 'CentGLAvps', 'CentPOVvps', 'ShopMPS', 'ShopVPS') NOT NULL, \n" + // from SkyRule 
        "skl2kind           enum ('NONE', 'CentGLAvps', 'CentPOVvps', 'ShopMPS', 'ShopVPS') NOT NULL, \n" + // from SkyRule 
        "origSrvID          int(4)  unsigned  NOT NULL DEFAULT 0,                                     \n" + // (recordov LanSrvID)
        "origRecID          int(10) unsigned  NOT NULL DEFAULT 0,                                     \n" + // (recordov LanRecID)
        "skladCD            varchar(6)        NOT NULL default '',                                    \n" + // (recordov sklCD 1) 
        "skladCD2           varchar(6)        NOT NULL default '',                                    \n" + // (recordov sklCD 2) 
        "lockAction         enum ('ADD', 'RWT', 'DEL') NOT NULL,                                      \n" + // lan WRITE action   
        "lockUID            varchar(16)       NOT NULL default '',                                    \n" + // ZXC.vvDB_User      
        "clientComputerName varchar(32)       NOT NULL default '',                                    \n" + // Environment.MachineName

        "PRIMARY KEY  (lockID), \n");  
   }

}

public static class VvUcList_Ex
{
   #region CreateTableUcList

   public static string Create_table_definition()
   {
      return (
        "recID      int(10)    unsigned NOT NULL auto_increment     COMMENT 'log entry ID-a',\n" +
        "enumName   varchar(32)         NOT NULL default 'Vv'       COMMENT 'PrgEnum: ZXC.VvSubModulEnum',\n" +
        "mDigit     char(1)             NOT NULL default ''         COMMENT 'PrgEnum: ZXC.VvModulEnum',\n" +
        "smDigit    char(1)             NOT NULL default ''         COMMENT 'PrgEnum: ZXC.VvSubModulKindEnum',\n" +
        "ucName     varchar(32)         NOT NULL default ''         COMMENT 'Imeto na uc-ot',\n" +
        "rootOnly   tinyint(1) unsigned NOT NULL default 0          COMMENT 'if true: uc is for root only',\n" +
        "okLogin1   varchar(16)         NOT NULL default ''         COMMENT 'only one of this okLogins can use it',\n" +
        "okLogin2   varchar(16)         NOT NULL default ''         COMMENT 'only one of this okLogins can use it',\n" +
        "okLogin3   varchar(16)         NOT NULL default ''         COMMENT 'only one of this okLogins can use it',\n" +
        "okLogin4   varchar(16)         NOT NULL default ''         COMMENT 'only one of this okLogins can use it',\n" +
        "stopLogin1 varchar(16)         NOT NULL default ''         COMMENT 'forbidden for this login',\n" +
        "stopLogin2 varchar(16)         NOT NULL default ''         COMMENT 'forbidden for this login',\n" +
        "stopLogin3 varchar(16)         NOT NULL default ''         COMMENT 'forbidden for this login',\n" +
        "stopLogin4 varchar(16)         NOT NULL default ''         COMMENT 'forbidden for this login',\n" +

        "PRIMARY KEY  (recID) \n");
   }

   #endregion CreateTableUcList

}

public static class VvUcList_AddNew
{
   public static uint TableVersionStatic_VEKTOR { get { return 296/*297*/; } }

   public static string AddNewVvUserControl_CommandText_VEKTOR(uint catchingVersion)
   {
      string tableName = ZXC.vvDB_ucListTableName;
      string dbName    = ZXC.VvDB_prjktDB_Name;

      string commandBeginning = "INSERT INTO " + dbName + "." + tableName + " SET ";

      switch(catchingVersion)
      {
         case   2: return commandBeginning + GetCommandEnd("PIZ_ListaObustava", "3", "5", "PIZ_ListaObustava");
         case   3: return commandBeginning + GetCommandEnd("RIZ_StanjeSklad_A", "1", "5", "RIZ_StanjeSklad_A");
         case   4: return commandBeginning + GetCommandEnd("RIZ_StanjeSklad_B", "1", "5", "RIZ_StanjeSklad_B");
                
         case   5: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PDV_URA .ToString(), "1", "5", "RIZ_PDV_URA");
         case   6: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PDV_IRA .ToString(), "1", "5", "RIZ_PDV_IRA");
         case   7: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PDV_PDV .ToString(), "1", "5", "RIZ_PDV_PDV");
         case   8: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PDV_PDVk.ToString(), "1", "5", "RIZ_PDV_PDVk");
                
         case   9: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_DUGOVANJA   .ToString(), "1", "5", "RIZ_DUGOVANJA");
         case  10: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_POTRAZIVANJA.ToString(), "1", "5", "RIZ_POTRAZIVANJA");
              
         case  11: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.KIZ_PartnerExport.ToString(), "4", "5", "KIZ_PartnerExport");
         case  12: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.FIZ_KplanExport  .ToString(), "2", "5", "FIZ_KplanExport");
         case  13: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_Kompenzacija .ToString(), "1", "5", "RIZ_Kompenzacija");
              
         case  14: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_LagerLista   .ToString(), "1", "5", "RIZ_LagerLista");
              
         case  15: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_TopDugovanja .ToString(), "1", "5", "RIZ_TopDugovanja" );
         case  16: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_TopPotraziv  .ToString(), "1", "5", "RIZ_TopPotraziv"  );
         case  17: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_DobavDospjeca.ToString(), "1", "5", "RIZ_DobavDospjeca");
         case  18: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_KupciDospjeca.ToString(), "1", "5", "RIZ_KupciDospjeca");
         case  19: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_BlgDnevnik   .ToString(), "1", "5", "RIZ_BlgDnevnik"   );
              
         case  20: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_RNP   .ToString(), "1", "2", "RNPDUC"     );
         case  21: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_RNS   .ToString(), "1", "2", "RNSDUC"     );
         case  22: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_PRJ   .ToString(), "1", "2", "PRJDUC"     );
         case  23: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_PRI_bc.ToString(), "1", "2", "PrimkaBcDUC");
         case  24: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_PPR   .ToString(), "1", "2", "PredatUProizDUC");
         case  25: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_POV   .ToString(), "1", "2", "PovratInterDUC");
              
         case  26: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.NAL_P.ToString(), "2", "2", "NalogProjektDUC");
              
         case  27: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_IFAdev.ToString(), "1", "2", "IFAdevDUC"         );
         case  28: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_UFAdev.ToString(), "1", "2", "UFAdevDUC"         );
         case  29: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_PRIdev.ToString(), "1", "2", "PrimkaDevDUC"      );
         case  30: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_NRU   .ToString(), "1", "2", "NarudzDobUvozDUC"  );
         case  31: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_NRS   .ToString(), "1", "2", "NarudzDobUslugaDUC");
              
         case  32: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.FIZ_BilancaSubKlas.ToString(), "2", "5", "RptF_BilancaSubKlas"   );
         case  33: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.FIZ_AnaSCL        .ToString(), "2", "5", "RptF_AnalitikaSKonta_L");
              
         case  34: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_RekapRN        .ToString(), "1", "5", "RIZ_RekapRN"         );
         case  35: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.FIZ_KKontaGroup_ALL.ToString(), "2", "5", "RptF_KKontaGroup_ALL");
              
         case  36: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_StanjeSklad_Kol.ToString(), "1", "5", "RIZ_StanjeSklad_Kol");
              
         case  37: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_IRM.ToString(), "1", "2", "IRM_DUC");
         case  38: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_URM.ToString(), "1", "2", "URM_DUC");
              
         case  39: return "ALTER TABLE vvektor.vvusercontrol MODIFY COLUMN mDigit CHAR(2)";
              
         case  40: return commandBeginning + GetCommandEnd(ZXC.VvModulEnum   .MODUL_OTHER        .ToString(), "10", "0", "");
         case  41: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_VIR              .ToString(), "10", "2", "VirmaniDUC");
         case  42: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_RAS              .ToString(), "10", "2", "RasterDUC");
         case  43: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_PNT              .ToString(), "10", "2", "PutNalTuzDUC");
         case  44: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_PNI              .ToString(), "10", "2", "PutNalInoDUC");
         case  45: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_PNL              .ToString(), "10", "2", "LokoVoznjaDUC");
         case  46: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_PNR              .ToString(), "10", "2", "PutRadListDUC");
         case  47: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.XIZ_P              .ToString(), "10", "3", "MixerReportUC");
         case  48: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum  .XIZ_RekapPutNalLoko.ToString(), "10", "5", "MixerReportUC");
              
         case  49: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_ZAH.ToString()              , "10", "2", "ZahtjeviDUC");
         case  50: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum  .XIZ_RekapZahtjev   .ToString(), "10", "5", "RekapZahtjev");
         case  51: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.XIZ_Z.ToString()              , "10", "3", "MixerReportUC");
              
         case  52: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_SMD.ToString()       , "10", "2", "SmdDUC");
         case  53: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.XIZ_S.ToString()       , "10", "3", "MixerReportUC");
         case  54: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum  .XIZ_RekapSMD.ToString(), "10", "5", "RekapSMD");
              
         case  55: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_EVD.ToString()       , "10", "2", "EvidencijaDUC");
         case  56: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.XIZ_E.ToString()       , "10", "3", "MixerReportUC");
         case  57: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum  .XIZ_RekapEVD.ToString(), "10", "5", "RekapEVD");
              
         case  58: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_IFAraster.ToString() , "1", "5", "RIZ_IFAraster");
              
         case  59: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_RAS_B.ToString()      , "10", "2", "RasterBDUC");
              
         case  60: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.XIZ_RekapEVN.ToString(), "10", "5", "RekapEVN");
              
         case  61: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_RVR.ToString()       , "10", "2", "RvrDUC");
         case  62: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.XIZ_R.ToString()       , "10", "3", "MixerReportUC");
         case  63: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum  .XIZ_RVR.ToString()     , "10", "5", "RVR");
              
         case  64: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_PSM.ToString()       , "1", "2", "PocetnoStanjeMPDUC");
         case  65: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_VMI.ToString()       , "1", "2", "MedjuSkladVMIuDUC");
              
         case  66: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_StanjeSklad_AP.ToString(), "1", "5", "RIZ_StanjeSklad_AP");
              
         case  67: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_IRV.ToString()           , "10", "2", "InterniRvrDUC");
         case  68: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.XIZ_IRV.ToString()           , "10", "5", "IRV");
              
         case  69: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_URM_2.ToString(), "1", "2", "URMDUC_2");
         case  70: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_KLK_2.ToString(), "1", "2", "KalkulacijaMpDUC_2");
              
         case  71: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_KPM     .ToString() , "1", "5", "RIZ_KPM");
         case  72: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_Knjizenja.ToString(), "1", "5", "RIZ_Knjizenja");
              
         case  73: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_UFM.ToString(), "1", "2", "UFMDUC");
              
         case  74: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PrometArtikla.ToString(), "1", "5", "RIZ_PrometArtikla");
              
         case  75: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_RekapFaktur_S.ToString(), "1", "5", "RIZ_RekapFaktur_S");
         case  76: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_RekapRN_S    .ToString(), "1", "5", "RIZ_RekapRN_S"    );
         case  77: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_RUC_Ira_S    .ToString(), "1", "5", "RIZ_RUC_Ira_S"    );
         case  78: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PDV_URA_2012 .ToString(), "1", "5", "RIZ_PDV_URA_2012" );
         case  79: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PDV_IRA_2012 .ToString(), "1", "5", "RIZ_PDV_IRA_2012" );
         case  80: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PDV_PDV_2012 .ToString(), "1", "5", "RIZ_PDV_PDV_2012" );
         case  81: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PDV_PDVk_2012.ToString(), "1", "5", "RIZ_PDV_PDVk_2012");
              
         case  82: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PrometArtikla_S .ToString(), "1", "5", "RIZ_RIZ_PrometArtikla_S" );
         case  83: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_LagerLista_S    .ToString(), "1", "5", "RIZ_RIZ_LagerLista_S"    );
         case  84: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_StanjeSklad_A_S .ToString(), "1", "5", "RIZ_RIZ_StanjeSklad_A_S" );
         case  85: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_StanjeSklad_AP_S.ToString(), "1", "5", "RIZ_RIZ_StanjeSklad_AP_S");
         case  86: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_StanjeSklad_B_S .ToString(), "1", "5", "RIZ_RIZ_StanjeSklad_B_S" );
              
         case  87: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.FIZ_KPI_orig        .ToString(), "2", "5", "FIZ_KPI_orig"         );
         case  88: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.FIZ_OTS_Kum         .ToString(), "2", "5", "FIZ_OTS_Kum"          );
         case  89: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_DUGOVANJA_Kum   .ToString(), "1", "5", "RIZ_DUGOVANJA_Kum"    );
         case  90: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_POTRAZIVANJA_Kum.ToString(), "1", "5", "RIZ_POTRAZIVANJA_Kum" );
              
         case  91: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_POTRAZIVANJA_S .ToString(), "1", "5", "RIZ_POTRAZIVANJA_S"  );
         case  92: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_DUGOVANJA_S    .ToString(), "1", "5", "RIZ_DUGOVANJA_S"     );
              
         case  93: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.GFI_TSI.ToString(), "2", "2", "GFI_TSI_DUC");
         case  94: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_Komparacija.ToString(), "1", "5", "RIZ_Komparacija");
              
         case  95: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PrometRazdoblja  .ToString(), "1", "5", "RIZ_PrometRazdoblja"  );
         case  96: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PrometRazdoblja_S.ToString(), "1", "5", "RIZ_PrometRazdoblja_S");
              
         case  97: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_KretanjeSklad_S   .ToString(), "1", "5", "RIZ_KretanjeSklad_S"   );
         case  98: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_StanjeSkladPoPRJ .ToString() , "1", "5", "RIZ_StanjeSklad_Prjkt ");
         case  99: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_StanjeReversa.ToString()     , "1", "5", "RIZ_StanjeSklad_Revers");

         case 100: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_KLD.ToString()  , "1", "2", "KalkulacijaMpDUC_Dev");
         case 101: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_IRM_2.ToString(), "1", "2", "IRMDUC_2"            );
         case 102: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_PNM.ToString()  , "1", "2", "PonMalDUC"           );

         case 103: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_InventurnaLista  .ToString(), "1", "5", "RIZ_InventurnaLista"  );
         case 104: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_InventurneRazlike.ToString(), "1", "5", "RIZ_InventurneRazlike");

         case 105: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_IZM.ToString(), "1", "2", "IZMDUC");
       //case 105: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_IMM.ToString(), "1", "2", "IMMDUC");

         case 106: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_KarticaKupca  .ToString(), "1", "5", "RIZ_KarticaKupca"  );
         case 107: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_KarticaKupca_S.ToString(), "1", "5", "RIZ_KarticaKupca_S");
         case 108: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_KarticaDobav  .ToString(), "1", "5", "RIZ_KarticaDobav"  );
         case 109: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_KarticaDobav_S.ToString(), "1", "5", "RIZ_KarticaDobav_S");

         case 110: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_ObrKamataKupac  .ToString(), "1", "5", "RIZ_ObrKamataKupac"  );
         case 111: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_ObrKamataKupac_S.ToString(), "1", "5", "RIZ_ObrKamataKupac_S");
         case 112: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_ObrKamataDobav  .ToString(), "1", "5", "RIZ_ObrKamataDobav"  );
         case 113: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_ObrKamataDobav_S.ToString(), "1", "5", "RIZ_ObrKamataDobav_S");

         case 114: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_KIZ.ToString(), "1", "2", "KIZDUC");
         case 115: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_PIK.ToString(), "1", "2", "PIKDUC");

         case 116: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_EXC.ToString(), "10", "2", "ExterniCjeniciDUC");

         case 117: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_IRP  .ToString(), "1", "2", "IRPDUC" );
         case 118: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_URP  .ToString(), "1", "2", "URPDUC" );
         case 119: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_PIZ_P.ToString(), "1", "2", "PIZpDUC");
         case 120: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_PRI_P.ToString(), "1", "2", "PRIpDUC");

         case 121: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_IFArasterB.ToString(), "1", "5", "RIZ_IFArasterB");
         case 122: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_PMV.ToString(), "10", "2", "PmvDUC");

         case 123: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PPMV_Prilog9.ToString(), "1", "5", "RIZ_PPMV_Prilog9");

         case 124: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_UPA.ToString(), "1", "2", "UPADUC");

         case 125: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PDV_URA_EU      .ToString(), "1", "5", "RIZ_PDV_URA_EU"      );
         case 126: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PDV_IRA_EU      .ToString(), "1", "5", "RIZ_PDV_IRA_EU"      );
         case 127: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PDV_PDV_EU      .ToString(), "1", "5", "RIZ_PDV_PDV_EU"      );
         case 128: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_ObrazacPDVS_EU  .ToString(), "1", "5", "RIZ_ObrazacPDVS_EU"  );
         case 129: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_ObrazacZP_EU    .ToString(), "1", "5", "RIZ_ObrazacZP_EU"    );
         case 130: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_ObrazacPDVS_EU_A.ToString(), "1", "5", "RIZ_ObrazacPDVS_EU_A");
         case 131: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_ObrazacZP_EU_A  .ToString(), "1", "5", "RIZ_ObrazacZP_EU_A"  );

         case 132: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_ProdajaPoDobav_S.ToString(), "1", "5", "RIZ_ProdajaPoDobav_S");
         case 133: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_ProdajaPoDobav_A.ToString(), "1", "5", "RIZ_ProdajaPoDobav_A");

         case 134: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_KKM.ToString(), "1", "2", "KKMDUC");
         case 135: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_UPM.ToString(), "1", "2", "PovratDobMalDUC");

         case 136: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_URM_D.ToString(), "1", "2", "URMDUC_Dev");

         case 137: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_Rekap_FISK_Faktur.ToString(), "1", "5", "RIZ_Rekap_FISK_Faktur");
         
         case 138: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_NRM.ToString(), "1", "2", "NRMDUC");

         case 139: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PromStSkladDobav_A.ToString(), "1", "5", "RIZ_PromStSkladDobav_A");
         case 140: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PromStSkladDobav_S.ToString(), "1", "5", "RIZ_PromStSkladDobav_S");
         case 141: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_ProdajaPoDobav_B_S.ToString(), "1", "5", "RIZ_ProdajaPoDobav_B_S");
         case 142: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_ProdajaPoDobav_B_A.ToString(), "1", "5", "RIZ_ProdajaPoDobav_B_A");

         case 143: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_Rekap_PIX.ToString(), "1", "5", "RIZ_Rekap_PIX");

         case 144: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.PLA_2014.ToString(), "3", "2", "Placa2014DUC");
         case 145: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.PLA_NP.ToString()  , "3", "2", "PlacaNPDUC");
         case 146: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.PIZ_JOPPD.ToString() , "3", "5", "PIZ_JOPPD");

         case 147: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_INM.ToString(), "1", "2", "InventuraMPDUC");

         case 148: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_BOR.ToString(), "1", "2", "BORDUC");
         case 149: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_PIM.ToString(), "1", "2", "PIMDUC");
         case 150: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_NOR.ToString(), "1", "2", "NORDUC");

         case 151: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_IZM_2.ToString(), "1", "2", "IZMDUC_2");
         case 152: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_RUC_Ira_Rtrans.ToString(), "1", "5", "RIZ_RUC_Ira_Rtrans");

         case 153: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PrometArtikla_OP_A.ToString(), "1", "5", "RIZ_PrometArtikla_OP_A");
         case 154: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PrometArtikla_OP_S.ToString(), "1", "5", "RIZ_PrometArtikla_OP_S");

         case 155: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_LagerLista_OP_A.ToString(), "1", "5", "RIZ_LagerLista_OP_A");
         case 156: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_LagerLista_OP_S.ToString(), "1", "5", "RIZ_LagerLista_OP_S");
         case 157: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_RUC_Provizija_A.ToString(), "1", "5", "RIZ_RUC_Provizija_A");
         case 158: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_RUC_Provizija_S.ToString(), "1", "5", "RIZ_RUC_Provizija_S");

         case 159: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_Rekap_PNP_Rtrans.ToString(), "1", "5", "RIZ_Rekap_PNP_Rtrans");
         case 160: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PDV_PDVk_2013.ToString()   , "1", "5", "RIZ_PDV_PDVk_2013"   );
         
         case 161: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_LagerLista_OPSkl_A.ToString(), "1", "5", "RIZ_LagerLista_OPSkl_A");
         case 162: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_LagerLista_OPSkl_S.ToString(), "1", "5", "RIZ_LagerLista_OPSkl_S");

         case 163: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.FIZ_BilancaUNeprof.ToString(), "2", "5", "FIZ_BilancaUNeprof");

         case 164: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.SKY.ToString()  , "0", "1", "SkyRuleUC");
         case 165: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.LsSKY.ToString(), "0", "4", "SkyRuleListUC");

         case 166: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_MVR.ToString(), "10", "2", "RvrMjesecDUC");
         case 167: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.PIZ_PotpisnaLista.ToString(), "3", "5", "PIZ_PotpisnaLista");

         case 168: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.XIZ_MVR.ToString(), "10", "5", "MVR");
         
         case 169: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_GST.ToString(),                 "10", "2", "KnjigaGostijuDUC");
         case 170: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum  .XIZ_GST.ToString(),               "10", "5", "GST");
         case 171: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum  .XIZ_PriBor.ToString(),            "10", "5", "XIZ_PriBor");
         case 172: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.XIZ_G.ToString(),                 "10", "3", "MixerReportUC");
         case 173: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum  .XIZ_GST_STR.ToString()          , "10", "5", "GST_STR");
         case 174: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum  .XIZ_TZNoc.ToString()            , "10", "5", "XIZ_TZNoc");
         case 175: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum  .XIZ_TZ_DolN.ToString()          , "10", "5", "XIZ_TZDolN");

         case 176: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_ObrazacPPO.ToString()         , "1", "5", "RIZ_ObrazacPPO");

         case 177: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_StanjeSklad_AMB  .ToString()  , "1", "5", "RIZ_StanjeSklad_AMB"  );
         case 178: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_StanjeSklad_AMB_S.ToString()  , "1", "5", "RIZ_StanjeSklad_AMB_S");

         case 179: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_TRI  .ToString()              , "1", "2", "TransformDUC"  );
         case 180: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_MSI_2.ToString()              , "1", "2", "MedjuSklad2DUC");
                                                                                                              
         case 181: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_MVI.ToString()                , "1", "2", "MedjuSkladMVIDUC");
         case 182: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_MMI.ToString()                , "1", "2", "MedjuSkladMMIDUC");

         case 183: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.SIN.ToString()                  , "1", "7", "SIN_UC");

         case 184: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.PIZ_KonacniObrPor.ToString()      , "3", "5", "PIZ_KonacniObrPor"      , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 185: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.PIZ_IsplatListOP .ToString()      , "3", "5", "PIZ_IsplatListOP"       , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                   
         case 186: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.PLAN.ToString()                 , "2", "2", "PlanDUC"                , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 187: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.FIZ_PlanPG_Plan1    .ToString()   , "2", "5", "FIZ_PlanPG_Plan1"       , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 188: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.FIZ_ThePln_Reali    .ToString()   , "2", "5", "FIZ_ThePln_Reali"       , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 189: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.FIZ_ThePln_Njv_Reali.ToString()   , "2", "5", "FIZ_ThePln_Njv_Reali"   , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                   
         case 190: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_BUP.ToString()                , "1", "2", "BlgUplat_M_DUC"         , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 191: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_BIS.ToString()                , "1", "2", "BlgIsplat_M_DUC"        , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                   
         case 192: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.FIZ_ThePln_Money.ToString()       , "2", "5", "FIZ_ThePln_Money"       , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                   
         case 193: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.FIZ_Pl1_ThePln_Reali.ToString()   , "2", "5", "FIZ_Pl1_ThePln_Reali"   , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 194: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.FIZ_Plan_PBN        .ToString()   , "2", "5", "FIZ_Plan_PBN"           , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                   
         case 195: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_RVR2.ToString()               ,"10", "2", "RvrDUC"                 , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                   
         case 196: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.STAT_NPF.ToString()             , "2", "2", "Statistika_NPF_DUC"     , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                   
         case 197: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_URZ.ToString()                ,"10", "2", "UrudzbeniDUC"           , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 198: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_RUG.ToString()                ,"10", "2", "UgovoriDUC"             , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                   
         case 199: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.PIZ_ObrazacIP1.ToString()         , "3", "5", "PIZ_ObrazacIP1"         , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 200: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.PIZ_ObrazacNP1.ToString()         , "3", "5", "PIZ_ObrazacNP1"         , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 201: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.XIZ_UrdzZap   .ToString()         ,"10", "5", "XIZ_UrdzZap"            , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 202: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.XIZ_RegUgv    .ToString()         ,"10", "5", "XIZ_RegUgv"             , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                   
         case 203: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_MVI_2.ToString()              , "1", "2", "MedjuSkladMVI2DUC"                                          );
                                                                                                                                                   
         case 204: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.PIZ_RAD1.ToString()               , "3", "5", "PIZ_RAD1"               , ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 205: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_RekapIRMasBlagIzvj.ToString() , "1", "5", "RIZ_RekapIRMasBlagIzvj"                                     );
                                                                                                              
         case 206: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_RGC.ToString()                , "10", "2", "RegistarCijeviDUC"     , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                                                    
         case 207: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_InventurnoStanje.ToString()   , "1", "5", "RIZ_InventurnoStanje"                                       );

         case 208: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.FIZ_KKontaGroup_ALL_Exp.ToString(), "2", "5", "FIZ_KKontaGroup_ALL_Exp"     , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                        
         case 209: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_WYR.ToString()                , "1", "2", "WYRDUC"                      , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                        
         case 210: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_InventurneRazlike_S.ToString(), "1", "5", "RIZ_InventurneRazlike_S"                                         );
         case 211: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_InventurnoStanje_S .ToString(), "1", "5", "RIZ_InventurnoStanje_S"                                          );
                                                                                                                                                        
         case 212: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_OPZSTAT1  .ToString()         , "1", "5", "RIZ_OPZSTAT1"                , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 213: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_OPZSTAT1_S.ToString()         , "1", "5", "RIZ_OPZSTAT1_S"              , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                        
         case 214: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.AIZ_ObrazacDI.ToString()          , "5", "5", "AIZ_ObrazacDI"               , ZXC.IsTEXTHOany2 ? "valentina" : "");
         // 29.02.2016:                                                                                                                                   ZXC.IsTEXTHOany je ovdje UVIJEK false jer jos nije bio InitializeVvDao 
         case 215: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_RNM.ToString()                , "1", "2", "RNMDUC"                      , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 216: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_PIP.ToString()                , "1", "2", "PIPDUC"                      , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                        
         case 217: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.LsRTR.ToString()                , "1", "4", "RtransListUC"                , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 218: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_SkladBilten.ToString()        , "1", "5", "RIZ_SkladBilten"             , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                        
         case 219: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_StanjeSklad_OP_A.ToString()   , "1", "5", "RIZ_StanjeSklad_OP_A"        , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 220: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_StanjeSklad_OP_S.ToString()   , "1", "5", "RIZ_StanjeSklad_OP_S"        , ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 221: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PrometRazdoblja_OP_A.ToString(),"1", "5", "RIZ_PrometRazdoblja_OP_A"    , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 222: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_PrometRazdoblja_OP_S.ToString(),"1", "5", "RIZ_PrometRazdoblja_OP_S"    , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                        
         case 223: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_ZAH_RNM.ToString()            ,"10", "2", "ZahtjevRNMDUC"               , ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 224: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_Rekap_IRMvsFtrans.ToString()  , "1", "5", "RIZ_Rekap_IRMvsFtrans"        , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 225: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_Rekap_IRAvsFtrans.ToString()  , "1", "5", "RIZ_Rekap_IRAvsFtrans"        , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 226: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_Rekap_RNM        .ToString()  , "1", "5", "RIZ_Rekap_RNM"                , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 227: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_Rekap_RNM_S      .ToString()  , "1", "5", "RIZ_Rekap_RNM_S"              , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 228: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_RNM_Proizvodi    .ToString()  , "1", "5", "RIZ_RNM_Proizvodi"            , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 229: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_RNM_Proizvodi_S  .ToString()  , "1", "5", "RIZ_RNM_Proizvodi_S"          , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                         
         case 230: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_ZPG              .ToString()  , "10", "2", "ZPG_DUC"                     , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 231: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_ZLJ              .ToString()  , "10", "2", "ZLJ_DUC"                     , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 232: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.XIZ_B              .ToString()  , "10", "3", "MixerReportUC"               , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 233: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.XIZ_ZLJ              .ToString()  , "10", "5", "ZLJ"                         , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 234: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.XIZ_ZOBJ             .ToString()  , "10", "5", "ZOBJ"                        , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 235: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.XIZ_ZPG              .ToString()  , "10", "5", "ZPG"                         , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 236: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.XIZ_ZUG              .ToString()  , "10", "5", "ZUG"                         , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 237: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_RNZ              .ToString()  ,  "1", "2", "RNZDUC"                      , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 238: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_Rekap_RNZ        .ToString()  ,  "1", "5", "RIZ_Rekap_RNZ"               , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 239: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_AVR              .ToString()  , "10", "2", "AvrDUC"                      , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 240: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.PIZ_RAD1_G           .ToString()  ,  "3", "5", "PIZ_RAD1_G"                  , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 241: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_BMW              .ToString()  , "10", "2", "BmwDUC"                      , ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 242: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_URA_SVD          .ToString()  ,  "1", "2", "URA_SVD_DUC"                 , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 243: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_IZD_SVD          .ToString()  ,  "1", "2", "IZD_SVD_DUC"                 , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 244: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_NRD_SVD          .ToString()  ,  "1", "2", "NRD_SVD_DUC"                 , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 245: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_UGO              .ToString()  ,  "1", "2", "UGODUC"                      , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 246: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.SVD_FinIzlazByAgr    .ToString()  ,  "1", "5", "SVD_FinIzlaz"                , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 247: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.SVD_FinIzlazByAgr_S  .ToString()  ,  "1", "5", "SVD_FinIzlaz_S"              , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 248: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.SVD_FinIzlazBySkl    .ToString()  ,  "1", "5", "SVD_FinSklIzlaz"             , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 249: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.SVD_FinIzlazBySkl_S  .ToString()  ,  "1", "5", "SVD_FinSklIzlaz_S"           , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 250: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_PNA              .ToString()  , "10", "2", "PredNrdDUC"                  , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 251: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.SVD_TopListaPartnera .ToString()  ,  "1", "5", "SVD_TopListaPartnera"        , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 252: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.SVD_PrmStSkl_ProsWeek.ToString()  ,  "1", "5", "SVD_PrmStSkl_ProsWeek"       , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                         
         case 253: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_VMI_2            .ToString()  ,  "1", "2", "MedjuSkladVMI2DUC"                                                );
                                                                                                                                                         
         case 254: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.SVD_PlanRealizUGO    .ToString()  ,  "1", "5", "SVD_PlanRealizUGO"           , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 255: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.SVD_PlanRealizUGO_S  .ToString()  ,  "1", "5", "SVD_PlanRealizUGO_S"         , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                         
         case 256: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_ObrazacPDV_MI    .ToString()  ,  "1", "5", "RIZ_ObrazacPDV_MI"           , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 257: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_ObrazacPDV_MU    .ToString()  ,  "1", "5", "RIZ_ObrazacPDV_MU"           , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 258: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_TemboWebShopExport.ToString() ,  "1", "5", "RIZ_TemboWebShopExport"      , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                         
         case 259: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.SVD_ArtikliKlinInv_Exp.ToString() ,  "1", "5", "SVD_ArtikliKlinInv_Exp"      , ZXC.IsTEXTHOany2 ? "valentina" : "");
                                                                                                                                                         
         case 260: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.SVD_URA_4Knjigovod  .ToString(),      "1", "5", "SVD_URA_4Knjigovod"         , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 261: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.SVD_URA_4Knjigovod_S.ToString(),      "1", "5", "SVD_URA_4Knjigovod_S"       , ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 262: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_Rekap_TH_DjelatRabat  .ToString(), "1", "5", "RIZ_Rekap_TH_DjelatRabat"  , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 263: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_Rekap_TH_DjelatRabat_S.ToString(), "1", "5", "RIZ_Rekap_TH_DjelatRabat_S", ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 264: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.FIZ_Dnevnik_Exp.ToString()           , "2", "5", "FIZ_Dnevnik_Exp"           , ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 265: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_Rekap_MER_STATUS.ToString()      , "1", "5", "RIZ_Rekap_MER_STATUS"      , ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 266: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_CJK.ToString()                   , "1", "2", "CjenikKupcaDUC"            , ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 267: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.SVD_NEW_4KNJ_S.ToString()            , "1", "5", "SVD_NEW_4KNJ_S"            , ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 268: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.RIZ_Jeftinije_hr_Export.ToString()   , "1", "5", "RIZ_Jeftinije_hr_Export"   , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 269: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.SVD_HALMED_Potrosnja.ToString()      , "1", "5", "SVD_HALMED_Potrosnja"      , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 270: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.SVD_HALMED_Provjera .ToString()      , "1", "5", "SVD_HALMED_Provjera"       , ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 271: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.PIZ_RekapNeoporPri.ToString()        , "3", "5", "PIZ_RekapNeoporPri"        , ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 272: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_NAK.ToString()                   , "10", "2", "NazivArtiklaZaKupcaDUC"   , ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 273: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_UGO_PTG   .ToString(), "1", "2", "UGO_PTG_DUC"    , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 274: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_ANU_PTG   .ToString(), "1", "2", "ANU_PTG_DUC"    , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 275: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_KUG_PTG   .ToString(), "1", "2", "KUG_PTG_DUC"    , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 276: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_A1_KUG_PTG.ToString(), "1", "2", "A1_KUG_PTG_DUC" , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 277: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_A1_ANU_PTG.ToString(), "1", "2", "A1_ANU_PTG_DUC" , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 278: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_DOD_PTG   .ToString(), "1", "2", "DOD_PTG_DUC"    , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 279: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_PR_DOD_PTG.ToString(), "1", "2", "PRN_DOD_PTG_DUC", ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 280: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_ZAH_SVD.ToString()   , "1", "2", "ZAH_SVD_DUC"    , ZXC.IsTEXTHOany2 ? "valentina" : "");
        
         case 281: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_KOP.ToString()       , "10", "2", "KOP_PTG_DUC"   , ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 282: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_FUG_PTG.ToString()   ,  "1", "7", "FUG_PTG_UC"     , ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 283: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.X_KDC    .ToString()   , "10", "2", "KDCDUC"         , ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 284: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.SVD_PrmArt4Nabava.ToString(), "1", "5", "SVD_PrmArt4Nabava", ZXC.IsTEXTHOany2 ? "valentina" : "");
         
         case 285: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_MOD_PTG.ToString(), "1", "2", "MOD_PTG_DUC", ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 286: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.LsRTO.ToString()    , "1", "4", "RtranOListUC", ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 287: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_PRI_PTG.ToString(), "1", "2", "PRI_PTG_DUC", ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 288: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_IZD_PTG.ToString(), "1", "2", "IZD_PTG_DUC", ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 289: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_MSI_PTG.ToString(), "1", "2", "MSI_PTG_DUC", ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 290: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_PST_PTG.ToString(), "1", "2", "PST_PTG_DUC", ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 291: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.PIZ_ObrazacIP1_v2.ToString(), "3", "5", "PIZ_ObrazacIP1_v2", ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 292: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.PLA_2024.ToString(), "3", "2", "PlacaOd2024DUC", ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 293: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_PON_MPC.ToString(), "1", "2", "PON_MPC_DUC", ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 294: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_IRA_MPC.ToString(), "1", "2", "IRA_MPC_DUC", ZXC.IsTEXTHOany2 ? "valentina" : "");

         case 295: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.FIZ_APT_K1.ToString() , "2", "5", "FIZ_APT_K1", ZXC.IsTEXTHOany2 ? "valentina" : "");
         case 296: return commandBeginning + GetCommandEnd(ZXC.VvReportEnum.PIZ_APT_K2.ToString() , "3", "5", "PIZ_APT_K2", ZXC.IsTEXTHOany2 ? "valentina" : "");

       //case 297: return commandBeginning + GetCommandEnd(ZXC.VvSubModulEnum.R_PRI_POT.ToString(), "1", "2", "POT_DUC"   , ZXC.IsTEXTHOany2 ? "valentina" : "");

         // SV DUH: rucno, kao i one prethodne, onemoguciti svaki novi UC koji njima ne treba! 

         // !!! 10.11.2016: TAMARA PAZI !!! : kada se dodaje neka UC koju zelimo da vidi centrala (a osim valentine) 
         // treba rucno iz QuerryBrowsera maknuti iz 'okLogin1' valentinu NA CENTRALI 


         // TAMARA, pazi! na novu varijantu GetCommandEnd() sa parametrom okLogin (ok primjer je case: 184) 

         //case 183: return ZXC.vvDB_ServerID > 12 ? commandBeginning + GetCommandEnd("trlababalan", "1", "2", "blabla", "valentina") :
         //                                          commandBeginning + GetCommandEnd("trlababalan", "1", "2", "blabla");

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   private static string GetCommandEnd(string enumName, string mDigit, string smDigit, string ucName)
   {
      return @"enumName = '" + enumName + @"',  mDigit = '" + mDigit + @"', smDigit = '" + smDigit + @"', ucName   = '" + ucName + @"' ";
   }

   // fuse za TEXTHO potrebe 
   private static string GetCommandEnd(string enumName, string mDigit, string smDigit, string ucName, string okLogin1)
   {
      return @"enumName = '" + enumName + @"',  mDigit = '" + mDigit + @"', smDigit = '" + smDigit + @"', ucName   = '" + ucName + @"', okLogin1   = '" + okLogin1 + @"' ";
   }

}

public static class VvLookUp_Ex
{
   #region CreateTableVvLokUp

   public static string Create_table_definition(string recordName)
   {
      return (
        "recID      int(10)       unsigned                   NOT NULL auto_increment      ,\n" +
        "theCd      varchar(32)   collate latin2_croatian_ci NOT NULL default ''          ,\n" +
        (recordName.StartsWith(ZXC.vvDB_luiPrefix + "_Dsc") ? "theName    varchar( 128)  collate latin2_croatian_ci NOT NULL default ''          ,\n" :
                                                              "theName    varchar(2048)  collate latin2_croatian_ci NOT NULL default ''          ,\n") +
        //"theName    varchar(128)  collate latin2_croatian_ci NOT NULL default ''          ,\n" +
        "theNumber  DECIMAL(12,2) collate latin2_croatian_ci NOT NULL default 0.00        ,\n" +
        "theFlag    tinyint(1)    unsigned                   NOT NULL default 0           ,\n" +
        "theInteger int(10)                                  NOT NULL default 0           ,\n" +
        "theDateT   date                                     NOT NULL default '0001-01-01',\n" +
        "theUinteger int(10)    unsigned                     NOT NULL default 0           ,\n" +
        "theString2  varchar(128) collate latin2_croatian_ci NOT NULL default ''          ,\n" +
        "theNumber2  DOUBLE(12,2)                            NOT NULL default 0.00        ,\n" +

        "PRIMARY KEY  (recID)\n"
      );
   }

   #endregion CreateTableVvLokUp

}

public static class VvRiskMacro_Ex
{
   #region CreateTable

   public static string Create_table_definition()
   {
      return (
        "RecID                      int(10) unsigned      NOT NULL auto_increment      ,\n" +
        "MacroName                  varchar(64)           NOT NULL default ''          ,\n" +
        "ReportZ                    int(3)       unsigned NOT NULL default '0'         ,\n" +
        "UseMacroDates              tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "DatumDo                    date                  NOT NULL default '0001-01-01',\n" +
        "DatumOd                    date                  NOT NULL default '0001-01-01',\n" +
        "ArtiklCdOD                 varchar(32)           NOT NULL default ''          ,\n" +   
        "ArtiklCdDO                 varchar(32)           NOT NULL default ''          ,\n" +   
        "ArtNameOD                  varchar(80)           NOT NULL default ''          ,\n" +   
        "ArtNameDO                  varchar(80)           NOT NULL default ''          ,\n" +   
        "SviArtikli                 tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "IznosOd                    decimal(10,2)         NOT NULL default '0.00'      ,\n" +   
        "IznosDo                    decimal(10,2)         NOT NULL default '0.00'      ,\n" +   
        "KD_naziv                   varchar(50)           NOT NULL default ''          ,\n" +   
        "KD_ticker                  char(6)               NOT NULL default ''          ,\n" +   
        "KD_sifra                   int(6)       unsigned NOT NULL default '0'         ,\n" +
        "TT                         char(3)               NOT NULL default ''          ,\n" +   
        "TtNumOd                    int(10)      unsigned NOT NULL default '0'         ,\n" +
        "TtNumDo                    int(10)      unsigned NOT NULL default '0'         ,\n" +
        "MT_naziv                   varchar(50)           NOT NULL default ''          ,\n" +
        "MT_ticker                  char(6)               NOT NULL default ''          ,\n" +
        "MT_sifra                   int(6)       unsigned NOT NULL default '0'         ,\n" +
        "SkladCD                    varchar(6)            NOT NULL default ''          ,\n" +
        "Napomena                   varchar(256)          NOT NULL default ''          ,\n" +
        "GrupaKupDob                char(4)               NOT NULL default ''          ,\n" +
        "NacPlac                    varchar(24)           NOT NULL default ''          ,\n" +
        "VezniDok                   varchar(32)           NOT NULL default ''          ,\n" +
        "DokNumOd                   int(10)      unsigned NOT NULL default '0'         ,\n" + 
        "DokNumDo                   int(10)      unsigned NOT NULL default '0'         ,\n" +
        "GrupiranjeDokum            varchar(32)           NOT NULL default ''          ,\n" +
        "GrupiranjeArtikla          varchar(32)           NOT NULL default ''          ,\n" +
        "AnalitSintet               char(1)               NOT NULL default ''          ,\n" +
        "AnaGrupaPoStranici         tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "VisiblePostoGrupFooter     tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "VisibleOnlyTopGroups       tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "NumOfTopGroups             tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "TopSort                    tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "PdvKnjiga                  tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "PdvKredit                  decimal(10,2)         NOT NULL default '0.00'      ,\n" +   
        "PdvPovrat                  decimal(10,2)         NOT NULL default '0.00'      ,\n" +   
        "PdvPredujam                decimal(10,2)         NOT NULL default '0.00'      ,\n" +   
        "PdvUstup                   decimal(10,2)         NOT NULL default '0.00'      ,\n" +   
        "ImaPorezZastupnika         tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "IsAutoPorezniKredit        tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "NacinPlacanja              varchar(24)           NOT NULL default ''          ,\n" +
        "PdvF_Osn                   decimal(10,2)         NOT NULL default '0.00'      ,\n" +   
        "PdvF_Pdv                   decimal(10,2)         NOT NULL default '0.00'      ,\n" +   
        "PdvIspravak                decimal(10,2)         NOT NULL default '0.00'      ,\n" +   
        "PdvObrSastavio             varchar(64)           NOT NULL default ''          ,\n" +
        "IsPrjktTel                 tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "IsPrjktFax                 tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "IsPrjktMail                tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "Tel                        varchar(64)           NOT NULL default ''          ,\n" +
        "Fax                        varchar(64)           NOT NULL default ''          ,\n" +
        "Mail                       varchar(64)           NOT NULL default ''          ,\n" +
        "IsVisibleTT                tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "IsVisibleAdress            tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "IsUserSastavio             tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "IsOtsAnalitKontre          tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "IsOtsDospjecaPoDan         tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "IsOtsKontakt               tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "IsOtsLineTipBr             tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "IsOtsDospOnly              tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "KompenzacijaBroj           varchar(64)           NOT NULL default ''          ,\n" +
        "ProjektCD                  varchar(64)           NOT NULL default ''          ,\n" +
        "OtsSaldoKompenzacijaAsText varchar(64)           NOT NULL default ''          ,\n" +
        "OtsDate                    date                  NOT NULL default '0001-01-01',\n" +
        "NeedsOTSFormular           tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "PdvR1                      tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "IsPrihodTT                 tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "OcuGraf                    tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "FuseStr1                   varchar(64)           NOT NULL default ''          ,\n" +
        "FuseStr2                   varchar(64)           NOT NULL default ''          ,\n" +
        "FuseStr3                   varchar(64)           NOT NULL default ''          ,\n" +
        "FuseStr4                   varchar(64)           NOT NULL default ''          ,\n" +
        "FuseBool1                  tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "FuseBool2                  tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "FuseBool3                  tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "FuseBool4                  tinyint(1)   unsigned NOT NULL default '0'         ,\n" +
        "FuseDeciml1                decimal(10,2)         NOT NULL default '0.00'      ,\n" +   
        "FuseDeciml2                decimal(10,2)         NOT NULL default '0.00'      ,\n" +   
        "FuseDeciml3                decimal(10,2)         NOT NULL default '0.00'      ,\n" +   
        "FuseDeciml4                decimal(10,2)         NOT NULL default '0.00'      ,\n" +
        "Date2                      date                  NOT NULL default '0001-01-01',\n" +
        "Date3                      date                  NOT NULL default '0001-01-01',\n" +

        "PRIMARY KEY       (recID), \n" +
        "INDEX   macroName (macroName)");
   }

   #endregion CreateTable

}

