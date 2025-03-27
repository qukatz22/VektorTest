using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public partial class MixerDUC : VvPolyDocumRecordUC
{
   #region Fieldz

   public Mixer mixer_rec;
   public Xtrans dgvXtrans_rec;
   public Xtrano dgvXtrano_rec;

   public int nextX = 0, nextY = ZXC.Qun4, razmakHamp = ZXC.Qun10;
   public VvHamper hamp_dokNum, hamp_dokDate, hamp_tt, hamp_napomena, hamp_intA, hamp_dateA, hamp_vezaPrcCd, hamp_mjTroska;
   public VvTextBox tbx_DokNum, tbx_DokDate, tbx_TtOpis, tbx_TtNum, tbx_TT, tbx_Napomena, tbx_intA, tbx_dateA,
                           tbx_ProjektCD, tbx_v1_tt, tbx_v1_ttNum, tbx_v2_tt, tbx_v2_ttNum, tbx_v1_ttOpis, tbx_v2_ttOpis, tbx_externLink1, tbx_externLink2,
                           tbx_mtros_cd, tbx_mtros_tk, tbx_mtros_Naziv;

   public VvDateTimePicker dtp_DokDate, dtp_dateA;

   public VvTextBox vvtbT_moneyA, vvtbT_kol, vvtbT_opis_128, vvtbT_kpdbNameA_50, vvtbT_kpdbUlBrA_32, vvtbT_kpdbMjestoA_32, vvtbT_kpdbZiroA_32, vvtbT_kpdbNameB_50, vvtbT_kpdbUlBrB_32,
                     vvtbT_kpdbMjestoB_32, vvtbT_kpdbZiroB_32, vvtbT_strA_2, vvtbT_strB_2, vvtbT_vezniDokA_64, vvtbT_vezniDokB_64, vvtbT_strC_2, vvtbT_kupdobCD,
                     vvtbT_artiklCD, vvtbT_artiklName, vvtbT_konto, vvtbT_intA, vvtbT_intB, vvtbT_KolMoneyA,
                     vvtbT2_moneyA, vvtbT2_opis_128, vvtbT2_konto, vvtbT2_devValuta, vvtbT_personCD, vvtbT_ime,
                     vvtbT_moneyB, vvtbT_moneyC, vvtbT_moneyD,
                     vvtbT_st_PO     , vvtbT_iznos_PO  , vvtbT_st_CO2    , vvtbT_iznos_CO2 ,
                     vvtbT_st_cm3    , vvtbT_iznos_cm3 , vvtbT_st_EUN    , vvtbT_iznos_EUN ,
                     vvtbT_dec01, vvtbT_dec11, vvtbT_dec21, vvtbT_dec31, vvtbT_str01, vvtbT_str11, vvtbT_str21, vvtbT_str31,vvtbT_dec01_2, vvtbT_dec11_2, vvtbT_dec21_2, vvtbT_dec31_2,
                     vvtbT_dec02, vvtbT_dec12, vvtbT_dec22,              vvtbT_str02, vvtbT_str12, vvtbT_str22,             vvtbT_dec02_2, vvtbT_dec12_2, vvtbT_dec22_2,             
                     vvtbT_dec03, vvtbT_dec13, vvtbT_dec23,              vvtbT_str03, vvtbT_str13, vvtbT_str23,             vvtbT_dec03_2, vvtbT_dec13_2, vvtbT_dec23_2,             
                     vvtbT_dec04, vvtbT_dec14, vvtbT_dec24,              vvtbT_str04, vvtbT_str14, vvtbT_str24,             vvtbT_dec04_2, vvtbT_dec14_2, vvtbT_dec24_2,             
                     vvtbT_dec05, vvtbT_dec15, vvtbT_dec25,              vvtbT_str05, vvtbT_str15, vvtbT_str25,             vvtbT_dec05_2, vvtbT_dec15_2, vvtbT_dec25_2,             
                     vvtbT_dec06, vvtbT_dec16, vvtbT_dec26,              vvtbT_str06, vvtbT_str16, vvtbT_str26,             vvtbT_dec06_2, vvtbT_dec16_2, vvtbT_dec26_2,             
                     vvtbT_dec07, vvtbT_dec17, vvtbT_dec27,              vvtbT_str07, vvtbT_str17, vvtbT_str27,             vvtbT_dec07_2, vvtbT_dec17_2, vvtbT_dec27_2,             
                     vvtbT_dec08, vvtbT_dec18, vvtbT_dec28,              vvtbT_str08, vvtbT_str18, vvtbT_str28,             vvtbT_dec08_2, vvtbT_dec18_2, vvtbT_dec28_2,             
                     vvtbT_dec09, vvtbT_dec19, vvtbT_dec29,              vvtbT_str09, vvtbT_str19, vvtbT_str29,             vvtbT_dec09_2, vvtbT_dec19_2, vvtbT_dec29_2,             
                     vvtbT_dec10, vvtbT_dec20, vvtbT_dec30,              vvtbT_str10, vvtbT_str20, vvtbT_str30,             vvtbT_dec10_2, vvtbT_dec20_2, vvtbT_dec30_2,             
                     vvtbT_MFS, vvtbT_RFS,vvtbT_PFS, vvtbR_SumMoney   , 
        
                     vvtbR_RealMPC_EUR            ,
                     vvtbR_HR_historyOsnovnaCij_Kn,
                     vvtbR_HR_historyOprermaCij_Kn,
                     vvtbR_HR_history_TOTAL_Cij_Kn, 
                     vvtbR_mm1Registracije        ,
                     vvtbR_yy1Registracije        ,
                     vvtbR_co2Emisija             ,
                     vvtbR_finalCostEUR           , 
                     vvtbR_monthAge               ,
                     vvtbR_ageSt                  ,
                     vvtbR_ONENEur                ,
                     vvtbR_VNPCEur                , 
                     vvtbR_PPMV_historyEur        ,       
                     vvtbR_PPMVeur                ,
                     vvtbR_toPayAutohouseEUR      ,
                     vvtbR_toPayTotalEur          
                     ;

   private VvCheckBox vvcbx_isXxx, vvcbx_isIntB;
   private CheckBox cbx_fakStop;

   //private VvTextBoxColumn colVvText, colVvTextInt;
   public VvTextBoxColumn colVvText, colVvTextInt;
   public VvDateTimePickerColumn colDate1, colDate2, colDate3, colDate4;
   public VvDateAnd_TIME_PickerColumn colDate1wT, colDate2wT, colDate4wT;
   public VvDate_TimeOnly_PickerColumn colDate1onlyT, colDate2onlyT,
      colDateOnlyT01  , colDateOnlyT02  , colDateOnlyT03  , colDateOnlyT04  , colDateOnlyT05  , colDateOnlyT06  , colDateOnlyT07  , colDateOnlyT08  , colDateOnlyT09  , colDateOnlyT10  ,
      colDateOnlyT11  , colDateOnlyT12  , colDateOnlyT13  , colDateOnlyT14  , colDateOnlyT15  , colDateOnlyT16  , colDateOnlyT17  , colDateOnlyT18  , colDateOnlyT19  , colDateOnlyT20  ,
      colDateOnlyT21  , colDateOnlyT22  , colDateOnlyT23  , colDateOnlyT24  , colDateOnlyT25  , colDateOnlyT26  , colDateOnlyT27  , colDateOnlyT28  , colDateOnlyT29  , colDateOnlyT30  , colDateOnlyT31  ,
      colDateOnlyT01_2, colDateOnlyT02_2, colDateOnlyT03_2, colDateOnlyT04_2, colDateOnlyT05_2, colDateOnlyT06_2, colDateOnlyT07_2, colDateOnlyT08_2, colDateOnlyT09_2, colDateOnlyT10_2,
      colDateOnlyT11_2, colDateOnlyT12_2, colDateOnlyT13_2, colDateOnlyT14_2, colDateOnlyT15_2, colDateOnlyT16_2, colDateOnlyT17_2, colDateOnlyT18_2, colDateOnlyT19_2, colDateOnlyT20_2,
      colDateOnlyT21_2, colDateOnlyT22_2, colDateOnlyT23_2, colDateOnlyT24_2, colDateOnlyT25_2, colDateOnlyT26_2, colDateOnlyT27_2, colDateOnlyT28_2, colDateOnlyT29_2, colDateOnlyT30_2, colDateOnlyT31_2 ;
   private DataGridViewTextBoxColumn colScrol;
   private VvCheckBoxColumn colCbox;
   private DataGridViewCheckBoxColumn colCbxClassic;

   protected ZXC.DbNavigationRestrictor dbNavigationRestrictor;

   /*protected*/
   public VvReport specificMixerReport;
   protected string specificMixerReportName;

   public Color clr_PutNal, clr_Virm, clr_Raster, clr_Zahtj, clr_SMD, clr_EVD, clr_PutNal2, clr_RVR, clr_IRV, clr_GFI, clr_Plan, clr_PMV,
                clr_MVR_NEdjelja, clr_MVR_SUbota, clr_MVR_VR_Fore, clr_MVR_VR_Back, clr_URZ, clr_RUG, clr_MVR_Praznik, clr_KOP_PTG, clr_KOPLine_PTG;
   public Button btn_v1TT, btn_v2TT, btn_proj, btn_goExLink1, btn_openExLink1, btn_goExLink2, btn_openExLink2;
   public KtoShemaDsc kshema;

   #endregion Fieldz

   #region Virtual Metodz

   protected virtual void InitializeDUC_Specific_Columns() { }
   protected virtual void InitializeDUC_Specific_Columns2() { }
   protected virtual void PutDefaultBabyDUCfields() { }
   protected virtual void AddColorsToBaby() { }
   protected virtual void CreateMixerDokumentPrintUC() { }
   protected virtual void CreateSpecificHampers() { }
   protected virtual void CalcLocationHamperBeloWGrid() { }
   /*protected*/public virtual void PutSpecificsFld() { }
   /*protected*/public virtual void PutSpecificsFld(Mixer mixerLocal_rec) { }
   protected virtual void GetSpecificsFld() { }

   #endregion Virtual Metodz

   #region Constructor

   public MixerDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul)
   {
      SuspendLayout();

      mixer_rec = _mixer;
    //kshema = new KtoShemaDsc(ZXC.dscLuiLst_KtoShema);
      kshema = ZXC.KSD;
      ThePrefferedRecordSorter = VirtualDataRecord.DefaultSorter;

      this.TheSubModul = vvSubModul;

      CreateTabPages();
      CreateHampers();

      nextY = hamp_napomena.Bottom;
      CreateSpecificHampers();

      #region TheG

      TheG       = CreateVvDataGridView(ThePolyGridTabControl.TabPages[0], "Xtrans Grid");
      TheSumGrid = CreateSumGrid(TheG, ThePolyGridTabControl.TabPages[0], "SUM Xtrans Grid");

      InitializeDUC_Specific_Columns();
      if(this is GFI_TSI_DUC == false) Initialize_Scroll_Columns();
      InitializeTheSUMGrid_Columns(TheG);

      TheG.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged_SumGrid);

      TheG.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(NZI_Grid_CellMouseDoubleClick_OpenFakturDUC);

      // 13.01.2017: 
      if(this is ZahtjevRNMDUC          ||
         this is ZahtjeviDUC            ||
         this is NazivArtiklaZaKupcaDUC ||
         this is ExtCjeniciDUC          ||
         this is PmvDUC                 ||
         this is RasterDUC              ||
         this is RasterBDUC              )
      {
         TheG.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(TheG_CellMouseDoubleClick_OpenArtiklUC);
      }


      if(this is KOP_PTG_DUC)
      {
         TheG.ThisIsOneRowFixedGrid = true; // da se ne moze dodavati niti brisati redove 
      }

      #endregion TheG

      #region TheG2

      TheG2 = CreateVvDataGridView(ThePolyGridTabControl.TabPages[1], "Xtrano Grid");
      TheSumGrid2 = CreateSumGrid(TheG2, ThePolyGridTabControl.TabPages[1], "SUM Xtrano Grid");

      InitializeDUC_Specific_Columns2();
      InitializeTheSUMGrid_Columns(TheG2);

      TheG2.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged_SumGrid);

      #endregion TheG2

      InitializeVvUserControl(parent);

      SetColors();
      AddColorsToBaby();

      #region CalcLocationSizeAnchor

      if(this is GFI_TSI_DUC)
      {
         TheSumGrid.Visible = false;
         nextY += ZXC.Qun2;
      }
      CalcLocationSizeAnchor_ThePolyGridTabControl(TheTabControl.TabPages[0], nextX, nextY);

      CalcLocationSizeAnchor_TheDGV(TheG, ZXC.Qun8, ZXC.Qun8, hamp_napomena.Width);
      //CalcLocationSizeAnchor_TheDGV(TheG, ZXC.Qun8, ZXC.Qun8, hamp_tt.Right);
      CalcLocationSizeAnchor_GridSum(TheG);

      CalcLocationSizeAnchor_TheDGVAndTheSumGrid_NEW(TheG2, ZXC.QunMrgn, ZXC.QunMrgn);
      CalcLocationHamperBeloWGrid();

      #endregion CalcLocationSizeAnchor

      SetXtransColumnIndexes();
      SetXtranoColumnIndexes();

      CreateMixerDokumentPrintUC();

      ResumeLayout();

      // 29.06.2015: 
      this.Validating += new CancelEventHandler(MixerDUC_Validating);

   }

   private void TheG_CellMouseDoubleClick_OpenArtiklUC(object sender, DataGridViewCellMouseEventArgs e)
   {
      int rowIdx = e.RowIndex;

      if(rowIdx.IsNegative()) return;

      if(e.ColumnIndex != ci.iT_artiklCD &&
         e.ColumnIndex != ci.iT_artiklName) return;

      string artiklCD   = TheG.GetStringCell(ci.iT_artiklCD  , rowIdx, false);
      // 19.12.2014: 
      if(artiklCD.IsEmpty()) return; // znaci da smo u zutome probali doubleclickom inicirati editiranje cell-a 

      //Artikl artikl_rec = new Artikl();
      //artikl_rec.VvDao.SetMe_Record_bySomeUniqueColumn(conn, artikl_rec, artiklCD, ZXC.ArtiklSchemaRows[ZXC.ArtCI.artiklCD], false);

      SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

    // 18.08.2011: bijo BUG! Kako je reference type, saljuci element ArtiklSifrar-a kao TheVvDataRecord, svaka promkena TheVvDataRecord-a je mijenjala i element ArtiklSifrar-a! 
    //Artikl artikl_rec = ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artiklCD);

      Artikl artikl_rec;
      try
      {
         artikl_rec = ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artiklCD).MakeDeepCopy();
      }
      catch(Exception ex)
      {
         artikl_rec = null;
      }

      if(artikl_rec != null)
      {
         TheVvTabPage.TheVvForm.OpenNew_Record_TabPage_wInitialRecord(TheVvTabPage.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.ART), artikl_rec);
      }
   }

   private void Initialize_Scroll_Columns()
   {
      ColumnForScroll_CreateColumn(ZXC.QUN);
   }

   #endregion Constructor

   #region TabPages

   private void CreateTabPages()
   {
      TheTabControl.TabPages.Add(CreateVvInnerTabPages("Osnovno", "", ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
      ThePolyGridTabControl.Parent = TheTabControl.TabPages[0];

      if(this is BmwDUC)
      {
         TheTabControl.TabPages.Add(CreateVvInnerTabPages("Zoom", "Zoom", ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
         TheTabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(TheTabControl_SelectionChanged_OsnovnoZoom);
      }
   }

   private void TheTabControl_SelectionChanged_OsnovnoZoom(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   {
      if(TheVvTabPage != null && TheVvTabPage.IsArhivaTabPage) return;

      if(TheVvTabPage != null && ((VvInnerTabPage)newPage) != null && ((VvInnerTabPage)newPage).TheInnerTabPageKindEnum == ZXC.VvInnerTabPageKindEnum.ReportViewer_TabPage) return;
      if(TheVvTabPage != null && ((VvInnerTabPage)newPage) != null && ((VvInnerTabPage)newPage).TheInnerTabPageKindEnum == ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage)
      {
         ThePanelForFilterUC_PrintTemplateUC.Visible = false;
         return;
      }

      if(newPage.Name == "Zoom")
      {
         ThePolyGridTabControl.Parent = (VvInnerTabPage)newPage;
         CalcLocationSizeAnchor_ThePolyGridTabControl(ThePolyGridTabControl.Parent, nextX, 0);
      }
      else
      {
         ThePolyGridTabControl.Parent = (VvInnerTabPage)newPage;
         CalcLocationSizeAnchor_ThePolyGridTabControl(ThePolyGridTabControl.Parent, nextX, nextY);
      }
   }

   protected const string ptgKop_TabPageName   = "KOREKCIJA OPL";
 //protected const string ptgKop_2_TabPageName = "KOREKCIJA DanZaFaktur";
   
   #endregion TabPages

   #region Hampers

   private void CreateHampers()
   {
      InitializeHamper_dokNum(out hamp_dokNum);
      InitializeHamper_dokDate(out hamp_dokDate);
      InitializeHamper_tt(out hamp_tt);
      InitializeHamper_IntA(out hamp_intA);
      InitializeHamper_DateA(out hamp_dateA);
      InitializeHamper_napomena(out hamp_napomena);
      InitializeHamper_VezaPrjCd(out hamp_vezaPrcCd);
      InitializeHamper_mjTroska(out hamp_mjTroska);
      CalcLocationOfHampers();
   }

   private void CalcLocationOfHampers()
   {
      hamp_dokNum.Location   = new Point(nextX, nextY);
      hamp_dokDate.Location  = new Point(hamp_dokNum.Right + ZXC.Qun4, nextY);
      hamp_tt.Location       = new Point(hamp_dokDate.Right + ZXC.Qun4, nextY);
      hamp_napomena.Location = new Point(nextX, hamp_tt.Bottom);

      if(this is RasterDUC || this is RasterBDUC)
      {
         hamp_intA.Visible = true;
         hamp_dateA.Visible = true;
         hamp_intA.Location = new Point(hamp_dokDate.Right, nextY);
         hamp_dateA.Location = new Point(hamp_intA.Right, nextY);
         hamp_tt.Location = new Point(hamp_dateA.Right, nextY);
      }


   }

   private void InitializeHamper_dokDate(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", TheTabControl.TabPages[0], false);

           if(this is PutNalDUC)     hamper.VvColWdt = new int[] { ZXC.Q7un - ZXC.Qun4 - ZXC.Qun2, ZXC.Q4un };
      else if(this is ZahtjeviDUC ||
              this is SmdDUC      ||
              this is EvidencijaDUC)         hamper.VvColWdt = new int[] { ZXC.Q8un - ZXC.Qun4, ZXC.Q4un };
      else if(this is KnjigaGostijuDUC)      hamper.VvColWdt = new int[] { ZXC.Q4un - ZXC.Qun4, ZXC.Q4un };
      else if(this is ZahtjevRNMDUC ||
              this is BmwDUC         )       hamper.VvColWdt = new int[] { ZXC.Q4un           , ZXC.Q4un };
      else if(this is UrudzbeniDUC  || 
              this is UgovoriDUC   )         hamper.VvColWdt = new int[] { ZXC.Q6un           , ZXC.Q4un };
      else if(this is ZLJ_DUC       ||
              this is ZPG_DUC         )      hamper.VvColWdt = new int[] { ZXC.Q5un, ZXC.Q4un };
      else if(this is NazivArtiklaZaKupcaDUC)hamper.VvColWdt = new int[] { ZXC.Q4un, ZXC.Q4un };
      else                                   hamper.VvColWdt = new int[] { ZXC.Q3un           , ZXC.Q4un };
      
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      string lblText = (this is KnjigaGostijuDUC) ? "Datum prijave:" : (this is UrudzbeniDUC) ? "Datum primitka:" : "Datum:";

                    hamper.CreateVvLabel  (0, 0, lblText, ContentAlignment.MiddleRight);
      tbx_DokDate = hamper.CreateVvTextBox(1, 0, "tbx_dokDate", "Datum dokumenta");
      tbx_DokDate.JAM_IsForDateTimePicker = true;
      dtp_DokDate = hamper.CreateVvDateTimePicker(1, 0, "", tbx_DokDate);
      dtp_DokDate.Name = "dtp_DokDate";
      tbx_DokDate.JAM_Highlighted = true;
            
      if(this is VirmanDUC) dtp_DokDate.ValueChanged += new EventHandler(dtp_DokDate_ValueChanged_SetAllRowDates);
   }
   private void InitializeHamper_napomena(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", TheTabControl.TabPages[0], false);

      if     (this is RasterDUC   || this is RasterBDUC   ) hamper.VvColWdt = new int[] { ZXC.Q4un, hamp_dokDate.Width + hamp_dokNum.Width - ZXC.Q4un - ZXC.Qun2 + hamp_intA.Width + hamp_dateA.Width };
      else if(this is PutNalDUC   || this is InterniRvrDUC) hamper.VvColWdt = new int[] { ZXC.Q4un, hamp_dokDate.Width + hamp_dokNum.Width - ZXC.Qun2 - ZXC.Q4un };
      else if(this is ZahtjeviDUC || 
              this is SmdDUC      ||
              this is EvidencijaDUC)                        hamper.VvColWdt = new int[] { ZXC.Q5un, hamp_dokDate.Width + hamp_dokNum.Width - ZXC.Qun2 - ZXC.Q4un - ZXC.QUN }; //new int[] { ZXC.Q5un, hamp_dokDate.Width + hamp_dokNum.Width + hamp_tt.Width - ZXC.Q5un- ZXC.Qun4};
      else if(this is GFI_TSI_DUC  )                        hamper.VvColWdt = new int[] { ZXC.Q5un, hamp_dokDate.Width + hamp_dokNum.Width + hamp_tt.Width - ZXC.Q4un - ZXC.Qun4 - ZXC.QUN };
      else if(this is KnjigaGostijuDUC)                     hamper.VvColWdt = new int[] { ZXC.Q5un, ZXC.Q10un*3 + ZXC.Q3un - ZXC.Qun4};
      else if(this is UrudzbeniDUC    )                     hamper.VvColWdt = new int[] { ZXC.Q6un, ZXC.Q10un * 3 - ZXC.Q2un };
      else if(this is UgovoriDUC      )                     hamper.VvColWdt = new int[] { ZXC.Q6un, ZXC.Q10un * 3 - ZXC.Qun2 };
      else if(this is RegistarCijeviDUC)                    hamper.VvColWdt = new int[] { ZXC.Q4un, ZXC.Q10un * 3 + ZXC.Q5un - ZXC.Qun2};
      else if(this is ZahtjevRNMDUC)                        hamper.VvColWdt = new int[] { ZXC.Q4un, ZXC.Q10un * 2 + ZXC.Q3un + ZXC.Qun2};
      else if(this is ZLJ_DUC || this is ZPG_DUC)           hamper.VvColWdt = new int[] { ZXC.Q5un, ZXC.Q10un * 3 + ZXC.Q5un + ZXC.Qun8};
      else if(this is BmwDUC                    )           hamper.VvColWdt = new int[] { ZXC.Q6un, ZXC.Q10un * 4 + ZXC.Q7un + ZXC.Qun8};
      else if(this is NazivArtiklaZaKupcaDUC    )           hamper.VvColWdt = new int[] { ZXC.Q4un, ZXC.Q10un     + ZXC.Q6un           };
      else if(this is KDCDUC              )           hamper.VvColWdt = new int[] { ZXC.Q4un, ZXC.Q10un*3   + ZXC.Q9un           };
      else                                                  hamper.VvColWdt = new int[] { ZXC.Q4un, hamp_dokDate.Width + hamp_dokNum.Width + hamp_tt.Width - ZXC.Q4un - ZXC.Qun4 };

      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                     hamper.CreateVvLabel(0, 0, "Napomena:", ContentAlignment.MiddleRight);
      tbx_Napomena = hamper.CreateVvTextBox(1, 0, "tbx_napomena", "Napomena", GetDB_ColumnSize(DB_ci.napomena));
      tbx_Napomena.Font = ZXC.vvFont.SmallFont;
      if(this is RasterDUC == false && this is RasterBDUC == false && this is KnjigaGostijuDUC == false) this.ControlForInitialFocus = tbx_Napomena;
   }
   private void InitializeHamper_tt(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false);

           if(this is PutNalDUC || this is ZahtjeviDUC || this is SmdDUC || this is EvidencijaDUC || this is InterniRvrDUC) hamper.VvColWdt = new int[] { ZXC.Q3un + ZXC.Qun4, ZXC.Q3un - ZXC.Qun2, ZXC.Q8un           , ZXC.Q3un + ZXC.Qun4 };
      else if(this is RasterDUC || this is RasterBDUC                                                                     ) hamper.VvColWdt = new int[] { ZXC.Q3un + ZXC.Qun2, ZXC.Q3un - ZXC.Qun2, ZXC.Q8un           , ZXC.Q3un + ZXC.Qun4 };
      else if(this is GFI_TSI_DUC                                                                                         ) hamper.VvColWdt = new int[] { ZXC.Q3un           , ZXC.Q3un - ZXC.Qun2, ZXC.Q8un           , ZXC.Q3un + ZXC.Qun4 };
      else if(this is KnjigaGostijuDUC                                                                                    ) hamper.VvColWdt = new int[] { ZXC.Q3un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun2, ZXC.Q4un           , ZXC.Q2un            };
      else if(this is UrudzbeniDUC || this is UgovoriDUC                                                                  ) hamper.VvColWdt = new int[] { ZXC.Q6un           , ZXC.Q3un - ZXC.Qun2, ZXC.Q6un           , ZXC.Q2un            };
      else if(this is RegistarCijeviDUC                                                                                   ) hamper.VvColWdt = new int[] { ZXC.Q4un           , ZXC.Q3un - ZXC.Qun2, ZXC.Q6un + ZXC.Qun4, ZXC.Q3un            };
      else if(this is ZPG_DUC || this is ZLJ_DUC                                                                          ) hamper.VvColWdt = new int[] { ZXC.Q5un           , ZXC.Q3un - ZXC.Qun2, ZXC.Q8un           , ZXC.Q3un            };
      else if(this is BmwDUC                                                                                              ) hamper.VvColWdt = new int[] { ZXC.Q4un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun2, ZXC.Q6un + ZXC.Qun2, ZXC.Q2un            };
      else if(this is KDCDUC                                                                                              ) hamper.VvColWdt = new int[] { ZXC.Q4un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un           , ZXC.Q3un            };
      else                                                                                                                  hamper.VvColWdt = new int[] { ZXC.Q4un           , ZXC.Q3un - ZXC.Qun2, ZXC.Q6un - ZXC.Qun2, ZXC.Q3un + ZXC.Qun4 };

      hamper.VvSpcBefCol    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN + ZXC.Qun8 };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      string text = (this is RegistarCijeviDUC) ? "Broj cijevi:" : "TipTrans:";

                   hamper.CreateVvLabel(0, 0, text, ContentAlignment.MiddleRight);
      tbx_TT     = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_tt", "Tip transakcije - racuna");
      tbx_TtOpis = hamper.CreateVvTextBox(2, 0, "tbx_ttOpis", "");
      tbx_TtNum  = hamper.CreateVvTextBox(3, 0, "tbx_ttNum", "Ovo bolje ostavi kako je...", 6);
      tbx_TT.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_TtOpis.JAM_ReadOnly = true;

      tbx_TtNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

           if(this is ZahtjeviDUC)        tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaMixTypeZahtjev   , (int)ZXC.Kolona.prva);
      else if(this is EvidencijaDUC)      tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaMixTypeEvidencija, (int)ZXC.Kolona.prva);
      else if(this is GFI_TSI_DUC)        tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaMix_GFI_TSI      , (int)ZXC.Kolona.prva);
      else if(this is Statistika_NPF_DUC) tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaMix_Statist_NPF  , (int)ZXC.Kolona.prva);
      else if(this is PlanDUC   )         tbx_TT.JAM_Set_LookUpTable(ZXC.luiLista_PlanTT          , (int)ZXC.Kolona.prva);
      else                                tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaMixerType        , (int)ZXC.Kolona.prva);

      if(this is ZahtjeviDUC || this is EvidencijaDUC || this is GFI_TSI_DUC || this is PlanDUC|| this is Statistika_NPF_DUC) tbx_TT.JAM_ReadOnly = false;
      else                                                                                                                    tbx_TT.JAM_ReadOnly = true;

      tbx_TT.JAM_lui_NameTaker_JAM_Name = tbx_TtOpis.JAM_Name;
      tbx_TT.JAM_ttNumTaker_JAM_Name   = tbx_TtNum.JAM_Name;

      tbx_TT.JAM_Highlighted = true;
      tbx_TtNum.JAM_Highlighted = true;
      tbx_TT.Font = ZXC.vvFont.LargeBoldFont;
      tbx_TtNum.Font = ZXC.vvFont.BaseBoldFont;
      tbx_TtNum.JAM_IsSupressTab = true;

      if(this is KDCDUC)
      {
         tbx_TT    .JAM_ReadOnly = 
         tbx_TtNum .JAM_ReadOnly = 
         tbx_TtOpis.JAM_ReadOnly = true;
      }


   }
   private void InitializeHamper_dokNum(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", TheTabControl.TabPages[0], false);

           if(this is ZahtjeviDUC || this is SmdDUC || this is EvidencijaDUC || this is GFI_TSI_DUC) hamper.VvColWdt = new int[] { ZXC.Q5un, ZXC.Q3un };
      else if(this is KnjigaGostijuDUC)                                                              hamper.VvColWdt = new int[] { ZXC.Q5un, ZXC.Q3un };
      else if(this is KDCDUC    )                                                              hamper.VvColWdt = new int[] { ZXC.Q3un, ZXC.Q3un };
      else                                                                                           hamper.VvColWdt = new int[] { ZXC.Q4un, ZXC.Q3un };

      hamper.VvSpcBefCol = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "IntBroj:", ContentAlignment.MiddleRight);

      tbx_DokNum = hamper.CreateVvTextBox(1, 0, "tbx_dokNum", "Ovo bolje ostavi kako je...", 6);
      tbx_DokNum.JAM_FillCharacter = '0';
      tbx_DokNum.JAM_ReadOnly = true;
   }
   private void InitializeHamper_IntA(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", TheTabControl.TabPages[0], false);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q2un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt        = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow     = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      string lblText    = (this is RasterDUC || this is RasterBDUC) ? "Rok plaćanja:" : "Offset:";
      string statusText = (this is RasterDUC || this is RasterBDUC) ? "Rok plaćanja u danima" : "Pomak kolone za upis podataka tekuće godine u odnosu na AOP kolonu";

                 hamper.CreateVvLabel(0, 0, lblText, ContentAlignment.MiddleRight);
      tbx_intA = hamper.CreateVvTextBox(1, 0, "tbx_RokPlac", statusText, GetDB_ColumnSize(DB_ci.intA));
      tbx_intA.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      if(this is RasterDUC || this is RasterBDUC)
      {
         tbx_intA.JAM_FieldExitMethod = new EventHandler(RokPlExitMethod);
         this.ControlForInitialFocus = tbx_intA;
      }
      hamper.Visible = false;

   }
   private void InitializeHamper_DateA(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", TheTabControl.TabPages[0], false);

      hamper.VvColWdt = new int[] { ZXC.Q5un, ZXC.Q4un };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      string text = (this is RasterDUC || this is RasterBDUC) ? "Dospijeće Plaćanja:" : "Kraj razdoblja:";

      hamper.CreateVvLabel(0, 0, text, ContentAlignment.MiddleRight);
      tbx_dateA = hamper.CreateVvTextBox(1, 0, "tbx_dateA", "Datum dospijeća računa");
      tbx_dateA.JAM_IsForDateTimePicker = true;
      dtp_dateA = hamper.CreateVvDateTimePicker(1, 0, "", tbx_dateA);
      dtp_dateA.Name = "dtp_dateA";

      if(this is RasterDUC || this is RasterBDUC) dtp_dateA.Leave += new EventHandler(DospDateExitMethod);
      hamper.Visible = false;
   }
   private void InitializeHamper_VezaPrjCd(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 4, "", TheTabControl.TabPages[0], false);

      hamper.VvColWdt = new int[] { ZXC.Q3un + ZXC.Qun4, ZXC.Q3un - ZXC.Qun2, ZXC.Q8un + ZXC.Qun2, ZXC.Q3un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Veza1:", ContentAlignment.MiddleRight);
      tbx_v1_tt = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_v1_tt", "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v1_tt));
      tbx_v1_ttOpis = hamper.CreateVvTextBox(2, 0, "tbx_v1_ttOpis", "Veza na interni dokument", 32);
      tbx_v1_ttNum = hamper.CreateVvTextBox(3, 0, "tbx_v1_ttNum", "Veza na interni dokument", 6/*GetDB_ColumnSize(DB_ci.iz_ttNum)*/);

      btn_v1TT = hamper.CreateVvButton(4, 0, new EventHandler(GoTo_MIXER_Dokument_Click), "");


      hamper.CreateVvLabel(0, 1, "Veza2:", ContentAlignment.MiddleRight);
      tbx_v2_tt = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_v2_tt", "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v2_tt));
      tbx_v2_ttOpis = hamper.CreateVvTextBox(2, 1, "tbx_v2_ttOpis", "Veza na interni dokument");
      tbx_v2_ttNum = hamper.CreateVvTextBox(3, 1, "tbx_v2_ttNum", "Veza na interni dokument" + " broj", 6/*GetDB_ColumnSize(DB_ci.na_ttNum)*/);

      btn_v2TT = hamper.CreateVvButton(4, 1, new EventHandler(GoTo_MIXER_Dokument_Click), "");

      hamper.CreateVvLabel(0, 2, "Projekt:", ContentAlignment.MiddleRight);
      tbx_ProjektCD = hamper.CreateVvTextBox(1, 2, "tbx_Projekt", "Projekt - ", GetDB_ColumnSize(DB_ci.projektCD), 1, 0);
      btn_proj = hamper.CreateVvButton(4, 2, new EventHandler(/*FakturDUC.*/GoToProjektCD_RISK_Dokument_Click), "");

      hamper.CreateVvLabel(0, 2, "Link1:", ContentAlignment.MiddleRight);
      tbx_externLink1 = hamper.CreateVvTextBox(1, 2, "tbx_externLink1", "Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.externLink1));

      btn_goExLink1 = hamper.CreateVvButton(2, 2, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink1.Name = "btn_goExLink1";
      btn_goExLink1.FlatStyle = FlatStyle.Flat;
      btn_goExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink1.Image = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink1.Tag = 1;
      btn_goExLink1.TabStop = false;
      btn_goExLink1.Visible = false;

      btn_openExLink1 = hamper.CreateVvButton(3, 2, new EventHandler(Show_ExternDokument_Click), "");

      hamper.CreateVvLabel(0, 2, "Link2:", ContentAlignment.MiddleRight);
      tbx_externLink2 = hamper.CreateVvTextBox(1, 2, "tbx_externLink2", "Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.externLink2));

      btn_goExLink2 = hamper.CreateVvButton(2, 2, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink2.Name = "btn_goExLink2";
      btn_goExLink2.FlatStyle = FlatStyle.Flat;
      btn_goExLink2.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink2.Image = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink2.Tag = 2;
      btn_goExLink2.TabStop = false;
      btn_goExLink2.Visible = false;

      btn_openExLink2 = hamper.CreateVvButton(3, 2, new EventHandler(Show_ExternDokument_Click), "");

      hamper.Visible = false;
   }
   private void InitializeHamper_mjTroska(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false, hamp_napomena.Right, hamp_napomena.Top, razmakHamp);
      //                                     0        1           2        3        4  
      if(this is RvrMjesecDUC) hamper.VvColWdt = new int[] { ZXC.Q4un, ZXC.Q3un - ZXC.Qun4, ZXC.Q3un - ZXC.Qun4, ZXC.Q10un - ZXC.Qun4 };
      else                     hamper.VvColWdt = new int[] { ZXC.Q3un + ZXC.Qun2, ZXC.Q3un + ZXC.Qun4, ZXC.Q3un, ZXC.Q7un + ZXC.Qun2 };
      
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Mjesto troška:", ContentAlignment.MiddleRight);
      tbx_mtros_cd = hamper.CreateVvTextBox(1, 0, "tbx_mtros_cd", "Šifra mjesta troška", GetDB_ColumnSize(DB_ci.mtrosCD));
      tbx_mtros_tk = hamper.CreateVvTextBox(2, 0, "tbx_mtros_tk", "Tiker mjesta troška", GetDB_ColumnSize(DB_ci.mtrosTK));
      tbx_mtros_Naziv = hamper.CreateVvTextBox(3, 0, "tbx_mtros_naziv", "Naziv mjesta troška");

      tbx_mtros_cd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_mtros_tk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_mtros_cd.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_mtros_tk.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_mtros_Naziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyMtrosTextBoxLeave));

      hamper.Visible = false;
   }

   public void RokPlExitMethod(object sender, EventArgs e)
   {
      if(Fld_IntA.NotZero()) Fld_DateA = Fld_DokDate + new TimeSpan(Fld_IntA, 0, 0, 0);
      else Fld_DateA = Fld_DokDate;
   }
   public void DospDateExitMethod(object sender, EventArgs e)
   {
      TimeSpan timeSpan;

      if(Fld_DateA >= DateTimePicker.MinimumDateTime && Fld_DateA <= DateTimePicker.MaximumDateTime)
      {
         timeSpan = Fld_DateA - Fld_DokDate;
         Fld_IntA = (int)timeSpan.TotalDays;
      }
      else
      {
         Fld_IntA = 0;
      }
   }

   public void Link_ExternDokument_Click(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None ||
         TheVvTabPage.WriteMode == ZXC.WriteMode.Delete ||
         this.Visible == false) return;

      #region fieldz

      Button btn = sender as Button;
      int linkId = ZXC.ValOrZero_Int(btn.Tag.ToString()); //if(linkId != 1) throw new Exception("Link_ExternDokument_Click: linkId unknown! (" + linkId.ToString() + ")");

      //string thisDocumentID = mixer_rec.TipBr;
      string thisDocumentID = mixer_rec.TT + mixer_rec.TtNum.ToString();

      if(this is GFI_TSI_DUC) thisDocumentID = "xls";

      #endregion fieldz

      #region FileDialog

      OpenFileDialog openFileDialog = new OpenFileDialog();

      switch(linkId)
      {
         case 1: openFileDialog.InitialDirectory = ZXC.TheVvForm.VvPref.eksternLinks1.DirectoryName; break;
         case 2: openFileDialog.InitialDirectory = ZXC.TheVvForm.VvPref.eksternLinks2.DirectoryName; break;
      }

      // volia 
      Clipboard.SetText(thisDocumentID); // !!! 

      //openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
      //openFileDialog.Filter = "(*" + thisDocumentID + "*)|" + "*" + thisDocumentID + "*" + "|All files (*.*)|*.*";
      openFileDialog.Filter = "Datoteke " + thisDocumentID + "|" + "*" + thisDocumentID + "*" + "|Sve datoteke (*.*)|*.*";
      openFileDialog.FilterIndex = 1;
      openFileDialog.RestoreDirectory = true;


      #endregion FileDialog

      if(openFileDialog.ShowDialog() == DialogResult.OK)
      {
         string fullPathName = openFileDialog.FileName;
         System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(fullPathName);

         string fileName = dInfo.Name;
         string directoryName = fullPathName.Substring(0, fullPathName.Length - (fileName.Length + 1));

         switch(linkId)
         {
            case 1: ZXC.TheVvForm.VvPref.eksternLinks1.DirectoryName = directoryName; Fld_ExternLink1 = fullPathName; break;
            case 2: ZXC.TheVvForm.VvPref.eksternLinks2.DirectoryName = directoryName; Fld_ExternLink2 = fullPathName; break;
         }
      }

      openFileDialog.Dispose();
   }
   public void Show_ExternDokument_Click(object sender, EventArgs e)
   {
      #region fieldz

      Button btn = sender as Button;
      int linkId = ZXC.ValOrZero_Int(btn.Tag.ToString()); //if(linkId != 1) throw new Exception("Link_ExternDokument_Click: linkId unknown! (" + linkId.ToString() + ")");

      string fullPathFileName = "";

      #endregion fieldz

      switch(linkId)
      {
         case 1: fullPathFileName = Fld_ExternLink1; break;
         case 2: fullPathFileName = Fld_ExternLink2; break;
      }

      if(fullPathFileName.IsEmpty()) return;

      // here we go 
      try
      {
         System.Diagnostics.Process.Start(fullPathFileName);
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, ex.Message);
      }
   }

   public void AnyMtrosTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      //if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_MtrosCD = kupdob_rec.KupdobCD/*RecID*/;
            Fld_MtrosTK = kupdob_rec.Ticker;
            Fld_MtrosName = kupdob_rec.Naziv;
         }
         else
         {
            Fld_MtrosCDAsTxt = Fld_MtrosTK = Fld_MtrosName = "";
         }
      }
   }

   public void AnyMtrosTextBoxLeave_Grid(object sender, EventArgs e)
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
            TheG.PutCell(ci.iT_kpdbMjestoA_32, currRow, kupdob_rec.Ticker);
            TheG.PutCell(ci.iT_kupdobCD      , currRow, kupdob_rec.KupdobCD/*RecID*/);
         }
         else
         {
            TheG.PutCell(ci.iT_kpdbMjestoA_32, currRow, "");
            TheG.PutCell(ci.iT_kupdobCD      , currRow, "");
         }
      }

   }

   #endregion Hampers

   #region TheGrid_Columns

   #region klasik

   protected void T_moneyA_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_moneyA = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_moneyA", TheVvDaoTrans, DB_Tci.t_moneyA, _statusText);

      vvtbT_moneyA.JAM_ShouldSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_moneyA, TheVvDaoTrans, DB_Tci.t_moneyA, _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }

   protected void T_kol_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_kol = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_kol", TheVvDaoTrans, DB_Tci.t_kol, _statusText);

      vvtbT_kol.JAM_ShouldSumGrid = true;
      //if(this is RasterDUC) vvtbT_kol.JAM_ShouldCopyPrevRow = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_kol, TheVvDaoTrans, DB_Tci.t_kol, _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth

      string colA = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_kol);

      if(this is PutNalDUC)
      {
         vvtbT_opis_128.JAM_lui_NumberTaker_JAM_Name/*T*/ = colA;
      }
   }

   protected void T_dateOd_CreateColumn(int _width, string _colHeader)
   {
      VvTextBox vvtb = new VvTextBox();

      if(this is VirmanDUC) vvtb.JAM_ShouldCopyPrevRow = true;

      vvtb.JAM_IsForDateTimePicker_WithTimeDisplay = false;

      colDate1 = TheG.CreateCalendarColumn(TheVvDaoTrans, DB_Tci.t_dateOd, _colHeader, _width);
      colDate1.Tag = vvtb;

   }

   protected void T_dateOd_WithTime_CreateColumn(int _width, string _colHeader)
   {
      VvTextBox vvtb = new VvTextBox();
      vvtb.JAM_IsForDateTimePicker_WithTimeDisplay = true;

      colDate1wT = TheG.CreateCalendarAnd_TIMEColumn(TheVvDaoTrans, DB_Tci.t_dateOd, _colHeader, _width);
      colDate1wT.Tag = vvtb;
   }

   protected void T_dateOd_OnlyTime_CreateColumn(int _width, string _colHeader)
   {
      VvTextBox vvtb = new VvTextBox();
      vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true;

      colDate1onlyT = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dateOd, _colHeader, _width);
      colDate1onlyT.Tag = vvtb;

   }

   protected void T_dateDo_CreateColumn(int _width, string _colHeader)
   {
      VvTextBox vvtb = new VvTextBox();
      if(this is VirmanDUC) vvtb.JAM_ShouldCopyPrevRow = true;

      vvtb.JAM_IsForDateTimePicker_WithTimeDisplay = false;

      colDate2 = TheG.CreateCalendarColumn(TheVvDaoTrans, DB_Tci.t_dateDo, _colHeader, _width);
      colDate2.Tag = vvtb;
   }

   protected void T_dateDo_WithTime_CreateColumn(int _width, string _colHeader)
   {
      VvTextBox vvtb = new VvTextBox();
      vvtb.JAM_ShouldCopyPrevRow = true;

      vvtb.JAM_IsForDateTimePicker_WithTimeDisplay = true;

      colDate2wT = TheG.CreateCalendarAnd_TIMEColumn(TheVvDaoTrans, DB_Tci.t_dateDo, _colHeader, _width);
      colDate2wT.Tag = vvtb;
    
   }

   protected void T_dateDo_OnlyTime_CreateColumn(int _width, string _colHeader)
   {
      VvTextBox vvtb = new VvTextBox();

      vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true;

      colDate2onlyT = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dateDo, _colHeader, _width);
      colDate2onlyT.Tag = vvtb;

   }
   protected void T_opis_128_CreateColumn(int _width, string _colHeader, string _statusText, int maxLength, VvLookUpLista _lookUpList)
   {

      if(_lookUpList == null)
      {
         vvtbT_opis_128 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_opis_128", TheVvDaoTrans, maxLength.IsNegative() ? maxLength : DB_Tci.t_opis_128, _statusText);
      }
      else
      {
         vvtbT_opis_128 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_opis_128", TheVvDaoTrans, maxLength.IsNegative() ? maxLength : DB_Tci.t_opis_128, _statusText);
         vvtbT_opis_128.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.druga);
         vvtbT_opis_128.JAM_lookUp_NOTobligatory = true;
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_opis_128, TheVvDaoTrans, DB_Tci.t_opis_128, _colHeader, _width);
      if(_width.IsZero())
      {
         colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
         colVvText.MinimumWidth = ZXC.Q7un;
      }

      if(this is PutNalDUC && _lookUpList != null)
      {
         vvtbT_opis_128.JAM_FieldExitMethod_2 = new EventHandler(OnExit_Relacija_RaiseKmChanged);
      }

   }

   protected void T_kpdbNameA_50_CreateColumn(int _width, string _colHeader, string _statusText, bool shouldCopyPrevRow)
   {
      vvtbT_kpdbNameA_50 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_kpdbNameA_50", TheVvDaoTrans, DB_Tci.t_kpdbNameA_50, _statusText);
      if(shouldCopyPrevRow) vvtbT_kpdbNameA_50.JAM_ShouldCopyPrevRow = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_kpdbNameA_50, TheVvDaoTrans, DB_Tci.t_kpdbNameA_50, _colHeader, _width);

      if(this is RasterDUC || this is RasterBDUC) vvtbT_kpdbNameA_50.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyKupdobTextBoxLeave));
      if(this is VirmanDUC                      ) vvtbT_kpdbNameA_50.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyKupdobTextBoxLeave_A));
      if(this is RvrMjesecDUC                   ) colVvText.Frozen = true;
   }

   protected void T_kpdbUlBrA_32_CreateColumn(int _width, string _colHeader, string _statusText, int maxLength)
   {
      vvtbT_kpdbUlBrA_32 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_kpdbUlBrA_32", TheVvDaoTrans, maxLength.IsNegative() ? maxLength : DB_Tci.t_kpdbUlBrA_32, _statusText);

      //08.04.2022.
      if(this is KDCDUC == false) vvtbT_kpdbUlBrA_32.JAM_ShouldCopyPrevRow = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_kpdbUlBrA_32, TheVvDaoTrans, DB_Tci.t_kpdbUlBrA_32, _colHeader, _width);

      if(this is RasterDUC || this is RasterBDUC) vvtbT_kpdbUlBrA_32.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
   }

   protected void T_kpdbMjestoA_32_CreateColumn(int _width, string _colHeader, string _statusText)
   {
      vvtbT_kpdbMjestoA_32 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_kpdbMjestoA_32", TheVvDaoTrans, DB_Tci.t_kpdbMjestoA_32, _statusText);
      vvtbT_kpdbMjestoA_32.JAM_ShouldCopyPrevRow = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_kpdbMjestoA_32, TheVvDaoTrans, DB_Tci.t_kpdbMjestoA_32, _colHeader, _width);

      if(this is PlanDUC) colVvText.Visible = (kshema.Dsc_IsPlanViaMtros);

   }

   protected void T_kpdbZiroA_32_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList)
   {
      if(this is PlanDUC) VvLookUpLista.LoadResultLuiList_PozicijePlana_PLN_or_RLZ(/* isPLN */ true);
      
      if(_lookUpList == null)
      {
         vvtbT_kpdbZiroA_32 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_kpdbZiroA_32", TheVvDaoTrans, DB_Tci.t_kpdbZiroA_32, _statusText);
      }
      else
      {
         vvtbT_kpdbZiroA_32 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_kpdbZiroA_32", TheVvDaoTrans, DB_Tci.t_kpdbZiroA_32, _statusText);
         //vvtbT_kpdbZiroA_32.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.druga);
         vvtbT_kpdbZiroA_32.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
         if(this is PlanDUC)
         {
            vvtbT_kpdbZiroA_32.JAM_lookUp_NOTobligatory = false;
         }
         else
         {
            vvtbT_kpdbZiroA_32.JAM_lookUp_NOTobligatory = true;
         }
      }

    //if(this is PlanDUC == false) vvtbT_kpdbZiroA_32.JAM_ShouldCopyPrevRow = true;
      if(this is PlanDUC == false && this is KDCDUC == false) vvtbT_kpdbZiroA_32.JAM_ShouldCopyPrevRow = true;
      
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_kpdbZiroA_32, TheVvDaoTrans, DB_Tci.t_kpdbZiroA_32, _colHeader, _width);
   }

   protected void T_kpdbNameB_50_CreateColumn(int _width, string _colHeader, string _statusText)
   {
      vvtbT_kpdbNameB_50 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_kpdbNameB_50", TheVvDaoTrans, DB_Tci.t_kpdbNameB_50, _statusText);
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_kpdbNameB_50, TheVvDaoTrans, DB_Tci.t_kpdbNameB_50, _colHeader, _width);

      if(this is RasterDUC || this is RasterBDUC) vvtbT_kpdbNameB_50.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyKupdobTextBoxLeave));
      if(this is VirmanDUC) vvtbT_kpdbNameB_50.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyKupdobTextBoxLeave_B));
   }

   protected void T_kpdbUlBrB_32_CreateColumn(int _width, string _colHeader, string _statusText)
   {
      vvtbT_kpdbUlBrB_32 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_kpdbUlBrB_32", TheVvDaoTrans, DB_Tci.t_kpdbUlBrB_32, _statusText);
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_kpdbUlBrB_32, TheVvDaoTrans, DB_Tci.t_kpdbUlBrB_32, _colHeader, _width);
   }

   protected void T_kpdbMjestoB_32_CreateColumn(int _width, string _colHeader, string _statusText)
   {
      vvtbT_kpdbMjestoB_32 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_kpdbMjestoB_32", TheVvDaoTrans, DB_Tci.t_kpdbMjestoB_32, _statusText);
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_kpdbMjestoB_32, TheVvDaoTrans, DB_Tci.t_kpdbMjestoB_32, _colHeader, _width);
   }

   protected void T_kpdbZiroB_32_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList)
   {
      if(_lookUpList == null)
      {
         vvtbT_kpdbZiroB_32 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_kpdbZiroB_32", TheVvDaoTrans, DB_Tci.t_kpdbZiroB_32, _statusText);
      }
      else
      {
         vvtbT_kpdbZiroB_32 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_kpdbZiroB_32", TheVvDaoTrans, DB_Tci.t_kpdbZiroB_32, _statusText);
         vvtbT_kpdbZiroB_32.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_kpdbZiroB_32, TheVvDaoTrans, DB_Tci.t_kpdbZiroB_32, _colHeader, _width);
   }

   protected void T_strA_2_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList)
   {
      if(_lookUpList == null)
      {
         vvtbT_strA_2 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_strA_2", TheVvDaoTrans, DB_Tci.t_strA_2, _statusText);
      }
      else
      {
         vvtbT_strA_2 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_strA_2", TheVvDaoTrans, DB_Tci.t_strA_2, _statusText);
         vvtbT_strA_2.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_strA_2, TheVvDaoTrans, DB_Tci.t_strA_2, _colHeader, _width);

   }

   protected void T_strB_2_CreateColumn(int _width, string _colHeader, string _statusText)
   {
      vvtbT_strB_2 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_strB_2", TheVvDaoTrans, DB_Tci.t_strB_2, _statusText);
      if((this is VirmanDUC)== false) vvtbT_strB_2.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_strB_2, TheVvDaoTrans, DB_Tci.t_strB_2, _colHeader, _width);
   }

   protected void T_vezniDokA_64_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList)
   {
      if(_lookUpList == null)
      {
         vvtbT_vezniDokA_64 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_vezniDokA_64", TheVvDaoTrans, DB_Tci.t_vezniDokA_64, _statusText);
      }
      else
      {
         vvtbT_vezniDokA_64 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtbT_vezniDokA_64", TheVvDaoTrans, DB_Tci.t_vezniDokA_64, _statusText);
         vvtbT_vezniDokA_64.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.druga);
         vvtbT_vezniDokA_64.JAM_lookUp_NOTobligatory = true;

      }
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_vezniDokA_64, TheVvDaoTrans, DB_Tci.t_vezniDokA_64, _colHeader, _width);
   }

   protected void T_vezniDokB_64_CreateColumn(int _width, string _colHeader, string _statusText)
   {
      vvtbT_vezniDokB_64 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_vezniDokB_64", TheVvDaoTrans, DB_Tci.t_vezniDokB_64, _statusText);
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_vezniDokB_64, TheVvDaoTrans, DB_Tci.t_vezniDokB_64, _colHeader, _width);
   }

   protected void T_strC_2_CreateColumn(int _width, string _colHeader, string _statusText)
   {
      vvtbT_strC_2 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_strC_2", TheVvDaoTrans, DB_Tci.t_strC_2, _statusText);
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_strC_2, TheVvDaoTrans, DB_Tci.t_strC_2, _colHeader, _width);
   }

   protected void T_kupdobCD_CreateColumn(int _width, string _colHeader, string _statusText)
   {
      vvtbT_kupdobCD = TheG.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColT_kupdobCD", TheVvDaoTrans, DB_Tci.t_kupdobCD, _statusText);
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_kupdobCD, TheVvDaoTrans, DB_Tci.t_kupdobCD, _colHeader, _width);

      if(this is RasterDUC || this is RasterBDUC) vvtbT_kupdobCD.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupdobTextBoxLeave));

      if(this is PlanDUC) colVvText.Visible = (kshema.Dsc_IsPlanViaMtros);

   }

   protected void T_artiklCD_CreateColumn(int _width, string _colHeader, string _statusText)
   {
      vvtbT_artiklCD = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_artiklCD", TheVvDaoTrans, DB_Tci.t_artiklCD, _statusText);
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_artiklCD, TheVvDaoTrans, DB_Tci.t_artiklCD, _colHeader, _width);

      if(this is PmvDUC)
         vvtbT_artiklCD.JAM_SetAutoCompleteData(Artikl.recordName, Artikl.sorterCD.SortType, ZXC.AutoCompleteRestrictor.ART_MOT_VOZILO_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Artikl_sorterSifra), new EventHandler(AnyArtiklTextBox_OnGrid_Leave));
      else
         vvtbT_artiklCD.JAM_SetAutoCompleteData(Artikl.recordName, Artikl.sorterCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Artikl_sorterSifra), new EventHandler(AnyArtiklTextBox_OnGrid_Leave));

   }

   protected void T_artiklName_CreateColumn(int _width, string _colHeader, string _statusText)
   {
      vvtbT_artiklName = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_artiklName", TheVvDaoTrans, DB_Tci.t_artiklName, _statusText);
      if(this is PmvDUC)
         vvtbT_artiklName.JAM_SetAutoCompleteData(Artikl.recordName, Artikl.sorterName.SortType, ZXC.AutoCompleteRestrictor.ART_MOT_VOZILO_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Artikl_sorterName), new EventHandler(AnyArtiklTextBox_OnGrid_Leave));
      else
         vvtbT_artiklName.JAM_SetAutoCompleteData(Artikl.recordName, Artikl.sorterName.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Artikl_sorterName), new EventHandler(AnyArtiklTextBox_OnGrid_Leave));

      if(this is RasterDUC)   vvtbT_artiklName.JAM_ShouldCopyPrevRowUnCond = true;
      //else                    vvtbT_artiklName.JAM_ShouldCopyPrevRow = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_artiklName, TheVvDaoTrans, DB_Tci.t_artiklName, _colHeader, _width);
      if(_width.IsZero())
      {
         colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
         colVvText.MinimumWidth = ZXC.Q5un;
      }
   }

   protected void T_isXxx_CreateColumn(int _width, string _colHeader)
   {
      vvcbx_isXxx = new VvCheckBox();
      colCbox = TheG.CreateVvCheckBoxColumn(vvcbx_isXxx, TheVvDaoTrans, DB_Tci.t_isXxx, _colHeader, _width);
   }

   protected void T_intA_CreateColumn(int _width, string _colHeader, string _statusText)
   {
      vvtbT_intA = TheG.CreateVvTextBoxFor_Integer_ColumnTemplate(false, "vvtb4ColT_intA", TheVvDaoTrans, DB_Tci.t_intA, _statusText);

      //colVvText = TheG.CreateVvTextBoxColumn(vvtbT_intA, TheVvDaoTrans, DB_Tci.t_intA, _colHeader, _width);
      //colVvText.DefaultCellStyle.Alignment =
      //colVvText.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
      
      colVvTextInt = TheG.CreateVvTextBoxColumn(vvtbT_intA, TheVvDaoTrans, DB_Tci.t_intA, _colHeader, _width);
      colVvTextInt.DefaultCellStyle.Alignment =
      colVvTextInt.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

   
   }

   protected void T_intB_CreateColumn(int _width, string _colHeader, string _statusText)
   {
      vvtbT_intB = TheG.CreateVvTextBoxFor_Integer_ColumnTemplate(false, "vvtb4ColT_intB", TheVvDaoTrans, DB_Tci.t_intB, _statusText);

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_intB, TheVvDaoTrans, DB_Tci.t_intB, _colHeader, _width);

      colVvText.DefaultCellStyle.Alignment =
      colVvText.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

   }

   protected void T_konto_CreateColumn(int _width, string _colHeader, string _statusText)
   {
      vvtbT_konto = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_konto", TheVvDaoTrans, DB_Tci.t_konto, _statusText);
      vvtbT_konto.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      vvtbT_konto.JAM_SetAutoCompleteData(Kplan.recordName, VvSQL.SorterType.KontoNaziv, ZXC.AutoCompleteRestrictor.KPL_Analitika_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_KplanNaziv_sorterCode), null);
      vvtbT_konto.JAM_FieldExitMethod = new EventHandler(OnExitKonto_ClearPreffix);

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_konto, TheVvDaoTrans, DB_Tci.t_konto, _colHeader, _width);
   }

   protected void T_personCD_CreateColumn(int _width, string _colHeader, string _statusText)
   {
      vvtbT_personCD = TheG.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColT_personCD", TheVvDaoTrans, /*DB_Tci.t_personCD*/ -4, _statusText);
      vvtbT_personCD.JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterSifra), new EventHandler(AnyPersonDgvTextBoxLeave));

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_personCD, TheVvDaoTrans, DB_Tci.t_personCD, _colHeader, _width);

      if(this is RvrMjesecDUC) colVvText.Frozen = true;
   }

   protected void R_iznos_CreateColumn(int _width, int numOfDecimalPlaces, string _colHeader, string _statusText)
   {
      vvtbT_KolMoneyA = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_KolMoneyA", null, -12, _statusText);
      vvtbT_KolMoneyA.JAM_ReadOnly = true;
      vvtbT_KolMoneyA.JAM_ShouldSumGrid = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_KolMoneyA, null, "R_KolMoneyA", _colHeader, _width);
      colVvText.MinimumWidth = _width;
   }

   protected void T_moneyB_CreateColumn(int _width, int numOfDecimalPlaces, string _colHeader, string _statusText, bool isReadOnly)
   {
      T_moneyB_CreateColumn(_width, numOfDecimalPlaces, _colHeader, _statusText, isReadOnly, true);
   }

   protected void T_moneyB_CreateColumn(int _width, int numOfDecimalPlaces, string _colHeader, string _statusText, bool isReadOnly, bool isVisible)
   {
      vvtbT_moneyB = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_moneyB", TheVvDaoTrans, DB_Tci.t_moneyB, _statusText);
      vvtbT_moneyB.JAM_ReadOnly = isReadOnly;

      vvtbT_moneyB.JAM_ShouldSumGrid = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_moneyB, TheVvDaoTrans, DB_Tci.t_moneyB, _colHeader, _width);

      colVvText.Visible = isVisible;

      colVvText.MinimumWidth = _width;
      if(this is GFI_TSI_DUC) colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;

      //if(this is PlanDUC)  colVvText.Visible = (kshema.Dsc_IsVisibleColFond);
         
   }

   protected void T_moneyC_CreateColumn(int _width, int numOfDecimalPlaces, string _colHeader, string _statusText, bool isReadOnly, bool shouldSumGrid)
   {
      vvtbT_moneyC = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_moneyC", TheVvDaoTrans, DB_Tci.t_moneyC, _statusText);
      vvtbT_moneyC.JAM_ReadOnly = isReadOnly;
      vvtbT_moneyC.JAM_ShouldSumGrid = shouldSumGrid;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_moneyC, TheVvDaoTrans, DB_Tci.t_moneyC, _colHeader, _width);
      colVvText.MinimumWidth = _width;
  
      //if(this is PlanDUC) colVvText.Visible = (kshema.Dsc_IsVisibleColFond);
   }

   protected void T_moneyD_CreateColumn(int _width, int numOfDecimalPlaces, string _colHeader, string _statusText)
   {
      vvtbT_moneyD = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_moneyD", TheVvDaoTrans, DB_Tci.t_moneyD, _statusText);
      vvtbT_moneyD.JAM_ReadOnly = true;
      vvtbT_moneyD.JAM_ShouldSumGrid = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_moneyD, TheVvDaoTrans, DB_Tci.t_moneyD, _colHeader, _width);
      colVvText.MinimumWidth = _width;
      if(this is PlanDUC && kshema.Dsc_IsVisibleColFond) vvtbT_moneyD.JAM_ReadOnly = false;

   }


  protected void R_MFS_CreateColumn(int _width, string _colHeader, string _statusText, int numOfDecimalPlaces)
   {
      vvtbT_MFS = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_MFS", null, -12, _statusText);
      vvtbT_MFS.JAM_ReadOnly      = true;
      vvtbT_MFS.JAM_ShouldSumGrid = true;
      vvtbT_MFS.JAM_ForeColor     = Color.DarkBlue;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_MFS, null, "R_MFS", _colHeader, _width);
      colVvText.MinimumWidth = _width;
   }
  protected void R_RFS_CreateColumn(int _width, string _colHeader, string _statusText, int numOfDecimalPlaces)
   {
      vvtbT_RFS = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_RFS", null, -12, _statusText);
      vvtbT_RFS.JAM_ReadOnly      = true;
      vvtbT_RFS.JAM_ShouldSumGrid = true;
      vvtbT_RFS.JAM_ForeColor = Color.Green;
      
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_RFS, null, "R_RFS", _colHeader, _width);
      colVvText.MinimumWidth = _width;
   }
  protected void R_PFS_CreateColumn(int _width, string _colHeader, string _statusText, int numOfDecimalPlaces)
   {
      vvtbT_PFS = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_PFS", null, -12, _statusText);
      vvtbT_PFS.JAM_ReadOnly      = true;
      vvtbT_PFS.JAM_ShouldSumGrid = true;
      vvtbT_PFS.JAM_ForeColor     = Color.Red;
      
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_PFS, null, "R_PFS", _colHeader, _width);
      colVvText.MinimumWidth = _width;
   }

  protected void R_SumMoney_CreateColumn(int _width, string _colHeader, string _statusText, int numOfDecimalPlaces)
  {
     vvtbR_SumMoney = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColR_SumMoney", null, -12, _statusText);
     vvtbR_SumMoney.JAM_ReadOnly = true;
     vvtbR_SumMoney.JAM_ForeColor = Color.Blue;

     colVvText = TheG.CreateVvTextBoxColumn(vvtbR_SumMoney, null, "R_SumMoney", _colHeader, _width);
     colVvText.MinimumWidth = _width;
     //if(this is PlanDUC) colVvText.Visible = (kshema.Dsc_IsVisibleColFond);

  }


   protected void T_date3_CreateColumn(int _width, string _colHeader)
   {
      VvTextBox vvtb = new VvTextBox();

      vvtb.JAM_IsForDateTimePicker_WithTimeDisplay = false;

      colDate3 = TheG.CreateCalendarColumn(TheVvDaoTrans, DB_Tci.t_date3, _colHeader, _width);
      colDate3.Tag = vvtb;

   }
   protected void T_date4_CreateColumn(int _width, string _colHeader)
   {
      VvTextBox vvtb = new VvTextBox();

      vvtb.JAM_IsForDateTimePicker_WithTimeDisplay = false;

      colDate4 = TheG.CreateCalendarColumn(TheVvDaoTrans, DB_Tci.t_date4, _colHeader, _width);
      colDate4.Tag = vvtb;

   }
   protected void T_date4_WithTime_CreateColumn(int _width, string _colHeader)
   {
      VvTextBox vvtb = new VvTextBox();
     
      vvtb.JAM_IsForDateTimePicker_WithTimeDisplay = true;

      colDate4wT = TheG.CreateCalendarAnd_TIMEColumn(TheVvDaoTrans, DB_Tci.t_date4, _colHeader, _width);
      colDate4wT.Tag = vvtb;

      if(this is KOP_PTG_DUC) vvtb.JAM_ReadOnly = true;
   }



   #endregion klasik

   #region dec01-31

   protected void T_dec01_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec01 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec01", TheVvDaoTrans, DB_Tci.t_dec01, _statusText);
      vvtbT_dec01.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec01, TheVvDaoTrans, DB_Tci.t_dec01, _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec02_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec02 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec02", TheVvDaoTrans, DB_Tci.t_dec02, _statusText);
      vvtbT_dec02.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec02, TheVvDaoTrans, DB_Tci.t_dec02, _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec03_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec03 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec03", TheVvDaoTrans, DB_Tci.t_dec03, _statusText);
      vvtbT_dec03.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec03, TheVvDaoTrans, DB_Tci.t_dec03, _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec04_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec04 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec04", TheVvDaoTrans, DB_Tci.t_dec04, _statusText);
      vvtbT_dec04.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec04, TheVvDaoTrans, DB_Tci.t_dec04 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec05_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec05 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec05",   TheVvDaoTrans, DB_Tci.t_dec05 , _statusText);
      vvtbT_dec05.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec05, TheVvDaoTrans, DB_Tci.t_dec05 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec06_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec06 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec06",   TheVvDaoTrans, DB_Tci.t_dec06 , _statusText);
      vvtbT_dec06.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec06, TheVvDaoTrans, DB_Tci.t_dec06 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec07_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec07 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec07",   TheVvDaoTrans, DB_Tci.t_dec07 , _statusText);
      vvtbT_dec07.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec07, TheVvDaoTrans, DB_Tci.t_dec07 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec08_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec08 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec08",   TheVvDaoTrans, DB_Tci.t_dec08 , _statusText);
      vvtbT_dec08.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec08, TheVvDaoTrans, DB_Tci.t_dec08 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec09_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec09 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec09",   TheVvDaoTrans, DB_Tci.t_dec09 , _statusText);
      vvtbT_dec09.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec09, TheVvDaoTrans, DB_Tci.t_dec09 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec10_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec10 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec10",   TheVvDaoTrans, DB_Tci.t_dec10 , _statusText);
      vvtbT_dec10.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec10, TheVvDaoTrans, DB_Tci.t_dec10 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec11_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec11 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec11",   TheVvDaoTrans, DB_Tci.t_dec11 , _statusText);
      vvtbT_dec11.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec11, TheVvDaoTrans, DB_Tci.t_dec11 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec12_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec12 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec12",   TheVvDaoTrans, DB_Tci.t_dec12 , _statusText);
      vvtbT_dec12.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec12, TheVvDaoTrans, DB_Tci.t_dec12 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec13_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec13 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec13",   TheVvDaoTrans, DB_Tci.t_dec13 , _statusText);
      vvtbT_dec13.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec13, TheVvDaoTrans, DB_Tci.t_dec13 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec14_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec14 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec14",   TheVvDaoTrans, DB_Tci.t_dec14 , _statusText);
      vvtbT_dec14.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec14, TheVvDaoTrans, DB_Tci.t_dec14 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec15_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec15 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec15",   TheVvDaoTrans, DB_Tci.t_dec15 , _statusText);
      vvtbT_dec15.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec15, TheVvDaoTrans, DB_Tci.t_dec15 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec16_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec16 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec16",   TheVvDaoTrans, DB_Tci.t_dec16 , _statusText);
      vvtbT_dec16.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec16, TheVvDaoTrans, DB_Tci.t_dec16 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec17_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec17 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec17",   TheVvDaoTrans, DB_Tci.t_dec17 , _statusText);
      vvtbT_dec17.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec17, TheVvDaoTrans, DB_Tci.t_dec17 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec18_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec18 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec18",   TheVvDaoTrans, DB_Tci.t_dec18 , _statusText);
      vvtbT_dec18.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec18, TheVvDaoTrans, DB_Tci.t_dec18 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec19_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec19 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec19",   TheVvDaoTrans, DB_Tci.t_dec19 , _statusText);
      vvtbT_dec19.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec19, TheVvDaoTrans, DB_Tci.t_dec19 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec20_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec20 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec20",   TheVvDaoTrans, DB_Tci.t_dec20 , _statusText);
      vvtbT_dec20.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec20, TheVvDaoTrans, DB_Tci.t_dec20 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec21_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec21 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec21",   TheVvDaoTrans, DB_Tci.t_dec21 , _statusText);
      vvtbT_dec21.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec21, TheVvDaoTrans, DB_Tci.t_dec21 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec22_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec22 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec22",   TheVvDaoTrans, DB_Tci.t_dec22 , _statusText);
      vvtbT_dec22.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec22, TheVvDaoTrans, DB_Tci.t_dec22 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec23_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec23 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec23",   TheVvDaoTrans, DB_Tci.t_dec23 , _statusText);
      vvtbT_dec23.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec23, TheVvDaoTrans, DB_Tci.t_dec23 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec24_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec24 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec24",   TheVvDaoTrans, DB_Tci.t_dec24 , _statusText);
      vvtbT_dec24.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec24, TheVvDaoTrans, DB_Tci.t_dec24 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec25_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec25 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec25",   TheVvDaoTrans, DB_Tci.t_dec25 , _statusText);
      vvtbT_dec25.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec25, TheVvDaoTrans, DB_Tci.t_dec25 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec26_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec26 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec26",   TheVvDaoTrans, DB_Tci.t_dec26 , _statusText);
      vvtbT_dec26.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec26, TheVvDaoTrans, DB_Tci.t_dec26 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec27_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec27 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec27",   TheVvDaoTrans, DB_Tci.t_dec27 , _statusText);
      vvtbT_dec27.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec27, TheVvDaoTrans, DB_Tci.t_dec27 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec28_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec28 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec28",   TheVvDaoTrans, DB_Tci.t_dec28 , _statusText);
      vvtbT_dec28.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec28, TheVvDaoTrans, DB_Tci.t_dec28 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec29_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec29 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec29",   TheVvDaoTrans, DB_Tci.t_dec29 , _statusText);
      vvtbT_dec29.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec29, TheVvDaoTrans, DB_Tci.t_dec29 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec30_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec30 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec30",   TheVvDaoTrans, DB_Tci.t_dec30 , _statusText);
      vvtbT_dec30.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec30, TheVvDaoTrans, DB_Tci.t_dec30 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }
   protected void T_dec31_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT_dec31 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec31",   TheVvDaoTrans, DB_Tci.t_dec31 , _statusText);
      vvtbT_dec31.JAM_ShouldCalcTransAndSumGrid = true;

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec31, TheVvDaoTrans, DB_Tci.t_dec31 , _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }

   #endregion dec01-31

   #region str01-31

   protected void T_str01_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str01 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str01", TheVvDaoTrans, DB_Tci.t_str01, _statusText);
      }
      else
      {
         vvtbT_str01 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str01",   TheVvDaoTrans, DB_Tci.t_str01 , _statusText);
         vvtbT_str01.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str01, TheVvDaoTrans, DB_Tci.t_str01 , _colHeader, _width);
      colVvText.Visible = isVisible;
      
   }

   protected void T_str02_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str02 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str02",   TheVvDaoTrans, DB_Tci.t_str02 , _statusText);
      }
      else
      {
         vvtbT_str02 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str02",   TheVvDaoTrans, DB_Tci.t_str02 , _statusText);
         vvtbT_str02.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str02, TheVvDaoTrans, DB_Tci.t_str02 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str03_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str03 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str03",   TheVvDaoTrans, DB_Tci.t_str03 , _statusText);
      }
      else
      {
         vvtbT_str03 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str03",   TheVvDaoTrans, DB_Tci.t_str03 , _statusText);
         vvtbT_str03.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str03, TheVvDaoTrans, DB_Tci.t_str03 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str04_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str04 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str04",   TheVvDaoTrans, DB_Tci.t_str04 , _statusText);
      }
      else
      {
         vvtbT_str04 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str04",   TheVvDaoTrans, DB_Tci.t_str04 , _statusText);
         vvtbT_str04.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str04, TheVvDaoTrans, DB_Tci.t_str04 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str05_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str05 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str05",   TheVvDaoTrans, DB_Tci.t_str05 , _statusText);
      }
      else
      {
         vvtbT_str05 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str05",   TheVvDaoTrans, DB_Tci.t_str05 , _statusText);
         vvtbT_str05.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str05, TheVvDaoTrans, DB_Tci.t_str05 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str06_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str06 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str06",   TheVvDaoTrans, DB_Tci.t_str06 , _statusText);
      }
      else
      {
         vvtbT_str06 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str06",   TheVvDaoTrans, DB_Tci.t_str06 , _statusText);
         vvtbT_str06.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str06, TheVvDaoTrans, DB_Tci.t_str06 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str07_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str07 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str07",   TheVvDaoTrans, DB_Tci.t_str07 , _statusText);
      }
      else
      {
         vvtbT_str07 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str07",   TheVvDaoTrans, DB_Tci.t_str07 , _statusText);
         vvtbT_str07.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str07, TheVvDaoTrans, DB_Tci.t_str07 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str08_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str08 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str08",   TheVvDaoTrans, DB_Tci.t_str08 , _statusText);
      }
      else
      {
         vvtbT_str08 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str08",   TheVvDaoTrans, DB_Tci.t_str08 , _statusText);
         vvtbT_str08.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str08, TheVvDaoTrans, DB_Tci.t_str08 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str09_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str09 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str09",   TheVvDaoTrans, DB_Tci.t_str09 , _statusText);
      }
      else
      {
         vvtbT_str09 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str09",   TheVvDaoTrans, DB_Tci.t_str09 , _statusText);
         vvtbT_str09.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str09, TheVvDaoTrans, DB_Tci.t_str09 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str10_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str10 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str10",   TheVvDaoTrans, DB_Tci.t_str10 , _statusText);
      }
      else
      {
         vvtbT_str10 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str10",   TheVvDaoTrans, DB_Tci.t_str10 , _statusText);
         vvtbT_str10.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str10, TheVvDaoTrans, DB_Tci.t_str10 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str11_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str11 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str11",   TheVvDaoTrans, DB_Tci.t_str11 , _statusText);
      }
      else
      {
         vvtbT_str11 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str11",   TheVvDaoTrans, DB_Tci.t_str11 , _statusText);
         vvtbT_str11.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str11, TheVvDaoTrans, DB_Tci.t_str11 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str12_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str12 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str12",   TheVvDaoTrans, DB_Tci.t_str12 , _statusText);
      }
      else
      {
         vvtbT_str12 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str12",   TheVvDaoTrans, DB_Tci.t_str12 , _statusText);
         vvtbT_str12.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str12, TheVvDaoTrans, DB_Tci.t_str12 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str13_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str13 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str13",   TheVvDaoTrans, DB_Tci.t_str13 , _statusText);
      }
      else
      {
         vvtbT_str13 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str13",   TheVvDaoTrans, DB_Tci.t_str13 , _statusText);
         vvtbT_str13.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str13, TheVvDaoTrans, DB_Tci.t_str13 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str14_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str14 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str14",   TheVvDaoTrans, DB_Tci.t_str14 , _statusText);
      }
      else
      {
         vvtbT_str14 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str14",   TheVvDaoTrans, DB_Tci.t_str14 , _statusText);
         vvtbT_str14.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str14, TheVvDaoTrans, DB_Tci.t_str14 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str15_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str15 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str15",   TheVvDaoTrans, DB_Tci.t_str15 , _statusText);
      }
      else
      {
         vvtbT_str15 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str15",   TheVvDaoTrans, DB_Tci.t_str15 , _statusText);
         vvtbT_str15.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str15, TheVvDaoTrans, DB_Tci.t_str15 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str16_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str16 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str16",   TheVvDaoTrans, DB_Tci.t_str16 , _statusText);
      }
      else
      {
         vvtbT_str16 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str16",   TheVvDaoTrans, DB_Tci.t_str16 , _statusText);
         vvtbT_str16.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str16, TheVvDaoTrans, DB_Tci.t_str16 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str17_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str17 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str17",   TheVvDaoTrans, DB_Tci.t_str17 , _statusText);
      }
      else
      {
         vvtbT_str17 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str17",   TheVvDaoTrans, DB_Tci.t_str17 , _statusText);
         vvtbT_str17.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str17, TheVvDaoTrans, DB_Tci.t_str17 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str18_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str18 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str18",   TheVvDaoTrans, DB_Tci.t_str18 , _statusText);
      }
      else
      {
         vvtbT_str18 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str18",   TheVvDaoTrans, DB_Tci.t_str18 , _statusText);
         vvtbT_str18.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str18, TheVvDaoTrans, DB_Tci.t_str18 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str19_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str19 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str19",   TheVvDaoTrans, DB_Tci.t_str19 , _statusText);
      }
      else
      {
         vvtbT_str19 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str19",   TheVvDaoTrans, DB_Tci.t_str19 , _statusText);
         vvtbT_str19.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str19, TheVvDaoTrans, DB_Tci.t_str19 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str20_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str20 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str20",   TheVvDaoTrans, DB_Tci.t_str20 , _statusText);
      }
      else
      {
         vvtbT_str20 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str20",   TheVvDaoTrans, DB_Tci.t_str20 , _statusText);
         vvtbT_str20.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str20, TheVvDaoTrans, DB_Tci.t_str20 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str21_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str21 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str21",   TheVvDaoTrans, DB_Tci.t_str21 , _statusText);
      }
      else
      {
         vvtbT_str21 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str21",   TheVvDaoTrans, DB_Tci.t_str21 , _statusText);
         vvtbT_str21.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str21, TheVvDaoTrans, DB_Tci.t_str21 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str22_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str22 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str22",   TheVvDaoTrans, DB_Tci.t_str22 , _statusText);
      }
      else
      {
         vvtbT_str22 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str22",   TheVvDaoTrans, DB_Tci.t_str22 , _statusText);
         vvtbT_str22.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str22, TheVvDaoTrans, DB_Tci.t_str22 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str23_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str23 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str23",   TheVvDaoTrans, DB_Tci.t_str23 , _statusText);
      }
      else
      {
         vvtbT_str23 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str23",   TheVvDaoTrans, DB_Tci.t_str23 , _statusText);
         vvtbT_str23.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str23, TheVvDaoTrans, DB_Tci.t_str23 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str24_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str24 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str24",   TheVvDaoTrans, DB_Tci.t_str24 , _statusText);
      }
      else
      {
         vvtbT_str24 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str24",   TheVvDaoTrans, DB_Tci.t_str24 , _statusText);
         vvtbT_str24.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str24, TheVvDaoTrans, DB_Tci.t_str24 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str25_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str25 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str25",   TheVvDaoTrans, DB_Tci.t_str25 , _statusText);
      }
      else
      {
         vvtbT_str25 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str25",   TheVvDaoTrans, DB_Tci.t_str25 , _statusText);
         vvtbT_str25.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str25, TheVvDaoTrans, DB_Tci.t_str25 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str26_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str26 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str26",   TheVvDaoTrans, DB_Tci.t_str26 , _statusText);
      }
      else
      {
         vvtbT_str26 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str26",   TheVvDaoTrans, DB_Tci.t_str26 , _statusText);
         vvtbT_str26.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str26, TheVvDaoTrans, DB_Tci.t_str26 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str27_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str27 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str27",   TheVvDaoTrans, DB_Tci.t_str27 , _statusText);
      }
      else
      {
         vvtbT_str27 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str27",   TheVvDaoTrans, DB_Tci.t_str27 , _statusText);
         vvtbT_str27.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str27, TheVvDaoTrans, DB_Tci.t_str27 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str28_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str28 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str28",   TheVvDaoTrans, DB_Tci.t_str28 , _statusText);
      }
      else
      {
         vvtbT_str28 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str28",   TheVvDaoTrans, DB_Tci.t_str28 , _statusText);
         vvtbT_str28.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str28, TheVvDaoTrans, DB_Tci.t_str28 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str29_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str29 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str29",   TheVvDaoTrans, DB_Tci.t_str29 , _statusText);
      }
      else
      {
         vvtbT_str29 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str29",   TheVvDaoTrans, DB_Tci.t_str29 , _statusText);
         vvtbT_str29.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str29, TheVvDaoTrans, DB_Tci.t_str29 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str30_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str30 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str30",   TheVvDaoTrans, DB_Tci.t_str30 , _statusText);
      }
      else
      {
         vvtbT_str30 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str30",   TheVvDaoTrans, DB_Tci.t_str30 , _statusText);
         vvtbT_str30.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str30, TheVvDaoTrans, DB_Tci.t_str30 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }
   protected void T_str31_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList, bool isVisible)
   {
      if(_lookUpList == null)
      {
         vvtbT_str31 = TheG.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_str31",   TheVvDaoTrans, DB_Tci.t_str31 , _statusText);
      }
      else
      {
         vvtbT_str31 = TheG.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_str31",   TheVvDaoTrans, DB_Tci.t_str31 , _statusText);
         vvtbT_str31.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_str31, TheVvDaoTrans, DB_Tci.t_str31 , _colHeader, _width);
      colVvText.Visible = isVisible;
   }

   #endregion str01-31

   #region dec01_02 do dec31_02 dodano 16.11.2017

   protected void T_dec01_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec01_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec01_2", TheVvDaoTrans, DB_Tci.t_dec01_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec01_2, TheVvDaoTrans, DB_Tci.t_dec01_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec02_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec02_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec02_2", TheVvDaoTrans, DB_Tci.t_dec02_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec02_2, TheVvDaoTrans, DB_Tci.t_dec02_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec03_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec03_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec03_2", TheVvDaoTrans, DB_Tci.t_dec03_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec03_2, TheVvDaoTrans, DB_Tci.t_dec03_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec04_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec04_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec04_2", TheVvDaoTrans, DB_Tci.t_dec04_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec04_2, TheVvDaoTrans, DB_Tci.t_dec04_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec05_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec05_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec05_2", TheVvDaoTrans, DB_Tci.t_dec05_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec05_2, TheVvDaoTrans, DB_Tci.t_dec05_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec06_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec06_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec06_2", TheVvDaoTrans, DB_Tci.t_dec06_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec06_2, TheVvDaoTrans, DB_Tci.t_dec06_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec07_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec07_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec07_2", TheVvDaoTrans, DB_Tci.t_dec07_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec07_2, TheVvDaoTrans, DB_Tci.t_dec07_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec08_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec08_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec08_2", TheVvDaoTrans, DB_Tci.t_dec08_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec08_2, TheVvDaoTrans, DB_Tci.t_dec08_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec09_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec09_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec09_2", TheVvDaoTrans, DB_Tci.t_dec09_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec09_2, TheVvDaoTrans, DB_Tci.t_dec09_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec10_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec10_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec10_2", TheVvDaoTrans, DB_Tci.t_dec10_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec10_2, TheVvDaoTrans, DB_Tci.t_dec10_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec11_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec11_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec11_2", TheVvDaoTrans, DB_Tci.t_dec11_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec11_2, TheVvDaoTrans, DB_Tci.t_dec11_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec12_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec12_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec12_2", TheVvDaoTrans, DB_Tci.t_dec12_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec12_2, TheVvDaoTrans, DB_Tci.t_dec12_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec13_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec13_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec13_2", TheVvDaoTrans, DB_Tci.t_dec13_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec13_2, TheVvDaoTrans, DB_Tci.t_dec13_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec14_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec14_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec14_2", TheVvDaoTrans, DB_Tci.t_dec14_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec14_2, TheVvDaoTrans, DB_Tci.t_dec14_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec15_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec15_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec15_2", TheVvDaoTrans, DB_Tci.t_dec15_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec15_2, TheVvDaoTrans, DB_Tci.t_dec15_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec16_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec16_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec16_2", TheVvDaoTrans, DB_Tci.t_dec16_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec16_2, TheVvDaoTrans, DB_Tci.t_dec16_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec17_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec17_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec17_2", TheVvDaoTrans, DB_Tci.t_dec17_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec17_2, TheVvDaoTrans, DB_Tci.t_dec17_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec18_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec18_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec18_2", TheVvDaoTrans, DB_Tci.t_dec18_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec18_2, TheVvDaoTrans, DB_Tci.t_dec18_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec19_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec19_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec19_2", TheVvDaoTrans, DB_Tci.t_dec19_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec19_2, TheVvDaoTrans, DB_Tci.t_dec19_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec20_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec20_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec20_2", TheVvDaoTrans, DB_Tci.t_dec20_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec20_2, TheVvDaoTrans, DB_Tci.t_dec20_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec21_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec21_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec21_2", TheVvDaoTrans, DB_Tci.t_dec21_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec21_2, TheVvDaoTrans, DB_Tci.t_dec21_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec22_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec22_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec22_2", TheVvDaoTrans, DB_Tci.t_dec22_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec22_2, TheVvDaoTrans, DB_Tci.t_dec22_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec23_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec23_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec23_2", TheVvDaoTrans, DB_Tci.t_dec23_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec23_2, TheVvDaoTrans, DB_Tci.t_dec23_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec24_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec24_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec24_2", TheVvDaoTrans, DB_Tci.t_dec24_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec24_2, TheVvDaoTrans, DB_Tci.t_dec24_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec25_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec25_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec25_2", TheVvDaoTrans, DB_Tci.t_dec25_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec25_2, TheVvDaoTrans, DB_Tci.t_dec25_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec26_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec26_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec26_2", TheVvDaoTrans, DB_Tci.t_dec26_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec26_2, TheVvDaoTrans, DB_Tci.t_dec26_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec27_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec27_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec27_2", TheVvDaoTrans, DB_Tci.t_dec27_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec27_2, TheVvDaoTrans, DB_Tci.t_dec27_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec28_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec28_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec28_2", TheVvDaoTrans, DB_Tci.t_dec28_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec28_2, TheVvDaoTrans, DB_Tci.t_dec28_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec29_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec29_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec29_2", TheVvDaoTrans, DB_Tci.t_dec29_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec29_2, TheVvDaoTrans, DB_Tci.t_dec29_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec30_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec30_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec30_2", TheVvDaoTrans, DB_Tci.t_dec30_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec30_2, TheVvDaoTrans, DB_Tci.t_dec30_2, _colHeader, _width); colVvText.MinimumWidth = _width; }
   protected void T_dec31_2_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces) { vvtbT_dec31_2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT_dec31_2", TheVvDaoTrans, DB_Tci.t_dec31_2, _statusText); colVvText = TheG.CreateVvTextBoxColumn(vvtbT_dec31_2, TheVvDaoTrans, DB_Tci.t_dec31_2, _colHeader, _width); colVvText.MinimumWidth = _width; }

   #endregion dec01_02 do dec31_02 dodano 16.11.2017
   
   #region AVR DUC

   protected void T_Time_01_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT01   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec01  , _colHeader, _width); colDateOnlyT01  .Tag = vvtb; }
   protected void T_Time_02_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT02   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec02  , _colHeader, _width); colDateOnlyT02  .Tag = vvtb; }
   protected void T_Time_03_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT03   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec03  , _colHeader, _width); colDateOnlyT03  .Tag = vvtb; }
   protected void T_Time_04_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT04   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec04  , _colHeader, _width); colDateOnlyT04  .Tag = vvtb; }
   protected void T_Time_05_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT05   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec05  , _colHeader, _width); colDateOnlyT05  .Tag = vvtb; }
   protected void T_Time_06_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT06   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec06  , _colHeader, _width); colDateOnlyT06  .Tag = vvtb; }
   protected void T_Time_07_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT07   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec07  , _colHeader, _width); colDateOnlyT07  .Tag = vvtb; }
   protected void T_Time_08_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT08   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec08  , _colHeader, _width); colDateOnlyT08  .Tag = vvtb; }
   protected void T_Time_09_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT09   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec09  , _colHeader, _width); colDateOnlyT09  .Tag = vvtb; }
   protected void T_Time_10_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT10   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec10  , _colHeader, _width); colDateOnlyT10  .Tag = vvtb; }
   protected void T_Time_11_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT11   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec11  , _colHeader, _width); colDateOnlyT11  .Tag = vvtb; }
   protected void T_Time_12_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT12   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec12  , _colHeader, _width); colDateOnlyT12  .Tag = vvtb; }
   protected void T_Time_13_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT13   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec13  , _colHeader, _width); colDateOnlyT13  .Tag = vvtb; }
   protected void T_Time_14_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT14   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec14  , _colHeader, _width); colDateOnlyT14  .Tag = vvtb; }
   protected void T_Time_15_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT15   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec15  , _colHeader, _width); colDateOnlyT15  .Tag = vvtb; }
   protected void T_Time_16_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT16   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec16  , _colHeader, _width); colDateOnlyT16  .Tag = vvtb; }
   protected void T_Time_17_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT17   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec17  , _colHeader, _width); colDateOnlyT17  .Tag = vvtb; }
   protected void T_Time_18_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT18   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec18  , _colHeader, _width); colDateOnlyT18  .Tag = vvtb; }
   protected void T_Time_19_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT19   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec19  , _colHeader, _width); colDateOnlyT19  .Tag = vvtb; }
   protected void T_Time_20_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT20   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec20  , _colHeader, _width); colDateOnlyT20  .Tag = vvtb; }
   protected void T_Time_21_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT21   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec21  , _colHeader, _width); colDateOnlyT21  .Tag = vvtb; }
   protected void T_Time_22_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT22   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec22  , _colHeader, _width); colDateOnlyT22  .Tag = vvtb; }
   protected void T_Time_23_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT23   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec23  , _colHeader, _width); colDateOnlyT23  .Tag = vvtb; }
   protected void T_Time_24_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT24   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec24  , _colHeader, _width); colDateOnlyT24  .Tag = vvtb; }
   protected void T_Time_25_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT25   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec25  , _colHeader, _width); colDateOnlyT25  .Tag = vvtb; }
   protected void T_Time_26_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT26   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec26  , _colHeader, _width); colDateOnlyT26  .Tag = vvtb; }
   protected void T_Time_27_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT27   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec27  , _colHeader, _width); colDateOnlyT27  .Tag = vvtb; }
   protected void T_Time_28_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT28   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec28  , _colHeader, _width); colDateOnlyT28  .Tag = vvtb; }
   protected void T_Time_29_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT29   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec29  , _colHeader, _width); colDateOnlyT29  .Tag = vvtb; }
   protected void T_Time_30_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT30   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec30  , _colHeader, _width); colDateOnlyT30  .Tag = vvtb; }
   protected void T_Time_31_CreateColumn  (int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT31   = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec31  , _colHeader, _width); colDateOnlyT31  .Tag = vvtb; }
   protected void T_Time_01_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT01_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec01_2, _colHeader, _width); colDateOnlyT01_2.Tag = vvtb; } 
   protected void T_Time_02_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT02_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec02_2, _colHeader, _width); colDateOnlyT02_2.Tag = vvtb; } 
   protected void T_Time_03_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT03_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec03_2, _colHeader, _width); colDateOnlyT03_2.Tag = vvtb; } 
   protected void T_Time_04_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT04_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec04_2, _colHeader, _width); colDateOnlyT04_2.Tag = vvtb; } 
   protected void T_Time_05_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT05_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec05_2, _colHeader, _width); colDateOnlyT05_2.Tag = vvtb; } 
   protected void T_Time_06_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT06_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec06_2, _colHeader, _width); colDateOnlyT06_2.Tag = vvtb; } 
   protected void T_Time_07_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT07_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec07_2, _colHeader, _width); colDateOnlyT07_2.Tag = vvtb; } 
   protected void T_Time_08_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT08_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec08_2, _colHeader, _width); colDateOnlyT08_2.Tag = vvtb; } 
   protected void T_Time_09_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT09_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec09_2, _colHeader, _width); colDateOnlyT09_2.Tag = vvtb; } 
   protected void T_Time_10_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT10_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec10_2, _colHeader, _width); colDateOnlyT10_2.Tag = vvtb; } 
   protected void T_Time_11_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT11_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec11_2, _colHeader, _width); colDateOnlyT11_2.Tag = vvtb; } 
   protected void T_Time_12_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT12_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec12_2, _colHeader, _width); colDateOnlyT12_2.Tag = vvtb; } 
   protected void T_Time_13_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT13_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec13_2, _colHeader, _width); colDateOnlyT13_2.Tag = vvtb; } 
   protected void T_Time_14_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT14_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec14_2, _colHeader, _width); colDateOnlyT14_2.Tag = vvtb; } 
   protected void T_Time_15_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT15_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec15_2, _colHeader, _width); colDateOnlyT15_2.Tag = vvtb; } 
   protected void T_Time_16_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT16_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec16_2, _colHeader, _width); colDateOnlyT16_2.Tag = vvtb; } 
   protected void T_Time_17_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT17_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec17_2, _colHeader, _width); colDateOnlyT17_2.Tag = vvtb; } 
   protected void T_Time_18_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT18_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec18_2, _colHeader, _width); colDateOnlyT18_2.Tag = vvtb; } 
   protected void T_Time_19_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT19_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec19_2, _colHeader, _width); colDateOnlyT19_2.Tag = vvtb; } 
   protected void T_Time_20_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT20_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec20_2, _colHeader, _width); colDateOnlyT20_2.Tag = vvtb; } 
   protected void T_Time_21_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT21_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec21_2, _colHeader, _width); colDateOnlyT21_2.Tag = vvtb; } 
   protected void T_Time_22_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT22_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec22_2, _colHeader, _width); colDateOnlyT22_2.Tag = vvtb; } 
   protected void T_Time_23_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT23_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec23_2, _colHeader, _width); colDateOnlyT23_2.Tag = vvtb; } 
   protected void T_Time_24_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT24_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec24_2, _colHeader, _width); colDateOnlyT24_2.Tag = vvtb; } 
   protected void T_Time_25_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT25_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec25_2, _colHeader, _width); colDateOnlyT25_2.Tag = vvtb; } 
   protected void T_Time_26_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT26_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec26_2, _colHeader, _width); colDateOnlyT26_2.Tag = vvtb; } 
   protected void T_Time_27_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT27_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec27_2, _colHeader, _width); colDateOnlyT27_2.Tag = vvtb; } 
   protected void T_Time_28_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT28_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec28_2, _colHeader, _width); colDateOnlyT28_2.Tag = vvtb; } 
   protected void T_Time_29_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT29_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec29_2, _colHeader, _width); colDateOnlyT29_2.Tag = vvtb; } 
   protected void T_Time_30_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT30_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec30_2, _colHeader, _width); colDateOnlyT30_2.Tag = vvtb; }
   protected void T_Time_31_2_CreateColumn(int _width, string _colHeader, string _statusText){ VvTextBox vvtb = new VvTextBox(); vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; colDateOnlyT31_2 = TheG.CreateCalendar_TimeOnly_Column(TheVvDaoTrans, DB_Tci.t_dec31_2, _colHeader, _width); colDateOnlyT31_2.Tag = vvtb; } 
   
   #endregion AVR DUC
  
   #region pmv

   protected void R_st_PO_CreateColumn    (int _width, int numOfDecimalPlaces    , string _colHeader, string _statusText)
   {
      vvtbT_st_PO = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_st_PO", null, -12, _statusText);
      vvtbT_st_PO.JAM_ReadOnly = true;
      vvtbT_st_PO.JAM_IsForPercent= true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_st_PO, null, "R_st_PO", _colHeader, _width);
   }
   protected void R_iznos_PO_CreateColumn (int _width, int numOfDecimalPlaces    , string _colHeader, string _statusText)
   {
      vvtbT_iznos_PO = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_iznos_PO", null, -12, _statusText);
      vvtbT_iznos_PO.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_iznos_PO, null, "R_iznos_PO", _colHeader, _width);
   }
   protected void R_st_CO2_CreateColumn   (int _width, int numOfDecimalPlaces    , string _colHeader, string _statusText)
   {
      vvtbT_st_CO2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_st_CO2", null, -12, _statusText);
      vvtbT_st_CO2.JAM_ReadOnly = true;
      vvtbT_st_CO2.JAM_IsForPercent = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_st_CO2, null, "R_st_CO2", _colHeader, _width);
   }
   protected void R_iznos_CO2_CreateColumn(int _width, int numOfDecimalPlaces    , string _colHeader, string _statusText)
   {
      vvtbT_iznos_CO2 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_iznos_CO2", null, -12, _statusText);
      vvtbT_iznos_CO2.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_iznos_CO2, null, "R_iznos_CO2", _colHeader, _width);
   }
   protected void R_st_cm3_CreateColumn   (int _width, int numOfDecimalPlaces    , string _colHeader, string _statusText)
   {
      vvtbT_st_cm3 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_st_cm3", null, -12, _statusText);
      vvtbT_st_cm3.JAM_ReadOnly = true;
      vvtbT_st_cm3.JAM_IsForPercent = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_st_cm3, null, "R_st_cm3", _colHeader, _width);
   }
   protected void R_iznos_cm3_CreateColumn(int _width, int numOfDecimalPlaces    , string _colHeader, string _statusText)
   {
      vvtbT_iznos_cm3 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_iznos_cm3", null, -12, _statusText);
      vvtbT_iznos_cm3.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_iznos_cm3, null, "R_iznos_cm3", _colHeader, _width);
   }
   protected void R_st_EUN_CreateColumn   (int _width, int numOfDecimalPlaces    , string _colHeader, string _statusText)
   {
      vvtbT_st_EUN = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_st_EUN", null, -12, _statusText);
      vvtbT_st_EUN.JAM_ReadOnly = true;
      vvtbT_st_EUN.JAM_IsForPercent = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_st_EUN, null, "R_st_EUN", _colHeader, _width);
   }
   protected void R_iznos_EUN_CreateColumn(int _width, int numOfDecimalPlaces    , string _colHeader, string _statusText)
   {
      vvtbT_iznos_EUN = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColT_iznos_EUN", null, -12, _statusText);
      vvtbT_iznos_EUN.JAM_ReadOnly = true;
      colVvText = TheG.CreateVvTextBoxColumn(vvtbT_iznos_EUN, null, "R_iznos_EUN", _colHeader, _width);
   }

   #endregion pmv

   #region BMW

   protected void R_finalCostEUR_CreateColumn           (int _width, string _colHeader, string _statusText, int numOfDecimalPlaces) { vvtbR_finalCostEUR            = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColR_finalCostEUR"           , null, -12, _statusText); vvtbR_finalCostEUR           .JAM_ReadOnly = true;   vvtbR_finalCostEUR           .JAM_ForeColor = Color.Green ; colVvText = TheG.CreateVvTextBoxColumn(vvtbR_finalCostEUR           , null, "R_finalCostEUR"           , _colHeader, _width); }
   protected void R_monthAge_CreateColumn               (int _width, string _colHeader, string _statusText, int numOfDecimalPlaces) { vvtbR_monthAge                = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColR_monthAge"               , null, -12, _statusText); vvtbR_monthAge               .JAM_ReadOnly = true; /*vvtbR_monthAge               .JAM_ForeColor = Color.Blue*/; colVvText = TheG.CreateVvTextBoxColumn(vvtbR_monthAge               , null, "R_monthAge"               , _colHeader, _width); }
   protected void R_ageSt_CreateColumn                  (int _width, string _colHeader, string _statusText, int numOfDecimalPlaces) { vvtbR_ageSt                   = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColR_ageSt"                  , null, -12, _statusText); vvtbR_ageSt                  .JAM_ReadOnly = true; /*vvtbR_ageSt                  .JAM_ForeColor = Color.Blue*/; colVvText = TheG.CreateVvTextBoxColumn(vvtbR_ageSt                  , null, "R_ageSt"                  , _colHeader, _width); }
   protected void R_co2Emisija_CreateColumn             (int _width, string _colHeader, string _statusText, int numOfDecimalPlaces) { vvtbR_co2Emisija              = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColR_co2Emisija "            , null, -12, _statusText); vvtbR_co2Emisija             .JAM_ReadOnly = true; /*vvtbR_co2Emisija             .JAM_ForeColor = Color.Blue*/; colVvText = TheG.CreateVvTextBoxColumn(vvtbR_co2Emisija             , null, "R_co2Emisija"             , _colHeader, _width); }
   protected void R_ONENEur_CreateColumn                (int _width, string _colHeader, string _statusText, int numOfDecimalPlaces) { vvtbR_ONENEur                 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColR_ONENEur"                , null, -12, _statusText); vvtbR_ONENEur                .JAM_ReadOnly = true;   vvtbR_ONENEur                .JAM_ForeColor = Color.Green ; colVvText = TheG.CreateVvTextBoxColumn(vvtbR_ONENEur                , null, "R_ONENEur"                , _colHeader, _width); }
   protected void R_RealMPC_EUR_CreateColumn            (int _width, string _colHeader, string _statusText, int numOfDecimalPlaces) { vvtbR_RealMPC_EUR             = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColR_RealMPC_EUR"            , null, -12, _statusText); vvtbR_RealMPC_EUR            .JAM_ReadOnly = true;   vvtbR_RealMPC_EUR            .JAM_ForeColor = Color.Green ; colVvText = TheG.CreateVvTextBoxColumn(vvtbR_RealMPC_EUR            , null, "R_RealMPC_EUR"            , _colHeader, _width); }
   protected void R_HR_history_TOTAL_Cij_Kn_CreateColumn(int _width, string _colHeader, string _statusText, int numOfDecimalPlaces) { vvtbR_HR_history_TOTAL_Cij_Kn = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColR_HR_history_TOTAL_Cij_Kn", null, -12, _statusText); vvtbR_HR_history_TOTAL_Cij_Kn.JAM_ReadOnly = true; /*vvtbR_HR_history_TOTAL_Cij_Kn.JAM_ForeColor = Color.Blue*/; colVvText = TheG.CreateVvTextBoxColumn(vvtbR_HR_history_TOTAL_Cij_Kn, null, "R_HR_history_TOTAL_Cij_Kn", _colHeader, _width); }

   protected void R_HR_historyOsnovnaCij_Kn_CreateColumn(int _width, string _colHeader, string _statusText, int numOfDecimalPlaces) { vvtbR_HR_historyOsnovnaCij_Kn = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColR_HR_historyOsnovnaCij_Kn", null, -12, _statusText); vvtbR_HR_historyOsnovnaCij_Kn.JAM_ReadOnly = true; /*vvtbR_HR_historyOsnovnaCij_Kn.JAM_ForeColor = Color.Green*/; colVvText = TheG.CreateVvTextBoxColumn(vvtbR_HR_historyOsnovnaCij_Kn, null, "R_HR_historyOsnovnaCij_Kn", _colHeader, _width); }
   protected void R_HR_historyOprermaCij_Kn_CreateColumn(int _width, string _colHeader, string _statusText, int numOfDecimalPlaces) { vvtbR_HR_historyOprermaCij_Kn = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColR_HR_historyOprermaCij_Kn", null, -12, _statusText); vvtbR_HR_historyOprermaCij_Kn.JAM_ReadOnly = true; /*vvtbR_HR_historyOprermaCij_Kn.JAM_ForeColor = Color.Green*/; colVvText = TheG.CreateVvTextBoxColumn(vvtbR_HR_historyOprermaCij_Kn, null, "R_HR_historyOprermaCij_Kn", _colHeader, _width); }
   protected void R_mm1Registracije_CreateColumn        (int _width, string _colHeader, string _statusText, int numOfDecimalPlaces) { vvtbR_mm1Registracije         = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColR_mm1Registracije"        , null, -12, _statusText); vvtbR_mm1Registracije        .JAM_ReadOnly = true; /*vvtbR_mm1Registracije        .JAM_ForeColor = Color.Green*/; colVvText = TheG.CreateVvTextBoxColumn(vvtbR_mm1Registracije        , null, "R_mm1Registracije"        , _colHeader, _width); }
   protected void R_yy1Registracije_CreateColumn        (int _width, string _colHeader, string _statusText, int numOfDecimalPlaces) { vvtbR_yy1Registracije         = TheG.CreateVvTextBoxFor_Integer_ColumnTemplate(false             , "vvtb4ColR_yy1Registracije"        , null, -12, _statusText); vvtbR_yy1Registracije        .JAM_ReadOnly = true; /*vvtbR_yy1Registracije        .JAM_ForeColor = Color.Green*/; colVvText = TheG.CreateVvTextBoxColumn(vvtbR_yy1Registracije        , null, "R_yy1Registracije"        , _colHeader, _width); }
   protected void R_VNPCEur_CreateColumn                (int _width, string _colHeader, string _statusText, int numOfDecimalPlaces) { vvtbR_VNPCEur                 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColR_VNPCEur"                , null, -12, _statusText); vvtbR_VNPCEur                .JAM_ReadOnly = true;   vvtbR_VNPCEur                .JAM_ForeColor = Color.Green  ; colVvText = TheG.CreateVvTextBoxColumn(vvtbR_VNPCEur                , null, "R_VNPCEur"                , _colHeader, _width); }
   protected void R_PPMV_historyEur_CreateColumn        (int _width, string _colHeader, string _statusText, int numOfDecimalPlaces) { vvtbR_PPMV_historyEur         = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColR_PPMV_historyEur"        , null, -12, _statusText); vvtbR_PPMV_historyEur        .JAM_ReadOnly = true;   vvtbR_PPMV_historyEur        .JAM_ForeColor = Color.Green  ; colVvText = TheG.CreateVvTextBoxColumn(vvtbR_PPMV_historyEur        , null, "R_PPMV_historyEur"        , _colHeader, _width); }
   protected void R_PPMVeur_CreateColumn                (int _width, string _colHeader, string _statusText, int numOfDecimalPlaces) { vvtbR_PPMVeur                 = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColR_PPMVeur"                , null, -12, _statusText); vvtbR_PPMVeur                .JAM_ReadOnly = true;   vvtbR_PPMVeur                .JAM_ForeColor = Color.Green  ; colVvText = TheG.CreateVvTextBoxColumn(vvtbR_PPMVeur                , null, "R_PPMVeur"                , _colHeader, _width); }
   protected void R_toPayAutohouseEUR_CreateColumn      (int _width, string _colHeader, string _statusText, int numOfDecimalPlaces) { vvtbR_toPayAutohouseEUR       = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColR_toPayAutohouseEUR"      , null, -12, _statusText); vvtbR_toPayAutohouseEUR      .JAM_ReadOnly = true;   vvtbR_toPayAutohouseEUR      .JAM_ForeColor = Color.Green  ; colVvText = TheG.CreateVvTextBoxColumn(vvtbR_toPayAutohouseEUR      , null, "R_toPayAutohouseEUR"      , _colHeader, _width); }
   protected void R_toPayTotalEur_CreateColumn          (int _width, string _colHeader, string _statusText, int numOfDecimalPlaces) { vvtbR_toPayTotalEur           = TheG.CreateVvTextBoxFor_Decimal_ColumnTemplate(numOfDecimalPlaces, "vvtb4ColR_toPayTotalEur"          , null, -12, _statusText); vvtbR_toPayTotalEur          .JAM_ReadOnly = true;   vvtbR_toPayTotalEur          .JAM_ForeColor = Color.Green  ; colVvText = TheG.CreateVvTextBoxColumn(vvtbR_toPayTotalEur          , null, "R_toPayTotalEur"          , _colHeader, _width); }

   #endregion BMW

   public void ColumnForScroll_CreateColumn(int _width)
   {
      colScrol = TheG.CreateScrollColumn("scrol", _width);
      colScrol.ReadOnly = true;
   }

   #endregion TheGrid_Columns

   #region TheGrid2_Columns

   protected void T_2moneyA_CreateColumn(int _width, string _colHeader, string _statusText, int _numOfDecimalPlaces)
   {
      vvtbT2_moneyA = TheG2.CreateVvTextBoxFor_Decimal_ColumnTemplate(_numOfDecimalPlaces, "vvtb4ColT2_moneyA", TheVvDaoTrans2, DB_Tci2.t_moneyA, _statusText);

      if(this is PutNalDUC) vvtbT2_moneyA.JAM_ShouldSumGrid = true;

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT2_moneyA, TheVvDaoTrans2, DB_Tci2.t_moneyA, _colHeader, _width);
      colVvText.MinimumWidth = _width;             // __mora biti == sum.MinWidth
   }

   protected void T_2opis_128_CreateColumn(int _width, string _colHeader, string _statusText, int maxLength)
   {
      vvtbT2_opis_128 = TheG2.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT2_opis_128", TheVvDaoTrans2, maxLength.IsNegative() ? maxLength : DB_Tci2.t_opis_128, _statusText);
      //vvtbT_opis_128.JAM_ShouldCopyPrevRow = true;

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT2_opis_128, TheVvDaoTrans2, DB_Tci2.t_opis_128, _colHeader, _width);

      if(_width.IsZero())
      {
         colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
         colVvText.MinimumWidth = ZXC.Q7un;
      }
   }

   protected void T_2konto_CreateColumn(int _width, string _colHeader, string _statusText)
   {
      vvtbT2_konto = TheG2.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT2_konto", TheVvDaoTrans2, DB_Tci2.t_konto, _statusText);
      vvtbT2_konto.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      vvtbT2_konto.JAM_SetAutoCompleteData(Kplan.recordName, VvSQL.SorterType.KontoNaziv, ZXC.AutoCompleteRestrictor.KPL_Analitika_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_KplanNaziv_sorterCode), null);
      vvtbT2_konto.JAM_FieldExitMethod = new EventHandler(OnExitKonto2_ClearPreffix);
      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT2_konto, TheVvDaoTrans2, DB_Tci2.t_konto, _colHeader, _width);
   }
   public void OnExitKonto2_ClearPreffix(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvTextBoxEditingControl vvTbKonto = sender as VvTextBoxEditingControl;

      string dirtyString = vvTbKonto.Text, cleanString;

      int spaceIdx = dirtyString.IndexOf(' ');

      if(dirtyString.Length.IsZero() || spaceIdx.IsNegative()) return;

      cleanString = dirtyString.Substring(0, spaceIdx);

      //vvTbKonto.Text = cleanString;
      //TheG.EditingControl.Text = cleanString;
      TheG2.PutCell(ci2.iT_konto, vvTbKonto.EditingControlDataGridView.CurrentRow.Index, cleanString);
   }

   protected void T_2devValuta_CreateColumn(int _width, string _colHeader, string _statusText, VvLookUpLista _lookUpList)
   {
      if(_lookUpList == null)
      {

         vvtbT2_devValuta = TheG2.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT2_devValuta", TheVvDaoTrans2, DB_Tci2.t_devName, _statusText);
      }
      else
      {
         vvtbT2_devValuta = TheG2.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT2_devValuta", TheVvDaoTrans2, DB_Tci2.t_devName, _statusText);
         vvtbT2_devValuta.JAM_Set_LookUpTable(_lookUpList, (int)ZXC.Kolona.prva);
      }

      colVvText = TheG2.CreateVvTextBoxColumn(vvtbT2_devValuta, TheVvDaoTrans2, DB_Tci2.t_devName, _colHeader, _width);
   }

   #endregion TheGrid2_Columns

   #region SetXtransColumnIndexes()

   /// <summary>
   /// Column Index na DataGridView-u (NE U DataBase to ti je 'DB_ci')
   /// </summary>
   protected Xtrans_colIdx ci;
   public Xtrans_colIdx DgvCI { get { return ci; } }

   public struct Xtrans_colIdx
   {
      internal int iT_recID;
      internal int iT_serial;
      internal int iT_moneyA;
      internal int iT_kol;
      internal int iT_opis_128;
      internal int iT_kpdbNameA_50;
      internal int iT_kpdbUlBrA_32;
      internal int iT_kpdbMjestoA_32;
      internal int iT_kpdbZiroA_32;
      internal int iT_kpdbNameB_50;
      internal int iT_kpdbUlBrB_32;
      internal int iT_kpdbMjestoB_32;
      internal int iT_kpdbZiroB_32;
      internal int iT_dateOd;
      internal int iT_dateDo;
      internal int iT_strA_2;
      internal int iT_strB_2;
      internal int iT_vezniDokA_64;
      internal int iT_vezniDokB_64;
      internal int iT_strC_2;
      internal int iT_kupdobCD;
      internal int iT_artiklCD;
      internal int iT_artiklName;
      internal int iT_isXxx;
      internal int iT_intA;
      internal int iT_intB;
      internal int iT_konto;
      internal int iT_kolMoneyA;
      internal int iT_personCD;
      internal int iT_moneyB;
      internal int iT_moneyC;
      internal int iT_moneyD;

      internal int iT_st_PO    ;
      internal int iT_iznos_PO ;
      internal int iT_st_CO2   ;
      internal int iT_iznos_CO2;
      internal int iT_st_cm3   ;
      internal int iT_iznos_cm3;
      internal int iT_st_EUN   ;
      internal int iT_iznos_EUN;

      internal int iT_dec01;
      internal int iT_dec02;
      internal int iT_dec03;
      internal int iT_dec04;
      internal int iT_dec05;
      internal int iT_dec06;
      internal int iT_dec07;
      internal int iT_dec08;
      internal int iT_dec09;
      internal int iT_dec10;
      internal int iT_dec11;
      internal int iT_dec12;
      internal int iT_dec13;
      internal int iT_dec14;
      internal int iT_dec15;
      internal int iT_dec16;
      internal int iT_dec17;
      internal int iT_dec18;
      internal int iT_dec19;
      internal int iT_dec20;
      internal int iT_dec21;
      internal int iT_dec22;
      internal int iT_dec23;
      internal int iT_dec24;
      internal int iT_dec25;
      internal int iT_dec26;
      internal int iT_dec27;
      internal int iT_dec28;
      internal int iT_dec29;
      internal int iT_dec30;
      internal int iT_dec31;
      internal int iT_dec01_2;
      internal int iT_dec02_2;
      internal int iT_dec03_2;
      internal int iT_dec04_2;
      internal int iT_dec05_2;
      internal int iT_dec06_2;
      internal int iT_dec07_2;
      internal int iT_dec08_2;
      internal int iT_dec09_2;
      internal int iT_dec10_2;
      internal int iT_dec11_2;
      internal int iT_dec12_2;
      internal int iT_dec13_2;
      internal int iT_dec14_2;
      internal int iT_dec15_2;
      internal int iT_dec16_2;
      internal int iT_dec17_2;
      internal int iT_dec18_2;
      internal int iT_dec19_2;
      internal int iT_dec20_2;
      internal int iT_dec21_2;
      internal int iT_dec22_2;
      internal int iT_dec23_2;
      internal int iT_dec24_2;
      internal int iT_dec25_2;
      internal int iT_dec26_2;
      internal int iT_dec27_2;
      internal int iT_dec28_2;
      internal int iT_dec29_2;
      internal int iT_dec30_2;
      internal int iT_dec31_2;

      internal int iT_str01;
      internal int iT_str02;
      internal int iT_str03;
      internal int iT_str04;
      internal int iT_str05;
      internal int iT_str06;
      internal int iT_str07;
      internal int iT_str08;
      internal int iT_str09;
      internal int iT_str10;
      internal int iT_str11;
      internal int iT_str12;
      internal int iT_str13;
      internal int iT_str14;
      internal int iT_str15;
      internal int iT_str16;
      internal int iT_str17;
      internal int iT_str18;
      internal int iT_str19;
      internal int iT_str20;
      internal int iT_str21;
      internal int iT_str22;
      internal int iT_str23;
      internal int iT_str24;
      internal int iT_str25;
      internal int iT_str26;
      internal int iT_str27;
      internal int iT_str28;
      internal int iT_str29;
      internal int iT_str30;
      internal int iT_str31;

      internal int iT_RFS;
      internal int iT_PFS;
      internal int iT_MFS;

      internal int iT_SumMoney;

      internal int iT_finalCostEUR           ;
      internal int iT_monthAge               ;
      internal int iT_ageSt                  ;
      internal int iT_co2Emisija             ;
      internal int iT_ONENEur                ;
      internal int iT_RealMPC_EUR            ;
      internal int iT_HR_history_TOTAL_Cij_Kn;
      internal int iT_HR_historyOsnovnaCij_Kn;
      internal int iT_HR_historyOprermaCij_Kn;
      internal int iT_mm1Registracije        ;
      internal int iT_yy1Registracije        ;
      internal int iT_VNPCEur                ;
      internal int iT_PPMV_historyEur        ;
      internal int iT_PPMVeur                ;
      internal int iT_toPayAutohouseEUR      ;
      internal int iT_toPayTotalEur          ;

      internal int iT_date3;
      internal int iT_date4;


   }

   private void SetXtransColumnIndexes()
   {
      ci = new Xtrans_colIdx();

      ci.iT_recID          = TheG.IdxForColumn("T_recID");
      ci.iT_serial         = TheG.IdxForColumn("T_serial");
      ci.iT_moneyA         = TheG.IdxForColumn("T_moneyA");
      ci.iT_kol            = TheG.IdxForColumn("T_kol");
      ci.iT_opis_128       = TheG.IdxForColumn("T_opis_128");
      ci.iT_kpdbNameA_50   = TheG.IdxForColumn("T_kpdbNameA_50");
      ci.iT_kpdbUlBrA_32   = TheG.IdxForColumn("T_kpdbUlBrA_32");
      ci.iT_kpdbMjestoA_32 = TheG.IdxForColumn("T_kpdbMjestoA_32");
      ci.iT_kpdbZiroA_32   = TheG.IdxForColumn("T_kpdbZiroA_32");
      ci.iT_kpdbNameB_50   = TheG.IdxForColumn("T_kpdbNameB_50");
      ci.iT_kpdbUlBrB_32   = TheG.IdxForColumn("T_kpdbUlBrB_32");
      ci.iT_kpdbMjestoB_32 = TheG.IdxForColumn("T_kpdbMjestoB_32");
      ci.iT_kpdbZiroB_32   = TheG.IdxForColumn("T_kpdbZiroB_32");
      ci.iT_dateOd         = TheG.IdxForColumn("T_dateOd");
      ci.iT_dateDo         = TheG.IdxForColumn("T_dateDo");
      ci.iT_strA_2         = TheG.IdxForColumn("T_strA_2");
      ci.iT_strB_2         = TheG.IdxForColumn("T_strB_2");
      ci.iT_vezniDokA_64   = TheG.IdxForColumn("T_vezniDokA_64");
      ci.iT_vezniDokB_64   = TheG.IdxForColumn("T_vezniDokB_64");
      ci.iT_strC_2         = TheG.IdxForColumn("T_strC_2");
      ci.iT_kupdobCD       = TheG.IdxForColumn("T_kupdobCD");
      ci.iT_artiklCD       = TheG.IdxForColumn("T_artiklCD");
      ci.iT_artiklName     = TheG.IdxForColumn("T_artiklName");
      ci.iT_isXxx          = TheG.IdxForColumn("T_isXxx");
      ci.iT_intA           = TheG.IdxForColumn("T_intA");
      ci.iT_intB           = TheG.IdxForColumn("T_intB");
      ci.iT_konto          = TheG.IdxForColumn("T_konto");
      ci.iT_kolMoneyA      = TheG.IdxForColumn("R_KolMoneyA");
      ci.iT_personCD       = TheG.IdxForColumn("T_personCD");
      ci.iT_moneyB         = TheG.IdxForColumn("T_moneyB");
      ci.iT_moneyC         = TheG.IdxForColumn("T_moneyC");
      ci.iT_moneyD         = TheG.IdxForColumn("T_moneyD");

      ci.iT_st_PO          = TheG.IdxForColumn("R_st_PO");
      ci.iT_iznos_PO       = TheG.IdxForColumn("R_iznos_PO");
      ci.iT_st_CO2         = TheG.IdxForColumn("R_st_CO2");
      ci.iT_iznos_CO2      = TheG.IdxForColumn("R_iznos_CO2");
      ci.iT_st_cm3         = TheG.IdxForColumn("R_st_cm3");
      ci.iT_iznos_cm3      = TheG.IdxForColumn("R_iznos_cm3");
      ci.iT_st_EUN         = TheG.IdxForColumn("R_st_EUN");
      ci.iT_iznos_EUN      = TheG.IdxForColumn("R_iznos_EUN");

      ci.iT_dec01          = TheG.IdxForColumn("T_dec01");
      ci.iT_dec02          = TheG.IdxForColumn("T_dec02");
      ci.iT_dec03          = TheG.IdxForColumn("T_dec03");
      ci.iT_dec04          = TheG.IdxForColumn("T_dec04");
      ci.iT_dec05          = TheG.IdxForColumn("T_dec05");
      ci.iT_dec06          = TheG.IdxForColumn("T_dec06");
      ci.iT_dec07          = TheG.IdxForColumn("T_dec07");
      ci.iT_dec08          = TheG.IdxForColumn("T_dec08");
      ci.iT_dec09          = TheG.IdxForColumn("T_dec09");
      ci.iT_dec10          = TheG.IdxForColumn("T_dec10");
      ci.iT_dec11          = TheG.IdxForColumn("T_dec11");
      ci.iT_dec12          = TheG.IdxForColumn("T_dec12");
      ci.iT_dec13          = TheG.IdxForColumn("T_dec13");
      ci.iT_dec14          = TheG.IdxForColumn("T_dec14");
      ci.iT_dec15          = TheG.IdxForColumn("T_dec15");
      ci.iT_dec16          = TheG.IdxForColumn("T_dec16");
      ci.iT_dec17          = TheG.IdxForColumn("T_dec17");
      ci.iT_dec18          = TheG.IdxForColumn("T_dec18");
      ci.iT_dec19          = TheG.IdxForColumn("T_dec19");
      ci.iT_dec20          = TheG.IdxForColumn("T_dec20");
      ci.iT_dec21          = TheG.IdxForColumn("T_dec21");
      ci.iT_dec22          = TheG.IdxForColumn("T_dec22");
      ci.iT_dec23          = TheG.IdxForColumn("T_dec23");
      ci.iT_dec24          = TheG.IdxForColumn("T_dec24");
      ci.iT_dec25          = TheG.IdxForColumn("T_dec25");
      ci.iT_dec26          = TheG.IdxForColumn("T_dec26");
      ci.iT_dec27          = TheG.IdxForColumn("T_dec27");
      ci.iT_dec28          = TheG.IdxForColumn("T_dec28");
      ci.iT_dec29          = TheG.IdxForColumn("T_dec29");
      ci.iT_dec30          = TheG.IdxForColumn("T_dec30");
      ci.iT_dec31          = TheG.IdxForColumn("T_dec31");

      ci.iT_dec01_2        = TheG.IdxForColumn("T_dec01_2");
      ci.iT_dec02_2        = TheG.IdxForColumn("T_dec02_2");
      ci.iT_dec03_2        = TheG.IdxForColumn("T_dec03_2");
      ci.iT_dec04_2        = TheG.IdxForColumn("T_dec04_2");
      ci.iT_dec05_2        = TheG.IdxForColumn("T_dec05_2");
      ci.iT_dec06_2        = TheG.IdxForColumn("T_dec06_2");
      ci.iT_dec07_2        = TheG.IdxForColumn("T_dec07_2");
      ci.iT_dec08_2        = TheG.IdxForColumn("T_dec08_2");
      ci.iT_dec09_2        = TheG.IdxForColumn("T_dec09_2");
      ci.iT_dec10_2        = TheG.IdxForColumn("T_dec10_2");
      ci.iT_dec11_2        = TheG.IdxForColumn("T_dec11_2");
      ci.iT_dec12_2        = TheG.IdxForColumn("T_dec12_2");
      ci.iT_dec13_2        = TheG.IdxForColumn("T_dec13_2");
      ci.iT_dec14_2        = TheG.IdxForColumn("T_dec14_2");
      ci.iT_dec15_2        = TheG.IdxForColumn("T_dec15_2");
      ci.iT_dec16_2        = TheG.IdxForColumn("T_dec16_2");
      ci.iT_dec17_2        = TheG.IdxForColumn("T_dec17_2");
      ci.iT_dec18_2        = TheG.IdxForColumn("T_dec18_2");
      ci.iT_dec19_2        = TheG.IdxForColumn("T_dec19_2");
      ci.iT_dec20_2        = TheG.IdxForColumn("T_dec20_2");
      ci.iT_dec21_2        = TheG.IdxForColumn("T_dec21_2");
      ci.iT_dec22_2        = TheG.IdxForColumn("T_dec22_2");
      ci.iT_dec23_2        = TheG.IdxForColumn("T_dec23_2");
      ci.iT_dec24_2        = TheG.IdxForColumn("T_dec24_2");
      ci.iT_dec25_2        = TheG.IdxForColumn("T_dec25_2");
      ci.iT_dec26_2        = TheG.IdxForColumn("T_dec26_2");
      ci.iT_dec27_2        = TheG.IdxForColumn("T_dec27_2");
      ci.iT_dec28_2        = TheG.IdxForColumn("T_dec28_2");
      ci.iT_dec29_2        = TheG.IdxForColumn("T_dec29_2");
      ci.iT_dec30_2        = TheG.IdxForColumn("T_dec30_2");
      ci.iT_dec31_2        = TheG.IdxForColumn("T_dec31_2");

      ci.iT_str01          = TheG.IdxForColumn("T_str01");
      ci.iT_str02          = TheG.IdxForColumn("T_str02");
      ci.iT_str03          = TheG.IdxForColumn("T_str03");
      ci.iT_str04          = TheG.IdxForColumn("T_str04");
      ci.iT_str05          = TheG.IdxForColumn("T_str05");
      ci.iT_str06          = TheG.IdxForColumn("T_str06");
      ci.iT_str07          = TheG.IdxForColumn("T_str07");
      ci.iT_str08          = TheG.IdxForColumn("T_str08");
      ci.iT_str09          = TheG.IdxForColumn("T_str09");
      ci.iT_str10          = TheG.IdxForColumn("T_str10");
      ci.iT_str11          = TheG.IdxForColumn("T_str11");
      ci.iT_str12          = TheG.IdxForColumn("T_str12");
      ci.iT_str13          = TheG.IdxForColumn("T_str13");
      ci.iT_str14          = TheG.IdxForColumn("T_str14");
      ci.iT_str15          = TheG.IdxForColumn("T_str15");
      ci.iT_str16          = TheG.IdxForColumn("T_str16");
      ci.iT_str17          = TheG.IdxForColumn("T_str17");
      ci.iT_str18          = TheG.IdxForColumn("T_str18");
      ci.iT_str19          = TheG.IdxForColumn("T_str19");
      ci.iT_str20          = TheG.IdxForColumn("T_str20");
      ci.iT_str21          = TheG.IdxForColumn("T_str21");
      ci.iT_str22          = TheG.IdxForColumn("T_str22");
      ci.iT_str23          = TheG.IdxForColumn("T_str23");
      ci.iT_str24          = TheG.IdxForColumn("T_str24");
      ci.iT_str25          = TheG.IdxForColumn("T_str25");
      ci.iT_str26          = TheG.IdxForColumn("T_str26");
      ci.iT_str27          = TheG.IdxForColumn("T_str27");
      ci.iT_str28          = TheG.IdxForColumn("T_str28");
      ci.iT_str29          = TheG.IdxForColumn("T_str29");
      ci.iT_str30          = TheG.IdxForColumn("T_str30");
      ci.iT_str31          = TheG.IdxForColumn("T_str31");

      ci.iT_RFS            = TheG.IdxForColumn("R_RFS");
      ci.iT_PFS            = TheG.IdxForColumn("R_PFS");
      ci.iT_MFS            = TheG.IdxForColumn("R_MFS");

      ci.iT_SumMoney       = TheG.IdxForColumn("R_SumMoney");

      ci.iT_finalCostEUR            = TheG.IdxForColumn("R_finalCostEUR"           );
      ci.iT_monthAge                = TheG.IdxForColumn("R_monthAge"               );
      ci.iT_ageSt                   = TheG.IdxForColumn("R_ageSt"                  );
      ci.iT_co2Emisija              = TheG.IdxForColumn("R_co2Emisija"             );
      ci.iT_ONENEur                 = TheG.IdxForColumn("R_ONENEur"                );
      ci.iT_RealMPC_EUR             = TheG.IdxForColumn("R_RealMPC_EUR"            );
      ci.iT_HR_history_TOTAL_Cij_Kn = TheG.IdxForColumn("R_HR_history_TOTAL_Cij_Kn");
      ci.iT_HR_historyOsnovnaCij_Kn = TheG.IdxForColumn("R_HR_historyOsnovnaCij_Kn");
      ci.iT_HR_historyOprermaCij_Kn = TheG.IdxForColumn("R_HR_historyOprermaCij_Kn");
      ci.iT_mm1Registracije         = TheG.IdxForColumn("R_mm1Registracije"        );
      ci.iT_yy1Registracije         = TheG.IdxForColumn("R_yy1Registracije"        );
      ci.iT_VNPCEur                 = TheG.IdxForColumn("R_VNPCEur"                );
      ci.iT_PPMV_historyEur         = TheG.IdxForColumn("R_PPMV_historyEur"        );
      ci.iT_PPMVeur                 = TheG.IdxForColumn("R_PPMVeur"                );
      ci.iT_toPayAutohouseEUR       = TheG.IdxForColumn("R_toPayAutohouseEUR"      );
      ci.iT_toPayTotalEur           = TheG.IdxForColumn("R_toPayTotalEur"          );

      ci.iT_date3 = TheG.IdxForColumn("T_date3");
      ci.iT_date4 = TheG.IdxForColumn("T_date4");

   }

   // Xtrano _____________________________________________________________________________________________________

   protected Xtrano_colIdx ci2;
   public Xtrano_colIdx DgvCI2 { get { return ci2; } }

   public struct Xtrano_colIdx
   {
      internal int iT_recID;
      internal int iT_serial;
      internal int iT_moneyA;
      internal int iT_opis_128;
      internal int iT_konto;
      internal int iT_devName;

   }

   private void SetXtranoColumnIndexes()
   {
      ci2 = new Xtrano_colIdx();

      ci2.iT_recID    = TheG2.IdxForColumn("T_recID");
      ci2.iT_serial   = TheG2.IdxForColumn("T_serial");
      ci2.iT_moneyA   = TheG2.IdxForColumn("T_moneyA");
      ci2.iT_opis_128 = TheG2.IdxForColumn("T_opis_128");
      ci2.iT_konto    = TheG2.IdxForColumn("T_konto");
      ci2.iT_devName  = TheG2.IdxForColumn("T_devName");
   }


   #endregion SetXtransColumnIndexes()

   #region Fld_

   public uint    Fld_DokNum     { get { return ZXC.ValOrZero_UInt(tbx_DokNum.Text); } set { tbx_DokNum.Text = value.ToString("000000"); } }
   public string  Fld_TT         { get { return tbx_TT.Text; } set { tbx_TT.Text = value; } }
   public string  Fld_TtOpis     { set { tbx_TtOpis.Text = value; } }

   public uint    Fld_TtNum
   {
      get { return ZXC.ValOrZero_UInt(tbx_TtNum.Text); }
      set
      {
         if(this is KOP_PTG_DUC) tbx_TtNum.Text = value.ToString("0000000");
         else                    tbx_TtNum.Text = value.ToString();
      }
   }
   public string  Fld_Napomena   { get { return tbx_Napomena.Text; } set { tbx_Napomena.Text = value; } }
   public int     Fld_IntA       { get { return tbx_intA.GetIntField(); } set { tbx_intA.PutIntField(value); } }

   public DateTime Fld_DokDate
   {
      get { return dtp_DokDate.Value; }
      set
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dtp_DokDate.Value = value;
         }
      }
   }
   public DateTime Fld_DateA
   {
      get { return dtp_dateA.Value; }
      set
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dtp_dateA.Value = value;
         }
      }
   }

   public string Fld_V1_tt       { get { return tbx_v1_tt.Text; }              set { tbx_v1_tt.Text = value; } }
   public uint Fld_V1_ttNum      { get { return tbx_v1_ttNum.GetUintField(); } set { tbx_v1_ttNum.PutUintField(value); } }
   public string Fld_V1_ttOpis   {                                             set { tbx_v1_ttOpis.Text = value; } }
   public string Fld_V2_tt       { get { return tbx_v2_tt.Text; }              set { tbx_v2_tt.Text = value; } }
   public uint Fld_V2_ttNum      { get { return tbx_v2_ttNum.GetUintField(); } set { tbx_v2_ttNum.PutUintField(value); } }
   public string Fld_V2_ttOpis   {                                             set { tbx_v2_ttOpis.Text = value; } }
   public string Fld_ProjektCD   { get { return tbx_ProjektCD.Text;   }        set { tbx_ProjektCD.Text = value; } }
   public string Fld_ExternLink1 { get { return tbx_externLink1.Text; }        set { tbx_externLink1.Text = value; } }
   public string Fld_ExternLink2 { get { return tbx_externLink2.Text; }        set { tbx_externLink2.Text = value; } }

   public uint   Fld_MtrosCD      { get { return tbx_mtros_cd.GetSomeRecIDField(); } set { tbx_mtros_cd.PutSomeRecIDField(value); } }
   public string Fld_MtrosCDAsTxt { get { return tbx_mtros_cd.Text;                } set { tbx_mtros_cd.Text = value; } }
   public string Fld_MtrosTK      { get { return tbx_mtros_tk.Text;                } set { tbx_mtros_tk.Text = value; } }
   public string Fld_MtrosName    { get { return tbx_mtros_Naziv.Text;             } set { tbx_mtros_Naziv.Text = value; } }

   #endregion Fld_

   #region PutFields(), GetFields()

   public override void PutDefaultDUCfields()
   {
      if(dbNavigationRestrictor.RestrictedValues[0] != null)
      {
         if(CtrlOK(tbx_TT))
         {
            Fld_TT = dbNavigationRestrictor.RestrictedValues[0];
            tbx_TT.EndEdit(); // Da se digne event OnExitSetLookUp_DataTakers
         }

         VvLookUpItem theLui = ZXC.luiListaMixerType.SingleOrDefault(lui => lui.Cd == Fld_TT);

         if(theLui != null)
         {
            Fld_TtOpis = theLui.Name;

            // 18.07.2018: 
            if(ZXC.IsSvDUH && this is PredNrdDUC && theLui.Cd == "PNA")
            {
               (this as PredNrdDUC).Fld_SkladCd = "10";
            }
         }
      }

      Put_NewTT_Num(TheVvDao.GetNextTtNum(TheDbConnection, Fld_TT, ""));

      PutDefaultBabyDUCfields();
   }

   private void dtp_DokDate_ValueChanged_SetAllRowDates(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      if(Fld_DokDate != DateTimePicker.MinimumDateTime && Fld_DokDate.Year != ZXC.projectYearFirstDay.Year) ZXC.aim_emsg(MessageBoxIcon.Warning, "Upozorenje. Zadali ste godinu koja nije 'radna'.");
      //if(Fld_DokDate != DateTimePicker.MinimumDateTime && Fld_DokDate      >  DateTime.Now)                 ZXC.aim_emsg(MessageBoxIcon.Warning, "Upozorenje. Zadali ste datum iz 'budućnosti'.");

      #region njet
      //DateTime dateOd, dateDo;

      //if(TheG.CI_OK(ci.iT_dateOd)) dateOd = TheG.GetDateCell(ci.iT_dateOd, 0, false);
      //else                         dateOd = DateTime.MinValue; 
      //if(TheG.CI_OK(ci.iT_dateDo)) dateDo = TheG.GetDateCell(ci.iT_dateDo, 0, false);
      //else                         dateDo = DateTime.MinValue;

      //if(dateOd != DateTime.MinValue ||
      //   dateDo != DateTime.MinValue) return;
      #endregion njet

      // ... here we go... 

      for(int rIdx = 0; rIdx < TheG.RowCount - 1; ++rIdx)
      {
         TheG.PutCell(ci.iT_dateOd, rIdx, Fld_DokDate);
         TheG.PutCell(ci.iT_dateDo, rIdx, Fld_DokDate);
      }

   }

   public override void PutFields(VvDataRecord _mixer)
   {
      PutFields(_mixer, false);
   }

   public override void PutFields(VvDataRecord _mixer, bool isCopyingToAnotherDUC)
   {
      // 06.05.2016: 
    //mixer_rec            = (Mixer)_mixer; 
      Mixer mixerLocal_rec = (Mixer)_mixer;

      // 06.05.2016: 
      this.mixer_rec.TakeTransesFrom (mixerLocal_rec);
      this.mixer_rec.TakeTransesFrom2(mixerLocal_rec);
      this.mixer_rec.TakeTransesFrom3(mixerLocal_rec);

      if(this is GFI_TSI_DUC)
      {
         (this as GFI_TSI_DUC).GFI_TSI_LoadBilancaData();

       //if(this is Statistika_NPF_DUC)
       //{
       //   (this as Statistika_NPF_DUC).GFI_TSI_SINT_BilancaData(mixerLocal_rec.Transes);
       //}
      }

      if(mixerLocal_rec != null)
      {
         PutMetaFileds(mixerLocal_rec.AddUID, mixerLocal_rec.AddTS, mixerLocal_rec.ModUID, mixerLocal_rec.ModTS, mixerLocal_rec.RecID, mixerLocal_rec.LanSrvID, mixerLocal_rec.LanRecID);

         PutIdentityFields(mixerLocal_rec.TT + "-" + mixerLocal_rec.TtNum.ToString(/*"000000"*/), mixerLocal_rec.DokDate.ToString(ZXC.VvDateFormat), "", "");

         VvHamper.SetChkBoxRadBttnAutoCheck(this, true);

         if(!isCopyingToAnotherDUC) Fld_DokNum   = mixerLocal_rec.DokNum  ;
                                    Fld_DokDate  = mixerLocal_rec.DokDate ;
         if(!isCopyingToAnotherDUC) Fld_TT       = mixerLocal_rec.TT      ;
         if(!isCopyingToAnotherDUC) Fld_TtNum    = mixerLocal_rec.TtNum   ;
                                    Fld_Napomena = mixerLocal_rec.Napomena;
                                    Fld_DateA    = mixerLocal_rec.DateA   ;
                                    Fld_IntA     = mixerLocal_rec.IntA    ;
         
         if(CtrlOK(tbx_mtros_cd)) Fld_MtrosCD = mixerLocal_rec.MtrosCD;
         if(CtrlOK(tbx_mtros_tk)) Fld_MtrosTK = mixerLocal_rec.MtrosTK;

         if(CtrlOK(tbx_mtros_tk))
         {
            SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.None);

            Kupdob kupdobSifrar_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpdb => kpdb.KupdobCD == mixerLocal_rec.MtrosCD);

            if(kupdobSifrar_rec != null && mixerLocal_rec.MtrosCD.NotZero())
            {
               Fld_MtrosName = kupdobSifrar_rec.Naziv;
            }
            else
            {
               Fld_MtrosName = "";
            }
         }


         // 06.05.2016: 
         if(isCopyingToAnotherDUC) PutSpecificsFld(mixerLocal_rec);
         else                      PutSpecificsFld(              );

         AddToolTipToTbx();

         PutDgvFields();

         VvHamper.SetChkBoxRadBttnAutoCheck(this, false);

         InitializeFilterUCFields();

         recordReportLoaded = false;
         DecideIfShouldLoad_VvReport(null, null, null);

      }

      TheG.ClearSelection();
      TheSumGrid.ClearSelection();

      TheG2.ClearSelection();
      TheSumGrid2.ClearSelection();

      if(this is GFI_TSI_DUC) GFI_ZeroFillInt();

      // da vrati 'sifrarRecordName' nazad na kupdob 
    //if(this is ZahtjevRNMDUC                                  ) SetSifrarAndAutocomplete<Kupdob>((ControlForInitialFocus as VvTextBox), VvSQL.SorterType.Name);
      if(this is ZahtjevRNMDUC || this is NazivArtiklaZaKupcaDUC) SetSifrarAndAutocomplete<Kupdob>((ControlForInitialFocus as VvTextBox), VvSQL.SorterType.Name);
      if(this is KDCDUC)                                          SetSifrarAndAutocomplete<Kupdob>((ControlForInitialFocus as VvTextBox), VvSQL.SorterType.Name);

   }

   private void AddToolTipToTbx()
   {
      tbx_Napomena.TextAsToolTip(toolTip);
   }

   // Tu dolazimo na 1 nacin: 1. Classic PutFields 
   private void InitializeFilterUCFields()
   {
      SetFilterRecordDependentDefaults(); // filter.DokNumOd = pr.DokNum (punimo bussiness od filtera, ne UC)
      //mozdaBitno      TheMixerDokumentFilterUC.PutFilterFields(TheMixerDokumentFilter);
   }

   public override void GetFields(bool dirtyFlagging)
   {
      mixer_rec.DokNum = Fld_DokNum;
      mixer_rec.DokDate = Fld_DokDate;
      mixer_rec.TT = Fld_TT;
      mixer_rec.TtNum = Fld_TtNum;
      mixer_rec.Napomena = Fld_Napomena;
      mixer_rec.DateA = Fld_DateA;
      mixer_rec.IntA = Fld_IntA;
      if(CtrlOK(tbx_mtros_cd)) mixer_rec.MtrosCD = Fld_MtrosCD;
      if(CtrlOK(tbx_mtros_tk)) mixer_rec.MtrosTK = Fld_MtrosTK;

      GetSpecificsFld();

      // 23.01.2018: GetDgvFields opkoljen if-om 
      if((this is BmwDUC) == false)
      {
         GetDgvFields(dirtyFlagging);
      }
   }

   #region Put_NewDocum_NumAndDateFields

   public override void Put_NewDocum_NumAndDateFields(uint dokNum, DateTime dokDate)
   {
      Fld_DokNum = dokNum;
      Fld_DokDate = dokDate;
   }

   public override void Put_NewTT_Num(uint ttNum)
   {
      Fld_TtNum = ttNum;
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
      string UGAN_tt;

      if(this is KOP_PTG_DUC)
      {
         //PutDgvFields1_KOP_PTG_DUC();
         //return;

         UGAN_tt = mixer_rec.V1_ttNum.IsZero() ? Faktur.TT_UGN : Faktur.TT_AUN;

         PTG_OtplatniPlan otplatniPlan = new PTG_OtplatniPlan(TheDbConnection, UGAN_tt, mixer_rec.TtNum);

         mixer_rec.Transes.Clear();

         Xtrans xtrans_rec;

         foreach(PTG_Rata rata in otplatniPlan.UGAN_RateList)
         {
            if(rata.HasKOP) xtrans_rec = rata.xtrans_rec;
            else            xtrans_rec = otplatniPlan.Create_Xtrans_FromPTG_Rata(rata);

            mixer_rec.Transes.Add(xtrans_rec);
         }
      }

      int rowIdx, idxCorrector;

      TheG.RowsRemoved -= new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);
      TheG.RowsAdded -= new DataGridViewRowsAddedEventHandler(grid_RowsAdded);
      TheG.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(TheG);

      if(this is PredNrdDUC)
      {
         PNA_Missing_UGO_TransList = new List<Rtrans>();
      }

      if(mixer_rec.Transes != null)
      {
         SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

         foreach(Xtrans xtrans_rec in mixer_rec.Transes)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheG.Rows.Add();

            rowIdx = TheG.RowCount - idxCorrector;

            PutDgvLineFields1(xtrans_rec, rowIdx, false);

            xtrans_rec.CalcTransResults(mixer_rec);

            PutDgvLineResultsFields1(rowIdx, xtrans_rec, false);
         }
      }
      TheG.RowsAdded += new DataGridViewRowsAddedEventHandler(grid_RowsAdded);
      TheG.RowsRemoved += new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);

      RenumerateLineNumbers(TheG, 0);

      UpdateLineCount(TheG);

      if(this is Statistika_NPF_DUC)
      {
         (this as Statistika_NPF_DUC).GFI_TSI_SINT_BilancaData(mixer_rec.Transes);
      }

      PutDgvTransSumFields();

      CalcLocationHamperBeloWGrid(); // mora ovdje kad se napuni grid onda se (ne)pojavi scrol koji onda sve poseremeti tt 03.02.2011.

      if(this is KOP_PTG_DUC)((KOP_PTG_DUC)this).DGV_PTG_HasKOP_CellValueChangedColor();

   }

   public List<Rtrans> PNA_Missing_UGO_TransList;

   public override void PutDgvLineFields1(VvTransRecord trans_rec, int rowIdx, bool skipRecID_andSerial_Columns)
   {
      Xtrans xtrans_rec = (Xtrans)trans_rec;

      if(skipRecID_andSerial_Columns == false)
      {
         TheG.PutCell(ci.iT_recID, rowIdx, xtrans_rec.T_recID);
         TheG.PutCell(ci.iT_serial, rowIdx, xtrans_rec.T_serial);
      }

      TheG.PutCell(ci.iT_moneyA        , rowIdx, xtrans_rec.T_moneyA);
      TheG.PutCell(ci.iT_kol           , rowIdx, xtrans_rec.T_kol);
      TheG.PutCell(ci.iT_opis_128      , rowIdx, xtrans_rec.T_opis_128);
      TheG.PutCell(ci.iT_kpdbNameA_50  , rowIdx, xtrans_rec.T_kpdbNameA_50);
      TheG.PutCell(ci.iT_kpdbUlBrA_32  , rowIdx, xtrans_rec.T_kpdbUlBrA_32);
      TheG.PutCell(ci.iT_kpdbMjestoA_32, rowIdx, xtrans_rec.T_kpdbMjestoA_32);
      TheG.PutCell(ci.iT_kpdbZiroA_32  , rowIdx, xtrans_rec.T_kpdbZiroA_32);
      TheG.PutCell(ci.iT_kpdbNameB_50  , rowIdx, xtrans_rec.T_kpdbNameB_50);
      TheG.PutCell(ci.iT_kpdbUlBrB_32  , rowIdx, xtrans_rec.T_kpdbUlBrB_32);
      TheG.PutCell(ci.iT_kpdbMjestoB_32, rowIdx, xtrans_rec.T_kpdbMjestoB_32);
      TheG.PutCell(ci.iT_kpdbZiroB_32  , rowIdx, xtrans_rec.T_kpdbZiroB_32);
      TheG.PutCell(ci.iT_dateOd        , rowIdx, xtrans_rec.T_dateOd);
      TheG.PutCell(ci.iT_dateDo        , rowIdx, xtrans_rec.T_dateDo);
      TheG.PutCell(ci.iT_strA_2        , rowIdx, xtrans_rec.T_strA_2);
      TheG.PutCell(ci.iT_strB_2        , rowIdx, xtrans_rec.T_strB_2);
      TheG.PutCell(ci.iT_vezniDokA_64  , rowIdx, xtrans_rec.T_vezniDokA_64);
      TheG.PutCell(ci.iT_vezniDokB_64  , rowIdx, xtrans_rec.T_vezniDokB_64);
      TheG.PutCell(ci.iT_strC_2        , rowIdx, xtrans_rec.T_strC_2);
      TheG.PutCell(ci.iT_kupdobCD      , rowIdx, xtrans_rec.T_kupdobCD);
      TheG.PutCell(ci.iT_artiklCD      , rowIdx, xtrans_rec.T_artiklCD);
      TheG.PutCell(ci.iT_artiklName    , rowIdx, xtrans_rec.T_artiklName);
      TheG.PutCell(ci.iT_isXxx         , rowIdx, VvCheckBox.GetString4Bool(xtrans_rec.T_isXxx));
      TheG.PutCell(ci.iT_intA          , rowIdx, xtrans_rec.T_intA);
      TheG.PutCell(ci.iT_intB          , rowIdx, xtrans_rec.T_intB);
      TheG.PutCell(ci.iT_konto         , rowIdx, xtrans_rec.T_konto);
      TheG.PutCell(ci.iT_personCD      , rowIdx, xtrans_rec.T_personCD);
      TheG.PutCell(ci.iT_moneyB        , rowIdx, xtrans_rec.T_moneyB);
      TheG.PutCell(ci.iT_moneyC        , rowIdx, xtrans_rec.T_moneyC);
      TheG.PutCell(ci.iT_moneyD        , rowIdx, xtrans_rec.T_moneyD);

      TheG.PutCell(ci.iT_str01         , rowIdx, xtrans_rec.T_str01  );
      TheG.PutCell(ci.iT_str02         , rowIdx, xtrans_rec.T_str02  );
      TheG.PutCell(ci.iT_str03         , rowIdx, xtrans_rec.T_str03  );
      TheG.PutCell(ci.iT_str04         , rowIdx, xtrans_rec.T_str04  );
      TheG.PutCell(ci.iT_str05         , rowIdx, xtrans_rec.T_str05  );
      TheG.PutCell(ci.iT_str06         , rowIdx, xtrans_rec.T_str06  );
      TheG.PutCell(ci.iT_str07         , rowIdx, xtrans_rec.T_str07  );
      TheG.PutCell(ci.iT_str08         , rowIdx, xtrans_rec.T_str08  );
      TheG.PutCell(ci.iT_str09         , rowIdx, xtrans_rec.T_str09  );
      TheG.PutCell(ci.iT_str10         , rowIdx, xtrans_rec.T_str10  );
      TheG.PutCell(ci.iT_str11         , rowIdx, xtrans_rec.T_str11  );
      TheG.PutCell(ci.iT_str12         , rowIdx, xtrans_rec.T_str12  );
      TheG.PutCell(ci.iT_str13         , rowIdx, xtrans_rec.T_str13  );
      TheG.PutCell(ci.iT_str14         , rowIdx, xtrans_rec.T_str14  );
      TheG.PutCell(ci.iT_str15         , rowIdx, xtrans_rec.T_str15  );
      TheG.PutCell(ci.iT_str16         , rowIdx, xtrans_rec.T_str16  );
      TheG.PutCell(ci.iT_str17         , rowIdx, xtrans_rec.T_str17  );
      TheG.PutCell(ci.iT_str18         , rowIdx, xtrans_rec.T_str18  );
      TheG.PutCell(ci.iT_str19         , rowIdx, xtrans_rec.T_str19  );
      TheG.PutCell(ci.iT_str20         , rowIdx, xtrans_rec.T_str20  );
      TheG.PutCell(ci.iT_str21         , rowIdx, xtrans_rec.T_str21  );
      TheG.PutCell(ci.iT_str22         , rowIdx, xtrans_rec.T_str22  );
      TheG.PutCell(ci.iT_str23         , rowIdx, xtrans_rec.T_str23  );
      TheG.PutCell(ci.iT_str24         , rowIdx, xtrans_rec.T_str24  );
      TheG.PutCell(ci.iT_str25         , rowIdx, xtrans_rec.T_str25  );
      TheG.PutCell(ci.iT_str26         , rowIdx, xtrans_rec.T_str26  );
      TheG.PutCell(ci.iT_str27         , rowIdx, xtrans_rec.T_str27  );
      TheG.PutCell(ci.iT_str28         , rowIdx, xtrans_rec.T_str28  );
      TheG.PutCell(ci.iT_str29         , rowIdx, xtrans_rec.T_str29  );
      TheG.PutCell(ci.iT_str30         , rowIdx, xtrans_rec.T_str30  );
      TheG.PutCell(ci.iT_str31         , rowIdx, xtrans_rec.T_str31  );

      TheG.PutCell(ci.iT_date3         , rowIdx, xtrans_rec.T_date3  );
      TheG.PutCell(ci.iT_date4         , rowIdx, xtrans_rec.T_date4  );

      // 28.11.2017: novo tretiranje decimala za AVR (AVR UC oce DateTime iz decimala) 
      if(this is AvrDUC == false) // tihis is NOT AVR DUC 
      {
#region NOT AVR DUC
      TheG.PutCell(ci.iT_dec01         , rowIdx, xtrans_rec.T_dec01  );
      TheG.PutCell(ci.iT_dec02         , rowIdx, xtrans_rec.T_dec02  );
      TheG.PutCell(ci.iT_dec03         , rowIdx, xtrans_rec.T_dec03  );
      TheG.PutCell(ci.iT_dec04         , rowIdx, xtrans_rec.T_dec04  );
      TheG.PutCell(ci.iT_dec05         , rowIdx, xtrans_rec.T_dec05  );
      TheG.PutCell(ci.iT_dec06         , rowIdx, xtrans_rec.T_dec06  );
      TheG.PutCell(ci.iT_dec07         , rowIdx, xtrans_rec.T_dec07  );
      TheG.PutCell(ci.iT_dec08         , rowIdx, xtrans_rec.T_dec08  );
      TheG.PutCell(ci.iT_dec09         , rowIdx, xtrans_rec.T_dec09  );
      TheG.PutCell(ci.iT_dec10         , rowIdx, xtrans_rec.T_dec10  );
      TheG.PutCell(ci.iT_dec11         , rowIdx, xtrans_rec.T_dec11  );
      TheG.PutCell(ci.iT_dec12         , rowIdx, xtrans_rec.T_dec12  );
      TheG.PutCell(ci.iT_dec13         , rowIdx, xtrans_rec.T_dec13  );
      TheG.PutCell(ci.iT_dec14         , rowIdx, xtrans_rec.T_dec14  );
      TheG.PutCell(ci.iT_dec15         , rowIdx, xtrans_rec.T_dec15  );
      TheG.PutCell(ci.iT_dec16         , rowIdx, xtrans_rec.T_dec16  );
      TheG.PutCell(ci.iT_dec17         , rowIdx, xtrans_rec.T_dec17  );
      TheG.PutCell(ci.iT_dec18         , rowIdx, xtrans_rec.T_dec18  );
      TheG.PutCell(ci.iT_dec19         , rowIdx, xtrans_rec.T_dec19  );
      TheG.PutCell(ci.iT_dec20         , rowIdx, xtrans_rec.T_dec20  );
      TheG.PutCell(ci.iT_dec21         , rowIdx, xtrans_rec.T_dec21  );
      TheG.PutCell(ci.iT_dec22         , rowIdx, xtrans_rec.T_dec22  );
      TheG.PutCell(ci.iT_dec23         , rowIdx, xtrans_rec.T_dec23  );
      TheG.PutCell(ci.iT_dec24         , rowIdx, xtrans_rec.T_dec24  );
      TheG.PutCell(ci.iT_dec25         , rowIdx, xtrans_rec.T_dec25  );
      TheG.PutCell(ci.iT_dec26         , rowIdx, xtrans_rec.T_dec26  );
      TheG.PutCell(ci.iT_dec27         , rowIdx, xtrans_rec.T_dec27  );
      TheG.PutCell(ci.iT_dec28         , rowIdx, xtrans_rec.T_dec28  );
      TheG.PutCell(ci.iT_dec29         , rowIdx, xtrans_rec.T_dec29  );
      TheG.PutCell(ci.iT_dec30         , rowIdx, xtrans_rec.T_dec30  );
      TheG.PutCell(ci.iT_dec31         , rowIdx, xtrans_rec.T_dec31  );

      TheG.PutCell(ci.iT_dec01_2       , rowIdx, xtrans_rec.T_dec01_2);
      TheG.PutCell(ci.iT_dec02_2       , rowIdx, xtrans_rec.T_dec02_2);
      TheG.PutCell(ci.iT_dec03_2       , rowIdx, xtrans_rec.T_dec03_2);
      TheG.PutCell(ci.iT_dec04_2       , rowIdx, xtrans_rec.T_dec04_2);
      TheG.PutCell(ci.iT_dec05_2       , rowIdx, xtrans_rec.T_dec05_2);
      TheG.PutCell(ci.iT_dec06_2       , rowIdx, xtrans_rec.T_dec06_2);
      TheG.PutCell(ci.iT_dec07_2       , rowIdx, xtrans_rec.T_dec07_2);
      TheG.PutCell(ci.iT_dec08_2       , rowIdx, xtrans_rec.T_dec08_2);
      TheG.PutCell(ci.iT_dec09_2       , rowIdx, xtrans_rec.T_dec09_2);
      TheG.PutCell(ci.iT_dec10_2       , rowIdx, xtrans_rec.T_dec10_2);
      TheG.PutCell(ci.iT_dec11_2       , rowIdx, xtrans_rec.T_dec11_2);
      TheG.PutCell(ci.iT_dec12_2       , rowIdx, xtrans_rec.T_dec12_2);
      TheG.PutCell(ci.iT_dec13_2       , rowIdx, xtrans_rec.T_dec13_2);
      TheG.PutCell(ci.iT_dec14_2       , rowIdx, xtrans_rec.T_dec14_2);
      TheG.PutCell(ci.iT_dec15_2       , rowIdx, xtrans_rec.T_dec15_2);
      TheG.PutCell(ci.iT_dec16_2       , rowIdx, xtrans_rec.T_dec16_2);
      TheG.PutCell(ci.iT_dec17_2       , rowIdx, xtrans_rec.T_dec17_2);
      TheG.PutCell(ci.iT_dec18_2       , rowIdx, xtrans_rec.T_dec18_2);
      TheG.PutCell(ci.iT_dec19_2       , rowIdx, xtrans_rec.T_dec19_2);
      TheG.PutCell(ci.iT_dec20_2       , rowIdx, xtrans_rec.T_dec20_2);
      TheG.PutCell(ci.iT_dec21_2       , rowIdx, xtrans_rec.T_dec21_2);
      TheG.PutCell(ci.iT_dec22_2       , rowIdx, xtrans_rec.T_dec22_2);
      TheG.PutCell(ci.iT_dec23_2       , rowIdx, xtrans_rec.T_dec23_2);
      TheG.PutCell(ci.iT_dec24_2       , rowIdx, xtrans_rec.T_dec24_2);
      TheG.PutCell(ci.iT_dec25_2       , rowIdx, xtrans_rec.T_dec25_2);
      TheG.PutCell(ci.iT_dec26_2       , rowIdx, xtrans_rec.T_dec26_2);
      TheG.PutCell(ci.iT_dec27_2       , rowIdx, xtrans_rec.T_dec27_2);
      TheG.PutCell(ci.iT_dec28_2       , rowIdx, xtrans_rec.T_dec28_2);
      TheG.PutCell(ci.iT_dec29_2       , rowIdx, xtrans_rec.T_dec29_2);
      TheG.PutCell(ci.iT_dec30_2       , rowIdx, xtrans_rec.T_dec30_2);
      TheG.PutCell(ci.iT_dec31_2       , rowIdx, xtrans_rec.T_dec31_2);
#endregion NOT AVR DUC
      }
      else // tihis IS AVR DUC 
      {
#region AVR DUC
      TheG.PutCell(ci.iT_dec01         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec01  ));
      TheG.PutCell(ci.iT_dec02         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec02  ));
      TheG.PutCell(ci.iT_dec03         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec03  ));
      TheG.PutCell(ci.iT_dec04         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec04  ));
      TheG.PutCell(ci.iT_dec05         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec05  ));
      TheG.PutCell(ci.iT_dec06         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec06  ));
      TheG.PutCell(ci.iT_dec07         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec07  ));
      TheG.PutCell(ci.iT_dec08         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec08  ));
      TheG.PutCell(ci.iT_dec09         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec09  ));
      TheG.PutCell(ci.iT_dec10         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec10  ));
      TheG.PutCell(ci.iT_dec11         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec11  ));
      TheG.PutCell(ci.iT_dec12         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec12  ));
      TheG.PutCell(ci.iT_dec13         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec13  ));
      TheG.PutCell(ci.iT_dec14         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec14  ));
      TheG.PutCell(ci.iT_dec15         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec15  ));
      TheG.PutCell(ci.iT_dec16         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec16  ));
      TheG.PutCell(ci.iT_dec17         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec17  ));
      TheG.PutCell(ci.iT_dec18         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec18  ));
      TheG.PutCell(ci.iT_dec19         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec19  ));
      TheG.PutCell(ci.iT_dec20         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec20  ));
      TheG.PutCell(ci.iT_dec21         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec21  ));
      TheG.PutCell(ci.iT_dec22         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec22  ));
      TheG.PutCell(ci.iT_dec23         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec23  ));
      TheG.PutCell(ci.iT_dec24         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec24  ));
      TheG.PutCell(ci.iT_dec25         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec25  ));
      TheG.PutCell(ci.iT_dec26         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec26  ));
      TheG.PutCell(ci.iT_dec27         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec27  ));
      TheG.PutCell(ci.iT_dec28         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec28  ));
      TheG.PutCell(ci.iT_dec29         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec29  ));
      TheG.PutCell(ci.iT_dec30         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec30  ));
      TheG.PutCell(ci.iT_dec31         , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec31  ));
                                                 
      TheG.PutCell(ci.iT_dec01_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec01_2));
      TheG.PutCell(ci.iT_dec02_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec02_2));
      TheG.PutCell(ci.iT_dec03_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec03_2));
      TheG.PutCell(ci.iT_dec04_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec04_2));
      TheG.PutCell(ci.iT_dec05_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec05_2));
      TheG.PutCell(ci.iT_dec06_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec06_2));
      TheG.PutCell(ci.iT_dec07_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec07_2));
      TheG.PutCell(ci.iT_dec08_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec08_2));
      TheG.PutCell(ci.iT_dec09_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec09_2));
      TheG.PutCell(ci.iT_dec10_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec10_2));
      TheG.PutCell(ci.iT_dec11_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec11_2));
      TheG.PutCell(ci.iT_dec12_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec12_2));
      TheG.PutCell(ci.iT_dec13_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec13_2));
      TheG.PutCell(ci.iT_dec14_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec14_2));
      TheG.PutCell(ci.iT_dec15_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec15_2));
      TheG.PutCell(ci.iT_dec16_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec16_2));
      TheG.PutCell(ci.iT_dec17_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec17_2));
      TheG.PutCell(ci.iT_dec18_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec18_2));
      TheG.PutCell(ci.iT_dec19_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec19_2));
      TheG.PutCell(ci.iT_dec20_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec20_2));
      TheG.PutCell(ci.iT_dec21_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec21_2));
      TheG.PutCell(ci.iT_dec22_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec22_2));
      TheG.PutCell(ci.iT_dec23_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec23_2));
      TheG.PutCell(ci.iT_dec24_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec24_2));
      TheG.PutCell(ci.iT_dec25_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec25_2));
      TheG.PutCell(ci.iT_dec26_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec26_2));
      TheG.PutCell(ci.iT_dec27_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec27_2));
      TheG.PutCell(ci.iT_dec28_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec28_2));
      TheG.PutCell(ci.iT_dec29_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec29_2));
      TheG.PutCell(ci.iT_dec30_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec30_2));
      TheG.PutCell(ci.iT_dec31_2       , rowIdx, ZXC.GetDateTimeFromDecimal_HHMMonly(xtrans_rec.T_dec31_2));
