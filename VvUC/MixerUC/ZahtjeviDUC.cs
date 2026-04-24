using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public partial class ZahtjeviDUC : MixerDUC
{
   #region Fieldz

   public VvTextBox tbx_kupDobCd, tbx_kupDobTk, tbx_kupDobName, 
                    tbx_dateTrazeniRok, tbx_brRadSati, 
                    tbx_projektCd2, tbx_projektIdent, tbx_vodProjekta, tbx_projektIdent2,
                    tbx_osobOdgIzvrs,
                    tbx_primjedba, tbx_dateMoguciRok, tbx_status,
                    tbx_strF_64, tbx_strD_32, tbx_ValName, tbx_newRN, tbx_newUg;

   public VvHamper  hamp_zahtjevi, hamp_Zahtjevi, hamp_obrDnev, hamp_sumeAll, hamp_linkIzvj, hamp_vozilo, hamp_zadatak;
   
   private VvDateTimePicker dtp_dateTrazeniRok, dtp_dateMoguciRok;

   #endregion Fieldz

   #region Constructor

   public ZahtjeviDUC(Control parent, Mixer _mixer, VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)
   {

      string[] okTtList = ZXC.luiListaMixTypeZahtjev.Select(lui => lui.Cd).ToArray();

      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor(Mixer.tt_colName, okTtList);


      //dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
      //  (Mixer.tt_colName, new string[] 
      //   { 
      //         Mixer.TT_ZAHT_NABD,
      //         Mixer.TT_ZAHT_NABU,
      //         Mixer.TT_ZAHT_FAKT,
      //         Mixer.TT_ZAHT_IPON,
      //         Mixer.TT_ZAHT_PROJ,
      //         Mixer.TT_ZAHT_OTRN,
      //         Mixer.TT_ZAHT_ROBA,
      //         Mixer.TT_ZAHT_MONT,
      //         Mixer.TT_ZAHT_SERV,
      //         Mixer.TT_ZAHT_KONS,
      //         Mixer.TT_ZAHT_PROZ,
      //         Mixer.TT_ZAHT_ORAD,
      //         Mixer.TT_ZAHT_USLG
      //   });
   }



   #endregion Constructor

   #region CreateSpecificHampers()
      
   protected override void CreateSpecificHampers()
   {
      hamp_mjTroska.Visible = true;
      hamp_mjTroska.Location = new Point(hamp_napomena.Right, hamp_napomena.Top);

      InitializeHamper_Zahtjevi(out hamp_zahtjevi);
      nextY = hamp_zahtjevi.Bottom;
   }
  
   public void InitializeHamper_Zahtjevi(out VvHamper hamper)
   {
      hamper = new VvHamper(12, 7, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0        1          2        3         4               5                     6         7                8                         9                  10       
      hamper.VvColWdt = new int[] { ZXC.Q5un, ZXC.Q3un, ZXC.Q3un, ZXC.Q4un, ZXC.Q5un, ZXC.Q4un - ZXC.Qun8, ZXC.Q3un + ZXC.Qun4, ZXC.Q3un, ZXC.Q4un + ZXC.Qun2, ZXC.Q2un + ZXC.QUN - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4- ZXC.Qun8, ZXC.Qun4, ZXC.Qun4,          ZXC.Qun4, ZXC.Qun4                            ,                0,                0 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel  (0, 0, "Partner:", ContentAlignment.MiddleRight);
      tbx_kupDobCd   = hamper.CreateVvTextBox(1, 0, "tbx_kupDobCd"  , "Sifra Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobCD));
      tbx_kupDobTk   = hamper.CreateVvTextBox(2, 0, "tbx_kupDobTk"  , "Tiker Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobTK));
      tbx_kupDobName = hamper.CreateVvTextBox(3, 0, "tbx_kupDobName", "Naziv Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobName), 1, 0);
      
      tbx_kupDobCd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_kupDobTk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_kupDobCd  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobTk  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyKupdobTextBoxLeave));
      
                    hamper.CreateVvLabel        (5, 0, "IzvVal:", ContentAlignment.MiddleRight);
      tbx_ValName = hamper.CreateVvTextBoxLookUp(6, 0, "tbx_ValName", "Naziv devizne valute", GetDB_ColumnSize(DB_ci.devName));
      tbx_ValName.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_ValName.JAM_Set_LookUpTable(ZXC.luiListaDeviza, (int)ZXC.Kolona.prva);

                   hamper.CreateVvLabel        (8, 0, "Status:", ContentAlignment.MiddleRight);
      tbx_status = hamper.CreateVvTextBoxLookUp(9, 0, "tbx_status", "Status", GetDB_ColumnSize(DB_ci.strH_32));
      tbx_status.JAM_Set_LookUpTable(ZXC.luiListaRiskStatus, (int)ZXC.Kolona.prva);
    //  tbx_status.JAM_lookUp_NOTobligatory = true;
      tbx_status.JAM_lookUp_MultiSelection = true;
      tbx_status.JAM_CharacterCasing = CharacterCasing.Upper;


                       hamper.CreateVvLabel  (0, 1, "Projekt:", ContentAlignment.MiddleRight);
      tbx_ProjektCD  = hamper.CreateVvTextBox(1, 1, "tbx_Projekt", "Projekt - ", GetDB_ColumnSize(DB_ci.projektCD), 1, 0);
      tbx_ProjektCD.JAM_SetAutoCompleteData(Faktur.recordName, Faktur.sorterTtNum.SortType, ZXC.AutoCompleteRestrictor.FAK_ExactTT_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Faktur_DUMMY), null);

      tbx_projektIdent = hamper.CreateVvTextBox(3, 1, "tbx_projektIdent", "", GetDB_ColumnSize(DB_ci.strE_256), 6, 0);
      tbx_projektIdent.JAM_ReadOnly = true;

      btn_proj = hamper.CreateVvButton(11, 1, new EventHandler(/*FakturDUC.*/GoToProjektCD_RISK_Dokument_Click), "");
      btn_proj.Name = "projekt";
      btn_proj.FlatStyle = FlatStyle.Flat;
      btn_proj.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_proj.Image = VvIco.TriangleBlue16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue2.ico")), 16, 16)*/.ToBitmap();
      btn_proj.Tag = 10;
      btn_proj.TabStop = false;

                         hamper.CreateVvLabel        (0, 2, "Odgovorni izvršitelj:", ContentAlignment.MiddleRight);
      tbx_osobOdgIzvrs = hamper.CreateVvTextBoxLookUp("tbx_osobOdgIzvrs", 1, 2,  "Odgovorni izvršitelj", GetDB_ColumnSize(DB_ci.strA_40), 1, 0);
      tbx_osobOdgIzvrs.JAM_Set_LookUpTable(ZXC.luiListaRiskVodPrjkt, (int)ZXC.Kolona.prva);

                        hamper.CreateVvLabel        (0, 3, "Voditelj projekta:", ContentAlignment.MiddleRight);
      tbx_vodProjekta = hamper.CreateVvTextBoxLookUp("tbx_vodProjekta", 1, 3,  "Vozilo", GetDB_ColumnSize(DB_ci.strG_40), 1, 0);
      tbx_vodProjekta.JAM_Set_LookUpTable(ZXC.luiListaRiskVodPrjkt, (int)ZXC.Kolona.prva);

                           hamper.CreateVvLabel  (3, 2, "Traženi rok izvršenja/Poč.izvođ.:",1, 0, ContentAlignment.MiddleRight);
      tbx_dateTrazeniRok = hamper.CreateVvTextBox(5, 2, "tbx_dateTrazeniRok", "Datum puta", GetDB_ColumnSize(DB_ci.dateA));
      tbx_dateTrazeniRok.JAM_IsForDateTimePicker = true;
      dtp_dateTrazeniRok = hamper.CreateVvDateTimePicker(5, 2, "", tbx_dateTrazeniRok);
      dtp_dateTrazeniRok.Name = "dtp_dateTrazeniRok";
      
                          hamper.CreateVvLabel  (3, 3, "Mogući rok izvršenja/Rok izvođ.:", 1, 0, ContentAlignment.MiddleRight);
      tbx_dateMoguciRok = hamper.CreateVvTextBox(5, 3, "tbx_dateMoguciRok", "Datum obračuna", GetDB_ColumnSize(DB_ci.dateB));
      tbx_dateMoguciRok.JAM_IsForDateTimePicker = true;
      dtp_dateMoguciRok = hamper.CreateVvDateTimePicker(5, 3, "", tbx_dateMoguciRok);
      dtp_dateMoguciRok.Name = "dtp_dateMoguciRok";

                      hamper.CreateVvLabel  (3, 4, "Broj radnih sati:", 1, 0, ContentAlignment.MiddleRight);
      tbx_brRadSati = hamper.CreateVvTextBox(5, 4, "tbx_dana", "Broj radnih sati", GetDB_ColumnSize(DB_ci.intA));

                    hamper.CreateVvLabel                       (0, 4, "Kontrola1:", ContentAlignment.MiddleRight);
      tbx_strD_32 = hamper.CreateVvTextBoxLookUp("tbx_strC_32", 1, 4,  "Kontrola/Izvršitelj", GetDB_ColumnSize(DB_ci.strC_32), 1, 0);
      tbx_strD_32.JAM_Set_LookUpTable(ZXC.luiListaRiskVodPrjkt, (int)ZXC.Kolona.prva);
      
                            hamper.CreateVvLabel        (6, 2, "Veza1:", ContentAlignment.MiddleRight);
      tbx_v1_tt     = hamper.CreateVvTextBoxLookUp(7, 2, "tbx_v1_tt"    , "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v1_tt));
      tbx_v1_ttOpis = hamper.CreateVvTextBox      (8, 2, "tbx_v1_ttOpis", "Veza na interni dokument", 32);
      tbx_v1_ttNum  = hamper.CreateVvTextBox      (9, 2, "tbx_v1_ttNum" , "Veza na interni dokument", 6/*GetDB_ColumnSize(DB_ci.iz_ttNum)*/);

      btn_v1TT = hamper.CreateVvButton(11, 2, new EventHandler(GoTo_MIXER_Dokument_Click), "");

      btn_v1TT.Name = "v1_TT";
      btn_v1TT.FlatStyle = FlatStyle.Flat;
      btn_v1TT.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_v1TT.Image = VvIco.TriangleBlue16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue2.ico")), 16, 16)*/.ToBitmap();
      btn_v1TT.Tag = 1;
      btn_v1TT.TabStop = false;

      tbx_v1_tt.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_v1_tt.JAM_Set_NOTobligatory_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_v1_tt.JAM_lui_NameTaker_JAM_Name = tbx_v1_ttOpis.JAM_Name;
      tbx_v1_ttOpis.JAM_ReadOnly           = true;
      tbx_v1_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

                      hamper.CreateVvLabel        (6, 3, "Veza2:", ContentAlignment.MiddleRight);
      tbx_v2_tt     = hamper.CreateVvTextBoxLookUp(7, 3, "tbx_v2_tt"    , "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v2_tt));
      tbx_v2_ttOpis = hamper.CreateVvTextBox      (8, 3, "tbx_v2_ttOpis", "Veza na interni dokument");
      tbx_v2_ttNum  = hamper.CreateVvTextBox      (9, 3, "tbx_v2_ttNum" , "Veza na interni dokument" + " broj", 6/*GetDB_ColumnSize(DB_ci.na_ttNum)*/);

      btn_v2TT = hamper.CreateVvButton(11, 3, new EventHandler(GoTo_MIXER_Dokument_Click), "");
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

                    hamper.CreateVvLabel(0, 5, "Kontrola2:", ContentAlignment.MiddleRight);
      tbx_strF_64 = hamper.CreateVvTextBoxLookUp("tbx_strF_64", 1, 5, "Kontrola/Izvršitelj", GetDB_ColumnSize(DB_ci.strF_64), 1, 0);
      tbx_strF_64.JAM_Set_LookUpTable(ZXC.luiListaRiskVodPrjkt, (int)ZXC.Kolona.prva);
      
                  hamper.CreateVvLabel  (3, 5, "UG/Narudž:", ContentAlignment.MiddleRight);
      tbx_newUg = hamper.CreateVvTextBox(4, 5, "tbx_newRN", "Broj ugovora / narudžbe", GetDB_ColumnSize(DB_ci.personPrezim), 1, 0);
                                            
                  hamper.CreateVvLabel  (6, 5, "Naziv RN:", ContentAlignment.MiddleRight);
      tbx_newRN = hamper.CreateVvTextBox(7, 5, "tbx_newRN", "Naziv novog Radnog Naloga", GetDB_ColumnSize(DB_ci.strB_128), 2, 0);


                          hamper.CreateVvLabel  (6, 4, "Sa RN:", ContentAlignment.MiddleRight);
      tbx_projektCd2    = hamper.CreateVvTextBox(7, 4, "tbx_odrediste", "Sa projekta", GetDB_ColumnSize(DB_ci.strC_32)/*, 1 , 0*/);
      tbx_projektCd2.JAM_SetAutoCompleteData(Faktur.recordName, Faktur.sorterTtNum.SortType, ZXC.AutoCompleteRestrictor.FAK_ExactTT_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Faktur_DUMMY), null);
      
      tbx_projektIdent2 = hamper.CreateVvTextBox(8, 4, "tbx_trskTerete", "Sa projekta", GetDB_ColumnSize(DB_ci.strD_32), 1, 0);
      tbx_projektIdent2.JAM_ReadOnly = true;

                      hamper.CreateVvLabel  (0, 6, "Primjedba:", ContentAlignment.MiddleRight);
      tbx_primjedba = hamper.CreateVvTextBox(1, 6, "tbx_externLink1", "Primjedba", GetDB_ColumnSize(DB_ci.strE_256), 4, 0);

                       hamper.CreateVvLabel  (6, 6, "Link:", ContentAlignment.MiddleRight);
      tbx_externLink1 = hamper.CreateVvTextBox(7, 6, "tbx_externLink1", "Izvještaj sa puta - Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.strE_256), 2, 0);

      btn_goExLink1           = hamper.CreateVvButton(10, 6, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink1.Name      = "btn_goExLink1";
      btn_goExLink1.FlatStyle = FlatStyle.Flat;
      btn_goExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink1.Image     = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink1.Tag       = 1;
      btn_goExLink1.TabStop   = false;
      btn_goExLink1.Visible   = false;
      
      btn_openExLink1           = hamper.CreateVvButton(11, 6, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLink1.Name      = "btn_openExLink1";
      btn_openExLink1.FlatStyle = FlatStyle.Flat;
      btn_openExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLink1.Image   = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLink1.Tag     = 1;
      btn_openExLink1.TabStop = false;

   }

   void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      //if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob  kupdob_rec;

      if(tb.Text != this.originalText)
      {
         originalText = tb.Text;
         kupdob_rec = VvUserControl.KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_KupDobName = kupdob_rec.Naziv;
            Fld_KupDobCd   = kupdob_rec.KupdobCD/*RecID*/;
            Fld_KupDobTk   = kupdob_rec.Ticker;
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
      T_artiklCD_CreateColumn      (ZXC.Q7un, "Šifra", "Šifra");
      T_artiklName_CreateColumn    (0, "Opis", "Opis/Naziv proizvoda, usluge, robe ...");
      T_kpdbMjestoA_32_CreateColumn(ZXC.Q2un    , "JM"      , "Jedinica mjere");
      T_kol_CreateColumn           (ZXC.Q2un    , "Kol"     , "Količina", 2);
      T_moneyA_CreateColumn        (ZXC.Q5un    , "Cijena"  , "Iznos"   , 2);
      R_iznos_CreateColumn         (ZXC.Q4un,  2, "Iznos"   , "");
      T_kpdbMjestoB_32_CreateColumn(ZXC.Q4un    , "Prilog"  , "Prilog");
      vvtbT_kol   .JAM_ShouldCalcTransAndSumGrid = true;
      vvtbT_moneyA.JAM_ShouldCalcTransAndSumGrid = true;

   }

   #endregion InitializeTheGrid_Columns()

   #region Fld_

   public uint     Fld_KupDobCd       { get { return tbx_kupDobCd    .GetSomeRecIDField(); } set { tbx_kupDobCd    .PutSomeRecIDField(value); } }
   public string   Fld_KupDobCdAsTxt  { get { return tbx_kupDobCd    .Text               ; } set { tbx_kupDobCd    .Text = value            ; } }
   public string   Fld_KupDobName     { get { return tbx_kupDobName  .Text               ; } set { tbx_kupDobName  .Text = value            ; } }
   public string   Fld_KupDobTk       { get { return tbx_kupDobTk    .Text               ; } set { tbx_kupDobTk    .Text = value            ; } }
   public string   Fld_OdgIzvrsitelj  { get { return tbx_osobOdgIzvrs.Text               ; } set { tbx_osobOdgIzvrs.Text = value            ; } }
  
   public DateTime Fld_DateTrazRok    
   {
      get { return dtp_dateTrazeniRok.Value; }
      set
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dtp_dateTrazeniRok.Value = value;
         }
      }
   }
   public DateTime Fld_DateMogRok     
   {
      get { return dtp_dateMoguciRok.Value; }
      set
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dtp_dateMoguciRok.Value = value;
         }
      }
   }
  
   public string Fld_DevName         { get { return tbx_ValName.Text                     ; } set { tbx_ValName.Text = value                  ; } }
   public string Fld_StrD_32         { get { return tbx_strD_32.Text                     ; } set { tbx_strD_32.Text = value                  ; } }
   public string Fld_StrF_64         { get { return tbx_strF_64.Text                     ; } set { tbx_strF_64.Text = value                  ; } }
  
   public string   Fld_ProjektCd2    { get { return tbx_projektCd2   .Text               ; } set { tbx_projektCd2   .Text = value            ; } } 
   public string   Fld_PrjIdent      {                                                       set { tbx_projektIdent .Text = value            ; } } 
   public string   Fld_PrjIdent2     {                                                       set { tbx_projektIdent2.Text = value            ; } } 
   public string   Fld_VodProjekta   { get { return tbx_vodProjekta  .Text               ; } set { tbx_vodProjekta  .Text = value            ; } } 
   public string   Fld_Primjedba     { get { return tbx_primjedba    .Text               ; } set { tbx_primjedba    .Text = value            ; } }
   public string   Fld_Status        { get { return tbx_status       .Text               ; } set { tbx_status       .Text = value            ; } }
   public int      Fld_BrRadSati     { get { return tbx_brRadSati    .GetIntField()      ; } set { tbx_brRadSati    .PutIntField    (value)  ; } }
   public string   Fld_NewUg         { get { return tbx_newUg        .Text               ; } set { tbx_newUg        .Text = value            ; } }
   public string   Fld_NewRN         { get { return tbx_newRN        .Text               ; } set { tbx_newRN        .Text = value            ; } }

   #endregion _Fld

   #region override PutSpecificsFld() GetSpecificsFld()
   
   /*protected*/public override void PutSpecificsFld()
   {
      PutSpecificsFld(mixer_rec);
   }

   /*protected*/public override void PutSpecificsFld(Mixer mixerLocal_rec)
   {
      if(CtrlOK(tbx_kupDobCd       )) Fld_KupDobCd        = mixerLocal_rec.KupdobCD  ;
      if(CtrlOK(tbx_kupDobName     )) Fld_KupDobName      = mixerLocal_rec.KupdobName;
      if(CtrlOK(tbx_kupDobTk       )) Fld_KupDobTk        = mixerLocal_rec.KupdobTK  ;
      if(CtrlOK(tbx_dateTrazeniRok )) Fld_DateTrazRok     = mixerLocal_rec.DateA     ;
      if(CtrlOK(tbx_dateMoguciRok  )) Fld_DateMogRok      = mixerLocal_rec.DateB     ;
      if(CtrlOK(tbx_ProjektCD      )) Fld_ProjektCD       = mixerLocal_rec.ProjektCD ;
      if(CtrlOK(tbx_brRadSati      )) Fld_BrRadSati       = mixerLocal_rec.IntA      ;
      if(CtrlOK(tbx_primjedba      )) Fld_Primjedba       = mixerLocal_rec.StrE_256  ;
      if(CtrlOK(tbx_osobOdgIzvrs   )) Fld_OdgIzvrsitelj   = mixerLocal_rec.StrA_40   ;
      if(CtrlOK(tbx_vodProjekta    )) Fld_VodProjekta     = mixerLocal_rec.StrG_40   ;
      if(CtrlOK(tbx_projektCd2     )) Fld_ProjektCd2      = mixerLocal_rec.StrC_32   ;
      if(CtrlOK(tbx_status         )) Fld_Status          = mixerLocal_rec.StrH_32   ;
      if(CtrlOK(tbx_ValName        )) Fld_DevName         = mixerLocal_rec.DevName   ;
      if(CtrlOK(tbx_strD_32        )) Fld_StrD_32         = mixerLocal_rec.StrD_32   ;
      if(CtrlOK(tbx_strF_64        )) Fld_StrF_64         = mixerLocal_rec.StrF_64   ;
                                      Fld_TtOpis          = ZXC.luiListaMixTypeZahtjev.GetNameForThisCd(mixerLocal_rec.TT);
     
      if(CtrlOK(tbx_newUg        ))   Fld_NewUg           = mixerLocal_rec.PersonPrezim;
      if(CtrlOK(tbx_newRN        ))   Fld_NewRN           = mixerLocal_rec.StrB_128    ;

      if(CtrlOK(tbx_v1_tt        ))   Fld_V1_tt           = mixerLocal_rec.V1_tt;
      if(CtrlOK(tbx_v1_ttOpis    ))   Fld_V1_ttOpis       = ZXC.GetNameForThisCdFromManyLuiLists(mixerLocal_rec.V1_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v1_ttNum     ))   Fld_V1_ttNum        = mixerLocal_rec.V1_ttNum;
      if(CtrlOK(tbx_v2_tt        ))   Fld_V2_tt           = mixerLocal_rec.V2_tt;
      if(CtrlOK(tbx_v2_ttOpis    ))   Fld_V2_ttOpis       = ZXC.GetNameForThisCdFromManyLuiLists(mixerLocal_rec.V2_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v2_ttNum     ))   Fld_V2_ttNum        = mixerLocal_rec.V2_ttNum;
      if(CtrlOK(tbx_externLink1  ))   Fld_ExternLink1     = mixerLocal_rec.ExternLink1;


      if(CtrlOK(tbx_projektIdent))
      {
         mixerLocal_rec.prjFaktur_rec_LOADED = false;

         Fld_PrjIdent = mixerLocal_rec.ProjektIdent1;
      }

      if(CtrlOK(tbx_projektIdent2))
      {
         mixerLocal_rec.prjFaktur_rec_LOADED2 = false;

         Fld_PrjIdent2 = mixerLocal_rec.ProjektIdent1_2;
      }

   }

   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd       ))  mixer_rec.KupdobCD  = Fld_KupDobCd     ;
      if(CtrlOK(tbx_kupDobName     ))  mixer_rec.KupdobName= Fld_KupDobName   ;
      if(CtrlOK(tbx_kupDobTk       ))  mixer_rec.KupdobTK  = Fld_KupDobTk     ;
                                                             
      if(CtrlOK(tbx_dateTrazeniRok ))  mixer_rec.DateA     = Fld_DateTrazRok  ;
      if(CtrlOK(tbx_dateMoguciRok  ))  mixer_rec.DateB     = Fld_DateMogRok   ;
      if(CtrlOK(tbx_ProjektCD      ))  mixer_rec.ProjektCD = Fld_ProjektCD    ;
      if(CtrlOK(tbx_brRadSati      ))  mixer_rec.IntA      = Fld_BrRadSati    ;
      if(CtrlOK(tbx_primjedba      ))  mixer_rec.StrE_256  = Fld_Primjedba    ;
                                                             
      if(CtrlOK(tbx_osobOdgIzvrs   ))  mixer_rec.StrA_40   = Fld_OdgIzvrsitelj;  
      if(CtrlOK(tbx_vodProjekta    ))  mixer_rec.StrG_40   = Fld_VodProjekta  ;
                                                             
      if(CtrlOK(tbx_projektCd2     ))  mixer_rec.StrC_32   = Fld_ProjektCd2   ;
      if(CtrlOK(tbx_status         ))  mixer_rec.StrH_32   = Fld_Status       ;
      if(CtrlOK(tbx_ValName        ))  mixer_rec.DevName    = Fld_DevName     ;
      if(CtrlOK(tbx_strD_32        ))  mixer_rec.StrD_32    = Fld_StrD_32     ;
      if(CtrlOK(tbx_strF_64        ))  mixer_rec.StrF_64    = Fld_StrF_64     ;

      if(CtrlOK(tbx_newUg          ))  mixer_rec.PersonPrezim= Fld_NewUg      ;
      if(CtrlOK(tbx_newRN          ))  mixer_rec.StrB_128    = Fld_NewRN      ;
      if(CtrlOK(tbx_v1_tt          ))  mixer_rec.V1_tt       = Fld_V1_tt      ;
      if(CtrlOK(tbx_v1_ttNum       ))  mixer_rec.V1_ttNum    = Fld_V1_ttNum   ;
      if(CtrlOK(tbx_v2_tt          ))  mixer_rec.V2_tt       = Fld_V2_tt      ;
      if(CtrlOK(tbx_v2_ttNum       ))  mixer_rec.V2_ttNum    = Fld_V2_ttNum   ;
      if(CtrlOK(tbx_externLink1    ))  mixer_rec.ExternLink1    = Fld_ExternLink1;
   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region PrintDocumentRecord

   public ZahtjeviFilterUC TheZahtjeviFilterUC { get; set; }
   public ZahtjeviFilter   TheZahtjeviFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.TheZahtjeviFilter = new ZahtjeviFilter(this);

      TheZahtjeviFilterUC        = new ZahtjeviFilterUC(this);
      TheZahtjeviFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheZahtjeviFilterUC.Width;

   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      ZahtjeviFilter mixerFilter = (ZahtjeviFilter)vvRptFilter;

      switch(mixerFilter.PrintZahtjevi)
      {
         case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_NABD: specificMixerReport = new RptX_Zahtjev(new Vektor.Reports.XIZ.CR_Zahtjevi(), "ZAHTJEV ZA DOMAĆU NABAVU"               , mixerFilter); break;
         case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_NABU: specificMixerReport = new RptX_Zahtjev(new Vektor.Reports.XIZ.CR_Zahtjevi(), "ZAHTJEV ZA NABAVU - UVOZ"               , mixerFilter); break;
         case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_FAKT: specificMixerReport = new RptX_Zahtjev(new Vektor.Reports.XIZ.CR_Zahtjevi(), "ZAHTJEV ZA FAKTURIRANJE"                , mixerFilter); break;
         case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_IPON: specificMixerReport = new RptX_Zahtjev(new Vektor.Reports.XIZ.CR_Zahtjevi(), "ZAHTJEV ZA IZRADU PONUDE"               , mixerFilter); break;
         case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_PROJ: specificMixerReport = new RptX_Zahtjev(new Vektor.Reports.XIZ.CR_Zahtjevi(), "ZAHTJEV ZA PROJEKTIRANJE"               , mixerFilter); break;
         case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_OTRN: specificMixerReport = new RptX_Zahtjev(new Vektor.Reports.XIZ.CR_Zahtjevi(), "ZAHTJEV ZA OTVRANJE RADNOG NALOGA"      , mixerFilter); break;
         case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_ROBA: specificMixerReport = new RptX_Zahtjev(new Vektor.Reports.XIZ.CR_Zahtjevi(), "ZAHTJEV ZA IZDAVANJE ROBE SA SKLADIŠTA" , mixerFilter); break;
         case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_MONT: specificMixerReport = new RptX_Zahtjev(new Vektor.Reports.XIZ.CR_Zahtjevi(), "ZAHTJEV ZA MONTAŽU"                     , mixerFilter); break;
         case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_SERV: specificMixerReport = new RptX_Zahtjev(new Vektor.Reports.XIZ.CR_Zahtjevi(), "ZAHTJEV ZA SERVIS"                      , mixerFilter); break;
         case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_KONS: specificMixerReport = new RptX_Zahtjev(new Vektor.Reports.XIZ.CR_Zahtjevi(), "ZAHTJEV KONSTRUKCIJSKOM ODJELU"         , mixerFilter); break;
         case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_PROZ: specificMixerReport = new RptX_Zahtjev(new Vektor.Reports.XIZ.CR_Zahtjevi(), "ZAHTJEV ZA PROIZVODNJU"                 , mixerFilter); break;
         case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_ORAD: specificMixerReport = new RptX_Zahtjev(new Vektor.Reports.XIZ.CR_Zahtjevi(), "ZAHTJEV ZA ODOBRENJE RADA"              , mixerFilter); break;
         case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_USLG: specificMixerReport = new RptX_Zahtjev(new Vektor.Reports.XIZ.CR_Zahtjevi(), "ZAHTJEV ZA USLUGU"                      , mixerFilter); break;
         
         default: ZXC.aim_emsg("{0}\nPrintSomeZahtjeviDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintZahtjevi); return null;
      }

      return specificMixerReport;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheZahtjeviFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheZahtjeviFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
   }


   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Zahtj, Color.Empty, clr_Zahtj);
   }

   #endregion Colors

   #region overrideTabPageTitle

   public override string TabPageTitle1
   {
      get { return " "; }
   }
   //public override string TabPageTitle2
   //{
   //   get { return "Obračun ostalih troškova"; }
   //}
   
   #endregion overrideTabPageTitle

}

