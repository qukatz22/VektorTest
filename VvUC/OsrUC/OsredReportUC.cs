using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using CrystalDecisions.Windows.Forms;

public class AmoReportUC : VvReportUC
{
   #region Fieldz
  
   private VvHamper    hamp_kupDobMjtr, hamp_AnalSint, hamp_redosljedKartica, hamp_rashodaktiv, hamp_razdoblje;
   public  VvTextBox   tbx_KD_sifra, tbx_KD_ticker, tbx_KD_naziv,
                       tbx_mtrosID, tbx_mtrosTick, tbx_mtrosNaziv;
   private RadioButton rbt_sve, rbt_rashod, rbt_aktiv, rbt_analitika, rbt_sintetika, rbt_godina,rbt_polugod, rbt_kvartal, rbt_mjesec, rbt_SortNaziv, rbt_SortSifra;
   private CheckBox    cbx_isInvBrUmjSifre;

   private int razmakIzmjedjuHampera;
   private int nextX, nextY, razmakHamp = ZXC.Qun10;
   private int maxHampWidth;

   public AmoFilterUC TheOsredFilterUC { get; set; }

   #endregion Fieldz

   #region Constructor

   public AmoReportUC(Control parent, VvRpt_Osred_Filter _rptFilter, VvSubModul vvSubModul)
   {
     this.TheSubModul  = vvSubModul;
     this.TheRptFilter = _rptFilter;
     
     SuspendLayout();

     TheOsredFilterUC        = new AmoFilterUC(this);
     TheOsredFilterUC.Parent = TheFilterPanel;
     maxHampWidth            = TheOsredFilterUC.MaxHamperWidth;
     razmakIzmjedjuHampera   = TheOsredFilterUC.razmakIzmjedjuHampera;

     TheOsredFilterUC.Height  = TheOsredFilterUC.Height - TheOsredFilterUC.hamperHorLine.Height
                                                        - TheOsredFilterUC.hamp_redosljedDokument.Height
                                                        - (4 * razmakIzmjedjuHampera);

     nextX = razmakIzmjedjuHampera;
     nextY = TheOsredFilterUC.Height;

     InitializeHamper_kupdob(out hamp_kupDobMjtr);
     nextY = hamp_kupDobMjtr.Bottom + razmakIzmjedjuHampera;

     TheOsredFilterUC.hamp_redosljedDokument.Location = new Point(nextX, nextY);
     TheOsredFilterUC.hamp_redosljedDokument.Parent   = TheFilterPanel;

     nextX = TheOsredFilterUC.hamp_redosljedDokument.Right + razmakIzmjedjuHampera;
     InitializeHamper_RedosljedKartica(out hamp_redosljedKartica);

     nextY = hamp_redosljedKartica.Bottom + razmakIzmjedjuHampera;
     nextX = TheOsredFilterUC.hamp_redosljedDokument.Left;

     InitializeHamper_RashodAktiv(out hamp_rashodaktiv);

     nextX = hamp_rashodaktiv.Right + razmakIzmjedjuHampera;
     InitializeHamper_Razdoblje(out hamp_razdoblje);

     nextY = hamp_rashodaktiv.Bottom + razmakIzmjedjuHampera;
     nextX = razmakIzmjedjuHampera;
     
     InitializeHamper_analitikaSintetika(out hamp_AnalSint);
     nextY = hamp_AnalSint.Bottom + razmakIzmjedjuHampera;

     TheOsredFilterUC.hamperHorLine.Location = new Point(nextX, nextY);
     TheOsredFilterUC.hamperHorLine.Parent   = TheFilterPanel;

     nextY = TheOsredFilterUC.hamperHorLine.Bottom + razmakIzmjedjuHampera;

     InitializeVvUserControl(parent);
     
     TheFilterPanel_Width = TheOsredFilterUC.Width;
     CalcTheFilterPanelWidth();

     //  ******************************************************************************************************
     // VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
     // nemoj se opet zajebati ovo nemere ici tu jerbo cisti filterUC dolazi na recordTabPage i on mora imati svoj
     // pa ti dodjeli tamo koamo treba tj. u hampere koji imaju vvTexBoxove ... 
     //  ******************************************************************************************************

     ResumeLayout();
   }

   #endregion Constructor

   #region hamper

