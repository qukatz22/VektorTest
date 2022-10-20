using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public partial class GFI_TSI_DUC : MixerDUC
{
   #region Fieldz

   public VvHamper hamp_links, hamp_sheet, hamp_upute1, hamp_upute2;
   public VvTextBox tbx_strC_32_SheetName;
      
   #endregion Fieldz

   #region Constructor

   public GFI_TSI_DUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_GFI,
            Mixer.TT_TSI,
            Mixer.TT_OPD

         });

      Is_FINA_Statistika = false;
   }
 
   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
      hamp_mjTroska.Visible = false;

      InitializeHamper_Sheet(out hamp_sheet);
      hamp_intA.Visible  = true;
      hamp_intA.Location = new Point(hamp_sheet.Right + ZXC.Qun4, hamp_sheet.Top);
      tbx_intA.MaxLength = 1;

      hamp_dateA.Visible    = true;
      hamp_dateA.Location   = new Point(hamp_sheet.Left, hamp_sheet.Bottom);

      InitializeHamper_Links(out hamp_links);
      InitializeHamper_Upute1(out hamp_upute1);
      InitializeHamper_Upute2(out hamp_upute2);

      hamp_napomena.Location = new Point(nextX, hamp_links.Bottom);
      nextY = hamp_napomena.Bottom;

      tbx_externLink1.JAM_FieldExitMethod = new EventHandler(ExternLink1Exit);
      tbx_externLink2.JAM_FieldEntryMethod = new EventHandler(ExternLink1Exit);
   }

   public void InitializeHamper_Upute1(out VvHamper hamper)
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
   public void InitializeHamper_Upute2(out VvHamper hamper)
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

   public void InitializeHamper_Sheet(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", TheTabControl.TabPages[0], false, nextX, hamp_tt.Bottom - ZXC.Qun8, razmakHamp);
      //                                     0          1      
      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q4un,  };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4,   };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }      
      hamper.VvBottomMargin = hamper.VvTopMargin;

                              hamper.CreateVvLabel  (0, 0, "SheetName:"           , ContentAlignment.MiddleRight);
      tbx_strC_32_SheetName = hamper.CreateVvTextBox(1, 0, "tbx_strC_32_SheetName", "SheetName u odabranom obrascu (npr. Bilanca, RDG, PodDop u GFI,  Podaci u TSI ...)", GetDB_ColumnSize(DB_ci.strC_32));
   }
   
   public void InitializeHamper_Links(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", TheTabControl.TabPages[0], false, hamp_tt.Left - ZXC.Q2un, hamp_tt.Bottom, razmakHamp);
      //                                     0          1                 2                  3         
      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q10un + ZXC.Q2un,  ZXC.QUN - ZXC.Qun4, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4,                   0 ,                  0 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }      
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
                        hamper.CreateVvLabel  (0, 0, "Original Excel:", ContentAlignment.MiddleRight);
      tbx_externLink1 = hamper.CreateVvTextBox(1, 0, "tbx_externLink1", "Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.externLink1));

      btn_goExLink1           = hamper.CreateVvButton(2, 0, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink1.Name      = "btn_goExLink1";
      btn_goExLink1.FlatStyle = FlatStyle.Flat;
      btn_goExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink1.Image     = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink1.Tag       = 1;
      btn_goExLink1.TabStop   = false;
      btn_goExLink1.Visible   = false;
      
      btn_openExLink1           = hamper.CreateVvButton(3, 0, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLink1.Name      = "btn_openExLink1";
      btn_openExLink1.FlatStyle = FlatStyle.Flat;
      btn_openExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLink1.Image   = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLink1.Tag     = 1;
      btn_openExLink1.TabStop = false;

                        hamper.CreateVvLabel  (0, 1, "SaveAs:", ContentAlignment.MiddleRight);
      tbx_externLink2 = hamper.CreateVvTextBox(1, 1, "tbx_externLink2", "Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.externLink2));

      btn_goExLink2           = hamper.CreateVvButton(2, 1, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink2.Name      = "btn_goExLink2";
      btn_goExLink2.FlatStyle = FlatStyle.Flat;
      btn_goExLink2.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink2.Image     = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink2.Tag       = 2;
      btn_goExLink2.TabStop   = false;
      btn_goExLink2.Visible   = false;
      
      btn_openExLink2           = hamper.CreateVvButton(3, 1, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLink2.Name      = "btn_openExLink2";
      btn_openExLink2.FlatStyle = FlatStyle.Flat;
      btn_openExLink2.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLink2.Image   = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLink2.Tag     = 2;
      btn_openExLink2.TabStop = false;


   }

   #endregion CreateSpecificHampers()

   #region Fld_

   public string Fld_StrC_32_SheetName { get { return tbx_strC_32_SheetName.Text; } set { tbx_strC_32_SheetName.Text = value; } }

   #endregion Fld_

   #region override PutSpecificsFld() GetSpecificsFld()

   /*protected*/public override void PutSpecificsFld()
   {
      Fld_StrC_32_SheetName = mixer_rec.StrC_32 ;
      Fld_ExternLink1       = mixer_rec.ExternLink1; 
      Fld_ExternLink2       = mixer_rec.ExternLink2;

      Fld_TtOpis   = ZXC.luiListaMix_GFI_TSI.GetNameForThisCd(mixer_rec.TT);
   }

   protected override void GetSpecificsFld()
   {
      mixer_rec.StrC_32     = Fld_StrC_32_SheetName;
      mixer_rec.ExternLink1 = Fld_ExternLink1;
      mixer_rec.ExternLink2 = Fld_ExternLink2;
   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_opis_128_CreateColumn      (0                    , "Naziv pozicije"   , "Naziv pozicije sa obrasca", 128, null);
      T_kpdbUlBrA_32_CreateColumn  (ZXC.Q2un             , "Cell"             , "XY pozicija u Excel tabilci", 5);
      T_intA_CreateColumn          (ZXC.Q2un             , "AOP"              , "AOP o");
      T_kpdbMjestoA_32_CreateColumn(ZXC.Q3un             , "RbrBilj"          , "Redni broj bilješke");
      T_moneyA_CreateColumn        (ZXC.Q5un             , "Prethodna God"    , "Prethodna godina (neto)", 0);
      T_moneyB_CreateColumn        (ZXC.Q5un, 0          , "Tekuća God"       , "Tekuća godina (neto)"    , true);
      T_kpdbNameA_50_CreateColumn  (ZXC.Q10un + ZXC.Q5un , "Formula / Pravilo", "Formula/Pravilo za obračun retka", false);
      T_strA_2_CreateColumn        (ZXC.QUN + ZXC.Qun2   , "Tip"              , "Tip redka: 'S'- sumarni redak u koji se ne unose pravila, 'N' - dodatni podaci koji nisu iz glavne knjige, ' '(prazno) - formula pravila za popunu pozicije u izvještaju", null);

      vvtbT_strA_2.JAM_AllowedInputCharacters = "SN";
      vvtbT_strA_2.MaxLength = 1;
      vvtbT_strA_2.JAM_CharacterCasing = CharacterCasing.Upper;
      vvtbT_kpdbUlBrA_32.JAM_CharacterCasing = CharacterCasing.Upper;

      vvtbT_moneyB.JAM_ForeColor = Color.DarkBlue;
      vvtbT_moneyB.JAM_BackColor = Color.PaleGreen;

   }

   #endregion InitializeTheGrid_Columns()

   #region PrintDocumentRecord

   public GFI_TSI_FilterUC  The_GFI_TSI_FilterUC { get; set; }
   public GFI_TSI_DocFilter The_GFI_TSI_DocFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.The_GFI_TSI_DocFilter = new GFI_TSI_DocFilter(this);

      The_GFI_TSI_FilterUC                         = new GFI_TSI_FilterUC(this);
      The_GFI_TSI_FilterUC.Parent                  = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = The_GFI_TSI_FilterUC.Width;
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      GFI_TSI_DocFilter mixerFilter = (GFI_TSI_DocFilter)vvRptFilter;

      switch(mixerFilter.Print_GFI_TSI)
      {
         case GFI_TSI_DocFilter.Print_GFI_TSIEnum.PravilaBilanca: specificMixerReport = new RptX_GFI_TSI(new Vektor.Reports.XIZ.CR_Pravila_GFI_TSI() , reportName, mixerFilter); break;
         case GFI_TSI_DocFilter.Print_GFI_TSIEnum.PrintDUC      : specificMixerReport = new RptX_GFI_TSI(new Vektor.Reports.XIZ.CR_GFI_TSI_DucPrint(), reportName, mixerFilter); break;

         default: ZXC.aim_emsg("{0}\nRasterBDocDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.Print_GFI_TSI); return null;
      }

      return specificMixerReport;

      //return new RptX_GFI_TSI(new Vektor.Reports.XIZ.CR_KontrolPravilaGFI_TSI(), reportName, mixerFilter);
      //return new RptX_GFI_TSI(new Vektor.Reports.XIZ.CR_Pravila_GFI_TSI(), reportName, mixerFilter);
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.The_GFI_TSI_DocFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.The_GFI_TSI_FilterUC;
      }
   }

   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_GFI, Color.Empty, clr_Raster);
   }

   #endregion Colors

   #region override void PutDefaultBabyDUCfields()

   protected override void PutDefaultBabyDUCfields()
   {
   }

   #endregion override void PutDefaultBabyDUCfields()

}

public class GFI_TSI_FilterUC : VvFilterUC
{
   #region Fieldz
  
   private VvHamper    hamp_rbt;
   private RadioButton rbt_bilanca, rbt_duc;

   #endregion Fieldz

   #region  Constructor

   public GFI_TSI_FilterUC(VvUserControl vvUC)
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
      hamper = new VvHamper(1, 3, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q7un +ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun2 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = ZXC.Qun4 + ZXC.Qun10;

      hamper.CreateVvLabel      (0, 0,       "Vrsta ispisa", ContentAlignment.MiddleLeft);

      rbt_bilanca = hamper.CreateVvRadioButton(0, 1, null, "Pravila + Bilanca", TextImageRelation.ImageBeforeText);
      rbt_duc     = hamper.CreateVvRadioButton(0, 2, null, "Pravila"          , TextImageRelation.ImageBeforeText);

      rbt_bilanca.Checked = true;
      rbt_bilanca.Tag = true;

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.HamperStyling(hamper);
   }
   

   #endregion Hampers

   #region Fld_

   public GFI_TSI_DocFilter.Print_GFI_TSIEnum Fld_PrintGFI_TSI
   {
      get
      {
              if(rbt_bilanca.Checked) return GFI_TSI_DocFilter.Print_GFI_TSIEnum.PravilaBilanca;
         else if(rbt_duc    .Checked) return GFI_TSI_DocFilter.Print_GFI_TSIEnum.PrintDUC      ;

              else throw new Exception("Print_GFI_TSIEnum: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case GFI_TSI_DocFilter.Print_GFI_TSIEnum.PravilaBilanca: rbt_bilanca.Checked = true; break;
            case GFI_TSI_DocFilter.Print_GFI_TSIEnum.PrintDUC      : rbt_duc    .Checked = true; break;
         }
      }
   }

   #endregion Fld_

   #region Put & GetFilterFields
  
   private GFI_TSI_DocFilter TheGFI_TSIDocFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as GFI_TSI_DocFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheGFI_TSIDocFilter = (GFI_TSI_DocFilter)_filter_data;

      if(TheGFI_TSIDocFilter != null)
      {
         Fld_PrintGFI_TSI = TheGFI_TSIDocFilter.Print_GFI_TSI;
      }

      // Za JAM_... : 
      this.ValidateChildren();

   }

   public override void GetFilterFields()
   {
      TheGFI_TSIDocFilter.Print_GFI_TSI =Fld_PrintGFI_TSI ;
   }

   #endregion Put & GetFilterFields

   #region AddFilterMemberz()
   
   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
   }

   #endregion AddFilterMemberz()

}

public class GFI_TSI_DocFilter : VvRpt_Mix_Filter
{
   public GFI_TSI_DUC theDUC;
   
   public enum Print_GFI_TSIEnum
   {
      PravilaBilanca, PrintDUC
   }

   public Print_GFI_TSIEnum Print_GFI_TSI { get; set; }

   public GFI_TSI_DocFilter(GFI_TSI_DUC _theDUC)
   {
      this.theDUC = _theDUC;
      SetDefaultFilterValues();
   }

   #region SetDefaultFilterValues()

   public override void SetDefaultFilterValues()
   {
      Print_GFI_TSI = Print_GFI_TSIEnum.PravilaBilanca;
   }

   #endregion SetDefaultFilterValues()

}
