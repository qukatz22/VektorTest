using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public partial class PutNalDUC : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_PersonCd, tbx_ime, tbx_prezime, 
                     tbx_datePuta, tbx_odrediste, tbx_zadatak, tbx_dana, tbx_vozilo, tbx_trskTerete, tbx_acc, 
                     tbx_dateOd, tbx_dateDo, tbx_brSati, tbx_brDnevn, tbx_iznosDnev, tbx_ValName,
                     tbx_prilog, tbx_radMjesto, 
                     tbx_ukDnevnice, tbx_ukPrijevoz, tbx_ukOsTr, tbx_ukAcc, tbx_ukIsplPov,
                     tbx_dateObracuna, tbx_voziloCD, tbx_konto, tbx_konto2, tbx_strE_256, tbx_drzava, tbx_pozicijaOp;

   public VvHamper  hamp_radnik, hamp_putNal, hamp_obrDnev, hamp_sumeAll, hamp_linkIzvj, hamp_vozilo, hamp_zadatak, hamp_veza, hamp_izvj;
   
   private VvDateTimePicker dtp_datePuta, dtp_dateOd, dtp_dateDo, dtp_dateObracuna;
  
   private CheckBox cbx_isPrivate;

   #endregion Fieldz

   #region Constructor

   public PutNalDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)
   {
   }

   #endregion Constructor

   #region CreateSpecificHampers()

   public void InitializeHamper_Radnik(out VvHamper hamper)
   {
      hamper = new VvHamper(12, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0        1                 2            3        4                5                  6                7       8         9             10               11     
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q2un, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un, ZXC.Q6un, ZXC.Q4un - ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.Q2un, ZXC.QUN , ZXC.Q2un , ZXC.Q3un+ZXC.Qun2, ZXC.Q3un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4,           ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,  ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4  };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

                     hamper.CreateVvLabel  (0, 0, "Djelatnik:", ContentAlignment.MiddleRight);
      tbx_PersonCd = hamper.CreateVvTextBox(1, 0, "tbx_PersonCd", "Sifra djelatnika", GetDB_ColumnSize(DB_ci.personCD)); 
      tbx_prezime  = hamper.CreateVvTextBox(2, 0, "tbx_prezime" , "Prezime"         , GetDB_ColumnSize(DB_ci.personPrezim), 1, 0);
      tbx_ime      = hamper.CreateVvTextBox(4, 0, "tbx_ime"     , "Ime"             , GetDB_ColumnSize(DB_ci.personIme));
      tbx_ime.JAM_ReadOnly = true;
      
      tbx_PersonCd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_prezime.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_prezime.Font = ZXC.vvFont.LargeBoldFont;
      
                      hamper.CreateVvLabel  (5, 0, "Radno mjesto:", ContentAlignment.MiddleRight);
      tbx_radMjesto = hamper.CreateVvTextBox(6, 0, "tbx_radMjesto", "Rdano mjesto", GetDB_ColumnSize(DB_ci.strA_40), 5, 0);
      tbx_radMjesto.JAM_ReadOnly = true;
      
      tbx_PersonCd.JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType     , new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterSifra)  , new EventHandler(AnyPersonTextBoxLeave));
      tbx_prezime .JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonTextBoxLeave));
      

   }

   public void InitializeHamper_PutNal(out VvHamper hamper)
   {
      hamper = new VvHamper(13, 4, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0        1                 2            3        4                5                  6                7                           8              9             10                  11                       12 
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q2un, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un, ZXC.Q6un, ZXC.Q4un - ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.Q2un - ZXC.Qun2, ZXC.QUN + ZXC.Qun2 , ZXC.QUN-ZXC.Qun4, ZXC.Q2un-ZXC.Qun4 , ZXC.Q2un+ZXC.Qun2+ZXC.Qun4, ZXC.Q3un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4,           ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4           ,         ZXC.Qun4,  ZXC.Qun4,          ZXC.Qun4,            ZXC.Qun4  };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN ,ZXC.QUN ,ZXC.QUN , ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8,ZXC.Qun8,ZXC.Qun8, ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

                     hamper.CreateVvLabel  (0, 0, "Datum puta:", ContentAlignment.MiddleRight);
      tbx_datePuta = hamper.CreateVvTextBox(1, 0, "tbx_datePuta", "Datum puta", GetDB_ColumnSize(DB_ci.dateA), 1, 0);
      tbx_datePuta.JAM_IsForDateTimePicker = true;
      dtp_datePuta = hamper.CreateVvDateTimePicker(1, 0, "", 1, 0, tbx_datePuta);
      dtp_datePuta.Name = "dtp_datePuta";
      tbx_datePuta.JAM_Highlighted = true;

      dtp_datePuta.Leave += new EventHandler(DatePutaExitMethod);

                      hamper.CreateVvLabel        (3, 0, "Odredište:", ContentAlignment.MiddleRight);
      tbx_odrediste = hamper.CreateVvTextBoxLookUp(4, 0, "tbx_odrediste", "Odredište", GetDB_ColumnSize(DB_ci.strC_32));
      tbx_odrediste.JAM_Set_LookUpTable(ZXC.luiListaMixerOdrediste, (int)ZXC.Kolona.prva);
      tbx_odrediste.JAM_lookUp_NOTobligatory = true;

                   hamper.CreateVvLabel       (5, 0, "Država:", ContentAlignment.MiddleRight);
      tbx_drzava = hamper.CreateVvTextBoxLookUp("tbx_drzava",6 , 0,  "Drzava", 32, 2, 0);

                      hamper.CreateVvLabel  (9, 0, "Dnevnica:", 1, 0, ContentAlignment.MiddleRight);
      tbx_iznosDnev = hamper.CreateVvTextBox(11, 0, "tbx_iznosDnev", "Iznos dnevnice", GetDB_ColumnSize(DB_ci.moneyB));
      tbx_iznosDnev.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_iznosDnev.TextAlign = HorizontalAlignment.Left;

      tbx_ValName = hamper.CreateVvTextBox(12, 0,"tbx_ValName",  "Naziv devizne valute", GetDB_ColumnSize(DB_ci.devName));
      
      tbx_drzava.JAM_Set_LookUpTable(ZXC.luiListaMixDnevnice, (int)ZXC.Kolona.prva);
      tbx_drzava.JAM_lui_NameTaker_JAM_Name   = tbx_ValName  .JAM_Name;
      tbx_drzava.JAM_lui_NumberTaker_JAM_Name = tbx_iznosDnev.JAM_Name;


                    hamper.CreateVvLabel        (               0, 1, "Zadatak:", ContentAlignment.MiddleRight);
      tbx_zadatak = hamper.CreateVvTextBoxLookUp("tbx_zadatak", 1, 1, "Zadatak", GetDB_ColumnSize(DB_ci.strB_128), 3, 2);
      tbx_zadatak.Multiline = true;
      tbx_zadatak.ScrollBars = ScrollBars.Vertical;
      tbx_zadatak.JAM_Set_LookUpTable(ZXC.luiListaMixerZadatak, (int)ZXC.Kolona.druga);
      tbx_zadatak.JAM_lookUp_NOTobligatory = true;


                 hamper.CreateVvLabel  (5, 1, "Trajanje puta:", ContentAlignment.MiddleRight);
      tbx_dana = hamper.CreateVvTextBox(6, 1, "tbx_dana", "Trajanje puta", GetDB_ColumnSize(DB_ci.intA));

                       hamper.CreateVvLabel  (7, 1, "Troškovi terete:", 2, 0, ContentAlignment.MiddleRight);
      tbx_trskTerete = hamper.CreateVvTextBox(10, 1, "tbx_trskTerete", "Troškovi putovanja terete",  GetDB_ColumnSize(DB_ci.strD_32), 2, 0);

                     hamper.CreateVvLabel        (5, 2, "Vozilo:", ContentAlignment.MiddleRight);
      tbx_voziloCD = hamper.CreateVvTextBoxLookUp("tbx_voziloCD", 6, 2,  "Registracija vozila", GetDB_ColumnSize(DB_ci.strH_32), 2, 0);
      tbx_voziloCD.JAM_Set_LookUpTable(ZXC.luiListaMixerVozilo, (int)ZXC.Kolona.prva);
      tbx_voziloCD.JAM_lookUp_NOTobligatory = true;

      tbx_vozilo = hamper.CreateVvTextBox(9, 2, "tbx_vozilo", "Vozilo", GetDB_ColumnSize(DB_ci.strG_40), 2, 0);
      tbx_voziloCD.JAM_lui_NameTaker_JAM_Name = tbx_vozilo.JAM_Name;
      
      cbx_isPrivate = hamper.CreateVvCheckBox_OLD(12, 2, null, "Privatno", System.Windows.Forms.RightToLeft.Yes);
      cbx_isPrivate.Name = "cbx_isPrivate";
      tbx_voziloCD.JAM_lui_FlagTaker_JAM_Name = cbx_isPrivate.Name;


                        hamper.CreateVvLabel  (5, 3, "Projekt:", ContentAlignment.MiddleRight);
      //tbx_ProjektName = hamper.CreateVvTextBox(6, 3, "tbx_ProjektName", "Naziv", 32, 1, 0);
      //tbx_ProjektName.JAM_ReadOnly = true;
      tbx_ProjektCD = hamper.CreateVvTextBox(6, 3, "tbx_Projekt", "Projekt - ", GetDB_ColumnSize(DB_ci.projektCD), 2, 0);
      tbx_ProjektCD.JAM_SetAutoCompleteData(Faktur.recordName, Faktur.sorterTtNum.SortType, ZXC.AutoCompleteRestrictor.FAK_ExactTT_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Faktur_DUMMY), null);

      btn_proj = hamper.CreateVvButton(9, 3, new EventHandler(/*FakturDUC.*/GoToProjektCD_RISK_Dokument_Click), "");
      btn_proj.Name = "projekt";
      btn_proj.FlatStyle = FlatStyle.Flat;
      btn_proj.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
    //btn_proj.Image = new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue2.ico")), 16, 16).ToBitmap();
      btn_proj.Image = VvIco.TriangleBlue16.ToBitmap();
      btn_proj.Tag = 10;
      btn_proj.TabStop = false;

                hamper.CreateVvLabel  (11, 3, "Akontacija:", ContentAlignment.MiddleRight);
      tbx_acc = hamper.CreateVvTextBox(12, 3, "tbx_acc", "Akontacija",  GetDB_ColumnSize(DB_ci.moneyA));
      tbx_acc.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

   }

   public void InitializeHamper_Vozilo(out VvHamper hamper)
   {
      hamper = new VvHamper(13, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0        1                 2            3         4                5                   6               7            8                9                      10                 11                    12                  
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q2un, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un, ZXC.Q3un - ZXC.Qun4, ZXC.Q3un, ZXC.Q4un - ZXC.Qun4, ZXC.Q4un, ZXC.QUN - ZXC.Qun4 , ZXC.Q2un, ZXC.Q6un - ZXC.Qun2, ZXC.QUN - ZXC.Qun4, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4,                  0 , ZXC.Qun4, ZXC.Qun4,                  0,                  0 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

                     hamper.CreateVvLabel        (0, 0, "Vozilo:", ContentAlignment.MiddleRight);
      tbx_voziloCD = hamper.CreateVvTextBoxLookUp("tbx_voziloCD", 1, 0,  "Registracija vozila", GetDB_ColumnSize(DB_ci.strH_32), 1, 0);
      tbx_voziloCD.JAM_Set_LookUpTable(ZXC.luiListaMixerVozilo, (int)ZXC.Kolona.prva);
      tbx_voziloCD.JAM_lookUp_NOTobligatory = true;
      tbx_voziloCD.JAM_FieldExitMethod_2 = new EventHandler(OnExitVoziloCD_GetZadnjeStanjeBrojila);

      tbx_vozilo = hamper.CreateVvTextBox(3, 0, "tbx_vozilo", "Vozilo", GetDB_ColumnSize(DB_ci.strG_40), 1, 0);
      tbx_voziloCD.JAM_lui_NameTaker_JAM_Name = tbx_vozilo.JAM_Name;

      cbx_isPrivate = hamper.CreateVvCheckBox_OLD(5, 0, null, "Privatno", System.Windows.Forms.RightToLeft.Yes);
      cbx_isPrivate.Name = "cbx_isPrivate";
      tbx_voziloCD.JAM_lui_FlagTaker_JAM_Name = cbx_isPrivate.Name;

                      hamper.CreateVvLabel  (6, 0, "Projekt:", ContentAlignment.MiddleRight);
      tbx_ProjektCD = hamper.CreateVvTextBox(7, 0, "tbx_Projekt", "Projekt - ", GetDB_ColumnSize(DB_ci.projektCD));
      tbx_ProjektCD.JAM_SetAutoCompleteData(Faktur.recordName, Faktur.sorterTtNum.SortType, ZXC.AutoCompleteRestrictor.FAK_ExactTT_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Faktur_DUMMY), null);

      btn_proj = hamper.CreateVvButton(8, 0, new EventHandler(/*FakturDUC.*/GoToProjektCD_RISK_Dokument_Click), "");
      btn_proj.Name = "projekt";
      btn_proj.FlatStyle = FlatStyle.Flat;
      btn_proj.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_proj.Image = VvIco.TriangleBlue16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue2.ico")), 16, 16)*/.ToBitmap();
      btn_proj.Tag = 10;
      btn_proj.TabStop = false;


                       hamper.CreateVvLabel  ( 9, 0, "Link:", ContentAlignment.MiddleRight);
      tbx_externLink1 = hamper.CreateVvTextBox(10, 0, "tbx_externLink1", "Izvještaj sa puta - Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.strE_256));

      btn_goExLink1           = hamper.CreateVvButton(11, 0, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink1.Name      = "btn_goExLink1";
      btn_goExLink1.FlatStyle = FlatStyle.Flat;
      btn_goExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink1.Image     = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink1.Tag       = 1;
      btn_goExLink1.TabStop   = false;

      btn_goExLink1.Visible = false;
      
      btn_openExLink1           = hamper.CreateVvButton(12, 0, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLink1.Name      = "btn_openExLink1";
      btn_openExLink1.FlatStyle = FlatStyle.Flat;
      btn_openExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLink1.Image   = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLink1.Tag     = 1;
      btn_openExLink1.TabStop = false;

   }

   public void InitializeHamper_Zadatak(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
       
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q10un*3 +ZXC.Q3un - ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

                    hamper.CreateVvLabel        (               0, 0, "Zadatak:", ContentAlignment.MiddleRight);
      tbx_zadatak = hamper.CreateVvTextBoxLookUp("tbx_zadatak", 1, 0, "Zadatak", GetDB_ColumnSize(DB_ci.strB_128), 0, 1);
      tbx_zadatak.Multiline = true;
      tbx_zadatak.ScrollBars = ScrollBars.Vertical;
      tbx_zadatak.JAM_Set_LookUpTable(ZXC.luiListaMixerZadatak, (int)ZXC.Kolona.druga);
      tbx_zadatak.JAM_lookUp_NOTobligatory = true;


   }

   public void InitializeHamper_obrDnev(out VvHamper hamper)
   {
      hamper = new VvHamper(10, 2, "", TheTabControl.TabPages[0], false, ZXC.QunMrgn, nextY + ZXC.Qun2, razmakHamp);
      //                                     0         1          2                3           4         5               6               7           8            9
      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q4un, ZXC.Q5un, ZXC.Q10un + ZXC.QUN, ZXC.Qun8, ZXC.Q2un, ZXC.Q2un - ZXC.Qun4, ZXC.Q2un - ZXC.Qun4, ZXC.Q3un + ZXC.Qun4, ZXC.Q2un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4           , ZXC.Qun4,  ZXC.Qun8,            ZXC.Qun4,            ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN ,ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4,ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel  (0, 0, "Obračun dnevnica", 1, 0, ContentAlignment.MiddleLeft);
      lbl.Font = ZXC.vvFont.BaseBoldFont;

      hamper.CreateVvLabel(0, 1, "Datum:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 0, "Odlazak:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 1, "Povratak:", ContentAlignment.MiddleRight);

      tbx_dateObracuna = hamper.CreateVvTextBox(1, 1, "tbx_dateObracuna", "Datum obračuna", GetDB_ColumnSize(DB_ci.dateB));
      tbx_dateObracuna.JAM_IsForDateTimePicker = true;
      dtp_dateObracuna = hamper.CreateVvDateTimePicker(1, 1, "", tbx_dateObracuna);
      dtp_dateObracuna.Name = "dtp_dateObracuna";
      

      tbx_dateOd = hamper.CreateVvTextBox(3, 0, "tbx_dateOd", "Datum odlaska", GetDB_ColumnSize(DB_ci.dateTimeA));
      tbx_dateOd.JAM_IsForDateTimePicker = true;
      dtp_dateOd = hamper.CreateVvDateTimePicker(3, 0, "", tbx_dateOd);
      dtp_dateOd.Name = "dtp_dateOd";
      tbx_dateOd.JAM_IsForDateTimePicker_WithTimeDisplay = true;
     // tbx_dateOd.JAM_FieldExitMethod_2 = new EventHandler(DateDoCalcSati);
      dtp_dateOd.Leave+= new EventHandler(CalcSati_CalcNumOfDnev);

      tbx_dateDo = hamper.CreateVvTextBox(3, 1, "tbx_dateDo", "Datum povratka", GetDB_ColumnSize(DB_ci.dateTimeB));
      tbx_dateDo.JAM_IsForDateTimePicker = true;
      dtp_dateDo = hamper.CreateVvDateTimePicker(3, 1, "", tbx_dateDo);
      dtp_dateDo.Name = "dtp_dateDo";
      tbx_dateDo.JAM_IsForDateTimePicker_WithTimeDisplay = true;
    //  tbx_dateDo.JAM_FieldExitMethod_2 = new EventHandler(DateDoCalcSati);
      dtp_dateDo.Leave += new EventHandler(CalcSati_CalcNumOfDnev);


                   hamper.CreateVvLabel  (6, 1, "Sati:", ContentAlignment.MiddleRight);
      tbx_brSati = hamper.CreateVvTextBox(7, 1, "tbx_brSati", "Broj sati", GetDB_ColumnSize(DB_ci.intB));
      tbx_brSati.JAM_ReadOnly = true;
      tbx_brSati.JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, true);

                    hamper.CreateVvLabel  (8, 1, "Br. dnevnica:", ContentAlignment.MiddleRight);
      tbx_brDnevn = hamper.CreateVvTextBox(9, 1, "tbx_brDnevn", "Broj dnevnica", GetDB_ColumnSize(DB_ci.moneyC));
      tbx_brDnevn.JAM_MarkAsNumericTextBox(1, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_brDnevn.JAM_ReadOnly = true;


      hamper.BackColor = Color.AliceBlue;

   }

   public void InitializeHamper_kontoPrilog(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 1, "", TheTabControl.TabPages[0], false, ZXC.QunMrgn, nextY, razmakHamp);
       
      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q4un, ZXC.Q5un , ZXC.Q4un, ZXC.Q3un, ZXC.Q10un + ZXC.Q5un + ZXC.Qun4+ ZXC.Qun2};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,                 ZXC.Qun4, ZXC.Qun4,             ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                   hamper.CreateVvLabel  (0, 0, "KontoDnev:", ContentAlignment.MiddleRight);
      tbx_konto  = hamper.CreateVvTextBox(1, 0, "tbx_konto", "Konto dnevnica", GetDB_ColumnSize(DB_ci.konto));
      tbx_konto.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_konto.JAM_SetAutoCompleteData(Kplan.recordName, VvSQL.SorterType.KontoNaziv, ZXC.AutoCompleteRestrictor.KPL_Analitika_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_KplanNaziv_sorterCode), null);
      tbx_konto.JAM_FieldExitMethod = new EventHandler(OnExitKontoTbx_ClearPreffix);

                   hamper.CreateVvLabel  (2, 0, "KontoPrijTroškova:", ContentAlignment.MiddleRight);
      tbx_konto2 = hamper.CreateVvTextBox(3, 0, "tbx_konto2", "Konto prijevoznih troškova", GetDB_ColumnSize(DB_ci.konto2));
      tbx_konto2.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_konto2.JAM_SetAutoCompleteData(Kplan.recordName, VvSQL.SorterType.KontoNaziv, ZXC.AutoCompleteRestrictor.KPL_Analitika_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_KplanNaziv_sorterCode), null);
      tbx_konto2.JAM_FieldExitMethod = new EventHandler(OnExitKontoTbx_ClearPreffix2);

                   hamper.CreateVvLabel  (4, 0, "Prilog:", ContentAlignment.MiddleRight);
      tbx_prilog = hamper.CreateVvTextBox(5, 0, "tbx_prilog", "Prilog", GetDB_ColumnSize(DB_ci.strF_64));

      hamper.BackColor = Color.Lavender;

   }
   
   public void OnExitKontoTbx_ClearPreffix(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvTextBox vvTbKonto = sender as VvTextBox;

      string dirtyString = vvTbKonto.Text, cleanString;

      int spaceIdx = dirtyString.IndexOf(' ');

      if(dirtyString.Length.IsZero() || spaceIdx.IsNegative()) return;

      cleanString = dirtyString.Substring(0, spaceIdx);

      Fld_Konto_Dnev   = cleanString;
   }
   
   public void OnExitKontoTbx_ClearPreffix2(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvTextBox vvTbKonto = sender as VvTextBox;

      string dirtyString = vvTbKonto.Text, cleanString;

      int spaceIdx = dirtyString.IndexOf(' ');

      if(dirtyString.Length.IsZero() || spaceIdx.IsNegative()) return;

      cleanString = dirtyString.Substring(0, spaceIdx);

      Fld_Konto2_PrjTr = cleanString;
   }

   public void InitializeHamper_ObracunSumeAll(out VvHamper hamper)
   {
      hamper = new VvHamper(10, 1, "", ThePolyGridTabControl.TabPages[0], false);
      //                                               0         1                   2        3                4               5        6              7
      hamper.VvColWdt      = new int[] { ZXC.Q3un - ZXC.Qun2, ZXC.Q4un - ZXC.Qun4, ZXC.Q3un- ZXC.Qun2, ZXC.Q4un- ZXC.Qun4, ZXC.Q2un+ ZXC.Qun4, ZXC.Q4un- ZXC.Qun4, ZXC.Q3un, ZXC.Q4un- ZXC.Qun4, ZXC.Q4un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4           , ZXC.Qun4,           ZXC.Qun4, ZXC.Qun4,           ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      if(this is PutNalInoDUC || this is PutNalTuzDUC)
      {
         hamper.CreateVvLabel(0, 0, "Dnevnice:", ContentAlignment.MiddleRight);
         hamper.CreateVvLabel(6, 0, "Akontacija:", ContentAlignment.MiddleRight);
      }
      hamper.CreateVvLabel(2, 0, "Prijevoz:"      , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(4, 0, "Ostalo:"        , ContentAlignment.MiddleRight);

      string isplPov = (this is LokoVoznjaDUC) ? "Ukupno:" : "Isplata/Povrat:";
      hamper.CreateVvLabel(8, 0, isplPov, ContentAlignment.MiddleRight);

      tbx_ukDnevnice = hamper.CreateVvTextBox(1, 0, "tbx_ukDnevnice", "Ukupno dnevnice"            , 12);
      tbx_ukPrijevoz = hamper.CreateVvTextBox(3, 0, "tbx_ukPrijevoz", "Ukupno prijevozni troškovi" , 12);
      tbx_ukOsTr     = hamper.CreateVvTextBox(5, 0, "tbx_ukOsTr"    , "Ukupno ostali troškovi"     , 12);
      tbx_ukAcc      = hamper.CreateVvTextBox(7, 0, "tbx_ukAcc"     , "Akontacija"                 , 12);
      tbx_ukIsplPov  = hamper.CreateVvTextBox(9, 0, "tbx_ukIsplPov" , "Ukupno za isplatu/povrat"   , 12);


      tbx_ukDnevnice.JAM_ReadOnly = true; 
      tbx_ukPrijevoz.JAM_ReadOnly = true; 
      tbx_ukOsTr    .JAM_ReadOnly = true; 
      tbx_ukAcc     .JAM_ReadOnly = true; 
      tbx_ukIsplPov .JAM_ReadOnly = true; 
      
      tbx_ukDnevnice.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_ukPrijevoz.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_ukOsTr    .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_ukAcc     .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_ukIsplPov .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      if(this is LokoVoznjaDUC || this is PutRadListDUC)
      { 
         tbx_ukDnevnice.Visible = false; 
         tbx_ukAcc     .Visible = false; 
      }
   }
 
   protected override void CalcLocationHamperBeloWGrid()
   {
         hamp_sumeAll.Location = new Point(TheSumGrid.Right - hamp_sumeAll.Width + ZXC.Qun4, TheSumGrid.Bottom);
         hamp_sumeAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
   }

   public void AnyPersonTextBoxLeave(object sender, EventArgs e)
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
            Fld_PersonCd  = person_rec.PersonCD/*RecID*/;
            Fld_Prezime   = person_rec.Prezime;
            Fld_Ime       = person_rec.Ime ;
            Fld_RadMjesto = person_rec.RadMj;
            
            ((PutNalFilter)this.VirtualRptFilter).Spol = person_rec.Spol;
         }
         else
         {
            Fld_PersonCdAsTxt = Fld_Prezime = Fld_Ime = Fld_RadMjesto = "";
         }
      }
   }

   public void InitializeHamper_Veza12(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 3, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q2un, ZXC.Q3un - ZXC.Qun2, ZXC.Q5un, ZXC.Q3un - ZXC.Qun4, ZXC.QUN-ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4,         ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                            hamper.CreateVvLabel  (0, 0, "Veza1:", ContentAlignment.MiddleRight);
      tbx_v1_tt     = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_v1_tt"    , "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v1_tt));
      tbx_v1_ttOpis = hamper.CreateVvTextBox      (2, 0, "tbx_v1_ttOpis", "Veza na interni dokument", 32);
      tbx_v1_ttNum  = hamper.CreateVvTextBox      (3, 0, "tbx_v1_ttNum" , "Veza na interni dokument", 6/*GetDB_ColumnSize(DB_ci.iz_ttNum)*/);

      btn_v1TT = hamper.CreateVvButton(4, 0, new EventHandler(GoTo_MIXER_Dokument_Click), "");

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

                      hamper.CreateVvLabel        (0, 1, "Veza2:", ContentAlignment.MiddleRight);
      tbx_v2_tt     = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_v2_tt"    , "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v2_tt));
      tbx_v2_ttOpis = hamper.CreateVvTextBox      (2, 1, "tbx_v2_ttOpis", "Veza na interni dokument");
      tbx_v2_ttNum  = hamper.CreateVvTextBox      (3, 1, "tbx_v2_ttNum" , "Veza na interni dokument" + " broj", 6/*GetDB_ColumnSize(DB_ci.na_ttNum)*/);

      btn_v2TT = hamper.CreateVvButton(4, 1, new EventHandler(GoTo_MIXER_Dokument_Click), "");
      btn_v2TT.Name = "v2_TT";

      btn_v2TT.FlatStyle = FlatStyle.Flat;
      btn_v2TT.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_v2TT.Image = VvIco.TriangleBlue16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue2.ico")), 16, 16)*/.ToBitmap();
      btn_v2TT.Tag = 2;
      btn_v2TT.TabStop = false;

      tbx_v2_tt.JAM_CharacterCasing = CharacterCasing.Upper;
      //tbx_v2_tt.JAM_Set_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_v2_tt.JAM_Set_NOTobligatory_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_v2_tt.JAM_lui_NameTaker_JAM_Name = tbx_v2_ttOpis.JAM_Name;
      tbx_v2_ttOpis.JAM_ReadOnly = true;

      tbx_v2_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      //12.03.2015.
      VvLookUpLista.LoadResultLuiList_PozicijePlana_PLN_or_RLZ(/* isPLN */ false);

                        hamper.CreateVvLabel        (0, 2, "Pozicija:", ContentAlignment.MiddleRight);
      tbx_externLink2 = hamper.CreateVvTextBoxLookUp(1, 2, "tbx_externLink2", "Pozicija", GetDB_ColumnSize(DB_ci.externLink2));
      tbx_pozicijaOp  = hamper.CreateVvTextBox      (2, 2, "tbx_pozicijaOp" , "Opis pozicije", 256, 1, 0);
      tbx_externLink2.JAM_Set_LookUpTable(ZXC.luiListaPozicijePlanaRLZ, (int)ZXC.Kolona.prva);
      tbx_externLink2.JAM_lui_NameTaker_JAM_Name = tbx_pozicijaOp.JAM_Name;
      tbx_pozicijaOp.JAM_ReadOnly = true;



      hamper.Visible = true;
   }
  
   public void InitializeHamper_Veza12Link(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 11, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q2un, ZXC.Q3un, ZXC.Q6un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun4, ZXC.QUN-ZXC.Qun4, ZXC.QUN-ZXC.Qun4  };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4,            ZXC.Qun4,                0,                0  };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvSpcBefRow[8] += ZXC.Qun2;

      hamper.VvBottomMargin = hamper.VvTopMargin;

                            hamper.CreateVvLabel  (0, 0, "Veza1:", ContentAlignment.MiddleRight);
      tbx_v1_tt     = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_v1_tt"    , "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v1_tt));
      tbx_v1_ttOpis = hamper.CreateVvTextBox      (2, 0, "tbx_v1_ttOpis", "Veza na interni dokument", 32);
      tbx_v1_ttNum  = hamper.CreateVvTextBox      (3, 0, "tbx_v1_ttNum" , "Veza na interni dokument", 6/*GetDB_ColumnSize(DB_ci.iz_ttNum)*/);

      btn_v1TT = hamper.CreateVvButton(5, 0, new EventHandler(GoTo_MIXER_Dokument_Click), "");

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

                      hamper.CreateVvLabel        (0, 1, "Veza2:", ContentAlignment.MiddleRight);
      tbx_v2_tt     = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_v2_tt"    , "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v2_tt));
      tbx_v2_ttOpis = hamper.CreateVvTextBox      (2, 1, "tbx_v2_ttOpis", "Veza na interni dokument");
      tbx_v2_ttNum  = hamper.CreateVvTextBox      (3, 1, "tbx_v2_ttNum" , "Veza na interni dokument" + " broj", 6/*GetDB_ColumnSize(DB_ci.na_ttNum)*/);

      btn_v2TT = hamper.CreateVvButton(5, 1, new EventHandler(GoTo_MIXER_Dokument_Click), "");
      btn_v2TT.Name = "v2_TT";

      btn_v2TT.FlatStyle = FlatStyle.Flat;
      btn_v2TT.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_v2TT.Image = VvIco.TriangleBlue16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue2.ico")), 16, 16)*/.ToBitmap();
      btn_v2TT.Tag = 2;
      btn_v2TT.TabStop = false;

      tbx_v2_tt.JAM_CharacterCasing = CharacterCasing.Upper;
      //tbx_v2_tt.JAM_Set_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_v2_tt.JAM_Set_NOTobligatory_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_v2_tt.JAM_lui_NameTaker_JAM_Name = tbx_v2_ttOpis.JAM_Name;
      tbx_v2_ttOpis.JAM_ReadOnly = true;

      tbx_v2_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

                        hamper.CreateVvLabel  (0, 2, "Link:", ContentAlignment.MiddleRight);
      tbx_externLink1 = hamper.CreateVvTextBox(1, 2, "tbx_externLink1", "Izvještaj sa puta - Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.externLink1), 2, 0);

      btn_goExLink1           = hamper.CreateVvButton(4, 2, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink1.Name      = "btn_goExLink1";
      btn_goExLink1.FlatStyle = FlatStyle.Flat;
      btn_goExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink1.Image     = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink1.Tag       = 1;
      btn_goExLink1.TabStop   = false;
      btn_goExLink1.Visible   = false;
      
      btn_openExLink1           = hamper.CreateVvButton(5, 2, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLink1.Name      = "btn_openExLink1";
      btn_openExLink1.FlatStyle = FlatStyle.Flat;
      btn_openExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLink1.Image   = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLink1.Tag     = 1;
      btn_openExLink1.TabStop = false;

      //12.03.2015.
      VvLookUpLista.LoadResultLuiList_PozicijePlana_PLN_or_RLZ(/* isPLN */ false);

                        hamper.CreateVvLabel        (0, 3, "Pozicija:", ContentAlignment.MiddleRight);
      tbx_externLink2 = hamper.CreateVvTextBoxLookUp(1, 3, "tbx_externLink2", "Pozicija", GetDB_ColumnSize(DB_ci.externLink2));
      tbx_pozicijaOp  = hamper.CreateVvTextBox      (2, 3, "tbx_pozicijaOp" , "Opis pozicije", 256, 1, 0);
      tbx_externLink2.JAM_Set_LookUpTable(ZXC.luiListaPozicijePlanaRLZ, (int)ZXC.Kolona.prva);
      tbx_externLink2.JAM_lui_NameTaker_JAM_Name = tbx_pozicijaOp.JAM_Name;
      tbx_pozicijaOp.JAM_ReadOnly = true;

                     hamper.CreateVvLabel  (0, 4, "Izvještaj:", ContentAlignment.MiddleLeft);
      tbx_strE_256 = hamper.CreateVvTextBox(0, 5,"tbx_strE_256", "Izvještaj sa puta dodatak", GetDB_ColumnSize(DB_ci.strE_256), 5, 5);
      tbx_strE_256.Multiline = true;
      tbx_strE_256.ScrollBars = ScrollBars.Vertical;

      hamper.Visible = true;
   }

   #endregion CreateSpecificHampers()
   
   #region Calc
  
   public void CalcSati_CalcNumOfDnev(object sender, EventArgs e)
   {
      TimeSpan ts;
      int brSati;
      
      DateTime dtpMinDate = DateTimePicker.MinimumDateTime;

      if(Fld_DateDo != dtpMinDate && Fld_DateOd != dtpMinDate)
      {
         ts     = Fld_DateDo.Subtract(Fld_DateOd);
         brSati = System.Convert.ToInt32(ts.TotalHours);
         
         Fld_BrSati = brSati;
         Fld_BrDnev = CalcDnevnice(brSati);
         Fld_UkDnev = Fld_IznosDnev * Fld_BrDnev;
      }
   }

   private decimal CalcDnevnice(int brSati)
   {
      decimal brojDnevnica;
      decimal ostatakDo24   = brSati % 24;
      decimal cisteDnevnice = (brSati - ostatakDo24)/24m;
      //ako je od 8 do 12 onda je pola a 24 je cijela

      //31.05.2022. ako je 12 sati onda je top već 1 cijela dnevnica pa se kao takva pribraja kada je = 12
           if(     ostatakDo24.IsZero()                )  brojDnevnica = cisteDnevnice;
      else if(8 <= ostatakDo24 && ostatakDo24 </*=*/ 12)  brojDnevnica = cisteDnevnice + 0.5m;
      else if(     ostatakDo24 >= 12                   )  brojDnevnica = cisteDnevnice + 1.0m;
      else                                                brojDnevnica = cisteDnevnice;

      return brojDnevnica;
   }

   #endregion Calc
  
   #region Fld_

   public uint     Fld_PersonCd      { get { return ZXC.ValOrZero_UInt(tbx_PersonCd.Text) ; } set { tbx_PersonCd .Text = value.ToString("0000"); } }
   public string   Fld_PersonCdAsTxt { get { return tbx_PersonCd .Text                    ; } set { tbx_PersonCd .Text = value                 ; } }
   public string   Fld_Prezime       { get { return tbx_prezime  .Text                    ; } set { tbx_prezime  .Text = value                 ; } }
   public string   Fld_Ime           { get { return tbx_ime      .Text                    ; } set { tbx_ime      .Text = value                 ; } }
   public string   Fld_RadMjesto     { get { return tbx_radMjesto.Text                    ; } set { tbx_radMjesto.Text = value                 ; } }
   public DateTime Fld_DatePuta      
   {
      get { return dtp_datePuta.Value; }
      set
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dtp_datePuta.Value = value;
         }
      }
   }
   public DateTime Fld_DateOd        
   {
      get { return dtp_dateOd.Value; }
      set
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dtp_dateOd.Value = value;
         }
      }
   }
   public DateTime Fld_DateDo        
   {
      get { return dtp_dateDo.Value; }
      set
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dtp_dateDo.Value = value;
         }
      }
   }
   public DateTime Fld_DateObracuna  
   {
      get { return dtp_dateObracuna.Value; }
      set
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dtp_dateObracuna.Value = value;
         }
      }
   }
   
   public string   Fld_Odrediste    { get { return tbx_odrediste .Text; }  set { tbx_odrediste  .Text = value; } } 
   public string   Fld_Zadatak      { get { return tbx_zadatak   .Text; }  set { tbx_zadatak    .Text = value; } } 
   public string   Fld_Vozilo       { get { return tbx_vozilo    .Text; }  set { tbx_vozilo     .Text = value; } } 
   public string   Fld_VoziloCD     { get { return tbx_voziloCD  .Text; }  set { tbx_voziloCD   .Text = value; } } 
   public string   Fld_TrskTeret    { get { return tbx_trskTerete.Text; }  set { tbx_trskTerete .Text = value; } } 
   public string   Fld_Prilog       { get { return tbx_prilog    .Text; }  set { tbx_prilog     .Text = value; } } 
   public string   Fld_DevName      { get { return tbx_ValName   .Text; }  set { tbx_ValName    .Text = value; } }
   public string   Fld_StrE_256_Izvj{ get { return tbx_strE_256  .Text; }  set { tbx_strE_256   .Text = value; } }
   public string   Fld_Konto_Dnev   { get { return tbx_konto     .Text; }  set { tbx_konto      .Text = value; } }
   public string   Fld_Konto2_PrjTr { get { return tbx_konto2    .Text; }  set { tbx_konto2     .Text = value; } }


   public int      Fld_Dana         { get { return tbx_dana      .GetIntField();     } set { tbx_dana      .PutIntField    (value); } }
   public int      Fld_BrSati       { get { return tbx_brSati    .GetIntField();     } set { tbx_brSati    .PutIntField    (value); } }
   public decimal  Fld_Acc          { get { return tbx_acc       .GetDecimalField(); } set { tbx_acc       .PutDecimalField(value); } }
   public decimal  Fld_BrDnev       { get { return tbx_brDnevn   .GetDecimalField(); } set { tbx_brDnevn   .PutDecimalField(value); } }
   public decimal  Fld_IznosDnev    { get { return tbx_iznosDnev .GetDecimalField(); } set { tbx_iznosDnev .PutDecimalField(value); } }
   public decimal  Fld_UkDnev       { get { return tbx_ukDnevnice.GetDecimalField(); } set { tbx_ukDnevnice.PutDecimalField(value); } }
   public decimal  Fld_UkPijevoz    { get { return tbx_ukPrijevoz.GetDecimalField(); } set { tbx_ukPrijevoz.PutDecimalField(value); } }
   public decimal  Fld_UkOstTr      { get { return tbx_ukOsTr    .GetDecimalField(); } set { tbx_ukOsTr    .PutDecimalField(value); } }
   public decimal  Fld_UkAcc        { get { return tbx_ukAcc     .GetDecimalField(); } set { tbx_ukAcc     .PutDecimalField(value); } }
   public decimal  Fld_UkIsplPov    { get { return tbx_ukIsplPov .GetDecimalField(); } set { tbx_ukIsplPov .PutDecimalField(value); } }
                                    
   public bool     Fld_IsPrivate    { get { return cbx_isPrivate.Checked; }            set { cbx_isPrivate .Checked       = value ; } }
   public string   Fld_Drzava       { get { return tbx_drzava   .Text;    }            set { tbx_drzava    .Text          = value ; } }
   public string   Fld_PozicijaOpis {                                                  set { tbx_pozicijaOp.Text          = value ; } }

   #endregion _Fld

   #region override PutSpecificsFld() GetSpecificsFld()
   
   protected override void PutDefaultBabyDUCfields()
   {
      if(CtrlOK(tbx_trskTerete)) Fld_TrskTeret = "Poslodavca";
    
    //if(CtrlOK(tbx_iznosDnev) && this is PutNalTuzDUC) Fld_IznosDnev = 170.00m; od 01.09.2019. je 200kn
    //if(CtrlOK(tbx_iznosDnev) && this is PutNalTuzDUC) Fld_IznosDnev = 200.00M;
      if(CtrlOK(tbx_iznosDnev) && this is PutNalTuzDUC) Fld_IznosDnev = ZXC.projectYearAsInt <= 2022 ? 200.00M : 26.55M; //euro dnevnica 25.01.2023.?!
   }

   /*protected*/public override void PutSpecificsFld()
   {
     if(CtrlOK(tbx_PersonCd    )) Fld_PersonCd        = mixer_rec.PersonCD    ;
     if(CtrlOK(tbx_prezime     )) Fld_Prezime         = mixer_rec.PersonPrezim;
     if(CtrlOK(tbx_ime         )) Fld_Ime             = mixer_rec.PersonIme   ;
     if(CtrlOK(tbx_datePuta    )) Fld_DatePuta        = mixer_rec.DateA       ;
     if(CtrlOK(tbx_dateOd      )) Fld_DateOd          = mixer_rec.DateTimeA   ;
     if(CtrlOK(tbx_dateDo      )) Fld_DateDo          = mixer_rec.DateTimeB   ;
     if(CtrlOK(tbx_dateObracuna)) Fld_DateObracuna    = mixer_rec.DateB       ;
     if(CtrlOK(tbx_odrediste   )) Fld_Odrediste       = mixer_rec.StrC_32     ;
     if(CtrlOK(tbx_zadatak     )) Fld_Zadatak         = mixer_rec.StrB_128    ;
     if(CtrlOK(tbx_voziloCD    )) Fld_VoziloCD        = mixer_rec.StrH_32     ;
     if(CtrlOK(tbx_vozilo      )) Fld_Vozilo          = mixer_rec.StrG_40     ;
     if(CtrlOK(tbx_trskTerete  )) Fld_TrskTeret       = mixer_rec.StrD_32     ;
     if(CtrlOK(tbx_prilog      )) Fld_Prilog          = mixer_rec.StrF_64     ;
     if(CtrlOK(tbx_externLink1 )) Fld_ExternLink1     = mixer_rec.ExternLink1 ;
     if(CtrlOK(tbx_ValName     )) Fld_DevName         = mixer_rec.DevName     ;
     if(CtrlOK(tbx_dana        )) Fld_Dana            = mixer_rec.IntA        ;
     if(CtrlOK(tbx_acc         )) Fld_Acc             = mixer_rec.MoneyA      ;
     if(CtrlOK(tbx_brSati      )) Fld_BrSati          = mixer_rec.IntB        ;
     if(CtrlOK(tbx_brDnevn     )) Fld_BrDnev          = mixer_rec.MoneyC      ;
     if(CtrlOK(tbx_iznosDnev   )) Fld_IznosDnev       = mixer_rec.MoneyB      ;
     if(CtrlOK(tbx_strE_256    )) Fld_StrE_256_Izvj   = mixer_rec.StrE_256    ;
                                  Fld_IsPrivate       = mixer_rec.IsXxx       ;
     if(CtrlOK(tbx_konto       )) Fld_Konto_Dnev      = mixer_rec.Konto       ;
     if(CtrlOK(tbx_konto2      )) Fld_Konto2_PrjTr    = mixer_rec.Konto2      ;
     if(CtrlOK(tbx_radMjesto   )) Fld_RadMjesto       = mixer_rec.StrA_40     ;
     if(CtrlOK(tbx_ukDnevnice  )) Fld_UkDnev          = mixer_rec.R_moneyBxC;
     if(CtrlOK(tbx_ukPrijevoz  )) Fld_UkPijevoz       = mixer_rec.Sum_KolMoneyA;
     if(CtrlOK(tbx_ukOsTr      )) Fld_UkOstTr         = mixer_rec.Sum2_MoneyA;
     if(CtrlOK(tbx_ukAcc       )) Fld_UkAcc           = mixer_rec.MoneyA;
     if(CtrlOK(tbx_ukIsplPov   )) Fld_UkIsplPov       = mixer_rec.R_PutNalToPay;
                                  Fld_TtOpis          = ZXC.luiListaMixTypePutNal.GetNameForThisCd(mixer_rec.TT);
     if(CtrlOK(tbx_ProjektCD))    Fld_ProjektCD       = mixer_rec.ProjektCD;

     if(CtrlOK(tbx_v1_tt       )) Fld_V1_tt           = mixer_rec.V1_tt;
     if(CtrlOK(tbx_v1_ttOpis   )) Fld_V1_ttOpis       = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V1_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
     if(CtrlOK(tbx_v1_ttNum    )) Fld_V1_ttNum        = mixer_rec.V1_ttNum;
     if(CtrlOK(tbx_v2_tt       )) Fld_V2_tt           = mixer_rec.V2_tt;
     if(CtrlOK(tbx_v2_ttOpis   )) Fld_V2_ttOpis       = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V2_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
     if(CtrlOK(tbx_v2_ttNum    )) Fld_V2_ttNum        = mixer_rec.V2_ttNum;

     if(CtrlOK(tbx_drzava      )) Fld_Drzava          = mixer_rec.KupdobName;

     if(CtrlOK(tbx_externLink2))  Fld_ExternLink2     = mixer_rec.ExternLink2;
     if(CtrlOK(tbx_externLink2))  Fld_PozicijaOpis    = ZXC.luiListaPozicijePlanaRLZ.GetNameForThisCd(mixer_rec.ExternLink2);

   }

   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_PersonCd     ))  mixer_rec.PersonCD     = Fld_PersonCd       ;
      if(CtrlOK(tbx_prezime      ))  mixer_rec.PersonPrezim = Fld_Prezime        ;
      if(CtrlOK(tbx_ime          ))  mixer_rec.PersonIme    = Fld_Ime            ;
      if(CtrlOK(tbx_datePuta     ))  mixer_rec.DateA        = Fld_DatePuta       ;
      if(CtrlOK(tbx_dateOd       ))  mixer_rec.DateTimeA    = Fld_DateOd         ;
      if(CtrlOK(tbx_dateDo       ))  mixer_rec.DateTimeB    = Fld_DateDo         ;
      if(CtrlOK(tbx_dateObracuna ))  mixer_rec.DateB        = Fld_DateObracuna   ;
      if(CtrlOK(tbx_odrediste    ))  mixer_rec.StrC_32      = Fld_Odrediste      ;
      if(CtrlOK(tbx_zadatak      ))  mixer_rec.StrB_128     = Fld_Zadatak        ;
      if(CtrlOK(tbx_voziloCD     ))  mixer_rec.StrH_32      = Fld_VoziloCD       ;
      if(CtrlOK(tbx_vozilo       ))  mixer_rec.StrG_40      = Fld_Vozilo         ;
      if(CtrlOK(tbx_trskTerete   ))  mixer_rec.StrD_32      = Fld_TrskTeret      ;
      if(CtrlOK(tbx_prilog       ))  mixer_rec.StrF_64      = Fld_Prilog         ;
      if(CtrlOK(tbx_externLink1  ))  mixer_rec.ExternLink1  = Fld_ExternLink1    ;
      if(CtrlOK(tbx_ValName      ))  mixer_rec.DevName      = Fld_DevName        ;
      if(CtrlOK(tbx_dana         ))  mixer_rec.IntA         = Fld_Dana           ;
      if(CtrlOK(tbx_acc          ))  mixer_rec.MoneyA       = Fld_Acc            ;
      if(CtrlOK(tbx_brSati       ))  mixer_rec.IntB         = Fld_BrSati         ;
      if(CtrlOK(tbx_brDnevn      ))  mixer_rec.MoneyC       = Fld_BrDnev         ;
      if(CtrlOK(tbx_iznosDnev    ))  mixer_rec.MoneyB       = Fld_IznosDnev      ;
      if(CtrlOK(tbx_strE_256     ))  mixer_rec.StrE_256     = Fld_StrE_256_Izvj  ;
                                     mixer_rec.IsXxx        = Fld_IsPrivate      ;
      if(CtrlOK(tbx_konto        ))  mixer_rec.Konto        = Fld_Konto_Dnev     ;
      if(CtrlOK(tbx_konto2       ))  mixer_rec.Konto2       = Fld_Konto2_PrjTr   ;
      if(CtrlOK(tbx_radMjesto    ))  mixer_rec.StrA_40      = Fld_RadMjesto      ;
      if(CtrlOK(tbx_ProjektCD    ))  mixer_rec.ProjektCD    = Fld_ProjektCD      ;

      if(CtrlOK(tbx_v1_tt        ))  mixer_rec.V1_tt        = Fld_V1_tt      ;
      if(CtrlOK(tbx_v1_ttNum     ))  mixer_rec.V1_ttNum     = Fld_V1_ttNum   ;
      if(CtrlOK(tbx_v2_tt        ))  mixer_rec.V2_tt        = Fld_V2_tt      ;
      if(CtrlOK(tbx_v2_ttNum     ))  mixer_rec.V2_ttNum     = Fld_V2_ttNum   ;

      if(CtrlOK(tbx_drzava       ))  mixer_rec.KupdobName   = Fld_Drzava ;
      if(CtrlOK(tbx_externLink2  ))  mixer_rec.ExternLink2  = Fld_ExternLink2;

   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region OnExitIntAIntBKolKm_CalcOthers DatePutaExitMethod

   public void OnExitIntAIntBKolKm_CalcOthers(object sender, EventArgs e)
   {
      VvTextBoxEditingControl vvTbx = sender as VvTextBoxEditingControl;

      int rIdx, cIdx = TheG.CurrentCell.ColumnIndex;

      if(vvTbx == null) // dosli smo iz popunjavanja lookup liste (MixerDUC.OnExit_Relacija_RaiseKmChanged())
      {
         rIdx = (int)sender;
      }
      else
      {
         rIdx = vvTbx.EditingControlDataGridView.CurrentRow.Index;
      }

      decimal km    =        /*dgvXtrans_rec.T_kol*/ TheG.GetDecimalCell(ci.iT_kol, rIdx, false);
      decimal start = (decimal)dgvXtrans_rec.T_intA;
      decimal end   = (decimal)dgvXtrans_rec.T_intB;

      //if     (km.IsZero()  && start.NotZero() && end.NotZero() && end > start)                                                 km    = end - start;
      //else if(km.NotZero() && start.IsZero()  && end.NotZero()               )                                                 start = end - km   ;
      //else if(km.NotZero() && start.NotZero() && end.IsZero()                )                                                 end   = km  + start;
      //else if(km.NotZero() && start.NotZero() && end.NotZero() && (vvTbx.Name.EndsWith("intA") || vvTbx.Name.EndsWith("kol"))) end   = km  + start;
      //else if(km.NotZero() && start.NotZero() && end.NotZero() &&  vvTbx.Name.EndsWith("intB"))                                km    = end - start;

      int prevEnd = 0;

      if(TheG.CI_OK(ci.iT_intB) && rIdx.IsPositive())
      {
         prevEnd = TheG.GetIntCell(ci.iT_intB, rIdx-1, false);
      }


      if(cIdx == ci.iT_intA) // upravo smo popunili polje START 
      {
         if(     km .NotZero()) end = km  + start;
         else if(end.NotZero()) km  = end - start;
      }
      else if(cIdx == ci.iT_intB) // upravo smo popunili polje END 
      {
         if(prevEnd.NotZero())
         {
            TheG.PutCell(ci.iT_intA, rIdx, (int)prevEnd);
            start = prevEnd;
         }

         if(     km   .NotZero()) start = end - km;
         else if(start.NotZero()) km    = end - start;
      }
      else if(cIdx == ci.iT_kol || cIdx == ci.iT_opis_128) // upravo smo popunili polje Kol/Km ili Relaciju
      {
         if(prevEnd.NotZero())
         {
            TheG.PutCell(ci.iT_intA, rIdx, (int)prevEnd);
            start = prevEnd;
         }

         if(   start.NotZero()) end   = start + km;
         else if(end.NotZero()) start = end   - km;
      }

      TheG.PutCell(ci.iT_kol , rIdx,      km   );
      TheG.PutCell(ci.iT_intA, rIdx, (int)start);
      TheG.PutCell(ci.iT_intB, rIdx, (int)end  );

   }

   private void OnExitVoziloCD_GetZadnjeStanjeBrojila(object sender, EventArgs e)
   {
      VvTextBox vvtb = sender as VvTextBox;

      if(vvtb.EditedHasChanges() == false) return;

      if(Fld_IsPrivate)
      {
      }
      else
      {
         decimal? zadnjeStanjeBrojila = XtransDao.GetZadnjeStanjeBrojila(TheDbConnection, Fld_VoziloCD, false);

         if(zadnjeStanjeBrojila == null) zadnjeStanjeBrojila = 0.00M;

         TheG.PutCell(ci.iT_intA, 0, (int)zadnjeStanjeBrojila);
              
      }
   }

   public void DatePutaExitMethod(object sender, EventArgs e)
   {
      if(Fld_DatePuta > DateTimePicker.MinimumDateTime && Fld_DatePuta < ZXC.Date01092019)
      {
         Fld_IznosDnev = 170.00M;
      }
      else
      {
         Fld_IznosDnev = 200.00M;
      }
   }

   #endregion OnExitIntAIntBKolKm_CalcOthers DatePutaExitMethod

   #region PrintDocumentRecord

   public PutNalFilterUC ThePutNalFilterUC { get; set; }
   public PutNalFilter   ThePutNalFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.ThePutNalFilter = new PutNalFilter(this);

      ThePutNalFilterUC        = new PutNalFilterUC(this);
      ThePutNalFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = ThePutNalFilterUC.Width;

   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      PutNalFilter mixerFilter = (PutNalFilter)vvRptFilter;

      switch(mixerFilter.PrintPutNal)
      {
         case PutNalFilter.PrintPutNalEnum.PutNalPutObrT  : specificMixerReport = new RptX_PutNal(new Vektor.Reports.XIZ.CR_PNT_NalogObracP()   , reportName, mixerFilter); break;
         case PutNalFilter.PrintPutNalEnum.PutNalPutObrI  : specificMixerReport = new RptX_PutNal(new Vektor.Reports.XIZ.CR_PNT_NalogObracP()   , reportName, mixerFilter); break;
         case PutNalFilter.PrintPutNalEnum.PutNalT        : specificMixerReport = new RptX_PutNal(new Vektor.Reports.XIZ.CR_PNT_NalogObracP()   , reportName, mixerFilter); break;
         case PutNalFilter.PrintPutNalEnum.PutNalI        : specificMixerReport = new RptX_PutNal(new Vektor.Reports.XIZ.CR_PNT_NalogObracP()   , reportName, mixerFilter); break;
         case PutNalFilter.PrintPutNalEnum.PutObrT        : specificMixerReport = new RptX_PutNal(new Vektor.Reports.XIZ.CR_PNT_NalogObracP()   , reportName, mixerFilter); break;
         case PutNalFilter.PrintPutNalEnum.PutObrI        : specificMixerReport = new RptX_PutNal(new Vektor.Reports.XIZ.CR_PNT_NalogObracP()   , reportName, mixerFilter); break;
         case PutNalFilter.PrintPutNalEnum.PutLoko        : specificMixerReport = new RptX_PutNal(new Vektor.Reports.XIZ.CR_LokoVoznja()        , reportName, mixerFilter); break;
         case PutNalFilter.PrintPutNalEnum.PutRadListNalog: specificMixerReport = new RptX_PutNal(new Vektor.Reports.XIZ.CR_PNR_PutniRadniList(), reportName, mixerFilter); break;
         case PutNalFilter.PrintPutNalEnum.PutRadListObr  : specificMixerReport = new RptX_PutNal(new Vektor.Reports.XIZ.CR_PNR_PutniRadniList(), reportName, mixerFilter); break;
         case PutNalFilter.PrintPutNalEnum.IzvjAll        : specificMixerReport = new RptX_PutNal(new Vektor.Reports.XIZ.CR_IzvjSaSluzbenPuta() , reportName, mixerFilter); break;
         case PutNalFilter.PrintPutNalEnum.IzvjOnly       : specificMixerReport = new RptX_PutNal(new Vektor.Reports.XIZ.CR_IzvjSaSluzbenPuta() , reportName, mixerFilter); break;
         
         default: ZXC.aim_emsg("{0}\nPrintSomePutNalDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintPutNal); return null;
      }

      return specificMixerReport;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.ThePutNalFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.ThePutNalFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
   }


   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_PutNal, Color.Empty, clr_PutNal2);
   }

   #endregion Colors

   #region overrideTabPageTitle

   public override string TabPageTitle1
   {
      get { return "Obračun prijevoznih troškova"; }
   }
   //public override string TabPageTitle2
   //{
   //   get { return "Obračun ostalih troškova"; }
   //}
   
   #endregion overrideTabPageTitle

   #region Ciribu - Ciriba

   private struct LokoVoznja
   {
      public DateTime date      ;
      public string   relacija  ;
      public string   zadatak   ;
      public decimal  kilometara;

      public LokoVoznja(DateTime _date, string _relacija, decimal _kilometara, string _zadatak)
      {
         this.date       = _date      ;
         this.relacija   = _relacija  ;
         this.zadatak    = _zadatak   ;
         this.kilometara = _kilometara;
      }

      public override string ToString()
      {
         return date + "/" + relacija + "/" + kilometara + "km/" + zadatak;
      }
   }

 //internal void Perform_CiribuCiriba(DateTime _dateAndTimeOd, DateTime _dateDo, decimal _kilometri, string _voziloCD, uint _personCD)
   internal void Perform_CiribuCiriba(CiribuCiribaUC theCbuCbaUC)
   {
      #region local variablez, initializations, ...

      SetSifrarAndAutocomplete<Person>(null, VvSQL.SorterType.Code);
      Person person_rec = VvUserControl.PersonSifrar.SingleOrDefault(person => person.PersonCD == theCbuCbaUC.Fld_PersonCd);

      decimal kilometaraAchieved = 0M;
    //decimal? zadnjeStanjeBrojila = XtransDao.GetZadnjeStanjeBrojila(conn, theUC.Fld_VoziloCD, false);
      decimal  zadnjeStanjeBrojila = theCbuCbaUC.Fld_PrethBrojilo; 

      List<LokoVoznja>   lokoVoznjeLista  = new List<LokoVoznja>();
      List<DateTime>     workingDaysList  = ZXC.WorkingDaysArray.Where(wda => wda >= theCbuCbaUC.Fld_DatumOd && wda <= theCbuCbaUC.Fld_DatumDo).ToList();
      List<VvLookUpItem> relacijeLuiLista = ZXC.luiListaMixerRelacija.Where(lui => lui.Flag == false).ToList();
      List<VvLookUpItem> zadaciLuiLista   = ZXC.luiListaMixerZadatak .Where(lui => lui.Flag == false).ToList();

      VvLookUpItem theVozilo = ZXC.luiListaMixerVozilo.SingleOrDefault(lui => lui.Cd == theCbuCbaUC.Fld_VoziloCD);
      VvLookUpItem theRelacija;
      VvLookUpItem theZadatak;
      LokoVoznja   theLokoVoznja;
      DateTime     theDate;
      int          theDayIndex, ctr=0;
      Random rnd = new Random(unchecked((int)(DateTime.Now.Ticks >> ++ctr)));

      decimal _kilometri;

      if(theCbuCbaUC.Fld_IsBrojilo) _kilometri = theCbuCbaUC.Fld_Km - (decimal)zadnjeStanjeBrojila;
      else                          _kilometri = theCbuCbaUC.Fld_Km;

      #endregion local variablez, initializations, ...

      #region while(_kilometri > kilometaraAchieved) and if(kilometaraAchieved > _kilometri)

      while(_kilometri > kilometaraAchieved)
      {
         if(workingDaysList.Count.IsZero()) // ispraznili smo set radnih dana, idemo ga ponovo napuniti pa bude vise loko voznji u jednom danu... 
         {
            workingDaysList = ZXC.WorkingDaysArray.Where(wda => wda >= theCbuCbaUC.Fld_DatumOd && wda <= theCbuCbaUC.Fld_DatumDo).ToList();

            rnd = new Random(unchecked((int)(DateTime.Now.Ticks >> ++ctr)));
         }

         theDate     = workingDaysList[theDayIndex = rnd.Next(workingDaysList.Count)];
         theRelacija = relacijeLuiLista.GetRandomItem(rnd);
         theZadatak  = zadaciLuiLista  .GetRandomItem(rnd);

         kilometaraAchieved += theRelacija.Number;

         theLokoVoznja = new LokoVoznja(theDate, theRelacija.Name, theRelacija.Number, theZadatak.Name);

         workingDaysList.RemoveAt(theDayIndex);

         lokoVoznjeLista.Add(theLokoVoznja);
      }

      LokoVoznja[] lokoVoznjeArray = lokoVoznjeLista.OrderBy(lv => lv.date).ToArray();

      if(kilometaraAchieved > _kilometri)
      {
         decimal diff = kilometaraAchieved - _kilometri;
         decimal koef = 1 - ZXC.DivSafe(diff, kilometaraAchieved);

         for(int i = 0; i < lokoVoznjeArray.Length; ++i)
         {
            lokoVoznjeArray[i].kilometara = (int)(lokoVoznjeArray[i].kilometara * koef);
         }

         decimal finalDiff = lokoVoznjeArray.Sum(lv => lv.kilometara) - _kilometri;

         if(finalDiff.NotZero())
         {
            lokoVoznjeArray[lokoVoznjeArray.Length-1].kilometara -= finalDiff;
         }

         finalDiff = lokoVoznjeArray.Sum(lv => lv.kilometara) - _kilometri;

         if(finalDiff.NotZero()) ZXC.aim_emsg("Nisam uspio finalDiff svesti na nulu. Trazena i postignuta kilometraza su NEJEDNAKE!");
      }

      #endregion while(_kilometri > kilometaraAchieved) and if(kilometaraAchieved > _kilometri)

      #region AutoAddMixer

      Mixer  mixer_rec = new Mixer();
      Xtrans xtrans_rec;
      ushort line = 0;

      #region Set mixer_rec

      mixer_rec.DokDate      = theCbuCbaUC.Fld_DatumDo;
      mixer_rec.TT           = Mixer.TT_PUTN_L;
      mixer_rec.PersonCD     = person_rec.PersonCD;
      mixer_rec.PersonIme    = person_rec.Ime;
      mixer_rec.PersonPrezim = person_rec.Prezime;
      mixer_rec.StrA_40      = person_rec.RadMj; // Person radMjesto 
      mixer_rec.Napomena     = theCbuCbaUC.Fld_Napomena;
      mixer_rec.MtrosCD      = theCbuCbaUC.Fld_MtrosCD;
      mixer_rec.MtrosTK      = theCbuCbaUC.Fld_MtrosTK;
      mixer_rec.IsXxx        = theCbuCbaUC.Fld_IsPrivate; // Is privat/osob. voz 
      mixer_rec.StrG_40      = theCbuCbaUC.Fld_Vozilo   ; // Vozilo-naziv        
      mixer_rec.StrH_32      = theCbuCbaUC.Fld_VoziloCD ; // VoziloCD            

      #endregion Set mixer_rec

      foreach(LokoVoznja lokoVoznja in lokoVoznjeArray)
      {
         #region Set rptXtrans_rec

         xtrans_rec = new Xtrans();

         xtrans_rec.T_dateOd       = lokoVoznja.date;
         xtrans_rec.T_opis_128     = lokoVoznja.relacija;
         xtrans_rec.T_kol          = lokoVoznja.kilometara;
         xtrans_rec.T_vezniDokA_64 = lokoVoznja.zadatak; // Zadatak Loko Voznje 

         //xtrans_rec.T_moneyA       = theCbuCbaUC.Fld_IsPrivate ? 2.00M : 0.00M; // Cijena              
         decimal cijenaLoko;
         if(ZXC.projectYearAsInt <= 2022) cijenaLoko = 3.00M;//kn bilo 2 do nekog 10 mj 2022
         else                             cijenaLoko = 0.40M;//EUR                          
         xtrans_rec.T_moneyA       = theCbuCbaUC.Fld_IsPrivate ? cijenaLoko : 0.00M; // Cijena              

       //if(theCbuCbaUC.Fld_IsPrivate == false) // za sluzbeno vozilo racunaj kilometriOd - kilometriDo (stanja brojila) 
       //if(theCbuCbaUC.Fld_IsBrojilo == true) // za sluzbeno vozilo racunaj kilometriOd - kilometriDo (stanja brojila) 
         // 23.11.2011: Naknadno dodao bez konzultiranja sa Tamarom. provjeri kasnije.
         if(theCbuCbaUC.Fld_IsBrojilo == true ||
            theCbuCbaUC.Fld_IsPrivate == false) // za sluzbeno vozilo ILI ako je zadan stanje brojila: racunaj kilometriOd - kilometriDo (stanja brojila) 
         {
            xtrans_rec.T_intA = (int)zadnjeStanjeBrojila; // PocSt Kilomet       
            
            zadnjeStanjeBrojila += lokoVoznja.kilometara;

            xtrans_rec.T_intB = (int)zadnjeStanjeBrojila; // ZavSt Kilomet       
         }

         #endregion Set rptXtrans_rec

         MixerDao.AutoSetMixer(TheDbConnection, ref line, mixer_rec, xtrans_rec);
      }

      #endregion AutoAddMixer
   }

   #endregion Ciribu - Ciriba

}

