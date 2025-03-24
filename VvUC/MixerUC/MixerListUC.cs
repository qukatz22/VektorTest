using System;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Collections.Generic;

public class MixerListUC : /*VvRecLstUC*/VvDocumRecLstUC
{
   #region Fieldz

   protected Mixer mixer_rec;

   public RadioButton rbtSortByTtNum, rbtSortByDokDate, rBtcurrChecked, rbtSortByPartner;

   public VvTextBox tbx_dokDate, tbx_ttNum, tbx_TT, tbx_Partner,
                    tbx_filtPartnerName, tbx_filtPartnerCD, tbx_filtPartnerTick,
                    tbx_filterTT, tbx_filterNapomena,
                    tbx_filterMtrosCD, tbx_filterMtrosTK, tbx_filterMtrosName,
                    tbx_filterPersonCD, tbx_filterPersonPrezime,
                    tbx_filterStrC_32, tbx_filterStrH_32,
                    tbx_filterProjekt,
                    tbx_filterStatus,
                    tbx_filterValName,
                    tbx_filterPersonAName,
                    tbx_filterPersonBName,
                    tbx_filterStrA_40 ,
                    tbx_filterStrB_128,
                    tbx_filterStrD_32 ,
                    tbx_filterStrE_256,
                    tbx_filterStrF_64 ,
                    tbx_filterStrG_40 
                    
                    ;

   public CheckBox cbx_biloGdjeUnazivu, cbx_biloGdjeUnapomeni, cbx_biloGdjeUstrC_32, cbx_biloGdjeUstrH_32;

   public VvDateTimePicker dtp_dokDate;

   public string Default_TT { get; set; }
   int sumOfColWidth = 0, colWidth;
   int colDateWidth = ZXC.Q4un;
   int colSif6Width = ZXC.Q3un + ZXC.Qun2;

   #endregion Fieldz

   #region Constructor

