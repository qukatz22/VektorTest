using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Linq;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand = MySql.Data.MySqlClient.MySqlCommand;
using System.Collections.Generic;
using Vektor.Reports.PIZ;
#endif

public class PersonUC : VvSifrarRecordUC
{
   #region Fieldz

   private VvTextBox tbx_PersonCd, tbx_ime, tbx_prezime,
                     tbx_ulica, tbx_grad, tbx_postaBr, tbx_tel, tbx_gsm, tbx_email,
                     tbx_datePri, tbx_dateOdj,
                     tbx_jmbg, tbx_oib, tbx_regob, tbx_osBrOsig,
                     tbx_banka_cd, tbx_banka_tk, tbx_banka_Naziv, tbx_pnbM, tbx_pnbV,
                     tbx_mtros_cd, tbx_mtros_tk, tbx_mtros_Naziv, tbx_strSpr, tbx_strSprCd, tbx_ts,
                     tbx_radMj, tbx_napomena, tbx_radMjOpis, tbx_radMjBroj,
                     tbx_zupan, tbx_zupCd,
                     tbx_birthDate, tbx_prevStazDD, tbx_prevStazMM, tbx_prevStazYY,

                     tbx_dokNum, tbx_dokDate, tbx_zaMMYY,
                     tbx_ttNum, tbx_tt, tbx_ttOpis,
                     tbx_fondSati, tbx_RSmID,
                     tbx_opcina, tbx_opcCd, tbx_stPrirez, tbx_koefOsOd, tbx_mioII, tbx_spec, tbx_invalid, tbx_invalPosto,
                     tbx_satiRada, tbx_satiBol, tbx_EVRcount, tbx_OBUcount,
                     tbx_bruto, tbx_theBruto,
                     tbx_neto, tbx_netoNaRUke, tbx_izdatFirme,
                     tbx_isPlaca, tbx_isUgDj, tbx_isAutH, tbx_isPoduz, tbx_isNadzO, tbx_isIzuzet,
                     tbx_benefStaz,
                     tbx_koefOO_1, tbx_koefOO_2, tbx_koefOO_3, tbx_koefOO_4 , tbx_koefOO_5 , tbx_koefOO_6, 
                     tbx_koefOO_7, tbx_koefOO_8, tbx_koefOO_9, tbx_koefOO_10, tbx_koefOO_11, tbx_koefOO_12,
                     tbx_isSO, tbx_isOtherVr,
                     tbx_birthMjestoDrz, tbx_naravVrRad    , tbx_drzavljanstvo , tbx_dozvola       , tbx_prijava       , 
                     tbx_strucno       , tbx_zanimanje     , tbx_trajanjeUgOdr , tbx_cl61ugSuglas  , tbx_cl62ugSuglas  , 
                     tbx_probniRad     , tbx_pripravStaz   , tbx_rezIspita     , tbx_radIno        , tbx_gdjeRadIno    , 
                     tbx_ustupRadnika  , tbx_ustupMjesto   , tbx_drzavaPovezDrs, tbx_posaoBenefStaz, tbx_nacinBenef    , 
                     tbx_sposobnost    , tbx_mjestoRada    , tbx_mirovanjeRO   , tbx_razlogOdj     , tbx_banka2TK      , 
                     tbx_banka2        , tbx_banka2_Naziv  , tbx_tfs           , tbx_skrRV          ,
                     tbx_IBAN_TekuciRedovni, tbx_IBAN_TekuciZasticeni, tbx_IBAN_ZiroRacun, tbx_IBAN_Banke;

   private CheckBox cbx_isIzuzet, cbx_isPlaca, cbx_isUgDj, cbx_isAutH, cbx_isPoduz, cbx_isNadzO, cbx_isSO, cbx_isOtherVr;

   private VvDateTimePicker dtp_datePri, dtp_dateOdj,
                            dtp_birthDate;

   private VvHamper hamp_naziv, hamp_date, hamp_radMj, hamp_banka, hamp_adres, hamp_numbers, hamp_vrstaRada, hamp_napom,
                    hampRO_doc, hampRO_parametri, hamp_Spol,
                    hamp_vrstaRadVremena, hamp_vrstaRadOdns, hamp_VrtsaIsplate,
                    hampRO_sati, hampRO_bruto, hampRO_neto, hamp_koefOO, hamp_prosireno, hamp_IBAN;

   private RadioButton rbt_rvPuno, rbt_rvNepun, rbt_rvSkr,
                       rbt_roNeodr, rbt_roOdr, rbt_roPripVjez,
                       rbt_tekuci, rbt_banka, rbt_gotovina,
                       rbt_musko, rbt_zensko, rbt_nepoznato;

   private int nextX = 0, nextY = 0, razmakHamp = ZXC.Qun10;

   /*private*/ public Person person_rec;

   private PersonDao.PersonCI DB_ci
   {
      get { return ZXC.PerCI; }
   }

   Panel panel_RO;

   private int colDateWidth = ZXC.Q4un + ZXC.Qun4;
   private int colSif6Width = ZXC.Q3un + ZXC.Qun8;

   Placa placa_rec;

   #endregion Fieldz

   #region Constructor

   public PersonUC(Control parent, Person _person, VvForm.VvSubModul vvSubModul)
   {
      person_rec = _person;

      ThePrefferedRecordSorter = VirtualDataRecord.DefaultSorter;

      this.TheSubModul = vvSubModul;

      SuspendLayout();

      CreateTabPages(parent);
      CreateWriteHampers();
      CreateReadOnlyHampers();
      CreateKoefOoHamper();
      hamp_IBAN.Location = new Point(panel_RO.Left, panel_RO.Bottom);
      InitializeVvUserControl(parent);
      CreateDataGridView_InitializeTheGrid_ReadOnly_Columns();

      ResumeLayout();

      placa_rec = new Placa();
   }

   #endregion Constructor

   #region TabPages

   private void CreateTabPages(Control _parent)
   {
      TheTabControl.TabPages.Add(CreateVvInnerTabPages("Matični" , "", ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
      TheTabControl.TabPages.Add(CreateVvInnerTabPages("Matični2", "", ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));

      VvTabPage vvTabPage = (VvTabPage)(_parent.Parent);

      // ovaj if sluzi kad se kartica dodaje i Find-a da se ne pojave transevi !!!!
      if(vvTabPage.TabPageKind != ZXC.VvTabPageKindEnum.RECORD_TabPage_INTERACTIVE)
      {
         TheTabControl.TabPages.Add(CreateVvInnerTabPages(ptrans_TabPageName, "", ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage));
         TheTabControl.TabPages.Add(CreateVvInnerTabPages(ptrane_TabPageName, "", ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage));
         TheTabControl.TabPages.Add(CreateVvInnerTabPages(ptrano_TabPageName, "", ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage));
      }
   }

   #endregion TabPages

   #region  HAMPER_Write

   private void CreateWriteHampers()
   {
      InitializeNazivHamper(out hamp_naziv);

      nextX = 0;
      nextY = hamp_naziv.Bottom + razmakHamp;
      InitializeAdresaHamper(out hamp_adres);

      nextY = hamp_adres.Bottom;
      InitializeDatumiHamper(out hamp_date);

      nextY = hamp_date.Bottom;
      InitializeNumbersHamper(out hamp_numbers);

      nextY = hamp_numbers.Bottom;
      InitializeRadMjesHamper(out hamp_radMj);

      nextY = hamp_radMj.Bottom;
      InitializeIsplataHamper(out hamp_banka);

      nextX = hamp_naziv.Right + razmakHamp + ZXC.Qun2;
      nextY = 0;
      InitializeVrsRadaHamper(out hamp_vrstaRada);

      nextX = hamp_vrstaRada.Right + ZXC.QUN;
      InitializeHampeSpol(out hamp_Spol);

      nextY = hamp_Spol.Bottom;
      nextX = hamp_vrstaRada.Right + ZXC.QUN;
      InitializeHamperVrstIsplate(out hamp_VrtsaIsplate);

      nextX = hamp_vrstaRada.Left;
      nextY = hamp_vrstaRada.Bottom + ZXC.Qun2;// +razmakHamp;
      InitializeHamperRadVr(out hamp_vrstaRadVremena);

      nextY = hamp_VrtsaIsplate.Bottom + ZXC.Qun2;
      nextX = hamp_VrtsaIsplate.Left;
      InitializeHamperRadOdnos(out hamp_vrstaRadOdns);

      nextX = hamp_banka.Right + ZXC.Qun2;
      nextY = hamp_vrstaRadVremena.Bottom + ZXC.Qun2;
      InitializeNapomenaHamper(out hamp_napom);
      
      InitializeProsirenoHamper(out hamp_prosireno);
      InitializeIBANHamper(out hamp_IBAN);
   }

   private void InitializeNazivHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q7un + ZXC.Qun2, ZXC.Q2un - ZXC.Qun2, ZXC.Q2un + ZXC.Qun2, ZXC.Q2un, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun4,            ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4,           ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                    hamper.CreateVvLabel  (0, 0, "Prezime:", ContentAlignment.MiddleRight);
      tbx_prezime = hamper.CreateVvTextBox(1, 0, "tbx_prezime", "Prezime radnika", GetDB_ColumnSize(DB_ci.prezime), 1, 0);
      tbx_prezime.JAM_DataRequired = true;

                     hamper.CreateVvLabel  (3, 0, "Sifra:", ContentAlignment.MiddleRight);
      tbx_PersonCd = hamper.CreateVvTextBox(4, 0, "tbx_personCD", "Sifra radnika", 4 /*GetDB_ColumnSize(DB_ci.personCD)*/, 1, 0);
      tbx_PersonCd.JAM_StatusText = "Sifra Osobe";
      tbx_PersonCd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_PersonCd.JAM_FillCharacter = '0';
      tbx_PersonCd.JAM_DataRequired = true;

                hamper.CreateVvLabel  (0, 1, "Ime:", ContentAlignment.MiddleRight);
      tbx_ime = hamper.CreateVvTextBox(1, 1, "tbx_ime", "Ime radnika", GetDB_ColumnSize(DB_ci.ime));
      tbx_ime.JAM_DataRequired = true;

               hamper.CreateVvLabel        (2, 1, "Tip:", ContentAlignment.MiddleRight);
      tbx_ts = hamper.CreateVvTextBoxLookUp(3, 1, "tbx_ts", "Šifra tipa radnika", GetDB_ColumnSize(DB_ci.ts));
      tbx_ts.JAM_Set_LookUpTable(ZXC.luiListaPersonTS, (int)ZXC.Kolona.prva);
      tbx_ts.JAM_lookUp_NOTobligatory = true;
      tbx_ts.JAM_lookUp_MultiSelection = true;
      tbx_ts.JAM_CharacterCasing = CharacterCasing.Upper;

                     hamper.CreateVvLabel   (4, 1, "Izuzet:", ContentAlignment.MiddleRight);
      tbx_isIzuzet = hamper.CreateVvTextBox (5, 1, "tbx_isIzuzet", "");
      cbx_isIzuzet = hamper.CreateVvCheckBox(5, 1, "", tbx_isIzuzet, "", "X");
      tbx_isIzuzet.JAM_Highlighted = true;
      tbx_isIzuzet.JAM_ForeColor = Color.Red;

      this.ControlForInitialFocus = tbx_prezime;

      tbx_prezime.Font =
      tbx_PersonCd.Font = ZXC.vvFont.LargeBoldFont;

      tbx_prezime.JAM_CharacterCasing = tbx_ime.JAM_CharacterCasing = CharacterCasing.Upper;
   }

   private void InitializeAdresaHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 4, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q4un - ZXC.Qun2, ZXC.Q2un + ZXC.Qun2, ZXC.Q3un, ZXC.Q5un + ZXC.Qun2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt    = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow = new int[] { ZXC.Qun8, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                  hamper.CreateVvLabel(0, 0, "Adresa:", ContentAlignment.MiddleRight);
      tbx_ulica = hamper.CreateVvTextBox(1, 0, "tbx_ulica", "Ulica i kućni broj", GetDB_ColumnSize(DB_ci.ulica), 3, 0);

                 hamper.CreateVvLabel(0, 1, "Grad:", ContentAlignment.MiddleRight);
      //tbx_postaBr = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_postaBr", "Broj poštanskog ureda", GetDB_ColumnSize(DB_ci.postaBr));
      tbx_postaBr = hamper.CreateVvTextBox(1, 1, "tbx_postaBr", "Broj poštanskog ureda", GetDB_ColumnSize(DB_ci.postaBr));
      tbx_grad = hamper.CreateVvTextBox(2, 1, "tbx_grad", "Grad", GetDB_ColumnSize(DB_ci.grad), 2, 0);
      //tbx_grad.JAM_ReadOnly     = true;
      tbx_postaBr.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      // TODO:
      //tbx_postaBr.JAM_Set_LookUpTable(ZXC.luiListaGrad, (int)ZXC.Kolona.prva);
      //tbx_postaBr.JAM_LookUpNameTakerVvTb = tbx_grad.JAM_Name;

      // 09.06.2015. preselila na drugi tab
//lbl_zupan = hamper.CreateVvLabel(0, 2, "Zupanija:", ContentAlignment.MiddleRight);
//tbx_zupCd = hamper.CreateVvTextBoxLookUp(1, 2, "tbx_zupanCd", "Šifra županije", GetDB_ColumnSize(DB_ci.zupCd));
//tbx_zupan = hamper.CreateVvTextBox(2, 2, "tbx_zupan", "Županija", GetDB_ColumnSize(DB_ci.zupan), 2, 0);
//tbx_zupan.JAM_ReadOnly = true;
//tbx_zupCd.JAM_Set_LookUpTable(ZXC.luiListaZupanija, (int)ZXC.Kolona.prva);
//tbx_zupCd.JAM_lui_NameTaker_JAM_Name = tbx_zupan.JAM_Name;

                hamper.CreateVvLabel  (0, 2, "Telefon:", ContentAlignment.MiddleRight);
      tbx_tel = hamper.CreateVvTextBox(1, 2, "tbx_tel", "Broj telefona", GetDB_ColumnSize(DB_ci.tel), 1, 0);

                hamper.CreateVvLabel  (3, 2, "Mobitel:", ContentAlignment.MiddleRight);
      tbx_gsm = hamper.CreateVvTextBox(4, 2, "tbx_gsm", "Broj mobitela", GetDB_ColumnSize(DB_ci.gsm));

                  hamper.CreateVvLabel  (0, 3, "e-mail:", ContentAlignment.MiddleRight);
      tbx_email = hamper.CreateVvTextBox(1, 3, "tbx_email", "e-mail", GetDB_ColumnSize(DB_ci.email), 3, 0);
   }

   private void InitializeNumbersHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q6un, ZXC.Q3un - ZXC.Qun4, ZXC.Q6un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "JMBG:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 0, "OIB:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 1, "REGOB:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 1, "OsBrO:", ContentAlignment.MiddleRight);

      tbx_jmbg     = hamper.CreateVvTextBox(1, 0, "tbx_jmbg"    , "Jedinstveni matični broj građana", GetDB_ColumnSize(DB_ci.jmbg));
      tbx_oib      = hamper.CreateVvTextBox(3, 0, "tbx_oib"     , "Osobni identifikacijski broj"    , GetDB_ColumnSize(DB_ci.oib));
      tbx_regob    = hamper.CreateVvTextBox(1, 1, "tbx_regob"   , "Registarski broj obveznika"      , GetDB_ColumnSize(DB_ci.regob));
      tbx_osBrOsig = hamper.CreateVvTextBox(3, 1, "tbx_osBrOsig", "Osobni broj osiguranika"         , GetDB_ColumnSize(DB_ci.osBrOsig));

      tbx_jmbg    .JAM_CharEdits =
      tbx_oib     .JAM_CharEdits =
      tbx_regob   .JAM_CharEdits =
      tbx_osBrOsig.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_oib.JAM_FieldExitWithValidationMethod += new System.ComponentModel.CancelEventHandler(CheckOIB_Field);
   }