public class PutNalTuzDUC : PutNalDUC
{
   #region Constructor

   public PutNalTuzDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul): base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_PUTN_T
         });
   }

   #endregion Constructor

   #region CreateSpecificHampers

   protected override void CreateSpecificHampers()
   {
      hamp_mjTroska.Visible = true;
      hamp_mjTroska.Location = new Point(hamp_napomena.Right, hamp_napomena.Top);

      InitializeHamper_Radnik(out hamp_radnik);

      nextY = hamp_radnik.Bottom;

      InitializeHamper_PutNal(out hamp_putNal);
      nextY = hamp_putNal.Bottom;

      nextX = hamp_tt.Right;
      nextY = hamp_tt.Top;
      InitializeHamper_Veza12Link(out hamp_veza);
      
      nextY = hamp_putNal.Bottom;

      InitializeHamper_obrDnev(out hamp_obrDnev);

      nextY = hamp_obrDnev.Bottom;
      InitializeHamper_kontoPrilog(out hamp_linkIzvj);

      nextY = hamp_linkIzvj.Bottom;
      InitializeHamper_ObracunSumeAll(out hamp_sumeAll);

   }
   
   #endregion CreateSpecificHampers

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_opis_128_CreateColumn (0       , "Relacija" , ""  , 128, ZXC.luiListaMixerRelacija);
      T_kol_CreateColumn      (ZXC.Q3un, "Kol/Km"   , "Km", 0);
      T_intA_CreateColumn     (ZXC.Q4un, "BrojiloOd", "");
      T_intB_CreateColumn     (ZXC.Q4un, "BrojiloDo", "");
      T_moneyA_CreateColumn   (ZXC.Q4un, "Cijena"   , "", 2);
      R_iznos_CreateColumn    (ZXC.Q4un, 2, "Iznos", "");

      vvtbT_kol.JAM_FieldExitMethod        = new EventHandler(OnExitIntAIntBKolKm_CalcOthers);
      vvtbT_intA.JAM_FieldExitMethod       = new EventHandler(OnExitIntAIntBKolKm_CalcOthers);
      vvtbT_intB.JAM_FieldExitMethod       = new EventHandler(OnExitIntAIntBKolKm_CalcOthers);

      vvtbT_kol    .JAM_ShouldCalcTransAndSumGrid = true;
      vvtbT_moneyA .JAM_ShouldCalcTransAndSumGrid = true;

   }

   #endregion InitializeTheGrid_Columns()

   #region InitializeDUC_Specific_Columns2()

   protected override void InitializeDUC_Specific_Columns2()
   {
      T_2opis_128_CreateColumn (        0, "Opis troška", ""  , 128);
      T_2moneyA_CreateColumn   (ZXC.Q4un , "Iznos", "", 2);
      T_2konto_CreateColumn    (ZXC.Q3un , "Konto", "");

      vvtbT2_moneyA.JAM_ShouldSumGrid = true;

   }
   public override void PutDgvTransSumFields2()
   {
      TheSumGrid2[ci2.iT_moneyA, 0].Value = mixer_rec.Sum2_MoneyA;
   }

   #endregion InitializeDUC_Specific_Columns2()
  
   #region overrideTabPageTitle

   public override string TabPageTitle2
   {
      get { return "Obračun ostalih troškova"; }
   }

   #endregion overrideTabPageTitle

}