   private void InitializeHamper_kupdob(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 4, "", TheFilterPanel, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q4un ,  ZXC.Q2un - ZXC.Qun8, ZXC.Q4un  };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8,              ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
      }
      hamper.VvSpcBefRow = new int[] { ZXC.Qun4, ZXC.Qun8, ZXC.Qun4, ZXC.Qun8 };

      hamper.VvBottomMargin = hamper.VvTopMargin;
   
      Label lbKupDob, lbMjTr;

      lbKupDob      = hamper.CreateVvLabel  (0, 0, "Partner:"      , ContentAlignment.MiddleRight);
 
      tbx_KD_sifra  = hamper.CreateVvTextBox(1, 0, "tbx_KD_sifra" , "Sfra dobavljaca", 6);
      tbx_KD_ticker = hamper.CreateVvTextBox(3, 0, "tbx_KD_ticker", "Ticker dobavljaca", 6);
      tbx_KD_naziv  = hamper.CreateVvTextBox(1, 1, "tbx_KupDob"   , "Naziv dobavljaca", 30, 2, 0);

      tbx_KD_sifra.JAM_MustTabOutBeforeSubmit  = true;
      tbx_KD_ticker.JAM_MustTabOutBeforeSubmit = true;
      tbx_KD_naziv.JAM_MustTabOutBeforeSubmit  = true;

      tbx_KD_sifra.JAM_SetAutoCompleteData (Kupdob.recordName, Kupdob.sorterKCD.SortType    , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyKupdobTextBoxLeave));
      tbx_KD_ticker.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_KD_naziv.JAM_SetAutoCompleteData (Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyKupdobTextBoxLeave));

      tbx_KD_ticker.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_KD_sifra.JAM_FillCharacter = '0';
      tbx_KD_sifra.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_KD_sifra.TextAlign         = HorizontalAlignment.Left;

      lbMjTr       = hamper.CreateVvLabel(0, 2, "MjTros:", ContentAlignment.MiddleRight);

      tbx_mtrosID    = hamper.CreateVvTextBox(1, 2, "tbx_mtrosID"  , "Sifra mjesta troska", 6);
      tbx_mtrosTick  = hamper.CreateVvTextBox(3, 2, "tbx_mtrosTick", "Ticker mjesta troska", 6);
      tbx_mtrosNaziv = hamper.CreateVvTextBox(1, 3, "tbx_mtrosNaziv", "Naziv mjesta troska", 30, 2, 0);

      tbx_mtrosTick.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_mtrosID.JAM_FillCharacter = '0';
      tbx_mtrosID.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_mtrosID.TextAlign         = HorizontalAlignment.Left;

      tbx_mtrosID.JAM_MustTabOutBeforeSubmit    = true;
      tbx_mtrosTick.JAM_MustTabOutBeforeSubmit  = true;
      tbx_mtrosNaziv.JAM_MustTabOutBeforeSubmit = true;

      tbx_mtrosID.   JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType    , ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyMtrosTextBoxLeave));
      tbx_mtrosTick. JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_mtrosNaziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyMtrosTextBoxLeave));

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, TheOsredFilterUC.MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void AnyKupdobTextBoxLeave(object sender, EventArgs e)
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
            Fld_KDNaziv  = kupdob_rec.Naziv;
            Fld_KDSifra  = kupdob_rec.KupdobCD/*RecID*/;
            Fld_KDTicker = kupdob_rec.Ticker;
         }
         else
         {
            Fld_KDNaziv = Fld_KDTicker = Fld_KDSifraAsTxtOd = "";
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
            Fld_MjTrSifra  = kupdob_rec.KupdobCD/*RecID*/;
            Fld_MjTrTicker = kupdob_rec.Ticker;
            Fld_MjTrNaziv  = kupdob_rec.Naziv;
         }
         else
         {
            Fld_MjTrSifraAsTxtOd = Fld_MjTrTicker = Fld_MjTrNaziv = "";
         }
      }
   }

   private void InitializeHamper_RedosljedKartica(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", TheFilterPanel, false, nextX, nextY, TheOsredFilterUC.razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q6un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 + ZXC.Qun5 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "Slijed ispisa Kartice:", ContentAlignment.MiddleLeft);
      
      rbt_SortNaziv = hamper.CreateVvRadioButton(0, 1, null, "Po nazivu", TextImageRelation.ImageBeforeText);
      rbt_SortNaziv.Checked = true;
      rbt_SortSifra = hamper.CreateVvRadioButton(0, 2, null, "Po šifri", TextImageRelation.ImageBeforeText);

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, TheOsredFilterUC.MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_RashodAktiv(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 4, "", TheFilterPanel, false, nextX, nextY, TheOsredFilterUC.razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q6un + ZXC.Qun4 + ZXC.Qun5};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 + ZXC.Qun5 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "Ispis DI:", ContentAlignment.MiddleLeft);
      
      rbt_sve    = hamper.CreateVvRadioButton(0, 1, null, "Sve"        , TextImageRelation.ImageBeforeText);
      rbt_sve.Checked = true;
      rbt_aktiv  = hamper.CreateVvRadioButton(0, 2, null, "Aktivne"    , TextImageRelation.ImageBeforeText);
      rbt_rashod = hamper.CreateVvRadioButton(0, 3, null, "Rashodovane", TextImageRelation.ImageBeforeText);

      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_Razdoblje(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 5, "", TheFilterPanel, false, nextX, nextY, TheOsredFilterUC.razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q6un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 + ZXC.Qun5 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "Ispis za razdoblje:", ContentAlignment.MiddleLeft);

      rbt_godina = hamper.CreateVvRadioButton(0, 1, null, "Godina", TextImageRelation.ImageBeforeText);
      rbt_godina.Checked = true;
      rbt_polugod = hamper.CreateVvRadioButton(0, 2, null, "Polugod", TextImageRelation.ImageBeforeText);
      rbt_kvartal = hamper.CreateVvRadioButton(0, 3, null, "Kvartal", TextImageRelation.ImageBeforeText);
      rbt_mjesec  = hamper.CreateVvRadioButton(0, 4, null, "Mjesec" , TextImageRelation.ImageBeforeText);

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, TheOsredFilterUC.MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }
   
   private void InitializeHamper_analitikaSintetika(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", TheFilterPanel, false, nextX, nextY, TheOsredFilterUC.razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q6un + ZXC.Qun4+ ZXC.Qun5 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 + ZXC.Qun5 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "Analalitika/Sintetika:", ContentAlignment.MiddleLeft);
      
      rbt_analitika = hamper.CreateVvRadioButton(0, 1, null, "Analitika", TextImageRelation.ImageBeforeText);
      rbt_analitika.Checked = true;
      rbt_sintetika = hamper.CreateVvRadioButton(0, 2, null, "Sintetika", TextImageRelation.ImageBeforeText);
     
      VvHamper.HamperStyling(hamper);
   }

   #region hamperDrillDown 

   //private void InitializeHamper_DrillDown(out VvHamper hamper)
   //{
   //   hamper = new VvHamper(1,1, "", TheFilterPanel, false, nextX, nextY, razmakHamp);

   //   hamper.VvColWdt      = new int[] { ZXC.Q6un + ZXC.Qun4 };
   //   hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 + ZXC.Qun5 };
   //   hamper.VvRightMargin = hamper.VvLeftMargin;
      
   //   for (int i = 0; i < hamper.VvNumOfRows; i++)
   //   {
   //      hamper.VvRowHgt[i]    = ZXC.QUN;
   //      hamper.VvSpcBefRow[i] = ZXC.Qun12;
   //   }
   //   hamper.VvBottomMargin = hamper.VvTopMargin;

   //   cb_DrillDown       = hamper.CreateVvCheckBox_OLD(0, 0, null, "Omogući DrillDown", RightToLeft.No);
   //   cb_zapamtiPostavke = hamper.CreateVvCheckBox_OLD(0, 1, null, "Zapamti postavke" , RightToLeft.No);

   //   VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, maxHampWidth, razmakIzmjedjuHampera);
   //   VvHamper.HamperStyling(hamper);
   //}

   #endregion hamperDrillDown

   #endregion hamper

   #region Fld_

   public string Fld_KDTicker
   {
      get { return tbx_KD_ticker.Text; }
      set {        tbx_KD_ticker.Text = value; }
   }

   public uint Fld_KDSifra
   {
      get { return ZXC.ValOrZero_UInt(tbx_KD_sifra.Text); }
      set {                           tbx_KD_sifra.Text = value.ToString("000000"); }

   }

   public string Fld_KDSifraAsTxtOd
   {
      get { return tbx_KD_sifra.Text; }
      set {        tbx_KD_sifra.Text = value; }
   }

   public string Fld_KDNaziv
   {
      get { return tbx_KD_naziv.Text; }
      set {        tbx_KD_naziv.Text = value; }
   }

   public string Fld_MjTrTicker
   {
      get { return tbx_mtrosTick.Text; }
      set {        tbx_mtrosTick.Text = value; }
   }

   public uint Fld_MjTrSifra
   {
      get { return ZXC.ValOrZero_UInt(tbx_mtrosID.Text); }
      set {                           tbx_mtrosID.Text = value.ToString("000000"); }
   }

   public string Fld_MjTrSifraAsTxtOd
   {
      get { return tbx_mtrosID.Text; }
      set {        tbx_mtrosID.Text = value; }
   }

   public string Fld_MjTrNaziv
   {
      get { return tbx_mtrosNaziv.Text; }
      set {        tbx_mtrosNaziv.Text = value; }
   }

   //public bool Fld_NeedsZapamtitPostavke
   //{
   //   get { return cb_zapamtiPostavke.Checked; }
   //   set {        cb_zapamtiPostavke.Checked = value; }
   //}

   public string Fld_AnalitikaSintetika
   {
      get
      {
         if     (rbt_analitika.Checked) return "A";
         else if(rbt_sintetika.Checked) return "S";

         else throw new Exception("Fld_AnalitikaSintetika: who df is checked?");
      }
   }

   public VvSQL.SorterType Fld_SifrarSort
   {
      get
      {
              if(rbt_SortSifra.Checked) return VvSQL.SorterType.Code;
         else if(rbt_SortNaziv.Checked) return VvSQL.SorterType.Name;
         else return VvSQL.SorterType.None;
      }
      set
      {
         switch(value)
         {
            case VvSQL.SorterType.Code: rbt_SortSifra.Checked = true; break;
            case VvSQL.SorterType.Name: rbt_SortNaziv.Checked = true; break;
         }
      }
   }

   public ZXC.AmortRazdoblje Fld_AmortRazdoblje
   {
      get
      {
              if(rbt_godina .Checked ) return ZXC.AmortRazdoblje.GODINA;
         else if(rbt_polugod.Checked ) return ZXC.AmortRazdoblje.POLUGOD;
         else if(rbt_kvartal.Checked ) return ZXC.AmortRazdoblje.KVARTAL;
         else if(rbt_mjesec .Checked ) return ZXC.AmortRazdoblje.MJESEC;

         else throw new Exception("Fld_AmortRazdoblje: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.AmortRazdoblje.GODINA : rbt_godina.Checked  = true; break;
            case ZXC.AmortRazdoblje.KVARTAL: rbt_kvartal.Checked = true; break;
            case ZXC.AmortRazdoblje.POLUGOD: rbt_polugod.Checked = true; break;
            case ZXC.AmortRazdoblje.MJESEC : rbt_mjesec.Checked  = true; break;
         }
      }
   }

   public ZXC.JeliJeTakav Fld_IsRashodovan
   {
      get
      {
              if(rbt_rashod.Checked)  return ZXC.JeliJeTakav.JE_TAKAV;
         else if(rbt_aktiv .Checked ) return ZXC.JeliJeTakav.NIJE_TAKAV;
         else if(rbt_sve   .Checked)  return ZXC.JeliJeTakav.NEBITNO;

         else throw new Exception("Fld_IsRashod: who df is checked?");
      }
   }

   public bool Fld_IsRashodSignif
   {
      get
      {
         if(rbt_sve.Checked) return false;
         else                return true;
      }
   }

   public bool? Fld_WeWantOnlyRashod
   {
      get
      {
              if(rbt_rashod.Checked) return true;
         else if(rbt_aktiv .Checked) return false;
         else if(rbt_sve   .Checked) return null;

         else throw new Exception("Fld_IsRashod: who df is checked?");
      }
   }

   #endregion Fld_

   #region PutFields(), GetFields()

   public override void GetFields(bool fuse)
   {
      GetFilterFields();
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheRptFilter = (VvRpt_Osred_Filter)_filter_data;

      if(TheRptFilter != null)
      {
         TheOsredFilterUC.PutFilterFields(TheRptFilter);

         Fld_KDTicker   = TheRptFilter.KupdobTk;
         Fld_KDSifra    = TheRptFilter.KupdobCd;
         Fld_KDNaziv    = TheRptFilter.KupdobNaziv;
         Fld_MjTrTicker = TheRptFilter.MjTrTk;
         Fld_MjTrSifra  = TheRptFilter.MjtrCd;
         Fld_MjTrNaziv  = TheRptFilter.MjtrNaziv;

         Fld_SifrarSort   = TheRptFilter.SorterType_Sifrar;

      }
      // Za JAM_... : 
      this.ValidateChildren();

   }

   public override void GetFilterFields()
   {
      TheOsredFilterUC.GetFilterFields();

      TheRptFilter.KupdobTk    = Fld_KDTicker;
      TheRptFilter.KupdobCd    = Fld_KDSifra;
      TheRptFilter.KupdobNaziv = Fld_KDNaziv;
      TheRptFilter.MjTrTk      = Fld_MjTrTicker;
      TheRptFilter.MjtrCd      = Fld_MjTrSifra;
      TheRptFilter.MjtrNaziv   = Fld_MjTrNaziv;

      TheRptFilter.SorterType_Sifrar   = Fld_SifrarSort;

      TheRptFilter.AnalitikaSintetika = Fld_AnalitikaSintetika;

      TheRptFilter.AmortRazdoblje = Fld_AmortRazdoblje;

      TheRptFilter.IsRashodovan       = Fld_IsRashodovan;
      TheRptFilter.IsRashodSignif     = Fld_IsRashodSignif;
      TheRptFilter.WeWantOnlyRashod   = Fld_WeWantOnlyRashod;
   }

   #endregion PutFields(), GetFields()

   #region Report/Filter

   public VvOsredReport TheVvOsrReport
   {
      get { return TheVvReport as VvOsredReport; } 
   }

   public VvRpt_Osred_Filter TheRptFilter
   {
      get;
      set;
   }

   #endregion Report/Filter

   #region Override

   public override VvRptFilter VirtualRptFilter
   {
      get { return this.TheRptFilter; }
   }

   public override void AddFilterMemberz()
   {
      TheOsredFilterUC.AddFilterMemberz(TheRptFilter, TheVvOsrReport);
   }

   public override void ResetRptFilterRbCbControls()
   {
      //cb_zapamtiPostavke.Checked = false;
      rbt_sve.Checked      = rbt_godina.Checked       = 
                             rbt_SortNaziv.Checked          =
                             rbt_analitika.Checked      = true;
   }

   #endregion Override
}

