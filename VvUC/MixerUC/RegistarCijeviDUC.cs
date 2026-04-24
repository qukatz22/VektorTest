using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public partial class RegistarCijeviDUC : MixerDUC
{
   #region Fieldz

   private VvTextBox

      /*artiklCd           */ tbx_artiklCd,
      /*(17)Cijev          */ tbx_strF_64,
      /*(18)Dimenzija      */ tbx_strG_40,
      /*(19)BrojSarze      */ tbx_strC_32 ,
      /*(20)Materijal      */ tbx_strD_32,
      /*(21)Standard       */ tbx_strB_128,
      /*(22)Proizvodac     */ tbx_kupDobCd, tbx_kupDobTk, tbx_kupDobName,
      /*(23)BrojUlAtesta   */ tbx_strA_40,
     //*(23)DatumUlAtesta  */ tbx_dateA,
      /*Kolicina           */ tbx_moneyA,
      /*() JedMjere        */ tbx_strH_32,
                              tbx_dateAt
      ;
   public VvHamper hamp_RegistarCijevi, hamp_partner, hamp_veze;
   public VvDateTimePicker dtp_dateAt;

   #endregion Fieldz

   #region Constructor

   public RegistarCijeviDUC(Control parent, Mixer _mixer, VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)
   {

      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_RGC
         });

   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {

      hamp_tt.Location      = new Point(ZXC.QunMrgn             , ZXC.Qun4);
      hamp_dokDate.Location = new Point(hamp_tt.Right + ZXC.QUN + ZXC.Qun2, ZXC.Qun4);

      nextX = ZXC.QunMrgn;
      nextY = hamp_tt.Bottom;

      InitializeHamper_RegistarCijevi(out hamp_RegistarCijevi);

      hamp_dokNum.Location = new Point(hamp_RegistarCijevi.Right - hamp_dokNum.Width, ZXC.Qun4);

      nextY = hamp_RegistarCijevi.Bottom;

      InitializeHamper_Partner(out hamp_partner);


      nextX = ZXC.QunMrgn;
      nextY = hamp_partner.Bottom;

      InitializeHamper_Veze(out hamp_veze);

      hamp_napomena.Location = new Point(nextX, hamp_veze.Bottom);
      nextY = hamp_napomena.Bottom;
   }

   public void InitializeHamper_RegistarCijevi(out VvHamper hamper)
   {
      hamper = new VvHamper(9, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0         1         2          3        4          5       6         7        8       
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q5un, ZXC.Q5un, ZXC.Q4un, ZXC.Q5un, ZXC.Q5un, ZXC.Q4un, ZXC.Q3un, ZXC.Q2un - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;


                     hamper.CreateVvLabel  (0, 0, "Cijev:", ContentAlignment.MiddleRight);
      tbx_artiklCd = hamper.CreateVvTextBox(1, 0, "tbx_artiklCd", "ArtiklCd", GetDB_ColumnSize(DB_ci.personPrezim));
      tbx_strF_64  = hamper.CreateVvTextBox(2, 0, "tbx_strF_64", "Cijev"    , GetDB_ColumnSize(DB_ci.strF_64), 1, 0);

      tbx_artiklCd.JAM_SetAutoCompleteData(Artikl.recordName, Artikl.sorterCD.SortType  , new EventHandler(OnVvTBEnter_SetAutocmplt_Artikl_sorterSifra), new EventHandler(AnyArtiklTextBox_Leave));
      tbx_strF_64 .JAM_SetAutoCompleteData(Artikl.recordName, Artikl.sorterName.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Artikl_sorterName) , new EventHandler(AnyArtiklTextBox_Leave));

                     hamper.CreateVvLabel  (4, 0, "Dimenzija:", ContentAlignment.MiddleRight);
      tbx_strG_40  = hamper.CreateVvTextBox(5, 0, "tbx_strG_40", "Dimenzija cijevi", GetDB_ColumnSize(DB_ci.strG_40));

                     hamper.CreateVvLabel  (6, 0, "Broj šarže:", ContentAlignment.MiddleRight);
      tbx_strC_32  = hamper.CreateVvTextBox(7, 0, "tbx_strC_32", "Broj šarže", GetDB_ColumnSize(DB_ci.strC_32), 1, 0);

                    hamper.CreateVvLabel   (0, 1, "Materijal:", ContentAlignment.MiddleRight);
      tbx_strD_32  = hamper.CreateVvTextBox(1, 1, "tbx_strD_32", "Materijal", GetDB_ColumnSize(DB_ci.strD_32));

                     hamper.CreateVvLabel  (2, 1, "Standard:", ContentAlignment.MiddleRight);
      tbx_strB_128 = hamper.CreateVvTextBox(3, 1, "tbx_strB_128", "Standard", GetDB_ColumnSize(DB_ci.strB_128), 1, 0);
      
                     hamper.CreateVvLabel  (5, 1, "Količina:", ContentAlignment.MiddleRight);
      tbx_moneyA   = hamper.CreateVvTextBox(6, 1, "tbx_moneyA", "Količina:", GetDB_ColumnSize(DB_ci.moneyA));
      tbx_moneyA.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      
                     hamper.CreateVvLabel  (7, 1, "Jed.mjere:", ContentAlignment.MiddleRight);
      tbx_strH_32  = hamper.CreateVvTextBox(8, 1, "tbx_strH_32", "Jed.mjere", GetDB_ColumnSize(DB_ci.strH_32));

      this.ControlForInitialFocus = tbx_strF_64;

   }

   private void InitializeHamper_Partner(out VvHamper hamper)
   {
      hamper = new VvHamper(8, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0                 1                
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un, ZXC.Q3un, ZXC.Q10un, ZXC.Q4un, ZXC.Q5un, ZXC.Q4un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel  (0, 0, "Proizvođač:"   , ContentAlignment.MiddleRight);
      tbx_kupDobCd   = hamper.CreateVvTextBox(1, 0, "tbx_kupDobCd"  , "Sifra Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobCD));
      tbx_kupDobTk   = hamper.CreateVvTextBox(2, 0, "tbx_kupDobTk"  , "Tiker Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobTK));
      tbx_kupDobName = hamper.CreateVvTextBox(3, 0, "tbx_kupDobName", "Naziv Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobName));

      tbx_kupDobCd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_kupDobTk.JAM_CharacterCasing = CharacterCasing.Upper;

      //this.ControlForInitialFocus = tbx_kupDobName;

      tbx_kupDobCd  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobTk  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyKupdobTextBoxLeave));

                    hamper.CreateVvLabel  (4, 0, "Broj atesta:", ContentAlignment.MiddleRight);
      tbx_strA_40 = hamper.CreateVvTextBox(5, 0, "tbx_strA_40" , "Broj ulaznog atesta", GetDB_ColumnSize(DB_ci.strA_40));

                    hamper.CreateVvLabel(6, 0, "Datum atesta:", ContentAlignment.MiddleRight);
      tbx_dateAt = hamper.CreateVvTextBox(7, 0, "tbx_dateAt", "Datum atesta", GetDB_ColumnSize(DB_ci.dateA));
      tbx_dateAt.JAM_IsForDateTimePicker = true;
      dtp_dateAt = hamper.CreateVvDateTimePicker(7, 0, "", tbx_dateAt);
      dtp_dateAt.Name = "dtp_dateAt";
      
   }

   private void InitializeHamper_Veze(out VvHamper hamper)
   {
      hamper = new VvHamper(11, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0               1              2         3              4                   5                   6                     7               8               9                 10       
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un - ZXC.Qun4, ZXC.Q2un, ZXC.Q10un, ZXC.QUN - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.Q4un - ZXC.Qun4, ZXC.Q3un - ZXC.Qun4, ZXC.Q7un, ZXC.Q3un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 ,                  0,                  0,            ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4,                  0 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
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

   private void AnyArtiklTextBox_Leave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Artikl  artikl_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         artikl_rec = ArtiklSifrar.Find(FoundInSifrar<Artikl>);

         if(artikl_rec != null && tb.Text != "")
         {
            Fld_ArtiklCd = artikl_rec.ArtiklCD;
            Fld_StrF_64  = artikl_rec.ArtiklName;
         }
         else if(this.sifrarSorterType == VvSQL.SorterType.Name && tb.Text != "") // ako smo dosli iz naziva, a artikl_rec je null, to je onda 'qwe' pattern (ne postoji kao sifrar) 
         {
            //Fld_PrjArtName = tb.Text;
            Fld_ArtiklCd = "";
         }
         else
         {
            Fld_ArtiklCd = Fld_StrF_64 = "";
         }
      }
   }

   #endregion CreateSpecificHampers()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_kpdbZiroA_32_CreateColumn(ZXC.Q3un , "Šifra"     , "Šifra"     , ZXC.luiListaSerlot); 
      T_vezniDokA_64_CreateColumn(ZXC.Q10un, "Opis"      , "Opis"      , null);
      T_kpdbNameA_50_CreateColumn(ZXC.Q10un, "Vrijednost", "Vrijednost", false);
      T_opis_128_CreateColumn    (        0, "Komentar"  , "Komentar"  , 128, null);
      
      vvtbT_kpdbZiroA_32.JAM_lui_NameTaker_JAM_Name = TheVvDaoTrans.GetSchemaColumnName(DB_Tci.t_vezniDokA_64);

   }

   #endregion InitializeTheGrid_Columns()

   #region Fld_

   public uint    Fld_KupDobCd      { get { return tbx_kupDobCd  .GetSomeRecIDField(); } set { tbx_kupDobCd.PutSomeRecIDField(value); } }
   public string  Fld_KupDobCdAsTxt { get { return tbx_kupDobCd  .Text;                } set { tbx_kupDobCd  .Text = value; } }
   public string  Fld_KupDobName    { get { return tbx_kupDobName.Text;                } set { tbx_kupDobName.Text = value; } }
   public string  Fld_KupDobTk      { get { return tbx_kupDobTk  .Text;                } set { tbx_kupDobTk  .Text = value; } }
   public string  Fld_StrA_40       { get { return tbx_strA_40   .Text;                } set { tbx_strA_40   .Text = value; } }
   public string  Fld_StrB_128      { get { return tbx_strB_128  .Text;                } set { tbx_strB_128  .Text = value; } }
   public string  Fld_StrC_32       { get { return tbx_strC_32   .Text;                } set { tbx_strC_32   .Text = value; } }
   public string  Fld_StrF_64       { get { return tbx_strF_64   .Text;                } set { tbx_strF_64   .Text = value; } }
   public string  Fld_StrG_40       { get { return tbx_strG_40   .Text;                } set { tbx_strG_40   .Text = value; } }
   public string  Fld_StrH_32       { get { return tbx_strH_32   .Text;                } set { tbx_strH_32   .Text = value; } }
   public string  Fld_StrD_32       { get { return tbx_strD_32   .Text;                } set { tbx_strD_32   .Text = value; } }
   public decimal Fld_MoneyA        { get { return tbx_moneyA.GetDecimalField();       } set { tbx_moneyA.PutDecimalField(value); } }
   public string  Fld_ArtiklCd      { get { return tbx_artiklCd  .Text;                } set { tbx_artiklCd  .Text = value; } }

   public DateTime Fld_DateAt
   {
      get { return dtp_dateAt.Value; }
      set
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dtp_dateAt.Value = value;
         }
      }
   }


   #endregion _Fld

   #region override PutSpecificsFld() GetSpecificsFld()

   /*protected*/
   public override void PutSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd  )) Fld_KupDobCd   = mixer_rec.KupdobCD;
      if(CtrlOK(tbx_kupDobName)) Fld_KupDobName = mixer_rec.KupdobName;
      if(CtrlOK(tbx_kupDobTk  )) Fld_KupDobTk   = mixer_rec.KupdobTK;
      if(CtrlOK(tbx_ProjektCD )) Fld_ProjektCD  = mixer_rec.ProjektCD;
      Fld_TtOpis = "REGISTAR CIJEVI";

      if(CtrlOK(tbx_v1_tt      )) Fld_V1_tt       = mixer_rec.V1_tt;
      if(CtrlOK(tbx_v1_ttOpis  )) Fld_V1_ttOpis   = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V1_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v1_ttNum   )) Fld_V1_ttNum    = mixer_rec.V1_ttNum;
      if(CtrlOK(tbx_v2_tt      )) Fld_V2_tt       = mixer_rec.V2_tt;
      if(CtrlOK(tbx_v2_ttOpis  )) Fld_V2_ttOpis   = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V2_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v2_ttNum   )) Fld_V2_ttNum    = mixer_rec.V2_ttNum;
      if(CtrlOK(tbx_externLink1)) Fld_ExternLink1 = mixer_rec.ExternLink1;
      if(CtrlOK(tbx_externLink2)) Fld_ExternLink2 = mixer_rec.ExternLink2;

      if(CtrlOK(tbx_strA_40 )) Fld_StrA_40  = mixer_rec.StrA_40;
      if(CtrlOK(tbx_strB_128)) Fld_StrB_128 = mixer_rec.StrB_128;
      if(CtrlOK(tbx_strC_32 )) Fld_StrC_32  = mixer_rec.StrC_32;
      if(CtrlOK(tbx_strF_64 )) Fld_StrF_64  = mixer_rec.StrF_64;
      if(CtrlOK(tbx_strG_40 )) Fld_StrG_40  = mixer_rec.StrG_40;
      if(CtrlOK(tbx_strH_32 )) Fld_StrH_32  = mixer_rec.StrH_32;
      if(CtrlOK(tbx_strD_32 )) Fld_StrD_32  = mixer_rec.StrD_32;
      if(CtrlOK(tbx_moneyA  )) Fld_MoneyA   = mixer_rec.MoneyA;
      if(CtrlOK(tbx_artiklCd)) Fld_ArtiklCd = mixer_rec.PersonPrezim;
      if(CtrlOK(tbx_dateAt  )) Fld_DateAt   = mixer_rec.DateA;


   }

   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd   )) mixer_rec.KupdobCD     = Fld_KupDobCd;
      if(CtrlOK(tbx_kupDobName )) mixer_rec.KupdobName   = Fld_KupDobName;
      if(CtrlOK(tbx_kupDobTk   )) mixer_rec.KupdobTK     = Fld_KupDobTk;
      if(CtrlOK(tbx_ProjektCD  )) mixer_rec.ProjektCD    = Fld_ProjektCD;
      if(CtrlOK(tbx_v1_tt      )) mixer_rec.V1_tt        = Fld_V1_tt;
      if(CtrlOK(tbx_v1_ttNum   )) mixer_rec.V1_ttNum     = Fld_V1_ttNum;
      if(CtrlOK(tbx_v2_tt      )) mixer_rec.V2_tt        = Fld_V2_tt;
      if(CtrlOK(tbx_v2_ttNum   )) mixer_rec.V2_ttNum     = Fld_V2_ttNum;
      if(CtrlOK(tbx_externLink1)) mixer_rec.ExternLink1  = Fld_ExternLink1;
      if(CtrlOK(tbx_externLink2)) mixer_rec.ExternLink2  = Fld_ExternLink2;

      if(CtrlOK(tbx_strA_40    )) mixer_rec.StrA_40      = Fld_StrA_40;
      if(CtrlOK(tbx_strB_128   )) mixer_rec.StrB_128     = Fld_StrB_128;
      if(CtrlOK(tbx_strC_32    )) mixer_rec.StrC_32      = Fld_StrC_32;
      if(CtrlOK(tbx_strF_64    )) mixer_rec.StrF_64      = Fld_StrF_64;
      if(CtrlOK(tbx_strG_40    )) mixer_rec.StrG_40      = Fld_StrG_40;
      if(CtrlOK(tbx_strH_32    )) mixer_rec.StrH_32      = Fld_StrH_32;
      if(CtrlOK(tbx_strD_32    )) mixer_rec.StrD_32      = Fld_StrD_32;
      if(CtrlOK(tbx_moneyA     )) mixer_rec.MoneyA       = Fld_MoneyA;
      if(CtrlOK(tbx_artiklCd   )) mixer_rec.PersonPrezim = Fld_ArtiklCd ;
      if(CtrlOK(tbx_dateAt     )) mixer_rec.DateA        = Fld_DateAt;

   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region PrintDocumentRecord

   public RegistarCijeviFilterUC TheRegistarCijeviFilterUC { get; set; }
   public RegistarCijeviFilter TheRegistarCijeviFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.TheRegistarCijeviFilter = new RegistarCijeviFilter(this);

      TheRegistarCijeviFilterUC = new RegistarCijeviFilterUC(this);
      TheRegistarCijeviFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheRegistarCijeviFilterUC.Width;

   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      RegistarCijeviFilter mixerFilter = (RegistarCijeviFilter)vvRptFilter;

      switch(mixerFilter.PrintRegistarCijevi)
      {
         //case RegistarCijeviFilter.PrintRegistarCijeviEnum.RegistarCijevi: specificMixerReport = new RptX_RegistarCijevi(new Vektor.Reports.XIZ.CR_UrudzbStambilj(), "RegistarCijevi", mixerFilter); break;
         //case RegistarCijeviFilter.PrintRegistarCijeviEnum.UrzPotvrda: specificMixerReport = new RptX_RegistarCijevi(new Vektor.Reports.XIZ.CR_RegistarCijeviPotvrda(), "UrzPotvrda", mixerFilter); break;

         default: ZXC.aim_emsg("{0}\nPrintSomeRegistarCijeviDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintRegistarCijevi); return null;
      }

      return specificMixerReport;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheRegistarCijeviFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheRegistarCijeviFilterUC;
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

   public void Load_RGC_FromLookUpList(VvLookUpLista theLookUpList)
   {
      Mixer mixer_rec   = new Mixer();
      Xtrans xtrans_rec = new Xtrans();

      ushort line = 0;

      mixer_rec.TT      = Mixer.TT_RGC;
      mixer_rec.DokDate = DateTime.Now;
      //mixer_rec.Napomena = mixer_rec.StrC_32;

      foreach(VvLookUpItem lui in theLookUpList)
      {
         
         xtrans_rec.Memset0(0);

         xtrans_rec.T_kpdbZiroA_32 = lui.Cd;                           // Sifra
         xtrans_rec.T_vezniDokA_64 = ZXC.LenLimitedStr(lui.Name, 64); // Opis

         MixerDao.AutoSetMixer(TheDbConnection, ref line, mixer_rec, xtrans_rec);
      }

   }

}

