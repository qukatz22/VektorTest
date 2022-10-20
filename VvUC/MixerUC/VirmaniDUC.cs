using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public class VirmanDUC       : MixerDUC
{
   #region Constructor

   public VirmanDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)

   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_VIRMAN
         });

      Kupdob.FillLookUpItemZiroList(ZXC.CURR_prjkt_rec);
   }

   #endregion Constructor

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {

      /* 10 */      T_kpdbNameA_50_CreateColumn  (ZXC.Q7un, "PlatiteljNaziv" , "Platitelj Naziv", true);
      /* 11 */      T_kpdbUlBrA_32_CreateColumn  (ZXC.Q6un, "PlatiteljAdresa", "Platitelj Adresa", 0);
      /* 12 */      T_kpdbMjestoA_32_CreateColumn(ZXC.Q4un, "PlatiteljMjesto", "Platitelj Mjesto");
      /* 13 */      T_kpdbZiroA_32_CreateColumn  (ZXC.Q7un, "PlatiteljŽiro"  , "Platitelj Žiro-račun", ZXC.luiListaRiskZiroRn);
      /* 16 */      T_strA_2_CreateColumn        (ZXC.Q2un, "ModPlat"        , "Model Poziva na broj Zaduženja - Platitelja", null);
      /* 18 */      T_vezniDokA_64_CreateColumn  (ZXC.Q2un, "PZ"             , "Poziv na broj Zaduženja - Platitelja", null);
      /* 10 */      T_kpdbNameB_50_CreateColumn  (ZXC.Q7un, "PrimateljNaziv" , "Primatelj Naziv");
      /* 11 */      T_kpdbUlBrB_32_CreateColumn  (ZXC.Q6un, "PrimateljAdresa", "Primatelj Adresa");
      /* 12 */      T_kpdbMjestoB_32_CreateColumn(ZXC.Q4un, "PrimateljMjesto", "Primatelj Mjesto");
      /* 13 */      T_kpdbZiroB_32_CreateColumn  (ZXC.Q7un, "PrimateljŽiro"  , "Primatelj Žiro-račun", null);
      /* 17 */      T_strB_2_CreateColumn        (ZXC.Q2un, "ModPrim"        , "Model Poziva na broj Odobrenja - Primatelja");
      /* 19 */      T_vezniDokB_64_CreateColumn  (ZXC.Q7un, "PnbOdob"        , "Poziva na broj Odobrenja - Primatelja");
      /* 07 */      T_moneyA_CreateColumn        (ZXC.Q5un, "Iznos"          , "Iznos", 2);
      /* 08 */      T_opis_128_CreateColumn      (ZXC.Q7un, "Opis Plaćanja"  , "Opis plaćanja", 0, null);
      /* 15 */      T_dateDo_CreateColumn        (ZXC.Q4un, "DatPodnošenja");
      /* 14 */      T_dateOd_CreateColumn        (ZXC.Q4un, "DatValute"    );
      /* 20 */      T_strC_2_CreateColumn        (ZXC.QUN + ZXC.Qun4, "ŠifPl", "Šifra opisa plaćanja");

   }

   public override void PutDgvTransSumFields1()
   {
      TheSumGrid.PutCell(ci.iT_moneyA, 0, mixer_rec.Sum_Money1);
   }

   #endregion InitializeTheGrid_Columns()

   #region PrintDocumentRecord

   public VirmanFilterUC  TheVirmanZnpFilterUC { get; set; }
   public VirmanDocFilter TheVirmanZnpDocFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.TheVirmanZnpDocFilter = new VirmanDocFilter(this);

      TheVirmanZnpFilterUC = new VirmanFilterUC(this);
      TheVirmanZnpFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheVirmanZnpFilterUC.Width;
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      VirmanDocFilter mixerFilter = (VirmanDocFilter)vvRptFilter;

      switch(mixerFilter.PrintVirZnp)
      {
         //case VirmanDocFilter.PrintVirZnpEnum.VirmOkvir  : specificMixerReport = new RptX_Virman(new Vektor.Reports.XIZ.CR_Xvirmani()      , reportName, mixerFilter); break;
         case VirmanDocFilter.PrintVirZnpEnum.SEPA         : specificMixerReport = new RptX_Virman(new Vektor.Reports.XIZ.CR_XVirmaniSEPA()  , reportName, mixerFilter); break;
         //case VirmanDocFilter.PrintVirZnpEnum.VirmNeOkvir: specificMixerReport = new RptX_Virman(new Vektor.Reports.XIZ.CR_XvirmanNoOkvir(), reportName, mixerFilter); break;
         case VirmanDocFilter.PrintVirZnpEnum.VirmNeOkvir  : specificMixerReport = new RptX_Virman(new Vektor.Reports.XIZ.CR_XVirmHUB3A()    , reportName, mixerFilter); break;
         case VirmanDocFilter.PrintVirZnpEnum.Znp          : specificMixerReport = new RptX_Virman(new Vektor.Reports.XIZ.CR_XvirmaniZNP()   , reportName, mixerFilter); break;

         default: ZXC.aim_emsg("{0}\nPrintSomeVirmanDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintVirZnp); return null;
      }

      return specificMixerReport;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheVirmanZnpDocFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheVirmanZnpFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
   }

   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Virm, Color.Empty, clr_Virm);
   }

   #endregion Colors

   #region override void PutDefaultBabyDUCfields()
  
   /*protected*/public override void PutSpecificsFld()
   {
      Fld_TtOpis = ZXC.luiListaMixerType.GetNameForThisCd(mixer_rec.TT);
   }
   protected override void PutDefaultBabyDUCfields()
   {
      #region Init First Grid Row (first virman)

      TheG.Rows.Add();

      PutPlatData(0, ZXC.CURR_prjkt_rec);
      PutVirmanDates(0, DateTime.Now);

      #endregion Init First Grid Row (first virman)

   }

   public void PutPlatData(int rowIdx, Kupdob kupdob_rec)
   {
      TheG.PutCell(ci.iT_kpdbNameA_50  , rowIdx, kupdob_rec.Naziv);
      TheG.PutCell(ci.iT_kpdbUlBrA_32  , rowIdx, kupdob_rec.Ulica1);
      TheG.PutCell(ci.iT_kpdbMjestoA_32, rowIdx, kupdob_rec.Grad);
      TheG.PutCell(ci.iT_kpdbZiroA_32  , rowIdx, ZXC.GetIBANfromOldZiro(kupdob_rec.Ziro1));
   }

   public void PutPrimData(int rowIdx, Kupdob kupdob_rec)
   {
      TheG.PutCell(ci.iT_kpdbNameB_50  , rowIdx, kupdob_rec.Naziv);
      TheG.PutCell(ci.iT_kpdbUlBrB_32  , rowIdx, kupdob_rec.Ulica1);
      TheG.PutCell(ci.iT_kpdbMjestoB_32, rowIdx, kupdob_rec.Grad);
      TheG.PutCell(ci.iT_kpdbZiroB_32  , rowIdx, ZXC.GetIBANfromOldZiro(kupdob_rec.Ziro1));
   }

   public void PutVirmanDates(int rowIdx, DateTime date)
   {
      TheG.PutCell(ci.iT_dateOd, rowIdx, date);
      TheG.PutCell(ci.iT_dateDo, rowIdx, date);
   }

   #endregion override void PutDefaultBabyDUCfields()

}

