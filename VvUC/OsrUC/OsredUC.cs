using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Linq;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using                  MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
using System.Collections.Generic;
#endif

public class OsredUC : VvSifrarRecordUC
{ 
   #region Fieldz

   private VvTextBox tbx_naziv   , tbx_OsredCd, tbx_serBroj   , tbx_konto, tbx_kontoIv, 
                     tbx_koefAm  , tbx_amortSt, tbx_vjekTraj  , tbx_inventBr,
                     tbx_mtrosCd , tbx_mtros  , tbx_mtrosTick ,
                     tbx_kupdobCd, tbx_kupdob, tbx_kupdobTick, tbx_brRacuna, tbx_grupa;

   private  VvHamper hampNaziv, hampGrupKto, hampMjTr_Dob;

   private  int    nextX = 0, nextY = 0, razmakHamp = ZXC.Qun10;
   private  Osred  osred_rec;

   private OsredDao.OsredCI DB_ci
   {
      get { return ZXC.OsrCI; }
   }

   #endregion Fieldz

   #region Constructor

   public OsredUC(Control parent, Osred _osred, VvSubModul vvSubModul)
   {
      osred_rec        = _osred;

      ThePrefferedRecordSorter = VirtualDataRecord.DefaultSorter;

      this.TheSubModul = vvSubModul;

      SuspendLayout();

      CreateTabPages(parent);
      CreateHampers();
      InitializeVvUserControl(parent);
      CreateDataGridView_InitializeTheGrid_ReadOnly_Columns();

      ResumeLayout();

      this.Validating += new System.ComponentModel.CancelEventHandler(OsredUC_Validating);
   }

   void OsredUC_Validating(object sender, System.ComponentModel.CancelEventArgs e)
   {
      #region Should validate enivej?

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None ||
         TheVvTabPage.WriteMode == ZXC.WriteMode.Delete ||
         this.Visible == false) return;

      #endregion Should validate enivej?


      //Dodajemo sifrar record u PG, pa treba provjeriti je li slobodna sifra i u NY 
      (bool thisSifraIs_Duplicated_InNY, VvSifrarRecord inNY_SifrarRecord) = IsThisSifra_Duplicated_InNY();

      if(thisSifraIs_Duplicated_InNY)
      {
         e.Cancel = true;

         ZXC.aim_emsg(MessageBoxIcon.Stop, "Dodajete šifru DI već zauzetu u novoj godini.\n\r\n\rIspravite ovu šifru na prvu sljedeću slobodnu,\n\r\n\rtj. da je 'slobodna' i u ovoj i u novoj godini.\n\r\n\ru novoj godini je:\n\r\n\r{0}", inNY_SifrarRecord);
      }

