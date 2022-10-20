using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public partial class Statistika_NPF_DUC : /*MixerDUC*/GFI_TSI_DUC
{
   #region Fieldz

   private VvHamper  hamp_upute1, hamp_upute2;
   
   #endregion Fieldz

   #region Constructor

   public Statistika_NPF_DUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul)
      : base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_NPF_BIL,
            Mixer.TT_NPF_SPR,
            Mixer.TT_NPF_PPR,
            Mixer.TT_NPF_GPR

         });

      Is_FINA_Statistika = true;
      tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaMix_Statist_NPF, (int)ZXC.Kolona.prva);
   }

   #endregion Constructor

   #region CreateSpecificHampers()

   #endregion CreateSpecificHampers()

   #region Fld_

   #endregion Fld_

   #region override PutSpecificsFld() GetSpecificsFld()

   /*protected*/public override void PutSpecificsFld()
   {
      Fld_StrC_32_SheetName = mixer_rec.StrC_32 ;
      Fld_ExternLink1       = mixer_rec.ExternLink1; 
      Fld_ExternLink2       = mixer_rec.ExternLink2;
 
      Fld_TtOpis = ZXC.luiListaMix_Statist_NPF.GetNameForThisCd(mixer_rec.TT);
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
      T_opis_128_CreateColumn      (0,                    "Naziv pozicije"   , "Naziv pozicije sa obrasca", 128, null);
      T_kpdbUlBrA_32_CreateColumn  (ZXC.Q2un,             "Cell"             , "XY pozicija u Excel tabilci", 5);
      T_intA_CreateColumn          (ZXC.Q2un,             "AOP"              , "AOP o");
      T_moneyA_CreateColumn        (ZXC.Q5un,             "Prethodna God"    , "Prethodna godina", 0);
      T_moneyB_CreateColumn        (ZXC.Q5un, 0,          "Tekuća God"       , "Tekuća godina", true);
      T_kpdbNameA_50_CreateColumn  (ZXC.Q10un + ZXC.Q5un, "Formula / Pravilo", "Formula/Pravilo za obračun retka", false);
      T_strA_2_CreateColumn        (ZXC.QUN + ZXC.Qun2,   "Tip"              , "Tip redka: 'S'- sumarni redak u koji se ne unose pravila, 'N' - dodatni podaci koji nisu iz glavne knjige, 'O'- redak naslova poglavlja,' '(prazno) - formula pravila za popunu pozicije u izvještaju", null);

      vvtbT_strA_2.JAM_AllowedInputCharacters = "SNO";
      vvtbT_strA_2.MaxLength = 1;
      vvtbT_strA_2.JAM_CharacterCasing = CharacterCasing.Upper;
      vvtbT_kpdbUlBrA_32.JAM_CharacterCasing = CharacterCasing.Upper;

      vvtbT_moneyB.JAM_ForeColor = Color.DarkBlue;
      vvtbT_moneyB.JAM_BackColor = Color.PaleGreen;

   }

   #endregion InitializeTheGrid_Columns()

   #region PrintDocumentRecord

   public Statistika_NPF_FilterUC The_Statistika_NPF_FilterUC { get; set; }
   public Statistika_NPF_DocFilter The_Statistika_NPF_DocFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.The_Statistika_NPF_DocFilter = new Statistika_NPF_DocFilter(this);

      The_Statistika_NPF_FilterUC = new Statistika_NPF_FilterUC(this);
      The_Statistika_NPF_FilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = The_Statistika_NPF_FilterUC.Width;
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      Statistika_NPF_DocFilter mixerFilter = (Statistika_NPF_DocFilter)vvRptFilter;

      switch(mixerFilter.Print_Statistika_NPF)
      {
         case Statistika_NPF_DocFilter.Print_Statistika_NPFEnum.BIL_NPF     : specificMixerReport = new RptX_STAT_NPF(new Vektor.Reports.XIZ.CR_StatistikaNPF(), reportName, mixerFilter); break;
         case Statistika_NPF_DocFilter.Print_Statistika_NPFEnum.S_PR_RAS_NPF: specificMixerReport = new RptX_STAT_NPF(new Vektor.Reports.XIZ.CR_StatistikaNPF(), reportName, mixerFilter); break;
         case Statistika_NPF_DocFilter.Print_Statistika_NPFEnum.PR_RAS_NPF  : specificMixerReport = new RptX_STAT_NPF(new Vektor.Reports.XIZ.CR_StatistikaNPF(), reportName, mixerFilter); break;
         case Statistika_NPF_DocFilter.Print_Statistika_NPFEnum.G_PR_IZ_NPF : specificMixerReport = new RptX_STAT_NPF(new Vektor.Reports.XIZ.CR_StatistikaNPF(), reportName, mixerFilter); break;

         default: ZXC.aim_emsg("{0}\nRasterBDocDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.Print_Statistika_NPF); return null;
      }

      return specificMixerReport;

      //return new RptX_Statistika_NPF(new Vektor.Reports.XIZ.CR_KontrolPravilaStatistika_NPF(), reportName, mixerFilter);
      //return new RptX_Statistika_NPF(new Vektor.Reports.XIZ.CR_Pravila_Statistika_NPF(), reportName, mixerFilter);
   }


   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.The_Statistika_NPF_DocFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.The_Statistika_NPF_FilterUC;
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

   public void Load_STAT_NPF_FromLookUpList(VvLookUpLista theLookUpList)
   {
      Mixer  mixer_rec  = new Mixer ();
      Xtrans xtrans_rec = new Xtrans();

      ushort line = 0;

      switch(theLookUpList.Title)
      {                                                                       // sheetName                      offset                 
         case ZXC.luiLista_PR_RAS_NPF_Name  : mixer_rec.TT = Mixer.TT_NPF_PPR; mixer_rec.StrC_32 = "Obrazac" ;mixer_rec.IntA = 2; break;
         case ZXC.luiLista_S_PR_RAS_NPF_Name: mixer_rec.TT = Mixer.TT_NPF_SPR; mixer_rec.StrC_32 = "Obrazac" ;mixer_rec.IntA = 2; break;  
         case ZXC.luiLista_BIL_NPF_Name     : mixer_rec.TT = Mixer.TT_NPF_BIL; mixer_rec.StrC_32 = "BIL"     ;mixer_rec.IntA = 2; break;  
         case ZXC.luiLista_G_PR_IZ_NPF_Name : mixer_rec.TT = Mixer.TT_NPF_GPR; mixer_rec.StrC_32 = ""        ;mixer_rec.IntA = 3; break;  
      }

      mixer_rec.DokDate = DateTime.Now;
      mixer_rec.Napomena = mixer_rec.StrC_32;
      
      foreach(VvLookUpItem lui in theLookUpList)
      {
         xtrans_rec.Memset0(0);

         string pravilo = "";

         switch(lui.Number.ToString0Vv())
         {                                                                        
            case "0": pravilo =  ""; break; // saldo konta
            case "1": pravilo = "D"; break; // kumulativ duguje konta
            case "2": pravilo = "P"; break; // kumulativ potrazuje konta
            case "3": pravilo = "d"; break; // promet duguje
            case "4": pravilo = "p"; break; // promet potrazuje
            case "5": pravilo =  ""; break; // podatak nije iz glavne knjige
            case "6": pravilo =  ""; break; // naslov ?

            case "9": pravilo = "x"; break; // suma koja ovisi o 'višoj' sume (jer je xtransList.Reverse()), pa ju treba naknadno resumirati 
            case "8": pravilo = "y"; break; // suma koja ovisi o 'x' sumi koja je prije ove faze morala biti KillNegativeTwins() 

            default:  pravilo =  ""; break; // po defaultu je uvijek saldo
         }
        

         string konto = lui.Integer.IsNegative() ? "0" + (lui.Integer * -1).ToString() : lui.Integer.ToString(); 

         xtrans_rec.T_kpdbUlBrA_32 = lui.Cd     ;                                                 // Cell XY - 
         xtrans_rec.T_kpdbNameA_50 = konto + pravilo ;                                            // Pravilo = konto + eventalno pravilo iz luiliste    - 
         xtrans_rec.T_opis_128     = ZXC.LenLimitedStr(lui.Name, 128);                            // NazivPozicije - 
         xtrans_rec.T_strA_2       = lui.Number < 5.00M ? "" : lui.Number == 5.00M ? "N" : "O"  ; // Tip     - 
         xtrans_rec.T_intA         = (int)lui.Uinteger;                                           // AOP

         xtrans_rec.T_strA_2 = lui.Flag == true ? "S" : xtrans_rec.T_strA_2;

         MixerDao.AutoSetMixer(TheDbConnection, ref line, mixer_rec, xtrans_rec);
      }

   }

}

