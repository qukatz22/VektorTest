using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public partial class PredNrdDUC : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_skladCd, tbx_skladOpis;
   private VvHamper  hamp_sklad;

   #endregion Fieldz

   #region Constructor

   public PredNrdDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul): base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_PNA
         });

   }


   #endregion Constructor

   #region CreateSpecificHampers()

   #endregion CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
      nextX = hamp_tt.Left;
      nextY = hamp_tt.Bottom;
      InitializeHamper_sklad(out hamp_sklad);

      hamp_napomena.Location = new Point(hamp_dokNum.Left, hamp_sklad.Bottom);
      nextY = hamp_napomena.Bottom;

   }

   private void InitializeHamper_sklad(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0          1                 2         
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un - ZXC.Qun2, ZXC.Q9un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel        (0, 0, "Sklad:"       , ContentAlignment.MiddleRight);
      tbx_skladCd   = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_skladCd"  , "Skladiste");
      tbx_skladOpis = hamper.CreateVvTextBox      (2, 0, "tbx_skladOpis", "Naziv skladista");

      tbx_skladCd.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_skladCd.JAM_lui_NameTaker_JAM_Name = tbx_skladOpis.JAM_Name;
      tbx_skladOpis.JAM_ReadOnly = true;

      // 18.07.2018: 
      tbx_skladCd.JAM_DataRequired = true;
   }

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_artiklCD_CreateColumn    (ZXC.Q4un   , "Šifra"   , "Šifra"                      );
      T_artiklName_CreateColumn  (       0   , "Naziv"   , "Naziv"                      );
      T_moneyB_CreateColumn      (ZXC.Q4un, 0, "ORG"     , "ORG"          , false       );
      T_moneyC_CreateColumn      (ZXC.Q4un, 2, "BOP"     , "BOP"          , false, false);
    //T_moneyD_CreateColumn      (ZXC.Q4un, 2, "COP"     , "COP"                        );
      T_kol_CreateColumn         (ZXC.Q3un,    "Količina", "Količina"     , 2           );
    //T_moneyA_CreateColumn      (ZXC.Q5un,    "Cijena"  , "Cijena"       , 4           );
    //T_dec01_CreateColumn       (ZXC.Q3un,    "Rbt"     , "Rbt"          , 2           );
    //T_dec02_CreateColumn       (ZXC.Q3un,    "PdvSt"   , "PdvSt"        , 2           );
      T_kpdbNameA_50_CreateColumn(ZXC.Q5un,    "Partner", "Partner", false);
      T_kpdbZiroA_32_CreateColumn(ZXC.Q5un,    "Ugovor"  , "Broj ugovora" , null);
      T_kpdbZiroB_32_CreateColumn(ZXC.Q5un,    "Narudžba", "Broj narudzbe", null);

      vvtbT_kpdbNameA_50.JAM_ReadOnly =
      vvtbT_kpdbZiroA_32.JAM_ReadOnly =
      vvtbT_kpdbZiroB_32.JAM_ReadOnly = true;
   }

   public void AnyArtiklTextBox_OnGrid_Leave_2(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvTextBoxEditingControl vvtb_editingControl = sender as VvTextBoxEditingControl;

      if(vvtb_editingControl == null) return;

      if(vvtb_editingControl.Text == this.originalText) return;

      VvDataGridView theGrid = ((VvDataGridView)vvtb_editingControl.EditingControlDataGridView);

      this.originalText = vvtb_editingControl.Text;
      Artikl artikl_rec = ArtiklSifrar.Find(FoundInSifrar<Artikl>);

      int currRow = vvtb_editingControl.EditingControlRowIndex;

      if(artikl_rec != null)
      {
         theGrid.PutCell(ci.iT_vezniDokA_64, currRow, artikl_rec.ArtiklCD);
         theGrid.PutCell(ci.iT_opis_128    , currRow, artikl_rec.ArtiklName);
      }

      // samo za DUC-eve
      ZXC.TheVvForm.SetDirtyFlag(sender);
   }

   #endregion InitializeTheGrid_Columns()

   #region Fld_

   public string Fld_SkladCd   { get { return tbx_skladCd.Text; } set { tbx_skladCd .Text = value; } }
   public string Fld_SkladOpis {                                  set { tbx_skladOpis.Text = value; } }

   #endregion _Fld

   #region override PutSpecificsFld() GetSpecificsFld()

   /*protected*/
   public override void PutSpecificsFld()
   {
      PutSpecificsFld(mixer_rec);
   }

   /*protected*/public override void PutSpecificsFld(Mixer mixerLocal_rec)
   {
      Fld_TtOpis = "Prednarudžba";

      if(CtrlOK(tbx_skladCd  )) Fld_SkladCd = mixerLocal_rec.Konto;
      if(CtrlOK(tbx_skladOpis)) Fld_SkladOpis = ZXC.luiListaSkladista.GetNameForThisCd(mixerLocal_rec.Konto);
   }

   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_skladCd)) mixer_rec.Konto = Fld_SkladCd;
   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region PrintDocumentRecord

   public PredNrdFilterUC ThePredNrdFilterUC { get; set; }
   public PredNrdFilter   ThePredNrdFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.ThePredNrdFilter = new PredNrdFilter(this);

      ThePredNrdFilterUC        = new PredNrdFilterUC(this);
      ThePredNrdFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = ThePredNrdFilterUC.Width;

   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      //PredNrdFilter mixerFilter = (PredNrdFilter)vvRptFilter;

      //switch(mixerFilter.PrintPrNRD)
      //{
      //   case PredNrdFilter.PrintPrNRDEnum.PrNRD: specificMixerReport = new RptX_PredNrd(new Vektor.Reports.XIZ.CR_NazlogZaIzradu(), "NALOG ZA IZRADU", mixerFilter); break;
         
      //   default: ZXC.aim_emsg("{0}\nPrintSomeDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintPrNRD); return null;
      //}

      return null /*specificMixerReport*/;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.ThePredNrdFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.ThePredNrdFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
   }


   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_PutNal, Color.Empty, clr_PutNal);
   }

   #endregion Colors

   #region overrideTabPageTitle

   public override string TabPageTitle1
   {
      get { return " "; }
   }
   //public override string TabPageTitle2
   //{
   //   get { return "Obračun ostalih troškova"; }
   //}
   
   #endregion overrideTabPageTitle


}

