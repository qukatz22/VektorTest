using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public class RasterBDUC : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_ValName, tbx_DevTecaj, tbx_ObjektName, tbx_NacPlac, tbx_ZiroRn;
   private VvHamper  hamp_ValName;
   private CheckBox  cbx_isCash;

   #endregion Fieldz

   #region Constructor

   public RasterBDUC(Control parent, Mixer _mixer, VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_RASTERB
         });

      Kupdob.FillLookUpItemZiroList(ZXC.CURR_prjkt_rec);

   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
      hamp_mjTroska.Visible = true;

      hamp_mjTroska.Location = new Point(hamp_tt.Left, hamp_tt.Bottom);

      InitializeHamper_ValutaName(out hamp_ValName);

      nextY = hamp_ValName.Bottom;
   }

   private void InitializeHamper_ValutaName(out VvHamper hamper)
   {
      hamper = new VvHamper(11, 1, "", TheTabControl.TabPages[0], false, hamp_napomena.Left, hamp_napomena.Bottom, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un, ZXC.Q3un, ZXC.Q5un, ZXC.Q8un +ZXC.Q2un -ZXC.Qun2, ZXC.Q6un, ZXC.Q4un - ZXC.Qun4, ZXC.Q3un, ZXC.Q2un,ZXC.Q2un - ZXC.Qun2, ZXC.Q7un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,                     ZXC.Qun8, ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                    hamper.CreateVvLabel(0, 0, "IzvVal:", ContentAlignment.MiddleRight);
      tbx_ValName = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_ValName", "Naziv devizne valute", GetDB_ColumnSize(DB_ci.devName));
      tbx_ValName.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_ValName.JAM_Set_LookUpTable(ZXC.luiListaDeviza, (int)ZXC.Kolona.prva);
      // Nota bene; buduci je tbx_DevName VvLookUp TextBox onde mu je tbx_tt.JAM_FieldExitMethod 'potrosena' lookUp methodom. 
    //  tbx_ValName.JAM_FieldExitMethod_2 = new EventHandler(OnExit_DevName_SetValutaNameInUse);

                     hamper.CreateVvLabel        (2, 0, "Tečaj:", ContentAlignment.MiddleRight);
      tbx_DevTecaj = hamper.CreateVvTextBoxLookUp(3, 0, "tbx_DevTecaj", "Devizni Tečaj",12);
      tbx_DevTecaj.JAM_ReadOnly = true;
      tbx_DevTecaj.JAM_MarkAsNumericTextBox(6, true, decimal.MaxValue, decimal.MinValue, true);

      hamper.CreateVvLabel(4, 0, "Naziv Objekta:", ContentAlignment.MiddleRight);
      tbx_ObjektName = hamper.CreateVvTextBox(5, 0, "tbx_nazivObjekta", "Naziv objekta", GetDB_ColumnSize(DB_ci.strC_32));

                    hamper.CreateVvLabel        (6, 0, "Način plaćanja:", ContentAlignment.MiddleRight);
      tbx_NacPlac = hamper.CreateVvTextBoxLookUp(7, 0, "tbx_NacPlac", "Način plaćanja", GetDB_ColumnSize(DB_ci.strC_32));

      cbx_isCash = hamper.CreateVvCheckBox_OLD(8, 0, null, "Got", RightToLeft.No);
      cbx_isCash.Name = "IsCash";
      cbx_isCash.Enabled = false;

      tbx_NacPlac.JAM_Set_LookUpTable(ZXC.luiListaRiskVrstaPl, (int)ZXC.Kolona.prva);
      tbx_NacPlac.JAM_lui_FlagTaker_JAM_Name = cbx_isCash.Name;

                   hamper.CreateVvLabel(9, 0, "IBAN:", ContentAlignment.MiddleRight);
      tbx_ZiroRn = hamper.CreateVvTextBoxLookUp(10, 0, "", "tbx_Ziro");

      tbx_ZiroRn.JAM_Set_LookUpTable(ZXC.luiListaRiskZiroRn, (int)ZXC.Kolona.prva);
      tbx_ZiroRn.JAM_lookUp_NOTobligatory = true;


   }

   #endregion CreateSpecificHampers()

   #region Fld_

   public string Fld_DevName   { get { return tbx_ValName.Text;                     }  set { tbx_ValName  .Text = value;            } }
 
   public ZXC.ValutaNameEnum Fld_DevNameAsEnum
   {
      get
      {
         if (tbx_ValName.Text.IsEmpty()) return ZXC.ValutaNameEnum.EMPTY;
         else                            return (ZXC.ValutaNameEnum)Enum.Parse(typeof(ZXC.ValutaNameEnum), tbx_ValName.Text, true);
      }
   }

   public string  Fld_ObjektName { get { return tbx_ObjektName.Text;            } set { tbx_ObjektName.Text        = value ; } }
   public decimal Fld_DevTecaj   { get { return tbx_DevTecaj.GetDecimalField(); } set { tbx_DevTecaj.PutDecimalField(value); } }
   public bool    Fld_IsNpCash   { get { return cbx_isCash.Checked;             } set { cbx_isCash.Checked         = value ; } }
   public string  Fld_NacPlac    { get { return tbx_NacPlac.Text;               } set { tbx_NacPlac.Text           = value ; } } 
   public string  Fld_ZiroRn     { get { return tbx_ZiroRn .Text;               } set { tbx_ZiroRn.Text            = value ; } } 


   #endregion Fld_
   
   #region override void PutDefaultBabyDUCfields()

   protected override void PutDefaultBabyDUCfields()
   {
   }
   /*protected*/public override void PutSpecificsFld()
   {
      if(CtrlOK(tbx_ValName   )) Fld_DevName    = mixer_rec.DevName;
      if(CtrlOK(tbx_ObjektName)) Fld_ObjektName = mixer_rec.StrC_32;

      if(Fld_DevName.NotEmpty() && Fld_DevName != /*"HRK"*/ ZXC.EURorHRKstr) Fld_DevTecaj = mixer_rec.DevTecaj;
      else                                                                   Fld_DevTecaj = 0.00M;

      Fld_TtOpis = ZXC.luiListaMixerType.GetNameForThisCd(mixer_rec.TT);

      if(CtrlOK(tbx_NacPlac)) Fld_NacPlac  = mixer_rec.StrD_32;
                              Fld_IsNpCash = mixer_rec.IsXxx  ;
      if(CtrlOK(tbx_ZiroRn))  Fld_ZiroRn   = mixer_rec.StrA_40;

   }
   
   protected override void GetSpecificsFld()
   {
     if(CtrlOK(tbx_ValName   )) mixer_rec.DevName = Fld_DevName   ;
     if(CtrlOK(tbx_ObjektName)) mixer_rec.StrC_32 = Fld_ObjektName;
     if(CtrlOK(tbx_NacPlac   )) mixer_rec.StrD_32 = Fld_NacPlac   ;
                                mixer_rec.IsXxx   = Fld_IsNpCash  ;
     if(CtrlOK(tbx_ZiroRn    )) mixer_rec.StrA_40 = Fld_ZiroRn    ;

   }

   #endregion override void PutDefaultBabyDUCfields()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_isXxx_CreateColumn         (ZXC.Q2un         ,    "Izuzet"            );
      T_kpdbUlBrB_32_CreateColumn  (ZXC.Q3un-ZXC.Qun2,    "Objekt"  , "Objekt");
      T_kpdbNameA_50_CreateColumn  (ZXC.Q6un         ,    "Partner" , "Kupac Naziv", false);
      T_kupdobCD_CreateColumn      (ZXC.Q3un, "ŠifraP", "Kupac Šifra");
      T_kpdbUlBrA_32_CreateColumn  (ZXC.Q3un         ,    "TikerP"  , "Kupac Ticker", -1 * ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KupdobDao.CI.ticker));
      T_artiklCD_CreateColumn      (ZXC.Q4un, "ŠifraArt", "Šifra Artikla");
      T_artiklName_CreateColumn    (ZXC.Q8un, "NazivArt", "Opis/Naziv proizvoda, usluge, robe ...");
      T_kol_CreateColumn           (ZXC.Q3un         ,    "Kol"     , "Količina", 2);
      T_moneyA_CreateColumn        (ZXC.Q4un         ,    "Cijena"  , "Iznos"   , 2);
      T_moneyB_CreateColumn        (ZXC.Q3un         , 2, "Nak1"    , "" , true);
      T_moneyC_CreateColumn        (ZXC.Q3un         , 2, "Nak2"    , "" , true, true);
      R_iznos_CreateColumn         (ZXC.Q4un         , 2, "Iznos"   , "Iznos bez PDV-a");
      T_kpdbMjestoA_32_CreateColumn(ZXC.Q4un, "TTbr" ,    "TTbr"           );
      T_moneyD_CreateColumn        (ZXC.Q4un,          2, "Ukupno"  , "Ukupan iznos racuna"  );

      //vvtbT_kpdbMjestoA_32.JAM_ReadOnly = true;

      vvtbT_kol   .JAM_ShouldCalcTransAndSumGrid = true;
      vvtbT_moneyA.JAM_ShouldCalcTransAndSumGrid = true;
      vvtbT_moneyB.JAM_ShouldCalcTransAndSumGrid = true;
      vvtbT_moneyC.JAM_ShouldCalcTransAndSumGrid = true;
      vvtbT_moneyD.JAM_ShouldSumGrid = true;
   }
 
   #endregion InitializeTheGrid_Columns()

   #region PrintDocumentRecord

   public RasterBFilterUC TheRasterBFilterUC   { get; set; }
   public RasterBDocFilter TheRasterBDocFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.TheRasterBDocFilter = new RasterBDocFilter(this);

      TheRasterBFilterUC                        = new RasterBFilterUC(this);
      TheRasterBFilterUC.Parent                 = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheRasterBFilterUC.Width;
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      RasterBDocFilter mixerFilter = (RasterBDocFilter)vvRptFilter;

      switch(mixerFilter.PrintrasterB)
      {
         case RasterBDocFilter.PrintRasterBEnum.Usko     : specificMixerReport = new RptX_RasterB(new Vektor.Reports.XIZ.CR_RasterPduc(), reportName, mixerFilter); break;
         case RasterBDocFilter.PrintRasterBEnum.Prosireno: specificMixerReport = new RptX_RasterB(new Vektor.Reports.XIZ.CR_RasterBduc(), reportName, mixerFilter); break;

         default: ZXC.aim_emsg("{0}\nRasterBDocDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintrasterB); return null;
      }

      return specificMixerReport;
      
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheRasterBDocFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheRasterBFilterUC;
      }
   }

   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Raster, Color.Empty, clr_Raster);
   }

   #endregion Colors





}

