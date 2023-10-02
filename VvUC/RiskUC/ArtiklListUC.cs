using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

public class ArtiklListUC : VvRecLstUC
{
   #region Fieldz

   private RadioButton rBtSortByName , rBtSortBySifra , rBtSortByBarCode,
                       rBtSortByName2, rBtSortBySifra2, rBtcurrChecked;

   private VvTextBox tbx_artiklName, tbx_artiklCD, tbx_barCode1,
                     tbx_artiklName2_gen, tbx_artiklCD2_ATK,  //SVD to su inace artiklName2, tbx_artiklCD2,
                     tbx_skladCD, tbx_skladOpis, tbx_grupa1CD, tbx_grupa1Opis, tbx_grupa2CD, tbx_grupa2Opis, tbx_grupa3CD, tbx_grupa3Opis,
                     tbx_ts, tbx_tsOpis, tbx_pdvKat, tbx_pdvKatOpis, tbx_jedMj, tbx_konto,
                     tbx_barCode2, tbx_serNo, tbx_placement, tbx_linkArtCD, 
                     tbx_drzPorjekla, tbx_atestBr,
                     tbx_dobCd, tbx_dobNaziv, tbx_dobTick, tbx_proizvCd, tbx_proizvNaziv, tbx_proizvTick,
                     tbx_artiklName2, tbx_artiklCD2,
                     tbx_masaNettoOd, tbx_masaBrutoOd, tbx_promjerOd, tbx_povrsinaOd, tbx_zapreminaOd, tbx_duljinaOd, tbx_sirinaOd,  tbx_visinaOd, tbx_starostOd, tbx_velicinaOd,
                     tbx_masaNettoDo, tbx_masaBrutoDo, tbx_promjerDo, tbx_povrsinaDo, tbx_zapreminaDo, tbx_duljinaDo, tbx_sirinaDo,  tbx_visinaDo, tbx_starostDo, tbx_velicinaDo,
                     tbx_boja, tbx_spol, 
                     tbx_kolStOd, tbx_prNabCijOd, tbx_finStOd, tbx_izlRezervKolOd, tbx_KolStFreeOd,    
                     tbx_kolStDo, tbx_prNabCijDo, tbx_finStDo, tbx_izlRezervKolDo, tbx_KolStFreeDo,
                     tbx_cjenikVpc1Od, tbx_rucOd, tbx_VPC2Od, tbx_MPC1Od, tbx_devCOd, tbx_rabat1Od, tbx_rabat2Od, tbx_minKolOd, tbx_marzaOd,
                     tbx_cjenikVpc1Do, tbx_rucDo, tbx_VPC2Do, tbx_MPC1Do, tbx_devCDo, tbx_rabat1Do, tbx_rabat2Do, tbx_minKolDo, tbx_marzaDo,
                     tbx_zaSkladCD, tbx_zaSkladOpis, tbx_snagaOd, tbx_snagaDo;

   private CheckBox    cbx_biloGdjeUArtiklName, cbx_isWKolOnly, cbx_isStatus, cbx_ikadaUminusu;
   private Artikl      artikl_rec;
   private RadioButton rbt_akcijaDa, rbt_akcijaNe, rbt_akcijaNebitno, rbt_rashodDa, rbt_rashodNe, rbt_rashodNebitno;

   private Button btn_pckInfo;
   private VvHamper hamp_pckInfo;

   #endregion Fieldz

   #region Constructor

   public ArtiklListUC(Control parent, Artikl _artikl, VvForm.VvSubModul vvSubModul): base(parent)
   {
      this.artikl_rec = _artikl;

      this.MasterSubModulEnum = ZXC.VvSubModulEnum.ART;
      this.TheSubModul        = vvSubModul;
      this.Parent.Text        = this.TheSubModul.subModul_name;

      TheFilterMembers = new System.Collections.Generic.List<VvSqlFilterMember>(); // namoj tamo di ne treba (npr Kplan) 
      
      Artikl.sorterName.BiloGdjeU_Tekstu = Fld_BiloGdjeUArtiklName = ZXC.TheVvForm.VvPref.findArtikl.IsBiloGdjeUnazivu;
      if(recordSorter.SortType == Artikl.sorterName.SortType) recordSorter.BiloGdjeU_Tekstu = Artikl.sorterName.BiloGdjeU_Tekstu;

      rBtSortByName   .Checked = ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_naziv  ;
      rBtSortBySifra  .Checked = ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_sifra  ;
      rBtSortByBarCode.Checked = ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_barCode;

      if(ZXC.IsSvDUH)
      {
         rBtSortBySifra2.Checked = ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_sifra2;
         rBtSortByName2 .Checked = ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_naziv2;
      }

      cbx_isWKolOnly  .Checked = ZXC.TheVvForm.VvPref.findArtikl.IsWKolOnly;
      cbx_isStatus    .Checked = ZXC.TheVvForm.VvPref.findArtikl.IsStatus;

      Fld_SituacijaZaSkladCD = ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD;

      // 02.05.2022:
      Fld_IsIzuzet           = ZXC.JeliJeTakav.NIJE_TAKAV;

      SetControlForInitialFocus();

      if(ZXC.IsPCTOGO) CreateBtnPCKinfo();

   }

   #endregion Constructor

   #region InitializeFindForm

   protected override void InitializeFindFormSpecifics()
   {
      recordSorter  = Artikl.sorterName;

      this.ds_artikl = new Vektor.DataLayer.DS_FindRecord.DS_findArtikl();

      this.Name = "ArtiklListUC";
      this.Text = "ArtiklListUC";

   }

   #endregion InitializeFindForm

   #region DataGridView

   protected override void CreateDataGridViewColumn()
   {
      DataGridViewTextBoxColumn col;

      int sumOfColWidth = 0, colWidth;
  
      sumOfColWidth += TheGrid.RowHeadersWidth;

      if(IsArhivaTabPage)
      {
         AddDGV_ArhivaColumns(ref sumOfColWidth);
      }

      if(ZXC.IsSvDUHdomena)
      {
         colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Šifra"   , colWidth, false, "artiklCD"   );
         colWidth = ZXC.Q2un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Naziv"   , colWidth, true , "artiklName" );
         colWidth = ZXC.Q5un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "ATK"     , colWidth, false, "artiklCD2"  );
         colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Generika", colWidth, false, "artiklName2");

         if(ZXC.IsSvDUH_ZAHonly)//28.10.2021.
         {
            colWidth = ZXC.Q3un;
            sumOfColWidth += colWidth;
            col = AddDGVColum_Decimal_4GridReadOnly(TheGrid, "KolSt", colWidth, 0, "ext_kolFree");
            col.DefaultCellStyle.ForeColor = Color.Red;
            col.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;
         }

         colWidth = ZXC.Q2un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly (TheGrid, "Gr1"     , colWidth, false, "grupa1CD");
         colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly(TheGrid, "PrNBC"   , colWidth,     2, "ext_lastPrNabCij");

         if(!ZXC.IsSvDUH_ZAHonly)
         {
            colWidth = ZXC.Q3un;
            sumOfColWidth += colWidth;
            AddDGVColum_Decimal_4GridReadOnly(TheGrid, "KolSt", colWidth, 0, "ext_kolFree");
         }

      }
      else
      {
         colWidth = ZXC.Q7un;            sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Šifra"     , colWidth, false, "artiklCD");
         colWidth = ZXC.Q2un;            sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Naziv"     , colWidth, true, "artiklName");
         colWidth = ZXC.Q2un;            sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Tip"       , colWidth, false, "ts");
         colWidth = ZXC.Q2un;            sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Gr1"       , colWidth, false, "grupa1CD");
         // 18.04.2018: 
         //colWidth = ZXC.Q3un;          sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly(TheGrid, "VPC1"  ¸  , colWidth, 2,      "ext_preDefVpc1");
         colWidth = ZXC.Q3un;            sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly(TheGrid, "PrNBC"    , colWidth, 2, "ext_lastPrNabCij");
         colWidth = ZXC.Q3un;            sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly(TheGrid, @"RucVp%"  , colWidth, 0, "ext_RucVpc1");
         //colWidth = ZXC.Q3un;          sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly(TheGrid, "MPC1"     , colWidth, 2,      "ext_preDefMpc1");
         colWidth = ZXC.Q3un;            sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly(TheGrid, "MPC1"     , colWidth, 2, "ext_lastMalopCij");
         colWidth = ZXC.Q3un + ZXC.Qun2; sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly(TheGrid, "KnjKolSt" , colWidth, ZXC.RRD.Dsc_KolNumOfDecimalPlaces/* 0*/, "ext_kolSt");
         colWidth = ZXC.Q3un + ZXC.Qun2; sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly(TheGrid, "FizKolSt" , colWidth, ZXC.RRD.Dsc_KolNumOfDecimalPlaces/* 0*/, "ext_kolFisycal");
         colWidth = ZXC.Q3un + ZXC.Qun2; sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly(TheGrid, "Rezervac" , colWidth, ZXC.RRD.Dsc_KolNumOfDecimalPlaces/* 0*/, "ext_Kolreserve");
         colWidth = ZXC.Q3un + ZXC.Qun2; sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly(TheGrid, "RaspKolSt", colWidth, ZXC.RRD.Dsc_KolNumOfDecimalPlaces/* 0*/, "ext_kolFree");
      }
      colWidth = ZXC.Q3un;                            AddDGVColum_RecID_4GridReadOnly  (TheGrid, "RecID"   , colWidth, false, 0, "recID");