public class PutNalInoDUC : PutNalDUC
{
   #region Constructor

   public PutNalInoDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_PUTN_I
         });
   }

   #endregion Constructor

   #region CreateSpecificHampers

   protected override void CreateSpecificHampers()
   {
      hamp_mjTroska.Visible = true;
      hamp_mjTroska.Location = new Point(hamp_napomena.Right, hamp_napomena.Top);

      InitializeHamper_Radnik(out hamp_radnik);

      nextY = hamp_radnik.Bottom;

      InitializeHamper_PutNal(out hamp_putNal);
      nextY = hamp_putNal.Bottom;

      nextX = hamp_tt.Right;
      nextY = hamp_tt.Top;
      InitializeHamper_Veza12Link(out hamp_veza);

      nextY = hamp_putNal.Bottom;

      InitializeHamper_obrDnev(out hamp_obrDnev);

      nextY = hamp_obrDnev.Bottom;
      InitializeHamper_kontoPrilog(out hamp_linkIzvj);

      nextY = hamp_linkIzvj.Bottom;
      InitializeHamper_ObracunSumeAll(out hamp_sumeAll);

   }

   #endregion CreateSpecificHampers

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_opis_128_CreateColumn    (0       , "Relacija" , ""  , 128, ZXC.luiListaMixerRelacija);
      T_kol_CreateColumn         (ZXC.Q3un, "Kol/Km"   , "Km", 0);
      T_intA_CreateColumn        (ZXC.Q4un, "BrojiloOd", "");
      T_intB_CreateColumn        (ZXC.Q4un, "BrojiloDo", "");
      T_moneyA_CreateColumn      (ZXC.Q4un, "Cijena"   , "", 2);
      T_kpdbZiroB_32_CreateColumn(ZXC.Q3un, "DevVal", "", ZXC.luiListaDeviza);
      R_iznos_CreateColumn       (ZXC.Q4un, 2, "Iznos", "");

      vvtbT_kol.JAM_FieldExitMethod        = new EventHandler(OnExitIntAIntBKolKm_CalcOthers);
      vvtbT_intA.JAM_FieldExitMethod       = new EventHandler(OnExitIntAIntBKolKm_CalcOthers);
      vvtbT_intB.JAM_FieldExitMethod       = new EventHandler(OnExitIntAIntBKolKm_CalcOthers);

      vvtbT_kol    .JAM_ShouldCalcTransAndSumGrid = true;
      vvtbT_moneyA .JAM_ShouldCalcTransAndSumGrid = true;

   }

   #endregion InitializeTheGrid_Columns()

   #region InitializeDUC_Specific_Columns2()

   protected override void InitializeDUC_Specific_Columns2()
   {
      T_2opis_128_CreateColumn (        0, "Opis troška", ""  , 128);
      T_2moneyA_CreateColumn   (ZXC.Q4un , "Iznos", "", 2);
      T_2devValuta_CreateColumn(ZXC.Q3un, "DevVal", "", ZXC.luiListaDeviza);
      T_2konto_CreateColumn(ZXC.Q3un, "Konto", "");

      vvtbT2_moneyA.JAM_ShouldSumGrid = true;
      vvtbT2_devValuta.JAM_CharacterCasing = CharacterCasing.Upper;
   }
   public override void PutDgvTransSumFields2()
   {
      TheSumGrid2[ci2.iT_moneyA, 0].Value = mixer_rec.Sum2_MoneyA;
   }

   #endregion InitializeDUC_Specific_Columns2()

   #region overrideTabPageTitle

   public override string TabPageTitle2
   {
      get { return "Obračun ostalih troškova"; }
   }

   #endregion overrideTabPageTitle

}