public class VirmanFilterUC  : VvFilterUC
{
   #region Fieldz

   private VvHamper    hamp_virDate, hamp_rbt;
   private VvTextBox   tbx_pomak, tbx_showVirDate;
   private VvCheckBox  cbx_showVirDatVal;
   private RadioButton rbt_virO, rbt_virNo, rbt_znp;

   #endregion Fieldz

   #region  Constructor

   public VirmanFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;
      
      CreateHampers();

      CreateHamper_4ButtonsResetGo_Width(this.MaxHamperWidth);

      hamp_virDate.Location = new Point(nextX, hamper4buttons.Bottom + ZXC.Qun4);
      hamp_rbt.Location     = new Point(nextX, hamp_virDate  .Bottom + ZXC.Qun4);
      hamperHorLine.Visible = false;

      nextY = hamp_rbt.Bottom + razmakIzmjedjuHampera;

      this.Width  = hamp_virDate.Width + ZXC.QUN;
      this.Height = hamp_rbt.Bottom + ZXC.QUN;
      
      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region Hampers

   private void CreateHampers()
   {
      InitializeHamper_Rbt(out hamp_rbt);
      this.MaxHamperWidth = hamp_rbt.Width;
    
      InitializeHamper_VirmanDate(out hamp_virDate);
   }