public class RegistarCijeviFilterUC : VvFilterUC
{
   #region Fieldz

   private VvHamper hamp_rbt;
   private RadioButton rbt_stambilj, rbt_potvrda;

   #endregion Fieldz

   #region  Constructor

   public RegistarCijeviFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;

      InitializeHamper_Rbt(out hamp_rbt);

      CreateHamper_4ButtonsResetGo_Width(hamp_rbt.Width);

      hamp_rbt.Location = new Point(nextX, hamper4buttons.Bottom + ZXC.Qun4);
      hamperHorLine.Visible = false;

      this.Width = hamp_rbt.Width + ZXC.QUN;
      this.Height = hamp_rbt.Bottom + ZXC.QUN;

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region hamper

   private void InitializeHamper_Rbt(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", this, false);

      hamper.VvColWdt = new int[] { ZXC.Q8un };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = ZXC.Qun4 + ZXC.Qun10;

      hamper.CreateVvLabel(0, 0, "Vrsta ispisa", ContentAlignment.MiddleLeft);

      rbt_potvrda = hamper.CreateVvRadioButton(0, 1, null, "Potvrda", TextImageRelation.ImageBeforeText);
      rbt_stambilj = hamper.CreateVvRadioButton(0, 2, null, "Štambilj", TextImageRelation.ImageBeforeText);
      rbt_potvrda.Checked = true;
      rbt_potvrda.Tag = true;

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.HamperStyling(hamper);
   }