public class LokoVoznjaDUC : PutNalDUC

{
   #region Constructor

   public LokoVoznjaDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_PUTN_L
         });
   }

   #endregion Constructor
 
   #region CreateSpecificHampers

   protected override void CreateSpecificHampers()
   {
      hamp_mjTroska.Visible = true;
      hamp_mjTroska.Location = new Point(hamp_napomena.Right, hamp_napomena.Top);

      InitializeHamper_Radnik(out hamp_radnik);


      nextX = hamp_tt.Right;
      nextY = hamp_tt.Top;
      InitializeHamper_Veza12(out hamp_veza);

      nextX = 0;
      nextY = hamp_radnik.Bottom;
      InitializeHamper_Vozilo(out hamp_vozilo);
      nextY = hamp_vozilo.Bottom;

      

      InitializeHamper_ObracunSumeAll(out hamp_sumeAll);
   }

   #endregion CreateSpecificHampers

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_dateOd_CreateColumn      (ZXC.Q4un, "Datum");
      T_opis_128_CreateColumn    (0, "Relacija", "", 128, ZXC.luiListaMixerRelacija);
      T_kol_CreateColumn         (ZXC.Q3un, "Km", "Km", 0);
      T_intA_CreateColumn        (ZXC.Q4un, "BrojiloOd", "");
      T_intB_CreateColumn        (ZXC.Q4un, "BrojiloDo", "");
      T_moneyA_CreateColumn      (ZXC.Q4un, "Cijena", "", 2);
      T_vezniDokA_64_CreateColumn(ZXC.Q6un, "Zadatak", "Zadatak", ZXC.luiListaMixerZadatak);

      R_iznos_CreateColumn    (ZXC.Q4un, 2, "Iznos", "");

      vvtbT_kol.JAM_FieldExitMethod        = new EventHandler(OnExitIntAIntBKolKm_CalcOthers);
      vvtbT_intA.JAM_FieldExitMethod       = new EventHandler(OnExitIntAIntBKolKm_CalcOthers);
      vvtbT_intB.JAM_FieldExitMethod       = new EventHandler(OnExitIntAIntBKolKm_CalcOthers);

      vvtbT_kol   .JAM_ShouldCalcTransAndSumGrid = true;
      vvtbT_moneyA.JAM_ShouldCalcTransAndSumGrid = true;

      vvtbT_moneyA.JAM_ShouldCopyPrevRow = true;

   }

   #endregion InitializeTheGrid_Columns()

   #region InitializeDUC_Specific_Columns2()

   protected override void InitializeDUC_Specific_Columns2()
   {
      T_2opis_128_CreateColumn (        0, "Opis troška", ""  , 128);
      T_2moneyA_CreateColumn   (ZXC.Q4un , "Iznos", "", 2);
      T_2konto_CreateColumn    (ZXC.Q3un , "Konto", "");

      vvtbT2_moneyA.JAM_ShouldSumGrid = true;

   }
   public override void PutDgvTransSumFields2()
   {
      TheSumGrid2[ci2.iT_moneyA, 0].Value = mixer_rec.Sum2_MoneyA;
   }

   #endregion InitializeDUC_Specific_Columns2()

   #region overrideTabPageTitle

   public override string TabPageTitle2
   {
      get { return "Obračun ostalih troškova"; }
   }

   #endregion overrideTabPageTitle

}