      grid_Width = sumOfColWidth + ZXC.QUN;
   }

   #endregion DataGridView

   #region Hamper
 
   private void SetControlForInitialFocus()
   {
      if(rBtSortByName.Checked)
      {
         this.ControlForInitialFocus = tbx_artiklName;
         radioButtonSortByName_Click(rBtSortByName, EventArgs.Empty);
      }
      if(rBtSortBySifra.Checked)
      {
         this.ControlForInitialFocus = tbx_artiklCD;
         radioButtonSortBySifra_Click(rBtSortBySifra, EventArgs.Empty);
      }
      if(rBtSortByBarCode.Checked)
      {
         this.ControlForInitialFocus = tbx_barCode1;
         radioButtonSortByBarcod_Click(rBtSortByBarCode, EventArgs.Empty);
      }

      if(ZXC.IsSvDUHdomena)
      {
         if(rBtSortByName2.Checked)
         {
            this.ControlForInitialFocus = tbx_artiklName2_gen;
            radioButtonSortByName2_generika_Click(rBtSortByName2, EventArgs.Empty);
         }
         if(rBtSortBySifra2.Checked)
         {
            this.ControlForInitialFocus = tbx_artiklCD2_ATK;
            radioButtonSortBySifra2_ATK_Click(rBtSortBySifra2, EventArgs.Empty);
         }
      }

   }

   protected override void CreateHamperSpecifikum()
   {
      if(ZXC.IsSvDUHdomena) 
      {
         hampSpecifikum = new VvHamper(11, 4, "", this, true, hampListaRastePada.Right + ZXC.Qun4, nextY, razmakHamp);

         //                                                   0             1          2         3        4              5                     6              7          8          9         10      
         hampSpecifikum.VvColWdt      = new int[] { ZXC.Q2un + ZXC.Qun4, ZXC.Q2un, ZXC.Q5un, ZXC.QUN, ZXC.Q5un, ZXC.QUN - ZXC.Qun8, ZXC.Q3un - ZXC.Qun2 , ZXC.Q5un, ZXC.Q2un  ,ZXC.Q2un  ,ZXC.Q4un  };
         hampSpecifikum.VvSpcBefCol   = new int[] {            ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8,          ZXC.Qun12,            ZXC.Qun12, ZXC.Qun8 , ZXC.Qun8 , ZXC.Qun8 , ZXC.Qun8  };
         hampSpecifikum.VvRightMargin = hampSpecifikum.VvLeftMargin;

         hampSpecifikum.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN  };
         hampSpecifikum.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
         hampSpecifikum.VvBottomMargin = hampSpecifikum.VvTopMargin;

         rBtSortBySifra = hampSpecifikum.CreateVvRadioButton(0, 0, new EventHandler(radioButtonSortBySifra_Click), "Od šifre:", 1, 0, TextImageRelation.ImageAboveText);
         tbx_artiklCD   = hampSpecifikum.CreateVvTextBox    (2, 0, "tbx_artiklCD", "");
         tbx_artiklCD.DoubleClick += new EventHandler(tbx_DoubleClick);
         tbx_artiklCD.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

         tbx_artiklCD.Tag   = rBtSortBySifra;
         rBtSortBySifra.Tag = tbx_artiklCD;

         rBtSortByName           = hampSpecifikum.CreateVvRadioButton (4, 0, new EventHandler(radioButtonSortByName_Click), "Od naziva:", TextImageRelation.ImageAboveText);
         cbx_biloGdjeUArtiklName = hampSpecifikum.CreateVvCheckBox_OLD(5, 0, CheckBox_biloGdjeUnazivu_Click, "", RightToLeft.No);
         tbx_artiklName          = hampSpecifikum.CreateVvTextBox     (6, 0, "tbx_artiklName", "",64, 1, 0);
         cbx_biloGdjeUArtiklName.Checked = true;
         SetVvPrefLink_AND_ToolTipText_ForGenericBiloGdjeUnazivuCheckBox(cbx_biloGdjeUArtiklName, new EventHandler(cbx_biloGdjeUArtiklName_Click_SaveToVvPref));

         rBtSortByName.Checked = true;
         tbx_artiklName.DoubleClick += new EventHandler(tbx_DoubleClick);
         tbx_artiklName.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

         tbx_artiklName.Tag      = rBtSortByName;
         rBtSortByName.Tag = tbx_artiklName;

         rBtSortBySifra2      = hampSpecifikum.CreateVvRadioButton(0, 1, new EventHandler(radioButtonSortBySifra2_ATK_Click), "Od ATK:", 1, 0, TextImageRelation.ImageAboveText);
         tbx_artiklCD2_ATK    = hampSpecifikum.CreateVvTextBox    (2, 1, "tbx_artiklCD2_ATK", "");
         tbx_artiklCD2_ATK.DoubleClick += new EventHandler(tbx_DoubleClick);
         tbx_artiklCD2_ATK.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

         tbx_artiklCD2_ATK.Tag   = rBtSortBySifra2;
         rBtSortBySifra2.Tag = tbx_artiklCD2_ATK;

         rBtSortByName2          = hampSpecifikum.CreateVvRadioButton (4, 1, new EventHandler(radioButtonSortByName2_generika_Click), "Od generike:", TextImageRelation.ImageAboveText);
       //cbx_biloGdjeUArtiklName = hampSpecifikum.CreateVvCheckBox_OLD(5, 1, CheckBox_biloGdjeUnazivu_Click, "", RightToLeft.No);
         tbx_artiklName2_gen     = hampSpecifikum.CreateVvTextBox     (6, 1, "tbx_artiklName2_gen", "",64, 1, 0);
       //cbx_biloGdjeUArtiklName.Checked = true;
       //SetVvPrefLink_AND_ToolTipText_ForGenericBiloGdjeUnazivuCheckBox(cbx_biloGdjeUArtiklName, new EventHandler(cbx_biloGdjeUArtiklName_Click_SaveToVvPref));

         rBtSortByName2.Checked = true;
         tbx_artiklName2_gen.DoubleClick += new EventHandler(tbx_DoubleClick);
         tbx_artiklName2_gen.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

         tbx_artiklName2_gen.Tag = rBtSortByName2;
         rBtSortByName2.Tag      = tbx_artiklName2_gen;

         rBtSortByBarCode = hampSpecifikum.CreateVvRadioButton(0, 2, new EventHandler(radioButtonSortByBarcod_Click), "Od Bar Koda:", 1, 0, TextImageRelation.ImageAboveText);
         tbx_barCode1    = hampSpecifikum.CreateVvTextBox     (2, 2, "tbx_barCode1", "");
         tbx_barCode1.DoubleClick += new EventHandler(tbx_DoubleClick);
         tbx_barCode1.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

         tbx_barCode1.Tag     = rBtSortByBarCode;
         rBtSortByBarCode.Tag = tbx_barCode1;

         this.ControlForInitialFocus = tbx_artiklCD;

         Label lbl_ZaSklad = hampSpecifikum.CreateVvLabel        (3, 2, "SITUACIJA ZA SKLADIŠTE", 2, 0, ContentAlignment.MiddleRight);
         tbx_zaSkladCD     = hampSpecifikum.CreateVvTextBoxLookUp(6, 2, "tbx_zaSkladCD", "");
         tbx_zaSkladOpis   = hampSpecifikum.CreateVvTextBox      (7, 2, "tbx_zaSkladOpis", "");
         tbx_zaSkladOpis.JAM_ReadOnly = true;
         tbx_zaSkladCD.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
         tbx_zaSkladCD.JAM_lui_NameTaker_JAM_Name = tbx_zaSkladOpis.JAM_Name;
         tbx_zaSkladCD.JAM_WriteOnly = true;

         cbx_isStatus   = hampSpecifikum.CreateVvCheckBox_OLD(8, 0, new EventHandler(cbx_IsStatus_Click_SaveToVvPref)  ,2 , 0, "Prikaži Stanje", RightToLeft.Yes);
         cbx_isWKolOnly = hampSpecifikum.CreateVvCheckBox_OLD(8, 1, new EventHandler(cbx_IsWKolOnly_Click_SaveToVvPref),2 , 0, "Samo Aktivni"  , RightToLeft.Yes);

                              hampSpecifikum.CreateVvLabel        (0, 3, "GrupaA:", ContentAlignment.MiddleRight);
         tbx_grupa1CD       = hampSpecifikum.CreateVvTextBoxLookUp(1, 3, "tbx_grupa1CD", "");
         tbx_grupa1Opis     = hampSpecifikum.CreateVvTextBox      (2, 3, "tbx_grupa1Opis", "");
         tbx_grupa1Opis.JAM_ReadOnly = true;
         tbx_grupa1CD.JAM_Set_LookUpTable(ZXC.luiListaGrupa1Artikla, (int)ZXC.Kolona.prva);
         tbx_grupa1CD.JAM_lui_NameTaker_JAM_Name = tbx_grupa1Opis.JAM_Name;
         tbx_grupa1CD.JAM_WriteOnly = true;

                              hampSpecifikum.CreateVvLabel        (4, 3, "GrupaB:", 1, 0, ContentAlignment.MiddleRight);
         tbx_grupa2CD       = hampSpecifikum.CreateVvTextBoxLookUp(6, 3, "tbx_grupa2CD", "");
         tbx_grupa2Opis     = hampSpecifikum.CreateVvTextBox      (7, 3, "tbx_grupa2Opis", "");
         tbx_grupa2Opis.JAM_ReadOnly = true;
         tbx_grupa2CD.JAM_Set_LookUpTable(ZXC.luiListaGrupa2Artikla, (int)ZXC.Kolona.prva);
         tbx_grupa2CD.JAM_lui_NameTaker_JAM_Name = tbx_grupa2Opis.JAM_Name;
         tbx_grupa2CD.JAM_WriteOnly = true;

                              hampSpecifikum.CreateVvLabel        ( 8, 3, ZXC.IsSPSISTdemo ? "Kat.Br." : "GrupaC:", ContentAlignment.MiddleRight);
         tbx_grupa3CD       = hampSpecifikum.CreateVvTextBoxLookUp( 9, 3, "tbx_grupa3CD", "");
         tbx_grupa3Opis     = hampSpecifikum.CreateVvTextBox      (10, 3, "tbx_grupa3Opis", "");
         tbx_grupa3Opis.JAM_ReadOnly = true;
         tbx_grupa3CD.JAM_Set_LookUpTable(ZXC.luiListaGrupa3Artikla, (int)ZXC.Kolona.prva);
         tbx_grupa3CD.JAM_lui_NameTaker_JAM_Name = tbx_grupa3Opis.JAM_Name;
         tbx_grupa3CD.JAM_WriteOnly = true;

         VvHamper.Open_Close_Fields_ForWriting(tbx_artiklCD2_ATK, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
         VvHamper.Open_Close_Fields_ForWriting(tbx_artiklName2_gen, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      }
      else
      {
         hampSpecifikum = new VvHamper(11, 3, "", this, true, hampListaRastePada.Right + ZXC.Qun4, nextY, razmakHamp);

         //                                                   0             1          2         3        4              5                     6              7          8          9         10      
         hampSpecifikum.VvColWdt      = new int[] { ZXC.Q2un + ZXC.Qun4, ZXC.Q2un, ZXC.Q5un, ZXC.QUN, ZXC.Q4un, ZXC.QUN - ZXC.Qun8, ZXC.Q3un - ZXC.Qun2 , ZXC.Q5un, ZXC.Q2un  ,ZXC.Q2un  ,ZXC.Q4un  };
         hampSpecifikum.VvSpcBefCol   = new int[] {            ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8,          ZXC.Qun12,            ZXC.Qun12, ZXC.Qun8 , ZXC.Qun8 , ZXC.Qun8 , ZXC.Qun8  };
         hampSpecifikum.VvRightMargin = hampSpecifikum.VvLeftMargin;

         hampSpecifikum.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN  };
         hampSpecifikum.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
         hampSpecifikum.VvBottomMargin = hampSpecifikum.VvTopMargin;

         rBtSortBySifra = hampSpecifikum.CreateVvRadioButton(0, 0, new EventHandler(radioButtonSortBySifra_Click), "Od šifre:", 1, 0, TextImageRelation.ImageAboveText);
         tbx_artiklCD   = hampSpecifikum.CreateVvTextBox    (2, 0, "tbx_artiklCD", "");
         tbx_artiklCD.DoubleClick += new EventHandler(tbx_DoubleClick);
         tbx_artiklCD.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

         tbx_artiklCD.Tag   = rBtSortBySifra;
         rBtSortBySifra.Tag = tbx_artiklCD;

         rBtSortByName           = hampSpecifikum.CreateVvRadioButton (4, 0, new EventHandler(radioButtonSortByName_Click), "Od naziva:", TextImageRelation.ImageAboveText);
         cbx_biloGdjeUArtiklName = hampSpecifikum.CreateVvCheckBox_OLD(5, 0, CheckBox_biloGdjeUnazivu_Click, "", RightToLeft.No);
         tbx_artiklName          = hampSpecifikum.CreateVvTextBox     (6, 0, "tbx_artiklName", "",64, 1, 0);
         cbx_biloGdjeUArtiklName.Checked = true;
         SetVvPrefLink_AND_ToolTipText_ForGenericBiloGdjeUnazivuCheckBox(cbx_biloGdjeUArtiklName, new EventHandler(cbx_biloGdjeUArtiklName_Click_SaveToVvPref));

         rBtSortByName.Checked = true;
         tbx_artiklName.DoubleClick += new EventHandler(tbx_DoubleClick);
         tbx_artiklName.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

         tbx_artiklName.Tag      = rBtSortByName;
         rBtSortByName.Tag = tbx_artiklName;


         rBtSortByBarCode = hampSpecifikum.CreateVvRadioButton(0, 1, new EventHandler(radioButtonSortByBarcod_Click), "Od Bar Koda:", 1, 0, TextImageRelation.ImageAboveText);
         tbx_barCode1    = hampSpecifikum.CreateVvTextBox     (2, 1, "tbx_barCode1", "");
         tbx_barCode1.DoubleClick += new EventHandler(tbx_DoubleClick);
         tbx_barCode1.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

         tbx_barCode1.Tag     = rBtSortByBarCode;
         rBtSortByBarCode.Tag = tbx_barCode1;

         this.ControlForInitialFocus = tbx_artiklCD;

         Label lbl_ZaSklad = hampSpecifikum.CreateVvLabel        (3, 1, "SITUACIJA ZA SKLADIŠTE", 2, 0, ContentAlignment.MiddleRight);
         tbx_zaSkladCD     = hampSpecifikum.CreateVvTextBoxLookUp(6, 1, "tbx_zaSkladCD", "");
         tbx_zaSkladOpis   = hampSpecifikum.CreateVvTextBox      (7, 1, "tbx_zaSkladOpis", "");
         tbx_zaSkladOpis.JAM_ReadOnly = true;
         tbx_zaSkladCD.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
         tbx_zaSkladCD.JAM_lui_NameTaker_JAM_Name = tbx_zaSkladOpis.JAM_Name;
         tbx_zaSkladCD.JAM_WriteOnly = true;

       //tbx_zaSkladCD.Leave       += new EventHandler(tbx_zaSkladCD_Leave_RememberTheLastUsedSkladCD);
       //tbx_zaSkladCD.TextChanged += new EventHandler(tbx_zaSkladCD_TextChanged_RememberTheLastUsedSkladCD);

         cbx_isStatus   = hampSpecifikum.CreateVvCheckBox_OLD(8, 0, new EventHandler(cbx_IsStatus_Click_SaveToVvPref)  ,2 , 0, "Prikaži Stanje", RightToLeft.Yes);
         cbx_isWKolOnly = hampSpecifikum.CreateVvCheckBox_OLD(8, 1, new EventHandler(cbx_IsWKolOnly_Click_SaveToVvPref),2 , 0, "Samo Aktivni"  , RightToLeft.Yes);

         //VvHamper.Open_Close_Fields_ForWriting(tbx_artiklCD  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
         //VvHamper.Open_Close_Fields_ForWriting(tbx_artiklName, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
         //VvHamper.Open_Close_Fields_ForWriting(tbx_barCode1  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
         //VvHamper.Open_Close_Fields_ForWriting(tbx_zaSkladCD , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

                              hampSpecifikum.CreateVvLabel        (0, 2, "GrupaA:", ContentAlignment.MiddleRight);
         tbx_grupa1CD       = hampSpecifikum.CreateVvTextBoxLookUp(1, 2, "tbx_grupa1CD", "");
         tbx_grupa1Opis     = hampSpecifikum.CreateVvTextBox      (2, 2, "tbx_grupa1Opis", "");
         tbx_grupa1Opis.JAM_ReadOnly = true;
         tbx_grupa1CD.JAM_Set_LookUpTable(ZXC.luiListaGrupa1Artikla, (int)ZXC.Kolona.prva);
         tbx_grupa1CD.JAM_lui_NameTaker_JAM_Name = tbx_grupa1Opis.JAM_Name;
         tbx_grupa1CD.JAM_WriteOnly = true;

                              hampSpecifikum.CreateVvLabel        (4, 2, "GrupaB:", 1, 0, ContentAlignment.MiddleRight);
         tbx_grupa2CD       = hampSpecifikum.CreateVvTextBoxLookUp(6, 2, "tbx_grupa2CD", "");
         tbx_grupa2Opis     = hampSpecifikum.CreateVvTextBox      (7, 2, "tbx_grupa2Opis", "");
         tbx_grupa2Opis.JAM_ReadOnly = true;
         tbx_grupa2CD.JAM_Set_LookUpTable(ZXC.luiListaGrupa2Artikla, (int)ZXC.Kolona.prva);
         tbx_grupa2CD.JAM_lui_NameTaker_JAM_Name = tbx_grupa2Opis.JAM_Name;
         tbx_grupa2CD.JAM_WriteOnly = true;

                              hampSpecifikum.CreateVvLabel        ( 8, 2, ZXC.IsSPSISTdemo ? "Kat.Br." : "GrupaC:", ContentAlignment.MiddleRight);
         tbx_grupa3CD       = hampSpecifikum.CreateVvTextBoxLookUp( 9, 2, "tbx_grupa3CD", "");
         tbx_grupa3Opis     = hampSpecifikum.CreateVvTextBox      (10, 2, "tbx_grupa3Opis", "");
         tbx_grupa3Opis.JAM_ReadOnly = true;
         tbx_grupa3CD.JAM_Set_LookUpTable(ZXC.luiListaGrupa3Artikla, (int)ZXC.Kolona.prva);
         tbx_grupa3CD.JAM_lui_NameTaker_JAM_Name = tbx_grupa3Opis.JAM_Name;
         tbx_grupa3CD.JAM_WriteOnly = true;
      }

      VvHamper.Open_Close_Fields_ForWriting(tbx_artiklCD  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_artiklName, ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_barCode1  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_zaSkladCD , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvFindDialog);

      VvHamper.Open_Close_Fields_ForWriting(tbx_grupa1CD, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_grupa2CD, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_grupa3CD, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

   }

   private void CreateBtnPCKinfo()
   {
      hamp_pckInfo = new VvHamper(1, 1, "", this, false);

      hamp_pckInfo.VvColWdt      = new int[] { ZXC.QunBtnW };
      hamp_pckInfo.VvSpcBefCol   = new int[] { ZXC.Qun8 };
      hamp_pckInfo.VvRightMargin = hamp_pckInfo.VvLeftMargin;

      hamp_pckInfo.VvRowHgt       = new int[] { ZXC.QunBtnH };
      hamp_pckInfo.VvSpcBefRow    = new int[] { ZXC.Qun2    };
      hamp_pckInfo.VvBottomMargin = hamp_pckInfo.VvTopMargin;

      btn_pckInfo = hamp_pckInfo.CreateVvButton(0, 0, new EventHandler(btn_PCKinfo_Click), "PCK Info");

      hamp_pckInfo.Location = new Point(hampIzlistaj.Left, hampIzlistaj.Bottom);
      hamp_pckInfo.Anchor = AnchorStyles.Top | AnchorStyles.Right;

   }

   #endregion Hamper

   #region Eveniti
   private void radioButtonSortByName_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Artikl.sorterName;

      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_naziv   =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_naziv2  =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_sifra   =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_sifra2  =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_barCode = false;

      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_naziv   = true;
   }

   private void radioButtonSortByName2_generika_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Artikl.sorterName2;

      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_naziv   =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_naziv2  =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_sifra   =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_sifra2  =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_barCode = false;

      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_naziv2  = true;
   }
    private void radioButtonSortBySifra_Click(object sender, System.EventArgs e)
    {
       RadioButton rbt   = sender as RadioButton;
       rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
       this.recordSorter = Artikl.sorterCD;
        
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_naziv   =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_naziv2  =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_sifra   =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_sifra2  =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_barCode = false;

      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_sifra   = true;
    }
  
    private void radioButtonSortBySifra2_ATK_Click(object sender, System.EventArgs e)
    {
    RadioButton rbt   = sender as RadioButton;
    rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
    this.recordSorter = Artikl.sorterCD2;
     
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_naziv   =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_naziv2  =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_sifra   =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_sifra2  =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_barCode = false;

      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_sifra2  = true;
   }

   private void radioButtonSortByBarcod_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Artikl.sorterBCode;
 
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_naziv   =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_naziv2  =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_sifra   =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_sifra2  =
      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_barCode = false;

      ZXC.TheVvForm.VvPref.findArtikl.IsFindBy_barCode = true;
}

   private void tbx_DoubleClick(object sender, System.EventArgs e)
   {
      VvTextBox vvTb  = sender as VvTextBox;
      RadioButton rbt = (RadioButton)vvTb.Tag;

      if (!rbt.Checked)
           rbt.PerformClick();
   }
   
   public void cbx_biloGdjeUArtiklName_Click_SaveToVvPref(object sender, EventArgs e)
   {
      ZXC.TheVvForm.VvPref.findArtikl.IsBiloGdjeUnazivu = Artikl.sorterName.BiloGdjeU_Tekstu = Fld_BiloGdjeUArtiklName;
   }

   private void cbx_IsWKolOnly_Click_SaveToVvPref(object sender, EventArgs e)
   {
      ZXC.TheVvForm.VvPref.findArtikl.IsWKolOnly = Fld_IsAktivOnly;
   }
   private void cbx_IsStatus_Click_SaveToVvPref(object sender, EventArgs e)
   {
      ZXC.TheVvForm.VvPref.findArtikl.IsStatus = Fld_IsShowSomeOfStatusData;
   }

   public void btn_PCKinfo_Click(object sender, EventArgs e)
   {
      int currRowIdx = TheGrid.CurrentRow.Index;

      string currArtiklCD = TheGrid.GetStringCell(0, currRowIdx, false);
      string artiklTS     = TheGrid.GetStringCell(2, currRowIdx, false);

      if(currArtiklCD.NotEmpty() && artiklTS == ZXC.PCK_TS)
      {
         List<PCK_ArtiklInfo_Line> PCK_ArtiklInfo_List = RtranoDao.Get_PCK_ArtiklInfo_List_ForArtiklAndSklad(TheDbConnection, currArtiklCD, Fld_SituacijaZaSkladCD);

         PCK_ArtiklInfo_Dlg pckDaoDlg = new PCK_ArtiklInfo_Dlg();
         pckDaoDlg.TheUC.PutDgvFields(PCK_ArtiklInfo_List);
         pckDaoDlg.ShowDialog();
         pckDaoDlg.Dispose();
      }
   }

   #endregion Eveniti

   #region HamperFilter

   protected override void CreateHamperFilter()
   {
      CreateHamperOpenFilter();

    //hampFilter = new VvHamper(16, 11, "", this, true, hampOpenFilter.Left, hampOpenFilter.Top + ZXC.Qun8, razmakHamp);
      hampFilter = new VvHamper(16, 11, "", this, true, hampOpenFilter.Left, hampSpecifikum != null ? hampSpecifikum.Bottom :  hampOpenFilter.Top + ZXC.Qun8, razmakHamp);

      //                                        0                1                      2              3              4                  5              6         7        8             9                   10                  11                     12                      13                  14             15
      hampFilter.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un - ZXC.Qun2, ZXC.Q2un + ZXC.Qun8 , ZXC.Q3un, ZXC.Q2un +ZXC.Qun4, ZXC.Q2un +ZXC.Qun4, ZXC.Q3un, ZXC.Q2un, ZXC.Q2un, ZXC.Q3un+ZXC.Qun2, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun2 , ZXC.Q3un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un };
      hampFilter.VvSpcBefCol   = new int[] { ZXC.Qun8,            ZXC.Qun8,            ZXC.Qun12, ZXC.Qun8,           ZXC.Qun8,          ZXC.Qun12, ZXC.Qun8, ZXC.Qun8, ZXC.Qun12,          ZXC.Qun8,            ZXC.Qun8,            ZXC.Qun8 ,             ZXC.Qun8,            ZXC.Qun8,            ZXC.Qun8, ZXC.QUN   };
      hampFilter.VvRightMargin = hampFilter.VvLeftMargin;

      for(int i = 0; i < hampFilter.VvNumOfRows; i++)
      {
         hampFilter.VvRowHgt[i]    = ZXC.QUN;
         hampFilter.VvSpcBefRow[i] = ZXC.Qun8;
      } 
      hampFilter.VvBottomMargin = hampFilter.VvTopMargin;

      hampFilter.CreateVvLabel(0, 0, "IZLISTAJ SAMO:", System.Drawing.ContentAlignment.MiddleRight);

                     hampFilter.CreateVvLabel        (0, 1, "Tip:", ContentAlignment.MiddleRight);
      tbx_ts       = hampFilter.CreateVvTextBoxLookUp(1, 1, "tbx_ts", "Tip (kategorija) artikla roba, mater, vlproizv, usluga, ambalaza, uzorak, prolazna stavka, takse");
      tbx_tsOpis   = hampFilter.CreateVvTextBox      (2, 1, "tbx_tsOpis", "");
      tbx_tsOpis.JAM_ReadOnly = true;
      tbx_ts.JAM_Set_LookUpTable(ZXC.luiListaArtiklTS, (int)ZXC.Kolona.prva);
      tbx_ts.JAM_lui_NameTaker_JAM_Name = tbx_tsOpis.JAM_Name;

      //                     hampFilter.CreateVvLabel        (0, 2, "Grupa A:", ContentAlignment.MiddleRight);
      //tbx_grupa1CD       = hampFilter.CreateVvTextBoxLookUp(1, 2, "tbx_grupa1CD", "");
      //tbx_grupa1Opis     = hampFilter.CreateVvTextBox      (2, 2, "tbx_grupa1Opis", "");
      //tbx_grupa1Opis.JAM_ReadOnly = true;
      //tbx_grupa1CD.JAM_Set_LookUpTable(ZXC.luiListaGrupa1Artikla, (int)ZXC.Kolona.prva);
      //tbx_grupa1CD.JAM_lui_NameTaker_JAM_Name = tbx_grupa1Opis.JAM_Name;

                          hampFilter.CreateVvLabel        (0, 3, "Uob Sklad:", ContentAlignment.MiddleRight);
      tbx_skladCD       = hampFilter.CreateVvTextBoxLookUp(1, 3, "tbx_skladCD", "");
      tbx_skladOpis     = hampFilter.CreateVvTextBox      (2, 3, "tbx_skladOpis", "");
      tbx_skladOpis.JAM_ReadOnly = true;
      tbx_skladCD.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_skladCD.JAM_lui_NameTaker_JAM_Name = tbx_skladOpis.JAM_Name;

                         hampFilter.CreateVvLabel        (0, 4, "PDV razred:", ContentAlignment.MiddleRight);
      tbx_pdvKat       = hampFilter.CreateVvTextBoxLookUp(1, 4, "tbx_pdvKat", "");
      tbx_pdvKatOpis   = hampFilter.CreateVvTextBox      (2, 4, "tbx_pdvKatOpis", "PDV razred (npr 23%, 10%, 0%, oslob, ...)");
      tbx_pdvKatOpis.JAM_ReadOnly = true;
      tbx_pdvKat.JAM_Set_LookUpTable(ZXC.luiListaPdvKat, (int)ZXC.Kolona.prva);
      tbx_pdvKat.JAM_lui_NameTaker_JAM_Name = tbx_pdvKatOpis.JAM_Name;

      //                     hampFilter.CreateVvLabel        (0, 5, "Grupa B:", ContentAlignment.MiddleRight);
      //tbx_grupa2CD       = hampFilter.CreateVvTextBoxLookUp(1, 5, "tbx_grupa2CD", "");
      //tbx_grupa2Opis     = hampFilter.CreateVvTextBox      (2, 5, "tbx_grupa2Opis", "");
      //tbx_grupa2Opis.JAM_ReadOnly = true;
      //tbx_grupa2CD.JAM_Set_LookUpTable(ZXC.luiListaGrupa2Artikla, (int)ZXC.Kolona.prva);
      //tbx_grupa2CD.JAM_lui_NameTaker_JAM_Name = tbx_grupa2Opis.JAM_Name;

      //                     hampFilter.CreateVvLabel        (0, 6, "Grupa C:", ContentAlignment.MiddleRight);
      //tbx_grupa3CD       = hampFilter.CreateVvTextBoxLookUp(1, 6, "tbx_grupa3CD", "");
      //tbx_grupa3Opis     = hampFilter.CreateVvTextBox      (2, 6, "tbx_grupa3Opis", "");
      //tbx_grupa3Opis.JAM_ReadOnly = true;
      //tbx_grupa3CD.JAM_Set_LookUpTable(ZXC.luiListaGrupa3Artikla, (int)ZXC.Kolona.prva);
      //tbx_grupa3CD.JAM_lui_NameTaker_JAM_Name = tbx_grupa3Opis.JAM_Name;

                  hampFilter.CreateVvLabel  (0, 7, "Serijski broj:", ContentAlignment.MiddleRight);
      tbx_serNo = hampFilter.CreateVvTextBox(1, 7, "tbx_serNo", "Serijski broj", 32, 1, 0);

                      hampFilter.CreateVvLabel  (0, 8, "Smještaj:", ContentAlignment.MiddleRight);
      tbx_placement = hampFilter.CreateVvTextBox(1, 8, "tbx_placement", "Smještaj", 32, 1, 0);

                        hampFilter.CreateVvLabel  (0, 9, "Made In:", ContentAlignment.MiddleRight);
      tbx_drzPorjekla = hampFilter.CreateVvTextBox(1, 9, "tbx_drzPorjekla", "DrzPorjekla", 32, 1, 0);

      cbx_ikadaUminusu = hampFilter.CreateVvCheckBox_OLD(3, 0, null, 2, 0, "Ikada u minusu", RightToLeft.Yes);

                        hampFilter.CreateVvLabel  (3, 1, ZXC.IsSvDUHdomena ? "Generika:" : "Naziv2:", ContentAlignment.MiddleRight);
      tbx_artiklName2 = hampFilter.CreateVvTextBox(4, 1, "tbx_artiklName2", "Naziv artikla2", 32, 1, 0);

                      hampFilter.CreateVvLabel  (3, 2, ZXC.IsSvDUHdomena ? "ATK:" : "Šifra2:", ContentAlignment.MiddleRight);
      tbx_artiklCD2 = hampFilter.CreateVvTextBox(4, 2, "tbx_artiklCD2", "Šifra artikla2", 32, 1, 0);

                     hampFilter.CreateVvLabel  (3, 3, "BarKod2", ContentAlignment.MiddleRight);
      tbx_barCode2 = hampFilter.CreateVvTextBox(4, 3, "tbx_barCode2", "BarCode2", 32, 1, 0);

                      hampFilter.CreateVvLabel  (3, 4, "VezniArtikl:", ContentAlignment.MiddleRight);
      tbx_linkArtCD = hampFilter.CreateVvTextBox(4, 4, "tbx_linkArtCD", "Vezni artikl", 32, 1, 0);

                     hampFilter.CreateVvLabel  (3, 5, "ZaDobav:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_dobCd    = hampFilter.CreateVvTextBox(4, 5, "tbx_dobCd"     , "Šifra dobavljaca");
      tbx_dobTick  = hampFilter.CreateVvTextBox(5, 5, "tbx_dobTick"   , "Ticker dobavljaca");
      tbx_dobNaziv = hampFilter.CreateVvTextBox(4, 6, "tbx_dobNaziv"  , "Naziv dobavljaca", 32, 1, 0);

      tbx_dobCd.JAM_CharEdits   = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_dobTick.JAM_CharacterCasing = CharacterCasing.Upper;

      //==============================================================================================================================

      tbx_dobCd   .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD   .SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyDobTextBoxLeave));
      tbx_dobTick .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyDobTextBoxLeave));
      tbx_dobNaziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv .SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyDobTextBoxLeave));

      //==============================================================================================================================

                        hampFilter.CreateVvLabel  (3, 7, "ZaProizvođ:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_proizvCd    = hampFilter.CreateVvTextBox(4, 7, "tbx_proizvCd"   , "Šifra proizvođača");
      tbx_proizvTick  = hampFilter.CreateVvTextBox(5, 7, "tbx_proizvTick" , "Ticker proizvođača");
      tbx_proizvNaziv = hampFilter.CreateVvTextBox(4, 8, "tbx_proizvNaziv", "Naziv proizvođača", 32, 1, 0);

      tbx_proizvCd.JAM_CharEdits   = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_proizvTick.JAM_CharacterCasing = CharacterCasing.Upper;

      //==============================================================================================================================

      tbx_proizvCd   .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD   .SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyProizvTextBoxLeave));
      tbx_proizvTick .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyProizvTextBoxLeave));
      tbx_proizvNaziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv .SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyProizvTextBoxLeave));

      //==============================================================================================================================

                          hampFilter.CreateVvLabel  (3, 9, "Atest Broj:", ContentAlignment.MiddleRight);
      tbx_atestBr       = hampFilter.CreateVvTextBox(4, 9, "tbx_atestBr", "", 32, 1, 0);


      hampFilter.CreateVvLabel(6, 0, "MasaNetto:", System.Drawing.ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(6, 1, "MasaBruto:", System.Drawing.ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(6, 2, "Promjer:"  , System.Drawing.ContentAlignment.MiddleRight); 
      hampFilter.CreateVvLabel(6, 3, "Površina:" , System.Drawing.ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(6, 4, "Zapremina:", System.Drawing.ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(6, 5, "Duljina:"  , System.Drawing.ContentAlignment.MiddleRight); 
      hampFilter.CreateVvLabel(6, 6, "Širina:"   , System.Drawing.ContentAlignment.MiddleRight); 
      hampFilter.CreateVvLabel(6, 7, "Visina:"   , System.Drawing.ContentAlignment.MiddleRight); 
      hampFilter.CreateVvLabel(6, 8, "Starost:"  , System.Drawing.ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(6, 9, "Veličina:" , System.Drawing.ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(6,10, "Snaga:"    , System.Drawing.ContentAlignment.MiddleRight);

      tbx_masaNettoOd     = hampFilter.CreateVvTextBox(7, 0, "tbx_masaNettoOd", "_masaNettoOd", 12);
      tbx_masaNettoDo     = hampFilter.CreateVvTextBox(8, 0, "tbx_masaNettoDo", "_masaNettoDo", 12);
      tbx_masaBrutoOd     = hampFilter.CreateVvTextBox(7, 1, "tbx_masaBrutoOd", "_masaBrutoOd", 12);
      tbx_masaBrutoDo     = hampFilter.CreateVvTextBox(8, 1, "tbx_masaBrutoDo", "_masaBrutoDo", 12);
      tbx_promjerOd       = hampFilter.CreateVvTextBox(7, 2, "tbx_promjerOd"  , "_promjerOd"  , 12); 
      tbx_promjerDo       = hampFilter.CreateVvTextBox(8, 2, "tbx_promjerDo"  , "_promjerDo"  , 12); 
      tbx_povrsinaOd      = hampFilter.CreateVvTextBox(7, 3, "tbx_povrsinaOd" , "_povrsinaOd" , 12);
      tbx_povrsinaDo      = hampFilter.CreateVvTextBox(8, 3, "tbx_povrsinaDo" , "_povrsinaDo" , 12);
      tbx_zapreminaOd     = hampFilter.CreateVvTextBox(7, 4, "tbx_zapreminaOd", "_zapreminaOd", 12);
      tbx_zapreminaDo     = hampFilter.CreateVvTextBox(8, 4, "tbx_zapreminaDo", "_zapreminaDo", 12);
      tbx_duljinaOd       = hampFilter.CreateVvTextBox(7, 5, "tbx_duljinaOd"  , "_duljinaOd"  , 12); 
      tbx_duljinaDo       = hampFilter.CreateVvTextBox(8, 5, "tbx_duljinaDo"  , "_duljinaDo"  , 12); 
      tbx_sirinaOd        = hampFilter.CreateVvTextBox(7, 6, "tbx_sirinaOd"   , "_sirinaOd"   , 12); 
      tbx_sirinaDo        = hampFilter.CreateVvTextBox(8, 6, "tbx_sirinaDo"   , "_sirinaDo"   , 12); 
      tbx_visinaOd        = hampFilter.CreateVvTextBox(7, 7, "tbx_visinaOd"   , "_visinaOd"   , 12); 
      tbx_visinaDo        = hampFilter.CreateVvTextBox(8, 7, "tbx_visinaDo"   , "_visinaDo"   , 12); 
      tbx_starostOd       = hampFilter.CreateVvTextBox(7, 8, "tbx_starostOd"  , "_starostOd"  , 12);
      tbx_starostDo       = hampFilter.CreateVvTextBox(8, 8, "tbx_starostDo"  , "_starostDo"  , 12);
      tbx_velicinaOd      = hampFilter.CreateVvTextBox(7, 9, "tbx_velicinaOd" , "_velicinaOd" , 12);
      tbx_velicinaDo      = hampFilter.CreateVvTextBox(8, 9, "tbx_velicinaDo" , "_velicinaDo" , 12);
      tbx_snagaOd         = hampFilter.CreateVvTextBox(7,10, "tbx_snagaOd"    , "_snagaOd"    , 12);
      tbx_snagaDo         = hampFilter.CreateVvTextBox(8,10, "tbx_snagaDo"    , "_snagaDo"    , 12);

      tbx_masaNettoOd.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_masaBrutoOd.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_promjerOd  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_povrsinaOd .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_zapreminaOd.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_duljinaOd  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_sirinaOd   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_visinaOd   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_starostOd  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_velicinaOd .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_masaNettoDo.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_masaBrutoDo.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_promjerDo  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_povrsinaDo .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_zapreminaDo.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_duljinaDo  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_sirinaDo   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_visinaDo   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_starostDo  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_velicinaDo .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_snagaOd    .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_snagaDo    .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);


                        hampFilter.CreateVvLabel  ( 9, 0, "Konto:", ContentAlignment.MiddleRight);
      tbx_konto       = hampFilter.CreateVvTextBox(10, 0, "tbx_konto", "Konto");
      tbx_konto.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_konto.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
     
                        hampFilter.CreateVvLabel  ( 9, 1, "JM:", ContentAlignment.MiddleRight);
      tbx_jedMj       = hampFilter.CreateVvTextBox(10, 1, "tbx_jedMj", "Jedinica mjere");

                        hampFilter.CreateVvLabel  ( 9, 2, "Boja:", ContentAlignment.MiddleRight);
      tbx_boja       = hampFilter.CreateVvTextBox(10, 2, "tbx_boja", "Boja");

                       hampFilter.CreateVvLabel  ( 9, 3, "Spol:", ContentAlignment.MiddleRight);
      tbx_spol       = hampFilter.CreateVvTextBox(10, 3, "tbx_spol", "Spol");

      hampFilter.CreateVvLabel(9, 4, "TRENUTNO STANJE", 2, 0, ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(9, 5, "Količina:"   , ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(9, 6, "PrNabCij:"   , ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(9, 7, "Vrijednost:" , ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(9, 8, "Rezervacije:", ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(9, 9, "Raspoloživo:", ContentAlignment.MiddleRight);

      tbx_kolStOd        = hampFilter.CreateVvTextBox (10, 5, "tbx_kolStOd"       , "Stanje zaliha - financijski saldo", 12);
      tbx_kolStDo        = hampFilter.CreateVvTextBox (11, 5, "tbx_kolStDo"       , "Stanje zaliha - financijski saldo", 12);
      tbx_prNabCijOd     = hampFilter.CreateVvTextBox (10, 6, "tbx_prNabCijOd"    , "Stanje zaliha - prosječna cijena", 12);
      tbx_prNabCijDo     = hampFilter.CreateVvTextBox (11, 6, "tbx_prNabCijDo"    , "Stanje zaliha - prosječna cijena", 12);
      tbx_finStOd        = hampFilter.CreateVvTextBox (10, 7, "tbx_finStOd"       , "Stanje zaliha - robno", 12);
      tbx_finStDo        = hampFilter.CreateVvTextBox (11, 7, "tbx_finStDo"       , "Stanje zaliha - robno", 12);
      tbx_izlRezervKolOd = hampFilter.CreateVvTextBox (10, 8, "tbx_izlRezervKolOd", "Količina obvezujuce ponude", 12);
      tbx_izlRezervKolDo = hampFilter.CreateVvTextBox (11, 8, "tbx_izlRezervKolDo", "Količina obvezujuce ponude", 12);
      tbx_KolStFreeOd    = hampFilter.CreateVvTextBox (10, 9, "tbx_KolStFreeOd"   , "Raspoloživa količina ", 12);
      tbx_KolStFreeDo    = hampFilter.CreateVvTextBox (11, 9, "tbx_KolStFreeDo"   , "Raspoloživa količina ", 12);

      tbx_kolStOd       .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_prNabCijOd    .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_finStOd       .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_izlRezervKolOd.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_KolStFreeOd   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_kolStDo       .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_prNabCijDo    .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_finStDo       .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_izlRezervKolDo.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_KolStFreeDo   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      hampFilter.CreateVvLabel(12,  0, "ZADANO" , 1, 0, ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(12,  1, "VPC1:"  , ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(12,  2, "RUC %:" , ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(12,  3, "VPC2:"  , ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(12,  4, "MPC:"   , ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(12,  5, "DevC:"  , ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(12,  6, "Rabat1:", ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(12,  7, "Rabat2:", ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(12,  8, "MinKol:", ContentAlignment.MiddleRight);
      hampFilter.CreateVvLabel(12,  9, "Marža:" , ContentAlignment.MiddleRight);


      tbx_cjenikVpc1Od = hampFilter.CreateVvTextBox (13, 1, "tbx_cjenikVpc1Od" , "", 12);
      tbx_cjenikVpc1Do = hampFilter.CreateVvTextBox (14, 1, "tbx_cjenikVpc1Do" , "", 12);
      tbx_rucOd        = hampFilter.CreateVvTextBox (13, 2, "tbx_rucOd"        , "", 12);
      tbx_rucDo        = hampFilter.CreateVvTextBox (14, 2, "tbx_rucDo"        , "", 12);
      tbx_VPC2Od       = hampFilter.CreateVvTextBox (13, 3, "tbx_VPC2Od"       , "", 12);
      tbx_VPC2Do       = hampFilter.CreateVvTextBox (14, 3, "tbx_VPC2Do"       , "", 12);
      tbx_MPC1Od       = hampFilter.CreateVvTextBox (13, 4, "tbx_MPC1Od"       , "", 12);
      tbx_MPC1Do       = hampFilter.CreateVvTextBox (14, 4, "tbx_MPC1Do"       , "", 12);
      tbx_devCOd       = hampFilter.CreateVvTextBox (13, 5, "tbx_devCOd"       , "", 12);
      tbx_devCDo       = hampFilter.CreateVvTextBox (14, 5, "tbx_devCDo"       , "", 12);
      tbx_rabat1Od     = hampFilter.CreateVvTextBox (13, 6, "tbx_rabat1O"      , "", 12);
      tbx_rabat1Do     = hampFilter.CreateVvTextBox (14, 6, "tbx_rabat1O"      , "", 12);
      tbx_rabat2Od     = hampFilter.CreateVvTextBox (13, 7, "tbx_rabat2O"      , "", 12);
      tbx_rabat2Do     = hampFilter.CreateVvTextBox (14, 7, "tbx_rabat2O"      , "", 12);
      tbx_minKolOd     = hampFilter.CreateVvTextBox (13, 8, "tbx_minKolO"      , "", 12);
      tbx_minKolDo     = hampFilter.CreateVvTextBox (14, 8, "tbx_minKolO"      , "", 12);
      tbx_marzaOd      = hampFilter.CreateVvTextBox (13, 9, "tbx_marzaOd"      , "", 12);
      tbx_marzaDo      = hampFilter.CreateVvTextBox (14, 9, "tbx_marzaDo"      , "", 12);

      tbx_cjenikVpc1Od.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_rucOd       .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_VPC2Od      .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_MPC1Od      .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_devCOd      .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_rabat1Od    .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_rabat2Od    .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_minKolOd    .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_marzaOd     .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_cjenikVpc1Do.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_rucDo       .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_VPC2Do      .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_MPC1Do      .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_devCDo      .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_rabat1Do    .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_rabat2Do    .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_minKolDo    .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_marzaDo     .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      
                          hampFilter.CreateVvLabel      (15, 0, "Na akciji:", System.Drawing.ContentAlignment.MiddleLeft);
      rbt_akcijaNebitno = hampFilter.CreateVvRadioButton(15, 1, null, "Svi" , TextImageRelation.ImageBeforeText);
      rbt_akcijaDa      = hampFilter.CreateVvRadioButton(15, 2, null, "Da"  , TextImageRelation.ImageBeforeText);
      rbt_akcijaNe      = hampFilter.CreateVvRadioButton(15, 3, null, "Ne"  , TextImageRelation.ImageBeforeText);
      rbt_akcijaNebitno.Checked = true;
      rbt_akcijaNebitno.Tag     = true;

      Label lbl_izuzet = hampFilter.CreateVvLabel(15, 5, "Izuzet:", System.Drawing.ContentAlignment.MiddleLeft);

      Panel panel = new Panel();
      panel.Parent = hampFilter;
      panel.Location = new Point(rbt_akcijaNe.Left, lbl_izuzet.Bottom);

      rbt_rashodNebitno = CreateRadioButton(panel, "Svi",  true, ZXC.Qun10, ZXC.Qun8);
      rbt_rashodDa      = CreateRadioButton(panel, "Da" , false, ZXC.Qun10, ZXC.QUN + ZXC.Qun4);
      rbt_rashodNe      = CreateRadioButton(panel, "Ne" , false, ZXC.Qun10, ZXC.Q2un + ZXC.Qun4);
      rbt_rashodNebitno.Tag = true;

      panel.Size = new Size(ZXC.Q3un, ZXC.Q4un);
      panel.BringToFront();

      VvHamper.Open_Close_Fields_ForWriting(hampFilter, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      hampFilter.Visible = false;

   }
   
   private RadioButton CreateRadioButton(Panel panel, string text, bool isChecked, int x, int y)
   {
      RadioButton rbt = new RadioButton();
      rbt.Parent      = panel;
      rbt.Location    = new System.Drawing.Point(x,y);
      rbt.Text        = text;
      rbt.Checked     = isChecked;
      rbt.Name        = text;
      rbt.Font        = ZXC.vvFont.SmallFont;
      rbt.TextImageRelation = TextImageRelation.ImageBeforeText;

      return rbt;

   }

   void AnyDobTextBoxLeave(object sender, EventArgs e)
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
            Fld_DobNaziv = kupdob_rec.Naziv;
            Fld_DobavCD  = kupdob_rec.KupdobCD;
            Fld_DobTick  = kupdob_rec.Ticker;
         }
         else
         {
            Fld_DobNaziv = Fld_DobTick = Fld_DobCdAsTxt = "";
         }
      }
   }

   void AnyProizvTextBoxLeave(object sender, EventArgs e)
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
            Fld_ProizvNaziv = kupdob_rec.Naziv;
            Fld_ProizCD     = kupdob_rec.KupdobCD;
            Fld_ProizvTick  = kupdob_rec.Ticker;
         }
         else
         {
            Fld_ProizvNaziv = Fld_ProizvTick = Fld_ProizvCdAsTxt = "";
         }
      }
   }

   #endregion HamperFilter

   #region Fld_
   
   public string Fld_FromName
   {
      get { return tbx_artiklName.Text; }
      set {        tbx_artiklName.Text = value; }
   }

   public string Fld_FromArtiklCD
   {
      get { return tbx_artiklCD.Text; }
      set {        tbx_artiklCD.Text = value; }
   }

   public string Fld_FromName2_GEN
   {
      get { return tbx_artiklName2_gen.Text; }
      set {        tbx_artiklName2_gen.Text = value; }
   }

   public string Fld_FromArtiklCD2_ATK
   {
      get { return tbx_artiklCD2_ATK.Text; }
      set {        tbx_artiklCD2_ATK.Text = value; }
   }

   public bool Fld_BiloGdjeUArtiklName
   {
      get { return cbx_biloGdjeUArtiklName.Checked; }
      set {        cbx_biloGdjeUArtiklName.Checked = value; }
   }
   public string Fld_FromBarCode1
   {
      get { return tbx_barCode1.Text; }
      set {        tbx_barCode1.Text = value; }
   }

   public string Fld_OnlySkladCD      
   {
       get { return tbx_skladCD.Text; }
       set {        tbx_skladCD.Text = value; }
   }

   public bool Fld_IsAktivOnly
   {
      get { return cbx_isWKolOnly.Checked; }
      set {        cbx_isWKolOnly.Checked = value; }
   }
  
   public bool Fld_IsShowSomeOfStatusData
   {
      get { return cbx_isStatus.Checked; }
      set {        cbx_isStatus.Checked = value; }
   }

   public string Fld_Grupa1CD      
   {
      get { return tbx_grupa1CD.Text; }
      set {        tbx_grupa1CD.Text = value; }
   }
   public string Fld_JedMj      
   {
         get { return tbx_jedMj.Text; }
         set {        tbx_jedMj.Text = value; }
   }        
    public string Fld_Konto      
   {
      get { return tbx_konto.Text; }
      set {        tbx_konto.Text = value; }
   }
   public string Fld_ArtiklCd2
   {
      get { return tbx_artiklCD2.Text; }
      set {        tbx_artiklCD2.Text = value; }
   }
   public string Fld_ArtiklName2
   {
      get { return tbx_artiklName2.Text; }
      set {        tbx_artiklName2.Text = value; }
   }
   public string Fld_Ts
   {
      get { return tbx_ts.Text; }
      set {        tbx_ts.Text = value; }
   }
   public string Fld_BarCode2
   {
      get { return tbx_barCode2.Text; }
      set {        tbx_barCode2.Text = value; }
   }
   public string Fld_SerNo       
   {
      get { return tbx_serNo.Text; }
      set {        tbx_serNo.Text = value; }
   }
   public string Fld_Grupa2CD     
   {
      get { return tbx_grupa2CD.Text; }
      set {        tbx_grupa2CD.Text = value; }
   }
   public string Fld_Grupa3CD     
   {
      get { return tbx_grupa3CD.Text; }
      set {        tbx_grupa3CD.Text = value; }
   }
   public string Fld_Placement    
   {
      get { return tbx_placement.Text; }
      set {        tbx_placement.Text = value; }
   }
   public string Fld_LinkArtCD    
   {
      get { return tbx_linkArtCD.Text; }
      set {        tbx_linkArtCD.Text = value; }
   }
   public string Fld_PdvKat       
   {
      get { return tbx_pdvKat.Text; }
      set {        tbx_pdvKat.Text = value; }
   }
   public string Fld_Boja
   {
      get { return tbx_boja.Text; }
      set {        tbx_boja.Text = value; }
   }
   public ushort Fld_Spol
   {
      get { return tbx_spol.GetUshortField(); ; }
      set {        tbx_boja.PutUshortField(value); }
   }
   public uint   Fld_DobavCD
   {
      get { return tbx_dobCd.GetSomeRecIDField();      }
      set {        tbx_dobCd.PutSomeRecIDField(value); }
   }
   public string Fld_DobCdAsTxt
   {
      get { return tbx_dobCd.Text;         }
      set {        tbx_dobCd.Text = value; }
   }
   public string Fld_DobNaziv
   {
      get { return tbx_dobNaziv.Text;         }
      set {        tbx_dobNaziv.Text = value; }
   }
   public string Fld_DobTick
   {
      get { return tbx_dobTick.Text;         }
      set {        tbx_dobTick.Text = value; }
   }
   public uint   Fld_ProizCD
   {
      get { return tbx_proizvCd.GetSomeRecIDField();      }
      set {        tbx_proizvCd.PutSomeRecIDField(value); }
   }
   public string Fld_ProizvCdAsTxt
   {
      get { return tbx_proizvCd.Text;         }
      set {        tbx_proizvCd.Text = value; }
   }
   public string Fld_ProizvNaziv
   {
      get { return tbx_proizvNaziv.Text;         }
      set {        tbx_proizvNaziv.Text = value; }
   }
   public string Fld_ProizvTick
   {
      get { return tbx_proizvTick.Text;         }
      set {        tbx_proizvTick.Text = value; }
   }
   public string Fld_MadeIn
   {
      get { return tbx_drzPorjekla.Text; }
      set { tbx_drzPorjekla.Text = value; }
   }
   public string Fld_AtestBr
   {
      get { return tbx_atestBr.Text; }
      set { tbx_atestBr.Text = value; }
   } 

   public ZXC.JeliJeTakav Fld_IsIzuzet
   {
      get
      {
         if     (rbt_rashodDa     .Checked) return ZXC.JeliJeTakav.JE_TAKAV;
         else if(rbt_rashodNe     .Checked) return ZXC.JeliJeTakav.NIJE_TAKAV;
         else if(rbt_rashodNebitno.Checked) return ZXC.JeliJeTakav.NEBITNO;

         else throw new Exception("Fld_IsIzuzet: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.JeliJeTakav.JE_TAKAV  : rbt_rashodDa     .Checked = true; break;
            case ZXC.JeliJeTakav.NIJE_TAKAV: rbt_rashodNe     .Checked = true; break;
            case ZXC.JeliJeTakav.NEBITNO   : rbt_rashodNebitno.Checked = true; break;
         }
      }
   }
   public ZXC.JeliJeTakav Fld_IsAkcija
   {
      get
      {
         if     (rbt_akcijaDa     .Checked) return ZXC.JeliJeTakav.JE_TAKAV;
         else if(rbt_akcijaNe     .Checked) return ZXC.JeliJeTakav.NIJE_TAKAV;
         else if(rbt_akcijaNebitno.Checked) return ZXC.JeliJeTakav.NEBITNO;

         else throw new Exception("Fld_IsIzuzet: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.JeliJeTakav.JE_TAKAV  : rbt_akcijaDa     .Checked = true; break;
            case ZXC.JeliJeTakav.NIJE_TAKAV: rbt_akcijaNe     .Checked = true; break;
            case ZXC.JeliJeTakav.NEBITNO   : rbt_akcijaNebitno.Checked = true; break;
         }
      }   
   }
   public decimal Fld_MasaNettoOd
   {
      get { return tbx_masaNettoOd.GetDecimalField(); }
      set {        tbx_masaNettoOd.PutDecimalField(value); }
   }
   public decimal Fld_MasaBrutoOd
   {
      get { return tbx_masaBrutoOd.GetDecimalField(); }
      set {        tbx_masaBrutoOd.PutDecimalField(value); }
   }
   public decimal Fld_PromjerOd
   {
      get { return tbx_promjerOd.GetDecimalField(); }
      set {        tbx_promjerOd.PutDecimalField(value); }
   }
   public decimal Fld_PovrsinaOd
   {
      get { return tbx_povrsinaOd.GetDecimalField(); }
      set {        tbx_povrsinaOd.PutDecimalField(value); }
   }
   public decimal Fld_ZapreminaOd
   {
      get { return tbx_zapreminaOd.GetDecimalField(); }
      set {        tbx_zapreminaOd.PutDecimalField(value); }
   }
   public decimal Fld_DuljinaOd
   {
      get { return tbx_duljinaOd.GetDecimalField(); }
      set {        tbx_duljinaOd.PutDecimalField(value); }
   }
   public decimal Fld_SirinaOd
   {
      get { return tbx_sirinaOd.GetDecimalField(); }
      set {        tbx_sirinaOd.PutDecimalField(value); }
   }
   public decimal Fld_VisinaOd
   {
      get { return tbx_visinaOd.GetDecimalField(); }
      set {        tbx_visinaOd.PutDecimalField(value); }
   }
   public decimal Fld_StarostOd
   {
      get { return tbx_starostOd.GetDecimalField(); }
      set {        tbx_starostOd.PutDecimalField(value); }
   }
   public string Fld_VelicinaOd
   {
      get { return tbx_velicinaOd.Text; }
      set {        tbx_velicinaOd.Text = value; }
   } 
   public decimal Fld_MasaNettoDo
   {
      get { return tbx_masaNettoDo.GetDecimalField(); }
      set {        tbx_masaNettoDo.PutDecimalField(value); }
   }
   public decimal Fld_MasaBrutoDo
   {
      get { return tbx_masaBrutoDo.GetDecimalField(); }
      set {        tbx_masaBrutoDo.PutDecimalField(value); }
   }
   public decimal Fld_PromjerDo
   {
      get { return tbx_promjerDo.GetDecimalField(); }
      set {        tbx_promjerDo.PutDecimalField(value); }
   }
   public decimal Fld_PovrsinaDo
   {
      get { return tbx_povrsinaDo.GetDecimalField(); }
      set {        tbx_povrsinaDo.PutDecimalField(value); }
   }
   public decimal Fld_ZapreminaDo
   {
      get { return tbx_zapreminaDo.GetDecimalField(); }
      set {        tbx_zapreminaDo.PutDecimalField(value); }
   }
   public decimal Fld_DuljinaDo
   {
      get { return tbx_duljinaDo.GetDecimalField(); }
      set {        tbx_duljinaDo.PutDecimalField(value); }
   }
   public decimal Fld_SirinaDo
   {
      get { return tbx_sirinaDo.GetDecimalField(); }
      set {        tbx_sirinaDo.PutDecimalField(value); }
   }
   public decimal Fld_VisinaDo
   {
      get { return tbx_visinaDo.GetDecimalField(); }
      set {        tbx_visinaDo.PutDecimalField(value); }
   }
   public decimal Fld_StarostDo
   {
      get { return tbx_starostDo.GetDecimalField(); }
      set {        tbx_starostDo.PutDecimalField(value); }
   }
   public string Fld_VelicinaDo
   {
      get { return tbx_velicinaDo.Text; }
      set {        tbx_velicinaDo.Text = value; }
   } 
  
   public decimal Fld_SnagaOd
   {
      get { return tbx_snagaOd.GetDecimalField(); }
      set {        tbx_snagaOd.PutDecimalField(value); }
   }
  
   public decimal Fld_SnagaDo
   {
      get { return tbx_snagaDo.GetDecimalField(); }
      set {        tbx_snagaDo.PutDecimalField(value); }
   }

   public decimal Fld_KolStOd       { get { return tbx_kolStOd       .GetDecimalField(); } set { tbx_kolStOd       .PutDecimalField(value); } }
   public decimal Fld_PrNabCijOd    { get { return tbx_prNabCijOd    .GetDecimalField(); } set { tbx_prNabCijOd    .PutDecimalField(value); } }
   public decimal Fld_FinStOd       { get { return tbx_finStOd       .GetDecimalField(); } set { tbx_finStOd       .PutDecimalField(value); } }
   public decimal Fld_IzlRezervKolOd{ get { return tbx_izlRezervKolOd.GetDecimalField(); } set { tbx_izlRezervKolOd.PutDecimalField(value); } }
   public decimal Fld_KolStFreeOd   { get { return tbx_KolStFreeOd   .GetDecimalField(); } set { tbx_KolStFreeOd   .PutDecimalField(value); } }
   public decimal Fld_KolStDo       { get { return tbx_kolStDo       .GetDecimalField(); } set { tbx_kolStDo       .PutDecimalField(value); } }
   public decimal Fld_PrNabCijDo    { get { return tbx_prNabCijDo    .GetDecimalField(); } set { tbx_prNabCijDo    .PutDecimalField(value); } }
   public decimal Fld_FinStDo       { get { return tbx_finStDo       .GetDecimalField(); } set { tbx_finStDo       .PutDecimalField(value); } }
   public decimal Fld_IzlRezervKolDo{ get { return tbx_izlRezervKolDo.GetDecimalField(); } set { tbx_izlRezervKolDo.PutDecimalField(value); } }
   public decimal Fld_KolStFreeDo   { get { return tbx_KolStFreeDo   .GetDecimalField(); } set { tbx_KolStFreeDo   .PutDecimalField(value); } }
   public decimal Fld_Vpc1Od        { get { return tbx_cjenikVpc1Od  .GetDecimalField(); } set { tbx_cjenikVpc1Od  .PutDecimalField(value); } }
   public decimal Fld_RucOd         { get { return tbx_rucOd         .GetDecimalField(); } set { tbx_rucOd         .PutDecimalField(value); } }
   public decimal Fld_VPC2Od        { get { return tbx_VPC2Od        .GetDecimalField(); } set { tbx_VPC2Od        .PutDecimalField(value); } }
   public decimal Fld_MPC1Od        { get { return tbx_MPC1Od        .GetDecimalField(); } set { tbx_MPC1Od        .PutDecimalField(value); } }
   public decimal Fld_DevCOd        { get { return tbx_devCOd        .GetDecimalField(); } set { tbx_devCOd        .PutDecimalField(value); } }
   public decimal Fld_Rabat1Od      { get { return tbx_rabat1Od      .GetDecimalField(); } set { tbx_rabat1Od      .PutDecimalField(value); } }
   public decimal Fld_Rabat2Od      { get { return tbx_rabat2Od      .GetDecimalField(); } set { tbx_rabat2Od      .PutDecimalField(value); } }
   public decimal Fld_MinKolOd      { get { return tbx_minKolOd      .GetDecimalField(); } set { tbx_minKolOd      .PutDecimalField(value); } }
   public decimal Fld_MarzaOd       { get { return tbx_marzaOd       .GetDecimalField(); } set { tbx_marzaOd       .PutDecimalField(value); } }
   public decimal Fld_Vpc1Do        { get { return tbx_cjenikVpc1Do  .GetDecimalField(); } set { tbx_cjenikVpc1Do  .PutDecimalField(value); } }
   public decimal Fld_RucDo         { get { return tbx_rucDo         .GetDecimalField(); } set { tbx_rucDo         .PutDecimalField(value); } }
   public decimal Fld_VPC2Do        { get { return tbx_VPC2Do        .GetDecimalField(); } set { tbx_VPC2Do        .PutDecimalField(value); } }
   public decimal Fld_MPC1Do        { get { return tbx_MPC1Do        .GetDecimalField(); } set { tbx_MPC1Do        .PutDecimalField(value); } }
   public decimal Fld_DevCDo        { get { return tbx_devCDo        .GetDecimalField(); } set { tbx_devCDo        .PutDecimalField(value); } }
   public decimal Fld_Rabat1Do      { get { return tbx_rabat1Do      .GetDecimalField(); } set { tbx_rabat1Do      .PutDecimalField(value); } }
   public decimal Fld_Rabat2Do      { get { return tbx_rabat2Do      .GetDecimalField(); } set { tbx_rabat2Do      .PutDecimalField(value); } }
   public decimal Fld_MinKolDo      { get { return tbx_minKolDo      .GetDecimalField(); } set { tbx_minKolDo      .PutDecimalField(value); } }
   public decimal Fld_MarzaDo       { get { return tbx_marzaDo       .GetDecimalField(); } set { tbx_marzaDo       .PutDecimalField(value); } }

   public string Fld_SituacijaZaSkladCD
   {
      get 
      {
         ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD = tbx_zaSkladCD.Text;
         return tbx_zaSkladCD.Text; 
      }
      set 
      {
         //ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD = value;
         tbx_zaSkladCD.Text = value; 
      }
   }
      
   public bool Fld_IsMinusEver
   {
      get { return cbx_ikadaUminusu.Checked; }
      set {        cbx_ikadaUminusu.Checked = value; }
   }

   #endregion Fld_

   #region Overriders And Specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.artikl_rec; }
      set {        this.artikl_rec = (Artikl)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.ArtiklDao; }
   }

   private Vektor.DataLayer.DS_FindRecord.DS_findArtikl ds_artikl;
   
   protected override DataSet VirtualUntypedDataSet { get { return ds_artikl; } }

   protected override object[] From_IndexSegmentValues
   {
      get
      {
         switch (recordSorter.SortType)
         {
            case VvSQL.SorterType.Name    : return new object[] { Fld_FromName         , Fld_FromArtiklCD, 0 };
            case VvSQL.SorterType.Code    : return new object[] { Fld_FromArtiklCD     ,                   0 };
            case VvSQL.SorterType.BarCode : return new object[] { Fld_FromBarCode1     ,                   0 };
            case VvSQL.SorterType.Name2   : return new object[] { Fld_FromName2_GEN    , Fld_FromName    , 0 };
            case VvSQL.SorterType.Code2   : return new object[] { Fld_FromArtiklCD2_ATK, Fld_FromName    , 0 };

            default: ZXC.aim_emsg("Q42: SortType [{0}] undifajnd in property 'From_IndexSegmentValues'", recordSorter.SortType); return null;
         }
      }
   }

   public override void SetListFilterRecordDependentDefaults()
   {
      Fld_SituacijaZaSkladCD = ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD;
   }

   //CiaoBaby
   //public override bool IsRemoveUnapropriatesNeeded
   //{
   //   get
   //   {
   //      return
   //      (
   //         Fld_IsWKolOnly == true ||

   //         Fld_KolStOd.NotZero() ||
   //         Fld_KolStDo.NotZero() ||
   //         Fld_PrNabCijOd.NotZero() ||
   //         Fld_PrNabCijDo.NotZero() ||
   //         Fld_FinStOd.NotZero() ||
   //         Fld_FinStDo.NotZero() ||
   //         Fld_IzlRezervKolOd.NotZero() ||
   //         Fld_IzlRezervKolDo.NotZero() ||
   //         Fld_KolStFreeOd.NotZero() ||
   //         Fld_KolStFreeDo.NotZero() ||
   //         Fld_Vpc1Od.NotZero() ||
   //         Fld_Vpc1Do.NotZero() ||
   //         Fld_RucOd.NotZero() ||
   //         Fld_RucDo.NotZero() ||
   //         Fld_VPC2Od.NotZero() ||
   //         Fld_VPC2Do.NotZero() ||
   //         Fld_MPC1Od.NotZero() ||
   //         Fld_MPC1Do.NotZero() ||
   //         Fld_DevCOd.NotZero() ||
   //         Fld_DevCDo.NotZero() ||
   //         Fld_Rabat1Od.NotZero() ||
   //         Fld_Rabat1Do.NotZero() ||
   //         Fld_Rabat2Od.NotZero() ||
   //         Fld_Rabat2Do.NotZero() ||
   //         Fld_MinKolOd.NotZero() ||
   //         Fld_MinKolDo.NotZero() ||
   //         Fld_MarzaOd.NotZero() ||
   //         Fld_MarzaDo.NotZero()
   //      );
   //   }
   //}

   #endregion Overriders and specifics

   #region AddFilterMemberz()

   /// <summary>
   /// get { return ZXC.ArtiklDao.TheSchemaTable.Rows; }
   /// </summary>
   private DataRowCollection ArtiklSchema
   {
      get { return ZXC.ArtiklDao.TheSchemaTable.Rows; }
   }

   /// <summary>
   ///  get { return ZXC.ArtiklDao.CI; }
   /// </summary>
   private ArtiklDao.ArtiklCI ArtCI
   {
      get { return ZXC.ArtiklDao.CI; }
   }

   private DataRowCollection ArtStatSchema
   {
      get { return ZXC.ArtStatDao.TheSchemaTable.Rows; }
   }

   private ArtStatDao.ArtStatCI ArtStatCI
   {
      get { return ZXC.ArtStatDao.CI; }
   }

   public override void AddFilterMemberz()
   {
      string text, comparer;
      uint num;
      decimal decimOd, decimDo;

      ZXC.JeliJeTakav jeLiTakav;

      DataRow drSchema;

      this.TheFilterMembers.Clear();

      #region Artikl Filter Members

      // Fld_Ts                                                                                                                                    
      drSchema = ArtiklSchema[ArtCI.ts];
      text = Fld_Ts;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "TS", text, " = "));
      }

      // Fld_SkladCD                                                                                                                                    
      drSchema = ArtiklSchema[ArtCI.skladCD];
      text = Fld_OnlySkladCD;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "SkladCD", text, " = "));
      }

      // Fld_Grupa1CD                                                                                                                                    
      drSchema = ArtiklSchema[ArtCI.grupa1CD];
      text = Fld_Grupa1CD;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Grupa1CD", text, " = "));
      }

      // Fld_Grupa2CD                                                                                                                                    
      drSchema = ArtiklSchema[ArtCI.grupa2CD];
      text = Fld_Grupa2CD;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Grupa2CD", text, " = "));
      }

      // Fld_Grupa3CD                                                                                                                                    
      drSchema = ArtiklSchema[ArtCI.grupa3CD];
      text = Fld_Grupa3CD;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Grupa3CD", text, " = "));
      }

      // Fld_PdvKat                                                                                                                                    
      drSchema = ArtiklSchema[ArtCI.pdvKat];
      text = Fld_PdvKat;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "PdvKat", text, " = "));
      }

      // Fld_DobavCD                                                                                                                                          
      drSchema = ArtiklSchema[ArtCI.dobavCD];
      num = Fld_DobavCD;

      if(num != 0)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Dobav", num, " = "));
      }

      // Fld_ProizCD                                                                                                                                          
      drSchema = ArtiklSchema[ArtCI.proizCD];
      num = Fld_ProizCD;

      if(num != 0)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Proiz", num, " = "));
      }

      // Fld_IsIzuzet                                                                                                                                         
      drSchema = ArtiklSchema[ArtCI.isRashod];
      jeLiTakav = Fld_IsIzuzet;

      if(jeLiTakav == ZXC.JeliJeTakav.JE_TAKAV)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, false, "JEizuzet", true, null, "", " = ", ""));
      }
      else if(jeLiTakav == ZXC.JeliJeTakav.NIJE_TAKAV)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, false, "NIJEizuzet", false, null, "", " = ", ""));
      }

      // Fld_IsAkcija                                                                                                                                         
      drSchema = ArtiklSchema[ArtCI.isAkcija];
      jeLiTakav = Fld_IsAkcija;

      if(jeLiTakav == ZXC.JeliJeTakav.JE_TAKAV)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, false, "JEakcija", true, null, "", " = ", ""));
      }
      else if(jeLiTakav == ZXC.JeliJeTakav.NIJE_TAKAV)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, false, "NIJEakcija", false, null, "", " = ", ""));
      }

      // Fld_JedMj                                                                                                                                       
      drSchema = ArtiklSchema[ArtCI.jedMj];
      text = Fld_JedMj;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "JedMj", text, " = "));
      }

      // Fld_Konto                                                                                                                                          
      drSchema = ArtiklSchema[ArtCI.konto];
      text = Fld_Konto;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Konto", text, " = "));
      }

      // Fld_ArtiklCd2                                                                                                                                            
      drSchema = ArtiklSchema[ArtCI.artiklCD2];
      text = Fld_ArtiklCd2;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "ArtiklCd2", text, " = "));
      }

      // Fld_ArtiklName2                                                                                                                                          
      drSchema = ArtiklSchema[ArtCI.artiklName2];
      text = Fld_ArtiklName2;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "ArtiklName2", text, " = "));
      }

      // Fld_BarCode2                                                                                                                                          
      drSchema = ArtiklSchema[ArtCI.barCode2];
      text = Fld_BarCode2;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "BarCode2", text, " = "));
      }

      // Fld_SerNo                                                                                                                                       
      drSchema = ArtiklSchema[ArtCI.serNo];
      text = Fld_SerNo;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "SerNo", text, " = "));
      }

      // Fld_Placement                                                                                                                                            
      drSchema = ArtiklSchema[ArtCI.placement];
      text = Fld_Placement;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Placement", text, " = "));
      }

      // Fld_LinkArtCD                                                                                                                                         
      drSchema = ArtiklSchema[ArtCI.linkArtCD];
      text = Fld_LinkArtCD;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "LinkArtCD", text, " = "));
      }

      // Fld_Boja                                                                                                                                              
      drSchema = ArtiklSchema[ArtCI.boja];
      text = Fld_Boja;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Boja", text, " = "));
      }

      // Fld_MasaNetto                                                                                                                                   
      drSchema = ArtiklSchema[ArtCI.masaNetto];
      decimOd = Fld_MasaNettoOd;
      decimDo = Fld_MasaNettoDo;

      if(decimOd.NotZero())
      {
         if(decimOd == decimDo) comparer = " = ";
         else                   comparer = " >= ";

         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "MasaNettoOd", decimOd, comparer));
      }

      if(decimDo.NotZero() && decimDo != decimOd)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "MasaNettoDo", decimDo, " <= "));
      }


      // Fld_MasaBruto                                                                                                                                   
      drSchema = ArtiklSchema[ArtCI.masaBruto];
      decimOd = Fld_MasaBrutoOd;
      decimDo = Fld_MasaBrutoDo;

      if(decimOd.NotZero())
      {
         if(decimOd == decimDo) comparer = " = ";
         else                   comparer = " >= ";

         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "MasaBrutoOd", decimOd, comparer));
      }

      if(decimDo.NotZero() && decimDo != decimOd)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "MasaBrutoDo", decimDo, " <= "));
      }


      // Fld_Promjer                                                                                                                                     
      drSchema = ArtiklSchema[ArtCI.promjer];
      decimOd = Fld_PromjerOd;
      decimDo = Fld_PromjerDo;

      if(decimOd.NotZero())
      {
         if(decimOd == decimDo) comparer = " = ";
         else                   comparer = " >= ";

         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "PromjerOd", decimOd, comparer));
      }

      if(decimDo.NotZero() && decimDo != decimOd)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "PromjerDo", decimDo, " <= "));
      }


      // Fld_Povrsina                                                                                                                                    
      drSchema = ArtiklSchema[ArtCI.povrsina];
      decimOd = Fld_PovrsinaOd;
      decimDo = Fld_PovrsinaDo;

      if(decimOd.NotZero())
      {
         if(decimOd == decimDo) comparer = " = ";
         else                   comparer = " >= ";

         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "PovrsinaOd", decimOd, comparer));
      }

      if(decimDo.NotZero() && decimDo != decimOd)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "PovrsinaDo", decimDo, " <= "));
      }


      // Fld_Zapremina                                                                                                                                               
      drSchema = ArtiklSchema[ArtCI.zapremina];
      decimOd = Fld_ZapreminaOd;
      decimDo = Fld_ZapreminaDo;

      if(decimOd.NotZero())
      {
         if(decimOd == decimDo) comparer = " = ";
         else                   comparer = " >= ";

         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "ZapreminaOd", decimOd, comparer));
      }

      if(decimDo.NotZero() && decimDo != decimOd)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "ZapreminaDo", decimDo, " <= "));
      }


      // Fld_Duljina                                                                                                                                           
      drSchema = ArtiklSchema[ArtCI.duljina];
      decimOd = Fld_DuljinaOd;
      decimDo = Fld_DuljinaDo;

      if(decimOd.NotZero())
      {
         if(decimOd == decimDo) comparer = " = ";
         else                   comparer = " >= ";

         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "DuljinaOd", decimOd, comparer));
      }

      if(decimDo.NotZero() && decimDo != decimOd)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "DuljinaDo", decimDo, " <= "));
      }


      // Fld_Sirina                                                                                                                                   
      drSchema = ArtiklSchema[ArtCI.sirina];
      decimOd = Fld_SirinaOd;
      decimDo = Fld_SirinaDo;

      if(decimOd.NotZero())
      {
         if(decimOd == decimDo) comparer = " = ";
         else                   comparer = " >= ";

         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "SirinaOd", decimOd, comparer));
      }

      if(decimDo.NotZero() && decimDo != decimOd)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "SirinaDo", decimDo, " <= "));
      }


      // Fld_Visina                                                                                                                                      
      drSchema = ArtiklSchema[ArtCI.visina];
      decimOd = Fld_VisinaOd;
      decimDo = Fld_VisinaDo;

      if(decimOd.NotZero())
      {
         if(decimOd == decimDo) comparer = " = ";
         else                   comparer = " >= ";

         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "VisinaOd", decimOd, comparer));
      }

      if(decimDo.NotZero() && decimDo != decimOd)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "VisinaDo", decimDo, " <= "));
      }


      // Fld_Starost                                                                                                                                     
      drSchema = ArtiklSchema[ArtCI.starost];
      decimOd = Fld_StarostOd;
      decimDo = Fld_StarostDo;

      if(decimOd.NotZero())
      {
         if(decimOd == decimDo) comparer = " = ";
         else                   comparer = " >= ";

         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "StarostOd", decimOd, comparer));
      }

      if(decimDo.NotZero() && decimDo != decimOd)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "StarostDo", decimDo, " <= "));
      }

      // Fld_Snaga                                                                                                                                   
      drSchema = ArtiklSchema[ArtCI.snaga];
      decimOd  = Fld_SnagaOd;
      decimDo  = Fld_SnagaDo;

      if(decimOd.NotZero())
      {
         if(decimOd == decimDo) comparer = " = ";
         else                   comparer = " >= ";

         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "SnagaOd", decimOd, comparer));
      }

      if(decimDo.NotZero() && decimDo != decimOd)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "SnagaDo", decimDo, " <= "));
      }

      #endregion Artikl Filter Members

      #region ArtiklStatus Filter Members

      //if(Fld_IsWKolOnly == true) this.TheFilterMembers.Add(new VvSqlFilterMember(ArtiklDao.ResCol_StanjeKol, 0, " > ", ArtStat.recordName));

      // !!!: Fld_IsMinusEver je obradjen u VvSql.EventualRelatedArtstat_ForWhereClause_FromFilterMembers()

      // Fld_IsAktivOnly pravilo mora ici samo od sebe ukoliko treba Fld_IsMinusEver
      if(Fld_IsAktivOnly == true ||
         Fld_IsMinusEver == true ) this.TheFilterMembers.Add(new VvSqlFilterMember(ArtiklDao.ResCol_StanjeKol, "NULL", " IS NOT ", ArtStat.recordName));

      AddFilterMember_ArtstatValueOdValueDo(ArtiklDao.ResCol_StanjeKol, Fld_KolStOd    , Fld_KolStDo    );
      AddFilterMember_ArtstatValueOdValueDo(ArtiklDao.ResCol_PrNabCij , Fld_PrNabCijOd , Fld_PrNabCijDo );
      AddFilterMember_ArtstatValueOdValueDo(ArtiklDao.ResCol_StanjeFinNBC, Fld_FinStOd    , Fld_FinStDo    );
      AddFilterMember_ArtstatValueOdValueDo(ArtiklDao.ResCol_StanjeKolFree  , Fld_KolStFreeOd, Fld_KolStFreeDo);
      AddFilterMember_ArtstatValueOdValueDo(ArtiklDao.ResCol_RucVEL   , Fld_RucOd      , Fld_RucDo      );

      AddFilterMember_ArtstatValueOdValueDo(ArtStatSchema[ArtStatCI.stanjeKolRezerv], Fld_IzlRezervKolOd, Fld_IzlRezervKolDo);
      AddFilterMember_ArtstatValueOdValueDo(ArtStatSchema[ArtStatCI.preDefVpc1     ], Fld_Vpc1Od        , Fld_Vpc1Do        );
    //AddFilterMember_ArtstatValueOdValueDo(ArtStatSchema[ArtStatCI.preDefVpc2     ], Fld_Vpc2Od        , Fld_Vpc2Do        );  
      AddFilterMember_ArtstatValueOdValueDo(ArtStatSchema[ArtStatCI.preDefMpc1     ], Fld_MPC1Od        , Fld_MPC1Do        );
      AddFilterMember_ArtstatValueOdValueDo(ArtStatSchema[ArtStatCI.preDefDevc     ], Fld_DevCOd        , Fld_DevCDo        );
      AddFilterMember_ArtstatValueOdValueDo(ArtStatSchema[ArtStatCI.preDefRbt1     ], Fld_Rabat1Od      , Fld_Rabat1Do      );
      AddFilterMember_ArtstatValueOdValueDo(ArtStatSchema[ArtStatCI.preDefRbt2     ], Fld_Rabat2Od      , Fld_Rabat2Do      );
      AddFilterMember_ArtstatValueOdValueDo(ArtStatSchema[ArtStatCI.preDefMinKol   ], Fld_MinKolOd      , Fld_MinKolDo      );
      AddFilterMember_ArtstatValueOdValueDo(ArtStatSchema[ArtStatCI.preDefMarza    ], Fld_MarzaOd       , Fld_MarzaDo       );

      #endregion ArtiklStatus Filter Members

   }

   private void AddFilterMember_ArtstatValueOdValueDo(string forcedColName, decimal decimOd, decimal decimDo)
   {
      string comparer;
      if(decimOd.NotZero())
      {
         if(decimOd == decimDo) comparer = " = ";
         else                   comparer = " >= ";

         this.TheFilterMembers.Add(new VvSqlFilterMember(forcedColName, decimOd, comparer, ArtStat.recordName));
      }

      if(decimDo.NotZero() && decimDo != decimOd)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(forcedColName, decimDo, " <= ", ArtStat.recordName));
      }

   }

   private void AddFilterMember_ArtstatValueOdValueDo(DataRow drSchema, decimal decimOd, decimal decimDo)
   {
      string comparer;
      if(decimOd.NotZero())
      {
         if(decimOd == decimDo) comparer = " = ";
         else                   comparer = " >= ";

         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, false, drSchema["ColumnName"]+"Od", decimOd, "", "", comparer, ArtStat.recordName));
      }

      if(decimDo.NotZero() && decimDo != decimOd)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, false, drSchema["ColumnName"]+"Do", decimDo, "", "", " <= ", ArtStat.recordName));
      }
   }

   #endregion AddFilterMemberz()

   #region override PerformAdditionalDataSetOperation

   //CiaoBaby
   //protected override bool PerformAdditionalDataSetOperation()
   //{
   //   // !!! 
   //   if(Fld_IsShowSomeOfStatusData == false && IsRemoveUnapropriatesNeeded == false) return true; // Homo tja!                                                        
      
   //   string  zaSkladCD = Fld_ZaSkladCD, artiklCD;
   //   DateTime dateDo;
   //   ArtStat  artStat_rec;

   //        if(ZXC.TheVvForm.TheVvUC is FakturExtDUC) dateDo = ((FakturExtDUC)ZXC.TheVvForm.TheVvUC).Fld_SkladDate; 
   //   else if(ZXC.TheVvForm.TheVvUC is FakturDUC   ) dateDo = ((FakturDUC)   ZXC.TheVvForm.TheVvUC).Fld_DokDate; 
   //   else                                           dateDo = DateTime.MinValue;

   //   for(int rIdx = 0; rIdx < TheGrid.RowCount /* - 1*/; ++rIdx)
   //   {
   //      artiklCD = TheGrid.GetStringCell("artiklCD", rIdx, false);

   //      artStat_rec = ArtiklDao.GetArtiklStatus(conn, artiklCD, zaSkladCD, dateDo);

   //      if(IsThisRowUnwanted(artStat_rec) == true) TheGrid.Rows.RemoveAt(rIdx--);
   //      else                                       PutArtStatLineFields (rIdx, artStat_rec);

   //   }

   //   return true;
   //}
   //private bool IsThisRowUnwanted(ArtStat artStat_rec)
   //{
   //   if(IsRemoveUnapropriatesNeeded == false)                             return false;
   //   if(artStat_rec == null)                                              return true;
   //   if(Fld_IsWKolOnly == true && artStat_rec.StanjeKol.IsZero() == true) return true;


   //   if(ValueIsOutOfBorders(Fld_KolStOd       , artStat_rec.StanjeKol      , Fld_KolStDo       )) return true;
   //   if(ValueIsOutOfBorders(Fld_PrNabCijOd    , artStat_rec.PrNabCij, Fld_PrNabCijDo    )) return true;
   //   if(ValueIsOutOfBorders(Fld_FinStOd       , artStat_rec.StanjeFinKNJ      , Fld_FinStDo       )) return true;
   //   if(ValueIsOutOfBorders(Fld_IzlRezervKolOd, artStat_rec.UkIzlazKolRezerv , Fld_IzlRezervKolDo)) return true;
   //   if(ValueIsOutOfBorders(Fld_KolStFreeOd   , artStat_rec.StanjeKolFree  , Fld_KolStFreeDo   )) return true;
   //   if(ValueIsOutOfBorders(Fld_Vpc1Od        , artStat_rec.PreDefVpc1     , Fld_Vpc1Do        )) return true;
   //   if(ValueIsOutOfBorders(Fld_RucOd         , artStat_rec.RucVpc1Stopa   , Fld_RucDo         )) return true;
   //   if(ValueIsOutOfBorders(Fld_VPC2Od        , artStat_rec.PreDefVpc2     , Fld_VPC2Do        )) return true;
   //   if(ValueIsOutOfBorders(Fld_MPC1Od        , artStat_rec.PreDefMpc1     , Fld_MPC1Do        )) return true;
   //   if(ValueIsOutOfBorders(Fld_DevCOd        , artStat_rec.PreDefDevc     , Fld_DevCDo        )) return true;
   //   if(ValueIsOutOfBorders(Fld_Rabat1Od      , artStat_rec.PreDefRbt1     , Fld_Rabat1Do      )) return true;
   //   if(ValueIsOutOfBorders(Fld_Rabat2Od      , artStat_rec.PreDefRbt2     , Fld_Rabat2Do      )) return true;
   //   if(ValueIsOutOfBorders(Fld_MinKolOd      , artStat_rec.PreDefMinKol   , Fld_MinKolDo      )) return true;
   //   if(ValueIsOutOfBorders(Fld_MarzaOd       , artStat_rec.PreDefMarza    , Fld_MarzaDo       )) return true;

   //   return false;
   //}
   //private bool ValueIsOutOfBorders(decimal lowerBorder, decimal statusValue, decimal upperBorder)
   //{
   //   return (lowerBorder.NotZero() && statusValue < lowerBorder) ||
   //          (upperBorder.NotZero() && statusValue > upperBorder);
   //}
   //private void PutArtStatLineFields(int rIdx, ArtStat artStat_rec)
   //{
   //   if(artStat_rec == null) return;

   //   // sada ovdje treba navesti sve mozebitnopojavljujuce kolone...

   //   TheGrid.PutCell("stanjeKol", rIdx, artStat_rec.StanjeKol);
   //   TheGrid.PutCell("vpc1"     , rIdx, artStat_rec.PreDefVpc1);
   //}

   #endregion override PerformAdditionalDataSetOperation
}

