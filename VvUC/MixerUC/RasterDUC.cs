using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public class RasterDUC : MixerDUC
{
   #region Constructor

   public RasterDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_RASTERF
         });
   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
      hamp_mjTroska.Visible = true;
      hamp_mjTroska.Location = new Point(hamp_napomena.Right, hamp_napomena.Top);
   }

   #endregion CreateSpecificHampers()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_isXxx_CreateColumn         (ZXC.Q2un , "Izuzet");
      T_kpdbNameA_50_CreateColumn  (ZXC.Q9un, "Partner" , "Kupac Naziv", false);
      T_kupdobCD_CreateColumn      (ZXC.Q3un, "ŠifraP", "Kupac Šifra");
      T_kpdbUlBrA_32_CreateColumn  (ZXC.Q3un,  "TikerP", "Kupac Ticker", -1 * ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KupdobDao.CI.ticker));
      T_artiklCD_CreateColumn      (ZXC.Q6un, "ŠifraArt", "Šifra Artikla");
      T_artiklName_CreateColumn    (0, "NazivArt", "Opis/Naziv proizvoda, usluge, robe ...");
      T_kol_CreateColumn           (ZXC.Q2un , "Kol"     , "Količina", 2);
      T_moneyA_CreateColumn        (ZXC.Q5un , "Cijena"  , "Iznos"   , 2);

      R_iznos_CreateColumn         (ZXC.Q5un, 2, "Iznos", "Iznos bez PDV-a");
      T_kpdbMjestoA_32_CreateColumn(ZXC.Q4un,    "TTbr" , "TTbr");
      T_moneyD_CreateColumn        (ZXC.Q4un, 2, "Ukupno", "Ukupan iznos racuna");
      vvtbT_kpdbMjestoA_32.JAM_ReadOnly = true;

      vvtbT_kol   .JAM_ShouldCalcTransAndSumGrid = true;
      vvtbT_moneyA.JAM_ShouldCalcTransAndSumGrid = true;
      vvtbT_moneyD.JAM_ShouldSumGrid             = true;
   }

   #endregion InitializeTheGrid_Columns()

   #region PrintDocumentRecord

   public RasterFilterUC  TheRasterFilterUC { get; set; }
   public RasterDocFilter TheRasterDocFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.TheRasterDocFilter = new RasterDocFilter(this);

      TheRasterFilterUC                         = new RasterFilterUC(this);
      TheRasterFilterUC.Parent                  = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheRasterFilterUC.Width;
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      RasterDocFilter mixerFilter = (RasterDocFilter)vvRptFilter;

      return new RptX_Raster(new Vektor.Reports.XIZ.CR_RasterDuc(), reportName, mixerFilter);
     // return new RptX_Raster(new Vektor.Reports.XIZ.CR_VirFromRaster(), reportName, mixerFilter); 
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheRasterDocFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheRasterFilterUC;
      }
   }

   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Raster, Color.Empty, clr_Raster);
   }

   #endregion Colors

   #region override void PutDefaultBabyDUCfields()

   protected override void PutDefaultBabyDUCfields()
   {


   }
   /*protected*/public override void PutSpecificsFld()
   {
      Fld_TtOpis = ZXC.luiListaMixerType.GetNameForThisCd(mixer_rec.TT);
   }


   #endregion override void PutDefaultBabyDUCfields()

}

public class RasterFilterUC : VvFilterUC
{
   #region Fieldz


   #endregion Fieldz

   #region  Constructor

   public RasterFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;

      CreateHamper_4ButtonsResetGo_Width(ZXC.Q4un);

      hamperHorLine.Visible = true;

      nextY = hamper4buttons.Bottom + razmakIzmjedjuHampera;

      this.Width = hamper4buttons.Width + ZXC.QUN;
      this.Height = hamper4buttons.Bottom + ZXC.QUN;

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region Put & GetFilterFields

   private RasterDocFilter TheRasterZnpDocFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as RasterDocFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
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

public class RasterDocFilter : VvRpt_Mix_Filter
{

   public RasterDUC theDUC;


   public RasterDocFilter(RasterDUC _theDUC)
   {
      this.theDUC = _theDUC;
      SetDefaultFilterValues();
   }

   #region SetDefaultFilterValues()

   public override void SetDefaultFilterValues()
   {
   }

   #endregion SetDefaultFilterValues()

}





