using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public partial class PlanDUC : MixerDUC
{
   #region Fieldz

   private VvHamper  hamp_upute1, hamp_upute2;
   private Color     clB_prGo, clF_prGo, clB_plan, clF_plan, clB_rbls, clF_rbls, clB_najv, clF_najv;   

   #endregion Fieldz

   #region Constructor

   public PlanDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_PLN, Mixer.TT_RBL, Mixer.TT_NJV, Mixer.TT_PBN
         });
   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
      hamp_mjTroska.Visible = false;

      InitializeHamper_Upute1(out hamp_upute1);
      InitializeHamper_Upute2(out hamp_upute2);

      hamp_napomena.Location = new Point(nextX, hamp_dokNum.Bottom);
      nextY = hamp_upute2.Bottom;
   }

   private void InitializeHamper_Upute1(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 7, "", TheTabControl.TabPages[0], false, hamp_tt.Right + ZXC.Qun2, ZXC.Qun4, razmakHamp);
      //                                     0   
      hamper.VvColWdt      = new int[] { ZXC.QUN-ZXC.Qun8 , ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8         ,       0 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN - ZXC.Qun4;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }      
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbld = hamper.CreateVvLabel(0, 0, "d", ContentAlignment.MiddleLeft);
      Label lblp = hamper.CreateVvLabel(0, 1, "p", ContentAlignment.MiddleLeft);
      Label lblD = hamper.CreateVvLabel(0, 2, "D", ContentAlignment.MiddleLeft);
      Label lblP = hamper.CreateVvLabel(0, 3, "P", ContentAlignment.MiddleLeft);
      Label lblS = hamper.CreateVvLabel(0, 4, "S", ContentAlignment.MiddleLeft);

      lblD.Font = ZXC.vvFont.SmallBoldFont;
      lblP.Font = ZXC.vvFont.SmallBoldFont;
      lblS.Font = ZXC.vvFont.SmallBoldFont;
      lbld.Font = ZXC.vvFont.SmallBoldFont;
      lblp.Font = ZXC.vvFont.SmallBoldFont;

      hamper.CreateVvLabel(1, 0, "- prom Dug", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(1, 1, "- prom Pot", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(1, 2, "- kum Dug" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(1, 3, "- kum Pot" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(1, 4, "- saldo"   , ContentAlignment.MiddleLeft);

      hamper.BackColor = Color.MintCream;
   }
   private void InitializeHamper_Upute2(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 7, "", TheTabControl.TabPages[0], false, hamp_upute1.Right + ZXC.Qun8, ZXC.Qun4, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q10un*2  + ZXC.QUN};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN - ZXC.Qun4;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "'' - Bez oznake slova podrazumijeva saldo   /  Pravila se razdvajaju zarezom ',' "                             , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(0, 1, "Upotreba 'do' /  Upotreba minusa '-'   /    Korijen konta", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(0, 2, "030d --> promDug konta korijena 030"                                               , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(0, 3, "460D, 462D   --> kumDug konta korijena 460 + kumDug konta korijena 462"            , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(0, 4, "460D do 462D --> kumDug kta kor. 460 + kumDug kta kor. 461 + kumDug kta kor. 462"  , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(0, 5, "031, - 039S --> od saldoDug konta korijena 031 oduzmi saldoPot konta korijena 039", ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(0, 6, "42D, -42001D --> od kumDug konta korijena 42 oduzmni kumDug konta 42001"           , ContentAlignment.MiddleLeft);

      hamper.BackColor = Color.MintCream;
   }

   #endregion CreateSpecificHampers()

   #region Fld_

   #endregion Fld_

   #region override PutSpecificsFld() GetSpecificsFld()

   /*protected*/public override void PutSpecificsFld()
   {
      Fld_TtOpis   = ZXC.luiLista_PlanTT.GetNameForThisCd(mixer_rec.TT);
   }

   protected override void GetSpecificsFld()
   {
   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      SetColors();

      T_kpdbZiroA_32_CreateColumn  (ZXC.Q4un             , "Pozicija"         , "Pozicija plana"       , ZXC.luiListaPozicijePlanaPLN); // dodati lookuplistu
      T_opis_128_CreateColumn      (0                    , "Naziv pozicije"   , "Naziv pozicije plana" , 128, null);
      T_moneyA_CreateColumn        (ZXC.Q5un             , "Iznos"            , "Iznos ", 2);


      if(kshema.Dsc_IsVisibleColFond && this.Fld_TT != Mixer.TT_PBN)
      {

         string tekGodPlus1 = (int.Parse(ZXC.projectYear) + 1).ToString();
         string tekGodPlus2 = (int.Parse(ZXC.projectYear) + 2).ToString();

         T_moneyB_CreateColumn  (ZXC.Q5un, 2,   "Iznos B", "Iznos prema izvoru financiranja B", false       );
         T_moneyC_CreateColumn  (ZXC.Q5un, 2,   "Iznos C", "Iznos prema izvoru financiranja C", false, false);
         T_moneyD_CreateColumn  (ZXC.Q5un, 2,   "Iznos D", "Iznos prema izvoru financiranja D"              );
         T_dec01_CreateColumn   (ZXC.Q5un,      "Iznos E", "Iznos prema izvoru financiranja E", 2           );
         T_dec02_CreateColumn   (ZXC.Q5un,      "Iznos F", "Iznos prema izvoru financiranja E", 2           );
         T_dec03_CreateColumn   (ZXC.Q5un,      "Iznos G", "Iznos prema izvoru financiranja E", 2           );
         R_SumMoney_CreateColumn(ZXC.Q5un,       "Ukupno", "Ukupno", 2);
         //T_dec04_CreateColumn   (ZXC.Q5un,    tekGodPlus1, "Projekcija " + tekGodPlus1 + " godine", 2);
         //T_dec05_CreateColumn   (ZXC.Q5un,    tekGodPlus2, "Projekcija " + tekGodPlus2 + " godine", 2);


      }

      T_kpdbNameA_50_CreateColumn(ZXC.Q10un + ZXC.QUN, "Formula / Pravilo", "Formula/Pravilo za obračun retka", false);

      T_kupdobCD_CreateColumn(ZXC.Q3un, "ŠifraMT", "Šifra mjesta troška");
      T_kpdbMjestoA_32_CreateColumn(ZXC.Q3un, "Ticker", "Ticker mjesta troška");

      vvtbT_kpdbZiroA_32.JAM_lui_NameTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_opis_128);
      
      vvtbT_kupdobCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      vvtbT_kpdbMjestoA_32.JAM_CharacterCasing = CharacterCasing.Upper;

      vvtbT_kupdobCD      .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyMtrosTextBoxLeave_Grid));
      vvtbT_kpdbMjestoA_32.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave_Grid));
    
       
   }

   private void SetColors()
   {
     clB_prGo = Color.AntiqueWhite ;
     clF_prGo = Color.Black        ;
     clB_plan = Color.LavenderBlush;
     clF_plan = Color.DarkBlue     ;
     clB_rbls = Color.Lavender     ;
     clF_rbls = Color.Black        ;
     clB_najv = Color.LightCyan    ;
     clF_najv = Color.Black        ;
   }

   #endregion InitializeTheGrid_Columns()

   #region PrintDocumentRecord

   public PlanFilterUC  The_PlanFilterUC { get; set; }
   public PlanDocFilter The_PlanDocFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.The_PlanDocFilter = new PlanDocFilter(this);

      The_PlanFilterUC                         = new PlanFilterUC(this);
      The_PlanFilterUC.Parent                  = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = The_PlanFilterUC.Width;
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      PlanDocFilter mixerFilter = (PlanDocFilter)vvRptFilter;

      //bool isPlanByFond  = kshema.Dsc_IsVisibleColFond;
      //bool isPlanByMtros = kshema.Dsc_IsPlanViaMtros  ;
      //bool isPlanClassic = isPlanByFond == false && isPlanByMtros == false;
      
      //Vektor.Reports.XIZ.CR_PrintPlana     cr_PlanClassic = /*IsPlanByFond ? null :*/ new Vektor.Reports.XIZ.CR_PrintPlana();
      //Vektor.Reports.XIZ.CR_PlanViaFondovi cr_PlanByFond  = /*IsPlanByFond ?*/        new Vektor.Reports.XIZ.CR_PlanViaFondovi();

      //CrystalDecisions.CrystalReports.Engine.ReportDocument repDoc = isPlanByFond  ?  (CrystalDecisions.CrystalReports.Engine.ReportDocument)cr_PlanByFond  :
      //                                                               isPlanByMtros ?  (CrystalDecisions.CrystalReports.Engine.ReportDocument)cr_PlanClassic :
      //                                                                                (CrystalDecisions.CrystalReports.Engine.ReportDocument)cr_PlanClassic ;

      //     if(this.Fld_TT == Mixer.TT_NJV)       mixerFilter.PrintPlan = PlanDocFilter.PrintPlanEnum.Najava    ; //The_PlanFilterUC.rbt_najave  .Checked = true;
      //else if(this.Fld_TT == Mixer.TT_PBN)       mixerFilter.PrintPlan = PlanDocFilter.PrintPlanEnum.BagatelNbv;//The_PlanFilterUC.rbt_bagNbv  .Checked = true;
      //else if(this.kshema.Dsc_IsVisibleColFond)  mixerFilter.PrintPlan = PlanDocFilter.PrintPlanEnum.PrihodPrim;//The_PlanFilterUC.rbt_prihPrim.Checked = true;
      //     else                                  mixerFilter.PrintPlan = PlanDocFilter.PrintPlanEnum.Plan      ;//The_PlanFilterUC.rbt_plan    .Checked = true;
           

      switch(mixerFilter.PrintPlan)
      {
         //case PlanDocFilter.PrintPlanEnum.Plan      : specificMixerReport = new RptX_Plan(repDoc                                , reportName, mixerFilter); break;
         //case PlanDocFilter.PrintPlanEnum.PrihodPrim: specificMixerReport = new RptX_Plan(repDoc                                , reportName, mixerFilter); break;
         //case PlanDocFilter.PrintPlanEnum.RashodIzdt: specificMixerReport = new RptX_Plan(repDoc                                , reportName, mixerFilter); break;
         //case PlanDocFilter.PrintPlanEnum.Najava    : specificMixerReport = new RptX_Plan(repDoc                                , reportName, mixerFilter); break;
         //case PlanDocFilter.PrintPlanEnum.BagatelNbv: specificMixerReport = new RptX_Plan(new Vektor.Reports.XIZ.CR_PlanNabave(), reportName, mixerFilter); break;

         default: ZXC.aim_emsg("{0}\nRasterBDocDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintPlan); return null;
      }

      return specificMixerReport;

      //return new RptXPlan(new Vektor.Reports.XIZ.CR_KontrolPravilaGFI_TSI(), reportName, mixerFilter);
      //return new RptXPlan(new Vektor.Reports.XIZ.CR_PravilaPlan(), reportName, mixerFilter);
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.The_PlanDocFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.The_PlanFilterUC;
      }
   }

   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Plan, Color.Empty, clr_Raster);
   }

   #endregion Colors

   #region override void PutDefaultBabyDUCfields()

   protected override void PutDefaultBabyDUCfields()
   {
   }

   #endregion override void PutDefaultBabyDUCfields()

   public void Load_Plan_FromLookUpList(VvLookUpLista theLookUpList)
   {
      Mixer  mixer_rec  = new Mixer ();
      Xtrans xtrans_rec = new Xtrans();

      ushort line = 0;

      switch(theLookUpList.Title)
      {                                                               
         case ZXC.luiListaPozicijePlanaPLN_Name : mixer_rec.TT = Mixer.TT_PLN; break;
         case ZXC.luiListaPozicijePlanaPBN_Name : mixer_rec.TT = Mixer.TT_PBN; break;
      }

      mixer_rec.DokDate  = DateTime.Now;
    //mixer_rec.Napomena = mixer_rec.StrC_32;

      foreach(VvLookUpItem lui in theLookUpList)
      {
         if(lui.Integer < 10) continue; // 2znamenkasti integer xy: x - isAnalitika za PLAN, y - isAnalitika za REALIZACIJU 
                                        // znaci da bi doaso na ovaj PlanDUC, mora biti x=1 (10 ili 11) 

         xtrans_rec.Memset0(0);

         xtrans_rec.T_kpdbZiroA_32 = lui.Cd                          ; // Pozicija npr 1.2.3 
         xtrans_rec.T_opis_128     = ZXC.LenLimitedStr(lui.Name, 128); // NazivPozicije - 

         MixerDao.AutoSetMixer(TheDbConnection, ref line, mixer_rec, xtrans_rec);
      }

   }
}