// Za pretragu rtransa ... npr. po 't_serlot' -u 

public class RtransListUC : VvDocumRecLstUC
{
   #region Fieldz

   protected Rtrans rtrans_rec;

   public RadioButton rbtSortByTtNum, rbtSortByDokDate, rbtSortByPartner, rBtcurrChecked,
                      rBtSortByName , rBtSortBySifra  , rBtSortBySerlot;

   public VvTextBox  tbx_dokDate, tbx_PartnerName, tbx_ttNum, tbx_TT,tbx_artiklName, tbx_artiklCD, tbx_serlot,
                     tbx_PartnerCD, tbx_PartnerTK;
   public CheckBox cbx_biloGdjeUnazivu, cbx_biloGdjeUArtiklName;

   public VvDateTimePicker dtp_dokDate;

   public string Default_TT { get; set; }
   
   #endregion Fieldz

   #region Constructor

   public RtransListUC(Control parent, Rtrans _rtrans, VvForm.VvSubModul vvSubModul ) : base(parent)
   {
      this.rtrans_rec = _rtrans;
      this.Parent.Text = "Lista RISK stavaka";

      this.MasterSubModulEnum = ZXC.VvSubModulEnum.ART;
      this.TheSubModul = vvSubModul;

      TheFilterMembers = new System.Collections.Generic.List<VvSqlFilterMember>(); // namoj tamo di ne treba (npr Kplan) 


      Supress_ImaLiIjedan_StartField_Neprazan_Action = true;
      //SelectFirstSortRadioButton(tbx_ttNum, tbx_TT, Default_TT);

      rbtSortByTtNum  .Checked = ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_ttNum   ;
      rbtSortByDokDate.Checked = ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_dokDate ;
      rbtSortByPartner.Checked = ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_kpdbName;
      rBtSortBySerlot .Checked = ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_serlot  ;
      rBtSortBySifra  .Checked = ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_sifra   ;
      rBtSortByName   .Checked = ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_naziv   ;

      hampOpenUtil.Visible = hampUtil.Visible = false;

      SetControlForInitialFocus();

   }

