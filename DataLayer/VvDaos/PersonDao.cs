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

public sealed class PersonDao : VvDaoBase, IVvDao
{
   
   #region Singleton Constructor & instancer

   private static PersonDao instance;

   private PersonDao(XSqlConnection conn, string dbName) : base(dbName, Person.recordNameArhiva, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static PersonDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if (instance == null)
      {
         instance = new PersonDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTablePerson

   public static   uint TableVersionStatic { get { return 9; } }

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

        "personCD  int(6)  unsigned NOT NULL default '0',\n" +
        "ime       varchar(24)      NOT NULL default '',\n" +
        "prezime   varchar(32)      NOT NULL default '',\n" +

/* 08 */" ulica    varchar(32)             NOT NULL default '',\n" +   
/* 09 */" grad     varchar(32)             NOT NULL default '',\n" +     
/* 10 */" postaBr  varchar(16)             NOT NULL default '',\n" +  
/* 11 */" tel      varchar(16)             NOT NULL default '',\n" +     
/* 12 */" gsm      varchar(24)             NOT NULL default '',\n" +      
/* 13 */" email    varchar(40)             NOT NULL default '',\n" +
/* 14 */" datePri  date                    NOT NULL default '0001-01-01',\n" +
/* 15 */" dateOdj  date                    NOT NULL default '0001-01-01',\n" +
/* 16 */" strSpr   varchar(32)             NOT NULL default '',\n" +    
/* 17 */" strSprCd char(6)                 NOT NULL default '',\n" +    
/* 18 */" jmbg     varchar(13)             NOT NULL default '',\n" +    
/* 19 */" oib      varchar(11)             NOT NULL default '',\n" +    
/* 20 */" regob    varchar(16)             NOT NULL default '',\n" +    
/* 21 */" osBrOsig varchar(16)             NOT NULL default '',\n" +    
/* 22 */" banka_cd int(6)         unsigned NOT NULL default '0',\n" +
/* 23 */" banka_tk varchar(32)             NOT NULL default '',\n" +
/* 24 */" pnbM     char(2)                 NOT NULL default '',\n" +
/* 25 */" pnbV     varchar(40)             NOT NULL default '',\n" +
/* 26 */" mtros_cd int(6)         unsigned NOT NULL default '0',\n" +
/* 27 */" mtros_tk char(6)                 NOT NULL default '',\n" +
/* 28 */" radMj    varchar(40)             NOT NULL default '',\n" +
/* 29 */" napomena varchar(1024)           NOT NULL default '',\n" +
/* 30 */" isIzuzet tinyint(1)     unsigned NOT NULL default '0',\n" +
/* 31 */" isPlaca  tinyint(1)     unsigned NOT NULL default '0',\n" +
/* 32 */" isUgDj   tinyint(1)     unsigned NOT NULL default '0',\n" +
/* 33 */" isAutH   tinyint(1)     unsigned NOT NULL default '0',\n" +
/* 34 */" isPoduz  tinyint(1)     unsigned NOT NULL default '0',\n" +
/* 35 */" isNadzO  tinyint(1)     unsigned NOT NULL default '0',\n" +
/* 36 */" zupan    varchar(24)             NOT NULL default '',\n" +
/* 37 */" zupCd    char(6)                 NOT NULL default '',\n" +
/* 38 */" birthDate date                   NOT NULL default '0001-01-01',\n" +
/* 39 */" vrstaRadVrem tinyint(1) unsigned NOT NULL default '0',\n" +
/* 40 */" vrstaRadOdns tinyint(1) unsigned NOT NULL default '0',\n" +
/* 41 */" vrstaIsplate tinyint(1) unsigned NOT NULL default '0',\n" +
/* 42 */" spol         tinyint(1) unsigned NOT NULL default '0',\n" +
/* 43 */" ts           char(4)             NOT NULL default '',\n" +
/*44+2*/" tsName       varchar(24)         NOT NULL default '',\n" +      
/* 45 */" prevStazDD   int(2)     unsigned NOT NULL default '0',\n" +
/* 46 */" prevStazMM   int(2)     unsigned NOT NULL default '0',\n" +
/* 47 */" prevStazYY   int(2)     unsigned NOT NULL default '0',\n" +
/* 48 */" korKoef01    decimal( 4,2)       NOT NULL default '0.00',\n" +   
/* 49 */" korKoef02    decimal( 4,2)       NOT NULL default '0.00',\n" +   
/* 50 */" korKoef03    decimal( 4,2)       NOT NULL default '0.00',\n" +   
/* 51 */" korKoef04    decimal( 4,2)       NOT NULL default '0.00',\n" +   
/* 52 */" korKoef05    decimal( 4,2)       NOT NULL default '0.00',\n" +   
/* 53 */" korKoef06    decimal( 4,2)       NOT NULL default '0.00',\n" +   
/* 54 */" korKoef07    decimal( 4,2)       NOT NULL default '0.00',\n" +   
/* 55 */" korKoef08    decimal( 4,2)       NOT NULL default '0.00',\n" +   
/* 56 */" korKoef09    decimal( 4,2)       NOT NULL default '0.00',\n" +   
/* 57 */" korKoef10    decimal( 4,2)       NOT NULL default '0.00',\n" +   
/* 58 */" korKoef11    decimal( 4,2)       NOT NULL default '0.00',\n" +   
/* 59 */" korKoef12    decimal( 4,2)       NOT NULL default '0.00',\n" +   
/*60+2*/" birthMjestoDrz varchar(64)       NOT NULL default ''    ,\n" +      
/*61+2*/" naravVrRad     varchar(32)       NOT NULL default ''    ,\n" +      
/*62+2*/" drzavljanstvo  varchar(32)       NOT NULL default ''    ,\n" +      
/*63+2*/" dozvola        varchar(32)       NOT NULL default ''    ,\n" +      
/*64+2*/" prijava        varchar(32)       NOT NULL default ''    ,\n" +      
/*65+2*/" strucno        varchar(64)       NOT NULL default ''    ,\n" +      
/*66+2*/" zanimanje      varchar(32)       NOT NULL default ''    ,\n" +      
/*67+2*/" trajanjeUgOdr  varchar(16)       NOT NULL default ''    ,\n" +      
/*68+2*/" cl61ugSuglas   varchar(32)       NOT NULL default ''    ,\n" +      
/*69+2*/" cl62ugSuglas   varchar(32)       NOT NULL default ''    ,\n" +      
/*70+2*/" probniRad      varchar(16)       NOT NULL default ''    ,\n" +      
/*71+2*/" pripravStaz    varchar(16)       NOT NULL default ''    ,\n" +      
/*72+2*/" rezIspita      varchar(32)       NOT NULL default ''    ,\n" +      
/*73+2*/" radIno         varchar(16)       NOT NULL default ''    ,\n" +      
/*74+2*/" gdjeRadIno     varchar(32)       NOT NULL default ''    ,\n" +      
/*75+2*/" ustupRadnika   varchar(16)       NOT NULL default ''    ,\n" +      
/*76+2*/" ustupMjesto    varchar(32)       NOT NULL default ''    ,\n" +      
/*77+2*/" drzavaPovezDrs varchar(32)       NOT NULL default ''    ,\n" +      
/*78+2*/" posaoBenefStaz varchar(32)       NOT NULL default ''    ,\n" +      
/*79+2*/" nacinBenef     varchar(16)       NOT NULL default ''    ,\n" +      
/*80+2*/" sposobnost     varchar(32)       NOT NULL default ''    ,\n" +      
/*81+2*/" mjestoRada     varchar(32)       NOT NULL default ''    ,\n" +      
/*82+2*/" mirovanjeRO    varchar(16)       NOT NULL default ''    ,\n" +      
/*83+2*/" razlogOdj      varchar(32)       NOT NULL default ''    ,\n" +      
/*84+2*/" banka2TK       varchar(32)       NOT NULL default ''    ,\n" +      
/*85+2*/" banka2         int(6)   unsigned NOT NULL default '0'   ,\n" +
/*86+2*/" tfs            decimal( 4,2)     NOT NULL default '0.00',\n" +   
/*87+2*/" skrRV          decimal( 4,2)     NOT NULL default '0.00',\n" +   
/*88+2*/" isSO         tinyint(1) unsigned NOT NULL default '0'   ,\n" +
/*89+2*/" isOtherVr    tinyint(1) unsigned NOT NULL default '0'   ,\n" +

/*90+2*/"x_brutoOsn    decimal(10,2)       NOT NULL default '0.00',\n" +   
/*91+2*/"x_godStaza    decimal( 4,1)       NOT NULL default '0.00',\n" +    
/*92+2*/"x_koef        decimal( 4,2)       NOT NULL default '0.00',\n" +   
/*93+2*/"x_prijevoz    decimal(10,2)       NOT NULL default '0.00',\n" +
/*94+2*/"x_dnFondSati  decimal( 5,2)       NOT NULL default '0.00',\n" +
/*95+2*/"x_opcCD       char(5)             NOT NULL default ''    ,\n" +
/*96+2*/"x_opcRadCD    char(5)             NOT NULL default ''    ,\n" +
/*97+2*/"x_isMioII     tinyint(1) unsigned NOT NULL default '0'   ,\n" +  
/*98+2*/"x_isTrgov     tinyint(1) unsigned NOT NULL default '0'   ,\n" +  

         CreateTable_ArhivaExtensionDefinition(isArhiva) +

                              "PRIMARY KEY  (recID),\n" +
        (isArhiva ? "" : "UNIQUE ") + "KEY BY_PREZIME (prezime, ime, personCD)\n," +
        (isArhiva ? "" : "UNIQUE ") + "KEY BY_CODE    (              personCD)\n" +

          (isArhiva ? ", KEY BYqOrigRecID (origRecID)\n" : "")

      );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Person.recordNameArhiva;
      else         tableName = Person.recordName;

      switch(catchingVersion)
      {
         case 2: return (isArhiva ? "ADD INDEX BYqOrigRecID (origRecID)\n" : "skip_me");

         case 3: return AlterTable_LanSrvID_And_LanRecID_Columns;

         case 4: return ("ADD COLUMN prevStazDD   int(2)     unsigned NOT NULL default '0' AFTER tsName    ,  " +
                         "ADD COLUMN prevStazMM   int(2)     unsigned NOT NULL default '0' AFTER prevStazDD,  " +
                         "ADD COLUMN prevStazYY   int(2)     unsigned NOT NULL default '0' AFTER prevStazMM;\n");

         case 5: return ("ADD COLUMN korKoef01     decimal( 4,2)       NOT NULL default '0.00' AFTER prevStazYY,  " +
                         "ADD COLUMN korKoef02     decimal( 4,2)       NOT NULL default '0.00' AFTER korKoef01 ,  " +
                         "ADD COLUMN korKoef03     decimal( 4,2)       NOT NULL default '0.00' AFTER korKoef02 ,  " +
                         "ADD COLUMN korKoef04     decimal( 4,2)       NOT NULL default '0.00' AFTER korKoef03 ,  " +
                         "ADD COLUMN korKoef05     decimal( 4,2)       NOT NULL default '0.00' AFTER korKoef04 ,  " +
                         "ADD COLUMN korKoef06     decimal( 4,2)       NOT NULL default '0.00' AFTER korKoef05 ,  " +
                         "ADD COLUMN korKoef07     decimal( 4,2)       NOT NULL default '0.00' AFTER korKoef06 ,  " +
                         "ADD COLUMN korKoef08     decimal( 4,2)       NOT NULL default '0.00' AFTER korKoef07 ,  " +
                         "ADD COLUMN korKoef09     decimal( 4,2)       NOT NULL default '0.00' AFTER korKoef08 ,  " +
                         "ADD COLUMN korKoef10     decimal( 4,2)       NOT NULL default '0.00' AFTER korKoef09 ,  " +
                         "ADD COLUMN korKoef11     decimal( 4,2)       NOT NULL default '0.00' AFTER korKoef10 ,  " +
                         "ADD COLUMN korKoef12     decimal( 4,2)       NOT NULL default '0.00' AFTER korKoef11 ;\n");

         case 6: return ("MODIFY COLUMN napomena    varchar(512)      NOT NULL default ''                         ,  " +
                         "ADD COLUMN birthMjestoDrz varchar(64)       NOT NULL default ''     AFTER korKoef12     ,  " +
                         "ADD COLUMN naravVrRad     varchar(32)       NOT NULL default ''     AFTER birthMjestoDrz,  " +
                         "ADD COLUMN drzavljanstvo  varchar(32)       NOT NULL default ''     AFTER naravVrRad    ,  " +
                         "ADD COLUMN dozvola        varchar(32)       NOT NULL default ''     AFTER drzavljanstvo ,  " +
                         "ADD COLUMN prijava        varchar(32)       NOT NULL default ''     AFTER dozvola       ,  " +
                         "ADD COLUMN strucno        varchar(64)       NOT NULL default ''     AFTER prijava       ,  " +
                         "ADD COLUMN zanimanje      varchar(32)       NOT NULL default ''     AFTER strucno       ,  " +
                         "ADD COLUMN trajanjeUgOdr  varchar(16)       NOT NULL default ''     AFTER zanimanje     ,  " +
                         "ADD COLUMN cl61ugSuglas   varchar(32)       NOT NULL default ''     AFTER trajanjeUgOdr ,  " +
                         "ADD COLUMN cl62ugSuglas   varchar(32)       NOT NULL default ''     AFTER cl61ugSuglas  ,  " +
                         "ADD COLUMN probniRad      varchar(16)       NOT NULL default ''     AFTER cl62ugSuglas  ,  " +
                         "ADD COLUMN pripravStaz    varchar(16)       NOT NULL default ''     AFTER probniRad     ,  " +
                         "ADD COLUMN rezIspita      varchar(32)       NOT NULL default ''     AFTER pripravStaz   ,  " +
                         "ADD COLUMN radIno         varchar(16)       NOT NULL default ''     AFTER rezIspita     ,  " +
                         "ADD COLUMN gdjeRadIno     varchar(32)       NOT NULL default ''     AFTER radIno        ,  " +
                         "ADD COLUMN ustupRadnika   varchar(16)       NOT NULL default ''     AFTER gdjeRadIno    ,  " +
                         "ADD COLUMN ustupMjesto    varchar(32)       NOT NULL default ''     AFTER ustupRadnika  ,  " +
                         "ADD COLUMN drzavaPovezDrs varchar(32)       NOT NULL default ''     AFTER ustupMjesto   ,  " +
                         "ADD COLUMN posaoBenefStaz varchar(32)       NOT NULL default ''     AFTER drzavaPovezDrs,  " +
                         "ADD COLUMN nacinBenef     varchar(16)       NOT NULL default ''     AFTER posaoBenefStaz,  " +
                         "ADD COLUMN sposobnost     varchar(32)       NOT NULL default ''     AFTER nacinBenef    ,  " +
                         "ADD COLUMN mjestoRada     varchar(32)       NOT NULL default ''     AFTER sposobnost    ,  " +
                         "ADD COLUMN mirovanjeRO    varchar(16)       NOT NULL default ''     AFTER mjestoRada    ,  " +
                         "ADD COLUMN razlogOdj      varchar(32)       NOT NULL default ''     AFTER mirovanjeRO   ,  " +
                         "ADD COLUMN banka2TK       varchar(32)       NOT NULL default ''     AFTER razlogOdj     ,  " +
                         "ADD COLUMN banka2         int(6)   unsigned NOT NULL default '0'    AFTER banka2TK      ,  " +
                         "ADD COLUMN tfs            decimal( 4,2)     NOT NULL default '0.00' AFTER banka2        ,  " +
                         "ADD COLUMN skrRV          decimal( 4,2)     NOT NULL default '0.00' AFTER tfs           ,  " +
                         "ADD COLUMN isSO         tinyint(1) unsigned NOT NULL default '0'    AFTER skrRV         ,  " +
                         "ADD COLUMN isOtherVr    tinyint(1) unsigned NOT NULL default '0'    AFTER isSO          ;\n");

         case 7: return ("ADD COLUMN x_brutoOsn   decimal(10,2)         NOT NULL default '0.00' AFTER isOtherVr   ,  " +
                         "ADD COLUMN x_godStaza   decimal( 4,1)         NOT NULL default '0.00' AFTER x_brutoOsn  ,  " +
                         "ADD COLUMN x_koef       decimal( 4,2)         NOT NULL default '0.00' AFTER x_godStaza  ,  " +
                         "ADD COLUMN x_prijevoz   decimal(10,2)         NOT NULL default '0.00' AFTER x_koef      ,  " +
                         "ADD COLUMN x_dnFondSati decimal( 5,2)         NOT NULL default '0.00' AFTER x_prijevoz  ,  " +
                         "ADD COLUMN x_opcCD      char(5)               NOT NULL default ''     AFTER x_dnFondSati,  " +
                         "ADD COLUMN x_opcRadCD   char(5)               NOT NULL default ''     AFTER x_opcCD     ,  " +
                         "ADD COLUMN x_isMioII    tinyint(1)   unsigned NOT NULL default '0'    AFTER x_opcRadCD  ;\n");

         case 8: return ("ADD COLUMN x_isTrgov    tinyint(1)   unsigned NOT NULL default '0'    AFTER x_isMioII  ;\n");

         case 9: return ("MODIFY COLUMN napomena    varchar(1024)      NOT NULL default ''                       ;\n");

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTablePerson

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      Person person = (Person)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.ID_Only ||
         plt == VvSQL.ParamListType.Old_Values)
      {
       //VvSQL.CreateCommandParameter(cmd, where_or_and, "prjktKupdobCD", person.RecID, XSqlDbType.Int32, 10);
         VvSQL.CreateCommandParameter(cmd, preffix, person.RecID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete ||
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         //DataRow theRow = TheSchemaTable.Rows.Find("modUID");

         VvSQL.CreateCommandParameter(cmd, preffix, person.AddTS,      TheSchemaTable.Rows[CI.addTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.ModTS,      TheSchemaTable.Rows[CI.modTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.AddUID,     TheSchemaTable.Rows[CI.addUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.ModUID,     TheSchemaTable.Rows[CI.modUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.LanSrvID,   TheSchemaTable.Rows[CI.lanSrvID]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.LanRecID,   TheSchemaTable.Rows[CI.lanRecID]);
   
         VvSQL.CreateCommandParameter(cmd, preffix, person.PersonCD,   TheSchemaTable.Rows[CI.personCD]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Ime,        TheSchemaTable.Rows[CI.ime]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Prezime,    TheSchemaTable.Rows[CI.prezime]);
      
         VvSQL.CreateCommandParameter(cmd, preffix, person.Ulica        , TheSchemaTable.Rows[CI.ulica]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Grad         , TheSchemaTable.Rows[CI.grad]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.PostaBr      , TheSchemaTable.Rows[CI.postaBr]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Tel          , TheSchemaTable.Rows[CI.tel]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Gsm          , TheSchemaTable.Rows[CI.gsm]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Email        , TheSchemaTable.Rows[CI.email]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.DatePri      , TheSchemaTable.Rows[CI.datePri]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.DateOdj      , TheSchemaTable.Rows[CI.dateOdj]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.StrSpr       , TheSchemaTable.Rows[CI.strSpr]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.StrSprCd     , TheSchemaTable.Rows[CI.strSprCd]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Jmbg         , TheSchemaTable.Rows[CI.jmbg]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Oib          , TheSchemaTable.Rows[CI.oib]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Regob        , TheSchemaTable.Rows[CI.regob]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.OsBrOsig     , TheSchemaTable.Rows[CI.osBrOsig]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.BankaCd      , TheSchemaTable.Rows[CI.banka_cd]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.BankaTk      , TheSchemaTable.Rows[CI.banka_tk]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.PnbM         , TheSchemaTable.Rows[CI.pnbM]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.PnbV         , TheSchemaTable.Rows[CI.pnbV]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.MtrosCd      , TheSchemaTable.Rows[CI.mtros_cd]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.MtrosTk      , TheSchemaTable.Rows[CI.mtros_tk]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.RadMj        , TheSchemaTable.Rows[CI.radMj]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Napomena     , TheSchemaTable.Rows[CI.napomena]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.IsIzuzet     , TheSchemaTable.Rows[CI.isIzuzet]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.IsPlaca      , TheSchemaTable.Rows[CI.isPlaca]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.IsUgDj       , TheSchemaTable.Rows[CI.isUgDj]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.IsAutH       , TheSchemaTable.Rows[CI.isAutH]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.IsPoduz      , TheSchemaTable.Rows[CI.isPoduz]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.IsNadzO      , TheSchemaTable.Rows[CI.isNadzO]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Zupan        , TheSchemaTable.Rows[CI.zupan]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.ZupCd        , TheSchemaTable.Rows[CI.zupCd]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.BirthDate    , TheSchemaTable.Rows[CI.birthDate]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.VrstaRadVrem , TheSchemaTable.Rows[CI.vrstaRadVrem]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.VrstaRadOdns , TheSchemaTable.Rows[CI.vrstaRadOdns]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.VrstaIsplate , TheSchemaTable.Rows[CI.vrstaIsplate]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Spol         , TheSchemaTable.Rows[CI.spol]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.TS           , TheSchemaTable.Rows[CI.ts]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.TsName       , TheSchemaTable.Rows[CI.tsName]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.PrevStazDD   , TheSchemaTable.Rows[CI.prevStazDD]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.PrevStazMM   , TheSchemaTable.Rows[CI.prevStazMM]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.PrevStazYY   , TheSchemaTable.Rows[CI.prevStazYY]);

         VvSQL.CreateCommandParameter(cmd, preffix, person.KorKoef01    , TheSchemaTable.Rows[CI.korKoef01 ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.KorKoef02    , TheSchemaTable.Rows[CI.korKoef02 ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.KorKoef03    , TheSchemaTable.Rows[CI.korKoef03 ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.KorKoef04    , TheSchemaTable.Rows[CI.korKoef04 ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.KorKoef05    , TheSchemaTable.Rows[CI.korKoef05 ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.KorKoef06    , TheSchemaTable.Rows[CI.korKoef06 ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.KorKoef07    , TheSchemaTable.Rows[CI.korKoef07 ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.KorKoef08    , TheSchemaTable.Rows[CI.korKoef08 ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.KorKoef09    , TheSchemaTable.Rows[CI.korKoef09 ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.KorKoef10    , TheSchemaTable.Rows[CI.korKoef10 ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.KorKoef11    , TheSchemaTable.Rows[CI.korKoef11 ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.KorKoef12    , TheSchemaTable.Rows[CI.korKoef12 ]);

         VvSQL.CreateCommandParameter(cmd, preffix, person.BirthMjestoDrz, TheSchemaTable.Rows[CI.birthMjestoDrz]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.NaravVrRad    , TheSchemaTable.Rows[CI.naravVrRad    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Drzavljanstvo , TheSchemaTable.Rows[CI.drzavljanstvo ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Dozvola       , TheSchemaTable.Rows[CI.dozvola       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Prijava       , TheSchemaTable.Rows[CI.prijava       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Strucno       , TheSchemaTable.Rows[CI.strucno       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Zanimanje     , TheSchemaTable.Rows[CI.zanimanje     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.TrajanjeUgOdr , TheSchemaTable.Rows[CI.trajanjeUgOdr ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Cl61ugSuglas  , TheSchemaTable.Rows[CI.cl61ugSuglas  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Cl62ugSuglas  , TheSchemaTable.Rows[CI.cl62ugSuglas  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.ProbniRad     , TheSchemaTable.Rows[CI.probniRad     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.PripravStaz   , TheSchemaTable.Rows[CI.pripravStaz   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.RezIspita     , TheSchemaTable.Rows[CI.rezIspita     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.RadIno        , TheSchemaTable.Rows[CI.radIno        ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.GdjeRadIno    , TheSchemaTable.Rows[CI.gdjeRadIno    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.UstupRadnika  , TheSchemaTable.Rows[CI.ustupRadnika  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.UstupMjesto   , TheSchemaTable.Rows[CI.ustupMjesto   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.DrzavaPovezDrs, TheSchemaTable.Rows[CI.drzavaPovezDrs]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.PosaoBenefStaz, TheSchemaTable.Rows[CI.posaoBenefStaz]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.NacinBenef    , TheSchemaTable.Rows[CI.nacinBenef    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Sposobnost    , TheSchemaTable.Rows[CI.sposobnost    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.MjestoRada    , TheSchemaTable.Rows[CI.mjestoRada    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.MirovanjeRO   , TheSchemaTable.Rows[CI.mirovanjeRO   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.RazlogOdj     , TheSchemaTable.Rows[CI.razlogOdj     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Banka2TK      , TheSchemaTable.Rows[CI.banka2TK      ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Banka2        , TheSchemaTable.Rows[CI.banka2        ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.Tfs           , TheSchemaTable.Rows[CI.tfs           ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.SkrRV         , TheSchemaTable.Rows[CI.skrRV         ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.IsSO          , TheSchemaTable.Rows[CI.isSO          ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.IsOtherVr     , TheSchemaTable.Rows[CI.isOtherVr     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.X_brutoOsn    , TheSchemaTable.Rows[CI.x_brutoOsn    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.X_godStaza    , TheSchemaTable.Rows[CI.x_godStaza    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.X_koef        , TheSchemaTable.Rows[CI.x_koef        ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.X_prijevoz    , TheSchemaTable.Rows[CI.x_prijevoz    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.X_dnFondSati  , TheSchemaTable.Rows[CI.x_dnFondSati  ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.X_opcCD       , TheSchemaTable.Rows[CI.x_opcCD       ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.X_opcRadCD    , TheSchemaTable.Rows[CI.x_opcRadCD    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.X_isMioII     , TheSchemaTable.Rows[CI.x_isMioII     ]);
         VvSQL.CreateCommandParameter(cmd, preffix, person.X_isTrgov     , TheSchemaTable.Rows[CI.x_isTrgov     ]);

         if(isArhiva)
         {
            VvSQL.CreateCommandParameter(cmd, preffix, person.OrigRecID, TheSchemaTable.Rows[CI.origRecID]);
            VvSQL.CreateCommandParameter(cmd, preffix, person.RecVer,    TheSchemaTable.Rows[CI.recVer]);
            VvSQL.CreateCommandParameter(cmd, preffix, person.ArAction,  TheSchemaTable.Rows[CI.arAction]);
            VvSQL.CreateCommandParameter(cmd, preffix, person.ArTS,      TheSchemaTable.Rows[CI.arTS]);
            VvSQL.CreateCommandParameter(cmd, preffix, person.ArUID,     TheSchemaTable.Rows[CI.arUID]);
         }

      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva)
   {
      PersonStruct rdrData = new PersonStruct();

      rdrData._recID       = reader.GetUInt32  (CI.recID    );
      rdrData._addTS       = reader.GetDateTime(CI.addTS    );
      rdrData._modTS       = reader.GetDateTime(CI.modTS    );
      rdrData._addUID      = reader.GetString  (CI.addUID   );
      rdrData._modUID      = reader.GetString  (CI.modUID   );
      rdrData._lanSrvID    = reader.GetUInt32  (CI.lanSrvID );
      rdrData._lanRecID    = reader.GetUInt32  (CI.lanRecID );

      rdrData._personCD  = reader.GetUInt32  (CI.personCD);
      rdrData._ime       = reader.GetString  (CI.ime     );
      rdrData._prezime   = reader.GetString  (CI.prezime );

      rdrData._ulica          = reader.GetString  (CI.ulica);
      rdrData._grad           = reader.GetString  (CI.grad);
      rdrData._postaBr        = reader.GetString  (CI.postaBr);
      rdrData._tel            = reader.GetString  (CI.tel);
      rdrData._gsm            = reader.GetString  (CI.gsm);
      rdrData._email          = reader.GetString  (CI.email);
      rdrData._datePri        = reader.GetDateTime(CI.datePri);
      rdrData._dateOdj        = reader.GetDateTime(CI.dateOdj);
      rdrData._strSpr         = reader.GetString  (CI.strSpr);
      rdrData._strSprCd       = reader.GetString  (CI.strSprCd);
      rdrData._jmbg           = reader.GetString  (CI.jmbg);
      rdrData._oib            = reader.GetString  (CI.oib);
      rdrData._regob          = reader.GetString  (CI.regob);
      rdrData._osBrOsig       = reader.GetString  (CI.osBrOsig);
      rdrData._banka_cd       = reader.GetUInt32  (CI.banka_cd);
      rdrData._banka_tk       = reader.GetString  (CI.banka_tk);
      rdrData._pnbM           = reader.GetString  (CI.pnbM);
      rdrData._pnbV           = reader.GetString  (CI.pnbV);
      rdrData._mtros_cd       = reader.GetUInt32  (CI.mtros_cd);
      rdrData._mtros_tk       = reader.GetString  (CI.mtros_tk);
      rdrData._radMj          = reader.GetString  (CI.radMj);
      rdrData._napomena       = reader.GetString  (CI.napomena);
      rdrData._isIzuzet       = reader.GetBoolean (CI.isIzuzet);
      rdrData._isPlaca        = reader.GetBoolean (CI.isPlaca);
      rdrData._isUgDj         = reader.GetBoolean (CI.isUgDj);
      rdrData._isAutH         = reader.GetBoolean (CI.isAutH);
      rdrData._isPoduz        = reader.GetBoolean (CI.isPoduz);
      rdrData._isNadzO        = reader.GetBoolean (CI.isNadzO);
      rdrData._zupan          = reader.GetString  (CI.zupan);
      rdrData._zupCd          = reader.GetString  (CI.zupCd);
      rdrData._birthDate      = reader.GetDateTime(CI.birthDate);
      rdrData._vrstaRadVrem   = reader.GetUInt16  (CI.vrstaRadVrem);
      rdrData._vrstaRadOdns   = reader.GetUInt16  (CI.vrstaRadOdns);
      rdrData._vrstaIsplate   = reader.GetUInt16  (CI.vrstaIsplate);
      rdrData._spol           = reader.GetUInt16  (CI.spol);
      rdrData._ts             = reader.GetString  (CI.ts);
      rdrData._tsName         = reader.GetString  (CI.tsName);
      rdrData._prevStazDD     = reader.GetUInt32  (CI.prevStazDD);
      rdrData._prevStazMM     = reader.GetUInt32  (CI.prevStazMM);
      rdrData._prevStazYY     = reader.GetUInt32  (CI.prevStazYY);

      rdrData._korKoef01      = reader.GetDecimal (CI.korKoef01);
      rdrData._korKoef02      = reader.GetDecimal (CI.korKoef02);
      rdrData._korKoef03      = reader.GetDecimal (CI.korKoef03);
      rdrData._korKoef04      = reader.GetDecimal (CI.korKoef04);
      rdrData._korKoef05      = reader.GetDecimal (CI.korKoef05);
      rdrData._korKoef06      = reader.GetDecimal (CI.korKoef06);
      rdrData._korKoef07      = reader.GetDecimal (CI.korKoef07);
      rdrData._korKoef08      = reader.GetDecimal (CI.korKoef08);
      rdrData._korKoef09      = reader.GetDecimal (CI.korKoef09);
      rdrData._korKoef10      = reader.GetDecimal (CI.korKoef10);
      rdrData._korKoef11      = reader.GetDecimal (CI.korKoef11);
      rdrData._korKoef12      = reader.GetDecimal (CI.korKoef12);
      
      rdrData._birthMjestoDrz = reader.GetString  (CI.birthMjestoDrz);
      rdrData._naravVrRad     = reader.GetString  (CI.naravVrRad    );
      rdrData._drzavljanstvo  = reader.GetString  (CI.drzavljanstvo );
      rdrData._dozvola        = reader.GetString  (CI.dozvola       );
      rdrData._prijava        = reader.GetString  (CI.prijava       );
      rdrData._strucno        = reader.GetString  (CI.strucno       );
      rdrData._zanimanje      = reader.GetString  (CI.zanimanje     );
      rdrData._trajanjeUgOdr  = reader.GetString  (CI.trajanjeUgOdr );
      rdrData._cl61ugSuglas   = reader.GetString  (CI.cl61ugSuglas  );
      rdrData._cl62ugSuglas   = reader.GetString  (CI.cl62ugSuglas  );
      rdrData._probniRad      = reader.GetString  (CI.probniRad     );
      rdrData._pripravStaz    = reader.GetString  (CI.pripravStaz   );
      rdrData._rezIspita      = reader.GetString  (CI.rezIspita     );
      rdrData._radIno         = reader.GetString  (CI.radIno        );
      rdrData._gdjeRadIno     = reader.GetString  (CI.gdjeRadIno    );
      rdrData._ustupRadnika   = reader.GetString  (CI.ustupRadnika  );
      rdrData._ustupMjesto    = reader.GetString  (CI.ustupMjesto   );
      rdrData._drzavaPovezDrs = reader.GetString  (CI.drzavaPovezDrs);
      rdrData._posaoBenefStaz = reader.GetString  (CI.posaoBenefStaz);
      rdrData._nacinBenef     = reader.GetString  (CI.nacinBenef    );
      rdrData._sposobnost     = reader.GetString  (CI.sposobnost    );
      rdrData._mjestoRada     = reader.GetString  (CI.mjestoRada    );
      rdrData._mirovanjeRO    = reader.GetString  (CI.mirovanjeRO   );
      rdrData._razlogOdj      = reader.GetString  (CI.razlogOdj     );
      rdrData._banka2TK       = reader.GetString  (CI.banka2TK      );
      rdrData._banka2         = reader.GetUInt32  (CI.banka2        );
      rdrData._tfs            = reader.GetDecimal (CI.tfs           );
      rdrData._skrRV          = reader.GetDecimal (CI.skrRV         );
      rdrData._isSO           = reader.GetBoolean (CI.isSO          );
      rdrData._isOtherVr      = reader.GetBoolean (CI.isOtherVr     );
      rdrData._x_brutoOsn     = reader.GetDecimal (CI.x_brutoOsn    );
      rdrData._x_godStaza     = reader.GetDecimal (CI.x_godStaza    );
      rdrData._x_koef         = reader.GetDecimal (CI.x_koef        );
      rdrData._x_prijevoz     = reader.GetDecimal (CI.x_prijevoz    );
      rdrData._x_dnFondSati   = reader.GetDecimal (CI.x_dnFondSati  );
      rdrData._x_opcCD        = reader.GetString  (CI.x_opcCD       );
      rdrData._x_opcRadCD     = reader.GetString  (CI.x_opcRadCD    );
      rdrData._x_isMioII      = reader.GetBoolean (CI.x_isMioII     );
      rdrData._x_isTrgov      = reader.GetBoolean (CI.x_isTrgov     );

      ((Person)vvDataRecord).CurrentData = rdrData;

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

   public struct PersonCI
   {
      internal int recID;
      internal int addTS;
      internal int modTS;
      internal int addUID;
      internal int modUID;
      internal int lanSrvID;
      internal int lanRecID;

      internal int personCD;
      internal int ime;
      internal int prezime;

      internal int ulica          ;
      internal int grad           ;
      internal int postaBr        ;
      internal int tel            ;
      internal int gsm            ;
      internal int email          ;
      internal int datePri        ;
      internal int dateOdj        ;
      internal int strSpr         ;
      internal int strSprCd       ;
      internal int jmbg           ;
      internal int oib            ;
      internal int regob          ;
      internal int osBrOsig       ;
      internal int banka_cd       ;
      internal int banka_tk       ;
      internal int pnbM           ;
      internal int pnbV           ;
      internal int mtros_cd       ;
      internal int mtros_tk       ;
      internal int radMj          ;
      internal int napomena       ;
      internal int isIzuzet       ;
      internal int isPlaca        ;
      internal int isUgDj         ;
      internal int isAutH         ;
      internal int isPoduz        ;
      internal int isNadzO        ;
      internal int zupan          ;
      internal int zupCd          ;
      internal int birthDate      ;
      internal int vrstaRadVrem   ;
      internal int vrstaRadOdns   ;
      internal int vrstaIsplate   ;
      internal int spol           ;
      internal int ts             ;
      internal int tsName         ;
      internal int prevStazDD     ;
      internal int prevStazMM     ;
      internal int prevStazYY     ;

      internal int korKoef01      ;
      internal int korKoef02      ;
      internal int korKoef03      ;
      internal int korKoef04      ;
      internal int korKoef05      ;
      internal int korKoef06      ;
      internal int korKoef07      ;
      internal int korKoef08      ;
      internal int korKoef09      ;
      internal int korKoef10      ;
      internal int korKoef11      ;
      internal int korKoef12      ;

      internal int birthMjestoDrz;
      internal int naravVrRad    ;
      internal int drzavljanstvo ;
      internal int dozvola       ;
      internal int prijava       ;
      internal int strucno       ;
      internal int zanimanje     ;
      internal int trajanjeUgOdr ;
      internal int cl61ugSuglas  ;
      internal int cl62ugSuglas  ;
      internal int probniRad     ;
      internal int pripravStaz   ;
      internal int rezIspita     ;
      internal int radIno        ;
      internal int gdjeRadIno    ;
      internal int ustupRadnika  ;
      internal int ustupMjesto   ;
      internal int drzavaPovezDrs;
      internal int posaoBenefStaz;
      internal int nacinBenef    ;
      internal int sposobnost    ;
      internal int mjestoRada    ;
      internal int mirovanjeRO   ;
      internal int razlogOdj     ;
      internal int banka2TK      ;
      internal int banka2        ;
      internal int tfs           ;
      internal int skrRV         ;
      internal int isSO          ;
      internal int isOtherVr     ;
      internal int x_brutoOsn    ;
      internal int x_godStaza    ;
      internal int x_koef        ;
      internal int x_prijevoz    ;
      internal int x_dnFondSati  ;
      internal int x_opcCD       ;
      internal int x_opcRadCD    ;
      internal int x_isMioII     ;
      internal int x_isTrgov     ;

      internal int origRecID;
      internal int recVer;
      internal int arAction;
      internal int arTS;
      internal int arUID;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public PersonCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID       = GetSchemaColumnIndex("recID");
      CI.addTS       = GetSchemaColumnIndex("addTS");
      CI.modTS       = GetSchemaColumnIndex("modTS");
      CI.addUID      = GetSchemaColumnIndex("addUID");
      CI.modUID      = GetSchemaColumnIndex("modUID");
      CI.lanSrvID    = GetSchemaColumnIndex("lanSrvID");
      CI.lanRecID    = GetSchemaColumnIndex("lanRecID");
      CI.personCD    = GetSchemaColumnIndex("personCD");
      CI.ime         = GetSchemaColumnIndex("ime");
      CI.prezime     = GetSchemaColumnIndex("prezime");

      CI.ulica          = GetSchemaColumnIndex("ulica");
      CI.grad           = GetSchemaColumnIndex("grad");
      CI.postaBr        = GetSchemaColumnIndex("postaBr");
      CI.tel            = GetSchemaColumnIndex("tel");
      CI.gsm            = GetSchemaColumnIndex("gsm");
      CI.email          = GetSchemaColumnIndex("email");
      CI.datePri        = GetSchemaColumnIndex("datePri");
      CI.dateOdj        = GetSchemaColumnIndex("dateOdj");
      CI.strSpr         = GetSchemaColumnIndex("strSpr");
      CI.strSprCd       = GetSchemaColumnIndex("strSprCd");
      CI.jmbg           = GetSchemaColumnIndex("jmbg");
      CI.oib            = GetSchemaColumnIndex("oib");
      CI.regob          = GetSchemaColumnIndex("regob");
      CI.osBrOsig       = GetSchemaColumnIndex("osBrOsig");
      CI.banka_cd       = GetSchemaColumnIndex("banka_cd");
      CI.banka_tk       = GetSchemaColumnIndex("banka_tk");
      CI.pnbM           = GetSchemaColumnIndex("pnbM");
      CI.pnbV           = GetSchemaColumnIndex("pnbV");
      CI.mtros_cd       = GetSchemaColumnIndex("mtros_cd");
      CI.mtros_tk       = GetSchemaColumnIndex("mtros_tk");
      CI.radMj          = GetSchemaColumnIndex("radMj");
      CI.napomena       = GetSchemaColumnIndex("napomena");
      CI.isIzuzet       = GetSchemaColumnIndex("isIzuzet");
      CI.isPlaca        = GetSchemaColumnIndex("isPlaca");
      CI.isUgDj         = GetSchemaColumnIndex("isUgDj");
      CI.isAutH         = GetSchemaColumnIndex("isAutH");
      CI.isPoduz        = GetSchemaColumnIndex("isPoduz");
      CI.isNadzO        = GetSchemaColumnIndex("isNadzO");
      CI.zupan          = GetSchemaColumnIndex("zupan");
      CI.zupCd          = GetSchemaColumnIndex("zupCd");
      CI.birthDate      = GetSchemaColumnIndex("birthDate");
      CI.vrstaRadVrem   = GetSchemaColumnIndex("vrstaRadVrem");
      CI.vrstaRadOdns   = GetSchemaColumnIndex("vrstaRadOdns");
      CI.vrstaIsplate   = GetSchemaColumnIndex("vrstaIsplate");
      CI.spol           = GetSchemaColumnIndex("spol");
      CI.ts             = GetSchemaColumnIndex("ts");
      CI.tsName         = GetSchemaColumnIndex("tsName");
      CI.prevStazDD     = GetSchemaColumnIndex("prevStazDD");
      CI.prevStazMM     = GetSchemaColumnIndex("prevStazMM");
      CI.prevStazYY     = GetSchemaColumnIndex("prevStazYY");

      CI.korKoef01      = GetSchemaColumnIndex("korKoef01");
      CI.korKoef02      = GetSchemaColumnIndex("korKoef02");
      CI.korKoef03      = GetSchemaColumnIndex("korKoef03");
      CI.korKoef04      = GetSchemaColumnIndex("korKoef04");
      CI.korKoef05      = GetSchemaColumnIndex("korKoef05");
      CI.korKoef06      = GetSchemaColumnIndex("korKoef06");
      CI.korKoef07      = GetSchemaColumnIndex("korKoef07");
      CI.korKoef08      = GetSchemaColumnIndex("korKoef08");
      CI.korKoef09      = GetSchemaColumnIndex("korKoef09");
      CI.korKoef10      = GetSchemaColumnIndex("korKoef10");
      CI.korKoef11      = GetSchemaColumnIndex("korKoef11");
      CI.korKoef12      = GetSchemaColumnIndex("korKoef12");

      CI.birthMjestoDrz      = GetSchemaColumnIndex("birthMjestoDrz");
      CI.naravVrRad          = GetSchemaColumnIndex("naravVrRad");
      CI.drzavljanstvo       = GetSchemaColumnIndex("drzavljanstvo");
      CI.dozvola             = GetSchemaColumnIndex("dozvola");
      CI.prijava             = GetSchemaColumnIndex("prijava");
      CI.strucno             = GetSchemaColumnIndex("strucno");
      CI.zanimanje           = GetSchemaColumnIndex("zanimanje");
      CI.trajanjeUgOdr       = GetSchemaColumnIndex("trajanjeUgOdr");
      CI.cl61ugSuglas        = GetSchemaColumnIndex("cl61ugSuglas");
      CI.cl62ugSuglas        = GetSchemaColumnIndex("cl62ugSuglas");
      CI.probniRad           = GetSchemaColumnIndex("probniRad");
      CI.pripravStaz         = GetSchemaColumnIndex("pripravStaz");
      CI.rezIspita           = GetSchemaColumnIndex("rezIspita");
      CI.radIno              = GetSchemaColumnIndex("radIno");
      CI.gdjeRadIno          = GetSchemaColumnIndex("gdjeRadIno");
      CI.ustupRadnika        = GetSchemaColumnIndex("ustupRadnika");
      CI.ustupMjesto         = GetSchemaColumnIndex("ustupMjesto");
      CI.drzavaPovezDrs      = GetSchemaColumnIndex("drzavaPovezDrs");
      CI.posaoBenefStaz      = GetSchemaColumnIndex("posaoBenefStaz");
      CI.nacinBenef          = GetSchemaColumnIndex("nacinBenef");
      CI.sposobnost          = GetSchemaColumnIndex("sposobnost");
      CI.mjestoRada          = GetSchemaColumnIndex("mjestoRada");
      CI.mirovanjeRO         = GetSchemaColumnIndex("mirovanjeRO");
      CI.razlogOdj           = GetSchemaColumnIndex("razlogOdj");
      CI.banka2TK            = GetSchemaColumnIndex("banka2TK");
      CI.banka2              = GetSchemaColumnIndex("banka2");
      CI.tfs                 = GetSchemaColumnIndex("tfs");
      CI.skrRV               = GetSchemaColumnIndex("skrRV");
      CI.isSO                = GetSchemaColumnIndex("isSO");
      CI.isOtherVr           = GetSchemaColumnIndex("isOtherVr");
      CI.x_brutoOsn          = GetSchemaColumnIndex("x_brutoOsn");
      CI.x_godStaza          = GetSchemaColumnIndex("x_godStaza");
      CI.x_koef              = GetSchemaColumnIndex("x_koef");
      CI.x_prijevoz          = GetSchemaColumnIndex("x_prijevoz");
      CI.x_dnFondSati        = GetSchemaColumnIndex("x_dnFondSati");
      CI.x_opcCD             = GetSchemaColumnIndex("x_opcCD");
      CI.x_opcRadCD          = GetSchemaColumnIndex("x_opcRadCD");
      CI.x_isMioII           = GetSchemaColumnIndex("x_isMioII");
      CI.x_isTrgov           = GetSchemaColumnIndex("x_isTrgov");

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
      bool inPtransRelation, inPtraneRelation, inPtranoRelation;
      int? recCount;
      uint personCD = (action == ZXC.PrivilegedAction.DELREC ? ((Person)vvDataRecord).PersonCD : ((Person)vvDataRecord).BackupedPersonCD);

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
      // For wanted personCD only                                                                                                                                            

      // Ptrans ______________________________________________________________________________________________
      DataRow drSchema = ZXC.PtransDao.TheSchemaTable.Rows[ZXC.PtransDao.CI.t_personCD];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elPersoneCD", personCD, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inPtransRelation = true;
      else             inPtransRelation = false;

      if(inPtransRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));
         Issue_RecordIsInSomeRelation_Mesage(recordName, personCD, (int)recCount);
      }

      // Ptrane ______________________________________________________________________________________________
      drSchema = ZXC.PtraneDao.TheSchemaTable.Rows[ZXC.PtraneDao.CI.t_personCD];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elPersoneCD", personCD, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inPtraneRelation = true;
      else             inPtraneRelation = false;

      if(inPtraneRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));
         Issue_RecordIsInSomeRelation_Mesage(recordName, personCD, (int)recCount);
      }

      // Ptrano ______________________________________________________________________________________________
      drSchema = ZXC.PtranoDao.TheSchemaTable.Rows[ZXC.PtranoDao.CI.t_personCD];
      filterMembers.Clear();
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elPersoneCD", personCD, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inPtranoRelation = true;
      else             inPtranoRelation = false;

      if(inPtranoRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));
         Issue_RecordIsInSomeRelation_Mesage(recordName, personCD, (int)recCount);
      }

      return (inPtransRelation || inPtraneRelation || inPtranoRelation);
   }

   #endregion IsThisRecordInSomeRelation

   #region GetPrevPtransForPerson

   public static Ptrans GetPrevPtransForPerson(XSqlConnection conn, uint wantedPersonCD, DateTime dokDateDo, uint dokNumDo, ZXC.DbNavigationRestrictor dbNavigationRestrictor)
   {
      Ptrans prevPtrans_rec = new Ptrans();

      // za VvDataRecord.SorterCurrVal: 
      prevPtrans_rec.T_personCD = wantedPersonCD;
      prevPtrans_rec.T_dokDate  = dokDateDo;
      prevPtrans_rec.T_dokNum   = dokNumDo;

      bool OK = prevPtrans_rec.VvDao.FrsPrvNxtLst_REC(conn, prevPtrans_rec, VvSQL.DBNavigActionType.PRV, Ptrans.sorter_Person_DokDate_DokNum, false, dbNavigationRestrictor, ZXC.DbNavigationRestrictor.Empty, ZXC.DbNavigationRestrictor.Empty);

      // fora da Caller ove metode zna da NEMA PrevPtrans-a! 
      if(!OK || prevPtrans_rec.T_personCD != wantedPersonCD)
      {
         prevPtrans_rec.T_recID = 0;
      }

      return prevPtrans_rec;
   }

   public static Ptrano GetPrevPtranoForPerson(XSqlConnection conn, uint wantedPersonCD, DateTime dokDateDo, uint dokNumDo, ZXC.DbNavigationRestrictor dbNavigationRestrictor, ZXC.DbNavigationRestrictor dbNavigationRestrictor2)
   {
      Ptrano prevPtrano_rec = new Ptrano();

      // za VvDataRecord.SorterCurrVal: 
      prevPtrano_rec.T_personCD = wantedPersonCD;
      prevPtrano_rec.T_dokDate  = dokDateDo;
      prevPtrano_rec.T_dokNum   = dokNumDo;

      bool OK = prevPtrano_rec.VvDao.FrsPrvNxtLst_REC(conn, prevPtrano_rec, VvSQL.DBNavigActionType.PRV, Ptrano.sorter_Person_DokDate_DokNum, false, dbNavigationRestrictor, dbNavigationRestrictor2, ZXC.DbNavigationRestrictor.Empty);

      // fora da Caller ove metode zna da NEMA PrevPtrans-a! 
      if(!OK || prevPtrano_rec.T_personCD != wantedPersonCD)
      {
         prevPtrano_rec.T_recID = 0;
      }

      return prevPtrano_rec;
   }

   #region OLD bad solution

   //public static Ptrans GetPrevPtransForPerson(XSqlConnection conn, uint wantedPersonCD, DateTime dokDateDo, uint dokNumDo, string wantedTT)
   //{
   //   Ptrans ptrano_rec = new Ptrans();

   //   using(XSqlCommand cmd = VvSQL.GTEREC_Command(conn, "*", new object[] { dokDateDo, dokNumDo }, Set_LastPtransForPerson_FilterMembers(wantedPersonCD, wantedTT), VvSQL.OrderDirectEnum.DESC, Ptrans.sorter_Person_DokDate_DokNum, 0, 1, false, false))
   //   {
   //      ZXC.PtransDao.ExecuteSingleFillFromDataReader(ptrano_rec, false, cmd);
   //   }

   //   return ptrano_rec;
   //}

   //private static List<VvSqlFilterMember> Set_LastPtransForPerson_FilterMembers(uint wantedPersonCD, string wantedTT)
   //{
   //   DataRow drSchema;
   //   List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

   //   // one t_personCD only                                                                                                                                            
   //   if(wantedPersonCD.NotZero())
   //   {
   //      drSchema = ZXC.PtransDao.TheSchemaTable.Rows[ZXC.PtransDao.CI.t_personCD];

   //      filterMembers.Add(new VvSqlFilterMember(drSchema, "elPersonCD", wantedPersonCD, " = "));
   //   }

   //   // filter for wantedTT                                                                                                                                             
   //   if(wantedTT.NotEmpty())
   //   {
   //      drSchema = ZXC.PtransDao.TheSchemaTable.Rows[ZXC.PtransDao.CI.t_tt];

   //      filterMembers.Add(new VvSqlFilterMember(drSchema, "elWantedTT", wantedTT, " = "));
   //   }

   //   return filterMembers;
   //}

   #endregion OLD bad solution

   #endregion GetPrevPtransForPerson

   #region Set_IMPORT_OFFIX_Columns

   //  //____ Specifics 2 start ______________________________________________________

   //fprintf(device, "%s\t", odohod_rec[0].o_cd);
   //fprintf(device, "%s\t", odohod_rec[0].o_ime);
   //fprintf(device, "%s\t", odohod_rec[0].o_prezime);
   ////fprintf(device, "%s\t", odohod_rec[0].o_opc_cd[5]);
   //fprintf(device, "%d\t", odohod_rec[0].o_cnd[0] == 'X' ? 1 : 0)); // isIzuzet
   //fprintf(device, "%s\t", odohod_rec[0].o_ulica);
   //fprintf(device, "%s\t", odohod_rec[0].o_grad);
   //fprintf(device, "%s\t", odohod_rec[0].o_zip);
   //fprintf(device, "%s\t", odohod_rec[0].o_sDate);
   //fprintf(device, "%s\t", odohod_rec[0].o_eDate);
   //fprintf(device, "%s\t", odohod_rec[0].o_ssprema);
   //fprintf(device, "%s\t", odohod_rec[0].o_jmbg);
   //fprintf(device, "%s\t", odohod_rec[0].o_regob);
   //fprintf(device, "%s\t", odohod_rec[0].o_osbro);
   ////fprintf(device, "%s\t", odohod_rec[0].o_opcina[31]);
   //fprintf(device, "%s\t", odohod_rec[0].o_zupanija);
   //fprintf(device, "%s\t", odohod_rec[0].o_tel);
   //fprintf(device, "%s\t", odohod_rec[0].o_radnomj);
   //fprintf(device, "%s\t", odohod_rec[0].o_orgjed); // zakelji u napomenu 
   //fprintf(device, "%s\t", odohod_rec[0].o_banka_cd);
   ////fprintf(device, "%s\t", odohod_rec[0].o_banka_name[31]); // ovo u Additional, kao i ticker 
   //fprintf(device, "%s\t", odohod_rec[0].o_pnb); // pnbV 
   //fprintf(device, "%s\t", odohod_rec[0].o_napomena1);
   //fprintf(device, "%s\t", odohod_rec[0].o_napomena2);
   //fprintf(device, "%d\t", odohod_rec[0].o_isplata[0] == 'G' ? 2 : odohod_rec[0].o_isplata[0] == 'B' ? 1 : 0));
   ////fprintf(device, "%s\t", odohod_rec[0].o_spc[2]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_hvidra[2]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_sindikat[2]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_flag1[2]); // dirNetto 
   ////fprintf(device, "%s\t", odohod_rec[0].o_MIO_II[2]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_ob_naz1[31]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_ob_naz2[31]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_ob_naz3[31]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_ob_naz4[31]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_ob_date1[9]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_ob_date2[9]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_ob_date3[9]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_ob_date4[9]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_ob_kcd1[7]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_ob_kcd2[7]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_ob_kcd3[7]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_ob_kcd4[7]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_evr_mmyy[5]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_evr_name[4][21]);
   //fprintf(device, "%s\t", odohod_rec[0].o_pnb0[3]); // pnbM 
   ////fprintf(device, "%s\t", odohod_rec[0].o_evr_OO[4][3]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_evr_od[4][3]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_evr_do[4][3]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_opc_rada_cd[5]);
   ////fprintf(device, "%s\t", odohod_rec[0].o_fuse[200]);
   //fprintf(device, "%s\t", odohod_rec[0].o_oib[12]);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_koef);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_koef2);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_stPrirez);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_prijevoz);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_netoAdd);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_bruto);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_bruto_1od2);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_satiR);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_satiR_1od2);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_satiB);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_satiB_1od2);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_gMStaza);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_ob_brR1);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_ob_brR2);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_ob_brR3);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_ob_brR4);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_ob_izn1);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_ob_izn2);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_ob_izn3);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_ob_izn4);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_zivotno);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_evr_stopa[4]);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_evr_hrs[4]);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_evr_brtOsn);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_evr_tobrok);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_evr_gStaza);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_evr_brtAdd);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_dopZdr);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_dobMio);
   ////fprintf(device, "%.2lf\t", odohod_rec[0].o_fuse_d[30]);
	
   //fprintf(device, "\n");
	
   //  //____ Specifics 2 end   ______________________________________________________

   public override string Set_IMPORT_OFFIX_Columns()
   {
      return

         "(" +

         "personCD      , " + // fprintf(device, "%s\t", odohod_rec[0].o_cd);
         "ime           , " + // fprintf(device, "%s\t", odohod_rec[0].o_ime);
         "prezime       , " + // fprintf(device, "%s\t", odohod_rec[0].o_prezime);
         "isIzuzet      , " + // fprintf(device, "%d\t", odohod_rec[0].o_cnd[0] == 'X' ? 1 : 0); // isIzuzet
         "ulica         , " + // fprintf(device, "%s\t", odohod_rec[0].o_ulica);
         "grad          , " + // fprintf(device, "%s\t", odohod_rec[0].o_grad);
         "postaBr       , " + // fprintf(device, "%s\t", odohod_rec[0].o_zip);
         "datePri       , " + // fprintf(device, "%s\t", odohod_rec[0].o_sDate);
         "dateOdj       , " + // fprintf(device, "%s\t", odohod_rec[0].o_eDate);
         "strSpr        , " + // fprintf(device, "%s\t", odohod_rec[0].o_ssprema);
         "jmbg          , " + // fprintf(device, "%s\t", odohod_rec[0].o_jmbg);
         "regob         , " + // fprintf(device, "%s\t", odohod_rec[0].o_regob);
         "osBrOsig      , " + // fprintf(device, "%s\t", odohod_rec[0].o_osbro);
         "@zupanPuse    , " + // fprintf(device, "%s\t", odohod_rec[0].o_zupanija);
         "tel           , " + // fprintf(device, "%s\t", odohod_rec[0].o_tel);
         "radMj         , " + // fprintf(device, "%s\t", odohod_rec[0].o_radnomj);
         "@napom3       , " + // fprintf(device, "%s\t", odohod_rec[0].o_orgjed); // zakelji u napomenu 
         "banka_cd      , " + // fprintf(device, "%s\t", odohod_rec[0].o_banka_cd);
         "pnbV          , " + // fprintf(device, "%s\t", odohod_rec[0].o_pnb); // pnbV 
         "@napom1       , " + // fprintf(device, "%s\t", odohod_rec[0].o_napomena1);
         "@napom2       , " + // fprintf(device, "%s\t", odohod_rec[0].o_napomena2);
         "vrstaIsplate  , " + // fprintf(device, "%d\t", odohod_rec[0].o_isplata[0] == 'G' ? 2 : odohod_rec[0].o_isplata[0] == 'B' ? 1 : 0);
         "pnbM          , " + // fprintf(device, "%s\t", odohod_rec[0].o_pnb0[3]); // pnbM 
         "oib             " + // fprintf(device, "%s\t", odohod_rec[0].o_oib[12]);

         ")"    + "\n" +

         "SET " + "\n" +

         "isPlaca = 1," + "\n" +

         "napomena = CONCAT(@napom1, ' ', @napom2, ' ', @napom3), " + "\n" +

         "addTS = CURRENT_TIMESTAMP, modTS = addTS," + "\n" +
         "addUID = '" + ZXC.vvDB_User + "'";
   }

   internal static void ImportFromOffix_Translate437_SetTickers(XSqlConnection conn)
   {
      int debugCount;

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Person>(conn, Translate437, null, "", ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out debugCount);
   }

   static bool Translate437(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      Person person_rec = vvDataRecord as Person;

      person_rec.Ime       = person_rec.Ime     .VvTranslate437ToLatin2();
      person_rec.Prezime   = person_rec.Prezime .VvTranslate437ToLatin2();
      person_rec.Ulica     = person_rec.Ulica   .VvTranslate437ToLatin2();
      person_rec.Grad      = person_rec.Grad    .VvTranslate437ToLatin2();
      person_rec.StrSpr    = person_rec.StrSpr  .VvTranslate437ToLatin2();
    //person_rec.Zupan     = person_rec.Zupan   .VvTranslate437ToLatin2();
      person_rec.RadMj     = person_rec.RadMj   .VvTranslate437ToLatin2();
      person_rec.Napomena  = person_rec.Napomena.VvTranslate437ToLatin2();

      if(person_rec.BankaCd.NotZero())
      {
         Kupdob kupdob_rec = new Kupdob(person_rec.BankaCd);

         bool OK = kupdob_rec.VvDao.SetMe_Record_bySomeUniqueColumn(conn, kupdob_rec, person_rec.BankaCd, ZXC.KupdobSchemaRows[ZXC.KpdbCI.kupdobCD], false, false);

         if(OK)
         {
            person_rec.BankaTk = kupdob_rec.Ticker;
         }
      }

      if(person_rec.MtrosCd.NotZero())
      {
         Kupdob kupdob_rec = new Kupdob(person_rec.MtrosCd);

         bool OK = kupdob_rec.VvDao.SetMe_Record_bySomeUniqueColumn(conn, kupdob_rec, person_rec.MtrosCd, ZXC.KupdobSchemaRows[ZXC.KpdbCI.kupdobCD], false, false);

         if(OK)
         {
            person_rec.MtrosTk = kupdob_rec.Ticker;
         }
      }

      return person_rec.EditedHasChanges();
   }

   #endregion Set_IMPORT_OFFIX_Columns

   public static Kupdob CreateKupdobFromPerson(XSqlConnection conn, Person person_rec)
   {
      Kupdob kupdob_rec = new Kupdob();

      uint newSifra = ZXC.KupdobDao.GetNextSifra_String(conn, Kupdob.recordName, kupdob_rec.SifraColName);

      kupdob_rec.KupdobCD = newSifra;

      kupdob_rec.Ticker   = person_rec.Ime.Substring(0,3) + person_rec.Prezime.Substring(0, 3);
      if(KupdobDao.KupdobTICKER_ForPerson_RalreadyExists(conn, kupdob_rec.Ticker, false))
      {
         for(int i=0; i < 10; ++i)
         {
            kupdob_rec.Ticker = kupdob_rec.Ticker.Substring(0, 5) + i.ToString();
            if(KupdobDao.KupdobTICKER_ForPerson_RalreadyExists(conn, kupdob_rec.Ticker, false) == false) break;
         }
         //if(i == 10)
      }

      kupdob_rec.Tip       = "R"; // Radnik 
      kupdob_rec.Naziv     = person_rec.Ime + " " + person_rec.Prezime;
      kupdob_rec.Ulica1    = person_rec.Ulica;
      kupdob_rec.Ulica2    = person_rec.Ulica;
      kupdob_rec.Grad      = person_rec.Grad;
      kupdob_rec.PostaBr   = person_rec.PostaBr;
      kupdob_rec.DugoIme   = person_rec.PrezimeIme;
      kupdob_rec.Zupan     = person_rec.Zupan;
      kupdob_rec.ZupCd     = person_rec.ZupCd;
      kupdob_rec.Ime       = person_rec.Ime;
      kupdob_rec.Prezime   = person_rec.Prezime;
      kupdob_rec.Tel1      = person_rec.Tel;
      kupdob_rec.Gsm       = person_rec.Gsm;
      kupdob_rec.Email     = person_rec.Email;
      kupdob_rec.Ziro1PnbM = person_rec.PnbM;
      kupdob_rec.Ziro1PnbV = person_rec.PnbV;
      kupdob_rec.Ziro1By   = person_rec.BankaTk;
      kupdob_rec.Regob     = person_rec.Regob;
      kupdob_rec.Napom1    = person_rec.Napomena;
      kupdob_rec.Napom2    = person_rec.PersonCD.ToString();
      kupdob_rec.Oib       = person_rec.Oib;
    //kupdob_rec.CentrID   = person_rec.MtrosCd;
    //kupdob_rec.CentrTick = person_rec.MtrosTk;
      kupdob_rec.Matbr     = person_rec.Jmbg;
      //kupdob_rec. = person_rec.;
      //kupdob_rec. = person_rec.;
      //kupdob_rec. = person_rec.;
      //kupdob_rec. = person_rec.;
      //kupdob_rec. = person_rec.;

      return kupdob_rec;
   }


}