public class PlanFilterUC : VvFilterUC
{
   #region Fieldz
  
   private VvHamper    hamp_rbt;
   public  RadioButton rbt_plan, rbt_prihPrim, rbt_rashIzd, rbt_najave, rbt_bagNbv;

   #endregion Fieldz

   #region  Constructor

   public PlanFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;
     
      CreateHampers();

      CreateHamper_4ButtonsResetGo_Width(ZXC.Q4un);

      hamp_rbt.Location = new Point(nextX, hamper4buttons.Bottom + ZXC.Qun4);
      hamperHorLine.Visible = false;

      this.Width  = hamper4buttons.Width + ZXC.Qun2;
      this.Height = hamp_rbt.Bottom + ZXC.QUN;

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region Hampers

   private void CreateHampers()
   {
      InitializeHamper_Rbt(out hamp_rbt);
   }

   private void InitializeHamper_Rbt(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 6, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q7un +ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun2 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = ZXC.Qun4 + ZXC.Qun10;

                     hamper.CreateVvLabel      (0, 0,       "Ispis", ContentAlignment.MiddleLeft);
      rbt_plan     = hamper.CreateVvRadioButton(0, 1, null, "Plan"                    , TextImageRelation.ImageBeforeText);
      rbt_prihPrim = hamper.CreateVvRadioButton(0, 2, null, "Plan prihoda i primitaka", TextImageRelation.ImageBeforeText);
      rbt_rashIzd  = hamper.CreateVvRadioButton(0, 3, null, "Plan rashoda i izdataka" , TextImageRelation.ImageBeforeText);
      rbt_najave   = hamper.CreateVvRadioButton(0, 4, null, "Ispis najava"            , TextImageRelation.ImageBeforeText);
      rbt_bagNbv   = hamper.CreateVvRadioButton(0, 5, null, "Plan bagatelne nabave"   , TextImageRelation.ImageBeforeText);

      rbt_plan.Checked = true;
      rbt_plan.Tag = true;
      
      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.HamperStyling(hamper);

      hamper.Visible = false;
   }
   