public class ZahtjeviFilterUC : VvFilterUC
{
   #region Fieldz

   private VvHamper    hamp_rbt;
   private RadioButton 
     rbt_NABD,
     rbt_NABU,
     rbt_FAKT,
     rbt_IPON,
     rbt_PROJ,
     rbt_OTRN,
     rbt_ROBA,
     rbt_MONT,
     rbt_SERV,
     rbt_KONS,
     rbt_PROZ,
     rbt_ORAD,
     rbt_USLG;



   #endregion Fieldz

   #region  Constructor

   public ZahtjeviFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;
     
      CreateHampers();

      CreateHamper_4ButtonsResetGo_Width(hamp_rbt.Width);

      hamp_rbt.Location = new Point(nextX, hamper4buttons.Bottom + ZXC.Qun4);
      hamperHorLine.Visible = false;

      this.Width  = hamp_rbt.Width + ZXC.QUN;
      this.Height = hamp_rbt.Bottom + ZXC.QUN;

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region Hampers

   private void CreateHampers()
   {
      InitializeHamper_Rbt(out hamp_rbt);
   }

   private void InitializeHamper_Rbt(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q8un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = ZXC.Qun4 + ZXC.Qun10;

      rbt_NABD = hamper.CreateVvRadioButton(0, 0, null, "Zahtjev za domaću nabavu"      , TextImageRelation.ImageBeforeText);
      rbt_NABU = hamper.CreateVvRadioButton(0, 0, null, "Zahtjev za nabavu - uvoz"      , TextImageRelation.ImageBeforeText);
      rbt_FAKT = hamper.CreateVvRadioButton(0, 0, null, "Zahtjev za fakturiranje"       , TextImageRelation.ImageBeforeText);
      rbt_IPON = hamper.CreateVvRadioButton(0, 0, null, "Zahtjev za izradu ponude"      , TextImageRelation.ImageBeforeText);
      rbt_PROJ = hamper.CreateVvRadioButton(0, 0, null, "Zahtjev za projektiranje"      , TextImageRelation.ImageBeforeText);
      rbt_OTRN = hamper.CreateVvRadioButton(0, 0, null, "Zahtjev za otvaranje RN"       , TextImageRelation.ImageBeforeText);
      rbt_ROBA = hamper.CreateVvRadioButton(0, 0, null, "Zahtjev za izdavanje robe"     , TextImageRelation.ImageBeforeText);
      rbt_MONT = hamper.CreateVvRadioButton(0, 0, null, "Zahtjev za montažu"            , TextImageRelation.ImageBeforeText);
      rbt_SERV = hamper.CreateVvRadioButton(0, 0, null, "Zahtjev za servis"             , TextImageRelation.ImageBeforeText);
      rbt_KONS = hamper.CreateVvRadioButton(0, 0, null, "Zahtjev konstrukcijskom odjelu", TextImageRelation.ImageBeforeText);
      rbt_PROZ = hamper.CreateVvRadioButton(0, 0, null, "Zahtjev za proizvodnju"        , TextImageRelation.ImageBeforeText);
      rbt_ORAD = hamper.CreateVvRadioButton(0, 0, null, "Zahtjev za odobrenje rada"     , TextImageRelation.ImageBeforeText);
      rbt_USLG = hamper.CreateVvRadioButton(0, 0, null, "Zahtjev za uslugu"             , TextImageRelation.ImageBeforeText);

      rbt_NABD.Visible =
      rbt_NABU.Visible =
      rbt_FAKT.Visible =
      rbt_IPON.Visible =
      rbt_PROJ.Visible =
      rbt_OTRN.Visible =
      rbt_ROBA.Visible =
      rbt_MONT.Visible =
      rbt_SERV.Visible =
      rbt_KONS.Visible =
      rbt_PROZ.Visible =
      rbt_ORAD.Visible =
      rbt_USLG.Visible = false;
      rbt_NABD.Checked = true;

      if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_NABD) { rbt_NABD.Visible = true; rbt_NABD.Checked = true; rbt_NABD.Tag = true; }
      if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_NABU) { rbt_NABU.Visible = true; rbt_NABU.Checked = true; rbt_NABU.Tag = true; }
      if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_FAKT) { rbt_FAKT.Visible = true; rbt_FAKT.Checked = true; rbt_FAKT.Tag = true; }
      if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_IPON) { rbt_IPON.Visible = true; rbt_IPON.Checked = true; rbt_IPON.Tag = true; }
      if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_PROJ) { rbt_PROJ.Visible = true; rbt_PROJ.Checked = true; rbt_PROJ.Tag = true; }
      if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_OTRN) { rbt_OTRN.Visible = true; rbt_OTRN.Checked = true; rbt_OTRN.Tag = true; }
      if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_ROBA) { rbt_ROBA.Visible = true; rbt_ROBA.Checked = true; rbt_ROBA.Tag = true; }
      if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_MONT) { rbt_MONT.Visible = true; rbt_MONT.Checked = true; rbt_MONT.Tag = true; }
      if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_SERV) { rbt_SERV.Visible = true; rbt_SERV.Checked = true; rbt_SERV.Tag = true; }
      if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_KONS) { rbt_KONS.Visible = true; rbt_KONS.Checked = true; rbt_KONS.Tag = true; }
      if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_PROZ) { rbt_PROZ.Visible = true; rbt_PROZ.Checked = true; rbt_PROZ.Tag = true; }
      if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_ORAD) { rbt_ORAD.Visible = true; rbt_ORAD.Checked = true; rbt_ORAD.Tag = true; }
      if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_USLG) { rbt_USLG.Visible = true; rbt_USLG.Checked = true; rbt_USLG.Tag = true; }

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.HamperStyling(hamper);
   }

   #endregion Hampers

   #region Fld_

   public ZahtjeviFilter.PrintZahtjeviEnum Fld_PrintZahtjevi
   {
      get
      {
              if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_NABD) return ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_NABD;   
         else if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_NABU) return ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_NABU;
         else if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_FAKT) return ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_FAKT;
         else if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_IPON) return ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_IPON;
         else if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_PROJ) return ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_PROJ;
         else if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_OTRN) return ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_OTRN;
         else if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_ROBA) return ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_ROBA;
         else if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_MONT) return ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_MONT;
         else if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_SERV) return ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_SERV;
         else if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_KONS) return ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_KONS;
         else if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_PROZ) return ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_PROZ;
         else if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_ORAD) return ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_ORAD;
         else if(((ZahtjeviDUC)TheVvUC).Fld_TT == Mixer.TT_ZAHT_USLG) return ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_USLG;

         else throw new Exception("Fld_PrintZahtjevi: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_NABD: rbt_NABD.Checked = true; break;
            case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_NABU: rbt_NABU.Checked = true; break;
            case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_FAKT: rbt_FAKT.Checked = true; break;
            case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_IPON: rbt_IPON.Checked = true; break;
            case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_PROJ: rbt_PROJ.Checked = true; break;
            case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_OTRN: rbt_OTRN.Checked = true; break;
            case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_ROBA: rbt_ROBA.Checked = true; break;
            case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_MONT: rbt_MONT.Checked = true; break;
            case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_SERV: rbt_SERV.Checked = true; break;
            case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_KONS: rbt_KONS.Checked = true; break;
            case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_PROZ: rbt_PROZ.Checked = true; break;
            case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_ORAD: rbt_ORAD.Checked = true; break;
            case ZahtjeviFilter.PrintZahtjeviEnum.TT_ZAHT_USLG: rbt_USLG.Checked = true; break;

         }
      }
   }


   #endregion Fld_

   #region Put & GetFilterFields

   private ZahtjeviFilter TheZahtjeviFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as ZahtjeviFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheZahtjeviFilter = (ZahtjeviFilter)_filter_data;

      if(TheZahtjeviFilter != null)
      {
         Fld_PrintZahtjevi     = TheZahtjeviFilter.PrintZahtjevi;
      }

      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
      TheZahtjeviFilter.PrintZahtjevi     = Fld_PrintZahtjevi;
   }

   #endregion Put & GetFilterFields

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
   }

   #endregion AddFilterMemberz()

}

public class ZahtjeviFilter : VvRpt_Mix_Filter
{

   public enum PrintZahtjeviEnum
   {
     TT_ZAHT_NABD,
     TT_ZAHT_NABU,
     TT_ZAHT_FAKT,
     TT_ZAHT_IPON,
     TT_ZAHT_PROJ,
     TT_ZAHT_OTRN,
     TT_ZAHT_ROBA,
     TT_ZAHT_MONT,
     TT_ZAHT_SERV,
     TT_ZAHT_KONS,
     TT_ZAHT_PROZ,
     TT_ZAHT_ORAD,
     TT_ZAHT_USLG
    }

   public PrintZahtjeviEnum PrintZahtjevi { get; set; }

   public ZahtjeviDUC theDUC;

   public ZahtjeviFilter(ZahtjeviDUC _theDUC)
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
      PrintZahtjevi = PrintZahtjeviEnum.TT_ZAHT_NABD;
   }

   #endregion SetDefaultFilterValues()

}
