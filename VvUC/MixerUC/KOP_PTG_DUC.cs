using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;


public partial class KOP_PTG_DUC : MixerDUC
{
   public PTG_Ugovor       ThePTG_Ugovor   { get; set; }
   public PTG_OtplatniPlan TheOtplatniPlan { get; set; }

   #region Fieldz

   private VvTextBox tbx_kupDobCd, tbx_kupDobTk, tbx_kupDobName, tbx_intB, tbx_KugPartner;
   private VvHamper hamp_KOP_PTG;
   private Label lblUgnOrAun;
   #endregion Fieldz

   #region Constructor

   public KOP_PTG_DUC(Control parent, Mixer _mixer, VvSubModul vvSubModul): base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
       { 
         Mixer.TT_KOP
       });

   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {

      nextX = 0;
      nextY = 0;
      hamp_tt.Visible = false;
      hamp_napomena.Visible = false;
      
      InitializeHamper_KOP_PTG   (out hamp_KOP_PTG);

      hamp_dokDate .Location = new Point(hamp_KOP_PTG.Right, hamp_KOP_PTG.Top);
      hamp_dokNum  .Location = new Point(hamp_dokDate.Right, hamp_KOP_PTG.Top);

      nextY = hamp_KOP_PTG.Bottom;

   }

   #endregion CreateSpecificHampers()

   #region hamper

   private void InitializeHamper_KOP_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(10, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
 
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun4, ZXC.Q10un + ZXC.Q4un, ZXC.Q3un, ZXC.Q3un - ZXC.Qun2, ZXC.Q9un, ZXC.QUN + ZXC.Qun12, ZXC.Q3un - ZXC.Qun2, ZXC.QUN };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8,            ZXC.Qun8,            ZXC.Qun8,             ZXC.Qun8, ZXC.Qun8,            ZXC.Qun8, ZXC.Qun8,           ZXC.Qun8 ,           ZXC.Qun12, ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;
 
      hamper.VvRowHgt       = new int[] { ZXC.QUN + ZXC.Qun8, ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] {           ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
 
                       hamper.CreateVvLabel  (0, 0, "Partner:", ContentAlignment.MiddleRight);
      tbx_kupDobCd   = hamper.CreateVvTextBox(1, 0, "tbx_kupDobCd"  , "Sifra Partnera", GetDB_ColumnSize(DB_ci.kupdobCD)  );
      tbx_kupDobTk   = hamper.CreateVvTextBox(2, 0, "tbx_kupDobTk"  , "Tiker Partnera", GetDB_ColumnSize(DB_ci.kupdobTK)  );
      tbx_kupDobName = hamper.CreateVvTextBox(3, 0, "tbx_kupDobName", "Naziv Partnera", GetDB_ColumnSize(DB_ci.kupdobName));

      tbx_kupDobCd  .JAM_ReadOnly = 
      tbx_kupDobTk  .JAM_ReadOnly = 
      tbx_kupDobName.JAM_ReadOnly = true;

      tbx_kupDobName.JAM_Highlighted = true;
      tbx_kupDobName.Font = ZXC.vvFont.LargeBoldFont;

      hamper.CreateVvLabel        (4, 0, "Tip:", ContentAlignment.MiddleRight);
      tbx_TT     = hamper.CreateVvTextBoxLookUp(5, 0, "tbx_tt", "Tip transakcije - racuna");
      tbx_TtOpis = hamper.CreateVvTextBox      (6, 0, "tbx_ttOpis", "");
      tbx_TtNum  = hamper.CreateVvTextBox      (7, 0, "tbx_ttNum", "Ovo bolje ostavi kako je...", 10, 2, 0); // !!! 

      tbx_TT    .Font = ZXC.vvFont.BaseBoldFont;
      tbx_TtOpis.Font = ZXC.vvFont.BaseBoldFont;
      tbx_TtNum .Font = ZXC.vvFont.LargeBoldFont;
 
      tbx_TT    .JAM_ReadOnly = 
      tbx_TtNum .JAM_ReadOnly = 
      tbx_TtOpis.JAM_ReadOnly = true;
 
      tbx_TtNum.TextAlign = HorizontalAlignment.Right;

      lblUgnOrAun = hamper.CreateVvLabel(6, 1, "", ContentAlignment.MiddleRight);

      //tbx_KugPartner = hamper.CreateVvTextBox(5, 1, "tbx_PTG_KugPartner", "Partner Krovnog Ugovora", 32, 1, 0);
      //tbx_KugPartner.JAM_IsSupressTab = true;
      //tbx_KugPartner.JAM_ReadOnly = true;

      tbx_v1_ttNum = hamper.CreateVvTextBox(7, 1, "tbx_v1_ttNum", "KUG num"  , GetDB_ColumnSize(DB_ci.v1_ttNum));
      tbx_v1_ttNum.JAM_ReadOnly = true;
      tbx_v2_ttNum = hamper.CreateVvTextBox(8, 1, "tbx_v2_ttNum", "UGAN num", GetDB_ColumnSize(DB_ci.v2_ttNum));
      tbx_v2_ttNum.JAM_ReadOnly = true;

      btn_v2TT = hamper.CreateVvButton(9, 1, new EventHandler(GoTo_UGAN_Dokument_Click), "");
      btn_v2TT.Name = "v2_TT";
      btn_v2TT.FlatStyle = FlatStyle.Flat;
      btn_v2TT.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_v2TT.Image = VvIco.TriangleBlue16.ToBitmap();
      btn_v2TT.Tag = 1;
      btn_v2TT.TabStop = false;

      hamper.CreateVvLabel  (0, 1, "Napomena:", ContentAlignment.MiddleRight);
      tbx_Napomena = hamper.CreateVvTextBox(1, 1, "tbx_napomena", "Napomena", GetDB_ColumnSize(DB_ci.napomena), 2, 0);
      tbx_Napomena.Font = ZXC.vvFont.SmallFont;

   }

   public void GoTo_UGAN_Dokument_Click(object sender, EventArgs e)
   {
      Point vvSubModulXY;
      string tt;

      if(Fld_V1_ttNum.IsZero())
      {
         vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_UGO_PTG);
         tt = Faktur.TT_UGN;
      }
      else
      {
         vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_ANU_PTG);
         tt = Faktur.TT_AUN;
      }

      VvTabPage existingTabPage = ZXC.TheVvForm.TheVvTabPage.TheVvForm.TheTabControl.Documents.Select(d => d.Control as VvTabPage).Where(p => p != null).FirstOrDefault(tab => tab.WriteMode == ZXC.WriteMode.None && tab.SubModul_xy == vvSubModulXY);

      Faktur linkedFaktur_rec;

      if(existingTabPage == null) linkedFaktur_rec = new Faktur();
      else linkedFaktur_rec = (Faktur)existingTabPage.TheVvDataRecord;

      bool dbOK = FakturDao.SetMeFaktur(ZXC.TheVvForm.TheDbConnection, linkedFaktur_rec, tt, (Fld_V1_ttNum * 100000) + Fld_V2_ttNum, false);

      if(dbOK == false) return;

      if(existingTabPage != null)
      {
         existingTabPage.Selected = true;

         string currTT = ((FakturDUC)(existingTabPage.TheVvDocumentRecordUC)).Fld_TT;
         uint currTTnum = ((FakturDUC)(existingTabPage.TheVvDocumentRecordUC)).Fld_TtNum;

         //if(currTT == _tt && currTTnum == _ttNum) return;

         if(dbOK)
         {
            existingTabPage.TheVvForm.PutFieldsActions(ZXC.TheVvForm.TheDbConnection, linkedFaktur_rec, existingTabPage.TheVvRecordUC);
         }
      }
      else
      {
         ZXC.TheVvForm.OpenNew_Record_TabPage(vvSubModulXY, linkedFaktur_rec.RecID);
      }

   }

   #endregion hamper

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_isFakStop_CreateColumn      (ZXC.Q2un + ZXC.Qun2, "NeFak");
      T_intA_CreateColumn           (ZXC.Q3un           , "RbrRate"   , "Redni broj rate"           );
      T_date3_CreateColumn          (ZXC.Q4un           , "Datum Rate"                              );
      T_moneyA_CreateColumn         (ZXC.Q5un           , "Iznos Rate", "Rata Iznos"     , 2        );
      T_dateOd_CreateColumn         (ZXC.Q4un           , "Datum Od"                                );
      T_dateDo_CreateColumn         (ZXC.Q4un           , "Datum Do"                                );
      T_opis_128_CreateColumn       (       0           , "Napomena"  , "Napomena"       , 128, null);
      T_date4_WithTime_CreateColumn (ZXC.Q7un- ZXC.Qun4 , "Datum korekcije"                         );
      T_kpdbNameA_50_CreateColumn   (ZXC.Q5un           , "Korigirao", "KOP napravio", false        );
      T_isXxx_CreateColumn          (ZXC.Q2un           , "Kop"                                     );
                                    
      vvtbT_intA        .JAM_ReadOnly = true;
      vvtbT_kpdbNameA_50.JAM_ReadOnly = true;

      vvtbT_moneyA.JAM_ShouldCalcTransAndSumGrid = true;

   }

   #endregion InitializeTheGrid_Columns()

   #region InitializeDUC_Specific_Columns2()

  //// protected override void InitializeDUC_Specific_Columns2()
  // {
  //    //T_2moneyA_CreateColumn   (ZXC.Q4un, "MoneyA"   , "", 2  );
  //    //T_2konto_CreateColumn    (ZXC.Q4un, "Konto"    , ""     );
  //    //T_2devValuta_CreateColumn(ZXC.Q4un, "DevValuta", "", null);
  //    //T_2opis_128_CreateColumn (0, "Opis128", "", 128);
  // }

   #endregion InitializeDUC_Specific_Columns2()

   #region Fld_

   public uint   Fld_KupDobCd      { get { return tbx_kupDobCd.GetSomeRecIDField(); } set { tbx_kupDobCd.PutSomeRecIDField(value); } }
   public string Fld_KupDobCdAsTxt { get { return tbx_kupDobCd  .Text;              } set { tbx_kupDobCd   .Text         = value ; } }
   public string Fld_KupDobName    { get { return tbx_kupDobName.Text;              } set { tbx_kupDobName .Text         = value ; } }
   public string Fld_KupDobTk      { get { return tbx_kupDobTk  .Text;              } set { tbx_kupDobTk   .Text         = value ; } }
 //public int    Fld_KUGnum        { get { return tbx_v1_ttNum.GetIntField();           } set { tbx_intB.PutIntField          (value); } }
 //public int    Fld_UGANnum       { get { return tbx_intB.GetIntField();           } set { tbx_intB.PutIntField          (value); } }
 //public string Fld_KUGpartner    { get { return tbx_KugPartner.Text;              } set { tbx_KugPartner.Text =          value ; } }

   public uint Fld_KUGnum
   {
      get { return ZXC.ValOrZero_UInt(tbx_v1_ttNum.Text); /*return  tbx_v1_ttNum.GetUintField();*/ } set { tbx_v1_ttNum.Text = value.ToString("00"); }
   }
   public uint Fld_UGANnum
   {
      get { return ZXC.ValOrZero_UInt(tbx_v2_ttNum.Text); /*return tbx_v2_ttNum.GetUintField();*/ }  set {  tbx_v2_ttNum.Text = value.ToString("00000"); }
   }


   #endregion _Fld

   #region override PutSpecificsFld() GetSpecificsFld()

   /*protected*/
   public override void PutSpecificsFld()
   {
      PutSpecificsFld(mixer_rec);
   }

   /*protected*/public override void PutSpecificsFld(Mixer mixerLocal_rec)
   {
      Fld_TtOpis = "KOREKCIJA OPL";

      if(CtrlOK(tbx_kupDobCd  )) Fld_KupDobCd   = mixer_rec.KupdobCD;
      if(CtrlOK(tbx_kupDobName)) Fld_KupDobName = mixer_rec.KupdobName;
      if(CtrlOK(tbx_kupDobTk  )) Fld_KupDobTk   = mixer_rec.KupdobTK;
      if(CtrlOK(tbx_v1_ttNum  )) Fld_KUGnum     = mixer_rec.V1_ttNum;
      if(CtrlOK(tbx_v2_ttNum  )) Fld_UGANnum    = mixer_rec.V2_ttNum;
    //if(CtrlOK(tbx_KugPartner)) Fld_KUGpartner = mixer_rec.StrF_64;

      TheVvTabPage.Fld_Col1 = "KOP-" +  mixer_rec.TtNum.ToString("0000000");
      TheVvTabPage.Fld_Col3 = mixer_rec.KupdobName;

      lblUgnOrAun.Text = Fld_KUGnum.IsZero() ? "UgovorBr:" : "AneksBr:";

   }

   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd  )) mixer_rec.KupdobCD   = Fld_KupDobCd  ;
      if(CtrlOK(tbx_kupDobName)) mixer_rec.KupdobName = Fld_KupDobName;
      if(CtrlOK(tbx_kupDobTk  )) mixer_rec.KupdobTK   = Fld_KupDobTk  ;
      if(CtrlOK(tbx_v1_ttNum  )) mixer_rec.V1_ttNum   = Fld_KUGnum    ;
      if(CtrlOK(tbx_v2_ttNum  )) mixer_rec.V2_ttNum   = Fld_UGANnum   ;
    //if(CtrlOK(tbx_KugPartner)) mixer_rec.StrF_64    = Fld_KUGpartner;

      //mixer_rec.V1_ttNum = mixer_rec.TtNum;
      //mixer_rec.V2_ttNum = mixer_rec.TtNum;
      //mixer_rec.V2_ttNum = ZXC.GetDesnoOdZareza_asUint(mixer_rec.TtNum, 100000);
   }

   public override void PutDgvTransSumFields()
   {
    //TheSumGrid[ci.iT_moneyA, 0].Value = mixer_rec.Sum_Money1;

      TheSumGrid.PutCell(ci.iT_moneyA, 0, SumOfRataMoneyColumn());
   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region PrintDocumentRecord

   //public PredNrdFilterUC ThePredNrdFilterUC { get; set; }
   //public PredNrdFilter   ThePredNrdFilter { get; set; }
   //
   //protected override void CreateMixerDokumentPrintUC()
   //{
   // //this.ThePredNrdFilter = new PredNrdFilter(this);
   // //
   // //ThePredNrdFilterUC        = new PredNrdFilterUC(this);
   // //ThePredNrdFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
   // //ThePanelForFilterUC_PrintTemplateUC.Width = ThePredNrdFilterUC.Width;
   //
   //}
   //
   //public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   //{
   //   //PredNrdFilter mixerFilter = (PredNrdFilter)vvRptFilter;
   //
   //   //switch(mixerFilter.PrintPrNRD)
   //   //{
   //   //   case PredNrdFilter.PrintPrNRDEnum.PrNRD: specificMixerReport = new RptX_PredNrd(new Vektor.Reports.XIZ.CR_NazlogZaIzradu(), "NALOG ZA IZRADU", mixerFilter); break;
   //      
   //   //   default: ZXC.aim_emsg("{0}\nPrintSomeDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintPrNRD); return null;
   //   //}
   //
   //   return null /*specificMixerReport*/;
   //}
   //
   //public override VvRptFilter VirtualRptFilter
   //{
   //   get
   //   {
   //      return this.ThePredNrdFilter;
   //   }
   //}
   //
   //public override VvFilterUC VirtualFilterUC
   //{
   //   get
   //   {
   //      return this.ThePredNrdFilterUC;
   //   }
   //}
   //
   //public override void SetFilterRecordDependentDefaults()
   //{
   //}
   //
   //
   #endregion PrintDocumentRecord

   #region Colors

   public void DGV_PTG_HasKOP_CellValueChangedColor()
   {
      VvDataGridView dgv = TheG;

      for(int i = 0; i < dgv.Rows.Count; i++)
      {
         for(int j = 0; j < dgv.Columns.Count; j++) // idemo od 3 pod pretpostavkom da je NeFak prva kolona nakon recId-a i 
         {
            if(dgv.Rows[i].Cells[ci.iT_intB ].Value != null && dgv.Rows[i].Cells[ci.iT_intB ].Value is true &&
               dgv.Rows[i].Cells[ci.iT_isXxx].Value != null && dgv.Rows[i].Cells[ci.iT_isXxx].Value.ToString() == "X")//isFakStop
            {
               dgv.Rows[i].Cells[j].Style.BackColor = Color.FromArgb(255, 164, 164);
             //dgv.Rows[i].Cells[j].Style.BackColor = Color.FromArgb(164, 255, 164);

               
            }
            else if(dgv.Rows[i].Cells[ci.iT_intB ].Value != null && dgv.Rows[i].Cells[ci.iT_intB ].Value is false &&
                    dgv.Rows[i].Cells[ci.iT_isXxx].Value != null && dgv.Rows[i].Cells[ci.iT_isXxx].Value.ToString() == "X")//only KOP
            {
               dgv.Rows[i].Cells[j].Style.BackColor = Color.FromArgb(250, 227, 227);
            }
         }
      }
   }

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_KOP_PTG, Color.Empty, clr_KOP_PTG);
   }

   #endregion Colors

   #region overrideTabPageTitle

   public override string TabPageTitle1
   {
      get { return ptgKop_TabPageName; }
   }

   //public override string TabPageTitle2
   //{
   //   get { return ptgKop_2_TabPageName; }
   //}

   #endregion overrideTabPageTitle

   internal decimal SumOfRataMoneyColumn()
   {
      decimal sumOfRataMoneyColumn = 
         
         TheG.Rows.Cast<DataGridViewRow>()
            .Sum(row => TheG.GetDecimalCell(ci.iT_moneyA, row.Index, false));

      return sumOfRataMoneyColumn;
   }

}