public class PutRadListDUC : PutNalDUC
{
   #region Constructor

   public PutRadListDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_PUTN_R
         });
   }

   #endregion Constructor

   #region CreateSpecificHampers

   protected override void CreateSpecificHampers()
   {
      hamp_mjTroska.Visible = true;
      hamp_mjTroska.Location = new Point(hamp_napomena.Right, hamp_napomena.Top);

      InitializeHamper_Radnik(out hamp_radnik);

      nextY = hamp_radnik.Bottom;

      InitializeHamper_Vozilo(out hamp_vozilo);
      nextY = hamp_vozilo.Bottom;

      InitializeHamper_Zadatak(out hamp_zadatak);

      nextX = hamp_tt.Right;
      nextY = hamp_tt.Top;
      InitializeHamper_Veza12(out hamp_veza);

      nextX = 0;
      nextY = hamp_zadatak.Bottom;

      InitializeHamper_ObracunSumeAll(out hamp_sumeAll);
      hamp_sumeAll.Visible = false;

   }

   #endregion CreateSpecificHampers

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_dateOd_WithTime_CreateColumn (ZXC.Q7un,    "Odlazak" );
      T_dateDo_WithTime_CreateColumn (ZXC.Q7un,    "Povratak");
      T_opis_128_CreateColumn        (0,           "Relacija"        , "", 128, ZXC.luiListaMixerRelacija);
      T_kol_CreateColumn             (ZXC.Q3un,    "Kol/Km"          , "Km", 0);
      T_intA_CreateColumn            (ZXC.Q4un,    "BrojiloOd"       , "BrojiloOd");
      T_intB_CreateColumn            (ZXC.Q4un,    "BrojiloDo"       , "BrojiloDo");
      T_kpdbNameB_50_CreateColumn    (ZXC.Q6un,    "Dobavljač goriva", "Dobavljač goriva");
      T_moneyB_CreateColumn          (ZXC.Q4un, 0, "KmPriTočenju"    , "Stanje brojila prilikom točenja goriva", false);
      T_moneyC_CreateColumn          (ZXC.Q4un, 2, "UtočenoLit"      , "Utoceno litara", false, true);

      vvtbT_kol .JAM_FieldExitMethod = new EventHandler(OnExitIntAIntBKolKm_CalcOthers);
      vvtbT_intA.JAM_FieldExitMethod = new EventHandler(OnExitIntAIntBKolKm_CalcOthers);
      vvtbT_intB.JAM_FieldExitMethod = new EventHandler(OnExitIntAIntBKolKm_CalcOthers);

      vvtbT_kol.JAM_ShouldCalcTransAndSumGrid = true;
      vvtbT_moneyB.JAM_ShouldSumGrid = false;
   }

   #endregion InitializeTheGrid_Columns()

   #region overrideTabPageTitle

   public override string TabPageTitle1
   {
      get { return "Obračun"; }
   }

   #endregion overrideTabPageTitle

}

public class PutNalFilterUC : VvFilterUC
{
   #region Fieldz

   private VvHamper    hamp_rbt, hamp_cbx;
   private RadioButton rbt_putNalog, rbt_putObrac, rbt_putNalIObrP, rbt_putRadListNalog, rbt_putRadListObr, rbt_putLoko, rbt_izvjAll, rbt_izvjOnlyPrimj;
   private CheckBox    cbx_printNapomena, cbx_ocuPNIuKn;

   #endregion Fieldz

   #region  Constructor

   public PutNalFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;
     
      CreateHampers();

      CreateHamper_4ButtonsResetGo_Width(hamp_rbt.Width);

      hamp_rbt.Location = new Point(nextX, hamper4buttons.Bottom + ZXC.Qun4);
      hamp_cbx.Location = new Point(nextX, hamp_rbt      .Bottom + ZXC.Qun4);
      hamperHorLine.Visible = false;

      this.Width  = hamp_rbt.Width + ZXC.QUN;
      this.Height = hamp_cbx.Bottom + ZXC.QUN;

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region Hampers

   private void CreateHampers()
   {
      InitializeHamper_Rbt(out hamp_rbt);
      InitializeHamper_Cbx(out hamp_cbx);
   }

   private void InitializeHamper_Rbt(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 7, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q8un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = ZXC.Qun4 + ZXC.Qun10;

      hamper.CreateVvLabel      (0, 0,       "Vrsta ispisa", ContentAlignment.MiddleLeft);

      rbt_putNalIObrP     = hamper.CreateVvRadioButton(0, 1, null, "Putni Nalog i Putni Obračun"    , TextImageRelation.ImageBeforeText);
      rbt_putNalog        = hamper.CreateVvRadioButton(0, 2, null, "Putni Nalog"                    , TextImageRelation.ImageBeforeText);
      rbt_putObrac        = hamper.CreateVvRadioButton(0, 3, null, "Putni Obračun"                  , TextImageRelation.ImageBeforeText);
      rbt_putRadListNalog = hamper.CreateVvRadioButton(0, 4, null, "Putni Radni List Nalog"         , TextImageRelation.ImageBeforeText);
      rbt_putRadListObr   = hamper.CreateVvRadioButton(0, 5, null, "Putni Radni List Obračun"       , TextImageRelation.ImageBeforeText);
      rbt_putLoko         = hamper.CreateVvRadioButton(0, 6, null, "Obračun Loko vožnje"            , TextImageRelation.ImageBeforeText);
      rbt_izvjAll         = hamper.CreateVvRadioButton(0, 4, null, "Izvještaj s puta prošireno"     , TextImageRelation.ImageBeforeText);
      rbt_izvjOnlyPrimj   = hamper.CreateVvRadioButton(0, 5, null, "Izvještaj s puta samo sa ekrana", TextImageRelation.ImageBeforeText);



      if(TheVvUC is PutNalInoDUC || TheVvUC is PutNalTuzDUC)
      {
         rbt_putRadListNalog.Visible = false;
         rbt_putRadListObr  .Visible = false;
         rbt_putLoko        .Visible = false;
         rbt_putNalIObrP    .Checked = true;
         rbt_putNalIObrP    .Tag = true;
      }
      else if(TheVvUC is LokoVoznjaDUC)
      {
         rbt_izvjAll        .Visible = 
         rbt_izvjOnlyPrimj  .Visible = 
         rbt_putNalIObrP    .Visible = 
         rbt_putNalog       .Visible = 
         rbt_putObrac       .Visible =
         rbt_putRadListNalog.Visible =
         rbt_putRadListObr  .Visible = false;
         
         rbt_putLoko.Checked = true;
         rbt_putLoko.Tag     = true;

         rbt_putLoko.Location = new Point(ZXC.Qun4, ZXC.Q2un);
      }
      else if(TheVvUC is PutRadListDUC)
      {
         rbt_izvjAll      .Visible =
         rbt_izvjOnlyPrimj.Visible = 
         rbt_putNalIObrP  .Visible = 
         rbt_putNalog     .Visible = 
         rbt_putObrac     .Visible =
         rbt_putLoko      .Visible = false;
         
         rbt_putRadListNalog.Checked = true;
         rbt_putRadListNalog.Tag     = true;

         rbt_putRadListNalog.Location = new Point(ZXC.Qun4, ZXC.Q2un);
         rbt_putRadListObr  .Location = new Point(ZXC.Qun4, ZXC.Q3un);
      }

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.HamperStyling(hamper);
   }
   
   private void InitializeHamper_Cbx(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 2, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q8un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = ZXC.Qun4 + ZXC.Qun10;

      cbx_printNapomena = hamper.CreateVvCheckBox_OLD(0, 0, null, "Printaj napomenu", System.Windows.Forms.RightToLeft.No);
      cbx_ocuPNIuKn     = hamper.CreateVvCheckBox_OLD(0, 1, null, " + Sume u kunama"   , System.Windows.Forms.RightToLeft.No);

      if(TheVvUC is PutNalInoDUC) cbx_ocuPNIuKn.Visible = true;
      else                        cbx_ocuPNIuKn.Visible = false;

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.HamperStyling(hamper);
   }

   #endregion Hampers

   #region Fld_

   public PutNalFilter.PrintPutNalEnum Fld_PrintPutNal
   {
      get
      {
         if(rbt_putNalIObrP.Checked)
         {
            if(TheVvUC is PutNalInoDUC)       return PutNalFilter.PrintPutNalEnum.PutNalPutObrI;
            else                              return PutNalFilter.PrintPutNalEnum.PutNalPutObrT;
         }                                    
         else if(rbt_putNalog.Checked)        
         {                                    
            if(TheVvUC is PutNalInoDUC)       return PutNalFilter.PrintPutNalEnum.PutNalI;
            else                              return PutNalFilter.PrintPutNalEnum.PutNalT;
         }                                    
         else if(rbt_putObrac.Checked)        
         {                                    
            if(TheVvUC is PutNalInoDUC)       return PutNalFilter.PrintPutNalEnum.PutObrI;
            else                              return PutNalFilter.PrintPutNalEnum.PutObrT;
         }
         else if(rbt_putLoko        .Checked) return PutNalFilter.PrintPutNalEnum.PutLoko;
         else if(rbt_putRadListNalog.Checked) return PutNalFilter.PrintPutNalEnum.PutRadListNalog;
         else if(rbt_putRadListObr  .Checked) return PutNalFilter.PrintPutNalEnum.PutRadListObr;
         else if(rbt_izvjAll        .Checked) return PutNalFilter.PrintPutNalEnum.IzvjAll ;
         else if(rbt_izvjOnlyPrimj  .Checked) return PutNalFilter.PrintPutNalEnum.IzvjOnly;

         else throw new Exception("Fld_PrintPutNal: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case PutNalFilter.PrintPutNalEnum.PutNalPutObrT  : rbt_putNalIObrP    .Checked = true; break;
            case PutNalFilter.PrintPutNalEnum.PutNalPutObrI  : rbt_putNalIObrP    .Checked = true; break;
            case PutNalFilter.PrintPutNalEnum.PutNalT        : rbt_putNalog       .Checked = true; break;
            case PutNalFilter.PrintPutNalEnum.PutNalI        : rbt_putNalog       .Checked = true; break;
            case PutNalFilter.PrintPutNalEnum.PutObrT        : rbt_putObrac       .Checked = true; break;
            case PutNalFilter.PrintPutNalEnum.PutObrI        : rbt_putObrac       .Checked = true; break;
            case PutNalFilter.PrintPutNalEnum.PutLoko        : rbt_putLoko        .Checked = true; break;
            case PutNalFilter.PrintPutNalEnum.PutRadListNalog: rbt_putRadListNalog.Checked = true; break;
            case PutNalFilter.PrintPutNalEnum.PutRadListObr  : rbt_putRadListObr  .Checked = true; break;
            case PutNalFilter.PrintPutNalEnum.IzvjAll        : rbt_izvjAll        .Checked = true; break;
            case PutNalFilter.PrintPutNalEnum.IzvjOnly       : rbt_izvjOnlyPrimj  .Checked = true; break;

         }
      }
   }

   public bool Fld_IsPrintNapomena { get { return cbx_printNapomena.Checked; } set { cbx_printNapomena.Checked = value; } }
   public bool Fld_OcuPNIuKunama   { get { return cbx_ocuPNIuKn.Checked;     } set { cbx_ocuPNIuKn.Checked = value; } }

   #endregion Fld_

   #region Put & GetFilterFields

   private PutNalFilter ThePutNalFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as PutNalFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      ThePutNalFilter = (PutNalFilter)_filter_data;

      if(ThePutNalFilter != null)
      {
         Fld_PrintPutNal     = ThePutNalFilter.PrintPutNal;
         Fld_IsPrintNapomena = ThePutNalFilter.IsPrintNapomena;
         Fld_OcuPNIuKunama   = ThePutNalFilter.OcuPNIuKunama ;
      }

      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
      ThePutNalFilter.PrintPutNal     = Fld_PrintPutNal;
      ThePutNalFilter.IsPrintNapomena = Fld_IsPrintNapomena;
      ThePutNalFilter.OcuPNIuKunama   = Fld_OcuPNIuKunama ;
   }

   #endregion Put & GetFilterFields

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
   }

   #endregion AddFilterMemberz()

}

public class PutNalFilter : VvRpt_Mix_Filter
{

   public enum PrintPutNalEnum
   {
      PutNalT, PutObrT, PutNalPutObrT, PutNalI, PutObrI, PutNalPutObrI, PutRadListNalog, PutRadListObr, PutLoko, IzvjAll, IzvjOnly
   }