#endregion AVR DUC
      }


      if(this is GFI_TSI_DUC || this is Statistika_NPF_DUC) DataGridView_CellValueChanged_GFI();

      #region PPMV Calc Trosarine

      if(this is PmvDUC)
      {
         Artikl artikl_rec = ArtiklSifrar.SingleOrDefault(artikl => artikl.ArtiklCD == xtrans_rec.T_artiklCD);
         if(artikl_rec != null) 
         {
            //TheG.PutCell(ci.iT_moneyB, rowIdx, artikl_rec.EmisCO2  );
            //TheG.PutCell(ci.iT_moneyC, rowIdx, artikl_rec.Zapremina);
            //TheG.PutCell(ci.iT_intA  , rowIdx, artikl_rec.EuroNorma_u);

            decimal osnovica = xtrans_rec.T_moneyA;
            bool isDizel = false; // TODO 

            decimal stopaPO  = Artikl.GetPpmvStopaFor_PorOsn(osnovica);
            decimal stopaCO2 = isDizel ?
                               Artikl.GetPpmvStopaFor_CO2_Dizel(artikl_rec.EmisCO2, artikl_rec.EuroNorma) : Artikl.GetPpmvStopaFor_CO2_Benz(artikl_rec.EmisCO2);
            decimal stopaCM3 = Artikl.GetPpmvStopaFor_CM3(artikl_rec.Zapremina);
            decimal stopaEN  = Artikl.GetPpmvStopaFor_EuroNorma(artikl_rec.EuroNorma);

            TheG.PutCell(ci.iT_st_PO , rowIdx, stopaPO );
            TheG.PutCell(ci.iT_st_CO2, rowIdx, stopaCO2);
            TheG.PutCell(ci.iT_st_cm3, rowIdx, stopaCM3);
            TheG.PutCell(ci.iT_st_EUN, rowIdx, stopaEN );

            TheG.PutCell(ci.iT_iznos_PO , rowIdx, ZXC.VvGet_25_of_100(osnovica, stopaPO ));
            TheG.PutCell(ci.iT_iznos_CO2, rowIdx, ZXC.VvGet_25_of_100(osnovica, stopaCO2));
            TheG.PutCell(ci.iT_iznos_cm3, rowIdx, ZXC.VvGet_25_of_100(osnovica, stopaCM3));
            TheG.PutCell(ci.iT_iznos_EUN, rowIdx, ZXC.VvGet_25_of_100(osnovica, stopaEN ));

         }
      }

      #endregion PPMV Calc Trosarine

      if(this is PredNrdDUC)
      {
         Rtrans UGOrtrans_rec = new Rtrans();

         bool UGOrtransFound = FakturDao.SetMeLastRtransForArtiklAndTT(TheDbConnection, UGOrtrans_rec, Faktur.TT_UGO, xtrans_rec.T_artiklCD, /*false*/true);

         if(UGOrtransFound)
         {
            Kupdob kupdob_rec = Get_Kupdob_FromVvUcSifrar(UGOrtrans_rec.T_kupdobCD);

            TheG.PutCell(ci.iT_kpdbNameA_50, rowIdx, kupdob_rec.Naziv              );
            TheG.PutCell(ci.iT_kpdbZiroA_32, rowIdx, "UGO-" + UGOrtrans_rec.T_ttNum);
         }
         else
         {
            PNA_Missing_UGO_TransList.Add(new Rtrans() { T_serial = (ushort)rowIdx, T_artiklCD = xtrans_rec.T_artiklCD, T_artiklName = xtrans_rec.T_artiklName } );
         }
      }

      if(this is KOP_PTG_DUC)
      {
            /* Clasic CheckBox: */ TheG.PutCell(ci.iT_intB, rowIdx, xtrans_rec.T_intB.NotZero() ? true : false);

          ///* VvCheckBox: */      TheG.PutCell(ci.iT_intB, rowIdx, VvCheckBox.GetString4Bool(xtrans_rec.T_intB.NotZero()));
      }
   }

   protected void T_isFakStop_CreateColumn(int _width, string _colHeader)
   {
      { // Clasic CheckBox: 
         cbx_fakStop = new CheckBox();
         colCbxClassic = TheG.CreateClassicCheckBoxColumn(cbx_fakStop, TheVvDaoTrans, DB_Tci.t_intB, _colHeader, _width);
         TheG.CellClick += TheG_CellClick;

      }

      //{ // VvCheckBox: 
      //   vvcbx_isIntB = new VvCheckBox();
      //   colCbox = TheG.CreateVvCheckBoxColumn(vvcbx_isIntB, TheVvDaoTrans, DB_Tci.t_intB, _colHeader, _width);
      //}
   }
   private void TheG_CellClick(object sender, DataGridViewCellEventArgs e)
   {
      DataGridViewCell cell = ((System.Windows.Forms.DataGridView)sender).CurrentCell;

      if(cell is DataGridViewCheckBoxCell) SendKeys.Send("{TAB}");
   }


   public override void PutDgvLineResultsFields1(int rowIdx, VvTransRecord trans_rec, bool passPtrResultsToZaglavljeTranses)
   {
      Xtrans Xtrans_rec = trans_rec as Xtrans;

      // 24.02.2022: 
    //if(passPtrResultsToZaglavljeTranses == true)
      if(passPtrResultsToZaglavljeTranses == true && mixer_rec.Transes.NotEmpty())
      {
         // NotaBene: bez ove provjere za Deleted bi ti se kada pobrises npr drugi (kao zadnji) row pa ga ponovno dodas i izazoves CalcTranses() skoci 
         // Exception da {"Sequence contains more than one matching element"}
         // u PutDgvLineResultsFields1():       if(passPtrResultsToZaglavljeTranses == true)... jerbo uprkos deletanju DGV row-a u biznisu deletani trans ostane! 
         Xtrans xtrans =
            mixer_rec.Transes.SingleOrDefault(ptr => ptr.T_serial == Xtrans_rec.T_serial && ptr.SaveTransesWriteMode != ZXC.WriteMode.Delete);
         if(xtrans != null)
         { 
            xtrans.XtrResults = Xtrans_rec.XtrResults;
         }
      }

       if(this is RasterBDUC  ) TheG.PutCell(ci.iT_kolMoneyA, rowIdx, Xtrans_rec.R_KolMoneyABC);
       else                     TheG.PutCell(ci.iT_kolMoneyA, rowIdx, Xtrans_rec.R_KolMoneyA);

      if(this is RvrMjesecDUC)
      {
         TheG.PutCell(ci.iT_RFS, rowIdx, Xtrans_rec.R_MVR_RFS);
         TheG.PutCell(ci.iT_PFS, rowIdx, Xtrans_rec.R_MVR_PFS);
         TheG.PutCell(ci.iT_MFS, rowIdx, Xtrans_rec.R_MVR_MFS);
      }
      if(this is PlanDUC)
      {
         TheG.PutCell(ci.iT_SumMoney, rowIdx, Xtrans_rec.R_SumMoney);
      }
   }

   public override void PutDgvTransSumFields1()
   {
      if(this is RasterBDUC) TheSumGrid.PutCell(ci.iT_kolMoneyA, 0, mixer_rec.Sum_KolMoneyABC);
      else                   TheSumGrid.PutCell(ci.iT_kolMoneyA, 0, mixer_rec.Sum_KolMoneyA);

      if(this is PmvDUC || this is PlanDUC)
      { }
      else
      {
         if(this is PutRadListDUC) { }
         else                      TheSumGrid.PutCell(ci.iT_moneyB, 0, mixer_rec.Sum_MoneyB);
         TheSumGrid.PutCell(ci.iT_moneyC, 0, mixer_rec.Sum_MoneyC);
         TheSumGrid.PutCell(ci.iT_moneyD, 0, mixer_rec.Sum_MoneyD);
      }
      if(this is PutNalDUC && TheG.RowCount.NotZero())
      {
         TheSumGrid.PutCell(ci.iT_kol, 0, mixer_rec.Sum_Kol);
         TheSumGrid.PutCell(ci.iT_intA, 0, TheG.GetIntCell(ci.iT_intA, 0, false));
         TheSumGrid.PutCell(ci.iT_intB, 0, TheG.GetIntCell(ci.iT_intB, TheG.RowCount - 1, false));

      }

      if((this is SmdDUC || this is InterniRvrDUC) && TheG.RowCount.NotZero())
      {
         TheSumGrid.PutCell(ci.iT_kol, 0, mixer_rec.Sum_Kol);
      }

      if(this is RvrMjesecDUC)
      {
         TheSumGrid.PutCell(ci.iT_dec01, 0, mixer_rec.Sum_Dec01);
         TheSumGrid.PutCell(ci.iT_dec02, 0, mixer_rec.Sum_Dec02);
         TheSumGrid.PutCell(ci.iT_dec03, 0, mixer_rec.Sum_Dec03);
         TheSumGrid.PutCell(ci.iT_dec04, 0, mixer_rec.Sum_Dec04);
         TheSumGrid.PutCell(ci.iT_dec05, 0, mixer_rec.Sum_Dec05);
         TheSumGrid.PutCell(ci.iT_dec06, 0, mixer_rec.Sum_Dec06);
         TheSumGrid.PutCell(ci.iT_dec07, 0, mixer_rec.Sum_Dec07);
         TheSumGrid.PutCell(ci.iT_dec08, 0, mixer_rec.Sum_Dec08);
         TheSumGrid.PutCell(ci.iT_dec09, 0, mixer_rec.Sum_Dec09);
         TheSumGrid.PutCell(ci.iT_dec10, 0, mixer_rec.Sum_Dec10);
         TheSumGrid.PutCell(ci.iT_dec11, 0, mixer_rec.Sum_Dec11);
         TheSumGrid.PutCell(ci.iT_dec12, 0, mixer_rec.Sum_Dec12);
         TheSumGrid.PutCell(ci.iT_dec13, 0, mixer_rec.Sum_Dec13);
         TheSumGrid.PutCell(ci.iT_dec14, 0, mixer_rec.Sum_Dec14);
         TheSumGrid.PutCell(ci.iT_dec15, 0, mixer_rec.Sum_Dec15);
         TheSumGrid.PutCell(ci.iT_dec16, 0, mixer_rec.Sum_Dec16);
         TheSumGrid.PutCell(ci.iT_dec17, 0, mixer_rec.Sum_Dec17);
         TheSumGrid.PutCell(ci.iT_dec18, 0, mixer_rec.Sum_Dec18);
         TheSumGrid.PutCell(ci.iT_dec19, 0, mixer_rec.Sum_Dec19);
         TheSumGrid.PutCell(ci.iT_dec20, 0, mixer_rec.Sum_Dec20);
         TheSumGrid.PutCell(ci.iT_dec21, 0, mixer_rec.Sum_Dec21);
         TheSumGrid.PutCell(ci.iT_dec22, 0, mixer_rec.Sum_Dec22);
         TheSumGrid.PutCell(ci.iT_dec23, 0, mixer_rec.Sum_Dec23);
         TheSumGrid.PutCell(ci.iT_dec24, 0, mixer_rec.Sum_Dec24);
         TheSumGrid.PutCell(ci.iT_dec25, 0, mixer_rec.Sum_Dec25);
         TheSumGrid.PutCell(ci.iT_dec26, 0, mixer_rec.Sum_Dec26);
         TheSumGrid.PutCell(ci.iT_dec27, 0, mixer_rec.Sum_Dec27);
         TheSumGrid.PutCell(ci.iT_dec28, 0, mixer_rec.Sum_Dec28);
         TheSumGrid.PutCell(ci.iT_dec29, 0, mixer_rec.Sum_Dec29);
         TheSumGrid.PutCell(ci.iT_dec30, 0, mixer_rec.Sum_Dec30);
         TheSumGrid.PutCell(ci.iT_dec31, 0, mixer_rec.Sum_Dec31);
      }

   }

   protected virtual void PutDgvFields2()
   {
      int rowIdx, idxCorrector;

      TheG2.RowsRemoved -= new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);
      TheG2.RowsAdded -= new DataGridViewRowsAddedEventHandler(grid_RowsAdded);
      TheG2.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(TheG2);

      if(mixer_rec.Transes2 != null)
      {

         foreach(Xtrano xtrano_rec in mixer_rec.Transes2)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheG2.Rows.Add();

            rowIdx = TheG2.RowCount - idxCorrector;

            PutDgvLineFields2(xtrano_rec, rowIdx, false);

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
      Xtrano xtrano_rec = (Xtrano)trans_rec;

      if(skipRecID_andSerial_Columns == false)
      {
         TheG2.PutCell(ci.iT_recID, rowIdx, xtrano_rec.T_recID);
         TheG2.PutCell(ci.iT_serial, rowIdx, xtrano_rec.T_serial);
      }

      TheG2.PutCell(ci2.iT_opis_128, rowIdx, xtrano_rec.T_opis_128);
      TheG2.PutCell(ci2.iT_moneyA, rowIdx, xtrano_rec.T_moneyA);
      TheG2.PutCell(ci2.iT_konto, rowIdx, xtrano_rec.T_konto);
      TheG2.PutCell(ci2.iT_devName, rowIdx, xtrano_rec.T_devName);

   }

   public override void PutDgvTransSumFields2()
   {
      //  TheSumGrid2[ci2.iT_moneyA, 0].Value = mixer_rec.Sum2_MoneyA;
   }

   protected virtual void PutDgvFields3() { }

   public override void PutDgvLineFields3(VvTransRecord trans_rec, int rowIdx, bool skipRecID_andSerial_Columns) { }

   public override void PutDgvTransSumFields3() { }

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
            GetDgvLineFields(TheG.CurrentRow.Index, dirtyFlagging, null);

            ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode = false;
         }

         return;
      }

      //15.11.2021: bikoz of KOP DUC koji u 'zutome' ima redak manje nego svi ostali: 
      int numOfUsefulRows = (this is KOP_PTG_DUC ? TheG.RowCount : TheG.RowCount - 1);

    //if(TheG.RowCount > 0) recIDtable = new uint[TheG.RowCount - 1];
      if(TheG.RowCount > 0) recIDtable = new uint[numOfUsefulRows  ];
      else                  recIDtable = null;

      mixer_rec.DiscardPreviouslyAddedTranses();

    //for(rIdx = 0; rIdx < TheG.RowCount - 1; ++rIdx)
      for(rIdx = 0; rIdx < numOfUsefulRows  ; ++rIdx)
      {
         GetDgvLineFields1(rIdx, dirtyFlagging, recIDtable);
      }

      MarkTransesToDelete(recIDtable);

   }

   protected virtual void GetDgvFields2(bool dirtyFlagging)
   {
      uint[] recIDtable;
      int rIdx;

      if(dirtyFlagging && ThePolyGridTabControl.SelectedTab.Title != TabPageTitle2) return;

      if(dirtyFlagging == true && ZXC.TheVvForm.VvFlag_RowsAreAddingOrDeletingOrBoth == false)
      {
         if((TheG2.CurrentCell != null && TheG2.CurrentCell.IsInEditMode) || ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode)
         {
            GetDgvLineFields2(TheG2.CurrentRow.Index, dirtyFlagging, null);

            ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode = false;
         }

         return;
      }

      if(TheG2.RowCount > 0) recIDtable = new uint[TheG2.RowCount - 1];
      else recIDtable = null;

      mixer_rec.DiscardPreviouslyAddedTranses2();

      for(rIdx = 0; rIdx < TheG2.RowCount - 1; ++rIdx)
      {
         GetDgvLineFields2(rIdx, dirtyFlagging, recIDtable);
      }

      MarkTranses2ToDelete(recIDtable);

   }

   protected virtual void GetDgvFields3(bool dirtyFlagging) { }

   public override VvTransRecord GetDgvLineFields1(int rIdx, bool dirtyFlagging, uint[] recIDtable)
   {
      if(this is KOP_PTG_DUC) return GetDgvLineFields1_KOP_PTG_DUC(rIdx, dirtyFlagging, recIDtable);

      uint recID;
      bool DB_RWT;
      Xtrans db_rec;

      // dgvAtrans_rec: buffer za GetField iz DataGridWievRow-a                                        
      // db_rec : pointer na existing row u DB-u kojega ako je EditedHasChanges treba RWTREC-ati 

      recID = TheG.GetUint32Cell(ci.iT_recID, rIdx, dirtyFlagging);

      if(recIDtable != null) recIDtable[rIdx] = recID;

      dgvXtrans_rec = new Xtrans();

      if(recID > 0) // Postojeci redak 
      {
         db_rec = mixer_rec.Transes.SingleOrDefault(vvDR => vvDR.VirtualRecID == recID);
      }
      else // novododani redak 
      {
         db_rec = null;
      }

      DB_RWT = (db_rec != null);

      dgvXtrans_rec.T_recID = recID;

      dgvXtrans_rec.T_parentID = mixer_rec.RecID;

      #region GetColumns

      dgvXtrans_rec.T_serial = (ushort)(rIdx + 1);
      if(DB_RWT) db_rec.T_serial = dgvXtrans_rec.T_serial;

      dgvXtrans_rec.T_dokNum = mixer_rec.DokNum;
      if(DB_RWT) db_rec.T_dokNum = dgvXtrans_rec.T_dokNum;

      dgvXtrans_rec.T_dokDate = mixer_rec.DokDate;
      if(DB_RWT) db_rec.T_dokDate = dgvXtrans_rec.T_dokDate;

      dgvXtrans_rec.T_TT = mixer_rec.TT;
      if(DB_RWT) db_rec.T_TT = dgvXtrans_rec.T_TT;

      dgvXtrans_rec.T_ttNum = mixer_rec.TtNum;
      if(DB_RWT) db_rec.T_ttNum = dgvXtrans_rec.T_ttNum;

      if(this is RvrMjesecDUC) // MMYYYY
      {
         // PAZI !!! ako je npr T_konto visible kolona onda ce dolje biti pregazena u klasicnom 'if(TheG.CI_OK(ci.iT_konto))'... 

                                     dgvXtrans_rec.T_konto = mixer_rec.StrD_32; // t_MMYYYY 
         if(DB_RWT) db_rec.T_konto = dgvXtrans_rec.T_konto;                     // t_MMYYYY 

                                     dgvXtrans_rec.T_isXxx = mixer_rec.IsXxx; // t_isTrgovac 
         if(DB_RWT) db_rec.T_isXxx = dgvXtrans_rec.T_isXxx;                   // t_isTrgovac 

         //                             dgvXtrans_rec.T_moneyC = mixer_rec.MoneyA; // sluzbeniMjesecniFondRadniSati 
         //if(DB_RWT) db_rec.T_moneyC = dgvXtrans_rec.T_moneyC;                    // sluzbeniMjesecniFondRadniSati 
      }

      if(TheG.CI_OK(ci.iT_kol))
      {
         dgvXtrans_rec.T_kol = TheG.GetDecimalCell(ci.iT_kol, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_kol = dgvXtrans_rec.T_kol;
      }
      if(TheG.CI_OK(ci.iT_opis_128))
      {
         dgvXtrans_rec.T_opis_128 = TheG.GetStringCell(ci.iT_opis_128, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_opis_128 = dgvXtrans_rec.T_opis_128;
      }
      if(TheG.CI_OK(ci.iT_kpdbNameA_50))
      {
         dgvXtrans_rec.T_kpdbNameA_50 = TheG.GetStringCell(ci.iT_kpdbNameA_50, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_kpdbNameA_50 = dgvXtrans_rec.T_kpdbNameA_50;
      }
      if(TheG.CI_OK(ci.iT_kpdbUlBrA_32))
      {
         dgvXtrans_rec.T_kpdbUlBrA_32 = TheG.GetStringCell(ci.iT_kpdbUlBrA_32, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_kpdbUlBrA_32 = dgvXtrans_rec.T_kpdbUlBrA_32;
      }
      if(TheG.CI_OK(ci.iT_kpdbMjestoA_32))
      {
         dgvXtrans_rec.T_kpdbMjestoA_32 = TheG.GetStringCell(ci.iT_kpdbMjestoA_32, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_kpdbMjestoA_32 = dgvXtrans_rec.T_kpdbMjestoA_32;
      }
      if(TheG.CI_OK(ci.iT_kpdbZiroA_32))
      {
         dgvXtrans_rec.T_kpdbZiroA_32 = TheG.GetStringCell(ci.iT_kpdbZiroA_32, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_kpdbZiroA_32 = dgvXtrans_rec.T_kpdbZiroA_32;
      }
      if(TheG.CI_OK(ci.iT_kpdbNameB_50))
      {
         dgvXtrans_rec.T_kpdbNameB_50 = TheG.GetStringCell(ci.iT_kpdbNameB_50, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_kpdbNameB_50 = dgvXtrans_rec.T_kpdbNameB_50;
      }
      if(TheG.CI_OK(ci.iT_kpdbUlBrB_32))
      {
         dgvXtrans_rec.T_kpdbUlBrB_32 = TheG.GetStringCell(ci.iT_kpdbUlBrB_32, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_kpdbUlBrB_32 = dgvXtrans_rec.T_kpdbUlBrB_32;
      }
      if(TheG.CI_OK(ci.iT_kpdbMjestoB_32))
      {
         dgvXtrans_rec.T_kpdbMjestoB_32 = TheG.GetStringCell(ci.iT_kpdbMjestoB_32, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_kpdbMjestoB_32 = dgvXtrans_rec.T_kpdbMjestoB_32;
      }
      if(TheG.CI_OK(ci.iT_kpdbZiroB_32))
      {
         dgvXtrans_rec.T_kpdbZiroB_32 = TheG.GetStringCell(ci.iT_kpdbZiroB_32, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_kpdbZiroB_32 = dgvXtrans_rec.T_kpdbZiroB_32;
      }
      if(TheG.CI_OK(ci.iT_dateOd))
      {
         dgvXtrans_rec.T_dateOd = TheG.GetDateCell(ci.iT_dateOd, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_dateOd = dgvXtrans_rec.T_dateOd;
      }
      if(TheG.CI_OK(ci.iT_dateDo))
      {
         dgvXtrans_rec.T_dateDo = TheG.GetDateCell(ci.iT_dateDo, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_dateDo = dgvXtrans_rec.T_dateDo;
      }
      if(TheG.CI_OK(ci.iT_strA_2))
      {
         dgvXtrans_rec.T_strA_2 = TheG.GetStringCell(ci.iT_strA_2, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_strA_2 = dgvXtrans_rec.T_strA_2;
      }
      if(TheG.CI_OK(ci.iT_strB_2))
      {
         dgvXtrans_rec.T_strB_2 = TheG.GetStringCell(ci.iT_strB_2, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_strB_2 = dgvXtrans_rec.T_strB_2;
      }
      if(TheG.CI_OK(ci.iT_vezniDokA_64))
      {
         dgvXtrans_rec.T_vezniDokA_64 = TheG.GetStringCell(ci.iT_vezniDokA_64, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_vezniDokA_64 = dgvXtrans_rec.T_vezniDokA_64;
      }
      if(TheG.CI_OK(ci.iT_vezniDokB_64))
      {
         dgvXtrans_rec.T_vezniDokB_64 = TheG.GetStringCell(ci.iT_vezniDokB_64, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_vezniDokB_64 = dgvXtrans_rec.T_vezniDokB_64;
      }
      if(TheG.CI_OK(ci.iT_strC_2))
      {
         dgvXtrans_rec.T_strC_2 = TheG.GetStringCell(ci.iT_strC_2, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_strC_2 = dgvXtrans_rec.T_strC_2;
      }

      if(TheG.CI_OK(ci.iT_kupdobCD))
      {
         dgvXtrans_rec.T_kupdobCD = TheG.GetUint32Cell(ci.iT_kupdobCD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_kupdobCD = dgvXtrans_rec.T_kupdobCD;
      }
      else // 16.02.2021: NAK treba T_kupdobCD sa zaglavlja
      {
         if(mixer_rec.TT == Mixer.TT_NAK)
         {
                dgvXtrans_rec.T_kupdobCD = mixer_rec.KupdobCD;
            if(DB_RWT) db_rec.T_kupdobCD = dgvXtrans_rec.T_kupdobCD;
         }
      }

      if(TheG.CI_OK(ci.iT_artiklCD))
      {
         dgvXtrans_rec.T_artiklCD = TheG.GetStringCell(ci.iT_artiklCD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_artiklCD = dgvXtrans_rec.T_artiklCD;
      }
      if(TheG.CI_OK(ci.iT_artiklName))
      {
         dgvXtrans_rec.T_artiklName = TheG.GetStringCell(ci.iT_artiklName, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_artiklName = dgvXtrans_rec.T_artiklName;
      }
      if(TheG.CI_OK(ci.iT_isXxx))
      {
         dgvXtrans_rec.T_isXxx = VvCheckBox.GetBool4String(TheG.GetStringCell(ci.iT_isXxx, rIdx, dirtyFlagging));
         if(DB_RWT) db_rec.T_isXxx = dgvXtrans_rec.T_isXxx;
      }
      if(TheG.CI_OK(ci.iT_konto))
      {
         dgvXtrans_rec.T_konto = TheG.GetStringCell(ci.iT_konto, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_konto = dgvXtrans_rec.T_konto;
      }
      if(TheG.CI_OK(ci.iT_intA))
      {
         dgvXtrans_rec.T_intA = TheG.GetIntCell(ci.iT_intA, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_intA = dgvXtrans_rec.T_intA;
      }
      if(TheG.CI_OK(ci.iT_intB))
      {
         dgvXtrans_rec.T_intB = TheG.GetIntCell(ci.iT_intB, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_intB = dgvXtrans_rec.T_intB;
      }
      if(TheG.CI_OK(ci.iT_personCD))
      {
         dgvXtrans_rec.T_personCD = TheG.GetUint32Cell(ci.iT_personCD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_personCD = dgvXtrans_rec.T_personCD;
      }
      if(TheG.CI_OK(ci.iT_moneyA))
      {
         // 17.06.2015: 
         bool shouldSkip = (this is Statistika_NPF_DUC && (/*dgvXtrans_rec.T_strA_2.IsEmpty() ||*/ dgvXtrans_rec.T_strA_2 == "S"));
         if(shouldSkip == false)
         {
                dgvXtrans_rec.T_moneyA = TheG.GetDecimalCell(ci.iT_moneyA, rIdx, dirtyFlagging);
            if(DB_RWT) db_rec.T_moneyA = dgvXtrans_rec.T_moneyA;
         }
      }
      if(TheG.CI_OK(ci.iT_moneyB))
      {
         // 17.06.2015: 
         bool shouldSkip = (this is Statistika_NPF_DUC && (dgvXtrans_rec.T_strA_2.IsEmpty() || dgvXtrans_rec.T_strA_2 == "S"));
         if(shouldSkip == false)
         {
                dgvXtrans_rec.T_moneyB = TheG.GetDecimalCell(ci.iT_moneyB, rIdx, dirtyFlagging);
            if(DB_RWT) db_rec.T_moneyB = dgvXtrans_rec.T_moneyB;
         }
      }
      if(TheG.CI_OK(ci.iT_moneyC))
      {
         dgvXtrans_rec.T_moneyC = TheG.GetDecimalCell(ci.iT_moneyC, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_moneyC = dgvXtrans_rec.T_moneyC;
      }
      if(TheG.CI_OK(ci.iT_moneyD))
      {
         dgvXtrans_rec.T_moneyD = TheG.GetDecimalCell(ci.iT_moneyD, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_moneyD = dgvXtrans_rec.T_moneyD;
      }

      #region str 01-31
      if(TheG.CI_OK(ci.iT_str01)) { dgvXtrans_rec.T_str01 = TheG.GetStringCell(ci.iT_str01, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str01 = dgvXtrans_rec.T_str01; }
      if(TheG.CI_OK(ci.iT_str02)) { dgvXtrans_rec.T_str02 = TheG.GetStringCell(ci.iT_str02, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str02 = dgvXtrans_rec.T_str02; }
      if(TheG.CI_OK(ci.iT_str03)) { dgvXtrans_rec.T_str03 = TheG.GetStringCell(ci.iT_str03, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str03 = dgvXtrans_rec.T_str03; }
      if(TheG.CI_OK(ci.iT_str04)) { dgvXtrans_rec.T_str04 = TheG.GetStringCell(ci.iT_str04, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str04 = dgvXtrans_rec.T_str04; }
      if(TheG.CI_OK(ci.iT_str05)) { dgvXtrans_rec.T_str05 = TheG.GetStringCell(ci.iT_str05, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str05 = dgvXtrans_rec.T_str05; }
      if(TheG.CI_OK(ci.iT_str06)) { dgvXtrans_rec.T_str06 = TheG.GetStringCell(ci.iT_str06, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str06 = dgvXtrans_rec.T_str06; }
      if(TheG.CI_OK(ci.iT_str07)) { dgvXtrans_rec.T_str07 = TheG.GetStringCell(ci.iT_str07, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str07 = dgvXtrans_rec.T_str07; }
      if(TheG.CI_OK(ci.iT_str08)) { dgvXtrans_rec.T_str08 = TheG.GetStringCell(ci.iT_str08, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str08 = dgvXtrans_rec.T_str08; }
      if(TheG.CI_OK(ci.iT_str09)) { dgvXtrans_rec.T_str09 = TheG.GetStringCell(ci.iT_str09, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str09 = dgvXtrans_rec.T_str09; }
      if(TheG.CI_OK(ci.iT_str10)) { dgvXtrans_rec.T_str10 = TheG.GetStringCell(ci.iT_str10, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str10 = dgvXtrans_rec.T_str10; }
      if(TheG.CI_OK(ci.iT_str11)) { dgvXtrans_rec.T_str11 = TheG.GetStringCell(ci.iT_str11, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str11 = dgvXtrans_rec.T_str11; }
      if(TheG.CI_OK(ci.iT_str12)) { dgvXtrans_rec.T_str12 = TheG.GetStringCell(ci.iT_str12, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str12 = dgvXtrans_rec.T_str12; }
      if(TheG.CI_OK(ci.iT_str13)) { dgvXtrans_rec.T_str13 = TheG.GetStringCell(ci.iT_str13, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str13 = dgvXtrans_rec.T_str13; }
      if(TheG.CI_OK(ci.iT_str14)) { dgvXtrans_rec.T_str14 = TheG.GetStringCell(ci.iT_str14, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str14 = dgvXtrans_rec.T_str14; }
      if(TheG.CI_OK(ci.iT_str15)) { dgvXtrans_rec.T_str15 = TheG.GetStringCell(ci.iT_str15, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str15 = dgvXtrans_rec.T_str15; }
      if(TheG.CI_OK(ci.iT_str16)) { dgvXtrans_rec.T_str16 = TheG.GetStringCell(ci.iT_str16, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str16 = dgvXtrans_rec.T_str16; }
      if(TheG.CI_OK(ci.iT_str17)) { dgvXtrans_rec.T_str17 = TheG.GetStringCell(ci.iT_str17, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str17 = dgvXtrans_rec.T_str17; }
      if(TheG.CI_OK(ci.iT_str18)) { dgvXtrans_rec.T_str18 = TheG.GetStringCell(ci.iT_str18, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str18 = dgvXtrans_rec.T_str18; }
      if(TheG.CI_OK(ci.iT_str19)) { dgvXtrans_rec.T_str19 = TheG.GetStringCell(ci.iT_str19, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str19 = dgvXtrans_rec.T_str19; }
      if(TheG.CI_OK(ci.iT_str20)) { dgvXtrans_rec.T_str20 = TheG.GetStringCell(ci.iT_str20, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str20 = dgvXtrans_rec.T_str20; }
      if(TheG.CI_OK(ci.iT_str21)) { dgvXtrans_rec.T_str21 = TheG.GetStringCell(ci.iT_str21, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str21 = dgvXtrans_rec.T_str21; }
      if(TheG.CI_OK(ci.iT_str22)) { dgvXtrans_rec.T_str22 = TheG.GetStringCell(ci.iT_str22, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str22 = dgvXtrans_rec.T_str22; }
      if(TheG.CI_OK(ci.iT_str23)) { dgvXtrans_rec.T_str23 = TheG.GetStringCell(ci.iT_str23, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str23 = dgvXtrans_rec.T_str23; }
      if(TheG.CI_OK(ci.iT_str24)) { dgvXtrans_rec.T_str24 = TheG.GetStringCell(ci.iT_str24, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str24 = dgvXtrans_rec.T_str24; }
      if(TheG.CI_OK(ci.iT_str25)) { dgvXtrans_rec.T_str25 = TheG.GetStringCell(ci.iT_str25, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str25 = dgvXtrans_rec.T_str25; }
      if(TheG.CI_OK(ci.iT_str26)) { dgvXtrans_rec.T_str26 = TheG.GetStringCell(ci.iT_str26, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str26 = dgvXtrans_rec.T_str26; }
      if(TheG.CI_OK(ci.iT_str27)) { dgvXtrans_rec.T_str27 = TheG.GetStringCell(ci.iT_str27, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str27 = dgvXtrans_rec.T_str27; }
      if(TheG.CI_OK(ci.iT_str28)) { dgvXtrans_rec.T_str28 = TheG.GetStringCell(ci.iT_str28, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str28 = dgvXtrans_rec.T_str28; }
      if(TheG.CI_OK(ci.iT_str29)) { dgvXtrans_rec.T_str29 = TheG.GetStringCell(ci.iT_str29, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str29 = dgvXtrans_rec.T_str29; }
      if(TheG.CI_OK(ci.iT_str30)) { dgvXtrans_rec.T_str30 = TheG.GetStringCell(ci.iT_str30, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str30 = dgvXtrans_rec.T_str30; }
      if(TheG.CI_OK(ci.iT_str31)) { dgvXtrans_rec.T_str31 = TheG.GetStringCell(ci.iT_str31, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_str31 = dgvXtrans_rec.T_str31; }
      #endregion str 01-31

      if(TheG.CI_OK(ci.iT_date3))
      {
            dgvXtrans_rec.T_date3 = TheG.GetDateCell(ci.iT_date3, rIdx, dirtyFlagging);
        if(DB_RWT) db_rec.T_date3 = dgvXtrans_rec.T_date3;
      }
      if(TheG.CI_OK(ci.iT_date4))
      {
            dgvXtrans_rec.T_date4 = TheG.GetDateCell(ci.iT_date4, rIdx, dirtyFlagging);
        if(DB_RWT) db_rec.T_date4 = dgvXtrans_rec.T_date4;
      }


      // 28.11.2017: novo tretiranje decimala za AVR (AVR UC oce DateTime iz decimala) 
      if(this is AvrDUC == false) // tihis is NOT AVR DUC 
      {
#region NOT AVR DUC
         if(TheG.CI_OK(ci.iT_dec01))   { dgvXtrans_rec.T_dec01   = TheG.GetDecimalCell(ci.iT_dec01  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec01   = dgvXtrans_rec.T_dec01  ; }
         if(TheG.CI_OK(ci.iT_dec02))   { dgvXtrans_rec.T_dec02   = TheG.GetDecimalCell(ci.iT_dec02  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec02   = dgvXtrans_rec.T_dec02  ; }
         if(TheG.CI_OK(ci.iT_dec03))   { dgvXtrans_rec.T_dec03   = TheG.GetDecimalCell(ci.iT_dec03  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec03   = dgvXtrans_rec.T_dec03  ; }
         if(TheG.CI_OK(ci.iT_dec04))   { dgvXtrans_rec.T_dec04   = TheG.GetDecimalCell(ci.iT_dec04  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec04   = dgvXtrans_rec.T_dec04  ; }
         if(TheG.CI_OK(ci.iT_dec05))   { dgvXtrans_rec.T_dec05   = TheG.GetDecimalCell(ci.iT_dec05  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec05   = dgvXtrans_rec.T_dec05  ; }
         if(TheG.CI_OK(ci.iT_dec06))   { dgvXtrans_rec.T_dec06   = TheG.GetDecimalCell(ci.iT_dec06  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec06   = dgvXtrans_rec.T_dec06  ; }
         if(TheG.CI_OK(ci.iT_dec07))   { dgvXtrans_rec.T_dec07   = TheG.GetDecimalCell(ci.iT_dec07  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec07   = dgvXtrans_rec.T_dec07  ; }
         if(TheG.CI_OK(ci.iT_dec08))   { dgvXtrans_rec.T_dec08   = TheG.GetDecimalCell(ci.iT_dec08  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec08   = dgvXtrans_rec.T_dec08  ; }
         if(TheG.CI_OK(ci.iT_dec09))   { dgvXtrans_rec.T_dec09   = TheG.GetDecimalCell(ci.iT_dec09  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec09   = dgvXtrans_rec.T_dec09  ; }
         if(TheG.CI_OK(ci.iT_dec10))   { dgvXtrans_rec.T_dec10   = TheG.GetDecimalCell(ci.iT_dec10  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec10   = dgvXtrans_rec.T_dec10  ; }
         if(TheG.CI_OK(ci.iT_dec11))   { dgvXtrans_rec.T_dec11   = TheG.GetDecimalCell(ci.iT_dec11  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec11   = dgvXtrans_rec.T_dec11  ; }
         if(TheG.CI_OK(ci.iT_dec12))   { dgvXtrans_rec.T_dec12   = TheG.GetDecimalCell(ci.iT_dec12  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec12   = dgvXtrans_rec.T_dec12  ; }
         if(TheG.CI_OK(ci.iT_dec13))   { dgvXtrans_rec.T_dec13   = TheG.GetDecimalCell(ci.iT_dec13  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec13   = dgvXtrans_rec.T_dec13  ; }
         if(TheG.CI_OK(ci.iT_dec14))   { dgvXtrans_rec.T_dec14   = TheG.GetDecimalCell(ci.iT_dec14  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec14   = dgvXtrans_rec.T_dec14  ; }
         if(TheG.CI_OK(ci.iT_dec15))   { dgvXtrans_rec.T_dec15   = TheG.GetDecimalCell(ci.iT_dec15  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec15   = dgvXtrans_rec.T_dec15  ; }
         if(TheG.CI_OK(ci.iT_dec16))   { dgvXtrans_rec.T_dec16   = TheG.GetDecimalCell(ci.iT_dec16  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec16   = dgvXtrans_rec.T_dec16  ; }
         if(TheG.CI_OK(ci.iT_dec17))   { dgvXtrans_rec.T_dec17   = TheG.GetDecimalCell(ci.iT_dec17  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec17   = dgvXtrans_rec.T_dec17  ; }
         if(TheG.CI_OK(ci.iT_dec18))   { dgvXtrans_rec.T_dec18   = TheG.GetDecimalCell(ci.iT_dec18  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec18   = dgvXtrans_rec.T_dec18  ; }
         if(TheG.CI_OK(ci.iT_dec19))   { dgvXtrans_rec.T_dec19   = TheG.GetDecimalCell(ci.iT_dec19  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec19   = dgvXtrans_rec.T_dec19  ; }
         if(TheG.CI_OK(ci.iT_dec20))   { dgvXtrans_rec.T_dec20   = TheG.GetDecimalCell(ci.iT_dec20  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec20   = dgvXtrans_rec.T_dec20  ; }
         if(TheG.CI_OK(ci.iT_dec21))   { dgvXtrans_rec.T_dec21   = TheG.GetDecimalCell(ci.iT_dec21  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec21   = dgvXtrans_rec.T_dec21  ; }
         if(TheG.CI_OK(ci.iT_dec22))   { dgvXtrans_rec.T_dec22   = TheG.GetDecimalCell(ci.iT_dec22  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec22   = dgvXtrans_rec.T_dec22  ; }
         if(TheG.CI_OK(ci.iT_dec23))   { dgvXtrans_rec.T_dec23   = TheG.GetDecimalCell(ci.iT_dec23  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec23   = dgvXtrans_rec.T_dec23  ; }
         if(TheG.CI_OK(ci.iT_dec24))   { dgvXtrans_rec.T_dec24   = TheG.GetDecimalCell(ci.iT_dec24  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec24   = dgvXtrans_rec.T_dec24  ; }
         if(TheG.CI_OK(ci.iT_dec25))   { dgvXtrans_rec.T_dec25   = TheG.GetDecimalCell(ci.iT_dec25  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec25   = dgvXtrans_rec.T_dec25  ; }
         if(TheG.CI_OK(ci.iT_dec26))   { dgvXtrans_rec.T_dec26   = TheG.GetDecimalCell(ci.iT_dec26  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec26   = dgvXtrans_rec.T_dec26  ; }
         if(TheG.CI_OK(ci.iT_dec27))   { dgvXtrans_rec.T_dec27   = TheG.GetDecimalCell(ci.iT_dec27  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec27   = dgvXtrans_rec.T_dec27  ; }
         if(TheG.CI_OK(ci.iT_dec28))   { dgvXtrans_rec.T_dec28   = TheG.GetDecimalCell(ci.iT_dec28  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec28   = dgvXtrans_rec.T_dec28  ; }
         if(TheG.CI_OK(ci.iT_dec29))   { dgvXtrans_rec.T_dec29   = TheG.GetDecimalCell(ci.iT_dec29  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec29   = dgvXtrans_rec.T_dec29  ; }
         if(TheG.CI_OK(ci.iT_dec30))   { dgvXtrans_rec.T_dec30   = TheG.GetDecimalCell(ci.iT_dec30  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec30   = dgvXtrans_rec.T_dec30  ; }
         if(TheG.CI_OK(ci.iT_dec31))   { dgvXtrans_rec.T_dec31   = TheG.GetDecimalCell(ci.iT_dec31  , rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec31   = dgvXtrans_rec.T_dec31  ; }
         if(TheG.CI_OK(ci.iT_dec01_2)) { dgvXtrans_rec.T_dec01_2 = TheG.GetDecimalCell(ci.iT_dec01_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec01_2 = dgvXtrans_rec.T_dec01_2; }
         if(TheG.CI_OK(ci.iT_dec02_2)) { dgvXtrans_rec.T_dec02_2 = TheG.GetDecimalCell(ci.iT_dec02_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec02_2 = dgvXtrans_rec.T_dec02_2; }
         if(TheG.CI_OK(ci.iT_dec03_2)) { dgvXtrans_rec.T_dec03_2 = TheG.GetDecimalCell(ci.iT_dec03_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec03_2 = dgvXtrans_rec.T_dec03_2; }
         if(TheG.CI_OK(ci.iT_dec04_2)) { dgvXtrans_rec.T_dec04_2 = TheG.GetDecimalCell(ci.iT_dec04_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec04_2 = dgvXtrans_rec.T_dec04_2; }
         if(TheG.CI_OK(ci.iT_dec05_2)) { dgvXtrans_rec.T_dec05_2 = TheG.GetDecimalCell(ci.iT_dec05_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec05_2 = dgvXtrans_rec.T_dec05_2; }
         if(TheG.CI_OK(ci.iT_dec06_2)) { dgvXtrans_rec.T_dec06_2 = TheG.GetDecimalCell(ci.iT_dec06_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec06_2 = dgvXtrans_rec.T_dec06_2; }
         if(TheG.CI_OK(ci.iT_dec07_2)) { dgvXtrans_rec.T_dec07_2 = TheG.GetDecimalCell(ci.iT_dec07_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec07_2 = dgvXtrans_rec.T_dec07_2; }
         if(TheG.CI_OK(ci.iT_dec08_2)) { dgvXtrans_rec.T_dec08_2 = TheG.GetDecimalCell(ci.iT_dec08_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec08_2 = dgvXtrans_rec.T_dec08_2; }
         if(TheG.CI_OK(ci.iT_dec09_2)) { dgvXtrans_rec.T_dec09_2 = TheG.GetDecimalCell(ci.iT_dec09_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec09_2 = dgvXtrans_rec.T_dec09_2; }
         if(TheG.CI_OK(ci.iT_dec10_2)) { dgvXtrans_rec.T_dec10_2 = TheG.GetDecimalCell(ci.iT_dec10_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec10_2 = dgvXtrans_rec.T_dec10_2; }
         if(TheG.CI_OK(ci.iT_dec11_2)) { dgvXtrans_rec.T_dec11_2 = TheG.GetDecimalCell(ci.iT_dec11_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec11_2 = dgvXtrans_rec.T_dec11_2; }
         if(TheG.CI_OK(ci.iT_dec12_2)) { dgvXtrans_rec.T_dec12_2 = TheG.GetDecimalCell(ci.iT_dec12_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec12_2 = dgvXtrans_rec.T_dec12_2; }
         if(TheG.CI_OK(ci.iT_dec13_2)) { dgvXtrans_rec.T_dec13_2 = TheG.GetDecimalCell(ci.iT_dec13_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec13_2 = dgvXtrans_rec.T_dec13_2; }
         if(TheG.CI_OK(ci.iT_dec14_2)) { dgvXtrans_rec.T_dec14_2 = TheG.GetDecimalCell(ci.iT_dec14_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec14_2 = dgvXtrans_rec.T_dec14_2; }
         if(TheG.CI_OK(ci.iT_dec15_2)) { dgvXtrans_rec.T_dec15_2 = TheG.GetDecimalCell(ci.iT_dec15_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec15_2 = dgvXtrans_rec.T_dec15_2; }
         if(TheG.CI_OK(ci.iT_dec16_2)) { dgvXtrans_rec.T_dec16_2 = TheG.GetDecimalCell(ci.iT_dec16_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec16_2 = dgvXtrans_rec.T_dec16_2; }
         if(TheG.CI_OK(ci.iT_dec17_2)) { dgvXtrans_rec.T_dec17_2 = TheG.GetDecimalCell(ci.iT_dec17_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec17_2 = dgvXtrans_rec.T_dec17_2; }
         if(TheG.CI_OK(ci.iT_dec18_2)) { dgvXtrans_rec.T_dec18_2 = TheG.GetDecimalCell(ci.iT_dec18_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec18_2 = dgvXtrans_rec.T_dec18_2; }
         if(TheG.CI_OK(ci.iT_dec19_2)) { dgvXtrans_rec.T_dec19_2 = TheG.GetDecimalCell(ci.iT_dec19_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec19_2 = dgvXtrans_rec.T_dec19_2; }
         if(TheG.CI_OK(ci.iT_dec20_2)) { dgvXtrans_rec.T_dec20_2 = TheG.GetDecimalCell(ci.iT_dec20_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec20_2 = dgvXtrans_rec.T_dec20_2; }
         if(TheG.CI_OK(ci.iT_dec21_2)) { dgvXtrans_rec.T_dec21_2 = TheG.GetDecimalCell(ci.iT_dec21_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec21_2 = dgvXtrans_rec.T_dec21_2; }
         if(TheG.CI_OK(ci.iT_dec22_2)) { dgvXtrans_rec.T_dec22_2 = TheG.GetDecimalCell(ci.iT_dec22_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec22_2 = dgvXtrans_rec.T_dec22_2; }
         if(TheG.CI_OK(ci.iT_dec23_2)) { dgvXtrans_rec.T_dec23_2 = TheG.GetDecimalCell(ci.iT_dec23_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec23_2 = dgvXtrans_rec.T_dec23_2; }
         if(TheG.CI_OK(ci.iT_dec24_2)) { dgvXtrans_rec.T_dec24_2 = TheG.GetDecimalCell(ci.iT_dec24_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec24_2 = dgvXtrans_rec.T_dec24_2; }
         if(TheG.CI_OK(ci.iT_dec25_2)) { dgvXtrans_rec.T_dec25_2 = TheG.GetDecimalCell(ci.iT_dec25_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec25_2 = dgvXtrans_rec.T_dec25_2; }
         if(TheG.CI_OK(ci.iT_dec26_2)) { dgvXtrans_rec.T_dec26_2 = TheG.GetDecimalCell(ci.iT_dec26_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec26_2 = dgvXtrans_rec.T_dec26_2; }
         if(TheG.CI_OK(ci.iT_dec27_2)) { dgvXtrans_rec.T_dec27_2 = TheG.GetDecimalCell(ci.iT_dec27_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec27_2 = dgvXtrans_rec.T_dec27_2; }
         if(TheG.CI_OK(ci.iT_dec28_2)) { dgvXtrans_rec.T_dec28_2 = TheG.GetDecimalCell(ci.iT_dec28_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec28_2 = dgvXtrans_rec.T_dec28_2; }
         if(TheG.CI_OK(ci.iT_dec29_2)) { dgvXtrans_rec.T_dec29_2 = TheG.GetDecimalCell(ci.iT_dec29_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec29_2 = dgvXtrans_rec.T_dec29_2; }
         if(TheG.CI_OK(ci.iT_dec30_2)) { dgvXtrans_rec.T_dec30_2 = TheG.GetDecimalCell(ci.iT_dec30_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec30_2 = dgvXtrans_rec.T_dec30_2; }
         if(TheG.CI_OK(ci.iT_dec31_2)) { dgvXtrans_rec.T_dec31_2 = TheG.GetDecimalCell(ci.iT_dec31_2, rIdx, dirtyFlagging); if(DB_RWT) db_rec.T_dec31_2 = dgvXtrans_rec.T_dec31_2; }

#endregion NOT AVR DUC
      }
      else // tihis IS AVR DUC 
      { 
#region AVR DUC
         if(TheG.CI_OK(ci.iT_dec01))   { dgvXtrans_rec.T_dec01   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec01  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec01   = dgvXtrans_rec.T_dec01  ; }
         if(TheG.CI_OK(ci.iT_dec02))   { dgvXtrans_rec.T_dec02   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec02  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec02   = dgvXtrans_rec.T_dec02  ; }
         if(TheG.CI_OK(ci.iT_dec03))   { dgvXtrans_rec.T_dec03   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec03  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec03   = dgvXtrans_rec.T_dec03  ; }
         if(TheG.CI_OK(ci.iT_dec04))   { dgvXtrans_rec.T_dec04   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec04  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec04   = dgvXtrans_rec.T_dec04  ; }
         if(TheG.CI_OK(ci.iT_dec05))   { dgvXtrans_rec.T_dec05   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec05  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec05   = dgvXtrans_rec.T_dec05  ; }
         if(TheG.CI_OK(ci.iT_dec06))   { dgvXtrans_rec.T_dec06   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec06  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec06   = dgvXtrans_rec.T_dec06  ; }
         if(TheG.CI_OK(ci.iT_dec07))   { dgvXtrans_rec.T_dec07   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec07  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec07   = dgvXtrans_rec.T_dec07  ; }
         if(TheG.CI_OK(ci.iT_dec08))   { dgvXtrans_rec.T_dec08   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec08  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec08   = dgvXtrans_rec.T_dec08  ; }
         if(TheG.CI_OK(ci.iT_dec09))   { dgvXtrans_rec.T_dec09   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec09  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec09   = dgvXtrans_rec.T_dec09  ; }
         if(TheG.CI_OK(ci.iT_dec10))   { dgvXtrans_rec.T_dec10   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec10  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec10   = dgvXtrans_rec.T_dec10  ; }
         if(TheG.CI_OK(ci.iT_dec11))   { dgvXtrans_rec.T_dec11   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec11  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec11   = dgvXtrans_rec.T_dec11  ; }
         if(TheG.CI_OK(ci.iT_dec12))   { dgvXtrans_rec.T_dec12   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec12  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec12   = dgvXtrans_rec.T_dec12  ; }
         if(TheG.CI_OK(ci.iT_dec13))   { dgvXtrans_rec.T_dec13   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec13  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec13   = dgvXtrans_rec.T_dec13  ; }
         if(TheG.CI_OK(ci.iT_dec14))   { dgvXtrans_rec.T_dec14   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec14  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec14   = dgvXtrans_rec.T_dec14  ; }
         if(TheG.CI_OK(ci.iT_dec15))   { dgvXtrans_rec.T_dec15   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec15  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec15   = dgvXtrans_rec.T_dec15  ; }
         if(TheG.CI_OK(ci.iT_dec16))   { dgvXtrans_rec.T_dec16   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec16  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec16   = dgvXtrans_rec.T_dec16  ; }
         if(TheG.CI_OK(ci.iT_dec17))   { dgvXtrans_rec.T_dec17   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec17  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec17   = dgvXtrans_rec.T_dec17  ; }
         if(TheG.CI_OK(ci.iT_dec18))   { dgvXtrans_rec.T_dec18   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec18  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec18   = dgvXtrans_rec.T_dec18  ; }
         if(TheG.CI_OK(ci.iT_dec19))   { dgvXtrans_rec.T_dec19   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec19  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec19   = dgvXtrans_rec.T_dec19  ; }
         if(TheG.CI_OK(ci.iT_dec20))   { dgvXtrans_rec.T_dec20   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec20  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec20   = dgvXtrans_rec.T_dec20  ; }
         if(TheG.CI_OK(ci.iT_dec21))   { dgvXtrans_rec.T_dec21   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec21  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec21   = dgvXtrans_rec.T_dec21  ; }
         if(TheG.CI_OK(ci.iT_dec22))   { dgvXtrans_rec.T_dec22   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec22  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec22   = dgvXtrans_rec.T_dec22  ; }
         if(TheG.CI_OK(ci.iT_dec23))   { dgvXtrans_rec.T_dec23   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec23  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec23   = dgvXtrans_rec.T_dec23  ; }
         if(TheG.CI_OK(ci.iT_dec24))   { dgvXtrans_rec.T_dec24   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec24  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec24   = dgvXtrans_rec.T_dec24  ; }
         if(TheG.CI_OK(ci.iT_dec25))   { dgvXtrans_rec.T_dec25   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec25  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec25   = dgvXtrans_rec.T_dec25  ; }
         if(TheG.CI_OK(ci.iT_dec26))   { dgvXtrans_rec.T_dec26   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec26  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec26   = dgvXtrans_rec.T_dec26  ; }
         if(TheG.CI_OK(ci.iT_dec27))   { dgvXtrans_rec.T_dec27   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec27  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec27   = dgvXtrans_rec.T_dec27  ; }
         if(TheG.CI_OK(ci.iT_dec28))   { dgvXtrans_rec.T_dec28   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec28  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec28   = dgvXtrans_rec.T_dec28  ; }
         if(TheG.CI_OK(ci.iT_dec29))   { dgvXtrans_rec.T_dec29   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec29  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec29   = dgvXtrans_rec.T_dec29  ; }
         if(TheG.CI_OK(ci.iT_dec30))   { dgvXtrans_rec.T_dec30   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec30  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec30   = dgvXtrans_rec.T_dec30  ; }
         if(TheG.CI_OK(ci.iT_dec31))   { dgvXtrans_rec.T_dec31   = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec31  , rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec31   = dgvXtrans_rec.T_dec31  ; }
         if(TheG.CI_OK(ci.iT_dec01_2)) { dgvXtrans_rec.T_dec01_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec01_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec01_2 = dgvXtrans_rec.T_dec01_2; }
         if(TheG.CI_OK(ci.iT_dec02_2)) { dgvXtrans_rec.T_dec02_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec02_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec02_2 = dgvXtrans_rec.T_dec02_2; }
         if(TheG.CI_OK(ci.iT_dec03_2)) { dgvXtrans_rec.T_dec03_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec03_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec03_2 = dgvXtrans_rec.T_dec03_2; }
         if(TheG.CI_OK(ci.iT_dec04_2)) { dgvXtrans_rec.T_dec04_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec04_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec04_2 = dgvXtrans_rec.T_dec04_2; }
         if(TheG.CI_OK(ci.iT_dec05_2)) { dgvXtrans_rec.T_dec05_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec05_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec05_2 = dgvXtrans_rec.T_dec05_2; }
         if(TheG.CI_OK(ci.iT_dec06_2)) { dgvXtrans_rec.T_dec06_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec06_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec06_2 = dgvXtrans_rec.T_dec06_2; }
         if(TheG.CI_OK(ci.iT_dec07_2)) { dgvXtrans_rec.T_dec07_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec07_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec07_2 = dgvXtrans_rec.T_dec07_2; }
         if(TheG.CI_OK(ci.iT_dec08_2)) { dgvXtrans_rec.T_dec08_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec08_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec08_2 = dgvXtrans_rec.T_dec08_2; }
         if(TheG.CI_OK(ci.iT_dec09_2)) { dgvXtrans_rec.T_dec09_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec09_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec09_2 = dgvXtrans_rec.T_dec09_2; }
         if(TheG.CI_OK(ci.iT_dec10_2)) { dgvXtrans_rec.T_dec10_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec10_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec10_2 = dgvXtrans_rec.T_dec10_2; }
         if(TheG.CI_OK(ci.iT_dec11_2)) { dgvXtrans_rec.T_dec11_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec11_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec11_2 = dgvXtrans_rec.T_dec11_2; }
         if(TheG.CI_OK(ci.iT_dec12_2)) { dgvXtrans_rec.T_dec12_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec12_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec12_2 = dgvXtrans_rec.T_dec12_2; }
         if(TheG.CI_OK(ci.iT_dec13_2)) { dgvXtrans_rec.T_dec13_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec13_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec13_2 = dgvXtrans_rec.T_dec13_2; }
         if(TheG.CI_OK(ci.iT_dec14_2)) { dgvXtrans_rec.T_dec14_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec14_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec14_2 = dgvXtrans_rec.T_dec14_2; }
         if(TheG.CI_OK(ci.iT_dec15_2)) { dgvXtrans_rec.T_dec15_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec15_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec15_2 = dgvXtrans_rec.T_dec15_2; }
         if(TheG.CI_OK(ci.iT_dec16_2)) { dgvXtrans_rec.T_dec16_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec16_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec16_2 = dgvXtrans_rec.T_dec16_2; }
         if(TheG.CI_OK(ci.iT_dec17_2)) { dgvXtrans_rec.T_dec17_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec17_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec17_2 = dgvXtrans_rec.T_dec17_2; }
         if(TheG.CI_OK(ci.iT_dec18_2)) { dgvXtrans_rec.T_dec18_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec18_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec18_2 = dgvXtrans_rec.T_dec18_2; }
         if(TheG.CI_OK(ci.iT_dec19_2)) { dgvXtrans_rec.T_dec19_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec19_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec19_2 = dgvXtrans_rec.T_dec19_2; }
         if(TheG.CI_OK(ci.iT_dec20_2)) { dgvXtrans_rec.T_dec20_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec20_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec20_2 = dgvXtrans_rec.T_dec20_2; }
         if(TheG.CI_OK(ci.iT_dec21_2)) { dgvXtrans_rec.T_dec21_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec21_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec21_2 = dgvXtrans_rec.T_dec21_2; }
         if(TheG.CI_OK(ci.iT_dec22_2)) { dgvXtrans_rec.T_dec22_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec22_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec22_2 = dgvXtrans_rec.T_dec22_2; }
         if(TheG.CI_OK(ci.iT_dec23_2)) { dgvXtrans_rec.T_dec23_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec23_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec23_2 = dgvXtrans_rec.T_dec23_2; }
         if(TheG.CI_OK(ci.iT_dec24_2)) { dgvXtrans_rec.T_dec24_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec24_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec24_2 = dgvXtrans_rec.T_dec24_2; }
         if(TheG.CI_OK(ci.iT_dec25_2)) { dgvXtrans_rec.T_dec25_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec25_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec25_2 = dgvXtrans_rec.T_dec25_2; }
         if(TheG.CI_OK(ci.iT_dec26_2)) { dgvXtrans_rec.T_dec26_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec26_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec26_2 = dgvXtrans_rec.T_dec26_2; }
         if(TheG.CI_OK(ci.iT_dec27_2)) { dgvXtrans_rec.T_dec27_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec27_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec27_2 = dgvXtrans_rec.T_dec27_2; }
         if(TheG.CI_OK(ci.iT_dec28_2)) { dgvXtrans_rec.T_dec28_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec28_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec28_2 = dgvXtrans_rec.T_dec28_2; }
         if(TheG.CI_OK(ci.iT_dec29_2)) { dgvXtrans_rec.T_dec29_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec29_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec29_2 = dgvXtrans_rec.T_dec29_2; }
         if(TheG.CI_OK(ci.iT_dec30_2)) { dgvXtrans_rec.T_dec30_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec30_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec30_2 = dgvXtrans_rec.T_dec30_2; }
         if(TheG.CI_OK(ci.iT_dec31_2)) { dgvXtrans_rec.T_dec31_2 = ZXC.GetDecimalFromDateTime(TheG.GetDateCell(ci.iT_dec31_2, rIdx, dirtyFlagging)); if(DB_RWT) db_rec.T_dec31_2 = dgvXtrans_rec.T_dec31_2; }

#endregion AVR DUC

      
      }

      #endregion GetColumns

      if(dgvXtrans_rec.T_recID == 0) // ADDED NEW, FRESH Ftrans             
      {
         mixer_rec.InvokeTransRemove(dgvXtrans_rec);

         dgvXtrans_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;

         dgvXtrans_rec.CalcTransResults(mixer_rec);

         mixer_rec.Transes.Add(dgvXtrans_rec);
      }
      else if(db_rec.EditedHasChanges())
      {
         db_rec.SaveTransesWriteMode = ZXC.WriteMode.Edit;
      }
      else
      {
         db_rec.SaveTransesWriteMode = ZXC.WriteMode.None;
      }

      return dgvXtrans_rec;
   }

   private VvTransRecord GetDgvLineFields1_KOP_PTG_DUC(int rIdx, bool dirtyFlagging, uint[] recIDtable)
   {
      #region Classic

      uint recID;
      bool DB_RWT;
      Xtrans db_rec;

      // dgvAtrans_rec: buffer za GetField iz DataGridWievRow-a                                        
      // db_rec : pointer na existing row u DB-u kojega ako je EditedHasChanges treba RWTREC-ati 

      recID = TheG.GetUint32Cell(ci.iT_recID, rIdx, dirtyFlagging);

      if(recIDtable != null) recIDtable[rIdx] = recID;

      dgvXtrans_rec = new Xtrans();

      if(recID > 0) // Postojeci redak 
      {
         db_rec = mixer_rec.Transes.SingleOrDefault(vvDR => vvDR.VirtualRecID == recID);
      }
      else // novododani redak 
      {
         db_rec = null;
      }

      DB_RWT = (db_rec != null);

      dgvXtrans_rec.T_recID = recID;

      dgvXtrans_rec.T_parentID = mixer_rec.RecID;

      #region GetColumns

      dgvXtrans_rec.T_serial = (ushort)(rIdx + 1);
      if(DB_RWT) db_rec.T_serial = dgvXtrans_rec.T_serial;

      dgvXtrans_rec.T_dokNum = mixer_rec.DokNum;
      if(DB_RWT) db_rec.T_dokNum = dgvXtrans_rec.T_dokNum;

      dgvXtrans_rec.T_dokDate = mixer_rec.DokDate;
      if(DB_RWT) db_rec.T_dokDate = dgvXtrans_rec.T_dokDate;

      dgvXtrans_rec.T_TT = mixer_rec.TT;
      if(DB_RWT) db_rec.T_TT = dgvXtrans_rec.T_TT;

      dgvXtrans_rec.T_ttNum = mixer_rec.TtNum;
      if(DB_RWT) db_rec.T_ttNum = dgvXtrans_rec.T_ttNum;

      if(TheG.CI_OK(ci.iT_opis_128))
     {
         dgvXtrans_rec.T_opis_128 = TheG.GetStringCell(ci.iT_opis_128, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_opis_128 = dgvXtrans_rec.T_opis_128;
      }
      if(TheG.CI_OK(ci.iT_dateOd))
      {
         dgvXtrans_rec.T_dateOd = TheG.GetDateCell(ci.iT_dateOd, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_dateOd = dgvXtrans_rec.T_dateOd;
      }
      if(TheG.CI_OK(ci.iT_dateDo))
      {
         dgvXtrans_rec.T_dateDo = TheG.GetDateCell(ci.iT_dateDo, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_dateDo = dgvXtrans_rec.T_dateDo;
      }
      if(TheG.CI_OK(ci.iT_isXxx))
      {
         dgvXtrans_rec.T_isXxx = VvCheckBox.GetBool4String(TheG.GetStringCell(ci.iT_isXxx, rIdx, dirtyFlagging));
         if(DB_RWT) db_rec.T_isXxx = dgvXtrans_rec.T_isXxx;
      }
      if(TheG.CI_OK(ci.iT_intA))
      {
         dgvXtrans_rec.T_intA = TheG.GetIntCell(ci.iT_intA, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_intA = dgvXtrans_rec.T_intA;
      }
      if(TheG.CI_OK(ci.iT_moneyA))
      {
          dgvXtrans_rec.T_moneyA = TheG.GetDecimalCell(ci.iT_moneyA, rIdx, dirtyFlagging);
          if(DB_RWT) db_rec.T_moneyA = dgvXtrans_rec.T_moneyA;
      }

      //if(TheG.CI_OK(ci.iT_intB))
      //{
      //   dgvXtrans_rec.T_intB = VvCheckBox.GetBool4String(TheG.GetStringCell(ci.iT_intB, rIdx, dirtyFlagging)) == true ? 1 : 0;
      //   if(DB_RWT) db_rec.T_intB = dgvXtrans_rec.T_intB;
      //}

      if(TheG.CI_OK(ci.iT_intB))
      {
         dgvXtrans_rec.T_intB = TheG.GetBoolCell(ci.iT_intB, rIdx, dirtyFlagging) == true ? 1 : 0;
         if(DB_RWT) db_rec.T_intB = dgvXtrans_rec.T_intB;
      }
      if(TheG.CI_OK(ci.iT_kpdbNameA_50))
      {
         dgvXtrans_rec.T_kpdbNameA_50 = TheG.GetStringCell(ci.iT_kpdbNameA_50, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_kpdbNameA_50 = dgvXtrans_rec.T_kpdbNameA_50;
      }

      if(TheG.CI_OK(ci.iT_date3))
      {
         dgvXtrans_rec.T_date3 = TheG.GetDateCell(ci.iT_date3, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_date3 = dgvXtrans_rec.T_date3;
      }
      if(TheG.CI_OK(ci.iT_date4))
      {
         dgvXtrans_rec.T_date4 = TheG.GetDateCell(ci.iT_date4, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_date4 = dgvXtrans_rec.T_date4;
      }



      #endregion GetColumns

      #endregion Classic

      #region KOP Voila!

      KOP_PTG_DUC theDUC             = this as KOP_PTG_DUC               ;
      PTG_Ugovor       thePTG_Ugovor      = theDUC.ThePTG_Ugovor              ;
      PTG_OtplatniPlan theOtplatniPlan    = theDUC.TheOtplatniPlan            ;
      List<PTG_Rata>   UGAN_RateList      = theOtplatniPlan.UGAN_RateList     ;
      List<PTG_Rata>   UGANwoKOP_RateList = theOtplatniPlan.UGANwoKOP_RateList;

      PTG_Rata original_PTG_Rata = UGANwoKOP_RateList.Single(r => r.Serial  == dgvXtrans_rec.T_serial);
      PTG_Rata current_PTG_Rata  = UGAN_RateList     .Single(r => r.Serial  == dgvXtrans_rec.T_serial);
    //PTG_Rata original_PTG_Rata = UGANwoKOP_RateList.Single(r => r.RataRbr == dgvXtrans_rec.T_intA  );
    //PTG_Rata current_PTG_Rata  = UGAN_RateList     .Single(r => r.RataRbr == dgvXtrans_rec.T_intA  );

      bool xtransWasKOP = DB_RWT;

      if(xtransWasKOP == false) // NE, ovaj redak nije postojao u data layeru 
      {
         if(original_PTG_Rata.IsEqual(dgvXtrans_rec)) // NE, nepromjenjeno s obzirom na originalni result ratu 
         {
            dgvXtrans_rec.SaveTransesWriteMode = ZXC.WriteMode.None;
         }
         else // DA, promjenjeno je s obzirom na originalni otplatni plan 
         {
            mixer_rec.InvokeTransRemove(dgvXtrans_rec);

            dgvXtrans_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;

            dgvXtrans_rec.T_isXxx = true;

            dgvXtrans_rec.T_date4        = VvSQL.GetServer_DateTime_Now(TheDbConnection);
            dgvXtrans_rec.T_kpdbNameA_50 = ZXC.CURR_userName;

            dgvXtrans_rec.CalcTransResults(mixer_rec);

            mixer_rec.Transes.Add(dgvXtrans_rec);
         }

         if(dgvXtrans_rec.SaveTransesWriteMode == ZXC.WriteMode.None) 
         {
            if(mixer_rec.Transes.Contains(dgvXtrans_rec))
            {
               mixer_rec.InvokeTransRemove(dgvXtrans_rec); // brisi ga jer je u ovoj turi bio dobio 'Add' pa vracen na staro 'None' pa ga treba ukloniti i iz biznisa 
            }
         }

      } // if(xtransWasKOP == false) // NE, ovaj redak nije postojao u data layeru 

      else // DA, ovaj redak je otprije postojao u data layeru 
      {
         if(original_PTG_Rata.IsEqual(dgvXtrans_rec)) // NE, nepromjenjeno s obzirom na originalnu result ratu 
         {
          //dgvXtrans_rec.SaveTransesWriteMode = ZXC.WriteMode.Delete;
            db_rec       .SaveTransesWriteMode = ZXC.WriteMode.Delete;
         }
         else // DA, promjenjeno je s obzirom na originalnu result ratu 
         {
            if(current_PTG_Rata.IsEqual(dgvXtrans_rec)) // NE, nepromjenjeno s obzirom na current datalayer ratu 
            {
             //dgvXtrans_rec.SaveTransesWriteMode = ZXC.WriteMode.None;
               db_rec       .SaveTransesWriteMode = ZXC.WriteMode.None;
            }
            else // DA, promjenjeno s obzirom na current datalayer ratu 
            {
             //dgvXtrans_rec.SaveTransesWriteMode = ZXC.WriteMode.Edit;
               db_rec       .SaveTransesWriteMode = ZXC.WriteMode.Edit;

               db_rec.T_date4        = dgvXtrans_rec.T_date4        = VvSQL.GetServer_DateTime_Now(TheDbConnection);
               db_rec.T_kpdbNameA_50 = dgvXtrans_rec.T_kpdbNameA_50 = ZXC.CURR_userName;

            }
         }
      }

      #endregion KOP Voila!

      //if(dgvXtrans_rec.T_recID == 0) // ADDED NEW, FRESH Ftrans             
      //{
      //   mixer_rec.InvokeTransRemove(dgvXtrans_rec);
      //
      //   dgvXtrans_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;
      //
      //   dgvXtrans_rec.CalcTransResults(mixer_rec);
      //
      //   mixer_rec.Transes.Add(dgvXtrans_rec);
      //}
      //else if(db_rec.EditedHasChanges())
      //{
      //   db_rec.SaveTransesWriteMode = ZXC.WriteMode.Edit;
      //}
      //else
      //{
      //   db_rec.SaveTransesWriteMode = ZXC.WriteMode.None;
      //}

      return dgvXtrans_rec;
   }

   public override VvTransRecord GetDgvLineFields2(int rIdx, bool dirtyFlagging, uint[] recIDtable)
   {
      uint recID;
      bool DB_RWT;
      Xtrano db_rec;

      // dgvAtrans_rec: buffer za GetField iz DataGridWievRow-a                                        
      // db_rec : pointer na existing row u DB-u kojega ako je EditedHasChanges treba RWTREC-ati 

      recID = TheG2.GetUint32Cell(ci.iT_recID, rIdx, dirtyFlagging);

      if(recIDtable != null) recIDtable[rIdx] = recID;

      dgvXtrano_rec = new Xtrano();

      if(recID > 0) // Postojeci redak 
      {
         db_rec = mixer_rec.Transes2.SingleOrDefault(vvDR => vvDR.VirtualRecID == recID);
      }
      else // novododani redak 
      {
         db_rec = null;
      }

      DB_RWT = (db_rec != null);

      dgvXtrano_rec.T_recID = recID;

      dgvXtrano_rec.T_parentID = mixer_rec.RecID;

      #region GetColumns

      //TODO: mozda bug! ovdje Za prvih Para kolona koristis ci. a Ne ci2.

      dgvXtrano_rec.T_serial = (ushort)(rIdx + 1);
      if(DB_RWT) db_rec.T_serial = dgvXtrano_rec.T_serial;

      dgvXtrano_rec.T_dokNum = mixer_rec.DokNum;
      if(DB_RWT) db_rec.T_dokNum = dgvXtrano_rec.T_dokNum;

      dgvXtrano_rec.T_dokDate = mixer_rec.DokDate;
      if(DB_RWT) db_rec.T_dokDate = dgvXtrano_rec.T_dokDate;

      dgvXtrano_rec.T_TT = mixer_rec.TT;
      if(DB_RWT) db_rec.T_TT = dgvXtrano_rec.T_TT;

      dgvXtrano_rec.T_ttNum = mixer_rec.TtNum;
      if(DB_RWT) db_rec.T_ttNum = dgvXtrano_rec.T_ttNum;

      if(TheG2.CI_OK(ci2.iT_opis_128))
      {
         dgvXtrano_rec.T_opis_128 = TheG2.GetStringCell(ci2.iT_opis_128, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_opis_128 = dgvXtrano_rec.T_opis_128;
      }
      if(TheG2.CI_OK(ci2.iT_moneyA))
      {
         dgvXtrano_rec.T_moneyA = TheG2.GetDecimalCell(ci2.iT_moneyA, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_moneyA = dgvXtrano_rec.T_moneyA;
      }
      if(TheG2.CI_OK(ci2.iT_konto))
      {
         dgvXtrano_rec.T_konto = TheG2.GetStringCell(ci2.iT_konto, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_konto = dgvXtrano_rec.T_konto;
      }
      if(TheG2.CI_OK(ci2.iT_devName))
      {
         dgvXtrano_rec.T_devName = TheG2.GetStringCell(ci2.iT_devName, rIdx, dirtyFlagging);
         if(DB_RWT) db_rec.T_devName = dgvXtrano_rec.T_devName;
      }

      #endregion GetColumns

      if(dgvXtrano_rec.T_recID == 0) // ADDED NEW, FRESH Ftrans             
      {
         mixer_rec.InvokeTransRemove2(dgvXtrano_rec);

         dgvXtrano_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;

         mixer_rec.Transes2.Add(dgvXtrano_rec);
      }
      else if(db_rec.EditedHasChanges())
      {
         db_rec.SaveTransesWriteMode = ZXC.WriteMode.Edit;
      }
      else
      {
         db_rec.SaveTransesWriteMode = ZXC.WriteMode.None;
      }

      return dgvXtrano_rec;

   }

   public override VvTransRecord GetDgvLineFields3(int rIdx, bool dirtyFlagging, uint[] recIDtable) { return null; }

   #endregion PutDgvFields(), GetDgvFields()

   #region Overriders and specifics

   public virtual bool HasDscSubVariants { get { return false; } }

   public override ZXC.DbNavigationRestrictor VvNavRestrictor_TT { get { return dbNavigationRestrictor; } }

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.mixer_rec; }
      set { this.mixer_rec = (Mixer)value; }
   }

   public override VvDocumentRecord VirtualDocumentRecord
   {
      get { return this.VirtualDataRecord as VvDocumentRecord; }
      set { this.VirtualDataRecord = (Mixer)value; }
   }

   public override VvPolyDocumRecord VirtualPolyDocumRecord
   {
      get { return this.VirtualDocumentRecord as VvPolyDocumRecord; }
      set { this.VirtualDocumentRecord = (Mixer)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.MixerDao; }
   }

   public override VvDaoBase TheVvDaoTrans
   {
      get { return ZXC.XtransDao; }
   }

   public override VvDaoBase TheVvDaoTrans2
   {
      get { return ZXC.XtranoDao; }
   }

   public override VvDaoBase TheVvDaoTrans3
   {
      get { return ZXC.XtranoDao; }
   }

   #region Document_Database_ColumnIndexes, Transes_Database_ColumnIndexes

   /// <summary>
   /// Column Index U DataBase (NE na DataGridView-u to ti je 'ci')
   /// </summary>
   /*private*/
   protected MixerDao.MixerCI DB_ci
   {
      get { return ZXC.MixCI; }
   }

   /// <summary>
   /// Column Index u XTRANS DataBase (NE na DataGridView-u to ti je 'ci')
   /// </summary>
   protected XtransDao.XtransCI DB_Tci
   {
      get { return ZXC.XtrCI; }
   }

   /// <summary>
   /// Column Index u XTRANO DataBase (NE na DataGridView-u to ti je 'ci')
   /// </summary>
   private XtranoDao.XtranoCI DB_Tci2
   {
      get { return ZXC.XtoCI; }
   }

   #endregion Document_Database_ColumnIndexes, Transes_Database_ColumnIndexes

   #region VvFindDialog !!!!  Overrida se Tamo Gdje Treba FindDialog (npr. na virmanima netreba)

   // na virmanima netreba ali to neznaci da negdje drugdje nece biti
   public override VvFindDialog CreateVvFindDialog()
   {
      return CreateFind_Mixer_Dialog(TheSubModul);
   }

   public static VvFindDialog CreateFind_Mixer_Dialog(VvForm.VvSubModul theSubModul)
   {
      VvForm.VvSubModul vvSubModul = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsMIX);
      VvDataRecord vvDataRecord = ZXC.TheVvForm.CreateTheVvDataRecord_SwitchSubModulEnum(vvSubModul);

      VvFindDialog vvFindDialog = new VvFindDialog();

      VvRecLstUC vvRecListUC = new MixerListUC(vvFindDialog, (Mixer)vvDataRecord, vvSubModul, theSubModul);
      vvFindDialog.TheRecListUC = vvRecListUC;


      return vvFindDialog;

   }

   #endregion VvFindDialog

   #region PrintDocumentRecord   !!!! ovdje nastaje MixerFilter a na Bebama se overridaju reporti ....

   //public MixerFilterUC  TheMixerFilterUC  { get; set; }
   //public MixerFilter    TheMixerFilter { get; set; }

   public override string VirtualReportName
   {
      get
      {
         return this.specificMixerReportName;
      }
   }

   // OVO SE OVERRIDA NA SVAKOJ BEBI JERO CE SVAKA IMATI SVOJE REPORTE I FILTERE
   //public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   //{
   //   MixerFilter mixerFilter = (MixerFilter)vvRptFilter;

   //   switch(mixerFilter.PrintVirZnp)
   //   {
   //      case VirmanDocFilter.PrintVirZnpEnum.Virmani: specificMixerReport = new RptX_Virman(new Vektor.Reports.XIZ.CR_Xvirmani(), reportName, mixerFilter); break;
   //      case VirmanDocFilter.PrintVirZnpEnum.Znp    : specificMixerReport = new RptX_Virman(new Vektor.Reports.XIZ.CR_Xvirmani(), reportName, mixerFilter); break;

   //      default: ZXC.aim_emsg("{0}\nPrintSomeVirmanDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintVirZnp); return null;
   //   }

   //   return specificMixerReport;
   //}

   //public override VvRptFilter VirtualRptFilter
   //{
   //   get
   //   {
   //      return this.TheMixerFilter;
   //   }
   //}

   //public override VvFilterUC VirtualFilterUC
   //{
   //   get
   //   {
   //      return this.TheMixerFilterUC;
   //   }
   //}

   //public override void SetFilterRecordDependentDefaults()
   //{
   //}

   #endregion PrintDocumentRecord

   public override string TabPageTitle1
   {
      get { return "Osnovno"; }
   }
   public override string TabPageTitle2
   {
      get { return ""; }
   }
   public override string TabPageTitle3
   {
      get { return ""; }
   }

   public override VvLookUpLista TheTtLookUpList
   {
      get { return ZXC.luiListaMixerType; }
   }

   #endregion Overriders and specifics

   #region CreateMixerDokumentPrintUC

   //protected void CreateMixerDokumentPrintUC(VvUserControl vvUC)
   //{
   //   this.TheMixerFilter = new MixerFilter(this);

   //   TheMixerFilterUC         = new MixerFilterUC(vvUC);
   //   TheMixerFilterUC.Parent  = ThePanelForFilterUC_PrintTemplateUC;
   //   ThePanelForFilterUC_PrintTemplateUC.Width = TheMixerFilterUC.Width;
   //}

   #endregion CreateMixerDokumentPrintUC

   #region Colors

   private void SetColors()
   {
      clr_Raster       = Color.LightBlue;
      clr_PutNal       = Color.Lavender;
      clr_PutNal2      = Color.LavenderBlush;
      clr_Virm         = Color.SteelBlue;
      clr_Zahtj        = Color.NavajoWhite;
      clr_SMD          = Color.LightGreen;
      clr_EVD          = Color.Aquamarine;
      clr_RVR          = Color.MediumSeaGreen;
      clr_IRV          = Color.LightSeaGreen;
      clr_GFI          = Color.MintCream;
      clr_Plan         = Color.DodgerBlue;
      clr_PMV          = Color.FromArgb(250, 116, 116);
      clr_MVR_NEdjelja = Color.FromArgb(207, 183, 194);
      clr_MVR_SUbota   = Color.FromArgb(223, 207, 214);
      clr_MVR_Praznik  = Color.FromArgb(253, 185, 253);
      clr_MVR_VR_Fore  = Color.FromArgb(18 , 54 , 97 );
      clr_MVR_VR_Back  = Color.FromArgb(207, 214, 223);
      clr_URZ          = Color.FromArgb(207, 183, 194);
      clr_RUG          = Color.FromArgb(223, 207, 214);
      clr_KOP_PTG      = Color.FromArgb(252, 98, 98);
      clr_KOPLine_PTG = Color.FromArgb(255, 164, 164);
   }

   public void SetUpColor(Color clrTampHeader, Color lblCrta, Color tabPagePoly2)
   {
      if(!clrTampHeader.IsEmpty) TheVvTabPage.tbx_col1.Tag = TheVvTabPage.tbx_col2.Tag = TheVvTabPage.tbx_col3.Tag = clrTampHeader;

      if(!lblCrta.IsEmpty) ZXC.vvColors.tamponPanel_Crta = lblCrta;
      else ZXC.vvColors.tamponPanel_Crta = Color.White;

      if(!tabPagePoly2.IsEmpty)
      {
         ThePolyGridTabControl.TabPages[TabPageTitle1].Tag = clrTampHeader;
         ThePolyGridTabControl.TabPages[TabPageTitle2].Tag = tabPagePoly2;
      }

      VvHamper.ApplyVVColorAndStyleTabCntrolChange(this);
   }

   #endregion Colors

   #region Q's

   public void GoToProjektCD_RISK_Dokument_Click(object sender, EventArgs e)
   {
      string tt;
      uint ttNum;

      Ftrans.ParseTipBr(Fld_ProjektCD, out tt, out ttNum);

      if(tt.IsEmpty()) return;

      FakturDUC.GoTo_RISK_Dokument(tt, ttNum);

   }

   public void GoTo_MIXER_Dokument_Click(object sender, EventArgs e)
   {
      Button btn = sender as Button;

      int vezaRbr = (int)btn.Tag;

      switch(vezaRbr)
      {
         case 1: if(Fld_V1_tt.NotEmpty() && Fld_V1_ttNum.NotZero()) FakturDUC.GoTo_RISK_Dokument(Fld_V1_tt, Fld_V1_ttNum); break;
         case 2: if(Fld_V2_tt.NotEmpty() && Fld_V2_ttNum.NotZero()) FakturDUC.GoTo_RISK_Dokument(Fld_V2_tt, Fld_V2_ttNum); break;
      }
   }

   private void OnExit_Relacija_RaiseKmChanged(object sender, EventArgs e)
   {
      VvTextBox vvtb = sender as VvTextBox;

      if(vvtb.EditedHasChanges() == false) return;

      PutNalDUC putNalDUC = this as PutNalDUC;

      int rIdx = TheG.CurrentRow.Index;

      putNalDUC.OnExitIntAIntBKolKm_CalcOthers(rIdx, e);

      GetDgvLineFields1(rIdx, false, null); // da napuni Document's business.Transes 

      PutDgvLineResultsFields1(rIdx, dgvXtrans_rec, true);

      PutDgvTransSumFields1();
   }

   public void OnExitKonto_ClearPreffix(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvTextBoxEditingControl vvTbKonto = sender as VvTextBoxEditingControl;

      string dirtyString = vvTbKonto.Text, cleanString;

      int spaceIdx = dirtyString.IndexOf(' ');

      if(dirtyString.Length.IsZero() || spaceIdx.IsNegative()) return;

      cleanString = dirtyString.Substring(0, spaceIdx);

      //vvTbKonto.Text = cleanString;
      //TheG.EditingControl.Text = cleanString;
      TheG.PutCell(ci.iT_konto, vvTbKonto.EditingControlDataGridView.CurrentRow.Index, cleanString);
   }

   private void AnyKupdobTextBoxLeave(object sender, EventArgs e)
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
            TheG.PutCell(ci.iT_kpdbUlBrA_32, currRow, kupdob_rec.Ticker);
            TheG.PutCell(ci.iT_kupdobCD, currRow, kupdob_rec.KupdobCD);
            TheG.PutCell(ci.iT_kpdbNameA_50, currRow, kupdob_rec.Naziv);
         }
         else
         {
            TheG.PutCell(ci.iT_kpdbUlBrA_32, currRow, "");
            TheG.PutCell(ci.iT_kupdobCD, currRow, "");
            TheG.PutCell(ci.iT_kpdbNameA_50, currRow, "");
         }

         // samo za DUC-eve
         ZXC.TheVvForm.SetDirtyFlag(sender);
      }
   }

   public void AnyPersonDgvTextBoxLeave(object sender, EventArgs e)
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
         theGrid.PutCell(theGrid.IdxForColumn("T_kpdbNameA_50"), currRow, person_rec.Prezime);
         theGrid.PutCell(theGrid.IdxForColumn("T_kpdbNameB_50"), currRow, person_rec.Ime);
      }
      else
      {
         theGrid.PutCell(theGrid.IdxForColumn("T_personCD"), currRow, 0);
         theGrid.PutCell(theGrid.IdxForColumn("T_kpdbNameA_50"), currRow, "");
         theGrid.PutCell(theGrid.IdxForColumn("T_kpdbNameB_50"), currRow, "");
      }

      if(this is RvrMjesecDUC && person_rec != null)
      {
         RvrMjesecDUC rvrMjesecDUC = this as RvrMjesecDUC;
         decimal lastPersonsPtransT_dfs = GetLastPersonsPtransT_dfs(person_rec.PersonCD);

         // ako uopce ne nadje lastPersonPtrans ili je lastPersonsPtransT_dfs nula, tada postavi SluzbeniDnevniFondRadniSati 
         if(lastPersonsPtransT_dfs.IsZero())
         {
            lastPersonsPtransT_dfs = ZXC.GetSluzbeniDnevniFondRadniSati(rvrMjesecDUC.Fld_IsTrgFondSati, rvrMjesecDUC.Fld_MMYYYY, rvrMjesecDUC.Fld_FondSati);
         }

         theGrid.PutCell(ci.iT_moneyA, currRow, lastPersonsPtransT_dfs);    // DFS 
         theGrid.PutCell(ci.iT_moneyB, currRow, rvrMjesecDUC.Fld_FondSati); // MFS klasicni 

         rvrMjesecDUC.GetDgvLineFields1(currRow, false, null); // business ok 

         theGrid.PutCell(ci.iT_RFS, currRow, rvrMjesecDUC.dgvXtrans_rec.R_MVR_RFS);
         theGrid.PutCell(ci.iT_PFS, currRow, rvrMjesecDUC.dgvXtrans_rec.R_MVR_PFS);
         theGrid.PutCell(ci.iT_MFS, currRow, rvrMjesecDUC.dgvXtrans_rec.R_MVR_MFS);

      }

      // samo za DUC-eve
      ZXC.TheVvForm.SetDirtyFlag(sender);
   }

   private decimal GetLastPersonsPtransT_dfs(uint personCD)
   {
      ZXC.DbNavigationRestrictor dbNavigationRestrictor = new ZXC.DbNavigationRestrictor("t_tt", new string[] { Placa.TT_REDOVANRAD, Placa.TT_PODUZETPLACA });

      Ptrans prevPtrans_rec = PersonDao.GetPrevPtransForPerson(TheDbConnection, personCD, DateTime.Now, 0, dbNavigationRestrictor);

      if(prevPtrans_rec.T_recID.IsZero()) // nije nadjen prevPtrans 
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nema prethodnog ptransa za sifru djelatnika [{0}]!\n\nNe mogu odrediti dnevni fond sati!", personCD);
         return 0M;
      }

      return prevPtrans_rec.T_dnFondSati;
   }

   private void AnyKupdobTextBoxLeave_B(object sender, EventArgs e)
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
            TheG.PutCell(ci.iT_kpdbNameB_50, currRow, kupdob_rec.Naziv);
            TheG.PutCell(ci.iT_kpdbUlBrB_32, currRow, kupdob_rec.Ulica1);
            TheG.PutCell(ci.iT_kpdbMjestoB_32, currRow, kupdob_rec.Grad);
            TheG.PutCell(ci.iT_kpdbZiroB_32, currRow, ZXC.GetIBANfromOldZiro(kupdob_rec.Ziro1));
         }
         else
         {
            TheG.PutCell(ci.iT_kpdbNameB_50, currRow, "");
            TheG.PutCell(ci.iT_kpdbUlBrB_32, currRow, "");
            TheG.PutCell(ci.iT_kpdbMjestoB_32, currRow, "");
            TheG.PutCell(ci.iT_kpdbZiroB_32, currRow, "");
         }

         // samo za DUC-eve
         ZXC.TheVvForm.SetDirtyFlag(sender);
      }
   }
   private void AnyKupdobTextBoxLeave_A(object sender, EventArgs e)
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
            TheG.PutCell(ci.iT_kpdbNameA_50, currRow, kupdob_rec.Naziv);
            TheG.PutCell(ci.iT_kpdbUlBrA_32, currRow, kupdob_rec.Ulica1);
            TheG.PutCell(ci.iT_kpdbMjestoA_32, currRow, kupdob_rec.Grad);
            TheG.PutCell(ci.iT_kpdbZiroA_32, currRow, ZXC.GetIBANfromOldZiro(kupdob_rec.Ziro1));
         }
         else
         {
            TheG.PutCell(ci.iT_kpdbNameA_50, currRow, "");
            TheG.PutCell(ci.iT_kpdbUlBrA_32, currRow, "");
            TheG.PutCell(ci.iT_kpdbMjestoA_32, currRow, "");
            TheG.PutCell(ci.iT_kpdbZiroA_32, currRow, "");
         }

         // samo za DUC-eve
         ZXC.TheVvForm.SetDirtyFlag(sender);
      }
   }

   public void AnyArtiklTextBox_OnGrid_Leave(object sender, EventArgs e)
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
         theGrid.PutCell(ci.iT_artiklCD  , currRow, artikl_rec.ArtiklCD);
         theGrid.PutCell(ci.iT_artiklName, currRow, artikl_rec.ArtiklName);

         if(this is RasterDUC || this is RasterBDUC)
         {
            theGrid.PutCell(ci.iT_moneyA, currRow, artikl_rec.ImportCij);
         }

         if(this is ZahtjeviDUC) theGrid.PutCell(ci.iT_kpdbMjestoA_32, currRow, artikl_rec.JedMj);

         #region RasterBDUC: eventual Linked Artikl1 and eventual Linked Artikl2

         if(this is RasterBDUC) // eventual Linked Artikl1 and eventual Linked Artikl2 
         {
            // 01.03.2021:
            theGrid.PutCell(ci.iT_moneyB, currRow, 0.00M);
            theGrid.PutCell(ci.iT_moneyC, currRow, 0.00M);

            if(artikl_rec.LinkArtCD.NotEmpty())
            {
               Artikl linkedArtikl_rec1 = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artikl_rec.LinkArtCD);

               if(linkedArtikl_rec1 != null)
               {
                  theGrid.PutCell(ci.iT_moneyB, currRow, linkedArtikl_rec1.ImportCij);

                  if(linkedArtikl_rec1.LinkArtCD.NotEmpty())
                  {
                     Artikl linkedArtikl_rec2 = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == linkedArtikl_rec1.LinkArtCD);

                     if(linkedArtikl_rec2 != null)
                     {
                        theGrid.PutCell(ci.iT_moneyC, currRow, linkedArtikl_rec2.ImportCij);
                     }
                  }
               }
            }
         }

         #endregion RasterBDUC: eventual Linked Artikl1 and eventual Linked Artikl2

         if(this is PmvDUC)
         {
            theGrid.PutCell(ci.iT_moneyB, currRow, artikl_rec.EmisCO2);
            theGrid.PutCell(ci.iT_moneyC, currRow, artikl_rec.Zapremina);
            theGrid.PutCell(ci.iT_intA  , currRow, artikl_rec.EuroNorma_u);

            ArtStat artStat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, artikl_rec.ArtiklCD, ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD);

            if(artStat_rec != null)
            {
               theGrid.PutCell(ci.iT_moneyA, currRow, artStat_rec.LastUlazMPC);
            }
         }

      }
      else if(this.sifrarSorterType == VvSQL.SorterType.Name && vvtb_editingControl.Text != "") // ako smo dosli iz naziva, a artikl_rec je null, to je onda 'qwe' pattern (ne postoji kao sifrar) 
      {
         theGrid.PutCell(ci.iT_artiklName, currRow, vvtb_editingControl.Text);
      }

      // samo za DUC-eve
      ZXC.TheVvForm.SetDirtyFlag(sender);
   }

   private int GetOneInteger4EuroNorma(ZXC.EuroNormaEnum enEnum)
   {
      switch(enEnum)
      {
         case ZXC.EuroNormaEnum.NIJEDNA: return 0;
         case ZXC.EuroNormaEnum.EuroI: return 1;
         case ZXC.EuroNormaEnum.EuroII: return 2;
         case ZXC.EuroNormaEnum.EuroIII: return 3;
         case ZXC.EuroNormaEnum.EuroIV: return 4;

         default: return 0;
      }
   }

   private ZXC.EuroNormaEnum GetEuroNormaEnumFromInteger(int num)
   {
      switch(num)
      {
         case 0: return ZXC.EuroNormaEnum.NIJEDNA;
         case 1: return ZXC.EuroNormaEnum.EuroI;
         case 2: return ZXC.EuroNormaEnum.EuroII;
         case 3: return ZXC.EuroNormaEnum.EuroIII;
         case 4: return ZXC.EuroNormaEnum.EuroIV;

         default: return ZXC.EuroNormaEnum.NIJEDNA;
      }
   }

   private void NZI_Grid_CellMouseDoubleClick_OpenFakturDUC(object sender, DataGridViewCellMouseEventArgs e)
   {
    //if((this is ZahtjevRNMDUC) == false) return;
      if((this is ZahtjevRNMDUC) == false && this is PredNrdDUC == false) return;

      int rowIdx = e.RowIndex;

      if(rowIdx.IsNegative()) return;

    //if(e.ColumnIndex != ci.iT_kpdbZiroB_32) return;
      if(e.ColumnIndex != ci.iT_kpdbZiroB_32 && e.ColumnIndex != ci.iT_kpdbZiroA_32) return;

      string tipBr = TheG.GetStringCell(e.ColumnIndex == ci.iT_kpdbZiroB_32 ? ci.iT_kpdbZiroB_32 : ci.iT_kpdbZiroA_32, rowIdx, false);

      if(tipBr.IsEmpty()) return;

      string tt;
      uint ttNum;

      bool parseOK = Ftrans.ParseTipBr(tipBr, out tt, out ttNum);

      if(parseOK == false) return;

      Faktur faktur_rec = new Faktur();

      bool fakturOK = ZXC.FakturDao.SetMe_VvDocumentRecord_byTtAndTtNum(TheDbConnection, faktur_rec, tt, ttNum, false, false);

      if(fakturOK)
      {
         ZXC.VvSubModulEnum vvSubModulEnum = FakturDUC.GetVvSubModulEnum_ForTT(tt);

         if(vvSubModulEnum == ZXC.VvSubModulEnum.UNDEF) return;

         TheVvTabPage.TheVvForm.OpenNew_Record_TabPage_wInitialRecord(TheVvTabPage.TheVvForm.GetSubModulXY(vvSubModulEnum), faktur_rec);
      }
   }

   #endregion Q's

   #region DataGridView_CellValueChanged_GFI

   private void DataGridView_CellValueChanged_GFI()
   {
      VvDataGridView dgv = TheG;
      int rowIdx;
      for(int i = 0; i < dgv.Rows.Count; i++)
      {
         rowIdx = i;
        
         foreach(DataGridViewTextBoxCell tbxCell in dgv.Rows[rowIdx].Cells)
         {
            if(dgv.Rows[rowIdx].Cells[ci.iT_strA_2].Value != null && dgv.Rows[rowIdx].Cells[ci.iT_strA_2].Value.ToString() == "N")
            {
               tbxCell.Style.BackColor = Color.LightSkyBlue;
            }
            else if(dgv.Rows[rowIdx].Cells[ci.iT_strA_2].Value != null && dgv.Rows[rowIdx].Cells[ci.iT_strA_2].Value.ToString() == "S")
            {
               tbxCell.Style.BackColor = Color.PaleGreen;
            }
            else if(dgv.Rows[rowIdx].Cells[ci.iT_strA_2].Value != null && dgv.Rows[rowIdx].Cells[ci.iT_strA_2].Value.ToString() == "O")
            {
               tbxCell.Style.BackColor = Color.LightGray;
            }

         }
      }
   }

   private void GFI_ZeroFillInt()
   {
    
      if(Fld_TT == Mixer.TT_TSI) colVvTextInt.DefaultCellStyle.Format = GetDgvCellStyleFormat_ZeroFillInt(2, true);
      else                       colVvTextInt.DefaultCellStyle.Format = GetDgvCellStyleFormat_ZeroFillInt(3, true);

   }

   #endregion DataGridView_CellValueChanged_GFI
   
}

public class VvRpt_Mix_Filter : VvRptFilter
{
   public VvRpt_Mix_Filter()
   {
      SetDefaultFilterValues();
   }

   public override void SetDefaultFilterValues()
   {
      ZXC.VvDataBaseInfo vvDBinfo = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects;

      int projectYear = int.Parse(vvDBinfo.ProjectYear);
      int thisYear = DateTime.Now.Year;

      DatumOd = vvDBinfo.ProjectYearFirstDay;

      if(projectYear < thisYear)
      {
         DatumDo = vvDBinfo.ProjectYearLastDay;
      }
      else
      {
         DatumDo = DateTime.Now;
      }

      SorterType_Dokument = VvSQL.SorterType.DokDate;
   }


   #region propertiez
  
   public DateTime DatumDo       { get; set; }
   public DateTime DatumOd       { get; set; }
   public string   KD_naziv      { get; set; } 
   public string   KD_ticker     { get; set; } 
   public uint     KD_sifra      { get; set; } 
   public string   TT            { get; set; } 
   public uint     TtNumOd       { get; set; } 
   public uint     TtNumDo       { get; set; } 
   public string   MT_naziv      { get; set; } 
   public string   MT_ticker     { get; set; } 
   public uint     MT_sifra      { get; set; }
   public uint     PersonCD      { get; set; }
   public string   PresonPrezime { get; set; }
   public string   Napomena      { get; set; } 
   public string   ProjektCd     { get; set; } 
   public string   ValName       { get; set; } 
   public uint     DokNumOd      { get; set; } 
   public uint     DokNumDo      { get; set; }
   public string   StrC_32       { get; set; }
   public string   StrH_32       { get; set; }
   public string   StrA_40       { get; set; }
   public string   StrG_40       { get; set; }

   public string GrupiranjePutNal     { get; set; }
   public string GrupiranjeZahtjeva   { get; set; }
   public string GrupiranjeEvidencija { get; set; }
   public string GrupiranjeUgovora    { get; set; }
   public string GrupiranjeSMD        { get; set; }
   public string GrupiranjeIRV        { get; set; }

   public string ZaMMYYYY             { get; set; }
   public bool   IsMVR_RadMj          { get; set; }
   public bool   IsKnjigaDomaca       { get; set; }
   public bool   IsPrekoPFS           { get; set; }
   public bool   IsGrupaPoStr         { get; set; }
   public bool   IsOnlyUkPers         { get; set; }

   public ZXC.JeliJeTakav IsAktivan { get; set; }

   #endregion propertiez

}

public class VvOneRowActionDlg : VvDialog
{
   #region Filedz
    
   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_rowDgv;
   private Label     lbl_text;

   #endregion Filedz

   #region Constructor

   public VvOneRowActionDlg()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Akcija za 1 Redak";

      CreateHamper();

      dlgWidth        = hamper.Right  + ZXC.QunMrgn;
      dlgHeight       = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_rowDgv, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
   }

   #endregion Constructor

   #region hamper

   private void CreateHamper()
   {
      hamper          = new VvHamper(2, 1, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q7un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      //int columnSize = (int)ZXC.KplanDao.GetSchemaColumnSize(ZXC.KplanDao.CI.konto);

      lbl_text   = hamper.CreateVvLabel  (0, 0, "", ContentAlignment.MiddleRight);
      tbx_rowDgv = hamper.CreateVvTextBox(1, 0, "tbx_rowDgv", "", 4);
      //tbx_rowDgv.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_rowDgv.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, false);

   }

   #endregion hamper

   #region Button_Click

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Button_Click

   #region Fld_ 

   public int    Fld_OneRow { get { return tbx_rowDgv.GetIntField(); } set { tbx_rowDgv.PutIntField(value); } }
   public string Fld_LblText { get { return lbl_text  .Text; } set { lbl_text  .Text = value; } }

   #endregion Fld_

}
  
public class VvAVRfromRNZDlg : VvDialog
{
   #region Filedz
    
   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_zaMj;

   #endregion Filedz

   #region Constructor

   public VvAVRfromRNZDlg(string text)
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = text;

      CreateHamper();

      dlgWidth        = hamper.Right  + ZXC.QunMrgn;
      dlgHeight       = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_zaMj, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
   }

   #endregion Constructor

   #region hamper

   private void CreateHamper()
   {
      hamper          = new VvHamper(2, 1, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                 hamper.CreateVvLabel        (0, 0, "Za mjesec:", ContentAlignment.MiddleRight);
      tbx_zaMj = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_zaMj", "Za mjesec", 6);
      tbx_zaMj.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      VvLookUpLista fondSatiLista = ZXC.luiListaFondSati_NOR;
      tbx_zaMj.JAM_Set_LookUpTable(fondSatiLista, (int)ZXC.Kolona.prva);
   }

   #endregion hamper

   #region Button_Click

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Button_Click

   #region Fld_ 

   public string Fld_ZaMjesec { get { return tbx_zaMj.Text; } set { tbx_zaMj.Text = value; } }

   #endregion Fld_

}