   #endregion Hampers

   #region Fld_

   public PlanDocFilter.PrintPlanEnum Fld_PrintPlanDUC
   {
      get
      {
              if(rbt_plan    .Checked) return PlanDocFilter.PrintPlanEnum.Plan      ;
         else if(rbt_prihPrim.Checked) return PlanDocFilter.PrintPlanEnum.PrihodPrim;
         else if(rbt_rashIzd .Checked) return PlanDocFilter.PrintPlanEnum.RashodIzdt;
         else if(rbt_najave  .Checked) return PlanDocFilter.PrintPlanEnum.Najava    ;
         else if(rbt_bagNbv  .Checked) return PlanDocFilter.PrintPlanEnum.BagatelNbv;

              else throw new Exception("PrintPlanEnum: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case PlanDocFilter.PrintPlanEnum.Plan      : rbt_plan    .Checked = true; break;
            case PlanDocFilter.PrintPlanEnum.PrihodPrim: rbt_prihPrim.Checked = true; break;
            case PlanDocFilter.PrintPlanEnum.RashodIzdt: rbt_rashIzd .Checked = true; break;
            case PlanDocFilter.PrintPlanEnum.Najava    : rbt_najave  .Checked = true; break;
            case PlanDocFilter.PrintPlanEnum.BagatelNbv: rbt_bagNbv  .Checked = true; break;
         }
      }
   }

