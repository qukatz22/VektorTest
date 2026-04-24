using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public class RvrDUC : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_kupDobCd, tbx_kupDobTk, tbx_kupDobName, tbx_projektIdent;
   private VvHamper  hamp_Rvr;

   #endregion Fieldz

   #region Constructor

   public RvrDUC(Control parent, Mixer _mixer, VvSubModul vvSubModul)  : base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
        (Mixer.tt_colName, new string[] 
         { 
               Mixer.TT_RVR
         });
   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
      hamp_mjTroska.Visible = true;
      hamp_mjTroska.Location = new Point(hamp_napomena.Right, hamp_napomena.Top);

      nextY = hamp_mjTroska.Bottom;
   }


   #endregion CreateSpecificHampers()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_personCD_CreateColumn       (ZXC.Q4un, "Šifra", "Šifra");
      T_kpdbNameA_50_CreateColumn   (ZXC.Q7un, "Prezime", "Prezime", false);
      T_kpdbNameB_50_CreateColumn   (ZXC.Q6un, "Ime", "Ime");
      T_dateOd_OnlyTime_CreateColumn(ZXC.Q3un, "Početak rada");
      T_dateDo_OnlyTime_CreateColumn(ZXC.Q3un, "Završetak rada");
      T_kol_CreateColumn            (ZXC.Q3un, "Sati", "Sati", 2);
      T_strA_2_CreateColumn         (ZXC.Q2un, "RVR", "Vrsta radnog vremena", this.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.X_RVR2 ? ZXC.luiListaMixRadVrijemRVR2 : ZXC.luiListaMixRadVrijemeRVR);
      T_opis_128_CreateColumn       (0, "Vrsta radnog vremena", "Vrsta radnog vremena", 128,  null);

      vvtbT_strA_2.JAM_lui_NameTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_opis_128);
      vvtbT_opis_128.JAM_ReadOnly = true;

      vvtbT_kpdbNameA_50.JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonDgvTextBoxLeave));
      vvtbT_kpdbNameB_50.JAM_ReadOnly = true;

      vvtbT_kol.JAM_ShouldSumGrid = true;

      TheG.CellLeave += new DataGridViewCellEventHandler(TheG_CellLeave_CalcSati);
    }

   void TheG_CellLeave_CalcSati(object sender, DataGridViewCellEventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      if(TheG.CurrentCell.ColumnIndex != DgvCI.iT_dateOd &&
         TheG.CurrentCell.ColumnIndex != DgvCI.iT_dateDo) return;

      // here we go ... znaci u 'zutome' smo i u jednoj od date kolona. 

      int rIdx = TheG.CurrentRow.Index;

      DateTime dateOd = TheG.GetDateCell(DgvCI.iT_dateOd, rIdx, true);
      DateTime dateDo = TheG.GetDateCell(DgvCI.iT_dateDo, rIdx, true);

      if(dateOd == DateTime.MinValue || dateDo == DateTime.MinValue) return;

      TimeSpan ts;
      decimal brSati;

      ts = dateDo.Subtract(dateOd);
      brSati = System.Convert.ToDecimal(ts.TotalHours);

      TheG.PutCell(DgvCI.iT_kol, rIdx, brSati);
   }

   public static decimal RVRelapsedHours(DateTime dateOd, DateTime dateDo)
   {
      TimeSpan ts;
      decimal brSati;

      if(dateOd == DateTime.MinValue || dateDo == DateTime.MinValue) return 0M;
      if(dateOd ==                      dateDo                     ) return 0M;

      if(dateDo.Hour.IsZero()) return 24 - dateOd.Hour;

      ts = dateDo.Subtract(dateOd);
      brSati = System.Convert.ToDecimal(ts.TotalHours);

      return brSati;
   }

   #endregion InitializeTheGrid_Columns()

   #region Fld_

   #endregion _Fld

   #region override PutSpecificsFld() GetSpecificsFld()

   /*protected*/public override void PutSpecificsFld()
   {

      //if(CtrlOK(tbx_v1_tt      )) Fld_V1_tt       = mixer_rec.V1_tt;
      //if(CtrlOK(tbx_v1_ttOpis  )) Fld_V1_ttOpis   = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V1_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      //if(CtrlOK(tbx_v1_ttNum   )) Fld_V1_ttNum    = mixer_rec.V1_ttNum;
      //if(CtrlOK(tbx_v2_tt      )) Fld_V2_tt       = mixer_rec.V2_tt;
      //if(CtrlOK(tbx_v2_ttOpis  )) Fld_V2_ttOpis   = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V2_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      //if(CtrlOK(tbx_v2_ttNum   )) Fld_V2_ttNum    = mixer_rec.V2_ttNum;
      //if(CtrlOK(tbx_externLink1)) Fld_ExternLink1 = mixer_rec.ExternLink1;

   }

   protected override void GetSpecificsFld()
   {
      //if(CtrlOK(tbx_v1_tt      )) mixer_rec.V1_tt       = Fld_V1_tt;
      //if(CtrlOK(tbx_v1_ttNum   )) mixer_rec.V1_ttNum    = Fld_V1_ttNum;
      //if(CtrlOK(tbx_v2_tt      )) mixer_rec.V2_tt       = Fld_V2_tt;
      //if(CtrlOK(tbx_v2_ttNum   )) mixer_rec.V2_ttNum    = Fld_V2_ttNum;
      //if(CtrlOK(tbx_externLink1)) mixer_rec.ExternLink1 = Fld_ExternLink1;
   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region PrintDocumentRecord

   public RvrFilterUC TheRvrFilterUC { get; set; }
   public RvrFilter   TheRvrFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.TheRvrFilter = new RvrFilter(this);

      TheRvrFilterUC        = new RvrFilterUC(this);
      TheRvrFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheRvrFilterUC.Width;

   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      RvrFilter mixerFilter = (RvrFilter)vvRptFilter;

      switch(mixerFilter.PrintRvr)
      {
         case RvrFilter.PrintRvrEnum.RVR: specificMixerReport = new RptX_RVR(new Vektor.Reports.XIZ.CR_RVRduc(), "RVR", mixerFilter); break;

         default: ZXC.aim_emsg("{0}\nPrintSomeRvrDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintRvr); return null;
      }

      return specificMixerReport;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheRvrFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheRvrFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
   }


   #endregion PrintDocumentRecord

   #region Colors
   
   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_RVR, Color.Empty, clr_RVR);
   }

   #endregion Colors

   #region overrideTabPageTitle

   public override string TabPageTitle1
   {
      get { return "Evidencija"; }
   }

   #endregion overrideTabPageTitle

}

public class RvrFilterUC : VvFilterUC
{
   #region Fieldz

   #endregion Fieldz

   #region  Constructor

   public RvrFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;

      CreateHamper_4ButtonsResetGo_Width(hamper4buttons.Width);

      hamperHorLine.Visible = false;

      this.Width  = hamper4buttons.Width  + ZXC.QUN;
      this.Height = hamper4buttons.Bottom + ZXC.QUN;

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region Put & GetFilterFields

   private RvrFilter TheRvrFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as RvrFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheRvrFilter = (RvrFilter)_filter_data;

      if(TheRvrFilter != null)
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

public class RvrFilter : VvRpt_Mix_Filter
{

   public enum PrintRvrEnum
   {
      RVR
   }

   public PrintRvrEnum PrintRvr { get; set; }

   public RvrDUC theDUC;

   public RvrFilter(RvrDUC _theDUC)
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
      PrintRvr        = PrintRvrEnum.RVR;
   }

   #endregion SetDefaultFilterValues()

}