   private void InitializeHamper_VirmanDate(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.QUN - ZXC.Qun4, ZXC.Qun4, ZXC.Q2un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,           ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;


                        hamper.CreateVvLabel  (0, 0, "Printaj datume", ContentAlignment.MiddleRight);
      tbx_showVirDate = hamper.CreateVvTextBox(1, 0, "tbx_showVirDat", "");
      tbx_showVirDate.JAM_Highlighted = true;
      cbx_showVirDatVal = hamper.CreateVvCheckBox(1, 0, "", tbx_showVirDate, "", "X");
      cbx_showVirDatVal.Checked = true;

                  hamper.CreateVvLabel  (0, 1, "Pomak printa virmana", ContentAlignment.MiddleRight);
      tbx_pomak = hamper.CreateVvTextBox(1, 1, "tbx_pomak", "Pomak printanja virmana 2 - 2mm, 3 - 3mm, 4 - 4mm, 5 - 5mm", 1, 1, 0);
      tbx_pomak.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_pomak.JAM_AllowedInputCharacters = "2345";
      tbx_pomak.TextAlign = HorizontalAlignment.Center;
      
      SetUpAsWriteOnlyTbx(hamper);

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, this.MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);  
   }

   private void InitializeHamper_Rbt(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 4, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q10un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = ZXC.Qun4 + ZXC.Qun10;
     
                 hamper.CreateVvLabel       (0, 0,       "Vrsta ispisa"             , ContentAlignment.MiddleLeft);
      rbt_virO  = hamper.CreateVvRadioButton(0, 1, null, "SEPA pain.001"            , TextImageRelation.ImageBeforeText);
      rbt_virNo = hamper.CreateVvRadioButton(0, 2, null, "Virmani bez pozadine"     , TextImageRelation.ImageBeforeText);
      rbt_znp   = hamper.CreateVvRadioButton(0, 3, null, "Zbrojni nalog za prijenos", TextImageRelation.ImageBeforeText);
      rbt_virO.Checked = true;
      rbt_virO.Tag     = true;

      VvHamper.HamperStyling(hamper);
   }

   #endregion Hampers

   #region Fld_

   public VirmanDocFilter.PrintVirZnpEnum Fld_PrintVirZnp
   {
      get
      {
         if     (rbt_virO .Checked) return VirmanDocFilter.PrintVirZnpEnum.SEPA;
         if     (rbt_virNo.Checked) return VirmanDocFilter.PrintVirZnpEnum.VirmNeOkvir;
         else if(rbt_znp  .Checked) return VirmanDocFilter.PrintVirZnpEnum.Znp    ;

         else throw new Exception("Fld_PrintVirZnp: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case VirmanDocFilter.PrintVirZnpEnum.SEPA  : rbt_virO .Checked = true; break;
            case VirmanDocFilter.PrintVirZnpEnum.VirmNeOkvir: rbt_virNo.Checked = true; break;
            case VirmanDocFilter.PrintVirZnpEnum.Znp        : rbt_znp  .Checked = true; break;
         }
      }
   }
   
   public int Fld_Pomak
   {
      get { return ZXC.ValOrZero_Int(tbx_pomak.Text); }
      set {                          tbx_pomak.Text = value.ToString(); }
   }
  
   public bool Fld_ShowVirDat
   {
      get { return cbx_showVirDatVal.Checked; }
      set {        cbx_showVirDatVal.Checked = value; }
   }


   #endregion Fld_

   #region Put & GetFilterFields

   private VirmanDocFilter TheVirmanZnpDocFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as VirmanDocFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheVirmanZnpDocFilter = (VirmanDocFilter)_filter_data;

      if(TheVirmanZnpDocFilter != null)
      {
         Fld_PrintVirZnp = TheVirmanZnpDocFilter.PrintVirZnp;
         Fld_Pomak       = TheVirmanZnpDocFilter.VirPomak;
         Fld_ShowVirDat  = TheVirmanZnpDocFilter.ShowVirDate;
      }

      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
      TheVirmanZnpDocFilter.PrintVirZnp = Fld_PrintVirZnp;
      TheVirmanZnpDocFilter.VirPomak    = Fld_Pomak;
      TheVirmanZnpDocFilter.ShowVirDate = Fld_ShowVirDat;
   }

   #endregion Put & GetFilterFields

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
      VirmanDocFilter theRptFilter = (VirmanDocFilter)_vvRptFilter;

      int numv = theRptFilter.VirPomak;
      if(numv.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember("VirPomak", numv));
      }

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember("ShowDate", theRptFilter.ShowVirDate));
   }

   #endregion AddFilterMemberz()

}

public class VirmanDocFilter : VvRpt_Mix_Filter
{

   public enum PrintVirZnpEnum
   {
    //VirmOkvir, VirmNeOkvir, Znp
      SEPA, VirmNeOkvir, Znp
   }

   public PrintVirZnpEnum PrintVirZnp { get; set; }

   public VirmanDUC theDUC;
  
   public int    VirPomak    { get; set; }
   public bool   ShowVirDate { get; set; }