   private void InitializeRadMjesHamper(out VvHamper hamper)
   {
    //hamper = new VvHamper(4, 5, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      hamper = new VvHamper(5, 5, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

    //hamper.VvColWdt = new int[] { ZXC.Q6un, ZXC.Q3un + ZXC.Qun2, ZXC.Q3un + ZXC.Qun2, ZXC.Q8un - ZXC.Qun4 };
      hamper.VvColWdt = new int[] { ZXC.Q6un, ZXC.Q3un + ZXC.Qun2, ZXC.Q3un + ZXC.Qun2, ZXC.Q5un - ZXC.Qun4, ZXC.Q3un - ZXC.Qun4 };
      
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN ,  ZXC.QUN, ZXC.QUN ,  ZXC.QUN,  ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Stručna sprema:"      , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 0, "Zanimanje:"           , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 1, "Mjesto troška:"       , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 2, "RadMjesto/NazivPosla:", ContentAlignment.MiddleRight);

      tbx_strSprCd  = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_strSprCd" , "Šifra stručne spreme", GetDB_ColumnSize(DB_ci.strSprCd));
      tbx_strSpr    = hamper.CreateVvTextBox      (2, 0, "tbx_strSpr"   , "Stručna sprema"      , GetDB_ColumnSize(DB_ci.strSpr), 1, 0);tbx_strSpr.Visible = false;
      tbx_zanimanje = hamper.CreateVvTextBox      (3, 0, "tbx_zanimanje", "Zanimanje"           , GetDB_ColumnSize(DB_ci.zanimanje), 1, 0);
      
      tbx_strSprCd.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_strSpr.JAM_ReadOnly = true;
      tbx_strSprCd.JAM_Set_LookUpTable(ZXC.luiListaStrSprema, (int)ZXC.Kolona.prva);
      tbx_strSprCd.JAM_lui_NameTaker_JAM_Name = tbx_strSpr.JAM_Name;

      tbx_mtros_cd    = hamper.CreateVvTextBox(1, 1, "tbx_mtros_cd", "Šifra mjesta troška", GetDB_ColumnSize(DB_ci.mtros_cd));
      tbx_mtros_tk    = hamper.CreateVvTextBox(2, 1, "tbx_mtros_tk", "Tiker mjesta troška", GetDB_ColumnSize(DB_ci.mtros_tk));
      tbx_mtros_Naziv = hamper.CreateVvTextBox(3, 1, "tbx_mtros_naziv", "Naziv mjesta troška", 64, 1, 0);

      if(ZXC.IsZASTITARI)
      {
         tbx_radMj = hamper.CreateVvTextBoxLookUp(1, 2, "tbx_radMj", "Opis radnog mjesta", GetDB_ColumnSize(DB_ci.radMj));
         tbx_radMj.JAM_Set_LookUpTable(ZXC.luiListaRadnoMjesto, (int)ZXC.Kolona.prva);
         tbx_radMjOpis = hamper.CreateVvTextBox(2, 2, "tbx_radMjOpis", "Radno mjesto", 32, 1, 0);
         tbx_radMjBroj = hamper.CreateVvTextBox(4, 2, "tbx_radMjBroj", "Radno mjesto");
         tbx_radMj.JAM_lui_NameTaker_JAM_Name    = tbx_radMjOpis.JAM_Name;
         tbx_radMj.JAM_lui_IntegerTaker_JAM_Name = tbx_radMjBroj.JAM_Name;
         tbx_radMjOpis.JAM_ReadOnly = true;
         tbx_radMjBroj.JAM_ReadOnly = true;

      }
      else
      {
         tbx_radMj = hamper.CreateVvTextBox(1, 2, "tbx_radMj", "Opis radnog mjesta", GetDB_ColumnSize(DB_ci.radMj), 3, 0);
      }

                       hamper.CreateVvLabel  (0, 3, "Narav i vrsta rada:", ContentAlignment.MiddleRight);
      tbx_naravVrRad = hamper.CreateVvTextBox(1, 3, "tbx_naravVrRad", "Narav i vrsta rada", GetDB_ColumnSize(DB_ci.naravVrRad), 3, 0);
                       hamper.CreateVvLabel  (0, 4, "Mjesto rada:", ContentAlignment.MiddleRight);
      tbx_mjestoRada = hamper.CreateVvTextBox(1, 4, "tbx_mjestoRada", "Mjesto rada - stalno ili glavno mjesto rada", GetDB_ColumnSize(DB_ci.mjestoRada), 3, 0);


      tbx_mtros_cd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_mtros_tk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_mtros_cd.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_mtros_tk.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_mtros_Naziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyMtrosTextBoxLeave));
   }

   private void InitializeIsplataHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 3, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q2un - ZXC.Qun4, ZXC.QUN + ZXC.Qun2, ZXC.Q3un + ZXC.Qun2, ZXC.Q7un + ZXC.Qun2 + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Banka:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 1, "PozNaBr:", ContentAlignment.MiddleRight);

      tbx_banka_cd    = hamper.CreateVvTextBox(1, 0, "tbx_bankaCd", "Šifra banke", GetDB_ColumnSize(DB_ci.banka_cd), 1, 0);
      tbx_banka_tk    = hamper.CreateVvTextBox(3, 0, "tbx_bankaTk", "Tiker banke", GetDB_ColumnSize(DB_ci.banka_tk));
      tbx_banka_Naziv = hamper.CreateVvTextBox(4, 0, "tbx_bankaNaziv", "Naziv banke");
      tbx_pnbM        = hamper.CreateVvTextBox(1, 1, "tbx_pnbM", "Model poziva na broj", GetDB_ColumnSize(DB_ci.pnbM));
      tbx_pnbV        = hamper.CreateVvTextBox(2, 1, "tbx_pnbV", "Poziv na broj odobrenja", GetDB_ColumnSize(DB_ci.pnbV), 2, 0);

      tbx_pnbM.JAM_CharEdits =
      tbx_banka_cd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_banka_tk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_banka_cd   .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , ZXC.AutoCompleteRestrictor.KID_Banka_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyBankaTextBoxLeave));
      tbx_banka_tk   .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Banka_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyBankaTextBoxLeave));
      tbx_banka_Naziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , ZXC.AutoCompleteRestrictor.KID_Banka_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyBankaTextBoxLeave));

                         hamper.CreateVvLabel  (0, 2, "Zaštičeni račun:", ContentAlignment.MiddleRight);
      tbx_banka2       = hamper.CreateVvTextBox(1, 2, "tbx_banka2"    , "Zaštičeni račun sifra - SAMO kao informacija"     , GetDB_ColumnSize(DB_ci.banka2), 1, 0); 
      tbx_banka2TK     = hamper.CreateVvTextBox(3, 2, "tbx_banka2TK"  , "Zaštičeni račun tiker - SAMO kao informacija"     , GetDB_ColumnSize(DB_ci.banka2TK));
      tbx_banka2_Naziv = hamper.CreateVvTextBox(4, 2, "tbx_bankaNaziv", "Zaštičeni račun naziv banke - SAMO kao informacija");
   
      tbx_banka2.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_banka2TK.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_banka2      .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , /*ZXC.AutoCompleteRestrictor.KID_Banka_Only,*/ new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyBanka2TextBoxLeave));
      tbx_banka2TK    .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, /*ZXC.AutoCompleteRestrictor.KID_Banka_Only,*/ new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyBanka2TextBoxLeave));
      tbx_banka2_Naziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , /*ZXC.AutoCompleteRestrictor.KID_Banka_Only,*/ new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyBanka2TextBoxLeave));

   }

   private void InitializeDatumiHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(9, 3, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q4un - ZXC.Qun4, ZXC.Q3un - ZXC.Qun8, ZXC.QUN + ZXC.Qun2, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4,           ZXC.Qun8,          ZXC.Qun10,           ZXC.Qun8,          ZXC.Qun10,           ZXC.Qun8,          ZXC.Qun10 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Mjesto i drž. rođenja:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(4, 0, "Državljan:", 1, 0, ContentAlignment.MiddleRight);
      tbx_birthMjestoDrz = hamper.CreateVvTextBox(1, 0, "tbx_birthMjestoDrz", "birthMjestoDrz", GetDB_ColumnSize(DB_ci.birthMjestoDrz), 2, 0);
      tbx_drzavljanstvo  = hamper.CreateVvTextBox(6, 0, "tbx_drzavljanstvo ", "drzavljanstvo ", GetDB_ColumnSize(DB_ci.drzavljanstvo), 2, 0);

      hamper.CreateVvLabel(0, 1, "Datum Rođenja:"     , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 1, "PrethStaž:"         , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(3, 1, "God:"               , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(5, 1, "Mj:"                , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(7, 1, "Dan:"               , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 2, "Datum Prijave:"     , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 2, "Datum Odjave:", 2, 0, ContentAlignment.MiddleRight);

      tbx_birthDate = hamper.CreateVvTextBox(1, 1, "tbx_birthDate", "Datum rođenja", GetDB_ColumnSize(DB_ci.birthDate));
      tbx_birthDate.JAM_IsForDateTimePicker = true;
      dtp_birthDate = hamper.CreateVvDateTimePicker(1, 1, "", tbx_birthDate);
      dtp_birthDate.Name = "dtp_birthDate";


      tbx_datePri = hamper.CreateVvTextBox(1, 2, "tbx_datePri", "Datum prijave radnika", GetDB_ColumnSize(DB_ci.datePri));
      tbx_datePri.JAM_IsForDateTimePicker = true;
      dtp_datePri = hamper.CreateVvDateTimePicker(1, 2, "", tbx_datePri);
      dtp_datePri.Name = "dtp_DatePri";

      tbx_dateOdj = hamper.CreateVvTextBox(5, 2, "tbx_dateOdj", "Datum odjave radnika", GetDB_ColumnSize(DB_ci.dateOdj),3,0);
      tbx_dateOdj.JAM_IsForDateTimePicker = true;
      dtp_dateOdj = hamper.CreateVvDateTimePicker(5, 2, "", 3, 0, tbx_dateOdj);
      dtp_dateOdj.Name = "dtp_DateOdj";

      tbx_prevStazYY = hamper.CreateVvTextBox(4, 1, "tbx_prevStazYY", "Datum prijave radnika", GetDB_ColumnSize(DB_ci.prevStazYY));
      tbx_prevStazMM = hamper.CreateVvTextBox(6, 1, "tbx_prevStazMM", "Datum prijave radnika", GetDB_ColumnSize(DB_ci.prevStazMM));
      tbx_prevStazDD = hamper.CreateVvTextBox(8, 1, "tbx_prevStazDD", "Datum prijave radnika", GetDB_ColumnSize(DB_ci.prevStazDD));

      tbx_prevStazYY.JAM_CharEdits = ZXC.JAM_CharEdits.NumericOnly;
      tbx_prevStazMM.JAM_CharEdits = ZXC.JAM_CharEdits.NumericOnly;
      tbx_prevStazDD.JAM_CharEdits = ZXC.JAM_CharEdits.NumericOnly;

      tbx_prevStazMM.JAM_MarkAsNumericTextBox(0, false, 0M, 12M-1M, true);
      tbx_prevStazDD.JAM_MarkAsNumericTextBox(0, false, 0M, 31M-1M, true);
   }

   private void InitializeHampeSpol(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q2un-ZXC.Qun8,ZXC.Q2un,ZXC.Q2un-ZXC.Qun8,ZXC.Q2un-ZXC.Qun8 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun12,ZXC.Qun8,ZXC.Qun8,ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN-ZXC.Qun12;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Spol:", ContentAlignment.MiddleLeft);

      rbt_nepoznato = hamper.CreateVvRadioButton(1, 0, null, "Nep", TextImageRelation.ImageBeforeText);
      rbt_musko     = hamper.CreateVvRadioButton(2, 0, null, "M"  , TextImageRelation.ImageBeforeText);
      rbt_zensko    = hamper.CreateVvRadioButton(3, 0, null, "Ž"  , TextImageRelation.ImageBeforeText);

      rbt_nepoznato.Checked = true;
      rbt_nepoznato.Tag = true;
   }

   private void InitializeHamperVrstIsplate(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 4, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q6un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN - ZXC.Qun12;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                     hamper.CreateVvLabel(0, 0, "Vrsta isplate:", ContentAlignment.MiddleLeft);
      rbt_tekuci   = hamper.CreateVvRadioButton(0, 1, null, "Tekući", TextImageRelation.ImageBeforeText);
      rbt_banka    = hamper.CreateVvRadioButton(0, 2, null, "Banka", TextImageRelation.ImageBeforeText);
      rbt_gotovina = hamper.CreateVvRadioButton(0, 3, null, "Gotovina", TextImageRelation.ImageBeforeText);

      rbt_gotovina.Enabled = false;

      rbt_tekuci.Checked = true;
      rbt_tekuci.Tag = true;
   }

   private void InitializeHamperRadOdnos(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 4, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q7un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN - ZXC.Qun12;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel      (0, 0, "Vrsta radnog odnosa:", ContentAlignment.MiddleLeft);
      rbt_roNeodr    = hamper.CreateVvRadioButton(0, 1, null, "Neodređeno vrijeme", TextImageRelation.ImageBeforeText);
      rbt_roOdr      = hamper.CreateVvRadioButton(0, 2, null, "Određeno vrijeme", TextImageRelation.ImageBeforeText);
      rbt_roPripVjez = hamper.CreateVvRadioButton(0, 3, null, "Pripravnici/vježbenici", TextImageRelation.ImageBeforeText);
      rbt_roNeodr.Checked = true;
      rbt_roNeodr.Tag = true;


   }

   private void InitializeHamperRadVr(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 4, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q7un , ZXC.Q2un, ZXC.Q2un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8  };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                    hamper.CreateVvLabel      (0, 0,       "Vrsta radnog vremena:" , ContentAlignment.MiddleLeft);
      rbt_rvPuno  = hamper.CreateVvRadioButton(0, 1, null, "Puno radno vrijeme"    , TextImageRelation.ImageBeforeText);
      rbt_rvNepun = hamper.CreateVvRadioButton(0, 2, null, "Nepuno radno vrijeme"  , TextImageRelation.ImageBeforeText);
      rbt_rvSkr   = hamper.CreateVvRadioButton(0, 3, null, "Skraćeno radno vrijeme", TextImageRelation.ImageBeforeText);
      rbt_rvPuno.Checked = true;
      rbt_rvPuno.Tag = true;

                  hamper.CreateVvLabel  (1, 0, "Tjedno u satima", 1, 0, ContentAlignment.MiddleRight);
      tbx_tfs   = hamper.CreateVvTextBox(2, 1, "tbx_tfs"  , "Tjedno radno vrijeme u satima", GetDB_ColumnSize(DB_ci.tfs));
      tbx_skrRV = hamper.CreateVvTextBox(2, 3, "tbx_skrRV", "Skraćeno radno vrijeme u satima", GetDB_ColumnSize(DB_ci.skrRV));
      
      tbx_tfs  .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, 99.99M, true);
      tbx_skrRV.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, 99.99M, true);

   }

   private void InitializeVrsRadaHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 8, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.QUN - ZXC.Qun4, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                   hamper.CreateVvLabel(0, 0, "Vrsta rada:", 1, 0, ContentAlignment.MiddleLeft);

                    hamper.CreateVvLabel   (1, 1, "Plaća", ContentAlignment.MiddleLeft);
      tbx_isPlaca = hamper.CreateVvTextBox (0, 1, "tbx_isPlaca", "");
      cbx_isPlaca = hamper.CreateVvCheckBox(0, 1, "", tbx_isPlaca, "", "X");
      tbx_isPlaca.JAM_Highlighted = true;

                   hamper.CreateVvLabel   (1, 2, "Autorski honorar", ContentAlignment.MiddleLeft);
      tbx_isAutH = hamper.CreateVvTextBox (0, 2, "tbx_isAutH", "");
      cbx_isAutH = hamper.CreateVvCheckBox(0, 2, "", tbx_isAutH, "", "X");
      tbx_isAutH.JAM_Highlighted = true;

                   hamper.CreateVvLabel   (1, 3, "Ugovor o djelu", ContentAlignment.MiddleLeft);
      tbx_isUgDj = hamper.CreateVvTextBox (0, 3, "tbx_isUgDj", "");
      cbx_isUgDj = hamper.CreateVvCheckBox(0, 3, "", tbx_isUgDj, "", "X");
      tbx_isUgDj.JAM_Highlighted = true;

                    hamper.CreateVvLabel   (1, 4, "Poduzetnička plaća", ContentAlignment.MiddleLeft);
      tbx_isPoduz = hamper.CreateVvTextBox (0, 4, "tbx_isPoduz", "");
      cbx_isPoduz = hamper.CreateVvCheckBox(0, 4, "", tbx_isPoduz, "", "X");
      tbx_isPoduz.JAM_Highlighted = true;

                    hamper.CreateVvLabel   (1, 5, "Nadzorni odbor", ContentAlignment.MiddleLeft);
      tbx_isNadzO = hamper.CreateVvTextBox (0, 5, "tbx_isNadzO", "");
      cbx_isNadzO = hamper.CreateVvCheckBox(0, 5, "", tbx_isNadzO, "", "X");
      tbx_isNadzO.JAM_Highlighted = true;

      
                 hamper.CreateVvLabel   (1, 6, "Stručno osposob.", ContentAlignment.MiddleLeft);
      tbx_isSO = hamper.CreateVvTextBox (0, 6, "tbx_isSO", "");
      cbx_isSO = hamper.CreateVvCheckBox(0, 6, "", tbx_isSO, "", "X");
      tbx_isSO.JAM_Highlighted = true;

                      hamper.CreateVvLabel   (1, 7, "Ostalo", ContentAlignment.MiddleLeft);
      tbx_isOtherVr = hamper.CreateVvTextBox (0, 7, "tbx_isOtherVr", "");
      cbx_isOtherVr = hamper.CreateVvCheckBox(0, 7, "", tbx_isOtherVr, "", "X");
      tbx_isOtherVr.JAM_Highlighted = true;

   }

   private void InitializeNapomenaHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 7, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q10un + ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvSpcBefRow[1] = ZXC.Qun10;
      hamper.VvSpcBefRow[2] = ZXC.Qun2;

      hamper.VvBottomMargin = hamper.VvTopMargin;

                     hamper.CreateVvLabel  (0, 0, "Napomena / ostalo:", ContentAlignment.MiddleLeft);
      tbx_napomena = hamper.CreateVvTextBox(0, 1, "tbx_napom", "Napomena", GetDB_ColumnSize(DB_ci.napomena), 0, 5);

      tbx_napomena.Multiline = true;
      tbx_napomena.ScrollBars = ScrollBars.Vertical;
   }

   void AnyBankaTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_BankaCd = kupdob_rec.KupdobCD/*RecID*/;
            Fld_BankaTk = kupdob_rec.Ticker;
            Fld_BankaNaziv = kupdob_rec.Naziv;
         }
         else
         {
            Fld_BankaCdAsTxt = Fld_BankaTk = Fld_BankaNaziv = "";
         }
      }
   }

   void AnyBanka2TextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_Banka2      = kupdob_rec.KupdobCD/*RecID*/;
            Fld_Banka2TK    = kupdob_rec.Ticker;
            Fld_BankaNaziv2 = kupdob_rec.Naziv;
         }
         else
         {
            Fld_BankaCd2AsTxt = Fld_Banka2TK = Fld_BankaNaziv2 = "";
         }
      }
   }

   void AnyMtrosTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_MtrosCd = kupdob_rec.KupdobCD/*RecID*/;
            Fld_MtrosTk = kupdob_rec.Ticker;
            Fld_MtrosNaziv = kupdob_rec.Naziv;
         }
         else
         {
            Fld_MtrosCdAsTxt = Fld_MtrosTk = Fld_MtrosNaziv = "";
         }
      }
   }

   private void InitializeProsirenoHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 20, "", TheTabControl.TabPages[1], false, ZXC.QunMrgn, ZXC.QunMrgn, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q8un*2, ZXC.Q2un, ZXC.Q10un*2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4  , ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0,  0, "Dozvola za boravak i rad:"                                           , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0,  1, "Potvrda o prijavi rada:"                                             , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0,  2, "Stručno osposobljavanje, psebni ispiti :"                            , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0,  3, "Ug. i sug.(čl.61 st.3 ZOR) i broj radnih sati:"                      , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0,  4, "Ug. i sug.(čl.62 st.3 ZOR) i broj radnih sati:"                      , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0,  5, "Probni rad (ako je ugovoren):"                                       , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0,  6, "Pripravniči staž:"                                                   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0,  7, "Rezultat polaganja stručnog ispita:"                                 , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0,  8, "Trajanje rada u inozemstvu:"                                         , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0,  9, "Država i mjesto rada, kod upućivanja radnika u ino:"                 , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 10, "Trajanje privremenog ustupa radnika u povez. društvo:"               , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 11, "Sjedište i mjesto rada ustupljenog radnika:"                         , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 12, "Država poslovnog nastana povezanog društva:"                         , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 13, "Naznaka poslova na kojima se staž osig. računa s poveć. trajanjem:"  , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 14, "Način računanja staža:"                                              , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 15, "Poslovi na kojima radnik može raditi nakon utvrđivanja sposobnosti:" , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 16, "Vrijeme mirovanja radnog odnosa:"                                    , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 17, "Razlog prestanka radnog odnosa:"                                     , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 18, "Trajanje ugovora na određeno:"                                       , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 19, "Županija:"                                                           , ContentAlignment.MiddleRight);
                              

      tbx_dozvola         = hamper.CreateVvTextBox      (1,  0, "tbx_dozvola"       , "Dozvola za boravak i rad:"                                                                                 , GetDB_ColumnSize(DB_ci.dozvola       ), 1, 0); 
      tbx_prijava         = hamper.CreateVvTextBox      (1,  1, "tbx_prijava"       , "Potvrda o prijavi rada, ako je stranac i ako ih je obvezan imati:"                                         , GetDB_ColumnSize(DB_ci.prijava       ), 1, 0); 
      tbx_strucno         = hamper.CreateVvTextBox      (1,  2, "tbx_strucno"       , "Stručno osposobljavanje, posebni ispiti, tečajevi i sl.:"                                                  , GetDB_ColumnSize(DB_ci.strucno       ), 1, 0); 
      tbx_cl61ugSuglas    = hamper.CreateVvTextBox      (1,  3, "tbx_cl61ugSuglas"  , "Ugovor i suglasnost o radu s drugim poslodavcem (čl.61 st.3 ZOR) i broj radnih sati:"                      , GetDB_ColumnSize(DB_ci.cl61ugSuglas  ), 1, 0); 
      tbx_cl62ugSuglas    = hamper.CreateVvTextBox      (1,  4, "tbx_cl62ugSuglas"  , "Ugovor i suglasnost o radu s drugim poslodavcem (čl.62 st.3 ZOR) i broj radnih sati:"                      , GetDB_ColumnSize(DB_ci.cl62ugSuglas  ), 1, 0); 
      tbx_probniRad       = hamper.CreateVvTextBox      (1,  5, "tbx_probniRad"     , "Probni rad (ako je ugovoren):"                                                                             , GetDB_ColumnSize(DB_ci.probniRad     ), 1, 0); 
      tbx_pripravStaz     = hamper.CreateVvTextBox      (1,  6, "tbx_pripravStaz"   , "Pripravniči staž:"                                                                                         , GetDB_ColumnSize(DB_ci.pripravStaz   ), 1, 0); 
      tbx_rezIspita       = hamper.CreateVvTextBox      (1,  7, "tbx_rezIspita"     , "Rezultat polaganja stručnog ispita (ako je isti predvđen):"                                                , GetDB_ColumnSize(DB_ci.rezIspita     ), 1, 0); 
      tbx_radIno          = hamper.CreateVvTextBox      (1,  8, "tbx_radIno"        , "Trajanje rada u inozemstvu:"                                                                               , GetDB_ColumnSize(DB_ci.radIno        ), 1, 0); 
      tbx_gdjeRadIno      = hamper.CreateVvTextBox      (1,  9, "tbx_gdjeRadIno"    , "Država i mjesto rada, kod upućivanja radnika u inozemstvo:"                                                , GetDB_ColumnSize(DB_ci.gdjeRadIno    ), 1, 0); 
      tbx_ustupRadnika    = hamper.CreateVvTextBox      (1, 10, "tbx_ustupRadnika"  , "Trajanje privremenog ustupanja radnika u povezano društvo:"                                                , GetDB_ColumnSize(DB_ci.ustupRadnika  ), 1, 0); 
      tbx_ustupMjesto     = hamper.CreateVvTextBox      (1, 11, "tbx_ustupMjesto"   , "Sjedište i mjesto rada ustupljenog radnika:"                                                               , GetDB_ColumnSize(DB_ci.ustupMjesto   ), 1, 0); 
      tbx_drzavaPovezDrs  = hamper.CreateVvTextBox      (1, 12, "tbx_drzavaPovezDrs", "Država poslovnog nastana povezanog društva:"                                                               , GetDB_ColumnSize(DB_ci.drzavaPovezDrs), 1, 0); 
      tbx_posaoBenefStaz  = hamper.CreateVvTextBox      (1, 13, "tbx_posaoBenefStaz", "Naznaka poslova na kojima se staž osiguranja računa s povećanim trajanjem:"                                , GetDB_ColumnSize(DB_ci.posaoBenefStaz), 1, 0); 
      tbx_nacinBenef      = hamper.CreateVvTextBox      (1, 14, "tbx_nacinBenef"    , "Način računanja staža:"                                                                                    , GetDB_ColumnSize(DB_ci.nacinBenef    ), 1, 0); 
      tbx_sposobnost      = hamper.CreateVvTextBox      (1, 15, "tbx_sposobnost"    , "Je li riječ o poslovima na kojima radnik može raditi nakon prethodnog i redovitog utvrđivanja sposobnosti:", GetDB_ColumnSize(DB_ci.sposobnost    ), 1, 0); 
      tbx_mirovanjeRO     = hamper.CreateVvTextBox      (1, 16, "tbx_mirovanjeRO"   , "Vrijeme mirovanja radnog odnosa:"                                                                          , GetDB_ColumnSize(DB_ci.mirovanjeRO   ), 1, 0); 
      tbx_razlogOdj       = hamper.CreateVvTextBox      (1, 17, "tbx_razlogOdj"     , "Razlog prestanka radnog odnosa:"                                                                           , GetDB_ColumnSize(DB_ci.razlogOdj     ), 1, 0);
      tbx_trajanjeUgOdr   = hamper.CreateVvTextBox      (1, 18, "tbx_trajanjeUgOdr ", "Trajanje ugovora na određeno "                                                                             , GetDB_ColumnSize(DB_ci.trajanjeUgOdr ), 1, 0); 
      tbx_zupCd           = hamper.CreateVvTextBoxLookUp(1, 19, "tbx_zupanCd", "Šifra županije", GetDB_ColumnSize(DB_ci.zupCd));
      tbx_zupan           = hamper.CreateVvTextBox      (2, 19, "tbx_zupan", "Županija", GetDB_ColumnSize(DB_ci.zupan)/*, 2, 0*/);
      tbx_zupan.JAM_ReadOnly = true;
      tbx_zupCd.JAM_Set_LookUpTable(ZXC.luiListaZupanija, (int)ZXC.Kolona.prva);
      tbx_zupCd.JAM_lui_NameTaker_JAM_Name = tbx_zupan.JAM_Name;

   }

   private void InitializeIBANHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 4, "", TheTabControl.TabPages[0], false, hamp_napom.Right, ZXC.QunMrgn, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q8un, ZXC.Q10un  + ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4  };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0,  0, "IBAN Tekući Redovni:"  , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0,  1, "IBAN Tekući Zaštićeni:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0,  2, "IBAN Žiro Račun:"      , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0,  3, "IBAN Banke:"           , ContentAlignment.MiddleRight);

      tbx_IBAN_TekuciRedovni   = hamper.CreateVvTextBox(1,  0, "tbx_IBAN_TekuciRedovni"  , "IBAN Tekući Redovni"  /*, GetDB_ColumnSize(DB_ci.dozvola       )*/); 
      tbx_IBAN_TekuciZasticeni = hamper.CreateVvTextBox(1,  1, "tbx_IBAN_TekuciZasticeni", "IBAN Tekući Zaštićeni"/*, GetDB_ColumnSize(DB_ci.prijava       )*/); 
      tbx_IBAN_ZiroRacun       = hamper.CreateVvTextBox(1,  2, "tbx_IBAN_ZiroRacun"      , "IBAN Žiro Račun"      /*, GetDB_ColumnSize(DB_ci.strucno       )*/); 
      tbx_IBAN_Banke           = hamper.CreateVvTextBox(1,  3, "tbx_IBAN_Banke"          , "IBAN Banke"           /*, GetDB_ColumnSize(DB_ci.cl61ugSuglas  )*/); 

      tbx_IBAN_TekuciRedovni  .JAM_ResultBox =
      tbx_IBAN_TekuciZasticeni.JAM_ResultBox =
      tbx_IBAN_ZiroRacun      .JAM_ResultBox =
      tbx_IBAN_Banke          .JAM_ResultBox = true;

      hamper.Visible = false; // FUSE 
   }


   #endregion  HAMPER_Write

   #region HAMPER_ReadOnly

   private void CreateReadOnlyHampers()
   {
      panel_RO = new Panel();
      panel_RO.Parent = TheTabControl.TabPages[0];
      panel_RO.Location = new Point(hamp_napom.Right + ZXC.Qun2, 0);

      nextX = 0;
      nextY = 0;
      InitializeROhamperDoc(out hampRO_doc);

      nextY = hampRO_doc.Bottom - ZXC.Qun4;
      InitializeROhamper_prirez(out hampRO_parametri);

      nextY = hampRO_parametri.Bottom - ZXC.Qun4;
      InitializeROhamper_sati(out hampRO_sati);

      nextY = hampRO_sati.Bottom + ZXC.Qun2;
      InitializeROhamper_bruto(out hampRO_bruto);

      nextY = hampRO_bruto.Bottom + ZXC.Qun2 - ZXC.Qun4;
      InitializeROhamper_neto(out hampRO_neto);

      panel_RO.Size = new Size(hampRO_doc.Right - hampRO_doc.Left + ZXC.QUN + ZXC.Qun2, hampRO_neto.Bottom + ZXC.Qun2);
      panel_RO.BackColor = ZXC.vvColors.vvPanel4TBoxResultBox_BackColor;
   }

   private void InitializeROhamperDoc(out VvHamper hamper)
   {
      hamper = new VvHamper(8, 3, "", panel_RO, false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q5un - ZXC.Qun2, ZXC.Q2un + ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.QUN, ZXC.Q3un - ZXC.Qun2 - ZXC.Qun4, ZXC.Q2un, ZXC.QUN - ZXC.Qun4, ZXC.Q2un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt = new int[] { ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lb = hamper.CreateVvLabel(0, 0, "Sa dokumenta:", ContentAlignment.MiddleRight);
      tbx_dokNum = hamper.CreateVvTextBox(1, 0, "tbx_dokNum", "Broj dokumenta sa kojeg se prikazuju podaci", 6, 1, 0);
      tbx_dokDate = hamper.CreateVvTextBox(3, 0, "tbx_dokDate", "Datum dokumenta sa kojeg se prikazuju podaci", 12, 1, 0);
      Label lb1 = hamper.CreateVvLabel(5, 0, "Za mj:", ContentAlignment.MiddleRight);
      tbx_zaMMYY = hamper.CreateVvTextBox(6, 0, "tbx_zaMMYY", "Mjesec/godina isplate prikaznih podataka - place", 6, 1, 0);

      Label lb2 = hamper.CreateVvLabel(0, 1, "", ContentAlignment.MiddleRight);
      tbx_tt = hamper.CreateVvTextBox(1, 1, "tbx_tt", "Tip transakcije dokumenta sa kojeg se prikazuju podaci");
      tbx_ttNum = hamper.CreateVvTextBox(2, 1, "tbx_ttNum", "Broj dokumenta prema tipu transakcije sa kojeg se prikazuju podaci", 4, 1, 0);
      tbx_ttOpis = hamper.CreateVvTextBox(4, 1, "tbx_ttOpis", "Opis tipa transakcije", 20, 3, 0);

      Label lb3 = hamper.CreateVvLabel(5, 2, "Fond sati:", 1, 0, ContentAlignment.MiddleRight);
      tbx_fondSati = hamper.CreateVvTextBox(7, 2, "tbx_fondSati", "Fond sati za prikazan mjesec");

      Label lb4 = hamper.CreateVvLabel(0, 2, "RSm ID:", ContentAlignment.MiddleRight);
      tbx_RSmID = hamper.CreateVvTextBox(1, 2, "tbx_RSmID", "RSm identifikator");

      tbx_dokNum.JAM_ResultBox =
      tbx_dokDate.JAM_ResultBox =
      tbx_zaMMYY.JAM_ResultBox =
      tbx_tt.JAM_ResultBox =
      tbx_ttNum.JAM_ResultBox =
      tbx_ttOpis.JAM_ResultBox =
      tbx_fondSati.JAM_ResultBox =
      tbx_RSmID.JAM_ResultBox = true;

      tbx_zaMMYY.Font =
      tbx_RSmID.Font = ZXC.vvFont.BaseBoldFont;

      ObojiLabele(hamper);
   }

   private void ObojiLabele(VvHamper hamper)
   {
      foreach(Control ctrl in hamper.Controls)
      {
         if(ctrl is Label) ctrl.ForeColor = Color.Black;
      }
   }

   private void InitializeROhamper_prirez(out VvHamper hamper)
   {
      hamper = new VvHamper(7, 4, "", panel_RO, false, nextX, nextY, razmakHamp);

      hamper.VvColWdt = new int[] { ZXC.Q5un - ZXC.Qun2, ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.Q6un + ZXC.Qun2 + ZXC.Qun12, ZXC.QUN - ZXC.Qun4, ZXC.QUN };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt = new int[] { ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl_opcina, lbl_prirez, lbl_koefOO, lbl_mio, lbl_spec, lbl_invalid;

      lbl_opcina = hamper.CreateVvLabel(0, 0, "Općina:", ContentAlignment.MiddleRight);
      tbx_opcCd = hamper.CreateVvTextBox(1, 0, "tbx_opcinaCd", "Šifra općine", 4, 1, 0);
      tbx_opcina = hamper.CreateVvTextBox(3, 0, "tbx_opcina", "Nayiv općine", 6, 3, 0);

      lbl_prirez = hamper.CreateVvLabel(0, 1, "Prirez:", ContentAlignment.MiddleRight);
      tbx_stPrirez = hamper.CreateVvTextBox(1, 1, "tbx_stPrirez", "Stopa prireza", 6, 1, 0);
      tbx_stPrirez.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_stPrirez.JAM_DisableNegativeNumberValues = true;

      lbl_mio = hamper.CreateVvLabel(4, 1, "MIO II :", ContentAlignment.MiddleRight);
      tbx_mioII = hamper.CreateVvTextBox(5, 1, "tbx_mioII", "Pripadnost MIO II stup ", 2, 1, 0);

      lbl_koefOO = hamper.CreateVvLabel(0, 2, "KoefOsOdb:", ContentAlignment.MiddleRight);
      tbx_koefOsOd = hamper.CreateVvTextBox(1, 2, "tbx_koefOsOd", "Koeficijent osobnog odbitka", 6, 1, 0);
      tbx_koefOsOd.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_koefOsOd.JAM_DisableNegativeNumberValues = true;

      lbl_spec = hamper.CreateVvLabel(3, 2, "Novozaposleni/Umirovljenik:", 2, 0, ContentAlignment.MiddleRight);
      tbx_spec = hamper.CreateVvTextBox(6, 2, "tbx_spec", "N - Novozaposleni radnik, U - Umirovljenik");


      lbl_invalid = hamper.CreateVvLabel(0, 3, "Invalid:", ContentAlignment.MiddleRight);
      tbx_invalid = hamper.CreateVvTextBox(1, 3, "tbx_invalid", "I - Invalid, H - Hrvatski ratni vojni invalid");
      tbx_invalPosto = hamper.CreateVvTextBox(2, 3, "tbx_invalPosto", "", 6, 1, 0);
      tbx_invalPosto.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_invalPosto.JAM_DisableNegativeNumberValues = true;
      Label lblPosto = hamper.CreateVvLabel(4, 3, "%", ContentAlignment.MiddleLeft);

      tbx_opcCd.JAM_ResultBox =
      tbx_opcina.JAM_ResultBox =
      tbx_stPrirez.JAM_ResultBox =
      tbx_koefOsOd.JAM_ResultBox =
      tbx_mioII.JAM_ResultBox =
      tbx_spec.JAM_ResultBox =
      tbx_invalid.JAM_ResultBox =
      tbx_invalPosto.JAM_ResultBox = true;

      tbx_mioII.TextAlign =
      tbx_spec.TextAlign =
      tbx_invalid.TextAlign = HorizontalAlignment.Center;

      ObojiLabele(hamper);
   }

   private void InitializeROhamper_sati(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 3, "", panel_RO, false, nextX, nextY, razmakHamp);

      hamper.VvColWdt = new int[] { ZXC.Q5un - ZXC.Qun2, ZXC.Q2un, ZXC.Q8un + ZXC.Qun12, ZXC.Q2un };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt = new int[] { ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl_sR = hamper.CreateVvLabel(0, 0, "EVR:", ContentAlignment.MiddleRight);
      Label lbl_sB = hamper.CreateVvLabel(0, 1, "OBU:", ContentAlignment.MiddleRight);
      Label lbl_sBF = hamper.CreateVvLabel(2, 0, "Sati na teret poslodavca:", ContentAlignment.MiddleRight);
      Label lbl_sBH = hamper.CreateVvLabel(2, 1, "Sati na teret zakonodavca:", ContentAlignment.MiddleRight);
      Label lbl_bSt = hamper.CreateVvLabel(1, 2, "Beneficirani radni staž:", 1, 0, ContentAlignment.MiddleRight);

      tbx_EVRcount = hamper.CreateVvTextBox(1, 0, "tbx_EVRcount", "Broj redaka evidencije rada");
      tbx_OBUcount = hamper.CreateVvTextBox(1, 1, "tbx_OBUcount", "Broj redaka obustava");

      tbx_satiRada = hamper.CreateVvTextBox(3, 0, "tbx_satiRada", "Ukupno sati na teret poslodavca");
      tbx_satiBol = hamper.CreateVvTextBox(3, 1, "tbx_satiBol", "Ukupno sati na teret zakonodavca");

      tbx_benefStaz = hamper.CreateVvTextBox(3, 2, "tbx_benefStaz", "Beneficirarani radni staz - staz s povecanim trajanjem");

      tbx_satiRada.JAM_ResultBox =
      tbx_satiBol.JAM_ResultBox =
      tbx_EVRcount.JAM_ResultBox =
      tbx_OBUcount.JAM_ResultBox =
      tbx_benefStaz.JAM_ResultBox = true;

      ObojiLabele(hamper);
   }

   private void InitializeROhamper_bruto(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 2, "", panel_RO, false, nextX, nextY, razmakHamp);

      hamper.VvColWdt = new int[] { ZXC.Q5un - ZXC.Qun2, ZXC.Q5un };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt = new int[] { ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl_bruto, lbl_brutoDod;

      lbl_bruto = hamper.CreateVvLabel(0, 0, "Bruto1:", ContentAlignment.MiddleRight);
      lbl_brutoDod = hamper.CreateVvLabel(0, 1, "Konačni bruto:", ContentAlignment.MiddleRight);

      tbx_bruto = hamper.CreateVvTextBox(1, 0, "tbx_bruto", "Osnovna bruto plaća", 12);
      tbx_bruto.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_bruto.JAM_DisableNegativeNumberValues = true;
      tbx_theBruto = hamper.CreateVvTextBox(1, 1, "tbx_theBruto", "Konačni bruto kao osnovca za obračun doprinosa, poreza i neta", 12);
      tbx_theBruto.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_theBruto.JAM_DisableNegativeNumberValues = true;

      tbx_bruto.JAM_ResultBox =
      tbx_theBruto.JAM_ResultBox = true;

      ObojiLabele(hamper);
   }

   private void InitializeROhamper_neto(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 3, "", panel_RO, false, nextX, nextY, razmakHamp);

      hamper.VvColWdt = new int[] { ZXC.Q5un - ZXC.Qun2, ZXC.Q5un };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt = new int[] { ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.QUN - ZXC.Qun5 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl_neto, lbl_netoUk, lbl_izdatFirme;

      lbl_neto = hamper.CreateVvLabel(0, 0, "Neto plaća:", ContentAlignment.MiddleRight);
      lbl_netoUk = hamper.CreateVvLabel(0, 1, "NETO na ruke:", ContentAlignment.MiddleRight);

      lbl_izdatFirme = hamper.CreateVvLabel(0, 2, "BRUTO 2:", ContentAlignment.MiddleRight);

      tbx_neto = hamper.CreateVvTextBox(1, 0, "tbx_neto", "Neto plaća na osnovu ukupnog bruta", 12);
      tbx_neto.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_neto.JAM_DisableNegativeNumberValues = true;

      tbx_netoNaRUke = hamper.CreateVvTextBox(1, 1, "tbx_netoZaIspl", "Neto za isplatu", 12);
      tbx_netoNaRUke.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_netoNaRUke.JAM_DisableNegativeNumberValues = true;

      tbx_izdatFirme = hamper.CreateVvTextBox(1, 2, "tbx_izdatFirme", "Bruto2 - Ukupni Izdatak", 12);
      tbx_izdatFirme.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_izdatFirme.JAM_DisableNegativeNumberValues = true;

      tbx_neto.JAM_ResultBox =
      tbx_netoNaRUke.JAM_ResultBox =
      tbx_izdatFirme.JAM_ResultBox = true;

      ObojiLabele(hamper);
   }

   #endregion HAMPER_ReadOnly

   #region CreateKoefOoHamper

   private void CreateKoefOoHamper()
   {
      hamp_koefOO = new VvHamper(2, 13, "", TheTabControl.TabPages[0], false, panel_RO.Right, panel_RO.Top, razmakHamp);

      hamp_koefOO.VvColWdt    = new int[] { ZXC.Q4un, ZXC.Q2un};
      hamp_koefOO.VvSpcBefCol = new int[] { ZXC.Qun4, ZXC.Qun4};
      hamp_koefOO.VvRightMargin = hamp_koefOO.VvLeftMargin;

      for(int i = 0; i < hamp_koefOO.VvNumOfRows; i++)
      {
         hamp_koefOO.VvRowHgt[i] = ZXC.QUN;
         hamp_koefOO.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamp_koefOO.VvBottomMargin = hamp_koefOO.VvTopMargin;

      hamp_koefOO.CreateVvLabel(0,  0, "Korekcija KoefOO:", 1, 0, ContentAlignment.MiddleRight);
      hamp_koefOO.CreateVvLabel(0,  1, "KoefOO u 1 mj:" , ContentAlignment.MiddleRight);
      hamp_koefOO.CreateVvLabel(0,  2, "KoefOO u 2 mj:" , ContentAlignment.MiddleRight);
      hamp_koefOO.CreateVvLabel(0,  3, "KoefOO u 3 mj:" , ContentAlignment.MiddleRight);
      hamp_koefOO.CreateVvLabel(0,  4, "KoefOO u 4 mj:" , ContentAlignment.MiddleRight);
      hamp_koefOO.CreateVvLabel(0,  5, "KoefOO u 5 mj:" , ContentAlignment.MiddleRight);
      hamp_koefOO.CreateVvLabel(0,  6, "KoefOO u 6 mj:" , ContentAlignment.MiddleRight);
      hamp_koefOO.CreateVvLabel(0,  7, "KoefOO u 7 mj:" , ContentAlignment.MiddleRight);
      hamp_koefOO.CreateVvLabel(0,  8, "KoefOO u 8 mj:" , ContentAlignment.MiddleRight);
      hamp_koefOO.CreateVvLabel(0,  9, "KoefOO u 9 mj:" , ContentAlignment.MiddleRight);
      hamp_koefOO.CreateVvLabel(0, 10, "KoefOO u 10 mj:", ContentAlignment.MiddleRight);
      hamp_koefOO.CreateVvLabel(0, 11, "KoefOO u 11 mj:", ContentAlignment.MiddleRight);
      hamp_koefOO.CreateVvLabel(0, 12, "KoefOO u 12 mj:", ContentAlignment.MiddleRight);

      tbx_koefOO_1  = hamp_koefOO.CreateVvTextBox(1, 1 , "tbx_koefOO_1 ", "Koeficijent osobnog odbitka u 1 ", 5);
      tbx_koefOO_2  = hamp_koefOO.CreateVvTextBox(1, 2 , "tbx_koefOO_2 ", "Koeficijent osobnog odbitka u 2 ", 5);
      tbx_koefOO_3  = hamp_koefOO.CreateVvTextBox(1, 3 , "tbx_koefOO_3 ", "Koeficijent osobnog odbitka u 3 ", 5);
      tbx_koefOO_4  = hamp_koefOO.CreateVvTextBox(1, 4 , "tbx_koefOO_4 ", "Koeficijent osobnog odbitka u 4 ", 5);
      tbx_koefOO_5  = hamp_koefOO.CreateVvTextBox(1, 5 , "tbx_koefOO_5 ", "Koeficijent osobnog odbitka u 5 ", 5);
      tbx_koefOO_6  = hamp_koefOO.CreateVvTextBox(1, 6 , "tbx_koefOO_6 ", "Koeficijent osobnog odbitka u 6 ", 5);
      tbx_koefOO_7  = hamp_koefOO.CreateVvTextBox(1, 7 , "tbx_koefOO_7 ", "Koeficijent osobnog odbitka u 7 ", 5);
      tbx_koefOO_8  = hamp_koefOO.CreateVvTextBox(1, 8 , "tbx_koefOO_8 ", "Koeficijent osobnog odbitka u 8 ", 5);
      tbx_koefOO_9  = hamp_koefOO.CreateVvTextBox(1, 9 , "tbx_koefOO_9 ", "Koeficijent osobnog odbitka u 9 ", 5);
      tbx_koefOO_10 = hamp_koefOO.CreateVvTextBox(1, 10, "tbx_koefOO_10", "Koeficijent osobnog odbitka u 10", 5);
      tbx_koefOO_11 = hamp_koefOO.CreateVvTextBox(1, 11, "tbx_koefOO_11", "Koeficijent osobnog odbitka u 11", 5);
      tbx_koefOO_12 = hamp_koefOO.CreateVvTextBox(1, 12, "tbx_koefOO_12", "Koeficijent osobnog odbitka u 12", 5);

      tbx_koefOO_1 .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_koefOO_2 .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_koefOO_3 .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_koefOO_4 .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_koefOO_5 .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_koefOO_6 .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_koefOO_7 .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_koefOO_8 .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_koefOO_9 .JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_koefOO_10.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_koefOO_11.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbx_koefOO_12.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);

      tbx_koefOO_1 .JAM_DisableNegativeNumberValues = true;
      tbx_koefOO_2 .JAM_DisableNegativeNumberValues = true;
      tbx_koefOO_3 .JAM_DisableNegativeNumberValues = true;
      tbx_koefOO_4 .JAM_DisableNegativeNumberValues = true;
      tbx_koefOO_5 .JAM_DisableNegativeNumberValues = true;
      tbx_koefOO_6 .JAM_DisableNegativeNumberValues = true;
      tbx_koefOO_7 .JAM_DisableNegativeNumberValues = true;
      tbx_koefOO_8 .JAM_DisableNegativeNumberValues = true;
      tbx_koefOO_9 .JAM_DisableNegativeNumberValues = true;
      tbx_koefOO_10.JAM_DisableNegativeNumberValues = true;
      tbx_koefOO_11.JAM_DisableNegativeNumberValues = true;
      tbx_koefOO_12.JAM_DisableNegativeNumberValues = true;

   }

   #endregion CreateKoefOoHamper
   
   #region Ptrans DataGridView

   private void CreateDataGridView_InitializeTheGrid_ReadOnly_Columns()
   {
      aTransesGrid[0] = CreateDataGridView_ReadOnly(TheTabControl.TabPages[ptrans_TabPageName], "Ptrans");
      aTransesGrid[0].Dock = DockStyle.Fill;
      InitializeTheGrid_ReadOnly_Columns_Ptrans();

      aTransesGrid[0].DoubleClick += new EventHandler(theFIRST_TransGrid_DoubleClick);
      aTransesGrid[0].KeyPress += new KeyPressEventHandler(theFIRST_TransGrid_KeyPress);

      aTransesGrid[1] = CreateDataGridView_ReadOnly(TheTabControl.TabPages[ptrane_TabPageName], "Ptrane");
      aTransesGrid[1].Dock = DockStyle.Fill;
      InitializeTheGrid_ReadOnly_Columns_Ptrane();

      aTransesGrid[1].DoubleClick += new EventHandler(theFIRST_TransGrid_DoubleClick);
      aTransesGrid[1].KeyPress += new KeyPressEventHandler(theFIRST_TransGrid_KeyPress);

      aTransesGrid[2] = CreateDataGridView_ReadOnly(TheTabControl.TabPages[ptrano_TabPageName], "Ptrano");
      aTransesGrid[2].Dock = DockStyle.Fill;
      InitializeTheGrid_ReadOnly_Columns_Ptrano();

      aTransesGrid[2].DoubleClick += new EventHandler(theFIRST_TransGrid_DoubleClick);
      aTransesGrid[2].KeyPress += new KeyPressEventHandler(theFIRST_TransGrid_KeyPress);

   }

   protected override void theFIRST_TransGrid_DoubleClick(object sender, EventArgs e)
   {
      DataGridView dgv = sender as DataGridView;
           if(dgv == aTransesGrid[1]) base.OpenNew_Record_TabPage_OnDoubleClick(/*ZXC.VvSubModulEnum.PLA_2014*/ZXC.VvSubModulEnum.PLA_2024, SelectedRecIDIn_SECOND_TransGrid);
      else if(dgv == aTransesGrid[2]) base.OpenNew_Record_TabPage_OnDoubleClick(/*ZXC.VvSubModulEnum.PLA_2014*/ZXC.VvSubModulEnum.PLA_2024, SelectedRecIDIn_THIRD_TransGrid);
      else                            base.OpenNew_Record_TabPage_OnDoubleClick(/*ZXC.VvSubModulEnum.PLA_2014*/ZXC.VvSubModulEnum.PLA_2024, SelectedRecIDIn_FIRST_TransGrid);
   }

   private void InitializeTheGrid_ReadOnly_Columns_Ptrans()
   {
      AddDGVColum_RecID_4GridReadOnly   (aTransesGrid[0], "RecID"   , colSif6Width, false, 0);
      AddDGVColum_DateTime_4GridReadOnly(aTransesGrid[0], "Datum"   , colDateWidth);
      AddDGVColum_Integer_4GridReadOnly (aTransesGrid[0], "BrDok"   , colSif6Width, true, 6);
      AddDGVColum_Integer_4GridReadOnly (aTransesGrid[0], "Red"     , ZXC.Q2un + ZXC.Qun4, false, 0);
      AddDGVColum_String_4GridReadOnly  (aTransesGrid[0], "TT"      , ZXC.Q2un, false);
      AddDGVColum_String_4GridReadOnly  (aTransesGrid[0], "ZaMjGod" , colSif6Width, false);
      AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[0], "Bruto1"  , ZXC.Q5un, 2);
      AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[0], "TheBruto", ZXC.Q5un, 2);
      AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[0], "Neto"    , ZXC.Q5un, 2);
      AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[0], "NaRuke"  , ZXC.Q5un, 2);
      AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[0], "Bruto2"  , ZXC.Q5un, 2);
   }

   private void InitializeTheGrid_ReadOnly_Columns_Ptrane()
   {
      AddDGVColum_RecID_4GridReadOnly   (aTransesGrid[1], "RecID"    , colSif6Width, false, 0);
      AddDGVColum_DateTime_4GridReadOnly(aTransesGrid[1], "Datum"    , colDateWidth);
      AddDGVColum_Integer_4GridReadOnly (aTransesGrid[1], "BrDok"    , colSif6Width, true, 6);
      AddDGVColum_Integer_4GridReadOnly (aTransesGrid[1], "Red"      , ZXC.Q2un + ZXC.Qun4, false, 0);
      AddDGVColum_String_4GridReadOnly  (aTransesGrid[1], "TT"       , ZXC.Q2un, false);
      AddDGVColum_String_4GridReadOnly  (aTransesGrid[1], "VrstaRada", ZXC.Q10un + ZXC.Q5un, false);
      AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[1], "Postotak" , ZXC.Q3un, 0);
      AddDGVColum_String_4GridReadOnly  (aTransesGrid[1], "RsOO"     , ZXC.Q2un, false);
      AddDGVColum_Integer_4GridReadOnly (aTransesGrid[1], "RsOD"     , ZXC.Q2un, true, 2);
      AddDGVColum_Integer_4GridReadOnly (aTransesGrid[1], "RsDO"     , ZXC.Q2un, true, 2);
      AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[1], "UkSati"   , ZXC.Q3un, 1);

      aTransesGrid[1].Columns[6].DefaultCellStyle.Format = VvUserControl.GetDgvCellStyleFormat_Number(0, false, true);

   }

   private void InitializeTheGrid_ReadOnly_Columns_Ptrano()
   {
      AddDGVColum_RecID_4GridReadOnly   (aTransesGrid[2], "RecID"        , colSif6Width, false, 0);
      AddDGVColum_DateTime_4GridReadOnly(aTransesGrid[2], "Datum"        , colDateWidth);
      AddDGVColum_Integer_4GridReadOnly (aTransesGrid[2], "BrDok"        , colSif6Width, true, 6);
      AddDGVColum_Integer_4GridReadOnly (aTransesGrid[2], "Red"          , ZXC.Q2un + ZXC.Qun4, false, 0);
      AddDGVColum_String_4GridReadOnly  (aTransesGrid[2], "TT"           , ZXC.Q2un, false);
      AddDGVColum_DateTime_4GridReadOnly(aTransesGrid[2], "DatumStart"   , colDateWidth);
      AddDGVColum_Integer_4GridReadOnly (aTransesGrid[2], "BrRata"       , ZXC.Q2un + ZXC.Qun2, false, 0);
      AddDGVColum_String_4GridReadOnly  (aTransesGrid[2], "Opis obustave", ZXC.Q8un, false);
      AddDGVColum_Integer_4GridReadOnly (aTransesGrid[2], "Sifra K/D"    , colSif6Width, true, 6);
      AddDGVColum_String_4GridReadOnly  (aTransesGrid[2], "Tiker K/D"    , colSif6Width, false);
      AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[2], "IznosOb"      , ZXC.Q4un, 2);
   }

   #endregion DataGridView

   #region Filter

   public override void CreateRptFilterAndRptFilterUC()
   {
      //ThePlacaFilter = new VvRpt_Placa_Filter();
      ThePersonFilter = new PersonCardFilter();
      ThePersonFilterUC = new PlacaSifrartFilterUC(this);

      ThePersonFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = ThePersonFilterUC.Width;
   }

   #endregion Filter

   #region Fld_

   public uint Fld_PersonCd
   {
      get { return ZXC.ValOrZero_UInt(tbx_PersonCd.Text); }
      set { tbx_PersonCd.Text = value.ToString("0000"); }
   }

   public string Fld_Ime
   {
      get { return tbx_ime.Text; }
      set { tbx_ime.Text = value; }
   }

   public string Fld_Prezime
   {
      get { return tbx_prezime.Text; }
      set { tbx_prezime.Text = value; }
   }

   public string Fld_Ulica
   {
      get { return tbx_ulica.Text; }
      set { tbx_ulica.Text = value; }
   }

   public string Fld_Grad
   {
      get { return tbx_grad.Text; }
      set { tbx_grad.Text = value; }
   }

   public string Fld_PostaBr
   {
      get { return tbx_postaBr.Text; }
      set { tbx_postaBr.Text = value; }
   }

   public string Fld_Tel
   {
      get { return tbx_tel.Text; }
      set { tbx_tel.Text = value; }
   }

   public string Fld_Gsm
   {
      get { return tbx_gsm.Text; }
      set { tbx_gsm.Text = value; }
   }

   public string Fld_Email
   {
      get { return tbx_email.Text; }
      set { tbx_email.Text = value; }
   }

   public DateTime Fld_DatePri
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_datePri.Value);
      }
      set
      {
         dtp_datePri.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_datePri.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_datePri);
      }
   }

   public DateTime Fld_DateOdj
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_dateOdj.Value);
      }
      set
      {
         dtp_dateOdj.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_dateOdj.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_dateOdj);
      }
   }

   public string Fld_StrSpr
   {
      get { return tbx_strSpr.Text; }
      set { tbx_strSpr.Text = value; }
   }

   public string Fld_StrSprCd
   {
      get { return tbx_strSprCd.Text; }
      set { tbx_strSprCd.Text = value; }
   }

   public string Fld_TS
   {
      get { return tbx_ts.Text; }
      set { tbx_ts.Text = value; }
   }

   public string Fld_Jmbg
   {
      get { return tbx_jmbg.Text; }
      set { tbx_jmbg.Text = value; }
   }

   public string Fld_Oib
   {
      get { return tbx_oib.Text; }
      set { tbx_oib.Text = value; }
   }

   public string Fld_Regob
   {
      get { return tbx_regob.Text; }
      set { tbx_regob.Text = value; }
   }

   public string Fld_OsBrOsig
   {
      get { return tbx_osBrOsig.Text; }
      set { tbx_osBrOsig.Text = value; }
   }

   public uint Fld_BankaCd
   {
      get { return tbx_banka_cd.GetSomeRecIDField(); }
      set { tbx_banka_cd.PutSomeRecIDField(value); }
   }

   public string Fld_BankaCdAsTxt
   {
      get { return tbx_banka_cd.Text; }
      set { tbx_banka_cd.Text = value; }
   }

   public string Fld_BankaTk
   {
      get { return tbx_banka_tk.Text; }
      set { tbx_banka_tk.Text = value; }
   }

   public string Fld_BankaNaziv
   {
      get { return tbx_banka_Naziv.Text; }
      set { tbx_banka_Naziv.Text = value; }
   }

   public string Fld_PnbM
   {
      get { return tbx_pnbM.Text; }
      set { tbx_pnbM.Text = value; }
   }

   public string Fld_PnbV
   {
      get { return tbx_pnbV.Text; }
      set { tbx_pnbV.Text = value; }
   }

   public uint Fld_MtrosCd
   {
      get { return tbx_mtros_cd.GetSomeRecIDField(); }
      set { tbx_mtros_cd.PutSomeRecIDField(value); }
   }

   public string Fld_MtrosCdAsTxt
   {
      get { return tbx_mtros_cd.Text; }
      set { tbx_mtros_cd.Text = value; }
   }

   public string Fld_MtrosTk
   {
      get { return tbx_mtros_tk.Text; }
      set { tbx_mtros_tk.Text = value; }
   }

   public string Fld_MtrosNaziv
   {
      get { return tbx_mtros_Naziv.Text; }
      set { tbx_mtros_Naziv.Text = value; }
   }

   public string Fld_RadMj
   {
      get { return tbx_radMj.Text; }
      set { tbx_radMj.Text = value; }
   }

   public string Fld_Napomena
   {
      get { return tbx_napomena.Text; }
      set { tbx_napomena.Text = value; }
   }

   public bool Fld_IsIzuzet
   {
      get { return cbx_isIzuzet.Checked; }
      set { cbx_isIzuzet.Checked = value; }
   }

   public string Fld_IsIzuzetX
   {
      set { tbx_isIzuzet.Text = value; }
   }

   public bool Fld_IsPlaca
   {
      get { return cbx_isPlaca.Checked; }
      set { cbx_isPlaca.Checked = value; }
   }

   public string Fld_IsPlacaX
   {
      set { tbx_isPlaca.Text = value; }
   }

   public bool Fld_IsUgDj
   {
      get { return cbx_isUgDj.Checked; }
      set { cbx_isUgDj.Checked = value; }
   }

   public string Fld_IsUgDjX
   {
      set { tbx_isUgDj.Text = value; }
   }

   public bool Fld_IsAutH
   {
      get { return cbx_isAutH.Checked; }
      set { cbx_isAutH.Checked = value; }
   }

   public string Fld_IsAutHX
   {
      set { tbx_isAutH.Text = value; }
   }

   public bool Fld_IsPoduz   {  get { return cbx_isPoduz.Checked; }  set { cbx_isPoduz.Checked = value; }  }

   public string Fld_IsPoduzX
   {
      set { tbx_isPoduz.Text = value; }
   }

   public bool Fld_IsNadzO
   {
      get { return cbx_isNadzO.Checked; }
      set { cbx_isNadzO.Checked = value; }
   }

   public string Fld_IsNadzOX
   {
      set { tbx_isNadzO.Text = value; }
   }

   public string Fld_Zupanija
   {
      get { return tbx_zupan.Text; }
      set { tbx_zupan.Text = value; }
   }

   public string Fld_ZupanCd
   {
      get { return tbx_zupCd.Text; }
      set { tbx_zupCd.Text = value; }
   }

   public DateTime Fld_BirthDate
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_birthDate.Value);
      }
      set
      {
         dtp_birthDate.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_birthDate.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_birthDate);
      }
   }

   //RadioButton choice = grpBox1.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Checked);
   //UserChoiceEnum userChoice = (UserChoiceEnum)choice.Tag;
   public ZXC.Spol Fld_Spol
   {
      get
      {

         // qweqweNEW: RadioButton checkedRadioButton = hamp_Spol.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Checked);
         // qweqweNEW: return checkedRadioButton != null ? (ZXC.Spol)checkedRadioButton.Tag : ZXC.Spol.NEPOZNATO;
         
         if(rbt_nepoznato.Checked) return ZXC.Spol.NEPOZNATO;
         else if(rbt_musko.Checked) return ZXC.Spol.MUSKO;
         else if(rbt_zensko.Checked) return ZXC.Spol.ZENSKO;

         // 03.03.2020: ciau nona Francesca 
       //else throw new Exception("Fld_Spol: who df is checked?");
         else return ZXC.Spol.NEPOZNATO;
      }
      set
      {
         // qweqweNEW: RadioButton radioButtonHavingThisEnumValueAsTag = hamp_Spol.Controls.OfType<RadioButton>().FirstOrDefault(x => (ZXC.Spol)x.Tag == value);
         // qweqweNEW: if(radioButtonHavingThisEnumValueAsTag != null) radioButtonHavingThisEnumValueAsTag.Checked = true;
         rbt_nepoznato.Checked =
         rbt_musko    .Checked =
         rbt_zensko   .Checked = false;

         switch(value)
         {
            case ZXC.Spol.NEPOZNATO: rbt_nepoznato.Checked = true; break;
            case ZXC.Spol.MUSKO: rbt_musko.Checked = true; break;
            case ZXC.Spol.ZENSKO: rbt_zensko.Checked = true; break;
         }
      }
   }

   public Person.VrstaRadnogVremenaEnum Fld_VrstaRadVrem
   {
      get
      {
         if(rbt_rvPuno.Checked) return Person.VrstaRadnogVremenaEnum.PUNO;
         else if(rbt_rvNepun.Checked) return Person.VrstaRadnogVremenaEnum.NEPUNO;
         else if(rbt_rvSkr.Checked) return Person.VrstaRadnogVremenaEnum.SKRACENO;

         else throw new Exception("Fld_VrstaRadVrem: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case Person.VrstaRadnogVremenaEnum.PUNO: rbt_rvPuno.Checked = true; break;
            case Person.VrstaRadnogVremenaEnum.NEPUNO: rbt_rvNepun.Checked = true; break;
            case Person.VrstaRadnogVremenaEnum.SKRACENO: rbt_rvSkr.Checked = true; break;
         }
      }
   }

   public Person.VrstaRadnogOdnosaEnum Fld_VrstaRadOdns
   {
      get
      {
         if(rbt_roNeodr.Checked) return Person.VrstaRadnogOdnosaEnum.NEODREDJENO;
         else if(rbt_roOdr.Checked) return Person.VrstaRadnogOdnosaEnum.ODREDJENO;
         else if(rbt_roPripVjez.Checked) return Person.VrstaRadnogOdnosaEnum.PRIPR_VJEZB;

         else throw new Exception("Fld_VrstaRadOdns: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case Person.VrstaRadnogOdnosaEnum.NEODREDJENO: rbt_roNeodr.Checked = true; break;
            case Person.VrstaRadnogOdnosaEnum.ODREDJENO: rbt_roOdr.Checked = true; break;
            case Person.VrstaRadnogOdnosaEnum.PRIPR_VJEZB: rbt_roPripVjez.Checked = true; break;
         }
      }
   }

   public Person.VrstaIsplateEnum Fld_VrstaIsplate
   {
      get
      {
         if(rbt_tekuci.Checked) return Person.VrstaIsplateEnum.TEKUCI;
         else if(rbt_banka.Checked) return Person.VrstaIsplateEnum.BANKA;
         else if(rbt_gotovina.Checked) return Person.VrstaIsplateEnum.GOTOVINA;

         else throw new Exception("Fld_VrstaIsplate: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case Person.VrstaIsplateEnum.TEKUCI: rbt_tekuci.Checked = true; break;
            case Person.VrstaIsplateEnum.BANKA: rbt_banka.Checked = true; break;
            case Person.VrstaIsplateEnum.GOTOVINA: rbt_gotovina.Checked = true; break;
         }
      }
   }

   public uint Fld_PrevStazYY { get { return tbx_prevStazYY.GetUintField(); } set { tbx_prevStazYY.PutUintField(value); } }
   public uint Fld_PrevStazMM { get { return tbx_prevStazMM.GetUintField(); } set { tbx_prevStazMM.PutUintField(value); } }
   public uint Fld_PrevStazDD { get { return tbx_prevStazDD.GetUintField(); } set { tbx_prevStazDD.PutUintField(value); } }
   
   public string  Fld_BankaCd2AsTxt  { get { return tbx_banka2        .Text;       } set { tbx_banka2        .Text = value;    } }
   public string  Fld_BankaNaziv2    { get { return tbx_banka2_Naziv  .Text;       } set { tbx_banka2_Naziv  .Text = value;    } }
   public uint    Fld_Banka2         { get { return tbx_banka2.GetSomeRecIDField();} set { tbx_banka2.PutSomeRecIDField(value);} }
   public string  Fld_Banka2TK       { get { return tbx_banka2TK      .Text;       } set { tbx_banka2TK      .Text = value;    } }
   public string  Fld_BirthMjestoDrz { get { return tbx_birthMjestoDrz.Text;       } set { tbx_birthMjestoDrz.Text = value;    } }
   public string  Fld_NaravVrRad     { get { return tbx_naravVrRad    .Text;       } set { tbx_naravVrRad    .Text = value;    } }
   public string  Fld_Drzavljanstvo  { get { return tbx_drzavljanstvo .Text;       } set { tbx_drzavljanstvo .Text = value;    } }
   public string  Fld_Dozvola        { get { return tbx_dozvola       .Text;       } set { tbx_dozvola       .Text = value;    } }
   public string  Fld_Prijava        { get { return tbx_prijava       .Text;       } set { tbx_prijava       .Text = value;    } }
   public string  Fld_Strucno        { get { return tbx_strucno       .Text;       } set { tbx_strucno       .Text = value;    } }
   public string  Fld_Zanimanje      { get { return tbx_zanimanje     .Text;       } set { tbx_zanimanje     .Text = value;    } }
   public string  Fld_TrajanjeUgOdr  { get { return tbx_trajanjeUgOdr .Text;       } set { tbx_trajanjeUgOdr .Text = value;    } }
   public string  Fld_Cl61ugSuglas   { get { return tbx_cl61ugSuglas  .Text;       } set { tbx_cl61ugSuglas  .Text = value;    } }
   public string  Fld_Cl62ugSuglas   { get { return tbx_cl62ugSuglas  .Text;       } set { tbx_cl62ugSuglas  .Text = value;    } }
   public string  Fld_ProbniRad      { get { return tbx_probniRad     .Text;       } set { tbx_probniRad     .Text = value;    } }
   public string  Fld_PripravStaz    { get { return tbx_pripravStaz   .Text;       } set { tbx_pripravStaz   .Text = value;    } }
   public string  Fld_RezIspita      { get { return tbx_rezIspita     .Text;       } set { tbx_rezIspita     .Text = value;    } }
   public string  Fld_RadIno         { get { return tbx_radIno        .Text;       } set { tbx_radIno        .Text = value;    } }
   public string  Fld_GdjeRadIno     { get { return tbx_gdjeRadIno    .Text;       } set { tbx_gdjeRadIno    .Text = value;    } }
   public string  Fld_UstupRadnika   { get { return tbx_ustupRadnika  .Text;       } set { tbx_ustupRadnika  .Text = value;    } }
   public string  Fld_UstupMjesto    { get { return tbx_ustupMjesto   .Text;       } set { tbx_ustupMjesto   .Text = value;    } }
   public string  Fld_DrzavaPovezDrs { get { return tbx_drzavaPovezDrs.Text;       } set { tbx_drzavaPovezDrs.Text = value;    } }
   public string  Fld_PosaoBenefStaz { get { return tbx_posaoBenefStaz.Text;       } set { tbx_posaoBenefStaz.Text = value;    } }
   public string  Fld_NacinBenef     { get { return tbx_nacinBenef    .Text;       } set { tbx_nacinBenef    .Text = value;    } }
   public string  Fld_Sposobnost     { get { return tbx_sposobnost    .Text;       } set { tbx_sposobnost    .Text = value;    } }
   public string  Fld_MjestoRada     { get { return tbx_mjestoRada    .Text;       } set { tbx_mjestoRada    .Text = value;    } }
   public string  Fld_MirovanjeRO    { get { return tbx_mirovanjeRO   .Text;       } set { tbx_mirovanjeRO   .Text = value;    } }
   public string  Fld_RazlogOdj      { get { return tbx_razlogOdj     .Text;       } set { tbx_razlogOdj     .Text = value;    } }
   public decimal Fld_Tfs            { get { return tbx_tfs     .GetDecimalField();} set { tbx_tfs  .PutDecimalField(value);   } }
   public decimal Fld_SkrRV          { get { return tbx_skrRV   .GetDecimalField();} set { tbx_skrRV.PutDecimalField(value);   } }
   public bool    Fld_IsSO           { get { return cbx_isSO     .Checked;         } set { cbx_isSO         .Checked = value;  } }
   public bool    Fld_IsOtherVr      { get { return cbx_isOtherVr.Checked;         } set { cbx_isOtherVr    .Checked = value;  } }


   // Result Fieldz _____________________________________________________________________________________________________

   public uint Fld_DokNum
   {
      set { tbx_dokNum.Text = value.ToString("000000"); }
   }

   public string Fld_DokDate
   {
      set
      {
         tbx_dokDate.Text = value;
      }
   }

   public string Fld_MMYYYY
   {
      set { tbx_zaMMYY.Text = value; }
   }

   public uint Fld_TtNum
   {
      set { tbx_ttNum.Text = value.ToString("0000"); }
   }

   public string Fld_Tt
   {
      set { tbx_tt.Text = value; }
   }

   public string Fld_TtOpis
   {
      set { tbx_ttOpis.Text = value; }
   }

   public decimal Fld_FondSati
   {
      set { tbx_fondSati.PutDecimalField(value); }
   }

   public string Fld_RSm_ID
   {
      set { tbx_RSmID.Text = value; }
   }

   public string Fld_Opcina
   {
      set { tbx_opcina.Text = value; }
   }

   public string Fld_OpcCd
   {
      set { tbx_opcCd.Text = value; }
   }

   public decimal Fld_StPrirez
   {
      set { tbx_stPrirez.PutDecimalField(value); }
   }

   public decimal Fld_KoefOsOd
   {
      set { tbx_koefOsOd.PutDecimalField(value); }
   }

   public string Fld_MioII
   {
      set { tbx_mioII.Text = value; }
   }

   public string Fld_Spc
   {
      set { tbx_spec.Text = value; }
   }

   public string Fld_InvalidTip
   {
      set { tbx_invalid.Text = value; }
   }

   public decimal Fld_KoefHRVI
   {
      set { tbx_invalPosto.PutDecimalField(value); }
   }

   public decimal Fld_SatiRada
   {
      set { tbx_satiRada.Text = value.ToString(""); }
   }

   public decimal Fld_SatiBol
   {
      set { tbx_satiBol.Text = value.ToString(""); }
   }

   public int Fld_EVRcount
   {
      set { tbx_EVRcount.Text = value.ToString(""); }
   }

   public int Fld_OBUcount
   {
      set { tbx_OBUcount.Text = value.ToString(""); }
   }

   public string Fld_BenefStaz
   {
      set { tbx_benefStaz.Text = value; }
   }

   public decimal Fld_BrutoOsn
   {
      set { tbx_bruto.PutDecimalField(value); }
   }

   public decimal Fld_TheBruto
   {
      set { tbx_theBruto.PutDecimalField(value); }
   }

   public decimal Fld_Netto
   {
      set { tbx_neto.PutDecimalField(value); }
   }

   public decimal Fld_NaRuke
   {
      set { tbx_netoNaRUke.PutDecimalField(value); }
   }

   public decimal Fld_2Pay
   {
      set { tbx_izdatFirme.PutDecimalField(value); }
   }


   public decimal Fld_KorKoef01   { get { return tbx_koefOO_1 .GetDecimalField(); }  set { tbx_koefOO_1 .PutDecimalField(value); }  }
   public decimal Fld_KorKoef02   { get { return tbx_koefOO_2 .GetDecimalField(); }  set { tbx_koefOO_2 .PutDecimalField(value); }  }
   public decimal Fld_KorKoef03   { get { return tbx_koefOO_3 .GetDecimalField(); }  set { tbx_koefOO_3 .PutDecimalField(value); }  }
   public decimal Fld_KorKoef04   { get { return tbx_koefOO_4 .GetDecimalField(); }  set { tbx_koefOO_4 .PutDecimalField(value); }  }
   public decimal Fld_KorKoef05   { get { return tbx_koefOO_5 .GetDecimalField(); }  set { tbx_koefOO_5 .PutDecimalField(value); }  }
   public decimal Fld_KorKoef06   { get { return tbx_koefOO_6 .GetDecimalField(); }  set { tbx_koefOO_6 .PutDecimalField(value); }  }
   public decimal Fld_KorKoef07   { get { return tbx_koefOO_7 .GetDecimalField(); }  set { tbx_koefOO_7 .PutDecimalField(value); }  }
   public decimal Fld_KorKoef08   { get { return tbx_koefOO_8 .GetDecimalField(); }  set { tbx_koefOO_8 .PutDecimalField(value); }  }
   public decimal Fld_KorKoef09   { get { return tbx_koefOO_9 .GetDecimalField(); }  set { tbx_koefOO_9 .PutDecimalField(value); }  }
   public decimal Fld_KorKoef10   { get { return tbx_koefOO_10.GetDecimalField(); }  set { tbx_koefOO_10.PutDecimalField(value); }  }
   public decimal Fld_KorKoef11   { get { return tbx_koefOO_11.GetDecimalField(); }  set { tbx_koefOO_11.PutDecimalField(value); }  }
   public decimal Fld_KorKoef12   { get { return tbx_koefOO_12.GetDecimalField(); }  set { tbx_koefOO_12.PutDecimalField(value); }  }

   public string  Fld_IBAN_TekuciRedovni   { get { return tbx_IBAN_TekuciRedovni  .Text;} set { tbx_IBAN_TekuciRedovni  .Text = value;} }
   public string  Fld_IBAN_TekuciZasticeni { get { return tbx_IBAN_TekuciZasticeni.Text;} set { tbx_IBAN_TekuciZasticeni.Text = value;} }
   public string  Fld_IBAN_ZiroRacun       { get { return tbx_IBAN_ZiroRacun      .Text;} set { tbx_IBAN_ZiroRacun      .Text = value;} }
   public string  Fld_IBAN_Banke           { get { return tbx_IBAN_Banke          .Text;} set { tbx_IBAN_Banke          .Text = value;} }

   public string Fld_RadnoMjesto_Opis  { set { tbx_radMjOpis.Text = value; } }
   public int    Fld_RadnoMjesto_Broj  { set { tbx_radMjBroj.Text = value.ToString(""); } }


   #endregion Fld_

   #region PutFields(), GetFields()

   public override void PutFields(VvDataRecord person)
   {
      person_rec = (Person)person;

      Kupdob kupdobSifrar_rec;

      if(person_rec != null)
      {
         PutMetaFileds(person_rec.AddUID, person_rec.AddTS, person_rec.ModUID, person_rec.ModTS, person_rec.RecID, person_rec.LanSrvID, person_rec.LanRecID);

         PutIdentityFields(person_rec.PersonCD.ToString("0000"), person_rec.Prezime, person_rec.Ime, "");

         VvHamper.SetChkBoxRadBttnAutoCheck(this, true);

         Fld_PersonCd     = person_rec.PersonCD;
         Fld_Ime          = person_rec.Ime;
         Fld_Prezime      = person_rec.Prezime;
         Fld_Ulica        = person_rec.Ulica;
         Fld_Grad         = person_rec.Grad;
         Fld_PostaBr      = person_rec.PostaBr;
         Fld_Tel          = person_rec.Tel;
         Fld_Gsm          = person_rec.Gsm;
         Fld_Email        = person_rec.Email;
         Fld_DatePri      = person_rec.DatePri;
         Fld_DateOdj      = person_rec.DateOdj;
         Fld_StrSpr       = person_rec.StrSpr;
         Fld_StrSprCd     = person_rec.StrSprCd;
         Fld_Jmbg         = person_rec.Jmbg;
         Fld_Oib          = person_rec.Oib;
         Fld_Regob        = person_rec.Regob;
         Fld_OsBrOsig     = person_rec.OsBrOsig;
         Fld_BankaCd      = person_rec.BankaCd;
         Fld_BankaTk      = person_rec.BankaTk;
         Fld_PnbM         = person_rec.PnbM;
         Fld_PnbV         = person_rec.PnbV;
         Fld_MtrosCd      = person_rec.MtrosCd;
         Fld_MtrosTk      = person_rec.MtrosTk;
         Fld_RadMj        = person_rec.RadMj;
         Fld_Napomena     = person_rec.Napomena;
         Fld_IsIzuzet     = person_rec.IsIzuzet;
         Fld_IsIzuzetX    = VvCheckBox.GetString4Bool(person_rec.IsIzuzet);
         Fld_IsPlaca      = person_rec.IsPlaca;
         Fld_IsPlacaX     = VvCheckBox.GetString4Bool(person_rec.IsPlaca);
         Fld_IsUgDj       = person_rec.IsUgDj;
         Fld_IsUgDjX      = VvCheckBox.GetString4Bool(person_rec.IsUgDj);
         Fld_IsAutH       = person_rec.IsAutH;
         Fld_IsAutHX      = VvCheckBox.GetString4Bool(person_rec.IsAutH);
         Fld_IsPoduz      = person_rec.IsPoduz;
         Fld_IsPoduzX     = VvCheckBox.GetString4Bool(person_rec.IsPoduz);
         Fld_IsNadzO      = person_rec.IsNadzO;
         Fld_IsNadzOX     = VvCheckBox.GetString4Bool(person_rec.IsNadzO);
         Fld_Zupanija     = person_rec.Zupan;
         Fld_ZupanCd      = person_rec.ZupCd;
         Fld_BirthDate    = person_rec.BirthDate;
         Fld_VrstaRadVrem = person_rec.VrstaRadVrem;
         Fld_VrstaRadOdns = person_rec.VrstaRadOdns;
         Fld_VrstaIsplate = person_rec.VrstaIsplate;
         Fld_Spol         = person_rec.Spol;
         //       Fld_TsName       = person_rec.TsName      ;
         Fld_TS           = person_rec.TS;

         Fld_PrevStazYY = person_rec.PrevStazYY;
         Fld_PrevStazMM = person_rec.PrevStazMM;
         Fld_PrevStazDD = person_rec.PrevStazDD;

         Fld_Banka2         = person_rec.Banka2        ;
         Fld_Banka2TK       = person_rec.Banka2TK      ;
         Fld_BirthMjestoDrz = person_rec.BirthMjestoDrz;
         Fld_NaravVrRad     = person_rec.NaravVrRad    ;
         Fld_Drzavljanstvo  = person_rec.Drzavljanstvo ;
         Fld_Dozvola        = person_rec.Dozvola       ;
         Fld_Prijava        = person_rec.Prijava       ;
         Fld_Strucno        = person_rec.Strucno       ;
         Fld_Zanimanje      = person_rec.Zanimanje     ;
         Fld_TrajanjeUgOdr  = person_rec.TrajanjeUgOdr ;
         Fld_Cl61ugSuglas   = person_rec.Cl61ugSuglas  ;
         Fld_Cl62ugSuglas   = person_rec.Cl62ugSuglas  ;
         Fld_ProbniRad      = person_rec.ProbniRad     ;
         Fld_PripravStaz    = person_rec.PripravStaz   ;
         Fld_RezIspita      = person_rec.RezIspita     ;
         Fld_RadIno         = person_rec.RadIno        ;
         Fld_GdjeRadIno     = person_rec.GdjeRadIno    ;
         Fld_UstupRadnika   = person_rec.UstupRadnika  ;
         Fld_UstupMjesto    = person_rec.UstupMjesto   ;
         Fld_DrzavaPovezDrs = person_rec.DrzavaPovezDrs;
         Fld_PosaoBenefStaz = person_rec.PosaoBenefStaz;
         Fld_NacinBenef     = person_rec.NacinBenef    ;
         Fld_Sposobnost     = person_rec.Sposobnost    ;
         Fld_MjestoRada     = person_rec.MjestoRada    ;
         Fld_MirovanjeRO    = person_rec.MirovanjeRO   ;
         Fld_RazlogOdj      = person_rec.RazlogOdj     ;
         Fld_Tfs            = person_rec.Tfs           ;
         Fld_SkrRV          = person_rec.SkrRV         ;
         Fld_IsSO           = person_rec.IsSO          ;
         Fld_IsOtherVr      = person_rec.IsOtherVr     ;

         Fld_KorKoef01 = person_rec.KorKoef01;
         Fld_KorKoef02 = person_rec.KorKoef02;
         Fld_KorKoef03 = person_rec.KorKoef03;
         Fld_KorKoef04 = person_rec.KorKoef04;
         Fld_KorKoef05 = person_rec.KorKoef05;
         Fld_KorKoef06 = person_rec.KorKoef06;
         Fld_KorKoef07 = person_rec.KorKoef07;
         Fld_KorKoef08 = person_rec.KorKoef08;
         Fld_KorKoef09 = person_rec.KorKoef09;
         Fld_KorKoef10 = person_rec.KorKoef10;
         Fld_KorKoef11 = person_rec.KorKoef11;
         Fld_KorKoef12 = person_rec.KorKoef12;


         if(ZXC.IsZASTITARI) Fld_RadnoMjesto_Opis = ZXC.luiListaRadnoMjesto.GetNameForThisCd   (person_rec.RadMj);
         if(ZXC.IsZASTITARI) Fld_RadnoMjesto_Broj = ZXC.luiListaRadnoMjesto.GetIntegerForThisCd(person_rec.RadMj);
         

         //===================== 

         SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.None);
         SetSifrarAndAutocomplete<Person>(null, VvSQL.SorterType.None);

         kupdobSifrar_rec = KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD/*VirtualRecID*/ == person_rec.BankaCd);
         if(kupdobSifrar_rec != null) Fld_BankaNaziv = kupdobSifrar_rec.Naziv;
         else                         Fld_BankaNaziv = "";

         kupdobSifrar_rec = KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD/*VirtualRecID*/ == person_rec.MtrosCd);
         if(kupdobSifrar_rec != null) Fld_MtrosNaziv = kupdobSifrar_rec.Naziv;
         else                         Fld_MtrosNaziv = "";

         kupdobSifrar_rec = KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD/*VirtualRecID*/ == person_rec.Banka2);
         if(kupdobSifrar_rec != null) Fld_BankaNaziv2 = kupdobSifrar_rec.Naziv;
         else                         Fld_BankaNaziv2 = "";

         //===================== 
         PutResultFields();

         VvHamper.SetChkBoxRadBttnAutoCheck(this, false);

         InitializeFilterUCFields();

         recordReportLoaded = false;
         DecideIfShouldLoad_VvReport(null, null, null);

         aTransesLoaded[0] = false; // ovdje treba nulirati sve postojece 'xyLoaded' varijable
         aTransesLoaded[1] = false; // ovdje treba nulirati sve postojece 'xyLoaded' varijable
         aTransesLoaded[2] = false; // ovdje treba nulirati sve postojece 'xyLoaded' varijable
         DecideIfShouldLoad_TransDGV(null, null, null);

         // 19.10.2016: 
         //PutIBAN_2017_Fields(person_rec);
      }
   }

   #region PutResultFields()

   private void PutResultFields()
   {
      Ptrans ptrans_rec = PersonDao.GetPrevPtransForPerson(TheDbConnection, person_rec.PersonCD, DateTime.Now, 0, ZXC.DbNavigationRestrictor.Empty);

      bool nemaPtransa = ptrans_rec.VirtualRecID == 0;

      if(nemaPtransa)
      {
         VvHamper.ClearFieldContents(panel_RO);
         return;
      }

      // Za calc ____________ START ________________________ 

      ZXC.PlacaDao.SetMe_Record_byRecID(TheDbConnection, placa_rec, ptrans_rec.T_parentID, false);

      //TheVvTabPage.LoadEventualDependentRecords(conn, placa_rec, TheVvTabPage.IsArhivaTabPage);
      placa_rec.VvDao.LoadTranses(TheDbConnection, placa_rec, TheVvTabPage.IsArhivaTabPage);

      ptrans_rec.CalcTransResults(placa_rec);

      // Za calc ____________ END __________________________ 

      Fld_DokNum     = ptrans_rec.T_dokNum;
      Fld_DokDate    = ptrans_rec.T_dokDate.ToString(ZXC.VvDateFormat);
      Fld_MMYYYY     = ptrans_rec.T_MMYYYY;
      Fld_TtNum      = ptrans_rec.T_ttNum;
      Fld_Tt         = ptrans_rec.T_TT;
      Fld_TtOpis     = ZXC.luiListaPlacaTT.GetNameForThisCd(ptrans_rec.T_TT);
      Fld_FondSati   = ptrans_rec.T_FondSati;
      Fld_RSm_ID     = ptrans_rec.T_RSm_ID;
      Fld_OpcCd      = ptrans_rec.T_opcCD;
      Fld_Opcina     = ptrans_rec.T_opcName;
                     
      Fld_StPrirez   = ptrans_rec.T_stPrirez;
      Fld_KoefOsOd   = ptrans_rec.T_koef;
      Fld_MioII      = GiveOneX4MioII(ptrans_rec.T_isMioII);
      Fld_Spc        = GiveOneLetter4Spc(ptrans_rec.T_spc);
      Fld_InvalidTip = GiveOneLetter4Invalid(ptrans_rec.T_invalidTip);
      Fld_KoefHRVI   = ptrans_rec.T_koefHRVI;
      Fld_SatiRada   = ptrans_rec.R_SatiR;
      Fld_SatiBol    = ptrans_rec.R_SatiB;
      Fld_EVRcount   = ptrans_rec.R_PtranEsCount;
      Fld_OBUcount   = ptrans_rec.R_PtranOsCount;
      Fld_BrutoOsn   = ptrans_rec.T_brutoOsn;
      Fld_TheBruto   = ptrans_rec.R_TheBruto;
      Fld_Netto      = ptrans_rec.R_Netto;
      Fld_NaRuke     = ptrans_rec.R_NaRuke;
      Fld_2Pay       = ptrans_rec.R_2Pay;
      Fld_BenefStaz  = ptrans_rec.T_rsB.ToString();

   }

   private string GiveOneX4MioII(bool isMioII)
   {
      if(isMioII) return "DA";
      else        return "NE";
   }

   private string GiveOneLetter4Spc(Ptrans.SpecEnum specEnum)
   {
      switch(specEnum)
      {
         case Ptrans.SpecEnum.NOVOZAPOSL   : return "N";
         case Ptrans.SpecEnum.PENZ         : return "U";
         case Ptrans.SpecEnum.MINMIONE     : return "M";
         case Ptrans.SpecEnum.MAXMIONE     : return "O";
         case Ptrans.SpecEnum.CLANUPRAVE   : return "C";
         case Ptrans.SpecEnum.NOVO_MINMIONE: return "X";

         default: return "";

      }
   }

   private string GiveOneLetter4Invalid(Ptrans.InvalidEnum invalidEnum)
   {
      switch(invalidEnum)
      {
         case Ptrans.InvalidEnum.HRVI   : return "H";
         case Ptrans.InvalidEnum.INVALID: return "I";

         default: return "";

      }
   }

   #endregion PutResultFields()

   public override void GetFields(bool fuse)
   {
      if(person_rec == null) person_rec = new Person();

      person_rec.PersonCD     = Fld_PersonCd;
      person_rec.Ime          = Fld_Ime;
      person_rec.Prezime      = Fld_Prezime;
      person_rec.Ulica        = Fld_Ulica;
      person_rec.Grad         = Fld_Grad;
      person_rec.PostaBr      = Fld_PostaBr;
      person_rec.Tel          = Fld_Tel;
      person_rec.Gsm          = Fld_Gsm;
      person_rec.Email        = Fld_Email;
      person_rec.DatePri      = Fld_DatePri;
      person_rec.DateOdj      = Fld_DateOdj;
      person_rec.StrSpr       = Fld_StrSpr;
      person_rec.StrSprCd     = Fld_StrSprCd;
      person_rec.Jmbg         = Fld_Jmbg;
      person_rec.Oib          = Fld_Oib;
      person_rec.Regob        = Fld_Regob;
      person_rec.OsBrOsig     = Fld_OsBrOsig;
      person_rec.BankaCd      = Fld_BankaCd;
      person_rec.BankaTk      = Fld_BankaTk;
      person_rec.PnbM         = Fld_PnbM;
      person_rec.PnbV         = Fld_PnbV;
      person_rec.MtrosCd      = Fld_MtrosCd;
      person_rec.MtrosTk      = Fld_MtrosTk;
      person_rec.RadMj        = Fld_RadMj;
      person_rec.Napomena     = Fld_Napomena;
      person_rec.IsIzuzet     = Fld_IsIzuzet;
      person_rec.IsPlaca      = Fld_IsPlaca;
      person_rec.IsUgDj       = Fld_IsUgDj;
      person_rec.IsAutH       = Fld_IsAutH;
      person_rec.IsPoduz      = Fld_IsPoduz;
      person_rec.IsNadzO      = Fld_IsNadzO;
    //person_rec.Zupan        = Fld_Zupanija     ;
      person_rec.ZupCd        = Fld_ZupanCd;
      person_rec.BirthDate    = Fld_BirthDate;
      person_rec.VrstaRadVrem = Fld_VrstaRadVrem;
      person_rec.VrstaRadOdns = Fld_VrstaRadOdns;
      person_rec.VrstaIsplate = Fld_VrstaIsplate;
      person_rec.Spol         = Fld_Spol;
    //  person_rec.TsName     = Fld_TsName       ;
      person_rec.TS           = Fld_TS;
      person_rec.PrevStazYY   = Fld_PrevStazYY;
      person_rec.PrevStazMM   = Fld_PrevStazMM;
      person_rec.PrevStazDD   = Fld_PrevStazDD;

      person_rec.Banka2         = Fld_Banka2         ;
      person_rec.Banka2TK       = Fld_Banka2TK       ;
      person_rec.BirthMjestoDrz = Fld_BirthMjestoDrz ;
      person_rec.NaravVrRad     = Fld_NaravVrRad     ;
      person_rec.Drzavljanstvo  = Fld_Drzavljanstvo  ;
      person_rec.Dozvola        = Fld_Dozvola        ;
      person_rec.Prijava        = Fld_Prijava        ;
      person_rec.Strucno        = Fld_Strucno        ;
      person_rec.Zanimanje      = Fld_Zanimanje      ;
      person_rec.TrajanjeUgOdr  = Fld_TrajanjeUgOdr  ;
      person_rec.Cl61ugSuglas   = Fld_Cl61ugSuglas   ;
      person_rec.Cl62ugSuglas   = Fld_Cl62ugSuglas   ;
      person_rec.ProbniRad      = Fld_ProbniRad      ;
      person_rec.PripravStaz    = Fld_PripravStaz    ;
      person_rec.RezIspita      = Fld_RezIspita      ;
      person_rec.RadIno         = Fld_RadIno         ;
      person_rec.GdjeRadIno     = Fld_GdjeRadIno     ;
      person_rec.UstupRadnika   = Fld_UstupRadnika   ;
      person_rec.UstupMjesto    = Fld_UstupMjesto    ;
      person_rec.DrzavaPovezDrs = Fld_DrzavaPovezDrs ;
      person_rec.PosaoBenefStaz = Fld_PosaoBenefStaz ;
      person_rec.NacinBenef     = Fld_NacinBenef     ;
      person_rec.Sposobnost     = Fld_Sposobnost     ;
      person_rec.MjestoRada     = Fld_MjestoRada     ;
      person_rec.MirovanjeRO    = Fld_MirovanjeRO    ;
      person_rec.RazlogOdj      = Fld_RazlogOdj      ;
      person_rec.Tfs            = Fld_Tfs            ;
      person_rec.SkrRV          = Fld_SkrRV          ;
      person_rec.IsSO           = Fld_IsSO           ;
      person_rec.IsOtherVr      = Fld_IsOtherVr      ;



      person_rec.KorKoef01 = Fld_KorKoef01;
      person_rec.KorKoef02 = Fld_KorKoef02;
      person_rec.KorKoef03 = Fld_KorKoef03;
      person_rec.KorKoef04 = Fld_KorKoef04;
      person_rec.KorKoef05 = Fld_KorKoef05;
      person_rec.KorKoef06 = Fld_KorKoef06;
      person_rec.KorKoef07 = Fld_KorKoef07;
      person_rec.KorKoef08 = Fld_KorKoef08;
      person_rec.KorKoef09 = Fld_KorKoef09;
      person_rec.KorKoef10 = Fld_KorKoef10;
      person_rec.KorKoef11 = Fld_KorKoef11;
      person_rec.KorKoef12 = Fld_KorKoef12;
   }

   #endregion PutFields(), GetFields()

   #region Put Trans DGV Fileds

   private const string ptrans_TabPageName = "PTrans";
   private const string ptrane_TabPageName = "ETrans";
   private const string ptrano_TabPageName = "OTrans";

   // Tu dolazimo na 1 nacin: 1. Classic PutFields 
   private void InitializeFilterUCFields()
   {
      SetFilterRecordDependentDefaults(); // filter.KontoOd i Do = kplan_rec.Konto (punimo bussiness od filtera, ne UC)
      ThePersonFilterUC.PutFilterFields(ThePersonFilter);
   }

   // Tu dolazimo na 2 nacina:          
   // 1. Classic PutFields              
   // 2. TheTabControl.SelectionChanged 
   public override void DecideIfShouldLoad_TransDGV(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   {
      ZXC.VvInnerTabPageKindEnum innerTabPageKind = ((VvInnerTabPage)TheTabControl.SelectedTab).TheInnerTabPageKindEnum;


      if(innerTabPageKind == ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage)
      {
         ThePersonFilter.IsPopulatingTransDGV = true;

         ThePersonFilter.PersonCards = (PersonCardFilter.PersonCardsEnum)Enum.Parse(typeof(PersonCardFilter.PersonCardsEnum), ((VvInnerTabPage)TheTabControl.SelectedTab).Name);

         ThePersonFilterUC.Fld_PersonCard = ThePersonFilter.PersonCards;

         if(aTransesLoaded[0] == false)
         {
            LoadRecordList_AND_PutTransDgvFields();
         }
      }
      
   }

   // Tu dolazimo na 2 nacina:          
   // 1. ButtonIzlistaj_Click (sa FilterUC-a) 
   // 2. DecideIfShouldLoad_TransDGV    
   public override void LoadRecordList_AND_PutTransDgvFields()
   {
      int rowIdx, idxCorrector;

      ThePersonFilterUC.GetFilterFields();
      ThePersonFilterUC.AddFilterMemberz(ThePersonFilter, null);

      if(person_rec.Ptranses == null) person_rec.Ptranses = new List<Ptrans>();
      else                            person_rec.Ptranses.Clear();

      if(person_rec.Ptranes == null) person_rec.Ptranes = new List<Ptrane>();
      else                           person_rec.Ptranes.Clear();

      if(person_rec.Ptranos == null) person_rec.Ptranos = new List<Ptrano>();
      else                           person_rec.Ptranos.Clear();

      string orderByColumns;

      if(ThePersonFilterUC.Fld_DokumentSort == VvSQL.SorterType.DokNum)
         orderByColumns = "                t_dokNum DESC, t_serial DESC";
      else
         orderByColumns = "t_dokDate DESC, t_dokNum DESC, t_serial DESC";

      VvDaoBase.LoadGenericVvDataRecordList<Ptrans>(TheDbConnection, person_rec.Ptranses, ThePersonFilter.FilterMembers, orderByColumns);
      VvDaoBase.LoadGenericVvDataRecordList<Ptrane>(TheDbConnection, person_rec.Ptranes , ThePersonFilter.FilterMembers, orderByColumns);
      VvDaoBase.LoadGenericVvDataRecordList<Ptrano>(TheDbConnection, person_rec.Ptranos , ThePersonFilter.FilterMembers, orderByColumns);

      aTransesLoaded[0] =
      aTransesLoaded[1] =
      aTransesLoaded[2] = true;

      //--------------------------------------------------------------------------------------------------------------------------------------------- 
      aTransesGrid[0].Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(aTransesGrid[0]);


      foreach(Ptrans ptrans_rec in person_rec.Ptranses)
      {
         aTransesGrid[0].Rows.Add();

         rowIdx = aTransesGrid[0].RowCount - idxCorrector;


         // Za calc ____________ START ________________________ 

         ZXC.PlacaDao.SetMe_Record_byRecID(TheDbConnection, placa_rec, ptrans_rec.T_parentID, false);

         //TheVvTabPage.LoadEventualDependentRecords(conn, placa_rec, TheVvTabPage.IsArhivaTabPage);
         placa_rec.VvDao.LoadTranses(TheDbConnection, placa_rec, TheVvTabPage.IsArhivaTabPage);

         ptrans_rec.CalcTransResults(placa_rec);

         // Za calc ____________ END __________________________ 

         aTransesGrid[0][ 0, rowIdx].Value = ptrans_rec.T_parentID;
         aTransesGrid[0][ 1, rowIdx].Value = ptrans_rec.T_dokDate;
         aTransesGrid[0][ 2, rowIdx].Value = ptrans_rec.T_dokNum;
         aTransesGrid[0][ 3, rowIdx].Value = ptrans_rec.T_serial;
         aTransesGrid[0][ 4, rowIdx].Value = ptrans_rec.T_TT;
         aTransesGrid[0][ 5, rowIdx].Value = ptrans_rec.T_MMYYYY;
         aTransesGrid[0][ 6, rowIdx].Value = ptrans_rec.T_brutoOsn;
         aTransesGrid[0][ 7, rowIdx].Value = ptrans_rec.R_TheBruto;
         aTransesGrid[0][ 8, rowIdx].Value = ptrans_rec.R_Netto;
         aTransesGrid[0][ 9, rowIdx].Value = ptrans_rec.R_NaRuke;
         aTransesGrid[0][10, rowIdx].Value = ptrans_rec.R_2Pay;

         aTransesGrid[0].Rows[rowIdx].HeaderCell.Value = (person_rec.Ptranses.Count - rowIdx).ToString();
      }
      //--------------------------------------------------------------------------------------------------------------------------------------------- 

      //--------------------------------------------------------------------------------------------------------------------------------------------- 
      aTransesGrid[1].Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(aTransesGrid[1]);

      foreach(Ptrane ptrane_rec in person_rec.Ptranes)
      {
         aTransesGrid[1].Rows.Add();

         rowIdx = aTransesGrid[1].RowCount - idxCorrector;

         aTransesGrid[1][0, rowIdx].Value = ptrane_rec.T_parentID;
         aTransesGrid[1][1, rowIdx].Value = ptrane_rec.T_dokDate;
         aTransesGrid[1][2, rowIdx].Value = ptrane_rec.T_dokNum;
         aTransesGrid[1][3, rowIdx].Value = ptrane_rec.T_serial;
         aTransesGrid[1][4, rowIdx].Value = ptrane_rec.T_TT;
         aTransesGrid[1][5, rowIdx].Value = ptrane_rec.T_vrstaR_name;
         aTransesGrid[1][6, rowIdx].Value = ptrane_rec.T_cijPerc;
         aTransesGrid[1][7, rowIdx].Value = ptrane_rec.T_rsOO;
         aTransesGrid[1][8, rowIdx].Value = ptrane_rec.T_rsOD;
         aTransesGrid[1][9, rowIdx].Value = ptrane_rec.T_rsDO;
         aTransesGrid[1][10, rowIdx].Value = ptrane_rec.T_sati;

         aTransesGrid[1].Rows[rowIdx].HeaderCell.Value = (person_rec.Ptranes.Count - rowIdx).ToString();
      }
      //--------------------------------------------------------------------------------------------------------------------------------------------- 

      //--------------------------------------------------------------------------------------------------------------------------------------------- 
      aTransesGrid[2].Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(aTransesGrid[2]);

      foreach(Ptrano ptrano_rec in person_rec.Ptranos)
      {
         aTransesGrid[2].Rows.Add();

         rowIdx = aTransesGrid[2].RowCount - idxCorrector;

         aTransesGrid[2][0, rowIdx].Value = ptrano_rec.T_parentID;
         aTransesGrid[2][1, rowIdx].Value = ptrano_rec.T_dokDate;
         aTransesGrid[2][2, rowIdx].Value = ptrano_rec.T_dokNum;
         aTransesGrid[2][3, rowIdx].Value = ptrano_rec.T_serial;
         aTransesGrid[2][4, rowIdx].Value = ptrano_rec.T_TT;
         aTransesGrid[2][5, rowIdx].Value = ptrano_rec.T_dateStart;
         aTransesGrid[2][6, rowIdx].Value = ptrano_rec.T_ukBrRata;
         aTransesGrid[2][7, rowIdx].Value = ptrano_rec.T_opisOb;
         aTransesGrid[2][8, rowIdx].Value = ptrano_rec.T_kupdob_cd;
         aTransesGrid[2][9, rowIdx].Value = ptrano_rec.T_kupdob_tk;
         aTransesGrid[2][10, rowIdx].Value = ptrano_rec.T_iznosOb;

         aTransesGrid[2].Rows[rowIdx].HeaderCell.Value = (person_rec.Ptranos.Count - rowIdx).ToString();
      }
      //--------------------------------------------------------------------------------------------------------------------------------------------- 

      //VvDocumentRecordUC.RenumerateLineNumbers(gridFtrans, 0);

   }

   #endregion Put Trans DGV Fileds

   #region Overriders And Specifics

   #region VvDataRecord/VvDaoBase

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.person_rec; }
      set { this.person_rec = (Person)value; }
   }

   public override VvSifrarRecord VirtualSifrarRecord
   {
      get { return this.VirtualDataRecord as VvSifrarRecord; }
      set { this.VirtualDataRecord = (Person)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.PersonDao; }
   }

   #endregion VvDataRecord/VvDaoBase

   #region VvFindDialog

   public override VvFindDialog CreateVvFindDialog()
   {
      return CreateFind_Person_Dialog();
   }

   public static VvFindDialog CreateFind_Person_Dialog()
   {
      VvForm.VvSubModul vvSubModul = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsPER);
      VvDataRecord vvDataRecord = ZXC.TheVvForm.CreateTheVvDataRecord_SwitchSubModulEnum(vvSubModul);

      VvFindDialog vvFindDialog = new VvFindDialog();

      VvRecLstUC vvRecListUC = new PersonListUC(vvFindDialog, (Person)vvDataRecord, vvSubModul);

      vvFindDialog.TheRecListUC = vvRecListUC;

      return vvFindDialog;
   }

   #endregion VvFindDialog

   #region PrintSifrarRecord

   public PlacaSifrartFilterUC ThePersonFilterUC { get; set; }
   public PersonCardFilter ThePersonFilter { get; set; }

   //public RptP_PersonCard    TheRptP_Person { get; set; }

   //public override VvReport VirtualReport
   //{
   //   get
   //   {
   //      return this.specificPersonReport;
   //   }
   //}

