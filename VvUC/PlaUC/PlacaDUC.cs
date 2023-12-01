using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

public partial class PlacaBaseDUC : VvPolyDocumRecordUC
{
   #region Fieldz

   public Placa placa_rec;

   public Ptrans  dgvPtrans_rec;
   private Ptrane dgvPtrane_rec;
   private Ptrano dgvPtrano_rec;

   private VvHamper hamp_zaglavlje;
   public  VvHamper hamp_rules, hamp_Open;

   public VvTextBox tbx_tt;
   private VvTextBox tbx_dokNum, tbx_dokDate, tbx_lookUpMMYYYY, tbx_fondSati, tbx_ttOpis, tbx_ttNum, tbx_lookUpVrstaObr,
                     tbx_mtros_cd, tbx_mtros_tk, tbx_mtros_Naziv,
                     tbx_napomena, tbx_RSmID,
                     tbx_stPor1, tbx_stPor2, tbx_stPor3, tbx_stPor4,
                     tbx_osnOdb, tbx_maxPorOsn1, tbx_maxPorOsn2, tbx_maxPorOsn3,
                     tbx_minMioOsn, tbx_maxMioOsn, tbx_stDodStaz, tbx_granBrRad,
                     tbx_stMioIz, tbx_stMioIz_II, tbx_stZdrNa, tbx_stZorNa,
                     tbx_stZapNa, tbx_stZapII, tbx_stZpi, tbx_stOthOlak,
                     tbx_stMioNaB1, tbx_stMioNa2B1, tbx_stMioNaB2, tbx_stMioNa2B2,
                     tbx_stMioNaB3, tbx_stMioNa2B3, tbx_stMioNaB4, tbx_stMioNa2B4, tbx_prosPlaca, tbx_stMioNa2B5,
                     tbx_stKrizPor1, tbx_stKrizPor2, tbx_isTrgFondSati, tbx_lookUpVrstaJoppd, tbx_VrKoefBr1, tbx_isLocked, tbx_stZdrDD,
                     tbx_mio1Granica1, tbx_mio1Granica2, tbx_mio1FiksOlk, tbx_mio1KoefOlk; // novo 2024

   private VvDateTimePicker dTP_datum;
   private Button btn_PlusMinus;
   private Label lblCrta;
   private int nextX = 0, nextY = 0, razmakHamp = ZXC.Qun10;

   VvCheckBox vvcbx_isMioII, vvcbx_isDirNeto, vvcbx_IsPoluSat, vvcbx_isZbirOb, vvcbx_isZastRn /* ptranoKind*/;

   private CheckBox cbx_isTrgFondSati, cbx_isLocked;
   
   private VvTextBox vvtbT_personCD, vvtbT_ime, vvtbT_prezime,
                     vvtbT_brutoOsn, vvtbT_topObrok, vvtbT_godStaza, vvtbT_dodBruto,
                     vvtbT_spc, vvtbT_koef, vvtbT_koefBruto1, vvtbT_dnFondSati, 
                     vvtbT_zivotno, vvtbT_dopZdr, vvtbT_dopZdr2020, vvtbT_dobMIO, vvtbT_koefHRVI,
                     vvtbT_invalidTip, vvtbT_opcCD, vvtbT_opcName, vvtbT_opcRadCD,
                     vvtbT_opcRadName, vvtbT_stPrirez, vvtbT_netoAdd,
                     vvtbT_prijevoz,
                     vvtbT_prezimeIme, vvtbT_thisStazSt,
                     vvtbT_brutoDodSt2, vvtbT_brutoDodSt3, vvtbT_pr3mjBruto, vvtbT_brutoKorekc,

                     vvtbT_personCD2, vvtbT_ime2, vvtbT_prezime2,
                     vvtbT_vrstaR_cd, vvtbT_vrstaR_name, vvtbT_cijPerc, vvtbT_sati,
                     vvtbT_rsOO, vvtbT_rsB, vvtbT_rsOD, vvtbT_rsDO, vvtbT_ip1gr, vvtbT_rbrIsprJop,

                     vvtbT_personCD3, vvtbT_ime3, vvtbT_prezime3,
                     vvtbT_ukBrRata, vvtbT_opisOb, vvtbT_kupdob_cd,
                     vvtbT_kupdob_tk, vvtbT_iznosOb, vvtbT_partija,
                     vvtbT_brutoDodSt, vvtbT_izNetoaSt, vvtbT_brDodpoloz,
                     vvtbT_rbrRate,    vvtbT_ptranoKind,
                     vvtbT_stPorez1, vvtbT_stPorez2, vvtbT_fixMio1Olak,//novo 2024

                     //R   
                     vvtbT_bruto100, vvtbT_theBruto, vvtbT_mioOsn, vvtbT_mio1stup,
                     vvtbT_mio2stup, vvtbT_mioAll, vvtbT_doprIz, vvtbT_odbitak,
                     vvtbT_premije, vvtbT_dohodak, vvtbT_porOsnAll, vvtbT_porOsn1,
                     vvtbT_porOsn2, vvtbT_porOsn3, vvtbT_porOsn4, vvtbT_por1uk,
                     vvtbT_por2uk, vvtbT_por3uk, vvtbT_por4uk, vvtbT_porezAll,
                     vvtbT_prirez, vvtbT_porPrirez, vvtbT_netto, vvtbT_obustave,
                     vvtbT_2Pay, vvtbT_naRuke, vvtbT_zdrNa, vvtbT_zorNa,
                     vvtbT_zapNa, vvtbT_zapII, vvtbT_zapAll, vvtbT_doprNa, vvtbT_doprAll,
                     vvtbT_satiUk, vvtbT_satiR, vvtbT_satiB, vvtbT_satiNeR,
                     vvtbT_numOfLinesPtrane, vvtbT_numOfLinesPtrano, vvtbT_fondSatiDiff,
                     vvtbT_mio1stupNa, vvtbT_mio2stupNa, vvtbT_mioAllNa,
                     vvtbT_krizPorOsn, vvtbT_krizPorUk,
                     vvtbT_zpiUk, vvtbT_daniZpi, vvtbT_nettoBlue,
                     
                     vvtbT_nacIsplCD, vvtbT_neoPrimCD, vvtbT_dokumCD,
                     vvtbT_stjecatCD, vvtbT_primDohCD, vvtbT_pocKrajCD,
                     //R
                     vvtbT_stjecatName, vvtbT_primDohName, vvtbT_pocKrajName,
                     vvtbT_neoPrimName, vvtbT_nacIsplName,
                     vvtbT_mio1Olk, vvtbT_mio1Osn;//novo 2024

   private VvTextBoxColumn           colVvText;
   private VvDateTimePickerColumn    colDate;
   private VvCheckBoxColumn          colCbox;
   private DataGridViewTextBoxColumn colScrol;
   private Panel panelRules;

   protected ZXC.DbNavigationRestrictor dbNavigationRestrictor;

   protected VvReport specificPlacaReport;
   protected string   specificPlacaReportName;

   #endregion Fieldz

   #region Virtual Metodz

   protected virtual void InitializeDUC_Specific_Columns_TheG() { }
   protected virtual void InitializeDUC_Specific_Columns_TheG2() { }
   protected virtual void InitializeDUC_Specific_Columns_TheG3() { }

   #endregion Virtual Metodz

   #region Constructor

   public PlacaBaseDUC(Control parent, Placa _placa, VvForm.VvSubModul vvSubModul)
   {
      placa_rec = _placa;

      ThePrefferedRecordSorter = VirtualDataRecord.DefaultSorter;

      this.TheSubModul = vvSubModul;

      SuspendLayout();

      TheTabControl.TabPages.Add(CreateVvInnerTabPages("Plaća", "", ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
      ThePolyGridTabControl.Parent = TheTabControl.TabPages[0];
      ThePolyGridTabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(ThePolyGridTabControl_SelectionChanged);

      CreateHampers();

      PlacaColChDefaultsList = new List<VvPref.VVColChooserStates>();

      #region TheG

      TheG              = CreateVvDataGridView(ThePolyGridTabControl.TabPages[0]      , "Ptrans Grid");
      TheColChooserGrid = CreateColChooserGrid(TheG, ThePolyGridTabControl.TabPages[0], "ColChooser Grid");
      TheSumGrid        = CreateSumGrid(TheG, ThePolyGridTabControl.TabPages[0]       , "SUM Ptrans Grid");

      //3.12.2013.InitializeTheGrid_Columns();
      InitializeDUC_Specific_Columns_TheG();

      InitializeTheChBoxGrid_Columns(TheG);
      InitializeTheSUMGrid_Columns(TheG);

      TheG.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged_SumGrid);
      TheG.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged_ChBoxGrid);

      #endregion TheG

      #region TheG2

      TheG2       = CreateVvDataGridView(ThePolyGridTabControl.TabPages[1], "Ptrane Grid");
      TheSumGrid2 = CreateSumGrid(TheG2, ThePolyGridTabControl.TabPages[1], "SUM Ptrane Grid");

      //3.12.2013. InitializeTheGrid2_Columns();
      InitializeDUC_Specific_Columns_TheG2();
      InitializeTheSUMGrid_Columns(TheG2);

      TheG2.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged_SumGrid);

      #endregion TheG2

      #region TheG3

      TheG3       = CreateVvDataGridView(ThePolyGridTabControl.TabPages[2], "Ptrano Grid");
      TheSumGrid3 = CreateSumGrid(TheG3, ThePolyGridTabControl.TabPages[2], "SUM Ptrano Grid");

      //3.12.2013.  InitializeTheGrid3_Columns();
      InitializeDUC_Specific_Columns_TheG3();
      InitializeTheSUMGrid_Columns(TheG3);

      TheG3.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged_SumGrid);

      #endregion TheG3

      InitializeVvUserControl(parent);

      #region CalcLocationSizeAnchor

      CalcLocationSizeAnchor_ThePolyGridTabControl(TheTabControl.TabPages[0], nextX, nextY);

      CalcLocationSizeAnchor_TheDGVAndTheSumGrid_WithChBoxGrid_NEW(TheG, ZXC.QunMrgn, ZXC.QunMrgn);
      CalcLocationSizeAnchor_TheDGVAndTheSumGrid_NEW(TheG2, ZXC.QunMrgn, ZXC.QunMrgn);
      CalcLocationSizeAnchor_TheDGVAndTheSumGrid_NEW(TheG3, ZXC.QunMrgn, ZXC.QunMrgn);

      #endregion CalcLocationSizeAnchor

      // 14.02.2014: 
      this.Validating += new System.ComponentModel.CancelEventHandler(PlacaDUC_Validating);

      ResumeLayout();

      SetPtransColumnIndexes();
      SetPtraneColumnIndexes();
      SetPtranoColumnIndexes();

      CreatePlacaDokumentPrintUC(this);

      PutFields_TheColChooserGrid();

      TheColChooserGrid.CurrentCellDirtyStateChanged += new EventHandler(TheColChooserGrid_CurrentCellDirtyStateChanged);
      TheColChooserGrid.CellValueChanged += new DataGridViewCellEventHandler(TheColChooserGrid_CellValueChanged);

      SelectVisibleColumns(ZXC.VvColSetVisible.BlueVisible);

      SetSifrarAndAutocomplete<Person>(null, VvSQL.SorterType.None);

   }

   // This event handler manually raises the CellValueChanged event
   // by calling the CommitEdit method.
   void TheColChooserGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
   {
      if(TheColChooserGrid.IsCurrentCellDirty)
      {
         TheColChooserGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
      }
   }

   #endregion Constructor

   #region ThePolyGridTabControl_SelectionChanged

   private void ThePolyGridTabControl_SelectionChanged(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   {
      Point xy = ZXC.TheVvForm.TheVvTabPage.SubModul_xy;
      ToolStrip ts = ZXC.TheVvForm.ats_SubModulSet[xy.X][xy.Y];
      bool viewChGird = false;

      if(TheG.ColumnHeadersDefaultCellStyle.BackColor == ZXC.vvColors.dataGridColumnHeaders_BackColor) viewChGird = true;
      else viewChGird = false;

      if(newPage == ThePolyGridTabControl.TabPages[0])
      {
         SetEnableDisableNewOldTabPage(ts, true, viewChGird);
      }
      else
      {
         SetEnableDisableNewOldTabPage(ts, false, viewChGird);
      }

   }

   private void SetEnableDisableNewOldTabPage(ToolStrip ts, bool enable, bool viewChGirdEnable)
   {
      ts.Items["allCol"].Enabled =
      ts.Items["blueCol"].Enabled =
      ts.Items["redCol"].Enabled =
      ts.Items["resetPlGrid"].Enabled = enable;

      ts.Items["viewChGrid"].Enabled = viewChGirdEnable;
   }

   #endregion ThePolyGridTabControl_SelectionChanged

   #region Hampers

   private void CreateHampers()
   {
      InitializeZaglavljeHamper(out hamp_zaglavlje);
      CreateHamperOpen();
      InitializePanel4RulesHamper();

      InitializeRulesHamper(out hamp_rules);
      panelRules.Size = new Size(hamp_rules.Right - hamp_rules.Left + ZXC.QUN + ZXC.Qun2, hamp_rules.Bottom + ZXC.Qun2);

      nextY = hamp_Open.Bottom;
   }

   private void InitializeZaglavljeHamper(out VvHamper zaglavljeHamper)
   {
      zaglavljeHamper = new VvHamper(17, 3, "", TheTabControl.TabPages[0], true, nextX, nextY, razmakHamp);
      //                                              0        1                 2                    3           4         5                6                   7             8        9        10                 11          12          13              14                   15            16
      zaglavljeHamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un, ZXC.QUN + ZXC.Qun2, ZXC.Q3un + ZXC.Qun2, ZXC.Q2un, ZXC.Q2un , ZXC.QUN + ZXC.Qun2, ZXC.QUN - ZXC.Qun4, ZXC.Q3un, ZXC.Q2un, ZXC.Q5un,  ZXC.QUN - ZXC.Qun4, ZXC.Q2un, ZXC.Q3un, ZXC.QUN + ZXC.Qun2, ZXC.Q2un - ZXC.Qun4, ZXC.Q2un };
      zaglavljeHamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4,           ZXC.Qun8,            ZXC.Qun8, ZXC.Qun8 ,ZXC.Qun8,            ZXC.Qun8,           ZXC.Qun8, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,            ZXC.Qun8, ZXC.Qun4, ZXC.Qun4,           ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4 };
      zaglavljeHamper.VvRightMargin = zaglavljeHamper.VvLeftMargin;

      zaglavljeHamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN, ZXC.QUN };
      zaglavljeHamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      zaglavljeHamper.VvBottomMargin = zaglavljeHamper.VvTopMargin;


                     zaglavljeHamper.CreateVvLabel(0, 0, "Broj Dokumenta:", ContentAlignment.MiddleRight);
      tbx_dokNum   = zaglavljeHamper.CreateVvTextBox(1, 0, "tbx_dokNum", "Ovo bolje ostavi kako je...", 6 /*GetDB_ColumnSize(DB_ci.dokNum)*/);
      tbx_dokNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_dokNum.JAM_FillCharacter = '0';
      tbx_dokNum.JAM_DataRequired = true;

               zaglavljeHamper.CreateVvLabel(3, 0, "Tip Plaće:", ContentAlignment.MiddleRight);
      tbx_tt = zaglavljeHamper.CreateVvTextBoxLookUp(4, 0, "tbx_TT", "Tip Plaće", 2/*GetDB_ColumnSize(DB_ci.tt)*/);
      tbx_tt.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_tt.JAM_DataRequired = true;
      tbx_tt.JAM_MustTabOutBeforeSubmit = true; // zbog Get_ttNum-a 

      // Nota bene; buduci je tbx_tt VvLookUp TextBox onde mu je tbx_tt.JAM_FieldExitMethod 'potrosena' lookUp methodom. 
      tbx_tt.JAM_FieldExitMethod_2 = new EventHandler(OnExit_TtOrMMYYYY_Set_RS_ID);
      tbx_tt.JAM_FieldExitMethod_3 = new EventHandler(OnExit_Tt_SetVrstaObracuna_And_PersonRestrictor);

      tbx_ttOpis = zaglavljeHamper.CreateVvTextBox(5, 0, "tbx_ttOpis", "", 32, 3, 0);
      tbx_ttOpis.JAM_ReadOnly = true;
      tbx_ttNum = zaglavljeHamper.CreateVvTextBox(9, 0, "tbx_TtNum", "", 4/*GetDB_ColumnSize(DB_ci.ttNum)*/);
      tbx_ttNum.JAM_StatusText = "Ovo bolje ostavi kako je...";
      tbx_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_ttNum.JAM_FillCharacter = '0';
      tbx_ttNum.JAM_DataRequired = true;

      tbx_tt.JAM_Set_LookUpTable(ZXC.luiListaPlacaTT, (int)ZXC.Kolona.prva);
      tbx_tt.JAM_lui_NameTaker_JAM_Name = tbx_ttOpis.JAM_Name;
      tbx_tt.JAM_ttNumTaker_JAM_Name    = tbx_ttNum.JAM_Name;

      this.ControlForInitialFocus = tbx_tt;


                    zaglavljeHamper.CreateVvLabel(0, 1, "Datum Isplate:", ContentAlignment.MiddleRight);
      tbx_dokDate = zaglavljeHamper.CreateVvTextBox(1, 1, "tbx_datum", "", GetDB_ColumnSize(DB_ci.dokDate), 1, 0);
      tbx_dokDate.JAM_IsForDateTimePicker = true;
      dTP_datum = zaglavljeHamper.CreateVvDateTimePicker(1, 1, "", 1, 0, tbx_dokDate);
      dTP_datum.Name = "dTP_datum";
      //tbx_dokDate.JAM_FieldExitMethod = new EventHandler(OnExit_DokDateOrMMYYYY_Set_PlacaRules);
      // PAZI: !!!, ako se npr. godina dio vvcb napise '99' onExit je value je jos uvijek stari koji je bio pri ulasku, cak i nakon validated eventa vvcb.value ostane stari. Kada zapravo value postane dobar mi je nepoznato! 
      dTP_datum.Leave += new EventHandler(OnExit_DokDateOrMMYYYY_Set_PlacaRules);

      if(this is PlacaNPDUC)
      {
         tbx_tt.JAM_ReadOnly = true;
         this.ControlForInitialFocus = tbx_ttNum;
      }


                              zaglavljeHamper.CreateVvLabel(3, 1, "Za Mjesec:", ContentAlignment.MiddleRight);
      tbx_lookUpMMYYYY      = zaglavljeHamper.CreateVvTextBoxLookUp( "tbx_MMYYYY", 4, 1, "Obračun plaće za zadani mjesec u godini", GetDB_ColumnSize(DB_ci.mmyyyy),1,0);
      tbx_lookUpMMYYYY.Font = ZXC.vvFont.LargeBoldFont;
      tbx_lookUpMMYYYY.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_lookUpMMYYYY.JAM_DataRequired = true;


                          zaglavljeHamper.CreateVvLabel(6, 1, "Trg:", ContentAlignment.MiddleRight);
      tbx_isTrgFondSati = zaglavljeHamper.CreateVvTextBox(7, 1, "tbx_TrgFondSati", "");
      cbx_isTrgFondSati = zaglavljeHamper.CreateVvCheckBox(7, 1, "", tbx_isTrgFondSati, "", "X");
      tbx_isTrgFondSati.JAM_Highlighted = true;
      cbx_isTrgFondSati.CheckStateChanged += new EventHandler(cbx_isTrgFondSati_CheckStateChanged_SetFondSati);

                     zaglavljeHamper.CreateVvLabel  (8, 1, "Fond Sati:", ContentAlignment.MiddleRight);
      tbx_fondSati = zaglavljeHamper.CreateVvTextBox(9, 1, "tbx_sati", "Fond radin sati za zadani mjesec prema tablici", GetDB_ColumnSize(DB_ci.fondSati));
      tbx_fondSati.JAM_ReadOnly = true;

      //VvLookUpLista fondSatiLista = ZXC.CURR_prjkt_rec.IsTrgRs ? ZXC.luiListaFondSati_TRG : ZXC.luiListaFondSati_NOR;
      VvLookUpLista fondSatiLista = ZXC.luiListaFondSati_NOR;

      tbx_lookUpMMYYYY.JAM_Set_LookUpTable(fondSatiLista, (int)ZXC.Kolona.prva);
      tbx_lookUpMMYYYY.JAM_lui_NameTaker_JAM_Name = tbx_fondSati.JAM_Name;
      tbx_lookUpMMYYYY.JAM_MustTabOutBeforeSubmit = true; // zbog pravilnog GetNextRS_ID 
      tbx_lookUpMMYYYY.JAM_FieldExitMethod_2 = new EventHandler(OnExit_TtOrMMYYYY_Set_RS_ID);
      tbx_lookUpMMYYYY.JAM_FieldExitMethod_3 = new EventHandler(OnExit_DokDateOrMMYYYY_Set_PlacaRules);

                     zaglavljeHamper.CreateVvLabel(0, 2, "Napomena:", ContentAlignment.MiddleRight);
      tbx_napomena = zaglavljeHamper.CreateVvTextBox(1, 2, "tbx_napomena", "Napomena", GetDB_ColumnSize(DB_ci.napomena), 8, 0);


      //cbx_isLocked = zaglavljeHamper.CreateVvCheckBox_OLD(11, 0, null, 1, 0, "Zaključano", RightToLeft.No);
      //cbx_isLocked.Enabled = false;
                     zaglavljeHamper.CreateVvLabel   (10, 0, "Zaključano:", ContentAlignment.MiddleRight);
      tbx_isLocked = zaglavljeHamper.CreateVvTextBox (11, 0, "tbx_isLocked", "");
      cbx_isLocked = zaglavljeHamper.CreateVvCheckBox(11, 0, "", tbx_isLocked, "", "X");
      tbx_isLocked.JAM_Highlighted = true;
      tbx_isLocked.JAM_ReadOnly = true;



                  zaglavljeHamper.CreateVvLabel  (10, 1, "Identifikator:", ContentAlignment.MiddleRight);
      tbx_RSmID = zaglavljeHamper.CreateVvTextBox(11, 1, "tbx_RSmID", "Identifikator / Oznaka izvjesca", 6/*, GetDB_ColumnSize(DB_ci.r)*/, 1, 0);
      tbx_RSmID.Font = ZXC.vvFont.LargeBoldFont;
      tbx_RSmID.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

                           zaglavljeHamper.CreateVvLabel        (13, 1, "Vrsta Obračuna:", 2, 0, ContentAlignment.MiddleRight);
      tbx_lookUpVrstaObr = zaglavljeHamper.CreateVvTextBoxLookUp(16, 1, "tbx_lookUpVrstaObr", "Vrsta Obračuna/Ispravka za RSm obrazac", GetDB_ColumnSize(DB_ci.vrstaObr));
      tbx_lookUpVrstaObr.JAM_Set_LookUpTable(ZXC.luiListaPlacaVrstaObr, (int)ZXC.Kolona.prva);
 
                                 zaglavljeHamper.CreateVvLabel    (13, 0, "Vrsta izvješća:", 2, 0, ContentAlignment.MiddleRight);
      tbx_lookUpVrstaJoppd = zaglavljeHamper.CreateVvTextBoxLookUp(16, 0, "tbx_lookUpVrstaIzvjJoppd", "Vrsta izvjestaja", GetDB_ColumnSize(DB_ci.vrstaJOPPD));
      tbx_lookUpVrstaJoppd.JAM_Set_LookUpTable(ZXC.luiListaVrstaIzvjJoppd, (int)ZXC.Kolona.prva);


                        zaglavljeHamper.CreateVvLabel  (10, 2, "Mjesto Troška:", ContentAlignment.MiddleRight);
      tbx_mtros_cd    = zaglavljeHamper.CreateVvTextBox(11, 2, "tbx_mtros_cd", "Šifra mjesta troška", GetDB_ColumnSize(DB_ci.mtros_cd), 1, 0);
      tbx_mtros_tk    = zaglavljeHamper.CreateVvTextBox(13, 2, "tbx_mtros_tk", "Tiker mjesta troška", GetDB_ColumnSize(DB_ci.mtros_tk));
      tbx_mtros_Naziv = zaglavljeHamper.CreateVvTextBox(14, 2, "tbx_mtros_naziv", "Naziv mjesta troška", 32, 2, 0);

      tbx_mtros_cd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_mtros_tk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_mtros_cd   .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD   .SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_mtros_tk   .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_mtros_Naziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv .SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyMtrosTextBoxLeave));

   }

   private void cbx_isTrgFondSati_CheckStateChanged_SetFondSati(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvCheckBox cbx = sender as VvCheckBox;
      VvLookUpLista fondSatiLista = /*ZXC.CURR_prjkt_rec.IsTrgRs*/ cbx.Checked ? ZXC.luiListaFondSati_TRG : ZXC.luiListaFondSati_NOR;
      
      tbx_lookUpMMYYYY.JAM_Set_LookUpTable(fondSatiLista, (int)ZXC.Kolona.prva);
      
      Fld_MMYYYY = "";
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
            Fld_MtrosCd = kupdob_rec.KupdobCD/*RecID*/;
            Fld_MtrosTk = kupdob_rec.Ticker;
            Fld_MtrosNaziv = kupdob_rec.Naziv;
         }
         else
         {
            Fld_MtrosCdAsTxt = Fld_MtrosTk = Fld_MtrosNaziv = "";
         }
      }
   }

   private void InitializePanel4RulesHamper()
   {
      panelRules = new Panel();
      panelRules.Parent = TheTabControl.TabPages[0];
      panelRules.Location = new Point(hamp_Open.Left, hamp_Open.Top + ZXC.QUN);
      panelRules.BackColor = ZXC.vvColors.hamperRules_BackColor;
      panelRules.Visible = false;
   }

   private void InitializeRulesHamper(out VvHamper hamper)
   {
      //CreateHamperOpen();

      hamper = new VvHamper(23, /*5*/9, "", panelRules /*TheTabControl.TabPages[0]*/ , false);

      hamper.VvColWdt = new int[]{ ZXC.Q3un - ZXC.Qun4, ZXC.Q2un + ZXC.Qun4, ZXC.QUN,
                                   ZXC.Q3un + ZXC.Qun4, ZXC.Q2un + ZXC.Qun4, ZXC.QUN,
                                   ZXC.Q3un - ZXC.Qun2, ZXC.Q2un + ZXC.Qun4, ZXC.QUN,
                                   ZXC.Q4un + ZXC.Qun4, ZXC.Q4un           , 
                                   ZXC.Q5un + ZXC.Qun4, ZXC.Q2un           , ZXC.Q2un,
                                   ZXC.Q5un + ZXC.Qun4, ZXC.Q2un + ZXC.Qun4, ZXC.QUN, 
                                   ZXC.Q3un           , ZXC.Q2un + ZXC.Qun4, ZXC.QUN,
                                   ZXC.Q5un           , ZXC.Q2un + ZXC.Qun4, ZXC.QUN,
                                   //ZXC.Q3un           , ZXC.Q2un + ZXC.Qun4, ZXC.QUN
                                  };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun8,
                                       ZXC.Qun4, ZXC.Qun4, ZXC.Qun8,
                                       ZXC.Qun4, ZXC.Qun4, ZXC.Qun8,
                                       ZXC.Qun4, ZXC.Qun4,
                                       ZXC.Qun4, ZXC.Qun4, ZXC.Qun8,
                                       ZXC.Qun4, ZXC.Qun8, ZXC.Qun8,
                                       ZXC.Qun8, ZXC.Qun8, ZXC.Qun8,
                                       ZXC.Qun4, ZXC.Qun8, ZXC.Qun8,
                                      // ZXC.Qun8, ZXC.Qun8, ZXC.Qun8
                                     };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Location = new Point(ZXC.Qun4, ZXC.Qun4);

      Label lbl_stPor1, lbl_stPor2, lbl_stPor3, lbl_stPor4, lbl_stPor1p, lbl_stPor2p, lbl_stPor3p, lbl_stPor4p,
            lbl_osnOdb, lbl_maxPorOsn1, lbl_maxPorOsn2, lbl_maxPorOsn3,
            lbl_minMioOsn, lbl_maxMioOsn,
            lbl_stMioIz, lbl_stMioIz_II, lbl_stZdrNa, lbl_stZorNa, lbl_stMioIzp, lbl_stMioIz_IIp, lbl_stZdrNap, lbl_stZorNap,
            lbl_stZapNa, lbl_stZapII, lbl_stZpi, lbl_stOthOlak, lbl_stZapNap, lbl_stZapIIp, lbl_stZpip, lbl_stOthOlakp,
            lbl_staz, lbl_stazPost, lbl_granBrRad,
            lbl_stMioNaB1, lbl_stMioNa2B1, lbl_stMioNaB2, lbl_stMioNa2B2,
            lbl_stMioNaB3, lbl_stMioNa2B3, lbl_stMioNaB4, lbl_stMioNa2B4, lbl_prosPlaca, lbl_stMioNa2B5,
            lbl_stMioNaB1p, lbl_stMioNa2B1p, lbl_stMioNaB2p, lbl_stMioNa2B2p,
            lbl_stMioNaB3p, lbl_stMioNa2B3p, lbl_stMioNaB4p, lbl_stMioNa2B4p, /*lbl_stMioNaB5p, */lbl_stMioNa2B5p,
            lbl_stKrizPor1, lbl_stKrizPor2, lbl_stKrizPor1p, lbl_stKrizPor2p, lbl_stZdrDD;


      lbl_stMioIz     = hamper.CreateVvLabel   (0, 0, "MIO I iz:", ContentAlignment.MiddleRight);
      lbl_stMioIz_II  = hamper.CreateVvLabel   (0, 1, "MIO II iz:", ContentAlignment.MiddleRight);
      lbl_stZdrNa     = hamper.CreateVvLabel   (0, 2, "Zdrav na:", ContentAlignment.MiddleRight);
      lbl_stZorNa     = hamper.CreateVvLabel   (0, 3, "ZOR na:", ContentAlignment.MiddleRight);
      lbl_stZdrDD     = hamper.CreateVvLabel   (0, 4, "Zdr DD:", ContentAlignment.MiddleRight);
      tbx_stMioIz     = hamper.CreateVvTextBox (1, 0, "tbx_stMioIz", "Stopa doprinosa za MIO I stup iz plaće",/*GetDB_ColumnSize*/ 5);
      tbx_stMioIz_II  = hamper.CreateVvTextBox (1, 1, "tbx_stMioIz_II", "Stopa doprinosa za MIO II stup iz plaće", 5);
      tbx_stZdrNa     = hamper.CreateVvTextBox (1, 2, "tbx_stZdrNa", "Stopa doprinosa za zdravstveno osiguranje na plaće", 5);
      tbx_stZorNa     = hamper.CreateVvTextBox (1, 3, "tbx_stZorNa", "Stopa doprinosa za ZOR na plaće", 5);
      tbx_stZdrDD     = hamper.CreateVvTextBox (1, 4, "tbx_stZdrDD", "Stopa doprinosa za ZDR na drugi doh.", 5);
      lbl_stMioIzp    = hamper.CreateVvLabel   (2, 0, "%", ContentAlignment.MiddleLeft);
      lbl_stMioIz_IIp = hamper.CreateVvLabel   (2, 1, "%", ContentAlignment.MiddleLeft);
      lbl_stZdrNap    = hamper.CreateVvLabel   (2, 2, "%", ContentAlignment.MiddleLeft);
      lbl_stZorNap    = hamper.CreateVvLabel   (2, 3, "%", ContentAlignment.MiddleLeft);
                        hamper.CreateVvLabel   (2, 4, "%", ContentAlignment.MiddleLeft);
      
                        hamper.CreateVvLabel  ( 0, 4, "Vrijednost koeficijenta za Bruto1:", 9, 0, ContentAlignment.MiddleRight);
      tbx_VrKoefBr1   = hamper.CreateVvTextBox(10, 4, "tbx_VrKoefBr1", "Vrijednost koeficijenta za Bruto1", 12);

      lbl_stZapNa     = hamper.CreateVvLabel  (3, 0, "Zapoš na:", ContentAlignment.MiddleRight);
      lbl_stZapII     = hamper.CreateVvLabel  (3, 1, "Zapoš II na:", ContentAlignment.MiddleRight);
      lbl_stZpi       = hamper.CreateVvLabel  (3, 2, "ZPI na:", ContentAlignment.MiddleRight);
      lbl_stOthOlak   = hamper.CreateVvLabel  (3, 3, "Izdatak:", ContentAlignment.MiddleRight);
      tbx_stZapNa     = hamper.CreateVvTextBox(4, 0, "tbx_stZapNa", "Stopa doprinosa za zapošljavanje na plaće", 5);
      tbx_stZapII     = hamper.CreateVvTextBox(4, 1, "tbx_stZapII", "Poseban doprinos za zapošljavenje na plaće  - preko 20 radnika", 5);
      tbx_stZpi       = hamper.CreateVvTextBox(4, 2, "tbx_stZpi", "Stopa doprinosa za ZPI", 5);
      tbx_stOthOlak   = hamper.CreateVvTextBox(4, 3, "tbx_stOthOlak", "Postotak priznatog izdatka za druge dohotke", 5);
      lbl_stZapNap    = hamper.CreateVvLabel  (5, 0, "%", ContentAlignment.MiddleLeft);
      lbl_stZapIIp    = hamper.CreateVvLabel  (5, 1, "%", ContentAlignment.MiddleLeft);
      lbl_stZpip      = hamper.CreateVvLabel  (5, 2, "%", ContentAlignment.MiddleLeft);
      lbl_stOthOlakp  = hamper.CreateVvLabel  (5, 3, "%", ContentAlignment.MiddleLeft);

      lbl_stPor1      = hamper.CreateVvLabel  (6, 0, "Porez 1:", ContentAlignment.MiddleRight);
      lbl_stPor2      = hamper.CreateVvLabel  (6, 1, "Porez 2:", ContentAlignment.MiddleRight);
      lbl_stPor3      = hamper.CreateVvLabel  (6, 2, "Porez 3:", ContentAlignment.MiddleRight);
      lbl_stPor4      = hamper.CreateVvLabel  (6, 3, "Porez 4:", ContentAlignment.MiddleRight);
      tbx_stPor1      = hamper.CreateVvTextBox(7, 0, "tbx_stPor1", "Stopa poreza na dohodak prvog poreznog razreda", 5);
      tbx_stPor2      = hamper.CreateVvTextBox(7, 1, "tbx_stPor2", "Stopa poreza na dohodak drugog poreznog razreda", 5);
      tbx_stPor3      = hamper.CreateVvTextBox(7, 2, "tbx_stPor3", "Stopa poreza na dohodak trećeg poreznog razreda", 5);
      tbx_stPor4      = hamper.CreateVvTextBox(7, 3, "tbx_stPor4", "Stopa poreza na dohodak četvrtog poreznog razreda", 5);
      lbl_stPor1p     = hamper.CreateVvLabel  (8, 0, "%", ContentAlignment.MiddleLeft);
      lbl_stPor2p     = hamper.CreateVvLabel  (8, 1, "%", ContentAlignment.MiddleLeft);
      lbl_stPor3p     = hamper.CreateVvLabel  (8, 2, "%", ContentAlignment.MiddleLeft);
      lbl_stPor4p     = hamper.CreateVvLabel  (8, 3, "%", ContentAlignment.MiddleLeft);

      lbl_osnOdb      = hamper.CreateVvLabel  ( 9, 0, "OsnovOdbitak:", ContentAlignment.MiddleRight);
      lbl_maxPorOsn1  = hamper.CreateVvLabel  ( 9, 1, "MaxPorOsnov1:", ContentAlignment.MiddleRight);
      lbl_maxPorOsn2  = hamper.CreateVvLabel  ( 9, 2, "MaxPorOsnov2:", ContentAlignment.MiddleRight);
      lbl_maxPorOsn3  = hamper.CreateVvLabel  ( 9, 3, "MaxPorOsnov3:", ContentAlignment.MiddleRight);
      tbx_osnOdb      = hamper.CreateVvTextBox(10, 0, "tbx_osnOdb", "Osnovni osobni odbitak", 12);
      tbx_maxPorOsn1  = hamper.CreateVvTextBox(10, 1, "tbx_maxPorOsn1", "Maximalna porezna osnovica za porez 1", 12);
      tbx_maxPorOsn2  = hamper.CreateVvTextBox(10, 2, "tbx_maxPorOsn2", "Maximalna porezna osnovica za porez 2", 12);
      tbx_maxPorOsn3  = hamper.CreateVvTextBox(10, 3, "tbx_maxPorOsn3", "Maximalna porezna osnovica za porez 3", 12);

      lbl_minMioOsn   = hamper.CreateVvLabel  (11, 0, "Min MIO osnovica:", ContentAlignment.MiddleRight);
      lbl_maxMioOsn   = hamper.CreateVvLabel  (11, 1, "Max MIO osnovica:", ContentAlignment.MiddleRight);
      lbl_staz        = hamper.CreateVvLabel  (11, 2, "Staž na godinu:", ContentAlignment.MiddleRight);
      lbl_stazPost    = hamper.CreateVvLabel  (13, 2, "%", ContentAlignment.MiddleLeft);
      lbl_granBrRad   = hamper.CreateVvLabel  (11, 3, "GraniBrojRadnika:", ContentAlignment.MiddleRight);
      tbx_minMioOsn   = hamper.CreateVvTextBox(12, 0, "tbx_minMioOsn", "Minimalna MIO osnovica", 12, 1, 0);
      tbx_maxMioOsn   = hamper.CreateVvTextBox(12, 1, "tbx_maxMioOsn", "Maximalna MIO osnovica", 12, 1, 0);
      tbx_stDodStaz   = hamper.CreateVvTextBox(12, 2, "tbx_stDodStaz", "Postotak staza na godinu", 5);
      tbx_granBrRad   = hamper.CreateVvTextBox(12, 3, "tbx_granBrRad", "Granični broj radnika za primjenu posebnog doprinosa za zapošljevanje", 5);


      lbl_stMioNaB1  = hamper.CreateVvLabel  (14, 0, "B 12/14 MIO I na:", ContentAlignment.MiddleRight);
      lbl_stMioNaB2  = hamper.CreateVvLabel  (14, 1, "B 12/15 MIO I na:", ContentAlignment.MiddleRight);
      lbl_stMioNaB3  = hamper.CreateVvLabel  (14, 2, "B 12/16 MIO I na:", ContentAlignment.MiddleRight);
      lbl_stMioNaB4  = hamper.CreateVvLabel  (14, 3, "B 12/18 MIO I na:", ContentAlignment.MiddleRight);
      tbx_stMioNaB1  = hamper.CreateVvTextBox(15, 0, "tbx_stMioNaB1", "Stopa doprinosa za MIO I stup na plaće - beneficirani radni staž: 12 mjeseci kao 14 mjeseci", 5);
      tbx_stMioNaB2  = hamper.CreateVvTextBox(15, 1, "tbx_stMioNaB2", "Stopa doprinosa za MIO I stup na plaće - beneficirani radni staž: 12 mjeseci kao 15 mjeseci", 5);
      tbx_stMioNaB3  = hamper.CreateVvTextBox(15, 2, "tbx_stMioNaB3", "Stopa doprinosa za MIO I stup na plaće - beneficirani radni staž: 12 mjeseci kao 16 mjeseci", 5);
      tbx_stMioNaB4  = hamper.CreateVvTextBox(15, 3, "tbx_stMioNaB4", "Stopa doprinosa za MIO I stup na plaće - beneficirani radni staž: 12 mjeseci kao 18 mjeseci", 5);
      lbl_stMioNaB1p = hamper.CreateVvLabel  (16, 0, "%", ContentAlignment.MiddleLeft);
      lbl_stMioNaB2p = hamper.CreateVvLabel  (16, 1, "%", ContentAlignment.MiddleLeft);
      lbl_stMioNaB3p = hamper.CreateVvLabel  (16, 2, "%", ContentAlignment.MiddleLeft);
      lbl_stMioNaB4p = hamper.CreateVvLabel  (16, 3, "%", ContentAlignment.MiddleLeft);


      lbl_stMioNa2B1 = hamper.CreateVvLabel  (17, 0, "MIO II na:", ContentAlignment.MiddleRight);
      lbl_stMioNa2B2 = hamper.CreateVvLabel  (17, 1, "MIO II na:", ContentAlignment.MiddleRight);
      lbl_stMioNa2B3 = hamper.CreateVvLabel  (17, 2, "MIO II na:", ContentAlignment.MiddleRight);
      lbl_stMioNa2B4 = hamper.CreateVvLabel  (17, 3, "MIO II na:", ContentAlignment.MiddleRight);
      tbx_stMioNa2B1 = hamper.CreateVvTextBox(18, 0, "tbx_stMioNa2B1", "Stopa doprinosa za MIO II stup na plaće - beneficirani radni staž: 12 mjeseci kao 14 mjeseci", 5);
      tbx_stMioNa2B2 = hamper.CreateVvTextBox(18, 1, "tbx_stMioNa2B2", "Stopa doprinosa za MIO II stup na plaće - beneficirani radni staž: 12 mjeseci kao 15 mjeseci", 5);
      tbx_stMioNa2B3 = hamper.CreateVvTextBox(18, 2, "tbx_stMioNa2B3", "Stopa doprinosa za MIO II stup na plaće - beneficirani radni staž: 12 mjeseci kao 16 mjeseci", 5);
      tbx_stMioNa2B4 = hamper.CreateVvTextBox(18, 3, "tbx_stMioNa2B4", "Stopa doprinosa za MIO II stup na plaće - beneficirani radni staž: 12 mjeseci kao 18 mjeseci", 5);
      lbl_stMioNa2B1p = hamper.CreateVvLabel (19, 0, "%", ContentAlignment.MiddleLeft);
      lbl_stMioNa2B2p = hamper.CreateVvLabel (19, 1, "%", ContentAlignment.MiddleLeft);
      lbl_stMioNa2B3p = hamper.CreateVvLabel (19, 2, "%", ContentAlignment.MiddleLeft);
      lbl_stMioNa2B4p = hamper.CreateVvLabel (19, 3, "%", ContentAlignment.MiddleLeft);

      lbl_prosPlaca   = hamper.CreateVvLabel  (20, 0, "Prosjecna placa:", ContentAlignment.MiddleRight);
      tbx_prosPlaca   = hamper.CreateVvTextBox(21, 0, "tbx_prosPlaca", "Stopa doprinosa za MIO I stup na plaće - beneficirani radni staž: 12 mjeseci kao 24 mjeseci", 12);
    //lbl_stMioNaB5p  = hamper.CreateVvLabel  (22, 0, "%", ContentAlignment.MiddleLeft);
      lbl_stMioNa2B5  = hamper.CreateVvLabel  (20, 1, "OsnDopČlanUprave:", ContentAlignment.MiddleRight);
      tbx_stMioNa2B5  = hamper.CreateVvTextBox(21, 1, "tbx_stMioNa2B5", "", 12);
    //lbl_stMioNa2B5  = hamper.CreateVvLabel  (20, 1, "MIO II na:", ContentAlignment.MiddleRight);
    //tbx_stMioNa2B5  = hamper.CreateVvTextBox(21, 1, "tbx_stMioNa2B5", "Stopa doprinosa za MIO II stup na plaće - beneficirani radni staž: 12 mjeseci kao 24 mjeseci", 5);
    //lbl_stMioNa2B5p = hamper.CreateVvLabel  (22, 1, "%", ContentAlignment.MiddleLeft);

      lbl_stKrizPor1  = hamper.CreateVvLabel(20, 2, "KrizPor1:", ContentAlignment.MiddleRight);
      lbl_stKrizPor2  = hamper.CreateVvLabel(20, 3, "KrizPor2:", ContentAlignment.MiddleRight);
      tbx_stKrizPor1  = hamper.CreateVvTextBox(21, 2, "tbx_stKrizPor1", "Stopa kriznog poreza 1", 5);
      tbx_stKrizPor2  = hamper.CreateVvTextBox(21, 3, "tbx_stKrizPor2", "Stopa kriznog poreza 2", 5);
      lbl_stKrizPor1p = hamper.CreateVvLabel(22, 2, "%", ContentAlignment.MiddleLeft);
      lbl_stKrizPor2p = hamper.CreateVvLabel(22, 3, "%", ContentAlignment.MiddleLeft);

      tbx_stMioIz    .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stMioIz_II .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stZdrNa    .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stZdrDD    .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stZorNa    .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stZapNa    .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stZapII    .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stZpi      .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stOthOlak  .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stPor1     .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stPor2     .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stPor3     .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stPor4     .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stDodStaz  .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stMioNaB1  .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stMioNaB2  .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stMioNaB3  .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stMioNaB4  .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_prosPlaca  .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stMioNa2B1 .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stMioNa2B2 .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stMioNa2B3 .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stMioNa2B4 .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stMioNa2B5 .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_osnOdb     .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_maxPorOsn1 .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_maxPorOsn2 .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_maxPorOsn3 .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_minMioOsn  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_maxMioOsn  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_stKrizPor1 .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stKrizPor2 .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_VrKoefBr1  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      tbx_stMioIz   .JAM_ReadOnly =
      tbx_stMioIz_II.JAM_ReadOnly =
      tbx_stZdrNa   .JAM_ReadOnly =
      tbx_stZdrDD   .JAM_ReadOnly =
      tbx_stZorNa   .JAM_ReadOnly =
      tbx_stZapNa   .JAM_ReadOnly =
      tbx_stZapII   .JAM_ReadOnly =
      tbx_stZpi     .JAM_ReadOnly =
      tbx_stOthOlak .JAM_ReadOnly =
      tbx_stPor1    .JAM_ReadOnly =
      tbx_stPor2    .JAM_ReadOnly =
      tbx_stPor3    .JAM_ReadOnly =
      tbx_stPor4    .JAM_ReadOnly =
      tbx_osnOdb    .JAM_ReadOnly =
      tbx_maxPorOsn1.JAM_ReadOnly =
      tbx_maxPorOsn2.JAM_ReadOnly =
      tbx_maxPorOsn3.JAM_ReadOnly =
      tbx_minMioOsn .JAM_ReadOnly =
      tbx_maxMioOsn .JAM_ReadOnly =
      tbx_stDodStaz .JAM_ReadOnly =
      tbx_granBrRad .JAM_ReadOnly =
      tbx_stMioNaB1 .JAM_ReadOnly =
      tbx_stMioNaB2 .JAM_ReadOnly =
      tbx_stMioNaB3 .JAM_ReadOnly =
      tbx_stMioNaB4 .JAM_ReadOnly =
      tbx_prosPlaca .JAM_ReadOnly =
      tbx_stMioNa2B1.JAM_ReadOnly =
      tbx_stMioNa2B2.JAM_ReadOnly =
      tbx_stMioNa2B3.JAM_ReadOnly =
      tbx_stMioNa2B4.JAM_ReadOnly =
      tbx_stMioNa2B5.JAM_ReadOnly =
      tbx_stKrizPor1.JAM_ReadOnly =
      tbx_stKrizPor2.JAM_ReadOnly =
      tbx_VrKoefBr1 .JAM_ReadOnly = true;

      hamper.CreateVvLabel(0, 5, "Mio1Granica1:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 6, "Mio1Granica2:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 7, "Mio1FiksOlk :", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 8, "Mio1KoefOlk :", ContentAlignment.MiddleRight);

      tbx_mio1Granica1 = hamper.CreateVvTextBox(10, 5, "tbx_mio1Granica1", "Mio1Granica1", 12);
      tbx_mio1Granica2 = hamper.CreateVvTextBox(10, 6, "tbx_mio1Granica2", "Mio1Granica2", 12);
      tbx_mio1FiksOlk  = hamper.CreateVvTextBox(10, 7, "tbx_mio1FiksOlk ", "Mio1FiksOlk ", 12);
      tbx_mio1KoefOlk  = hamper.CreateVvTextBox(10, 8, "tbx_mio1KoefOlk ", "Mio1KoefOlk ", 12);
     
      tbx_mio1Granica1.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_mio1Granica2.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_mio1FiksOlk .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_mio1KoefOlk .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      tbx_mio1Granica1.JAM_ReadOnly =
      tbx_mio1Granica2.JAM_ReadOnly =
      tbx_mio1FiksOlk .JAM_ReadOnly =
      tbx_mio1KoefOlk .JAM_ReadOnly = true;

      hamper.Visible = false;
   }

   private void CreateHamperOpen()
   {
      hamp_Open = new VvHamper(2, 1, "", TheTabControl.TabPages[0], false);
      hamp_Open.Location = new Point(nextX + ZXC.QunMrgn, hamp_zaglavlje.Bottom);

      hamp_Open.VvColWdt = new int[] { ZXC.QUN, ZXC.Q3un - ZXC.Qun2 };
      hamp_Open.VvSpcBefCol = new int[] { 0, ZXC.Qun4 };
      hamp_Open.VvRightMargin = hamp_Open.VvLeftMargin;

      hamp_Open.VvRowHgt = new int[] { ZXC.QUN };
      hamp_Open.VvSpcBefRow = new int[] { 0 };
      hamp_Open.VvBottomMargin = hamp_Open.VvTopMargin;

      btn_PlusMinus = hamp_Open.CreateVvButton(0, 0, new EventHandler(btn_PlusMinus_Click), "+");
      btn_PlusMinus.Tag = "PlusMinus";
      Label lblNaslov = hamp_Open.CreateVvLabel(1, 0, "Pravila", ContentAlignment.MiddleLeft);

      lblCrta = new Label();
      lblCrta.Parent = TheTabControl.TabPages[0];
      lblCrta.Location = new Point(hamp_Open.Right, hamp_Open.Top + ZXC.Qun4 + ZXC.Qun5);
      lblCrta.Size = new Size(ZXC.Q4un, 1);
      lblCrta.BackColor = Color.LightGray;

   }

   private void btn_PlusMinus_Click(object sender, System.EventArgs e)
   {
      Button btn = sender as Button;

      if(btn.Text == "+")
      {
         btn.Text = "-";
         hamp_rules.Visible = true;
         panelRules.Visible = true;
         lblCrta.Visible = false;

         nextY = panelRules.Bottom;

      }
      else
      {
         btn.Text = "+";
         hamp_rules.Visible = false;
         panelRules.Visible = false;
         lblCrta.Visible = true;

         nextY = hamp_Open.Bottom;
      }

      ThePolyGridTabControl.Location = new Point(ZXC.QunMrgn, nextY + ZXC.QunMrgn);
      ThePolyGridTabControl.Height = ThePolyGridTabControl.Parent.Height - nextY - ZXC.QUN;
   }

   #endregion Hampers

   #region TheGrid_Columns

   #region InitializeTheGrid_Columns()

   protected void ColumnForScroll_CreateColumn(int _width)
   {
      colScrol = TheG.CreateScrollColumn("scrol", _width);
      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates("scrol", true, true));
      colScrol.ReadOnly = true;
   }

   #endregion InitializeTheGrid_Columns()

   #region T_Columns

   /*07*/
   protected void T_personCD_CreateColumn(int _width)
   {
      vvtbT_personCD = TheG.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColT_personCD", TheVvDaoTrans, /*DB_Tci.t_personCD*/ -4, "Šifra radnika");
      vvtbT_personCD.JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterSifra), new EventHandler(AnyPersonTextBoxLeave));
      vvtbT_personCD.JAM_DataRequired = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_personCD, TheVvDaoTrans, DB_Tci.t_personCD, "Šifra", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, true));

      colVvText.Frozen = true;
   }

   /*08*/
   protected void T_ime_CreateColumn(int _width)
   {
      int nazivMaxLen = ZXC.PersonDao.GetSchemaColumnSize(ZXC.PerCI.ime);

      vvtbT_ime = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_ime", TheVvDaoTrans, (-1 * nazivMaxLen), "Ime radnika");
      // Just for testing: 
      //vvtbT_ime = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_ime", TheVvDaoTrans, DB_Tci.t_ime      , "Ime radnika");
      //vvtbT_ime.JAM_Set_LookUpTable(ZXC.luiListaOpcina, (int)ZXC.Kolona.prva);
      //vvtbT_ime.JAM_lui_NameTaker_JAM_Name    = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_prezime);
      //vvtbT_ime.JAM_lui_NumberTaker_JAM_Name  = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_personCD);
      vvtbT_ime.JAM_ReadOnly = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_ime, TheVvDaoTrans, DB_Tci.t_ime, "Ime", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));

   }

   /*09*/
   protected void T_prezime_CreateColumn(int _width)
   {
      vvtbT_prezime = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_prezime", TheVvDaoTrans, DB_Tci.t_prezime, "Prezime radnika");
      vvtbT_prezime.JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonTextBoxLeave));

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_prezime, TheVvDaoTrans, DB_Tci.t_prezime, "Prezime", _width);
      //colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      //colVvText.MinimumWidth = ZXC.Q6un;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));

      colVvText.Frozen = true;

      // 13.02.2020: HZTK... 
      // da, dodao ovu ExitMethod ali ipak kasnije ugasio jer ne treba / nesmije?! 
      // na neku foru stvar proradi (čiščenje ', Robert' djela stringa i bez ovoga)
      // AnyPersonTextBoxLeave obavi svoje 
      //vvtbT_prezime.JAM_FieldExitMethod = new EventHandler(OnExitPrezime_ClearPreffix);

   }

   public void OnExitPrezime_ClearPreffix(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      string dirtyString = "", thePrezime;
      int rowIdx = -1;

      VvTextBoxEditingControl vvTbPrezime = null;
      

      vvTbPrezime = sender as VvTextBoxEditingControl;

      if(vvTbPrezime == null) return;

      dirtyString = vvTbPrezime.Text;

      rowIdx = vvTbPrezime.EditingControlDataGridView.CurrentRow.Index;

      int commaIdx = dirtyString.IndexOf(',');

      // 22.03.2018: 
    //if(dirtyString.Length.IsZero () || spaceIdx.IsNegative      ())   return;
      if(dirtyString.Length.NotZero() && commaIdx.IsZeroOrPositive()) //return;
      { 
         thePrezime = dirtyString.Substring(0, commaIdx);
      }
      else
      {
         thePrezime = dirtyString;
      }

      //vvTbKonto.Text = cleanString;
      //TheG.EditingControl.Text = cleanString;
      TheG.PutCell(ci.iT_prezime, rowIdx, thePrezime);

   }


   protected void R_prezimeIme_CreateColumn(int _width)
   {
      vvtbT_prezimeIme = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_prezimeIme", null, -16, "PrezimeIme radnika");
      vvtbT_prezimeIme.JAM_ReadOnly = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_prezimeIme, null, "R_prezimeIme", "Djelatnik", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }


   /*10*/
   protected void T_brutoOsn_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_brutoOsn = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_brutoOsn", TheVvDaoTrans, DB_Tci.t_brutoOsn, "Bruto osnova");
      vvtbT_brutoOsn.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_brutoOsn, TheVvDaoTrans, DB_Tci.t_brutoOsn, "BRUTO 1", _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth // ako ce se zbrajati
      colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*11*/
   protected void T_topObrok_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_topObrok = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_topObrok", TheVvDaoTrans, DB_Tci.t_topObrok, "Topli obrok");
      vvtbT_topObrok.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_topObrok, TheVvDaoTrans, DB_Tci.t_topObrok, "TopObr", _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth // ako ce se zbrajati

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*12*/
   protected void T_godStaza_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_godStaza = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_godStaza", TheVvDaoTrans, DB_Tci.t_godStaza, "Godine staža");
      vvtbT_godStaza.JAM_ShouldCalcTrans = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_godStaza, TheVvDaoTrans, DB_Tci.t_godStaza, "GodSt", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*13*/
   protected void T_dodBruto_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_dodBruto = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_dodBruto", TheVvDaoTrans, DB_Tci.t_dodBruto, "Dodatak na bruto - unos u fiksnom iznosu ili preko kolone StDodBr-stopa dodatka na bruto");
      vvtbT_dodBruto.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dodBruto, TheVvDaoTrans, DB_Tci.t_dodBruto, "DodBruto", _width);
      //vvtbT_dodBruto.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth // ako ce se zbrajati

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*14*/
   protected void T_isMioII_CreateColumn(int _width)
   {
      vvcbx_isMioII = new VvCheckBox();
      vvcbx_isMioII.JAM_ShouldCalcTrans = true; // vidi komentar na VvDtpAndCheckBox, line: 185 
      colCbox = TheG.CreateVvCheckBoxColumn(vvcbx_isMioII, TheVvDaoTrans, DB_Tci.t_isMioII, "JeMioII", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colCbox.Name, false, true));
   }

   /*15*/
   protected void T_spc_CreateColumn(int _width)
   {
      vvtbT_spc = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_spc", TheVvDaoTrans, DB_Tci.t_spc, "Spec: N-novozaposleni radnik, U-umirovljenik, M-zanemari MinMioOsn: obr. na zadani Bruto, X-Novoza i Minmio, O-zanemari MaxMioOsn, obr.na zadani bruto, C-Clan uprave, manji ili jednaki bruto od osnovice za doprinose, I-Izaslan Radnik");
    //vvtbT_spc.JAM_AllowedInputCharacters = "UNMO"; 30.01.2017. clan uprave ima drugaciju mio osnovicu
      vvtbT_spc.JAM_AllowedInputCharacters = "UNMOCXI";
      vvtbT_spc.JAM_CharacterCasing = CharacterCasing.Upper;
      vvtbT_spc.JAM_ShouldCalcTrans = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_spc, TheVvDaoTrans, DB_Tci.t_spc, "Spc", _width);
      colVvText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*16*/
   protected void T_koef_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_koef = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_koef", TheVvDaoTrans, DB_Tci.t_koef, "Koeficijent osobnog odbitka");
      vvtbT_koef.JAM_ShouldCalcTrans = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_koef, TheVvDaoTrans, DB_Tci.t_koef, "KfOsO", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*17*/
   protected void T_zivotno_CreateColumn(int _width, int numOfDecimalPlaces, string colHeader, string description, bool shouldCalcTransAndSumGrid)
   {
      vvtbT_zivotno = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_zivotno", TheVvDaoTrans, DB_Tci.t_zivotno, description);
      vvtbT_zivotno.JAM_ShouldCalcTransAndSumGrid = shouldCalcTransAndSumGrid;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_zivotno, TheVvDaoTrans, DB_Tci.t_zivotno, colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth // ako ce se zbrajati

      vvtbT_zivotno.JAM_ForeColor = Color.BlueViolet;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*18*/
 //protected void T_dopZdr_CreateColumn(int _width, int numOfDecimalPlaces)
   protected void T_dopZdr_CreateColumn(int _width, int numOfDecimalPlaces, string header, string opis)
   {
      vvtbT_dopZdr = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_dopZdr", TheVvDaoTrans, DB_Tci.t_dopZdr, opis);
      vvtbT_dopZdr.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dopZdr, TheVvDaoTrans, DB_Tci.t_dopZdr, header, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth // ako ce se zbrajati

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*19*/
 //protected void T_dobMIO_CreateColumn(int _width, int numOfDecimalPlaces)
   protected void T_dobMIO_CreateColumn(int _width, int numOfDecimalPlaces, string header, string opis)
   {
      vvtbT_dobMIO = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_dobMIO", TheVvDaoTrans, DB_Tci.t_dobMIO, opis);
      vvtbT_dobMIO.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dobMIO, TheVvDaoTrans, DB_Tci.t_dobMIO, header, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth // ako ce se zbrajati

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*20*/
   protected void T_koefHRVI_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_koefHRVI = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_koefHRVI", TheVvDaoTrans, DB_Tci.t_koefHRVI, "Koeficijent HRVI");
      vvtbT_koefHRVI.JAM_ShouldCalcTrans = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_koefHRVI, TheVvDaoTrans, DB_Tci.t_koefHRVI, "HRVI", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*21*/
   protected void T_invalidTip_CreateColumn(int _width)
   {
      vvtbT_invalidTip = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_invalid", TheVvDaoTrans, DB_Tci.t_invalidTip, "H - Hrvatski ratni vojni invalid; I - Invalid");
      vvtbT_invalidTip.JAM_AllowedInputCharacters = "HI";
      vvtbT_invalidTip.JAM_CharacterCasing = CharacterCasing.Upper;
      vvtbT_invalidTip.JAM_ShouldCalcTrans = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_invalidTip, TheVvDaoTrans, DB_Tci.t_invalidTip, "I", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*22*/
   protected void T_opcCD_CreateColumn_ORIG(int _width)
   {
      vvtbT_opcCD = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_opcCD", TheVvDaoTrans, DB_Tci.t_opcCD, "Šifra općine");
      vvtbT_opcCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      vvtbT_opcCD.JAM_Set_LookUpTable(ZXC.luiListaOpcina, (int)ZXC.Kolona.prva);
      vvtbT_opcCD.JAM_ShouldCalcTrans = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_opcCD, TheVvDaoTrans, DB_Tci.t_opcCD, "SifOpć", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   protected void T_opcCD_CreateColumn(int _width)
   {
      vvtbT_opcCD = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_opcCD", TheVvDaoTrans, DB_Tci.t_opcCD, "Šifra općine");
      vvtbT_opcCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      vvtbT_opcCD.JAM_Set_LookUpTable(ZXC.luiListaOpcina, (int)ZXC.Kolona.prva);
      vvtbT_opcCD.JAM_ShouldCalcTrans = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_opcCD, TheVvDaoTrans, DB_Tci.t_opcCD, "SifOpć", _width);

    //vvtbT_opcName.JAM_lui_CdTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_opcCD);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*23*/
   protected void T_opcName_CreateColumn_ORIG(int _width)
   {
      vvtbT_opcName = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_opcName", TheVvDaoTrans, DB_Tci.t_opcName, "Naziv općine");
      vvtbT_opcName.JAM_ReadOnly = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_opcName, TheVvDaoTrans, DB_Tci.t_opcName, "Općina ", _width);

      vvtbT_opcCD.JAM_lui_NameTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_opcName);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   protected void T_opcName_CreateColumn(int _width)
   {
      vvtbT_opcName = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_opcName", TheVvDaoTrans, DB_Tci.t_opcName, "Naziv općine");
      vvtbT_opcName.JAM_Set_LookUpTable(ZXC.luiListaOpcina, (int)ZXC.Kolona.druga, true);
      vvtbT_opcName.JAM_ShouldCalcTrans = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_opcName, TheVvDaoTrans, DB_Tci.t_opcName, "Općina", _width);

      vvtbT_opcCD  .JAM_lui_NameTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_opcName);
      vvtbT_opcName.JAM_lui_CdTaker_JAM_Name   = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_opcCD  );

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*24*/
   protected void T_opcRadCD_CreateColumn(int _width)
   {
      vvtbT_opcRadCD = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_opcRadCD", TheVvDaoTrans, DB_Tci.t_opcRadCD, "Šifra općine rada");
      vvtbT_opcRadCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      vvtbT_opcRadCD.JAM_Set_LookUpTable(ZXC.luiListaOpcina, (int)ZXC.Kolona.prva);

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_opcRadCD, TheVvDaoTrans, DB_Tci.t_opcRadCD, "SOpR", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*25*/
   protected void T_opcRadName_CreateColumn_ORIG(int _width)
   {
      vvtbT_opcRadName = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_opcRadName", TheVvDaoTrans, DB_Tci.t_opcRadName, "Naziv općine rada");
      vvtbT_opcRadName.JAM_ReadOnly = true;
      vvtbT_opcRadCD.JAM_lui_NameTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_opcRadName);

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_opcRadName, TheVvDaoTrans, DB_Tci.t_opcRadName, "OpRada", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }
   protected void T_opcRadName_CreateColumn(int _width)
   {
      vvtbT_opcRadName = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_opcRadName", TheVvDaoTrans, DB_Tci.t_opcRadName, "Naziv općine");
      vvtbT_opcRadName.JAM_Set_LookUpTable(ZXC.luiListaOpcina, (int)ZXC.Kolona.druga, true);
      vvtbT_opcRadName.JAM_ShouldCalcTrans = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_opcRadName, TheVvDaoTrans, DB_Tci.t_opcRadName, "OpRada", _width);

      vvtbT_opcRadCD.JAM_lui_NameTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_opcRadName);
      vvtbT_opcRadName.JAM_lui_CdTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_opcRadCD);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }


   /*26*/
   protected void T_stPrirez_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_stPrirez = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_stPrirez", TheVvDaoTrans, DB_Tci.t_stPrirez, "Stopa prireza");
      vvtbT_stPrirez.JAM_ReadOnly = true;
      vvtbT_stPrirez.JAM_ShouldCalcTrans = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_stPrirez, TheVvDaoTrans, DB_Tci.t_stPrirez, "StPrir", _width);

      vvtbT_opcCD  .JAM_lui_NumberTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_stPrirez);
      vvtbT_opcName.JAM_lui_NumberTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_stPrirez);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*27*/
   protected void T_netoAdd_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      string colHeader = this is PlacaNPDUC ? "IznosNP" : "NetoDod";
      vvtbT_netoAdd = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_netoAdd", TheVvDaoTrans, DB_Tci.t_netoAdd, "Dodatak na neto");
      vvtbT_netoAdd.JAM_ShouldSumGrid = true;
      vvtbT_netoAdd.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_netoAdd, TheVvDaoTrans, DB_Tci.t_netoAdd, colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth // ako ce se zbrajati

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*28*/
   protected void T_isDirNeto_CreateColumn(int _width)
   {
      //vvcb_isDirNetto = new VvCheckBox("N", "D");

      vvcbx_isDirNeto = new VvCheckBox();
      vvcbx_isDirNeto.JAM_ShouldCalcTrans = true; // vidi komentar na VvDtpAndCheckBox, line: 185 
      colCbox = TheG.CreateVvCheckBoxColumn(vvcbx_isDirNeto, TheVvDaoTrans, DB_Tci.t_isDirNeto, "DirN", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colCbox.Name, false, true));
   }

   /*29*/
   protected void T_prijevoz_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_prijevoz = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_prijevoz", TheVvDaoTrans, DB_Tci.t_prijevoz, "Prijevoz");
      vvtbT_prijevoz.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_prijevoz, TheVvDaoTrans, DB_Tci.t_prijevoz, "Prijevoz", _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth // ako ce se zbrajati

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*30*/
   protected void T_IsPoluSat_CreateColumn(int _width)
   {
      vvcbx_IsPoluSat = new VvCheckBox();
      vvcbx_IsPoluSat.JAM_ShouldCalcTrans = true; // vidi komentar na VvDtpAndCheckBox, line: 185 
      colCbox = TheG.CreateVvCheckBoxColumn(vvcbx_IsPoluSat, TheVvDaoTrans, DB_Tci.t_isPoluSat, "PoluSat", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colCbox.Name, false, true));
   }

   /* 34 */
   protected void T_rsB_CreateColumn(int _width)
   {
      vvtbT_rsB = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_rsB", TheVvDaoTrans, DB_Tci.t_rsB, "Beneficirani radni staž 0-5");
      vvtbT_rsB.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      vvtbT_rsB.JAM_Set_LookUpTable(ZXC.luiListaStazBRsm, (int)ZXC.Kolona.prva);
      vvtbT_rsB.JAM_ShouldCalcTrans = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_rsB, TheVvDaoTrans, DB_Tci.t_rsB, "RSmB", _width);
      colVvText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }

   /* 35 */
   protected void T_nacIsplCD_CreateColumn(int _width)
   {
      vvtbT_nacIsplCD = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_nacIsplCD", TheVvDaoTrans, DB_Tci.t_nacIsplCD, "Način isplate");
      vvtbT_nacIsplCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      vvtbT_nacIsplCD.JAM_Set_LookUpTable(ZXC.luiListaNacIspl, (int)ZXC.Kolona.prva);
      vvtbT_nacIsplCD.JAM_ForeColor = ZXC.vvColors.tabPage4TheG_BackColor;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_nacIsplCD, TheVvDaoTrans, DB_Tci.t_nacIsplCD, "NacISpl", _width);
      colVvText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }
   protected void R_nacIsplName_CreateColumn(int _width)
   {
      vvtbT_nacIsplName = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_nacIsplName", null, -12, "Opis načina isplate");
      vvtbT_nacIsplName.JAM_ReadOnly = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_nacIsplName, null, "R_nacIsplName", "Način isplate", _width);

      vvtbT_nacIsplCD.JAM_lui_NameTaker_JAM_Name = "R_nacIsplName";
      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }


   /* 36 */
   protected void T_neoPrimCD_CreateColumn(int _width)
   {
      vvtbT_neoPrimCD = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_neoPrimCD", TheVvDaoTrans, DB_Tci.t_neoPrimCD, "Oznaka neoporezivog primitka - JOPPD strB");
      vvtbT_neoPrimCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      vvtbT_neoPrimCD.JAM_Set_LookUpTable(ZXC.luiListaNeoporPrim, (int)ZXC.Kolona.prva);
      vvtbT_neoPrimCD.JAM_ForeColor = ZXC.vvColors.tabPage4TheG_BackColor;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_neoPrimCD, TheVvDaoTrans, DB_Tci.t_neoPrimCD, "OznNP", _width);
      colVvText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }
   protected void R_neoPrimName_CreateColumn(int _width)
   {
      vvtbT_neoPrimName = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_neoPrimName", null, -12, "Opis neoporezivog primitka");
      vvtbT_neoPrimName.JAM_ReadOnly = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_neoPrimName, null, "R_neoPrimName", "NeoporezPrimitak", _width);

      vvtbT_neoPrimCD.JAM_lui_NameTaker_JAM_Name = "R_neoPrimName";
      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }


   /* 37 */
   protected void T_dokumCD_CreateColumn(int _width)
   {
      vvtbT_dokumCD = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_dokumCD", TheVvDaoTrans, DB_Tci.t_dokumCD, "Dokument");

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dokumCD, TheVvDaoTrans, DB_Tci.t_dokumCD, "Dokument", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   protected void AnyPersonTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      Person person_rec;
      //     PersonDokumData osrStat_rec;

      VvTextBoxEditingControl vvtb_editingControl = sender as VvTextBoxEditingControl;

      if(vvtb_editingControl == null) return;

      if(vvtb_editingControl.Text == this.originalText) return;

      VvDataGridView theGrid = ((VvDataGridView)vvtb_editingControl.EditingControlDataGridView);

      this.originalText = vvtb_editingControl.Text;
      person_rec = PersonSifrar.Find(FoundInSifrar<Person>);

      int currRow = vvtb_editingControl.EditingControlRowIndex;

      if(person_rec != null && vvtb_editingControl.Text != "")
      {
         theGrid.PutCell(theGrid.IdxForColumn("T_personCD"), currRow, person_rec.PersonCD);
         theGrid.PutCell(theGrid.IdxForColumn("T_prezime"), currRow, person_rec.Prezime);
         theGrid.PutCell(theGrid.IdxForColumn("T_ime"), currRow, person_rec.Ime);
      }
      else
      {
         theGrid.PutCell(theGrid.IdxForColumn("T_personCD"), currRow, 0);
         theGrid.PutCell(theGrid.IdxForColumn("T_prezime"), currRow, "");
         theGrid.PutCell(theGrid.IdxForColumn("T_ime"), currRow, "");
      }

      
      //******* 05.11.2014. da opcinaCD i opcRadCd dojde sa eventualno prethodnog obracuna

      ZXC.DbNavigationRestrictor dbNavigationRestrictor = new ZXC.DbNavigationRestrictor("t_tt", new string[] { Fld_TT, Placa.TT_REDOVANRAD, Placa.TT_PODUZETPLACA, Placa.TT_AUTORHONOR, Placa.TT_AUTORHONUMJ, Placa.TT_AHSAMOSTUMJ, Placa.TT_NADZORODBOR, Placa.TT_TURSITVIJECE, Placa.TT_PLACAUNARAVI });

      // 06.10.2017: 
      if(person_rec == null) person_rec = new Person();

      Ptrans prevPtrans_rec = PersonDao.GetPrevPtransForPerson(TheDbConnection, person_rec.PersonCD, Fld_DokDate, Fld_DokNum, dbNavigationRestrictor);
      bool personImaPrevPtrans = prevPtrans_rec.VirtualRecID != 0;
      if(personImaPrevPtrans)
      {
         theGrid.PutCell(theGrid.IdxForColumn("T_opcCD"   ), currRow, prevPtrans_rec.T_opcCD   );
         theGrid.PutCell(theGrid.IdxForColumn("T_opcRadCD"), currRow, prevPtrans_rec.T_opcRadCD);

         // 26.09.2016: tek 
         if(true)
         {
            VvLookUpItem lui;
            
            lui = ZXC.luiListaOpcina.GetLuiForThisCd(prevPtrans_rec.T_opcCD   ); if(lui != null) { theGrid.PutCell(theGrid.IdxForColumn("T_opcName")   , currRow, lui.Name); theGrid.PutCell(theGrid.IdxForColumn("T_stPrirez"), currRow, lui.Number);}
            lui = ZXC.luiListaOpcina.GetLuiForThisCd(prevPtrans_rec.T_opcRadCD); if(lui != null) { theGrid.PutCell(theGrid.IdxForColumn("T_opcRadName"), currRow, lui.Name); }

            theGrid.PutCell(theGrid.IdxForColumn("T_koef")   , currRow,                           prevPtrans_rec.T_koef    );
            theGrid.PutCell(theGrid.IdxForColumn("T_isMioII"), currRow, VvCheckBox.GetString4Bool(prevPtrans_rec.T_isMioII));
         }

      }

      //******* 05.11.2014. da opcinaCD i opcRadCd dojde sa eventualno prethodnog obracuna



      // samo za DUC-eve
      ZXC.TheVvForm.SetDirtyFlag(sender);
   }
   
   /*38*/
   protected void T_brutoDodSt_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_brutoDodSt = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_brutoDodSt", TheVvDaoTrans, DB_Tci.t_brutoDodSt, "Stopa dodatka na bruto (stimulacije)");
      vvtbT_brutoDodSt.JAM_ShouldCalcTrans = true;
      vvtbT_brutoDodSt.JAM_IsForPercent = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_brutoDodSt, TheVvDaoTrans, DB_Tci.t_brutoDodSt, "StDodBr", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*39*/
   protected void T_brDodpoloz_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_brDodpoloz = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_brDodpoloz", TheVvDaoTrans, DB_Tci.t_brDodPoloz, "Položajni dodatak na bruto - ne ulazi u osnovicu obračuna dodatka na minuli rad");
      vvtbT_brDodpoloz.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_brDodpoloz, TheVvDaoTrans, DB_Tci.t_brDodPoloz, "PolDodBrt", _width);
      //vvtbT_dodBruto.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth // ako ce se zbrajati

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   /*40*/
   protected void T_koefBruto1_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_koefBruto1 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_koefBruto1", TheVvDaoTrans, DB_Tci.t_koefBruto1, "Koeficijent za Bruto1");

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_koefBruto1, TheVvDaoTrans, DB_Tci.t_koefBruto1, "KfBr1", _width);
      //colVvText.Visible = IsBruto1ViaKoef;
      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, IsBruto1ViaKoef));
   }

   /*41*/
   protected void T_dnFondSati_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_dnFondSati = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_dnFondSati", TheVvDaoTrans, DB_Tci.t_dnFondSati, "Dnevni fond sati za nepuno radno, za obr. dnevnih minuta dijeli se sat sa minutama npr. za 30min 60:30=0.50, za 24min 60:24=0.40 ");

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dnFondSati, TheVvDaoTrans, DB_Tci.t_dnFondSati, "DFS", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));

   }

   /*42*/
   protected void T_thisStazSt_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_thisStazSt = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_thisStazSt", TheVvDaoTrans, DB_Tci.t_thisStazSt, "Stopa dodatka na neprekinuti staž kod istog poslodavca - računa se na Bruto1");
      vvtbT_thisStazSt.JAM_ShouldCalcTrans = true;
      vvtbT_thisStazSt.JAM_IsForPercent = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_thisStazSt, TheVvDaoTrans, DB_Tci.t_thisStazSt, "StDodStz", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   protected void T_brutoDodSt2_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_brutoDodSt2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_brutoDodSt2", TheVvDaoTrans, DB_Tci.t_brutoDodSt2, "Stopa dodatka na Bruto1");
      vvtbT_brutoDodSt2.JAM_ShouldCalcTrans = true;
      vvtbT_brutoDodSt2.JAM_IsForPercent = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_brutoDodSt2, TheVvDaoTrans, DB_Tci.t_brutoDodSt2, "StD1", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }
   
   protected void T_brutoDodSt3_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_brutoDodSt3 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_brutoDodSt3", TheVvDaoTrans, DB_Tci.t_brutoDodSt3, "Stopa dodatka na Bruto1");
      vvtbT_brutoDodSt3.JAM_ShouldCalcTrans = true;
      vvtbT_brutoDodSt3.JAM_IsForPercent = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_brutoDodSt3, TheVvDaoTrans, DB_Tci.t_brutoDodSt3, "StD2", _width);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }
      
   protected void T_pr3mjBruto_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_pr3mjBruto = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_pr3mjBruto", TheVvDaoTrans, DB_Tci.t_pr3mjBruto, "Prosjek 3 zadnje bruto plaće");
      vvtbT_pr3mjBruto.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_pr3mjBruto, TheVvDaoTrans, DB_Tci.t_pr3mjBruto, "Pr3Place", _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth // ako ce se zbrajati

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   protected void T_brutoKorekc_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_brutoKorekc = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_brutoKorekc", TheVvDaoTrans, DB_Tci.t_brutoKorekc, "Korekcija bruta");
      vvtbT_brutoKorekc.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_brutoKorekc, TheVvDaoTrans, DB_Tci.t_brutoKorekc, "BrutoKorekc", _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth // ako ce se zbrajati

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   protected void T_dopZdr2020_CreateColumn(int _width, int numOfDecimalPlaces, string header, string opis)
   {
      vvtbT_dopZdr2020 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_dopZdr2020", TheVvDaoTrans, DB_Tci.t_dopZdr2020, opis);
      vvtbT_dopZdr2020.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dopZdr2020, TheVvDaoTrans, DB_Tci.t_dopZdr2020, header, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth // ako ce se zbrajati

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }

   protected void T_stPorez1_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_stPorez1 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_stPorez1", TheVvDaoTrans, DB_Tci.t_stPorez1, "Stopa poreza 1");
    //vvtbT_stPorez1.JAM_ReadOnly = true;
      vvtbT_stPorez1.JAM_ShouldCalcTrans = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_stPorez1, TheVvDaoTrans, DB_Tci.t_stPorez1, "StPor1", _width);

    //vvtbT_opcCD  .JAM_lui_NumberTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_stPrirez);
    //vvtbT_opcName.JAM_lui_NumberTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_stPrirez);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }
   protected void T_stPorez2_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_stPorez2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_stPorez2", TheVvDaoTrans, DB_Tci.t_stPorez2, "Stopa poreza 2");
    //vvtbT_stPorez2.JAM_ReadOnly = true;
      vvtbT_stPorez2.JAM_ShouldCalcTrans = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_stPorez2, TheVvDaoTrans, DB_Tci.t_stPorez2, "StPor2", _width);

    //vvtbT_opcCD  .JAM_lui_NumberTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_stPrirez);
    //vvtbT_opcName.JAM_lui_NumberTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_stPrirez);

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }
   protected void T_fixMio1Olak_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_fixMio1Olak = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_fixMio1Olak", TheVvDaoTrans, DB_Tci.t_fixMio1Olak, "Fiksna olaksica ya MIO 1 - kod vise poslodavaca");
      vvtbT_fixMio1Olak.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_fixMio1Olak, TheVvDaoTrans, DB_Tci.t_fixMio1Olak, "FixMio1Olk", _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth // ako ce se zbrajati

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }


   #endregion T_Columns

   #region R_Columns

   /* 01R */
   protected void R_bruto100_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_bruto100 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_bruto100", null, -12, "Bruto 100%");
      vvtbT_bruto100.JAM_ReadOnly = true;
      vvtbT_bruto100.Tag = Color.Red;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_bruto100, null, "R_bruto100", "Bruto100%", _width);

      vvtbT_bruto100.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 02R */
   protected void R_theBruto_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_theBruto = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_theBruto", null, -12, "Bruto osnovica za obracun doprinosa");
      vvtbT_theBruto.JAM_ReadOnly = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_theBruto, null, "R_theBruto", "TheBruto", _width);
      colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;

      vvtbT_theBruto.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 03R */
   protected void R_mioOsn_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_mioOsn = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_mioOsn", null, -12, "Osnovica za doprinos za MIO");
      vvtbT_mioOsn.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_mioOsn, null, "R_mioOsn", "MIO Osn", _width);

      vvtbT_mioOsn.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 04R */
   protected void R_mio1stup_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_mio1stup = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_mio1stup", null, -12, "Iznos doprinosa za MIO I");
      vvtbT_mio1stup.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_mio1stup, null, "R_mio1stup", "MIO I iz", _width);

      vvtbT_mio1stup.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 05R */
   protected void R_mio2stup_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_mio2stup = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_mio2stup", null, -12, "Iznos doprinosa za MIO II");
      vvtbT_mio2stup.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_mio2stup, null, "R_mio2stup", "MIO II iz", _width);

      vvtbT_mio2stup.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 06R */
   protected void R_mioAll_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_mioAll = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_mioAll", null, -12, "Ukupan iznos doprinosa za MIO");
      vvtbT_mioAll.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_mioAll, null, "R_mioAll", "Uk MIO iz", _width);

      vvtbT_mioAll.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 07R */
   protected void R_doprIz_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_doprIz = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_doprIz", null, -12, "Ukupni iznos doprinosa iz place");
      vvtbT_doprIz.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_doprIz, null, "R_doprIz", "UkDoprIz", _width);

      vvtbT_doprIz.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 08R */
   protected void R_odbitak_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_odbitak = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_odbitak", null, -12, "Ukupni iznos odbitka");
      vvtbT_odbitak.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_odbitak, null, "R_odbitak", "UkOdbitak", _width);

      vvtbT_odbitak.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 09R */
   protected void R_premije_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_premije = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_premije", null, -12, "Ukupni iznos premija");
      vvtbT_premije.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_premije, null, "R_premije", "UkPremije", _width);

      vvtbT_premije.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 10R */
   protected void R_dohodak_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_dohodak = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_dohodak", null, -12, "Dohodak");
      vvtbT_dohodak.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dohodak, null, "R_dohodak", "Dohodak", _width);

      vvtbT_dohodak.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 11R */
   protected void R_porOsnAll_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_porOsnAll = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_porOsnAll", null, -12, "Ukupna porezna osnovica");
      vvtbT_porOsnAll.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_porOsnAll, null, "R_porOsnAll", "PorOsnov", _width);

      vvtbT_porOsnAll.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 12R */
   protected void R_porOsn1_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_porOsn1 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_porOsn1", null, -12, "Porezna osnovica za porez od 15%");
      vvtbT_porOsn1.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_porOsn1, null, "R_porOsn1", "PorOsnov1", _width);

      vvtbT_porOsn1.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 13R */
   protected void R_porOsn2_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_porOsn2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_porOsn2", null, -12, "Porezna osnovica za porez od 25%");
      vvtbT_porOsn2.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_porOsn2, null, "R_porOsn2", "PorOsnov2", _width);

      vvtbT_porOsn2.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 14R */
   protected void R_porOsn3_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_porOsn3 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_porOsn3", null, -12, "Porezna osnovica za porez od 35%");
      vvtbT_porOsn3.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_porOsn3, null, "R_porOsn3", "PorOsnov3", _width);

      vvtbT_porOsn3.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 15R */
   protected void R_porOsn4_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_porOsn4 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_porOsn4", null, -12, "Porezna osnovica za porez od 45%");
      vvtbT_porOsn4.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_porOsn4, null, "R_porOsn4", "PorOsnov4", _width);

      vvtbT_porOsn4.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 16R */
   protected void R_por1uk_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_por1uk = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_por1uk", null, -12, "Iznos poreza od 15%");
      vvtbT_por1uk.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_por1uk, null, "R_por1uk", "Porez 1", _width);

      vvtbT_por1uk.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 17R */
   protected void R_por2uk_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_por2uk = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_por2uk", null, -12, "Iznos poreza od 25%");
      vvtbT_por2uk.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_por2uk, null, "R_por2uk", "Porez 2", _width);

      vvtbT_por2uk.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 18R */
   protected void R_por3uk_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_por3uk = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_por3uk", null, -12, "Iznos poreza od 35%");
      vvtbT_por3uk.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_por3uk, null, "R_por3uk", "Porez 3", _width);

      vvtbT_por3uk.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 19R */
   protected void R_por4uk_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_por4uk = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_por4uk", null, -12, "Iznos poreza od 45%");
      vvtbT_por4uk.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_por4uk, null, "R_por4uk", "Porez 4", _width);

      vvtbT_por4uk.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 20R */
   protected void R_porezAll_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_porezAll = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_porezAll", null, -12, "Ukupan iznos poreza na dohodak");
      vvtbT_porezAll.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_porezAll, null, "R_porezAll", "UkPorez", _width);

      vvtbT_porezAll.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 21R */
   protected void R_prirez_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_prirez = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_prirez", null, -12, "Iznos prireza na porez na dohodak");
      vvtbT_prirez.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_prirez, null, "R_prirez", "Prirez", _width);

      vvtbT_prirez.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 22R */
   protected void R_porPrirez_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_porPrirez = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_porPrirez", null, -12, "Ukupan iznos poreza i prireza ");
      vvtbT_porPrirez.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_porPrirez, null, "R_porPrirez", "Porez i Prir", _width);

      vvtbT_porPrirez.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 23R */
   protected void R_netto_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_netto = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_netto", null, -12, "Neto");
      vvtbT_netto.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_netto, null, "R_netto", "Neto", _width);

      vvtbT_netto.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;
      colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   protected void R_nettoBlue_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_nettoBlue = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_nettoBlue", null, -12, "Neto");
      vvtbT_nettoBlue.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_nettoBlue, null, "R_nettoBlue", "Neto", _width);

      vvtbT_nettoBlue.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;
      colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, false, true));
   }


   /* 24R */
   protected void R_obustave_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_obustave = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_obustave", null, -12, "Ukupne obustave iz neta");
      vvtbT_obustave.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_obustave, null, "R_obustave", "Obustave", _width);

      vvtbT_obustave.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 25R */
   protected void R_2Pay_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_2Pay = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_2Pay", null, -12, "Ukupan izdatak za placu");
      vvtbT_2Pay.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_2Pay, null, "R_2Pay", "BRUTO 2", _width);
      colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;

      vvtbT_2Pay.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 26R */
   protected void R_naRuke_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_naRuke = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_naRuke", null, -12, "Neto za isplatu");
      vvtbT_naRuke.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_naRuke, null, "R_naRuke", "Na ruke", _width);

      vvtbT_naRuke.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 27R */
   protected void R_zdrNa_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_zdrNa = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_zdrNa", null, -12, "Iznos doprinosa za zdravstveno osiguranja na placu");
      vvtbT_zdrNa.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_zdrNa, null, "R_zdrNa", "DopZdrOsig", _width);

      vvtbT_zdrNa.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 28R */
   protected void R_zorNa_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_zorNa = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_zorNa", null, -12, "Iznos doprinosa za ZOR na placu");
      vvtbT_zorNa.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_zorNa, null, "R_zorNa", "Dop ZOR", _width);

      vvtbT_zorNa.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 29R */
   protected void R_zapNa_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_zapNa = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_zapNa", null, -12, "Iznos doprinosa za zaposljavanje na placu");
      vvtbT_zapNa.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_zapNa, null, "R_zapNa", "DopZapos", _width);

      vvtbT_zapNa.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 30R */
   protected void R_zapII_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_zapII = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_zapII", null, -12, "Iznos posebnog doprinosa za zaposljavanje na placu");
      vvtbT_zapII.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_zapII, null, "R_zapII", "DopZaposII", _width);

      vvtbT_zapII.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 31R */
   protected void R_zapAll_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_zapAll = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_zapAll", null, -12, "Ukupan iznos doprinosa za zaposljavanje na placu");
      vvtbT_zapAll.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_zapAll, null, "R_zapAll", "UkDopZapos", _width);

      vvtbT_zapAll.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 32R */
   protected void R_doprNa_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_doprNa = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_doprNa", null, -12, "Ukupan iznos doprinosa na placu");
      vvtbT_doprNa.JAM_ReadOnly = true;
      vvtbT_doprNa.JAM_ShouldSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_doprNa, null, "R_doprNa", "UkDopNaPl", _width);
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 33R */
   protected void R_doprAll_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_doprAll = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_doprAll", null, -12, "Ukupan iznos doprinosa iz i na placu");
      vvtbT_doprAll.JAM_ReadOnly = true;
      vvtbT_doprAll.JAM_ShouldSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_doprAll, null, "R_doprAll", "UkDoprinosi", _width);
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 34R */
   protected void R_satiRada_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_satiR = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_satiR", null, -12, "Sati rada - sati na teret Poslodavca");
      vvtbT_satiR.JAM_ReadOnly = true;
      vvtbT_satiR.JAM_ShouldSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_satiR, null, "R_satiR", "SatiP", _width);
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, true));

   }
   /* 35R */
   protected void R_satiBolovanja_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_satiB = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_satiB", null, -12, "Sati na teret Zakonodavca / HZZO, HZMO, MOBIMS ...");
      vvtbT_satiB.JAM_ReadOnly = true;
      vvtbT_satiB.JAM_ShouldSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_satiB, null, "R_satiB", "SatiZ", _width);
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, true));

   }
   /* 36R */
   protected void R_satiUkupno_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_satiUk = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_satiUk", null, -12, "Ukupan broj sati");
      vvtbT_satiUk.JAM_ReadOnly = true;
      vvtbT_satiUk.JAM_ShouldSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_satiUk, null, "R_satiUk", "SatiUk", _width);
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, true));

   }
   /* 37R */
   protected void R_numOfLinesPtrane_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_numOfLinesPtrane = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtbT_numOfLinesPtrane", null, -12, "Broj redova u tablici Evidencije rada");
      vvtbT_numOfLinesPtrane.JAM_ReadOnly = true;
      vvtbT_numOfLinesPtrane.JAM_ForeColor = ZXC.vvColors.tabPage4TheG2_BackColor;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_numOfLinesPtrane, null, "R_numOfLinesPtrane", "EVR", _width);
      colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, true));
   }
   /* 38R */
   protected void R_numOfLinesPtrano_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_numOfLinesPtrano = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtbT_numOfLinesPtrano", null, -12, "Broj redova u tablici Obustave");
      vvtbT_numOfLinesPtrano.JAM_ReadOnly = true;
      vvtbT_numOfLinesPtrano.JAM_ForeColor = Color.Orange;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_numOfLinesPtrano, null, "R_numOfLinesPtrano", "OBU", _width);
      colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, true));
   }
   /* 39R */
   protected void R_fondSatiDiff_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_fondSatiDiff = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtbT_fondSatiDiff", null, -12, "Razlika do punog fonda sati");
      vvtbT_fondSatiDiff.JAM_ReadOnly = true;
      vvtbT_fondSatiDiff.JAM_ForeColor = Color.Red;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_fondSatiDiff, null, "R_fondSatiDiff", "Raz", _width);
      colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, true));
   }
   /* 40R */
   protected void R_mio1stupNa_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_mio1stupNa = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtbT_mio1stupNa", null, -12, "Iznos doprinosa za MIO I na placu za beneficirani radni staz");
      vvtbT_mio1stupNa.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_mio1stupNa, null, "R_mio1stupNa", "MIO I Na", _width);

      vvtbT_mio1stupNa.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 41R */
   protected void R_mio2stupNa_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_mio2stupNa = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtbT_mio2stupNa", null, -12, "Iznos doprinosa za MIO II na placu za beneficirani radni staz");
      vvtbT_mio2stupNa.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_mio2stupNa, null, "R_mio2stupNa", "MIO II Na", _width);

      vvtbT_mio2stupNa.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 42R */
   protected void R_mioAllNa_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_mioAllNa = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_mioAllNa", null, -12, "Ukupan iznos doprinosa za MIO na placu za beneficirani radni staz");
      vvtbT_mioAllNa.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_mioAllNa, null, "R_mioAllNa", "Uk MIO Na", _width);

      vvtbT_mioAllNa.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 44R */
   protected void R_krizPorOsn_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_krizPorOsn = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_krizPorOsn", null, -12, "Osnovica za krizni porez");
      vvtbT_krizPorOsn.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_krizPorOsn, null, "R_krizPorOsn", "Osnovica KrizP", _width);

      vvtbT_krizPorOsn.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 43R */
   protected void R_krizPorUk_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_krizPorUk = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_krizPorUk", null, -12, "Krizni porez");
      vvtbT_krizPorUk.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_krizPorUk, null, "R_krizPorUk", "KrizniPor", _width);

      vvtbT_krizPorUk.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 45R */
   protected void R_zpiUk_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_zpiUk = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_zpiUk", null, -12, "Doprinos za ZPI");
      vvtbT_zpiUk.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_zpiUk, null, "R_zpiUk", "DoprZPI", _width);

      vvtbT_zpiUk.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 46R */
   protected void R_daniZpi_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_daniZpi = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_daniZpi", null, -12, "Dani za ZPI");
      vvtbT_daniZpi.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_daniZpi, null, "R_daniZpi", "DaniZPI", _width);

      vvtbT_daniZpi.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
   }
   /* 57R */
   protected void R_satiNeRad_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_satiNeR = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_satiNeR", null, -12, "Ukupni neodrađeni sati rada - sati bolovanja na teret poslodavca, sati plaćenog dopusta, neodrađenih, a plaćenih sati za blagdane, praznike i slično");
      vvtbT_satiNeR.JAM_ReadOnly = true;
      vvtbT_satiNeR.JAM_ShouldSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_satiNeR, null, "R_satiNeR", "SatiNeR", _width);
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, true));

   }
   
 ////2024
 //protected void R_mio1Osn_CreateColumn(int _width, int numOfDecimalPlaces)
 //{
 //   vvtbT_mio1Osn = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_mio1Osn", null, -12, "Osnovica za doprinos za MIO I stup");
 //   vvtbT_mio1Osn.JAM_ReadOnly = true;
 //   colVvText = TheG.CreateVvTextBoxColumn(vvtbT_mio1Osn, null, "R_mio1Osn", "MIO Osn", _width);
 //
 //   vvtbT_mioOsn.JAM_ShouldSumGrid = true;
 //   colVvText.MinimumWidth = _width;
 //
 //   PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, false));
 //}

   protected void R_mio1Olk_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_mio1Olk = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_mio1Olk", null, -12, "Olaksica za MIO I");
    //vvtbT_mio1Olk.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_mio1Olk, null, "R_mio1Olk", "MIO I Olk", _width);

      vvtbT_mio1Olk.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, true));
   }
   protected void R_mio1Osn_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_mio1Osn = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_mio1Osn", null, -12, "Osnovica za MIO I");
    //vvtbT_mio1Osn.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_mio1Osn, null, "R_mio1Osn", "MIO I Osn", _width);

      vvtbT_mio1Osn.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;

      PlacaColChDefaultsList.Add(new VvPref.VVColChooserStates(colVvText.Name, true, true));
   }


   #endregion R_Columns

   #endregion TheGrid_Columns

   #region TheGrid2_Columns

   //private void InitializeTheGrid2_Columns()
   //{
   //   int w;

   //   TheG2.TheSumOfPreferredWidths = 0;
   //   TheG2.TheSumOfPreferredWidths += TheG2.RowHeadersWidth;

   //   w = ZXC.Q3un; T_personCD_2_CreateColumn(w); TheG2.TheSumOfPreferredWidths += w;
   //   w = ZXC.Q8un; T_prezime_2_CreateColumn(w); TheG2.TheSumOfPreferredWidths += w;
   //   w = ZXC.Q6un; T_ime_2_CreateColumn(w); TheG2.TheSumOfPreferredWidths += w;

   //   /* 10 */      w = ZXC.Q2un + ZXC.Qun4;  T_vrstaRCd_CreateColumn(w);   TheG2.TheSumOfPreferredWidths += w;
   //   /* 11 */      w = ZXC.Q10un + ZXC.Q5un; T_vrstaRName_CreateColumn(w); TheG2.TheSumOfPreferredWidths += w;
   //   /* 12 */      w = ZXC.Q3un - ZXC.Qun8;  T_cijPerc_CreateColumn(w, 0); TheG2.TheSumOfPreferredWidths += w;
   //   /* 14 */      w = ZXC.Q3un - ZXC.Qun4;  T_rsOO_CreateColumn(w);       TheG2.TheSumOfPreferredWidths += w;
   //   /* 15 */      w = ZXC.Q3un - ZXC.Qun4;  T_rsOD_CreateColumn(w);       TheG2.TheSumOfPreferredWidths += w;
   //   /* 16 */      w = ZXC.Q3un - ZXC.Qun4;  T_rsDO_CreateColumn(w);       TheG2.TheSumOfPreferredWidths += w;
   //   /* 13 */      w = ZXC.Q3un - ZXC.Qun2;  T_sati_CreateColumn(w, 1);    TheG2.TheSumOfPreferredWidths += w;
   //}

   protected void T_personCD_2_CreateColumn(int _width)
   {
      vvtbT_personCD2 = TheG2.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColT_personCD2", TheVvDaoTrans2, DB_Tci2.t_personCD, "Šifra radnika");
      vvtbT_personCD2.JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterSifra), new EventHandler(AnyPersonTextBoxLeave));
      vvtbT_personCD2.JAM_DataRequired = true;

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_personCD2, TheVvDaoTrans2, DB_Tci2.t_personCD, "Šifra", _width);
   }

   protected void T_ime_2_CreateColumn(int _width)
   {
      int nazivMaxLen = ZXC.PersonDao.GetSchemaColumnSize(ZXC.PerCI.ime);

      vvtbT_ime2 = TheG2.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_ime2", TheVvDaoTrans2, (-1 * nazivMaxLen), "Ime radnika");
      vvtbT_ime2.JAM_ReadOnly = true;

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_ime2, TheVvDaoTrans2, DB_Tci2.t_ime, "Ime", _width);
   }

   protected void T_prezime_2_CreateColumn(int _width)
   {
      vvtbT_prezime2 = TheG2.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_prezime2", TheVvDaoTrans2, DB_Tci2.t_prezime, "Prezime radnika");
      vvtbT_prezime2.JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonTextBoxLeave));

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_prezime2, TheVvDaoTrans2, DB_Tci2.t_prezime, "Prezime", _width);
      //colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      //colVvText.MinimumWidth = ZXC.Q6un;
   }

   /* 10 */
   protected void T_vrstaRCd_CreateColumn(int _width)
   {
      vvtbT_vrstaR_cd = TheG2.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_vrstaRCd", TheVvDaoTrans2, DB_Tci2.t_vrstaR_cd, "Sifra Vrste Rada ");
      vvtbT_vrstaR_cd.JAM_Set_LookUpTable(ZXC.luiListaVrstaRadaEVR, (int)ZXC.Kolona.prva);

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_vrstaR_cd, TheVvDaoTrans2, DB_Tci2.t_vrstaR_cd, "VR", _width);
   }

   /* 11 */
   protected void T_vrstaRName_CreateColumn(int _width)
   {
      vvtbT_vrstaR_name = TheG2.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_vrstaRName", TheVvDaoTrans2, DB_Tci2.t_vrstaR_name, "Naziv Vrste Rada ");
      vvtbT_vrstaR_name.JAM_ReadOnly = true;

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_vrstaR_name, TheVvDaoTrans2, DB_Tci2.t_vrstaR_name, "Vrsta Rada", _width);

      vvtbT_vrstaR_cd.JAM_lui_NameTaker_JAM_Name = TheVvDaoTrans2.GetSchemaColumnName(DB_Tci2.t_vrstaR_name);

   }

   /* 12 */
   protected void T_cijPerc_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_cijPerc = TheG2.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_cijPerc", TheVvDaoTrans2, DB_Tci2.t_cijPerc, "Postotak cijene rada");
      vvtbT_cijPerc.JAM_ReadOnly = true;

      vvtbT_cijPerc.JAM_ShouldCalcTrans = true;

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_cijPerc, TheVvDaoTrans2, DB_Tci2.t_cijPerc, "Postotak", _width);
      colVvText.DefaultCellStyle.Format = VvUserControl.GetDgvCellStyleFormat_Number(0, false, true);
      vvtbT_vrstaR_cd.JAM_lui_NumberTaker_JAM_Name = TheVvDaoTrans2.GetSchemaColumnName(DB_Tci2.t_cijPerc);
   }

   /* 13 */
   protected void T_sati_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_sati = TheG2.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_sati", TheVvDaoTrans2, DB_Tci2.t_sati, "Sati");
      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_sati, TheVvDaoTrans2, DB_Tci2.t_sati, "Sati", _width);
   }

   /* 14 */
   protected void T_rsOO_CreateColumn(int _width)
   {
      vvtbT_rsOO = TheG2.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_rsOO", TheVvDaoTrans2, DB_Tci2.t_rsOO, "Osnova obracuna");
      vvtbT_rsOO.JAM_ReadOnly = true;

      vvtbT_rsOO.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      //vvtbT_rsOO.JAM_Set_LookUpTable(ZXC.luiListaOsnovaOsigRsm, (int)ZXC.Kolona.prva);

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_rsOO, TheVvDaoTrans2, DB_Tci2.t_rsOO, "OO", _width);

      vvtbT_vrstaR_cd.JAM_lui_IntegerTaker_JAM_Name = TheVvDaoTrans2.GetSchemaColumnName(DB_Tci2.t_rsOO);
      colVvText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
   }

   /* 15 */
   protected void T_rsOD_CreateColumn(int _width)
   {
      vvtbT_rsOD = TheG2.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColT_rsOD", TheVvDaoTrans2, DB_Tci2.t_rsOD, "Obrazac, strana B  OD");
      vvtbT_rsOD.JAM_FieldExitWithValidationMethod += new System.ComponentModel.CancelEventHandler(CalcSati_forOD);

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_rsOD, TheVvDaoTrans2, DB_Tci2.t_rsOD, "OD", _width);
      colVvText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
   }

   /* 16 */
   protected void T_rsDO_CreateColumn(int _width)
   {
      vvtbT_rsDO = TheG2.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColT_rsDO", TheVvDaoTrans2, DB_Tci2.t_rsDO, "Obrazac, strana B  DO");
      vvtbT_rsDO.JAM_FieldExitWithValidationMethod += new System.ComponentModel.CancelEventHandler(CalcSati_forDO);
      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_rsDO, TheVvDaoTrans2, DB_Tci2.t_rsDO, "DO", _width);
      colVvText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
   }

   /* 17 */
   protected void T_stjecatCD_CreateColumn(int _width)
   {
      vvtbT_stjecatCD = TheG2.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_stjecatCD", TheVvDaoTrans2, DB_Tci2.t_stjecatCD, "Oznaka stjecatelja primitka/osiguranika ");
      vvtbT_stjecatCD.JAM_Set_LookUpTable(ZXC.luiListaStjecatelj, (int)ZXC.Kolona.prva);
      vvtbT_stjecatCD.JAM_ForeColor = ZXC.vvColors.tabPage4TheG_BackColor;

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_stjecatCD, TheVvDaoTrans2, DB_Tci2.t_stjecatCD, "Stjecatelj", _width);
      colVvText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
   }
   protected void R_stjecatName_CreateColumn(int _width)
   {
      vvtbT_stjecatName = TheG2.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_stjecatName", null, -12, "Opis stjecatelja/osiguranika");
      vvtbT_stjecatName.JAM_ReadOnly = true;

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_stjecatName, null, "R_stjecatName", "Stjecatelj/osiguranik", _width);

      vvtbT_stjecatCD.JAM_lui_NameTaker_JAM_Name = "R_stjecatName";
   }

   /* 18 */
   protected void T_primDohCD_CreateColumn(int _width)
   {
      vvtbT_primDohCD = TheG2.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_primDohCD", TheVvDaoTrans2, DB_Tci2.t_primDohCD, "Oznaka primitka/obveze doprinosa");
      vvtbT_primDohCD.JAM_Set_LookUpTable(ZXC.luiListaPrimDoh, (int)ZXC.Kolona.prva);
      vvtbT_primDohCD.JAM_ForeColor = ZXC.vvColors.tabPage4TheG_BackColor;

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_primDohCD, TheVvDaoTrans2, DB_Tci2.t_primDohCD, "PrimDoh", _width);
      colVvText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
   }
   protected void R_primDohName_CreateColumn(int _width)
   {
      vvtbT_primDohName = TheG2.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_primDohName", null, -12, "Opis primitka/obveze doprinosa");
      vvtbT_primDohName.JAM_ReadOnly = true;

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_primDohName, null, "R_primDohName", "Primitak/obveza doprinosa", _width);

      vvtbT_primDohCD.JAM_lui_NameTaker_JAM_Name = "R_primDohName";
   }

   /* 19 */
   protected void T_pocKrajCD_CreateColumn(int _width)
   {
      vvtbT_pocKrajCD = TheG2.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_pocKrajCD", TheVvDaoTrans2, DB_Tci2.t_pocKrajCD, "Oznaka prvog/zadnjeg mjeseca u osiguranju po istoj osnovi");
      vvtbT_pocKrajCD.JAM_Set_LookUpTable(ZXC.luiListaPocKraj, (int)ZXC.Kolona.prva);
      vvtbT_pocKrajCD.JAM_ForeColor = ZXC.vvColors.tabPage4TheG_BackColor;

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_pocKrajCD, TheVvDaoTrans2, DB_Tci2.t_pocKrajCD, "OznMj", _width);
      colVvText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
   }
   protected void R_pocKrajName_CreateColumn(int _width)
   {
      vvtbT_pocKrajName = TheG2.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_pocKrajName", null, -12, "Opis prvog/zadnjeg mjeseca u osiguranju po istoj osnovi");
      vvtbT_pocKrajName.JAM_ReadOnly = true;

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_pocKrajName, null, "R_pocKrajName", "Prvi/zadnji mjesec po istoj osnovi", _width);

      vvtbT_pocKrajCD.JAM_lui_NameTaker_JAM_Name = "R_pocKrajName";
   }
   /*20*/
   protected void T_ip1gr_CreateColumn(int _width)
   {
      vvtbT_ip1gr = TheG2.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColT_ip1gr", TheVvDaoTrans2, /*DB_Tci2.t_ip1gr*/-2, "Oznaka grupe redka Obrasca IP1");

      colVvText   = TheG2.CreateVvTextBoxColumn(vvtbT_ip1gr, TheVvDaoTrans2, DB_Tci2.t_ip1gr, "IP1", _width);

      vvtbT_vrstaR_cd.JAM_lui_UintegerTaker_JAM_Name = TheVvDaoTrans2.GetSchemaColumnName(DB_Tci2.t_ip1gr);
   }

   protected void T_rbrIsprJop_CreateColumn(int _width)
   {
      vvtbT_rbrIsprJop = TheG2.CreateVvTextBoxFor_Integer_ColumnTemplate(false, "vvtb4ColT_rbrIsprJop", TheVvDaoTrans2, DB_Tci2.t_rbrIsprJop, "Redni broj redka izvornog JOPPD Obrasca za koji je potrebno napraviti 2-Ispravak ili 3-Dopunu");

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT_rbrIsprJop, TheVvDaoTrans2, DB_Tci2.t_rbrIsprJop, "RbrJop", _width);

      vvtbT_rbrIsprJop.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      vvtbT_rbrIsprJop.JAM_ForeColor = Color.BlueViolet;
   }

   #endregion TheGrid2_Columns

   #region TheGrid3_Columns

   //private void InitializeTheGrid3_Columns()
   //{
   //   int w;

   //   TheG3.TheSumOfPreferredWidths = 0;
   //   TheG3.TheSumOfPreferredWidths += TheG3.RowHeadersWidth;

   //   w = ZXC.Q3un; T_personCD_3_CreateColumn(w); TheG3.TheSumOfPreferredWidths += w;
   //   w = ZXC.Q8un; T_prezime_3_CreateColumn(w); TheG3.TheSumOfPreferredWidths += w;
   //   w = ZXC.Q6un; T_ime_3_CreateColumn(w); TheG3.TheSumOfPreferredWidths += w;

   //   /* 10 */       w = ZXC.Q5un; T_dateStart_CreateColumn(w) ; TheG3.TheSumOfPreferredWidths += w;
   //   /* 11 */       w = ZXC.Q2un; T_ukBrRata_CreateColumn(w)  ; TheG3.TheSumOfPreferredWidths += w;
   //   /* 12 */       w = ZXC.Q8un; T_opisOb_CreateColumn(w)    ; TheG3.TheSumOfPreferredWidths += w;
   //   /* 16 */       w = ZXC.Q2un; T_isZbirObus_CreateColumn(w); TheG3.TheSumOfPreferredWidths += w;
   //   /* 17 */       w = ZXC.Q7un; T_partija_CreateColumn(w)   ; TheG3.TheSumOfPreferredWidths += w;
   //   /* 13 */       w = ZXC.Q4un; T_kupdob_cd_CreateColumn(w) ; TheG3.TheSumOfPreferredWidths += w;
   //   /* 14 */       w = ZXC.Q4un; T_kupdob_tk_CreateColumn(w) ; TheG3.TheSumOfPreferredWidths += w;
   //   /* 15 */       w = ZXC.Q4un; T_iznosOb_CreateColumn(w, 2); TheG3.TheSumOfPreferredWidths += w;

   //}


   protected void T_personCD_3_CreateColumn(int _width)
   {
      vvtbT_personCD3 = TheG3.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColT_personCD3", TheVvDaoTrans3, DB_Tci3.t_personCD, "Šifra radnika");
      vvtbT_personCD3.JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterSifra), new EventHandler(AnyPersonTextBoxLeave));
      vvtbT_personCD3.JAM_DataRequired = true;

      colVvText = TheG3.CreateVvTextBoxColumn(vvtbT_personCD3, TheVvDaoTrans3, DB_Tci3.t_personCD, "Šifra", _width);
   }

   protected void T_ime_3_CreateColumn(int _width)
   {
      int nazivMaxLen = ZXC.PersonDao.GetSchemaColumnSize(ZXC.PerCI.ime);

      vvtbT_ime3 = TheG3.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_ime3", TheVvDaoTrans3, (-1 * nazivMaxLen), "Ime radnika");
      vvtbT_ime3.JAM_ReadOnly = true;

      colVvText = TheG3.CreateVvTextBoxColumn(vvtbT_ime3, TheVvDaoTrans3, DB_Tci3.t_ime, "Ime", _width);
   }

   protected void T_prezime_3_CreateColumn(int _width)
   {
      vvtbT_prezime3 = TheG3.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_prezime3", TheVvDaoTrans3, DB_Tci3.t_prezime, "Prezime radnika");
      vvtbT_prezime3.JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonTextBoxLeave));

      colVvText = TheG3.CreateVvTextBoxColumn(vvtbT_prezime3, TheVvDaoTrans3, DB_Tci3.t_prezime, "Prezime", _width);
      //colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      //colVvText.MinimumWidth = ZXC.Q6un;
   }


   /* 10 */
   protected void T_dateStart_CreateColumn(int _width)
   {
      colDate = TheG3.CreateCalendarColumn(TheVvDaoTrans3, DB_Tci3.t_dateStart, "Datum početka", _width);
   }

   /* 11 */
   protected void T_ukBrRata_CreateColumn(int _width)
   {
      vvtbT_ukBrRata = TheG3.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColT_ukBrRata", TheVvDaoTrans3, DB_Tci3.t_ukBrRata, "Ukupni broj rata");
      vvtbT_ukBrRata.JAM_FieldExitMethod = new EventHandler(OnExit_ukBrRata_SetRbrRate1);

      colVvText = TheG3.CreateVvTextBoxColumn(vvtbT_ukBrRata, TheVvDaoTrans3, DB_Tci3.t_ukBrRata, "UkBrRata", _width);
   }

   private void OnExit_ukBrRata_SetRbrRate1(object sender, EventArgs e)
   {
      VvTextBoxEditingControl vvTbx = sender as VvTextBoxEditingControl;

      int rIdx = vvTbx.EditingControlDataGridView.CurrentRow.Index;

      uint ukBrRata = TheG3.GetUint32Cell(ci3.iT_ukBrRata, rIdx, false);
      uint rbrRate  = TheG3.GetUint32Cell(ci3.iT_rbrRate, rIdx, false);

      if(ukBrRata.NotZero() && rbrRate.IsZero()) TheG3.PutCell(ci3.iT_rbrRate, rIdx, 1); ;
   }

   /* 12 */
   protected void T_opisOb_CreateColumn(int _width)
   {
      vvtbT_opisOb = TheG3.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_opisOb", TheVvDaoTrans3, DB_Tci3.t_opisOb, "Opis obustave");
      colVvText = TheG3.CreateVvTextBoxColumn(vvtbT_opisOb, TheVvDaoTrans3, DB_Tci3.t_opisOb, "Opis Obustave", _width);
   }

   /* 13 */
   protected void T_kupdob_cd_CreateColumn(int _width)
   {
      vvtbT_kupdob_cd = TheG3.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColT_kupdob_cd", TheVvDaoTrans3, DB_Tci3.t_kupdob_cd, "Sifra iz adresara");
      vvtbT_kupdob_cd.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupdobTextBoxLeave));

      colVvText = TheG3.CreateVvTextBoxColumn(vvtbT_kupdob_cd, TheVvDaoTrans3, DB_Tci3.t_kupdob_cd, "Sif K/D", _width);
   }

   /* 14 */
   protected void T_kupdob_tk_CreateColumn(int _width)
   {
      vvtbT_kupdob_tk = TheG3.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_kupdob_tk", TheVvDaoTrans3, DB_Tci3.t_kupdob_tk, "Ticker iz adresara");
      vvtbT_kupdob_tk.JAM_CharacterCasing = CharacterCasing.Upper;
      vvtbT_kupdob_tk.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));

      colVvText = TheG3.CreateVvTextBoxColumn(vvtbT_kupdob_tk, TheVvDaoTrans3, DB_Tci3.t_kupdob_tk, "Ticker", _width);
   }

   /* 15 */
   protected void T_iznosOb_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_iznosOb = TheG3.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_iznosOb", TheVvDaoTrans3, DB_Tci3.t_iznosOb, "Iznos Obustave");
      vvtbT_iznosOb.JAM_ShouldCalcTrans = true;

      colVvText = TheG3.CreateVvTextBoxColumn(vvtbT_iznosOb, TheVvDaoTrans3, DB_Tci3.t_iznosOb, "IznosOb", _width);

      vvtbT_iznosOb.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = _width;
   }

   /*16*/
   protected void T_isZbirObus_CreateColumn(int _width)
   {
      vvcbx_isZbirOb = new VvCheckBox();
      colCbox = TheG3.CreateVvCheckBoxColumn(vvcbx_isZbirOb, TheVvDaoTrans3, DB_Tci3.t_isZbir, "IsZbir", _width);
   }

   protected void AnyKupdobTextBoxLeave(object sender, EventArgs e)
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
            TheG3.PutCell(ci3.iT_kupdob_tk, currRow, kupdob_rec.Ticker);
            TheG3.PutCell(ci3.iT_kupdob_cd, currRow, kupdob_rec.KupdobCD/*RecID*/);
         }
         else
         {
            TheG3.PutCell(ci3.iT_kupdob_tk, currRow, "");
            TheG3.PutCell(ci3.iT_kupdob_cd, currRow, "");
         }

         // samo za DUC-eve
         ZXC.TheVvForm.SetDirtyFlag(sender);
      }
   }

   protected void T_partija_CreateColumn(int _width)
   {
      vvtbT_partija = TheG3.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_partija", TheVvDaoTrans3, DB_Tci3.t_partija, "Broj obustave");
      colVvText     = TheG3.CreateVvTextBoxColumn(vvtbT_partija, TheVvDaoTrans3, DB_Tci3.t_partija, "Broj Obustave", _width);
   }

   protected void T_izNetoaSt_CreateColumn(int _width, int numOfDecimalPlaces)
   {
      vvtbT_izNetoaSt = TheG3.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_izNetoaSt", TheVvDaoTrans3, DB_Tci3.t_izNetoaSt, "Stopa obustave iz neta");
      vvtbT_izNetoaSt.JAM_FieldExitMethod = new EventHandler(OnExit_IzNetoaSt);
      vvtbT_izNetoaSt.JAM_IsForPercent = true;

      colVvText = TheG3.CreateVvTextBoxColumn(vvtbT_izNetoaSt, TheVvDaoTrans3, DB_Tci3.t_izNetoaSt, "StIzNeta", _width);

      colVvText.MinimumWidth = _width;
   }

   //private void OnExit_IzNetoaSt(object sender, EventArgs e)
   public void OnExit_IzNetoaSt(object sender, EventArgs e)
   {
      VvTextBoxEditingControl vvTbx = sender as VvTextBoxEditingControl;

      int rIdx = vvTbx.EditingControlDataGridView.CurrentRow.Index;

      decimal izNetoaSt = TheG3.GetDecimalCell(ci3.iT_izNetoaSt, rIdx, false);

      if(izNetoaSt.IsZero()) return;

      uint personCD = TheG3.GetUint32Cell(ci3.iT_personCD, rIdx, false);

    //decimal neto      = TheG .GetDecimalCell(ci .iT_netto, ?????, false);
      Ptrans ptrans_rec = placa_rec.Transes.SingleOrDefault(ptr => ptr.T_personCD == personCD);

      if(ptrans_rec == null) return;

      // 09.11.2016: 
    //decimal iznosOb = ZXC.VvGet_25_on_100(ptrans_rec.R_Netto   , izNetoaSt);
      decimal iznosOb = ZXC.VvGet_25_of_100(ptrans_rec.R_ObustOsn, izNetoaSt);

      TheG3.PutCell(ci3.iT_iznosOb, rIdx, iznosOb);
   }

  /* 19 */
   protected void T_rbrRate_CreateColumn(int _width)
   {
      vvtbT_rbrRate = TheG3.CreateVvTextBoxFor_Integer_ColumnTemplate(false, "vvtb4ColT_rbrRate", TheVvDaoTrans3, DB_Tci3.t_rbrRate, "Redni broj rate");
      colVvText = TheG3.CreateVvTextBoxColumn(vvtbT_rbrRate, TheVvDaoTrans3, DB_Tci3.t_rbrRate, "RBrRate", _width);
   }

   /*20*/
   //protected void T_isZastRn_CreateColumn(int _width) // ptranoKind
   //{
   //   vvcbx_isZastRn = new VvCheckBox();
   //   colCbox = TheG3.CreateVvCheckBoxColumn(vvcbx_isZastRn, TheVvDaoTrans3, DB_Tci3.t_isZastRn, "IsZastRn", _width);
   //}


  /*20*/
   protected void T_ptranoKind_CreateColumn(int _width)
   {
      vvtbT_ptranoKind = TheG3.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_ptranoKind", TheVvDaoTrans3, DB_Tci3.t_isZastRn, " Z - ZAŠTIĆENI RAČUN, N - NEZAŠTIĆENI - klasičan tekući račun, prazno - obustave");
      vvtbT_ptranoKind.JAM_AllowedInputCharacters = "NZ";
      vvtbT_ptranoKind.JAM_CharacterCasing = CharacterCasing.Upper;

      colVvText = TheG3.CreateVvTextBoxColumn(vvtbT_ptranoKind, TheVvDaoTrans3, DB_Tci3.t_isZastRn, "Z/N rn", _width);
      colVvText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
   }



   #endregion TheGrid3_Columns

   #region SetPtransColumnIndexes()

   /// <summary>
   /// Column Index na DataGridView-u (NE U DataBase to ti je 'DB_ci')
   /// </summary>
   private Ptrans_colIdx ci;
   public Ptrans_colIdx DgvCI { get { return ci; } }

   public struct Ptrans_colIdx
   {
      internal int iT_recID;
      internal int iT_serial;
      internal int iT_personCD;
      internal int iT_prezime;
      internal int iT_ime;

      internal int iT_prezimeIme;

      internal int iT_brutoOsn;
      internal int iT_topObrok;
      internal int iT_godStaza;
      internal int iT_dodBruto;
      internal int iT_isMioII;
      internal int iT_spc;
      internal int iT_koef;
      internal int iT_zivotno;
      internal int iT_dopZdr;
      internal int iT_dobMIO;
      internal int iT_koefHRVI;
      internal int iT_invalidTip;
      internal int iT_opcCD;
      internal int iT_opcName;
      internal int iT_opcRadCD;
      internal int iT_opcRadName;
      internal int iT_stPrirez;
      internal int iT_netoAdd;
      internal int iT_isDirNeto;
      internal int iT_prijevoz;
      internal int iT_isPoluSat;
      internal int iT_rsB;
      internal int iT_nacIsplCD;
      internal int iT_neoPrimCD;
      internal int iT_dokumCD;
      internal int iT_brutoDodSt;
      internal int iT_brDodPoloz ;
      internal int iT_koefBruto1;
      internal int iT_dnFondSati;
      internal int iT_thisStazSt;
      internal int iT_brutoDodSt2;
      internal int iT_brutoDodSt3;
      internal int iT_pr3mjBruto ;
      internal int iT_brutoKorekc;
      internal int iT_dopZdr2020;
      internal int iT_stPorez1   ;
      internal int iT_stPorez2   ;
      internal int iT_fixMio1Olak;

      /* 01R */      internal int iT_bruto100;
      /* 02R */      internal int iT_theBruto;
      /* 03R */      internal int iT_mioOsn;
      /* 04R */      internal int iT_mio1stup;
      /* 05R */      internal int iT_mio2stup;
      /* 06R */      internal int iT_mioAll;
      /* 07R */      internal int iT_doprIz;
      /* 08R */      internal int iT_odbitak;
      /* 09R */      internal int iT_premije;
      /* 10R */      internal int iT_dohodak;
      /* 11R */      internal int iT_porOsnAll;
      /* 12R */      internal int iT_porOsn1;
      /* 13R */      internal int iT_porOsn2;
      /* 14R */      internal int iT_porOsn3;
      /* 15R */      internal int iT_porOsn4;
      /* 16R */      internal int iT_por1uk;
      /* 17R */      internal int iT_por2uk;
      /* 18R */      internal int iT_por3uk;
      /* 19R */      internal int iT_por4uk;
      /* 20R */      internal int iT_porezAll;
      /* 21R */      internal int iT_prirez;
      /* 22R */      internal int iT_porPrirez;
      /* 23R */      internal int iT_netto;
      /* 24R */      internal int iT_obustave;
      /* 25R */      internal int iT_2Pay;
      /* 26R */      internal int iT_naRuke;
      /* 27R */      internal int iT_zdrNa;
      /* 28R */      internal int iT_zorNa;
      /* 29R */      internal int iT_zapNa;
      /* 30R */      internal int iT_zapII;
      /* 31R */      internal int iT_zapAll;
      /* 32R */      internal int iT_doprNa;
      /* 33R */      internal int iT_doprAll;
      /* 34R */      internal int iT_satiR;
      /* 35R */      internal int iT_satiB;
      /* 36R */      internal int iT_satiUk;
      /* 37R */      internal int iT_numOfLinesPtrane;
      /* 38R */      internal int iT_numOfLinesPtrano;
      /* 39R */      internal int iT_fondSatiDiff;
      /* 40R */      internal int iT_mio1stupNa;
      /* 41R */      internal int iT_mio2stupNa;
      /* 42R */      internal int iT_mioAllNa;
      /* 43R */      internal int iT_krizPorUk;
      /* 44R */      internal int iT_krizPorOsn;
      /* 45R */      internal int iT_zpiUk;
      /* 46R */      internal int iT_daniZpi;
      /* 23R */      internal int iT_nettoBlue;
                     internal int iT_nacIsplName;
                     internal int iT_neoPrimName;
      /* 57R  */     internal int iT_satiNeR;
      /*      */     internal int iT_mio1Olk;
      /*      */     internal int iT_mio1Osn;

   }

   private void SetPtransColumnIndexes()
   {
      ci = new Ptrans_colIdx();

      ci.iT_recID    = TheG.IdxForColumn("T_recID");
      ci.iT_serial   = TheG.IdxForColumn("T_serial");
      ci.iT_personCD = TheG.IdxForColumn("T_personCD");
      ci.iT_prezime  = TheG.IdxForColumn("T_prezime");
      ci.iT_ime      = TheG.IdxForColumn("T_ime");

      ci.iT_prezimeIme  = TheG.IdxForColumn("R_prezimeIme");
                        
      ci.iT_brutoOsn    = TheG.IdxForColumn("T_brutoOsn");
      ci.iT_topObrok    = TheG.IdxForColumn("T_topObrok");
      ci.iT_godStaza    = TheG.IdxForColumn("T_godStaza");
      ci.iT_dodBruto    = TheG.IdxForColumn("T_dodBruto");
      ci.iT_isMioII     = TheG.IdxForColumn("T_isMioII");
      ci.iT_spc         = TheG.IdxForColumn("T_spc");
      ci.iT_koef        = TheG.IdxForColumn("T_koef");
      ci.iT_zivotno     = TheG.IdxForColumn("T_zivotno");
      ci.iT_dopZdr      = TheG.IdxForColumn("T_dopZdr");
      ci.iT_dobMIO      = TheG.IdxForColumn("T_dobMIO");
      ci.iT_koefHRVI    = TheG.IdxForColumn("T_koefHRVI");
      ci.iT_invalidTip  = TheG.IdxForColumn("T_invalidTip");
      ci.iT_opcCD       = TheG.IdxForColumn("T_opcCD");
      ci.iT_opcName     = TheG.IdxForColumn("T_opcName");
      ci.iT_opcRadCD    = TheG.IdxForColumn("T_opcRadCD");
      ci.iT_opcRadName  = TheG.IdxForColumn("T_opcRadName");
      ci.iT_stPrirez    = TheG.IdxForColumn("T_stPrirez");
      ci.iT_netoAdd     = TheG.IdxForColumn("T_netoAdd");
      ci.iT_isDirNeto   = TheG.IdxForColumn("T_isDirNeto");
      ci.iT_prijevoz    = TheG.IdxForColumn("T_prijevoz");
      ci.iT_isPoluSat   = TheG.IdxForColumn("T_IsPoluSat");
      ci.iT_rsB         = TheG.IdxForColumn("T_rsB");
      ci.iT_nacIsplCD   = TheG.IdxForColumn("T_nacIsplCD");
      ci.iT_neoPrimCD   = TheG.IdxForColumn("T_neoPrimCD");
      ci.iT_dokumCD     = TheG.IdxForColumn("T_dokumCD");
      ci.iT_brutoDodSt  = TheG.IdxForColumn("T_brutoDodSt");
      ci.iT_brDodPoloz  = TheG.IdxForColumn("T_brDodPoloz");
      ci.iT_dopZdr2020  = TheG.IdxForColumn("T_dopZdr2020");
      ci.iT_stPorez1    = TheG.IdxForColumn("T_stPorez1");
      ci.iT_stPorez2    = TheG.IdxForColumn("T_stPorez2");
      ci.iT_fixMio1Olak = TheG.IdxForColumn("T_fixMio1Olak");

      /* 01R */      ci.iT_bruto100         = TheG.IdxForColumn("R_bruto100");
      /* 02R */      ci.iT_theBruto         = TheG.IdxForColumn("R_theBruto");
      /* 03R */      ci.iT_mioOsn           = TheG.IdxForColumn("R_mioOsn");
      /* 04R */      ci.iT_mio1stup         = TheG.IdxForColumn("R_mio1stup");
      /* 05R */      ci.iT_mio2stup         = TheG.IdxForColumn("R_mio2stup");
      /* 06R */      ci.iT_mioAll           = TheG.IdxForColumn("R_mioAll");
      /* 07R */      ci.iT_doprIz           = TheG.IdxForColumn("R_doprIz");
      /* 08R */      ci.iT_odbitak          = TheG.IdxForColumn("R_odbitak");
      /* 09R */      ci.iT_premije          = TheG.IdxForColumn("R_premije");
      /* 10R */      ci.iT_dohodak          = TheG.IdxForColumn("R_dohodak");
      /* 11R */      ci.iT_porOsnAll        = TheG.IdxForColumn("R_porOsnAll");
      /* 12R */      ci.iT_porOsn1          = TheG.IdxForColumn("R_porOsn1");
      /* 13R */      ci.iT_porOsn2          = TheG.IdxForColumn("R_porOsn2");
      /* 14R */      ci.iT_porOsn3          = TheG.IdxForColumn("R_porOsn3");
      /* 15R */      ci.iT_porOsn4          = TheG.IdxForColumn("R_porOsn4");
      /* 16R */      ci.iT_por1uk           = TheG.IdxForColumn("R_por1uk");
      /* 17R */      ci.iT_por2uk           = TheG.IdxForColumn("R_por2uk");
      /* 18R */      ci.iT_por3uk           = TheG.IdxForColumn("R_por3uk");
      /* 19R */      ci.iT_por4uk           = TheG.IdxForColumn("R_por4uk");
      /* 20R */      ci.iT_porezAll         = TheG.IdxForColumn("R_porezAll");
      /* 21R */      ci.iT_prirez           = TheG.IdxForColumn("R_prirez");
      /* 22R */      ci.iT_porPrirez        = TheG.IdxForColumn("R_porPrirez");
      /* 23R */      ci.iT_netto            = TheG.IdxForColumn("R_netto");
      /* 24R */      ci.iT_obustave         = TheG.IdxForColumn("R_obustave");
      /* 25R */      ci.iT_2Pay             = TheG.IdxForColumn("R_2Pay");
      /* 26R */      ci.iT_naRuke           = TheG.IdxForColumn("R_naRuke");
      /* 27R */      ci.iT_zdrNa            = TheG.IdxForColumn("R_zdrNa");
      /* 28R */      ci.iT_zorNa            = TheG.IdxForColumn("R_zorNa");
      /* 29R */      ci.iT_zapNa            = TheG.IdxForColumn("R_zapNa");
      /* 30R */      ci.iT_zapII            = TheG.IdxForColumn("R_zapII");
      /* 31R */      ci.iT_zapAll           = TheG.IdxForColumn("R_zapAll");
      /* 32R */      ci.iT_doprNa           = TheG.IdxForColumn("R_doprNa");
      /* 33R */      ci.iT_doprAll          = TheG.IdxForColumn("R_doprAll");
      /* 34R */      ci.iT_satiR            = TheG.IdxForColumn("R_satiR");
      /* 35R */      ci.iT_satiB            = TheG.IdxForColumn("R_satiB");
      /* 36R */      ci.iT_satiUk           = TheG.IdxForColumn("R_satiUk");
      /* 37R */      ci.iT_numOfLinesPtrane = TheG.IdxForColumn("R_numOfLinesPtrane");
      /* 38R */      ci.iT_numOfLinesPtrano = TheG.IdxForColumn("R_numOfLinesPtrano");
      /* 39R */      ci.iT_fondSatiDiff     = TheG.IdxForColumn("R_fondSatiDiff");
      /* 40R */      ci.iT_mio1stupNa       = TheG.IdxForColumn("R_mio1stupNa");
      /* 41R */      ci.iT_mio2stupNa       = TheG.IdxForColumn("R_mio2stupNa");
      /* 42R */      ci.iT_mioAllNa         = TheG.IdxForColumn("R_mioAllNa");
      /* 43R */      ci.iT_krizPorUk        = TheG.IdxForColumn("R_krizPorUk");
      /* 44R */      ci.iT_krizPorOsn       = TheG.IdxForColumn("R_krizPorOsn");
      /* 45R */      ci.iT_zpiUk            = TheG.IdxForColumn("R_zpiUk");
      /* 45R */      ci.iT_daniZpi          = TheG.IdxForColumn("R_daniZpi");
      /* 23R */      ci.iT_nettoBlue        = TheG.IdxForColumn("R_nettoBlue");
                     ci.iT_neoPrimName      = TheG.IdxForColumn("R_neoPrimName");
                     ci.iT_nacIsplName      = TheG.IdxForColumn("R_nacIsplName");

                     ci.iT_koefBruto1      = TheG.IdxForColumn("T_koefBruto1") ;
                     ci.iT_dnFondSati      = TheG.IdxForColumn("T_dnFondSati") ;
                     ci.iT_thisStazSt      = TheG.IdxForColumn("T_thisStazSt") ;
                     ci.iT_brutoDodSt2     = TheG.IdxForColumn("T_brutoDodSt2");
                     ci.iT_brutoDodSt3     = TheG.IdxForColumn("T_brutoDodSt3");
                     ci.iT_pr3mjBruto      = TheG.IdxForColumn("T_pr3mjBruto" );
                     ci.iT_brutoKorekc     = TheG.IdxForColumn("T_brutoKorekc");

      /* 57R */      ci.iT_satiNeR         = TheG.IdxForColumn("R_satiNeR");

                     ci.iT_mio1Olk         = TheG.IdxForColumn("R_mio1Olk");
                     ci.iT_mio1Osn         = TheG.IdxForColumn("R_mio1Osn");
   }

   //=== PtranE, Trans2, VirtualTranses2, TheG2, ... ============================================= 
   /// <summary>
   /// Column Index na DataGridView-u (NE U DataBase to ti je 'DB_ci')
   /// </summary>
   private Ptrane_colIdx ci2;
   public Ptrane_colIdx DgvCI2 { get { return ci2; } }

   public struct Ptrane_colIdx
   {
      internal int iT_recID;
      internal int iT_serial;
      internal int iT_personCD;
      internal int iT_prezime;
      internal int iT_ime;

      internal int iT_vrstaR_cd;
      internal int iT_vrstaR_name;
      internal int iT_cijPerc;
      internal int iT_sati;
      internal int iT_rsOO;
      internal int iT_rsOD;
      internal int iT_rsDO;
      internal int iT_stjecatCD;
      internal int iT_primDohCD;
      internal int iT_pocKrajCD;
      internal int iT_ip1gr;
      internal int iT_rbrIsprJop;
   
      internal int iR_stjecatName;
      internal int iR_primDohName;
      internal int iR_pocKrajName;
   }

   private void SetPtraneColumnIndexes()
   {
      ci2 = new Ptrane_colIdx();

      ci2.iT_recID       = TheG2.IdxForColumn("T_recID");
      ci2.iT_serial      = TheG2.IdxForColumn("T_serial");
      ci2.iT_personCD    = TheG2.IdxForColumn("T_personCD");
      ci2.iT_prezime     = TheG2.IdxForColumn("T_prezime");
      ci2.iT_ime         = TheG2.IdxForColumn("T_ime");

      ci2.iT_vrstaR_cd   = TheG2.IdxForColumn("T_vrstaR_cd");
      ci2.iT_vrstaR_name = TheG2.IdxForColumn("T_vrstaR_name");
      ci2.iT_cijPerc     = TheG2.IdxForColumn("T_cijPerc");
      ci2.iT_sati        = TheG2.IdxForColumn("T_sati");
      ci2.iT_rsOO        = TheG2.IdxForColumn("T_rsOO");
      ci2.iT_rsOD        = TheG2.IdxForColumn("T_rsOD");
      ci2.iT_rsDO        = TheG2.IdxForColumn("T_rsDO");
      ci2.iT_stjecatCD   = TheG2.IdxForColumn("T_stjecatCD");
      ci2.iT_primDohCD   = TheG2.IdxForColumn("T_primDohCD");
      ci2.iT_pocKrajCD   = TheG2.IdxForColumn("T_pocKrajCD");
      ci2.iT_ip1gr       = TheG2.IdxForColumn("T_ip1gr");
      ci2.iT_rbrIsprJop  = TheG2.IdxForColumn("T_rbrIsprJop");

      ci2.iR_stjecatName = TheG2.IdxForColumn("R_stjecatName");
      ci2.iR_primDohName = TheG2.IdxForColumn("R_primDohName");
      ci2.iR_pocKrajName = TheG2.IdxForColumn("R_pocKrajName");

   }

   //=== PtranO, Trans3, VirtualTranses3, TheG3, ... ============================================= 
   /// <summary>
   /// Column Index na DataGridView-u (NE U DataBase to ti je 'DB_ci')
   /// </summary>
   private Ptrano_colIdx ci3;
   public Ptrano_colIdx DgvCI3 { get { return ci3; } }

   public struct Ptrano_colIdx
   {
      internal int iT_recID;
      internal int iT_serial;
      internal int iT_personCD;
      internal int iT_prezime;
      internal int iT_ime;

      internal int iT_dateStart;
      internal int iT_ukBrRata;
      internal int iT_opisOb;
      internal int iT_kupdob_cd;
      internal int iT_kupdob_tk;
      internal int iT_iznosOb;
      internal int iT_isZbir;
      internal int iT_partija;
      internal int iT_izNetoaSt;
      internal int iT_rbrRate;
      internal int iT_isZastRn; //ptranoKind

   }

   private void SetPtranoColumnIndexes()
   {
      ci3 = new Ptrano_colIdx();

      ci3.iT_recID    = TheG3.IdxForColumn("T_recID");
      ci3.iT_serial   = TheG3.IdxForColumn("T_serial");
      ci3.iT_personCD = TheG3.IdxForColumn("T_personCD");
      ci3.iT_prezime  = TheG3.IdxForColumn("T_prezime");
      ci3.iT_ime      = TheG3.IdxForColumn("T_ime");

      ci3.iT_dateStart = TheG3.IdxForColumn("T_dateStart");
      ci3.iT_ukBrRata  = TheG3.IdxForColumn("T_ukBrRata");
      ci3.iT_opisOb    = TheG3.IdxForColumn("T_opisOb");
      ci3.iT_kupdob_cd = TheG3.IdxForColumn("T_kupdob_cd");
      ci3.iT_kupdob_tk = TheG3.IdxForColumn("T_kupdob_tk");
      ci3.iT_iznosOb   = TheG3.IdxForColumn("T_iznosOb");
      ci3.iT_isZbir    = TheG3.IdxForColumn("T_isZbir");
      ci3.iT_partija   = TheG3.IdxForColumn("T_partija");
      ci3.iT_izNetoaSt = TheG3.IdxForColumn("T_izNetoaSt");
      ci3.iT_rbrRate   = TheG3.IdxForColumn("T_rbrRate");
      ci3.iT_isZastRn  = TheG3.IdxForColumn("T_isZastRn"); // ptranoKind
   }

   #endregion SetPtransColumnIndexes()

   #region Fld_

   public uint Fld_DokNum
   {
      get { return ZXC.ValOrZero_UInt(tbx_dokNum.Text); }
      set { tbx_dokNum.Text = value.ToString("000000"); }
   }

   public string Fld_TT
   {
      get { return tbx_tt.Text; }
      set { tbx_tt.Text = value; }
   }

   public string Fld_TipTranOpis
   {
      set { tbx_ttOpis.Text = value; }
   }

   public DateTime Fld_DokDate
   {
      get { return dTP_datum.Value; }
      set
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dTP_datum.Value = value;
         }
      }
   }

   public string Fld_Napomena
   {
      get { return tbx_napomena.Text; }
      set { tbx_napomena.Text = value; }
   }

   public uint Fld_TTNum
   {
      get { return ZXC.ValOrZero_UInt(tbx_ttNum.Text); }
      set { tbx_ttNum.Text = value.ToString("0000"); }
   }

   public uint Fld_MtrosCd
   {
      get { return tbx_mtros_cd.GetSomeRecIDField(); }
      set { tbx_mtros_cd.PutSomeRecIDField(value); }
   }

   public string Fld_MtrosCdAsTxt
   {
      get { return tbx_mtros_cd.Text; }
      set { tbx_mtros_cd.Text = value; }
   }

   public string Fld_MtrosTk
   {
      get { return tbx_mtros_tk.Text; }
      set { tbx_mtros_tk.Text = value; }
   }

   public string Fld_MtrosNaziv
   {
      get { return tbx_mtros_Naziv.Text; }
      set { tbx_mtros_Naziv.Text = value; }
   }

   public string Fld_MMYYYY
   {
      get { return tbx_lookUpMMYYYY.Text; }
      set { tbx_lookUpMMYYYY.Text = value; }
   }

   public decimal Fld_FondSati
   {
      get { return tbx_fondSati.GetDecimalField(); }
      set { tbx_fondSati.PutDecimalField(value); }
   }

   public string Fld_VrstaObr
   {
      get { return tbx_lookUpVrstaObr.Text; }
      set { tbx_lookUpVrstaObr.Text = value; }
   }
   public string Fld_VrstaJOPPD
   {
      get { return tbx_lookUpVrstaJoppd.Text; }
      set { tbx_lookUpVrstaJoppd.Text = value; }
   }

   public string Fld_RSm_ID
   {
      get { return tbx_RSmID.Text; }
      set { tbx_RSmID.Text = value; }
   }

   public bool Fld_IsTrgFondSati
   {
      get { return cbx_isTrgFondSati.Checked; }
      set { cbx_isTrgFondSati.Checked = value; }
   }

   //* rules ************************************** 

   /* 17 */
   public decimal Fld_StPor1
   {
      get { return tbx_stPor1.GetDecimalField(); }
      set { tbx_stPor1.PutDecimalField(value); }
   }
   /* 18 */
   public decimal Fld_StPor2
   {
      get { return tbx_stPor2.GetDecimalField(); }
      set { tbx_stPor2.PutDecimalField(value); }
   }
   /* 19 */
   public decimal Fld_StPor3
   {
      get { return tbx_stPor3.GetDecimalField(); }
      set { tbx_stPor3.PutDecimalField(value); }
   }
   /* 20 */
   public decimal Fld_StPor4
   {
      get { return tbx_stPor4.GetDecimalField(); }
      set { tbx_stPor4.PutDecimalField(value); }
   }
   /* 21 */
   public decimal Fld_OsnOdb
   {
      get { return tbx_osnOdb.GetDecimalField(); }
      set { tbx_osnOdb.PutDecimalField(value); }
   }
   public decimal Fld_OsnOdb_EUR
   {
      get { return tbx_osnOdb.GetDecimalField(); }
      set { tbx_osnOdb.PutDecimalField(value); }
   }
   /* 22 */
   public decimal Fld_StMioIz
   {
      get { return tbx_stMioIz.GetDecimalField(); }
      set { tbx_stMioIz.PutDecimalField(value); }
   }
   /* 23 */
   public decimal Fld_StMioIz2
   {
      get { return tbx_stMioIz_II.GetDecimalField(); }
      set { tbx_stMioIz_II.PutDecimalField(value); }
   }
   /* 24 */
   public decimal Fld_StZdrNa
   {
      get { return tbx_stZdrNa.GetDecimalField(); }
      set { tbx_stZdrNa.PutDecimalField(value); }
   }
   /* 25 */
   public decimal Fld_StZorNa
   {
      get { return tbx_stZorNa.GetDecimalField(); }
      set { tbx_stZorNa.PutDecimalField(value); }
   }
   /* 26 */
   public decimal Fld_StZapNa
   {
      get { return tbx_stZapNa.GetDecimalField(); }
      set { tbx_stZapNa.PutDecimalField(value); }
   }
   /* 27 */
   public decimal Fld_StZapII
   {
      get { return tbx_stZapII.GetDecimalField(); }
      set { tbx_stZapII.PutDecimalField(value); }
   }
   /* 28 */
   public decimal Fld_MinMioOsn
   {
      get { return tbx_minMioOsn.GetDecimalField(); }
      set { tbx_minMioOsn.PutDecimalField(value); }
   }
   public decimal Fld_MinMioOsn_EUR
   {
      get { return tbx_minMioOsn.GetDecimalField(); }
      set { tbx_minMioOsn.PutDecimalField(value); }
   }
   /* 29 */
   public decimal Fld_MaxMioOsn
   {
      get { return tbx_maxMioOsn.GetDecimalField(); }
      set { tbx_maxMioOsn.PutDecimalField(value); }
   }
   public decimal Fld_MaxMioOsn_EUR
   {
      get { return tbx_maxMioOsn.GetDecimalField(); }
      set { tbx_maxMioOsn.PutDecimalField(value); }
   }
   /* 30 */
   public decimal Fld_MaxPorOsn1
   {
      get { return tbx_maxPorOsn1.GetDecimalField(); }
      set { tbx_maxPorOsn1.PutDecimalField(value); }
   }
   public decimal Fld_MaxPorOsn1_EUR
   {
      get { return tbx_maxPorOsn1.GetDecimalField(); }
      set { tbx_maxPorOsn1.PutDecimalField(value); }
   }
   /* 31 */
   public decimal Fld_MaxPorOsn2
   {
      get { return tbx_maxPorOsn2.GetDecimalField(); }
      set { tbx_maxPorOsn2.PutDecimalField(value); }
   }
   public decimal Fld_MaxPorOsn2_EUR
   {
      get { return tbx_maxPorOsn2.GetDecimalField(); }
      set { tbx_maxPorOsn2.PutDecimalField(value); }
   }
   /* 32 */
   public decimal Fld_MaxPorOsn3
   {
      get { return tbx_maxPorOsn3.GetDecimalField(); }
      set { tbx_maxPorOsn3.PutDecimalField(value); }
   }
   public decimal Fld_MaxPorOsn3_EUR
   {
      get { return tbx_maxPorOsn3.GetDecimalField(); }
      set { tbx_maxPorOsn3.PutDecimalField(value); }
   }
   /* 33 */
   public decimal Fld_StZpi
   {
      get { return tbx_stZpi.GetDecimalField(); }
      set { tbx_stZpi.PutDecimalField(value); }
   }
   /* 34 */
   public decimal Fld_StOthOlak
   {
      get { return tbx_stOthOlak.GetDecimalField(); }
      set { tbx_stOthOlak.PutDecimalField(value); }
   }
   /* 35 */
   public decimal Fld_StDodStaz
   {
      get { return tbx_stDodStaz.GetDecimalField(); }
      set { tbx_stDodStaz.PutDecimalField(value); }
   }
   /* 36 */
   public uint Fld_GranBrRad
   {
      get { return ZXC.ValOrZero_UInt(tbx_granBrRad.Text); }
      set { tbx_granBrRad.Text = value.ToString(); }
   }

   /* 37 */
   public decimal Fld_StMioNaB1
   {
      get { return tbx_stMioNaB1.GetDecimalField(); }
      set { tbx_stMioNaB1.PutDecimalField(value); }
   }
   /* 38 */
   public decimal Fld_StMioNa2B1
   {
      get { return tbx_stMioNa2B1.GetDecimalField(); }
      set { tbx_stMioNa2B1.PutDecimalField(value); }
   }

   /* 39 */
   public decimal Fld_StMioNaB2
   {
      get { return tbx_stMioNaB2.GetDecimalField(); }
      set { tbx_stMioNaB2.PutDecimalField(value); }
   }
   /* 40 */
   public decimal Fld_StMioNa2B2
   {
      get { return tbx_stMioNa2B2.GetDecimalField(); }
      set { tbx_stMioNa2B2.PutDecimalField(value); }
   }
   /* 41 */
   public decimal Fld_StMioNaB3
   {
      get { return tbx_stMioNaB3.GetDecimalField(); }
      set { tbx_stMioNaB3.PutDecimalField(value); }
   }
   /* 42 */
   public decimal Fld_StMioNa2B3
   {
      get { return tbx_stMioNa2B3.GetDecimalField(); }
      set { tbx_stMioNa2B3.PutDecimalField(value); }
   }
   /* 43 */
   public decimal Fld_StMioNaB4
   {
      get { return tbx_stMioNaB4.GetDecimalField(); }
      set { tbx_stMioNaB4.PutDecimalField(value); }
   }
   /* 44 */
   public decimal Fld_StMioNa2B4
   {
      get { return tbx_stMioNa2B4.GetDecimalField(); }
      set { tbx_stMioNa2B4.PutDecimalField(value); }
   }

   /* 45 */
   public decimal Fld_ProsPlaca
   {
      get { return tbx_prosPlaca.GetDecimalField(); }
      set { tbx_prosPlaca.PutDecimalField(value); }
   }
   public decimal Fld_ProsPlaca_EUR
   {
      get { return tbx_prosPlaca.GetDecimalField(); }
      set { tbx_prosPlaca.PutDecimalField(value); }
   }
   /* 46 */
   public decimal Fld_OsnDopClUp
   {
      get { return tbx_stMioNa2B5.GetDecimalField(); }
      set { tbx_stMioNa2B5.PutDecimalField(value); }
   }
   public decimal Fld_OsnDopClUp_EUR
   {
      get { return tbx_stMioNa2B5.GetDecimalField(); }
      set { tbx_stMioNa2B5.PutDecimalField(value); }
   }

   /* 47 */
   public decimal Fld_StKrizPor1
   {
      get { return tbx_stKrizPor1.GetDecimalField(); }
      set { tbx_stKrizPor1.PutDecimalField(value); }
   }
   /* 48 */
   public decimal Fld_StKrizPor2
   {
      get { return tbx_stKrizPor2.GetDecimalField(); }
      set { tbx_stKrizPor2.PutDecimalField(value); }
   }

   /* 50 */
   public decimal Fld_VrKoefBr1
   {
      get { return tbx_VrKoefBr1.GetDecimalField(); }
      set { tbx_VrKoefBr1.PutDecimalField(value); }
   }

   /* 51 */
   public decimal Fld_StZdrDD
   {
      get { return tbx_stZdrDD.GetDecimalField(); }
      set { tbx_stZdrDD.PutDecimalField(value); }
   }
   public bool Fld_IsLocked
   {
      get { return cbx_isLocked.Checked; }
      set { cbx_isLocked.Checked = value; }
   }

   public decimal Fld_Mio1Granica1  { get { return tbx_mio1Granica1.GetDecimalField(); } set { tbx_mio1Granica1.PutDecimalField(value); }  }
   public decimal Fld_Mio1Granica2  { get { return tbx_mio1Granica2.GetDecimalField(); } set { tbx_mio1Granica2.PutDecimalField(value); }  }
   public decimal Fld_Mio1FiksOlk   { get { return tbx_mio1FiksOlk .GetDecimalField(); } set { tbx_mio1FiksOlk .PutDecimalField(value); }  }
   public decimal Fld_Mio1KoefOlk   { get { return tbx_mio1KoefOlk .GetDecimalField(); } set { tbx_mio1KoefOlk .PutDecimalField(value); } }


   #endregion Fld_

   #region PutFields(), GetFields()

   public override void PutFields(VvDataRecord placa)
   {
      placa_rec = (Placa)placa;

      Kupdob kupdobSifrar_rec;

      if(placa_rec != null)
      {

         PutMetaFileds(placa_rec.AddUID, placa_rec.AddTS, placa_rec.ModUID, placa_rec.ModTS, placa_rec.RecID, placa_rec.LanSrvID, placa_rec.LanRecID);

         PutIdentityFields(placa_rec.DokNum.ToString("000000"), placa_rec.DokDate.ToString(ZXC.VvDateFormat), placa_rec.TT, "");

         Fld_DokNum         = placa_rec.DokNum;
         Fld_DokDate        = placa_rec.DokDate;
         Fld_Napomena       = placa_rec.Napomena;
         Fld_TT             = placa_rec.TT;
         Fld_TTNum          = placa_rec.TtNum;
         Fld_TipTranOpis    = ZXC.luiListaPlacaTT.GetNameForThisCd(placa_rec.TT);
         Fld_MtrosCd        = placa_rec.MtrosCd;
         Fld_MtrosTk        = placa_rec.MtrosTk;
         Fld_MMYYYY         = placa_rec.MMYYYY;
         Fld_FondSati       = placa_rec.FondSati;
         Fld_VrstaObr       = placa_rec.VrstaObr;
         Fld_VrstaJOPPD     = placa_rec.VrstaJOPPD;
         Fld_RSm_ID         = placa_rec.RSm_ID;
         Fld_IsTrgFondSati  = placa_rec.IsTrgFondSati;
         Fld_IsLocked       = placa_rec.IsLocked;

         VvLookUpLista fondSatiLista = (Fld_IsTrgFondSati == true ? ZXC.luiListaFondSati_TRG : ZXC.luiListaFondSati_NOR);

         tbx_lookUpMMYYYY.JAM_Set_LookUpTable(fondSatiLista, (int)ZXC.Kolona.prva);

         tbx_napomena.TextAsToolTip(toolTip);

         if(ZXC.projectYearAsInt <= 2022) // old kune era 
         {
            Fld_OsnOdb     = placa_rec.Rule_OsnOdb    ;
            Fld_MinMioOsn  = placa_rec.Rule_MinMioOsn ;
            Fld_MaxMioOsn  = placa_rec.Rule_MaxMioOsn ;
            Fld_MaxPorOsn1 = placa_rec.Rule_MaxPorOsn1;
            Fld_MaxPorOsn2 = placa_rec.Rule_MaxPorOsn2;
            Fld_MaxPorOsn3 = placa_rec.Rule_MaxPorOsn3;
            Fld_ProsPlaca  = placa_rec.Rule_ProsPlaca ;
            Fld_OsnDopClUp = placa_rec.Rule_OsnDopClUp;
         }
         else // ZXC.projectYearAsInt >= 2023 ... new euro era 
         {
            Fld_OsnOdb_EUR     = placa_rec.Rule_OsnOdb    ;
            Fld_MinMioOsn_EUR  = placa_rec.Rule_MinMioOsn ;
            Fld_MaxMioOsn_EUR  = placa_rec.Rule_MaxMioOsn ;
            Fld_MaxPorOsn1_EUR = placa_rec.Rule_MaxPorOsn1;
            Fld_MaxPorOsn2_EUR = placa_rec.Rule_MaxPorOsn2;
            Fld_MaxPorOsn3_EUR = placa_rec.Rule_MaxPorOsn3;
            Fld_ProsPlaca_EUR  = placa_rec.Rule_ProsPlaca ;
            Fld_OsnDopClUp_EUR = placa_rec.Rule_OsnDopClUp;
         }

         Fld_StPor1 = placa_rec.Rule_StPor1;
         Fld_StPor2     = placa_rec.Rule_StPor2;
         Fld_StPor3     = placa_rec.Rule_StPor3;
         Fld_StPor4     = placa_rec.Rule_StPor4;
         Fld_StMioIz    = placa_rec.Rule_StMio1stup;
         Fld_StMioIz2   = placa_rec.Rule_StMio2stup;
         Fld_StZdrNa    = placa_rec.Rule_StZdrNa;
         Fld_StZorNa    = placa_rec.Rule_StZorNa;
         Fld_StZapNa    = placa_rec.Rule_StZapNa;
         Fld_StZapII    = placa_rec.Rule_StZapII;
         Fld_StZpi      = placa_rec.Rule_StZpi;
         Fld_StOthOlak  = placa_rec.Rule_StOthOlak;
         Fld_StDodStaz  = placa_rec.Rule_StDodStaz;
         Fld_GranBrRad  = placa_rec.Rule_GranBrRad;

         Fld_StMioNaB1  = placa_rec.Rule_StMioNaB1;
         Fld_StMioNa2B1 = placa_rec.Rule_StMioNa2B1;
         Fld_StMioNaB2  = placa_rec.Rule_StMioNaB2;
         Fld_StMioNa2B2 = placa_rec.Rule_StMioNa2B2;
         Fld_StMioNaB3  = placa_rec.Rule_StMioNaB3;
         Fld_StMioNa2B3 = placa_rec.Rule_StMioNa2B3;
         Fld_StMioNaB4  = placa_rec.Rule_StMioNaB4;
         Fld_StMioNa2B4 = placa_rec.Rule_StMioNa2B4;

         Fld_StKrizPor1 = placa_rec.Rule_StKrizPor1;
         Fld_StKrizPor2 = placa_rec.Rule_StKrizPor2;
         Fld_VrKoefBr1  = placa_rec.Rule_VrKoefBr1 ;

         Fld_StZdrDD    = placa_rec.Rule_StZdrDD   ;

         Fld_Mio1Granica1 = placa_rec.Rule_Mio1Granica1;
         Fld_Mio1Granica2 = placa_rec.Rule_Mio1Granica2;
         Fld_Mio1FiksOlk  = placa_rec.Rule_Mio1FiksOlk ;
         Fld_Mio1KoefOlk  = placa_rec.Rule_Mio1KoefOlk ;

         SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.None);

         kupdobSifrar_rec = KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD/*VirtualRecID*/ == placa_rec.MtrosCd);
         if(kupdobSifrar_rec != null) Fld_MtrosNaziv = kupdobSifrar_rec.Naziv;
         else Fld_MtrosNaziv = "";

         PutDgvFields();

         InitializeFilterUCFields();

         recordReportLoaded = false;
         DecideIfShouldLoad_VvReport(null, null, null);

      }

      TheG.ClearSelection();
      TheSumGrid.ClearSelection();

      TheG2.ClearSelection();
      TheSumGrid2.ClearSelection();

      TheG3.ClearSelection();
      TheSumGrid3.ClearSelection();
   }

   // Tu dolazimo na 1 nacin: 1. Classic PutFields 
   private void InitializeFilterUCFields()
   {
      SetFilterRecordDependentDefaults(); // filter.DokNumOd = pr.DokNum (punimo bussiness od filtera, ne UC)
      ThePlacaDokumentFilterUC.PutFilterFields(ThePlacaDokumentFilter);

   }

   public override void GetFields(bool dirtyFlagging)
   {
      placa_rec.DokNum        = Fld_DokNum;
      placa_rec.DokDate       = Fld_DokDate;
      placa_rec.Napomena      = Fld_Napomena;
      placa_rec.TT            = Fld_TT;
      placa_rec.TtNum         = Fld_TTNum;
      placa_rec.MtrosCd       = Fld_MtrosCd;
      placa_rec.MtrosTk       = Fld_MtrosTk;
      placa_rec.MMYYYY        = Fld_MMYYYY;
      placa_rec.FondSati      = Fld_FondSati;
      placa_rec.VrstaObr      = Fld_VrstaObr;
      placa_rec.VrstaJOPPD    = Fld_VrstaJOPPD;
      placa_rec.RSm_ID        = Fld_RSm_ID;
      placa_rec.IsTrgFondSati = Fld_IsTrgFondSati;
      placa_rec.IsLocked      = Fld_IsLocked;

      if(ZXC.projectYearAsInt <= 2022) // old kune era 
      {
         placa_rec.Rule_OsnOdb     = Fld_OsnOdb    ;
         placa_rec.Rule_MinMioOsn  = Fld_MinMioOsn ;
         placa_rec.Rule_MaxMioOsn  = Fld_MaxMioOsn ;
         placa_rec.Rule_MaxPorOsn1 = Fld_MaxPorOsn1;
         placa_rec.Rule_MaxPorOsn2 = Fld_MaxPorOsn2;
         placa_rec.Rule_MaxPorOsn3 = Fld_MaxPorOsn3;
         placa_rec.Rule_ProsPlaca  = Fld_ProsPlaca ;
         placa_rec.Rule_OsnDopClUp = Fld_OsnDopClUp;
      }
      else // ZXC.projectYearAsInt >= 2023 ... new euro era 
      {
         placa_rec.Rule_OsnOdb     = Fld_OsnOdb_EUR    ;
         placa_rec.Rule_MinMioOsn  = Fld_MinMioOsn_EUR ;
         placa_rec.Rule_MaxMioOsn  = Fld_MaxMioOsn_EUR ;
         placa_rec.Rule_MaxPorOsn1 = Fld_MaxPorOsn1_EUR;
         placa_rec.Rule_MaxPorOsn2 = Fld_MaxPorOsn2_EUR;
         placa_rec.Rule_MaxPorOsn3 = Fld_MaxPorOsn3_EUR;
         placa_rec.Rule_ProsPlaca  = Fld_ProsPlaca_EUR ;
         placa_rec.Rule_OsnDopClUp = Fld_OsnDopClUp_EUR;
      }

      placa_rec.Rule_StPor1     = Fld_StPor1;
      placa_rec.Rule_StPor2     = Fld_StPor2;
      placa_rec.Rule_StPor3     = Fld_StPor3;
      placa_rec.Rule_StPor4     = Fld_StPor4;
      placa_rec.Rule_StMio1stup = Fld_StMioIz;
      placa_rec.Rule_StMio2stup = Fld_StMioIz2;
      placa_rec.Rule_StZdrNa    = Fld_StZdrNa;
      placa_rec.Rule_StZorNa    = Fld_StZorNa;
      placa_rec.Rule_StZapNa    = Fld_StZapNa;
      placa_rec.Rule_StZapII    = Fld_StZapII;
      placa_rec.Rule_StZpi      = Fld_StZpi;
      placa_rec.Rule_StOthOlak  = Fld_StOthOlak;
      placa_rec.Rule_StDodStaz  = Fld_StDodStaz;
      placa_rec.Rule_GranBrRad  = Fld_GranBrRad;

      placa_rec.Rule_StMioNaB1  = Fld_StMioNaB1;
      placa_rec.Rule_StMioNa2B1 = Fld_StMioNa2B1;
      placa_rec.Rule_StMioNaB2  = Fld_StMioNaB2;
      placa_rec.Rule_StMioNa2B2 = Fld_StMioNa2B2;
      placa_rec.Rule_StMioNaB3  = Fld_StMioNaB3;
      placa_rec.Rule_StMioNa2B3 = Fld_StMioNa2B3;
      placa_rec.Rule_StMioNaB4  = Fld_StMioNaB4;
      placa_rec.Rule_StMioNa2B4 = Fld_StMioNa2B4;

      placa_rec.Rule_StKrizPor1 = Fld_StKrizPor1;
      placa_rec.Rule_StKrizPor2 = Fld_StKrizPor2;
      placa_rec.Rule_VrKoefBr1  = Fld_VrKoefBr1 ;

      placa_rec.Rule_StZdrDD    = Fld_StZdrDD   ;

      placa_rec.Rule_Mio1Granica1 = Fld_Mio1Granica1;
      placa_rec.Rule_Mio1Granica2 = Fld_Mio1Granica2;
      placa_rec.Rule_Mio1FiksOlk  = Fld_Mio1FiksOlk ;
      placa_rec.Rule_Mio1KoefOlk  = Fld_Mio1KoefOlk ;

      GetDgvFields(dirtyFlagging);
   }

   #region Put_NewDocum_NumAndDateFields

   public override void Put_NewDocum_NumAndDateFields(uint dokNum, DateTime dokDate)
   {
      Fld_DokNum = dokNum;
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
      PutDgvFields1();
      PutDgvFields2();
      PutDgvFields3();
   }

   private void PutDgvFields1()
   {
      int rowIdx, idxCorrector;

      TheG.RowsRemoved -= new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);
      TheG.RowsAdded -= new DataGridViewRowsAddedEventHandler(grid_RowsAdded);
      TheG.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(TheG);

      if(placa_rec.Transes != null)
      {
         foreach(Ptrans ptrans_rec in placa_rec.Transes)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheG.Rows.Add();

            rowIdx = TheG.RowCount - idxCorrector;

            PutDgvLineFields1(ptrans_rec, rowIdx, false);

            ptrans_rec.CalcTransResults(placa_rec);

            PutDgvLineResultsFields1(rowIdx, ptrans_rec, false);
         }
      }
      TheG.RowsAdded += new DataGridViewRowsAddedEventHandler(grid_RowsAdded);
      TheG.RowsRemoved += new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);

      RenumerateLineNumbers(TheG, 0);

      UpdateLineCount(TheG);

      PutDgvTransSumFields1();
   }

   public override void PutDgvLineFields1(VvTransRecord trans_rec, int rowIdx, bool skipRecID_andSerial_Columns)
   {
      Ptrans ptrans_rec = (Ptrans)trans_rec;

      if(skipRecID_andSerial_Columns == false)
      {
         TheG.PutCell(ci.iT_recID, rowIdx, ptrans_rec.T_recID);
         TheG.PutCell(ci.iT_serial, rowIdx, ptrans_rec.T_serial);
      }

      TheG.PutCell(ci.iT_personCD, rowIdx, ptrans_rec.T_personCD);

      TheG.PutCell(ci.iT_prezimeIme, rowIdx, ptrans_rec.T_prezimeIme);
      TheG.PutCell(ci.iT_prezime   , rowIdx, ptrans_rec.T_prezime);
      TheG.PutCell(ci.iT_ime       , rowIdx, ptrans_rec.T_ime);

      TheG.PutCell(ci.iT_brutoOsn   , rowIdx, ptrans_rec.T_brutoOsn);
      TheG.PutCell(ci.iT_topObrok   , rowIdx, ptrans_rec.T_topObrok);
      TheG.PutCell(ci.iT_godStaza   , rowIdx, ptrans_rec.T_godStaza);
      TheG.PutCell(ci.iT_dodBruto   , rowIdx, ptrans_rec.T_dodBruto); // !!!!! ptrans_rec.T_dodBruto se dolje u 'PutDgvLineResultsFields1' pregazi kao da je result, ako je T_brutoDodSt neprazan !!!!!!
      TheG.PutCell(ci.iT_isMioII    , rowIdx, VvCheckBox.GetString4Bool(ptrans_rec.T_isMioII));
      TheG.PutCell(ci.iT_spc        , rowIdx, GetOneLetter4Spc(ptrans_rec.T_spc));
      TheG.PutCell(ci.iT_koef       , rowIdx, ptrans_rec.T_koef);
      TheG.PutCell(ci.iT_zivotno    , rowIdx, ptrans_rec.T_zivotno);
      TheG.PutCell(ci.iT_dopZdr     , rowIdx, ptrans_rec.T_dopZdr);
      TheG.PutCell(ci.iT_dobMIO     , rowIdx, ptrans_rec.T_dobMIO);
      TheG.PutCell(ci.iT_koefHRVI   , rowIdx, ptrans_rec.T_koefHRVI);
      TheG.PutCell(ci.iT_invalidTip , rowIdx, GetOneLetter4Invalid(ptrans_rec.T_invalidTip));
      TheG.PutCell(ci.iT_opcCD      , rowIdx, ptrans_rec.T_opcCD);
      TheG.PutCell(ci.iT_opcName    , rowIdx, ptrans_rec.T_opcName);
      TheG.PutCell(ci.iT_opcRadCD   , rowIdx, ptrans_rec.T_opcRadCD);
      TheG.PutCell(ci.iT_opcRadName , rowIdx, ptrans_rec.T_opcRadName);
      TheG.PutCell(ci.iT_stPrirez   , rowIdx, ptrans_rec.T_stPrirez);
      TheG.PutCell(ci.iT_netoAdd    , rowIdx, ptrans_rec.T_NetoAdd);
      TheG.PutCell(ci.iT_isDirNeto  , rowIdx, VvCheckBox.GetString4Bool(ptrans_rec.T_isDirNeto));
      TheG.PutCell(ci.iT_prijevoz   , rowIdx, ptrans_rec.T_prijevoz);
      TheG.PutCell(ci.iT_isPoluSat  , rowIdx, VvCheckBox.GetString4Bool(ptrans_rec.T_IsPoluSat));
      TheG.PutCell(ci.iT_rsB        , rowIdx, ptrans_rec.T_rsB);
      TheG.PutCell(ci.iT_nacIsplCD  , rowIdx, ptrans_rec.T_nacIsplCD);
      TheG.PutCell(ci.iT_neoPrimCD  , rowIdx, ptrans_rec.T_neoPrimCD);
      TheG.PutCell(ci.iT_dokumCD    , rowIdx, ptrans_rec.T_dokumCD);
      TheG.PutCell(ci.iT_brutoDodSt , rowIdx, ptrans_rec.T_brutoDodSt);
      TheG.PutCell(ci.iT_brDodPoloz , rowIdx, ptrans_rec.T_brDodPoloz);
      TheG.PutCell(ci.iT_koefBruto1 , rowIdx, ptrans_rec.T_koefBruto1);
      TheG.PutCell(ci.iT_dnFondSati , rowIdx, ptrans_rec.T_dnFondSati);
      TheG.PutCell(ci.iT_thisStazSt , rowIdx, ptrans_rec.T_thisStazSt);
      TheG.PutCell(ci.iT_brutoDodSt2, rowIdx, ptrans_rec.T_brutoDodSt2);
      TheG.PutCell(ci.iT_brutoDodSt3, rowIdx, ptrans_rec.T_brutoDodSt3);
      TheG.PutCell(ci.iT_pr3mjBruto , rowIdx, ptrans_rec.T_pr3mjBruto );
      TheG.PutCell(ci.iT_brutoKorekc, rowIdx, ptrans_rec.T_brutoKorekc);
      TheG.PutCell(ci.iT_dopZdr2020 , rowIdx, ptrans_rec.T_dopZdr2020);
      TheG.PutCell(ci.iT_stPorez1   , rowIdx, ptrans_rec.T_stPorez1);
      TheG.PutCell(ci.iT_stPorez2   , rowIdx, ptrans_rec.T_stPorez2);
      TheG.PutCell(ci.iT_fixMio1Olak, rowIdx, ptrans_rec.T_fixMio1Olak);

   }

   public override void PutDgvLineResultsFields1(int rowIdx, VvTransRecord trans_rec, bool passPtrResultsToZaglavljeTranses)
   {
      Ptrans ptrans_rec = trans_rec as Ptrans;

      if(passPtrResultsToZaglavljeTranses == true)
      {
         // NotaBene: bez ove provjere za Deleted bi ti se kada pobrises npr drugi (kao zadnji) row pa ga ponovno dodas i izazoves CalcTranses() skoci 
         // Exception da {"Sequence contains more than one matching element"}
         // u PutDgvLineResultsFields1():       if(passPtrResultsToZaglavljeTranses == true)... jerbo uprkos deletanju DGV row-a u biznisu deletani trans ostane! 
         placa_rec.Transes.SingleOrDefault(ptr => ptr.T_serial == ptrans_rec.T_serial && ptr.SaveTransesWriteMode != ZXC.WriteMode.Delete).PtrResults = ptrans_rec.PtrResults;
      }

      /* 01R */      TheG.PutCell(ci.iT_bruto100        , rowIdx, ptrans_rec.R_Bruto100);
      /* 02R */      TheG.PutCell(ci.iT_theBruto        , rowIdx, ptrans_rec.R_TheBruto);
      /* 03R */      TheG.PutCell(ci.iT_mioOsn          , rowIdx, ptrans_rec.R_MioOsn);
      /* 04R */      TheG.PutCell(ci.iT_mio1stup        , rowIdx, ptrans_rec.R_Mio1stup);
      /* 05R */      TheG.PutCell(ci.iT_mio2stup        , rowIdx, ptrans_rec.R_Mio2stup);
      /* 06R */      TheG.PutCell(ci.iT_mioAll          , rowIdx, ptrans_rec.R_MioAll);
      /* 07R */      TheG.PutCell(ci.iT_doprIz          , rowIdx, ptrans_rec.R_DoprIz);
      /* 08R */      TheG.PutCell(ci.iT_odbitak         , rowIdx, ptrans_rec.R_Odbitak);
      /* 09R */      TheG.PutCell(ci.iT_premije         , rowIdx, ptrans_rec.R_Premije);
      /* 10R */      TheG.PutCell(ci.iT_dohodak         , rowIdx, ptrans_rec.R_Dohodak);
      /* 11R */      TheG.PutCell(ci.iT_porOsnAll       , rowIdx, ptrans_rec.R_PorOsnAll);
      /* 12R */      TheG.PutCell(ci.iT_porOsn1         , rowIdx, ptrans_rec.R_PorOsn1);
      /* 13R */      TheG.PutCell(ci.iT_porOsn2         , rowIdx, ptrans_rec.R_PorOsn2);
      /* 14R */      TheG.PutCell(ci.iT_porOsn3         , rowIdx, ptrans_rec.R_PorOsn3);
      /* 15R */      TheG.PutCell(ci.iT_porOsn4         , rowIdx, ptrans_rec.R_PorOsn4);
      /* 16R */      TheG.PutCell(ci.iT_por1uk          , rowIdx, ptrans_rec.R_Por1Uk);
      /* 17R */      TheG.PutCell(ci.iT_por2uk          , rowIdx, ptrans_rec.R_Por2Uk);
      /* 18R */      TheG.PutCell(ci.iT_por3uk          , rowIdx, ptrans_rec.R_Por3Uk);
      /* 19R */      TheG.PutCell(ci.iT_por4uk          , rowIdx, ptrans_rec.R_Por4Uk);
      /* 20R */      TheG.PutCell(ci.iT_porezAll        , rowIdx, ptrans_rec.R_PorezAll);
      /* 21R */      TheG.PutCell(ci.iT_prirez          , rowIdx, ptrans_rec.R_Prirez);
      /* 22R */      TheG.PutCell(ci.iT_porPrirez       , rowIdx, ptrans_rec.R_PorPrirez);
      /* 23R */      TheG.PutCell(ci.iT_netto           , rowIdx, ptrans_rec.R_Netto);
      /* 24R */      TheG.PutCell(ci.iT_obustave        , rowIdx, ptrans_rec.R_Obustave);
      /* 25R */      TheG.PutCell(ci.iT_2Pay            , rowIdx, ptrans_rec.R_2Pay);
      /* 26R */      TheG.PutCell(ci.iT_naRuke          , rowIdx, ptrans_rec.R_NaRuke);
      /* 27R */      TheG.PutCell(ci.iT_zdrNa           , rowIdx, ptrans_rec.R_ZdrNa);
      /* 28R */      TheG.PutCell(ci.iT_zorNa           , rowIdx, ptrans_rec.R_ZorNa);
      /* 29R */      TheG.PutCell(ci.iT_zapNa           , rowIdx, ptrans_rec.R_ZapNa);
      /* 30R */      TheG.PutCell(ci.iT_zapII           , rowIdx, ptrans_rec.R_ZapII);
      /* 31R */      TheG.PutCell(ci.iT_zapAll          , rowIdx, ptrans_rec.R_ZapAll);
      /* 32R */      TheG.PutCell(ci.iT_doprNa          , rowIdx, ptrans_rec.R_DoprNa);
      /* 33R */      TheG.PutCell(ci.iT_doprAll         , rowIdx, ptrans_rec.R_DoprAll);
      /* 34R */      TheG.PutCell(ci.iT_satiR           , rowIdx, ptrans_rec.R_SatiR);
      /* 35R */      TheG.PutCell(ci.iT_satiB           , rowIdx, ptrans_rec.R_SatiB);
      /* 36R */      TheG.PutCell(ci.iT_satiUk          , rowIdx, ptrans_rec.R_SatiUk);
      /* 37R */      TheG.PutCell(ci.iT_numOfLinesPtrane, rowIdx, ptrans_rec.R_PtranEsCount);
      /* 38R */      TheG.PutCell(ci.iT_numOfLinesPtrano, rowIdx, ptrans_rec.R_PtranOsCount);
      /* 39R */      TheG.PutCell(ci.iT_fondSatiDiff    , rowIdx, ptrans_rec.R_FondSatiDiff);
      /* 40R */      TheG.PutCell(ci.iT_mio1stupNa      , rowIdx, ptrans_rec.R_Mio1stupNa);
      /* 41R */      TheG.PutCell(ci.iT_mio2stupNa      , rowIdx, ptrans_rec.R_Mio2stupNa);
      /* 42R */      TheG.PutCell(ci.iT_mioAllNa        , rowIdx, ptrans_rec.R_MioAllNa);
      /* 43R */      TheG.PutCell(ci.iT_krizPorUk       , rowIdx, ptrans_rec.R_KrizPorUk);
      /* 44R */      TheG.PutCell(ci.iT_krizPorOsn      , rowIdx, ptrans_rec.R_KrizPorOsn);
      /* 45R */      TheG.PutCell(ci.iT_zpiUk           , rowIdx, ptrans_rec.R_ZpiUk);
      /* 46R */      TheG.PutCell(ci.iT_daniZpi         , rowIdx, ptrans_rec.R_DaniZpi);
      /* 23R */      TheG.PutCell(ci.iT_nettoBlue       , rowIdx, ptrans_rec.R_Netto);
      /* 57R */      TheG.PutCell(ci.iT_satiNeR         , rowIdx, ptrans_rec.R_SatiNeR);

      TheG.PutCell(ci.iT_nacIsplName, rowIdx, ZXC.luiListaNacIspl   .GetNameForThisCd(ptrans_rec.T_nacIsplCD));
      TheG.PutCell(ci.iT_neoPrimName, rowIdx, ZXC.luiListaNeoporPrim.GetNameForThisCd(ptrans_rec.T_neoPrimCD));

      // 11.02.2014:
      if(ptrans_rec.T_brutoDodSt.NotZero()) TheG.PutCell(ci.iT_dodBruto, rowIdx, ptrans_rec.T_dodBruto);
   }

   public override void PutDgvTransSumFields1()
   {
      TheSumGrid.PutCell(ci.iT_brutoOsn,   0, placa_rec.S_tBrutoOsn  );
      TheSumGrid.PutCell(ci.iT_bruto100,   0, placa_rec.S_rBruto100  );
      TheSumGrid.PutCell(ci.iT_2Pay,       0, placa_rec.S_r2Pay      );
      TheSumGrid.PutCell(ci.iT_dobMIO,     0, placa_rec.S_tDobMIO    );
      TheSumGrid.PutCell(ci.iT_dodBruto,   0, placa_rec.S_tDodBruto  );
      TheSumGrid.PutCell(ci.iT_dohodak,    0, placa_rec.S_rDohodak   );
      TheSumGrid.PutCell(ci.iT_doprAll,    0, placa_rec.S_rDoprAll   );
      TheSumGrid.PutCell(ci.iT_doprIz,     0, placa_rec.S_rDoprIz    );
      TheSumGrid.PutCell(ci.iT_doprNa,     0, placa_rec.S_rDoprNa    );
      TheSumGrid.PutCell(ci.iT_dopZdr,     0, placa_rec.S_tDopZdr    );
      TheSumGrid.PutCell(ci.iT_mioAll,     0, placa_rec.S_rMioAll    );
      TheSumGrid.PutCell(ci.iT_mio1stup,   0, placa_rec.S_rMio1stup  );
      TheSumGrid.PutCell(ci.iT_mio2stup,   0, placa_rec.S_rMio2stup  );
      TheSumGrid.PutCell(ci.iT_mioOsn,     0, placa_rec.S_rMioOsn    );
      TheSumGrid.PutCell(ci.iT_naRuke,     0, placa_rec.S_rNaRuke    );
      TheSumGrid.PutCell(ci.iT_netoAdd,    0, placa_rec.S_tNetoAdd   );
      TheSumGrid.PutCell(ci.iT_netto,      0, placa_rec.S_rNetto     );
      TheSumGrid.PutCell(ci.iT_obustave,   0, placa_rec.S_rObustave  );
      TheSumGrid.PutCell(ci.iT_odbitak,    0, placa_rec.S_rOdbitak   );
      TheSumGrid.PutCell(ci.iT_por1uk,     0, placa_rec.S_rPor1uk    );
      TheSumGrid.PutCell(ci.iT_por2uk,     0, placa_rec.S_rPor2uk    );
      TheSumGrid.PutCell(ci.iT_por3uk,     0, placa_rec.S_rPor3uk    );
      TheSumGrid.PutCell(ci.iT_por4uk,     0, placa_rec.S_rPor4uk    );
      TheSumGrid.PutCell(ci.iT_porezAll,   0, placa_rec.S_rPorezAll  );
      TheSumGrid.PutCell(ci.iT_porOsn1,    0, placa_rec.S_rPorOsn1   );
      TheSumGrid.PutCell(ci.iT_porOsn2,    0, placa_rec.S_rPorOsn2   );
      TheSumGrid.PutCell(ci.iT_porOsn3,    0, placa_rec.S_rPorOsn3   );
      TheSumGrid.PutCell(ci.iT_porOsn4,    0, placa_rec.S_rPorOsn4   );
      TheSumGrid.PutCell(ci.iT_porOsnAll,  0, placa_rec.S_rPorOsnAll );
      TheSumGrid.PutCell(ci.iT_porPrirez,  0, placa_rec.S_rPorPrirez );
      TheSumGrid.PutCell(ci.iT_premije,    0, placa_rec.S_rPremije   );
      TheSumGrid.PutCell(ci.iT_prijevoz,   0, placa_rec.S_tPrijevoz  );
      TheSumGrid.PutCell(ci.iT_prirez,     0, placa_rec.S_rPrirez    );
      TheSumGrid.PutCell(ci.iT_theBruto,   0, placa_rec.S_rTheBruto  );
      TheSumGrid.PutCell(ci.iT_topObrok,   0, placa_rec.S_tTopObrok  );
      TheSumGrid.PutCell(ci.iT_zapAll,     0, placa_rec.S_rZapAll    );
      TheSumGrid.PutCell(ci.iT_zapII,      0, placa_rec.S_rZapII     );
      TheSumGrid.PutCell(ci.iT_zapNa,      0, placa_rec.S_rZapNa     );
      TheSumGrid.PutCell(ci.iT_zdrNa,      0, placa_rec.S_rZdrNa     );
      TheSumGrid.PutCell(ci.iT_zorNa,      0, placa_rec.S_rZorNa     );
      TheSumGrid.PutCell(ci.iT_zivotno,    0, placa_rec.S_tZivotno   );
      TheSumGrid.PutCell(ci.iT_mioAllNa,   0, placa_rec.S_rMioAllNa  );
      TheSumGrid.PutCell(ci.iT_mio1stupNa, 0, placa_rec.S_rMio1stupNa);
      TheSumGrid.PutCell(ci.iT_mio2stupNa, 0, placa_rec.S_rMio2stupNa);
      TheSumGrid.PutCell(ci.iT_krizPorUk,  0, placa_rec.S_rKrizPorUk );
      TheSumGrid.PutCell(ci.iT_krizPorOsn, 0, placa_rec.S_rKrizPorOsn);
      TheSumGrid.PutCell(ci.iT_satiR,      0, placa_rec.S_rSatiR     );
      TheSumGrid.PutCell(ci.iT_satiB,      0, placa_rec.S_rSatiB     );
      TheSumGrid.PutCell(ci.iT_zpiUk,      0, placa_rec.S_rZpiUk     );
      TheSumGrid.PutCell(ci.iT_daniZpi,    0, placa_rec.S_rDaniZpi   );
      TheSumGrid.PutCell(ci.iT_nettoBlue,  0, placa_rec.S_rNetto     );
      TheSumGrid.PutCell(ci.iT_brDodPoloz, 0, placa_rec.S_tBrDodPoloz);
      TheSumGrid.PutCell(ci.iT_satiNeR   , 0, placa_rec.S_rSatiNeR   );
      TheSumGrid.PutCell(ci.iT_dopZdr2020, 0, placa_rec.S_tDopZdr2020);

   }

   private void PutDgvFields2()
   {
      int rowIdx, idxCorrector;

      TheG2.RowsRemoved -= new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);
      TheG2.RowsAdded -= new DataGridViewRowsAddedEventHandler(grid_RowsAdded);
      TheG2.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(TheG2);

      if(placa_rec.Transes2 != null)
      {
         SetSifrarAndAutocomplete<Person>(null, VvSQL.SorterType.None);

         foreach(Ptrane ptrane_rec in placa_rec.Transes2)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheG2.Rows.Add();

            rowIdx = TheG2.RowCount - idxCorrector;

            PutDgvLineFields2(ptrane_rec, rowIdx, false);

            PutDgvLineResultsFields2(rowIdx, ptrane_rec, false);
         }
      }

      TheG2.RowsAdded += new DataGridViewRowsAddedEventHandler(grid_RowsAdded);
      TheG2.RowsRemoved += new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);

      RenumerateLineNumbers(TheG2, 0);

      UpdateLineCount(TheG2);

      PutDgvTransSumFields2();
   }

   public override void PutDgvLineFields2(VvTransRecord trans_rec, int rowIdx, bool skipRecID_andSerial_Columns)
   {
      Ptrane ptrane_rec = (Ptrane)trans_rec;

      if(skipRecID_andSerial_Columns == false)
      {
         TheG2.PutCell(ci.iT_recID, rowIdx, ptrane_rec.T_recID);
         TheG2.PutCell(ci.iT_serial, rowIdx, ptrane_rec.T_serial);
      }

      TheG2.PutCell(ci.iT_personCD    , rowIdx, ptrane_rec.T_personCD   );
      TheG2.PutCell(ci.iT_prezime     , rowIdx, ptrane_rec.T_prezime    );
      TheG2.PutCell(ci.iT_ime         , rowIdx, ptrane_rec.T_ime        );
      TheG2.PutCell(ci2.iT_vrstaR_cd  , rowIdx, ptrane_rec.T_vrstaR_cd  );
      TheG2.PutCell(ci2.iT_vrstaR_name, rowIdx, ptrane_rec.T_vrstaR_name);
      TheG2.PutCell(ci2.iT_cijPerc    , rowIdx, ptrane_rec.T_cijPerc    );
      TheG2.PutCell(ci2.iT_sati       , rowIdx, ptrane_rec.T_sati       );
      TheG2.PutCell(ci2.iT_rsOO       , rowIdx, ptrane_rec.T_rsOO       );
      TheG2.PutCell(ci2.iT_rsOD       , rowIdx, ptrane_rec.T_rsOD       );
      TheG2.PutCell(ci2.iT_rsDO       , rowIdx, ptrane_rec.T_rsDO       );
      TheG2.PutCell(ci2.iT_stjecatCD  , rowIdx, ptrane_rec.T_stjecatCD  );
      TheG2.PutCell(ci2.iT_primDohCD  , rowIdx, ptrane_rec.T_primDohCD  );
      TheG2.PutCell(ci2.iT_pocKrajCD  , rowIdx, ptrane_rec.T_pocKrajCD  );
      TheG2.PutCell(ci2.iT_ip1gr      , rowIdx, ptrane_rec.T_ip1gr      );
      TheG2.PutCell(ci2.iT_rbrIsprJop , rowIdx, ptrane_rec.T_rbrIsprJop );

      TheG2.PutCell(ci2.iR_stjecatName, rowIdx, ZXC.luiListaStjecatelj.GetNameForThisCd(ptrane_rec.T_stjecatCD)); 
      TheG2.PutCell(ci2.iR_primDohName, rowIdx, ZXC.luiListaPrimDoh   .GetNameForThisCd(ptrane_rec.T_primDohCD)); 
      TheG2.PutCell(ci2.iR_pocKrajName, rowIdx, ZXC.luiListaPocKraj   .GetNameForThisCd(ptrane_rec.T_pocKrajCD)); 

   }

   public override void PutDgvTransSumFields2()
   {
   }

   private void PutDgvFields3()
   {
      int rowIdx, idxCorrector;

      TheG3.RowsRemoved -= new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);
      TheG3.RowsAdded -= new DataGridViewRowsAddedEventHandler(grid_RowsAdded);
      TheG3.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(TheG3);

      if(placa_rec.Transes3 != null)
      {
         SetSifrarAndAutocomplete<Person>(null, VvSQL.SorterType.None);

         foreach(Ptrano ptrano_rec in placa_rec.Transes3)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheG3.Rows.Add();

            rowIdx = TheG3.RowCount - idxCorrector;

            PutDgvLineFields3(ptrano_rec, rowIdx, false);

            PutDgvLineResultsFields3(rowIdx, ptrano_rec, false);
         }
      }
      TheG3.RowsAdded += new DataGridViewRowsAddedEventHandler(grid_RowsAdded);
      TheG3.RowsRemoved += new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);

      RenumerateLineNumbers(TheG3, 0);

      UpdateLineCount(TheG3);

      //04.12.2013 kak!!!
      //PutDgvTransSumFields3();
      if(this is PlacaDUC || this is Placa2014DUC) PutDgvTransSumFields3();
   }

   public override void PutDgvLineFields3(VvTransRecord trans_rec, int rowIdx, bool skipRecID_andSerial_Columns)
   {
      Ptrano ptrano_rec = (Ptrano)trans_rec;

      if(skipRecID_andSerial_Columns == false)
      {
         TheG3.PutCell(ci.iT_recID, rowIdx, ptrano_rec.T_recID);
         TheG3.PutCell(ci.iT_serial, rowIdx, ptrano_rec.T_serial);
      }

      TheG3.PutCell(ci.iT_personCD, rowIdx, ptrano_rec.T_personCD);

      TheG3.PutCell(ci.iT_prezime, rowIdx, ptrano_rec.T_prezime);

      TheG3.PutCell(ci.iT_ime, rowIdx, ptrano_rec.T_ime);

      TheG3.PutCell(ci3.iT_dateStart, rowIdx, ptrano_rec.T_dateStart);
      TheG3.PutCell(ci3.iT_ukBrRata , rowIdx, ptrano_rec.T_ukBrRata);
      TheG3.PutCell(ci3.iT_opisOb   , rowIdx, ptrano_rec.T_opisOb);
      TheG3.PutCell(ci3.iT_kupdob_cd, rowIdx, ptrano_rec.T_kupdob_cd);
      TheG3.PutCell(ci3.iT_kupdob_tk, rowIdx, ptrano_rec.T_kupdob_tk);
      TheG3.PutCell(ci3.iT_iznosOb  , rowIdx, ptrano_rec.T_iznosOb);
      TheG3.PutCell(ci3.iT_isZbir   , rowIdx, VvCheckBox.GetString4Bool(ptrano_rec.T_isZbir));
      TheG3.PutCell(ci3.iT_partija  , rowIdx, ptrano_rec.T_partija);
      TheG3.PutCell(ci3.iT_izNetoaSt, rowIdx, ptrano_rec.T_izNetoaSt);
      TheG3.PutCell(ci3.iT_rbrRate  , rowIdx, ptrano_rec.T_rbrRate);
      TheG3.PutCell(ci3.iT_isZastRn , rowIdx, GetOneLetter4PtranoKind(ptrano_rec.T_ptranoKind));

   }

   public override void PutDgvTransSumFields3()
   {
      TheSumGrid3[ci3.iT_iznosOb, 0].Value = placa_rec.S_rObustave;
   }

   // $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ 
   // $$$$ GetFields $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ 
   // $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ 

   private void GetDgvFields(bool dirtyFlagging)
   {
      GetDgvFields1(dirtyFlagging);
      GetDgvFields2(dirtyFlagging);
      GetDgvFields3(dirtyFlagging);
   }

   private void GetDgvFields1(bool dirtyFlagging)
   {
      uint[] recIDtable;
      int rIdx;

      if(dirtyFlagging && ThePolyGridTabControl.SelectedTab.Title != TabPageTitle1) return;

      if(dirtyFlagging == true && ZXC.TheVvForm.VvFlag_RowsAreAddingOrDeletingOrBoth == false)
      {
         if((TheG.CurrentCell != null && TheG.CurrentCell.IsInEditMode) || ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode)
         {
            GetDgvLineFields1(TheG.CurrentRow.Index, dirtyFlagging, null);

            ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode = false;
         }

         return;
      }

      if(TheG.RowCount > 0) recIDtable = new uint[TheG.RowCount - 1];
      else recIDtable = null;

      placa_rec.DiscardPreviouslyAddedTranses();

      for(rIdx = 0; rIdx < TheG.RowCount - 1; ++rIdx)
      {
         GetDgvLineFields1(rIdx, dirtyFlagging, recIDtable);
      }

      MarkTransesToDelete(recIDtable);
   }

   private void GetDgvFields2(bool dirtyFlagging)
   {
      uint[] recIDtable;
      int rIdx;

      if(dirtyFlagging && ThePolyGridTabControl.SelectedTab.Title != TabPageTitle2) return;

      if(dirtyFlagging == true && ZXC.TheVvForm.VvFlag_RowsAreAddingOrDeletingOrBoth == false)
      {
         //15.10.2014: zbog TakeMVR; kada aktivni grid NIJE TheG2 onda je TheG2.CurrentRow NULL pa ode exception?!                                                 
       //if((TheG2.CurrentCell != null && TheG2.CurrentCell.IsInEditMode) ||  ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode                             ) 
         if((TheG2.CurrentCell != null && TheG2.CurrentCell.IsInEditMode) || (ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode && TheG2.CurrentRow != null))
         {
            GetDgvLineFields2(TheG2.CurrentRow.Index, dirtyFlagging, null);

            ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode = false;
         }

         return;
      }

      if(TheG2.RowCount > 0) recIDtable = new uint[TheG2.RowCount - 1];
      else recIDtable = null;

      placa_rec.DiscardPreviouslyAddedTranses2();

      for(rIdx = 0; rIdx < TheG2.RowCount - 1; ++rIdx)
      {
         GetDgvLineFields2(rIdx, dirtyFlagging, recIDtable);
      }

      MarkTranses2ToDelete(recIDtable);
   }

   private void GetDgvFields3(bool dirtyFlagging)
   {
      uint[] recIDtable;
      int rIdx;

      if(dirtyFlagging && ThePolyGridTabControl.SelectedTab.Title != TabPageTitle3) return;

      if(dirtyFlagging == true && ZXC.TheVvForm.VvFlag_RowsAreAddingOrDeletingOrBoth == false)
      {
         if((TheG3.CurrentCell != null && TheG3.CurrentCell.IsInEditMode) || ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode)
         {
            GetDgvLineFields3(TheG3.CurrentRow.Index, dirtyFlagging, null);

            ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode = false;
         }

         return;
      }

      if(TheG3.RowCount > 0) recIDtable = new uint[TheG3.RowCount - 1];
      else recIDtable = null;

      placa_rec.DiscardPreviouslyAddedTranses3();

      for(rIdx = 0; rIdx < TheG3.RowCount - 1; ++rIdx)
      {
         GetDgvLineFields3(rIdx, dirtyFlagging, recIDtable);
      }

      MarkTranses3ToDelete(recIDtable);
   }

   public override VvTransRecord GetDgvLineFields1(int rIdx, bool dirtyFlagging, uint[] recIDtable)
   {
      uint recID;
      bool DB_RWT;
      Ptrans db_rec;

      // dgvAtrans_rec: buffer za GetField iz DataGridWievRow-a                                        
      // db_rec : pointer na existing row u DB-u kojega ako je EditedHasChanges treba RWTREC-ati 

      recID = TheG.GetUint32Cell(ci.iT_recID, rIdx, dirtyFlagging);

      if(recIDtable != null) recIDtable[rIdx] = recID;

      dgvPtrans_rec = new Ptrans();

      if(recID > 0) // Postojeci redak 
      {
         db_rec = placa_rec.Transes.SingleOrDefault(vvDR => vvDR.VirtualRecID == recID);
      }
      else // novododani redak 
      {
         db_rec = null;
      }

      DB_RWT = (db_rec != null);

      dgvPtrans_rec.T_recID = recID;

      dgvPtrans_rec.T_parentID = placa_rec.RecID;

      #region GetColumns

                                   dgvPtrans_rec.T_serial = (ushort)(rIdx + 1);
      if(DB_RWT) db_rec.T_serial = dgvPtrans_rec.T_serial;

                                   dgvPtrans_rec.T_dokNum = placa_rec.DokNum;
      if(DB_RWT) db_rec.T_dokNum = dgvPtrans_rec.T_dokNum;

                                    dgvPtrans_rec.T_dokDate = placa_rec.DokDate;
      if(DB_RWT) db_rec.T_dokDate = dgvPtrans_rec.T_dokDate;

                               dgvPtrans_rec.T_TT = placa_rec.TT;
      if(DB_RWT) db_rec.T_TT = dgvPtrans_rec.T_TT;

                                  dgvPtrans_rec.T_ttNum = placa_rec.TtNum;
      if(DB_RWT) db_rec.T_ttNum = dgvPtrans_rec.T_ttNum;

                                   dgvPtrans_rec.T_MMYYYY = placa_rec.MMYYYY;
      if(DB_RWT) db_rec.T_MMYYYY = dgvPtrans_rec.T_MMYYYY;

                                     dgvPtrans_rec.T_FondSati = placa_rec.FondSati;
      if(DB_RWT) db_rec.T_FondSati = dgvPtrans_rec.T_FondSati;

                                   dgvPtrans_rec.T_RSm_ID = placa_rec.RSm_ID;
      if(DB_RWT) db_rec.T_RSm_ID = dgvPtrans_rec.T_RSm_ID;

      if(TheG.CI_OK(ci.iT_personCD))
      {
                                        dgvPtrans_rec.T_personCD = TheG.GetUint32Cell(ci.iT_personCD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_personCD = dgvPtrans_rec.T_personCD;
      }
      if(TheG.CI_OK(ci.iT_prezime))
      {
                                       dgvPtrans_rec.T_prezime = TheG.GetStringCell(ci.iT_prezime, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_prezime = dgvPtrans_rec.T_prezime;
      }
      if(TheG.CI_OK(ci.iT_ime))
      {
                                   dgvPtrans_rec.T_ime = TheG.GetStringCell(ci.iT_ime, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_ime = dgvPtrans_rec.T_ime;
      }

      if(TheG.CI_OK(ci.iT_brutoOsn))
      {
                                        dgvPtrans_rec.T_brutoOsn = TheG.GetDecimalCell(ci.iT_brutoOsn, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_brutoOsn = dgvPtrans_rec.T_brutoOsn;
      }
      if(TheG.CI_OK(ci.iT_topObrok))
      {
                                        dgvPtrans_rec.T_topObrok = TheG.GetDecimalCell(ci.iT_topObrok, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_topObrok = dgvPtrans_rec.T_topObrok;
      }
      if(TheG.CI_OK(ci.iT_godStaza))
      {
                                        dgvPtrans_rec.T_godStaza = TheG.GetDecimalCell(ci.iT_godStaza, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_godStaza = dgvPtrans_rec.T_godStaza;
      }
      if(TheG.CI_OK(ci.iT_dodBruto))
      {
                                        dgvPtrans_rec.T_dodBruto = TheG.GetDecimalCell(ci.iT_dodBruto, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_dodBruto = dgvPtrans_rec.T_dodBruto;
      }
      if(TheG.CI_OK(ci.iT_isMioII))
      {
                                     //dgvAtrans_rec.T_isMioII = TheG.GetBoolCell(ci.iT_isMioII, rIdx, dirtyFlagging);
                                       dgvPtrans_rec.T_isMioII = VvCheckBox.GetBool4String(TheG.GetStringCell(ci.iT_isMioII, rIdx, dirtyFlagging));
         if(DB_RWT) db_rec.T_isMioII = dgvPtrans_rec.T_isMioII;
      }
      if(TheG.CI_OK(ci.iT_spc))
      {
                                   dgvPtrans_rec.T_spc = GetSpecEnumFromFirstLetter(TheG.GetStringCell(ci.iT_spc, rIdx, dirtyFlagging));
         if(DB_RWT) db_rec.T_spc = dgvPtrans_rec.T_spc;
      }
      if(TheG.CI_OK(ci.iT_koef))
      {
                                    dgvPtrans_rec.T_koef = TheG.GetDecimalCell(ci.iT_koef, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_koef = dgvPtrans_rec.T_koef;
      }
      if(TheG.CI_OK(ci.iT_zivotno))
      {
                                       dgvPtrans_rec.T_zivotno = TheG.GetDecimalCell(ci.iT_zivotno, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_zivotno = dgvPtrans_rec.T_zivotno;
      }
      if(TheG.CI_OK(ci.iT_dopZdr))
      {
                                      dgvPtrans_rec.T_dopZdr = TheG.GetDecimalCell(ci.iT_dopZdr, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_dopZdr = dgvPtrans_rec.T_dopZdr;
      }
      if(TheG.CI_OK(ci.iT_dobMIO))
      {
                                      dgvPtrans_rec.T_dobMIO = TheG.GetDecimalCell(ci.iT_dobMIO, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_dobMIO = dgvPtrans_rec.T_dobMIO;
      }
      if(TheG.CI_OK(ci.iT_koefHRVI))
      {
                                        dgvPtrans_rec.T_koefHRVI = TheG.GetDecimalCell(ci.iT_koefHRVI, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_koefHRVI = dgvPtrans_rec.T_koefHRVI;
      }
      if(TheG.CI_OK(ci.iT_invalidTip))
      {
                                          dgvPtrans_rec.T_invalidTip = GetInvalidEnumFromFirstLetter(TheG.GetStringCell(ci.iT_invalidTip, rIdx, dirtyFlagging));
         if(DB_RWT) db_rec.T_invalidTip = dgvPtrans_rec.T_invalidTip;
      }
      if(TheG.CI_OK(ci.iT_opcCD))
      {
                                     dgvPtrans_rec.T_opcCD = TheG.GetStringCell(ci.iT_opcCD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_opcCD = dgvPtrans_rec.T_opcCD;
      }
      if(TheG.CI_OK(ci.iT_opcName))
      {
                                       dgvPtrans_rec.T_opcName = TheG.GetStringCell(ci.iT_opcName, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_opcName = dgvPtrans_rec.T_opcName;
      }
      if(TheG.CI_OK(ci.iT_opcRadCD))
      {
                                        dgvPtrans_rec.T_opcRadCD = TheG.GetStringCell(ci.iT_opcRadCD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_opcRadCD = dgvPtrans_rec.T_opcRadCD;
      }
      if(TheG.CI_OK(ci.iT_opcRadName))
      {
                                          dgvPtrans_rec.T_opcRadName = TheG.GetStringCell(ci.iT_opcRadName, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_opcRadName = dgvPtrans_rec.T_opcRadName;
      }
      if(TheG.CI_OK(ci.iT_stPrirez))
      {
                                        dgvPtrans_rec.T_stPrirez = TheG.GetDecimalCell(ci.iT_stPrirez, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_stPrirez = dgvPtrans_rec.T_stPrirez;
      }
      if(TheG.CI_OK(ci.iT_netoAdd))
      {
                                       dgvPtrans_rec.T_NetoAdd = TheG.GetDecimalCell(ci.iT_netoAdd, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_NetoAdd = dgvPtrans_rec.T_NetoAdd;
      }
      if(TheG.CI_OK(ci.iT_isDirNeto))
      {
                                       //dgvAtrans_rec.T_isDirNeto = TheG.GetBoolCell(ci.iT_isDirNeto, rIdx, dirtyFlagging);
                                         dgvPtrans_rec.T_isDirNeto = VvCheckBox.GetBool4String(TheG.GetStringCell(ci.iT_isDirNeto, rIdx, dirtyFlagging));
         if(DB_RWT) db_rec.T_isDirNeto = dgvPtrans_rec.T_isDirNeto;
      }
      if(TheG.CI_OK(ci.iT_prijevoz))
      {
                                        dgvPtrans_rec.T_prijevoz = TheG.GetDecimalCell(ci.iT_prijevoz, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_prijevoz = dgvPtrans_rec.T_prijevoz;
      }


      if(TheG.CI_OK(ci.iT_isPoluSat))
      {
                                         //dgvAtrans_rec.T_isMioII = TheG.GetBoolCell(ci.iT_isMioII, rIdx, dirtyFlagging);
                                         dgvPtrans_rec.T_IsPoluSat = VvCheckBox.GetBool4String(TheG.GetStringCell(ci.iT_isPoluSat, rIdx, dirtyFlagging));
         if(DB_RWT) db_rec.T_IsPoluSat = dgvPtrans_rec.T_IsPoluSat;
      }

      if(TheG.CI_OK(ci.iT_rsB))
      {
                                   dgvPtrans_rec.T_rsB = TheG.GetUint32Cell(ci.iT_rsB, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_rsB = dgvPtrans_rec.T_rsB;
      }
      
      if(TheG.CI_OK(ci.iT_nacIsplCD))
      {
                                         dgvPtrans_rec.T_nacIsplCD = TheG.GetStringCell(ci.iT_nacIsplCD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_nacIsplCD = dgvPtrans_rec.T_nacIsplCD;
      }
    
      if(TheG.CI_OK(ci.iT_neoPrimCD))
      {
                                         dgvPtrans_rec.T_neoPrimCD = TheG.GetStringCell(ci.iT_neoPrimCD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_neoPrimCD = dgvPtrans_rec.T_neoPrimCD;
      }
      
      if(TheG.CI_OK(ci.iT_dokumCD))
      {
                                       dgvPtrans_rec.T_dokumCD = TheG.GetStringCell(ci.iT_dokumCD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_dokumCD = dgvPtrans_rec.T_dokumCD;
      }
      if(TheG.CI_OK(ci.iT_brutoDodSt))
      {
                                          dgvPtrans_rec.T_brutoDodSt = TheG.GetDecimalCell(ci.iT_brutoDodSt, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_brutoDodSt = dgvPtrans_rec.T_brutoDodSt;
      }
      if(TheG.CI_OK(ci.iT_brDodPoloz))
      {
                                          dgvPtrans_rec.T_brDodPoloz = TheG.GetDecimalCell(ci.iT_brDodPoloz, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_brDodPoloz = dgvPtrans_rec.T_brDodPoloz;
      }
      if(TheG.CI_OK(ci.iT_koefBruto1))
      {
                                          dgvPtrans_rec.T_koefBruto1 = TheG.GetDecimalCell(ci.iT_koefBruto1, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_koefBruto1 = dgvPtrans_rec.T_koefBruto1;
      }
      if(TheG.CI_OK(ci.iT_dnFondSati))
      {
                                          dgvPtrans_rec.T_dnFondSati = TheG.GetDecimalCell(ci.iT_dnFondSati, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_dnFondSati = dgvPtrans_rec.T_dnFondSati;
      }
      if(TheG.CI_OK(ci.iT_thisStazSt))
      {
                                          dgvPtrans_rec.T_thisStazSt = TheG.GetDecimalCell(ci.iT_thisStazSt, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_thisStazSt = dgvPtrans_rec.T_thisStazSt;
      }
      if(TheG.CI_OK(ci.iT_brutoDodSt2))
      {
                                           dgvPtrans_rec.T_brutoDodSt2 = TheG.GetDecimalCell(ci.iT_brutoDodSt2, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_brutoDodSt2 = dgvPtrans_rec.T_brutoDodSt2;
      }
      if(TheG.CI_OK(ci.iT_brutoDodSt3))
      {
                                           dgvPtrans_rec.T_brutoDodSt3 = TheG.GetDecimalCell(ci.iT_brutoDodSt3, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_brutoDodSt3 = dgvPtrans_rec.T_brutoDodSt3;
      }
      if(TheG.CI_OK(ci.iT_pr3mjBruto))
      {
                                          dgvPtrans_rec.T_pr3mjBruto = TheG.GetDecimalCell(ci.iT_pr3mjBruto, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_pr3mjBruto = dgvPtrans_rec.T_pr3mjBruto;
      }
      if(TheG.CI_OK(ci.iT_brutoKorekc))
      {
                                           dgvPtrans_rec.T_brutoKorekc = TheG.GetDecimalCell(ci.iT_brutoKorekc, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_brutoKorekc = dgvPtrans_rec.T_brutoKorekc;
      }

      if(TheG.CI_OK(ci.iT_dopZdr2020))
      {
                                          dgvPtrans_rec.T_dopZdr2020 = TheG.GetDecimalCell(ci.iT_dopZdr2020, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_dopZdr2020 = dgvPtrans_rec.T_dopZdr2020;
      }

      if(TheG.CI_OK(ci.iT_stPorez1))
      {
                                        dgvPtrans_rec.T_stPorez1 = TheG.GetDecimalCell(ci.iT_stPorez1, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_stPorez1 = dgvPtrans_rec.T_stPorez1;
      }
      if(TheG.CI_OK(ci.iT_stPorez2))
      {
                                        dgvPtrans_rec.T_stPorez2 = TheG.GetDecimalCell(ci.iT_stPorez2, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_stPorez2 = dgvPtrans_rec.T_stPorez2;
      }
      if(TheG.CI_OK(ci.iT_fixMio1Olak))
      {
                                           dgvPtrans_rec.T_fixMio1Olak = TheG.GetDecimalCell(ci.iT_fixMio1Olak, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_fixMio1Olak = dgvPtrans_rec.T_fixMio1Olak;
      }






      #endregion GetColumns

      if(dgvPtrans_rec.T_recID == 0) // ADDED NEW, FRESH Ftrans             
      {
         placa_rec.InvokeTransRemove(dgvPtrans_rec);

         dgvPtrans_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;

         dgvPtrans_rec.CalcTransResults(placa_rec);

         placa_rec.Transes.Add(dgvPtrans_rec);
      }
      else if(db_rec.EditedHasChanges())
      {
         db_rec.SaveTransesWriteMode = ZXC.WriteMode.Edit;
      }
      else
      {
         db_rec.SaveTransesWriteMode = ZXC.WriteMode.None;
      }

      return dgvPtrans_rec;
   }

   public override VvTransRecord GetDgvLineFields2(int rIdx, bool dirtyFlagging, uint[] recIDtable)
   {
      uint recID;
      bool DB_RWT;
      Ptrane db_rec;

      // dgvAtrans_rec: buffer za GetField iz DataGridWievRow-a                                        
      // db_rec : pointer na existing row u DB-u kojega ako je EditedHasChanges treba RWTREC-ati 

      recID = TheG2.GetUint32Cell(ci.iT_recID, rIdx, dirtyFlagging);

      if(recIDtable != null) recIDtable[rIdx] = recID;

      dgvPtrane_rec = new Ptrane();

      if(recID > 0) // Postojeci redak 
      {
         db_rec = placa_rec.Transes2.SingleOrDefault(vvDR => vvDR.VirtualRecID == recID);
      }
      else // novododani redak 
      {
         db_rec = null;
      }

      DB_RWT = (db_rec != null);

      dgvPtrane_rec.T_recID = recID;

      dgvPtrane_rec.T_parentID = placa_rec.RecID;

      #region GetColumns

      //TODO: mozda bug! ovdje Za prvih Para kolona koristis ci. a Ne ci2.

                                   dgvPtrane_rec.T_serial = (ushort)(rIdx + 1);
      if(DB_RWT) db_rec.T_serial = dgvPtrane_rec.T_serial;

                                   dgvPtrane_rec.T_dokNum = placa_rec.DokNum;
      if(DB_RWT) db_rec.T_dokNum = dgvPtrane_rec.T_dokNum;

                                    dgvPtrane_rec.T_dokDate = placa_rec.DokDate;
      if(DB_RWT) db_rec.T_dokDate = dgvPtrane_rec.T_dokDate;

                               dgvPtrane_rec.T_TT = placa_rec.TT;
      if(DB_RWT) db_rec.T_TT = dgvPtrane_rec.T_TT;

                                  dgvPtrane_rec.T_ttNum = placa_rec.TtNum;
      if(DB_RWT) db_rec.T_ttNum = dgvPtrane_rec.T_ttNum;

      if(TheG2.CI_OK(ci.iT_personCD))
      {
                                        dgvPtrane_rec.T_personCD = TheG2.GetUint32Cell(ci.iT_personCD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_personCD = dgvPtrane_rec.T_personCD;
      }
      if(TheG2.CI_OK(ci.iT_prezime))
      {
                                       dgvPtrane_rec.T_prezime = TheG2.GetStringCell(ci.iT_prezime, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_prezime = dgvPtrane_rec.T_prezime;
      }
      if(TheG2.CI_OK(ci.iT_ime))
      {
                                   dgvPtrane_rec.T_ime = TheG2.GetStringCell(ci.iT_ime, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_ime = dgvPtrane_rec.T_ime;
      }

      if(TheG2.CI_OK(ci2.iT_vrstaR_cd))
      {
                                         dgvPtrane_rec.T_vrstaR_cd = TheG2.GetStringCell(ci2.iT_vrstaR_cd, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_vrstaR_cd = dgvPtrane_rec.T_vrstaR_cd;
      }
      if(TheG2.CI_OK(ci2.iT_vrstaR_name))
      {
                                           dgvPtrane_rec.T_vrstaR_name = TheG2.GetStringCell(ci2.iT_vrstaR_name, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_vrstaR_name = dgvPtrane_rec.T_vrstaR_name;
      }
      if(TheG2.CI_OK(ci2.iT_cijPerc))
      {
                                       dgvPtrane_rec.T_cijPerc = TheG2.GetDecimalCell(ci2.iT_cijPerc, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_cijPerc = dgvPtrane_rec.T_cijPerc;
      }
      if(TheG2.CI_OK(ci2.iT_sati))
      {
                                    dgvPtrane_rec.T_sati = TheG2.GetDecimalCell(ci2.iT_sati, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_sati = dgvPtrane_rec.T_sati;
      }
      if(TheG2.CI_OK(ci2.iT_rsOO))
      {
                                    dgvPtrane_rec.T_rsOO = TheG2.GetStringCell(ci2.iT_rsOO, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_rsOO = dgvPtrane_rec.T_rsOO;
      }

      if(TheG2.CI_OK(ci2.iT_rsOD))
      {
                                    dgvPtrane_rec.T_rsOD = TheG2.GetUint32Cell(ci2.iT_rsOD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_rsOD = dgvPtrane_rec.T_rsOD;
      }
      if(TheG2.CI_OK(ci2.iT_rsDO))
      {
                                    dgvPtrane_rec.T_rsDO = TheG2.GetUint32Cell(ci2.iT_rsDO, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_rsDO = dgvPtrane_rec.T_rsDO;
      }
      if(TheG2.CI_OK(ci2.iT_stjecatCD))
      {
                                         dgvPtrane_rec.T_stjecatCD = TheG2.GetStringCell(ci2.iT_stjecatCD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_stjecatCD = dgvPtrane_rec.T_stjecatCD;
      }
      if(TheG2.CI_OK(ci2.iT_primDohCD))
      {
                                         dgvPtrane_rec.T_primDohCD = TheG2.GetStringCell(ci2.iT_primDohCD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_primDohCD = dgvPtrane_rec.T_primDohCD;
      }
      if(TheG2.CI_OK(ci2.iT_pocKrajCD))
      {
                                         dgvPtrane_rec.T_pocKrajCD = TheG2.GetStringCell(ci2.iT_pocKrajCD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_pocKrajCD = dgvPtrane_rec.T_pocKrajCD;
      }
      if(TheG2.CI_OK(ci2.iT_ip1gr))
      {
                                     dgvPtrane_rec.T_ip1gr = TheG2.GetUint32Cell(ci2.iT_ip1gr, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_ip1gr = dgvPtrane_rec.T_ip1gr;
      }
      if(TheG2.CI_OK(ci2.iT_rbrIsprJop))
      {
                                          dgvPtrane_rec.T_rbrIsprJop = TheG2.GetIntCell(ci2.iT_rbrIsprJop, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_rbrIsprJop = dgvPtrane_rec.T_rbrIsprJop;
      }

      #endregion GetColumns

      if(dgvPtrane_rec.T_recID == 0) // ADDED NEW, FRESH Ftrans             
      {
         placa_rec.InvokeTransRemove2(dgvPtrane_rec);

         dgvPtrane_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;

         placa_rec.Transes2.Add(dgvPtrane_rec);
      }
      else if(db_rec.EditedHasChanges())
      {
         db_rec.SaveTransesWriteMode = ZXC.WriteMode.Edit;
      }
      else
      {
         db_rec.SaveTransesWriteMode = ZXC.WriteMode.None;
      }

      return dgvPtrane_rec;
   }

   public override VvTransRecord GetDgvLineFields3(int rIdx, bool dirtyFlagging, uint[] recIDtable)
   {
      uint recID;
      bool DB_RWT;
      Ptrano db_rec;

      // dgvAtrans_rec: buffer za GetField iz DataGridWievRow-a                                        
      // db_rec : pointer na existing row u DB-u kojega ako je EditedHasChanges treba RWTREC-ati 

      recID = TheG3.GetUint32Cell(ci.iT_recID, rIdx, dirtyFlagging);

      if(recIDtable != null) recIDtable[rIdx] = recID;

      dgvPtrano_rec = new Ptrano();

      if(recID > 0) // Postojeci redak 
      {
         db_rec = placa_rec.Transes3.SingleOrDefault(vvDR => vvDR.VirtualRecID == recID);
      }
      else // novododani redak 
      {
         db_rec = null;
      }

      DB_RWT = (db_rec != null);

      dgvPtrano_rec.T_recID = recID;

      dgvPtrano_rec.T_parentID = placa_rec.RecID;

      #region GetColumns

                                   dgvPtrano_rec.T_serial = (ushort)(rIdx + 1);
      if(DB_RWT) db_rec.T_serial = dgvPtrano_rec.T_serial;

                                   dgvPtrano_rec.T_dokNum = placa_rec.DokNum;
      if(DB_RWT) db_rec.T_dokNum = dgvPtrano_rec.T_dokNum;

                                    dgvPtrano_rec.T_dokDate = placa_rec.DokDate;
      if(DB_RWT) db_rec.T_dokDate = dgvPtrano_rec.T_dokDate;

                               dgvPtrano_rec.T_TT = placa_rec.TT;
      if(DB_RWT) db_rec.T_TT = dgvPtrano_rec.T_TT;

                                  dgvPtrano_rec.T_ttNum = placa_rec.TtNum;
      if(DB_RWT) db_rec.T_ttNum = dgvPtrano_rec.T_ttNum;

      if(TheG3.CI_OK(ci.iT_personCD))
      {
         dgvPtrano_rec.T_personCD = TheG3.GetUint32Cell(ci.iT_personCD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_personCD = dgvPtrano_rec.T_personCD;
      }
      if(TheG3.CI_OK(ci.iT_prezime))
      {
         dgvPtrano_rec.T_prezime = TheG3.GetStringCell(ci.iT_prezime, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_prezime = dgvPtrano_rec.T_prezime;
      }
      if(TheG3.CI_OK(ci.iT_ime))
      {
         dgvPtrano_rec.T_ime = TheG3.GetStringCell(ci.iT_ime, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_ime = dgvPtrano_rec.T_ime;
      }

      if(TheG3.CI_OK(ci3.iT_dateStart))
      {
         dgvPtrano_rec.T_dateStart = TheG3.GetDateCell(ci3.iT_dateStart, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_dateStart = dgvPtrano_rec.T_dateStart;
      }
      if(TheG3.CI_OK(ci3.iT_ukBrRata))
      {
         dgvPtrano_rec.T_ukBrRata = TheG3.GetUint32Cell(ci3.iT_ukBrRata, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_ukBrRata = dgvPtrano_rec.T_ukBrRata;
      }
      if(TheG3.CI_OK(ci3.iT_opisOb))
      {
             dgvPtrano_rec.T_opisOb = TheG3.GetStringCell(ci3.iT_opisOb, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_opisOb = dgvPtrano_rec.T_opisOb;
      }
      if(TheG3.CI_OK(ci3.iT_kupdob_cd))
      {
             dgvPtrano_rec.T_kupdob_cd = TheG3.GetUint32Cell(ci3.iT_kupdob_cd, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_kupdob_cd = dgvPtrano_rec.T_kupdob_cd;
      }
      if(TheG3.CI_OK(ci3.iT_kupdob_tk))
      {
             dgvPtrano_rec.T_kupdob_tk = TheG3.GetStringCell(ci3.iT_kupdob_tk, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_kupdob_tk = dgvPtrano_rec.T_kupdob_tk;
      }
      if(TheG3.CI_OK(ci3.iT_iznosOb))
      {
             dgvPtrano_rec.T_iznosOb = TheG3.GetDecimalCell(ci3.iT_iznosOb, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_iznosOb = dgvPtrano_rec.T_iznosOb;
      }
      if(TheG3.CI_OK(ci3.iT_isZbir))
      {
             dgvPtrano_rec.T_isZbir = VvCheckBox.GetBool4String(TheG3.GetStringCell(ci3.iT_isZbir, rIdx, dirtyFlagging));
         if(DB_RWT) db_rec.T_isZbir = dgvPtrano_rec.T_isZbir;
      }
      if(TheG3.CI_OK(ci3.iT_partija))
      {
             dgvPtrano_rec.T_partija = TheG3.GetStringCell(ci3.iT_partija, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_partija = dgvPtrano_rec.T_partija;
      }
      if(TheG3.CI_OK(ci3.iT_izNetoaSt))
      {
             dgvPtrano_rec.T_izNetoaSt = TheG3.GetDecimalCell(ci3.iT_izNetoaSt, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_izNetoaSt = dgvPtrano_rec.T_izNetoaSt;
      }
      if(TheG3.CI_OK(ci3.iT_rbrRate))
      {
             dgvPtrano_rec.T_rbrRate = TheG3.GetUint32Cell(ci3.iT_rbrRate, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_rbrRate = dgvPtrano_rec.T_rbrRate;
      }
      if(TheG3.CI_OK(ci3.iT_isZastRn)) // ptranoKind
      {
             dgvPtrano_rec.T_ptranoKind = GetPtranoKindEnumFromFirstLetter(TheG3.GetStringCell(ci3.iT_isZastRn, rIdx, dirtyFlagging));
         if(DB_RWT) db_rec.T_ptranoKind = dgvPtrano_rec.T_ptranoKind;
      }

      #endregion GetColumns

      if(dgvPtrano_rec.T_recID == 0) // ADDED NEW, FRESH Ftrans             
      {
         placa_rec.InvokeTransRemove3(dgvPtrano_rec);

         dgvPtrano_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;

         placa_rec.Transes3.Add(dgvPtrano_rec);
      }
      else if(db_rec.EditedHasChanges())
      {
         db_rec.SaveTransesWriteMode = ZXC.WriteMode.Edit;
      }
      else
      {
         db_rec.SaveTransesWriteMode = ZXC.WriteMode.None;
      }

      return dgvPtrano_rec;
   }

   #endregion PutDgvFields(), GetDgvFields()

   #region Overriders and specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.placa_rec; }
      set { this.placa_rec = (Placa)value; }
   }

   public override VvDocumentRecord VirtualDocumentRecord
   {
      get { return this.VirtualDataRecord as VvDocumentRecord; }
      set { this.VirtualDataRecord = (Placa)value; }
   }

   public override VvPolyDocumRecord VirtualPolyDocumRecord
   {
      get { return this.VirtualDocumentRecord as VvPolyDocumRecord; }
      set { this.VirtualDocumentRecord = (Placa)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.PlacaDao; }
   }

   public override VvDaoBase TheVvDaoTrans
   {
      get { return ZXC.PtransDao; }
   }

   public override VvDaoBase TheVvDaoTrans2
   {
      get { return ZXC.PtraneDao; }
   }

   public override VvDaoBase TheVvDaoTrans3
   {
      get { return ZXC.PtranoDao; }
   }

   #region Document_Database_ColumnIndexes, Transes_Database_ColumnIndexes

   /// <summary>
   /// Column Index U DataBase (NE na DataGridView-u to ti je 'ci')
   /// </summary>
   private PlacaDao.PlacaCI DB_ci
   {
      get { return ZXC.PlaCI; }
   }

   /// <summary>
   /// Column Index u PTRANS DataBase (NE na DataGridView-u to ti je 'ci')
   /// </summary>
   private PtransDao.PtransCI DB_Tci
   {
      get { return ZXC.PtrCI; }
   }

   /// <summary>
   /// Column Index u PTRANE DataBase (NE na DataGridView-u to ti je 'ci')
   /// </summary>
   private PtraneDao.PtraneCI DB_Tci2
   {
      get { return ZXC.PteCI; }
   }

   /// <summary>
   /// Column Index u PTRANO DataBase (NE na DataGridView-u to ti je 'ci')
   /// </summary>
   private PtranoDao.PtranoCI DB_Tci3
   {
      get { return ZXC.PtoCI; }
   }

   #endregion Document_Database_ColumnIndexes, Transes_Database_ColumnIndexes

   #region Update_VvDataRecord (Legacy naming convention)

   public override VvFindDialog CreateVvFindDialog()
   {
      return CreateFind_Placa_Dialog();
   }

   public static VvFindDialog CreateFind_Placa_Dialog()
   {
      VvForm.VvSubModul vvSubModul = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsPLA);
      VvDataRecord vvDataRecord = ZXC.TheVvForm.CreateTheVvDataRecord_SwitchSubModulEnum(vvSubModul);

      VvFindDialog vvFindDialog = new VvFindDialog();

      VvRecLstUC vvRecListUC = new PlacaListUC(vvFindDialog, (Placa)vvDataRecord, vvSubModul);
      vvFindDialog.TheRecListUC = vvRecListUC;

      return vvFindDialog;
   }

   #endregion Update_VvDataRecord (Legacy naming convention)

   #region PrintDocumentRecord

   public PlacaDokumentFilterUC ThePlacaDokumentFilterUC { get; set; }
   public PlacaDokumentFilter   ThePlacaDokumentFilter   { get; set; }

   public override string VirtualReportName
   {
      get
      {
         return this.specificPlacaReportName;
      }
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      //PlacaDokumentFilter placaFilter = (PlacaDokumentFilter)vvRptFilter;
      VvRpt_Placa_Filter placaFilter = (VvRpt_Placa_Filter)vvRptFilter;
      if(this.Fld_TT == Placa.TT_PODUZETPLACA || this.Fld_TT == Placa.TT_REDOVANRAD)
       //return new VvPlacaReport(new Vektor.Reports.PIZ.CR_ObracunPlace  (), new ZXC.VvRptExternTblChooser_Placa(true , false, false, false, false), reportName, placaFilter);
       //return new VvPlacaReport(new Vektor.Reports.PIZ.CR_ObracunPlace_2(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, false, false), reportName, placaFilter);
         return new VvPlacaReport(new Vektor.Reports.PIZ.CR_ObracunPlace_2(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, true, true ), reportName, placaFilter);
     else if(this.Fld_TT == Placa.TT_NEOPOREZPRIM)
         return new VvPlacaReport(new Vektor.Reports.PIZ.CR_NeoporeziviPrim(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, false, false), reportName, placaFilter);
      else
         return new VvPlacaReport(new Vektor.Reports.PIZ.CR_ObracunDrDoh(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, false, false), reportName, placaFilter);

   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.ThePlacaDokumentFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.ThePlacaDokumentFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
      ThePlacaDokumentFilter.DokNumOd = placa_rec.DokNum;
   }

   #endregion PrintDocumentRecord

   public override string TabPageTitle1
   {
      get { return "Osnovno"; }
   }
   public override string TabPageTitle2
   {
      get { return "EvidRada"; }
   }
   public override string TabPageTitle3
   {
      get { return "Obustave"; }
   }

   public override VvLookUpLista TheTtLookUpList
   {
      get { return ZXC.luiListaPlacaTT; }
   }

   public override ZXC.DbNavigationRestrictor VvNavRestrictor_TT { get { return dbNavigationRestrictor; } }

   #endregion Overriders and specifics

   #region CreatePlacaDokumentPrintUC

   protected void CreatePlacaDokumentPrintUC(VvUserControl vvUC)
   {
      this.ThePlacaDokumentFilter = new PlacaDokumentFilter();

      ThePlacaDokumentFilterUC = new PlacaDokumentFilterUC(vvUC);
      ThePlacaDokumentFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = ThePlacaDokumentFilterUC.Width;

   }

   #endregion CreatePlacaDokumentPrintUC

}

public partial class PlacaDUC     : PlacaBaseDUC//VvPolyDocumRecordUC
{

   #region Constructor

   public PlacaDUC(Control parent, Placa _placa, VvForm.VvSubModul vvSubModul): base(parent, _placa, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
      (Placa.tt_colName, new string[] 
         { 
            Placa.TT_AUTORHONOR,
            Placa.TT_AUTORHONUMJ,
            Placa.TT_IDD_KOLONA_4,
            Placa.TT_NADZORODBOR,
            Placa.TT_PODUZETPLACA,
            Placa.TT_REDOVANRAD,
            Placa.TT_UGOVORODJELU
         });
   }

   #endregion Constructor

   #region TheGrid_Columns

   protected override void InitializeDUC_Specific_Columns_TheG()
   {
      int w;

      TheG.TheSumOfPreferredWidths = 0;
      TheG.TheSumOfPreferredWidths += TheG.RowHeadersWidth;

      /*07*/         w = ZXC.Q2un + ZXC.Qun2; T_personCD_CreateColumn(w);            TheG.TheSumOfPreferredWidths += w;
      /*08*/         w = ZXC.Q7un;            T_prezime_CreateColumn(w);             TheG.TheSumOfPreferredWidths += w; /*Fill*/// minWidth=ZXC.Q6un
      /*09*/         w = ZXC.Q5un;            T_ime_CreateColumn(w);                 TheG.TheSumOfPreferredWidths += w;
      /*  */         w = ZXC.Q7un;            R_prezimeIme_CreateColumn(w);          TheG.TheSumOfPreferredWidths += w;
                                                                                     
      /* NetoBlue */ w = ZXC.Q4un;            R_nettoBlue_CreateColumn(w, 2);        TheG.TheSumOfPreferredWidths += w;
                                             
      /* 37R */      w = ZXC.Q2un;            R_numOfLinesPtrane_CreateColumn(w, 0); TheG.TheSumOfPreferredWidths += w;
      /* 38R */      w = ZXC.Q2un;            R_numOfLinesPtrano_CreateColumn(w, 0); TheG.TheSumOfPreferredWidths += w;

      /*10*/         w = ZXC.Q4un;            T_brutoOsn_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /*11*/         w = ZXC.Q3un;            T_topObrok_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /*12*/         w = ZXC.Q2un + ZXC.Qun2; T_godStaza_CreateColumn(w, 0);         TheG.TheSumOfPreferredWidths += w;
      /*13*/         w = ZXC.Q4un;            T_dodBruto_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /* 01R */      w = ZXC.Q4un;            R_bruto100_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
                                              
      /* 30 */       w = ZXC.Q2un;            T_IsPoluSat_CreateColumn(w);           TheG.TheSumOfPreferredWidths += w;
                                                                                     
      /* 34R */      w = ZXC.Q3un;            R_satiRada_CreateColumn(w, 1);         TheG.TheSumOfPreferredWidths += w;
      /* 35R */      w = ZXC.Q3un;            R_satiBolovanja_CreateColumn(w, 1);    TheG.TheSumOfPreferredWidths += w;
      /* 36R */      w = ZXC.Q3un;            R_satiUkupno_CreateColumn(w, 1);       TheG.TheSumOfPreferredWidths += w;
      /* 39R */      w = ZXC.Q3un;            R_fondSatiDiff_CreateColumn(w, 1);     TheG.TheSumOfPreferredWidths += w;

      /* 02R */      w = ZXC.Q4un;            R_theBruto_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;

      /*15*/         w = ZXC.QUN + ZXC.Qun2;  T_spc_CreateColumn(w);                 TheG.TheSumOfPreferredWidths += w;
      /*20*/         w = ZXC.Q2un;            T_koefHRVI_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /*21*/         w = ZXC.QUN + ZXC.Qun2;  T_invalidTip_CreateColumn(w);          TheG.TheSumOfPreferredWidths += w;
                     
      /*14*/         w = ZXC.Q2un + ZXC.Qun2; T_isMioII_CreateColumn(w);             TheG.TheSumOfPreferredWidths += w;
      /* 03R */      w = ZXC.Q4un;            R_mioOsn_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 04R */      w = ZXC.Q4un;            R_mio1stup_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /* 05R */      w = ZXC.Q4un;            R_mio2stup_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /* 06R */      w = ZXC.Q4un;            R_mioAll_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 07R */      w = ZXC.Q4un;            R_doprIz_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;

      /* 34 */       w = ZXC.Q2un + ZXC.Qun4; T_rsB_CreateColumn(w);                 TheG.TheSumOfPreferredWidths += w;
      /* 40R */      w = ZXC.Q4un;            R_mio1stupNa_CreateColumn(w, 2);       TheG.TheSumOfPreferredWidths += w;
      /* 41R */      w = ZXC.Q4un;            R_mio2stupNa_CreateColumn(w, 2);       TheG.TheSumOfPreferredWidths += w;
      /* 42R */      w = ZXC.Q4un;            R_mioAllNa_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;

      /* 27R */      w = ZXC.Q4un;            R_zdrNa_CreateColumn(w, 2);            TheG.TheSumOfPreferredWidths += w;
      /* 28R */      w = ZXC.Q4un;            R_zorNa_CreateColumn(w, 2);            TheG.TheSumOfPreferredWidths += w;
      /* 29R */      w = ZXC.Q4un;            R_zapNa_CreateColumn(w, 2);            TheG.TheSumOfPreferredWidths += w;
      /* 30R */      w = ZXC.Q4un;            R_zapII_CreateColumn(w, 2);            TheG.TheSumOfPreferredWidths += w;
      /* 31R */      w = ZXC.Q4un;            R_zapAll_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 46R */      w = ZXC.Q2un;            R_daniZpi_CreateColumn(w, 0);          TheG.TheSumOfPreferredWidths += w;
      /* 45R */      w = ZXC.Q4un;            R_zpiUk_CreateColumn(w, 2);            TheG.TheSumOfPreferredWidths += w;

      /* 32R */      w = ZXC.Q4un;            R_doprNa_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 33R */      w = ZXC.Q4un;            R_doprAll_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;

      /*17*/         w = ZXC.Q4un;            T_zivotno_CreateColumn(w, 2, "Zivotno", "Životno osiguranje kao olakšica za porez i prirez na dohodak"             , true); TheG.TheSumOfPreferredWidths += w;
      /*18*/         w = ZXC.Q4un;            T_dopZdr_CreateColumn (w, 2, "DopZdr" , "Dopunsko zdravstveno osiguranje kao olakšica za porez i prirez na dohodak"      ); TheG.TheSumOfPreferredWidths += w;
      /*19*/         w = ZXC.Q4un;            T_dobMIO_CreateColumn (w, 2, "DobMIO" , "Dobrovoljno mirovinsko osiguranje kao olakšica za porez i prirez na dohodak"    ); TheG.TheSumOfPreferredWidths += w;
      /* 09R */      w = ZXC.Q4un;            R_premije_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;

      /* 10R */      w = ZXC.Q4un;            R_dohodak_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;

      /*16*/         w = ZXC.Q3un - ZXC.Qun4; T_koef_CreateColumn(w, 2);             TheG.TheSumOfPreferredWidths += w;
      /* 08R */      w = ZXC.Q4un;            R_odbitak_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;


      /* 12R */      w = ZXC.Q4un;            R_porOsn1_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;
      /* 13R */      w = ZXC.Q4un;            R_porOsn2_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;
      /* 14R */      w = ZXC.Q4un;            R_porOsn3_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;
      /* 15R */      w = ZXC.Q4un;            R_porOsn4_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;
      /* 11R */      w = ZXC.Q4un;            R_porOsnAll_CreateColumn(w, 2);        TheG.TheSumOfPreferredWidths += w;
                                              
      /* 16R */      w = ZXC.Q4un;            R_por1uk_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 17R */      w = ZXC.Q4un;            R_por2uk_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 18R */      w = ZXC.Q4un;            R_por3uk_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 19R */      w = ZXC.Q4un;            R_por4uk_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 20R */      w = ZXC.Q4un;            R_porezAll_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /*22*/         w = ZXC.Q3un - ZXC.Qun4; T_opcCD_CreateColumn(w);               TheG.TheSumOfPreferredWidths += w;
      /*23*/         w = ZXC.Q5un;            T_opcName_CreateColumn(w);             TheG.TheSumOfPreferredWidths += w;
      /*26*/         w = ZXC.Q3un - ZXC.Qun2; T_stPrirez_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /*24*/         w = ZXC.Q3un - ZXC.Qun4; T_opcRadCD_CreateColumn(w);            TheG.TheSumOfPreferredWidths += w;
      /*25*/         w = ZXC.Q5un;            T_opcRadName_CreateColumn(w);          TheG.TheSumOfPreferredWidths += w;
      /* 21R */      w = ZXC.Q4un;            R_prirez_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 22R */      w = ZXC.Q4un;            R_porPrirez_CreateColumn(w, 2);        TheG.TheSumOfPreferredWidths += w;

      /* 23R */      w = ZXC.Q4un;            R_netto_CreateColumn(w, 2);            TheG.TheSumOfPreferredWidths += w;
                                              
      /* 44R */      w = ZXC.Q5un;            R_krizPorOsn_CreateColumn(w, 2);       TheG.TheSumOfPreferredWidths += w;
      /* 43R */      w = ZXC.Q4un;            R_krizPorUk_CreateColumn(w, 2);        TheG.TheSumOfPreferredWidths += w;


      /*27*/         w = ZXC.Q4un;            T_netoAdd_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;
      /*28*/         w = ZXC.Q2un - ZXC.Qun4; T_isDirNeto_CreateColumn(w);           TheG.TheSumOfPreferredWidths += w;
      /*29*/         w = ZXC.Q3un + ZXC.Qun2; T_prijevoz_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;

      /* 24R */      w = ZXC.Q4un;            R_obustave_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /* 26R */      w = ZXC.Q4un;            R_naRuke_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;

      /* 25R */      w = ZXC.Q4un;            R_2Pay_CreateColumn(w, 2);             TheG.TheSumOfPreferredWidths += w;
                                                                                     
      /*ScrollCol*/  w = ZXC.QUN;             ColumnForScroll_CreateColumn(w);       TheG.TheSumOfPreferredWidths += w;


      //1111   hamp_zaglavlje.VvColWdt[14] = TheG.TheSumOfPreferredWidths - hamp_zaglavlje.Right + 4 * ZXC.QUN;// +ZXC.Q6un; // zbog poradi scrolla koji se pojavljije na datagridu
   }

   #endregion TheGrid_Columns

   #region TheGrid2_Columns

   protected override void InitializeDUC_Specific_Columns_TheG2()
   {
      int w;

      TheG2.TheSumOfPreferredWidths = 0;
      TheG2.TheSumOfPreferredWidths += TheG2.RowHeadersWidth;

      w = ZXC.Q3un; T_personCD_2_CreateColumn(w); TheG2.TheSumOfPreferredWidths += w;
      w = ZXC.Q8un; T_prezime_2_CreateColumn(w); TheG2.TheSumOfPreferredWidths += w;
      w = ZXC.Q6un; T_ime_2_CreateColumn(w); TheG2.TheSumOfPreferredWidths += w;

      /* 10 */      w = ZXC.Q2un + ZXC.Qun4;  T_vrstaRCd_CreateColumn(w);   TheG2.TheSumOfPreferredWidths += w;
      /* 11 */      w = ZXC.Q10un + ZXC.Q5un; T_vrstaRName_CreateColumn(w); TheG2.TheSumOfPreferredWidths += w;
      /* 12 */      w = ZXC.Q3un - ZXC.Qun8;  T_cijPerc_CreateColumn(w, 0); TheG2.TheSumOfPreferredWidths += w;
      /* 14 */      w = ZXC.Q3un - ZXC.Qun4;  T_rsOO_CreateColumn(w);       TheG2.TheSumOfPreferredWidths += w;
      /* 15 */      w = ZXC.Q3un - ZXC.Qun4;  T_rsOD_CreateColumn(w);       TheG2.TheSumOfPreferredWidths += w;
      /* 16 */      w = ZXC.Q3un - ZXC.Qun4;  T_rsDO_CreateColumn(w);       TheG2.TheSumOfPreferredWidths += w;
      /* 13 */      w = ZXC.Q3un - ZXC.Qun2;  T_sati_CreateColumn(w, 1);    TheG2.TheSumOfPreferredWidths += w;
   }

   #endregion TheGrid2_Columns

   #region TheGrid3_Columns

   protected override void InitializeDUC_Specific_Columns_TheG3()
   {
      int w;

      TheG3.TheSumOfPreferredWidths = 0;
      TheG3.TheSumOfPreferredWidths += TheG3.RowHeadersWidth;

      w = ZXC.Q3un; T_personCD_3_CreateColumn(w); TheG3.TheSumOfPreferredWidths += w;
      w = ZXC.Q8un; T_prezime_3_CreateColumn(w); TheG3.TheSumOfPreferredWidths += w;
      w = ZXC.Q6un; T_ime_3_CreateColumn(w); TheG3.TheSumOfPreferredWidths += w;

      /* 10 */       w = ZXC.Q5un; T_dateStart_CreateColumn(w) ; TheG3.TheSumOfPreferredWidths += w;
      /* 11 */       w = ZXC.Q2un; T_ukBrRata_CreateColumn(w)  ; TheG3.TheSumOfPreferredWidths += w;
      /* 12 */       w = ZXC.Q8un; T_opisOb_CreateColumn(w)    ; TheG3.TheSumOfPreferredWidths += w;
      /* 16 */       w = ZXC.Q2un; T_isZbirObus_CreateColumn(w); TheG3.TheSumOfPreferredWidths += w;
      /* 17 */       w = ZXC.Q7un; T_partija_CreateColumn(w)   ; TheG3.TheSumOfPreferredWidths += w;
      /* 13 */       w = ZXC.Q4un; T_kupdob_cd_CreateColumn(w) ; TheG3.TheSumOfPreferredWidths += w;
      /* 14 */       w = ZXC.Q4un; T_kupdob_tk_CreateColumn(w) ; TheG3.TheSumOfPreferredWidths += w;
      /* 15 */       w = ZXC.Q4un; T_iznosOb_CreateColumn(w, 2); TheG3.TheSumOfPreferredWidths += w;

   }

   #endregion TheGrid3_Columns

   public override List<VvPref.VVColChooserStates> TheColChooserStates
   {
      get
      {
         return TheVvTabPage.TheVvForm.VvPref.placaDUC.ColChooserStates;
      }
      set
      {
         TheVvTabPage.TheVvForm.VvPref.placaDUC.ColChooserStates = value;
      }
   }
}

public partial class Placa2014DUC : PlacaBaseDUC // placa od 2014 nadalje!!!
{

   #region Constructor

   public Placa2014DUC(Control parent, Placa _placa, VvForm.VvSubModul vvSubModul): base(parent, _placa, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
      (Placa.tt_colName, new string[] 
         { 
            Placa.TT_AUTORHONOR,
            Placa.TT_AUTORHONUMJ,
            Placa.TT_AHSAMOSTUMJ,
            Placa.TT_IDD_KOLONA_4,
            Placa.TT_NADZORODBOR,
            Placa.TT_TURSITVIJECE,
            Placa.TT_PODUZETPLACA,
            Placa.TT_REDOVANRAD,
            Placa.TT_UGOVORODJELU,
            Placa.TT_BIVSIRADNIK,
            Placa.TT_PLACAUNARAVI, 
            Placa.TT_SEZZAPPOLJOP, 
            Placa.TT_POREZNADOBIT, 
            Placa.TT_STRUCNOOSPOS, 
            Placa.TT_NEPLACDOPUST, 
            Placa.TT_SAMODOPRINOS,
            Placa.TT_DDBEZDOPRINO,
            Placa.TT_AUVECASTOPA ,
            Placa.TT_NR1_PX1NEDOP,
            Placa.TT_NR2_P01NEDOP,
            Placa.TT_NR3_PX1DADOP
         });
   }

   #endregion Constructor

   #region TheGrid_Columns

   protected override void InitializeDUC_Specific_Columns_TheG()
   {
      int w;

      TheG.TheSumOfPreferredWidths = 0;
      TheG.TheSumOfPreferredWidths += TheG.RowHeadersWidth;

      /*07*/         w = ZXC.Q2un + ZXC.Qun2; T_personCD_CreateColumn(w);            TheG.TheSumOfPreferredWidths += w;
      /*08*/         w = ZXC.Q7un;            T_prezime_CreateColumn(w);             TheG.TheSumOfPreferredWidths += w; /*Fill*/// minWidth=ZXC.Q6un
      /*09*/         w = ZXC.Q5un;            T_ime_CreateColumn(w);                 TheG.TheSumOfPreferredWidths += w;
      /*  */         w = ZXC.Q7un;            R_prezimeIme_CreateColumn(w);          TheG.TheSumOfPreferredWidths += w;
                                                                               
      /* NetoBlue */ w = ZXC.Q4un;            R_nettoBlue_CreateColumn(w, 2);        TheG.TheSumOfPreferredWidths += w;

      /*07*/
      w = ZXC.Q2un + ZXC.Qun4;/* 22.04.16.*/ T_zivotno_CreateColumn(w, 0, "RbrJop", "Redni broj redka izvornog JOPPD Obrasca za koji je potrebno napraviti 2-Ispravak ili 3-Dopunu", false); TheG.TheSumOfPreferredWidths += w;
      /* 37R */      w = ZXC.Q2un;            R_numOfLinesPtrane_CreateColumn(w, 0); TheG.TheSumOfPreferredWidths += w;
      /* 38R */      w = ZXC.Q2un;            R_numOfLinesPtrano_CreateColumn(w, 0); TheG.TheSumOfPreferredWidths += w;
  
      /*  */         w = ZXC.Q2un + ZXC.Qun8; T_koefBruto1_CreateColumn(w, 2);       TheG.TheSumOfPreferredWidths += w;
      /*  */         w = ZXC.Q2un + ZXC.Qun8; T_brutoDodSt2_CreateColumn(w, 2);      TheG.TheSumOfPreferredWidths += w;
      /*  */         w = ZXC.Q2un + ZXC.Qun8; T_brutoDodSt3_CreateColumn(w, 2);      TheG.TheSumOfPreferredWidths += w;
      /*  */         w = ZXC.Q2un;            T_dnFondSati_CreateColumn(w, 2/*0 do 05.04.2019.*/);       TheG.TheSumOfPreferredWidths += w;

      /*10*/         w = ZXC.Q4un;            T_brutoOsn_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /*11*/         w = ZXC.Q3un;            T_topObrok_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /*12*/         w = ZXC.Q2un + ZXC.Qun2; T_godStaza_CreateColumn(w, 0);         TheG.TheSumOfPreferredWidths += w;
      /*00*/         w = ZXC.Q3un;            T_brutoDodSt_CreateColumn(w, 2);       TheG.TheSumOfPreferredWidths += w;
      /*13*/         w = ZXC.Q4un;            T_dodBruto_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /*  */         w = ZXC.Q4un;            T_brDodpoloz_CreateColumn(w, 2);       TheG.TheSumOfPreferredWidths += w;

      /* 01R */      w = ZXC.Q4un;            R_bruto100_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
                                              
      /* 30 */       w = ZXC.Q3un;            T_IsPoluSat_CreateColumn(w);           TheG.TheSumOfPreferredWidths += w;
 
      /* 01R */      w = ZXC.Q4un;            T_pr3mjBruto_CreateColumn(w, 2);       TheG.TheSumOfPreferredWidths += w;

                                                                                    
      /* 34R */      w = ZXC.Q3un;            R_satiRada_CreateColumn(w, 1);         TheG.TheSumOfPreferredWidths += w;
      /* 35R */      w = ZXC.Q3un;            R_satiBolovanja_CreateColumn(w, 1);    TheG.TheSumOfPreferredWidths += w;
      /* 36R */      w = ZXC.Q3un;            R_satiUkupno_CreateColumn(w, 1);       TheG.TheSumOfPreferredWidths += w;
      /* 39R */      w = ZXC.Q3un;            R_fondSatiDiff_CreateColumn(w, 1);     TheG.TheSumOfPreferredWidths += w;
      /* 57R */      w = ZXC.Q3un;            R_satiNeRad_CreateColumn(w, 1);        TheG.TheSumOfPreferredWidths += w;

      /*11*/         w = ZXC.Q3un;            T_thisStazSt_CreateColumn(w, 2);       TheG.TheSumOfPreferredWidths += w;
      /*   */        w = ZXC.Q4un;            T_brutoKorekc_CreateColumn(w, 2);      TheG.TheSumOfPreferredWidths += w;
      /* 02R */      w = ZXC.Q4un;            R_theBruto_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;

      /*15*/         w = ZXC.QUN + ZXC.Qun2;  T_spc_CreateColumn(w);                 TheG.TheSumOfPreferredWidths += w;
      /*20*/         w = ZXC.Q2un;            T_koefHRVI_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /*21*/         w = ZXC.QUN + ZXC.Qun2;  T_invalidTip_CreateColumn(w);          TheG.TheSumOfPreferredWidths += w;
                     
      /*14*/         w = ZXC.Q2un + ZXC.Qun2; T_isMioII_CreateColumn(w);             TheG.TheSumOfPreferredWidths += w;

      /* 03R */      w = ZXC.Q4un;            T_fixMio1Olak_CreateColumn(w, 2);       TheG.TheSumOfPreferredWidths += w;
      /* 03R */      w = ZXC.Q4un;            R_mio1Olk_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 03R */      w = ZXC.Q4un;            R_mio1Osn_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;


      /* 03R */      w = ZXC.Q4un;            R_mioOsn_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 04R */      w = ZXC.Q4un;            R_mio1stup_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /* 05R */      w = ZXC.Q4un;            R_mio2stup_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /* 06R */      w = ZXC.Q4un;            R_mioAll_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 07R */      w = ZXC.Q4un;            R_doprIz_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;

      /* 34 */       w = ZXC.Q2un + ZXC.Qun4; T_rsB_CreateColumn(w);                 TheG.TheSumOfPreferredWidths += w;
      /* 40R */      w = ZXC.Q4un;            R_mio1stupNa_CreateColumn(w, 2);       TheG.TheSumOfPreferredWidths += w;
      /* 41R */      w = ZXC.Q4un;            R_mio2stupNa_CreateColumn(w, 2);       TheG.TheSumOfPreferredWidths += w;
      /* 42R */      w = ZXC.Q4un;            R_mioAllNa_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;

      /* 27R */      w = ZXC.Q4un;            R_zdrNa_CreateColumn(w, 2);            TheG.TheSumOfPreferredWidths += w;
      /* 28R */      w = ZXC.Q4un;            R_zorNa_CreateColumn(w, 2);            TheG.TheSumOfPreferredWidths += w;
      /* 29R */      w = ZXC.Q4un;            R_zapNa_CreateColumn(w, 2);            TheG.TheSumOfPreferredWidths += w;
      /* 30R */      w = ZXC.Q4un;            R_zapII_CreateColumn(w, 2);            TheG.TheSumOfPreferredWidths += w;
      /* 31R */      w = ZXC.Q4un;            R_zapAll_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 46R */      w = ZXC.Q2un;            R_daniZpi_CreateColumn(w, 0);          TheG.TheSumOfPreferredWidths += w;
      /* 45R */      w = ZXC.Q4un;            R_zpiUk_CreateColumn(w, 2);            TheG.TheSumOfPreferredWidths += w;

      /* 32R */      w = ZXC.Q4un;            R_doprNa_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 33R */      w = ZXC.Q4un;            R_doprAll_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;

      /* 10R */      w = ZXC.Q4un;            R_dohodak_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;

      // 26.08.2021: 
    //               w = ZXC.Q3un - ZXC.Qun4; T_koef_CreateColumn(w, 2);             TheG.TheSumOfPreferredWidths += w;
      /*16*/         w = ZXC.Q3un - ZXC.Qun4; T_koef_CreateColumn(w, 4);             TheG.TheSumOfPreferredWidths += w;
      /* 08R */      w = ZXC.Q4un;            R_odbitak_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;

      /* 12R */      w = ZXC.Q4un;            R_porOsn1_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;
      /* 13R */      w = ZXC.Q4un;            R_porOsn2_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;
      /* 14R */      w = ZXC.Q4un;            R_porOsn3_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;
      /* 15R */      w = ZXC.Q4un;            R_porOsn4_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;
      /* 11R */      w = ZXC.Q4un;            R_porOsnAll_CreateColumn(w, 2);        TheG.TheSumOfPreferredWidths += w;
      
      /* 11R */      w = ZXC.Q3un;            T_stPorez1_CreateColumn(w, 2);        TheG.TheSumOfPreferredWidths += w;
      /* 11R */      w = ZXC.Q3un;            T_stPorez2_CreateColumn(w, 2);        TheG.TheSumOfPreferredWidths += w;

      /* 16R */      w = ZXC.Q4un;            R_por1uk_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 17R */      w = ZXC.Q4un;            R_por2uk_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 18R */      w = ZXC.Q4un;            R_por3uk_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 19R */      w = ZXC.Q4un;            R_por4uk_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 20R */      w = ZXC.Q4un;            R_porezAll_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      
      /*22*/         w = ZXC.Q3un - ZXC.Qun4; T_opcCD_CreateColumn(w);               TheG.TheSumOfPreferredWidths += w;
      /*23*/         w = ZXC.Q5un;            T_opcName_CreateColumn(w);             TheG.TheSumOfPreferredWidths += w;
      /*26*/         w = ZXC.Q3un - ZXC.Qun2; T_stPrirez_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      
      /*24*/         w = ZXC.Q3un - ZXC.Qun4; T_opcRadCD_CreateColumn(w);            TheG.TheSumOfPreferredWidths += w;
      /*25*/         w = ZXC.Q5un;            T_opcRadName_CreateColumn(w);          TheG.TheSumOfPreferredWidths += w;
      /* 21R */      w = ZXC.Q4un;            R_prirez_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
      /* 22R */      w = ZXC.Q4un;            R_porPrirez_CreateColumn(w, 2);        TheG.TheSumOfPreferredWidths += w;

      /* 23R */      w = ZXC.Q4un;            R_netto_CreateColumn(w, 2);            TheG.TheSumOfPreferredWidths += w;
                                              
      /*27*/         w = ZXC.Q4un;            T_netoAdd_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;
      /*36*/         w = ZXC.Q3un - ZXC.Qun4; T_neoPrimCD_CreateColumn(w);           TheG.TheSumOfPreferredWidths += w;
                     w = ZXC.Q5un           ; R_neoPrimName_CreateColumn(w);         TheG.TheSumOfPreferredWidths += w;

      /*28*/         w = ZXC.Q2un - ZXC.Qun4; T_isDirNeto_CreateColumn(w);           TheG.TheSumOfPreferredWidths += w;
      /*29*/         w = ZXC.Q3un + ZXC.Qun2; T_prijevoz_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /*18*/         w = ZXC.Q4un;            T_dopZdr_CreateColumn     (w, 2, "NP63"     , "NP63 Neoporezive nagrade za radne rezultate i drugi oblici dodatnog nagrađivanja"            ); TheG.TheSumOfPreferredWidths += w;
      /*19*/         w = ZXC.Q4un;            T_dobMIO_CreateColumn     (w, 2, "NP tobrok", "NP65 Novčane paušalne nakande za podmirivanje troškova prehrane radnika do propisanog iznosa"); TheG.TheSumOfPreferredWidths += w;
      /*18*/         w = ZXC.Q4un;            T_dopZdr2020_CreateColumn (w, 2, "DodZdr"   , "NP71 Premije dopunskog i dodatnog zdravstvenog osiguranja do propisanog iznosa."             ); TheG.TheSumOfPreferredWidths += w;

      /* 24R */      w = ZXC.Q4un;            R_obustave_CreateColumn(w, 2);         TheG.TheSumOfPreferredWidths += w;
      /* 26R */      w = ZXC.Q4un;            R_naRuke_CreateColumn(w, 2);           TheG.TheSumOfPreferredWidths += w;
 
      /*35*/         w = ZXC.Q3un - ZXC.Qun4; T_nacIsplCD_CreateColumn(w);           TheG.TheSumOfPreferredWidths += w;
                     w = ZXC.Q5un           ; R_nacIsplName_CreateColumn(w);         TheG.TheSumOfPreferredWidths += w;

      /* 25R */      w = ZXC.Q4un;            R_2Pay_CreateColumn(w, 2);             TheG.TheSumOfPreferredWidths += w;
                                                                                     
      /*ScrollCol*/  w = ZXC.QUN;             ColumnForScroll_CreateColumn(w);       TheG.TheSumOfPreferredWidths += w;
   }

   #endregion TheGrid_Columns

   #region TheGrid2_Columns

   protected override void InitializeDUC_Specific_Columns_TheG2()
   {
      int w;

      TheG2.TheSumOfPreferredWidths = 0;
      TheG2.TheSumOfPreferredWidths += TheG2.RowHeadersWidth;

      w = ZXC.Q3un; T_personCD_2_CreateColumn(w); TheG2.TheSumOfPreferredWidths += w;
      w = ZXC.Q8un; T_prezime_2_CreateColumn(w) ; TheG2.TheSumOfPreferredWidths += w;
      w = ZXC.Q6un; T_ime_2_CreateColumn(w)     ; TheG2.TheSumOfPreferredWidths += w;

      /* 10 */      w = ZXC.Q2un  + ZXC.Qun4;  T_vrstaRCd_CreateColumn(w);      TheG2.TheSumOfPreferredWidths += w;
      /* 11 */      w = ZXC.Q10un + ZXC.Q5un;  T_vrstaRName_CreateColumn(w);    TheG2.TheSumOfPreferredWidths += w;
      /* 12 */      w = ZXC.Q3un  - ZXC.Qun8;  T_cijPerc_CreateColumn(w, 0);    TheG2.TheSumOfPreferredWidths += w;
      /* 14 */      w = ZXC.Q3un  - ZXC.Qun4;  T_rsOO_CreateColumn(w);          TheG2.TheSumOfPreferredWidths += w;
      /* 20 */      w = ZXC.Q2un            ;  T_ip1gr_CreateColumn(w);         TheG2.TheSumOfPreferredWidths += w;
      /* 20 */      w = ZXC.Q2un  + ZXC.Qun4;  T_rbrIsprJop_CreateColumn(w);    TheG2.TheSumOfPreferredWidths += w;
      /* 15 */      w = ZXC.Q3un  - ZXC.Qun4;  T_rsOD_CreateColumn(w);          TheG2.TheSumOfPreferredWidths += w;
      /* 16 */      w = ZXC.Q3un  - ZXC.Qun4;  T_rsDO_CreateColumn(w);          TheG2.TheSumOfPreferredWidths += w;
      /* 13 */      w = ZXC.Q3un  - ZXC.Qun2;  T_sati_CreateColumn(w, 1);       TheG2.TheSumOfPreferredWidths += w;
      /* 17 */      w = ZXC.Q3un  - ZXC.Qun2;  T_stjecatCD_CreateColumn(w);     TheG2.TheSumOfPreferredWidths += w;
                    w = ZXC.Q5un            ;  R_stjecatName_CreateColumn(w);   TheG2.TheSumOfPreferredWidths += w;
      /* 18 */      w = ZXC.Q3un  - ZXC.Qun2;  T_primDohCD_CreateColumn(w);     TheG2.TheSumOfPreferredWidths += w;
                    w = ZXC.Q5un            ;  R_primDohName_CreateColumn(w);   TheG2.TheSumOfPreferredWidths += w;
      /* 19 */      w = ZXC.Q3un  - ZXC.Qun2;  T_pocKrajCD_CreateColumn(w);     TheG2.TheSumOfPreferredWidths += w;
                    w = ZXC.Q5un            ;  R_pocKrajName_CreateColumn(w);   TheG2.TheSumOfPreferredWidths += w;

   }

   #endregion TheGrid2_Columns

   #region TheGrid3_Columns

   protected override void InitializeDUC_Specific_Columns_TheG3()
   {
      int w;

      TheG3.TheSumOfPreferredWidths = 0;
      TheG3.TheSumOfPreferredWidths += TheG3.RowHeadersWidth;

      w = ZXC.Q3un; T_personCD_3_CreateColumn(w); TheG3.TheSumOfPreferredWidths += w;
      w = ZXC.Q7un; T_prezime_3_CreateColumn(w); TheG3.TheSumOfPreferredWidths += w;
      w = ZXC.Q5un; T_ime_3_CreateColumn(w); TheG3.TheSumOfPreferredWidths += w;

      /* 10 */       w = ZXC.Q5un; T_dateStart_CreateColumn(w)   ; TheG3.TheSumOfPreferredWidths += w;
      /* 11 */       w = ZXC.Q3un; T_ukBrRata_CreateColumn(w)    ; TheG3.TheSumOfPreferredWidths += w;
      /* 12 */       w = ZXC.Q8un; T_opisOb_CreateColumn(w)      ; TheG3.TheSumOfPreferredWidths += w;
      /* 16 */       w = ZXC.Q2un; T_isZbirObus_CreateColumn(w)  ; TheG3.TheSumOfPreferredWidths += w;
      /* 17 */       w = ZXC.Q7un; T_partija_CreateColumn(w)     ; TheG3.TheSumOfPreferredWidths += w;
      /* 13 */       w = ZXC.Q3un; T_kupdob_cd_CreateColumn(w)   ; TheG3.TheSumOfPreferredWidths += w;
      /* 14 */       w = ZXC.Q3un; T_kupdob_tk_CreateColumn(w)   ; TheG3.TheSumOfPreferredWidths += w;
      /* 20 */       w = ZXC.Q2un; T_ptranoKind_CreateColumn(w)  ; TheG3.TheSumOfPreferredWidths += w;
      /* 00 */       w = ZXC.Q3un; T_izNetoaSt_CreateColumn(w, 2); TheG3.TheSumOfPreferredWidths += w;
      /* 15 */       w = ZXC.Q4un; T_iznosOb_CreateColumn(w, 2)  ; TheG3.TheSumOfPreferredWidths += w;
      /* 19 */       w = ZXC.Q3un; T_rbrRate_CreateColumn(w)     ; TheG3.TheSumOfPreferredWidths += w;

   }

   #endregion TheGrid3_Columns

   public override List<VvPref.VVColChooserStates> TheColChooserStates
   {
      get
      {
         return TheVvTabPage.TheVvForm.VvPref.placa2014DUC.ColChooserStates;
      }
      set
      {
         TheVvTabPage.TheVvForm.VvPref.placa2014DUC.ColChooserStates = value;
      }
   }
 
   public override void PutDefaultDUCfields()
   {
      Fld_VrstaJOPPD = "1";
   }
}

public partial class PlacaNPDUC   : PlacaBaseDUC
{

   #region Constructor

   public PlacaNPDUC(Control parent, Placa _placa, VvForm.VvSubModul vvSubModul): base(parent, _placa, vvSubModul)
   {

      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Placa.tt_colName, new string[] 
         { 
            Placa.TT_NEOPOREZPRIM
         });



      ThePolyGridTabControl.TabPages[TabPageTitle2].Visible = false;
      ThePolyGridTabControl.TabPages[TabPageTitle3].Visible = false;

      hamp_rules.Visible = hamp_Open.Visible = false;
   }

   #endregion Constructor

   #region TheGrid_Columns

   protected override void InitializeDUC_Specific_Columns_TheG()
   {
            int w;

            TheG.TheSumOfPreferredWidths = 0;
            TheG.TheSumOfPreferredWidths += TheG.RowHeadersWidth;

            /*07*/         w = ZXC.Q2un + ZXC.Qun2; T_personCD_CreateColumn(w);            TheG.TheSumOfPreferredWidths += w;
            /*08*/         w = ZXC.Q7un;            T_prezime_CreateColumn(w);             TheG.TheSumOfPreferredWidths += w; /*Fill*/// minWidth=ZXC.Q6un
            /*09*/         w = ZXC.Q5un;            T_ime_CreateColumn(w);                 TheG.TheSumOfPreferredWidths += w;
            /*  */         w = ZXC.Q7un;            R_prezimeIme_CreateColumn(w);          TheG.TheSumOfPreferredWidths += w;
           
            /*07*/         w = ZXC.Q2un + ZXC.Qun4;/* 22.04.16.*/ T_zivotno_CreateColumn(w, 0, "RbrJop", "Redni broj redka izvornog JOPPD Obrasca za koji je potrebno napraviti 2-Ispravak ili 3-Dopunu", false); TheG.TheSumOfPreferredWidths += w;

            /*22*/         w = ZXC.Q3un - ZXC.Qun4; T_opcCD_CreateColumn(w);               TheG.TheSumOfPreferredWidths += w;
            /*23*/         w = ZXC.Q5un;            T_opcName_CreateColumn(w);             TheG.TheSumOfPreferredWidths += w;
            /*24*/         w = ZXC.Q3un - ZXC.Qun4; T_opcRadCD_CreateColumn(w);            TheG.TheSumOfPreferredWidths += w;
            /*25*/         w = ZXC.Q5un;            T_opcRadName_CreateColumn(w);          TheG.TheSumOfPreferredWidths += w;

            /*36*/         w = ZXC.Q3un - ZXC.Qun4; T_neoPrimCD_CreateColumn(w);           TheG.TheSumOfPreferredWidths += w;
                           w = ZXC.Q6un           ; R_neoPrimName_CreateColumn(w);         TheG.TheSumOfPreferredWidths += w;

            /*27*/         w = ZXC.Q4un;            T_netoAdd_CreateColumn(w, 2);          TheG.TheSumOfPreferredWidths += w;
 
            /*37*/         w = ZXC.Q3un - ZXC.Qun4; T_dokumCD_CreateColumn(w);             TheG.TheSumOfPreferredWidths += w;
            /*35*/         w = ZXC.Q3un - ZXC.Qun4; T_nacIsplCD_CreateColumn(w);           TheG.TheSumOfPreferredWidths += w;
                           w = ZXC.Q5un           ; R_nacIsplName_CreateColumn(w);         TheG.TheSumOfPreferredWidths += w;

            /*ScrollCol*/  w = ZXC.QUN;             ColumnForScroll_CreateColumn(w);       TheG.TheSumOfPreferredWidths += w;

   }

   #endregion TheGrid_Columns

   #region overrideTabPageTitle

   public override string TabPageTitle2
   {
      get { return ""; }
   }
   public override string TabPageTitle3
   {
      get { return ""; }
   }

   #endregion overrideTabPageTitle

   public override List<VvPref.VVColChooserStates> TheColChooserStates
   {
      get
      {
         return TheVvTabPage.TheVvForm.VvPref.placaNPDUC.ColChooserStates;
      }
      set
      {
         TheVvTabPage.TheVvForm.VvPref.placaNPDUC.ColChooserStates = value;
      }
   }

   public override void PutDefaultDUCfields()
   {
      Fld_TT = dbNavigationRestrictor.RestrictedValues[0];
      tbx_tt.EndEdit(); // Da se digne event OnExitSetLookUp_DataTakers
      
      VvLookUpItem theLui = ZXC.luiListaPlacaTT.SingleOrDefault(lui => lui.Cd == Fld_TT);
      if(theLui != null) Fld_TipTranOpis = theLui.Name;

      Put_NewTT_Num(TheVvDao.GetNextTtNum(TheDbConnection, Fld_TT, ""));

      Fld_VrstaJOPPD =  "1";
      Fld_VrstaObr   = "01";
   }
}


public class Placa_AddPersonsDlg : VvDialog
{
   #region Fieldz

   private Button okButton, cancelButton;
   private int dlgWidth, dlgHeight;
   public PersonListUC vvRecListUC;

   #endregion Fieldz

   #region Constructor

   public Placa_AddPersonsDlg()
   {
      this.StartPosition = FormStartPosition.CenterScreen;

      AddRecListUC();

      dlgWidth = vvRecListUC.hampFilter.Width + ZXC.Q6un;
      dlgHeight = vvRecListUC.hampFilter.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      okButton.BringToFront();

      this.Text = "Ispuni tablicu djelatnicima i podacima sa prethodnog dokumenta";
   }

   private void AddRecListUC()
   {
      VvForm.VvSubModul vvSubModul = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsPER);
      VvDataRecord vvDataRecord = ZXC.TheVvForm.CreateTheVvDataRecord_SwitchSubModulEnum(vvSubModul);

      vvRecListUC = new PersonListUC(this, (Person)vvDataRecord, vvSubModul);

      vvRecListUC.TheGrid           .Visible =
      vvRecListUC.grBoxLimitiraj    .Visible =
      vvRecListUC.hampListaRastePada.Visible =
      vvRecListUC.hampSpecifikum    .Visible =
      vvRecListUC.hampIzlistaj      .Visible =
      vvRecListUC.hampOpenFilter    .Visible = false;

      if(vvRecListUC.hampOpenUtil != null ) // zato sto obicni korisnici nemaju util neo samo superuser 
      {
         vvRecListUC.hampOpenUtil.Visible =
         vvRecListUC.hampUtil.Visible = false;
      }
      vvRecListUC.hampFilter.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);

   }

   #endregion Constructor

   #region Event cancelButton

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Event cancelButton

}

public class Placa_CalcBrutoFromNetoDlg : VvDialog
{
   #region Fieldz

   private Button okButton, cancelButton;
   private int dlgWidth, dlgHeight;

   private VvHamper hamper;
   private VvTextBox tbx_personCD, tbx_prezime, tbx_neto, tbx_isAfterKrizPor;
   private VvCheckBox cbx_isAfterKrizPor;

   #endregion Fieldz

   #region Constructor

   public Placa_CalcBrutoFromNetoDlg(uint _personCD, string _prezimeIme)
   {
      this.StartPosition = FormStartPosition.CenterScreen;

      InitializeHamper(out hamper);

      dlgWidth = hamper.Width + ZXC.Q6un;
      dlgHeight = hamper.Height + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      okButton.BringToFront();

      this.Text = "Bruto iz neta";

      Fld_PersonCD = _personCD;
      Fld_PrezimeIme = _prezimeIme;
   }

   private void InitializeHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", this, false, ZXC.QUN, ZXC.QUN, 0);

      hamper.VvColWdt = new int[] { ZXC.Q4un, ZXC.Q4un, ZXC.QUN - ZXC.Qun4, ZXC.Q9un };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun2;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl0 = hamper.CreateVvLabel(0, 0, "Za radnika:", ContentAlignment.MiddleRight);
      tbx_personCD = hamper.CreateVvTextBox(1, 0, "tbx_personCD", "");
      tbx_prezime = hamper.CreateVvTextBox(2, 0, "tbx_prezime", "", 32, 1, 0);

      tbx_personCD.JAM_ReadOnly =
      tbx_prezime.JAM_ReadOnly = true;

      Label lbl1 = hamper.CreateVvLabel(0, 1, "Zadani neto:", ContentAlignment.MiddleRight);
      tbx_neto = hamper.CreateVvTextBox(1, 1, "tbx_neto", "", 12);
      tbx_neto.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_neto.JAM_DisableNegativeNumberValues = true;

      Label lbl2 = hamper.CreateVvLabel(3, 1, "Neto nakon posebnog poreza", ContentAlignment.MiddleLeft);
      tbx_isAfterKrizPor = hamper.CreateVvTextBox(2, 1, "tbx_isAfterKrizPor", "");
      cbx_isAfterKrizPor = hamper.CreateVvCheckBox(2, 1, "", tbx_isAfterKrizPor, "", "X");
      tbx_isAfterKrizPor.JAM_Highlighted = true;

      VvHamper.Open_Close_Fields_ForWriting(tbx_personCD, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_prezime, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_neto, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_isAfterKrizPor, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
   }

   #endregion Constructor

   #region Fld_

   public uint Fld_PersonCD
   {
      get { return ZXC.ValOrZero_UInt(tbx_personCD.Text); }
      set { tbx_personCD.Text = value.ToString("0000"); }
   }

   public string Fld_PrezimeIme
   {
      get { return tbx_prezime.Text; }
      set { tbx_prezime.Text = value; }
   }

   public decimal Fld_Neto
   {
      get { return tbx_neto.GetDecimalField(); }
      set { tbx_neto.PutDecimalField(value); }
   }

   public string Fld_IsAfterKrizPorTbx
   {
      get { return tbx_isAfterKrizPor.Text; }
      set { tbx_isAfterKrizPor.Text = value; }
   }

   public bool Fld_IsAfterKrizPor
   {
      get { return cbx_isAfterKrizPor.Checked; }
      set { cbx_isAfterKrizPor.Checked = value; }
   }

   #endregion Fld_

   #region Event cancelButton

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Event cancelButton

}

public class Placa2NalogRulesDlg : VvDialog 
{
   private Button   okButton, cancelButton;
   private int dlgWidth, dlgHeight;

   public Placa2NalogRulesUC TheUC { get; private set; }

   public Placa2NalogRulesDlg()
   {

      TheUC = new Placa2NalogRulesUC();

      SuspendLayout();

      this.Font        = ZXC.vvFont.BaseFont;
      this.Style       = ZXC.vvColors.vvform_VisualStyle;
      this.BackColor   = ZXC.vvColors.userControl_BackColor;

      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Shema Konta za knjiženje dohodaka";

      CreateTheUC();

      dlgWidth        = TheUC.Width;
      dlgHeight       = TheUC.Height + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);
      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      ResumeLayout();

   }

   private void CreateTheUC()
   {
      TheUC.Parent   = this;
      TheUC.Location = new Point(0, 0);
   }

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

}

public class Placa2NalogRulesUC : VvOtherUC 
{
   #region Fieldz

   private VvHamper hamp_ShemaRR, hamp_ShemaPP, hamp_ShemaAH, hamp_ShemaUD, hamp_shemaNO, hamp_shemaSN, hamp_shemaNE, hamp_shemaST, hamp_shemaSZ, hamp_shemaTV, hamp_shemaSD, hamp_CR_OP, hamp_ST_NoD;
   private VvTextBox tbx_Porez_D, tbx_Prirez_D, tbx_MIO_I_D, tbx_MIO_II_D, tbx_ZdrOsig_D, tbx_ZOR_D, tbx_ZPI_D, tbx_ZAP_D, tbx_ZPP_D, tbx_NETO_D, tbx_PREVOZ_D, tbx_MIO_1Na_D, tbx_MIO_2Na_D,
                     tbx_Porez_P, tbx_Prirez_P, tbx_MIO_I_P, tbx_MIO_II_P, tbx_ZdrOsig_P, tbx_ZOR_P, tbx_ZPI_P, tbx_ZAP_P, tbx_ZPP_P, tbx_NETO_P, tbx_PREVOZ_P, tbx_MIO_1Na_P, tbx_MIO_2Na_P,
                     tbx_PpPorez_D , tbx_PpPrirez_D, tbx_PpMIO_I_D, tbx_PpMIO_II_D, tbx_PpZdrOsig_D, tbx_PpZOR_D, tbx_PpZPI_D, tbx_PpZAP_D, tbx_PpZPP_D, tbx_PpNETO_D, tbx_PpPREVOZ_D, tbx_PpMIO_1Na_D, tbx_PpMIO_2Na_D,
                     tbx_PpPorez_P , tbx_PpPrirez_P, tbx_PpMIO_I_P, tbx_PpMIO_II_P, tbx_PpZdrOsig_P, tbx_PpZOR_P, tbx_PpZPI_P, tbx_PpZAP_P, tbx_PpZPP_P, tbx_PpNETO_P, tbx_PpPREVOZ_P, tbx_PpMIO_1Na_P, tbx_PpMIO_2Na_P,
                     tbx_AhPorez_D , tbx_AhPrirez_D, tbx_AhZPI_D, tbx_AhNETO_D, tbx_AhMIO_I_D, tbx_AhMIO_II_D, tbx_AhZdrOsig_D,
                     tbx_AhPorez_P , tbx_AhPrirez_P, tbx_AhZPI_P, tbx_AhNETO_P, tbx_AhMIO_I_P, tbx_AhMIO_II_P, tbx_AhZdrOsig_P,
                     tbx_UdPorez_D , tbx_UdPrirez_D, tbx_UdMIO_I_D, tbx_UdMIO_II_D, tbx_UdZdrOsig_D, tbx_UdZPI_D, tbx_UdNETO_D, 
                     tbx_UdPorez_P , tbx_UdPrirez_P, tbx_UdMIO_I_P, tbx_UdMIO_II_P, tbx_UdZdrOsig_P, tbx_UdZPI_P, tbx_UdNETO_P, 
                     tbx_NoPorez_D , tbx_NoPrirez_D, tbx_NoMIO_I_D, tbx_NoMIO_II_D, tbx_NoZdrOsig_D, tbx_NoZPI_D, tbx_NoNETO_D, 
                     tbx_NoPorez_P , tbx_NoPrirez_P, tbx_NoMIO_I_P, tbx_NoMIO_II_P, tbx_NoZdrOsig_P, tbx_NoZPI_P, tbx_NoNETO_P, 
                     tbx_SnPorez_D , tbx_SnPrirez_D, tbx_SnZPI_D, tbx_SnNETO_D,
                     tbx_SnPorez_P , tbx_SnPrirez_P, tbx_SnZPI_P, tbx_SnNETO_P,
                     tbx_NePorez_D , tbx_NePrirez_D, tbx_NeMIO_I_D, tbx_NeMIO_II_D, tbx_NeZdrOsig_D, tbx_NeNETO_D,
                     tbx_NePorez_P , tbx_NePrirez_P, tbx_NeMIO_I_P, tbx_NeMIO_II_P, tbx_NeZdrOsig_P, tbx_NeNETO_P,
                     tbx_StPorez_D , tbx_StPrirez_D, tbx_StMIO_oo_D, tbx_StZOs_zzr_D, tbx_StZPI_D, tbx_StNETO_D, 
                     tbx_StPorez_P , tbx_StPrirez_P, tbx_StMIO_oo_P, tbx_StZOs_zzr_P, tbx_StZPI_P, tbx_StNETO_P,
                     tbx_SzPorez_D , tbx_SzPrirez_D, tbx_SzNETO_D, 
                     tbx_SzPorez_P , tbx_SzPrirez_P, tbx_SzNETO_P,
                     tbx_TvPorez_D , tbx_TvPrirez_D, tbx_TvMIO_I_D, tbx_TvMIO_II_D, tbx_TvZdrOsig_D, tbx_TvZPI_D, tbx_TvNETO_D, 
                     tbx_TvPorez_P , tbx_TvPrirez_P, tbx_TvMIO_I_P, tbx_TvMIO_II_P, tbx_TvZdrOsig_P, tbx_TvZPI_P, tbx_TvNETO_P,
                     tbx_Sd_MIO_I_D, tbx_Sd_MIO_II_D, tbx_Sd_ZdrOsig_D, tbx_Sd_ZOR_D, tbx_Sd_ZAP_D,
                     tbx_Sd_MIO_I_P, tbx_Sd_MIO_II_P, tbx_Sd_ZdrOsig_P, tbx_Sd_ZOR_P, tbx_Sd_ZAP_P,
                     tbx_pozicijaRR, tbx_pozicijaAH, tbx_pozicijaUD, tbx_pozicijaNO, tbx_pozicijaSO, tbx_pozicijaTV,
                     tbx_KfBr1, tbx_StD1, tbx_StD2, tbx_DodBruto, tbx_PolDodBrt, tbx_StDodStz, tbx_VrKfBr1, tbx_BrutoKorekc,
                     tbx_NP63_nagr_D, tbx_NP65_preh_D, tbx_NP71_dZdr_D,
                     tbx_NP63_nagr_P, tbx_NP65_preh_P, tbx_NP71_dZdr_P,
                     tbx_NP17_dnev_D, tbx_NP17_dnev_P, 
                     tbx_NP18_loko_D, tbx_NP18_loko_P, 
                     tbx_NP21_darD_D, tbx_NP21_darD_P, 
                     tbx_NP22_regB_D, tbx_NP22_regB_P, 
                     tbx_NP26_otpr_D, tbx_NP26_otpr_P, 
                     tbx_NP60_jubi_D, tbx_NP60_jubi_P
                     ;

   private CheckBox cbx_visibleVrKoefBruto1, cxb_isObustOsnDohodak, cxb_isNPnaRR_Umjesecu;

   public KtoShemaPlacaDsc KSP { get; set; }

   private bool IsVisiblePozicija;

   private RadioButton rbt_stIzNeta, rbt_stIzDohotka;

   #endregion Fieldz

   #region Constructor

   public Placa2NalogRulesUC()
   {

      SuspendLayout();

      CreateShemaKontoHampers();

    //this.Size = new Size(hamp_ShemaRR.Right + ZXC.QunMrgn, hamp_CR_OP.Bottom);
      this.Size = new Size(hamp_ShemaRR.Right + ZXC.Q2un, hamp_shemaSD.Bottom);
      this.AutoScroll = true;

      if(ZXC.CURR_userName == ZXC.vvDB_systemSuperUserName || ZXC.CURR_userName == ZXC.vvDB_programSuperUserName || ZXC.CURR_user_rec.IsSuper)
               VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvOtherUC);
      else
               VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);

      KSP = new KtoShemaPlacaDsc(ZXC.dscLuiLst_KtoShemaPlaca);

      PutKtoShemaPlacaDscFields(KSP);

      ResumeLayout();
   }

   #endregion Constructor

   #region Hampers ShemaKonta
  
   private void CreateShemaKontoHampers()
   {

      IsVisiblePozicija = (ZXC.CURR_prjkt_rec.IsNeprofit || ZXC.CURR_prjkt_rec.PlanKind == ZXC.PlanKindEnum.PlnBy_FOND);

      InitializeHamper_RR(out hamp_ShemaRR);
      InitializeHamper_PP(out hamp_ShemaPP);
      InitializeHamper_UD(out hamp_ShemaUD);
      InitializeHamper_NO(out hamp_shemaNO);
      InitializeHamper_AH(out hamp_ShemaAH);
      InitializeHamper_SZ(out hamp_shemaSZ);
      InitializeHamper_SN(out hamp_shemaSN);
      InitializeHamper_NE(out hamp_shemaNE);
      InitializeHamper_SO(out hamp_shemaST);
      InitializeHamper_TV(out hamp_shemaTV);
      InitializeHamper_SD(out hamp_shemaSD);
      InitializeHamper_CR_OpisDodataka(out hamp_CR_OP);
      InitializeHamper_IsOsnovObDoh(out hamp_ST_NoD);
   }

   private void InitializeHamper_RR(out VvHamper hamper)
   {
    //hamper          = new VvHamper(15, 5, "", this, false);
      hamper          = new VvHamper(15, 8, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel( 0, 0, "REDOVAN RAD - RR", 4, 0, ContentAlignment.MiddleLeft);
      lbl.Font = ZXC.vvFont.SmallBoldFont;

      hamper.CreateVvLabel(0, 2, "Duguje:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 3, "Potražuje:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 6, "Duguje:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 7, "Potražuje:", ContentAlignment.MiddleRight);

      hamper.CreateVvLabel( 1, 1, "Porez"    , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 2, 1, "Prirez"   , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 3, 1, "MIO_I"    , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 4, 1, "MIO_II"   , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 5, 1, "ZdrOsig"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 6, 1, "ZOR"      , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 7, 1, "ZPI"      , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 8, 1, "ZAP"      , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 9, 1, "ZPP"      , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(10, 1, "NETO"     , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(11, 1, "PREVOZ"   , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(12, 1, "MIO_1Na"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(13, 1, "MIO_2Na"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 1, 5, "Np63_Nagr", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 2, 5, "Np65_Preh", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 3, 5, "Np71_DZdr", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 4, 5, "NP17_dnev", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 5, 5, "NP18_loko", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 6, 5, "NP21_darD", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 7, 5, "NP22_regB", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 8, 5, "NP26_otpr", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 9, 5, "NP60_jubi", ContentAlignment.MiddleLeft);


      tbx_Porez_D   = hamper.CreateVvTextBox( 1, 2, "tbx_Porez_D"  , "Konto Porez"                          , 8);
      tbx_Prirez_D  = hamper.CreateVvTextBox( 2, 2, "tbx_Prirez_D" , "Konto Prirez"                         , 8);
      tbx_MIO_I_D   = hamper.CreateVvTextBox( 3, 2, "tbx_MIO_I_D"  , "Konto MIO I"                          , 8);
      tbx_MIO_II_D  = hamper.CreateVvTextBox( 4, 2, "tbx_MIO_II_D" , "Konto MIO II"                         , 8);
      tbx_ZdrOsig_D = hamper.CreateVvTextBox( 5, 2, "tbx_ZdrOsig_D", "Konto Zdravstveno osiguranje"         , 8);
      tbx_ZOR_D     = hamper.CreateVvTextBox( 6, 2, "tbx_ZOR_D"    , "Konto ZOR"                            , 8);
      tbx_ZPI_D     = hamper.CreateVvTextBox( 7, 2, "tbx_ZPI_D"    , "Konto ZPI - sluzbeni put u inozemstvo", 8);
      tbx_ZAP_D     = hamper.CreateVvTextBox( 8, 2, "tbx_ZAP_D"    , "Konto Zaposljavanje"                  , 8);
      tbx_ZPP_D     = hamper.CreateVvTextBox( 9, 2, "tbx_ZPP_D"    , "Konto Zaposljavanje II"               , 8);
      tbx_NETO_D    = hamper.CreateVvTextBox(10, 2, "tbx_NETO_D"   , "Konto Neto"                           , 8);
      tbx_PREVOZ_D  = hamper.CreateVvTextBox(11, 2, "tbx_PREVOZ_D" , "Konto Prijevoz"                       , 8);
      tbx_MIO_1Na_D = hamper.CreateVvTextBox(12, 2, "tbx_MIO_1Na_D", "Konto MIO I NA"                       , 8);
      tbx_MIO_2Na_D = hamper.CreateVvTextBox(13, 2, "tbx_MIO_2Na_D", "Konto MIO II NA"                      , 8);

      tbx_NP63_nagr_D = hamper.CreateVvTextBox( 1, 6, "tbx_NP63_nagr_D", "Konto Neoporezivi primitak 63 nagrade"             , 8);
      tbx_NP65_preh_D = hamper.CreateVvTextBox( 2, 6, "tbx_NP65_preh_D", "Konto Neoporezivi primitak 65 prehrana"            , 8);
      tbx_NP71_dZdr_D = hamper.CreateVvTextBox( 3, 6, "tbx_NP71_dZdr_D", "Konto Neoporezivi primitak 71 dopunsko zdravstveno", 8);
      tbx_NP17_dnev_D = hamper.CreateVvTextBox( 4, 6, "tbx_NP17_dnev_D", "Konto Neoporezivi primitak 17 dnevnice"            , 8); 
      tbx_NP18_loko_D = hamper.CreateVvTextBox( 5, 6, "tbx_NP18_loko_D", "Konto Neoporezivi primitak 18 loko vožnja"         , 8); 
      tbx_NP21_darD_D = hamper.CreateVvTextBox( 6, 6, "tbx_NP21_darD_D", "Konto Neoporezivi primitak 21 dar djetetu"         , 8); 
      tbx_NP22_regB_D = hamper.CreateVvTextBox( 7, 6, "tbx_NP22_regB_D", "Konto Neoporezivi primitak 22 regres, božićnica"   , 8); 
      tbx_NP26_otpr_D = hamper.CreateVvTextBox( 8, 6, "tbx_NP26_otpr_D", "Konto Neoporezivi primitak 26 otpremnina"          , 8); 
      tbx_NP60_jubi_D = hamper.CreateVvTextBox( 9, 6, "tbx_NP60_jubi_D", "Konto Neoporezivi primitak 60 jubilarne nagrade"   , 8);

      tbx_Porez_P   = hamper.CreateVvTextBox( 1, 3, "tbx_Porez_P"  , "Konto Porez"                          , 8);
      tbx_Prirez_P  = hamper.CreateVvTextBox( 2, 3, "tbx_Prirez_P" , "Konto Prirez"                         , 8);
      tbx_MIO_I_P   = hamper.CreateVvTextBox( 3, 3, "tbx_MIO_I_P"  , "Konto MIO I"                          , 8);
      tbx_MIO_II_P  = hamper.CreateVvTextBox( 4, 3, "tbx_MIO_II_P" , "Konto MIO II"                         , 8);
      tbx_ZdrOsig_P = hamper.CreateVvTextBox( 5, 3, "tbx_ZdrOsig_P", "Konto Zdravstveno osiguranje"         , 8);
      tbx_ZOR_P     = hamper.CreateVvTextBox( 6, 3, "tbx_ZOR_P"    , "Konto ZOR"                            , 8);
      tbx_ZPI_P     = hamper.CreateVvTextBox( 7, 3, "tbx_ZPI_P"    , "Konto ZPI - sluzbeni put u inozemstvo", 8);
      tbx_ZAP_P     = hamper.CreateVvTextBox( 8, 3, "tbx_ZAP_P"    , "Konto Zaposljavanje"                  , 8);
      tbx_ZPP_P     = hamper.CreateVvTextBox( 9, 3, "tbx_ZPP_P"    , "Konto Zaposljavanje II"               , 8);
      tbx_NETO_P    = hamper.CreateVvTextBox(10, 3, "tbx_NETO_P"   , "Konto Neto"                           , 8);
      tbx_PREVOZ_P  = hamper.CreateVvTextBox(11, 3, "tbx_PREVOZ_P" , "Konto Prijevoz"                       , 8);
      tbx_MIO_1Na_P = hamper.CreateVvTextBox(12, 3, "tbx_MIO_1Na_P", "Konto MIO I NA"                       , 8);
      tbx_MIO_2Na_P = hamper.CreateVvTextBox(13, 3, "tbx_MIO_2Na_P", "Konto MIO II NA"                      , 8);

      tbx_NP63_nagr_P = hamper.CreateVvTextBox(1, 7, "tbx_NP63_nagr_P", "Konto Neoporezivi primitak 63 nagrade"             , 8);
      tbx_NP65_preh_P = hamper.CreateVvTextBox(2, 7, "tbx_NP65_preh_P", "Konto Neoporezivi primitak 65 prehrana"            , 8);
      tbx_NP71_dZdr_P = hamper.CreateVvTextBox(3, 7, "tbx_NP71_dZdr_P", "Konto Neoporezivi primitak 71 dopunsko zdravstveno", 8);
      tbx_NP17_dnev_P = hamper.CreateVvTextBox(4, 7, "tbx_NP17_dnev_p", "Konto Neoporezivi primitak 17 dnevnice"            , 8);
      tbx_NP18_loko_P = hamper.CreateVvTextBox(5, 7, "tbx_NP18_loko_p", "Konto Neoporezivi primitak 18 loko vožnja"         , 8);
      tbx_NP21_darD_P = hamper.CreateVvTextBox(6, 7, "tbx_NP21_darD_p", "Konto Neoporezivi primitak 21 dar djetetu"         , 8);
      tbx_NP22_regB_P = hamper.CreateVvTextBox(7, 7, "tbx_NP22_regB_p", "Konto Neoporezivi primitak 22 regres, božićnica"   , 8);
      tbx_NP26_otpr_P = hamper.CreateVvTextBox(8, 7, "tbx_NP26_otpr_p", "Konto Neoporezivi primitak 26 otpremnina"          , 8);
      tbx_NP60_jubi_P = hamper.CreateVvTextBox(9, 7, "tbx_NP60_jubi_p", "Konto Neoporezivi primitak 60 jubilarne nagrade"   , 8);

      tbx_Porez_D    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_Prirez_D   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_MIO_I_D    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_MIO_II_D   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_ZdrOsig_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_ZOR_D      .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_ZPI_D      .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_ZAP_D      .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_ZPP_D      .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NETO_D     .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PREVOZ_D   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_MIO_1Na_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_MIO_2Na_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_Porez_P    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_Prirez_P   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_MIO_I_P    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_MIO_II_P   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_ZdrOsig_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_ZOR_P      .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_ZPI_P      .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_ZAP_P      .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_ZPP_P      .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NETO_P     .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PREVOZ_P   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_MIO_1Na_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_MIO_2Na_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP63_nagr_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP65_preh_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP71_dZdr_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP63_nagr_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP65_preh_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP71_dZdr_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP17_dnev_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP18_loko_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP21_darD_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP22_regB_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP26_otpr_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP60_jubi_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP17_dnev_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP18_loko_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP21_darD_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP22_regB_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP26_otpr_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NP60_jubi_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;



      tbx_Porez_D    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_Prirez_D   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_MIO_I_D    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_MIO_II_D   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_ZdrOsig_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_ZOR_D      .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_ZPI_D      .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_ZAP_D      .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_ZPP_D      .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NETO_D     .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PREVOZ_D   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_MIO_1Na_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_MIO_2Na_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_Porez_P    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_Prirez_P   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_MIO_I_P    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_MIO_II_P   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_ZdrOsig_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_ZOR_P      .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_ZPI_P      .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_ZAP_P      .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_ZPP_P      .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NETO_P     .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PREVOZ_P   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_MIO_1Na_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_MIO_2Na_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP63_nagr_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP65_preh_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP71_dZdr_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP63_nagr_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP65_preh_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP71_dZdr_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP17_dnev_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP18_loko_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP21_darD_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP22_regB_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP26_otpr_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP60_jubi_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP17_dnev_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP18_loko_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP21_darD_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP22_regB_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP26_otpr_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NP60_jubi_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);


      //VvLookUpLista.LoadPozicijePlana_PLN_or_RLZ(/* isPLN */ false);
      Label lblP         = hamper.CreateVvLabel        (0, 4, "Pozicija:"      , ContentAlignment.MiddleRight);
      tbx_pozicijaRR     = hamper.CreateVvTextBoxLookUp(1, 4, "tbx_pozicijaRR", "Pozicija", 32);
      tbx_pozicijaRR.JAM_Set_LookUpTable(ZXC.luiListaPozicijePlanaRLZ, (int)ZXC.Kolona.prva);
      
      tbx_pozicijaRR.Visible = 
      lblP          .Visible = IsVisiblePozicija;
   }

   private void InitializeHamper_PP(out VvHamper hamper)
   {
      hamper = new VvHamper(15, 4, "", this, false);
     
      hamper.Location = new Point(ZXC.QunMrgn, hamp_ShemaRR.Bottom + ZXC.Qun2);
      //                                     0               1                   2                    3                    4                      5                 6              7               8              9                    10                    11          
      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "PODUZETNIČKA PLAĆA - PP", 4, 0, ContentAlignment.MiddleLeft);
      lbl.Font = ZXC.vvFont.SmallBoldFont;
      
      hamper.CreateVvLabel(0, 2, "Duguje:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 3, "Potražuje:", ContentAlignment.MiddleRight);

      hamper.CreateVvLabel( 1, 1, "Porez"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 2, 1, "Prirez" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 3, 1, "MIO_I"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 4, 1, "MIO_II" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 5, 1, "ZdrOsig", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 6, 1, "ZOR"    , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 7, 1, "ZPI"    , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 8, 1, "ZAP"    , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 9, 1, "ZPP"    , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(10, 1, "NETO"   , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(11, 1, "PREVOZ" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(12, 1, "MIO_1Na", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(13, 1, "MIO_2Na", ContentAlignment.MiddleLeft);


      tbx_PpPorez_D   = hamper.CreateVvTextBox( 1, 2, "tbx_PpPorez_D"  , "Konto Porez"                          , 8);
      tbx_PpPrirez_D  = hamper.CreateVvTextBox( 2, 2, "tbx_PpPrirez_D" , "Konto Prirez"                         , 8);
      tbx_PpMIO_I_D   = hamper.CreateVvTextBox( 3, 2, "tbx_PpMIO_I_D"  , "Konto MIO I"                          , 8);
      tbx_PpMIO_II_D  = hamper.CreateVvTextBox( 4, 2, "tbx_PpMIO_II_D" , "Konto MIO II"                         , 8);
      tbx_PpZdrOsig_D = hamper.CreateVvTextBox( 5, 2, "tbx_PpZdrOsig_D", "Konto Zdravstveno osiguranje"         , 8);
      tbx_PpZOR_D     = hamper.CreateVvTextBox( 6, 2, "tbx_PpZOR_D"    , "Konto ZOR"                            , 8);
      tbx_PpZPI_D     = hamper.CreateVvTextBox( 7, 2, "tbx_PpZPI_D"    , "Konto ZPI - sluzbeni put u inozemstvo", 8);
      tbx_PpZAP_D     = hamper.CreateVvTextBox( 8, 2, "tbx_PpZAP_D"    , "Konto Zaposljavanje"                  , 8);
      tbx_PpZPP_D     = hamper.CreateVvTextBox( 9, 2, "tbx_PpZPP_D"    , "Konto Zaposljavanje II"               , 8);
      tbx_PpNETO_D    = hamper.CreateVvTextBox(10, 2, "tbx_PpNETO_D"   , "Konto Neto"                           , 8);
      tbx_PpPREVOZ_D  = hamper.CreateVvTextBox(11, 2, "tbx_PpPREVOZ_D" , "Konto Prijevoz"                       , 8);
      tbx_PpMIO_1Na_D = hamper.CreateVvTextBox(12, 2, "tbx_PpMIO_1Na_D", "Konto MIO I NA"                       , 8);
      tbx_PpMIO_2Na_D = hamper.CreateVvTextBox(13, 2, "tbx_PpMIO_2Na_D", "Konto MIO II NA"                      , 8);
      
      tbx_PpPorez_P   = hamper.CreateVvTextBox( 1, 3, "tbx_PpPorez_P"  , "Konto Porez"                          , 8);
      tbx_PpPrirez_P  = hamper.CreateVvTextBox( 2, 3, "tbx_PpPrirez_P" , "Konto Prirez"                         , 8);
      tbx_PpMIO_I_P   = hamper.CreateVvTextBox( 3, 3, "tbx_PpMIO_I_P"  , "Konto MIO I"                          , 8);
      tbx_PpMIO_II_P  = hamper.CreateVvTextBox( 4, 3, "tbx_PpMIO_II_P" , "Konto MIO II"                         , 8);
      tbx_PpZdrOsig_P = hamper.CreateVvTextBox( 5, 3, "tbx_PpZdrOsig_P", "Konto Zdravstveno osiguranje"         , 8);
      tbx_PpZOR_P     = hamper.CreateVvTextBox( 6, 3, "tbx_PpZOR_P"    , "Konto ZOR"                            , 8);
      tbx_PpZPI_P     = hamper.CreateVvTextBox( 7, 3, "tbx_PpZPI_P"    , "Konto ZPI - sluzbeni put u inozemstvo", 8);
      tbx_PpZAP_P     = hamper.CreateVvTextBox( 8, 3, "tbx_PpZAP_P"    , "Konto Zaposljavanje"                  , 8);
      tbx_PpZPP_P     = hamper.CreateVvTextBox( 9, 3, "tbx_PpZPP_P"    , "Konto Zaposljavanje II"               , 8);
      tbx_PpNETO_P    = hamper.CreateVvTextBox(10, 3, "tbx_PpNETO_P"   , "Konto Neto"                           , 8);
      tbx_PpPREVOZ_P  = hamper.CreateVvTextBox(11, 3, "tbx_PpPREVOZ_P" , "Konto Prijevoz"                       , 8);
      tbx_PpMIO_1Na_P = hamper.CreateVvTextBox(12, 3, "tbx_PpMIO_1Na_P", "Konto MIO I NA"                       , 8);
      tbx_PpMIO_2Na_P = hamper.CreateVvTextBox(13, 3, "tbx_PpMIO_2Na_P", "Konto MIO II NA"                      , 8);

      tbx_PpPorez_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_PpPrirez_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_PpMIO_I_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpMIO_II_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpZdrOsig_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpZOR_D    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpZPI_D    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpZAP_D    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpZPP_D    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpNETO_D   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpPREVOZ_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_PpMIO_1Na_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_PpMIO_2Na_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpPorez_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_PpPrirez_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_PpMIO_I_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpMIO_II_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpZdrOsig_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpZOR_P    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpZPI_P    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpZAP_P    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpZPP_P    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpNETO_P   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpPREVOZ_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_PpMIO_1Na_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_PpMIO_2Na_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 

      
      tbx_PpPorez_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpPrirez_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpMIO_I_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpMIO_II_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpZdrOsig_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpZOR_D    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpZPI_D    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpZAP_D    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpZPP_D    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpNETO_D   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpPREVOZ_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpMIO_1Na_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpMIO_2Na_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpPorez_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpPrirez_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpMIO_I_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpMIO_II_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpZdrOsig_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpZOR_P    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpZPI_P    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpZAP_P    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpZPP_P    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpNETO_P   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpPREVOZ_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpMIO_1Na_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpMIO_2Na_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);

   }

   private void InitializeHamper_UD(out VvHamper hamper)
   {
      hamper = new VvHamper(8, 5, "", this, false);
     
      hamper.Location = new Point(ZXC.QunMrgn, hamp_ShemaPP.Bottom + ZXC.Qun2);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "UGOVOR O DJELU - UD", 4, 0, ContentAlignment.MiddleLeft);
      lbl.Font = ZXC.vvFont.SmallBoldFont;

      hamper.CreateVvLabel(0, 2, "Duguje:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 3, "Potražuje:", ContentAlignment.MiddleRight);

      hamper.CreateVvLabel(1, 1, "Porez"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(2, 1, "Prirez" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(3, 1, "MIO_I"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(4, 1, "MIO_II" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(5, 1, "ZdrOsig", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(6, 1, "ZPI"    , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(7, 1, "NETO"   , ContentAlignment.MiddleLeft);


      tbx_UdPorez_D   = hamper.CreateVvTextBox(1, 2, "tbx_UdPorez_D"  , "Konto Porez"                          , 8);
      tbx_UdPrirez_D  = hamper.CreateVvTextBox(2, 2, "tbx_UdPrirez_D" , "Konto Prirez"                         , 8);
      tbx_UdMIO_I_D   = hamper.CreateVvTextBox(3, 2, "tbx_UdMIO_I_D"  , "Konto MIO I"                          , 8);
      tbx_UdMIO_II_D  = hamper.CreateVvTextBox(4, 2, "tbx_UdMIO_II_D" , "Konto MIO II"                         , 8);
      tbx_UdZdrOsig_D = hamper.CreateVvTextBox(5, 2, "tbx_UdZdrOsig_D", "Konto Zdravstveno osiguranje"         , 8);
      tbx_UdZPI_D     = hamper.CreateVvTextBox(6, 2, "tbx_UdZPI_D"    , "Konto ZPI - sluzbeni put u inozemstvo", 8);
      tbx_UdNETO_D    = hamper.CreateVvTextBox(7, 2, "tbx_UdNETO_D"   , "Konto Neto"                           , 8);
      
      tbx_UdPorez_P   = hamper.CreateVvTextBox(1, 3, "tbx_UdPorez_P"  , "Konto Porez"                          , 8);
      tbx_UdPrirez_P  = hamper.CreateVvTextBox(2, 3, "tbx_UdPrirez_P" , "Konto Prirez"                         , 8);
      tbx_UdMIO_I_P   = hamper.CreateVvTextBox(3, 3, "tbx_UdMIO_I_P"  , "Konto MIO I"                          , 8);
      tbx_UdMIO_II_P  = hamper.CreateVvTextBox(4, 3, "tbx_UdMIO_II_P" , "Konto MIO II"                         , 8);
      tbx_UdZdrOsig_P = hamper.CreateVvTextBox(5, 3, "tbx_UdZdrOsig_P", "Konto Zdravstveno osiguranje"         , 8);
      tbx_UdZPI_P     = hamper.CreateVvTextBox(6, 3, "tbx_UdZPI_P"    , "Konto ZPI - sluzbeni put u inozemstvo", 8);
      tbx_UdNETO_P    = hamper.CreateVvTextBox(7, 3, "tbx_UdNETO_P"   , "Konto Neto"                           , 8);

      tbx_UdPorez_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_UdPrirez_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_UdMIO_I_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_UdMIO_II_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_UdZdrOsig_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_UdZPI_D    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_UdNETO_D   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_UdPorez_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_UdPrirez_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_UdMIO_I_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_UdMIO_II_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_UdZdrOsig_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_UdZPI_P    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_UdNETO_P   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      
      tbx_UdPorez_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_UdPrirez_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_UdMIO_I_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_UdMIO_II_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_UdZdrOsig_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_UdZPI_D    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_UdNETO_D   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_UdPorez_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_UdPrirez_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_UdMIO_I_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_UdMIO_II_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_UdZdrOsig_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_UdZPI_P    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_UdNETO_P   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);

      Label lblP         = hamper.CreateVvLabel        (0, 4, "Pozicija:"      , ContentAlignment.MiddleRight);
      tbx_pozicijaUD     = hamper.CreateVvTextBoxLookUp(1, 4, "tbx_pozicijaUD", "Pozicija", 32);
      tbx_pozicijaUD.JAM_Set_LookUpTable(ZXC.luiListaPozicijePlanaRLZ, (int)ZXC.Kolona.prva);
      tbx_pozicijaUD.Visible = 
      lblP          .Visible = IsVisiblePozicija;

   }
   
   private void InitializeHamper_NO(out VvHamper hamper)
   {
      hamper = new VvHamper(8, 5, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, hamp_ShemaUD.Bottom + ZXC.Qun2);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "NADZORNI ODBOR - NO", 2, 0, ContentAlignment.MiddleLeft);
      lbl.Font = ZXC.vvFont.SmallBoldFont;

      hamper.CreateVvLabel(0, 2, "Duguje:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 3, "Potražuje:", ContentAlignment.MiddleRight);

      hamper.CreateVvLabel(1, 1, "Porez"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(2, 1, "Prirez" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(3, 1, "MIO_I"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(4, 1, "MIO_II" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(5, 1, "ZdrOsig", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(6, 1, "ZPI"    , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(7, 1, "NETO"   , ContentAlignment.MiddleLeft);

      tbx_NoPorez_D   = hamper.CreateVvTextBox(1, 2, "tbx_NoPorez_D"  , "Konto Porez"                          , 8);
      tbx_NoPrirez_D  = hamper.CreateVvTextBox(2, 2, "tbx_NoPrirez_D" , "Konto Prirez"                         , 8);
      tbx_NoMIO_I_D   = hamper.CreateVvTextBox(3, 2, "tbx_NoMIO_I_D"  , "Konto MIO I"                          , 8);
      tbx_NoMIO_II_D  = hamper.CreateVvTextBox(4, 2, "tbx_NoMIO_II_D" , "Konto MIO II"                         , 8);
      tbx_NoZdrOsig_D = hamper.CreateVvTextBox(5, 2, "tbx_NoZdrOsig_D", "Konto Zdravstveno osiguranje"         , 8);
      tbx_NoZPI_D     = hamper.CreateVvTextBox(6, 2, "tbx_NoZPI_D"    , "Konto ZPI - sluzbeni put u inozemstvo", 8);
      tbx_NoNETO_D    = hamper.CreateVvTextBox(7, 2, "tbx_NoNETO_D"   , "Konto Neto"                           , 8);
      
      tbx_NoPorez_P   = hamper.CreateVvTextBox(1, 3, "tbx_NoPorez_P"  , "Konto Porez"                          , 8);
      tbx_NoPrirez_P  = hamper.CreateVvTextBox(2, 3, "tbx_NoPrirez_P" , "Konto Prirez"                         , 8);
      tbx_NoMIO_I_P   = hamper.CreateVvTextBox(3, 3, "tbx_NoMIO_I_P"  , "Konto MIO I"                          , 8);
      tbx_NoMIO_II_P  = hamper.CreateVvTextBox(4, 3, "tbx_NoMIO_II_P" , "Konto MIO II"                         , 8);
      tbx_NoZdrOsig_P = hamper.CreateVvTextBox(5, 3, "tbx_NoZdrOsig_P", "Konto Zdravstveno osiguranje"         , 8);
      tbx_NoZPI_P     = hamper.CreateVvTextBox(6, 3, "tbx_NoZPI_P"    , "Konto ZPI - sluzbeni put u inozemstvo", 8);
      tbx_NoNETO_P    = hamper.CreateVvTextBox(7, 3, "tbx_NoNETO_P"   , "Konto Neto"                           , 8);

      tbx_NoPorez_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NoPrirez_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NoMIO_I_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NoMIO_II_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NoZdrOsig_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NoZPI_D    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NoNETO_D   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NoPorez_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NoPrirez_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NoMIO_I_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NoMIO_II_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NoZdrOsig_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NoZPI_P    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NoNETO_P   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      
      tbx_NoPorez_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NoPrirez_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NoMIO_I_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NoMIO_II_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NoZdrOsig_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NoZPI_D    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NoNETO_D   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NoPorez_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NoPrirez_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NoMIO_I_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NoMIO_II_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NoZdrOsig_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NoZPI_P    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NoNETO_P   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);

      Label lblP         = hamper.CreateVvLabel        (0, 4, "Pozicija:"      , ContentAlignment.MiddleRight);
      tbx_pozicijaNO     = hamper.CreateVvTextBoxLookUp(1, 4, "tbx_pozicijaNO", "Pozicija", 32);
      tbx_pozicijaNO.JAM_Set_LookUpTable(ZXC.luiListaPozicijePlanaRLZ, (int)ZXC.Kolona.prva);
      tbx_pozicijaNO.Visible = 
      lblP          .Visible = IsVisiblePozicija;

   }

   private void InitializeHamper_AH(out VvHamper hamper)
   {
      hamper = new VvHamper(8, 5, "", this, false);

      hamper.Location = new Point(hamp_ShemaUD.Right + ZXC.QUN, hamp_ShemaUD.Top);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "AUTORSKI HONORAR - AH", 4, 0, ContentAlignment.MiddleLeft);
      lbl.Font = ZXC.vvFont.SmallBoldFont;

      hamper.CreateVvLabel(0, 2, "Duguje:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 3, "Potražuje:", ContentAlignment.MiddleRight);

      hamper.CreateVvLabel(1, 1, "MIO I"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(2, 1, "MIO II" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(3, 1, "ZdrOsig", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(4, 1, "Porez"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(5, 1, "Prirez" , ContentAlignment.MiddleLeft);
    //hamper.CreateVvLabel(6, 1, "ZPI"    , ContentAlignment.MiddleLeft); 23.01.2014. nemam pojma kaj to tu uopce je trebalo
      hamper.CreateVvLabel(6, 1, "NETO"   , ContentAlignment.MiddleLeft);
      
      tbx_AhMIO_I_D   = hamper.CreateVvTextBox(1, 2, "tbx_AhMIOI__D"  , "Konto MIO I"                          , 8);
      tbx_AhMIO_II_D  = hamper.CreateVvTextBox(2, 2, "tbx_AhMIOII_D"  , "Konto MIO II"                         , 8);
      tbx_AhZdrOsig_D = hamper.CreateVvTextBox(3, 2, "tbx_AhZdrO_D"   , "Konto Zdr"                            , 8);
      tbx_AhPorez_D   = hamper.CreateVvTextBox(4, 2, "tbx_AhPorez_D"  , "Konto Porez"                          , 8);
      tbx_AhPrirez_D  = hamper.CreateVvTextBox(5, 2, "tbx_AhPrirez_D" , "Konto Prirez"                         , 8);
      tbx_AhZPI_D     = hamper.CreateVvTextBox(6, 2, "tbx_AhZPI_D"    , "Konto ZPI - sluzbeni put u inozemstvo", 8);
      tbx_AhNETO_D    = hamper.CreateVvTextBox(6, 2, "tbx_AhNETO_D"   , "Konto Neto"                           , 8);

      tbx_AhMIO_I_P   = hamper.CreateVvTextBox(1, 3, "tbx_AhMIOI__D"  , "Konto MIO I"                          , 8);
      tbx_AhMIO_II_P  = hamper.CreateVvTextBox(2, 3, "tbx_AhMIOII_D"  , "Konto MIO II"                         , 8);
      tbx_AhZdrOsig_P = hamper.CreateVvTextBox(3, 3, "tbx_AhZdrO_D"   , "Konto Zdr"                            , 8);
      tbx_AhPorez_P   = hamper.CreateVvTextBox(4, 3, "tbx_AhPorez_P"  , "Konto Porez"                          , 8);
      tbx_AhPrirez_P  = hamper.CreateVvTextBox(5, 3, "tbx_AhPrirez_P" , "Konto Prirez"                         , 8);
      tbx_AhZPI_P     = hamper.CreateVvTextBox(6, 3, "tbx_AhZPI_P"    , "Konto ZPI - sluzbeni put u inozemstvo", 8);
      tbx_AhNETO_P    = hamper.CreateVvTextBox(6, 3, "tbx_AhNETO_P"   , "Konto Neto"                           , 8);
     

      tbx_AhPorez_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_AhPrirez_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_AhZPI_D    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_AhNETO_D   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_AhPorez_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_AhPrirez_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_AhZPI_P    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_AhNETO_P   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_AhMIO_I_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_AhMIO_II_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_AhZdrOsig_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_AhMIO_I_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_AhMIO_II_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_AhZdrOsig_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;


      tbx_AhPorez_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_AhPrirez_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_AhZPI_D    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_AhNETO_D   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_AhPorez_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_AhPrirez_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_AhZPI_P    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_AhNETO_P   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_AhMIO_I_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_AhMIO_II_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_AhZdrOsig_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_AhMIO_I_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_AhMIO_II_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_AhZdrOsig_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);

      tbx_AhZPI_D.Visible = tbx_AhZPI_P.Visible = false;

      Label lblP         = hamper.CreateVvLabel        (0, 4, "Pozicija:"      , ContentAlignment.MiddleRight);
      tbx_pozicijaAH     = hamper.CreateVvTextBoxLookUp(1, 4, "tbx_pozicijaAH", "Pozicija", 32);
      tbx_pozicijaAH.JAM_Set_LookUpTable(ZXC.luiListaPozicijePlanaRLZ, (int)ZXC.Kolona.prva);
      tbx_pozicijaAH.Visible = 
      lblP          .Visible = IsVisiblePozicija;

   }

   private void InitializeHamper_SZ(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 4, "", this, false);

      hamper.Location = new Point(hamp_ShemaAH.Left, hamp_shemaNO.Top);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "SEZONSKO ZAPOŠLJAVANJE U POLJOPRIVREDI - SZ", 4, 0, ContentAlignment.MiddleLeft);
      lbl.Font = ZXC.vvFont.SmallBoldFont;

      hamper.CreateVvLabel(0, 2, "Duguje:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 3, "Potražuje:", ContentAlignment.MiddleRight);
      
      hamper.CreateVvLabel(1, 1, "Porez"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(2, 1, "Prirez" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(3, 1, "NETO"   , ContentAlignment.MiddleLeft);
      
      tbx_SzPorez_D   = hamper.CreateVvTextBox(1, 2, "tbx_SzPorez_D"  , "Konto Porez" , 8);
      tbx_SzPrirez_D  = hamper.CreateVvTextBox(2, 2, "tbx_SzPrirez_D" , "Konto Prirez", 8);
      tbx_SzNETO_D    = hamper.CreateVvTextBox(3, 2, "tbx_SzNETO_D"   , "Konto Neto"  , 8);
      
      tbx_SzPorez_P   = hamper.CreateVvTextBox(1, 3, "tbx_SzPorez_P"  , "Konto Porez" , 8);
      tbx_SzPrirez_P  = hamper.CreateVvTextBox(2, 3, "tbx_SzPrirez_P" , "Konto Prirez", 8);
      tbx_SzNETO_P    = hamper.CreateVvTextBox(3, 3, "tbx_SzNETO_P"   , "Konto Neto"  , 8);

      tbx_SzPorez_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_SzPrirez_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_SzNETO_D   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_SzPorez_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_SzPrirez_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_SzNETO_P   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 

      tbx_SzPorez_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_SzPrirez_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_SzNETO_D   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_SzPorez_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_SzPrirez_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_SzNETO_P   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);

   }

   private void InitializeHamper_SN(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 4, "", this, false);

      hamper.Location = new Point(hamp_shemaNO.Right + ZXC.Q3un, hamp_shemaNO.Top);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "ŠPORTAŠI, UMJETNICI, NOVINARI - SN", 4, 0, ContentAlignment.MiddleLeft);
      lbl.Font = ZXC.vvFont.SmallBoldFont;

      hamper.CreateVvLabel(0, 2, "Duguje:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 3, "Potražuje:", ContentAlignment.MiddleRight);
      
      hamper.CreateVvLabel(1, 1, "Porez"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(2, 1, "Prirez" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(3, 1, "ZPI"    , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(4, 1, "NETO"   , ContentAlignment.MiddleLeft);
      
      tbx_SnPorez_D   = hamper.CreateVvTextBox(1, 2, "tbx_SnPorez_D"  , "Konto Porez"                          , 8);
      tbx_SnPrirez_D  = hamper.CreateVvTextBox(2, 2, "tbx_SnPrirez_D" , "Konto Prirez"                         , 8);
      tbx_SnZPI_D     = hamper.CreateVvTextBox(3, 2, "tbx_SnZPI_D"    , "Konto ZPI - sluzbeni put u inozemstvo", 8);
      tbx_SnNETO_D    = hamper.CreateVvTextBox(4, 2, "tbx_SnNETO_D"   , "Konto Neto"                           , 8);
      
      tbx_SnPorez_P   = hamper.CreateVvTextBox(1, 3, "tbx_SnPorez_P"  , "Konto Porez"                          , 8);
      tbx_SnPrirez_P  = hamper.CreateVvTextBox(2, 3, "tbx_SnPrirez_P" , "Konto Prirez"                         , 8);
      tbx_SnZPI_P     = hamper.CreateVvTextBox(3, 3, "tbx_SnZPI_P"    , "Konto ZPI - sluzbeni put u inozemstvo", 8);
      tbx_SnNETO_P    = hamper.CreateVvTextBox(4, 3, "tbx_SnNETO_P"   , "Konto Neto"                           , 8);

      tbx_SnPorez_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_SnPrirez_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_SnZPI_D    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_SnNETO_D   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_SnPorez_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_SnPrirez_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_SnZPI_P    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_SnNETO_P   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 

      tbx_SnPorez_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_SnPrirez_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_SnZPI_D    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_SnNETO_D   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_SnPorez_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_SnPrirez_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_SnZPI_P    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_SnNETO_P   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);

      hamper.Visible = false;
   }

   private void InitializeHamper_NE(out VvHamper hamper)
   {
      hamper = new VvHamper(7, 4, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, hamp_shemaNO.Bottom + ZXC.Qun2);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "NEREZIDENTI - NE", 2, 0, ContentAlignment.MiddleLeft);
      lbl.Font = ZXC.vvFont.SmallBoldFont;

      hamper.CreateVvLabel(0, 2, "Duguje:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 3, "Potražuje:", ContentAlignment.MiddleRight);

      hamper.CreateVvLabel(1, 1, "Porez"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(2, 1, "Prirez" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(3, 1, "MIO_I"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(4, 1, "MIO_II" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(5, 1, "ZdrOsig", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(6, 1, "NETO"   , ContentAlignment.MiddleLeft);

      tbx_NePorez_D   = hamper.CreateVvTextBox(1, 2, "tbx_NePorez_D"  , "Konto Porez"                          , 8);
      tbx_NePrirez_D  = hamper.CreateVvTextBox(2, 2, "tbx_NePrirez_D" , "Konto Prirez"                         , 8);
      tbx_NeMIO_I_D   = hamper.CreateVvTextBox(3, 2, "tbx_NeMIO_I_D"  , "Konto MIO I"                          , 8);
      tbx_NeMIO_II_D  = hamper.CreateVvTextBox(4, 2, "tbx_NeMIO_II_D" , "Konto MIO II"                         , 8);
      tbx_NeZdrOsig_D = hamper.CreateVvTextBox(5, 2, "tbx_NeZdrOsig_D", "Konto Zdravstveno osiguranje"         , 8);
      tbx_NeNETO_D    = hamper.CreateVvTextBox(6, 2, "tbx_NeNETO_D"   , "Konto Neto"                           , 8);
      
      tbx_NePorez_P   = hamper.CreateVvTextBox(1, 3, "tbx_NePorez_P"  , "Konto Porez"                          , 8);
      tbx_NePrirez_P  = hamper.CreateVvTextBox(2, 3, "tbx_NePrirez_P" , "Konto Prirez"                         , 8);
      tbx_NeMIO_I_P   = hamper.CreateVvTextBox(3, 3, "tbx_NeMIO_I_P"  , "Konto MIO I"                          , 8);
      tbx_NeMIO_II_P  = hamper.CreateVvTextBox(4, 3, "tbx_NeMIO_II_P" , "Konto MIO II"                         , 8);
      tbx_NeZdrOsig_P = hamper.CreateVvTextBox(5, 3, "tbx_NeZdrOsig_P", "Konto Zdravstveno osiguranje"         , 8);
      tbx_NeNETO_P    = hamper.CreateVvTextBox(6, 3, "tbx_NeNETO_P"   , "Konto Neto"                           , 8);

      tbx_NePorez_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NePrirez_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NeMIO_I_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NeMIO_II_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NeZdrOsig_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NeNETO_D   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NePorez_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NePrirez_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_NeMIO_I_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NeMIO_II_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NeZdrOsig_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_NeNETO_P   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      
      tbx_NePorez_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NePrirez_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NeMIO_I_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NeMIO_II_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NeZdrOsig_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NeNETO_D   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NePorez_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NePrirez_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NeMIO_I_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NeMIO_II_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NeZdrOsig_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_NeNETO_P   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);

      hamper.Visible = false;

   }

   private void InitializeHamper_SO(out VvHamper hamper)
   {
      hamper = new VvHamper(8, 5, "", this, false);
      hamper.Location = new Point(hamp_shemaNE.Right, hamp_shemaNE.Top);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "STRUČNO OSPOSOBLJAVANJE - SO", 2, 0, ContentAlignment.MiddleLeft);
      lbl.Font = ZXC.vvFont.SmallBoldFont;

      hamper.CreateVvLabel(0, 2, "Duguje:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 3, "Potražuje:", ContentAlignment.MiddleRight);

      hamper.CreateVvLabel(1, 1, "Porez"    , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(2, 1, "Prirez"   , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(3, 1, "MIO_oo"   , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(4, 1, "ZdrOs_zzr", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(5, 1, "ZPI"      , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(6, 1, "NETO"     , ContentAlignment.MiddleLeft);

      tbx_StPorez_D   = hamper.CreateVvTextBox(1, 2, "tbx_StPorez_D"  , "Konto Porez"                          , 8);
      tbx_StPrirez_D  = hamper.CreateVvTextBox(2, 2, "tbx_StPrirez_D" , "Konto Prirez"                         , 8);
      tbx_StMIO_oo_D  = hamper.CreateVvTextBox(3, 2, "tbx_StMIO_I_D"  , "Konto MIO u određenim okolnostima"    , 8);
      tbx_StZOs_zzr_D = hamper.CreateVvTextBox(4, 2, "tbx_StZdrOsig_D", "Konto posebni doprinos za zdrav. osig. zaštite zdravlja na radu"         , 8);
      tbx_StZPI_D     = hamper.CreateVvTextBox(5, 2, "tbx_StZPI_D"    , "Konto ZPI - sluzbeni put u inozemstvo", 8);
      tbx_StNETO_D    = hamper.CreateVvTextBox(6, 2, "tbx_StNETO_D"   , "Konto Neto"                           , 8);
      
      tbx_StPorez_P   = hamper.CreateVvTextBox(1, 3, "tbx_StPorez_P"  , "Konto Porez"                          , 8);
      tbx_StPrirez_P  = hamper.CreateVvTextBox(2, 3, "tbx_StPrirez_P" , "Konto Prirez"                         , 8);
      tbx_StMIO_oo_P  = hamper.CreateVvTextBox(3, 3, "tbx_StMIO_I_P"  , "Konto MIO u određenim okolnostima"    , 8);
      tbx_StZOs_zzr_P = hamper.CreateVvTextBox(4, 3, "tbx_StZdrOsig_P", "Konto posebni doprinos za zdrav. osig. zaštite zdravlja na radu", 8);
      tbx_StZPI_P     = hamper.CreateVvTextBox(5, 3, "tbx_StZPI_P"    , "Konto ZPI - sluzbeni put u inozemstvo", 8);
      tbx_StNETO_P    = hamper.CreateVvTextBox(6, 3, "tbx_StNETO_P"   , "Konto Neto"                           , 8);

      tbx_StPorez_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_StPrirez_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_StMIO_oo_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_StZOs_zzr_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_StZPI_D    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_StNETO_D   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_StPorez_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_StPrirez_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_StMIO_oo_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_StZOs_zzr_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_StZPI_P    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_StNETO_P   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      
      tbx_StPorez_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_StPrirez_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_StMIO_oo_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_StZOs_zzr_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_StZPI_D    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_StNETO_D   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_StPorez_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_StPrirez_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_StMIO_oo_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_StZOs_zzr_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_StZPI_P    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_StNETO_P   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);

      Label lblP         = hamper.CreateVvLabel        (0, 4, "Pozicija:"      , ContentAlignment.MiddleRight);
      tbx_pozicijaSO     = hamper.CreateVvTextBoxLookUp(1, 4, "tbx_pozicijaSO", "Pozicija", 32);
      tbx_pozicijaSO.JAM_Set_LookUpTable(ZXC.luiListaPozicijePlanaRLZ, (int)ZXC.Kolona.prva);
      tbx_pozicijaSO.Visible = 
      lblP          .Visible = IsVisiblePozicija;

      hamper.Visible = false;

   }

   private void InitializeHamper_TV(out VvHamper hamper)
   {
      hamper = new VvHamper(8, 5, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, hamp_shemaNO.Bottom + ZXC.Qun2);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "TURISTIČKO VIJEĆE - TV", 2, 0, ContentAlignment.MiddleLeft);
      lbl.Font = ZXC.vvFont.SmallBoldFont;

      hamper.CreateVvLabel(0, 2, "Duguje:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 3, "Potražuje:", ContentAlignment.MiddleRight);

      hamper.CreateVvLabel(1, 1, "Porez"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(2, 1, "Prirez" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(3, 1, "MIO_I"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(4, 1, "MIO_II" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(5, 1, "ZdrOsig", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(6, 1, "ZPI"    , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(7, 1, "NETO"   , ContentAlignment.MiddleLeft);

      tbx_TvPorez_D   = hamper.CreateVvTextBox(1, 2, "tbx_TvPorez_D"  , "Konto Porez"                          , 8);
      tbx_TvPrirez_D  = hamper.CreateVvTextBox(2, 2, "tbx_TvPrirez_D" , "Konto Prirez"                         , 8);
      tbx_TvMIO_I_D   = hamper.CreateVvTextBox(3, 2, "tbx_TvMIO_I_D"  , "Konto MIO I"                          , 8);
      tbx_TvMIO_II_D  = hamper.CreateVvTextBox(4, 2, "tbx_TvMIO_II_D" , "Konto MIO II"                         , 8);
      tbx_TvZdrOsig_D = hamper.CreateVvTextBox(5, 2, "tbx_TvZdrOsig_D", "Konto Zdravstveno osiguranje"         , 8);
      tbx_TvZPI_D     = hamper.CreateVvTextBox(6, 2, "tbx_TvZPI_D"    , "Konto ZPI - sluzbeni put u inozemstvo", 8);
      tbx_TvNETO_D    = hamper.CreateVvTextBox(7, 2, "tbx_TvNETO_D"   , "Konto Neto"                           , 8);
      
      tbx_TvPorez_P   = hamper.CreateVvTextBox(1, 3, "tbx_TvPorez_P"  , "Konto Porez"                          , 8);
      tbx_TvPrirez_P  = hamper.CreateVvTextBox(2, 3, "tbx_TvPrirez_P" , "Konto Prirez"                         , 8);
      tbx_TvMIO_I_P   = hamper.CreateVvTextBox(3, 3, "tbx_TvMIO_I_P"  , "Konto MIO I"                          , 8);
      tbx_TvMIO_II_P  = hamper.CreateVvTextBox(4, 3, "tbx_TvMIO_II_P" , "Konto MIO II"                         , 8);
      tbx_TvZdrOsig_P = hamper.CreateVvTextBox(5, 3, "tbx_TvZdrOsig_P", "Konto Zdravstveno osiguranje"         , 8);
      tbx_TvZPI_P     = hamper.CreateVvTextBox(6, 3, "tbx_TvZPI_P"    , "Konto ZPI - sluzbeni put u inozemstvo", 8);
      tbx_TvNETO_P    = hamper.CreateVvTextBox(7, 3, "tbx_TvNETO_P"   , "Konto Neto"                           , 8);

      tbx_TvPorez_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_TvPrirez_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_TvMIO_I_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_TvMIO_II_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_TvZdrOsig_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_TvZPI_D    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_TvNETO_D   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_TvPorez_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_TvPrirez_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_TvMIO_I_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_TvMIO_II_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_TvZdrOsig_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_TvZPI_P    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_TvNETO_P   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      
      tbx_TvPorez_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_TvPrirez_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_TvMIO_I_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_TvMIO_II_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_TvZdrOsig_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_TvZPI_D    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_TvNETO_D   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_TvPorez_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_TvPrirez_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_TvMIO_I_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_TvMIO_II_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_TvZdrOsig_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_TvZPI_P    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_TvNETO_P   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);

      Label lblP         = hamper.CreateVvLabel        (0, 4, "Pozicija:"      , ContentAlignment.MiddleRight);
      tbx_pozicijaTV     = hamper.CreateVvTextBoxLookUp(1, 4, "tbx_pozicijaTV", "Pozicija", 32);
      tbx_pozicijaTV.JAM_Set_LookUpTable(ZXC.luiListaPozicijePlanaRLZ, (int)ZXC.Kolona.prva);
      tbx_pozicijaTV.Visible = 
      lblP          .Visible = IsVisiblePozicija;

   }

   private void InitializeHamper_SD(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 4, "", this, false);

      hamper.Location = new Point(hamp_shemaTV.Right, hamp_shemaTV.Top);
      //                                     0         1         2        3         4          5   
      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "OBRTNICI samo doprinosi - SD", 4, 0, ContentAlignment.MiddleLeft);
      lbl.Font = ZXC.vvFont.SmallBoldFont;
      
      hamper.CreateVvLabel(0, 2, "Duguje:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 3, "Potražuje:", ContentAlignment.MiddleRight);

      hamper.CreateVvLabel( 1, 1, "MIO_I"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 2, 1, "MIO_II" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 3, 1, "ZdrOsig", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 4, 1, "ZOR"    , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel( 5, 1, "ZAP"    , ContentAlignment.MiddleLeft);

      tbx_Sd_MIO_I_D   = hamper.CreateVvTextBox( 1, 2, "tbx_SdMIO_I_D"  , "", 8);
      tbx_Sd_MIO_II_D  = hamper.CreateVvTextBox( 2, 2, "tbx_SdMIO_II_D" , "", 8);
      tbx_Sd_ZdrOsig_D = hamper.CreateVvTextBox( 3, 2, "tbx_SdZdrOsig_D", "", 8);
      tbx_Sd_ZOR_D     = hamper.CreateVvTextBox( 4, 2, "tbx_SdZOR_D"    , "", 8);
      tbx_Sd_ZAP_D     = hamper.CreateVvTextBox( 5, 2, "tbx_SdZAP_D"    , "", 8);
      
      tbx_Sd_MIO_I_P   = hamper.CreateVvTextBox( 1, 3, "tbx_SdMIO_I_P"  , "", 8);
      tbx_Sd_MIO_II_P  = hamper.CreateVvTextBox( 2, 3, "tbx_SdMIO_II_P" , "", 8);
      tbx_Sd_ZdrOsig_P = hamper.CreateVvTextBox( 3, 3, "tbx_SdZdrOsig_P", "", 8);
      tbx_Sd_ZOR_P     = hamper.CreateVvTextBox( 4, 3, "tbx_SdZOR_P"    , "", 8);
      tbx_Sd_ZAP_P     = hamper.CreateVvTextBox( 5, 3, "tbx_SdZAP_P"    , "", 8);

      tbx_PpMIO_I_D  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpMIO_II_D .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpZdrOsig_D.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpZOR_D    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpZAP_D    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpMIO_I_P  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpMIO_II_P .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpZdrOsig_P.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpZOR_P    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 
      tbx_PpZAP_P    .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly; 

      tbx_PpMIO_I_D  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpMIO_II_D .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpZdrOsig_D.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpZOR_D    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpZAP_D    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpMIO_I_P  .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpMIO_II_P .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpZdrOsig_P.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpZOR_P    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_PpZAP_P    .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
   }
   
   private void InitializeHamper_CR_OpisDodataka(out VvHamper hamper)
   {
      hamper          = new VvHamper(7, 5, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, hamp_shemaTV.Bottom + ZXC.Qun2);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q7un, ZXC.Q7un, ZXC.Q7un, ZXC.Q7un, ZXC.Q7un, ZXC.Q7un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "NAZIVI DODATAKA NA PLAĆU za print Isplatna Lista Plaće OP", 4, 0, ContentAlignment.MiddleLeft);
      lbl.Font = ZXC.vvFont.SmallBoldFont;

      hamper.CreateVvLabel(0, 2, "Naziv:"      , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 1, "Kolona"      , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(1, 1, "KfBr1"       , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(2, 1, "StD1"        , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(3, 1, "StD2"        , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(4, 1, "DodBruto"    , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(5, 1, "PolDodBrt"   , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(6, 1, "StDodStz"    , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(0, 3, "TekstOsn:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 4, "BrutoKorekc:", ContentAlignment.MiddleRight);

      tbx_KfBr1       = hamper.CreateVvTextBox(1, 2, "tbx_KfBr1"      , "");
      tbx_StD1        = hamper.CreateVvTextBox(2, 2, "tbx_StD1"       , "");
      tbx_StD2        = hamper.CreateVvTextBox(3, 2, "tbx_StD2"       , "");
      tbx_DodBruto    = hamper.CreateVvTextBox(4, 2, "tbx_DodBruto"   , "");
      tbx_PolDodBrt   = hamper.CreateVvTextBox(5, 2, "tbx_PolDodBrt"  , "");
      tbx_StDodStz    = hamper.CreateVvTextBox(6, 2, "tbx_StDodStz"   , "");
      tbx_VrKfBr1     = hamper.CreateVvTextBox(1, 3, "tbx_VrKfBr1"    , "");
      tbx_BrutoKorekc = hamper.CreateVvTextBox(1, 4, "tbx_BrutoKorekc", "");

      cbx_visibleVrKoefBruto1 = hamper.CreateVvCheckBox_OLD(2, 3, null, 3, 0, "Prikaži i vrijednost Koeficijenta za Bruto1", System.Windows.Forms.RightToLeft.No);

   }

   private void InitializeHamper_IsOsnovObDoh(out VvHamper hamper)
   {
      hamper          = new VvHamper(1, 2, "", this, false);
      hamper.Location = new Point(hamp_CR_OP.Left, hamp_CR_OP.Bottom + ZXC.QunMrgn);

      hamper.VvColWdt      = new int[] { ZXC.Q10un * 3 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8      };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun2, ZXC.Qun2 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cxb_isObustOsnDohodak = hamper.CreateVvCheckBox_OLD(0, 0, null, "Osnovica za obračun obustave na osnovu postotka je Dohodak (standardna osnovica je Neto)", System.Windows.Forms.RightToLeft.No);
     
      cxb_isNPnaRR_Umjesecu   = hamper.CreateVvCheckBox_OLD(0, 1, null, "Mjesec (datum od i do na Joppd obrascu) za NeoPrim na obračunu plaće (RR) je mjesec U KOJEM JE ISPLATA (standardno je kao i plaća tj. za mjesec)", System.Windows.Forms.RightToLeft.No);
   
   }

   #endregion Hampers ShemaKonta

   #region Fld_
   
   public string Fld_Porez_D       {  get { return tbx_Porez_D    .Text; } set { tbx_Porez_D    .Text = value; } }
   public string Fld_Prirez_D      {  get { return tbx_Prirez_D   .Text; } set { tbx_Prirez_D   .Text = value; } }
   public string Fld_MIO_I_D       {  get { return tbx_MIO_I_D    .Text; } set { tbx_MIO_I_D    .Text = value; } }
   public string Fld_MIO_II_D      {  get { return tbx_MIO_II_D   .Text; } set { tbx_MIO_II_D   .Text = value; } }
   public string Fld_ZdrOsig_D     {  get { return tbx_ZdrOsig_D  .Text; } set { tbx_ZdrOsig_D  .Text = value; } }
   public string Fld_ZOR_D         {  get { return tbx_ZOR_D      .Text; } set { tbx_ZOR_D      .Text = value; } }
   public string Fld_ZPI_D         {  get { return tbx_ZPI_D      .Text; } set { tbx_ZPI_D      .Text = value; } }
   public string Fld_ZAP_D         {  get { return tbx_ZAP_D      .Text; } set { tbx_ZAP_D      .Text = value; } }
   public string Fld_ZPP_D         {  get { return tbx_ZPP_D      .Text; } set { tbx_ZPP_D      .Text = value; } }
   public string Fld_NETO_D        {  get { return tbx_NETO_D     .Text; } set { tbx_NETO_D     .Text = value; } }
   public string Fld_PREVOZ_D      {  get { return tbx_PREVOZ_D   .Text; } set { tbx_PREVOZ_D   .Text = value; } }
   public string Fld_MIO_1Na_D     {  get { return tbx_MIO_1Na_D  .Text; } set { tbx_MIO_1Na_D  .Text = value; } }
   public string Fld_MIO_2Na_D     {  get { return tbx_MIO_2Na_D  .Text; } set { tbx_MIO_2Na_D  .Text = value; } }
   public string Fld_Porez_P       {  get { return tbx_Porez_P    .Text; } set { tbx_Porez_P    .Text = value; } }
   public string Fld_Prirez_P      {  get { return tbx_Prirez_P   .Text; } set { tbx_Prirez_P   .Text = value; } }
   public string Fld_MIO_I_P       {  get { return tbx_MIO_I_P    .Text; } set { tbx_MIO_I_P    .Text = value; } }
   public string Fld_MIO_II_P      {  get { return tbx_MIO_II_P   .Text; } set { tbx_MIO_II_P   .Text = value; } }
   public string Fld_ZdrOsig_P     {  get { return tbx_ZdrOsig_P  .Text; } set { tbx_ZdrOsig_P  .Text = value; } }
   public string Fld_ZOR_P         {  get { return tbx_ZOR_P      .Text; } set { tbx_ZOR_P      .Text = value; } }
   public string Fld_ZPI_P         {  get { return tbx_ZPI_P      .Text; } set { tbx_ZPI_P      .Text = value; } }
   public string Fld_ZAP_P         {  get { return tbx_ZAP_P      .Text; } set { tbx_ZAP_P      .Text = value; } }
   public string Fld_ZPP_P         {  get { return tbx_ZPP_P      .Text; } set { tbx_ZPP_P      .Text = value; } }
   public string Fld_NETO_P        {  get { return tbx_NETO_P     .Text; } set { tbx_NETO_P     .Text = value; } }
   public string Fld_PREVOZ_P      {  get { return tbx_PREVOZ_P   .Text; } set { tbx_PREVOZ_P   .Text = value; } }
   public string Fld_MIO_1Na_P     {  get { return tbx_MIO_1Na_P  .Text; } set { tbx_MIO_1Na_P  .Text = value; } }
   public string Fld_MIO_2Na_P     {  get { return tbx_MIO_2Na_P  .Text; } set { tbx_MIO_2Na_P  .Text = value; } }
   public string Fld_PpPorez_D     {  get { return tbx_PpPorez_D  .Text; } set { tbx_PpPorez_D  .Text = value; } }
   public string Fld_PpPrirez_D    {  get { return tbx_PpPrirez_D .Text; } set { tbx_PpPrirez_D .Text = value; } }
   public string Fld_PpMIO_I_D     {  get { return tbx_PpMIO_I_D  .Text; } set { tbx_PpMIO_I_D  .Text = value; } }
   public string Fld_PpMIO_II_D    {  get { return tbx_PpMIO_II_D .Text; } set { tbx_PpMIO_II_D .Text = value; } }
   public string Fld_PpZdrOsig_D   {  get { return tbx_PpZdrOsig_D.Text; } set { tbx_PpZdrOsig_D.Text = value; } }
   public string Fld_PpZOR_D       {  get { return tbx_PpZOR_D    .Text; } set { tbx_PpZOR_D    .Text = value; } }
   public string Fld_PpZPI_D       {  get { return tbx_PpZPI_D    .Text; } set { tbx_PpZPI_D    .Text = value; } }
   public string Fld_PpZAP_D       {  get { return tbx_PpZAP_D    .Text; } set { tbx_PpZAP_D    .Text = value; } }
   public string Fld_PpZPP_D       {  get { return tbx_PpZPP_D    .Text; } set { tbx_PpZPP_D    .Text = value; } }
   public string Fld_PpNETO_D      {  get { return tbx_PpNETO_D   .Text; } set { tbx_PpNETO_D   .Text = value; } }
   public string Fld_PpPREVOZ_D    {  get { return tbx_PpPREVOZ_D .Text; } set { tbx_PpPREVOZ_D .Text = value; } }
   public string Fld_PpMIO_1Na_D   {  get { return tbx_PpMIO_1Na_D.Text; } set { tbx_PpMIO_1Na_D.Text = value; } }
   public string Fld_PpMIO_2Na_D   {  get { return tbx_PpMIO_2Na_D.Text; } set { tbx_PpMIO_2Na_D.Text = value; } }
   public string Fld_PpPorez_P     {  get { return tbx_PpPorez_P  .Text; } set { tbx_PpPorez_P  .Text = value; } }
   public string Fld_PpPrirez_P    {  get { return tbx_PpPrirez_P .Text; } set { tbx_PpPrirez_P .Text = value; } }
   public string Fld_PpMIO_I_P     {  get { return tbx_PpMIO_I_P  .Text; } set { tbx_PpMIO_I_P  .Text = value; } }
   public string Fld_PpMIO_II_P    {  get { return tbx_PpMIO_II_P .Text; } set { tbx_PpMIO_II_P .Text = value; } }
   public string Fld_PpZdrOsig_P   {  get { return tbx_PpZdrOsig_P.Text; } set { tbx_PpZdrOsig_P.Text = value; } }
   public string Fld_PpZOR_P       {  get { return tbx_PpZOR_P    .Text; } set { tbx_PpZOR_P    .Text = value; } }
   public string Fld_PpZPI_P       {  get { return tbx_PpZPI_P    .Text; } set { tbx_PpZPI_P    .Text = value; } }
   public string Fld_PpZAP_P       {  get { return tbx_PpZAP_P    .Text; } set { tbx_PpZAP_P    .Text = value; } }
   public string Fld_PpZPP_P       {  get { return tbx_PpZPP_P    .Text; } set { tbx_PpZPP_P    .Text = value; } }
   public string Fld_PpNETO_P      {  get { return tbx_PpNETO_P   .Text; } set { tbx_PpNETO_P   .Text = value; } }
   public string Fld_PpPREVOZ_P    {  get { return tbx_PpPREVOZ_P .Text; } set { tbx_PpPREVOZ_P .Text = value; } }
   public string Fld_PpMIO_1Na_P   {  get { return tbx_PpMIO_1Na_P.Text; } set { tbx_PpMIO_1Na_P.Text = value; } }
   public string Fld_PpMIO_2Na_P   {  get { return tbx_PpMIO_2Na_P.Text; } set { tbx_PpMIO_2Na_P.Text = value; } }
   public string Fld_UdPorez_D     {  get { return tbx_UdPorez_D  .Text; } set { tbx_UdPorez_D  .Text = value; } }
   public string Fld_UdPrirez_D    {  get { return tbx_UdPrirez_D .Text; } set { tbx_UdPrirez_D .Text = value; } }
   public string Fld_UdMIO_I_D     {  get { return tbx_UdMIO_I_D  .Text; } set { tbx_UdMIO_I_D  .Text = value; } }
   public string Fld_UdMIO_II_D    {  get { return tbx_UdMIO_II_D .Text; } set { tbx_UdMIO_II_D .Text = value; } }
   public string Fld_UdZdrOsig_D   {  get { return tbx_UdZdrOsig_D.Text; } set { tbx_UdZdrOsig_D.Text = value; } }
   public string Fld_UdZPI_D       {  get { return tbx_UdZPI_D    .Text; } set { tbx_UdZPI_D    .Text = value; } }
   public string Fld_UdNETO_D      {  get { return tbx_UdNETO_D   .Text; } set { tbx_UdNETO_D   .Text = value; } }
   public string Fld_UdPorez_P     {  get { return tbx_UdPorez_P  .Text; } set { tbx_UdPorez_P  .Text = value; } }
   public string Fld_UdPrirez_P    {  get { return tbx_UdPrirez_P .Text; } set { tbx_UdPrirez_P .Text = value; } }
   public string Fld_UdMIO_I_P     {  get { return tbx_UdMIO_I_P  .Text; } set { tbx_UdMIO_I_P  .Text = value; } }
   public string Fld_UdMIO_II_P    {  get { return tbx_UdMIO_II_P .Text; } set { tbx_UdMIO_II_P .Text = value; } }
   public string Fld_UdZdrOsig_P   {  get { return tbx_UdZdrOsig_P.Text; } set { tbx_UdZdrOsig_P.Text = value; } }
   public string Fld_UdZPI_P       {  get { return tbx_UdZPI_P    .Text; } set { tbx_UdZPI_P    .Text = value; } }
   public string Fld_UdNETO_P      {  get { return tbx_UdNETO_P   .Text; } set { tbx_UdNETO_P   .Text = value; } }
   public string Fld_NoPorez_D     {  get { return tbx_NoPorez_D  .Text; } set { tbx_NoPorez_D  .Text = value; } }
   public string Fld_NoPrirez_D    {  get { return tbx_NoPrirez_D .Text; } set { tbx_NoPrirez_D .Text = value; } }
   public string Fld_NoMIO_I_D     {  get { return tbx_NoMIO_I_D  .Text; } set { tbx_NoMIO_I_D  .Text = value; } }
   public string Fld_NoMIO_II_D    {  get { return tbx_NoMIO_II_D .Text; } set { tbx_NoMIO_II_D .Text = value; } }
   public string Fld_NoZdrOsig_D   {  get { return tbx_NoZdrOsig_D.Text; } set { tbx_NoZdrOsig_D.Text = value; } }
   public string Fld_NoZPI_D       {  get { return tbx_NoZPI_D    .Text; } set { tbx_NoZPI_D    .Text = value; } }
   public string Fld_NoNETO_D      {  get { return tbx_NoNETO_D   .Text; } set { tbx_NoNETO_D   .Text = value; } }
   public string Fld_NoPorez_P     {  get { return tbx_NoPorez_P  .Text; } set { tbx_NoPorez_P  .Text = value; } }
   public string Fld_NoPrirez_P    {  get { return tbx_NoPrirez_P .Text; } set { tbx_NoPrirez_P .Text = value; } }
   public string Fld_NoMIO_I_P     {  get { return tbx_NoMIO_I_P  .Text; } set { tbx_NoMIO_I_P  .Text = value; } }
   public string Fld_NoMIO_II_P    {  get { return tbx_NoMIO_II_P .Text; } set { tbx_NoMIO_II_P .Text = value; } }
   public string Fld_NoZdrOsig_P   {  get { return tbx_NoZdrOsig_P.Text; } set { tbx_NoZdrOsig_P.Text = value; } }
   public string Fld_NoZPI_P       {  get { return tbx_NoZPI_P    .Text; } set { tbx_NoZPI_P    .Text = value; } }
   public string Fld_NoNETO_P      {  get { return tbx_NoNETO_P   .Text; } set { tbx_NoNETO_P   .Text = value; } }
   public string Fld_AhPorez_D     {  get { return tbx_AhPorez_D  .Text; } set { tbx_AhPorez_D  .Text = value; } }
   public string Fld_AhPrirez_D    {  get { return tbx_AhPrirez_D .Text; } set { tbx_AhPrirez_D .Text = value; } }
   public string Fld_AhZPI_D       {  get { return tbx_AhZPI_D    .Text; } set { tbx_AhZPI_D    .Text = value; } }
   public string Fld_AhNETO_D      {  get { return tbx_AhNETO_D   .Text; } set { tbx_AhNETO_D   .Text = value; } }
   public string Fld_AhPorez_P     {  get { return tbx_AhPorez_P  .Text; } set { tbx_AhPorez_P  .Text = value; } }
   public string Fld_AhPrirez_P    {  get { return tbx_AhPrirez_P .Text; } set { tbx_AhPrirez_P .Text = value; } }
   public string Fld_AhZPI_P       {  get { return tbx_AhZPI_P    .Text; } set { tbx_AhZPI_P    .Text = value; } }
   public string Fld_AhNETO_P      {  get { return tbx_AhNETO_P   .Text; } set { tbx_AhNETO_P   .Text = value; } }
   public string Fld_SnPorez_D     {  get { return tbx_SnPorez_D  .Text; } set { tbx_SnPorez_D  .Text = value; } }
   public string Fld_SnPrirez_D    {  get { return tbx_SnPrirez_D .Text; } set { tbx_SnPrirez_D .Text = value; } }
   public string Fld_SnZPI_D       {  get { return tbx_SnZPI_D    .Text; } set { tbx_SnZPI_D    .Text = value; } }
   public string Fld_SnNETO_D      {  get { return tbx_SnNETO_D   .Text; } set { tbx_SnNETO_D   .Text = value; } }
   public string Fld_SnPorez_P     {  get { return tbx_SnPorez_P  .Text; } set { tbx_SnPorez_P  .Text = value; } }
   public string Fld_SnPrirez_P    {  get { return tbx_SnPrirez_P .Text; } set { tbx_SnPrirez_P .Text = value; } }
   public string Fld_SnZPI_P       {  get { return tbx_SnZPI_P    .Text; } set { tbx_SnZPI_P    .Text = value; } }
   public string Fld_SnNETO_P      {  get { return tbx_SnNETO_P   .Text; } set { tbx_SnNETO_P   .Text = value; } }
   public string Fld_NePorez_D     {  get { return tbx_NePorez_D  .Text; } set { tbx_NePorez_D  .Text = value; } }
   public string Fld_NePrirez_D    {  get { return tbx_NePrirez_D .Text; } set { tbx_NePrirez_D .Text = value; } }
   public string Fld_NeMIO_I_D     {  get { return tbx_NeMIO_I_D  .Text; } set { tbx_NeMIO_I_D  .Text = value; } }
   public string Fld_NeMIO_II_D    {  get { return tbx_NeMIO_II_D .Text; } set { tbx_NeMIO_II_D .Text = value; } }
   public string Fld_NeZdrOsig_D   {  get { return tbx_NeZdrOsig_D.Text; } set { tbx_NeZdrOsig_D.Text = value; } }
   public string Fld_NeNETO_D      {  get { return tbx_NeNETO_D   .Text; } set { tbx_NeNETO_D   .Text = value; } }
   public string Fld_NePorez_P     {  get { return tbx_NePorez_P  .Text; } set { tbx_NePorez_P  .Text = value; } }
   public string Fld_NePrirez_P    {  get { return tbx_NePrirez_P .Text; } set { tbx_NePrirez_P .Text = value; } }
   public string Fld_NeMIO_I_P     {  get { return tbx_NeMIO_I_P  .Text; } set { tbx_NeMIO_I_P  .Text = value; } }
   public string Fld_NeMIO_II_P    {  get { return tbx_NeMIO_II_P .Text; } set { tbx_NeMIO_II_P .Text = value; } }
   public string Fld_NeZdrOsig_P   {  get { return tbx_NeZdrOsig_P.Text; } set { tbx_NeZdrOsig_P.Text = value; } }
   public string Fld_NeNETO_P      {  get { return tbx_NeNETO_P   .Text; } set { tbx_NeNETO_P   .Text = value; } }
   public string Fld_StPorez_D     {  get { return tbx_StPorez_D  .Text; } set { tbx_StPorez_D  .Text = value; } }
   public string Fld_StPrirez_D    {  get { return tbx_StPrirez_D .Text; } set { tbx_StPrirez_D .Text = value; } }
   public string Fld_StMIO_oo_D    {  get { return tbx_StMIO_oo_D .Text; } set { tbx_StMIO_oo_D .Text = value; } }
   public string Fld_StZOs_zzr_D   {  get { return tbx_StZOs_zzr_D.Text; } set { tbx_StZOs_zzr_D.Text = value; } }
   public string Fld_StZPI_D       {  get { return tbx_StZPI_D    .Text; } set { tbx_StZPI_D    .Text = value; } }
   public string Fld_StNETO_D      {  get { return tbx_StNETO_D   .Text; } set { tbx_StNETO_D   .Text = value; } }
   public string Fld_StPorez_P     {  get { return tbx_StPorez_P  .Text; } set { tbx_StPorez_P  .Text = value; } }
   public string Fld_StPrirez_P    {  get { return tbx_StPrirez_P .Text; } set { tbx_StPrirez_P .Text = value; } }
   public string Fld_StMIO_oo_P    {  get { return tbx_StMIO_oo_P .Text; } set { tbx_StMIO_oo_P .Text = value; } }
   public string Fld_StZOs_zzr_P   {  get { return tbx_StZOs_zzr_P.Text; } set { tbx_StZOs_zzr_P.Text = value; } }
   public string Fld_StZPI_P       {  get { return tbx_StZPI_P    .Text; } set { tbx_StZPI_P    .Text = value; } }
   public string Fld_StNETO_P      {  get { return tbx_StNETO_P   .Text; } set { tbx_StNETO_P   .Text = value; } }  
   public string Fld_SzPorez_D     {  get { return tbx_SzPorez_D  .Text; } set { tbx_SzPorez_D  .Text = value; } }
   public string Fld_SzPrirez_D    {  get { return tbx_SzPrirez_D .Text; } set { tbx_SzPrirez_D .Text = value; } }
   public string Fld_SzNETO_D      {  get { return tbx_SzNETO_D   .Text; } set { tbx_SzNETO_D   .Text = value; } }
   public string Fld_SzPorez_P     {  get { return tbx_SzPorez_P  .Text; } set { tbx_SzPorez_P  .Text = value; } }
   public string Fld_SzPrirez_P    {  get { return tbx_SzPrirez_P .Text; } set { tbx_SzPrirez_P .Text = value; } }
   public string Fld_SzNETO_P      {  get { return tbx_SzNETO_P   .Text; } set { tbx_SzNETO_P   .Text = value; } }
   public string Fld_TvPorez_D     {  get { return tbx_TvPorez_D  .Text; } set { tbx_TvPorez_D  .Text = value; } }
   public string Fld_TvPrirez_D    {  get { return tbx_TvPrirez_D .Text; } set { tbx_TvPrirez_D .Text = value; } }
   public string Fld_TvMIO_I_D     {  get { return tbx_TvMIO_I_D  .Text; } set { tbx_TvMIO_I_D  .Text = value; } }
   public string Fld_TvMIO_II_D    {  get { return tbx_TvMIO_II_D .Text; } set { tbx_TvMIO_II_D .Text = value; } }
   public string Fld_TvZdrOsig_D   {  get { return tbx_TvZdrOsig_D.Text; } set { tbx_TvZdrOsig_D.Text = value; } }
   public string Fld_TvZPI_D       {  get { return tbx_TvZPI_D    .Text; } set { tbx_TvZPI_D    .Text = value; } }
   public string Fld_TvNETO_D      {  get { return tbx_TvNETO_D   .Text; } set { tbx_TvNETO_D   .Text = value; } }
   public string Fld_TvPorez_P     {  get { return tbx_TvPorez_P  .Text; } set { tbx_TvPorez_P  .Text = value; } }
   public string Fld_TvPrirez_P    {  get { return tbx_TvPrirez_P .Text; } set { tbx_TvPrirez_P .Text = value; } }
   public string Fld_TvMIO_I_P     {  get { return tbx_TvMIO_I_P  .Text; } set { tbx_TvMIO_I_P  .Text = value; } }
   public string Fld_TvMIO_II_P    {  get { return tbx_TvMIO_II_P .Text; } set { tbx_TvMIO_II_P .Text = value; } }
   public string Fld_TvZdrOsig_P   {  get { return tbx_TvZdrOsig_P.Text; } set { tbx_TvZdrOsig_P.Text = value; } }
   public string Fld_TvZPI_P       {  get { return tbx_TvZPI_P    .Text; } set { tbx_TvZPI_P    .Text = value; } }
   public string Fld_TvNETO_P      {  get { return tbx_TvNETO_P   .Text; } set { tbx_TvNETO_P   .Text = value; } }
   
   public string Fld_AhMIO_I_P     {  get { return tbx_AhMIO_I_P  .Text; } set { tbx_AhMIO_I_P  .Text = value; } }
   public string Fld_AhMIO_II_P    {  get { return tbx_AhMIO_II_P .Text; } set { tbx_AhMIO_II_P .Text = value; } }
   public string Fld_AhZdrOsig_P   {  get { return tbx_AhZdrOsig_P.Text; } set { tbx_AhZdrOsig_P.Text = value; } }
   public string Fld_AhMIO_I_D     {  get { return tbx_AhMIO_I_D  .Text; } set { tbx_AhMIO_I_D  .Text = value; } }
   public string Fld_AhMIO_II_D    {  get { return tbx_AhMIO_II_D .Text; } set { tbx_AhMIO_II_D .Text = value; } }
   public string Fld_AhZdrOsig_D   {  get { return tbx_AhZdrOsig_D.Text; } set { tbx_AhZdrOsig_D.Text = value; } }

   public string Fld_SD_MIO_I_D    {  get { return tbx_Sd_MIO_I_D    .Text; } set { tbx_Sd_MIO_I_D    .Text = value; } }
   public string Fld_SD_MIO_II_D   {  get { return tbx_Sd_MIO_II_D   .Text; } set { tbx_Sd_MIO_II_D   .Text = value; } }
   public string Fld_SD_ZdrOsig_D  {  get { return tbx_Sd_ZdrOsig_D  .Text; } set { tbx_Sd_ZdrOsig_D  .Text = value; } }
   public string Fld_SD_ZOR_D      {  get { return tbx_Sd_ZOR_D      .Text; } set { tbx_Sd_ZOR_D      .Text = value; } }
   public string Fld_SD_ZAP_D      {  get { return tbx_Sd_ZAP_D      .Text; } set { tbx_Sd_ZAP_D      .Text = value; } }
   public string Fld_SD_MIO_I_P    {  get { return tbx_Sd_MIO_I_P    .Text; } set { tbx_Sd_MIO_I_P    .Text = value; } }
   public string Fld_SD_MIO_II_P   {  get { return tbx_Sd_MIO_II_P   .Text; } set { tbx_Sd_MIO_II_P   .Text = value; } }
   public string Fld_SD_ZdrOsig_P  {  get { return tbx_Sd_ZdrOsig_P  .Text; } set { tbx_Sd_ZdrOsig_P  .Text = value; } }
   public string Fld_SD_ZOR_P      {  get { return tbx_Sd_ZOR_P      .Text; } set { tbx_Sd_ZOR_P      .Text = value; } }
   public string Fld_SD_ZAP_p      {  get { return tbx_Sd_ZAP_P      .Text; } set { tbx_Sd_ZAP_P      .Text = value; } }


   public string Fld_PozicijaRR   { get { return tbx_pozicijaRR.Text; } set { tbx_pozicijaRR.Text = value; } }
   public string Fld_PozicijaAH   { get { return tbx_pozicijaAH.Text; } set { tbx_pozicijaAH.Text = value; } }
   public string Fld_PozicijaNO   { get { return tbx_pozicijaNO.Text; } set { tbx_pozicijaNO.Text = value; } }
   public string Fld_PozicijaUD   { get { return tbx_pozicijaUD.Text; } set { tbx_pozicijaUD.Text = value; } }
   public string Fld_PozicijaSO   { get { return tbx_pozicijaSO.Text; } set { tbx_pozicijaSO.Text = value; } }
   public string Fld_PozicijaTV   { get { return tbx_pozicijaTV.Text; } set { tbx_pozicijaTV.Text = value; } }

   public string Fld_CR_KfBr1       { get { return tbx_KfBr1      .Text; } set { tbx_KfBr1      .Text = value; } }
   public string Fld_CR_StD1        { get { return tbx_StD1       .Text; } set { tbx_StD1       .Text = value; } }
   public string Fld_CR_StD2        { get { return tbx_StD2       .Text; } set { tbx_StD2       .Text = value; } }
   public string Fld_CR_DodBruto    { get { return tbx_DodBruto   .Text; } set { tbx_DodBruto   .Text = value; } }
   public string Fld_CR_PolDodBrt   { get { return tbx_PolDodBrt  .Text; } set { tbx_PolDodBrt  .Text = value; } }
   public string Fld_CR_StDodStz    { get { return tbx_StDodStz   .Text; } set { tbx_StDodStz   .Text = value; } }
   public string Fld_CR_VrKfBr1     { get { return tbx_VrKfBr1    .Text; } set { tbx_VrKfBr1    .Text = value; } }
   public string Fld_CR_BrutoKorekc { get { return tbx_BrutoKorekc.Text; } set { tbx_BrutoKorekc.Text = value; } }

   public bool Fld_CR_IsVrKfBrt1     { get { return cbx_visibleVrKoefBruto1.Checked; } set { cbx_visibleVrKoefBruto1.Checked = value; } }
   public bool Fld_IsObustOsnDoh     { get { return cxb_isObustOsnDohodak  .Checked; } set { cxb_isObustOsnDohodak  .Checked = value; } }
   public bool Fld_IsNPnaRR_U_mj { get { return cxb_isNPnaRR_Umjesecu  .Checked; } set { cxb_isNPnaRR_Umjesecu.Checked = value; } }

   public string Fld_NP63_nagr_D { get { return tbx_NP63_nagr_D.Text; } set { tbx_NP63_nagr_D.Text = value; } }
   public string Fld_NP65_preh_D { get { return tbx_NP65_preh_D.Text; } set { tbx_NP65_preh_D.Text = value; } }
   public string Fld_NP71_dZdr_D { get { return tbx_NP71_dZdr_D.Text; } set { tbx_NP71_dZdr_D.Text = value; } }
   public string Fld_NP63_nagr_P { get { return tbx_NP63_nagr_P.Text; } set { tbx_NP63_nagr_P.Text = value; } }
   public string Fld_NP65_preh_P { get { return tbx_NP65_preh_P.Text; } set { tbx_NP65_preh_P.Text = value; } }
   public string Fld_NP71_dZdr_P { get { return tbx_NP71_dZdr_P.Text; } set { tbx_NP71_dZdr_P.Text = value; } }

   public string Fld_NP17_dnev_D { get { return tbx_NP17_dnev_D.Text; } set { tbx_NP17_dnev_D.Text = value; } }
   public string Fld_NP18_loko_D { get { return tbx_NP18_loko_D.Text; } set { tbx_NP18_loko_D.Text = value; } }
   public string Fld_NP21_darD_D { get { return tbx_NP21_darD_D.Text; } set { tbx_NP21_darD_D.Text = value; } }
   public string Fld_NP22_regB_D { get { return tbx_NP22_regB_D.Text; } set { tbx_NP22_regB_D.Text = value; } }
   public string Fld_NP26_otpr_D { get { return tbx_NP26_otpr_D.Text; } set { tbx_NP26_otpr_D.Text = value; } }
   public string Fld_NP60_jubi_D { get { return tbx_NP60_jubi_D.Text; } set { tbx_NP60_jubi_D.Text = value; } }
   public string Fld_NP17_dnev_P { get { return tbx_NP17_dnev_P.Text; } set { tbx_NP17_dnev_P.Text = value; } }
   public string Fld_NP18_loko_P { get { return tbx_NP18_loko_P.Text; } set { tbx_NP18_loko_P.Text = value; } }
   public string Fld_NP21_darD_P { get { return tbx_NP21_darD_P.Text; } set { tbx_NP21_darD_P.Text = value; } }
   public string Fld_NP22_regB_P { get { return tbx_NP22_regB_P.Text; } set { tbx_NP22_regB_P.Text = value; } }
   public string Fld_NP26_otpr_P { get { return tbx_NP26_otpr_P.Text; } set { tbx_NP26_otpr_P.Text = value; } }
   public string Fld_NP60_jubi_P { get { return tbx_NP60_jubi_P.Text; } set { tbx_NP60_jubi_P.Text = value; } }

   #endregion Fld_

   #region PutShemaKontoFields(), GetShemaKontoFields()

   private void PutKtoShemaPlacaDscFields(KtoShemaPlacaDsc KSP)
   {
      Fld_Porez_D        = KSP.Dsc_Porez_D      ;
      Fld_Prirez_D       = KSP.Dsc_Prirez_D     ;
      Fld_MIO_I_D        = KSP.Dsc_MIO_I_D      ;
      Fld_MIO_II_D       = KSP.Dsc_MIO_II_D     ;
      Fld_ZdrOsig_D      = KSP.Dsc_ZdrOsig_D    ;
      Fld_ZOR_D          = KSP.Dsc_ZOR_D        ;
      Fld_ZPI_D          = KSP.Dsc_ZPI_D        ;
      Fld_ZAP_D          = KSP.Dsc_ZAP_D        ;
      Fld_ZPP_D          = KSP.Dsc_ZPP_D        ;
      Fld_NETO_D         = KSP.Dsc_NETO_D       ;
      Fld_PREVOZ_D       = KSP.Dsc_PREVOZ_D     ;
      Fld_MIO_1Na_D      = KSP.Dsc_MIO_1Na_D    ;
      Fld_MIO_2Na_D      = KSP.Dsc_MIO_2Na_D    ;
      Fld_Porez_P        = KSP.Dsc_Porez_P      ;
      Fld_Prirez_P       = KSP.Dsc_Prirez_P     ;
      Fld_MIO_I_P        = KSP.Dsc_MIO_I_P      ;
      Fld_MIO_II_P       = KSP.Dsc_MIO_II_P     ;
      Fld_ZdrOsig_P      = KSP.Dsc_ZdrOsig_P    ;
      Fld_ZOR_P          = KSP.Dsc_ZOR_P        ;
      Fld_ZPI_P          = KSP.Dsc_ZPI_P        ;
      Fld_ZAP_P          = KSP.Dsc_ZAP_P        ;
      Fld_ZPP_P          = KSP.Dsc_ZPP_P        ;
      Fld_NETO_P         = KSP.Dsc_NETO_P       ;
      Fld_PREVOZ_P       = KSP.Dsc_PREVOZ_P     ;
      Fld_MIO_1Na_P      = KSP.Dsc_MIO_1Na_P    ;
      Fld_MIO_2Na_P      = KSP.Dsc_MIO_2Na_P    ;
      Fld_PpPorez_D      = KSP.Dsc_PpPorez_D    ;
      Fld_PpPrirez_D     = KSP.Dsc_PpPrirez_D   ;
      Fld_PpMIO_I_D      = KSP.Dsc_PpMIO_I_D    ;
      Fld_PpMIO_II_D     = KSP.Dsc_PpMIO_II_D   ;
      Fld_PpZdrOsig_D    = KSP.Dsc_PpZdrOsig_D  ;
      Fld_PpZOR_D        = KSP.Dsc_PpZOR_D      ;
      Fld_PpZPI_D        = KSP.Dsc_PpZPI_D      ;
      Fld_PpZAP_D        = KSP.Dsc_PpZAP_D      ;
      Fld_PpZPP_D        = KSP.Dsc_PpZPP_D      ;
      Fld_PpNETO_D       = KSP.Dsc_PpNETO_D     ;
      Fld_PpPREVOZ_D     = KSP.Dsc_PpPREVOZ_D   ;
      Fld_PpMIO_1Na_D    = KSP.Dsc_PpMIO_1Na_D  ;
      Fld_PpMIO_2Na_D    = KSP.Dsc_PpMIO_2Na_D  ;
      Fld_PpPorez_P      = KSP.Dsc_PpPorez_P    ;
      Fld_PpPrirez_P     = KSP.Dsc_PpPrirez_P   ;
      Fld_PpMIO_I_P      = KSP.Dsc_PpMIO_I_P    ;
      Fld_PpMIO_II_P     = KSP.Dsc_PpMIO_II_P   ;
      Fld_PpZdrOsig_P    = KSP.Dsc_PpZdrOsig_P  ;
      Fld_PpZOR_P        = KSP.Dsc_PpZOR_P      ;
      Fld_PpZPI_P        = KSP.Dsc_PpZPI_P      ;
      Fld_PpZAP_P        = KSP.Dsc_PpZAP_P      ;
      Fld_PpZPP_P        = KSP.Dsc_PpZPP_P      ;
      Fld_PpNETO_P       = KSP.Dsc_PpNETO_P     ;
      Fld_PpPREVOZ_P     = KSP.Dsc_PpPREVOZ_P   ;
      Fld_PpMIO_1Na_P    = KSP.Dsc_PpMIO_1Na_P  ;
      Fld_PpMIO_2Na_P    = KSP.Dsc_PpMIO_2Na_P  ;
      Fld_UdPorez_D      = KSP.Dsc_UdPorez_D    ;
      Fld_UdPrirez_D     = KSP.Dsc_UdPrirez_D   ;
      Fld_UdMIO_I_D      = KSP.Dsc_UdMIO_I_D    ;
      Fld_UdMIO_II_D     = KSP.Dsc_UdMIO_II_D   ;
      Fld_UdZdrOsig_D    = KSP.Dsc_UdZdrOsig_D  ;
      Fld_UdZPI_D        = KSP.Dsc_UdZPI_D      ;
      Fld_UdNETO_D       = KSP.Dsc_UdNETO_D     ;
      Fld_UdPorez_P      = KSP.Dsc_UdPorez_P    ;
      Fld_UdPrirez_P     = KSP.Dsc_UdPrirez_P   ;
      Fld_UdMIO_I_P      = KSP.Dsc_UdMIO_I_P    ;
      Fld_UdMIO_II_P     = KSP.Dsc_UdMIO_II_P   ;
      Fld_UdZdrOsig_P    = KSP.Dsc_UdZdrOsig_P  ;
      Fld_UdZPI_P        = KSP.Dsc_UdZPI_P      ;
      Fld_UdNETO_P       = KSP.Dsc_UdNETO_P     ;
      Fld_NoPorez_D      = KSP.Dsc_NoPorez_D    ;
      Fld_NoPrirez_D     = KSP.Dsc_NoPrirez_D   ;
      Fld_NoMIO_I_D      = KSP.Dsc_NoMIO_I_D    ;
      Fld_NoMIO_II_D     = KSP.Dsc_NoMIO_II_D   ;
      Fld_NoZdrOsig_D    = KSP.Dsc_NoZdrOsig_D  ;
      Fld_NoZPI_D        = KSP.Dsc_NoZPI_D      ;
      Fld_NoNETO_D       = KSP.Dsc_NoNETO_D     ;
      Fld_NoPorez_P      = KSP.Dsc_NoPorez_P    ;
      Fld_NoPrirez_P     = KSP.Dsc_NoPrirez_P   ;
      Fld_NoMIO_I_P      = KSP.Dsc_NoMIO_I_P    ;
      Fld_NoMIO_II_P     = KSP.Dsc_NoMIO_II_P   ;
      Fld_NoZdrOsig_P    = KSP.Dsc_NoZdrOsig_P  ;
      Fld_NoZPI_P        = KSP.Dsc_NoZPI_P      ;
      Fld_NoNETO_P       = KSP.Dsc_NoNETO_P     ;
      Fld_AhPorez_D      = KSP.Dsc_AhPorez_D    ;
      Fld_AhPrirez_D     = KSP.Dsc_AhPrirez_D   ;
      Fld_AhZPI_D        = KSP.Dsc_AhZPI_D      ;
      Fld_AhNETO_D       = KSP.Dsc_AhNETO_D     ;
      Fld_AhPorez_P      = KSP.Dsc_AhPorez_P    ;
      Fld_AhPrirez_P     = KSP.Dsc_AhPrirez_P   ;
      Fld_AhZPI_P        = KSP.Dsc_AhZPI_P      ;
      Fld_AhNETO_P       = KSP.Dsc_AhNETO_P     ;
      Fld_SnPorez_D      = KSP.Dsc_SnPorez_D    ;
      Fld_SnPrirez_D     = KSP.Dsc_SnPrirez_D   ;
      Fld_SnZPI_D        = KSP.Dsc_SnZPI_D      ;
      Fld_SnNETO_D       = KSP.Dsc_SnNETO_D     ;
      Fld_SnPorez_P      = KSP.Dsc_SnPorez_P    ;
      Fld_SnPrirez_P     = KSP.Dsc_SnPrirez_P   ;
      Fld_SnZPI_P        = KSP.Dsc_SnZPI_P      ;
      Fld_SnNETO_P       = KSP.Dsc_SnNETO_P     ;
      Fld_NePorez_D      = KSP.Dsc_NePorez_D    ;
      Fld_NePrirez_D     = KSP.Dsc_NePrirez_D   ;
      Fld_NeMIO_I_D      = KSP.Dsc_NeMIO_I_D    ;
      Fld_NeMIO_II_D     = KSP.Dsc_NeMIO_II_D   ;
      Fld_NeZdrOsig_D    = KSP.Dsc_NeZdrOsig_D  ;
      Fld_NeNETO_D       = KSP.Dsc_NeNETO_D     ;
      Fld_NePorez_P      = KSP.Dsc_NePorez_P    ;
      Fld_NePrirez_P     = KSP.Dsc_NePrirez_P   ;
      Fld_NeMIO_I_P      = KSP.Dsc_NeMIO_I_P    ;
      Fld_NeMIO_II_P     = KSP.Dsc_NeMIO_II_P   ;
      Fld_NeZdrOsig_P    = KSP.Dsc_NeZdrOsig_P  ;
      Fld_NeNETO_P       = KSP.Dsc_NeNETO_P     ;
      Fld_StPorez_D      = KSP.Dsc_StPorez_D    ;
      Fld_StPrirez_D     = KSP.Dsc_StPrirez_D   ;
      Fld_StMIO_oo_D     = KSP.Dsc_StMIO_oo_D   ;
      Fld_StZOs_zzr_D    = KSP.Dsc_StZOs_zzr_D  ;
      Fld_StZPI_D        = KSP.Dsc_StZPI_D      ;
      Fld_StNETO_D       = KSP.Dsc_StNETO_D     ;
      Fld_StPorez_P      = KSP.Dsc_StPorez_P    ;
      Fld_StPrirez_P     = KSP.Dsc_StPrirez_P   ;
      Fld_StMIO_oo_P     = KSP.Dsc_StMIO_oo_P   ;
      Fld_StZOs_zzr_P    = KSP.Dsc_StZOs_zzr_P  ;
      Fld_StZPI_P        = KSP.Dsc_StZPI_P      ;
      Fld_StNETO_P       = KSP.Dsc_StNETO_P     ;
      Fld_SzPorez_D      = KSP.Dsc_SzPorez_D    ;
      Fld_SzPrirez_D     = KSP.Dsc_SzPrirez_D   ;
      Fld_SzNETO_D       = KSP.Dsc_SzNETO_D     ;
      Fld_SzPorez_P      = KSP.Dsc_SzPorez_P    ;
      Fld_SzPrirez_P     = KSP.Dsc_SzPrirez_P   ;
      Fld_SzNETO_P       = KSP.Dsc_SzNETO_P     ;
      Fld_TvPorez_D      = KSP.Dsc_TvPorez_D     ;
      Fld_TvPrirez_D     = KSP.Dsc_TvPrirez_D    ;
      Fld_TvMIO_I_D      = KSP.Dsc_TvMIO_I_D     ;
      Fld_TvMIO_II_D     = KSP.Dsc_TvMIO_II_D    ;
      Fld_TvZdrOsig_D    = KSP.Dsc_TvZdrOsig_D   ;
      Fld_TvZPI_D        = KSP.Dsc_TvZPI_D       ;
      Fld_TvNETO_D       = KSP.Dsc_TvNETO_D      ;
      Fld_TvPorez_P      = KSP.Dsc_TvPorez_P     ;
      Fld_TvPrirez_P     = KSP.Dsc_TvPrirez_P    ;
      Fld_TvMIO_I_P      = KSP.Dsc_TvMIO_I_P     ;
      Fld_TvMIO_II_P     = KSP.Dsc_TvMIO_II_P    ;
      Fld_TvZdrOsig_P    = KSP.Dsc_TvZdrOsig_P   ;
      Fld_TvZPI_P        = KSP.Dsc_TvZPI_P       ;
      Fld_TvNETO_P       = KSP.Dsc_TvNETO_P      ;
      Fld_PozicijaAH     = KSP.Dsc_PozicijaAH    ;
      Fld_PozicijaNO     = KSP.Dsc_PozicijaNO    ;
      Fld_PozicijaUD     = KSP.Dsc_PozicijaUD    ;
      Fld_PozicijaSO     = KSP.Dsc_PozicijaSO    ;
      Fld_PozicijaTV     = KSP.Dsc_PozicijaTV    ;
      Fld_PozicijaRR     = KSP.Dsc_PozicijaRR    ;
      Fld_CR_KfBr1       = KSP.Dsc_CR_KfBr1      ;
      Fld_CR_StD1        = KSP.Dsc_CR_StD1       ;
      Fld_CR_StD2        = KSP.Dsc_CR_StD2       ;
      Fld_CR_DodBruto    = KSP.Dsc_CR_DodBruto   ;
      Fld_CR_PolDodBrt   = KSP.Dsc_CR_PolDodBrt  ;
      Fld_CR_StDodStz    = KSP.Dsc_CR_StDodStz   ;
      Fld_CR_IsVrKfBrt1  = KSP.Dsc_CR_IsVrKfBr1  ;
      Fld_CR_VrKfBr1     = KSP.Dsc_CR_VrKfBr1    ;
      Fld_CR_BrutoKorekc = KSP.Dsc_CR_BrutoKorekc;
      Fld_SD_MIO_I_D     = KSP.Dsc_MIO_I_SD_D    ; 
      Fld_SD_MIO_II_D    = KSP.Dsc_MIO_II_SD_D   ;
      Fld_SD_ZdrOsig_D   = KSP.Dsc_ZdrOsig_SD_D  ;
      Fld_SD_ZOR_D       = KSP.Dsc_ZOR_SD_D      ;
      Fld_SD_ZAP_D       = KSP.Dsc_ZAP_SD_D      ;
      Fld_SD_MIO_I_P     = KSP.Dsc_MIO_I_SD_P    ;
      Fld_SD_MIO_II_P    = KSP.Dsc_MIO_II_SD_P   ;
      Fld_SD_ZdrOsig_P   = KSP.Dsc_ZdrOsig_SD_P  ;
      Fld_SD_ZOR_P       = KSP.Dsc_ZOR_SD_P      ;
      Fld_SD_ZAP_p       = KSP.Dsc_ZAP_SD_P      ;
      Fld_IsObustOsnDoh  = KSP.Dsc_IsObustOsnDoh ;
      Fld_IsNPnaRR_U_mj  = KSP.Dsc_IsNPnaRR_U_mj ;
      Fld_AhMIO_I_P      = KSP.Dsc_AhMIO_I_P     ;
      Fld_AhMIO_II_P     = KSP.Dsc_AhMIO_II_P    ;
      Fld_AhZdrOsig_P    = KSP.Dsc_AhZdrOsig_P   ;
      Fld_AhMIO_I_D      = KSP.Dsc_AhMIO_I_D     ;
      Fld_AhMIO_II_D     = KSP.Dsc_AhMIO_II_D    ;
      Fld_AhZdrOsig_D    = KSP.Dsc_AhZdrOsig_D   ;

      Fld_NP63_nagr_D    = KSP.Dsc_NP63_nagr_D   ;
      Fld_NP65_preh_D    = KSP.Dsc_NP65_preh_D   ;
      Fld_NP71_dZdr_D    = KSP.Dsc_NP71_dZdr_D   ;
      Fld_NP63_nagr_P    = KSP.Dsc_NP63_nagr_P   ;
      Fld_NP65_preh_P    = KSP.Dsc_NP65_preh_P   ;
      Fld_NP71_dZdr_P    = KSP.Dsc_NP71_dZdr_P   ;

      Fld_NP17_dnev_D    = KSP.Dsc_NP17_dnev_D   ;
      Fld_NP18_loko_D    = KSP.Dsc_NP18_loko_D   ;
      Fld_NP21_darD_D    = KSP.Dsc_NP21_darD_D   ;
      Fld_NP22_regB_D    = KSP.Dsc_NP22_regB_D   ;
      Fld_NP26_otpr_D    = KSP.Dsc_NP26_otpr_D   ;
      Fld_NP60_jubi_D    = KSP.Dsc_NP60_jubi_D   ;
      Fld_NP17_dnev_P    = KSP.Dsc_NP17_dnev_P   ;
      Fld_NP18_loko_P    = KSP.Dsc_NP18_loko_P   ;
      Fld_NP21_darD_P    = KSP.Dsc_NP21_darD_P   ;
      Fld_NP22_regB_P    = KSP.Dsc_NP22_regB_P   ;
      Fld_NP26_otpr_P    = KSP.Dsc_NP26_otpr_P   ;
      Fld_NP60_jubi_P    = KSP.Dsc_NP60_jubi_P   ;


   }

   public void GetKtoShemaPlacaDscFields()
   {
       KSP.Dsc_Porez_D        = Fld_Porez_D       ;
       KSP.Dsc_Prirez_D       = Fld_Prirez_D      ;
       KSP.Dsc_MIO_I_D        = Fld_MIO_I_D       ;
       KSP.Dsc_MIO_II_D       = Fld_MIO_II_D      ;
       KSP.Dsc_ZdrOsig_D      = Fld_ZdrOsig_D     ;
       KSP.Dsc_ZOR_D          = Fld_ZOR_D         ;
       KSP.Dsc_ZPI_D          = Fld_ZPI_D         ;
       KSP.Dsc_ZAP_D          = Fld_ZAP_D         ;
       KSP.Dsc_ZPP_D          = Fld_ZPP_D         ;
       KSP.Dsc_NETO_D         = Fld_NETO_D        ;
       KSP.Dsc_PREVOZ_D       = Fld_PREVOZ_D      ;
       KSP.Dsc_MIO_1Na_D      = Fld_MIO_1Na_D     ;
       KSP.Dsc_MIO_2Na_D      = Fld_MIO_2Na_D     ;
       KSP.Dsc_Porez_P        = Fld_Porez_P       ;
       KSP.Dsc_Prirez_P       = Fld_Prirez_P      ;
       KSP.Dsc_MIO_I_P        = Fld_MIO_I_P       ;
       KSP.Dsc_MIO_II_P       = Fld_MIO_II_P      ;
       KSP.Dsc_ZdrOsig_P      = Fld_ZdrOsig_P     ;
       KSP.Dsc_ZOR_P          = Fld_ZOR_P         ;
       KSP.Dsc_ZPI_P          = Fld_ZPI_P         ;
       KSP.Dsc_ZAP_P          = Fld_ZAP_P         ;
       KSP.Dsc_ZPP_P          = Fld_ZPP_P         ;
       KSP.Dsc_NETO_P         = Fld_NETO_P        ;
       KSP.Dsc_PREVOZ_P       = Fld_PREVOZ_P      ;
       KSP.Dsc_MIO_1Na_P      = Fld_MIO_1Na_P     ;
       KSP.Dsc_MIO_2Na_P      = Fld_MIO_2Na_P     ;
       KSP.Dsc_PpPorez_D      = Fld_PpPorez_D     ;
       KSP.Dsc_PpPrirez_D     = Fld_PpPrirez_D    ;
       KSP.Dsc_PpMIO_I_D      = Fld_PpMIO_I_D     ;
       KSP.Dsc_PpMIO_II_D     = Fld_PpMIO_II_D    ;
       KSP.Dsc_PpZdrOsig_D    = Fld_PpZdrOsig_D   ;
       KSP.Dsc_PpZOR_D        = Fld_PpZOR_D       ;
       KSP.Dsc_PpZPI_D        = Fld_PpZPI_D       ;
       KSP.Dsc_PpZAP_D        = Fld_PpZAP_D       ;
       KSP.Dsc_PpZPP_D        = Fld_PpZPP_D       ;
       KSP.Dsc_PpNETO_D       = Fld_PpNETO_D      ;
       KSP.Dsc_PpPREVOZ_D     = Fld_PpPREVOZ_D    ;
       KSP.Dsc_PpMIO_1Na_D    = Fld_PpMIO_1Na_D   ;
       KSP.Dsc_PpMIO_2Na_D    = Fld_PpMIO_2Na_D   ;
       KSP.Dsc_PpPorez_P      = Fld_PpPorez_P     ;
       KSP.Dsc_PpPrirez_P     = Fld_PpPrirez_P    ;
       KSP.Dsc_PpMIO_I_P      = Fld_PpMIO_I_P     ;
       KSP.Dsc_PpMIO_II_P     = Fld_PpMIO_II_P    ;
       KSP.Dsc_PpZdrOsig_P    = Fld_PpZdrOsig_P   ;
       KSP.Dsc_PpZOR_P        = Fld_PpZOR_P       ;
       KSP.Dsc_PpZPI_P        = Fld_PpZPI_P       ;
       KSP.Dsc_PpZAP_P        = Fld_PpZAP_P       ;
       KSP.Dsc_PpZPP_P        = Fld_PpZPP_P       ;
       KSP.Dsc_PpNETO_P       = Fld_PpNETO_P      ;
       KSP.Dsc_PpPREVOZ_P     = Fld_PpPREVOZ_P    ;
       KSP.Dsc_PpMIO_1Na_P    = Fld_PpMIO_1Na_P   ;
       KSP.Dsc_PpMIO_2Na_P    = Fld_PpMIO_2Na_P   ;
       KSP.Dsc_UdPorez_D      = Fld_UdPorez_D     ;
       KSP.Dsc_UdPrirez_D     = Fld_UdPrirez_D    ;
       KSP.Dsc_UdMIO_I_D      = Fld_UdMIO_I_D     ;
       KSP.Dsc_UdMIO_II_D     = Fld_UdMIO_II_D    ;
       KSP.Dsc_UdZdrOsig_D    = Fld_UdZdrOsig_D   ;
       KSP.Dsc_UdZPI_D        = Fld_UdZPI_D       ;
       KSP.Dsc_UdNETO_D       = Fld_UdNETO_D      ;
       KSP.Dsc_UdPorez_P      = Fld_UdPorez_P     ;
       KSP.Dsc_UdPrirez_P     = Fld_UdPrirez_P    ;
       KSP.Dsc_UdMIO_I_P      = Fld_UdMIO_I_P     ;
       KSP.Dsc_UdMIO_II_P     = Fld_UdMIO_II_P    ;
       KSP.Dsc_UdZdrOsig_P    = Fld_UdZdrOsig_P   ;
       KSP.Dsc_UdZPI_P        = Fld_UdZPI_P       ;
       KSP.Dsc_UdNETO_P       = Fld_UdNETO_P      ;
       KSP.Dsc_NoPorez_D      = Fld_NoPorez_D     ;
       KSP.Dsc_NoPrirez_D     = Fld_NoPrirez_D    ;
       KSP.Dsc_NoMIO_I_D      = Fld_NoMIO_I_D     ;
       KSP.Dsc_NoMIO_II_D     = Fld_NoMIO_II_D    ;
       KSP.Dsc_NoZdrOsig_D    = Fld_NoZdrOsig_D   ;
       KSP.Dsc_NoZPI_D        = Fld_NoZPI_D       ;
       KSP.Dsc_NoNETO_D       = Fld_NoNETO_D      ;
       KSP.Dsc_NoPorez_P      = Fld_NoPorez_P     ;
       KSP.Dsc_NoPrirez_P     = Fld_NoPrirez_P    ;
       KSP.Dsc_NoMIO_I_P      = Fld_NoMIO_I_P     ;
       KSP.Dsc_NoMIO_II_P     = Fld_NoMIO_II_P    ;
       KSP.Dsc_NoZdrOsig_P    = Fld_NoZdrOsig_P   ;
       KSP.Dsc_NoZPI_P        = Fld_NoZPI_P       ;
       KSP.Dsc_NoNETO_P       = Fld_NoNETO_P      ;
       KSP.Dsc_AhPorez_D      = Fld_AhPorez_D     ;
       KSP.Dsc_AhPrirez_D     = Fld_AhPrirez_D    ;
       KSP.Dsc_AhZPI_D        = Fld_AhZPI_D       ;
       KSP.Dsc_AhNETO_D       = Fld_AhNETO_D      ;
       KSP.Dsc_AhPorez_P      = Fld_AhPorez_P     ;
       KSP.Dsc_AhPrirez_P     = Fld_AhPrirez_P    ;
       KSP.Dsc_AhZPI_P        = Fld_AhZPI_P       ;
       KSP.Dsc_AhNETO_P       = Fld_AhNETO_P      ;
       KSP.Dsc_SnPorez_D      = Fld_SnPorez_D     ;
       KSP.Dsc_SnPrirez_D     = Fld_SnPrirez_D    ;
       KSP.Dsc_SnZPI_D        = Fld_SnZPI_D       ;
       KSP.Dsc_SnNETO_D       = Fld_SnNETO_D      ;
       KSP.Dsc_SnPorez_P      = Fld_SnPorez_P     ;
       KSP.Dsc_SnPrirez_P     = Fld_SnPrirez_P    ;
       KSP.Dsc_SnZPI_P        = Fld_SnZPI_P       ;
       KSP.Dsc_SnNETO_P       = Fld_SnNETO_P      ;
       KSP.Dsc_NePorez_D      = Fld_NePorez_D     ;
       KSP.Dsc_NePrirez_D     = Fld_NePrirez_D    ;
       KSP.Dsc_NeMIO_I_D      = Fld_NeMIO_I_D     ;
       KSP.Dsc_NeMIO_II_D     = Fld_NeMIO_II_D    ;
       KSP.Dsc_NeZdrOsig_D    = Fld_NeZdrOsig_D   ;
       KSP.Dsc_NeNETO_D       = Fld_NeNETO_D      ;
       KSP.Dsc_NePorez_P      = Fld_NePorez_P     ;
       KSP.Dsc_NePrirez_P     = Fld_NePrirez_P    ;
       KSP.Dsc_NeMIO_I_P      = Fld_NeMIO_I_P     ;
       KSP.Dsc_NeMIO_II_P     = Fld_NeMIO_II_P    ;
       KSP.Dsc_NeZdrOsig_P    = Fld_NeZdrOsig_P   ;
       KSP.Dsc_NeNETO_P       = Fld_NeNETO_P      ;
       KSP.Dsc_StPorez_D      = Fld_StPorez_D     ;
       KSP.Dsc_StPrirez_D     = Fld_StPrirez_D    ;
       KSP.Dsc_StMIO_oo_D     = Fld_StMIO_oo_D    ;
       KSP.Dsc_StZOs_zzr_D    = Fld_StZOs_zzr_D   ;
       KSP.Dsc_StZPI_D        = Fld_StZPI_D       ;
       KSP.Dsc_StNETO_D       = Fld_StNETO_D      ;
       KSP.Dsc_StPorez_P      = Fld_StPorez_P     ;
       KSP.Dsc_StPrirez_P     = Fld_StPrirez_P    ;
       KSP.Dsc_StMIO_oo_P     = Fld_StMIO_oo_P    ;
       KSP.Dsc_StZOs_zzr_P    = Fld_StZOs_zzr_P   ;
       KSP.Dsc_StZPI_P        = Fld_StZPI_P       ;
       KSP.Dsc_StNETO_P       = Fld_StNETO_P      ;
       KSP.Dsc_SzPorez_D      = Fld_SzPorez_D     ;
       KSP.Dsc_SzPrirez_D     = Fld_SzPrirez_D    ;
       KSP.Dsc_SzNETO_D       = Fld_SzNETO_D      ;
       KSP.Dsc_SzPorez_P      = Fld_SzPorez_P     ;
       KSP.Dsc_SzPrirez_P     = Fld_SzPrirez_P    ;
       KSP.Dsc_SzNETO_P       = Fld_SzNETO_P      ;
       KSP.Dsc_TvPorez_D      = Fld_TvPorez_D     ;
       KSP.Dsc_TvPrirez_D     = Fld_TvPrirez_D    ;
       KSP.Dsc_TvMIO_I_D      = Fld_TvMIO_I_D     ;
       KSP.Dsc_TvMIO_II_D     = Fld_TvMIO_II_D    ;
       KSP.Dsc_TvZdrOsig_D    = Fld_TvZdrOsig_D   ;
       KSP.Dsc_TvZPI_D        = Fld_TvZPI_D       ;
       KSP.Dsc_TvNETO_D       = Fld_TvNETO_D      ;
       KSP.Dsc_TvPorez_P      = Fld_TvPorez_P     ;
       KSP.Dsc_TvPrirez_P     = Fld_TvPrirez_P    ;
       KSP.Dsc_TvMIO_I_P      = Fld_TvMIO_I_P     ;
       KSP.Dsc_TvMIO_II_P     = Fld_TvMIO_II_P    ;
       KSP.Dsc_TvZdrOsig_P    = Fld_TvZdrOsig_P   ;
       KSP.Dsc_TvZPI_P        = Fld_TvZPI_P       ;
       KSP.Dsc_TvNETO_P       = Fld_TvNETO_P      ;
       KSP.Dsc_PozicijaAH     = Fld_PozicijaAH    ;
       KSP.Dsc_PozicijaNO     = Fld_PozicijaNO    ;
       KSP.Dsc_PozicijaUD     = Fld_PozicijaUD    ;
       KSP.Dsc_PozicijaSO     = Fld_PozicijaSO    ;
       KSP.Dsc_PozicijaTV     = Fld_PozicijaTV    ;
       KSP.Dsc_PozicijaRR     = Fld_PozicijaRR    ;
       KSP.Dsc_CR_KfBr1       = Fld_CR_KfBr1      ;
       KSP.Dsc_CR_StD1        = Fld_CR_StD1       ;
       KSP.Dsc_CR_StD2        = Fld_CR_StD2       ;
       KSP.Dsc_CR_DodBruto    = Fld_CR_DodBruto   ;
       KSP.Dsc_CR_PolDodBrt   = Fld_CR_PolDodBrt  ;
       KSP.Dsc_CR_StDodStz    = Fld_CR_StDodStz   ;
       KSP.Dsc_CR_IsVrKfBr1   = Fld_CR_IsVrKfBrt1 ;
       KSP.Dsc_CR_VrKfBr1     = Fld_CR_VrKfBr1    ;
       KSP.Dsc_CR_BrutoKorekc = Fld_CR_BrutoKorekc;
       KSP.Dsc_MIO_I_SD_D     = Fld_SD_MIO_I_D    ; 
       KSP.Dsc_MIO_II_SD_D    = Fld_SD_MIO_II_D   ;
       KSP.Dsc_ZdrOsig_SD_D   = Fld_SD_ZdrOsig_D  ;
       KSP.Dsc_ZOR_SD_D       = Fld_SD_ZOR_D      ;
       KSP.Dsc_ZAP_SD_D       = Fld_SD_ZAP_D      ;
       KSP.Dsc_MIO_I_SD_P     = Fld_SD_MIO_I_P    ;
       KSP.Dsc_MIO_II_SD_P    = Fld_SD_MIO_II_P   ;
       KSP.Dsc_ZdrOsig_SD_P   = Fld_SD_ZdrOsig_P  ;
       KSP.Dsc_ZOR_SD_P       = Fld_SD_ZOR_P      ;
       KSP.Dsc_ZAP_SD_P       = Fld_SD_ZAP_p      ;
       KSP.Dsc_IsObustOsnDoh  = Fld_IsObustOsnDoh ;
       KSP.Dsc_IsNPnaRR_U_mj  = Fld_IsNPnaRR_U_mj;
       KSP.Dsc_AhMIO_I_P      = Fld_AhMIO_I_P     ;
       KSP.Dsc_AhMIO_II_P     = Fld_AhMIO_II_P    ;
       KSP.Dsc_AhZdrOsig_P    = Fld_AhZdrOsig_P   ;
       KSP.Dsc_AhMIO_I_D      = Fld_AhMIO_I_D     ;
       KSP.Dsc_AhMIO_II_D     = Fld_AhMIO_II_D    ;
       KSP.Dsc_AhZdrOsig_D    = Fld_AhZdrOsig_D   ;
       KSP.Dsc_NP63_nagr_D    = Fld_NP63_nagr_D   ;
       KSP.Dsc_NP65_preh_D    = Fld_NP65_preh_D   ;
       KSP.Dsc_NP71_dZdr_D    = Fld_NP71_dZdr_D   ;
       KSP.Dsc_NP63_nagr_P    = Fld_NP63_nagr_P   ;
       KSP.Dsc_NP65_preh_P    = Fld_NP65_preh_P   ;
       KSP.Dsc_NP71_dZdr_P    = Fld_NP71_dZdr_P   ;
       KSP.Dsc_NP17_dnev_D    = Fld_NP17_dnev_D   ;
       KSP.Dsc_NP18_loko_D    = Fld_NP18_loko_D   ;
       KSP.Dsc_NP21_darD_D    = Fld_NP21_darD_D   ;
       KSP.Dsc_NP22_regB_D    = Fld_NP22_regB_D   ;
       KSP.Dsc_NP26_otpr_D    = Fld_NP26_otpr_D   ;
       KSP.Dsc_NP60_jubi_D    = Fld_NP60_jubi_D   ;
       KSP.Dsc_NP17_dnev_P    = Fld_NP17_dnev_P   ;
       KSP.Dsc_NP18_loko_P    = Fld_NP18_loko_P   ;
       KSP.Dsc_NP21_darD_P    = Fld_NP21_darD_P   ;
       KSP.Dsc_NP22_regB_P    = Fld_NP22_regB_P   ;
       KSP.Dsc_NP26_otpr_P    = Fld_NP26_otpr_P   ;
       KSP.Dsc_NP60_jubi_P    = Fld_NP60_jubi_P   ;

       KSP.SaveDscToLookUpItemList();

   }

   #endregion PutShemaKontoFields(), GetShemaKontoFields()
}

public class KtoShemaPlacaDsc : VvLookupAsDsc
{
   #region DataLayer Propertiz

   public string Dsc_Porez_D        { get; set; }
   public string Dsc_Prirez_D       { get; set; }
   public string Dsc_MIO_I_D        { get; set; }
   public string Dsc_MIO_II_D       { get; set; }
   public string Dsc_ZdrOsig_D      { get; set; }
   public string Dsc_ZOR_D          { get; set; }
   public string Dsc_ZPI_D          { get; set; }
   public string Dsc_ZAP_D          { get; set; }
   public string Dsc_ZPP_D          { get; set; }
   public string Dsc_NETO_D         { get; set; }
   public string Dsc_PREVOZ_D       { get; set; }
   public string Dsc_MIO_1Na_D      { get; set; }
   public string Dsc_MIO_2Na_D      { get; set; }
   public string Dsc_Porez_P        { get; set; }
   public string Dsc_Prirez_P       { get; set; }
   public string Dsc_MIO_I_P        { get; set; }
   public string Dsc_MIO_II_P       { get; set; }
   public string Dsc_ZdrOsig_P      { get; set; }
   public string Dsc_ZOR_P          { get; set; }
   public string Dsc_ZPI_P          { get; set; }
   public string Dsc_ZAP_P          { get; set; }
   public string Dsc_ZPP_P          { get; set; }
   public string Dsc_NETO_P         { get; set; }
   public string Dsc_PREVOZ_P       { get; set; }
   public string Dsc_MIO_1Na_P      { get; set; }
   public string Dsc_MIO_2Na_P      { get; set; }
   public string Dsc_PpPorez_D      { get; set; }
   public string Dsc_PpPrirez_D     { get; set; }
   public string Dsc_PpMIO_I_D      { get; set; }
   public string Dsc_PpMIO_II_D     { get; set; }
   public string Dsc_PpZdrOsig_D    { get; set; }
   public string Dsc_PpZOR_D        { get; set; }
   public string Dsc_PpZPI_D        { get; set; }
   public string Dsc_PpZAP_D        { get; set; }
   public string Dsc_PpZPP_D        { get; set; }
   public string Dsc_PpNETO_D       { get; set; }
   public string Dsc_PpPREVOZ_D     { get; set; }
   public string Dsc_PpMIO_1Na_D    { get; set; }
   public string Dsc_PpMIO_2Na_D    { get; set; }
   public string Dsc_PpPorez_P      { get; set; }
   public string Dsc_PpPrirez_P     { get; set; }
   public string Dsc_PpMIO_I_P      { get; set; }
   public string Dsc_PpMIO_II_P     { get; set; }
   public string Dsc_PpZdrOsig_P    { get; set; }
   public string Dsc_PpZOR_P        { get; set; }
   public string Dsc_PpZPI_P        { get; set; }
   public string Dsc_PpZAP_P        { get; set; }
   public string Dsc_PpZPP_P        { get; set; }
   public string Dsc_PpNETO_P       { get; set; }
   public string Dsc_PpPREVOZ_P     { get; set; }
   public string Dsc_PpMIO_1Na_P    { get; set; }
   public string Dsc_PpMIO_2Na_P    { get; set; }
   public string Dsc_UdPorez_D      { get; set; }
   public string Dsc_UdPrirez_D     { get; set; }
   public string Dsc_UdMIO_I_D      { get; set; }
   public string Dsc_UdMIO_II_D     { get; set; }
   public string Dsc_UdZdrOsig_D    { get; set; }
   public string Dsc_UdZPI_D        { get; set; }
   public string Dsc_UdNETO_D       { get; set; }
   public string Dsc_UdPorez_P      { get; set; }
   public string Dsc_UdPrirez_P     { get; set; }
   public string Dsc_UdMIO_I_P      { get; set; }
   public string Dsc_UdMIO_II_P     { get; set; }
   public string Dsc_UdZdrOsig_P    { get; set; }
   public string Dsc_UdZPI_P        { get; set; }
   public string Dsc_UdNETO_P       { get; set; }
   public string Dsc_NoPorez_D      { get; set; }
   public string Dsc_NoPrirez_D     { get; set; }
   public string Dsc_NoMIO_I_D      { get; set; }
   public string Dsc_NoMIO_II_D     { get; set; }
   public string Dsc_NoZdrOsig_D    { get; set; }
   public string Dsc_NoZPI_D        { get; set; }
   public string Dsc_NoNETO_D       { get; set; }
   public string Dsc_NoPorez_P      { get; set; }
   public string Dsc_NoPrirez_P     { get; set; }
   public string Dsc_NoMIO_I_P      { get; set; }
   public string Dsc_NoMIO_II_P     { get; set; }
   public string Dsc_NoZdrOsig_P    { get; set; }
   public string Dsc_NoZPI_P        { get; set; }
   public string Dsc_NoNETO_P       { get; set; }
   public string Dsc_AhPorez_D      { get; set; }
   public string Dsc_AhPrirez_D     { get; set; }
   public string Dsc_AhZPI_D        { get; set; }
   public string Dsc_AhNETO_D       { get; set; }
   public string Dsc_AhPorez_P      { get; set; }
   public string Dsc_AhPrirez_P     { get; set; }
   public string Dsc_AhZPI_P        { get; set; }
   public string Dsc_AhNETO_P       { get; set; }
   public string Dsc_SnPorez_D      { get; set; }
   public string Dsc_SnPrirez_D     { get; set; }
   public string Dsc_SnZPI_D        { get; set; }
   public string Dsc_SnNETO_D       { get; set; }
   public string Dsc_SnPorez_P      { get; set; }
   public string Dsc_SnPrirez_P     { get; set; }
   public string Dsc_SnZPI_P        { get; set; }
   public string Dsc_SnNETO_P       { get; set; }
   public string Dsc_NePorez_D      { get; set; }
   public string Dsc_NePrirez_D     { get; set; }
   public string Dsc_NeMIO_I_D      { get; set; }
   public string Dsc_NeMIO_II_D     { get; set; }
   public string Dsc_NeZdrOsig_D    { get; set; }
   public string Dsc_NeNETO_D       { get; set; }
   public string Dsc_NePorez_P      { get; set; }
   public string Dsc_NePrirez_P     { get; set; }
   public string Dsc_NeMIO_I_P      { get; set; }
   public string Dsc_NeMIO_II_P     { get; set; }
   public string Dsc_NeZdrOsig_P    { get; set; }
   public string Dsc_NeNETO_P       { get; set; }
   public string Dsc_StPorez_D      { get; set; }
   public string Dsc_StPrirez_D     { get; set; }
   public string Dsc_StMIO_oo_D     { get; set; }
   public string Dsc_StZOs_zzr_D    { get; set; }
   public string Dsc_StZPI_D        { get; set; }
   public string Dsc_StNETO_D       { get; set; }
   public string Dsc_StPorez_P      { get; set; }
   public string Dsc_StPrirez_P     { get; set; }
   public string Dsc_StMIO_oo_P     { get; set; }
   public string Dsc_StZOs_zzr_P    { get; set; }
   public string Dsc_StZPI_P        { get; set; }
   public string Dsc_StNETO_P       { get; set; }
   public string Dsc_SzPorez_D      { get; set; }
   public string Dsc_SzPrirez_D     { get; set; }
   public string Dsc_SzNETO_D       { get; set; }
   public string Dsc_SzPorez_P      { get; set; }
   public string Dsc_SzPrirez_P     { get; set; }
   public string Dsc_SzNETO_P       { get; set; }
   public string Dsc_TvPorez_D      { get; set; } 
   public string Dsc_TvPrirez_D     { get; set; }
   public string Dsc_TvMIO_I_D      { get; set; }
   public string Dsc_TvMIO_II_D     { get; set; }
   public string Dsc_TvZdrOsig_D    { get; set; }
   public string Dsc_TvZPI_D        { get; set; }
   public string Dsc_TvNETO_D       { get; set; }
   public string Dsc_TvPorez_P      { get; set; }
   public string Dsc_TvPrirez_P     { get; set; }
   public string Dsc_TvMIO_I_P      { get; set; }
   public string Dsc_TvMIO_II_P     { get; set; }
   public string Dsc_TvZdrOsig_P    { get; set; }
   public string Dsc_TvZPI_P        { get; set; }
   public string Dsc_TvNETO_P       { get; set; }
   public string Dsc_PozicijaAH     { get; set; }
   public string Dsc_PozicijaNO     { get; set; }
   public string Dsc_PozicijaUD     { get; set; }
   public string Dsc_PozicijaSO     { get; set; }
   public string Dsc_PozicijaTV     { get; set; }
   public string Dsc_PozicijaRR     { get; set; }
   public string Dsc_CR_KfBr1       { get; set; }
   public string Dsc_CR_StD1        { get; set; }
   public string Dsc_CR_StD2        { get; set; }
   public string Dsc_CR_DodBruto    { get; set; }
   public string Dsc_CR_PolDodBrt   { get; set; }
   public string Dsc_CR_StDodStz    { get; set; }
   public string Dsc_CR_VrKfBr1     { get; set; }
   public bool   Dsc_CR_IsVrKfBr1   { get; set; }
   public string Dsc_CR_BrutoKorekc { get; set; }
   public string Dsc_MIO_I_SD_D     { get; set; }
   public string Dsc_MIO_II_SD_D    { get; set; }
   public string Dsc_ZdrOsig_SD_D   { get; set; }
   public string Dsc_ZOR_SD_D       { get; set; }
   public string Dsc_ZAP_SD_D       { get; set; }
   public string Dsc_MIO_I_SD_P     { get; set; }
   public string Dsc_MIO_II_SD_P    { get; set; }
   public string Dsc_ZdrOsig_SD_P   { get; set; }
   public string Dsc_ZOR_SD_P       { get; set; }
   public string Dsc_ZAP_SD_P       { get; set; }
   public bool   Dsc_IsObustOsnDoh  { get; set; }
   public bool   Dsc_IsNPnaRR_U_mj  { get; set; }
   public string Dsc_AhMIO_I_P      { get; set; }
   public string Dsc_AhMIO_II_P     { get; set; }
   public string Dsc_AhZdrOsig_P    { get; set; }
   public string Dsc_AhMIO_I_D      { get; set; }
   public string Dsc_AhMIO_II_D     { get; set; }
   public string Dsc_AhZdrOsig_D    { get; set; }
   public string Dsc_NP63_nagr_D    { get; set; }
   public string Dsc_NP65_preh_D    { get; set; }
   public string Dsc_NP71_dZdr_D    { get; set; }
   public string Dsc_NP63_nagr_P    { get; set; }
   public string Dsc_NP65_preh_P    { get; set; }
   public string Dsc_NP71_dZdr_P    { get; set; }
   public string Dsc_NP17_dnev_D    { get; set; }
   public string Dsc_NP18_loko_D    { get; set; }
   public string Dsc_NP21_darD_D    { get; set; }
   public string Dsc_NP22_regB_D    { get; set; }
   public string Dsc_NP26_otpr_D    { get; set; }
   public string Dsc_NP60_jubi_D    { get; set; }
   public string Dsc_NP17_dnev_P    { get; set; }
   public string Dsc_NP18_loko_P    { get; set; }
   public string Dsc_NP21_darD_P    { get; set; }
   public string Dsc_NP22_regB_P    { get; set; }
   public string Dsc_NP26_otpr_P    { get; set; }
   public string Dsc_NP60_jubi_P    { get; set; }

   #endregion DataLayer Propertiz

   #region Constructor

   public KtoShemaPlacaDsc(VvLookUpLista vvLookUpLista) : base(vvLookUpLista)
   {
   }
   public KtoShemaPlacaDsc(VvLookUpLista vvLookUpLista, bool weNeedDefaultList) : base(vvLookUpLista, weNeedDefaultList)
   {
   }

   #endregion Constructor

   #region override SetDefaultValues

   public override void SetDefaultValues(VvLookUpLista luiList)
   {
      #region defautValue

      Dsc_Porez_D        = "4250";
      Dsc_Prirez_D       = "4251";
      Dsc_MIO_I_D        = "4270";
      Dsc_MIO_II_D       = "4271";
      Dsc_ZdrOsig_D      = "4260";
      Dsc_ZOR_D          = "4265";
      Dsc_ZPI_D          = "4266";
      Dsc_ZAP_D          = "4262";
      Dsc_ZPP_D          = "4263";
      Dsc_NETO_D         = "4200";
      Dsc_PREVOZ_D       = "4610";
      Dsc_MIO_1Na_D      = "42711";
      Dsc_MIO_2Na_D      = "42712";
      Dsc_Porez_P        = "2410";
      Dsc_Prirez_P       = "2411";
      Dsc_MIO_I_P        = "24204";
      Dsc_MIO_II_P       = "24205";
      Dsc_ZdrOsig_P      = "24200";
      Dsc_ZOR_P          = "24212";
      Dsc_ZPI_P          = "24213";
      Dsc_ZAP_P          = "24202";
      Dsc_ZPP_P          = "24203";
      Dsc_NETO_P         = "2300";
      Dsc_PREVOZ_P       = "2300";
      Dsc_MIO_1Na_P      = "24221";
      Dsc_MIO_2Na_P      = "24222";
      Dsc_PpPorez_D      = "4250";
      Dsc_PpPrirez_D     = "4251";
      Dsc_PpMIO_I_D      = "4270";
      Dsc_PpMIO_II_D     = "4271";
      Dsc_PpZdrOsig_D    = "4260";
      Dsc_PpZOR_D        = "4265";
      Dsc_PpZPI_D        = "4266";
      Dsc_PpZAP_D        = "4262";
      Dsc_PpZPP_D        = "4263";
      Dsc_PpNETO_D       = "4200";
      Dsc_PpPREVOZ_D     = "4610";
      Dsc_PpMIO_1Na_D    = "42711";
      Dsc_PpMIO_2Na_D    = "42712";
      Dsc_PpPorez_P      = "2410";
      Dsc_PpPrirez_P     = "2411";
      Dsc_PpMIO_I_P      = "24204";
      Dsc_PpMIO_II_P     = "24205";
      Dsc_PpZdrOsig_P    = "24200";
      Dsc_PpZOR_P        = "24212";
      Dsc_PpZPI_P        = "24213";
      Dsc_PpZAP_P        = "24202";
      Dsc_PpZPP_P        = "24203";
      Dsc_PpNETO_P       = "2300";
      Dsc_PpPREVOZ_P     = "2300";
      Dsc_PpMIO_1Na_P    = "24221";
      Dsc_PpMIO_2Na_P    = "24222";
      Dsc_UdPorez_D      = "4250";
      Dsc_UdPrirez_D     = "4251";
      Dsc_UdMIO_I_D      = "4270";
      Dsc_UdMIO_II_D     = "4271";
      Dsc_UdZdrOsig_D    = "4260";
      Dsc_UdZPI_D        = "4266";
      Dsc_UdNETO_D       = "4200";
      Dsc_UdPorez_P      = "2410";
      Dsc_UdPrirez_P     = "2411";
      Dsc_UdMIO_I_P      = "24204";
      Dsc_UdMIO_II_P     = "24205";
      Dsc_UdZdrOsig_P    = "24200";
      Dsc_UdZPI_P        = "24213";
      Dsc_UdNETO_P       = "2300" ;
      Dsc_NoPorez_D      = "4250";
      Dsc_NoPrirez_D     = "4251";
      Dsc_NoMIO_I_D      = "4270";
      Dsc_NoMIO_II_D     = "4271";
      Dsc_NoZdrOsig_D    = "4260";
      Dsc_NoZPI_D        = "4266";
      Dsc_NoNETO_D       = "4200";
      Dsc_NoPorez_P      = "2410";
      Dsc_NoPrirez_P     = "2411";
      Dsc_NoMIO_I_P      = "24204";
      Dsc_NoMIO_II_P     = "24205";
      Dsc_NoZdrOsig_P    = "24200";
      Dsc_NoZPI_P        = "24213";
      Dsc_NoNETO_P       = "2300" ;
      Dsc_AhPorez_D      = "";
      Dsc_AhPrirez_D     = "";
      Dsc_AhZPI_D        = "";
      Dsc_AhNETO_D       = "";
      Dsc_AhPorez_P      = "";
      Dsc_AhPrirez_P     = "";
      Dsc_AhZPI_P        = "";
      Dsc_AhNETO_P       = "";
      Dsc_SnPorez_D      = "";
      Dsc_SnPrirez_D     = "";
      Dsc_SnZPI_D        = "";
      Dsc_SnNETO_D       = "";
      Dsc_SnPorez_P      = "";
      Dsc_SnPrirez_P     = "";
      Dsc_SnZPI_P        = "";
      Dsc_SnNETO_P       = "";
      Dsc_NePorez_D      = "";
      Dsc_NePrirez_D     = "";
      Dsc_NeMIO_I_D      = "";
      Dsc_NeMIO_II_D     = "";
      Dsc_NeZdrOsig_D    = "";
      Dsc_NeNETO_D       = "";
      Dsc_NePorez_P      = "";
      Dsc_NePrirez_P     = "";
      Dsc_NeMIO_I_P      = "";
      Dsc_NeMIO_II_P     = "";
      Dsc_NeZdrOsig_P    = "";
      Dsc_NeNETO_P       = "";
      Dsc_StPorez_D      = "";
      Dsc_StPrirez_D     = "";
      Dsc_StMIO_oo_D     = "";
      Dsc_StZOs_zzr_D    = "";
      Dsc_StZPI_D        = "";
      Dsc_StNETO_D       = "";
      Dsc_StPorez_P      = "";
      Dsc_StPrirez_P     = "";
      Dsc_StMIO_oo_P     = "";
      Dsc_StZOs_zzr_P    = "";
      Dsc_StZPI_P        = "";
      Dsc_StNETO_P       = "";
      Dsc_SzPorez_D      = "";
      Dsc_SzPrirez_D     = "";
      Dsc_SzNETO_D       = "";
      Dsc_SzPorez_P      = "";
      Dsc_SzPrirez_P     = "";
      Dsc_SzNETO_P       = "";
      Dsc_TvPorez_D      = ""; 
      Dsc_TvPrirez_D     = ""; 
      Dsc_TvMIO_I_D      = ""; 
      Dsc_TvMIO_II_D     = ""; 
      Dsc_TvZdrOsig_D    = ""; 
      Dsc_TvZPI_D        = ""; 
      Dsc_TvNETO_D       = ""; 
      Dsc_TvPorez_P      = ""; 
      Dsc_TvPrirez_P     = ""; 
      Dsc_TvMIO_I_P      = ""; 
      Dsc_TvMIO_II_P     = ""; 
      Dsc_TvZdrOsig_P    = ""; 
      Dsc_TvZPI_P        = ""; 
      Dsc_TvNETO_P       = ""; 
      Dsc_PozicijaAH     = ""; 
      Dsc_PozicijaNO     = ""; 
      Dsc_PozicijaUD     = ""; 
      Dsc_PozicijaSO     = ""; 
      Dsc_PozicijaTV     = ""; 
      Dsc_PozicijaRR     = "";
      Dsc_CR_KfBr1       = "Koeficijent"; 
      Dsc_CR_StD1        = ""; 
      Dsc_CR_StD2        = ""; 
      Dsc_CR_DodBruto    = "Bruto dodatak"; 
      Dsc_CR_PolDodBrt   = "Ostali dodatak"; 
      Dsc_CR_StDodStz    = "Bruto dodatak/stimulacija";
      Dsc_CR_VrKfBr1     = "Osnovica";
      Dsc_CR_IsVrKfBr1   = false;
      Dsc_CR_BrutoKorekc = "Korekcija";
      Dsc_MIO_I_SD_D       = "";
      Dsc_MIO_II_SD_D      = "";
      Dsc_ZdrOsig_SD_D     = "";
      Dsc_ZOR_SD_D         = "";
      Dsc_ZAP_SD_D         = "";
      Dsc_MIO_I_SD_P       = "";
      Dsc_MIO_II_SD_P      = "";
      Dsc_ZdrOsig_SD_P     = "";
      Dsc_ZOR_SD_P         = "";
      Dsc_ZAP_SD_P         = "";
      Dsc_IsObustOsnDoh    = false;
      Dsc_IsNPnaRR_U_mj    = false;
      Dsc_AhMIO_I_P        = "";
      Dsc_AhMIO_II_P       = "";
      Dsc_AhZdrOsig_P      = "";
      Dsc_AhMIO_I_D        = "";
      Dsc_AhMIO_II_D       = "";
      Dsc_AhZdrOsig_D      = "";
      Dsc_NP63_nagr_D      = "";
      Dsc_NP65_preh_D      = "";
      Dsc_NP71_dZdr_D      = "";
      Dsc_NP63_nagr_P      = "2300";
      Dsc_NP65_preh_P      = "2300";
      Dsc_NP71_dZdr_P      = "2300";
      Dsc_NP17_dnev_D      = "";
      Dsc_NP18_loko_D      = "";
      Dsc_NP21_darD_D      = "";
      Dsc_NP22_regB_D      = "";
      Dsc_NP26_otpr_D      = "";
      Dsc_NP60_jubi_D      = "";
      Dsc_NP17_dnev_P      = "2300";
      Dsc_NP18_loko_P      = "2300";
      Dsc_NP21_darD_P      = "2300";
      Dsc_NP22_regB_P      = "2300";
      Dsc_NP26_otpr_P      = "2300";
      Dsc_NP60_jubi_P      = "2300";

      #endregion defautValue
   }

   #endregion override SetDefaultValues
}

public class Placa_CalcOvrvRestdlg : VvDialog
{
   #region Fieldz

   private Button okButton, cancelButton;
   private int dlgWidth, dlgHeight;

   private VvHamper hamper;
   private VvTextBox tbx_personCD, tbx_prezime, tbx_neto, tbx_obustave, tbx_maxOvrv, tbx_iznosOvrv;

   #endregion Fieldz

   #region Constructor

   public Placa_CalcOvrvRestdlg(uint _personCD, string _prezimeIme, decimal _netto, decimal _obustave, decimal _maxOvrha)
   {
      this.StartPosition = FormStartPosition.CenterScreen;

      InitializeHamper(out hamper);

      dlgWidth        = hamper.Width + ZXC.Q6un;
      dlgHeight       = hamper.Height + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      okButton.BringToFront();

      this.Text = "Obračun preostalog neta za ovrhu";

      Fld_PersonCD   = _personCD;
      Fld_PrezimeIme = _prezimeIme;
      Fld_Neto       = _netto;
      Fld_Obustave   = _obustave;
      Fld_MaxOvrha   = 
      Fld_IznosOvrhe = _maxOvrha;
   }

   private void InitializeHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 6, "", this, false, ZXC.QUN, ZXC.QUN, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q10un, ZXC.Q4un, ZXC.QUN - ZXC.Qun4, ZXC.Q9un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                     hamper.CreateVvLabel  (0, 0, "Za radnika:", ContentAlignment.MiddleRight);
      tbx_personCD = hamper.CreateVvTextBox(1, 0, "tbx_personCD", "");
      tbx_prezime  = hamper.CreateVvTextBox(2, 0, "tbx_prezime", "", 32, 1, 0);

                 hamper.CreateVvLabel  (0, 1, "Neto:", ContentAlignment.MiddleRight);
      tbx_neto = hamper.CreateVvTextBox(1, 1, "tbx_neto", "", 12);
      tbx_neto.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_neto.JAM_DisableNegativeNumberValues = true;

                     hamper.CreateVvLabel  (0, 2, "Obustave:", ContentAlignment.MiddleRight);
      tbx_obustave = hamper.CreateVvTextBox(1, 2, "tbx_obustave", "", 12);
      tbx_obustave.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

                     hamper.CreateVvLabel  (0, 3, "Max iznos ovrhe (neto/3-obustave):", ContentAlignment.MiddleRight);
      tbx_maxOvrv  = hamper.CreateVvTextBox(1, 3, "tbx_maxOvrv", "", 12);
      tbx_maxOvrv.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

                      hamper.CreateVvLabel  (0, 4, "Željeni iznos ovrhe:", ContentAlignment.MiddleRight);
      tbx_iznosOvrv = hamper.CreateVvTextBox(1, 4, "tbx_iznosOvrv", "", 12);
      tbx_iznosOvrv.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      tbx_maxOvrv.JAM_ReadOnly =
      tbx_personCD  .JAM_ReadOnly =
      tbx_prezime   .JAM_ReadOnly = 
      tbx_neto      .JAM_ReadOnly = 
      tbx_obustave  .JAM_ReadOnly = true;


      VvHamper.Open_Close_Fields_ForWriting(tbx_personCD , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_prezime  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_neto     , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_obustave , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_maxOvrv  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_iznosOvrv, ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvDialog);
   }

   #endregion Constructor

   #region Fld_

   public uint Fld_PersonCD { get { return ZXC.ValOrZero_UInt(tbx_personCD.Text); } set { tbx_personCD.Text = value.ToString("0000"); } }

   public string Fld_PrezimeIme { get { return tbx_prezime.Text; } set { tbx_prezime.Text = value; } }

   public decimal Fld_Neto       { get { return tbx_neto     .GetDecimalField(); } set { tbx_neto     .PutDecimalField(value); } }
   public decimal Fld_Obustave   { get { return tbx_obustave .GetDecimalField(); } set { tbx_obustave .PutDecimalField(value); } }
   public decimal Fld_MaxOvrha   { get { return tbx_maxOvrv  .GetDecimalField(); } set { tbx_maxOvrv  .PutDecimalField(value); } }
   public decimal Fld_IznosOvrhe { get { return tbx_iznosOvrv.GetDecimalField(); } set { tbx_iznosOvrv.PutDecimalField(value); } }

   #endregion Fld_

   #region Event cancelButton

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Event cancelButton

}
