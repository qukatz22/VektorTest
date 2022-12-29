using System;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CrystalDecisions.Windows.Forms;
using System.Collections.Generic;

public class RiskReportUC              : VvReportUC
{
   #region Fieldz

   private RadioButton rbt_ArtiklSvi , rbt_ArtiklSaPrometom;

   private int razmakIzmjedjuHampera;
   private int nextX, nextY , razmakHamp = ZXC.Qun10;
   private int maxHampWidth;

   public RiskFilterUC TheRiskFilterUC { get; set; }

   #endregion Fieldz

   #region Constructor

   public RiskReportUC(Control parent, VvRpt_RiSk_Filter _rptFilter, VvForm.VvSubModul vvSubModul)
   {
      this.TheRptFilter = _rptFilter;
      this.TheSubModul  = vvSubModul;

      this.SuspendLayout();

      TheRiskFilterUC        = new RiskFilterUC(this);
      TheRiskFilterUC.Parent = TheFilterPanel;
      maxHampWidth          = TheRiskFilterUC.MaxHamperWidth;
      razmakIzmjedjuHampera = TheRiskFilterUC.razmakIzmjedjuHampera;
      nextX = razmakIzmjedjuHampera;
      nextY = TheRiskFilterUC.Height;

      InitializeVvUserControl(parent);
   
      //  sirina FilterPanela
      TheFilterPanel_Width = TheRiskFilterUC.Width;
      CalcTheFilterPanelWidth();

      this.ResumeLayout();

    // 27.10.2021: 
    //31.03.2022. razdvajamo IsOnlyIOSknjizenje i ForceIRMkaoIRA??????
      if(ZXC.KSD.Dsc_IsOnlyIOSknjizenje)
      {
         SetSifrarAndAutocomplete<Kplan>(null, VvSQL.SorterType.None);
         FakturDao.Fak2Nal_CheckAndPrebaciIfNeeded(TheDbConnection);
      }
   }

   #endregion Constructor

   #region Hampers

   #region hamper-i odluke *****

   private void InitializeHamper_Artikli(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", TheFilterPanel, false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] {ZXC.Q6un + ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] {ZXC.Qun4 + ZXC.Qun5};
      hamper.VvRightMargin = hamper.VvLeftMargin;
      
      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
      Label lbl            = hamper.CreateVvLabel      (0, 0,       "Izbor artikla:", ContentAlignment.MiddleLeft);
      rbt_ArtiklSaPrometom = hamper.CreateVvRadioButton(0, 1, null, "Samo sa prometom", TextImageRelation.ImageBeforeText);
      rbt_ArtiklSaPrometom.Checked = true;
      rbt_ArtiklSvi        = hamper.CreateVvRadioButton(0, 2, null, "Svi artikli"    , TextImageRelation.ImageBeforeText);

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, maxHampWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   //private void InitializeHamper_Kamate(out VvHamper hamper)
   //{
   //   hamper = new VvHamper(1, 4, "", TheFilterPanel, false, nextX, nextY, razmakHamp);

   //   hamper.VvColWdt      = new int[] { ZXC.Q4un};
   //   hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 + ZXC.Qun5};
   //   hamper.VvRightMargin = hamper.VvLeftMargin;
      
   //   for (int i = 0; i < hamper.VvNumOfRows; i++)
   //   {
   //      hamper.VvRowHgt[i]    = ZXC.QUN;
   //   }
   //   hamper.VvSpcBefRow = new int[] { ZXC.Qun12, ZXC.Qun12, ZXC.Qun12, ZXC.Qun12 };
   //   hamper.VvBottomMargin = hamper.VvTopMargin;
      
   //   Label lbl        = hamper.CreateVvLabel      (0, 0, "Kamate:", ContentAlignment.MiddleLeft);
   //   rbKamate_RealUpl = hamper.CreateVvRadioButton(0, 1, null, "Realne uplate", TextImageRelation.ImageBeforeText);
   //   rbKamate_RealUpl.Checked = true;
   //   rbKamate_TeorUpl = hamper.CreateVvRadioButton(0, 2, null, "i teoretske uplate", TextImageRelation.ImageBeforeText);

   //   VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, maxHampWidth, razmakIzmjedjuHampera);
   //   VvHamper.HamperStyling(hamper);
   //}

   #endregion hamper-i odluke *****

   #region hamperDrillDown 

   private void InitializeHamper_DrillDown(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", TheFilterPanel, false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q6un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 + ZXC.Qun5 };
      hamper.VvRightMargin = hamper.VvLeftMargin;
      
      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

  //    cb_DrillDown       = hamper.CreateVvCheckBox_OLD(0, 0, null, "Omogući DrillDown", RightToLeft.No);
  //    cb_zapamtiPostavke = hamper.CreateVvCheckBox_OLD(0, 1, null, "Zapamti postavke" , RightToLeft.No);

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, maxHampWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   #endregion hamperDrillDown

   #endregion Hampers

   #region PutFields(), GetFields()

   #region Fld_

   //public bool Fld_NeedsDrillDown
   //{
   //   get { return cb_DrillDown.Checked; }
   //   set { cb_DrillDown.Checked = value; }
   //}
   //public bool Fld_NeedsZapamtitPostavke
   //{
   //   get { return cb_zapamtiPostavke.Checked; }
   //   set { cb_zapamtiPostavke.Checked = value; }
   //}

   #endregion Fld_

   public override void GetFields(bool fuse)
   {
      GetFilterFields();
   }

   public override void GetFilterFields()
   {
      TheRiskFilterUC.GetFilterFields();

     // TheRptFilter.NeedsDrillDown   = Fld_NeedsDrillDown;
   }

   #region PutFields(_vvRptFilter)

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheRptFilter = (VvRpt_RiSk_Filter)_filter_data;

      if(TheRptFilter != null)
      {
         TheRiskFilterUC.PutFilterFields(TheRptFilter);
         
       //  Fld_NeedsDrillDown    = TheRptFilter.NeedsDrillDown;
      }
      
      // Za JAM_... : 
      this.ValidateChildren();
   }

   //ubuduce koristi static method: VvHamper.Set_ControlText_ThreadSafe(Control theControl, string theText) 
   public void Set_KontoDoText_ThreadSafe(string text)
   {
      // InvokeRequired required compares the thread ID of the
      // calling thread to the thread ID of the creating thread.
      // If these threads are different, it returns true.
      if(this.TheRiskFilterUC.tbx_artiklCdDO.InvokeRequired)
      {
         SetTextCallback d = new SetTextCallback(Set_KontoDoText_ThreadSafe);
         this.Invoke(d, new object[] { text });
      }
      else
      {
         this.TheRiskFilterUC.tbx_artiklCdDO.Text = text;
      }
   }

   #endregion PutFields(_vvRptFilter)

   #endregion PutFields(), GetFields()

   #region VvFakturReport

   public VvRiskReport TheVvRasReport
   {
      get { return TheVvReport as VvRiskReport; }
   }

   public VvRpt_RiSk_Filter TheRptFilter
   {
      get;
      set;
   }

   #endregion VvFakturReport

   #region Override

   public override VvRptFilter VirtualRptFilter
   {
      get { return this.TheRptFilter; }
   }

   public override void AddFilterMemberz()
   {
      TheRiskFilterUC.AddFilterMemberz(TheRptFilter, TheVvRasReport);
      //this.artiklListUc.AddFilterMemberz();

   }

   public override void ResetRptFilterRbCbControls()
   {
      //cb_DrillDown.Checked   = /*cb_zapamtiPostavke.Checked =*/ false;
     
   }

   #endregion Override

}

#region QQQ! TAB_RiskFilterUC

public partial class RiskFilterUC : VvFilterUC
{
   #region Fieldz

   private VvTextBox tbx_artiklCdOD, tbx_artiklNazivOD, tbx_artiklNazivDO, tbx_fakturOD, tbx_fakturDO,
                            tbx_KD_naziv, tbx_KD_sifra, tbx_KD_ticker,
                            tbxLookUp_TT, tbx_TTOpis,
                            tbx_skladCd, tbx_skladOpis, tbx_skladCd2, tbx_skladOpis2,
                            tbx_MT_naziv, tbx_MT_sifra, tbx_MT_ticker,
                            tbx_NumOfGrups,
                            tbx_pdvKredit, tbx_pdvPovrat, tbx_pdvPredujam, tbx_pdvUstup,
                            tbx_NacPlac,
                            tbx_obrazacSastavio, tbx_pdvF_Osn, tbx_pdvF_Pdv, tbx_pdvIspravak, tbx_PretpPriUvz_Osn, tbx_PretpPriUvz_Pdv,
                            tbx_tel, tbx_fax, tbx_mail, tbx_kompBr, tbx_Projekt, tbx_Status,
                            tbx_compDateOd,
                            tbx_Fof_III5, tbx_Fof_III6, tbx_Fof_III7, tbx_PdvGEOkind,
                            tbx_personCD, tbx_personName, tbx_TipKupDob, tbx_rptNaziv, tbx_rptNapomena,
                            tbx_redakVII ,
                            tbx_pdv_811  ,
                            tbx_pdv_812  ,
                            tbx_pdv_813  ,
                            tbx_pdv_814  ,
                            tbx_pdv_815  ,
                            tbx_pdv_820  ,
                            tbx_pdv_831vr, tbx_pdv_831br    ,
                            tbx_pdv_832vr, tbx_pdv_832br    ,
                            tbx_pdv_833vr, tbx_pdv_833br    ,
                            tbx_pdv_860  , tbx_pdv_840,
                            tbx_Artikl_TS, tbx_Artikl_Gr1, tbx_Artikl_Gr2, tbx_Artikl_Gr3,
                            tbx_FRUF_Name, tbx_FRUF_Value, 
                            tbx_vezDok2OD, tbx_vezDok2DO
                            ;

   public VvTextBox tbx_artiklCdDO, tbx_compDateDo;
   private RadioButton rbSort_Datum, rbSort_Broj, rbSort_Partner,
                            rbArtSort_ByName, rbArtSort_BySifra, rbArtSort_ByBarCode, rbArtSort_topByKol, rbArtSort_topByFin, rbArtSort_topByRuc,
                            rbt_grupNull, rbt_grupUser, rbt_grupPartner, rbt_grupValuta, rbt_grupPutnik, rbt_grupMjTros, rbt_grupPosJed, rbt_grupProjekt, rbt_grupNacPl, rbt_grupTT, rbt_grupMonth, rbt_grupSkladCD, rbt_grupTH_CycleM, rbt_grupGodina,
                            rbt_grupArtNull, rbt_grupArtTs, rbt_grupArtGr1, rbt_grupArtGr2, rbt_grupArtGr3,
                            rbt_pdvKnjigaR, rbt_pdvKnjigaP, rbt_pdvKnjigUr, rbt_pdvKnjigUu, rbt_pdvKnjigNijedna,
                            rbt_pdvR1, rbt_pdvR2, rbt_pdvSviR,
                            rbt_grupWeek, rbt_grupDay, rbt_grupHour,
                            rbt_grupDayOfWeek, rbt_grupHourOfDay,
                            rbt_svaSklad, rbt_onlyVelepSkl, rbt_onlyMalopSkl, rbt_TMB_VpskVps2,
                            rbt_Lijek, rbt_Potros, rbt_LiP,
                            rbt_svdSvaSkl, rbt_svdDonac, rbt_svdNeDonac;
   private VvHamper hamper_OdDo, hamper_Partner, hamper_TTSklad, hamper_AllSklad, hamper_TopSort, hamper_pdv, hamper_NacPlac, hamper_IOS, hamper_ODDoArtikl, hamper_ODDoTT, 
                    hamper_status, hamper_compDate, hamper_rptNap, hamper_OdDoVezDok2, hamper_svdLiP, hamper_svdSklad, hamper_uvozIzvoz;
   public VvHamper hamper_Sort, hamper_HorLine, hamper_grupDok, hamper_grupArtikl, hamper_ArtSort, hamper_pdvRtip, 
                   hamp_macro, hamper_Color, hamper_Bool2;
   private CheckBox cbx_grupaPoStr, cbx_visiblePosto, cbx_topGroups, cbx_topSort, cbx_ocuGraf,
                    cbx_isPrjktTel, cbx_isPrjktFax, cbx_IsPrjktMail,
                    cbx_isVisibleTT, cbx_isVisibleAdress, cbx_isCurrUser,
                    cbx_otsAnalitika, cbx_otsDospjeca, cbx_otsKontakt, cbx_otsLineTipBr, cbx_otsDospjelo, cbx_isPrihodTT, cbx_isMpskPoNBC, cbx_isNoColor, cbx_isOtsNevezano,
                    cbx_isPoslJed, cbx_isPecatPotpis, cbx_onlyArtiklSaStanjem, cbx_isForceOtsByDokDate, cbx_isRashodTT, cbx_isChkO, cbx_isOpzStatVezDok, cbx_IsBlgInIzvVal,
                    cbx_FRUF_BiloGdjeU, cbx_uvozIzvozOnly;

   private Label lbl_grupa, lbl_grupa_skl2, lbl_ThKune;

   private Button btn_macroPrint, btn_macroNew, btn_macroDel, btn_macroRename;
   private VvTextBox tbx_DatumOTS, tbx_ThKune;
   private VvDateTimePicker dtp_DatumOTS, dtp_compDateOd, dtp_compDateDo;
   private ComboBox combbx_macro;
   private CheckBox[] cbx_sklad;

   #endregion Fieldz

   #region  Constructor

   public RiskFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;

      nextX = razmakIzmjedjuHampera = ZXC.Qun8;
      InitializeHamper_Macro(out hamp_macro);
      nextY = hamp_macro.Bottom + razmakIzmjedjuHampera * 2;
      InitializeHamper_DatumOdDo(out hamper_OdDo);
      nextY = hamper_OdDo.Bottom + razmakIzmjedjuHampera;

      MaxHamperWidth = hamper_OdDo.Width;
      CreateHamper_4ButtonsResetGo_Width(MaxHamperWidth);
      hamper4buttons.Location = new Point(nextX, nextX);

      hamperHorLine.Parent = this;
      CreateHampers();

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      if(IsRiskMacroListInitialized == false)
      {
         LoadVvRiskMacroList();
         IsRiskMacroListInitialized = true;
      }

      InitializeVvRiskMacroList_ComboBox();

      this.ResumeLayout();
   }


   #endregion  Constructor

   #region PANELS

   public const string FilterSetREALIZ   = "REALIZ"    /*RiskFilterSetEnum.REALIZ .ToString()*/;
   public const string FilterSetIOS      = "IOS"       /*RiskFilterSetEnum.IOS    .ToString()*/;
   public const string FilterSetPDV      = "PDV"       /*RiskFilterSetEnum.SKLAD  .ToString()*/;
   public const string FilterSetSKLAD    = "SKLAD"     /*RiskFilterSetEnum.PDV    .ToString()*/;
   public const string FilterSetPRM_ART  = "PRM ART"   /*RiskFilterSetEnum.PRM_ART.ToString()*/;
   public const string FilterSetREALIZ_S = "REALIZ_S"  /*RiskFilterSetEnum.REALIZ .ToString()*/;

   #endregion PANELS

   #region CalcLocationHamper

   private void CalcLocationHamper_REALIZ(bool isSintetika)
   {
      if(isSintetika)
      {
         hamper_compDate.Location = new Point(nextX, hamper_OdDo.Bottom + razmakIzmjedjuHampera);
         hamper_TTSklad .Location = new Point(nextX, hamper_compDate.Bottom + razmakIzmjedjuHampera);
         hamper_compDate.Visible = true;
         CompDateOdExitMethod(null, EventArgs.Empty);
         VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_compDate, MaxHamperWidth, razmakIzmjedjuHampera);
      }
      else
      {
         hamper_TTSklad.Location = new Point(nextX, hamper_OdDo.Bottom + razmakIzmjedjuHampera);
         hamper_compDate.Visible = false;
      }


      if(ZXC.IsSvDUH)
      {
         hamper_OdDoVezDok2.Visible = hamper_svdLiP.Visible = hamper_svdSklad.Visible = true;

         hamper_OdDoVezDok2.Location = new Point(nextX, hamper_TTSklad.Top        + razmakIzmjedjuHampera + ZXC.Q3un + ZXC.Qun2 );
         hamper_svdLiP     .Location = new Point(nextX, hamper_OdDoVezDok2.Bottom + razmakIzmjedjuHampera);
         hamper_svdSklad.Location    = new Point(nextX, hamper_svdLiP.Bottom + razmakIzmjedjuHampera);

         VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_OdDoVezDok2, MaxHamperWidth, razmakIzmjedjuHampera);
         VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_svdLiP     , MaxHamperWidth, razmakIzmjedjuHampera);
         VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_svdSklad   , MaxHamperWidth, razmakIzmjedjuHampera);

         hamper_OdDoVezDok2.BringToFront();
         hamper_svdLiP     .BringToFront();
         hamper_svdSklad   .BringToFront();

      }

      hamper_ODDoTT   .Location = new Point(               nextX, ZXC.IsSvDUH ? hamper_svdSklad.Bottom + razmakIzmjedjuHampera : hamper_TTSklad  .Bottom + razmakIzmjedjuHampera);
      hamper_NacPlac  .Location = new Point(               nextX, hamper_ODDoTT   .Bottom + razmakIzmjedjuHampera);
      hamper_pdvRtip  .Location = new Point(               nextX, hamper_NacPlac  .Bottom + razmakIzmjedjuHampera);
      hamper_status   .Location = new Point(hamper_pdvRtip.Right, hamper_NacPlac  .Bottom + razmakIzmjedjuHampera);
      hamper_Partner  .Location = new Point(               nextX, hamper_pdvRtip  .Bottom + razmakIzmjedjuHampera);
      hamper_Sort     .Location = new Point(               nextX, hamper_Partner  .Bottom + razmakIzmjedjuHampera);
      hamper_grupDok  .Location = new Point(               nextX, hamper_Sort     .Bottom + razmakIzmjedjuHampera);
      hamper_TopSort  .Location = new Point(               nextX, hamper_grupDok  .Bottom + razmakIzmjedjuHampera);
    //hamperHorLine   .Location = new Point(               nextX, hamper_TopSort  .Bottom + razmakIzmjedjuHampera);
      hamper_uvozIzvoz.Location = new Point(               nextX, hamper_TopSort  .Bottom + razmakIzmjedjuHampera);
      hamperHorLine   .Location = new Point(               nextX, hamper_uvozIzvoz.Bottom + razmakIzmjedjuHampera);

      hamper_TTSklad  .Visible = true;
      hamper_ODDoTT   .Visible = true;
      hamper_NacPlac  .Visible = true;
      hamper_Partner  .Visible = true;
      hamper_Sort     .Visible = true;
      hamper_grupDok  .Visible = true;
      hamper_TopSort  .Visible = true;
      hamper_pdvRtip  .Visible = true;
      hamper_status   .Visible = true;
      hamper_uvozIzvoz.Visible = true;

      hamper_pdv       .Visible = false;
      hamper_IOS       .Visible = false;
      hamper_Color     .Visible = false;
      hamper_ODDoArtikl.Visible = false;
      hamper_grupArtikl.Visible = false;
      hamper_ArtSort   .Visible = false;
      hamper_rptNap    .Visible = false;
      hamper_Bool2     .Visible = false;

      rbt_onlyVelepSkl.Visible = rbt_onlyMalopSkl.Visible = rbt_svaSklad.Visible = rbt_TMB_VpskVps2.Visible = false;
      lbl_grupa_skl2  .Visible = tbx_skladCd2    .Visible = tbx_skladOpis2.Visible = true;

      cbx_isMpskPoNBC  .Visible = cbx_isChkO.Visible = false;
      cbx_IsBlgInIzvVal.Visible = true;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_TTSklad  , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_ODDoTT   , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_NacPlac  , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_status   , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_Partner  , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_Sort     , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_grupDok  , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_TopSort  , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_uvozIzvoz, MaxHamperWidth, razmakIzmjedjuHampera);

      if(ZXC.IsSvDUH)
      {
         hamper_pdvRtip.Visible = false;
         lbl_grupa_skl2.Visible = tbx_skladCd2.Visible = tbx_skladOpis2.Visible = false;
         cbx_IsBlgInIzvVal.Visible = false; // 04.12.2019. ovo vise ne koristimo za ista pa je bolje da se ne vidi da ne zbunjuje
         hamper_uvozIzvoz.Visible = false;

      }
      hamper_uvozIzvoz.BringToFront();
      //AddHamperHorLine(hamper_TopSort.Bottom + razmakIzmjedjuHampera);
      AddHamperHorLine(hamper_uvozIzvoz.Bottom + razmakIzmjedjuHampera);
   }

   private void CalcLocationHamper_IOS()
   {
      hamper_IOS    .Location = new Point(nextX, hamper_OdDo   .Bottom + razmakIzmjedjuHampera);
      hamper_Partner.Location = new Point(nextX, hamper_IOS    .Bottom + razmakIzmjedjuHampera);
      hamper_ODDoTT .Location = new Point(nextX, hamper_Partner.Bottom + razmakIzmjedjuHampera);
      hamper_Color  .Location = new Point(nextX, hamper_ODDoTT .Bottom + razmakIzmjedjuHampera);
      hamper_Bool2  .Location = new Point(nextX, hamper_ODDoTT .Bottom + razmakIzmjedjuHampera); hamper_Bool2.BringToFront(); // 14.09.2017
      hamper_rptNap .Location = new Point(nextX, hamper_Color  .Bottom + razmakIzmjedjuHampera);
      hamperHorLine .Location = new Point(nextX, hamper_rptNap .Bottom + razmakIzmjedjuHampera);

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_IOS    , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_Partner, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_Color  , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_rptNap , MaxHamperWidth, razmakIzmjedjuHampera);

      hamper_TTSklad    .Visible = false;
    //hamper_ODDoTT     .Visible = false;
      hamper_Partner    .Visible = true;
      hamper_Sort       .Visible = false;
      hamper_grupDok    .Visible = false;
      hamper_TopSort    .Visible = false;
      hamper_pdvRtip    .Visible = false;
      hamper_pdv        .Visible = false;
      hamper_IOS        .Visible = true;
      hamper_Color      .Visible = true;
      hamper_ODDoArtikl .Visible = false;
      hamper_grupArtikl .Visible = false;
      hamper_ArtSort    .Visible = false;
      hamper_NacPlac    .Visible = false;
      hamper_compDate   .Visible = false;
      hamper_rptNap     .Visible = true;
      hamper_status     .Visible = false;
      hamper_Bool2      .Visible = true;
      hamper_uvozIzvoz  .Visible = false;

      //AddHamperHorLine(hamper_Color.Bottom + razmakIzmjedjuHampera);

      hamper_OdDoVezDok2.Visible = hamper_svdLiP.Visible = hamper_svdSklad.Visible = false;

      cbx_isNoColor.Text = "Ispiši bez boja";

      AddHamperHorLine(hamper_rptNap.Bottom + razmakIzmjedjuHampera);
   }

   private void CalcLocationHamper_SKLAD()
   {
      hamper_TTSklad   .Location = new Point(nextX           , hamper_OdDo      .Bottom + razmakIzmjedjuHampera);
    //hamper_AllSklad  .Location = new Point(nextX, hamper_TTSklad   .Bottom + razmakIzmjedjuHampera);
      hamper_ODDoTT    .Location = new Point(nextX            , hamper_TTSklad   .Bottom + razmakIzmjedjuHampera);
      hamper_ODDoArtikl.Location = new Point(nextX            , hamper_ODDoTT    .Bottom + razmakIzmjedjuHampera);
      hamper_Bool2     .Location = new Point(nextX + ZXC.Qun10, hamper_ODDoArtikl.Bottom - ZXC.QUN - ZXC.Qun4  - ZXC.Qun8); hamper_Bool2.BringToFront(); //14.09.2017.
      hamper_NacPlac   .Location = new Point(nextX            , hamper_ODDoArtikl.Bottom + razmakIzmjedjuHampera);
      hamper_Partner   .Location = new Point(nextX            , hamper_NacPlac   .Bottom + razmakIzmjedjuHampera);
      hamper_ArtSort   .Location = new Point(nextX            , hamper_Partner   .Bottom + razmakIzmjedjuHampera);
      hamper_grupArtikl.Location = new Point(nextX            , hamper_ArtSort   .Bottom + razmakIzmjedjuHampera);
      hamper_TopSort   .Location = new Point(nextX            , hamper_grupArtikl.Bottom + razmakIzmjedjuHampera);
      hamperHorLine    .Location = new Point(nextX            , hamper_TopSort   .Bottom + razmakIzmjedjuHampera);

      if(ZXC.IsSvDUH)
      {
         rbt_onlyVelepSkl.Visible = rbt_onlyMalopSkl.Visible = rbt_svaSklad.Visible = false;
         cbx_isMpskPoNBC.Visible = cbx_isChkO.Visible = true;

         hamper_svdLiP.Visible = true;
         hamper_svdSklad.Visible = false;

         hamper_svdLiP.Location  = new Point(nextX, hamper_grupArtikl.Bottom + razmakIzmjedjuHampera);
         hamper_TopSort.Location = new Point(nextX, hamper_svdLiP    .Bottom + razmakIzmjedjuHampera);

         VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_svdLiP, MaxHamperWidth, razmakIzmjedjuHampera);

         hamper_svdLiP.BringToFront();
      }


      hamper_TTSklad.Visible = true;
      hamper_AllSklad  .Visible = false;// TH
      hamper_ODDoTT    .Visible = true;
      hamper_ODDoArtikl.Visible = true;
      hamper_NacPlac   .Visible = true;
      hamper_Partner   .Visible = true;
      hamper_ArtSort   .Visible = true;
      hamper_grupArtikl.Visible = true;
      hamper_TopSort   .Visible = true;

      hamper_Sort     .Visible = false;
      hamper_grupDok  .Visible = false;
      hamper_pdvRtip  .Visible = false;
      hamper_pdv      .Visible = false;
      hamper_IOS      .Visible = false;
      hamper_Color    .Visible = false;
      hamper_compDate .Visible = false;
      hamper_rptNap   .Visible = false;
      hamper_status   .Visible = false;
      hamper_Bool2    .Visible = true;
      hamper_uvozIzvoz.Visible = false;

      rbt_onlyVelepSkl.Visible = rbt_onlyMalopSkl.Visible = rbt_svaSklad  .Visible = true;
      lbl_grupa_skl2   .Visible = tbx_skladCd2    .Visible = tbx_skladOpis2.Visible = false;
      cbx_isMpskPoNBC  .Visible = cbx_isChkO.Visible = true;
      cbx_IsBlgInIzvVal.Visible = false;


      if(ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.TEMBO || ZXC.CURR_prjkt_rec.Ticker.StartsWith("TEMBO")) rbt_TMB_VpskVps2.Visible = true;
      else                                                                                                  rbt_TMB_VpskVps2.Visible = false;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_TTSklad   , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_ODDoTT    , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_ODDoArtikl, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_NacPlac   , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_Partner   , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_ArtSort   , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_grupArtikl, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_TopSort   , MaxHamperWidth, razmakIzmjedjuHampera);

      cbx_isNoColor.Text = "Artikli bez izlaza";

      // 24.01.2022. 
      //hamper_OdDoVezDok2.Visible = hamper_svdLiP.Visible = false;
      hamper_OdDoVezDok2.Visible = false;

      AddHamperHorLine(hamper_TopSort.Bottom + razmakIzmjedjuHampera);
   }

   private void CalcLocationHamper_PRM_ART()
   {
      hamper_TTSklad   .Location = new Point(nextX, hamper_OdDo      .Bottom + razmakIzmjedjuHampera);
      hamper_ODDoTT    .Location = new Point(nextX, hamper_TTSklad   .Bottom + razmakIzmjedjuHampera);
      hamper_ODDoArtikl.Location = new Point(nextX, hamper_ODDoTT    .Bottom + razmakIzmjedjuHampera);
      hamper_NacPlac   .Location = new Point(nextX, hamper_ODDoArtikl.Bottom + razmakIzmjedjuHampera);
      hamper_Partner   .Location = new Point(nextX, hamper_NacPlac   .Bottom + razmakIzmjedjuHampera);
      hamper_ArtSort   .Location = new Point(nextX, hamper_Partner   .Bottom + razmakIzmjedjuHampera);
      hamper_grupDok   .Location = new Point(nextX, hamper_ArtSort   .Bottom + razmakIzmjedjuHampera);
      hamper_grupArtikl.Location = new Point(nextX, hamper_grupDok   .Bottom + razmakIzmjedjuHampera);
      hamper_TopSort   .Location = new Point(nextX, hamper_grupArtikl.Bottom + razmakIzmjedjuHampera);
      hamperHorLine    .Location = new Point(nextX, hamper_TopSort   .Bottom + razmakIzmjedjuHampera);

      hamper_TTSklad   .Visible = true;
      hamper_ODDoTT    .Visible = true;
      hamper_ODDoArtikl.Visible = true;
      hamper_NacPlac   .Visible = true;
      hamper_Partner   .Visible = true;
      hamper_ArtSort   .Visible = true;
      hamper_grupArtikl.Visible = true;
      hamper_TopSort   .Visible = true;
      hamper_grupDok   .Visible = true;

      hamper_Sort     .Visible = false;
      hamper_pdvRtip  .Visible = false;
      hamper_pdv      .Visible = false;
      hamper_IOS      .Visible = false;
      hamper_Color    .Visible = false;
      hamper_compDate .Visible = false;
      hamper_rptNap   .Visible = false;
      hamper_Bool2    .Visible = false;
      hamper_uvozIzvoz.Visible = false;

      rbt_onlyVelepSkl.Visible = rbt_onlyMalopSkl.Visible = rbt_svaSklad  .Visible = true;
      lbl_grupa_skl2   .Visible = tbx_skladCd2    .Visible = tbx_skladOpis2.Visible = false;
      cbx_isMpskPoNBC  .Visible = cbx_isChkO.Visible = true;
      cbx_IsBlgInIzvVal.Visible = false;

      if(ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.TEMBO || ZXC.CURR_prjkt_rec.Ticker.StartsWith("TEMBO")) rbt_TMB_VpskVps2.Visible = true;
      else                                                                                                  rbt_TMB_VpskVps2.Visible = false;


      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_TTSklad   , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_ODDoTT    , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_ODDoArtikl, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_NacPlac   , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_Partner   , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_ArtSort   , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_grupDok   , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_grupArtikl, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_TopSort   , MaxHamperWidth, razmakIzmjedjuHampera);

      hamper_OdDoVezDok2.Visible = false;

      AddHamperHorLine(hamper_TopSort.Bottom + razmakIzmjedjuHampera);

      if(ZXC.IsSvDUH)
      {
         hamper_svdLiP.Visible = true;

         hamper_svdLiP.Location = new Point(nextX, hamper_grupArtikl.Bottom + razmakIzmjedjuHampera);
         hamper_TopSort.Location = new Point(nextX, hamper_svdLiP.Bottom + razmakIzmjedjuHampera);

         VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_svdLiP, MaxHamperWidth, razmakIzmjedjuHampera);

         hamper_svdLiP.BringToFront();
      }
   }

   private void CalcLocationHamper_PDV()
   {
      hamper_pdv    .Location = new Point(nextX, hamper_OdDo   .Bottom + razmakIzmjedjuHampera); hamper_status.Location = new Point(hamper_pdv.Right - hamper_status.Width, hamper_pdv.Top + ZXC.Q5un - ZXC.Qun4 + razmakIzmjedjuHampera);
      hamper_Partner.Location = new Point(nextX, hamper_pdv    .Bottom + razmakIzmjedjuHampera);
      hamperHorLine .Location = new Point(nextX, hamper_Partner.Bottom + razmakIzmjedjuHampera);
      hamper_status.BringToFront();

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_pdv    , MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_Partner, MaxHamperWidth, razmakIzmjedjuHampera);

      hamper_TTSklad   .Visible = false;
      hamper_ODDoTT    .Visible = false;
      hamper_ODDoArtikl.Visible = false;
      hamper_Partner   .Visible = true;
      hamper_ArtSort   .Visible = false;
      hamper_grupArtikl.Visible = false;
      hamper_TopSort   .Visible = false;
      hamper_grupDok   .Visible = false;

      hamper_Sort     .Visible = false;
      hamper_pdvRtip  .Visible = false;
      hamper_pdv      .Visible = true;
      hamper_IOS      .Visible = false;
      hamper_Color    .Visible = false;
      hamper_NacPlac  .Visible = false;
      hamper_compDate .Visible = false;
      hamper_rptNap   .Visible = false;
      hamper_Bool2    .Visible = false;
      hamper_uvozIzvoz.Visible = false;

      hamper_OdDoVezDok2.Visible = hamper_svdLiP.Visible = hamper_svdSklad.Visible = false;

      AddHamperHorLine(hamper_Partner.Bottom + razmakIzmjedjuHampera);
   }

   private void AddHamperHorLine(int _nextY)
   {
      nextY = LocationOfHamper_HorLine(nextX, _nextY, MaxHamperWidth) + ZXC.QUN;
      this.Size = new Size(MaxHamperWidth + 4 * razmakIzmjedjuHampera, hamperHorLine.Bottom + ZXC.QUN);
   }

   #endregion CalcLocationHamper

   #region hampers

   private void CreateHampers()
   {
      razmakIzmjedjuHampera = ZXC.Qun8;

      InitializeHamper_OdDo_DateComp(out hamper_compDate);
      InitializeHamper_OdDo_Artikl(out hamper_ODDoArtikl);
      InitializeHamper_OdDo_TT(out hamper_ODDoTT);
      InitializeHamper_TTSklad(out hamper_TTSklad);
      InitializeHamper_SvaSklad(out hamper_AllSklad);
      InitializeHamper_FuseBool2(out hamper_Bool2);

      InitializeHamper_PdvRtip(out hamper_pdvRtip);
      InitializeHamper_Partner(out hamper_Partner);
      InitializeHamper_Sort(out hamper_Sort);
      InitializeHamper_ArtSort(out hamper_ArtSort);
      InitializeHamper_GrupirajDok(out hamper_grupDok);
      InitializeHamper_GrupirajArtikl(out hamper_grupArtikl);
      InitializeHamper_GroupTopSort(out hamper_TopSort);
      InitializeHamper_OTS(out hamper_IOS);
      InitializeHamper_Pvd(out hamper_pdv);
      InitializeHamper_ProjektNacPlac(out hamper_NacPlac);
      InitializeHamper_Status(out hamper_status);
      InitializeHamper_ColorVisible(out hamper_Color);
      InitializeHamper_RptNapomena(out hamper_rptNap);

      InitializeHamper_OdDo_VezDok2(out hamper_OdDoVezDok2);
      InitializeHamper_SvdLiP(out hamper_svdLiP);
      InitializeHamper_SvdSkladDonac(out hamper_svdSklad);

      InitializeHamper_UvozIzvoz(out hamper_uvozIzvoz);

      CalcLocationHamper_REALIZ(false);

      hamperHorLine.BorderStyle = System.Windows.Forms.BorderStyle.None;
   }

   private void InitializeHamper_FuseBool2(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN - ZXC.Qun10;
         hamper.VvSpcBefRow[i] = ZXC.Qun10;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbx_isNoColor = hamper.CreateVvCheckBox_OLD(0, 0, null, "", System.Windows.Forms.RightToLeft.No);
   }

   private void InitializeHamper_Macro(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 2, "", this, false, nextX, nextY - ZXC.Qun4, razmakHampGroup);

      hamper.VvColWdt = new int[] { ZXC.Q2un + ZXC.Qun2, ZXC.QUN - ZXC.Qun2 - ZXC.Qun8, ZXC.Q3un - ZXC.Qun8, ZXC.Q3un - ZXC.Qun8, ZXC.Q3un - ZXC.Qun8 };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = ZXC.Qun8;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN + ZXC.Qun10;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvRowHgt[0] = ZXC.QUN + ZXC.Qun8;
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "Macro:", ContentAlignment.MiddleCenter);

      combbx_macro = hamper.CreateVvComboBox(1, 0, "", 3, 0, "combbx_macro", ComboBoxStyle.DropDownList);
      combbx_macro.SelectedIndexChanged += new EventHandler(Combbx_macro_SelectedIndexChanged);

      btn_macroPrint = hamper.CreateVvButton(0, 1, new EventHandler(MacroGO_Click), "Izlistaj", 1, 0);
      btn_macroPrint.Name = "macroPrint";
      btn_macroPrint.FlatStyle = FlatStyle.Flat;
      btn_macroPrint.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_macroPrint.Image = VvIco.Go2_16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.go2.ico")), 16, 16)*/.ToBitmap();
      btn_macroPrint.TextImageRelation = TextImageRelation.ImageBeforeText;

      btn_macroNew = hamper.CreateVvButton(2, 1, new EventHandler(MacroNew_Click), "Dodaj");
      btn_macroNew.Name = "macroNew";
      btn_macroNew.FlatStyle = FlatStyle.Flat;
      btn_macroNew.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_macroNew.Image = VvIco.New16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_RTB.new.ico")), 16, 16)*/.ToBitmap();
      btn_macroNew.TextImageRelation = TextImageRelation.ImageBeforeText;

      btn_macroRename = hamper.CreateVvButton(3, 1, new EventHandler(MacroEdit_Click), "Ispravi");
      btn_macroRename.Name = "macroDel";
      btn_macroRename.FlatStyle = FlatStyle.Flat;
      btn_macroRename.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_macroRename.Image = VvIco.Opn2_16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.opn2.ico")), 16, 16)*/.ToBitmap();
      btn_macroRename.TextImageRelation = TextImageRelation.ImageBeforeText;

      btn_macroDel = hamper.CreateVvButton(4, 1, new EventHandler(MacroDel_Click), "Briši");
      btn_macroDel.Name = "macroDel";
      btn_macroDel.FlatStyle = FlatStyle.Flat;
      btn_macroDel.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_macroDel.Image = VvIco.Del16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_RTB.del.ico")), 16, 16)*/.ToBitmap();
      btn_macroDel.TextImageRelation = TextImageRelation.ImageBeforeText;

      VvHamper.HamperStyling(hamper);

      lbl.Font = ZXC.vvFont.BaseBoldFont;
      lbl.ForeColor = Color.DarkBlue;
      lbl.BackColor = Color.LightSteelBlue;
      hamper.BackColor = Color.LightSteelBlue;
   }

   private void InitializeHamper_DatumOdDo(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 2, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt = new int[] { ZXC.Q4un - ZXC.Qun4, ZXC.Q4un, ZXC.Q4un };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "DatumOdDo:", ContentAlignment.MiddleRight);
      tbx_DatumOD = hamper.CreateVvTextBox(1, 0, "tbx_datumOd", "Od datuma");
      tbx_DatumOD.JAM_IsForDateTimePicker = true;
      dtp_DatumOD = hamper.CreateVvDateTimePicker(1, 0, "", tbx_DatumOD);
      dtp_DatumOD.Name = "dtp_DatumOD";
      dtp_DatumOD.Tag = tbx_DatumOD;
      tbx_DatumOD.Tag = dtp_DatumOD;

      tbx_DatumDO = hamper.CreateVvTextBox(2, 0, "tbx_datumDo", "");
      tbx_DatumDO.JAM_IsForDateTimePicker = true;
      dtp_DatumDO = hamper.CreateVvDateTimePicker(2, 0, "", tbx_DatumDO);
      dtp_DatumDO.Name = "dtp_DatumDO";
      dtp_DatumDO.Tag = tbx_DatumDO;
      tbx_DatumDO.Tag = dtp_DatumDO;

      tbx_DatumOD.ContextMenu = dtp_DatumOD.ContextMenu =
      tbx_DatumDO.ContextMenu = dtp_DatumDO.ContextMenu = CreateNewContexMenu_Date();

      SetUpAsWriteOnlyTbx(hamper);
      // VvHamper.HamperStyling(hamper);
      VvHamper.AddLabelLine(hamper);

      // 12.06.2012: 
      tbx_DatumOD.TextChanged += new EventHandler(CompDateOdExitMethod);
      tbx_DatumDO.TextChanged += new EventHandler(CompDateOdExitMethod);
   }

   private void InitializeHamper_OdDo_Artikl(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 4, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q4un - ZXC.Qun4 - ZXC.Qun8, ZXC.Q4un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvRowHgt[2] = ZXC.QUN;
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Šifra OdDo:", ContentAlignment.MiddleRight);
      tbx_artiklCdOD = hamper.CreateVvTextBox(1, 0, "tbx_artiklCdOD", "Od šifre artikla");
      tbx_artiklCdDO = hamper.CreateVvTextBox(2, 0, "tbx_artiklCdDO", "Do šifre artikla");
      hamper.CreateVvLabel(0, 1, "Naziv OdDo:", ContentAlignment.MiddleRight);
      tbx_artiklNazivOD = hamper.CreateVvTextBox(1, 1, "tbx_artiklNazivOD", "Od naziva artikla");
      tbx_artiklNazivDO = hamper.CreateVvTextBox(2, 1, "tbx_artiklNazivDO", "Do naziva artikla");

      tbx_artiklCdOD.JAM_SetAutoCompleteData(Artikl.recordName, Artikl.sorterCD.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Artikl_sorterSifra), null);
      tbx_artiklCdDO.JAM_SetAutoCompleteData(Artikl.recordName, Artikl.sorterCD.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Artikl_sorterSifra), null);

      tbx_artiklNazivOD.JAM_SetAutoCompleteData(Artikl.recordName, Artikl.sorterName.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Artikl_sorterName), null);
      tbx_artiklNazivDO.JAM_SetAutoCompleteData(Artikl.recordName, Artikl.sorterName.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Artikl_sorterName), null);

      cbx_onlyArtiklSaStanjem = hamper.CreateVvCheckBox_OLD(0, 2, null, 2, 0, "Artikli sa stanjem", System.Windows.Forms.RightToLeft.Yes);
      cbx_onlyArtiklSaStanjem.Checked = true;

      VvHamper.AddLabelLine(hamper);

      SetUpAsWriteOnlyTbx(hamper);

      //    VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_OdDo_TT(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 2, "", this, false);

      hamper.VvColWdt = new int[] { ZXC.Q5un + ZXC.Qun8, ZXC.Q3un + ZXC.Qun4, ZXC.Q3un + ZXC.Qun4 };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "TT brojOdDo:", ContentAlignment.MiddleRight);
      tbx_fakturOD = hamper.CreateVvTextBox(1, 0, "tbx_fakturOD", "Od dokumenta broj", 7);
      tbx_fakturOD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_fakturOD.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      tbx_fakturDO = hamper.CreateVvTextBox(2, 0, "tbx_fakturDO", "Do dokumenta broj", 7);
      tbx_fakturDO.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_fakturDO.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      SetUpAsWriteOnlyTbx(hamper);
      VvHamper.AddLabelLine(hamper);

      // VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_Partner(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 6, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q2un + ZXC.Qun8, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun2, ZXC.Q4un + ZXC.Qun2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun12, ZXC.Qun12, ZXC.Qun12, ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel  (0, 0, "Partner:", ContentAlignment.MiddleRight);
      tbx_KD_sifra  = hamper.CreateVvTextBox(1, 0, "tbx_KD_sifra", "Šifra partnera", 6);
      tbx_KD_ticker = hamper.CreateVvTextBox(2, 0, "tbx_KD_ticker", "Ticker partnera", 6);
      tbx_KD_naziv  = hamper.CreateVvTextBox(3, 0, "tbx_KupDob", "Naziv partnera");

      tbx_KD_sifra .JAM_MustTabOutBeforeSubmit = true;
      tbx_KD_ticker.JAM_MustTabOutBeforeSubmit = true;
      tbx_KD_naziv .JAM_MustTabOutBeforeSubmit = true;

      tbx_KD_sifra .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyKupdobTextBoxLeave));
      tbx_KD_ticker.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_KD_naziv .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyKupdobTextBoxLeave));

      tbx_KD_ticker.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_KD_sifra .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_KD_sifra .JAM_MarkAsNumericTextBox(0, false);
      tbx_KD_sifra .TextAlign = HorizontalAlignment.Left;
      tbx_KD_sifra .JAM_FillCharacter = '0';

      tbx_KD_sifra.JAM_WriteOnly = tbx_KD_ticker.JAM_WriteOnly = tbx_KD_naziv.JAM_WriteOnly = true;

                      hamper.CreateVvLabel  (0, 1, "MjTroš:", ContentAlignment.MiddleRight);
      tbx_MT_sifra  = hamper.CreateVvTextBox(1, 1, "tbx_MT_sifra", "Šifra mjesta troška", 6);
      tbx_MT_ticker = hamper.CreateVvTextBox(2, 1, "tbx_MT_ticker", "Ticker mjesta troška", 6);
      tbx_MT_naziv  = hamper.CreateVvTextBox(3, 1, "tbx_MTnaziv", "Naziv mjesta troška");

      tbx_MT_sifra.JAM_MarkAsNumericTextBox(0, false);
      tbx_MT_sifra.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_MT_sifra.JAM_FillCharacter = '0';
      tbx_MT_sifra.TextAlign = HorizontalAlignment.Left;
      tbx_MT_ticker.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_MT_sifra .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_MT_ticker.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_MT_naziv .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyMtrosTextBoxLeave));

      cbx_isPoslJed = hamper.CreateVvCheckBox_OLD(1, 2, null, /*2, 0, */"PoslJed kao Partner", System.Windows.Forms.RightToLeft.No); // poslovna jedinica kao partner

                      hamper.CreateVvLabel        (2, 2, "Grupa:", ContentAlignment.MiddleRight);
      tbx_TipKupDob = hamper.CreateVvTextBoxLookUp(3, 2, "tbx_TipKupDob", "Proizvoljna sifra, grupa partnera - mogućnost pripadnosti u više grupa", 10);
      tbx_TipKupDob.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_TipKupDob.JAM_Set_LookUpTable(ZXC.luiListaGrupaPartnera, (int)ZXC.Kolona.prva);
      tbx_TipKupDob.JAM_lookUp_NOTobligatory  = true;
      tbx_TipKupDob.JAM_lookUp_MultiSelection = true;

                     hamper.CreateVvLabel    (0, 3, "Komerc.:", ContentAlignment.MiddleRight);
      tbx_personCD = hamper.CreateVvTextBox  (1, 3, "tbx_PersonCD", "Putnik", 6);
      tbx_personName = hamper.CreateVvTextBox(2, 3, "tbx_PersonName", "Putnik", 64, 1, 0);
      tbx_personCD.JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Person_sorterSifra), new EventHandler(AnyPersonTextBoxLeave));
      tbx_personCD  .JAM_CharEdits       = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_personName.JAM_CharacterCasing = CharacterCasing.Upper;

      cbx_FRUF_BiloGdjeU = hamper.CreateVvCheckBox_OLD(0, 4, null, "", RightToLeft.No);

      tbx_FRUF_Name = hamper.CreateVvTextBoxLookUp(1, 4, "tbx_FRUF_Name", "FRUF_Name");
      tbx_FRUF_Name.JAM_Set_LookUpTable(ZXC.luiListaFakRptUniFilter, (int)ZXC.Kolona.prva);
      tbx_FRUF_Value = hamper.CreateVvTextBox(2, 4, "tbx_FRUF_Value", "FRUF_Value", 64, 1, 0);
      
      //VvHamper.HamperStyling(hamper);
      VvHamper.AddLabelLine(hamper);

   }

   void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(TheVvUC.isPopulatingSifrar) return;

      //if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.TheVvUC.originalText)
      {
         this.TheVvUC.originalText = tb.Text;
         kupdob_rec = VvUserControl.KupdobSifrar.Find(TheVvUC.FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_KD_naziv = kupdob_rec.Naziv;
            Fld_KD_sifra = kupdob_rec.KupdobCD/*RecID*/;
            Fld_KD_ticker = kupdob_rec.Ticker;
         }
         else
         {
            Fld_KD_naziv = Fld_KD_ticker = Fld_KD_SifraAsTxt = "";
         }
      }
   }

   void AnyMtrosTextBoxLeave(object sender, EventArgs e)
   {
      if(TheVvUC.isPopulatingSifrar) return;

      //if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.TheVvUC.originalText)
      {
         this.TheVvUC.originalText = tb.Text;

         kupdob_rec = VvUserControl.KupdobSifrar.Find(TheVvUC.FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_MT_sifra = kupdob_rec.KupdobCD/*RecID*/;
            Fld_MT_ticker = kupdob_rec.Ticker;
            Fld_MT_naziv = kupdob_rec.Naziv;
         }
         else
         {
            Fld_MT_SifraAsTxt = Fld_MT_ticker = Fld_MT_naziv = "";
         }
      }
   }

   public void AnyPersonTextBoxLeave(object sender, EventArgs e)
   {
      if(TheVvUC.isPopulatingSifrar) return;

      TextBox tb = sender as TextBox;
      Person person_rec;

      if(tb.Text != this.TheVvUC.originalText)
      {
         this.TheVvUC.originalText = tb.Text;

         person_rec = VvUserControl.PersonSifrar.Find(TheVvUC.FoundInSifrar<Person>);

         if(person_rec != null && tb.Text != "")
         {
            Fld_Putnik_PersonCD = person_rec.PersonCD/*RecID*/;
            Fld_Putnik_PersonName = person_rec.PrezimeIme;
         }
         else
         {
            Fld_PersonCdAsTxt = Fld_Putnik_PersonName = "";
         }
      }
   }

   private void InitializeHamper_TTSklad(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 8, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q2un + ZXC.Qun2, ZXC.Q3un, ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8,            ZXC.Qun8, ZXC.Qun8, ZXC.Qun8};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvRowHgt[0] = ZXC.QUN - ZXC.Qun8;
      hamper.VvRowHgt[3] = ZXC.Qun8;
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbx_isPrihodTT = hamper.CreateVvCheckBox_OLD(0, 0, null, 3, 0, "Prihod: IFA, IRA, IRM, IOD, IPV", System.Windows.Forms.RightToLeft.No);
      cbx_isPrihodTT.CheckedChanged += new EventHandler(IsPrihodTT_Checked);

      cbx_isRashodTT = hamper.CreateVvCheckBox_OLD(0, 1, null, 3, 0, "Rashod: UFA, URA, URM, UOD, UPV", System.Windows.Forms.RightToLeft.No);
      cbx_isRashodTT.CheckedChanged += new EventHandler(IsRashodTT_Checked);

                     hamper.CreateVvLabel        (0, ZXC.IsSvDUH ? 0 : 2, "TipTrans:", ContentAlignment.MiddleRight);
      tbxLookUp_TT = hamper.CreateVvTextBoxLookUp(1, ZXC.IsSvDUH ? 0 : 2, "tbxLookUp_TT", "Tip transakcije");
      tbx_TTOpis   = hamper.CreateVvTextBox      (2, ZXC.IsSvDUH ? 0 : 2, "tbx_TTOpis_InVisible", "Opis tipa transkacije", 32, 1, 0);
      tbxLookUp_TT.JAM_CharacterCasing = CharacterCasing.Upper;
      tbxLookUp_TT.JAM_Set_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbxLookUp_TT.JAM_lui_NameTaker_JAM_Name = tbx_TTOpis.JAM_Name;
      tbx_TTOpis.JAM_ReadOnly = true;
      tbx_TTOpis.Tag = ZXC.vvColors.userControl_BackColor;
      //     tbxLookUp_TT.JAM_DataRequired = true;
      Label lbCrta = hamper.CreateVvLabel(0, 3, "", ContentAlignment.MiddleLeft);
      lbCrta.Size = new System.Drawing.Size(ZXC.Q10un + ZXC.Q5un, ZXC.Qun12);
      lbCrta.BackColor = Color.DarkGray;

                      hamper.CreateVvLabel        (0, ZXC.IsSvDUH ? 2 : 4, "Sklad1:", ContentAlignment.MiddleRight);
      tbx_skladCd   = hamper.CreateVvTextBoxLookUp(1, ZXC.IsSvDUH ? 2 : 4, "tbx_skladCd", "Šifra skladišta");
      tbx_skladOpis = hamper.CreateVvTextBox      (2, ZXC.IsSvDUH ? 2 : 4, "tbx_skladOpis_InVisible", "Naziv skladišta", 32, 1, 0);
      tbx_skladCd.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_skladCd.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_skladCd.JAM_lui_NameTaker_JAM_Name = tbx_skladOpis.JAM_Name;
      //  tbx_skladCd.Leave       += new EventHandler(/*TheVvUC.*/tbx_zaSkladCD_Leave_RememberTheLastUsedSkladCD);
      //tbx_skladCd.TextChanged += new EventHandler(TheVvUC.tbx_zaSkladCD_TextChanged_RememberTheLastUsedSkladCD);

      tbx_skladOpis.JAM_ReadOnly = true;
      tbx_skladOpis.Tag = ZXC.vvColors.userControl_BackColor;
      
      lbl_grupa_skl2 = hamper.CreateVvLabel        (0, 5, "Sklad2:", ContentAlignment.MiddleRight);
      tbx_skladCd2   = hamper.CreateVvTextBoxLookUp(1, 5, "tbx_skladCd2"  , "Šifra skladišta2");
      tbx_skladOpis2 = hamper.CreateVvTextBox      (2, 5, "tbx_skladOpis2", "Naziv skladišta2", 32, 1, 0);
      tbx_skladCd2.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_skladCd2.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_skladCd2.JAM_lui_NameTaker_JAM_Name = tbx_skladOpis2.JAM_Name;

      tbx_skladOpis2.JAM_ReadOnly = true;
      tbx_skladOpis2.Tag = ZXC.vvColors.userControl_BackColor;


      rbt_svaSklad = hamper.CreateVvRadioButton(0, 5, null, "SvaSkl", TextImageRelation.ImageBeforeText);
      rbt_svaSklad.Checked = true;
      rbt_svaSklad.Tag = true;

      rbt_onlyVelepSkl = hamper.CreateVvRadioButton(1, 5, null, "Velep", TextImageRelation.ImageBeforeText);
      rbt_onlyMalopSkl = hamper.CreateVvRadioButton(2, 5, null, "Malop", TextImageRelation.ImageBeforeText);
      rbt_TMB_VpskVps2 = hamper.CreateVvRadioButton(3, 5, null, "TMB"  , TextImageRelation.ImageBeforeText);

      cbx_isChkO      = hamper.CreateVvCheckBox_OLD(0, 6, null, "Chk0"       , System.Windows.Forms.RightToLeft.No);
      cbx_isMpskPoNBC = hamper.CreateVvCheckBox_OLD(2, 6, null, 1, 0, "MPSK po NBC", System.Windows.Forms.RightToLeft.No);

      lbl_ThKune = hamper.CreateVvLabel  (0, 6, "Iznos u kunama:", 1, 0, ContentAlignment.MiddleRight);
      tbx_ThKune = hamper.CreateVvTextBox(2, 6, "tbx_ThKune", "Iznos u kunama", 12, 1, 0);
      tbx_ThKune.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      lbl_ThKune.Visible = tbx_ThKune.Visible = ZXC.IsTEXTHOany2;


      if(ZXC.IsSvDUH) cbx_IsBlgInIzvVal = hamper.CreateVvCheckBox_OLD(0, 6, null, 3, 0, "Alternativa" , System.Windows.Forms.RightToLeft.No);
      else            cbx_IsBlgInIzvVal = hamper.CreateVvCheckBox_OLD(2, 6, null, 1, 0, "BLG u IzvVal", System.Windows.Forms.RightToLeft.Yes);

      if(ZXC.IsSvDUH)
      {
         cbx_isPrihodTT.Visible = cbx_isRashodTT.Visible   = tbx_skladCd2.Visible     = lbl_grupa_skl2.Visible   = 
         rbt_svaSklad.Visible   = rbt_onlyVelepSkl.Visible = rbt_onlyMalopSkl.Visible = rbt_TMB_VpskVps2.Visible = 
         cbx_isChkO.Visible     = cbx_isMpskPoNBC.Visible  = false;
         cbx_IsBlgInIzvVal.Visible = false; // 04.12.2019. ovo vise ne koristimo za ista pa je bolje da se ne vidi da ne zbunjuje
      }

      VvHamper.AddLabelLine(hamper);
      //VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_ProjektNacPlac(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", this, false);

      hamper.VvColWdt = new int[] { ZXC.Q2un, ZXC.Q4un - ZXC.Qun2, ZXC.Q2un, ZXC.Q4un };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvRowHgt[0] = ZXC.QUN - ZXC.Qun8;
      hamper.VvBottomMargin = hamper.VvTopMargin;

                    hamper.CreateVvLabel  (0, 0, ZXC.IsSvDUH ? "Ugovor:" : "Projekt:", ContentAlignment.MiddleRight);
      tbx_Projekt = hamper.CreateVvTextBox(1, 0, "tbx_Projekt", "Projekt");
      tbx_Projekt.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_Projekt.JAM_SetAutoCompleteData(Faktur.recordName, Faktur.sorterTtNum.SortType, ZXC.AutoCompleteRestrictor.FAK_ExactTT_Only, TheVvUC.OnVvTBEnter_SetAutocmplt_Faktur_DUMMY, null);

      if(ZXC.IsSvDUH == false) hamper.CreateVvLabel(2, 0, "NačPl:", ContentAlignment.MiddleRight);
      tbx_NacPlac            = hamper.CreateVvTextBoxLookUp(3, 0, "tbx_NacPlac", "Način plaćanja");

      tbx_NacPlac.JAM_Set_LookUpTable(ZXC.luiListaRiskVrstaPl, (int)ZXC.Kolona.prva);

      if(ZXC.IsSvDUH) tbx_NacPlac.Visible = false;

      //VvHamper.HamperStyling(hamper);
      VvHamper.AddLabelLine(hamper);

   }

   public void IsPrihodTT_Checked(object sender, EventArgs ea)
   {
      CheckBox cBox = sender as CheckBox;
      if(cBox.Checked)
      {
         VvHamper.Open_Close_Fields_ForWriting(tbxLookUp_TT, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvReportUC);
         Fld_TT = "";
         cbx_isRashodTT.Enabled = false;
      }
      else
      {
         VvHamper.Open_Close_Fields_ForWriting(tbxLookUp_TT, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
         Fld_TT = ZXC.TheVvForm.VvPref.reportPrefs.RiskTT;
         cbx_isRashodTT.Enabled = true;

      }
   }
   public void IsRashodTT_Checked(object sender, EventArgs ea)
   {
      CheckBox cBox = sender as CheckBox;
      if(cBox.Checked)
      {
         VvHamper.Open_Close_Fields_ForWriting(tbxLookUp_TT, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvReportUC);
         Fld_TT = "";
         cbx_isPrihodTT.Enabled = false;

      }
      else
      {
         VvHamper.Open_Close_Fields_ForWriting(tbxLookUp_TT, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
         Fld_TT = ZXC.TheVvForm.VvPref.reportPrefs.RiskTT;
         cbx_isPrihodTT.Enabled = true;

      }
   }

   private void InitializeHamper_Sort(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q3un +  ZXC.Qun2, ZXC.Q3un - ZXC.Qun4, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN - ZXC.Qun8;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Slijed ispisa:", ContentAlignment.MiddleLeft);
      rbSort_Datum = hamper.CreateVvRadioButton(1, 0, null, "Datum", TextImageRelation.ImageBeforeText);
      rbSort_Datum.Checked = true;
      rbSort_Datum.Tag = true;
      rbSort_Broj = hamper.CreateVvRadioButton(2, 0, null, "Broj", TextImageRelation.ImageBeforeText);
      rbSort_Partner = hamper.CreateVvRadioButton(3, 0, null, "Partner", TextImageRelation.ImageBeforeText);

      //VvHamper.HamperStyling(hamper);
      VvHamper.AddLabelLine(hamper);

   }

   private void InitializeHamper_ArtSort(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 3, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q3un - ZXC.Qun4, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN - ZXC.Qun8;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Slijed ispisa:", ContentAlignment.MiddleLeft);
      rbArtSort_ByName = hamper.CreateVvRadioButton(1, 0, null, "Naziv", TextImageRelation.ImageBeforeText);
      rbArtSort_ByName.Checked = true;
      rbArtSort_ByName.Tag = true;
      rbArtSort_BySifra   = hamper.CreateVvRadioButton(2, 0, null, "Šifra", TextImageRelation.ImageBeforeText);
      rbArtSort_ByBarCode = hamper.CreateVvRadioButton(3, 0, null, ZXC.IsSvDUH ? "s_lio" : "Bar kod", TextImageRelation.ImageBeforeText);

                           hamper.CreateVvLabel      (0, 1, "TopSort:", ContentAlignment.MiddleLeft);
      rbArtSort_topByKol    = hamper.CreateVvRadioButton(1, 1, null, "Količina", TextImageRelation.ImageBeforeText);
      rbArtSort_topByFin    = hamper.CreateVvRadioButton(2, 1, null, "Iznos"   , TextImageRelation.ImageBeforeText);
      rbArtSort_topByRuc = hamper.CreateVvRadioButton(3, 1, null, "RUC"     , TextImageRelation.ImageBeforeText);

      // VvHamper.HamperStyling(hamper);
      VvHamper.AddLabelLine(hamper);

   }

   private void InitializeHamper_GrupirajDok(out VvHamper hamper)
   {
      bool isTH = ZXC.IsTEXTHOany/* && ZXC.vvDB_ServerID != ZXC.vvDB_ServerID_CENTRALA*/;

      hamper = new VvHamper(3, isTH ? 8 : 7, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q4un - ZXC.Qun2, ZXC.Q4un, ZXC.Q4un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8           , ZXC.Qun8, ZXC.Qun8            };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN - ZXC.Qun8;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

    //hamper.CreateVvLabel(0, 0, "Grupiraj Dokumente po:", 1, 0, ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(0, 0, "GrupDok po:"                    , ContentAlignment.MiddleLeft);

    //rbt_grupNull = hamper.CreateVvRadioButton(2, 0, null, "NE", TextImageRelation.ImageBeforeText);
      rbt_grupNull = hamper.CreateVvRadioButton(1, 0, null, "NE", TextImageRelation.ImageBeforeText);
      rbt_grupNull.Checked = true;
      rbt_grupNull.Tag = true;

      rbt_grupGodina    = hamper.CreateVvRadioButton(2, 0, null, "Godini", TextImageRelation.ImageBeforeText);

      rbt_grupTH_CycleM = hamper.CreateVvRadioButton(0, 1, null, "TH_CycleMoment", 2, 0, TextImageRelation.ImageBeforeText);

      rbt_grupPartner   = hamper.CreateVvRadioButton(0, isTH ? 2 : 1, null, "Partneru"                          , TextImageRelation.ImageBeforeText);
      rbt_grupUser      = hamper.CreateVvRadioButton(0, isTH ? 3 : 2, null, "User-u"                            , TextImageRelation.ImageBeforeText);
      rbt_grupMjTros    = hamper.CreateVvRadioButton(0, isTH ? 4 : 3, null, "MjTroška"                          , TextImageRelation.ImageBeforeText);
      rbt_grupPosJed    = hamper.CreateVvRadioButton(0, isTH ? 5 : 4, null, "PoslJed"                           , TextImageRelation.ImageBeforeText);
    //rbt_grupValuta    = hamper.CreateVvRadioButton(0, isTH ? 6 : 5, null, "IzvValuti"                         , TextImageRelation.ImageBeforeText); //19.11.2013. vratit cemo po potrebi
      rbt_grupPutnik    = hamper.CreateVvRadioButton(0, isTH ? 6 : 5, null, "Komerc."                           , TextImageRelation.ImageBeforeText);
      rbt_grupProjekt   = hamper.CreateVvRadioButton(1, isTH ? 2 : 1, null, ZXC.IsSvDUH ? "Ugovoru" : "Projektu", TextImageRelation.ImageBeforeText);
      rbt_grupNacPl     = hamper.CreateVvRadioButton(1, isTH ? 3 : 2, null, "NačinuPl"                          , TextImageRelation.ImageBeforeText);
      rbt_grupTT        = hamper.CreateVvRadioButton(1, isTH ? 4 : 3, null, "TT"                                , TextImageRelation.ImageBeforeText);
      rbt_grupSkladCD   = hamper.CreateVvRadioButton(1, isTH ? 5 : 4, null, "Skladištima"                       , TextImageRelation.ImageBeforeText);
      rbt_grupMonth     = hamper.CreateVvRadioButton(1, isTH ? 6 : 5, null, "Mjesecima"                         , TextImageRelation.ImageBeforeText);
      rbt_grupWeek      = hamper.CreateVvRadioButton(2, isTH ? 2 : 1, null, "Tjednima"                          , TextImageRelation.ImageBeforeText);
      rbt_grupDay       = hamper.CreateVvRadioButton(2, isTH ? 3 : 2, null, "Danima"                            , TextImageRelation.ImageBeforeText);
      rbt_grupHour      = hamper.CreateVvRadioButton(2, isTH ? 4 : 3, null, "Satima"                            , TextImageRelation.ImageBeforeText);
      rbt_grupDayOfWeek = hamper.CreateVvRadioButton(2, isTH ? 5 : 4, null, "Dan u Tjednu"                      , TextImageRelation.ImageBeforeText);
      rbt_grupHourOfDay = hamper.CreateVvRadioButton(2, isTH ? 6 : 5, null, "Sat u Danu"                        , TextImageRelation.ImageBeforeText);

      if(!isTH) rbt_grupTH_CycleM.Visible = false;

      if(ZXC.IsSvDUH)
      {
         rbt_grupUser     .Visible = 
         rbt_grupMjTros   .Visible = 
         rbt_grupPosJed   .Visible = 
         rbt_grupPutnik   .Visible = 
         rbt_grupNacPl    .Visible = 
         rbt_grupWeek     .Visible =
         rbt_grupDay      .Visible =
         rbt_grupHour     .Visible =
         rbt_grupDayOfWeek.Visible =
         rbt_grupHourOfDay.Visible = false;
      }

      //VvHamper.HamperStyling(hamper);
      VvHamper.AddLabelLine(hamper);

   }

   private void InitializeHamper_GrupirajArtikl(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 5, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q3un - ZXC.Qun2, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN - ZXC.Qun8;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
      hamper.VvSpcBefRow[2]   = ZXC.Qun12 ;
      hamper.VvRowHgt[3]   = ZXC.QUN ;

      hamper.CreateVvLabel(0, 0, "Grup Artikle po:", 1, 0, ContentAlignment.MiddleLeft);
      rbt_grupArtNull = hamper.CreateVvRadioButton(3, 0, null, "NE", TextImageRelation.ImageBeforeText);
      rbt_grupArtNull.Checked = true;
      rbt_grupArtNull.Tag = true;
      rbt_grupArtTs = hamper.CreateVvRadioButton(0, 1, null, "Tipu", TextImageRelation.ImageBeforeText);
      rbt_grupArtGr1 = hamper.CreateVvRadioButton(1, 1, null, "GrupiA", TextImageRelation.ImageBeforeText);
      rbt_grupArtGr2 = hamper.CreateVvRadioButton(2, 1, null, "GrupiB", TextImageRelation.ImageBeforeText);
      rbt_grupArtGr3 = hamper.CreateVvRadioButton(3, 1, null, ZXC.IsSPSISTdemo ? "Kat.Br.": "GrupiC", TextImageRelation.ImageBeforeText);

      if(ZXC.IsSvDUH == false) hamper.CreateVvLabel        (0, 2, "SamoTip:", ContentAlignment.MiddleRight);
      tbx_Artikl_TS          = hamper.CreateVvTextBoxLookUp(0, 3, "tbx_artikl_TS", "Tip artikla");
      tbx_Artikl_TS.JAM_Set_LookUpTable(ZXC.luiListaArtiklTS, (int)ZXC.Kolona.prva);

                       hamper.CreateVvLabel        (1, 2, "SamoGrA:", ContentAlignment.MiddleRight);
      tbx_Artikl_Gr1 = hamper.CreateVvTextBoxLookUp(1, 3, "tbx_artikl_Gr1", "Grupa A artikla");
      tbx_Artikl_Gr1.JAM_Set_LookUpTable(ZXC.luiListaGrupa1Artikla, (int)ZXC.Kolona.prva);

                       hamper.CreateVvLabel        (2, 2, "SamoGrB:", ContentAlignment.MiddleRight);
      tbx_Artikl_Gr2 = hamper.CreateVvTextBoxLookUp(2, 3, "tbx_artikl_Gr2", "Grupa B artikla");
      tbx_Artikl_Gr2.JAM_Set_LookUpTable(ZXC.luiListaGrupa2Artikla, (int)ZXC.Kolona.prva);

                       hamper.CreateVvLabel        (3, 2, "SamoGrC:", ContentAlignment.MiddleRight);
      tbx_Artikl_Gr3 = hamper.CreateVvTextBoxLookUp(3, 3, "tbx_artikl_Gr3", "Grupa C artikla");
      tbx_Artikl_Gr3.JAM_Set_LookUpTable(ZXC.luiListaGrupa3Artikla, (int)ZXC.Kolona.prva);

      if(ZXC.IsSvDUH)
      {
         rbt_grupArtTs.Visible = tbx_Artikl_TS.Visible = false;
      }


      VvHamper.AddLabelLine(hamper);

      //VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_GroupTopSort(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 3, "", this, false);

      hamper.VvColWdt = new int[] { ZXC.Q3un + ZXC.Qun2 + ZXC.Qun8, ZXC.Q4un, ZXC.Q2un, ZXC.Q2un - ZXC.Qun8 };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun10;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbx_topSort    = hamper.CreateVvCheckBox_OLD(0, 0, null            , "Top sort", RightToLeft.No);
      cbx_topGroups  = hamper.CreateVvCheckBox_OLD(1, 0, null            , "Prikaži Top", RightToLeft.No);
      tbx_NumOfGrups = hamper.CreateVvTextBox     (2, 0, "tbx_NumOfGrups", "Top broj grupa");
      lbl_grupa      = hamper.CreateVvLabel       (3, 0, "grupa"         , ContentAlignment.MiddleLeft);

      cbx_ocuGraf      = hamper.CreateVvCheckBox_OLD(0, 1, null, "Graf", RightToLeft.No);
      cbx_visiblePosto = hamper.CreateVvCheckBox_OLD(1, 1, null, "% u Ukupnom", RightToLeft.No);
      cbx_grupaPoStr   = hamper.CreateVvCheckBox_OLD(2, 1, null, 1, 0, "Grupa/Strani", RightToLeft.No);

      cbx_ocuGraf.Checked = true;

      if(ZXC.IsSvDUH) // 09.05.2018.
      {
         cbx_topSort.Visible = cbx_topGroups.Visible = tbx_NumOfGrups.Visible = lbl_grupa.Visible = cbx_visiblePosto.Visible = false;
      }

      // VvHamper.HamperStyling(hamper);
      VvHamper.AddLabelLine(hamper);

   }

   private void InitializeHamper_PdvRtip(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", this, false);

      hamper.VvColWdt = new int[] { ZXC.Q2un - ZXC.Qun4, ZXC.Q2un - ZXC.Qun8, ZXC.Q2un - ZXC.Qun8, ZXC.Q2un - ZXC.Qun8 };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun12, ZXC.Qun12, 0, 0 };
      hamper.VvRightMargin = ZXC.Qun8;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                               hamper.CreateVvLabel(0, 0, "RTip:", ContentAlignment.MiddleRight);
      rbt_pdvSviR            = hamper.CreateVvRadioButton(1, 0, null, "Svi", TextImageRelation.ImageBeforeText);
      rbt_pdvSviR.Checked = true;
      rbt_pdvSviR.Tag = true;
      rbt_pdvR1 = hamper.CreateVvRadioButton(2, 0, null, "R1", TextImageRelation.ImageBeforeText);
      rbt_pdvR2 = hamper.CreateVvRadioButton(3, 0, null, "R2", TextImageRelation.ImageBeforeText);

      VvHamper.AddLabelLine(hamper);

      //VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_Status(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 2, "", this, false);

      hamper.VvColWdt = new int[] { ZXC.Q2un, ZXC.Q2un };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Status:", ContentAlignment.MiddleRight);
      tbx_Status = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_Status", "Status dokumenta");
      tbx_Status.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_Status.JAM_Set_LookUpTable(ZXC.luiListaRiskStatus, (int)ZXC.Kolona.prva);
      tbx_Status.JAM_lookUp_MultiSelection = true;

      VvHamper.AddLabelLine(hamper);

   }

   private void InitializeHamper_OdDo_DateComp(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 2, "", this, false);

      hamper.VvColWdt = new int[] { ZXC.Q4un - ZXC.Qun4, ZXC.Q4un, ZXC.Q4un };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "UsporedOdDo:", ContentAlignment.MiddleRight);
      tbx_compDateOd = hamper.CreateVvTextBox(1, 0, "tbx_compDateOd", "Od datuma");
      tbx_compDateOd.JAM_IsForDateTimePicker = true;
      dtp_compDateOd = hamper.CreateVvDateTimePicker(1, 0, "", tbx_compDateOd);
      dtp_compDateOd.Name = "dtp_compDateOd";
      dtp_compDateOd.Tag = tbx_compDateOd;
      tbx_compDateOd.Tag = dtp_compDateOd;

      tbx_compDateDo = hamper.CreateVvTextBox(2, 0, "tbx_compDateDo", "");
      tbx_compDateDo.JAM_IsForDateTimePicker = true;
      dtp_compDateDo = hamper.CreateVvDateTimePicker(2, 0, "", tbx_compDateDo);
      dtp_compDateDo.Name = "dtp_compDateDo";
      dtp_compDateDo.Tag = tbx_compDateDo;
      tbx_compDateDo.Tag = dtp_compDateDo;

      tbx_compDateOd.JAM_WriteOnly = true;
      tbx_compDateDo.JAM_ReadOnly = true;

      //dtp_compDateOd.Leave += new EventHandler(CompDateOdExitMethod);
      tbx_compDateOd.TextChanged += new EventHandler(CompDateOdExitMethod);
      VvHamper.AddLabelLine(hamper);
   }

   private void InitializeHamper_RptNapomena(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 5, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q3un-ZXC.Qun2 , ZXC.Q10un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun12, ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                          hamper.CreateVvLabel  (0, 0, "Naslov:", ContentAlignment.MiddleRight);
      tbx_rptNaziv     = hamper.CreateVvTextBox(1, 0, "tbx_nazivReporta", "");
                          hamper.CreateVvLabel  (0, 1, "Napomena:", ContentAlignment.MiddleRight);
      tbx_rptNapomena = hamper.CreateVvTextBox(1, 1, "tbx_napomenaOnRpt", "Napomena", 1024, 0, 2);
      tbx_rptNapomena.Multiline = true;
      tbx_rptNapomena.ScrollBars = ScrollBars.Vertical;

      VvHamper.AddLabelLine(hamper);
   }


   private void InitializeHamper_SvaSklad(out VvHamper hamper)
   {
      int numOfSklad = ZXC.luiListaSkladista.Count;
      
      //uint skladBR = (uint)ZXC.luiListaSkladista.GetIntegerForThisCd(skladCD);

      cbx_sklad = new CheckBox[numOfSklad];
      hamper    = new VvHamper(2, numOfSklad/2 + 2, "", this, false);
      
      hamper.VvColWdt    = new int[] { ZXC.Q5un, ZXC.Q5un};
      hamper.VvSpcBefCol = new int[] { ZXC.Qun4 , ZXC.Qun4};
      hamper.VvRightMargin = 0;

      for(int i = 0; i < numOfSklad/2 + 2; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN - ZXC.Qun8;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      int hampRow  = 0;
      int hampRow2 = 0;
     //int hampRow3 = 0;


      for(int j = 0; j < numOfSklad; ++j)
      {
            if(j < numOfSklad / 2)
            {
               cbx_sklad[j] = hamper.CreateVvCheckBox_OLD(0, hampRow, null, " iz lui liste",  System.Windows.Forms.RightToLeft.No);
               hampRow += 1;
            }
            else if(j >= numOfSklad / 2)
            {
               cbx_sklad[j] = hamper.CreateVvCheckBox_OLD(1, hampRow2, null, " iz lui liste", System.Windows.Forms.RightToLeft.No);
               hampRow2 += 1;
            }
            //else if(j > 41 && j< 60)
            //{
            //   cbx_sklad[j] = hamper.CreateVvCheckBox_OLD(2, hampRow3, null, " iz lui liste", System.Windows.Forms.RightToLeft.No);
            //   hampRow3 += 1;
            //}

            //cbx_sklad[j].Tag = vvSubModul;
         }

     // hamper.Height = hampRow2 * (ZXC.QUN + ZXC.Qun4) + ZXC;
      VvHamper.AddLabelLine(hamper);

      hamper.Visible = false;
   }

   private void InitializeHamper_OdDo_VezDok2(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 2, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q5un- ZXC.Qun8, ZXC.Q4un- ZXC.Qun2, ZXC.Q4un- ZXC.Qun2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                     hamper.CreateVvLabel  (0, 0, ZXC.IsSvDUHdomena ? "KnjigBrOdDo:" : "VezDok2OdDo:", ContentAlignment.MiddleRight);
      tbx_vezDok2OD = hamper.CreateVvTextBox(1, 0, "tbx_vezDok2OD", "Od dokumenta broj", 14);
      tbx_vezDok2OD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_vezDok2OD.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      tbx_vezDok2DO = hamper.CreateVvTextBox(2, 0, "tbx_vezDok2DO", "Do dokumenta broj", 14);
      tbx_vezDok2DO.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_vezDok2DO.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      SetUpAsWriteOnlyTbx(hamper);
      VvHamper.AddLabelLine(hamper);

      // VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_SvdLiP(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un, ZXC.Q4un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun12, ZXC.Qun12, 0, 0 };
      hamper.VvRightMargin = ZXC.Qun8;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                          hamper.CreateVvLabel(0, 0, "Lijek/Potr:", ContentAlignment.MiddleRight);
      rbt_LiP     = hamper.CreateVvRadioButton(1, 0, null, "SviLP", TextImageRelation.ImageBeforeText);
      rbt_LiP.Checked = true;
      rbt_LiP   .Tag = true;
      rbt_Lijek   = hamper.CreateVvRadioButton(2, 0, null, "Lijek" , TextImageRelation.ImageBeforeText);
      rbt_Potros  = hamper.CreateVvRadioButton(3, 0, null, "Potrošni", TextImageRelation.ImageBeforeText);

      //VvHamper.AddLabelLine(hamper);

      //VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_SvdSkladDonac(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", this, false);

      hamper.VvColWdt     = new int[] { ZXC.Q3un, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un, ZXC.Q4un };
      hamper.VvSpcBefCol  = new int[] { ZXC.Qun12, ZXC.Qun12, 0, 0 };
      hamper.VvRightMargin = ZXC.Qun8;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel      (0, 0, "Sklad:", ContentAlignment.MiddleRight);
      rbt_svdSvaSkl = hamper.CreateVvRadioButton(1, 0, null, "Sva", TextImageRelation.ImageBeforeText);
      rbt_svdSvaSkl.Checked = true;
      rbt_svdSvaSkl.Tag = true;
      rbt_svdDonac   = hamper.CreateVvRadioButton(2, 0, null, "Donac", TextImageRelation.ImageBeforeText);
      rbt_svdNeDonac = hamper.CreateVvRadioButton(3, 0, null, "NeDonac", TextImageRelation.ImageBeforeText);


      //VvHamper.AddLabelLine(hamper);

      //VvHamper.HamperStyling(hamper);
   }


   private void InitializeHamper_UvozIzvoz(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 2, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q5un  };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun12 };
      hamper.VvRightMargin = ZXC.Qun8;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbx_uvozIzvozOnly = hamper.CreateVvCheckBox_OLD(0, 0, null, "Uvoz/Izvoz", System.Windows.Forms.RightToLeft.No);

      VvHamper.AddLabelLine(hamper);

      //VvHamper.HamperStyling(hamper);
   }


   #region OTS

   private void InitializeHamper_OTS(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 11, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q4un, ZXC.QUN };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN - ZXC.Qun10;
         hamper.VvSpcBefRow[i] = ZXC.Qun10;
      }
      hamper.VvSpcBefRow[0] = ZXC.Qun8;
      hamper.VvSpcBefRow[7] = ZXC.Qun8;
      hamper.VvRowHgt[6]    = ZXC.QUN;
      hamper.VvRowHgt[7]    = ZXC.QUN;
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbx_otsDospjelo   = hamper.CreateVvCheckBox_OLD(0, 0, null, 2, 0, "Ispiši samo dospjele račune"     , System.Windows.Forms.RightToLeft.No);
      cbx_otsAnalitika  = hamper.CreateVvCheckBox_OLD(0, 1, null, 2, 0, "Ispiši i kompenzacije / prijeboj", System.Windows.Forms.RightToLeft.No);
      cbx_otsDospjeca   = hamper.CreateVvCheckBox_OLD(0, 2, null, 2, 0, "Tablica po kategorijama dana"    , System.Windows.Forms.RightToLeft.No);
      cbx_otsKontakt    = hamper.CreateVvCheckBox_OLD(0, 3, null, 2, 0, "Ispiši kontakt informacije"      , System.Windows.Forms.RightToLeft.No);
      cbx_otsLineTipBr  = hamper.CreateVvCheckBox_OLD(0, 4, null, 2, 0, "Linija razdvajanja računa"       , System.Windows.Forms.RightToLeft.No);
      cbx_isOtsNevezano = hamper.CreateVvCheckBox_OLD(0, 5, null, 2, 0, "Ispiši samo nevezane račune"     , System.Windows.Forms.RightToLeft.No);

                   hamper.CreateVvLabel  (0, 6, "Kompenzacija Broj:", ContentAlignment.MiddleLeft);
      tbx_kompBr = hamper.CreateVvTextBox(1, 6, "tbx_kompBr", "Broj kompenzacije");

                     hamper.CreateVvLabel  (0, 7, "IOS na dan:", ContentAlignment.MiddleLeft);
      tbx_DatumOTS = hamper.CreateVvTextBox(1, 7, "tbx_datumOd", "");
      tbx_DatumOTS.JAM_IsForDateTimePicker = true;
      dtp_DatumOTS = hamper.CreateVvDateTimePicker(1, 7, "", tbx_DatumOTS);
      dtp_DatumOTS.Name = "dtp_DatumOD";
      dtp_DatumOTS.Tag = tbx_DatumOTS;
      tbx_DatumOTS.Tag = dtp_DatumOTS;

      cbx_isForceOtsByDokDate = hamper.CreateVvCheckBox_OLD(0, 8, null, 2, 0, "IOS sortiraj po datumu"          , System.Windows.Forms.RightToLeft.No);

      //15.02.2017. knjigovodstveni servisi u origBrDok=vezniDok cuvaj broj racuna svog korisnika pa da ga mou ispisati
      cbx_isOpzStatVezDok     = hamper.CreateVvCheckBox_OLD(0, 9, null, 2, 0, "OPZ-STAT broj računa = OrigBrDok", System.Windows.Forms.RightToLeft.No);

      // VvHamper.HamperStyling(hamper);
      VvHamper.AddLabelLine(hamper);

   }

   private void InitializeHamper_ColorVisible(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", this, false);
      
      hamper.VvColWdt      = new int[] { ZXC.Q10un + ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN - ZXC.Qun10;
         hamper.VvSpcBefRow[i] = ZXC.Qun10;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

    //cbx_isNoColor     = hamper.CreateVvCheckBox_OLD(0, 0, null, "Ispiši bez boja"                           , System.Windows.Forms.RightToLeft.No);
      cbx_isPecatPotpis = hamper.CreateVvCheckBox_OLD(0, 1, null, "Dokument je punovažan bez pečata i potpisa", System.Windows.Forms.RightToLeft.No);

      VvHamper.AddLabelLine(hamper);

   }

   #endregion OTS

   #region Pdv

   private void InitializeHamper_Pvd(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 23/*22*/, "", this, false);
                                      
      hamper.VvColWdt      = new int[] { ZXC.Q2un + ZXC.Qun8, ZXC.Q3un + ZXC.Qun4 + ZXC.Qun10, ZXC.Q3un + ZXC.Qun4 + ZXC.Qun10, ZXC.Q2un + ZXC.Qun8, ZXC.QUN  + ZXC.Qun10  };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun12,                      ZXC.Qun12,                      ZXC.Qun12,           ZXC.Qun12, ZXC.Qun12};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      int pomak1 = 0;
      int pomak2 = 0;
                            hamper.CreateVvLabel      (0, 0, "Knjiga IRA / URA:", 1, 0, ContentAlignment.MiddleRight);
                            hamper.CreateVvLabel      (0, 1, "Knjiga URA:"      , 1, 0, ContentAlignment.MiddleRight);
     
      rbt_pdvKnjigaR      = hamper.CreateVvRadioButton(2, 0, null, "Redovna" ,       TextImageRelation.ImageAboveText);
      rbt_pdvKnjigaP      = hamper.CreateVvRadioButton(3, 0, null, "Predujma", 1, 0, TextImageRelation.ImageAboveText);
      rbt_pdvKnjigUr      = hamper.CreateVvRadioButton(2, 1, null, "Uvz robe",       TextImageRelation.ImageAboveText);
      rbt_pdvKnjigUu      = hamper.CreateVvRadioButton(3, 1, null, "Uvz uslg", 1, 0, TextImageRelation.ImageAboveText);
      rbt_pdvKnjigNijedna = hamper.CreateVvRadioButton(2, 2, null, "Nijedna" ,       TextImageRelation.ImageAboveText);
      rbt_pdvKnjigaR.Checked = true;
      rbt_pdvKnjigaR.Tag = true;

                                           hamper.CreateVvLabel  (3, 2, "PdvGEO:", ContentAlignment.MiddleRight);
      tbx_PdvGEOkind                     = hamper.CreateVvTextBox(4, 2, "tbx_PdvGEOkind", "H - RH, E - EU, O - Treće zemlje, T - Tuzemni prijenos porezne obveze, B - Porezni obveznik bez sjedišta u RH ", 1);
      tbx_PdvGEOkind.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_PdvGEOkind.JAM_AllowedInputCharacters = "HEOTB";
      tbx_PdvGEOkind.TextAlign = HorizontalAlignment.Center;

                            hamper.CreateVvLabel       (0, 3, "Prikaži u 2 reda:", 1, 0, ContentAlignment.MiddleRight);
      cbx_isVisibleTT     = hamper.CreateVvCheckBox_OLD(2, 3, null, "Račun", System.Windows.Forms.RightToLeft.No);
      cbx_isVisibleAdress = hamper.CreateVvCheckBox_OLD(3, 3, null, 1, 0, "Naziv", System.Windows.Forms.RightToLeft.No);

                     hamper.CreateVvLabel  (0, 4, "Dopune PDV obrasca:", 1, 0, ContentAlignment.MiddleRight);
                     hamper.CreateVvLabel  (0, 5, "II.14. NakOslIzv:"  , 1, 0, ContentAlignment.MiddleRight);
      tbx_pdvF_Osn = hamper.CreateVvTextBox(2, 5, "tbx_pdvF_Osn", "Porezna osnovica: Naknadno oslobođenje izvoza u okviru osobnog putničkog prometa", 12);
      tbx_pdvF_Pdv = hamper.CreateVvTextBox(3, 5, "tbx_pdvF_Pdv", "Iznos poreza: Naknadno oslobođenje izvoza u okviru osobnog putničkog prometa", 12, 1, 0);

      tbx_pdvF_Osn.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_pdvF_Pdv.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      // dodano 09.06.2020. - do corone se nije nista desavalo sa redkom III.14, ukOsnObr_WO i ukPdvObr_WO su bili 0, pa neka sada svak stavi kaj mu treba

                     hamper.CreateVvLabel (0, 6, "III.14.Pretp pri uvozu:"  , 1, 0, ContentAlignment.MiddleRight);
      tbx_PretpPriUvz_Osn = hamper.CreateVvTextBox(2, 6, "tbx_314_Osn", "Porezna osnovica: Pretporez pri uvozu", 12);
      tbx_PretpPriUvz_Pdv = hamper.CreateVvTextBox(3, 6, "tbx_314_Pdv", "Iznos poreza: Pretporez pri uvozu", 12, 1, 0);

      tbx_PretpPriUvz_Osn.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_PretpPriUvz_Pdv.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);


      #region bef2015
      Label lbl0   = hamper.CreateVvLabel  (0, 6, "PdvObr", ContentAlignment.MiddleLeft);
      Label lbl1   = hamper.CreateVvLabel  (1, 6, "III.5", ContentAlignment.MiddleLeft);
      Label lbl2   = hamper.CreateVvLabel  (2, 6, "III.6", ContentAlignment.MiddleLeft);
      Label lbl3   = hamper.CreateVvLabel  (3, 6, "III.7", 1, 0, ContentAlignment.MiddleLeft);
      tbx_Fof_III5 = hamper.CreateVvTextBox(1, 7, "tbx_Fof_III5", "III.5", 12);
      tbx_Fof_III6 = hamper.CreateVvTextBox(2, 7, "tbx_Fof_III6", "III.6", 12);
      tbx_Fof_III7 = hamper.CreateVvTextBox(3, 7, "tbx_Fof_III7", "III.7", 12, 1, 0);

      tbx_Fof_III5.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_Fof_III6.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_Fof_III7.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      if(ZXC.projectYearFirstDay.Year > 2014)
      {
         lbl0.Visible = lbl1.Visible = lbl2.Visible = lbl3.Visible = tbx_Fof_III5.Visible = tbx_Fof_III6.Visible = tbx_Fof_III7.Visible = false;
         pomak1 = 2;
         pomak2 = 6;
      }
      #endregion bef2015


                        hamper.CreateVvLabel  (0, 9-pomak1, "III.15. Ipravci pretpor:", 1, 0, ContentAlignment.MiddleRight);
      tbx_pdvIspravak = hamper.CreateVvTextBox(2, 9-pomak1, "tbx_pdvIspravak", "Ispravci pretporeza", 12);
      tbx_pdvIspravak.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

                      hamper.CreateVvLabel  (0, 10-pomak1, "Porezni kredit:", 1, 0, ContentAlignment.MiddleRight);
      tbx_pdvKredit = hamper.CreateVvTextBox(2, 10-pomak1, "tbx_pdvKredir", "Pdv V.Po prethodnom obr: neuplaćeno, više uplaćeno - poreyni kredit //  Pdv-K V. Uplaćeno da odana podnošenja prijave", 12);
      tbx_pdvKredit.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      Label lbl8   = hamper.CreateVvLabel  (0, 9, "VII ", ContentAlignment.MiddleRight);
      tbx_redakVII = hamper.CreateVvTextBox(1, 9, "tbx_redakVII ", "redakVII ", 12);
      
      Label lbl9    = hamper.CreateVvLabel  (0, 10, "VIII.1.1.", ContentAlignment.MiddleRight);
      Label lbl10   = hamper.CreateVvLabel  (2, 10, "VIII.1.2.", ContentAlignment.MiddleRight);
      tbx_pdv_811   = hamper.CreateVvTextBox(1, 10, "tbx_pdv_811  ", "pdv_811  ", 12);
      tbx_pdv_812   = hamper.CreateVvTextBox(3, 10, "tbx_pdv_812  ", "pdv_812  ", 12, 1, 0);
      
      Label lbl11   = hamper.CreateVvLabel  (0, 11, "VIII.1.3.", ContentAlignment.MiddleRight);
      Label lbl12   = hamper.CreateVvLabel  (2, 11, "VIII.1.4.", ContentAlignment.MiddleRight);
      tbx_pdv_813   = hamper.CreateVvTextBox(1, 11, "tbx_pdv_813  ", "pdv_813  ", 12);
      tbx_pdv_814   = hamper.CreateVvTextBox(3, 11, "tbx_pdv_814  ", "pdv_814  ", 12, 1, 0);
      
      Label lbl13   = hamper.CreateVvLabel  (0, 12, "VIII.1.5.", ContentAlignment.MiddleRight);
      Label lbl14   = hamper.CreateVvLabel  (2, 12, "VIII.2.0.", ContentAlignment.MiddleRight);
      tbx_pdv_815   = hamper.CreateVvTextBox(1, 12, "tbx_pdv_815  ", "pdv_815  ", 12);
      tbx_pdv_820   = hamper.CreateVvTextBox(3, 12, "tbx_pdv_820  ", "pdv_820  ", 12, 1, 0);

      Label lbl15   = hamper.CreateVvLabel  (0, 13, "VIII.3.1.v", ContentAlignment.MiddleRight);
      Label lbl16   = hamper.CreateVvLabel  (2, 13, "VIII.3.1.b", ContentAlignment.MiddleRight);
      tbx_pdv_831vr = hamper.CreateVvTextBox(1, 13, "tbx_pdv_831vr", "pdv_831vr", 12);
      tbx_pdv_831br = hamper.CreateVvTextBox(3, 13, "tbx_pdv_831br", "pdv_831br", 12, 1 , 0);

      Label lbl17   = hamper.CreateVvLabel  (0, 14, "VIII.3.2.v", ContentAlignment.MiddleRight);
      Label lbl18   = hamper.CreateVvLabel  (2, 14, "VIII.3.2.v", ContentAlignment.MiddleRight);
      tbx_pdv_832vr = hamper.CreateVvTextBox(1, 14, "tbx_pdv_832vr", "pdv_832vr", 12);
      tbx_pdv_832br = hamper.CreateVvTextBox(3, 14, "tbx_pdv_832br", "pdv_832br", 12, 1, 0);

      Label lbl19   = hamper.CreateVvLabel  (0, 15, "VIII.3.3.v", ContentAlignment.MiddleRight);
      Label lbl20   = hamper.CreateVvLabel  (2, 15, "VIII.3.3.v", ContentAlignment.MiddleRight);
      tbx_pdv_833vr = hamper.CreateVvTextBox(1, 15, "tbx_pdv_833vr", "pdv_833vr", 12);
      tbx_pdv_833br = hamper.CreateVvTextBox(3, 15, "tbx_pdv_833br", "pdv_833br", 12, 1, 0);

      Label lbl21   = hamper.CreateVvLabel  (0, 16, "VIII.6.  ", ContentAlignment.MiddleRight);
      tbx_pdv_860   = hamper.CreateVvTextBox(1, 16, "tbx_pdv_860  ", "pdv_860  ", 12);

      Label lbl22   = hamper.CreateVvLabel  (2, 16, "VIII.4.  ", ContentAlignment.MiddleRight);
      tbx_pdv_840   = hamper.CreateVvTextBox(3, 16, "tbx_pdv_840  ", "pdv_840  ", 12, 1, 0);

      tbx_redakVII.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_pdv_811   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_pdv_812   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_pdv_813   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_pdv_814   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_pdv_815   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_pdv_820   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_pdv_831vr .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true); 
      tbx_pdv_832vr .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true); 
      tbx_pdv_833vr .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true); 
      tbx_pdv_860   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_pdv_840   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      tbx_pdv_831br.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_pdv_832br.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_pdv_833br.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      if(ZXC.projectYearFirstDay.Year < 2015)
      {
         lbl8.Visible  = lbl9.Visible  = lbl10.Visible = lbl11.Visible = lbl12.Visible = lbl13.Visible = lbl14.Visible = lbl15.Visible =
         lbl16.Visible = lbl17.Visible = lbl18.Visible = lbl19.Visible = lbl20.Visible = lbl21.Visible = lbl22.Visible =
         tbx_pdv_831br.Visible = 
         tbx_pdv_832br.Visible = 
         tbx_pdv_833br.Visible = 
         tbx_redakVII .Visible = 
         tbx_pdv_811  .Visible = 
         tbx_pdv_812  .Visible = 
         tbx_pdv_813  .Visible = 
         tbx_pdv_814  .Visible = 
         tbx_pdv_815  .Visible = 
         tbx_pdv_820  .Visible = 
         tbx_pdv_831vr.Visible = 
         tbx_pdv_832vr.Visible = 
         tbx_pdv_833vr.Visible = 
         tbx_pdv_860  .Visible = 
         tbx_pdv_840  .Visible = false;
      }


      hamper.CreateVvLabel(0, 11+pomak2, "PdvObr:" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(1, 11+pomak2, "Povrat"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(2, 11+pomak2, "Predujam", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(3, 11+pomak2, "Ustup"   , 1, 0, ContentAlignment.MiddleLeft);

      tbx_pdvPovrat   = hamper.CreateVvTextBox(1, 12+pomak2, "tbx_pdvPovrat"  , "Povrat"       , 12);
      tbx_pdvPredujam = hamper.CreateVvTextBox(2, 12+pomak2, "tbx_pdvPredujam", "Predujam"     , 12);
      tbx_pdvUstup    = hamper.CreateVvTextBox(3, 12+pomak2, "tbx_pdvUstup"   , "Ustup povrata", 12, 1, 0);

      tbx_pdvPovrat  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_pdvPredujam.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_pdvUstup   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

                            hamper.CreateVvLabel       (0, 13+pomak2, "Sastav.:", 0, 0, ContentAlignment.MiddleLeft);
      cbx_isCurrUser      = hamper.CreateVvCheckBox_OLD(1, 13+pomak2, null, "User", System.Windows.Forms.RightToLeft.No);
      tbx_obrazacSastavio = hamper.CreateVvTextBox     (2, 13+pomak2, "tbx_obrazacSastavio", "Obračun sastavio ", 32, 2, 0);

                        hamper.CreateVvLabel       (0, 14+pomak2,             "IzPrj:", ContentAlignment.MiddleLeft);
      cbx_isPrjktTel  = hamper.CreateVvCheckBox_OLD(1, 14+pomak2, null,       "Tel"   , System.Windows.Forms.RightToLeft.No);
      cbx_isPrjktFax  = hamper.CreateVvCheckBox_OLD(2, 14+pomak2, null,       "Fax"   , System.Windows.Forms.RightToLeft.No);
      cbx_IsPrjktMail = hamper.CreateVvCheckBox_OLD(3, 14+pomak2, null, 1, 0, "Mail"  , System.Windows.Forms.RightToLeft.No);

                 hamper.CreateVvLabel  (0, 15+pomak2, "Kontakt:", ContentAlignment.MiddleLeft);
      tbx_tel  = hamper.CreateVvTextBox(1, 15+pomak2, "tbx_tel" , "Kontak telefon");
      tbx_fax  = hamper.CreateVvTextBox(2, 15+pomak2, "tbx_fax" , "Kontakt fax");
      tbx_mail = hamper.CreateVvTextBox(3, 15+pomak2, "tbx_mail", "Kontakt mail", 42, 1, 0);

      //VvHamper.HamperStyling(hamper);
      VvHamper.AddLabelLine(hamper);

   }

   #endregion Pdv

   #endregion hampers

   #region Fld_

   public DateTime Fld_DatumOd
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumOD.Value);
      }
      set
      {
         dtp_DatumOD.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DatumOD.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumOD);
      }
   }

   public DateTime Fld_DatumDo
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumDO.Value).EndOfDay();
      }
      set
      {
         dtp_DatumDO.Value = ZXC.ValOr_01011753_DateTime(value).EndOfDay();
         tbx_DatumDO.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumDO);
      }
   }

   public string Fld_TT
   {
      get { return tbxLookUp_TT.Text; }
      set { tbxLookUp_TT.Text = value; }
   }

   public string Fld_MacroName
   {
      //get { return combbx_macro.SelectedItem.ToString(); }
      get
      {
         object selectedItem = (VvRiskMacro)VvHamper.Get_ComboBoxSelectedItem_CallBack_ThreadSafe(combbx_macro);

         if(selectedItem == null) return "";
         else return selectedItem.ToString();
      }
      set { combbx_macro.SelectedItem = ZXC.VvRiskMacroList.SingleOrDefault(macro => macro.MacroName == value); }
      //get { return ""; }
   }

   public string Fld_ArtiklCdOd
   {
      get { return tbx_artiklCdOD.Text; }
      set { tbx_artiklCdOD.Text = value; }
   }

   public string Fld_ArtiklCdDo
   {
      get { return tbx_artiklCdDO.Text; }
      set { tbx_artiklCdDO.Text = value; }
   }

   public string Fld_ArtiklNazivOd
   {
      get { return tbx_artiklNazivOD.Text; }
      set { tbx_artiklNazivOD.Text = value; }
   }

   public string Fld_ArtiklNazivDo
   {
      get { return tbx_artiklNazivDO.Text; }
      set { tbx_artiklNazivDO.Text = value; }
   }

   public uint Fld_TtNumOd
   {
      get { return ZXC.ValOrZero_UInt(tbx_fakturOD.Text); }
      set { tbx_fakturOD.Text = value.ToString(); }
   }

   public uint Fld_TtNumDo
   {
      get { return ZXC.ValOrZero_UInt(tbx_fakturDO.Text); }
      set { tbx_fakturDO.Text = value.ToString(); }
   }

   public VvSQL.SorterType Fld_DokumSort
   {
      get
      {
         if(rbSort_Datum.Checked) return VvSQL.SorterType.DokDate;
         else if(rbSort_Broj.Checked) return VvSQL.SorterType.TtNum;
         else if(rbSort_Partner.Checked) return VvSQL.SorterType.KpdbName;
         else return VvSQL.SorterType.None;
      }
      set
      {
         switch(value)
         {
            case VvSQL.SorterType.DokDate: rbSort_Datum.Checked = true; break;
            case VvSQL.SorterType.TtNum: rbSort_Broj.Checked = true; break;
            case VvSQL.SorterType.KpdbName: rbSort_Partner.Checked = true; break;
         }
      }
   }

   public VvSQL.SorterType Fld_ArtiklSort
   {
      get
      {
              if(rbArtSort_ByName   .Checked) return VvSQL.SorterType.Name       ;
         else if(rbArtSort_BySifra  .Checked) return VvSQL.SorterType.Code       ;
         else if(rbArtSort_topByKol .Checked) return VvSQL.SorterType.ArtTopByKol;
         else if(rbArtSort_topByFin .Checked) return VvSQL.SorterType.ArtTopByFin;
         else if(rbArtSort_topByRuc .Checked) return VvSQL.SorterType.ArtTopByRuc;

         else if(rbArtSort_ByBarCode.Checked)
            if(ZXC.IsSvDUH) return VvSQL.SorterType.s_lio  ;
            else            return VvSQL.SorterType.BarCode;

         else return VvSQL.SorterType.None;
      }
      set
      {
         switch(value)
         {
            case VvSQL.SorterType.Name       : rbArtSort_ByName   .Checked = true; break;
            case VvSQL.SorterType.Code       : rbArtSort_BySifra  .Checked = true; break;
            case VvSQL.SorterType.ArtTopByKol: rbArtSort_topByKol .Checked = true; break;
            case VvSQL.SorterType.ArtTopByFin: rbArtSort_topByFin .Checked = true; break;
            case VvSQL.SorterType.ArtTopByRuc: rbArtSort_topByRuc .Checked = true; break;

            case VvSQL.SorterType.s_lio      : 
            case VvSQL.SorterType.BarCode    : rbArtSort_ByBarCode.Checked = true; break;
         }
      }
   }

   public bool Fld_NeedsHorizontalLine
   {
      get { return cb_Line.Checked; }
      set {        cb_Line.Checked = value; }
   }

   public bool Fld_ZaExportToExcel
   {
      get { return cb_zaExport.Checked; }
      set {        cb_zaExport.Checked = value; }
   }

   public string Fld_GrupiranjeDok
   {
      get
      {
              if(rbt_grupNull     .Checked) return "";
         else if(rbt_grupUser     .Checked) return "AddUID";
         else if(rbt_grupPartner  .Checked) return "KupdobName";
         else if(rbt_grupMjTros   .Checked) return "MtrosName";
       //else if(rbt_grupValuta   .Checked) return "DevName";
         else if(rbt_grupPutnik   .Checked) return "Putnik";
         else if(rbt_grupProjekt  .Checked) return "ProjektCD";
         else if(rbt_grupNacPl    .Checked) return "NacPlac";
         else if(rbt_grupTT       .Checked) return "TT";
         else if(rbt_grupMonth    .Checked) return "DokMonth";
         else if(rbt_grupWeek     .Checked) return "DokWeek";
         else if(rbt_grupDay      .Checked) return "DokDay";
         else if(rbt_grupHour     .Checked) return "AddTS";
         else if(rbt_grupDayOfWeek.Checked) return "DayOfWeek";
         else if(rbt_grupHourOfDay.Checked) return "HourOfDay";
         else if(rbt_grupSkladCD  .Checked) return "SkladCD";
         else if(rbt_grupPosJed   .Checked) return "PosJedName";
         else if(rbt_grupTH_CycleM.Checked) return "TH_CycleM";
         else if(rbt_grupGodina   .Checked) return "DokYear";
         else return "";
      }
      set
      {
              if(value == "AddUID"    ) rbt_grupUser     .Checked = true;
         else if(value == "KupdobName") rbt_grupPartner  .Checked = true;
         else if(value == "MtrosName" ) rbt_grupMjTros   .Checked = true;
       //else if(value == "DevName"   ) rbt_grupValuta   .Checked = true;
         else if(value == "Putnik"    ) rbt_grupPutnik   .Checked = true;
         else if(value == "ProjektCD" ) rbt_grupProjekt  .Checked = true;
         else if(value == "NacPlac"   ) rbt_grupNacPl    .Checked = true;
         else if(value == "TT"        ) rbt_grupTT       .Checked = true;
         else if(value == "DokMonth"  ) rbt_grupMonth    .Checked = true;
         else if(value == "DokWeek"   ) rbt_grupWeek     .Checked = true;
         else if(value == "DokDay"    ) rbt_grupDay      .Checked = true;
         else if(value == "AddTS"     ) rbt_grupHour     .Checked = true;
         else if(value == "DayOfWeek" ) rbt_grupDayOfWeek.Checked = true;
         else if(value == "HourOfDay" ) rbt_grupHourOfDay.Checked = true;
         else if(value == "SkladCD"   ) rbt_grupSkladCD  .Checked = true;
         else if(value == "PosJedName") rbt_grupPosJed   .Checked = true;
         else if(value == "TH_CycleM" ) rbt_grupTH_CycleM.Checked = true;
         else if(value == "DokYear"   ) rbt_grupGodina   .Checked = true;
         else rbt_grupNull.Checked = true;
      }
   }
   public string Fld_GrupiranjeArtikla
   {
      get
      {
              if(rbt_grupArtNull.Checked) return "";
         else if(rbt_grupArtTs  .Checked) return "TS";
         else if(rbt_grupArtGr1 .Checked) return "Grupa1CD";
         else if(rbt_grupArtGr2 .Checked) return "Grupa2CD";
         else if(rbt_grupArtGr3 .Checked) return "Grupa3CD";
         else return "";
      }
      set
      {
              if(value == "TS"      ) rbt_grupArtTs.Checked = true;
         else if(value == "Grupa1CD") rbt_grupArtGr1.Checked = true;
         else if(value == "Grupa2CD") rbt_grupArtGr2.Checked = true;
         else if(value == "Grupa3CD") rbt_grupArtGr3.Checked = true;
         else rbt_grupArtNull.Checked = true;
      }
   }

 //public ZXC.JeliJeTakav Fld_JeliJe_HALMED_Upareni // SVI, samo NEUPARENI, samo UPARENI 
 //{
 //   get
 //   {
 //           if(rbt_grupArtNull.Checked) return ZXC.JeliJeTakav.NEBITNO   ;
 //      else if(rbt_grupArtGr1 .Checked) return ZXC.JeliJeTakav.NIJE_TAKAV;
 //      else if(rbt_grupArtGr2 .Checked) return ZXC.JeliJeTakav.JE_TAKAV  ;
 //
 //      else return ZXC.JeliJeTakav.NEBITNO;
 //   }
 //}

   public ZXC.UparenostKind Fld_JeliJe_HALMED_Uparen // SVI, samo NEUPARENI, samo UPARENI 
   {
      get
      {
              if(rbt_grupArtNull.Checked) return ZXC.UparenostKind.NEBITNO  ;
         else if(rbt_grupArtGr1 .Checked) return ZXC.UparenostKind.NE_uparen;
         else if(rbt_grupArtGr2 .Checked) return ZXC.UparenostKind.DA_uparen;

       //else if(rbt_grupArtGr3 .Checked) return ZXC.UparenostKind.Lose_uparen;

         else return ZXC.UparenostKind.NEBITNO;
      }
   }

   //public string Fld_AnalitSintet
   //{
   //   get
   //   {
   //           if(rbt_analitika.Checked) return "A";
   //      else if(rbt_sintetika.Checked) return "S";
   //      else return "";
   //   }
   //   set
   //   {
   //      if(value == "S") rbt_sintetika.Checked = true;
   //      else             rbt_analitika.Checked = true;
   //   }
   //}

   public bool Fld_AnaGrupaPoStranici
   {
      get { return cbx_grupaPoStr.Checked; }
      set { cbx_grupaPoStr.Checked = value; }
   }

   public bool Fld_VisiblePostoGrupFooter
   {
      get { return cbx_visiblePosto.Checked; }
      set { cbx_visiblePosto.Checked = value; }
   }

   public bool Fld_VisibleOnlyTopGroups
   {
      get { return cbx_topGroups.Checked; }
      set { cbx_topGroups.Checked = value; }
   }

   public int Fld_NumOfTopGroups  { get { return tbx_NumOfGrups.GetIntField(); } set { tbx_NumOfGrups.PutIntField(value); } }

   public bool Fld_TopSort
   {
      get { return cbx_topSort.Checked; }
      set { cbx_topSort.Checked = value; }
   }

   public string Fld_SkladCD
   {
      get
      {
         ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD = tbx_skladCd.Text;
         return tbx_skladCd.Text;
      }
      set
      {
         //ZXC.TheVvForm.theLastUsedSkladCD = value;
         tbx_skladCd.Text = value;
      }
   }
   public string Fld_SkladCD2 {  get{  return tbx_skladCd2.Text; } set { tbx_skladCd2.Text = value; } }

   public ZXC.PdvKnjigaEnum Fld_PdvKnjiga
   {
      get
      {
              if(rbt_pdvKnjigaR     .Checked) return ZXC.PdvKnjigaEnum.REDOVNA ;
         else if(rbt_pdvKnjigaP     .Checked) return ZXC.PdvKnjigaEnum.PREDUJAM;
         else if(rbt_pdvKnjigUr     .Checked) return ZXC.PdvKnjigaEnum.UVOZ_ROB;
         else if(rbt_pdvKnjigUu     .Checked) return ZXC.PdvKnjigaEnum.UVOZ_USL;
         else if(rbt_pdvKnjigNijedna.Checked) return ZXC.PdvKnjigaEnum.NIJEDNA ;

         else throw new Exception("Fld_PdvKnjiga: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.PdvKnjigaEnum.REDOVNA : rbt_pdvKnjigaR     .Checked = true; break;
            case ZXC.PdvKnjigaEnum.PREDUJAM: rbt_pdvKnjigaP     .Checked = true; break;
            case ZXC.PdvKnjigaEnum.UVOZ_ROB: rbt_pdvKnjigUr     .Checked = true; break;
            case ZXC.PdvKnjigaEnum.UVOZ_USL: rbt_pdvKnjigUu     .Checked = true; break;
            case ZXC.PdvKnjigaEnum.NIJEDNA : rbt_pdvKnjigNijedna.Checked = true; break;
            
            default: rbt_pdvKnjigaR.Checked = true; break;

         }
      }
   }

  public ZXC.PdvGEOkindEnum Fld_PdvGEOkind
   {
      get
      {
         switch(tbx_PdvGEOkind.Text)
         {
            case "H": return ZXC.PdvGEOkindEnum.HR   ;
            case "E": return ZXC.PdvGEOkindEnum.EU   ;
            case "O": return ZXC.PdvGEOkindEnum.WORLD;
            case "T": return ZXC.PdvGEOkindEnum.TP   ;
            case "B": return ZXC.PdvGEOkindEnum.BS   ;

            default: return ZXC.PdvGEOkindEnum.HR;
         }
      }
      set
      {
         switch(value)
         {
            case ZXC.PdvGEOkindEnum.HR   : tbx_PdvGEOkind.Text = "H"; break;
            case ZXC.PdvGEOkindEnum.EU   : tbx_PdvGEOkind.Text = "E"; break;
            case ZXC.PdvGEOkindEnum.WORLD: tbx_PdvGEOkind.Text = "O"; break;
            case ZXC.PdvGEOkindEnum.TP   : tbx_PdvGEOkind.Text = "T"; break;
            case ZXC.PdvGEOkindEnum.BS   : tbx_PdvGEOkind.Text = "B"; break;
            default:                       tbx_PdvGEOkind.Text = ""; break;
         }
      }
   }


   public decimal Fld_PdvKredit
   {
      get { return tbx_pdvKredit.GetDecimalField(); }
      set { tbx_pdvKredit.PutDecimalField(value); }
   }
   public decimal Fld_PdvPovrat
   {
      get { return tbx_pdvPovrat.GetDecimalField(); }
      set { tbx_pdvPovrat.PutDecimalField(value); }
   }
   public decimal Fld_PdvPredujam
   {
      get { return tbx_pdvPredujam.GetDecimalField(); }
      set { tbx_pdvPredujam.PutDecimalField(value); }
   }
   public decimal Fld_PdvUstup
   {
      get { return tbx_pdvUstup.GetDecimalField(); }
      set { tbx_pdvUstup.PutDecimalField(value); }
   }

   public decimal Fld_PdvF_Osn
   {
      get { return tbx_pdvF_Osn.GetDecimalField(); }
      set { tbx_pdvF_Osn.PutDecimalField(value); }
   }
   public decimal Fld_PdvF_Pdv
   {
      get { return tbx_pdvF_Pdv.GetDecimalField(); }
      set { tbx_pdvF_Pdv.PutDecimalField(value); }
   }

   public decimal Fld_PretpPriUvz_Osn
   {
      get { return tbx_PretpPriUvz_Osn.GetDecimalField(); }
      set {        tbx_PretpPriUvz_Osn.PutDecimalField(value); }
   }
   public decimal Fld_PretpPriUvz_Pdv
   {
      get { return tbx_PretpPriUvz_Pdv.GetDecimalField(); }
      set {        tbx_PretpPriUvz_Pdv.PutDecimalField(value); }
   }


   public decimal Fld_PdvIspravak
   {
      get { return tbx_pdvIspravak.GetDecimalField(); }
      set { tbx_pdvIspravak.PutDecimalField(value); }
   }

   public bool Fld_IsPrjktTel
   {
      get { return cbx_isPrjktTel.Checked; }
      set { cbx_isPrjktTel.Checked = value; }
   }

   public bool Fld_IsPrjktFax
   {
      get { return cbx_isPrjktFax.Checked; }
      set { cbx_isPrjktFax.Checked = value; }
   }

   public bool Fld_IsPrjktMail
   {
      get { return cbx_IsPrjktMail.Checked; }
      set { cbx_IsPrjktMail.Checked = value; }
   }


   public string Fld_Tel
   {
      get { return tbx_tel.Text; }
      set { tbx_tel.Text = value; }
   }

   public string Fld_Fax
   {
      get { return tbx_fax.Text; }
      set { tbx_fax.Text = value; }
   }

   public string Fld_Mail
   {
      get { return tbx_mail.Text; }
      set { tbx_mail.Text = value; }
   }

   public string Fld_NacinPlacanja
   {
      get { return tbx_NacPlac.Text; }
      set { tbx_NacPlac.Text = value; }
   }

   public string Fld_PdvObrSastavio
   {
      get { return tbx_obrazacSastavio.Text; }
      set { tbx_obrazacSastavio.Text = value; }
   }

   //public decimal Fld_IznosOd
   //{
   //   get { return ZXC.ValOrZero_Decimal(tbx_iznosOD.Text, 2); }
   //   set {                              tbx_iznosOD.Text = value.ToString("N", ZXC.GetNumberFormatInfo(2)); }
   //}

   //public decimal Fld_IznosDo
   //{
   //   get { return ZXC.ValOrZero_Decimal(tbx_iznosDO.Text, 2); }
   //   set {                              tbx_iznosDO.Text = value.ToString("N", ZXC.GetNumberFormatInfo(2)); }
   //}

   public string Fld_KD_ticker
   {
      get { return tbx_KD_ticker.Text; }
      set { tbx_KD_ticker.Text = value; }
   }

   public uint Fld_KD_sifra
   {
      get { return ZXC.ValOrZero_UInt(tbx_KD_sifra.Text); }
      set { tbx_KD_sifra.Text = value.ToString("000000"); }

   }

   public string Fld_KD_SifraAsTxt
   {
      get { return tbx_KD_sifra.Text; }
      set { tbx_KD_sifra.Text = value; }
   }

   public string Fld_KD_naziv
   {
      get { return tbx_KD_naziv.Text; }
      set { tbx_KD_naziv.Text = value; }
   }

   //public string Fld_Grupa1CD
   //{
   //   get { return tbx_Grupa1CD.Text; }
   //   set {        tbx_Grupa1CD.Text = value; }
   //}

   public string Fld_MT_ticker
   {
      get { return tbx_MT_ticker.Text; }
      set { tbx_MT_ticker.Text = value; }
   }

   public uint Fld_MT_sifra
   {
      get { return ZXC.ValOrZero_UInt(tbx_MT_sifra.Text); }
      set { tbx_MT_sifra.Text = value.ToString("000000"); }

   }

   public string Fld_MT_SifraAsTxt
   {
      get { return tbx_MT_sifra.Text; }
      set { tbx_MT_sifra.Text = value; }
   }

   public string Fld_MT_naziv
   {
      get { return tbx_MT_naziv.Text; }
      set { tbx_MT_naziv.Text = value; }
   }

   public bool Fld_IsVisibleTT { get { return cbx_isVisibleTT.Checked; } set { cbx_isVisibleTT.Checked = value; } }
   public bool Fld_IsVisibleAdress
   {
      get { return cbx_isVisibleAdress.Checked; }
      set { cbx_isVisibleAdress.Checked = value; }
   }
   public bool Fld_IsUserSastavio
   {
      get { return cbx_isCurrUser.Checked; }
      set { cbx_isCurrUser.Checked = value; }
   }

   public bool Fld_IsOtsAnalitKontre    { get { return cbx_otsAnalitika       .Checked; } set { cbx_otsAnalitika       .Checked = value; } }
   public bool Fld_IsOtsDospjecaPoDan   { get { return cbx_otsDospjeca        .Checked; } set { cbx_otsDospjeca        .Checked = value; } }
   public bool Fld_IsOtsKontakt         { get { return cbx_otsKontakt         .Checked; } set { cbx_otsKontakt         .Checked = value; } }
   public bool Fld_IsOtsLineTipBr       { get { return cbx_otsLineTipBr       .Checked; } set { cbx_otsLineTipBr       .Checked = value; } }
   public bool Fld_IsOtsDospjelo        { get { return cbx_otsDospjelo        .Checked; } set { cbx_otsDospjelo        .Checked = value; } }
   public bool Fld_IsOtsNevezano        { get { return cbx_isOtsNevezano      .Checked; } set { cbx_isOtsNevezano      .Checked = value; } }
   public bool Fld_IsForceOtsByDokDate  { get { return cbx_isForceOtsByDokDate.Checked; } set { cbx_isForceOtsByDokDate.Checked = value; } }
  
   public string Fld_KompenzacijaBroj { get { return tbx_kompBr.Text; } set { tbx_kompBr.Text = value; } }

   public DateTime Fld_OtsDate
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumOTS.Value);
      }
      set
      {
         dtp_DatumOTS.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DatumOTS.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumOTS);
      }
   }

   public string Fld_ProjektCD
   {
      get { return tbx_Projekt.Text; }
      set { tbx_Projekt.Text = value; }
   }

   public ZXC.PdvR12Enum Fld_PdvR12
   {
      get
      {
              if(rbt_pdvR1.Checked)   return ZXC.PdvR12Enum.R1;
         else if(rbt_pdvR2.Checked)   return ZXC.PdvR12Enum.R2;
         else if(rbt_pdvSviR.Checked) return ZXC.PdvR12Enum.NIJEDNO;

         else throw new Exception("Fld_PdvR12: who df is checked?");

      }
      set
      {
         switch(value)
         {
            case ZXC.PdvR12Enum.R1: rbt_pdvR1.Checked = true; break;
            case ZXC.PdvR12Enum.R2: rbt_pdvR2.Checked = true; break;
            case ZXC.PdvR12Enum.NIJEDNO: rbt_pdvSviR.Checked = true; break;
            default: rbt_pdvSviR.Checked = true; break;

         }
      }
   }

   public bool   Fld_IsPrihodTT { get { return cbx_isPrihodTT.Checked; } set { cbx_isPrihodTT.Checked = value; } }
   public bool   Fld_OcuGraf    { get { return cbx_ocuGraf.Checked; } set { cbx_ocuGraf.Checked = value; } }
   public string Fld_Status     { get { return tbx_Status.Text; } set { tbx_Status.Text = value; } }


   public DateTime Fld_CompDateOd
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_compDateOd.Value);
      }
      set
      {
         dtp_compDateOd.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_compDateOd.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_compDateOd);
      }
   }

   public DateTime Fld_CompDateDo
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_compDateDo.Value).EndOfDay();
      }
      set
      {
         dtp_compDateDo.Value = ZXC.ValOr_01011753_DateTime(value).EndOfDay();
         tbx_compDateDo.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_compDateDo);
      }
   }


   public decimal Fld_Pdv_Fof_III5 { get { return tbx_Fof_III5.GetDecimalField(); } set { tbx_Fof_III5.PutDecimalField(value); } }
   public decimal Fld_Pdv_Fof_III6 { get { return tbx_Fof_III6.GetDecimalField(); } set { tbx_Fof_III6.PutDecimalField(value); } }
   public decimal Fld_Pdv_Fof_III7 { get { return tbx_Fof_III7.GetDecimalField(); } set { tbx_Fof_III7.PutDecimalField(value); } }

   public bool Fld_IsMpskPoNBC    { get { return cbx_isMpskPoNBC  .Checked; } set { cbx_isMpskPoNBC  .Checked = value; } }
   public bool Fld_IsChkO         { get { return cbx_isChkO       .Checked; } set { cbx_isChkO       .Checked = value; } }
   public bool Fld_IsNoColorPrint { get { return cbx_isNoColor    .Checked; } set { cbx_isNoColor    .Checked = value; } }
   public bool Fld_IsPoslJed      { get { return cbx_isPoslJed    .Checked; } set { cbx_isPoslJed    .Checked = value; } }
   public bool Fld_IsPecatotpis   { get { return cbx_isPecatPotpis.Checked; } set { cbx_isPecatPotpis.Checked = value; } }
   public bool Fld_IsBlgInIzvVal  { get { return cbx_IsBlgInIzvVal.Checked; } set { cbx_IsBlgInIzvVal.Checked = value; } }

   public bool Fld_OnlyArtiklSaStanjem { get { return cbx_onlyArtiklSaStanjem.Checked; } set { cbx_onlyArtiklSaStanjem.Checked = value; } }

   public uint   Fld_Putnik_PersonCD      { get { return tbx_personCD.GetSomeRecIDField(); } set { tbx_personCD.PutSomeRecIDField(value); } }
   public string Fld_PersonCdAsTxt        { get { return tbx_personCD.Text;                } set { tbx_personCD.Text = value;             } }
   public string Fld_Putnik_PersonName    { get { return tbx_personName.Text;              } set { tbx_personName.Text = value;           } }

   public string Fld_KupDobTip            { get { return tbx_TipKupDob.Text; } set { tbx_TipKupDob.Text = value; } }

   public string Fld_RptNapomena { get { return tbx_rptNapomena.Text; } set { tbx_rptNapomena.Text = value; } }
   public string Fld_RptNaziv    { get { return tbx_rptNaziv   .Text; } set { tbx_rptNaziv   .Text = value; } }

   public bool Fld_IsRashodTT { get { return cbx_isRashodTT.Checked; } set { cbx_isRashodTT.Checked = value; } }

   public int     Fld_Pdv_831br { get { return tbx_pdv_831br.GetIntField();     } set { tbx_pdv_831br.PutIntField    (value); } }
   public int     Fld_Pdv_832br { get { return tbx_pdv_832br.GetIntField();     } set { tbx_pdv_832br.PutIntField    (value); } }
   public int     Fld_Pdv_833br { get { return tbx_pdv_833br.GetIntField();     } set { tbx_pdv_833br.PutIntField    (value); } }
   public decimal Fld_Pdv_RedVII{ get { return tbx_redakVII .GetDecimalField(); } set { tbx_redakVII .PutDecimalField(value); } }
   public decimal Fld_Pdv_811   { get { return tbx_pdv_811  .GetDecimalField(); } set { tbx_pdv_811  .PutDecimalField(value); } }
   public decimal Fld_Pdv_812   { get { return tbx_pdv_812  .GetDecimalField(); } set { tbx_pdv_812  .PutDecimalField(value); } }
   public decimal Fld_Pdv_813   { get { return tbx_pdv_813  .GetDecimalField(); } set { tbx_pdv_813  .PutDecimalField(value); } }
   public decimal Fld_Pdv_814   { get { return tbx_pdv_814  .GetDecimalField(); } set { tbx_pdv_814  .PutDecimalField(value); } }
   public decimal Fld_Pdv_815   { get { return tbx_pdv_815  .GetDecimalField(); } set { tbx_pdv_815  .PutDecimalField(value); } }
   public decimal Fld_Pdv_820   { get { return tbx_pdv_820  .GetDecimalField(); } set { tbx_pdv_820  .PutDecimalField(value); } }
   public decimal Fld_Pdv_831vr { get { return tbx_pdv_831vr.GetDecimalField(); } set { tbx_pdv_831vr.PutDecimalField(value); } }
   public decimal Fld_Pdv_832vr { get { return tbx_pdv_832vr.GetDecimalField(); } set { tbx_pdv_832vr.PutDecimalField(value); } }
   public decimal Fld_Pdv_833vr { get { return tbx_pdv_833vr.GetDecimalField(); } set { tbx_pdv_833vr.PutDecimalField(value); } }
   public decimal Fld_Pdv_860   { get { return tbx_pdv_860  .GetDecimalField(); } set { tbx_pdv_860  .PutDecimalField(value); } }
   public decimal Fld_Pdv_840   { get { return tbx_pdv_840  .GetDecimalField(); } set { tbx_pdv_840  .PutDecimalField(value); } }

   public ZXC.KomisijaKindEnum Fld_SkladFilter
   {
      get
      {
              if(rbt_svaSklad    .Checked) return ZXC.KomisijaKindEnum.NIJE        ;
         else if(rbt_onlyVelepSkl.Checked) return ZXC.KomisijaKindEnum.VELEPRODAJNA;
         else if(rbt_onlyMalopSkl.Checked) return ZXC.KomisijaKindEnum.MALOPRODAJNA;
         else if(rbt_TMB_VpskVps2.Checked) return ZXC.KomisijaKindEnum.TMB2SKL     ;

         else throw new Exception("Fld_SkladFilter: who df is checked?");

      }
      set
      {
         switch(value)
         {
            case ZXC.KomisijaKindEnum.NIJE        : rbt_svaSklad    .Checked= true; break;
            case ZXC.KomisijaKindEnum.VELEPRODAJNA: rbt_onlyVelepSkl.Checked= true; break;
            case ZXC.KomisijaKindEnum.MALOPRODAJNA: rbt_onlyMalopSkl.Checked= true; break;
            case ZXC.KomisijaKindEnum.TMB2SKL     : rbt_TMB_VpskVps2.Checked= true; break;
            default:                                rbt_svaSklad    .Checked = true; break;

         }
      }
   }

   public bool Fld_IsOpzStatRnVezDok { get { return cbx_isOpzStatVezDok.Checked; } set { cbx_isOpzStatVezDok.Checked = value; } }

   public string Fld_Artikl_TS  { get { return tbx_Artikl_TS .Text; } set { tbx_Artikl_TS .Text = value; } }
   public string Fld_Artikl_Gr1 { get { return tbx_Artikl_Gr1.Text; } set { tbx_Artikl_Gr1.Text = value; } }
   public string Fld_Artikl_Gr2 { get { return tbx_Artikl_Gr2.Text; } set { tbx_Artikl_Gr2.Text = value; } }
   public string Fld_Artikl_Gr3 { get { return tbx_Artikl_Gr3.Text; } set { tbx_Artikl_Gr3.Text = value; } }

   public string Fld_FRUF_Name      { get { return tbx_FRUF_Name .Text;        } set { tbx_FRUF_Name     .Text    = value; } }
   public string Fld_FRUF_Value     { get { return tbx_FRUF_Value.Text;        } set { tbx_FRUF_Value    .Text    = value; } }
   public bool   Fld_FRUF_BiloGdjeU { get { return cbx_FRUF_BiloGdjeU.Checked; } set { cbx_FRUF_BiloGdjeU.Checked = value; } }

   public string Fld_SVD_KnjBr_OD { get { return tbx_vezDok2OD.Text; } set { tbx_vezDok2OD.Text = value; } }
   public string Fld_SVD_KnjBr_DO { get { return tbx_vezDok2DO.Text; } set { tbx_vezDok2DO.Text = value; } }

   public ZXC.PdvZPkindEnum Fld_SVD_LiP
   {
      get
      {
              if(rbt_Lijek .Checked) return ZXC.PdvZPkindEnum.SVD_LJEK;
         else if(rbt_Potros.Checked) return ZXC.PdvZPkindEnum.SVD_POTR;
         else if(rbt_LiP   .Checked) return ZXC.PdvZPkindEnum.NoZP    ;

         else throw new Exception("Fld_Svd_LP: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.PdvZPkindEnum.SVD_LJEK: rbt_Lijek .Checked = true; break;
            case ZXC.PdvZPkindEnum.SVD_POTR: rbt_Potros.Checked = true; break;
            case ZXC.PdvZPkindEnum.NoZP    : rbt_LiP   .Checked = true; break;
            default                        : rbt_LiP   .Checked = true; break;
         }
      }
   }

   public ZXC.JeliJeTakav Fld_SVD_IsDonacSklad // SVI, samo DON, not DON 
   {
      get
      {
              if(rbt_svdSvaSkl .Checked) return ZXC.JeliJeTakav.NEBITNO   ;
         else if(rbt_svdDonac  .Checked) return ZXC.JeliJeTakav.JE_TAKAV  ;
         else if(rbt_svdNeDonac.Checked) return ZXC.JeliJeTakav.NIJE_TAKAV;
   
         else return ZXC.JeliJeTakav.NEBITNO;
      }
     //set
     // {
     //    switch(value)
     //    {
     //       case ZXC.JeliJeTakav.NEBITNO   : rbt_svdSvaSkl .Checked = true; break;
     //       case ZXC.JeliJeTakav.JE_TAKAV  : rbt_svdDonac  .Checked = true; break;
     //       case ZXC.JeliJeTakav.NIJE_TAKAV: rbt_svdNeDonac.Checked = true; break;
     //       default                        : rbt_svdSvaSkl .Checked = true; break;
     //    }
     // }

   }

   public bool Fld_UvozIzvozOnly { get { return cbx_uvozIzvozOnly.Checked; } set { cbx_uvozIzvozOnly.Checked = value; } }

   public decimal Fld_TH_Kune { get { return tbx_ThKune.GetDecimalField(); } set { tbx_ThKune.PutDecimalField(value); } }

   #endregion Fld_

   #region PutFilterFields(), GetFilterFields()

   private VvRpt_RiSk_Filter TheRtransFilter
   {
      // 06.04.2012: pa ovo nikada onda za PutFilterFields nije niti radilo?! 
      //get { return this.TheVvUC.VirtualRptFilter as VvRpt_RiSk_Filter; }
      //set {        this.TheVvUC.VirtualRptFilter = value; }
      get { return ((RiskReportUC)this.TheVvUC).TheRptFilter as VvRpt_RiSk_Filter; }
      set { ((RiskReportUC)this.TheVvUC).TheRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheRtransFilter = (VvRpt_RiSk_Filter)_filter_data;

      if(TheRtransFilter != null)
      {
         Fld_DatumOd       = TheRtransFilter.DatumOd;
         Fld_DatumDo       = TheRtransFilter.DatumDo.EndOfDay();

         Fld_ArtiklCdOd    = TheRtransFilter.ArtiklCdOD;
         Fld_ArtiklCdDo    = TheRtransFilter.ArtiklCdDO;
         Fld_ArtiklNazivOd = TheRtransFilter.ArtNameOD;
         Fld_ArtiklNazivDo = TheRtransFilter.ArtNameDO;
         Fld_TtNumOd       = TheRtransFilter.TtNumOd;
         Fld_TtNumDo       = TheRtransFilter.TtNumDo;

         Fld_TT            = TheRtransFilter.TT;

         ZXC.PutRiskFilterFieldsInProgress = true;
         Fld_MacroName = TheRtransFilter.MacroName;
         ZXC.PutRiskFilterFieldsInProgress = false;

         Fld_SkladCD                = TheRtransFilter.SkladCD;
         Fld_SkladCD2               = TheRtransFilter.SkladCD2;
         Fld_KD_naziv               = TheRtransFilter.KD_naziv;
         Fld_KD_sifra               = TheRtransFilter.KD_sifra;
         Fld_KD_ticker              = TheRtransFilter.KD_ticker;
         Fld_MT_naziv               = TheRtransFilter.MT_naziv;
         Fld_MT_sifra               = TheRtransFilter.MT_sifra;
         Fld_MT_ticker              = TheRtransFilter.MT_ticker;

         Fld_DokumSort              = TheRtransFilter.SorterType_Dokument;
         Fld_ArtiklSort             = TheRtransFilter.SorterType_Sifrar;
         Fld_NeedsHorizontalLine    = TheRtransFilter.NeedsHorizontalLine;
         Fld_ZaExportToExcel        = TheRtransFilter.ZaExportToExcel;

         Fld_IsOtsAnalitKontre      = TheRtransFilter.IsOtsAnalitKontre;
         Fld_IsOtsDospjecaPoDan     = TheRtransFilter.IsOtsDospjecaPoDan;
         Fld_IsOtsKontakt           = TheRtransFilter.IsOtsKontakt;
         Fld_IsOtsLineTipBr         = TheRtransFilter.IsOtsLineTipBr;
         Fld_IsOtsDospjelo          = TheRtransFilter.IsOtsDospOnly;
         Fld_KompenzacijaBroj       = TheRtransFilter.KompenzacijaBroj;
         Fld_OtsDate                = TheRtransFilter.OtsDate;
         Fld_ProjektCD              = TheRtransFilter.ProjektCD;
         Fld_PdvR12                 = TheRtransFilter.PdvR12;
         Fld_IsPrihodTT             = TheRtransFilter.IsPrihodTT;
         Fld_OcuGraf                = TheRtransFilter.OcuGraf;
         Fld_IsUserSastavio         = TheRtransFilter.IsUserSastavio;
         Fld_GrupiranjeDok          = TheRtransFilter.GrupiranjeDokum;
         Fld_GrupiranjeArtikla      = TheRtransFilter.GrupiranjeArtikla;
         //Fld_AnalitSintet            = TheRtransFilter.AnalitSintet           ;
         Fld_AnaGrupaPoStranici     = TheRtransFilter.AnaGrupaPoStranici;
         Fld_VisiblePostoGrupFooter = TheRtransFilter.VisiblePostoGrupFooter;
         Fld_VisibleOnlyTopGroups   = TheRtransFilter.VisibleOnlyTopGroups;
         Fld_NumOfTopGroups         = TheRtransFilter.NumOfTopGroups;
         Fld_TopSort                = TheRtransFilter.TopSort;
         Fld_PdvKnjiga              = TheRtransFilter.PdvKnjiga;
         Fld_PdvKredit              = TheRtransFilter.PdvKredit;
         Fld_PdvPovrat              = TheRtransFilter.PdvPovrat;
         Fld_PdvPredujam            = TheRtransFilter.PdvPredujam;
         Fld_PdvUstup               = TheRtransFilter.PdvUstup;
         Fld_NacinPlacanja          = TheRtransFilter.NacinPlacanja;
         Fld_PdvF_Osn               = TheRtransFilter.PdvF_Osn;
         Fld_PdvF_Pdv               = TheRtransFilter.PdvF_Pdv;
         Fld_PretpPriUvz_Osn        = TheRtransFilter.PretpPriUvz_Osn;
         Fld_PretpPriUvz_Pdv        = TheRtransFilter.PretpPriUvz_Pdv;
         Fld_PdvIspravak            = TheRtransFilter.PdvIspravak;
         Fld_PdvObrSastavio         = TheRtransFilter.PdvObrSastavio;
         Fld_IsPrjktTel             = TheRtransFilter.IsPrjktTel;
         Fld_IsPrjktFax             = TheRtransFilter.IsPrjktFax;
         Fld_IsPrjktMail            = TheRtransFilter.IsPrjktMail;
         Fld_Tel                    = TheRtransFilter.Tel;
         Fld_Fax                    = TheRtransFilter.Fax;
         Fld_Mail                   = TheRtransFilter.Mail;
         Fld_IsVisibleTT            = TheRtransFilter.IsVisibleTT;
         Fld_IsVisibleAdress        = TheRtransFilter.IsVisibleAdress;
         Fld_Status                 = TheRtransFilter.FuseStr1;
         Fld_CompDateOd             = TheRtransFilter.Date2;

         Fld_Pdv_Fof_III5           = TheRtransFilter.Pdv_Fof_III5;
         Fld_Pdv_Fof_III6           = TheRtransFilter.Pdv_Fof_III6;
         Fld_Pdv_Fof_III7           = TheRtransFilter.Pdv_Fof_III7;

         Fld_IsMpskPoNBC            = TheRtransFilter.FuseBool1;
         Fld_IsNoColorPrint         = TheRtransFilter.FuseBool2; // 14.09.2017. kod Artikl rpt ovo sluzi kao OnlyArtiklNoIzlaz 
         Fld_IsPoslJed              = TheRtransFilter.FuseBool3; // IsPoslJed instead of Kupdob 

         Fld_PdvGEOkind             = TheRtransFilter.PdvGeoKind;
         Fld_IsPecatotpis           = TheRtransFilter.IsPecatPotpis;
         Fld_OnlyArtiklSaStanjem    = TheRtransFilter.FuseBool4;

         Fld_Putnik_PersonCD        = TheRtransFilter.Putnik_PersonCD  ;
         Fld_Putnik_PersonName      = TheRtransFilter.Putnik_PersonName;
         // 04.12.2015: odkada je Putnik_PersonCD u RiskMacro DataLayeru treba i ovo: 
         if(TheRtransFilter.Putnik_PersonCD.NotZero() && TheRtransFilter.Putnik_PersonName.IsEmpty())
         {
            TheVvUC.SetSifrarAndAutocomplete<Person>(null, VvSQL.SorterType.None);

            Person personSifrar_rec = VvUserControl.PersonSifrar.SingleOrDefault(vvDR => vvDR.PersonCD == TheRtransFilter.Putnik_PersonCD);
            if(personSifrar_rec != null) Fld_Putnik_PersonName = personSifrar_rec.PrezimeIme;
            else                         Fld_Putnik_PersonName = "";
            
         }

         Fld_KupDobTip              = TheRtransFilter.FuseStr3;

         Fld_RptNapomena            = TheRtransFilter.Napomena;
         Fld_RptNaziv               = TheRtransFilter.FuseStr4;

         Fld_IsOtsNevezano          = TheRtransFilter.IsOtsNevezano;
         Fld_IsForceOtsByDokDate    = TheRtransFilter.IsForceOtsByDokDate;

         Fld_IsRashodTT             = TheRtransFilter.IsRashodTT;

         Fld_Pdv_831br              = TheRtransFilter.Pdv_831br ;
         Fld_Pdv_832br              = TheRtransFilter.Pdv_832br ;
         Fld_Pdv_833br              = TheRtransFilter.Pdv_833br ;
         Fld_Pdv_RedVII             = TheRtransFilter.Pdv_RedVII;
         Fld_Pdv_811                = TheRtransFilter.Pdv_811   ;
         Fld_Pdv_812                = TheRtransFilter.Pdv_812   ;
         Fld_Pdv_813                = TheRtransFilter.Pdv_813   ;
         Fld_Pdv_814                = TheRtransFilter.Pdv_814   ;
         Fld_Pdv_815                = TheRtransFilter.Pdv_815   ;
         Fld_Pdv_820                = TheRtransFilter.Pdv_820   ;
         Fld_Pdv_831vr              = TheRtransFilter.Pdv_831vr ;
         Fld_Pdv_832vr              = TheRtransFilter.Pdv_832vr ;
         Fld_Pdv_833vr              = TheRtransFilter.Pdv_833vr ;
         Fld_Pdv_860                = TheRtransFilter.Pdv_860   ;
         Fld_Pdv_840                = TheRtransFilter.Pdv_840   ;
                                    
         Fld_SkladFilter            = TheRtransFilter.SkladFilter;
                                    
         Fld_IsChkO                 = TheRtransFilter.IsChk0;
         Fld_IsOpzStatRnVezDok      = TheRtransFilter.IsChk0;
         Fld_IsBlgInIzvVal          = TheRtransFilter.IsBlgInIzvVal;

         Fld_Artikl_TS              = TheRtransFilter.Artikl_TS;
         Fld_Artikl_Gr1             = TheRtransFilter.Artikl_Gr1;
         Fld_Artikl_Gr2             = TheRtransFilter.Artikl_Gr2;
         Fld_Artikl_Gr3             = TheRtransFilter.Artikl_Gr3;
         Fld_FRUF_Name              = TheRtransFilter.FRUF_Name ;
         Fld_FRUF_Value             = TheRtransFilter.FRUF_Value;
         Fld_FRUF_BiloGdjeU         = TheRtransFilter.FRUF_BiloGdjeU;

         Fld_SVD_KnjBr_OD           = TheRtransFilter.SVD_KnjBr_OD;
         Fld_SVD_KnjBr_DO           = TheRtransFilter.SVD_KnjBr_DO;
         Fld_SVD_LiP                = TheRtransFilter.SVD_LiP     ;
       //Fld_SVD_IsDonacSklad   = TheRtransFilter.SVD_IsDonacSklad;

         Fld_UvozIzvozOnly          = TheRtransFilter.UvozIzvozOnly;
         Fld_TH_Kune                = TheRtransFilter.TH_Blg_uKunama;
      }

      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
      // 06.04.2012: ovo sluzi de prekine 'vezu' izmedju refrence type-ova, odervajs ako si imao selektiran RiskMacro onda bi sadrzaj macro-a bio kompromotiran ovim GetFields-om!
      TheRtransFilter = new VvRpt_RiSk_Filter();
      // FUSEs =============================================

    //TheRtransFilter.Napomena =     // 23.06.2014. za napomenu na reportu
      TheRtransFilter.NacPlac  =
      TheRtransFilter.VezniDok =
      TheRtransFilter.OtsSaldoKompenzacijaAsText =
    //TheRtransFilter.FuseStr1 =     // Status
      TheRtransFilter.FuseStr2 = ""; // od 25.11.2012: this is reportShortName
    //TheRtransFilter.FuseStr3 =     // 06.02.2014. KupDobTip
    //TheRtransFilter.FuseStr4 = ""; // 23.06.2014. naziv reporta

      // Ovaj se koristi kod Artikla, ali je u biti fuse
      TheRtransFilter.GrupaKupDob = "";

      // ===================================================

      TheRtransFilter.DatumOd = Fld_DatumOd;
      TheRtransFilter.DatumDo = Fld_DatumDo.EndOfDay();

      TheRtransFilter.ArtiklCdOD = Fld_ArtiklCdOd;
      TheRtransFilter.ArtiklCdDO = Fld_ArtiklCdDo;
      TheRtransFilter.ArtNameOD = Fld_ArtiklNazivOd;
      TheRtransFilter.ArtNameDO = Fld_ArtiklNazivDo;

      TheRtransFilter.TtNumOd = Fld_TtNumOd;
      TheRtransFilter.TtNumDo = Fld_TtNumDo;

      ZXC.TheVvForm.VvPref.reportPrefs.RiskTT     = TheRtransFilter.TT = Fld_TT;
      ZXC.TheVvForm.VvPref.reportPrefs.IsPrihodTT = TheRtransFilter.IsPrihodTT = Fld_IsPrihodTT;
      ZXC.TheVvForm.VvPref.reportPrefs.IsRashodTT = TheRtransFilter.IsRashodTT = Fld_IsRashodTT;
      ZXC.TheVvForm.VvPref.reportPrefs.MacroName  = TheRtransFilter.MacroName = Fld_MacroName;

      TheRtransFilter.SkladCD  = Fld_SkladCD;
      TheRtransFilter.SkladCD2 = Fld_SkladCD2;

      TheRtransFilter.SorterType_Dokument    = Fld_DokumSort;
      TheRtransFilter.SorterType_Sifrar      = Fld_ArtiklSort;
      TheRtransFilter.GrupiranjeDokum        = Fld_GrupiranjeDok;
      TheRtransFilter.GrupiranjeArtikla      = Fld_GrupiranjeArtikla;
      //TheRtransFilter.AnalitSintet              = Fld_AnalitSintet;           
      TheRtransFilter.AnaGrupaPoStranici     = Fld_AnaGrupaPoStranici;
      TheRtransFilter.VisiblePostoGrupFooter = Fld_VisiblePostoGrupFooter;
      TheRtransFilter.VisibleOnlyTopGroups   = Fld_VisibleOnlyTopGroups;
      TheRtransFilter.NumOfTopGroups         = Fld_NumOfTopGroups;
      TheRtransFilter.TopSort                = Fld_TopSort;
      TheRtransFilter.PdvKnjiga              = Fld_PdvKnjiga;
      TheRtransFilter.PdvKredit              = Fld_PdvKredit;
      TheRtransFilter.PdvPovrat              = Fld_PdvPovrat;
      TheRtransFilter.PdvPredujam            = Fld_PdvPredujam;
      TheRtransFilter.PdvUstup               = Fld_PdvUstup;
      TheRtransFilter.NacinPlacanja          = Fld_NacinPlacanja;
      TheRtransFilter.PdvF_Osn               = Fld_PdvF_Osn;
      TheRtransFilter.PdvF_Pdv               = Fld_PdvF_Pdv;
      TheRtransFilter.PretpPriUvz_Osn        = Fld_PretpPriUvz_Osn;
      TheRtransFilter.PretpPriUvz_Pdv        = Fld_PretpPriUvz_Pdv;
      TheRtransFilter.PdvIspravak            = Fld_PdvIspravak;
      TheRtransFilter.PdvObrSastavio         = Fld_PdvObrSastavio;
      TheRtransFilter.IsPrjktTel             = Fld_IsPrjktTel;
      TheRtransFilter.IsPrjktFax             = Fld_IsPrjktFax;
      TheRtransFilter.IsPrjktMail            = Fld_IsPrjktMail;
      TheRtransFilter.Tel                    = Fld_Tel;
      TheRtransFilter.Fax                    = Fld_Fax;
      TheRtransFilter.Mail                   = Fld_Mail;
      TheRtransFilter.IsVisibleTT            = Fld_IsVisibleTT;
      TheRtransFilter.IsVisibleAdress        = Fld_IsVisibleAdress;
      TheRtransFilter.KD_naziv               = Fld_KD_naziv;
      TheRtransFilter.KD_sifra               = Fld_KD_sifra;
      TheRtransFilter.KD_ticker              = Fld_KD_ticker;
      TheRtransFilter.MT_naziv               = Fld_MT_naziv;
      TheRtransFilter.MT_sifra               = Fld_MT_sifra;
      TheRtransFilter.MT_ticker              = Fld_MT_ticker;
      TheRtransFilter.IsUserSastavio         = Fld_IsUserSastavio;
      TheRtransFilter.IsOtsAnalitKontre      = Fld_IsOtsAnalitKontre;
      TheRtransFilter.IsOtsDospjecaPoDan     = Fld_IsOtsDospjecaPoDan;
      TheRtransFilter.IsOtsKontakt           = Fld_IsOtsKontakt;
      TheRtransFilter.IsOtsLineTipBr         = Fld_IsOtsLineTipBr;
      TheRtransFilter.KompenzacijaBroj       = Fld_KompenzacijaBroj;
      TheRtransFilter.OtsDate                = Fld_OtsDate;
      TheRtransFilter.IsOtsDospOnly          = Fld_IsOtsDospjelo;
      TheRtransFilter.ProjektCD              = Fld_ProjektCD;
      TheRtransFilter.PdvR12                 = Fld_PdvR12;
      TheRtransFilter.OcuGraf                = Fld_OcuGraf;
      TheRtransFilter.FuseStr1               = Fld_Status;
      TheRtransFilter.Date2                  = Fld_CompDateOd;

      TheRtransFilter.NeedsHorizontalLine    = Fld_NeedsHorizontalLine;
      TheRtransFilter.ZaExportToExcel        = Fld_ZaExportToExcel;

      TheRtransFilter.Pdv_Fof_III5           = Fld_Pdv_Fof_III5  ;
      TheRtransFilter.Pdv_Fof_III6           = Fld_Pdv_Fof_III6  ;
      TheRtransFilter.Pdv_Fof_III7           = Fld_Pdv_Fof_III7  ;
      TheRtransFilter.FuseBool1              = Fld_IsMpskPoNBC   ;
      TheRtransFilter.FuseBool2              = Fld_IsNoColorPrint;
      TheRtransFilter.FuseBool3              = Fld_IsPoslJed     ; // IsPoslJed instead of Kupdob 
                                             
      TheRtransFilter.PdvGeoKind             = Fld_PdvGEOkind   ;
      TheRtransFilter.IsPecatPotpis          = Fld_IsPecatotpis ;
      TheRtransFilter.FuseBool4              = Fld_OnlyArtiklSaStanjem;
                                             
      TheRtransFilter.Putnik_PersonCD        = Fld_Putnik_PersonCD  ;
      TheRtransFilter.Putnik_PersonName      = Fld_Putnik_PersonName;
      TheRtransFilter.FuseStr3               = Fld_KupDobTip ;
      TheRtransFilter.Napomena               = Fld_RptNapomena;
      TheRtransFilter.FuseStr4               = Fld_RptNaziv;

      TheRtransFilter.IsOtsNevezano          = Fld_IsOtsNevezano ;
      TheRtransFilter.IsForceOtsByDokDate    = Fld_IsForceOtsByDokDate;

      TheRtransFilter.Pdv_831br              = Fld_Pdv_831br   ;
      TheRtransFilter.Pdv_832br              = Fld_Pdv_832br   ;
      TheRtransFilter.Pdv_833br              = Fld_Pdv_833br   ;
      TheRtransFilter.Pdv_RedVII             = Fld_Pdv_RedVII  ;
      TheRtransFilter.Pdv_811                = Fld_Pdv_811     ;
      TheRtransFilter.Pdv_812                = Fld_Pdv_812     ;
      TheRtransFilter.Pdv_813                = Fld_Pdv_813     ;
      TheRtransFilter.Pdv_814                = Fld_Pdv_814     ;
      TheRtransFilter.Pdv_815                = Fld_Pdv_815     ;
      TheRtransFilter.Pdv_820                = Fld_Pdv_820     ;
      TheRtransFilter.Pdv_831vr              = Fld_Pdv_831vr   ;
      TheRtransFilter.Pdv_832vr              = Fld_Pdv_832vr   ;
      TheRtransFilter.Pdv_833vr              = Fld_Pdv_833vr   ;
      TheRtransFilter.Pdv_860                = Fld_Pdv_860     ;
      TheRtransFilter.Pdv_840                = Fld_Pdv_840     ;

      TheRtransFilter.SkladFilter            = Fld_SkladFilter      ;
      TheRtransFilter.IsChk0                 = Fld_IsChkO           ;
      TheRtransFilter.IsOpzStatRnVezDok      = Fld_IsOpzStatRnVezDok;
      TheRtransFilter.IsBlgInIzvVal          = Fld_IsBlgInIzvVal    ;
      TheRtransFilter.Artikl_TS              = Fld_Artikl_TS        ;
      TheRtransFilter.Artikl_Gr1             = Fld_Artikl_Gr1       ;
      TheRtransFilter.Artikl_Gr2             = Fld_Artikl_Gr2       ;
      TheRtransFilter.Artikl_Gr3             = Fld_Artikl_Gr3       ;
      TheRtransFilter.FRUF_Name              = Fld_FRUF_Name        ;
      TheRtransFilter.FRUF_Value             = Fld_FRUF_Value       ;
      TheRtransFilter.FRUF_BiloGdjeU         = Fld_FRUF_BiloGdjeU   ;
      TheRtransFilter.SVD_KnjBr_OD           = Fld_SVD_KnjBr_OD     ;
      TheRtransFilter.SVD_KnjBr_DO           = Fld_SVD_KnjBr_DO     ;
      TheRtransFilter.SVD_LiP                = Fld_SVD_LiP          ;
      TheRtransFilter.SVD_IsDonacSklad       = Fld_SVD_IsDonacSklad;
      TheRtransFilter.UvozIzvozOnly          = Fld_UvozIzvozOnly;
      TheRtransFilter.TH_Blg_uKunama                = Fld_TH_Kune ;

   }

   #endregion PutFilterFields(), GetFilterFields()

   #region ArtiklList_Click // FakturList_Click

   //private void ArtiklList_Click(object sender, EventArgs e)
   //{
   //   if(((RiskReportUC)TheVvUC).TheFilterPanelBottom.Visible == false)
   //   {
   //      ((RiskReportUC)TheVvUC).TheFilterPanelBottom.Visible = true;
   //      btn_ArtiklList.Text = "Artikl -";
   //      artiklListUc        = new ArtiklListUC(((RiskReportUC)TheVvUC).TheFilterPanelBottom, artikl_rec, new VvForm.VvSubModul(true));

   //      if(fakturListUc != null) fakturListUc.Visible = false;

   //      SetNotVisibilitiRecListHampers(artiklListUc);
   //   }
   //   else
   //   {
   //      if(fakturListUc != null && fakturListUc.Visible)
   //      {
   //         fakturListUc.Visible = false;
   //         btn_ArtiklList.Text = "Artikl -";
   //         artiklListUc = new ArtiklListUC(((RiskReportUC)TheVvUC).TheFilterPanelBottom, artikl_rec, new VvForm.VvSubModul(true));
   //         SetNotVisibilitiRecListHampers(artiklListUc);
   //      }
   //      else
   //      {
   //         ((RiskReportUC)TheVvUC).TheFilterPanelBottom.Visible = false;
   //         btn_ArtiklList.Text = "Artikl +";
   //      }
   //   }

   //}
   //private void FakturList_Click(object sender, EventArgs e)
   //{
   //   if(((RiskReportUC)TheVvUC).TheFilterPanelBottom.Visible == false)
   //   {
   //      ((RiskReportUC)TheVvUC).TheFilterPanelBottom.Visible = true;
   //      btn_FakturList.Text = "Faktur -";
   //      fakturListUc = new FakturListUC(((RiskReportUC)TheVvUC).TheFilterPanelBottom, faktur_rec, new VvForm.VvSubModul(true), new VvForm.VvSubModul(true));
   //      if(artiklListUc != null) artiklListUc.Visible = false;
   //      SetNotVisibilitiRecListHampers(fakturListUc);
   //   }
   //   else
   //   {

   //      if(artiklListUc != null && artiklListUc.Visible)
   //      {
   //         artiklListUc.Visible = false;
   //         btn_FakturList.Text = "Faktur -";
   //         fakturListUc = new FakturListUC(((RiskReportUC)TheVvUC).TheFilterPanelBottom, faktur_rec, new VvForm.VvSubModul(true), new VvForm.VvSubModul(true));
   //         SetNotVisibilitiRecListHampers(fakturListUc);
   //      }
   //      else
   //      {
   //         ((RiskReportUC)TheVvUC).TheFilterPanelBottom.Visible = false;
   //         btn_FakturList.Text = "Faktur +";
   //      }
   //   }

   //}

   private void SetNotVisibilitiRecListHampers(VvRecLstUC vvRecListUC)
   {
      vvRecListUC.TheGrid.Visible =
      vvRecListUC.grBoxLimitiraj.Visible =
      vvRecListUC.hampListaRastePada.Visible =
      vvRecListUC.hampSpecifikum.Visible =
      vvRecListUC.hampIzlistaj.Visible =
      vvRecListUC.hampOpenFilter.Visible = false;
      vvRecListUC.tbx_limitNum.Text = "10";

      if(vvRecListUC.hampOpenUtil != null) // zato sto obicni korisnici nemaju util neo samo superuser 
      {
         vvRecListUC.hampOpenUtil.Visible =
         vvRecListUC.hampUtil.Visible = false;
      }

      vvRecListUC.hampFilter.Visible = true;
      vvRecListUC.hampFilter.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);

      vvRecListUC.Size = new Size(vvRecListUC.hampFilter.Width + ZXC.Q2un, vvRecListUC.hampFilter.Bottom + ZXC.Q2un);
      ((RiskReportUC)TheVvUC).TheFilterPanelBottom.Height = vvRecListUC.Bottom;
   }

   #endregion ArtiklList_Click // FakturList_Click

   #region AddFilterMemberz() --- VOILA! ---

   #region Sam Lokal Propertiz

   private DataRowCollection    ArtSch   { get { return ZXC.ArtiklDao.TheSchemaTable.Rows; } }
   private ArtiklDao.ArtiklCI   ArtCI    { get { return ZXC.ArtiklDao.CI; } }
   private DataRowCollection    ArsSch   { get { return ZXC.ArtStatDao.TheSchemaTable.Rows; } }
   private ArtStatDao.ArtStatCI ArsCI    { get { return ZXC.ArtStatDao.CI; } }
   private DataRowCollection    FakSch   { get { return ZXC.FakturDao.TheSchemaTable.Rows; } }
   private FakturDao.FakturCI   FakCI    { get { return ZXC.FakturDao.CI; } }
   private DataRowCollection    FakExSch { get { return ZXC.FaktExDao.TheSchemaTable.Rows; } }
   private FaktExDao.FaktExCI   FakExCI  { get { return ZXC.FaktExDao.CI; } }
   private DataRowCollection    RtrSch   { get { return ZXC.RtransDao.TheSchemaTable.Rows; } }
   private RtransDao.RtransCI   RtrCI    { get { return ZXC.RtransDao.CI; } }
   private DataRowCollection    FtrSch   { get { return ZXC.FtransDao.TheSchemaTable.Rows; } }
   private FtransDao.FtransCI   FtrCI    { get { return ZXC.FtransDao.CI; } }
   private DataRowCollection    KupSch   { get { return ZXC.KupdobDao.TheSchemaTable.Rows; } }
   private KupdobDao.KupdobCI   KupCI    { get { return ZXC.KupdobDao.CI; } }

   #endregion Sam Lokal Propertiz

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
      VvRpt_RiSk_Filter theRptFilter    = (VvRpt_RiSk_Filter)_vvRptFilter;
      VvRiskReport      theVvRiskReport = (VvRiskReport     )_vvReport   ;

      DateTime dateODorig, dateDOorig, dateOD, dateDO;
      string comparer;
      string text;
      bool isDummy;
      uint numOD  , numDO  , num;
      uint num2_OD, num2_DO     ;
      DataRow drSchema;

      theRptFilter.FilterMembers.Clear();
      theRptFilter.ClearAllFilters_FromClauseGotTableName();

      //isDummy = false;
      ZXC.RIZ_FilterStyle filterStyle = theVvRiskReport.FilterStyle;
      bool isArtiklFilterStyle = filterStyle == ZXC.RIZ_FilterStyle.Artikl;

      // !!! --- !!! 
      if(isArtiklFilterStyle)                   { AddFilterMemberzFor_Artikl_FilterStyle(theRptFilter, theVvRiskReport); return; }
      if(theVvRiskReport is RptR_PDV)           { AddFilterMemberzFor_Pdv_FilterStyle   (theRptFilter, theVvRiskReport); return; }
      if(theVvRiskReport is RptR_EU_PdvGEOkind) { AddFilterMemberzFor_GEOPdv_FilterStyle(theRptFilter, theVvRiskReport); return; }
      if(theVvRiskReport is RptR_OTS)           { AddFilterMemberzFor_OTS_FilterStyle   (theRptFilter, theVvRiskReport); return; }
      if(theVvRiskReport is RptR_Kamate_Kartica){ AddFilterMemberzFor_OTS_FilterStyle   (theRptFilter, theVvRiskReport); return; }
      if(theVvRiskReport is RptR_BLAG)          { AddFilterMemberzFor_BLAG_FilterStyle  (theRptFilter, theVvRiskReport); return; }
      if(theVvRiskReport is RptR_KnjigaPopisa)  { AddFilterMemberzFor_KPM_FilterStyle   (theRptFilter, theVvRiskReport); return; }

      #region Classic filters

      // Fld_DatumOdDo                                                                                                                                                   

      switch(filterStyle)
      {
         //case ZXC.RIZ_FilterStyle.Artikl: drSchema = ArsSch[ArsCI.t_skladDate]; break;
         case ZXC.RIZ_FilterStyle.Faktur: drSchema = FakSch[FakCI.dokDate]; break;
         case ZXC.RIZ_FilterStyle.Rtrans: drSchema = RtrSch[RtrCI.t_skladDate]; break;
         //case ZXC.RIZ_FilterStyle.Ftrans: drSchema = FtrSch[FtrCI.t_dokDate  ]; break;

         default: drSchema = null; break;
      }

      dateODorig = dateOD = theRptFilter.DatumOd;
      dateDOorig = dateDO = theRptFilter.DatumDo.EndOfDay();

      if(theVvRiskReport.VirtualReportDocument is Vektor.Reports.RIZ.CR_ProvizijaKomerc) // force lowest dateOD, highest dateDO 
      { // ovdje, dakle, kronolosko ogranicenje ne smije trzati na fakture nego samo na uplate 
         theRptFilter.Date3 = theRptFilter.DatumOd;
         theRptFilter.DatumOd = dateOD = /*DateTime.MinValue*/ZXC.Date01012014;
         dateDO = ZXC.projectYearLastDay;
      }

      if(dateOD == dateDO) comparer = " = " ;
      else                 comparer = " >= ";

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DateOD", dateOD, dateODorig.ToString("dd.MM.yyyy."), "Od datuma:", comparer, ""));
      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DateDO", dateDO, dateDOorig.ToString("dd.MM.yyyy."), "Do datuma:", " <= ", ""));

      // Fld_TTnumOdDo                                                                                                                                     

      drSchema = ZXC.FakturSchemaRows[ZXC.FakCI.ttNum];
      numOD = theRptFilter.TtNumOd;
      numDO = theRptFilter.TtNumDo;

      if(numOD.NotZero())
      {
         if(numOD == numDO) comparer = " = ";
         else comparer = " >= ";

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNumOD", numOD, numOD.ToString(""), "Od TT broj:", comparer, ""/*, Rtrans.recordName*/));
      }
      else if(numDO.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "TtNumOD", numOD, "-", "Od TT broj:", " >= ", ""/*, Rtrans.recordName*/));
      }

      if(numDO.NotZero())
      {
         if(numDO == numOD) isDummy = true;
         else isDummy = false;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "TtNumDO", numDO, numDO.ToString(""), "Do TT broj:", " <= ", ""/*, Rtrans.recordName*/));
      }
      else if(numOD.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "TtNumDO", numDO, "-", "Do TT broj:", " <= ", ""/*, Rtrans.recordName*/));
      }
      
      #region RptR_PrometArtikla: Artikl od - do

      if(theVvRiskReport is RptR_PrometArtikla)
      {
         if(theRptFilter.ArtNameOD .NotEmpty()) theRptFilter.FilterMembers.Add(new VvSqlFilterMember(RtrSch[RtrCI.t_artiklName], false, "ArtiklNameOD", theRptFilter.ArtNameOD , theRptFilter.ArtNameOD , "Od naziva artikla:", " >= ", ""     ));
         if(theRptFilter.ArtNameDO .NotEmpty()) theRptFilter.FilterMembers.Add(new VvSqlFilterMember(RtrSch[RtrCI.t_artiklName], false, "ArtiklNameDO", theRptFilter.ArtNameDO , theRptFilter.ArtNameDO , "Do naziva artikla:", " <= ", ""     ));
         if(theRptFilter.ArtiklCdOD.NotEmpty()) theRptFilter.FilterMembers.Add(new VvSqlFilterMember(RtrSch[RtrCI.t_artiklCD  ], false, "ArtiklCdOD"  , theRptFilter.ArtiklCdOD, theRptFilter.ArtiklCdOD, "Od šifre artikla:" , " >= ", "", "R"));
         if(theRptFilter.ArtiklCdDO.NotEmpty()) theRptFilter.FilterMembers.Add(new VvSqlFilterMember(RtrSch[RtrCI.t_artiklCD  ], false, "ArtiklCdDO"  , theRptFilter.ArtiklCdDO, theRptFilter.ArtiklCdDO, "Do šifre artikla:" , " <= ", "", "R"));

         // 15.02.2019: 
         if(theRptFilter.Artikl_TS .NotEmpty()) theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.artiklTS], false, "Artikl_TS"   , theRptFilter.Artikl_TS  , theRptFilter.Artikl_TS  + " " + ZXC.luiListaArtiklTS     .GetNameForThisCd(theRptFilter.Artikl_TS ), "Za tip artikla:", " = ", ""/*ArtStat.recordName*/));
         if(theRptFilter.Artikl_Gr1.NotEmpty()) theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.artGrCd1], false, "Artikl_Gr1"  , theRptFilter.Artikl_Gr1 , theRptFilter.Artikl_Gr1 + " " + ZXC.luiListaGrupa1Artikla.GetNameForThisCd(theRptFilter.Artikl_Gr1), "Za artikl GrA:" , " = ", ""/*ArtStat.recordName*/));
         if(theRptFilter.Artikl_Gr2.NotEmpty()) theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.artGrCd2], false, "Artikl_Gr2"  , theRptFilter.Artikl_Gr2 , theRptFilter.Artikl_Gr2 + " " + ZXC.luiListaGrupa2Artikla.GetNameForThisCd(theRptFilter.Artikl_Gr2), "Za artikl GrB:" , " = ", ""/*ArtStat.recordName*/));
         if(theRptFilter.Artikl_Gr3.NotEmpty()) theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.artGrCd3], false, "Artikl_Gr3"  , theRptFilter.Artikl_Gr3 , theRptFilter.Artikl_Gr3 + " " + ZXC.luiListaGrupa3Artikla.GetNameForThisCd(theRptFilter.Artikl_Gr3), "Za artikl GrC:" , " = ", ""/*ArtStat.recordName*/));

      }

      #endregion Artikl od - do

      #region SvDUH Knjigovodstveni Broj Od - DO

      if(ZXC.IsSvDUH)
      {
         //  Fld_pdvZPkind                                                                                                                                                                           

         bool imaLiSmislaFiltriratiPo_LiP_FakturDataLayeru = true;

         if(theRptFilter.TT != Faktur.TT_URA)
         {
            imaLiSmislaFiltriratiPo_LiP_FakturDataLayeru = false;
         }
         else // TT == URA 
         {
            // PAZI: !!! ovdje treba navesti svaki izvj kojeg zelimo filtrirati po LiP osnovu 
            // a sapada pd stanje skladista / lager lista / promet artikla                    
            if(
               theVvRiskReport.VirtualReportDocument is Vektor.Reports.RIZ.CR_LagerLista ||
               theVvRiskReport.VirtualReportDocument is Vektor.Reports.RIZ.CR_StanjeSklad_B ||
               theVvRiskReport.VirtualReportDocument is Vektor.Reports.RIZ.CR_PrometArtikla_Grup
               )
            {
               imaLiSmislaFiltriratiPo_LiP_FakturDataLayeru = false;
            }
         }

         text = (theRptFilter.SVD_LiP == ZXC.PdvZPkindEnum.SVD_LJEK) ? "LIJEKOVI" : (theRptFilter.SVD_LiP == ZXC.PdvZPkindEnum.SVD_POTR) ? "POTROŠNI" : "LIJEKOVI I POTROŠNI";
         // 21.01.2022 : 
       //if(                                                theRptFilter.SVD_LiP != ZXC.PdvZPkindEnum.NoZP) 
         if(imaLiSmislaFiltriratiPo_LiP_FakturDataLayeru && theRptFilter.SVD_LiP != ZXC.PdvZPkindEnum.NoZP)
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.pdvZPkind], false, "LP", theRptFilter.SVD_LiP, text /*theRptFilter.SVD_LiP.ToString()*/, "Za L/P tip računa:", " = ", ""));
         }

         // SvDUH Only: OD - DO VezniDok2                                                                                                                   

         num2_OD = ZXC.ValOrZero_UInt(theRptFilter.SVD_KnjBr_OD);
         num2_DO = ZXC.ValOrZero_UInt(theRptFilter.SVD_KnjBr_DO);

         if(num2_OD.NotZero() && num2_DO.NotZero())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.vezniDok2], "veza2", "", " != "));

            theRptFilter.FilterMembers.Add(new VvSqlFilterMember("CAST(veznidok2 AS UNSIGNED)", "KnjBrOD", num2_OD, theRptFilter.SVD_KnjBr_OD, "Knj Br OD:", " >= "));
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember("CAST(veznidok2 AS UNSIGNED)", "KnjBrDO", num2_DO, theRptFilter.SVD_KnjBr_DO, "Knj Br DO:", " <= "));
         }

      } // if(ZXC.IsSvDUH) 

      #endregion SvDUH Knjigovodstveni Broj Od - DO

      // Fld_KupdobCD                                                                                                                                     

      if(theRptFilter.KD_sifra.NotZero() && 
         (theVvRiskReport is RptR_StanjePoDobav ) == false && 
         (theVvRiskReport is RptR_ProdajaPoDobav) == false)
      {
         if(theRptFilter.FuseBool3) // IsPoslJed instead of Kupdob 
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.posJedCD], false, "kupdobCD", theRptFilter.KD_sifra, theRptFilter.KD_naziv, "Za Partnera:", " = ", ""));
         else
         {
            if(ZXC.IsSvDUH && theRptFilter.KD_naziv.Length == 1)
            {
               theRptFilter.FilterMembers.Add(new VvSqlFilterMember(KupSch[KupCI.isMtr ], false, "isMtr" , true,                  "",                    "",                " = ", Kupdob.recordName + "_fak", "R"));
               theRptFilter.FilterMembers.Add(new VvSqlFilterMember(KupSch[KupCI.ulica1], false, "ulica1", theRptFilter.KD_naziv, theRptFilter.KD_naziv, "Zavod / klinika", " = ", Kupdob.recordName + "_fak", "R"));
            }
            else // classic 
            {
               theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.kupdobCD], false, "kupdobCD", theRptFilter.KD_sifra, theRptFilter.KD_naziv, "Za Partnera:", " = ", ""));
            }
         }
      }

      // Fld_Putnik_PersonCD                                                                                                                                     

      if(theRptFilter.Putnik_PersonCD.NotZero() && 
         (theVvRiskReport is RptR_StanjePoDobav ) == false && 
         (theVvRiskReport is RptR_ProdajaPoDobav) == false)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.personCD], false, "personCD", theRptFilter.Putnik_PersonCD, theRptFilter.Putnik_PersonName, "Za Komercijalistu:", " = ", ""));
      }

      // Fld_MtrosCD                                                                                                                                     

      if(theRptFilter.MT_sifra.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.mtrosCD], false, "MtrosCD", theRptFilter.MT_sifra, theRptFilter.MT_naziv, "Za mjesto troška:", " = ", ""));
      }

      isDummy = false;

      // Fld_kupdobTip KUPDOB JOIN!                                                                                                                                     

      if(theRptFilter.FuseStr3.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(KupSch[KupCI.tip], false, "kupdobTip", theRptFilter.FuseStr3, theRptFilter.FuseStr3, "Za TIP partnera:", " = ", Kupdob.recordName + "_fak", "R"));
      }

      //  Fld_TT                                                                                                                                                                           
      
      if(theVvRiskReport is RptR_StanjePoDobav)
      {
         string ulazIzlazClause = ArtiklDao.Rtr_UlazIzlaz_Kol;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", "prihod", ulazIzlazClause, "Svi relevantni.", "Za tip:", " IN "));
      }
      else if(theRptFilter.IsPrihodTT || theVvRiskReport is RptR_Rekap_FISK_Faktur) // IFA, IRA, IRM, IOD, IPV 
      {
         string IN_clause = TtInfo.Prihod_IN_Clause;

       //theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", IN_clause, " IN "));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", "prihod", IN_clause, "Prihod - IFA, IRA, IRM, IOD, IPV", "Za tip:", " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!
      }
      else if(theRptFilter.IsRashodTT                                             ) // UFA, URA, URM, UOD, UPV, UPM, UFM, UPA 
      {
         string IN_clause = TtInfo.Rashod_IN_Clause;

       //theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", IN_clause, " IN "));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", "rashod", IN_clause, "Rashod - UFA, URA, URM, UOD, UPV, UPM, UFM, UPA", "Za tip:", " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!
      }
      else // Classic, just one/all TT filter 
      {
         drSchema = ZXC.FakturSchemaRows[ZXC.FakCI.tt];

         text = theRptFilter.TT;
         if(text.NotEmpty()) { theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TT", text, text, "Za tip:", " = ", "")); }
      }
      // Fld_SkladCD                                                                                                                                                   

      switch(theVvRiskReport.FilterStyle)
      {
         //case ZXC.RIZ_FilterStyle.Artikl: drSchema = ArsSch[ArsCI.t_skladCD]; break;
         case ZXC.RIZ_FilterStyle.Faktur: drSchema = FakSch[FakCI.skladCD]; break;
         case ZXC.RIZ_FilterStyle.Rtrans: drSchema = RtrSch[RtrCI.t_skladCD]; break;

         default: drSchema = null; break;
      }
      text = theRptFilter.SkladCD;

      bool is2sklad = theRptFilter.TtInfo.HasTwinTT || theRptFilter.TtInfo.HasSplitTT;

      if(text.NotEmpty())
      {
         // 01.09.2015: 
       //theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "SkladCD", text, text,                              "Za skladište:", " = ", ""));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "SkladCD", text, text, is2sklad ? "Sa skladišta:" : "Za skladište:", " = ", ""));
      }

      // 27.08.2015: Fld_SkladCD2                                                                                                                                                   

      switch(theVvRiskReport.FilterStyle)
      {
       //case ZXC.RIZ_FilterStyle.Artikl: drSchema = ArsSch[ArsCI.t_skladCD]; break;
         case ZXC.RIZ_FilterStyle.Faktur: drSchema = FakSch[FakCI.skladCD2 ]; break;
         case ZXC.RIZ_FilterStyle.Rtrans: drSchema = RtrSch[RtrCI.t_skladCD]; break;

         default: drSchema = null; break;
      }
      text = theRptFilter.SkladCD2;

      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "SkladCD2", text, text, is2sklad ? "Na skladište:" : "Za skladište2:", " = ", ""));
      }

      //  Fld_nacPlac                                                                                                                                                                           
      drSchema = ZXC.FaktExSchemaRows[ZXC.FexCI.nacPlac];

      text = theRptFilter.NacinPlacanja;
      if(text.NotEmpty()) { theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "NacinPlac", text, text, "Za način plaćanja:", " = ", "")); }


      //                                                                                                                                                                     

      // Fld_Projekt                                                                                                                                         

      drSchema = ZXC.FakturSchemaRows[ZXC.FakCI.projektCD];
      text = theRptFilter.ProjektCD;

      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "ProjektCD", text, text, "Za projekt:", " = ", ""));
      }

      //  Fld_pdvR12tip                                                                                                                                                                           

      if(theRptFilter.PdvR12 != ZXC.PdvR12Enum.NIJEDNO)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.pdvR12], false, "PdvR12tip", theRptFilter.PdvR12, theRptFilter.PdvR12.ToString(), "Za PDV tip računa:", " = ", ""));
      }

      //  Fld_ Uvoz / Izvoz                                                                                                                                                                           

      drSchema = ZXC.FaktExSchemaRows[ZXC.FexCI.pdvGEOkind];
      if(theRptFilter.UvozIzvozOnly == true)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, ZXC.FM_OR_Enum.OPEN_OR , false, "UvIzOnlyOR1", ZXC.PdvGEOkindEnum.EU   , "", "Uvoz/Izvoz", " = ", ""));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.devName], ZXC.FM_OR_Enum.NONE , false, "UvIzOnlyORdname", "EUR"   , "", "Uvoz/Izvoz", " = ", ""));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, ZXC.FM_OR_Enum.CLOSE_OR, false, "UvIzOnlyOR2", ZXC.PdvGEOkindEnum.WORLD, "", "Uvoz/Izvoz", " = ", ""));
      }

      //  Fld_Status                                                                                                                                                                          
      drSchema = ZXC.FaktExSchemaRows[ZXC.FexCI.statusCD];

      text = theRptFilter.FuseStr1;
      if(text.NotEmpty()) { theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Status", text, text, "Za status:", " REGEXP ", "")); }

      ////  Fld_IsMpskPoNBC                                                                                                                                                                         

      //if(Fld_IsMpskPoNBC)
      //{
      //   theRptFilter.FilterMembers.Add(new VvSqlFilterMember("IsMpskPoNBC", true));
      //}

      // 24.10.2017: 
      // TEMBO Dokumenti u nizu: samo kunske / samo devizne 
      if(ZXC.IsTEMBO && theVvRiskReport is RptR_PrnManyFak)
      {
         RptR_PrnManyFak rptR_PrnManyFak = theVvRiskReport as RptR_PrnManyFak;

         drSchema = ZXC.FaktExSchemaRows[ZXC.FexCI.devName];

         if(rptR_PrnManyFak.subDsc == 0) // Daj samo kunske 
         {
            text = ""; // faktur_rec.DevName == ""; 
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DevName", text, text, "Za valutu plaćanja:", " = ", ""));

            // tu bude BUG ako koriste doslovno "HRK"! 

         }
         else if(rptR_PrnManyFak.subDsc == 4) // Daj samo devizne 
         {
            text = ""; // faktur_rec.DevName != ""; 
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DevName1", text, text, "Za valutu plaćanja:", " != ", ""));
            text = /*"HRK"*/ ZXC.EURorHRKstr; // faktur_rec.DevName != "HRK"; 
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DevName2", text, text, "Za valutu plaćanja:", " != ", ""));
         }

      } // if(ZXC.IsTEMBO && theVvRiskReport is RptR_PrnManyFak) 

      #endregion Classic filters

      #region FRUF

      string FRUF_Name            = theRptFilter.FRUF_Name;
      string FRUF_Value           = theRptFilter.FRUF_Value;
      bool   FRUF_biloGdjeUtekstu = theRptFilter.FRUF_BiloGdjeU;

    //if(FRUF_Value.NotEmpty())
      if(FRUF_Name .NotEmpty())
      {
         bool   goodToGo      = true ;
         string FRUF_niceName = ""   ;
         string operand       = " = ";
         switch(FRUF_Name)
         {
            case "Napom"    : drSchema = ZXC.FakturSchemaRows[ZXC.FakCI.napomena]; FRUF_niceName = "Napomenu: "     ; break;
            case "Veza1"    : drSchema = ZXC.FakturSchemaRows[ZXC.FakCI.v1_tt   ]; FRUF_niceName = "TT Veze1: "     ; break;
            case "Veza2"    : drSchema = ZXC.FakturSchemaRows[ZXC.FakCI.v2_tt   ]; FRUF_niceName = "TT Veze2: "     ; break;
            case "Veza3"    : drSchema = ZXC.FaktExSchemaRows[ZXC.FexCI.v3_tt   ]; FRUF_niceName = "TT Veze3: "     ; break;
            case "Veza4"    : drSchema = ZXC.FaktExSchemaRows[ZXC.FexCI.v4_tt   ]; FRUF_niceName = "TT Veze4: "     ; break;
            case "OrigBrDok": drSchema = ZXC.FakturSchemaRows[ZXC.FakCI.vezniDok]; FRUF_niceName = "Orig. br. dok: "; break;
            case "Opis"     : drSchema = ZXC.FakturSchemaRows[ZXC.FakCI.opis    ]; FRUF_niceName = "Opis: "         ; break;

            default         : goodToGo = false; ZXC.aim_emsg("Nepoznati FRUF Name: " + FRUF_Name)                   ; break; // Unknown FRUF_Name 
         }

         if(goodToGo)
         {
            if(FRUF_biloGdjeUtekstu)
            {
               operand    = " LIKE "              ;
               FRUF_Value = "%" + FRUF_Value + "%";
            }

            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, FRUF_Name, FRUF_Value, FRUF_Value, "Za " + FRUF_niceName, operand, ""));
         }
      }


      #endregion FRUF

      //ABRAKADABRA if(fakturListUc != null) fakturListUc.AddFilterMemberz();
   }

   private void AddFilterMemberzFor_OTS_FilterStyle(VvRpt_RiSk_Filter theRptFilter, VvRiskReport theVvRiskReport)
   {
      DateTime dateOD = theRptFilter.DatumOd;
      DateTime dateDO = theRptFilter.DatumDo.EndOfDay();
      string comparer;

      if(dateOD == dateDO) comparer = "  = ";
      else                 comparer = " >= ";

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FtrSch[FtrCI.t_dokDate], false, "DateOD", dateOD, dateOD.ToString("dd.MM.yyyy."), "Od datuma:", comparer, ""));
      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FtrSch[FtrCI.t_dokDate], false, "DateDO", dateDO, dateDO.ToString("dd.MM.yyyy."), "Do datuma:", " <= "  , ""));

      // Fld_KupdobCD                                                                                                                                     

      if(theRptFilter.KD_sifra.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FtrSch[FtrCI.t_kupdob_cd], false, "kupdobCD", theRptFilter.KD_sifra, theRptFilter.KD_naziv, "Za partnera:", " = ", ""));
      }

      // 05.08.2015: Fld_kupdobTip KUPDOB JOIN!                                                                                                                                     

      if(theRptFilter.FuseStr3.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(KupSch[KupCI.tip], false, "kupdobTip", theRptFilter.FuseStr3, theRptFilter.FuseStr3, "Za TIP partnera:", " = ", ""));
      }

      // Fld_Putnik_PersonCD                                                                                                                                     

      if(theRptFilter.Putnik_PersonCD.NotZero())
      {
         // 20.01.2014: Nezatvoreni racuni is IPOSa na nalogu za knj. nemaju podatak po komerc. pa do dalnjega taj podatak vadimo iz Kupdoba a ne fakture
         // kada se zatvore svi rac. iz PG. onda cemo vratiti na faktur.personCD jer ovo ce biti krivo ako u toku godine partner predje na drugog komerc. 
       //theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.personCD], false, "personCD", theRptFilter.Putnik_PersonCD, theRptFilter.Putnik_PersonName, "Za Komercijalistu:", " = ", ""));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(KupSch  [KupCI  .putnikID], false, "personCD", theRptFilter.Putnik_PersonCD, theRptFilter.Putnik_PersonName, "Za Komercijalistu:", " = ", ""));
      }

      //  Fld_nacPlac                                                                                                                                                                           

      if(theRptFilter.NacinPlacanja.NotEmpty()) 
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.nacPlac], false, "NacinPlac", theRptFilter.NacinPlacanja, theRptFilter.NacinPlacanja, "Za način plaćanja:", " = ", "")); 
      }
      // Fld_SkladCD                                                                                                                                                   

      if(theRptFilter.SkladCD.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.skladCD], false, "SkladCD", theRptFilter.SkladCD, theRptFilter.SkladCD, "Za skladište:", " = ", ""));
      }
      // Fld_Projekt                                                                                                                                         

      if(theRptFilter.ProjektCD.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.projektCD], false, "ProjektCD", theRptFilter.ProjektCD, theRptFilter.ProjektCD, "Za projekt:", " = ", ""));
      }
      //  Fld_pdvR12tip                                                                                                                                                                           

      if(theRptFilter.PdvR12 != ZXC.PdvR12Enum.NIJEDNO)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.pdvR12], false, "PdvR12tip", theRptFilter.PdvR12, theRptFilter.PdvR12.ToString(), "Za PDV tip računa:", " = ", ""));
      }

#region 06.11.2014: proba filtriranja Kamato po jednom racunu
      // Fld_TTnumOdDo                                                                                                                                     
      bool isDummy;
      uint numOD, numDO, num;
      DataRow drSchema;

      drSchema = ZXC.FakturSchemaRows[ZXC.FakCI.ttNum];
      numOD = theRptFilter.TtNumOd;
      numDO = theRptFilter.TtNumDo;

      if(numOD.NotZero())
      {
         if(numOD == numDO) comparer = " = ";
         else comparer = " >= ";

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNumOD", numOD, numOD.ToString(""), "Od TT broj:", comparer, ""/*, Rtrans.recordName*/));
      }
      else if(numDO.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "TtNumOD", numOD, "-", "Od TT broj:", " >= ", ""/*, Rtrans.recordName*/));
      }

      if(numDO.NotZero())
      {
         if(numDO == numOD) isDummy = true;
         else isDummy = false;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "TtNumDO", numDO, numDO.ToString(""), "Do TT broj:", " <= ", ""/*, Rtrans.recordName*/));
      }
      else if(numOD.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "TtNumDO", numDO, "-", "Do TT broj:", " <= ", ""/*, Rtrans.recordName*/));
      }

      #endregion 06.11.2014: proba filtriranja Kamato po jednom racunu

   }

   private void AddFilterMemberzFor_KPM_FilterStyle(VvRpt_RiSk_Filter theRptFilter, VvRiskReport theVvRiskReport)
   {
      DateTime dateOD = theRptFilter.DatumOd;
      DateTime dateDO = theRptFilter.DatumDo.EndOfDay();

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.dokDate], false, "DateOD", dateOD, dateOD.ToString("dd.MM.yyyy."), "Od datuma:", " >= ", ""));
      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.dokDate], false, "DateDO", dateDO, dateDO.ToString("dd.MM.yyyy."), "Do datuma:", " <= ", ""));

      // Fld_SkladCD                                                                                                                                                   

      if(theRptFilter.SkladCD.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.skladCD ], ZXC.FM_OR_Enum.OPEN_OR , false, "SkladCD",  theRptFilter.SkladCD, theRptFilter.SkladCD, "Za skladište:", " = ", ""));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.skladCD2], ZXC.FM_OR_Enum.CLOSE_OR, false, "SkladCD2", theRptFilter.SkladCD, ""                  , ""             , " = ", ""));
      }
      //else throw new Exception("Skladiste ne moze bit prazno.");

   }

   private void AddFilterMemberzFor_Pdv_FilterStyle(VvRpt_RiSk_Filter theRptFilter, VvRiskReport theVvRiskReport)
   {
      string IN_clause = ((RptR_PDV)theVvRiskReport).isURA ? TtInfo.UlazniPdv_IN_Clause : TtInfo.IzlazniPdv_IN_Clause;

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", IN_clause, " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!

      DateTime dateOD = theRptFilter.DatumOd;
      DateTime dateDO = theRptFilter.DatumDo.EndOfDay();

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.pdvDate], ZXC.FM_OR_Enum.OPEN_OR , false, "PdvFakDateOD", dateOD, dateOD.ToString("dd.MM.yyyy."), "Od datuma:", " >= ", ""));
      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FtrSch  [FtrCI.t_dokDate], ZXC.FM_OR_Enum.CLOSE_OR, false, "PdvFtrDateOD", dateOD, ""                            , ""          , " >= ", ""));

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.pdvDate], ZXC.FM_OR_Enum.OPEN_OR , false, "PdvFakDateDO", dateDO, dateDO.ToString("dd.MM.yyyy."), "Do datuma:", " <= ", ""));
      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FtrSch  [FtrCI.t_dokDate], ZXC.FM_OR_Enum.CLOSE_OR, false, "PdvFtrDateDO", dateDO, ""                            , ""          , " <= ", ""));

      if(theRptFilter.MT_sifra.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.mtrosCD], false, "MtrosCD", theRptFilter.MT_sifra, theRptFilter.MT_sifra.ToString(), "Za mj. troška:", " = ", ""));
      }

      if(theVvRiskReport is RptR_PDV_Knjiga)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.pdvKnjiga], false, "PdvKnjiga", theRptFilter.PdvKnjiga, theRptFilter.PdvKnjiga.ToString(), "Pdv Knjiga:", " = ", ""));

         if(theRptFilter.PdvKnjiga == ZXC.PdvKnjigaEnum.NIJEDNA)
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.pdvGEOkind], false, "PdvGeoKind", theRptFilter.PdvGeoKind, theRptFilter.PdvGeoKind.ToString(), "PdvGEO:", " = ", ""));
         }
      }

      //if(theVvRiskReport is RptR_PDV_PPO)
      //{
      //   theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.s_ukOsn07], "s_ukOsn07", 0M, " != "));
      //}

      //  Fld_Status                                                                                                                                                                          
      
      string text = theRptFilter.FuseStr1;
      if(theRptFilter.FuseStr1.NotEmpty()) { theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.statusCD], false, "Status", text, text, "Za status:", " REGEXP ", "")); }

   }

   private void AddFilterMemberzFor_GEOPdv_FilterStyle(VvRpt_RiSk_Filter theRptFilter, VvRiskReport theVvRiskReport)
   {
      string IN_clause = (((RptR_EU_PdvGEOkind)theVvRiskReport).IsZP ? TtInfo.IzlazniPdv_IN_Clause.Replace(", '" + Faktur.TT_UPA + "'", "") : TtInfo.UlazniPdv_IN_Clause)/*.Replace(", '" + Faktur.TT_UPA + "'", "")*/;

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", IN_clause, " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!

      DateTime dateOD = theRptFilter.DatumOd;
      DateTime dateDO = theRptFilter.DatumDo.EndOfDay();

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.pdvDate], false, "PdvFakDateOD", dateOD, dateOD.ToString("dd.MM.yyyy."), "Od datuma:", " >= ", ""));
      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.pdvDate], false, "PdvFakDateDO", dateDO, dateDO.ToString("dd.MM.yyyy."), "Do datuma:", " <= ", ""));

   }

   private void AddFilterMemberzFor_Artikl_FilterStyle(VvRpt_RiSk_Filter theRptFilter, VvRiskReport theVvRiskReport)
   {
      #region Obligatory SkladCD & DateDo filterMemberz as dummies, just for print

      // 27.08.2012: start 

    //bool isPrometRazdoblja = theVvRiskReport.VirtualReportDocument is Vektor.Reports.RIZ.CR_PrometRazdoblja;
      bool isPrometRazdoblja = theVvRiskReport.VirtualReportDocument is Vektor.Reports.RIZ.CR_PrometRazdoblja ||
                               theVvRiskReport.VirtualReportDocument is Vektor.Reports.RIZ.CR_PrometRazdoblja_OP;

      bool isKretanjeSklad   = theVvRiskReport.VirtualReportDocument is Vektor.Reports.RIZ.CR_KretanjeSklad  ;
      DateTime samtajmDummy_samtajmNOTdummy_DateOd = ZXC.projectYearFirstDay;
      bool isDummyDateOD;

      bool dateOD_IsCorrected = false;
      if(theRptFilter.DatumOd != ZXC.projectYearFirstDay && isPrometRazdoblja == false /*&& isKretanjeSklad == false*/)
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Stanje Skladišta nema smisla tražiti za razdobljeOD koje nije 01.01. tekuće godine.\n\nRazdobljeOD postavljam na 01.01.tekuće godine!\n\nUkoliko želite analizirati promete za neko razdoblje, odaberite izvještaj 'Promet Razdoblja'.");
         // 13.07.2022: tek sad? 
         dateOD_IsCorrected = true;
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.t_skladDate], false, "DateOD", ZXC.projectYearFirstDay, ZXC.projectYearFirstDay.ToString("dd.MM.yyyy."), "Od datuma:", " >= ", "", "A"));
      }

      // 16.09.2016: 
      //if(isPrometRazdoblja/* || isKretanjeSklad*/                           )
      if(isPrometRazdoblja/* || isKretanjeSklad*/ || theRptFilter.SviArtikli)
      {
         samtajmDummy_samtajmNOTdummy_DateOd = theRptFilter.DatumOd;
         isDummyDateOD = true;
      }
      else
      {
         samtajmDummy_samtajmNOTdummy_DateOd = ZXC.projectYearFirstDay;
         isDummyDateOD = false;
      }
      // 27.08.2012: end 

      // 29.11.2012: stavijo da theRptFilter.DatumOd ne bude dummy 
      // 17.01.2013: dodatno stavijo da u slucaju isPrometRazdoblja ipak bude dummy 
      //theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.t_skladDate], true , "DateOD", dummyDateOdJustForPrint, dummyDateOdJustForPrint.ToString("dd.MM.yyyy."), "Od datuma:", ""    , ""));

      // 13.07.2022: dodan if() prije dateOD 
      if(dateOD_IsCorrected == false)
      {
      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.t_skladDate], isDummyDateOD, "DateOD", samtajmDummy_samtajmNOTdummy_DateOd, samtajmDummy_samtajmNOTdummy_DateOd.ToString("dd.MM.yyyy."), "Od datuma:", " >= ", "", "A"));
      }
      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.t_skladDate], true         , "DateDO", theRptFilter.DatumDo.EndOfDay()    , theRptFilter.DatumDo.ToString("dd.MM.yyyy.")               , "Do datuma:",     "", ""));

      if(theRptFilter.SkladCD.NotEmpty())
      {
         string cijenaID = "NBC";
         if(theRptFilter.FuseBool1 == false && // ako je false znaci NE prislijavamo NBC na eventualni MPSK 
            ZXC.luiListaSkladista.GetFlagForThisCd(theRptFilter.SkladCD) == true)
         {
            cijenaID = "MPC";
         }
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.t_skladCD], true, "SkladCD", theRptFilter.SkladCD, theRptFilter.SkladCD + " po " + cijenaID, "Za skladište:", " = ", ""));
      }

      #endregion Obligatory SkladCD & DateDo filterMemberz as dummies, just for print

      #region Artikl od - do

      if(theRptFilter.ArtNameOD .NotEmpty()) theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArtSch[ArtCI.artiklName], false, "ArtiklNameOD", theRptFilter.ArtNameOD , theRptFilter.ArtNameOD , "Od naziva artikla:", " >= ", ""));
      if(theRptFilter.ArtNameDO .NotEmpty()) theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArtSch[ArtCI.artiklName], false, "ArtiklNameDO", theRptFilter.ArtNameDO , theRptFilter.ArtNameDO , "Do naziva artikla:", " <= ", ""));
      if(theRptFilter.ArtiklCdOD.NotEmpty()) theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArtSch[ArtCI.artiklCD  ], false, "ArtiklCdOD"  , theRptFilter.ArtiklCdOD, theRptFilter.ArtiklCdOD, "Od šifre artikla:" , " >= ", ""));
      if(theRptFilter.ArtiklCdDO.NotEmpty()) theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArtSch[ArtCI.artiklCD  ], false, "ArtiklCdDO"  , theRptFilter.ArtiklCdDO, theRptFilter.ArtiklCdDO, "Do šifre artikla:" , " <= ", ""));

      // 12.04.2018: 
      if(theRptFilter.Artikl_TS .NotEmpty()) theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArtSch[ArtCI.ts        ], false, "Artikl_TS"   , theRptFilter.Artikl_TS  , theRptFilter.Artikl_TS  + " " + ZXC.luiListaArtiklTS     .GetNameForThisCd(theRptFilter.Artikl_TS ), "Za tip artikla:", " = ", ""));
      if(theRptFilter.Artikl_Gr1.NotEmpty()) theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArtSch[ArtCI.grupa1CD  ], false, "Artikl_Gr1"  , theRptFilter.Artikl_Gr1 , theRptFilter.Artikl_Gr1 + " " + ZXC.luiListaGrupa1Artikla.GetNameForThisCd(theRptFilter.Artikl_Gr1), "Za artikl GrA:" , " = ", ""));
      //17.04.2018.
      if(theRptFilter.Artikl_Gr2.NotEmpty()) theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArtSch[ArtCI.grupa2CD  ], false, "Artikl_Gr2"  , theRptFilter.Artikl_Gr2 , theRptFilter.Artikl_Gr2 + " " + ZXC.luiListaGrupa2Artikla.GetNameForThisCd(theRptFilter.Artikl_Gr2), "Za artikl GrB:" , " = ", ""));
      if(theRptFilter.Artikl_Gr3.NotEmpty()) theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArtSch[ArtCI.grupa3CD  ], false, "Artikl_Gr3"  , theRptFilter.Artikl_Gr3 , theRptFilter.Artikl_Gr3 + " " + ZXC.luiListaGrupa3Artikla.GetNameForThisCd(theRptFilter.Artikl_Gr3), "Za artikl GrC:" , " = ", ""));

      #endregion ArtiklCD od - do

      if(theVvRiskReport.VirtualReportDocument is Vektor.Reports.RIZ.CR_LagerLista    ||
         theVvRiskReport.VirtualReportDocument is Vektor.Reports.RIZ.CR_LagerLista_OP ||

        (ZXC.vvDB_VvDomena == "vvMN" && 
         theVvRiskReport.VirtualReportDocument is Vektor.Reports.RIZ.CR_LagreEtikete) ||

         theVvRiskReport.VirtualReportDocument is Vektor.Reports.RIZ.CR_LagerListaPodJM_SvaSkl)
      {
         // 09.12.2015: 
         if(theRptFilter.IsChk0 == false) // normal case 
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArtiklDao.ResCol_StanjeKol, 0M, " <> ", ""));
         }
         else // provjeravamo kada je KolSt nula vs FinSt 
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(         ArtiklDao.ResCol_StanjeKol         , 0.000M, " <> ", ZXC.FM_OR_Enum.OPEN_OR ));
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember("ABS(" + ArtiklDao.ResCol_StanjeFinNBC + ")", 0.015M, "  > ", ZXC.FM_OR_Enum.NONE    ));

            // fuka te ovdje sto ponekad iako je VPSK u FinStMPC ima stanje pa izlazi kao greska a nije
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember("ABS(" + ArtiklDao.ResCol_StanjeFinMPC + ")", 0.015M, "  > ", ZXC.FM_OR_Enum.CLOSE_OR));
         }
      } // if LagerLista 

      // 16.10.2014: 
      else if(theRptFilter.FuseBool4 == true) // prikazi samo artikle sa stanjem 
      {
         // 09.12.2015: 
         if(theRptFilter.IsChk0 == false) // normal case 
         {
            if(theVvRiskReport.VirtualReportDocument is Vektor.Reports.RIZ.CR_InventurneRazlike == false) // clasic, normal case 
            {
               theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArtiklDao.ResCol_StanjeKol, "Samo artikli sa stanjem", 0.00M, "", "Samo artikli sa stanjem", " <> "));
            }
            else // YES, this IS Inventura! 
            {
               theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArtiklDao.ResCol_StanjeKol, 0.000M, " <> ", ZXC.FM_OR_Enum.OPEN_OR ));
               theRptFilter.FilterMembers.Add(new VvSqlFilterMember("invKolDiff"              , 0.000M, " <> ", ZXC.FM_OR_Enum.CLOSE_OR));
            }
         }
         else // provjeravamo kada je KolSt nula vs FinSt 
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(         ArtiklDao.ResCol_StanjeKol         , 0.000M, " <> ", ZXC.FM_OR_Enum.OPEN_OR ));
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember("ABS(" + ArtiklDao.ResCol_StanjeFinNBC + ")", 0.015M, "  > ", ZXC.FM_OR_Enum.NONE    ));

            // fuka te ovdje sto ponekad iako je VPSK u FinStMPC ima stanje pa izlazi kao greska a nije
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember("ABS(" + ArtiklDao.ResCol_StanjeFinMPC + ")", 0.015M, "  > ", ZXC.FM_OR_Enum.CLOSE_OR));
         }
      } // else if(theRptFilter.FuseBool4 == true) // prikazi samo artikle sa stanjem  

      if(theRptFilter.FuseBool2 == true) // prikazi samo artikle bez izlaza 
      {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ArtiklDao.ResCol_IzlazKol, "Samo artikli BEZ izlaza", 0.00M, "", "Samo artikli BEZ izlaza", " = "));
      } // if(theRptFilter.FuseBool2 == true) // prikazi samo artikle bez izlaza 

      //ABRAKADABRA if(artiklListUc != null) artiklListUc.AddFilterMemberz();
   }

   private void AddFilterMemberzFor_BLAG_FilterStyle(VvRpt_RiSk_Filter theRptFilter, VvRiskReport theVvRiskReport)
   {
      DateTime dateOD = theRptFilter.DatumOd;
      DateTime dateDO = theRptFilter.DatumDo.EndOfDay();

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.dokDate], false, "DateOD", dateOD, dateOD.ToString("dd.MM.yyyy."), "Od datuma:", " >= ", ""));
      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.dokDate], false, "DateDO", dateDO, dateDO.ToString("dd.MM.yyyy."), "Do datuma:", " <= ", ""));

      string IN_clause = TtInfo.Blagajna_IN_Clause;

    //theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", "('UPL', 'ISP')", " IN "));
      theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", IN_clause, " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!

      if(theRptFilter.MT_sifra.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.mtrosCD], false, "MtrosCD", theRptFilter.MT_sifra, theRptFilter.MT_sifra.ToString(), "Za mj. troška:", " = ", ""));
      }

      // Fld_SkladCD                                                                                                                                                   

      if(theRptFilter.SkladCD.NotEmpty())
      {
         string text = theRptFilter.SkladCD + " " + ZXC.luiListaSkladista.GetNameForThisCd(theRptFilter.SkladCD);
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.skladCD], false, "SkladCD", theRptFilter.SkladCD, text/*theRptFilter.SkladCD*/, "Za blagajnu (skl):", " = ", ""));
      }

   }

   #endregion AddFilterMemberz()

   #region Macro

   private static bool IsRiskMacroListInitialized = false;

   private VvRiskMacro SelectedMacro
   {
      get
      {
         return (VvRiskMacro)combbx_macro.SelectedItem;
      }
   }

   public void LoadVvRiskMacroList()
   {
      if(ZXC.VvRiskMacroList == null) ZXC.VvRiskMacroList = new List<VvRiskMacro>();
      else ZXC.VvRiskMacroList.Clear();

      VvDaoBase.LoadVvRiskMacroList(ZXC.VvRiskMacroList);
   }

   public void InitializeVvRiskMacroList_ComboBox()
   {
      if(ZXC.VvRiskMacroList == null || ZXC.VvRiskMacroList.Count.IsZero()) return;

      combbx_macro.Items.Clear();

      foreach(VvRiskMacro riskMacro in ZXC.VvRiskMacroList)
      {
         combbx_macro.Items.Add(riskMacro);
         //         tsCbx_MacroRiskReport.Items.Add(riskMacro);
      }

   }

   public void MacroNew_Click(object sender, EventArgs e)
   {
      #region Dialog OvoOno

      RiskMacroNewDlg dlg = new RiskMacroNewDlg(TheRtransFilter.DatumOd, TheRtransFilter.DatumDo.EndOfDay());

      dlg.Fld_MacroName = TheVvUC.TheVvTabPage.TheVvForm.tsCbxReport.SelectedItem.ToString() + " - ";

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      dlg.Dispose();

      if(dlg.Fld_MacroName.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ime MACRO-a ne smije biti prazno.");
         return;
      }

      int selectedReportIndex = TheVvUC.TheVvTabPage.TheVvForm.tsCbxReport.SelectedIndex;
      string shortReportName  = TheVvUC.TheVvTabPage.TheVvForm.tsCbxReport.SelectedItem.ToString();

      if(selectedReportIndex.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nije odabran nijedan izvještaj.");
         return;
      }

      if(VvDaoBase.DoesRiskMacroExists(ZXC.PrjConnection, dlg.Fld_MacroName))
      {
         //DialogResult result = MessageBox.Show("Da li zaista zelite \'pregaziti\' postojeći macro <" + dlg.Fld_MacroName + ">", "Potvrdite PREPISIVANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
         //if(result != DialogResult.Yes) return;
         ZXC.aim_emsg(MessageBoxIcon.Error, "Macro <{0}> već postoji. Odaberite neki drugi naziv.", dlg.Fld_MacroName);
         return;
      }

      #endregion Dialog OvoOno

      GetFilterFields();

      VvRiskMacro riskMacro = new VvRiskMacro(dlg.Fld_MacroName, shortReportName, selectedReportIndex, dlg.Fld_ZapamtiDatume, TheRtransFilter);

      bool OK = VvDaoBase.InsertInRiskMacro(riskMacro);

      if(OK)
      {
         ZXC.VvRiskMacroList.Add(riskMacro);

         combbx_macro.Items.Add(riskMacro);
      }

   }

   public void MacroDel_Click(object sender, EventArgs e)
   {
      if(SelectedMacro == null)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nije odabran nijedan macro.");
         return;
      }

      DialogResult result = MessageBox.Show("Da li zaista zelite OBRISATI macro <" + SelectedMacro + ">", "Potvrdite BRISANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if(result != DialogResult.Yes) return;

      bool OK = VvDaoBase.DeleteFromRiskMacro(ZXC.PrjConnection, SelectedMacro);

      if(OK)
      {
         ZXC.VvRiskMacroList.Remove(SelectedMacro);

         combbx_macro.Items.Remove(SelectedMacro);
      }

   }

   private void PutMacroFields(VvRiskMacro riskMacro)
   {
      if(riskMacro == null) return;

      if(riskMacro.UseMacroDates == false)
      {
         riskMacro.RptFilter.DatumOd = Fld_DatumOd;
         riskMacro.RptFilter.DatumDo = Fld_DatumDo.EndOfDay();
      }

      riskMacro.RptFilter.MacroName = riskMacro.MacroName;

      PutFilterFields(riskMacro.RptFilter);

      //TheVvUC.TheVvTabPage.TheVvForm.tsCbxReport.SelectedItem = theMacro;
      TheVvUC.TheVvTabPage.TheVvForm.tsCbxReport.SelectedIndex = riskMacro.ReportZ;
      // 25.11.2012: 
      TheVvUC.TheVvTabPage.TheVvForm.tsCbxReport.SelectedItem = riskMacro.RptFilter.FuseStr2;
   }

   private void Combbx_macro_SelectedIndexChanged(object sender, EventArgs e)
   {
      if(ZXC.PutRiskFilterFieldsInProgress == false) MacroGO_Click("indexChanged", e);
   }

   public void MacroGO_Click(object sender, EventArgs e)
   {
      if(SelectedMacro == null)
      {
         if(sender.ToString() != "indexChanged") ZXC.aim_emsg(MessageBoxIcon.Error, "Nije odabran nijedan macro.");
         return;
      }

      PutMacroFields(SelectedMacro);

      btn_GO.PerformClick();
   }

   public void MacroEdit_Click(object sender, EventArgs e)
   {
      if(SelectedMacro == null)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nije odabran nijedan macro.");
         return;
      }

      #region Dialog OvoOno

      RiskMacroNewDlg dlg = new RiskMacroNewDlg(TheRtransFilter.DatumOd, TheRtransFilter.DatumDo.EndOfDay());

      dlg.Fld_MacroName = SelectedMacro.MacroName;

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      dlg.Dispose();

      if(dlg.Fld_MacroName.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ime MACRO-a ne smije biti prazno.");
         return;
      }

      int selectedReportIndex = TheVvUC.TheVvTabPage.TheVvForm.tsCbxReport.SelectedIndex;
      string shortReportName  = TheVvUC.TheVvTabPage.TheVvForm.tsCbxReport.SelectedItem.ToString();

      if(selectedReportIndex.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nije odabran nijedan izvještaj.");
         return;
      }

      if(dlg.Fld_MacroName != SelectedMacro.MacroName && VvDaoBase.DoesRiskMacroExists(ZXC.PrjConnection, dlg.Fld_MacroName))
      {
         //DialogResult result = MessageBox.Show("Da li zaista zelite \'pregaziti\' postojeći macro <" + dlg.Fld_MacroName + ">", "Potvrdite PREPISIVANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
         //if(result != DialogResult.Yes) return;
         ZXC.aim_emsg(MessageBoxIcon.Error, "Macro <{0}> već postoji. Odaberite neki drugi naziv.", dlg.Fld_MacroName);
         return;
      }

      #endregion Dialog OvoOno

      #region Delete Old Macro

      bool OK = VvDaoBase.DeleteFromRiskMacro(ZXC.PrjConnection, SelectedMacro);

      if(OK)
      {
         ZXC.VvRiskMacroList.Remove(SelectedMacro);

         combbx_macro.Items.Remove(SelectedMacro);
      }

      #endregion Delete Old Macro

      GetFilterFields();

      VvRiskMacro riskMacro = new VvRiskMacro(dlg.Fld_MacroName, shortReportName, selectedReportIndex, dlg.Fld_ZapamtiDatume, TheRtransFilter);

      OK = VvDaoBase.InsertInRiskMacro(riskMacro);

      if(OK)
      {
         ZXC.VvRiskMacroList.Add(riskMacro);

         combbx_macro.Items.Add(riskMacro);

         this.Fld_MacroName = riskMacro.MacroName;
      }
   }

   #endregion Macro

   #region SetFilterSet_OnRiskFilterUC

   public void SetFilterSet_OnRiskFilterUC(VvForm.VvReportSubModul theReportSubModul)
   {
      //string currentFilterSetTabPageTitle = tabControlRiskFilterUC.SelectedTab.Title;
      string filterSetName = "";

      switch(theReportSubModul.groupNum)
      {
         case 1: filterSetName = FilterSetREALIZ  ; break;
         case 2: filterSetName = FilterSetREALIZ_S; break;
         case 3: case 4: filterSetName = FilterSetIOS    ; break;
         case 5: case 6: if(theReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_PrometArtikla      || 
                            theReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_PrometArtikla_S    ||
                            theReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_PrometArtikla_OP_A ||
                            theReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_PrometArtikla_OP_S    )
                         filterSetName = FilterSetPRM_ART;
                         else
                         filterSetName = FilterSetSKLAD  ; break;
         case 7: case 8: filterSetName = FilterSetPDV    ; break;
      }

      SetHampersVisibiliti(filterSetName);
     // if(currentFilterSetTabPageTitle != filterSetName) tabControlRiskFilterUC.TabPages[filterSetName].Selected = true;
   }

   public void SetHampersVisibiliti(string filterSetName)
   {
      switch(filterSetName)
      {
         case FilterSetREALIZ  : CalcLocationHamper_REALIZ(false); break;
         case FilterSetREALIZ_S: CalcLocationHamper_REALIZ(true ); break;
         case FilterSetIOS     : CalcLocationHamper_IOS        ();break;
         case FilterSetPDV     : CalcLocationHamper_PDV        ();break;
         case FilterSetSKLAD   : CalcLocationHamper_SKLAD      ();break;
         case FilterSetPRM_ART : CalcLocationHamper_PRM_ART    ();break; 
      }

      ((RiskReportUC)TheVvUC).CalcTheFilterPanelWidth();
   }

   #endregion SetFilterSet_OnRiskFilterUC

   public void CompDateOdExitMethod(object sender, EventArgs e)
   {
      if(hamper_compDate.Visible == false) return;

      DateTime mainDateOd = Fld_DatumOd   ;
    //DateTime mainDateDo = new DateTime(Fld_DatumDo.Year, Fld_DatumDo.Month, Fld_DatumDo.Day); // zbog sati i minuta koje stvori DateTime.Now 
      DateTime mainDateDo = Fld_DatumDo.EndOfDay();
      DateTime compDateOd = Fld_CompDateOd;
      DateTime compDateDo;

      if(compDateOd.IsEmpty()) return;

      bool isWholeMonths;

      if(mainDateOd.Day == 1 && mainDateDo.Day == DateTime.DaysInMonth(mainDateDo.Year, mainDateDo.Month)) // prvi i zadnji dan u mjesecu 
         isWholeMonths = true;
      else
         isWholeMonths = false;

      if(isWholeMonths)
      {
         if(mainDateDo.Month < mainDateOd.Month) return;

         int diffMonth = mainDateDo.Month - mainDateOd.Month;
         int diffYear  = mainDateDo.Year  - mainDateOd.Year ;
         int compDoMonth, compDoYear;

         if(compDateOd.Month + diffMonth <= 12)
         {
            compDoMonth = compDateOd.Month + diffMonth;
            compDoYear  = compDateOd.Year;
         }
         else
         {
            compDoMonth = compDateOd.Month + diffMonth - 12;
            compDoYear  = compDateOd.Year  + diffYear ;
         }

         compDateDo = new DateTime(compDoYear, compDoMonth, DateTime.DaysInMonth(compDoYear, compDoMonth)).EndOfDay();
      }
      else
      {
         if(mainDateOd.Day   == compDateOd.Day &&
            mainDateOd.Month == compDateOd.Month)
         {
            compDateDo = new DateTime(compDateOd.Year, mainDateDo.Month, mainDateDo.Day).EndOfDay();
         }
         else
         {
            TimeSpan ts = mainDateDo - mainDateOd;

            compDateDo = compDateOd + ts;
         }
      }

      Fld_CompDateDo = compDateDo.EndOfDay();
   }
}

#endregion TAB_RiskFilterUC

public class ArtiklSifrarFilterUC : VvFilterUC
{
   #region Fieldz

   private VvHamper hamp_OdDo, hamp_robKart, hamp_tt, hamp_sklad, hamp_partner, hamp_napom;

   private VvTextBox tbx_zaSkladCD, tbx_skladOpis,
                     tbxLookUp_TT, tbx_TTopis, 
                     tbx_ttNumOd , tbx_ttNumDo,
                     tbx_dokNumOd, tbx_dokNumDo, 
                     tbx_kupDobCD, tbx_kupDobTiker, tbx_kupDobNaziv,
                     tbx_grupaPart, tbx_vezniDok, tbx_nacinPl, tbx_napomena;

   private RadioButton rbt_artikL, rbt_artikP, rbt_robKartA, rbt_robKartAP, rbt_robKartB, rbt_robKartKol, rbt_transakc, rbt_rbKrtKolSerlot, rbt_robKartAMB;

   private CheckBox cbx_biloGdjeUnapomeni, cbx_grupaPoStr, cbx_MPSkpoNBC;
   public  CheckBox cbx_isRobKartica;
   private ArtiklUC theArtiklUC;

   #endregion Fieldz

   #region Constructor

   public ArtiklSifrarFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();

      TheVvUC = vvUC;

      theArtiklUC = (ArtiklUC)vvUC;
     
      razmakIzmjedjuHampera = ZXC.Qun8;

      InitializeHamper_RobKart(out hamp_robKart);
      nextY = hamp_robKart.Bottom + razmakIzmjedjuHampera;

      nextY = hamp_robKart.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_sklad(out hamp_sklad);
      
      nextY = hamp_sklad.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_OdDo(out hamp_OdDo);

      MaxHamperWidth = hamp_OdDo.Width;
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamp_OdDo, MaxHamperWidth, razmakIzmjedjuHampera);

      CreateHamper_4ButtonsResetGo_Width(MaxHamperWidth);

      

      nextY = hamp_OdDo.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_tt(out hamp_tt);

      nextY = hamp_tt.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_partner(out hamp_partner);

      nextY = hamp_partner.Bottom + razmakIzmjedjuHampera;

      InitializeHamper_Napom(out hamp_napom);
      nextY = hamp_napom.Bottom + razmakIzmjedjuHampera;

      nextY = LocationOfHamper_HorLine(nextX, nextY, MaxHamperWidth) + razmakIzmjedjuHampera;

     // InitializeHamper_Card(out hamp_card);

      CreatehapmPrintDoc();

      nextY = LocationOfHamper_PrintDocument(nextX, nextY, MaxHamperWidth) + razmakIzmjedjuHampera;

      this.Size = new Size(MaxHamperWidth + 4 * razmakIzmjedjuHampera, nextY + ZXC.QunMrgn);

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      SetZaUpisOtvorenoZatvoreno(ZXC.ZaUpis.Zatvoreno);
      EnableDisableRbt(true);

      this.ResumeLayout();

   }


   #endregion Constructor

   #region Hamper_s
   
   private void CreatehapmPrintDoc()
   {
      hamperPrintDoc = new VvHamper(1, 10, "", this, false);

      hamperPrintDoc.VvColWdt      = new int[] { ZXC.Q10un + ZXC.QUN };
      hamperPrintDoc.VvSpcBefCol   = new int[] { ZXC.Qun2 };
      hamperPrintDoc.VvRightMargin = hamperPrintDoc.VvLeftMargin;

      for(int i = 0; i < hamperPrintDoc.VvNumOfRows; i++)
      {
         hamperPrintDoc.VvRowHgt[i] = ZXC.QUN;
         hamperPrintDoc.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamperPrintDoc.VvBottomMargin = hamperPrintDoc.VvTopMargin;

      hamperPrintDoc.CreateVvLabel(0, 0, "Ispis Dokumenta:", ContentAlignment.MiddleLeft);

      rbt_robKartA       = hamperPrintDoc.CreateVvRadioButton(0, 1, null, "Robna kartica A (Landscape)"    , TextImageRelation.ImageBeforeText);
      rbt_robKartAP      = hamperPrintDoc.CreateVvRadioButton(0, 2, null, "Robna kartica A (Portrait)"     , TextImageRelation.ImageBeforeText);
      rbt_robKartB       = hamperPrintDoc.CreateVvRadioButton(0, 3, null, "Robna kartica B"                , TextImageRelation.ImageBeforeText);
      rbt_robKartAMB     = hamperPrintDoc.CreateVvRadioButton(0, 4, null, "Robna kartica AMB"              , TextImageRelation.ImageBeforeText);
      rbt_robKartKol     = hamperPrintDoc.CreateVvRadioButton(0, 5, null, "Robna kartica Količinska"       , TextImageRelation.ImageBeforeText);
      rbt_rbKrtKolSerlot = hamperPrintDoc.CreateVvRadioButton(0, 6, null, "Robna kartica Količinska Serlot", TextImageRelation.ImageBeforeText);
      rbt_transakc       = hamperPrintDoc.CreateVvRadioButton(0, 7, null, "Rekapitulacija transakcija"     , TextImageRelation.ImageBeforeText);
      rbt_artikL         = hamperPrintDoc.CreateVvRadioButton(0, 8, null, "Matični podaci (Landscape)"     , TextImageRelation.ImageBeforeText);
      rbt_artikP         = hamperPrintDoc.CreateVvRadioButton(0, 9, null, "Matični podaci (Portrait)"      , TextImageRelation.ImageBeforeText);

      rbt_robKartA.Checked = true;
      rbt_robKartA.Tag     = true;

      VvHamper.HamperStyling(hamperPrintDoc);

   }
 
   private void InitializeHamper_RobKart(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q10un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun2 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun10;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbx_isRobKartica = hamper.CreateVvCheckBox_OLD(0, 0, new EventHandler(IsRobnaKartica_Checked), "Robna kartica (a ne transakcije)", System.Windows.Forms.RightToLeft.No);
      
      cbx_isRobKartica.Checked = true;

      VvHamper.HamperStyling(hamper);

   }

   public void IsRobnaKartica_Checked(object sender, EventArgs ea)
   {
      CheckBox cBox = sender as CheckBox;
      if(cBox.Checked)
      {
         SetZaUpisOtvorenoZatvoreno(ZXC.ZaUpis.Zatvoreno);
         EnableDisableRbt(true);
      }
      else
      {
         SetZaUpisOtvorenoZatvoreno(ZXC.ZaUpis.Otvoreno);
         EnableDisableRbt(false);
      }
   }

   public void EnableDisableRbt(bool enable)
   {
     rbt_artikL       .Enabled =
     rbt_artikP       .Enabled = 
     rbt_robKartA     .Enabled =
     rbt_robKartAMB   .Enabled =
     rbt_robKartAP    .Enabled =
     rbt_robKartKol   .Enabled =
     rbt_rbKrtKolSerlot.Enabled =
     rbt_robKartB     .Enabled = enable;
     rbt_robKartA     .Checked = enable;

     rbt_transakc.Enabled = !enable;
     rbt_transakc.Checked = !enable;
     
   }

   private void SetZaUpisOtvorenoZatvoreno(ZXC.ZaUpis zaUpis)
   {
      VvHamper.Open_Close_Fields_ForWriting(hamp_tt     ,             zaUpis, ZXC.ParentControlKind.VvReportUC);
      VvHamper.Open_Close_Fields_ForWriting(hamp_partner,             zaUpis, ZXC.ParentControlKind.VvReportUC);
      VvHamper.Open_Close_Fields_ForWriting(hamp_napom  ,             zaUpis, ZXC.ParentControlKind.VvReportUC);
      VvHamper.Open_Close_Fields_ForWriting(hamp_OdDo   ,             zaUpis, ZXC.ParentControlKind.VvReportUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_DatumDO, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.Open_Close_Fields_ForWriting(dtp_DatumDO, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

   }

   private void InitializeHamper_OdDo(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 4, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q4un + ZXC.Qun4, ZXC.Q4un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun8,            ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvRowHgt[0]    = ZXC.QUN - ZXC.Qun8;
      hamper.VvSpcBefRow[1] = ZXC.Qun12;
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(1, 0, "OD:", ContentAlignment.MiddleCenter);
      hamper.CreateVvLabel(2, 0, "DO:", ContentAlignment.MiddleCenter);

      hamper.CreateVvLabel(0, 1, "Datum:", ContentAlignment.MiddleRight);
      tbx_DatumOD      = hamper.CreateVvTextBox(1, 1, "tbx_datumOd", "");
      tbx_DatumOD.JAM_IsForDateTimePicker = true;
      dtp_DatumOD      = hamper.CreateVvDateTimePicker(1, 1, "", tbx_DatumOD);
      dtp_DatumOD.Name = "dtp_datumOd";
      dtp_DatumOD.Tag  = tbx_DatumOD;
      tbx_DatumOD.Tag  = dtp_DatumOD;

      tbx_DatumDO      = hamper.CreateVvTextBox(2, 1, "tbx_datumDo", "");
      tbx_DatumDO.JAM_IsForDateTimePicker = true;
      dtp_DatumDO      = hamper.CreateVvDateTimePicker(2, 1, "", tbx_DatumDO);
      dtp_DatumDO.Name = "dtp_datumDo";
      dtp_DatumDO.Tag  = tbx_DatumDO;
      tbx_DatumDO.Tag  = dtp_DatumDO;

      hamper.CreateVvLabel(0, 2, "Dok Broj:", ContentAlignment.MiddleRight);
      tbx_dokNumOd = hamper.CreateVvTextBox(1, 2, "tbx_dokNumOd", "", 6);
      tbx_dokNumDo = hamper.CreateVvTextBox(2, 2, "tbx_dokNumDo", "", 6);

      hamper.CreateVvLabel(0, 3, "TT Broj:", ContentAlignment.MiddleRight);
      tbx_ttNumOd = hamper.CreateVvTextBox(1, 3, "tbx_ttNumOd", "", 6);
      tbx_ttNumDo = hamper.CreateVvTextBox(2, 3, "tbx_ttNumDo", "", 6);

      tbx_ttNumOd .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_ttNumDo .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_dokNumOd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_dokNumDo.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_ttNumOd .JAM_FillCharacter = '0';
      tbx_ttNumDo .JAM_FillCharacter = '0';
      tbx_dokNumOd.JAM_FillCharacter = '0';
      tbx_dokNumDo.JAM_FillCharacter = '0';

      tbx_DatumOD.ContextMenu = dtp_DatumOD.ContextMenu =
                                tbx_DatumDO.ContextMenu =
                                dtp_DatumDO.ContextMenu = CreateNewContexMenu_Date();

      VvHamper.HamperStyling(hamper);

   }

   private void InitializeHamper_tt(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 1, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun2, ZXC.QUN + ZXC.Qun4, ZXC.Q2un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun8,            ZXC.Qun8,           ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel        (0, 0, "TT:", ContentAlignment.MiddleRight);
      tbx_TTopis   = hamper.CreateVvTextBox      (2, 0, "tbx_TtOpis", "", 32, 2, 0);
      tbxLookUp_TT = hamper.CreateVvTextBoxLookUp(1, 0, "tbxLookUp_TT", "");

      tbxLookUp_TT.JAM_CharacterCasing = CharacterCasing.Upper;
      tbxLookUp_TT.JAM_Set_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbxLookUp_TT.JAM_lui_NameTaker_JAM_Name = tbx_TTopis.JAM_Name;

      tbx_TTopis.JAM_ReadOnly = true;
      tbx_TTopis.Tag = ZXC.vvColors.userControl_BackColor;

      VvHamper.HamperStyling(hamper);

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamp_robKart, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamp_sklad, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);

   }

   private void InitializeHamper_sklad(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un - ZXC.Qun2, ZXC.Q2un, ZXC.Q4un-ZXC.Qun8 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel        (0, 0, "Za Sklad:", ContentAlignment.MiddleRight);
      tbx_skladOpis   = hamper.CreateVvTextBox      (2, 0, "tbx_skladOpis", "", 32, 1, 0);
      tbx_zaSkladCD = hamper.CreateVvTextBoxLookUp(1, 0, "tbxLookUp_sklad", "");
      tbx_zaSkladCD.Leave += new EventHandler(/*TheVvUC.*/tbx_zaSkladCD_Leave_RememberTheLastUsedSkladCD);
      //tbx_zaSkladCD.TextChanged += new EventHandler(tbx_zaSkladCD_TextChanged_RememberTheLastUsedSkladCD);

      tbx_zaSkladCD.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_zaSkladCD.JAM_WriteOnly = true;
      tbx_zaSkladCD.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_zaSkladCD.JAM_lui_NameTaker_JAM_Name = tbx_skladOpis.JAM_Name;

      tbx_skladOpis.JAM_ReadOnly = true;
      tbx_skladOpis.Tag = ZXC.vvColors.userControl_BackColor;

      cbx_grupaPoStr = hamper.CreateVvCheckBox_OLD(2, 1, null, 1, 0, "Sklad/Stranica", RightToLeft.No);
      cbx_MPSkpoNBC  = hamper.CreateVvCheckBox_OLD(0, 1, null, 1, 0,  "MPSKpoNBC"    , RightToLeft.No);

      VvHamper.HamperStyling(hamper);

   }
   private void tbx_zaSkladCD_Leave_RememberTheLastUsedSkladCD(object sender, EventArgs e)
   {
     // if(ZXC.TheVvForm != null && ZXC.TheVvForm.TheVvTabPage != null && ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvTextBox vvtb = sender as VvTextBox;

      ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD = vvtb.Text;

      if(TheVvUC is ArtiklUC) (TheVvUC as ArtiklUC).Fld_ZaSkladCD = vvtb.Text;
   }

   private void InitializeHamper_partner(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 4, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt = new int[] { ZXC.Q3un, ZXC.Q4un - ZXC.Qun2 - ZXC.Qun4, ZXC.QUN + ZXC.Qun2 + ZXC.Qun4, ZXC.Q4un - ZXC.Qun2 - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun8,              ZXC.Qun4, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
      }
      hamper.VvSpcBefRow = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun4, ZXC.Qun8};

      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel  (0, 0, "Partner:"      , ContentAlignment.MiddleRight);
 
      tbx_kupDobCD    = hamper.CreateVvTextBox(1, 0, "tbx_kupDobCD"   , "Šifra partnera", 6);
      tbx_kupDobTiker = hamper.CreateVvTextBox(3, 0, "tbx_kupDobTiker", "Ticker partnera", 6);
      tbx_kupDobNaziv = hamper.CreateVvTextBox(1, 1, "tbx_kupDobNaziv", "Naziv partnera", 30, 2, 0);

      tbx_kupDobCD   .JAM_MustTabOutBeforeSubmit = true;
      tbx_kupDobTiker.JAM_MustTabOutBeforeSubmit = true;
      tbx_kupDobNaziv.JAM_MustTabOutBeforeSubmit = true;

      tbx_kupDobCD   .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobTiker.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobNaziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyKupdobTextBoxLeave));

      tbx_kupDobTiker.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_kupDobCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      hamper.CreateVvLabel(0, 2, "GrupaPart:", ContentAlignment.MiddleRight);
      tbx_grupaPart = hamper.CreateVvTextBoxLookUp(1, 2, "tbx_grupaPart", "Samo za grupu partnera");
      tbx_grupaPart.JAM_Set_LookUpTable(ZXC.luiListaGrupaPartnera, (int)ZXC.Kolona.prva);
      tbx_grupaPart.JAM_lookUp_NOTobligatory = true;
      tbx_grupaPart.JAM_lookUp_MultiSelection = true;
      tbx_grupaPart.JAM_CharacterCasing = CharacterCasing.Upper;

      hamper.CreateVvLabel(0, 3, "VezDok:", ContentAlignment.MiddleRight);
      tbx_vezniDok = hamper.CreateVvTextBox(1, 3, "tbx_vezniDok", "Vezni dokument");

      hamper.CreateVvLabel(2, 3, "NaPl:", ContentAlignment.MiddleRight);
      tbx_nacinPl = hamper.CreateVvTextBox(3, 3, "tbx_nacinPl", "Način plaćanja");

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);

   }

   void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(TheVvUC.isPopulatingSifrar) return;

      //if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob  kupdob_rec;

      if(tb.Text != this.TheVvUC.originalText)
      {
         this.TheVvUC.originalText = tb.Text;
         kupdob_rec = VvUserControl.KupdobSifrar.Find(TheVvUC.FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_KupDobNaziv  = kupdob_rec.Naziv;
            Fld_KupDobCd     = kupdob_rec.KupdobCD/*RecID*/;
            Fld_KupDobTicker = kupdob_rec.Ticker;
         }
         else
         {
            Fld_KupDobNaziv = Fld_KupDobTicker = Fld_KupDobCdAsTxt = "";
         }
      }
   }
 
   private void InitializeHamper_Napom(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q8un -ZXC.Qun4, ZXC.QUN - ZXC.Qun5 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun8, ZXC.Qun12};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel       (0, 0, "Napomena:", ContentAlignment.MiddleRight);
      tbx_napomena          = hamper.CreateVvTextBox     (1, 0, "tbx_napomena", "Za napomenu");
      cbx_biloGdjeUnapomeni = hamper.CreateVvCheckBox_OLD(2, 0, null, "", RightToLeft.Yes, true);
      cbx_biloGdjeUnapomeni.Checked = true;

      VvHamper.HamperStyling(hamper);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
   }


   #endregion Hamper_s

   #region Fld_

   public DateTime Fld_DatumOd
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumOD.Value);
      }
      set
      {
         dtp_DatumOD.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DatumOD.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumOD);
      }
   }

   public DateTime Fld_DatumDo
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumDO.Value).EndOfDay();
      }
      set
      {
         dtp_DatumDO.Value = ZXC.ValOr_01011753_DateTime(value).EndOfDay();
         tbx_DatumDO.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumDO);
      }
   }

   public string Fld_TT
   {
      get { return tbxLookUp_TT.Text; }
      set {        tbxLookUp_TT.Text = value; }
   }

   public uint Fld_TtNumOd
   {
      get { return tbx_ttNumOd.GetSomeRecIDField();      }
      set {        tbx_ttNumOd.PutSomeRecIDField(value); }
   }

   public uint Fld_TtNumDo
   {
      get { return tbx_ttNumDo.GetSomeRecIDField(); }
      set {        tbx_ttNumDo.PutSomeRecIDField(value); }
   }

   public uint Fld_DokNumOd
   {
      get { return tbx_dokNumOd.GetSomeRecIDField();      }
      set {        tbx_dokNumOd.PutSomeRecIDField(value); }
   }

   public uint Fld_DokNumDo
   {
      get { return tbx_dokNumDo.GetSomeRecIDField(); }
      set {        tbx_dokNumDo.PutSomeRecIDField(value); }
   }

   public string Fld_Sklad
   {
      get 
      {
         ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD = tbx_zaSkladCD.Text;
         return tbx_zaSkladCD.Text; 
      }
      set 
      {
         //ZXC.TheVvForm.theLastUsedSkladCD = value;
         tbx_zaSkladCD.Text = value; 
      }
   }

   public bool Fld_NeedsHorizontalLine
   {
      get { return cb_Line.Checked; }
      set {        cb_Line.Checked = value; }
   }

   public string Fld_KupDobTicker
   {
      get { return tbx_kupDobTiker.Text; }
      set {        tbx_kupDobTiker.Text = value; }
   }
  
   public uint Fld_KupDobCd
   {
         get { return tbx_kupDobCD.GetSomeRecIDField();      }
         set {        tbx_kupDobCD.PutSomeRecIDField(value); }

   }
 
   public string Fld_KupDobCdAsTxt
   {
      get { return tbx_kupDobCD.Text; }
      set {        tbx_kupDobCD.Text = value; }
   }

   public string Fld_KupDobNaziv
   {
      get { return tbx_kupDobNaziv.Text; }
      set {        tbx_kupDobNaziv.Text = value; }
   }

   public string Fld_GrupaKupDob
   {
      get { return tbx_grupaPart.Text; }
      set {        tbx_grupaPart.Text = value; }
   }

   public string Fld_NacinPl
   {
      get { return tbx_nacinPl.Text; }
      set {        tbx_nacinPl.Text = value; }
   }

   public bool Fld_BiloGdjeUnapomeni
   {
      get { return cbx_biloGdjeUnapomeni.Checked; }
      set {        cbx_biloGdjeUnapomeni.Checked = value; }
   }

   public string Fld_Napomena
   {
      get { return tbx_napomena.Text; }
      set {        tbx_napomena.Text = value; }
   }

   public string Fld_VezniDok
   {
      get { return tbx_vezniDok.Text; }
      set {        tbx_vezniDok.Text = value; }
   }

   public ArtiklCardFilter.ArtiklCardsEnum Fld_ArtiklCard
   {
      get
      {
         if     (rbt_artikL        .Checked) return ArtiklCardFilter.ArtiklCardsEnum.ArtiklL;
         else if(rbt_artikP        .Checked) return ArtiklCardFilter.ArtiklCardsEnum.Artikl;
         else if(rbt_robKartA      .Checked) return ArtiklCardFilter.ArtiklCardsEnum.RobKartA;
         else if(rbt_robKartAMB    .Checked) return ArtiklCardFilter.ArtiklCardsEnum.RobKartAMB;
         else if(rbt_robKartAP     .Checked) return ArtiklCardFilter.ArtiklCardsEnum.RobKartAP;
         else if(rbt_robKartB      .Checked) return ArtiklCardFilter.ArtiklCardsEnum.RobKartB;
         else if(rbt_robKartKol    .Checked) return ArtiklCardFilter.ArtiklCardsEnum.RobKartKol;
         else if(rbt_rbKrtKolSerlot.Checked) return ArtiklCardFilter.ArtiklCardsEnum.RbKrtKolSerlot;
         else if(rbt_transakc      .Checked) return ArtiklCardFilter.ArtiklCardsEnum.RekapTrans;

         else throw new Exception("Fld_PrintSomeDebitDoc: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ArtiklCardFilter.ArtiklCardsEnum.ArtiklL       : rbt_artikL        .Checked = true; break;
            case ArtiklCardFilter.ArtiklCardsEnum.Artikl        : rbt_artikL        .Checked = true; break;
            case ArtiklCardFilter.ArtiklCardsEnum.RobKartA      : rbt_robKartA      .Checked = true; break;
            case ArtiklCardFilter.ArtiklCardsEnum.RobKartAMB    : rbt_robKartAMB    .Checked = true; break;
            case ArtiklCardFilter.ArtiklCardsEnum.RobKartAP     : rbt_robKartAP     .Checked = true; break;
            case ArtiklCardFilter.ArtiklCardsEnum.RobKartB      : rbt_robKartB      .Checked = true; break;
            case ArtiklCardFilter.ArtiklCardsEnum.RobKartKol    : rbt_robKartKol    .Checked = true; break;
            case ArtiklCardFilter.ArtiklCardsEnum.RbKrtKolSerlot: rbt_rbKrtKolSerlot.Checked = true; break;
            case ArtiklCardFilter.ArtiklCardsEnum.RekapTrans    : rbt_transakc      .Checked = true; break;
         }
      }
   }

   public bool Fld_IsRobKartica
   {
      get { return cbx_isRobKartica.Checked; }
      set {        cbx_isRobKartica.Checked = value; }
   }

   public bool Fld_GrupaPoStranici
   {
      get { return cbx_grupaPoStr.Checked; }
      set {        cbx_grupaPoStr.Checked = value; }
   }

   public bool Fld_MPSKpoNBC  { get { return cbx_MPSkpoNBC.Checked; } set { cbx_MPSkpoNBC.Checked = value; } }

   #endregion Fld_

   #region PutFilterFields(), GetFilterFields()

   private ArtiklCardFilter TheArtiklFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as ArtiklCardFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheArtiklFilter = (ArtiklCardFilter)_filter_data;

      if(TheArtiklFilter != null)
      {
         Fld_Sklad           = TheArtiklFilter.SkladCD;
         Fld_TT              = TheArtiklFilter.TT;
         Fld_TtNumOd         = TheArtiklFilter.TtNumOd;
         Fld_TtNumDo         = TheArtiklFilter.TtNumDo;
         Fld_DokNumOd        = TheArtiklFilter.DokNumOd;
         Fld_DokNumDo        = TheArtiklFilter.DokNumDo;
         Fld_KupDobTicker    = TheArtiklFilter.KD_ticker;
         Fld_KupDobCd        = TheArtiklFilter.KD_sifra;
         Fld_KupDobNaziv     = TheArtiklFilter.KD_naziv;
         Fld_NacinPl         = TheArtiklFilter.NacPlac;
         Fld_GrupaKupDob     = TheArtiklFilter.GrupaKupDob;
         Fld_VezniDok        = TheArtiklFilter.VezniDok;
         Fld_Napomena        = TheArtiklFilter.Napomena;
         Fld_GrupaPoStranici = TheArtiklFilter.AnaGrupaPoStranici;
         Fld_DatumOd         = TheArtiklFilter.DatumOd;
         Fld_DatumDo         = TheArtiklFilter.DatumDo.EndOfDay();
         Fld_MPSKpoNBC       = TheArtiklFilter.FuseBool1;

         Fld_NeedsHorizontalLine = TheArtiklFilter.NeedsHorizontalLine;
         Fld_ArtiklCard          = TheArtiklFilter.ArtiklCards;
        // tbx_skladOpis.Text      = ZXC.luiListaSkladista.GetNameForThisCd(TheArtiklFilter.SkladCD);
      }
      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
       TheArtiklFilter.SkladCD            = Fld_Sklad; 
       TheArtiklFilter.TT                 = Fld_TT;
       TheArtiklFilter.TtNumOd            = Fld_TtNumOd;
       TheArtiklFilter.TtNumDo            = Fld_TtNumDo; 
       TheArtiklFilter.DokNumOd           = Fld_DokNumOd;
       TheArtiklFilter.DokNumDo           = Fld_DokNumDo;
       TheArtiklFilter.KD_ticker          = Fld_KupDobTicker;
       TheArtiklFilter.KD_sifra           = Fld_KupDobCd;
       TheArtiklFilter.KD_naziv           = Fld_KupDobNaziv ;
       TheArtiklFilter.NacPlac            = Fld_NacinPl;
       TheArtiklFilter.GrupaKupDob        = Fld_GrupaKupDob;
       TheArtiklFilter.VezniDok           = Fld_VezniDok;
       TheArtiklFilter.Napomena           = Fld_Napomena;
       TheArtiklFilter.AnaGrupaPoStranici = Fld_GrupaPoStranici;
       TheArtiklFilter.DatumOd            = Fld_DatumOd;
       TheArtiklFilter.DatumDo            = Fld_DatumDo.EndOfDay();
       TheArtiklFilter.FuseBool1          = Fld_MPSKpoNBC;
      
       TheArtiklFilter.NeedsHorizontalLine = Fld_NeedsHorizontalLine;
       TheArtiklFilter.ArtiklCards         = Fld_ArtiklCard ;
   }

   internal void tbx_zaSkladCD_TextChanged_RememberTheLastUsedSkladCD(object sender, EventArgs e)
   {
      VvTextBox vvtb = sender as VvTextBox;

      //if(ZXC.TheVvForm != null && ZXC.TheVvForm.TheVvTabPage != null &&
      //   ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None &&
      //   vvtb.VVtag2 is bool && ((bool)vvtb.VVtag2) == true) return; // return-aj ako je WriteMode none,a ali samo sa FekturDUC-a, za ostalo nastavi dalje dolje. 

      ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD = vvtb.Text;
   }


   #endregion PutFilterFields(), GetFilterFields()

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
      ArtiklCardFilter   theRptFilter    = (ArtiklCardFilter)_vvRptFilter;
      VvRiskReport       theVvRiskReport = (VvRiskReport)    _vvReport;
     
 
      DateTime dateOD, dateDO;
      string   comparer;
      string   text, preffix;
      bool     isDummy, isCheck;
      uint     numOD, numDO;
      DataRow  drSchema;

      theRptFilter.FilterMembers.Clear();
      theRptFilter.ClearAllFilters_FromClauseGotTableName();

      isDummy = false;

      string forcedPreffix4JoinWithArtstat = "";

      if(Fld_ArtiklCard == ArtiklCardFilter.ArtiklCardsEnum.RobKartA      ||
         Fld_ArtiklCard == ArtiklCardFilter.ArtiklCardsEnum.RobKartB      ||
         Fld_ArtiklCard == ArtiklCardFilter.ArtiklCardsEnum.RobKartAP     ||
         Fld_ArtiklCard == ArtiklCardFilter.ArtiklCardsEnum.RobKartAMB    ||
         Fld_ArtiklCard == ArtiklCardFilter.ArtiklCardsEnum.RobKartKol    ||
         Fld_ArtiklCard == ArtiklCardFilter.ArtiklCardsEnum.RbKrtKolSerlot )
      {
         if(TheArtiklFilter.IsPopulatingTransDGV == false) forcedPreffix4JoinWithArtstat = "R";
      }

     // Fld_DatumOdDo                                                                                                                                                   

      drSchema = ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladDate];

    //dateOD = theRptFilter.DatumOd;                                                       ... verzija 1. 
    //dateOD = ZXC.projectYearFirstDay;                                                    ... verzija 2. 
    //if(theRptFilter.DatumOd < ZXC.projectYearFirstDay) dateOD = theRptFilter.DatumOd   ; ... verzija 3. 
    //else                                               dateOD = ZXC.projectYearFirstDay; ... verzija 3. 

      // 06.03.2020:                                                                                              
      // primjecujemo da ovo gore 'if(theRptFilter.DatumOd < ZXC.projectYearFirstDay)' ... je besmislica.         
      // idemo lijepo razluciti; kada je bilo koji vid robne kartice tada mora datumOD biti 01.01. tekuce godine  
      // rekap transakcija MOZE biti i neki drugi datumOD ... ALI IZ OVE GODINE (ne kuzim zasto je ikada spominjana prosla godina 

      //  ... verzija 4. 
      if(Fld_ArtiklCard != ArtiklCardFilter.ArtiklCardsEnum.RekapTrans) dateOD = ZXC.projectYearFirstDay;
      else                                                              dateOD = theRptFilter.DatumOd   ;

      dateDO = theRptFilter.DatumDo.EndOfDay();

      if(dateOD == dateDO) comparer = " = ";
      else                 comparer = " >= ";

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DateOD", dateOD, dateOD.ToString("dd.MM.yyyy."), "Od datuma:", comparer, "", forcedPreffix4JoinWithArtstat));

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DateDO", dateDO, dateDO.ToString("dd.MM.yyyy."), "Do datuma:", " <= "  , "", forcedPreffix4JoinWithArtstat));

      // Fld_ArtiklOdDo                                                                                                                                   

      drSchema = ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD];
      text = theArtiklUC.Fld_ArtiklCd;

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "ArtiklCD", text, text + " " + theArtiklUC.Fld_ArtiklName, "Za artikl:", " = ", "", forcedPreffix4JoinWithArtstat));

      //  Fld_Sklad                                                                                                                                                                           

      drSchema = ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladCD];
      text     = theRptFilter.SkladCD;

      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "SkladCD", text, text + " " + tbx_skladOpis.Text, "Za skladište:", " = ", "", forcedPreffix4JoinWithArtstat));
      }
      //  Fld_TT                                                                                                                                                                           

      if(Fld_ArtiklCard == ArtiklCardFilter.ArtiklCardsEnum.RekapTrans)
      {
         drSchema = ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt];
         text = theRptFilter.TT;

         if(text.NotEmpty())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TT", text, text, "Za tip:", " = ", ""));
         }
      }
      // Fld_TTnumOdDo                                                                                                                                     

      if(Fld_ArtiklCard == ArtiklCardFilter.ArtiklCardsEnum.RekapTrans)
      {
         drSchema = ZXC.RtransSchemaRows[ZXC.RtrCI.t_ttNum];
         numOD = theRptFilter.TtNumOd;
         numDO = theRptFilter.TtNumDo;

         if(numOD.NotZero())
         {
            if(numOD == numDO) comparer = " = ";
            else comparer = " >= ";

            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNumOD", numOD, numOD.ToString("000000"), "Od TT broj:", comparer, ""/*, Rtrans.recordName*/));
         }
         else if(numDO.NotZero())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "TtNumOD", numOD, "-", "Od TT broj:", " >= ", ""/*, Rtrans.recordName*/));
         }

         if(numDO.NotZero())
         {
            if(numDO == numOD) isDummy = true;
            else isDummy = false;

            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "TtNumDO", numDO, numDO.ToString("000000"), "Do TT broj:", " <= ", ""/*, Rtrans.recordName*/));
         }
         else if(numOD.NotZero())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "TtNumDO", numDO, "-", "Do TT broj:", " <= ", ""/*, Rtrans.recordName*/));
         }

         isDummy = false;
      }
      // Fld_KupdobCD                                                                                                                                     

      if(Fld_ArtiklCard == ArtiklCardFilter.ArtiklCardsEnum.RekapTrans)
      {
         drSchema = ZXC.RtransSchemaRows[ZXC.RtrCI.t_kupdobCD];
         numOD = theRptFilter.KD_sifra;
         text = Fld_KupDobCdAsTxt + " " + Fld_KupDobTicker + " " + Fld_KupDobNaziv;
         if(numOD.NotZero())
         {
            if(ZXC.IsSvDUH && theRptFilter.KD_naziv.Length == 1)
            {
               theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.KupdobSchemaRows[ZXC.KpdbCI.isMtr ], false, "isMtr" , true,                  "",                    "",                " = ", Kupdob.recordName));
               theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.KupdobSchemaRows[ZXC.KpdbCI.ulica1], false, "ulica1", theRptFilter.KD_naziv, theRptFilter.KD_naziv, "Zavod / klinika", " = ", Kupdob.recordName));
            }
            else // classic 
            {
               theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Partner", numOD, text, "Za partnera:", " = ", ""));
            }
         }
      }
      // Fld_DokNumOdDo                                                                                                                                     

      if(Fld_ArtiklCard == ArtiklCardFilter.ArtiklCardsEnum.RekapTrans)
      {
         drSchema = ZXC.RtransSchemaRows[ZXC.RtrCI.t_dokNum];
         numOD = theRptFilter.DokNumOd;
         numDO = theRptFilter.DokNumDo;

         if(numOD.NotZero())
         {
            if(numOD == numDO) comparer = " = ";
            else comparer = " >= ";

            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DokNumOD", numOD, numOD.ToString("000000"), "Od dokumenta broj:", comparer, ""/*, Rtrans.recordName*/));
         }
         else if(numDO.NotZero())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "DokNumOD", numOD, "-", "Od dokumenta broj:", " >= ", ""/*, Rtrans.recordName*/));
         }

         if(numDO.NotZero())
         {
            if(numDO == numOD) isDummy = true;
            else isDummy = false;

            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "DokNumDO", numDO, numDO.ToString("000000"), "Do dokumenta broj:", " <= ", ""/*, Rtrans.recordName*/));
         }
         else if(numOD.NotZero())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "DokNumDO", numDO, "-", "Do dokumenta broj:", " <= ", ""/*, Rtrans.recordName*/));
         }

         isDummy = false;
      }

      //       
      // Nota bene!!!                                                                                                                                                   
      // U FIlterMember-u: 'relatedTable' treba zadati kada podatka po kojemu filtriramo NEMA u ptrans-u, vec dolazi iz neke druge tablice (npr. Person, Placa, ...)    
      // U FIlterMember-u: 'forcedPreffix' treba zadati kada se podatak po kojemu filtriramo nalazi i u Ptransu i u (Ptrane-ou ili Ptrano-u)                            
      //       

      // Fld_Napomena                                                                                                                                    

      if(Fld_ArtiklCard == ArtiklCardFilter.ArtiklCardsEnum.RekapTrans)
      {
         drSchema = ZXC.FakturSchemaRows[ZXC.FakCI.napomena];
         text     = theRptFilter.Napomena;

         isCheck = Fld_BiloGdjeUnapomeni;
         preffix = isCheck ? "%" : "";

         if(text.NotEmpty())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "F_Napomena", preffix + text + "%", text, "Za dok. napomenu:", " LIKE ", Faktur.recordName));
         }
      }
      // Fld_Grupa                                                                                                                                      

      if(Fld_ArtiklCard == ArtiklCardFilter.ArtiklCardsEnum.RekapTrans)
      {
         drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.tip];
         text     = theRptFilter.GrupaKupDob;

         if(text.NotEmpty())
         {
            text = ZXC.GetOrovaniCharPattern(theRptFilter.GrupaKupDob);
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "KDgrupa", text, text, "Za grupu partnera:", " REGEXP ", Kupdob.recordName));
         }
      }

      // Fld_VezniDok                                                                                                                                                                           

      if(Fld_ArtiklCard == ArtiklCardFilter.ArtiklCardsEnum.RekapTrans)
      {
         drSchema = ZXC.FakturSchemaRows[ZXC.FakCI.vezniDok];
         text     = theRptFilter.VezniDok;

         if(text.NotEmpty())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "F_VezniDok", text, text, "Za vezni dokument:", " = ", Faktur.recordName));
         }
      }

      // Fld_NacinPl                                                                                                                                                                           
      //drSchema = ZXC.FakturSchemaRows[ZXC.FakCI.nacPlac]; TODO:!!!
      if(Fld_ArtiklCard == ArtiklCardFilter.ArtiklCardsEnum.RekapTrans)
      {
         text = theRptFilter.NacPlac;

         if(text.NotEmpty())
         {
            throw new Exception("TODO: kada dodje nacin placanja bussuness u Faktur.");
            //theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "F_NacinPlacanja", text, text, "Za nacin placanja:", " = ", Faktur.recordName));
         }
      }
   }

   #endregion AddFilterMemberz()

}

#region TAB_FakturDocFilterUC

public class FakturDocFilterUC    : VvFilterUC
{
   #region Fieldz

   private CheckBox  cbx_verLine, cbx_TableBorder, cbx_signPrimaoc, cbx_tekstualIznos,
                     cbx_IsAddTtNumRn, cbx_IsAddYearRn, cbx_IsAddKupDobCdRn, cbx_IsAddTT,
                     cbx_IsAddTtNumPb, cbx_IsAddYearPb, cbx_IsAddKupDobCdPb,
                     cbx_T_artiklCD   , cbx_T_artiklName , cbx_Napomena     , cbx_BarCode1     , cbx_ArtiklCD2    ,   cbx_T_garancija   ,
                     cbx_ArtiklName2  , cbx_BarCode2     , cbx_LongOpis     , cbx_SerNo        , cbx_T_jedMj      ,     
                     cbx_T_kol        , cbx_T_cij        , cbx_R_KC         , cbx_T_rbt1St     , cbx_R_rbt1       ,      
                     cbx_T_rbt2St     , cbx_R_rbt2       , cbx_R_cij_KCR    , cbx_R_KCR        , cbx_T_mrzSt  /*Serlot 26.02.2016.*/    ,
                     cbx_R_mrz/*R_cijOP 18.12.2013.*/, cbx_R_cij_KCRM/*Tkn_Cij 29.08.2013*/   , cbx_R_KCRM/*Rkn_KCRP 29.08.2013*/       , cbx_R_ztr, cbx_T_pdvSt,  
                     cbx_R_pdv        , cbx_R_cij_KCRP   , cbx_R_KCRP       , cbx_T_doCijMal   , cbx_T_noCijMal   , cbx_R_mjMasaN,
                     cbx_razmak       , cbx_ocuHeader    , cbx_ocuFooter    , cbx_ocuFooter2, cbx_ocuLogo      , cbx_ocuPnb, cbx_ocuR12,
                     cbx_blgOcuColKonto  , cbx_blgOcuColRacun  , cbx_blgOcu2na1strani, cbx_blgOcuOkvirUplsp,
                     cbx_OcuKupDobTel       , cbx_OcuProjektTel        , cbx_OcuKupDobOpis,
                     cbx_OcuKupDobFax       , cbx_OcuProjektFax        ,
                     cbx_OcuKupDobOib       , cbx_OcuProjektOib        ,
                     cbx_OcuKupDobMail      , cbx_OcuProjektMail       ,
                     cbx_OcuKupDobNr        ,
                     cbx_PotvrdaNarudzbe    ,
                     cbx_OcuIspisDospjecePl ,
                     cbx_OcuIspisVeze1      ,
                     cbx_OcuIspisVeze2      ,
                     cbx_OcuIspisVeze3      ,
                     cbx_OcuIspisVeze4      ,
                     cbx_OcuIspisVezDok2    ,
                     cbx_OcuIspisNapomena2  ,
                     cbx_OcuIspisDokNum2    ,
                     cbx_OcuMemoHAllPages, cbx_OcuMemoFAllPages,
                     cbx_OcuTitleOkvir, cbx_OcuTitleBoja,
                     cbx_OcuKupDobBoja, cbx_OcuPrjktBoja,
                     cbx_OcuLinijeHeader, cbx_OcuLinijeFooter, cbx_OcuIspisVirmana, cbx_OcuKDZiro_Vir,
                     cbx_ocuVezDok,
                     cbx_ocuZiro, cbx_ocuLinijePerson, cbx_ocuTextNap2, cbx_ocuNapomena, cbx_ocuFirmuUpotpis, cbx_ocuTitulu, cbx_textOpisDrugiRed,
                     cbx_fakAdresKaoPoslJed, cbx_plVirUrokuDana, cbx_ispisOtpremnicaBr, cbx_ispisUgovora, cbx_onlyUkupnaTezina, cbx_centarNapKaoNaslov, 
                     cbx_OibIspodAdrese,
                     cbx_ocuIspisPrjktBr,
                     cbx_OcuDevCijAndDevTec, cbx_OcuRokIsporDokDate, cbx_OcuMtrosName,
                     cbx_OcuDateX, cbx_OcuPomakVirmana, cbx_OcuPosPrint, cbx_OcuMojuPoslJed, cbx_OcuDatumRacuna,
                     cbx_OcuNapomUmjKupDob, cbx_OcuLikvidator, cbx_Ocu_OTS_saldo, cbx_Ocu_BarkodTtNum, cbx_Ocu_MemoAddGore,
                     cbx_necuFiskalDodatak, cbx_Ocu_BarKodPDF417, cbx_Necu_prikazEUR;

   private VvTextBox   tbx_lblPrjktPerson, tbx_lblUserPerson, tbx_lblOsobaA, tbx_lblOsobaB, 
                       tbx_ValName, tbx_nazivPoslJed,
                       tbx_prefixIR1Rn, tbx_prefixIR2Rn, tbx_prefixIR1Pb, tbx_prefixIR2Pb, tbx_pnbM, tbx_nazivPrinta,                                      
                       tbx_prefixIR1_Rn, tbx_prefixIR2_Rn, tbx_IsAddTtNum_Rn, tbx_IsAddYear_Rn, tbx_IsAddKupDobCd_Rn, tbx_SeparIfTT_Rn,
                       tbx_prefixIR1_Pn, tbx_prefixIR2_Pn, tbx_IsAddTtNum_Pn, tbx_IsAddYear_Pn, tbx_IsAddKupDobCd_Pn,
                       tbx_NumDecT_kol      , tbx_NumDecT_cij      , tbx_NumDecR_KC       , tbx_NumDecT_rbt1St   , 
                       tbx_NumDecR_rbt1     , tbx_NumDecT_rbt2St   , tbx_NumDecR_rbt2     , tbx_NumDecR_cij_KCR  , 
                       tbx_NumDecR_KCR      , tbx_NumDecT_mrzSt    , tbx_NumDecR_mrz      , tbx_NumDecR_cij_KCRM , 
                       tbx_NumDecR_KCRM     , tbx_NumDecR_ztr      , tbx_NumDecT_pdvSt    , tbx_NumDecR_pdv      , 
                       tbx_NumDecR_cij_KCRP, tbx_NumDecR_KCRP    , tbx_NumDecT_doCijMal , tbx_NumDecT_noCijMal  ,
                       tbx_Primio, tbx_opciA, tbx_opciB, tbx_scalingLogo, tbx_belowGrid,
                       tbx_hrv, tbx_eng, tbx_njem, tbx_tal,
                       tbx_LblVeze1  ,
                       tbx_LblVeze2  ,
                       tbx_LblVeze3  ,
                       tbx_LblVeze4  ,
                       tbx_LblVezDok2,
                       tbx_LblOsobaX, tbx_AboveGrid,
                       tbx_beforNRD, tbx_afterNRD,
                       tbx_obrazacA, tbx_obrazacB, tbx_obrazacC, tbx_obrazacD, tbx_obrazacE, tbx_postoBefore, tbx_postoAfter, tbx_koefPIX,
                       tbx_lblDateX, tbx_belowOnPOS, tbx_memoPOS, tbx_memoAdd,
                       tbx_scalingLogoFP,
                       tbx_rezervacija1, tbx_rezervacija2, tbx_oslobodenjePDV;

   private RadioButton rbt_AdresRight, rbt_AdresLeft, rbt_RptPort, rbt_RptLandsc,
                       rbt_ira1, rbt_ira2, rbt_ira3, rbt_ira4, rbt_ira5,
                       rbt_opis7, rbt_opis8, rbt_opis9, rbt_opis10, 
                       rbt_col7, rbt_col8, rbt_col9, rbt_col10,
                       rbt_belGr7, rbt_belGr8, rbt_belGr9, rbt_belGr10,
                       rbt_hrv, rbt_eng, rbt_njem, rbt_tal,
                      
                       //adresa
                       rbt_adresOkvir, rbt_adresNoOkvir,
                       rbt_adresOnlyPartner, rbt_adresBoth,
                       //pageNum
                       rbt_alignmentPageNum_L, rbt_alignmentPageNum_C, rbt_alignmentPageNum_R,
                       rbt_printPageNum_PHeader, rbt_printPageNum_PFooter, rbt_printPageNum_NO,
                       //dokNum
                       rbt_alignmentDokNum_L, rbt_alignmentDokNum_C, rbt_alignmentDokNum_R,
                       rbt_printDokNum_beforAdres, rbt_printDokNum_afterAdres,

                       rbt_mig_RH, rbt_mig_RF,
                       rbt_leftMargin1, rbt_leftMargin2, rbt_leftMargin3, rbt_leftMargin4,rbt_leftMargin5,
                       rbt_rightMargin1, rbt_rightMargin2, rbt_rightMargin3, rbt_rightMargin4,
                       rbt_personRight, rbt_personCentar,
                       rbt_logoFP_NE, rbt_logoFP_Footer, rbt_logoFP_Potpis, rbt_logoFP_Logo2,
                       rbt_alignmentLogoFP_L, rbt_alignmentLogoFP_C, rbt_alignmentLogoFP_R,
                       rbt_alignmentMemAdd_L, rbt_alignmentMemAdd_C, rbt_alignmentMemAdd_R,
                       
                       rbt_posClassic, rbt_posBixolon01, rbt_posEpson01, rbt_posPos80, rbt_posBixolon02, rbt_posEpson02
                       /*, rbt_KLK_URM_DUC, rbt_KLK_URM_Naljepnica*/;

   private VvHamper    hamp_Columns, hamp_personi, hamp_adresLR, hamp_orient, hamp_OpenFilter, hamp_RnPnb,
                       hamp_ChoseIra, hamp_belowGrid, hamp_CbxOstalo, hamp_CbxTablica, hamp_ostalo, hamp_poslJed, hamp_logo, hamp_valuta, 
                       hamp_FontOpis, hamp_FontCol, hamp_FontBelGr, hamp_jezik, hamp_Blg,
                       hamp_adresOkvir, hamp_adresToFrom, hamp_adresSastojci,
                       hamp_pageHF, hamp_pageLCR,
                       hamp_dokNumHFadres, hamp_dokNumLCR,
                       hamp_migHF, hamp_NRD, hamp_memo, hamp_title,
                       hamp_leftMargin, hamp_rightMargin, hamp_postotakOpis, hamp_rtranoOtpr, hamp_memoPOS, hamp_Logo2Algiment, hamp_memoAdd, hamp_posPrinteri,
                       hamp_priKalk, hamp_rezervacije, /*hamp_RNMDUC,*/ hamp_oslobodenjePDV, hamp_KLK_URM, hamp_UGANducPTG;
   private Button      btn_OpenFilter, btn_RtranoOtpr,btn_RtranoOtprM2, btn_RtranoOtprDimZ, btn_Rtrano4PIX, btn_PpmvPrilog1,
                       btn_PriKalkulacija, btn_RNM_Nalog, btn_RNM_Realz, btn_barkodPrint,
                       btn_KLK_URM_DUC, btn_KLK_URM_Naljepnica, btn_KLK_URM_DUC_Ztr,
                       btn_PTG_OPL, btn_PTG_DOD, btn_PTG_UGAN;

   public VvHamper     hamp_ppmvPrilog1;

   private int minFilterWidth, maxFilterWidth, maxFilterHeight;
   private Label lbl_textAbovGr, lbl_oslobodenjePDV, lbl_Ostalo;

   //5.5.tam
  // private Faktur mixer_rec { get { return ((FakturExtDUC)TheVvUC).mixer_rec; } }
   private Faktur faktur_rec { get { return (TheVvUC is FakturExtDUC ? ((FakturExtDUC)TheVvUC).faktur_rec : ((FakturDUC)TheVvUC).faktur_rec); } }

   private bool isFakExt;
  
   private Crownwood.DotNetMagic.Controls.TabControl tabControlPrintFak;
   private Crownwood.DotNetMagic.Controls.TabPage tabMemo, tabPartner, tabIzgled, tabKolone, tabOsobeVeze, tabOstalo, tabTextDopune, tabBlag;

   // 06.11.2013: 
   //public PrnFakDsc PFD { get; set; }

   public bool IsRtrnoOtpr      = false;
   public bool IsRtrnoOtpr_m2   = false;
   public bool IsRtrnoOtpr_dimZ = false;
   public bool IsRtrno4PIX      = false;
   public bool IsPPMV_Prilog1   = false;
   public bool IsPri_Kalk       = false;
   public bool IsRnmNalog       = false;
   public bool IsRnmRealiz      = false;
   public bool IsBarCodePrint   = false;
   public bool IsKlkUrm_DUC     = true;
   public bool IsKlkUrm_Etiketa = false;
   public bool IsKlkUrm_Ztr     = false;
   public bool IsPTG_UGAN       = false;
   public bool IsPTG_OPL        = false;
   public bool IsPTG_DOD        = false;

   #endregion Fieldz

   #region  Constructor

   public FakturDocFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;
      
      isFakExt = (TheVvUC is FakturExtDUC);

      hamperHorLine.Visible = false;

      CreateHamper_4ButtonsResetGo_Width(ZXC.Q4un);

      CreateTabControlMitTabPages();
      CreateHampers();

      if(ZXC.CURR_userName != ZXC.vvDB_systemSuperUserName && ZXC.CURR_userName != ZXC.vvDB_programSuperUserName && !ZXC.CURR_user_rec.IsSuper)
         btn_OpenFilter.Visible = false;
      else
         btn_OpenFilter.Visible = true;

      if(((FakturDUC)TheVvUC).IsRadNalog) btn_OpenFilter.Visible = false;

      ChooseDscLuiList(null, EventArgs.Empty);

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      //ThePFD = new PrnFakDsc(FakturDUC.GetDscLuiListForThisTT(RptFilter.TT, 0));

      this.ResumeLayout();
   }


   #endregion  Constructor

   #region TabControl
  
   private void CreateTabControlMitTabPages()
   {
      tabControlPrintFak                  = new Crownwood.DotNetMagic.Controls.TabControl();
      tabControlPrintFak.Parent           = this;
      tabControlPrintFak.Location         = new Point(0, 0);
      tabControlPrintFak.Size             = new Size(this.Width - ZXC.Q10un, this.Height);
      tabControlPrintFak.Anchor           = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      tabControlPrintFak.Appearance       = Crownwood.DotNetMagic.Controls.VisualAppearance.MultiDocument;
      tabControlPrintFak.PositionTop      = true;
      tabControlPrintFak.ShowClose        = false;
      tabControlPrintFak.Style            = ZXC.vvColors.vvform_VisualStyle;
      tabControlPrintFak.OfficeStyle      = ZXC.vvColors.tabControl_OfficeStyle;
      tabControlPrintFak.MediaPlayerStyle = ZXC.vvColors.tabControl_MediaPlayerStyle;
      tabControlPrintFak.Visible          = false;

      tabMemo        = new Crownwood.DotNetMagic.Controls.TabPage(TabPageTitle1);
      tabPartner     = new Crownwood.DotNetMagic.Controls.TabPage(TabPageTitle2);
      tabIzgled      = new Crownwood.DotNetMagic.Controls.TabPage(TabPageTitle3);
      tabKolone      = new Crownwood.DotNetMagic.Controls.TabPage(TabPageTitle4);
      tabOsobeVeze   = new Crownwood.DotNetMagic.Controls.TabPage(TabPageTitle5);
      tabOstalo      = new Crownwood.DotNetMagic.Controls.TabPage(TabPageTitle6);
      tabTextDopune  = new Crownwood.DotNetMagic.Controls.TabPage(TabPageTitle7);
      tabBlag        = new Crownwood.DotNetMagic.Controls.TabPage(TabPageTitle8);

      if(TheVvUC is BlgUplatDUC || TheVvUC is BlgIsplatDUC || TheVvUC is BlgUplat_M_DUC || TheVvUC is BlgIsplat_M_DUC)
      {
         tabControlPrintFak.TabPages.Add(tabBlag);
      }
      else
      {
         tabControlPrintFak.TabPages.Add(tabMemo);
         tabControlPrintFak.TabPages.Add(tabPartner);
         tabControlPrintFak.TabPages.Add(tabIzgled);
         tabControlPrintFak.TabPages.Add(tabKolone);
         tabControlPrintFak.TabPages.Add(tabOsobeVeze);
         tabControlPrintFak.TabPages.Add(tabOstalo);
         tabControlPrintFak.TabPages.Add(tabTextDopune);
      }

      tabMemo      .AutoScroll = true;
      tabPartner   .AutoScroll = true;
      tabIzgled    .AutoScroll = true;
      tabKolone    .AutoScroll = true;
      tabOsobeVeze .AutoScroll = true;
      tabOstalo    .AutoScroll = true;
      tabTextDopune.AutoScroll = true;
      tabBlag      .AutoScroll = true;

   }

   public string TabPageTitle1 { get { return "Memo/Izgled"   ; } }
   public string TabPageTitle2 { get { return "Partner/Adresa"; } }
   public string TabPageTitle3 { get { return "Račun/Pnb"     ; } }
   public string TabPageTitle4 { get { return "Kolone/Tablica"; } }
   public string TabPageTitle5 { get { return "Osobe/Veze"    ; } }
   public string TabPageTitle6 { get { return "TekstoviDopuna"; } }
   public string TabPageTitle7 { get { return "Ostalo"        ; } }
   public string TabPageTitle8 { get { return "Blagajna"      ; } }

   #endregion TabControl

   #region Hampers

   private void CreateHampers()
   {
      CreateHamperChoseIra(out hamp_ChoseIra);

      if(TheVvUC is IRMDUC)
      {
         CreateHamperPosPrinteri(out hamp_posPrinteri);
      }

      CreateHamperOpenFilter(out hamp_OpenFilter);
      RecalculateHamper4buttons();

      if(TheVvUC is FakturExtDUC && (FakturExtDUC)TheVvUC is IRPDUC) CreateHamperRtranoOtprFilter(out hamp_rtranoOtpr);

      if(TheVvUC is FakturExtDUC && (FakturExtDUC)TheVvUC is IRMDUC_2) CreateHamperPPMV_Prilog1(out hamp_ppmvPrilog1);
      if(TheVvUC is FakturExtDUC && (FakturExtDUC)TheVvUC is RNPDUC) CreateHamper_Rezervacije(out hamp_rezervacije);
      //if(TheVvUC is FakturExtDUC && (FakturExtDUC)TheVvUC is RNMDUC  ) CreateHamper_RNMDUC     (out hamp_RNMDUC);

      //if(TheVvUC is FakturExtDUC && (FakturExtDUC)TheVvUC is PrimkaDevDUC)                                     CreateHamperPrimkaKalkulacija(out hamp_priKalk, "Kalkulacija");
      // 29.03.2016.  
      if(TheVvUC is FakturExtDUC && ((FakturExtDUC)TheVvUC is PrimkaVpDUC || (FakturExtDUC)TheVvUC is KIZDUC)) CreateHamperPrimkaKalkulacija(out hamp_priKalk, (FakturExtDUC)TheVvUC is PrimkaVpDUC ? "Kalkulacija" : "KIZ skraćeni");
    
    //if(TheVvUC is FakturDUC && ((FakturDUC)TheVvUC is MedjuSkladDUC || (FakturDUC)TheVvUC is MedjuSkladDUC)                        ) CreateHamperPrimkaKalkulacija(out hamp_priKalk, "MSI KOL");
      if(TheVvUC is FakturDUC && ((FakturDUC)TheVvUC is MedjuSkladDUC || (FakturDUC)TheVvUC is MedjuSkladDUC) && ZXC.IsSvDUH == false) CreateHamperPrimkaKalkulacija(out hamp_priKalk, "MSI KOL");

      //07.09.2020.
      if(TheVvUC is FakturDUC && ((FakturDUC)TheVvUC is URMDUC || (FakturDUC)TheVvUC is KalkulacijaMpDUC))
      {
         CreateHamper_KLK_URM(out hamp_KLK_URM);
         hamper4buttons.Visible =
         hamp_ChoseIra.Visible =
         hamperHorLine.Visible =
         hamp_OpenFilter.Visible = false;
         minFilterWidth = hamp_KLK_URM.Width + ZXC.QUN;
      }

      //08.11.2021.
      if(ZXC.IsPCTOGO && TheVvUC is FakturExtDUC && (FakturExtDUC)TheVvUC is UGNorAUN_PTG_DUC)
      {
         CreateHamper_UGANDUC_PTG_Filter(out hamp_UGANducPTG);
         hamper4buttons .Visible =
         hamp_ChoseIra  .Visible =
         hamperHorLine  .Visible =
         hamp_OpenFilter.Visible = false;
         minFilterWidth = hamp_UGANducPTG.Width/* + ZXC.Qun8*/;
      }

      nextX = ZXC.Qun4;
      nextY = ZXC.Qun4;
      razmakIzmjedjuHampera = ZXC.Qun4;
      // ---------------- Blg  ------------------------------------

      CreateHampBlg(out hamp_Blg);

      // ---------------- Memo  ------------------------------------
      CreateHamperOrient      (out hamp_orient);
      CreateHamper_migHF      (out hamp_migHF);
      CreateHamperLogo        (out hamp_logo);
      CreateHamper_logo2Aligm (out hamp_Logo2Algiment);
      CreateHamper_memo       (out hamp_memo);
      CreateHamper_memoAdd    (out hamp_memoAdd);
      CreateHamperLeftMargin  (out hamp_leftMargin);
      CreateHamperRightMargin (out hamp_rightMargin);
      CreateHamper_pageHF     (out hamp_pageHF);
      CreateHamper_pageLCR    (out hamp_pageLCR);
      CreateHamperFontOpis    (out hamp_FontOpis);
      CreateHamperFontBelGr   (out hamp_FontBelGr);
      CreateHamper_memoPOS    (out hamp_memoPOS);

     
      // ---------------- Adresa  ------------------------------------
      CreateHamperAdres          (out hamp_adresLR);
      CreateHamper_adresOkvir    (out hamp_adresOkvir);
      CreateHamper_adresToFrom   (out hamp_adresToFrom);
      CreateHamper_adresSastojci (out hamp_adresSastojci);
      CreateHamperPoslJed        (out hamp_poslJed);
     
      // ---------------- DokNum  ------------------------------------

      CreateHamperRnPnb(out hamp_RnPnb);

      CreateHamper_dokNumHFadres (out hamp_dokNumHFadres);
      CreateHamper_dokNumLCR     (out hamp_dokNumLCR);
      CreateHamper_dokNumTitle   (out hamp_title);

      // ---------------- Columns  ------------------------------------

      CreateHamperColumns(out hamp_Columns);
      CreateHamperFontCol(out hamp_FontCol);
      CreateHampCbxTablica(out hamp_CbxTablica);

      // ----------------Person/veze  ------------------------------------
      CreateHamperPersoni(out hamp_personi);
      CreateHampOstalo(out hamp_ostalo);


      // ---------------- Tekst  ------------------------------------
      CreateHamperBelowGrid(out hamp_belowGrid);
      CreateHamperNRD(out hamp_NRD);
      CreateHamperPostotak(out hamp_postotakOpis);
      CreateHamperOslobodenjePDV(out hamp_oslobodenjePDV);

      // ---------------- Ostalo  ------------------------------------

      CreateHamperValuta(out hamp_valuta);
      CreateHamperJezik(out hamp_jezik);
      CreateHampCbxOstalo(out hamp_CbxOstalo);

      //07.09.2020.
    //minFilterWidth = hamp_ChoseIra.Width + 2 * razmakIzmjedjuHampera;
      if(TheVvUC is FakturDUC && ((FakturDUC)TheVvUC is URMDUC || (FakturDUC)TheVvUC is KalkulacijaMpDUC))
         minFilterWidth = hamp_KLK_URM.Width + ZXC.QUN;
      else if(TheVvUC is FakturDUC && ((FakturDUC)TheVvUC is UGNorAUN_PTG_DUC))
         minFilterWidth = hamp_UGANducPTG.Width + ZXC.Qun4;
      else
         minFilterWidth = hamp_ChoseIra.Width + 2 * razmakIzmjedjuHampera;
     
      // maxFilterWidth = hamp_Columns.Width + 2 * razmakIzmjedjuHampera + ZXC.QUN + ZXC.Qun2;
      tabControlPrintFak.Width = hamp_ostalo.Width;
      maxFilterWidth = tabControlPrintFak.Width + hamp_ChoseIra.Width + 2 * razmakIzmjedjuHampera + ZXC.Qun2;

      maxFilterHeight = ZXC.Q10un * 2 + ZXC.Q5un;
      CalcLocationHampersOnFilter(true);


      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamp_NRD    , hamp_belowGrid.Width, razmakIzmjedjuHampera);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamp_Columns, hamp_Columns.Width  , razmakIzmjedjuHampera);

      //SetVisibilitiOfHampers(false);
   }


   #region     Memo/Izgled 
   
   private void CreateHamperOrient     (out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", tabControlPrintFak.TabPages[TabPageTitle1], false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q4un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel      (0, 0, "Orijentacija dokumenta:", ContentAlignment.MiddleRight);
      rbt_RptPort   = hamper.CreateVvRadioButton(1, 0, null, "Portrait", TextImageRelation.ImageBeforeText);
      rbt_RptLandsc = hamper.CreateVvRadioButton(2, 0, null, "Landscape", TextImageRelation.ImageBeforeText);
      //rbt_RptPort.Checked = true;
      //rbt_RptPort.Tag     = true;
      VvHamper.HamperStyling(hamper);
   }
   
   private void CreateHamper_migHF     (out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", tabControlPrintFak.TabPages[TabPageTitle1], false, nextX, hamp_orient.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q4un, ZXC.Q4un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;


                   hamper.CreateVvLabel      (0, 0, "Pozicija dod. podataka:", ContentAlignment.MiddleRight);
      rbt_mig_RH = hamper.CreateVvRadioButton(1, 0, null, "Header", TextImageRelation.ImageBeforeText);
      rbt_mig_RF = hamper.CreateVvRadioButton(2, 0, null, "Footer", TextImageRelation.ImageBeforeText);
      //rbt_mig_RH.Checked = true;
      //rbt_mig_RH.Tag = true;

      VvHamper.HamperStyling(hamper);
   }
  
   private void CreateHamperLogo       (out VvHamper hamper)
   {
      hamper = new VvHamper(6, 2, "", tabControlPrintFak.TabPages[TabPageTitle1], false, nextX, hamp_migHF.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un , ZXC.Q4un - ZXC.Qun4, ZXC.Q3un - ZXC.Qun4, ZXC.Q3un - ZXC.Qun4, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8 , ZXC.Qun8, ZXC.Qun12, ZXC.Qun12, ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel  (0, 0, "Skaliranje LogoH:", ContentAlignment.MiddleRight);
      tbx_scalingLogo = hamper.CreateVvTextBox(1, 0, "tbx_scalingLogo", "Postotak skaliranja logo slike za print", 7);
      tbx_scalingLogo.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_scalingLogo.JAM_IsForPercent = true;

                          hamper.CreateVvLabel  (0, 1, "Skaliranje Slika2:", ContentAlignment.MiddleRight);
      tbx_scalingLogoFP = hamper.CreateVvTextBox(1, 1, "tbx_scalingLogoFP", "Postotak skaliranja slike za print", 7);
      tbx_scalingLogoFP.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_scalingLogoFP.JAM_IsForPercent = true;

                          hamper.CreateVvLabel      (2, 1, "Slika2 kao:"      , ContentAlignment.MiddleRight);
      rbt_logoFP_Footer = hamper.CreateVvRadioButton(3, 1, null, "Footer"     , TextImageRelation.ImageBeforeText);
      rbt_logoFP_Potpis = hamper.CreateVvRadioButton(4, 1, null, "Pečat"      , TextImageRelation.ImageBeforeText);
      rbt_logoFP_NE     = hamper.CreateVvRadioButton(5, 1, null, "Ne prikazuj", TextImageRelation.ImageBeforeText);
      rbt_logoFP_Logo2  = hamper.CreateVvRadioButton(5, 0, null, "Logo2"      , TextImageRelation.ImageBeforeText); //15.01.2020.


      SetUpAsWriteOnlyTbx(hamper);

      cbx_ocuLogo = hamper.CreateVvCheckBox_OLD (2, 0, null, "Vidljiv Logo", RightToLeft.No);
      if(TheVvUC is BlgUplatDUC || TheVvUC is BlgIsplatDUC || TheVvUC is BlgUplat_M_DUC || TheVvUC is BlgIsplat_M_DUC)
      {
         hamper.Parent = tabControlPrintFak.TabPages[TabPageTitle8];
         hamper.Location = new Point(nextX, hamp_Blg.Bottom + razmakIzmjedjuHampera);
      }
      VvHamper.HamperStyling(hamper);
   }

   private void CreateHamper_logo2Aligm(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", tabControlPrintFak.TabPages[TabPageTitle1], false, hamp_logo.Right + razmakIzmjedjuHampera, hamp_logo.Bottom - ZXC.QUN - ZXC.Qun4, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q3un + ZXC.Qun8, ZXC.Q3un , ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8 , ZXC.Qun12, ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

                              hamper.CreateVvLabel      (0, 0, "Pozic.Slike2:", ContentAlignment.MiddleRight);
      rbt_alignmentLogoFP_L = hamper.CreateVvRadioButton(1, 0, null, "Lijevo"     , TextImageRelation.ImageBeforeText);
      rbt_alignmentLogoFP_C = hamper.CreateVvRadioButton(2, 0, null, "Centar"     , TextImageRelation.ImageBeforeText);
      rbt_alignmentLogoFP_R = hamper.CreateVvRadioButton(3, 0, null, "Desno"      , TextImageRelation.ImageBeforeText);
    
      SetUpAsWriteOnlyTbx(hamper);
   
      VvHamper.HamperStyling(hamper);
   }


   private void CreateHamper_memo      (out VvHamper hamper)
   {
      hamper = new VvHamper(7, 2, "", tabControlPrintFak.TabPages[TabPageTitle1], false, nextX, hamp_logo.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt = new int[] { ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q4un };

      for(int i = 0; i < hamper.VvNumOfCols; i++)
      {
         hamper.VvSpcBefCol[i] = ZXC.Qun8;
      }
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
     
      hamper.CreateVvLabel(0, 0, "Memorandum:", ContentAlignment.MiddleRight);

      cbx_ocuHeader        = hamper.CreateVvCheckBox_OLD(1, 0, null, "VidljivHeader"  , RightToLeft.No);
      cbx_OcuMemoHAllPages = hamper.CreateVvCheckBox_OLD(2, 0, null, "HeSveStr"       , RightToLeft.No); //vidljiv memoHeader na svim stranicama
      cbx_OcuLinijeHeader  = hamper.CreateVvCheckBox_OLD(3, 0, null, "HeCrte"         , RightToLeft.No);
      cbx_ocuFooter        = hamper.CreateVvCheckBox_OLD(4, 0, null, "VidljivFooter"  , RightToLeft.No);
      cbx_ocuFooter2       = hamper.CreateVvCheckBox_OLD(4, 1, null, "Footer2"        , RightToLeft.No);
      cbx_OcuMemoFAllPages = hamper.CreateVvCheckBox_OLD(5, 0, null, "FoSveStr"       , RightToLeft.No); //vidljiv memoFooter na svim stranicama
      cbx_OcuLinijeFooter  = hamper.CreateVvCheckBox_OLD(6, 0, null, "FoCrte"         , RightToLeft.No);

      cbx_OcuMojuPoslJed   = hamper.CreateVvCheckBox_OLD(1, 1, null, 2, 0,"Print PoslJed ili oznake sklad u zaglavlju", System.Windows.Forms.RightToLeft.No);

      if(TheVvUC is BlgUplatDUC || TheVvUC is BlgIsplatDUC || TheVvUC is BlgUplat_M_DUC || TheVvUC is BlgIsplat_M_DUC)
      {
         hamper.Parent = tabControlPrintFak.TabPages[TabPageTitle8];
      }

      VvHamper.HamperStyling(hamper);

   }
 
   private void CreateHamper_memoAdd(out VvHamper hamper)
   {
      hamper = new VvHamper(7, 1, "", tabControlPrintFak.TabPages[TabPageTitle1], false, hamp_memo.Left, hamp_memo.Bottom + razmakIzmjedjuHampera, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q10un*2 - ZXC.Q3un, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun4, ZXC.Q3un - ZXC.Qun4, ZXC.Q3un - ZXC.Qun4, ZXC.Q3un - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8,               ZXC.Qun8,                   0, ZXC.Qun12, ZXC.Qun12 , ZXC.Qun12, ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

                    hamper.CreateVvLabel  (0, 0, "DodMemo:", ContentAlignment.MiddleRight);
      tbx_memoAdd = hamper.CreateVvTextBox(1, 0, "tbx_memoAdd", "Dodatak Memo");


                              hamper.CreateVvLabel      (3, 0, "Pozicija:"   , ContentAlignment.MiddleRight);
      rbt_alignmentMemAdd_L = hamper.CreateVvRadioButton(4, 0, null, "Lijevo", TextImageRelation.ImageBeforeText);
      rbt_alignmentMemAdd_C = hamper.CreateVvRadioButton(5, 0, null, "Centar", TextImageRelation.ImageBeforeText);
      rbt_alignmentMemAdd_R = hamper.CreateVvRadioButton(6, 0, null, "Desno" , TextImageRelation.ImageBeforeText);

      cbx_Ocu_MemoAddGore = hamper.CreateVvCheckBox_OLD(2, 0, null, "Gore", RightToLeft.No);

      SetUpAsWriteOnlyTbx(hamper);
   
      VvHamper.HamperStyling(hamper);
   }

   private void CreateHamperLeftMargin (out VvHamper hamper)
   {
      hamper = new VvHamper(6, 1, "", tabControlPrintFak.TabPages[TabPageTitle1], false, nextX, hamp_memoAdd.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel      (0, 0, "Lijeva margina:", ContentAlignment.MiddleRight);
      rbt_leftMargin1 = hamper.CreateVvRadioButton(1, 0, null, "1.0cm", TextImageRelation.ImageBeforeText);
      rbt_leftMargin2 = hamper.CreateVvRadioButton(2, 0, null, "1.5cm", TextImageRelation.ImageBeforeText);
      rbt_leftMargin3 = hamper.CreateVvRadioButton(3, 0, null, "2.0cm", TextImageRelation.ImageBeforeText);
      rbt_leftMargin4 = hamper.CreateVvRadioButton(4, 0, null, "2.5cm", TextImageRelation.ImageBeforeText);
      rbt_leftMargin5 = hamper.CreateVvRadioButton(5, 0, null, "3.0cm", TextImageRelation.ImageBeforeText);
      //rbt_leftMargin1.Checked = true;
      //rbt_leftMargin1.Tag     = true;
      if(TheVvUC is BlgUplatDUC || TheVvUC is BlgIsplatDUC || TheVvUC is BlgUplat_M_DUC || TheVvUC is BlgIsplat_M_DUC) hamper.Visible = false;

      VvHamper.HamperStyling(hamper);
   }
   private void CreateHamperRightMargin(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 1, "", tabControlPrintFak.TabPages[TabPageTitle1], false, nextX, hamp_leftMargin.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel      (0, 0, "Desna margina:", ContentAlignment.MiddleRight);
      rbt_rightMargin1 = hamper.CreateVvRadioButton(1, 0, null, "1.0cm", TextImageRelation.ImageBeforeText);
      rbt_rightMargin2 = hamper.CreateVvRadioButton(2, 0, null, "1.5cm", TextImageRelation.ImageBeforeText);
      rbt_rightMargin3 = hamper.CreateVvRadioButton(3, 0, null, "2.0cm", TextImageRelation.ImageBeforeText);
      rbt_rightMargin4 = hamper.CreateVvRadioButton(4, 0, null, "2.5cm", TextImageRelation.ImageBeforeText);
      //rbt_rightMargin1.Checked = true;
      //rbt_rightMargin1.Tag     = true;
      if(TheVvUC is BlgUplatDUC || TheVvUC is BlgIsplatDUC || TheVvUC is BlgUplat_M_DUC || TheVvUC is BlgIsplat_M_DUC) hamper.Visible = false;

      VvHamper.HamperStyling(hamper);
   }

   private void CreateHamper_pageHF    (out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", tabControlPrintFak.TabPages[TabPageTitle1], false, nextX, hamp_rightMargin.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                                 hamper.CreateVvLabel(0, 0, "Ispis Broja stranice:" , ContentAlignment.MiddleRight);
      rbt_printPageNum_PHeader = hamper.CreateVvRadioButton(1, 0, null, "Header"    , TextImageRelation.ImageBeforeText);
      rbt_printPageNum_PFooter = hamper.CreateVvRadioButton(2, 0, null, "Footer"    , TextImageRelation.ImageBeforeText);
      rbt_printPageNum_NO      = hamper.CreateVvRadioButton(3, 0, null, "Ne ispisuj", TextImageRelation.ImageBeforeText);
      //rbt_printPageNum_PHeader.Checked = true;
      //rbt_printPageNum_PHeader.Tag = true;

      VvHamper.HamperStyling(hamper);
   }
   private void CreateHamper_pageLCR   (out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", tabControlPrintFak.TabPages[TabPageTitle1], false, nextX, hamp_pageHF.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Poravnanje Broja Str:", ContentAlignment.MiddleRight);
      rbt_alignmentPageNum_L = hamper.CreateVvRadioButton(1, 0, null, "Lijevo", TextImageRelation.ImageBeforeText);
      rbt_alignmentPageNum_C = hamper.CreateVvRadioButton(2, 0, null, "Centar", TextImageRelation.ImageBeforeText);
      rbt_alignmentPageNum_R = hamper.CreateVvRadioButton(3, 0, null, "Desno", TextImageRelation.ImageBeforeText);
      //rbt_alignmentPageNum_R.Checked = true;
      //rbt_alignmentPageNum_R.Tag = true;

      VvHamper.HamperStyling(hamper);
   }
   private void CreateHamperFontOpis   (out VvHamper hamper)
   {
      hamper = new VvHamper(5, 1, "", tabControlPrintFak.TabPages[TabPageTitle1], false, nextX, hamp_pageLCR.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q2un, ZXC.Q2un, ZXC.Q2un, ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                   hamper.CreateVvLabel(0, 0, "Font Dokumenta:", ContentAlignment.MiddleRight);
      rbt_opis7  = hamper.CreateVvRadioButton(1, 0, null, "7", TextImageRelation.ImageBeforeText);
      rbt_opis8  = hamper.CreateVvRadioButton(2, 0, null, "8", TextImageRelation.ImageBeforeText);
      rbt_opis9  = hamper.CreateVvRadioButton(3, 0, null, "9", TextImageRelation.ImageBeforeText);
      rbt_opis10 = hamper.CreateVvRadioButton(4, 0, null, "10", TextImageRelation.ImageBeforeText);
      //rbt_opis10.Checked = true;
      //rbt_opis10.Tag = true;

      VvHamper.HamperStyling(hamper);
   }
   private void CreateHamperFontBelGr  (out VvHamper hamper)
   {
      hamper = new VvHamper(5, 1, "", tabControlPrintFak.TabPages[TabPageTitle1], false, nextX, hamp_FontOpis.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q2un, ZXC.Q2un, ZXC.Q2un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Font Ispod Tablice:", ContentAlignment.MiddleRight);
      rbt_belGr7  = hamper.CreateVvRadioButton(1, 0, null, "7", TextImageRelation.ImageBeforeText);
      rbt_belGr8  = hamper.CreateVvRadioButton(2, 0, null, "8", TextImageRelation.ImageBeforeText);
      rbt_belGr9  = hamper.CreateVvRadioButton(3, 0, null, "9", TextImageRelation.ImageBeforeText);
      rbt_belGr10 = hamper.CreateVvRadioButton(4, 0, null, "10", TextImageRelation.ImageBeforeText);
      //rbt_belGr10.Checked = true;
      //rbt_belGr10.Tag = true;
      if(TheVvUC is BlgUplatDUC || TheVvUC is BlgIsplatDUC || TheVvUC is BlgUplat_M_DUC || TheVvUC is BlgIsplat_M_DUC) hamper.Visible = false;

      VvHamper.HamperStyling(hamper);

   }


   private void CreateHamper_memoPOS(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 6, "", tabControlPrintFak.TabPages[TabPageTitle1], false, nextX, hamp_FontBelGr.Bottom + ZXC.Qun4, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q10un*2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4             };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8, 0, 0, 0, ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

                    hamper.CreateVvLabel    (0, 0, "POS Memo:", ContentAlignment.MiddleLeft);
      tbx_memoPOS = hamper.CreateVvTextBox  (0, 1, "tbx_memoPOS", "Memo za POS", 2048, 0, 4);
      tbx_memoPOS.Multiline = true;
      tbx_memoPOS.ScrollBars = ScrollBars.Both;
     
      SetUpAsWriteOnlyTbx(hamper);

      VvHamper.HamperStyling(hamper);
   }

   #endregion  Memo/Izgled
   
   #region     Partner       
  
   private void CreateHamperAdres         (out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", tabControlPrintFak.TabPages[TabPageTitle2], false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q5un, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel      (0, 0, "Pozicija Adrese:", ContentAlignment.MiddleRight);
      rbt_AdresLeft  = hamper.CreateVvRadioButton(1, 0, null, "Lijevo", TextImageRelation.ImageBeforeText);
      rbt_AdresRight = hamper.CreateVvRadioButton(2, 0, null, "Desno", TextImageRelation.ImageBeforeText);
      //rbt_AdresRight.Checked = true;
      //rbt_AdresRight.Tag = true;

      VvHamper.HamperStyling(hamper);

   }

   private void CreateHamper_adresOkvir   (out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", tabControlPrintFak.TabPages[TabPageTitle2], false, nextX, hamp_adresLR.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q5un, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel      (0, 0, "OkvirAdrese:", ContentAlignment.MiddleRight);
      rbt_adresOkvir   = hamper.CreateVvRadioButton(1, 0, null, "Da", TextImageRelation.ImageBeforeText);
      rbt_adresNoOkvir = hamper.CreateVvRadioButton(2, 0, null, "Ne", TextImageRelation.ImageBeforeText);
      //rbt_adresNoOkvir.Checked = true;
      //rbt_adresNoOkvir.Tag     = true;

      VvHamper.HamperStyling(hamper);
   }
  
   private void CreateHamper_adresToFrom  (out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", tabControlPrintFak.TabPages[TabPageTitle2], false, nextX, hamp_adresOkvir.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q5un, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                             hamper.CreateVvLabel(0, 0, "Oblik adrese:", ContentAlignment.MiddleRight);
      rbt_adresOnlyPartner = hamper.CreateVvRadioButton(1, 0, null, "Samo partner", TextImageRelation.ImageBeforeText);
      rbt_adresBoth        = hamper.CreateVvRadioButton(2, 0, null, "Obje adrese", TextImageRelation.ImageBeforeText);
      //rbt_adresOnlyPartner.Checked = true;
      //rbt_adresOnlyPartner.Tag = true;

      VvHamper.HamperStyling(hamper);
   }
   private void CreateHamper_adresSastojci(out VvHamper hamper)
   {
      hamper = new VvHamper(7, 2, "", tabControlPrintFak.TabPages[TabPageTitle2], false, nextX, hamp_adresToFrom.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt = new int[] { ZXC.Q4un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un};

      for(int i = 0; i < hamper.VvNumOfCols; i++)
      {
         hamper.VvSpcBefCol[i] = ZXC.Qun8;
      }
      hamper.VvRightMargin = hamper.VvLeftMargin;


      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                          hamper.CreateVvLabel       (0, 0, "Partner:", ContentAlignment.MiddleRight);
      cbx_OcuKupDobTel  = hamper.CreateVvCheckBox_OLD(1, 0, null, "Tel"   , System.Windows.Forms.RightToLeft.No);
      cbx_OcuKupDobFax  = hamper.CreateVvCheckBox_OLD(2, 0, null, "Fax"   , System.Windows.Forms.RightToLeft.No);
      cbx_OcuKupDobOib  = hamper.CreateVvCheckBox_OLD(3, 0, null, "OIB"   , System.Windows.Forms.RightToLeft.No);
      cbx_OcuKupDobMail = hamper.CreateVvCheckBox_OLD(4, 0, null, "e-mail", System.Windows.Forms.RightToLeft.No);
      cbx_OcuKupDobNr   = hamper.CreateVvCheckBox_OLD(5, 0, null, "N/r"   , System.Windows.Forms.RightToLeft.No);
      cbx_OcuKupDobBoja = hamper.CreateVvCheckBox_OLD(6, 0, null, "Boja"  , System.Windows.Forms.RightToLeft.No);

                           hamper.CreateVvLabel       (0, 1, "Projekt:", ContentAlignment.MiddleRight);
      cbx_OcuProjektTel  = hamper.CreateVvCheckBox_OLD(1, 1, null, "Tel"   , System.Windows.Forms.RightToLeft.No);
      cbx_OcuProjektFax  = hamper.CreateVvCheckBox_OLD(2, 1, null, "Fax"   , System.Windows.Forms.RightToLeft.No);
      cbx_OcuProjektOib  = hamper.CreateVvCheckBox_OLD(3, 1, null, "OIB"   , System.Windows.Forms.RightToLeft.No);
      cbx_OcuProjektMail = hamper.CreateVvCheckBox_OLD(4, 1, null, "e-mail", System.Windows.Forms.RightToLeft.No);
      cbx_OcuPrjktBoja   = hamper.CreateVvCheckBox_OLD(6, 1, null, "Boja"  , System.Windows.Forms.RightToLeft.No);

      VvHamper.HamperStyling(hamper);
   }
   private void CreateHamperPoslJed       (out VvHamper hamper)
   {
      hamper = new VvHamper(2, 6, "", tabControlPrintFak.TabPages[TabPageTitle2], false, nextX, hamp_adresSastojci.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q8un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "NazivPoslJed:", ContentAlignment.MiddleRight);
      tbx_nazivPoslJed = hamper.CreateVvTextBox(1, 0, "tbx_PoslJed", "Labela za ispis Poslovne jedinice, npr. Poslovna jed:, Isporučeno: ...");

      cbx_fakAdresKaoPoslJed = hamper.CreateVvCheckBox_OLD(0, 1, null, 1, 0, "Adresa Fakturiranja kao PoslJed", RightToLeft.No);
      cbx_ispisUgovora       = hamper.CreateVvCheckBox_OLD(0, 2, null, 1, 0, "Ispis Ugovora sa Partnera"      , RightToLeft.No);

      cbx_OibIspodAdrese     = hamper.CreateVvCheckBox_OLD(0, 4, null, 1, 0, "OIB ispod adrese SamoPartner"  , RightToLeft.No);
      cbx_OcuKupDobOpis      = hamper.CreateVvCheckBox_OLD(0, 5, null, 1, 0, "Tvrtka - širi opis SamoPartner", RightToLeft.No);

      VvHamper.HamperStyling(hamper);
      SetUpAsWriteOnlyTbx(hamper);
   }

   #endregion  Partner
   
   #region     Račun/Pnb     
   
   private void CreateHamperRnPnb         (out VvHamper hamper)
   {
      hamper = new VvHamper(15, 3, "", tabControlPrintFak.TabPages[TabPageTitle3], false, nextX, nextY, razmakHampGroup);

      //    0        1        2                           
      hamper.VvColWdt = new int[] { ZXC.Q4un, ZXC.Q3un -ZXC.Qun8, ZXC.QUN , 
                                         ZXC.QUN - ZXC.Qun8, ZXC.Q2un + ZXC.Qun2, //  3,  4
                                         ZXC.QUN - ZXC.Qun8, ZXC.Q3un + ZXC.Qun4, //  5,  6
                                         ZXC.QUN - ZXC.Qun8, ZXC.Q3un + ZXC.Qun4, //  7,  8
                                         ZXC.QUN - ZXC.Qun8, ZXC.Q3un + ZXC.Qun4, //  9, 10
                                         ZXC.QUN - ZXC.Qun8, ZXC.Q3un           , // 11, 12
                                         ZXC.QUN - ZXC.Qun8, ZXC.Q3un           };// 13, 14

      hamper.VvSpcBefCol = new int[] { ZXC.Qun2, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, 
                                         ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8,
                                         ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel (0, 0, "NazivPrinta:", ContentAlignment.MiddleLeft);
      tbx_nazivPrinta = hamper.CreateVvTextBox(0, 1, "tbx_nazivPrinta", "Naslov za printanje  dokumenta, npr. Račun, Račun-Otpremnica, Račun-Izdatnica ...", 32, 2, 0);

      cbx_ocuR12 = hamper.CreateVvCheckBox_OLD(1, 0, null, 1, 0, "Ispis R12", System.Windows.Forms.RightToLeft.No);

      cbx_ocuPnb = hamper.CreateVvCheckBox_OLD(0, 2, null, "IspisPnb", System.Windows.Forms.RightToLeft.No);
      hamper.CreateVvLabel(1, 2, "ModPlać:", ContentAlignment.MiddleRight);
      tbx_pnbM = hamper.CreateVvTextBox(2, 2, "tbx_pnbM", "Model plaćnja - mala kućica", 2);
      tbx_pnbM.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      cbx_IsAddTT = hamper.CreateVvCheckBox_OLD(3, 0, null, 1, 0, "TipTrans", System.Windows.Forms.RightToLeft.No);
      tbx_SeparIfTT_Rn = hamper.CreateVvTextBox(3, 1, "tbx_IsAddTT_Rn", "Znakovni Prefix TT za ispis broja računa", 1);


                         hamper.CreateVvLabel  (5, 0, "Prefix 1:", 1, 0, ContentAlignment.MiddleLeft);
      tbx_prefixIR1_Rn = hamper.CreateVvTextBox(5, 1, "tbx_prefixIR1_Rn", "Znakovni Prefix 1 za ispis broja računa", 1);
      tbx_prefixIR1Rn  = hamper.CreateVvTextBox(6, 1, "tbx_prefixIR1", "Prefix 1 za ispis broja računa", 10);
      tbx_prefixIR1_Pn = hamper.CreateVvTextBox(5, 2, "tbx_prefixIR1_Pn", "Znakovni Prefix 1 za ispis poziva na broj za plaćanje", 1);
      tbx_prefixIR1Pb  = hamper.CreateVvTextBox(6, 2, "tbx_prefixIR1Pb", "Prefix 1 za ispis poziva na broj za plaćanje", 10);
      tbx_prefixIR1Pb.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);


                         hamper.CreateVvLabel  (7, 0, "Prefix 2:", 1, 0, ContentAlignment.MiddleLeft);
      tbx_prefixIR2_Rn = hamper.CreateVvTextBox(7, 1, "tbx_prefixIR2_Rn", "Znakovni Prefix 2 za ispis broja računa", 1);
      tbx_prefixIR2Rn  = hamper.CreateVvTextBox(8, 1, "tbx_prefixIR2", "Prefix 2 za ispis broja računa, ako postoji Prefix1 veže se na njega Prefix1-Prefix2", 10);
      tbx_prefixIR2_Pn = hamper.CreateVvTextBox(7, 2, "tbx_prefixIR2_Pn", "Znakovni Prefix 2 za ispis poziva na broj za plaćanje", 1);
      tbx_prefixIR2Pb  = hamper.CreateVvTextBox(8, 2, "tbx_prefixIR2Pb", "Prefix 2 za ispis poziva na broj za plaćanje, ako postoji Prefix1 veže se na njega Prefix1-Prefix2", 10);

                          hamper.CreateVvLabel       ( 9, 0, "Broj dokumenta", 1, 0, ContentAlignment.MiddleLeft);
      tbx_IsAddTtNum_Rn = hamper.CreateVvTextBox     ( 9, 1, "tbx_IsAddTtNum_Rn", "Znakovni Prefix TtNum za ispis broja računa", 1);
      cbx_IsAddTtNumRn  = hamper.CreateVvCheckBox_OLD(10, 1, null, "", System.Windows.Forms.RightToLeft.No);
      tbx_IsAddTtNum_Pn = hamper.CreateVvTextBox     ( 9, 2, "tbx_IsAddTtNum_Pn", "Znakovni Prefix TtNum za ispis poziva na broj za plaćanje", 1);
      cbx_IsAddTtNumPb  = hamper.CreateVvCheckBox_OLD(10, 2, null, "", System.Windows.Forms.RightToLeft.No);

                if(isFakExt) hamper.CreateVvLabel       (11, 0, "Šifra partnera", 1, 0, ContentAlignment.MiddleLeft);
      tbx_IsAddKupDobCd_Rn = hamper.CreateVvTextBox     (11, 1, "tbx_IsAddKupDobCd_Rn", "Znakovni Prefix sifre partnera za ispis broja računa", 1);
      cbx_IsAddKupDobCdRn  = hamper.CreateVvCheckBox_OLD(12, 1, null, "", System.Windows.Forms.RightToLeft.No);
      tbx_IsAddKupDobCd_Pn = hamper.CreateVvTextBox     (11, 2, "tbx_IsAddKupDobCd_Pn", "Znakovni Prefix sifre partnera za ispis poziva na broj za plaćanje", 1);
      cbx_IsAddKupDobCdPb  = hamper.CreateVvCheckBox_OLD(12, 2, null, "", System.Windows.Forms.RightToLeft.No);

                         hamper.CreateVvLabel       (13, 0, "Tekuća godina", 1, 0, ContentAlignment.MiddleLeft);
      tbx_IsAddYear_Rn = hamper.CreateVvTextBox     (13, 1, "tbx_IsAddYear_Rn", "Znakovni Prefix godine za ispis broja računa", 1);
      cbx_IsAddYearRn  = hamper.CreateVvCheckBox_OLD(14, 1, null, "", System.Windows.Forms.RightToLeft.No);
      tbx_IsAddYear_Pn = hamper.CreateVvTextBox     (13, 2, "tbx_IsAddYear_Pn", "Znakovni Prefix godine za ispis poziva na broj za plaćanje", 1);
      cbx_IsAddYearPb  = hamper.CreateVvCheckBox_OLD(14, 2, null, "", System.Windows.Forms.RightToLeft.No);

      cbx_ocuR12.Visible =
      cbx_ocuPnb.Visible =
      tbx_pnbM.Visible =
      tbx_pnbM.Visible =
      tbx_prefixIR1_Pn.Visible =
      tbx_prefixIR1Pb.Visible =
      tbx_prefixIR2_Pn.Visible =
      tbx_prefixIR2Pb.Visible =
      tbx_IsAddTtNum_Pn.Visible =
      cbx_IsAddTtNumPb.Visible =
      tbx_IsAddKupDobCd_Rn.Visible =
      cbx_IsAddKupDobCdRn.Visible =
      tbx_IsAddKupDobCd_Pn.Visible =
      cbx_IsAddKupDobCdPb.Visible =
      tbx_IsAddYear_Pn.Visible =
      cbx_IsAddYearPb.Visible = isFakExt;

      VvHamper.HamperStyling(hamper);
      SetUpAsWriteOnlyTbx(hamper);
   }

   private void CreateHamper_dokNumHFadres(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", tabControlPrintFak.TabPages[TabPageTitle3], false, nextX, hamp_RnPnb.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q5un, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                                   hamper.CreateVvLabel(0, 0, "Ispis Broja Dokumenta:", ContentAlignment.MiddleRight);
      rbt_printDokNum_beforAdres = hamper.CreateVvRadioButton(1, 0, null, "Prije Adrese", TextImageRelation.ImageBeforeText);
      rbt_printDokNum_afterAdres = hamper.CreateVvRadioButton(2, 0, null, "Poslije Adrese", TextImageRelation.ImageBeforeText);
      //rbt_printDokNum_beforAdres.Checked = true;
      //rbt_printDokNum_beforAdres.Tag = true;

      VvHamper.HamperStyling(hamper);
   }
   private void CreateHamper_dokNumLCR    (out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", tabControlPrintFak.TabPages[TabPageTitle3], false, nextX, hamp_dokNumHFadres.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Poravnanje Broja Dokumenta:", ContentAlignment.MiddleRight);
      rbt_alignmentDokNum_L = hamper.CreateVvRadioButton(1, 0, null, "Lijevo", TextImageRelation.ImageBeforeText);
      rbt_alignmentDokNum_C = hamper.CreateVvRadioButton(2, 0, null, "Centar", TextImageRelation.ImageBeforeText);
      rbt_alignmentDokNum_R = hamper.CreateVvRadioButton(3, 0, null, "Desno", TextImageRelation.ImageBeforeText);
      //rbt_alignmentDokNum_L.Checked = true;
      //rbt_alignmentDokNum_L.Tag = true;

      VvHamper.HamperStyling(hamper);
   }
   private void CreateHamper_dokNumTitle  (out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", tabControlPrintFak.TabPages[TabPageTitle3], false, nextX, hamp_dokNumLCR.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      for(int i = 0; i < hamper.VvNumOfCols; i++)
      {
         hamper.VvColWdt[i]    = ZXC.Q10un + ZXC.Q5un;
         hamper.VvSpcBefCol[i] = ZXC.Qun4;
      }
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbx_OcuTitleOkvir   = hamper.CreateVvCheckBox_OLD(0, 0, null, "Okvir na Broju Dokumenta"               , RightToLeft.No);
      cbx_OcuTitleBoja    = hamper.CreateVvCheckBox_OLD(0, 1, null, "Boja na Broju Dokumenta"                , RightToLeft.No);
      cbx_OcuIspisDokNum2 = hamper.CreateVvCheckBox_OLD(0, 2, null, "Ponovljeni Broj Dokumenta (SamoPartner)", RightToLeft.No); // jos jednom ponovljeni broj racuna
    
      VvHamper.HamperStyling(hamper);

   }

   #endregion  Račun/Pnb
   
   #region     Kolone        
  
   private void CreateHamperColumns(out VvHamper hamper)
   {
//      hamper = new VvHamper(16, 6, "", tabControlPrintFak.TabPages[TabPageTitle4], false, nextX, nextY, razmakHampGroup);
      hamper = new VvHamper(14, 7, "", tabControlPrintFak.TabPages[TabPageTitle4], false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt = new int[] {ZXC.Q4un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.Q4un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, 
                                   ZXC.Q4un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.Q4un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4,
                                   ZXC.Q4un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.Q4un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4,
                                   ZXC.Q3un + ZXC.Qun4, ZXC.QUN - ZXC.Qun4//, ZXC.Q4un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4
      };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun2         ,            ZXC.Qun10, ZXC.QUN - ZXC.Qun2,           ZXC.Qun10,
                                       ZXC.QUN - ZXC.Qun2,           ZXC.Qun10, ZXC.QUN - ZXC.Qun2,           ZXC.Qun10,
                                       ZXC.QUN - ZXC.Qun2,           ZXC.Qun10, ZXC.QUN - ZXC.Qun2,           ZXC.Qun10,
                                       ZXC.QUN - ZXC.Qun2,           ZXC.Qun10//, ZXC.QUN - ZXC.Qun4,           ZXC.Qun10 
      };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvRowHgt[0]    = ZXC.QUN - ZXC.Qun8;
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Odabir kolona:", 3, 0, ContentAlignment.MiddleLeft);

      cbx_T_artiklCD    = hamper.CreateVvCheckBox_OLD( 0, 1, null, "ŠifraArt" , System.Windows.Forms.RightToLeft.No);
      cbx_T_artiklName  = hamper.CreateVvCheckBox_OLD( 2, 1, null, "NazivArt" , System.Windows.Forms.RightToLeft.No);
      cbx_Napomena      = hamper.CreateVvCheckBox_OLD( 4, 1, null, "NapSaArt" , System.Windows.Forms.RightToLeft.No);
      cbx_BarCode1      = hamper.CreateVvCheckBox_OLD( 6, 1, null, "BarKod1"  , System.Windows.Forms.RightToLeft.No);
      cbx_ArtiklCD2     = hamper.CreateVvCheckBox_OLD( 8, 1, null, "ŠifraArt2", System.Windows.Forms.RightToLeft.No);
      cbx_ArtiklName2   = hamper.CreateVvCheckBox_OLD(10, 1, null, "NazivArt2", System.Windows.Forms.RightToLeft.No);
      cbx_BarCode2      = hamper.CreateVvCheckBox_OLD(12, 1, null, "BarKod2"  , System.Windows.Forms.RightToLeft.No);
      
      cbx_LongOpis      = hamper.CreateVvCheckBox_OLD( 0, 2, null, "DugiOpis" , System.Windows.Forms.RightToLeft.No);
      cbx_SerNo         = hamper.CreateVvCheckBox_OLD( 2, 2, null, "SerBroj"  , System.Windows.Forms.RightToLeft.No);
      cbx_T_jedMj       = hamper.CreateVvCheckBox_OLD( 4, 2, null, "JedMj"    , System.Windows.Forms.RightToLeft.No);
      cbx_T_kol         = hamper.CreateVvCheckBox_OLD( 6, 2, null, "Količina" , System.Windows.Forms.RightToLeft.No);
      cbx_T_cij         = hamper.CreateVvCheckBox_OLD( 8, 2, null, "Cijena"   , System.Windows.Forms.RightToLeft.No);
      cbx_R_KC          = hamper.CreateVvCheckBox_OLD(10, 2, null, "Kol*Cij"  , System.Windows.Forms.RightToLeft.No);
      cbx_T_rbt1St      = hamper.CreateVvCheckBox_OLD(12, 2, null, "StRbt1"   , System.Windows.Forms.RightToLeft.No);
      
      cbx_R_rbt1        = hamper.CreateVvCheckBox_OLD( 0, 3, null, "IznosRbt1", System.Windows.Forms.RightToLeft.No);
      cbx_T_rbt2St      = hamper.CreateVvCheckBox_OLD( 2, 3, null, "StRbt2"   , System.Windows.Forms.RightToLeft.No);
      cbx_R_rbt2        = hamper.CreateVvCheckBox_OLD( 4, 3, null, "IznosRbt2", System.Windows.Forms.RightToLeft.No);
      cbx_R_cij_KCR     = hamper.CreateVvCheckBox_OLD( 6, 3, null, "CijSaRbt" , System.Windows.Forms.RightToLeft.No);
      cbx_R_KCR         = hamper.CreateVvCheckBox_OLD( 8, 3, null, "IznSaRbt" , System.Windows.Forms.RightToLeft.No);
      cbx_T_pdvSt       = hamper.CreateVvCheckBox_OLD(10, 3, null, "StPdv"    , System.Windows.Forms.RightToLeft.No);
      cbx_R_pdv         = hamper.CreateVvCheckBox_OLD(12, 3, null, "IznosPdv" , System.Windows.Forms.RightToLeft.No);
     
      cbx_R_cij_KCRP    = hamper.CreateVvCheckBox_OLD( 0, 4, null, "CijSaPdv" , System.Windows.Forms.RightToLeft.No);
      cbx_R_KCRP        = hamper.CreateVvCheckBox_OLD( 2, 4, null, "IznSaPdv" , System.Windows.Forms.RightToLeft.No);
      cbx_T_doCijMal    = hamper.CreateVvCheckBox_OLD( 4, 4, null, "TipArtikla", System.Windows.Forms.RightToLeft.No);//za Tip Artikl TS

    // 11.05.2020. za theVPC
    //cbx_T_noCijMal    = hamper.CreateVvCheckBox_OLD( 6, 4, null, ""         , System.Windows.Forms.RightToLeft.No); //"NoCijMal"
      cbx_T_noCijMal    = hamper.CreateVvCheckBox_OLD( 0, 6, null, 2, 0, "Ispis VPC cijene na IRM", System.Windows.Forms.RightToLeft.No);

      cbx_R_mjMasaN     = hamper.CreateVvCheckBox_OLD( 6, 4, null, "Težina"   , System.Windows.Forms.RightToLeft.No);
      cbx_T_garancija   = hamper.CreateVvCheckBox_OLD( 8, 4, null, "MjJamstva", System.Windows.Forms.RightToLeft.No);

      tbx_NumDecT_kol      = hamp_Columns.CreateVvTextBox( 7, 2, "tbx_NumDecT_kol"     , "Željeni broj decimala za pripadajuću kolonu", 1);
      tbx_NumDecT_cij      = hamp_Columns.CreateVvTextBox( 9, 2, "tbx_NumDecT_cij"     , "Željeni broj decimala za pripadajuću kolonu", 1);
      tbx_NumDecR_KC       = hamp_Columns.CreateVvTextBox(11, 2, "tbx_NumDecR_KC"      , "Željeni broj decimala za pripadajuću kolonu", 1);
      tbx_NumDecT_rbt1St   = hamp_Columns.CreateVvTextBox(13, 2, "tbx_NumDecT_rbt1St"  , "Željeni broj decimala za pripadajuću kolonu", 1);
      
      tbx_NumDecR_rbt1     = hamp_Columns.CreateVvTextBox( 1, 3, "tbx_NumDecR_rbt1"    , "Željeni broj decimala za pripadajuću kolonu", 1);
      tbx_NumDecT_rbt2St   = hamp_Columns.CreateVvTextBox( 3, 3, "tbx_NumDecT_rbt2St"  , "Željeni broj decimala za pripadajuću kolonu", 1);
      tbx_NumDecR_rbt2     = hamp_Columns.CreateVvTextBox( 5, 3, "tbx_NumDecR_rbt2"    , "Željeni broj decimala za pripadajuću kolonu", 1);
      tbx_NumDecR_cij_KCR  = hamp_Columns.CreateVvTextBox( 7, 3, "tbx_NumDecR_cij_KCR" , "Željeni broj decimala za pripadajuću kolonu", 1);
      tbx_NumDecR_KCR      = hamp_Columns.CreateVvTextBox( 9, 3, "tbx_NumDecR_KCR"     , "Željeni broj decimala za pripadajuću kolonu", 1);
      tbx_NumDecT_pdvSt    = hamp_Columns.CreateVvTextBox(11, 3, "tbx_NumDecT_pdvSt"   , "Željeni broj decimala za pripadajuću kolonu", 1);
      tbx_NumDecR_pdv      = hamp_Columns.CreateVvTextBox(13, 3, "tbx_NumDecR_pdv"     , "Željeni broj decimala za pripadajuću kolonu", 1);
      
      tbx_NumDecR_cij_KCRP = hamp_Columns.CreateVvTextBox( 1, 4, "tbx_NumDecR_cij_KCRP", "Željeni broj decimala za pripadajuću kolonu", 1);
      tbx_NumDecR_KCRP     = hamp_Columns.CreateVvTextBox( 3, 4, "tbx_NumDecR_KCRP"    , "Željeni broj decimala za pripadajuću kolonu", 1);
      tbx_NumDecT_doCijMal = hamp_Columns.CreateVvTextBox( 5, 4, "tbx_NumDecT_doCijMal", "Željeni broj decimala za pripadajuću kolonu", 1);

    //11.05.2020.
    //tbx_NumDecT_noCijMal = hamp_Columns.CreateVvTextBox( 7, 4, "tbx_NumDecT_noCijMal", "Željeni broj decimala za pripadajuću kolonu", 1);
      tbx_NumDecT_noCijMal = hamp_Columns.CreateVvTextBox( 3, 6, "tbx_NumDecT_noCijMal", "Željeni broj decimala za pripadajuću kolonu", 1);

      // 29.08.2013. upotrebljeno za Tkn_Cij i Rkn_KCRP + cijena i iznosi u kunama kada je faktura devizna
      cbx_R_cij_KCRM       = hamper.CreateVvCheckBox_OLD (10, 4, null, "CijHRK"              , System.Windows.Forms.RightToLeft.No);
      cbx_R_KCRM           = hamper.CreateVvCheckBox_OLD (12, 4, null, "IznHRK"              , System.Windows.Forms.RightToLeft.No);
      tbx_NumDecR_cij_KCRM = hamp_Columns.CreateVvTextBox(11, 4,       "tbx_NumDecR_cij_KCRM", "Željeni broj decimala za pripadajuću kolonu", 1);
      tbx_NumDecR_KCRM     = hamp_Columns.CreateVvTextBox(13, 4,       "tbx_NumDecR_KCRM"    , "Željeni broj decimala za pripadajuću kolonu", 1);

      cbx_R_mrz       = hamper.CreateVvCheckBox_OLD (0, 5, null, "CijOP", System.Windows.Forms.RightToLeft.No);                     // 18.12.2013. za R_cijOP
      tbx_NumDecR_mrz = hamp_Columns.CreateVvTextBox(1, 5, "tbx_NumDecR_mrz", "Željeni broj decimala za pripadajuću kolonu", 1);    // 18.12.2013. za R_cijOP
 
      cbx_R_ztr        = hamper.CreateVvCheckBox_OLD( 4, 6, null, "Ispis NBC ", System.Windows.Forms.RightToLeft.No);
      tbx_NumDecR_ztr = hamp_Columns.CreateVvTextBox(13, 3, "tbx_NumDecR_ztr", "Željeni broj decimala za pripadajuću kolonu", 1);


      // za FUSE 10.07.2012. ovo je nepotrebno i stvara greske ak se upotrebljava jer nema ih u CR_ dakle fuse za nekaj drugo - dsc postoji pa se ne mora nadopunjavati
      tbx_NumDecT_mrzSt = hamp_Columns.CreateVvTextBox(7 , 3, "tbx_NumDecT_mrzSt", "Željeni broj decimala za pripadajuću kolonu", 1);
      // 10.07.2012. ovo je nepotrebno i stvara greske ak se upotrebljava jer nema ih u CR_ dakle fuse za nekaj drugo - dsc postoji pa se ne mora nadopunjavati
      //26.02.2016. Serlot
      cbx_T_mrzSt = hamper.CreateVvCheckBox_OLD(2, 5, null, "Serlot", System.Windows.Forms.RightToLeft.No);


      tbx_NumDecT_kol      .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecT_cij      .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecR_KC       .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecT_rbt1St   .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecR_rbt1     .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecT_rbt2St   .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecR_rbt2     .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecR_cij_KCR  .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecR_KCR      .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecT_mrzSt    .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecR_mrz      .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecR_cij_KCRM .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecR_KCRM     .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecR_ztr      .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecT_pdvSt    .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecR_pdv      .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecR_cij_KCRP .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecR_KCRP     .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecT_doCijMal .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_NumDecT_noCijMal .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);

      cbx_R_mjMasaN        .Visible =
      cbx_T_garancija      .Visible =
      cbx_T_rbt1St         .Visible =
      cbx_R_rbt1           .Visible =
      cbx_T_rbt2St         .Visible =
      cbx_R_rbt2           .Visible =
      cbx_R_cij_KCR        .Visible =
      cbx_R_KCR            .Visible =
    // 10.07.2012. ovo je nepotrebno i stvara greske ak se upotrebljava jer nema ih u CR_ dakle fuse za nekaj drugo - dsc postoji pa se ne mora nadopunjavati
      cbx_R_mrz            .Visible =          // 18.12.2013.
      cbx_R_cij_KCRM       .Visible =
      cbx_R_KCRM           .Visible =          // 29.08.2013. upotrebljeno za Tkn_Cij i Rkn_KCRP + cijena i iznosi u kunama kada je faktura devizna
      cbx_R_ztr            .Visible =           // 30.03.2021. nbc
      cbx_T_pdvSt          .Visible =
      cbx_R_pdv            .Visible =
      cbx_R_cij_KCRP       .Visible =
      cbx_R_KCRP           .Visible =
      cbx_T_doCijMal       .Visible =
      cbx_T_noCijMal       .Visible =
      tbx_NumDecT_rbt1St   .Visible =
      tbx_NumDecR_rbt1     .Visible =
      tbx_NumDecT_rbt2St   .Visible =
      tbx_NumDecR_rbt2     .Visible =
      tbx_NumDecR_cij_KCR  .Visible =
      tbx_NumDecR_KCR      .Visible =
      tbx_NumDecT_pdvSt    .Visible =
      tbx_NumDecR_pdv      .Visible =
      tbx_NumDecR_cij_KCRP .Visible =
      tbx_NumDecR_KCRP     .Visible =
      tbx_NumDecR_cij_KCRM .Visible =
      tbx_NumDecR_mrz      .Visible = 
      tbx_NumDecR_KCRM     .Visible =
      tbx_NumDecT_doCijMal .Visible =
      tbx_NumDecT_noCijMal .Visible = isFakExt;

      cbx_T_mrzSt          .Visible = !isFakExt; //26.02.2016. serlot

      tbx_NumDecT_doCijMal.Visible = false; // 23.05.2012. ovo nije sluyilo nicemu

    // 11.05.2020.
    //tbx_NumDecT_noCijMal.Visible = false;
    //cbx_T_noCijMal.Visible       = false;

      cbx_T_doCijMal.Visible       = true ;

      // 10.07.2012. ovo je nepotrebno i stvara greske ak se upotrebljava jer nema ih u CR_ dakle fuse za nekaj drugo - dsc postoji pa se ne mora nadopunjavati
      tbx_NumDecT_mrzSt.Visible = false;

      VvHamper.HamperStyling(hamper);
      SetUpAsWriteOnlyTbx(hamper);
   }
   private void CreateHamperFontCol(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 1, "", tabControlPrintFak.TabPages[TabPageTitle4], false, nextX, hamp_Columns.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un + ZXC.Qun2, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun2 + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "FontKolona:", ContentAlignment.MiddleRight);
      rbt_col7 = hamper.CreateVvRadioButton(1, 0, null, "7", TextImageRelation.ImageBeforeText);
      rbt_col8 = hamper.CreateVvRadioButton(2, 0, null, "8", TextImageRelation.ImageBeforeText);
      rbt_col9 = hamper.CreateVvRadioButton(3, 0, null, "9", TextImageRelation.ImageBeforeText);
      rbt_col10 = hamper.CreateVvRadioButton(4, 0, null, "10", TextImageRelation.ImageBeforeText);
      //rbt_col10.Checked = true;
      //rbt_col10.Tag = true;

      VvHamper.HamperStyling(hamper);

   }

   private void CreateHampCbxTablica(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 10, "", tabControlPrintFak.TabPages[TabPageTitle4], false, nextX, hamp_FontCol.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      for(int i = 0; i < hamper.VvNumOfCols; i++)
      {
         hamper.VvColWdt[i]    = ZXC.Q10un  + ZXC.Q5un;
         hamper.VvSpcBefCol[i] = ZXC.Qun4;
      }
      hamper.VvRightMargin = hamper.VvLeftMargin;
      
      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN - ZXC.Qun8;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
     
                             hamper.CreateVvLabel       (0, 0,       "TABLICA:"              , ContentAlignment.MiddleLeft);
      cb_Line              = hamper.CreateVvCheckBox_OLD(0, 1, null, "Horizontalne Crte"     , RightToLeft.No);
      cbx_verLine          = hamper.CreateVvCheckBox_OLD(0, 2, null, "Vertikalni Okvir"      , RightToLeft.No);
      cbx_TableBorder      = hamper.CreateVvCheckBox_OLD(0, 3, null, "Sivi Naslov"           , RightToLeft.No);
      cbx_tekstualIznos    = hamper.CreateVvCheckBox_OLD(0, 4, null, "Tekstualni Iznos"      , RightToLeft.No);
      cbx_razmak           = hamper.CreateVvCheckBox_OLD(0, 5, null, "Veći Razmak Redova"    , RightToLeft.No);
      cbx_ocuTitulu        = hamper.CreateVvCheckBox_OLD(0, 6, null, "Veći Sveukupno"        , RightToLeft.No);  // veci za 2 i deblji sveukupno 
      cbx_textOpisDrugiRed = hamper.CreateVvCheckBox_OLD(0, 7, null, "Jednaki Font u 2.Redu" , RightToLeft.No);  // jednaki font za redak bez kolicine i cijene
      cbx_onlyUkupnaTezina = hamper.CreateVvCheckBox_OLD(0, 8, null, "Ispis Ukupne Težine"   , RightToLeft.No);

      cbx_OcuDevCijAndDevTec = hamper.CreateVvCheckBox_OLD(0, 9, null, "Ispis DevCijene i DevTecaja u redku tablice", RightToLeft.No);

      if(TheVvUC is BlgIsplatDUC || TheVvUC is BlgUplatDUC || TheVvUC is BlgUplat_M_DUC || TheVvUC is BlgIsplat_M_DUC)
      {
         cbx_OcuDevCijAndDevTec.Visible = 
         cbx_onlyUkupnaTezina       .Visible =
         cbx_ocuTitulu              .Visible =
         cbx_verLine                .Visible =
         //cbx_tekstualIznos          .Visible =
         cbx_razmak                 .Visible =
         cbx_textOpisDrugiRed       .Visible =false;

         cbx_tekstualIznos.Text = "Bez dvostrukih crta okvira";
      }

      if(isFakExt == false)
      {
         cbx_OcuDevCijAndDevTec.Visible = 
         cbx_ocuTitulu         .Visible =
         cbx_razmak            .Visible =
         cbx_textOpisDrugiRed  .Visible = false;
      }

      if(TheVvUC is BlgUplatDUC || TheVvUC is BlgIsplatDUC || TheVvUC is BlgUplat_M_DUC || TheVvUC is BlgIsplat_M_DUC)
      {
         hamper.Parent   = tabControlPrintFak.TabPages[TabPageTitle8];
         hamper.Location = new Point(nextX, hamp_memo.Bottom + razmakIzmjedjuHampera);
      }

      VvHamper.HamperStyling(hamper);
   }

   #endregion  Kolone

   #region     OsobeVeze     
 
   private void CreateHamperPersoni (out VvHamper hamper)
   {
      hamper = new VvHamper(6, 5, "", tabControlPrintFak.TabPages[TabPageTitle5], false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt    = new int[] { ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun4, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };

      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt = new int[] { ZXC.QUN - ZXC.Qun4, ZXC.QUN, ZXC.Qun4, ZXC.QUN, ZXC.QUN };

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbx_signPrimaoc = hamper.CreateVvCheckBox_OLD(0, 0, null, "Primio", System.Windows.Forms.RightToLeft.No);
      tbx_Primio      = hamper.CreateVvTextBox(0, 1, "tbx_Primio", "Labela za  npr. Robu preuzeo:, Primio:, ...");

      hamper.CreateVvLabel(1, 0, "Odgovorna osoba:", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(2, 0, "Ime User-a:"     , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(3, 0, "Putnik:"         , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(4, 0, "Osoba B:"        , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(5, 0, "Osoba X:"        , ContentAlignment.MiddleLeft);

      tbx_lblPrjktPerson = hamper.CreateVvTextBox(1, 1, "tbx_lblPrjktPerson", "Labela za odgovornu osobu navedena na projektu ,  npr. Direktor:, Odgovorna osoba:  ...");
      tbx_lblUserPerson  = hamper.CreateVvTextBox(2, 1, "tbx_lblUserPerson" , "Labela za usera logiranog u program ,  npr. Dokument sastavio:, Fakturirao:, Sastavio: ...");
      tbx_lblOsobaA      = hamper.CreateVvTextBox(3, 1, "tbx_lblOsobaA"     , "Labela za osobuA,  npr. Dokument sastavio:, Fakturirao:, Sastavio: ...");
      tbx_lblOsobaB      = hamper.CreateVvTextBox(4, 1, "tbx_lblOsobaB"     , "Labela za osobuB ,  npr. Dokument sastavio:, Fakturirao:, Sastavio: ...");
      tbx_LblOsobaX      = hamper.CreateVvTextBox(5, 1, "tbx_LblOsobaX"     , "Labela za osobuX ,  npr. Dokument sastavio:, Fakturirao:, Sastavio: ...");

      cbx_ocuLinijePerson = hamper.CreateVvCheckBox_OLD(0, 3, null, "Ispis Crta"     , System.Windows.Forms.RightToLeft.No);
      cbx_ocuFirmuUpotpis = hamper.CreateVvCheckBox_OLD(1, 3, null, "Naziv Poduzeća" , System.Windows.Forms.RightToLeft.No);

                         hamper.CreateVvLabel      (0, 4, "Poravnanje", ContentAlignment.MiddleRight);
      rbt_personRight  = hamper.CreateVvRadioButton(1, 4, null, "Desno" , TextImageRelation.ImageBeforeText);
      rbt_personCentar = hamper.CreateVvRadioButton(2, 4, null, "Centar", TextImageRelation.ImageBeforeText);
      //rbt_personRight.Checked = true;
      //rbt_personRight.Tag     = true;
         
      VvHamper.HamperStyling(hamper);
      SetUpAsWriteOnlyTbx(hamper);
   }
   private void CreateHampOstalo    (out VvHamper hamper)
   {
      hamper = new VvHamper(7, 4, "", tabControlPrintFak.TabPages[TabPageTitle5], false, nextX, hamp_personi.Bottom + ZXC.Qun4, razmakHampGroup);

      for(int i = 0; i < hamper.VvNumOfCols; i++)
      {
         hamper.VvColWdt[i] = ZXC.Q5un;
         hamper.VvSpcBefCol[i] = ZXC.Qun8;
      }
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt = new int[] { ZXC.QUN - ZXC.Qun4, ZXC.QUN, ZXC.QUN - ZXC.Qun4, ZXC.QUN };

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
    //     hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvSpcBefRow[2] = ZXC.QUN;

      hamper.VvBottomMargin = hamper.VvTopMargin;


                  hamper.CreateVvLabel  (0, 0, "LabelaOpćiA", ContentAlignment.MiddleLeft);
      tbx_opciA = hamper.CreateVvTextBox(0, 1, "tbx_opciA", "Labela za Opći A");

                  hamper.CreateVvLabel  (1, 0, "LabelaOpćiB", ContentAlignment.MiddleLeft);
      tbx_opciB = hamper.CreateVvTextBox(1, 1, "tbx_opciB", "Labela za Opći A");

      cbx_OcuIspisVezDok2  = hamper.CreateVvCheckBox_OLD(2, 0, null, "VezDok2", System.Windows.Forms.RightToLeft.No);
      cbx_OcuIspisVeze1    = hamper.CreateVvCheckBox_OLD(3, 0, null, "Veza1"  , System.Windows.Forms.RightToLeft.No);
      cbx_OcuIspisVeze2    = hamper.CreateVvCheckBox_OLD(4, 0, null, "Veza2"  , System.Windows.Forms.RightToLeft.No);
      cbx_OcuIspisVeze3    = hamper.CreateVvCheckBox_OLD(5, 0, null, "Veza3"  , System.Windows.Forms.RightToLeft.No);
      cbx_OcuIspisVeze4    = hamper.CreateVvCheckBox_OLD(6, 0, null, "Veza4"  , System.Windows.Forms.RightToLeft.No);

      tbx_LblVezDok2 = hamper.CreateVvTextBox(2, 1, "tbx_LblVezDok2" , "Labela za VezDok2");
      tbx_LblVeze1   = hamper.CreateVvTextBox(3, 1, "tbx_LblVeze1"   , "Labela za Veza1  ");
      tbx_LblVeze2   = hamper.CreateVvTextBox(4, 1, "tbx_LblVeze2"   , "Labela za Veza2  ");
      tbx_LblVeze3   = hamper.CreateVvTextBox(5, 1, "tbx_LblVeze3"   , "Labela za Veza3  ");
      tbx_LblVeze4   = hamper.CreateVvTextBox(6, 1, "tbx_LblVeze4"   , "Labela za Veza4  ");

      cbx_OcuDateX = hamper.CreateVvCheckBox_OLD(0, 2, null, "DatumX", System.Windows.Forms.RightToLeft.No);
      tbx_lblDateX = hamper.CreateVvTextBox     (0, 3,       "tbx_lblDateX", "Labela za Datumx");

      VvHamper.HamperStyling(hamper);
      SetUpAsWriteOnlyTbx(hamper);
   }

   #endregion  OsobeVeze

   #region     TekstDopune   

   private void CreateHamperBelowGrid(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 10, "", tabControlPrintFak.TabPages[TabPageTitle6], false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q10un*3 + ZXC.Q7un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN,  ZXC.QUN,  ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8,       0, ZXC.Qun4,       0,       0,       0,       0,       0, ZXC.Qun4,      0 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      lbl_textAbovGr = hamper.CreateVvLabel  (0, 0, "Tekst iznad tablice:", ContentAlignment.MiddleLeft);
      tbx_AboveGrid  = hamper.CreateVvTextBox(0, 1, "tbx_AboveGrid", "Tekst koji se vidi iznad tablice", 2048);
      tbx_AboveGrid.Multiline = true;
      tbx_AboveGrid.ScrollBars = ScrollBars.Both;
         
                      hamper.CreateVvLabel  (0, 2, "Tekst ispod tablice:", ContentAlignment.MiddleLeft);
      tbx_belowGrid = hamper.CreateVvTextBox(0, 3, "tbx_belowGrid", "Tekst koji se vidi ispod tablice", 2048, 0, 4);
      tbx_belowGrid.Multiline = true;
      tbx_belowGrid.ScrollBars = ScrollBars.Both;
      //tbx_belowGrid.ScrollBars = ScrollBars.Horizontal;

                       hamper.CreateVvLabel  (0, 8, "POS tekst:", ContentAlignment.MiddleLeft);
      tbx_belowOnPOS = hamper.CreateVvTextBox(0, 9, "tbx_belowOnPOS", "Tekst koji se vidi ispod tablice", 2048);
      tbx_belowOnPOS.Multiline = true;
      tbx_belowOnPOS.ScrollBars = ScrollBars.Both;

      SetUpAsWriteOnlyTbx(hamper);

      if(isFakExt == false)
      {
         lbl_textAbovGr.Visible =
         tbx_AboveGrid .Visible = false;
      }


      VvHamper.HamperStyling(hamper);

   }
  
   private void CreateHamperNRD     (out VvHamper hamper)
   {
      hamper = new VvHamper(2, 2, "", tabControlPrintFak.TabPages[TabPageTitle6], false, nextX, hamp_belowGrid.Bottom + ZXC.Qun4, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un,   ZXC.Q10un*3+ZXC.Q2un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8,           ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN};
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbx_PotvrdaNarudzbe = hamper.CreateVvCheckBox_OLD(0, 0, null, "PotvrdaNarudžbe", RightToLeft.No);

      tbx_beforNRD = hamper.CreateVvTextBox(1, 0, "tbx_beforNRD", "Tekst koji se vidi iznad potpisa", 2048);
      tbx_beforNRD.Multiline = true;
      tbx_beforNRD.ScrollBars = ScrollBars.Both;

                     hamper.CreateVvLabel  (0, 1, "TekstNapomena:", ContentAlignment.MiddleLeft);
      tbx_afterNRD = hamper.CreateVvTextBox(1, 1, "tbx_afterNRD", "Tekst koji se vidi ispod potpisa", 2048);
      tbx_afterNRD.Multiline = true;
      tbx_afterNRD.ScrollBars = ScrollBars.Both;

      SetUpAsWriteOnlyTbx(hamper);

      VvHamper.HamperStyling(hamper);

   }

   private void CreateHamperPostotak (out VvHamper hamper)
   {
      hamper = new VvHamper(2, 2, "", tabControlPrintFak.TabPages[TabPageTitle6], false, nextX, hamp_NRD.Bottom + ZXC.Qun4, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un,   ZXC.Q10un*3+ZXC.Q2un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8,           ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN};
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Tekst prije postotka:", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(0, 1, "Tekst poslije postot.:", ContentAlignment.MiddleLeft);

      tbx_postoBefore = hamper.CreateVvTextBox(1, 0, "tbx_postoBefore", "Tekst koji se vidi prije postotka", 2048);
      tbx_postoBefore.Multiline = true;
      tbx_postoBefore.ScrollBars = ScrollBars.Both;

      tbx_postoAfter = hamper.CreateVvTextBox(1, 1, "tbx_postoAfter", "Tekst koji se vidi nakon postotka", 2048);
      tbx_postoAfter.Multiline = true;
      tbx_postoAfter.ScrollBars = ScrollBars.Both;

      SetUpAsWriteOnlyTbx(hamper);

      VvHamper.HamperStyling(hamper);

   }

   private void CreateHamperOslobodenjePDV(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", tabControlPrintFak.TabPages[TabPageTitle6], false, nextX, hamp_postotakOpis.Bottom + ZXC.Qun4, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q10un*3 + ZXC.Q7un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN ,  ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8,        0,        0 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      lbl_oslobodenjePDV  = hamper.CreateVvLabel  (0, 0, "Oslobođeno PDV-a:", ContentAlignment.MiddleLeft);
      tbx_oslobodenjePDV  = hamper.CreateVvTextBox(0, 1, "tbx_oslobodenjePDV", "Oslobođeno PDV-a", 2048, 0, 1);
      tbx_oslobodenjePDV.Multiline = true;
      tbx_oslobodenjePDV.ScrollBars = ScrollBars.Both;
         
      SetUpAsWriteOnlyTbx(hamper);

      if(isFakExt == false)
      {
         lbl_oslobodenjePDV.Visible =
         tbx_oslobodenjePDV.Visible = false;
      }

      VvHamper.HamperStyling(hamper);

   }

   #endregion  TekstDopune

   #region     Ostalo        

   private void CreateHamperValuta  (out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", tabControlPrintFak.TabPages[TabPageTitle7], false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                    hamper.CreateVvLabel(0, 0, "IznosUValuti:", ContentAlignment.MiddleRight);
      tbx_ValName = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_ValName", "Naziv devizne valute u kojoj će se prikazati dokument koji se printa");
      tbx_ValName.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_ValName.JAM_Set_LookUpTable(ZXC.luiListaDeviza, (int)ZXC.Kolona.prva);


      SetUpAsWriteOnlyTbx(hamper);
      VvHamper.HamperStyling(hamper);

   }
   private void CreateHamperJezik   (out VvHamper hamper)
   {
      hamper = new VvHamper(2, 5, "", tabControlPrintFak.TabPages[TabPageTitle7], false, nextX, hamp_valuta.Bottom + razmakIzmjedjuHampera, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.QUN};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                 hamper.CreateVvLabel      (0, 0, "Izbor jezika", 1, 0, ContentAlignment.MiddleLeft);
      rbt_hrv  = hamper.CreateVvRadioButton(0, 1, null, "HRV", TextImageRelation.ImageBeforeText);
      rbt_eng  = hamper.CreateVvRadioButton(0, 2, null, "ENG", TextImageRelation.ImageBeforeText);
      rbt_njem = hamper.CreateVvRadioButton(0, 3, null, "C", TextImageRelation.ImageBeforeText);
      rbt_tal  = hamper.CreateVvRadioButton(0, 4, null, "D", TextImageRelation.ImageBeforeText);
      rbt_hrv.Checked = true;
      rbt_hrv.Tag = true;

      tbx_hrv  = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_hrv" , "Unos za hrv ");
      tbx_eng  = hamper.CreateVvTextBoxLookUp(1, 2, "tbx_eng" , "Unos za eng ");
      tbx_njem = hamper.CreateVvTextBoxLookUp(1, 3, "tbx_njem", "Unos za njem");
      tbx_tal  = hamper.CreateVvTextBoxLookUp(1, 4, "tbx_tal" , "Unos za tal ");

      tbx_hrv .JAM_Set_LookUpTable(ZXC.luiListaRiskJezikHrv, (int)ZXC.Kolona.prva);
      tbx_eng .JAM_Set_LookUpTable(ZXC.luiListaRiskJezikEng, (int)ZXC.Kolona.prva);
      tbx_njem.JAM_Set_LookUpTable(ZXC.luiListaRiskJezikC  , (int)ZXC.Kolona.prva);
      tbx_tal .JAM_Set_LookUpTable(ZXC.luiListaRiskJezikD  , (int)ZXC.Kolona.prva);

      SetUpAsWriteOnlyTbx(hamper);

      VvHamper.HamperStyling(hamper);

   }
   private void CreateHampCbxOstalo       (out VvHamper hamper)
   {
      hamper = new VvHamper(1, 23, "", tabControlPrintFak.TabPages[TabPageTitle7], false, hamp_valuta.Right + ZXC.Qun4, nextY, razmakHampGroup);

      for(int i = 0; i < hamper.VvNumOfCols; i++)
      {
         hamper.VvColWdt[i]    = ZXC.Q10un + ZXC.Q10un;
         hamper.VvSpcBefCol[i] = ZXC.Qun4;
      }
      hamper.VvRightMargin = hamper.VvLeftMargin;
      
      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN - ZXC.Qun8;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
     
         lbl_Ostalo          = hamper.CreateVvLabel       (0,  0,       "Odabir ISPISA:"            , ContentAlignment.MiddleLeft);
      cbx_OcuIspisDospjecePl = hamper.CreateVvCheckBox_OLD(0,  1, null, "Dospijeća Plaćanja"        , RightToLeft.No);
      cbx_ocuNapomena        = hamper.CreateVvCheckBox_OLD(0,  2, null, "Napomena"                  , RightToLeft.No);
      cbx_OcuIspisNapomena2  = hamper.CreateVvCheckBox_OLD(0,  3, null, "Napomena2"                 , RightToLeft.No); // uopce da se ispise nampomena2
      cbx_ocuTextNap2        = hamper.CreateVvCheckBox_OLD(0,  4, null, "Tekst Napomena2"           , RightToLeft.No); // da se ispise text nampomena2
      cbx_OcuIspisVirmana    = hamper.CreateVvCheckBox_OLD(0,  5, null, "Virmana"                   , RightToLeft.No);
      cbx_OcuKDZiro_Vir      = hamper.CreateVvCheckBox_OLD(0,  6, null, "ŽiroRn Partnera na Virmanu", RightToLeft.No);
      cbx_ocuVezDok          = hamper.CreateVvCheckBox_OLD(0,  7, null, "OrigBrDok"                 , RightToLeft.No);
      cbx_ocuZiro            = hamper.CreateVvCheckBox_OLD(0,  8, null, "Žiro-računa"               , RightToLeft.No);
                                                              
      cbx_plVirUrokuDana     = hamper.CreateVvCheckBox_OLD(0,  9, null, "Nač.pl: Virmanom "         , RightToLeft.No);
      cbx_ispisOtpremnicaBr  = hamper.CreateVvCheckBox_OLD(0, 10, null, "Otpremnica broj:"          , RightToLeft.No);
      cbx_centarNapKaoNaslov = hamper.CreateVvCheckBox_OLD(0, 11, null, "Napomena kao 'naslov'"     , RightToLeft.No);
      cbx_ocuIspisPrjktBr    = hamper.CreateVvCheckBox_OLD(0, 12, null, "Projekta"                  , RightToLeft.No);
      
      cbx_OcuRokIsporDokDate = hamper.CreateVvCheckBox_OLD(0, 13, null, "Rok isporuke isti kao datum dokumenta"                           , RightToLeft.No);
      cbx_OcuMtrosName       = hamper.CreateVvCheckBox_OLD(0, 14, null, "Naziv Mjesta Troška uz OpćiB"                                    , RightToLeft.No);
      cbx_OcuPomakVirmana    = hamper.CreateVvCheckBox_OLD(0, 15, null, "Pomak ispisa HUB3 obrasca"                                       , RightToLeft.No);
      cbx_OcuDatumRacuna     = hamper.CreateVvCheckBox_OLD(0, 16, null, "Ispiši i Datum računa"                                           , RightToLeft.No);
      cbx_Ocu_OTS_saldo      = hamper.CreateVvCheckBox_OLD(0, 17, null, "Ispiši stanje duga partnera"                                     , RightToLeft.No);
      cbx_Ocu_BarkodTtNum    = hamper.CreateVvCheckBox_OLD(0, 18, null, "Prikaži barkod broja računa"                                     , RightToLeft.No);
      cbx_necuFiskalDodatak  = hamper.CreateVvCheckBox_OLD(0, 19, null, "Ne prikazuj Fiskal dodatak"                                      , RightToLeft.No);
      cbx_Ocu_BarKodPDF417   = hamper.CreateVvCheckBox_OLD(0, 20, null, "Prikaži barkod podataka za plaćanje računa"                      , RightToLeft.No);
      cbx_Necu_prikazEUR     = hamper.CreateVvCheckBox_OLD(0, 21, null, "Ne prikazuj dvojno iskazivanje ukupnog iznosa po fiksnom tečaju" , RightToLeft.No);


      if(TheVvUC is BlgIsplatDUC || TheVvUC is BlgUplatDUC || TheVvUC is BlgUplat_M_DUC || TheVvUC is BlgIsplat_M_DUC)
      {
         cbx_OcuDatumRacuna    .Visible =
         cbx_OcuMtrosName      .Visible =
         cbx_OcuRokIsporDokDate.Visible = 
         cbx_ocuIspisPrjktBr   .Visible =
         cbx_plVirUrokuDana    .Visible =
         cbx_ispisOtpremnicaBr .Visible =
         cbx_centarNapKaoNaslov.Visible =
         cbx_OcuIspisDospjecePl.Visible =
         cbx_PotvrdaNarudzbe   .Visible =
         cbx_OcuTitleOkvir     .Visible =
         cbx_OcuIspisNapomena2 .Visible =
         cbx_ocuTextNap2       .Visible =
         cbx_OcuIspisDokNum2   .Visible =
         cbx_ocuVezDok         .Visible =
         cbx_OcuIspisVirmana   .Visible =
         cbx_OcuKDZiro_Vir     .Visible =
         cbx_OcuPomakVirmana   .Visible =
       //cbx_Ocu_OTS_saldo     .Visible =
         cbx_Ocu_BarkodTtNum   .Visible =
         cbx_necuFiskalDodatak .Visible = 
         cbx_Ocu_BarKodPDF417  .Visible =
         cbx_Necu_prikazEUR    .Visible = 
         cbx_ocuZiro           .Visible = false;
      }

      if(isFakExt == false)
      {
         cbx_OcuDatumRacuna    .Visible =
         cbx_OcuMtrosName      .Visible =
         cbx_OcuRokIsporDokDate.Visible = 
         cbx_ocuIspisPrjktBr   .Visible =
         cbx_plVirUrokuDana    .Visible =
         cbx_ispisOtpremnicaBr .Visible =
         cbx_centarNapKaoNaslov.Visible =
         cbx_OcuIspisDospjecePl.Visible =
         cbx_PotvrdaNarudzbe   .Visible =
         cbx_OcuIspisNapomena2 .Visible =
         cbx_ocuTextNap2       .Visible =
         cbx_OcuIspisDokNum2   .Visible =
         cbx_ocuVezDok         .Visible =
         cbx_OcuIspisVirmana   .Visible =
         cbx_OcuPomakVirmana   .Visible =
         cbx_ocuZiro           .Visible =
       //cbx_Ocu_OTS_saldo     .Visible =
         cbx_necuFiskalDodatak.Visible =
         cbx_OcuKDZiro_Vir    .Visible = 
         lbl_Ostalo           .Visible = 
         cbx_Necu_prikazEUR   .Visible = false;
      }

      if(TheVvUC is IFADUC || TheVvUC is IRADUC || TheVvUC is IRPDUC) cbx_Ocu_OTS_saldo.Visible = true;
      else                                                            cbx_Ocu_OTS_saldo.Visible = false;

      //cbx_Ocu_BarkodTtNum.Visible = false;
        cbx_Ocu_BarkodTtNum .Visible = true;
        cbx_Ocu_BarKodPDF417.Visible = true;

      VvHamper.HamperStyling(hamper);
   }

   #endregion  Ostalo

   #region Blagajna
 
   private void CreateHampBlg(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 3, "", tabControlPrintFak.TabPages[TabPageTitle8], false, nextX, nextY, razmakHampGroup);

      for(int i = 0; i < hamper.VvNumOfCols; i++)
      {
         hamper.VvColWdt[i]    = ZXC.Q5un ;
         hamper.VvSpcBefCol[i] = ZXC.Qun4;
      }
      hamper.VvRightMargin = hamper.VvLeftMargin;
      hamper.VvSpcBefCol[0] = ZXC.Qun2;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbx_blgOcuColKonto    = hamper.CreateVvCheckBox_OLD(0, 0, null, "Kolona Konto"  , RightToLeft.No);
      cbx_blgOcuColRacun    = hamper.CreateVvCheckBox_OLD(1, 0, null, "Kolona Račun"  , RightToLeft.No);
      cbx_blgOcu2na1strani  = hamper.CreateVvCheckBox_OLD(2, 0, null, "2 na 1 Strani"   , RightToLeft.No);
      cbx_blgOcuOkvirUplsp  = hamper.CreateVvCheckBox_OLD(3, 0, null, "Okvir Isp/Upl", RightToLeft.No);
      cbx_OcuNapomUmjKupDob = hamper.CreateVvCheckBox_OLD(0, 1, null, 3, 0, "Ispiši Napomenu umjesto Uplatitelja / Isplatitelja", RightToLeft.No);
      cbx_OcuLikvidator     = hamper.CreateVvCheckBox_OLD(0, 2, null, "Likvidator", RightToLeft.No);

      VvHamper.HamperStyling(hamper);
   }
   
   #endregion Blagajna


   private void CreateHamperChoseIra(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 8, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.QUN - ZXC.Qun8, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                 hamper.CreateVvLabel      (0, 0, "Izbor ispisa:", 1, 0, ContentAlignment.MiddleLeft);
      
      rbt_ira1 = hamper.CreateVvRadioButton(0, 1, new EventHandler(ChooseDscLuiList), "", TextImageRelation.ImageBeforeText);
      rbt_ira2 = hamper.CreateVvRadioButton(0, 2, new EventHandler(ChooseDscLuiList), "", TextImageRelation.ImageBeforeText);
      rbt_ira3 = hamper.CreateVvRadioButton(0, 3, new EventHandler(ChooseDscLuiList), "", TextImageRelation.ImageBeforeText);
      rbt_ira4 = hamper.CreateVvRadioButton(0, 4, new EventHandler(ChooseDscLuiList), "", TextImageRelation.ImageBeforeText);
      rbt_ira5 = hamper.CreateVvRadioButton(0, 5, new EventHandler(ChooseDscLuiList), "", TextImageRelation.ImageBeforeText);
      rbt_ira1.Checked = true;
      rbt_ira1.Tag = true;
      rbt_ira1.Name = "1";
      rbt_ira2.Name = "2";
      rbt_ira3.Name = "3";
      rbt_ira4.Name = "4";
      rbt_ira5.Name = "5";

      cbx_OcuPosPrint = hamper.CreateVvCheckBox_OLD(0, 7, null, 1, 0, "POS print", System.Windows.Forms.RightToLeft.No);


      tbx_obrazacA = hamper.CreateVvTextBox(1, 1, "tbx_obrazacA", "Naziv obrasca A");
      tbx_obrazacB = hamper.CreateVvTextBox(1, 2, "tbx_obrazacB", "Naziv obrasca B");
      tbx_obrazacC = hamper.CreateVvTextBox(1, 3, "tbx_obrazacC", "Naziv obrasca C");
      tbx_obrazacD = hamper.CreateVvTextBox(1, 4, "tbx_obrazacD", "Naziv obrasca D");
      tbx_obrazacE = hamper.CreateVvTextBox(1, 5, "tbx_obrazacE", "Naziv obrasca E");
      tbx_obrazacA.Text = "ObrazacA";
      tbx_obrazacB.Text = "ObrazacB";
      tbx_obrazacC.Text = "ObrazacC";
      tbx_obrazacD.Text = "ObrazacD";
      tbx_obrazacE.Text = "ObrazacE";
      tbx_obrazacA.Font = 
      tbx_obrazacB.Font =
      tbx_obrazacC.Font = 
      tbx_obrazacD.Font =
      tbx_obrazacE.Font = ZXC.vvFont.SmallFont;

      if(TheVvUC is IFADUC || TheVvUC is PonMalDUC || TheVvUC is IRMDUC_2 || TheVvUC is IRPDUC)
      {
         rbt_ira2    .Visible =
         rbt_ira3    .Visible =
         rbt_ira4    .Visible =
         tbx_obrazacB.Visible =
         tbx_obrazacC.Visible = true;
         tbx_obrazacD.Visible = true;

         rbt_ira5.Visible = tbx_obrazacE.Visible = false;
      }
      else if(TheVvUC is IRADUC)
      {
         rbt_ira2.Visible =
         rbt_ira3.Visible =
         tbx_obrazacB.Visible =
         tbx_obrazacC.Visible = true;
         
         rbt_ira4    .Visible =
         tbx_obrazacD.Visible = true;
         rbt_ira5.Visible = tbx_obrazacE.Visible = true;

      }
      else if(TheVvUC is IRMDUC)
      {
         rbt_ira2    .Visible =
         tbx_obrazacB.Visible = true;
         
         tbx_obrazacC.Visible = 
         rbt_ira3    .Visible =
         rbt_ira4    .Visible =
         tbx_obrazacD.Visible = false;
         rbt_ira5.Visible = tbx_obrazacE.Visible = false;

      }
      else if(TheVvUC is PonudaDUC || TheVvUC is ObvezPonudaDUC)
      {
         rbt_ira2    .Visible =
         tbx_obrazacB.Visible =true;
         rbt_ira3    .Visible =
         rbt_ira4    .Visible =
         tbx_obrazacC.Visible = 
         tbx_obrazacD.Visible = true;
         rbt_ira5.Visible = tbx_obrazacE.Visible = false;

      }

      else
      {
         tbx_obrazacB.Visible =
         tbx_obrazacC.Visible =
         tbx_obrazacD.Visible =
         rbt_ira2    .Visible =
         rbt_ira3    .Visible = 
         rbt_ira4    .Visible = false;
         rbt_ira5.Visible = tbx_obrazacE.Visible = false;

      }

      if(TheVvUC is IRMDUC || TheVvUC is IRMDUC_2)
      {
         cbx_OcuPosPrint.Visible = true;
      }
      else
      {
         cbx_OcuPosPrint.Visible = false;
      }


      VvHamper.HamperStyling(hamper);
   }

   private void CreateHamperPosPrinteri(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 7, "", this, false);
      hamper.Location = new Point(ZXC.Qun12, hamp_ChoseIra.Bottom + ZXC.Q3un);

      hamper.VvColWdt      = new int[] { ZXC.Q10un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8  };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel      (0, 0, "Odabir POS printera", ContentAlignment.MiddleLeft);
      rbt_posClassic   = hamper.CreateVvRadioButton(0, 1, null, "Classic"     , TextImageRelation.ImageBeforeText);
      rbt_posBixolon01 = hamper.CreateVvRadioButton(0, 2, null, "Bixolon 01"  , TextImageRelation.ImageBeforeText);
      rbt_posBixolon02 = hamper.CreateVvRadioButton(0, 3, null, "Bixolon 02"  , TextImageRelation.ImageBeforeText);
      rbt_posEpson01   = hamper.CreateVvRadioButton(0, 4, null, "Epson G"     , TextImageRelation.ImageBeforeText);
      rbt_posEpson02   = hamper.CreateVvRadioButton(0, 5, null, "Epson 02"    , TextImageRelation.ImageBeforeText);
      rbt_posPos80     = hamper.CreateVvRadioButton(0, 6, null, "Pos80"       , TextImageRelation.ImageBeforeText);
      rbt_posClassic.Checked = true;
      rbt_posClassic.Tag     = true;
      
      VvHamper.HamperStyling(hamper);
   }

   //--------------------------------------------------------------------------------------------------------------------------------------
   private void RecalculateHamper4buttons()
   {
      btn_Reset.Visible = false;
      hamper4buttons.BorderStyle = BorderStyle.None;
   }

   private void CalcLocationHampersOnFilter(bool isMinFilter)
   {
      ZXC.ZaUpis zaUpisOtvZatv;

      if(isMinFilter)
      {
         this.Size = new Size(minFilterWidth, TheVvUC.Height);
         ((VvRecordUC)TheVvUC).ThePanelForFilterUC_PrintTemplateUC.Width = minFilterWidth;

         hamper4buttons .Location = new Point(ZXC.Qun2 - ZXC.Qun4, 0);
         hamp_ChoseIra  .Location = new Point(ZXC.Qun2 - ZXC.Qun4 + ZXC.Qun8, hamper4buttons.Bottom);
         hamp_OpenFilter.Location = new Point(ZXC.Qun2 - ZXC.Qun2, hamp_ChoseIra.Bottom + razmakIzmjedjuHampera);

         tbx_obrazacA.JAM_ReadOnly = true;
         tbx_obrazacB.JAM_ReadOnly = true;
         tbx_obrazacC.JAM_ReadOnly = true;
         tbx_obrazacD.JAM_ReadOnly = true;
         tbx_obrazacE.JAM_ReadOnly = true;

         btn_OpenFilter.Text = "Postavke +";

         zaUpisOtvZatv = ZXC.ZaUpis.Zatvoreno;
         tabControlPrintFak.Visible = false;
      }
      else
      {
         this.Size = new Size(maxFilterWidth + ZXC.QUN, maxFilterHeight);
         ((VvRecordUC)TheVvUC).ThePanelForFilterUC_PrintTemplateUC.Width  = maxFilterWidth + ZXC.QUN;
         ((VvRecordUC)TheVvUC).ThePanelForFilterUC_PrintTemplateUC.Height = maxFilterHeight;

         //hamper4buttons .Location = new Point(maxFilterWidth - hamp_ChoseIra.Width - razmakIzmjedjuHampera, 0);
         //hamp_ChoseIra  .Location = new Point(maxFilterWidth - hamp_ChoseIra.Width - razmakIzmjedjuHampera + ZXC.Qun10, hamper4buttons.Bottom);
         //hamp_OpenFilter.Location = new Point(maxFilterWidth - hamp_ChoseIra.Width - razmakIzmjedjuHampera - ZXC.Qun4, hamp_ChoseIra.Bottom + razmakIzmjedjuHampera);
        
         hamper4buttons .Location = new Point(maxFilterWidth - hamp_ChoseIra.Width - ZXC.Qun8 , 0);
         hamp_ChoseIra  .Location = new Point(maxFilterWidth - hamp_ChoseIra.Width - ZXC.Qun8 + ZXC.Qun10, hamper4buttons.Bottom);
         hamp_OpenFilter.Location = new Point(maxFilterWidth - hamp_ChoseIra.Width - ZXC.Qun8 - ZXC.Qun4, hamp_ChoseIra.Bottom + razmakIzmjedjuHampera);

         btn_Reset.Parent = this;
         btn_Reset.Location = new Point(hamp_OpenFilter.Left + ZXC.Qun4, hamp_OpenFilter.Bottom + razmakIzmjedjuHampera);

         btn_OpenFilter.Text = "Postavke -";

         tbx_obrazacA.JAM_ReadOnly = false;
         tbx_obrazacB.JAM_ReadOnly = false;
         tbx_obrazacC.JAM_ReadOnly = false;
         tbx_obrazacD.JAM_ReadOnly = false;
         tbx_obrazacE.JAM_ReadOnly = false;

         zaUpisOtvZatv = ZXC.ZaUpis.Otvoreno;

         tabControlPrintFak.Visible = true;
         tabControlPrintFak.BringToFront();
         tabControlPrintFak.Size = new System.Drawing.Size(maxFilterWidth - hamp_ChoseIra.Width - razmakIzmjedjuHampera, maxFilterHeight -ZXC.QUN );
     //    VvHamper.SetChkBoxRadBttnAutoCheck(tabControlPrintFak, true); mislim da je duplo i nepotrebno

      }

    // 05.04.2016. sto ce nam Reset kad ionako u pocetnim postavkama nema nista pametno ?!  
    //btn_Reset.Visible = !isMinFilter;
      btn_Reset.Visible = false;
      
      VvHamper.Open_Close_Fields_ForWriting(tbx_obrazacA, zaUpisOtvZatv, ZXC.ParentControlKind.VvReportUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_obrazacB, zaUpisOtvZatv, ZXC.ParentControlKind.VvReportUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_obrazacC, zaUpisOtvZatv, ZXC.ParentControlKind.VvReportUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_obrazacD, zaUpisOtvZatv, ZXC.ParentControlKind.VvReportUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_obrazacE, zaUpisOtvZatv, ZXC.ParentControlKind.VvReportUC);

      if(TheVvUC is FakturExtDUC && (FakturExtDUC)TheVvUC is IRPDUC) VvHamper.Open_Close_Fields_ForWriting(tbx_koefPIX, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
   }

   private void CreateHamperOpenFilter(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", this, false);
      
      hamper.VvColWdt      = new int[] { ZXC.QunBtnW };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QunBtnH;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      btn_OpenFilter = hamper.CreateVvButton(0, 0, new EventHandler(ButtonOpenFilter_Click), "Postavke +");
      
   }

   public void ButtonOpenFilter_Click(object sender, EventArgs e)
   {
      if(((VvRecordUC)TheVvUC).ThePanelForFilterUC_PrintTemplateUC.Width == maxFilterWidth + ZXC.QUN)
      {
         CalcLocationHampersOnFilter(true);
         //SetVisibilitiOfHampers(false);
      }
      else
      {
         CalcLocationHampersOnFilter(false);
         //SetVisibilitiOfHampers(true);
      }
   }

   private void SetVisibilitiOfHampers(bool isVisible)
   {
      hamp_dokNumLCR     .Visible =
      hamp_migHF         .Visible =
      hamp_NRD           .Visible =
      hamp_title         .Visible =
      hamp_dokNumHFadres .Visible =
      hamp_pageLCR       .Visible =
      hamp_pageHF        .Visible =
      hamp_adresSastojci .Visible =
      hamp_adresToFrom   .Visible =
      hamp_adresOkvir    .Visible =
      hamp_Columns       .Visible =
      hamp_personi       .Visible =
      hamp_adresLR       .Visible =
      hamp_orient        .Visible =
      hamp_RnPnb         .Visible =
      hamp_belowGrid     .Visible =
      hamp_CbxOstalo     .Visible =
      hamp_ostalo        .Visible =
      hamp_poslJed       .Visible =
      hamp_logo          .Visible =
      hamp_memo          .Visible =
      hamp_FontBelGr     .Visible =
      hamp_FontOpis      .Visible =
      hamp_FontCol       .Visible =
      hamp_jezik         .Visible =
      hamp_valuta        .Visible = isVisible;

      hamp_Blg.Visible = false;

      if(TheVvUC is BlgIsplatDUC || TheVvUC is BlgUplatDUC || TheVvUC is BlgUplat_M_DUC || TheVvUC is BlgIsplat_M_DUC)
      {
         hamp_dokNumLCR     .Visible =
         hamp_migHF         .Visible =
         hamp_NRD           .Visible =
         hamp_title         .Visible =
         hamp_dokNumHFadres .Visible =
         hamp_pageLCR       .Visible =
         hamp_pageHF        .Visible =
         hamp_adresSastojci .Visible =
         hamp_adresToFrom   .Visible =
         hamp_adresOkvir    .Visible =
         hamp_Columns       .Visible =
         hamp_personi       .Visible =
         hamp_adresLR       .Visible =
         hamp_orient        .Visible =
         hamp_RnPnb         .Visible =
         hamp_belowGrid     .Visible =
         hamp_ostalo        .Visible =
         hamp_poslJed       .Visible =
         hamp_FontBelGr     .Visible =
         hamp_FontOpis      .Visible =
         hamp_FontCol       .Visible =
         hamp_jezik         .Visible =
         hamp_valuta        .Visible = false;
         hamp_Blg           .Visible = isVisible;
      }
   //            hamp_dokNumLCR.Visible =
   //                  hamp_title.Visible =
   //hamp_pageLCR.Visible =
   //hamp_pageHF.Visible =

      if(isFakExt == false)
      {
         hamp_dokNumHFadres.Visible =
         hamp_migHF        .Visible =
         hamp_NRD          .Visible =
         hamp_adresSastojci.Visible =
         hamp_adresToFrom  .Visible =
         hamp_adresOkvir   .Visible =
         hamp_adresLR      .Visible =
         hamp_ostalo       .Visible =
         hamp_poslJed      .Visible =
         hamp_jezik        .Visible =
         hamp_valuta       .Visible = false;

      }
   }

   #endregion Hampers

   #region Hampers +

   #region Rtrano

   private void CreateHamperRtranoOtprFilter(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 6, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.QunBtnW };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QunBtnH;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvRowHgt[4] = 
      hamper.VvRowHgt[5] = ZXC.QUN;
      hamper.VvBottomMargin = hamper.VvTopMargin;

      btn_RtranoOtpr     = hamper.CreateVvButton(0, 0, new EventHandler(ButtonRtranoOtpr_Click)    , "Otpremnica"   );
      btn_RtranoOtprM2   = hamper.CreateVvButton(0, 1, new EventHandler(ButtonRtranoOtprM2_Click)  , "Otpremnica m2");
      btn_RtranoOtprDimZ = hamper.CreateVvButton(0, 2, new EventHandler(ButtonRtranoOtprDimZ_Click), "Otpr Deb/Str");
      //cbx_isRtranoM2   = hamper.CreateVvCheckBox_OLD(0, 1, null, "Otpr. m2", System.Windows.Forms.RightToLeft.No);

      btn_Rtrano4PIX = hamper.CreateVvButton(0, 3, new EventHandler(ButtonRtrano4Pixux_Click), "ObrZaPix");

      hamper.CreateVvLabel(0, 4, "% od", ContentAlignment.BottomLeft);
      tbx_koefPIX = hamper.CreateVvTextBox(0, 5, "tbx_koefPIX", "Koeficijent PIX", 7);
      tbx_koefPIX.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, decimal.MinValue, false);
      tbx_koefPIX.JAM_IsForPercent = true;
      tbx_koefPIX.JAM_WriteOnly = true;

      hamper.Location = new Point(hamp_ChoseIra.Left, hamp_OpenFilter.Bottom + ZXC.Q10un);
   }
   public void ButtonRtranoOtpr_Click(object sender, EventArgs e)
   {
      IsRtrnoOtpr    = true;
      
      ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(true);
      
      IsRtrnoOtpr    = false;

   }
   public void ButtonRtranoOtprM2_Click(object sender, EventArgs e)
   {
      IsRtrnoOtpr_m2 = true;
      
      ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(true);

      IsRtrnoOtpr_m2 = false;
   }
   public void ButtonRtranoOtprDimZ_Click(object sender, EventArgs e)
   {
      IsRtrnoOtpr_dimZ = true;
      
      ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(true);

      IsRtrnoOtpr_dimZ = false;
   }
   public void ButtonRtrano4Pixux_Click(object sender, EventArgs e)
   {
      IsRtrno4PIX    = true;
      
      ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(true);
      
      IsRtrno4PIX = false;
   }

   #endregion Rtrano

   #region PrimkaKalkulacija

   private void CreateHamperPrimkaKalkulacija(out VvHamper hamper, string btnText)
   {
      hamper = new VvHamper(1, 2, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.QunBtnW };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4    };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QunBtnH;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      btn_PriKalkulacija = hamper.CreateVvButton(0, 0, new EventHandler(ButtonPriKalk_Click)    , btnText);

      btn_barkodPrint    = hamper.CreateVvButton(0, 1, new EventHandler(BarCodePrint_Click), "Barkod naljepnice");

      hamper.Location = new Point(hamp_ChoseIra.Left, hamp_OpenFilter.Bottom + ZXC.Q10un);

      btn_barkodPrint.Visible = false;
   }
   public void ButtonPriKalk_Click(object sender, EventArgs e)
   {
      IsPri_Kalk = true;

      ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(true);

      IsPri_Kalk = false;

   }
   public void BarCodePrint_Click(object sender, EventArgs e)
   {
      IsBarCodePrint = true;

      ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(true);

      IsBarCodePrint = false;

   }
   
   #endregion PrimkaKalkulacija

   #region PPMV

   private void CreateHamperPPMV_Prilog1(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.QunBtnW };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QunBtnH;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      btn_PpmvPrilog1 = hamper.CreateVvButton(0, 0, new EventHandler(ButtonPPMV_Click), "PPMV Prilog1" );

      hamper.Location = new Point(hamp_ChoseIra.Left, hamp_OpenFilter.Bottom + ZXC.Q10un);

      hamper.Visible = false;
   }

   public void ButtonPPMV_Click(object sender, EventArgs e)
   {
      IsPPMV_Prilog1 = true;

      ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(true);

      IsPPMV_Prilog1 = false;
   }

   #endregion PPMV

   #region AnalizaRNP

   private void CreateHamper_Rezervacije(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 2, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q2un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN,  ZXC.QUN};
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
      hamper.Location = new Point(hamp_ChoseIra.Left, hamper4buttons.Bottom);
      hamperHorLine.Visible = hamp_ChoseIra.Visible = hamper4buttons.Visible = false;

                          hamper.CreateVvLabel  (0, 0, "Rezerv1:", ContentAlignment.MiddleRight);
      tbx_rezervacija1  = hamper.CreateVvTextBox(1, 0, "tbx_rezervacija1", "", 12);

                         hamper.CreateVvLabel  (0, 1, "Rezerv2:", ContentAlignment.MiddleRight);
      tbx_rezervacija2 = hamper.CreateVvTextBox(1, 1, "tbx_rezervacija2", "", 12);

      tbx_rezervacija1.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_rezervacija2.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      SetUpAsWriteOnlyTbx(hamper);
      
      VvHamper.HamperStyling(hamper);
   }

   #endregion AnalizaRNP

   #region RNM

   private void CreateHamper_RNMDUC(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.QunBtnW };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QunBtnH;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      btn_RNM_Nalog = hamper.CreateVvButton(0, 0, new EventHandler(ButtonRNM_Nalog_Click), "Nalog"      );
      btn_RNM_Realz = hamper.CreateVvButton(0, 1, new EventHandler(ButtonRNM_Realz_Click), "Realizacija");

      hamper.Location = new Point(hamp_ChoseIra.Left + ZXC.QUN, hamper4buttons.Bottom );
      hamperHorLine.Visible = hamp_ChoseIra.Visible = hamper4buttons.Visible = false;

   }
   public void ButtonRNM_Nalog_Click(object sender, EventArgs e)
   {
      IsRnmNalog = true;

      ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(true);

      IsRnmNalog = false;
   }
   public void ButtonRNM_Realz_Click(object sender, EventArgs e)
   {
      IsRnmRealiz = true;

      ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(true);

      IsRnmRealiz = false;
   }

   #endregion RNM

   #region KLK_URM naljepnice

   private void CreateHamper_KLK_URM(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QunBtnH;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      btn_KLK_URM_DUC        = hamper.CreateVvButton(0, 0, new EventHandler(ButtonKLKURM_DUC_Click    ), "KLK / URM");
      btn_KLK_URM_DUC_Ztr    = hamper.CreateVvButton(0, 1, new EventHandler(ButtonKLKURM_DUC_ZTR_Click), "KLK + ZTR");
      btn_KLK_URM_Naljepnica = hamper.CreateVvButton(0, 2, new EventHandler(ButtonKLKURM_Etiketa_Click), "Etiketa");

      if(TheVvUC is URMDUC) btn_KLK_URM_DUC_Ztr.Visible = false;


      hamper.Location = new Point(0, 0);
      hamper.BringToFront();
   }
   public void ButtonKLKURM_DUC_Click(object sender, EventArgs e)
   {
      IsKlkUrm_DUC = true;

      ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(true);

      IsKlkUrm_DUC = false;
   }
   public void ButtonKLKURM_Etiketa_Click(object sender, EventArgs e)
   {
      IsKlkUrm_Etiketa = true;

      ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(true);

      IsKlkUrm_Etiketa = false;
   }
   public void ButtonKLKURM_DUC_ZTR_Click(object sender, EventArgs e)
   {
      IsKlkUrm_Ztr = true;

      ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(true);

      IsKlkUrm_Ztr = false;
   }

   #endregion KLK_URM naljepnice

   #region PCTOGO

   private void CreateHamper_UGANDUC_PTG_Filter(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 5, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QunBtnH;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      btn_PTG_UGAN = hamper.CreateVvButton(0, 0, new EventHandler(Button_btn_PTG_UGAN_Click), "UGAN");
      btn_PTG_OPL  = hamper.CreateVvButton(0, 1, new EventHandler(Button_btn_PTG_OPL_Click ), "Otplatni Plan");
      btn_PTG_DOD  = hamper.CreateVvButton(0, 2, new EventHandler(Button_btn_PTG_DOD_Click ), "Dodaci");

      hamper.Location = new Point(ZXC.Qun8, ZXC.Qun2);
      hamper.BringToFront();

   }

   public void Button_btn_PTG_UGAN_Click(object sender, EventArgs e) { IsPTG_UGAN = true; ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(true); IsPTG_UGAN = false; }
   public void Button_btn_PTG_OPL_Click (object sender, EventArgs e) { IsPTG_OPL  = true; ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(true); IsPTG_OPL  = false; }
   public void Button_btn_PTG_DOD_Click (object sender, EventArgs e) { IsPTG_DOD  = true; ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(true); IsPTG_DOD  = false; }

   #endregion PCTOGO



   #endregion Hampers +

   #region Fld_

   public bool   Fld_NeedsHorizontalLine { get { return cb_Line.Checked; }  set {        cb_Line.Checked = value; } }

   public string Fld_AdresaLeftRight   
   {
      get
      {
         if     (rbt_AdresLeft.Checked)  return "L";
         else if(rbt_AdresRight.Checked) return "R";

         else throw new Exception("Fld_AdresaLeftRight: who df is checked?");
      }
      set
      {
         if     (value.ToString() == "L") rbt_AdresLeft.Checked = true;
         else if(value.ToString() == "R") rbt_AdresRight.Checked = true;
      
      }
   }
   public string Fld_RptOrientation    
   {
      get
      {
         if     (rbt_RptLandsc.Checked)  return "L";
         else if(rbt_RptPort  .Checked)  return "P";

         else throw new Exception("Fld_RptOrientation: who df is checked?");
      }
      
      set
      {
         if     (value.ToString() == "L") rbt_RptLandsc.Checked = true;
         else if(value.ToString() == "P") rbt_RptPort  .Checked = true;
      
      }

   }

   public string Fld_LblPrjktPerson    { get { return tbx_lblPrjktPerson .Text;    }  set {        tbx_lblPrjktPerson .Text    = value; } }
   public string Fld_LblUserPerson     { get { return tbx_lblUserPerson  .Text;    }  set {        tbx_lblUserPerson  .Text    = value; } }
   public string Fld_LblOsobaA         { get { return tbx_lblOsobaA      .Text;    }  set {        tbx_lblOsobaA      .Text    = value; } }
   public string Fld_LblOsobaB         { get { return tbx_lblOsobaB      .Text;    }  set {        tbx_lblOsobaB      .Text    = value; } }
   public bool   Fld_VertikalLine      { get { return cbx_verLine        .Checked; }  set {        cbx_verLine        .Checked = value; } }
   public bool   Fld_TableBorder       { get { return cbx_TableBorder    .Checked; }  set {        cbx_TableBorder    .Checked = value; } }
   public bool   Fld_SignPrimaoc       { get { return cbx_signPrimaoc    .Checked; }  set {        cbx_signPrimaoc    .Checked = value; } }
   public bool   Fld_TekstualIznos     { get { return cbx_tekstualIznos  .Checked; }  set {        cbx_tekstualIznos  .Checked = value; } }
   public bool   Fld_IsAddTtNum        { get { return cbx_IsAddTtNumRn   .Checked; }  set {        cbx_IsAddTtNumRn   .Checked = value; } }
   public bool   Fld_IsAddYear         { get { return cbx_IsAddYearRn    .Checked; }  set {        cbx_IsAddYearRn    .Checked = value; } }
   public bool   Fld_IsAddKupDobCd     { get { return cbx_IsAddKupDobCdRn.Checked; }  set {        cbx_IsAddKupDobCdRn.Checked = value; } }
   public string Fld_DevName           { get { return tbx_ValName        .Text;    }  set {        tbx_ValName        .Text    = value; } }
   
   public ZXC.ValutaNameEnum Fld_DevNameAsEnum
   {
      get
      {
         if(tbx_ValName.Text.IsEmpty()) return ZXC.ValutaNameEnum.EMPTY;
         else                           return (ZXC.ValutaNameEnum)Enum.Parse(typeof(ZXC.ValutaNameEnum), tbx_ValName.Text, true);
      }
   }
   
   public string Fld_NazivPoslJed       
   {
      get { return tbx_nazivPoslJed.Text; }
      set {        tbx_nazivPoslJed.Text = value; }
   }
   public string Fld_PrefixIR1          
   {
      get { return tbx_prefixIR1Rn.Text;}
      set {        tbx_prefixIR1Rn.Text = value; }
   }
   public string Fld_PrefixIR2          
   {
      get { return tbx_prefixIR2Rn.Text; }
      set {        tbx_prefixIR2Rn.Text = value; }
   }

   // 10.11.2020: nije se pamtio kao uint jer: 1. nije bio dodan 'theUinteger' u doticnoj tablici                        
   //                                          2. lista nije bila dodana u eksplicitno navođenje korisnika theUinteger-a 
   //                                             u VvDaoBase.ThisLuiList_DoesntNeed_ExtraData(string luiListTitle);     
 //public uint Fld_PnbM
 //{
 //   get { return tbx_pnbM.GetUintField(); }
 //   set {        tbx_pnbM.PutUintField(value); }
 //}
   public int Fld_PnbM
   {
      get { return tbx_pnbM.GetIntField(     ); }
      set {        tbx_pnbM.PutIntField(value); }
   }

   public string Fld_PrefixIR1_Rn       
   {
      get { return tbx_prefixIR1_Rn.Text; }
      set {        tbx_prefixIR1_Rn.Text = value; }
   }
   public string Fld_PrefixIR1_Pn       
   {
      get { return tbx_prefixIR1_Pn.Text; }
      set {        tbx_prefixIR1_Pn.Text = value; }
   }
   public string Fld_PrefixIR2_Rn       
   {
      get { return tbx_prefixIR2_Rn.Text; }
      set {        tbx_prefixIR2_Rn.Text = value; }
   }
   public string Fld_PrefixIR2_Pn       
   {
      get { return tbx_prefixIR2_Pn.Text; }
      set {        tbx_prefixIR2_Pn.Text = value; }
   }
   public string Fld_IsAddTtNum_Rn      
         {
      get { return tbx_IsAddTtNum_Rn.Text; }
      set {        tbx_IsAddTtNum_Rn.Text = value; }
   }
   public string Fld_IsAddTtNum_Pn      
   {
      get { return tbx_IsAddTtNum_Pn.Text; }
      set {        tbx_IsAddTtNum_Pn.Text = value; }
   }
   public string Fld_IsAddYear_Rn       
         {
      get { return tbx_IsAddYear_Rn.Text; }
      set {        tbx_IsAddYear_Rn.Text = value; }
   }
   public string Fld_IsAddYear_Pn       
   {
      get { return tbx_IsAddYear_Pn.Text; }
      set {        tbx_IsAddYear_Pn.Text = value; }
   }
   public string Fld_IsAddKupDobCd_Rn   
         {
      get { return tbx_IsAddKupDobCd_Rn.Text; }
      set {        tbx_IsAddKupDobCd_Rn.Text = value; }
   }
   public string Fld_IsAddKupDobCd_Pn   
   {
      get { return tbx_IsAddKupDobCd_Pn.Text; }
      set {        tbx_IsAddKupDobCd_Pn.Text = value; }
   }
   public string Fld_Title              
   {
      get { return tbx_nazivPrinta.Text;}
      set {        tbx_nazivPrinta.Text = value; }
   }
   public ushort Fld_ChoseObrazac       
   {
     get
      {
         if     (rbt_ira1.Checked) return 1;
         else if(rbt_ira2.Checked) return 2;
         else if(rbt_ira3.Checked) return 3;
         else if(rbt_ira4.Checked) return 4;
         else if(rbt_ira5.Checked) return 5; //15.05.2020.

         else throw new Exception("Fld_ChoseObrazac: who df is checked?");
      }
     set
      {
         if     ((ushort)value == 0 || (ushort)value == 1) rbt_ira1.Checked = true;
         else if((ushort)value == 2                      ) rbt_ira2.Checked = true;
         else if((ushort)value == 3                      ) rbt_ira3.Checked = true;
         else if((ushort)value == 4                      ) rbt_ira4.Checked = true;
         else if((ushort)value == 5                      ) rbt_ira5.Checked = true;
     
      }
   }
   public string Fld_LblPrimio          
   {
      get { return tbx_Primio.Text; }
      set {        tbx_Primio.Text = value; }
   }
   public bool   Fld_RazmakRows         
   {
      get { return cbx_razmak.Checked; }
      set {        cbx_razmak.Checked = value; }
   }
   public string Fld_LblOpciA           
   {
      get { return tbx_opciA.Text; }
      set {        tbx_opciA.Text = value; }
   }
   public string Fld_LblOpciB           
   {
      get { return tbx_opciB.Text; }
      set {        tbx_opciB.Text = value; }
   }
   public bool   Fld_T_artiklCD         {  get { return cbx_T_artiklCD  .Checked; } set { cbx_T_artiklCD  .Checked = value; }   }
   public bool   Fld_T_artiklName       {  get { return cbx_T_artiklName.Checked; } set { cbx_T_artiklName.Checked = value; }   } 
   public bool   Fld_Napomena           {  get { return cbx_Napomena    .Checked; } set { cbx_Napomena    .Checked = value; }   }
   public bool   Fld_BarCode1           {  get { return cbx_BarCode1    .Checked; } set { cbx_BarCode1    .Checked = value; }   }
   public bool   Fld_ArtiklCD2          {  get { return cbx_ArtiklCD2   .Checked; } set { cbx_ArtiklCD2   .Checked = value; }   }
   public bool   Fld_ArtiklName2        {  get { return cbx_ArtiklName2 .Checked; } set { cbx_ArtiklName2 .Checked = value; }   }
   public bool   Fld_BarCode2           {  get { return cbx_BarCode2    .Checked; } set { cbx_BarCode2    .Checked = value; }   }
   public bool   Fld_LongOpis           {  get { return cbx_LongOpis    .Checked; } set { cbx_LongOpis    .Checked = value; }   }
   public bool   Fld_SerNo              {  get { return cbx_SerNo       .Checked; } set { cbx_SerNo       .Checked = value; }   }
   public bool   Fld_T_jedMj            {  get { return cbx_T_jedMj     .Checked; } set { cbx_T_jedMj     .Checked = value; }   }
   public bool   Fld_T_kol              {  get { return cbx_T_kol       .Checked; } set { cbx_T_kol       .Checked = value; }   }
   public bool   Fld_T_cij              {  get { return cbx_T_cij       .Checked; } set { cbx_T_cij       .Checked = value; }   }
   public bool   Fld_R_KC               {  get { return cbx_R_KC        .Checked; } set { cbx_R_KC        .Checked = value; }   }
   public bool   Fld_T_rbt1St           {  get { return cbx_T_rbt1St    .Checked; } set { cbx_T_rbt1St    .Checked = value; }   }
   public bool   Fld_R_rbt1             {  get { return cbx_R_rbt1      .Checked; } set { cbx_R_rbt1      .Checked = value; }   }
   public bool   Fld_T_rbt2St           {  get { return cbx_T_rbt2St    .Checked; } set { cbx_T_rbt2St    .Checked = value; }   }
   public bool   Fld_R_rbt2             {  get { return cbx_R_rbt2      .Checked; } set { cbx_R_rbt2      .Checked = value; }   }
   public bool   Fld_R_cij_KCR          {  get { return cbx_R_cij_KCR   .Checked; } set { cbx_R_cij_KCR   .Checked = value; }   }
   public bool   Fld_R_KCR              {  get { return cbx_R_KCR       .Checked; } set { cbx_R_KCR       .Checked = value; }   }
   public bool   Fld_T_mrzSt            {  get { return cbx_T_mrzSt     .Checked; } set { cbx_T_mrzSt     .Checked = value; }   }// 26.02.2016. T_serlot
   public bool   Fld_R_mrz              {  get { return cbx_R_mrz       .Checked; } set { cbx_R_mrz       .Checked = value; }   }//18.12.2013. za R_cijOP
   public bool   Fld_R_cij_KCRM         {  get { return cbx_R_cij_KCRM  .Checked; } set { cbx_R_cij_KCRM  .Checked = value; }   }// 29.08.2013. za Kn Tkn_Cij 
   public bool   Fld_R_KCRM             {  get { return cbx_R_KCRM      .Checked; } set { cbx_R_KCRM      .Checked = value; }   }// 29.08.2013. za Kn Rkn_KCRP
   public bool   Fld_R_ztr              {  get { return cbx_R_ztr       .Checked; } set { cbx_R_ztr       .Checked = value; }   }// 30.03.2021. za NBC
   public bool   Fld_T_pdvSt            {  get { return cbx_T_pdvSt     .Checked; } set { cbx_T_pdvSt     .Checked = value; }   }
   public bool   Fld_R_pdv              {  get { return cbx_R_pdv       .Checked; } set { cbx_R_pdv       .Checked = value; }   }
   public bool   Fld_R_cij_KCRP         {  get { return cbx_R_cij_KCRP  .Checked; } set { cbx_R_cij_KCRP  .Checked = value; }   }
   public bool   Fld_R_KCRP             {  get { return cbx_R_KCRP      .Checked; } set { cbx_R_KCRP      .Checked = value; }   }
   public bool   Fld_T_doCijMal         {  get { return cbx_T_doCijMal  .Checked; } set { cbx_T_doCijMal  .Checked = value; }   }//ArtiklTS
   public bool   Fld_T_noCijMal         {  get { return cbx_T_noCijMal  .Checked; } set { cbx_T_noCijMal  .Checked = value; }   }//11.05.2020. theVPC
   public bool   Fld_R_mjMasaN          {  get { return cbx_R_mjMasaN   .Checked; } set { cbx_R_mjMasaN   .Checked = value; }   }
   public bool   Fld_T_garancija        {  get { return cbx_T_garancija .Checked; } set { cbx_T_garancija .Checked = value; }   }

   public int    Fld_NumDecT_kol        {  get { return tbx_NumDecT_kol      .GetIntField();}  set { tbx_NumDecT_kol      .PutIntField(value); } }
   public int    Fld_NumDecT_cij        {  get { return tbx_NumDecT_cij      .GetIntField();}  set { tbx_NumDecT_cij      .PutIntField(value); } }
   public int    Fld_NumDecR_KC         {  get { return tbx_NumDecR_KC       .GetIntField();}  set { tbx_NumDecR_KC       .PutIntField(value); } }
   public int    Fld_NumDecT_rbt1St     {  get { return tbx_NumDecT_rbt1St   .GetIntField();}  set { tbx_NumDecT_rbt1St   .PutIntField(value); } }
   public int    Fld_NumDecR_rbt1       {  get { return tbx_NumDecR_rbt1     .GetIntField();}  set { tbx_NumDecR_rbt1     .PutIntField(value); } }
   public int    Fld_NumDecT_rbt2St     {  get { return tbx_NumDecT_rbt2St   .GetIntField();}  set { tbx_NumDecT_rbt2St   .PutIntField(value); } }
   public int    Fld_NumDecR_rbt2       {  get { return tbx_NumDecR_rbt2     .GetIntField();}  set { tbx_NumDecR_rbt2     .PutIntField(value); } }
   public int    Fld_NumDecR_cij_KCR    {  get { return tbx_NumDecR_cij_KCR  .GetIntField();}  set { tbx_NumDecR_cij_KCR  .PutIntField(value); } }
   public int    Fld_NumDecR_KCR        {  get { return tbx_NumDecR_KCR      .GetIntField();}  set { tbx_NumDecR_KCR      .PutIntField(value); } }
   public int    Fld_NumDecT_mrzSt      {  get { return tbx_NumDecT_mrzSt    .GetIntField();}  set { tbx_NumDecT_mrzSt    .PutIntField(value); } }
   public int    Fld_NumDecR_mrz        {  get { return tbx_NumDecR_mrz      .GetIntField();}  set { tbx_NumDecR_mrz      .PutIntField(value); } }// 18.12.2013. za R_cijOP
   public int    Fld_NumDecR_cij_KCRM   {  get { return tbx_NumDecR_cij_KCRM .GetIntField();}  set { tbx_NumDecR_cij_KCRM .PutIntField(value); } }// 29.08.2013. za Kn Tkn_Cij 
   public int    Fld_NumDecR_KCRM       {  get { return tbx_NumDecR_KCRM     .GetIntField();}  set { tbx_NumDecR_KCRM     .PutIntField(value); } }// 29.08.2013. za Kn Rkn_KCRP
   public int    Fld_NumDecR_ztr        {  get { return tbx_NumDecR_ztr      .GetIntField();}  set { tbx_NumDecR_ztr      .PutIntField(value); } }
   public int    Fld_NumDecT_pdvSt      {  get { return tbx_NumDecT_pdvSt    .GetIntField();}  set { tbx_NumDecT_pdvSt    .PutIntField(value); } }
   public int    Fld_NumDecR_pdv        {  get { return tbx_NumDecR_pdv      .GetIntField();}  set { tbx_NumDecR_pdv      .PutIntField(value); } }
   public int    Fld_NumDecR_cij_KCRP   {  get { return tbx_NumDecR_cij_KCRP .GetIntField();}  set { tbx_NumDecR_cij_KCRP .PutIntField(value); } }
   public int    Fld_NumDecR_KCRP       {  get { return tbx_NumDecR_KCRP     .GetIntField();}  set { tbx_NumDecR_KCRP     .PutIntField(value); } }
   public int    Fld_NumDecT_doCijMal   {  get { return tbx_NumDecT_doCijMal .GetIntField();}  set { tbx_NumDecT_doCijMal .PutIntField(value); } }
   public int    Fld_NumDecT_noCijMal   {  get { return tbx_NumDecT_noCijMal .GetIntField();}  set { tbx_NumDecT_noCijMal .PutIntField(value); } }// 11.05.2020. theVPC

   public string Fld_BelowGrid          { get { return tbx_belowGrid     .Text;             }  set { tbx_belowGrid     .Text        =  value; } }
   public string Fld_BelowOnPOS         { get { return tbx_belowOnPOS    .Text;             }  set { tbx_belowOnPOS    .Text        =  value; } }
   public string Fld_TekstOslobodenPDV     { get { return tbx_oslobodenjePDV.Text;             }  set { tbx_oslobodenjePDV.Text        =  value; } }

   public bool   Fld_OcuHeader  { get { return cbx_ocuHeader.Checked;  } set { cbx_ocuHeader .Checked = value; } }
   public bool   Fld_OcuFooter  { get { return cbx_ocuFooter.Checked;  } set { cbx_ocuFooter .Checked = value; } }
   public bool   Fld_OcuFooter2 { get { return cbx_ocuFooter2.Checked; } set { cbx_ocuFooter2.Checked = value; } }
   public bool   Fld_OcuLogo    { get { return cbx_ocuLogo  .Checked;  } set { cbx_ocuLogo   .Checked = value; } }
   public decimal Fld_ScalingLogo        
   {
      get { return tbx_scalingLogo.GetDecimalField(); }
      set {        tbx_scalingLogo.PutDecimalField(value); }
   }
   public bool   Fld_OcuIspisPnb { get { return cbx_ocuPnb.Checked; } set { cbx_ocuPnb.Checked = value; } }

   public bool   Fld_IsAddTT { get { return cbx_IsAddTT.Checked; } set { cbx_IsAddTT.Checked = value; } }
   //public string Fld_TTinf
   //{
   //   get { return tbx_TTinf.Text; }
   //   set {        tbx_TTinf.Text = value; }
   //}
   public string Fld_SeparIfTT_Rn { get { return tbx_SeparIfTT_Rn.Text; } set { tbx_SeparIfTT_Rn.Text = value; } }

   public int Fld_FontOpis       
   {
      get
      {
         if     (rbt_opis7 .Checked) return  7;
         else if(rbt_opis8 .Checked) return  8;
         else if(rbt_opis9 .Checked) return  9;
         else if(rbt_opis10.Checked) return 10;

         else throw new Exception("Fld_AdresaLeftRight: who df is checked?");
      }
      set
      {
         if     ((int)value ==  7) rbt_opis7 .Checked = true;
         else if((int)value ==  8) rbt_opis8 .Checked = true;
         else if((int)value ==  9) rbt_opis9 .Checked = true;
         else if((int)value == 10) rbt_opis10.Checked = true;
      
      }
   }
   public int Fld_FontColumns    
   {
      get
      {
         if     (rbt_col7 .Checked) return  7;
         else if(rbt_col8 .Checked) return  8;
         else if(rbt_col9 .Checked) return  9;
         else if(rbt_col10.Checked) return 10;

         else throw new Exception("Fld_AdresaLeftRight: who df is checked?");
      }
      set
      {
         if     ((int)value ==  7) rbt_col7 .Checked = true;
         else if((int)value ==  8) rbt_col8 .Checked = true;
         else if((int)value ==  9) rbt_col9 .Checked = true;
         else if((int)value == 10) rbt_col10.Checked = true;
      
      }
   }
   public int Fld_FontBelGr      
   {
      get
      {
         if     (rbt_belGr7 .Checked) return  7;
         else if(rbt_belGr8 .Checked) return  8;
         else if(rbt_belGr9 .Checked) return  9;
         else if(rbt_belGr10.Checked) return 10;

         else throw new Exception("Fld_AdresaLeftRight: who df is checked?");
      }
      set
      {
         if     ((int)value ==  7) rbt_belGr7 .Checked = true;
         else if((int)value ==  8) rbt_belGr8 .Checked = true;
         else if((int)value ==  9) rbt_belGr9 .Checked = true;
         else if((int)value == 10) rbt_belGr10.Checked = true;
      
      }
   }

   public uint   Fld_PrefixIR1Pb        
   {
      get { return tbx_prefixIR1Pb.GetUintField();}
      set {        tbx_prefixIR1Pb.PutUintField(value); }
   }
   public string Fld_PrefixIR2Pb        
   {
      get { return tbx_prefixIR2Pb.Text; }
      set {        tbx_prefixIR2Pb.Text = value; }
   }
   public bool   Fld_IsAddTtNum_Pb      
   {
      get { return cbx_IsAddTtNumPb.Checked; }
      set {        cbx_IsAddTtNumPb.Checked = value; }
   }
   public bool   Fld_IsAddYear_Pb       
   {
      get { return cbx_IsAddYearPb.Checked; }
      set {        cbx_IsAddYearPb.Checked = value; }
   }
   public bool   Fld_IsAddKupDobCd_Pb   
   {
      get { return cbx_IsAddKupDobCdPb.Checked; }
      set {        cbx_IsAddKupDobCdPb.Checked = value; }
   }

   public bool Fld_OcuR12     
   {
      get { return cbx_ocuR12.Checked; }
      set {        cbx_ocuR12.Checked = value; }
   }

   public string Fld_JezikReporta       
   {
      get
      {
         if     (rbt_hrv .Checked) return "H";
         else if(rbt_eng .Checked) return "E";
         else if(rbt_njem.Checked) return "C";
         else if(rbt_tal .Checked) return "D";

         else throw new Exception("Fld_JezikRpta: who df is checked?");
      }
      set
      {
         if     ((string)value == "H") rbt_hrv .Checked = true;
         else if((string)value == "E") rbt_eng .Checked = true;
         else if((string)value == "C") rbt_njem.Checked = true;
         else if((string)value == "D") rbt_tal .Checked = true;
      }
   }

   public bool Fld_blgOcuColKonto   { get { return cbx_blgOcuColKonto  .Checked; } set { cbx_blgOcuColKonto  .Checked = value; }  }
   public bool Fld_blgOcuColRacun   { get { return cbx_blgOcuColRacun  .Checked; } set { cbx_blgOcuColRacun  .Checked = value; }  }
   public bool Fld_blgOcu2na1strani { get { return cbx_blgOcu2na1strani.Checked; } set { cbx_blgOcu2na1strani.Checked = value; }  }
   public bool Fld_blgOcuOkvirUplsp { get { return cbx_blgOcuOkvirUplsp.Checked; } set { cbx_blgOcuOkvirUplsp.Checked = value; }  }

   // new 06.2011.______________________________________________________________________________________________________________________________
   public bool   Fld_OcuKupDobTel        { get { return cbx_OcuKupDobTel          .Checked; } set { cbx_OcuKupDobTel          .Checked = value; }  }
   public bool   Fld_OcuProjektTel       { get { return cbx_OcuProjektTel         .Checked; } set { cbx_OcuProjektTel         .Checked = value; }  }
   public bool   Fld_OcuKupDobFax        { get { return cbx_OcuKupDobFax          .Checked; } set { cbx_OcuKupDobFax          .Checked = value; }  }
   public bool   Fld_OcuProjektFax       { get { return cbx_OcuProjektFax         .Checked; } set { cbx_OcuProjektFax         .Checked = value; }  }
   public bool   Fld_OcuKupDobOib        { get { return cbx_OcuKupDobOib          .Checked; } set { cbx_OcuKupDobOib          .Checked = value; }  }
   public bool   Fld_OcuProjektOib       { get { return cbx_OcuProjektOib         .Checked; } set { cbx_OcuProjektOib         .Checked = value; }  }
   public bool   Fld_OcuKupDobMail       { get { return cbx_OcuKupDobMail         .Checked; } set { cbx_OcuKupDobMail         .Checked = value; }  }
   public bool   Fld_OcuProjektMail      { get { return cbx_OcuProjektMail        .Checked; } set { cbx_OcuProjektMail        .Checked = value; }  }
   public bool   Fld_OcuKupDobNr         { get { return cbx_OcuKupDobNr           .Checked; } set { cbx_OcuKupDobNr           .Checked = value; }  }
   public bool   Fld_PotvrdaNarudzbe     { get { return cbx_PotvrdaNarudzbe       .Checked; } set { cbx_PotvrdaNarudzbe       .Checked = value; }  }
   public bool   Fld_OcuIspisDospjecePl  { get { return cbx_OcuIspisDospjecePl    .Checked; } set { cbx_OcuIspisDospjecePl    .Checked = value; }  }
   public bool   Fld_OcuIspisVeze1       { get { return cbx_OcuIspisVeze1         .Checked; } set { cbx_OcuIspisVeze1         .Checked = value; }  }
   public bool   Fld_OcuIspisVeze2       { get { return cbx_OcuIspisVeze2         .Checked; } set { cbx_OcuIspisVeze2         .Checked = value; }  }
   public bool   Fld_OcuIspisVeze3       { get { return cbx_OcuIspisVeze3         .Checked; } set { cbx_OcuIspisVeze3         .Checked = value; }  }
   public bool   Fld_OcuIspisVeze4       { get { return cbx_OcuIspisVeze4         .Checked; } set { cbx_OcuIspisVeze4         .Checked = value; }  }
   public bool   Fld_OcuIspisVezDok2     { get { return cbx_OcuIspisVezDok2       .Checked; } set { cbx_OcuIspisVezDok2       .Checked = value; }  }
   public bool   Fld_OcuIspisNapomena2   { get { return cbx_OcuIspisNapomena2     .Checked; } set { cbx_OcuIspisNapomena2     .Checked = value; }  }
   public bool   Fld_OcuIspisDokNum2     { get { return cbx_OcuIspisDokNum2       .Checked; } set { cbx_OcuIspisDokNum2       .Checked = value; }  }
   public bool   Fld_OcuMemoHOnAllPages  { get { return cbx_OcuMemoHAllPages      .Checked; } set { cbx_OcuMemoHAllPages      .Checked = value; }  }
   public bool   Fld_OcuMemoFOnAllPages  { get { return cbx_OcuMemoFAllPages      .Checked; } set { cbx_OcuMemoFAllPages      .Checked = value; }  }
   public bool   Fld_OcuTitleOkvir       { get { return cbx_OcuTitleOkvir         .Checked; } set { cbx_OcuTitleOkvir         .Checked = value; }  }
   public bool   Fld_OcuTitleBoja        { get { return cbx_OcuTitleBoja          .Checked; } set { cbx_OcuTitleBoja          .Checked = value; }  }
   public bool   Fld_OcuKupDobBoja       { get { return cbx_OcuKupDobBoja         .Checked; } set { cbx_OcuKupDobBoja         .Checked = value; }  }
   public bool   Fld_OcuPrjktBoja        { get { return cbx_OcuPrjktBoja          .Checked; } set { cbx_OcuPrjktBoja          .Checked = value; }  }
   public bool   Fld_OcuLinijeHeader     { get { return cbx_OcuLinijeHeader       .Checked; } set { cbx_OcuLinijeHeader       .Checked = value; }  }
   public bool   Fld_OcuLinijeFooter     { get { return cbx_OcuLinijeFooter       .Checked; } set { cbx_OcuLinijeFooter       .Checked = value; }  }
   public bool   Fld_OcuIspisVirmana     { get { return cbx_OcuIspisVirmana       .Checked; } set { cbx_OcuIspisVirmana       .Checked = value; }  }
   public bool   Fld_OcuKDZiro_Vir       { get { return cbx_OcuKDZiro_Vir         .Checked; } set { cbx_OcuKDZiro_Vir         .Checked = value; }  }
   public bool   Fld_OcuVezDok           { get { return cbx_ocuVezDok             .Checked; } set { cbx_ocuVezDok             .Checked = value; }  }
   public bool   Fld_OcuJednakiFtTxt2Red { get { return cbx_textOpisDrugiRed      .Checked; } set { cbx_textOpisDrugiRed      .Checked = value; }  }

   public string Fld_LblVeze1            { get { return tbx_LblVeze1  .Text; } set { tbx_LblVeze1  .Text = value; } }
   public string Fld_LblVeze2            { get { return tbx_LblVeze2  .Text; } set { tbx_LblVeze2  .Text = value; } }
   public string Fld_LblVeze3            { get { return tbx_LblVeze3  .Text; } set { tbx_LblVeze3  .Text = value; } }
   public string Fld_LblVeze4            { get { return tbx_LblVeze4  .Text; } set { tbx_LblVeze4  .Text = value; } }
   public string Fld_LblVezDok2          { get { return tbx_LblVezDok2.Text; } set { tbx_LblVezDok2.Text = value; } }
   public string Fld_LblOsobaX           { get { return tbx_LblOsobaX .Text; } set { tbx_LblOsobaX .Text = value; } } 
   public string Fld_AboveGrid           { get { return tbx_AboveGrid .Text; } set { tbx_AboveGrid .Text = value; } }
   public string Fld_BeforNRD            { get { return tbx_beforNRD  .Text; } set { tbx_beforNRD  .Text = value; } }
   public string Fld_AfterNRD            { get { return tbx_afterNRD  .Text; } set { tbx_afterNRD  .Text = value; } }
  
   public string Fld_AlignmentPageNum           
   {
      get
      {
         if     (rbt_alignmentPageNum_L.Checked) return "L";
         else if(rbt_alignmentPageNum_C.Checked) return "C";
         else if(rbt_alignmentPageNum_R.Checked) return "R";

         else throw new Exception("Fld_AlignmentPageNum: who df is checked?");
      }
      set
      {
         if     ((string)value == "L") rbt_alignmentPageNum_L.Checked = true;
         else if((string)value == "C") rbt_alignmentPageNum_C.Checked = true;
         else if((string)value == "R") rbt_alignmentPageNum_R.Checked = true;
      
      }
   }
   public string Fld_PrintPageNum               
   {
      get
      {
         if     (rbt_printPageNum_PHeader.Checked) return "H";
         else if(rbt_printPageNum_PFooter.Checked) return "F";
         else if(rbt_printPageNum_NO     .Checked) return "N";

         else throw new Exception("Fld_PrintPageNum: who df is checked?");
      }
      set
      {
         if     ((string)value == "H") rbt_printPageNum_PHeader.Checked = true;
         else if((string)value == "F") rbt_printPageNum_PFooter.Checked = true;
         else if((string)value == "N") rbt_printPageNum_NO     .Checked = true;
      
      }
   }
   public string Fld_AlignmentDokNum            
   {
      get
      {
         if     (rbt_alignmentDokNum_L.Checked) return "L";
         else if(rbt_alignmentDokNum_C.Checked) return "C";
         else if(rbt_alignmentDokNum_R.Checked) return "R";

         else throw new Exception("Fld_AlignmentDokNum: who df is checked?");
      }
      set
      {
         if     ((string)value == "L") rbt_alignmentDokNum_L.Checked = true;
         else if((string)value == "C") rbt_alignmentDokNum_C.Checked = true;
         else if((string)value == "R") rbt_alignmentDokNum_R.Checked = true;
      
      }
   }
   public bool   Fld_AdresOkvir                 
   {
      get
      {
              if(rbt_adresOkvir.Checked)   return true;
         else if(rbt_adresNoOkvir.Checked) return false;

         else throw new Exception("Fld_AdresOkvir: who df is checked?");
      }
      set
      {
         if     (value == true) rbt_adresOkvir.Checked   = true;
         else                   rbt_adresNoOkvir.Checked = true;
      }
   }
   public bool   Fld_AdresOnlyPartner           
   {
      get
      {
              if(rbt_adresOnlyPartner.Checked)   return true;
         else if(rbt_adresBoth       .Checked) return false;

         else throw new Exception("Fld_AdresOnlyPartner: who df is checked?");
      }
      set
      {
         if  (value == true) rbt_adresOnlyPartner.Checked   = true;
         else                rbt_adresBoth       .Checked = true;

         rbt_adresOnlyPartner.Tag = rbt_adresOnlyPartner.Checked; // bez ovoga bi VvHamper.ClearClearFieldContents uvijek postavljao prvi rbt na defaultni 
         rbt_adresBoth       .Tag = rbt_adresBoth       .Checked; // bez ovoga bi VvHamper.ClearClearFieldContents uvijek postavljao prvi rbt na defaultni 

      }
   }
   public bool   Fld_PrintDokNum_BeforAdres     
   {
      get
      {
              if(rbt_printDokNum_beforAdres.Checked) return true;
         else if(rbt_printDokNum_afterAdres.Checked) return false;

         else throw new Exception("Fld_PrintDokNum_BeforAdres: who df is checked?");
      }
      set
      {
         if  (value == true) rbt_printDokNum_beforAdres.Checked = true;
         else                rbt_printDokNum_afterAdres.Checked = true;
      }
   }
   public bool   Fld_MigPositionHeader          
   {
      get
      {
              if(rbt_mig_RH.Checked) return true;
         else if(rbt_mig_RF.Checked) return false;

         else throw new Exception("Fld_MigPositionHeader: who df is checked?");
      }
      set
      {
         if  (value == true) rbt_mig_RH.Checked = true;
         else                rbt_mig_RF.Checked = true;
      }
   }


   public string Fld_ObrazacA { get { return tbx_obrazacA.Text; } set { tbx_obrazacA.Text = value; } }
   public string Fld_ObrazacB { get { return tbx_obrazacB.Text; } set { tbx_obrazacB.Text = value; } }
   public string Fld_ObrazacC { get { return tbx_obrazacC.Text; } set { tbx_obrazacC.Text = value; } }
   public string Fld_ObrazacD { get { return tbx_obrazacD.Text; } set { tbx_obrazacD.Text = value; } }
   public string Fld_ObrazacE { get { return tbx_obrazacE.Text; } set { tbx_obrazacE.Text = value; } }

   public int  Fld_LeftMargin        
   {
      get
      {
         if     (rbt_leftMargin1.Checked) return 1;
         else if(rbt_leftMargin2.Checked) return 2;
         else if(rbt_leftMargin3.Checked) return 3;
         else if(rbt_leftMargin4.Checked) return 4;
         else if(rbt_leftMargin5.Checked) return 5;

         else throw new Exception("Fld_LeftMargin: who df is checked?");
      }
      set
      {
         if     ((int)value == 1) rbt_leftMargin1.Checked = true;
         else if((int)value == 2) rbt_leftMargin2.Checked = true;
         else if((int)value == 3) rbt_leftMargin3.Checked = true;
         else if((int)value == 4) rbt_leftMargin4.Checked = true;
         else if((int)value == 5) rbt_leftMargin5.Checked = true;
      
      }
   }
   public int  Fld_RightMargin       
   {
      get
      {
         if     (rbt_rightMargin1.Checked) return 1;
         else if(rbt_rightMargin2.Checked) return 2;
         else if(rbt_rightMargin3.Checked) return 3;
         else if(rbt_rightMargin4.Checked) return 4;

         else throw new Exception("Fld_LeftMargin: who df is checked?");
      }
      set
      {
         if     ((int)value == 1) rbt_rightMargin1.Checked = true;
         else if((int)value == 2) rbt_rightMargin2.Checked = true;
         else if((int)value == 3) rbt_rightMargin3.Checked = true;
         else if((int)value == 4) rbt_rightMargin4.Checked = true;
      
      }
   }
   public bool Fld_OcuLinijePerson   { get { return cbx_ocuLinijePerson.Checked; } set { cbx_ocuLinijePerson.Checked = value; } }
   public bool Fld_OcuZiroFromFak    { get { return cbx_ocuZiro        .Checked; } set { cbx_ocuZiro        .Checked = value; } }
   public bool Fld_IspisNapomene     { get { return cbx_ocuNapomena    .Checked; } set { cbx_ocuNapomena    .Checked = value; } }
   public bool Fld_OcuTextNap2       { get { return cbx_ocuTextNap2    .Checked; } set { cbx_ocuTextNap2    .Checked = value; } }
   public bool Fld_OcuFirmuUpotpis   { get { return cbx_ocuFirmuUpotpis.Checked; } set { cbx_ocuFirmuUpotpis.Checked = value; } }
   public bool Fld_OcuTitulu         { get { return cbx_ocuTitulu      .Checked; } set { cbx_ocuTitulu      .Checked = value; } }
   public bool Fld_PositionPersonR   
   {
      get
      {
              if(rbt_personRight .Checked) return true;
         else if(rbt_personCentar.Checked) return false;

         else throw new Exception("Fld_MigPositionHeader: who df is checked?");
      }
      set
      {
         if(value == true) rbt_personRight .Checked = true;
         else              rbt_personCentar.Checked = true;
      }
   }
     
   public bool Fld_FakAdresKaoPoslJed  { get { return cbx_fakAdresKaoPoslJed.Checked; } set { cbx_fakAdresKaoPoslJed.Checked = value; } }
   public bool Fld_PlVirUrokuDana      { get { return cbx_plVirUrokuDana    .Checked; } set { cbx_plVirUrokuDana    .Checked = value; } }
   public bool Fld_IspisOtpremnicaBr   { get { return cbx_ispisOtpremnicaBr .Checked; } set { cbx_ispisOtpremnicaBr .Checked = value; } }
   public bool Fld_IspisUgovora        { get { return cbx_ispisUgovora      .Checked; } set { cbx_ispisUgovora      .Checked = value; } }
   public bool Fld_OnlyArtiklLongOpis  { get { return cbx_onlyUkupnaTezina  .Checked; } set { cbx_onlyUkupnaTezina  .Checked = value; } }  // UKUPNA TEZINA ONLY BEZ KOLONE
   public bool Fld_CentarNapKaoNaslov  { get { return cbx_centarNapKaoNaslov.Checked; } set { cbx_centarNapKaoNaslov.Checked = value; } }
   public bool Fld_OibIspodAdreseOnlyP { get { return cbx_OibIspodAdrese    .Checked; } set { cbx_OibIspodAdrese    .Checked = value; } }
   public bool Fld_OcuKupDobOpisOnlyP  { get { return cbx_OcuKupDobOpis     .Checked; } set { cbx_OcuKupDobOpis     .Checked = value; } }
   public bool Fld_VisibleProjektCd    { get { return cbx_ocuIspisPrjktBr   .Checked; } set { cbx_ocuIspisPrjktBr   .Checked = value; } }

   public bool Fld_OcuDevCijAndDevTec  { get { return cbx_OcuDevCijAndDevTec.Checked; } set { cbx_OcuDevCijAndDevTec.Checked = value; } }
   public bool Fld_OcuRokIsporDokDate  { get { return cbx_OcuRokIsporDokDate.Checked; } set { cbx_OcuRokIsporDokDate.Checked = value; } }
   public bool Fld_OcuMtrosName        { get { return cbx_OcuMtrosName      .Checked; } set { cbx_OcuMtrosName      .Checked = value; } }

   public string Fld_TextPostoBefore   { get { return tbx_postoBefore.Text; } set { tbx_postoBefore.Text = value; } }
   public string Fld_TextPostoAfter    { get { return tbx_postoAfter .Text; } set { tbx_postoAfter .Text = value; } }
                                       
   public decimal Fld_Koef4PIX         { get { return tbx_koefPIX.GetDecimalField(); } set { tbx_koefPIX.PutDecimalField(value); } }
                                       
   public bool   Fld_OcuDateX          { get { return cbx_OcuDateX       .Checked; } set { cbx_OcuDateX       .Checked = value; } }
   public string Fld_LblDateX          { get { return tbx_lblDateX       .Text;    } set { tbx_lblDateX       .Text    = value; } }
   public bool   Fld_OcuPomakVirmana   { get { return cbx_OcuPomakVirmana.Checked; } set { cbx_OcuPomakVirmana.Checked = value; } }
   public bool   Fld_OcuPosPrint       { get { return cbx_OcuPosPrint    .Checked; } set { cbx_OcuPosPrint    .Checked = value; } }
   public bool   Fld_OcuMojuPoslJed    { get { return cbx_OcuMojuPoslJed .Checked; } set { cbx_OcuMojuPoslJed .Checked = value; } }
                                       
   public string Fld_MemoAdd           { get { return tbx_memoAdd.Text           ; } set { tbx_memoAdd        .Text    = value; } }
   public string Fld_MemoPOS           { get { return tbx_memoPOS.Text           ; } set { tbx_memoPOS        .Text    = value; } }
                                       
   public bool   Fld_OcuDatumRacuna    { get { return cbx_OcuDatumRacuna   .Checked ; } set { cbx_OcuDatumRacuna   .Checked  = value; } }
   public bool   Fld_OcuNapomUmjKupDob { get { return cbx_OcuNapomUmjKupDob.Checked ; } set { cbx_OcuNapomUmjKupDob.Checked  = value; } }
   public bool   Fld_OcuLikvidator     { get { return cbx_OcuLikvidator    .Checked ; } set { cbx_OcuLikvidator    .Checked  = value; } }

   public decimal Fld_ScalingLogo2_FP  { get { return tbx_scalingLogoFP.GetDecimalField(); } set { tbx_scalingLogoFP.PutDecimalField(value); }}

   public string Fld_AlignmentLogo2_FP
   {
      get
      {
              if(rbt_alignmentLogoFP_L.Checked) return "L";
         else if(rbt_alignmentLogoFP_C.Checked) return "C";
         else if(rbt_alignmentLogoFP_R.Checked) return "R";

         else throw new Exception("Fld_AlignmentLogoFP: who df is checked?");
      }
      set
      {
              if((string)value == "L") rbt_alignmentLogoFP_L.Checked = true;
         else if((string)value == "C") rbt_alignmentLogoFP_C.Checked = true;
         else if((string)value == "R") rbt_alignmentLogoFP_R.Checked = true;

      }
   }
   
   public string Fld_IsLogo2_FPN
   {
      get
      {
              if(rbt_logoFP_Footer.Checked) return "F";
         else if(rbt_logoFP_Potpis.Checked) return "P";
         else if(rbt_logoFP_NE    .Checked) return "N";
         else if(rbt_logoFP_Logo2 .Checked) return "L";

         else throw new Exception("Fld_AlignmentLogoFP: who df is checked?");
      }
      set
      {
              if((string)value == "F") rbt_logoFP_Footer.Checked = true;
         else if((string)value == "P") rbt_logoFP_Potpis.Checked = true;
         else if((string)value == "N") rbt_logoFP_NE    .Checked = true;
         else if((string)value == "L") rbt_logoFP_Logo2 .Checked = true;

      }
   }
 
   public string Fld_AlignmentMemoAdd
   {
      get
      {
              if(rbt_alignmentMemAdd_L.Checked) return "L";
         else if(rbt_alignmentMemAdd_C.Checked) return "C";
         else if(rbt_alignmentMemAdd_R.Checked) return "R";

         else throw new Exception("Fld_AlignmentLogoFP: who df is checked?");
      }
      set
      {
              if((string)value == "L") rbt_alignmentMemAdd_L.Checked = true;
         else if((string)value == "C") rbt_alignmentMemAdd_C.Checked = true;
         else if((string)value == "R") rbt_alignmentMemAdd_R.Checked = true;

      }
   }

   public VvPref.POSprinterKind Fld_PosPrintKind
   {
      get
      {
              if(rbt_posClassic  .Checked) return VvPref.POSprinterKind.CLASSIC  ;
         else if(rbt_posBixolon01.Checked) return VvPref.POSprinterKind.BIXOLON01;
         else if(rbt_posBixolon02.Checked) return VvPref.POSprinterKind.BIXOLON02;
         else if(rbt_posEpson01  .Checked) return VvPref.POSprinterKind.EPSON01  ;
         else if(rbt_posEpson02  .Checked) return VvPref.POSprinterKind.EPSON02  ;
         else if(rbt_posPos80    .Checked) return VvPref.POSprinterKind.POS80    ;

              else throw new Exception("Fld_PosPrintKind: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case VvPref.POSprinterKind.CLASSIC  : rbt_posClassic  .Checked = true; break;
            case VvPref.POSprinterKind.BIXOLON01: rbt_posBixolon01.Checked = true; break;
            case VvPref.POSprinterKind.BIXOLON02: rbt_posBixolon02.Checked = true; break;
            case VvPref.POSprinterKind.EPSON01  : rbt_posEpson01  .Checked = true; break;
            case VvPref.POSprinterKind.EPSON02  : rbt_posEpson02  .Checked = true; break;
            case VvPref.POSprinterKind.POS80    : rbt_posPos80    .Checked = true; break;
         }
      }
   }

   public decimal Fld_Rezervacija1 { get { return tbx_rezervacija1.GetDecimalField(); } set { tbx_rezervacija1.PutDecimalField(value); } }
   public decimal Fld_Rezervacija2 { get { return tbx_rezervacija2.GetDecimalField(); } set { tbx_rezervacija2.PutDecimalField(value); } }

   public bool    Fld_OcuOTS_saldo      { get { return cbx_Ocu_OTS_saldo    .Checked; } set { cbx_Ocu_OTS_saldo    .Checked = value; } }
   public bool    Fld_OcuBarkodTtNum    { get { return cbx_Ocu_BarkodTtNum  .Checked; } set { cbx_Ocu_BarkodTtNum  .Checked = value; } }
   public bool    Fld_OcuBarKodPDF417   { get { return cbx_Ocu_BarKodPDF417 .Checked; } set { cbx_Ocu_BarKodPDF417 .Checked = value; } }
   public bool    Fld_OcuMemoAddGore    { get { return cbx_Ocu_MemoAddGore  .Checked; } set { cbx_Ocu_MemoAddGore  .Checked = value; } }
   public bool    Fld_NecuFiskalDodatak { get { return cbx_necuFiskalDodatak.Checked; } set { cbx_necuFiskalDodatak.Checked = value; } }
   public bool    Fld_Necu_prikazEUR    { get { return cbx_Necu_prikazEUR   .Checked; } set { cbx_Necu_prikazEUR   .Checked = value; } }

   #endregion Fld_

   #region Put & GetFilterFields

   private FakturDocFilter TheFakturDocFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as FakturDocFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheFakturDocFilter = (FakturDocFilter)_filter_data;

      if(TheFakturDocFilter != null && TheFakturDocFilter.PFD != null)
      {
         Fld_NeedsHorizontalLine = TheFakturDocFilter.PFD.Dsc_HorizontalLine;
      
         Fld_AdresaLeftRight  = TheFakturDocFilter.PFD.Dsc_AdresaLeftRight;
         Fld_RptOrientation   = TheFakturDocFilter.PFD.Dsc_RptOrientation ;
         Fld_VertikalLine     = TheFakturDocFilter.PFD.Dsc_VertikalLine   ;
         Fld_TableBorder      = TheFakturDocFilter.PFD.Dsc_TableBorder    ;
         Fld_TekstualIznos    = TheFakturDocFilter.PFD.Dsc_TekstualIznos  ;
         Fld_RazmakRows       = TheFakturDocFilter.PFD.Dsc_RazmakRows     ;

         Fld_IsAddTtNum       = TheFakturDocFilter.PFD.Dsc_IsAddTtNum     ;
         Fld_IsAddYear        = TheFakturDocFilter.PFD.Dsc_IsAddYear      ;
         Fld_IsAddKupDobCd    = TheFakturDocFilter.PFD.Dsc_IsAddKupDobCd  ;
         Fld_PrefixIR1        = TheFakturDocFilter.PFD.Dsc_Prefix1Rn      ;
         Fld_PrefixIR2        = TheFakturDocFilter.PFD.Dsc_Prefix2Rn      ;
         Fld_PnbM             = TheFakturDocFilter.PFD.Dsc_PnbM           ;
         Fld_PrefixIR1_Rn     = TheFakturDocFilter.PFD.Dsc_Separ1_Rn      ;
         Fld_PrefixIR1_Pn     = TheFakturDocFilter.PFD.Dsc_Separ1_Pn      ;
         Fld_PrefixIR2_Rn     = TheFakturDocFilter.PFD.Dsc_Separ2_Rn      ;
         Fld_PrefixIR2_Pn     = TheFakturDocFilter.PFD.Dsc_Separ2_Pn      ;
         Fld_IsAddTtNum_Rn    = TheFakturDocFilter.PFD.Dsc_SeparIfTtNum_Rn;
         Fld_IsAddTtNum_Pn    = TheFakturDocFilter.PFD.Dsc_SeparIfTtNum_Pn;
         Fld_IsAddYear_Rn     = TheFakturDocFilter.PFD.Dsc_SeparIfYear_Rn ;
         Fld_IsAddYear_Pn     = TheFakturDocFilter.PFD.Dsc_SeparIfYear_Pn ;
         Fld_IsAddKupDobCd_Rn = TheFakturDocFilter.PFD.Dsc_SeparIfKDcd_Rn ;
         Fld_IsAddKupDobCd_Pn = TheFakturDocFilter.PFD.Dsc_SeparIfKDcd_Pn ;
         Fld_Title            = TheFakturDocFilter.PFD.Dsc_Title          ;
      
         Fld_PrefixIR1Pb      = TheFakturDocFilter.PFD.Dsc_PrefixIR1Pb     ;
         Fld_PrefixIR2Pb      = TheFakturDocFilter.PFD.Dsc_PrefixIR2PbTx     ;
         Fld_IsAddTtNum_Pb    = TheFakturDocFilter.PFD.Dsc_IsAddTtNum_Pb   ;
         Fld_IsAddYear_Pb     = TheFakturDocFilter.PFD.Dsc_IsAddYear_Pb    ;
         Fld_IsAddKupDobCd_Pb = TheFakturDocFilter.PFD.Dsc_IsAddKupDobCd_Pb;

         Fld_LblPrjktPerson   = TheFakturDocFilter.PFD.Dsc_LblPrjktPerson ;
         Fld_LblUserPerson    = TheFakturDocFilter.PFD.Dsc_LblUserPerson  ;
         Fld_LblOsobaA        = TheFakturDocFilter.PFD.Dsc_LblOsobaA      ;
         Fld_LblOsobaB        = TheFakturDocFilter.PFD.Dsc_LblOsobaB      ;
         Fld_SignPrimaoc      = TheFakturDocFilter.PFD.Dsc_SignPrimaoc    ;
         Fld_LblPrimio        = TheFakturDocFilter.PFD.Dsc_LblPrimio      ;
         Fld_LblOpciA         = TheFakturDocFilter.PFD.Dsc_LblOpciA      ;
         Fld_LblOpciB         = TheFakturDocFilter.PFD.Dsc_LblOpciB      ;

         Fld_DevName          = (faktur_rec.TtInfo.IsExtendableTT ? faktur_rec.DevName : "");
         Fld_NazivPoslJed     = TheFakturDocFilter.PFD.Dsc_NazivPoslJed    ;
         Fld_ChoseObrazac     = TheFakturDocFilter.ChosenObrazac           ;

         Fld_T_artiklCD        = TheFakturDocFilter.PFD.Dsc_T_artiklCD           ;
         Fld_T_artiklName      = TheFakturDocFilter.PFD.Dsc_T_artiklName         ;
         Fld_Napomena          = TheFakturDocFilter.PFD.Dsc_NapomenaArt          ;
         Fld_BarCode1          = TheFakturDocFilter.PFD.Dsc_BarCode1             ;
         Fld_ArtiklCD2         = TheFakturDocFilter.PFD.Dsc_ArtiklCD2            ;
         Fld_ArtiklName2       = TheFakturDocFilter.PFD.Dsc_ArtiklName2          ;
         Fld_BarCode2          = TheFakturDocFilter.PFD.Dsc_BarCode2             ;
         Fld_LongOpis          = TheFakturDocFilter.PFD.Dsc_LongOpis             ;
         Fld_SerNo             = TheFakturDocFilter.PFD.Dsc_SerNo                ;
         Fld_T_jedMj           = TheFakturDocFilter.PFD.Dsc_T_jedMj              ;
         Fld_T_kol             = TheFakturDocFilter.PFD.Dsc_T_kol                ;
         Fld_T_cij             = TheFakturDocFilter.PFD.Dsc_T_cij                ;
         Fld_R_KC              = TheFakturDocFilter.PFD.Dsc_R_KC                 ;
         Fld_T_rbt1St          = TheFakturDocFilter.PFD.Dsc_T_rbt1St             ;
         Fld_R_rbt1            = TheFakturDocFilter.PFD.Dsc_R_rbt1               ;
         Fld_T_rbt2St          = TheFakturDocFilter.PFD.Dsc_T_rbt2St             ;
         Fld_R_rbt2            = TheFakturDocFilter.PFD.Dsc_R_rbt2               ;
         Fld_R_cij_KCR         = TheFakturDocFilter.PFD.Dsc_R_cij_KCR            ;
         Fld_R_KCR             = TheFakturDocFilter.PFD.Dsc_R_KCR                ;
         Fld_T_mrzSt           = TheFakturDocFilter.PFD.Dsc_T_mrzSt              ;// 26.02.2016. za T_serlot
         Fld_R_mrz             = TheFakturDocFilter.PFD.Dsc_R_mrz                ;// 18.12.2013. za R_cijOP
         Fld_R_cij_KCRM        = TheFakturDocFilter.PFD.Dsc_R_cij_KCRM           ;// 29.08.2013. za Kn Tkn_Cij 
         Fld_R_KCRM            = TheFakturDocFilter.PFD.Dsc_R_KCRM               ;// 29.08.2013. za Kn Rkn_KCRP
         Fld_R_ztr             = TheFakturDocFilter.PFD.Dsc_R_ztr                ;// 30.03.2021. za NBC
         Fld_T_pdvSt           = TheFakturDocFilter.PFD.Dsc_T_pdvSt              ;
         Fld_R_pdv             = TheFakturDocFilter.PFD.Dsc_R_pdv                ;
         Fld_R_cij_KCRP        = TheFakturDocFilter.PFD.Dsc_R_cij_KCRMP          ;
         Fld_R_KCRP            = TheFakturDocFilter.PFD.Dsc_R_KCRMP              ;
         Fld_T_doCijMal        = TheFakturDocFilter.PFD.Dsc_T_doCijMal           ;
         Fld_T_noCijMal        = TheFakturDocFilter.PFD.Dsc_T_noCijMal           ;//11.05.2020. theVPC on IRM
         Fld_R_mjMasaN         = TheFakturDocFilter.PFD.Dsc_R_mjMasaN            ;
         Fld_T_garancija       = TheFakturDocFilter.PFD.Dsc_T_garancija          ;
         Fld_NumDecT_kol       = TheFakturDocFilter.PFD.Dsc_NumDecT_kol          ;
         Fld_NumDecT_cij       = TheFakturDocFilter.PFD.Dsc_NumDecT_cij          ;
         Fld_NumDecR_KC        = TheFakturDocFilter.PFD.Dsc_NumDecR_KC           ;
         Fld_NumDecT_rbt1St    = TheFakturDocFilter.PFD.Dsc_NumDecT_rbt1St       ;
         Fld_NumDecR_rbt1      = TheFakturDocFilter.PFD.Dsc_NumDecR_rbt1         ;
         Fld_NumDecT_rbt2St    = TheFakturDocFilter.PFD.Dsc_NumDecT_rbt2St       ;
         Fld_NumDecR_rbt2      = TheFakturDocFilter.PFD.Dsc_NumDecR_rbt2         ;
         Fld_NumDecR_cij_KCR   = TheFakturDocFilter.PFD.Dsc_NumDecR_cij_KCR      ;
         Fld_NumDecR_KCR       = TheFakturDocFilter.PFD.Dsc_NumDecR_KCR          ;
         Fld_NumDecT_mrzSt     = TheFakturDocFilter.PFD.Dsc_NumDecT_mrzSt        ;
         Fld_NumDecR_mrz       = TheFakturDocFilter.PFD.Dsc_NumDecR_mrz          ;// 18.12.2013. za R_cijOP
         Fld_NumDecR_cij_KCRM  = TheFakturDocFilter.PFD.Dsc_NumDecR_cij_KCRM     ;// 29.08.2013. za Kn Tkn_Cij 
         Fld_NumDecR_KCRM      = TheFakturDocFilter.PFD.Dsc_NumDecR_KCRM         ;// 29.08.2013. za Kn Rkn_KCRP
         Fld_NumDecR_ztr       = TheFakturDocFilter.PFD.Dsc_NumDecR_ztr          ;
         Fld_NumDecT_pdvSt     = TheFakturDocFilter.PFD.Dsc_NumDecT_pdvSt        ;
         Fld_NumDecR_pdv       = TheFakturDocFilter.PFD.Dsc_NumDecR_pdv          ;
         Fld_NumDecR_cij_KCRP = TheFakturDocFilter.PFD.Dsc_NumDecR_cij_KCRMP     ;
         Fld_NumDecR_KCRP     = TheFakturDocFilter.PFD.Dsc_NumDecR_KCRMP         ;
         Fld_NumDecT_doCijMal  = TheFakturDocFilter.PFD.Dsc_NumDecT_doCijMal     ;//23.05.2012.thePFD.Dsc_T_doCijMal ---->  artiklTS
         Fld_NumDecT_noCijMal  = TheFakturDocFilter.PFD.Dsc_NumDecT_noCijMal     ;// 11.05.2020. za theVPC

         Fld_BelowGrid         = TheFakturDocFilter.PFD.Dsc_BelowGrid            ;  
         Fld_OcuHeader         = TheFakturDocFilter.PFD.Dsc_OcuHeader            ;
         Fld_OcuFooter         = TheFakturDocFilter.PFD.Dsc_OcuFooter            ;
         Fld_OcuFooter2        = TheFakturDocFilter.PFD.Dsc_OcuFooter2            ;
         Fld_OcuLogo           = TheFakturDocFilter.PFD.Dsc_OcuLogo              ;
         Fld_ScalingLogo       = TheFakturDocFilter.PFD.Dsc_ScalingPostoLogo     ;
         Fld_OcuIspisPnb       = TheFakturDocFilter.PFD.Dsc_OcuIspisPnb          ;
         
         Fld_IsAddTT           = TheFakturDocFilter.PFD.Dsc_IsAddTT              ;
         Fld_SeparIfTT_Rn      = TheFakturDocFilter.PFD.Dsc_SeparIfTT_Rn         ;

         Fld_FontOpis          = TheFakturDocFilter.PFD.Dsc_FontOpis   ;
         Fld_FontColumns       = TheFakturDocFilter.PFD.Dsc_FontColumns;
         Fld_FontBelGr         = TheFakturDocFilter.PFD.Dsc_FontBelGr  ;

         Fld_OcuR12            = TheFakturDocFilter.PFD.Dsc_OcuR12  ;
         Fld_JezikReporta      = TheFakturDocFilter.PFD.Dsc_JezikReport  ;

         Fld_blgOcuColKonto    = TheFakturDocFilter.PFD.Dsc_BlgOcuColKonto  ;
         Fld_blgOcuColRacun    = TheFakturDocFilter.PFD.Dsc_BlgOcuColRacun  ;
         Fld_blgOcu2na1strani  = TheFakturDocFilter.PFD.Dsc_BlgOcu2na1strani;
         Fld_blgOcuOkvirUplsp  = TheFakturDocFilter.PFD.Dsc_BlgOcuOkvirUplsp;

         Fld_AdresOnlyPartner       = TheFakturDocFilter.PFD.Dsc_AdresOnlyPartner      ;
         Fld_OcuKupDobTel           = TheFakturDocFilter.PFD.Dsc_OcuKupDobTel          ;       
         Fld_OcuProjektTel          = TheFakturDocFilter.PFD.Dsc_OcuProjektTel         ;
         Fld_OcuKupDobFax           = TheFakturDocFilter.PFD.Dsc_OcuKupDobFax          ;
         Fld_OcuProjektFax          = TheFakturDocFilter.PFD.Dsc_OcuProjektFax         ;
         Fld_OcuKupDobOib           = TheFakturDocFilter.PFD.Dsc_OcuKupDobOib          ;
         Fld_OcuProjektOib          = TheFakturDocFilter.PFD.Dsc_OcuProjektOib         ;
         Fld_OcuKupDobMail          = TheFakturDocFilter.PFD.Dsc_OcuKupDobMail         ;
         Fld_OcuProjektMail         = TheFakturDocFilter.PFD.Dsc_OcuProjektMail        ;
         Fld_OcuKupDobNr            = TheFakturDocFilter.PFD.Dsc_OcuKupDobNr           ;
         Fld_PotvrdaNarudzbe        = TheFakturDocFilter.PFD.Dsc_PotvrdaNarudzbe       ;
         Fld_OcuIspisDospjecePl     = TheFakturDocFilter.PFD.Dsc_OcuIspisDospjecePl    ;
         Fld_OcuIspisVeze1          = TheFakturDocFilter.PFD.Dsc_OcuIspisVeze1         ;
         Fld_OcuIspisVeze2          = TheFakturDocFilter.PFD.Dsc_OcuIspisVeze2         ;
         Fld_OcuIspisVeze3          = TheFakturDocFilter.PFD.Dsc_OcuIspisVeze3         ;
         Fld_OcuIspisVeze4          = TheFakturDocFilter.PFD.Dsc_OcuIspisVeze4         ;
         Fld_OcuIspisVezDok2        = TheFakturDocFilter.PFD.Dsc_OcuIspisVezDok2       ;
         Fld_OcuIspisNapomena2      = TheFakturDocFilter.PFD.Dsc_OcuIspisNapomena2     ;
         Fld_OcuIspisDokNum2        = TheFakturDocFilter.PFD.Dsc_OcuIspisDokNum2       ;
         Fld_OcuMemoHOnAllPages     = TheFakturDocFilter.PFD.Dsc_OcuMemoHOnAllPages    ;
         Fld_OcuMemoFOnAllPages     = TheFakturDocFilter.PFD.Dsc_OcuMemoFOnAllPages    ;
         Fld_LblVeze1               = TheFakturDocFilter.PFD.Dsc_LblVeze1              ;
         Fld_LblVeze2               = TheFakturDocFilter.PFD.Dsc_LblVeze2              ;
         Fld_LblVeze3               = TheFakturDocFilter.PFD.Dsc_LblVeze3              ;
         Fld_LblVeze4               = TheFakturDocFilter.PFD.Dsc_LblVeze4              ;
         Fld_LblVezDok2             = TheFakturDocFilter.PFD.Dsc_LblVezDok2            ;
         Fld_LblOsobaX              = TheFakturDocFilter.PFD.Dsc_LblOsobaX             ;
         Fld_AboveGrid              = TheFakturDocFilter.PFD.Dsc_AboveGrid             ;
         Fld_AlignmentPageNum       = TheFakturDocFilter.PFD.Dsc_AlignmentPageNum      ;
         Fld_PrintPageNum           = TheFakturDocFilter.PFD.Dsc_PrintPageNum          ;
         Fld_AlignmentDokNum        = TheFakturDocFilter.PFD.Dsc_AlignmentDokNum       ;
         Fld_AdresOkvir             = TheFakturDocFilter.PFD.Dsc_AdresOkvir            ;
         Fld_PrintDokNum_BeforAdres = TheFakturDocFilter.PFD.Dsc_PrintDokNum_BeforAdres;
         Fld_MigPositionHeader      = TheFakturDocFilter.PFD.Dsc_MigPositionHeader     ;
         Fld_OcuTitleOkvir          = TheFakturDocFilter.PFD.Dsc_OcuTitleOkvir         ;
         Fld_OcuTitleBoja           = TheFakturDocFilter.PFD.Dsc_OcuTitleBoja          ;
         Fld_OcuKupDobBoja          = TheFakturDocFilter.PFD.Dsc_OcuKupDobBoja         ;
         Fld_OcuPrjktBoja           = TheFakturDocFilter.PFD.Dsc_OcuPrjktBoja          ;
         Fld_OcuLinijeHeader        = TheFakturDocFilter.PFD.Dsc_OcuLinijeHeader       ;
         Fld_OcuLinijeFooter        = TheFakturDocFilter.PFD.Dsc_OcuLinijeFooter       ;
         Fld_BeforNRD               = TheFakturDocFilter.PFD.Dsc_BeforNRD              ;
         Fld_AfterNRD               = TheFakturDocFilter.PFD.Dsc_AfterNRD              ;
         Fld_OcuIspisVirmana        = TheFakturDocFilter.PFD.Dsc_OcuIspisVirmana       ;
         Fld_ObrazacA               = TheFakturDocFilter.PFD.Dsc_ObrazacA              ;
         Fld_ObrazacB               = TheFakturDocFilter.PFD.Dsc_ObrazacB              ;
         Fld_ObrazacC               = TheFakturDocFilter.PFD.Dsc_ObrazacC              ;
         Fld_ObrazacD               = TheFakturDocFilter.PFD.Dsc_ObrazacD              ;
         Fld_OcuKDZiro_Vir          = TheFakturDocFilter.PFD.Dsc_OcuKDZiro_Vir         ;
         Fld_OcuVezDok              = TheFakturDocFilter.PFD.Dsc_OcuIspisLblNapomena   ;

         Fld_LeftMargin             = TheFakturDocFilter.PFD.Dsc_LeftMargin            ;
         Fld_RightMargin            = TheFakturDocFilter.PFD.Dsc_RightMargin           ;
         Fld_OcuLinijePerson        = TheFakturDocFilter.PFD.Dsc_OcuLinijePerson       ;
         Fld_OcuZiroFromFak         = TheFakturDocFilter.PFD.Dsc_OcuZiroFromFak        ;
         Fld_IspisNapomene          = TheFakturDocFilter.PFD.Dsc_IspisNapomene         ;
         Fld_OcuTextNap2            = TheFakturDocFilter.PFD.Dsc_OcuTextNap2           ;
         Fld_PositionPersonR        = TheFakturDocFilter.PFD.Dsc_PositionPersonR       ;
         Fld_OcuFirmuUpotpis        = TheFakturDocFilter.PFD.Dsc_OcuFirmuUpotpis       ;
         Fld_OcuTitulu              = TheFakturDocFilter.PFD.Dsc_OcuTitulu             ;
         Fld_OcuJednakiFtTxt2Red    = TheFakturDocFilter.PFD.Dsc_OcuJednakiFtTxt2Red;

         Fld_FakAdresKaoPoslJed     = TheFakturDocFilter.PFD.Dsc_FakAdresKaoPoslJed;
         Fld_PlVirUrokuDana         = TheFakturDocFilter.PFD.Dsc_PlVirUrokuDana    ;
         Fld_IspisOtpremnicaBr      = TheFakturDocFilter.PFD.Dsc_IspisOtpremnicaBr ;
         Fld_IspisUgovora           = TheFakturDocFilter.PFD.Dsc_IspisUgovora      ;
         Fld_OnlyArtiklLongOpis     = TheFakturDocFilter.PFD.Dsc_OnlyArtiklLongOpis;   // UKUPNA TEZINA ONLY BEZ KOLONE
         Fld_CentarNapKaoNaslov     = TheFakturDocFilter.PFD.Dsc_CentarNapKaoNaslov;
         Fld_OibIspodAdreseOnlyP    = TheFakturDocFilter.PFD.Dsc_OibIspodAdreseOnlyP;
         Fld_VisibleProjektCd       = TheFakturDocFilter.PFD.Dsc_VisibleProjektCd;

         Fld_OcuDevCijAndDevTec     = TheFakturDocFilter.PFD.Dsc_OcuDevCijAndDevTec;
         Fld_OcuRokIsporDokDate     = TheFakturDocFilter.PFD.Dsc_OcuRokIsporDokDate;
         Fld_OcuMtrosName           = TheFakturDocFilter.PFD.Dsc_OcuMtrosName      ;

         Fld_TextPostoBefore        = TheFakturDocFilter.PFD.Dsc_TextPostoBefore;
         Fld_TextPostoAfter         = TheFakturDocFilter.PFD.Dsc_TextPostoAfter ;
         
         Fld_OcuDateX               = TheFakturDocFilter.PFD.Dsc_OcuDateX       ;
         Fld_LblDateX               = TheFakturDocFilter.PFD.Dsc_LblDateX       ;
         Fld_OcuPomakVirmana        = TheFakturDocFilter.PFD.Dsc_OcuPomakVirmana;
       //Fld_OcuPosPrint            = TheFakturDocFilter.PFD.Dsc_OcuPosPrint    ;
         if(TheVvUC is IRMDUC)
         {
            Fld_OcuPosPrint  = ZXC.TheVvForm.VvPref.fakturIRMDUC.isPOSprint;
            Fld_PosPrintKind = ZXC.TheVvForm.VvPref.fakturIRMDUC.posPrintKind;
         }
         if(TheVvUC is IRMDUC_2)
         {
            Fld_OcuPosPrint = ZXC.TheVvForm.VvPref.fakturIRMDUC2.isPOSprint;
         }
         Fld_OcuMojuPoslJed         = TheFakturDocFilter.PFD.Dsc_OcuMojuPoslJed ;
         Fld_BelowOnPOS             = TheFakturDocFilter.PFD.Dsc_BelowOnPOS     ;
         Fld_TekstOslobodenPDV      = TheFakturDocFilter.PFD.Dsc_TekstOslobodenPDV ;

         Fld_MemoPOS                = TheFakturDocFilter.PFD.Dsc_MemoPOS;
         Fld_MemoAdd                = TheFakturDocFilter.PFD.Dsc_MemoAdd;  
         Fld_OcuDatumRacuna         = TheFakturDocFilter.PFD.Dsc_OcuDatumRacuna ;
         Fld_OcuNapomUmjKupDob      = TheFakturDocFilter.PFD.Dsc_OcuNapomUmjKupDob;
         Fld_OcuLikvidator          = TheFakturDocFilter.PFD.Dsc_OcuLikvidator;

         Fld_ScalingLogo2_FP        = TheFakturDocFilter.PFD.Dsc_ScalingLogo2_FP   ;
         Fld_AlignmentLogo2_FP      = TheFakturDocFilter.PFD.Dsc_AlignmentLogo2_FP ;
         Fld_IsLogo2_FPN            = TheFakturDocFilter.PFD.Dsc_IsLogo2_FPN       ;
         Fld_AlignmentMemoAdd       = TheFakturDocFilter.PFD.Dsc_AlignmentMemoAdd  ;
         Fld_OcuOTS_saldo           = TheFakturDocFilter.PFD.Dsc_OcuOTS_saldo      ;
         Fld_OcuBarkodTtNum         = TheFakturDocFilter.PFD.Dsc_OcuBarkodTtNum    ;
         Fld_OcuBarKodPDF417        = TheFakturDocFilter.PFD.Dsc_OcuBarKodPDF417   ;
         Fld_Necu_prikazEUR         = TheFakturDocFilter.PFD.Dsc_Necu_prikazEUR    ;
         Fld_OcuMemoAddGore         = TheFakturDocFilter.PFD.Dsc_OcuMemoAddGore    ;
         Fld_NecuFiskalDodatak      = TheFakturDocFilter.PFD.Dsc_NecuFiskalDodatak ;

         Fld_OcuKupDobOpisOnlyP     = TheFakturDocFilter.PFD.Dsc_OcuDugoImeOnlyP;

      }

      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
      if(TheFakturDocFilter.IsRNP_Analiza)
      {
         TheFakturDocFilter.RNP_Rezervacija1 = Fld_Rezervacija1;
         TheFakturDocFilter.RNP_Rezervacija2 = Fld_Rezervacija2;
      }

      if(TheFakturDocFilter.PFD == null) return;

      TheFakturDocFilter.PFD.Dsc_HorizontalLine   = Fld_NeedsHorizontalLine;
      TheFakturDocFilter.PFD.Dsc_AdresaLeftRight  = Fld_AdresaLeftRight;
      TheFakturDocFilter.PFD.Dsc_RptOrientation   = Fld_RptOrientation ;
      TheFakturDocFilter.PFD.Dsc_VertikalLine     = Fld_VertikalLine   ;
      TheFakturDocFilter.PFD.Dsc_TableBorder      = Fld_TableBorder    ;
      TheFakturDocFilter.PFD.Dsc_TekstualIznos    = Fld_TekstualIznos  ;
      TheFakturDocFilter.PFD.Dsc_RazmakRows       = Fld_RazmakRows     ;
                                                  
      TheFakturDocFilter.PFD.Dsc_IsAddTtNum       = Fld_IsAddTtNum      ;
      TheFakturDocFilter.PFD.Dsc_IsAddYear        = Fld_IsAddYear       ;
      TheFakturDocFilter.PFD.Dsc_IsAddKupDobCd    = Fld_IsAddKupDobCd   ;
      TheFakturDocFilter.PFD.Dsc_Prefix1Rn        = Fld_PrefixIR1       ;
      TheFakturDocFilter.PFD.Dsc_Prefix2Rn        = Fld_PrefixIR2       ;
      TheFakturDocFilter.PFD.Dsc_PnbM             = Fld_PnbM            ;
      TheFakturDocFilter.PFD.Dsc_Separ1_Rn        = Fld_PrefixIR1_Rn    ;
      TheFakturDocFilter.PFD.Dsc_Separ1_Pn        = Fld_PrefixIR1_Pn    ;
      TheFakturDocFilter.PFD.Dsc_Separ2_Rn        = Fld_PrefixIR2_Rn    ;
      TheFakturDocFilter.PFD.Dsc_Separ2_Pn        = Fld_PrefixIR2_Pn    ;
      TheFakturDocFilter.PFD.Dsc_SeparIfTtNum_Rn  = Fld_IsAddTtNum_Rn   ;
      TheFakturDocFilter.PFD.Dsc_SeparIfTtNum_Pn  = Fld_IsAddTtNum_Pn   ;
      TheFakturDocFilter.PFD.Dsc_SeparIfYear_Rn   = Fld_IsAddYear_Rn    ;
      TheFakturDocFilter.PFD.Dsc_SeparIfYear_Pn   = Fld_IsAddYear_Pn    ;
      TheFakturDocFilter.PFD.Dsc_SeparIfKDcd_Rn   = Fld_IsAddKupDobCd_Rn;
      TheFakturDocFilter.PFD.Dsc_SeparIfKDcd_Pn   = Fld_IsAddKupDobCd_Pn;
      TheFakturDocFilter.PFD.Dsc_Title            = Fld_Title           ;
      TheFakturDocFilter.PFD.Dsc_PrefixIR1Pb      = Fld_PrefixIR1Pb     ;
      TheFakturDocFilter.PFD.Dsc_PrefixIR2PbTx    = Fld_PrefixIR2Pb     ;
      TheFakturDocFilter.PFD.Dsc_IsAddTtNum_Pb    = Fld_IsAddTtNum_Pb   ;
      TheFakturDocFilter.PFD.Dsc_IsAddYear_Pb     = Fld_IsAddYear_Pb    ;
      TheFakturDocFilter.PFD.Dsc_IsAddKupDobCd_Pb = Fld_IsAddKupDobCd_Pb;

      TheFakturDocFilter.PFD.Dsc_LblPrjktPerson   = Fld_LblPrjktPerson  ;
      TheFakturDocFilter.PFD.Dsc_LblUserPerson    = Fld_LblUserPerson   ;
      TheFakturDocFilter.PFD.Dsc_LblOsobaA        = Fld_LblOsobaA       ;
      TheFakturDocFilter.PFD.Dsc_LblOsobaB        = Fld_LblOsobaB       ;
      TheFakturDocFilter.PFD.Dsc_SignPrimaoc      = Fld_SignPrimaoc     ;
      TheFakturDocFilter.PFD.Dsc_LblPrimio        = Fld_LblPrimio       ;
      TheFakturDocFilter.PFD.Dsc_LblOpciA         = Fld_LblOpciA        ;
      TheFakturDocFilter.PFD.Dsc_LblOpciB         = Fld_LblOpciB        ;
                                                  
      TheFakturDocFilter.PFD.Dsc_NazivPoslJed     = Fld_NazivPoslJed    ;
      
      TheFakturDocFilter.ChosenObrazac            = Fld_ChoseObrazac;

      TheFakturDocFilter.PFD.Dsc_T_artiklCD        = Fld_T_artiklCD       ;
      TheFakturDocFilter.PFD.Dsc_T_artiklName      = Fld_T_artiklName     ;
      TheFakturDocFilter.PFD.Dsc_NapomenaArt       = Fld_Napomena         ;
      TheFakturDocFilter.PFD.Dsc_BarCode1          = Fld_BarCode1         ;
      TheFakturDocFilter.PFD.Dsc_ArtiklCD2         = Fld_ArtiklCD2        ;
      TheFakturDocFilter.PFD.Dsc_ArtiklName2       = Fld_ArtiklName2      ;
      TheFakturDocFilter.PFD.Dsc_BarCode2          = Fld_BarCode2         ;
      TheFakturDocFilter.PFD.Dsc_LongOpis          = Fld_LongOpis         ;
      TheFakturDocFilter.PFD.Dsc_SerNo             = Fld_SerNo            ;
      TheFakturDocFilter.PFD.Dsc_T_jedMj           = Fld_T_jedMj          ;
      TheFakturDocFilter.PFD.Dsc_T_kol             = Fld_T_kol            ;
      TheFakturDocFilter.PFD.Dsc_T_cij             = Fld_T_cij            ;
      TheFakturDocFilter.PFD.Dsc_R_KC              = Fld_R_KC             ;
      TheFakturDocFilter.PFD.Dsc_T_rbt1St          = Fld_T_rbt1St         ;
      TheFakturDocFilter.PFD.Dsc_R_rbt1            = Fld_R_rbt1           ;
      TheFakturDocFilter.PFD.Dsc_T_rbt2St          = Fld_T_rbt2St         ;
      TheFakturDocFilter.PFD.Dsc_R_rbt2            = Fld_R_rbt2           ;
      TheFakturDocFilter.PFD.Dsc_R_cij_KCR         = Fld_R_cij_KCR        ;
      TheFakturDocFilter.PFD.Dsc_R_KCR             = Fld_R_KCR            ;
      TheFakturDocFilter.PFD.Dsc_T_mrzSt           = Fld_T_mrzSt          ;//  26.02.2016. za T_serlot
      TheFakturDocFilter.PFD.Dsc_R_mrz             = Fld_R_mrz            ;// 18.12.2013. za R_cijOP
      TheFakturDocFilter.PFD.Dsc_R_cij_KCRM        = Fld_R_cij_KCRM       ;// 29.08.2013. za Kn Tkn_Cij 
      TheFakturDocFilter.PFD.Dsc_R_KCRM            = Fld_R_KCRM           ;// 29.08.2013. za Kn Rkn_KCRP
      TheFakturDocFilter.PFD.Dsc_R_ztr             = Fld_R_ztr            ;//030.03.2021. za NBC
      TheFakturDocFilter.PFD.Dsc_T_pdvSt           = Fld_T_pdvSt          ;
      TheFakturDocFilter.PFD.Dsc_R_pdv             = Fld_R_pdv            ;
      TheFakturDocFilter.PFD.Dsc_R_cij_KCRMP       = Fld_R_cij_KCRP       ;
      TheFakturDocFilter.PFD.Dsc_R_KCRMP           = Fld_R_KCRP           ;
      TheFakturDocFilter.PFD.Dsc_T_doCijMal        = Fld_T_doCijMal       ;
      TheFakturDocFilter.PFD.Dsc_T_noCijMal        = Fld_T_noCijMal       ;//11.05.2020. theVPC on IRM
      TheFakturDocFilter.PFD.Dsc_R_mjMasaN         = Fld_R_mjMasaN        ;
      TheFakturDocFilter.PFD.Dsc_T_garancija       = Fld_T_garancija;

      TheFakturDocFilter.PFD.Dsc_NumDecT_kol       = Fld_NumDecT_kol      ;
      TheFakturDocFilter.PFD.Dsc_NumDecT_cij       = Fld_NumDecT_cij      ;
      TheFakturDocFilter.PFD.Dsc_NumDecR_KC        = Fld_NumDecR_KC       ;
      TheFakturDocFilter.PFD.Dsc_NumDecT_rbt1St    = Fld_NumDecT_rbt1St   ;
      TheFakturDocFilter.PFD.Dsc_NumDecR_rbt1      = Fld_NumDecR_rbt1     ;
      TheFakturDocFilter.PFD.Dsc_NumDecT_rbt2St    = Fld_NumDecT_rbt2St   ;
      TheFakturDocFilter.PFD.Dsc_NumDecR_rbt2      = Fld_NumDecR_rbt2     ;
      TheFakturDocFilter.PFD.Dsc_NumDecR_cij_KCR   = Fld_NumDecR_cij_KCR  ;
      TheFakturDocFilter.PFD.Dsc_NumDecR_KCR       = Fld_NumDecR_KCR      ;
      TheFakturDocFilter.PFD.Dsc_NumDecT_mrzSt     = Fld_NumDecT_mrzSt    ;
      TheFakturDocFilter.PFD.Dsc_NumDecR_mrz       = Fld_NumDecR_mrz      ;// 18.12.2013. za R_cijOP
      TheFakturDocFilter.PFD.Dsc_NumDecR_cij_KCRM  = Fld_NumDecR_cij_KCRM ;// 29.08.2013. za Kn Tkn_Cij 
      TheFakturDocFilter.PFD.Dsc_NumDecR_KCRM      = Fld_NumDecR_KCRM     ;// 29.08.2013. za Kn Rkn_KCRP
      TheFakturDocFilter.PFD.Dsc_NumDecR_ztr       = Fld_NumDecR_ztr      ;
      TheFakturDocFilter.PFD.Dsc_NumDecT_pdvSt     = Fld_NumDecT_pdvSt    ;
      TheFakturDocFilter.PFD.Dsc_NumDecR_pdv       = Fld_NumDecR_pdv      ;
      TheFakturDocFilter.PFD.Dsc_NumDecR_cij_KCRMP = Fld_NumDecR_cij_KCRP ;
      TheFakturDocFilter.PFD.Dsc_NumDecR_KCRMP     = Fld_NumDecR_KCRP     ;
      TheFakturDocFilter.PFD.Dsc_NumDecT_doCijMal  = Fld_NumDecT_doCijMal ;
      TheFakturDocFilter.PFD.Dsc_NumDecT_noCijMal  = Fld_NumDecT_noCijMal ;

      TheFakturDocFilter.PFD.Dsc_BelowGrid         = Fld_BelowGrid        ;  
      TheFakturDocFilter.PFD.Dsc_OcuHeader         = Fld_OcuHeader        ;
      TheFakturDocFilter.PFD.Dsc_OcuFooter         = Fld_OcuFooter        ;
      TheFakturDocFilter.PFD.Dsc_OcuFooter2        = Fld_OcuFooter2       ;
      TheFakturDocFilter.PFD.Dsc_OcuLogo           = Fld_OcuLogo          ;
      TheFakturDocFilter.PFD.Dsc_ScalingPostoLogo  = Fld_ScalingLogo      ;
      TheFakturDocFilter.PFD.Dsc_OcuIspisPnb       = Fld_OcuIspisPnb      ;

      TheFakturDocFilter.PFD.Dsc_IsAddTT           = Fld_IsAddTT          ;
      TheFakturDocFilter.PFD.Dsc_SeparIfTT_Rn      = Fld_SeparIfTT_Rn     ;

      TheFakturDocFilter.PFD.Dsc_FontOpis          = Fld_FontOpis        ;
      TheFakturDocFilter.PFD.Dsc_FontColumns       = Fld_FontColumns     ;
      TheFakturDocFilter.PFD.Dsc_FontBelGr         = Fld_FontBelGr       ;

      TheFakturDocFilter.PFD.Dsc_OcuR12                 = Fld_OcuR12                ;
      TheFakturDocFilter.PFD.Dsc_JezikReport            = Fld_JezikReporta          ;
                                                        
      TheFakturDocFilter.PFD.Dsc_BlgOcuColKonto         = Fld_blgOcuColKonto        ;
      TheFakturDocFilter.PFD.Dsc_BlgOcuColRacun         = Fld_blgOcuColRacun        ;
      TheFakturDocFilter.PFD.Dsc_BlgOcu2na1strani       = Fld_blgOcu2na1strani      ;
      TheFakturDocFilter.PFD.Dsc_BlgOcuOkvirUplsp       = Fld_blgOcuOkvirUplsp      ;

      TheFakturDocFilter.PFD.Dsc_OcuKupDobTel           = Fld_OcuKupDobTel          ;       
      TheFakturDocFilter.PFD.Dsc_OcuProjektTel          = Fld_OcuProjektTel         ;
      TheFakturDocFilter.PFD.Dsc_OcuKupDobFax           = Fld_OcuKupDobFax          ;
      TheFakturDocFilter.PFD.Dsc_OcuProjektFax          = Fld_OcuProjektFax         ;
      TheFakturDocFilter.PFD.Dsc_OcuKupDobOib           = Fld_OcuKupDobOib          ;
      TheFakturDocFilter.PFD.Dsc_OcuProjektOib          = Fld_OcuProjektOib         ;
      TheFakturDocFilter.PFD.Dsc_OcuKupDobMail          = Fld_OcuKupDobMail         ;
      TheFakturDocFilter.PFD.Dsc_OcuProjektMail         = Fld_OcuProjektMail        ;
      TheFakturDocFilter.PFD.Dsc_OcuKupDobNr            = Fld_OcuKupDobNr           ;
      TheFakturDocFilter.PFD.Dsc_PotvrdaNarudzbe        = Fld_PotvrdaNarudzbe       ;
      TheFakturDocFilter.PFD.Dsc_OcuIspisDospjecePl     = Fld_OcuIspisDospjecePl    ;
      TheFakturDocFilter.PFD.Dsc_OcuIspisVeze1          = Fld_OcuIspisVeze1         ;
      TheFakturDocFilter.PFD.Dsc_OcuIspisVeze2          = Fld_OcuIspisVeze2         ;
      TheFakturDocFilter.PFD.Dsc_OcuIspisVeze3          = Fld_OcuIspisVeze3         ;
      TheFakturDocFilter.PFD.Dsc_OcuIspisVeze4          = Fld_OcuIspisVeze4         ;
      TheFakturDocFilter.PFD.Dsc_OcuIspisVezDok2        = Fld_OcuIspisVezDok2       ;
      TheFakturDocFilter.PFD.Dsc_OcuIspisNapomena2      = Fld_OcuIspisNapomena2     ;
      TheFakturDocFilter.PFD.Dsc_OcuIspisDokNum2        = Fld_OcuIspisDokNum2       ;
      TheFakturDocFilter.PFD.Dsc_OcuMemoHOnAllPages     = Fld_OcuMemoHOnAllPages    ;
      TheFakturDocFilter.PFD.Dsc_OcuMemoFOnAllPages     = Fld_OcuMemoFOnAllPages    ;
      TheFakturDocFilter.PFD.Dsc_LblVeze1               = Fld_LblVeze1              ;
      TheFakturDocFilter.PFD.Dsc_LblVeze2               = Fld_LblVeze2              ;
      TheFakturDocFilter.PFD.Dsc_LblVeze3               = Fld_LblVeze3              ;
      TheFakturDocFilter.PFD.Dsc_LblVeze4               = Fld_LblVeze4              ;
      TheFakturDocFilter.PFD.Dsc_LblVezDok2             = Fld_LblVezDok2            ;
      TheFakturDocFilter.PFD.Dsc_LblOsobaX              = Fld_LblOsobaX             ;
      TheFakturDocFilter.PFD.Dsc_AboveGrid              = Fld_AboveGrid             ;
      TheFakturDocFilter.PFD.Dsc_AlignmentPageNum       = Fld_AlignmentPageNum      ;
      TheFakturDocFilter.PFD.Dsc_PrintPageNum           = Fld_PrintPageNum          ;
      TheFakturDocFilter.PFD.Dsc_AlignmentDokNum        = Fld_AlignmentDokNum       ;
      TheFakturDocFilter.PFD.Dsc_AdresOkvir             = Fld_AdresOkvir            ;
      TheFakturDocFilter.PFD.Dsc_AdresOnlyPartner       = Fld_AdresOnlyPartner      ;
      TheFakturDocFilter.PFD.Dsc_PrintDokNum_BeforAdres = Fld_PrintDokNum_BeforAdres;
      TheFakturDocFilter.PFD.Dsc_MigPositionHeader      = Fld_MigPositionHeader     ;
      TheFakturDocFilter.PFD.Dsc_OcuTitleOkvir          = Fld_OcuTitleOkvir         ;
      TheFakturDocFilter.PFD.Dsc_OcuTitleBoja           = Fld_OcuTitleBoja          ;
      TheFakturDocFilter.PFD.Dsc_OcuKupDobBoja          = Fld_OcuKupDobBoja         ;
      TheFakturDocFilter.PFD.Dsc_OcuPrjktBoja           = Fld_OcuPrjktBoja          ;
      TheFakturDocFilter.PFD.Dsc_OcuLinijeHeader        = Fld_OcuLinijeHeader       ;
      TheFakturDocFilter.PFD.Dsc_OcuLinijeFooter        = Fld_OcuLinijeFooter       ;
      TheFakturDocFilter.PFD.Dsc_BeforNRD               = Fld_BeforNRD              ;
      TheFakturDocFilter.PFD.Dsc_AfterNRD               = Fld_AfterNRD              ;
      TheFakturDocFilter.PFD.Dsc_OcuIspisVirmana        = Fld_OcuIspisVirmana       ;
      TheFakturDocFilter.PFD.Dsc_ObrazacA               = Fld_ObrazacA              ;
      TheFakturDocFilter.PFD.Dsc_ObrazacB               = Fld_ObrazacB              ;
      TheFakturDocFilter.PFD.Dsc_ObrazacC               = Fld_ObrazacC              ;
      TheFakturDocFilter.PFD.Dsc_ObrazacD               = Fld_ObrazacD              ;
      TheFakturDocFilter.PFD.Dsc_OcuKDZiro_Vir          = Fld_OcuKDZiro_Vir         ;
      TheFakturDocFilter.PFD.Dsc_OcuIspisLblNapomena    = Fld_OcuVezDok             ;

      TheFakturDocFilter.PFD.Dsc_LeftMargin             = Fld_LeftMargin            ;
      TheFakturDocFilter.PFD.Dsc_RightMargin            = Fld_RightMargin           ;
      TheFakturDocFilter.PFD.Dsc_OcuLinijePerson        = Fld_OcuLinijePerson       ;
      TheFakturDocFilter.PFD.Dsc_OcuZiroFromFak         = Fld_OcuZiroFromFak        ;
      TheFakturDocFilter.PFD.Dsc_IspisNapomene          = Fld_IspisNapomene         ;
      TheFakturDocFilter.PFD.Dsc_OcuTextNap2            = Fld_OcuTextNap2           ;
      TheFakturDocFilter.PFD.Dsc_PositionPersonR        = Fld_PositionPersonR       ;
      TheFakturDocFilter.PFD.Dsc_OcuFirmuUpotpis        = Fld_OcuFirmuUpotpis       ;
      TheFakturDocFilter.PFD.Dsc_OcuTitulu              = Fld_OcuTitulu             ;
      TheFakturDocFilter.PFD.Dsc_OcuJednakiFtTxt2Red    = Fld_OcuJednakiFtTxt2Red   ;

      TheFakturDocFilter.PFD.Dsc_FakAdresKaoPoslJed     = Fld_FakAdresKaoPoslJed    ;
      TheFakturDocFilter.PFD.Dsc_PlVirUrokuDana         = Fld_PlVirUrokuDana        ;
      TheFakturDocFilter.PFD.Dsc_IspisOtpremnicaBr      = Fld_IspisOtpremnicaBr     ;
      TheFakturDocFilter.PFD.Dsc_IspisUgovora           = Fld_IspisUgovora          ;
      TheFakturDocFilter.PFD.Dsc_OnlyArtiklLongOpis     = Fld_OnlyArtiklLongOpis    ;   // UKUPNA TEZINA ONLY BEZ KOLONE
      TheFakturDocFilter.PFD.Dsc_CentarNapKaoNaslov     = Fld_CentarNapKaoNaslov    ;
      TheFakturDocFilter.PFD.Dsc_OibIspodAdreseOnlyP    = Fld_OibIspodAdreseOnlyP   ;
      TheFakturDocFilter.PFD.Dsc_VisibleProjektCd       = Fld_VisibleProjektCd      ;

      TheFakturDocFilter.PFD.Dsc_OcuDevCijAndDevTec     = Fld_OcuDevCijAndDevTec    ;
      TheFakturDocFilter.PFD.Dsc_OcuRokIsporDokDate     = Fld_OcuRokIsporDokDate    ;
      TheFakturDocFilter.PFD.Dsc_OcuMtrosName           = Fld_OcuMtrosName ;

      TheFakturDocFilter.PFD.Dsc_TextPostoBefore        = Fld_TextPostoBefore       ;
      TheFakturDocFilter.PFD.Dsc_TextPostoAfter         = Fld_TextPostoAfter        ;

      TheFakturDocFilter.PFD.Dsc_OcuDateX               = Fld_OcuDateX              ;
      TheFakturDocFilter.PFD.Dsc_LblDateX               = Fld_LblDateX              ;
      TheFakturDocFilter.PFD.Dsc_OcuPomakVirmana        = Fld_OcuPomakVirmana       ;
      TheFakturDocFilter.PFD.Dsc_OcuMojuPoslJed         = Fld_OcuMojuPoslJed        ;
      TheFakturDocFilter.PFD.Dsc_BelowOnPOS             = Fld_BelowOnPOS            ;

      TheFakturDocFilter.PFD.Dsc_MemoPOS                = Fld_MemoPOS               ;
      TheFakturDocFilter.PFD.Dsc_MemoAdd                = Fld_MemoAdd               ;  
      TheFakturDocFilter.PFD.Dsc_OcuPosPrint            = Fld_OcuPosPrint           ;
      TheFakturDocFilter.PFD.Dsc_OcuDatumRacuna         = Fld_OcuDatumRacuna        ;
      TheFakturDocFilter.PFD.Dsc_OcuNapomUmjKupDob      = Fld_OcuNapomUmjKupDob     ;
      TheFakturDocFilter.PFD.Dsc_OcuLikvidator          = Fld_OcuLikvidator         ;
      TheFakturDocFilter.PFD.Dsc_ScalingLogo2_FP        = Fld_ScalingLogo2_FP       ;
      TheFakturDocFilter.PFD.Dsc_AlignmentLogo2_FP      = Fld_AlignmentLogo2_FP     ;
      TheFakturDocFilter.PFD.Dsc_IsLogo2_FPN            = Fld_IsLogo2_FPN           ;
      TheFakturDocFilter.PFD.Dsc_AlignmentMemoAdd       = Fld_AlignmentMemoAdd      ;
      TheFakturDocFilter.PFD.Dsc_OcuOTS_saldo           = Fld_OcuOTS_saldo          ;
      TheFakturDocFilter.PFD.Dsc_OcuBarkodTtNum         = Fld_OcuBarkodTtNum        ;
      TheFakturDocFilter.PFD.Dsc_OcuBarKodPDF417        = Fld_OcuBarKodPDF417       ;
      TheFakturDocFilter.PFD.Dsc_OcuMemoAddGore         = Fld_OcuMemoAddGore        ;
      TheFakturDocFilter.PFD.Dsc_NecuFiskalDodatak      = Fld_NecuFiskalDodatak     ;
      TheFakturDocFilter.PFD.Dsc_OcuDugoImeOnlyP        = Fld_OcuKupDobOpisOnlyP    ;
      TheFakturDocFilter.PFD.Dsc_TekstOslobodenPDV      = Fld_TekstOslobodenPDV     ;
      TheFakturDocFilter.PFD.Dsc_Necu_prikazEUR         = Fld_Necu_prikazEUR ;


      if(TheVvUC is IRMDUC)
      {
         ZXC.TheVvForm.VvPref.fakturIRMDUC.isPOSprint   = Fld_OcuPosPrint;
         ZXC.TheVvForm.VvPref.fakturIRMDUC.posPrintKind = Fld_PosPrintKind;
      }
      if(TheVvUC is IRMDUC_2)
      {
         ZXC.TheVvForm.VvPref.fakturIRMDUC2.isPOSprint = Fld_OcuPosPrint;
      }


      TheFakturDocFilter.PFD.SaveDscToLookUpItemList();

      TheFakturDocFilter.DevNameAsEnum              = Fld_DevNameAsEnum;

   }

   #endregion Put & GetFilterFields

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
   }

   #endregion AddFilterMemberz()

   #region ChooseDscLuiList

   private void ChooseDscLuiList(object sender, EventArgs e)
   {
      RadioButton rb = sender as RadioButton;

      (TheVvUC as FakturDUC).LoadDscLuiList_And_PutFilterFields(Fld_ChoseObrazac);

      foreach(Control rbt in hamp_ChoseIra.Controls)
      {
         if(rbt is RadioButton)
         {
            if(rbt == rb) rbt.Tag = true;
            else          rbt.Tag = false;
         }
      }

      this.btn_GO.PerformClick();

   }

   #endregion ChooseDscLuiList

}

#endregion TAB_FakturDocFilterUC

#region FakturDocFilterUC

//public class FakturDocFilterUC    : VvFilterUC
//{
//   #region Fieldz

//   private CheckBox  cbx_verLine, cbx_TableBorder, cbx_signPrimaoc, cbx_tekstualIznos,
//                     cbx_IsAddTtNumRn, cbx_IsAddYearRn, cbx_IsAddKupDobCdRn, cbx_IsAddTT,
//                     cbx_IsAddTtNumPb, cbx_IsAddYearPb, cbx_IsAddKupDobCdPb,
//                     cbx_T_artiklCD   , cbx_T_artiklName , cbx_Napomena     , cbx_BarCode1     , cbx_ArtiklCD2    ,   
//                     cbx_ArtiklName2  , cbx_BarCode2     , cbx_LongOpis     , cbx_SerNo        , cbx_T_jedMj      ,     
//                     cbx_T_kol        , cbx_T_cij        , cbx_R_KC         , cbx_T_rbt1St     , cbx_R_rbt1       ,      
//                     cbx_T_rbt2St     , cbx_R_rbt2       , cbx_R_cij_KCR    , cbx_R_KCR        , cbx_T_mrzSt      ,   
//                     cbx_R_mrz        , cbx_R_cij_KCRM   , cbx_R_KCRM       , cbx_R_ztr        , cbx_T_pdvSt      ,  
//                     cbx_R_pdv        , cbx_R_cij_KCRP   , cbx_R_KCRP       , cbx_T_doCijMal   , cbx_T_noCijMal   , cbx_R_mjMasaN,
//                     cbx_razmak       , cbx_ocuHeader    , cbx_ocuFooter    , cbx_ocuLogo      , cbx_ocuPnb, cbx_ocuR12,
//                     cbx_blgOcuColKonto  , cbx_blgOcuColRacun  , cbx_blgOcu2na1strani, cbx_blgOcuOkvirUplsp,
//                     cbx_OcuKupDobTel       , cbx_OcuProjektTel        ,
//                     cbx_OcuKupDobFax       , cbx_OcuProjektFax        ,
//                     cbx_OcuKupDobOib       , cbx_OcuProjektOib        ,
//                     cbx_OcuKupDobMail      , cbx_OcuProjektMail       ,
//                     cbx_OcuKupDobNr        ,
//                     cbx_PotvrdaNarudzbe    ,
//                     cbx_OcuIspisDospjecePl ,
//                     cbx_OcuIspisVeze1      ,
//                     cbx_OcuIspisVeze2      ,
//                     cbx_OcuIspisVeze3      ,
//                     cbx_OcuIspisVeze4      ,
//                     cbx_OcuIspisVezDok2    ,
//                     cbx_OcuIspisNapomena2  ,
//                     cbx_OcuIspisDokNum2    ,
//                     cbx_OcuMemoHAllPages, cbx_OcuMemoFAllPages,
//                     cbx_OcuTitleOkvir, cbx_OcuTitleBoja,
//                     cbx_OcuKupDobBoja, cbx_OcuPrjktBoja,
//                     cbx_OcuLinijeHeader, cbx_OcuLinijeFooter, cbx_OcuIspisVirmana, cbx_OcuKDZiro_Vir,
//                     cbx_ocuVezDok,
//                     cbx_ocuZiro, cbx_ocuLinijePerson, cbx_ocuTextNap2, cbx_ocuNapomena, cbx_ocuFirmuUpotpis, cbx_ocuTitulu,cbx_textOpisDrugiRed;

//   private VvTextBox   tbx_lblPrjktPerson, tbx_lblUserPerson, tbx_lblOsobaA, tbx_lblOsobaB, 
//                       tbx_ValName, tbx_nazivPoslJed,
//                       tbx_prefixIR1Rn, tbx_prefixIR2Rn, tbx_prefixIR1Pb, tbx_prefixIR2Pb, tbx_pnbM, tbx_nazivPrinta,                                      
//                       tbx_prefixIR1_Rn, tbx_prefixIR2_Rn, tbx_IsAddTtNum_Rn, tbx_IsAddYear_Rn, tbx_IsAddKupDobCd_Rn, tbx_SeparIfTT_Rn,
//                       tbx_prefixIR1_Pn, tbx_prefixIR2_Pn, tbx_IsAddTtNum_Pn, tbx_IsAddYear_Pn, tbx_IsAddKupDobCd_Pn,
//                       tbx_NumDecT_kol      , tbx_NumDecT_cij      , tbx_NumDecR_KC       , tbx_NumDecT_rbt1St   , 
//                       tbx_NumDecR_rbt1     , tbx_NumDecT_rbt2St   , tbx_NumDecR_rbt2     , tbx_NumDecR_cij_KCR  , 
//                       tbx_NumDecR_KCR      , tbx_NumDecT_mrzSt    , tbx_NumDecR_mrz      , tbx_NumDecR_cij_KCRM , 
//                       tbx_NumDecR_KCRM     , tbx_NumDecR_ztr      , tbx_NumDecT_pdvSt    , tbx_NumDecR_pdv      , 
//                       tbx_NumDecR_cij_KCRP, tbx_NumDecR_KCRP    , tbx_NumDecT_doCijMal , tbx_NumDecT_noCijMal  ,
//                       tbx_Primio, tbx_opciA, tbx_opciB, tbx_scalingLogo, tbx_belowGrid,
//                       tbx_hrv, tbx_eng, tbx_njem, tbx_tal,
//                       tbx_LblVeze1  ,
//                       tbx_LblVeze2  ,
//                       tbx_LblVeze3  ,
//                       tbx_LblVeze4  ,
//                       tbx_LblVezDok2,
//                       tbx_LblOsobaX, tbx_AboveGrid,
//                       tbx_beforNRD, tbx_afterNRD,
//                       tbx_obrazacA, tbx_obrazacB, tbx_obrazacC, tbx_obrazacD;

//   private RadioButton rbt_AdresRight, rbt_AdresLeft, rbt_RptPort, rbt_RptLandsc,
//                       rbt_ira1, rbt_ira2, rbt_ira3, rbt_ira4,
//                       rbt_opis7, rbt_opis8, rbt_opis9, rbt_opis10, rbt_opis11, rbt_opis12,
//                       rbt_col7, rbt_col8, rbt_col9, rbt_col10,
//                       rbt_belGr7, rbt_belGr8, rbt_belGr9, rbt_belGr10,
//                       rbt_hrv, rbt_eng, rbt_njem, rbt_tal,
                      
//                       //adresa
//                       rbt_adresOkvir, rbt_adresNoOkvir,
//                       rbt_adresOnlyPartner, rbt_adresBoth,
//                       //pageNum
//                       rbt_alignmentPageNum_L, rbt_alignmentPageNum_C, rbt_alignmentPageNum_R,
//                       rbt_printPageNum_PHeader, rbt_printPageNum_PFooter, rbt_printPageNum_NO,
//                       //dokNum
//                       rbt_alignmentDokNum_L, rbt_alignmentDokNum_C, rbt_alignmentDokNum_R,
//                       rbt_printDokNum_beforAdres, rbt_printDokNum_afterAdres,

//                       rbt_mig_RH, rbt_mig_RF,
//                       rbt_leftMargin1, rbt_leftMargin2, rbt_leftMargin3, rbt_leftMargin4,rbt_leftMargin5,
//                       rbt_rightMargin1, rbt_rightMargin2, rbt_rightMargin3, rbt_rightMargin4,
//                       rbt_personRight, rbt_personCentar;

//   private VvHamper    hamp_Columns, hamp_personi, hamp_adresLR, hamp_orient, hamp_OpenFilter, hamp_RnPnb,
//                       hamp_ChoseIra, hamp_belowGrid, hamp_Cbx, hamp_ostalo, hamp_poslJed, hamp_logo, hamp_valuta, 
//                       hamp_FontOpis, hamp_FontCol, hamp_FontBelGr, hamp_jezik, hamp_Blg,
//                       hamp_adresOkvir, hamp_adresToFrom, hamp_adresSastojci,
//                       hamp_pageHF, hamp_pageLCR,
//                       hamp_dokNumHFadres, hamp_dokNumLCR,
//                       hamp_migHF, hamp_NRD, hamp_memo, hamp_title,
//                       hamp_leftMargin, hamp_rightMargin;
//   private Button      btn_OpenFilter;

//   private int minFilterWidth, maxFilterWidth, maxFilterHeight;
//   private Label lbl_textAbovGr, lbl_Ostalo;

//   //5.5.tam
//  // private Faktur mixer_rec { get { return ((FakturExtDUC)TheVvUC).mixer_rec; } }
//   private Faktur faktur_rec { get { return (TheVvUC is FakturExtDUC ? ((FakturExtDUC)TheVvUC).faktur_rec : ((FakturDUC)TheVvUC).faktur_rec); } }

//   private bool isFakExt;

//   #endregion Fieldz

//   #region  Constructor

//   public FakturDocFilterUC(VvUserControl vvUC)
//   {
//      this.SuspendLayout();
//      TheVvUC = vvUC;
      
//      isFakExt = (TheVvUC is FakturExtDUC);

//      hamperHorLine.Visible = false;

//      CreateHamper_4ButtonsResetGo_Width(ZXC.Q4un);
     
//      CreateHampers();

//      if(ZXC.CURR_userName != ZXC.vvDB_systemSuperUserName && ZXC.CURR_userName != ZXC.vvDB_programSuperUserName && !ZXC.CURR_user_rec.IsSuper)
//         btn_OpenFilter.Visible = false;
//      else
//         btn_OpenFilter.Visible = true;

//      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

//      this.ResumeLayout();
//   }

//   #endregion  Constructor

//   #region Hampers

//   private void CreateHampers()
//   {
//      CreateHamperChoseIra  (out hamp_ChoseIra);
//      CreateHamperOpenFilter(out hamp_OpenFilter);
//      CreateHamperColumns   (out hamp_Columns);
//      RecalculateHamper4buttons();

//      minFilterWidth = hamp_ChoseIra.Width + 2 * razmakIzmjedjuHampera;
//      maxFilterWidth = hamp_Columns.Width + 2 * razmakIzmjedjuHampera + ZXC.QUN + ZXC.Qun2;

//      CalcLocationHampersOnFilter(true);

//      nextX = ZXC.Qun4;
//      nextY = ZXC.Qun4;
//      razmakIzmjedjuHampera = ZXC.Qun4;
//      // ---------------- Page ------------------------------------
//      CreateHamperOrient(out hamp_orient);

//      nextX = hamp_orient.Right + razmakIzmjedjuHampera;
//      CreateHamper_pageHF(out hamp_pageHF);

//      nextX = hamp_pageHF.Right + razmakIzmjedjuHampera;
//      CreateHamper_pageLCR(out hamp_pageLCR);

//      // ---------------- Jezik ----------------------------------
     
//      nextX = hamp_pageLCR.Right + razmakIzmjedjuHampera;
//      CreateHamperValuta(out hamp_valuta);
//      CreateHamperJezik(out hamp_jezik);
//      hamp_jezik.Location = new Point(hamp_valuta.Right-hamp_jezik.Width, hamp_valuta.Bottom + razmakIzmjedjuHampera);

//      nextX = ZXC.Qun4;
//      nextY = hamp_pageLCR.Bottom + razmakIzmjedjuHampera;
//      CreateHamperLeftMargin(out hamp_leftMargin);
     
//      nextX = hamp_leftMargin.Right +  ZXC.Qun4;
//      CreateHamperRightMargin(out hamp_rightMargin);



//      // ---------------- Memo  ------------------------------------
//      nextX = ZXC.Qun4;
//      nextY = hamp_leftMargin.Bottom + razmakIzmjedjuHampera;
//      CreateHamperLogo(out hamp_logo);
//      nextX = hamp_logo.Right + razmakIzmjedjuHampera;
//      CreateHamper_memo(out hamp_memo);

     
//      // ---------------- Adresa  ------------------------------------
//      nextX = ZXC.Qun4;
//      nextY = hamp_logo.Bottom + razmakIzmjedjuHampera;
//      CreateHamperAdres(out hamp_adresLR);

//      nextX = hamp_adresLR.Right + razmakIzmjedjuHampera;
//      CreateHamper_adresOkvir(out hamp_adresOkvir);

//      nextX = hamp_adresOkvir.Right + razmakIzmjedjuHampera;
//      CreateHamper_adresToFrom(out hamp_adresToFrom);

//      nextX = hamp_adresToFrom.Right + razmakIzmjedjuHampera;
//      CreateHamperPoslJed(out hamp_poslJed);

//      nextX = ZXC.Qun4;
//      nextY = hamp_adresLR.Bottom + razmakIzmjedjuHampera;
//      CreateHamper_adresSastojci(out hamp_adresSastojci);
     
//      // ---------------- DokNum  ------------------------------------

//      nextY = hamp_adresSastojci.Bottom + razmakIzmjedjuHampera;
//      CreateHamperRnPnb(out hamp_RnPnb);

//      nextY = hamp_RnPnb.Bottom + razmakIzmjedjuHampera;
//      nextX = ZXC.Qun4;
//      CreateHamper_dokNumHFadres(out hamp_dokNumHFadres);

//      nextX = hamp_dokNumHFadres.Right + razmakIzmjedjuHampera;
//      CreateHamper_dokNumLCR(out hamp_dokNumLCR);
      
//      nextX = hamp_dokNumLCR.Right + razmakIzmjedjuHampera;
//      CreateHamper_dokNumTitle(out hamp_title);

//      // ---------------- Columns  ------------------------------------
//      nextX = ZXC.Qun4;
//      nextY = hamp_title.Bottom + razmakIzmjedjuHampera;
//      hamp_Columns.Location = new Point(nextX, nextY);

//      // ---------------- Font  ------------------------------------

//      nextX = ZXC.Qun4;
//      nextY = hamp_Columns.Bottom + razmakIzmjedjuHampera;
//      CreateHamperFontOpis(out hamp_FontOpis);

//      nextX = hamp_FontOpis.Right + razmakIzmjedjuHampera;
//      CreateHamperFontCol(out hamp_FontCol);

//      nextX = hamp_FontCol.Right + razmakIzmjedjuHampera;
//      CreateHamperFontBelGr(out hamp_FontBelGr);



//      // ---------------- Ostalo  ------------------------------------
//      nextX = hamp_FontBelGr.Right + razmakIzmjedjuHampera;

//      CreateHamper_migHF(out hamp_migHF);

//      nextX = ZXC.Qun4;
//      nextY = hamp_FontBelGr.Bottom + razmakIzmjedjuHampera;

//      CreateHamperPersoni(out hamp_personi);

//      nextY = hamp_personi.Bottom + razmakIzmjedjuHampera;
//      CreateHampOstalo(out hamp_ostalo);

//      nextY = hamp_ostalo.Bottom + razmakIzmjedjuHampera;

//      CreateHampBlg(out hamp_Blg);
      
//      CreateHamperBelowGrid(out hamp_belowGrid);
//      nextY = hamp_belowGrid.Bottom + razmakIzmjedjuHampera;

//      CreateHamperNRD(out hamp_NRD);

//      nextX = hamp_belowGrid.Right + razmakIzmjedjuHampera;
//      nextY = hamp_Columns.Bottom + razmakIzmjedjuHampera;
//      CreateHampCbx(out hamp_Cbx);

//      maxFilterHeight = hamp_NRD.Bottom + 2 * razmakIzmjedjuHampera;

//      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamp_NRD    , hamp_belowGrid.Width, razmakIzmjedjuHampera);
//      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamp_Columns, hamp_Columns.Width  , razmakIzmjedjuHampera);

//      SetVisibilitiOfHampers(false);
//   }

//   private void CreateHamper_adresOkvir(out VvHamper hamper)
//   {
//      hamper = new VvHamper(3, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q2un, ZXC.Q2un };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i]    = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//                         hamper.CreateVvLabel      (0, 0, "OkvirAdrese:", ContentAlignment.MiddleRight);
//      rbt_adresOkvir   = hamper.CreateVvRadioButton(1, 0, null, "Da", TextImageRelation.ImageBeforeText);
//      rbt_adresNoOkvir = hamper.CreateVvRadioButton(2, 0, null, "Ne", TextImageRelation.ImageBeforeText);
//      rbt_adresNoOkvir.Checked = true;
//      rbt_adresNoOkvir.Tag     = true;

//      VvHamper.HamperStyling(hamper);
//   }

//   private void CreateHamper_adresToFrom(out VvHamper hamper)
//   {
//      hamper = new VvHamper(3, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt      = new int[] { ZXC.Q3un + ZXC.Qun4, ZXC.Q2un, ZXC.Q2un };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i]    = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      hamper.CreateVvLabel(0, 0, "SamoPartner:", ContentAlignment.MiddleRight);
//      rbt_adresOnlyPartner = hamper.CreateVvRadioButton(1, 0, null, "Da", TextImageRelation.ImageBeforeText);
//      rbt_adresBoth = hamper.CreateVvRadioButton(2, 0, null, "Ne", TextImageRelation.ImageBeforeText);
//      rbt_adresOnlyPartner.Checked = true;
//      rbt_adresOnlyPartner.Tag = true;

//      VvHamper.HamperStyling(hamper);
//   }

//   private void CreateHamper_adresSastojci(out VvHamper hamper)
//   {
//      hamper = new VvHamper(13, 1, "", this, false, nextX, nextY, razmakHampGroup);
//      //                                     0               1           2        3        4                    5                 6              7                  8        9          10               11                12             
//      hamper.VvColWdt = new int[] { ZXC.Q3un - ZXC.Qun4, ZXC.Q2un, ZXC.Q2un, ZXC.Q2un, ZXC.Q3un - ZXC.Qun4, ZXC.Q2un, ZXC.Q3un - ZXC.Qun4,  ZXC.Q3un - ZXC.Qun4, ZXC.Q2un, ZXC.Q2un, ZXC.Q2un, ZXC.Q3un - ZXC.Qun4, ZXC.Q3un - ZXC.Qun4, };

//      for(int i = 0; i < hamper.VvNumOfCols; i++)
//      {
//         hamper.VvSpcBefCol[i] = ZXC.Qun8;
//      }
//      hamper.VvRightMargin = hamper.VvLeftMargin;


//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//                          hamper.CreateVvLabel       (0, 0, "Partner:", ContentAlignment.MiddleRight);
//      cbx_OcuKupDobTel  = hamper.CreateVvCheckBox_OLD(1, 0, null, "Tel"   , System.Windows.Forms.RightToLeft.No);
//      cbx_OcuKupDobFax  = hamper.CreateVvCheckBox_OLD(2, 0, null, "Fax"   , System.Windows.Forms.RightToLeft.No);
//      cbx_OcuKupDobOib  = hamper.CreateVvCheckBox_OLD(3, 0, null, "OIB"   , System.Windows.Forms.RightToLeft.No);
//      cbx_OcuKupDobMail = hamper.CreateVvCheckBox_OLD(4, 0, null, "e-mail", System.Windows.Forms.RightToLeft.No);
//      cbx_OcuKupDobNr   = hamper.CreateVvCheckBox_OLD(5, 0, null, "N/r"   , System.Windows.Forms.RightToLeft.No);
//      cbx_OcuKupDobBoja = hamper.CreateVvCheckBox_OLD(6, 0, null, "Boja"  , System.Windows.Forms.RightToLeft.No);

//                           hamper.CreateVvLabel       ( 7, 0, "Projekt:", ContentAlignment.MiddleRight);
//      cbx_OcuProjektTel  = hamper.CreateVvCheckBox_OLD( 8, 0, null, "Tel"   , System.Windows.Forms.RightToLeft.No);
//      cbx_OcuProjektFax  = hamper.CreateVvCheckBox_OLD( 9, 0, null, "Fax"   , System.Windows.Forms.RightToLeft.No);
//      cbx_OcuProjektOib  = hamper.CreateVvCheckBox_OLD(10, 0, null, "OIB"   , System.Windows.Forms.RightToLeft.No);
//      cbx_OcuProjektMail = hamper.CreateVvCheckBox_OLD(11, 0, null, "e-mail", System.Windows.Forms.RightToLeft.No);
//      cbx_OcuPrjktBoja   = hamper.CreateVvCheckBox_OLD(12, 0, null, "Boja"  , System.Windows.Forms.RightToLeft.No);

//      VvHamper.HamperStyling(hamper);
//   }

//   private void CreateHamper_pageHF(out VvHamper hamper)
//   {
//      hamper = new VvHamper(4, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un- ZXC.Qun8, ZXC.Q3un- ZXC.Qun4, ZXC.Q2un };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//                                 hamper.CreateVvLabel(0, 0, "IspisStrBr:", ContentAlignment.MiddleRight);
//      rbt_printPageNum_PHeader = hamper.CreateVvRadioButton(1, 0, null, "Header", TextImageRelation.ImageBeforeText);
//      rbt_printPageNum_PFooter = hamper.CreateVvRadioButton(2, 0, null, "Footer", TextImageRelation.ImageBeforeText);
//      rbt_printPageNum_NO      = hamper.CreateVvRadioButton(3, 0, null, "Ne", TextImageRelation.ImageBeforeText);
//      rbt_printPageNum_PHeader.Checked = true;
//      rbt_printPageNum_PHeader.Tag = true;

//      VvHamper.HamperStyling(hamper);
//   }

//   private void CreateHamper_pageLCR(out VvHamper hamper)
//   {
//      hamper = new VvHamper(4, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt      = new int[] { ZXC.Q3un + ZXC.Qun2, ZXC.Q3un- ZXC.Qun4, ZXC.Q3un- ZXC.Qun4, ZXC.Q3un- ZXC.Qun4 };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      hamper.CreateVvLabel(0, 0, "PoravnanjeStr:", ContentAlignment.MiddleRight);
//      rbt_alignmentPageNum_L = hamper.CreateVvRadioButton(1, 0, null, "Lijevo", TextImageRelation.ImageBeforeText);
//      rbt_alignmentPageNum_C = hamper.CreateVvRadioButton(2, 0, null, "Centar", TextImageRelation.ImageBeforeText);
//      rbt_alignmentPageNum_R = hamper.CreateVvRadioButton(3, 0, null, "Desno", TextImageRelation.ImageBeforeText);
//      rbt_alignmentPageNum_R.Checked = true;
//      rbt_alignmentPageNum_R.Tag = true;

//      VvHamper.HamperStyling(hamper);
//   }

//   private void CreateHamper_dokNumHFadres(out VvHamper hamper)
//   {
//      hamper = new VvHamper(3, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//                                   hamper.CreateVvLabel(0, 0, "IspisDokNum:", ContentAlignment.MiddleRight);
//      rbt_printDokNum_beforAdres = hamper.CreateVvRadioButton(1, 0, null, "PrijeAdres", TextImageRelation.ImageBeforeText);
//      rbt_printDokNum_afterAdres = hamper.CreateVvRadioButton(2, 0, null, "PoslijeAdres", TextImageRelation.ImageBeforeText);
//      rbt_printDokNum_beforAdres.Checked = true;
//      rbt_printDokNum_beforAdres.Tag = true;

//      VvHamper.HamperStyling(hamper);
//   }

//   private void CreateHamper_dokNumLCR(out VvHamper hamper)
//   {
//      hamper = new VvHamper(4, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
//      hamper.VvSpcBefCol = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      hamper.CreateVvLabel(0, 0, "PoravnanjeDokNum:", ContentAlignment.MiddleRight);
//      rbt_alignmentDokNum_L = hamper.CreateVvRadioButton(1, 0, null, "Lijevo", TextImageRelation.ImageBeforeText);
//      rbt_alignmentDokNum_C = hamper.CreateVvRadioButton(2, 0, null, "Centar", TextImageRelation.ImageBeforeText);
//      rbt_alignmentDokNum_R = hamper.CreateVvRadioButton(3, 0, null, "Desno", TextImageRelation.ImageBeforeText);
//      rbt_alignmentDokNum_L.Checked = true;
//      rbt_alignmentDokNum_L.Tag = true;

//      VvHamper.HamperStyling(hamper);
//   }

//   private void CreateHamper_migHF(out VvHamper hamper)
//   {
//      hamper = new VvHamper(3, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un - ZXC.Qun4, ZXC.Q3un - ZXC.Qun4 };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;


//      hamper.CreateVvLabel(0, 0, "Prošireno:", ContentAlignment.MiddleRight);
//      rbt_mig_RH = hamper.CreateVvRadioButton(1, 0, null, "Header", TextImageRelation.ImageBeforeText);
//      rbt_mig_RF = hamper.CreateVvRadioButton(2, 0, null, "Footer", TextImageRelation.ImageBeforeText);
//      rbt_mig_RH.Checked = true;
//      rbt_mig_RH.Tag = true;

//      VvHamper.HamperStyling(hamper);
//   }

//   private void CreateHamperChoseIra(out VvHamper hamper)
//   {
//      hamper = new VvHamper(2, 5, "", this, false);

//      hamper.VvColWdt      = new int[] { ZXC.QUN - ZXC.Qun8, ZXC.Q5un };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//                 hamper.CreateVvLabel      (0, 0, "Izbor ispisa:", 1, 0, ContentAlignment.MiddleLeft);
//      rbt_ira1 = hamper.CreateVvRadioButton(0, 1, new EventHandler(ChooseDscLuiList), "", TextImageRelation.ImageBeforeText);
//      rbt_ira2 = hamper.CreateVvRadioButton(0, 2, new EventHandler(ChooseDscLuiList), "", TextImageRelation.ImageBeforeText);
//      rbt_ira3 = hamper.CreateVvRadioButton(0, 3, new EventHandler(ChooseDscLuiList), "", TextImageRelation.ImageBeforeText);
//      rbt_ira4 = hamper.CreateVvRadioButton(0, 4, new EventHandler(ChooseDscLuiList), "", TextImageRelation.ImageBeforeText);
//      rbt_ira1.Checked = true;
//      rbt_ira1.Tag = true;
//      rbt_ira1.Name = "1";
//      rbt_ira2.Name = "2";
//      rbt_ira3.Name = "3";
//      rbt_ira3.Name = "4";

//      tbx_obrazacA = hamper.CreateVvTextBox(1, 1, "tbx_obrazacA", "Naziv obrasca A");
//      tbx_obrazacB = hamper.CreateVvTextBox(1, 2, "tbx_obrazacB", "Naziv obrasca B");
//      tbx_obrazacC = hamper.CreateVvTextBox(1, 3, "tbx_obrazacC", "Naziv obrasca C");
//      tbx_obrazacD = hamper.CreateVvTextBox(1, 4, "tbx_obrazacD", "Naziv obrasca D");
//      tbx_obrazacA.Text = "ObrazacA";
//      tbx_obrazacB.Text = "ObrazacB";
//      tbx_obrazacC.Text = "ObrazacC";
//      tbx_obrazacD.Text = "ObrazacD";
//      tbx_obrazacA.Font = 
//      tbx_obrazacB.Font =
//      tbx_obrazacC.Font = 
//      tbx_obrazacD.Font = ZXC.vvFont.SmallFont;

//      if(TheVvUC is IFADUC)
//      {
//         rbt_ira2    .Visible =
//         rbt_ira3    .Visible =
//         rbt_ira4    .Visible =
//         tbx_obrazacB.Visible =
//         tbx_obrazacC.Visible = true;
//         tbx_obrazacD.Visible = true;
//      }
//      else if(TheVvUC is IRADUC)
//      {
//         rbt_ira2.Visible =
//         rbt_ira3.Visible =
//         tbx_obrazacB.Visible =
//         tbx_obrazacC.Visible = true;
         
//         rbt_ira4    .Visible =
//         tbx_obrazacD.Visible = false;
//      }
//      else
//      {
//         tbx_obrazacB.Visible =
//         tbx_obrazacC.Visible =
//         tbx_obrazacD.Visible =
//         rbt_ira2    .Visible =
//         rbt_ira3    .Visible = 
//         rbt_ira4    .Visible = false;
//      }

//      VvHamper.HamperStyling(hamper);
//   }

//   private void CreateHamperRnPnb(out VvHamper hamper)
//   {
//      hamper = new VvHamper(15, 3, "", this, false, nextX, nextY, razmakHampGroup);

//      //    0        1        2                           
//      hamper.VvColWdt = new int[] { ZXC.Q4un, ZXC.Q3un -ZXC.Qun8, ZXC.QUN , 
//                                         ZXC.QUN - ZXC.Qun8, ZXC.Q2un + ZXC.Qun2, //  3,  4
//                                         ZXC.QUN - ZXC.Qun8, ZXC.Q3un + ZXC.Qun4, //  5,  6
//                                         ZXC.QUN - ZXC.Qun8, ZXC.Q3un + ZXC.Qun4, //  7,  8
//                                         ZXC.QUN - ZXC.Qun8, ZXC.Q3un + ZXC.Qun4, //  9, 10
//                                         ZXC.QUN - ZXC.Qun8, ZXC.Q3un           , // 11, 12
//                                         ZXC.QUN - ZXC.Qun8, ZXC.Q3un           };// 13, 14

//      hamper.VvSpcBefCol = new int[] { ZXC.Qun2, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, 
//                                         ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8,
//                                         ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8};
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//                        hamper.CreateVvLabel (0, 0, "NazivPrinta:", ContentAlignment.MiddleLeft);
//      tbx_nazivPrinta = hamper.CreateVvTextBox(0, 1, "tbx_nazivPrinta", "Naslov za printanje  dokumenta, npr. Račun, Račun-Otpremnica, Račun-Izdatnica ...", 32, 2, 0);

//      cbx_ocuR12 = hamper.CreateVvCheckBox_OLD(1, 0, null, 1, 0, "Ispis R12", System.Windows.Forms.RightToLeft.No);

//      cbx_ocuPnb = hamper.CreateVvCheckBox_OLD(0, 2, null, "IspisPnb", System.Windows.Forms.RightToLeft.No);
//      hamper.CreateVvLabel(1, 2, "ModPlać:", ContentAlignment.MiddleRight);
//      tbx_pnbM = hamper.CreateVvTextBox(2, 2, "tbx_pnbM", "Model plaćnja - mala kućica", 2);
//      tbx_pnbM.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

//      cbx_IsAddTT = hamper.CreateVvCheckBox_OLD(3, 0, null, 1, 0, "TipTrans", System.Windows.Forms.RightToLeft.No);
//      tbx_SeparIfTT_Rn = hamper.CreateVvTextBox(3, 1, "tbx_IsAddTT_Rn", "Znakovni Prefix TT za ispis broja računa", 1);


//                         hamper.CreateVvLabel  (5, 0, "Prefix 1:", 1, 0, ContentAlignment.MiddleLeft);
//      tbx_prefixIR1_Rn = hamper.CreateVvTextBox(5, 1, "tbx_prefixIR1_Rn", "Znakovni Prefix 1 za ispis broja računa", 1);
//      tbx_prefixIR1Rn  = hamper.CreateVvTextBox(6, 1, "tbx_prefixIR1", "Prefix 1 za ispis broja računa", 10);
//      tbx_prefixIR1_Pn = hamper.CreateVvTextBox(5, 2, "tbx_prefixIR1_Pn", "Znakovni Prefix 1 za ispis poziva na broj za plaćanje", 1);
//      tbx_prefixIR1Pb  = hamper.CreateVvTextBox(6, 2, "tbx_prefixIR1Pb", "Prefix 1 za ispis poziva na broj za plaćanje", 10);
//      tbx_prefixIR1Pb.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);


//                         hamper.CreateVvLabel  (7, 0, "Prefix 2:", 1, 0, ContentAlignment.MiddleLeft);
//      tbx_prefixIR2_Rn = hamper.CreateVvTextBox(7, 1, "tbx_prefixIR2_Rn", "Znakovni Prefix 2 za ispis broja računa", 1);
//      tbx_prefixIR2Rn  = hamper.CreateVvTextBox(8, 1, "tbx_prefixIR2", "Prefix 2 za ispis broja računa, ako postoji Prefix1 veže se na njega Prefix1-Prefix2", 10);
//      tbx_prefixIR2_Pn = hamper.CreateVvTextBox(7, 2, "tbx_prefixIR2_Pn", "Znakovni Prefix 2 za ispis poziva na broj za plaćanje", 1);
//      tbx_prefixIR2Pb  = hamper.CreateVvTextBox(8, 2, "tbx_prefixIR2Pb", "Prefix 2 za ispis poziva na broj za plaćanje, ako postoji Prefix1 veže se na njega Prefix1-Prefix2", 10);

//                          hamper.CreateVvLabel       ( 9, 0, "Broj dokumenta", 1, 0, ContentAlignment.MiddleLeft);
//      tbx_IsAddTtNum_Rn = hamper.CreateVvTextBox     ( 9, 1, "tbx_IsAddTtNum_Rn", "Znakovni Prefix TtNum za ispis broja računa", 1);
//      cbx_IsAddTtNumRn  = hamper.CreateVvCheckBox_OLD(10, 1, null, "", System.Windows.Forms.RightToLeft.No);
//      tbx_IsAddTtNum_Pn = hamper.CreateVvTextBox     ( 9, 2, "tbx_IsAddTtNum_Pn", "Znakovni Prefix TtNum za ispis poziva na broj za plaćanje", 1);
//      cbx_IsAddTtNumPb  = hamper.CreateVvCheckBox_OLD(10, 2, null, "", System.Windows.Forms.RightToLeft.No);

//                if(isFakExt) hamper.CreateVvLabel       (11, 0, "Šifra partnera", 1, 0, ContentAlignment.MiddleLeft);
//      tbx_IsAddKupDobCd_Rn = hamper.CreateVvTextBox     (11, 1, "tbx_IsAddKupDobCd_Rn", "Znakovni Prefix sifre partnera za ispis broja računa", 1);
//      cbx_IsAddKupDobCdRn  = hamper.CreateVvCheckBox_OLD(12, 1, null, "", System.Windows.Forms.RightToLeft.No);
//      tbx_IsAddKupDobCd_Pn = hamper.CreateVvTextBox     (11, 2, "tbx_IsAddKupDobCd_Pn", "Znakovni Prefix sifre partnera za ispis poziva na broj za plaćanje", 1);
//      cbx_IsAddKupDobCdPb  = hamper.CreateVvCheckBox_OLD(12, 2, null, "", System.Windows.Forms.RightToLeft.No);

//                         hamper.CreateVvLabel       (13, 0, "Tekuća godina", 1, 0, ContentAlignment.MiddleLeft);
//      tbx_IsAddYear_Rn = hamper.CreateVvTextBox     (13, 1, "tbx_IsAddYear_Rn", "Znakovni Prefix godine za ispis broja računa", 1);
//      cbx_IsAddYearRn  = hamper.CreateVvCheckBox_OLD(14, 1, null, "", System.Windows.Forms.RightToLeft.No);
//      tbx_IsAddYear_Pn = hamper.CreateVvTextBox     (13, 2, "tbx_IsAddYear_Pn", "Znakovni Prefix godine za ispis poziva na broj za plaćanje", 1);
//      cbx_IsAddYearPb  = hamper.CreateVvCheckBox_OLD(14, 2, null, "", System.Windows.Forms.RightToLeft.No);

//      cbx_ocuR12.Visible =
//      cbx_ocuPnb.Visible =
//      tbx_pnbM.Visible =
//      tbx_pnbM.Visible =
//      tbx_prefixIR1_Pn.Visible =
//      tbx_prefixIR1Pb.Visible =
//      tbx_prefixIR2_Pn.Visible =
//      tbx_prefixIR2Pb.Visible =
//      tbx_IsAddTtNum_Pn.Visible =
//      cbx_IsAddTtNumPb.Visible =
//      tbx_IsAddKupDobCd_Rn.Visible =
//      cbx_IsAddKupDobCdRn.Visible =
//      tbx_IsAddKupDobCd_Pn.Visible =
//      cbx_IsAddKupDobCdPb.Visible =
//      tbx_IsAddYear_Pn.Visible =
//      cbx_IsAddYearPb.Visible = isFakExt;

//      VvHamper.HamperStyling(hamper);
//      SetUpAsWriteOnlyTbx(hamper);
//   }

//   private void CreateHamper_dokNumTitle(out VvHamper hamper)
//   {
//      hamper = new VvHamper(3, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      for(int i = 0; i < hamper.VvNumOfCols; i++)
//      {
//         hamper.VvColWdt[i]    = ZXC.Q4un;
//         hamper.VvSpcBefCol[i] = ZXC.Qun4;
//      }
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      cbx_OcuTitleOkvir   = hamper.CreateVvCheckBox_OLD(0, 0, null, "TitleOkvir", RightToLeft.No);
//      cbx_OcuTitleBoja    = hamper.CreateVvCheckBox_OLD(1, 0, null, "TitleBoja" , RightToLeft.No);
//      cbx_OcuIspisDokNum2 = hamper.CreateVvCheckBox_OLD(2, 0, null, "DokNum2put", RightToLeft.No); // jos jednom ponovljeni broj racuna
//      VvHamper.HamperStyling(hamper);

//   }

//   private void CreateHampCbx(out VvHamper hamper)
//   {
//      hamper = new VvHamper(1, 17, "", this, false, nextX, nextY, razmakHampGroup);

//      for(int i = 0; i < hamper.VvNumOfCols; i++)
//      {
//         hamper.VvColWdt[i]    = ZXC.Q4un + ZXC.Qun2;
//         hamper.VvSpcBefCol[i] = ZXC.Qun4;
//      }
//      hamper.VvRightMargin = hamper.VvLeftMargin;
      
//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i]    = ZXC.QUN - ZXC.Qun8;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;
     
//                               hamper.CreateVvLabel       (0,  0,       "TABLICA:"       , ContentAlignment.MiddleLeft);
//      cb_Line                = hamper.CreateVvCheckBox_OLD(0,  1, null, "HorizontCrte"   , RightToLeft.No);
//      cbx_verLine            = hamper.CreateVvCheckBox_OLD(0,  2, null, "VertikalOkvir"  , RightToLeft.No);
//      cbx_TableBorder        = hamper.CreateVvCheckBox_OLD(0,  3, null, "SiviNaslov"     , RightToLeft.No);
//      cbx_tekstualIznos      = hamper.CreateVvCheckBox_OLD(0,  4, null, "TekstIznos"     , RightToLeft.No);
//      cbx_razmak             = hamper.CreateVvCheckBox_OLD(0,  5, null, "VećiRazmakRed"  , RightToLeft.No);
//      cbx_ocuTitulu          = hamper.CreateVvCheckBox_OLD(0,  6, null, "VećiSveukupno"  , RightToLeft.No);  // veci za 2 i deblji sveukupno 
//      cbx_textOpisDrugiRed   = hamper.CreateVvCheckBox_OLD(0,  7, null, "JednakiFont2Red", RightToLeft.No);  // jednaki font za redak bez kolicine i cijene
//         lbl_Ostalo          = hamper.CreateVvLabel       (0,  8,       "OSTALO:"      , ContentAlignment.MiddleLeft);
//      cbx_OcuIspisDospjecePl = hamper.CreateVvCheckBox_OLD(0,  9, null, "IspisDospjeća", RightToLeft.No);
//      cbx_ocuNapomena        = hamper.CreateVvCheckBox_OLD(0, 10, null, "IspisNapomene", RightToLeft.No);
//      cbx_OcuIspisNapomena2  = hamper.CreateVvCheckBox_OLD(0, 11, null, "IspisNapom2"  , RightToLeft.No); // uopce da se ispise nampomena2
//      cbx_ocuTextNap2        = hamper.CreateVvCheckBox_OLD(0, 12, null, "IspisTextNap2", RightToLeft.No); // da se ispise text nampomena2
//      cbx_OcuIspisVirmana    = hamper.CreateVvCheckBox_OLD(0, 13, null, "IspisVirmana" , RightToLeft.No);
//      cbx_OcuKDZiro_Vir      = hamper.CreateVvCheckBox_OLD(0, 14, null, "KDziroVir"    , RightToLeft.No);
//      cbx_ocuVezDok          = hamper.CreateVvCheckBox_OLD(0, 15, null, "OrigBrDok"    , RightToLeft.No);
//      cbx_ocuZiro            = hamper.CreateVvCheckBox_OLD(0, 16, null, "IspisŽiroRn"  , RightToLeft.No);



//      if(TheVvUC is BlgIsplatDUC || TheVvUC is BlgUplatDUC)
//      {
//         cbx_ocuTitulu         .Visible =
//         cbx_verLine           .Visible =
//         cbx_tekstualIznos     .Visible =
//         cbx_razmak            .Visible =
//         cbx_OcuIspisDospjecePl.Visible =
//         cbx_PotvrdaNarudzbe   .Visible =
//         cbx_OcuTitleOkvir     .Visible =
//         cbx_OcuIspisNapomena2 .Visible =
//         cbx_ocuTextNap2       .Visible =
//         cbx_OcuIspisDokNum2   .Visible =
//         cbx_ocuVezDok         .Visible =
//         cbx_OcuIspisVirmana   .Visible =
//         cbx_OcuKDZiro_Vir     .Visible =
//         cbx_textOpisDrugiRed  .Visible =
//         cbx_ocuZiro           .Visible = false;
//      }

//      if(isFakExt == false)
//      {
//         cbx_ocuTitulu         .Visible =
//         cbx_razmak            .Visible =
//         cbx_OcuIspisDospjecePl.Visible =
//         cbx_PotvrdaNarudzbe   .Visible =
//         cbx_OcuIspisNapomena2 .Visible =
//         cbx_ocuTextNap2       .Visible =
//         cbx_OcuIspisDokNum2   .Visible =
//         cbx_ocuVezDok         .Visible =
//         cbx_OcuIspisVirmana   .Visible =
//         cbx_ocuZiro           .Visible =
//         cbx_textOpisDrugiRed  .Visible =
//         cbx_OcuKDZiro_Vir     .Visible = false;
//         lbl_Ostalo            .Visible = false;
//      }

//      VvHamper.HamperStyling(hamper);
//   }

//   private void CreateHamperColumns(out VvHamper hamper)
//   {
//      hamper = new VvHamper(16, 5, "", this, false);

//      hamper.VvColWdt = new int[] { ZXC.Q4un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.Q4un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, 
//                                         ZXC.Q4un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.Q4un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4,
//                                         ZXC.Q4un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.Q4un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4,
//                                         ZXC.Q4un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.Q4un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4
//      };
//      hamper.VvSpcBefCol = new int[] { ZXC.Qun2,                     ZXC.Qun10, ZXC.QUN - ZXC.Qun4,           ZXC.Qun10,
//                                         ZXC.QUN - ZXC.Qun4,           ZXC.Qun10, ZXC.QUN - ZXC.Qun4,           ZXC.Qun10,
//                                         ZXC.QUN - ZXC.Qun4,           ZXC.Qun10, ZXC.QUN - ZXC.Qun4,           ZXC.Qun10,
//                                         ZXC.QUN - ZXC.Qun4,           ZXC.Qun10, ZXC.QUN - ZXC.Qun4,           ZXC.Qun10 
//      };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvRowHgt[0] = ZXC.QUN - ZXC.Qun8;
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      hamper.CreateVvLabel(0, 0, "Odabir kolona:", 3, 0, ContentAlignment.MiddleLeft);

//      cbx_T_artiklCD    = hamper.CreateVvCheckBox_OLD(0, 1, null, "ŠifraArt", System.Windows.Forms.RightToLeft.No);
//      cbx_T_artiklName  = hamper.CreateVvCheckBox_OLD(2, 1, null, "NazivArt", System.Windows.Forms.RightToLeft.No);
//      cbx_Napomena      = hamper.CreateVvCheckBox_OLD(4, 1, null, "NapSaArt", System.Windows.Forms.RightToLeft.No);
//      cbx_BarCode1      = hamper.CreateVvCheckBox_OLD(6, 1, null, "BarKod1", System.Windows.Forms.RightToLeft.No);
//      cbx_ArtiklCD2     = hamper.CreateVvCheckBox_OLD(8, 1, null, "ŠifraArt2", System.Windows.Forms.RightToLeft.No);
//      cbx_ArtiklName2   = hamper.CreateVvCheckBox_OLD(10, 1, null, "NazivArt2", System.Windows.Forms.RightToLeft.No);
//      cbx_BarCode2      = hamper.CreateVvCheckBox_OLD(12, 1, null, "BarKod2", System.Windows.Forms.RightToLeft.No);
//      cbx_LongOpis      = hamper.CreateVvCheckBox_OLD(14, 1, null, "DugiOpis", System.Windows.Forms.RightToLeft.No);

//      cbx_SerNo         = hamper.CreateVvCheckBox_OLD(0, 2, null, "SerBroj", System.Windows.Forms.RightToLeft.No);
//      cbx_T_jedMj       = hamper.CreateVvCheckBox_OLD(2, 2, null, "JedMj", System.Windows.Forms.RightToLeft.No);
//      cbx_T_kol         = hamper.CreateVvCheckBox_OLD(4, 2, null, "Količina", System.Windows.Forms.RightToLeft.No);
//      cbx_T_cij         = hamper.CreateVvCheckBox_OLD(6, 2, null, "Cijena", System.Windows.Forms.RightToLeft.No);
//      cbx_R_KC          = hamper.CreateVvCheckBox_OLD(8, 2, null, "Kol*Cij", System.Windows.Forms.RightToLeft.No);
//      cbx_T_rbt1St      = hamper.CreateVvCheckBox_OLD(10, 2, null, "StRbt1", System.Windows.Forms.RightToLeft.No);
//      cbx_R_rbt1        = hamper.CreateVvCheckBox_OLD(12, 2, null, "IznosRbt1", System.Windows.Forms.RightToLeft.No);
//      cbx_T_rbt2St      = hamper.CreateVvCheckBox_OLD(14, 2, null, "StRbt2", System.Windows.Forms.RightToLeft.No);

//      cbx_R_rbt2        = hamper.CreateVvCheckBox_OLD(0, 3, null, "IznosRbt2", System.Windows.Forms.RightToLeft.No);
//      cbx_R_cij_KCR     = hamper.CreateVvCheckBox_OLD(2, 3, null, "CijSaRbt", System.Windows.Forms.RightToLeft.No);
//      cbx_R_KCR         = hamper.CreateVvCheckBox_OLD(4, 3, null, "IznSaRbt", System.Windows.Forms.RightToLeft.No);
//      cbx_T_mrzSt       = hamper.CreateVvCheckBox_OLD(6, 3, null, "StMrž", System.Windows.Forms.RightToLeft.No);
//      cbx_R_mrz         = hamper.CreateVvCheckBox_OLD(8, 3, null, "IznosMrž", System.Windows.Forms.RightToLeft.No);
//      cbx_R_cij_KCRM    = hamper.CreateVvCheckBox_OLD(10, 3, null, "CijSaMrž", System.Windows.Forms.RightToLeft.No);
//      cbx_R_KCRM        = hamper.CreateVvCheckBox_OLD(12, 3, null, "IznSaMrž", System.Windows.Forms.RightToLeft.No);
//      cbx_R_ztr         = hamper.CreateVvCheckBox_OLD(14, 3, null, "ZavTrošak", System.Windows.Forms.RightToLeft.No);

//      cbx_T_pdvSt       = hamper.CreateVvCheckBox_OLD( 0, 4, null, "StPdv"   , System.Windows.Forms.RightToLeft.No);
//      cbx_R_pdv         = hamper.CreateVvCheckBox_OLD( 2, 4, null, "IznosPdv", System.Windows.Forms.RightToLeft.No);
//      cbx_R_cij_KCRP    = hamper.CreateVvCheckBox_OLD( 4, 4, null, "CijSaPdv", System.Windows.Forms.RightToLeft.No);
//      cbx_R_KCRP        = hamper.CreateVvCheckBox_OLD( 6, 4, null, "IznSaPdv", System.Windows.Forms.RightToLeft.No);
//      cbx_T_doCijMal    = hamper.CreateVvCheckBox_OLD( 8, 4, null, "DoCijMal", System.Windows.Forms.RightToLeft.No);
//      cbx_T_noCijMal    = hamper.CreateVvCheckBox_OLD(10, 4, null, "NoCijMal", System.Windows.Forms.RightToLeft.No);
//      cbx_R_mjMasaN     = hamper.CreateVvCheckBox_OLD(12, 4, null, "Težina"  , System.Windows.Forms.RightToLeft.No);

//      tbx_NumDecT_kol      = hamp_Columns.CreateVvTextBox( 5, 2, "tbx_NumDecT_kol"     , "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecT_cij      = hamp_Columns.CreateVvTextBox( 7, 2, "tbx_NumDecT_cij"     , "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecR_KC       = hamp_Columns.CreateVvTextBox( 9, 2, "tbx_NumDecR_KC"      , "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecT_rbt1St   = hamp_Columns.CreateVvTextBox(11, 2, "tbx_NumDecT_rbt1St"  , "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecR_rbt1     = hamp_Columns.CreateVvTextBox(13, 2, "tbx_NumDecR_rbt1"    , "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecT_rbt2St   = hamp_Columns.CreateVvTextBox(15, 2, "tbx_NumDecT_rbt2St"  , "Željeni broj decimala za pripadajuću kolonu", 1);

//      tbx_NumDecR_rbt2     = hamp_Columns.CreateVvTextBox( 1, 3, "tbx_NumDecR_rbt2"    , "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecR_cij_KCR  = hamp_Columns.CreateVvTextBox( 3, 3, "tbx_NumDecR_cij_KCR" , "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecR_KCR      = hamp_Columns.CreateVvTextBox( 5, 3, "tbx_NumDecR_KCR"     , "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecT_mrzSt    = hamp_Columns.CreateVvTextBox( 7, 3, "tbx_NumDecT_mrzSt"   , "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecR_mrz      = hamp_Columns.CreateVvTextBox( 9, 3, "tbx_NumDecR_mrz"     , "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecR_cij_KCRM = hamp_Columns.CreateVvTextBox(11, 3, "tbx_NumDecR_cij_KCRM", "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecR_KCRM     = hamp_Columns.CreateVvTextBox(13, 3, "tbx_NumDecR_KCRM"    , "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecR_ztr      = hamp_Columns.CreateVvTextBox(15, 3, "tbx_NumDecR_ztr"     , "Željeni broj decimala za pripadajuću kolonu", 1);

//      tbx_NumDecT_pdvSt    = hamp_Columns.CreateVvTextBox( 1, 4, "tbx_NumDecT_pdvSt"   , "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecR_pdv      = hamp_Columns.CreateVvTextBox( 3, 4, "tbx_NumDecR_pdv"     , "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecR_cij_KCRP = hamp_Columns.CreateVvTextBox( 5, 4, "tbx_NumDecR_cij_KCRP", "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecR_KCRP     = hamp_Columns.CreateVvTextBox( 7, 4, "tbx_NumDecR_KCRP"    , "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecT_doCijMal = hamp_Columns.CreateVvTextBox( 9, 4, "tbx_NumDecT_doCijMal", "Željeni broj decimala za pripadajuću kolonu", 1);
//      tbx_NumDecT_noCijMal = hamp_Columns.CreateVvTextBox(11, 4, "tbx_NumDecT_noCijMal", "Željeni broj decimala za pripadajuću kolonu", 1);

//      tbx_NumDecT_kol      .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecT_cij      .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecR_KC       .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecT_rbt1St   .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecR_rbt1     .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecT_rbt2St   .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecR_rbt2     .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecR_cij_KCR  .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecR_KCR      .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecT_mrzSt    .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecR_mrz      .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecR_cij_KCRM .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecR_KCRM     .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecR_ztr      .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecT_pdvSt    .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecR_pdv      .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecR_cij_KCRP .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecR_KCRP     .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecT_doCijMal .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);
//      tbx_NumDecT_noCijMal .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);

//      cbx_R_mjMasaN        .Visible =
//      cbx_T_rbt1St         .Visible =
//      cbx_R_rbt1           .Visible =
//      cbx_T_rbt2St         .Visible =
//      cbx_R_rbt2           .Visible =
//      cbx_R_cij_KCR        .Visible =
//      cbx_R_KCR            .Visible =
//      cbx_T_mrzSt          .Visible =
//      cbx_R_mrz            .Visible =
//      cbx_R_cij_KCRM       .Visible =
//      cbx_R_KCRM           .Visible =
//      cbx_R_ztr            .Visible =
//      cbx_T_pdvSt          .Visible =
//      cbx_R_pdv            .Visible =
//      cbx_R_cij_KCRP       .Visible =
//      cbx_R_KCRP           .Visible =
//      cbx_T_doCijMal       .Visible =
//      cbx_T_noCijMal       .Visible =
//      tbx_NumDecT_rbt1St   .Visible =
//      tbx_NumDecR_rbt1     .Visible =
//      tbx_NumDecT_rbt2St   .Visible =
//      tbx_NumDecR_rbt2     .Visible =
//      tbx_NumDecR_cij_KCR  .Visible =
//      tbx_NumDecR_KCR      .Visible =
//      tbx_NumDecT_mrzSt    .Visible =
//      tbx_NumDecR_mrz      .Visible =
//      tbx_NumDecR_cij_KCRM .Visible =
//      tbx_NumDecR_KCRM     .Visible =
//      tbx_NumDecR_ztr      .Visible =
//      tbx_NumDecT_pdvSt    .Visible =
//      tbx_NumDecR_pdv      .Visible =
//      tbx_NumDecR_cij_KCRP .Visible =
//      tbx_NumDecR_KCRP     .Visible =
//      tbx_NumDecT_doCijMal .Visible =
//      tbx_NumDecT_noCijMal .Visible = isFakExt;

//      VvHamper.HamperStyling(hamper);
//      SetUpAsWriteOnlyTbx(hamper);
//   }

//   private void CreateHamperAdres(out VvHamper hamper)
//   {
//      hamper = new VvHamper(3, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt      = new int[] { ZXC.Q3un - ZXC.Qun4, ZXC.Q3un- ZXC.Qun8, ZXC.Q3un- ZXC.Qun8 };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//                       hamper.CreateVvLabel      (0, 0, "Adresa:", ContentAlignment.MiddleRight);
//      rbt_AdresLeft  = hamper.CreateVvRadioButton(1, 0, null, "Lijevo", TextImageRelation.ImageBeforeText);
//      rbt_AdresRight = hamper.CreateVvRadioButton(2, 0, null, "Desno", TextImageRelation.ImageBeforeText);
//      rbt_AdresRight.Checked = true;
//      rbt_AdresRight.Tag = true;

//      VvHamper.HamperStyling(hamper);

//   }

//   private void CreateHamperPoslJed(out VvHamper hamper)
//   {
//      hamper = new VvHamper(2, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt      = new int[] { ZXC.Q4un - ZXC.Qun2, ZXC.Q5un };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      hamper.CreateVvLabel(0, 0, "NazivPoslJed:", ContentAlignment.MiddleRight);
//      tbx_nazivPoslJed = hamper.CreateVvTextBox(1, 0, "tbx_PoslJed", "Labela za ispis Poslovne jedinice, npr. Poslovna jed:, Isporučeno: ...");

//      VvHamper.HamperStyling(hamper);
//      SetUpAsWriteOnlyTbx(hamper);
//   }

//   private void CreateHamperOrient(out VvHamper hamper)
//   {
//      hamper = new VvHamper(3, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt      = new int[] { ZXC.Q2un, ZXC.Q2un + ZXC.Qun4, ZXC.Q2un + ZXC.Qun4 };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//                      hamper.CreateVvLabel(0, 0, "Ispis:", ContentAlignment.MiddleRight);
//      rbt_RptPort   = hamper.CreateVvRadioButton(1, 0, null, "Port", TextImageRelation.ImageBeforeText);
//      rbt_RptLandsc = hamper.CreateVvRadioButton(2, 0, null, "Land", TextImageRelation.ImageBeforeText);
//      rbt_RptPort.Checked = true;
//      rbt_RptPort.Tag     = true;
//      VvHamper.HamperStyling(hamper);

//   }

//   private void CreateHamperPersoni(out VvHamper hamper)
//   {
//      hamper = new VvHamper(9, 2, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt    = new int[] { ZXC.Q5un - ZXC.Qun4, ZXC.Q5un - ZXC.Qun4,ZXC.Q5un - ZXC.Qun4, ZXC.Q5un - ZXC.Qun4,ZXC.Q5un - ZXC.Qun4, ZXC.Q5un - ZXC.Qun4, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
//      hamper.VvSpcBefCol = new int[] {            ZXC.Qun4,            ZXC.Qun8,           ZXC.Qun8,            ZXC.Qun8,           ZXC.Qun8,            ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };

//      //for(int i = 0; i < hamper.VvNumOfCols; i++)
//      //{
//      //   hamper.VvColWdt[i] = ZXC.Q5un - ZXC.Qun4;
//      //   hamper.VvSpcBefCol[i] = ZXC.Qun8;
//      //}
//      //hamper.VvSpcBefCol[0] = ZXC.Qun2;
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      hamper.VvRowHgt = new int[] { ZXC.QUN - ZXC.Qun4, ZXC.QUN };

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
////         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun4;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      cbx_signPrimaoc = hamper.CreateVvCheckBox_OLD(0, 0, null, "Primio", System.Windows.Forms.RightToLeft.No);
//      tbx_Primio = hamper.CreateVvTextBox(0, 1, "tbx_Primio", "Labela za  npr. Robu preuzeo:, Primio:, ...");

//      hamper.CreateVvLabel(1, 0, "Odgovorna osoba:", ContentAlignment.MiddleLeft);
//      hamper.CreateVvLabel(2, 0, "Ime User-a:", ContentAlignment.MiddleLeft);
//      hamper.CreateVvLabel(3, 0, "Osoba A:", ContentAlignment.MiddleLeft);
//      hamper.CreateVvLabel(4, 0, "Osoba B:", ContentAlignment.MiddleLeft);
//      hamper.CreateVvLabel(5, 0, "Osoba X:", ContentAlignment.MiddleLeft);

//      tbx_lblPrjktPerson = hamper.CreateVvTextBox(1, 1, "tbx_lblPrjktPerson", "Labela za odgovornu osobu navedena na projektu ,  npr. Direktor:, Odgovorna osoba:  ...");
//      tbx_lblUserPerson  = hamper.CreateVvTextBox(2, 1, "tbx_lblUserPerson" , "Labela za usera logiranog u program ,  npr. Dokument sastavio:, Fakturirao:, Sastavio: ...");
//      tbx_lblOsobaA      = hamper.CreateVvTextBox(3, 1, "tbx_lblOsobaA"     , "Labela za osobuA,  npr. Dokument sastavio:, Fakturirao:, Sastavio: ...");
//      tbx_lblOsobaB      = hamper.CreateVvTextBox(4, 1, "tbx_lblOsobaB"     , "Labela za osobuB ,  npr. Dokument sastavio:, Fakturirao:, Sastavio: ...");
//      tbx_LblOsobaX      = hamper.CreateVvTextBox(5, 1, "tbx_LblOsobaX"     , "Labela za osobuX ,  npr. Dokument sastavio:, Fakturirao:, Sastavio: ...");

//      cbx_ocuLinijePerson = hamper.CreateVvCheckBox_OLD(6, 0, null, "IspisCrta", System.Windows.Forms.RightToLeft.No);
//      cbx_ocuFirmuUpotpis = hamper.CreateVvCheckBox_OLD(7, 0, null, 1, 0, "NazivPoduzeća" , System.Windows.Forms.RightToLeft.No);

//                         hamper.CreateVvLabel      (6, 1, "Poravnanje", ContentAlignment.MiddleLeft);
//      rbt_personRight  = hamper.CreateVvRadioButton(7, 1, null, "Desno"     , TextImageRelation.ImageBeforeText);
//      rbt_personCentar = hamper.CreateVvRadioButton(8, 1, null, "Centar", TextImageRelation.ImageBeforeText);
//      rbt_personRight.Checked = true;
//      rbt_personRight.Tag     = true;
         
//      VvHamper.HamperStyling(hamper);
//      SetUpAsWriteOnlyTbx(hamper);
//   }

//   private void CreateHamperBelowGrid(out VvHamper hamper)
//   {
//      hamper = new VvHamper(1, 8, "", this, false, nextX, nextY, 0);

//      hamper.VvColWdt      = new int[] { ZXC.Q10un*3 + ZXC.Q9un };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      hamper.VvRowHgt       = new int[] { ZXC.QUN,  ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN };
//      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, 0, ZXC.Qun8, 0, 0, 0, 0, 0 };
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      lbl_textAbovGr = hamper.CreateVvLabel  (0, 0, "Tekst iznad tablice:", ContentAlignment.MiddleLeft);
//      tbx_AboveGrid  = hamper.CreateVvTextBox(0, 1, "tbx_AboveGrid", "Tekst koji se vidi iznad tablice", 2048);
//      tbx_AboveGrid.Multiline = true;
//      tbx_AboveGrid.ScrollBars = ScrollBars.Both;
         
//         hamper.CreateVvLabel  (0, 2, "Tekst ispod tablice:", ContentAlignment.MiddleLeft);
//      tbx_belowGrid = hamper.CreateVvTextBox(0, 3, "tbx_belowGrid", "Tekst koji se vidi ispod tablice", 2048, 0, 4);
//      tbx_belowGrid.Multiline = true;
//      tbx_belowGrid.ScrollBars = ScrollBars.Both;
//      //tbx_belowGrid.ScrollBars = ScrollBars.Horizontal;
//      SetUpAsWriteOnlyTbx(hamper);

//      if(isFakExt == false)
//      {
//         lbl_textAbovGr.Visible =
//         tbx_AboveGrid .Visible = false;
//      }

//      VvHamper.HamperStyling(hamper);

//   }

//   private void CreateHampOstalo(out VvHamper hamper)
//   {
//      hamper = new VvHamper(7, 2, "", this, false, nextX, nextY, razmakHampGroup);

//      for(int i = 0; i < hamper.VvNumOfCols; i++)
//      {
//         hamper.VvColWdt[i] = ZXC.Q5un - ZXC.Qun4;
//         hamper.VvSpcBefCol[i] = ZXC.Qun8;
//      }
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      hamper.VvRowHgt = new int[] { ZXC.QUN-ZXC.Qun4, ZXC.QUN};

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//    //     hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;


//      hamper.CreateVvLabel(0, 0, "LabelaOpćiA", ContentAlignment.MiddleLeft);
//      tbx_opciA = hamper.CreateVvTextBox(0, 1, "tbx_opciA", "Labela za Opći A");

//      hamper.CreateVvLabel(1, 0, "LabelaOpćiB", ContentAlignment.MiddleLeft);
//      tbx_opciB = hamper.CreateVvTextBox(1, 1, "tbx_opciB", "Labela za Opći A");

//      cbx_OcuIspisVezDok2 = hamper.CreateVvCheckBox_OLD(2, 0, null, "VezDok2", System.Windows.Forms.RightToLeft.No);
//      cbx_OcuIspisVeze1 = hamper.CreateVvCheckBox_OLD(3, 0, null, "Veza1", System.Windows.Forms.RightToLeft.No);
//      cbx_OcuIspisVeze2 = hamper.CreateVvCheckBox_OLD(4, 0, null, "Veza2", System.Windows.Forms.RightToLeft.No);
//      cbx_OcuIspisVeze3 = hamper.CreateVvCheckBox_OLD(5, 0, null, "Veza3", System.Windows.Forms.RightToLeft.No);
//      cbx_OcuIspisVeze4 = hamper.CreateVvCheckBox_OLD(6, 0, null, "Veza4", System.Windows.Forms.RightToLeft.No);

//      tbx_LblVezDok2 = hamper.CreateVvTextBox(2, 1, "tbx_LblVezDok2", "Labela za VezDok2");
//      tbx_LblVeze1 = hamper.CreateVvTextBox(3, 1, "tbx_LblVeze1", "Labela za Veza1  ");
//      tbx_LblVeze2 = hamper.CreateVvTextBox(4, 1, "tbx_LblVeze2", "Labela za Veza2  ");
//      tbx_LblVeze3 = hamper.CreateVvTextBox(5, 1, "tbx_LblVeze3", "Labela za Veza3  ");
//      tbx_LblVeze4 = hamper.CreateVvTextBox(6, 1, "tbx_LblVeze4", "Labela za Veza4  ");

//      VvHamper.HamperStyling(hamper);
//      SetUpAsWriteOnlyTbx(hamper);
//   }

//   private void CreateHamperLogo(out VvHamper hamper)
//   {
//      hamper = new VvHamper(2, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un - ZXC.Qun2 + ZXC.Qun10};
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      hamper.CreateVvLabel(0, 0, "SkaliranjeLoga", ContentAlignment.MiddleRight);
//      tbx_scalingLogo = hamper.CreateVvTextBox(1, 0, "tbx_scalingLogo", "Postotak skaliranja logo slike za print", 7);
//      tbx_scalingLogo.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, decimal.MinValue, false);

//      tbx_scalingLogo.JAM_IsForPercent = true;

//      SetUpAsWriteOnlyTbx(hamper);

//      VvHamper.HamperStyling(hamper);
//   }

//   private void CreateHamper_memo(out VvHamper hamper)
//   {
//      hamper = new VvHamper(7, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt = new int[] { ZXC.Q4un, ZXC.Q4un + ZXC.Qun4, ZXC.Q3un, ZXC.Q3un, ZXC.Q4un + ZXC.Qun4, ZXC.Q3un, ZXC.Q3un };

//      for(int i = 0; i < hamper.VvNumOfCols; i++)
//      {
//    //     hamper.VvColWdt[i]    = ZXC.Q4un;
//         hamper.VvSpcBefCol[i] = ZXC.Qun8;
//      }
//      hamper.VvRightMargin = hamper.VvLeftMargin;
//    //  hamper.VvSpcBefCol[0] = ZXC.Qun2;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      cbx_ocuLogo          = hamper.CreateVvCheckBox_OLD(0, 0, null, "VidljivLogo"    , RightToLeft.No);
//      cbx_ocuHeader        = hamper.CreateVvCheckBox_OLD(1, 0, null, "VidljivHeader"  , RightToLeft.No);
//      cbx_OcuMemoHAllPages = hamper.CreateVvCheckBox_OLD(2, 0, null, "HeSveStr"       , RightToLeft.No); //vidljiv memoHeader na svim stranicama
//      cbx_OcuLinijeHeader  = hamper.CreateVvCheckBox_OLD(3, 0, null, "HeCrte"         , RightToLeft.No);
//      cbx_ocuFooter        = hamper.CreateVvCheckBox_OLD(4, 0, null, "VidljivFooter"  , RightToLeft.No);
//      cbx_OcuMemoFAllPages = hamper.CreateVvCheckBox_OLD(5, 0, null, "FoSveStr"       , RightToLeft.No); //vidljiv memoFooter na svim stranicama
//      cbx_OcuLinijeFooter  = hamper.CreateVvCheckBox_OLD(6, 0, null, "FoCrte"         , RightToLeft.No);
      
//      VvHamper.HamperStyling(hamper);

//   }

//   private void CreateHamperValuta(out VvHamper hamper)
//   {
//      hamper = new VvHamper(2, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt      = new int[] { ZXC.Q3un + ZXC.Qun2, ZXC.Q3un - ZXC.Qun2 };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//                    hamper.CreateVvLabel(0, 0, "IznosUValuti:", ContentAlignment.MiddleRight);
//      tbx_ValName = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_ValName", "Naziv devizne valute u kojoj će se prikazati dokument koji se printa");
//      tbx_ValName.JAM_CharacterCasing = CharacterCasing.Upper;
//      tbx_ValName.JAM_Set_LookUpTable(ZXC.luiListaDeviza, (int)ZXC.Kolona.prva);


//      SetUpAsWriteOnlyTbx(hamper);
//      VvHamper.HamperStyling(hamper);

//   }

//   private void CreateHamperFontOpis(out VvHamper hamper)
//   {
//      hamper = new VvHamper(5, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun2 + ZXC.Qun4};
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8,           ZXC.Qun8,           ZXC.Qun8,            ZXC.Qun8,                     ZXC.Qun8};
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      hamper.CreateVvLabel(0, 0, "FontDokumenta:", ContentAlignment.MiddleRight);
//      rbt_opis7 = hamper.CreateVvRadioButton(1, 0, null, "7", TextImageRelation.ImageBeforeText);
//      rbt_opis8 = hamper.CreateVvRadioButton(2, 0, null, "8", TextImageRelation.ImageBeforeText);
//      rbt_opis9 = hamper.CreateVvRadioButton(3, 0, null, "9", TextImageRelation.ImageBeforeText);
//      rbt_opis10 = hamper.CreateVvRadioButton(4, 0, null, "10", TextImageRelation.ImageBeforeText);
//      rbt_opis10.Checked = true;
//      rbt_opis10.Tag = true;

//      VvHamper.HamperStyling(hamper);
//   }
  
//   private void CreateHamperFontCol(out VvHamper hamper)
//   {
//      hamper = new VvHamper(5, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt      = new int[] { ZXC.Q3un + ZXC.Qun2, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun2 + ZXC.Qun4 };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      hamper.CreateVvLabel(0, 0, "FontKolona:", ContentAlignment.MiddleRight);
//      rbt_col7 = hamper.CreateVvRadioButton(1, 0, null, "7", TextImageRelation.ImageBeforeText);
//      rbt_col8 = hamper.CreateVvRadioButton(2, 0, null, "8", TextImageRelation.ImageBeforeText);
//      rbt_col9 = hamper.CreateVvRadioButton(3, 0, null, "9", TextImageRelation.ImageBeforeText);
//      rbt_col10 = hamper.CreateVvRadioButton(4, 0, null, "10", TextImageRelation.ImageBeforeText);
//      rbt_col10.Checked = true;
//      rbt_col10.Tag = true;

//      VvHamper.HamperStyling(hamper);

//   }
  
//   private void CreateHamperFontBelGr(out VvHamper hamper)
//   {
//      hamper = new VvHamper(5, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt      = new int[] { ZXC.Q3un + ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun2 + ZXC.Qun4 };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      hamper.CreateVvLabel(0, 0, "FontIspodTbl:", ContentAlignment.MiddleRight);
//      rbt_belGr7 = hamper.CreateVvRadioButton(1, 0, null, "7", TextImageRelation.ImageBeforeText);
//      rbt_belGr8 = hamper.CreateVvRadioButton(2, 0, null, "8", TextImageRelation.ImageBeforeText);
//      rbt_belGr9 = hamper.CreateVvRadioButton(3, 0, null, "9", TextImageRelation.ImageBeforeText);
//      rbt_belGr10 = hamper.CreateVvRadioButton(4, 0, null, "10", TextImageRelation.ImageBeforeText);
//      rbt_belGr10.Checked = true;
//      rbt_belGr10.Tag = true;

//      VvHamper.HamperStyling(hamper);

//   }

//   private void CreateHamperJezik(out VvHamper hamper)
//   {
//      //hamper = new VvHamper(2, 5, "", this, false, nextX, nextY, razmakHampGroup);
//      hamper = new VvHamper(2, 5, "", this, false);

//      hamper.VvColWdt      = new int[] { ZXC.Q2un + ZXC.Qun8, ZXC.QUN - ZXC.Qun8 };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun12 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      hamper.CreateVvLabel(0, 0, "Izbor jezika", 1, 0, ContentAlignment.MiddleLeft);
//      rbt_hrv = hamper.CreateVvRadioButton(0, 1, null, "HRV", TextImageRelation.ImageBeforeText);
//      rbt_eng = hamper.CreateVvRadioButton(0, 2, null, "ENG", TextImageRelation.ImageBeforeText);
//      rbt_njem = hamper.CreateVvRadioButton(0, 3, null, "C", TextImageRelation.ImageBeforeText);
//      rbt_tal = hamper.CreateVvRadioButton(0, 4, null, "D", TextImageRelation.ImageBeforeText);
//      rbt_hrv.Checked = true;
//      rbt_hrv.Tag = true;

//      tbx_hrv = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_hrv", "Unos za hrv ");
//      tbx_eng = hamper.CreateVvTextBoxLookUp(1, 2, "tbx_eng", "Unos za eng ");
//      tbx_njem = hamper.CreateVvTextBoxLookUp(1, 3, "tbx_njem", "Unos za njem");
//      tbx_tal = hamper.CreateVvTextBoxLookUp(1, 4, "tbx_tal", "Unos za tal ");

//      tbx_hrv.JAM_Set_LookUpTable(ZXC.luiListaRiskJezikHrv, (int)ZXC.Kolona.prva);
//      tbx_eng.JAM_Set_LookUpTable(ZXC.luiListaRiskJezikEng, (int)ZXC.Kolona.prva);
//      tbx_njem.JAM_Set_LookUpTable(ZXC.luiListaRiskJezikC, (int)ZXC.Kolona.prva);
//      tbx_tal.JAM_Set_LookUpTable(ZXC.luiListaRiskJezikD, (int)ZXC.Kolona.prva);

//      SetUpAsWriteOnlyTbx(hamper);

//      VvHamper.HamperStyling(hamper);

//   }

//   private void CreateHampBlg(out VvHamper hamper)
//   {
//      hamper = new VvHamper(8, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      for(int i = 0; i < hamper.VvNumOfCols; i++)
//      {
//         hamper.VvColWdt[i] = ZXC.Q4un - ZXC.Qun4 + ZXC.QUN - ZXC.Qun4;
//         hamper.VvSpcBefCol[i] = ZXC.QUN - ZXC.Qun4 + ZXC.Qun10;
//      }
//      hamper.VvRightMargin = hamper.VvLeftMargin;
//      hamper.VvSpcBefCol[0] = ZXC.Qun2;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      cbx_blgOcuColKonto = hamper.CreateVvCheckBox_OLD(0, 0, null, "KolonaKonto", RightToLeft.No);
//      cbx_blgOcuColRacun = hamper.CreateVvCheckBox_OLD(1, 0, null, "KolonaRačun", RightToLeft.No);
//      cbx_blgOcu2na1strani = hamper.CreateVvCheckBox_OLD(2, 0, null, "2na1Strani", RightToLeft.No);
//      cbx_blgOcuOkvirUplsp = hamper.CreateVvCheckBox_OLD(3, 0, null, "Okvir Isp/Upl", RightToLeft.No);


//      VvHamper.HamperStyling(hamper);
//   }

//   private void CreateHamperNRD(out VvHamper hamper)
//   {
//      hamper = new VvHamper(2, 2, "", this, false, nextX, nextY, 0);

//      hamper.VvColWdt      = new int[] { ZXC.Q4un,   ZXC.Q10un*3+ZXC.Q6un};
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8,           ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      hamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN};
//      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      cbx_PotvrdaNarudzbe = hamper.CreateVvCheckBox_OLD(0, 0, null, "PotvrdaNarudžbe", RightToLeft.No);

//      tbx_beforNRD = hamper.CreateVvTextBox(1, 0, "tbx_beforNRD", "Tekst koji se vidi iznad potpisa", 2048);
//      tbx_beforNRD.Multiline = true;
//      tbx_beforNRD.ScrollBars = ScrollBars.Both;

//                     hamper.CreateVvLabel  (0, 1, "TekstNapomena:", ContentAlignment.MiddleLeft);
//      tbx_afterNRD = hamper.CreateVvTextBox(1, 1, "tbx_afterNRD", "Tekst koji se vidi ispod potpisa", 2048);
//      tbx_afterNRD.Multiline = true;
//      tbx_afterNRD.ScrollBars = ScrollBars.Both;

//      SetUpAsWriteOnlyTbx(hamper);

//      VvHamper.HamperStyling(hamper);

//   }

//   private void CreateHamperLeftMargin(out VvHamper hamper)
//   {
//      hamper = new VvHamper(6, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un- ZXC.Qun2, ZXC.Q3un- ZXC.Qun2, ZXC.Q3un- ZXC.Qun2, ZXC.Q3un- ZXC.Qun2};
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8};
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i]    = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//                        hamper.CreateVvLabel      (0, 0, "Lijeva margina:", ContentAlignment.MiddleRight);
//      rbt_leftMargin1 = hamper.CreateVvRadioButton(1, 0, null, "1.0cm", TextImageRelation.ImageBeforeText);
//      rbt_leftMargin2 = hamper.CreateVvRadioButton(2, 0, null, "1.5cm", TextImageRelation.ImageBeforeText);
//      rbt_leftMargin3 = hamper.CreateVvRadioButton(3, 0, null, "2.0cm", TextImageRelation.ImageBeforeText);
//      rbt_leftMargin4 = hamper.CreateVvRadioButton(4, 0, null, "2.5cm", TextImageRelation.ImageBeforeText);
//      rbt_leftMargin5 = hamper.CreateVvRadioButton(5, 0, null, "3.0cm", TextImageRelation.ImageBeforeText);
//      rbt_leftMargin1.Checked = true;
//      rbt_leftMargin1.Tag     = true;

//      VvHamper.HamperStyling(hamper);
//   }

//   private void CreateHamperRightMargin(out VvHamper hamper)
//   {
//      hamper = new VvHamper(5, 1, "", this, false, nextX, nextY, razmakHampGroup);

//      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un- ZXC.Qun2, ZXC.Q3un- ZXC.Qun2, ZXC.Q3un- ZXC.Qun2};
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8};
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i]    = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//                         hamper.CreateVvLabel      (0, 0, "Desna margina:", ContentAlignment.MiddleRight);
//      rbt_rightMargin1 = hamper.CreateVvRadioButton(1, 0, null, "1.0cm", TextImageRelation.ImageBeforeText);
//      rbt_rightMargin2 = hamper.CreateVvRadioButton(2, 0, null, "1.5cm", TextImageRelation.ImageBeforeText);
//      rbt_rightMargin3 = hamper.CreateVvRadioButton(3, 0, null, "2.0cm", TextImageRelation.ImageBeforeText);
//      rbt_rightMargin4 = hamper.CreateVvRadioButton(4, 0, null, "2.5cm", TextImageRelation.ImageBeforeText);
//      rbt_rightMargin1.Checked = true;
//      rbt_rightMargin1.Tag     = true;

//      VvHamper.HamperStyling(hamper);
//   }

//   //--------------------------------------------------------------------------------------------------------------------------------------
//   private void RecalculateHamper4buttons()
//   {
//      btn_Reset.Visible = false;
//      hamper4buttons.BorderStyle = BorderStyle.None;
//   }

//   private void CalcLocationHampersOnFilter(bool isMinFilter)
//   {
//      ZXC.ZaUpis zaUpisOtvZatv;

//      if(isMinFilter)
//      {
//         this.Size = new Size(minFilterWidth, TheVvUC.Height);
//         ((VvRecordUC)TheVvUC).ThePanelForFilterUC_PrintTemplateUC.Width = minFilterWidth;

//         hamper4buttons .Location = new Point(ZXC.Qun2 - ZXC.Qun4, 0);
//         hamp_ChoseIra  .Location = new Point(ZXC.Qun2 - ZXC.Qun4 + ZXC.Qun8, hamper4buttons.Bottom);
//         hamp_OpenFilter.Location = new Point(ZXC.Qun2 - ZXC.Qun2, hamp_ChoseIra.Bottom + razmakIzmjedjuHampera);

//         tbx_obrazacA.JAM_ReadOnly = true;
//         tbx_obrazacB.JAM_ReadOnly = true;
//         tbx_obrazacC.JAM_ReadOnly = true;
//         tbx_obrazacD.JAM_ReadOnly = true;

//         btn_OpenFilter.Text = "Postavke +";

//         zaUpisOtvZatv = ZXC.ZaUpis.Zatvoreno;

//      }
//      else
//      {
//         this.Size = new Size(maxFilterWidth + ZXC.QUN, maxFilterHeight);
//         ((VvRecordUC)TheVvUC).ThePanelForFilterUC_PrintTemplateUC.Width = maxFilterWidth + ZXC.QUN;

//         hamper4buttons .Location = new Point(maxFilterWidth - hamp_ChoseIra.Width - razmakIzmjedjuHampera, 0);
//         hamp_ChoseIra  .Location = new Point(maxFilterWidth - hamp_ChoseIra.Width - razmakIzmjedjuHampera + ZXC.Qun10, hamper4buttons.Bottom);
//         hamp_OpenFilter.Location = new Point(maxFilterWidth - hamp_ChoseIra.Width - razmakIzmjedjuHampera - ZXC.Qun4, hamp_ChoseIra.Bottom + razmakIzmjedjuHampera);

//         btn_Reset.Parent = this;
//         btn_Reset.Location = new Point(hamp_OpenFilter.Left + ZXC.Qun4, hamp_OpenFilter.Bottom + razmakIzmjedjuHampera);

//         btn_OpenFilter.Text = "Postavke -";

//         tbx_obrazacA.JAM_ReadOnly = false;
//         tbx_obrazacB.JAM_ReadOnly = false;
//         tbx_obrazacC.JAM_ReadOnly = false;
//         tbx_obrazacD.JAM_ReadOnly = false;

//         zaUpisOtvZatv = ZXC.ZaUpis.Otvoreno;
//      }

//      btn_Reset.Visible = !isMinFilter;
//      VvHamper.Open_Close_Fields_ForWriting(tbx_obrazacA, zaUpisOtvZatv, ZXC.ParentControlKind.VvReportUC);
//      VvHamper.Open_Close_Fields_ForWriting(tbx_obrazacB, zaUpisOtvZatv, ZXC.ParentControlKind.VvReportUC);
//      VvHamper.Open_Close_Fields_ForWriting(tbx_obrazacC, zaUpisOtvZatv, ZXC.ParentControlKind.VvReportUC);
//      VvHamper.Open_Close_Fields_ForWriting(tbx_obrazacD, zaUpisOtvZatv, ZXC.ParentControlKind.VvReportUC);

//   }

//   private void CreateHamperOpenFilter(out VvHamper hamper)
//   {
//      hamper = new VvHamper(1, 1, "", this, false);
      
//      hamper.VvColWdt      = new int[] { ZXC.QunBtnW };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i] = ZXC.QunBtnH;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;

//      //if(ZXC.CURR_userName != ZXC.vvDB_systemSuperUserName &&
//      //   ZXC.CURR_userName != ZXC.vvDB_programSuperUserName && !ZXC.CURR_user_rec.IsSuper)
//      {
//         btn_OpenFilter = hamper.CreateVvButton(0, 0, new EventHandler(ButtonOpenFilter_Click), "Postavke +");
//      }
//   }

//   public void ButtonOpenFilter_Click(object sender, EventArgs e)
//   {
//      if(((VvRecordUC)TheVvUC).ThePanelForFilterUC_PrintTemplateUC.Width == maxFilterWidth + ZXC.QUN)
//      {
//         CalcLocationHampersOnFilter(true);
//         SetVisibilitiOfHampers(false);
//      }
//      else
//      {
//         CalcLocationHampersOnFilter(false);
//         SetVisibilitiOfHampers(true);
//      }
//   }

//   private void SetVisibilitiOfHampers(bool isVisible)
//   {
//      hamp_dokNumLCR     .Visible =
//      hamp_migHF         .Visible =
//      hamp_NRD           .Visible =
//      hamp_title         .Visible =
//      hamp_dokNumHFadres .Visible =
//      hamp_pageLCR       .Visible =
//      hamp_pageHF        .Visible =
//      hamp_adresSastojci .Visible =
//      hamp_adresToFrom   .Visible =
//      hamp_adresOkvir    .Visible =
//      hamp_Columns       .Visible =
//      hamp_personi       .Visible =
//      hamp_adresLR       .Visible =
//      hamp_orient        .Visible =
//      hamp_RnPnb         .Visible =
//      hamp_belowGrid     .Visible =
//      hamp_Cbx           .Visible =
//      hamp_ostalo        .Visible =
//      hamp_poslJed       .Visible =
//      hamp_logo          .Visible =
//      hamp_memo          .Visible =
//      hamp_FontBelGr     .Visible =
//      hamp_FontOpis      .Visible =
//      hamp_FontCol       .Visible =
//      hamp_jezik         .Visible =
//      hamp_valuta        .Visible = isVisible;

//      hamp_Blg.Visible = false;

//      if(TheVvUC is BlgIsplatDUC || TheVvUC is BlgUplatDUC)
//      {
//         hamp_dokNumLCR     .Visible =
//         hamp_migHF         .Visible =
//         hamp_NRD           .Visible =
//         hamp_title         .Visible =
//         hamp_dokNumHFadres .Visible =
//         hamp_pageLCR       .Visible =
//         hamp_pageHF        .Visible =
//         hamp_adresSastojci .Visible =
//         hamp_adresToFrom   .Visible =
//         hamp_adresOkvir    .Visible =
//         hamp_Columns       .Visible =
//         hamp_personi       .Visible =
//         hamp_adresLR       .Visible =
//         hamp_orient        .Visible =
//         hamp_RnPnb         .Visible =
//         hamp_belowGrid     .Visible =
//         hamp_ostalo        .Visible =
//         hamp_poslJed       .Visible =
//         hamp_FontBelGr     .Visible =
//         hamp_FontOpis      .Visible =
//         hamp_FontCol       .Visible =
//         hamp_jezik         .Visible =
//         hamp_valuta        .Visible = false;
//         hamp_Blg           .Visible = isVisible;
//      }
//   //            hamp_dokNumLCR.Visible =
//   //                  hamp_title.Visible =
//   //hamp_pageLCR.Visible =
//   //hamp_pageHF.Visible =

//      if(isFakExt == false)
//      {
//         hamp_dokNumHFadres.Visible =
//         hamp_migHF        .Visible =
//         hamp_NRD          .Visible =
//         hamp_adresSastojci.Visible =
//         hamp_adresToFrom  .Visible =
//         hamp_adresOkvir   .Visible =
//         hamp_adresLR      .Visible =
//         hamp_ostalo       .Visible =
//         hamp_poslJed      .Visible =
//         hamp_jezik        .Visible =
//         hamp_valuta       .Visible = false;

//      }
//   }

//   #endregion Hampers

//   #region Fld_

//   public bool   Fld_NeedsHorizontalLine { get { return cb_Line.Checked; }  set {        cb_Line.Checked = value; } }

//   public string Fld_AdresaLeftRight   
//   {
//      get
//      {
//         if     (rbt_AdresLeft.Checked)  return "L";
//         else if(rbt_AdresRight.Checked) return "R";

//         else throw new Exception("Fld_AdresaLeftRight: who df is checked?");
//      }
//      set
//      {
//         if     (value.ToString() == "L") rbt_AdresLeft.Checked = true;
//         else if(value.ToString() == "R") rbt_AdresRight.Checked = true;
      
//      }
//   }
//   public string Fld_RptOrientation    
//   {
//      get
//      {
//         if     (rbt_RptLandsc.Checked)  return "L";
//         else if(rbt_RptPort  .Checked)  return "P";

//         else throw new Exception("Fld_RptOrientation: who df is checked?");
//      }
      
//      set
//      {
//         if     (value.ToString() == "L") rbt_RptLandsc.Checked = true;
//         else if(value.ToString() == "P") rbt_RptPort  .Checked = true;
      
//      }

//   }

//   public string Fld_LblPrjktPerson    { get { return tbx_lblPrjktPerson .Text;    }  set {        tbx_lblPrjktPerson .Text    = value; } }
//   public string Fld_LblUserPerson     { get { return tbx_lblUserPerson  .Text;    }  set {        tbx_lblUserPerson  .Text    = value; } }
//   public string Fld_LblOsobaA         { get { return tbx_lblOsobaA      .Text;    }  set {        tbx_lblOsobaA      .Text    = value; } }
//   public string Fld_LblOsobaB         { get { return tbx_lblOsobaB      .Text;    }  set {        tbx_lblOsobaB      .Text    = value; } }
//   public bool   Fld_VertikalLine      { get { return cbx_verLine        .Checked; }  set {        cbx_verLine        .Checked = value; } }
//   public bool   Fld_TableBorder       { get { return cbx_TableBorder    .Checked; }  set {        cbx_TableBorder    .Checked = value; } }
//   public bool   Fld_SignPrimaoc       { get { return cbx_signPrimaoc    .Checked; }  set {        cbx_signPrimaoc    .Checked = value; } }
//   public bool   Fld_TekstualIznos     { get { return cbx_tekstualIznos  .Checked; }  set {        cbx_tekstualIznos  .Checked = value; } }
//   public bool   Fld_IsAddTtNum        { get { return cbx_IsAddTtNumRn   .Checked; }  set {        cbx_IsAddTtNumRn   .Checked = value; } }
//   public bool   Fld_IsAddYear         { get { return cbx_IsAddYearRn    .Checked; }  set {        cbx_IsAddYearRn    .Checked = value; } }
//   public bool   Fld_IsAddKupDobCd     { get { return cbx_IsAddKupDobCdRn.Checked; }  set {        cbx_IsAddKupDobCdRn.Checked = value; } }
//   public string Fld_DevName           { get { return tbx_ValName        .Text;    }  set {        tbx_ValName        .Text    = value; } }
   
//   public ZXC.ValutaNameEnum Fld_DevNameAsEnum
//   {
//      get
//      {
//         if(tbx_ValName.Text.IsEmpty()) return ZXC.ValutaNameEnum.EMPTY;
//         else                           return (ZXC.ValutaNameEnum)Enum.Parse(typeof(ZXC.ValutaNameEnum), tbx_ValName.Text, true);
//      }
//   }
   
//   public string Fld_NazivPoslJed       
//   {
//      get { return tbx_nazivPoslJed.Text; }
//      set {        tbx_nazivPoslJed.Text = value; }
//   }
//   public string Fld_PrefixIR1          
//   {
//      get { return tbx_prefixIR1Rn.Text;}
//      set {        tbx_prefixIR1Rn.Text = value; }
//   }
//   public string Fld_PrefixIR2          
//   {
//      get { return tbx_prefixIR2Rn.Text; }
//      set {        tbx_prefixIR2Rn.Text = value; }
//   }
//   public uint   Fld_PnbM               
//   {
//      get { return tbx_pnbM.GetUintField(); }
//      set {        tbx_pnbM.PutUintField(value); }
//   }
//   public string Fld_PrefixIR1_Rn       
//   {
//      get { return tbx_prefixIR1_Rn.Text; }
//      set {        tbx_prefixIR1_Rn.Text = value; }
//   }
//   public string Fld_PrefixIR1_Pn       
//   {
//      get { return tbx_prefixIR1_Pn.Text; }
//      set {        tbx_prefixIR1_Pn.Text = value; }
//   }
//   public string Fld_PrefixIR2_Rn       
//   {
//      get { return tbx_prefixIR2_Rn.Text; }
//      set {        tbx_prefixIR2_Rn.Text = value; }
//   }
//   public string Fld_PrefixIR2_Pn       
//   {
//      get { return tbx_prefixIR2_Pn.Text; }
//      set {        tbx_prefixIR2_Pn.Text = value; }
//   }
//   public string Fld_IsAddTtNum_Rn      
//         {
//      get { return tbx_IsAddTtNum_Rn.Text; }
//      set {        tbx_IsAddTtNum_Rn.Text = value; }
//   }
//   public string Fld_IsAddTtNum_Pn      
//   {
//      get { return tbx_IsAddTtNum_Pn.Text; }
//      set {        tbx_IsAddTtNum_Pn.Text = value; }
//   }
//   public string Fld_IsAddYear_Rn       
//         {
//      get { return tbx_IsAddYear_Rn.Text; }
//      set {        tbx_IsAddYear_Rn.Text = value; }
//   }
//   public string Fld_IsAddYear_Pn       
//   {
//      get { return tbx_IsAddYear_Pn.Text; }
//      set {        tbx_IsAddYear_Pn.Text = value; }
//   }
//   public string Fld_IsAddKupDobCd_Rn   
//         {
//      get { return tbx_IsAddKupDobCd_Rn.Text; }
//      set {        tbx_IsAddKupDobCd_Rn.Text = value; }
//   }
//   public string Fld_IsAddKupDobCd_Pn   
//   {
//      get { return tbx_IsAddKupDobCd_Pn.Text; }
//      set {        tbx_IsAddKupDobCd_Pn.Text = value; }
//   }
//   public string Fld_Title              
//   {
//      get { return tbx_nazivPrinta.Text;}
//      set {        tbx_nazivPrinta.Text = value; }
//   }
//   public ushort Fld_ChoseObrazac       
//   {
//      get
//      {
//         if     (rbt_ira1.Checked) return 1;
//         else if(rbt_ira2.Checked) return 2;
//         else if(rbt_ira3.Checked) return 3;
//         else if(rbt_ira4.Checked) return 4;

//         else throw new Exception("Fld_AdresaLeftRight: who df is checked?");
//      }
//      set
//      {
//         if     ((ushort)value == 1) rbt_ira1.Checked = true;
//         else if((ushort)value == 2) rbt_ira2.Checked = true;
//         else if((ushort)value == 3) rbt_ira3.Checked = true;
//         else if((ushort)value == 4) rbt_ira4.Checked = true;
     
//      }
//   }
//   public string Fld_LblPrimio          
//   {
//      get { return tbx_Primio.Text; }
//      set {        tbx_Primio.Text = value; }
//   }
//   public bool Fld_RazmakRows           
//   {
//      get { return cbx_razmak.Checked; }
//      set {        cbx_razmak.Checked = value; }
//   }
//   public string Fld_LblOpciA           
//   {
//      get { return tbx_opciA.Text; }
//      set {        tbx_opciA.Text = value; }
//   }
//   public string Fld_LblOpciB           
//   {
//      get { return tbx_opciB.Text; }
//      set {        tbx_opciB.Text = value; }
//   }
//   public bool Fld_T_artiklCD         {  get { return cbx_T_artiklCD  .Checked; } set { cbx_T_artiklCD  .Checked = value; }   }
//   public bool Fld_T_artiklName       {  get { return cbx_T_artiklName.Checked; } set { cbx_T_artiklName.Checked = value; }   } 
//   public bool Fld_Napomena           {  get { return cbx_Napomena    .Checked; } set { cbx_Napomena    .Checked = value; }   }
//   public bool Fld_BarCode1           {  get { return cbx_BarCode1    .Checked; } set { cbx_BarCode1    .Checked = value; }   }
//   public bool Fld_ArtiklCD2          {  get { return cbx_ArtiklCD2   .Checked; } set { cbx_ArtiklCD2   .Checked = value; }   }
//   public bool Fld_ArtiklName2        {  get { return cbx_ArtiklName2 .Checked; } set { cbx_ArtiklName2 .Checked = value; }   }
//   public bool Fld_BarCode2           {  get { return cbx_BarCode2    .Checked; } set { cbx_BarCode2    .Checked = value; }   }
//   public bool Fld_LongOpis           {  get { return cbx_LongOpis    .Checked; } set { cbx_LongOpis    .Checked = value; }   }
//   public bool Fld_SerNo              {  get { return cbx_SerNo       .Checked; } set { cbx_SerNo       .Checked = value; }   }
//   public bool Fld_T_jedMj            {  get { return cbx_T_jedMj     .Checked; } set { cbx_T_jedMj     .Checked = value; }   }
//   public bool Fld_T_kol              {  get { return cbx_T_kol       .Checked; } set { cbx_T_kol       .Checked = value; }   }
//   public bool Fld_T_cij              {  get { return cbx_T_cij       .Checked; } set { cbx_T_cij       .Checked = value; }   }
//   public bool Fld_R_KC               {  get { return cbx_R_KC        .Checked; } set { cbx_R_KC        .Checked = value; }   }
//   public bool Fld_T_rbt1St           {  get { return cbx_T_rbt1St    .Checked; } set { cbx_T_rbt1St    .Checked = value; }   }
//   public bool Fld_R_rbt1             {  get { return cbx_R_rbt1      .Checked; } set { cbx_R_rbt1      .Checked = value; }   }
//   public bool Fld_T_rbt2St           {  get { return cbx_T_rbt2St    .Checked; } set { cbx_T_rbt2St    .Checked = value; }   }
//   public bool Fld_R_rbt2             {  get { return cbx_R_rbt2      .Checked; } set { cbx_R_rbt2      .Checked = value; }   }
//   public bool Fld_R_cij_KCR          {  get { return cbx_R_cij_KCR   .Checked; } set { cbx_R_cij_KCR   .Checked = value; }   }
//   public bool Fld_R_KCR              {  get { return cbx_R_KCR       .Checked; } set { cbx_R_KCR       .Checked = value; }   }
//   public bool Fld_T_mrzSt            {  get { return cbx_T_mrzSt     .Checked; } set { cbx_T_mrzSt     .Checked = value; }   }
//   public bool Fld_R_mrz              {  get { return cbx_R_mrz       .Checked; } set { cbx_R_mrz       .Checked = value; }   }
//   public bool Fld_R_cij_KCRM         {  get { return cbx_R_cij_KCRM  .Checked; } set { cbx_R_cij_KCRM  .Checked = value; }   }
//   public bool Fld_R_KCRM             {  get { return cbx_R_KCRM      .Checked; } set { cbx_R_KCRM      .Checked = value; }   }
//   public bool Fld_R_ztr              {  get { return cbx_R_ztr       .Checked; } set { cbx_R_ztr       .Checked = value; }   }
//   public bool Fld_T_pdvSt            {  get { return cbx_T_pdvSt     .Checked; } set { cbx_T_pdvSt     .Checked = value; }   }
//   public bool Fld_R_pdv              {  get { return cbx_R_pdv       .Checked; } set { cbx_R_pdv       .Checked = value; }   }
//   public bool Fld_R_cij_KCRP         {  get { return cbx_R_cij_KCRP  .Checked; } set { cbx_R_cij_KCRP  .Checked = value; }   }
//   public bool Fld_R_KCRP             {  get { return cbx_R_KCRP      .Checked; } set { cbx_R_KCRP      .Checked = value; }   }
//   public bool Fld_T_doCijMal         {  get { return cbx_T_doCijMal  .Checked; } set { cbx_T_doCijMal  .Checked = value; }   }
//   public bool Fld_T_noCijMal         {  get { return cbx_T_noCijMal  .Checked; } set { cbx_T_noCijMal  .Checked = value; }   }
//   public bool Fld_R_mjMasaN          {  get { return cbx_R_mjMasaN   .Checked; } set { cbx_R_mjMasaN   .Checked = value; }   }

//   public int  Fld_NumDecT_kol        {  get { return tbx_NumDecT_kol      .GetIntField();}  set { tbx_NumDecT_kol      .PutIntField(value); } }
//   public int  Fld_NumDecT_cij        {  get { return tbx_NumDecT_cij      .GetIntField();}  set { tbx_NumDecT_cij      .PutIntField(value); } }
//   public int  Fld_NumDecR_KC         {  get { return tbx_NumDecR_KC       .GetIntField();}  set { tbx_NumDecR_KC       .PutIntField(value); } }
//   public int  Fld_NumDecT_rbt1St     {  get { return tbx_NumDecT_rbt1St   .GetIntField();}  set { tbx_NumDecT_rbt1St   .PutIntField(value); } }
//   public int  Fld_NumDecR_rbt1       {  get { return tbx_NumDecR_rbt1     .GetIntField();}  set { tbx_NumDecR_rbt1     .PutIntField(value); } }
//   public int  Fld_NumDecT_rbt2St     {  get { return tbx_NumDecT_rbt2St   .GetIntField();}  set { tbx_NumDecT_rbt2St   .PutIntField(value); } }
//   public int  Fld_NumDecR_rbt2       {  get { return tbx_NumDecR_rbt2     .GetIntField();}  set { tbx_NumDecR_rbt2     .PutIntField(value); } }
//   public int  Fld_NumDecR_cij_KCR    {  get { return tbx_NumDecR_cij_KCR  .GetIntField();}  set { tbx_NumDecR_cij_KCR  .PutIntField(value); } }
//   public int  Fld_NumDecR_KCR        {  get { return tbx_NumDecR_KCR      .GetIntField();}  set { tbx_NumDecR_KCR      .PutIntField(value); } }
//   public int  Fld_NumDecT_mrzSt      {  get { return tbx_NumDecT_mrzSt    .GetIntField();}  set { tbx_NumDecT_mrzSt    .PutIntField(value); } }
//   public int  Fld_NumDecR_mrz        {  get { return tbx_NumDecR_mrz      .GetIntField();}  set { tbx_NumDecR_mrz      .PutIntField(value); } }
//   public int  Fld_NumDecR_cij_KCRM   {  get { return tbx_NumDecR_cij_KCRM .GetIntField();}  set { tbx_NumDecR_cij_KCRM .PutIntField(value); } }
//   public int  Fld_NumDecR_KCRM       {  get { return tbx_NumDecR_KCRM     .GetIntField();}  set { tbx_NumDecR_KCRM     .PutIntField(value); } }
//   public int  Fld_NumDecR_ztr        {  get { return tbx_NumDecR_ztr      .GetIntField();}  set { tbx_NumDecR_ztr      .PutIntField(value); } }
//   public int  Fld_NumDecT_pdvSt      {  get { return tbx_NumDecT_pdvSt    .GetIntField();}  set { tbx_NumDecT_pdvSt    .PutIntField(value); } }
//   public int  Fld_NumDecR_pdv        {  get { return tbx_NumDecR_pdv      .GetIntField();}  set { tbx_NumDecR_pdv      .PutIntField(value); } }
//   public int  Fld_NumDecR_cij_KCRP  {  get { return tbx_NumDecR_cij_KCRP.GetIntField();}  set { tbx_NumDecR_cij_KCRP.PutIntField(value); } }
//   public int  Fld_NumDecR_KCRP      {  get { return tbx_NumDecR_KCRP    .GetIntField();}  set { tbx_NumDecR_KCRP    .PutIntField(value); } }
//   public int  Fld_NumDecT_doCijMal   {  get { return tbx_NumDecT_doCijMal .GetIntField();}  set { tbx_NumDecT_doCijMal .PutIntField(value); } }
//   public int  Fld_NumDecT_noCijMal   {  get { return tbx_NumDecT_noCijMal .GetIntField();}  set { tbx_NumDecT_noCijMal .PutIntField(value); } }

//   public string Fld_BelowGrid
//   {
//      get { return tbx_belowGrid.Text;         }
//      set {        tbx_belowGrid.Text = value; }
//   }

//   public bool   Fld_OcuHeader  { get { return cbx_ocuHeader.Checked; } set { cbx_ocuHeader.Checked = value; } }
//   public bool   Fld_OcuFooter  { get { return cbx_ocuFooter.Checked; } set { cbx_ocuFooter.Checked = value; } }
//   public bool   Fld_OcuLogo    { get { return cbx_ocuLogo  .Checked; } set { cbx_ocuLogo  .Checked = value; } }
//   public decimal Fld_ScalingLogo        
//   {
//      get { return tbx_scalingLogo.GetDecimalField(); }
//      set {        tbx_scalingLogo.PutDecimalField(value); }
//   }
//   public bool   Fld_OcuIspisPnb { get { return cbx_ocuPnb.Checked; } set { cbx_ocuPnb.Checked = value; } }

//   public bool   Fld_IsAddTT { get { return cbx_IsAddTT.Checked; } set { cbx_IsAddTT.Checked = value; } }
//   //public string Fld_TTinf
//   //{
//   //   get { return tbx_TTinf.Text; }
//   //   set {        tbx_TTinf.Text = value; }
//   //}
//   public string Fld_SeparIfTT_Rn { get { return tbx_SeparIfTT_Rn.Text; } set { tbx_SeparIfTT_Rn.Text = value; } }

//   public int Fld_FontOpis       
//   {
//      get
//      {
//         if     (rbt_opis7 .Checked) return  7;
//         else if(rbt_opis8 .Checked) return  8;
//         else if(rbt_opis9 .Checked) return  9;
//         else if(rbt_opis10.Checked) return 10;

//         else throw new Exception("Fld_AdresaLeftRight: who df is checked?");
//      }
//      set
//      {
//         if     ((int)value ==  7) rbt_opis7 .Checked = true;
//         else if((int)value ==  8) rbt_opis8 .Checked = true;
//         else if((int)value ==  9) rbt_opis9 .Checked = true;
//         else if((int)value == 10) rbt_opis10.Checked = true;
      
//      }
//   }
//   public int Fld_FontColumns    
//   {
//      get
//      {
//         if     (rbt_col7 .Checked) return  7;
//         else if(rbt_col8 .Checked) return  8;
//         else if(rbt_col9 .Checked) return  9;
//         else if(rbt_col10.Checked) return 10;

//         else throw new Exception("Fld_AdresaLeftRight: who df is checked?");
//      }
//      set
//      {
//         if     ((int)value ==  7) rbt_col7 .Checked = true;
//         else if((int)value ==  8) rbt_col8 .Checked = true;
//         else if((int)value ==  9) rbt_col9 .Checked = true;
//         else if((int)value == 10) rbt_col10.Checked = true;
      
//      }
//   }
//   public int Fld_FontBelGr      
//   {
//      get
//      {
//         if     (rbt_belGr7 .Checked) return  7;
//         else if(rbt_belGr8 .Checked) return  8;
//         else if(rbt_belGr9 .Checked) return  9;
//         else if(rbt_belGr10.Checked) return 10;

//         else throw new Exception("Fld_AdresaLeftRight: who df is checked?");
//      }
//      set
//      {
//         if     ((int)value ==  7) rbt_belGr7 .Checked = true;
//         else if((int)value ==  8) rbt_belGr8 .Checked = true;
//         else if((int)value ==  9) rbt_belGr9 .Checked = true;
//         else if((int)value == 10) rbt_belGr10.Checked = true;
      
//      }
//   }

//   public uint Fld_PrefixIR1Pb          
//   {
//      get { return tbx_prefixIR1Pb.GetUintField();}
//      set {        tbx_prefixIR1Pb.PutUintField(value); }
//   }
//   public string Fld_PrefixIR2Pb        
//   {
//      get { return tbx_prefixIR2Pb.Text; }
//      set {        tbx_prefixIR2Pb.Text = value; }
//   }
//   public bool Fld_IsAddTtNum_Pb        
//   {
//      get { return cbx_IsAddTtNumPb.Checked; }
//      set {        cbx_IsAddTtNumPb.Checked = value; }
//   }
//   public bool Fld_IsAddYear_Pb         
//   {
//      get { return cbx_IsAddYearPb.Checked; }
//      set {        cbx_IsAddYearPb.Checked = value; }
//   }
//   public bool Fld_IsAddKupDobCd_Pb     
//   {
//      get { return cbx_IsAddKupDobCdPb.Checked; }
//      set {        cbx_IsAddKupDobCdPb.Checked = value; }
//   }

//   public bool Fld_OcuR12     
//   {
//      get { return cbx_ocuR12.Checked; }
//      set {        cbx_ocuR12.Checked = value; }
//   }

//   public string Fld_JezikReporta       
//   {
//      get
//      {
//         if     (rbt_hrv .Checked) return "H";
//         else if(rbt_eng .Checked) return "E";
//         else if(rbt_njem.Checked) return "C";
//         else if(rbt_tal .Checked) return "D";

//         else throw new Exception("Fld_JezikRpta: who df is checked?");
//      }
//      set
//      {
//         if     ((string)value == "H") rbt_hrv .Checked = true;
//         else if((string)value == "E") rbt_eng .Checked = true;
//         else if((string)value == "C") rbt_njem.Checked = true;
//         else if((string)value == "D") rbt_tal .Checked = true;
//      }
//   }

//   public bool Fld_blgOcuColKonto   { get { return cbx_blgOcuColKonto  .Checked; } set { cbx_blgOcuColKonto  .Checked = value; }  }
//   public bool Fld_blgOcuColRacun   { get { return cbx_blgOcuColRacun  .Checked; } set { cbx_blgOcuColRacun  .Checked = value; }  }
//   public bool Fld_blgOcu2na1strani { get { return cbx_blgOcu2na1strani.Checked; } set { cbx_blgOcu2na1strani.Checked = value; }  }
//   public bool Fld_blgOcuOkvirUplsp { get { return cbx_blgOcuOkvirUplsp.Checked; } set { cbx_blgOcuOkvirUplsp.Checked = value; }  }

//   // new 06.2011.______________________________________________________________________________________________________________________________
//   public bool   Fld_OcuKupDobTel        { get { return cbx_OcuKupDobTel          .Checked; } set { cbx_OcuKupDobTel          .Checked = value; }  }
//   public bool   Fld_OcuProjektTel       { get { return cbx_OcuProjektTel         .Checked; } set { cbx_OcuProjektTel         .Checked = value; }  }
//   public bool   Fld_OcuKupDobFax        { get { return cbx_OcuKupDobFax          .Checked; } set { cbx_OcuKupDobFax          .Checked = value; }  }
//   public bool   Fld_OcuProjektFax       { get { return cbx_OcuProjektFax         .Checked; } set { cbx_OcuProjektFax         .Checked = value; }  }
//   public bool   Fld_OcuKupDobOib        { get { return cbx_OcuKupDobOib          .Checked; } set { cbx_OcuKupDobOib          .Checked = value; }  }
//   public bool   Fld_OcuProjektOib       { get { return cbx_OcuProjektOib         .Checked; } set { cbx_OcuProjektOib         .Checked = value; }  }
//   public bool   Fld_OcuKupDobMail       { get { return cbx_OcuKupDobMail         .Checked; } set { cbx_OcuKupDobMail         .Checked = value; }  }
//   public bool   Fld_OcuProjektMail      { get { return cbx_OcuProjektMail        .Checked; } set { cbx_OcuProjektMail        .Checked = value; }  }
//   public bool   Fld_OcuKupDobNr         { get { return cbx_OcuKupDobNr           .Checked; } set { cbx_OcuKupDobNr           .Checked = value; }  }
//   public bool   Fld_PotvrdaNarudzbe     { get { return cbx_PotvrdaNarudzbe       .Checked; } set { cbx_PotvrdaNarudzbe       .Checked = value; }  }
//   public bool   Fld_OcuIspisDospjecePl  { get { return cbx_OcuIspisDospjecePl    .Checked; } set { cbx_OcuIspisDospjecePl    .Checked = value; }  }
//   public bool   Fld_OcuIspisVeze1       { get { return cbx_OcuIspisVeze1         .Checked; } set { cbx_OcuIspisVeze1         .Checked = value; }  }
//   public bool   Fld_OcuIspisVeze2       { get { return cbx_OcuIspisVeze2         .Checked; } set { cbx_OcuIspisVeze2         .Checked = value; }  }
//   public bool   Fld_OcuIspisVeze3       { get { return cbx_OcuIspisVeze3         .Checked; } set { cbx_OcuIspisVeze3         .Checked = value; }  }
//   public bool   Fld_OcuIspisVeze4       { get { return cbx_OcuIspisVeze4         .Checked; } set { cbx_OcuIspisVeze4         .Checked = value; }  }
//   public bool   Fld_OcuIspisVezDok2     { get { return cbx_OcuIspisVezDok2       .Checked; } set { cbx_OcuIspisVezDok2       .Checked = value; }  }
//   public bool   Fld_OcuIspisNapomena2   { get { return cbx_OcuIspisNapomena2     .Checked; } set { cbx_OcuIspisNapomena2     .Checked = value; }  }
//   public bool   Fld_OcuIspisDokNum2     { get { return cbx_OcuIspisDokNum2       .Checked; } set { cbx_OcuIspisDokNum2       .Checked = value; }  }
//   public bool   Fld_OcuMemoHOnAllPages  { get { return cbx_OcuMemoHAllPages      .Checked; } set { cbx_OcuMemoHAllPages      .Checked = value; }  }
//   public bool   Fld_OcuMemoFOnAllPages  { get { return cbx_OcuMemoFAllPages      .Checked; } set { cbx_OcuMemoFAllPages      .Checked = value; }  }
//   public bool   Fld_OcuTitleOkvir       { get { return cbx_OcuTitleOkvir         .Checked; } set { cbx_OcuTitleOkvir         .Checked = value; }  }
//   public bool   Fld_OcuTitleBoja        { get { return cbx_OcuTitleBoja          .Checked; } set { cbx_OcuTitleBoja          .Checked = value; }  }
//   public bool   Fld_OcuKupDobBoja       { get { return cbx_OcuKupDobBoja         .Checked; } set { cbx_OcuKupDobBoja         .Checked = value; }  }
//   public bool   Fld_OcuPrjktBoja        { get { return cbx_OcuPrjktBoja          .Checked; } set { cbx_OcuPrjktBoja          .Checked = value; }  }
//   public bool   Fld_OcuLinijeHeader     { get { return cbx_OcuLinijeHeader       .Checked; } set { cbx_OcuLinijeHeader       .Checked = value; }  }
//   public bool   Fld_OcuLinijeFooter     { get { return cbx_OcuLinijeFooter       .Checked; } set { cbx_OcuLinijeFooter       .Checked = value; }  }
//   public bool   Fld_OcuIspisVirmana     { get { return cbx_OcuIspisVirmana       .Checked; } set { cbx_OcuIspisVirmana       .Checked = value; }  }
//   public bool   Fld_OcuKDZiro_Vir       { get { return cbx_OcuKDZiro_Vir         .Checked; } set { cbx_OcuKDZiro_Vir         .Checked = value; }  }
//   public bool   Fld_OcuVezDok           { get { return cbx_ocuVezDok             .Checked; } set { cbx_ocuVezDok             .Checked = value; }  }
//   public bool   Fld_OcuJednakiFtTxt2Red { get { return cbx_textOpisDrugiRed      .Checked; } set { cbx_textOpisDrugiRed      .Checked = value; }  }

//   public string Fld_LblVeze1            { get { return tbx_LblVeze1  .Text; } set { tbx_LblVeze1  .Text = value; } }
//   public string Fld_LblVeze2            { get { return tbx_LblVeze2  .Text; } set { tbx_LblVeze2  .Text = value; } }
//   public string Fld_LblVeze3            { get { return tbx_LblVeze3  .Text; } set { tbx_LblVeze3  .Text = value; } }
//   public string Fld_LblVeze4            { get { return tbx_LblVeze4  .Text; } set { tbx_LblVeze4  .Text = value; } }
//   public string Fld_LblVezDok2          { get { return tbx_LblVezDok2.Text; } set { tbx_LblVezDok2.Text = value; } }
//   public string Fld_LblOsobaX           { get { return tbx_LblOsobaX .Text; } set { tbx_LblOsobaX .Text = value; } } 
//   public string Fld_AboveGrid           { get { return tbx_AboveGrid .Text; } set { tbx_AboveGrid .Text = value; } }
//   public string Fld_BeforNRD            { get { return tbx_beforNRD  .Text; } set { tbx_beforNRD  .Text = value; } }
//   public string Fld_AfterNRD            { get { return tbx_afterNRD  .Text; } set { tbx_afterNRD  .Text = value; } }
  
//   public string Fld_AlignmentPageNum           
//   {
//      get
//      {
//         if     (rbt_alignmentPageNum_L.Checked) return "L";
//         else if(rbt_alignmentPageNum_C.Checked) return "C";
//         else if(rbt_alignmentPageNum_R.Checked) return "R";

//         else throw new Exception("Fld_AlignmentPageNum: who df is checked?");
//      }
//      set
//      {
//         if     ((string)value == "L") rbt_alignmentPageNum_L.Checked = true;
//         else if((string)value == "C") rbt_alignmentPageNum_C.Checked = true;
//         else if((string)value == "R") rbt_alignmentPageNum_R.Checked = true;
      
//      }
//   }
//   public string Fld_PrintPageNum               
//   {
//      get
//      {
//         if     (rbt_printPageNum_PHeader.Checked) return "H";
//         else if(rbt_printPageNum_PFooter.Checked) return "F";
//         else if(rbt_printPageNum_NO     .Checked) return "N";

//         else throw new Exception("Fld_PrintPageNum: who df is checked?");
//      }
//      set
//      {
//         if     ((string)value == "H") rbt_printPageNum_PHeader.Checked = true;
//         else if((string)value == "F") rbt_printPageNum_PFooter.Checked = true;
//         else if((string)value == "N") rbt_printPageNum_NO     .Checked = true;
      
//      }
//   }
//   public string Fld_AlignmentDokNum            
//   {
//      get
//      {
//         if     (rbt_alignmentDokNum_L.Checked) return "L";
//         else if(rbt_alignmentDokNum_C.Checked) return "C";
//         else if(rbt_alignmentDokNum_R.Checked) return "R";

//         else throw new Exception("Fld_AlignmentDokNum: who df is checked?");
//      }
//      set
//      {
//         if     ((string)value == "L") rbt_alignmentDokNum_L.Checked = true;
//         else if((string)value == "C") rbt_alignmentDokNum_C.Checked = true;
//         else if((string)value == "R") rbt_alignmentDokNum_R.Checked = true;
      
//      }
//   }
//   public bool   Fld_AdresOkvir                 
//   {
//      get
//      {
//              if(rbt_adresOkvir.Checked)   return true;
//         else if(rbt_adresNoOkvir.Checked) return false;

//         else throw new Exception("Fld_AdresOkvir: who df is checked?");
//      }
//      set
//      {
//         if     (value == true) rbt_adresOkvir.Checked   = true;
//         else                   rbt_adresNoOkvir.Checked = true;
//      }
//   }
//   public bool   Fld_AdresOnlyPartner           
//   {
//      get
//      {
//              if(rbt_adresOnlyPartner.Checked)   return true;
//         else if(rbt_adresBoth       .Checked) return false;

//         else throw new Exception("Fld_AdresOnlyPartner: who df is checked?");
//      }
//      set
//      {
//         if  (value == true) rbt_adresOnlyPartner.Checked   = true;
//         else                rbt_adresBoth       .Checked = true;
//      }
//   }
//   public bool   Fld_PrintDokNum_BeforAdres     
//   {
//      get
//      {
//              if(rbt_printDokNum_beforAdres.Checked) return true;
//         else if(rbt_printDokNum_afterAdres.Checked) return false;

//         else throw new Exception("Fld_PrintDokNum_BeforAdres: who df is checked?");
//      }
//      set
//      {
//         if  (value == true) rbt_printDokNum_beforAdres.Checked = true;
//         else                rbt_printDokNum_afterAdres.Checked = true;
//      }
//   }
//   public bool   Fld_MigPositionHeader          
//   {
//      get
//      {
//              if(rbt_mig_RH.Checked) return true;
//         else if(rbt_mig_RF.Checked) return false;

//         else throw new Exception("Fld_MigPositionHeader: who df is checked?");
//      }
//      set
//      {
//         if  (value == true) rbt_mig_RH.Checked = true;
//         else                rbt_mig_RF.Checked = true;
//      }
//   }


//   public string Fld_ObrazacA { get { return tbx_obrazacA.Text; } set { tbx_obrazacA.Text = value; } }
//   public string Fld_ObrazacB { get { return tbx_obrazacB.Text; } set { tbx_obrazacB.Text = value; } }
//   public string Fld_ObrazacC { get { return tbx_obrazacC.Text; } set { tbx_obrazacC.Text = value; } }
//   public string Fld_ObrazacD { get { return tbx_obrazacD.Text; } set { tbx_obrazacD.Text = value; } }

//   public int  Fld_LeftMargin        
//   {
//      get
//      {
//         if     (rbt_leftMargin1.Checked) return 1;
//         else if(rbt_leftMargin2.Checked) return 2;
//         else if(rbt_leftMargin3.Checked) return 3;
//         else if(rbt_leftMargin4.Checked) return 4;
//         else if(rbt_leftMargin5.Checked) return 5;

//         else throw new Exception("Fld_LeftMargin: who df is checked?");
//      }
//      set
//      {
//         if     ((int)value == 1) rbt_leftMargin1.Checked = true;
//         else if((int)value == 2) rbt_leftMargin2.Checked = true;
//         else if((int)value == 3) rbt_leftMargin3.Checked = true;
//         else if((int)value == 4) rbt_leftMargin4.Checked = true;
//         else if((int)value == 5) rbt_leftMargin5.Checked = true;
      
//      }
//   }
//   public int  Fld_RightMargin       
//   {
//      get
//      {
//         if     (rbt_rightMargin1.Checked) return 1;
//         else if(rbt_rightMargin2.Checked) return 2;
//         else if(rbt_rightMargin3.Checked) return 3;
//         else if(rbt_rightMargin4.Checked) return 4;

//         else throw new Exception("Fld_LeftMargin: who df is checked?");
//      }
//      set
//      {
//         if     ((int)value == 1) rbt_rightMargin1.Checked = true;
//         else if((int)value == 2) rbt_rightMargin2.Checked = true;
//         else if((int)value == 3) rbt_rightMargin3.Checked = true;
//         else if((int)value == 4) rbt_rightMargin4.Checked = true;
      
//      }
//   }
//   public bool Fld_OcuLinijePerson   { get { return cbx_ocuLinijePerson.Checked; } set { cbx_ocuLinijePerson.Checked = value; } }
//   public bool Fld_OcuZiroFromFak    { get { return cbx_ocuZiro        .Checked; } set { cbx_ocuZiro        .Checked = value; } }
//   public bool Fld_IspisNapomene     { get { return cbx_ocuNapomena    .Checked; } set { cbx_ocuNapomena    .Checked = value; } }
//   public bool Fld_OcuTextNap2       { get { return cbx_ocuTextNap2    .Checked; } set { cbx_ocuTextNap2    .Checked = value; } }
//   public bool Fld_OcuFirmuUpotpis   { get { return cbx_ocuFirmuUpotpis.Checked; } set { cbx_ocuFirmuUpotpis.Checked = value; } }
//   public bool Fld_OcuTitulu         { get { return cbx_ocuTitulu      .Checked; } set { cbx_ocuTitulu      .Checked = value; } }
//   public bool Fld_PositionPersonR   
//   {
//      get
//      {
//              if(rbt_personRight .Checked) return true;
//         else if(rbt_personCentar.Checked) return false;

//         else throw new Exception("Fld_MigPositionHeader: who df is checked?");
//      }
//      set
//      {
//         if(value == true) rbt_personRight .Checked = true;
//         else              rbt_personCentar.Checked = true;
//      }
//   }
        
//   #endregion Fld_

//   #region Put & GetFilterFields

//   private FakturDocFilter TheFakturDocFilter
//   {
//      get { return this.TheVvUC.VirtualRptFilter as FakturDocFilter; }
//      set {        this.TheVvUC.VirtualRptFilter = value; }
//   }

//   public override void PutFilterFields(VvRptFilter _filter_data)
//   {
//      TheFakturDocFilter = (FakturDocFilter)_filter_data;

//      if(TheFakturDocFilter != null)
//      {
//         Fld_NeedsHorizontalLine = TheFakturDocFilter.PFD.Dsc_HorizontalLine;
      
//         Fld_AdresaLeftRight  = TheFakturDocFilter.PFD.Dsc_AdresaLeftRight;
//         Fld_RptOrientation   = TheFakturDocFilter.PFD.Dsc_RptOrientation ;
//         Fld_VertikalLine     = TheFakturDocFilter.PFD.Dsc_VertikalLine   ;
//         Fld_TableBorder      = TheFakturDocFilter.PFD.Dsc_TableBorder    ;
//         Fld_TekstualIznos    = TheFakturDocFilter.PFD.Dsc_TekstualIznos  ;
//         Fld_RazmakRows       = TheFakturDocFilter.PFD.Dsc_RazmakRows     ;

//         Fld_IsAddTtNum       = TheFakturDocFilter.PFD.Dsc_IsAddTtNum     ;
//         Fld_IsAddYear        = TheFakturDocFilter.PFD.Dsc_IsAddYear      ;
//         Fld_IsAddKupDobCd    = TheFakturDocFilter.PFD.Dsc_IsAddKupDobCd  ;
//         Fld_PrefixIR1        = TheFakturDocFilter.PFD.Dsc_Prefix1Rn      ;
//         Fld_PrefixIR2        = TheFakturDocFilter.PFD.Dsc_Prefix2Rn      ;
//         Fld_PnbM             = TheFakturDocFilter.PFD.Dsc_PnbM           ;
//         Fld_PrefixIR1_Rn     = TheFakturDocFilter.PFD.Dsc_Separ1_Rn      ;
//         Fld_PrefixIR1_Pn     = TheFakturDocFilter.PFD.Dsc_Separ1_Pn      ;
//         Fld_PrefixIR2_Rn     = TheFakturDocFilter.PFD.Dsc_Separ2_Rn      ;
//         Fld_PrefixIR2_Pn     = TheFakturDocFilter.PFD.Dsc_Separ2_Pn      ;
//         Fld_IsAddTtNum_Rn    = TheFakturDocFilter.PFD.Dsc_SeparIfTtNum_Rn;
//         Fld_IsAddTtNum_Pn    = TheFakturDocFilter.PFD.Dsc_SeparIfTtNum_Pn;
//         Fld_IsAddYear_Rn     = TheFakturDocFilter.PFD.Dsc_SeparIfYear_Rn ;
//         Fld_IsAddYear_Pn     = TheFakturDocFilter.PFD.Dsc_SeparIfYear_Pn ;
//         Fld_IsAddKupDobCd_Rn = TheFakturDocFilter.PFD.Dsc_SeparIfKDcd_Rn ;
//         Fld_IsAddKupDobCd_Pn = TheFakturDocFilter.PFD.Dsc_SeparIfKDcd_Pn ;
//         Fld_Title            = TheFakturDocFilter.PFD.Dsc_Title          ;
      
//         Fld_PrefixIR1Pb      = TheFakturDocFilter.PFD.Dsc_PrefixIR1Pb     ;
//         Fld_PrefixIR2Pb      = TheFakturDocFilter.PFD.Dsc_PrefixIR2PbTx     ;
//         Fld_IsAddTtNum_Pb    = TheFakturDocFilter.PFD.Dsc_IsAddTtNum_Pb   ;
//         Fld_IsAddYear_Pb     = TheFakturDocFilter.PFD.Dsc_IsAddYear_Pb    ;
//         Fld_IsAddKupDobCd_Pb = TheFakturDocFilter.PFD.Dsc_IsAddKupDobCd_Pb;

//         Fld_LblPrjktPerson   = TheFakturDocFilter.PFD.Dsc_LblPrjktPerson ;
//         Fld_LblUserPerson    = TheFakturDocFilter.PFD.Dsc_LblUserPerson  ;
//         Fld_LblOsobaA        = TheFakturDocFilter.PFD.Dsc_LblOsobaA      ;
//         Fld_LblOsobaB        = TheFakturDocFilter.PFD.Dsc_LblOsobaB      ;
//         Fld_SignPrimaoc      = TheFakturDocFilter.PFD.Dsc_SignPrimaoc    ;
//         Fld_LblPrimio        = TheFakturDocFilter.PFD.Dsc_LblPrimio      ;
//         Fld_LblOpciA         = TheFakturDocFilter.PFD.Dsc_LblOpciA      ;
//         Fld_LblOpciB         = TheFakturDocFilter.PFD.Dsc_LblOpciB      ;

//         Fld_DevName          = (faktur_rec.TtInfo.IsExtendableTT ? faktur_rec.DevName : "");
//         Fld_NazivPoslJed     = TheFakturDocFilter.PFD.Dsc_NazivPoslJed    ;
//         Fld_ChoseObrazac     = TheFakturDocFilter.ChosenObrazac           ;

//         Fld_T_artiklCD        = TheFakturDocFilter.PFD.Dsc_T_artiklCD           ;
//         Fld_T_artiklName      = TheFakturDocFilter.PFD.Dsc_T_artiklName         ;
//         Fld_Napomena          = TheFakturDocFilter.PFD.Dsc_NapomenaArt          ;
//         Fld_BarCode1          = TheFakturDocFilter.PFD.Dsc_BarCode1             ;
//         Fld_ArtiklCD2         = TheFakturDocFilter.PFD.Dsc_ArtiklCD2            ;
//         Fld_ArtiklName2       = TheFakturDocFilter.PFD.Dsc_ArtiklName2          ;
//         Fld_BarCode2          = TheFakturDocFilter.PFD.Dsc_BarCode2             ;
//         Fld_LongOpis          = TheFakturDocFilter.PFD.Dsc_LongOpis             ;
//         Fld_SerNo             = TheFakturDocFilter.PFD.Dsc_SerNo                ;
//         Fld_T_jedMj           = TheFakturDocFilter.PFD.Dsc_T_jedMj              ;
//         Fld_T_kol             = TheFakturDocFilter.PFD.Dsc_T_kol                ;
//         Fld_T_cij             = TheFakturDocFilter.PFD.Dsc_T_cij                ;
//         Fld_R_KC              = TheFakturDocFilter.PFD.Dsc_R_KC                 ;
//         Fld_T_rbt1St          = TheFakturDocFilter.PFD.Dsc_T_rbt1St             ;
//         Fld_R_rbt1            = TheFakturDocFilter.PFD.Dsc_R_rbt1               ;
//         Fld_T_rbt2St          = TheFakturDocFilter.PFD.Dsc_T_rbt2St             ;
//         Fld_R_rbt2            = TheFakturDocFilter.PFD.Dsc_R_rbt2               ;
//         Fld_R_cij_KCR         = TheFakturDocFilter.PFD.Dsc_R_cij_KCR            ;
//         Fld_R_KCR             = TheFakturDocFilter.PFD.Dsc_R_KCR                ;
//         Fld_T_mrzSt           = TheFakturDocFilter.PFD.Dsc_T_mrzSt              ;
//         Fld_R_mrz             = TheFakturDocFilter.PFD.Dsc_R_mrz                ;
//         Fld_R_cij_KCRM        = TheFakturDocFilter.PFD.Dsc_R_cij_KCRM           ;
//         Fld_R_KCRM            = TheFakturDocFilter.PFD.Dsc_R_KCRM               ;
//         Fld_R_ztr             = TheFakturDocFilter.PFD.Dsc_R_ztr                ;
//         Fld_T_pdvSt           = TheFakturDocFilter.PFD.Dsc_T_pdvSt              ;
//         Fld_R_pdv             = TheFakturDocFilter.PFD.Dsc_R_pdv                ;
//         Fld_R_cij_KCRP        = TheFakturDocFilter.PFD.Dsc_R_cij_KCRMP          ;
//         Fld_R_KCRP            = TheFakturDocFilter.PFD.Dsc_R_KCRMP              ;
//         Fld_T_doCijMal        = TheFakturDocFilter.PFD.Dsc_T_doCijMal           ;
//         Fld_T_noCijMal        = TheFakturDocFilter.PFD.Dsc_T_noCijMal           ;
//         Fld_R_mjMasaN         = TheFakturDocFilter.PFD.Dsc_R_mjMasaN            ;
//         Fld_NumDecT_kol       = TheFakturDocFilter.PFD.Dsc_NumDecT_kol          ;
//         Fld_NumDecT_cij       = TheFakturDocFilter.PFD.Dsc_NumDecT_cij          ;
//         Fld_NumDecR_KC        = TheFakturDocFilter.PFD.Dsc_NumDecR_KC           ;
//         Fld_NumDecT_rbt1St    = TheFakturDocFilter.PFD.Dsc_NumDecT_rbt1St       ;
//         Fld_NumDecR_rbt1      = TheFakturDocFilter.PFD.Dsc_NumDecR_rbt1         ;
//         Fld_NumDecT_rbt2St    = TheFakturDocFilter.PFD.Dsc_NumDecT_rbt2St       ;
//         Fld_NumDecR_rbt2      = TheFakturDocFilter.PFD.Dsc_NumDecR_rbt2         ;
//         Fld_NumDecR_cij_KCR   = TheFakturDocFilter.PFD.Dsc_NumDecR_cij_KCR      ;
//         Fld_NumDecR_KCR       = TheFakturDocFilter.PFD.Dsc_NumDecR_KCR          ;
//         Fld_NumDecT_mrzSt     = TheFakturDocFilter.PFD.Dsc_NumDecT_mrzSt        ;
//         Fld_NumDecR_mrz       = TheFakturDocFilter.PFD.Dsc_NumDecR_mrz          ;
//         Fld_NumDecR_cij_KCRM  = TheFakturDocFilter.PFD.Dsc_NumDecR_cij_KCRM     ;
//         Fld_NumDecR_KCRM      = TheFakturDocFilter.PFD.Dsc_NumDecR_KCRM         ;
//         Fld_NumDecR_ztr       = TheFakturDocFilter.PFD.Dsc_NumDecR_ztr          ;
//         Fld_NumDecT_pdvSt     = TheFakturDocFilter.PFD.Dsc_NumDecT_pdvSt        ;
//         Fld_NumDecR_pdv       = TheFakturDocFilter.PFD.Dsc_NumDecR_pdv          ;
//         Fld_NumDecR_cij_KCRP = TheFakturDocFilter.PFD.Dsc_NumDecR_cij_KCRMP    ;
//         Fld_NumDecR_KCRP     = TheFakturDocFilter.PFD.Dsc_NumDecR_KCRMP        ;
//         Fld_NumDecT_doCijMal  = TheFakturDocFilter.PFD.Dsc_NumDecT_doCijMal     ;
//         Fld_NumDecT_noCijMal  = TheFakturDocFilter.PFD.Dsc_NumDecT_noCijMal     ;

//         Fld_BelowGrid         = TheFakturDocFilter.PFD.Dsc_BelowGrid            ;  
//         Fld_OcuHeader         = TheFakturDocFilter.PFD.Dsc_OcuHeader            ;
//         Fld_OcuFooter         = TheFakturDocFilter.PFD.Dsc_OcuFooter            ;
//         Fld_OcuLogo           = TheFakturDocFilter.PFD.Dsc_OcuLogo              ;
//         Fld_ScalingLogo       = TheFakturDocFilter.PFD.Dsc_ScalingPostoLogo     ;
//         Fld_OcuIspisPnb       = TheFakturDocFilter.PFD.Dsc_OcuIspisPnb          ;
         
//         Fld_IsAddTT           = TheFakturDocFilter.PFD.Dsc_IsAddTT              ;
//         Fld_SeparIfTT_Rn      = TheFakturDocFilter.PFD.Dsc_SeparIfTT_Rn         ;

//         Fld_FontOpis          = TheFakturDocFilter.PFD.Dsc_FontOpis   ;
//         Fld_FontColumns       = TheFakturDocFilter.PFD.Dsc_FontColumns;
//         Fld_FontBelGr         = TheFakturDocFilter.PFD.Dsc_FontBelGr  ;

//         Fld_OcuR12            = TheFakturDocFilter.PFD.Dsc_OcuR12  ;
//         Fld_JezikReporta      = TheFakturDocFilter.PFD.Dsc_JezikReport  ;

//         Fld_blgOcuColKonto    = TheFakturDocFilter.PFD.Dsc_BlgOcuColKonto  ;
//         Fld_blgOcuColRacun    = TheFakturDocFilter.PFD.Dsc_BlgOcuColRacun  ;
//         Fld_blgOcu2na1strani  = TheFakturDocFilter.PFD.Dsc_BlgOcu2na1strani;
//         Fld_blgOcuOkvirUplsp  = TheFakturDocFilter.PFD.Dsc_BlgOcuOkvirUplsp;

//         Fld_AdresOnlyPartner       = TheFakturDocFilter.PFD.Dsc_AdresOnlyPartner      ;
//         Fld_OcuKupDobTel           = TheFakturDocFilter.PFD.Dsc_OcuKupDobTel          ;       
//         Fld_OcuProjektTel          = TheFakturDocFilter.PFD.Dsc_OcuProjektTel         ;
//         Fld_OcuKupDobFax           = TheFakturDocFilter.PFD.Dsc_OcuKupDobFax          ;
//         Fld_OcuProjektFax          = TheFakturDocFilter.PFD.Dsc_OcuProjektFax         ;
//         Fld_OcuKupDobOib           = TheFakturDocFilter.PFD.Dsc_OcuKupDobOib          ;
//         Fld_OcuProjektOib          = TheFakturDocFilter.PFD.Dsc_OcuProjektOib         ;
//         Fld_OcuKupDobMail          = TheFakturDocFilter.PFD.Dsc_OcuKupDobMail         ;
//         Fld_OcuProjektMail         = TheFakturDocFilter.PFD.Dsc_OcuProjektMail        ;
//         Fld_OcuKupDobNr            = TheFakturDocFilter.PFD.Dsc_OcuKupDobNr           ;
//         Fld_PotvrdaNarudzbe        = TheFakturDocFilter.PFD.Dsc_PotvrdaNarudzbe       ;
//         Fld_OcuIspisDospjecePl     = TheFakturDocFilter.PFD.Dsc_OcuIspisDospjecePl    ;
//         Fld_OcuIspisVeze1          = TheFakturDocFilter.PFD.Dsc_OcuIspisVeze1         ;
//         Fld_OcuIspisVeze2          = TheFakturDocFilter.PFD.Dsc_OcuIspisVeze2         ;
//         Fld_OcuIspisVeze3          = TheFakturDocFilter.PFD.Dsc_OcuIspisVeze3         ;
//         Fld_OcuIspisVeze4          = TheFakturDocFilter.PFD.Dsc_OcuIspisVeze4         ;
//         Fld_OcuIspisVezDok2        = TheFakturDocFilter.PFD.Dsc_OcuIspisVezDok2       ;
//         Fld_OcuIspisNapomena2      = TheFakturDocFilter.PFD.Dsc_OcuIspisNapomena2     ;
//         Fld_OcuIspisDokNum2        = TheFakturDocFilter.PFD.Dsc_OcuIspisDokNum2       ;
//         Fld_OcuMemoHOnAllPages     = TheFakturDocFilter.PFD.Dsc_OcuMemoHOnAllPages    ;
//         Fld_OcuMemoFOnAllPages     = TheFakturDocFilter.PFD.Dsc_OcuMemoFOnAllPages    ;
//         Fld_LblVeze1               = TheFakturDocFilter.PFD.Dsc_LblVeze1              ;
//         Fld_LblVeze2               = TheFakturDocFilter.PFD.Dsc_LblVeze2              ;
//         Fld_LblVeze3               = TheFakturDocFilter.PFD.Dsc_LblVeze3              ;
//         Fld_LblVeze4               = TheFakturDocFilter.PFD.Dsc_LblVeze4              ;
//         Fld_LblVezDok2             = TheFakturDocFilter.PFD.Dsc_LblVezDok2            ;
//         Fld_LblOsobaX              = TheFakturDocFilter.PFD.Dsc_LblOsobaX             ;
//         Fld_AboveGrid              = TheFakturDocFilter.PFD.Dsc_AboveGrid             ;
//         Fld_AlignmentPageNum       = TheFakturDocFilter.PFD.Dsc_AlignmentPageNum      ;
//         Fld_PrintPageNum           = TheFakturDocFilter.PFD.Dsc_PrintPageNum          ;
//         Fld_AlignmentDokNum        = TheFakturDocFilter.PFD.Dsc_AlignmentDokNum       ;
//         Fld_AdresOkvir             = TheFakturDocFilter.PFD.Dsc_AdresOkvir            ;
//         Fld_PrintDokNum_BeforAdres = TheFakturDocFilter.PFD.Dsc_PrintDokNum_BeforAdres;
//         Fld_MigPositionHeader      = TheFakturDocFilter.PFD.Dsc_MigPositionHeader     ;
//         Fld_OcuTitleOkvir          = TheFakturDocFilter.PFD.Dsc_OcuTitleOkvir         ;
//         Fld_OcuTitleBoja           = TheFakturDocFilter.PFD.Dsc_OcuTitleBoja          ;
//         Fld_OcuKupDobBoja          = TheFakturDocFilter.PFD.Dsc_OcuKupDobBoja         ;
//         Fld_OcuPrjktBoja           = TheFakturDocFilter.PFD.Dsc_OcuPrjktBoja          ;
//         Fld_OcuLinijeHeader        = TheFakturDocFilter.PFD.Dsc_OcuLinijeHeader       ;
//         Fld_OcuLinijeFooter        = TheFakturDocFilter.PFD.Dsc_OcuLinijeFooter       ;
//         Fld_BeforNRD               = TheFakturDocFilter.PFD.Dsc_BeforNRD              ;
//         Fld_AfterNRD               = TheFakturDocFilter.PFD.Dsc_AfterNRD              ;
//         Fld_OcuIspisVirmana        = TheFakturDocFilter.PFD.Dsc_OcuIspisVirmana       ;
//         Fld_ObrazacA               = TheFakturDocFilter.PFD.Dsc_ObrazacA              ;
//         Fld_ObrazacB               = TheFakturDocFilter.PFD.Dsc_ObrazacB              ;
//         Fld_ObrazacC               = TheFakturDocFilter.PFD.Dsc_ObrazacC              ;
//         Fld_ObrazacD               = TheFakturDocFilter.PFD.Dsc_ObrazacD              ;
//         Fld_OcuKDZiro_Vir          = TheFakturDocFilter.PFD.Dsc_OcuKDZiro_Vir         ;
//         Fld_OcuVezDok              = TheFakturDocFilter.PFD.Dsc_OcuIspisLblNapomena   ;

//         Fld_LeftMargin             = TheFakturDocFilter.PFD.Dsc_LeftMargin            ;
//         Fld_RightMargin            = TheFakturDocFilter.PFD.Dsc_RightMargin           ;
//         Fld_OcuLinijePerson        = TheFakturDocFilter.PFD.Dsc_OcuLinijePerson       ;
//         Fld_OcuZiroFromFak         = TheFakturDocFilter.PFD.Dsc_OcuZiroFromFak        ;
//         Fld_IspisNapomene          = TheFakturDocFilter.PFD.Dsc_IspisNapomene         ;
//         Fld_OcuTextNap2            = TheFakturDocFilter.PFD.Dsc_OcuTextNap2           ;
//         Fld_PositionPersonR        = TheFakturDocFilter.PFD.Dsc_PositionPersonR       ;
//         Fld_OcuFirmuUpotpis        = TheFakturDocFilter.PFD.Dsc_OcuFirmuUpotpis       ;
//         Fld_OcuTitulu              = TheFakturDocFilter.PFD.Dsc_OcuTitulu             ;
//         Fld_OcuJednakiFtTxt2Red    = TheFakturDocFilter.PFD.Dsc_OcuJednakiFtTxt2Red;
//       }

//      // Za JAM_... : 
//      this.ValidateChildren();
//   }

//   public override void GetFilterFields()
//   {
//      if(TheFakturDocFilter.PFD == null) return;

//      TheFakturDocFilter.PFD.Dsc_HorizontalLine   = Fld_NeedsHorizontalLine;
//      TheFakturDocFilter.PFD.Dsc_AdresaLeftRight  = Fld_AdresaLeftRight;
//      TheFakturDocFilter.PFD.Dsc_RptOrientation   = Fld_RptOrientation ;
//      TheFakturDocFilter.PFD.Dsc_VertikalLine     = Fld_VertikalLine   ;
//      TheFakturDocFilter.PFD.Dsc_TableBorder      = Fld_TableBorder    ;
//      TheFakturDocFilter.PFD.Dsc_TekstualIznos    = Fld_TekstualIznos  ;
//      TheFakturDocFilter.PFD.Dsc_RazmakRows       = Fld_RazmakRows     ;
                                                  
//      TheFakturDocFilter.PFD.Dsc_IsAddTtNum       = Fld_IsAddTtNum      ;
//      TheFakturDocFilter.PFD.Dsc_IsAddYear        = Fld_IsAddYear       ;
//      TheFakturDocFilter.PFD.Dsc_IsAddKupDobCd    = Fld_IsAddKupDobCd   ;
//      TheFakturDocFilter.PFD.Dsc_Prefix1Rn        = Fld_PrefixIR1       ;
//      TheFakturDocFilter.PFD.Dsc_Prefix2Rn        = Fld_PrefixIR2       ;
//      TheFakturDocFilter.PFD.Dsc_PnbM             = Fld_PnbM            ;
//      TheFakturDocFilter.PFD.Dsc_Separ1_Rn        = Fld_PrefixIR1_Rn    ;
//      TheFakturDocFilter.PFD.Dsc_Separ1_Pn        = Fld_PrefixIR1_Pn    ;
//      TheFakturDocFilter.PFD.Dsc_Separ2_Rn        = Fld_PrefixIR2_Rn    ;
//      TheFakturDocFilter.PFD.Dsc_Separ2_Pn        = Fld_PrefixIR2_Pn    ;
//      TheFakturDocFilter.PFD.Dsc_SeparIfTtNum_Rn  = Fld_IsAddTtNum_Rn   ;
//      TheFakturDocFilter.PFD.Dsc_SeparIfTtNum_Pn  = Fld_IsAddTtNum_Pn   ;
//      TheFakturDocFilter.PFD.Dsc_SeparIfYear_Rn   = Fld_IsAddYear_Rn    ;
//      TheFakturDocFilter.PFD.Dsc_SeparIfYear_Pn   = Fld_IsAddYear_Pn    ;
//      TheFakturDocFilter.PFD.Dsc_SeparIfKDcd_Rn   = Fld_IsAddKupDobCd_Rn;
//      TheFakturDocFilter.PFD.Dsc_SeparIfKDcd_Pn   = Fld_IsAddKupDobCd_Pn;
//      TheFakturDocFilter.PFD.Dsc_Title            = Fld_Title           ;
//      TheFakturDocFilter.PFD.Dsc_PrefixIR1Pb      = Fld_PrefixIR1Pb     ;
//      TheFakturDocFilter.PFD.Dsc_PrefixIR2PbTx    = Fld_PrefixIR2Pb     ;
//      TheFakturDocFilter.PFD.Dsc_IsAddTtNum_Pb    = Fld_IsAddTtNum_Pb   ;
//      TheFakturDocFilter.PFD.Dsc_IsAddYear_Pb     = Fld_IsAddYear_Pb    ;
//      TheFakturDocFilter.PFD.Dsc_IsAddKupDobCd_Pb = Fld_IsAddKupDobCd_Pb;

//      TheFakturDocFilter.PFD.Dsc_LblPrjktPerson   = Fld_LblPrjktPerson  ;
//      TheFakturDocFilter.PFD.Dsc_LblUserPerson    = Fld_LblUserPerson   ;
//      TheFakturDocFilter.PFD.Dsc_LblOsobaA        = Fld_LblOsobaA       ;
//      TheFakturDocFilter.PFD.Dsc_LblOsobaB        = Fld_LblOsobaB       ;
//      TheFakturDocFilter.PFD.Dsc_SignPrimaoc      = Fld_SignPrimaoc     ;
//      TheFakturDocFilter.PFD.Dsc_LblPrimio        = Fld_LblPrimio       ;
//      TheFakturDocFilter.PFD.Dsc_LblOpciA         = Fld_LblOpciA        ;
//      TheFakturDocFilter.PFD.Dsc_LblOpciB         = Fld_LblOpciB        ;
                                                  
//      TheFakturDocFilter.PFD.Dsc_NazivPoslJed     = Fld_NazivPoslJed    ;
      
//      TheFakturDocFilter.ChosenObrazac            = Fld_ChoseObrazac;

//      TheFakturDocFilter.PFD.Dsc_T_artiklCD        = Fld_T_artiklCD       ;
//      TheFakturDocFilter.PFD.Dsc_T_artiklName      = Fld_T_artiklName     ;
//      TheFakturDocFilter.PFD.Dsc_NapomenaArt       = Fld_Napomena         ;
//      TheFakturDocFilter.PFD.Dsc_BarCode1          = Fld_BarCode1         ;
//      TheFakturDocFilter.PFD.Dsc_ArtiklCD2         = Fld_ArtiklCD2        ;
//      TheFakturDocFilter.PFD.Dsc_ArtiklName2       = Fld_ArtiklName2      ;
//      TheFakturDocFilter.PFD.Dsc_BarCode2          = Fld_BarCode2         ;
//      TheFakturDocFilter.PFD.Dsc_LongOpis          = Fld_LongOpis         ;
//      TheFakturDocFilter.PFD.Dsc_SerNo             = Fld_SerNo            ;
//      TheFakturDocFilter.PFD.Dsc_T_jedMj           = Fld_T_jedMj          ;
//      TheFakturDocFilter.PFD.Dsc_T_kol             = Fld_T_kol            ;
//      TheFakturDocFilter.PFD.Dsc_T_cij             = Fld_T_cij            ;
//      TheFakturDocFilter.PFD.Dsc_R_KC              = Fld_R_KC             ;
//      TheFakturDocFilter.PFD.Dsc_T_rbt1St          = Fld_T_rbt1St         ;
//      TheFakturDocFilter.PFD.Dsc_R_rbt1            = Fld_R_rbt1           ;
//      TheFakturDocFilter.PFD.Dsc_T_rbt2St          = Fld_T_rbt2St         ;
//      TheFakturDocFilter.PFD.Dsc_R_rbt2            = Fld_R_rbt2           ;
//      TheFakturDocFilter.PFD.Dsc_R_cij_KCR         = Fld_R_cij_KCR        ;
//      TheFakturDocFilter.PFD.Dsc_R_KCR             = Fld_R_KCR            ;
//      TheFakturDocFilter.PFD.Dsc_T_mrzSt           = Fld_T_mrzSt          ;
//      TheFakturDocFilter.PFD.Dsc_R_mrz             = Fld_R_mrz            ;
//      TheFakturDocFilter.PFD.Dsc_R_cij_KCRM        = Fld_R_cij_KCRM       ;
//      TheFakturDocFilter.PFD.Dsc_R_KCRM            = Fld_R_KCRM           ;
//      TheFakturDocFilter.PFD.Dsc_R_ztr             = Fld_R_ztr            ;
//      TheFakturDocFilter.PFD.Dsc_T_pdvSt           = Fld_T_pdvSt          ;
//      TheFakturDocFilter.PFD.Dsc_R_pdv             = Fld_R_pdv            ;
//      TheFakturDocFilter.PFD.Dsc_R_cij_KCRMP       = Fld_R_cij_KCRP       ;
//      TheFakturDocFilter.PFD.Dsc_R_KCRMP           = Fld_R_KCRP           ;
//      TheFakturDocFilter.PFD.Dsc_T_doCijMal        = Fld_T_doCijMal       ;
//      TheFakturDocFilter.PFD.Dsc_T_noCijMal        = Fld_T_noCijMal       ;
//      TheFakturDocFilter.PFD.Dsc_R_mjMasaN         = Fld_R_mjMasaN        ;
     
//      TheFakturDocFilter.PFD.Dsc_NumDecT_kol       = Fld_NumDecT_kol      ;
//      TheFakturDocFilter.PFD.Dsc_NumDecT_cij       = Fld_NumDecT_cij      ;
//      TheFakturDocFilter.PFD.Dsc_NumDecR_KC        = Fld_NumDecR_KC       ;
//      TheFakturDocFilter.PFD.Dsc_NumDecT_rbt1St    = Fld_NumDecT_rbt1St   ;
//      TheFakturDocFilter.PFD.Dsc_NumDecR_rbt1      = Fld_NumDecR_rbt1     ;
//      TheFakturDocFilter.PFD.Dsc_NumDecT_rbt2St    = Fld_NumDecT_rbt2St   ;
//      TheFakturDocFilter.PFD.Dsc_NumDecR_rbt2      = Fld_NumDecR_rbt2     ;
//      TheFakturDocFilter.PFD.Dsc_NumDecR_cij_KCR   = Fld_NumDecR_cij_KCR  ;
//      TheFakturDocFilter.PFD.Dsc_NumDecR_KCR       = Fld_NumDecR_KCR      ;
//      TheFakturDocFilter.PFD.Dsc_NumDecT_mrzSt     = Fld_NumDecT_mrzSt    ;
//      TheFakturDocFilter.PFD.Dsc_NumDecR_mrz       = Fld_NumDecR_mrz      ;
//      TheFakturDocFilter.PFD.Dsc_NumDecR_cij_KCRM  = Fld_NumDecR_cij_KCRM ;
//      TheFakturDocFilter.PFD.Dsc_NumDecR_KCRM      = Fld_NumDecR_KCRM     ;
//      TheFakturDocFilter.PFD.Dsc_NumDecR_ztr       = Fld_NumDecR_ztr      ;
//      TheFakturDocFilter.PFD.Dsc_NumDecT_pdvSt     = Fld_NumDecT_pdvSt    ;
//      TheFakturDocFilter.PFD.Dsc_NumDecR_pdv       = Fld_NumDecR_pdv      ;
//      TheFakturDocFilter.PFD.Dsc_NumDecR_cij_KCRMP = Fld_NumDecR_cij_KCRP ;
//      TheFakturDocFilter.PFD.Dsc_NumDecR_KCRMP     = Fld_NumDecR_KCRP     ;
//      TheFakturDocFilter.PFD.Dsc_NumDecT_doCijMal  = Fld_NumDecT_doCijMal ;
//      TheFakturDocFilter.PFD.Dsc_NumDecT_noCijMal  = Fld_NumDecT_noCijMal ;

//      TheFakturDocFilter.PFD.Dsc_BelowGrid         = Fld_BelowGrid        ;  
//      TheFakturDocFilter.PFD.Dsc_OcuHeader         = Fld_OcuHeader        ;
//      TheFakturDocFilter.PFD.Dsc_OcuFooter         = Fld_OcuFooter        ;
//      TheFakturDocFilter.PFD.Dsc_OcuLogo           = Fld_OcuLogo          ;
//      TheFakturDocFilter.PFD.Dsc_ScalingPostoLogo  = Fld_ScalingLogo      ;
//      TheFakturDocFilter.PFD.Dsc_OcuIspisPnb       = Fld_OcuIspisPnb      ;

//      TheFakturDocFilter.PFD.Dsc_IsAddTT           = Fld_IsAddTT          ;
//      TheFakturDocFilter.PFD.Dsc_SeparIfTT_Rn      = Fld_SeparIfTT_Rn     ;

//      TheFakturDocFilter.PFD.Dsc_FontOpis          = Fld_FontOpis        ;
//      TheFakturDocFilter.PFD.Dsc_FontColumns       = Fld_FontColumns     ;
//      TheFakturDocFilter.PFD.Dsc_FontBelGr         = Fld_FontBelGr       ;

//      TheFakturDocFilter.PFD.Dsc_OcuR12            = Fld_OcuR12          ;
//      TheFakturDocFilter.PFD.Dsc_JezikReport       = Fld_JezikReporta          ;

//      TheFakturDocFilter.PFD.Dsc_BlgOcuColKonto    = Fld_blgOcuColKonto   ;
//      TheFakturDocFilter.PFD.Dsc_BlgOcuColRacun    = Fld_blgOcuColRacun   ;
//      TheFakturDocFilter.PFD.Dsc_BlgOcu2na1strani  = Fld_blgOcu2na1strani ;
//      TheFakturDocFilter.PFD.Dsc_BlgOcuOkvirUplsp  = Fld_blgOcuOkvirUplsp ;

//      TheFakturDocFilter.PFD.Dsc_OcuKupDobTel           = Fld_OcuKupDobTel          ;       
//      TheFakturDocFilter.PFD.Dsc_OcuProjektTel          = Fld_OcuProjektTel         ;
//      TheFakturDocFilter.PFD.Dsc_OcuKupDobFax           = Fld_OcuKupDobFax          ;
//      TheFakturDocFilter.PFD.Dsc_OcuProjektFax          = Fld_OcuProjektFax         ;
//      TheFakturDocFilter.PFD.Dsc_OcuKupDobOib           = Fld_OcuKupDobOib          ;
//      TheFakturDocFilter.PFD.Dsc_OcuProjektOib          = Fld_OcuProjektOib         ;
//      TheFakturDocFilter.PFD.Dsc_OcuKupDobMail          = Fld_OcuKupDobMail         ;
//      TheFakturDocFilter.PFD.Dsc_OcuProjektMail         = Fld_OcuProjektMail        ;
//      TheFakturDocFilter.PFD.Dsc_OcuKupDobNr            = Fld_OcuKupDobNr           ;
//      TheFakturDocFilter.PFD.Dsc_PotvrdaNarudzbe        = Fld_PotvrdaNarudzbe       ;
//      TheFakturDocFilter.PFD.Dsc_OcuIspisDospjecePl     = Fld_OcuIspisDospjecePl    ;
//      TheFakturDocFilter.PFD.Dsc_OcuIspisVeze1          = Fld_OcuIspisVeze1         ;
//      TheFakturDocFilter.PFD.Dsc_OcuIspisVeze2          = Fld_OcuIspisVeze2         ;
//      TheFakturDocFilter.PFD.Dsc_OcuIspisVeze3          = Fld_OcuIspisVeze3         ;
//      TheFakturDocFilter.PFD.Dsc_OcuIspisVeze4          = Fld_OcuIspisVeze4         ;
//      TheFakturDocFilter.PFD.Dsc_OcuIspisVezDok2        = Fld_OcuIspisVezDok2       ;
//      TheFakturDocFilter.PFD.Dsc_OcuIspisNapomena2      = Fld_OcuIspisNapomena2     ;
//      TheFakturDocFilter.PFD.Dsc_OcuIspisDokNum2        = Fld_OcuIspisDokNum2       ;
//      TheFakturDocFilter.PFD.Dsc_OcuMemoHOnAllPages     = Fld_OcuMemoHOnAllPages    ;
//      TheFakturDocFilter.PFD.Dsc_OcuMemoFOnAllPages     = Fld_OcuMemoFOnAllPages    ;
//      TheFakturDocFilter.PFD.Dsc_LblVeze1               = Fld_LblVeze1              ;
//      TheFakturDocFilter.PFD.Dsc_LblVeze2               = Fld_LblVeze2              ;
//      TheFakturDocFilter.PFD.Dsc_LblVeze3               = Fld_LblVeze3              ;
//      TheFakturDocFilter.PFD.Dsc_LblVeze4               = Fld_LblVeze4              ;
//      TheFakturDocFilter.PFD.Dsc_LblVezDok2             = Fld_LblVezDok2            ;
//      TheFakturDocFilter.PFD.Dsc_LblOsobaX              = Fld_LblOsobaX             ;
//      TheFakturDocFilter.PFD.Dsc_AboveGrid              = Fld_AboveGrid             ;
//      TheFakturDocFilter.PFD.Dsc_AlignmentPageNum       = Fld_AlignmentPageNum      ;
//      TheFakturDocFilter.PFD.Dsc_PrintPageNum           = Fld_PrintPageNum          ;
//      TheFakturDocFilter.PFD.Dsc_AlignmentDokNum        = Fld_AlignmentDokNum       ;
//      TheFakturDocFilter.PFD.Dsc_AdresOkvir             = Fld_AdresOkvir            ;
//      TheFakturDocFilter.PFD.Dsc_AdresOnlyPartner       = Fld_AdresOnlyPartner      ;
//      TheFakturDocFilter.PFD.Dsc_PrintDokNum_BeforAdres = Fld_PrintDokNum_BeforAdres;
//      TheFakturDocFilter.PFD.Dsc_MigPositionHeader      = Fld_MigPositionHeader     ;
//      TheFakturDocFilter.PFD.Dsc_OcuTitleOkvir          = Fld_OcuTitleOkvir         ;
//      TheFakturDocFilter.PFD.Dsc_OcuTitleBoja           = Fld_OcuTitleBoja          ;
//      TheFakturDocFilter.PFD.Dsc_OcuKupDobBoja          = Fld_OcuKupDobBoja         ;
//      TheFakturDocFilter.PFD.Dsc_OcuPrjktBoja           = Fld_OcuPrjktBoja          ;
//      TheFakturDocFilter.PFD.Dsc_OcuLinijeHeader        = Fld_OcuLinijeHeader       ;
//      TheFakturDocFilter.PFD.Dsc_OcuLinijeFooter        = Fld_OcuLinijeFooter       ;
//      TheFakturDocFilter.PFD.Dsc_BeforNRD               = Fld_BeforNRD              ;
//      TheFakturDocFilter.PFD.Dsc_AfterNRD               = Fld_AfterNRD              ;
//      TheFakturDocFilter.PFD.Dsc_OcuIspisVirmana        = Fld_OcuIspisVirmana       ;
//      TheFakturDocFilter.PFD.Dsc_ObrazacA               = Fld_ObrazacA              ;
//      TheFakturDocFilter.PFD.Dsc_ObrazacB               = Fld_ObrazacB              ;
//      TheFakturDocFilter.PFD.Dsc_ObrazacC               = Fld_ObrazacC              ;
//      TheFakturDocFilter.PFD.Dsc_ObrazacD               = Fld_ObrazacD              ;
//      TheFakturDocFilter.PFD.Dsc_OcuKDZiro_Vir          = Fld_OcuKDZiro_Vir         ;
//      TheFakturDocFilter.PFD.Dsc_OcuIspisLblNapomena    = Fld_OcuVezDok             ;

//      TheFakturDocFilter.PFD.Dsc_LeftMargin             = Fld_LeftMargin            ;
//      TheFakturDocFilter.PFD.Dsc_RightMargin            = Fld_RightMargin           ;
//      TheFakturDocFilter.PFD.Dsc_OcuLinijePerson        = Fld_OcuLinijePerson       ;
//      TheFakturDocFilter.PFD.Dsc_OcuZiroFromFak         = Fld_OcuZiroFromFak        ;
//      TheFakturDocFilter.PFD.Dsc_IspisNapomene          = Fld_IspisNapomene         ;
//      TheFakturDocFilter.PFD.Dsc_OcuTextNap2            = Fld_OcuTextNap2           ;
//      TheFakturDocFilter.PFD.Dsc_PositionPersonR        = Fld_PositionPersonR       ;
//      TheFakturDocFilter.PFD.Dsc_OcuFirmuUpotpis        = Fld_OcuFirmuUpotpis       ;
//      TheFakturDocFilter.PFD.Dsc_OcuTitulu              = Fld_OcuTitulu             ;
//      TheFakturDocFilter.PFD.Dsc_OcuJednakiFtTxt2Red    = Fld_OcuJednakiFtTxt2Red   ;


//      TheFakturDocFilter.PFD.SaveDscToLookUpItemList();

//      TheFakturDocFilter.DevNameAsEnum              = Fld_DevNameAsEnum;

//   }

//   #endregion Put & GetFilterFields

//   #region AddFilterMemberz()

//   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
//   {
//   }

//   #endregion AddFilterMemberz()

//   #region ChooseDscLuiList

//   private void ChooseDscLuiList(object sender, EventArgs e)
//   {
//      RadioButton rb = sender as RadioButton;

//      (TheVvUC as FakturDUC).LoadDscLuiList_And_PutFilterFields(Fld_ChoseObrazac);

//      foreach(Control rbt in hamp_ChoseIra.Controls)
//      {
//         if(rbt is RadioButton)
//         {
//            if(rbt == rb) rbt.Tag = true;
//            else          rbt.Tag = false;
//         }
//      }

//      this.btn_GO.PerformClick();

//   }

//   #endregion ChooseDscLuiList

//}

#endregion FakturDocFilterUC

#region RiskMacroNewDlg

public class RiskMacroNewDlg : VvDialog
   {
   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private CheckBox  cbx_zapamtiDatume;
   private VvTextBox tbx_macroName;
   DateTime dateOD, dateDO;

   public RiskMacroNewDlg(DateTime _dateOD, DateTime _dateDO)
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "RiskMacro";

      this.dateOD = _dateOD;
      this.dateDO = _dateDO;

      CreateHamper();

      dlgWidth  = hamper.Right + ZXC.QUN; 
      dlgHeight = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
   }

   private void CreateHamper()
   {
      hamper          = new VvHamper(2, 2, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q4un , ZXC.Q10un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 , ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel  (0, 0, "Macro naziv:", ContentAlignment.MiddleRight);
      tbx_macroName = hamper.CreateVvTextBox(1, 0, "tbx_macroName", "");

      cbx_zapamtiDatume = hamper.CreateVvCheckBox_OLD(1, 1, null, "Zapamti i datume OD / DO", RightToLeft.No);
      cbx_zapamtiDatume.Font = ZXC.vvFont.BaseFont;
      cbx_zapamtiDatume.CheckedChanged += new EventHandler(cbx_zapamtiDatume_CheckedChanged);
   }

   void cbx_zapamtiDatume_CheckedChanged(object sender, EventArgs e)
   {
      CheckBox cb = sender as CheckBox;

      string dates = " (" + dateOD.ToString(ZXC.VvDateDdMmFormat) + "-" + dateDO.ToString(ZXC.VvDateDdMmFormat) + ")";

      if(cb.Checked) tbx_macroName.Text += dates;
      else           tbx_macroName.Text = tbx_macroName.Text.Replace(dates, "");
   }

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   public bool   Fld_ZapamtiDatume { get { return cbx_zapamtiDatume.Checked; } set { cbx_zapamtiDatume.Checked = value; } }
   public string Fld_MacroName     { get { return tbx_macroName    .Text   ; } set { tbx_macroName    .Text    = value; } }

}

#endregion RiskMacroNewDlg

#region RiskMacroRenameDlg

public class RiskMacroRenameDlg : VvDialog
{
   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_macroName;

   public RiskMacroRenameDlg()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "RiskMacroIspravi";

      CreateHamper();

      dlgWidth  = hamper.Right + ZXC.QUN; 
      dlgHeight = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
   }

   private void CreateHamper()
   {
      hamper          = new VvHamper(2, 2, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q4un , ZXC.Q10un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 , ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel  (0, 0, "Novi Macro naziv:", ContentAlignment.MiddleRight);
      tbx_macroName = hamper.CreateVvTextBox(1, 0, "tbx_macroName", "");

   }

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   public string Fld_NewMacroName     { get { return tbx_macroName    .Text   ; } set { tbx_macroName    .Text    = value; } }

}

#endregion RiskMacroRenameDlg