   public VirmanDocFilter(VirmanDUC _theDUC)
   {
      this.theDUC = _theDUC;
      SetDefaultFilterValues();
   }

   #region SetDefaultFilterValues()

   public override void SetDefaultFilterValues()
   {
      ZXC.VvDataBaseInfo vvDBinfo = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects;
      int projectYear = int.Parse(vvDBinfo.ProjectYear);
      int thisYear = DateTime.Now.Year;
   
      PrintVirZnp = PrintVirZnpEnum.SEPA;
   }

   #endregion SetDefaultFilterValues()

}

public class ProjektPayDlg : VvDialog
{
   #region Propertiez

   private Crownwood.DotNetMagic.Forms.DotNetMagicForm ThePreviewProjektPayForm { get; set; }

   #endregion Propertiez

   #region Fieldz

   private Button okButton, cancelButton;
   private int    dlgWidth, dlgHeight;

   public  ProjektPayUC TheUC { get; private set; }
  
   #endregion Fieldz

   #region Constructor

   public ProjektPayDlg()
   {

      TheUC = new ProjektPayUC();

      SuspendLayout();

      this.Font        = ZXC.vvFont.BaseFont;
      this.Style       = ZXC.vvColors.vvform_VisualStyle;
      this.BackColor   = ZXC.vvColors.userControl_BackColor;

      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Projekt placanja";

      CreateTheUC();

      dlgWidth        = TheUC.Width;
      dlgHeight       = TheUC.Height + ZXC.QunMrgn * 2 + ZXC.QunBtnH;


      this.ClientSize = new Size(dlgWidth, dlgHeight);
      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
     
      ResumeLayout();
   }

   #endregion Constructor

   #region UC

   private void CreateTheUC()
   {
      TheUC.Parent   = this;
      TheUC.Location = new Point(0, 0);
   }

   #endregion UC
}

public class ProjektPayUC  : VvOtherUC
{
   #region Fieldz

   private VvHamper     hamp_davanja;
   private RadioButton  rbt_ToPayA  , rbt_ToPayB  , rbt_ToPayC  , 
                        rbt_PidSume , rbt_PidSRent, rbt_PidTurst, rbt_PidDobit,
                        rbt_PidKmDopr, rbt_PidKmClan, rbt_PidMO1  , rbt_PidMO2,
                        rbt_PidZdr  , rbt_PidZor;
   private VvTextBox    tbx_razdoblje;

   #endregion Fieldz

   #region Constructor

   public ProjektPayUC()
   {
      SuspendLayout();

      CreateHampers();

      this.Size = new Size(hamp_davanja.Right + ZXC.Q2un, hamp_davanja.Bottom);

      ResumeLayout();
   }

   #endregion Constructor

   #region Hampers

   private void CreateHampers()
   {
      InitializeHamper_Davanja (out hamp_davanja);
   }


