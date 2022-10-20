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

public sealed class AmortDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static AmortDao instance;

   private AmortDao(XSqlConnection conn, string dbName) : base(dbName, Amort.recordNameArhiva, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static AmortDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new AmortDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableAmort

   public static   uint TableVersionStatic { get { return 3; } }

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
         /* 19 */  "napomena     varchar(80)          NOT NULL default '',\n" +
         /* 10 */  "flagA        tinyint(1) unsigned  NOT NULL default 0,\n" +
         /* 11 */  "dug          DECIMAL(12,2)        NOT NULL DEFAULT 0.00,\n" +
         /* 12 */  "pot          DECIMAL(12,2)        NOT NULL DEFAULT 0.00,\n" +

         CreateTable_ArhivaExtensionDefinition(isArhiva) +

                                "PRIMARY KEY            (recID),\n" +
          (isArhiva ? "" : "UNIQUE ") + "KEY BY_DOKNUM  (dokNum),\n" +
          (isArhiva ? "" : "UNIQUE ") + "KEY BY_DOKDATE (dokDate, dokNum),\n" +
          (isArhiva ? "" : "UNIQUE ") + "KEY BY_TTNUM   (tt, ttNum)\n" +

          (isArhiva ? ", KEY BYqOrigRecID (origRecID)\n" : "")

         );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Amort.recordNameArhiva;
      else         tableName = Amort.recordName;

      switch(catchingVersion)
      {
         case 2: return (isArhiva ? "ADD INDEX BYqOrigRecID (origRecID)\n" : "skip_me");

         case 3: return AlterTable_LanSrvID_And_LanRecID_Columns;

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableAmort

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      Amort amort = (Amort)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, amort.RecID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, amort.AddTS,    TheSchemaTable.Rows[CI.addTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, amort.ModTS,    TheSchemaTable.Rows[CI.modTS]);
         VvSQL.CreateCommandParameter(cmd, preffix, amort.AddUID,   TheSchemaTable.Rows[CI.addUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, amort.ModUID,   TheSchemaTable.Rows[CI.modUID]);
         VvSQL.CreateCommandParameter(cmd, preffix, amort.LanSrvID, TheSchemaTable.Rows[CI.lanSrvID]);
         VvSQL.CreateCommandParameter(cmd, preffix, amort.LanRecID, TheSchemaTable.Rows[CI.lanRecID]);

      /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, amort.DokNum,        TheSchemaTable.Rows[CI.dokNum]);
      /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, amort.DokDate,       TheSchemaTable.Rows[CI.dokDate]);
      /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, amort.TT,            TheSchemaTable.Rows[CI.tt]);
      /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, amort.TtNum,         TheSchemaTable.Rows[CI.ttNum]);
      /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, amort.Napomena,      TheSchemaTable.Rows[CI.napomena]);
      /* 10 */ VvSQL.CreateCommandParameter(cmd, preffix, amort.FlagA,         TheSchemaTable.Rows[CI.flagA]);
      /* 11 */ VvSQL.CreateCommandParameter(cmd, preffix, amort.Dug,           TheSchemaTable.Rows[CI.dug]);
      /* 12 */ VvSQL.CreateCommandParameter(cmd, preffix, amort.Pot,           TheSchemaTable.Rows[CI.pot]);

         if(isArhiva)
         {
            VvSQL.CreateCommandParameter(cmd, preffix, amort.OrigRecID, TheSchemaTable.Rows[CI.origRecID]);
            VvSQL.CreateCommandParameter(cmd, preffix, amort.RecVer,    TheSchemaTable.Rows[CI.recVer]);
            VvSQL.CreateCommandParameter(cmd, preffix, amort.ArAction,  TheSchemaTable.Rows[CI.arAction]);
            VvSQL.CreateCommandParameter(cmd, preffix, amort.ArTS,      TheSchemaTable.Rows[CI.arTS]);
            VvSQL.CreateCommandParameter(cmd, preffix, amort.ArUID,     TheSchemaTable.Rows[CI.arUID]);
         }
      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataRow

   //public void FillFromDataRow(VvDataRecord arhivedDataRecord, Vektor.DataLayer.DS_amort.amortRow amortRow)
   //{
   //   AmortStruct drData = new AmortStruct();

   //   drData._recID = (uint)amortRow.prjktKupdobCD;
   //   drData._addTS = amortRow.addTS;
   //   drData._modTS = amortRow.modTS;
   //   drData._addUID = amortRow.addUID;
   //   drData._modUID = amortRow.modUID;

   //   /* 05 */  drData._dokNum       = amortRow.dokNum;    
   //   /* 06 */  drData._dokVer       = amortRow.dokVer;     
   //   /* 07 */  drData._dokDate      = amortRow.dokDate;    
   //   /* 08 */  drData._tt           = amortRow.tt;      
   //   /* 09 */  drData._ttNum        = amortRow.ttNum;  
   //   /* 10 */  drData._napomena     = amortRow.napomena;   
   //   /* 11 */  drData._flagA        = amortRow.flagA;   
   //   /* 12 */  drData._arAction     = amortRow.arAction;     
   //   /* 13 */  drData._arTS       = amortRow.arTS;  
   //   /* 14 */  drData._arUID        = amortRow.arUID;   
   //   /* 15 */  drData._tipBrVer_old = amortRow.tipBrVer_old;    
   //   /* 16 */  drData._tipBrVer_new = amortRow.tipBrVer_new;    
   //   /* 17 */  drData._dug          = amortRow.dug;    
   //   /* 18 */  drData._pot          = amortRow.pot;    

   //   ((Amort)arhivedDataRecord).CurrentData = drData;

   //   return;
   //}

   #endregion FillFromDataRow

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva)
   {
      AmortStruct rdrData = new AmortStruct();

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
      /* 11 */      rdrData._dug          = reader.GetDecimal  (CI.dug);
      /* 12 */      rdrData._pot          = reader.GetDecimal  (CI.pot);

      ((Amort)vvDataRecord).CurrentData = rdrData;

      if(((Amort)vvDataRecord).Transes != null) ((Amort)vvDataRecord).Transes.Clear();

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
      Amort amort_rec = (Amort)vvDocumentRecord;

      if(amort_rec.Transes == null) amort_rec.Transes = new List<Atrans>();
      else                          amort_rec.Transes.Clear();

      LoadGenericTransesList<Atrans>(conn, amort_rec.Transes, amort_rec.RecID, isArhiva);
   }

   #endregion LoadTranses

   #region AmortCI struct & InitializeSchemaColumnIndexes()

   public struct AmortCI
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

      internal int origRecID;
      internal int recVer;
      internal int arAction;
      internal int arTS;
      internal int arUID;
   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public AmortCI CI;

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

      CI.origRecID      = GetSchemaColumnIndex("origRecID");
      CI.recVer         = GetSchemaColumnIndex("recVer");
      CI.arAction       = GetSchemaColumnIndex("arAction");
      CI.arTS           = GetSchemaColumnIndex("arTS");
      CI.arUID          = GetSchemaColumnIndex("arUID");
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()







   #region AutoAddAmort

   private static uint currDokNum_ForAutoAddAmort;

   private static void AutoAddAmort(XSqlConnection conn, ref ushort line, Amort amort_rec, Atrans atrans_rec)
   {
      IVvDao amortDao  = amort_rec .VvDao;
      IVvDao atransDao = atrans_rec.VvDao;

      ushort linesPerDocumentLIMIT = ushort.MaxValue;

      if(line < linesPerDocumentLIMIT) line++;
      else                             line=1;

      if(line == 1) // new zaglavlje needed 
      {
         line = 1; 

         //amort_rec.VirtualTranses = new List<VvTransRecord>(); // ?? !! bez ovoga zajebava. 

         currDokNum_ForAutoAddAmort = amortDao.GetNextDokNum(conn, Amort.recordName);

         amort_rec.DokNum = currDokNum_ForAutoAddAmort;
         amort_rec.TtNum  = amortDao.GetNextTtNum(conn, amort_rec.VirtualTT, null);


         /* $$$ */ amortDao.ADDREC(conn, amort_rec);

         
         // save new Amort data for further transes 
         ZXC.AmortRec = amort_rec.MakeDeepCopy();

      } // new zaglavlje needed 


      atrans_rec.T_serial   = line;

      atrans_rec.T_parentID = ZXC.AmortRec.RecID;
      atrans_rec.T_dokNum   = ZXC.AmortRec.DokNum;
      atrans_rec.T_dokDate  = ZXC.AmortRec.DokDate;
      atrans_rec.T_TT       = ZXC.AmortRec.TT;

      /* $$$    atransDao.ADDREC(conn, dtrans_rec);*/
      /* $$$ */
      atransDao.ADDREC(conn, atrans_rec, false, false, false, false);

   }

   public static void AutoSetAmort(XSqlConnection conn, ref ushort line, Amort amort_rec, Atrans atrans_rec)
   {
      AutoAddAmort(conn, ref line, amort_rec, atrans_rec);
   }

   public static void AutoSetAmort(
      
      XSqlConnection conn, 
      ref ushort     line, 

      DateTime n_dokDate,
      string   n_tt,
      string   n_napomena,

      string   t_osredCD,
      string   t_opis,
      decimal  t_kol,
      string   t_koef_am,
      decimal  t_amort_st,
      decimal  t_normalAm,
      decimal  t_dug,
      decimal  t_pot)
   {

      Amort  amort_rec  = new Amort();
      Atrans atrans_rec = new Atrans();

      amort_rec.DokDate  = n_dokDate;
      amort_rec.TT       = n_tt;
      amort_rec.Napomena = n_napomena;

      atrans_rec.T_osredCD = t_osredCD;
      atrans_rec.T_opis    = t_opis;
      atrans_rec.T_kol     = t_kol;
      atrans_rec.T_koefAm  = t_koef_am;
      atrans_rec.T_amortSt = t_amort_st;
      atrans_rec.T_normalAm= t_normalAm;
      atrans_rec.T_dug     = t_dug;
      atrans_rec.T_pot     = t_pot;

      AutoAddAmort(conn, ref line, amort_rec, atrans_rec);
   }

   #endregion AutoAddAmort

   #region Amortize

   public static bool Amortize(XSqlConnection dbConnection, DateTime _dateAmort, string _opis, ZXC.AmortRazdoblje _amortRazdoblje)
   {
      ushort line       = 0;
      bool   isNotEmpty = false;

      AmortResults      amortResults;

      List<Osred>       osredList   = ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Osred>(null, VvSQL.SorterType.None);
      List<OsredStatus> osrStatList = OsredDao.GetOsredStatusList(dbConnection, null, _amortRazdoblje, _dateAmort);

      var osredRichList = osredList.Join(osrStatList, o => o.OsredCD, s => s.OsredCD, (o, s) => new { o, s }); // Linq 

      // dobije se isto. Ne kuzim ovo inner / outer sequence razliku 
      // var osredRichList2= osrStatList.Join(osredList, o => o.OsredCD, s => s.OsredCD, (o, s) => new { o, s });

      //foreach(Osred osredRich_rec in osredList)
      foreach(var osredRich_rec in osredRichList)
      {
         if(osredRich_rec.o.IsRashod == true)  continue;
         if(osredRich_rec.s.OldKnjVr.IsZero()) continue;

         amortResults = GetNewAmort(dbConnection, osredRich_rec.o, osredRich_rec.s, _dateAmort); // !!! 

         if(amortResults.justCalculatedNewAmort.IsZero()) continue;

         if(!isNotEmpty) isNotEmpty = true;

         AutoSetAmort(

            /* XSqlConnection conn  */ dbConnection,
            /* ref ushort     line  */ ref line,

            /* DateTime n_dokDate   */ _dateAmort,
            /* string   n_tt        */ Amort.AMORT_TT,
            /* string   n_napomena  */ _opis,

            /* string   t_personCD   */ osredRich_rec.o.OsredCD,
            /* string   t_opis      */ /*"AMORTIZACIJA " + _dateAmort.Year.ToString()*/ _opis,
            // 18.02.2015: 
            // decimal  t_kol       */ 1.00M,
            /* decimal  t_kol       */ osredRich_rec.s.KolSt,
            /* string   t_koef_am   */ osredRich_rec.o.KoefAm.NotEmpty() ? osredRich_rec.o.KoefAm : "1",
            /* decimal  t_amort_st  */ osredRich_rec.o.AmortSt,
            /* decimal  t_amort_st  */ amortResults.justCalculatedNewNormalAmort,
            /* decimal  t_dug       */ 0.00M,
            /* decimal  t_pot       */ amortResults.justCalculatedNewAmort
         );

      }

      return isNotEmpty;
   }

   #region AmortBlock Struct
   
   private struct  AmortResults
   {
      public decimal justCalculatedNewAmort      ;
      public decimal justCalculatedNewNormalAmort;
   }

   private struct  AmortBlock
   {
      public DateTime nab_date;   /* <- */
      public DateTime am_date;    /* <- */
      public string   am_koef;
      public int      nab_month;
      public int      am_month;
      public int      diff_months;
      public decimal  rest2amort;
      public decimal  normalAmort;
      public decimal  am_stopaInUse;  /* actually used */
      public decimal  am_stopaNormal; /* jednostruka */
      public decimal  nab_vr;      /* <- */
      public decimal  am_date_vr;
      public decimal  am_yr_vr;
   }

   #endregion AmortBlock Struct

   private static AmortResults GetNewAmort(XSqlConnection dbConnection, Osred osred_rec, OsredStatus osredStatus, DateTime dateAmort)
   {
      List<Atrans> atransList = OsredDao.GetAtransListForOsred(dbConnection, osred_rec.OsredCD, dateAmort);

      var nabTable = atransList.Where(atr => atr.T_TT == Amort.NABAVA_TT).Select(atr => new { nabDate = atr.T_dokDate, nabVrij = (atr.T_dug - atr.T_pot) }); // Linq

      AmortResults amortResults = new AmortResults();

      if(nabTable.Count().IsZero()) return amortResults;

      // _________________________________________________________________________________________________________ 

      AmortBlock ab = new AmortBlock(); // !!! 

      decimal    ukAmVr = 0.00M, ukNormAmVr = 0.00M, oldSaldo = 0.00M;

      if(osred_rec.KoefAm.NotEmpty())
         ab.am_koef = osred_rec.KoefAm;
      else
         ab.am_koef = "1";
      

      switch(ab.am_koef[0])
      {
         case '2' : ab.am_stopaInUse =   2.00M * osred_rec.AmortSt; break;
         case 'P' : ab.am_stopaInUse = 100.00M * 1.00M            ; break;
         default  : ab.am_stopaInUse =   1.00M * osred_rec.AmortSt; break;
      }

      ab.am_stopaNormal = osred_rec.AmortSt;
      ab.rest2amort     = osredStatus.UkSALDO;
      ab.am_date        = dateAmort;

      foreach(var nabItem in nabTable)
      {
         ab.nab_date = nabItem.nabDate;
         ab.nab_vr   = nabItem.nabVrij;

         CalcMeAmort(ref ab, osredStatus.DateAmRazdobljeStart); // !!! 

         if(ab.diff_months.IsZero())
         {
            if(!((ab.am_koef[0] == 'P') && (ab.nab_month == 13))) continue;
         }

         ukAmVr     += ab.am_date_vr;
         ukNormAmVr += ab.normalAmort;

         ab.am_date_vr = 0.00M;

      } // foreach 

      oldSaldo = osredStatus.UkSALDO;

      if(ukAmVr > oldSaldo) ukAmVr = oldSaldo;

      amortResults.justCalculatedNewAmort       = ukAmVr;
      amortResults.justCalculatedNewNormalAmort = ukNormAmVr;

      return amortResults;
   }

   private static void CalcMeAmort(ref AmortBlock abp, DateTime date_pg)
   {
      int  calc_adjustment = 1; /* Razlika nab 0101 ili pg 0101 */
      
      if(abp.nab_date < date_pg)  {  /* nabavljeno prije godine ove amortizacije*/
         abp.nab_date    = date_pg;
         calc_adjustment = 0;
      }
      
      abp.am_month    = abp.am_date .Month ;
      abp.nab_month   = abp.nab_date.Month + calc_adjustment;
      abp.diff_months = abp.am_month - abp.nab_month + 1;

      if(abp.am_koef[0] == 'P') {

         abp.am_yr_vr   = abp.nab_vr * (abp.am_stopaInUse/100.00M);
         abp.am_date_vr = abp.rest2amort;
      }
      else {
         if(abp.nab_month > 12 || abp.diff_months <= 0)  { /* ?! */
            abp.am_yr_vr = abp.am_date_vr = 0.00M;
            abp.diff_months = 0;
         }
         else  {
            abp.am_yr_vr   = abp.nab_vr * (abp.am_stopaInUse/100.00M);
            abp.am_date_vr = abp.am_yr_vr / 12.00M * (decimal)(abp.diff_months);
         }
      }

      /*_________ ovo dole jos jemput za normalStopaAmort _________*/

      if(abp.nab_month > 12 || abp.diff_months <= 0)  { /* ?! */
         abp.am_yr_vr = abp.normalAmort = 0.00M;
         abp.diff_months = 0;
      }
      else  {
         abp.am_yr_vr    = abp.nab_vr * (abp.am_stopaNormal/100.00M);
         abp.normalAmort = abp.am_yr_vr / 12.00M * (decimal)(abp.diff_months);
      }
      /*----------------------------------------------------------------*/

      return /*(abp.am_yr_vr)*/;
   }

   #endregion Amortize

   #region Inventurize

   public static bool Inventurize(XSqlConnection dbConnection, DateTime _dateInventura, string _opis)
   {
      ushort line       = 0;
      bool   isNotEmpty = false;

      List<Osred>       osredList   = ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Osred>(null, VvSQL.SorterType.None);
      List<OsredStatus> osrStatList = OsredDao.GetOsredStatusList(dbConnection, null, ZXC.AmortRazdoblje.GODINA, _dateInventura);

      var osredRichList = osredList.Join(osrStatList, o => o.OsredCD, s => s.OsredCD, (o, s) => new { o, s }); // Linq 

      // dobije se isto. Ne kuzim ovo inner / outer sequence razliku 
      // var osredRichList2= osrStatList.Join(osredList, o => o.OsredCD, s => s.OsredCD, (o, s) => new { o, s });

      InventurnoStanjeDLG dlgS = new InventurnoStanjeDLG();

      foreach(var osredRich_rec in osredRichList)
      {
         if(osredRich_rec.s.IsRashodovan == true)  continue;
         //if(osredRich_rec.s.OldKnjVr.IsZero()) continue;

         //----------------------------------------------------------------------------- 
         dlgS.Fld_OsredCd   = osredRich_rec.o.OsredCD;
         dlgS.Fld_Naziv     = osredRich_rec.o.Naziv;
         dlgS.Fld_InvStanje = osredRich_rec.s.KolSt;

         if(dlgS.ShowDialog() != System.Windows.Forms.DialogResult.OK) continue;
         //----------------------------------------------------------------------------- 

         if(!isNotEmpty) isNotEmpty = true;

         AutoSetAmort(

            /* XSqlConnection conn  */ dbConnection,
            /* ref ushort     line  */ ref line,

            /* DateTime n_dokDate   */ _dateInventura,
            /* string   n_tt        */ Amort.INVENT_TT,
            /* string   n_napomena  */ _opis,

            /* string   t_personCD   */ osredRich_rec.o.OsredCD,
            /* string   t_opis      */ /*"AMORTIZACIJA " + _dateAmort.Year.ToString()*/ _opis,
            /* decimal  t_kol       */ dlgS.Fld_InvStanje,
            /* string   t_koef_am   */ osredRich_rec.o.KoefAm.NotEmpty() ? osredRich_rec.o.KoefAm : "1",
            /* decimal  t_amort_st  */ osredRich_rec.o.AmortSt,
            /* decimal  t_          */ 0.00M,
            /* decimal  t_dug       */ osredRich_rec.s.UkDugS,
            /* decimal  t_pot       */ osredRich_rec.s.UkPotS
         );

         ZXC.TheVvForm.PutFieldsActions(dbConnection, ZXC.AmortRec, ZXC.TheVvForm.TheVvRecordUC);

      } // foreach(var osredRich_rec in osredRichList) 

      dlgS.Dispose();

      return isNotEmpty;
   }

   #endregion Inventurize

}