   public bool IsPrintNapomena { get; set; }
   public bool OcuPNIuKunama   { get; set; }
   public ZXC.Spol Spol        { get; set; }
   public PrintPutNalEnum PrintPutNal { get; set; }

   public PutNalDUC theDUC;

   public PutNalFilter(PutNalDUC _theDUC)
   {
      this.theDUC = _theDUC;
      SetDefaultFilterValues();
   }

   #region SetDefaultFilterValues()

   public override void SetDefaultFilterValues()
   {
      ZXC.VvDataBaseInfo vvDBinfo = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects;
      int projectYear = int.Parse(vvDBinfo.ProjectYear);
      int thisYear = DateTime.Now.Year;

      PrintPutNal    = PrintPutNalEnum.PutNalPutObrT;

   }

   #endregion SetDefaultFilterValues()

}

public class CiribuCiribaDlg : VvDialog
{
   #region Propertiez

   private Crownwood.DotNetMagic.Forms.DotNetMagicForm ThePreviewCiribuCiribaDlgForm { get; set; }

   #endregion Propertiez

   #region Fieldz

   private Button okButton, cancelButton;
   private int    dlgWidth, dlgHeight;

   public CiribuCiribaUC TheUC { get; private set; }
  
   #endregion Fieldz

   #region Constructor

   public CiribuCiribaDlg()
   {

      TheUC = new CiribuCiribaUC();

      SuspendLayout();

      this.Font        = ZXC.vvFont.BaseFont;
      this.Style       = ZXC.vvColors.vvform_VisualStyle;
      this.BackColor   = ZXC.vvColors.userControl_BackColor;

      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "CiribuCiriba";

      CreateTheUC();

      dlgWidth        = TheUC.Width;
      dlgHeight       = TheUC.Height + ZXC.QunMrgn * 2 + ZXC.QunBtnH;


      this.ClientSize = new Size(dlgWidth, dlgHeight);
      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      
      // !!! 
      this.AcceptButton = null;
      
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      cancelButton.Click += new EventHandler(cancelButton_Click); // Da supresa validaciju

      //TheUC.Validating += new CancelEventHandler(CiribuCiriba_Validating);
      this.FormClosing += new FormClosingEventHandler(CiribuCiriba_FormClosing);

      ResumeLayout();
   }

   void cancelButton_Click(object sender, EventArgs e)
   {
      // Je'ote konj! Ne znam drugacije supressati validaciju ako ne stisne OK nego Cancel button!!!
      TheUC.Fld_DatumOd = TheUC.Fld_DatumDo = DateTime.Now;
      TheUC.Fld_PersonCd = 1;
      TheUC.Fld_VoziloCD = "q";
      TheUC.Fld_IsBrojilo = false;
   }

   //void CiribuCiriba_Validating(object sender, CancelEventArgs e)
   void CiribuCiriba_FormClosing(object sender, FormClosingEventArgs e)
    {

      if(TheUC.Fld_DatumOd == DateTime.MinValue || 
         TheUC.Fld_DatumDo == DateTime.MinValue) { ZXC.aim_emsg(MessageBoxIcon.Error, "Molim, zadajte razdoblje." ); e.Cancel = true; }
      if(TheUC.Fld_PersonCd.IsZero())            { ZXC.aim_emsg(MessageBoxIcon.Error, "Molim, zadajte djelatnika."); e.Cancel = true; }
      if(TheUC.Fld_VoziloCD.IsEmpty())           { ZXC.aim_emsg(MessageBoxIcon.Error, "Molim, zadajte vozilo."    ); e.Cancel = true; }
      if(TheUC.Fld_IsBrojilo && 
         TheUC.Fld_Km < TheUC.Fld_PrethBrojilo)
                                                 { ZXC.aim_emsg(MessageBoxIcon.Error, "Zavrsno stanje brojila ne može biti manje od početnog stanja brojila."    ); e.Cancel = true; }
   }

   #endregion Constructor

   #region UC

   private void CreateTheUC()
   {
      TheUC.Parent   = this;
      TheUC.Location = new Point(0, 0);
   }

   #endregion UC
}

public class CiribuCiribaUC  : VvOtherUC
{
   #region Fieldz

   private VvTextBox tbx_PersonCd, tbx_ime, tbx_prezime,
                     tbx_voziloCD, tbx_vozilo,
                     tbx_DatumOD, tbx_DatumDO, 
                     tbx_prethBrojilo, tbx_km,
                     tbx_mtros_cd, tbx_mtros_tk, tbx_mtros_Naziv, tbx_napomena;

   public  VvHamper         hamp_cbucba;
   private VvDateTimePicker dtp_DatumOD, dtp_DatumDO;
   private CheckBox         cbx_isPrivate;
   private RadioButton      rbt_km, rbt_brojilo;

   #endregion Fieldz

   #region Constructor

   public CiribuCiribaUC()
   {
      SuspendLayout();

      CreateHampers();

      this.Size = new Size(hamp_cbucba.Right + ZXC.Q2un, hamp_cbucba.Bottom);

      ResumeLayout();
   }

   #endregion Constructor

   #region Hampers

   private void CreateHampers()
   {
      InitializeHamper_cbucba(out hamp_cbucba);
   }


   private void InitializeHamper_cbucba(out VvHamper hamper)
   {

      hamper = new VvHamper(6, 8, "", this, false);
      //                                     0        1          2          3        4         5    
      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q3un,  ZXC.Q3un, ZXC.Q4un, ZXC.Q2un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,  ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;
      
      hamper.Location      = new Point(ZXC.QunMrgn, ZXC.QunMrgn);

      for(int i = 0; i < hamper.VvRowHgt.Length; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN  ;
         hamper.VvSpcBefRow[i] = ZXC.Qun8 ;
      }
      hamper.VvRowHgt[5] = ZXC.QUN - ZXC.Qun4;
      hamper.VvRowHgt[6] = ZXC.QUN - ZXC.Qun4;
      hamper.VvSpcBefRow[5] = ZXC.Qun4;
      hamper.VvSpcBefRow[6] = ZXC.Qun10;
      hamper.VvSpcBefRow[7] = ZXC.Qun4;

      hamper.VvBottomMargin = hamper.VvTopMargin;

                     hamper.CreateVvLabel  (0, 0, "Djelatnik:", ContentAlignment.MiddleRight);
      tbx_PersonCd = hamper.CreateVvTextBox(1, 0, "tbx_PersonCd", "Sifra djelatnika"); 
      tbx_prezime  = hamper.CreateVvTextBox(2, 0, "tbx_prezime" , "Prezime"         , 40, 1, 0);
      tbx_ime      = hamper.CreateVvTextBox(4, 0, "tbx_ime"     , "Ime"             , 40, 1, 0);
      tbx_ime.JAM_ReadOnly = true;
      
      tbx_PersonCd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_prezime.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_prezime.Font = ZXC.vvFont.LargeBoldFont;
      
      tbx_PersonCd.JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType     , new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterSifra)  , new EventHandler(AnyPersonTextBoxLeave));
      tbx_prezime .JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonTextBoxLeave));
      
                        hamper.CreateVvLabel  (0, 1, "Mjesto troška:", ContentAlignment.MiddleRight);
      tbx_mtros_cd    = hamper.CreateVvTextBox(1, 1, "tbx_mtros_cd", "Šifra mjesta troška");
      tbx_mtros_tk    = hamper.CreateVvTextBox(2, 1, "tbx_mtros_tk", "Tiker mjesta troška");
      tbx_mtros_Naziv = hamper.CreateVvTextBox(3, 1, "tbx_mtros_naziv", "Naziv mjesta troška", 40, 2, 0);

      tbx_mtros_cd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_mtros_tk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_mtros_cd   .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD   .SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_mtros_tk   .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_mtros_Naziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv .SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyMtrosTextBoxLeave));

                     hamper.CreateVvLabel       (0, 2, "Vozilo:", ContentAlignment.MiddleRight);
      tbx_voziloCD = hamper.CreateVvTextBoxLookUp("tbx_voziloCD",1, 2,  "Registracija vozila", 40, 1, 0);
      tbx_voziloCD.JAM_Set_LookUpTable(ZXC.luiListaMixerVozilo, (int)ZXC.Kolona.prva);
      tbx_voziloCD.JAM_lookUp_NOTobligatory = true;

      tbx_vozilo = hamper.CreateVvTextBox(3, 2, "tbx_vozilo", "Vozilo", 40, 1, 0);
      tbx_voziloCD.JAM_lui_NameTaker_JAM_Name = tbx_vozilo.JAM_Name;
      tbx_voziloCD.JAM_FieldExitMethod_2 = new EventHandler(OnExitVoziloCD_GetZadnjeStanjeBrojila);

      cbx_isPrivate = hamper.CreateVvCheckBox_OLD(5, 2, null, "Privatno", System.Windows.Forms.RightToLeft.Yes);
      cbx_isPrivate.Name = "cbx_isPrivate";
      tbx_voziloCD.JAM_lui_FlagTaker_JAM_Name = cbx_isPrivate.Name;


                    hamper.CreateVvLabel  (0, 3, "Za razdoblje:", ContentAlignment.MiddleRight);
      tbx_DatumOD = hamper.CreateVvTextBox(1, 3, "tbx_datumOd", "Od datuma", 12, 1, 0);
      tbx_DatumOD.JAM_IsForDateTimePicker = true;
      dtp_DatumOD = hamper.CreateVvDateTimePicker(1, 3, "", 1, 0, tbx_DatumOD);
      dtp_DatumOD.Name = "dtp_DatumOD";
      dtp_DatumOD.Tag  = tbx_DatumOD;
      tbx_DatumOD.Tag  = dtp_DatumOD;

      tbx_DatumDO = hamper.CreateVvTextBox(3, 3, "tbx_datumDo", "", 12, 1, 0);
      tbx_DatumDO.JAM_IsForDateTimePicker = true;
      dtp_DatumDO = hamper.CreateVvDateTimePicker(3, 3, "", 1, 0, tbx_DatumDO);
      dtp_DatumDO.Name = "dtp_DatumDO";
      dtp_DatumDO.Tag = tbx_DatumDO;
      tbx_DatumDO.Tag = dtp_DatumDO;

      tbx_DatumOD.ContextMenu = dtp_DatumOD.ContextMenu = 
      tbx_DatumDO.ContextMenu = dtp_DatumDO.ContextMenu =  CreateNewContexMenu_Date();

                         hamper.CreateVvLabel  (0, 4, "Prethodno stanje Brojila:", ContentAlignment.MiddleRight);
      tbx_prethBrojilo = hamper.CreateVvTextBox(1, 4, "tbx_prethBrojilo", "Od datuma", 12, 1, 0);
      tbx_prethBrojilo.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_prethBrojilo.TextAlign = HorizontalAlignment.Left;

      rbt_brojilo = hamper.CreateVvRadioButton(0, 5, null, "Završno stanje brojila:", TextImageRelation.TextBeforeImage);
      rbt_km      = hamper.CreateVvRadioButton(0, 6, null, "Prijeđeni kilometri:"   , TextImageRelation.TextBeforeImage);
      rbt_km.Checked = true;
      rbt_brojilo.TextAlign = ContentAlignment.MiddleRight;
      rbt_km     .TextAlign = ContentAlignment.MiddleRight;
      rbt_km     .ImageAlign = 
      rbt_brojilo.ImageAlign = ContentAlignment.MiddleRight;

      tbx_km = hamper.CreateVvTextBox(1, 5, "tbx_km", "Kilometara", 12, 1,1);
      tbx_km.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_km.TextAlign = HorizontalAlignment.Left;

                     hamper.CreateVvLabel  (0, 7, "Napomena:", ContentAlignment.MiddleRight);
      tbx_napomena = hamper.CreateVvTextBox(1, 7, "tbx_napomena", "Napomena", 32, 4, 0);

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvOtherUC);
   }

   private void OnExitVoziloCD_GetZadnjeStanjeBrojila(object sender, EventArgs e)
   {
      VvTextBox vvtb = sender as VvTextBox;

      if(vvtb.EditedHasChanges() == false) return;

      if(Fld_IsPrivate)
      {
         Fld_PrethBrojilo = 0.00M;
         Fld_IsBrojilo    = false;
      }
      else
      {
         decimal? zadnjeStanjeBrojila = XtransDao.GetZadnjeStanjeBrojila(TheDbConnection, Fld_VoziloCD, false);

         if(zadnjeStanjeBrojila == null) zadnjeStanjeBrojila = 0.00M;

         Fld_PrethBrojilo = (decimal)zadnjeStanjeBrojila;
         Fld_IsBrojilo    = true;
      }
   }

   #region Datumi_ContexMenu

   private VvStandardTextBoxContextMenu CreateNewContexMenu_Date()
   {
      VvStandardTextBoxContextMenu date_ContexMenu = new VvStandardTextBoxContextMenu(new MenuItem[] 
            { 
               new MenuItem("Danas"           , IspuniDatume),
               new MenuItem("Tekuća godina"   , IspuniDatume),
               new MenuItem("Tekući mjesec"   , IspuniDatume),
               new MenuItem("Prvi kvartal"    , IspuniDatume),
               new MenuItem("Drugi kvartal"   , IspuniDatume),
               new MenuItem("Treći kvartal"   , IspuniDatume),
               new MenuItem("Četvrti kvartal" , IspuniDatume),
               new MenuItem("1 -11 mjesec"    , IspuniDatume),
               new MenuItem("1 -10 mjesec"    , IspuniDatume),
               new MenuItem("1 - 9 mjesec"    , IspuniDatume),
               new MenuItem("1 - 8 mjesec"    , IspuniDatume),
               new MenuItem("1 - 7 mjesec"    , IspuniDatume),
               new MenuItem("1 - 6 mjesec"    , IspuniDatume),
               new MenuItem("1 - 5 mjesec"    , IspuniDatume),
               new MenuItem("1 - 4 mjesec"    , IspuniDatume),
               new MenuItem("1 - 3 mjesec"    , IspuniDatume),
               new MenuItem("1 - 2 mjesec"    , IspuniDatume),
               new MenuItem("Siječanj"        , IspuniDatume),
               new MenuItem("Veljača"         , IspuniDatume),
               new MenuItem("Ožujak"          , IspuniDatume),
               new MenuItem("Travanj"         , IspuniDatume),
               new MenuItem("Svibanj"         , IspuniDatume),
               new MenuItem("Lipanj"          , IspuniDatume),
               new MenuItem("Srpanj"          , IspuniDatume),
               new MenuItem("Kolovoz"         , IspuniDatume),
               new MenuItem("Rujan"           , IspuniDatume),
               new MenuItem("Listopad"        , IspuniDatume),
               new MenuItem("Studeni"         , IspuniDatume),
               new MenuItem("Prosinac"        , IspuniDatume),
               new MenuItem("-")
            });

      return date_ContexMenu;
   }

   private void IspuniDatume(object sender, EventArgs e)
   {
      MenuItem tsmi = sender as MenuItem;

      string text = tsmi.Text;
      string textOd = "";
      string textDo = "";
      string mj02 = "28"; ;

      // ovo je dobro. ostavi ovako (TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.ProjectYear;) 
      string godina = ZXC.projectYear;
      int god = int.Parse(godina);

      if(DateTime.IsLeapYear(god)) mj02 = "29";
      else mj02 = "28";

      switch(text)
      { //                              mmOD           dd.mmDO
         case "Tekuća godina"  : textOd = "01"; textDo = "31.12"; break;
         case "Tekući mjesec"  : textOd = DateTime.Today.Month.ToString(); textDo = DateTime.DaysInMonth(god, DateTime.Today.Month).ToString() + "." + DateTime.Today.Month.ToString(); break;
         case "Prvi kvartal"   : textOd = "01"; textDo = "31.03"; break;
         case "Drugi kvartal"  : textOd = "04"; textDo = "30.06"; break;
         case "Treći kvartal"  : textOd = "07"; textDo = "30.09"; break;
         case "Četvrti kvartal": textOd = "10"; textDo = "31.12"; break;
         case "1 -11 mjesec"   : textOd = "01"; textDo = "30.11"; break;
         case "1 -10 mjesec"   : textOd = "01"; textDo = "31.10"; break;
         case "1 - 9 mjesec"   : textOd = "01"; textDo = "30.09"; break;
         case "1 - 8 mjesec"   : textOd = "01"; textDo = "31.08"; break;
         case "1 - 7 mjesec"   : textOd = "01"; textDo = "31.07"; break;
         case "1 - 6 mjesec"   : textOd = "01"; textDo = "30.06"; break;
         case "1 - 5 mjesec"   : textOd = "01"; textDo = "31.05"; break;
         case "1 - 4 mjesec"   : textOd = "01"; textDo = "30.04"; break;
         case "1 - 3 mjesec"   : textOd = "01"; textDo = "31.03"; break;
         case "1 - 2 mjesec"   : textOd = "01"; textDo = mj02 + ".02"; break;
         case "Siječanj"       : textOd = "01"; textDo = "31.01"; break;
         case "Veljača"        : textOd = "02"; textDo = mj02 + ".02"; break;
         case "Ožujak"         : textOd = "03"; textDo = "31.03"; break;
         case "Travanj"        : textOd = "04"; textDo = "30.04"; break;
         case "Svibanj"        : textOd = "05"; textDo = "31.05"; break;
         case "Lipanj"         : textOd = "06"; textDo = "30.06"; break;
         case "Srpanj"         : textOd = "07"; textDo = "31.07"; break;
         case "Kolovoz"        : textOd = "08"; textDo = "31.08"; break;
         case "Rujan"          : textOd = "09"; textDo = "30.09"; break;
         case "Listopad"       : textOd = "10"; textDo = "31.10"; break;
         case "Studeni"        : textOd = "11"; textDo = "30.11"; break;
         case "Prosinac"       : textOd = "12"; textDo = "31.12"; break;
         case "Danas"          : textOd = ""  ; textDo = ""     ; break;
      }


      if(text == "Danas")
      {
         tbx_DatumOD.Text = DateTime.Today.ToString("dd.MM.yyyy");
         tbx_DatumDO.Text = DateTime.Today.ToString("dd.MM.yyyy");
      }
      else
      {
         tbx_DatumOD.Text = "01." + textOd + "." + godina;
         tbx_DatumDO.Text = textDo + "." + godina;
      }
      dtp_DatumOD.Value = DateTime.Parse(tbx_DatumOD.Text);
      dtp_DatumDO.Value = DateTime.Parse(tbx_DatumDO.Text);

   }

   #endregion Datumi_ContexMenu

   public void AnyPersonTextBoxLeave(object sender, EventArgs e)
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
            Fld_PersonCd  = person_rec.PersonCD/*RecID*/;
            Fld_Prezime   = person_rec.Prezime;
            Fld_Ime       = person_rec.Ime ;
         }
         else
         {
            Fld_PersonCdAsTxt = Fld_Prezime = Fld_Ime =  "";
         }
      }
   }

   public void AnyMtrosTextBoxLeave(object sender, EventArgs e)
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
            Fld_MtrosCD  = kupdob_rec.KupdobCD/*RecID*/;
            Fld_MtrosTK  = kupdob_rec.Ticker;
            Fld_MtrosName = kupdob_rec.Naziv;
         }
         else
         {
            Fld_MtrosCDAsTxt = Fld_MtrosTK = Fld_MtrosName = "";
         }
      }
   }

   #endregion Hampers

   #region Fld_
   
   public uint     Fld_PersonCd      
   {
      get { return ZXC.ValOrZero_UInt(tbx_PersonCd.Text); }
      set {                           tbx_PersonCd.Text = value.ToString("0000"); }
   }
   public string   Fld_PersonCdAsTxt 
   {
      get { return tbx_PersonCd.Text; }
      set {        tbx_PersonCd.Text = value; }
   }
   public string   Fld_Prezime       
   {
      get { return tbx_prezime.Text; }
      set {        tbx_prezime.Text = value; }
   }
   public string   Fld_Ime           
   {
      get { return tbx_ime.Text; }
      set {        tbx_ime.Text = value; }
   }
   public DateTime Fld_DatumOd       
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumOD.Value);
      }
      set
      {
         dtp_DatumOD.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DatumOD.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumOD);
      }
   }
   public DateTime Fld_DatumDo       
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumDO.Value);
      }
      set
      {
         dtp_DatumDO.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DatumDO.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumDO);
      }
   }
   public decimal  Fld_Km           { get { return tbx_km.GetDecimalField();           } set { tbx_km.PutDecimalField(value); } }
   public decimal  Fld_PrethBrojilo { get { return tbx_prethBrojilo.GetDecimalField(); } set { tbx_prethBrojilo.PutDecimalField(value); } }
   public uint     Fld_MtrosCD       
   {
      get { return tbx_mtros_cd.GetSomeRecIDField(); }
      set {        tbx_mtros_cd.PutSomeRecIDField(value); }
   }
   public string   Fld_MtrosCDAsTxt  
   {
      get { return tbx_mtros_cd.Text; }
      set {        tbx_mtros_cd.Text = value; }
   }
   public string   Fld_MtrosTK       
   {
      get { return tbx_mtros_tk.Text; }
      set {        tbx_mtros_tk.Text = value; }
   } 
   public string   Fld_MtrosName     
   {
      get { return tbx_mtros_Naziv.Text; }
      set {        tbx_mtros_Naziv.Text = value; }
   } 
   public string   Fld_Napomena      
   {
      get { return tbx_napomena.Text; }
      set {        tbx_napomena.Text = value; }
   }
   public bool     Fld_IsBrojilo     
   {
      get
      {
              if(rbt_brojilo.Checked) return true;
         else if(rbt_km     .Checked) return false;

         else throw new Exception("Fld_IsBrojilo: who df is checked?");
      }
      set
      {
         if(value == true) rbt_brojilo.Checked = true;
         else              rbt_km     .Checked = true;
      }
   }
   public string   Fld_VoziloCD      
   {
      get { return tbx_voziloCD.Text; }
      set {        tbx_voziloCD.Text = value; }
   }
   public string   Fld_Vozilo        
   {
      get { return tbx_vozilo.Text; }
      set {        tbx_vozilo.Text = value; }
   }
   public bool     Fld_IsPrivate { get { return cbx_isPrivate.Checked; } set { cbx_isPrivate.Checked = value; } }


   #endregion Fld_
}


