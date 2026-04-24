using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;


public class DevTecUC : VvDocumentRecordUC
{
   #region Fieldz

   private DevTec2 devTec_rec;
   private Htrans2 dgvHtrans_rec;

   private VvHamper zaglavljeHamper;

   private VvTextBox tbx_dokNum, tbx_dokDate, tbx_napomena, tbx_TT,
                     tbx_TtOpis, tbx_TtNum, tbx_dateCreated, tbx_extDokNum;

   private VvDateTimePicker dTP_dokDate, dTP_dateCreated;
   private int nextX = 0, nextY = 0, razmakHamp = ZXC.Qun10;

   private VvTextBox vvtbT_valName, vvtbT_valNazivSifra, vvtbT_valJedinica, vvtbT_kupovni, vvtbT_srednji, vvtbT_prodajni;

   private VvTextBoxColumn colVvText;

   #endregion Fieldz

   #region Constructor

   public DevTecUC(Control parent, DevTec2 _devTec, VvSubModul vvSubModul)
   {
      devTec_rec = _devTec;

      ThePrefferedRecordSorter = VirtualDataRecord.DefaultSorter;

      this.TheSubModul = vvSubModul;

      SuspendLayout();

      TheTabControl.TabPages.Add(CreateVvInnerTabPages("DevTec", "", ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));

      InitializeZaglavljeHamper(out zaglavljeHamper);
      nextX = zaglavljeHamper.Right;

      nextY = zaglavljeHamper.Bottom;

      TheG = CreateVvDataGridView(TheTabControl.TabPages[0], "Htrans Grid");

      TheSumGrid = CreateSumGrid(TheG, TheTabControl.TabPages[0], "SUM Htrans Grid");

      InitializeTheGrid_Columns();

      InitializeTheSUMGrid_Columns(TheG);

      TheG.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged_SumGrid);


      InitializeVvUserControl(parent);

      CalcLocationSizeAnchor_TheDGV(TheG, nextX, nextY, zaglavljeHamper.Width);

      CalcLocationSizeAnchor_GridSum(TheG);

      ResumeLayout();

      SetNalogColumnIndexes();