public class AmoFilterUC : VvFilterUC
{
   #region Fieldz

   public VvHamper     hamp_OdDo, hamp_tt, hamp_redosljedDokument;
   private VvTextBox   tbx_sifraOd, /*tbx_sifraDo,*/ tbx_nazivOd, tbx_nazivDo, tbx_BrDokOD, tbx_BrDokDO,
                       tbx_TTopis, tbxLookUp_TT, tbx_KontoOd, tbx_KontoDo;
   public VvTextBox    tbx_sifraDo; // za VvHamper.Set_ControlText_ThreadSafe(reportUC.TheOsredFilterUC.tbx_sifraDo, filter.OsredCDod); 
   public RadioButton  rbt_SortBroj, rbt_SortDatum;
   private CheckBox    cbx_isInvBrUmjSifre, cbx_isPrikazSaldaInv;

   #endregion Fieldz

   #region Constructor

   public AmoFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();

      TheVvUC = vvUC;

      InitializeHamper_OdDo(out hamp_OdDo);

      MaxHamperWidth = hamp_OdDo.Width;
      CreateHamper_4ButtonsResetGo_Width(MaxHamperWidth);
      
      nextY = hamp_OdDo.Bottom + razmakIzmjedjuHampera;
  
      InitializeHamper_tt(out hamp_tt);

