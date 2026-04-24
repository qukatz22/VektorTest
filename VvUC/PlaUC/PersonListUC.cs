using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Drawing;

public class PersonListUC : VvRecLstUC
{
   #region Fieldz

   private RadioButton rBtSortByPrezime, rBtSortBySifra, 
                       rBtcurrChecked,
                       rbt_tekuci, rbt_banka, rbt_gotovina, rbt_nebitno,
                       rbt_svi, rbt_izuzet, rbt_neIzuzet;
  
   private VvTextBox   tbx_prezime, tbx_personCD,
                       tbx_jmbg, tbx_oib, tbx_regob, tbx_osBrOsig,
                       tbx_banka_cd, tbx_banka_tk, tbx_banka_Naziv,
                       tbx_mtros_cd, tbx_mtros_tk, tbx_mtros_Naziv, tbx_strSpr, tbx_strSprCd,
                       tbx_radMj, tbx_napomena, tbx_ts,
                       tbx_isPlaca, tbx_isUgDj, tbx_isAutH, tbx_isPoduz, tbx_isNadzO,
                       tbx_osnovno,tbx_evr, tbx_obu ;

   private CheckBox    cbx_biloGdjeUprezimenu, cbx_biloGdjeURadMjesto, cbx_biloGdjeUnapomeni,
                       cbx_isPlaca, cbx_isUgDj, cbx_isAutH, cbx_isPoduz, cbx_isNadzO,cbx_isIDDk4,
                       cbx_osnovno, cbx_evr,cbx_obu ;

   private Person      person_rec;

   #endregion Fieldz

   #region Constructor

   public PersonListUC(Control parent, Person _person, VvSubModul vvSubModul): base(parent)
   {
      this.person_rec = _person;

      this.MasterSubModulEnum = ZXC.VvSubModulEnum.PER;
      this.TheSubModul        = vvSubModul;
      this.Parent.Text        = this.TheSubModul.subModul_name;

      TheFilterMembers = new System.Collections.Generic.List<VvSqlFilterMember>(); // namoj tamo di ne treba (npr Kplan) 
   }

   #endregion Constructor

   #region InitializeFindForm

   protected override void InitializeFindFormSpecifics()
   {
      recordSorter  = Person.sorterPrezime;

     this.ds_person = new Vektor.DataLayer.DS_FindRecord.DS_findPerson();

      this.Name = "PersonListUC";
      this.Text = "PersonListUC";
   }

   #endregion InitializeFindForm

   #region DataGridView

   protected override void CreateDataGridViewColumn()
   {
      int sumOfColWidth = 0, colWidth;
  
      sumOfColWidth += TheGrid.RowHeadersWidth;

      if(IsArhivaTabPage)
      {
         AddDGV_ArhivaColumns(ref sumOfColWidth);
      }

      colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly(TheGrid, "Šifra"  , colWidth, true , 4, "personCD");
      colWidth = ZXC.Q7un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly (TheGrid, "Prezime", colWidth, true    , "prezime");
      colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly (TheGrid, "Ime"    , colWidth, false   , "ime");
      colWidth = ZXC.Q7un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly (TheGrid, "Adresa" , colWidth, false   , "ulica");
      colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly (TheGrid, "Grad"   , colWidth, false   , "grad");
      colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly (TheGrid, "Telefon", colWidth, false   , "tel");
      colWidth = ZXC.Q3un;                            AddDGVColum_RecID_4GridReadOnly  (TheGrid, "RecID"  , colWidth, false, 0, "recID");

      grid_Width = sumOfColWidth + ZXC.QUN;
   }

   #endregion DataGridView

   #region Hamper

   protected override void CreateHamperSpecifikum()
   {
      hampSpecifikum = new VvHamper(3, 2, "", this, true, hampListaRastePada.Right + ZXC.Qun4, nextY, razmakHamp);

      hampSpecifikum.VvColWdt      = new int[] { ZXC.Q5un, ZXC.QUN - ZXC.Qun8, ZXC.Q7un };
      hampSpecifikum.VvSpcBefCol   = new int[] { ZXC.Qun4,          ZXC.Qun12, ZXC.Qun12 };
      hampSpecifikum.VvRightMargin = hampSpecifikum.VvLeftMargin;

      hampSpecifikum.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN };
      hampSpecifikum.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hampSpecifikum.VvBottomMargin = hampSpecifikum.VvTopMargin;

