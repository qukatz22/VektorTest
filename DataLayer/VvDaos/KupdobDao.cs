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

public sealed class KupdobDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static KupdobDao instance;

   private KupdobDao(XSqlConnection conn, string dbName) : base(dbName, Kupdob.recordNameArhiva, conn)// nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static KupdobDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if (instance == null)
      {
         instance = new KupdobDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableKupdob

   public static   uint TableVersionStatic { get { return /*!!!*/ 40 /*!!!*/; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition(bool needsPrjktExtensions, bool isArhiva)
   {
      return (
         /* 00 */  "recID int(10) unsigned NOT NULL auto_increment,\n" +
         /* 01 */  "addTS timestamp                 NULL DEFAULT NULL,\n" +
         /* 02 */  "modTS timestamp                 default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,\n" +
         /* 03 */  "addUID varchar(16)     NOT NULL default 'XY',\n" +
         /* 04 */  "modUID varchar(16)     NOT NULL default '',\n" +
        CreateTable_LanSrvID_And_LanRecID_Columns +
         /* 05 */  "naziv     varchar(50)  NOT NULL default '',\n" +
         /* 06 */  "ticker       char(6)   NOT NULL default '',\n" +
         /* 07 */"kupdobCD int(6) unsigned NOT NULL default '0',\n" +
         /* 08 */  "matbr     varchar(13)  NOT NULL default '',\n" +
         /* 09 */  "tip          char(4)   NOT NULL default '',\n" +
         /* 10 */  "dugoIme   varchar(64)  NOT NULL default '',\n" +
         /* 11 */  "ulica1    varchar(32)  NOT NULL default '' COMMENT 'adr sjedista',\n" +
         /* 12 */  "ulica2    varchar(32)  NOT NULL default '' COMMENT 'adr fakturiranja',\n" +
         /* 13 */  "grad      varchar(32)  NOT NULL default '',\n" +
         /* 14 */  "postaBr   varchar(16)  NOT NULL default '',\n" +
         /* 15 */  "opcina    varchar(24)  NOT NULL default '',\n" +
         /* 16 */  "opcCd        char(4)   NOT NULL default '',\n" +
         /* 17 */  "zupan     varchar(24)  NOT NULL default '',\n" +
         /* 18 */  "zupCd        char(4)   NOT NULL default '',\n" +
         /* 19 */  "ime       varchar(24)  NOT NULL default '',\n" +
         /* 20 */  "prezime   varchar(24)  NOT NULL default '',\n" +
         /* 21 */  "tel1      varchar(32)  NOT NULL default '',\n" +
         /* 22 */  "tel2      varchar(32)  NOT NULL default '',\n" +
         /* 23 */  "fax       varchar(32)  NOT NULL default '',\n" +
         /* 24 */  "gsm       varchar(32)  NOT NULL default '',\n" +
         /* 25 */  "email     varchar(40)  NOT NULL default '',\n" +
         /* 26 */  "url       varchar(32)  NOT NULL default '',\n" +
         /* 27 */  "ziro1     varchar(24)  NOT NULL default '',\n" +
         /* 28 */  "ziro1By   varchar(32)  NOT NULL default '',\n" +
         /* 29 */  "ziro1PnbM    char(2)   NOT NULL default '',\n" +
         /* 30 */  "ziro1PnbV varchar(24)  NOT NULL default '',\n" +
         /* 31 */  "ziro2     varchar(24)  NOT NULL default '',\n" +
         /* 32 */  "ziro2By   varchar(32)  NOT NULL default '',\n" +
         /* 33 */  "ziro2PnbM    char(2)   NOT NULL default '',\n" +
         /* 34 */  "ziro2PnbV varchar(24)  NOT NULL default '',\n" +
         /* 35 */  "ziro3     varchar(24)  NOT NULL default '',\n" +
         /* 36 */  "ziro3By   varchar(32)  NOT NULL default '',\n" +
         /* 37 */  "ziro3PnbM    char(2)   NOT NULL default '',\n" +
         /* 38 */  "ziro3PnbV varchar(24)  NOT NULL default '',\n" +
         /* 39 */  "ziro4     varchar(24)  NOT NULL default '',\n" +
         /* 40 */  "ziro4By   varchar(32)  NOT NULL default '',\n" +
         /* 41 */  "ziro4PnbM    char(2)   NOT NULL default '',\n" +
         /* 42 */  "ziro4PnbV varchar(24)  NOT NULL default '',\n" +
         /* 43 */  "kontoDug  varchar(6)   NOT NULL default '',\n" +
         /* 44 */  "regob     varchar(10)  NOT NULL default '',\n" +
         /* 45 */  "sifDcd    varchar(8)   NOT NULL default '',\n" +
         /* 46 */  "sifDname  varchar(128) NOT NULL default '',\n" +
         /* 47 */  "date      date         NOT NULL default '0001-01-01' COMMENT 'some fuse date',\n" +
         /* 48 */  "putnikID      int(10)  unsigned NOT NULL default 0,\n" +
         /* 49 */  "putName   varchar(24)  NOT NULL default '',\n" +
         /* 50 */  "fuse1     varchar(24)  NOT NULL default '' COMMENT 'some fuse1',\n" +
         /* 51 */  "fuse2     varchar(32)  NOT NULL default '' COMMENT 'some fuse2',\n" +
         /* 52 */  "napom1    varchar(64)  NOT NULL default '',\n" +
         /* 53 */  "napom2    varchar(64)  NOT NULL default '',\n" +
         /* 54 */  "komentar varchar(4096) NOT NULL default '',\n" +
         /* 55 */  "isObrt    tinyint(1)   unsigned NOT NULL default 0,\n" +
         /* 56 */  "isFrgn    tinyint(1)   unsigned NOT NULL default 0,\n" +
         /* 57 */  "isPdv     tinyint(1)   unsigned NOT NULL default 0,\n" +
         /* 58 */  "isXxx     tinyint(1)   unsigned NOT NULL default 0,\n" +
         /* 59 */  "isYyy     tinyint(1)   unsigned NOT NULL default 0,\n" +
         /* 60 */  "isZzz     tinyint(1)   unsigned NOT NULL default 0,\n" +
         /* 61 */  "stRbt1    DECIMAL(10,4)         NOT NULL DEFAULT 0,\n" +
         /* 62 */  "stRbt2    DECIMAL(10,4)         NOT NULL DEFAULT 0,\n" +
         /* 63 */  "stSRbt    DECIMAL(10,4)         NOT NULL DEFAULT 0,\n" +
         /* 64 */  "stCsSc    DECIMAL(10,4)         NOT NULL DEFAULT 0,\n" +
         /* 65 */  "stProviz  DECIMAL(10,4)         NOT NULL DEFAULT 0,\n" +
         /* 66 */  "pnbMProv     char(2)            NOT NULL default '',\n" +
         /* 67 */  "pnbVProv  varchar(24)           NOT NULL default '',\n" +
         /* 68 */  "pnbMPlaca    char(2)            NOT NULL default '',\n" +
         /* 69 */  "pnbVPlaca varchar(24)           NOT NULL default '',\n" +
         /* 70 */  "valutaPl smallint(3)            NOT NULL default 0,\n" +
         /* 71 */  "rokOtprm smallint(3)            NOT NULL default 0,\n" +
         /* 72 */  "isCentr   tinyint(1)   unsigned NOT NULL default 0,\n" +
         /* 73 */  "centrID       int(10)  unsigned NOT NULL default 0,\n" +
         /* 74 */  "centrTick char(6)               NOT NULL default '',\n" +
         /* 75 */  "kontoPot  varchar(6)            NOT NULL default '',\n" +
         /* 76 */  "isMtr     tinyint(1)   unsigned NOT NULL default 0,\n" +
         /* 77 */  "isKupac   tinyint(1)   unsigned NOT NULL default 0,\n" +
         /* 78 */  "isDobav   tinyint(1)   unsigned NOT NULL default 0,\n" +
         /* 79 */  "isBanka   tinyint(1)   unsigned NOT NULL default 0,\n" +
         /* 80 */  "oib       varchar(12)           NOT NULL default '',\n" +
         /* 81 */  "drzava    varchar(32)           NOT NULL default '',\n" +
         /* 82 */  "swift     varchar(64)           NOT NULL default '',\n" +
         /* 83 */  "iban      varchar(64)           NOT NULL default '',\n" +
         /* 84 */  "devName   char(3)               NOT NULL default '',\n" +
         /* 85 */  "finLimit  DECIMAL(12,2)         NOT NULL DEFAULT 0 ,\n" +
         /* 86 */  "ugovorNo  varchar(32)           NOT NULL default '',\n" +
         /* 87 */  "komisija  tinyint(1)   unsigned NOT NULL default 0 ,\n" +
         /* 88 */  "sklKonto  varchar(16)           NOT NULL default '',\n" +
         /* 89 */  "sklNum        int(10)  unsigned NOT NULL default 0 ,\n" +
         /* 90 */  "vatCntryCode  char(2)               NOT NULL default '',\n" +
         /* 91 */  "mitoIzn   DECIMAL(12,2)         NOT NULL DEFAULT 0 ,\n" +
         /* 92 */  "mitoSt    DECIMAL(12,2)         NOT NULL DEFAULT 0 ,\n" +
         /* 93 */  "investTr  DECIMAL(12,2)         NOT NULL DEFAULT 0 ,\n" +
         /*94+2*/  "trecaStr  DECIMAL(12,2)         NOT NULL DEFAULT 0 ,\n" +

         /* 95 */  "timeOd_1  time                  NOT NULL default '00:00:00',\n" +
         /* 95 */  "timeDo_1  time                  NOT NULL default '00:00:00',\n" +
         /* 95 */  "timeOd_2  time                  NOT NULL default '00:00:00',\n" +
         /* 95 */  "timeDo_2  time                  NOT NULL default '00:00:00',\n" +
         /* 95 */  "timeOd_3  time                  NOT NULL default '00:00:00',\n" +
         /* 95 */  "timeDo_3  time                  NOT NULL default '00:00:00',\n" +
         /* 95 */  "timeOd_4  time                  NOT NULL default '00:00:00',\n" +
         /* 95 */  "timeDo_4  time                  NOT NULL default '00:00:00',\n" +
         /* 95 */  "timeOd_5  time                  NOT NULL default '00:00:00',\n" +
         /* 95 */  "timeDo_5  time                  NOT NULL default '00:00:00',\n" +
         /* 95 */  "timeOd_6  time                  NOT NULL default '00:00:00',\n" +
         /* 95 */  "timeDo_6  time                  NOT NULL default '00:00:00',\n" +
         /* 95 */  "timeOd_7  time                  NOT NULL default '00:00:00',\n" +
         /* 95 */  "timeDo_7  time                  NOT NULL default '00:00:00',\n" +

         /*109 */  "r1Kind       tinyint(1) unsigned NOT NULL default 0,\n" +
         /*110 */  "idIsPolStmnt tinyint(1) unsigned NOT NULL default 0,\n"            +
         /*111 */  "idBirthDate  date                NOT NULL default '0001-01-01',\n" +
         /*112 */  "idExpDate    date                NOT NULL default '0001-01-01',\n" +
         /*113 */  "idNumber     varchar(64)         NOT NULL default '',\n"           +
         /*114 */  "idIssuer     varchar(64)         NOT NULL default '',\n"           +
         /*115 */  "idCitizenshp varchar(64)         NOT NULL default '',\n"           +

         (needsPrjktExtensions ? PrjktDao.Create_table_PrjktExtensions_definition() : "") +

         CreateTable_ArhivaExtensionDefinition(isArhiva) +

         "PRIMARY KEY (recID) ,\n" +
         "KEY BY_NAZIV (naziv),\n" +
        (isArhiva ? "" : "UNIQUE ") + "KEY BY_CODE (kupdobCD)\n" +

          (isArhiva ? ", KEY BYqOrigRecID (origRecID)\n" : "")

      );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isPrjkt, bool isArhiva)
   {
      string tableName;

      if(isPrjkt) // Prjkt table 
      {
         if(isArhiva) tableName = Prjkt.recordNameArhiva;
         else         tableName = Prjkt.recordName;
      }
      else // Kupdob table 
      {
         if(isArhiva) tableName = Kupdob.recordNameArhiva;
         else         tableName = Kupdob.recordName;
      }

      string dbName = VvSQL.GetDbNameForThisTableName(tableName);

      // Just few remarcks: 
      // 1. Ovo gore table name i dbName ti nece trebati za druge tablice. Ovdje je to specificno zbog Kupdob--->Prjkt inheritance-a 
      //    a i inace ti tableName i dbName trebaju samo zbog UPDATE clauzule a ne i za 'obicni'ADD COLUMN (UPDATE ti je isao da popunis novododanu kolonu kupdobCD sa unikatnim prjktKupdobCD vrijednostima) 
      // 2. Ovu pretumbaciju sa dodavanjem kupdobCD-a si morao u dvije faze (ver. 2, pa ver. 3) jer nemozes imati UNIQUE index dok je kupdobCD prazan.

      switch(catchingVersion)
      {  
         case 2: return ("ADD COLUMN kupdobCD  int(6)  unsigned NOT NULL default '0' AFTER ticker;\n" +
                         "UPDATE " + dbName + "." + tableName + " SET kupdobCD = recID");

         case 3: return ("ADD " + (isArhiva ? "" : "UNIQUE ") + "INDEX BY_CODE (kupdobCD)\n");

         case 4: return ("ADD COLUMN oib       varchar(11)           NOT NULL default '' AFTER isBanka;\n");

         case 5: return ("ADD COLUMN drzava    varchar(32)           NOT NULL default '' AFTER oib,    " +
                         "ADD COLUMN swift     varchar(64)           NOT NULL default '' AFTER drzava, " +
                         "ADD COLUMN iban      varchar(64)           NOT NULL default '' AFTER swift,  " +
                         "ADD COLUMN devName   char(3)               NOT NULL default '' AFTER iban,   " +
                         "ADD COLUMN finLimit  DECIMAL(12,2)         NOT NULL DEFAULT 0  AFTER devName;\n");

         case 6: if(isPrjkt == false) return "";
            return ("ADD memoHeader varchar(2048)       NOT NULL default '' AFTER isTrgRs,     " +
                    "ADD memoFooter varchar(2048)       NOT NULL default '' AFTER memoHeader,  " +
                    "ADD theLogo    MEDIUMBLOB                              AFTER memoFooter;\n");
         case 7: if(isPrjkt == false) return "";
            return ("ADD belowGrid  varchar(2048)       NOT NULL default '' AFTER theLogo;\n");

         case 8: return ("MODIFY COLUMN sifDname  varchar(128) NOT NULL default '';");
         //NotaBene!!! Kada opet trebas promjenu samo u Prjkt-u a ne i u Kupdob-u onda ne zaboravi, osim ovoga 'if(isPrjkt == false) return "";' i u VvSql.cs.ALTER_TABLE_ForCatchUp_Command... 'commaOrNot' 

         case 9: if(isPrjkt == false) return "";
            return ("ADD isNoMinus  tinyint(1) unsigned NOT NULL default 0 COMMENT 'ne dozvoli minus' AFTER belowGrid,     " +
                    "ADD porIspost  varchar(32)         NOT NULL default ''                           AFTER isNoMinus,  " +
                    "ADD isChkPrKol tinyint(1) unsigned NOT NULL default 0                            AFTER porIspost;\n");

         case 10: return ("MODIFY COLUMN naziv varchar(64) NOT NULL default '';");

         case 11: return ("ADD COLUMN ugovorNo varchar(32)           NOT NULL default '' AFTER finLimit;\n");

         case 12: if(isPrjkt == false) return ""; // PAZI !!!: Ne zaboravi u VvSql.ALTER_TABLE_ForCatchUp_Command srediti If() za commaOrNot !!! 
            return ("ADD rvrOd      datetime            NOT NULL default '0001-01-01 00:00:00' COMMENT 'Radno Vrijeme OD' AFTER isChkPrKol,  " +
                    "ADD rvrDo      datetime            NOT NULL default '0001-01-01 00:00:00' COMMENT 'Radno Vrijeme DO' AFTER rvrOd     ;\n");

         case 13: if(isPrjkt == false) return ""; // PAZI !!!: Ne zaboravi u VvSql.ALTER_TABLE_ForCatchUp_Command srediti If() za commaOrNot !!! 
            return ("ADD isFiskalOnline tinyint(1) unsigned NOT NULL default 0 AFTER rvrDo     ;\n");

         case 14: if(isPrjkt == false) return ""; // PAZI !!!: Ne zaboravi u VvSql.ALTER_TABLE_ForCatchUp_Command srediti If() za commaOrNot !!! 
            return ("ADD certFile   MEDIUMBLOB                              AFTER isFiskalOnline,  " +
                    "ADD certPasswd varchar(32)         NOT NULL default '' AFTER certFile     ;\n");

         case 15: if(isPrjkt == false) return ""; // PAZI !!!: Ne zaboravi u VvSql.ALTER_TABLE_ForCatchUp_Command srediti If() za commaOrNot !!! 
            return ("ADD isNoTtNumChk tinyint(1) unsigned NOT NULL default 0 AFTER certPasswd     ;\n");

         case 16: return ("ADD COLUMN komisija  tinyint(1)   unsigned NOT NULL default 0  AFTER ugovorNo, " +
                          "ADD COLUMN sklKonto  varchar(16)           NOT NULL default '' AFTER komisija, " +
                          "ADD COLUMN sklNum        int(10)  unsigned NOT NULL default 0  AFTER sklKonto;\n");

         case 17: if(isPrjkt == false) return ""; // PAZI !!!: Ne zaboravi u VvSql.ALTER_TABLE_ForCatchUp_Command srediti If() za commaOrNot !!! 
            return ("ADD isFiskCashOnly tinyint(1) unsigned NOT NULL default 0  AFTER isNoTtNumChk  ,  " +
                    "ADD fiskTtOnly     char(3)             NOT NULL default '' AFTER isFiskCashOnly; \n");

         case 18: return (isArhiva ? "ADD INDEX BYqOrigRecID (origRecID)\n" : "skip_me");

         case 19: return ("ADD COLUMN vatCntryCode  char(2)               NOT NULL default '' AFTER sklNum,  " +
                          "MODIFY COLUMN oib      varchar(12)           NOT NULL default ''             ;\n");

         case 20: return ("MODIFY COLUMN komentar varchar(4096) NOT NULL default ''                     ;\n");

         case 21: if(isPrjkt == false) return ""; // PAZI !!!: Ne zaboravi u VvSql.ALTER_TABLE_ForCatchUp_Command srediti If() za commaOrNot !!! 
            return ("ADD isNeprofit tinyint(1) unsigned NOT NULL default 0  AFTER fiskTtOnly  ; \n");

         case 22: return ("ADD COLUMN mitoIzn   DECIMAL(12,2)         NOT NULL DEFAULT 0 AFTER vatCntryCode, " +
                          "ADD COLUMN mitoSt    DECIMAL(12,2)         NOT NULL DEFAULT 0 AFTER mitoIzn, " +
                          "ADD COLUMN investTr  DECIMAL(12,2)         NOT NULL DEFAULT 0 AFTER mitoSt, " +
                          "ADD COLUMN trecaStr  DECIMAL(12,2)         NOT NULL DEFAULT 0 AFTER investTr;\n");

         case 23: if(isPrjkt == false) return ""; // PAZI !!!: Ne zaboravi u VvSql.ALTER_TABLE_ForCatchUp_Command srediti If() za commaOrNot !!! 
                  return ("ADD COLUMN skySrvrHost    varchar(32)         NOT NULL default '' AFTER isNeprofit, " +
                          "ADD COLUMN skyPassword    varchar(32)         NOT NULL default '' AFTER skySrvrHost, " +
                          "ADD COLUMN skyVvDomena    varchar(6)          NOT NULL default '' AFTER skyPassword;\n");

         case 24: return AlterTable_LanSrvID_And_LanRecID_Columns;

         case 25: if(isPrjkt == false) return ""; // PAZI !!!: Ne zaboravi u VvSql.ALTER_TABLE_ForCatchUp_Command srediti If() za commaOrNot !!! 
            return ("ADD vrKoefBr1      decimal(12,2)       NOT NULL default '0.0'  AFTER skyVvDomena; \n");

         case 26: if(isPrjkt == false) return ""; // PAZI !!!: Ne zaboravi u VvSql.ALTER_TABLE_ForCatchUp_Command srediti If() za commaOrNot !!! 
                  return ("ADD COLUMN stStz2029      decimal(4,2)        NOT NULL default '0.0' AFTER vrKoefBr1      , " +
                          "ADD COLUMN stStz3034      decimal(4,2)        NOT NULL default '0.0' AFTER stStz2029      , " +
                          "ADD COLUMN stStz3500      decimal(4,2)        NOT NULL default '0.0' AFTER stStz3034      , " +
                          "ADD COLUMN isObustOver3   tinyint(1) unsigned NOT NULL default 0     AFTER stStz3500      , " +
                          "ADD COLUMN isCheckStaz    tinyint(1) unsigned NOT NULL default 0     AFTER isObustOver3   , " +
                          "ADD COLUMN isObrStazaLast tinyint(1) unsigned NOT NULL default 0     AFTER isCheckStaz    , " +
                          "ADD COLUMN isSkipStzOnBol tinyint(1) unsigned NOT NULL default 0     AFTER isObrStazaLast , " +
                          "ADD COLUMN isFullStzOnPol tinyint(1) unsigned NOT NULL default 0     AFTER isSkipStzOnBol ;\n");

         case 27: if(isPrjkt == false) return ""; // PAZI !!!: Ne zaboravi u VvSql.ALTER_TABLE_ForCatchUp_Command srediti If() za commaOrNot !!! 
                  return ("DROP COLUMN stStz2029 , " +
                          "DROP COLUMN stStz3034 , " +
                          "DROP COLUMN stStz3500 ;\n");

         case 28: if(isPrjkt == false) return "";
                  return ("ADD theLogo2 MEDIUMBLOB AFTER isFullStzOnPol;\n");

         case 29: if(isPrjkt == false) return ""; // PAZI !!!: Ne zaboravi u VvSql.ALTER_TABLE_ForCatchUp_Command srediti If() za commaOrNot !!! 
                  return ("MODIFY COLUMN skyPassword    varchar(128)        NOT NULL default ''                     ;\n");

         case 30: if(isPrjkt == false) return ""; // PAZI !!!: Ne zaboravi u VvSql.ALTER_TABLE_ForCatchUp_Command srediti If() za commaOrNot !!! 
                  return ("ADD rnoRkp         varchar(32)         NOT NULL default '' AFTER theLogo2     ;\n");

         case 31: if(isPrjkt == false) return ""; // PAZI !!!: Ne zaboravi u VvSql.ALTER_TABLE_ForCatchUp_Command srediti If() za commaOrNot !!! 
                  return ("ADD shouldPeriodLock tinyint(1) unsigned NOT NULL default 0 AFTER rnoRkp,          " +
                          "ADD periodLockDay    tinyint(2) unsigned NOT NULL default 0 AFTER shouldPeriodLock;\n");

         case 32: if(isPrjkt == false) return ""; // PAZI !!!: Ne zaboravi u VvSql.ALTER_TABLE_ForCatchUp_Command srediti If() za commaOrNot !!! 
                  return ("ADD isBtchBookg tinyint(1) unsigned NOT NULL default 0  AFTER periodLockDay; \n");

         case 33: return ("ADD COLUMN timeOd_1 time NOT NULL default '00:00:00' AFTER trecaStr,\n" +
                          "ADD COLUMN timeDo_1 time NOT NULL default '00:00:00' AFTER timeOd_1,\n" +
                          "ADD COLUMN timeOd_2 time NOT NULL default '00:00:00' AFTER timeDo_1,\n" +
                          "ADD COLUMN timeDo_2 time NOT NULL default '00:00:00' AFTER timeOd_2,\n" +
                          "ADD COLUMN timeOd_3 time NOT NULL default '00:00:00' AFTER timeDo_2,\n" +
                          "ADD COLUMN timeDo_3 time NOT NULL default '00:00:00' AFTER timeOd_3,\n" +
                          "ADD COLUMN timeOd_4 time NOT NULL default '00:00:00' AFTER timeDo_3,\n" +
                          "ADD COLUMN timeDo_4 time NOT NULL default '00:00:00' AFTER timeOd_4,\n" +
                          "ADD COLUMN timeOd_5 time NOT NULL default '00:00:00' AFTER timeDo_4,\n" +
                          "ADD COLUMN timeDo_5 time NOT NULL default '00:00:00' AFTER timeOd_5,\n" +
                          "ADD COLUMN timeOd_6 time NOT NULL default '00:00:00' AFTER timeDo_5,\n" +
                          "ADD COLUMN timeDo_6 time NOT NULL default '00:00:00' AFTER timeOd_6,\n" +
                          "ADD COLUMN timeOd_7 time NOT NULL default '00:00:00' AFTER timeDo_6,\n" +
                          "ADD COLUMN timeDo_7 time NOT NULL default '00:00:00' AFTER timeOd_7;\n" );

         case 34: if(isPrjkt == false) return "";
                  return ("ADD memoFooter2 varchar(2048)       NOT NULL default '' AFTER isBtchBookg;\n");

         case 35: if(isPrjkt == false) return "";
                  return ("ADD isNoAutoFiskal tinyint(1) unsigned NOT NULL default 0  AFTER memoFooter2;\n");

         case 36: if(isPrjkt == false) return "";
                  return ("ADD COLUMN m2pShaSec        varchar(128)           NOT NULL default '' AFTER isNoAutoFiskal, "   +
                          "ADD COLUMN m2pApikey        varchar(128)           NOT NULL default '' AFTER m2pShaSec     , "   +
                          "ADD COLUMN m2pSerno         varchar(16)           NOT NULL default '' AFTER m2pApikey     , "   +
                          "ADD COLUMN m2pModel         varchar(16)           NOT NULL default '' AFTER m2pSerno      ; \n");

         case 37: if(isPrjkt == false) return "";
                  return ("ADD f2_Provider tinyint(1) unsigned NOT NULL default 0  AFTER m2pModel;\n");

         case 38: return ("ADD COLUMN isAMS         tinyint(1) unsigned NOT NULL default 0           AFTER timeDo_7    ,\n" +
                          "ADD COLUMN idIsPolStmnt  tinyint(1) unsigned NOT NULL default 0           AFTER isAMS       ,\n" +
                          "ADD COLUMN idBirthDate   date                NOT NULL default '0001-01-01'AFTER idIsPolStmnt,\n" +
                          "ADD COLUMN idExpDate     date                NOT NULL default '0001-01-01'AFTER idBirthDate ,\n" +
                          "ADD COLUMN idNumber      varchar(64)         NOT NULL default ''          AFTER idExpDate   ,\n" +
                          "ADD COLUMN idIssuer      varchar(64)         NOT NULL default ''          AFTER idNumber    ,\n" +
                          "ADD COLUMN idCitizenshp  varchar(64)         NOT NULL default ''          AFTER idIssuer    ;\n");

         case 39: if(isPrjkt == false) return "";
                  return ("ADD f2_RolaKind tinyint(1) unsigned NOT NULL default 0  AFTER f2_Provider;\n");

         case 40: return ("CHANGE COLUMN isAMS r1Kind tinyint(1) unsigned NOT NULL default 0;\n");

         // !!! PAZI NA 'commaOrNot' u ALTER_TABLE_ForCatchUp_Command !!! 

         default: throw new Exception("For table " + Kupdob.recordName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableKupdob

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      Kupdob kupdob = (Kupdob)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, kupdob.RecID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, kupdob.AddTS,    TheSchemaTable.Rows[CI.addTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, kupdob.ModTS,    TheSchemaTable.Rows[CI.modTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, kupdob.AddUID,   TheSchemaTable.Rows[CI.addUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, kupdob.ModUID,   TheSchemaTable.Rows[CI.modUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, kupdob.LanSrvID, TheSchemaTable.Rows[CI.lanSrvID]);
         VvSQL.CreateCommandParameter(cmd, preffix, kupdob.LanRecID, TheSchemaTable.Rows[CI.lanRecID]);

      /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Naziv,     TheSchemaTable.Rows[CI.naziv]);
      /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ticker,    TheSchemaTable.Rows[CI.ticker]);
      /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.KupdobCD,  TheSchemaTable.Rows[CI.kupdobCD]);
      /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Matbr,     TheSchemaTable.Rows[CI.matbr]);
      /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Tip,       TheSchemaTable.Rows[CI.tip]);
      /* 00 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.DugoIme,   TheSchemaTable.Rows[CI.dugoIme]);  
      /* 11 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ulica1,    TheSchemaTable.Rows[CI.ulica1]);   
      /* 12 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ulica2,    TheSchemaTable.Rows[CI.ulica2]);   
      /* 13 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Grad,      TheSchemaTable.Rows[CI.grad]);
      /* 14 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.PostaBr,   TheSchemaTable.Rows[CI.postaBr]);
      /* 15 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Opcina,    TheSchemaTable.Rows[CI.opcina]);   
      /* 16 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.OpcCd,     TheSchemaTable.Rows[CI.opcCd]);    
      /* 17 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Zupan,     TheSchemaTable.Rows[CI.zupan]);    
      /* 18 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.ZupCd,     TheSchemaTable.Rows[CI.zupCd]);    
      /* 19 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ime,       TheSchemaTable.Rows[CI.ime]);      
      /* 10 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Prezime,   TheSchemaTable.Rows[CI.prezime]);  
      /* 21 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Tel1,      TheSchemaTable.Rows[CI.tel1]);     
      /* 22 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Tel2,      TheSchemaTable.Rows[CI.tel2]);     
      /* 23 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Fax,       TheSchemaTable.Rows[CI.fax]);      
      /* 24 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Gsm,       TheSchemaTable.Rows[CI.gsm]);      
      /* 25 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Email,     TheSchemaTable.Rows[CI.email]);    
      /* 26 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Url,       TheSchemaTable.Rows[CI.url]);      
      /* 27 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ziro1,     TheSchemaTable.Rows[CI.ziro1]);
      /* 28 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ziro1By,   TheSchemaTable.Rows[CI.ziro1By]);  
      /* 29 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ziro1PnbM, TheSchemaTable.Rows[CI.ziro1PnbM]);
      /* 20 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ziro1PnbV, TheSchemaTable.Rows[CI.ziro1PnbV]);
      /* 31 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ziro2,     TheSchemaTable.Rows[CI.ziro2]);    
      /* 32 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ziro2By,   TheSchemaTable.Rows[CI.ziro2By]);  
      /* 33 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ziro2PnbM, TheSchemaTable.Rows[CI.ziro2PnbM]);
      /* 34 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ziro2PnbV, TheSchemaTable.Rows[CI.ziro2PnbV]);
      /* 35 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ziro3,     TheSchemaTable.Rows[CI.ziro3]);    
      /* 36 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ziro3By,   TheSchemaTable.Rows[CI.ziro3By]);  
      /* 37 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ziro3PnbM, TheSchemaTable.Rows[CI.ziro3PnbM]);
      /* 38 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ziro3PnbV, TheSchemaTable.Rows[CI.ziro3PnbV]);
      /* 39 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ziro4,     TheSchemaTable.Rows[CI.ziro4]);    
      /* 30 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ziro4By,   TheSchemaTable.Rows[CI.ziro4By]);  
      /* 41 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ziro4PnbM, TheSchemaTable.Rows[CI.ziro4PnbM]);
      /* 42 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Ziro4PnbV, TheSchemaTable.Rows[CI.ziro4PnbV]);
      /* 43 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.KontoDug,  TheSchemaTable.Rows[CI.kontoDug]);    
      /* 44 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Regob,     TheSchemaTable.Rows[CI.regob]);    
      /* 45 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.SifDcd,    TheSchemaTable.Rows[CI.sifDcd]);   
      /* 46 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.SifDname,  TheSchemaTable.Rows[CI.sifDname]); 
      /* 47 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Date,      TheSchemaTable.Rows[CI.date]);
      /* 48 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.PutnikID,  TheSchemaTable.Rows[CI.putnikID]); 
      /* 49 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.PutName,   TheSchemaTable.Rows[CI.putName]);  
      /* 40 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Fuse1,     TheSchemaTable.Rows[CI.fuse1]);    
      /* 51 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Fuse2,     TheSchemaTable.Rows[CI.fuse2]);    
      /* 52 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Napom1,    TheSchemaTable.Rows[CI.napom1]);   
      /* 53 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Napom2,    TheSchemaTable.Rows[CI.napom2]);   
      /* 54 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Komentar,  TheSchemaTable.Rows[CI.komentar]); 
      /* 55 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob./*IsObrt*/PdvRTip,    TheSchemaTable.Rows[CI.isObrt]);   
      /* 56 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.IsFrgn,    TheSchemaTable.Rows[CI.isFrgn]);   
      /* 57 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.IsPdv,     TheSchemaTable.Rows[CI.isPdv]);    
      /* 58 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.IsDevizno, TheSchemaTable.Rows[CI.isXxx]);    
      /* 59 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.IsYyy,     TheSchemaTable.Rows[CI.isYyy]);    
      /* 50 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.IsZzz,     TheSchemaTable.Rows[CI.isZzz]);    
      /* 61 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.StRbt1,    TheSchemaTable.Rows[CI.stRbt1]);   
      /* 62 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.StRbt2,    TheSchemaTable.Rows[CI.stRbt2]);   
      /* 63 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.StSRbt,    TheSchemaTable.Rows[CI.stSRbt]);   
      /* 64 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.StCsSc,    TheSchemaTable.Rows[CI.stCsSc]);   
      /* 65 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.StProviz,  TheSchemaTable.Rows[CI.stProviz]); 
      /* 66 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.PnbMProv,  TheSchemaTable.Rows[CI.pnbMProv]); 
      /* 67 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.PnbVProv,  TheSchemaTable.Rows[CI.pnbVProv]); 
      /* 68 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.PnbMPlaca, TheSchemaTable.Rows[CI.pnbMPlaca]);
      /* 69 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.PnbVPlaca, TheSchemaTable.Rows[CI.pnbVPlaca]);
      /* 60 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.ValutaPl,  TheSchemaTable.Rows[CI.valutaPl]); 
      /* 71 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.RokOtprm,  TheSchemaTable.Rows[CI.rokOtprm]); 
      /* 72 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.IsCentr,   TheSchemaTable.Rows[CI.isCentr]);  
      /* 73 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.CentrID,   TheSchemaTable.Rows[CI.centrID]);  
      /* 74 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.CentrTick, TheSchemaTable.Rows[CI.centrTick]);
      /* 75 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.KontoPot,  TheSchemaTable.Rows[CI.kontoPot]);    
      /* 76 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.IsMtr  ,   TheSchemaTable.Rows[CI.isMtr]);  
      /* 77 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.IsKupac,   TheSchemaTable.Rows[CI.isKupac]);  
      /* 78 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.IsDobav,   TheSchemaTable.Rows[CI.isDobav]);  
      /* 79 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.IsBanka,   TheSchemaTable.Rows[CI.isBanka]);  
      /* 80 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Oib,       TheSchemaTable.Rows[CI.oib]);  
      /* 81 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Drzava  ,  TheSchemaTable.Rows[CI.drzava  ]);  
      /* 82 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Swift   ,  TheSchemaTable.Rows[CI.swift   ]);  
      /* 83 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Iban    ,  TheSchemaTable.Rows[CI.iban    ]);  
    ///* 84 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.DevName ,  TheSchemaTable.Rows[CI.devName ]);  
      /* 84 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.DevName_OLD ,  TheSchemaTable.Rows[CI.devName ]);  
      /* 85 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.FinLimit,  TheSchemaTable.Rows[CI.finLimit]);  
      /* 86 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.UgovorNo,  TheSchemaTable.Rows[CI.ugovorNo]);  
      /* 87 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.Komisija,  TheSchemaTable.Rows[CI.komisija]);  
      /* 88 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.SklKonto,  TheSchemaTable.Rows[CI.sklKonto]);  
      /* 89 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.SklNum  ,  TheSchemaTable.Rows[CI.sklNum  ]);  
      /* 90 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.VatCntryCode, TheSchemaTable.Rows[CI.vatCntryCode]);  
      /* 91 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.MitoIzn ,  TheSchemaTable.Rows[CI.mitoIzn ]);  
      /* 92 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.MitoSt  ,  TheSchemaTable.Rows[CI.mitoSt  ]);  
      /* 93 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.InvestTr,  TheSchemaTable.Rows[CI.investTr]);  
      /* 94 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.TrecaStr,  TheSchemaTable.Rows[CI.trecaStr]);  
      
      /* 95 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.TimeOd_1,  TheSchemaTable.Rows[CI.timeOd_1]);  
      /* 95 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.TimeDo_1,  TheSchemaTable.Rows[CI.timeDo_1]);  
      /* 95 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.TimeOd_2,  TheSchemaTable.Rows[CI.timeOd_2]);  
      /* 95 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.TimeDo_2,  TheSchemaTable.Rows[CI.timeDo_2]);  
      /* 95 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.TimeOd_3,  TheSchemaTable.Rows[CI.timeOd_3]);  
      /* 95 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.TimeDo_3,  TheSchemaTable.Rows[CI.timeDo_3]);  
      /* 95 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.TimeOd_4,  TheSchemaTable.Rows[CI.timeOd_4]);  
      /* 95 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.TimeDo_4,  TheSchemaTable.Rows[CI.timeDo_4]);  
      /* 95 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.TimeOd_5,  TheSchemaTable.Rows[CI.timeOd_5]);  
      /* 95 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.TimeDo_5,  TheSchemaTable.Rows[CI.timeDo_5]);  
      /* 95 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.TimeOd_6,  TheSchemaTable.Rows[CI.timeOd_6]);  
      /* 95 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.TimeDo_6,  TheSchemaTable.Rows[CI.timeDo_6]);  
      /* 95 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.TimeOd_7,  TheSchemaTable.Rows[CI.timeOd_7]);  
      /* 95 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.TimeDo_7,  TheSchemaTable.Rows[CI.timeDo_7]);  
      /*109 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.R1kind  ,  TheSchemaTable.Rows[CI.R1kind  ]);  
      /*110 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.IdIsPolStmnt, TheSchemaTable.Rows[CI.idIsPolStmnt]);  
      /*111 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.IdBirthDate , TheSchemaTable.Rows[CI.idBirthDate ]);  
      /*112 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.IdExpDate   , TheSchemaTable.Rows[CI.idExpDate   ]);  
      /*113 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.IdNumber    , TheSchemaTable.Rows[CI.idNumber    ]);  
      /*114 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.IdIssuer    , TheSchemaTable.Rows[CI.idIssuer    ]);  
      /*115 */ VvSQL.CreateCommandParameter(cmd, preffix, kupdob.IdCitizenshp, TheSchemaTable.Rows[CI.idCitizenshp]);  

         if(isArhiva)
         {
            VvSQL.CreateCommandParameter(cmd, preffix, kupdob.OrigRecID, TheSchemaTable.Rows[CI.origRecID]);
            VvSQL.CreateCommandParameter(cmd, preffix, kupdob.RecVer,    TheSchemaTable.Rows[CI.recVer]);
            VvSQL.CreateCommandParameter(cmd, preffix, kupdob.ArAction,  TheSchemaTable.Rows[CI.arAction]);
            VvSQL.CreateCommandParameter(cmd, preffix, kupdob.ArTS,      TheSchemaTable.Rows[CI.arTS]);
            VvSQL.CreateCommandParameter(cmd, preffix, kupdob.ArUID,     TheSchemaTable.Rows[CI.arUID]);
         }
      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataRow

   //public void FillFromDataRow(VvDataRecord arhivedDataRecord, Vektor.DataLayer.TypedDataSet_kupdob.kupdobRow kupdobRow)
   //public void FillFromDataRow(VvDataRecord arhivedDataRecord, Vektor.DataLayer.DS_kupdob.kupdobRow kupdobRow)
   //{
   //   KupdobStruct drData = new KupdobStruct();

   //   drData._recID  = (uint)kupdobRow.prjktKupdobCD;
   //   drData._addTS  = kupdobRow.addTS;
   //   drData._modTS  = kupdobRow.modTS;
   //   drData._addUID = kupdobRow.addUID;
   //   drData._modUID = kupdobRow.modUID;

   //   /* 05 */  drData._naziv     = kupdobRow.naziv    ;    
   //   /* 06 */  drData._ticker      = kupdobRow.ticker     ;     
   //   /* 07 */  drData._matbr     = kupdobRow.matbr    ;    
   //   /* 08 */  drData._ts       = kupdobRow.ts      ;      
   //   /* 09 */  drData._dugoIme   = kupdobRow.dugoIme  ;  
   //   /* 10 */  drData._ulica1    = kupdobRow.ulica1   ;   
   //   /* 11 */  drData._ulica2    = kupdobRow.ulica2   ;   
   //   /* 12 */  drData._grad      = kupdobRow.grad     ;     
   //   /* 13 */  drData._postaBr   = kupdobRow.postaBr  ;  
   //   /* 14 */  drData._opcina    = kupdobRow.opcina   ;   
   //   /* 15 */  drData._opcCd     = kupdobRow.opcCd    ;    
   //   /* 16 */  drData._zupan     = kupdobRow.zupan    ;    
   //   /* 17 */  drData._zupCd     = kupdobRow.zupCd    ;    
   //   /* 18 */  drData._ime       = kupdobRow.ime      ;      
   //   /* 19 */  drData._prezime   = kupdobRow.prezime  ;  
   //   /* 20 */  drData._tel1      = kupdobRow.tel1     ;     
   //   /* 21 */  drData._tel2      = kupdobRow.tel2     ;     
   //   /* 22 */  drData._fax       = kupdobRow.fax      ;      
   //   /* 23 */  drData._gsm       = kupdobRow.gsm      ;      
   //   /* 24 */  drData._email     = kupdobRow.email    ;    
   //   /* 25 */  drData._url       = kupdobRow.url      ;      
   //   /* 26 */  drData._ziro1     = kupdobRow.ziro1    ;    
   //   /* 27 */  drData._ziro1By   = kupdobRow.ziro1By  ;  
   //   /* 28 */  drData._ziro1PnbM = kupdobRow.ziro1PnbM;
   //   /* 29 */  drData._ziro1PnbV = kupdobRow.ziro1PnbV;
   //   /* 30 */  drData._ziro2     = kupdobRow.ziro2    ;    
   //   /* 31 */  drData._ziro2By   = kupdobRow.ziro2By  ;  
   //   /* 32 */  drData._ziro2PnbM = kupdobRow.ziro2PnbM;
   //   /* 33 */  drData._ziro2PnbV = kupdobRow.ziro2PnbV;
   //   /* 34 */  drData._ziro3     = kupdobRow.ziro3    ;    
   //   /* 35 */  drData._ziro3By   = kupdobRow.ziro3By  ;  
   //   /* 36 */  drData._ziro3PnbM = kupdobRow.ziro3PnbM;
   //   /* 37 */  drData._ziro3PnbV = kupdobRow.ziro3PnbV;
   //   /* 38 */  drData._ziro4     = kupdobRow.ziro4    ;    
   //   /* 39 */  drData._ziro4By   = kupdobRow.ziro4By  ;  
   //   /* 40 */  drData._ziro4PnbM = kupdobRow.ziro4PnbM;
   //   /* 41 */  drData._ziro4PnbV = kupdobRow.ziro4PnbV;
   //   /* 42 */  drData._konto     = kupdobRow.konto    ;    
   //   /* 43 */  drData._regob     = kupdobRow.regob    ;    
   //   /* 44 */  drData._sifDcd    = kupdobRow.sifDcd   ;   
   //   /* 45 */  drData._sifDname  = kupdobRow.sifDname ; 
   //   /* 46 */  drData._date      = kupdobRow.date     ;
   //   /* 47 */  drData._putnikID  = kupdobRow.putnikID ; 
   //   /* 48 */  drData._putName   = kupdobRow.putName  ;  
   //   /* 49 */  drData._fuse1     = kupdobRow.fuse1    ;    
   //   /* 50 */  drData._fuse2     = kupdobRow.fuse2    ;    
   //   /* 51 */  drData._napom1    = kupdobRow.napom1   ;   
   //   /* 52 */  drData._napom2    = kupdobRow.napom2   ;   
   //   /* 53 */  drData._komentar  = kupdobRow.komentar ; 
   //   /* 54 */  drData._isObrt    = kupdobRow.isObrt   ;   
   //   /* 55 */  drData._isFrgn    = kupdobRow.isFrgn   ;   
   //   /* 56 */  drData._isPdv     = kupdobRow.isPdv    ;    
   //   /* 57 */  drData._isXxx     = kupdobRow.isXxx    ;    
   //   /* 58 */  drData._isYyy     = kupdobRow.isYyy    ;    
   //   /* 59 */  drData._isZzz     = kupdobRow.isZzz    ;    
   //   /* 60 */  drData._stRbt1    = kupdobRow.stRbt1   ;   
   //   /* 61 */  drData._stRbt2    = kupdobRow.stRbt2   ;   
   //   /* 62 */  drData._stSRbt    = kupdobRow.stSRbt   ;   
   //   /* 63 */  drData._stCsSc    = kupdobRow.stCsSc   ;   
   //   /* 64 */  drData._stProviz  = kupdobRow.stProviz ; 
   //   /* 65 */  drData._pnbMProv  = kupdobRow.pnbMProv ; 
   //   /* 66 */  drData._pnbVProv  = kupdobRow.pnbVProv ; 
   //   /* 67 */  drData._pnbMPlaca = kupdobRow.pnbMPlaca;
   //   /* 68 */  drData._pnbVPlaca = kupdobRow.pnbVPlaca;
   //   /* 69 */  drData._valutaPl  = kupdobRow.valutaPl ; 
   //   /* 70 */  drData._rokOtprm  = kupdobRow.rokOtprm ; 
   //   /* 71 */  drData._isCentr   = kupdobRow.isCentr  ;  
   //   /* 72 */  drData._centrID   = kupdobRow.centrID  ;  
   //   /* 73 */  drData._centrName = kupdobRow.centrName;

   //   ((Kupdob)arhivedDataRecord).CurrentData = drData;

   //   return;
   //}

   #endregion FillFromDataRow

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva)
   {
      KupdobStruct rdrData = new KupdobStruct();

      rdrData._recID   = reader.GetUInt32  (CI.recID);     
      rdrData._addTS   = reader.GetDateTime(CI.addTS);
      rdrData._modTS   = reader.GetDateTime(CI.modTS);
      rdrData._addUID  = reader.GetString  (CI.addUID);
      rdrData._modUID  = reader.GetString  (CI.modUID);
      rdrData._lanSrvID= reader.GetUInt32  (CI.lanSrvID);     
      rdrData._lanRecID= reader.GetUInt32  (CI.lanRecID);     

      /* 05 */      rdrData._naziv     = reader.GetString(CI.naziv);
      /* 06 */      rdrData._ticker    = reader.GetString(CI.ticker);
      /* 07 */      rdrData._kupdobCD  = reader.GetUInt32(CI.kupdobCD);
      /* 08 */      rdrData._matbr     = reader.GetString(CI.matbr);
      /* 09 */      rdrData._tip       = reader.GetString(CI.tip);
      /* 00 */      rdrData._dugoIme   = reader.GetString(CI.dugoIme);
      /* 11 */      rdrData._ulica1    = reader.GetString(CI.ulica1);
      /* 12 */      rdrData._ulica2    = reader.GetString(CI.ulica2);
      /* 13 */      rdrData._grad      = reader.GetString(CI.grad);
      /* 14 */      rdrData._postaBr   = reader.GetString(CI.postaBr);
      /* 15 */      rdrData._opcina    = reader.GetString(CI.opcina);
      /* 16 */      rdrData._opcCd     = reader.GetString(CI.opcCd);
      /* 17 */      rdrData._zupan     = reader.GetString(CI.zupan);
      /* 18 */      rdrData._zupCd     = reader.GetString(CI.zupCd);
      /* 19 */      rdrData._ime       = reader.GetString(CI.ime);
      /* 10 */      rdrData._prezime   = reader.GetString(CI.prezime);
      /* 21 */      rdrData._tel1      = reader.GetString(CI.tel1);
      /* 22 */      rdrData._tel2      = reader.GetString(CI.tel2);
      /* 23 */      rdrData._fax       = reader.GetString(CI.fax);
      /* 24 */      rdrData._gsm       = reader.GetString(CI.gsm);
      /* 25 */      rdrData._email     = reader.GetString(CI.email);
      /* 26 */      rdrData._url       = reader.GetString(CI.url);
      /* 27 */      rdrData._ziro1     = reader.GetString(CI.ziro1);
      /* 28 */      rdrData._ziro1By   = reader.GetString(CI.ziro1By);
      /* 29 */      rdrData._ziro1PnbM = reader.GetString(CI.ziro1PnbM);
      /* 20 */      rdrData._ziro1PnbV = reader.GetString(CI.ziro1PnbV);
      /* 31 */      rdrData._ziro2     = reader.GetString(CI.ziro2);
      /* 32 */      rdrData._ziro2By   = reader.GetString(CI.ziro2By);
      /* 33 */      rdrData._ziro2PnbM = reader.GetString(CI.ziro2PnbM);
      /* 34 */      rdrData._ziro2PnbV = reader.GetString(CI.ziro2PnbV);
      /* 35 */      rdrData._ziro3     = reader.GetString(CI.ziro3);
      /* 36 */      rdrData._ziro3By   = reader.GetString(CI.ziro3By);
      /* 37 */      rdrData._ziro3PnbM = reader.GetString(CI.ziro3PnbM);
      /* 38 */      rdrData._ziro3PnbV = reader.GetString(CI.ziro3PnbV);
      /* 39 */      rdrData._ziro4     = reader.GetString(CI.ziro4);
      /* 30 */      rdrData._ziro4By   = reader.GetString(CI.ziro4By);
      /* 41 */      rdrData._ziro4PnbM = reader.GetString(CI.ziro4PnbM);
      /* 42 */      rdrData._ziro4PnbV = reader.GetString(CI.ziro4PnbV);
      /* 43 */      rdrData._kontoDug  = reader.GetString(CI.kontoDug);
      /* 44 */      rdrData._regob     = reader.GetString(CI.regob);
      /* 45 */      rdrData._sifDcd    = reader.GetString(CI.sifDcd);
      /* 46 */      rdrData._sifDname  = reader.GetString(CI.sifDname);
      /* 47 */      rdrData._date      = reader.GetDateTime(CI.date);
      /* 48 */      rdrData._putnikID  = reader.GetUInt32(CI.putnikID);
      /* 49 */      rdrData._putName   = reader.GetString(CI.putName);
      /* 40 */      rdrData._fuse1     = reader.GetString(CI.fuse1);
      /* 51 */      rdrData._fuse2     = reader.GetString(CI.fuse2);
      /* 52 */      rdrData._napom1    = reader.GetString(CI.napom1);
      /* 53 */      rdrData._napom2    = reader.GetString(CI.napom2);
      /* 54 */      rdrData._komentar  = reader.GetString(CI.komentar);
    ///* 55 */      rdrData._isObrt    = reader.GetBoolean(CI.isObrt);
      /* 55 */      rdrData._isObrt    = reader.GetUInt16 (CI.isObrt);
      /* 56 */      rdrData._isFrgn    = reader.GetBoolean(CI.isFrgn);
      /* 57 */      rdrData._isPdv     = reader.GetBoolean(CI.isPdv);
      /* 58 */      rdrData._isXxx     = reader.GetBoolean(CI.isXxx);
      /* 59 */      rdrData._isYyy     = reader.GetBoolean(CI.isYyy);
      /* 50 */      rdrData._isZzz     = reader.GetBoolean(CI.isZzz);
      /* 61 */      rdrData._stRbt1    = reader.GetDecimal(CI.stRbt1);
      /* 62 */      rdrData._stRbt2    = reader.GetDecimal(CI.stRbt2);
      /* 63 */      rdrData._stSRbt    = reader.GetDecimal(CI.stSRbt);
      /* 64 */      rdrData._stCsSc    = reader.GetDecimal(CI.stCsSc);
      /* 65 */      rdrData._stProviz  = reader.GetDecimal(CI.stProviz);
      /* 66 */      rdrData._pnbMProv  = reader.GetString(CI.pnbMProv);
      /* 67 */      rdrData._pnbVProv  = reader.GetString(CI.pnbVProv);
      /* 68 */      rdrData._pnbMPlaca = reader.GetString(CI.pnbMPlaca);
      /* 69 */      rdrData._pnbVPlaca = reader.GetString(CI.pnbVPlaca);
      /* 60 */      rdrData._valutaPl  = reader.GetInt16(CI.valutaPl);
      /* 71 */      rdrData._rokOtprm  = reader.GetInt16(CI.rokOtprm);
      /* 72 */      rdrData._isCentr   = reader.GetBoolean(CI.isCentr);
      /* 73 */      rdrData._centrID   = reader.GetUInt32(CI.centrID);
      /* 74 */      rdrData._centrTick = reader.GetString(CI.centrTick);
      /* 75 */      rdrData._kontoPot  = reader.GetString(CI.kontoPot);
      /* 76 */      rdrData._isMtr     = reader.GetBoolean(CI.isMtr);
      /* 77 */      rdrData._isKupac   = reader.GetBoolean(CI.isKupac);
      /* 78 */      rdrData._isDobav   = reader.GetBoolean(CI.isDobav);
      /* 79 */      rdrData._isBanka   = reader.GetBoolean(CI.isBanka);
      /* 80 */      rdrData._oib       = reader.GetString (CI.oib);
      /* 81 */      rdrData._drzava    = reader.GetString (CI.drzava  );
      /* 82 */      rdrData._swift     = reader.GetString (CI.swift   );
      /* 83 */      rdrData._iban      = reader.GetString (CI.iban    );
      /* 84 */      rdrData._devName   = reader.GetString (CI.devName );
      /* 85 */      rdrData._finLimit  = reader.GetDecimal(CI.finLimit);
      /* 86 */      rdrData._ugovorNo  = reader.GetString (CI.ugovorNo);
      /* 87 */      rdrData._komisija  = reader.GetUInt16 (CI.komisija);
      /* 88 */      rdrData._sklKonto  = reader.GetString (CI.sklKonto);
      /* 89 */      rdrData._sklNum    = reader.GetUInt32 (CI.sklNum  );
      /* 90 */      rdrData._vatCntryCode = reader.GetString (CI.vatCntryCode);
      /* 91 */      rdrData._mitoIzn  = reader.GetDecimal(CI.mitoIzn );
      /* 92 */      rdrData._mitoSt   = reader.GetDecimal(CI.mitoSt  );
      /* 93 */      rdrData._investTr = reader.GetDecimal(CI.investTr);
      /* 94 */      rdrData._trecaStr = reader.GetDecimal(CI.trecaStr);

      /* 94 */      rdrData._timeOd_1 = reader.GetTimeSpan(CI.timeOd_1);
      /* 94 */      rdrData._timeDo_1 = reader.GetTimeSpan(CI.timeDo_1);
      /* 94 */      rdrData._timeOd_2 = reader.GetTimeSpan(CI.timeOd_2);
      /* 94 */      rdrData._timeDo_2 = reader.GetTimeSpan(CI.timeDo_2);
      /* 94 */      rdrData._timeOd_3 = reader.GetTimeSpan(CI.timeOd_3);
      /* 94 */      rdrData._timeDo_3 = reader.GetTimeSpan(CI.timeDo_3);
      /* 94 */      rdrData._timeOd_4 = reader.GetTimeSpan(CI.timeOd_4);
      /* 94 */      rdrData._timeDo_4 = reader.GetTimeSpan(CI.timeDo_4);
      /* 94 */      rdrData._timeOd_5 = reader.GetTimeSpan(CI.timeOd_5);
      /* 94 */      rdrData._timeDo_5 = reader.GetTimeSpan(CI.timeDo_5);
      /* 94 */      rdrData._timeOd_6 = reader.GetTimeSpan(CI.timeOd_6);
      /* 94 */      rdrData._timeDo_6 = reader.GetTimeSpan(CI.timeDo_6);
      /* 94 */      rdrData._timeOd_7 = reader.GetTimeSpan(CI.timeOd_7);
      /* 94 */      rdrData._timeDo_7 = reader.GetTimeSpan(CI.timeDo_7);

      /*109 */      rdrData._R1kind       = reader.GetUInt16  (CI.R1kind)      ;
      /*110 */      rdrData._idIsPolStmnt = reader.GetBoolean (CI.idIsPolStmnt);
      /*111 */      rdrData._idBirthDate  = reader.GetDateTime(CI.idBirthDate) ;
      /*112 */      rdrData._idExpDate    = reader.GetDateTime(CI.idExpDate)   ;
      /*113 */      rdrData._idNumber     = reader.GetString  (CI.idNumber)    ;
      /*114 */      rdrData._idIssuer     = reader.GetString  (CI.idIssuer)    ;
      /*115 */      rdrData._idCitizenshp = reader.GetString  (CI.idCitizenshp);

      ((Kupdob)vvDataRecord).CurrentData = rdrData;

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

   #region KupdobCI struct & InitializeSchemaColumnIndexes()

   public struct KupdobCI
   {
      internal int recID;
      internal int addTS;
      internal int modTS;
      internal int addUID;
      internal int modUID;
      internal int lanSrvID;
      internal int lanRecID;

      /* 05 */ internal int   naziv    ;    
      /* 06 */ internal int   ticker   ;     
      /* 07 */ internal int   kupdobCD ;    
      /* 08 */ internal int   matbr    ;    
      /* 09 */ internal int   tip      ;      
      /* 00 */ internal int   dugoIme  ;  
      /* 11 */ internal int   ulica1   ;   
      /* 12 */ internal int   ulica2   ;   
      /* 13 */ internal int   grad     ;     
      /* 14 */ internal int   postaBr  ;  
      /* 15 */ internal int   opcina   ;   
      /* 16 */ internal int   opcCd    ;    
      /* 17 */ internal int   zupan    ;    
      /* 18 */ internal int   zupCd    ;    
      /* 19 */ internal int   ime      ;      
      /* 10 */ internal int   prezime  ;  
      /* 21 */ internal int   tel1     ;     
      /* 22 */ internal int   tel2     ;     
      /* 23 */ internal int   fax      ;      
      /* 24 */ internal int   gsm      ;      
      /* 25 */ internal int   email    ;    
      /* 26 */ internal int   url      ;      
      /* 27 */ internal int   ziro1    ;    
      /* 28 */ internal int   ziro1By  ;  
      /* 29 */ internal int   ziro1PnbM;
      /* 20 */ internal int   ziro1PnbV;
      /* 31 */ internal int   ziro2    ;    
      /* 32 */ internal int   ziro2By  ;  
      /* 33 */ internal int   ziro2PnbM;
      /* 34 */ internal int   ziro2PnbV;
      /* 35 */ internal int   ziro3    ;    
      /* 36 */ internal int   ziro3By  ;  
      /* 37 */ internal int   ziro3PnbM;
      /* 38 */ internal int   ziro3PnbV;
      /* 39 */ internal int   ziro4    ;    
      /* 30 */ internal int   ziro4By  ;  
      /* 41 */ internal int   ziro4PnbM;
      /* 42 */ internal int   ziro4PnbV;
      /* 43 */ internal int   kontoDug ;    
      /* 44 */ internal int   regob    ;    
      /* 45 */ internal int   sifDcd   ;   
      /* 46 */ internal int   sifDname ; 
      /* 47 */ internal int   date     ;
      /* 48 */ internal int   putnikID ; 
      /* 49 */ internal int   putName  ;  
      /* 40 */ internal int   fuse1    ;    
      /* 51 */ internal int   fuse2    ;    
      /* 52 */ internal int   napom1   ;   
      /* 53 */ internal int   napom2   ;   
      /* 54 */ internal int   komentar ; 
      /* 55 */ internal int   isObrt   ;   
      /* 56 */ internal int   isFrgn   ;   
      /* 57 */ internal int   isPdv    ;    
      /* 58 */ internal int   isXxx    ;    
      /* 59 */ internal int   isYyy    ;    
      /* 50 */ internal int   isZzz    ;    
      /* 61 */ internal int   stRbt1   ;   
      /* 62 */ internal int   stRbt2   ;   
      /* 63 */ internal int   stSRbt   ;   
      /* 64 */ internal int   stCsSc   ;   
      /* 65 */ internal int   stProviz ; 
      /* 66 */ internal int   pnbMProv ; 
      /* 67 */ internal int   pnbVProv ; 
      /* 68 */ internal int   pnbMPlaca;
      /* 69 */ internal int   pnbVPlaca;
      /* 60 */ internal int   valutaPl ; 
      /* 71 */ internal int   rokOtprm ; 
      /* 72 */ internal int   isCentr  ;  
      /* 73 */ internal int   centrID  ;  
      /* 74 */ internal int   centrTick;
      /* 75 */ internal int   kontoPot ;
      /* 76 */ internal int   isMtr    ;
      /* 77 */ internal int   isKupac  ;
      /* 78 */ internal int   isDobav  ;
      /* 79 */ internal int   isBanka  ;
      /* 80 */ internal int   oib      ;
      /* 81 */ internal int   drzava   ;
      /* 82 */ internal int   swift    ;
      /* 83 */ internal int   iban     ;
      /* 84 */ internal int   devName  ;
      /* 85 */ internal int   finLimit ;
      /* 86 */ internal int   ugovorNo ;
      /* 87 */ internal int   komisija ;
      /* 88 */ internal int   sklKonto ;
      /* 89 */ internal int   sklNum   ;
      /* 90 */ internal int   vatCntryCode;
      /* 91 */ internal int   mitoIzn ;
      /* 92 */ internal int   mitoSt  ;
      /* 93 */ internal int   investTr;
      /* 94 */ internal int   trecaStr;

      /* 95 */ internal int   timeOd_1;
      /* 95 */ internal int   timeDo_1;
      /* 95 */ internal int   timeOd_2;
      /* 95 */ internal int   timeDo_2;
      /* 95 */ internal int   timeOd_3;
      /* 95 */ internal int   timeDo_3;
      /* 95 */ internal int   timeOd_4;
      /* 95 */ internal int   timeDo_4;
      /* 95 */ internal int   timeOd_5;
      /* 95 */ internal int   timeDo_5;
      /* 95 */ internal int   timeOd_6;
      /* 95 */ internal int   timeDo_6;
      /* 95 */ internal int   timeOd_7;
      /* 95 */ internal int   timeDo_7;

      /*109 */ internal int R1kind      ;
      /*110 */ internal int idIsPolStmnt;
      /*111 */ internal int idBirthDate ;
      /*112 */ internal int idExpDate   ;
      /*113 */ internal int idNumber    ;
      /*114 */ internal int idIssuer    ;
      /*115 */ internal int idCitizenshp;
      
      internal int origRecID;
      internal int recVer;
      internal int arAction;
      internal int arTS;
      internal int arUID;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public KupdobCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID     = GetSchemaColumnIndex("recID");
      CI.addTS     = GetSchemaColumnIndex("addTS");
      CI.modTS     = GetSchemaColumnIndex("modTS");
      CI.addUID    = GetSchemaColumnIndex("addUID");
      CI.modUID    = GetSchemaColumnIndex("modUID");
      CI.lanSrvID  = GetSchemaColumnIndex("lanSrvID");
      CI.lanRecID  = GetSchemaColumnIndex("lanRecID");

      CI.naziv     = GetSchemaColumnIndex("naziv");
      CI.ticker    = GetSchemaColumnIndex("ticker");
      CI.kupdobCD  = GetSchemaColumnIndex("kupdobCD");
      CI.matbr     = GetSchemaColumnIndex("matbr");
      CI.tip       = GetSchemaColumnIndex("tip");
      CI.dugoIme   = GetSchemaColumnIndex("dugoIme");
      CI.ulica1    = GetSchemaColumnIndex("ulica1");
      CI.ulica2    = GetSchemaColumnIndex("ulica2");
      CI.grad      = GetSchemaColumnIndex("grad");
      CI.postaBr   = GetSchemaColumnIndex("postaBr");
      CI.opcina    = GetSchemaColumnIndex("opcina");
      CI.opcCd     = GetSchemaColumnIndex("opcCd");
      CI.zupan     = GetSchemaColumnIndex("zupan");
      CI.zupCd     = GetSchemaColumnIndex("zupCd");
      CI.ime       = GetSchemaColumnIndex("ime");
      CI.prezime   = GetSchemaColumnIndex("prezime");
      CI.tel1      = GetSchemaColumnIndex("tel1");
      CI.tel2      = GetSchemaColumnIndex("tel2");
      CI.fax       = GetSchemaColumnIndex("fax");
      CI.gsm       = GetSchemaColumnIndex("gsm");
      CI.email     = GetSchemaColumnIndex("email");
      CI.url       = GetSchemaColumnIndex("url");
      CI.ziro1     = GetSchemaColumnIndex("ziro1");
      CI.ziro1By   = GetSchemaColumnIndex("ziro1By");
      CI.ziro1PnbM = GetSchemaColumnIndex("ziro1PnbM");
      CI.ziro1PnbV = GetSchemaColumnIndex("ziro1PnbV");
      CI.ziro2     = GetSchemaColumnIndex("ziro2");
      CI.ziro2By   = GetSchemaColumnIndex("ziro2By");
      CI.ziro2PnbM = GetSchemaColumnIndex("ziro2PnbM");
      CI.ziro2PnbV = GetSchemaColumnIndex("ziro2PnbV");
      CI.ziro3     = GetSchemaColumnIndex("ziro3");
      CI.ziro3By   = GetSchemaColumnIndex("ziro3By");
      CI.ziro3PnbM = GetSchemaColumnIndex("ziro3PnbM");
      CI.ziro3PnbV = GetSchemaColumnIndex("ziro3PnbV");
      CI.ziro4     = GetSchemaColumnIndex("ziro4");
      CI.ziro4By   = GetSchemaColumnIndex("ziro4By");
      CI.ziro4PnbM = GetSchemaColumnIndex("ziro4PnbM");
      CI.ziro4PnbV = GetSchemaColumnIndex("ziro4PnbV");
      CI.kontoDug  = GetSchemaColumnIndex("kontoDug");
      CI.regob     = GetSchemaColumnIndex("regob");
      CI.sifDcd    = GetSchemaColumnIndex("sifDcd");
      CI.sifDname  = GetSchemaColumnIndex("sifDname");
      CI.date      = GetSchemaColumnIndex("date");
      CI.putnikID  = GetSchemaColumnIndex("putnikID");
      CI.putName   = GetSchemaColumnIndex("putName");
      CI.fuse1     = GetSchemaColumnIndex("fuse1");
      CI.fuse2     = GetSchemaColumnIndex("fuse2");
      CI.napom1    = GetSchemaColumnIndex("napom1");
      CI.napom2    = GetSchemaColumnIndex("napom2");
      CI.komentar  = GetSchemaColumnIndex("komentar");
      CI.isObrt    = GetSchemaColumnIndex("isObrt");
      CI.isFrgn    = GetSchemaColumnIndex("isFrgn");
      CI.isPdv     = GetSchemaColumnIndex("isPdv");
      CI.isXxx     = GetSchemaColumnIndex("isXxx");
      CI.isYyy     = GetSchemaColumnIndex("isYyy");
      CI.isZzz     = GetSchemaColumnIndex("isZzz");
      CI.stRbt1    = GetSchemaColumnIndex("stRbt1");
      CI.stRbt2    = GetSchemaColumnIndex("stRbt2");
      CI.stSRbt    = GetSchemaColumnIndex("stSRbt");
      CI.stCsSc    = GetSchemaColumnIndex("stCsSc");
      CI.stProviz  = GetSchemaColumnIndex("stProviz");
      CI.pnbMProv  = GetSchemaColumnIndex("pnbMProv");
      CI.pnbVProv  = GetSchemaColumnIndex("pnbVProv");
      CI.pnbMPlaca = GetSchemaColumnIndex("pnbMPlaca");
      CI.pnbVPlaca = GetSchemaColumnIndex("pnbVPlaca");
      CI.valutaPl  = GetSchemaColumnIndex("valutaPl");
      CI.rokOtprm  = GetSchemaColumnIndex("rokOtprm");
      CI.isCentr   = GetSchemaColumnIndex("isCentr");
      CI.centrID   = GetSchemaColumnIndex("centrID");
      CI.centrTick = GetSchemaColumnIndex("centrTick");
      CI.kontoPot  = GetSchemaColumnIndex("kontoPot");
      CI.isMtr     = GetSchemaColumnIndex("isMtr");
      CI.isKupac   = GetSchemaColumnIndex("isKupac");
      CI.isDobav   = GetSchemaColumnIndex("isDobav");
      CI.isBanka   = GetSchemaColumnIndex("isBanka");
      CI.oib       = GetSchemaColumnIndex("oib");
      CI.drzava    = GetSchemaColumnIndex("drzava");
      CI.swift     = GetSchemaColumnIndex("swift");
      CI.iban      = GetSchemaColumnIndex("iban");
      CI.devName   = GetSchemaColumnIndex("devName");
      CI.finLimit  = GetSchemaColumnIndex("finLimit");
      CI.ugovorNo  = GetSchemaColumnIndex("ugovorNo");
      CI.komisija  = GetSchemaColumnIndex("komisija");
      CI.sklKonto  = GetSchemaColumnIndex("sklKonto");
      CI.sklNum    = GetSchemaColumnIndex("sklNum"  );
      CI.vatCntryCode = GetSchemaColumnIndex("vatCntryCode");
      CI.mitoIzn   = GetSchemaColumnIndex("mitoIzn"   );
      CI.mitoSt    = GetSchemaColumnIndex("mitoSt"    );
      CI.investTr  = GetSchemaColumnIndex("investTr"  );
      CI.trecaStr  = GetSchemaColumnIndex("trecaStr"  );
      CI.timeOd_1  = GetSchemaColumnIndex("timeOd_1"  );
      CI.timeDo_1  = GetSchemaColumnIndex("timeDo_1"  );
      CI.timeOd_2  = GetSchemaColumnIndex("timeOd_2"  );
      CI.timeDo_2  = GetSchemaColumnIndex("timeDo_2"  );
      CI.timeOd_3  = GetSchemaColumnIndex("timeOd_3"  );
      CI.timeDo_3  = GetSchemaColumnIndex("timeDo_3"  );
      CI.timeOd_4  = GetSchemaColumnIndex("timeOd_4"  );
      CI.timeDo_4  = GetSchemaColumnIndex("timeDo_4"  );
      CI.timeOd_5  = GetSchemaColumnIndex("timeOd_5"  );
      CI.timeDo_5  = GetSchemaColumnIndex("timeDo_5"  );
      CI.timeOd_6  = GetSchemaColumnIndex("timeOd_6"  );
      CI.timeDo_6  = GetSchemaColumnIndex("timeDo_6"  );
      CI.timeOd_7  = GetSchemaColumnIndex("timeOd_7"  );
      CI.timeDo_7  = GetSchemaColumnIndex("timeDo_7"  );
      CI.R1kind    = GetSchemaColumnIndex("r1Kind"    );

      CI.idIsPolStmnt = GetSchemaColumnIndex("idIsPolStmnt");
      CI.idBirthDate  = GetSchemaColumnIndex("idBirthDate");
      CI.idExpDate    = GetSchemaColumnIndex("idExpDate");
      CI.idNumber     = GetSchemaColumnIndex("idNumber");
      CI.idIssuer     = GetSchemaColumnIndex("idIssuer");
      CI.idCitizenshp = GetSchemaColumnIndex("idCitizenshp");

      CI.origRecID      = GetSchemaColumnIndex("origRecID");
      CI.recVer         = GetSchemaColumnIndex("recVer");
      CI.arAction       = GetSchemaColumnIndex("arAction");
      CI.arTS           = GetSchemaColumnIndex("arTS");
      CI.arUID          = GetSchemaColumnIndex("arUID");
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()





   #region LoadFtranses

   public void LoadKupdobFtranses(XSqlConnection dbConnection, Kupdob kupdob_rec)
   {
      if(kupdob_rec.Ftranses == null) kupdob_rec.Ftranses = new List<Ftrans>();
      else                            kupdob_rec.Ftranses.Clear();

      uint    kupdob_cd;
      DataRow drSchema;

      System.Collections.Generic.List<VvSqlFilterMember> filterMembers = new System.Collections.Generic.List<VvSqlFilterMember>(1);
      
      // For this Kupdob only                                                                                                                                            
      drSchema  = ZXC.FtransDao.TheSchemaTable.Rows[ZXC.FtransDao.CI.t_kupdob_cd];
      //kupdob_cd = artikl_rec.RecID;
      kupdob_cd = kupdob_rec.KupdobCD;

      filterMembers.Add(new VvSqlFilterMember(drSchema, "theKupdobCd", kupdob_cd, " = "));

      LoadGenericVvDataRecordList<Ftrans>(dbConnection, kupdob_rec.Ftranses, filterMembers, "t_dokDate DESC, t_dokNum DESC, t_serial DESC");
   }

   #endregion LoadFtranses

   #region IsThisRecordInSomeRelation

   public override bool IsThisRecordInSomeRelation(ZXC.PrivilegedAction action, XSqlConnection conn, VvDataRecord vvDataRecord)
   {

      // 12.12.2012: 
      if(ZXC.ThisIsVektorProject == false) return false;

      bool inFtransKupdobRelation   = false, inFtransMtrosRelation = false, 
           inOsredKupdobRelation    = false, inOsredMtrosRelation  = false,
           inPersonBankaRelation    = false, inPersonMtrosRelation = false, inPtranoKupdobRelation = false, inPlacaRelation = false,
           inArtiklDobavRelation    = false, inArtiklProizvRelation= false,
           inFakturKupdobRelation   = false, inFakturMtrosRelation = false, inFakturPosJedRelation = false, inFakturPrimPlatRelation = false,
           inKupdobCentralaRelation = false, 
           inMixerKupdobRelation    = false, inMixerMtrosRelation = false
           /*, inFakturRelation, inArtikl, inRtransRelation(:-za mtros na mTrans-u)*/;

      int? recCount;
      DataRow drSchema;
      Kupdob kupdob_rec = (Kupdob)vvDataRecord;

      //uint kupdobCD = artikl_rec.RecID;
      //uint kupdobCD = artikl_rec.KupdobCD;
      uint kupdobCD = (action == ZXC.PrivilegedAction.DELREC ? kupdob_rec.KupdobCD : kupdob_rec.BackupedKupdobCD);

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);

      #region Kupdob

      drSchema = ZXC.KupdobDao.TheSchemaTable.Rows[ZXC.KupdobDao.CI.centrID];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elKupdob", kupdobCD, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inKupdobCentralaRelation = true;
      else             inKupdobCentralaRelation = false;

      if(inKupdobCentralaRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));
         Issue_RecordIsInSomeRelation_Mesage(recordName, kupdob_rec, (int)recCount);
      }

      #endregion Kupdob

      #region Ftrans

      drSchema = ZXC.FtransDao.TheSchemaTable.Rows[ZXC.FtransDao.CI.t_kupdob_cd];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elKupdob", kupdobCD, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inFtransKupdobRelation = true;
      else             inFtransKupdobRelation = false;

      drSchema = ZXC.FtransDao.TheSchemaTable.Rows[ZXC.FtransDao.CI.t_mtros_cd];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elMtros", kupdobCD, " = "));

      recCount += CountRecords(conn, filterMembers);

      if(recCount > 0) inFtransMtrosRelation = true;
      else             inFtransMtrosRelation = false;

      if(inFtransKupdobRelation || inFtransMtrosRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));
         Issue_RecordIsInSomeRelation_Mesage(recordName, kupdob_rec, (int)recCount);
      }

      #endregion Ftrans

      #region Person, Placa

      drSchema = ZXC.PersonDao.TheSchemaTable.Rows[ZXC.PersonDao.CI.banka_cd];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elBanka", kupdobCD, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inPersonBankaRelation = true;
      else             inPersonBankaRelation = false;

      drSchema = ZXC.PersonDao.TheSchemaTable.Rows[ZXC.PersonDao.CI.mtros_cd];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elMtros", kupdobCD, " = "));

      recCount += CountRecords(conn, filterMembers);

      if(recCount > 0) inPersonMtrosRelation = true;
      else             inPersonMtrosRelation = false;

      if(inPersonBankaRelation || inPersonMtrosRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));
         Issue_RecordIsInSomeRelation_Mesage(recordName, kupdob_rec, (int)recCount);
      }

      drSchema = ZXC.PtranoDao.TheSchemaTable.Rows[ZXC.PtranoDao.CI.t_kupdob_cd];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elKupdob", kupdobCD, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inPtranoKupdobRelation = true;
      else             inPtranoKupdobRelation = false;

      if(inPtranoKupdobRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));
         Issue_RecordIsInSomeRelation_Mesage(recordName, kupdob_rec, (int)recCount);
      }

      drSchema = ZXC.PlacaDao.TheSchemaTable.Rows[ZXC.PlacaDao.CI.mtros_cd];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elMtros", kupdobCD, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inPlacaRelation = true;
      else             inPlacaRelation = false;

      if(inPlacaRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));
         Issue_RecordIsInSomeRelation_Mesage(recordName, kupdob_rec, (int)recCount);
      }


      #endregion Placa

      #region Osred

      drSchema = ZXC.OsredDao.TheSchemaTable.Rows[ZXC.OsredDao.CI.kupdob_cd];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elKupdob", kupdobCD, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inOsredKupdobRelation = true;
      else             inOsredKupdobRelation = false;

      drSchema = ZXC.OsredDao.TheSchemaTable.Rows[ZXC.OsredDao.CI.mtros_cd];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elMtros", kupdobCD, " = "));

      recCount += CountRecords(conn, filterMembers);

      if(recCount > 0) inOsredMtrosRelation = true;
      else             inOsredMtrosRelation = false;

      if(inOsredKupdobRelation || inOsredMtrosRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));
         Issue_RecordIsInSomeRelation_Mesage(recordName, kupdob_rec, (int)recCount);
      }

      #endregion Osred

      #region Artikl

      drSchema = ZXC.ArtiklDao.TheSchemaTable.Rows[ZXC.ArtiklDao.CI.dobavCD];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elDobav", kupdobCD, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inArtiklDobavRelation = true;
      else             inArtiklDobavRelation = false;

      drSchema = ZXC.ArtiklDao.TheSchemaTable.Rows[ZXC.ArtiklDao.CI.proizCD];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elProizv", kupdobCD, " = "));

      recCount += CountRecords(conn, filterMembers);

      if(recCount > 0) inArtiklProizvRelation = true;
      else             inArtiklProizvRelation = false;

      if(inArtiklDobavRelation || inArtiklProizvRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));
         Issue_RecordIsInSomeRelation_Mesage(recordName, kupdob_rec, (int)recCount);
      }

      #endregion Artikl

      #region Faktur

      drSchema = ZXC.FaktExDao.TheSchemaTable.Rows[ZXC.FaktExDao.CI.kupdobCD];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elKupdob", kupdobCD, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inFakturKupdobRelation = true;
      else             inFakturKupdobRelation = false;

      drSchema = ZXC.FaktExDao.TheSchemaTable.Rows[ZXC.FaktExDao.CI.mtrosCD];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elMtros", kupdobCD, " = "));

      recCount += CountRecords(conn, filterMembers);

      if(recCount > 0) inFakturMtrosRelation = true;
      else             inFakturMtrosRelation = false;

      drSchema = ZXC.FaktExDao.TheSchemaTable.Rows[ZXC.FaktExDao.CI.posJedCD];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elPosJed", kupdobCD, " = "));

      recCount += CountRecords(conn, filterMembers);

      if(recCount > 0) inFakturPosJedRelation = true;
      else             inFakturPosJedRelation = false;

      drSchema = ZXC.FaktExDao.TheSchemaTable.Rows[ZXC.FaktExDao.CI.primPlatCD];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elPrimPlat", kupdobCD, " = "));

      recCount += CountRecords(conn, filterMembers);

      if(recCount > 0) inFakturPrimPlatRelation = true;
      else             inFakturPrimPlatRelation = false;

      if(inFakturKupdobRelation || inFakturMtrosRelation || inFakturPosJedRelation || inFakturPrimPlatRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));
         Issue_RecordIsInSomeRelation_Mesage(recordName, kupdob_rec, (int)recCount);
      }


      #endregion Faktur

      #region Mixer

      drSchema = ZXC.MixerDao.TheSchemaTable.Rows[ZXC.MixerDao.CI.kupdobCD];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elKupdob", kupdobCD, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inMixerKupdobRelation = true;
      else             inMixerKupdobRelation = false;

      drSchema = ZXC.MixerDao.TheSchemaTable.Rows[ZXC.MixerDao.CI.mtrosCD];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elMtros", kupdobCD, " = "));

      recCount += CountRecords(conn, filterMembers);

      if(recCount > 0) inMixerMtrosRelation = true;
      else             inMixerMtrosRelation = false;

      if(inMixerKupdobRelation || inMixerMtrosRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));
         Issue_RecordIsInSomeRelation_Mesage(recordName, kupdob_rec, (int)recCount);
      }

      #endregion Mixer


      return(inFtransKupdobRelation   || inFtransMtrosRelation  || 
             inOsredKupdobRelation    || inOsredMtrosRelation   || 
             inPersonBankaRelation    || inPersonMtrosRelation  || inPtranoKupdobRelation || inPlacaRelation ||
             inArtiklDobavRelation    || inArtiklProizvRelation ||
             inFakturKupdobRelation   || inFakturMtrosRelation  || inFakturPosJedRelation || inFakturPrimPlatRelation ||
             inKupdobCentralaRelation || inMixerKupdobRelation  || inMixerMtrosRelation); 
   }

   #endregion IsThisRecordInSomeRelation

   #region Utils

   //public string GetTickerForRecID(XSqlConnection conn, uint prjktKupdobCD)
   //{
   //   Kupdob artikl_rec = new Kupdob();
   //   SetMe_Record_byRecID(conn, artikl_rec, prjktKupdobCD);

   //   return artikl_rec.Ticker;
   //}

   public string GetTickerForKupdobCD(XSqlConnection conn, uint kupdobCD, bool isPrjkt)
   {
      VvDataRecord vvDataRecord;

      if(isPrjkt)  vvDataRecord = new Prjkt();
      else         vvDataRecord = new Kupdob();

      bool OK = SetMe_Record_bySomeUniqueColumn(conn, vvDataRecord, kupdobCD, ZXC.KupdobSchemaRows[ZXC.KpdbCI.kupdobCD], false, false);

      return (OK ? ((Kupdob)vvDataRecord).Ticker : "");
   }

   public static bool KupdobOIBalreadyExists(XSqlConnection conn, string kupdobOIB, uint kupdobCD, bool isPrjkt)
   {
      if(kupdobOIB.IsEmpty()) return false;

      VvDataRecord vvDataRecord;

      if(isPrjkt)  vvDataRecord = new Prjkt();
      else         vvDataRecord = new Kupdob();

      bool OK = ZXC.KupdobDao.SetMe_Record_bySomeUniqueColumn(conn, vvDataRecord, kupdobOIB, ZXC.KupdobSchemaRows[ZXC.KpdbCI.oib], false, true);

      if(OK && ((Kupdob)vvDataRecord).KupdobCD != kupdobCD)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "UPOZORENJE:\n\nOIB [{0}] u datoteci veæ postoji!\n\n[{1} / {2} / {3}]",
            kupdobOIB, ((Kupdob)vvDataRecord).KupdobCD, ((Kupdob)vvDataRecord).Ticker, ((Kupdob)vvDataRecord).Naziv);
      }

      return OK;
   }

   public static bool KupdobTICKER_ForPerson_RalreadyExists(XSqlConnection conn, string kupdobTICKER, /*uint kupdobCD,*/ bool isPrjkt)
   {
      if(kupdobTICKER.IsEmpty()) return true;

      VvDataRecord vvDataRecord;

      if(isPrjkt)  vvDataRecord = new Prjkt();
      else         vvDataRecord = new Kupdob();

      bool OK = ZXC.KupdobDao.SetMe_Record_bySomeUniqueColumn(conn, vvDataRecord, kupdobTICKER, ZXC.KupdobSchemaRows[ZXC.KpdbCI.ticker], false, true);

      //if(OK && ((Kupdob)vvDataRecord).KupdobCD != kupdobCD)
      //{
      //   ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "UPOZORENJE:\n\nOIB [{0}] u datoteci veæ postoji!\n\n[{1} / {2} / {3}]",
      //      kupdobTICKER, ((Kupdob)vvDataRecord).KupdobCD, ((Kupdob)vvDataRecord).Ticker, ((Kupdob)vvDataRecord).Naziv);
      //}

      return OK;
   }

   public static bool KupdobTICKER_AlreadyExists(XSqlConnection conn, string kupdobTICKER, uint kupdobCD, bool isPrjkt)
   {
      if(kupdobTICKER.IsEmpty()) return true;

      VvDataRecord vvDataRecord;

      if(isPrjkt) vvDataRecord = new Prjkt();
      else        vvDataRecord = new Kupdob();

      bool tickerExists = ZXC.KupdobDao.SetMe_Record_bySomeUniqueColumn(conn, vvDataRecord, kupdobTICKER, ZXC.KupdobSchemaRows[ZXC.KpdbCI.ticker], false, true);

      if(tickerExists)
      {
         if(((Kupdob)vvDataRecord).KupdobCD != kupdobCD)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "GREŠKA:\n\nTICKER [{0}] u datoteci veæ postoji!\n\n[{1} / {2} / {3}]",
               kupdobTICKER, ((Kupdob)vvDataRecord).KupdobCD, ((Kupdob)vvDataRecord).Ticker, ((Kupdob)vvDataRecord).Naziv);
         }
         else
         {
            tickerExists = false;
         }
      }

      return tickerExists;
   }