public class RasterBFilterUC : VvFilterUC
{
   #region Fieldz

   private VvHamper    hamp_rbt, hamp_cbx;
   private RadioButton rbt_raster, rbt_resterDetalj;
   private CheckBox    cbx_razmakRed;

   #endregion Fieldz

   #region  Constructor

   public RasterBFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;

      CreateHampers();

      CreateHamper_4ButtonsResetGo_Width(ZXC.Q4un);

      hamp_rbt.Location      = new Point(nextX, hamper4buttons.Bottom + ZXC.Qun4);
      hamp_cbx.Location      = new Point(nextX, hamp_rbt.Bottom + ZXC.Qun4);
      hamperHorLine.Location = new Point(nextX, hamp_cbx.Bottom + ZXC.Qun4);
      hamperHorLine.Parent   = this;

      nextY = LocationOfHamper_HorLine(nextX, hamp_cbx.Bottom, hamp_cbx.Width) + razmakIzmjedjuHampera;

      this.Width  = hamp_rbt.Width + ZXC.QUN;
      this.Height = hamperHorLine.Bottom + ZXC.QUN;

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region Hampers

   private void CreateHampers()
   {
      InitializeHamper_Rbt(out hamp_rbt);
      InitializeHamper_Cbx(out hamp_cbx);
   }

