using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public class SmdDUC : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_kupDobCd, tbx_kupDobTk, tbx_kupDobName, tbx_projektIdent;
   private VvHamper  hamp_Smd;

   #endregion Fieldz

   #region Constructor

   public SmdDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul)  : base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
        (Mixer.tt_colName, new string[] 
         { 
               Mixer.TT_SMD
         });
   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
      hamp_mjTroska.Visible = true;
      hamp_mjTroska.Location = new Point(hamp_napomena.Right, hamp_napomena.Top);

      InitializeHamper_Smd(out hamp_Smd);
      nextY = hamp_Smd.Bottom;
   }

   public void InitializeHamper_Smd(out VvHamper hamper)
   {
      hamper = new VvHamper(13, 3, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0        1          2        3         4               5                     6                   7                              8                         9                  10       
      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un, ZXC.Q3un, ZXC.Q4un, ZXC.Q5un, ZXC.QUN - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.Q2un - ZXC.Qun8 + ZXC.Qun2, ZXC.Q3un + ZXC.Qun4, ZXC.Q3un, ZXC.Q4un + ZXC.Qun2, ZXC.Q2un + ZXC.QUN - ZXC.Qun4, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,                 0 ,                 0, ZXC.Qun4 - ZXC.Qun8           , ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, 0 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel  (0, 0, "Projekt:", ContentAlignment.MiddleRight);
      tbx_ProjektCD = hamper.CreateVvTextBox(1, 0, "tbx_Projekt", "Projekt - ", GetDB_ColumnSize(DB_ci.projektCD), 1, 0);
      tbx_ProjektCD.JAM_SetAutoCompleteData(Faktur.recordName, Faktur.sorterTtNum.SortType, ZXC.AutoCompleteRestrictor.FAK_ExactTT_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Faktur_DUMMY), null);

      tbx_projektIdent = hamper.CreateVvTextBox(3, 0, "tbx_projektIdent", "", GetDB_ColumnSize(DB_ci.strE_256), 8, 0);
      tbx_projektIdent.JAM_ReadOnly = true;

      btn_proj = hamper.CreateVvButton(12, 0, new EventHandler(/*FakturDUC.*/GoToProjektCD_RISK_Dokument_Click), "");
      btn_proj.Name = "projekt";
      btn_proj.FlatStyle = FlatStyle.Flat;
      btn_proj.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_proj.Image = VvIco.TriangleBlue16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue2.ico")), 16, 16)*/.ToBitmap();
      btn_proj.Tag = 10;
      btn_proj.TabStop = false;

                       hamper.CreateVvLabel  (0, 1, "Partner:", ContentAlignment.MiddleRight);
      tbx_kupDobCd   = hamper.CreateVvTextBox(1, 1, "tbx_kupDobCd", "Sifra Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobCD));
      tbx_kupDobTk   = hamper.CreateVvTextBox(2, 1, "tbx_kupDobTk", "Tiker Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobTK));
      tbx_kupDobName = hamper.CreateVvTextBox(3, 1, "tbx_kupDobName", "Naziv Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobName), 1, 0);

      tbx_kupDobCd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_kupDobTk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_kupDobCd.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobTk.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyKupdobTextBoxLeave));

                      hamper.CreateVvLabel        ( 7, 1, "Veza1:",1, 0, ContentAlignment.MiddleRight);
      tbx_v1_tt     = hamper.CreateVvTextBoxLookUp( 9, 1, "tbx_v1_tt", "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v1_tt));
      tbx_v1_ttOpis = hamper.CreateVvTextBox      (10, 1, "tbx_v1_ttOpis", "Veza na interni dokument", 32);
      tbx_v1_ttNum  = hamper.CreateVvTextBox      (11, 1, "tbx_v1_ttNum", "Veza na interni dokument", 6/*GetDB_ColumnSize(DB_ci.iz_ttNum)*/);

      btn_v1TT = hamper.CreateVvButton(12, 1, new EventHandler(GoTo_MIXER_Dokument_Click), "");

      btn_v1TT.Name = "v1_TT";
      btn_v1TT.FlatStyle = FlatStyle.Flat;
      btn_v1TT.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_v1TT.Image = VvIco.TriangleBlue16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue2.ico")), 16, 16)*/.ToBitmap();
      btn_v1TT.Tag = 1;
      btn_v1TT.TabStop = false;

      tbx_v1_tt.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_v1_tt.JAM_Set_NOTobligatory_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_v1_tt.JAM_lui_NameTaker_JAM_Name = tbx_v1_ttOpis.JAM_Name;
      tbx_v1_ttOpis.JAM_ReadOnly = true;
      tbx_v1_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

                      hamper.CreateVvLabel        ( 7, 2, "Veza2:", 1, 0, ContentAlignment.MiddleRight);
      tbx_v2_tt     = hamper.CreateVvTextBoxLookUp( 9, 2, "tbx_v2_tt", "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v2_tt));
      tbx_v2_ttOpis = hamper.CreateVvTextBox      (10, 2, "tbx_v2_ttOpis", "Veza na interni dokument");
      tbx_v2_ttNum  = hamper.CreateVvTextBox      (11, 2, "tbx_v2_ttNum", "Veza na interni dokument" + " broj", 6/*GetDB_ColumnSize(DB_ci.na_ttNum)*/);

      btn_v2TT = hamper.CreateVvButton(12, 2, new EventHandler(GoTo_MIXER_Dokument_Click), "");
      btn_v2TT.Name = "v2_TT";

      btn_v2TT.FlatStyle = FlatStyle.Flat;
      btn_v2TT.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_v2TT.Image = VvIco.TriangleBlue16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue2.ico")), 16, 16)*/.ToBitmap();
      btn_v2TT.Tag = 2;
      btn_v2TT.TabStop = false;

      tbx_v2_tt.JAM_CharacterCasing = CharacterCasing.Upper;
      //tbx_v2_tt.JAM_Set_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_v2_tt.JAM_Set_NOTobligatory_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_v2_tt.JAM_lui_NameTaker_JAM_Name = tbx_v2_ttOpis.JAM_Name;
      tbx_v2_ttOpis.JAM_ReadOnly = true;

      tbx_v2_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

                       hamper.CreateVvLabel  (0, 2, "Link:", ContentAlignment.MiddleRight);
      tbx_externLink1 = hamper.CreateVvTextBox(1, 2, "tbx_externLink1", "Izvještaj sa puta - Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.strE_256), 3, 0);

      btn_goExLink1           = hamper.CreateVvButton(5, 2, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink1.Name      = "btn_goExLink1";
      btn_goExLink1.FlatStyle = FlatStyle.Flat;
      btn_goExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink1.Image     = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink1.Tag       = 1;
      btn_goExLink1.TabStop   = false;
      btn_goExLink1.Visible   = false;
      
      btn_openExLink1           = hamper.CreateVvButton(6, 2, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLink1.Name      = "btn_openExLink1";
      btn_openExLink1.FlatStyle = FlatStyle.Flat;
      btn_openExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLink1.Image   = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLink1.Tag     = 1;
      btn_openExLink1.TabStop = false;

   }

   void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      //if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.originalText)
      {
         originalText = tb.Text;
         kupdob_rec = VvUserControl.KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_KupDobName = kupdob_rec.Naziv;
            Fld_KupDobCd = kupdob_rec.KupdobCD/*RecID*/;
            Fld_KupDobTk = kupdob_rec.Ticker;
         }
         else
         {
            Fld_KupDobName = Fld_KupDobTk = Fld_KupDobCdAsTxt = "";
         }
      }
   }

   #endregion CreateSpecificHampers()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_personCD_CreateColumn      (ZXC.Q4un, "Šifra", "Šifra");
      T_kpdbNameA_50_CreateColumn  (ZXC.Q7un, "Prezime", "Prezime", false);
      T_kpdbNameB_50_CreateColumn  (ZXC.Q6un, "Ime", "Ime");
      T_kol_CreateColumn           (ZXC.Q3un, "Sati", "Sati", 2);
      T_opis_128_CreateColumn      (0, "Komentar", "Komentar", 128,  null);

      vvtbT_kpdbNameA_50.JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonDgvTextBoxLeave));
      vvtbT_kpdbNameB_50.JAM_ReadOnly = true;

      vvtbT_kol.JAM_ShouldSumGrid = true;
   }

   #endregion InitializeTheGrid_Columns()

   #region Fld_

   public uint   Fld_KupDobCd       { get { return tbx_kupDobCd   .GetSomeRecIDField()   ; }  set { tbx_kupDobCd    .PutSomeRecIDField(value)       ; } }
   public string Fld_KupDobCdAsTxt  { get { return tbx_kupDobCd   .Text                  ; }  set { tbx_kupDobCd    .Text = value                   ; } }
   public string Fld_KupDobName     { get { return tbx_kupDobName .Text                  ; }  set { tbx_kupDobName  .Text = value                   ; } }
   public string Fld_KupDobTk       { get { return tbx_kupDobTk   .Text                  ; }  set { tbx_kupDobTk    .Text = value                   ; } }
   public string Fld_PrjIdent       {                                                         set { tbx_projektIdent.Text = value                   ; } }

   #endregion _Fld

   #region override PutSpecificsFld() GetSpecificsFld()

   /*protected*/public override void PutSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd   )) Fld_KupDobCd   = mixer_rec.KupdobCD;
      if(CtrlOK(tbx_kupDobName )) Fld_KupDobName = mixer_rec.KupdobName;
      if(CtrlOK(tbx_kupDobTk   )) Fld_KupDobTk   = mixer_rec.KupdobTK;
      if(CtrlOK(tbx_ProjektCD  )) Fld_ProjektCD  = mixer_rec.ProjektCD;
                                  Fld_TtOpis     = ZXC.luiListaMixTypeZahtjev.GetNameForThisCd(mixer_rec.TT);

      if(CtrlOK(tbx_v1_tt      )) Fld_V1_tt       = mixer_rec.V1_tt;
      if(CtrlOK(tbx_v1_ttOpis  )) Fld_V1_ttOpis   = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V1_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v1_ttNum   )) Fld_V1_ttNum    = mixer_rec.V1_ttNum;
      if(CtrlOK(tbx_v2_tt      )) Fld_V2_tt       = mixer_rec.V2_tt;
      if(CtrlOK(tbx_v2_ttOpis  )) Fld_V2_ttOpis   = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V2_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v2_ttNum   )) Fld_V2_ttNum    = mixer_rec.V2_ttNum;
      if(CtrlOK(tbx_externLink1)) Fld_ExternLink1 = mixer_rec.ExternLink1;

      if(CtrlOK(tbx_projektIdent))
      {
         mixer_rec.prjFaktur_rec_LOADED = false;

         Fld_PrjIdent = mixer_rec.ProjektIdent1;
      }

   }

   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd   )) mixer_rec.KupdobCD    = Fld_KupDobCd;
      if(CtrlOK(tbx_kupDobName )) mixer_rec.KupdobName  = Fld_KupDobName;
      if(CtrlOK(tbx_kupDobTk   )) mixer_rec.KupdobTK    = Fld_KupDobTk;
      if(CtrlOK(tbx_ProjektCD  )) mixer_rec.ProjektCD   = Fld_ProjektCD;
      if(CtrlOK(tbx_v1_tt      )) mixer_rec.V1_tt       = Fld_V1_tt;
      if(CtrlOK(tbx_v1_ttNum   )) mixer_rec.V1_ttNum    = Fld_V1_ttNum;
      if(CtrlOK(tbx_v2_tt      )) mixer_rec.V2_tt       = Fld_V2_tt;
      if(CtrlOK(tbx_v2_ttNum   )) mixer_rec.V2_ttNum    = Fld_V2_ttNum;
      if(CtrlOK(tbx_externLink1)) mixer_rec.ExternLink1 = Fld_ExternLink1;
   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region PrintDocumentRecord

   public SmdFilterUC TheSmdFilterUC { get; set; }
   public SmdFilter   TheSmdFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.TheSmdFilter = new SmdFilter(this);

      TheSmdFilterUC = new SmdFilterUC(this);
      TheSmdFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheSmdFilterUC.Width;

   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      SmdFilter mixerFilter = (SmdFilter)vvRptFilter;

      switch(mixerFilter.PrintSmd)
      {
         case SmdFilter.PrintSmdEnum.SMD: specificMixerReport = new RptX_SMD(new Vektor.Reports.XIZ.CR_SMD(), "SMD", mixerFilter); break;

         default: ZXC.aim_emsg("{0}\nPrintSomeSmdDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintSmd); return null;
      }

      return specificMixerReport;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheSmdFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheSmdFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
   }


   #endregion PrintDocumentRecord

   #region Colors
   
   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_SMD, Color.Empty, clr_SMD);
   }

   #endregion Colors

   #region overrideTabPageTitle

   public override string TabPageTitle1
   {
      get { return "Obračun"; }
   }
   //public override string TabPageTitle2
   //{
   //   get { return "Obračun ostalih troškova"; }
   //}

   #endregion overrideTabPageTitle

}