#if BiloJednom

   public static ZXC.AMSstatus RefreshKupdob_AMSstatus(XSqlConnection dbConnection, Kupdob kupdob_rec)
   {
      if(!ZXC.IsF2_2026_rules) return ZXC.AMSstatus.U_AMSu_JE;

      // idemo u refresh AMSstatusa i za nepoznat i za NIJE_U_AMSu status 
      // tj. samo ako je U_AMSu_JE, prestajemo refreshati ovog kupdob-a   
      if(kupdob_rec.AMSstatus == ZXC.AMSstatus.U_AMSu_JE /*||
         kupdob_rec.AMSstatus == ZXC.AMSstatus.NIJE_U_AMSu*/) return kupdob_rec.AMSstatus;

      WebApiResult<VvMER_Response_Data_AllActions> webApiResult; // za MER 

      bool refreshStatusOK = true;

      ZXC.AMSstatus refreshedAMSstatus = ZXC.AMSstatus.NEPOZNAT;

      switch(ZXC.F2_TheProvider)
      {
         case ZXC.F2_Provider_enum.MER:

            try
            {
               webApiResult = Vv_eRacun_HTTP.VvMER_WebService_CheckAMS(kupdob_rec.Oib);

               if(webApiResult == null || webApiResult.StatusCode == null)
               {
                  refreshStatusOK = false;
               }
               else if(webApiResult.ExceptionMessage.NotEmpty() || webApiResult.StatusCode != (int)System.Net.HttpStatusCode.OK)
               {
                  Vv_eRacun_HTTP.Show_WebApiResult_ErrorMessageBox(webApiResult, ZXC.F2_WebApi.CheckAMS);
                  refreshStatusOK = false;
               }

               if(refreshStatusOK)
               {
                  switch(webApiResult.StatusCode)
                  {
                     case (int)System.Net.HttpStatusCode.OK      : refreshedAMSstatus = ZXC.AMSstatus.U_AMSu_JE  ; break;
                     case (int)System.Net.HttpStatusCode.NotFound: refreshedAMSstatus = ZXC.AMSstatus.NIJE_U_AMSu; break;
                     default                                     : refreshedAMSstatus = ZXC.AMSstatus.NEPOZNAT   ; break;
                  }
               }
            }
            catch(Exception ex)
            {
               refreshStatusOK = false;
            }
            break;

         case ZXC.F2_Provider_enum.PND:

            try
            {
               webApiResult = Vv_eRacun_HTTP.VvPND_WebService_CheckAMS(kupdob_rec.Oib);

               if(webApiResult == null || webApiResult.ResponseData == null)
               {
                  refreshStatusOK = false;
               }
               else
               {
                  refreshedAMSstatus = (bool)webApiResult.ResponseData.publishedOnAms ? ZXC.AMSstatus.U_AMSu_JE : ZXC.AMSstatus.NIJE_U_AMSu;
               }
            }
            catch(Exception ex)
            {
               refreshStatusOK = false;
            }
            break;
      }

      #region RWTREC Kupdob with new AMSstatus

      if(ZXC.IsF2_2026_rules && refreshStatusOK)
      {
         kupdob_rec.BeginEdit();

         kupdob_rec.AMSstatus = refreshedAMSstatus;

         kupdob_rec.VvDao.RWTREC(ZXC.TheVvForm.TheDbConnection, kupdob_rec, false, false);

         kupdob_rec.EndEdit();

         //SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);
      }

      #endregion RWTREC Kupdob with new AMSstatus

      if(refreshStatusOK) return kupdob_rec.AMSstatus  ;
      else                return ZXC.AMSstatus.NEPOZNAT; 
   }

