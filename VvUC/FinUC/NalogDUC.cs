using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;


public abstract class NalogDUC : VvDocumentRecordUC
{
   #region Constructor
   
   protected KtoShemaDsc KSD ;

   public NalogDUC(Control parent, Nalog _nalog, VvForm.VvSubModul vvSubModul)
   {
      nalog_rec = _nalog;

    //KSD = new KtoShemaDsc(ZXC.dscLuiLst_KtoShema);
      KSD = ZXC.KSD;

      this.IsShowingConvertedMoney = false;
      this.ValutaNameInUse         = ZXC.ValutaNameEnum.EMPTY;

      ThePrefferedRecordSorter = VirtualDataRecord.DefaultSorter;

      this.TheSubModul = vvSubModul;

      SuspendLayout();

      TheTabControl.TabPages.Add(CreateVvInnerTabPages("Nalog", "", ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
  //    CreateNalogPrintTemplateUC();

      InitializeZaglavljeHamper(out zaglavljeHamper);
     // nextX = zaglavljeHamper.Right;

      nextY = zaglavljeHamper.Bottom + ZXC.QunMrgn;
      nextX = ZXC.QunMrgn;
      TheG               = CreateVvDataGridView  (TheTabControl.TabPages[0], "Ftrans Grid");
      TheColChooserGrid  = CreateColChooserGrid_1(TheG, this, "ColChooser Grid1");
      TheSumGrid         = CreateSumGrid         (TheG, TheTabControl.TabPages[0], "SUM Ftrans Grid");

      InitializeTheGrid_Columns();
      InitializeTheChBoxGrid_Columns1(TheG);
      InitializeTheSUMGrid_Columns(TheG);

      TheG.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged_SumGrid);
     // TheG.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged_ChBoxGrid);


      InitializeVvUserControl(parent);


      CalcLocationSizeAnchor_TheDGV_ChoosGrid(TheG, nextX, nextY, zaglavljeHamper, false);
     // CalcLocationSizeAnchor_GridSum(TheG);

      ResumeLayout();
      
      SetNalogColumnIndexes();

      CreateNalogPrintTemplateUC(this);

      #region Some Testis

      // some testis for fuse: 
      // vidi jos nize primjer za 'UpdateSomething_MethodPointer' 

      //VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Kupdob>(Kupdob.tableName, UpdateSomething_MethodPointer);


      /* */
      /* */
      /* */


      // more testis:

      //ushort line = 0;
      //for(int i = 0; i < 10; ++i)
      //{
      //   NalogDao.AutoSetNalog(TheVvTabPage.conn, ref line,
      //      new DateTime(1997, 8,26),
      //      "PS",
      //      "AutoSetNalog TESTIS",
      //      "1201" + ((ushort)(i + 1)).ToString(),
      //      1040,
      //      /*"TICKER"*/ ZXC.KupdobDao.GetTickerForRecID(TheVvTabPage.conn, 1040),
      //      0,
      //      "A0001234",
      //      "Odlicno knjizenje " + ((ushort)(i+1)).ToString(),
      //      new DateTime(2004, 2, 20),
      //      "", "",
      //      123.45M,
      //      543.21M);
      //}

      #endregion Some Testis

      this.Validating += new CancelEventHandler(NalogDUC_Validating);

      TheG.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(TheG_CellMouseDoubleClick_OpenFakturDUC);

      // --- !!! --- 
      SetSifrarAndAutocomplete<Kplan>(null, VvSQL.SorterType.None);
      SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.None);
      SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.None);

