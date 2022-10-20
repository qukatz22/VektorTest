using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public class InterniRvrDUC : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_PersonCd, tbx_prezime , tbx_ime, tbx_radMjesto;
   private VvHamper  hamp_radnik;

   #endregion Fieldz

   #region Constructor

   public InterniRvrDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul)  : base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
        (Mixer.tt_colName, new string[] 
         { 
               Mixer.TT_IRV
         });
   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
      hamp_mjTroska.Visible = true;
      hamp_mjTroska.Location = new Point(hamp_napomena.Right, hamp_napomena.Top);

      InitializeHamper_Radnik(out hamp_radnik);
      nextY = hamp_radnik.Bottom;
   }

   public void InitializeHamper_Radnik(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0        1        2            3        4        5       
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q2un, ZXC.Q9un - ZXC.Qun4, ZXC.Q7un+ ZXC.Qun8, ZXC.Q4un, ZXC.Q6un  + ZXC.Qun2};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4           ,            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

                     hamper.CreateVvLabel  (0, 0, "Djelatnik:", ContentAlignment.MiddleRight);
      tbx_PersonCd = hamper.CreateVvTextBox(1, 0, "tbx_PersonCd", "Sifra djelatnika", GetDB_ColumnSize(DB_ci.personCD)); 
      tbx_prezime  = hamper.CreateVvTextBox(2, 0, "tbx_prezime" , "Prezime"         , GetDB_ColumnSize(DB_ci.personPrezim));
      tbx_ime      = hamper.CreateVvTextBox(3, 0, "tbx_ime"     , "Ime"             , GetDB_ColumnSize(DB_ci.personIme));
      tbx_ime.JAM_ReadOnly = true;
      
      tbx_PersonCd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_prezime.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_prezime.Font = ZXC.vvFont.LargeBoldFont;
      
                      hamper.CreateVvLabel  (4, 0, "Radno mjesto:", ContentAlignment.MiddleRight);
      tbx_radMjesto = hamper.CreateVvTextBox(5, 0, "tbx_radMjesto", "Rdano mjesto", GetDB_ColumnSize(DB_ci.strA_40));
      tbx_radMjesto.JAM_ReadOnly = true;
      
      tbx_PersonCd.JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType     , new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterSifra)  , new EventHandler(AnyPersonTextBoxLeave));
      tbx_prezime .JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonTextBoxLeave));
      

   }

   public void AnyPersonTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      //if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Person person_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         person_rec = PersonSifrar.Find(FoundInSifrar<Person>);

         if(person_rec != null && tb.Text != "")
         {
            Fld_PersonCd  = person_rec.PersonCD/*RecID*/;
            Fld_Prezime   = person_rec.Prezime;
            Fld_Ime       = person_rec.Ime ;
            Fld_RadMjesto = person_rec.RadMj;

         }
         else
         {
            Fld_PersonCdAsTxt = Fld_Prezime = Fld_Ime = Fld_RadMjesto = "";
         }
      }
   }

   #endregion CreateSpecificHampers()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_dateOd_OnlyTime_CreateColumn(ZXC.Q3un, "Početak rada");
      T_dateDo_OnlyTime_CreateColumn(ZXC.Q3un, "Završetak rada");
      T_kol_CreateColumn            (ZXC.Q3un, "Sati", "Sati", 2);
      T_opis_128_CreateColumn       (0, "Opis rada", "Komentar", 128, null);
      T_kpdbUlBrA_32_CreateColumn   (ZXC.Q5un, "Projekt", "Projekt", 32);
      T_kpdbUlBrB_32_CreateColumn   (ZXC.Q4un, "Mjesto" , "Mjesto rada - ");

      vvtbT_kol.JAM_ShouldSumGrid = true;
      vvtbT_kpdbUlBrA_32.JAM_SetAutoCompleteData(Faktur.recordName, Faktur.sorterTtNum.SortType, ZXC.AutoCompleteRestrictor.FAK_ExactTT_Only, OnVvTBEnter_SetAutocmplt_Faktur_DUMMY, null);
      TheG.CellLeave += new DataGridViewCellEventHandler(TheG_CellLeave_CalcSati);
  


   }

   void TheG_CellLeave_CalcSati(object sender, DataGridViewCellEventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      if(TheG.CurrentCell.ColumnIndex != DgvCI.iT_dateOd &&
         TheG.CurrentCell.ColumnIndex != DgvCI.iT_dateDo) return;

      // here we go ... znaci u 'zutome' smo i u jednoj od date kolona. 

      int rIdx = TheG.CurrentRow.Index;

      DateTime dateOd = TheG.GetDateCell(DgvCI.iT_dateOd, rIdx, true);
      DateTime dateDo = TheG.GetDateCell(DgvCI.iT_dateDo, rIdx, true);

      if(dateOd == DateTime.MinValue || dateDo == DateTime.MinValue) return;

      TimeSpan ts;
      decimal brSati;

      ts = dateDo.Subtract(dateOd);
      brSati = System.Convert.ToDecimal(ts.TotalHours);

      TheG.PutCell(DgvCI.iT_kol, rIdx, brSati);

   }

   #endregion InitializeTheGrid_Columns()

   #region Fld_

   public uint     Fld_PersonCd      { get { return ZXC.ValOrZero_UInt(tbx_PersonCd.Text) ; } set { tbx_PersonCd .Text = value.ToString("0000"); } }
   public string   Fld_PersonCdAsTxt { get { return tbx_PersonCd .Text                    ; } set { tbx_PersonCd .Text = value                 ; } }
   public string   Fld_Prezime       { get { return tbx_prezime  .Text                    ; } set { tbx_prezime  .Text = value                 ; } }
   public string   Fld_Ime           { get { return tbx_ime      .Text                    ; } set { tbx_ime      .Text = value                 ; } }
   public string   Fld_RadMjesto     { get { return tbx_radMjesto.Text                    ; } set { tbx_radMjesto.Text = value                 ; } }

   #endregion _Fld

   #region override PutSpecificsFld() GetSpecificsFld()

   /*protected*/public override void PutSpecificsFld()
   {
      Fld_PersonCd        = mixer_rec.PersonCD    ;
      Fld_Prezime         = mixer_rec.PersonPrezim;
      Fld_Ime             = mixer_rec.PersonIme   ;
      Fld_RadMjesto       = mixer_rec.StrA_40     ;

                                  Fld_TtOpis     = ZXC.luiListaMixTypeZahtjev.GetNameForThisCd(mixer_rec.TT);

      if(CtrlOK(tbx_v1_tt      )) Fld_V1_tt       = mixer_rec.V1_tt;
      if(CtrlOK(tbx_v1_ttOpis  )) Fld_V1_ttOpis   = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V1_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v1_ttNum   )) Fld_V1_ttNum    = mixer_rec.V1_ttNum;
      if(CtrlOK(tbx_v2_tt      )) Fld_V2_tt       = mixer_rec.V2_tt;
      if(CtrlOK(tbx_v2_ttOpis  )) Fld_V2_ttOpis   = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V2_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v2_ttNum   )) Fld_V2_ttNum    = mixer_rec.V2_ttNum;
      if(CtrlOK(tbx_externLink1)) Fld_ExternLink1 = mixer_rec.ExternLink1;


   }

   protected override void GetSpecificsFld()
   {
      mixer_rec.PersonCD     = Fld_PersonCd       ;
      mixer_rec.PersonPrezim = Fld_Prezime        ;
      mixer_rec.PersonIme    = Fld_Ime            ;
      mixer_rec.StrA_40      = Fld_RadMjesto      ;

      if(CtrlOK(tbx_ProjektCD  )) mixer_rec.ProjektCD   = Fld_ProjektCD;
      if(CtrlOK(tbx_v1_tt      )) mixer_rec.V1_tt       = Fld_V1_tt;
      if(CtrlOK(tbx_v1_ttNum   )) mixer_rec.V1_ttNum    = Fld_V1_ttNum;
      if(CtrlOK(tbx_v2_tt      )) mixer_rec.V2_tt       = Fld_V2_tt;
      if(CtrlOK(tbx_v2_ttNum   )) mixer_rec.V2_ttNum    = Fld_V2_ttNum;
      if(CtrlOK(tbx_externLink1)) mixer_rec.ExternLink1 = Fld_ExternLink1;
   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region PrintDocumentRecord

   public InterniRVRFilterUC TheInterniRVRFilterUC { get; set; }
   public InterniRVRFilter   TheInterniRVRFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.TheInterniRVRFilter = new InterniRVRFilter(this);

      TheInterniRVRFilterUC = new InterniRVRFilterUC(this);
      TheInterniRVRFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheInterniRVRFilterUC.Width;

   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      InterniRVRFilter mixerFilter = (InterniRVRFilter)vvRptFilter;

      switch(mixerFilter.PrintInterniRVR)
      {
         case InterniRVRFilter.PrintInterniRVREnum.InterniRVR: specificMixerReport = new RptX_InterniRvr(new Vektor.Reports.XIZ.CR_IRVduc(), "InterniRVR", mixerFilter); break;

         default: ZXC.aim_emsg("{0}\nPrintSomeInterniRVRDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintInterniRVR); return null;
      }

      return specificMixerReport;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheInterniRVRFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheInterniRVRFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
   }


   #endregion PrintDocumentRecord

   #region Colors
   
   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_IRV, Color.Empty, clr_IRV);
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

public class InterniRVRFilterUC : VvFilterUC
{
   #region Fieldz

   #endregion Fieldz

   #region  Constructor

   public InterniRVRFilterUC(VvUserControl vvUC)
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

   private InterniRVRFilter TheInterniRVRFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as InterniRVRFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheInterniRVRFilter = (InterniRVRFilter)_filter_data;

      if(TheInterniRVRFilter != null)
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

public class InterniRVRFilter : VvRpt_Mix_Filter
{

   public enum PrintInterniRVREnum
   {
      InterniRVR
   }

   public PrintInterniRVREnum PrintInterniRVR { get; set; }

   public InterniRvrDUC theDUC;

   public InterniRVRFilter(InterniRvrDUC _theDUC)
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
      PrintInterniRVR        = PrintInterniRVREnum.InterniRVR;
   }

   #endregion SetDefaultFilterValues()

}