      rBtSortByPrezime       = hampSpecifikum.CreateVvRadioButton(0, 0, new EventHandler(radioButtonSortByPrezime_Click), "Od prezimena:", TextImageRelation.ImageAboveText);
      cbx_biloGdjeUprezimenu = hampSpecifikum.CreateVvCheckBox_OLD   (1, 0, CheckBox_biloGdjeUnazivu_Click, "", RightToLeft.No);
      tbx_prezime            = hampSpecifikum.CreateVvTextBox    (2, 0, "tbx_prezime", "");
      cbx_biloGdjeUprezimenu.Checked = true;
      SetVvPrefLink_AND_ToolTipText_ForGenericBiloGdjeUnazivuCheckBox(cbx_biloGdjeUprezimenu, new EventHandler(cbx_biloGdjeUprezimenu_Click_SaveToVvPref));

      rBtSortByPrezime.Checked = true;
      tbx_prezime.DoubleClick += new EventHandler(tbx_DoubleClick);
      tbx_prezime.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

      tbx_prezime.Tag      = rBtSortByPrezime;
      rBtSortByPrezime.Tag = tbx_prezime;

      rBtSortBySifra = hampSpecifikum.CreateVvRadioButton(0, 1, new EventHandler(radioButtonSortBySifra_Click), "Od šifre:", TextImageRelation.ImageAboveText);
      tbx_personCD   = hampSpecifikum.CreateVvTextBox    (2, 1, "tbx_personCD", "");
      tbx_personCD.DoubleClick += new EventHandler(tbx_DoubleClick);
      tbx_personCD.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

      tbx_personCD.Tag      = rBtSortBySifra;
      rBtSortBySifra.Tag = tbx_personCD;
      
      this.ControlForInitialFocus = tbx_prezime;

      VvHamper.Open_Close_Fields_ForWriting(tbx_prezime , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_personCD, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
   }

   #endregion Hamper

   #region Eveniti

