using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public class PmvDUC : MixerDUC
{
   #region Fieldz

   #endregion Fieldz

   #region Constructor

   public PmvDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_PMV
         });
   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
   }

   #endregion CreateSpecificHampers()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_artiklCD_CreateColumn    (ZXC.Q4un, "ArtiklCD", "Opis/Naziv proizvoda, usluge, robe ...");
      T_artiklName_CreateColumn  (0, "NazivArt", "Opis/Naziv proizvoda, usluge, robe ...");
      T_moneyA_CreateColumn      (ZXC.Q4un           ,    "Por.Osnovica", ""                                      ,    2);
      T_moneyB_CreateColumn      (ZXC.Q3un - ZXC.Qun4, 0, "Em.CO2"      , "Emisija CO2", true);
      T_moneyC_CreateColumn      (ZXC.Q3un - ZXC.Qun4, 0, "Ob.cm3"      , "Obujam motora cm3", true, false);
      T_intA_CreateColumn        (ZXC.Q3un - ZXC.Qun4,    "EuroN"       , "EuroNorma");
      R_st_PO_CreateColumn       (ZXC.Q3un - ZXC.Qun4, 1, "St PO"       , "Stopa posebnog poreza na temelju cijene vozila");
      R_iznos_PO_CreateColumn    (ZXC.Q4un - ZXC.Qun4, 2, "Iznos PO "   , "Iznos posebnog poreza na telju cijene vozila");
      R_st_CO2_CreateColumn      (ZXC.Q3un - ZXC.Qun4, 1, "St CO2"      , "Stopa posebnog poreza na temelju CO2   ");
      R_iznos_CO2_CreateColumn   (ZXC.Q4un - ZXC.Qun4, 2, "Iznos CO2"   , "Iznos posebnog poreza na temelju CO2");
      R_st_cm3_CreateColumn      (ZXC.Q3un - ZXC.Qun4, 1, "St cm3"      , "Stopa posebnog poreza na temelju obujma motora u cm3");
      R_iznos_cm3_CreateColumn   (ZXC.Q4un - ZXC.Qun4, 2, "Iznos cm3"   , "Iznos posebnog poreza na temelju obujma motora u cm3");
      R_st_EUN_CreateColumn      (ZXC.Q3un - ZXC.Qun4, 2, "St EUN"      , "Stopa posebnog poreza na temelju razine emisije ispu[nih plinova");
      R_iznos_EUN_CreateColumn   (ZXC.Q4un - ZXC.Qun4, 2, "Iznos EUN"   , "Iznos posebnog poreza na temelju razine emisije ispu[nih plinova");

      vvtbT_intA  .JAM_ReadOnly =  true;
   }

   #endregion InitializeTheGrid_Columns()

   #region PrintDocumentRecord

//   public PmvFilterUC ThePmvFilterUC { get; set; }
//   public PmvDocFilter ThePmvDocFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      //this.ThePmvDocFilter = new PmvDocFilter(this);

      //ThePmvFilterUC = new PmvFilterUC(this);
      //ThePmvFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      //ThePanelForFilterUC_PrintTemplateUC.Width = ThePmvFilterUC.Width;
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      //PmvDocFilter mixerFilter = (PmvDocFilter)vvRptFilter;

      //return new RptX_Pmv(new Vektor.Reports.XIZ.CR_PmvDuc(), reportName, mixerFilter);
      //// return new RptX_Pmv(new Vektor.Reports.XIZ.CR_VirFromPmv(), reportName, mixerFilter); 
      return null;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         //return this.ThePmvDocFilter;
         return null;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         //return this.ThePmvFilterUC;
         return null;
      }
   }

   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_PMV, Color.Empty, clr_PMV);
   }

   #endregion Colors

   #region Fld_

   #endregion Fld_

   #region override void PutDefaultBabyDUCfields()

   protected override void PutDefaultBabyDUCfields()
   {

   }
   /*protected*/public override void PutSpecificsFld()
   {
      Fld_TtOpis = ZXC.luiListaMixerType.GetNameForThisCd(mixer_rec.TT);
   }
   protected override void GetSpecificsFld()
   {
   }

   #endregion override void PutDefaultBabyDUCfields()

}