   protected override void InitializeFindFormSpecifics()
   {
      recordSorter = Rtrans.sorterTtNum;

      this.ds_rtrans = new Vektor.DataLayer.DS_FindRecord.DS_findRtrans();

      this.Name = "RtransListUC";
      this.Text = "RtransListUC";
   }

   #endregion Constructor

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
         this.ControlForInitialFocus = tbx_PartnerName;
         rbtSortByPartner_Click(rbtSortByPartner, EventArgs.Empty);
      }
      if(rBtSortByName.Checked)
      {
         this.ControlForInitialFocus = tbx_artiklName;
         radioButtonSortByName_Click(rBtSortByName, EventArgs.Empty);
      }
      if(rBtSortBySifra.Checked)
      {
         this.ControlForInitialFocus = tbx_artiklCD;
         radioButtonSortBySifra_Click(rBtSortBySifra, EventArgs.Empty);
      }
      if(rBtSortBySerlot.Checked)
      {
         this.ControlForInitialFocus = tbx_serlot;
         radioButtonSortBySerlot_Click(rBtSortBySerlot, EventArgs.Empty);
      }

   }

   protected override void CreateHamperSpecifikum()
   {
      hampSpecifikum = new VvHamper(9, 2, "", this, true, hampListaRastePada.Right + ZXC.Qun4, nextY, razmakHamp);
      //                                              0       1          2        3        4           5          6       7          8     
      hampSpecifikum.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un, ZXC.Q4un, ZXC.Q5un, ZXC.Q6un, ZXC.Q5un, ZXC.Q3un, ZXC.Q3un, ZXC.Q7un };
      hampSpecifikum.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hampSpecifikum.VvRightMargin = hampSpecifikum.VvLeftMargin;

      hampSpecifikum.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN   };
      hampSpecifikum.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4  };
      hampSpecifikum.VvBottomMargin = hampSpecifikum.VvTopMargin;

      rBtSortBySerlot = hampSpecifikum.CreateVvRadioButton (0, 0, new EventHandler(radioButtonSortBySerlot_Click), "Od serlot:", TextImageRelation.ImageAboveText);
      tbx_serlot      = hampSpecifikum.CreateVvTextBox     (1, 0, "tbx_serlot", "", 32, 1, 0);
      tbx_serlot.DoubleClick += new EventHandler(tbx_DoubleClick);
      tbx_serlot.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

      tbx_serlot.Tag     = rBtSortBySerlot;
      rBtSortBySerlot.Tag = tbx_serlot;

      
      rBtSortBySifra = hampSpecifikum.CreateVvRadioButton(3, 0, new EventHandler(radioButtonSortBySifra_Click), "Od šifre artikla:", TextImageRelation.ImageAboveText);
      tbx_artiklCD   = hampSpecifikum.CreateVvTextBox    (4, 0, "tbx_artiklCD", "");
      tbx_artiklCD.DoubleClick += new EventHandler(tbx_DoubleClick);
      tbx_artiklCD.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

      tbx_artiklCD.Tag   = rBtSortBySifra;
      rBtSortBySifra.Tag = tbx_artiklCD;

      rBtSortByName           = hampSpecifikum.CreateVvRadioButton (5, 0, new EventHandler(radioButtonSortByName_Click), "Od naziva artikla:", TextImageRelation.ImageAboveText);
      tbx_artiklName          = hampSpecifikum.CreateVvTextBox     (6, 0, "tbx_artiklName", "",64, 2, 0);

      rBtSortByName.Checked = true;
      tbx_artiklName.DoubleClick += new EventHandler(tbx_DoubleClick);
      tbx_artiklName.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

      tbx_artiklName.Tag  = rBtSortByName;
      rBtSortByName.Tag   = tbx_artiklName;

      this.ControlForInitialFocus = tbx_serlot;
      
      rbtSortByTtNum     = hampSpecifikum.CreateVvRadioButton  (0, 1, new EventHandler(rbtSortByTtNum_Click), "Od dokumenta:", TextImageRelation.ImageAboveText);
      tbx_TT             = hampSpecifikum.CreateVvTextBoxLookUp(1, 1, "tbx_TT", "");
      tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_ttNum          = hampSpecifikum.CreateVvTextBox     (2, 1, "tbx_ttNum", "");
      tbx_ttNum.Tag      = rbtSortByTtNum;
      tbx_TT.Tag         = rbtSortByTtNum;
      rbtSortByTtNum.Tag = tbx_ttNum;
      tbx_ttNum.DoubleClick  += new EventHandler(tbx_DoubleClick);
      tbx_ttNum.JAM_ValueType = typeof(int);
      tbx_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_TT.JAM_CharacterCasing = CharacterCasing.Upper;

      rbtSortByDokDate     = hampSpecifikum.CreateVvRadioButton(3, 1, new EventHandler(rbtSortByDokDate_Click), "Od datuma:", TextImageRelation.ImageAboveText);
      tbx_dokDate          = hampSpecifikum.CreateVvTextBox    (4, 1, "tbx_dokDate", "");
      rbtSortByDokDate.Tag = tbx_dokDate;
      tbx_dokDate.JAM_IsForDateTimePicker = true;

      dtp_dokDate      = hampSpecifikum.CreateVvDateTimePicker(4, 1, "", tbx_dokDate);
      dtp_dokDate.Name = "dtp_dokDate";
      dtp_dokDate.Tag  = rbtSortByDokDate;

      tbx_dokDate.DoubleClick += new EventHandler(tbx_DoubleClick);

      rbtSortByPartner        = hampSpecifikum.CreateVvRadioButton(5, 1, new EventHandler(rbtSortByPartner_Click), "Od partnera:", TextImageRelation.ImageAboveText);
      
      tbx_PartnerCD    = hampSpecifikum.CreateVvTextBox(6, 1, "tbx_PartnerCd"  , "Šifra partnera" );
      tbx_PartnerTK    = hampSpecifikum.CreateVvTextBox(7, 1, "tbx_PartnerTick", "Ticker partnera");
      tbx_PartnerName  = hampSpecifikum.CreateVvTextBox(8, 1, "tbx_PartnerName", "Naziv partnera" );
      
      tbx_PartnerCD.Tag    = rbtSortByPartner;
      rbtSortByPartner.Tag = tbx_PartnerCD;
      //tbx_PartnerCD.DoubleClick += new EventHandler(tbx_DoubleClick);

      tbx_PartnerCD.JAM_CharEdits       = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_PartnerTK.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_PartnerCD  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD   .SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyKupDobTextBoxLeave));
      tbx_PartnerTK  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupDobTextBoxLeave));
      tbx_PartnerName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv .SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyKupDobTextBoxLeave));
      
      VvHamper.Open_Close_Fields_ForWriting(tbx_serlot     , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_artiklCD   , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_artiklName , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_TT         , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_ttNum      , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog); 
      VvHamper.Open_Close_Fields_ForWriting(tbx_dokDate    , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_PartnerName, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_PartnerCD  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_PartnerTK  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);

   }

   #endregion Hamperi

   #region Eveniti

   public void rbtSortByDokDate_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Rtrans.sorterDokDate;

      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_ttNum    = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_dokDate  = true ;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_kpdbName = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_naziv    = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_sifra    = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_serlot   = false;
   }

   public void rbtSortByPartner_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Rtrans.sorterKpdbName;

      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_ttNum    = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_dokDate  = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_kpdbName = true;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_naziv    = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_sifra    = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_serlot   = false;

      VvHamper.Open_Close_Fields_ForWriting(tbx_PartnerName, ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_PartnerTK  , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvFindDialog);
   }

   public void rbtSortByTtNum_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Rtrans.sorterTtNum;

      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_ttNum    = true;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_dokDate  = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_kpdbName = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_naziv    = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_sifra    = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_serlot   = false;

      VvHamper.Open_Close_Fields_ForWriting(tbx_TT, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
   }
   
   private void radioButtonSortByName_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Rtrans.sorterArtiklName;

      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_naziv    = true;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_sifra    = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_serlot   = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_ttNum    = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_dokDate  = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_kpdbName = false;
   }
 
   private void radioButtonSortBySifra_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Rtrans.sorterArtiklCD;
       
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_naziv    = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_sifra    = true;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_serlot  = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_ttNum    = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_dokDate  = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_kpdbName = false;
   }
  
   private void radioButtonSortBySerlot_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Rtrans.sorterSerlot;
 
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_naziv    = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_sifra    = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_serlot   = true;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_ttNum    = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_dokDate  = false;
      ZXC.TheVvForm.VvPref.findRtrans.IsFindBy_kpdbName = false;
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
      
      //this.ControlForInitialFocus = tbxTtNum;

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

      //VvHamper.Open_Close_Fields_ForWriting(tbxTtNum, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
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
            Fld_PartnerName = kupdob_rec.Naziv;
            Fld_PartnerCD   = kupdob_rec.KupdobCD;
            Fld_PartnerTK   = kupdob_rec.Ticker;
         }
         else
         {
            Fld_PartnerName = Fld_PartnerTK = Fld_PartnerCdAsTxt = "";
         }
      }
   }


   #endregion Eveniti

   #region DataGridView

   protected override void CreateDataGridViewColumn()
   {
      int sumOfColWidth = 0, colWidth;
      int colDateWidth = ZXC.Q4un;
      int colSif6Width = ZXC.Q3un + ZXC.Qun2;

      sumOfColWidth += TheGrid.RowHeadersWidth;

      if(IsArhivaTabPage)
      {
         AddDGV_ArhivaColumns(ref sumOfColWidth);
      }

      
      colWidth = colSif6Width;                                  AddDGVColum_RecID_4GridReadOnly   (TheGrid, "RecID"    , colWidth, false, 0, "recID");

      colWidth = ZXC.Q6un          ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Šifra Artikla"   , colWidth, false  , "t_artiklCD"  );
      colWidth = ZXC.Q9un          ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Naziv Artikla"   , colWidth, true   , "t_artiklName");
      colWidth = ZXC.Q4un          ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Serlot"          , colWidth, false  , "t_serlot"    );
      colWidth = ZXC.Q4un          ; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (TheGrid, "ŠifPart"         , colWidth, true, 6, "t_kupdob_cd" );
      colWidth = ZXC.Q7un          ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Naziv Partnera"  , colWidth, false  , "ext_kpdbName");
      colWidth = ZXC.Q2un          ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "TT"              , colWidth, false  , "t_tt"        );
      colWidth = ZXC.Q3un          ; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (TheGrid, "TT Broj"         , colWidth, true, 6, "t_ttNum"     );
      colWidth = ZXC.Q4un          ; sumOfColWidth += colWidth; AddDGVColum_DateTime_4GridReadOnly(TheGrid, "Datum"           , colWidth         , "t_skladDate" );

      colWidth = colSif6Width;                                  AddDGVColum_RecID_4GridReadOnly   (TheGrid, "ParentRecID"    , colWidth, false, 0, "t_parentID");

      grid_Width = sumOfColWidth + ZXC.QUN;
   }

   #endregion DataGridView

   #region Fld_

   public string SelectedTT
   {
      get
      {
         if(TheGrid.CurrentRow != null)
            return (TheGrid.CurrentRow.Cells["t_tt"].Value.ToString());
         else
            return "";
      }
   }

   public uint SelectedParentID
   {
      get
      {
         if(TheGrid.CurrentRow != null)
            return ZXC.ValOrZero_UInt(TheGrid.CurrentRow.Cells["t_parentID"].Value.ToString());
         else
            return 0 ;
      }
   }

   // 30.12.2012:
 //public uint     Fld_FromTtNum             { get { return tbx_ttNum.GetSomeRecIDField();           } set { tbx_ttNum.PutSomeRecIDField(value);            }   }
   public uint     Fld_FromTtNum             
   { 
      get 
      {
         uint fldTextAsUint = tbx_ttNum.GetSomeRecIDField();

         if(fldTextAsUint.NotZero() && 
            ZXC.TtInfo(Fld_FromTT).IsSklCdInTtNum && 
            fldTextAsUint < Faktur.BaseTtNum)
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

   public string   Fld_FromTT          { get { return tbx_TT        .Text ; } set { tbx_TT        .Text  = value; } }
   public DateTime Fld_FromDokDate     { get { return dtp_dokDate   .Value; } set { dtp_dokDate   .Value = value; } }
   public string   Fld_FromPartner     { get { return tbx_PartnerName   .Text ; } set { tbx_PartnerName   .Text  = value; } }
   public string   Fld_FromArtName        { get { return tbx_artiklName.Text ; } set { tbx_artiklName.Text  = value; } }
   public string   Fld_FromArtiklCD    { get { return tbx_artiklCD  .Text ; } set { tbx_artiklCD  .Text  = value; } }
   public string   Fld_FromSerlot      { get { return tbx_serlot    .Text ; } set { tbx_serlot    .Text  = value; } }

   //public bool Fld_BiloGdjeUArtiklName { get { return cbx_biloGdjeUArtiklName.Checked; } set { cbx_biloGdjeUArtiklName.Checked = value; } }
   //public bool Fld_BiloGdjeKupDobName  { get { return cbx_biloGdjeUnazivu    .Checked; } set { cbx_biloGdjeUnazivu    .Checked = value; } }

   public uint     Fld_PartnerCD         { get { return tbx_PartnerCD  .GetSomeRecIDField();   } set { tbx_PartnerCD  .PutSomeRecIDField(value);    }   }
   public string   Fld_PartnerCdAsTxt    { get { return tbx_PartnerCD  .Text;                  } set { tbx_PartnerCD  .Text = value;                }   }
   public string   Fld_PartnerName       { get { return tbx_PartnerName.Text;                  } set { tbx_PartnerName.Text = value;              }   }
   public string   Fld_PartnerTK         { get { return tbx_PartnerTK  .Text;                  } set { tbx_PartnerTK  .Text = value;              }   }

    


   #endregion Fld_

   #region Overriders and specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.rtrans_rec; }
      set {        this.rtrans_rec = (Rtrans)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.RtransDao; }
   }

   private Vektor.DataLayer.DS_FindRecord.DS_findRtrans ds_rtrans;

   protected override DataSet VirtualUntypedDataSet { get { return ds_rtrans; } }

   protected override object[] From_IndexSegmentValues
   {
      get
      {
         switch(recordSorter.SortType)
         {
            case VvSQL.SorterType.Code    : return new object[] { Fld_FromArtiklCD, "", Fld_FromDokDate.Date, ZXC.TtInfo(Fld_FromTT).TtSort, Fld_FromTtNum, 0 };
            case VvSQL.SorterType.Name    : return new object[] { Fld_FromArtName ,     Fld_FromDokDate.Date, ZXC.TtInfo(Fld_FromTT).TtSort, Fld_FromTtNum, 0 };
            case VvSQL.SorterType.TtNum   : return new object[] {                                             ZXC.TtInfo(Fld_FromTT).TtSort, Fld_FromTtNum, 0 };
            case VvSQL.SorterType.DokDate : return new object[] {                       Fld_FromDokDate.Date, ZXC.TtInfo(Fld_FromTT).TtSort, Fld_FromTtNum, 0 };
            case VvSQL.SorterType.KpdbName: return new object[] { Fld_PartnerCD   ,     Fld_FromDokDate.Date, ZXC.TtInfo(Fld_FromTT).TtSort, Fld_FromTtNum, 0 }; // nije name nego cd ! 
            case VvSQL.SorterType.Serlot  : return new object[] { Fld_FromSerlot  , "", Fld_FromDokDate.Date, ZXC.TtInfo(Fld_FromTT).TtSort, Fld_FromTtNum, 0 };

            default: ZXC.aim_emsg("Q42: SortType [{0}] undifajnd in property 'From_IndexSegmentValues'", recordSorter.SortType); return null;
         }
      }
   }

   public override void SetListFilterRecordDependentDefaults()
   {
      //if(Default_TT == "UNDEF") Fld_FromTT = Fld_FilterTT = "";
      //else                      Fld_FromTT = Fld_FilterTT = Default_TT;
   }

   protected override VvTextBox VvTbx_Virtual_TT       { get { return this.tbx_TT; } }
 //protected override VvTextBox VvTbx_VirtualFilter_TT { get { return this.tbx_filterTT; } set { this.tbx_filterTT = value; } }
 //protected override VvTextBox VvTbx_VirtualFilter_TT { get; set; }
   protected override VvTextBox VvTbx_VirtualFilter_TT { get { return this.tbx_TT; } set { this.tbx_TT = value; } }

   #endregion Overriders and specifics


   #region AddFilterMemberz()

   #endregion AddFilterMemberz()

}


