using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;


public class AmortDUC : VvDocumentRecordUC
{
   #region Fieldz
   
   private Amort    amort_rec;
   private Atrans   dgvAtrans_rec;

   private VvHamper zaglavljeHamper;

   private VvTextBox tbx_brNaloga, tbx_datum, tbx_napomena, tbx_TT,
                     tbx_TtOpis, tbx_TtNum;

   private VvDateTimePicker dTP_datum;
   private int nextX = 0, nextY = 0, razmakHamp = ZXC.Qun10;

   private VvTextBox /*vvtbT_serial,*/ vvtbT_osredCD, vvtbT_naziv, vvtbT_opis, vvtbT_kol,
                     vvtbT_koef_am, vvtbT_amort_st, vvtbT_dug, vvtbT_pot;

   private VvTextBoxColumn colVvText;
   private DataGridViewTextBoxColumn colScrol;

   #endregion Fieldz

   #region Constructor

   public AmortDUC(Control parent, Amort _amort, VvSubModul vvSubModul)
   {
      amort_rec = _amort;

      ThePrefferedRecordSorter = VirtualDataRecord.DefaultSorter;

      this.TheSubModul = vvSubModul;

      SuspendLayout();

      TheTabControl.TabPages.Add(CreateVvInnerTabPages("Amort", "", ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));

      InitializeZaglavljeHamper(out zaglavljeHamper);
      nextX = zaglavljeHamper.Right;

      nextY = zaglavljeHamper.Bottom;

      TheG       = CreateVvDataGridView(TheTabControl.TabPages[0], "Atrans Grid");

      TheSumGrid = CreateSumGrid(TheG, TheTabControl.TabPages[0], "SUM Atrans Grid");

      InitializeTheGrid_Columns();

      InitializeTheSUMGrid_Columns(TheG);

      TheG.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged_SumGrid);


      InitializeVvUserControl(parent);

      CalcLocationSizeAnchor_TheDGV(TheG, nextX, nextY, zaglavljeHamper.Width);

      CalcLocationSizeAnchor_GridSum(TheG);

      ResumeLayout();

      SetNalogColumnIndexes();

