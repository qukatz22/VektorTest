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
using System.Collections.Generic;
using Vektor.DataLayer.DS_Reports;
#endif

public sealed class PtransDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static PtransDao instance;

   private PtransDao(XSqlConnection conn, string dbName) : base(dbName, Ptrans.recordName, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static PtransDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new PtransDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTablePtrans

   public static   uint TableVersionStatic { get { return 14; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition()
   {
      return (
         /* 00 */  "recID        int(10)      unsigned NOT NULL auto_increment,\n" +
         /* 01 */  "t_parentID   int(10)      unsigned NOT NULL               ,\n" +
         /* 02 */  "t_dokNum     int(10)      unsigned NOT NULL               ,\n" +
         /* 03 */  "t_serial     smallint(5)  unsigned NOT NULL               ,\n" +
         /* 04 */  "t_dokDate    date                  NOT NULL default '0001-01-01',\n" +
         /* 05 */  "t_tt         char(3)               NOT NULL default ''    ,\n" +
         /* 06 */  "t_ttNum      int(10)      unsigned NOT NULL               ,\n" +
         /* 07 */  "t_mmyyyy     char(6)               NOT NULL default ''    ,\n" +
         /* 08 */  "t_fondSati   decimal(6,1)          NOT NULL default '0.0' ,\n" +
         /* 09 */  "t_rSm_ID     char(5)               NOT NULL default ''    ,\n" +
         /* 10 */  "t_personCD   int(6)       unsigned NOT NULL default '0'   ,\n" +
         /* 11 */  "t_ime        varchar(24)           NOT NULL default ''    ,\n" +
         /* 12 */  "t_prezime    varchar(32)           NOT NULL default ''    ,\n" +

         /* 13 */  "t_brutoOsn   decimal(10,2)         NOT NULL default '0.00',\n" +   
         /* 14 */  "t_topObrok   decimal(10,2)         NOT NULL default '0.00',\n" +     
         /* 15 */  "t_godStaza   decimal( 4,1)         NOT NULL default '0.00',\n" +    
         /* 16 */  "t_dodBruto   decimal(10,2)         NOT NULL default '0.00',\n" +    
         /* 17 */  "t_isMioII    tinyint(1)   unsigned NOT NULL default '0'   ,\n" +  
         /* 18 */  "t_spc        tinyint(1)   unsigned NOT NULL default '0'   ,\n" +  
       ///* 19 */  "t_koef       decimal( 4,2)         NOT NULL default '0.00',\n" +
         /* 19 */  "t_koef       decimal( 6,4)         NOT NULL default '0.00',\n" +   
         /* 20 */  "t_zivotno    decimal(10,2)         NOT NULL default '0.00',\n" +   
         /* 21 */  "t_dopZdr     decimal(10,2)         NOT NULL default '0.00',\n" +   
         /* 22 */  "t_dobMIO     decimal(10,2)         NOT NULL default '0.00',\n" +   
         /* 23 */  "t_koefHRVI   decimal( 5,2)         NOT NULL default '0.00',\n" +
         /* 24 */  "t_invalidTip tinyint(1)   unsigned NOT NULL default '0'   ,\n" +  
         /* 25 */  "t_opcCD      char(5)               NOT NULL default ''    ,\n" +
         /* 26 */  "t_opcName    varchar(32)           NOT NULL default ''    ,\n" +
         /* 27 */  "t_opcRadCD   char(5)               NOT NULL default ''    ,\n" +
         /* 28 */  "t_opcRadName varchar(32)           NOT NULL default ''    ,\n" +
         /* 39 */  "t_stPrirez   decimal( 5,2)         NOT NULL default '0.00',\n" +   
         /* 30 */  "t_netoAdd    decimal(10,2)         NOT NULL default '0.00',\n" +   
         /* 31 */  "t_isDirNeto  tinyint(1)   unsigned NOT NULL default '0'   ,\n" +
         /* 32 */  "t_prijevoz   decimal(10,2)         NOT NULL default '0.00',\n" +
         /* 33 */  "t_isPoluSat  tinyint(1)   unsigned NOT NULL default '0'   ,\n" +
         /* 34 */  "t_rsB        tinyint(1)   unsigned NOT NULL default '0'   ,\n" +
         /* 35 */  "t_nacIsplCD  char(5)               NOT NULL default ''    ,\n" +
         /* 36 */  "t_neoPrimCD  char(5)               NOT NULL default ''    ,\n" +
         /* 37 */  "t_dokumCD    char(5)               NOT NULL default ''    ,\n" +
         /* 38 */  "t_brutoDodSt decimal( 5,2)         NOT NULL default '0.00',\n" +
         /* 39 */  "t_brDodPoloz decimal(10,2)         NOT NULL default '0.00',\n" +
         /* 40 */  "t_koefBruto1 decimal(10,2)         NOT NULL default '0.00',\n" +
         /* 41 */  "t_dnFondSati decimal( 5,2)         NOT NULL default '0.00',\n" +
         /* 42 */  "t_thisStazSt decimal( 5,2)         NOT NULL default '0.00',\n" +
         /* 43 */  "t_brutoDodSt2 decimal( 5,2)        NOT NULL default '0.00',\n" +
         /* 44 */  "t_brutoDodSt3 decimal( 5,2)        NOT NULL default '0.00',\n" +
         /* 45 */  "t_pr3mjBruto  decimal(10,2)        NOT NULL default '0.00',\n" +
         /* 46 */  "t_brutoKorekc decimal(10,2)        NOT NULL default '0.00',\n" +
         /* 47 */  "t_dopZdr2020  decimal(10,2)        NOT NULL default '0.00',\n" +
         /* 48 */  "t_stPorez1    decimal( 5,2)        NOT NULL default '0.00',\n" +
         /* 49 */  "t_stPorez2    decimal( 5,2)        NOT NULL default '0.00',\n" +
         /* 50 */  "t_fixMio1Olak decimal(10,2)        NOT NULL default '0.00',\n" +
         /* 51 */  "t_Mio1OlkKind tinyint(1)  unsigned NOT NULL default '0'   ,\n" +

          "PRIMARY KEY                   (recID)                                                 ,\n" +
          /*"UNIQUE*/" KEY BY_LINKER     (t_parentID, t_serial)                                  ,\n" +
          /*"UNIQUE*/" KEY BY_DOKDATE    (t_dokDate,  t_dokNum,    t_serial)                     ,\n" +
          /*"UNIQUE*/" KEY BY_PERSON     (t_personCD, t_dokDate,   t_dokNum,  t_serial)           \n"
         );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Ptrans.recordNameArhiva;
      else         tableName = Ptrans.recordName;

      switch(catchingVersion)
      {
         case 2: return ("ADD COLUMN t_nacIsplCD char(5)  NOT NULL default '' AFTER t_rsB      ,  " +
                         "ADD COLUMN t_neoPrimCD char(5)  NOT NULL default '' AFTER t_nacIsplCD,  " +
                         "ADD COLUMN t_dokumCD   char(5)  NOT NULL default '' AFTER t_neoPrimCD;\n");

         case 3: return ("MODIFY COLUMN t_rSm_ID       char(5)              NOT NULL default '';");

         case 4: return ("ADD COLUMN t_brutoDodSt decimal( 5,2) NOT NULL default '0.00' AFTER t_dokumCD;   ");
         case 5: return ("ADD COLUMN t_brDodPoloz decimal(10,2) NOT NULL default '0.00' AFTER t_brutoDodSt;");
         case 6: return ("ADD COLUMN t_koefBruto1 decimal(10,2) NOT NULL default '0.00' AFTER t_brDodPoloz;");
         case 7: return ("ADD COLUMN t_dnFondSati decimal( 5,2) NOT NULL default '0.00' AFTER t_koefBruto1;");
         case 8: return ("ADD COLUMN t_thisStazSt decimal( 5,2) NOT NULL default '0.00' AFTER t_dnFondSati;");

         case 9: return ("ADD COLUMN t_brutoDodSt2 decimal( 5,2)        NOT NULL default '0.00' AFTER t_thisStazSt ,  " +
                         "ADD COLUMN t_brutoDodSt3 decimal( 5,2)        NOT NULL default '0.00' AFTER t_brutoDodSt2,  " +
                         "ADD COLUMN t_pr3mjBruto  decimal(10,2)        NOT NULL default '0.00' AFTER t_brutoDodSt3;\n");

         case 10: return ("ADD COLUMN t_brutoKorekc decimal(10,2) NOT NULL default '0.00' AFTER t_pr3mjBruto;");
         case 11: return ("ADD COLUMN t_dopZdr2020  decimal(10,2) NOT NULL default '0.00' AFTER t_brutoKorekc;");

         case 12: return (" MODIFY COLUMN t_koef DECIMAL(6,4) NOT NULL DEFAULT '0.00';");

         case 13: return("ADD COLUMN t_stPorez1    decimal( 5,2)        NOT NULL default '0.00' AFTER t_dopZdr2020,  " +
                         "ADD COLUMN t_stPorez2    decimal( 5,2)        NOT NULL default '0.00' AFTER t_stPorez1  ,  " +
                         "ADD COLUMN t_fixMio1Olak decimal(10,2)        NOT NULL default '0.00' AFTER t_stPorez2  ;\n");

         case 14: return("ADD COLUMN t_Mio1OlkKind tinyint(1)  unsigned NOT NULL default '0' AFTER t_fixMio1Olak  ;\n");

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTablePtrans

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool dummy)
   {
      string preffix;
      Ptrans ptrans = (Ptrans)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_recID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {

      /* 01 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_parentID,   TheSchemaTable.Rows[CI.t_parentID]); 
      /* 02 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_dokNum,     TheSchemaTable.Rows[CI.t_dokNum]);
      /* 03 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_serial,     TheSchemaTable.Rows[CI.t_serial]);
      /* 04 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_dokDate,    TheSchemaTable.Rows[CI.t_dokDate]);
      /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_TT,         TheSchemaTable.Rows[CI.t_tt]);
      /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_ttNum,      TheSchemaTable.Rows[CI.t_ttNum]);
      /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_MMYYYY,     TheSchemaTable.Rows[CI.t_mmyyyy]);
      /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_FondSati,   TheSchemaTable.Rows[CI.t_fondSati]);
      /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_RSm_ID,     TheSchemaTable.Rows[CI.t_rSm_ID]);
      /* 10 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_personCD,   TheSchemaTable.Rows[CI.t_personCD]);
      /* 11 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_ime,        TheSchemaTable.Rows[CI.t_ime]);
      /* 12 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_prezime,    TheSchemaTable.Rows[CI.t_prezime]);

      /* 13 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_brutoOsn  , TheSchemaTable.Rows[CI.t_brutoOsn  ]);
      /* 14 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_topObrok  , TheSchemaTable.Rows[CI.t_topObrok  ]);
      /* 15 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_godStaza  , TheSchemaTable.Rows[CI.t_godStaza  ]);
      /* 16 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_dodBruto  , TheSchemaTable.Rows[CI.t_dodBruto  ]);
      /* 17 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_isMioII   , TheSchemaTable.Rows[CI.t_isMioII   ]);
      /* 18 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_spc       , TheSchemaTable.Rows[CI.t_spc       ]);
      /* 19 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_koef      , TheSchemaTable.Rows[CI.t_koef      ]);
      /* 20 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_zivotno   , TheSchemaTable.Rows[CI.t_zivotno   ]);
      /* 21 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_dopZdr    , TheSchemaTable.Rows[CI.t_dopZdr    ]);
      /* 22 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_dobMIO    , TheSchemaTable.Rows[CI.t_dobMIO    ]);
      /* 23 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_koefHRVI  , TheSchemaTable.Rows[CI.t_koefHRVI  ]);
      /* 24 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_invalidTip, TheSchemaTable.Rows[CI.t_invalidTip]);
      /* 25 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_opcCD     , TheSchemaTable.Rows[CI.t_opcCD     ]);
      /* 26 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_opcName   , TheSchemaTable.Rows[CI.t_opcName   ]);
      /* 27 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_opcRadCD  , TheSchemaTable.Rows[CI.t_opcRadCD  ]);
      /* 28 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_opcRadName, TheSchemaTable.Rows[CI.t_opcRadName]);
      /* 29 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_stPrirez  , TheSchemaTable.Rows[CI.t_stPrirez  ]);
      /* 30 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_NetoAdd   , TheSchemaTable.Rows[CI.t_netoAdd   ]);
      /* 31 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_isDirNeto , TheSchemaTable.Rows[CI.t_isDirNeto ]);
      /* 32 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_prijevoz  , TheSchemaTable.Rows[CI.t_prijevoz  ]);
      /* 33 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_IsPoluSat , TheSchemaTable.Rows[CI.t_isPoluSat ]);
      /* 34 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_rsB       , TheSchemaTable.Rows[CI.t_rsB       ]);
      /* 35 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_nacIsplCD , TheSchemaTable.Rows[CI.t_nacIsplCD ]);
      /* 36 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_neoPrimCD , TheSchemaTable.Rows[CI.t_neoPrimCD ]);
      /* 37 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_dokumCD   , TheSchemaTable.Rows[CI.t_dokumCD   ]);
      /* 38 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_brutoDodSt, TheSchemaTable.Rows[CI.t_brutoDodSt]);
      /* 39 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_brDodPoloz, TheSchemaTable.Rows[CI.t_brDodPoloz]);
      /* 40 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_koefBruto1, TheSchemaTable.Rows[CI.t_koefBruto1]);
      /* 41 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_dnFondSati, TheSchemaTable.Rows[CI.t_dnFondSati]);
      /* 42 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_thisStazSt, TheSchemaTable.Rows[CI.t_thisStazSt]);
      /* 43 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_brutoDodSt2,TheSchemaTable.Rows[CI.t_brutoDodSt2]);
      /* 44 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_brutoDodSt3,TheSchemaTable.Rows[CI.t_brutoDodSt3]);
      /* 45 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_pr3mjBruto ,TheSchemaTable.Rows[CI.t_pr3mjBruto ]);
      /* 46 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_brutoKorekc,TheSchemaTable.Rows[CI.t_brutoKorekc]);
      /* 47 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_dopZdr2020 ,TheSchemaTable.Rows[CI.t_dopZdr2020 ]);
      /* 48 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_stPorez1   ,TheSchemaTable.Rows[CI.t_stPorez1   ]);
      /* 49 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_stPorez2   ,TheSchemaTable.Rows[CI.t_stPorez2   ]);
      /* 50 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_fixMio1Olak,TheSchemaTable.Rows[CI.t_fixMio1Olak]);
      /* 51 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrans.T_Mio1OlkKind,TheSchemaTable.Rows[CI.t_Mio1OlkKind]);

      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva /* dummy */)
   {
      PtransStruct rdrData = new PtransStruct();

      rdrData._recID = reader.GetUInt32(CI.recID);

      //rdrData._addTS = reader.GetDateTime(1);
      //rdrData._modTS = reader.GetDateTime(2);
      //rdrData._addUID = reader.GetString(3);
      //rdrData._modUID = reader.GetString(4);


      /* 01 */ rdrData._t_parentID   = reader.GetUInt32  (CI.t_parentID);
      /* 02 */ rdrData._t_dokNum     = reader.GetUInt32  (CI.t_dokNum);
      /* 03 */ rdrData._t_serial     = reader.GetUInt16  (CI.t_serial);
      /* 04 */ rdrData._t_dokDate    = reader.GetDateTime(CI.t_dokDate);
      /* 05 */ rdrData._t_tt         = reader.GetString  (CI.t_tt);
      /* 06 */ rdrData._t_ttNum      = reader.GetUInt32  (CI.t_ttNum);
      /* 07 */ rdrData._t_mmyyyy     = reader.GetString  (CI.t_mmyyyy);
      /* 08 */ rdrData._t_fondSati   = reader.GetDecimal (CI.t_fondSati);
      /* 09 */ rdrData._t_rSm_ID     = reader.GetString  (CI.t_rSm_ID);
      /* 10 */ rdrData._t_personCD   = reader.GetUInt32  (CI.t_personCD);
      /* 11 */ rdrData._t_ime        = reader.GetString  (CI.t_ime);
      /* 12 */ rdrData._t_prezime    = reader.GetString  (CI.t_prezime);

      /* 13 */ rdrData._t_brutoOsn   = reader.GetDecimal (CI.t_brutoOsn  );
      /* 14 */ rdrData._t_topObrok   = reader.GetDecimal (CI.t_topObrok  );
      /* 15 */ rdrData._t_godStaza   = reader.GetDecimal (CI.t_godStaza  );
      /* 16 */ rdrData._t_dodBruto   = reader.GetDecimal (CI.t_dodBruto  );
      /* 17 */ rdrData._t_isMioII    = reader.GetBoolean (CI.t_isMioII   );
      /* 18 */ rdrData._t_spc        = reader.GetByte    (CI.t_spc       );
      /* 19 */ rdrData._t_koef       = reader.GetDecimal (CI.t_koef      );
      /* 20 */ rdrData._t_zivotno    = reader.GetDecimal (CI.t_zivotno   );
      /* 21 */ rdrData._t_dopZdr     = reader.GetDecimal (CI.t_dopZdr    );
      /* 22 */ rdrData._t_dobMIO     = reader.GetDecimal (CI.t_dobMIO    );
      /* 23 */ rdrData._t_koefHRVI   = reader.GetDecimal (CI.t_koefHRVI  );
      /* 24 */ rdrData._t_invalidTip = reader.GetUInt16  (CI.t_invalidTip);
      /* 25 */ rdrData._t_opcCD      = reader.GetString  (CI.t_opcCD     );
      /* 26 */ rdrData._t_opcName    = reader.GetString  (CI.t_opcName   );
      /* 27 */ rdrData._t_opcRadCD   = reader.GetString  (CI.t_opcRadCD  );
      /* 28 */ rdrData._t_opcRadName = reader.GetString  (CI.t_opcRadName);
      /* 39 */ rdrData._t_stPrirez   = reader.GetDecimal (CI.t_stPrirez  );
      /* 30 */ rdrData._t_netoAdd    = reader.GetDecimal (CI.t_netoAdd   );
      /* 31 */ rdrData._t_isDirNeto  = reader.GetBoolean (CI.t_isDirNeto );
      /* 32 */ rdrData._t_prijevoz   = reader.GetDecimal (CI.t_prijevoz  );
      /* 33 */ rdrData._t_isPoluSat  = reader.GetBoolean (CI.t_isPoluSat );
      /* 34 */ rdrData._t_rsB        = reader.GetUInt16  (CI.t_rsB       );
      /* 35 */ rdrData._t_nacIsplCD  = reader.GetString  (CI.t_nacIsplCD );
      /* 36 */ rdrData._t_neoPrimCD  = reader.GetString  (CI.t_neoPrimCD );
      /* 37 */ rdrData._t_dokumCD    = reader.GetString  (CI.t_dokumCD   );
      /* 38 */ rdrData._t_brutoDodSt = reader.GetDecimal (CI.t_brutoDodSt);
      /* 39 */ rdrData._t_brDodPoloz = reader.GetDecimal (CI.t_brDodPoloz);
      /* 40 */ rdrData._t_koefBruto1 = reader.GetDecimal (CI.t_koefBruto1);
      /* 41 */ rdrData._t_dnFondSati = reader.GetDecimal (CI.t_dnFondSati);
      /* 42 */ rdrData._t_thisStazSt = reader.GetDecimal (CI.t_thisStazSt);
      /* 43 */ rdrData._t_brutoDodSt2= reader.GetDecimal (CI.t_brutoDodSt2);
      /* 44 */ rdrData._t_brutoDodSt3= reader.GetDecimal (CI.t_brutoDodSt3);
      /* 45 */ rdrData._t_pr3mjBruto = reader.GetDecimal (CI.t_pr3mjBruto );
      /* 46 */ rdrData._t_brutoKorekc= reader.GetDecimal (CI.t_brutoKorekc);
      /* 47 */ rdrData._t_dopZdr2020 = reader.GetDecimal (CI.t_dopZdr2020 );
      /* 48 */ rdrData._t_stPorez1   = reader.GetDecimal (CI.t_stPorez1   );
      /* 49 */ rdrData._t_stPorez2   = reader.GetDecimal (CI.t_stPorez2   );
      /* 50 */ rdrData._t_fixMio1Olak= reader.GetDecimal (CI.t_fixMio1Olak);
      /* 51 */ rdrData._t_Mio1OlkKind= reader.GetByte    (CI.t_Mio1OlkKind);

      ((Ptrans)vvDataRecord).CurrentData = rdrData;

      // NE !: ((Ptrans)vvDataRecord).CalcTransResults();
      
      return;
   }

   #endregion FillFromDataReader

   #region FillFromTypedPtransDataRow

   // TODO: vidi treba li mozda za ubuduce genericki pa overridano                   
   // FillFromTypedPtransDataRow(VvDataRecord vvDataRecord, SomeTypedDataRow dataRow) 
   // fali li ti tu mozda i onih prvih 5 univerzalinih RecID, AddTS, ModTS, ...      

   public static void FillFromTypedPtransDataRow(Ptrans ptrans_rec, DS_Placa.IzvjTableRow ptransRow)
   {
      ptrans_rec.Memset0(0);
   
      /* 01 */   ptrans_rec.T_parentID   = ptransRow.t_parentID  ;
      /* 02 */   ptrans_rec.T_dokNum     = ptransRow.t_dokNum    ;
      /* 03 */   ptrans_rec.T_serial     = ptransRow.t_serial    ;
      /* 04 */   ptrans_rec.T_dokDate    = ptransRow.t_dokDate   ;
      /* 05 */   ptrans_rec.T_TT         = ptransRow.t_tt        ;
      /* 06 */   ptrans_rec.T_ttNum      = ptransRow.t_ttNum     ;
      /* 07 */   ptrans_rec.T_MMYYYY     = ptransRow.t_mmyyyy    ;
      /* 08 */   ptrans_rec.T_FondSati   = ptransRow.t_fondSati  ;
      /* 09 */   ptrans_rec.T_RSm_ID     = ptransRow.t_rSm_ID    ;
      /* 10 */   ptrans_rec.T_personCD   = ptransRow.t_personCD  ;
      /* 11 */   ptrans_rec.T_ime        = ptransRow.t_ime       ;
      /* 12 */   ptrans_rec.T_prezime    = ptransRow.t_prezime   ;
      /* 13 */   ptrans_rec.T_brutoOsn   = ptransRow.t_brutoOsn  ;
      /* 14 */   ptrans_rec.T_topObrok   = ptransRow.t_topObrok  ;
      /* 15 */   ptrans_rec.T_godStaza   = ptransRow.t_godStaza  ;
      /* 16 */   ptrans_rec.T_dodBruto   = ptransRow.t_dodBruto  ;
      /* 17 */   ptrans_rec.T_isMioII    = Convert.ToBoolean(ptransRow.t_isMioII);
      /* 18 */   ptrans_rec.T_spc        = (Ptrans.SpecEnum)ptransRow.t_spc;
      /* 19 */   ptrans_rec.T_koef       = ptransRow.t_koef      ;
      /* 20 */   ptrans_rec.T_zivotno    = ptransRow.t_zivotno   ;
      /* 21 */   ptrans_rec.T_dopZdr     = ptransRow.t_dopZdr    ;
      /* 22 */   ptrans_rec.T_dobMIO     = ptransRow.t_dobMIO    ;
      /* 23 */   ptrans_rec.T_koefHRVI   = ptransRow.t_koefHRVI  ;
      /* 24 */   ptrans_rec.T_invalidTip = (Ptrans.InvalidEnum)ptransRow.t_invalidTip;
      /* 25 */   ptrans_rec.T_opcCD      = ptransRow.t_opcCD     ;
      /* 26 */   ptrans_rec.T_opcName    = ptransRow.t_opcName   ;
      /* 27 */   ptrans_rec.T_opcRadCD   = ptransRow.t_opcRadCD  ;
      /* 28 */   ptrans_rec.T_opcRadName = ptransRow.t_opcRadName;
      /* 39 */   ptrans_rec.T_stPrirez   = ptransRow.t_stPrirez  ;
      /* 30 */   ptrans_rec.T_NetoAdd    = ptransRow.t_netoAdd   ;
      /* 31 */   ptrans_rec.T_isDirNeto  = Convert.ToBoolean(ptransRow.t_isDirNeto);
      /* 32 */   ptrans_rec.T_prijevoz   = ptransRow.t_prijevoz  ;
      /* 33 */   ptrans_rec.T_IsPoluSat  = Convert.ToBoolean(ptransRow.t_isPoluSat);
      /* 34 */   ptrans_rec.T_rsB        = ptransRow.t_rsB       ;
      /* 35 */   ptrans_rec.T_nacIsplCD  = ptransRow.t_nacIsplCD ;
      /* 36 */   ptrans_rec.T_neoPrimCD  = ptransRow.t_neoPrimCD ;
      /* 37 */   ptrans_rec.T_dokumCD    = ptransRow.t_dokumCD   ;
      /* 38 */   ptrans_rec.T_brutoDodSt = ptransRow.t_brutoDodSt;
      /* 39 */   ptrans_rec.T_brDodPoloz = ptransRow.t_brDodPoloz;
      /* 40 */   ptrans_rec.T_koefBruto1 = ptransRow.t_koefBruto1;
      /* 41 */   ptrans_rec.T_dnFondSati = ptransRow.t_dnFondSati;
      /* 42 */   ptrans_rec.T_thisStazSt = ptransRow.t_thisStazSt;
      /* 43 */   ptrans_rec.T_brutoDodSt2= ptransRow.t_brutoDodSt2;
      /* 44 */   ptrans_rec.T_brutoDodSt3= ptransRow.t_brutoDodSt3;
      /* 45 */   ptrans_rec.T_pr3mjBruto = ptransRow.t_pr3mjBruto ;
      /* 46 */   ptrans_rec.T_brutoKorekc= ptransRow.t_brutoKorekc;
      /* 47 */   ptrans_rec.T_dopZdr2020 = ptransRow.t_dopZdr2020 ;
      /* 48 */   ptrans_rec.T_stPorez1   = ptransRow.t_stPorez1   ;
      /* 49 */   ptrans_rec.T_stPorez2   = ptransRow.t_stPorez2   ;
      /* 50 */   ptrans_rec.T_fixMio1Olak= ptransRow.t_fixMio1Olak;
      /* 51 */   ptrans_rec.T_Mio1OlkKind= (Ptrans.Mio1OlkKindEnum)ptransRow.t_Mio1OlkKind;



      // ovo se vec obavi u 'OnFirstLineOfDocumentAction_InitializePlacaRec' 
      //Ptrane ptrane_rec;
      //foreach(DS_Placa.ptraneRow ptraneRow in ptraneRowsOfThisPerson)
      //{
      //   ptrane_rec = new Ptrane();
         
      //   PtraneDao.FillFromTypedPtraneDataRow(ptrane_rec, ptraneRow);

      //   placa_rec.Transes2.Add(ptrane_rec);
      //}
   }

   #endregion FillFromTypedPtransDataRow

   #region FillTypedDataRowResults

   public static void FillTypedDataRowResults(DS_Placa.IzvjTableRow ptransRow, Ptrans ptrans_rec, EnumerableRowCollection<DS_Placa.ptraneRow> ptraneRowsOfThisPerson, Placa placa_rec)
   {
      //01.12.2014.
      // ovo je za sada samo za konacni obracun poreza inace ovdje ne bi smjeli doci
      if(ptraneRowsOfThisPerson == null && placa_rec == null) 
      { 
            ptransRow.t_parentID   = 0                    ;
            ptransRow.t_prezime    = ptrans_rec.T_prezime ;
            ptransRow.t_tt         = Placa.TT_REDOVANRAD  ;
            ptransRow.t_personCD   = ptrans_rec.T_personCD;
            ptransRow.t_ime        = ptrans_rec.T_ime     ;
            ptransRow.t_opcCD      = ptrans_rec.T_opcCD   ;
            ptransRow.t_opcName    = ptrans_rec.T_opcName ;
            ptransRow.t_opcRadCD   = ptrans_rec.T_opcRadCD;
            ptransRow.t_rsB        = 0                    ;
            ptransRow.t_isPoluSat  = 0                    ;
            ptransRow.t_nacIsplCD  = "1"                  ;
            ptransRow.t_prijevoz   = 0.00M                ;
            ptransRow.t_spc        = 0                    ;
            ptransRow.t_opcCD      = ptrans_rec.T_opcCD   ;
            ptransRow.t_opcRadCD   = ptrans_rec.T_opcRadCD;
            ptransRow.t_stPrirez   = ptrans_rec.T_stPrirez;
            ptransRow.t_brDodPoloz = ptrans_rec.T_brDodPoloz ;
            ptransRow.t_brutoDodSt = ptrans_rec.T_brutoDodSt ;
            ptransRow.t_brutoOsn   = ptrans_rec.T_brutoOsn      ;
            ptransRow.t_topObrok   = ptrans_rec.T_topObrok      ;
            ptransRow.t_dnFondSati = ptrans_rec.T_dnFondSati ;
            ptransRow.t_godStaza   = ptrans_rec.T_godStaza   ;
            ptransRow.t_dodBruto   = ptrans_rec.T_dodBruto    ;
            ptransRow.t_koefBruto1 = ptrans_rec.T_koefBruto1;
            ptransRow.t_zivotno    = ptrans_rec.T_zivotno;
            ptransRow.t_dopZdr     = ptrans_rec.T_dopZdr;
            ptransRow.t_dopZdr2020 = ptrans_rec.T_dopZdr2020;
            ptransRow.t_dobMIO     = ptrans_rec.T_dobMIO;
            ptransRow.t_netoAdd    = ptrans_rec.T_NetoAdd;
            
            ptransRow.t_prijevoz   = ptrans_rec.T_prijevoz   ;
            ptransRow.t_topObrok   = ptrans_rec.T_topObrok   ;

            ptransRow.t_dokDate    = ptrans_rec.T_dokDate  ;
            
            ptransRow.t_stPorez1   = ptrans_rec.T_stPorez1   ;
            ptransRow.t_stPorez2   = ptrans_rec.T_stPorez2   ;
            ptransRow.t_fixMio1Olak= ptrans_rec.T_fixMio1Olak;
            //ptransRow.t_mmyyyy     = ptrans_rec.T_mmyyyy     ;
      }

      /* 01 */ ptransRow.R_Bruto100        = ptrans_rec.R_Bruto100        ;
      /* 02 */ ptransRow.R_TheBruto        = ptrans_rec.R_TheBruto        ;
      /* 03 */ ptransRow.R_MioOsn          = ptrans_rec.R_MioOsn          ;
      /* 04 */ ptransRow.R_Mio1stup        = ptrans_rec.R_Mio1stup        ;
      /* 05 */ ptransRow.R_Mio2stup        = ptrans_rec.R_Mio2stup        ;
      /* 06 */ ptransRow.R_MioAll          = ptrans_rec.R_MioAll          ;
      /* 07 */ ptransRow.R_DoprIz          = ptrans_rec.R_DoprIz          ;
      /* 08 */ ptransRow.R_Odbitak         = ptrans_rec.R_Odbitak         ;
      /* 09 */ ptransRow.R_Premije         = ptrans_rec.R_Premije         ;
      /* 10 */ ptransRow.R_Dohodak         = ptrans_rec.R_Dohodak         ;
      /* 11 */ ptransRow.R_PorOsnAll       = ptrans_rec.R_PorOsnAll       ;
      /* 12 */ ptransRow.R_PorOsn1         = ptrans_rec.R_PorOsn1         ;
      /* 13 */ ptransRow.R_PorOsn2         = ptrans_rec.R_PorOsn2         ;
      /* 14 */ ptransRow.R_PorOsn3         = ptrans_rec.R_PorOsn3         ;
      /* 15 */ ptransRow.R_PorOsn4         = ptrans_rec.R_PorOsn4         ;
      /* 16 */ ptransRow.R_Por1Uk          = ptrans_rec.R_Por1Uk          ;
      /* 17 */ ptransRow.R_Por2Uk          = ptrans_rec.R_Por2Uk          ;
      /* 18 */ ptransRow.R_Por3Uk          = ptrans_rec.R_Por3Uk          ;
      /* 19 */ ptransRow.R_Por4Uk          = ptrans_rec.R_Por4Uk          ;
      /* 20 */ ptransRow.R_PorezAll        = ptrans_rec.R_PorezAll        ;
      /* 21 */ ptransRow.R_Prirez          = ptrans_rec.R_Prirez          ;
      /* 22 */ ptransRow.R_PorPrirez       = ptrans_rec.R_PorPrirez       ;
      /* 23 */ ptransRow.R_Netto           = ptrans_rec.R_Netto           ;
      /* 24 */ ptransRow.R_Obustave        = ptrans_rec.R_Obustave        ;
      /* 25 */ ptransRow.R_2Pay            = ptrans_rec.R_2Pay            ;
      /* 26 */ ptransRow.R_NaRuke          = ptrans_rec.R_NaRuke          ;
      /* 27 */ ptransRow.R_ZdrNa           = ptrans_rec.R_ZdrNa           ;
      /* 28 */ ptransRow.R_ZorNa           = ptrans_rec.R_ZorNa           ;
      /* 29 */ ptransRow.R_ZapNa           = ptrans_rec.R_ZapNa           ;
      /* 30 */ ptransRow.R_ZapII           = ptrans_rec.R_ZapII           ;
      /* 31 */ ptransRow.R_ZapAll          = ptrans_rec.R_ZapAll          ;
      /* 32 */ ptransRow.R_DoprNa          = ptrans_rec.R_DoprNa          ;
      /* 33 */ ptransRow.R_DoprAll         = ptrans_rec.R_DoprAll         ;
      /* 34 */ ptransRow.R_SatiR           = ptrans_rec.R_SatiR           ;
      /* 35 */ ptransRow.R_SatiB           = ptrans_rec.R_SatiB           ;
      /* 36 */ ptransRow.R_SatiUk          = ptrans_rec.R_SatiUk          ; 
      /* 37 */ ptransRow.R_PtranEsCount    = ptrans_rec.R_PtranEsCount    ;
      /* 38 */ ptransRow.R_PtranOsCount    = ptrans_rec.R_PtranOsCount    ;
      /* 39 */ ptransRow.R_FondSatiDiff    = ptrans_rec.R_FondSatiDiff    ;
      /* 40 */ ptransRow.R_Mio1stupNa      = ptrans_rec.R_Mio1stupNa      ;
      /* 41 */ ptransRow.R_Mio2stupNa      = ptrans_rec.R_Mio2stupNa      ;
      /* 42 */ ptransRow.R_MioAllNa        = ptrans_rec.R_MioAllNa        ;
      /* 43 */ ptransRow.R_KrizPorOsn      = ptrans_rec.R_KrizPorOsn      ;
      /* 44 */ ptransRow.R_KrizPorUk       = ptrans_rec.R_KrizPorUk       ;
      /* 45 */ ptransRow.R_ZpiUk           = ptrans_rec.R_ZpiUk           ;
      /* 46 */ ptransRow.R_DaniZpi         = ptrans_rec.R_DaniZpi         ;
      /* 47 */ ptransRow.R_NettoWoAdd      = ptrans_rec.R_NettoWoAdd      ;
      /* 48 */ ptransRow.R_AHizdatak       = ptrans_rec.R_AHizdatak       ;
      /* 49 */ ptransRow.R_NettoAftKrp     = ptrans_rec.R_NettoAftKrp     ;
      /* 50 */ ptransRow.R_BrtDodNaStaz    = ptrans_rec.R_BrtDodNaStaz    ;

      if(placa_rec != null)
      /*    */ ptransRow.R_vrstaObr        = placa_rec.VrstaObr           ;
      
      /* 51 */ ptransRow.R_min_rsOd        = ptrans_rec.R_min_rsOd        ;
      /* 52 */ ptransRow.R_max_rsDo        = ptrans_rec.R_max_rsDo        ;
      /* 53 */ ptransRow.R_praznikHrs      = ptrans_rec.R_praznikHrs      ;
      /* 54 */ ptransRow.R_thisStazDod     = ptrans_rec.R_thisStazDod     ;
      /* 55 */ ptransRow.R_brutoDod2       = ptrans_rec.R_brutoDod2       ;
      /* 56 */ ptransRow.R_brutoDod3       = ptrans_rec.R_brutoDod3       ;
      /* 57 */ ptransRow.R_SatiNeR         = ptrans_rec.R_SatiNeR         ;
      /* 58 */ ptransRow.R_ukBrutoKoef     = ptrans_rec.R_ukBrutoKoef     ;
      /* 59 */ ptransRow.R_SatiOnlyRad     = ptrans_rec.R_SatiOnlyRad     ;
      /* 60 */ ptransRow.R_SatiOnlyRadBruto= ptrans_rec.R_SatiOnlyRadBruto;
      /* 61 */ ptransRow.R_PraznikHrsBruto = ptrans_rec.R_PraznikHrsBruto ;
      /* 62 */ ptransRow.R_SatiNeRBruto    = ptrans_rec.R_SatiNeRBruto    ;
      /* 63 */ ptransRow.R_brutoPoEVR      = ptrans_rec.R_brutoPoEVR      ;
      /* 63 */ ptransRow.R_osnovicaDop     = ptrans_rec.R_osnovicaDop     ;
   
      /* 65 */ ptransRow.R_Sati12	       = ptrans_rec.R_Sati12	        ; 
      /* 66 */ ptransRow.R_Bruto12         = ptrans_rec.R_Bruto12         ;
      /* 67 */ ptransRow.R_Sati13	       = ptrans_rec.R_Sati13	        ;
      /* 68 */ ptransRow.R_Bruto13         = ptrans_rec.R_Bruto13         ;
      /* 69 */ ptransRow.R_Sati14	       = ptrans_rec.R_Sati14	        ;
      /* 70 */ ptransRow.R_Bruto14         = ptrans_rec.R_Bruto14         ;
      /* 71 */ ptransRow.R_Sati15	       = ptrans_rec.R_Sati15	        ;
      /* 72 */ ptransRow.R_Bruto15         = ptrans_rec.R_Bruto15         ;
      /* 73 */ ptransRow.R_Sati16          = ptrans_rec.R_Sati16          ;
      /* 74 */ ptransRow.R_Bruto16         = ptrans_rec.R_Bruto16         ;
      /* 75 */ ptransRow.R_Sati17          = ptrans_rec.R_Sati17          ;
      /* 76 */ ptransRow.R_Bruto17         = ptrans_rec.R_Bruto17         ;
      /* 77 */ ptransRow.R_Sati20          = ptrans_rec.R_Sati20          ;
      /* 78 */ ptransRow.R_Bruto20         = ptrans_rec.R_Bruto20         ;
      /* 79 */ ptransRow.R_Sati30          = ptrans_rec.R_Sati30          ;
      /* 80 */ ptransRow.R_Bruto30         = ptrans_rec.R_Bruto30         ;
      /* 81 */ ptransRow.R_Sati40          = ptrans_rec.R_Sati40          ;
      /* 82 */ ptransRow.R_Bruto40         = ptrans_rec.R_Bruto40         ;
      /* 83 */ ptransRow.R_Sum1do3         = ptrans_rec.R_Sum1do3         ;
      /* 84 */ ptransRow.R_ZasticeniNeto   = ptrans_rec.R_ZasticeniNeto   ;
      /* 85 */ ptransRow.R_NetoIsplata     = ptrans_rec.R_NetoIsplata     ;
      /* 86 */ ptransRow.R_OnlyObustave    = ptrans_rec.R_OnlyObustave    ;
      /* 87 */ ptransRow.R_Bruto30E        = ptrans_rec.R_Bruto30E        ;
      /* 88 */ ptransRow.R_Bruto30D        = ptrans_rec.R_Bruto30D        ;
      /* 89 */ ptransRow.R_Sati18          = ptrans_rec.R_Sati18          ;
      /* 90 */ ptransRow.R_Bruto18         = ptrans_rec.R_Bruto18         ;
      /* 91 */ ptransRow.R_porIBAN         = ptrans_rec.R_porIBAN         ;
      /* 92 */ ptransRow.R_NetoWoZast      = ptrans_rec.R_NetoWoZast      ;
      /* 93 */ ptransRow.R_hasNonObust     = ptrans_rec.R_hasNonObust  == true ? (Byte)1 : (Byte)0;
               ptransRow.R_hasNonObustN    = ptrans_rec.R_hasNonObustN == true ? (Byte)1 : (Byte)0;
               ptransRow.R_hasNonObustZ    = ptrans_rec.R_hasNonObustZ == true ? (Byte)1 : (Byte)0;
               ptransRow.R_NetoALLNeto     = ptrans_rec.R_NetoALLNeto     ;
               ptransRow.R_NezasticNeto    = ptrans_rec.R_NezasticNeto    ;

               ptransRow.R_isONPN     = ptrans_rec.R_isONPN == true ? (Byte)1 : (Byte)0;
               ptransRow.R_netto_ONPN = ptrans_rec.R_netto_ONPN;
               ptransRow.R_bruto_ONPN = ptrans_rec.R_bruto_ONPN;
               ptransRow.R_satiR_ONPN = ptrans_rec.R_satiR_ONPN;
               ptransRow.R_netto_PIVR = ptrans_rec.R_netto_PIVR;
               ptransRow.R_bruto_PIVR = ptrans_rec.R_bruto_PIVR;
               ptransRow.R_satiR_PIVR = ptrans_rec.R_satiR_PIVR;

               ptransRow.R_daniR      = ptrans_rec.R_daniR     ;
               ptransRow.R_daniB      = ptrans_rec.R_daniB     ;

      /* 95 */ ptransRow.R_Netto_EUR  = ptrans_rec.R_Netto_EUR ;
      /* 96 */ ptransRow.R_Netto_Kn   = ptrans_rec.R_Netto_Kn  ;

      /* 97 */ ptransRow.R_Mio1Olk     = ptrans_rec.R_Mio1Olk  ;
      /* 98 */ ptransRow.R_Mio1Osn     = ptrans_rec.R_Mio1Osn  ;
      /* 99 */ ptransRow.R_Mio1OlkKind = (byte)ptrans_rec.R_Mio1OlkKind;


      Ptrane ptrane_rec;
      if(ptraneRowsOfThisPerson != null) foreach(DS_Placa.ptraneRow ptraneRow in ptraneRowsOfThisPerson)
      {
         //ptrane_rec = placa_rec.Transes2.SingleOrDefault(ptrane => ptrane.T_personCD  == ptraneRow.t_personCD  &&
         //                                                          ptrane.T_vrstaR_cd == ptraneRow.t_vrstaR_cd &&
         //                                                          ptrane.T_rsOD      == ptraneRow.t_rsOD      &&
         //                                                          ptrane.T_rsDO      == ptraneRow.t_rsDO);
         
         ptrane_rec = placa_rec.Transes2.SingleOrDefault(ptrane => ptrane.T_serial == ptraneRow.t_serial);
         
         /* 01 */ ptraneRow.R_EvrCijena     = ptrane_rec.R_EvrCijena    ;
         /* 02 */ ptraneRow.R_EvrBruto      = ptrane_rec.R_EvrBruto     ;
         /* 03 */ ptraneRow.R_ThisEvrCijena = ptrane_rec.R_ThisEvrCijena;
         /* 04 */ ptraneRow.R_ThisEvrBruto  = ptrane_rec.R_ThisEvrBruto ;
      }

   }

   #endregion FillTypedDataRowResults

   #region FtrCI struct & InitializeSchemaColumnIndexes()

   public struct PtransCI
   {
      internal int recID;

      /* 01 */  internal int t_parentID;
      /* 02 */  internal int t_dokNum;
      /* 03 */  internal int t_serial;
      /* 04 */  internal int t_dokDate;
      /* 05 */  internal int t_tt;
      /* 06 */  internal int t_ttNum;
      /* 07 */  internal int t_mmyyyy;
      /* 08 */  internal int t_fondSati;
      /* 09 */  internal int t_rSm_ID;
      /* 00 */  internal int t_personCD;
      /* 01 */  internal int t_ime;
      /* 02 */  internal int t_prezime;

      /* 13 */  internal int t_brutoOsn  ;   
      /* 14 */  internal int t_topObrok  ;   
      /* 15 */  internal int t_godStaza  ;   
      /* 16 */  internal int t_dodBruto  ;   
      /* 17 */  internal int t_isMioII   ;   
      /* 18 */  internal int t_spc       ;
      /* 19 */  internal int t_koef      ;
      /* 20 */  internal int t_zivotno   ;   
      /* 21 */  internal int t_dopZdr    ;   
      /* 22 */  internal int t_dobMIO    ;   
      /* 23 */  internal int t_koefHRVI  ;
      /* 24 */  internal int t_invalidTip;
      /* 25 */  internal int t_opcCD     ;
      /* 26 */  internal int t_opcName   ;
      /* 27 */  internal int t_opcRadCD  ;
      /* 28 */  internal int t_opcRadName;
      /* 29 */  internal int t_stPrirez  ;
      /* 30 */  internal int t_netoAdd   ;   
      /* 31 */  internal int t_isDirNeto ;   
      /* 32 */  internal int t_prijevoz  ;   
      /* 33 */  internal int t_isPoluSat ;   
      /* 34 */  internal int t_rsB       ;   
      /* 35 */  internal int t_nacIsplCD ;   
      /* 36 */  internal int t_neoPrimCD ;   
      /* 37 */  internal int t_dokumCD   ;   
      /* 38 */  internal int t_brutoDodSt;   
      /* 39 */  internal int t_brDodPoloz;   
      /* 40 */  internal int t_koefBruto1;   
      /* 41 */  internal int t_dnFondSati;   
      /* 42 */  internal int t_thisStazSt;   
      /* 43 */  internal int t_brutoDodSt2;   
      /* 44 */  internal int t_brutoDodSt3;   
      /* 45 */  internal int t_pr3mjBruto ;   
      /* 46 */  internal int t_brutoKorekc;   
      /* 47 */  internal int t_dopZdr2020 ;   
      /* 48 */  internal int t_stPorez1   ;
      /* 49 */  internal int t_stPorez2   ;
      /* 50 */  internal int t_fixMio1Olak;
      /* 51 */  internal int t_Mio1OlkKind;

   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public PtransCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID       = GetSchemaColumnIndex("recID");
      //FtrCI.addTS       = GetSchemaColumnIndex("addTS");
      //FtrCI.modTS       = GetSchemaColumnIndex("modTS");
      //FtrCI.addUID      = GetSchemaColumnIndex("addUID");
      //FtrCI.modUID      = GetSchemaColumnIndex("modUID");

      /* 01 */ CI.t_parentID   = GetSchemaColumnIndex("t_parentID"  );
      /* 02 */ CI.t_dokNum     = GetSchemaColumnIndex("t_dokNum"    );
      /* 03 */ CI.t_serial     = GetSchemaColumnIndex("t_serial"    );
      /* 04 */ CI.t_dokDate    = GetSchemaColumnIndex("t_dokDate"   );
      /* 05 */ CI.t_tt         = GetSchemaColumnIndex("t_tt"        );
      /* 06 */ CI.t_ttNum      = GetSchemaColumnIndex("t_ttNum"     );
      /* 07 */ CI.t_mmyyyy     = GetSchemaColumnIndex("t_mmyyyy"    );
      /* 08 */ CI.t_fondSati   = GetSchemaColumnIndex("t_fondSati"  );
      /* 09 */ CI.t_rSm_ID     = GetSchemaColumnIndex("t_rSm_ID"    );
      /* 10 */ CI.t_personCD   = GetSchemaColumnIndex("t_personCD"  );
      /* 11 */ CI.t_ime        = GetSchemaColumnIndex("t_ime"       );
      /* 12 */ CI.t_prezime    = GetSchemaColumnIndex("t_prezime"   );

      /* 13 */ CI.t_brutoOsn   = GetSchemaColumnIndex("t_brutoOsn"  );
      /* 14 */ CI.t_topObrok   = GetSchemaColumnIndex("t_topObrok"  );
      /* 15 */ CI.t_godStaza   = GetSchemaColumnIndex("t_godStaza"  );
      /* 16 */ CI.t_dodBruto   = GetSchemaColumnIndex("t_dodBruto"  );
      /* 17 */ CI.t_isMioII    = GetSchemaColumnIndex("t_isMioII"   );
      /* 18 */ CI.t_spc        = GetSchemaColumnIndex("t_spc"       );
      /* 29 */ CI.t_koef       = GetSchemaColumnIndex("t_koef"      );
      /* 20 */ CI.t_zivotno    = GetSchemaColumnIndex("t_zivotno"   );
      /* 21 */ CI.t_dopZdr     = GetSchemaColumnIndex("t_dopZdr"    );
      /* 22 */ CI.t_dobMIO     = GetSchemaColumnIndex("t_dobMIO"    );
      /* 23 */ CI.t_koefHRVI   = GetSchemaColumnIndex("t_koefHRVI"  );
      /* 24 */ CI.t_invalidTip = GetSchemaColumnIndex("t_invalidTip");
      /* 25 */ CI.t_opcCD      = GetSchemaColumnIndex("t_opcCD"     );
      /* 26 */ CI.t_opcName    = GetSchemaColumnIndex("t_opcName"   );
      /* 27 */ CI.t_opcRadCD   = GetSchemaColumnIndex("t_opcRadCD"  );
      /* 28 */ CI.t_opcRadName = GetSchemaColumnIndex("t_opcRadName");
      /* 39 */ CI.t_stPrirez   = GetSchemaColumnIndex("t_stPrirez"  );
      /* 30 */ CI.t_netoAdd    = GetSchemaColumnIndex("t_netoAdd"   );
      /* 31 */ CI.t_isDirNeto  = GetSchemaColumnIndex("t_isDirNeto" );
      /* 32 */ CI.t_prijevoz   = GetSchemaColumnIndex("t_prijevoz"  );
      /* 33 */ CI.t_isPoluSat  = GetSchemaColumnIndex("t_isPoluSat" );
      /* 34 */ CI.t_rsB        = GetSchemaColumnIndex("t_rsB"       );
      /* 35 */ CI.t_nacIsplCD  = GetSchemaColumnIndex("t_nacIsplCD" );
      /* 36 */ CI.t_neoPrimCD  = GetSchemaColumnIndex("t_neoPrimCD" );
      /* 37 */ CI.t_dokumCD    = GetSchemaColumnIndex("t_dokumCD"   );
      /* 38 */ CI.t_brutoDodSt = GetSchemaColumnIndex("t_brutoDodSt");
      /* 39 */ CI.t_brDodPoloz = GetSchemaColumnIndex("t_brDodPoloz");
      /* 40 */ CI.t_koefBruto1 = GetSchemaColumnIndex("t_koefBruto1");
      /* 41 */ CI.t_dnFondSati = GetSchemaColumnIndex("t_dnFondSati");
      /* 42 */ CI.t_thisStazSt = GetSchemaColumnIndex("t_thisStazSt");
      /* 43 */ CI.t_brutoDodSt2= GetSchemaColumnIndex("t_brutoDodSt2");
      /* 44 */ CI.t_brutoDodSt3= GetSchemaColumnIndex("t_brutoDodSt3");
      /* 45 */ CI.t_pr3mjBruto = GetSchemaColumnIndex("t_pr3mjBruto ");
      /* 46 */ CI.t_brutoKorekc= GetSchemaColumnIndex("t_brutoKorekc");
      /* 47 */ CI.t_dopZdr2020 = GetSchemaColumnIndex("t_dopZdr2020" );
      /* 48 */ CI.t_stPorez1   = GetSchemaColumnIndex("t_stPorez1"   );
      /* 49 */ CI.t_stPorez2   = GetSchemaColumnIndex("t_stPorez2"   );
      /* 50 */ CI.t_fixMio1Olak= GetSchemaColumnIndex("t_fixMio1Olak");
      /* 51 */ CI.t_Mio1OlkKind= GetSchemaColumnIndex("t_Mio1OlkKind");

   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()

   #region KONACNI OBRACUN POREZA alfa13, beta14, gama15

   internal static Ptrans GetPtransSum_alfa13(XSqlConnection conn, uint personCD, out List<Ptrans> thisPersonWholeYearPtransList)
   {
      Ptrans ptransSumRec_alfa13 = new Ptrans();

      // odi u dataLayer i nabavi sve ptranse po ovom personu...
      // sumiraj to sve kao jedan ptrans 

      thisPersonWholeYearPtransList = new List<Ptrans>();

      List<VvSqlFilterMember> filterMembers_alfa13 = Set_FilterMembers_alfa13(personCD/*, dokDate, dokNum*/);

      LoadGenericVvDataRecordList<Ptrans>(conn, thisPersonWholeYearPtransList, filterMembers_alfa13, "");

      if(thisPersonWholeYearPtransList.Count == 0) return null;

      Placa placa_rec = new Placa();

      if(ThisPersonHasNot12Placa_or_PrirezNotKonzistentan(thisPersonWholeYearPtransList)) return null;

      // jebiga dakle, sada moras za ovaj dolazni Ptrans                              
      // 1. naci sve prosle Ptrans-ove u mjesecu,                                     
      // 2. pa naci njihova zaglavlja (Placa)                                         
      // 3. pa jos doljepiti Placi Ptrane-ove ali samo za ovog Person-a               
      // 4. da bi mogao pravilno izracunati CalcTransResult za Ptrans-ove iz tocke 1. 
      // Jer CalcTransResults treba i Placa_rec i Placa_rec.Transes2 za obracun...    
      foreach(Ptrans ptrans_rec in thisPersonWholeYearPtransList)
      {
         ZXC.PlacaDao.SetMe_Record_byRecID(conn, placa_rec, ptrans_rec.T_parentID, false);

         List<VvSqlFilterMember> filterMembers_2 = Set_FilterMembers_2(placa_rec.RecID, personCD);

         LoadGenericVvDataRecordList<Ptrane>(conn, placa_rec.Transes2, filterMembers_2, ""); 

         ptrans_rec.CalcTransResults(placa_rec);

         
         ptransSumRec_alfa13.T_parentID    = 0;
         ptransSumRec_alfa13.T_prezime     = ptrans_rec.T_prezime;
         ptransSumRec_alfa13.T_ime         = ptrans_rec.T_ime;
         ptransSumRec_alfa13.T_personCD    = ptrans_rec.T_personCD;
         ptransSumRec_alfa13.T_opcCD       = ptrans_rec.T_opcCD   ;
         ptransSumRec_alfa13.T_opcName     = ptrans_rec.T_opcName ;
         ptransSumRec_alfa13.T_opcRadCD    = ptrans_rec.T_opcRadCD;
         ptransSumRec_alfa13.T_stPrirez    = ptrans_rec.T_stPrirez;

         ptransSumRec_alfa13.T_dokDate     = ptrans_rec.T_dokDate;

         if(placa_rec.VrstaObr != "00")
         {
            ptransSumRec_alfa13.R_TheBruto    += ptrans_rec.R_TheBruto   .Ron2();
            ptransSumRec_alfa13.R_DoprIz      += ptrans_rec.R_DoprIz     .Ron2();
            ptransSumRec_alfa13.R_Dohodak     += ptrans_rec.R_Dohodak    .Ron2();
            ptransSumRec_alfa13.R_Odbitak     += ptrans_rec.R_Odbitak    .Ron2();
            ptransSumRec_alfa13.R_PorOsnAll   += ptrans_rec.R_PorOsnAll  .Ron2();
            ptransSumRec_alfa13.R_PorOsn1     += ptrans_rec.R_PorOsn1    .Ron2();
            ptransSumRec_alfa13.R_PorOsn2     += ptrans_rec.R_PorOsn2    .Ron2();
            ptransSumRec_alfa13.R_PorOsn3     += ptrans_rec.R_PorOsn3    .Ron2();
            ptransSumRec_alfa13.R_PorezAll    += ptrans_rec.R_PorezAll   .Ron2();
            ptransSumRec_alfa13.R_Prirez      += ptrans_rec.R_Prirez     .Ron2();
            ptransSumRec_alfa13.R_PorPrirez   += ptrans_rec.R_PorPrirez  .Ron2();
            ptransSumRec_alfa13.R_NettoAftKrp += ptrans_rec.R_NettoAftKrp.Ron2();
            ptransSumRec_alfa13.R_Netto       += ptrans_rec.R_Netto      .Ron2();

            // 03.12.2015: mozda i nepotrebno, a mozda i smeta?! start
            ptransSumRec_alfa13.R_Por1Uk      += ptrans_rec.R_Por1Uk     .Ron2();
            ptransSumRec_alfa13.R_Por2Uk      += ptrans_rec.R_Por2Uk     .Ron2();
            ptransSumRec_alfa13.R_Por3Uk      += ptrans_rec.R_Por3Uk     .Ron2();
            // 03.12.2015: mozda i nepotrebno, a mozda i smeta?! end 
         }
      }

      return ptransSumRec_alfa13;

   }

   private static bool ThisPersonHasNot12Placa_or_PrirezNotKonzistentan(List<Ptrans> ptransList)
   {
      bool thisPersonHasNot12Placa;
      bool prirezNotKonzistentan  ;

      int mjeseciCount = ptransList.Where(pt => pt.T_TT == Placa.TT_REDOVANRAD).Select(ptr => ptr.T_dokDate.Month).Distinct().Count();

      thisPersonHasNot12Placa = (mjeseciCount < 12);

      int prirezKindCount = ptransList.GroupBy(ptr => ptr.T_opcCD).Count();

      prirezNotKonzistentan = (prirezKindCount != 1);

      return thisPersonHasNot12Placa || prirezNotKonzistentan;
   }

   private static List<VvSqlFilterMember> Set_FilterMembers_alfa13(uint personCD/*, DateTime dokDate, uint dokNum*/)
   {
      DataRow drSchema;
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(5);

      // For TT 'Redovna Placa' only                                                                                                                                            
      drSchema = ZXC.PtransDao.TheSchemaTable.Rows[ZXC.PtransDao.CI.t_tt];
      filterMembers.Add(new VvSqlFilterMember(drSchema, ZXC.FM_OR_Enum.OPEN_OR , false, "elTTor1", Placa.TT_REDOVANRAD  , "", "", " = ", ""));
      filterMembers.Add(new VvSqlFilterMember(drSchema, ZXC.FM_OR_Enum.CLOSE_OR, false, "elTTor2", Placa.TT_PLACAUNARAVI, "", "", " = ", ""));
    //filterMembers.Add(new VvSqlFilterMember(drSchema, ZXC.FM_OR_Enum.CLOSE_OR, false, "elTTor2", Placa.TT_NEPLACDOPUST, "", "", " = ", ""));

      // For wanted personCD only                                                                                                                                            
      drSchema = ZXC.PtransDao.TheSchemaTable.Rows[ZXC.PtransDao.CI.t_personCD];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elPerson", personCD, " = "));

      // Za uplate u GODINI                                                                                                                                                 
      drSchema = ZXC.PtransDao.TheSchemaTable.Rows[ZXC.PtransDao.CI.t_dokDate];

      DateTime dateOd, dateDo;

      dateOd = ZXC.projectYearFirstDay;
      dateDo = ZXC.projectYearLastDay ;

      filterMembers.Add(new VvSqlFilterMember(drSchema, "elDateOD", dateOd, " >= "));
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elDateDO", dateDo, " <= "));

      return filterMembers;
   }

   internal static Ptrans GetPtransSum_beta14(Ptrans ptransSumRec_alfa13, PrulesStruct pR, DS_Placa.personRow personRowOfCurrentTrans, List<Ptrans> thisPersonWholeYearPtransList)
   {
      Ptrans ptransSumRec_beta14 = new Ptrans();

      decimal godMaxPorOsn1 = pR._maxPorOsn1 * 12;
      decimal godMaxPorOsn2 = pR._maxPorOsn2 * 12;

      ptransSumRec_beta14.T_parentID    = 0                            ;
      ptransSumRec_beta14.T_prezime     = ptransSumRec_alfa13.T_prezime;
      ptransSumRec_beta14.T_ime         = ptransSumRec_alfa13.T_ime;
      ptransSumRec_beta14.T_personCD    = ptransSumRec_alfa13.T_personCD;
      ptransSumRec_beta14.T_opcCD       = ptransSumRec_alfa13.T_opcCD   ;
      ptransSumRec_beta14.T_opcName     = ptransSumRec_alfa13.T_opcName ;
      ptransSumRec_beta14.T_opcRadCD    = ptransSumRec_alfa13.T_opcRadCD;
      ptransSumRec_beta14.T_stPrirez    = ptransSumRec_alfa13.T_stPrirez;


      ptransSumRec_beta14.R_Odbitak     = ZakonomDozvoljenFondOdbitka(personRowOfCurrentTrans, thisPersonWholeYearPtransList, pR);
      
      ptransSumRec_beta14.R_TheBruto    = ptransSumRec_alfa13.R_TheBruto   ;
      ptransSumRec_beta14.R_DoprIz      = ptransSumRec_alfa13.R_DoprIz     ;
      ptransSumRec_beta14.R_Dohodak     = ptransSumRec_alfa13.R_Dohodak    ;
                                        
      ptransSumRec_beta14.R_PorOsnAll   = (ptransSumRec_beta14.R_Dohodak - ptransSumRec_beta14.R_Odbitak).Ron2();
                                        
      ptransSumRec_beta14.R_PorOsn1     = ptransSumRec_beta14.R_PorOsnAll >  godMaxPorOsn1 ?  godMaxPorOsn1 : ptransSumRec_beta14.R_PorOsnAll;
                                        
      ptransSumRec_beta14.R_PorOsn2     = ptransSumRec_beta14.R_PorOsnAll > godMaxPorOsn2 ? godMaxPorOsn2 - godMaxPorOsn1 :
                                                                            (ptransSumRec_beta14.R_PorOsnAll < godMaxPorOsn2 && ptransSumRec_beta14.R_PorOsnAll > godMaxPorOsn1) ? ptransSumRec_beta14.R_PorOsnAll - godMaxPorOsn1 : 0.00M;
                                        
      ptransSumRec_beta14.R_PorOsn3     = ptransSumRec_beta14.R_PorOsnAll > godMaxPorOsn2 ? ptransSumRec_beta14.R_PorOsnAll - godMaxPorOsn2 : 0.00M ;

      // 03.12.2015: mozda i nepotrebno, a mozda i smeta?! start 
      ptransSumRec_beta14.R_Por1Uk      = (ptransSumRec_beta14.R_PorOsn1 * pR._stpor1 / 100.00M).Ron2();
      ptransSumRec_beta14.R_Por2Uk      = (ptransSumRec_beta14.R_PorOsn2 * pR._stpor2 / 100.00M).Ron2();
      ptransSumRec_beta14.R_Por3Uk      = (ptransSumRec_beta14.R_PorOsn3 * pR._stpor3 / 100.00M).Ron2();
      // 03.12.2015: mozda i nepotrebno, a mozda i smeta?! end 

      ptransSumRec_beta14.R_PorezAll    = (ptransSumRec_beta14.R_PorOsn1 * pR._stpor1 / 100.00M).Ron2() + 
                                          (ptransSumRec_beta14.R_PorOsn2 * pR._stpor2 / 100.00M).Ron2() +
                                          (ptransSumRec_beta14.R_PorOsn3 * pR._stpor3 / 100.00M).Ron2();
                                        
      ptransSumRec_beta14.R_Prirez      = (ptransSumRec_beta14.R_PorezAll * ptransSumRec_alfa13.T_stPrirez / 100.00M).Ron2();

      ptransSumRec_beta14.R_NettoAftKrp = 
      ptransSumRec_beta14.R_Netto       = ptransSumRec_beta14.R_Dohodak - ptransSumRec_beta14.R_PorezAll - ptransSumRec_beta14.R_Prirez;

      ptransSumRec_beta14.R_PorPrirez   = ptransSumRec_beta14.R_PorezAll + ptransSumRec_beta14.R_Prirez;

      // koliko je za radnika uplaceno poreza toliko eventualno moze dobiti natrag a ne vise!!!!! OVO DOLJE NEVALJA
      if(ptransSumRec_beta14.R_PorPrirez * -1.00M > ptransSumRec_alfa13.R_PorPrirez/* || ptransSumRec_beta14.R_PorOsnAll < ptransSumRec_alfa13.R_PorOsnAll*/)
      {
         if(ptransSumRec_beta14.R_PorOsnAll < 0.00M )
         {
            ptransSumRec_beta14.R_PorOsnAll =
            ptransSumRec_beta14.R_PorOsn1   =
            ptransSumRec_beta14.R_PorOsn2   =
            ptransSumRec_beta14.R_PorOsn3   = 0.00M;
         }

         //ptransSumRec_beta14.R_PorezAll    = 
         //ptransSumRec_beta14.R_Prirez      = 
         //ptransSumRec_beta14.R_PorPrirez   = 
         //ptransSumRec_beta14.R_NettoAftKrp = 
         //ptransSumRec_beta14.R_Netto       = 0.00M;   
         
         //01.12.20105.
         ptransSumRec_beta14.R_PorezAll    = 
         ptransSumRec_beta14.R_Prirez      = 
         ptransSumRec_beta14.R_PorPrirez   = 0.00M;
         ptransSumRec_beta14.R_NettoAftKrp = 
         ptransSumRec_beta14.R_Netto       = ptransSumRec_beta14.R_Dohodak;     

      
      }

      // 01.12.2015. ako je R_PorOsnAll manja od 0 onda je zapravo 0 ne moze biti minus
      if(ptransSumRec_beta14.R_PorOsnAll < 0)
      {
            ptransSumRec_beta14.R_PorOsnAll   =
            ptransSumRec_beta14.R_PorOsn1     =
            ptransSumRec_beta14.R_PorOsn2     =
            ptransSumRec_beta14.R_PorOsn3     = 
            ptransSumRec_beta14.R_PorezAll    =
            ptransSumRec_beta14.R_Prirez      =
            ptransSumRec_beta14.R_PorPrirez   = 0.00M;
            ptransSumRec_beta14.R_NettoAftKrp =
            ptransSumRec_beta14.R_Netto       = ptransSumRec_beta14.R_Dohodak;  

      }
         
         return ptransSumRec_beta14;
   }

   private static decimal ZakonomDozvoljenFondOdbitka(DS_Placa.personRow personRowOfCurrentTrans, List<Ptrans> thisPersonWholeYearPtransList, PrulesStruct pR)
   {
      decimal fondOdbitka = 0M;
      decimal usedKoef;
      decimal korKoef;
      decimal ispravanKoef;

      var monthlyPtransesOfThisPerson = thisPersonWholeYearPtransList.GroupBy(ptr => ptr.T_dokDate.Month);

    // 30.11.2017:
    //foreach(var ptrGR in monthlyPtransesOfThisPerson)
    //{
    //   usedKoef     = ptrGR.First().T_koef;
    //   korKoef      = GetKorKoefForMonth(ptrGR.Key, personRowOfCurrentTrans);
    //   ispravanKoef = korKoef.IsZero() ? usedKoef : korKoef;
    //
    //   fondOdbitka += (ispravanKoef * pR._osnOdb).Ron2();
    //}
      foreach(var ptrGR in monthlyPtransesOfThisPerson)
      {
         usedKoef     = ptrGR.First().T_koef;
         korKoef      = GetKorKoefForMonth(ptrGR.Key, personRowOfCurrentTrans);
         ispravanKoef = korKoef.IsZero() ? usedKoef : korKoef;

   // novo za euro eru u 2023 a vjerujem da će trebati novo i za kraj 2024
       //if(ptrGR.First().T_dokDate >= ZXC.Date01012017) // novo za isplate od 01.01.2017.
       //{
       // //26.11.2020.!!!!!! koeficijent za alfu se promjenio u 2020 godini pa ga treba promjeniti i ovdje
       // // zato dodajemo ovo kao i na ptransu:
       //   decimal koefZaOsnOdb = ptrGR.First().T_dokDate < ZXC.Date01012020 ? 1.50M : 1.60M; // 23.12.2019. od 01.01.2020. je ovaj koef 1.60 tj osnovni osobni odbitak je 4000
       //   
       // //decimal alfa = ZXC.Ron((pR._osnOdb * 1.50M       ) / 100M, 0) * 100M; // alfa = osnOdb2017 x 1.5          ... zaokruzen na 100 kn = 3.800 
       //   decimal alfa = ZXC.Ron((pR._osnOdb * koefZaOsnOdb) / 100M, 0) * 100M; // .. od 2020 je 4000
       //
       //   decimal beta = pR._osnOdb * (ispravanKoef - 1.00M)                  ; // beta = osnOdb2017 x koefOvisanOdDjece                   
       //                                                                         //R_Odbitak = T_koef.NotZero() ? alfa + beta : 0.00M;
       // //fondOdbitka +=                          (alfa + beta)        .Ron2();
       //   fondOdbitka += (ispravanKoef.NotZero() ? alfa + beta : 0.00M).Ron2();
       //}
       //else // po starom do 31.12.2016.
       //{
       //   fondOdbitka += (ispravanKoef * pR._osnOdb).Ron2();
       //}

         if(ptrGR.First().T_dokDate < ZXC.Date01012017)// dpo 31.12.2016
         {
            fondOdbitka += (ispravanKoef * pR._osnOdb).Ron2();
         }
         else if(ptrGR.First().T_dokDate >= ZXC.Date01012017 && ptrGR.First().T_dokDate < ZXC.EURoERAstart)
         {
          //26.11.2020.!!!!!! koeficijent za alfu se promjenio u 2020 godini pa ga treba promjeniti i ovdje
          // zato dodajemo ovo kao i na ptransu:
            decimal koefZaOsnOdb = ptrGR.First().T_dokDate < ZXC.Date01012020 ? 1.50M : 1.60M; // 23.12.2019. od 01.01.2020. je ovaj koef 1.60 tj osnovni osobni odbitak je 4000
            decimal alfa         = ZXC.Ron((pR._osnOdb * koefZaOsnOdb) / 100M, 0) * 100M; // .. od 2020 je 4000
            decimal beta         = pR._osnOdb * (ispravanKoef - 1.00M)                  ; // beta = osnOdb2017 x koefOvisanOdDjece                   
                                                                                          //R_Odbitak = T_koef.NotZero() ? alfa + beta : 0.00M;
            fondOdbitka += (ispravanKoef.NotZero() ? alfa + beta : 0.00M).Ron2();
         }
         else//EURoERaStart 2023 godina NOVO!!!!!
         { 
            bool plUmjesecu_isSijecanj2023 = ptrGR.First().T_dokDate <= ZXC.Date31012023;
            decimal koefZaOsnOdb = 1.60M; 
            decimal alfa         = pR._osnOdb * koefZaOsnOdb          ; // ovdje ga ne zaokruzujemo jer je on yapravo = 4.000 kn pa ga treba samo preracunati u eur-e tj u pR._osnOdb vec jesu euri 
            decimal beta         = pR._osnOdb * (ispravanKoef - 1.00M); // beta = osnOdb x koefOvisanOdDjece                                                         

            if(!plUmjesecu_isSijecanj2023)
            {
               alfa = alfa.Ron2();
               beta = beta.Ron2();
            }

            if(ispravanKoef < 1.00M) fondOdbitka += ZXC.Ron2(alfa * ispravanKoef);
            else                     fondOdbitka += (alfa + beta)                        ;
         
         
         }

      }

      return fondOdbitka.Ron2();
   }

   private static decimal GetKorKoefForMonth(int month, DS_Placa.personRow personRowOfCurrentTrans)
   {
      if(personRowOfCurrentTrans == null) return 0M;

      switch(month)
      {
         case 01: return personRowOfCurrentTrans.korKoef01;
         case 02: return personRowOfCurrentTrans.korKoef02;
         case 03: return personRowOfCurrentTrans.korKoef03;
         case 04: return personRowOfCurrentTrans.korKoef04;
         case 05: return personRowOfCurrentTrans.korKoef05;
         case 06: return personRowOfCurrentTrans.korKoef06;
         case 07: return personRowOfCurrentTrans.korKoef07;
         case 08: return personRowOfCurrentTrans.korKoef08;
         case 09: return personRowOfCurrentTrans.korKoef09;
         case 10: return personRowOfCurrentTrans.korKoef10;
         case 11: return personRowOfCurrentTrans.korKoef11;
         case 12: return personRowOfCurrentTrans.korKoef12;

         default: return 0M;
      }
   }

   internal static Ptrans GetPtransSum_gama15(Ptrans ptransSumRec_alfa13, Ptrans ptransSumRec_beta14)
   {
      Ptrans ptransSumRec_gama15 = new Ptrans();

      ptransSumRec_gama15.T_parentID = 0                             ;
      ptransSumRec_gama15.T_prezime  = ptransSumRec_alfa13.T_prezime ;
      ptransSumRec_gama15.T_ime      = ptransSumRec_alfa13.T_ime     ;
      ptransSumRec_gama15.T_personCD = ptransSumRec_alfa13.T_personCD;
      ptransSumRec_gama15.T_TT       = ptransSumRec_alfa13.T_TT      ;
      ptransSumRec_gama15.T_opcCD    = ptransSumRec_alfa13.T_opcCD   ;
      ptransSumRec_gama15.T_opcName  = ptransSumRec_alfa13.T_opcName ;
      ptransSumRec_gama15.T_opcRadCD = ptransSumRec_alfa13.T_opcRadCD;
      ptransSumRec_gama15.T_stPrirez = ptransSumRec_alfa13.T_stPrirez;

      ptransSumRec_gama15.T_dokDate  = ptransSumRec_alfa13.T_dokDate ;

      ptransSumRec_gama15.R_TheBruto    = (ptransSumRec_beta14.R_TheBruto    - ptransSumRec_alfa13.R_TheBruto   ).Ron2();
      ptransSumRec_gama15.R_DoprIz      = (ptransSumRec_beta14.R_DoprIz      - ptransSumRec_alfa13.R_DoprIz     ).Ron2();
      ptransSumRec_gama15.R_Dohodak     = (ptransSumRec_beta14.R_Dohodak     - ptransSumRec_alfa13.R_Dohodak    ).Ron2();
      ptransSumRec_gama15.R_Odbitak     = (ptransSumRec_beta14.R_Odbitak     - ptransSumRec_alfa13.R_Odbitak    ).Ron2();
      ptransSumRec_gama15.R_PorOsnAll   = (ptransSumRec_beta14.R_PorOsnAll   - ptransSumRec_alfa13.R_PorOsnAll  ).Ron2();
      ptransSumRec_gama15.R_PorOsn1     = (ptransSumRec_beta14.R_PorOsn1     - ptransSumRec_alfa13.R_PorOsn1    ).Ron2();
      ptransSumRec_gama15.R_PorOsn2     = (ptransSumRec_beta14.R_PorOsn2     - ptransSumRec_alfa13.R_PorOsn2    ).Ron2();
      ptransSumRec_gama15.R_PorOsn3     = (ptransSumRec_beta14.R_PorOsn3     - ptransSumRec_alfa13.R_PorOsn3    ).Ron2();

      // 03.12.2015: mozda i nepotrebno, a mozda i smeta?! start 
      ptransSumRec_gama15.R_Por1Uk      = (ptransSumRec_beta14.R_Por1Uk      - ptransSumRec_alfa13.R_Por1Uk     ).Ron2();
      ptransSumRec_gama15.R_Por2Uk      = (ptransSumRec_beta14.R_Por2Uk      - ptransSumRec_alfa13.R_Por2Uk     ).Ron2();
      ptransSumRec_gama15.R_Por3Uk      = (ptransSumRec_beta14.R_Por3Uk      - ptransSumRec_alfa13.R_Por3Uk     ).Ron2();
      // 03.12.2015: mozda i nepotrebno, a mozda i smeta?! end 

      ptransSumRec_gama15.R_PorezAll    = (ptransSumRec_beta14.R_PorezAll    - ptransSumRec_alfa13.R_PorezAll   ).Ron2();
      ptransSumRec_gama15.R_Prirez      = (ptransSumRec_beta14.R_Prirez      - ptransSumRec_alfa13.R_Prirez     ).Ron2();

      // 05.12.2014. ako je obracunati porez za povrat veci od uplacenog
      if(ptransSumRec_gama15.R_PorezAll * -1.00M > ptransSumRec_alfa13.R_PorezAll)
      {
         // 03.12.2015: mozda i nepotrebno, a mozda i smeta?! start 
         ptransSumRec_gama15.R_Por1Uk   = (-1.00M * ptransSumRec_alfa13.R_Por1Uk).Ron2();
         ptransSumRec_gama15.R_Por2Uk   = (-1.00M * ptransSumRec_alfa13.R_Por2Uk).Ron2();
         ptransSumRec_gama15.R_Por3Uk   = (-1.00M * ptransSumRec_alfa13.R_Por3Uk).Ron2();
         // 03.12.2015: mozda i nepotrebno, a mozda i smeta?! end 

         ptransSumRec_gama15.R_PorezAll = (-1.00M * ptransSumRec_alfa13.R_PorezAll).Ron2();
         ptransSumRec_gama15.R_Prirez   = (-1.00M * ptransSumRec_alfa13.R_Prirez).Ron2();
      }

      ptransSumRec_gama15.R_PorPrirez   = (ptransSumRec_gama15.R_PorezAll    + ptransSumRec_gama15.R_Prirez     ).Ron2();

      ptransSumRec_gama15.R_NettoAftKrp = (ptransSumRec_gama15.R_PorPrirez * -1.00M                             ).Ron2();
    //ptransSumRec_gama15.R_NettoAftKrp = (ptransSumRec_beta14.R_NettoAftKrp - ptransSumRec_alfa13.R_NettoAftKrp).Ron2();
      ptransSumRec_gama15.R_Netto       = (ptransSumRec_gama15.R_NettoAftKrp                                    ).Ron2();
      ptransSumRec_gama15.R_NaRuke      = (ptransSumRec_gama15.R_NettoAftKrp                                    ).Ron2();

      if(ptransSumRec_gama15.R_Odbitak != ptransSumRec_gama15.R_PorOsnAll)
      {
         ptransSumRec_gama15.R_Odbitak = ptransSumRec_gama15.R_PorOsnAll * -1.00M;
      }

      // nedobro 16.12.2012 ako je osobni odbitak veci od vec prikazane porezne osnovice onda u joppd obrazac smije doci osobni odbitak i porezna osnovica samo u tom iznosu
      // zapravo ako je osobni odbitak gama > porezne osnovice alfa i od porezne osnovice beta onda je on jednak poreznoj osnovici alfa
      // to jos dobro razmisli pa onda napravi !!!!

      return ptransSumRec_gama15;
   }

   #endregion KONACNI OBRACUN POREZA alfa13, beta14, gama15

   //internal static Ptrans GetPtransSum_beta14(Ptrans ptransSumRec_alfa13)
   //{
   //   Ptrans ptransSumRec_beta14 = new Ptrans();

   //   ptransSumRec_beta14.R_TheBruto = ptransSumRec_alfa13.R_TheBruto;
   //   // ... dopuniti 

   //   return ptransSumRec_beta14;
   //}

   //internal static Ptrans GetPtransSum_gama15(Ptrans ptransSumRec_alfa13, Ptrans ptransSumRec_beta14)
   //{
   //   Ptrans ptransSumRec_gama15 = new Ptrans();

   //   ptransSumRec_gama15.R_TheBruto = ptransSumRec_beta14.R_TheBruto - ptransSumRec_alfa13.R_TheBruto;
   //   // ... dopuniti 

   //   return ptransSumRec_gama15;
   //}

   #region GetAlreadySpentPtransInThisMonth

   internal static Ptrans.AlreadySpentPtransInThisMonthStruct GetAlreadySpentPtransInThisMonth(XSqlConnection conn, uint personCD, uint dokNum, DateTime dokDate)
   {
      Ptrans.AlreadySpentPtransInThisMonthStruct spent = new Ptrans.AlreadySpentPtransInThisMonthStruct();

      List<Ptrans> PtransList = new List<Ptrans>();

      List<VvSqlFilterMember> filterMembers_1 = Set_FilterMembers_1(personCD, dokDate, dokNum);

      LoadGenericVvDataRecordList<Ptrans>(conn, PtransList, filterMembers_1, "");

      if(PtransList.Count == 0) return spent;

      Placa placa_rec = new Placa();

      // jebiga dakle, sada moras za ovaj dolazni Ptrans                              
      // 1. naci sve prosle Ptrans-ove u mjesecu,                                     
      // 2. pa naci njihova zaglavlja (Placa)                                         
      // 3. pa jos doljepiti Placi Ptrane-ove ali samo za ovog Person-a               
      // 4. da bi mogao pravilno izracunati CalcTransResult za Ptrans-ove iz tocke 1. 
      // Jer CalcTransResults treba i Placa_rec i Placa_rec.Transes2 za obracun...    
      foreach(Ptrans ptrans_rec in PtransList)
      {
         ZXC.PlacaDao.SetMe_Record_byRecID(conn, placa_rec, ptrans_rec.T_parentID, false);

         // ovo bi ucitalo i Trans1, 2 i 3 i to i za ostale Persone, sto nas ovdje NE interesira! 
         //ZXC.PlacaDao.LoadGenericTransesList<Ptrane>(conn, pr.Transes2, pr.RecID, false); 

         List<VvSqlFilterMember> filterMembers_2 = Set_FilterMembers_2(placa_rec.RecID, personCD);

         LoadGenericVvDataRecordList<Ptrane>(conn, placa_rec.Transes2, filterMembers_2, ""); 

         ptrans_rec.CalcTransResults(placa_rec);
      }

      spent.Odbitak    = PtransList.Sum(ptrn => ptrn.R_Odbitak   );
      spent.PorOsn1    = PtransList.Sum(ptrn => ptrn.R_PorOsn1   );
      spent.PorOsn2    = PtransList.Sum(ptrn => ptrn.R_PorOsn2   );
      spent.PorOsn3    = PtransList.Sum(ptrn => ptrn.R_PorOsn3   );
      spent.MioOsn     = PtransList.Sum(ptrn => ptrn.R_MioOsn    );
    //spent.KrizPorOsn = PtransList.Sum(ptrn => ptrn.R_KrizPorOsn);
      spent.KrizPorOsn = PtransList.Sum(ptrn => (ptrn.R_Netto - ptrn.R_Premije).Ron2());
      spent.KrizPorUk  = PtransList.Sum(ptrn => ptrn.R_KrizPorUk );

      // 2024: 
      spent.TheBruto   = PtransList.Sum(ptrn => ptrn.R_TheBruto);
      spent.Mio1Olak   = PtransList.Sum(ptrn => ptrn.R_Mio1Olk );

      return spent;
   }

   private static List<VvSqlFilterMember> Set_FilterMembers_1(uint personCD, DateTime dokDate, uint dokNum)
   {
      DataRow drSchema;
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(5);

      // For TT 'Redovna Placa' only                                                                                                                                            
      // 6.2.2011: izbacen filter member 't_tt' 
      //drSchema = ZXC.PtransDao.TheSchemaTable.Rows[ZXC.PtransDao.CI.t_tt];
      //filterMembers.Add(new VvSqlFilterMember(drSchema, "elTipTransakcije", Placa.TT_REDOVANRAD, " = "));

      // For wanted personCD only                                                                                                                                            
      drSchema = ZXC.PtransDao.TheSchemaTable.Rows[ZXC.PtransDao.CI.t_personCD];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elPerson", personCD, " = "));

      // Skip this document                                                                                                                                                  
      drSchema = ZXC.PtransDao.TheSchemaTable.Rows[ZXC.PtransDao.CI.t_dokNum];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elDokNum", dokNum, /*" != "*/" < ")); // !!! inace ode rekurzija u StackOverFlow ako imas dva opracuna na isti dan 

      // Za uplate u mjesecu                                                                                                                                                 
      drSchema = ZXC.PtransDao.TheSchemaTable.Rows[ZXC.PtransDao.CI.t_dokDate];

      DateTime dateOd, dateDo;

      dateOd = new DateTime(dokDate.Year, dokDate.Month, 1);

      // tu imas dvije fore da znas za ubuduce kada trebas zadnji dan u mjesecu, pa ili ili 
      /* fora 1: */   dateDo = dateOd.AddMonths(1).AddDays(-1);
      /* fora 2: */ //dokDateDo = new DateTime(dokDate.Year, dokDate.Month, DateTime.DaysInMonth(year, month));

      filterMembers.Add(new VvSqlFilterMember(drSchema, "elDateOD", dateOd, " >= "));
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elDateDO", /*dokDateDo NE! */ dokDate, " <= "));

      return filterMembers;
   }

   private static List<VvSqlFilterMember> Set_FilterMembers_2(uint parentID, uint personCD)
   {
      DataRow drSchema;
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      // For parentRecID                                                                                                                                                  
      drSchema = ZXC.PtraneDao.TheSchemaTable.Rows[ZXC.PtraneDao.CI.t_parentID];

      filterMembers.Add(new VvSqlFilterMember(drSchema, "elDokNum", parentID, " = "));

      // For wanted personCD only                                                                                                                                            
      drSchema = ZXC.PtraneDao.TheSchemaTable.Rows[ZXC.PtraneDao.CI.t_personCD];

      filterMembers.Add(new VvSqlFilterMember(drSchema, "elPerson", personCD, " = "));

      return filterMembers;
   }

   #endregion GetAlreadySpentPtransInThisMonth

   #region Set_IMPORT_OFFIX_Columns

//     //____ Specifics 2 start ______________________________________________________
          
//"t_dokNum     , "*/ fprintf(device, "%s\t", ptrans_rec[0].t_br);	
//"t_serial     , "*/ fprintf(device, "%d\t", ptrans_rec[0].t_serial);
//"t_dokDate    , "*/ fprintf(device, "%s\t", GetMySqlDate(ptrans_rec[0].t_date));
//"t_mmyyyy     , "*/ fprintf(device, "%2.2s20%2.2s\t", ptrans_rec[0].t_mmyy, ptrans_rec[0].t_mmyy+2);
//"t_fondSati   , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_RSdaTbl);
//"t_rSm_ID     , "*/ fprintf(device, "%s\t", getRS_ID(ptrans_rec[0].t_br, ptrans_rec[0].t_mmyy, ""/*placa_rec[0].p_napomena*/, 0));
//"t_personCD   , "*/ fprintf(device, "%s\t", ptrans_rec[0].t_odohod_cd);
//"t_ime        , "*/ fprintf(device, "%s\t", ptrans_rec[0].t_ime);
//"t_prezime    , "*/ fprintf(device, "%s\t", ptrans_rec[0].t_prezime);
//"t_brutoOsn   , "*/ fprintf(device, "%.2lf\t", isZero(ptrans_rec[0].t_evr_brtOsn) ? ptrans_rec[0].t_bruto : ptrans_rec[0].t_evr_brtOsn);
//"t_topObrok   , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_evr_tobrok);
//"t_godStaza   , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_evr_gStaza);
//"t_dodBruto   , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_evr_brtAdd);
//"t_isMioII    , "*/ fprintf(device, "%d\t"   , ptrans_rec[0].t_MIO_II[0] == 'X' ? 1 : 0);
//"t_spc        , "*/ fprintf(device, "%d\t"   , ptrans_rec[0].t_spc[0] == 'U' ? 1 : ptrans_rec[0].t_spc[0] == 'N' ? 0 : 2);
//"t_koef       , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_koef);
//"t_zivotno    , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_zivotno);
//"t_dopZdr     , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_dopZdr);
//"t_dobMIO     , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_dobMio);
//"t_koefHRVI   , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_koef_HRVI);
//"t_invalidTip , "*/ if(notZero(ptrans_rec[0].t_koef_HRVI))
//                    {
//                       if(ptrans_rec[0].t_spc[0] == 'R') fprintf(device, "%d\t"   , 0); 
//                       else                              fprintf(device, "%d\t"   , 1); 
//                    }
//                    else
//                    {
//                       fprintf(device, "%d\t"   , 2);
//                    }
//"t_opcCD      , "*/ fprintf(device, "%s\t", ptrans_rec[0].t_opc_cd);
//"t_opcName    , "*/ fprintf(device, "%s\t", ptrans_rec[0].t_opcina);
//"t_opcRadCD   , "*/ fprintf(device, "%s\t", ptrans_rec[0].t_opc_rada_cd);
//"t_opcRadName , "*/ // SKIP 
//"t_stPrirez   , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_stPrirez);
//"t_netoAdd    , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_netoAdd);
//"t_isDirNeto  , "*/ fprintf(device, "%d\t"   , ptrans_rec[0].t_dirNetoFl[0] == 'D' ? 1 : 0);
//"t_prijevoz   , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_prijevoz);
//"t_isPoluSat  , "*/ // SET ... = 0 
//"t_rsB          "*/ // SET ... = 0 


//   fprintf(device, "\n");

//     //____ Specifics 2 end   ______________________________________________________

   public override string Set_IMPORT_OFFIX_Columns()
   {
      return

         "(" +

         "t_dokNum     , " + //"t_dokNum     , "*/ fprintf(device, "%s\t", ptrans_rec[0].t_br);	
         "t_serial     , " + //"t_serial     , "*/ fprintf(device, "%d\t", ptrans_rec[0].t_serial);
         "t_dokDate    , " + //"t_dokDate    , "*/ fprintf(device, "%s\t", GetMySqlDate(ptrans_rec[0].t_date));
         "t_mmyyyy     , " + //"t_mmyyyy     , "*/ fprintf(device, "%2.2s20%2.2s\t", ptrans_rec[0].t_mmyy, ptrans_rec[0].t_mmyy+2);
         "t_fondSati   , " + //"t_fondSati   , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_RSdaTbl);
         "t_rSm_ID     , " + //"t_rSm_ID     , "*/ fprintf(device, "%s\t", getRS_ID(ptrans_rec[0].t_br, ptrans_rec[0].t_mmyy, ""/*placa_rec[0].p_napomena*/, 0));
         "t_personCD   , " + //"t_personCD   , "*/ fprintf(device, "%s\t", ptrans_rec[0].t_odohod_cd);
         "t_ime        , " + //"t_ime        , "*/ fprintf(device, "%s\t", ptrans_rec[0].t_ime);
         "t_prezime    , " + //"t_prezime    , "*/ fprintf(device, "%s\t", ptrans_rec[0].t_prezime);
         "t_brutoOsn   , " + //"t_brutoOsn   , "*/ fprintf(device, "%.2lf\t", isZero(ptrans_rec[0].t_evr_brtOsn) ? ptrans_rec[0].t_bruto : ptrans_rec[0].t_evr_brtOsn);
         "t_topObrok   , " + //"t_topObrok   , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_evr_tobrok);
         "t_godStaza   , " + //"t_godStaza   , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_evr_gStaza);
         "t_dodBruto   , " + //"t_dodBruto   , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_evr_brtAdd);
         "t_isMioII    , " + //"t_isMioII    , "*/ fprintf(device, "%d\t"   , ptrans_rec[0].t_MIO_II[0] == 'X' ? 1 : 0);
         "t_spc        , " + //"t_spc        , "*/ fprintf(device, "%d\t"   , ptrans_rec[0].t_spc[0] == 'U' ? 1 : ptrans_rec[0].t_spc[0] == 'N' ? 0 : 2);
         "t_koef       , " + //"t_koef       , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_koef);
         "t_zivotno    , " + //"t_zivotno    , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_zivotno);
         "t_dopZdr     , " + //"t_dopZdr     , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_dopZdr);
         "t_dobMIO     , " + //"t_dobMIO     , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_dobMio);
         "t_koefHRVI   , " + //"t_koefHRVI   , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_koef_HRVI);
         "t_invalidTip , " + // vidi gore u komentaru 
         "t_opcCD      , " + //"t_opcCD      , "*/ fprintf(device, "%s\t", ptrans_rec[0].t_opc_cd);
         "t_opcName    , " + //"t_opcName    , "*/ fprintf(device, "%s\t", ptrans_rec[0].t_opcina);
         "t_opcRadCD   , " + //"t_opcRadCD   , "*/ fprintf(device, "%s\t", ptrans_rec[0].t_opc_rada_cd);
       //"t_opcRadName , " + //"t_opcRadName , "*/ // SKIP 
         "t_stPrirez   , " + //"t_stPrirez   , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_stPrirez);
         "t_netoAdd    , " + //"t_netoAdd    , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_netoAdd);
         "t_isDirNeto  , " + //"t_isDirNeto  , "*/ fprintf(device, "%d\t"   , ptrans_rec[0].t_dirNetoFl[0] == 'D' ? 1 : 0);
         "t_prijevoz     " + //"t_prijevoz   , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_prijevoz);
         
         ")"    + "\n" +

         "SET " + "\n" +

         "t_tt       = 'RR'      , " +
         "t_ttNum    = t_dokNum  , " +
         "t_isPoluSat= 0         , " +
         "t_rsB      = 0         , " +
         "t_parentID = t_dokNum + 1"; // zbog obracuna br 0. a RecId nemre biti 0!
   }

   internal static void ImportFromOffix_Translate437(XSqlConnection conn)
   {
      int debugCount;

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Ptrans>(conn, Translate437, null, "", ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out debugCount);
   }

   static bool Translate437(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      Ptrans ptrans_rec = vvDataRecord as Ptrans;

      ptrans_rec.T_ime       = ptrans_rec.T_ime    .VvTranslate437ToLatin2();
      ptrans_rec.T_prezime   = ptrans_rec.T_prezime.VvTranslate437ToLatin2();

      return ptrans_rec.EditedHasChanges();
   }

   #endregion Set_IMPORT_OFFIX_Columns

}