public class PredNrdFilterUC : VvFilterUC
{
   #region Fieldz

   #endregion Fieldz

   #region  Constructor

   public PredNrdFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;
     
      hamperHorLine.Visible = false;

      this.Width  = hamper4buttons.Width + ZXC.QUN;
      this.Height = hamper4buttons.Bottom + ZXC.QUN;

      hamper4buttons.Visible = false;

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region Put & GetFilterFields

   private PredNrdFilter ThePredNrdFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as PredNrdFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      ThePredNrdFilter = (PredNrdFilter)_filter_data;

      if(ThePredNrdFilter != null)
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

public class PredNrdFilter : VvRpt_Mix_Filter
{

   public enum PrintPrNRDEnum
   {
      PrNRD
   }

   public PrintPrNRDEnum PrintPrNRD { get; set; }

   public PredNrdDUC theDUC;

   public PredNrdFilter(PredNrdDUC _theDUC)
   {
      this.theDUC = _theDUC;
      SetDefaultFilterValues();
   }

   #region SetDefaultFilterValues()

   public override void SetDefaultFilterValues()
   {
      ZXC.VvDataBaseInfo vvDBinfo = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects;
      int projectYear            = int.Parse(vvDBinfo.ProjectYear);
      int thisYear               = DateTime.Now.Year;
      PrintPrNRD                 = PrintPrNRDEnum.PrNRD;
   }

   #endregion SetDefaultFilterValues()

}