//   protected VvReport specificPersonReport;
   protected string specificPersonReportName;

   public override string VirtualReportName
   {
      get
      {
         return this.specificPersonReportName;
      }
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      PersonCardFilter personCardFilter = (PersonCardFilter)vvRptFilter;

      switch(personCardFilter.PersonCards)
      {
         case PersonCardFilter.PersonCardsEnum.Matični: return new VvPlacaReport(new CR_PersonMaticni(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, false, false), reportName, personCardFilter);
         case PersonCardFilter.PersonCardsEnum.PTrans: return new VvPlacaReport(new CR_PersonPtrans(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, false, false), reportName, personCardFilter);
         case PersonCardFilter.PersonCardsEnum.ETrans: return new VvPlacaReport(new CR_PersonPtrane(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, true, false), reportName, personCardFilter);
         case PersonCardFilter.PersonCardsEnum.OTrans: return new VvPlacaReport(new CR_PersonPtrano(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, false, true), reportName, personCardFilter);

         default: ZXC.aim_emsg("{0}\nPrintSomePersonDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), personCardFilter.PersonCards); return null;
      }
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.ThePersonFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.ThePersonFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
      this.ThePersonFilter.PersonCDod = person_rec.PersonCD;
      this.ThePersonFilter.PersonCDdo = person_rec.PersonCD;
      this.ThePersonFilter.PrezimeOd = person_rec.Prezime;
      this.ThePersonFilter.PrezimeDo = person_rec.Prezime;
   }

   #region SetReportAndReportName_ForThisInnerTabPage

   protected override void SetReportAndReportName_ForThisInnerTabPage(string selectedTabName)
   {
      PersonCardFilter.PersonCardsEnum wantedDocumentEnum;

      //if(selectedTabName.Contains("č")) selectedTabName.Replace("č", "c");

      if(selectedTabName == "PredPrint")
      {
         wantedDocumentEnum = ThePersonFilter.PersonCards;
      }
      else
      {
         if(selectedTabName == "Matični2") selectedTabName = "Matični";
         wantedDocumentEnum = (PersonCardFilter.PersonCardsEnum)Enum.Parse(typeof(PersonCardFilter.PersonCardsEnum), selectedTabName);
      }

      ThePersonFilter.PersonCards = wantedDocumentEnum;
      ThePersonFilterUC.Fld_PersonCard = wantedDocumentEnum;

      switch(wantedDocumentEnum)
      {
         case PersonCardFilter.PersonCardsEnum.Matični: specificPersonReportName = "Kartica djelatnika";
            TheVvReport = new VvPlacaReport(new CR_PersonMaticni(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, false, false), specificPersonReportName, ThePersonFilter);
            break;
         case PersonCardFilter.PersonCardsEnum.PTrans: specificPersonReportName = "REKAPITULACIJA PLAĆA";
            TheVvReport = new VvPlacaReport(new CR_PersonPtrans(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, false, false), specificPersonReportName, ThePersonFilter);
            break;
         case PersonCardFilter.PersonCardsEnum.ETrans: specificPersonReportName = "REKAPITULACIJA EVIDENCIJE RADA";
            TheVvReport = new VvPlacaReport(new CR_PersonPtrane(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, true, false), specificPersonReportName, ThePersonFilter);
            break;
         case PersonCardFilter.PersonCardsEnum.OTrans: specificPersonReportName = "REKAPITULACIJA OBUSTAVA";
            TheVvReport = new VvPlacaReport(new CR_PersonPtrano(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, false, true), specificPersonReportName, ThePersonFilter);
            break;
      }
   }

   #endregion SetReportAndReportName_ForThisInnerTabPage

   #endregion PrintSifrarRecord

   public override Size ThisUcSize
   {
      get
      {
         return new Size(/*panel_RO.Right*/ hamp_napom.Right + ZXC.QunMrgn, hampRO_neto.Bottom + ZXC.QunMrgn);
      }
   }

   #region PutNew_Sifra_Field

   public override void PutNew_Sifra_Field(uint newSifra)
   {
      Fld_PersonCd = newSifra;
   }

   #endregion PutNew_Sifra_Field

   #region CalcIBAN_2017

   private void PutIBAN_2017_Fields(Person person_rec)
   {
      Fld_IBAN_TekuciRedovni   = CalcIBAN_2017_TEKUCI_REDOVNI  (person_rec/*, Person.IBAN_kind.TEKUCI_REDOVNI  */);
      Fld_IBAN_TekuciZasticeni = CalcIBAN_2017_TEKUCI_ZASTICENI(person_rec/*, Person.IBAN_kind.TEKUCI_ZASTICENI*/);
      Fld_IBAN_ZiroRacun       = CalcIBAN_2017_ZIRO_RACUN      (person_rec/*, Person.IBAN_kind.ZIRO_RACUN      */);
      Fld_IBAN_Banke           = CalcIBAN_2017_BANKA_IBAN      (person_rec/*, Person.IBAN_kind.BANKA_IBAN      */);
   }

   private bool IsPnbV_Normal { get { return person_rec.PnbV.SubstringSafe(11, 2) == VvPlacaReport.Normal_IBAN_root; } }
   private bool IsPnbV_Zastic { get { return person_rec.PnbV.SubstringSafe(11, 2) == VvPlacaReport.Zastic_IBAN_root; } }
   private bool IsPnbV_ZiroRn { get { return person_rec.PnbV.SubstringSafe(11, 2) == VvPlacaReport.ZiroRn_IBAN_root; } }

   private Kupdob GetLast_RR_Ptrano_Kupdob(Person person_rec, bool isRedovni)
   {
      Kupdob kupdob_rec = null;

      ZXC.DbNavigationRestrictor dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor("t_tt"      , new string[] { /*Fld_TT,*/ Placa.TT_REDOVANRAD, Placa.TT_PODUZETPLACA/*, Placa.TT_AUTORHONOR, Placa.TT_AUTORHONUMJ, Placa.TT_NADZORODBOR, Placa.TT_TURSITVIJECE, Placa.TT_PLACAUNARAVI*/ });
      
      ZXC.DbNavigationRestrictor dbNavigationRestrictor_2;
      
      if(isRedovni)
      {
         dbNavigationRestrictor_2 = new ZXC.DbNavigationRestrictor("t_isZastRn", new string[] { ((int)Ptrans.PtranoKind.NEZASTICENIrn).ToString() });
      }
      else
      {
         dbNavigationRestrictor_2 = new ZXC.DbNavigationRestrictor("t_isZastRn", new string[] { ((int)Ptrans.PtranoKind.ZASTICENIrn).ToString() });
      }
      
      Ptrano prevPtrano_rec = PersonDao.GetPrevPtranoForPerson(TheDbConnection, person_rec.PersonCD, ZXC.projectYearLastDay, uint.MaxValue, dbNavigationRestrictor_TT, dbNavigationRestrictor_2);
      bool personImaPrevPtrano = prevPtrano_rec.VirtualRecID != 0;
      if(personImaPrevPtrano)
      {
         kupdob_rec = Get_Kupdob_FromVvUcSifrar(prevPtrano_rec.T_kupdob_cd);
      }

      if(kupdob_rec == null)
      {
         // ipak ne provjeravaj 
         //if(isRedovni) ZXC.aim_emsg(MessageBoxIcon.Warning, "Person\n\n{0}\n\nNema 'Last_RR_Ptrano_Kupdoba'", person_rec);

         kupdob_rec = new Kupdob();
      }

      return kupdob_rec;
   }

   private string CalcIBAN_2017_TEKUCI_REDOVNI(Person person_rec)
   {
      string theIBAN;
      Person.IBAN_kind wantedIBAN_kind = Person.IBAN_kind.TEKUCI_REDOVNI;

      if(person_rec.VrstaIsplate == Person.VrstaIsplateEnum.BANKA)
      {
         if(IsPnbV_Normal)
         {
            theIBAN = person_rec.PnbV;

            CheckIBANroot(wantedIBAN_kind, theIBAN);

            return theIBAN;
         }
         else if(IsPnbV_Zastic)
         {
            theIBAN = GetLast_RR_Ptrano_Kupdob(person_rec, true).Ziro1_asIBAN;

            CheckIBANroot(wantedIBAN_kind, theIBAN);

            return theIBAN;
         }
      }

      if(person_rec.VrstaIsplate == Person.VrstaIsplateEnum.TEKUCI)
      {
         if(person_rec.Is_BankaKupdob_Ziro1_Normal)
         {
            theIBAN = person_rec.BankaKupdob.Ziro1_asIBAN;

            CheckIBANroot(wantedIBAN_kind, theIBAN);

            return theIBAN;
         }
         else if(person_rec.Is_BankaKupdob_Ziro1_Zastic)
         {
            theIBAN = GetLast_RR_Ptrano_Kupdob(person_rec, true).Ziro1_asIBAN;

            CheckIBANroot(wantedIBAN_kind, theIBAN);

            return theIBAN;
         }
      }

      if(person_rec.IsPlaca) ZXC.aim_emsg(MessageBoxIcon.Warning, "Ne mogu saznati IBAN za\n\nTekući račun - REDOVNI.");

      return "";
   }

   private string CalcIBAN_2017_TEKUCI_ZASTICENI(Person person_rec)
   {
      string theIBAN;
      Person.IBAN_kind wantedIBAN_kind = Person.IBAN_kind.TEKUCI_ZASTICENI;

      if(person_rec.VrstaIsplate == Person.VrstaIsplateEnum.BANKA)
      {
         if(IsPnbV_Zastic)
         {
            theIBAN = person_rec.PnbV;

            CheckIBANroot(wantedIBAN_kind, theIBAN);

            return theIBAN;
         }
         else if(IsPnbV_Normal)
         {
            theIBAN = GetLast_RR_Ptrano_Kupdob(person_rec, false).Ziro1_asIBAN;

            CheckIBANroot(wantedIBAN_kind, theIBAN);

            return theIBAN;
         }
      }

      if(person_rec.VrstaIsplate == Person.VrstaIsplateEnum.TEKUCI)
      {
         if(person_rec.Is_BankaKupdob_Ziro1_Zastic)
         {
            theIBAN = person_rec.BankaKupdob.Ziro1_asIBAN;

            CheckIBANroot(wantedIBAN_kind, theIBAN);

            return theIBAN;
         }
         else if(person_rec.Is_BankaKupdob_Ziro1_Normal)
         {
            theIBAN = GetLast_RR_Ptrano_Kupdob(person_rec, false).Ziro1_asIBAN;

            CheckIBANroot(wantedIBAN_kind, theIBAN);

            return theIBAN;
         }
      }

      //if(person_rec.IsPlaca) ZXC.aim_emsg(MessageBoxIcon.Warning, "Ne mogu saznati IBAN za\n\nTekući račun - ZASTICENI.");

      return "";
   }

   private string CalcIBAN_2017_ZIRO_RACUN(Person person_rec) 
   {
      string theIBAN;
      Person.IBAN_kind wantedIBAN_kind = Person.IBAN_kind.ZIRO_RACUN;

      if(person_rec.Is_BankaKupdob_Ziro1_ZiroRn)
      {
         theIBAN = person_rec.BankaKupdob.Ziro1_asIBAN;

         CheckIBANroot(wantedIBAN_kind, theIBAN);

         return theIBAN;
      }

      if(person_rec.IsAutH   ||
         person_rec.IsUgDj   ||
         person_rec.IsNadzO  ||
         person_rec.IsOtherVr )
        
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Ne mogu saznati IBAN za\n\nŽIRO RAČUN.");

      return "";
   }

   private string CalcIBAN_2017_BANKA_IBAN(Person person_rec)
   {
      if(person_rec.VrstaIsplate == Person.VrstaIsplateEnum.BANKA)
      {
         string bankaKupdobZiro1 = person_rec.BankaKupdob.Ziro1_asIBAN;

         if(bankaKupdobZiro1.IsEmpty()) ZXC.aim_emsg(MessageBoxIcon.Warning, "Ne mogu saznati IBAN\n\nBANKE.");

         return bankaKupdobZiro1;
      }

      return "";
   }

   private void CheckIBANroot(Person.IBAN_kind wantedIBAN_kind, string theIBAN)
   {
      if(theIBAN.IsEmpty()) return;

      if(Person.CheckIBANroot(wantedIBAN_kind, theIBAN) == false) ZXC.aim_emsg(MessageBoxIcon.Error, "Krivi IBAN_kind.\n\nTraženi kind\t{0}\n\nIBAN\t{1}", wantedIBAN_kind, theIBAN);
   }

   #endregion CalcIBAN_2017

   #endregion Overriders And Specifics

   #region Update_VvDataRecord (Legacy naming convention)

   /// <summary>
   /// 'FindVvDataRecord' procedura. Inicirana:
   /// 1. Context menu (Mouse right click)
   /// 2. Mouse click (Ctrl ili Alt click)
   /// 3. Keyboard initiated (Ctrl/Alt + F/Space)
   /// </summary>
   /// <param name="startValue"></param>
   /// <returns></returns>
   public static string Update_Person(VvSQL.SorterType whichInformation, object startValue, ZXC.AutoCompleteRestrictor sifrarRestrictor)
   {
      Person person_rec = new Person();
      PersonListUC personListUC;
      XSqlConnection dbConnection = ZXC.TheVvForm.TheDbConnection;

      VvFindDialog dlg = CreateFind_Person_Dialog();

      personListUC = (PersonListUC)(dlg.TheRecListUC);

      switch(whichInformation)
      {
         case VvSQL.SorterType.Person: personListUC.Fld_FromPrezime = startValue.ToString(); break;
         case VvSQL.SorterType.Code: personListUC.Fld_FromPersonCD = ZXC.ValOrZero_UInt(startValue.ToString()); break;

         default: ZXC.aim_emsg(" 111: For Person, trazi po [{1}] still nedovrseno!", whichInformation); break;
      }

      #region sifrarRestrictor
      
      // new, from 05.06.2009 

      if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.PER_RR_Only   ) personListUC.Fld_FilterIsPlaca = true;
      if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.PER_AHiAU_Only) personListUC.Fld_FilterIsAutH  = true;
      if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.PER_UD_Only   ) personListUC.Fld_FilterIsUgDj  = true;
      if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.PER_PP_Only   ) personListUC.Fld_FilterIsPoduz = true;
      if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.PER_NO_Only   ) personListUC.Fld_FilterIsNadzO = true;

      #endregion sifrarRestrictor

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         if(!ZXC.PersonDao.SetMe_Record_byRecID(dbConnection, person_rec, (uint)dlg.SelectedRecID, false)) return null;
      }
      else
      {
         person_rec = null;
      }

      if(dlg.SelectionIsNewlyAddedRecord == true) ZXC.ShouldForceSifrarRefreshing = true;

      dlg.Dispose();

      if(person_rec != null)
      {
         switch(whichInformation)
         {
            // 13.02.2020: HZTK... 
          //case VvSQL.SorterType.Person: return person_rec.Prezime            ;
            case VvSQL.SorterType.Person: return person_rec.PrezimeIme         ;
            case VvSQL.SorterType.Code  : return person_rec.PersonCD.ToString();

            default: ZXC.aim_emsg(" 222: For Person, trazi po [{1}] still nedovrseno!", whichInformation); return null;
         }
      }
      else return null;
   }

   #endregion Update_VvDataRecord (Legacy naming convention)

}

