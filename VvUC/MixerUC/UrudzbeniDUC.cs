using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public partial class UrudzbeniDUC : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_kupDobCd, tbx_kupDobTk, tbx_kupDobName,
                     ////*DatNast  */tbx_dateA   ,
                     /*KlasOzn     */ tbx_strA_40 ,
                     /*Predmet     */ tbx_strB_128,
                     /*KlasSadrzOzn*/ tbx_strC_32,
                     /*UrBroj      */ tbx_strD_32,
                     /*Prijenos    */ tbx_strE_256,
                     /*UstrJed     */ tbx_strG_40 ,
                     /*OznRazv     */ tbx_strH_32 ,
                     /*DatRazv     */ tbx_dateB   ,
                                      tbx_klasOznOpis
                 ;
   public VvHamper hamp_Urudzbeni, hamp_partner, hamp_veze;
   public VvDateTimePicker dtp_dateB;
   private Button btn_nextNum;

   #endregion Fieldz

   #region Constructor

   public UrudzbeniDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul)
      : base(parent, _mixer, vvSubModul)
   {

      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_URZ
         });

   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {

      hamp_tt     .Location = new Point(ZXC.QunMrgn       , ZXC.QunMrgn);
      hamp_dokDate.Location = new Point(ZXC.QunMrgn, hamp_tt.Bottom);

      nextX = hamp_dokDate.Right;
      nextY = hamp_dokDate.Top;

      InitializeHamper_Partner(out hamp_partner);

      hamp_dokNum.Location = new Point(hamp_partner.Right - hamp_dokNum.Width, ZXC.QunMrgn);

      nextX = ZXC.QunMrgn;
      nextY = hamp_partner.Bottom;

      InitializeHamper_Urudzbeni(out hamp_Urudzbeni);

      nextY = hamp_Urudzbeni.Bottom;
      InitializeHamper_Veze     (out hamp_veze   );

      hamp_napomena.Location = new Point(nextX, hamp_veze.Bottom);
      nextY = hamp_napomena.Bottom;
   }


   private void InitializeHamper_Partner(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0                 1                
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un, ZXC.Q3un, ZXC.Q10un + ZXC.Q3un - ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4            };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel  (0, 0, "Partner:", ContentAlignment.MiddleRight);
      tbx_kupDobCd   = hamper.CreateVvTextBox(1, 0, "tbx_kupDobCd"  , "Sifra Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobCD)  );
      tbx_kupDobTk   = hamper.CreateVvTextBox(2, 0, "tbx_kupDobTk"  , "Tiker Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobTK)  );
      tbx_kupDobName = hamper.CreateVvTextBox(3, 0, "tbx_kupDobName", "Naziv Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobName));

      tbx_kupDobCd.JAM_CharEdits       = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_kupDobTk.JAM_CharacterCasing = CharacterCasing.Upper;

      //this.ControlForInitialFocus = tbx_kupDobName;

      tbx_kupDobCd  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobTk  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyKupdobTextBoxLeave));

      tbx_kupDobCd.JAM_FieldExitMethod += new EventHandler(OnExit_Set_Klasa);
      tbx_kupDobTk.JAM_FieldExitMethod += new EventHandler(OnExit_Set_Klasa);
      tbx_kupDobName.JAM_FieldExitMethod += new EventHandler(OnExit_Set_Klasa);
   }

   public void InitializeHamper_Urudzbeni(out VvHamper hamper)
   {
      hamper = new VvHamper(7, 7, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0         1        2          3         4         5              6          
      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q5un, ZXC.Q7un, ZXC.Q5un, ZXC.Q3un, ZXC.Q7un, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,                  0 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;


      hamper.CreateVvLabel(0, 0, "Datum nastanka:", ContentAlignment.MiddleRight);
      tbx_dateA = hamper.CreateVvTextBox(1, 0, "tbx_DatNast", "Datum nastanka pismene", GetDB_ColumnSize(DB_ci.dateA));
      tbx_dateA.JAM_IsForDateTimePicker = true;
      dtp_dateA = hamper.CreateVvDateTimePicker(1, 0, "", tbx_dateA);
      dtp_dateA.Name = "dtp_dateA";
      dtp_dateA.Leave += new EventHandler(OnExit_Set_Klasa);
      dtp_DokDate.Leave += new EventHandler(OnExit_Set_Klasa);

      VvLookUpLista.LoadResultLuiList_KlasifikJOB(); // !!! 

      hamper.CreateVvLabel(0, 1, "Klas-Sadrž/Ozn:", ContentAlignment.MiddleRight);
      tbx_strC_32 = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_strA_40", "KlasOzn", GetDB_ColumnSize(DB_ci.strC_32));
      tbx_strC_32.JAM_Set_LookUpTable(ZXC.luiListaKlasifikJOB, (int)ZXC.Kolona.prva);
      tbx_klasOznOpis = hamper.CreateVvTextBox(2, 1, "tbx_klasOznOpis", "tbx_klasOznOpis", 64, 3, 0);
      tbx_strC_32.JAM_lui_NameTaker_JAM_Name = tbx_klasOznOpis.JAM_Name;
      tbx_klasOznOpis.JAM_ReadOnly = true;
      tbx_strC_32.JAM_FieldExitMethod_2 = new EventHandler(OnExit_Set_Klasa);

      hamper.CreateVvLabel(0, 2, "Klasa:", ContentAlignment.MiddleRight);
      tbx_strA_40 = hamper.CreateVvTextBox(1, 2, "tbx_klasOznRO", "tbx_klasOznRO", GetDB_ColumnSize(DB_ci.strA_40), 1, 0);
      tbx_strA_40.JAM_ReadOnly = true;

      hamper.CreateVvLabel(3, 2, "Urudžbeni broj:", ContentAlignment.MiddleRight);
      tbx_strD_32 = hamper.CreateVvTextBox(4, 2, "tbx_UrBrAl", "Urudžbeni broj", GetDB_ColumnSize(DB_ci.strD_32), 1, 0);
      tbx_strD_32.JAM_ReadOnly = true;

      btn_nextNum = hamper.CreateVvButton(6, 2, new EventHandler(GetNextSifraWroot_String_btnClick), "");
      btn_nextNum.Name = "btn_nextNum";
      btn_nextNum.FlatStyle = FlatStyle.Flat;
      btn_nextNum.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_nextNum.Image = VvIco.TriangleGreenL24/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_greenLeft.ico")), 24, 24)*/.ToBitmap();
      btn_nextNum.TabStop = false;
    //btn_nextNum.Visible = false;


      hamper.CreateVvLabel(0, 3, "Predmet:", ContentAlignment.MiddleRight);
      tbx_strB_128 = hamper.CreateVvTextBox(1, 3, "tbx_Predmet", "Predmet ", GetDB_ColumnSize(DB_ci.strB_128), 4, 0);

      hamper.CreateVvLabel(0, 4, "Ustrojstvena jedinica:", ContentAlignment.MiddleRight);
      tbx_strG_40     /* */ = hamper.CreateVvTextBox(1, 4, "tbx_UstrJed", "Ustrojstvena jedinica ", GetDB_ColumnSize(DB_ci.strG_40));

      hamper.CreateVvLabel(2, 4, "Razvođenje: Datum:", ContentAlignment.MiddleRight);
      tbx_dateB     /*   */ = hamper.CreateVvTextBox(3, 4, "tbx_DatRazv", "Razvođenje: Datum:", GetDB_ColumnSize(DB_ci.dateB));
      tbx_dateB.JAM_IsForDateTimePicker = true;
      dtp_dateB = hamper.CreateVvDateTimePicker(3, 4, "", tbx_dateB);
      dtp_dateB.Name = "dtp_dateB";

      hamper.CreateVvLabel(4, 4, "Oznaka:", ContentAlignment.MiddleRight);
      tbx_strH_32     /* */ = hamper.CreateVvTextBox(5, 4, "tbx_OznRazv", "Razvođenje: Oznaka:", GetDB_ColumnSize(DB_ci.strH_32));

      hamper.CreateVvLabel(0, 5, "Prijenos:", ContentAlignment.MiddleRight);
      tbx_strE_256    /**/ = hamper.CreateVvTextBox(1, 5, "tbx_Prijenos", "Prijenos", GetDB_ColumnSize(DB_ci.strE_256), 4, 1);
      tbx_strE_256.Multiline = true;
      tbx_strE_256.ScrollBars = ScrollBars.Vertical;



   }

   private void InitializeHamper_Veze(out VvHamper hamper)
   {
      hamper = new VvHamper(11, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0               1                   2                  3            4                   5                   6              7        8            9           10       
      hamper.VvColWdt = new int[] { ZXC.Q6un, ZXC.Q3un - ZXC.Qun4, ZXC.Q2un, ZXC.Q6un, ZXC.QUN - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.Q2un + ZXC.Qun8, ZXC.Q3un - ZXC.Qun4, ZXC.Q6un, ZXC.Q3un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, 0, 0, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, 0 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;


      hamper.CreateVvLabel(0, 0, "Externi Link1:", ContentAlignment.MiddleRight);
      tbx_externLink1 = hamper.CreateVvTextBox(1, 0, "tbx_externLink1", "Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.externLink1), 2, 0);
      btn_goExLink1 = hamper.CreateVvButton(4, 0, new EventHandler(Link_ExternDokument_Click), "");
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

      hamper.CreateVvLabel(0, 1, "Externi Link2:", ContentAlignment.MiddleRight);
      tbx_externLink2 = hamper.CreateVvTextBox(1, 1, "tbx_externLink2", "Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.externLink2), 2, 0);
      btn_goExLink2 = hamper.CreateVvButton(4, 1, new EventHandler(Link_ExternDokument_Click), "");
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

      hamper.CreateVvLabel(6, 0, "Veza1:", ContentAlignment.MiddleRight);
      tbx_v1_tt = hamper.CreateVvTextBoxLookUp(7, 0, "tbx_v1_tt", "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v1_tt));
      tbx_v1_ttOpis = hamper.CreateVvTextBox(8, 0, "tbx_v1_ttOpis", "Veza na interni dokument", 32);
      tbx_v1_ttNum = hamper.CreateVvTextBox(9, 0, "tbx_v1_ttNum", "Veza na interni dokument", 6/*GetDB_ColumnSize(DB_ci.iz_ttNum)*/);
      btn_v1TT = hamper.CreateVvButton(10, 0, new EventHandler(GoTo_MIXER_Dokument_Click), "");

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

      hamper.CreateVvLabel(6, 1, "Veza2:", ContentAlignment.MiddleRight);
      tbx_v2_tt = hamper.CreateVvTextBoxLookUp(7, 1, "tbx_v2_tt", "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v2_tt));
      tbx_v2_ttOpis = hamper.CreateVvTextBox(8, 1, "tbx_v2_ttOpis", "Veza na interni dokument");
      tbx_v2_ttNum = hamper.CreateVvTextBox(9, 1, "tbx_v2_ttNum", "Veza na interni dokument" + " broj", 6/*GetDB_ColumnSize(DB_ci.na_ttNum)*/);
      btn_v2TT = hamper.CreateVvButton(10, 1, new EventHandler(GoTo_MIXER_Dokument_Click), "");
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

   public uint   Fld_KupDobCd      { get { return tbx_kupDobCd.GetSomeRecIDField(); } set { tbx_kupDobCd.PutSomeRecIDField(value); } }
   public string Fld_KupDobCdAsTxt { get { return tbx_kupDobCd  .Text;              } set { tbx_kupDobCd   .Text = value; } }
   public string Fld_KupDobName    { get { return tbx_kupDobName.Text;              } set { tbx_kupDobName .Text = value; } }
   public string Fld_KupDobTk      { get { return tbx_kupDobTk  .Text;              } set { tbx_kupDobTk   .Text = value; } }
   public string Fld_StrA_40       { get { return tbx_strA_40   .Text;              } set { tbx_strA_40    .Text = value; } }   /*Klasifikacijska Ozn RO */
   public string Fld_StrB_128      { get { return tbx_strB_128  .Text;              } set { tbx_strB_128   .Text = value; } }   /*Predmet                */
   public string Fld_StrC_32       { get { return tbx_strC_32   .Text;              } set { tbx_strC_32    .Text = value; } }   /*Klas Sadrzaj/Oznaka    */
   public string Fld_StrE_256      { get { return tbx_strE_256  .Text;              } set { tbx_strE_256   .Text = value; } }   /*Prijenos               */
   public string Fld_StrG_40       { get { return tbx_strG_40   .Text;              } set { tbx_strG_40    .Text = value; } }   /*UstrJed                */
   public string Fld_StrH_32       { get { return tbx_strH_32   .Text;              } set { tbx_strH_32    .Text = value; } }   /*OznRazv                */
   public string Fld_StrD_32       { get { return tbx_strD_32   .Text;              } set { tbx_strD_32    .Text = value; } }   /* Urudzbeni broj        */
   public string Fld_KlasOznOpis   {                                                  set { tbx_klasOznOpis.Text = value; } }

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
      if(CtrlOK(tbx_kupDobCd)) Fld_KupDobCd = mixer_rec.KupdobCD;
      if(CtrlOK(tbx_kupDobName)) Fld_KupDobName = mixer_rec.KupdobName;
      if(CtrlOK(tbx_kupDobTk)) Fld_KupDobTk = mixer_rec.KupdobTK;
      if(CtrlOK(tbx_ProjektCD)) Fld_ProjektCD = mixer_rec.ProjektCD;
      Fld_TtOpis = "URUDŽBENI ZAPISNIK";

      if(CtrlOK(tbx_v1_tt)) Fld_V1_tt = mixer_rec.V1_tt;
      if(CtrlOK(tbx_v1_ttOpis)) Fld_V1_ttOpis = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V1_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v1_ttNum)) Fld_V1_ttNum = mixer_rec.V1_ttNum;
      if(CtrlOK(tbx_v2_tt)) Fld_V2_tt = mixer_rec.V2_tt;
      if(CtrlOK(tbx_v2_ttOpis)) Fld_V2_ttOpis = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V2_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v2_ttNum)) Fld_V2_ttNum = mixer_rec.V2_ttNum;
      if(CtrlOK(tbx_externLink1)) Fld_ExternLink1 = mixer_rec.ExternLink1;
      if(CtrlOK(tbx_externLink2)) Fld_ExternLink2 = mixer_rec.ExternLink2;

      if(CtrlOK(tbx_strA_40)) Fld_StrA_40 = mixer_rec.StrA_40;
      if(CtrlOK(tbx_strB_128)) Fld_StrB_128 = mixer_rec.StrB_128;
      if(CtrlOK(tbx_strC_32)) Fld_StrC_32 = mixer_rec.StrC_32;
      if(CtrlOK(tbx_strE_256)) Fld_StrE_256 = mixer_rec.StrE_256;
      if(CtrlOK(tbx_strG_40)) Fld_StrG_40 = mixer_rec.StrG_40;
      if(CtrlOK(tbx_strH_32)) Fld_StrH_32 = mixer_rec.StrH_32;
      if(CtrlOK(tbx_strD_32)) Fld_StrD_32 = mixer_rec.StrD_32;
      if(CtrlOK(tbx_dateB)) Fld_DateB = mixer_rec.DateB;
      if(CtrlOK(tbx_klasOznOpis)) Fld_KlasOznOpis = ZXC.luiListaKlasifikJOB.GetNameForThisCd(mixer_rec.StrC_32);

   }

   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd)) mixer_rec.KupdobCD = Fld_KupDobCd;
      if(CtrlOK(tbx_kupDobName)) mixer_rec.KupdobName = Fld_KupDobName;
      if(CtrlOK(tbx_kupDobTk)) mixer_rec.KupdobTK = Fld_KupDobTk;
      if(CtrlOK(tbx_ProjektCD)) mixer_rec.ProjektCD = Fld_ProjektCD;
      if(CtrlOK(tbx_v1_tt)) mixer_rec.V1_tt = Fld_V1_tt;
      if(CtrlOK(tbx_v1_ttNum)) mixer_rec.V1_ttNum = Fld_V1_ttNum;
      if(CtrlOK(tbx_v2_tt)) mixer_rec.V2_tt = Fld_V2_tt;
      if(CtrlOK(tbx_v2_ttNum)) mixer_rec.V2_ttNum = Fld_V2_ttNum;
      if(CtrlOK(tbx_externLink1)) mixer_rec.ExternLink1 = Fld_ExternLink1;
      if(CtrlOK(tbx_externLink2)) mixer_rec.ExternLink2 = Fld_ExternLink2;

      if(CtrlOK(tbx_strA_40)) mixer_rec.StrA_40 = Fld_StrA_40;
      if(CtrlOK(tbx_strB_128)) mixer_rec.StrB_128 = Fld_StrB_128;
      if(CtrlOK(tbx_strC_32)) mixer_rec.StrC_32 = Fld_StrC_32;
      if(CtrlOK(tbx_strE_256)) mixer_rec.StrE_256 = Fld_StrE_256;
      if(CtrlOK(tbx_strG_40)) mixer_rec.StrG_40 = Fld_StrG_40;
      if(CtrlOK(tbx_strH_32)) mixer_rec.StrH_32 = Fld_StrH_32;
      if(CtrlOK(tbx_strD_32)) mixer_rec.StrD_32 = Fld_StrD_32;
      if(CtrlOK(tbx_dateB)) mixer_rec.DateB = Fld_DateB;

   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region PrintDocumentRecord

   public UrudzbeniFilterUC TheUrudzbeniFilterUC { get; set; }
   public UrudzbeniFilter TheUrudzbeniFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.TheUrudzbeniFilter = new UrudzbeniFilter(this);

      TheUrudzbeniFilterUC = new UrudzbeniFilterUC(this);
      TheUrudzbeniFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheUrudzbeniFilterUC.Width;

   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      UrudzbeniFilter mixerFilter = (UrudzbeniFilter)vvRptFilter;

      switch(mixerFilter.PrintUrudzbeni)
      {
         case UrudzbeniFilter.PrintUrudzbeniEnum.Urudzbeni : specificMixerReport = new RptX_Urudzbeni(new Vektor.Reports.XIZ.CR_UrudzbStambilj()  , "Urudzbeni" , mixerFilter); break;
         case UrudzbeniFilter.PrintUrudzbeniEnum.UrzPotvrda: specificMixerReport = new RptX_Urudzbeni(new Vektor.Reports.XIZ.CR_UrudzbeniPotvrda(), "UrzPotvrda", mixerFilter); break;

         default: ZXC.aim_emsg("{0}\nPrintSomeUrudzbeniDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintUrudzbeni); return null;
      }

      return specificMixerReport;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheUrudzbeniFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheUrudzbeniFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
   }


   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_URZ, Color.Empty, clr_URZ);
   }

   #endregion Colors

   #region overrideTabPageTitle

   public override string TabPageTitle1
   {
      get { return ""; }
   }

   #endregion overrideTabPageTitle

   private void OnExit_Set_Klasa/*AndUrBroj*/(object sender, EventArgs e)
   {
      // strC_32  - klasSadrzOznaka
      // dokDate  - datum primitka 
      // dateA    - datum nastanka 
      // str_A40  - Klasa    = 3klasifSadrzaj/yyNastajanja-klasifOblik/klasifRbr 
      // str_D32  - urBroj   = kupDobCd - yyPrimitka - ttKlasaNum u Klasi        

      string yyNastajanja = mixer_rec.DateA.ToString("yy");
      string yyPrimitka = mixer_rec.DokDate.ToString("yy");

      string klasifSadrzaj = "", klasifOblik = "", klasifRbr = "", klasadozn = mixer_rec.StrC_32, theKlasa = ""/*, theUrBroj = ""*/;

      string[] klasadozn_tokenStrArray = klasadozn.Split(new char[] { '/' }, StringSplitOptions.None);

      if(klasadozn_tokenStrArray.Length > 0) klasifSadrzaj = klasadozn_tokenStrArray[0];
      if(klasadozn_tokenStrArray.Length > 1) klasifOblik = klasadozn_tokenStrArray[1];
      if(klasadozn_tokenStrArray.Length > 2) klasifRbr = klasadozn_tokenStrArray[2];

      theKlasa = klasifSadrzaj + "/" + yyNastajanja + "-" + klasifOblik + "/" + klasifRbr;

      //theUrBroj = Fld_KupDobCdAsTxt + "-" + yyPrimitka + "-" + theKlasa_rbrNum.ToString("000");

      Fld_StrA_40 = theKlasa;
      //Fld_StrD_32 = theUrBroj;
   }

   public void GetNextSifraWroot_String_btnClick(object sender, EventArgs e)
   {
      string yyPrimitka = Fld_DokDate.ToString("yy");

      uint theKlasa_rbrNum = ZXC.MixerDao.GetNext_NAMED_TtNum(TheDbConnection, "strA_40", "SUBSTRING(strD_32, 11)", /*theKlasa*/Fld_StrA_40);

      string theUrBroj = Fld_KupDobCdAsTxt + "-" + yyPrimitka + "-" + theKlasa_rbrNum.ToString("0000");

      Fld_StrD_32 = theUrBroj;
      ZXC.TheVvForm.SetDirtyFlag(sender);
   }

}