public class Statistika_NPF_FilterUC : VvFilterUC
{
   #region Fieldz

   #endregion Fieldz

   #region  Constructor

   public Statistika_NPF_FilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;

      CreateHamper_4ButtonsResetGo_Width(ZXC.Q4un);

      hamperHorLine.Visible = false;

      this.Width  = hamper4buttons.Width + ZXC.Qun2;
      this.Height = hamper4buttons.Bottom + ZXC.QUN;

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region Hampers

   #endregion Hampers

   #region Fld_

   //public Statistika_NPF_DocFilter.Print_Statistika_NPFEnum Fld_PrintStatistika_NPF
   //{
   //   get
   //   {
   //      if(rbt_bilanca.Checked) return Statistika_NPF_DocFilter.Print_Statistika_NPFEnum.PravilaBilanca;
         
   //   }
   //   set
   //   {
   //      switch(value)
   //      {
   //         case Statistika_NPF_DocFilter.Print_Statistika_NPFEnum.PravilaBilanca: rbt_bilanca.Checked = true; break;
   //         case Statistika_NPF_DocFilter.Print_Statistika_NPFEnum.PrintDUC: rbt_duc.Checked = true; break;
   //      }
   //   }
   //}

   #endregion Fld_

   #region Put & GetFilterFields