public class VvRenamePersonDlg : VvDialog
{
   #region Fieldz

   private Button okButton, cancelButton;
   private VvHamper hamper;
   private int dlgWidth, dlgHeight;
   private VvTextBox tbx_newPersonCD;

   #endregion Fieldz

   #region Constructor

   public VvRenamePersonDlg()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Preimenuj šifru radnika";

      CreateHamper();

      dlgWidth = hamper.Right + ZXC.QunMrgn;
      dlgHeight = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_newPersonCD, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
   }

   #endregion Constructor

   #region hamper

   private void CreateHamper()
   {
      hamper = new VvHamper(2, 1, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt = new int[] { ZXC.Q5un, ZXC.Q4un };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      int columnSize = ZXC.PersonDao.GetSchemaColumnSize(ZXC.PersonDao.CI.personCD);

      Label lbl = hamper.CreateVvLabel(0, 0, "Nova šifra:", ContentAlignment.MiddleRight);
      tbx_newPersonCD = hamper.CreateVvTextBox(1, 0, "tbx_newPersonCD", "", columnSize);
   }

   #endregion hamper

   #region Event cancelButton

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Event cancelButton

   #region Fld_

   public uint Fld_NewPersonCD
   {
      get { return ZXC.ValOrZero_UInt(tbx_newPersonCD.Text); }
      set { tbx_newPersonCD.Text = value.ToString("0000"); }
   }

   #endregion Fld_

}