   private void radioButtonSortByPrezime_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Person.sorterPrezime;
   }
 
   private void radioButtonSortBySifra_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Person.sorterCD;
   }

   private void tbx_DoubleClick(object sender, System.EventArgs e)
   {
      VvTextBox vvTb  = sender as VvTextBox;
      RadioButton rbt = (RadioButton)vvTb.Tag;

      if (!rbt.Checked)
           rbt.PerformClick();
   }
   public void cbx_biloGdjeUprezimenu_Click_SaveToVvPref(object sender, EventArgs e)
   {
     // ZXC.TheVvForm.VvPref.findpOperana.IsBiloGdjePrezimenu = OperAna.sorterPrezime.BiloGdjeU_Tekstu = Fld_BiloGdjePrezimenu;
   }

   #endregion Eveniti

   #region HamperFilter

   protected override void CreateHamperFilter()
   {
      CreateHamperOpenFilter();

      hampFilter = new VvHamper(11, 6, "", this, true, hampOpenFilter.Left, hampOpenFilter.Top, razmakHamp);

      hampFilter.VvColWdt      = new int[] { ZXC.Q4un + ZXC.Qun2, ZXC.Q3un, ZXC.Q5un + ZXC.Qun8, ZXC.Q5un, ZXC.QUN - ZXC.Qun8, ZXC.Q2un + ZXC.Qun8, ZXC.Q3un + ZXC.Qun8, ZXC.Q7un, ZXC.QUN - ZXC.Qun4, ZXC.Q5un, ZXC.Q4un };
      hampFilter.VvSpcBefCol   = new int[] {            ZXC.QUN , ZXC.Qun4, ZXC.Qun4           , ZXC.Qun4,          ZXC.Qun12,            ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4,           ZXC.QUN , ZXC.Qun4, ZXC.Qun2};
      hampFilter.VvRightMargin = hampFilter.VvLeftMargin;

      hampFilter.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN, ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN };
      hampFilter.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4,ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hampFilter.VvBottomMargin = hampFilter.VvTopMargin;

      Label lbl_jmbg, lbl_oib, lbl_regob, lbl_osBrOsig, lblIzlistaj;

         VvHamper inerHamper      = new VvHamper(1, 4, "", hampFilter, false, 0, ZXC.Q2un - ZXC.Qun4, ZXC.Qun4);
         inerHamper.VvColWdt      = new int[] { ZXC.Q3un};
         inerHamper.VvSpcBefCol   = new int[] { ZXC.QUN };
         inerHamper.VvRightMargin = inerHamper.VvLeftMargin;

         inerHamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN };
         inerHamper.VvSpcBefRow    = new int[] { ZXC.Qun5, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
         inerHamper.VvBottomMargin = inerHamper.VvTopMargin;

         Label lblX   = inerHamper.CreateVvLabel      (0, 0, "Radnici:", ContentAlignment.MiddleLeft);
         rbt_svi      = inerHamper.CreateVvRadioButton(0, 1, null, "Svi", TextImageRelation.ImageBeforeText);
         rbt_neIzuzet = inerHamper.CreateVvRadioButton(0, 2, null, "Aktivni", TextImageRelation.ImageBeforeText);
         rbt_izuzet   = inerHamper.CreateVvRadioButton(0, 3, null, "Izuzeti", TextImageRelation.ImageBeforeText);
         rbt_svi.Checked = true;
         rbt_svi.Tag     = true;
         inerHamper.BringToFront();

      lbl_jmbg     = hampFilter.CreateVvLabel(1, 1, "JMBG:"    , ContentAlignment.MiddleRight);
      lbl_oib      = hampFilter.CreateVvLabel(1, 2, "OIB:"     , ContentAlignment.MiddleRight);
      lbl_regob    = hampFilter.CreateVvLabel(1, 3, "REGOB:"   , ContentAlignment.MiddleRight);
      lbl_osBrOsig = hampFilter.CreateVvLabel(1, 4, "OsBrOsig:", ContentAlignment.MiddleRight);

      tbx_jmbg     = hampFilter.CreateVvTextBox(2, 1, "tbx_jmbg"    , "Jedinstveni matični broj građana");
      tbx_oib      = hampFilter.CreateVvTextBox(2, 2, "tbx_oib"     , "Osobni identifikacijski broj"    );
      tbx_regob    = hampFilter.CreateVvTextBox(2, 3, "tbx_regob"   , "Registarski broj obveznika"      );
      tbx_osBrOsig = hampFilter.CreateVvTextBox(2, 4, "tbx_osBrOsig", "Osobni broj osiguranika"         );

      tbx_jmbg.JAM_CharEdits     = 
      tbx_oib.JAM_CharEdits      = 
      tbx_regob.JAM_CharEdits    = 
      tbx_osBrOsig.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

         VvHamper dlgHamper      = new VvHamper(2, 4, "", hampFilter, false, inerHamper.Right, inerHamper.Top, ZXC.Qun4);
         
         dlgHamper.VvColWdt      = new int[] {  ZXC.QUN - ZXC.Qun4, ZXC.Q6un };
         dlgHamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
         dlgHamper.VvRightMargin = dlgHamper.VvLeftMargin;

         dlgHamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN };
         dlgHamper.VvSpcBefRow    = new int[] { ZXC.Qun5, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
         dlgHamper.VvBottomMargin = dlgHamper.VvTopMargin;

         Label lblEvr  = dlgHamper.CreateVvLabel(0, 0, "Sa prethodnog dokumenta:", 1, 0, ContentAlignment.MiddleLeft);

         Label lblp = dlgHamper.CreateVvLabel(1, 1, "Osnovno", ContentAlignment.MiddleLeft);
         tbx_osnovno = dlgHamper.CreateVvTextBox(0, 1, "tbx_osnovno", "");
         cbx_osnovno = dlgHamper.CreateVvCheckBox(0, 1, "", tbx_osnovno, "", "X");
         tbx_osnovno.JAM_Highlighted = true;

         Label lble = dlgHamper.CreateVvLabel(1, 2, "Evidencija rada", ContentAlignment.MiddleLeft);
         tbx_evr = dlgHamper.CreateVvTextBox(0, 2, "tbx_evr", "");
         cbx_evr = dlgHamper.CreateVvCheckBox(0, 2, "", tbx_evr, "", "X");
         tbx_evr.JAM_Highlighted = true;

         Label lblo = dlgHamper.CreateVvLabel(1, 3, "Obustave", ContentAlignment.MiddleLeft);
         tbx_obu = dlgHamper.CreateVvTextBox(0, 3, "tbx_obu", "");
         cbx_obu = dlgHamper.CreateVvCheckBox(0, 3, "", tbx_obu, "", "X");
         tbx_obu.JAM_Highlighted = true;


      Label lbl_mtros_cd, lbl_strSpr, lbl_radMj, lbl_banka_cd, lbl_napom, lbl_ts;

      lbl_ts       = hampFilter.CreateVvLabel(3, 0, "Tip Person:"     , ContentAlignment.MiddleRight);
      lbl_strSpr   = hampFilter.CreateVvLabel(3, 1, "Strucna sprema:" , ContentAlignment.MiddleRight);
      lbl_mtros_cd = hampFilter.CreateVvLabel(3, 2, "Mjesto troška:", ContentAlignment.MiddleRight);
      lbl_radMj    = hampFilter.CreateVvLabel(3, 3, "Radno mjesto:"   , ContentAlignment.MiddleRight);
      lbl_banka_cd = hampFilter.CreateVvLabel(3, 4, "Banka:"          , ContentAlignment.MiddleRight);
      lbl_napom    = hampFilter.CreateVvLabel(3, 5, "Napomena:"       , ContentAlignment.MiddleRight);

      tbx_ts     = hampFilter.CreateVvTextBoxLookUp("tbx_ts", 4, 0, "Šifra tipa",6, 1, 0);
      tbx_ts.JAM_Set_LookUpTable(ZXC.luiListaPersonTS, (int)ZXC.Kolona.prva);
      tbx_ts.JAM_lookUp_NOTobligatory  = true;
      tbx_ts.JAM_lookUp_MultiSelection = true;
      tbx_ts.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_strSprCd = hampFilter.CreateVvTextBoxLookUp("tbx_strSprCd", 4, 1, "Šifra stručne spreme", 6, 1, 0);
      tbx_strSpr   = hampFilter.CreateVvTextBox(6, 1, "tbx_strSpr", "Stručna sprema", 32, 1, 0);
      tbx_strSprCd.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_strSpr.JAM_ReadOnly    = true;
      tbx_strSprCd.JAM_Set_LookUpTable(ZXC.luiListaStrSprema, (int)ZXC.Kolona.prva);
      tbx_strSprCd.JAM_lui_NameTaker_JAM_Name = tbx_strSpr.JAM_Name;

      tbx_mtros_cd    = hampFilter.CreateVvTextBox(4, 2, "tbx_mtros_cd"   , "Šifra mjesta troška", 6,1,0);
      tbx_mtros_tk    = hampFilter.CreateVvTextBox(6, 2, "tbx_mtros_tk"   , "Tiker mjesta troška"       );
      tbx_mtros_Naziv = hampFilter.CreateVvTextBox(7, 2, "tbx_mtros_naziv", "Naziv mjesta troška"       );

      tbx_radMj       = hampFilter.CreateVvTextBox(5, 3, "tbx_radMj", "Opis radnog mjesta", 32, 2, 0);
      cbx_biloGdjeURadMjesto = hampFilter.CreateVvCheckBox_OLD(4, 3, CheckBox_biloGdjeUnazivu_Click, "", RightToLeft.No);
      cbx_biloGdjeURadMjesto.Checked = true;

      tbx_mtros_cd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_mtros_tk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_mtros_cd.   JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.    SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra)  , new EventHandler(AnyMtrosTextBoxLeave));
      tbx_mtros_tk.   JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_mtros_Naziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv. SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)   , new EventHandler(AnyMtrosTextBoxLeave));

      tbx_banka_cd    = hampFilter.CreateVvTextBox(4, 4, "tbx_bankaCd"   , "Šifra banke", 6, 1, 0);
      tbx_banka_tk    = hampFilter.CreateVvTextBox(6, 4, "tbx_bankaTk"   , "Tiker banke"         );
      tbx_banka_Naziv = hampFilter.CreateVvTextBox(7, 4, "tbx_bankaNaziv", "Naziv banke"         );

      tbx_banka_cd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_banka_tk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_banka_cd.   JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType    , ZXC.AutoCompleteRestrictor.KID_Banka_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyKupdobTextBoxLeave));
      tbx_banka_tk.   JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Banka_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_banka_Naziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , ZXC.AutoCompleteRestrictor.KID_Banka_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyKupdobTextBoxLeave));

      tbx_napomena = hampFilter.CreateVvTextBox(5, 5, "tbx_napomena", "Napomena",  32, 2,0);
      cbx_biloGdjeUnapomeni = hampFilter.CreateVvCheckBox_OLD(4, 5, CheckBox_biloGdjeUnazivu_Click, "", RightToLeft.No);
      cbx_biloGdjeUnapomeni.Checked = true;

      Label lbl   = hampFilter.CreateVvLabel   (8, 0,"Vrsta rada:", 1, 0, ContentAlignment.MiddleLeft);

      Label lbl1  = hampFilter.CreateVvLabel   (9, 1, "Plaća", ContentAlignment.MiddleLeft);
      tbx_isPlaca = hampFilter.CreateVvTextBox (8, 1, "tbx_isPlaca", "");
      cbx_isPlaca = hampFilter.CreateVvCheckBox(8, 1, "", tbx_isPlaca, "", "X");
      tbx_isPlaca.JAM_Highlighted = true;

      Label lbl2 = hampFilter.CreateVvLabel   (9, 2, "Autorski honorar", ContentAlignment.MiddleLeft);
      tbx_isAutH = hampFilter.CreateVvTextBox (8, 2, "tbx_isAutH", "");
      cbx_isAutH = hampFilter.CreateVvCheckBox(8, 2, "", tbx_isAutH, "", "X");
      tbx_isAutH.JAM_Highlighted = true;

      Label lbl3 = hampFilter.CreateVvLabel   (9, 3, "Ugovor o djelu", ContentAlignment.MiddleLeft);
      tbx_isUgDj = hampFilter.CreateVvTextBox (8, 3, "tbx_isUgDj", "");
      cbx_isUgDj = hampFilter.CreateVvCheckBox(8, 3, "", tbx_isUgDj, "", "X");
      tbx_isUgDj.JAM_Highlighted = true;

      Label lbl4  = hampFilter.CreateVvLabel   (9, 4, "Poduzetnička plaća", ContentAlignment.MiddleLeft);
      tbx_isPoduz = hampFilter.CreateVvTextBox (8, 4, "tbx_isPoduz", "");
      cbx_isPoduz = hampFilter.CreateVvCheckBox(8, 4, "", tbx_isPoduz, "", "X");
      tbx_isPoduz.JAM_Highlighted = true;

      Label lbl5  = hampFilter.CreateVvLabel   (9, 5, "Nadzorni odbor", ContentAlignment.MiddleLeft);
      tbx_isNadzO = hampFilter.CreateVvTextBox (8, 5, "tbx_isNadzO", "");
      cbx_isNadzO = hampFilter.CreateVvCheckBox(8, 5, "", tbx_isNadzO, "", "X");
      tbx_isNadzO.JAM_Highlighted = true;


      Label lblvi  = hampFilter.CreateVvLabel      (10, 0, "Vrsta isplate:", ContentAlignment.MiddleLeft);
      rbt_nebitno  = hampFilter.CreateVvRadioButton(10, 1, null, "Sve"     , TextImageRelation.ImageBeforeText);
      rbt_tekuci   = hampFilter.CreateVvRadioButton(10, 2, null, "Tekući"  , TextImageRelation.ImageBeforeText);
      rbt_banka    = hampFilter.CreateVvRadioButton(10, 3, null, "Banka"   , TextImageRelation.ImageBeforeText);
      rbt_gotovina = hampFilter.CreateVvRadioButton(10, 4, null, "Gotovina", TextImageRelation.ImageBeforeText);
      rbt_nebitno.Checked = true;
      rbt_nebitno.Tag     = true; 

      VvHamper.Open_Close_Fields_ForWriting(hampFilter, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      if(this.Parent is VvDialog)
      {
         lblIzlistaj = hampFilter.CreateVvLabel(0, 0, "", System.Drawing.ContentAlignment.MiddleRight);
         
         hampFilter.Visible = true;

         lbl_jmbg.Visible = lbl_oib.Visible = lbl_regob.Visible = lbl_osBrOsig.Visible = 
         tbx_jmbg.Visible = tbx_oib.Visible = tbx_regob.Visible = tbx_osBrOsig.Visible =  false;

         dlgHamper.Visible = true;
         dlgHamper.BringToFront();

         rbt_neIzuzet.Checked = true;
         rbt_neIzuzet.Tag     = true;

         VvHamper.Open_Close_Fields_ForWriting(tbx_isPlaca , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
         VvHamper.Open_Close_Fields_ForWriting(tbx_isAutH  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
         VvHamper.Open_Close_Fields_ForWriting(tbx_isUgDj  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
         VvHamper.Open_Close_Fields_ForWriting(tbx_isPoduz , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
         VvHamper.Open_Close_Fields_ForWriting(tbx_isNadzO , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
         VvHamper.Open_Close_Fields_ForWriting(inerHamper  , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvDialog);
      }
      else
      {
         lblIzlistaj = hampFilter.CreateVvLabel(0, 0, "IZLISTAJ SAMO:", System.Drawing.ContentAlignment.MiddleLeft);
         hampFilter.Visible = false;
         dlgHamper. Visible = false;
      }
   }
 
   void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      //if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob  kupdob_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_FilterBankaCd = kupdob_rec.KupdobCD/*RecID*/;
            Fld_FilterBankaTk = kupdob_rec.Ticker;
            Fld_FilterBankaNaziv = kupdob_rec.Naziv;
         }
         else
         {
            Fld_FilterBankaCdAsTxt = Fld_FilterBankaTk = Fld_FilterBankaNaziv = "";
         }
      }
   }

   void AnyMtrosTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      //if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_FilterMtrosCd = kupdob_rec.KupdobCD/*RecID*/;
            Fld_FilterMtrosTk = kupdob_rec.Ticker;
            Fld_FilterMtrosNaziv = kupdob_rec.Naziv;
         }
         else
         {
            Fld_FilterMtrosCdAsTxt = Fld_FilterMtrosTk = Fld_FilterMtrosNaziv = "";
         }
      }
   }

   #endregion HamperFilter

   #region Fld_
   
   public string Fld_FromPrezime
   {
      get { return tbx_prezime.Text; }
      set {        tbx_prezime.Text = value; }
   }

   public uint Fld_FromPersonCD
   {
      get { return ZXC.ValOrZero_UInt(tbx_personCD.Text); }
      set {                           tbx_personCD.Text = value.ToString("0000"); }
   }

   public bool Fld_BiloGdjeUprezimenu
   {
      get { return cbx_biloGdjeUprezimenu.Checked; }
      set {        cbx_biloGdjeUprezimenu.Checked = value; }
   }

   public string Fld_FilterStrSpr
   {
      get { return tbx_strSpr.Text; }
      set {        tbx_strSpr.Text = value; }
   }

   public string Fld_FilterStrSprCd
   {
      get { return tbx_strSprCd.Text; }
      set {        tbx_strSprCd.Text = value; }
   }

   public string Fld_FilterJmbg
   {
      get { return tbx_jmbg.Text; }
      set {        tbx_jmbg.Text = value; }
   }

   public string Fld_FilterOib
   {
      get { return tbx_oib.Text; }
      set {        tbx_oib.Text = value; }
   }

   public string Fld_FilterRegob
   {
      get { return tbx_regob.Text; }
      set {        tbx_regob.Text = value; }
   }

   public string Fld_FilterOsBrOsig
   {
      get { return tbx_osBrOsig.Text; }
      set {        tbx_osBrOsig.Text = value; }
   }

   public uint Fld_FilterBankaCd
   {
      get { return tbx_banka_cd.GetSomeRecIDField(); }
      set {        tbx_banka_cd.PutSomeRecIDField(value); }
 }

   public string Fld_FilterBankaCdAsTxt
   {
      get { return tbx_banka_cd.Text;         }
      set {        tbx_banka_cd.Text = value; }
   }

   public string Fld_FilterBankaTk
   {
      get { return tbx_banka_tk.Text; }
      set {        tbx_banka_tk.Text = value; }
   }

   public string Fld_FilterBankaNaziv
   {
      get { return tbx_banka_Naziv.Text; }
      set {        tbx_banka_Naziv.Text = value; }
   }

   public uint Fld_FilterMtrosCd
   {
      get { return tbx_mtros_cd.GetSomeRecIDField(); }
      set {        tbx_mtros_cd.PutSomeRecIDField(value); }
   }

   public string Fld_FilterMtrosCdAsTxt
   {
      get { return tbx_mtros_cd.Text;         }
      set {        tbx_mtros_cd.Text = value; }
   }

   public string Fld_FilterMtrosTk
   {
      get { return tbx_mtros_tk.Text; }
      set {        tbx_mtros_tk.Text = value; }
   }

   public string Fld_FilterMtrosNaziv
   {
      get { return tbx_mtros_Naziv.Text; }
      set {        tbx_mtros_Naziv.Text = value; }
   }

   public string Fld_FilterRadMj
   {
      get { return tbx_radMj.Text; }
      set {        tbx_radMj.Text = value; }
   }

   public string Fld_FilterTS
   {
      get { return tbx_ts.Text; }
      set {        tbx_ts.Text = value; }
   }

   public bool Fld_BiloGdjeURadMjesto
   {
      get { return cbx_biloGdjeURadMjesto.Checked; }
      set {        cbx_biloGdjeURadMjesto.Checked = value; }
   }

   public string Fld_FilterNapomena
   {
      get { return tbx_napomena.Text; }
      set {        tbx_napomena.Text = value; }
   }

   public bool Fld_BiloGdjeUNapomeni
   {
      get { return cbx_biloGdjeUnapomeni.Checked; }
      set {        cbx_biloGdjeUnapomeni.Checked = value; }
   }

   public bool Fld_FilterIsPlaca
   {
      get { return cbx_isPlaca.Checked; }
      set {        cbx_isPlaca.Checked = value; }
   }

   public bool Fld_FilterIsUgDj
   {
      get { return cbx_isUgDj.Checked; }
      set {        cbx_isUgDj.Checked = value; }
   }

   public bool Fld_FilterIsAutH
   {
      get { return cbx_isAutH.Checked; }
      set {        cbx_isAutH.Checked = value; }
   }

   public bool Fld_FilterIsIDDk4
   {
      get { return cbx_isIDDk4.Checked; }
      set {        cbx_isIDDk4.Checked = value; }
   }

   public bool Fld_FilterIsPoduz
   {
      get { return cbx_isPoduz.Checked; }
      set {        cbx_isPoduz.Checked = value; }
   }

   public bool Fld_FilterIsNadzO
   {
      get { return cbx_isNadzO.Checked; }
      set {        cbx_isNadzO.Checked = value; }
   }

   public Person.VrstaIsplateEnum? Fld_FilterVrstaIsplate
   {
      get
      {
              if(rbt_tekuci  .Checked)return Person.VrstaIsplateEnum.TEKUCI  ;
         else if(rbt_banka   .Checked)return Person.VrstaIsplateEnum.BANKA   ;
         else if(rbt_gotovina.Checked)return Person.VrstaIsplateEnum.GOTOVINA;
         else if(rbt_nebitno .Checked)return /*Person.VrstaIsplateEnum.NEBITNO*/ null;

              else throw new Exception("Fld_VrstaIsplate: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case Person.VrstaIsplateEnum.TEKUCI  : rbt_tekuci  .Checked = true; break;
            case Person.VrstaIsplateEnum.BANKA   : rbt_banka   .Checked = true; break;
            case Person.VrstaIsplateEnum.GOTOVINA: rbt_gotovina.Checked = true; break;
            //case Person.VrstaIsplateEnum.NEBITNO : rbt_nebitno .Checked = true; break;
            default                              : rbt_nebitno .Checked = true; break;
         }
      }
   }

   public ZXC.JeliJeTakav Fld_IsIzuzet
   {
      get
      {
         if     (rbt_izuzet.Checked)  return ZXC.JeliJeTakav.JE_TAKAV;
         else if(rbt_neIzuzet.Checked)return ZXC.JeliJeTakav.NIJE_TAKAV;
         else if(rbt_svi.Checked)     return ZXC.JeliJeTakav.NEBITNO;

         else throw new Exception("Fld_IsIzuzet: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.JeliJeTakav.JE_TAKAV  : rbt_izuzet.Checked   = true; break;
            case ZXC.JeliJeTakav.NIJE_TAKAV: rbt_neIzuzet.Checked = true; break;
            case ZXC.JeliJeTakav.NEBITNO   : rbt_svi.Checked      = true; break;
         }
      }

   }

   public bool Fld_OcuPtrans
   {
      get { return cbx_osnovno.Checked; }
      set {        cbx_osnovno.Checked = value; }
   }

   public bool Fld_OcuPtrane
   {
      get { return cbx_evr.Checked; }
      set {        cbx_evr.Checked = value; }
   }

   public bool Fld_OcuPtrano
   {
      get { return cbx_obu.Checked; }
      set {        cbx_obu.Checked = value; }
   }

   #endregion Fld_

   #region Overriders And Specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.person_rec; }
      set {        this.person_rec = (Person)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.PersonDao; }
   }

   private Vektor.DataLayer.DS_FindRecord.DS_findPerson ds_person;

   protected override DataSet VirtualUntypedDataSet { get { return ds_person; } }

   protected override object[] From_IndexSegmentValues
   {
      get
      {
         switch (recordSorter.SortType)
         {
            case VvSQL.SorterType.Person : return new object[] { Fld_FromPrezime , Fld_FromPersonCD, 0 };
            case VvSQL.SorterType.Code   : return new object[] { Fld_FromPersonCD,                   0 };

            default: ZXC.aim_emsg("Q42: SortType [{0}] undifajnd in property 'From_IndexSegmentValues'", recordSorter.SortType); return null;
         }
      }
   }

   #endregion Overriders and specifics

   #region AddFilterMemberz()

   /// <summary>
   /// get { return ZXC.PersonDao.TheSchemaTable.Rows; }
   /// </summary>
   private DataRowCollection PersonSchemaRows
   {
      get { return ZXC.PersonDao.TheSchemaTable.Rows; }
   }

   /// <summary>
   ///  get { return ZXC.PersonDao.CI; }
   /// </summary>
   private PersonDao.PersonCI PersonCI
   {
      get { return ZXC.PersonDao.CI; }
   }

   public override void AddFilterMemberz()
   {
      string  text, preffix;
      uint    num;
      bool    isCheck;
      DataRow drSchema;

      ZXC.JeliJeTakav jeLiTakav;

      this.TheFilterMembers.Clear();

      // Fld_FilterJmbg                                                                                                                                          

      drSchema = PersonSchemaRows[PersonCI.jmbg];
      text     = Fld_FilterJmbg;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Jmbg", text, " = "));
      }

      // Fld_FilterOib                                                                                                                                          

      drSchema = PersonSchemaRows[PersonCI.oib];
      text     = Fld_FilterOib;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Oib", text, " = "));
      }

      // Fld_FilterTS                                                                                                                                          

      drSchema = PersonSchemaRows[PersonCI.ts];
      text     = Fld_FilterTS;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "TS", text, " = "));
      }

      // Fld_FilterRegob                                                                                                                                          

      drSchema = PersonSchemaRows[PersonCI.regob];
      text     = Fld_FilterRegob;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Regob", text, " = "));
      }

      // Fld_FilterOsBrOsig                                                                                                                                          

      drSchema = PersonSchemaRows[PersonCI.osBrOsig];
      text     = Fld_FilterOsBrOsig;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "OsBrOsig", text, " = "));
      }

      //// Fld_FilterIsIzuzet                                                                                                                                          

      //drSchema = PersonSchemaRows[PersonCI.isIzuzet];
      //isCheck  = Fld_FilterIsIzuzet;

      //if(isCheck)
      //{
      //   this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "IsIzuzet", isCheck, " = "));
      //}

      // Fld_FilterMtrosCd                                                                                                                                          

      drSchema = PersonSchemaRows[PersonCI.mtros_cd];
      num      = Fld_FilterMtrosCd;

      if(num != 0)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Mtros", num, " = "));
      }

      // Fld_FilterBankaCd                                                                                                                                          

      drSchema = PersonSchemaRows[PersonCI.banka_cd];
      num      = Fld_FilterBankaCd;

      if(num != 0)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Banka", num, " = "));
      }

      // Fld_FilterStrSpr                                                                                                                                            

      drSchema = PersonSchemaRows[PersonCI.strSprCd];
      text     = Fld_FilterStrSprCd;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "StrSpr", text, " = "));
      }

      // Fld_FilterRadMj                                                                                                                                         

      drSchema = PersonSchemaRows[PersonCI.radMj];
      text     = Fld_FilterRadMj;
      isCheck  = Fld_BiloGdjeURadMjesto;
      preffix  = isCheck ? "%" : "";

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "RadMj", preffix + text + "%", " LIKE "));
      }

      // Fld_FilterNapomena                                                                                                                                         

      drSchema = PersonSchemaRows[PersonCI.napomena];
      text     = Fld_FilterNapomena;
      isCheck  = Fld_BiloGdjeUNapomeni;
      preffix  = isCheck ? "%" : "";

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Napomena", preffix + text + "%", " LIKE "));
      }

      // Fld_FilterIsAutH                                                                                                                                         

      drSchema = PersonSchemaRows[PersonCI.isAutH];
      isCheck = Fld_FilterIsAutH;

      if(isCheck)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "IsAutH", isCheck, " = "));
      }

      // Fld_FilterIsPlaca                                                                                                                                         

      drSchema = PersonSchemaRows[PersonCI.isPlaca];
      isCheck  = Fld_FilterIsPlaca;

      if(isCheck)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "IsPlaca", isCheck, " = "));
      }

      // Fld_FilterIsUgDj                                                                                                                                         

      drSchema = PersonSchemaRows[PersonCI.isUgDj];
      isCheck  = Fld_FilterIsUgDj;

      if(isCheck)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "IsUgDj", isCheck, " = "));
      }

      // Fld_FilterIsNadzO                                                                                                                                         

      drSchema = PersonSchemaRows[PersonCI.isNadzO];
      isCheck  = Fld_FilterIsNadzO;

      if(isCheck)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "IsNadzO", isCheck, " = "));
      }

      // Fld_FilterIsPoduz                                                                                                                                         

      drSchema = PersonSchemaRows[PersonCI.isPoduz];
      isCheck  = Fld_FilterIsPoduz;

      if(isCheck)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "IsPoduz", isCheck, " = "));
      }

      // Fld_FilterVrstaIsplate                                                                                                                                         

      drSchema = PersonSchemaRows[PersonCI.vrstaIsplate];
      Person.VrstaIsplateEnum? vrstaIsplate = Fld_FilterVrstaIsplate;

      if(vrstaIsplate != null)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "VrIspl", (ushort)vrstaIsplate, " = "));
      }

      // Fld_IsIzuzet                                                                                                                                         

      drSchema  = PersonSchemaRows[PersonCI.isIzuzet];
      jeLiTakav = Fld_IsIzuzet;

      if(jeLiTakav == ZXC.JeliJeTakav.JE_TAKAV)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, false, "JEizuzet",   true, null, "", " = ", ""));
      }
      else if(jeLiTakav == ZXC.JeliJeTakav.NIJE_TAKAV)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, false, "NIJEizuzet", false, null, "", " = ", ""));
      }

   }

   #endregion AddFilterMemberz()

}