public class UrudzbeniFilterUC : VvFilterUC
{
   #region Fieldz
  
   private VvHamper    hamp_rbt;
   private RadioButton rbt_stambilj, rbt_potvrda;

   #endregion Fieldz

   #region  Constructor

   public UrudzbeniFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;

      InitializeHamper_Rbt(out hamp_rbt);

      CreateHamper_4ButtonsResetGo_Width(hamp_rbt.Width);

      hamp_rbt.Location = new Point(nextX, hamper4buttons.Bottom + ZXC.Qun4);
      hamperHorLine.Visible = false;

      this.Width  = hamp_rbt.Width  + ZXC.QUN;
      this.Height = hamp_rbt.Bottom + ZXC.QUN;

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region hamper

   private void InitializeHamper_Rbt(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", this, false);

      hamper.VvColWdt      = new int[] { ZXC.Q8un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = ZXC.Qun4 + ZXC.Qun10;

      hamper.CreateVvLabel(0, 0, "Vrsta ispisa", ContentAlignment.MiddleLeft);

      rbt_potvrda  = hamper.CreateVvRadioButton(0, 1, null, "Potvrda", TextImageRelation.ImageBeforeText);
      rbt_stambilj = hamper.CreateVvRadioButton(0, 2, null, "Štambilj", TextImageRelation.ImageBeforeText);
      rbt_potvrda.Checked = true;
      rbt_potvrda.Tag = true;
      
      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.HamperStyling(hamper);
   }

   #endregion hamper

   #region Fld_

   public UrudzbeniFilter.PrintUrudzbeniEnum Fld_PrintUrudzbeni
   {
      get
      {
         if(rbt_potvrda.Checked) return UrudzbeniFilter.PrintUrudzbeniEnum.UrzPotvrda;
         else                    return UrudzbeniFilter.PrintUrudzbeniEnum.Urudzbeni ; 
      }
      set
      {
         switch(value)
         {
            case UrudzbeniFilter.PrintUrudzbeniEnum.UrzPotvrda: rbt_potvrda .Checked = true; break;
            case UrudzbeniFilter.PrintUrudzbeniEnum.Urudzbeni : rbt_stambilj.Checked = true; break;
         }
      }
   }


   #endregion Fld_

   #region Put & GetFilterFields

   private UrudzbeniFilter TheUrudzbeniFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as UrudzbeniFilter; }
      set { this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheUrudzbeniFilter = (UrudzbeniFilter)_filter_data;

      if(TheUrudzbeniFilter != null)
      {
         Fld_PrintUrudzbeni = TheUrudzbeniFilter.PrintUrudzbeni;
      }

      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
      TheUrudzbeniFilter.PrintUrudzbeni = Fld_PrintUrudzbeni;
   }

   #endregion Put & GetFilterFields

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
   }

   #endregion AddFilterMemberz()

}

public class UrudzbeniFilter : VvRpt_Mix_Filter
{

   public enum PrintUrudzbeniEnum
   {
      Urudzbeni, UrzPotvrda
   }

   public PrintUrudzbeniEnum PrintUrudzbeni { get; set; }

   public UrudzbeniDUC theDUC;

   public UrudzbeniFilter(UrudzbeniDUC _theDUC)
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
      PrintUrudzbeni = PrintUrudzbeniEnum.Urudzbeni;
   }

   #endregion SetDefaultFilterValues()

}