#endif

   public static ZXC.F2_R1enum GetMandatory_Kupdob_R1enum_FromDialog(XSqlConnection dbConnection, Kupdob kupdob_rec)
   {
      if(!ZXC.IsF2_2026_rules) return ZXC.F2_R1enum.B2B;

      bool refreshR1StatusOK = true;
      ZXC.F2_R1enum refreshedR1status = ZXC.F2_R1enum.Nepoznato;

      // Dialog ovo ono ... 
      // refreshedR1status = dlg.Fld_R1kind;

      #region RWTREC Kupdob with new R1status

      if(ZXC.IsF2_2026_rules && refreshR1StatusOK)
      {
         kupdob_rec.BeginEdit();

         kupdob_rec.R1kind = refreshedR1status;

         kupdob_rec.VvDao.RWTREC(ZXC.TheVvForm.TheDbConnection, kupdob_rec, false, false);

         kupdob_rec.EndEdit();

         //SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);
      }

      #endregion RWTREC Kupdob with new R1status

      if(refreshR1StatusOK) return kupdob_rec.R1kind      ;
      else                  return ZXC.F2_R1enum.Nepoznato; 
   }


   #endregion Utils


   #region Set_IMPORT_OFFIX_Columns

   //  //____ Specifics 2 start ______________________________________________________

   //fprintf(device, "%s\t", artikl_rec[0].k_kupdob_cd);
   //fprintf(device, "%s\t", artikl_rec[0].k_kupdob);
   //fprintf(device, "%s\t", artikl_rec[0].k_adr2);
   //fprintf(device, "%s\t", artikl_rec[0].k_prezime);
   //fprintf(device, "%s\t", artikl_rec[0].k_ime);
   //fprintf(device, "%s\t", artikl_rec[0].k_adr1);
   //fprintf(device, "%s\t", artikl_rec[0].k_grupa);
   //fprintf(device, "%s\t", artikl_rec[0].k_zip);
   //fprintf(device, "%s\t", artikl_rec[0].k_matbr);
   //fprintf(device, "%s\t", artikl_rec[0].k_sif_cd);
   //fprintf(device, "%s\t", artikl_rec[0].k_ziro[0]);
   //fprintf(device, "%s\t", artikl_rec[0].k_banka[0]);
   //fprintf(device, "%s\t", artikl_rec[0].k_poziv[0]);
   //fprintf(device, "%s\t", artikl_rec[0].k_ziro[1]);
   //fprintf(device, "%s\t", artikl_rec[0].k_banka[1]);
   //fprintf(device, "%s\t", artikl_rec[0].k_poziv[1]);
   //fprintf(device, "%s\t", artikl_rec[0].k_ziro[2]);
   //fprintf(device, "%s\t", artikl_rec[0].k_banka[2]);
   //fprintf(device, "%s\t", artikl_rec[0].k_poziv[2]);
   //fprintf(device, "%s\t", artikl_rec[0].k_ziro[3]);
   //fprintf(device, "%s\t", artikl_rec[0].k_banka[3]);
   //fprintf(device, "%s\t", artikl_rec[0].k_poziv[3]);
   //fprintf(device, "%s\t", artikl_rec[0].k_opcina);
   //fprintf(device, "%s\t", artikl_rec[0].k_opcina_cd);
   //fprintf(device, "%s\t", artikl_rec[0].k_adr3);
   //fprintf(device, "%s\t", artikl_rec[0].k_tel1);
   //fprintf(device, "%s\t", artikl_rec[0].k_tel2);
   //fprintf(device, "%s\t", artikl_rec[0].k_fax);
   //fprintf(device, "%s\t", artikl_rec[0].k_email);
   //fprintf(device, "%s\t", artikl_rec[0].k_www);
   //fprintf(device, "%s\t", artikl_rec[0].k_napomena1);
   //fprintf(device, "%s\t", artikl_rec[0].k_napomena2);
   //fprintf(device, "%s\t", artikl_rec[0].k_napomena3);
   //fprintf(device, "%s\t", artikl_rec[0].k_napomena4);
   //fprintf(device, "%s\t", artikl_rec[0].k_ugovor1);
   //fprintf(device, "%s\t", artikl_rec[0].k_ugovor2);
   //fprintf(device, "%s\t", artikl_rec[0].k_pnb0p);
   //fprintf(device, "%s\t", artikl_rec[0].k_ugovor3);
   //fprintf(device, "%s\t", artikl_rec[0].k_pnb0);
   //fprintf(device, "%s\t", artikl_rec[0].k_ugovor4);
   //fprintf(device, "%s\t", artikl_rec[0].k_oib);
   //fprintf(device, "%s\t", artikl_rec[0].k_konto);
   //fprintf(device, "%s\t", artikl_rec[0].k_flag1); // u PDV-u 
   //fprintf(device, "%s\t", artikl_rec[0].k_obrt);
   //fprintf(device, "%.2lf\t", artikl_rec[0].k_rabat1);
   //fprintf(device, "%.2lf\t", artikl_rec[0].k_rabat2);
   //fprintf(device, "%.2lf\t", artikl_rec[0].k_cssc);
   //fprintf(device, "%.2lf\t", artikl_rec[0].k_srabat);
   //fprintf(device, "%.2lf\t", artikl_rec[0].k_valuta);
   //fprintf(device, "%.2lf\t", artikl_rec[0].k_rok);
   //fprintf(device, "%.2lf\t", artikl_rec[0].k_delta); // % proviz banci 


   //fprintf(device, "\n");

   //  //____ Specifics 2 end   ______________________________________________________

 //public /*override*/ string Set_IMPORT_OFFIX_Columns_ORIG() // ORIG! 
   public   override   string Set_IMPORT_OFFIX_Columns     () // ORIG! 
   {
      return

         "(" +

         "@KCD, "       + //fprintf(device, "%s\t", artikl_rec[0].k_kupdob_cd);
         "naziv, "      + //fprintf(device, "%s\t", artikl_rec[0].k_kupdob);
         "grad, "       + //fprintf(device, "%s\t", artikl_rec[0].k_adr2);
         "prezime, "    + //fprintf(device, "%s\t", artikl_rec[0].k_prezime);
         "ime, "        + //fprintf(device, "%s\t", artikl_rec[0].k_ime);
         "ulica1, "     + //fprintf(device, "%s\t", artikl_rec[0].k_adr1);
         "tip, "        + //fprintf(device, "%s\t", artikl_rec[0].k_grupa);
         "postaBr, "    + //fprintf(device, "%s\t", artikl_rec[0].k_zip);
         "matbr, "      + //fprintf(device, "%s\t", artikl_rec[0].k_matbr);
         "sifDcd, "     + //fprintf(device, "%s\t", artikl_rec[0].k_sif_cd);
         "ziro1, "      + //fprintf(device, "%s\t", artikl_rec[0].k_ziro[0]);
         "ziro1By, "    + //fprintf(device, "%s\t", artikl_rec[0].k_banka[0]);
         "ziro1PnbV, "  + //fprintf(device, "%s\t", artikl_rec[0].k_poziv[0]);
         "ziro2, "      + //fprintf(device, "%s\t", artikl_rec[0].k_ziro[1]);
         "ziro2By, "    + //fprintf(device, "%s\t", artikl_rec[0].k_banka[1]);
         "ziro2PnbV, "  + //fprintf(device, "%s\t", artikl_rec[0].k_poziv[1]);
         "ziro3, "      + //fprintf(device, "%s\t", artikl_rec[0].k_ziro[2]);
         "ziro3By, "    + //fprintf(device, "%s\t", artikl_rec[0].k_banka[2]);
         "ziro3PnbV, "  + //fprintf(device, "%s\t", artikl_rec[0].k_poziv[2]);
         "ziro4, "      + //fprintf(device, "%s\t", artikl_rec[0].k_ziro[3]);
         "ziro4By, "    + //fprintf(device, "%s\t", artikl_rec[0].k_banka[3]);
         "ziro4PnbV, "  + //fprintf(device, "%s\t", artikl_rec[0].k_poziv[3]);
         "opcina, "     + //fprintf(device, "%s\t", artikl_rec[0].k_opcina);
         "opcCd, "      + //fprintf(device, "%s\t", artikl_rec[0].k_opcina_cd);
         "ulica2, "     + //fprintf(device, "%s\t", artikl_rec[0].k_adr3);
         "tel1, "       + //fprintf(device, "%s\t", artikl_rec[0].k_tel1);
         "tel2, "       + //fprintf(device, "%s\t", artikl_rec[0].k_tel2);
         "fax, "        + //fprintf(device, "%s\t", artikl_rec[0].k_fax);
         "email, "      + //fprintf(device, "%s\t", artikl_rec[0].k_email);
         "url, "        + //fprintf(device, "%s\t", artikl_rec[0].k_www);
         "napom1, "     + //fprintf(device, "%s\t", artikl_rec[0].k_napomena1);
         "napom2, "     + //fprintf(device, "%s\t", artikl_rec[0].k_napomena2);
         "@napomena3, " + //fprintf(device, "%s\t", artikl_rec[0].k_napomena3);
         "@napomena4, " + //fprintf(device, "%s\t", artikl_rec[0].k_napomena4);
         "fuse1, "      + //fprintf(device, "%s\t", artikl_rec[0].k_ugovor1);
         "fuse2, "      + //fprintf(device, "%s\t", artikl_rec[0].k_ugovor2);
         "pnbMProv, "   + //fprintf(device, "%s\t", artikl_rec[0].k_pnb0p);
         "pnbVProv, "   + //fprintf(device, "%s\t", artikl_rec[0].k_ugovor3);
         "pnbMPlaca, "  + //fprintf(device, "%s\t", artikl_rec[0].k_pnb0);
         "pnbVPlaca, "  + //fprintf(device, "%s\t", artikl_rec[0].k_ugovor4);
         "oib, "        + //fprintf(device, "%s\t", artikl_rec[0].k_oib);
         "kontoDug, "   + //fprintf(device, "%s\t", artikl_rec[0].k_konto);
         "@isPdv, "     + //fprintf(device, "%s\t", artikl_rec[0].k_flag1); // u PDV-u 
         "@isObrt, "    + //fprintf(device, "%s\t", artikl_rec[0].k_obrt);
         "stRbt1, "     + //fprintf(device, "%.2lf\t", artikl_rec[0].k_rabat1);
         "stRbt2, "     + //fprintf(device, "%.2lf\t", artikl_rec[0].k_rabat2);
         "stCsSc, "     + //fprintf(device, "%.2lf\t", artikl_rec[0].k_cssc);
         "stSRbt, "     + //fprintf(device, "%.2lf\t", artikl_rec[0].k_srabat);
         "valutaPl, "   + //fprintf(device, "%.2lf\t", artikl_rec[0].k_valuta);
         "rokOtprm, "   + //fprintf(device, "%.2lf\t", artikl_rec[0].k_rok);
         "finLimit, "   + //fprintf(device, "%.2lf\t", kupdob_rec[0].k_alfa); finLimit dodadno 07.12.2011! 
         "stProviz, "   + //fprintf(device, "%.2lf\t", artikl_rec[0].k_delta); // % proviz banci 
         "centrID   "   + //fprintf(device, "%s\t", kupdob_rec[0].k_kupdob_cd); // CENTRALA ID!

         ")"    + "\n" +

         "SET " + "\n" +

         "kupdobCD = @KCD, " + "\n" +

         "ticker = UPPER(CONCAT(SUBSTRING(naziv, 1, 3), SUBSTRING(@KCD, 4, 3))), " + "\n" +

         "isPdv  = IF(@isPdv  = 'D', 1, 0), " + "\n" +
         "isObrt = IF(@isObrt = 'X', 1, 0), " + "\n" +

         "komentar = CONCAT(@napomena3, ' ', @napomena4), " + "\n" +

         "addTS = CURRENT_TIMESTAMP, modTS = addTS," + "\n" +
         "addUID = '" + ZXC.vvDB_User + "'";
   }

   public /*override*/ string Set_IMPORT_OFFIX_Columns_SvDUH2018() // 2018: SvDUH 
   {
      return

         "(" +

         "@KCD, "       + //fprintf(device, "%s\t", artikl_rec[0].k_kupdob_cd);                            kupdob_rec[0].k_kupdob_cd);
         "naziv, "      + //fprintf(device, "%s\t", artikl_rec[0].k_kupdob);                               kupdob_rec[0].k_kupdob);
         "grad, "       + //fprintf(device, "%s\t", artikl_rec[0].k_adr2);                                 kupdob_rec[0].k_adr2);
         "prezime, "    + //fprintf(device, "%s\t", artikl_rec[0].k_prezime);                              kupdob_rec[0].k_prezime);
         "ime, "        + //fprintf(device, "%s\t", artikl_rec[0].k_ime);                                  kupdob_rec[0].k_ime);
         "ulica1, "     + //fprintf(device, "%s\t", artikl_rec[0].k_adr1);                                 kupdob_rec[0].k_adr1);
         "tip, "        + //fprintf(device, "%s\t", artikl_rec[0].k_grupa);                                kupdob_rec[0].k_grupa);
         "postaBr, "    + //fprintf(device, "%s\t", artikl_rec[0].k_zip);                                  kupdob_rec[0].k_zip);
         "matbr, "      + //fprintf(device, "%s\t", artikl_rec[0].k_matbr);                                kupdob_rec[0].k_matbr);
         "sifDcd, "     + //fprintf(device, "%s\t", artikl_rec[0].k_sif_cd);                               kupdob_rec[0].k_sif_cd);
         "ziro1, "      + //fprintf(device, "%s\t", artikl_rec[0].k_ziro[0]);                              kupdob_rec[0].k_ziro[0]);
         "ziro1By, "    + //fprintf(device, "%s\t", artikl_rec[0].k_banka[0]);                             kupdob_rec[0].k_banka[0]);
         "ziro1PnbV, "  + //fprintf(device, "%s\t", artikl_rec[0].k_poziv[0]);                             kupdob_rec[0].k_poziv[0]);
         "ziro2, "      + //fprintf(device, "%s\t", artikl_rec[0].k_ziro[1]);                              kupdob_rec[0].k_ziro[1]);
         "ziro2By, "    + //fprintf(device, "%s\t", artikl_rec[0].k_banka[1]);                             kupdob_rec[0].k_banka[1]);
         "ziro2PnbV, "  + //fprintf(device, "%s\t", artikl_rec[0].k_poziv[1]);                             kupdob_rec[0].k_poziv[1]);
         "ziro3, "      + //fprintf(device, "%s\t", artikl_rec[0].k_ziro[2]);                              kupdob_rec[0].k_ziro[2]);
         "ziro3By, "    + //fprintf(device, "%s\t", artikl_rec[0].k_banka[2]);                             kupdob_rec[0].k_banka[2]);
         "ziro3PnbV, "  + //fprintf(device, "%s\t", artikl_rec[0].k_poziv[2]);                             kupdob_rec[0].k_poziv[2]);
         "ziro4, "      + //fprintf(device, "%s\t", artikl_rec[0].k_ziro[3]);                              kupdob_rec[0].k_ziro[3]);
         "ziro4By, "    + //fprintf(device, "%s\t", artikl_rec[0].k_banka[3]);                             kupdob_rec[0].k_banka[3]);
         "ziro4PnbV, "  + //fprintf(device, "%s\t", artikl_rec[0].k_poziv[3]);                             kupdob_rec[0].k_poziv[3]);
         "opcina, "     + //fprintf(device, "%s\t", artikl_rec[0].k_opcina);                               kupdob_rec[0].k_opcina);
         "opcCd, "      + //fprintf(device, "%s\t", artikl_rec[0].k_opcina_cd);                            kupdob_rec[0].k_opcina_cd);
         "ulica2, "     + //fprintf(device, "%s\t", artikl_rec[0].k_adr3);                                 kupdob_rec[0].k_adr3);
         "tel1, "       + //fprintf(device, "%s\t", artikl_rec[0].k_tel1);                                 kupdob_rec[0].k_tel1);
         "tel2, "       + //fprintf(device, "%s\t", artikl_rec[0].k_tel2);                                 kupdob_rec[0].k_tel2);
         "fax, "        + //fprintf(device, "%s\t", artikl_rec[0].k_fax);                                  kupdob_rec[0].k_fax);
         "email, "      + //fprintf(device, "%s\t", artikl_rec[0].k_email);                                kupdob_rec[0].k_email);
         "url, "        + //fprintf(device, "%s\t", artikl_rec[0].k_www);                                  kupdob_rec[0].k_www);
         "napom1, "     + //fprintf(device, "%s\t", artikl_rec[0].k_napomena1);                            kupdob_rec[0].k_napomena1);
         "napom2, "     + //fprintf(device, "%s\t", artikl_rec[0].k_napomena2);                            kupdob_rec[0].k_napomena2);
         "@napomena3, " + //fprintf(device, "%s\t", artikl_rec[0].k_napomena3);                            kupdob_rec[0].k_napomena3);
         "@napomena4, " + //fprintf(device, "%s\t", artikl_rec[0].k_napomena4);                            kupdob_rec[0].k_napomena4);
         "finLimit  "   + //fprintf(device, "%.2lf\t", kupdob_rec[0].k_alfa); finLimit dodadno 07.12.2011! kupdob_rec[0].k_alfa); // Limit !!! TODO: !!! 4 INTRADE export to Vektor (mozda da ga puknes u 'regob' col Vektorovog Kupdob-a  

         ")"    + "\n" +

         "SET " + "\n" +

         "kupdobCD = @KCD, " + "\n" +

         "ticker = UPPER(CONCAT(SUBSTRING(naziv, 1, 3), SUBSTRING(@KCD, 4, 3))), " + "\n" +

         "isPdv  = IF(@isPdv  = 'D', 1, 0), " + "\n" +
         "isObrt = IF(@isObrt = 'X', 1, 0), " + "\n" +

         "komentar = CONCAT(@napomena3, ' ', @napomena4), " + "\n" +

         "addTS = CURRENT_TIMESTAMP, modTS = addTS," + "\n" +
         "addUID = '" + ZXC.vvDB_User + "'";
   }

   public /*override*/ string Set_IMPORT_OFFIX_Columns_SENSO() // SENSO Plus d.o.o. 
   {
      return

         "(" +

         "@KCD     ," + 
         "@naziv   ," +
         "@isKup   ," +
         "@isDob   ," +
         "@ulica1  ," +
         "@postaBr ," +
         "drzava   ," +
         "matbr    ," +
         "@vatDummy," +
         "tel1     ," +
         "fax      ," +
         "valutaPl ," +
         "finLimit ," +
         "devName  ," +
         "@npDummy ," +
         "swift    ," +
         "@napom1  ," +
         "oib      ," +
         "ziro1     " +

// PAZI!!! da zadnji NEMA ZAREZ

         ")"    + "\n" +

         "SET " + "\n" +

         "naziv = RTRIM(@naziv), " + "\n" +

         "kupdobCD = @KCD, "       + "\n" +
         "ulica1   = @ulica1, "    + "\n" +
         "ulica2   = @ulica1, "    + "\n" +
         "napom1   = @napom1, "    + "\n" +
         "email    = @napom1, "    + "\n" +
         "postaBr  = @postaBr,"    + "\n" +
         "grad     = IF(@postaBr = '10000', 'Zagreb', ''), " + "\n" +

         //"tip = CONCAT(IF(@isKup  = 'da', 'K', ''), IF(@isDob  = 'da', 'D', '')), " + "\n" +
         "tip = CONCAT(@isKup, @isDob), " + "\n" +
         //"ticker = CONCAT(SUBSTRING(naziv, 1, 3), SUBSTRING(@KCD, 4, 3)), " + "\n" +
         "ticker = UPPER(CONCAT(SUBSTRING(naziv, 1, 5), SUBSTRING(@KCD, -1, 1))), " + "\n" +


         "addTS = CURRENT_TIMESTAMP, modTS = addTS," + "\n" +
         "addUID = '" + ZXC.vvDB_User + "'";
   }

   public /*override*/ string Set_IMPORT_OFFIX_Columns_Mandaric() // Mandadric VRATI NA ORIG KADA SI GIOTOV!!!
   {
      return

         "(" +

      //"@Prvi	           ," +
      "@Broj	           ," +
      "@StrBroj	        ," +
      "@Ime_firme	     ," +
      "@Tip	           ," +
      "@Ime_partnera	  ," +
      "@Sjediste	     ," +
      "@Adresa	        ," +
      "@Ziroracun	     ," +
      "@Tel	           ," +
      "@Fax	           ," +
      "@Ime_osobe	     ," +
      "@Matbr	        ," +
      "@Post_broj	     ," +
      "@Drzava	        ," +
      "@Banka	        ," +
      "@RacBank	        ," +
      "@RabKat	        ," +
      "@NacinPlac	     ," +
      "@OsigurPlac	     ," +
      "@PocSt	        ," +
      "@Opis	           ," +
      "@Email	        ," +
      "@Banka2	        ," +
      "@Www	           ," +
      "@PJedZap	        ," +
      "@PZrnNal	        ," +
      "@PVodBrDi	     ," +
      "@PZrnNDI	        ," +
      "@PTipZrn	        ," +
      "@RBkom	        ," +
      "@NazivKom	     ," +
      "@RegBroj	        ," +
      "@Sifradjel	     ," +
      "@ObvPDVa	        ," +
      "@Poljoprivrednik,	" +
      "@OIB             " +

// PAZI!!! da zadnji NEMA ZAREZ

         ")" + "\n" +

         "SET " + "\n" +

         "kupdobCD   = @Broj + IF(@Tip=1, 10000, 20000), " + "\n" +
         "tip        = IF(@Tip=1, 'K', 'D'), " + "\n" +
         "naziv      = RTRIM(@Ime_partnera), " + "\n" +
         "grad       = @Sjediste    , " + "\n" +
         "ulica1     = @Adresa      , " + "\n" +
         "ulica2     = @Adresa      , " + "\n" +
         "ziro1      = @Ziroracun   , " + "\n" +
         "tel1       = @Tel         , " + "\n" +
         "fax        = @Fax         , " + "\n" +
         "ime        = @Ime_osobe   , " + "\n" +
         "matbr      = @Matbr       , " + "\n" +
         "postaBr    = @Post_broj   , " + "\n" +
         "drzava     = @Drzava      , " + "\n" +
         "ziro1By    = @Banka       , " + "\n" +
         "ziro1PnbV  = @RacBank     , " + "\n" +
         "oib        = @OIB         , " + "\n" +

         //"ticker = CONCAT(SUBSTRING(naziv, 1, 3), SUBSTRING(@KCD, 4, 3)), " + "\n" +
         "ticker = UPPER(CONCAT(SUBSTRING(naziv, 1, 5), SUBSTRING(kupdobCD, -1, 1))), " + "\n" +


         "addTS = CURRENT_TIMESTAMP, modTS = addTS," + "\n" +
         "addUID = '" + ZXC.vvDB_User + "'";
   }

   internal static void ImportFromOffix_Translate437(XSqlConnection conn)
   {
      int debugCount;

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Kupdob>(conn, Translate437, null, "", ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out debugCount);
   }

   static bool Translate437(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      Kupdob kupdob_rec = vvDataRecord as Kupdob;

      kupdob_rec.Ticker  = kupdob_rec.Ticker  .VvTranslate437ToLatin2().Replace("SUM", "ŠUM");
      kupdob_rec.Naziv   = kupdob_rec.Naziv   .VvTranslate437ToLatin2().Replace("SUM", "ŠUM");
      kupdob_rec.Ulica1  = kupdob_rec.Ulica1  .VvTranslate437ToLatin2();
    //kupdob_rec.Ulica2  = kupdob_rec.Ulica2  .VvTranslate437ToLatin2();
      kupdob_rec.Ulica2  = kupdob_rec.Ulica1  ; 
      kupdob_rec.DugoIme = kupdob_rec.DugoIme .VvTranslate437ToLatin2();
      kupdob_rec.Grad    = kupdob_rec.Grad    .VvTranslate437ToLatin2();
      kupdob_rec.Ime     = kupdob_rec.Ime     .VvTranslate437ToLatin2();
      kupdob_rec.Opcina  = kupdob_rec.Opcina  .VvTranslate437ToLatin2();
      kupdob_rec.Prezime = kupdob_rec.Prezime .VvTranslate437ToLatin2();
      kupdob_rec.Ziro1By = kupdob_rec.Ziro1By .VvTranslate437ToLatin2();
      kupdob_rec.Ziro2By = kupdob_rec.Ziro2By .VvTranslate437ToLatin2();
      kupdob_rec.Ziro3By = kupdob_rec.Ziro3By .VvTranslate437ToLatin2();
      kupdob_rec.Ziro4By = kupdob_rec.Ziro4By .VvTranslate437ToLatin2();
      kupdob_rec.Zupan   = kupdob_rec.Zupan   .VvTranslate437ToLatin2();
      kupdob_rec.Tip     = kupdob_rec.Tip     .VvTranslate437ToLatin2();
      kupdob_rec.Napom1  = kupdob_rec.Napom1  .VvTranslate437ToLatin2();
      kupdob_rec.Napom2  = kupdob_rec.Napom2  .VvTranslate437ToLatin2();
      kupdob_rec.Komentar= kupdob_rec.Komentar.VvTranslate437ToLatin2();
      
      return kupdob_rec.EditedHasChanges();
   }

   #endregion Set_IMPORT_OFFIX_Columns

}