public class RtranoListUC : VvRecLstUC
{
   #region Fieldz

   protected Rtrano rtrano_rec;
   public VvTextBox tbx_serno;
   
   #endregion Fieldz

   #region Constructor

   public RtranoListUC(Control parent, Rtrano _rtrano, VvForm.VvSubModul vvSubModul ) : base(parent)
   {
      this.rtrano_rec  = _rtrano;
      this.Parent.Text = "Serno info";

      this.MasterSubModulEnum = ZXC.VvSubModulEnum.ART;
      this.TheSubModul        = vvSubModul;

      TheFilterMembers = new System.Collections.Generic.List<VvSqlFilterMember>(); // namoj tamo di ne treba (npr Kplan) 

      hampOpenUtil.Visible   = hampUtil.Visible = hampListaRastePada.Visible =  false;
      grBoxLimitiraj.Visible = false;

      SetControlForInitialFocus();

   }

   protected override void InitializeFindFormSpecifics()
   {
      recordSorter = Rtrano.sorterSerno;

      this.ds_sernoRtrano = new Vektor.DataLayer.DS_FindRecord.DS_findSernoRtrano();

      this.Name = "SernoRtranoListUC";
      this.Text = "SernoRtranoListUC";
   }

   #endregion Constructor

   #region Hamperi