      CreateDevTecDokumentPrintUC(this);

   }

   #endregion Constructor

   #region ZaglavljeHamper

   private void InitializeZaglavljeHamper(out VvHamper zaglavljeHamper)
   {
      zaglavljeHamper = new VvHamper(8, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      zaglavljeHamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un + ZXC.Qun4, ZXC.Q6un, ZXC.Q5un, ZXC.Q4un, ZXC.Q3un, ZXC.Q8un, ZXC.Q3un - ZXC.Qun2 };
      zaglavljeHamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4 };
      zaglavljeHamper.VvRightMargin = zaglavljeHamper.VvLeftMargin;

      //int width = 792 - nextX - zaglavljeHamper.Width - zaglavljeHamper.VvRightMargin - zaglavljeHamper.VvLeftMargin;
      //zaglavljeHamper.VvColWdt[8] = width; // zbog poradi scrolla koji se pojavljije na datagridu

      zaglavljeHamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN };
      zaglavljeHamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun2 };
      zaglavljeHamper.VvBottomMargin = zaglavljeHamper.VvTopMargin;

      Label lbdokNum, lbBanka, lbdokDate, lbNapom, lbDatumF, lbBrojT;

      lbdokNum   = zaglavljeHamper.CreateVvLabel  (0, 0, "Broj dokumenta:", ContentAlignment.MiddleRight);
      tbx_dokNum = zaglavljeHamper.CreateVvTextBox(1, 0, "tbx_dokNum", "Ovo bolje ostavi kako je...", 6);
      tbx_dokNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_dokNum.JAM_FillCharacter = '0';
      tbx_dokNum.JAM_DataRequired = true;

      this.ControlForInitialFocus = tbx_dokNum;


      lbdokDate   = zaglavljeHamper.CreateVvLabel  (2, 0, "Datum primjene tečaja:", ContentAlignment.MiddleRight);
      tbx_dokDate = zaglavljeHamper.CreateVvTextBox(3, 0, "tbx_dokDate", "Datum");
      tbx_dokDate.JAM_IsForDateTimePicker = true;
      dTP_dokDate      = zaglavljeHamper.CreateVvDateTimePicker(3, 0, "", tbx_dokDate);
      dTP_dokDate.Name = "dTP_datum";

      lbBanka = zaglavljeHamper.CreateVvLabel        (4, 0, "Banka:", ContentAlignment.MiddleRight);
      tbx_TT  = zaglavljeHamper.CreateVvTextBoxLookUp(5, 0, "tbx_TT", "Banka");
      tbx_TT.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_TT.JAM_DataRequired = true;
      tbx_TT.JAM_MustTabOutBeforeSubmit = true;
      tbx_TT.Text = "HNB";

      tbx_TtOpis = zaglavljeHamper.CreateVvTextBox(6, 0, "tbx_VKOpis_InVisible", "");
      tbx_TtOpis.JAM_ReadOnly = true;
      tbx_TtOpis.Text = "Hrvatska narodna banka";

      tbx_TtNum = zaglavljeHamper.CreateVvTextBox(7, 0, "tbx_TtNum", "Ovo bolje ostavi kako je...", 4);
      tbx_TtNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_TtNum.JAM_FillCharacter = '0';
     // tbx_TtNum.JAM_DataRequired = true;

      //tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaDevTecTT, (int)ZXC.Kolona.prva);
      //tbx_TT.JAM_lui_NameTaker_JAM_Name = tbx_TtOpis.JAM_Name;
      //tbx_TT.JAM_ttNumTaker_JAM_Name = tbx_TtNum.JAM_Name;

      lbBrojT       = zaglavljeHamper.CreateVvLabel  (0, 1, "Tečajna lista broj:", ContentAlignment.MiddleRight);
      tbx_extDokNum = zaglavljeHamper.CreateVvTextBox(1, 1, "tbx_extDokNum", "Tečajna lista utvrđena na dan");

      lbDatumF        = zaglavljeHamper.CreateVvLabel  (2, 1, "Utvrđena na dan:", ContentAlignment.MiddleRight);
      tbx_dateCreated = zaglavljeHamper.CreateVvTextBox(3, 1, "tbx_dateCreated", "Datum");
      tbx_dateCreated.JAM_IsForDateTimePicker = true;
      dTP_dateCreated = zaglavljeHamper.CreateVvDateTimePicker(3, 1, "", tbx_dateCreated);
      dTP_dateCreated.Name = "dTP_dateCreated";


      lbNapom      = zaglavljeHamper.CreateVvLabel  (4, 1, "Napomena:", ContentAlignment.MiddleRight);
      tbx_napomena = zaglavljeHamper.CreateVvTextBox(5, 1, "tbx_napomena", "Opis knjizenja / napomena.", 80/* GetDB_ColumnSize(DB_ci.napomena)*/, 2, 0);
   }

   #endregion ZaglavljeHamper

   #region TheGrid_Columns

   public void InitializeTheGrid_Columns()
   {
      T_valName_CreateColumn   (ZXC.Q3un);

      T_NazivSifra_CreateColumn(ZXC.Q10un + ZXC.Q2un);
      
      T_Jedinica_CreateColumn  (ZXC.Q2un, 0);

      T_kupovni_CreateColumn   (ZXC.Q5un, 6);
 
      T_srednji_CreateColumn   (ZXC.Q5un, 6);
      
      T_prodajni_CreateColumn  (ZXC.Q5un, 6);
   }

   private void T_valName_CreateColumn(int _width)
   {
      vvtbT_valName = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_valName", TheVvDaoTrans, DB_Tci.t_valName, "Valuta");
      vvtbT_valName.JAM_Set_LookUpTable(ZXC.luiListaDeviza, (int)ZXC.Kolona.prva);

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_valName, TheVvDaoTrans, DB_Tci.t_valName, "Valuta", _width);
   }

   private void T_NazivSifra_CreateColumn(int _width)
   {
      vvtbT_valNazivSifra = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_valNazivSifra", null, -12, "(Šifra) Naziv devize");
      vvtbT_valNazivSifra.JAM_ReadOnly = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_valNazivSifra, null, "T_nazivDev", "(Šifra) Naziv devize ", _width);

     // vvtbT_valName.JAM_lui_NameTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_opcName);
   }

   private void T_Jedinica_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_valJedinica = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_valJedinica", null, -12, "Jedinica valute");
      vvtbT_valJedinica.JAM_ReadOnly = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_valJedinica, null, "T_jedDev", "Jed", _width);
  //    vvtbT_opcCD.JAM_lui_NumberTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_stPrirez);

   }

   private void T_kupovni_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_kupovni = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_kupovni", TheVvDaoTrans, DB_Tci.t_kupovni, "Kupovni tečaj za devize");

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_kupovni, TheVvDaoTrans, DB_Tci.t_kupovni, "Kupovni T", _width);
   }

   private void T_srednji_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_srednji = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_srednji", TheVvDaoTrans, DB_Tci.t_srednji, "Srednji tečaj za devize");

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_srednji, TheVvDaoTrans, DB_Tci.t_srednji, "Srednji T", _width);
      colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;
   }
 
   private void T_prodajni_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_prodajni = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_prodajni", TheVvDaoTrans, DB_Tci.t_prodajni, "Prodajni tečaj za devize");

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_prodajni, TheVvDaoTrans, DB_Tci.t_prodajni, "Prodajni T", _width);
   }

   #endregion TheGrid_Columns

   #region SetNalogColumnIndexes()

   /// <summary>
   /// Column Index na DataGridView-u (NE U DataBase to ti je 'DB_ci')
   /// </summary>
   private DevTec_colIdx ci;
   public DevTec_colIdx DgvCI { get { return ci; } }

   public struct DevTec_colIdx
   {
      internal int iT_recID;
      internal int iT_serial;
      internal int iT_valName;
      internal int iT_nazivDev;
      internal int iT_jedDev;
      internal int iT_kupovni;
      internal int iT_srednji;
      internal int iT_prodajni;
   }

   private void SetNalogColumnIndexes()
   {
      ci = new DevTec_colIdx();

      ci.iT_recID     = TheG.IdxForColumn("T_recID");
      ci.iT_serial    = TheG.IdxForColumn("T_serial");
      ci.iT_valName   = TheG.IdxForColumn("T_valName");
      ci.iT_nazivDev  = TheG.IdxForColumn("T_nazivDev");
      ci.iT_jedDev    = TheG.IdxForColumn("T_jedDev");
      ci.iT_kupovni   = TheG.IdxForColumn("T_kupovni");
      ci.iT_srednji   = TheG.IdxForColumn("T_srednji");
      ci.iT_prodajni  = TheG.IdxForColumn("T_prodajni");
   }

   #endregion SetNalogColumnIndexes()

   #region Fld_

   public uint Fld_DokNum
   {
      get { return ZXC.ValOrZero_UInt(tbx_dokNum.Text); }
      set {                           tbx_dokNum.Text = value.ToString("000000"); }
   }

   public string Fld_TT
   {
      get { return tbx_TT.Text; }
      set {        tbx_TT.Text = value; }
   }

   public string Fld_TipTranOpis
   {
      set { tbx_TtOpis.Text = value; }
   }

   public DateTime Fld_DokDate
   {
      get { return dTP_dokDate.Value; }
      set
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dTP_dokDate.Value = value;
         }
      }
   }

   public DateTime Fld_DateCreated
   {
      get { return dTP_dateCreated.Value; }
      set
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dTP_dateCreated.Value = value;
         }
      }
   }

   public string Fld_Napomena
   {
      get { return tbx_napomena.Text; }
      set {        tbx_napomena.Text = value; }
   }

   public uint Fld_TTNum
   {
      get { return ZXC.ValOrZero_UInt(tbx_TtNum.Text); }
      set {                           tbx_TtNum.Text = value.ToString("0000"); }

   }

   public uint Fld_ExtDokNum
   {
      get { return ZXC.ValOrZero_UInt(tbx_extDokNum.Text); }
      set { tbx_extDokNum.Text = value.ToString(); }
   }

   #endregion Fld_

   #region PutFields(), GetFields()

   public override void PutFields(VvDataRecord devTec)
   {
      devTec_rec = (DevTec2)devTec;
      if(devTec_rec != null)
      {

         PutMetaFileds(devTec_rec.AddUID, devTec_rec.AddTS, devTec_rec.ModUID, devTec_rec.ModTS, devTec_rec.RecID, devTec_rec.LanSrvID, devTec_rec.LanRecID);

         PutIdentityFields(devTec_rec.DokNum.ToString("000000"), devTec_rec.DokDate.ToString(ZXC.VvDateFormat), devTec_rec.TT, "");

         Fld_DokNum      = devTec_rec.DokNum;
         Fld_DokDate     = devTec_rec.DokDate;
         Fld_Napomena    = devTec_rec.Napomena;
         Fld_TT          = devTec_rec.TT;
         Fld_TTNum       = devTec_rec.TtNum;
         Fld_DateCreated = devTec_rec.DateCreated;
         Fld_ExtDokNum   = devTec_rec.ExtDokNum;

 //        Fld_TipTranOpis = ZXC.luiListaDevTecTT.GetNameForThisCd(devTec_rec.TT);

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
      SetFilterRecordDependentDefaults(); // filter.DokNumOd = devTec_rec.DokNum (punimo bussiness od filtera, ne UC)
 //     TheDevTecDokumentFilterUC.PutFilterFields(TheDevTecDokumentFilter);
   }

   public override void GetFields(bool dirtyFlagging)
   {
      devTec_rec.DokNum      = Fld_DokNum;
      devTec_rec.DokDate     = Fld_DokDate;
      devTec_rec.Napomena    = Fld_Napomena;
      devTec_rec.TT          = Fld_TT;
      devTec_rec.TtNum       = Fld_TTNum;
      devTec_rec.DateCreated = Fld_DateCreated ;
      devTec_rec.ExtDokNum   = Fld_ExtDokNum   ;

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
      TheG.RowsAdded   -= new DataGridViewRowsAddedEventHandler(grid_RowsAdded);
      TheG.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(TheG);

      if(devTec_rec.Transes != null)
      {
       //  SetSifrarAndAutocomplete<Osred>(null, VvSQL.SorterType.None);

         foreach(Htrans2 htrans_rec in devTec_rec.Transes)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheG.Rows.Add();

            rowIdx = TheG.RowCount - idxCorrector;

            PutDgvLineFields(htrans_rec, rowIdx, false);
         }
      }

      TheG.RowsAdded   += new DataGridViewRowsAddedEventHandler(grid_RowsAdded);
      TheG.RowsRemoved += new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);

      RenumerateLineNumbers(TheG, 0);
      UpdateLineCount(TheG);

      PutDgvTransSumFields();
   }

   public override void PutDgvLineFields(VvTransRecord trans_rec, int rowIdx, bool skipRecID_andSerial_Columns)
   {
      Htrans2 htrans_rec = (Htrans2)trans_rec;

      VvLookUpItem lui =  ZXC.luiListaDeviza.GetLuiForThisCd(htrans_rec.T_ValName);

 
      if(skipRecID_andSerial_Columns == false)
      {
         TheG.PutCell(ci.iT_recID, rowIdx, htrans_rec.T_recID);
         TheG.PutCell(ci.iT_serial, rowIdx, htrans_rec.T_serial);
      }


      TheG.PutCell(ci.iT_valName, rowIdx, htrans_rec.T_ValName);

      if(lui != null) TheG.PutCell(ci.iT_nazivDev, rowIdx, lui.Name);
      if(lui != null) TheG.PutCell(ci.iT_jedDev  , rowIdx, lui.Number);

      TheG.PutCell(ci.iT_kupovni , rowIdx, htrans_rec.T_Kupovni);
      TheG.PutCell(ci.iT_srednji , rowIdx, htrans_rec.T_Srednji);
      TheG.PutCell(ci.iT_prodajni, rowIdx, htrans_rec.T_Prodajni);
      
      //PutDgvLineResultsFields(rowIdx, htrans_rec);
   }

   public override void PutDgvTransSumFields()
   {
      //TheSumGrid[ci.iT_kol, 0].Value = devTec_rec.Sum_Kol;
      //TheSumGrid[ci.iT_dug, 0].Value = devTec_rec.Sum_Dug;
      //TheSumGrid[ci.iT_pot, 0].Value = devTec_rec.Sum_Pot;
   }

   private void GetDgvFieldsOLD(bool dirtyFlagging)
   {
      int rIdx = 0;
      uint recID;
      bool DB_RWT;
      uint[] recIDtable;
      Htrans2 dgv_rec, db_rec;

      if(TheG.RowCount > 0) recIDtable = new uint[TheG.RowCount - 1];
      else recIDtable = null;

      devTec_rec.DiscardPreviouslyAddedTranses();

      //foreach(DataGridViewRow gridRow in TheG.Rows)
      for(rIdx = 0; rIdx < TheG.RowCount - 1; ++rIdx)
      {
         dgv_rec = new Htrans2();

         if(TheG.CI_OK(ci.iT_recID)) { recID = recIDtable[rIdx] = dgv_rec.T_recID = TheG.GetUint32Cell(ci.iT_recID, rIdx, dirtyFlagging); }
         else { recID = 0; ZXC.aim_emsg("!!! Column T_recID MISSING!!!"); }

         db_rec = null;
         if(recID > 0) db_rec = devTec_rec.Transes.SingleOrDefault(vvDR => vvDR.VirtualRecID == recID);
         else db_rec = null;

         DB_RWT = (db_rec != null);

         // dgvHtrans_rec: buffer za GetField iz DataGridWievRow-a                                        
         // db_rec : pointer na existing row u DB-u kojega ako je EditedHasChanges treba RWTREC-ati 

         dgv_rec.T_parentID = devTec_rec.RecID;

         dgv_rec.T_serial = (ushort)(rIdx + 1);
         if(DB_RWT) db_rec.T_serial = dgv_rec.T_serial;

         dgv_rec.T_dokNum = devTec_rec.DokNum;
         if(DB_RWT) db_rec.T_dokNum = dgv_rec.T_dokNum;

         dgv_rec.T_dokDate = devTec_rec.DokDate;
         if(DB_RWT) db_rec.T_dokDate = dgv_rec.T_dokDate;

         dgv_rec.T_TT = devTec_rec.TT;
         if(DB_RWT) db_rec.T_TT = dgv_rec.T_TT;

         //if(TheG.CI_OK(ci.iT_osredCD))
         //{
         //   dgv_rec.T_osredCD = TheG.GetStringCell(ci.iT_osredCD, rIdx, dirtyFlagging);
         //   if(DB_RWT) db_rec.T_osredCD = dgv_rec.T_osredCD;
         //}
         //if(TheG.CI_OK(ci.iT_opis))
         //{
         //   dgv_rec.T_opis = TheG.GetStringCell(ci.iT_opis, rIdx, dirtyFlagging);
         //   if(DB_RWT) db_rec.T_opis = dgv_rec.T_opis;
         //}
         //if(TheG.CI_OK(ci.iT_kol))
         //{
         //   dgv_rec.T_kol = TheG.GetDecimalCell(ci.iT_kol, rIdx, dirtyFlagging);
         //   if(DB_RWT) db_rec.T_kol = dgv_rec.T_kol;
         //}
         //if(TheG.CI_OK(ci.iT_koef_am))
         //{
         //   dgv_rec.T_koefAm = TheG.GetStringCell(ci.iT_koef_am, rIdx, dirtyFlagging);
         //   if(DB_RWT) db_rec.T_koefAm = dgv_rec.T_koefAm;
         //}
         //if(TheG.CI_OK(ci.iT_devTec_st))
         //{
         //   dgv_rec.T_devTecSt = TheG.GetDecimalCell(ci.iT_devTec_st, rIdx, dirtyFlagging);
         //   if(DB_RWT) db_rec.T_devTecSt = dgv_rec.T_devTecSt;
         //}
         //if(TheG.CI_OK(ci.iT_dug))
         //{
         //   dgv_rec.T_dug = TheG.GetDecimalCell(ci.iT_dug, rIdx, dirtyFlagging);
         //   if(DB_RWT) db_rec.T_dug = dgv_rec.T_dug;
         //}
         //if(TheG.CI_OK(ci.iT_pot))
         //{
         //   dgv_rec.T_pot = TheG.GetDecimalCell(ci.iT_pot, rIdx, dirtyFlagging);
         //   if(DB_RWT) db_rec.T_pot = dgv_rec.T_pot;
         //}


         if(dgv_rec.T_recID == 0) // ADDED NEW, FRESH Ftrans             
         {
            dgv_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;
            devTec_rec.Transes.Add(dgv_rec);
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
      int rIdx;

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
      else recIDtable = null;

      devTec_rec.DiscardPreviouslyAddedTranses();

      for(rIdx = 0; rIdx < TheG.RowCount - 1; ++rIdx)
      {
         GetDgvLineFields(rIdx, dirtyFlagging, recIDtable);
      }

      MarkTransesToDelete(recIDtable);

   }

   public override VvTransRecord GetDgvLineFields(int rIdx, bool dirtyFlagging, uint[] recIDtable)
   {
      uint recID;
      bool DB_RWT;
      Htrans2 db_rec;

      // dgvHtrans_rec: buffer za GetField iz DataGridWievRow-a                                        
      // db_rec : pointer na existing row u DB-u kojega ako je EditedHasChanges treba RWTREC-ati 

      recID = TheG.GetUint32Cell(ci.iT_recID, rIdx, dirtyFlagging);

      if(recIDtable != null) recIDtable[rIdx] = recID;

      dgvHtrans_rec = new Htrans2();

      if(recID > 0) // Postojeci redak 
      {
         db_rec = devTec_rec.Transes.SingleOrDefault(vvDR => vvDR.VirtualRecID == recID);
      }
      else // novododani redak 
      {
         db_rec = null;
      }

      DB_RWT = (db_rec != null);

      dgvHtrans_rec.T_recID = recID;

      dgvHtrans_rec.T_parentID = devTec_rec.RecID;

      #region GetColumns

          dgvHtrans_rec.T_serial = (ushort)(rIdx + 1);
      if(DB_RWT) db_rec.T_serial = dgvHtrans_rec.T_serial;

          dgvHtrans_rec.T_dokNum = devTec_rec.DokNum;
      if(DB_RWT) db_rec.T_dokNum = dgvHtrans_rec.T_dokNum;

          dgvHtrans_rec.T_dokDate = devTec_rec.DokDate;
      if(DB_RWT) db_rec.T_dokDate = dgvHtrans_rec.T_dokDate;

          dgvHtrans_rec.T_TT = devTec_rec.TT;
      if(DB_RWT) db_rec.T_TT = dgvHtrans_rec.T_TT;

      if(TheG.CI_OK(ci.iT_valName))
      {
             dgvHtrans_rec.T_ValName = TheG.GetStringCell(ci.iT_valName, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_ValName = dgvHtrans_rec.T_ValName;
      }
      if(TheG.CI_OK(ci.iT_kupovni))
      {
             dgvHtrans_rec.T_Kupovni = TheG.GetDecimalCell(ci.iT_kupovni, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_Kupovni = dgvHtrans_rec.T_Kupovni;
      }
      if(TheG.CI_OK(ci.iT_srednji))
      {
             dgvHtrans_rec.T_Srednji = TheG.GetDecimalCell(ci.iT_srednji, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_Srednji = dgvHtrans_rec.T_Srednji;
      }
      if(TheG.CI_OK(ci.iT_prodajni))
      {
             dgvHtrans_rec.T_Prodajni = TheG.GetDecimalCell(ci.iT_prodajni, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_Prodajni = dgvHtrans_rec.T_Prodajni;
      }
      #endregion GetColumns

      if(dgvHtrans_rec.T_recID == 0) // ADDED NEW, FRESH Ftrans             
      {
         devTec_rec.InvokeTransRemove(dgvHtrans_rec);

         dgvHtrans_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;

         devTec_rec.Transes.Add(dgvHtrans_rec);
      }
      else if(db_rec.EditedHasChanges())
      {
         db_rec.SaveTransesWriteMode = ZXC.WriteMode.Edit;
      }
      else
      {
         db_rec.SaveTransesWriteMode = ZXC.WriteMode.None;
      }

      return dgvHtrans_rec;
   }

   #endregion PutDgvFields(), GetDgvFields()

   #region Overriders and specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.devTec_rec; }
      set {        this.devTec_rec = (DevTec2)value; }
   }

   public override VvDocumentRecord VirtualDocumentRecord
   {
      get { return this.VirtualDataRecord as VvDocumentRecord; }
      set {        this.VirtualDataRecord = (DevTec2)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.DevTecDao; }
   }

   public override VvDaoBase TheVvDaoTrans
   {
      get { return ZXC.HtransDao; }
   }

   #region Document_Database_ColumnIndexes, Transes_Database_ColumnIndexes

   /// <summary>
   /// Column Index U DataBase (NE na DataGridView-u to ti je 'ci')
   /// </summary>
   private DevTecDao.DevTecCI DB_ci
   {
      get { return ZXC.DTecCI; }
   }

   /// <summary>
   /// Column Index u TRANS DataBase (NE na DataGridView-u to ti je 'ci')
   /// </summary>
   private HtransDao.HtransCI DB_Tci
   {
      get { return ZXC.HtrCI; }
   }

   #endregion Document_Database_ColumnIndexes, Transes_Database_ColumnIndexes

   #region Update_VvDataRecord (Legacy naming convention)

   public override VvFindDialog CreateVvFindDialog()
   {
      return CreateFind_DevTec_Dialog();
   }

   public static VvFindDialog CreateFind_DevTec_Dialog()
   {
      VvSubModul vvSubModul = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsDTEC);
      VvDataRecord    vvDataRecord = ZXC.TheVvForm.CreateTheVvDataRecord_SwitchSubModulEnum(vvSubModul);

      VvFindDialog vvFindDialog = new VvFindDialog();
      VvRecLstUC   vvRecListUC  = new DevTecListUC(vvFindDialog, (DevTec2)vvDataRecord, vvSubModul);

      vvFindDialog.TheRecListUC = vvRecListUC;

      return vvFindDialog;
   }

   #endregion Update_VvDataRecord (Legacy naming convention)

   #region PrintDocumentRecord

   //public DevTecDokumentFilterUC TheDevTecDokumentFilterUC { get; set; }
   //public DevTecDokumentFilter TheDevTecDokumentFilter { get; set; }

   //public RptO_DevTecDocument TheRptO_DevTecDocument
   //{
   //   get;
   //   set;
   //}

   //public override VvReport VirtualReport
   //{
   //   get
   //   {
   //      return this.TheRptO_DevTecDocument;
   //   }
   //}

   //public override string VirtualReportName
   //{
   //   get
   //   {
   //      return "TEMELJNICA DUGOTRAJNE IMOVINE";
   //   }
   //}

   //public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   //{
   //   return new RptO_DevTecDocument(reportName, (DevTecDokumentFilter)vvRptFilter);
   //}

   //public override VvRptFilter VirtualRptFilter
   //{
   //   get
   //   {
   //      return this.TheDevTecDokumentFilter;
   //   }
   //}

   //public override VvFilterUC VirtualFilterUC
   //{
   //   get
   //   {
   //      return this.TheDevTecDokumentFilterUC;
   //   }
   //}

   //public override void SetFilterRecordDependentDefaults()
   //{
   //   TheDevTecDokumentFilter.DokNumOd = devTec_rec.DokNum;
   //}

   #endregion PrintDocumentRecord


   public override VvLookUpLista TheTtLookUpList
   {
      get { return null; } // TODO: ! 
   }

   #endregion Overriders and specifics

   #region CreateDevTecDokumentPrintUC

   protected void CreateDevTecDokumentPrintUC(VvUserControl vvUC)
   {
      //this.TheDevTecDokumentFilter = new DevTecDokumentFilter();

      //TheDevTecDokumentFilterUC = new DevTecDokumentFilterUC(vvUC);
      //TheDevTecDokumentFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      //ThePanelForFilterUC_PrintTemplateUC.Width = TheDevTecDokumentFilterUC.Width;

   }

   #endregion CreateDevTecDokumentPrintUC

}

public class DownLoadOnDateDLG : VvDialog
{
   #region Fieldz

   private Button         okButton, cancelButton;
   private VvHamper       hamper;
   private int            dlgWidth, dlgHeight;
   private DateTimePicker dtp_datum;

   #endregion Fieldz

   #region Constructor

   public DownLoadOnDateDLG()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Download Tečajne Liste";

      CreateHamper();

      dlgWidth  = hamper.Right + ZXC.QunMrgn;
      dlgHeight = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

   }

   #endregion Constructor

   #region Hamper

   private void CreateHamper()
   {
      hamper          = new VvHamper(3, 1, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q5un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun2 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbDat;

      lbDat     = hamper.CreateVvLabel         (0, 0, "Datum primjene:", ContentAlignment.MiddleRight);
      dtp_datum = hamper.CreateVvDateTimePicker(1, 0, "", null);

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
      get { return dtp_datum.Value; }
      set {        dtp_datum.Value = value; }
   }


   #endregion Fld_
}
