using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;


public partial class BmwDUC : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_artiklCd               ,// _strC_32 
                     tbx_artiklName             ,// _strA_40 
                     tbx_RealMPC_EUR            ,// _moneyA  
                     tbx_HR_historyOsnovnaCij_Kn,// _moneyB  
                     tbx_HR_historyOprermaCij_Kn,// _moneyC  
                     tbx_mm1Registracije        ,// _intA    
                     tbx_yy1Registracije        ,// _intB    
                     tbx_co2Emisija             ,// _mtrosCD 
                     tbx_km                     ,// _personCD 
                   //tbx_buyOnDate              ,// _dokDate 
                     tbxR_PPMV_history          , tbxR_PPMV_historyEur,
                     tbxR_PPMVkn                ,
                     tbxR_PPMVeur               ,
                     tbxR_toPayAutohouseEUR     ,
                     tbxR_toPayAutohouseKn      ,
                     tbxR_toPayHR_PDVkn         ,
                     tbxR_good_PDVkn            ,
                     tbxR_toPayTotalKn          ,
                     tbxR_toPayTotalEur         ,
                     tbxR_finalCostKn           ,
                     tbxR_finalCostEUR          ,
                     tbxR_todayTecaj            ,
                     tbxR_VN, tbxR_PC, tbxR_VNPC, tbxR_VNPCEur,
                     tbxR_ON, tbxR_EN, tbxR_ONEN, tbxR_ONENEur,
                     tbxR_monthAge, tbxR_ageSt,
                     tbxR_HR_history_TOTAL_Cij_Kn;
      
   private CheckBox cbx_isBenzinac;

   private VvHamper   hamp_artikl, hamp_veze;

   #endregion Fieldz

   #region Constructor

   public BmwDUC(Control parent, Mixer _mixer, VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)
   {

      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_BMW
         });
   
   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
      InitializeHamper_Artikl(out hamp_artikl);
      hamp_artikl.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);

      hamp_dokDate.Location = new Point(hamp_artikl.Right - hamp_dokDate.Width, ZXC.QunMrgn);
      hamp_tt     .Location = new Point(hamp_artikl.Right                     , ZXC.QunMrgn);


      nextX = hamp_artikl.Right;
      nextY = hamp_tt.Bottom + ZXC.Qun4;

      InitializeHamper_Veze(out hamp_veze);

      hamp_veze    .Location = new Point(hamp_artikl.Right, hamp_tt    .Bottom);
      hamp_napomena.Location = new Point(ZXC.QunMrgn  , hamp_artikl.Bottom);
      hamp_dokNum  .Location = new Point(hamp_veze.Right -hamp_dokNum .Width, hamp_napomena.Top);
      nextY = hamp_napomena.Bottom;

      hamp_dokDate.BringToFront();
      hamp_tt     .BringToFront();
      hamp_dokNum .BringToFront();


      TheTabControl.TabPages[0].BackColor =

      hamp_napomena.BackColor =
      hamp_dokDate .BackColor = 
      hamp_dokNum  .BackColor = 
      hamp_artikl  .BackColor = 
      hamp_veze    .BackColor = 
      hamp_tt      .BackColor = Color.FromArgb(182, 163, 102);

   }

   private void InitializeHamper_Artikl(out VvHamper hamper)
   {
      int decimalCol = ZXC.Q4un - ZXC.Qun2;

      hamper = new VvHamper(11, 7, "", TheTabControl.TabPages[0], false/*, nextX, nextY, razmakHamp*/);
      //                                     0         1         2         3              4         5          6            7          8         9           10    
      hamper.VvColWdt      = new int[] { ZXC.Q6un, decimalCol, ZXC.Q4un, decimalCol, decimalCol, ZXC.Q4un, decimalCol, decimalCol, ZXC.Q4un, decimalCol, decimalCol};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,   ZXC.Qun4, ZXC.Qun4,   ZXC.Qun4,   ZXC.Qun4, ZXC.Qun4,   ZXC.Qun4,   ZXC.Qun4, ZXC.Qun4,   ZXC.Qun4,   ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel  (0, 0, "Artikl:"                 , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel  (6, 0, "todayTecaj:"             , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel  (0, 1, "RealMPC_EUR:"            , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel  (0, 2, "HR_historyOsnovnaCij_Kn:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel  (2, 2, "OprermaCij_Kn:"          , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel  (0, 3, "HR_history_TOTAL_Cij_Kn:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel  (0, 4, "Co2Emisija:"             , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel  (0, 5, "mm1Registracije:"        , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel  (0, 6, "yy1Registracije:"        , ContentAlignment.MiddleRight);


      tbx_artiklCd                 = hamper.CreateVvTextBox(1, 0, "tbx_artiklCd"                , "artiklCd"               , GetDB_ColumnSize(DB_ci.strC_32));  
      tbx_artiklName               = hamper.CreateVvTextBox(2, 0, "tbx_artiklName"              , "artiklName"             , GetDB_ColumnSize(DB_ci.strA_40), 3, 0); 
      tbxR_todayTecaj              = hamper.CreateVvTextBox(7, 0, "tbxR_todayTecaj"             , "todayTecaj"             , 12); 
      tbx_RealMPC_EUR              = hamper.CreateVvTextBox(1, 1, "tbx_RealMPC_EUR"             , "RealMPC_EUR"            , GetDB_ColumnSize(DB_ci.moneyA) ); 
      tbx_HR_historyOsnovnaCij_Kn  = hamper.CreateVvTextBox(1, 2, "tbx_HR_historyOsnovnaCij_Kn" , "HR_historyOsnovnaCij_Kn", GetDB_ColumnSize(DB_ci.moneyB) ); 
      tbx_HR_historyOprermaCij_Kn  = hamper.CreateVvTextBox(3, 2, "tbx_HR_historyOprermaCij_Kn" , "HR_historyOprermaCij_Kn", GetDB_ColumnSize(DB_ci.moneyC) );
      tbxR_HR_history_TOTAL_Cij_Kn = hamper.CreateVvTextBox(1, 3, "tbxR_HR_history_TOTAL_Cij_Kn", "HR_history_TOTAL_Cij_Kn", 12);
      tbx_co2Emisija               = hamper.CreateVvTextBox(1, 4, "tbx_co2Emisija"              , "co2Emisija"             , GetDB_ColumnSize(DB_ci.mtrosCD));
      tbx_mm1Registracije          = hamper.CreateVvTextBox(1, 5, "tbx_mm1Registracije"         , "mm1Registracije"        , GetDB_ColumnSize(DB_ci.intA)); 
      tbx_yy1Registracije          = hamper.CreateVvTextBox(1, 6, "tbx_yy1Registracije"         , "yy1Registracije"        , GetDB_ColumnSize(DB_ci.intB)   ); 

      tbx_RealMPC_EUR             .JAM_FieldExitMethod = new EventHandler(OnExit_Recalc);
      tbx_HR_historyOsnovnaCij_Kn .JAM_FieldExitMethod = new EventHandler(OnExit_Recalc);
      tbx_HR_historyOprermaCij_Kn .JAM_FieldExitMethod = new EventHandler(OnExit_Recalc);
      tbx_co2Emisija              .JAM_FieldExitMethod = new EventHandler(OnExit_Recalc);
      tbx_mm1Registracije         .JAM_FieldExitMethod = new EventHandler(OnExit_Recalc);
      tbx_yy1Registracije         .JAM_FieldExitMethod = new EventHandler(OnExit_Recalc);

      SetSifrarAndAutocomplete<Artikl>(tbx_artiklName, VvSQL.SorterType.Name);

      tbx_artiklCd  .JAM_SetAutoCompleteData(Artikl.recordName, Artikl.sorterCD  .SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Artikl_sorterSifra), new EventHandler(AnyArtiklTextBoxLeave));
      tbx_artiklName.JAM_SetAutoCompleteData(Artikl.recordName, Artikl.sorterName.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Artikl_sorterName ), new EventHandler(AnyArtiklTextBoxLeave));

      cbx_isBenzinac = hamper.CreateVvCheckBox_OLD(3, 1, null, "Benzinac", System.Windows.Forms.RightToLeft.Yes);
      cbx_isBenzinac.Name = "cbx_isBenzinac";

      tbx_km        = hamper.CreateVvTextBox(4, 1, "tbx_km", "km", GetDB_ColumnSize(DB_ci.personCD));
      hamper.CreateVvLabel(5, 1, "km", ContentAlignment.MiddleLeft);


      tbx_RealMPC_EUR.JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_HR_historyOsnovnaCij_Kn.JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_HR_historyOprermaCij_Kn.JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);

      hamper.CreateVvLabel(2, 3, "VN PC:"    , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 4, "ON EN:"    , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 5, "monthAge:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 6, "ageSt:"   , ContentAlignment.MiddleRight);

      tbxR_VN        = hamper.CreateVvTextBox(3, 3, "tbxR_VN"       , "VN"       , 12);
      tbxR_PC        = hamper.CreateVvTextBox(4, 3, "tbxR_PC"       , "PC"       , 12);
   // tbxR_VNPC      = hamper.CreateVvTextBox(5, 4, "tbxR_VNPC"     , "VNPC"     , 12);
      tbxR_ON        = hamper.CreateVvTextBox(3, 4, "tbxR_ON"       , "ON"       , 12);
      tbxR_EN        = hamper.CreateVvTextBox(4, 4, "tbxR_EN"       , "EN"       , 12);
    //tbxR_ONEN      = hamper.CreateVvTextBox(5, 5, "tbxR_ONEN"     , "ONEN"     , 12);
      tbxR_monthAge  = hamper.CreateVvTextBox(3, 5, "tbxR_monthAge" , "monthAge" , 12);
      tbxR_ageSt     = hamper.CreateVvTextBox(3, 6, "tbxR_ageSt"    , "ageSt"    , 12);
           
      hamper.CreateVvLabel  (5,  3, "VNPC:"          , ContentAlignment.MiddleRight); //
      hamper.CreateVvLabel  (5,  4, "ONEN:"          , ContentAlignment.MiddleRight); //
      hamper.CreateVvLabel  (5,  5, "PPMV_history:"  , ContentAlignment.MiddleRight); //
      hamper.CreateVvLabel  (5,  6, "PPMV:"          , ContentAlignment.MiddleRight); //hamper.CreateVvLabel(0,  1, "PPMVeur:"          , ContentAlignment.MiddleRight);
                                                     
      hamper.CreateVvLabel ( 9, 1, "Kn:"             , ContentAlignment.MiddleRight); hamper.CreateVvLabel(10,  1, "EUR:"              , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel ( 8, 2, "toPayAutohouse:" , ContentAlignment.MiddleRight); //hamper.CreateVvLabel(0,  2, "toPayAutohouseEUR:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel ( 8, 3, "toPayHR_PDV:"    , ContentAlignment.MiddleRight); //
      hamper.CreateVvLabel ( 8, 4, "good_PDV:"       , ContentAlignment.MiddleRight); //
      hamper.CreateVvLabel ( 8, 5, "toPayTotal:"     , ContentAlignment.MiddleRight); //hamper.CreateVvLabel(0,  5, "toPayTotalEur:"    , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel ( 8, 6, "finalCost:"      , ContentAlignment.MiddleRight); //hamper.CreateVvLabel(0,  6, "finalCostEUR:"     , ContentAlignment.MiddleRight);


      tbxR_VNPC            = hamper.CreateVvTextBox(6, 3, "tbxR_VNPC", "VNPC", 12);
         tbxR_VNPCEur         = hamper.CreateVvTextBox(7, 3, "tbxR_VNPCEur", "VNPC", 12);
      tbxR_ONEN            = hamper.CreateVvTextBox(6, 4, "tbxR_ONEN", "ONEN", 12);
         tbxR_ONENEur            = hamper.CreateVvTextBox(7, 4, "tbxR_ONENEur", "ONEN", 12);
      tbxR_PPMV_history      = hamper.CreateVvTextBox(6,  5, "tbxR_PPMV_history"     , "PPMV_history"     , 12);     
         tbxR_PPMV_historyEur   = hamper.CreateVvTextBox(7,  5, "tbxR_PPMV_historyEur"  , "PPMV_historyEur"  , 12);     
      tbxR_PPMVkn            = hamper.CreateVvTextBox(6,  6, "tbxR_PPMVkn"           , "PPMVkn"           , 12);     
         tbxR_PPMVeur           = hamper.CreateVvTextBox(7,  6, "tbxR_PPMVeur"          , "PPMVeur"          , 12);     
      
      tbxR_toPayAutohouseKn  = hamper.CreateVvTextBox(9,  2, "tbxR_toPayAutohouseKn" , "toPayAutohouseKn" , 12);     
         tbxR_toPayAutohouseEUR = hamper.CreateVvTextBox(10,  2, "tbxR_toPayAutohouseEUR", "toPayAutohouseEUR", 12);     
      tbxR_toPayHR_PDVkn     = hamper.CreateVvTextBox(9,  3, "tbxR_toPayHR_PDVkn"    , "toPayHR_PDVkn"    , 12);     
      tbxR_good_PDVkn        = hamper.CreateVvTextBox(9,  4, "tbxR_good_PDVkn"       , "good_PDVkn"       , 12);     
      tbxR_toPayTotalKn      = hamper.CreateVvTextBox(9,  5, "tbxR_toPayTotalKn"     , "toPayTotalKn"     , 12);     
         tbxR_toPayTotalEur     = hamper.CreateVvTextBox(10, 5, "tbxR_toPayTotalEur"    , "toPayTotalEur"    , 12);     
      tbxR_finalCostKn       = hamper.CreateVvTextBox(9, 6, "tbxR_finalCostKn"      , "finalCostKn"      , 12);     
         tbxR_finalCostEUR      = hamper.CreateVvTextBox(10, 6, "tbxR_finalCostEUR"     , "finalCostEUR"     , 12);

      tbxR_PPMV_history           .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_PPMV_historyEur        .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_PPMVkn                 .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_PPMVeur                .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_toPayAutohouseEUR      .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_toPayAutohouseKn       .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_toPayHR_PDVkn          .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_good_PDVkn             .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_toPayTotalKn           .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_toPayTotalEur          .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_finalCostKn            .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_finalCostEUR           .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_HR_history_TOTAL_Cij_Kn.JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_todayTecaj             .JAM_MarkAsNumericTextBox(6, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_VN                     .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true); 
      tbxR_PC                     .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_VNPC                   .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_VNPCEur                .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_ON                     .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_EN                     .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_ONEN                   .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_ONENEur                .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_monthAge               .JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);
      tbxR_ageSt                  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      
      tbxR_PPMV_history           .JAM_ReadOnly =
      tbxR_PPMV_historyEur        .JAM_ReadOnly =
      tbxR_PPMVkn                 .JAM_ReadOnly =
      tbxR_PPMVeur                .JAM_ReadOnly =
      tbxR_toPayAutohouseEUR      .JAM_ReadOnly =
      tbxR_toPayAutohouseKn       .JAM_ReadOnly =
      tbxR_toPayHR_PDVkn          .JAM_ReadOnly =
      tbxR_good_PDVkn             .JAM_ReadOnly =
      tbxR_toPayTotalKn           .JAM_ReadOnly =
      tbxR_toPayTotalEur          .JAM_ReadOnly =
      tbxR_finalCostKn            .JAM_ReadOnly =
      tbxR_finalCostEUR           .JAM_ReadOnly =
      tbxR_todayTecaj             .JAM_ReadOnly =
      tbxR_VN                     .JAM_ReadOnly =          
      tbxR_PC                     .JAM_ReadOnly =          
      tbxR_VNPC                   .JAM_ReadOnly =          
      tbxR_VNPCEur                .JAM_ReadOnly =          
      tbxR_ON                     .JAM_ReadOnly =          
      tbxR_EN                     .JAM_ReadOnly =          
      tbxR_ONEN                   .JAM_ReadOnly =          
      tbxR_ONENEur                 .JAM_ReadOnly =          
      tbxR_monthAge               .JAM_ReadOnly =          
      tbxR_ageSt                  .JAM_ReadOnly =          
      tbxR_HR_history_TOTAL_Cij_Kn.JAM_ReadOnly = true;

      this.ControlForInitialFocus = tbx_artiklName;

      tbx_RealMPC_EUR       .JAM_ForeColor = 
      tbxR_toPayAutohouseEUR.JAM_ForeColor = 
      tbxR_toPayTotalEur    .JAM_ForeColor = 
      tbxR_finalCostEUR     .JAM_ForeColor = 
      tbxR_ONENEur          .JAM_ForeColor = 
      tbxR_VNPCEur          .JAM_ForeColor = 
      tbxR_PPMV_historyEur  .JAM_ForeColor = 
      tbxR_todayTecaj       .JAM_ForeColor = 
      tbxR_PPMVeur          .JAM_ForeColor = Color.Green;

      tbxR_finalCostEUR.JAM_Highlighted = true;
      tbxR_finalCostEUR.JAM_ForeColor = Color.BlueViolet;
      tbxR_finalCostEUR.JAM_BackColor = Color.LightGreen;
   }

   private void AnyArtiklTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Artikl artikl_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         artikl_rec = ArtiklSifrar.Find(FoundInSifrar<Artikl>);

         if(artikl_rec != null && tb.Text != "")
         {
            Fld_ArtiklCd   = artikl_rec.ArtiklCD;
            Fld_ArtiklName = artikl_rec.ArtiklName;
         }
         else if(this.sifrarSorterType == VvSQL.SorterType.Name && tb.Text != "") // ako smo dosli iz naziva, a artikl_rec je null, to je onda 'qwe' pattern (ne postoji kao sifrar) 
         {
            Fld_ArtiklCd = "";
         }
         else
         {
            Fld_ArtiklCd = Fld_ArtiklName = "";
         }
      }
   }

   private void InitializeHamper_Veze(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 4, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      //                                     0          1                          2                3                4                   5            
      hamper.VvColWdt      = new int[] { ZXC.Q4un- ZXC.Qun2, ZXC.Q3un- ZXC.Qun4 , ZXC.Q6un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4           ,            ZXC.Qun4, ZXC.Qun4           ,                  0,                  0 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel  (0, 0, "Link1:", ContentAlignment.MiddleRight);
      tbx_externLink1 = hamper.CreateVvTextBox(1, 0, "tbx_externLink1", "Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.externLink1), 2, 0);
      btn_goExLink1   = hamper.CreateVvButton (4, 0, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink1.Name = "btn_goExLink1";
      btn_goExLink1.FlatStyle = FlatStyle.Flat;
      btn_goExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink1.Image = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink1.Tag = 1;
      btn_goExLink1.TabStop = false;
      btn_goExLink1.Visible = false;

      btn_openExLink1 = hamper.CreateVvButton(5, 0, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLink1.Name = "btn_openExLink1";
      btn_openExLink1.FlatStyle = FlatStyle.Flat;
      btn_openExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLink1.Image = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLink1.Tag = 1;
      btn_openExLink1.TabStop = false;

                        hamper.CreateVvLabel  (0, 1, "Link2:", ContentAlignment.MiddleRight);
      tbx_externLink2 = hamper.CreateVvTextBox(1, 1, "tbx_externLink2", "Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.externLink2), 2, 0);
      btn_goExLink2   = hamper.CreateVvButton (4, 1, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink2.Name = "btn_goExLink2";
      btn_goExLink2.FlatStyle = FlatStyle.Flat;
      btn_goExLink2.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink2.Image = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink2.Tag = 2;
      btn_goExLink2.TabStop = false;
      btn_goExLink2.Visible = false;

      btn_openExLink2 = hamper.CreateVvButton(5, 1, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLink2.Name = "btn_openExLink2";
      btn_openExLink2.FlatStyle = FlatStyle.Flat;
      btn_openExLink2.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLink2.Image = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLink2.Tag = 2;
      btn_openExLink2.TabStop = false;

                      hamper.CreateVvLabel        ( 0, 2, "Veza1:", ContentAlignment.MiddleRight);
      tbx_v1_tt     = hamper.CreateVvTextBoxLookUp( 1, 2, "tbx_v1_tt", "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v1_tt));
      tbx_v1_ttOpis = hamper.CreateVvTextBox      ( 2, 2, "tbx_v1_ttOpis", "Veza na interni dokument", 32);
      tbx_v1_ttNum  = hamper.CreateVvTextBox      ( 3, 2, "tbx_v1_ttNum", "Veza na interni dokument", 6/*GetDB_ColumnSize(DB_ci.iz_ttNum)*/);
      btn_v1TT      = hamper.CreateVvButton       ( 5, 2, new EventHandler(GoTo_MIXER_Dokument_Click), "");

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

                      hamper.CreateVvLabel        (0, 3, "Veza2:", ContentAlignment.MiddleRight);
      tbx_v2_tt     = hamper.CreateVvTextBoxLookUp(1, 3, "tbx_v2_tt", "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v2_tt));
      tbx_v2_ttOpis = hamper.CreateVvTextBox      (2, 3, "tbx_v2_ttOpis", "Veza na interni dokument");
      tbx_v2_ttNum  = hamper.CreateVvTextBox      (3, 3, "tbx_v2_ttNum", "Veza na interni dokument" + " broj", 6/*GetDB_ColumnSize(DB_ci.na_ttNum)*/);
      btn_v2TT      = hamper.CreateVvButton       (5, 3, new EventHandler(GoTo_MIXER_Dokument_Click), "");
      btn_v2TT.Name = "v2_TT";

      btn_v2TT.FlatStyle = FlatStyle.Flat;
      btn_v2TT.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_v2TT.Image = VvIco.TriangleBlue16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue2.ico")), 16, 16)*/.ToBitmap();
      btn_v2TT.Tag = 2;
      btn_v2TT.TabStop = false;

      tbx_v2_tt.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_v2_tt.JAM_Set_NOTobligatory_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_v2_tt.JAM_lui_NameTaker_JAM_Name = tbx_v2_ttOpis.JAM_Name;
      tbx_v2_ttOpis.JAM_ReadOnly = true;

      tbx_v2_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      hamper.BringToFront();
   }

   #endregion CreateSpecificHampers()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {

      R_finalCostEUR_CreateColumn           (ZXC.Q4un + ZXC.Qun4, "FinalCostEUR" , "finalCostEUR"           , 0);
                                                                                                           
      R_RealMPC_EUR_CreateColumn            (ZXC.Q4un + ZXC.Qun4, "RealMPC_EUR"  , "RealMPC_EUR"            , 0);
      R_HR_historyOsnovnaCij_Kn_CreateColumn(ZXC.Q4un - ZXC.Qun4, "OsnovnaCij"   , "HR_historyOsnovnaCij_Kn", 0);
      R_HR_historyOprermaCij_Kn_CreateColumn(ZXC.Q4un - ZXC.Qun4, "OprermaCij"   , "HR_historyOprermaCij_Kn", 0);
      R_HR_history_TOTAL_Cij_Kn_CreateColumn(ZXC.Q4un - ZXC.Qun4, "TOTAL_Cij"    , "His_Total_Cij_Kn"       , 0);
      R_co2Emisija_CreateColumn             (ZXC.Q3un - ZXC.Qun2, "Co2Em"        , "co2Emisija"             , 0);
      R_mm1Registracije_CreateColumn        (ZXC.Q3un - ZXC.Qun2, "mm1Reg"       , "mm1Registracije"        , 0);
      R_yy1Registracije_CreateColumn        (ZXC.Q3un - ZXC.Qun2, "yy1Reg"       , "yy1Registracije"        , 0);

      R_toPayTotalEur_CreateColumn          (ZXC.Q4un + ZXC.Qun4, "toPayTotEur"  , "toPayTotalEur"          , 0);
      R_toPayAutohouseEUR_CreateColumn      (ZXC.Q4un + ZXC.Qun4, "toPayAutHouse", "toPayAutohouseEUR"      , 0);
      
      R_monthAge_CreateColumn               (ZXC.Q3un - ZXC.Qun2, "mmAge"         , "monthAge"              , 0);
      R_ageSt_CreateColumn                  (ZXC.Q3un - ZXC.Qun2, "ageSt"         , "ageSt"                 , 2);
      R_PPMVeur_CreateColumn                (ZXC.Q4un - ZXC.Qun2, "PPMV"          , "PPMVeur"               , 0);
      R_PPMV_historyEur_CreateColumn        (ZXC.Q4un - ZXC.Qun2, "PPMV_hist"     , "PPMV_historyEur"       , 0);
      R_ONENEur_CreateColumn                (ZXC.Q4un - ZXC.Qun2, "ONEN"          , "ONENEur"               , 0);
      R_VNPCEur_CreateColumn                (ZXC.Q4un - ZXC.Qun2, "VNPC   "       , "VNPCEur"               , 0);
  }

   #endregion InitializeTheGrid_Columns()

   #region Fld_

   public string  Fld_ArtiklCd                { get { return tbx_artiklCd                .Text;              } set { tbx_artiklCd  .Text                        = value;  } }
   public string  Fld_ArtiklName              { get { return tbx_artiklName              .Text;              } set { tbx_artiklName.Text                        = value;  } }
   public decimal Fld_RealMPC_EUR             { get { return tbx_RealMPC_EUR             .GetDecimalField(); } set { tbx_RealMPC_EUR             .PutDecimalField(value); } }
   public decimal Fld_HR_historyOsnovnaCij_Kn { get { return tbx_HR_historyOsnovnaCij_Kn .GetDecimalField(); } set { tbx_HR_historyOsnovnaCij_Kn .PutDecimalField(value); } }
   public decimal Fld_HR_historyOprermaCij_Kn { get { return tbx_HR_historyOprermaCij_Kn .GetDecimalField(); } set { tbx_HR_historyOprermaCij_Kn .PutDecimalField(value); } }
   public int     Fld_mm1Registracije         { get { return tbx_mm1Registracije         .GetIntField();     } set { tbx_mm1Registracije         .PutIntField    (value); } }
   public int     Fld_yy1Registracije         { get { return tbx_yy1Registracije         .GetIntField();     } set { tbx_yy1Registracije         .PutIntField    (value); } }
   public uint    Fld_co2Emisija              { get { return tbx_co2Emisija              .GetUintField();    } set { tbx_co2Emisija              .PutUintField   (value); } }
   public uint    Fld_km                      { get { return tbx_km                      .GetUintField();    } set { tbx_km                      .PutUintField   (value); } }
   public bool    Fld_IsBenzinac              { get { return cbx_isBenzinac              .Checked;           } set { cbx_isBenzinac.Checked                     = value ; } }
                                                                                                                                                 
   public decimal Fld_PPMV_history            { get { return tbxR_PPMV_history           .GetDecimalField(); } set { tbxR_PPMV_history           .PutDecimalField(value); } }
   public decimal Fld_PPMV_historyEur         { get { return tbxR_PPMV_historyEur        .GetDecimalField(); } set { tbxR_PPMV_historyEur        .PutDecimalField(value); } }
   public decimal Fld_PPMVkn                  { get { return tbxR_PPMVkn                 .GetDecimalField(); } set { tbxR_PPMVkn                 .PutDecimalField(value); } }
   public decimal Fld_PPMVeur                 { get { return tbxR_PPMVeur                .GetDecimalField(); } set { tbxR_PPMVeur                .PutDecimalField(value); } }
   public decimal Fld_toPayAutohouseEUR       { get { return tbxR_toPayAutohouseEUR      .GetDecimalField(); } set { tbxR_toPayAutohouseEUR      .PutDecimalField(value); } }
   public decimal Fld_toPayAutohouseKn        { get { return tbxR_toPayAutohouseKn       .GetDecimalField(); } set { tbxR_toPayAutohouseKn       .PutDecimalField(value); } }
   public decimal Fld_toPayHR_PDVkn           { get { return tbxR_toPayHR_PDVkn          .GetDecimalField(); } set { tbxR_toPayHR_PDVkn          .PutDecimalField(value); } }
   public decimal Fld_good_PDVkn              { get { return tbxR_good_PDVkn             .GetDecimalField(); } set { tbxR_good_PDVkn             .PutDecimalField(value); } }
   public decimal Fld_toPayTotalKn            { get { return tbxR_toPayTotalKn           .GetDecimalField(); } set { tbxR_toPayTotalKn           .PutDecimalField(value); } }
   public decimal Fld_toPayTotalEur           { get { return tbxR_toPayTotalEur          .GetDecimalField(); } set { tbxR_toPayTotalEur          .PutDecimalField(value); } }
   public decimal Fld_finalCostKn             { get { return tbxR_finalCostKn            .GetDecimalField(); } set { tbxR_finalCostKn            .PutDecimalField(value); } }
   public decimal Fld_finalCostEUR            { get { return tbxR_finalCostEUR           .GetDecimalField(); } set { tbxR_finalCostEUR           .PutDecimalField(value); } }
   public decimal Fld_HR_history_TOTAL_Cij_Kn { get { return tbxR_HR_history_TOTAL_Cij_Kn.GetDecimalField(); } set { tbxR_HR_history_TOTAL_Cij_Kn.PutDecimalField(value); } }
   public decimal Fld_todayTecaj              { get { return tbxR_todayTecaj             .GetDecimalField(); } set { tbxR_todayTecaj             .PutDecimalField(value); } }
   public decimal Fld_VN                      { get { return tbxR_VN                     .GetDecimalField(); } set { tbxR_VN                     .PutDecimalField(value); } }
   public decimal Fld_PC                      { get { return tbxR_PC                     .GetDecimalField(); } set { tbxR_PC                     .PutDecimalField(value); } }
   public decimal Fld_VNPC                    { get { return tbxR_VNPC                   .GetDecimalField(); } set { tbxR_VNPC                   .PutDecimalField(value); } }
   public decimal Fld_VNPCEur                 { get { return tbxR_VNPCEur                .GetDecimalField(); } set { tbxR_VNPCEur                .PutDecimalField(value); } }
   public decimal Fld_ON                      { get { return tbxR_ON                     .GetDecimalField(); } set { tbxR_ON                     .PutDecimalField(value); } }
   public decimal Fld_EN                      { get { return tbxR_EN                     .GetDecimalField(); } set { tbxR_EN                     .PutDecimalField(value); } }
   public decimal Fld_ONEN                    { get { return tbxR_ONEN                   .GetDecimalField(); } set { tbxR_ONEN                   .PutDecimalField(value); } }
   public decimal Fld_ONENEur                 { get { return tbxR_ONENEur                .GetDecimalField(); } set { tbxR_ONENEur                .PutDecimalField(value); } }
   public decimal Fld_monthAge                { get { return tbxR_monthAge               .GetDecimalField(); } set { tbxR_monthAge               .PutDecimalField(value); } }
   public decimal Fld_ageSt                   { get { return tbxR_ageSt                  .GetDecimalField(); } set { tbxR_ageSt                  .PutDecimalField(value); } }

   #endregion _Fld

   #region override PutSpecificsFld() GetSpecificsFld()

   /*protected*/
   public override void PutSpecificsFld()
   {
      Fld_TtOpis = "BMW";

      if(CtrlOK(tbx_artiklCd               )) Fld_ArtiklCd                = mixer_rec.StrC_32;
      if(CtrlOK(tbx_artiklName             )) Fld_ArtiklName              = mixer_rec.StrA_40;
      if(CtrlOK(tbx_RealMPC_EUR            )) Fld_RealMPC_EUR             = mixer_rec.MoneyA ;
      if(CtrlOK(tbx_HR_historyOsnovnaCij_Kn)) Fld_HR_historyOsnovnaCij_Kn = mixer_rec.MoneyB ;
      if(CtrlOK(tbx_HR_historyOprermaCij_Kn)) Fld_HR_historyOprermaCij_Kn = mixer_rec.MoneyC ;
      if(CtrlOK(tbx_mm1Registracije        )) Fld_mm1Registracije         = mixer_rec.IntA   ;
      if(CtrlOK(tbx_yy1Registracije        )) Fld_yy1Registracije         = mixer_rec.IntB   ;
      if(CtrlOK(tbx_co2Emisija             )) Fld_co2Emisija              = mixer_rec.MtrosCD;
      if(CtrlOK(tbx_km                     )) Fld_km                      = mixer_rec.PersonCD;
                                              Fld_IsBenzinac              = mixer_rec.IsXxx;

      if(CtrlOK(tbx_v1_tt                  )) Fld_V1_tt                   = mixer_rec.V1_tt;
      if(CtrlOK(tbx_v1_ttOpis              )) Fld_V1_ttOpis               = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V1_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v1_ttNum               )) Fld_V1_ttNum                = mixer_rec.V1_ttNum;
      if(CtrlOK(tbx_v2_tt                  )) Fld_V2_tt                   = mixer_rec.V2_tt;
      if(CtrlOK(tbx_v2_ttOpis              )) Fld_V2_ttOpis               = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V2_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v2_ttNum               )) Fld_V2_ttNum                = mixer_rec.V2_ttNum;
      if(CtrlOK(tbx_externLink1            )) Fld_ExternLink1             = mixer_rec.ExternLink1;
      if(CtrlOK(tbx_externLink2            )) Fld_ExternLink2             = mixer_rec.ExternLink2;

      Put_BMW_ResultFields(mixer_rec);
   }

   public void OnExit_Recalc(object sender, EventArgs e)
   {
      Put_BMW_ResultFields(mixer_rec);
   }

   internal BMW Create_BMW_FromMixer(Mixer _mixer_rec)
   {
      return new BMW(
               _mixer_rec.MoneyA, // RealMPC_EUR             
               _mixer_rec.MoneyB, // HR_historyOsnovnaCij_Kn 
               _mixer_rec.MoneyC, // HR_historyOprermaCij_Kn 
               _mixer_rec.IntA, // mm1Registracije         
               _mixer_rec.IntB, // yy1Registracije         
          (int)_mixer_rec.MtrosCD, // co2Emisija              
               _mixer_rec.IsXxx, // isBenzinac              
               _mixer_rec.DokDate       // buyOnDate               
               );
   }

   private void Put_BMW_ResultFields(Mixer _mixer_rec)
   {
      BMW _theBMW = Create_BMW_FromMixer(mixer_rec);

      Fld_PPMV_history            = _theBMW.PPMV_historyKn         ;
      Fld_PPMV_historyEur         = _theBMW.PPMV_historyEUR        ;
      Fld_PPMVkn                  = _theBMW.PPMV_Kn                ;
      Fld_PPMVeur                 = _theBMW.PPMV_EUR               ;
      Fld_toPayAutohouseEUR       = _theBMW.toPayAutohouseEUR      ;
    //Fld_toPayAutohouseKn        = _theBMW.toPayAutohouseKn       ;
    //Fld_toPayHR_PDVkn           = _theBMW.toPayHR_PDVkn          ;
    //Fld_good_PDVkn              = _theBMW.good_PDVkn             ;
    //Fld_toPayTotalKn            = _theBMW.toPayTotalKn           ;
    //Fld_toPayTotalEur           = _theBMW.toPayTotalEUR          ;
    //Fld_finalCostKn             = _theBMW.finalCostKn            ;
    //Fld_finalCostEUR            = _theBMW.finalCostEUR           ;
      Fld_HR_history_TOTAL_Cij_Kn = _theBMW.HR_history_TOTAL_Cij_Kn;
      Fld_todayTecaj              = _theBMW.todayTecaj             ;
      Fld_VN                      = _theBMW.VN                     ;
      Fld_PC                      = _theBMW.PC                     ;
      Fld_VNPCEur                 = _theBMW.VNPCEur                ;
      Fld_VNPC                    = _theBMW.VNPC                   ;
      Fld_ON                      = _theBMW.ON                     ;
      Fld_EN                      = _theBMW.EN                     ;
      Fld_ONEN                    = _theBMW.ONEN                   ;
      Fld_ONENEur                 = _theBMW.ONENEur                ;
      Fld_monthAge                = _theBMW.monthAge               ;
      Fld_ageSt                   = _theBMW.ageSt                  ;

      Fld_toPayAutohouseKn        =   _theBMW.IsNHR ? _theBMW.HR_history_TOTAL_Cij_Kn : _theBMW.toPayAutohouseKn;
      Fld_toPayHR_PDVkn           =   _theBMW.IsNHR ? _theBMW.NHR_toPayHR_PDVkn       : _theBMW.toPayHR_PDVkn   ;
      Fld_good_PDVkn              =   _theBMW.IsNHR ? _theBMW.NHR_good_PDVkn          : _theBMW.good_PDVkn      ;
      Fld_toPayTotalKn            =   _theBMW.IsNHR ? _theBMW.NHR_toPayTotalKn        : _theBMW.toPayTotalKn    ;
      Fld_finalCostKn             =   _theBMW.IsNHR ? _theBMW.NHR_finalCostKn         : _theBMW.finalCostKn     ;
      Fld_toPayTotalEur           =   _theBMW.IsNHR ? _theBMW.NHR_toPayTotalEUR       : _theBMW.toPayTotalEUR   ;
      Fld_finalCostEUR            =   _theBMW.IsNHR ? _theBMW.NHR_finalCostEUR        : _theBMW.finalCostEUR    ;

   }

   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_artiklCd               ))  mixer_rec.StrC_32    = Fld_ArtiklCd               ;
      if(CtrlOK(tbx_artiklName             ))  mixer_rec.StrA_40    = Fld_ArtiklName             ;
      if(CtrlOK(tbx_RealMPC_EUR            ))  mixer_rec.MoneyA     = Fld_RealMPC_EUR            ;
      if(CtrlOK(tbx_HR_historyOsnovnaCij_Kn))  mixer_rec.MoneyB     = Fld_HR_historyOsnovnaCij_Kn;
      if(CtrlOK(tbx_HR_historyOprermaCij_Kn))  mixer_rec.MoneyC     = Fld_HR_historyOprermaCij_Kn;
      if(CtrlOK(tbx_mm1Registracije        ))  mixer_rec.IntA       = Fld_mm1Registracije        ;
      if(CtrlOK(tbx_yy1Registracije        ))  mixer_rec.IntB       = Fld_yy1Registracije        ;
      if(CtrlOK(tbx_co2Emisija             ))  mixer_rec.MtrosCD    = Fld_co2Emisija             ;
      if(CtrlOK(tbx_km                     ))  mixer_rec.PersonCD   = Fld_km                     ;
                                               mixer_rec.IsXxx      = Fld_IsBenzinac             ;


      if(CtrlOK(tbx_ProjektCD              )) mixer_rec.ProjektCD   = Fld_ProjektCD;
      if(CtrlOK(tbx_v1_tt                  )) mixer_rec.V1_tt       = Fld_V1_tt;
      if(CtrlOK(tbx_v1_ttNum               )) mixer_rec.V1_ttNum    = Fld_V1_ttNum;
      if(CtrlOK(tbx_v2_tt                  )) mixer_rec.V2_tt       = Fld_V2_tt;
      if(CtrlOK(tbx_v2_ttNum               )) mixer_rec.V2_ttNum    = Fld_V2_ttNum;
      if(CtrlOK(tbx_externLink1            )) mixer_rec.ExternLink1 = Fld_ExternLink1;
      if(CtrlOK(tbx_externLink2            )) mixer_rec.ExternLink2 = Fld_ExternLink2;
   }

   internal int PutDgvLineFields_BMW(BMW theBMW/*, int rowIdx*/, bool isHighlighted, ref int highlightedRowIdx)
   {
      TheG.Rows.Add();
      int rowIdx = TheG.RowCount - /*idxCorrector*/ 1;

      TheG.ClearSelection();

      theBMW.CalcResults(); // !!! 

      TheG.PutCell(ci.iT_finalCostEUR            , rowIdx, theBMW.finalCostEUR           );
      TheG.PutCell(ci.iT_monthAge                , rowIdx, theBMW.monthAge               );
      TheG.PutCell(ci.iT_ageSt                   , rowIdx, theBMW.ageSt                  );
      TheG.PutCell(ci.iT_co2Emisija              , rowIdx, theBMW.co2Emisija             );
      TheG.PutCell(ci.iT_ONENEur                 , rowIdx, theBMW.ONENEur                );
      TheG.PutCell(ci.iT_RealMPC_EUR             , rowIdx, theBMW.RealMPC_EUR            );
      TheG.PutCell(ci.iT_HR_history_TOTAL_Cij_Kn , rowIdx, theBMW.HR_history_TOTAL_Cij_Kn);
      TheG.PutCell(ci.iT_HR_historyOsnovnaCij_Kn , rowIdx, theBMW.HR_historyOsnovnaCij_Kn);
      TheG.PutCell(ci.iT_HR_historyOprermaCij_Kn , rowIdx, theBMW.HR_historyOprermaCij_Kn);
      TheG.PutCell(ci.iT_mm1Registracije         , rowIdx, theBMW.mm1Registracije        );
      TheG.PutCell(ci.iT_yy1Registracije         , rowIdx, theBMW.yy1Registracije        );
      TheG.PutCell(ci.iT_VNPCEur                 , rowIdx, theBMW.VNPCEur                );
      TheG.PutCell(ci.iT_PPMV_historyEur         , rowIdx, theBMW.PPMV_historyEUR        );
      TheG.PutCell(ci.iT_PPMVeur                 , rowIdx, theBMW.PPMV_EUR                );
      TheG.PutCell(ci.iT_toPayAutohouseEUR       , rowIdx, theBMW.toPayAutohouseEUR      );
      TheG.PutCell(ci.iT_toPayTotalEur           , rowIdx, theBMW.toPayTotalEUR          );

      //if(isHighlighted) TheG.Rows[rowIdx].DefaultCellStyle.BackColor = Color.Pink                                      ;
      //else              TheG.Rows[rowIdx].DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;

      if(isHighlighted) highlightedRowIdx = rowIdx;

      return rowIdx;
   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region PrintDocumentRecord

   public BmwFilterUC TheBmwFilterUC { get; set; }
   public BmwFilter   TheBmwFilter   { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.TheBmwFilter = new BmwFilter(this);

      TheBmwFilterUC        = new BmwFilterUC(this);
      TheBmwFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheBmwFilterUC.Width;

   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      //BmwFilter mixerFilter = (BmwFilter)vvRptFilter;
      EvidencijaFilter mixerFilter = (EvidencijaFilter)vvRptFilter;

      //switch(mixerFilter.PrintBmw)
      switch(mixerFilter.PrintEvidencija)
      {
         //case BmwFilter.PrintBmwEnum.Bmw: specificMixerReport = new RptX_Bmw(new Vektor.Reports.XIZ.CR_Bmw_DUC(), "Bmw", mixerFilter); break;
         case EvidencijaFilter.PrintEvidencijaEnum.Evidencija: specificMixerReport = new RptX_Evidencija(new Vektor.Reports.XIZ.CR_EvidencijaDUC(), "Bmw", mixerFilter); break;

         default: ZXC.aim_emsg("{0}\nPrintSomeBmwDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintEvidencija); return null;
      }

      return specificMixerReport;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheBmwFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheBmwFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
   }


   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
    //SetUpColor(clr_PutNal, Color.Empty, clr_PutNal);
      SetUpColor(clr_PutNal, Color.Empty, clr_PutNal);
   }

   public void ColorOfColumns(string colName, int highlightedRowIdx)
   {
      VvTextBox vvtb;

      foreach(DataGridViewColumn col in TheG.Columns)
      {
         vvtb = col.Tag as VvTextBox;

         if(col != null)
         {
            if(col.Name.Contains(colName)) col.DefaultCellStyle.BackColor = Color.Pink;
            else
            {
               if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;
               else                                                           col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_False_BackColor;
            }
         }
      }

      foreach(Control ctrl in hamp_artikl.Controls)
      {
         VvTextBox tbx;

         if(ctrl is VvTextBox)
         {
            tbx = ctrl as VvTextBox;

            if(tbx.Name.Contains(colName)) tbx.BackColor = Color.Pink;
            else
            {
               if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) tbx.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;
               else                                                           tbx.BackColor = ZXC.vvColors.dataGridCellReadOnly_False_BackColor;
            }
            if(tbx.Name.Contains("finalCostEUR")) tbx.BackColor = Color.LightGreen;
         }
      }

      if(highlightedRowIdx.NotZero())
      {
         TheG.Rows[highlightedRowIdx].DefaultCellStyle.BackColor = Color.Pink;
      }

   }

   #endregion Colors

   #region overrideTabPageTitle

   public override string TabPageTitle1
   {
      get { return ""; }
   }

   #endregion overrideTabPageTitle

}

public class BmwFilterUC : VvFilterUC
{
   #region Fieldz

   #endregion Fieldz

   #region  Constructor

   public BmwFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;

      CreateHamper_4ButtonsResetGo_Width(hamper4buttons.Width);

      hamperHorLine.Visible = false;

      this.Width = hamper4buttons.Width + ZXC.QUN;
      this.Height = hamper4buttons.Bottom + ZXC.QUN;

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region Put & GetFilterFields

   private BmwFilter TheBmwFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as BmwFilter; }
      set { this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheBmwFilter = (BmwFilter)_filter_data;

      if(TheBmwFilter != null)
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

public class BmwFilter : VvRpt_Mix_Filter
{

   public enum PrintBmwEnum
   {
      Bmw
   }

   public PrintBmwEnum PrintBmw { get; set; }

   public BmwDUC theDUC;

   public BmwFilter(BmwDUC _theDUC)
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
      PrintBmw        = PrintBmwEnum.Bmw;
   }

   #endregion SetDefaultFilterValues()

}