   private void SetControlForInitialFocus()
   {
      this.ControlForInitialFocus = tbx_serno;
   }

   protected override void CreateHamperSpecifikum()
   {
      hampSpecifikum = new VvHamper(2, 1, "", this, true, ZXC.Qun4, nextY, razmakHamp);
      //                                              0       1     
      hampSpecifikum.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q7un};
      hampSpecifikum.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4};
      hampSpecifikum.VvRightMargin = hampSpecifikum.VvLeftMargin;

      hampSpecifikum.VvRowHgt       = new int[] { ZXC.QUN };
      hampSpecifikum.VvSpcBefRow    = new int[] { ZXC.Qun4};
      hampSpecifikum.VvBottomMargin = hampSpecifikum.VvTopMargin;

                  hampSpecifikum.CreateVvLabel   (0, 0, "Serijski broj:", ContentAlignment.MiddleRight);
      tbx_serno = hampSpecifikum.CreateVvTextBox (1, 0, "tbx_serno", "", 32);
      tbx_serno.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);
       
      //tbx_sernoFilter = hampSpecifikum.CreateVvTextBox (1, 0, "tbx_sernoFilter", "", 32);
      //tbx_sernoFilter.Visible = false;
      VvHamper.Open_Close_Fields_ForWriting(tbx_serno     , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvFindDialog);
   }

   #endregion Hamperi

   #region Eveniti

   #endregion Eveniti

   #region DataGridView
   protected override void CreateDataGridViewColumn()
   {
      int sumOfColWidth = 0, colWidth;
      int colDateWidth = ZXC.Q4un;
      int colSif6Width = ZXC.Q3un + ZXC.Qun2;

      sumOfColWidth += TheGrid.RowHeadersWidth;

      if(IsArhivaTabPage)
      {
         AddDGV_ArhivaColumns(ref sumOfColWidth);
      }

      
      colWidth = colSif6Width;                                  AddDGVColum_RecID_4GridReadOnly   (TheGrid, "RecID"    , colWidth, false, 0, "recID");
      
      colWidth = ZXC.Q5un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Serno"           , colWidth, false  , "t_serno"     );
      colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Šifra Artikla"   , colWidth, false  , "t_artiklCD"  );
      colWidth = ZXC.Q9un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Naziv Artikla"   , colWidth, true   , "t_artiklName");
    //colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly (TheGrid, "RAM"             , colWidth,     0  , "t_dimZ"      );
    //colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly (TheGrid, "HDD"             , colWidth,     0  , "t_decC"      );
      colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly (TheGrid, "RAM"             , colWidth,     0  , "ext_PCK_RAM" );
      colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly (TheGrid, "HDD"             , colWidth,     0  , "ext_PCK_HDD" );
      colWidth = ZXC.Q4un; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (TheGrid, "ŠifPart"         , colWidth, true, 6, "t_kupdob_cd" );
      colWidth = ZXC.Q7un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Naziv Partnera"  , colWidth, false  , "ext_kpdbName");
      colWidth = ZXC.Q2un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "TT"              , colWidth, false  , "t_tt"        );
      colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (TheGrid, "TT Broj"         , colWidth, true, 6, "t_ttNum"     );
      colWidth = ZXC.Q4un; sumOfColWidth += colWidth; AddDGVColum_DateTime_4GridReadOnly(TheGrid, "Datum"           , colWidth         , "t_skladDate" );

      colWidth = colSif6Width;                        AddDGVColum_RecID_4GridReadOnly   (TheGrid, "ParentID"       , colWidth, false, 0, "t_parentID");

      grid_Width = sumOfColWidth + ZXC.QUN;
   }

   #endregion DataGridView

   #region Fld_

   public string Fld_SerNo       { get { return tbx_serno.Text; } set { tbx_serno.Text = value; } }

   public string SelectedTT
   {
      get
      {
         if(TheGrid.CurrentRow != null)
            return (TheGrid.CurrentRow.Cells["t_tt"].Value.ToString());
         else
            return "";
      }
   }

   public uint SelectedParentID
   {
      get
      {
         if(TheGrid.CurrentRow != null)
            return ZXC.ValOrZero_UInt(TheGrid.CurrentRow.Cells["t_parentID"].Value.ToString());
         else
            return 0 ;
      }
   }

   #endregion Fld_

   #region Overriders and specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.rtrano_rec; }
      set {        this.rtrano_rec = (Rtrano)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.RtranoDao; }
   }

   private Vektor.DataLayer.DS_FindRecord.DS_findSernoRtrano ds_sernoRtrano;

   protected override DataSet VirtualUntypedDataSet { get { return ds_sernoRtrano; } }

   protected override object[] From_IndexSegmentValues
   {
      get
      {
         switch(recordSorter.SortType)
         {
            //case VvSQL.SorterType.Code    : return new object[] { Fld_FromArtiklCD, "", Fld_FromDokDate.Date, ZXC.TtInfo(Fld_FromTT).TtSort, Fld_FromTtNum, 0 };
            //case VvSQL.SorterType.Name    : return new object[] { Fld_FromArtName ,     Fld_FromDokDate.Date, ZXC.TtInfo(Fld_FromTT).TtSort, Fld_FromTtNum, 0 };
            //case VvSQL.SorterType.TtNum   : return new object[] {                                             ZXC.TtInfo(Fld_FromTT).TtSort, Fld_FromTtNum, 0 };
            //case VvSQL.SorterType.DokDate : return new object[] {                       Fld_FromDokDate.Date, ZXC.TtInfo(Fld_FromTT).TtSort, Fld_FromTtNum, 0 };
            //case VvSQL.SorterType.KpdbName: return new object[] { Fld_PartnerCD   ,     Fld_FromDokDate.Date, ZXC.TtInfo(Fld_FromTT).TtSort, Fld_FromTtNum, 0 }; // nije name nego cd ! 
            //case VvSQL.SorterType.Serlot  : return new object[] { Fld_FromSerlot  , "", Fld_FromDokDate.Date, ZXC.TtInfo(Fld_FromTT).TtSort, Fld_FromTtNum, 0 };
                        
            case VvSQL.SorterType.Serno   : return new object[] { Fld_SerNo,            DateTime.MinValue, 0, 0, 0 };

            default: ZXC.aim_emsg("Q42: SortType [{0}] undifajnd in property 'From_IndexSegmentValues'", recordSorter.SortType); return null;
         }
      }
   }

   public override void SetListFilterRecordDependentDefaults()
   {
      //if(Default_TT == "UNDEF") Fld_FromTT = Fld_FilterTT = "";
      //else                      Fld_FromTT = Fld_FilterTT = Default_TT;
   }

   //protected override VvTextBox VvTbx_Virtual_TT       { get { return this.tbx_TT; } }
   //protected override VvTextBox VvTbx_VirtualFilter_TT { get { return this.tbx_TT; } set { this.tbx_TT = value; } }

   #endregion Overriders and specifics

   #region AddFilterMemberz()
   public DataRowCollection RtranoSchemaRows
   {
      get { return ZXC.RtranoDao.TheSchemaTable.Rows; }
   }

   public RtranoDao.RtranoCI RtranoCI
   {
      get { return ZXC.RtranoDao.CI; }
   }

   public override void AddFilterMemberz()
   {
      string text;
      
      DataRow drSchema;

      this.TheFilterMembers.Clear();

      // Fld_FilterTT                                                                                                                                          

      drSchema = RtranoSchemaRows[RtranoCI.t_serno];
      text = Fld_SerNo;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Serno", text, " = "));
      }
   }

   #endregion AddFilterMemberz()

}