   #endregion hamper

   #region Fld_

   public RegistarCijeviFilter.PrintRegistarCijeviEnum Fld_PrintRegistarCijevi
   {
      get
      {
         if(rbt_potvrda.Checked) return RegistarCijeviFilter.PrintRegistarCijeviEnum.UrzPotvrda;
         else return RegistarCijeviFilter.PrintRegistarCijeviEnum.RegistarCijevi;
      }
      set
      {
         switch(value)
         {
            case RegistarCijeviFilter.PrintRegistarCijeviEnum.UrzPotvrda: rbt_potvrda.Checked = true; break;
            case RegistarCijeviFilter.PrintRegistarCijeviEnum.RegistarCijevi: rbt_stambilj.Checked = true; break;
         }
      }
   }


   #endregion Fld_

   #region Put & GetFilterFields

   private RegistarCijeviFilter TheRegistarCijeviFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as RegistarCijeviFilter; }
      set { this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheRegistarCijeviFilter = (RegistarCijeviFilter)_filter_data;

      if(TheRegistarCijeviFilter != null)
      {
         Fld_PrintRegistarCijevi = TheRegistarCijeviFilter.PrintRegistarCijevi;
      }

      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
      TheRegistarCijeviFilter.PrintRegistarCijevi = Fld_PrintRegistarCijevi;
   }

   #endregion Put & GetFilterFields

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
   }

   #endregion AddFilterMemberz()

}

public class RegistarCijeviFilter : VvRpt_Mix_Filter
{

   public enum PrintRegistarCijeviEnum
   {
      RegistarCijevi, UrzPotvrda
   }

   public PrintRegistarCijeviEnum PrintRegistarCijevi { get; set; }

   public RegistarCijeviDUC theDUC;

   public RegistarCijeviFilter(RegistarCijeviDUC _theDUC)
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
      PrintRegistarCijevi = PrintRegistarCijeviEnum.RegistarCijevi;
   }

   #endregion SetDefaultFilterValues()

}