   private void InitializeHamper_Rbt(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q8un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = ZXC.Qun4 + ZXC.Qun10;

      hamper.CreateVvLabel      (0, 0,       "Vrsta ispisa", ContentAlignment.MiddleLeft);

      rbt_raster       = hamper.CreateVvRadioButton(0, 1, null, "Raster"        , TextImageRelation.ImageBeforeText);
      rbt_resterDetalj = hamper.CreateVvRadioButton(0, 2, null, "Raster detalji", TextImageRelation.ImageBeforeText);

      rbt_raster.Checked = true;
      rbt_raster.Tag = true;

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.HamperStyling(hamper);
   }
   
   private void InitializeHamper_Cbx(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q8un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = ZXC.Qun4 + ZXC.Qun10;

      cbx_razmakRed = hamper.CreateVvCheckBox_OLD(0, 0, null, "Veći razmak redova", System.Windows.Forms.RightToLeft.No);

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.HamperStyling(hamper);
   }

   #endregion Hampers

   #region Fld_

   public RasterBDocFilter.PrintRasterBEnum Fld_PrintRasterB
   {
      get
      {
         if     (rbt_raster      .Checked) return RasterBDocFilter.PrintRasterBEnum.Usko     ;
         else if(rbt_resterDetalj.Checked) return RasterBDocFilter.PrintRasterBEnum.Prosireno;

         else throw new Exception("Fld_PrintRasterB: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case RasterBDocFilter.PrintRasterBEnum.Usko     : rbt_raster      .Checked = true; break;
            case RasterBDocFilter.PrintRasterBEnum.Prosireno: rbt_resterDetalj.Checked = true; break;
         }
      }
   }