   #endregion Fld_

   #region Put & GetFilterFields
  
   private PlanDocFilter ThePlanDocFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as PlanDocFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      ThePlanDocFilter = (PlanDocFilter)_filter_data;

      if(ThePlanDocFilter != null)
      {
         Fld_PrintPlanDUC = ThePlanDocFilter.PrintPlan;
      }

      // Za JAM_... : 
      this.ValidateChildren();

   }

   public override void GetFilterFields()
   {
      ThePlanDocFilter.PrintPlan = Fld_PrintPlanDUC ;
   }

   #endregion Put & GetFilterFields

   #region AddFilterMemberz()
   
   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
   }

   #endregion AddFilterMemberz()

}

public class PlanDocFilter : VvRpt_Mix_Filter
{
   public PlanDUC theDUC;
   
   public enum PrintPlanEnum
   {
      Plan, PrihodPrim, RashodIzdt, Najava, BagatelNbv
   }

   public PrintPlanEnum PrintPlan { get; set; }

   public PlanDocFilter(PlanDUC _theDUC)
   {
      this.theDUC = _theDUC;
      SetDefaultFilterValues();
   }

   #region SetDefaultFilterValues()

   public override void SetDefaultFilterValues()
   {
      PrintPlan = PrintPlanEnum.PrihodPrim;
   }