      CreateAmortDokumentPrintUC(this);

   }

   #endregion Constructor

   #region ZaglavljeHamper

   private void InitializeZaglavljeHamper(out VvHamper zaglavljeHamper)
   {
      zaglavljeHamper = new VvHamper(9, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      //zaglavljeHamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q5un, ZXC.Q4un, ZXC.Q5un, ZXC.Q4un + ZXC.QUN - ZXC.Qun8, ZXC.Q2un + ZXC.Qun4, ZXC.Q6un, ZXC.Q3un - ZXC.Qun2, 0 };
      zaglavljeHamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q5un, ZXC.Q4un, ZXC.Q5un, ZXC.Q4un + ZXC.QUN - ZXC.Qun8, ZXC.Q2un + ZXC.Qun4, ZXC.Q6un, ZXC.Q3un - ZXC.Qun2, 0 };
      zaglavljeHamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      zaglavljeHamper.VvRightMargin = zaglavljeHamper.VvLeftMargin;

      //int width = 792 - nextX - zaglavljeHamper.Width - zaglavljeHamper.VvRightMargin - zaglavljeHamper.VvLeftMargin; //22.11.2010. ode u minus
      //zaglavljeHamper.VvColWdt[8] = width; // zbog poradi scrolla koji se pojavljije na datagridu

      zaglavljeHamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN };
      zaglavljeHamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun2 };
      zaglavljeHamper.VvBottomMargin = zaglavljeHamper.VvTopMargin;

      Label lbBrNal, lbVrsKnj, lbDatum, lbNapom;

      lbBrNal  = zaglavljeHamper.CreateVvLabel(0, 0, "Broj transakcije:", ContentAlignment.MiddleRight);
      lbDatum  = zaglavljeHamper.CreateVvLabel(2, 0, "Datum:"           , ContentAlignment.MiddleRight);
      lbVrsKnj = zaglavljeHamper.CreateVvLabel(4, 0, "Vrsta trans.:"    , ContentAlignment.MiddleRight);
      lbNapom  = zaglavljeHamper.CreateVvLabel(0, 1, "Napomena:"        , ContentAlignment.MiddleRight);

      tbx_brNaloga                   = zaglavljeHamper.CreateVvTextBox(1, 0, "tbx_brNaloga", "Ovo bolje ostavi kako je...", 6);
      tbx_brNaloga.JAM_CharEdits     = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_brNaloga.JAM_FillCharacter = '0';
      tbx_brNaloga.JAM_DataRequired  = true;

      this.ControlForInitialFocus = tbx_brNaloga;
      tbx_brNaloga.JAM_StatusText = "Ovo bolje ostavi kako je...";
      
      tbx_datum       = zaglavljeHamper.CreateVvTextBox(3, 0, "tbx_datum", "Datum");
      tbx_datum.JAM_IsForDateTimePicker = true;

      dTP_datum = zaglavljeHamper.CreateVvDateTimePicker(3, 0, "", tbx_datum);
      dTP_datum.Name      = "dTP_datum";

      tbx_TT = zaglavljeHamper.CreateVvTextBoxLookUp(5, 0, "tbx_TT", "Tip transakcije");
      tbx_TT.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_TT.JAM_DataRequired = true;
      tbx_TT.JAM_MustTabOutBeforeSubmit = true;

      tbx_TtOpis = zaglavljeHamper.CreateVvTextBox(6, 0, "tbx_VKOpis_InVisible", "");
      tbx_TtOpis.JAM_ReadOnly = true;

      tbx_TtNum = zaglavljeHamper.CreateVvTextBox(7, 0, "tbx_TtNum", "Ovo bolje ostavi kako je...", 4);
      tbx_TtNum.JAM_CharEdits  = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_TtNum.JAM_FillCharacter = '0';
      tbx_TtNum.JAM_DataRequired = true;

      tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaAmortTT, (int)ZXC.Kolona.prva);
      tbx_TT.JAM_lui_NameTaker_JAM_Name = tbx_TtOpis.JAM_Name;
      tbx_TT.JAM_ttNumTaker_JAM_Name    = tbx_TtNum.JAM_Name;

      tbx_napomena = zaglavljeHamper.CreateVvTextBox(1, 1, "tbx_napomena", "Opis knjizenja / napomena.", 80/* GetDB_ColumnSize(DB_ci.napomena)*/, 6, 0);
   }

   #endregion ZaglavljeHamper

   #region TheGrid_Columns

   public void InitializeTheGrid_Columns()
   {
      T_osredCD_CreateColumn (ZXC.Q5un);

      T_naziv_CreateColumn(/*Fill*/);
      
      T_opis_CreateColumn    (ZXC.Q8un);
      
      T_kol_CreateColumn     (ZXC.Q2un, 0);
      
      T_koef_am_CreateColumn (ZXC.Q2un);
      
      T_amort_st_CreateColumn(ZXC.Q3un, 2);
      
      T_dug_CreateColumn     (ZXC.Q5un, 2);
      
      T_pot_CreateColumn     (ZXC.Q5un, 2);
     
      ColumnForScroll_CreateColumn(ZXC.QUN);
   }
   
   private void T_osredCD_CreateColumn(int _width)
   {
      vvtbT_osredCD = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_osredCD", TheVvDaoTrans, DB_Tci.t_osredCD, "Šifra dugotrajne imovine");
      vvtbT_osredCD.JAM_SetAutoCompleteData(Osred.recordName, Osred.sorterCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Osred_sorterSifra), new EventHandler(AnyOsredTextBoxLeave));
      vvtbT_osredCD.JAM_DataRequired = true;

      colVvText     = TheG.CreateVvTextBoxColumn(vvtbT_osredCD, TheVvDaoTrans, DB_Tci.t_osredCD, "Šifra", _width);
   }

   private void T_naziv_CreateColumn()
   {
      int nazivMaxLen = ZXC.OsredDao.GetSchemaColumnSize(ZXC.OsrCI.naziv);

      vvtbT_naziv = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_naziv", TheVvDaoTrans, (-1 * nazivMaxLen), "Naziv dugotrajne imovine");
      vvtbT_naziv.JAM_SetAutoCompleteData(Osred.recordName, Osred.sorterNaziv.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Osred_sorterName), new EventHandler(AnyOsredTextBoxLeave));

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_naziv, TheVvDaoTrans, "T_naziv", "Naziv", 0);
      colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      colVvText.MinimumWidth = ZXC.Q8un;
   }

   private void T_opis_CreateColumn(int _width)
   {
      vvtbT_opis = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_opis", TheVvDaoTrans, DB_Tci.t_opis, "Tekstualni opis knjiženja.");

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_opis, TheVvDaoTrans, DB_Tci.t_opis, "Opis", _width);
      //colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      //colVvText.MinimumWidth = 50;
   }

   private void T_kol_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_kol = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_kol", TheVvDaoTrans, DB_Tci.t_kol, "Količina");
      vvtbT_kol.JAM_FieldExitMethod = new EventHandler(OnExitKol);

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_kol, TheVvDaoTrans, DB_Tci.t_kol, "Kol", _width);
   }

   private void T_koef_am_CreateColumn(int _width)
   {
      vvtbT_koef_am = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_koef_am", TheVvDaoTrans, DB_Tci.t_koef_am, "Koeficijent amortizacije");
      vvtbT_koef_am.JAM_ReadOnly = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_koef_am, TheVvDaoTrans, DB_Tci.t_koef_am, "KAm", _width);
      colVvText.DefaultCellStyle.Alignment =
      colVvText.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
}

   private void T_amort_st_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_amort_st = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_amort_st", TheVvDaoTrans, DB_Tci.t_amort_st, "Stopa amortizacije");
      vvtbT_amort_st.JAM_ReadOnly = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_amort_st, TheVvDaoTrans, DB_Tci.t_amort_st, "Stopa Am", _width);
   }

   private void T_dug_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_dug = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_dug", TheVvDaoTrans, DB_Tci.t_dug, "Iznos DUGUJE");
      vvtbT_dug.JAM_ShouldSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dug, TheVvDaoTrans, DB_Tci.t_dug, "Duguje", _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }

   private void T_pot_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_pot = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_pot", TheVvDaoTrans, DB_Tci.t_pot, "Iznos POTRAZUJE");
      vvtbT_pot.JAM_ShouldSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_pot, TheVvDaoTrans, DB_Tci.t_pot, "Potrazuje", _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }

   protected void ColumnForScroll_CreateColumn(int _width)
   {
      colScrol = TheG.CreateScrollColumn("scrol", _width);
      colScrol.ReadOnly = true;
   }

   private void AnyOsredTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      Osred osred_rec;
      OsredStatus osrStat_rec;

      VvTextBoxEditingControl vvtb_editingControl = sender as VvTextBoxEditingControl;

      if(vvtb_editingControl == null) return;

      if(vvtb_editingControl.Text == this.originalText) return;

      this.originalText = vvtb_editingControl.Text;
      osred_rec = OsredSifrar.Find(FoundInSifrar<Osred>);

      int currRow = vvtb_editingControl.EditingControlRowIndex;

      if(osred_rec != null && vvtb_editingControl.Text != "")
      {
         TheG.PutCell(ci.iT_osredCD , currRow, osred_rec.OsredCD);
         TheG.PutCell(ci.iT_naziv   , currRow, osred_rec.Naziv);
         TheG.PutCell(ci.iT_amort_st, currRow, osred_rec.AmortSt);
         TheG.PutCell(ci.iT_koef_am , currRow, osred_rec.KoefAm);

         if(Fld_TT == Amort.RASHOD_TT)
         {
            osrStat_rec = OsredDao.GetOsredStatus(TheDbConnection, osred_rec.OsredCD, ZXC.AmortRazdoblje./*NOW*/GODINA, Fld_DokDate);

            TheG.PutCell(ci.iT_kol, currRow, osrStat_rec.KolSt);
            TheG.PutCell(ci.iT_dug, currRow, osrStat_rec.UkDugS * -1.00M);
            TheG.PutCell(ci.iT_pot, currRow, osrStat_rec.UkPotS * -1.00M);
         }
      }
      else
      {
         TheG.PutCell(ci.iT_osredCD , currRow, "");
         TheG.PutCell(ci.iT_naziv   , currRow, "");
         TheG.PutCell(ci.iT_amort_st, currRow, 0.00M);
         TheG.PutCell(ci.iT_koef_am , currRow, "");

         if(Fld_TT == Amort.RASHOD_TT)
         {
            TheG.PutCell(ci.iT_kol, currRow, 0.00M);
            TheG.PutCell(ci.iT_dug, currRow, 0.00M);
            TheG.PutCell(ci.iT_pot, currRow, 0.00M);
         }
      }

      // samo za DUC-eve
      ZXC.TheVvForm.SetDirtyFlag(sender);
   }

   public void OnExitKol(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      if(Fld_TT != Amort.RASHOD_TT) return;

      VvTextBoxEditingControl vvTbKol = sender as VvTextBoxEditingControl;

      if(vvTbKol.EditedHasChanges() == false) return;

      int rIdx = TheG.CurrentRow.Index;

      Atrans atrans_rec = (Atrans)GetDgvLineFields(rIdx, false, null); // da napuni Document's business.Transes 

      DialogResult result = MessageBox.Show("Da li zaista zelite revalorizirati financijski rashod s obzirom na kolicinski\n\n" +
                                             "za osnovno sredstvo: [" + atrans_rec.T_osredCD + "]\n\n",
                                             " Potvrdite revalorizaciju rashoda?!",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Warning);

      if(result != DialogResult.Yes) return;

      decimal oldKol;
      decimal oldDug;
      decimal oldPot;

      decimal newKol;
      decimal newDug;
      decimal newPot;

      OsredStatus osrStat_rec = OsredDao.GetOsredStatus(TheDbConnection, atrans_rec.T_osredCD, ZXC.AmortRazdoblje./*NOW*/GODINA, Fld_DokDate);

      oldKol = osrStat_rec.KolSt          ;
      oldDug = osrStat_rec.UkDugS * -1.00M;
      oldPot = osrStat_rec.UkPotS * -1.00M;

      newKol = atrans_rec.T_kol;

      decimal ratio = ZXC.DivSafe(newKol, oldKol);

      newDug = oldDug * ratio;
      newPot = oldPot * ratio;

      TheG.PutCell(ci.iT_dug, rIdx, newDug);
      TheG.PutCell(ci.iT_pot, rIdx, newPot);

   }

   #endregion TheGrid_Columns

   #region SetNalogColumnIndexes()

   /// <summary>
   /// Column Index na DataGridView-u (NE U DataBase to ti je 'DB_ci')
   /// </summary>
   private Amort_colIdx ci;
   public  Amort_colIdx DgvCI { get { return ci; } }

   public struct Amort_colIdx
   {
      internal int iT_recID   ;
      internal int iT_serial  ;
      internal int iT_osredCD ;
      internal int iT_naziv   ;
      internal int iT_opis    ;
      internal int iT_kol     ;
      internal int iT_koef_am ;
      internal int iT_amort_st;
      internal int iT_dug     ;
      internal int iT_pot     ;
   }

   private void SetNalogColumnIndexes()
   {
      ci = new Amort_colIdx();

      ci.iT_recID    = TheG.IdxForColumn("T_recID"   );
      ci.iT_serial   = TheG.IdxForColumn("T_serial"  );
      ci.iT_osredCD  = TheG.IdxForColumn("T_osredCD" );
      ci.iT_naziv    = TheG.IdxForColumn("T_naziv"   );
      ci.iT_opis     = TheG.IdxForColumn("T_opis"    );
      ci.iT_kol      = TheG.IdxForColumn("T_kol"     );
      ci.iT_koef_am  = TheG.IdxForColumn("T_koef_am" );
      ci.iT_amort_st = TheG.IdxForColumn("T_amort_st");
      ci.iT_dug      = TheG.IdxForColumn("T_dug"     );
      ci.iT_pot      = TheG.IdxForColumn("T_pot"     );

   }

   #endregion SetNalogColumnIndexes()

   #region Fld_

   public uint Fld_DokNum
   {
      get { return ZXC.ValOrZero_UInt(tbx_brNaloga.Text); }
      set { tbx_brNaloga.Text = value.ToString("000000"); }
   }

   public string Fld_TT
   {
      get { return tbx_TT.Text; }
      set { tbx_TT.Text = value; }
   }

   public string Fld_TipTranOpis
   {
      set { tbx_TtOpis.Text = value; }
   }

   public DateTime Fld_DokDate
   {
      get { return  dTP_datum.Value;         }
      set 
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dTP_datum.Value = value;
         }
      }
}

   public string Fld_DokDateAsTxt
   {
      get { return tbx_datum.Text; }
      set { tbx_datum.Text = value; }
   }

   public string Fld_Napomena
   {
      get { return tbx_napomena.Text; }
      set { tbx_napomena.Text = value; }
   }

   public uint Fld_TTNum
   {
      get { return ZXC.ValOrZero_UInt(tbx_TtNum.Text); }
      set { tbx_TtNum.Text = value.ToString("0000"); }

   }

   #endregion Fld_

   #region PutFields(), GetFields()

   public override void PutFields(VvDataRecord amort)
   {
      amort_rec = (Amort)amort;
      if(amort_rec != null)
      {

         PutMetaFileds(amort_rec.AddUID, amort_rec.AddTS, amort_rec.ModUID, amort_rec.ModTS, amort_rec.RecID, amort_rec.LanSrvID, amort_rec.LanRecID);

         PutIdentityFields(amort_rec.DokNum.ToString("000000"), amort_rec.DokDate.ToString(ZXC.VvDateFormat), amort_rec.TT, "");

         Fld_DokNum       = amort_rec.DokNum;
         Fld_DokDateAsTxt = amort_rec.DokDate.ToString(ZXC.VvDateFormat);
         Fld_DokDate      = amort_rec.DokDate;
         Fld_Napomena     = amort_rec.Napomena;
         Fld_TT           = amort_rec.TT;
         Fld_TTNum        = amort_rec.TtNum;
         Fld_TipTranOpis  = ZXC.luiListaAmortTT.GetNameForThisCd(amort_rec.TT);

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
      SetFilterRecordDependentDefaults(); // filter.DokNumOd = amort_rec.DokNum (punimo bussiness od filtera, ne UC)
      TheAmortDokumentFilterUC.PutFilterFields(TheAmortDokumentFilter);
   }

   public override void GetFields(bool dirtyFlagging)
   {
      amort_rec.DokNum   = Fld_DokNum;
      amort_rec.DokDate  = Fld_DokDate;
      amort_rec.Napomena = Fld_Napomena;
      amort_rec.TT       = Fld_TT;
      amort_rec.TtNum    = Fld_TTNum;

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
      Fld_TTNum = ttNum;
   }

   #endregion Put_NewDocum_NumAndDateFields

   #endregion PutFields(), GetFields()

   #region PutDgvFields(), GetDgvFields()

   private void PutDgvFields()
   {
      int rowIdx, idxCorrector;

      TheG.RowsRemoved -= new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);
      TheG.RowsAdded   -= new DataGridViewRowsAddedEventHandler  (grid_RowsAdded);
      TheG.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(TheG);

      if(amort_rec.Transes != null)
      {
         SetSifrarAndAutocomplete<Osred>(null, VvSQL.SorterType.None);

         foreach(Atrans atrans_rec in amort_rec.Transes)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheG.Rows.Add();

            rowIdx = TheG.RowCount - idxCorrector;

            PutDgvLineFields(atrans_rec, rowIdx, false);
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
      Atrans atrans_rec = (Atrans)trans_rec;

      if(skipRecID_andSerial_Columns == false)
      {
         TheG.PutCell(ci.iT_recID,  rowIdx, atrans_rec.T_recID);
         TheG.PutCell(ci.iT_serial, rowIdx, atrans_rec.T_serial);
      }

      Osred osred_rec = OsredSifrar.SingleOrDefault(osred => osred.OsredCD == atrans_rec.T_osredCD);
      TheG.PutCell(ci.iT_osredCD, rowIdx, atrans_rec.T_osredCD);

      if(osred_rec != null) TheG.PutCell(ci.iT_naziv, rowIdx, osred_rec.Naziv);

      TheG.PutCell(ci.iT_opis,      rowIdx, atrans_rec.T_opis);
      TheG.PutCell(ci.iT_kol,       rowIdx, atrans_rec.T_kol);
      TheG.PutCell(ci.iT_koef_am,   rowIdx, atrans_rec.T_koefAm);
      TheG.PutCell(ci.iT_amort_st,  rowIdx, atrans_rec.T_amortSt);
      TheG.PutCell(ci.iT_dug,       rowIdx, atrans_rec.T_dug);
      TheG.PutCell(ci.iT_pot,       rowIdx, atrans_rec.T_pot);

      //ptrano_rec.CalcTransResults();

      //PutDgvLineResultsFields(rowIdx, atrans_rec);
   }

   public override void PutDgvTransSumFields()
   {
      TheSumGrid[ci.iT_kol, 0].Value = amort_rec.Sum_Kol;
      TheSumGrid[ci.iT_dug, 0].Value = amort_rec.Sum_Dug;
      TheSumGrid[ci.iT_pot, 0].Value = amort_rec.Sum_Pot;
   }

   private void GetDgvFieldsOLD(bool dirtyFlagging)
   {
      int    rIdx = 0;
      uint   recID;
      bool   DB_RWT;
      uint[] recIDtable;
      Atrans dgv_rec, db_rec;

      if(TheG.RowCount > 0) recIDtable = new uint[TheG.RowCount - 1];
      else recIDtable = null;

      amort_rec.DiscardPreviouslyAddedTranses();

      //foreach(DataGridViewRow gridRow in TheG.Rows)
      for(rIdx = 0; rIdx < TheG.RowCount - 1; ++rIdx)
      {
         dgv_rec = new Atrans();

         if(TheG.CI_OK(ci.iT_recID)) { recID = recIDtable[rIdx] = dgv_rec.T_recID = TheG.GetUint32Cell(ci.iT_recID, rIdx, dirtyFlagging); }
         else                        { recID = 0; ZXC.aim_emsg("!!! Column T_recID MISSING!!!"); }

         db_rec = null;
         if(recID > 0) db_rec = amort_rec.Transes.SingleOrDefault(vvDR => vvDR.VirtualRecID == recID);
         else          db_rec = null;

         DB_RWT = (db_rec != null);

         // dgvAtrans_rec: buffer za GetField iz DataGridWievRow-a                                        
         // db_rec : pointer na existing row u DB-u kojega ako je EditedHasChanges treba RWTREC-ati 

         dgv_rec.T_parentID = amort_rec.RecID;

         dgv_rec.T_serial = (ushort)(rIdx + 1);
         if(DB_RWT) db_rec.T_serial = dgv_rec.T_serial;

         dgv_rec.T_dokNum = amort_rec.DokNum;
         if(DB_RWT) db_rec.T_dokNum = dgv_rec.T_dokNum;

         dgv_rec.T_dokDate = amort_rec.DokDate;
         if(DB_RWT) db_rec.T_dokDate = dgv_rec.T_dokDate;

         dgv_rec.T_TT = amort_rec.TT;
         if(DB_RWT) db_rec.T_TT = dgv_rec.T_TT;

         if(TheG.CI_OK(ci.iT_osredCD))
         {
            dgv_rec.T_osredCD = TheG.GetStringCell(ci.iT_osredCD, rIdx, dirtyFlagging);
            if(DB_RWT) db_rec.T_osredCD = dgv_rec.T_osredCD;
         }
         if(TheG.CI_OK(ci.iT_opis))
         {
            dgv_rec.T_opis = TheG.GetStringCell(ci.iT_opis, rIdx, dirtyFlagging);
            if(DB_RWT) db_rec.T_opis = dgv_rec.T_opis;
         }
         if(TheG.CI_OK(ci.iT_kol))
         {
            dgv_rec.T_kol = TheG.GetDecimalCell(ci.iT_kol, rIdx, dirtyFlagging);
            if(DB_RWT) db_rec.T_kol = dgv_rec.T_kol;
         }
         if(TheG.CI_OK(ci.iT_koef_am))
         {
            dgv_rec.T_koefAm = TheG.GetStringCell(ci.iT_koef_am, rIdx, dirtyFlagging);
            if(DB_RWT) db_rec.T_koefAm = dgv_rec.T_koefAm;
         }
         if(TheG.CI_OK(ci.iT_amort_st))
         {
            dgv_rec.T_amortSt = TheG.GetDecimalCell(ci.iT_amort_st, rIdx, dirtyFlagging);
            if(DB_RWT) db_rec.T_amortSt = dgv_rec.T_amortSt;
         }
         if(TheG.CI_OK(ci.iT_dug))
         {
            dgv_rec.T_dug = TheG.GetDecimalCell(ci.iT_dug, rIdx, dirtyFlagging);
            if(DB_RWT) db_rec.T_dug = dgv_rec.T_dug;
         }
         if(TheG.CI_OK(ci.iT_pot))
         {
            dgv_rec.T_pot = TheG.GetDecimalCell(ci.iT_pot, rIdx, dirtyFlagging);
            if(DB_RWT) db_rec.T_pot = dgv_rec.T_pot;
         }
 

         if(dgv_rec.T_recID == 0) // ADDED NEW, FRESH Ftrans             
         {
            dgv_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;
            amort_rec.Transes.Add(dgv_rec);
         }
         else if(db_rec.EditedHasChanges())
         {
            db_rec.SaveTransesWriteMode = ZXC.WriteMode.Edit;
         }
         else
         {
            db_rec.SaveTransesWriteMode = ZXC.WriteMode.None;
         }

      } // for(rIdx=0; rIdx < TheG.Rows.Count - 1; ++rIdx) 

      MarkTransesToDelete(recIDtable);

   }

   private void GetDgvFields(bool dirtyFlagging)
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

      amort_rec.DiscardPreviouslyAddedTranses();

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
      Atrans db_rec;

      // dgvAtrans_rec: buffer za GetField iz DataGridWievRow-a                                        
      // db_rec : pointer na existing row u DB-u kojega ako je EditedHasChanges treba RWTREC-ati 

      recID = TheG.GetUint32Cell(ci.iT_recID, rIdx, dirtyFlagging);

      if(recIDtable != null) recIDtable[rIdx] = recID;

      dgvAtrans_rec = new Atrans();

      if(recID > 0) // Postojeci redak 
      {
         db_rec = amort_rec.Transes.SingleOrDefault(vvDR => vvDR.VirtualRecID == recID);
      }
      else // novododani redak 
      {
         db_rec = null;
      }

      DB_RWT = (db_rec != null);

      dgvAtrans_rec.T_recID = recID;

      dgvAtrans_rec.T_parentID = amort_rec.RecID;

      #region GetColumns

                                   dgvAtrans_rec.T_serial = (ushort)(rIdx + 1);
      if(DB_RWT) db_rec.T_serial = dgvAtrans_rec.T_serial;

                                   dgvAtrans_rec.T_dokNum = amort_rec.DokNum;
      if(DB_RWT) db_rec.T_dokNum = dgvAtrans_rec.T_dokNum;

                                    dgvAtrans_rec.T_dokDate = amort_rec.DokDate;
      if(DB_RWT) db_rec.T_dokDate = dgvAtrans_rec.T_dokDate;

                               dgvAtrans_rec.T_TT = amort_rec.TT;
      if(DB_RWT) db_rec.T_TT = dgvAtrans_rec.T_TT;

      if(TheG.CI_OK(ci.iT_osredCD))
      {
                                       dgvAtrans_rec.T_osredCD = TheG.GetStringCell(ci.iT_osredCD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_osredCD = dgvAtrans_rec.T_osredCD;
      }
      if(TheG.CI_OK(ci.iT_opis))
      {
                                    dgvAtrans_rec.T_opis = TheG.GetStringCell(ci.iT_opis, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_opis = dgvAtrans_rec.T_opis;
      }
      if(TheG.CI_OK(ci.iT_kol))
      {
                                   dgvAtrans_rec.T_kol = TheG.GetDecimalCell(ci.iT_kol, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_kol = dgvAtrans_rec.T_kol;
      }
      if(TheG.CI_OK(ci.iT_koef_am))
      {
                                      dgvAtrans_rec.T_koefAm = TheG.GetStringCell(ci.iT_koef_am, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_koefAm = dgvAtrans_rec.T_koefAm;
      }
      if(TheG.CI_OK(ci.iT_amort_st))
      {
                                       dgvAtrans_rec.T_amortSt = TheG.GetDecimalCell(ci.iT_amort_st, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_amortSt = dgvAtrans_rec.T_amortSt;
      }
      if(TheG.CI_OK(ci.iT_dug))
      {
                                   dgvAtrans_rec.T_dug = TheG.GetDecimalCell(ci.iT_dug, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_dug = dgvAtrans_rec.T_dug;
      }
      if(TheG.CI_OK(ci.iT_pot))
      {
                                   dgvAtrans_rec.T_pot = TheG.GetDecimalCell(ci.iT_pot, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_pot = dgvAtrans_rec.T_pot;
      }

      #endregion GetColumns

      if(dgvAtrans_rec.T_recID == 0) // ADDED NEW, FRESH Ftrans             
      {
         amort_rec.InvokeTransRemove(dgvAtrans_rec);

         dgvAtrans_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;

         amort_rec.Transes.Add(dgvAtrans_rec);
      }
      else if(db_rec.EditedHasChanges())
      {
         db_rec.SaveTransesWriteMode = ZXC.WriteMode.Edit;
      }
      else
      {
         db_rec.SaveTransesWriteMode = ZXC.WriteMode.None;
      }

      return dgvAtrans_rec;
   }

   #endregion PutDgvFields(), GetDgvFields()

   #region Overriders and specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.amort_rec; }
      set {        this.amort_rec = (Amort)value; }
   }

   public override VvDocumentRecord VirtualDocumentRecord
   {
      get { return this.VirtualDataRecord as VvDocumentRecord; }
      set {        this.VirtualDataRecord = (Amort)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.AmortDao; }
   }

   public override VvDaoBase TheVvDaoTrans
   {
      get { return ZXC.AtransDao; }
   }

   #region Document_Database_ColumnIndexes, Transes_Database_ColumnIndexes

   /// <summary>
   /// Column Index U DataBase (NE na DataGridView-u to ti je 'ci')
   /// </summary>
   private AmortDao.AmortCI DB_ci
   {
      get { return ZXC.AmoCI; }
   }

   /// <summary>
   /// Column Index u TRANS DataBase (NE na DataGridView-u to ti je 'ci')
   /// </summary>
   private AtransDao.AtransCI DB_Tci
   {
      get { return ZXC.AtrCI; }
   }

   #endregion Document_Database_ColumnIndexes, Transes_Database_ColumnIndexes

   #region Update_VvDataRecord (Legacy naming convention)

   public override VvFindDialog CreateVvFindDialog()
   {
      return CreateFind_Amort_Dialog();
   }

   public static VvFindDialog CreateFind_Amort_Dialog()
   {
      VvSubModul vvSubModul   = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsAMO);
      VvDataRecord      vvDataRecord = ZXC.TheVvForm.CreateTheVvDataRecord_SwitchSubModulEnum(vvSubModul);

      VvFindDialog vvFindDialog = new VvFindDialog();

      VvRecLstUC   vvRecListUC  = new AmortListUC(vvFindDialog, (Amort)vvDataRecord, vvSubModul);

      vvFindDialog.TheRecListUC = vvRecListUC;

      return vvFindDialog;
   }

   #endregion Update_VvDataRecord (Legacy naming convention)

   #region PrintDocumentRecord

   public AmortDokumentFilterUC TheAmortDokumentFilterUC { get; set; }
   public AmortDokumentFilter   TheAmortDokumentFilter   { get; set; }

   //public RptO_AmortDocument TheRptO_AmortDocument
   //{
   //   get;
   //   set;
   //}

   //public override VvReport VirtualReport
   //{
   //   get
   //   {
   //      return this.TheRptO_AmortDocument;
   //   }
   //}

   public override string VirtualReportName
   {
      get
      {
         return "TEMELJNICA DUGOTRAJNE IMOVINE";
      }
   }
 
   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      return new RptO_AmortDocument(reportName, (AmortDokumentFilter)vvRptFilter);
   }
 
   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheAmortDokumentFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheAmortDokumentFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
      TheAmortDokumentFilter.DokNumOd = amort_rec.DokNum;
   }

   #endregion PrintDocumentRecord

   public override VvLookUpLista TheTtLookUpList
   {
      get { return ZXC.luiListaAmortTT; }
   }


   #endregion Overriders and specifics

   #region CreateAmortDokumentPrintUC

   protected void CreateAmortDokumentPrintUC(VvUserControl vvUC)
   {
      this.TheAmortDokumentFilter = new AmortDokumentFilter();

      TheAmortDokumentFilterUC        = new AmortDokumentFilterUC(vvUC);
      TheAmortDokumentFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheAmortDokumentFilterUC.Width;

   }

   #endregion CreateAmortDokumentPrintUC

}

public class AmortizacijaDLG : VvDialog
{
   #region Fieldz

   private Button         okButton, cancelButton;
   private VvHamper       hamper;
   private int            dlgWidth, dlgHeight;
   private VvTextBox      tbx_opis, tbx_datum;
   private DateTimePicker dtp_datum;
   private ComboBox       cbx_razd;
   private CheckBox       chkBox_isNiv;

   #endregion Fieldz

   #region Constructor

   public AmortizacijaDLG()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Amortizacija dugotrajne imovine";

      CreateHamper();

      dlgWidth  = hamper.Right  + ZXC.QunMrgn; 
      dlgHeight = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      this.Load += new EventHandler(AmortizacijaDLG_Load);
   }

   #endregion Constructor

   #region Hamper

   private void CreateHamper()
   {
      hamper          = new VvHamper(3, 4, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q7un, ZXC.Q5un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN  , ZXC.QUN , ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun2, ZXC.Qun2, ZXC.Qun2, ZXC.Qun2 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbRazd, lbDat, lbOpis;

      lbRazd = hamper.CreateVvLabel(0, 0, "Obračunsko razdoblje:", ContentAlignment.MiddleRight);
      lbDat  = hamper.CreateVvLabel(0, 1, "Datum obračuna:"      , ContentAlignment.MiddleRight);
      lbOpis = hamper.CreateVvLabel(0, 2, "Opis:"                , ContentAlignment.MiddleRight);

      cbx_razd  = hamper.CreateVvComboBox      (1, 0,"", "cbx_razdoblje", ComboBoxStyle.DropDownList);
      
    // 12.12.2017: ovaj dtp_datum nije imao vvTbx
    //dtp_datum = hamper.CreateVvDateTimePicker(1, 1, "", null);
      tbx_datum = hamper.CreateVvTextBox       (1, 1, "tbx_dokDate", "Datum dokumenta");
      tbx_datum.JAM_IsForDateTimePicker = true;
      dtp_datum = hamper.CreateVvDateTimePicker(1, 1, "", tbx_datum);
      dtp_datum.Name = "dtp_datum";
      
      
      tbx_opis  = hamper.CreateVvTextBox       (1, 2, "tbx_opis", "", 32, 1, 0);

            
      //cbx_razd.Items.AddRange(new object[] { "Godina", "Kvartal", "Mjesec" });
      cbx_razd.DataSource = Enum.GetValues(typeof(ZXC.AmortRazdoblje));
      //cbx_razd.SelectedIndex = 0;
      cbx_razd.SelectedItem = ZXC.AmortRazdoblje.GODINA;

      chkBox_isNiv = hamper.CreateVvCheckBox_OLD(0, 3, null, "Nivelacija konverzije", RightToLeft.No);

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);

   }

   #endregion Hamper

   #region Eveniti

   private void AmortizacijaDLG_Load(object sender, EventArgs e)
   {
      ZXC.VvDataBaseInfo vvDBinfo = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects;
      Fld_Datum = vvDBinfo.ProjectYearLastDay;
   }

   private void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Eveniti

   #region Fld_

   public DateTime Fld_Datum
   {
      get { return dtp_datum.Value;         }
      set {        dtp_datum.Value = value; }
   }

   public string Fld_Opis
   {
      get { return tbx_opis.Text;         }
      set {        tbx_opis.Text = value; }
   }

   public ZXC.AmortRazdoblje Fld_Razdoblje
   {
      get { return (ZXC.AmortRazdoblje)cbx_razd.SelectedItem; }
      set { cbx_razd.SelectedItem = value; }
   }

   public bool Fld_isNivelirajEURoNeravnotezu
   {
      get { return chkBox_isNiv.Checked;         }
      set {        chkBox_isNiv.Checked = value; }
   }

   #endregion Fld_

}

public class InventuraDLG : VvDialog
{
   #region Fieldz

   private Button         okButton, cancelButton;
   private VvHamper       hamper;
   private int            dlgWidth, dlgHeight;
   private VvTextBox      tbx_opis;
   private DateTimePicker dtp_datum;

   #endregion Fieldz

   #region Constructor

   public InventuraDLG()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Inventura dugotrajne imovine";

      CreateHamper();

      dlgWidth  = hamper.Right  + ZXC.QunMrgn; 
      dlgHeight = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

   }

   #endregion Constructor

   #region Hamper

   private void CreateHamper()
   {
      hamper          = new VvHamper(3, 2, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q5un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun2, ZXC.Qun2};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbDat, lbOpis;

      lbDat  = hamper.CreateVvLabel(0, 0, "Datum obračuna:", ContentAlignment.MiddleRight);
      lbOpis = hamper.CreateVvLabel(0, 1, "Opis:"          , ContentAlignment.MiddleRight);

      dtp_datum = hamper.CreateVvDateTimePicker(1, 0, "", null);
      tbx_opis  = hamper.CreateVvTextBox       (1, 1, "tbx_opis", "", 32, 1, 0);

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);

   }

   #endregion Hamper

   #region Eveniti

   private void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Eveniti

   #region Fld_

   public DateTime Fld_Datum
   {
      get { return dtp_datum.Value;         }
      set {        dtp_datum.Value = value; }
   }

   public string Fld_Opis
   {
      get { return tbx_opis.Text;         }
      set {        tbx_opis.Text = value; }
   }

   #endregion Fld_
}