public class SmdFilterUC : VvFilterUC
{
   #region Fieldz

   #endregion Fieldz

   #region  Constructor

   public SmdFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;

      CreateHamper_4ButtonsResetGo_Width(hamper4buttons.Width);

      hamperHorLine.Visible = false;

      this.Width  = hamper4buttons.Width  + ZXC.QUN;
      this.Height = hamper4buttons.Bottom + ZXC.QUN;

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region Put & GetFilterFields

   private SmdFilter TheSmdFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as SmdFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheSmdFilter = (SmdFilter)_filter_data;

      if(TheSmdFilter != null)
      {
      }

      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
   }

   #endregion Put & GetFilterFields

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
   }

   #endregion AddFilterMemberz()

}

public class SmdFilter : VvRpt_Mix_Filter
{

   public enum PrintSmdEnum
   {
      SMD
   }

   public PrintSmdEnum PrintSmd { get; set; }

   public SmdDUC theDUC;

   public SmdFilter(SmdDUC _theDUC)
   {
      this.theDUC = _theDUC;
      SetDefaultFilterValues();
   }

   #region SetDefaultFilterValues()

   public override void SetDefaultFilterValues()
   {
      ZXC.VvDataBaseInfo vvDBinfo = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects;
      int projectYear = int.Parse(vvDBinfo.ProjectYear);
      int thisYear    = DateTime.Now.Year;
      PrintSmd        = PrintSmdEnum.SMD;
   }

   #endregion SetDefaultFilterValues()

}