      nextY = hamp_tt.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_RedosljedDokument(out hamp_redosljedDokument);

      nextY = hamp_redosljedDokument.Bottom + razmakIzmjedjuHampera;
      nextY = LocationOfHamper_HorLine(nextX, nextY, MaxHamperWidth) + razmakIzmjedjuHampera;

      this.Size = new Size(MaxHamperWidth + 2 * razmakIzmjedjuHampera, nextY + ZXC.QunMrgn);
      
      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion Constructor

   #region Hamper_s

   private void InitializeHamper_OdDo(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 6, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q5un, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbOd, lbDo, lbDatum, lbSifra, lbNaziv, lbKonto, lbTrans;

      lbOd = hamper.CreateVvLabel(1, 0, "OD:", ContentAlignment.MiddleCenter);
      lbDo = hamper.CreateVvLabel(2, 0, "DO:", ContentAlignment.MiddleCenter);

      lbDatum     = hamper.CreateVvLabel(0, 1, "Datum:", ContentAlignment.MiddleRight);
      tbx_DatumOD = hamper.CreateVvTextBox(1, 1, "tbx_datumOd", "Od datuma");
      tbx_DatumOD.JAM_IsForDateTimePicker = true;
      dtp_DatumOD      = hamper.CreateVvDateTimePicker(1, 1, "", tbx_DatumOD);
      dtp_DatumOD.Name = "dtp_datumOd";
      dtp_DatumOD.Tag  = tbx_DatumOD;
      tbx_DatumOD.Tag  = dtp_DatumOD;

      tbx_DatumDO = hamper.CreateVvTextBox(2, 1, "tbx_datumDo", "Do datuma");
      tbx_DatumDO.JAM_IsForDateTimePicker = true;
      dtp_DatumDO      = hamper.CreateVvDateTimePicker(2, 1, "", tbx_DatumDO);
      dtp_DatumDO.Name = "dtp_datumDo";
      dtp_DatumDO.Tag  = tbx_DatumDO;
      tbx_DatumDO.Tag  = dtp_DatumDO;

      tbx_DatumOD.JAM_WriteOnly = tbx_DatumDO.JAM_WriteOnly = true;

      lbSifra = hamper.CreateVvLabel(0, 2, "Sifra:", ContentAlignment.MiddleRight);
      lbNaziv = hamper.CreateVvLabel(0, 3, "Naziv:", ContentAlignment.MiddleRight);
      lbKonto = hamper.CreateVvLabel(0, 4, "Konto:", ContentAlignment.MiddleRight);
    
      tbx_sifraOd = hamper.CreateVvTextBox(1, 2, "tbx_sifraOd", "Od sifre");
      tbx_sifraDo = hamper.CreateVvTextBox(2, 2, "tbx_sifraDo", "Do sifre");

      tbx_nazivOd = hamper.CreateVvTextBox(1, 3, "tbx_nazivOd", "Od naziva");
      tbx_nazivDo = hamper.CreateVvTextBox(2, 3, "tbx_nazivDo", "Do naziva");
     
      tbx_KontoOd = hamper.CreateVvTextBox(1, 4, "tbx_kontoOd", "Od konta", 8);
      tbx_KontoDo = hamper.CreateVvTextBox(2, 4, "tbx_kontoDo", "Do konta", 8);

      if(TheVvUC is OsredUC)
      {
         tbx_sifraOd.JAM_ReadOnly = tbx_sifraDo.JAM_ReadOnly = true;
         tbx_nazivOd.JAM_ReadOnly = tbx_nazivDo.JAM_ReadOnly = true;
         tbx_KontoOd.JAM_ReadOnly = tbx_KontoDo.JAM_ReadOnly = true;
      }
      else
      {
         tbx_sifraOd.JAM_SetAutoCompleteData(Osred.recordName, Osred.sorterCD.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Osred_sorterSifra), null);
         tbx_sifraDo.JAM_SetAutoCompleteData(Osred.recordName, Osred.sorterCD.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Osred_sorterSifra), null);

