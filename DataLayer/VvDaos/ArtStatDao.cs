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
using System.Data.SqlTypes;
#endif

public sealed class ArtStatDao : VvDaoBase, IVvDao
{
   
   #region Singleton Constructor & instancer

   private static ArtStatDao instance;

   private ArtStatDao(XSqlConnection conn, string dbName) : base(dbName, ArtStat.recordName/*Arhiva*/, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static ArtStatDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if (instance == null)
      {
         instance = new ArtStatDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableArtStat

   public static   uint TableVersionStatic { get { return 24; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition(bool isArhiva)
   {
      return 
      (
        " recID int(10)               unsigned NOT NULL auto_increment,\n" +

/* 01 */" addTS timestamp                 default '0000-00-00 00:00:00'  ,\n" +
/* 02 */" addUID varchar(16)     NOT NULL default 'XY'                   ,\n" +
/* 03 */" rtransRecID      int(10)        unsigned NOT NULL default '0',\n" +   
/* 04 */" t_artiklCD       varchar(32)             NOT NULL default '' ,\n" +
/* 05 */" t_skladCD        varchar( 6)             NOT NULL default '' ,\n" +     
/* 06 */" t_skladDate      date                    NOT NULL default '0001-01-01',\n" +
/* 07 */" t_tt             char(3)                 NOT NULL default ''    ,\n" +
/* 08 */" t_ttSort         tinyint(4)              NOT NULL default '0'   ,\n" +    
/* 09 */" t_ttNum          int(10)        unsigned NOT NULL default '0'   ,\n" +    
/* 10 */" t_serial         smallint(5)    unsigned NOT NULL default '0'   ,\n" +   
/* 11 */" transRbr         int(10)        unsigned NOT NULL default '0'   ,\n" +   
/* 12 */" pstFinNBC        decimal(12,4)           NOT NULL default '0.00',\n" +
/* 13 */" ulazFinNBC       decimal(12,4)           NOT NULL default '0.00',\n" +
/* 14 */" izlazFinNBC      decimal(12,4)           NOT NULL default '0.00',\n" +
/* 15 */" ulazFirmaFinNBC  decimal(12,4)           NOT NULL default '0.00',\n" +
/* 16 */" izlazFirmaFinNBC decimal(12,4)           NOT NULL default '0.00',\n" +
/* 17 */" lastPrNabCij     decimal(16,8)           NOT NULL default '0.00',\n" +
/* 18 */" pstFinMPC        decimal(12,4)           NOT NULL default '0.00',\n" +
/* 19 */" ulazFinMPC       decimal(12,4)           NOT NULL default '0.00',\n" +
/* 20 */" izlazFinMPC      decimal(12,4)           NOT NULL default '0.00',\n" +
/* 21 */" ulazFirmaFinMPC  decimal(12,4)           NOT NULL default '0.00',\n" +
/* 22 */" izlazFirmaFinMPC decimal(12,4)           NOT NULL default '0.00',\n" +
/* 23 */" lastMalopCij     decimal(12,4)           NOT NULL default '0.00',\n" +
/* 24 */" pstKol           decimal(12,4)           NOT NULL default '0.00',\n" +
/* 25 */" invKol           decimal(12,4)           NOT NULL default '0.00',\n" +
/* 26 */" invFin           decimal(12,4)           NOT NULL default '0.00',\n" + // invFinNBC 
/* 27 */" ulazKol          decimal(12,4)           NOT NULL default '0.00',\n" +
/* 28 */" ulazFinKCR       decimal(12,4)           NOT NULL default '0.00',\n" +
/* 29 */" ulazCijMin       decimal(12,4)           NOT NULL default '0.00',\n" +
/* 30 */" ulazCijMax       decimal(12,4)           NOT NULL default '0.00',\n" +
/* 31 */" ulazCijLast      decimal(12,4)           NOT NULL default '0.00',\n" +
/* 32 */" izlazKol         decimal(12,4)           NOT NULL default '0.00',\n" +
/* 33 */" stanjeKolRezerv  decimal(12,4)           NOT NULL default '0.00',\n" +
/* 34 */" izlFinProdKCR    decimal(12,4)           NOT NULL default '0.00',\n" +
/* 35 */" izlazCijMin      decimal(12,4)           NOT NULL default '0.00',\n" +
/* 36 */" izlazCijMax      decimal(12,4)           NOT NULL default '0.00',\n" +   
/* 37 */" izlazCijLast     decimal(12,4)           NOT NULL default '0.00',\n" +   
/* 38 */" preDefVpc1       decimal(12,4)           NOT NULL default '0.00',\n" +   
/* 39 */" preDefVpc2       decimal(12,4)           NOT NULL default '0.00',\n" +   
/* 40 */" preDefMpc1       decimal(12,4)           NOT NULL default '0.00',\n" +   
/* 41 */" preDefDevc       decimal(12,4)           NOT NULL default '0.00',\n" +   
/* 42 */" preDefRbt1       decimal( 5,2)           NOT NULL default '0.00',\n" +   
/* 43 */" preDefRbt2       decimal( 5,2)           NOT NULL default '0.00',\n" +   
/* 44 */" preDefMinKol     decimal(12,4)           NOT NULL default '0.00',\n" +   
/* 45 */" preDefMarza      decimal( 9,6)           NOT NULL default '0.00',\n" +   
/* 46 */" ulazKolFisycal   decimal(12,4)           NOT NULL default '0.00',\n" +
/* 47 */" izlazKolFisycal  decimal(12,4)           NOT NULL default '0.00',\n" +
/* 48 */" dateZadUlaz      date                    NOT NULL default '0001-01-01',\n" +
/* 49 */" dateZadIzlaz     date                    NOT NULL default '0001-01-01',\n" +
/* 50 */" dateZadPst       date                    NOT NULL default '0001-01-01',\n" +
/* 51 */" dateZadInv       date                    NOT NULL default '0001-01-01',\n" +
/* 52 */" artiklTS         varchar(6)              NOT NULL default '',    \n" +
/* 53 */" artiklJM         varchar(12)             NOT NULL default '',    \n" +
/* 54 */" frsMinTt         char(3)                 NOT NULL default ''    ,\n" +
/* 55 */" frsMinTtNum      int(10)        unsigned NOT NULL default '0'   ,\n" +    
/* 56 */" ulazFirmaKol     decimal(12,4)           NOT NULL default '0.00',\n" +
/* 57 */" izlazFirmaKol    decimal(12,4)           NOT NULL default '0.00',\n" +
/* 58 */" rezervKolNaruc   decimal(12,4)           NOT NULL default '0.00',\n" +
/* 59 */" rezervKolIsporu  decimal(12,4)           NOT NULL default '0.00',\n" +
/* 60 */" rtrPstKol         decimal(12,4)          NOT NULL default '0.00',\n" +
/* 61 */" rtrUlazKol        decimal(12,4)          NOT NULL default '0.00',\n" +
/* 62 */" rtrIzlazKol       decimal(12,4)          NOT NULL default '0.00',\n" +
/* 63 */" rtrUlazKolFisycal decimal(12,4)          NOT NULL default '0.00',\n" +
/* 64 */" rtrIzlzKolFisycal decimal(12,4)          NOT NULL default '0.00',\n" +
/* 65 */" rtrKolNaruceno    decimal(12,4)          NOT NULL default '0.00',\n" +
/* 66 */" rtrKolIsporuceno  decimal(12,4)          NOT NULL default '0.00',\n" +
/* 67 */" rtrPstVrjNBC      decimal(12,4)          NOT NULL default '0.00',\n" +
/* 68 */" rtrUlazVrjNBC     decimal(12,4)          NOT NULL default '0.00',\n" +
/* 69 */" rtrIzlazVrjNBC    decimal(12,4)          NOT NULL default '0.00',\n" +
/* 70 */" rtrPstCijNBC      decimal(16,8)          NOT NULL default '0.00',\n" +
/* 71 */" rtrUlazCijNBC     decimal(16,8)          NOT NULL default '0.00',\n" +
/* 72 */" rtrIzlazCijNBC    decimal(16,8)          NOT NULL default '0.00',\n" +
/* 73 */" rtrCijenaNBC      decimal(16,8)          NOT NULL default '0.00',\n" +
/* 74 */" rtrPstVrjMPC      decimal(12,4)          NOT NULL default '0.00',\n" +
/* 75 */" rtrUlazVrjMPC     decimal(12,4)          NOT NULL default '0.00',\n" +
/* 76 */" rtrIzlazVrjMPC    decimal(12,4)          NOT NULL default '0.00',\n" +
/* 77 */" rtrPstCijMPC      decimal(12,4)          NOT NULL default '0.00',\n" +
/* 78 */" rtrUlazCijMPC     decimal(12,4)          NOT NULL default '0.00',\n" +
/* 79 */" rtrIzlazCijMPC    decimal(12,4)          NOT NULL default '0.00',\n" +
/* 80 */" rtrCijenaMPC      decimal(12,4)          NOT NULL default '0.00',\n" +
/* 81 */" prevMalopCij      decimal(12,4)          NOT NULL default '0.00',\n" +
/* 82 */" orgPak            decimal(12,4)          NOT NULL default '0.00',\n" +
/* 83 */" orgPakJM          varchar(12)            NOT NULL default '',    \n" +
/* 84 */" ulazKol2          decimal(12,4)          NOT NULL default '0.00',\n" +
/* 85 */" izlazKol2         decimal(12,4)          NOT NULL default '0.00',\n" +
/* 86 */" pstKol2           decimal(12,4)          NOT NULL default '0.00',\n" +
/* 87 */" invKol2           decimal(12,4)          NOT NULL default '0.00',\n" +
/* 88 */" ulazFirmaKol2     decimal(12,4)          NOT NULL default '0.00',\n" +
/* 89 */" izlazFirmaKol2    decimal(12,4)          NOT NULL default '0.00',\n" +
/* 90 */" rtrPstKol2        decimal(12,4)          NOT NULL default '0.00',\n" +
/* 91 */" rtrUlazKol2       decimal(12,4)          NOT NULL default '0.00',\n" +
/* 92 */" rtrIzlazKol2      decimal(12,4)          NOT NULL default '0.00',\n" +
/* 93 */" artGrCd1          varchar(6)             NOT NULL default ''    ,\n" +     
/* 94 */" artGrCd2          varchar(6)             NOT NULL default ''    ,\n" +     
/* 95 */" artGrCd3          varchar(6)             NOT NULL default ''    ,\n" +     
/* 96 */" rtrPdvSt          decimal( 5,2)          NOT NULL default '0.00',\n" +
/* 97 */" rtrIsIrmUslug     tinyint(1)    unsigned NOT NULL default  0    ,\n" +
/* 98 */" rtrParentID       int(10)       unsigned NOT NULL default  0    ,\n" +
/* 99 */" invKolDiff        decimal(12,4)          NOT NULL default '0.00',\n" +
/*100 */" invKol2Diff       decimal(12,4)          NOT NULL default '0.00',\n" +
/*101 */" invFinDiff        decimal(12,4)          NOT NULL default '0.00',\n" + // invFinDiffNBC
/*102 */" invFinDiffMPC     decimal(12,4)          NOT NULL default '0.00',\n" +
/*103 */" invFinMPC         decimal(12,4)          NOT NULL default '0.00',\n" +
/*104 */" prNBCBefThisUlaz  decimal(12,4)          NOT NULL default '0.00',\n" +

         //CreateTable_ArhivaExtensionDefinition(isArhiva) +

         "PRIMARY KEY          (recID)      , \n" +
         "UNIQUE  KEY BY_rtrID (rtransRecID), \n" +
         "UNIQUE  KEY BY_artCD (t_artiklCD, t_skladCD, t_skladDate, t_ttSort, t_ttNum, t_serial), \n" +
         "INDEX   BY_fakID     (rtrParentID) \n"
      );
   }

   public static string Alter_table_definition(uint catchingVersion)
   {
      string tableName = ArtStat.recordName;

      switch(catchingVersion)
      {
         case 2: return ("VvRECREATE"); // Dakle, kada zelis force-ati regeneraciju cache-a samo gore inkrementiras 'TableVersion' i tu dodas noci case da returna "VvRECREATE" 
         case 3: return ("VvRECREATE"); // Dakle, kada zelis force-ati regeneraciju cache-a samo gore inkrementiras 'TableVersion' i tu dodas noci case da returna "VvRECREATE" 
         case 4: return ("ADD COLUMN addTS timestamp                 default '0000-00-00 00:00:00' AFTER recID, \n" +
                         "ADD COLUMN addUID varchar(16)     NOT NULL default 'XY'                  AFTER addTS; \n");
         case 5: 
         case 6:
         case 7: return ("VvRECREATE"); // Dakle, kada zelis force-ati regeneraciju cache-a samo gore inkrementiras 'TableVersion' i tu dodas noci case da returna "VvRECREATE" 
         case 8: return ("VvRECREATE"); // Dakle, kada zelis force-ati regeneraciju cache-a samo gore inkrementiras 'TableVersion' i tu dodas noci case da returna "VvRECREATE" 
         case 9: return ("VvRECREATE"); // Dakle, kada zelis force-ati regeneraciju cache-a samo gore inkrementiras 'TableVersion' i tu dodas noci case da returna "VvRECREATE" 

         case 10: return ("VvRECREATE"); // Dakle, kada zelis force-ati regeneraciju cache-a samo gore inkrementiras 'TableVersion' i tu dodas noci case da returna "VvRECREATE" 
         case 11: return ("VvRECREATE"); // Dakle, kada zelis force-ati regeneraciju cache-a samo gore inkrementiras 'TableVersion' i tu dodas noci case da returna "VvRECREATE" 
         case 12: return ("VvRECREATE"); // Dakle, kada zelis force-ati regeneraciju cache-a samo gore inkrementiras 'TableVersion' i tu dodas noci case da returna "VvRECREATE" 
         case 13: return ("VvRECREATE"); // Dakle, kada zelis force-ati regeneraciju cache-a samo gore inkrementiras 'TableVersion' i tu dodas noci case da returna "VvRECREATE" 

         case 14: return ("ADD COLUMN ulazKol2          decimal(12,4)          NOT NULL default '0.00' AFTER orgPakJM      , \n" +
                          "ADD COLUMN izlazKol2         decimal(12,4)          NOT NULL default '0.00' AFTER ulazKol2      , \n" +
                          "ADD COLUMN pstKol2           decimal(12,4)          NOT NULL default '0.00' AFTER izlazKol2     , \n" +
                          "ADD COLUMN invKol2           decimal(12,4)          NOT NULL default '0.00' AFTER pstKol2       , \n" +
                          "ADD COLUMN ulazFirmaKol2     decimal(12,4)          NOT NULL default '0.00' AFTER invKol2       , \n" +
                          "ADD COLUMN izlazFirmaKol2    decimal(12,4)          NOT NULL default '0.00' AFTER ulazFirmaKol2 , \n" +
                          "ADD COLUMN rtrPstKol2        decimal(12,4)          NOT NULL default '0.00' AFTER izlazFirmaKol2, \n" +
                          "ADD COLUMN rtrUlazKol2       decimal(12,4)          NOT NULL default '0.00' AFTER rtrPstKol2    , \n" +
                          "ADD COLUMN rtrIzlazKol2      decimal(12,4)          NOT NULL default '0.00' AFTER rtrUlazKol2   ; \n");

         case 15: return ("ADD COLUMN artGrCd1          varchar(6)             NOT NULL default '' AFTER rtrIzlazKol2     , \n" +
                          "ADD COLUMN artGrCd2          varchar(6)             NOT NULL default '' AFTER artGrCd1         , \n" +
                          "ADD COLUMN artGrCd3          varchar(6)             NOT NULL default '' AFTER artGrCd2         ; \n");

         case 16: return ("ADD COLUMN rtrPdvSt          decimal( 5,2)          NOT NULL default '0.00' AFTER artGrCd3     , \n" +
                          "ADD COLUMN rtrIsIrmUslug     tinyint(1)    unsigned NOT NULL default  0     AFTER rtrPdvSt     , \n" +
                          "ADD COLUMN rtrParentID       int(10)       unsigned NOT NULL default  0     AFTER rtrIsIrmUslug; \n");

         case 17: return ("ADD INDEX BY_fakID (rtrParentID);\n");

         case 18: return ("MODIFY COLUMN lastPrNabCij      decimal(16,8)          NOT NULL default '0.00' , \n" +
                          "MODIFY COLUMN rtrPstCijNBC      decimal(16,8)          NOT NULL default '0.00' , \n" +
                          "MODIFY COLUMN rtrUlazCijNBC     decimal(16,8)          NOT NULL default '0.00' , \n" +
                          "MODIFY COLUMN rtrIzlazCijNBC    decimal(16,8)          NOT NULL default '0.00' , \n" +
                          "MODIFY COLUMN rtrCijenaNBC      decimal(16,8)          NOT NULL default '0.00' ; \n");

         case 19: return ("ADD COLUMN invKolDiff        decimal(12,4)          NOT NULL default '0.00' AFTER rtrParentID, \n" +
                          "ADD COLUMN invKol2Diff       decimal(12,4)          NOT NULL default '0.00' AFTER invKolDiff , \n" +
                          "ADD COLUMN invFinDiff        decimal(12,4)          NOT NULL default '0.00' AFTER invKol2Diff; \n");

         case 20: return ("MODIFY COLUMN t_ttSort       tinyint(4)             NOT NULL default '0'  ");

         case 21: return ("ADD COLUMN invFinDiffMPC     decimal(12,4)          NOT NULL default '0.00' AFTER invFinDiff,    \n" +
                          "ADD COLUMN invFinMPC         decimal(12,4)          NOT NULL default '0.00' AFTER invFinDiffMPC; \n");

         case 22: return ("VvRECREATE"); // Dakle, kada zelis force-ati regeneraciju cache-a samo gore inkrementiras 'TableVersion' i tu dodas noci case da returna "VvRECREATE" 

         case 23: return ("ADD COLUMN prNBCBefThisUlaz     decimal(12,4)          NOT NULL default '0.00' AFTER invFinMPC;  \n");

         case 24: return ("VvRECREATE"); // Dakle, kada zelis force-ati regeneraciju cache-a samo gore inkrementiras 'TableVersion' i tu dodas noci case da returna "VvRECREATE" 

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableArtStat

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      ArtStat artstat = (ArtStat)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         //VvSQL.CreateCommandParameter(cmd, where_or_and, "prjktKupdobCD", artikl.RecID, XSqlDbType.Int32, 10);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RecID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   ||
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         //DataRow theRow = TheSchemaTable.Rows.Find("modUID");

         VvSQL.CreateCommandParameter(cmd, preffix, artstat.AddTS             , TheSchemaTable.Rows[CI.addTS            ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.AddUID            , TheSchemaTable.Rows[CI.addUID           ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtransRecID       , TheSchemaTable.Rows[CI.rtransRecID      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.ArtiklCD          , TheSchemaTable.Rows[CI.t_artiklCD       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.SkladCD           , TheSchemaTable.Rows[CI.t_skladCD        ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.SkladDate         , TheSchemaTable.Rows[CI.t_skladDate      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.TT                , TheSchemaTable.Rows[CI.t_tt             ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.TtSort            , TheSchemaTable.Rows[CI.t_ttSort         ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.TtNum             , TheSchemaTable.Rows[CI.t_ttNum          ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.Serial            , TheSchemaTable.Rows[CI.t_serial         ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.TransRbr          , TheSchemaTable.Rows[CI.transRbr         ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkPstFinNBC       , TheSchemaTable.Rows[CI.pstFinNBC        ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkUlazFinNBC      , TheSchemaTable.Rows[CI.ulazFinNBC       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkIzlazFinNBC     , TheSchemaTable.Rows[CI.izlazFinNBC      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkUlazFirmaFinNBC , TheSchemaTable.Rows[CI.ulazFirmaFinNBC  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkIzlazFirmaFinNBC, TheSchemaTable.Rows[CI.izlazFirmaFinNBC ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.LastPrNabCij      , TheSchemaTable.Rows[CI.lastPrNabCij     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkPstFinMPC       , TheSchemaTable.Rows[CI.pstFinMPC        ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkUlazFinMPC      , TheSchemaTable.Rows[CI.ulazFinMPC       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkIzlazFinMPC     , TheSchemaTable.Rows[CI.izlazFinMPC      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkUlazFirmaFinMPC , TheSchemaTable.Rows[CI.ulazFirmaFinMPC  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkIzlazFirmaFinMPC, TheSchemaTable.Rows[CI.izlazFirmaFinMPC ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.LastUlazMPC      , TheSchemaTable.Rows[CI.lastMalopCij     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkPstKol          , TheSchemaTable.Rows[CI.pstKol           ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.InvKol            , TheSchemaTable.Rows[CI.invKol           ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.InvFinNBC            , TheSchemaTable.Rows[CI.invFin           ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkUlazKol         , TheSchemaTable.Rows[CI.ulazKol          ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkUlazFinNBC      , TheSchemaTable.Rows[CI.ulazFinKCR       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UlazCijMin        , TheSchemaTable.Rows[CI.ulazCijMin       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UlazCijMax        , TheSchemaTable.Rows[CI.ulazCijMax       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UlazCijLast       , TheSchemaTable.Rows[CI.ulazCijLast      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkIzlazKol        , TheSchemaTable.Rows[CI.izlazKol         ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkStanjeKolRezerv , TheSchemaTable.Rows[CI.stanjeKolRezerv  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkIzlFinProdKCR   , TheSchemaTable.Rows[CI.izlFinProdKCR    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.IzlazCijMin       , TheSchemaTable.Rows[CI.izlazCijMin      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.IzlazCijMax       , TheSchemaTable.Rows[CI.izlazCijMax      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.IzlazCijLast      , TheSchemaTable.Rows[CI.izlazCijLast     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.PreDefVpc1        , TheSchemaTable.Rows[CI.preDefVpc1       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.PreDefVpc2        , TheSchemaTable.Rows[CI.preDefVpc2       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.PreDefMpc1        , TheSchemaTable.Rows[CI.preDefMpc1       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.PreDefDevc        , TheSchemaTable.Rows[CI.preDefDevc       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.PreDefRbt1        , TheSchemaTable.Rows[CI.preDefRbt1       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.PreDefRbt2        , TheSchemaTable.Rows[CI.preDefRbt2       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.PreDefMinKol      , TheSchemaTable.Rows[CI.preDefMinKol     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.PreDefMarza       , TheSchemaTable.Rows[CI.preDefMarza      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkUlazKolFisycal  , TheSchemaTable.Rows[CI.ulazKolFisycal   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkIzlazKolFisycal , TheSchemaTable.Rows[CI.izlazKolFisycal  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.DateZadUlaz       , TheSchemaTable.Rows[CI.dateZadUlaz      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.DateZadIzlaz      , TheSchemaTable.Rows[CI.dateZadIzlaz     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.DateZadPst        , TheSchemaTable.Rows[CI.dateZadPst       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.DateZadInv        , TheSchemaTable.Rows[CI.dateZadInv       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.ArtiklTS          , TheSchemaTable.Rows[CI.artiklTS         ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.ArtiklJM          , TheSchemaTable.Rows[CI.artiklJM         ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.FrsMinTt          , TheSchemaTable.Rows[CI.frsMinTt         ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.FrsMinTtNum       , TheSchemaTable.Rows[CI.frsMinTtNum      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkUlazFirmaKol    , TheSchemaTable.Rows[CI.ulazFirmaKol     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkIzlazFirmaKol   , TheSchemaTable.Rows[CI.izlazFirmaKol    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkRezervKolNaruc  , TheSchemaTable.Rows[CI.rezervKolNaruc   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkRezervKolIsporu , TheSchemaTable.Rows[CI.rezervKolIsporu  ]);

         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrPstKol         , TheSchemaTable.Rows[CI.rtrPstKol        ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrUlazKol        , TheSchemaTable.Rows[CI.rtrUlazKol       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrIzlazKol       , TheSchemaTable.Rows[CI.rtrIzlazKol      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrUlazKolFisycal , TheSchemaTable.Rows[CI.rtrUlazKolFisycal]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrIzlazKolFisycal, TheSchemaTable.Rows[CI.rtrIzlzKolFisycal]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrKolNaruceno    , TheSchemaTable.Rows[CI.rtrKolNaruceno   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrKolIsporuceno  , TheSchemaTable.Rows[CI.rtrKolIsporuceno ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrPstVrjNBC      , TheSchemaTable.Rows[CI.rtrPstVrjNBC     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrUlazVrjNBC     , TheSchemaTable.Rows[CI.rtrUlazVrjNBC    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrIzlazVrjNBC    , TheSchemaTable.Rows[CI.rtrIzlazVrjNBC   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrPstCijNBC      , TheSchemaTable.Rows[CI.rtrPstCijNBC     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrUlazCijNBC     , TheSchemaTable.Rows[CI.rtrUlazCijNBC    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrIzlazCijNBC    , TheSchemaTable.Rows[CI.rtrIzlazCijNBC   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrCijenaNBC      , TheSchemaTable.Rows[CI.rtrCijenaNBC     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrPstVrjMPC      , TheSchemaTable.Rows[CI.rtrPstVrjMPC     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrUlazVrjMPC     , TheSchemaTable.Rows[CI.rtrUlazVrjMPC    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrIzlazVrjMPC    , TheSchemaTable.Rows[CI.rtrIzlazVrjMPC   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrPstCijMPC      , TheSchemaTable.Rows[CI.rtrPstCijMPC     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrUlazCijMPC     , TheSchemaTable.Rows[CI.rtrUlazCijMPC    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrIzlazCijMPC    , TheSchemaTable.Rows[CI.rtrIzlazCijMPC   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrCijenaMPC      , TheSchemaTable.Rows[CI.rtrCijenaMPC     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.PrevMalopCij      , TheSchemaTable.Rows[CI.prevMalopCij     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.OrgPak            , TheSchemaTable.Rows[CI.orgPak           ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.OrgPakJM          , TheSchemaTable.Rows[CI.orgPakJM         ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkUlazKol2        , TheSchemaTable.Rows[CI.ulazKol2         ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkIzlazKol2       , TheSchemaTable.Rows[CI.izlazKol2        ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkPstKol2         , TheSchemaTable.Rows[CI.pstKol2          ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.InvKol2           , TheSchemaTable.Rows[CI.invKol2          ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkUlazFirmaKol2   , TheSchemaTable.Rows[CI.ulazFirmaKol2    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.UkIzlazFirmaKol2  , TheSchemaTable.Rows[CI.izlazFirmaKol2   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrPstKol2        , TheSchemaTable.Rows[CI.rtrPstKol2       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrUlazKol2       , TheSchemaTable.Rows[CI.rtrUlazKol2      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrIzlazKol2      , TheSchemaTable.Rows[CI.rtrIzlazKol2     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.ArtGrCd1          , TheSchemaTable.Rows[CI.artGrCd1         ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.ArtGrCd2          , TheSchemaTable.Rows[CI.artGrCd2         ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.ArtGrCd3          , TheSchemaTable.Rows[CI.artGrCd3         ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrPdvSt          , TheSchemaTable.Rows[CI.rtrPdvSt         ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrIsIrmUslug     , TheSchemaTable.Rows[CI.rtrIsIrmUslug    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.RtrParentID       , TheSchemaTable.Rows[CI.rtrParentID      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.InvKolDiff        , TheSchemaTable.Rows[CI.invKolDiff       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.InvKol2Diff       , TheSchemaTable.Rows[CI.invKol2Diff      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.InvFinDiffNBC     , TheSchemaTable.Rows[CI.invFinDiffNBC    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.InvFinDiffMPC     , TheSchemaTable.Rows[CI.invFinDiffMPC    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.InvFinMPC         , TheSchemaTable.Rows[CI.invFinMPC        ]);
         VvSQL.CreateCommandParameter(cmd, preffix, artstat.PrNBCBefThisUlaz  , TheSchemaTable.Rows[CI.prNBCBefThisUlaz ]);
      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva)
   {
      ArtStat artstat_rec = ((ArtStat)vvDataRecord);

      // Extender additons
      int ciOffset;
      if(vvDataRecord.IsFillingFromJoinReader) ciOffset = RtransDao.lastRtransCI + 1; // kada se puni JOIN-om za FakturDUC.PutFields pa ide cio Rtrans prije njega
      else                                     ciOffset = 0;

      FillFromDataReader(vvDataRecord, reader, isArhiva, ciOffset);
   }

   internal void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva, int ciOffset)
   {
      ArtStatStruct rdrData = new ArtStatStruct();

      try
      {
                  rdrData._recID   = reader.GetUInt32   (CI.recID            + ciOffset);
         rdrData._addTS            = reader.GetDateTime (CI.addTS            + ciOffset);
         rdrData._addUID           = reader.GetString   (CI.addUID           + ciOffset);
         rdrData._rtransRecID      = reader.GetUInt32   (CI.rtransRecID      + ciOffset);
         rdrData._t_artiklCD       = reader.GetString   (CI.t_artiklCD       + ciOffset);
         rdrData._t_skladCD        = reader.GetString   (CI.t_skladCD        + ciOffset);
         rdrData._t_skladDate      = reader.GetDateTime (CI.t_skladDate      + ciOffset);
         rdrData._t_tt             = reader.GetString   (CI.t_tt             + ciOffset);
         rdrData._t_ttSort         = reader.GetInt16    (CI.t_ttSort         + ciOffset);
         rdrData._t_ttNum          = reader.GetUInt32   (CI.t_ttNum          + ciOffset);
         rdrData._t_serial         = reader.GetUInt16   (CI.t_serial         + ciOffset);
         rdrData._transRbr         = reader.GetUInt32   (CI.transRbr         + ciOffset);
         rdrData._pstFinNBC        = reader.GetDecimal  (CI.pstFinNBC        + ciOffset);
         rdrData._ulazFinNBC       = reader.GetDecimal  (CI.ulazFinNBC       + ciOffset);
         rdrData._izlazFinNBC      = reader.GetDecimal  (CI.izlazFinNBC      + ciOffset);
         rdrData._ulazFirmaFinNBC  = reader.GetDecimal  (CI.ulazFirmaFinNBC  + ciOffset);
         rdrData._izlazFirmaFinNBC = reader.GetDecimal  (CI.izlazFirmaFinNBC + ciOffset);
         rdrData._lastPrNabCij     = reader.GetDecimal  (CI.lastPrNabCij     + ciOffset);
         rdrData._pstFinMPC        = reader.GetDecimal  (CI.pstFinMPC        + ciOffset);
         rdrData._ulazFinMPC       = reader.GetDecimal  (CI.ulazFinMPC       + ciOffset);
         rdrData._izlazFinMPC      = reader.GetDecimal  (CI.izlazFinMPC      + ciOffset);
         rdrData._ulazFirmaFinMPC  = reader.GetDecimal  (CI.ulazFirmaFinMPC  + ciOffset);
         rdrData._izlazFirmaFinMPC = reader.GetDecimal  (CI.izlazFirmaFinMPC + ciOffset);
         rdrData._lastMalopCij     = reader.GetDecimal  (CI.lastMalopCij     + ciOffset);
         rdrData._pstKol           = reader.GetDecimal  (CI.pstKol           + ciOffset);
         rdrData._invKol           = reader.GetDecimal  (CI.invKol           + ciOffset);
         rdrData._invFin           = reader.GetDecimal  (CI.invFin           + ciOffset);
         rdrData._ulazKol          = reader.GetDecimal  (CI.ulazKol          + ciOffset);
         rdrData._ulazFinKCR       = reader.GetDecimal  (CI.ulazFinKCR       + ciOffset);
         rdrData._ulazCijMin       = reader.GetDecimal  (CI.ulazCijMin       + ciOffset);
         rdrData._ulazCijMax       = reader.GetDecimal  (CI.ulazCijMax       + ciOffset);
         rdrData._ulazCijLast      = reader.GetDecimal  (CI.ulazCijLast      + ciOffset);
         rdrData._izlazKol         = reader.GetDecimal  (CI.izlazKol         + ciOffset);
         rdrData._stanjeKolRezerv  = reader.GetDecimal  (CI.stanjeKolRezerv  + ciOffset);
         rdrData._izlFinProdKCR    = reader.GetDecimal  (CI.izlFinProdKCR    + ciOffset);
         rdrData._izlazCijMin      = reader.GetDecimal  (CI.izlazCijMin      + ciOffset);
         rdrData._izlazCijMax      = reader.GetDecimal  (CI.izlazCijMax      + ciOffset);
         rdrData._izlazCijLast     = reader.GetDecimal  (CI.izlazCijLast     + ciOffset);
         rdrData._preDefVpc1       = reader.GetDecimal  (CI.preDefVpc1       + ciOffset);
         rdrData._preDefVpc2       = reader.GetDecimal  (CI.preDefVpc2       + ciOffset);
         rdrData._preDefMpc1       = reader.GetDecimal  (CI.preDefMpc1       + ciOffset);
         rdrData._preDefDevc       = reader.GetDecimal  (CI.preDefDevc       + ciOffset);
         rdrData._preDefRbt1       = reader.GetDecimal  (CI.preDefRbt1       + ciOffset);
         rdrData._preDefRbt2       = reader.GetDecimal  (CI.preDefRbt2       + ciOffset);
         rdrData._preDefMinKol     = reader.GetDecimal  (CI.preDefMinKol     + ciOffset);
         rdrData._preDefMarza      = reader.GetDecimal  (CI.preDefMarza      + ciOffset);
         rdrData._ulazKolFisycal   = reader.GetDecimal  (CI.ulazKolFisycal   + ciOffset);
         rdrData._izlazKolFisycal  = reader.GetDecimal  (CI.izlazKolFisycal  + ciOffset);
         rdrData._dateZadUlaz      = reader.GetDateTime (CI.dateZadUlaz      + ciOffset);
         rdrData._dateZadIzlaz     = reader.GetDateTime (CI.dateZadIzlaz     + ciOffset);
         rdrData._dateZadPst       = reader.GetDateTime (CI.dateZadPst       + ciOffset);
         rdrData._dateZadInv       = reader.GetDateTime (CI.dateZadInv       + ciOffset);
         rdrData._artiklTS         = reader.GetString   (CI.artiklTS         + ciOffset);
         rdrData._artiklJM         = reader.GetString   (CI.artiklJM         + ciOffset);
         rdrData._frsMinTt         = reader.GetString   (CI.frsMinTt         + ciOffset);
         rdrData._frsMinTtNum      = reader.GetUInt32   (CI.frsMinTtNum      + ciOffset);
         rdrData._ulazFirmaKol     = reader.GetDecimal  (CI.ulazFirmaKol     + ciOffset);
         rdrData._izlazFirmaKol    = reader.GetDecimal  (CI.izlazFirmaKol    + ciOffset);
         rdrData._rezervKolNaruc   = reader.GetDecimal  (CI.rezervKolNaruc   + ciOffset);
         rdrData._rezervKolIsporu  = reader.GetDecimal  (CI.rezervKolIsporu  + ciOffset);

         rdrData._rtrPstKol         = reader.GetDecimal(CI.rtrPstKol         + ciOffset);
         rdrData._rtrUlazKol        = reader.GetDecimal(CI.rtrUlazKol        + ciOffset);
         rdrData._rtrIzlazKol       = reader.GetDecimal(CI.rtrIzlazKol       + ciOffset);
         rdrData._rtrUlazKolFisycal = reader.GetDecimal(CI.rtrUlazKolFisycal + ciOffset);
         rdrData._rtrIzlzKolFisycal = reader.GetDecimal(CI.rtrIzlzKolFisycal + ciOffset);
         rdrData._rtrKolNaruceno    = reader.GetDecimal(CI.rtrKolNaruceno    + ciOffset);
         rdrData._rtrKolIsporuceno  = reader.GetDecimal(CI.rtrKolIsporuceno  + ciOffset);
         rdrData._rtrPstVrjNBC      = reader.GetDecimal(CI.rtrPstVrjNBC      + ciOffset);
         rdrData._rtrUlazVrjNBC     = reader.GetDecimal(CI.rtrUlazVrjNBC     + ciOffset);
         rdrData._rtrIzlazVrjNBC    = reader.GetDecimal(CI.rtrIzlazVrjNBC    + ciOffset);
         rdrData._rtrPstCijNBC      = reader.GetDecimal(CI.rtrPstCijNBC      + ciOffset);
         rdrData._rtrUlazCijNBC     = reader.GetDecimal(CI.rtrUlazCijNBC     + ciOffset);
         rdrData._rtrIzlazCijNBC    = reader.GetDecimal(CI.rtrIzlazCijNBC    + ciOffset);
         rdrData._rtrCijenaNBC      = reader.GetDecimal(CI.rtrCijenaNBC      + ciOffset);
         rdrData._rtrPstVrjMPC      = reader.GetDecimal(CI.rtrPstVrjMPC      + ciOffset);
         rdrData._rtrUlazVrjMPC     = reader.GetDecimal(CI.rtrUlazVrjMPC     + ciOffset);
         rdrData._rtrIzlazVrjMPC    = reader.GetDecimal(CI.rtrIzlazVrjMPC    + ciOffset);
         rdrData._rtrPstCijMPC      = reader.GetDecimal(CI.rtrPstCijMPC      + ciOffset);
         rdrData._rtrUlazCijMPC     = reader.GetDecimal(CI.rtrUlazCijMPC     + ciOffset);
         rdrData._rtrIzlazCijMPC    = reader.GetDecimal(CI.rtrIzlazCijMPC    + ciOffset);
         rdrData._rtrCijenaMPC      = reader.GetDecimal(CI.rtrCijenaMPC      + ciOffset);
         rdrData._prevMalopCij      = reader.GetDecimal(CI.prevMalopCij      + ciOffset);
         rdrData._orgPak            = reader.GetDecimal(CI.orgPak            + ciOffset);
         rdrData._orgPakJM          = reader.GetString (CI.orgPakJM          + ciOffset);
         rdrData._ulazKol2          = reader.GetDecimal(CI.ulazKol2          + ciOffset);
         rdrData._izlazKol2         = reader.GetDecimal(CI.izlazKol2         + ciOffset);
         rdrData._pstKol2           = reader.GetDecimal(CI.pstKol2           + ciOffset);
         rdrData._invKol2           = reader.GetDecimal(CI.invKol2           + ciOffset);
         rdrData._ulazFirmaKol2     = reader.GetDecimal(CI.ulazFirmaKol2     + ciOffset);
         rdrData._izlazFirmaKol2    = reader.GetDecimal(CI.izlazFirmaKol2    + ciOffset);
         rdrData._rtrPstKol2        = reader.GetDecimal(CI.rtrPstKol2        + ciOffset);
         rdrData._rtrUlazKol2       = reader.GetDecimal(CI.rtrUlazKol2       + ciOffset);
         rdrData._rtrIzlazKol2      = reader.GetDecimal(CI.rtrIzlazKol2      + ciOffset);
         rdrData._artGrCd1          = reader.GetString (CI.artGrCd1          + ciOffset);
         rdrData._artGrCd2          = reader.GetString (CI.artGrCd2          + ciOffset);
         rdrData._artGrCd3          = reader.GetString (CI.artGrCd3          + ciOffset);
         rdrData._rtrPdvSt          = reader.GetDecimal(CI.rtrPdvSt          + ciOffset);
         rdrData._rtrIsIrmUslug     = reader.GetBoolean(CI.rtrIsIrmUslug     + ciOffset);
         rdrData._rtrParentID       = reader.GetUInt32 (CI.rtrParentID       + ciOffset);
         rdrData._invKolDiff        = reader.GetDecimal(CI.invKolDiff        + ciOffset);
         rdrData._invKol2Diff       = reader.GetDecimal(CI.invKol2Diff       + ciOffset);
         rdrData._invFinDiff        = reader.GetDecimal(CI.invFinDiffNBC     + ciOffset);
         rdrData._invFinDiffMPC     = reader.GetDecimal(CI.invFinDiffMPC     + ciOffset);
         rdrData._invFinMPC         = reader.GetDecimal(CI.invFinMPC         + ciOffset);
         rdrData._prNBCBefThisUlaz  = reader.GetDecimal(CI.prNBCBefThisUlaz  + ciOffset);

      }
      catch(SqlNullValueException) // kada pri join-u sa rtrans-om nema pripadajuceg artstat recorda 
      {
      }

      ((ArtStat)vvDataRecord).CurrentData = rdrData;

      return;
   }

   internal void FillFromDataReader_forSUM_list(VvDataRecord vvDataRecord, XSqlDataReader reader)
   {
      ArtStatStruct rdrData = new ArtStatStruct();

      rdrData._t_tt              = reader.GetSafeString ( 0);
      rdrData._t_skladCD         = reader.GetSafeString ( 1);
      rdrData._rtrPstKol         = reader.GetSafeDecimal( 2);
      rdrData._rtrUlazKol        = reader.GetSafeDecimal( 3);
      rdrData._rtrIzlazKol       = reader.GetSafeDecimal( 4);
      rdrData._rtrPstVrjNBC      = reader.GetSafeDecimal( 5);
      rdrData._rtrUlazVrjNBC     = reader.GetSafeDecimal( 6);
      rdrData._rtrIzlazVrjNBC    = reader.GetSafeDecimal( 7);
      rdrData._rtrPstVrjMPC      = reader.GetSafeDecimal( 8);
      rdrData._rtrUlazVrjMPC     = reader.GetSafeDecimal( 9);
      rdrData._rtrIzlazVrjMPC    = reader.GetSafeDecimal(10);

      ((ArtStat)vvDataRecord).CurrentData = rdrData;

      return;
   }

   #endregion FillFromDataReader

   #region FtrCI struct & InitializeSchemaColumnIndexes()

   public struct ArtStatCI
   {
      internal int recID;

      internal int addTS           ;
      internal int addUID          ;
      internal int rtransRecID     ;
      internal int t_artiklCD      ;
      internal int t_skladCD       ;
      internal int t_skladDate     ;
      internal int t_tt            ;
      internal int t_ttSort        ;
      internal int t_ttNum         ;
      internal int t_serial        ;
      internal int transRbr        ;

      internal int pstFinNBC       ;
      internal int ulazFinNBC      ;
      internal int izlazFinNBC     ;
      internal int ulazFirmaFinNBC ;
      internal int izlazFirmaFinNBC;
      internal int lastPrNabCij    ;
      internal int pstFinMPC       ;
      internal int ulazFinMPC      ;
      internal int izlazFinMPC     ;
      internal int ulazFirmaFinMPC ;
      internal int izlazFirmaFinMPC;
      internal int lastMalopCij    ;
      internal int pstKol          ;
      internal int invKol          ;
      internal int invFin          ;
      internal int ulazKol         ;
      internal int ulazFinKCR      ;
      internal int ulazCijMin      ;
      internal int ulazCijMax      ;
      internal int ulazCijLast     ;
      internal int izlazKol        ;
      internal int stanjeKolRezerv ;
      internal int izlFinProdKCR   ;
      internal int izlazCijMin     ;
      internal int izlazCijMax     ;
      internal int izlazCijLast    ;
      internal int preDefVpc1      ;
      internal int preDefVpc2      ;
      internal int preDefMpc1      ;
      internal int preDefDevc      ;
      internal int preDefRbt1      ;
      internal int preDefRbt2      ;
      internal int preDefMinKol    ;
      internal int preDefMarza     ;
      internal int ulazKolFisycal  ;
      internal int izlazKolFisycal ;
      internal int dateZadUlaz     ;
      internal int dateZadIzlaz    ;
      internal int dateZadPst      ;
      internal int dateZadInv      ;
      internal int artiklTS        ;
      internal int artiklJM        ;
      internal int frsMinTt        ;
      internal int frsMinTtNum     ;
      internal int ulazFirmaKol    ;
      internal int izlazFirmaKol   ;
      internal int rezervKolNaruc  ;
      internal int rezervKolIsporu ;

      internal int rtrPstKol        ;
      internal int rtrUlazKol       ;
      internal int rtrIzlazKol      ;
      internal int rtrUlazKolFisycal;
      internal int rtrIzlzKolFisycal;
      internal int rtrKolNaruceno   ;
      internal int rtrKolIsporuceno ;
      internal int rtrPstVrjNBC     ;
      internal int rtrUlazVrjNBC    ;
      internal int rtrIzlazVrjNBC   ;
      internal int rtrPstCijNBC     ;
      internal int rtrUlazCijNBC    ;
      internal int rtrIzlazCijNBC   ;
      internal int rtrCijenaNBC     ;
      internal int rtrPstVrjMPC     ;
      internal int rtrUlazVrjMPC    ;
      internal int rtrIzlazVrjMPC   ;
      internal int rtrPstCijMPC     ;
      internal int rtrUlazCijMPC    ;
      internal int rtrIzlazCijMPC   ;
      internal int rtrCijenaMPC     ;
      internal int prevMalopCij     ;
      internal int orgPak           ;
      internal int orgPakJM         ;
      internal int ulazKol2         ;
      internal int izlazKol2        ;
      internal int pstKol2          ;
      internal int invKol2          ;
      internal int ulazFirmaKol2    ;
      internal int izlazFirmaKol2   ;
      internal int rtrPstKol2       ;
      internal int rtrUlazKol2      ;
      internal int rtrIzlazKol2     ;
      internal int artGrCd1         ;
      internal int artGrCd2         ;
      internal int artGrCd3         ;
      internal int rtrPdvSt         ;
      internal int rtrIsIrmUslug    ;
      internal int rtrParentID      ;
      internal int invKolDiff       ;
      internal int invKol2Diff      ;
      internal int invFinDiffNBC    ;
      internal int invFinDiffMPC    ;
      internal int invFinMPC        ;
      internal int prNBCBefThisUlaz ;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public ArtStatCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID            = GetSchemaColumnIndex("recID");

      CI.addTS            = GetSchemaColumnIndex("addTS");
      CI.addUID           = GetSchemaColumnIndex("addUID");
      CI.rtransRecID      = GetSchemaColumnIndex("rtransRecID");
      CI.t_artiklCD       = GetSchemaColumnIndex("t_artiklCD");
      CI.t_skladCD        = GetSchemaColumnIndex("t_skladCD");
      CI.t_skladDate      = GetSchemaColumnIndex("t_skladDate");
      CI.t_tt             = GetSchemaColumnIndex("t_tt");
      CI.t_ttSort         = GetSchemaColumnIndex("t_ttSort");
      CI.t_ttNum          = GetSchemaColumnIndex("t_ttNum");
      CI.t_serial         = GetSchemaColumnIndex("t_serial");
      CI.transRbr         = GetSchemaColumnIndex("transRbr");

      CI.pstFinNBC        = GetSchemaColumnIndex("pstFinNBC");
      CI.ulazFinNBC       = GetSchemaColumnIndex("ulazFinNBC");
      CI.izlazFinNBC      = GetSchemaColumnIndex("izlazFinNBC");
      CI.ulazFirmaFinNBC  = GetSchemaColumnIndex("ulazFirmaFinNBC");
      CI.izlazFirmaFinNBC = GetSchemaColumnIndex("izlazFirmaFinNBC");
      CI.lastPrNabCij     = GetSchemaColumnIndex("lastPrNabCij");
      CI.pstFinMPC        = GetSchemaColumnIndex("pstFinMPC");
      CI.ulazFinMPC       = GetSchemaColumnIndex("ulazFinMPC");
      CI.izlazFinMPC      = GetSchemaColumnIndex("izlazFinMPC");
      CI.ulazFirmaFinMPC  = GetSchemaColumnIndex("ulazFirmaFinMPC");
      CI.izlazFirmaFinMPC = GetSchemaColumnIndex("izlazFirmaFinMPC");
      CI.lastMalopCij     = GetSchemaColumnIndex("lastMalopCij");
      CI.pstKol           = GetSchemaColumnIndex("pstKol");
      CI.invKol           = GetSchemaColumnIndex("invKol");
      CI.invFin           = GetSchemaColumnIndex("invFin");
      CI.ulazKol          = GetSchemaColumnIndex("ulazKol");
      CI.ulazFinKCR       = GetSchemaColumnIndex("ulazFinKCR");
      CI.ulazCijMin       = GetSchemaColumnIndex("ulazCijMin");
      CI.ulazCijMax       = GetSchemaColumnIndex("ulazCijMax");
      CI.ulazCijLast      = GetSchemaColumnIndex("ulazCijLast");
      CI.izlazKol         = GetSchemaColumnIndex("izlazKol");
      CI.stanjeKolRezerv  = GetSchemaColumnIndex("stanjeKolRezerv");
      CI.izlFinProdKCR    = GetSchemaColumnIndex("izlFinProdKCR");
      CI.izlazCijMin      = GetSchemaColumnIndex("izlazCijMin");
      CI.izlazCijMax      = GetSchemaColumnIndex("izlazCijMax");
      CI.izlazCijLast     = GetSchemaColumnIndex("izlazCijLast");
      CI.preDefVpc1       = GetSchemaColumnIndex("preDefVpc1");
      CI.preDefVpc2       = GetSchemaColumnIndex("preDefVpc2");
      CI.preDefMpc1       = GetSchemaColumnIndex("preDefMpc1");
      CI.preDefDevc       = GetSchemaColumnIndex("preDefDevc");
      CI.preDefRbt1       = GetSchemaColumnIndex("preDefRbt1");
      CI.preDefRbt2       = GetSchemaColumnIndex("preDefRbt2");
      CI.preDefMinKol     = GetSchemaColumnIndex("preDefMinKol");
      CI.preDefMarza      = GetSchemaColumnIndex("preDefMarza");
      CI.ulazKolFisycal   = GetSchemaColumnIndex("ulazKolFisycal");
      CI.izlazKolFisycal  = GetSchemaColumnIndex("izlazKolFisycal");
      CI.dateZadUlaz      = GetSchemaColumnIndex("dateZadUlaz");
      CI.dateZadIzlaz     = GetSchemaColumnIndex("dateZadIzlaz");
      CI.dateZadPst       = GetSchemaColumnIndex("dateZadPst");
      CI.dateZadInv       = GetSchemaColumnIndex("dateZadInv");
      CI.artiklTS         = GetSchemaColumnIndex("artiklTS");
      CI.artiklJM         = GetSchemaColumnIndex("artiklJM");
      CI.frsMinTt         = GetSchemaColumnIndex("frsMinTt");
      CI.frsMinTtNum      = GetSchemaColumnIndex("frsMinTtNum");
      CI.ulazFirmaKol     = GetSchemaColumnIndex("ulazFirmaKol");
      CI.izlazFirmaKol    = GetSchemaColumnIndex("izlazFirmaKol");
      CI.rezervKolNaruc   = GetSchemaColumnIndex("rezervKolNaruc");
      CI.rezervKolIsporu  = GetSchemaColumnIndex("rezervKolIsporu");

      CI.rtrPstKol         = GetSchemaColumnIndex("rtrPstKol");
      CI.rtrUlazKol        = GetSchemaColumnIndex("rtrUlazKol");
      CI.rtrIzlazKol       = GetSchemaColumnIndex("rtrIzlazKol");
      CI.rtrUlazKolFisycal = GetSchemaColumnIndex("rtrUlazKolFisycal");
      CI.rtrIzlzKolFisycal = GetSchemaColumnIndex("rtrIzlzKolFisycal");
      CI.rtrKolNaruceno    = GetSchemaColumnIndex("rtrKolNaruceno");
      CI.rtrKolIsporuceno  = GetSchemaColumnIndex("rtrKolIsporuceno");
      CI.rtrPstVrjNBC      = GetSchemaColumnIndex("rtrPstVrjNBC");
      CI.rtrUlazVrjNBC     = GetSchemaColumnIndex("rtrUlazVrjNBC");
      CI.rtrIzlazVrjNBC    = GetSchemaColumnIndex("rtrIzlazVrjNBC");
      CI.rtrPstCijNBC      = GetSchemaColumnIndex("rtrPstCijNBC");
      CI.rtrUlazCijNBC     = GetSchemaColumnIndex("rtrUlazCijNBC");
      CI.rtrIzlazCijNBC    = GetSchemaColumnIndex("rtrIzlazCijNBC");
      CI.rtrCijenaNBC      = GetSchemaColumnIndex("rtrCijenaNBC");
      CI.rtrPstVrjMPC      = GetSchemaColumnIndex("rtrPstVrjMPC");
      CI.rtrUlazVrjMPC     = GetSchemaColumnIndex("rtrUlazVrjMPC");
      CI.rtrIzlazVrjMPC    = GetSchemaColumnIndex("rtrIzlazVrjMPC");
      CI.rtrPstCijMPC      = GetSchemaColumnIndex("rtrPstCijMPC");
      CI.rtrUlazCijMPC     = GetSchemaColumnIndex("rtrUlazCijMPC");
      CI.rtrIzlazCijMPC    = GetSchemaColumnIndex("rtrIzlazCijMPC");
      CI.rtrCijenaMPC      = GetSchemaColumnIndex("rtrCijenaMPC");
      CI.prevMalopCij      = GetSchemaColumnIndex("prevMalopCij");
      CI.orgPak            = GetSchemaColumnIndex("orgPak");
      CI.orgPakJM          = GetSchemaColumnIndex("orgPakJM");
      CI.ulazKol2          = GetSchemaColumnIndex("ulazKol2");
      CI.izlazKol2         = GetSchemaColumnIndex("izlazKol2");
      CI.pstKol2           = GetSchemaColumnIndex("pstKol2");
      CI.invKol2           = GetSchemaColumnIndex("invKol2");
      CI.ulazFirmaKol2     = GetSchemaColumnIndex("ulazFirmaKol2");
      CI.izlazFirmaKol2    = GetSchemaColumnIndex("izlazFirmaKol2");
      CI.rtrPstKol2        = GetSchemaColumnIndex("rtrPstKol2");
      CI.rtrUlazKol2       = GetSchemaColumnIndex("rtrUlazKol2");
      CI.rtrIzlazKol2      = GetSchemaColumnIndex("rtrIzlazKol2");
      CI.artGrCd1          = GetSchemaColumnIndex("artGrCd1");
      CI.artGrCd2          = GetSchemaColumnIndex("artGrCd2");
      CI.artGrCd3          = GetSchemaColumnIndex("artGrCd3");
      CI.rtrPdvSt          = GetSchemaColumnIndex("rtrPdvSt");
      CI.rtrIsIrmUslug     = GetSchemaColumnIndex("rtrIsIrmUslug");
      CI.rtrParentID       = GetSchemaColumnIndex("rtrParentID");
      CI.invKolDiff        = GetSchemaColumnIndex("invKolDiff");
      CI.invKol2Diff       = GetSchemaColumnIndex("invKol2Diff");
      CI.invFinDiffNBC     = GetSchemaColumnIndex("invFinDiff");
      CI.invFinDiffMPC     = GetSchemaColumnIndex("invFinDiffMPC");
      CI.invFinMPC         = GetSchemaColumnIndex("invFinMPC");
      CI.prNBCBefThisUlaz  = GetSchemaColumnIndex("prNBCBefThisUlaz");
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()

//#if GetArtstat_SUM_list_Old_Orig

   internal static void GetArtstat_SUM_list/*_Old_Orig*/(XSqlConnection conn, bool isPrmRazdoblja, List<ArtStat> artstatList, string _skladCD, DateTime _dateOd, DateTime _dateDo, VvRpt_RiSk_Filter RptFilter)
   {
      bool success = true;
      ArtStat artstat_rec;

      ZXC.sqlErrNo = 0;

    //if(artstatList == null) artstatList = new List<ArtStat>();
    //else                    artstatList   .Clear();

      bool isForceMPSK_by_NBC = RptFilter.FuseBool1; // RptFilter.FuseBool1 je Fld_IsNbcZaMPSK 

      string ulazShadowTT_IN_Clause       = TtInfo.GetSql_IN_Clause(ZXC.TtInfoArray.Where(tti => tti.IsUlazniShadowTT   ).Select(tti => tti.TheTT).ToArray());
      string izlazShadowTT_IN_Clause      = TtInfo.GetSql_IN_Clause(ZXC.TtInfoArray.Where(tti => tti.IsIzlazniShadowTT  ).Select(tti => tti.TheTT).ToArray());
      string uraPovratShadowTT_IN_Clause  = TtInfo.GetSql_IN_Clause(ZXC.TtInfoArray.Where(tti => tti.IsUraPovratShadowTT).Select(tti => tti.TheTT).ToArray());

      using(XSqlCommand cmd = (VvSQL.GetArtstat_SUM_list_Command(conn, isPrmRazdoblja, _skladCD, _dateOd, _dateDo, RptFilter, ulazShadowTT_IN_Clause, izlazShadowTT_IN_Clause, uraPovratShadowTT_IN_Clause, isForceMPSK_by_NBC)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;

               while(success && reader.Read())
               {
                  artstat_rec = new ArtStat();

                  ZXC.ArtStatDao.FillFromDataReader_forSUM_list(artstat_rec, reader);

                  artstat_rec.TtSort        = artstat_rec.TtInfo.TtSort ; 
                  artstat_rec.SkladDate     = _dateDo                   ; // tek toliko da ima nekaj 

                  artstat_rec.UkPstKol      = artstat_rec.RtrPstKol     ; 
                  artstat_rec.UkUlazKol     = artstat_rec.RtrUlazKol    ; 
                  artstat_rec.UkIzlazKol    = artstat_rec.RtrIzlazKol   ; 
                  artstat_rec.UkPstFinNBC   = artstat_rec.RtrPstVrjNBC  ; 
                  artstat_rec.UkUlazFinNBC  = artstat_rec.RtrUlazVrjNBC ; 
                  artstat_rec.UkIzlazFinNBC = artstat_rec.RtrIzlazVrjNBC; 
                  artstat_rec.UkPstFinMPC   = artstat_rec.RtrPstVrjMPC  ; 
                  artstat_rec.UkUlazFinMPC  = artstat_rec.RtrUlazVrjMPC ; 
                  artstat_rec.UkIzlazFinMPC = artstat_rec.RtrIzlazVrjMPC;

                  artstat_rec.ArtiklCD      = ZXC.luiListaFakturType.GetNameForThisCd(artstat_rec.TT);

                  if(artstat_rec.TT == Faktur.TT_NUV && artstat_rec.RtrUlazVrjMPC .IsZero() // nemoj ubacivati NUV/NIV ako nemaju sto za reci 
                     ||
                     artstat_rec.TT == Faktur.TT_NIV && artstat_rec.RtrIzlazVrjMPC.IsZero()) continue;

                  // 01.02.2023: 
                  if(artstat_rec.TT == Faktur.TT_NUP && artstat_rec.RtrUlazVrjNBC.IsZero()) continue;

                  artstatList.Add(artstat_rec);
               }
               reader.Close();
            }
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("GetArtstat_SUM_list", ex, System.Windows.Forms.MessageBoxButtons.OK);

            ZXC.sqlErrNo = ex.Number;
         }
      } // using 
   }
//#endif
   internal static void GetArtstat_SUM_list_NEWandWRONG(XSqlConnection conn, bool isPrmRazdoblja, List<ArtStat> artstatList, string _skladCD, DateTime _dateOd, DateTime _dateDo, VvRpt_RiSk_Filter RptFilter)
   {
      bool success = true;
    //ArtStat artstat_rec;

      ZXC.sqlErrNo = 0;

    //if(artstatList == null) artstatList = new List<ArtStat>();
    //else                    artstatList   .Clear();

      bool isForceMPSK_by_NBC = RptFilter.FuseBool1; // RptFilter.FuseBool1 je Fld_IsNbcZaMPSK 

      string ulazShadowTT_IN_Clause       = TtInfo.GetSql_IN_Clause(ZXC.TtInfoArray.Where(tti => tti.IsUlazniShadowTT   ).Select(tti => tti.TheTT).ToArray());
      string izlazShadowTT_IN_Clause      = TtInfo.GetSql_IN_Clause(ZXC.TtInfoArray.Where(tti => tti.IsIzlazniShadowTT  ).Select(tti => tti.TheTT).ToArray());
      string uraPovratShadowTT_IN_Clause  = TtInfo.GetSql_IN_Clause(ZXC.TtInfoArray.Where(tti => tti.IsUraPovratShadowTT).Select(tti => tti.TheTT).ToArray());

    //while(success && reader.Read())
      foreach(ArtStat artstat_rec in artstatList)
      {
         //artstat_rec = new ArtStat();

         //ZXC.ArtStatDao.FillFromDataReader_forSUM_list(artstat_rec, reader);

         artstat_rec.TtSort        = artstat_rec.TtInfo.TtSort ; 
         artstat_rec.SkladDate     = _dateDo                   ; // tek toliko da ima nekaj 

         artstat_rec.UkPstKol      = artstat_rec.RtrPstKol     ; 
         artstat_rec.UkUlazKol     = artstat_rec.RtrUlazKol    ; 
         artstat_rec.UkIzlazKol    = artstat_rec.RtrIzlazKol   ; 
         artstat_rec.UkPstFinNBC   = artstat_rec.RtrPstVrjNBC  ; 
         artstat_rec.UkUlazFinNBC  = artstat_rec.RtrUlazVrjNBC ; 
         artstat_rec.UkIzlazFinNBC = artstat_rec.RtrIzlazVrjNBC; 
         artstat_rec.UkPstFinMPC   = artstat_rec.RtrPstVrjMPC  ; 
         artstat_rec.UkUlazFinMPC  = artstat_rec.RtrUlazVrjMPC ; 
         artstat_rec.UkIzlazFinMPC = artstat_rec.RtrIzlazVrjMPC;

         artstat_rec.ArtiklCD      = ZXC.luiListaFakturType.GetNameForThisCd(artstat_rec.TT);

         if(artstat_rec.TT == Faktur.TT_NUV && artstat_rec.RtrUlazVrjMPC .IsZero() // nemoj ubacivati NUV/NIV ako nemaju sto za reci 
            ||
            artstat_rec.TT == Faktur.TT_NIV && artstat_rec.RtrIzlazVrjMPC.IsZero()) continue;

         // 01.02.2023: 
         if(artstat_rec.TT == Faktur.TT_NUP && artstat_rec.RtrUlazVrjNBC.IsZero()) continue;

         //artstatList.Add(artstat_rec);
      }
   }

}