   private void InitializeHamper_Davanja(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 14, "", this, false);

      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);

      hamper.VvColWdt      = new int[] { ZXC.Q10un, ZXC.Q2un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      //rbt_ToPayA   = hamper.CreateVvRadioButton(0,  0, null, "Fakturiranje"          , TextImageRelation.ImageBeforeText);
      //rbt_ToPayB   = hamper.CreateVvRadioButton(0,  1, null, "Poseban Porez"         , TextImageRelation.ImageBeforeText);
      //rbt_ToPayC   = hamper.CreateVvRadioButton(0,  2, null, "ToPayC   "             , TextImageRelation.ImageBeforeText);
      rbt_PidDobit  = hamper.CreateVvRadioButton(0, 0, null, "Porez na Dobit/Dohodak"   , TextImageRelation.ImageBeforeText);
    //rbt_PidKmDopr = hamper.CreateVvRadioButton(0, 1, null, "Komorske Doprinos"        , TextImageRelation.ImageBeforeText);
      rbt_PidKmDopr = hamper.CreateVvRadioButton(0, 1, null, "Doprinos za Zapošljavanje", TextImageRelation.ImageBeforeText);
      rbt_PidKmClan = hamper.CreateVvRadioButton(0, 2, null, "Komorski Članarine"       , TextImageRelation.ImageBeforeText);
      rbt_PidMO1    = hamper.CreateVvRadioButton(0, 3, null, "Doprinos za MIO1"         , TextImageRelation.ImageBeforeText);
      rbt_PidMO2    = hamper.CreateVvRadioButton(0, 4, null, "Doprinos za MIO2"         , TextImageRelation.ImageBeforeText);
      rbt_PidZdr    = hamper.CreateVvRadioButton(0, 5, null, "Doprinos za ZdravOsig"    , TextImageRelation.ImageBeforeText);
      rbt_PidZor    = hamper.CreateVvRadioButton(0, 6, null, "Doprinos ZOR"             , TextImageRelation.ImageBeforeText);
      rbt_PidSRent  = hamper.CreateVvRadioButton(0, 7, null, "Spomenička Renta"         , TextImageRelation.ImageBeforeText);
      rbt_PidTurst  = hamper.CreateVvRadioButton(0, 8, null, "Turstička članarina "     , TextImageRelation.ImageBeforeText);
      rbt_PidSume   = hamper.CreateVvRadioButton(0, 9, null, "Doprinos za Šume"         , TextImageRelation.ImageBeforeText);
      rbt_PidDobit.Checked = true;

                      hamper.CreateVvLabel  (0, 13, "Razdoblje u Pozivu na broj Odobrenja", ContentAlignment.MiddleRight);
      tbx_razdoblje = hamper.CreateVvTextBox(1, 13, "tbx_razdoblje", "Razdoblje", 4);
      tbx_razdoblje.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvOtherUC);
   }

   #endregion Hampers

   #region Fld_

   public ZXC.ProjektPaySetEnum Fld_ProjektPaySet
   {
      get
      {
         //     if(rbt_ToPayA   .Checked) return ZXC.ProjektPaySetEnum.ToPayA  ;
         //else if(rbt_ToPayB   .Checked) return ZXC.ProjektPaySetEnum.ToPayB  ;
         //else if(rbt_ToPayC   .Checked) return ZXC.ProjektPaySetEnum.ToPayC  ;
              if(rbt_PidSume  .Checked) return ZXC.ProjektPaySetEnum.PidSume ;
         else if(rbt_PidSRent .Checked) return ZXC.ProjektPaySetEnum.PidSRent;
         else if(rbt_PidTurst .Checked) return ZXC.ProjektPaySetEnum.PidTurst;
         else if(rbt_PidDobit .Checked) return ZXC.ProjektPaySetEnum.PidDobit;
         else if(rbt_PidKmDopr.Checked) return ZXC.ProjektPaySetEnum.PidKmDopr;
         else if(rbt_PidKmClan.Checked) return ZXC.ProjektPaySetEnum.PidKmClan;
         else if(rbt_PidMO1   .Checked) return ZXC.ProjektPaySetEnum.PidMO1  ;
         else if(rbt_PidMO2   .Checked) return ZXC.ProjektPaySetEnum.PidMO2  ;
         else if(rbt_PidZdr   .Checked) return ZXC.ProjektPaySetEnum.PidZdr  ;
         else if(rbt_PidZor   .Checked) return ZXC.ProjektPaySetEnum.PidZor  ;

         else throw new Exception("Fld_ProjektPayTime_Ulaz: who df is checked?");
      }
      set
      {
         switch(value)
         {
            //case ZXC.ProjektPaySetEnum.ToPayA     : rbt_ToPayA  .Checked  = true; break;
            //case ZXC.ProjektPaySetEnum.ToPayB     : rbt_ToPayB  .Checked  = true; break;
            //case ZXC.ProjektPaySetEnum.ToPayC     : rbt_ToPayC  .Checked  = true; break;
            case ZXC.ProjektPaySetEnum.PidSume    : rbt_PidSume .Checked  = true; break;
            case ZXC.ProjektPaySetEnum.PidSRent   : rbt_PidSRent.Checked  = true; break;
            case ZXC.ProjektPaySetEnum.PidTurst   : rbt_PidTurst.Checked  = true; break;
            case ZXC.ProjektPaySetEnum.PidDobit   : rbt_PidDobit.Checked  = true; break;
            case ZXC.ProjektPaySetEnum.PidKmDopr  : rbt_PidKmDopr.Checked = true; break;
            case ZXC.ProjektPaySetEnum.PidKmClan  : rbt_PidKmClan.Checked = true; break;
            case ZXC.ProjektPaySetEnum.PidMO1     : rbt_PidMO1  .Checked  = true; break;
            case ZXC.ProjektPaySetEnum.PidMO2     : rbt_PidMO2  .Checked  = true; break;
            case ZXC.ProjektPaySetEnum.PidZdr     : rbt_PidZdr  .Checked  = true; break;
            case ZXC.ProjektPaySetEnum.PidZor     : rbt_PidZor  .Checked  = true; break;
         }
      }
   }

   public string Fld_Razdoblje
   {
      get { return tbx_razdoblje.Text; }
      set {        tbx_razdoblje.Text = value; }
   }

   #endregion Fld_
}