public class LoadExcelPnrDLG : Crownwood.DotNetMagic.Forms.DotNetMagicForm
{
   #region Fieldz

   private ToolStripPanel    tsPanel;
   private ToolStrip         ts_excel;
   private ToolStripButton   tsb_offOpen, tsb_acceptIzv/*, tsb_previewIzv*/;
   private ToolStripMenuItem mi_Open/*, mi_prew*/;

   #endregion Fieldz

   #region Propertiez

   ZXC.ImportMode LoadRasterAction { get; set; }

   private PutRadListDUC TheDUC
   {
      get { return (PutRadListDUC)ZXC.TheVvForm.TheVvDocumentRecordUC; }
   }

   public LoadExcelPnrUC TheUC { get; private set; }

 //private Crownwood.DotNetMagic.Forms.DotNetMagicForm ThePreviewExcelForm { get; set; }

   private OpenFileDialog TheOpenFileDialog { get; set; }

   private CrystalDecisions.CrystalReports.Engine.ReportDocument TheCR_PreviewExcel    { get; set; }

   #endregion Propertiez

   #region Constructor

   public LoadExcelPnrDLG(ZXC.ImportMode _loadRasterAction)
   {
      ZXC.CurrentForm = this;

      TheUC = new LoadExcelPnrUC();

      LoadRasterAction = _loadRasterAction;

      SuspendLayout();

      this.Font        = ZXC.vvFont.BaseFont;
      this.Style       = ZXC.vvColors.vvform_VisualStyle;
      this.BackColor   = ZXC.vvColors.userControl_BackColor;
      this.MaximizeBox = false;

      this.StartPosition = FormStartPosition.Manual;

      CreateToolStripExcel();
      CreateTheUC();

      this.ClientSize = new Size(TheUC.Width, TheUC.Height + ts_excel.Height);
      this.Location   = new Point(SystemInformation.WorkingArea.Width - this.Width, 0);

      EnableDisabled_AB_CD(false);

      this.Load        += new EventHandler(LoadExcelPnrDLG_Load);
    //this.FormClosing += new FormClosingEventHandler(LoadExcelPnrDLG_FormClosing_ClosePreviewForm); 

    //?? if(this.DialogResult == DialogResult.Cancel) ThePreviewExcelForm.Close();

      this.FormClosing += new FormClosingEventHandler(TheUC.FormClosing_GetAlfaFields);

      this.FormClosing += new FormClosingEventHandler(RestoreZxcCurrentForm_FormClosing);

      #region FileDialog

      TheOpenFileDialog = new OpenFileDialog();
      TheOpenFileDialog.InitialDirectory = TheUC.Fld_DirectoryName;
      TheOpenFileDialog.Filter = "Excel files|*.xls; *.xlsx|All files (*.*)|*.*";
      TheOpenFileDialog.FilterIndex = 1;
      TheOpenFileDialog.RestoreDirectory = true;


      #endregion FileDialog

    //ShowHide_LblTbxTsBtn(); kada bi se koristio za vise namjena kao i u remonsteru

      ResumeLayout();

      VvHamper.AttachEscPressForEachControl(this);
   }

   private void ShowHide_LblTbxTsBtn()
   {
      //if(LoadRasterAction == ZXC.ImportMode.ADDREC)
      //{
         this.Text          = "Učitaj PNR";
      //}
   }

   #endregion Constructor

   #region ToolStrip+Button

   private void CreateToolStripExcel()
   {
      tsPanel           = new ToolStripPanel();
      tsPanel.Parent    = this;
      tsPanel.Dock      = DockStyle.Top;
      tsPanel.Name      = "tsPanel_Excel";
      tsPanel.BackColor = ZXC.vvColors.tsPanel_BackColor;

      ts_excel         = new ToolStrip();
      ts_excel.Parent = tsPanel;
      ts_excel.Name   = "ts_excel";
      ts_excel.ShowItemToolTips = true;
      ts_excel.GripStyle = ToolStripGripStyle.Hidden;
      ts_excel.BackColor = ZXC.vvColors.tsPanel_BackColor;

      MenuStrip menu = new MenuStrip();
      menu.Parent    = this;
      menu.Visible   = false;

      tsb_offOpen = new ToolStripButton("Otvori", VvIco.OffOpen32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.OffOpen.ico")), 32, 32)*/.ToBitmap(), new EventHandler(OpenButton_Click), "tsb_offOpen");
      tsb_offOpen.ToolTipText = "Otvori datoteku";
      ts_excel.Items.Add(tsb_offOpen);

      mi_Open = new ToolStripMenuItem("Otvori", null, new EventHandler(OpenButton_Click), Keys.Control | Keys.O);
      menu.Items.Add(mi_Open);

      tsb_acceptIzv = new ToolStripButton("Start", VvIco.Next32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.next.ico")), 32, 32)*/.ToBitmap(), new EventHandler(StartButton_Click), "tsb_acceptIzv");
      tsb_acceptIzv.ToolTipText = "Započni kreiranje Rastera";
      ts_excel.Items.Add(tsb_acceptIzv);

    
      foreach(ToolStripButton tsb in ts_excel.Items)
      {
         tsb.ImageScaling      = ToolStripItemImageScaling.None;
         tsb.DisplayStyle      = ToolStripItemDisplayStyle.ImageAndText;
         tsb.TextImageRelation = TextImageRelation.ImageAboveText;
      }
   }

   #endregion ToolStrip+Button

   #region TheUC

   private void CreateTheUC()
   {
      TheUC.Parent   = this;
      TheUC.Location = new Point(0, ts_excel.Bottom);
   }

   #endregion TheUC

   #region FormClosing + Load

   void LoadExcelPnrDLG_Load(object sender, EventArgs e)
   {
      ZXC.CurrentForm = this;
      //tsb_acceptIzv.Enabled =
      //tsb_previewIzv.Enabled = false; 
   
   }

   void RestoreZxcCurrentForm_FormClosing(object sender, FormClosingEventArgs e)
   {
      ZXC.CurrentForm = ZXC.TheVvForm;
   }

   #endregion FormClosing + Load

   #region Buttons_clik

   protected virtual void StartButton_Click(object sender, EventArgs e)
   {
      bool OK;

      if(TheUC.ThePNRraster == null)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Molim, odaberite prije Excel datoteku.");
         return;
      }

      if(ZXC.LoadImportFile_HasErrors == true)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Excel datoteka ima greske. Ponovite akciju 'Otvori'");
         return;
      }

      TheUC.ThePNRraster.voziloOK = TheUC.Fld_VoziloCD.NotEmpty();
      TheUC.ThePNRraster.personOK = TheUC.Fld_PersonCD.NotZero ();

      if(TheUC.ThePNRraster.voziloOK == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nije zadano vozilo!");
         return;
      }
      if(TheUC.ThePNRraster.personOK == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nije zadan djelatnik!");
         return;
      }
      
      if(Placa.GetDateTimeFromMMYYYY(TheUC.Fld_ZaMjesec, true).IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nije zadan mjesec u formatu MMGGGG!\n\n{0}", TheUC.Fld_ZaMjesec);
         return;
      }


      //EnableDisabled_AB_CD(true);

      Cursor.Current = Cursors.WaitCursor;

      //================================================== 
      //=== Here we go! ================================== 
      //================================================== 

      ushort line = 0;
      //uint   addRecCount = 0;
      //uint   rwtRecCount = 0;

      Mixer  mixer_rec = new Mixer();
      Xtrans xtrans_rec;

      #region Set Mixer Data

      mixer_rec.TT           = Mixer.TT_PUTN_R;
      mixer_rec.DokDate      = Placa.GetDateTimeFromMMYYYY(TheUC.Fld_ZaMjesec, true);

      mixer_rec.PersonCD     = TheUC.Fld_PersonCD  ; 
      mixer_rec.PersonPrezim = TheUC.Fld_PersonPrez; 
      mixer_rec.PersonIme    = TheUC.Fld_PersonIme ; 
      mixer_rec.StrH_32      = TheUC.Fld_VoziloCD  ; 
      mixer_rec.StrG_40      = TheUC.Fld_VoziloName; 
      mixer_rec.Napomena     = TheUC.Fld_Napomena  ;
     
      #endregion Set Mixer Data

      foreach(PNRrasterStruct PNRraster_rec in TheUC.ThePNRraster.ThePNRrasterList)
      {
         xtrans_rec = new Xtrans();

         #region Set Xtrans Data

         xtrans_rec.T_opis_128     = PNRraster_rec.Relacija  ;
         xtrans_rec.T_kol          = PNRraster_rec.KmDiff    ;
         xtrans_rec.T_intA         = PNRraster_rec.KmStart   ;
         xtrans_rec.T_intB         = PNRraster_rec.KmEnd     ;
         xtrans_rec.T_dateOd       = 
         xtrans_rec.T_dateDo       = PNRraster_rec.PutDate   ;
         xtrans_rec.T_kpdbNameB_50 = PNRraster_rec.BenzPumpa ;
         xtrans_rec.T_moneyB       = PNRraster_rec.KmOnPumpa ;
         xtrans_rec.T_moneyC       = PNRraster_rec.BenzLitara;

         #endregion Set Xtrans Data

         MixerDao.AutoSetMixer(ZXC.TheVvForm.TheDbConnection, ref line, mixer_rec, xtrans_rec);

      }

      Cursor.Current = Cursors.Default;

      this.Close();

      ZXC.TheVvForm.LastRecord_OnClick(null, EventArgs.Empty);

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo.\n\nUcitao {0} linija datoteke [{1}]\n\n", TheUC.ThePNRraster.NumOfPNRrasterFileLines, TheUC.ThePNRraster.FullPathFileName);
   }

   private void OpenButton_Click(object sender, EventArgs e)
   {

   //if(TheUC.Fld_PersonCD.IsEmpty() && 
   //   (LoadRasterAction == ZXC.ImportMode.ADDREC ||
   //      LoadRasterAction == ZXC.ImportMode.ADDREC_4_DOPUNA ||
   //      LoadRasterAction == ZXC.ImportMode.RWTREC_4_OVRHA))
   //{
   //   ZXC.aim_emsg(MessageBoxIcon.Error, "Molim, zadajte prije oznaku Rastera.");
   //   return;
   //}

   //if(TheUC.Fld_PersonPrez.IsZero() && 
   //   (LoadRasterAction == ZXC.ImportMode.RWTREC_4_OVRHA))
   //{
   //   ZXC.aim_emsg(MessageBoxIcon.Error, "Molim, zadajte prije pocetni JBBr.");
   //   return;
   //}

      if(TheOpenFileDialog.ShowDialog() == DialogResult.OK)
      {
         TheUC.Open_Load_PutFields(TheOpenFileDialog.FileName);

         //28.04.2014.
         //if(TheUC.ThePNRraster.voziloOK == false || TheUC.ThePNRraster.personOK == false)
         //{
         //   tsb_acceptIzv.Enabled = false;
         //}
      }

      TheOpenFileDialog.Dispose();
   }

#if NJETT
   private void PreviewExcelButton_Click(object sender, EventArgs e)
   {
      SuspendLayout();

      Cursor.Current = Cursors.WaitCursor;

      SetForm4Preview();

      #region Set VvPreviewReportUC

      VvStandAloneReportViewerUC thePreviewExcelUC = new VvStandAloneReportViewerUC();

      thePreviewExcelUC.Parent = ThePreviewExcelForm;
      thePreviewExcelUC.Dock   = DockStyle.Fill;

      #endregion Set VvPreviewReportUC

      ResumeLayout();

      #region FillDataset, CreateReportDocument, SetDataSource, AssignReportSource

      bool isAnalyse = (LoadRasterAction == ZXC.ImportMode.ANALYSE_Obrok_Only);

      TheUC.FillIzvodDataSet(TheUC.ThePNRraster, isAnalyse);

      if(isAnalyse) TheCR_PreviewExcel = new Remonster.Reports.CR_AnalizaExcela(); 
      else          TheCR_PreviewExcel = new Remonster.Reports.CR_PreviewExcel();

      thePreviewExcelUC.SetDataSource_And_AssignReportSource(TheCR_PreviewExcel, TheUC.TheDS_DbtReport);

      #endregion FillDataset, CreateReportDocument, SetDataSource, AssignReportSource

      ZXC.TheVvForm.ToolStripReportVisible_tsBtnOnTsReportVisible(false, ZXC.ReportMode.Done, false);

      Cursor.Current = Cursors.Default;
   }

   private void SetForm4Preview()
   {
      ThePreviewExcelForm = new Crownwood.DotNetMagic.Forms.DotNetMagicForm();

      ThePreviewExcelForm.Show();

      tsb_previewIzv.Enabled = false;

      ThePreviewExcelForm.Font      = ZXC.vvFont.BaseFont;
      ThePreviewExcelForm.Style     = ZXC.vvColors.vvform_VisualStyle;
      ThePreviewExcelForm.BackColor = ZXC.vvColors.userControl_BackColor;


      ThePreviewExcelForm.FormClosing += new FormClosingEventHandler(ThePreviewExcelForm_FormClosing_EnabledPreviewButton);

      //this.Location                = new Point(0, 0);
      //ThePreviewExcelForm.Location = new Point(this.Right, 0);
      ThePreviewExcelForm.Location = Point.Empty;
      ThePreviewExcelForm.Size     = new Size(SystemInformation.WorkingArea.Width - this.Width, SystemInformation.WorkingArea.Height);
      this.Location                = new Point(ThePreviewExcelForm.Right, 0);
   }


#endif