   public MixerListUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul, VvForm.VvSubModul vvMasterSubModul)      : base(parent)
   {
      this.mixer_rec = _mixer;

      this.MasterSubModulEnum = vvMasterSubModul.subModulEnum;
      this.TheSubModul        = vvSubModul;
      this.Parent.Text        = this.TheSubModul.subModul_name;
      
      this.Default_TT = ((MixerDUC)ZXC.TheVvForm.TheVvUC).Fld_TT;

      //SpecificsTTcolumns(Default_TT);
      SpecificsTThampers(Default_TT);

      TheFilterMembers = new System.Collections.Generic.List<VvSqlFilterMember>(); // namoj tamo di ne treba (npr Kplan) 

      Supress_ImaLiIjedan_StartField_Neprazan_Action = true;
      SelectFirstSortRadioButton(tbx_ttNum, tbx_TT, Default_TT);

      DaLiJeParentFindDialog();
   }

   private void SpecificsTThampers(string Default_TT)
   {
      SetVisibilitiSortPartner(false);

      if(this.Default_TT == Mixer.TT_VIRMAN || this.Default_TT == Mixer.TT_RASTERF)
      {
         CreateHamperFilterOnlyNapomena();
      }
      if(this.Default_TT == Mixer.TT_PUTN_I || this.Default_TT == Mixer.TT_PUTN_T || this.Default_TT == Mixer.TT_PUTN_L || this.Default_TT == Mixer.TT_PUTN_R)
      {
         CreateHamperFilter4Putnalog();
      }
      if(this.Default_TT.StartsWith("Z") && (this.Default_TT != Mixer.TT_ZPG && this.Default_TT != Mixer.TT_ZLJ))
      {
         CreateHamperFilter4Zahtjevi();
         SetVisibilitiSortPartner(true);
      }
      if(this.Default_TT == Mixer.TT_SMD)
      {
         CreateHamperFilter4Smd();
      }
      if(this.Default_TT.StartsWith("E"))
      {
         CreateHamperFilter4Evidencije();
         SetVisibilitiSortPartner(true);
      }
      if(this.Default_TT == Mixer.TT_IRV)
      {
         CreateHamperFilter4RadnoVrijemeIRV();
      }
      if(this.Default_TT == Mixer.TT_URZ || this.Default_TT == Mixer.TT_RUG)
      {
         CreateHamperFilter4_RUG();
         SetVisibilitiSortPartner(true);
      }
      if(this.Default_TT == Mixer.TT_ZPG || this.Default_TT == Mixer.TT_ZLJ)
      {
         CreateHamperFilter4Zastitari();
         SetVisibilitiSortPartner(true);
      }
      if(this.Default_TT == Mixer.TT_KDC)
      {
         SetVisibilitiSortPartner(true);
      }

      CalculateLocationAndSize();

   }


   protected override void InitializeFindFormSpecifics()
   {
      recordSorter = Mixer.sorterTtNum;

      this.ds_mixer = new Vektor.DataLayer.DS_FindRecord.DS_findMixer();
      

      this.Name = "MixerListUC";
      this.Text = "MixerListUC";
   }

   #endregion Constructor

   #region FindDialog

   private void DaLiJeParentFindDialog()
   {
         tbx_filterTT.Text = Default_TT;
         OtvoriZatvoriTtOvisnoOtt(true);

         ((VvFindDialog)this.Parent).button_OpenTPage.Visible = false;
   }
  
   private void OtvoriZatvoriTtOvisnoOtt(bool readOnly)
   {

      tbx_filterTT.Text = Default_TT;

      if(Default_TT.StartsWith("Z"))
      {
         tbx_TT      .JAM_ReadOnly = false;
         tbx_filterTT.JAM_ReadOnly = false;

         if(ZXC.IsZASTITARI)
         {
            tbx_TT.      JAM_Set_LookUpTable(ZXC.luiListaMixTypeZastitari, (int)ZXC.Kolona.prva);
            tbx_filterTT.JAM_Set_LookUpTable(ZXC.luiListaMixTypeZastitari, (int)ZXC.Kolona.prva);
         }
         else
         {
            tbx_TT.      JAM_Set_LookUpTable(ZXC.luiListaMixTypeZahtjev, (int)ZXC.Kolona.prva);
            tbx_filterTT.JAM_Set_LookUpTable(ZXC.luiListaMixTypeZahtjev, (int)ZXC.Kolona.prva);
         }

         VvHamper.Open_Close_Fields_ForWriting(tbx_TT, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
         VvHamper.Open_Close_Fields_ForWriting(tbx_filterTT, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      }
      else if(Default_TT.StartsWith("E"))
      {
         tbx_TT      .JAM_ReadOnly = false;
         tbx_filterTT.JAM_ReadOnly = false;

         tbx_TT      .JAM_Set_LookUpTable(ZXC.luiListaMixTypeEvidencija, (int)ZXC.Kolona.prva);
         tbx_filterTT.JAM_Set_LookUpTable(ZXC.luiListaMixTypeEvidencija, (int)ZXC.Kolona.prva);

         VvHamper.Open_Close_Fields_ForWriting(tbx_TT      , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
         VvHamper.Open_Close_Fields_ForWriting(tbx_filterTT, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      }
      else if(Default_TT == Mixer.TT_GFI     || Default_TT == Mixer.TT_TSI     || Default_TT == Mixer.TT_OPD)
      {
         tbx_TT.JAM_ReadOnly       = false;
         tbx_filterTT.JAM_ReadOnly = false;

         tbx_TT      .JAM_Set_LookUpTable(ZXC.luiListaMix_GFI_TSI, (int)ZXC.Kolona.prva);
         tbx_filterTT.JAM_Set_LookUpTable(ZXC.luiListaMix_GFI_TSI, (int)ZXC.Kolona.prva);

         VvHamper.Open_Close_Fields_ForWriting(tbx_TT      , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
         VvHamper.Open_Close_Fields_ForWriting(tbx_filterTT, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      }
      else if(Default_TT == Mixer.TT_NPF_BIL || Default_TT == Mixer.TT_NPF_PPR || Default_TT == Mixer.TT_NPF_SPR || Default_TT == Mixer.TT_NPF_GPR)
      {
         tbx_TT.JAM_ReadOnly       = false;
         tbx_filterTT.JAM_ReadOnly = false;

         tbx_TT      .JAM_Set_LookUpTable(ZXC.luiListaMix_Statist_NPF, (int)ZXC.Kolona.prva);
         tbx_filterTT.JAM_Set_LookUpTable(ZXC.luiListaMix_Statist_NPF, (int)ZXC.Kolona.prva);

         VvHamper.Open_Close_Fields_ForWriting(tbx_TT      , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
         VvHamper.Open_Close_Fields_ForWriting(tbx_filterTT, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      }
      else if(Default_TT == Mixer.TT_PLN     || Default_TT == Mixer.TT_RBL     || Default_TT == Mixer.TT_NJV     || Default_TT == Mixer.TT_PBN)
      {
         tbx_TT.JAM_ReadOnly       = false;
         tbx_filterTT.JAM_ReadOnly = false;

         tbx_TT      .JAM_Set_LookUpTable(ZXC.luiLista_PlanTT, (int)ZXC.Kolona.prva);
         tbx_filterTT.JAM_Set_LookUpTable(ZXC.luiLista_PlanTT, (int)ZXC.Kolona.prva);

         VvHamper.Open_Close_Fields_ForWriting(tbx_TT      , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
         VvHamper.Open_Close_Fields_ForWriting(tbx_filterTT, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      }

      else
      {
         tbx_TT.JAM_ReadOnly = readOnly;

         ZXC.ZaUpis zaUpisOtvZat;
         if(readOnly) zaUpisOtvZat = ZXC.ZaUpis.Zatvoreno;
         else         zaUpisOtvZat = ZXC.ZaUpis.Otvoreno;
         VvHamper.Open_Close_Fields_ForWriting(tbx_TT      , zaUpisOtvZat, ZXC.ParentControlKind.VvFindDialog);
         VvHamper.Open_Close_Fields_ForWriting(tbx_filterTT, zaUpisOtvZat, ZXC.ParentControlKind.VvFindDialog);

      }
   }

   #endregion FindDialog

   #region Hamperi

   protected override void CreateHamperSpecifikum()
   {
      hampSpecifikum = new VvHamper(6, 2, "", this, true, hampListaRastePada.Right + ZXC.Qun4, nextY, razmakHamp);

      hampSpecifikum.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un, ZXC.Q3un,            ZXC.Q4un, ZXC.Q4un, ZXC.Q9un };
      hampSpecifikum.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun2 + ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hampSpecifikum.VvRightMargin = hampSpecifikum.VvLeftMargin;

      hampSpecifikum.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN  };
      hampSpecifikum.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hampSpecifikum.VvBottomMargin = hampSpecifikum.VvTopMargin;

      tbx_TT = hampSpecifikum.CreateVvTextBoxLookUp(1, 0, "tbx_TT", "");

      //if(ZXC.TheVvForm.TheVvUC is PutNalDUC) tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaMixTypePutNal, (int)ZXC.Kolona.prva);
      //else tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaMixerType, (int)ZXC.Kolona.prva);

      if(this.Parent is VvFindDialog) tbx_TT.JAM_ReadOnly = true;

      rbtSortByTtNum = hampSpecifikum.CreateVvRadioButton(0, 0, new EventHandler(rbtSortByTtNum_Click), "Od dokumenta:", TextImageRelation.ImageAboveText);
      tbx_ttNum      = hampSpecifikum.CreateVvTextBox    (2, 0, "tbx_ttNum", "");
      tbx_ttNum.Tag  = rbtSortByTtNum;
      tbx_TT.Tag     = rbtSortByTtNum;
      rbtSortByTtNum.Tag = tbx_ttNum;
      tbx_ttNum.DoubleClick += new EventHandler(tbx_DoubleClick);
      tbx_ttNum.JAM_ValueType = typeof(int);
      tbx_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_TT.JAM_CharacterCasing = CharacterCasing.Upper;


      rbtSortByDokDate = hampSpecifikum.CreateVvRadioButton(3, 0, new EventHandler(rbtSortByDokDate_Click), "Od datuma:", TextImageRelation.ImageAboveText);
      tbx_dokDate      = hampSpecifikum.CreateVvTextBox    (4, 0, "tbx_dokDate", "");
      rbtSortByDokDate.Tag = tbx_dokDate;
      tbx_dokDate.JAM_IsForDateTimePicker = true;

      dtp_dokDate      = hampSpecifikum.CreateVvDateTimePicker(4, 0, "", tbx_dokDate);
      dtp_dokDate.Name = "dtp_dokDate";
      dtp_dokDate.Tag  = rbtSortByDokDate;

      tbx_dokDate.DoubleClick += new EventHandler(tbx_DoubleClick);

      VvHamper.Open_Close_Fields_ForWriting(tbx_TT     , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_ttNum  , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_dokDate, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);

      rbtSortByPartner = hampSpecifikum.CreateVvRadioButton(0, 1, new EventHandler(rbtSortByPartner_Click), "Od partnera:", TextImageRelation.ImageAboveText);
      tbx_Partner = hampSpecifikum.CreateVvTextBox(1, 1, "tbx_Partner", "", 32, 2, 0);
      tbx_Partner.Tag = rbtSortByPartner;
      rbtSortByPartner.Tag = tbx_Partner;
      tbx_Partner.DoubleClick += new EventHandler(tbx_DoubleClick);

      tbx_Partner.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);
      VvHamper.Open_Close_Fields_ForWriting(tbx_Partner, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);

      cbx_biloGdjeUnazivu = hampSpecifikum.CreateVvCheckBox_OLD(4, 1, CheckBox_biloGdjeUnazivu_Click, "", RightToLeft.No, true);

   }

   protected override void CreateHamperFilter()
   {
      CreateHamperOpenFilter();
      CreateHamperFilterOnlyNapomena();
   }

   private void CreateHamperFilter4Zahtjevi()
   {
      hampFilter = new VvHamper(10, 5, "", this, true, hampOpenFilter.Left, hampOpenFilter.Top, razmakHamp);
      //                                         0          1        2          3            4              5           6        7        8                 9
      hampFilter.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un, ZXC.Q3un, ZXC.Q7un, ZXC.QUN - ZXC.Qun4, ZXC.Q3un, ZXC.Q4un, ZXC.Q4un, ZXC.Q5un, ZXC.QUN - ZXC.Qun4 };
      hampFilter.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun12, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun12 };
      hampFilter.VvRightMargin = hampFilter.VvLeftMargin;

      for(int i = 0; i < hampFilter.VvNumOfRows; i++)
      {
         hampFilter.VvRowHgt[i] = ZXC.QUN;
         hampFilter.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hampFilter.VvBottomMargin = hampFilter.VvTopMargin;

      Label lblFilter = hampFilter.CreateVvLabel(0, 0, "Izlistaj samo one koji se odnose na:", 2, 0, System.Drawing.ContentAlignment.MiddleRight);

      hampFilter.CreateVvLabel(0, 1, "Napomenu:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterNapomena = hampFilter.CreateVvTextBox(1, 1, "tbx_filtNapomena", "Filter Napomena", 32, 2, 0);
      cbx_biloGdjeUnapomeni = hampFilter.CreateVvCheckBox_OLD(4, 1, null, "", RightToLeft.Yes, true);
      cbx_biloGdjeUnapomeni.Checked = true;

      //==============================================================================================================================

      hampFilter.CreateVvLabel(0, 2, "Partnera:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filtPartnerCD   = hampFilter.CreateVvTextBox(1, 2, "tbx_filtPartnerCd", "Šifra partnera");
      tbx_filtPartnerTick = hampFilter.CreateVvTextBox(2, 2, "tbx_filtPartnerTick", "Ticker partnera");
      tbx_filtPartnerName = hampFilter.CreateVvTextBox(3, 2, "tbx_filtPartnerName", "Naziv partnera");

      tbx_filtPartnerCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_filtPartnerTick.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_filtPartnerCD  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupDobTextBoxLeave));
      tbx_filtPartnerTick.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupDobTextBoxLeave));
      tbx_filtPartnerName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyKupDobTextBoxLeave));

      //--------------------------------------------------------------------------------------------------------------------------------------

      hampFilter.CreateVvLabel(0, 3, "MjTroška:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterMtrosCD = hampFilter.CreateVvTextBox(1, 3, "tbx_filterMtrosCD", "Šifra  mjesta troška");
      tbx_filterMtrosTK = hampFilter.CreateVvTextBox(2, 3, "tbx_filterMtrosTK", "Ticker mjesta troška");
      tbx_filterMtrosName = hampFilter.CreateVvTextBox(3, 3, "tbx_filterMtrosName", "Naziv  mjesta troška");

      tbx_filterMtrosCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_filterMtrosTK.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_filterMtrosCD.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_filterMtrosTK.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_filterMtrosName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyMtrosTextBoxLeave));


      hampFilter.CreateVvLabel(5, 1, "TipTrans:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterTT = hampFilter.CreateVvTextBox(6, 1, "tbx_filtTT", "TipTrans");
      tbx_filterTT.JAM_ReadOnly = true;
      // tbx_TT.JAM_PairThisWithTwin(tbx_filterTT);

      hampFilter.CreateVvLabel(5, 2, "Projekt:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterProjekt = hampFilter.CreateVvTextBox(6, 2, "tbx_Projekt", "Projekt - ");
      tbx_filterProjekt.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_filterProjekt.JAM_SetAutoCompleteData(Faktur.recordName, Faktur.sorterTtNum.SortType, ZXC.AutoCompleteRestrictor.FAK_ExactTT_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Faktur_DUMMY), null);

                         hampFilter.CreateVvLabel        (5, 3, "Status:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterStrH_32 = hampFilter.CreateVvTextBoxLookUp(6, 3, "tbx_filterStrH_32", "Status");
      tbx_filterStrH_32.JAM_Set_LookUpTable(ZXC.luiListaRiskStatus, (int)ZXC.Kolona.prva);
      tbx_filterStrH_32.JAM_lookUp_MultiSelection = true;
      tbx_filterStrH_32.JAM_CharacterCasing = CharacterCasing.Upper;

      hampFilter.CreateVvLabel(7, 1, "Odgovorni izvršitelj:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterStrA_40 = hampFilter.CreateVvTextBoxLookUp(8, 1, "tbx_filterStrA_40", "Odgovorni izvršitelj");
      tbx_filterStrA_40.JAM_Set_LookUpTable(ZXC.luiListaRiskVodPrjkt, (int)ZXC.Kolona.prva);
      
      hampFilter.CreateVvLabel(7, 2, "Voditelj projekta:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterStrG_40 = hampFilter.CreateVvTextBoxLookUp(8, 2, "tbx_filterStrG_40", "Voditelj projekta");
      tbx_filterStrG_40.JAM_Set_LookUpTable(ZXC.luiListaRiskVodPrjkt, (int)ZXC.Kolona.prva);

      VvHamper.Open_Close_Fields_ForWriting(hampFilter, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      hampFilter.Visible = false;

   }

   private void CreateHamperFilter4Putnalog()
   {
      hampFilter = new VvHamper(10, 5, "", this, true, hampOpenFilter.Left, hampOpenFilter.Top, razmakHamp);
      //                                         0          1        2          3            4              5           6        7        8                 9
      hampFilter.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un, ZXC.Q3un, ZXC.Q7un, ZXC.QUN - ZXC.Qun4, ZXC.Q3un, ZXC.Q4un,  ZXC.Q4un, ZXC.Q5un, ZXC.QUN - ZXC.Qun4};
      hampFilter.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,          ZXC.Qun12, ZXC.Qun4, ZXC.Qun4,  ZXC.Qun4, ZXC.Qun4,          ZXC.Qun12};
      hampFilter.VvRightMargin = hampFilter.VvLeftMargin;

      for(int i = 0; i < hampFilter.VvNumOfRows; i++)
      {
         hampFilter.VvRowHgt[i]    = ZXC.QUN;
         hampFilter.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hampFilter.VvBottomMargin = hampFilter.VvTopMargin;

      Label lblFilter = hampFilter.CreateVvLabel(0, 0, "Izlistaj samo one koji se odnose na:", 2, 0, System.Drawing.ContentAlignment.MiddleRight);

                              hampFilter.CreateVvLabel       (0, 1, "Napomenu:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterNapomena    = hampFilter.CreateVvTextBox     (1, 1, "tbx_filtNapomena", "Filter Napomena", 32, 2, 0);
      cbx_biloGdjeUnapomeni = hampFilter.CreateVvCheckBox_OLD(4, 1, null, "", RightToLeft.Yes, true);
      cbx_biloGdjeUnapomeni.Checked = true;

                            hampFilter.CreateVvLabel  (0, 2, "MjTroška:"          , System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterMtrosCD   = hampFilter.CreateVvTextBox(1, 2, "tbx_filterMtrosCD"  , "Šifra  mjesta troška");
      tbx_filterMtrosTK   = hampFilter.CreateVvTextBox(2, 2, "tbx_filterMtrosTK"  , "Ticker mjesta troška");
      tbx_filterMtrosName = hampFilter.CreateVvTextBox(3, 2, "tbx_filterMtrosName", "Naziv  mjesta troška");

      tbx_filterMtrosCD.JAM_CharEdits       = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_filterMtrosTK.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_filterMtrosCD  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_filterMtrosTK  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_filterMtrosName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyMtrosTextBoxLeave));

      //==============================================================================================================================

                                hampFilter.CreateVvLabel  (0, 3, "Djelatnika:"            , System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterPersonCD      = hampFilter.CreateVvTextBox(1, 3, "tbx_filterPersonCD"     , "Šifra djelatnika");
      tbx_filterPersonPrezime = hampFilter.CreateVvTextBox(2, 3, "tbx_filterPersonPrezime", "Naziv djelatnika", 40, 1, 0);
      tbx_filterPersonCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_filterPersonCD     .JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType     , new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterSifra)  , new EventHandler(AnyPersonTextBoxLeave));
      tbx_filterPersonPrezime.JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonTextBoxLeave));

      //--------------------------------------------------------------------------------------------------------------------------------------

                     hampFilter.CreateVvLabel  (5, 1, "TipTrans:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterTT = hampFilter.CreateVvTextBox(6, 1, "tbx_filtTT", "TipTrans");
      tbx_filterTT.JAM_ReadOnly = true;
     // tbx_TT.JAM_PairThisWithTwin(tbx_filterTT);

                          hampFilter.CreateVvLabel  (5, 2, "Projekt:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterProjekt = hampFilter.CreateVvTextBox(6, 2, "tbx_Projekt", "Projekt - ");
      tbx_filterProjekt.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_filterProjekt.JAM_SetAutoCompleteData(Faktur.recordName, Faktur.sorterTtNum.SortType, ZXC.AutoCompleteRestrictor.FAK_ExactTT_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Faktur_DUMMY), null);

                          hampFilter.CreateVvLabel        (5, 3, "IzvVal:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterValName = hampFilter.CreateVvTextBoxLookUp(6, 3, "tbx_filterValName", "Naziv devizne valute");
      tbx_filterValName.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_filterValName.JAM_Set_LookUpTable(ZXC.luiListaDeviza, (int)ZXC.Kolona.prva);

                            hampFilter.CreateVvLabel        (7, 1, "Odredište:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterStrC_32 = hampFilter.CreateVvTextBoxLookUp(8, 1, "tbx_odrediste",  "Odredište");
      tbx_filterStrC_32.JAM_Set_LookUpTable(ZXC.luiListaMixerOdrediste, (int)ZXC.Kolona.prva);
      tbx_filterStrC_32.JAM_lookUp_NOTobligatory = true;
      cbx_biloGdjeUstrC_32 = hampFilter.CreateVvCheckBox_OLD(9, 1, null, "", RightToLeft.Yes, true);

                         hampFilter.CreateVvLabel        (7, 2, "Vozilo:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterStrH_32 = hampFilter.CreateVvTextBoxLookUp(8, 2, "tbx_vozilo",  "Vozilo");
      tbx_filterStrH_32.JAM_Set_LookUpTable(ZXC.luiListaMixerVozilo, (int)ZXC.Kolona.prva);
      tbx_filterStrH_32.JAM_lookUp_NOTobligatory = true;
      cbx_biloGdjeUstrH_32 = hampFilter.CreateVvCheckBox_OLD(9, 2, null, "", RightToLeft.Yes, true);

      VvHamper.Open_Close_Fields_ForWriting(hampFilter, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      hampFilter.Visible = false;

   }

   private void CreateHamperFilter4Smd()
   {
      hampFilter = new VvHamper(10, 4, "", this, true, hampOpenFilter.Left, hampOpenFilter.Top, razmakHamp);
      //                                         0          1        2          3            4              5           6        7        8                 9
      hampFilter.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un, ZXC.Q3un, ZXC.Q7un, ZXC.QUN - ZXC.Qun4, ZXC.Q3un, ZXC.Q4un, ZXC.Q4un, ZXC.Q5un, ZXC.QUN - ZXC.Qun4 };
      hampFilter.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun12, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun12 };
      hampFilter.VvRightMargin = hampFilter.VvLeftMargin;

      for(int i = 0; i < hampFilter.VvNumOfRows; i++)
      {
         hampFilter.VvRowHgt[i] = ZXC.QUN;
         hampFilter.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hampFilter.VvBottomMargin = hampFilter.VvTopMargin;

      Label lblFilter = hampFilter.CreateVvLabel(0, 0, "Izlistaj samo one koji se odnose na:", 2, 0, System.Drawing.ContentAlignment.MiddleRight);

                              hampFilter.CreateVvLabel       (0, 1, "Napomenu:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterNapomena    = hampFilter.CreateVvTextBox     (1, 1, "tbx_filtNapomena", "Filter Napomena", 32, 2, 0);
      cbx_biloGdjeUnapomeni = hampFilter.CreateVvCheckBox_OLD(4, 1, null, "", RightToLeft.Yes, true);
      cbx_biloGdjeUnapomeni.Checked = true;

      //==============================================================================================================================

      hampFilter.CreateVvLabel(0, 2, "Partnera:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filtPartnerCD   = hampFilter.CreateVvTextBox(1, 2, "tbx_filtPartnerCd", "Šifra partnera");
      tbx_filtPartnerTick = hampFilter.CreateVvTextBox(2, 2, "tbx_filtPartnerTick", "Ticker partnera");
      tbx_filtPartnerName = hampFilter.CreateVvTextBox(3, 2, "tbx_filtPartnerName", "Naziv partnera");

      tbx_filtPartnerCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_filtPartnerTick.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_filtPartnerCD  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupDobTextBoxLeave));
      tbx_filtPartnerTick.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupDobTextBoxLeave));
      tbx_filtPartnerName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyKupDobTextBoxLeave));

      //--------------------------------------------------------------------------------------------------------------------------------------

      hampFilter.CreateVvLabel(0, 3, "MjTroška:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterMtrosCD = hampFilter.CreateVvTextBox(1, 3, "tbx_filterMtrosCD", "Šifra  mjesta troška");
      tbx_filterMtrosTK = hampFilter.CreateVvTextBox(2, 3, "tbx_filterMtrosTK", "Ticker mjesta troška");
      tbx_filterMtrosName = hampFilter.CreateVvTextBox(3, 3, "tbx_filterMtrosName", "Naziv  mjesta troška");

      tbx_filterMtrosCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_filterMtrosTK.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_filterMtrosCD.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_filterMtrosTK.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_filterMtrosName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyMtrosTextBoxLeave));


      hampFilter.CreateVvLabel(5, 1, "TipTrans:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterTT = hampFilter.CreateVvTextBox(6, 1, "tbx_filtTT", "TipTrans");
      tbx_filterTT.JAM_ReadOnly = true;
      // tbx_TT.JAM_PairThisWithTwin(tbx_filterTT);


      VvHamper.Open_Close_Fields_ForWriting(hampFilter, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      hampFilter.Visible = false;

   }

   private void CreateHamperFilterOnlyNapomena()
   {
      hampFilter = new VvHamper(10, 3, "", this, true, hampOpenFilter.Left, hampOpenFilter.Top, razmakHamp);
      //                                         0          1        2          3            4              5           6        7        8                 9
      hampFilter.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un, ZXC.Q3un, ZXC.Q7un, ZXC.QUN - ZXC.Qun4, ZXC.Q3un, ZXC.Q3un,  ZXC.Q4un - ZXC.Qun2, ZXC.Q6un, ZXC.QUN - ZXC.Qun4};
      hampFilter.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,          ZXC.Qun12, ZXC.Qun4, ZXC.Qun4,  ZXC.Qun4, ZXC.Qun4,          ZXC.Qun12};
      hampFilter.VvRightMargin = hampFilter.VvLeftMargin;

      for(int i = 0; i < hampFilter.VvNumOfRows; i++)
      {
         hampFilter.VvRowHgt[i]    = ZXC.QUN;
         hampFilter.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hampFilter.VvBottomMargin = hampFilter.VvTopMargin;

      Label lblFilter = hampFilter.CreateVvLabel(0, 0, "Izlistaj samo one koji se odnose na:", 2, 0, System.Drawing.ContentAlignment.MiddleRight);

                              hampFilter.CreateVvLabel       (0, 1, "Napomenu:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterNapomena    = hampFilter.CreateVvTextBox     (1, 1, "tbx_filtNapomena", "Filter Napomena", 32, 2, 0);
      cbx_biloGdjeUnapomeni = hampFilter.CreateVvCheckBox_OLD(4, 1, null, "", RightToLeft.Yes, true);
      cbx_biloGdjeUnapomeni.Checked = true;

                     hampFilter.CreateVvLabel  (5, 1, "TipTrans:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterTT = hampFilter.CreateVvTextBox(6, 1, "tbx_filtTT", "TipTrans");
      tbx_filterTT.JAM_ReadOnly = true;

      //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      tbx_filtPartnerCD       = hampFilter.CreateVvTextBox      (1, 2, "tbx_filtPartnerCd"  , "Šifra partnera");
      tbx_filtPartnerTick     = hampFilter.CreateVvTextBox      (2, 2, "tbx_filtPartnerTick", "Ticker partnera");
      tbx_filtPartnerName     = hampFilter.CreateVvTextBox      (3, 2, "tbx_filtPartnerName", "Naziv partnera");
      tbx_filterMtrosCD       = hampFilter.CreateVvTextBox      (1, 2, "tbx_filterMtrosCD"  , "Šifra  mjesta troška");
      tbx_filterMtrosTK       = hampFilter.CreateVvTextBox      (2, 2, "tbx_filterMtrosTK"  , "Ticker mjesta troška");
      tbx_filterMtrosName     = hampFilter.CreateVvTextBox      (3, 2, "tbx_filterMtrosName", "Naziv  mjesta troška");
      tbx_filterPersonCD      = hampFilter.CreateVvTextBox      (1, 2, "tbx_filterPersonCD"     , "Šifra djelatnika");
      tbx_filterPersonPrezime = hampFilter.CreateVvTextBox      (2, 2, "tbx_filterPersonPrezime", "Naziv djelatnika", 40, 1, 0);
      tbx_filterValName       = hampFilter.CreateVvTextBoxLookUp(6, 2, "tbx_filterValName", "Naziv devizne valute");
      tbx_filterStrC_32       = hampFilter.CreateVvTextBoxLookUp(8, 1, "tbx_odrediste",  "Odredište");
      cbx_biloGdjeUstrC_32    = hampFilter.CreateVvCheckBox_OLD (9, 1, null, "", RightToLeft.Yes, true);
      tbx_filterStrH_32       = hampFilter.CreateVvTextBoxLookUp(8, 2, "tbx_vozilo",  "Vozilo");
      cbx_biloGdjeUstrH_32    = hampFilter.CreateVvCheckBox_OLD (9, 2, null, "", RightToLeft.Yes, true);
      tbx_filterProjekt       = hampFilter.CreateVvTextBox      (6, 2, "tbx_Projekt", "Projekt - ");
      tbx_filterStrA_40       = hampFilter.CreateVvTextBoxLookUp(8, 2, "tbx_vozilo", "Vozilo");
      tbx_filterStrG_40       = hampFilter.CreateVvTextBoxLookUp(8, 2, "tbx_vozilo", "Vozilo");

      tbx_filterStrA_40       .Visible = 
      tbx_filterStrG_40       .Visible = 
      tbx_filtPartnerCD       .Visible = 
      tbx_filtPartnerTick     .Visible = 
      tbx_filtPartnerName     .Visible = 
      tbx_filterMtrosCD       .Visible = 
      tbx_filterMtrosTK       .Visible = 
      tbx_filterMtrosName     .Visible = 
      tbx_filterPersonCD      .Visible = 
      tbx_filterPersonPrezime .Visible = 
      tbx_filterValName       .Visible = 
      tbx_filterStrC_32       .Visible = 
      cbx_biloGdjeUstrC_32    .Visible = 
      tbx_filterStrH_32       .Visible = 
      cbx_biloGdjeUstrH_32    .Visible = 
      tbx_filterProjekt       .Visible = false;
      
      VvHamper.Open_Close_Fields_ForWriting(hampFilter, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      hampFilter.Visible = false;

   }
  
   private void CreateHamperFilter4Evidencije()
   {
      hampFilter = new VvHamper(10, 5, "", this, true, hampOpenFilter.Left, hampOpenFilter.Top, razmakHamp);
      //                                         0          1        2          3            4              5           6        7        8                 9
      hampFilter.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un, ZXC.Q3un, ZXC.Q7un, ZXC.QUN - ZXC.Qun4, ZXC.Q3un, ZXC.Q4un, ZXC.Q4un, ZXC.Q5un, ZXC.QUN - ZXC.Qun4 };
      hampFilter.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun12, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun12 };
      hampFilter.VvRightMargin = hampFilter.VvLeftMargin;

      for(int i = 0; i < hampFilter.VvNumOfRows; i++)
      {
         hampFilter.VvRowHgt[i] = ZXC.QUN;
         hampFilter.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hampFilter.VvBottomMargin = hampFilter.VvTopMargin;

      Label lblFilter = hampFilter.CreateVvLabel(0, 0, "Izlistaj samo one koji se odnose na:", 2, 0, System.Drawing.ContentAlignment.MiddleRight);

      hampFilter.CreateVvLabel(0, 1, "Napomenu:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterNapomena = hampFilter.CreateVvTextBox(1, 1, "tbx_filtNapomena", "Filter Napomena", 32, 2, 0);
      cbx_biloGdjeUnapomeni = hampFilter.CreateVvCheckBox_OLD(4, 1, null, "", RightToLeft.Yes, true);
      cbx_biloGdjeUnapomeni.Checked = true;

      //==============================================================================================================================

      hampFilter.CreateVvLabel(0, 2, "Partnera:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filtPartnerCD   = hampFilter.CreateVvTextBox(1, 2, "tbx_filtPartnerCd", "Šifra partnera");
      tbx_filtPartnerTick = hampFilter.CreateVvTextBox(2, 2, "tbx_filtPartnerTick", "Ticker partnera");
      tbx_filtPartnerName = hampFilter.CreateVvTextBox(3, 2, "tbx_filtPartnerName", "Naziv partnera");

      tbx_filtPartnerCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_filtPartnerTick.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_filtPartnerCD  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupDobTextBoxLeave));
      tbx_filtPartnerTick.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupDobTextBoxLeave));
      tbx_filtPartnerName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyKupDobTextBoxLeave));

      //--------------------------------------------------------------------------------------------------------------------------------------

      hampFilter.CreateVvLabel(0, 3, "MjTroška:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterMtrosCD = hampFilter.CreateVvTextBox(1, 3, "tbx_filterMtrosCD", "Šifra  mjesta troška");
      tbx_filterMtrosTK = hampFilter.CreateVvTextBox(2, 3, "tbx_filterMtrosTK", "Ticker mjesta troška");
      tbx_filterMtrosName = hampFilter.CreateVvTextBox(3, 3, "tbx_filterMtrosName", "Naziv  mjesta troška");

      tbx_filterMtrosCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_filterMtrosTK.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_filterMtrosCD.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_filterMtrosTK.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_filterMtrosName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyMtrosTextBoxLeave));


      hampFilter.CreateVvLabel(5, 1, "TipTrans:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterTT = hampFilter.CreateVvTextBox(6, 1, "tbx_filtTT", "TipTrans");
      tbx_filterTT.JAM_ReadOnly = true;
      // tbx_TT.JAM_PairThisWithTwin(tbx_filterTT);

      hampFilter.CreateVvLabel(5, 2, "Projekt:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterProjekt = hampFilter.CreateVvTextBox(6, 2, "tbx_Projekt", "Projekt - ");
      tbx_filterProjekt.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_filterProjekt.JAM_SetAutoCompleteData(Faktur.recordName, Faktur.sorterTtNum.SortType, ZXC.AutoCompleteRestrictor.FAK_ExactTT_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Faktur_DUMMY), null);

                         hampFilter.CreateVvLabel        (5, 3, "Status:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterStrH_32 = hampFilter.CreateVvTextBoxLookUp(6, 3, "tbx_filterStrH_32", "Status");
      tbx_filterStrH_32.JAM_Set_LookUpTable(ZXC.luiListaRiskStatus, (int)ZXC.Kolona.prva);
      tbx_filterStrH_32.JAM_lookUp_MultiSelection = true;
      tbx_filterStrH_32.JAM_CharacterCasing = CharacterCasing.Upper;

                          hampFilter.CreateVvLabel        (7, 1, "Osoba:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterStrG_40 = hampFilter.CreateVvTextBoxLookUp(8, 1, "tbx_filterStrG_40", "Osoba");
      tbx_filterStrG_40.JAM_Set_LookUpTable(ZXC.luiListaRiskVodPrjkt, (int)ZXC.Kolona.prva);

      VvHamper.Open_Close_Fields_ForWriting(hampFilter, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      hampFilter.Visible = false;

   }

   private void CreateHamperFilter4RadnoVrijemeIRV()
   {
      hampFilter = new VvHamper(10, 5, "", this, true, hampOpenFilter.Left, hampOpenFilter.Top, razmakHamp);
      //                                         0          1        2          3            4              5           6        7        8                 9
      hampFilter.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un, ZXC.Q3un, ZXC.Q7un, ZXC.QUN - ZXC.Qun4, ZXC.Q3un, ZXC.Q4un,  ZXC.Q4un, ZXC.Q5un, ZXC.QUN - ZXC.Qun4};
      hampFilter.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,          ZXC.Qun12, ZXC.Qun4, ZXC.Qun4,  ZXC.Qun4, ZXC.Qun4,          ZXC.Qun12};
      hampFilter.VvRightMargin = hampFilter.VvLeftMargin;

      for(int i = 0; i < hampFilter.VvNumOfRows; i++)
      {
         hampFilter.VvRowHgt[i]    = ZXC.QUN;
         hampFilter.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hampFilter.VvBottomMargin = hampFilter.VvTopMargin;

      Label lblFilter = hampFilter.CreateVvLabel(0, 0, "Izlistaj samo one koji se odnose na:", 2, 0, System.Drawing.ContentAlignment.MiddleRight);

                              hampFilter.CreateVvLabel       (0, 1, "Napomenu:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterNapomena    = hampFilter.CreateVvTextBox     (1, 1, "tbx_filtNapomena", "Filter Napomena", 32, 2, 0);
      cbx_biloGdjeUnapomeni = hampFilter.CreateVvCheckBox_OLD(4, 1, null, "", RightToLeft.Yes, true);
      cbx_biloGdjeUnapomeni.Checked = true;

                            hampFilter.CreateVvLabel  (0, 2, "MjTroška:"          , System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterMtrosCD   = hampFilter.CreateVvTextBox(1, 2, "tbx_filterMtrosCD"  , "Šifra  mjesta troška");
      tbx_filterMtrosTK   = hampFilter.CreateVvTextBox(2, 2, "tbx_filterMtrosTK"  , "Ticker mjesta troška");
      tbx_filterMtrosName = hampFilter.CreateVvTextBox(3, 2, "tbx_filterMtrosName", "Naziv  mjesta troška");

      tbx_filterMtrosCD.JAM_CharEdits       = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_filterMtrosTK.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_filterMtrosCD  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_filterMtrosTK  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_filterMtrosName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyMtrosTextBoxLeave));

      //==============================================================================================================================

                                hampFilter.CreateVvLabel  (0, 3, "Djelatnika:"            , System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterPersonCD      = hampFilter.CreateVvTextBox(1, 3, "tbx_filterPersonCD"     , "Šifra djelatnika");
      tbx_filterPersonPrezime = hampFilter.CreateVvTextBox(2, 3, "tbx_filterPersonPrezime", "Naziv djelatnika", 40, 1, 0);
      tbx_filterPersonCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_filterPersonCD     .JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType     , new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterSifra)  , new EventHandler(AnyPersonTextBoxLeave));
      tbx_filterPersonPrezime.JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonTextBoxLeave));

      //--------------------------------------------------------------------------------------------------------------------------------------

                     hampFilter.CreateVvLabel  (5, 1, "TipTrans:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterTT = hampFilter.CreateVvTextBox(6, 1, "tbx_filtTT", "TipTrans");
      tbx_filterTT.JAM_ReadOnly = true;
     // tbx_TT.JAM_PairThisWithTwin(tbx_filterTT);

      VvHamper.Open_Close_Fields_ForWriting(hampFilter, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      hampFilter.Visible = false;

   }

   private void CreateHamperFilter4_RUG()
   {
      hampFilter = new VvHamper(10, 5, "", this, true, hampOpenFilter.Left, hampOpenFilter.Top, razmakHamp);
      //                                         0          1        2          3            4              5           6        7        8                 9
      hampFilter.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un, ZXC.Q3un, ZXC.Q7un, ZXC.QUN - ZXC.Qun4, ZXC.Q3un, ZXC.Q4un, ZXC.Q4un, ZXC.Q5un, ZXC.QUN - ZXC.Qun4 };
      hampFilter.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun12, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun12 };
      hampFilter.VvRightMargin = hampFilter.VvLeftMargin;

      for(int i = 0; i < hampFilter.VvNumOfRows; i++)
      {
         hampFilter.VvRowHgt[i] = ZXC.QUN;
         hampFilter.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hampFilter.VvBottomMargin = hampFilter.VvTopMargin;

      Label lblFilter = hampFilter.CreateVvLabel(0, 0, "Izlistaj samo one koji se odnose na:", 2, 0, System.Drawing.ContentAlignment.MiddleRight);

      hampFilter.CreateVvLabel(0, 1, "Napomenu:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterNapomena = hampFilter.CreateVvTextBox(1, 1, "tbx_filtNapomena", "Filter Napomena", 32, 2, 0);
      cbx_biloGdjeUnapomeni = hampFilter.CreateVvCheckBox_OLD(4, 1, null, "", RightToLeft.Yes, true);
      cbx_biloGdjeUnapomeni.Checked = true;

      //==============================================================================================================================

                            hampFilter.CreateVvLabel  (0, 2, "Partnera:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filtPartnerCD   = hampFilter.CreateVvTextBox(1, 2, "tbx_filtPartnerCd", "Šifra partnera");
      tbx_filtPartnerTick = hampFilter.CreateVvTextBox(2, 2, "tbx_filtPartnerTick", "Ticker partnera");
      tbx_filtPartnerName = hampFilter.CreateVvTextBox(3, 2, "tbx_filtPartnerName", "Naziv partnera");

      tbx_filtPartnerCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_filtPartnerTick.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_filtPartnerCD  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupDobTextBoxLeave));
      tbx_filtPartnerTick.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupDobTextBoxLeave));
      tbx_filtPartnerName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyKupDobTextBoxLeave));

      //--------------------------------------------------------------------------------------------------------------------------------------

                          hampFilter.CreateVvLabel        (0, 3, "Vrsta ugovora:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterStrH_32 = hampFilter.CreateVvTextBoxLookUp("tbx_filterStrH_32", 1, 3, "Vrsta ugovora", 32, 2, 0);
      tbx_filterStrH_32.JAM_Set_LookUpTable(ZXC.luiListaVrstaUgovora, (int)ZXC.Kolona.prva);
      
      VvHamper.Open_Close_Fields_ForWriting(hampFilter, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      hampFilter.Visible = false;
   }

   private void CreateHamperFilter4Zastitari()
   {
      hampFilter = new VvHamper(10, 3, "", this, true, hampOpenFilter.Left, hampOpenFilter.Top, razmakHamp);
      //                                         0          1        2          3            4              5           6        7        8                 9
      hampFilter.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un, ZXC.Q3un, ZXC.Q7un, ZXC.QUN - ZXC.Qun4, ZXC.Q3un, ZXC.Q4un,  ZXC.Q4un, ZXC.Q5un, ZXC.QUN - ZXC.Qun4};
      hampFilter.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,          ZXC.Qun12, ZXC.Qun4, ZXC.Qun4,  ZXC.Qun4, ZXC.Qun4,          ZXC.Qun12};
      hampFilter.VvRightMargin = hampFilter.VvLeftMargin;

      for(int i = 0; i < hampFilter.VvNumOfRows; i++)
      {
         hampFilter.VvRowHgt[i]    = ZXC.QUN;
         hampFilter.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hampFilter.VvBottomMargin = hampFilter.VvTopMargin;

      Label lblFilter = hampFilter.CreateVvLabel(0, 0, "Izlistaj samo one koji se odnose na:", 2, 0, System.Drawing.ContentAlignment.MiddleRight);

                                hampFilter.CreateVvLabel  (0, 1, "Djelatnika:"            , System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterPersonCD      = hampFilter.CreateVvTextBox(1, 1, "tbx_filterPersonCD"     , "Šifra djelatnika");
      tbx_filterPersonPrezime = hampFilter.CreateVvTextBox(2, 1, "tbx_filterPersonPrezime", "Naziv djelatnika", 40, 1, 0);
      tbx_filterPersonCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_filterPersonCD     .JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType     , new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterSifra)  , new EventHandler(AnyPersonTextBoxLeave));
      tbx_filterPersonPrezime.JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonTextBoxLeave));

                              hampFilter.CreateVvLabel       (0, 2, "Napomenu:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterNapomena    = hampFilter.CreateVvTextBox     (1, 2, "tbx_filtNapomena", "Filter Napomena", 32, 2, 0);
      cbx_biloGdjeUnapomeni = hampFilter.CreateVvCheckBox_OLD(4, 2, null, "", RightToLeft.Yes, true);
      cbx_biloGdjeUnapomeni.Checked = true;


      //--------------------------------------------------------------------------------------------------------------------------------------

                     hampFilter.CreateVvLabel  (5, 1, "TipTrans:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterTT = hampFilter.CreateVvTextBox(6, 1, "tbx_filtTT", "TipTrans");
      tbx_filterTT.JAM_ReadOnly = true;
     // tbx_TT.JAM_PairThisWithTwin(tbx_filterTT);

      VvHamper.Open_Close_Fields_ForWriting(hampFilter, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      hampFilter.Visible = false;

   }

   private void SetVisibilitiSpecificsTT(string Default_TT)
   {
      if(this.Default_TT == Mixer.TT_VIRMAN || this.Default_TT == Mixer.TT_RASTERF)
      {
         tbx_filterNapomena.Visible = true;
         cbx_biloGdjeUnapomeni.Visible = true;
      }
   }

   private void SetVisibilitiSortPartner(bool isVisible)
   {
      rbtSortByPartner   .Visible = 
      tbx_Partner        .Visible = 
      cbx_biloGdjeUnazivu.Visible = isVisible;

   }

   #endregion Hamperi

   #region Eveniti

   public void rbtSortByDokDate_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt = sender as RadioButton;
      rBtcurrChecked = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Mixer.sorterDokDate;
   }

   public void rbtSortByTtNum_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt = sender as RadioButton;
      rBtcurrChecked = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Mixer.sorterTtNum;
      VvHamper.Open_Close_Fields_ForWriting(tbx_ttNum, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      DaLiJeParentFindDialog(); // za nakon Reseta
   }

   public void rbtSortByPartner_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt = sender as RadioButton;
      rBtcurrChecked = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Mixer.sorterKpdbName;

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

   private void AnyKupDobTextBoxLeave(object sender, EventArgs e)
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
            Fld_FiltKupDobName = kupdob_rec.Naziv;
            Fld_FiltKupDobCD = kupdob_rec.KupdobCD;
            Fld_FiltKupDobTick = kupdob_rec.Ticker;
         }
         else
         {
            Fld_FiltKupDobName = Fld_FiltKupDobTick = Fld_FiltKupDobCdAsTxt = "";
         }
      }
   }

   private void AnyMtrosTextBoxLeave(object sender, EventArgs e)
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
            Fld_FilterMT_sifra = kupdob_rec.KupdobCD/*RecID*/;
            Fld_FilterMT_ticker = kupdob_rec.Ticker;
            Fld_FilterMT_naziv = kupdob_rec.Naziv;
         }
         else
         {
            Fld_FilterMT_SifraAsTxt = Fld_FilterMT_ticker = Fld_FilterMT_naziv = "";
         }
      }
   }

   private void AnyPersonTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      //if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Person person_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         person_rec = PersonSifrar.Find(FoundInSifrar<Person>);

         if(person_rec != null && tb.Text != "")
         {
            Fld_FilterPersonCd      = person_rec.PersonCD/*RecID*/;
            Fld_FilterPersonPrezime = person_rec.PrezimeIme;
         }
         else
         {
            Fld_FilterPersonCdAsTx = Fld_FilterPersonPrezime = "";
         }
      }
   }

   #endregion Eveniti

   #region DataGridView

   protected override void CreateDataGridViewColumn()
   {
      //int sumOfColWidth = 0, colWidth;
      //int colDateWidth = ZXC.Q4un;
      //int colSif6Width = ZXC.Q3un + ZXC.Qun2;

      sumOfColWidth += TheGrid.RowHeadersWidth;

      if(IsArhivaTabPage)
      {
         AddDGV_ArhivaColumns(ref sumOfColWidth);
      }


      colWidth = colSif6Width; AddDGVColum_RecID_4GridReadOnly(TheGrid, "RecID", colWidth, false, 0, "recID");

      colWidth = ZXC.Q2un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "TT"      , colWidth, false,    "tt");
      colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (TheGrid, "Broj"    , colWidth, true, 6,  "ttNum");
      colWidth = ZXC.Q4un; sumOfColWidth += colWidth; AddDGVColum_DateTime_4GridReadOnly(TheGrid, "Datum"   , colWidth,           "dokDate");

      
     // colWidth = ZXC.Q9un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Partner" , colWidth, true,     "kupdobName");
     //// colWidth = ZXC.Q2un + ZXC.Qun4; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Status", colWidth, false, "ext_statusCD");

     // grid_Width = sumOfColWidth + ZXC.QUN;
      this.Default_TT = ((MixerDUC)ZXC.TheVvForm.TheVvUC).Fld_TT;
      SpecificsTTcolumns(Default_TT);

   }

   private void SpecificsTTcolumns(string Default_TT)
   {
      if(this.Default_TT == Mixer.TT_PUTN_I || this.Default_TT == Mixer.TT_PUTN_T || this.Default_TT == Mixer.TT_PUTN_L || this.Default_TT == Mixer.TT_PUTN_R)
      {
         colWidth = ZXC.Q5un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Radnik"      , colWidth, false, "personPrezim");
         colWidth = ZXC.Q5un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Reg Vozila"  , colWidth, false, "strH_32");
         colWidth = ZXC.Q5un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Marka Vozila", colWidth, false, "strG_40");
      }
      if(this.Default_TT == Mixer.TT_PUTN_I || this.Default_TT == Mixer.TT_PUTN_T ||  this.Default_TT == Mixer.TT_PUTN_R)
      {
         colWidth = ZXC.Q5un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Zadatak", colWidth, false, "strB_128");
      }
      if(this.Default_TT.StartsWith("Z") && (this.Default_TT != Mixer.TT_ZPG && this.Default_TT != Mixer.TT_ZLJ))
      {
         colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Partner"          , colWidth, false, "kupDobName");
         colWidth = ZXC.Q4un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Projekt"          , colWidth, false, "projektCd");
         colWidth = ZXC.Q5un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "OdgovIzvršitelj"  , colWidth, false, "strA_40");
         colWidth = ZXC.Q5un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Voditelj projekta", colWidth, false, "strG_40");
         colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Status"           , colWidth, false, "strH_32");
      }
      if(this.Default_TT.StartsWith("E"))
      {
         colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Partner", colWidth, false, "kupDobName");
         colWidth = ZXC.Q4un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Projekt", colWidth, false, "projektCd");
         colWidth = ZXC.Q5un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Osoba"  , colWidth, false, "strG_40");
         colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Status" , colWidth, false, "strH_32");
      }
      if(this.Default_TT == Mixer.TT_SMD)
      {
         colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Partner"          , colWidth, false, "kupDobName");
         colWidth = ZXC.Q4un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Projekt"          , colWidth, false, "projektCd");
      }
      if(this.Default_TT == Mixer.TT_IRV)
      {
         colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Prezime", colWidth, false, "personPrezim");
         colWidth = ZXC.Q4un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Ime"    , colWidth, false, "personIme");
      }
      if(this.Default_TT == Mixer.TT_RUG)
      {
         colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Partner"      , colWidth, false, "kupDobName");
         colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Vrsta ugovora", colWidth, false, "strH_32");
         colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Broj ugovora" , colWidth, false, "strF_64");
      }
      if(this.Default_TT == Mixer.TT_URZ)
      {
         colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Partner"       , colWidth, false, "kupDobName");
         colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Urudžbeni broj", colWidth, false, "strD_32");
         colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Predmet"       , colWidth, false, "strB_128");
         colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Klasa"         , colWidth, false, "strA_40");
      }
      if(this.Default_TT == Mixer.TT_NZI)
      {
         colWidth = ZXC.Q10un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Partner"       , colWidth, false, "kupDobName");
         colWidth = ZXC.Q4un - ZXC.Qun2 ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Vrsta RNM"     , colWidth, false, "strD_32");
      }
      if(this.Default_TT == Mixer.TT_ZPG || this.Default_TT == Mixer.TT_ZLJ)
      {
         colWidth = ZXC.Q5un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Radnik"      , colWidth, false, "personPrezim");
      }
      if(this.Default_TT == Mixer.TT_NAK)
      {
         colWidth = ZXC.Q10un + ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Partner", colWidth, false, "kupDobName");
      }
      if(this.Default_TT == Mixer.TT_KDC)
      {
         colWidth = ZXC.Q9un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Partner", colWidth, false, "kupDobName");
      }

      colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Napomena", colWidth, true, "napomena");
      grid_Width = sumOfColWidth + ZXC.QUN;
      //CalculateLocationAndSize();


   }

   #endregion DataGridView

   #region Fld_

   public uint       Fld_FromTtNum           { get { return tbx_ttNum.GetSomeRecIDField();         } set { tbx_ttNum.PutSomeRecIDField         (value); } }
   public string     Fld_FromTT              { get { return tbx_TT.Text;                           } set { tbx_TT                 .Text       = value;  } }
   public DateTime   Fld_FromDokDate         { get { return dtp_dokDate.Value;                     } set { dtp_dokDate            .Value      = value;  } }
   public string     Fld_FromPartner         { get { return tbx_Partner.Text;                      } set { tbx_Partner            .Text       = value;  } }
   public uint       Fld_FiltKupDobCD        { get { return tbx_filtPartnerCD.GetSomeRecIDField(); } set { tbx_filtPartnerCD.PutSomeRecIDField (value); } }
   public string     Fld_FiltKupDobCdAsTxt   { get { return tbx_filtPartnerCD.Text;                } set { tbx_filtPartnerCD      .Text       = value;  } }
   public string     Fld_FiltKupDobName      { get { return tbx_filtPartnerName.Text;              } set { tbx_filtPartnerName    .Text       = value;  } }
   public string     Fld_FiltKupDobTick      { get { return tbx_filtPartnerTick.Text;              } set { tbx_filtPartnerTick    .Text       = value;  } }
   public string     Fld_FilterTT            { get { return tbx_filterTT.Text;                     } set { tbx_filterTT           .Text       = value;  } }
   public string     Fld_FilterNapomena      { get { return tbx_filterNapomena.Text;               } set { tbx_filterNapomena     .Text       = value;  } }
   public bool       Fld_BiloGdjeUNapomena   { get { return cbx_biloGdjeUnapomeni.Checked;         } set { cbx_biloGdjeUnapomeni  .Checked    = value;  } }
   public bool       Fld_BiloGdjeUstrC_32    { get { return cbx_biloGdjeUstrC_32.Checked;          } set { cbx_biloGdjeUstrC_32 .Checked    = value;  } }
   public bool       Fld_BiloGdjeUStrH_32    { get { return cbx_biloGdjeUstrH_32.Checked;          } set { cbx_biloGdjeUstrH_32    .Checked    = value;  } }
   public string     Fld_FilterMT_ticker     { get { return tbx_filterMtrosTK.Text;                } set { tbx_filterMtrosTK      .Text       = value;  } }
   public uint       Fld_FilterMT_sifra      { get { return tbx_filterMtrosCD.GetSomeRecIDField(); } set { tbx_filterMtrosCD.PutSomeRecIDField (value); } }
   public string     Fld_FilterMT_SifraAsTxt { get { return tbx_filterMtrosCD.Text;                } set { tbx_filterMtrosCD      .Text       = value;  } }
   public string     Fld_FilterMT_naziv      { get { return tbx_filterMtrosName.Text;              } set { tbx_filterMtrosName    .Text       = value;  } }
   public uint       Fld_FilterPersonCd      { get { return tbx_filterPersonCD.GetSomeRecIDField();} set { tbx_filterPersonCD.PutSomeRecIDField(value); } }
   public string     Fld_FilterPersonCdAsTx  { get { return tbx_filterPersonCD.Text;               } set { tbx_filterPersonCD     .Text       = value;  } }
   public string     Fld_FilterPersonPrezime { get { return tbx_filterPersonPrezime.Text;          } set { tbx_filterPersonPrezime.Text       = value;  } }
   public string     Fld_FilterValName       { get { return tbx_filterValName .Text;                } set { tbx_filterValName      .Text       = value;  } }
   public string     Fld_ProjektCD           { get { return tbx_filterProjekt .Text;                } set { tbx_filterProjekt      .Text       = value;  } }
   public string     Fld_StrA_40             { get { return tbx_filterStrA_40 .Text;                } set { tbx_filterStrA_40      .Text       = value;  } }
   public string     Fld_StrB_128            { get { return tbx_filterStrB_128.Text;                } set { tbx_filterStrB_128     .Text       = value;  } }
   public string     Fld_StrC_32             { get { return tbx_filterStrC_32 .Text;                } set { tbx_filterStrC_32      .Text       = value;  } }
   public string     Fld_StrD_32             { get { return tbx_filterStrD_32 .Text;                } set { tbx_filterStrD_32      .Text       = value;  } }
   public string     Fld_StrE_256            { get { return tbx_filterStrE_256.Text;                } set { tbx_filterStrE_256     .Text       = value;  } }
   public string     Fld_StrF_64             { get { return tbx_filterStrF_64 .Text;                } set { tbx_filterStrF_64      .Text       = value;  } }
   public string     Fld_StrG_40             { get { return tbx_filterStrG_40 .Text;                } set { tbx_filterStrG_40      .Text       = value;  } }
   public string     Fld_StrH_32             { get { return tbx_filterStrH_32 .Text;                } set { tbx_filterStrH_32       .Text       = value;  } }

   #endregion Fld_

   #region Overriders and specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.mixer_rec; }
      set {        this.mixer_rec = (Mixer)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.MixerDao; }
   }

   private Vektor.DataLayer.DS_FindRecord.DS_findMixer ds_mixer;

   protected override DataSet VirtualUntypedDataSet { get { return ds_mixer; } }

   protected override object[] From_IndexSegmentValues
   {
      get
      {
         switch(recordSorter.SortType)
         {
          //case VvSQL.SorterType.DokNum  : return new object[] { Fld_FromDokNum,                   0 };
            case VvSQL.SorterType.DokDate : return new object[] { Fld_FromDokDate,             0  , 0 };
            case VvSQL.SorterType.TtNum   : return new object[] { Fld_FromTT     , Fld_FromTtNum  , 0 };
            case VvSQL.SorterType.KpdbName: return new object[] { Fld_FromTT     , Fld_FromPartner, Fld_FromTtNum, 0 };

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
   /// get { return ZXC.MixerDao.TheSchemaTable.Rows; }
   /// </summary>
   public DataRowCollection MixerSchemaRows
   {
      get { return ZXC.MixerDao.TheSchemaTable.Rows; }
   }

   /// <summary>
   ///  get { return ZXC.MixerDao.CI; }
   /// </summary>
   public MixerDao.MixerCI MixerCI
   {
      get { return ZXC.MixerDao.CI; }
   }

   public override void AddFilterMemberz()
   {
      string text, preffix;
      uint num;
      bool isCheck;

      DataRow drSchema;

      this.TheFilterMembers.Clear();


      // Fld_FiltPartnerCD                                                                                                                                          
      drSchema = MixerSchemaRows[MixerCI.kupdobCD];
      num = Fld_FiltKupDobCD;

      if(num != 0)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Partner", num, " = "));
      }

      // Fld_FilterNapomena                                                                                                                                          

      drSchema = MixerSchemaRows[MixerCI.napomena];
      text = Fld_FilterNapomena;
      isCheck = Fld_BiloGdjeUNapomena;
      preffix = isCheck ? "%" : "";

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Napomena", preffix + text + "%", " LIKE "));
      }
      
      // Fld_FiltPersonCD                                                                                                                                          
      drSchema = MixerSchemaRows[MixerCI.personCD];
      num      = Fld_FilterPersonCd;

      if(num != 0)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Radnik", num, " = "));
      }

      // Fld_FiltMT_sifra                                                                                                                                          
      drSchema = MixerSchemaRows[MixerCI.mtrosCD];
      num      = Fld_FilterMT_sifra;

      if(num != 0)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "MjTroska", num, " = "));
      }

      // Fld_strH_32    /* VoziloCD           */                                                                                                                                         
      // Fld_FilterStrH32        Status
      drSchema = MixerSchemaRows[MixerCI.strH_32];
      text = Fld_StrH_32;

      if(this.Default_TT.StartsWith("Z") && (this.Default_TT != Mixer.TT_ZPG && this.Default_TT != Mixer.TT_ZLJ))
      {

         if(text.NotEmpty())
         {
            this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "StrH32", text, " REGEXP "));
         }
         //drSchema = FaktExSchemaRows[FaktExCI.statusCD];
         //text = Fld_FilterStatus;

         //if(text.NotEmpty())
         //{
         //   this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Status", text, " REGEXP "));
         //}

      }
      else
      {
         isCheck = Fld_BiloGdjeUStrH_32;
         preffix = isCheck ? "%" : "";

         if(text.NotEmpty())
         {
            this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "StrH32", preffix + text + "%", " LIKE "));
         }
      }
      // Fld_strC_32   /* Odrediste          */                                                                                                                                  

      drSchema = MixerSchemaRows[MixerCI.strC_32];
      text     = Fld_StrC_32;
      isCheck  = Fld_BiloGdjeUstrC_32;
      preffix  = isCheck ? "%" : "";

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "StrC32", preffix + text + "%", " LIKE "));
      }

      // Fld_FilterProjektCd                                                                                                                                         

      drSchema = MixerSchemaRows[MixerCI.projektCD];
      text = Fld_ProjektCD;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "ProjektCd", text, " = "));
      }

      // Fld_FilterStrA40   odgIzvrsitelj                                                                                                                                         

      drSchema = MixerSchemaRows[MixerCI.strA_40];
      text = Fld_StrA_40;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "StrA40", text, " = "));
      }
     
      // Fld_FilterStrG40   voditeljProjekta                                                                                                                                         

      drSchema = MixerSchemaRows[MixerCI.strG_40];
      text = Fld_StrG_40;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "StrG40", text, " = "));
      }

      // Fld_FilterTT                                                                                                                                          

      drSchema = MixerSchemaRows[MixerCI.tt];
      //      text = Fld_FilterTT;
      text = Fld_FromTT;

      if(((MixerDUC)ZXC.TheVvForm.TheVvUC).Fld_TT.StartsWith("Z") && (this.Default_TT != Mixer.TT_ZPG && this.Default_TT != Mixer.TT_ZLJ))
      {
         //TheFilterMembers.Add(new VvSqlFilterMember("tt", "('ZND', 'ZNU', 'ZFA', 'ZIP', 'ZPJ', 'ZRN', 'ZSI', 'ZMO', 'ZSR', 'ZKO', 'ZPR', 'ZOR', 'ZUS')", " IN "));
         string inSetClause = VvSQL.GetInSetClause(ZXC.luiListaMixTypeZahtjev.Select(lui => lui.Cd));
         TheFilterMembers.Add(new VvSqlFilterMember("tt", inSetClause, " IN "));

         if(text.NotEmpty()) { TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "PreferredTT", text, " = ")); }
      }
      else if(((MixerDUC)ZXC.TheVvForm.TheVvUC).Fld_TT.StartsWith("E"))
      {
         //TheFilterMembers.Add(new VvSqlFilterMember("tt", "('EPO', 'EUG', 'EZP', 'EPP', 'ETP', 'EMC', 'EPG', 'EAT', 'ESV', 'EUV', 'ERD', 'EOD', 'EKO', 'EPU', 'EOS', 'EN1', 'EN2', 'EN3', 'EN4', 'EN5', 'EN6')", " IN "));
         string inSetClause = VvSQL.GetInSetClause(ZXC.luiListaMixTypeEvidencija.Select(lui => lui.Cd));
         TheFilterMembers.Add(new VvSqlFilterMember("tt", inSetClause, " IN "));

         if(text.NotEmpty()) { TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "PreferredTT", text, " = ")); }
      }
      else
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "PreferredTT", text, " = "));

      }
   }

   #endregion AddFilterMemberz()
}
