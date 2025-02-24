using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Collections.Generic;

public class FakturListUC : /*VvRecLstUC*/VvDocumRecLstUC
{
   #region Fieldz

   protected Faktur faktur_rec;

   public RadioButton rbtSortByTtNum, rbtSortByDokDate, rbtSortByPartner, rBtcurrChecked;

   public VvTextBox  tbx_dokDate, tbx_Partner, tbx_ttNum, tbx_TT,
                     tbx_filtPartnerName, tbx_filtPartnerCD, tbx_filtPartnerTick,
                     tbx_filterTT, tbx_filterSkladCd, tbx_filterSkladOpis, tbx_filterNapomena, tbx_filterVezniDok,
                     tbx_filterMtrosCD, tbx_filterMtrosTK, tbx_filterMtrosName,
                     tbx_filterPrimPlatCD, tbx_filterPrimPlatTK, tbx_filterPrimPlatName,
                     tbx_filterNapomena2,
                     tbx_filterOpis, 
                     tbx_filterPosJedCd, tbx_filterPosJedTk, tbx_filterPosJedName,
                     tbx_filterV_tt, tbx_filterV_ttNum, tbx_filterV_ttOpis,
                     tbx_filterCjenikTT, tbx_filterCjenikTTnum,
                     tbx_filterProjekt, 
                     tbx_filterStatus,
                     tbx_filterValName,
                     tbx_filterNacPlac,
                     tbx_filterTipOtpreme,
                     tbx_filterPersonAName, tbx_filterPersonAcd,
                     tbx_filterPersonBName,
                     tbx_filterOpciAvalue,
                     tbx_filterOpciBvalue,
                     tbx_filterDostName, tbx_filterDostAddr,
                     tbx_filterVezniDok2, tbx_filterFco,
                     tbx_filterPdvKnjiga, tbx_filterPdvR12,

                     tbx_filterKdOib, tbx_filterKupdobUlica, tbx_filterKupdobMjesto,
                     tbx_filterZiroRn,
                     tbx_Svd_PersonAName2,
                     tbx_Svd_OpciAlabel        ,
                     tbx_Svd_OpciAvalue;

   public CheckBox cbx_biloGdjeUnazivu, cbx_biloGdjeUnapomeni, cbx_biloGdjeUnapomeni2, cbx_biloGdjeUopisu;

   public VvDateTimePicker dtp_dokDate;

   public string Default_TT { get; set; }
   
   #endregion Fieldz

   #region Constructor