      // Umjetnine i sl.stvari jesu osred ali se neamortiziraju tj amStopa im je 0 22.10.2010.
    //if(Fld_AmortSt.IsZero() || Fld_Vijek.IsZero())
    //{
    //   ZXC.RaiseErrorProvider(tbx_amortSt, "Podaci 'zakonska stopa amortizacije' i 'vijek trajanja' nesmiju biti prazni.");
    //   e.Cancel = true;
    //}
   }

   #endregion Constructor

   #region TabPages

   private void CreateTabPages(Control _parent)
   {
      TheTabControl.TabPages.Add(CreateVvInnerTabPages("Matični", "", ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));

      VvTabPage vvTabPage = (VvTabPage)(_parent.Parent);

      if(vvTabPage.TabPageKind != ZXC.VvTabPageKindEnum.RECORD_TabPage_INTERACTIVE)
         TheTabControl.TabPages.Add(CreateVvInnerTabPages(atrans_TabPageName, "", ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage));
   }

   #endregion TabPages

   #region  HAMPER
  
   private void CreateHampers()
   {
      InitializeNazivHamper(out hampNaziv);
      nextY = hampNaziv.Bottom + ZXC.Qun8;
      InitializeGrupKtoHamper(out hampGrupKto);
      nextY = hampGrupKto.Bottom + ZXC.Qun8;
      InitializeDobavljac(out hampMjTr_Dob);

      nextX = hampNaziv.Right + ZXC.QUN;
      nextY = ZXC.Qun8;
   }

   private void InitializeNazivHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q4un + ZXC.Qun2, ZXC.Q10un + ZXC.Qun2, ZXC.Q4un, ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] {            ZXC.Qun4,             ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      
                  hamper.CreateVvLabel    (0, 0, "Naziv:"   , ContentAlignment.MiddleRight);
      tbx_naziv = hamper.CreateVvTextBox  (1, 0, "tbx_naziv",  "Naziv dugotrajne imovine", GetDB_ColumnSize(DB_ci.naziv), 2, 0);
                    hamper.CreateVvLabel  (0, 1, "Šifra:"   , ContentAlignment.MiddleRight);
      tbx_OsredCd = hamper.CreateVvTextBox(1, 1, "tbx_osredCD", "Sifra dugotrajne imovine", GetDB_ColumnSize(DB_ci.osredCD));
       
                  hamper.CreateVvLabel    (2, 1, "Grupa/AOP:"   , ContentAlignment.MiddleRight);
                  tbx_grupa = hamper.CreateVvTextBox(3, 1, "tbx_grupa", "001-GrađObj, 002-Brod, 003-OsnStado, 004-NematImov, 005-OsobAut, 006-Oprema, 007-Vozila(osim OA), 008-Mehaniz, 009-Računala, 010-RačMreže, 011-MobTel, 012-AlatiInv, 013-OstalaDI", GetDB_ColumnSize(DB_ci.grupa));

      tbx_naziv.JAM_DataRequired  = true;
      this.ControlForInitialFocus = tbx_naziv;
      tbx_naziv.JAM_StatusText    = "Naziv dugotrajne imovine";
   }

   private void InitializeGrupKtoHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 3, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q4un + ZXC.Qun2, ZXC.Q3un, ZXC.Q3un + ZXC.Qun4, ZXC.Q8un , ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4           , ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 ,            ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN,  ZXC.QUN,  ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbSerBr, lbkoefAm, lbzajkSt, lbvijek, lbKonto, lbKontoIv; 
       
      
      lbKonto     = hamper.CreateVvLabel  (0, 0, "Konto:"           , ContentAlignment.MiddleRight);
      tbx_konto   = hamper.CreateVvTextBox(1, 0, "tbx_konto"    , "Konto dugotrajne imovine iz kontnog plana", GetDB_ColumnSize(DB_ci.konto));
      lbKontoIv   = hamper.CreateVvLabel  (0, 1, "Konto IV:"        , ContentAlignment.MiddleRight);
      tbx_kontoIv = hamper.CreateVvTextBox(1, 1, "tbx_kontoIv"  , "Konto ispravka vrijednosti dugotrajne imovine iz kontnog plana", GetDB_ColumnSize(DB_ci.konto_iv));
      
      tbx_konto.JAM_CharEdits = tbx_kontoIv.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_konto.  JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_kontoIv.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);

      lbSerBr     = hamper.CreateVvLabel  (0, 2, "Serijski broj:"    , ContentAlignment.MiddleRight);
      tbx_serBroj = hamper.CreateVvTextBox(1, 2, "tbx_serBroj"  , "Serijski broj dugotrajne imovine", GetDB_ColumnSize(DB_ci.ser_br), 1,0);

      lbkoefAm     = hamper.CreateVvLabel  (3, 0, "Koeficijent amort. stope:", ContentAlignment.MiddleRight);
      tbx_koefAm   = hamper.CreateVvTextBox(4, 0, "tbx_koefAm"               , "'1' - Zakonska, '2' - Ubrzana (dvostruka), 'P' - potpuni otpis (100%).", GetDB_ColumnSize(DB_ci.koef_am));
      lbzajkSt     = hamper.CreateVvLabel  (3, 1, "Zakonska stopa amort.:"   , ContentAlignment.MiddleRight);
      tbx_amortSt  = hamper.CreateVvTextBox(4, 1, "tbx_zakStopa"              , "Trenutno vazeca zakonska (jednostruka) stopa otpisa", GetDB_ColumnSize(DB_ci.amort_st));
      lbvijek      = hamper.CreateVvLabel  (3, 2, "Vijek trajanja:"          , ContentAlignment.MiddleRight);
      tbx_vjekTraj = hamper.CreateVvTextBox(4, 2, "tbx_vijek"                , "Vijek trajanja", GetDB_ColumnSize(DB_ci.vijek));

      this.ControlForInitialFocus   = tbx_naziv;

      //tbx_amortSt. JAM_MarkAsNumericTextBox(2, true, 1.00M, 100.00M, true);
      //tbx_vjekTraj.JAM_MarkAsNumericTextBox(2, true, 1.00M, 100.00M, true);
      tbx_amortSt. JAM_MarkAsNumericTextBox(2, true, 0.00M, 100.00M, true);
      tbx_vjekTraj.JAM_MarkAsNumericTextBox(2, true, 0.00M, 1000.00M, true);
      tbx_amortSt. JAM_MustTabOutBeforeSubmit = true;
      tbx_vjekTraj.JAM_MustTabOutBeforeSubmit = true;

      tbx_amortSt.JAM_FieldExitMethod = new EventHandler(AmortStExitMethod);
      tbx_vjekTraj.JAM_FieldExitMethod = new EventHandler(VjekTrajExitMethod);
 
   }

   public void AmortStExitMethod(object sender, EventArgs e)
   {
      if(Fld_AmortSt.NotZero()) Fld_Vijek = 100 / Fld_AmortSt;
      else                     Fld_Vijek = 0.00M;  
   }

   public void VjekTrajExitMethod(object sender, EventArgs e)
   {
      if(Fld_Vijek.NotZero()) Fld_AmortSt = 100 / Fld_Vijek;
      else                    Fld_AmortSt = 0.00M;  
   }

   private void InitializeDobavljac(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 3, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q4un + ZXC.Qun2, ZXC.Q3un + ZXC.Qun8, ZXC.Q3un + ZXC.Qun8, ZXC.Q3un + ZXC.Qun2, ZXC.Q7un + ZXC.Qun2 + ZXC.Qun12};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4           , ZXC.Qun4           ,            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN,  ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbinvOD, lbkupdob, lbmjtrs, lbbrrc;
 
      lbinvOD  = hamper.CreateVvLabel(0, 0, "Inventarni broj:", ContentAlignment.MiddleRight);
      lbbrrc   = hamper.CreateVvLabel(3, 0, "Račun nabave:"   , ContentAlignment.MiddleRight);
      lbkupdob = hamper.CreateVvLabel(0, 1, "Dobavljač:"      , ContentAlignment.MiddleRight);
      lbmjtrs  = hamper.CreateVvLabel(0, 2, "Mjesto troška:"  , ContentAlignment.MiddleRight);

      tbx_inventBr   = hamper.CreateVvTextBox(1, 0, "tbx_inventBr", "Inventarni broj", GetDB_ColumnSize(DB_ci.invbr_od), 1, 0);
      tbx_brRacuna   = hamper.CreateVvTextBox(4, 0, "tbx_brRacuna", "Broj računa"    , GetDB_ColumnSize(DB_ci.dokum_cd), 0, 0);

      tbx_kupdobCd   = hamper.CreateVvTextBox(1, 1, "tbx_kupdobCd", "Šifra dobavljača  iz poslovnog adresara", GetDB_ColumnSize(DB_ci.kupdob_cd)     );
      tbx_kupdobTick = hamper.CreateVvTextBox(2, 1, "tbx_kupdob"  , "Ticker dobavljača iz poslovnog adresara", GetDB_ColumnSize(DB_ci.kupdob_tk)   );
      tbx_kupdob     = hamper.CreateVvTextBox(3, 1, "tbx_kupdob"  , "Naziv dobavljača  iz poslovnog adresara",                               32, 1, 0);
      tbx_mtrosCd    = hamper.CreateVvTextBox(1, 2, "tbx_mtrosCd" , "Šifra mjesta troška"                    , GetDB_ColumnSize(DB_ci.mtros_cd)      );
      tbx_mtrosTick  = hamper.CreateVvTextBox(2, 2, "tbx_mjTros"  , "Ticker mjesta troška"                   , GetDB_ColumnSize(DB_ci.mtros_tk)    );
      tbx_mtros      = hamper.CreateVvTextBox(3, 2, "tbx_mjTros"  , "Naziv mjesta troška"                    ,                               32, 1, 0);

      tbx_kupdobCd.JAM_CharEdits   = tbx_mtrosCd.JAM_CharEdits   = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_kupdobTick.JAM_CharacterCasing = tbx_mtrosTick.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_kupdobCd.  JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.    SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra)  , new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupdobTick.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker) , new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupdob.    JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv. SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)   , new EventHandler(AnyKupdobTextBoxLeave));

      tbx_mtrosCd.  JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.    SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra)  , new EventHandler(AnyMtrosTextBoxLeave));
      tbx_mtrosTick.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_mtros.    JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv. SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)   , new EventHandler(AnyMtrosTextBoxLeave));
   }

   void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob  kupdob_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;
         
         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_KupdobCd   = kupdob_rec.KupdobCD/*RecID*/;
            Fld_KupdobTick = kupdob_rec.Ticker;
            Fld_Kupdob     = kupdob_rec.Naziv;
         }
         else
         {
            Fld_KupdobCdAsTxt = Fld_KupdobTick = Fld_Kupdob = "";
         }
      }
   }

   void AnyMtrosTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_MtrosCd   = kupdob_rec.KupdobCD/*RecID*/;
            Fld_MtrosTick = kupdob_rec.Ticker;
            Fld_Mtros     = kupdob_rec.Naziv;
         }
         else
         {
            Fld_MtrosCdAsTxt = Fld_MtrosTick = Fld_Mtros = "";
         }
      }
   }

   #endregion  HAMPER

   #region Atrans DataGridView

   private void CreateDataGridView_InitializeTheGrid_ReadOnly_Columns()
   {
      aTransesGrid[0]      = CreateDataGridView_ReadOnly(TheTabControl.TabPages[atrans_TabPageName], "Atrans");
      aTransesGrid[0].Dock = DockStyle.Fill;
      int minGridWIdth     = InitializeTheGrid_ReadOnly_Columns(); // ovaj int bas i ne treba vise ali.....

      aTransesGrid[0].DoubleClick += new EventHandler        (theFIRST_TransGrid_DoubleClick);
      aTransesGrid[0].KeyPress    += new KeyPressEventHandler(theFIRST_TransGrid_KeyPress);
   }

   protected override void theFIRST_TransGrid_DoubleClick(object sender, EventArgs e)
   {
      base.OpenNew_Record_TabPage_OnDoubleClick(ZXC.VvSubModulEnum.AMO, SelectedRecIDIn_FIRST_TransGrid);
   }

   private int InitializeTheGrid_ReadOnly_Columns()
   {
      int sumOfColWidth = 0, colWidth;
      int colDateWidth  = ZXC.Q4un + ZXC.Qun4;
      int colSif6Width  = ZXC.Q3un + ZXC.Qun8;

      sumOfColWidth += aTransesGrid[0].RowHeadersWidth;
      colWidth = colSif6Width;                                   AddDGVColum_RecID_4GridReadOnly   (aTransesGrid[0], "RecID"    , colWidth, false, 0);
      colWidth = colDateWidth;        sumOfColWidth += colWidth; AddDGVColum_DateTime_4GridReadOnly(aTransesGrid[0], "Datum"    , colWidth);
      colWidth = colSif6Width;        sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (aTransesGrid[0], "BrDok"    , colWidth, true, 6);
      colWidth = ZXC.Q2un + ZXC.Qun4; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (aTransesGrid[0], "Red"      , colWidth, false, 0);
      colWidth = ZXC.Q2un;            sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (aTransesGrid[0], "TT"       , colWidth, false);
      colWidth = ZXC.Q6un;            sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (aTransesGrid[0], "Opis"     , colWidth, true);
      colWidth = ZXC.Q2un;            sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[0], "Kol"      , colWidth, 0);
      colWidth = ZXC.Q2un;            sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (aTransesGrid[0], "KoAm"     , colWidth, false);
      colWidth = ZXC.Q3un;            sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[0], "StAm"     , colWidth, 2);
      colWidth = ZXC.Q6un;            sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[0], "Duguje"   , colWidth, 2);
      colWidth = ZXC.Q6un;            sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[0], "Potražuje", colWidth, 2);

      return sumOfColWidth;
   }

   #endregion DataGridView

   #region Filter

   public override void CreateRptFilterAndRptFilterUC()
   {
      TheAtransFilter = new VvRpt_Osred_Filter();
      TheOsrFilterUC  = new AmoFilterUC(this);
      
      TheOsrFilterUC.Parent                     = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheOsrFilterUC.Width;

   }

   #endregion Filter

   #region Fld_

   public string Fld_OsredCd
   {
      get { return tbx_OsredCd.Text; }
      set { tbx_OsredCd.Text = value; }
   }

   public string Fld_Naziv
   {
      get { return tbx_naziv.Text;         }
      set {        tbx_naziv.Text = value; }
   }

   public string Fld_Konto
   {
      get { return tbx_konto.Text;         }
      set {        tbx_konto.Text = value; }
   }

   public string Fld_KontoIv
   {
      get { return tbx_kontoIv.Text;         }
      set {        tbx_kontoIv.Text = value; }
   }

   public string Fld_SerBr
   {
      get { return tbx_serBroj.Text;         }
      set {        tbx_serBroj.Text = value; }
   }

   public uint Fld_MtrosCd
   {
      get { return tbx_mtrosCd.GetSomeRecIDField();      }
      set {        tbx_mtrosCd.PutSomeRecIDField(value); }
   }

   public string Fld_MtrosCdAsTxt
   {
      get { return tbx_mtrosCd.Text;         }
      set {        tbx_mtrosCd.Text = value; }
   }

   public string Fld_Mtros
   {
      get { return tbx_mtros.Text;         }
      set {        tbx_mtros.Text = value; }
   }
   
   public string Fld_MtrosTick
   {
      get { return tbx_mtrosTick.Text;         }
      set {        tbx_mtrosTick.Text = value; }
   }

   public uint Fld_KupdobCd
   {
      get { return tbx_kupdobCd.GetSomeRecIDField();      }
      set {        tbx_kupdobCd.PutSomeRecIDField(value); }
   }

   public string Fld_KupdobCdAsTxt
   {
      get { return tbx_kupdobCd.Text;         }
      set {        tbx_kupdobCd.Text = value; }
   }

   public string Fld_Kupdob
   {
      get { return tbx_kupdob.Text;         }
      set {        tbx_kupdob.Text = value; }
   }

   public string Fld_KupdobTick
   {
      get { return tbx_kupdobTick.Text;         }
      set {        tbx_kupdobTick.Text = value; }
   }

   public string Fld_DokumCd
   {
      get { return tbx_brRacuna.Text;         }
      set {        tbx_brRacuna.Text = value; }
   }

   public string Fld_InvbrOd
   {
      get { return tbx_inventBr.Text;         }
      set {        tbx_inventBr.Text = value; }
   }

   public string Fld_KoefAm
   {
      get { return tbx_koefAm.Text;         }
      set {        tbx_koefAm.Text = value; }
   }
   
   public decimal Fld_AmortSt
   {
      get { return tbx_amortSt.GetDecimalField();      }
      set {        tbx_amortSt.PutDecimalField(value); }
   }

   public decimal Fld_Vijek
   {
      get { return tbx_vjekTraj.GetDecimalField();      }
      set {        tbx_vjekTraj.PutDecimalField(value); }
   }

   public string Fld_Grupa
   {
      get { return tbx_grupa.Text; }
      set {        tbx_grupa.Text = value; }
   }

   #endregion Fld_
   
   #region PutFields(), GetFields()

   public override void PutFields(VvDataRecord osred)
   {
      osred_rec = (Osred)osred;
      Kupdob kupdobSifrar_rec;

      if(osred_rec != null)
      {
         PutMetaFileds(osred_rec.AddUID, osred_rec.AddTS, osred_rec.ModUID, osred_rec.ModTS, osred_rec.RecID, osred_rec.LanSrvID, osred_rec.LanRecID);
       
         Fld_OsredCd    = osred_rec.OsredCD;
         Fld_Naziv      = osred_rec.Naziv;
         Fld_Konto      = osred_rec.Konto;
         Fld_KontoIv    = osred_rec.KontoIv;
         Fld_SerBr      = osred_rec.SerBr;
         Fld_KupdobCd   = osred_rec.KupdobCd;
         Fld_KupdobTick = osred_rec.KupdobTk;
         Fld_MtrosTick  = osred_rec.MtrosTk;
         Fld_MtrosCd    = osred_rec.MtrosCd;
         Fld_DokumCd    = osred_rec.DokumCd;
         Fld_InvbrOd    = osred_rec.InvbrOd;
         Fld_KoefAm     = osred_rec.KoefAm;
         Fld_AmortSt    = osred_rec.AmortSt;
         Fld_Vijek      = osred_rec.Vijek;
         Fld_Grupa      = osred_rec.Grupa;

         //===================== 

         SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.None);

         //kupdobSifrar_rec = KupdobSifrar.FindByRecID(osredRich_rec.KupdobCd);
         kupdobSifrar_rec = KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD/*VirtualRecID*/ == osred_rec.KupdobCd);
         if(kupdobSifrar_rec != null) Fld_Kupdob = kupdobSifrar_rec.Naziv;
         else                         Fld_Kupdob = "";

         //kupdobSifrar_rec = KupdobSifrar.FindByRecID(osredRich_rec.MtrosCd);
         kupdobSifrar_rec = KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD/*VirtualRecID*/ == osred_rec.MtrosCd);
         if(kupdobSifrar_rec != null) Fld_Mtros = kupdobSifrar_rec.Naziv;
         else                         Fld_Mtros = "";

         //===================== 


         PutIdentityFields(osred_rec.RecID.ToString("000000"), osred_rec.Naziv, "", "");

         InitializeFilterUCFields();

         recordReportLoaded = false;
         DecideIfShouldLoad_VvReport(null, null, null);

         aTransesLoaded[0] = false; // ovdje treba nulirati sve postojece 'xyLoaded' varijable
         DecideIfShouldLoad_TransDGV(null, null, null);
      }
   }

   public override void GetFields(bool fuse)
   {
      if(osred_rec == null) osred_rec = new Osred();

      osred_rec.OsredCD  = Fld_OsredCd;
      osred_rec.Naziv    = Fld_Naziv;
      osred_rec.Konto    = Fld_Konto;
      osred_rec.KontoIv  = Fld_KontoIv;
      osred_rec.SerBr    = Fld_SerBr;
      osred_rec.KupdobCd = Fld_KupdobCd;
      osred_rec.KupdobTk = Fld_KupdobTick;
      osred_rec.MtrosCd  = Fld_MtrosCd;
      osred_rec.MtrosTk  = Fld_MtrosTick;
      osred_rec.DokumCd  = Fld_DokumCd;
      osred_rec.InvbrOd  = Fld_InvbrOd;
      osred_rec.KoefAm   = Fld_KoefAm;
      osred_rec.AmortSt  = Fld_AmortSt;
      osred_rec.Vijek    = Fld_Vijek;
      osred_rec.Grupa    = Fld_Grupa;

   }

   #endregion PutFields(), GetFields()

   #region Put Trans DGV Fileds

   private const string atrans_TabPageName = "AM Trans";

   // Tu dolazimo na 1 nacin: 1. Classic PutFields 
   private void InitializeFilterUCFields()
   {
      SetFilterRecordDependentDefaults(); // filter.KontoOd i Do = kplan_rec.Konto (punimo bussiness od filtera, ne UC)
      TheOsrFilterUC.PutFilterFields(TheAtransFilter);
   }

   // Tu dolazimo na 2 nacina:          
   // 1. Classic PutFields              
   // 2. TheTabControl.SelectionChanged 
   public override void DecideIfShouldLoad_TransDGV(VvInnerTabControl sender, VvInnerTabPage oldPage, VvInnerTabPage newPage)
   {
      ZXC.VvInnerTabPageKindEnum innerTabPageKind = ((VvInnerTabPage)TheTabControl.SelectedTab).TheInnerTabPageKindEnum;

      if(aTransesLoaded[0] == false &&
         innerTabPageKind == ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage)
      {
         LoadRecordList_AND_PutTransDgvFields();
      }

   }

   // Tu dolazimo na 2 nacina:          
   // 1. ButtonIzlistaj_Click (sa FilterUC-a) 
   // 2. DecideIfShouldLoad_TransDGV    
   public override void LoadRecordList_AND_PutTransDgvFields()
   {
      int rowIdx, idxCorrector;

      TheOsrFilterUC.GetFilterFields();
      TheOsrFilterUC.AddFilterMemberz(TheAtransFilter, null);

      if(osred_rec.Atranses == null) osred_rec.Atranses = new  List <Atrans>();
      else                           osred_rec.Atranses.Clear();

      string orderByColumns;
      if(TheOsrFilterUC.Fld_DokumentSort == VvSQL.SorterType.DokNum)
         orderByColumns = "                t_dokNum DESC, t_serial DESC";
      else
         orderByColumns = "t_dokDate DESC, t_dokNum DESC, t_serial DESC";

      VvDaoBase.LoadGenericVvDataRecordList<Atrans>(TheDbConnection, osred_rec.Atranses, TheAtransFilter.FilterMembers, orderByColumns);

      aTransesLoaded[0] = true;

      aTransesGrid[0].Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(aTransesGrid[0]);

      foreach(Atrans atrans_rec in osred_rec.Atranses)
      {
         aTransesGrid[0].Rows.Add();

         rowIdx = aTransesGrid[0].RowCount - idxCorrector;

         aTransesGrid[0][0,  rowIdx].Value = atrans_rec.T_parentID;
         aTransesGrid[0][1,  rowIdx].Value = atrans_rec.T_dokDate;
         aTransesGrid[0][2,  rowIdx].Value = atrans_rec.T_dokNum;
         aTransesGrid[0][3,  rowIdx].Value = atrans_rec.T_serial;
         aTransesGrid[0][4,  rowIdx].Value = atrans_rec.T_TT;
         aTransesGrid[0][5,  rowIdx].Value = atrans_rec.T_opis;
         aTransesGrid[0][6,  rowIdx].Value = atrans_rec.T_kol;
         aTransesGrid[0][7,  rowIdx].Value = atrans_rec.T_koefAm;
         aTransesGrid[0][8,  rowIdx].Value = atrans_rec.T_amortSt;
         aTransesGrid[0][9,  rowIdx].Value = atrans_rec.T_dug;
         aTransesGrid[0][10, rowIdx].Value = atrans_rec.T_pot;

         aTransesGrid[0].Rows[rowIdx].HeaderCell.Value = (osred_rec.Atranses.Count - rowIdx).ToString();
      }

      //VvDocumentRecordUC.RenumerateLineNumbers(gridFtrans, 0);

   }

   #endregion Put Trans DGV Fileds

   #region Overriders And Specifics

   #region VvDataRecord/VvDaoBase

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.osred_rec; }
      set { this.osred_rec = (Osred)value; }
   }

   public override VvSifrarRecord VirtualSifrarRecord
   {
      get { return this.VirtualDataRecord as VvSifrarRecord; }
      set {        this.VirtualDataRecord = (Osred)value;    }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.OsredDao; }
   }
   
   #endregion VvDataRecord/VvDaoBase

   #region VvFindDialog

   public override VvFindDialog CreateVvFindDialog()
   {
      return CreateFind_Osred_Dialog();
   }

   public static VvFindDialog CreateFind_Osred_Dialog()
   {
      VvSubModul vvSubModul   = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsOSR);
      VvDataRecord      vvDataRecord = ZXC.TheVvForm.CreateTheVvDataRecord_SwitchSubModulEnum(vvSubModul);

      VvFindDialog vvFindDialog = new VvFindDialog();

      VvRecLstUC vvRecListUC = new OsredListUC(vvFindDialog, (Osred)vvDataRecord, vvSubModul);

      vvFindDialog.TheRecListUC = vvRecListUC;

      return vvFindDialog;
   }

   #endregion VvFindDialog

   #region PrintSifrarRecord

   public VvRpt_Osred_Filter TheAtransFilter { get; set; }

   public AmoFilterUC    TheOsrFilterUC { get; set; }

   //public RptO_KartOsred      TheRptO_KOsred { get; set; }

   //public override VvReport VirtualReport
   //{
   //   get
   //   {
   //      return this.TheRptO_KOsred;
   //   }
   //}

   public override string VirtualReportName
   {
      get
      {
         return "KARTICA DUGOTRAJNE IMOVINE";
      }
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      return new RptO_KartOsred(reportName, (VvRpt_Osred_Filter)vvRptFilter);
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheAtransFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheOsrFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
      this.TheAtransFilter.OsredCDod = osred_rec.OsredCD;
      this.TheAtransFilter.OsredCDdo = osred_rec.OsredCD;
      this.TheAtransFilter.NazivOd   = osred_rec.Naziv;
      this.TheAtransFilter.NazivDo   = osred_rec.Naziv;
      this.TheAtransFilter.KontoOd   = osred_rec.Konto;
      this.TheAtransFilter.KontoDo   = osred_rec.Konto;
   }

   #endregion PrintSifrarRecord

   public override Size ThisUcSize
   {
      get
      {
         return new Size(hampGrupKto.Right + ZXC.QunMrgn, hampMjTr_Dob.Bottom + ZXC.QunMrgn);
      }
   }

   #region PutNew_Sifra_Field

   public override void PutNew_Sifra_Field(uint newSifra)
   {
      Fld_OsredCd = newSifra.ToString("000000");
   }

   #endregion PutNew_Sifra_Field

   #endregion Overriders And Specifics

   #region Update_VvDataRecord (Legacy naming convention)

   /// <summary>
   /// 'FindVvDataRecord' procedura. Inicirana:
   /// 1. Context menu (Mouse right click)
   /// 2. Mouse click (Ctrl ili Alt click)
   /// 3. Keyboard initiated (Ctrl/Alt + F/Space)
   /// </summary>
   /// <param name="startValue"></param>
   /// <returns></returns>
   public static string Update_Osred(VvSQL.SorterType whichInformation, object startValue)
   {
      Osred           osred_rec    = new Osred();
      OsredListUC     osredListUC   ;
      XSqlConnection  dbConnection = ZXC.TheVvForm.TheDbConnection;

      VvFindDialog dlg = CreateFind_Osred_Dialog();

      osredListUC = (OsredListUC)(dlg.TheRecListUC);
      
      switch(whichInformation)
      {
         case VvSQL.SorterType.Name: osredListUC.Fld_FromNaziv   = startValue.ToString(); break;
         case VvSQL.SorterType.Code: osredListUC.Fld_FromOsredCD = startValue.ToString(); break;

         default: ZXC.aim_emsg(" 111: For Osred, trazi po [{1}] still nedovrseno!", whichInformation); break;
      }

      if (dlg.ShowDialog() == DialogResult.OK)
      {
         if(!ZXC.OsredDao.SetMe_Record_byRecID(dbConnection, osred_rec, (uint)dlg.SelectedRecID, false)) return null;
      }
      else
      {
         osred_rec = null;
      }

      if(dlg.SelectionIsNewlyAddedRecord == true) ZXC.ShouldForceSifrarRefreshing = true;

      dlg.Dispose();

      if(osred_rec != null)
      {
         switch(whichInformation)
         {
            case VvSQL.SorterType.Name: return osred_rec.Naziv;
            case VvSQL.SorterType.Code: return osred_rec.OsredCD;

            default: ZXC.aim_emsg(" 222: For Osred, trazi po [{1}] still nedovrseno!", whichInformation); return null;
         }
      }
      else return null;
   }

   #endregion Update_VvDataRecord (Legacy naming convention)

}

public class VvRenameOsredDlg : VvDialog
{
   #region Fieldz
 
   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_newOsredCD;

   #endregion Fieldz

   #region Constructor

   public VvRenameOsredDlg()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Preimenuj šifru DI";

      CreateHamper();

      dlgWidth        = hamper.Right  + ZXC.QunMrgn;
      dlgHeight       = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_newOsredCD, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
   }

   #endregion Constructor

   #region hamper

   private void CreateHamper()
   {
      hamper          = new VvHamper(2, 1, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      int columnSize = ZXC.OsredDao.GetSchemaColumnSize(ZXC.OsredDao.CI.osredCD);

      Label lbl      = hamper.CreateVvLabel  (0, 0, "Nova šifra:"   ,  ContentAlignment.MiddleRight);
      tbx_newOsredCD = hamper.CreateVvTextBox(1, 0, "tbx_newOsredCD", "", columnSize);
   }

   #endregion hamper

   #region Event cancelButton

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Event cancelButton

   #region Fld_

   public string Fld_NewOsredCD
   {
      get { return tbx_newOsredCD.Text; }
      set {        tbx_newOsredCD.Text = value; }
   }

   #endregion Fld_

}