public class InventurnoStanjeDLG : VvDialog
{
   #region Fieldz

   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_stanje,tbx_OsredCd, tbx_Naziv;

   #endregion Fieldz

   #region Constructor

   public InventurnoStanjeDLG()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Unesite inventurno stanje";

      CreateHamper();

      dlgWidth  = hamper.Right  + ZXC.QunMrgn; 
      dlgHeight = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

   }

   #endregion Constructor

   #region Hamper

   private void CreateHamper()
   {
      hamper          = new VvHamper(3, 3, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.QunBtnW, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4   , ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN, ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4,ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      
      Label lbCd  = hamper.CreateVvLabel(0, 0, "Šifra:", ContentAlignment.MiddleRight);
      Label lbNaz = hamper.CreateVvLabel(0, 1, "Naziv:", ContentAlignment.MiddleRight);
      Label lbkol = hamper.CreateVvLabel(0, 2, "Količina:", ContentAlignment.MiddleRight);

      tbx_OsredCd = hamper.CreateVvTextBox(1, 0, "tbx_OsredCd", "", 32, 1, 0);
      tbx_Naziv   = hamper.CreateVvTextBox(1, 1, "tbx_Naziv"  , "", 32, 1, 0);
      tbx_stanje  = hamper.CreateVvTextBox(1, 2, "tbx_stanje" , "");

      tbx_OsredCd.JAM_ReadOnly = tbx_Naziv.JAM_ReadOnly = true;

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);

   }

   #endregion Hamper

   #region Eveniti

   private void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Eveniti

   #region Fld_

   public string Fld_OsredCd
   {
      get { return tbx_OsredCd.Text; }
      set {        tbx_OsredCd.Text = value; }
   }

   public string Fld_Naziv
   {
      get { return tbx_Naziv.Text; }
      set {        tbx_Naziv.Text = value; }
   }

   public decimal Fld_InvStanje
   {
      get { return tbx_stanje.GetDecimalField(); }
      set {        tbx_stanje.PutDecimalField(value); }
   }

   #endregion Fld_

}