         tbx_nazivOd.JAM_SetAutoCompleteData(Osred.recordName, Osred.sorterNaziv.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Osred_sorterName), null);
         tbx_nazivDo.JAM_SetAutoCompleteData(Osred.recordName, Osred.sorterNaziv.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Osred_sorterName), null);

         tbx_KontoOd.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
         tbx_KontoDo.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);

         tbx_sifraOd.JAM_WriteOnly = tbx_sifraDo.JAM_WriteOnly = true;
         tbx_nazivOd.JAM_WriteOnly = tbx_nazivDo.JAM_WriteOnly = true;
         tbx_KontoOd.JAM_WriteOnly = tbx_KontoDo.JAM_WriteOnly = true;
         
         tbx_sifraOd.JAM_MustTabOutBeforeSubmit = tbx_sifraDo.JAM_MustTabOutBeforeSubmit = true;
         tbx_nazivOd.JAM_MustTabOutBeforeSubmit = tbx_nazivDo.JAM_MustTabOutBeforeSubmit = true;
         tbx_KontoOd.JAM_MustTabOutBeforeSubmit = tbx_KontoDo.JAM_MustTabOutBeforeSubmit = true;
     } 

      tbx_sifraOd.TextAlign = tbx_sifraDo.TextAlign = HorizontalAlignment.Left;

      tbx_DatumOD.ContextMenu = dtp_DatumOD.ContextMenu =
                                tbx_DatumDO.ContextMenu =
                                dtp_DatumDO.ContextMenu = CreateNewContexMenu_Date();

      lbTrans = hamper.CreateVvLabel(0, 5, "Br.Dok:", ContentAlignment.MiddleRight);
      
      tbx_BrDokOD                   = hamper.CreateVvTextBox(1, 5, "tbx_BrTransOD", "Od broja dokumenta", 6);
      tbx_BrDokOD.JAM_CharEdits     = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_BrDokOD.JAM_FillCharacter = '0';
      tbx_BrDokOD.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      tbx_BrDokDO                   = hamper.CreateVvTextBox(2, 5, "tbx_BrTransDO", "Do broja dokumenta", 6);
      tbx_BrDokDO.JAM_CharEdits     = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_BrDokDO.JAM_FillCharacter = '0';
      tbx_BrDokDO.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      if(TheVvUC is AmortDUC) tbx_BrDokOD.JAM_ReadOnly  = tbx_BrDokDO.JAM_ReadOnly = true;
      else                    tbx_BrDokOD.JAM_WriteOnly = tbx_BrDokDO.JAM_WriteOnly = true;

      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_tt(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 3, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] {ZXC.Q3un, ZXC.Q2un + ZXC.Qun2, ZXC.Q7un + ZXC.Qun2 };
      hamper.VvSpcBefCol   = new int[] {ZXC.Qun8, ZXC.Qun8           , ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbVrKnjz;

      lbVrKnjz     = hamper.CreateVvLabel        (0, 0, "TT:", ContentAlignment.MiddleRight);
      tbx_TTopis   = hamper.CreateVvTextBox      (2, 0, "tbx_TtOpis", "Za tip transakcije");
      tbxLookUp_TT = hamper.CreateVvTextBoxLookUp(1, 0, "tbxLookUp_TT", "");

      tbxLookUp_TT.JAM_CharacterCasing = CharacterCasing.Upper;
      tbxLookUp_TT.JAM_WriteOnly = true;
      tbxLookUp_TT.JAM_Set_LookUpTable(ZXC.luiListaAmortTT, (int)ZXC.Kolona.prva);
      tbxLookUp_TT.JAM_lui_NameTaker_JAM_Name = tbx_TTopis.JAM_Name;

      tbx_TTopis.JAM_ReadOnly = true;
      tbx_TTopis.Tag          = ZXC.vvColors.userControl_BackColor;

      cbx_isInvBrUmjSifre = hamper.CreateVvCheckBox_OLD(1, 1, null, 1, 0, "Tekst InvBr umjesto Sifre", RightToLeft.No);
      cbx_isPrikazSaldaInv     = hamper.CreateVvCheckBox_OLD(1, 2, null, 1, 0, "Prikaz stanja na Inventuri", RightToLeft.No);

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_RedosljedDokument(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q6un + ZXC.Qun4 + ZXC.Qun5 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 + ZXC.Qun5 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "Slijed ispisa Dokumenta:", ContentAlignment.MiddleLeft);
      
      rbt_SortDatum = hamper.CreateVvRadioButton(0, 1, null, "Po datumu", TextImageRelation.ImageBeforeText);
      rbt_SortDatum.Checked = true;
      rbt_SortBroj = hamper.CreateVvRadioButton(0, 2, null, "Po broju", TextImageRelation.ImageBeforeText);

      VvHamper.HamperStyling(hamper);
   }


   #endregion Hamper_s

   #region Fld_

   public DateTime Fld_DatumOd
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumOD.Value);
      }
      set
      {
         dtp_DatumOD.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DatumOD.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumOD);
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
         tbx_DatumDO.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumDO);
      }
   }

   public string Fld_OSredCdOd
   {
      get { return tbx_sifraOd.Text; }
      set {        tbx_sifraOd.Text = value; }
   }

   public string Fld_OSredCdDo
   {
      get { return tbx_sifraDo.Text; }
      set {        tbx_sifraDo.Text = value; }
   }

   public string Fld_NazivOd
   {
      get { return tbx_nazivOd.Text; }
      set {        tbx_nazivOd.Text = value; }
   }

   public string Fld_NazivDo
   {
      get { return tbx_nazivDo.Text; }
      set {        tbx_nazivDo.Text = value; }
   }

   public string Fld_KontoOd
   {
      get { return tbx_KontoOd.Text; }
      set {        tbx_KontoOd.Text = value; }
   }

   public string Fld_KontoDo
   {
      get { return tbx_KontoDo.Text; }
      set {        tbx_KontoDo.Text = value; }
   }

   public uint Fld_BrDokOd
   {
      get { return ZXC.ValOrZero_UInt(tbx_BrDokOD.Text); }
      set {                           tbx_BrDokOD.Text = value.ToString("000000"); }
   }

   public uint Fld_BrDokDo
   {
      get { return ZXC.ValOrZero_UInt(tbx_BrDokDO.Text); }
      set {                           tbx_BrDokDO.Text = value.ToString("000000"); }
   }

   public string Fld_TipTrans
   {
      get { return tbxLookUp_TT.Text; }
      set {        tbxLookUp_TT.Text = value; }
   }

   public bool Fld_NeedsHorizontalLine
   {
      get { return cb_Line.Checked; }
      set {        cb_Line.Checked = value; }
   }

   public VvSQL.SorterType Fld_DokumentSort
   {
      get
      {
              if(rbt_SortDatum.Checked) return VvSQL.SorterType.DokDate;
         else if(rbt_SortBroj.Checked)  return VvSQL.SorterType.DokNum;
         else return VvSQL.SorterType.None;
      }
      set
      {
         switch(value)
         {
            case VvSQL.SorterType.DokDate: rbt_SortDatum.Checked = true; break;
            case VvSQL.SorterType.DokNum : rbt_SortBroj.Checked  = true; break;
         }
      }
   }

   public bool Fld_IsInvBrUmjSifre  { get { return cbx_isInvBrUmjSifre .Checked; } set { cbx_isInvBrUmjSifre .Checked = value; } }
   public bool Fld_IsPrikazSaldaInv { get { return cbx_isPrikazSaldaInv.Checked; } set { cbx_isPrikazSaldaInv.Checked = value; } }

   #endregion Fld_

   #region PutFilterFields(), GetFilterFields()

   private VvRpt_Osred_Filter TheAtransFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as VvRpt_Osred_Filter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheAtransFilter = (VvRpt_Osred_Filter)_filter_data;

      if(TheAtransFilter != null)
      {
         Fld_OSredCdOd        = TheAtransFilter.OsredCDod;
         Fld_OSredCdDo        = TheAtransFilter.OsredCDdo;    
         Fld_NazivOd          = TheAtransFilter.NazivOd;      
         Fld_NazivDo          = TheAtransFilter.NazivDo;      
         Fld_BrDokOd          = TheAtransFilter.DokNumOd; 
         Fld_BrDokDo          = TheAtransFilter.DokNumDo;
         Fld_TipTrans         = TheAtransFilter.TipTran;
         Fld_DatumOd          = TheAtransFilter.DatumOd;
         Fld_DatumDo          = TheAtransFilter.DatumDo;
         Fld_KontoOd          = TheAtransFilter.KontoOd;
         Fld_KontoDo          = TheAtransFilter.KontoDo;
         Fld_IsInvBrUmjSifre  = TheAtransFilter.IsInvBrUmjSifre;
         Fld_IsPrikazSaldaInv = TheAtransFilter.IsPrikazSaldaInv;

         Fld_DokumentSort = TheAtransFilter.SorterType_Dokument;
       
         Fld_NeedsHorizontalLine = TheAtransFilter.NeedsHorizontalLine;
      }
      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
         TheAtransFilter.OsredCDod        = Fld_OSredCdOd ;
         TheAtransFilter.OsredCDdo        = Fld_OSredCdDo ;    
         TheAtransFilter.NazivOd          = Fld_NazivOd   ;      
         TheAtransFilter.NazivDo          = Fld_NazivDo   ;      
         TheAtransFilter.DokNumOd         = Fld_BrDokOd   ; 
         TheAtransFilter.DokNumDo         = Fld_BrDokDo   ;
         TheAtransFilter.TipTran          = Fld_TipTrans  ;
         TheAtransFilter.DatumOd          = Fld_DatumOd   ;
         TheAtransFilter.DatumDo          = Fld_DatumDo   ;
         TheAtransFilter.KontoOd          = Fld_KontoOd   ;
         TheAtransFilter.KontoDo          = Fld_KontoDo   ;
         TheAtransFilter.IsInvBrUmjSifre  = Fld_IsInvBrUmjSifre;
         TheAtransFilter.IsPrikazSaldaInv =Fld_IsPrikazSaldaInv ;
        
         TheAtransFilter.SorterType_Dokument = Fld_DokumentSort;
 
         TheAtransFilter.NeedsHorizontalLine = Fld_NeedsHorizontalLine;
   }

   #endregion PutFilterFields(), GetFilterFields()

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
      VvRpt_Osred_Filter theRptFilter   = (VvRpt_Osred_Filter)_vvRptFilter;
      VvOsredReport      theVvOsrReport = (VvOsredReport)     _vvReport;

      
      DateTime dateOD, dateDO;
      string   comparer;
      //string   relatedTableName = "";
      string   text, textOD, textDO;
      bool     isDummy, isOsredFilterStyle;
      uint     num, numOD, numDO;
      DataRow  drSchema;

      //AmoReportUC amoReportUC = (AmoReportUC)TheVvUC;

      theRptFilter.FilterMembers.Clear();
      theRptFilter.ClearAllFilters_FromClauseGotTableName();

      if(theVvOsrReport != null)
      {
         isOsredFilterStyle = theVvOsrReport.FilterStyle == ZXC.AIZ_FilterStyle.Osred;
      }
      else
      {
         isOsredFilterStyle = false;
      }


      // Fld_DatumOdDo                                                                                                                                  

      if(theVvOsrReport != null && theVvOsrReport.FilterStyle == ZXC.AIZ_FilterStyle.Amort && !theRptFilter.NeedsDrillDown)
      {
         drSchema = ZXC.AmortSchemaRows[ZXC.AmoCI.dokDate];
      }
      else
      {
         drSchema = ZXC.AtransSchemaRows[ZXC.AtrCI.t_dokDate];
      }

      dateOD = theRptFilter.DatumOd;
      dateDO = theRptFilter.DatumDo;

      if(dateOD == dateDO) comparer = " = ";
      else                 comparer = " >= ";


      if(isOsredFilterStyle) isDummy = true; // overridnig isDummy rule for Osred FilterStyle 
      else                   isDummy = false;

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "DateOD", dateOD, dateOD.ToString("dd.MM.yyyy."), "Za razdoblje:", comparer, ""));

      isDummy = false;

      if(isOsredFilterStyle) isDummy = true; // overridnig isDummy rule for Osred FilterStyle 

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "DateDO", dateDO, dateDO.ToString("dd.MM.yyyy."), "-", " <= ", ""));

    
      // pocetakAmRazdoblja za RptO_ListaPoKontu

      if(theVvOsrReport is RptO_ListaPoKontu)
      {
         DateTime dateStartAm = OsredStatus.GetDateAmRazdobljeStart(ZXC.AmortRazdoblje.GODINA, dateDO);
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "pocetakAmRazdoblja", dateStartAm, dateStartAm.ToShortDateString(), "Pocetak am. razdoblja:", "", ""));
      }


      // Fld_OsredCDOdDo                                                                                                                                     

      if(isOsredFilterStyle)
      {
         drSchema = ZXC.OsredSchemaRows[ZXC.OsrCI.osredCD];
      }
      else
      {
         drSchema = ZXC.AtransSchemaRows[ZXC.AtrCI.t_osredCD];
      }

      textOD   = theRptFilter.OsredCDod;
      textDO   = theRptFilter.OsredCDdo;

      if(textOD.NotEmpty())
      {
         if(textOD == textDO) comparer = " = ";
         else                 comparer = " >= ";

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "OsredCDOD", textOD, textOD, "Od šifre:", comparer, ""));
      }
      else if(textDO.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "OsredCDOD", textOD, "-", "Od šifre:", " >= ", ""));
      }

      if(textDO.NotEmpty())
      {
         if(textDO == textOD) isDummy = true;
         else                 isDummy = false;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "OsredCDDO", textDO, textDO, "Do šifre:", " <= ", ""));
      }
      else if(textOD.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "OsredCDDO", textDO, "-", "Do šifre:", " <= ", ""));
      }

      // Fld_OsredNazivOdDo                                                                                                                                     

      drSchema = ZXC.OsredSchemaRows[ZXC.OsrCI.naziv];

      textOD = theRptFilter.NazivOd;
      textDO = theRptFilter.NazivDo;

      if(textOD.NotEmpty())
      {
         if(textOD == textDO) comparer = " = ";
         else                 comparer = " >= ";

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "OsredNazivOD", textOD, textOD, "Od naziva:", comparer, Osred.recordName));
      }
      else if(textDO.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "OsredNazivOD", textOD, "-", "Od naziva:", " >= ", Osred.recordName));
      }

      if(textDO.NotEmpty())
      {
         if(textDO == textOD) isDummy = true;
         else                 isDummy = false;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "OsredNazivDO", textDO, textDO, "Do naziva:", " <= ", Osred.recordName));
      }
      else if(textOD.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "OsredNazivDO", textDO, "-", "Do naziva:", " <= ", Osred.recordName));
      }

      // Fld_BrDokOdDo                                                                                                                                   

      if(theVvOsrReport != null && theVvOsrReport.FilterStyle == ZXC.AIZ_FilterStyle.Amort && !theRptFilter.NeedsDrillDown)
      {
         drSchema = ZXC.AmortSchemaRows[ZXC.AmoCI.dokNum];
      }
      else
      {
         drSchema = ZXC.AtransSchemaRows[ZXC.AtrCI.t_dokNum];
      }

      numOD = theRptFilter.DokNumOd;
      numDO = theRptFilter.DokNumDo;

      if(numOD != 0)
      {
         if(numOD == numDO) comparer = " = ";
         else               comparer = " >= ";

         if(isOsredFilterStyle) isDummy = true; // overridnig isDummy rule for Kplan FilterStyle 
         else                   isDummy = false;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "BrDokOD", numOD, numOD.ToString("000000"), "Od dokumenta broj:", comparer, ""));
      }
      else if(numDO != 0)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "BrDokOD", numOD, "-", "Od dokumenta broj:", " >= ", ""));
      }

      if(numDO != 0)
      {
         if(numDO == numOD) isDummy = true;
         else               isDummy = false;

         if(isOsredFilterStyle) isDummy = true; // overridnig isDummy rule for Kplan FilterStyle 
         //else                   isDummy = false;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "BrDokDO", numDO, numDO.ToString("000000"), "Do dokumenta broj:", " <= ", ""));
      }
      else if(numOD != 0)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "BrDokDO", numDO, "-", "Do dokumenta broj:", " <= ", ""));
      }

      // Fld_TT                                                                                                                                     

      drSchema = ZXC.AtransSchemaRows[ZXC.AtrCI.t_tt];
      text     = theRptFilter.TipTran;

      if(isOsredFilterStyle) isDummy = true; // overridnig isDummy rule for Kplan FilterStyle 
      else                   isDummy = false;

      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "TipTran", text, text, "Za tip transakcije:", " = ", ""));
      }

      // Fld_OsredKontoOdDo                                                                                                                                     

      drSchema = ZXC.OsredSchemaRows[ZXC.OsrCI.konto];

      textOD = theRptFilter.KontoOd;
      textDO = theRptFilter.KontoDo;

      if(textOD.NotEmpty())
      {
         if(textOD == textDO) comparer = " = ";
         else                 comparer = " >= ";

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "OsredKontoOD", textOD, textOD, "Od konta:", comparer, Osred.recordName));
      }
      else if(textDO.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "OsredKontoOD", textOD, "-", "Od konta:", " >= ", Osred.recordName));
      }

      if(textDO.NotEmpty())
      {
         if(textDO == textOD) isDummy = true;
         else                 isDummy = false;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "OsredKontoDO", textDO, textDO, "Do konta:", " <= ", Osred.recordName));
      }
      else if(textOD.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "OsredKontoDO", textDO, "-", "Do konta:", " <= ", Osred.recordName));
      }

      if(TheVvUC is AmoReportUC)
      {
         AmoReportUC amoReportUC = (AmoReportUC)TheVvUC;

         // Fld_KupdobCD                                                                                                                                     

         drSchema = ZXC.OsredSchemaRows[ZXC.OsrCI.kupdob_cd];
         num = theRptFilter.KupdobCd;

         if(num.NotZero())
         {
            string kcd_string = amoReportUC.tbx_KD_sifra.Text + " " + amoReportUC.tbx_KD_ticker.Text + " " + amoReportUC.tbx_KD_naziv.Text;

            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "KupdobNaOsredot", num,/* num.ToString("000000")*/kcd_string, "Za dobavljaca:", " = ", Osred.recordName));
         }

         // Fld_MtrosCD                                                                                                                                     

         drSchema = ZXC.OsredSchemaRows[ZXC.OsrCI.mtros_cd];
         num = theRptFilter.MjtrCd;

         if(num.NotZero())
         {
            //string kcd_string = amoReportUC.tbx_mtrosID.Text + " " + amoReportUC.tbx_mtrosTick.Text + " " + amoReportUC.tbx_mtrosNaziv.Text;
            string kcd_string = amoReportUC.tbx_mtrosNaziv.Text + " " + amoReportUC.tbx_mtrosTick.Text;

            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "MtrosNaOsredot", num, kcd_string, "Za mj. troska:", " = ", Osred.recordName));
         }

         //                                                                                                                                            
      }
   }


   #endregion AddFilterMemberz()

}