   public FakturListUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul, VvForm.VvSubModul vvMasterSubModul) : base(parent)
   {
      this.faktur_rec = _faktur;

      this.MasterSubModulEnum = vvMasterSubModul.subModulEnum;
      this.TheSubModul        = vvSubModul;
      this.Parent.Text        = this.TheSubModul.subModul_name;


    //if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_PRI_bc || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_PRIdev || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_PRI_P)
      if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_PRI_bc || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_PRIdev || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_PRI_P || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_PRI_PTG)
         this.Default_TT = Faktur.TT_PRI;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_UFAdev)
         this.Default_TT = Faktur.TT_UFA;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_IFAdev)
         this.Default_TT = Faktur.TT_IFA;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_URM_2 || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_URM_D)
         this.Default_TT = Faktur.TT_URM;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_KLK_2 || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_KLD)
         this.Default_TT = Faktur.TT_KLK;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_IRM_2)
         this.Default_TT = Faktur.TT_IRM;
    //else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_MSI_2)
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_MSI_2 || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_MSI_PTG)
         this.Default_TT = Faktur.TT_MSI;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_MVI_2)
         this.Default_TT = Faktur.TT_MVI;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_VMI_2)
         this.Default_TT = Faktur.TT_VMI;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_IRP)
         this.Default_TT = Faktur.TT_IRA;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_URP || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_URA_SVD || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_URA_PTG)
         this.Default_TT = Faktur.TT_URA;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_PIZ_P)
         this.Default_TT = Faktur.TT_PIX;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_IZM || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_IZM_2)
         this.Default_TT = Faktur.TT_IZM;
    //else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_IZD || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_IZD_SVD)
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_IZD || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_IZD_SVD || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_IZD_PTG)
         this.Default_TT = Faktur.TT_IZD;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_NRD || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_NRD_SVD)
         this.Default_TT = Faktur.TT_NRD;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_ZAH_SVD)
         this.Default_TT = Faktur.TT_ZAH;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_PST || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_PST_PTG)
         this.Default_TT = Faktur.TT_PST;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_PON_MPC)
         this.Default_TT = Faktur.TT_PON;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_IRA_MPC || this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_IRA_PTG)
         this.Default_TT = Faktur.TT_IRA;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_IZD_MPC)
         this.Default_TT = Faktur.TT_IZD;
      else if(this.MasterSubModulEnum == ZXC.VvSubModulEnum.R_OPN_MPC)
         this.Default_TT = Faktur.TT_OPN;
      else
         this.Default_TT = vvMasterSubModul.subModul_shortName;

      TheFilterMembers = new System.Collections.Generic.List<VvSqlFilterMember>(); // namoj tamo di ne treba (npr Kplan) 

      Faktur.sorterKpdbName.BiloGdjeU_Tekstu = Fld_BiloGdjeKupDobName = ZXC.TheVvForm.VvPref.findFakturKupdob.IsBiloGdjeUnazivu;
      if(recordSorter.SortType == Faktur.sorterKpdbName.SortType) recordSorter.BiloGdjeU_Tekstu = Faktur.sorterKpdbName.BiloGdjeU_Tekstu;

      Supress_ImaLiIjedan_StartField_Neprazan_Action = true;
      SelectFirstSortRadioButton(tbx_ttNum, tbx_TT, Default_TT);

      rbtSortByTtNum  .Checked = ZXC.TheVvForm.VvPref.findFaktur.IsFindBy_ttNum   ;
      rbtSortByDokDate.Checked = ZXC.TheVvForm.VvPref.findFaktur.IsFindBy_dokDate ;
      rbtSortByPartner.Checked = ZXC.TheVvForm.VvPref.findFaktur.IsFindBy_kpdbName;

      SetControlForInitialFocus();

      DaLiJeParentFindDialog();
   }

   protected override void InitializeFindFormSpecifics()
   {
      recordSorter = Faktur.sorterTtNum;

      this.ds_faktur = new Vektor.DataLayer.DS_FindRecord.DS_findFaktur();

      this.Name = "FakturListUC";
      this.Text = "FakturListUC";
   }

   #endregion Constructor

   #region FindDialog

   private void DaLiJeParentFindDialog()
   {
      if(this.Parent is VvFindDialog &&
         // 31.5.2011:
         (ZXC.TheVvForm.TheVvUC is VirmanDUC) == false
         )
      {
         tbx_filterTT.Text = Default_TT;

         if(Default_TT == Faktur.TT_RNP && (ZXC.TheVvForm.TheVvUC is RNPDUC) == false)
         {
            tbx_TT.JAM_ReadOnly = false;
            tbx_filterTT.JAM_ReadOnly = false;
            VvHamper.Open_Close_Fields_ForWriting(tbx_TT, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
            VvHamper.Open_Close_Fields_ForWriting(tbx_filterTT, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
         }
         else if(Default_TT == Faktur.TT_WRN)
         {
            tbx_TT.JAM_ReadOnly = false;
            tbx_filterTT.JAM_ReadOnly = false;
            VvHamper.Open_Close_Fields_ForWriting(tbx_TT, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
            VvHamper.Open_Close_Fields_ForWriting(tbx_filterTT, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

            tbx_TT.Text = tbx_filterTT.Text = "YRN";
         }
         else if(Default_TT == Faktur.TT_CJ_VP1) OtvoriZatvoriTtOvisnoOtt(false);
         else OtvoriZatvoriTtOvisnoOtt(true);

         //06.10.2021.
         if(ZXC.IsSvDUH_ZAHonly && (Default_TT == Faktur.TT_ZAH || Default_TT == Faktur.TT_IZD))
         {
            Kupdob kupdob_rec = this.Get_Kupdob_FromVvUcSifrar_byTicker(ZXC.CURR_userName);

            tbx_filtPartnerTick.Text = ZXC.CURR_userName;
            tbx_filtPartnerCD  .Text = kupdob_rec.KupdobCD.ToString("000000");
            tbx_filtPartnerName.Text = kupdob_rec.Naziv;

            VvHamper.Open_Close_Fields_ForWriting(tbx_filtPartnerTick, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
            VvHamper.Open_Close_Fields_ForWriting(tbx_filtPartnerCD  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
            VvHamper.Open_Close_Fields_ForWriting(tbx_filtPartnerName, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);

         }
      }
      else
      {
         tbx_filterTT.Text = "";
         tbx_TT      .Text = "";
         tbx_TT      .JAM_ReadOnly = false;
         tbx_filterTT.JAM_ReadOnly = false;
         VvHamper.Open_Close_Fields_ForWriting(tbx_TT      , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
         VvHamper.Open_Close_Fields_ForWriting(tbx_filterTT, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      }
   }
  
   private void OtvoriZatvoriTtOvisnoOtt(bool readOnly)
   {
      tbx_TT      .JAM_ReadOnly = readOnly;
      tbx_filterTT.JAM_ReadOnly = readOnly;

      ZXC.ZaUpis zaUpisOtvZat;
      if(readOnly) zaUpisOtvZat = ZXC.ZaUpis.Zatvoreno;
      else         zaUpisOtvZat = ZXC.ZaUpis.Otvoreno;

      VvHamper.Open_Close_Fields_ForWriting(tbx_TT      , zaUpisOtvZat, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_filterTT, zaUpisOtvZat, ZXC.ParentControlKind.VvFindDialog);

      if(!readOnly)
      {
         tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaRiskCjenik, (int)ZXC.Kolona.prva);
         tbx_filterTT.JAM_Set_LookUpTable(ZXC.luiListaRiskCjenik, (int)ZXC.Kolona.prva);
      }

   }
   #endregion FindDialog
 
   #region Hamperi

   private void SetControlForInitialFocus()
   {
      if(rbtSortByTtNum.Checked)
      {
         this.ControlForInitialFocus = tbx_ttNum;
         rbtSortByTtNum_Click(rbtSortByTtNum, EventArgs.Empty);
      }
      if(rbtSortByDokDate.Checked)
      {
         this.ControlForInitialFocus = tbx_dokDate;
         rbtSortByDokDate_Click(rbtSortByDokDate, EventArgs.Empty);
      }
      if(rbtSortByPartner.Checked)
      {
         this.ControlForInitialFocus = tbx_Partner;
         rbtSortByPartner_Click(rbtSortByPartner, EventArgs.Empty);
      }
   }

   protected override void CreateHamperSpecifikum()
   {
      hampSpecifikum = new VvHamper(8, 2, "", this, true, hampListaRastePada.Right + ZXC.Qun4, nextY, razmakHamp);

      hampSpecifikum.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q3un, ZXC.Q3un,          ZXC.Q5un, ZXC.Q4un,          ZXC.Q5un, ZXC.Q8un, ZXC.QUN - ZXC.Qun8 };
      hampSpecifikum.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun2+ZXC.Qun8, ZXC.Qun4, ZXC.Qun2+ZXC.Qun8, ZXC.Qun8,          ZXC.Qun12 };
      hampSpecifikum.VvRightMargin = hampSpecifikum.VvLeftMargin;

      hampSpecifikum.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN  };
      hampSpecifikum.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hampSpecifikum.VvBottomMargin = hampSpecifikum.VvTopMargin;


      tbx_TT = hampSpecifikum.CreateVvTextBoxLookUp(1, 0, "tbx_TT", "");
      tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);

      if(this.Parent is VvFindDialog) tbx_TT.JAM_ReadOnly = true;

      rbtSortByTtNum     = hampSpecifikum.CreateVvRadioButton(0, 0, new EventHandler(rbtSortByTtNum_Click), "Od dokumenta:", TextImageRelation.ImageAboveText);
      tbx_ttNum          = hampSpecifikum.CreateVvTextBox    (2, 0, "tbx_ttNum", "");
      tbx_ttNum.Tag      = rbtSortByTtNum;
      tbx_TT.Tag   = rbtSortByTtNum;
      rbtSortByTtNum.Tag = tbx_ttNum;
      tbx_ttNum.DoubleClick  += new EventHandler(tbx_DoubleClick);
      tbx_ttNum.JAM_ValueType = typeof(int);
      tbx_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_TT.JAM_CharacterCasing = CharacterCasing.Upper;

      rbtSortByDokDate     = hampSpecifikum.CreateVvRadioButton(3, 0, new EventHandler(rbtSortByDokDate_Click), "Od datuma:", TextImageRelation.ImageAboveText);
      tbx_dokDate          = hampSpecifikum.CreateVvTextBox    (4, 0, "tbx_dokDate", "");
      rbtSortByDokDate.Tag = tbx_dokDate;
      tbx_dokDate.JAM_IsForDateTimePicker = true;

      dtp_dokDate      = hampSpecifikum.CreateVvDateTimePicker(4, 0, "", tbx_dokDate);
      dtp_dokDate.Name = "dtp_dokDate";
      dtp_dokDate.Tag  = rbtSortByDokDate;

      tbx_dokDate.DoubleClick += new EventHandler(tbx_DoubleClick);

      rbtSortByPartner        = hampSpecifikum.CreateVvRadioButton(5, 0, new EventHandler(rbtSortByPartner_Click), "Od partnera:", TextImageRelation.ImageAboveText);
      tbx_Partner             = hampSpecifikum.CreateVvTextBox    (6, 0, "tbx_Partner", "", 32);
      tbx_Partner.Tag         = rbtSortByPartner;
      rbtSortByPartner.Tag    = tbx_Partner;
      tbx_Partner.DoubleClick += new EventHandler(tbx_DoubleClick);

      tbx_Partner.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

      cbx_biloGdjeUnazivu = hampSpecifikum.CreateVvCheckBox_OLD(7, 0, CheckBox_biloGdjeUnazivu_Click, "", RightToLeft.No, true);
      SetVvPrefLink_AND_ToolTipText_ForGenericBiloGdjeUnazivuCheckBox(cbx_biloGdjeUnazivu, new EventHandler(cbx_biloGdjeUnazivu_Click_SaveToVvPref));
      
                            hampSpecifikum.CreateVvLabel        (0, 1, "Skladište:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterSkladCd   = hampSpecifikum.CreateVvTextBoxLookUp(1, 1, "tbx_filterSkladCd", "Skladište");
      tbx_filterSkladOpis = hampSpecifikum.CreateVvTextBox      (2, 1, "tbx_filterSkladOpiS", "", 36, 1, 0);
      tbx_filterSkladCd.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_filterSkladOpis.JAM_ReadOnly  = true;
      tbx_filterSkladCd.JAM_WriteOnly = true;

      tbx_filterSkladCd.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_filterSkladCd.JAM_lui_NameTaker_JAM_Name = tbx_filterSkladOpis.JAM_Name;


      VvHamper.Open_Close_Fields_ForWriting(tbx_TT           , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_ttNum        , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog); 
      VvHamper.Open_Close_Fields_ForWriting(tbx_dokDate      , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_Partner      , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_filterSkladCd, ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvFindDialog);

   }

   protected override void CreateHamperFilter()
   {
      CreateHamperOpenFilter();

      bool isSvd = ZXC.IsSvDUH;

      hampFilter = new VvHamper(11, 10, "", this, true, hampOpenFilter.Left, hampOpenFilter.Top, razmakHamp);

      hampFilter.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un, ZXC.Q3un, ZXC.Q6un, ZXC.QUN - ZXC.Qun4, ZXC.Q4un, ZXC.Q3un - ZXC.Qun4, ZXC.Q5un, ZXC.Q4un, ZXC.Q3un  , ZXC.Q3un};
      hampFilter.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,           ZXC.Qun12, ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 , ZXC.Qun4};
      hampFilter.VvRightMargin = hampFilter.VvLeftMargin;

      for(int i = 0; i < hampFilter.VvNumOfRows; i++)
      {
         hampFilter.VvRowHgt[i]    = ZXC.QUN;
         hampFilter.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hampFilter.VvBottomMargin = hampFilter.VvTopMargin;

      Label lblFilter  = hampFilter.CreateVvLabel (0, 0, "Izlistaj samo one koji se odnose na:", 2, 0, System.Drawing.ContentAlignment.MiddleRight);
    
                             hampFilter.CreateVvLabel  (0, 1, "Partnera:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filtPartnerCD    = hampFilter.CreateVvTextBox(1, 1, "tbx_filtPartnerCd"  , "Šifra partnera" );
      tbx_filtPartnerTick  = hampFilter.CreateVvTextBox(2, 1, "tbx_filtPartnerTick", "Ticker partnera");
      tbx_filtPartnerName  = hampFilter.CreateVvTextBox(3, 1, "tbx_filtPartnerName", "Naziv partnera" );

      tbx_filtPartnerCD.JAM_CharEdits         = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_filtPartnerTick.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_filtPartnerCD  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD   .SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyKupDobTextBoxLeave));
      tbx_filtPartnerTick.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupDobTextBoxLeave));
      tbx_filtPartnerName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv .SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyKupDobTextBoxLeave));

      //==============================================================================================================================

      
      if(!isSvd)             hampFilter.CreateVvLabel  (0, 2, "PoslJed:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterPosJedCd   = hampFilter.CreateVvTextBox(1, 2, "tbx_filterPosJedCd"  , "Šifra poslovne jedinice" );
      tbx_filterPosJedTk   = hampFilter.CreateVvTextBox(2, 2, "tbx_filterPosJedTk"  , "Ticker poslovne jedinice");
      tbx_filterPosJedName = hampFilter.CreateVvTextBox(3, 2, "tbx_filterPosJedName", "Naziv poslovne jedinice");

      tbx_filterPosJedCd.JAM_CharEdits      = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_filterPosJedTk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_filterPosJedCd  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD   .SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyPoslJedTextBoxLeave));
      tbx_filterPosJedTk  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyPoslJedTextBoxLeave));
      tbx_filterPosJedName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv .SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyPoslJedTextBoxLeave));

      tbx_filterPosJedCd  .Visible = !isSvd;
      tbx_filterPosJedTk  .Visible = !isSvd;
      tbx_filterPosJedName.Visible = !isSvd;
      //==============================================================================================================================

      if(!isSvd)            hampFilter.CreateVvLabel  (0, 3, "MjTroška:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterMtrosCD   = hampFilter.CreateVvTextBox(1, 3, "tbx_filterMtrosCD"  , "Šifra  mjesta troška" );
      tbx_filterMtrosTK   = hampFilter.CreateVvTextBox(2, 3, "tbx_filterMtrosTK"  , "Ticker mjesta troška");
      tbx_filterMtrosName = hampFilter.CreateVvTextBox(3, 3, "tbx_filterMtrosName", "Naziv  mjesta troška" );

      tbx_filterMtrosCD.JAM_CharEdits       = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_filterMtrosTK.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_filterMtrosCD  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyMtrosTextBoxLeave));
      tbx_filterMtrosTK  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_filterMtrosName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyMtrosTextBoxLeave));

      tbx_filterMtrosCD   .Visible = !isSvd;
      tbx_filterMtrosTK   .Visible = !isSvd;
      tbx_filterMtrosName .Visible = !isSvd;
      //==============================================================================================================================

      if(!isSvd)               hampFilter.CreateVvLabel  (0, 4, "PrimPlat:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterPrimPlatCD   = hampFilter.CreateVvTextBox(1, 4, "tbx_filtPrimPlatCd"  , "Šifra  primatelja / platitelja" );
      tbx_filterPrimPlatTK   = hampFilter.CreateVvTextBox(2, 4, "tbx_filtPrimPlatTick", "Ticker primatelja / platitelja");
      tbx_filterPrimPlatName = hampFilter.CreateVvTextBox(3, 4, "tbx_filtPrimPlatName", "Naziv  primatelja / platitelja" );

      tbx_filterPrimPlatCD.JAM_CharEdits       = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_filterPrimPlatTK.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_filterPrimPlatCD  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyPrimPlatTextBoxLeave));
      tbx_filterPrimPlatTK  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyPrimPlatTextBoxLeave));
      tbx_filterPrimPlatName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyPrimPlatTextBoxLeave));

      tbx_filterPrimPlatCD  .Visible = !isSvd;
      tbx_filterPrimPlatTK  .Visible = !isSvd;
      tbx_filterPrimPlatName.Visible = !isSvd;


      //==============================================================================================================================
                           hampFilter.CreateVvLabel   (0, isSvd ? 2 : 5, "OrigBrDok:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterVezniDok = hampFilter.CreateVvTextBox (1, isSvd ? 2 : 5, "tbx_filtVeznidok", "Filter originalni broj dokumenta dokument", 32, 2, 0);


                                hampFilter.CreateVvLabel        (0, isSvd ? 3 : 6, "Veza:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterV_tt          = hampFilter.CreateVvTextBoxLookUp(1, isSvd ? 3 : 6, "filterV_tt"         , "Interna veza na Vektor dokument");
      tbx_filterV_ttOpis      = hampFilter.CreateVvTextBox      (3, isSvd ? 3 : 6, "filterfilter_ttOpis", "Interna veza na Vektor dokument");
      tbx_filterV_ttNum       = hampFilter.CreateVvTextBox      (2, isSvd ? 3 : 6, "filterV_ttNum"      , "Interna veza na Vektor dokument", 6);

      tbx_filterV_tt.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_filterV_tt.JAM_Set_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_filterV_tt.JAM_lui_NameTaker_JAM_Name = tbx_filterV_ttOpis.JAM_Name;
      tbx_filterV_ttOpis.JAM_ReadOnly = true;
      tbx_filterV_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;


      //==============================================================================================================================

                              hampFilter.CreateVvLabel       (0, isSvd ? 4 : 7, "Napomena:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterNapomena    = hampFilter.CreateVvTextBox     (1, isSvd ? 4 : 7, "tbx_filtNapomena", "Filter Napomena", 32, 2, 0);
      cbx_biloGdjeUnapomeni = hampFilter.CreateVvCheckBox_OLD(4, isSvd ? 4 : 7, null, "", RightToLeft.Yes, true);
      cbx_biloGdjeUnapomeni.Checked = true;
      
                               hampFilter.CreateVvLabel       (0, isSvd ? 5 : 8, isSvd ? "Odobravatelj" : "Napomena2:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterNapomena2    = hampFilter.CreateVvTextBox     (1, isSvd ? 5 : 8, "tbx_filterNapomena2", "Filter Napomena2", 32, 2, 0);
      cbx_biloGdjeUnapomeni2 = hampFilter.CreateVvCheckBox_OLD(4, isSvd ? 5 : 8, null, "", RightToLeft.Yes, true);
      cbx_biloGdjeUnapomeni2.Checked = true;


      if(!isSvd)           hampFilter.CreateVvLabel       (0, 9, "Opis:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterOpis     = hampFilter.CreateVvTextBox     (1, 9, "tbx_filterOpis", "Filter Opis", 32, 2, 0);
      cbx_biloGdjeUopisu = hampFilter.CreateVvCheckBox_OLD(4, 9, null, "", RightToLeft.Yes, true);
      cbx_biloGdjeUopisu.Checked = true;
      tbx_filterOpis    .Visible = !isSvd;
      cbx_biloGdjeUopisu.Visible = !isSvd;

      //==============================================================================================================================

      //                      hampFilter.CreateVvLabel        (5, 1, "Skladište:", System.Drawing.ContentAlignment.MiddleRight);
      //tbx_filterSkladCd   = hampFilter.CreateVvTextBoxLookUp(6, 1, "tbx_filterSkladCd", "Skladište");
      //tbx_filterSkladOpis = hampFilter.CreateVvTextBox      (7, 1, "tbx_filterSkladOpiS", "");
      //tbx_filterSkladCd.JAM_CharacterCasing = CharacterCasing.Upper;
      //tbx_filterSkladOpis.JAM_ReadOnly  = true;
          
      //tbx_filterSkladCd.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      //tbx_filterSkladCd.JAM_lui_NameTaker_JAM_Name = tbx_filterSkladOpis.JAM_Name;


      if(!isSvd)           hampFilter.CreateVvLabel        (5, 2, "Način plaćanja:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterNacPlac = hampFilter.CreateVvTextBoxLookUp("tbx_NacPlac", 6, 2, "Način plaćanja", 32, 1, 0);
      tbx_filterNacPlac.JAM_Set_LookUpTable(ZXC.luiListaRiskVrstaPl, (int)ZXC.Kolona.prva);
      tbx_filterNacPlac.Visible = !isSvd;

      if(!isSvd)             hampFilter.CreateVvLabel        (5, 3, "TipOtpreme:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterTipOtpreme = hampFilter.CreateVvTextBoxLookUp("tbx_TipOtpreme", 6, 3, "TipOtpreme", 32, 1, 0);
      tbx_filterTipOtpreme.JAM_Set_LookUpTable(ZXC.luiListaRiskTipOtprem, (int)ZXC.Kolona.prva);
      tbx_filterTipOtpreme.Visible = !isSvd;


      if(!isSvd)              hampFilter.CreateVvLabel  (5, 4, "OpciA:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterOpciAvalue  = hampFilter.CreateVvTextBox(6, 4, "tbx_OpciAvalue", "Unos teksta za ispis na računu", 32, 1, 0);
      if(!isSvd)              hampFilter.CreateVvLabel  (5, 5, "OpciB:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterOpciBvalue  = hampFilter.CreateVvTextBox(6, 5, "tbx_OpciBvalue", "Unos teksta za ispis na računu", 32, 1, 0);
      if(!isSvd)              hampFilter.CreateVvLabel  (5, 6, "OsobaA:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterPersonAcd   = hampFilter.CreateVvTextBox(6, 6, "tbx_PersonName", "Djelatnik");
      tbx_filterPersonAName = hampFilter.CreateVvTextBox(7, 6, "tbx_PersonName", "Djelatnik");
      tbx_filterPersonAcd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_filterPersonAName.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_filterPersonAcd  .JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType     , new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterSifra)  , new EventHandler(AnyPersonTextBoxLeave));
      tbx_filterPersonAName.JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonTextBoxLeave));

      if(!isSvd)              hampFilter.CreateVvLabel  (5, 7, "OsobaB:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterPersonBName = hampFilter.CreateVvTextBox(6, 7, "tbx_odgvPersName", "Odgovorna osoba", 32, 1, 0);


      if(!isSvd)           hampFilter.CreateVvLabel  (5, 8, "DostNaziv:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterDostName = hampFilter.CreateVvTextBox(6, 8, "tbx_dostName", "Naziv dostave", 32, 1, 0);
      if(!isSvd)           hampFilter.CreateVvLabel  (5, 9, "DostAdres:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterDostAddr = hampFilter.CreateVvTextBox(6, 9, "tbx_DostAddr", "Adresa dostave", 32, 1, 0);

      tbx_filterOpciAvalue .Visible = !isSvd;
      tbx_filterOpciBvalue .Visible = !isSvd;
      tbx_filterPersonAcd  .Visible = !isSvd;
      tbx_filterPersonAName.Visible = !isSvd;
      tbx_filterPersonBName.Visible = !isSvd;
      tbx_filterDostName   .Visible = !isSvd;
      tbx_filterDostAddr   .Visible = !isSvd;

      if(isSvd)              hampFilter.CreateVvLabel  (5, 2, "Ime:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_Svd_PersonAName2 = hampFilter.CreateVvTextBox(6, 2, "tbx_OpciAvalue", "Ime", 32, 1, 0);
      if(isSvd)              hampFilter.CreateVvLabel  (5, 3, "Prezime:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_Svd_OpciAlabel   = hampFilter.CreateVvTextBox(6, 3, "tbx_OpciBvalue", "Prezime", 32, 1, 0);
      if(isSvd)              hampFilter.CreateVvLabel  (5, 4, "MBO:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_Svd_OpciAvalue   = hampFilter.CreateVvTextBox(6, 4, "tbx_OpciBvalue", "MBO", 32, 1, 0);
      tbx_Svd_OpciAvalue.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_Svd_PersonAName2.Visible = isSvd;
      tbx_Svd_OpciAlabel  .Visible = isSvd;
      tbx_Svd_OpciAvalue  .Visible = isSvd;

      //--------------------------------------------------------------------------------------------------------------------------------------
                     hampFilter.CreateVvLabel        (8, 1, "TipTrans:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterTT = hampFilter.CreateVvTextBoxLookUp(9, 1, "tbx_filtTT", "TipTrans");
      tbx_filterTT.JAM_Set_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_filterTT.JAM_CharacterCasing = CharacterCasing.Upper;

      if(!isSvd)              hampFilter.CreateVvLabel        ( 8, 2, "Cjenik:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterCjenikTT    = hampFilter.CreateVvTextBoxLookUp( 9, 2, "tbx_filterCjenikTT", "CjenikTT" );
      tbx_filterCjenikTTnum = hampFilter.CreateVvTextBox      (10, 2, "tbx_filterCjenikTTnum", "CjenikNum", 6);
      tbx_filterCjenikTT.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_filterCjenikTTnum.JAM_CharEdits     = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_filterCjenikTT.JAM_Set_LookUpTable(ZXC.luiListaRiskCjenik, (int)ZXC.Kolona.prva);
      tbx_filterCjenikTT   .Visible = !isSvd;
      tbx_filterCjenikTTnum.Visible = !isSvd;

      if(!isSvd)          hampFilter.CreateVvLabel  (8, 3, "IzvVal:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterValName = hampFilter.CreateVvTextBoxLookUp(9, 3, "tbx_filterValName", "Naziv devizne valute");
      tbx_filterValName.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_filterValName.JAM_Set_LookUpTable(ZXC.luiListaDeviza, (int)ZXC.Kolona.prva);
      tbx_filterValName.Visible = !isSvd;


                         hampFilter.CreateVvLabel        (8, isSvd ? 2 : 4, "Status:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterStatus = hampFilter.CreateVvTextBoxLookUp(9, isSvd ? 2 : 4, "tbx_filterStatus", "Status");
      tbx_filterStatus.JAM_Set_LookUpTable(ZXC.luiListaRiskStatus, (int)ZXC.Kolona.prva);
      tbx_filterStatus.JAM_lookUp_NOTobligatory = true;
      tbx_filterStatus.JAM_lookUp_MultiSelection = true;


                          hampFilter.CreateVvLabel  (8, isSvd ? 3 : 5, "Projekt:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterProjekt = hampFilter.CreateVvTextBox(9, isSvd ? 3 : 5, "tbx_Projekt", "Projekt - ");
      tbx_filterProjekt.JAM_CharacterCasing = CharacterCasing.Upper;


      if(!isSvd)                hampFilter.CreateVvLabel  (8, 6, "PdvKnjiga:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterPdvKnjiga     = hampFilter.CreateVvTextBox(9, 6, "tbx_PdvKnjiga", "PdvKnjiga  R - Redovna knjiga URA/IRA, P - Knjiga predujmova URA/IRA, U - Knjiga uvoza robe URA,   S - Knjiga uvoza usluga URA");
      tbx_filterPdvKnjiga.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_filterPdvKnjiga.JAM_AllowedInputCharacters = "RUPS";
      tbx_filterPdvKnjiga.TextAlign = HorizontalAlignment.Center;
      tbx_filterPdvKnjiga.Visible = !isSvd;

      if(!isSvd)             hampFilter.CreateVvLabel  (8, 7, "Račun R-:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterPdvR12 =     hampFilter.CreateVvTextBox(9, 7, "tbx_PdvR12", "Oznaka računa 1-R1, 2-R2");
      tbx_filterPdvR12.JAM_AllowedInputCharacters = "12";
      tbx_filterPdvR12.TextAlign = HorizontalAlignment.Center;
      tbx_filterPdvR12.Visible = !isSvd;

                            hampFilter.CreateVvLabel  (8, isSvd ? 4 : 8, "VezniDok2:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterVezniDok2 = hampFilter.CreateVvTextBox(9, isSvd ? 4 : 8, "tbx_VezniDok2", "VezniDok2");
      if(!isSvd)            hampFilter.CreateVvLabel  (8, 9, "Fco:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterFco       = hampFilter.CreateVvTextBox(9, 9, "tbx_Fco", "Franco");
      tbx_filterFco.Visible = !isSvd;

      VvHamper.Open_Close_Fields_ForWriting(hampFilter  , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      hampFilter.Visible = false;
   }

   #endregion Hamperi

   #region Eveniti

   public void cbx_biloGdjeUnazivu_Click_SaveToVvPref(object sender, EventArgs e)
   {
      ZXC.TheVvForm.VvPref.findFakturKupdob.IsBiloGdjeUnazivu = Faktur.sorterKpdbName.BiloGdjeU_Tekstu = Fld_BiloGdjeKupDobName;
   }

   public void rbtSortByDokDate_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Faktur.sorterDokDate;

      ZXC.TheVvForm.VvPref.findFaktur.IsFindBy_ttNum    = false;
      ZXC.TheVvForm.VvPref.findFaktur.IsFindBy_dokDate  = true;
      ZXC.TheVvForm.VvPref.findFaktur.IsFindBy_kpdbName = false;
   }

   public void rbtSortByPartner_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Faktur.sorterKpdbName;

      ZXC.TheVvForm.VvPref.findFaktur.IsFindBy_ttNum    = false;
      ZXC.TheVvForm.VvPref.findFaktur.IsFindBy_dokDate  = false;
      ZXC.TheVvForm.VvPref.findFaktur.IsFindBy_kpdbName = true;
   }

   public void rbtSortByTtNum_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Faktur.sorterTtNum;
      VvHamper.Open_Close_Fields_ForWriting(tbx_ttNum, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);


      ZXC.TheVvForm.VvPref.findFaktur.IsFindBy_ttNum    = true;
      ZXC.TheVvForm.VvPref.findFaktur.IsFindBy_dokDate  = false;
      ZXC.TheVvForm.VvPref.findFaktur.IsFindBy_kpdbName = false;

      DaLiJeParentFindDialog(); // za nakon Reseta
   }

   public void tbx_DoubleClick(object sender, System.EventArgs e)
   {
      VvTextBox vvTb = sender as VvTextBox;

      RadioButton rbt;

      if(vvTb.Tag is DateTimePicker)
         rbt = (RadioButton)((DateTimePicker)vvTb.Tag).Tag;
      else
         rbt = (RadioButton)vvTb.Tag;

      if(!rbt.Checked) rbt.PerformClick();
   }

   public void SelectFirstSortRadioButton(VvTextBox tbxTtNum, VvTextBox tbxTt, string tbx_TtText)
   {
      VvHamper.ClearFieldContents(hampSpecifikum);
      
      this.ControlForInitialFocus = tbxTtNum;

      if(tbxTtNum.Tag is RadioButton)
      {
         RadioButton rbt = (RadioButton)tbxTtNum.Tag;
         rbt.Checked = true;
      }
      else // pucamo na slijepo; valjda hocemo datumski tbx koju u Tag-u nema Rbt nego DtaTimePickerEx 
      {
         rbtSortByDokDate.Checked = true;
         dtp_dokDate.Value = ZXC.ValOrDefault_DateTime(tbx_TtText, DateTimePicker.MinimumDateTime);
         //dtp_dokDate.Visible = true;
         //dtp_dokDate.Focus();
         //tbxTtNum.Visible = false;
      }

      if(tbxTt != null)
      {
         tbxTt.Text = tbx_TtText;
         VvHamper.Open_Close_Fields_ForWriting(tbxTt, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      }
      else
      {
         tbxTtNum.Text = tbx_TtText;
      }

      VvHamper.Open_Close_Fields_ForWriting(tbxTtNum, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
   }

   void AnyKupDobTextBoxLeave(object sender, EventArgs e)
   {
      if(this.isPopulatingSifrar) return;

      //if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;
         kupdob_rec = KupdobSifrar.Find(this.FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_FiltPartnerName = kupdob_rec.Naziv;
            Fld_FiltPartnerCD   = kupdob_rec.KupdobCD;
            Fld_FiltPartnerTick = kupdob_rec.Ticker;
         }
         else
         {
            Fld_FiltPartnerName = Fld_FiltPartnerTick = Fld_FiltPartnerCdAsTxt = "";
         }
      }
   }

   void AnyMtrosTextBoxLeave(object sender, EventArgs e)
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
            Fld_FilterMT_sifra  = kupdob_rec.KupdobCD/*RecID*/;
            Fld_FilterMT_ticker = kupdob_rec.Ticker;
            Fld_FilterMT_naziv  = kupdob_rec.Naziv;
         }
         else
         {
            Fld_FilterMT_SifraAsTxt = Fld_FilterMT_ticker = Fld_FilterMT_naziv = "";
         }
      }
   }

   void AnyPrimPlatTextBoxLeave(object sender, EventArgs e)
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
            Fld_FilterPrimPlat_sifra  = kupdob_rec.KupdobCD/*RecID*/;
            Fld_FilterPrimPlat_ticker = kupdob_rec.Ticker;
            Fld_FilterPrimPlat_naziv  = kupdob_rec.Naziv;
         }
         else
         {
            Fld_FilterPrimPlat_SfAsTx = Fld_FilterPrimPlat_ticker = Fld_FilterPrimPlat_naziv = "";
         }
      }
   }

   void AnyPoslJedTextBoxLeave(object sender, EventArgs e)
   {
      if(this.isPopulatingSifrar) return;

      //if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;
         kupdob_rec = KupdobSifrar.Find(this.FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_FiltPoslJedName = kupdob_rec.Naziv;
            Fld_FiltPoslJedCD   = kupdob_rec.KupdobCD;
            Fld_FiltPoslJedTick = kupdob_rec.Ticker;
         }
         else
         {
            Fld_FiltPoslJedName = Fld_FiltPoslJedTick = Fld_FiltPoslJedCdAsTxt = "";
         }
      }
   }

   void AnyPersonTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      TextBox tb = sender as TextBox;
      Person person_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         person_rec = PersonSifrar.Find(FoundInSifrar<Person>);

         if(person_rec != null && tb.Text != "")
         {
            Fld_FilterPersonAcd   = person_rec.PersonCD/*RecID*/;
            Fld_FilterPersonAName = person_rec.PrezimeIme;
         } 
         else
         {
            Fld_FilterPersonAcdAsTxt = Fld_FilterPersonAName = "";
         }
      }
   }
   
   #endregion Eveniti

   #region DataGridView
   protected override void CreateDataGridViewColumn()
   {
      DataGridViewTextBoxColumn col;
      bool isDOKO = (ZXC.TheVvForm.TheVvUC is DIZ_PTG_DUC || ZXC.TheVvForm.TheVvUC is PVR_PTG_DUC || /*ZXC.TheVvForm.TheVvUC is PVD_PTG_DUC ||*/ ZXC.TheVvForm.TheVvUC is ZIZ_PTG_DUC);
      bool isUGAN = (ZXC.TheVvForm.TheVvUC is UGO_PTG_DUC || ZXC.TheVvForm.TheVvUC is ANU_PTG_DUC);
      bool is2NPl = (ZXC.IsTETRAGRAM_ANY && (ZXC.TheVvForm.TheVvUC is IRA_MPC_DUC || ZXC.TheVvForm.TheVvUC is PON_MPC_DUC || ZXC.TheVvForm.TheVvUC is OPN_MPC_DUC || ZXC.TheVvForm.TheVvUC is POT_DUC));
      
      int numOfTtNum = isDOKO ? 10 : isUGAN ? 7 : 6;

      int sumOfColWidth = 0, colWidth;
      int colDateWidth = ZXC.Q4un;
      int colSif6Width = ZXC.Q3un + ZXC.Qun2;

      sumOfColWidth += TheGrid.RowHeadersWidth;

      if(IsArhivaTabPage)
      {
         AddDGV_ArhivaColumns(ref sumOfColWidth);
      }

      
      colWidth = colSif6Width;                                  AddDGVColum_RecID_4GridReadOnly   (TheGrid, "RecID"    , colWidth, false, 0, "recID");

     // colWidth = ZXC.Q4un          ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Dodao"    , colWidth, false   , "addUID");

      // 11.10.2017: zakljucujem da je korisnije iskazati posljed nego centralu, jer ako i nema posljed tu dolazi centrala (koipra se posljed u centralu kod ADDRECa) 
    //colWidth = ZXC.Q9un          ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Partner"  , colWidth, true    , "ext_kupdobName");
      string partnerCaption = ZXC.RRD.Dsc_IsCentralaFindFaktur ? "Partner (Centrala)" : "Partner (Poslovna Jedinica)";
      string partnerDSname  = ZXC.RRD.Dsc_IsCentralaFindFaktur ? "ext_kupdobName"     : "ext_posJedName"             ;
      colWidth = ZXC.Q9un          ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, partnerCaption  , colWidth, true    , partnerDSname);

      colWidth = ZXC.Q2un                                          ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "TT"       , colWidth, false   , "tt");
      colWidth = (isDOKO || isUGAN) ? ZXC.Q4un : ZXC.Q3un          ; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (TheGrid, "Broj"     , colWidth, true, /*6*/numOfTtNum, "ttNum");

      if(isDOKO)
      {
         colWidth = ZXC.Q2un; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly(TheGrid, "KUG", colWidth, true, 2, "v1_ttNum");
         colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly(TheGrid, "UGAN", colWidth, true, 5, "v2_ttNum");
      }

      colWidth = ZXC.Q4un          ; sumOfColWidth += colWidth; AddDGVColum_DateTime_4GridReadOnly(TheGrid, "Datum"    , colWidth          , "dokDate");
      colWidth = ZXC.Q2un+ZXC.Qun4 ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Sklad"    , colWidth, false   , "skladCD");
    //colWidth = ZXC.Q5un          ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Broj u PG", colWidth, false   , "vezniDok2");
    
      if(is2NPl)
      { 
         colWidth = ZXC.Q5un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Način plaćanja " , colWidth, false  , "ext_NacPlac" );
         colWidth = ZXC.Q5un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Način plaćanja 2", colWidth, false  , "ext_NacPlac2");
         colWidth = ZXC.Q2un - ZXC.Qun4; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "V1 TT"           , colWidth, false  , "v1_TT");
         colWidth = ZXC.Q3un - ZXC.Qun4; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (TheGrid, "Broj1"           , colWidth, true, 6, "v1_ttNum");
         colWidth = ZXC.Q2un - ZXC.Qun4; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "V2 TT"           , colWidth, false  , "v2_TT");
         colWidth = ZXC.Q3un - ZXC.Qun4; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (TheGrid, "Broj2"           , colWidth, true, 6, "v2_ttNum");
      }

      colWidth = ZXC.Q5un          ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "OrigBrDok", colWidth, false   , "vezniDok");

      if(ZXC.IsSvDUH)
      {
         colWidth = ZXC.Q4un + ZXC.Qun4; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Pac.Prezime", colWidth, false, "ext_opciAlabel");
         colWidth = ZXC.Q4un + ZXC.Qun4; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Pac.MBO"    , colWidth, false, "ext_opciAvalue");
      }
      colWidth = ZXC.Q8un          ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Napomena" , colWidth, false   , "napomena");
      colWidth = ZXC.Q2un+ZXC.Qun4 ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Status"   , colWidth, false   , "ext_statusCD");
      colWidth = ZXC.Q3un+ZXC.Qun2 ; sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly (TheGrid, "Iznos"    , colWidth,        2, "ext_s_ukKCRMP");

      grid_Width = sumOfColWidth + ZXC.QUN;
   }

   #endregion DataGridView

   #region Fld_
   // 30.12.2012:
 //public uint     Fld_FromTtNum             { get { return tbx_ttNum.GetSomeRecIDField();           } set { tbx_ttNum.PutSomeRecIDField(value);            }   }
   public uint     Fld_FromTtNum             
   { 
      get 
      {
         uint fldTextAsUint = tbx_ttNum.GetSomeRecIDField();

         if(fldTextAsUint.NotZero() && 
            ZXC.TtInfo(Fld_FromTT).IsSklCdInTtNum && 
            fldTextAsUint < Faktur.NultiTtNum)
         {
            return Faktur.GetTtNumFromRbr(ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD, tbx_ttNum.GetSomeRecIDField());
         }
         else
         {
            return fldTextAsUint;
         }
      } 

      set { tbx_ttNum.PutSomeRecIDField(value); }   
   }

   public string   Fld_FromTT                { get { return tbx_TT.Text;                             } set { tbx_TT.Text = value;                           }   }
   public DateTime Fld_FromDokDate           { get { return dtp_dokDate.Value;                       } set { dtp_dokDate.Value = value;                     }   }
   public string   Fld_FromPartner           { get { return tbx_Partner.Text;                        } set { tbx_Partner.Text = value;                      }   }
   public bool     Fld_BiloGdjeKupDobName    { get { return cbx_biloGdjeUnazivu.Checked;             } set { cbx_biloGdjeUnazivu.Checked = value;           }   }
   public uint     Fld_FiltPartnerCD         { get { return tbx_filtPartnerCD.GetSomeRecIDField();   } set { tbx_filtPartnerCD.PutSomeRecIDField(value);    }   }
   public string   Fld_FiltPartnerCdAsTxt    { get { return tbx_filtPartnerCD.Text;                  } set { tbx_filtPartnerCD.Text = value;                }   }
   public string   Fld_FiltPartnerName       { get { return tbx_filtPartnerName.Text;                } set { tbx_filtPartnerName.Text = value;              }   }
   public string   Fld_FiltPartnerTick       { get { return tbx_filtPartnerTick.Text;                } set { tbx_filtPartnerTick.Text = value;              }   }
   public string   Fld_FilterTT              { get { return tbx_filterTT.Text;                       } set { tbx_filterTT.Text = value;                     }   }
   public string   Fld_FilterSkladCD         { get { return tbx_filterSkladCd.Text;                  } set { tbx_filterSkladCd.Text = value;                }   }
   public string   Fld_FilterSkladOpis       {                                                         set { tbx_filterSkladOpis.Text = value;              }   }
   public string   Fld_FilterNapomena        { get { return tbx_filterNapomena.Text;                 } set { tbx_filterNapomena.Text = value;               }   }
   public bool     Fld_BiloGdjeUNapomena     { get { return cbx_biloGdjeUnapomeni.Checked;           } set { cbx_biloGdjeUnapomeni.Checked = value;         }   }
   public string   Fld_FilterNapomena2       { get { return tbx_filterNapomena2.Text;                } set { tbx_filterNapomena2.Text = value;              }   }
   public bool     Fld_BiloGdjeUNapomena2    { get { return cbx_biloGdjeUnapomeni2.Checked;          } set { cbx_biloGdjeUnapomeni2.Checked = value;        }   }
   public string   Fld_FilterOpis            { get { return tbx_filterOpis.Text;                     } set { tbx_filterOpis.Text = value;                   }   }
   public bool     Fld_BiloGdjeUOpisu        { get { return cbx_biloGdjeUopisu.Checked;              } set { cbx_biloGdjeUopisu.Checked = value;            }   }
   public string   Fld_FilterOrigBrDok       { get { return tbx_filterVezniDok.Text;                 } set { tbx_filterVezniDok.Text = value;               }   }
   public string   Fld_FilterMT_ticker       { get { return tbx_filterMtrosTK.Text;                  } set { tbx_filterMtrosTK.Text = value;                }   }
   public uint     Fld_FilterMT_sifra        { get { return tbx_filterMtrosCD.GetSomeRecIDField();   } set { tbx_filterMtrosCD.PutSomeRecIDField(value);    }   }
   public string   Fld_FilterMT_SifraAsTxt   { get { return tbx_filterMtrosCD.Text;                  } set { tbx_filterMtrosCD.Text = value;                }   }
   public string   Fld_FilterMT_naziv        { get { return tbx_filterMtrosName.Text;                } set { tbx_filterMtrosName.Text = value;              }   }
   public string   Fld_FilterPrimPlat_ticker { get { return tbx_filterPrimPlatTK.Text;               } set { tbx_filterPrimPlatTK.Text = value;             }   }
   public uint     Fld_FilterPrimPlat_sifra  { get { return tbx_filterPrimPlatCD.GetSomeRecIDField();} set { tbx_filterPrimPlatCD.PutSomeRecIDField(value); }   }
   public string   Fld_FilterPrimPlat_SfAsTx { get { return tbx_filterPrimPlatCD.Text;               } set { tbx_filterPrimPlatCD.Text = value;             }   }
   public string   Fld_FilterPrimPlat_naziv  { get { return tbx_filterPrimPlatName.Text;             } set { tbx_filterPrimPlatName.Text = value;           }   }
   public uint     Fld_FiltPoslJedCD         { get { return tbx_filterPosJedCd.GetSomeRecIDField();  } set { tbx_filterPosJedCd.PutSomeRecIDField(value);   }   }
   public string   Fld_FiltPoslJedCdAsTxt    { get { return tbx_filterPosJedCd.Text;                 } set { tbx_filterPosJedCd.Text = value;               }   }
   public string   Fld_FiltPoslJedName       { get { return tbx_filterPosJedName.Text;               } set { tbx_filterPosJedName.Text = value;             }   }
   public string   Fld_FiltPoslJedTick       { get { return tbx_filterPosJedTk.Text;                 } set { tbx_filterPosJedTk.Text = value;               }   }
   public string   Fld_FilterCjenikTT        { get { return tbx_filterCjenikTT.Text;                 } set { tbx_filterCjenikTT.Text = value;               }   } 
   public uint     Fld_FilterCjenikTTnum     { get { return tbx_filterCjenikTTnum.GetUintField();    } set { tbx_filterCjenikTTnum.PutUintField(value);     }   }
   public string   Fld_FilterVeza_tt         { get { return tbx_filterV_tt.Text;                     } set { tbx_filterV_tt.Text = value;                   }   }
   public uint     Fld_FilterVeza_ttNum      { get { return tbx_filterV_ttNum.GetUintField();        } set { tbx_filterV_ttNum.PutUintField(value);         }   }
   public string   Fld_FilterVeza_ttOpis     {                                                         set { tbx_filterV_ttOpis.Text = value;               }   }
   public string   Fld_FilterValName         { get { return tbx_filterValName.Text;                  } set { tbx_filterValName.Text = value;                }   } 
   public string   Fld_FilterStatus          { get { return tbx_filterStatus.Text;                   } set { tbx_filterStatus.Text = value;                 }   } 
   public string   Fld_FilterProjekt         { get { return tbx_filterProjekt.Text;                  } set { tbx_filterProjekt.Text = value;                }   } 
   public string   Fld_FilterNacPlac         { get { return tbx_filterNacPlac.Text;                  } set { tbx_filterNacPlac.Text = value;                }   } 
   public string   Fld_FilterTipOtpreme      { get { return tbx_filterTipOtpreme.Text;               } set { tbx_filterTipOtpreme.Text = value;             }   } 
   public string   Fld_FilterOpciAvalue      { get { return tbx_filterOpciAvalue .Text;              } set { tbx_filterOpciAvalue .Text = value;            }   } 
   public string   Fld_FilterOpciBvalue      { get { return tbx_filterOpciBvalue .Text;              } set { tbx_filterOpciBvalue .Text = value;            }   } 
   public string   Fld_FilterPersonAName     { get { return tbx_filterPersonAName.Text;              } set { tbx_filterPersonAName.Text = value;            }   } 
   public string   Fld_FilterPersonBName     { get { return tbx_filterPersonBName.Text;              } set { tbx_filterPersonBName.Text = value;            }   } 
   public string   Fld_FilterDostName        { get { return tbx_filterDostName.Text;                 } set { tbx_filterDostName.Text = value;               }   } 
   public string   Fld_FilterDostAddr        { get { return tbx_filterDostAddr.Text;                 } set { tbx_filterDostAddr.Text = value;               }   } 
   public string   Fld_FilterVezniDok2       { get { return tbx_filterVezniDok2.Text;                } set { tbx_filterVezniDok2.Text = value;              }   } 
   public string   Fld_FilterFco             { get { return tbx_filterFco      .Text;                } set { tbx_filterFco      .Text = value;              }   } 

    
    
   public ZXC.PdvKnjigaEnum Fld_FilterPdvKnjiga 
   {
      get
      {
         switch(tbx_filterPdvKnjiga.Text)
         {
            case "R": return ZXC.PdvKnjigaEnum.REDOVNA;
            case "P": return ZXC.PdvKnjigaEnum.PREDUJAM;
            case "U": return ZXC.PdvKnjigaEnum.UVOZ_ROB;
            case "S": return ZXC.PdvKnjigaEnum.UVOZ_USL;
         
            default:  return ZXC.PdvKnjigaEnum.NIJEDNA;
         }
      }
      set
      {
         switch(value)
         {
            case ZXC.PdvKnjigaEnum.REDOVNA:  tbx_filterPdvKnjiga.Text = "R"; break;
            case ZXC.PdvKnjigaEnum.PREDUJAM: tbx_filterPdvKnjiga.Text = "P"; break;
            case ZXC.PdvKnjigaEnum.UVOZ_ROB: tbx_filterPdvKnjiga.Text = "U"; break;
            case ZXC.PdvKnjigaEnum.UVOZ_USL: tbx_filterPdvKnjiga.Text = "S"; break;
            default:                         tbx_filterPdvKnjiga.Text = ""; break;
         }
      }
   }
   public ZXC.PdvR12Enum    Fld_FilterPdvR12    
   {
      get
      {
         switch(tbx_filterPdvR12.Text)
         {
            case "1": return ZXC.PdvR12Enum.R1;
            case "2": return ZXC.PdvR12Enum.R2;

            default: return ZXC.PdvR12Enum.NIJEDNO;
         }
      }
      set
      {
         switch(value)
         {
            case ZXC.PdvR12Enum.R1: tbx_filterPdvR12.Text = "1"; break;
            case ZXC.PdvR12Enum.R2: tbx_filterPdvR12.Text = "2"; break;
            default               : tbx_filterPdvR12.Text =  ""; break;
         }
      }
   }


   public uint   Fld_FilterPersonAcd      { get { return tbx_filterPersonAcd.GetSomeRecIDField(); } set { tbx_filterPersonAcd.PutSomeRecIDField(value); } }
   public string Fld_FilterPersonAcdAsTxt { get { return tbx_filterPersonAcd.Text;                } set { tbx_filterPersonAcd.Text = value;             } }


  public string Fld_FilterPersonAName2{ get { return tbx_Svd_PersonAName2.Text;                } set { tbx_Svd_PersonAName2.Text = value;             } }
  public string Fld_OpciAlabel        { get { return tbx_Svd_OpciAlabel  .Text;                } set { tbx_Svd_OpciAlabel  .Text = value;             } }
  public string Fld_OpciAvalue        { get { return tbx_Svd_OpciAvalue  .Text;                } set { tbx_Svd_OpciAvalue  .Text = value;             } }

   #endregion Fld_

   #region Overriders and specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.faktur_rec; }
      set {        this.faktur_rec = (Faktur)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.FakturDao; }
   }

   private Vektor.DataLayer.DS_FindRecord.DS_findFaktur ds_faktur;

   protected override DataSet VirtualUntypedDataSet { get { return ds_faktur; } }

   protected override object[] From_IndexSegmentValues
   {
      get
      {
         switch(recordSorter.SortType)
         {
            case VvSQL.SorterType.TtNum   : return new object[] { ZXC.TtInfo(Fld_FromTT).TtSort, Fld_FromTtNum                      , 0 };
            case VvSQL.SorterType.DokDate : return new object[] { ZXC.TtInfo(Fld_FromTT).TtSort, Fld_FromDokDate.Date, Fld_FromTtNum, 0 };
            case VvSQL.SorterType.KpdbName: return new object[] { ZXC.TtInfo(Fld_FromTT).TtSort, Fld_FromPartner     , Fld_FromTtNum, 0 };

            default: ZXC.aim_emsg("Q42: SortType [{0}] undifajnd in property 'From_IndexSegmentValues'", recordSorter.SortType); return null;
         }
      }
   }

   public override void SetListFilterRecordDependentDefaults()
   {
      if(Default_TT == "UNDEF") Fld_FromTT = Fld_FilterTT = "";
      else                      Fld_FromTT = Fld_FilterTT = Default_TT;
   }

   protected override VvTextBox VvTbx_Virtual_TT       { get { return this.tbx_TT; } }
   protected override VvTextBox VvTbx_VirtualFilter_TT { get { return this.tbx_filterTT; } set { this.tbx_filterTT = value; } }

   #endregion Overriders and specifics

   #region AddFilterMemberz()

   /// <summary>
   /// get { return ZXC.FakturDao.TheSchemaTable.Rows; }
   /// </summary>
   public DataRowCollection FakturSchemaRows
   {
      get { return ZXC.FakturDao.TheSchemaTable.Rows; }
   }

   /// <summary>
   ///  get { return ZXC.FakturDao.CI; }
   /// </summary>
   public FakturDao.FakturCI FakturCI
   {
      get { return ZXC.FakturDao.CI; }
   }


   /// <summary>
   /// get { return ZXC.FaktExDao.TheSchemaTable.Rows; }
   /// </summary>
   public DataRowCollection FaktExSchemaRows
   {
      get { return ZXC.FaktExDao.TheSchemaTable.Rows; }
   }

   /// <summary>
   ///  get { return ZXC.FaktExDao.CI; }
   /// </summary>
   public FaktExDao.FaktExCI FaktExCI
   {
      get { return ZXC.FaktExDao.CI; }
   }

   public override void AddFilterMemberz()
   {
      string text, preffix;
      uint   num;
      bool   isCheck;

      DataRow drSchema;

      this.TheFilterMembers.Clear();

      // Fld_FilterTT                                                                                                                                          

      drSchema = FakturSchemaRows[FakturCI.tt];
      text     = Fld_FilterTT;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "PreferredTT", text, " = "));
      }

      // Fld_FiltPartnerCD                                                                                                                                          
      drSchema = FaktExSchemaRows[FaktExCI.kupdobCD];
      num      = Fld_FiltPartnerCD;

      if(num != 0)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Partner", num, " = "));
      }

      // Fld_FilterSkladCD                                                                                                                                    
      drSchema = FakturSchemaRows[FakturCI.skladCD];
      text     = Fld_FilterSkladCD;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "SkladCD", text, " = "));
      }


      // Fld_FilterNapomena                                                                                                                                          

      drSchema = FakturSchemaRows[FakturCI.napomena];
      text     = Fld_FilterNapomena;
      isCheck  = Fld_BiloGdjeUNapomena;
      preffix  = isCheck ? "%" : "";

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Napomena", preffix + text + "%", " LIKE "));
      }

      // Fld_FilterVezniDok                                                                                                                                          

      drSchema = FakturSchemaRows[FakturCI.vezniDok];
      text     = Fld_FilterOrigBrDok;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "VezniDok", text, " = "));
      }

      //Fld_FilterStatus                                                                                                                                                          

      drSchema = FaktExSchemaRows[FaktExCI.statusCD];
      text     = Fld_FilterStatus;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Status", text, " REGEXP "));
      }

      //Fld_FilterNapomena2      
      //Fld_BiloGdjeUNapomena2   
      //Fld_FilterOpis           
      //Fld_BiloGdjeUOpisu       
      //Fld_FilterOrigBrDok      
      //Fld_FilterMT_ticker      
      //Fld_FilterMT_sifra       
      //Fld_FilterMT_SifraAsTxt  
      //Fld_FilterMT_naziv       
      //Fld_FilterPrimPlat_ticker
      //Fld_FilterPrimPlat_sifra 
      //Fld_FilterPrimPlat_SfAsTx
      //Fld_FilterPrimPlat_naziv 
      //Fld_FiltPoslJedCD        
      //Fld_FiltPoslJedCdAsTxt   
      //Fld_FiltPoslJedName      
      //Fld_FiltPoslJedTick      
      //Fld_FilterCjenikTT       
      //Fld_FilterCjenikTTnum    
      //Fld_FilterVeza_tt        
      //Fld_FilterVeza_ttNum     
      //Fld_FilterVeza_ttOpis    
      //Fld_FilterValName        
      //Fld_FilterProjekt        
      //Fld_FilterNacPlac        
      //Fld_FilterTipOtpreme     
      //Fld_FilterOpciAvalue     
      //Fld_FilterOpciBvalue     
      //Fld_FilterPersonAName    
      //Fld_FilterPersonBName    
      //Fld_FilterDostName       
      //Fld_FilterDostAddr       
      //Fld_FilterVezniDok2      
      //Fld_FilterFco            
      //Fld_FilterPdvKnjiga
      //Fld_FilterPdvR12   

      // tek od 30.11.2015: SVI ovi dole navedeni 

      // Fld_FiltMtrosCD                                                                                                                                          
      drSchema = FaktExSchemaRows[FaktExCI.mtrosCD];
      num = Fld_FilterMT_sifra;

      if(num != 0)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Mtros", num, " = "));
      }

      // Fld_FilterValName                                                                                                                                          
      drSchema = FaktExSchemaRows[FaktExCI.devName];
      text = Fld_FilterValName;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "IzvVal", text, " = "));
      }

      // Fld_FilterVezniDok2                                                                                                                                          

      drSchema = FaktExSchemaRows[FaktExCI.vezniDok2];
      text = Fld_FilterVezniDok2;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "VezniDok2", text, " = "));
      }

      // Fld_FilterPersonAName                                                                                                                                          

      drSchema = FaktExSchemaRows[FaktExCI.personName];
      text = Fld_FilterPersonAName;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "PersonAName", text, " = "));
      }

      // Fld_FilterPersonAName2 - SVD: ime pacijenta                                                                                                                                          

      drSchema = FaktExSchemaRows[FaktExCI.personName];
      text = Fld_FilterPersonAName2;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "PersonAName2", text, " = "));
      }

      // Fld_OpciAlabel                                                                                                                                          

      drSchema = FaktExSchemaRows[FaktExCI.opciAlabel];
      text = Fld_OpciAlabel; // SVD: prezime pacijenta 

      if(text.NotEmpty())
      {
       //this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "OpciAlabel", text,       " = "   ));
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "OpciAlabel", text + "%", " LIKE "));
      }

      // Fld_OpciAvalue                                                                                                                                          

      drSchema = FaktExSchemaRows[FaktExCI.opciAvalue];
      text = Fld_OpciAvalue; // SVD: MBO pacijenta 

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "OpciAvalue", text,       " = "   ));
      }


   }

   #endregion AddFilterMemberz()

}
