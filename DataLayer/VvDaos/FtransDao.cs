using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using                   MySql.Data.MySqlClient;
using XSqlConnection  = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand     = MySql.Data.MySqlClient.MySqlCommand;
using XSqlDataReader  = MySql.Data.MySqlClient.MySqlDataReader;
using XSqlDbType      = MySql.Data.MySqlClient.MySqlDbType;
using XSqlParameter   = MySql.Data.MySqlClient.MySqlParameter;
using XSqlException   = MySql.Data.MySqlClient.MySqlException;
using XSqlDataAdapter = MySql.Data.MySqlClient.MySqlDataAdapter;
#endif

public sealed class FtransDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static FtransDao instance;

   private FtransDao(XSqlConnection conn, string dbName) : base(dbName, Ftrans.recordName, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static FtransDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new FtransDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableFtrans

   public static   uint TableVersionStatic { get { return 10; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition()
   {
      return (
         /* 00 */  "recID       int(10)     unsigned NOT NULL auto_increment,\n" +
         /* 01 */  "t_parentID  int(10)     unsigned NOT NULL,\n" +
         /* 02 */  "t_dokNum    int(10)     unsigned NOT NULL,\n" +
         /* 03 */  "t_serial    smallint(5) unsigned NOT NULL default '0',\n" +
         /* 04 */  "t_dokDate   date                 NOT NULL default '0001-01-01',\n" +
         /* 05 */  "t_konto     char(9)              NOT NULL default '',\n" +
         /* 06 */  "t_kupdob_cd int(6)      unsigned NOT NULL default '0',\n" +
         /* 07 */  "t_ticker    char(6)              NOT NULL default '',\n" +
         /* 08 */  "t_mtros_cd  int(6)      unsigned NOT NULL default '0',\n" +
         /* 09 */  "t_mtros_tk  char(6)              NOT NULL default '',\n" +
         /* 10 */  "t_tipBr     varchar(40)          NOT NULL default '',\n" +
         /* 11 */  "t_opis      varchar(80)          NOT NULL default '',\n" +
         /* 12 */  "t_valuta    date                 NOT NULL default '0001-01-01',\n" +
         /* 13 */  "t_tt        char(3)              NOT NULL default '',\n" +
         /* 14 */  "t_pdv       char(1)              NOT NULL default '',\n" +
         /* 15 */  "t_037       char(1)              NOT NULL default '',\n" +
         /* 16 */  "t_dug       decimal(12,2)        NOT NULL default '0.00',\n" +
         /* 17 */  "t_pot       decimal(12,2)        NOT NULL default '0.00',\n" +
         /* 18 */  "t_projektCD varchar(16)          NOT NULL default ''    ,\n" +
         /* 19 */  "t_pdvKnjiga tinyint(1)  unsigned NOT NULL default '0'   ,\n" +
         /* 20 */  "t_fakRecID  int(10)     unsigned NOT NULL default '0'   ,\n" +
         /* 21 */  "t_otsKind   tinyint(1)  unsigned NOT NULL default '0'   ,\n" +
         /* 22 */  "t_fond      char(1)              NOT NULL default ''    ,\n" +
         /* 23 */  "t_pozicija  varchar(32)          NOT NULL default ''    ,\n" +
         /* 24 */  "t_progAktiv varchar(32)          NOT NULL default ''    ,\n" +
         /* 25 */  "t_fakYear   int(4)      unsigned NOT NULL default '0'   ,\n" +

          "PRIMARY KEY                   (recID)                                                 ,\n" +
          /*"UNIQUE*/" KEY BY_LINKER     (t_parentID, t_serial)                                  ,\n" +
          /*"UNIQUE*/" KEY BY_DOKDATE    (t_dokDate,  t_dokNum,    t_serial)                     ,\n" +
          /*"UNIQUE*/" KEY BY_KONTO_DATE (t_konto,    t_dokDate,   t_dokNum,  t_serial)          ,\n" +
          /*"UNIQUE*/" KEY BY_KONTO_NUM  (t_konto,    t_dokNum,    t_serial)                     ,\n" +
          /*"UNIQUE*/" KEY BY_SCONTI     (t_konto,    t_kupdob_cd, t_dokDate, t_dokNum, t_serial),\n" +
          /*"UNIQUE*/" KEY BY_OTS        (t_konto,    t_kupdob_cd, t_tipBr,   t_dokNum, t_serial),\n" +
          /*"UNIQUE*/" KEY BY_FakRecID   (t_fakRecID, recID) \n"
         );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Ftrans.recordNameArhiva;
      else         tableName = Ftrans.recordName;

      string dbName = VvSQL.GetDbNameForThisTableName(tableName);

      switch(catchingVersion)
      {
         case 2: return ("ADD COLUMN t_projektCD varchar(16) NOT NULL default '' AFTER t_pot;\n");

         case 3: return ("MODIFY COLUMN t_tipBr VARCHAR(40) CHARACTER SET latin2 COLLATE latin2_croatian_ci NOT NULL, \n" +
                         "MODIFY COLUMN t_opis  VARCHAR(80) CHARACTER SET latin2 COLLATE latin2_croatian_ci NOT NULL;");

         case 4: return ("ADD INDEX BY_FakRecID (t_fakRecID, recID),\n" +
                         "ADD COLUMN t_pdvKnjiga tinyint(1)  unsigned NOT NULL default '0' AFTER t_projektCD    ,  " +
                         "ADD COLUMN t_fakRecID  int(10)     unsigned NOT NULL default '0' AFTER t_pdvKnjiga    ,\n" +
                         "ADD COLUMN t_otsKind   tinyint(1)  unsigned NOT NULL default '0' AFTER t_fakRecID     ;\n");

         case 5: return ("MODIFY COLUMN t_konto     char(8)           NOT NULL default ''                       ;\n");

         case 6: return ("ADD COLUMN t_fond      char(1)              NOT NULL default ''  AFTER t_otsKind      ;\n");

         case 7: return ("ADD COLUMN t_pozicija  varchar(32)          NOT NULL default ''  AFTER t_fond         ;\n");

         case 8: return ("ADD COLUMN t_progAktiv  varchar(32)          NOT NULL default ''  AFTER t_pozicija    ;\n");

         case 9: return ("MODIFY COLUMN t_konto     char(9)           NOT NULL default ''                       ;\n");

        case 10: return ("ADD COLUMN t_fakYear   int(4)      unsigned NOT NULL default '0'   AFTER t_progAktiv  ;\n");

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }


   #endregion CreateTableFtrans

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool dummy)
   {
      string preffix;
      Ftrans ftrans = (Ftrans)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_recID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {

      /* 01 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_parentID,  TheSchemaTable.Rows[CI.t_parentID]); 
      /* 02 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_dokNum,    TheSchemaTable.Rows[CI.t_dokNum]);
      /* 03 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_serial,    TheSchemaTable.Rows[CI.t_serial]);
      /* 04 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_dokDate,   TheSchemaTable.Rows[CI.t_dokDate]);
      /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_konto,     TheSchemaTable.Rows[CI.t_konto]);
      /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_kupdob_cd, TheSchemaTable.Rows[CI.t_kupdob_cd]);
      /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_ticker,    TheSchemaTable.Rows[CI.t_ticker]);
      /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_mtros_cd,  TheSchemaTable.Rows[CI.t_mtros_cd]);
      /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_mtros_tk,  TheSchemaTable.Rows[CI.t_mtros_tk]);
      /* 00 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_tipBr,     TheSchemaTable.Rows[CI.t_tipBr]);
      /* 11 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_opis,      TheSchemaTable.Rows[CI.t_opis]);
      /* 12 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_valuta,    TheSchemaTable.Rows[CI.t_valuta]);
      /* 13 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_TT,        TheSchemaTable.Rows[CI.t_tt]);
      /* 14 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_pdv,       TheSchemaTable.Rows[CI.t_pdv]);
      /* 15 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_037,       TheSchemaTable.Rows[CI.t_037]);
      /* 16 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_dug,       TheSchemaTable.Rows[CI.t_dug]);
      /* 17 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_pot,       TheSchemaTable.Rows[CI.t_pot]);
      /* 18 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_projektCD, TheSchemaTable.Rows[CI.t_projektCD]);
      /* 19 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_pdvKnjiga, TheSchemaTable.Rows[CI.t_pdvKnjiga]);
      /* 20 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_fakRecID , TheSchemaTable.Rows[CI.t_fakRecID ]);
      /* 21 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_otsKind  , TheSchemaTable.Rows[CI.t_otsKind  ]);
      /* 22 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_fond     , TheSchemaTable.Rows[CI.t_fond     ]);
      /* 23 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_pozicija , TheSchemaTable.Rows[CI.t_pozicija ]);
      /* 24 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_progAktiv, TheSchemaTable.Rows[CI.t_progAktiv]);
      /* 25 */ VvSQL.CreateCommandParameter(cmd, preffix, ftrans.T_fakYear  , TheSchemaTable.Rows[CI.t_fakYear]  );
      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva /* dummy */)
   {
      FtransStruct rdrData = new FtransStruct();

      rdrData._recID = reader.GetUInt32(CI.recID);

      //rdrData._addTS = reader.GetDateTime(1);
      //rdrData._modTS = reader.GetDateTime(2);
      //rdrData._addUID = reader.GetString(3);
      //rdrData._modUID = reader.GetString(4);


      /* 01 */      rdrData._t_parentID   = reader.GetUInt32  (CI.t_parentID);
      /* 02 */      rdrData._t_dokNum     = reader.GetUInt32  (CI.t_dokNum);
      /* 03 */      rdrData._t_serial     = reader.GetUInt16  (CI.t_serial);
      /* 04 */      rdrData._t_dokDate    = reader.GetDateTime(CI.t_dokDate);
      /* 05 */      rdrData._t_konto      = reader.GetString  (CI.t_konto);
      /* 06 */      rdrData._t_kupdob_cd  = reader.GetUInt32  (CI.t_kupdob_cd);
      /* 07 */      rdrData._t_ticker     = reader.GetString  (CI.t_ticker);
      /* 08 */      rdrData._t_mtros_cd   = reader.GetUInt32  (CI.t_mtros_cd);
      /* 09 */      rdrData._t_mtros_tk   = reader.GetString  (CI.t_mtros_tk);
      /* 10 */      rdrData._t_tipBr      = reader.GetString  (CI.t_tipBr);
      /* 11 */      rdrData._t_opis       = reader.GetString  (CI.t_opis);
      /* 12 */      rdrData._t_valuta     = reader.GetDateTime(CI.t_valuta);
      /* 13 */      rdrData._t_tt         = reader.GetString  (CI.t_tt);
      /* 14 */      rdrData._t_pdv        = reader.GetString  (CI.t_pdv);
      /* 15 */      rdrData._t_037        = reader.GetString  (CI.t_037);
      /* 16 */      rdrData._t_dug        = reader.GetDecimal (CI.t_dug);
      /* 17 */      rdrData._t_pot        = reader.GetDecimal (CI.t_pot);
      /* 18 */      rdrData._t_projektCD  = reader.GetString  (CI.t_projektCD);
      /* 19 */      rdrData._t_pdvKnjiga  = reader.GetUInt16  (CI.t_pdvKnjiga);
      /* 20 */      rdrData._t_fakRecID   = reader.GetUInt32  (CI.t_fakRecID);
      /* 21 */      rdrData._t_otsKind    = reader.GetUInt16  (CI.t_otsKind);
      /* 22 */      rdrData._t_fond       = reader.GetString  (CI.t_fond);
      /* 23 */      rdrData._t_pozicija   = reader.GetString  (CI.t_pozicija);
      /* 24 */      rdrData._t_progAktiv  = reader.GetString  (CI.t_progAktiv);
      /* 25 */      rdrData._t_fakYear    = reader.GetUInt32  (CI.t_fakYear);

      ((Ftrans)vvDataRecord).CurrentData = rdrData;

      return;
   }

   #endregion FillFromDataReader

   #region FtrCI struct & InitializeSchemaColumnIndexes()

   public struct FtransCI
   {
   internal int recID;

   /* 01 */   internal int  t_parentID;
   /* 02 */   internal int  t_dokNum;
   /* 03 */   internal int  t_serial;
   /* 04 */   internal int  t_dokDate;
   /* 05 */   internal int  t_konto;
   /* 06 */   internal int  t_kupdob_cd;
   /* 07 */   internal int  t_ticker;
   /* 08 */   internal int  t_mtros_cd;
   /* 09 */   internal int  t_mtros_tk;
   /* 10 */   internal int  t_tipBr;
   /* 11 */   internal int  t_opis;
   /* 12 */   internal int  t_valuta;
   /* 13 */   internal int  t_tt;
   /* 14 */   internal int  t_pdv;
   /* 15 */   internal int  t_037;
   /* 16 */   internal int  t_dug;
   /* 17 */   internal int  t_pot;
   /* 18 */   internal int  t_projektCD;
   /* 19 */   internal int  t_pdvKnjiga;
   /* 20 */   internal int  t_fakRecID ;
   /* 21 */   internal int  t_otsKind  ;
   /* 22 */   internal int  t_fond     ;
   /* 23 */   internal int  t_pozicija ;
   /* 24 */   internal int  t_progAktiv;
   /* 25 */   internal int  t_fakYear  ;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public FtransCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID       = GetSchemaColumnIndex("recID");
      //FtrCI.addTS       = GetSchemaColumnIndex("addTS");
      //FtrCI.modTS       = GetSchemaColumnIndex("modTS");
      //FtrCI.addUID      = GetSchemaColumnIndex("addUID");
      //FtrCI.modUID      = GetSchemaColumnIndex("modUID");

      CI.t_parentID  = GetSchemaColumnIndex("t_parentID");
      CI.t_dokNum    = GetSchemaColumnIndex("t_dokNum");
      CI.t_serial    = GetSchemaColumnIndex("t_serial");
      CI.t_dokDate   = GetSchemaColumnIndex("t_dokDate");
      CI.t_konto     = GetSchemaColumnIndex("t_konto");
      CI.t_kupdob_cd = GetSchemaColumnIndex("t_kupdob_cd");
      CI.t_ticker    = GetSchemaColumnIndex("t_ticker");
      CI.t_mtros_cd  = GetSchemaColumnIndex("t_mtros_cd");
      CI.t_mtros_tk  = GetSchemaColumnIndex("t_mtros_tk");
      CI.t_tipBr     = GetSchemaColumnIndex("t_tipBr");
      CI.t_opis      = GetSchemaColumnIndex("t_opis");
      CI.t_valuta    = GetSchemaColumnIndex("t_valuta");
      CI.t_tt        = GetSchemaColumnIndex("t_tt");
      CI.t_pdv       = GetSchemaColumnIndex("t_pdv");
      CI.t_037       = GetSchemaColumnIndex("t_037");
      CI.t_dug       = GetSchemaColumnIndex("t_dug");
      CI.t_pot       = GetSchemaColumnIndex("t_pot");
      CI.t_projektCD = GetSchemaColumnIndex("t_projektCD");
      CI.t_pdvKnjiga = GetSchemaColumnIndex("t_pdvKnjiga");
      CI.t_fakRecID  = GetSchemaColumnIndex("t_fakRecID");
      CI.t_otsKind   = GetSchemaColumnIndex("t_otsKind");
      CI.t_fond      = GetSchemaColumnIndex("t_fond");
      CI.t_pozicija  = GetSchemaColumnIndex("t_pozicija");
      CI.t_progAktiv = GetSchemaColumnIndex("t_progAktiv");
      CI.t_fakYear   = GetSchemaColumnIndex("t_fakYear");
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()

   #region Set_IMPORT_OFFIX_Columns

   //  //____ Specifics 2 start ______________________________________________________

   //fprintf(device, "%s\t", ftrans_rec[0].t_nalog);
   //fprintf(device, "%s\t", ftrans_rec[0].t_serial);
   //fprintf(device, "%s\t", ftrans_rec[0].t_konto);
   //fprintf(device, "%s\t", ftrans_rec[0].t_kupdob_cd);
   //fprintf(device, "%s\t", ftrans_rec[0].t_mtros_cd);
   //fprintf(device, "%s\t", ftrans_rec[0].t_TipBr);
   //fprintf(device, "%s\t", ftrans_rec[0].t_opis);
   //fprintf(device, "%s\t", GetMySqlDate(ftrans_rec[0].t_valuta));
   //fprintf(device, "%s\t", ftrans_rec[0].t_tip);
   //fprintf(device, "%s\t", GetMySqlDate(ftrans_rec[0].t_date));
   //fprintf(device, "%s\t", ftrans_rec[0].t_pdv);
   //fprintf(device, "%s\t", ftrans_rec[0].t_037);
   //fprintf(device, "%s\t", ftrans_rec[0].t_dug);
   //fprintf(device, "%s\t", ftrans_rec[0].t_pot);
	
   //fprintf(device, "\n");

   //  //____ Specifics 2 end   ______________________________________________________

   public override string Set_IMPORT_OFFIX_Columns()
   {
      return

         "(" +

         "t_dokNum   , " + //fprintf(device, "%s\t", ftrans_rec[0].t_nalog);
         "t_serial   , " + //fprintf(device, "%s\t", ftrans_rec[0].t_serial);
         "t_konto    , " + //fprintf(device, "%s\t", ftrans_rec[0].t_konto);
         "t_kupdob_cd, " + //fprintf(device, "%s\t", ftrans_rec[0].t_kupdob_cd);
         "t_ticker   , " + //fprintf(device, "%s\t", ftrans_rec[0].t_kupdob_cd);
         "t_mtros_cd , " + //fprintf(device, "%s\t", ftrans_rec[0].t_mtros_cd);
         "t_mtros_tk , " + //fprintf(device, "%s\t", ftrans_rec[0].t_mtros_cd);
         "t_tipBr    , " + //fprintf(device, "%s\t", ftrans_rec[0].t_TipBr);
         "t_opis     , " + //fprintf(device, "%s\t", ftrans_rec[0].t_opis);
         "t_valuta   , " + //fprintf(device, "%s\t", ftrans_rec[0].t_valuta[0] ? GetMySqlDate(ftrans_rec[0].t_valuta) : "0001-01-01");
         "t_tt       , " + //fprintf(device, "%s\t", ftrans_rec[0].t_tip);
         "t_dokDate  , " + //fprintf(device, "%s\t", GetMySqlDate(ftrans_rec[0].t_date));
         "t_pdv      , " + //fprintf(device, "%s\t", ftrans_rec[0].t_pdv);
         "t_037      , " + //fprintf(device, "%s\t", ftrans_rec[0].t_037);
         "t_dug      , " + //fprintf(device, "%s\t", ftrans_rec[0].t_dug);
         "t_pot        " + /*fprintf(device, "%s\t", ftrans_rec[0].t_pot);*/        // zadnji nema zarez! 

         ")"    + "\n" +

         "SET " + "\n" +

         "t_parentID = t_dokNum ";
   }

   static bool SetFtransTicker(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      Ftrans ftrans_rec = vvDataRecord as Ftrans;

      ftrans_rec.T_opis = ftrans_rec.T_opis.VvTranslate437ToLatin2();

      if(ftrans_rec.T_kupdob_cd.NotZero())
      {

         Kupdob kupdob_rec = new Kupdob(ftrans_rec.T_kupdob_cd);

         bool OK = kupdob_rec.VvDao.SetMe_Record_bySomeUniqueColumn(conn, kupdob_rec, ftrans_rec.T_kupdob_cd, ZXC.KupdobSchemaRows[ZXC.KpdbCI.kupdobCD], false, /*false*/true);

         if(OK)
         {
            ftrans_rec.T_ticker = kupdob_rec.Ticker;
         }
      }

      return ftrans_rec.EditedHasChanges();
   }

   internal static void ImportFromOffix_Translate437_SetTickers(XSqlConnection conn)
   {
      //DataRow drSchema;
      //List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
      //// Skip 't_kupdob_cd.IsZero'                                                                                                                                            
      //drSchema = ZXC.FtransDao.TheSchemaTable.Rows[ZXC.FtransDao.CI.t_kupdob_cd];
      //filterMembers.Add(new VvSqlFilterMember(drSchema, "theKupdobCd", 0, " != "));

      int debugCount;

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Ftrans>(conn, SetFtransTicker, null, "recID", ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out debugCount);
   
   }

   #endregion Set_IMPORT_OFFIX_Columns

   #region PROIZVODNJA

   /*private*/ internal enum OtpLista 
   {
      nrspIndir,
      raspIndir,
      raspDirkt,
      nrspRezsk,
      amortizac,
      proUtijek,
      rnpFaktur,
   }

   #region OTP - Obracun Troskova Proizvodnje (onaj prvi koji rasporedjuje indirektne i stvara nalog)

   internal static bool GetObracunTroskovaProizvodnjeListe(
      XSqlConnection   dbConn          ,
      DateTime         dateOD          ,
      DateTime         dateDO          ,
      KtoShemaDsc      op              ,
      out List<Ftrans> nrspIndirFtrList,
      out List<Ftrans> raspDirktFtrList,
      out List<Ftrans> proUtijekFtrList,
      out List<Faktur> rnpFakturList   )
   {
      #region variablez

      nrspIndirFtrList = new List<Ftrans>();
      raspDirktFtrList = new List<Ftrans>();
      proUtijekFtrList = new List<Ftrans>();

      rnpFakturList          = new List<Faktur>();

      //List<VvSqlFilterMember> filterMembers;

      string orderBY = "t_dokDate, t_dokNum, t_serial";

      DateTime dateFD = ZXC.projectYearFirstDay;
      string   tt     = op.Dsc_otp_obrProTT    ;

      #endregion variablez

LoadGenericVvDataRecordList<Ftrans>(dbConn, nrspIndirFtrList, GetFM_ftrOTP(dateOD, dateDO, op.Dsc_otp_niAnaGR, op.Dsc_otp_niKtoRoot, tt, OtpLista.nrspIndir, false), "ftr", orderBY);
LoadGenericVvDataRecordList<Ftrans>(dbConn, raspDirktFtrList, GetFM_ftrOTP(dateOD, dateDO, op.Dsc_otp_rdAnaGR, op.Dsc_otp_rdKtoRoot, tt, OtpLista.raspDirkt, false), "ftr", orderBY);
LoadGenericVvDataRecordList<Ftrans>(dbConn, proUtijekFtrList, GetFM_ftrOTP(dateFD, dateDO, ""                , op.Dsc_otp_ktoObrade, tt, OtpLista.proUtijek, false), "ftr", orderBY);

LoadGenericVvDataRecordList<Faktur>(dbConn, rnpFakturList   , GetFM_fakOTP(raspDirktFtrList, proUtijekFtrList, null, tt, false), "L", "dokDate, dokNum", true, "DISTINCT L.*, R.*", "");

      return (/*nrspIndirFtrList.Count.NotZero() &&*/ rnpFakturList.Count.NotZero());
   }

   #endregion OTP - Obracun Troskova Proizvodnje (onaj prvi koji rasporedjuje indirektne i stvara nalog)

   #region FilterMemberz - zajednicki za OTP i IATP

   /*private*/internal static List<VvSqlFilterMember> GetFM_ftrOTP( // Old, classic 
      DateTime         dateOD      , 
      DateTime         dateDO      , 
      string           anaGR       , 
      string           kontoRoot   , 
      string           obrProTT    ,
      OtpLista         whichList   ,
    //string           projektCD   ,
      bool             isIATP      ) // da li je INTEGRALNA ANALIZA TROSKOVA PORIZVODNJE, ili onaj prvobitni OTP 
   {
      return GetFM_ftrOTP(
         /*DateTime   */      dateOD        , 
         /*DateTime   */      dateDO        , 
         /*string     */      anaGR         , 
         /*string     */      kontoRoot     , 
         /*string     */      obrProTT      ,
         /*OtpLista   */      whichList     ,
         /*string     */    /*projektCD*/ "", // !!! 
         /*bool       */      isIATP        );
   }

   /*private*/internal static List<VvSqlFilterMember> GetFM_ftrOTP(
      DateTime         dateOD      , 
      DateTime         dateDO      , 
      string           anaGR       , 
      string           kontoRoot   , 
      string           obrProTT    ,
      OtpLista         whichList   ,
      string           projektCD   ,
      bool             isIATP      ) // da li je INTEGRALNA ANALIZA TROSKOVA PORIZVODNJE, ili onaj prvobitni OTP 
   {
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(5);

      #region FilterMembers

      DataRowCollection  FtrSch = ZXC.FtransSchemaRows;
      FtransDao.FtransCI FtrCI  = ZXC.FtransDao.CI;       
      DataRowCollection  KplSch = ZXC.KplanSchemaRows;
      KplanDao.KplanCI   KplCI  = ZXC.KplanDao.CI;       

      if(dateOD   .NotEmpty()) filterMembers.Add(new VvSqlFilterMember(FtrSch[FtrCI.t_dokDate  ], false, "elDateOD"   , dateOD             , "", "", " >= ", ""));
      if(dateDO   .NotEmpty()) filterMembers.Add(new VvSqlFilterMember(FtrSch[FtrCI.t_dokDate  ], false, "elDateDO"   , dateDO             , "", "", " <= ", ""));

      if(kontoRoot.NotEmpty())
      {
         if(whichList == OtpLista.proUtijek)
            filterMembers.Add(new VvSqlFilterMember(FtrSch[FtrCI.t_konto], false, "elKontoRoot", kontoRoot      , "", "", " = "   , ""));
         else if(whichList == OtpLista.nrspRezsk) // izbaci amortizaciju iz rrezijskih 
            filterMembers.Add(new VvSqlFilterMember(FtrSch[FtrCI.t_konto], false, "elKontoRoot", kontoRoot + "%", "", "", " NOT LIKE ", ""));
         else                                                                                                                      
            filterMembers.Add(new VvSqlFilterMember(FtrSch[FtrCI.t_konto], false, "elKontoRoot", kontoRoot + "%", "", "", " LIKE ", ""));

      }

      // 07.03.2017: dodan fm ProjektCD za potrebe 'Get_RI_razd_ForProjektCD()' 
      if(projektCD.NotEmpty())
      {
         filterMembers.Add(new VvSqlFilterMember(FtrSch[FtrCI.t_projektCD], false, "elProjektCD", projektCD, "", "", " = ", ""));
      }

      // obrProTT               
      if(true) // dakle, olvejz 
      {
         string likeOrNotLike = 
            ((whichList == OtpLista.nrspIndir || 
              whichList == OtpLista.nrspRezsk || 
              whichList == OtpLista.amortizac) ? " NOT LIKE " : " LIKE "); 
         // za nrspIndir da u t_ptjktCD bude bilo sta osim 'RNP' (ili 'RNS', ...)
             //nrspRezsk
             //amortizac


         filterMembers.Add(new VvSqlFilterMember(FtrSch[FtrCI.t_projektCD], false, "elobrProTT", obrProTT + "%", "", "", likeOrNotLike, ""));
      }

      if(anaGR.NotEmpty())
      {
         string text = ZXC.GetOrovaniCharPattern(anaGR);
         filterMembers.Add(new VvSqlFilterMember(KplSch[KplCI.anaGr], false, "elAnaGR", text, text, "", " REGEXP ", Kplan.recordName));
      }

      if(whichList == OtpLista.nrspIndir)
      {
         string unbalancedSaldoOnlySubQuerry = "\n(\n   SELECT SUM(sub.t_dug - sub.t_pot) AS saldo\n   FROM ftrans sub FORCE INDEX (BY_KONTO_DATE)\n" + 
                                                    "   WHERE sub.t_konto = ftr.t_konto AND sub.t_dokDate >= ?filter_elDateOD AND sub.t_dokDate <= ?filter_elDateDO\n)\n";
         filterMembers.Add(new VvSqlFilterMember(unbalancedSaldoOnlySubQuerry, 0, " != ")); // namjerno kao zadnji 
      }

      //if(whichList == OTP_Lista.proUtijek && isIATP)
      //{
      //   string unbalancedSaldoOnlySubQuerry = "\n(\n   SELECT SUM(sub.t_dug - sub.t_pot) AS saldo\n   FROM ftrans sub FORCE INDEX (BY_KONTO_DATE)\n" +
      //                                              "   WHERE sub.t_konto = ftr.t_konto AND sub.t_dokDate >= ?filter_elDateOD AND sub.t_dokDate <= ?filter_elDateDO\n)\n";
      //   filterMembers.Add(new VvSqlFilterMember(unbalancedSaldoOnlySubQuerry, 0, " != ")); // namjerno kao zadnji 
      //}

      #endregion FilterMembers

      return filterMembers;
   }

   private static List<VvSqlFilterMember> GetFM_fakOTP(List<Ftrans> raspDirktFtrList, List<Ftrans> proUtijekFtrList, List<Ftrans> raspIndirFtrList, string obrProTT, bool isIATP) // isIATP - da li je INTEGRALNA ANALIZA TROSKOVA PORIZVODNJE, ili onaj prvobitni OTP 

 //private static List<VvSqlFilterMember> GetFM_fakOTP(
 //   DateTime         dateOD      , 
 //   DateTime         dateDO      , 
 //   string           anaGR       , 
 //   string           kontoRoot   , 
 //   string           obrProTT    )
   {
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(5);

      #region FilterMembers PUSE (ovdje bi ti zbog razlicitih kronoloskih granica trebala kompleksna boolova recenice koju filterMembersi jos nemreju)

      //DataRowCollection  FtrSch = ZXC.FtransDao.TheSchemaTable.Rows;
      //FtransDao.FtransCI FtrCI  = ZXC.FtransDao.CI;       
      //DataRowCollection  KplSch = ZXC.KplanDao.TheSchemaTable.Rows;
      //KplanDao.KplanCI   KplCI  = ZXC.KplanDao.CI;       

      //if(dateOD.NotEmpty())    filterMembers.Add(new VvSqlFilterMember(FtrSch[FtrCI.t_dokDate  ], false, "elDateOD"  , dateOD        , "", "", " >= "  , Ftrans.recordName));
      //if(dateDO.NotEmpty())    filterMembers.Add(new VvSqlFilterMember(FtrSch[FtrCI.t_dokDate  ], false, "elDateDO"  , dateDO        , "", "", " <= "  , Ftrans.recordName));
      //if(true) /* olvejz */    filterMembers.Add(new VvSqlFilterMember(FtrSch[FtrCI.t_projektCD], false, "elobrProTT", obrProTT + "%", "", "", " LIKE ", Ftrans.recordName));

      //if(kontoRoot.NotEmpty()) filterMembers.Add(new VvSqlFilterMember(FtrSch[FtrCI.t_konto], ZXC.FM_OR_Enum.OPEN_OR, false, "elKontoRoot", kontoRoot + "%", "", "", " LIKE ", Ftrans.recordName));
      //if(anaGR.NotEmpty())
      //{
      //   string text = ZXC.GetOrovaniCharPattern(anaGR);
      //   filterMembers.Add(new VvSqlFilterMember(KplSch[KplCI.anaGr], ZXC.FM_OR_Enum.CLOSE_OR, false, "elAnaGR", text, text, "", " REGEXP ", Kplan.recordName));
      //}

      #endregion FilterMembers

      DataRowCollection  FakSch = ZXC.FakturDao.TheSchemaTable.Rows;
      FakturDao.FakturCI FakCI  = ZXC.FakturDao.CI;       

      filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.tt], "elObrProTT", obrProTT, " = "));

      string[] rnpTtNums;

      if(isIATP)
      {
         rnpTtNums =
         raspDirktFtrList.Select(ftr => ftr.T_projektCD.Substring(4))
         .Union(
         proUtijekFtrList/*.Where(qwe)*/.Select(ftr => ftr.T_projektCD.Substring(4)))
         .Union(
         raspIndirFtrList/*.Where(qwe)*/.Select(ftr => ftr.T_projektCD.Substring(4)))
         .Distinct().ToArray();
      }
      else
      {
         rnpTtNums =
         raspDirktFtrList.Select(ftr => ftr.T_projektCD.Substring(4))
         .Union(
         proUtijekFtrList/*.Where(qwe)*/.Select(ftr => ftr.T_projektCD.Substring(4)))
         .Distinct().ToArray();
      }

      string inSetClause = VvSQL.GetInSetClause_AsInt(rnpTtNums);

      filterMembers.Add(new VvSqlFilterMember("ttNum", inSetClause, " IN ")); // NOTA BENE! kada koristis inSetClause uvijek koristi ovasj constructor filterMembers-a jerbo inace 'input string was in incorrect format'

      return filterMembers;
   }

   #endregion FilterMemberz - zajednicki za OTP i IATP

   #region IATP - Integralna Analiza Troskova Proizvodnje (onaj drugi koji rasporedjuje rezijske i amortizaciju i NE stvara nalog)

   internal static bool GetAnalizaTroskovaProizvodnjeListe(
      XSqlConnection   conn            ,
      DateTime         dateOD          ,
      DateTime         dateDO          ,
      KtoShemaDsc      op              ,
      out List<Ftrans> raspIndirFtrList,
      out List<Ftrans> raspDirktFtrList,
      out List<Ftrans> nrspRezskFtrList,
      out List<Ftrans> nrspAmortFtrList,
      out List<Ftrans> proUtijekFtrList,
      out List<Faktur> rnpFakturList)
   {
      #region variablez

      raspIndirFtrList = new List<Ftrans>();
      raspDirktFtrList = new List<Ftrans>();
      nrspRezskFtrList = new List<Ftrans>();
      nrspAmortFtrList = new List<Ftrans>();
      proUtijekFtrList = new List<Ftrans>();

      rnpFakturList    = new List<Faktur>();

      //List<VvSqlFilterMember> filterMembers;

      string orderBY = "t_dokDate, t_dokNum, t_serial";

      DateTime dateFD  = ZXC.projectYearFirstDay;
      TimeSpan oneDay  = new TimeSpan(1, 0, 0, 0);
      DateTime proizDO = (dateOD == dateFD ? dateOD : dateOD - oneDay); // ako je razd od 1.1. onda na 1.1, a ako je razd npr. od 1.4. onda do 31.3 
      string   tt      = op.Dsc_otp_obrProTT;

      #endregion variablez

   LoadGenericVvDataRecordList<Ftrans>(conn, raspIndirFtrList, GetFM_ftrOTP(dateOD, dateDO , op.Dsc_otp_niAnaGR, op.Dsc_otp_niKtoRoot  , tt, OtpLista.raspIndir, true), "ftr", orderBY);
   LoadGenericVvDataRecordList<Ftrans>(conn, raspDirktFtrList, GetFM_ftrOTP(dateOD, dateDO , op.Dsc_otp_rdAnaGR, op.Dsc_otp_rdKtoRoot  , tt, OtpLista.raspDirkt, true), "ftr", orderBY);
   LoadGenericVvDataRecordList<Ftrans>(conn, nrspRezskFtrList, GetFM_ftrOTP(dateOD, dateDO , op.Dsc_atp_rrAnaGR, op.Dsc_atp_ktoAmKorjen, tt, OtpLista.nrspRezsk, true), "ftr", orderBY);
   if(op.Dsc_atp_hocuAmort)
   LoadGenericVvDataRecordList<Ftrans>(conn, nrspAmortFtrList, GetFM_ftrOTP(dateOD, dateDO , op.Dsc_atp_rrAnaGR, op.Dsc_atp_ktoAmKorjen, tt, OtpLista.amortizac, true), "ftr", orderBY);
 //LoadGenericVvDataRecordList<Ftrans>(conn, proUtijekFtrList, GetFM_ftrOTP(dateFD, proizDO, "",                 op.Dsc_otp_ktoObrade  , tt, OtpLista.proUtijek, true), "ftr", orderBY);
   LoadGenericVvDataRecordList<Ftrans>(conn, proUtijekFtrList, GetFM_ftrOTP(dateFD, dateDO , "",                 op.Dsc_otp_ktoObrade  , tt, OtpLista.proUtijek, true), "ftr", orderBY);

   LoadGenericVvDataRecordList<Faktur>(conn, rnpFakturList   , GetFM_fakOTP(raspDirktFtrList, proUtijekFtrList, raspIndirFtrList, tt, true), "L", "dokDate, dokNum", true, "DISTINCT L.*, R.*", "");

      return (/*nrspIndirFtrList.Count.NotZero() &&*/ rnpFakturList.Count.NotZero());
   }

   internal static int[] GetPrevYearsList(uint rnpTtNum, bool excludeCurrYear)
   {
      int rnYear = ZXC.ValOrZero_Int(rnpTtNum.ToString().SubstringSafe(0, 2)) + 2000;

      List<int> prevYears = new List<int>();

      for(int currPrevYear = (excludeCurrYear ? ZXC.projectYearFirstDay.Year - 1 : ZXC.projectYearFirstDay.Year); currPrevYear >= rnYear; --currPrevYear)
      {
         prevYears.Add(currPrevYear);
      }

      prevYears.Reverse();

      return prevYears.ToArray();
   }

   // 07.03.2017: 
   internal static decimal Get_RI_razd_ForProjektCD(XSqlConnection conn, string projektCD, DateTime dateOD, DateTime dateDO, KtoShemaDsc op)
   {
      List<Ftrans> raspIndirFtrList_froProjektCD = new List<Ftrans>();

      LoadGenericVvDataRecordList<Ftrans>(conn, raspIndirFtrList_froProjektCD, 
         GetFM_ftrOTP(dateOD, dateDO, op.Dsc_otp_niAnaGR, op.Dsc_otp_niKtoRoot, /*tt*/op.Dsc_otp_obrProTT, OtpLista.raspIndir, projektCD, true), 
         "ftr", "t_dokDate, t_dokNum, t_serial");

      decimal RI_razd = raspIndirFtrList_froProjektCD.Sum(ftr => ftr.R_DugMinusPot);

      return RI_razd;
   }

   #endregion IATP - Integralna Analiza Troskova Proizvodnje (onaj drugi koji rasporedjuje rezijske i amortizaciju i NE stvara nalog)

   #endregion PROIZVODNJA

   #region RISK comparation

   public static decimal GetMoneyBy_Konto_Dates_MtrosCD(XSqlConnection conn, string konto, DateTime dateOD, DateTime dateDO, uint mtrosCD, bool isDug, bool isMalopSkl)
   {
      return GetMoneyBy_Konto_Dates_MtrosCD(conn, konto, dateOD, dateDO, mtrosCD, isDug, isMalopSkl, false);
   }
   public static decimal GetMoneyBy_Konto_Dates_MtrosCD(XSqlConnection conn, string konto, DateTime dateOD, DateTime dateDO, uint mtrosCD, bool isDug, bool isMalopSkl, bool isIRA)
   {
      List<Ftrans>  theFtransList = new List<Ftrans>();
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(5);

      filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_dokDate ], "dateOD" , dateOD , " >= "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_dokDate ], "dateDO" , dateDO , " <= "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_konto   ], "konto"  , konto  , "  = "));

      if(isIRA == false) // mtrosCD NE trebamo za IRA 
      filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_mtros_cd], "mtrosCD", mtrosCD, "  = "));

      string TT_IRMA = isIRA ? "IR" : "IM";

      if(isIRA)
      {
         filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_tt], "theTT", "IR", " = "));
      }

      else if(konto == ZXC.KSD.Dsc_Kto_Realizacija) // jerbo se na konto nab vrij prodane robe knjizi i iz velep i iz malop a tu ocemo samo malop 
      {
         filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_tt], "theTT", "IM", " = "));
      }

      VvDaoBase.LoadGenericVvDataRecordList(conn, theFtransList, filterMembers, "");

      return theFtransList.Sum(ftr => isDug ? ftr.T_dug : ftr.T_pot);
   }

   #endregion RISK comparation

   public static List<Ftrans> Get_Naplaceno_OR_TodoMAP_FtransList_For_FakRecID(XSqlConnection conn, uint fakRecID, bool isTODO)
   {
      bool success = true;
      Ftrans todoMAP_ftrans_rec = new Ftrans();
      List<Ftrans> todoMAP_FtransList = new List<Ftrans>();

      ZXC.sqlErrNo = 0;

      if(fakRecID.IsZero()) return todoMAP_FtransList;

      using(XSqlCommand cmd = (VvSQL.Get_Naplaceno_OR_TodoMAP_FtransList_For_FakRecID_Command(conn, fakRecID, isTODO)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;

               while(success && reader.Read())
               {
                  todoMAP_ftrans_rec = new Ftrans();

                  ZXC.FtransDao.FillFromDataReader(todoMAP_ftrans_rec, reader, false);

                  todoMAP_FtransList.Add(todoMAP_ftrans_rec);
               }
               reader.Close();
            }
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("Get_TodoMAP_FtransList_For_FakRecID", ex, System.Windows.Forms.MessageBoxButtons.OK);

            ZXC.sqlErrNo = ex.Number;
         }
      } // using 

      return todoMAP_FtransList;
   }

   public static List<Ftrans> Get_MAP_FtransList(XSqlConnection conn)
   {
      bool success = true;
      Ftrans todoMAP_ftrans_rec = new Ftrans();
      List<Ftrans> todoMAP_FtransList = new List<Ftrans>();

      ZXC.sqlErrNo = 0;

      using(XSqlCommand cmd = (VvSQL.Get_MAP_FtransList_Command(conn)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;

               while(success && reader.Read())
               {
                  todoMAP_ftrans_rec = new Ftrans();

                  ZXC.FtransDao.FillFromDataReader(todoMAP_ftrans_rec, reader, false);

                  todoMAP_FtransList.Add(todoMAP_ftrans_rec);
               }
               reader.Close();
            }
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("Get_TodoMAP_FtransList_For_FakRecID", ex, System.Windows.Forms.MessageBoxButtons.OK);

            ZXC.sqlErrNo = ex.Number;
         }
      } // using 

      return todoMAP_FtransList;
   }

   public static Xtrano Get_MAP_Xtrano(XSqlConnection conn, uint ftrans_recID)
   {
      Xtrano map_xtrano_rec = new Xtrano();

      bool OK = map_xtrano_rec.VvDao.SetMe_Record_bySomeUniqueColumn(conn, map_xtrano_rec, ftrans_recID, ZXC.XtranoSchemaRows[ZXC.XtoCI.t_parentID], false, true);

      return OK ? map_xtrano_rec : null;
   }

   public static bool IsMAPdone(XSqlConnection conn, Ftrans ftrans_rec)
   {
      return true; ; // privremeno dok se ne implementira MAP u potpunosti
      return Get_MAP_Xtrano(conn, ftrans_rec.T_recID) != null;
   }

}