public class AmortDokumentFilterUC : VvFilterUC
{

   #region  Constructor

   public AmortDokumentFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;

      CreateHamper_4ButtonsResetGo_Width(0); // 0 zato sto je ovdje najsiri hamper bas taj s buttonsima
      MaxHamperWidth = hamper4buttons.Width;
      
      nextY = hamper4buttons.Bottom + razmakIzmjedjuHampera;
      nextY = LocationOfHamper_HorLine(nextX, nextY, MaxHamperWidth) + razmakIzmjedjuHampera;

      this.Size = new Size(MaxHamperWidth + 2 * razmakIzmjedjuHampera, nextY + ZXC.QunMrgn);

      this.ResumeLayout();

   }

   #endregion  Constructor

   #region Fld_

   public bool Fld_NeedsHorizontalLine
   {
      get { return cb_Line.Checked; }
      set {        cb_Line.Checked = value; }
   }

   #endregion Fld_

   #region Put & GetFilterFields

   private AmortDokumentFilter TheAmortDokumentFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as AmortDokumentFilter; }
      set { this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheAmortDokumentFilter = (AmortDokumentFilter)_filter_data;

      if(TheAmortDokumentFilter != null)
      {
         Fld_NeedsHorizontalLine = TheAmortDokumentFilter.NeedsHorizontalLine;
      }

      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
      TheAmortDokumentFilter.NeedsHorizontalLine = Fld_NeedsHorizontalLine;
   }

   #endregion Put & GetFilterFields

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
      VvRpt_Osred_Filter theRptFilter   = (VvRpt_Osred_Filter)_vvRptFilter;
      VvOsredReport      theVvOsrReport = (VvOsredReport)     _vvReport;

      uint    dokNum;
      DataRow drSchema;

      theRptFilter.FilterMembers.Clear();
      theRptFilter.ClearAllFilters_FromClauseGotTableName();

      // Fld_BrNalogaOdDo                                                                                                                                   

      drSchema = ZXC.AtransSchemaRows[ZXC.AtrCI.t_dokNum];

      dokNum = theRptFilter.DokNumOd;

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TheDokNum", dokNum, dokNum.ToString("000000"), "Nalog:", " = ", ""));

   }

   #endregion AddFilterMemberz()

}