   #endregion SetDefaultFilterValues()

}

public class VvRenamePozicijaDlg : VvDialog
{
   #region Filedz
    
   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_newPozicija, tbx_oldPozicija;

   #endregion Filedz

   #region Constructor

   public VvRenamePozicijaDlg()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Preimenuj Poziciju";

      CreateHamper();

      dlgWidth        = hamper.Right  + ZXC.QunMrgn;
      dlgHeight       = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_newPozicija, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_oldPozicija, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
   }

   #endregion Constructor

   #region hamper

   private void CreateHamper()
   {
      hamper          = new VvHamper(2, 2, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN ,ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4,ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
                        hamper.CreateVvLabel        (0, 0, "Stara Pozicija:", ContentAlignment.MiddleRight);
                        hamper.CreateVvLabel        (0, 1, "Nova Pozicija:", ContentAlignment.MiddleRight);
      tbx_oldPozicija = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_newPozicija", "");
      tbx_newPozicija = hamper.CreateVvTextBox(1, 1, "tbx_newPozicija", "");
      tbx_oldPozicija.JAM_Set_LookUpTable(ZXC.luiListaPozicijePlana, (int)ZXC.Kolona.prva);
      tbx_oldPozicija.JAM_FieldExitMethod_2 = new EventHandler(OnExitOld_CopyToNew);
   }
   private void OnExitOld_CopyToNew(object sender, EventArgs e)
   {
      VvTextBox vvtb = sender as VvTextBox;

      if(vvtb.EditedHasChanges() == false) return;

      tbx_newPozicija.Text = tbx_oldPozicija.Text;
   }

   #endregion hamper

   #region Button_Click

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Button_Click

   #region Fld_ 
   
   public string Fld_OldPozicija { get { return tbx_oldPozicija.Text; } set { tbx_oldPozicija.Text = value; } }
   public string Fld_NewPozicija { get { return tbx_newPozicija.Text; } set { tbx_newPozicija.Text = value; } }

   #endregion Fld_

}