   private Statistika_NPF_DocFilter TheStatistika_NPFDocFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as Statistika_NPF_DocFilter; }
      set { this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheStatistika_NPFDocFilter = (Statistika_NPF_DocFilter)_filter_data;

      if(TheStatistika_NPFDocFilter != null)
      {
        // Fld_PrintStatistika_NPF = TheStatistika_NPFDocFilter.Print_Statistika_NPF;
      }

      // Za JAM_... : 
      this.ValidateChildren();

   }

   public override void GetFilterFields()
   {
    //  TheStatistika_NPFDocFilter.Print_Statistika_NPF = Fld_PrintStatistika_NPF;
   }

   #endregion Put & GetFilterFields

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
   }

   #endregion AddFilterMemberz()

}

public class Statistika_NPF_DocFilter : VvRpt_Mix_Filter
{
   public Statistika_NPF_DUC theDUC;

   public enum Print_Statistika_NPFEnum
   {
     PR_RAS_NPF, S_PR_RAS_NPF, BIL_NPF, G_PR_IZ_NPF 
   }

   public Print_Statistika_NPFEnum Print_Statistika_NPF { get; set; }

   public Statistika_NPF_DocFilter(Statistika_NPF_DUC _theDUC)
   {
      this.theDUC = _theDUC;
      SetDefaultFilterValues();
   }

   #region SetDefaultFilterValues()

   public override void SetDefaultFilterValues()
   {
     // Print_Statistika_NPF = Print_Statistika_NPFEnum.PravilaBilanca;
   }

   #endregion SetDefaultFilterValues()



}
