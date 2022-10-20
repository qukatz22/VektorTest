using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public partial class EvidencijaDUC : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_kupDobCd , tbx_kupDobTk, tbx_kupDobName, tbx_projektIdent,
                     tbx_primjedba, tbx_osoba, tbx_status, tbx_iznos, tbx_ValName,
                     tbx_StrA_40_BrNacr, tbx_StrC_32_Ormar,	tbx_StrD_32_BrPapira, tbx_StrE_256_NazNacrt, tbx_StrF_64_Format;
   public VvHamper   hamp_evidencija, hamp_nacrti;

   #endregion Fieldz

   #region Constructor

   public EvidencijaDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul)  : base(parent, _mixer, vvSubModul)
   {
      //------ 
      //string[] okTtList = new string[ZXC.luiListaMixTypeEvidencija.Count];
      string[] okTtList = ZXC.luiListaMixTypeEvidencija.Select(lui => lui.Cd).ToArray();

      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor(Mixer.tt_colName, okTtList);

      //------ 

      //dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
      //  (Mixer.tt_colName, new string[] 
      //   { 
      //            Mixer.TT_EVD_PON,
      //            Mixer.TT_EVD_UGV,
      //            Mixer.TT_EVD_ZAP,
      //            Mixer.TT_EVD_PRJ,
      //            Mixer.TT_EVD_TPL,
      //            Mixer.TT_EVD_MCD,
      //            Mixer.TT_EVD_PRG,
      //            Mixer.TT_EVD_ATS,
      //            Mixer.TT_EVD_USV,
      //            Mixer.TT_EVD_UVR,
      //            Mixer.TT_EVD_RDK,
      //            Mixer.TT_EVD_ODL,
      //            Mixer.TT_EVD_KMP,
      //            Mixer.TT_EVD_PUN,
      //            Mixer.TT_EVD_OST,
      //            Mixer.TT_EVD_N10,
      //            Mixer.TT_EVD_N20,
      //            Mixer.TT_EVD_N30,
      //            Mixer.TT_EVD_N40,
      //            Mixer.TT_EVD_N50,
      //            Mixer.TT_EVD_N60

      //   });
   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
      hamp_mjTroska.Visible = true;
      hamp_mjTroska.Location = new Point(hamp_napomena.Right, hamp_napomena.Top);

      InitializeHamper_Evidencija(out hamp_evidencija);

      nextX = hamp_tt.Right;
      nextY = hamp_tt.Top;
      InitializeHamper_Nacrti(out hamp_nacrti);

      nextY = hamp_evidencija.Bottom;
   }

   public void InitializeHamper_Evidencija(out VvHamper hamper)
   {
      hamper = new VvHamper(11, 5, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0                 1                   2                  3                  4                   5                   6                  7                   8                9           10       
      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un- ZXC.Qun4, ZXC.Q3un- ZXC.Qun4, ZXC.Q10un- ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.Q2un+ZXC.Qun8, ZXC.Q3un + ZXC.Qun2, ZXC.Q6un+ ZXC.Qun4, ZXC.Q4un, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,           ZXC.Qun4,           ZXC.Qun4,            ZXC.Qun4,                 0 ,                 0 ,          ZXC.Qun4,            ZXC.Qun4,           ZXC.Qun4, ZXC.Qun4,                  0 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }      
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
                       hamper.CreateVvLabel  (0, 0, "Partner:"      , ContentAlignment.MiddleRight);
      tbx_kupDobCd   = hamper.CreateVvTextBox(1, 0, "tbx_kupDobCd"  , "Sifra Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobCD));
      tbx_kupDobTk   = hamper.CreateVvTextBox(2, 0, "tbx_kupDobTk"  , "Tiker Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobTK));
      tbx_kupDobName = hamper.CreateVvTextBox(3, 0, "tbx_kupDobName", "Naziv Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobName));

      tbx_kupDobCd.JAM_CharEdits       = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_kupDobTk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_kupDobCd.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobTk.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyKupdobTextBoxLeave));

                  hamper.CreateVvLabel        (4, 0, "Osoba:", 2, 0, ContentAlignment.MiddleRight);
      tbx_osoba = hamper.CreateVvTextBoxLookUp("tbx_osoba", 7, 0,  "Osoba", GetDB_ColumnSize(DB_ci.strG_40), 2, 0);
      tbx_osoba.JAM_Set_LookUpTable(ZXC.luiListaRiskVodPrjkt, (int)ZXC.Kolona.prva);
      tbx_osoba.JAM_lookUp_NOTobligatory = true;

                      hamper.CreateVvLabel  (0, 1, "Projekt:", ContentAlignment.MiddleRight);
      tbx_ProjektCD = hamper.CreateVvTextBox(1, 1, "tbx_Projekt", "Projekt - ", GetDB_ColumnSize(DB_ci.projektCD), 1, 0);
      tbx_ProjektCD.JAM_SetAutoCompleteData(Faktur.recordName, Faktur.sorterTtNum.SortType, ZXC.AutoCompleteRestrictor.FAK_ExactTT_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Faktur_DUMMY), null);

      tbx_projektIdent = hamper.CreateVvTextBox(3, 1, "tbx_projektIdent", "", GetDB_ColumnSize(DB_ci.strE_256));
      tbx_projektIdent.JAM_ReadOnly = true;

      btn_proj = hamper.CreateVvButton(5, 1, new EventHandler(/*FakturDUC.*/GoToProjektCD_RISK_Dokument_Click), "");
      btn_proj.Name = "projekt";
      btn_proj.FlatStyle = FlatStyle.Flat;
      btn_proj.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_proj.Image = VvIco.TriangleBlue16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue2.ico")), 16, 16)*/.ToBitmap();
      btn_proj.Tag = 10;
      btn_proj.TabStop = false;

                    hamper.CreateVvLabel        (4, 1, "IzvVal:", 2, 0, ContentAlignment.MiddleRight);
      tbx_ValName = hamper.CreateVvTextBoxLookUp(7, 1, "tbx_ValName",  "Naziv devizne valute", GetDB_ColumnSize(DB_ci.devName));
      tbx_ValName.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_ValName.JAM_Set_LookUpTable(ZXC.luiListaDeviza, (int)ZXC.Kolona.prva);

                  hamper.CreateVvLabel  (8, 1, "Iznos:", ContentAlignment.MiddleRight);
      tbx_iznos = hamper.CreateVvTextBox(9, 1, "tbx_iznos", "Iznos", GetDB_ColumnSize(DB_ci.moneyA));
      tbx_iznos.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

                      hamper.CreateVvLabel        (6, 2, "Veza1:"       , ContentAlignment.MiddleRight);
      tbx_v1_tt     = hamper.CreateVvTextBoxLookUp(7, 2, "tbx_v1_tt"    , "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v1_tt));
      tbx_v1_ttOpis = hamper.CreateVvTextBox      (8, 2, "tbx_v1_ttOpis", "Veza na interni dokument", 32);
      tbx_v1_ttNum  = hamper.CreateVvTextBox      (9, 2, "tbx_v1_ttNum" , "Veza na interni dokument", 6/*GetDB_ColumnSize(DB_ci.iz_ttNum)*/);

      btn_v1TT = hamper.CreateVvButton(10, 2, new EventHandler(GoTo_MIXER_Dokument_Click), "");

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

                      hamper.CreateVvLabel        (6, 3, "Veza2:"       ,ContentAlignment.MiddleRight);
      tbx_v2_tt     = hamper.CreateVvTextBoxLookUp(7, 3, "tbx_v2_tt"    , "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v2_tt));
      tbx_v2_ttOpis = hamper.CreateVvTextBox      (8, 3, "tbx_v2_ttOpis", "Veza na interni dokument");
      tbx_v2_ttNum  = hamper.CreateVvTextBox      (9, 3, "tbx_v2_ttNum" , "Veza na interni dokument" + " broj", 6/*GetDB_ColumnSize(DB_ci.na_ttNum)*/);

      btn_v2TT = hamper.CreateVvButton(10, 3, new EventHandler(GoTo_MIXER_Dokument_Click), "");
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

                        hamper.CreateVvLabel  (0, 2, "Link1:", ContentAlignment.MiddleRight);
      tbx_externLink1 = hamper.CreateVvTextBox(1, 2, "tbx_externLink1", "Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.externLink1), 2, 0);

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

                        hamper.CreateVvLabel  (0, 3, "Link2:", ContentAlignment.MiddleRight);
      tbx_externLink2 = hamper.CreateVvTextBox(1, 3, "tbx_externLink2", "Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.externLink2), 2, 0);

      btn_goExLink2           = hamper.CreateVvButton(4, 3, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink2.Name      = "btn_goExLink2";
      btn_goExLink2.FlatStyle = FlatStyle.Flat;
      btn_goExLink2.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink2.Image     = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink2.Tag       = 2;
      btn_goExLink2.TabStop   = false;
      btn_goExLink2.Visible   = false;
      
      btn_openExLink2           = hamper.CreateVvButton(5, 3, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLink2.Name      = "btn_openExLink2";
      btn_openExLink2.FlatStyle = FlatStyle.Flat;
      btn_openExLink2.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLink2.Image   = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLink2.Tag     = 2;
      btn_openExLink2.TabStop = false;

                      hamper.CreateVvLabel  (0, 4, "Primjedba:"   , ContentAlignment.MiddleRight);
      tbx_primjedba = hamper.CreateVvTextBox(1, 4, "tbx_primjedba", "Primjedba", GetDB_ColumnSize(DB_ci.strB_128), 6, 0);

                   hamper.CreateVvLabel        (8, 4, "Status:", ContentAlignment.MiddleRight);
      tbx_status = hamper.CreateVvTextBoxLookUp(9, 4, "tbx_iznos", "Iznos", GetDB_ColumnSize(DB_ci.strH_32));
      tbx_status.JAM_Set_LookUpTable(ZXC.luiListaRiskStatus, (int)ZXC.Kolona.prva);
      tbx_status.JAM_lookUp_MultiSelection = true;
      tbx_status.JAM_CharacterCasing = CharacterCasing.Upper;

   }

   public void InitializeHamper_Nacrti(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 7, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      
      hamper.VvColWdt      = new int[] { ZXC.Q3un+ ZXC.Qun2, ZXC.Q4un, ZXC.Q4un, ZXC.Q4un};
      hamper.VvSpcBefCol   = new int[] {           ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }      
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
                          hamper.CreateVvLabel  (0, 0, "Ormar:", ContentAlignment.MiddleRight);
      tbx_StrC_32_Ormar = hamper.CreateVvTextBox(1, 0, "tbx_StrC_32_Ormar", "Ormar", GetDB_ColumnSize(DB_ci.strC_32));
      
                             hamper.CreateVvLabel  (2, 0, "Broj papira:", ContentAlignment.MiddleRight);
      tbx_StrD_32_BrPapira = hamper.CreateVvTextBox(3, 0, "tbx_StrD_32_BrPapira", "Broj papira", GetDB_ColumnSize(DB_ci.strD_32));

                           hamper.CreateVvLabel  (0, 1, "Broj dok:", ContentAlignment.MiddleRight);
      tbx_StrA_40_BrNacr = hamper.CreateVvTextBox(1, 1, "tbx_StrA_40_BrNacr", "Broj nacrta", GetDB_ColumnSize(DB_ci.strA_40));
      
                              hamper.CreateVvLabel  (2, 1, "Format:", ContentAlignment.MiddleRight);
      tbx_StrF_64_Format    = hamper.CreateVvTextBox(3, 1, "tbx_StrF_64_Format"   , "Format"      , GetDB_ColumnSize(DB_ci.strF_64));
              
                              hamper.CreateVvLabel  (0, 2, "Naziv nacrta:", ContentAlignment.MiddleRight);
      tbx_StrE_256_NazNacrt = hamper.CreateVvTextBox(1, 2, "tbx_StrE_256_NazNacrt", "Naziv nacrta", GetDB_ColumnSize(DB_ci.strE_256), 2, 4);
      tbx_StrE_256_NazNacrt.Multiline = true;
      tbx_StrE_256_NazNacrt.ScrollBars = ScrollBars.Vertical;

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
      T_opis_128_CreateColumn      (0, "Komentar", "Komentar", 128,  null);
   }

   #endregion InitializeTheGrid_Columns()

   #region Fld_

   public uint    Fld_KupDobCd          { get { return tbx_kupDobCd         .GetSomeRecIDField(); } set { tbx_kupDobCd         .PutSomeRecIDField(value) ; } }
   public string  Fld_KupDobCdAsTxt     { get { return tbx_kupDobCd         .Text               ; } set { tbx_kupDobCd         .Text = value             ; } }
   public string  Fld_KupDobName        { get { return tbx_kupDobName       .Text               ; } set { tbx_kupDobName       .Text = value             ; } }
   public string  Fld_KupDobTk          { get { return tbx_kupDobTk         .Text               ; } set { tbx_kupDobTk         .Text = value             ; } }
   public string  Fld_PrjIdent          {                                                           set { tbx_projektIdent     .Text = value             ; } }
   public string  Fld_Osoba             { get { return tbx_osoba            .Text               ; } set { tbx_osoba            .Text = value             ; } } 
   public string  Fld_Primjedba         { get { return tbx_primjedba        .Text               ; } set { tbx_primjedba        .Text = value             ; } }
   public string  Fld_Status            { get { return tbx_status           .Text               ; } set { tbx_status           .Text = value             ; } }
   public string  Fld_DevName           { get { return tbx_ValName          .Text               ; } set { tbx_ValName          .Text = value             ; } }
   public decimal Fld_Iznos             { get { return tbx_iznos            .GetDecimalField()  ; } set { tbx_iznos            .PutDecimalField(value)   ; } }
   public string  Fld_StrA_40_BrNacr    { get { return tbx_StrA_40_BrNacr   .Text               ; } set { tbx_StrA_40_BrNacr   .Text = value             ; } }
   public string  Fld_StrC_32_Ormar		 { get { return tbx_StrC_32_Ormar	 .Text               ; } set { tbx_StrC_32_Ormar	 .Text = value             ; } }
   public string  Fld_StrD_32_BrPapira  { get { return tbx_StrD_32_BrPapira .Text               ; } set { tbx_StrD_32_BrPapira .Text = value             ; } }
   public string  Fld_StrE_256_NazNacrt { get { return tbx_StrE_256_NazNacrt.Text               ; } set { tbx_StrE_256_NazNacrt.Text = value             ; } }
   public string  Fld_StrF_64_Format    { get { return tbx_StrF_64_Format   .Text               ; } set { tbx_StrF_64_Format   .Text = value             ; } }

   #endregion _Fld

   #region override PutSpecificsFld() GetSpecificsFld()

   /*protected*/public override void PutSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd         )) Fld_KupDobCd           = mixer_rec.KupdobCD;
      if(CtrlOK(tbx_kupDobName       )) Fld_KupDobName         = mixer_rec.KupdobName;
      if(CtrlOK(tbx_kupDobTk         )) Fld_KupDobTk           = mixer_rec.KupdobTK;
      if(CtrlOK(tbx_ProjektCD        )) Fld_ProjektCD          = mixer_rec.ProjektCD;
                                        Fld_TtOpis             = ZXC.luiListaMixTypeEvidencija.GetNameForThisCd(mixer_rec.TT);
                                     
      if(CtrlOK(tbx_v1_tt            )) Fld_V1_tt              = mixer_rec.V1_tt;
      if(CtrlOK(tbx_v1_ttOpis        )) Fld_V1_ttOpis          = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V1_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v1_ttNum         )) Fld_V1_ttNum           = mixer_rec.V1_ttNum;
      if(CtrlOK(tbx_v2_tt            )) Fld_V2_tt              = mixer_rec.V2_tt;
      if(CtrlOK(tbx_v2_ttOpis        )) Fld_V2_ttOpis          = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V2_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v2_ttNum         )) Fld_V2_ttNum           = mixer_rec.V2_ttNum;
      if(CtrlOK(tbx_externLink1      )) Fld_ExternLink1        = mixer_rec.ExternLink1;
      if(CtrlOK(tbx_externLink2      )) Fld_ExternLink2        = mixer_rec.ExternLink2;
      if(CtrlOK(tbx_primjedba        )) Fld_Primjedba          = mixer_rec.StrB_128;
      if(CtrlOK(tbx_osoba            )) Fld_Osoba              = mixer_rec.StrG_40;
      if(CtrlOK(tbx_status           )) Fld_Status             = mixer_rec.StrH_32;
      if(CtrlOK(tbx_ValName          )) Fld_DevName            = mixer_rec.DevName;
      if(CtrlOK(tbx_iznos            )) Fld_Iznos              = mixer_rec.MoneyA;
      if(CtrlOK(tbx_StrA_40_BrNacr   )) Fld_StrA_40_BrNacr     = mixer_rec.StrA_40 ;
      if(CtrlOK(tbx_StrC_32_Ormar	 )) Fld_StrC_32_Ormar		= mixer_rec.StrC_32 ;
      if(CtrlOK(tbx_StrD_32_BrPapira )) Fld_StrD_32_BrPapira   = mixer_rec.StrD_32 ;
      if(CtrlOK(tbx_StrE_256_NazNacrt)) Fld_StrE_256_NazNacrt  = mixer_rec.StrE_256;
      if(CtrlOK(tbx_StrF_64_Format   )) Fld_StrF_64_Format     = mixer_rec.StrF_64 ;

      if(CtrlOK(tbx_projektIdent))
      {
         mixer_rec.prjFaktur_rec_LOADED = false;

         Fld_PrjIdent = mixer_rec.ProjektIdent1;
      }
   }

   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd         )) mixer_rec.KupdobCD    = Fld_KupDobCd;
      if(CtrlOK(tbx_kupDobName       )) mixer_rec.KupdobName  = Fld_KupDobName;
      if(CtrlOK(tbx_kupDobTk         )) mixer_rec.KupdobTK    = Fld_KupDobTk;
      if(CtrlOK(tbx_ProjektCD        )) mixer_rec.ProjektCD   = Fld_ProjektCD;
      if(CtrlOK(tbx_v1_tt            )) mixer_rec.V1_tt       = Fld_V1_tt;
      if(CtrlOK(tbx_v1_ttNum         )) mixer_rec.V1_ttNum    = Fld_V1_ttNum;
      if(CtrlOK(tbx_v2_tt            )) mixer_rec.V2_tt       = Fld_V2_tt;
      if(CtrlOK(tbx_v2_ttNum         )) mixer_rec.V2_ttNum    = Fld_V2_ttNum;
      if(CtrlOK(tbx_externLink1      )) mixer_rec.ExternLink1 = Fld_ExternLink1;
      if(CtrlOK(tbx_externLink2      )) mixer_rec.ExternLink2 = Fld_ExternLink2;
      if(CtrlOK(tbx_primjedba        )) mixer_rec.StrB_128    = Fld_Primjedba;
      if(CtrlOK(tbx_osoba            )) mixer_rec.StrG_40     = Fld_Osoba;
      if(CtrlOK(tbx_status           )) mixer_rec.StrH_32     = Fld_Status;
      if(CtrlOK(tbx_ValName          )) mixer_rec.DevName     = Fld_DevName;
      if(CtrlOK(tbx_iznos            )) mixer_rec.MoneyA      = Fld_Iznos  ;
      if(CtrlOK(tbx_StrA_40_BrNacr   )) mixer_rec.StrA_40     = Fld_StrA_40_BrNacr;
      if(CtrlOK(tbx_StrC_32_Ormar	 )) mixer_rec.StrC_32     = Fld_StrC_32_Ormar;
      if(CtrlOK(tbx_StrD_32_BrPapira )) mixer_rec.StrD_32     = Fld_StrD_32_BrPapira;
      if(CtrlOK(tbx_StrE_256_NazNacrt)) mixer_rec.StrE_256    = Fld_StrE_256_NazNacrt;
      if(CtrlOK(tbx_StrF_64_Format   )) mixer_rec.StrF_64     = Fld_StrF_64_Format;
 }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region PrintDocumentRecord

   public EvidencijaFilterUC TheEvidencijaFilterUC { get; set; }
   public EvidencijaFilter   TheEvidencijaFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.TheEvidencijaFilter = new EvidencijaFilter(this);

      TheEvidencijaFilterUC = new EvidencijaFilterUC(this);
      TheEvidencijaFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheEvidencijaFilterUC.Width;

   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      EvidencijaFilter mixerFilter = (EvidencijaFilter)vvRptFilter;

      switch(mixerFilter.PrintEvidencija)
      {
         case EvidencijaFilter.PrintEvidencijaEnum.Evidencija: specificMixerReport = new RptX_Evidencija(new Vektor.Reports.XIZ.CR_EvidencijaDUC(), "EVIDENCIJA", mixerFilter); break;

         default: ZXC.aim_emsg("{0}\nPrintSomeEvidencijaDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintEvidencija); return null;
      }

      return specificMixerReport;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheEvidencijaFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheEvidencijaFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
   }


   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_EVD, Color.Empty, clr_EVD);
   }

   #endregion Colors

   #region overrideTabPageTitle

   public override string TabPageTitle1
   {
      get { return ""; }
   }

   #endregion overrideTabPageTitle

}

public class EvidencijaFilterUC : VvFilterUC
{
   #region Fieldz

   #endregion Fieldz

   #region  Constructor

   public EvidencijaFilterUC(VvUserControl vvUC)
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

   private EvidencijaFilter TheEvidencijaFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as EvidencijaFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheEvidencijaFilter = (EvidencijaFilter)_filter_data;

      if(TheEvidencijaFilter != null)
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

public class EvidencijaFilter : VvRpt_Mix_Filter
{

   public enum PrintEvidencijaEnum
   {
      Evidencija
   }

   public PrintEvidencijaEnum PrintEvidencija { get; set; }

   public EvidencijaDUC theDUC;

   public EvidencijaFilter(EvidencijaDUC _theDUC)
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
      PrintEvidencija        = PrintEvidencijaEnum.Evidencija;
   }

   #endregion SetDefaultFilterValues()

}