   public bool Fld_RazmakRed           { get { return cbx_razmakRed.Checked; } set { cbx_razmakRed.Checked = value; } }
   public bool Fld_NeedsHorizontalLine { get { return cb_Line      .Checked; } set { cb_Line      .Checked = value; } }

   #endregion Fld_

   #region Put & GetFilterFields

   private RasterBDocFilter TheRasterBDocFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as RasterBDocFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheRasterBDocFilter = (RasterBDocFilter)_filter_data;

      if(TheRasterBDocFilter != null)
      {
         Fld_PrintRasterB        = TheRasterBDocFilter.PrintrasterB;
         Fld_RazmakRed           = TheRasterBDocFilter.IsRazmakRed;
         Fld_NeedsHorizontalLine = TheRasterBDocFilter.NeedsHorizontalLine;
      }

      // Za JAM_... : 
      this.ValidateChildren();

   }

   public override void GetFilterFields()
   {
      TheRasterBDocFilter.PrintrasterB        = Fld_PrintRasterB;
      TheRasterBDocFilter.IsRazmakRed         = Fld_RazmakRed   ;
      TheRasterBDocFilter.NeedsHorizontalLine = Fld_NeedsHorizontalLine ;
   }

   #endregion Put & GetFilterFields

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
   }

   #endregion AddFilterMemberz()

}

public class RasterBDocFilter : VvRpt_Mix_Filter
{

   public RasterBDUC theDUC;
   
   public enum PrintRasterBEnum
   {
      Usko, Prosireno
   }

   public bool IsRazmakRed               { get; set; }
   public PrintRasterBEnum  PrintrasterB { get; set; }

   public RasterBDocFilter(RasterBDUC _theDUC)
   {
      this.theDUC = _theDUC;
      SetDefaultFilterValues();
   }

   #region SetDefaultFilterValues()

   public override void SetDefaultFilterValues()
   {
      PrintrasterB = PrintRasterBEnum.Usko;
   }

   #endregion SetDefaultFilterValues()

}
