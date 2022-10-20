using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;


public partial class ZLJ_DUC : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_kupDobCd, tbx_kupDobTk, tbx_kupDobName, /* Ovlastena zdravstvena ustanova */
                     tbx_PersonCd, tbx_prezime , tbx_ime       , /* Zastitar                       */
                     //tbx_datum                                 , /* Datum obavljenog pregleda      */
                     //tbx_dateA                                 , /* Datum eventualno ponovljenog   */
                     tbx_dateB                                 , /* Datum izdavanja potvrde        */
                     tbx_strG_40                               ; /* Broj potvrde   */
   public VvDateTimePicker dtp_dateB;
   public VvHamper   hamp_ZLJ, hamp_dateB, hamp_partner, hamp_person, hamp_veze;
   
   #endregion Fieldz

   #region Constructor

   public ZLJ_DUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul)
      : base(parent, _mixer, vvSubModul)
   {

      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_ZLJ
         });

   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
      hamp_tt     .Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn                  );
      
      nextX = ZXC.QunMrgn;
      nextY = hamp_tt.Bottom + ZXC.Qun4;
      InitializeHamper_Person(out hamp_person);
            
      hamp_dokDate.Location = new Point(ZXC.QunMrgn, hamp_person.Bottom + ZXC.Qun4);
      
      nextX = hamp_dokDate.Right + ZXC.Qun2;
      nextY = hamp_dokDate.Top;
      InitializeHamper_DateB(out hamp_dateB);

      nextY = hamp_dokDate.Bottom;
      nextX = ZXC.QunMrgn;
      InitializeHamper_Partner(out hamp_partner);

  
      nextY = hamp_partner.Bottom + ZXC.Qun8;

      InitializeHamper_ZLJ(out hamp_ZLJ);

      nextY = hamp_person .Top - ZXC.Qun4;
      nextX = hamp_partner.Right + ZXC.QUN;
      InitializeHamper_Veze(out hamp_veze);


      hamp_dokNum.Location = new Point(hamp_veze.Right - ZXC.Q2un - hamp_dokNum.Width + ZXC.Qun2, ZXC.QunMrgn);

      hamp_napomena.Location = new Point(ZXC.QunMrgn, hamp_ZLJ.Bottom);
      nextY = hamp_napomena.Bottom;

      //tbx_prezime   .TabIndex = 0;
      //tbx_DokDate   .TabIndex = 1;
      //tbx_kupDobName.TabIndex = 2;
      //tbx_dateB     .TabIndex = 3;
      //tbx_strG_40   .TabIndex = 4;
      //tbx_Napomena  .TabIndex = 5;

   }

   private void InitializeHamper_DateB(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q10un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                  hamper.CreateVvLabel  (0, 0, "Ponovni pregled:", ContentAlignment.MiddleRight);
      tbx_dateA = hamper.CreateVvTextBox(1, 0, "tbx_dateA", "Ponovni pregled");
      tbx_dateA.JAM_IsForDateTimePicker = true;
      dtp_dateA = hamper.CreateVvDateTimePicker(1, 0, "", tbx_dateA);
      dtp_dateA.Name = "dtp_dateA";

   }


   public void InitializeHamper_ZLJ(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0         1        2         3     
      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q4un, ZXC.Q8un - ZXC.Qun4, ZXC.Q7un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      
                      hamper.CreateVvLabel  (0, 0,      "Potvrda izdana:", ContentAlignment.MiddleRight);
      tbx_dateB     = hamper.CreateVvTextBox(1, 0, "tbx_dateB", "Datum izdavanja potvrde" , GetDB_ColumnSize(DB_ci.dateB));
      tbx_dateB.JAM_IsForDateTimePicker = true;
      dtp_dateB                    = hamper.CreateVvDateTimePicker(1, 0, "", tbx_dateB);
      dtp_dateB.Name = "dtp_dateA";
   
                     hamper.CreateVvLabel  (2, 0,                "Broj:", ContentAlignment.MiddleRight);
      tbx_strG_40  = hamper.CreateVvTextBox(3, 0, "tbx_UstrJed", "Broj potvrde/uvjerenja" , GetDB_ColumnSize(DB_ci.strG_40));
   }
   
   public void InitializeHamper_Person(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0        1          2         3   
      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un - ZXC.Qun2, ZXC.Q8un, ZXC.Q8un + ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4};
      hamper.VvBottomMargin = hamper.VvTopMargin;

                     hamper.CreateVvLabel  (0, 0, "Zaštitar:", ContentAlignment.MiddleRight);
      tbx_PersonCd = hamper.CreateVvTextBox(1, 0, "tbx_PersonCd", "Šifra"  , GetDB_ColumnSize(DB_ci.personCD)); 
      tbx_prezime  = hamper.CreateVvTextBox(2, 0, "tbx_prezime" , "Prezime", GetDB_ColumnSize(DB_ci.personPrezim));
      tbx_ime      = hamper.CreateVvTextBox(3, 0, "tbx_ime"     , "Ime"    , GetDB_ColumnSize(DB_ci.personIme));
      tbx_ime.JAM_ReadOnly = true;
      
      tbx_PersonCd.JAM_CharEdits       = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_prezime .JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_prezime.Font = ZXC.vvFont.LargeBoldFont;

      SetSifrarAndAutocomplete<Person>(tbx_prezime, VvSQL.SorterType.Person);

      tbx_PersonCd.JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterSifra), new EventHandler(AnyPersonTextBoxLeave));
      tbx_prezime .JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonTextBoxLeave));

      this.ControlForInitialFocus = tbx_prezime;
   }
  
   private void InitializeHamper_Partner(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0                 1                
      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un, ZXC.Q3un, ZXC.Q10un + ZXC.Q3un - ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4            };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel  (0, 0, "Ovlaštena ustanova:", ContentAlignment.MiddleRight);
      tbx_kupDobCd   = hamper.CreateVvTextBox(1, 0, "tbx_kupDobCd"  , "Sifra Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobCD)  );
      tbx_kupDobTk   = hamper.CreateVvTextBox(2, 0, "tbx_kupDobTk"  , "Tiker Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobTK)  );
      tbx_kupDobName = hamper.CreateVvTextBox(3, 0, "tbx_kupDobName", "Naziv Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobName));

      tbx_kupDobCd.JAM_CharEdits       = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_kupDobTk.JAM_CharacterCasing = CharacterCasing.Upper;

      //this.ControlForInitialFocus = tbx_kupDobName;

      tbx_kupDobCd  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobTk  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyKupdobTextBoxLeave));
   }

   private void InitializeHamper_Veze(out VvHamper hamper)
   {
    //hamper = new VvHamper(11, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      hamper = new VvHamper(6, 4, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      //                                     0                          1              2                3                4                   5                   6              7        8            9           10       
    //hamper.VvColWdt      = new int[] { ZXC.Q5un           , ZXC.Q3un - ZXC.Qun4, ZXC.Q2un, ZXC.Q7un+ ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.Q2un + ZXC.Qun8, ZXC.Q3un- ZXC.Qun4 , ZXC.Q6un, ZXC.Q3un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4 };
    //hamper.VvSpcBefCol   = new int[] { ZXC.Qun4           ,            ZXC.Qun4, ZXC.Qun4,           ZXC.Qun4,                  0,                  0,            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,                  0 };
      hamper.VvColWdt      = new int[] { ZXC.Q2un + ZXC.Qun8, ZXC.Q3un- ZXC.Qun4 , ZXC.Q6un, ZXC.Q3un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] {            ZXC.Qun4, ZXC.Qun4           , ZXC.Qun4, ZXC.Qun4           ,                  0,                  0 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun2;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      
                        hamper.CreateVvLabel  (0, 0, "Link1:", ContentAlignment.MiddleRight);
      tbx_externLink1 = hamper.CreateVvTextBox(1, 0, "tbx_externLink1", "Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.externLink1), 2, 0);
      btn_goExLink1   = hamper.CreateVvButton (4, 0, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink1.Name = "btn_goExLink1";
      btn_goExLink1.FlatStyle = FlatStyle.Flat;
      btn_goExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink1.Image = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink1.Tag = 1;
      btn_goExLink1.TabStop = false;
      btn_goExLink1.Visible = false;

      btn_openExLink1 = hamper.CreateVvButton(5, 0, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLink1.Name = "btn_openExLink1";
      btn_openExLink1.FlatStyle = FlatStyle.Flat;
      btn_openExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLink1.Image = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLink1.Tag = 1;
      btn_openExLink1.TabStop = false;

                        hamper.CreateVvLabel  (0, 1, "Link2:", ContentAlignment.MiddleRight);
      tbx_externLink2 = hamper.CreateVvTextBox(1, 1, "tbx_externLink2", "Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.externLink2), 2, 0);
      btn_goExLink2   = hamper.CreateVvButton (4, 1, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink2.Name = "btn_goExLink2";
      btn_goExLink2.FlatStyle = FlatStyle.Flat;
      btn_goExLink2.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink2.Image = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink2.Tag = 2;
      btn_goExLink2.TabStop = false;
      btn_goExLink2.Visible = false;

      btn_openExLink2 = hamper.CreateVvButton(5, 1, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLink2.Name = "btn_openExLink2";
      btn_openExLink2.FlatStyle = FlatStyle.Flat;
      btn_openExLink2.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLink2.Image = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLink2.Tag = 2;
      btn_openExLink2.TabStop = false;

                      hamper.CreateVvLabel        ( 0, 2, "Veza1:", ContentAlignment.MiddleRight);
      tbx_v1_tt     = hamper.CreateVvTextBoxLookUp( 1, 2, "tbx_v1_tt", "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v1_tt));
      tbx_v1_ttOpis = hamper.CreateVvTextBox      ( 2, 2, "tbx_v1_ttOpis", "Veza na interni dokument", 32);
      tbx_v1_ttNum  = hamper.CreateVvTextBox      ( 3, 2, "tbx_v1_ttNum", "Veza na interni dokument", 6/*GetDB_ColumnSize(DB_ci.iz_ttNum)*/);
      btn_v1TT      = hamper.CreateVvButton       ( 5, 2, new EventHandler(GoTo_MIXER_Dokument_Click), "");

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

                      hamper.CreateVvLabel        (0, 3, "Veza2:", ContentAlignment.MiddleRight);
      tbx_v2_tt     = hamper.CreateVvTextBoxLookUp(1, 3, "tbx_v2_tt", "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v2_tt));
      tbx_v2_ttOpis = hamper.CreateVvTextBox      (2, 3, "tbx_v2_ttOpis", "Veza na interni dokument");
      tbx_v2_ttNum  = hamper.CreateVvTextBox      (3, 3, "tbx_v2_ttNum", "Veza na interni dokument" + " broj", 6/*GetDB_ColumnSize(DB_ci.na_ttNum)*/);
      btn_v2TT      = hamper.CreateVvButton       (5, 3, new EventHandler(GoTo_MIXER_Dokument_Click), "");
      btn_v2TT.Name = "v2_TT";

      btn_v2TT.FlatStyle = FlatStyle.Flat;
      btn_v2TT.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_v2TT.Image = VvIco.TriangleBlue16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue2.ico")), 16, 16)*/.ToBitmap();
      btn_v2TT.Tag = 2;
      btn_v2TT.TabStop = false;

      tbx_v2_tt.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_v2_tt.JAM_Set_NOTobligatory_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_v2_tt.JAM_lui_NameTaker_JAM_Name = tbx_v2_ttOpis.JAM_Name;
      tbx_v2_ttOpis.JAM_ReadOnly = true;

      tbx_v2_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      hamper.BringToFront();
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
            //Fld_RadMjesto = person_rec.RadMj;
         }
         else
         {
            Fld_PersonCdAsTxt = Fld_Prezime = Fld_Ime = /*Fld_RadMjesto =*/ "";
         }
      }
   }

   void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      //if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.originalText)
      {
         originalText = tb.Text;
         kupdob_rec = VvUserControl.KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_KupDobName = kupdob_rec.Naziv;
            Fld_KupDobCd = kupdob_rec.KupdobCD/*RecID*/;
            Fld_KupDobTk = kupdob_rec.Ticker;
         }
         else
         {
            Fld_KupDobName = Fld_KupDobTk = Fld_KupDobCdAsTxt = "";
         }
      }
   }

   #endregion CreateSpecificHampers()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_opis_128_CreateColumn(0, "Komentar", "Komentar", 128, null);
   }

   #endregion InitializeTheGrid_Columns()

   #region Fld_

   public uint    Fld_KupDobCd      { get { return tbx_kupDobCd.GetSomeRecIDField();       } set { tbx_kupDobCd.PutSomeRecIDField(value);        } }
   public string  Fld_KupDobCdAsTxt { get { return tbx_kupDobCd.Text;                      } set { tbx_kupDobCd  .Text = value;                  } }
   public string  Fld_KupDobName    { get { return tbx_kupDobName.Text;                    } set { tbx_kupDobName.Text = value;                  } }
   public string  Fld_KupDobTk      { get { return tbx_kupDobTk.Text;                      } set { tbx_kupDobTk  .Text = value;                  } }
   public uint    Fld_PersonCd      { get { return ZXC.ValOrZero_UInt(tbx_PersonCd.Text) ; } set { tbx_PersonCd  .Text = value.ToString("0000"); } }
   public string  Fld_PersonCdAsTxt { get { return tbx_PersonCd .Text                    ; } set { tbx_PersonCd  .Text = value                 ; } }
   public string  Fld_Prezime       { get { return tbx_prezime  .Text                    ; } set { tbx_prezime   .Text = value                 ; } }
   public string  Fld_Ime           { get { return tbx_ime      .Text                    ; } set { tbx_ime       .Text = value                 ; } }
              
   public string  Fld_StrG_40       { get { return tbx_strG_40 .Text;                } set { tbx_strG_40  .Text = value;            } }
   
   public DateTime Fld_DateB        
   {
      get { return dtp_dateB.Value; }
      set
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dtp_dateB.Value = value;
         }
      }
   }

   #endregion _Fld

   #region override PutSpecificsFld() GetSpecificsFld()

   /*protected*/public override void PutSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd  )) Fld_KupDobCd   = mixer_rec.KupdobCD    ;
      if(CtrlOK(tbx_kupDobName)) Fld_KupDobName = mixer_rec.KupdobName  ;
      if(CtrlOK(tbx_kupDobTk  )) Fld_KupDobTk   = mixer_rec.KupdobTK    ;
      if(CtrlOK(tbx_PersonCd  )) Fld_PersonCd   = mixer_rec.PersonCD    ;
      if(CtrlOK(tbx_prezime   )) Fld_Prezime    = mixer_rec.PersonPrezim;
      if(CtrlOK(tbx_ime       )) Fld_Ime        = mixer_rec.PersonIme   ;

      
      if(CtrlOK(tbx_ProjektCD )) Fld_ProjektCD  = mixer_rec.ProjektCD;
      Fld_TtOpis = "LIJEČNIČKI PREGLED";

      if(CtrlOK(tbx_v1_tt      )) Fld_V1_tt       = mixer_rec.V1_tt;
      if(CtrlOK(tbx_v1_ttOpis  )) Fld_V1_ttOpis   = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V1_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v1_ttNum   )) Fld_V1_ttNum    = mixer_rec.V1_ttNum;
      if(CtrlOK(tbx_v2_tt      )) Fld_V2_tt       = mixer_rec.V2_tt;
      if(CtrlOK(tbx_v2_ttOpis  )) Fld_V2_ttOpis   = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V2_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v2_ttNum   )) Fld_V2_ttNum    = mixer_rec.V2_ttNum;
      if(CtrlOK(tbx_externLink1)) Fld_ExternLink1 = mixer_rec.ExternLink1;
      if(CtrlOK(tbx_externLink2)) Fld_ExternLink2 = mixer_rec.ExternLink2;

      if(CtrlOK(tbx_strG_40 )) Fld_StrG_40        = mixer_rec.StrG_40  ;
      if(CtrlOK(tbx_dateB   )) Fld_DateB          = mixer_rec.DateB;
   }

   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd   )) mixer_rec.KupdobCD     = Fld_KupDobCd  ;
      if(CtrlOK(tbx_kupDobName )) mixer_rec.KupdobName   = Fld_KupDobName;
      if(CtrlOK(tbx_kupDobTk   )) mixer_rec.KupdobTK     = Fld_KupDobTk  ;
      if(CtrlOK(tbx_PersonCd   )) mixer_rec.PersonCD     = Fld_PersonCd  ;
      if(CtrlOK(tbx_prezime    )) mixer_rec.PersonPrezim = Fld_Prezime   ;
      if(CtrlOK(tbx_ime        )) mixer_rec.PersonIme    = Fld_Ime       ;

      if(CtrlOK(tbx_ProjektCD  )) mixer_rec.ProjektCD   = Fld_ProjektCD;
      if(CtrlOK(tbx_v1_tt      )) mixer_rec.V1_tt       = Fld_V1_tt;
      if(CtrlOK(tbx_v1_ttNum   )) mixer_rec.V1_ttNum    = Fld_V1_ttNum;
      if(CtrlOK(tbx_v2_tt      )) mixer_rec.V2_tt       = Fld_V2_tt;
      if(CtrlOK(tbx_v2_ttNum   )) mixer_rec.V2_ttNum    = Fld_V2_ttNum;
      if(CtrlOK(tbx_externLink1)) mixer_rec.ExternLink1 = Fld_ExternLink1;
      if(CtrlOK(tbx_externLink2)) mixer_rec.ExternLink2 = Fld_ExternLink2;

      if(CtrlOK(tbx_strG_40    )) mixer_rec.StrG_40     = Fld_StrG_40   ;
      if(CtrlOK(tbx_dateB      )) mixer_rec.DateB       = Fld_DateB     ;

   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region PrintDocumentRecord

   public ZLJFilterUC TheZLJFilterUC { get; set; }
   public ZLJFilter TheZLJFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.TheZLJFilter = new ZLJFilter(this);

      TheZLJFilterUC = new ZLJFilterUC(this);
      TheZLJFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheZLJFilterUC.Width;

   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      //ZLJFilter mixerFilter = (ZLJFilter)vvRptFilter;
      EvidencijaFilter mixerFilter = (EvidencijaFilter)vvRptFilter;

      //switch(mixerFilter.PrintZLJ)
      switch(mixerFilter.PrintEvidencija)
      {
         //case ZLJFilter.PrintZLJEnum.ZLJ: specificMixerReport = new RptX_ZLJ(new Vektor.Reports.XIZ.CR_ZLJ_DUC(), "ZLJ", mixerFilter); break;
         case EvidencijaFilter.PrintEvidencijaEnum.Evidencija: specificMixerReport = new RptX_Evidencija(new Vektor.Reports.XIZ.CR_EvidencijaDUC(), "ZLJ", mixerFilter); break;

         default: ZXC.aim_emsg("{0}\nPrintSomeZLJDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintEvidencija); return null;
      }

      return specificMixerReport;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheZLJFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheZLJFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
   }


   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_RUG, Color.Empty, clr_RUG);
   }

   #endregion Colors

   #region overrideTabPageTitle

   public override string TabPageTitle1
   {
      get { return ""; }
   }

   #endregion overrideTabPageTitle

}

public class ZLJFilterUC : VvFilterUC
{
   #region Fieldz

   #endregion Fieldz

   #region  Constructor

   public ZLJFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;

      CreateHamper_4ButtonsResetGo_Width(hamper4buttons.Width);

      hamperHorLine.Visible = false;

      this.Width = hamper4buttons.Width + ZXC.QUN;
      this.Height = hamper4buttons.Bottom + ZXC.QUN;

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region Put & GetFilterFields

   private ZLJFilter TheZLJFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as ZLJFilter; }
      set { this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheZLJFilter = (ZLJFilter)_filter_data;

      if(TheZLJFilter != null)
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

public class ZLJFilter : VvRpt_Mix_Filter
{

   public enum PrintZLJEnum
   {
      ZLJ
   }

   public PrintZLJEnum PrintZLJ { get; set; }

   public ZLJ_DUC theDUC;

   public ZLJFilter(ZLJ_DUC _theDUC)
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
      PrintZLJ = PrintZLJEnum.ZLJ;
   }

   #endregion SetDefaultFilterValues()

}