   private void AbortExcelButton_Click(object sender, EventArgs e)
   {
      this.Close();

      ZXC.TheVvForm.EscapeAction_OnClick(this, EventArgs.Empty);
   }

   #endregion Buttons_clik

   #region Methods

   private void EnableDisabled_AB_CD(bool isIzvAccept)
   {
      tsb_acceptIzv.Enabled = !isIzvAccept;
      tsb_offOpen.Enabled   = mi_Open.Enabled = !isIzvAccept;

      if(isIzvAccept)
      {
         VvHamper.Open_Close_Fields_ForWriting(TheUC.panel_Datoteka, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      }
      else
      {
         VvHamper.Open_Close_Fields_ForWriting(TheUC.panel_Datoteka, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
      }
   }

   #endregion Methods

}

public class LoadExcelPnrUC : VvOtherUC
{
   #region Fieldz

   public  Panel     panel_Datoteka, panel_ExcelInfo;
   private VvHamper  hamp_Datoteka, hamp_Raster, hamp_ExcelInfo;
   private int       dlgWidth, dlgHeight, nextX, nextY, razmak;

   public VvTextBox tbx_fileName, tbx_directoryName, tbx_personCD, tbx_napomena, tbx_ukStavaka, tbx_SheetName, tbx_personPrez,
                    tbx_voziloCD, tbx_voziloName, tbx_personIme, tbx_zaMjesec;
   public Label lbRas, lbJB, lbJBName, lbOvrYear;

   #endregion Fieldz

   #region Constructor

   public LoadExcelPnrUC()
   {

      #region UC Initialization

      SuspendLayout();

      CreatePanelsForHampers();

      nextX = nextY = ZXC.Qun4;
      razmak        = ZXC.QUN;
      InitializeHamperDatoteka(out hamp_Datoteka);

      nextX = hamp_Datoteka.Right ;
      InitializeHamperRaster(out hamp_Raster);

      nextX = ZXC.QunMrgn;
      nextY = ZXC.Qun4;
    //InitializeHamperExcelInfo(out hamp_ExcelInfo);

    //nextY = ZXC.Qun4;
      LocationAndSize_PanelsForhampers_AndClientSize();

      SendKeys.Send("{TAB}");

      ResumeLayout();

      #endregion UC Initialization

      PutAlfaFields(ZXC.TheVvForm.VvPref.loadPNRExcelPrefs);

   }

   #endregion Constructor

   #region Propertiez

 //  public Remonster.DataLayer.DS_Reports.DS_DbtReport TheDS_DbtReport { get; set; }

   public PNRraster ThePNRraster { get; set; }

   private VvDaoBase TheVvDaoTrans
   {
      get { return ZXC.XtransDao; }
   }

   private PutRadListDUC TheDUC
   {
      get { return (PutRadListDUC)ZXC.TheVvForm.TheVvDocumentRecordUC; }
   }

   private XtransDao.XtransCI DB_Tci
   {
      get { return ZXC.XtrCI; }
   }

   public int CurrentTransRowIdx { get; set; }

   #endregion Propertiez

   #region Panels

   #region PanelsForHampers

   private void CreatePanelsForHampers()
   {
      panel_Datoteka  = new Panel();
//    panel_ExcelInfo = new Panel();

      panel_Datoteka.Parent = this;
//    panel_ExcelInfo.Parent = this;

      panel_Datoteka.BorderStyle = BorderStyle.FixedSingle;
//    panel_ExcelInfo.BorderStyle = BorderStyle.FixedSingle;

   }

   private void LocationAndSize_PanelsForhampers_AndClientSize()
   {
      int razmak = ZXC.Qun8;

      dlgWidth = hamp_Datoteka.Width + hamp_Raster.Width + ZXC.QunMrgn;

      panel_Datoteka.Size  = new Size(dlgWidth, hamp_Raster.Bottom + nextY);
    //panel_ExcelInfo.Size = new Size(dlgWidth, hamp_ExcelInfo.Bottom + nextY);

      panel_Datoteka.Location  = new Point(0, 0);
    //panel_ExcelInfo.Location = new Point(0, panel_Datoteka.Bottom + razmak * 3);

    //dlgHeight = panel_ExcelInfo.Bottom;
      dlgHeight = panel_Datoteka.Bottom + ZXC.QunMrgn;
      this.Size = new Size(dlgWidth, dlgHeight);

    //VvHamper.Open_Close_Fields_ForWriting(panel_ExcelInfo, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);

    //panel_ExcelInfo.BackColor = Color.FromArgb(255, 224, 192);
   }

   #endregion PanelsForHampers

   #region panel_Datoteka

   private void InitializeHamperDatoteka(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 3, "", panel_Datoteka, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q4un, ZXC.Q4un, ZXC.Q4un, ZXC.Q4un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.QUN , ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] {ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Datoteka:"    , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 1, "Directory:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 2, "SheetName:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 2, "Broj stavaka:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(4, 2, "Za mjesec:"   , ContentAlignment.MiddleRight);

      tbx_fileName      = hamper.CreateVvTextBox(1, 0, "tbx_fileName"     , "", 32, 4, 0);
      tbx_directoryName = hamper.CreateVvTextBox(1, 1, "tbx_directoryName", "", 32, 4, 0);
      tbx_SheetName     = hamper.CreateVvTextBox(1, 2, "tbx_SheetName"    , ""          );
      tbx_ukStavaka     = hamper.CreateVvTextBox(3, 2, "tbx_ukStavaka"    , "");
      tbx_zaMjesec      = hamper.CreateVvTextBox(5, 2, "tbx_za Mjesec"    , "");

      tbx_ukStavaka.JAM_ReadOnly = true;
      tbx_directoryName.JAM_ReadOnly = true;
      
   }

   private void InitializeHamperRaster(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 3, "", panel_Datoteka, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q2un, ZXC.Q6un, ZXC.Q6un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun4, ZXC.Qun8 , ZXC.Qun4  };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Djelatnik:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 1, "Vozilo:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 2, "Napomena:" , ContentAlignment.MiddleRight);

      tbx_personCD   = hamper.CreateVvTextBox(1, 0, "tbx_personCD"  , "", 6);
      tbx_personPrez = hamper.CreateVvTextBox(2, 0, "tbx_personPrez", "");
      tbx_personIme  = hamper.CreateVvTextBox(3, 0, "tbx_personIme" , "");
      tbx_voziloCD   = hamper.CreateVvTextBoxLookUp("tbx_voziloCD", 1, 1,"", 32, 1, 0);
      tbx_voziloName = hamper.CreateVvTextBox(3, 1, "tbx_voziloName", "");
      tbx_napomena   = hamper.CreateVvTextBox(1, 2, "tbx_napomena"  , "", 64, 2, 0);

      tbx_personCD  .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_personCD  .JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType     , new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterSifra)  , new EventHandler(AnyPersonTextBoxLeave));
      tbx_personPrez.JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonTextBoxLeave));
      
      tbx_personIme.JAM_ReadOnly = true;

      tbx_voziloCD.JAM_Set_LookUpTable(ZXC.luiListaMixerVozilo, (int)ZXC.Kolona.prva);
      tbx_voziloCD.JAM_lookUp_NOTobligatory = true;
      tbx_voziloCD.JAM_lui_NameTaker_JAM_Name = tbx_voziloName.JAM_Name;

    }
  
   public void AnyPersonTextBoxLeave(object sender, EventArgs e)
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
            Fld_PersonCD   = person_rec.PersonCD/*RecID*/;
            Fld_PersonPrez = person_rec.Prezime;
            Fld_PersonIme  = person_rec.Ime ;
         }
         else
         {
            Fld_PersonCdAsTxt = Fld_PersonPrez = Fld_PersonIme = "";
         }
      }
   }

   #endregion panel_Datoteka

   #region panel_ExcelInfo

//private void InitializeHamperExcelInfo(out VvHamper hamper)
//{
//   hamper = new VvHamper(3, 2, "", panel_ExcelInfo, false, nextX, nextY, 0);

//   hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un, ZXC.Q2un };
//   hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
//   hamper.VvRightMargin = hamper.VvLeftMargin;

//   hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN};
//   hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
//   hamper.VvBottomMargin = hamper.VvTopMargin;

//   Label lbstv = hamper.CreateVvLabel(0, 0, "Broj stavaka:"    , ContentAlignment.MiddleRight);

//   tbx_ukStavaka = hamper.CreateVvTextBox(1, 0, "tbx_ukStavaka", "");

//   tbx_ukStavaka.JAM_ReadOnly = true;
//}

   #endregion panel_ExcelInfo

   #endregion Panels

   #region Fld_

   public string Fld_FileName       { get { return tbx_fileName     .Text; } set { tbx_fileName     .Text = value; } }
   public string Fld_DirectoryName  { get { return tbx_directoryName.Text; } set { tbx_directoryName.Text = value; } }
   public string Fld_Napomena       { get { return tbx_napomena     .Text; } set { tbx_napomena     .Text = value; } }
   public string Fld_SheetName      { get { return tbx_SheetName    .Text; } set { tbx_SheetName    .Text = value; } }
   public string Fld_PersonPrez     { get { return tbx_personPrez   .Text; } set { tbx_personPrez   .Text = value; } }
   public string Fld_PersonIme      { get { return tbx_personIme    .Text; } set { tbx_personIme    .Text = value; } }
   public string Fld_VoziloCD       { get { return tbx_voziloCD     .Text; } set { tbx_voziloCD     .Text = value; } }
   public string Fld_VoziloName     { get { return tbx_voziloName   .Text; } set { tbx_voziloName   .Text = value; } }

   public uint   Fld_PersonCD       { get { return ZXC.ValOrZero_UInt(tbx_personCD.Text); } set { tbx_personCD .Text = value.ToString(); } }
   public string Fld_PersonCdAsTxt  { get { return tbx_personCD .Text                   ; } set { tbx_personCD .Text = value           ; } }
   public uint   Fld_UkStavaka      {                                                       set { tbx_ukStavaka.Text = value.ToString(); } }
   public string Fld_ZaMjesec       { get { return tbx_zaMjesec .Text                   ; } set { tbx_zaMjesec .Text = value           ; } }

   #endregion Fld_

   #region PutFields(), GetFields()

   private void PutAlfaFields(VvPref.LoadPNRExcelPrefs loadPNRPrefs)
   {
      Fld_DirectoryName = loadPNRPrefs.DirectoryName;
      Fld_SheetName     = loadPNRPrefs.SheetName;
   }

   public void FormClosing_GetAlfaFields(object sender, FormClosingEventArgs e)
   {
      ZXC.TheVvForm.VvPref.loadPNRExcelPrefs.DirectoryName = Fld_DirectoryName;
      ZXC.TheVvForm.VvPref.loadPNRExcelPrefs.SheetName     = Fld_SheetName;
   }

   private void PutSelectedRasterFileNameFields(PNRraster PNRraster_rec)
   {
      Fld_FileName      = PNRraster_rec.FileName;
      Fld_DirectoryName = PNRraster_rec.DirectoryName;

      Fld_UkStavaka     = (uint)PNRraster_rec.NumOfPNRrasterFileLines;
      Fld_ZaMjesec      =       PNRraster_rec.dateRasterMMYYYY       ;

      if(PNRraster_rec.personOK)
      {
         Fld_PersonCD   = PNRraster_rec.thePerson.PersonCD;
         Fld_PersonIme  = PNRraster_rec.thePerson.Ime;
         Fld_PersonPrez = PNRraster_rec.thePerson.Prezime;
      }
      else
      {
         Fld_PersonCD   = 0;
         Fld_PersonIme  = 
         Fld_PersonPrez = "";
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ne postoji djelatnik sa sifrom\n\n{0}", PNRraster_rec.personCD);
         
      }

      if(PNRraster_rec.voziloOK)
      {
         Fld_VoziloCD   = PNRraster_rec.theVoziloLUI.Cd;
         Fld_VoziloName = PNRraster_rec.theVoziloLUI.Name;
      }
      else
      {
         Fld_VoziloCD   = 
         Fld_VoziloName = "";
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ne postoji vozilo sa registracijom\n\n{0}", PNRraster_rec.voziloCD);
      }

      this.Text = "Učitaj Raster [" + ThePNRraster.FileName + "]";
   }

   //private void PutFields()
   //{
   //   //Fld_DirectoryName = 
   //   //Fld_Raster        = 
   //   //Fld_Napomena      = 
   //   //Fld_UkStavaka =
   //   //Fld_UkDug     =

   //}

   //public void FormClosing_GetFields(object sender, FormClosingEventArgs e)
   //{
   //   ZXC.TheVvForm.VvPref.dbtExcel.DirectoryName = Fld_A_DirectoryName;
   //   ZXC.TheVvForm.VvPref.dbtExcel.KontoZiro     = Fld_A_Raster;
   //   ZXC.TheVvForm.VvPref.dbtExcel.NepozDug      = Fld_A_Napomena;
   //}


   #endregion PutFields(), GetFields()

   #region Methods BEFORE Start Operation

   public bool Open_Load_PutFields(string fullPathFileName)
   {
      ThePNRraster = new PNRraster(fullPathFileName, Fld_SheetName);

      if(ThePNRraster.BadData)
      {
         VvSQL.ReportGenericError("OPEN FILE", string.Format("Datoteka\n\n{0}\n\nNe izgleda kao PutniRadniList 'Excel' datoteka!", fullPathFileName), System.Windows.Forms.MessageBoxButtons.OK);
         return false;
      }
      else
      {
         PutSelectedRasterFileNameFields(ThePNRraster);
         //PutBetaFields(TheMixerUplata);
         return true;
      }
   }

   #endregion Methods BEFORE Start Operation

#if NJETT

   #region FillDataset

   public void FillIzvodDataSet(PNRraster _thePNRraster, bool isAnalysing)
   {
      if(_thePNRraster == null) return;

      TheDS_DbtReport = new Remonster.DataLayer.DS_Reports.DS_DbtReport();

      int excelRowIdx = 1;

      foreach(Mixer mixer_rec in ThePNRraster.TheMixerList)
      {
         excelRowIdx++;

         if(isAnalysing && ThisMixerIsNotInteresting(mixer_rec.CustCode)) continue;

         #region Mixers

         DS_DbtReport.mixerRow mixerRow = (DS_DbtReport.mixerRow)TheDS_DbtReport.mixer.Rows.Add(

            mixer_rec.RecID,
            mixer_rec.AddTS,
            mixer_rec.ModTS,
            mixer_rec.AddUID,
            mixer_rec.ModUID,
            mixer_rec.DokNum,
            mixer_rec.DokDate,
            mixer_rec.TT,
            mixer_rec.TtNum,
            mixer_rec.Napomena,
            mixer_rec.FlagA,
            mixer_rec.FName,
            mixer_rec.Status,
            mixer_rec.TotalDug,
            mixer_rec.CustCode,
            mixer_rec.CustID,
            mixer_rec.Ugovori,
            mixer_rec.CIme,
            mixer_rec.CPrezime,
            mixer_rec.CNaziv,
            mixer_rec.CAdresa,
            mixer_rec.CPostBr,
            mixer_rec.CGrad,
            mixer_rec.BImePrezime,
            mixer_rec.BNaziv,
            mixer_rec.BAdresa,
            mixer_rec.BPostBr,
            mixer_rec.BGrad,
            mixer_rec.UIme,
            mixer_rec.UPrezime,
            mixer_rec.UNaziv,
            mixer_rec.UAdresa,
            mixer_rec.UPostBr,
            mixer_rec.UGrad,
            mixer_rec.MatBr,
            mixer_rec.Msisdn,
            mixer_rec.Contact1,
            mixer_rec.Contact2,
            mixer_rec.DatumUplate,
            mixer_rec.IznosUplate,
            mixer_rec.IsObrok,
            mixer_rec.IsBadAddress,
            mixer_rec.Op1Status,
            mixer_rec.Op2Status,
            mixer_rec.OvrStatus,
            mixer_rec.Ziro,
            mixer_rec.Banka
         );

         mixerRow.S_excelRowIdx = excelRowIdx;

         #endregion Mixers

         #region Dtranses

         foreach(Dtrans xtrans_rec in mixer_rec.Transes)
         {
            TheDS_DbtReport.IzvjTable.Rows.Add(
               xtrans_rec.T_parentID,  
               xtrans_rec.T_dokNum,    
               xtrans_rec.T_serial,    
               xtrans_rec.T_dokDate,   
               xtrans_rec.T_TT,        
               xtrans_rec.T_ttNum,     
               xtrans_rec.T_razdob  ,  
               xtrans_rec.T_brojRn  ,  
               xtrans_rec.T_valuta  ,  
               xtrans_rec.T_iznosRn ,  
               xtrans_rec.T_iznosDug,  
               xtrans_rec.T_iznosPla
            );
         }

         #endregion Dtranses
      }

      Fld_UkStavaka = (uint)ThePNRraster.NumOfVipRasterFileLines;
   }

   private bool ThisMixerIsNotInteresting(string customerCode)
   {
      Mixer mixer_rec = new Mixer();

      bool found = ZXC.MixerDao.SetMe_Record_bySomeUniqueColumn(ZXC.TheVvForm.TheDbConnection, mixer_rec, customerCode, ZXC.MixerSchemaRows[ZXC.DbtCI.custCode], false, true);

      if(!found) return true;

      return (mixer_rec.IsObrok == false); // zelimo dobiti u report SAMO one koji imaju odobreno obrocno 
   }

   #endregion FillDataset

#endif
}
