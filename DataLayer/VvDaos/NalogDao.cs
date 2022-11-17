using System;
using System.Collections.Generic;
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
using XSqlException  = MySql.Data.MySqlClient.MySqlException;
using XSqlDataReader = MySql.Data.MySqlClient.MySqlDataReader;
#endif

public sealed class NalogDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static NalogDao instance;

   private NalogDao(XSqlConnection conn, string dbName) : base(dbName, Nalog.recordNameArhiva, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static NalogDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new NalogDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableNalog

   public static   uint TableVersionStatic { get { return 5; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition(bool isArhiva)
   {
      return (
         /* 00 */  "recID        int(10)     unsigned NOT NULL auto_increment,\n" +
         /* 01 */  "addTS        timestamp                     default '0000-00-00 00:00:00',\n" +
         /* 02 */  "modTS        timestamp                     default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,\n" +
         /* 03 */  "addUID       varchar(16)          NOT NULL default 'XY',\n" +
         /* 04 */  "modUID       varchar(16)          NOT NULL default '',\n" +
        CreateTable_LanSrvID_And_LanRecID_Columns +
         /* 05 */  "dokNum       int(10)    unsigned  NOT NULL,\n" +
         /* 06 */  "dokDate      date                 NOT NULL default '0001-01-01',\n" +
         /* 07 */  "tt           char(3)              NOT NULL default '',\n" +
         /* 08 */  "ttNum        int(10)    unsigned  NOT NULL,\n" +
         /* 09 */  "napomena     varchar(128)         NOT NULL default '',\n" +
         /* 10 */  "flagA        tinyint(1) unsigned  NOT NULL default 0,\n" +
         /* 11 */  "dug          DECIMAL(12,2)        NOT NULL DEFAULT 0.00,\n" +
         /* 12 */  "pot          DECIMAL(12,2)        NOT NULL DEFAULT 0.00,\n" +
         /*13+2*/  "devName      char(3)              NOT NULL default ''  ,\n" +

         CreateTable_ArhivaExtensionDefinition(isArhiva) + 

                                "PRIMARY KEY            (recID),\n" +
          (isArhiva ? "" : "UNIQUE ") + "KEY BY_DOKNUM  (dokNum),\n" +
          (isArhiva ? "" : "UNIQUE ") + "KEY BY_DOKDATE (dokDate, dokNum),\n" +
                                 "       KEY BY_TTNUM   (tt, ttNum)\n" // da si ne zadas kasnije probleme kod importa ili vec postojecih datoteka, 
                                                //jer inace ima smisla da bude 'UNIQUE'. Razmisli mozda kasnije.
                                                // NE, NE!!! Npr Izvodi brojevi se dupliciraju kada u jednom danu imas i devizni izvod. 
                                                // OSTAVI NON UNIQUE!      
          +

          (isArhiva ? ", KEY BYqOrigRecID (origRecID)\n" : "")
      );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Nalog.recordNameArhiva;
      else         tableName = Nalog.recordName;

      string dbName = VvSQL.GetDbNameForThisTableName(tableName);

      switch(catchingVersion)
      {
         case 2: return ("MODIFY COLUMN napomena     varchar(128)         NOT NULL default '';\n");
         case 3: return ("ADD    COLUMN devName      char(3)              NOT NULL default '' AFTER pot    ;\n");

         case 4: return (isArhiva ? "ADD INDEX BYqOrigRecID (origRecID)\n" : "skip_me");

         case 5: return AlterTable_LanSrvID_And_LanRecID_Columns;

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableNalog

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      Nalog nalog = (Nalog)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, nalog.RecID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, nalog.AddTS,    TheSchemaTable.Rows[CI.addTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, nalog.ModTS,    TheSchemaTable.Rows[CI.modTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, nalog.AddUID,   TheSchemaTable.Rows[CI.addUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, nalog.ModUID,   TheSchemaTable.Rows[CI.modUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, nalog.LanSrvID, TheSchemaTable.Rows[CI.lanSrvID]);
         VvSQL.CreateCommandParameter(cmd, preffix, nalog.LanRecID, TheSchemaTable.Rows[CI.lanRecID]);

         /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, nalog.DokNum,        TheSchemaTable.Rows[CI.dokNum]);
         /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, nalog.DokDate,       TheSchemaTable.Rows[CI.dokDate]);
         /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, nalog.TT,            TheSchemaTable.Rows[CI.tt]);
         /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, nalog.TtNum,         TheSchemaTable.Rows[CI.ttNum]);
         /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, nalog.Napomena,      TheSchemaTable.Rows[CI.napomena]);
         /* 10 */ VvSQL.CreateCommandParameter(cmd, preffix, nalog.FlagA,         TheSchemaTable.Rows[CI.flagA]);
         /* 11 */ VvSQL.CreateCommandParameter(cmd, preffix, nalog.Dug,           TheSchemaTable.Rows[CI.dug]);
         /* 12 */ VvSQL.CreateCommandParameter(cmd, preffix, nalog.Pot,           TheSchemaTable.Rows[CI.pot]);
         /* 13 */ VvSQL.CreateCommandParameter(cmd, preffix, nalog.DevName,       TheSchemaTable.Rows[CI.devName]);

         if(isArhiva)
         {
            VvSQL.CreateCommandParameter(cmd, preffix, nalog.OrigRecID, TheSchemaTable.Rows[CI.origRecID]);
            VvSQL.CreateCommandParameter(cmd, preffix, nalog.RecVer,    TheSchemaTable.Rows[CI.recVer]);
            VvSQL.CreateCommandParameter(cmd, preffix, nalog.ArAction,  TheSchemaTable.Rows[CI.arAction]);
            VvSQL.CreateCommandParameter(cmd, preffix, nalog.ArTS,    TheSchemaTable.Rows[CI.arTS]);
            VvSQL.CreateCommandParameter(cmd, preffix, nalog.ArUID,     TheSchemaTable.Rows[CI.arUID]);
         }

      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataRow

   //public void FillFromDataRow(VvDataRecord arhivedDataRecord, Vektor.DataLayer.DS_nalog.nalogRow nalogRow)
   //{
   //   NalogStruct drData = new NalogStruct();

   //   drData._recID = (uint)nalogRow.prjktKupdobCD;
   //   drData._addTS = nalogRow.addTS;
   //   drData._modTS = nalogRow.modTS;
   //   drData._addUID = nalogRow.addUID;
   //   drData._modUID = nalogRow.modUID;

   //   /* 05 */  drData._dokNum       = nalogRow.dokNum;    
   //   /* 06 */  drData._dokVer       = nalogRow.dokVer;     
   //   /* 07 */  drData._dokDate      = nalogRow.dokDate;    
   //   /* 08 */  drData._tt           = nalogRow.tt;      
   //   /* 09 */  drData._ttNum        = nalogRow.ttNum;  
   //   /* 10 */  drData._napomena     = nalogRow.napomena;   
   //   /* 11 */  drData._flagA        = nalogRow.flagA;   
   //   /* 12 */  drData._arAction     = nalogRow.arAction;     
   //   /* 13 */  drData._arTS       = nalogRow.arTS;  
   //   /* 14 */  drData._arUID        = nalogRow.arUID;   
   //   /* 15 */  drData._tipBrVer_old = nalogRow.tipBrVer_old;    
   //   /* 16 */  drData._tipBrVer_new = nalogRow.tipBrVer_new;    
   //   /* 17 */  drData._dug          = nalogRow.dug;    
   //   /* 18 */  drData._pot          = nalogRow.pot;    

   //   ((Nalog)arhivedDataRecord).CurrentData = drData;

   //   return;
   //}

   #endregion FillFromDataRow

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva)
   {
      NalogStruct rdrData = new NalogStruct();

      rdrData._recID    = reader.GetUInt32  (CI.recID);
      rdrData._addTS    = reader.GetDateTime(CI.addTS);
      rdrData._modTS    = reader.GetDateTime(CI.modTS);
      rdrData._addUID   = reader.GetString  (CI.addUID);
      rdrData._modUID   = reader.GetString  (CI.modUID);
      rdrData._lanSrvID = reader.GetUInt32  (CI.lanSrvID);
      rdrData._lanRecID = reader.GetUInt32  (CI.lanRecID);

      /* 05 */      rdrData._dokNum       = reader.GetUInt32   (CI.dokNum);
      /* 06 */      rdrData._dokDate      = reader.GetDateTime (CI.dokDate);
      /* 07 */      rdrData._tt           = reader.GetString   (CI.tt);
      /* 08 */      rdrData._ttNum        = reader.GetUInt32   (CI.ttNum);
      /* 09 */      rdrData._napomena     = reader.GetString   (CI.napomena);
      /* 10 */      rdrData._flagA        = reader.GetBoolean  (CI.flagA);
      /* 11 */      rdrData._dug          = reader.GetDecimal  (CI.dug);
      /* 12 */      rdrData._pot          = reader.GetDecimal  (CI.pot);
      /* 13 */      rdrData._devName      = reader.GetString   (CI.devName);
                    

      ((Nalog)vvDataRecord).CurrentData = rdrData;

      if(((Nalog)vvDataRecord).Transes != null) ((Nalog)vvDataRecord).Transes.Clear();

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
      Nalog nalog_rec = (Nalog)vvDocumentRecord;

      if(nalog_rec.Transes == null) nalog_rec.Transes = new List<Ftrans>();
      else                          nalog_rec.Transes.Clear();
    //if(nalog_rec.VirtualTranses == null) nalog_rec.VirtualTranses = new VvList<VvTransRecord>();

      LoadGenericTransesList<Ftrans>(conn, nalog_rec.Transes, nalog_rec.RecID, isArhiva);
   }

   #endregion LoadTranses

   #region NalogCI struct & InitializeSchemaColumnIndexes()

   public struct NalogCI
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
      internal int dug;
      internal int pot;
      internal int devName;

      internal int origRecID;
      internal int recVer;
      internal int arAction;
      internal int arTS;
      internal int arUID;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public NalogCI CI;

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
      CI.dug            = GetSchemaColumnIndex("dug");
      CI.pot            = GetSchemaColumnIndex("pot");
      CI.devName        = GetSchemaColumnIndex("devName");

      CI.origRecID      = GetSchemaColumnIndex("origRecID");
      CI.recVer         = GetSchemaColumnIndex("recVer");
      CI.arAction       = GetSchemaColumnIndex("arAction");
      CI.arTS           = GetSchemaColumnIndex("arTS");
      CI.arUID          = GetSchemaColumnIndex("arUID");
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()







   #region AutoAddNalog

   private static uint currDokNum_ForAutoAddNalog;

   private static void AutoAddNalog(XSqlConnection conn, ref ushort line, Nalog nalog_rec, Ftrans ftrans_rec)
   {
      IVvDao nalogDao  = nalog_rec .VvDao;
      IVvDao ftransDao = ftrans_rec.VvDao;

      ushort linesPerDocumentLIMIT = ushort.MaxValue;

      if(line < linesPerDocumentLIMIT) line++;
      else                             line=1;

    // AGSJAJ import only: 
    //if(line == 1 || nalog_rec.DokDate != ZXC.NalogRec.DokDate || nalog_rec.TT != ZXC.NalogRec.TT) // new zaglavlje needed bikoz: (1.st stavka ili novi curr_dokNum) 
      if(line == 1 || nalog_rec.DokDate != ZXC.NalogRec.DokDate) // new zaglavlje needed bikoz: (1.st stavka ili novi curr_dokNum) 
      {
         line = 1; 

         //nalog_rec.VirtualTranses = new List<VvTransRecord>(); // ?? !! bez ovoga zajebava. 

         currDokNum_ForAutoAddNalog = nalogDao.GetNextDokNum(conn, Nalog.recordName);

         nalog_rec.DokNum = currDokNum_ForAutoAddNalog;
         nalog_rec.TtNum  = nalogDao.GetNextTtNum(conn, nalog_rec.VirtualTT, null);


         /* $$$ */ nalogDao.ADDREC(conn, nalog_rec);

         
         // save new Nalog data for further transes 
         ZXC.NalogRec = nalog_rec.MakeDeepCopy();

      } // new zaglavlje needed 


      ftrans_rec.T_serial   = line;

      ftrans_rec.T_parentID = ZXC.NalogRec.RecID;
      ftrans_rec.T_dokNum   = ZXC.NalogRec.DokNum;
      ftrans_rec.T_dokDate  = ZXC.NalogRec.DokDate;
      ftrans_rec.T_TT       = ZXC.NalogRec.TT;

      // 21.01.2016: 
      ftrans_rec.SetOtsKind();

      /* $$$    ftransDao.ADDREC(conn, ftrans_rec);*/
      /* $$$ */
      // 06.01.2016: 
    //ftransDao.ADDREC(conn, ftrans_rec, false, false, false, false);
      ftransDao.ADDREC(conn, ftrans_rec, true , false, false, false); // za PS_SConti... RWT each ftrans_rec for new tipBr pa mu treba recID 

   }

   public static void AutoSetNalog(XSqlConnection conn, ref ushort line, Nalog nalog_rec, Ftrans ftrans_rec)
   {
      AutoAddNalog(conn, ref line, nalog_rec, ftrans_rec);
   }

   public static /*void*/Ftrans AutoSetNalog(
      
      XSqlConnection conn, 
      ref ushort     line, 

      DateTime n_dokDate,
      string   n_tt,
      string   n_napomena,

      string   t_konto,
      uint     t_kupdob_cd,
      string   t_ticker,
      uint     t_mtros_cd,
      string   t_mtros_tk,
      string   t_tipBr,
      string   t_opis,
      DateTime t_valuta,
      string   t_pdv,
      string   t_037,
      string   t_projektCD,
      ZXC.PdvKnjigaEnum t_pdvKnjiga,
      uint     t_fakRecID ,
      ZXC.OtsKindEnum t_otsKind  ,
      string   t_fond,
      string   t_pozicija,
      decimal  t_dug,
      decimal  t_pot)
   {

      Nalog  nalog_rec  = new Nalog();
      Ftrans ftrans_rec = new Ftrans();

      nalog_rec.DokDate  = n_dokDate;
      nalog_rec.TT       = n_tt;
      nalog_rec.Napomena = n_napomena;

      ftrans_rec.T_konto      = t_konto;
      ftrans_rec.T_kupdob_cd  = t_kupdob_cd;
      ftrans_rec.T_ticker     = t_ticker;
      ftrans_rec.T_mtros_cd   = t_mtros_cd;
      ftrans_rec.T_mtros_tk   = t_mtros_tk;
      ftrans_rec.T_tipBr      = t_tipBr;
      ftrans_rec.T_opis       = t_opis;
      ftrans_rec.T_valuta     = t_valuta;
      ftrans_rec.T_pdv        = t_pdv;
      ftrans_rec.T_037        = t_037;
      ftrans_rec.T_projektCD  = t_projektCD;
      ftrans_rec.T_pdvKnjiga  = t_pdvKnjiga;
      ftrans_rec.T_fakRecID   = t_fakRecID ;
      ftrans_rec.T_otsKind    = t_otsKind  ;
      ftrans_rec.T_fond       = t_fond     ;
      ftrans_rec.T_pozicija   = t_pozicija ;
      ftrans_rec.T_dug        = t_dug;
      ftrans_rec.T_pot        = t_pot;

      AutoAddNalog(conn, ref line, nalog_rec, ftrans_rec);

      return ftrans_rec;
   }

   public static /*void*/Ftrans AutoSetNalog(
      
      XSqlConnection conn, 
      ref ushort      line, 

      DateTime n_dokDate,
      string   n_tip,
      string   n_napomena,

      string   t_konto,
      string   t_opis,
      decimal  t_dug,
      decimal  t_pot)
   {

      Nalog  nalog_rec  = new Nalog();
      Ftrans ftrans_rec = new Ftrans();

      nalog_rec.DokDate  = n_dokDate;
      nalog_rec.TT       = n_tip;
      nalog_rec.Napomena = n_napomena;

      ftrans_rec.T_konto      = t_konto;
      ftrans_rec.T_opis       = t_opis;
      ftrans_rec.T_dug        = t_dug;
      ftrans_rec.T_pot        = t_pot;

      AutoAddNalog(conn, ref line, nalog_rec, ftrans_rec);

      return ftrans_rec;
   }

   public static void AutoSetNalog(
      
      XSqlConnection conn, 
      ref ushort      line, 

      DateTime n_dokDate,
      string   n_tip,
      string   n_napomena,

      string   t_projektCD,
      string   t_konto,
      string   t_opis,
      decimal  t_dug,
      decimal  t_pot)
   {

      Nalog  nalog_rec  = new Nalog();
      Ftrans ftrans_rec = new Ftrans();

      nalog_rec.DokDate  = n_dokDate;
      nalog_rec.TT       = n_tip;
      nalog_rec.Napomena = n_napomena;

      ftrans_rec.T_projektCD  = t_projektCD;
      ftrans_rec.T_konto      = t_konto;
      ftrans_rec.T_opis       = t_opis;
      ftrans_rec.T_dug        = t_dug;
      ftrans_rec.T_pot        = t_pot;

      AutoAddNalog(conn, ref line, nalog_rec, ftrans_rec);
   }

   public static void AutoSetNalog(
      
      XSqlConnection conn, 
      ref ushort      line, 

      DateTime n_dokDate,
      string   n_tip,
      string   n_napomena,

      string   t_konto,
      string   t_opis,
      uint     t_mtros_cd,
      string   t_mtros_tk,
      decimal  t_dug,
      decimal  t_pot)
   {

      Nalog  nalog_rec  = new Nalog();
      Ftrans ftrans_rec = new Ftrans();

      nalog_rec.DokDate  = n_dokDate;
      nalog_rec.TT       = n_tip;
      nalog_rec.Napomena = n_napomena;

      ftrans_rec.T_konto      = t_konto;
      ftrans_rec.T_opis       = t_opis;
      ftrans_rec.T_mtros_cd   = t_mtros_cd;
      ftrans_rec.T_mtros_tk   = t_mtros_tk;
      ftrans_rec.T_dug        = t_dug;
      ftrans_rec.T_pot        = t_pot;

      AutoAddNalog(conn, ref line, nalog_rec, ftrans_rec);
   }

   #endregion AutoAddNalog

   #region Close47 34 346

   public static void Close47(XSqlConnection conn, bool ocePrenosNaRashode)
   {
      List<Kplan> listaKonta;
      int xy;
      DateTime projectYearLastDay = ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.ProjectYearLastDay;
      ushort line = 0;
      decimal dug, pot, sum4d, sum7d, sum7p, setDug, setPot;
              dug= pot= sum4d= sum7d= sum7p = decimal.Zero;

      // Rolling for klasa '4' __________________________________ 
      // Rolling for klasa '4' __________________________________ 
      // Rolling for klasa '4' __________________________________ 

      listaKonta = KplanDao.GetKplanList_ForKlasa(conn, "4");

      foreach(Kplan kplan_rec in listaKonta)
      {
         dug = KplanDao.GetKplan_SaldoOrDugOrPot_SUM(conn, ZXC.SaldoOrDugOrPot.DUG, kplan_rec.Konto, "", DateTime.MinValue, DateTime.MaxValue);
         pot = 0;

         //puse: 
         //if(ZXC.IsZero(dug)) continue;
         if(dug.IsZero()) continue;

         sum4d += dug;

         AutoSetNalog(conn, ref line,

            /* DateTime n_dokDate   */ projectYearLastDay,
            /* string   n_tt       */ "TM",
            /* string   n_napomena  */ "ZATVARANJE KLASA '4' i '7'",

            /* string   t_konto     */ kplan_rec.Konto,
            /* string   t_opis      */ "ZATVARANJE KL. '4'",
            /* decimal  t_dug       */ 0,
            /* decimal  t_pot       */ dug);
         //------------------------------------------------------------------------- 

      } // foreach(Kplan kplan_rec in listaKonta) 

      AutoSetNalog(conn, ref line,

         /* DateTime n_dokDate   */ projectYearLastDay,
         /* string   n_tt       */ "TM",
         /* string   n_napomena  */ "ZATVARANJE KLASA '4' i '7'",

         /* string   t_konto     */ "4900",
         /* string   t_opis      */ "ZATVARANJE KL. '4'",
         /* decimal  t_dug       */ sum4d,
         /* decimal  t_pot       */ 0);
      //------------------------------------------------------------------------- 

      if(ocePrenosNaRashode)
      {
         AutoSetNalog(conn, ref line,

            /* DateTime n_dokDate   */ projectYearLastDay,
            /* string   n_tt       */ "TM",
            /* string   n_napomena  */ "ZATVARANJE KLASA '4' i '7'",

            /* string   t_konto     */ "4900",
            /* string   t_opis      */ "ZATVARANJE KL. '4'",
            /* decimal  t_dug       */ 0,
            /* decimal  t_pot       */ sum4d);
         //------------------------------------------------------------------------- 

         AutoSetNalog(conn, ref line,

            /* DateTime n_dokDate   */ projectYearLastDay,
            /* string   n_tt       */ "TM",
            /* string   n_napomena  */ "ZATVARANJE KLASA '4' i '7'",

            /* string   t_konto     */ "7000",
            /* string   t_opis      */ "ZATVARANJE KL. '4'",
            /* decimal  t_dug       */ sum4d,
            /* decimal  t_pot       */ 0);
         //------------------------------------------------------------------------- 

         //AutoSetNalog(conn, ref line,         t-zato da ne zbraja duplo

         //   /* DateTime n_dokDate   */ projectYearLastDay,
         //   /* string   n_tt       */ "TM",
         //   /* string   n_napomena  */ "ZATVARANJE KLASA '4' i '7'",

         //   /* string   t_konto     */ "7000",
         //   /* string   t_opis      */ "ZATVARANJE KL. '7'",
         //   /* decimal  t_dug       */ 0,
         //   /* decimal  t_pot       */ sum4d);
         ////------------------------------------------------------------------------- 

      } // if(ocePrenosNaRashode) 


      // Rolling for klasa '7' __________________________________ 
      // Rolling for klasa '7' __________________________________ 
      // Rolling for klasa '7' __________________________________ 


      listaKonta = KplanDao.GetKplanList_ForKlasa(conn, "7");

      foreach(Kplan kplan_rec in listaKonta)
      {
         dug = KplanDao.GetKplan_SaldoOrDugOrPot_SUM(conn, ZXC.SaldoOrDugOrPot.DUG, kplan_rec.Konto, "", DateTime.MinValue, DateTime.MaxValue);
         pot = KplanDao.GetKplan_SaldoOrDugOrPot_SUM(conn, ZXC.SaldoOrDugOrPot.POT, kplan_rec.Konto, "", DateTime.MinValue, DateTime.MaxValue);

         // 12.02.2016: 
       //xy = int.Parse(kplan_rec.Konto.Remove(2));
         xy = ZXC.ValOrZero_Int(kplan_rec.Konto.SubstringSafe(0, 2));

         if(xy >= 70 && xy <= 73 && dug.NotZero())
         {
            sum7d += dug;

            setDug = 0;
            setPot = dug;
         }
         else if(xy >= 74 && xy <= 78 && pot.NotZero())
         {
            sum7p += pot;

            setDug = pot;
            setPot = 0;
         }
         else continue;

         AutoSetNalog(conn, ref line,

            /* DateTime n_dokDate   */ projectYearLastDay,
            /* string   n_tt       */ "TM",
            /* string   n_napomena  */ "ZATVARANJE KLASA '4' i '7'",

            /* string   t_konto     */ kplan_rec.Konto,
            /* string   t_opis      */ "ZATVARANJE KL. '7'",
            /* decimal  t_dug       */ setDug,
            /* decimal  t_pot       */ setPot);
         //------------------------------------------------------------------------- 

      } // foreach(Kplan kplan_rec in listaKonta) 

      if(ocePrenosNaRashode) 
      {
         AutoSetNalog(conn, ref line,

            /* DateTime n_dokDate   */ projectYearLastDay,
            /* string   n_tt       */ "TM",
            /* string   n_napomena  */ "ZATVARANJE KLASA '4' i '7'",

            /* string   t_konto     */ "7900",
            /* string   t_opis      */ "ZATVARANJE KL. '7'",
            /* decimal  t_dug       */ sum7d,         // + sum4d, t-zato da ne zbraja duplo
            /* decimal  t_pot       */ 0);
         //------------------------------------------------------------------------- 

      } // if(ocePrenosNaRashode) 

      if(ocePrenosNaRashode) sum7d = decimal.Zero;

      sum4d = sum7d-sum7p;

      if(sum4d >= 0)
      {
         setDug = sum4d;
         setPot = 0;
      }
      else
      {
         setDug = 0;
         setPot = -sum4d;
      }

      AutoSetNalog(conn, ref line,

         /* DateTime n_dokDate   */ projectYearLastDay,
         /* string   n_tt       */ "TM",
         /* string   n_napomena  */ "ZATVARANJE KLASA '4' i '7'",

         /* string   t_konto     */ "7900",
         /* string   t_opis      */ "ZATVARANJE KL. '7'",
         /* decimal  t_dug       */ setDug,
         /* decimal  t_pot       */ setPot);
      //------------------------------------------------------------------------- 

   }

   public static void Close34(XSqlConnection conn, bool ocePrenosNaRashode)
   {
      List<Kplan> listaKonta;
      DateTime projectYearLastDay = ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.ProjectYearLastDay;
      ushort line = 0;
      decimal dug, pot, sum4d, sum3p, setDug, setPot;
              dug= pot= sum4d= sum3p = decimal.Zero;

      // Rolling for klasa '4' __________________________________ 
      // Rolling for klasa '4' __________________________________ 
      // Rolling for klasa '4' __________________________________ 

      listaKonta = KplanDao.GetKplanList_ForKlasa(conn, "4");

      foreach(Kplan kplan_rec in listaKonta)
      {
         dug = KplanDao.GetKplan_SaldoOrDugOrPot_SUM(conn, ZXC.SaldoOrDugOrPot.DUG, kplan_rec.Konto, "", DateTime.MinValue, DateTime.MaxValue);
         pot = 0;

         if(dug.IsZero()) continue;

         sum4d += dug;

         AutoSetNalog(conn, ref line,

            /* DateTime n_dokDate   */ projectYearLastDay,
            /* string   n_tt        */ "TM",
            /* string   n_napomena  */ "ZATVARANJE KLASA '3' i '4'",

            /* string   t_konto     */ kplan_rec.Konto,
            /* string   t_opis      */ "ZATVARANJE KL. '4'",
            /* decimal  t_dug       */ 0,
            /* decimal  t_pot       */ dug);
         //------------------------------------------------------------------------- 

      } // foreach(Kplan kplan_rec in listaKonta) 

      AutoSetNalog(conn, ref line,

         /* DateTime n_dokDate   */ projectYearLastDay,
         /* string   n_tt        */ "TM",
         /* string   n_napomena  */ "ZATVARANJE KLASA '3' i '4'",

         /* string   t_konto     */ "49110",
         /* string   t_opis      */ "ZATVARANJE KL. '4'",
         /* decimal  t_dug       */ sum4d,
         /* decimal  t_pot       */ 0);
      //------------------------------------------------------------------------- 


      // Rolling for klasa '3' __________________________________ 
      // Rolling for klasa '3' __________________________________ 
      // Rolling for klasa '3' __________________________________ 
      
      listaKonta = KplanDao.GetKplanList_ForKlasa(conn, "3");

      foreach(Kplan kplan_rec in listaKonta)
      {
         pot = KplanDao.GetKplan_SaldoOrDugOrPot_SUM(conn, ZXC.SaldoOrDugOrPot.POT, kplan_rec.Konto, "", DateTime.MinValue, DateTime.MaxValue);
         dug = 0;

         if(pot.IsZero()) continue;

         sum3p += pot;
      
         AutoSetNalog(conn, ref line,

            /* DateTime n_dokDate   */ projectYearLastDay,
            /* string   n_tt        */ "TM",
            /* string   n_napomena  */ "ZATVARANJE KLASA '3' i '4'",

            /* string   t_konto     */ kplan_rec.Konto,
            /* string   t_opis      */ "ZATVARANJE KL. '3'",
            /* decimal  t_dug       */ pot,
            /* decimal  t_pot       */ 0);
         //------------------------------------------------------------------------- 

      } // foreach(Kplan kplan_rec in listaKonta) 

      AutoSetNalog(conn, ref line,

            /* DateTime n_dokDate   */ projectYearLastDay,
            /* string   n_tt        */ "TM",
            /* string   n_napomena  */ "ZATVARANJE KLASA '3' i '4'",

            /* string   t_konto     */ "39110",
            /* string   t_opis      */ "ZATVARANJE KL. '3'",
            /* decimal  t_dug       */ 0,         // + sum4d, t-zato da ne zbraja duplo
            /* decimal  t_pot       */ sum3p);
         //------------------------------------------------------------------------- 

      if(ocePrenosNaRashode)
      {
        AutoSetNalog(conn, ref line,

               /* DateTime n_dokDate   */ projectYearLastDay,
               /* string   n_tt        */ "TM",
               /* string   n_napomena  */ "ZATVARANJE KLASA '3' i '4'",

               /* string   t_konto     */ "49110",
               /* string   t_opis      */ "ZATVARANJE KL. '4'",
               /* decimal  t_dug       */ 0,
               /* decimal  t_pot       */ sum4d);
            //------------------------------------------------------------------------- 

         AutoSetNalog(conn, ref line,

            /* DateTime n_dokDate   */ projectYearLastDay,
            /* string   n_tt        */ "TM",
            /* string   n_napomena  */ "ZATVARANJE KLASA '3' i '4'",

            /* string   t_konto     */ "39110",
            /* string   t_opis      */ "ZATVARANJE KL. '3'",
            /* decimal  t_dug       */ sum3p,
            /* decimal  t_pot       */ 0);
         //------------------------------------------------------------------------- 

         if(sum4d >= sum3p)
         {
            setDug = sum4d - sum3p;
            setPot = 0;
         }
         else
         {
            setDug = 0;
            setPot = sum3p - sum4d;
         }

         AutoSetNalog(conn, ref line,

            /* DateTime n_dokDate   */ projectYearLastDay,
            /* string   n_tt        */ "TM",
            /* string   n_napomena  */ "ZATVARANJE KLASA '3' i '4'",

            /* string   t_konto     */ "52110",
            /* string   t_opis      */ "OBRAÈUN PRIHODA I RASHODA",
            /* decimal  t_dug       */ setDug,
            /* decimal  t_pot       */ setPot);
         //------------------------------------------------------------------------- 

      } // if(ocePrenosNaRashode) na obracun prihoda i rahoda

   }

   public static void Close346(XSqlConnection conn, bool ocePrenosNaRashode)
   {
      List<Kplan> listaKonta;
      DateTime projectYearLastDay = ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.ProjectYearLastDay;
      ushort line = 0;
      decimal dug, pot, sum4d, sum3d, sum6p/*, setDug, setPot*/;
              dug= pot= sum4d= sum3d= sum6p = decimal.Zero;


      // Rolling for klasa '3' __________________________________ 
      // Rolling for klasa '3' __________________________________ 
      // Rolling for klasa '3' __________________________________ 

      listaKonta = KplanDao.GetKplanList_ForKlasa(conn, "3");

      foreach(Kplan kplan_rec in listaKonta)
      {
         dug = KplanDao.GetKplan_SaldoOrDugOrPot_SUM(conn, ZXC.SaldoOrDugOrPot.DUG, kplan_rec.Konto, "", DateTime.MinValue, DateTime.MaxValue);
         pot = 0;

         if(dug.IsZero()) continue;

         sum4d += dug;

         AutoSetNalog(conn, ref line,

            /* DateTime n_dokDate   */ projectYearLastDay,
            /* string   n_tt        */ "TM",
            /* string   n_napomena  */ "ZATVARANJE KLASA '3','4' i '6'",

            /* string   t_konto     */ kplan_rec.Konto,
            /* string   t_opis      */ "ZATVARANJE KL. '3'",
            /* decimal  t_dug       */ 0,
            /* decimal  t_pot       */ dug);
         //------------------------------------------------------------------------- 

      } // foreach(Kplan kplan_rec in listaKonta) 

      AutoSetNalog(conn, ref line,

         /* DateTime n_dokDate   */ projectYearLastDay,
         /* string   n_tt        */ "TM",
         /* string   n_napomena  */ "ZATVARANJE KLASA '3','4' i '6'",

         /* string   t_konto     */ "92111",
         /* string   t_opis      */ "ZATVARANJE KL. '3'",
         /* decimal  t_dug       */ sum3d,
         /* decimal  t_pot       */ 0);
      //------------------------------------------------------------------------- 


      // Rolling for klasa '4' __________________________________ 
      // Rolling for klasa '4' __________________________________ 
      // Rolling for klasa '4' __________________________________ 

      listaKonta = KplanDao.GetKplanList_ForKlasa(conn, "4");

      foreach(Kplan kplan_rec in listaKonta)
      {
         dug = KplanDao.GetKplan_SaldoOrDugOrPot_SUM(conn, ZXC.SaldoOrDugOrPot.DUG, kplan_rec.Konto, "", DateTime.MinValue, DateTime.MaxValue);
         pot = 0;

         if(dug.IsZero()) continue;

         sum4d += dug;

         AutoSetNalog(conn, ref line,

            /* DateTime n_dokDate   */ projectYearLastDay,
            /* string   n_tt        */ "TM",
            /* string   n_napomena  */ "ZATVARANJE KLASA '3','4' i '6'",

            /* string   t_konto     */ kplan_rec.Konto,
            /* string   t_opis      */ "ZATVARANJE KL. '4'",
            /* decimal  t_dug       */ 0,
            /* decimal  t_pot       */ dug);
         //------------------------------------------------------------------------- 

      } // foreach(Kplan kplan_rec in listaKonta) 

      AutoSetNalog(conn, ref line,

         /* DateTime n_dokDate   */ projectYearLastDay,
         /* string   n_tt        */ "TM",
         /* string   n_napomena  */ "ZATVARANJE KLASA '3','4' i '6'",

         /* string   t_konto     */ "92121",
         /* string   t_opis      */ "ZATVARANJE KL. '4'",
         /* decimal  t_dug       */ sum4d,
         /* decimal  t_pot       */ 0);
      //------------------------------------------------------------------------- 

      // Rolling for klasa '6' __________________________________ 
      // Rolling for klasa '6' __________________________________ 
      // Rolling for klasa '6' __________________________________ 

      listaKonta = KplanDao.GetKplanList_ForKlasa(conn, "6");

      foreach(Kplan kplan_rec in listaKonta)
      {
         pot = KplanDao.GetKplan_SaldoOrDugOrPot_SUM(conn, ZXC.SaldoOrDugOrPot.POT, kplan_rec.Konto, "", DateTime.MinValue, DateTime.MaxValue);
         dug = 0;

         if(pot.IsZero()) continue;

         sum6p += pot;
      
         AutoSetNalog(conn, ref line,

            /* DateTime n_dokDate   */ projectYearLastDay,
            /* string   n_tt        */ "TM",
            /* string   n_napomena  */ "ZATVARANJE KLASA '3','4' i '6'",

            /* string   t_konto     */ kplan_rec.Konto,
            /* string   t_opis      */ "ZATVARANJE KL. '6'",
            /* decimal  t_dug       */ pot,
            /* decimal  t_pot       */ 0);
         //------------------------------------------------------------------------- 

      } // foreach(Kplan kplan_rec in listaKonta) 

      AutoSetNalog(conn, ref line,

            /* DateTime n_dokDate   */ projectYearLastDay,
            /* string   n_tt        */ "TM",
            /* string   n_napomena  */ "ZATVARANJE KLASA '3','4' i '6'",

            /* string   t_konto     */ "92111",
            /* string   t_opis      */ "ZATVARANJE KL. '6'",
            /* decimal  t_dug       */ 0,         
            /* decimal  t_pot       */ sum6p);
         //------------------------------------------------------------------------- 
   }

   #endregion Close47 34 346

   #region PS GlavnaKnjiga + SaldaConti

   public static bool PS_SConti(XSqlConnection tabPagesConnection, string PG_dbName, List<Kplan> kplanList, bool skipWYRN)
   {
      #region Init stuff

      Ftrans ftrans_rec = new Ftrans();
      IVvDao vvDao      = ZXC.FtransDao;

      ushort  line    = 0;
      int     ncount  = 0;
      int     rnCount = 0;
      decimal currDug = 0, currPot = 0;

      string currKTO = "", currTCK = "", currTBR = "", currOpis = "";
      string currPrjktCD = "";
      uint   currKCD = 0;
      uint   currMCD = 0; // 25.05.2021. 
      string currMTK = "";// 25.05.2021. 

      DateTime currDokDate, currValuta, ngT_valuta;
               currDokDate= currValuta = DateTime.MinValue;

      // 29.12.2015: 
      List<Ftrans> currGroupFtransList = new List<Ftrans>();
      bool isSaldaKontiKTO;
    //bool firstRnOccur = true;
    //List<string> pgT_tipBrList   = new List<string>();
      List<Ftrans> addedSaldaKontiFtransList = new List<Ftrans>();
      Ftrans       addedSaldaKontiFtrans;

      string dlgTipBr;
      DateTime dlgDokDate, dlgDospDate = DateTime.MinValue;
      decimal dlgPdvSt, dlgOsnovica, dlgPdv, dlgProlaz;
      ZXC.PdvKolTipEnum dlgPdvKolTip;
      AddDateToWYRNdlg dlg;

      #endregion Init stuff

      using(XSqlConnection tempConnection = VvSQL.CREATE_TEMP_XSqlConnection(PG_dbName))
      {
         #region ADDREC ftrans in NY 

         // 29.12.2015: 
       //using(XSqlCommand cmd = VvSQL.Get_SELECTzvjezdicaFROM_Command(tempConnection, Ftrans.recordName, "", null, "t_konto, t_kupdob_cd, t_tipBr",                                                         "", ""))
         // 23.03.2017:                                                                                                                                                      
       //using(XSqlCommand cmd = VvSQL.Get_SELECTzvjezdicaFROM_Command(tempConnection, Ftrans.recordName, "", null, "t_konto, t_kupdob_cd, t_tipBr ,                         t_dokDate, t_dokNum, t_serial", "", ""))
         // 25.05.2021:                                                                                                                                                      
       //using(XSqlCommand cmd = VvSQL.Get_SELECTzvjezdicaFROM_Command(tempConnection, Ftrans.recordName, "", null, "t_konto, t_kupdob_cd, t_tipBr, t_projektCD,             t_dokDate, t_dokNum, t_serial", "", ""))
         using(XSqlCommand cmd = VvSQL.Get_SELECTzvjezdicaFROM_Command(tempConnection, Ftrans.recordName, "", null, "t_konto, t_kupdob_cd, t_tipBr, t_projektCD, t_mtros_cd, t_dokDate, t_dokNum, t_serial", "", ""))
         {
            try
            {
               using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult /*| CommandBehavior.SingleRow*/))
               {
                  while(reader.HasRows && reader.Read())
                  {
                     vvDao.FillFromDataReader(ftrans_rec, reader, false);

                     //============================================================================================================= 
                     //= loop START ================================================================================================ 
                     //============================================================================================================= 

                     if(NewGroupOf(ftrans_rec, currKTO, currKCD, currMCD, currTBR, currPrjktCD, kplanList))
                     {
                        isSaldaKontiKTO = IsSaldaKontiKTO_ForNalogPS(currKTO, kplanList);
                        if(!isSaldaKontiKTO) currTBR = ""; // da na nalogu TipBr bude prazan 

                        if((currDug - currPot).NotZero())
                        {
                           // 29.12.2015: 
                           if(!isSaldaKontiKTO) // obican konto 
                           {
                              SendToPsNalog(tabPagesConnection, currKTO, currKCD, currTCK, currMCD, currMTK, currTBR, currPrjktCD, currDokDate, currValuta, ref line, currDug, currPot, currOpis, false);
                              ncount++;
                           }
                           else // YES, this IS SaldaKontiKTO ... prebaci ga analiticki (otvaranje i sva eventualna djelomicna zatvaranja) 
                           {
                              foreach(Ftrans ftrans in currGroupFtransList.OrderBy(f => f.T_otsKind)) // silimo da otvaranje dode na vrh ukoliko je zmesano 
                              {
                                 addedSaldaKontiFtrans = SendToPsNalog(tabPagesConnection, ftrans.T_konto, ftrans.T_kupdob_cd, ftrans.T_ticker, ftrans.T_mtros_cd, ftrans.T_mtros_tk, ftrans.R_forcedTipBr, ftrans.T_projektCD, ftrans.T_dokDate, ftrans.T_valuta, ref line, ftrans.T_dug, ftrans.T_pot, ftrans.T_opis, true);
                                 ncount++;
                               //addedSaldaKontiFtrans.T_tipBr = ftrans.T_tipBr; // jer ga SendToPsNalog reformira sa onim '5p' na pocetku 
                                 if(IsSaldaKontiKTO(ftrans.T_konto)) addedSaldaKontiFtransList.Add(addedSaldaKontiFtrans);
                              }
                           }

                        } /* ako saldoOTS nije Nula */

                        currDug     = currPot =           0M;
                        currKTO     = ftrans_rec.T_konto    ;
                        currKCD     = ftrans_rec.T_kupdob_cd;
                        currTCK     = ftrans_rec.T_ticker   ;
                        currMCD     = ftrans_rec.T_mtros_cd ;
                        currMTK     = ftrans_rec.T_mtros_tk ;
                        currTBR     = ftrans_rec.T_tipBr    ; 
                      //currTBR     = ftrans_rec.R_forcedTipBr; // !!! novo u 2016. Ako je T_tipBr prazan onda : ["XY:" + T_dokDate.Year + "-" + T_recID] 
                        currPrjktCD = ftrans_rec.T_projektCD; 

                        // 29.12.2015: 
                        currGroupFtransList = new List<Ftrans>();

                        // Bijo BUG! dodano tek 20.01.2012: 
                        currDokDate = ftrans_rec.T_dokDate;
                        currValuta  = ftrans_rec.T_valuta ;
                        currOpis    = ftrans_rec.T_opis   ;

                     } /* novi set KTO - KCD - TBR */

                     currDug += ftrans_rec.T_dug;
                     currPot += ftrans_rec.T_pot;

                     // 29.12.2015: 
                     currGroupFtransList.Add(ftrans_rec.MakeDeepCopy());

                     //============================================================================================================= 
                     //= loop END ================================================================================================== 
                     //============================================================================================================= 

                  } // while(reader.HasRows && reader.Read()) 

                  reader.Close();

                  if((currDug - currPot).NotZero()) /* ...za ZADNJEGA */
                  {
                     isSaldaKontiKTO = IsSaldaKontiKTO_ForNalogPS(currKTO, kplanList);
                     if(!isSaldaKontiKTO) currTBR = ""; // da na nalogu TipBr bude prazan 

                     // 29.12.2015: 
                     if(!isSaldaKontiKTO) // obican konto 
                     { 
                        SendToPsNalog(tabPagesConnection, currKTO, currKCD, currTCK, currMCD, currMTK, currTBR, currPrjktCD, currDokDate, currValuta, ref line, currDug, currPot, currOpis, false);
                        ncount++;
                     }
                     else // YES, this IS SaldaKontiKTO ... prebaci ga analiticki (otvaranje i sva eventualna djelomicna zatvaranja) 
                     {
                        foreach(Ftrans ftrans in currGroupFtransList.OrderBy(f => f.T_otsKind)) // silimo da otvaranje dode na vrh ukoliko je zmesano 
                        {
                           addedSaldaKontiFtrans = SendToPsNalog(tabPagesConnection, ftrans.T_konto, ftrans.T_kupdob_cd, ftrans.T_ticker, ftrans.T_mtros_cd, ftrans.T_mtros_tk, ftrans.R_forcedTipBr, ftrans.T_projektCD, ftrans.T_dokDate, ftrans.T_valuta, ref line, ftrans.T_dug, ftrans.T_pot, ftrans.T_opis, true);
                           ncount++;
                         //addedSaldaKontiFtrans.T_tipBr = ftrans.T_tipBr; // jer ga SendToPsNalog reformira sa onim '5p' na pocetku 
                           if(IsSaldaKontiKTO(ftrans.T_konto)) addedSaldaKontiFtransList.Add(addedSaldaKontiFtrans);
                        }
                     }

                  } /* ako saldoOTS nije Nula */
               }
            }
            catch(XSqlException ex) { VvSQL.ReportSqlError("PS_SConti", ex, System.Windows.Forms.MessageBoxButtons.OK); return false; }

         } // using(XSqlCommand cmd = VvSQL.Get_SELECTzvjezdicaFROM_Command(conn, Ftrans.tableName, null, "t_konto, t_kupdob_cd, t_tipBr")) 

         #endregion ADDREC ftrans in NY

         #region AutoADD Pg ILI PgPg _WYRN_faktur OR just get ngTipBr if EQLREC is OK (ponavljamo prenos) ... then RWTREC ftrans in NY

         // new in 2015/2016: 
         string ngT_tipBr; uint fakRecID; string origTipBr;
       //if(skipWYRN == false) foreach(var ftrGR in addedSaldaKontiFtransList.GroupBy(ftr =>                                 ftr.R_forcedTipBr))
         if(skipWYRN == false) foreach(var ftrGR in addedSaldaKontiFtransList.GroupBy(ftr => ftr.T_konto + ftr.T_kupdob_cd + ftr.R_forcedTipBr))
         {
            if(ZXC.CURR_prjkt_rec.NoNeedFor_WRN_UFRA && (ZXC.TheVvForm.TheVvUC.Get_Kplan_FromVvUcSifrar(ftrGR.First().T_konto)).IsKontoDobav) continue; //ako CURR_prjkt nije niti poduzece po naplati i ako nije ObrtPdvR2 tada ge se preskace

            ngT_valuta = ftrGR.First().T_valuta;

            // first check nije li ponavljanje prenosa 
            ngT_tipBr = FindWYRNfakturInNG(tabPagesConnection, ftrGR.First().T_kupdob_cd, ftrGR.First().R_forcedTipBr, out fakRecID, ref ngT_valuta);

            if(ngT_tipBr == "NO_NG") // NEMA WYRN faktur_rec-a u ovoj godini 
            {
               // ADD it to RISK: try first PG variant 
               ngT_tipBr = AutoADD_PG_WYRN_faktur(tabPagesConnection, tempConnection, ftrGR.First().R_forcedTipBr, ftrGR.First().OrigPgTipBr, out fakRecID, ref ngT_valuta);

               // ADD it to RISK: there is NO_PG faktur, go with PgPg variant 
               if(ngT_tipBr == "NO_PG") // NEMA orig faktur_rec-a u prosloj godini 
               {
                //if(ftrGR.Count() == 1 && ftrGR.First().T_otsKind == ZXC.OtsKindEnum.ZATVARANJE)
                  if(ftrGR.Count(f => f.T_otsKind == ZXC.OtsKindEnum.OTVARANJE).IsZero())
                  {
                     continue; // preskaci 'usamljene' uplate - za njih ne radi wyrn 
                  }

                  #region Get PgPg WYRN Data dialog

                //dlg = new AddDateToWYRNdlg(ZXC.TheVvForm.TheVvUC, ftrGR.First());
                  dlg = new AddDateToWYRNdlg(ZXC.TheVvForm.TheVvUC, ftrGR.ToList());

                  var dlgResult = dlg.ShowDialog();

                  if(dlgResult == System.Windows.Forms.DialogResult.Abort) break;

                  dlgPdvSt     = dlg.TheUC.Fld_PdvSt    ;
                  dlgOsnovica  = dlg.TheUC.Fld_Osnovica ;
                  dlgPdv       = dlg.TheUC.Fld_Pdv      ;
                  dlgProlaz    = dlg.TheUC.Fld_Prolaz   ;
                  dlgPdvKolTip = dlg.TheUC.Fld_PdvKolTip;
                  dlgDokDate   = dlg.TheUC.Fld_DokDate  ;
                  dlgDospDate  = dlg.TheUC.Fld_DospDate ;
                  dlgTipBr     = dlg.TheUC.Fld_TipBr    ;


                  dlg.Dispose();

                  #endregion Get PgPg WYRN Data dialog

                //ngT_tipBr = AutoADD_PgPg_WYRN_faktur(tabPagesConnection, ftrGR.First(), out fakRecID, dokDate, 1400M, 250M, 400, ZXC.PdvKolTipEnum.PROLAZ); // ovdje ces upasti u probleme ako otvaranje NIJE prvi ftrans (npr avans pa tek onda otvaranje) 
                  ngT_tipBr = NalogDao.AutoADD_PgPg_WYRN_faktur(tabPagesConnection, ftrGR.First(), out fakRecID, // ovdje ces upasti u probleme ako otvaranje NIJE prvi ftrans (npr avans pa tek onda otvaranje) 
                                                                      dlgTipBr    ,
                                                                      dlgDokDate  , 
                                                                      dlgDospDate ,
                                                                      dlgPdvSt    , 
                                                                      dlgOsnovica , 
                                                                      dlgPdv      , 
                                                                      dlgProlaz   ,
                                                                      dlgPdvKolTip);
                  ngT_valuta = dlgDospDate;

               } // if(ngT_tipBr == "NO_PG") // NEMA orig faktur_rec-a u prosloj godini 

               rnCount++;
            } // if(ngT_tipBr == "NO_NG") // NEMA WYRN faktur_rec-a u ovoj godini 

            // RWT each ftrans_rec for new tipBr 
            if(ngT_tipBr.NotEmpty()) foreach(Ftrans ftrans in ftrGR)
            {
               ZXC.TheVvForm.BeginEdit(ftrans);

               origTipBr         = ftrans.R_forcedTipBr;
               ftrans.T_tipBr    = ngT_tipBr           ;
               ftrans.T_fakRecID = fakRecID            ;
                                                       
               ftrans.T_valuta   = ngT_valuta          ; // !!! 


               ftrans.T_opis = LimitedOpisStr(origTipBr + "/" + ftrans.T_opis);

               ftrans.VvDao.RWTREC(tabPagesConnection, ftrans, false, true, false);

               ZXC.TheVvForm.EndEdit(ftrans);
            }
         } // foreach(var ftrGR in addedSaldaKontiFtransList.GroupBy(ftr => ftr.T_tipBr)) 

         #endregion AutoADD_ PG/PgPg _WYRN_faktur OR just get ngTipBr if EQLREC is OK (ponavljamo prenos) ... then RWTREC ftrans in NY

      } // using(XSqlConnection tempConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection()) 

      ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Gotovo. Dodao:\n\n{0}\tstavki naloga PS\n\ni\n\n{1}\tprenesenih RISK dokumenata", ncount, rnCount);

      return true; // localOK 
   }

   private static string FindWYRNfakturInNG(XSqlConnection ngConn, uint kupdobCD, string ngT_tipBr, out uint fakRecID, ref DateTime ngT_valuta)
   {
      if(ngT_tipBr.IsEmpty()) { fakRecID = 0; return "NO_NG"; } // !!! 

      Faktur ngWYRNfaktur_rec = new Faktur();

    //bool OK = FakturDao.SetMeFaktur_ByVezniDok2           (ngConn, ngWYRNfaktur_rec,           ngT_tipBr);
      bool OK = FakturDao.SetMeFaktur_ByKupdobCdAndVezniDok2(ngConn, ngWYRNfaktur_rec, kupdobCD, ngT_tipBr);

      if(!OK) { fakRecID = 0; return "NO_NG"; } // !!! 

      fakRecID   = ngWYRNfaktur_rec.RecID   ;
      ngT_valuta = ngWYRNfaktur_rec.DospDate;

      return ngWYRNfaktur_rec.TT_And_TtNum;
   }

   // faktur je stariji od 1 godine i NEMA ga u pgFaktur data layeru ... CreateDefault WYRN faktur_rec sa defaultnim PDV podacima 
   public static string AutoADD_PgPg_WYRN_faktur(XSqlConnection conn, Ftrans pgpgFtrans_rec, out uint fakRecID, 
      string            dlgTipBr    ,
      DateTime          dlgDokDate  , 
      DateTime          dlgDospDate ,
      decimal           dlgPdvSt    , 
      decimal           dlgOsnovica , 
      decimal           dlgPdv      , 
      decimal           dlgProlaz   ,
      ZXC.PdvKolTipEnum dlgPdvKolTip)
   {
      decimal moneyKCRP = pgpgFtrans_rec.R_DugMinusPot_ABS;

      bool isULAZ  = Kplan.GetIsKontoDobav(pgpgFtrans_rec.T_konto);
      bool isIZLAZ = !isULAZ;

      #region Set faktur_rec

      Faktur faktur_rec = new Faktur();
      ushort line = 0;

      faktur_rec.VezniDok2 = dlgTipBr; // !!! 

      if(isULAZ)       faktur_rec.TT = Faktur.TT_WRN;
      else /* IZLAZ */ faktur_rec.TT = Faktur.TT_YRN;

      faktur_rec.SkladDate =
      faktur_rec.DokDate   = (dlgDokDate.NotEmpty() ? dlgDokDate : pgpgFtrans_rec.DokDateFromOpis);
      faktur_rec.DospDate  = dlgDospDate;
      faktur_rec.SkladCD   = "";

      #region Kupdob Data

      Kupdob kupdob_rec = ZXC.TheVvForm.TheVvUC.Get_Kupdob_FromVvUcSifrar(pgpgFtrans_rec.T_kupdob_cd);
      if(kupdob_rec != null)
      {
         faktur_rec.KupdobCD     = kupdob_rec.KupdobCD;
         faktur_rec.KupdobTK     = kupdob_rec.Ticker  ;
         faktur_rec.KupdobName   = kupdob_rec.Naziv   ;
         faktur_rec.KdOib        = kupdob_rec.Oib;
         faktur_rec.VatCntryCode = kupdob_rec.VatCntryCode;
         faktur_rec.KdUlica      = kupdob_rec.Ulica2;
         faktur_rec.KdZip        = kupdob_rec.PostaBr;
         faktur_rec.KdMjesto     = kupdob_rec.Grad;
         faktur_rec.DevName      = kupdob_rec.DevName;

         faktur_rec.PdvKnjiga    = ZXC.PdvKnjigaEnum.REDOVNA;

         if(faktur_rec.TT == Faktur.TT_WRN)
            faktur_rec.ZiroRn = ZXC.GetIBANfromOldZiro(kupdob_rec.Ziro1);

         if(kupdob_rec.PdvRTip == ZXC.PdvRTipEnum.OBRT_R2 && faktur_rec.TT == Faktur.TT_WRN)
         {
            faktur_rec.PdvR12 = ZXC.PdvR12Enum.R2;
         }
         else
         {
            faktur_rec.PdvR12 = ZXC.PdvR12Enum.R1;
         }

         #region Putnik 06.11.2013.

         if(kupdob_rec.PutnikID.NotZero()) faktur_rec.PersonCD   = kupdob_rec.PutnikID;
         if(kupdob_rec.PutName.NotEmpty()) faktur_rec.PersonName = kupdob_rec.PutName ;

         #endregion Putnik

         #region EU VAT Code Action

         if(ZXC.EU_VatCodes_woHR.Contains(kupdob_rec.VatCntryCode) && faktur_rec.TtInfo.IsUlazniPdvTT)
         {
            faktur_rec.PdvKnjiga  = ZXC.PdvKnjigaEnum .NIJEDNA;
            faktur_rec.PdvGEOkind = ZXC.PdvGEOkindEnum.EU;
            faktur_rec.PdvR12     = ZXC.PdvR12Enum    .R1;
         }
         if(ZXC.EU_VatCodes_woHR.Contains(kupdob_rec.VatCntryCode) && faktur_rec.TtInfo.IsIzlazniPdvTT)
         {
            faktur_rec.PdvKnjiga  = ZXC.PdvKnjigaEnum .REDOVNA;
            faktur_rec.PdvGEOkind = ZXC.PdvGEOkindEnum.EU;
            faktur_rec.PdvR12     = ZXC.PdvR12Enum    .R1;
         }

         #endregion EU VAT Code Action

      }
      else ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Nema partnera sa šifrom [{0}]\n\n[{0}] [{1}]", pgpgFtrans_rec.T_kupdob_cd, pgpgFtrans_rec.T_ticker);

      #endregion Kupdob Data

      #endregion Set faktur_rec

      #region Set Rtrans

      Rtrans kcr_rtrans_rec;
      kcr_rtrans_rec = new Rtrans();

      kcr_rtrans_rec.T_TT         = faktur_rec.TT;
      kcr_rtrans_rec.T_artiklCD   = "";
      kcr_rtrans_rec.T_artiklName = "PDV OSNOVICA";
      kcr_rtrans_rec.T_jedMj      = "";
      kcr_rtrans_rec.T_kol        = 1M;
      kcr_rtrans_rec.T_kol2       = 0M;
      kcr_rtrans_rec.T_pdvSt      = dlgPdvSt;
      kcr_rtrans_rec.T_cij        = dlgOsnovica;
      kcr_rtrans_rec.T_pdvColTip  = dlgPdvKolTip;

      kcr_rtrans_rec.CalcTransResults(null);
      faktur_rec.Transes.Add(kcr_rtrans_rec);

      Rtrans prlz_rtrans_rec = null;
      if(dlgProlaz.NotZero())
      {
         prlz_rtrans_rec = new Rtrans();

         prlz_rtrans_rec.T_TT         = faktur_rec.TT;
         prlz_rtrans_rec.T_artiklCD   = "";
         prlz_rtrans_rec.T_artiklName = "PROLAZNA STAVKA";
         prlz_rtrans_rec.T_jedMj      = "";
         prlz_rtrans_rec.T_kol        = 1M;
         prlz_rtrans_rec.T_kol2       = 0M;
         prlz_rtrans_rec.T_pdvSt      = 0M;
         prlz_rtrans_rec.T_cij        = dlgProlaz;
         prlz_rtrans_rec.T_pdvColTip  = dlgPdvKolTip;

         prlz_rtrans_rec.CalcTransResults(null);
         faktur_rec.Transes.Add(prlz_rtrans_rec);
      }

      #endregion Set Rtrans

      #region TakeTransesSumToDokumentSum
      
      faktur_rec.TakeTransesSumToDokumentSum(true);
      faktur_rec.Transes = null;

      #endregion TakeTransesSumToDokumentSum

      FakturDao.AutoSetFaktur(conn, ref line, faktur_rec, kcr_rtrans_rec);

      if(prlz_rtrans_rec != null && dlgProlaz.NotZero())
      {
         FakturDao.AutoSetFaktur(conn, ref line, faktur_rec, prlz_rtrans_rec);
      }

      fakRecID = faktur_rec.RecID;

      bool OK = fakRecID.NotZero();

      if(OK) return faktur_rec.TT_And_TtNum;
    //if(OK) return pgpgFtrans_rec.T_tipBr ;
      else   return "";
   }

   // faktur NIJE stariji od 1 godine i IMA ga u pgFaktur data layeru ... 'CopyOUT' 
   private static string AutoADD_PG_WYRN_faktur(XSqlConnection conn, XSqlConnection pgConn, string pgT_tipBr /*5pIRA-100007*/, string origPgT_tipBr /*IRA-100007*/, out uint fakRecID, ref DateTime ngT_valuta)
   {
      string origTT;
      uint   ttNum ;

    //Ftrans.ParseTipBr(pgT_tipBr    , out     tt, out ttNum);
      Ftrans.ParseTipBr(origPgT_tipBr, out origTT, out ttNum);

      Faktur pgFaktur_rec = new Faktur();

      bool OK = FakturDao.SetMeFaktur(pgConn, pgFaktur_rec, origTT, ttNum, /*false*/true);

      if(!OK) { fakRecID = 0; return "NO_PG"; } // !!! 

      string newWyrnTT     = Ftrans.Get_WYRNtt(origTT/*, pgFaktur_rec.DokDate.Year*/);
      short  newWyrnTtSort = ZXC.TtInfo(newWyrnTT).TtSort;

      pgFaktur_rec.VvDao.LoadTranses(pgConn, pgFaktur_rec, false);

      if(pgFaktur_rec.TtInfo.IsMalopFin_I) // IRM 
      {
         pgFaktur_rec.SkladCD = ""; // da ocisti 'MPSK' jer mu inace ne bi WYRN dao sejvati koda event. RWTREC-a 
      }

      pgFaktur_rec.TT        = newWyrnTT;
      pgFaktur_rec.TtSort    = newWyrnTtSort;
      pgFaktur_rec.TtNum     = pgFaktur_rec.VvDao.GetNextTtNum(conn, newWyrnTT, "");
      pgFaktur_rec.VezniDok2 = pgT_tipBr;    
      
      foreach(Rtrans rtrans_rec in pgFaktur_rec.Transes)
      {
         if(rtrans_rec.TtInfo.IsStornoTT)
         {
            rtrans_rec.T_cij = -1M * Math.Abs(rtrans_rec.T_cij); // force NEGATIVE t_cij 
         }

         if(rtrans_rec.TtInfo.IsMalopFin_I) // IRM 
         {
            rtrans_rec.TransformMalop_T_cij_ToVelep_T_cij();
         }

         rtrans_rec.T_TT     = newWyrnTT;
         rtrans_rec.T_ttSort = newWyrnTtSort;

         // 11.04.2022: 
         rtrans_rec.T_ttNum  = pgFaktur_rec.TtNum;

         rtrans_rec.CalcTransResults(null);
      }

      pgFaktur_rec.TakeTransesSumToDokumentSum(true);

      if(ZXC.projectYearAsInt == 2023)
      {
         pgFaktur_rec.Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB<Faktur>(conn);

         pgFaktur_rec.Transes.ForEach(rtr => rtr.Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB<Rtrans>(conn));
      }

      OK = pgFaktur_rec.VvDao.ADDREC(conn, pgFaktur_rec);

      fakRecID = pgFaktur_rec.RecID;

      ngT_valuta = pgFaktur_rec.DospDate;

      if(OK) return pgFaktur_rec.TT_And_TtNum;
      else   return "";
   }

   private static bool NewGroupOf(Ftrans ftransPG_rec, string currKTO, uint currKCD, uint currMCD, string currTBR, string currPrjktCD, List<Kplan> kplanList)
   {

      if(currKTO != ftransPG_rec.T_konto)        return(true);

      if(IsSaldaKontiKTO_ForNalogPS(currKTO, kplanList))
      {
         if(currKCD != ftransPG_rec.T_kupdob_cd) return(true);

         // 03.1.2022: tek sada primjecujemo da ova novost dodadna 21.05.2021: da kada gledas da li je saldo grupe knjizenja na nuli                 
         //            a da bi zakljucio ide li ili ne u PS salda konti dodan MtrosCD kao novi, dodatni kriterij ... zjebes ostalima koji to ne zele 
         //            a iz nekog razloga (say, Plodine) imaju mtrosCD na otvaranju a nemaju na zatvaranju pa im ova novost smeta.                   
         // ... pa je currMCD ispitivanje zaokruzeno if()-om                                                                                         

         if(ZXC.KSD.Dsc_Is_OTSviaMtrosCD) 
         {
            if(currMCD != ftransPG_rec.T_mtros_cd) return (true);
         }

         // 04.04.2012:
       //if(currTBR           != ftransPG_rec.T_tipBr          ) return (true);
         if(currTBR.ToUpper() != ftransPG_rec.T_tipBr.ToUpper()) return (true);

       //if(currTBR.ToUpper() != ftransPG_rec.R_forcedTipBr.ToUpper()) return (true);
      }

      // 23.03.2017: 
      if(IsProizvodnjaUtijekuKTO(currKTO))
      {
         if(currPrjktCD != ftransPG_rec.T_projektCD) return (true);
      }

      return(false);
   }

   /*private*/public static bool IsProizvodnjaUtijekuKTO(string KTO)
   {
      if(KTO.IsEmpty() || KTO.Length < 2) return false;

      return (KTO == ZXC.KSD.Dsc_otp_ktoObrade);
   }

   /*private*/public static bool IsSaldaKontiKTO_ForNalogPS(string KTO, List<Kplan> kplanList)
   {
      if(KTO.IsEmpty() || KTO.Length < 2) return false;

      // C# 2.0: 
      //Kplan kplan_rec = kplanList.FindByKonto(KTO);

      // C# 3.0: 
      /*var kplanPartial_rec =
         (from a in kplanList
         //.Where(
         select new { PsRule = a.PsRule }).Single();
      */

      // LINQ 
      var PsRule =

          kplanList 
         .Where (k => k.Konto == KTO)
         .Select(k => k.PsRule)
         .SingleOrDefault();

      if(PsRule == Kplan.PsRuleEnum.SUPRESS_SaldaKontiKTO) return false;
      if(PsRule == Kplan.PsRuleEnum.FORCE_SaldaKontiKTO  ) return true ;

      // !!! U 2016 uvodimo izbacivanje konta gotovine (npr. 1203 kod Ducatija) iz izvjestaja OTS 
      // izbacivanje istoga iz ove metode se moze obaviti i tako da mu se zada 'PsRule == Kplan.PsRuleEnum.SUPRESS_SaldaKontiKTO' 
      // mozda ubuduce to izjednaciti tj da izvj OTS gleda ovaj PsRule svog konta 

      // 19.01.2016: 
      if(Kplan.GetIsKontoKupac(KTO) || Kplan.GetIsKontoDobav(KTO)) return true;

      #region Explicit Legacy Rules

// 19.01.2016: 
#if NoNoLegacyAnymore

      if(KTO.Remove(2) == "12" || 
         KTO.Remove(2) == "22" ||
         KTO == "0631"    ||
         KTO == "0640"    ||
         KTO == "0670"    ||
         KTO == "0690"    ||
         KTO == "1130"    ||
         KTO == "1133"    ||
         KTO == "1139"    ||
         KTO == "113000"  ||
         KTO == "113001"  ||
         KTO == "113002"  ||
         KTO == "113003"  ||
         KTO == "113004"  ||
         KTO == "113005"  ||
         KTO == "1131"    ||
         KTO == "130000"  ||
         KTO == "130100"  ||
         KTO == "1301"    || // hmid 2005 ---> 2006 
         KTO == "14730"   ||
         KTO == "147300"  ||
         KTO == "147301"  ||
         KTO == "147302"  ||
         KTO == "147303"  ||
         KTO == "147400"  ||
         KTO == "147610"  ||
         KTO == "147700"  ||
         KTO == "147702"  ||
         KTO == "147704"  ||
         KTO == "1490"    ||
         KTO == "167600"  ||
         KTO == "167700"  ||
         KTO == "167800"  ||
         KTO == "167900"  ||
         KTO == "2000"    ||
         KTO == "2011"    ||
         KTO == "2012"    ||
         KTO == "2013"    ||
         KTO == "2130"    ||
         KTO == "21300"   ||
         KTO == "213000"  ||
         KTO == "213001"  ||
         KTO == "213002"  ||
         KTO == "213003"  ||
         KTO == "213004"  ||
         KTO == "213005"  ||
         KTO == "213006"  ||
         KTO == "21301"   ||
         KTO == "2131"    ||
         KTO == "2140"    ||
         KTO == "2142"    ||
         KTO == "2146"    ||
         KTO == "2149"    ||
         KTO == "2339"    ||
         KTO == "23390"   ||
         KTO == "233900"  ||
         KTO == "233901"  ||
         KTO == "233906"  ||
         KTO == "2335"    ||
         KTO == "2399"    ||
         KTO == "247900"  ||
         KTO == "2499"    ||
         KTO == "2510"    ||
         KTO == "25110"   ||
         KTO == "25111"   ||
         KTO == "2520"    ||
         KTO == "2523"    ||
         KTO == "267600"  ||
         KTO == "267700"  ||
         KTO == "3700"    ||
         KTO == "3730"    ||
   //bobesic addition start:
         KTO == "1408"    ||
         KTO == "2406"    ||
         KTO == "6700"    ||
         KTO == "6710"    ||
   //bobesic addition end   
         
   //zorica 2008 addition start:
         KTO == "0641"    ||
         KTO == "06410"    ||
         KTO == "06411"    ||
         KTO == "06412"    ||
         KTO == "06413"    ||
         KTO == "06414"    ||
         KTO == "06415"    ||
         KTO == "06416"    ||
         KTO == "06417"    ||
         KTO == "06418"    ||
         KTO == "06419"    ||
         KTO == "06420"    ||
         KTO == "06421"    ||
         KTO == "06422"    ||
         KTO == "06430"    ||
         KTO == "06431"    ||
         KTO == "06432"    ||
   //zorica 2008 addition end   
         KTO == "3790")

         return(true);
#endif
      #endregion Explicit Legacy Rules

      return (false);
   }

   /*private*/
   public static bool IsSaldaKontiKTO(string KTO)
   {
      if(Kplan.GetIsKontoKupac(KTO) || Kplan.GetIsKontoDobav(KTO)) return true;

      return (false);
   }

   private static string LimitedOpisStr(string data)
   {
      return ZXC.LenLimitedStr(data, ZXC.FtransDao.GetSchemaColumnSize(ZXC.FtrCI.t_opis));
   }

   public static string GetTBRforPsNalog(string origTBR)
   {
      ZXC.VvDataBaseInfo dbInfo = ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage;
      string newTBR;
      int maxTBRlen = 40; // TODO: sznaj iz sheme... 

      if(origTBR.NotEmpty())
      {
         // 29.12.2015: 
         if(origTBR.Length > 6 && char.IsDigit(origTBR[0]) && origTBR[1] == 'p')
         {
            return origTBR;
         }

         if(origTBR.Length <= (maxTBRlen - 2))
         {
            newTBR = dbInfo.ProjectPreviousYear_LastDigit + "p" + origTBR;
         }
         else if(origTBR.Length == (maxTBRlen - 1))
         {
            newTBR = dbInfo.ProjectPreviousYear_LastDigit + origTBR;
         }
         else
         {
            newTBR = dbInfo.ProjectPreviousYear_LastDigit + origTBR.Remove(0, 1);
         }
      }
      else
      {
         newTBR = "";
      }

      return newTBR;
   }

   private static /*void*/Ftrans SendToPsNalog(XSqlConnection conn, string _currKTO, uint _currKCD, string _currTCK, uint _currMCD, string _currMTK, string _currTBR, string _currPrjktCD, DateTime _dokDate, DateTime _valuta, ref ushort _line, decimal dug, decimal pot, string currOpis, bool isSaldaKontiKTO)
   {
      ZXC.VvDataBaseInfo dbInfo = ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage;

      string newTipBr, opis;
      decimal cDug, cPot;

      // !!! 
      newTipBr = GetTBRforPsNalog(_currTBR); // za 'prisiljene' scKta ostavi orig TipBr 
    //newTipBr =                 (_currTBR); // unchanged! 

      // saldoOTS  ______________________________________________________________________________ 
      // 13.01.2016: 
      decimal saldo = dug - pot;
      if(isSaldaKontiKTO == false) // NIJE saldaKontiKTO 
      {
         if(saldo > 0) { cDug = saldo; cPot = 0     ; }
         else          { cDug = 0    ; cPot = -saldo; }
      }
      else // DA, JE saldaKontiKTO 
      {
         //if(Kplan.GetIsKontoKupac(_currKTO)) // KUPAC
         //{
         //   if(saldo > 0) { cDug = saldo; cPot = 0     ; }
         //   else           { cDug = 0     ; cPot = saldo; }
         //}
         //else // DOBAVLJAC 
         //{
         //   if(Kplan.GetIsKontoKupac(_currKTO) && saldo > 0) { cDug = saldo; cPot = 0; }
         //   else { cDug = 0; cPot = -saldo; }
         //}

         cDug = dug;
         cPot = pot;
      }

      // t_opis  ______________________________________________________________________________ 
      if(_currTBR.IsEmpty()) opis = "PS " + dbInfo.ProjectYear;
      // 29.12.2015: 
    //else                                          opis = "RN od " + _dokDate.ToString(ZXC.VvDateFormat);
      else if(currOpis.Contains("RN od ") == false) opis = "RN od " + _dokDate.ToString(ZXC.VvDateFormat); // ako je true, znaci da je racun iz pretPrethodne godine 
      else                                          opis = "";

      if(opis.IsEmpty()) opis = currOpis;
      else               opis = LimitedOpisStr(opis + "/" + currOpis);

      // 15.11.2022: 
      if(ZXC.projectYearAsInt == 2023) // !!! pazi da li se doticna NY operacija (Init_NY, SendToPsNalog, PS_RISK, ...) izvodi u PG ili u NY!!! 
      {
         cDug = ZXC.EURiIzKuna_HRD_(cDug);
         cPot = ZXC.EURiIzKuna_HRD_(cPot);
      }

      //------------------------------------------------------------------------- 
      /* */
      /* */
      Ftrans ftrans_rec = 
      /* */
      /* */  AutoSetNalog(conn, ref _line,
      /* */
      /* */ /*DateTime n_dokDate    */ dbInfo.ProjectYearFirstDay,
      /* */ /*string   n_tt        */ Nalog.PS_TT,
      /* */ /*string   n_napomena   */ "POCETNO STANJE " + dbInfo.ProjectYear,
      /* */
      /* */ /*string   t_konto      */ _currKTO,
      /* */ /*uint     t_kupdob_cd  */ _currKCD,
      /* */ /*string   t_ticker     */ _currTCK,
      /* */ /*uint     t_mtros_cd   */ _currMCD,
      /* */ /*uint     t_mtros_tk   */ _currMTK,
      /* */ /*string   t_tipBr      */ newTipBr,
      /* */ /*string   t_opis       */ opis,
      /* */ /*DateTime t_valuta     */ _valuta,
      /* */ /*string   t_pdv        */ "",
      /* */ /*string   t_037        */ "",
      /* */ /* string   t_projektCD */ _currPrjktCD, // !!! tek od 23.03.2017 
      /* */ /* ushort   t_pdvKnjiga */ ZXC.PdvKnjigaEnum.NIJEDNA,
      /* */ /* uint     t_fakRecID  */ 0,
      /* */ /* OtsKindEnum t_otsKind*/ ZXC.OtsKindEnum.NIJEDNO,
      /* */ /* string   t_fond      */ "",
      /* */ /* string   t_pozicija  */ "",
      /* */ /*decimal  t_dug        */ cDug,
      /* */ /*decimal  t_pot        */ cPot);
      /* */
      //------------------------------------------------------------------------- 

      ftrans_rec.OrigPgTipBr = _currTBR; // !!! 

      return ftrans_rec;
   }

   #endregion PS GlavnaKnjiga + SaldaConti

   #region GetOTS_FtransList

   public static List<Ftrans> GetOTS_FtransByTipBrSortedList(XSqlConnection conn, string konto, uint kupdob_cd, DateTime dateDo)
   {
      List<Ftrans> OtsFtransList = new List<Ftrans>();

      List<VvSqlFilterMember> filterMembers = Set_GetOts_FilterMembers(konto, kupdob_cd, dateDo);

      LoadGenericVvDataRecordList<Ftrans>(conn, OtsFtransList, filterMembers, "ftr", "t_konto, t_kupdob_cd, t_tipBr, t_dokNum, t_serial");

      // 10.10.2012:
    //return OtsFtransList;
    //return ResortedList_ByFormattedTtNum(OtsFtransList);
      return OtsFtransList.OrderBy(ftr => ftr.R_tipBr_Resorted).ToList();
   }

   //private static List<Ftrans> ResortedList_ByFormattedTtNum(List<Ftrans> _OtsFtransList)
   //{
   //   string tt;
   //   uint ttNum;

   //   for(int i = 0; i < _OtsFtransList.Count; ++i)
   //   {
   //      Ftrans.ParseTipBr(_OtsFtransList[i].T_tipBr, out tt, out ttNum);

   //      _OtsFtransList[i].T_tipBr = tt + "-" + ttNum.ToString("000000");
   //   }

   //   return _OtsFtransList.OrderBy(list => list.T_tipBr).ToList();
   //}

   private static List<VvSqlFilterMember> Set_GetOts_FilterMembers(string konto, uint kupdob_cd, DateTime dateDo)
   {
      DataRow drSchema;
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>();

      // For wanted konto only                                                                                                                                            
      drSchema = ZXC.FtransDao.TheSchemaTable.Rows[ZXC.FtransDao.CI.t_konto];

      filterMembers.Add(new VvSqlFilterMember(drSchema, "elKonto", konto, " = "));

      if(kupdob_cd.NotZero())
      {
         // For wantedkupdob_cd only                                                                                                                                            
         drSchema = ZXC.FtransDao.TheSchemaTable.Rows[ZXC.FtransDao.CI.t_kupdob_cd];

         filterMembers.Add(new VvSqlFilterMember(drSchema, "elkupdob_cd", kupdob_cd, " = "));
      }

      // 21.05.2010 da hvata i 'avanse' 
      //if(dateDo != DateTime.MaxValue)
      //{
      //   // t_dokDate untill dokDateDo only                                                                                                                                            
      //   drSchema = ZXC.FtransDao.TheSchemaTable.Rows[ZXC.FtransDao.CI.t_dokDate];
      //   filterMembers.Add(new VvSqlFilterMember(drSchema, "elDateDO", dateDo, " <= "));
      //}



      // SUB Querry filter for OTS balance 

      filterMembers.Add(new VvSqlFilterMember(otsSubQuerry, /*0.00M*/ 0, " != "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_dokDate], true, "dateOTS", dateDo, "", "", "", "")); // za otsSubQuerry value parametra '?prm_dateOTS' 

      return filterMembers;
   }
                                                                                                                                                                                                                                          // AND sub.t_dokDate <= ?prm_dateOTS ---> dodano 15.11.2011 
   private static string otsSubQuerry = "\n(\n   SELECT SUM(sub.t_dug - sub.t_pot) AS saldo\n   FROM ftrans sub FORCE INDEX (BY_OTS)\n   WHERE sub.t_konto = ftr.t_konto AND sub.t_kupdob_cd = ftr.t_kupdob_cd AND sub.t_tipBr = ftr.t_tipBr AND sub.t_dokDate <= ?prm_dateOTS\n)\n";

   #endregion GetOTS_FtransList

   #region Set_IMPORT_OFFIX_Columns

   //  //____ Specifics 2 start ______________________________________________________

   //fprintf(device, "%s\t", nalog_rec[0].n_nalog);
   //fprintf(device, "%s\t", nalog_rec[0].n_tip  );
   //fprintf(device, "%s\t", GetMySqlDate(nalog_rec[0].n_date));
   //fprintf(device, "%s\t", nalog_rec[0].n_napomena);
   //fprintf(device, "%s\t", nalog_rec[0].n_add_uid);
   //fprintf(device, "%s\t", GetMySqlDate(nalog_rec[0].n_date_UnDok));
   //fprintf(device, "%s\t", nalog_rec[0].n_edit_uid);
   //fprintf(device, "%s\t", nalog_rec[0].n_date_edit[0] ? GetMySqlDate(nalog_rec[0].n_date_edit) : GetMySqlDate(nalog_rec[0].n_date_UnDok));
	
   //fprintf(device, "\n");

   //  //____ Specifics 2 end   ______________________________________________________

   public override string Set_IMPORT_OFFIX_Columns()
   {
      return

         "(" + 

         "dokNum, "   + //fprintf(device, "%s\t", nalog_rec[0].n_nalog);
         "tt, "       + //fprintf(device, "%s\t", nalog_rec[0].n_tip  );
         "dokDate, "  + //fprintf(device, "%s\t", GetMySqlDate(nalog_rec[0].n_date));
         "napomena, " + //fprintf(device, "%s\t", nalog_rec[0].n_napomena);
         "addUID, "   + //fprintf(device, "%s\t", nalog_rec[0].n_add_uid);
         "addTS, "    + //fprintf(device, "%s\t", GetMySqlDate(nalog_rec[0].n_date_UnDok));
         "modUID, "   + //fprintf(device, "%s\t", nalog_rec[0].n_edit_uid);
         "modTS"      + //fprintf(device, "%s\t", nalog_rec[0].n_date_edit[0] ? GetMySqlDate(nalog_rec[0].n_date_edit) : GetMySqlDate(nalog_rec[0].n_date_UnDok));

         ")"    + "\n" +

         "SET " + "\n" +

         "recID = dokNum, " + "\n" +
         "ttNum = dokNum  ";
   }

   #endregion Set_IMPORT_OFFIX_Columns

   #region LoadFtransListFor_OtsReport()

   internal static void LoadFtransListFor_OtsReport(XSqlConnection conn, List<Ftrans> TheFtransList, List<VvSqlFilterMember> filterMembers, DateTime dateOTS, bool isOtsKupaca, bool needsKontra, bool isOtsDospOnly, bool isKarticaAndNotOTS)
   {
      #region lokal variablez & bools

      // 06.11.2014: 
    //string kupacKonta = "120, 121, 122";
    //string dobavKonta = "220, 221, 222";
    //KtoShemaDsc KSD = new KtoShemaDsc(ZXC.dscLuiLst_KtoShema);
      KtoShemaDsc KSD = ZXC.KSD;

      string kupacKonta = KSD.Dsc_KupacKontaIOS.TrimEnd(','); // ako bil ostavi zarez na kraju 
      string dobavKonta = KSD.Dsc_DobavKontaIOS.TrimEnd(','); // ako bil ostavi zarez na kraju 

      string wantedKontoSet, kontraKontoSet;

    //if(isOtsKupaca) { wantedKontoSet = "12"; kontraKontoSet = "22"; }
    //else            { wantedKontoSet = "22"; kontraKontoSet = "12"; }
      if(isOtsKupaca) { wantedKontoSet = kupacKonta; kontraKontoSet = dobavKonta; }
      else            { wantedKontoSet = dobavKonta; kontraKontoSet = kupacKonta; }

      bool isOtsAndNotKartica = !isKarticaAndNotOTS;

      string orderBy, anotherJoinClause;

      #endregion lokal variablez & bools

      #region Classic without kontra

      if(needsKontra == false)
      {
         // 12.01.2016: izbaci konto gotovine (npr kto 1203 kod Ducati) iz ovog seta 
         string ktoGotovina = ZXC.KSD.Dsc_IrmKupciCash;
         filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_konto], "ktoGotovina", ktoGotovina, " != ")); 

         filterMembers.Add(new VvSqlFilterMember("SUBSTRING(ftr.t_konto, 1, 3)", "(" + wantedKontoSet + ")", " IN "));
         if(isOtsAndNotKartica)
         {
            filterMembers.Add(new VvSqlFilterMember(otsSubQuerry, 0, " != "));
         }
         filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_dokDate], true, "dateOTS", dateOTS, "", "", "", "")); // za otsSubQuerry value parametra '?prm_dateOTS' 

         if(isKarticaAndNotOTS)
         {
            orderBy = "naziv, t_dokDate, t_tipBr,   t_serial"; // NOTA BENE! Od 10.10.2012 u biti ignororas ovaj sort, vec resortiras u  RptR_OTS.FillRiskReportLists() 
         }
         else
         {
            // 15.02.2012: 
            //orderBy = "naziv, t_tipBr,   t_dokDate, t_serial";
            //orderBy = "naziv, t_dokDate, t_tipBr,   t_serial";

            // 10.10.2012: opet vratio
            orderBy = "naziv, t_tipBr,   t_dokDate, t_serial"; // NOTA BENE! Od 10.10.2012 u biti ignororas ovaj sort, vec resortiras u  RptR_OTS.FillRiskReportLists() 
         }

         anotherJoinClause = "\n\nLEFT JOIN kupdob k ON ftr.t_kupdob_cd = k.kupdobCD\nLEFT JOIN faktur L ON ftr.t_fakRecID = L.RecID\nLEFT JOIN faktEx R ON ftr.t_fakRecID = R.fakturRecID\n";

         LoadGenericVvDataRecordList(conn, TheFtransList, filterMembers, "ftr", orderBy, false, "ftr.*", anotherJoinClause);

         // !!! -------------------------------------------------------------- 
         // !!! -------------------------------------------------------------- 
         // !!! -------------------------------------------------------------- 
         //if(isKarticaAndNotOTS) return; ipak NE. Da dole SetOtsInfo_IOS popuni ftrans_rec.OtsOtvor i ftrans_rec.OtsZatvor 
         // !!! -------------------------------------------------------------- 
         // !!! -------------------------------------------------------------- 
         // !!! -------------------------------------------------------------- 
      }

      #endregion Classic without kontra

      #region needsKontra: GetDistinctKupdobCDInWantedSet(), GetWantedSet With KontraSet if kupdob is mentioned in WantedSet (even if ots doesn't exists in WantedSet show kontraSet)

      else // needsKontra == true 
      {
         string distinctKupdobCD_CommaSeparated_InWantedSet = GetDistinctKupdobCDInWantedSet(conn, filterMembers, wantedKontoSet, dateOTS);

         filterMembers.Add(new VvSqlFilterMember(otsSubQuerry, 0, " != ")); // namjerno u duplo kao zadnji 
         filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_dokDate], true, "dateOTS", dateOTS, "", "", "", "")); // za otsSubQuerry value parametra '?prm_dateOTS' 

         orderBy = "naziv, t_konto DESC, t_tipBr, t_dokDate, t_serial".Replace("DESC", (isOtsKupaca ? "" : "DESC"));

         GetWantedSetWithKontraSet(conn, TheFtransList, filterMembers, wantedKontoSet, kontraKontoSet, orderBy, distinctKupdobCD_CommaSeparated_InWantedSet);
      }

      #endregion needsKontra: GetDistinctKupdobCDInWantedSet(), GetWantedSet With KontraSet if kupdob is mentioned in WantedSet (even if ots doesn't exists in WantedSet show kontraSet)

      #region Perform Additional DataSource Operation

      List<Ftrans> ftransesToRemoveList = null;

      if(isOtsDospOnly) ftransesToRemoveList = new List<Ftrans>();

      foreach(Ftrans ftrans_rec in TheFtransList)
      {
         SetOtsInfo_IOS(ftrans_rec, kontraKontoSet, TheFtransList, dateOTS);

         // 26.4.2011: remarkirano jer kada je karticna kuca onda ufa ide na 1200, ... za sada nemozemo razlikovati korektno od greske, pa necemo javljati poruke upozorenja do daljnjega... 
         //CheckFtrans_Ots(ftrans_rec, isOtsKupaca);

         // 20.05.2014: tembo treba ttNum ... 
         if(ftrans_rec.T_TT == Nalog.KP_TT) 
         {
            Nalog nalog_rec = new Nalog();

            ZXC.NalogDao.SetMe_Record_byRecID(conn, nalog_rec, ftrans_rec.T_parentID, false);

            ftrans_rec.R_ttNum = nalog_rec.TtNum;
         }

         // Remove nedospjele 
         if(isOtsDospOnly && ftrans_rec.OtsZakas.IsNegative()) ftransesToRemoveList.Add(ftrans_rec);
      }

      if(isOtsDospOnly)
      {
         foreach(Ftrans ftransToRemove in ftransesToRemoveList)
         {
            TheFtransList.Remove(ftransToRemove);
         }
      }

      #endregion Perform Additional DataSource Operation

   }

   #region SetOtsInfo metodz

   /// <summary>
   /// Za IOS. NE za Karticu Partnera i Obracun Kamata (po partneru)
   /// </summary>
   /// <param name="ftrans_rec"></param>
   /// <param name="kontraKontoSet"></param>
   /// <param name="theFtransList"></param>
   /// <param name="dateDo"></param>
   public static void SetOtsInfo_IOS(Ftrans ftrans_rec, string kontraKontoSet, List<Ftrans> theFtransList, DateTime dateDo) // Za IOS. NE za Karticu Partnera i Obracun Kamata (po partneru)
   {
      DateTime dateOd;

      if(kontraKontoSet.IsEmpty())                                           ftrans_rec.OtsIsKontra = false;
      else if(ftrans_rec.T_konto.StartsWith(kontraKontoSet.Substring(0, 2))) ftrans_rec.OtsIsKontra = true ;
      else                                                                   ftrans_rec.OtsIsKontra = false;

      // for some imported PS 
      if(ftrans_rec.T_otsKind == ZXC.OtsKindEnum.NIJEDNO && ftrans_rec.T_TT == "PS") ftrans_rec.SetOtsKind();

      if(ftrans_rec.T_otsKind == ZXC.OtsKindEnum.OTVARANJE) // --------------------------------------------------------------------------------- 
      {
         if(ftrans_rec.OtsIsIRA) ftrans_rec.OtsOtvor = ftrans_rec.T_dug - ftrans_rec.T_pot;
         else                    ftrans_rec.OtsOtvor = ftrans_rec.T_pot - ftrans_rec.T_dug;

         ftrans_rec.OtsZatvor = 0.00M;

         dateOd = GetOtsDateOd(ftrans_rec);
      }
      else if(ftrans_rec.T_otsKind == ZXC.OtsKindEnum.ZATVARANJE) // --------------------------------------------------------------------------- 
      {
         if(ftrans_rec.OtsIsIRA) ftrans_rec.OtsZatvor = ftrans_rec.T_pot - ftrans_rec.T_dug;
         else                    ftrans_rec.OtsZatvor = ftrans_rec.T_dug - ftrans_rec.T_pot;

         ftrans_rec.OtsOtvor = 0.00M;

         dateOd = GetOpeningDateOd(ftrans_rec, theFtransList);
      }
      //else throw new Exception(ftrans_rec + " Nije niti otvaranje niti zatvarsnje!?"); // -------------------------------------------------------
      else
      {
         // 27.01.2014: 
       //ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, ftrans_rec + " Nije niti otvaranje niti zatvaranje!?"); // -------------------------------------------------------
         if(ftrans_rec.R_DugPlusPot.NotZero())
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, ftrans_rec + " Nije niti otvaranje niti zatvaranje!?"); // -------------------------------------------------------
         }
         dateOd = dateDo;
      }

      //12.10.2012:
    //ftrans_rec.OtsZakas = (int)((dateDo      - dateOd     ).TotalDays);
      ftrans_rec.OtsZakas = (int)((dateDo.Date - dateOd.Date).TotalDays);

   }

   private static DateTime GetOpeningDateOd(Ftrans ftrans_rec, List<Ftrans> theFtransList)
   {
      // 25.08.2014: 
    //Ftrans ftransOtvaranja_rec = theFtransList.FirstOrDefault(ftr =>  ftr.T_tipBr                                                                       == ftrans_rec.T_tipBr  && ftr.T_otsKind == ZXC.OtsKindEnum.OTVARANJE);
      Ftrans ftransOtvaranja_rec = theFtransList.FirstOrDefault(ftr => (ftr.T_tipBr.IsEmpty() ? (ftr.T_kupdob_cd == ftrans_rec.T_kupdob_cd) : ftr.T_tipBr == ftrans_rec.T_tipBr) && ftr.T_otsKind == ZXC.OtsKindEnum.OTVARANJE);

      if(ftransOtvaranja_rec == null) return GetOtsDateOd(ftrans_rec);
      else                            return GetOtsDateOd(ftransOtvaranja_rec);
   }

   public static DateTime GetOtsDateOd(Ftrans ftrans_rec)
   {
      if(ftrans_rec.T_valuta.Equals(DateTime.MinValue)) return ftrans_rec.T_dokDate; // t_valuta je empty tj "01.01.0001" 
      else                                              return ftrans_rec.T_valuta;
   }

   private static void CheckFtrans_Ots(Ftrans ftrans_rec, bool isOtsKupaca)
   {
      string tt;
      uint   ttNum;

      bool parseOK = ftrans_rec.ParseTipBr(out tt, out ttNum);

      if(parseOK == false) return;

      if(ftrans_rec.OtsIsKontra) return;

      if( isOtsKupaca && ZXC.TtInfo(tt).IsUlazniPdvTT)  ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Nalog\n {0}\n\nTipTransakcije {1} je ULAZNI tip, a konto {2} su kupci!?",       ftrans_rec, tt, ftrans_rec.T_konto);
      if(!isOtsKupaca && ZXC.TtInfo(tt).IsIzlazniPdvTT) ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Nalog\n {0}\n\nTipTransakcije {1} je IZLAZNI tip, a konto {2} su dobavljaèi!?", ftrans_rec, tt, ftrans_rec.T_konto);
   }

   #endregion SetOtsInfo metodz

   private static bool GetWantedSetWithKontraSet(XSqlConnection conn, List<Ftrans> TheFtransList, List<VvSqlFilterMember> filterMembers, string wantedKontoSet, string kontraKontoSet, string orderBy, string kupdobListForKontra)
   {
      bool success = true;
      Ftrans ftrans_rec = new Ftrans();

      ZXC.sqlErrNo = 0;

      using(XSqlCommand cmd = (VvSQL.GetWantedSetWithKontraSet_Command(conn, filterMembers, wantedKontoSet, kontraKontoSet, orderBy, kupdobListForKontra)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               success = reader.HasRows;
               while(success && reader.Read())
               {
                  ftrans_rec = new Ftrans();
                  ZXC.FtransDao.FillFromDataReader(ftrans_rec, reader, true);
                  TheFtransList.Add(ftrans_rec);
               }
               reader.Close();
            }
         }
         catch(XSqlException ex) { success = false; VvSQL.ReportSqlError("GetWantedSetWithKontraSet", ex, System.Windows.Forms.MessageBoxButtons.OK); ZXC.sqlErrNo = ex.Number; }
      } 
      return success;
   }

   private static string GetDistinctKupdobCDInWantedSet(XSqlConnection conn, List<VvSqlFilterMember> filterMembers, string wantedKontoSet, DateTime dateDo)
   {
      System.Text.StringBuilder kupdobCDlist = new System.Text.StringBuilder();

      List<VvSqlFilterMember> filterMembers_DistinctKupdobCD = new List<VvSqlFilterMember>(filterMembers);
      filterMembers_DistinctKupdobCD.Add(new VvSqlFilterMember("SUBSTRING(ftr.t_konto, 1, 3)", "(" + wantedKontoSet + ")", " IN "));
      filterMembers_DistinctKupdobCD.Add(new VvSqlFilterMember(otsSubQuerry,                0,                             " != ")); // namjerno u duplo kao zadnji 
      filterMembers_DistinctKupdobCD.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_dokDate], true, "dateOTS", dateDo, "", "", "", "")); // za otsSubQuerry value parametra '?prm_dateOTS' 

      ZXC.sqlErrNo = 0;

      using(XSqlCommand cmd = (VvSQL.GetDistinctKupdobCDInWantedSet_Command(conn, filterMembers_DistinctKupdobCD)))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
            {
               while(reader.HasRows && reader.Read())
               {
                  kupdobCDlist.Append(reader.GetString(0) + ",");
               }
               reader.Close();
            }
         }
         catch(XSqlException ex) { VvSQL.ReportSqlError("GetDistinctKupdobCDInWantedSet", ex, System.Windows.Forms.MessageBoxButtons.OK); ZXC.sqlErrNo = ex.Number; }
      }

      if(kupdobCDlist.ToString().IsEmpty()) kupdobCDlist.Append("0");

      return "(" + kupdobCDlist.ToString().TrimEnd(',') + ")";
   }

   #endregion LoadFtransListFor_OtsReport()

}