      FakturDao.Fak2Nal_CheckAndPrebaciIfNeeded(TheDbConnection);
   }

   private void TheG_CellMouseDoubleClick_OpenFakturDUC(object sender, DataGridViewCellMouseEventArgs e)
   {
      int rowIdx = e.RowIndex;

      if(rowIdx.IsNegative()) return;

      if(e.ColumnIndex != ci.iT_kupdob_cd &&
         e.ColumnIndex != ci.iT_kupdob_name && 
         e.ColumnIndex != ci.iT_ticker && 
         e.ColumnIndex != ci.iT_tipBr && 
         e.ColumnIndex != ci.iT_valuta) return;

      uint   fakturRecID = TheG.GetUint32Cell(ci.iT_fakRecID, rowIdx, false);
      uint   fakturYear  = TheG.GetUint32Cell(ci.iT_fakYear , rowIdx, false);
      string tipBr       = TheG.GetStringCell(ci.iT_tipBr   , rowIdx, false);

      if(fakturRecID.IsZero()) return;

      if(fakturYear != ZXC.projectYearAsInt) { ZXC.aim_emsg(MessageBoxIcon.Stop, "Racun nije iz ove godine"); return; }

      Faktur faktur_rec = new Faktur();
      bool fakturOK = ZXC.FakturDao.SetMe_Record_byRecID(TheDbConnection, faktur_rec, fakturRecID, false);

      if(fakturOK)
      {
         string tt;
         uint   ttNum;

         bool parseOK = Ftrans.ParseTipBr(tipBr, out tt, out ttNum);

         if(parseOK == false) return;

         ZXC.VvSubModulEnum vvSubModulEnum = FakturDUC.GetVvSubModulEnum_ForTT(tt);

         if(vvSubModulEnum == ZXC.VvSubModulEnum.UNDEF) return;

         TheVvTabPage.TheVvForm.OpenNew_Record_TabPage_wInitialRecord(TheVvTabPage.TheVvForm.GetSubModulXY(vvSubModulEnum), faktur_rec);
      }
   }

   void NalogDUC_Validating(object sender, CancelEventArgs e)
   {
      #region Should validate enivej?

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None ||
         TheVvTabPage.WriteMode == ZXC.WriteMode.Delete ||
         this.Visible == false) return;

      GetFields(false);

      #endregion Should validate enivej?

      #region IsDocumentFromLockedPeriod

      // 09.02.2016: 

      if(VvDaoBase.IsDocumentFromLockedPeriod(Fld_DokDate.Date, false)) e.Cancel = true;

      #endregion IsDocumentFromLockedPeriod

      #region Check Saldo Balance

      // 16.10.2015: 
    //decimal ukDug = nalog_rec.Transes          .Sum(trn => trn.T_dug);
    //decimal ukPot = nalog_rec.Transes          .Sum(trn => trn.T_pot);
    //decimal ukDug = nalog_rec.TransesNonDeleted.Sum(trn => trn.T_dug);
    //decimal ukPot = nalog_rec.TransesNonDeleted.Sum(trn => trn.T_pot);
    //
    //decimal saldo = (ukDug - ukPot).Ron2();

      if(nalog_rec.Saldo.NotZero())
      {
         DialogResult result = MessageBox.Show("Knjiženja nisu u ravnoteži.\n\nSaldo: " + nalog_rec.Saldo.ToStringVv() + "\n\nŽelite li zaista usnimiti ovaj nalog?", "Potvrdite usnimavanje NEURAVNOTEŽENOG naloga?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

         if(result != DialogResult.Yes) e.Cancel = true;
      }

      #endregion Check Saldo Balance

      #region Check Column Konto

      SetSifrarAndAutocomplete<Kplan>(null, VvSQL.SorterType.Konto);

    //foreach(Ftrans ftrans_rec in nalog_rec.Transes)
      foreach(Ftrans ftrans_rec in nalog_rec.TransesNonDeleted)
      {
         if(KplanSifrar.Select(kplan => kplan.Konto).Contains(ftrans_rec.T_konto) == false)
         {
            DialogResult result = MessageBox.Show("Konto ne postoji.\n\n(Redak: " + ftrans_rec.T_serial + " Konto: " + ftrans_rec.T_konto + "\n\nŽelite li zaista usnimiti ovaj nalog?", "Potvrdite usnimavanje naloga?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if(result != DialogResult.Yes) e.Cancel = true;
         }
         else
         {
            string theTip = KplanSifrar.Single(kplan => kplan.Konto == ftrans_rec.T_konto).Tip;
            if(theTip != "A")
            {
               DialogResult result = MessageBox.Show("Tip konta: " + theTip + " nedozvoljen.\n\n(Redak: " + ftrans_rec.T_serial + " Konto: " + ftrans_rec.T_konto + "\n\nŽelite li zaista usnimiti ovaj nalog?", "Potvrdite usnimavanje naloga?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

               if(result != DialogResult.Yes) e.Cancel = true;
            }
         }

         // 31.08.2015: 
         if(Is_T_Pozicija_Empty(ftrans_rec.T_pozicija, ftrans_rec.T_konto, ZXC.CURR_prjkt_rec.PlanKind, ftrans_rec.T_opis))
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Redak {0}\n\nPozicija ne smije biti prazna.\n\nKonto [{1}]", ftrans_rec.T_serial, ftrans_rec.T_konto);
            e.Cancel = true;
         }

         #region 2018 Kerempuh oce strogo

         if(ZXC.KSD.Dsc_isExtValidation) // TODO 
         {
            string kto = ftrans_rec.T_konto;

            // 1. konta 3-8 moraju imati mjesto troška,
            if(kto.StartsWith("3") || kto.StartsWith("4") || kto.StartsWith("5") || kto.StartsWith("6") || kto.StartsWith("7") || kto.StartsWith("8"))
            {
               if(ftrans_rec.T_mtros_cd.IsZero()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Redak {0}\n\nMjesto troška ne smije biti prazno.\n\nKonto [{1}]", ftrans_rec.T_serial, ftrans_rec.T_konto);  e.Cancel = true; }
            }

            // 2. Saldakonti konta moraju imati partnera a možda i ttBr 
            if(NalogDao.IsSaldaKontiKTO(kto) && (ftrans_rec.T_kupdob_cd.IsZero() || ftrans_rec.T_tipBr.IsEmpty())) { ZXC.aim_emsg(MessageBoxIcon.Error, "Redak {0}\n\nPopuniti i partnera i broj raèuna.\n\nKonto [{1}]", ftrans_rec.T_serial, ftrans_rec.T_konto); e.Cancel = true; }

            // 3. Ne smije otiæi u novi redak ako je ostavila prazno dug ili pot, 
            if(ftrans_rec.T_dug.IsZero() && ftrans_rec.T_pot.IsZero()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Redak {0}\n\nDopuniti ili duguje ili potražuje.\n\nKonto [{1}]", ftrans_rec.T_serial, ftrans_rec.T_konto); e.Cancel = true; }
         }

         #endregion 2018 Kerempuh oce strogo
      }

      #endregion Check Column Konto

      #region IsShowingConvertedMoney

      if(IsShowingConvertedMoney)
      {
         TheVvTabPage.TheVvForm.RISK_ToggleKnDeviza(null, EventArgs.Empty);
      }

      // if still... 
      if(IsShowingConvertedMoney)
      {
       //ZXC.aim_emsg(MessageBoxIcon.Error, "Prije usnimavanja preracunajte valutu u kune.");
         ZXC.aim_emsg(MessageBoxIcon.Error, "Prije usnimavanja preracunajte valutu u eure.");
         e.Cancel = true;
      }

      #endregion IsShowingConvertedMoney

      #region Check DokDate, SkladDate

      if(Fld_DokDate != DateTimePicker.MinimumDateTime && Fld_DokDate.Date < ZXC.projectYearFirstDay) ZXC.aim_emsg(MessageBoxIcon.Warning, "Datum dokumenta: {0} je stariji od prvog dana u radnoj godini!?", Fld_DokDate.ToString(ZXC.VvDateFormat));
      if(Fld_DokDate != DateTimePicker.MinimumDateTime && Fld_DokDate.Date > DateTime.Now.Date      ) ZXC.aim_emsg(MessageBoxIcon.Warning, "Datum dokumenta: {0} je iz buduænosti!?", Fld_DokDate.ToString(ZXC.VvDateFormat));

      #endregion Check DokDate, SkladDate

   }

   public static string ZatvaranjeKlaseOpis = "ZATVARANJE KL.";

   public static bool Is_T_Pozicija_Empty(string pozicija, string konto, ZXC.PlanKindEnum planKind, string opis)
   {
      // 02.03.2017: 
      if(opis.StartsWith(ZatvaranjeKlaseOpis)) return false;

      if(planKind == ZXC.PlanKindEnum.PlnBy_FOND &&
         (konto.StartsWith("3") ||
          konto.StartsWith("4") ||
          konto.StartsWith("6"))) // Kerempuh 
      {
         if(pozicija.IsEmpty())
         {
            return true;
         }
      }

      if(planKind == ZXC.PlanKindEnum.PlnBy_POZICIJA &&
         (konto.StartsWith("3") ||
          konto.StartsWith("4"))) // TURZML 
      {
         if(pozicija.IsEmpty())
         {
            return true;
         }
      }

      return false;
   }

   public static bool Is_T_Pozicija_MissingInLuiList(string pozicija, string konto, ZXC.PlanKindEnum planKind, string opis)
   {
      // 02.03.2017: 
      if(opis.StartsWith(ZatvaranjeKlaseOpis)) return false;

      if(planKind == ZXC.PlanKindEnum.PlnBy_FOND &&
         (konto.StartsWith("3") ||
          konto.StartsWith("4") ||
          konto.StartsWith("6"))) // Kerempuh 
      {
         VvLookUpItem lui = ZXC.luiListaPozicijePlana.GetLuiForThisCd(pozicija);
         if(lui == null)
         {
            return true;
         }
      }

      if(planKind == ZXC.PlanKindEnum.PlnBy_POZICIJA &&
         (konto.StartsWith("3") ||
          konto.StartsWith("4"))) // TURZML 
      {
         VvLookUpItem lui = ZXC.luiListaPozicijePlana.GetLuiForThisCd(pozicija);
         if(lui == null)
         {
            return true;
         }
      }

      return false;
   }

   public static bool Is_T_Pozicija_KontraOfKontoType(string pozicija, string konto, ZXC.PlanKindEnum planKind)
   {
      if(planKind == ZXC.PlanKindEnum.PlnBy_FOND &&
         (konto.StartsWith("3") ||
          konto.StartsWith("4") ||
          konto.StartsWith("6"))) // Kerempuh 
      {
         if(konto.StartsWith("3") || konto.StartsWith("4")) // should be RASHOD
         {
            if(pozicija.StartsWith("P")) return true;
         }
         else if(konto.StartsWith("6")) // should be PRIHOD
         {
            if(pozicija.StartsWith("R")) return true;
         }
      }

      if(planKind == ZXC.PlanKindEnum.PlnBy_POZICIJA &&
         (konto.StartsWith("3") ||
          konto.StartsWith("4"))) // TURZML 
      {
         if(konto.StartsWith("4")) // should be RASHOD
         {
            if(pozicija.StartsWith("P")) return true;
         }
         else if(konto.StartsWith("3")) // should be PRIHOD
         {
            if(pozicija.StartsWith("R")) return true;
         }
      }

      return false;
   }

   //static bool UpdateSomething(VvDataRecord vvDataRecord)
   //{
   //   bool OK = true;
   //   string naziv;
   //   Kupdob artikl_rec = vvDataRecord as Kupdob;

   //   artikl_rec.BeginEdit();

   //   //artikl_rec.Ticker = artikl_rec.RecID.ToString("000000");
   //   //artikl_rec.Ticker = "";
   //   //artikl_rec.Ticker = String.Format("{0,-6}", artikl_rec.Naziv);
   //   naziv = artikl_rec.Naziv;
   //   artikl_rec.Ticker = String.Format("{0}", naziv.Length > 6 ? naziv.Remove(6) : naziv);

   //   Console.WriteLine("[{0,6}] [{1}] [{2}]", artikl_rec.KupdobCD/*RecID*/, artikl_rec.Ticker, artikl_rec.Naziv);

   //   OK = vvDataRecord.VvDao.RWTREC(ZXC.TheVvForm.conn, vvDataRecord, false, false);

   //   artikl_rec.EndEdit();

   //   return OK;
   //}

   #endregion Constructor

   #region Overriders and specifics

   public Nalog  nalog_rec;
   private Ftrans dgvFtrans_rec;

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.nalog_rec; }
      set {        this.nalog_rec = (Nalog)value; }
   }

   public override VvDocumentRecord VirtualDocumentRecord
   {
      get { return this.VirtualDataRecord as VvDocumentRecord; }
      set {        this.VirtualDataRecord = (Nalog)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.NalogDao; }
   }

   public override VvDaoBase TheVvDaoTrans
   {
      get { return ZXC.FtransDao; }
   }

   public VvHamper zaglavljeHamper;

   private VvTextBox tbx_brNaloga, tbx_datum, tbx_napomena, tbx_vrstaKnj,
                     tbx_VrKnjizOpis, tbx_VKnum, tbx_ValName;

   private VvDateTimePicker dTP_datum;
   private int nextX = 0, nextY = 0, razmakHamp = ZXC.Qun10;

   private VvTextBox /*vvtbT_serial,*/ vvtbT_konto, vvtbT_opis, vvtbT_kupdob_cd,
                     vvtbT_ticker, vvtbT_tipBr, vvtbT_dug, vvtbT_pot,
                     vvtbT_037   , vvtbT_pdv  , vvtbT_mtros_cd, vvtbT_mtros_tk,
                     vvtbT_kupdobName, vvtbT_pdvKnjiga, vvtbT_projektCD, vvtbT_pozicija, vvtbT_pozicijaName, vvtbT_fond, vvtbT_fondName, vvtbT_progAktiv;

   private VvTextBoxColumn colVvText;
   private VvDateTimePickerColumn colDate;
   private DataGridViewTextBoxColumn colScrol;


   #region Document_Database_ColumnIndexes, Transes_Database_ColumnIndexes

   /// <summary>
   /// Column Index U DataBase (NE na DataGridView-u to ti je 'ci')
   /// </summary>
   private NalogDao.NalogCI DB_ci
   {
      get { return ZXC.NalCI; }
   }

   /// <summary>
   /// Column Index u TRANS DataBase (NE na DataGridView-u to ti je 'ci')
   /// </summary>
   private FtransDao.FtransCI DB_Tci
   {
      get { return ZXC.FtrCI; }
   }

   #endregion Document_Database_ColumnIndexes, Transes_Database_ColumnIndexes

   #region Update_VvDataRecord (Legacy naming convention)

   public override VvFindDialog CreateVvFindDialog()
   {
      return CreateFind_Nalog_Dialog();
   }

   public static VvFindDialog CreateFind_Nalog_Dialog()
   {
      VvForm.VvSubModul vvSubModul   = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsNAL);
      VvDataRecord      vvDataRecord = ZXC.TheVvForm.CreateTheVvDataRecord_SwitchSubModulEnum(vvSubModul);

      VvFindDialog vvFindDialog = new VvFindDialog();

      VvRecLstUC vvRecListUC = new NalogListUC(vvFindDialog, (Nalog)vvDataRecord, vvSubModul);

      vvFindDialog.TheRecListUC = vvRecListUC;

      return vvFindDialog;
   }

   #endregion Update_VvDataRecord (Legacy naming convention)

   #region PrintDocumentRecord

   public NalogTemplateFilterUC TheNalogTemplateFilterUC { get; set; }
   public NalogTemplateFilter   TheNalogTemplateFilter   { get; set; }

   //public RptF_NalogDocument TheRptF_NalogDocument
   //{
   //   get;
   //   set;
   //}

   //public override VvReport VirtualReport
   //{
   //   get
   //   {
   //      return this.TheRptF_NalogDocument;
   //   }
   //}

   public override string VirtualReportName
   {
      get
      {
         return "NALOG ZA KNJIŽENJE";
      }
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      NalogTemplateFilter nalogTemplateFilter = (NalogTemplateFilter)vvRptFilter;

      switch(nalogTemplateFilter.NalogTemplateID)
      {
         case NalogTemplateFilter.PrintTemplateID.Firma  : return new RptF_NalogDocument        (reportName, (NalogTemplateFilter)vvRptFilter);
         case NalogTemplateFilter.PrintTemplateID.Obrt   : return new RptF_NalogDocument_Obrt   (reportName, (NalogTemplateFilter)vvRptFilter);
         case NalogTemplateFilter.PrintTemplateID.MjTr   : return new RptF_NalogDocument_MjTr   (reportName, (NalogTemplateFilter)vvRptFilter);
         case NalogTemplateFilter.PrintTemplateID.Projekt: return new RptF_NalogDocument_Projekt(reportName, (NalogTemplateFilter)vvRptFilter);

         default: ZXC.aim_emsg("{0}\n\nNalogTemplateID <{1}> undone!", ZXC.GetMethodNameDaStack(), nalogTemplateFilter.NalogTemplateID); return null;
      }
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheNalogTemplateFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheNalogTemplateFilterUC;
      }
   }
                
   // ovi defaultovi se odnose na defaultove pojedinih DUC-eva (Firma, Obrt, Mtr, ...),                   
   // default-ove za SVE NalogDUC-eve rjesavas sa 'SetDefaultFilterValues()' na NalogTemplateFilter klasi 
   public override void SetFilterRecordDependentDefaults()
   {
      TheNalogTemplateFilter.DokNumOd = nalog_rec.DokNum;

      SetFilterUcDependentDefaults();
   }

   protected virtual void SetFilterUcDependentDefaults()
   {
   }

   #endregion PrintDocumentRecord

   public override VvLookUpLista TheTtLookUpList
   {
      get { return ZXC.luiListaNalogTT; }
   }


   #endregion Overriders and specifics

   #region ZaglavljeHamper

   private void InitializeZaglavljeHamper(out VvHamper zaglavljeHamper)
   {
      zaglavljeHamper = new VvHamper(11, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                               0              1                   2                  3                4              5                6              7             8               9             10     
      zaglavljeHamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un - ZXC.Qun4, ZXC.Q3un -ZXC.Qun2, ZXC.Q4un + ZXC.Qun4, ZXC.Q5un, ZXC.Q2un + ZXC.Qun4, ZXC.Q6un, ZXC.Q2un + ZXC.Qun4, ZXC.Q3un, ZXC.Q3un - ZXC.Qun4, ZXC.QUN  };
      zaglavljeHamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun4,           ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4 };
      zaglavljeHamper.VvRightMargin = zaglavljeHamper.VvLeftMargin;

      //int width = 792 - nextX - zaglavljeHamper.Width - zaglavljeHamper.VvRightMargin - zaglavljeHamper.VvLeftMargin; // 22.11.2010. ode u minus !!!
      //zaglavljeHamper.VvColWdt[8] = width; // zbog poradi scrolla koji se pojavljije na datagridu

      zaglavljeHamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN };
      zaglavljeHamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun2 };
      zaglavljeHamper.VvBottomMargin = zaglavljeHamper.VvTopMargin;

      zaglavljeHamper.CreateVvLabel(0, 0, "Broj naloga:"    , ContentAlignment.MiddleRight);
      zaglavljeHamper.CreateVvLabel(2, 0, "Datum:"          , ContentAlignment.MiddleRight);
      zaglavljeHamper.CreateVvLabel(4, 0, "Vrsta knjiženja:", ContentAlignment.MiddleRight);
      zaglavljeHamper.CreateVvLabel(0, 1, "Napomena:"       , ContentAlignment.MiddleRight);

      tbx_brNaloga = zaglavljeHamper.CreateVvTextBox(1, 0, "tbx_brNaloga", "Ovo bolje ostavi kako je...", 6);
      tbx_brNaloga.JAM_CharEdits     = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_brNaloga.JAM_FillCharacter = '0';
      tbx_brNaloga.JAM_DataRequired  = true;

      this.ControlForInitialFocus = tbx_brNaloga;
      tbx_brNaloga.JAM_StatusText = "Redni broj naloga";

      tbx_datum                         = zaglavljeHamper.CreateVvTextBox(3, 0, "tbx_datum", "Datum");
      tbx_datum.JAM_IsForDateTimePicker = true;
      dTP_datum = zaglavljeHamper.CreateVvDateTimePicker(3, 0, "", tbx_datum);
      dTP_datum.Name      = "dTP_datum";
      dTP_datum.ValueChanged += new EventHandler(dtp_DokDate_ValueChanged_CheckYear);

      tbx_vrstaKnj = zaglavljeHamper.CreateVvTextBoxLookUp(5, 0, "tbx_vrstaKnj", "Tip transakcije - vrsta knjiženja");
      tbx_vrstaKnj.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_vrstaKnj.JAM_DataRequired = true;
      tbx_vrstaKnj.JAM_MustTabOutBeforeSubmit = true;

      tbx_VrKnjizOpis              = zaglavljeHamper.CreateVvTextBox(6, 0, "tbx_VKOpis_InVisible", "");
      tbx_VrKnjizOpis.JAM_ReadOnly = true;

      tbx_VKnum                   = zaglavljeHamper.CreateVvTextBox(7, 0, "tbx_TtNum", "Redni broj po Vrsti knjiženja", 4);
      //tbx_VKnum.JAM_StatusText    = "Ovo bolje ostavi kako je...";
      tbx_VKnum.JAM_CharEdits     = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_VKnum.JAM_FillCharacter = '0';
      tbx_VKnum.JAM_DataRequired  = true;

      tbx_vrstaKnj.JAM_Set_LookUpTable(ZXC.luiListaNalogTT, (int)ZXC.Kolona.prva);
      tbx_vrstaKnj.JAM_lui_NameTaker_JAM_Name = tbx_VrKnjizOpis.JAM_Name;
      tbx_vrstaKnj.JAM_ttNumTaker_JAM_Name    = tbx_VKnum.JAM_Name;

      tbx_napomena = zaglavljeHamper.CreateVvTextBox(1, 1, "tbx_napomena", "Opis knjiženja / napomena.", /*80*/ GetDB_ColumnSize(DB_ci.napomena), 8, 0);

                    zaglavljeHamper.CreateVvLabel(8, 0, "IzvVal:", ContentAlignment.MiddleRight);
      tbx_ValName = zaglavljeHamper.CreateVvTextBoxLookUp(9, 0, "tbx_ValName", "Naziv devizne valute", GetDB_ColumnSize(DB_ci.devName));
      tbx_ValName.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_ValName.JAM_Set_LookUpTable(ZXC.luiListaDeviza, (int)ZXC.Kolona.prva);
      // Nota bene; buduci je tbx_DevName VvLookUp TextBox onde mu je tbx_tt.JAM_FieldExitMethod 'potrosena' lookUp methodom. 

      tbx_ValName.JAM_FieldExitMethod_2 = new EventHandler(OnExit_DevName_SetValutaNameInUse);


      //ne sljkaka:
      //ToolTip tt = new ToolTip();
      //tt.SetToolTip(tbx_napomena, tbx_napomena.Text);
      //tt.ShowAlways = true; mozda zbog ovoga i prosljaka

      //tbx_napomena.JAM_Highlighted = true;
      //tbx_napomena.JAM_ReverseVideo = true;
      //VvTextBox.Klone(ref tbx_napomena, tbx_brNaloga);
      //tbx_napomena.JAM_FieldExitWithValidationMethod = new CancelEventHandler(TestingFldValidation);
   }
   private void OnExit_DevName_SetValutaNameInUse(object sender, EventArgs e)
   {
      VvTextBox vvtb = sender as VvTextBox;

      if(vvtb.EditedHasChanges() == false) return;

      if(Fld_DevNameAsEnum == ZXC.ValutaNameEnum.EMPTY || Fld_DevNameAsEnum == /*ZXC.ValutaNameEnum.HRK*/ZXC.EURorHRK_NameEnum) IsShowingConvertedMoney = false;
      else                                                                                                                      IsShowingConvertedMoney = true;

      ValutaNameInUse = Fld_DevNameAsEnum;

      TheVvTabPage.TheVvForm.ToggleDevizaVisualApperiance(IsShowingConvertedMoney, Fld_DevName, Fld_DevTecaj);

   }

   private void dtp_DokDate_ValueChanged_CheckYear(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      if(Fld_DokDate != DateTimePicker.MinimumDateTime && Fld_DokDate.Year != ZXC.projectYearFirstDay.Year) ZXC.aim_emsg(MessageBoxIcon.Warning, "Upozorenje. Zadali ste godinu koja nije 'radna'.");
   }

   public void OnEnterSaveKontoAndKupdobCd(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvTextBoxEditingControl vtbec = sender as VvTextBoxEditingControl;
      VvDataGridView          dgv   = (VvDataGridView)vtbec.EditingControlDataGridView;

      int rowIdx = dgv.CurrentRow.Index;
      int colIdx = dgv.CurrentCellAddress.X;

      VvTextBox vvtb = (VvTextBox)(dgv.Columns[colIdx].Tag);
      
      vtbec.OtsKonto    = vvtb.OtsKonto    = dgv.GetStringCell(ci.iT_konto,     rowIdx, false);
      vtbec.OtsKupdobCd = vvtb.OtsKupdobCd = dgv.GetUint32Cell(ci.iT_kupdob_cd, rowIdx, false);
      vtbec.OtsDateDo   = vvtb.OtsDateDo   = Fld_DokDate;

      decimal t_dug = dgv.GetDecimalCell(ci.iT_dug, rowIdx, false);
      decimal t_pot = dgv.GetDecimalCell(ci.iT_pot, rowIdx, false);

      vtbec.OtsMoney = vvtb.OtsMoney = t_dug.NotZero() ? t_dug : t_pot;
   }

   public void TestingFldEntryFunction(object sender, EventArgs e)
   {
      if(sender is VvTextBoxEditingControl)
      {
         VvTextBoxEditingControl vtbec = sender as VvTextBoxEditingControl;
         DataGridView dgv = vtbec.EditingControlDataGridView;

         ZXC.aim_emsg("row: <{0}> col: <{1}> row: <{2}> col: <{3}>",
            TheG.CurrentCellAddress.Y + 1, TheG.CurrentCellAddress.X + 1,
             dgv.CurrentCellAddress.Y + 1, dgv.CurrentCellAddress.X + 1);
      }
   }

   public void TestingFldExitFunction(object sender, EventArgs e)
   {
      ZXC.aim_emsg("Izadjosmo");
   }


   #endregion ZaglavljeHamper
   
   #region TheGrid_Columns

   protected void T_konto_CreateColumn(int _width)
   {
      vvtbT_konto = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_konto", TheVvDaoTrans, DB_Tci.t_konto, "Konto iz Kontnog Plana");
      vvtbT_konto.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      vvtbT_konto.JAM_DataRequired = true;
      // 15.4.2011:
    //vvtbT_konto.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType,  ZXC.AutoCompleteRestrictor.KPL_Analitika_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      vvtbT_konto.JAM_SetAutoCompleteData(Kplan.recordName, VvSQL.SorterType.KontoNaziv, ZXC.AutoCompleteRestrictor.KPL_Analitika_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_KplanNaziv_sorterCode), null);
      //vvtbT_konto.JAM_MustFill = true;

      // 22.03.2018: 
      vvtbT_konto.JAM_FieldExitMethod/*_2*/         = new       EventHandler(OnExitKonto_ClearPreffix);
      vvtbT_konto.JAM_FieldExitWithValidationMethod = new CancelEventHandler(OnExitKonto_CheckIfEmpty);
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_konto, TheVvDaoTrans, DB_Tci.t_konto, "Konto", _width);
   }

   public void OnExitKonto_ClearPreffix(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      string dirtyString = "", theKonto;
      int rowIdx = -1;

      VvTextBoxEditingControl vvTbKonto = null;
      

      vvTbKonto = sender as VvTextBoxEditingControl;

      if(vvTbKonto == null) return;

      dirtyString = vvTbKonto.Text;

      rowIdx = vvTbKonto.EditingControlDataGridView.CurrentRow.Index;

      int spaceIdx = dirtyString.IndexOf(' ');

      // 22.03.2018: 
    //if(dirtyString.Length.IsZero () || spaceIdx.IsNegative      ())   return;
      if(dirtyString.Length.NotZero() && spaceIdx.IsZeroOrPositive()) //return;
      { 
         theKonto = dirtyString.Substring(0, spaceIdx);
      }
      else
      {
         theKonto = dirtyString;
      }

      //vvTbKonto.Text = cleanString;
      //TheG.EditingControl.Text = cleanString;
      TheG.PutCell(ci.iT_konto, rowIdx, theKonto);

      // 10.04.2015: 
      if(this.KSD.Dsc_IsVisibleColFond)
      {
         Kplan kplan_rec = KplanSifrar.SingleOrDefault(k => k.Konto == theKonto);
         if(kplan_rec != null)
         {
            TheG.PutCell(ci.iT_fond    , rowIdx, kplan_rec.Fond);
            TheG.PutCell(ci.iT_fondName, rowIdx, ZXC.luiListaFinFond.GetNameForThisCd(kplan_rec.Fond));

            Set_ProgAktiv_AutocompleteCustomSource(theKonto);
         }

         string progAktiv = TheG.GetStringCell(ci.iT_progAktiv, rowIdx, false);
         if(progAktiv.NotEmpty())
         {
            OnExitProgAktiv_SetPozicija(sender, e);
         }
         else
         {
            VvLookUpItem lui = GetDefault_progAktiv_Lui(theKonto);
            if(lui != null)
            {
               TheG.PutCell(ci.iT_pozicija    , rowIdx, lui.Cd);
               TheG.PutCell(ci.iT_pozicijaName, rowIdx, lui.Name);
               TheG.PutCell(ci.iT_progAktiv   , rowIdx, GetProgAktiv_FromLui(lui));
            }
         }
      }

   }

   public void OnExitKonto_CheckIfEmpty(object sender, CancelEventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      string dirtyString = "", theKonto;
      int rowIdx = -1;

      dirtyString = TheG.EditingControl.Text;

      rowIdx = TheG.CurrentRow.Index;

      int spaceIdx = dirtyString.IndexOf(' ');

      // 22.03.2018: 
    //if(dirtyString.Length.IsZero () || spaceIdx.IsNegative      ())   return;
      if(dirtyString.Length.NotZero() && spaceIdx.IsZeroOrPositive()) //return;
      { 
         theKonto = dirtyString.Substring(0, spaceIdx);
      }
      else
      {
         theKonto = dirtyString;
      }

      if(KSD.Dsc_isExtValidation)
      {
         if(KplanSifrar.SingleOrDefault(k => k.Konto == theKonto) == null)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Konto [{0}]\n\nNe postoji.", theKonto); e.Cancel = true;
         }
      }
   }

   private void Set_ProgAktiv_AutocompleteCustomSource(string konto)
   {
      string[] stringArray = ZXC.luiListaPozicijePlanaRLZ.Where(pp => pp.Cd.Contains(konto.SubstringSafe(0, 4))).Select(pp => pp.Cd.SubstringSafe(0, 6)).ToArray();

      // 11.02.2017: 
      if(vvtbT_progAktiv == null) return;

      vvtbT_progAktiv.AutoCompleteCustomSource.Clear();
      vvtbT_progAktiv.AutoCompleteCustomSource.AddRange(stringArray);
   }


   protected void T_opis_CreateColumn()
   {
      vvtbT_opis = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_opis", TheVvDaoTrans, DB_Tci.t_opis, "Tekstualni opis knjiženja.");
      //vvtbT_opis.JAM_FieldEntryMethod = new EventHandler(TestingFldEntryFunction);
      //vvtbT_opis.JAM_FieldExitMethod  = new EventHandler(TestingFldExitFunction);
      //vvtbT_opis.JAM_FieldExitWithValidationMethod = new CancelEventHandler(TestingFldValidation);
      vvtbT_opis.JAM_ShouldCopyPrevRow = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_opis, TheVvDaoTrans, DB_Tci.t_opis, "Opis", 0);
      colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      //colText.FillWeight = 120;
      colVvText.MinimumWidth = 50;
   }

   protected void T_kupdob_cd_CreateColumn(int _width)
   {
      vvtbT_kupdob_cd = TheG.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColT_kupdob_cd", TheVvDaoTrans, DB_Tci.t_kupdob_cd, "Sifra iz adresara");
      //vvtbT_kupdob_cd.Validated += new EventHandler(vvtb4ColT_kupdob_cd_Validated);
      vvtbT_kupdob_cd.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupdobTextBoxLeave));

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_kupdob_cd, TheVvDaoTrans, DB_Tci.t_kupdob_cd, "SifK/D", _width);
      //colVvText.Visible = isVisible;
   }

   protected void T_ticker_CreateColumn(int _width)
   {
      vvtbT_ticker = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_ticker", TheVvDaoTrans, DB_Tci.t_ticker, "Ticker iz adresara");
      vvtbT_ticker.JAM_CharacterCasing = CharacterCasing.Upper;
      vvtbT_ticker.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_ticker, TheVvDaoTrans, DB_Tci.t_ticker, "Ticker", _width);
      //colVvText.Visible = isVisible;
   }

   protected void T_mtros_cd_CreateColumn(int _width, bool isVisible)
   {
      vvtbT_mtros_cd = TheG.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColT_mtros_cd", TheVvDaoTrans, DB_Tci.t_mtros_cd, "Šifra mjesta troška");
      vvtbT_mtros_cd.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyMtrosTextBoxLeave));
      if(ZXC.CURR_prjkt_rec.IsNeprofit == false) vvtbT_mtros_cd.JAM_ShouldCopyPrevRowUnCond = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_mtros_cd, TheVvDaoTrans, DB_Tci.t_mtros_cd, "SifMTR", _width);
      colVvText.Visible = isVisible;
   }

   protected void T_mtros_tk_CreateColumn(int _width, bool isVisible)
   {
      vvtbT_mtros_tk = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_mtros_tk", TheVvDaoTrans, DB_Tci.t_mtros_tk, "Ticker mjesta troška iz adresara");
      vvtbT_mtros_tk.JAM_CharacterCasing = CharacterCasing.Upper;
      vvtbT_mtros_tk.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_mtros_tk, TheVvDaoTrans, DB_Tci.t_mtros_tk, "TickMTR", _width);
      colVvText.Visible = isVisible;
   }

   protected void T_tipBr_CreateColumn(int _width)
   {
      vvtbT_tipBr = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_tipBr", TheVvDaoTrans, DB_Tci.t_tipBr, "Vezni dokument iz robnog knjigovodstva");
      //vvtbT_tipBr.JAM_CharacterCasing = CharacterCasing.Upper;
      vvtbT_tipBr.JAM_SetOtsInfoData(/*new EventHandler(TipBrTextBoxLeave)*/);
      vvtbT_tipBr.JAM_FieldEntryMethod = new EventHandler(OnEnterSaveKontoAndKupdobCd);
      vvtbT_tipBr.JAM_FieldExitMethod  = new EventHandler(TipBrTextBoxLeave);

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_tipBr, TheVvDaoTrans, DB_Tci.t_tipBr, "TT Br", _width);
   }

   protected void T_valuta_CreateColumn(int _width)
   {
      colDate = TheG.CreateCalendarColumn(TheVvDaoTrans, DB_Tci.t_valuta, "Valuta", _width);
   }

   protected void T_pdv_CreateColumn(int _width, bool isVisible)
   {
      vvtbT_pdv = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_pdv", TheVvDaoTrans, DB_Tci.t_pdv,
         "Izbijanje PDV-a. Prazno: izbijaj 25%. 'N': NE izb.(0%pdv) '5': izbijaj 5%, 'T': izbijaj 10%(do 31.12.2013. a od 01.01.2014. 'D') ili 13%, '2': izbijaj 22%, '3': izbijaj 23%");

      vvtbT_pdv.JAM_CharacterCasing = CharacterCasing.Upper;
      vvtbT_pdv.JAM_AllowedInputCharacters = "01235789NTPD";

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_pdv, TheVvDaoTrans, DB_Tci.t_pdv, "P", _width);
      colVvText.Visible = isVisible;
   }

   protected void T_037_CreateColumn(int _width, bool isVisible)
   {
      bool isBef2017 = ZXC.projectYearFirstDay.Year < ZXC.Date01012017.Year;
      string tekst = isBef2017 ? "Koliko u kol. 15 (ostatak ide u 14). Prazno: 100%, '0': 0%, '3': 30%, '7': 70%." :
                                 "Koliko u kol. 15 (ostatak ide u 14). Prazno: 100%, '0': 0%, '3': 30%, '7': 70%, '5': 50% ";

      vvtbT_037 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_037", TheVvDaoTrans, DB_Tci.t_037, tekst);
    //15.02.2017. ide novo 50/50
    //vvtbT_037.JAM_AllowedInputCharacters = "037";
      vvtbT_037.JAM_AllowedInputCharacters = isBef2017 ? "037" : "0375";

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_037, TheVvDaoTrans, DB_Tci.t_037, "I", _width);
      colVvText.Visible = isVisible;
   }

   protected void T_dug_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_dug = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_dug", TheVvDaoTrans, DB_Tci.t_dug, "Iznos DUGUJE");
      vvtbT_dug.JAM_ShouldSumGrid = true;
      
      //vvtb4ColT_dug.JAM_MarkAsNumericTextBox(2, true, 1000, true);

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dug, TheVvDaoTrans, DB_Tci.t_dug, "Duguje", _width);
      // colVvText.AutoSizeMode= DataGridViewAutoSizeColumnMode.AllCells; ___ne zbog sume
      /*???!!!*/
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }

   protected void T_pot_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_pot = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_pot", TheVvDaoTrans, DB_Tci.t_pot, "Iznos POTRAZUJE");
      vvtbT_pot.JAM_ShouldSumGrid = true;
      //vvtb4ColT_dug.JAM_MarkAsNumericTextBox(2, true, 1000, true);

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_pot, TheVvDaoTrans, DB_Tci.t_pot, "Potražuje", _width);
      // colVvText.AutoSizeMode= DataGridViewAutoSizeColumnMode.AllCells; ___ne zbog sume
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }

   protected void T_kdName_CreateColumn(int _width)
   {
      vvtbT_kupdobName = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_kupdobName", null, -12, "Naziv partnera iz adresara");
      vvtbT_kupdobName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyKupdobTextBoxLeave));

      //colVvText = TheG.CreateVvTextBoxColumn(vvtbT_kupdobName, TheVvDaoTrans, DB_Tci.t_kupdobName, "Partner", _width);
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_kupdobName, null, "T_kupdob_name", "Partner", _width);
   }

   protected void T_projekt_CreateColumn(int _width, bool isVisible)
   {
      vvtbT_projektCD = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_projektCD", TheVvDaoTrans, DB_Tci.t_projektCD, "Projekt");
      vvtbT_projektCD.JAM_CharacterCasing = CharacterCasing.Upper;
      //vvtbT_projekt.JAM_SetOtsInfoData(/*new EventHandler(TipBrTextBoxLeave)*/);
      //vvtbT_projekt.JAM_FieldEntryMethod = new EventHandler(OnEnterSaveKontoAndKupdobCd);
      //vvtbT_projekt.JAM_FieldExitMethod = new EventHandler(TipBrTextBoxLeave);
      vvtbT_projektCD.JAM_SetAutoCompleteData(Faktur.recordName, Faktur.sorterTtNum.SortType, ZXC.AutoCompleteRestrictor.FAK_ExactTT_Only, OnVvTBEnter_SetAutocmplt_Faktur_DUMMY, null);

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_projektCD, TheVvDaoTrans, DB_Tci.t_projektCD, "Projekt", _width);
      colVvText.Visible = isVisible;
   }

   protected abstract void InitializeDUC_Specific_Columns();

   public void InitializeTheGrid_Columns()
   {
      InitializeDUC_Specific_Columns();
   }

   private void TipBrTextBoxLeave(object sender, EventArgs e)
   {
      if(ZXC.DumpChosenOtsList_OnNalogDUC_InProgress)
      {
         ZXC.DumpChosenOtsList_OnNalogDUC_InProgress = false;

         return;
      }

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      Faktur faktur_rec = new Faktur();

      VvTextBoxEditingControl vvtb_editingControl = sender as VvTextBoxEditingControl;

      if(vvtb_editingControl == null) return;

      if(vvtb_editingControl.EditedHasChanges())
      {
         int currRow = vvtb_editingControl.EditingControlRowIndex;

         string t_tipBr = TheG.GetStringCell(ci.iT_tipBr, currRow, false);

         string tt;
         uint   ttNum;

         bool found=false, parseOK = Ftrans.ParseTipBr(t_tipBr, out tt, out ttNum);

         if(parseOK)
         {
            found = FakturDao.SetMeFaktur(TheDbConnection, faktur_rec, tt, ttNum, false);
         }

         if(found && vvtb_editingControl.Text != "")
         {
            TheG.PutCell(ci.iT_fakRecID, currRow, faktur_rec.RecID);
            TheG.PutCell(ci.iT_fakYear , currRow, faktur_rec.DokDate.Year);
         }
         else
         {
            // 12.01.2017: start 
            bool shouldCheck = Fld_Napomena.ToUpper().Contains("RASTER") &&
                              (Fld_TipTran == "UR" || Fld_TipTran == "IR");
            if(shouldCheck)
            {
               ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE!!!\n\nZa zadani 'TT Br' (broj raèuna)\n\n ne mogu pronaæi vezani dokument u modulu 'RAÈUNI I SKLADIŠTE'.\n\nBuduæe automatsko uèitavanje raèuna æe biti pogrešno i/ili duplo!\n\nPreporuka je da ovaj podatak ostavite kakav je bio pri automatskom uèitavanju.");
            }
            // 12.01.2017:  end  

            TheG.PutCell(ci.iT_fakRecID, currRow, 0);
            TheG.PutCell(ci.iT_fakYear , currRow, 0);
         }
      }

   }

   private void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      Kupdob kupdob_rec;

      VvTextBoxEditingControl vvtb_editingControl = sender as VvTextBoxEditingControl;

      if(vvtb_editingControl == null) return;

      if(vvtb_editingControl.Text != this.originalText)
      {
         this.originalText = vvtb_editingControl.Text;
         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         int currRow = vvtb_editingControl.EditingControlRowIndex;

         if(kupdob_rec != null && vvtb_editingControl.Text != "")
         {
            TheG.PutCell(ci.iT_ticker     , currRow, kupdob_rec.Ticker);
            TheG.PutCell(ci.iT_kupdob_cd  , currRow, kupdob_rec.KupdobCD);
            TheG.PutCell(ci.iT_kupdob_name, currRow, kupdob_rec.Naziv);
         }
         else
         {
            TheG.PutCell(ci.iT_ticker     , currRow, "");
            TheG.PutCell(ci.iT_kupdob_cd  , currRow, "");
            TheG.PutCell(ci.iT_kupdob_name, currRow, "");
         }

         // samo za DUC-eve
         ZXC.TheVvForm.SetDirtyFlag(sender);
      }

   }

   private void AnyMtrosTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      Kupdob kupdob_rec;

      VvTextBoxEditingControl vvtb_editingControl = sender as VvTextBoxEditingControl;

      if(vvtb_editingControl == null) return;

      if(vvtb_editingControl.Text != this.originalText)
      {
         this.originalText = vvtb_editingControl.Text;
         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         int currRow = vvtb_editingControl.EditingControlRowIndex;

         if(kupdob_rec != null && vvtb_editingControl.Text != "")
         {
            TheG.PutCell(ci.iT_mtros_tk, currRow, kupdob_rec.Ticker);
            TheG.PutCell(ci.iT_mtros_cd, currRow, kupdob_rec.KupdobCD/*RecID*/);
         }
         else
         {
            TheG.PutCell(ci.iT_mtros_tk, currRow, "");
            TheG.PutCell(ci.iT_mtros_cd, currRow, "");
         }
      }

   }

   protected void T_pdvKnjiga_CreateColumn(int _width, bool isVisible)
   {
      vvtbT_pdvKnjiga = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_pdvKnjiga", TheVvDaoTrans, DB_Tci.t_pdvKnjiga, "PdvKnjiga  R - Redovna knjiga URA/IRA, P - Knjiga predujmova URA/IRA, U - Knjiga uvoza robe URA,   S - Knjiga uvoza usluga URA");
      vvtbT_pdvKnjiga.JAM_AllowedInputCharacters = "RPUS";
      vvtbT_pdvKnjiga.JAM_CharacterCasing = CharacterCasing.Upper;
      //vvtbT_pdvKnjiga.JAM_ShouldCalcTrans = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_pdvKnjiga, TheVvDaoTrans, DB_Tci.t_pdvKnjiga, "K", _width);
      colVvText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
      colVvText.Visible = isVisible;

   }

   protected void T_fakRecID_CreateColumn(VvDataGridView _theGrid)
   {
      CreateAllwaysInvisibleDataGridViewColumn(_theGrid, "t_fakRecID");
   }

   protected void T_fakYear_CreateColumn(VvDataGridView _theGrid)
   {
      CreateAllwaysInvisibleDataGridViewColumn(_theGrid, "t_fakYear");
   }

   protected void T_otsKind_CreateColumn(VvDataGridView _theGrid)
   {
      CreateAllwaysInvisibleDataGridViewColumn(_theGrid, "t_otsKind");
   }

   protected void ColumnForScroll_CreateColumn(int _width)
   {
      colScrol = TheG.CreateScrollColumn("scrol", _width);
      colScrol.ReadOnly = true;
   }

   protected void T_pozicija_CreateColumn(int _width, bool isVisible)
   {
      VvLookUpLista.LoadResultLuiList_PozicijePlana_PLN_or_RLZ(/* isPLN */ false);

      vvtbT_pozicija = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_pozicija", TheVvDaoTrans, DB_Tci.t_pozicija, "Pozicija iz Plana");
      vvtbT_pozicija.JAM_CharacterCasing = CharacterCasing.Upper;

      vvtbT_pozicija.JAM_Set_LookUpTable(ZXC.luiListaPozicijePlanaRLZ, (int)ZXC.Kolona.prva);

      vvtbT_pozicija.JAM_ReadOnly = KSD.Dsc_IsVisibleColProgAkt;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_pozicija, TheVvDaoTrans, DB_Tci.t_pozicija, "Pozicija", _width);
      colVvText.Visible = isVisible;

   }

   protected void T_pozicijaName_CreateColumn(int _width, bool isVisible)
   {
      vvtbT_pozicijaName = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_pozicijaName", null, -12, "Naziv pozicije plana");
      vvtbT_pozicijaName.JAM_ReadOnly = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_pozicijaName, null, "T_pozicijaName", "Naziv pozicije plana", _width);
      colVvText.Visible = isVisible;

      vvtbT_pozicija.JAM_lui_NameTaker_JAM_Name = "T_pozicijaName";
   }

   protected void T_fond_CreateColumn(int _width, bool isVisible)
   {
      vvtbT_fond = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_fond", TheVvDaoTrans, DB_Tci.t_fond, "Fond");
      vvtbT_fond.JAM_CharacterCasing = CharacterCasing.Upper;

      vvtbT_fond.JAM_Set_LookUpTable(ZXC.luiListaFinFond, (int)ZXC.Kolona.prva);

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_fond, TheVvDaoTrans, DB_Tci.t_fond, "Fond", _width);
      colVvText.Visible = isVisible;

   }

   protected void T_fondName_CreateColumn(int _width, bool isVisible)
   {
      vvtbT_fondName = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_fondName", null, -12, "Naziv Fonda");
      vvtbT_fondName.JAM_ReadOnly = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_fondName, null, "T_fondName", "Naziv F", _width);
      colVvText.Visible = isVisible;

      vvtbT_fond.JAM_lui_NameTaker_JAM_Name = "T_fondName";
   }

   protected void T_progAktiv_CreateColumn(int _width, bool isVisible)
   {
      vvtbT_progAktiv = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_progAktiv", TheVvDaoTrans, DB_Tci.t_progAktiv, "Program + Aktivnost ili projekt");
//    vvtbT_progAktiv = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_progAktiv", TheVvDaoTrans, DB_Tci.t_progAktiv, "Program + Aktivnost ili projekt");
      vvtbT_progAktiv.JAM_CharacterCasing = CharacterCasing.Upper;
      vvtbT_progAktiv.JAM_FieldExitMethod_2 = new EventHandler(OnExitProgAktiv_SetPozicija);
//    vvtbT_fond.JAM_Set_LookUpTable(ZXC.luiListaFinFond, (int)ZXC.Kolona.prva);

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_progAktiv, TheVvDaoTrans, DB_Tci.t_progAktiv, "ProgAkt", _width);
      colVvText.Visible = isVisible;

   }

   public void OnExitProgAktiv_SetPozicija(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvTextBoxEditingControl vvTbProgAktiv = sender as VvTextBoxEditingControl;

      if(vvTbProgAktiv == null) return;

      int rowIdx = vvTbProgAktiv.EditingControlDataGridView.CurrentRow.Index;

      string progAktiv = TheG.GetStringCell(ci.iT_progAktiv, rowIdx, false);
      string konto     = TheG.GetStringCell(ci.iT_konto    , rowIdx, false);

      if(progAktiv.IsEmpty())
      {
         progAktiv = GetProgAktiv_FromLui(GetDefault_progAktiv_Lui(konto));
         TheG.PutCell(ci.iT_progAktiv, rowIdx, progAktiv);
      }

      if(konto.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Zadajte prvo konto pa ponovite unos ProgAktiv ovog retka!");
      }

      string pozicija = progAktiv + " " + konto.SubstringSafe(0, 4);

      VvLookUpItem lui;
      if(ComponovanaPozicijaExists(pozicija, out lui))
      {
         TheG.PutCell(ci.iT_pozicija    , rowIdx, lui.Cd  );
         TheG.PutCell(ci.iT_pozicijaName, rowIdx, lui.Name);
      }
      else
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "U Lookup Listi nema pozicije [{0}]", pozicija);
         TheG.PutCell(ci.iT_progAktiv   , rowIdx, "");
         TheG.PutCell(ci.iT_pozicija    , rowIdx, "");
         TheG.PutCell(ci.iT_pozicijaName, rowIdx, "");
      }
   }

   private bool ComponovanaPozicijaExists(string pozicija, out VvLookUpItem lui)
   {
      lui = ZXC.luiListaPozicijePlanaRLZ.GetLuiForThisCd(pozicija);
      return lui != null;
   }

   private VvLookUpItem GetDefault_progAktiv_Lui(string konto)
   {
      VvLookUpItem lui = ZXC.luiListaPozicijePlanaRLZ.FirstOrDefault(pp => pp.Cd.Contains(konto.SubstringSafe(0, 4)));

      return lui;
   }

   private string GetProgAktiv_FromLui(VvLookUpItem lui)
   {
      string progAktiv = "";

      if(lui != null) progAktiv = lui.Cd.SubstringSafe(0, 6);

      return progAktiv;
   }

   #endregion TheGrid_Columns

   #region Fld_

   public uint Fld_DokNum
   {
      get { return ZXC.ValOrZero_UInt(tbx_brNaloga.Text); }
      set {                           tbx_brNaloga.Text = value.ToString("000000"); }
   }

   public string Fld_TipTran
   {
      get { return tbx_vrstaKnj.Text; }
      set {        tbx_vrstaKnj.Text = value; }
   }

   public string Fld_TtOpis
   {
      set {  tbx_VrKnjizOpis.Text = value; }
   }

   public DateTime Fld_DokDate
   {
      get { return dTP_datum.Value; }
      set 
      {  
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
            dTP_datum.Value = value; 
      }
   }

   public string Fld_DokDateAsTxt
   {
      get { return tbx_datum.Text; }
      set {        tbx_datum.Text = value; }
   }

   public string Fld_Napomena
   {
      get { return tbx_napomena.Text; }
      set {        tbx_napomena.Text = value; }
   }

   public uint Fld_TtNum
   {
      get { return ZXC.ValOrZero_UInt(tbx_VKnum.Text); }
      set {                           tbx_VKnum.Text = value.ToString("0000"); }

   }

   public string Fld_DevName
   {
      get { return tbx_ValName.Text; }
      set {        tbx_ValName.Text = value; }
   }

   public ZXC.ValutaNameEnum Fld_DevNameAsEnum
   {
      get
      {
         if(tbx_ValName.Text.IsEmpty()) return ZXC.ValutaNameEnum.EMPTY;
         else                           return (ZXC.ValutaNameEnum)Enum.Parse(typeof(ZXC.ValutaNameEnum), tbx_ValName.Text, true);
      }
   }

   public string Fld_DevTecaj { get; set; }

   #endregion Fld_

   #region CreateNalogPrintTemplateUC

   protected void CreateNalogPrintTemplateUC(VvUserControl vvUC)
   {
      this.TheNalogTemplateFilter     = new NalogTemplateFilter();

      TheNalogTemplateFilterUC                   = new NalogTemplateFilterUC(vvUC);
      TheNalogTemplateFilterUC.Parent            = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheNalogTemplateFilterUC.Width;

   }

   #endregion CreateNalogPrintTemplateUC

   #region PutFields(), GetFields()

   public override void PutFields(VvDataRecord _nalog)
   {
      #region ToggleDeviza Additions

      this.IsShowingConvertedMoney = false;
      TheVvTabPage.TheVvForm.ToggleDevizaVisualApperiance(false, "", "");
      
      #endregion ToggleDeviza Additions

      PutFields(_nalog, false);

      #region ToggleDeviza Additions

      if(nalog_rec.DevName.NotEmpty() &&
         nalog_rec.DevName != /*ZXC.ValutaNameEnum.HRK*/ZXC.EURorHRK_NameEnum.ToString())

         TheVvTabPage.TheVvForm.NAL_ToggleKnDeviza(null, EventArgs.Empty);

      #endregion ToggleDeviza Additions

   }

   public override void PutFields(VvDataRecord nalog, bool isCopyingToAnotherDUC)
   {
      nalog_rec = (Nalog)nalog;
      if(nalog_rec != null)
      {

         PutMetaFileds(nalog_rec.AddUID, nalog_rec.AddTS, nalog_rec.ModUID, nalog_rec.ModTS, nalog_rec.RecID, nalog_rec.LanSrvID, nalog_rec.LanRecID);

         PutIdentityFields(nalog_rec.DokNum.ToString("000000"), nalog_rec.DokDate.ToString(ZXC.VvDateFormat), nalog_rec.TT, "");

         Fld_DokNum       = nalog_rec.DokNum;
         Fld_DokDateAsTxt = nalog_rec.DokDate.ToString(ZXC.VvDateFormat);
         Fld_DokDate      = nalog_rec.DokDate;
         Fld_Napomena     = nalog_rec.Napomena;
         Fld_TipTran      = nalog_rec.TT;
         Fld_TtNum        = nalog_rec.TtNum;
         Fld_DevName      = nalog_rec.DevName;
         Fld_DevTecaj     = nalog_rec.DevTecaj.ToString();

         Fld_TtOpis = ZXC.luiListaNalogTT.GetNameForThisCd(nalog_rec.TT);

         tbx_napomena.TextAsToolTip(toolTip);

         PutDgvFields();

         InitializeFilterUCFields();

         recordReportLoaded = false;
         DecideIfShouldLoad_VvReport(null, null, null);

      }

      TheG.ClearSelection();
      TheSumGrid.ClearSelection();

   }

   // Tu dolazimo na 1 nacin: 1. Classic PutFields 
   private void InitializeFilterUCFields()
   {
      SetFilterRecordDependentDefaults(); // filter.DokNumOd = nalog_rec.DokNum (punimo bussiness od filtera, ne UC)
      TheNalogTemplateFilterUC.PutFilterFields(TheNalogTemplateFilter);
   }

   public override void GetFields(bool dirtyFlagging)
   {
      nalog_rec.DokNum   = Fld_DokNum;
      nalog_rec.DokDate  = Fld_DokDate;
      nalog_rec.Napomena = Fld_Napomena;
      nalog_rec.TT       = Fld_TipTran;
      nalog_rec.TtNum    = Fld_TtNum;
      nalog_rec.DevName  = Fld_DevName;

      GetDgvFields(dirtyFlagging);
   }

   #region Put_NewDocum_NumAndDateFields

   public override void Put_NewDocum_NumAndDateFields(uint dokNum, DateTime dokDate)
   {
      Fld_DokNum  = dokNum;
      Fld_DokDate = dokDate;
   }

   public override void Put_NewTT_Num(uint ttNum)
   {
      Fld_TtNum = ttNum;
   }

   #endregion Put_NewDocum_NumAndDateFields

   #endregion PutFields(), GetFields()

   #region PutDgvFields(), GetDgvFields()

   #region SetNalogColumnIndexes()

   /// <summary>
   /// Column Index na DataGridView-u (NE U DataBase to ti je 'DB_ci')
   /// </summary>
   private Nalog_colIdx ci;
   public  Nalog_colIdx DgvCI { get { return ci; } }

   public struct Nalog_colIdx
   {
      internal int iT_recID;
      internal int iT_serial;
      internal int iT_konto;
      internal int iT_opis;
      internal int iT_kupdob_cd;
      internal int iT_kupdob_name;
      internal int iT_ticker;
      internal int iT_mtros_cd;
      internal int iT_mtros_tk;
      internal int iT_tipBr;
      internal int iT_valuta;
      internal int iT_pdv;
      internal int iT_037;
      internal int iT_dug;
      internal int iT_pot;
      internal int iT_pdvKnjiga;
      internal int iT_fakRecID;
      internal int iT_otsKind;
      internal int iT_projektCD;
      internal int iT_pozicija;
      internal int iT_pozicijaName;
      internal int iT_fond;
      internal int iT_fondName;
      internal int iT_progAktiv;
      internal int iT_fakYear;
   }

   private void SetNalogColumnIndexes()
   {
      ci = new Nalog_colIdx();

      ci.iT_recID        = TheG.IdxForColumn("T_recID");
      ci.iT_serial       = TheG.IdxForColumn("T_serial");
      ci.iT_konto        = TheG.IdxForColumn("T_konto");
      ci.iT_opis         = TheG.IdxForColumn("T_opis");
      ci.iT_kupdob_cd    = TheG.IdxForColumn("T_kupdob_cd");
      ci.iT_kupdob_name  = TheG.IdxForColumn("T_kupdob_name");
      ci.iT_ticker       = TheG.IdxForColumn("T_ticker");
      ci.iT_tipBr        = TheG.IdxForColumn("T_tipBr");
      ci.iT_valuta       = TheG.IdxForColumn("T_valuta");
      ci.iT_dug          = TheG.IdxForColumn("T_dug");
      ci.iT_pot          = TheG.IdxForColumn("T_pot");

      ci.iT_mtros_cd     = TheG.IdxForColumn("T_mtros_cd");
      ci.iT_mtros_tk     = TheG.IdxForColumn("T_mtros_tk");
      ci.iT_pdv          = TheG.IdxForColumn("T_pdv");
      ci.iT_037          = TheG.IdxForColumn("T_037");
      ci.iT_pdvKnjiga    = TheG.IdxForColumn("T_pdvKnjiga");
      ci.iT_fakRecID     = TheG.IdxForColumn("T_fakRecID");
      ci.iT_fakYear      = TheG.IdxForColumn("T_fakYear");
      ci.iT_otsKind      = TheG.IdxForColumn("T_otsKind");
      ci.iT_projektCD    = TheG.IdxForColumn("T_projektCD");
      ci.iT_pozicija     = TheG.IdxForColumn("T_pozicija");
      ci.iT_pozicijaName = TheG.IdxForColumn("T_pozicijaName");
      ci.iT_fond         = TheG.IdxForColumn("T_fond");
      ci.iT_fondName     = TheG.IdxForColumn("T_fondName");
      ci.iT_progAktiv    = TheG.IdxForColumn("T_progAktiv");
   }

   #endregion SetNalogColumnIndexes()

   /*private*/ public void PutDgvFields()
   {
      int rowIdx, idxCorrector;

      TheG.RowsRemoved -= new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);
      TheG.RowsAdded   -= new DataGridViewRowsAddedEventHandler  (grid_RowsAdded);
      TheG.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(TheG);
      
      if(nalog_rec.Transes != null)
      {
         SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

         foreach(Ftrans ftrans_rec in nalog_rec.Transes)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheG.Rows.Add();

            rowIdx = TheG.RowCount - idxCorrector;

            PutDgvLineFields(ftrans_rec, rowIdx, false);
         }
      }

      TheG.RowsAdded   += new DataGridViewRowsAddedEventHandler  (grid_RowsAdded);
      TheG.RowsRemoved += new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);

      RenumerateLineNumbers(TheG, 0);

      UpdateLineCount(TheG);

      PutDgvTransSumFields();
   }

   public override void PutDgvLineFields(VvTransRecord trans_rec, int rowIdx, bool skipRecID_andSerial_Columns)
   {
      Ftrans ftrans_rec = (Ftrans)trans_rec;

      if(skipRecID_andSerial_Columns == false)
      {
         TheG.PutCell(ci.iT_recID,  rowIdx, ftrans_rec.T_recID);
         TheG.PutCell(ci.iT_serial, rowIdx, ftrans_rec.T_serial);
      }

      TheG.PutCell(ci.iT_konto,        rowIdx, ftrans_rec.T_konto);
      TheG.PutCell(ci.iT_opis,         rowIdx, ftrans_rec.T_opis);
      TheG.PutCell(ci.iT_ticker,       rowIdx, ftrans_rec.T_ticker);
      TheG.PutCell(ci.iT_kupdob_cd,    rowIdx, ftrans_rec.T_kupdob_cd);
      TheG.PutCell(ci.iT_tipBr,        rowIdx, ftrans_rec.T_tipBr);
      TheG.PutCell(ci.iT_valuta,       rowIdx, ftrans_rec.T_valuta);
      TheG.PutCell(ci.iT_dug,          rowIdx, VvCurrency(ftrans_rec.T_dug));
      TheG.PutCell(ci.iT_pot,          rowIdx, VvCurrency(ftrans_rec.T_pot));
      TheG.PutCell(ci.iT_pdv,          rowIdx, ftrans_rec.T_pdv);
      TheG.PutCell(ci.iT_037,          rowIdx, ftrans_rec.T_037);
      TheG.PutCell(ci.iT_mtros_cd,     rowIdx, ftrans_rec.T_mtros_cd);
      TheG.PutCell(ci.iT_mtros_tk,     rowIdx, ftrans_rec.T_mtros_tk);
      TheG.PutCell(ci.iT_pdvKnjiga,    rowIdx, GetOneLetter4PdvKnjiga(ftrans_rec.T_pdvKnjiga));
      TheG.PutCell(ci.iT_fakRecID ,    rowIdx, ftrans_rec.T_fakRecID);
      TheG.PutCell(ci.iT_fakYear  ,    rowIdx, ftrans_rec.T_fakYear);
      TheG.PutCell(ci.iT_otsKind  ,    rowIdx, ftrans_rec.T_otsKind);
      TheG.PutCell(ci.iT_projektCD,    rowIdx, ftrans_rec.T_projektCD);
      TheG.PutCell(ci.iT_pozicija ,    rowIdx, ftrans_rec.T_pozicija);
      TheG.PutCell(ci.iT_pozicijaName, rowIdx, ZXC.luiListaPozicijePlanaRLZ.GetNameForThisCd(ftrans_rec.T_pozicija));
      TheG.PutCell(ci.iT_fond    ,     rowIdx, ftrans_rec.T_fond);
      TheG.PutCell(ci.iT_fondName,     rowIdx, ZXC.luiListaFinFond.GetNameForThisCd(ftrans_rec.T_fond));
      TheG.PutCell(ci.iT_progAktiv,    rowIdx, ftrans_rec.T_progAktiv);

      Kupdob kupdobSifrar_rec = KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD == ftrans_rec.T_kupdob_cd);

      if(kupdobSifrar_rec != null) TheG.PutCell(ci.iT_kupdob_name, rowIdx, kupdobSifrar_rec.Naziv);
      else                         TheG.PutCell(ci.iT_kupdob_name, rowIdx, "");

      //ptrano_rec.CalcTransResults();

      //PutDgvLineResultsFields(rowIdx, ftrans_rec);

      if(ZXC.RRD.Dsc_F2_TT.NotEmpty())
      {
         if(ftrans_rec.R_IsMAP_Ftr)
         {
            TheG.Rows[rowIdx].DefaultCellStyle.BackColor = Color.LightCoral;

            if(FtransDao.IsMAPdone(TheDbConnection, ftrans_rec))
            {
               TheG.Rows[rowIdx].DefaultCellStyle.BackColor = Color.LightGreen;
            }
         }
      } 
   }

   public override void PutDgvTransSumFields()
   {
      TheSumGrid[ci.iT_dug, 0].Value = VvCurrency(nalog_rec.Sum_Dug);
      TheSumGrid[ci.iT_pot, 0].Value = VvCurrency(nalog_rec.Sum_Pot);
   }

   /*private*/internal void GetDgvFields(bool dirtyFlagging)
   {
      uint[] recIDtable;
      int    rIdx;

      if(dirtyFlagging == true && ZXC.TheVvForm.VvFlag_RowsAreAddingOrDeletingOrBoth == false)
      {
         if((TheG.CurrentCell != null && TheG.CurrentCell.IsInEditMode) || ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode)
         {
            GetDgvLineFields(TheG.CurrentRow.Index, dirtyFlagging, null);

            ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode = false;
         }

         return;
      }

      if(TheG.RowCount > 0) recIDtable = new uint[TheG.RowCount - 1];
      else                  recIDtable = null;

      nalog_rec.DiscardPreviouslyAddedTranses();

      for(rIdx = 0; rIdx < TheG.RowCount - 1; ++rIdx)
      {
         GetDgvLineFields(rIdx, dirtyFlagging, recIDtable);
      } 

      MarkTransesToDelete(recIDtable);

   }

   public override VvTransRecord GetDgvLineFields(int rIdx, bool dirtyFlagging, uint[] recIDtable)
   {
      uint   recID;
      bool   DB_RWT;
      Ftrans db_rec;

      // dgvAtrans_rec: buffer za GetField iz DataGridWievRow-a                                        
      // db_rec : pointer na existing row u DB-u kojega ako je EditedHasChanges treba RWTREC-ati 

      recID = TheG.GetUint32Cell(ci.iT_recID, rIdx, dirtyFlagging);

      if(recIDtable != null) recIDtable[rIdx] = recID;

      dgvFtrans_rec = new Ftrans();

      if(recID > 0) // Postojeci redak 
      {
         db_rec = nalog_rec.Transes.SingleOrDefault(vvDR => vvDR.VirtualRecID == recID);
      }
      else // novododani redak 
      {
         db_rec  = null;
      }

      DB_RWT = (db_rec != null);

      dgvFtrans_rec.T_recID = recID;

      dgvFtrans_rec.T_parentID = nalog_rec.RecID;

      #region GetColumns

                                     dgvFtrans_rec.T_fakRecID = TheG.GetUint32Cell(ci.iT_fakRecID, rIdx, dirtyFlagging);
      if(DB_RWT) db_rec.T_fakRecID = dgvFtrans_rec.T_fakRecID;

                                     dgvFtrans_rec.T_fakYear = TheG.GetUint32Cell(ci.iT_fakYear, rIdx, dirtyFlagging);
      if(DB_RWT) db_rec.T_fakYear  = dgvFtrans_rec.T_fakYear;

                                   dgvFtrans_rec.T_serial = (ushort)(rIdx + 1);
      if(DB_RWT) db_rec.T_serial = dgvFtrans_rec.T_serial;

                                   dgvFtrans_rec.T_dokNum = nalog_rec.DokNum;
      if(DB_RWT) db_rec.T_dokNum = dgvFtrans_rec.T_dokNum;

                                    dgvFtrans_rec.T_dokDate = nalog_rec.DokDate;
      if(DB_RWT) db_rec.T_dokDate = dgvFtrans_rec.T_dokDate;

                               dgvFtrans_rec.T_TT = nalog_rec.TT;
      if(DB_RWT) db_rec.T_TT = dgvFtrans_rec.T_TT;

      if(TheG.CI_OK(ci.iT_konto))
      {
                                     dgvFtrans_rec.T_konto = TheG.GetStringCell(ci.iT_konto, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_konto = dgvFtrans_rec.T_konto;
      }
      if(TheG.CI_OK(ci.iT_opis))
      {
                                    dgvFtrans_rec.T_opis = TheG.GetStringCell(ci.iT_opis, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_opis = dgvFtrans_rec.T_opis;
      }
      if(TheG.CI_OK(ci.iT_kupdob_cd))
      {
                                         dgvFtrans_rec.T_kupdob_cd = TheG.GetUint32Cell(ci.iT_kupdob_cd, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_kupdob_cd = dgvFtrans_rec.T_kupdob_cd;
      }
      if(TheG.CI_OK(ci.iT_ticker))
      {
                                      dgvFtrans_rec.T_ticker = TheG.GetStringCell(ci.iT_ticker, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_ticker = dgvFtrans_rec.T_ticker;
      }
      if(TheG.CI_OK(ci.iT_tipBr))
      {
                                     dgvFtrans_rec.T_tipBr = TheG.GetStringCell(ci.iT_tipBr, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_tipBr = dgvFtrans_rec.T_tipBr;
      }
      if(TheG.CI_OK(ci.iT_valuta))
      {
                                      dgvFtrans_rec.T_valuta = TheG.GetDateCell(ci.iT_valuta, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_valuta = dgvFtrans_rec.T_valuta;
      }
      if(TheG.CI_OK(ci.iT_dug))
      {
                                   dgvFtrans_rec.T_dug = TheG.GetDecimalCell(ci.iT_dug, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_dug = dgvFtrans_rec.T_dug;
      }
      if(TheG.CI_OK(ci.iT_pot))
      {
                                   dgvFtrans_rec.T_pot = TheG.GetDecimalCell(ci.iT_pot, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_pot = dgvFtrans_rec.T_pot;
      }
      if(TheG.CI_OK(ci.iT_pdv))
      {
                                   dgvFtrans_rec.T_pdv = TheG.GetStringCell(ci.iT_pdv, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_pdv = dgvFtrans_rec.T_pdv;
      }
      if(TheG.CI_OK(ci.iT_037))
      {
                                   dgvFtrans_rec.T_037 = TheG.GetStringCell(ci.iT_037, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_037 = dgvFtrans_rec.T_037;
      }
      if(TheG.CI_OK(ci.iT_mtros_cd))
      {
                                        dgvFtrans_rec.T_mtros_cd = TheG.GetUint32Cell(ci.iT_mtros_cd, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_mtros_cd = dgvFtrans_rec.T_mtros_cd;
      }
      if(TheG.CI_OK(ci.iT_mtros_tk))
      {
                                        dgvFtrans_rec.T_mtros_tk = TheG.GetStringCell(ci.iT_mtros_tk, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_mtros_tk = dgvFtrans_rec.T_mtros_tk;
      }
      if(TheG.CI_OK(ci.iT_pdvKnjiga))
      {
                                         dgvFtrans_rec.T_pdvKnjiga = GetPdvKnjigaEnumFromFirstLetter(TheG.GetStringCell(ci.iT_pdvKnjiga, rIdx, dirtyFlagging));
         if(DB_RWT) db_rec.T_pdvKnjiga = dgvFtrans_rec.T_pdvKnjiga;
      }
      if(TheG.CI_OK(ci.iT_projektCD))
      {
                                         dgvFtrans_rec.T_projektCD = TheG.GetStringCell(ci.iT_projektCD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_projektCD = dgvFtrans_rec.T_projektCD;
      }
      if(TheG.CI_OK(ci.iT_pozicija))
      {
                                        dgvFtrans_rec.T_pozicija = TheG.GetStringCell(ci.iT_pozicija, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_pozicija = dgvFtrans_rec.T_pozicija;
      }
      if(TheG.CI_OK(ci.iT_fond))
      {
                                    dgvFtrans_rec.T_fond = TheG.GetStringCell(ci.iT_fond, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_fond = dgvFtrans_rec.T_fond;
      }
      if(TheG.CI_OK(ci.iT_progAktiv))
      {
                                         dgvFtrans_rec.T_progAktiv = TheG.GetStringCell(ci.iT_progAktiv, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_progAktiv = dgvFtrans_rec.T_progAktiv;
      }

      #endregion GetColumns

      // 28.02.2011: 
                 dgvFtrans_rec.SetOtsKind(); // dgvFtrans_rec.T_otsKind = ... 
      if(DB_RWT) db_rec.T_otsKind = dgvFtrans_rec.T_otsKind;

      if(dgvFtrans_rec.T_recID == 0) // ADDED NEW, FRESH Ftrans             
      {
         nalog_rec.InvokeTransRemove(dgvFtrans_rec);

         dgvFtrans_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;

         nalog_rec.Transes.Add(dgvFtrans_rec);
      }
      else if(db_rec.EditedHasChanges())
      {
         db_rec.SaveTransesWriteMode = ZXC.WriteMode.Edit;
      }
      else
      {
         db_rec.SaveTransesWriteMode = ZXC.WriteMode.None;
      }

      return dgvFtrans_rec;
   }

   public void PutDGVRowManually(int rowIdx, string konto, string opis, string ticker, uint kupdob_cd, uint fakRecID, uint fakYear, string tipBr, string projektCD, uint mtrosCD, DateTime valuta, decimal dug, decimal pot)
   {
      TheG.PutCell(ci.iT_konto,     rowIdx, konto);
      TheG.PutCell(ci.iT_opis,      rowIdx, opis);
      TheG.PutCell(ci.iT_ticker,    rowIdx, ticker);
      TheG.PutCell(ci.iT_kupdob_cd, rowIdx, kupdob_cd/*.ToString("000000")*/);
      TheG.PutCell(ci.iT_fakRecID,  rowIdx, fakRecID);
      TheG.PutCell(ci.iT_fakYear ,  rowIdx, fakYear );
      TheG.PutCell(ci.iT_tipBr,     rowIdx, tipBr);
      TheG.PutCell(ci.iT_projektCD, rowIdx, projektCD);
      TheG.PutCell(ci.iT_mtros_cd , rowIdx, mtrosCD);
      TheG.PutCell(ci.iT_valuta,    rowIdx, valuta);
      TheG.PutCell(ci.iT_dug,       rowIdx, dug);
      TheG.PutCell(ci.iT_pot,       rowIdx, pot);
   }

   #endregion PutDgvFields(), GetDgvFields()

   #region DumpChosenOtsList

   internal void DumpChosenOtsList(List<OtsTipBrGroupInfo> choosenOtsList)
   {
      decimal dug, pot;
      string currTicker;

      int startRowIdx   = TheG.CurrentRow.Index;
      int currentRowIdx = TheG.CurrentRow.Index;

      currTicker = TheG.GetStringCell(ci.iT_ticker, startRowIdx, false);

      foreach(OtsTipBrGroupInfo otsInfo in choosenOtsList)
      {
         if(currentRowIdx != startRowIdx)
         {
            TheG.Rows.Insert(currentRowIdx, 1);
         }

         if(otsInfo.IsKupac)
         {
            pot = otsInfo.UkSaldo;
            dug = 0.00M;
         }
         else
         {
            dug = otsInfo.UkSaldo;
            pot = 0.00M;
         }

         PutDGVRowManually(currentRowIdx, otsInfo.Konto, otsInfo.OpenDokumentOpis, currTicker, otsInfo.KupdobCd, otsInfo.FakRecID, otsInfo.FakYear, otsInfo.TipBr, otsInfo.ProjektCD, otsInfo.MtrosCD, otsInfo.OpenDokumentValuta, dug, pot);

         currentRowIdx++;
      }
   }

   #endregion DumpChosenOtsList

   #region PdvKnjiga

   private string GetOneLetter4PdvKnjiga(ZXC.PdvKnjigaEnum pdvKnjigaEnum)
   {
      switch(pdvKnjigaEnum)
      {
         case ZXC.PdvKnjigaEnum.REDOVNA : return "R";
         case ZXC.PdvKnjigaEnum.PREDUJAM: return "P";
         case ZXC.PdvKnjigaEnum.UVOZ_ROB: return "U";
         case ZXC.PdvKnjigaEnum.UVOZ_USL: return "S";

         default: return "";
      }
   }

   private ZXC.PdvKnjigaEnum GetPdvKnjigaEnumFromFirstLetter(string firtsLetter)
   {
      switch(firtsLetter)
      {
         case "R": return ZXC.PdvKnjigaEnum.REDOVNA;
         case "P": return ZXC.PdvKnjigaEnum.PREDUJAM;
         case "U": return ZXC.PdvKnjigaEnum.UVOZ_ROB;
         case "S": return ZXC.PdvKnjigaEnum.UVOZ_USL;

         default: return ZXC.PdvKnjigaEnum.NIJEDNA;
      }
   }

   #endregion PdvKnjiga

   #region VvCurrency converter

   internal ZXC.ValutaNameEnum ValutaNameInUse { get; set; }
   public bool         IsShowingConvertedMoney { get; set; }
   public decimal      DevTecaj                { get; set; }

   public override bool IsOkToInitiateThisAction(ZXC.WriteMode writeMode)
   {
      switch(writeMode)
      {
         case ZXC.WriteMode.Add:
         //case ZXC.WriteMode.Edit:
         //case ZXC.WriteMode.Delete:
         if(IsShowingConvertedMoney)
         {
            //ZXC.aim_emsg(MessageBoxIcon.Stop, "Prvo se, molim, vratite 'u Kune'.");
            //return false;
            TheVvTabPage.TheVvForm.RISK_ToggleKnDeviza(null, EventArgs.Empty);
            return true;
         }
         else return true;

         default: return true;
      }
   }

   internal decimal VvCurrency(decimal _money)
   {
      if(ValutaNameInUse         == ZXC.ValutaNameEnum.EMPTY ||
         ValutaNameInUse         == /*ZXC.ValutaNameEnum.HRK*/ZXC.EURorHRK_NameEnum   ||
         IsShowingConvertedMoney == false)
      {
         return _money;
      }
      else if(TheVvTabPage.WriteMode != ZXC.WriteMode.None)
      {
         return _money;
      }
      else
      {
         if(IsShowingConvertedMoney) // daj kune u devizu 
         {
            return ZXC.DivSafe(_money, DevTecaj);
         }
         else // daj devizu u kune
         {
            return (_money * DevTecaj);
         }
      }
   }

   #endregion VvCurrency converter
}

public class NalogFirmaDUC : NalogDUC
{
   public NalogFirmaDUC(Control parent, Nalog _nalog, VvForm.VvSubModul vvSubModul) : base(parent, _nalog, vvSubModul)
   {
   }

   protected override void InitializeDUC_Specific_Columns()
   {
      
      T_konto_CreateColumn(ZXC.Q3un + ZXC.Qun4);

      T_opis_CreateColumn(/*Fill*/);

      T_pdv_CreateColumn(ZXC.QUN, KSD.Dsc_IsVisibleColObrpdv);
      T_037_CreateColumn(ZXC.QUN, KSD.Dsc_IsVisibleColObr037);


      T_progAktiv_CreateColumn(ZXC.Q3un - ZXC.Qun2, KSD.Dsc_IsVisibleColProgAkt );
      T_fond_CreateColumn     (ZXC.Q2un - ZXC.Qun4, KSD.Dsc_IsVisibleColFond    );
      T_fondName_CreateColumn (ZXC.Q3un - ZXC.Qun2, KSD.Dsc_IsVisibleColFondName);
      
      T_pozicija_CreateColumn    (ZXC.Q4un, KSD.Dsc_IsVisibleColPozicija);
      T_pozicijaName_CreateColumn(ZXC.Q6un, KSD.Dsc_IsVisibleColPozName );

      T_kdName_CreateColumn(ZXC.Q6un);

      T_ticker_CreateColumn(ZXC.Q3un);

      T_kupdob_cd_CreateColumn(ZXC.Q3un - ZXC.Qun2);

      T_tipBr_CreateColumn    (ZXC.Q5un);

      T_valuta_CreateColumn(ZXC.Q4un);
      T_projekt_CreateColumn(ZXC.Q5un, KSD.Dsc_IsVisibleColProjekt);

      T_mtros_cd_CreateColumn(ZXC.Q3un - ZXC.Qun2, KSD.Dsc_IsVisibleColMtrosCD);
      T_mtros_tk_CreateColumn(ZXC.Q3un, KSD.Dsc_IsVisibleColMtrosTK);

      T_dug_CreateColumn(ZXC.Q5un/*+ZXC.Qun2*/, 2);
                                 
      T_pot_CreateColumn(ZXC.Q5un/*+ZXC.Qun2*/, 2);

      T_fakRecID_CreateColumn(this.TheG);
      T_fakYear_CreateColumn (this.TheG);
      T_otsKind_CreateColumn (this.TheG);
      
      ColumnForScroll_CreateColumn(ZXC.QUN);
   }

   protected override void SetFilterUcDependentDefaults()
   {
      TheNalogTemplateFilter.NalogTemplateID = NalogTemplateFilter.PrintTemplateID.Firma;
   }
}

public class NalogObrtDUC  : NalogDUC
{
   public NalogObrtDUC(Control parent, Nalog _nalog, VvForm.VvSubModul vvSubModul) : base(parent, _nalog, vvSubModul)
   {
   }

   protected override void InitializeDUC_Specific_Columns()
   {
      
      T_konto_CreateColumn(ZXC.Q4un);

      T_opis_CreateColumn(/*Fill*/);

      T_pdv_CreateColumn(ZXC.QUN, true);

      T_037_CreateColumn(ZXC.QUN, true);

//      T_pdvKnjiga_CreateColumn(ZXC.QUN, true);

      T_kdName_CreateColumn(ZXC.Q6un);

      T_ticker_CreateColumn(ZXC.Q3un);

      T_kupdob_cd_CreateColumn(ZXC.Q3un - ZXC.Qun2);

      T_tipBr_CreateColumn(ZXC.Q5un);

      T_valuta_CreateColumn(ZXC.Q4un);

      T_dug_CreateColumn(ZXC.Q5un, 2);

      T_pot_CreateColumn(ZXC.Q5un, 2);

      T_fakRecID_CreateColumn(this.TheG);
      T_fakYear_CreateColumn (this.TheG);
      T_otsKind_CreateColumn (this.TheG);
      ColumnForScroll_CreateColumn(ZXC.QUN);
   }

   protected override void SetFilterUcDependentDefaults()
   {
      TheNalogTemplateFilter.NalogTemplateID = NalogTemplateFilter.PrintTemplateID.Obrt;
   }
}

public class NalogMtrDUC   : NalogDUC
{
   public NalogMtrDUC(Control parent, Nalog _nalog, VvForm.VvSubModul vvSubModul) : base(parent, _nalog, vvSubModul)
   {
   }

   protected override void InitializeDUC_Specific_Columns()
   {
      T_konto_CreateColumn(ZXC.Q4un);

      T_opis_CreateColumn(/*Fill*/);

      T_kdName_CreateColumn(ZXC.Q6un);

      T_ticker_CreateColumn(ZXC.Q3un);

      T_kupdob_cd_CreateColumn(ZXC.Q3un - ZXC.Qun2);

      T_tipBr_CreateColumn(ZXC.Q5un);

      T_valuta_CreateColumn(ZXC.Q4un);

      T_mtros_cd_CreateColumn(ZXC.Q3un, true);

      T_mtros_tk_CreateColumn(ZXC.Q3un, true);

      T_dug_CreateColumn(ZXC.Q5un, 2);

      T_pot_CreateColumn(ZXC.Q5un, 2);
   
      T_fakRecID_CreateColumn(this.TheG);
      T_fakYear_CreateColumn(this.TheG);

      T_otsKind_CreateColumn(this.TheG);
  
      ColumnForScroll_CreateColumn(ZXC.QUN);
   }

   protected override void SetFilterUcDependentDefaults()
   {
      TheNalogTemplateFilter.NalogTemplateID = NalogTemplateFilter.PrintTemplateID.MjTr;
   }
}

public class NalogProjektDUC : NalogDUC
{
   public NalogProjektDUC(Control parent, Nalog _nalog, VvForm.VvSubModul vvSubModul) : base(parent, _nalog, vvSubModul)
   {
   }

   protected override void InitializeDUC_Specific_Columns()
   {
      T_konto_CreateColumn(ZXC.Q4un);

      T_opis_CreateColumn(/*Fill*/);

      T_kdName_CreateColumn(ZXC.Q6un);

      T_ticker_CreateColumn(ZXC.Q3un);

      T_kupdob_cd_CreateColumn(ZXC.Q3un - ZXC.Qun2);

      T_tipBr_CreateColumn(ZXC.Q5un);

      T_valuta_CreateColumn(ZXC.Q4un);

      T_projekt_CreateColumn(ZXC.Q5un, true);
      
      T_dug_CreateColumn(ZXC.Q5un, 2);

      T_pot_CreateColumn(ZXC.Q5un, 2);

      T_fakRecID_CreateColumn(this.TheG);
      T_fakYear_CreateColumn(this.TheG);

      T_otsKind_CreateColumn(this.TheG);

      ColumnForScroll_CreateColumn(ZXC.QUN);
   }

   protected override void SetFilterUcDependentDefaults()
   {
      TheNalogTemplateFilter.NalogTemplateID = NalogTemplateFilter.PrintTemplateID.Projekt;
   }

}


