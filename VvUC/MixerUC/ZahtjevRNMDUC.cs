using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public partial class ZahtjevRNMDUC : MixerDUC
{
   #region Fieldz
   
   public VvTextBox tbx_kupDobCd, tbx_kupDobTk, tbx_kupDobName, 
                    tbx_dateTrazeniRok,
                    tbx_primjedba, tbx_standard,
                    tbx_vrstaRNM, tbx_vrstaRNMopis, tbx_izlaznoSkl, tbx_izlaznoSklOpis, tbx_ulaznoSkl, tbx_ulaznoSklOpis, tbx_newUg;

   public VvHamper  hamp_nzi, hamp_opis, hamp_partner, hamp_sklVeze, hamp_link, hamp_rok;
   
   private VvDateTimePicker dtp_dateTrazeniRok;

   #endregion Fieldz

   #region Constructor

   public ZahtjevRNMDUC(Control parent, Mixer _mixer, VvSubModul vvSubModul): base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_NZI
         });

   }

   
   #endregion Constructor

   #region CreateSpecificHampers()
      
   protected override void CreateSpecificHampers()
   {

      InitializeHamper_partner(out hamp_partner);
      nextY = hamp_partner.Bottom;

      hamp_tt.Location = new Point(hamp_partner.Right, hamp_partner.Top);

      InitializeHamper_nzi(out hamp_nzi);
      hamp_dokDate.Location = new Point(hamp_partner.Right - hamp_dokDate.Width, hamp_nzi.Top);

      nextX = hamp_tt.Left;
      nextY = hamp_tt.Bottom;
      InitializeHamper_sklVeze(out hamp_sklVeze);

      InitializeHamper_rok(out hamp_rok);
      hamp_rok.Location = new Point(hamp_partner.Right - hamp_dokDate.Width, hamp_dokDate.Bottom);

      hamp_dokNum.Location = new Point(hamp_dokDate.Right - hamp_dokNum.Width, hamp_rok.Bottom);

      nextX = hamp_nzi.Left;
      nextY = hamp_nzi.Bottom;

      InitializeHamper_link(out hamp_link);

      hamp_napomena.Location = new Point(nextX, hamp_link.Bottom);
      nextY = hamp_napomena.Bottom + ZXC.Qun8;

      InitializeHamper_opis(out hamp_opis);
      nextY = hamp_opis.Bottom;


   }
  
   public void InitializeHamper_partner(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false, nextX, ZXC.QunMrgn, razmakHamp);
      //                                     0        1          2        3    
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un, ZXC.Q3un, ZXC.Q10un + ZXC.Q7un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
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
      tbx_kupDobName = hamper.CreateVvTextBox(3, 0, "tbx_kupDobName", "Naziv Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobName));
      
      tbx_kupDobCd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_kupDobTk.JAM_CharacterCasing = CharacterCasing.Upper;
 
      this.ControlForInitialFocus = tbx_kupDobName;

      tbx_kupDobCd  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobTk  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyKupdobTextBoxLeave));


   }

   public void InitializeHamper_nzi(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0        1          2    
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q2un, ZXC.Q8un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
                     hamper.CreateVvLabel  (0, 0, "Standard:", ContentAlignment.MiddleRight);
      tbx_standard = hamper.CreateVvTextBox(1, 0, "tbx_standard", "Standard", GetDB_ColumnSize(DB_ci.strF_64), 1, 0);

                         hamper.CreateVvLabel        (0, 1, "Vrsta RNM:", ContentAlignment.MiddleRight);
      tbx_vrstaRNM     = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_vezniDok", "Vrsta RNM", GetDB_ColumnSize(DB_ci.strD_32));
      tbx_vrstaRNMopis = hamper.CreateVvTextBox      (2, 1, "tbx_vrstaRNMopis", "Vrsta RNM opis");
      tbx_vrstaRNM.JAM_Set_LookUpTable(ZXC.luiListaVrstaRNM, (int)ZXC.Kolona.prva);
      tbx_vrstaRNM.JAM_lui_NameTaker_JAM_Name = tbx_vrstaRNMopis.JAM_Name;
      tbx_vrstaRNMopis.JAM_ReadOnly = true;
      tbx_vrstaRNM.JAM_DataRequired = true;
   }

   public void InitializeHamper_sklVeze(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 5, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0          1                 2                          3                       4        
      hamper.VvColWdt      = new int[] {  ZXC.Q4un,  ZXC.Q3un- ZXC.Qun2, ZXC.Q6un , ZXC.Q2un + ZXC.QUN - ZXC.Qun4, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] {  ZXC.Qun4,  ZXC.Qun4,            ZXC.Qun4,                      ZXC.Qun4,                  0 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                           hamper.CreateVvLabel        (0, 0, "IzlaznoSkl:"       , ContentAlignment.MiddleRight);
      tbx_izlaznoSkl     = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_izlaznoSkl"    ,  "Izlazno skladiste");
      tbx_izlaznoSklOpis = hamper.CreateVvTextBox      (2, 0, "tbx_izlaznoSklOpis",  "Naziv izlaznog skladista", 64, 1, 0);
      tbx_izlaznoSkl.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_izlaznoSkl.JAM_lui_NameTaker_JAM_Name = tbx_izlaznoSklOpis.JAM_Name;
      tbx_izlaznoSklOpis.JAM_ReadOnly = true;
      tbx_izlaznoSkl.JAM_DataRequired = true;

                           hamper.CreateVvLabel       (0, 1, "UlaznoSkl:"       , ContentAlignment.MiddleRight);
      tbx_ulaznoSkl     = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_ulaznoSkl"    ,  "Ulazno skladiste");
      tbx_ulaznoSklOpis = hamper.CreateVvTextBox      (2, 1, "tbx_ulaznoSklOpis", "Naziv ulaznog skladista", 64, 1, 0);
      tbx_ulaznoSkl.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_ulaznoSkl.JAM_lui_NameTaker_JAM_Name = tbx_ulaznoSklOpis.JAM_Name;
      tbx_ulaznoSklOpis.JAM_ReadOnly = true;
      tbx_ulaznoSkl.JAM_DataRequired = true;

     
                            hamper.CreateVvLabel  (0, 2, "Veza1:", ContentAlignment.MiddleRight);
      tbx_v1_tt     = hamper.CreateVvTextBoxLookUp(1, 2, "tbx_v1_tt"    , "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v1_tt));
      tbx_v1_ttOpis = hamper.CreateVvTextBox      (2, 2, "tbx_v1_ttOpis", "Veza na interni dokument", 32);
      tbx_v1_ttNum  = hamper.CreateVvTextBox      (3, 2, "tbx_v1_ttNum" , "Veza na interni dokument", 6/*GetDB_ColumnSize(DB_ci.iz_ttNum)*/);

      btn_v1TT = hamper.CreateVvButton(4, 2, new EventHandler(GoTo_MIXER_Dokument_Click), "");

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

                      hamper.CreateVvLabel        (0, 3, "Veza2:", ContentAlignment.MiddleRight);
      tbx_v2_tt     = hamper.CreateVvTextBoxLookUp(1, 3, "tbx_v2_tt"    , "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v2_tt));
      tbx_v2_ttOpis = hamper.CreateVvTextBox      (2, 3, "tbx_v2_ttOpis", "Veza na interni dokument");
      tbx_v2_ttNum  = hamper.CreateVvTextBox      (3, 3, "tbx_v2_ttNum" , "Veza na interni dokument" + " broj", 6/*GetDB_ColumnSize(DB_ci.na_ttNum)*/);

      btn_v2TT = hamper.CreateVvButton(4, 3, new EventHandler(GoTo_MIXER_Dokument_Click), "");
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

   public void InitializeHamper_rok(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0        1    
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q4un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
                           hamper.CreateVvLabel  (0, 0, "Rok izvršenja:", ContentAlignment.MiddleRight);
      tbx_dateTrazeniRok = hamper.CreateVvTextBox(1, 0, "tbx_dateTrazeniRok", "Traženi rok izvršenja", GetDB_ColumnSize(DB_ci.dateA));
      tbx_dateTrazeniRok.JAM_IsForDateTimePicker = true;
      dtp_dateTrazeniRok = hamper.CreateVvDateTimePicker(1, 0, "", tbx_dateTrazeniRok);
      dtp_dateTrazeniRok.Name = "dtp_dateTrazeniRok";
      
   }

   public void InitializeHamper_link(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0        1                2                3         
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q10un + ZXC.Qun4,  ZXC.QUN - ZXC.Qun4, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4,                   0,                  0 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
                                           
                       hamper.CreateVvLabel   (0, 0, "Link:", ContentAlignment.MiddleRight);
      tbx_externLink1 = hamper.CreateVvTextBox(1, 0, "tbx_externLink1", "Izvještaj sa puta - Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.strE_256));

      btn_goExLink1           = hamper.CreateVvButton(2, 0, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink1.Name      = "btn_goExLink1";
      btn_goExLink1.FlatStyle = FlatStyle.Flat;
      btn_goExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink1.Image     = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink1.Tag       = 1;
      btn_goExLink1.TabStop   = false;
      btn_goExLink1.Visible   = false;
      
      btn_openExLink1           = hamper.CreateVvButton(3, 0, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLink1.Name      = "btn_openExLink1";
      btn_openExLink1.FlatStyle = FlatStyle.Flat;
      btn_openExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLink1.Image   = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLink1.Tag     = 1;
      btn_openExLink1.TabStop = false;

   }

   public void InitializeHamper_opis(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0        1    
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q10un * 3 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
                                           
                      hamper.CreateVvLabel  (0, 0, "Opis:", ContentAlignment.MiddleRight);
      tbx_primjedba = hamper.CreateVvTextBox(1, 0, "tbx_externLink1", "Opis", GetDB_ColumnSize(DB_ci.strE_256));
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
      T_artiklCD_CreateColumn      (ZXC.Q4un   , "Šifra Proizvoda" , "Šifra proizvoda");
      T_artiklName_CreateColumn    (       0   , "Naziv Proizvoda" , "Opis/Naziv proizvoda ...");
    //T_kpdbMjestoA_32_CreateColumn(ZXC.Q2un   , "JM"              , "Jedinica mjere");
      T_kol_CreateColumn           (ZXC.Q3un   , "KolProiz"        , "Količina proizvoda",  2);
      T_vezniDokA_64_CreateColumn  (ZXC.Q4un   , "Šifra Materijala", "", null);
      T_opis_128_CreateColumn      (ZXC.Q10un  , "Naziv Materijala", "Opis naziv materijala", 128, null);
      T_kpdbZiroA_32_CreateColumn  (ZXC.Q4un   , "RGC"             , "Broj registra cijevi", ZXC.luiListaSerlot);
      T_moneyA_CreateColumn        (ZXC.Q3un   , "KolMat"          , "Kolicina materijala", 2);
      T_kpdbZiroB_32_CreateColumn  (ZXC.Q5un   , "RNM"             , "Broj Radnog naloga RNM", null);

      vvtbT_artiklCD  .JAM_BackColor = Color.PaleGreen;
      vvtbT_artiklName.JAM_BackColor = Color.PaleGreen;
      vvtbT_kol       .JAM_BackColor = Color.PaleGreen;

    //vvtbT_kpdbZiroB_32.JAM_ReadOnly = true;

      vvtbT_vezniDokA_64.JAM_SetAutoCompleteData(Artikl.recordName, Artikl.sorterCD.SortType  , new EventHandler(OnVvTBEnter_SetAutocmplt_Artikl_sorterSifra), new EventHandler(AnyArtiklTextBox_OnGrid_Leave_2));
      vvtbT_opis_128    .JAM_SetAutoCompleteData(Artikl.recordName, Artikl.sorterName.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Artikl_sorterName) , new EventHandler(AnyArtiklTextBox_OnGrid_Leave_2));

      vvtbT_kpdbZiroA_32.JAM_lookUp_NOTobligatory = true;
      vvtbT_kpdbZiroA_32.JAM_FieldEntryMethod += new System.EventHandler(OnSerlotEnter_FillDataSource);

   }

   public void AnyArtiklTextBox_OnGrid_Leave_2(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvTextBoxEditingControl vvtb_editingControl = sender as VvTextBoxEditingControl;

      if(vvtb_editingControl == null) return;

      if(vvtb_editingControl.Text == this.originalText) return;

      VvDataGridView theGrid = ((VvDataGridView)vvtb_editingControl.EditingControlDataGridView);

      this.originalText = vvtb_editingControl.Text;
      Artikl artikl_rec = ArtiklSifrar.Find(FoundInSifrar<Artikl>);

      int currRow = vvtb_editingControl.EditingControlRowIndex;

      if(artikl_rec != null)
      {
         theGrid.PutCell(ci.iT_vezniDokA_64, currRow, artikl_rec.ArtiklCD);
         theGrid.PutCell(ci.iT_opis_128    , currRow, artikl_rec.ArtiklName);
      }

      // samo za DUC-eve
      ZXC.TheVvForm.SetDirtyFlag(sender);
   }

   public void OnSerlotEnter_FillDataSource(object sender, EventArgs e)
   {
      // /* temp. For deploy: */ return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;
      
      if(ZXC.RRD.Dsc_IsSerlotVisible == false) return;
      
      DataGridView dgv = null;
      
      if(sender is VvTextBoxEditingControl)
      {
         VvTextBoxEditingControl vtbec = sender as VvTextBoxEditingControl;
         dgv = vtbec.EditingControlDataGridView;
      
       //ZXC.aim_emsg("row: <{0}> col: <{1}> row: <{2}> col: <{3}>",
       //   TheG.CurrentCellAddress.Y + 1, TheG.CurrentCellAddress.X + 1,
       //    dgv.CurrentCellAddress.Y + 1,  dgv.CurrentCellAddress.X + 1);
      }
      else return;
      
      int rIdx = dgv.CurrentRow.Index;
      
      string artiklCD = TheG.GetStringCell(ci.iT_vezniDokA_64, rIdx, true);

      // clear 
      ZXC.luiListaSerlot.Clear(); vvtbT_kpdbZiroA_32.JAM_Set_LookUpTable(ZXC.luiListaSerlot, (int)ZXC.Kolona.prva);

      if(artiklCD.IsEmpty() || Fld_IzlaznoSkl.IsEmpty() /*IZLAZNO SKLADISTE*/) return;
      
      List<ZXC.VvUtilDataPackage> availableSerlots = RtransDao.GetFreeSerlotList_ForArtikl(TheDbConnection, artiklCD, Fld_IzlaznoSkl, Fld_DokDate);
      
      #region Ako zelimo via AutoComplete

      //var strings = availableSerlots.Select(s => "RGC-" + s.TheName + " : " + s.TheDecimal.ToStringVv() + "kg");
      
      //string[] stringArray = strings.ToArray();
      //
      //vvtbT_serlot.AutoCompleteCustomSource.Clear();
      //vvtbT_serlot.AutoCompleteCustomSource.AddRange(stringArray);
      
      #endregion Ako zelimo via AutoComplete
      
      ZXC.luiListaSerlot.Clear();
      
      availableSerlots.ForEach(sl => ZXC.luiListaSerlot.Add(new VvLookUpItem(sl.TheStr1, sl.TheStr2, sl.TheDecimal, false, 0, sl.TheDate, 0, "")));

      vvtbT_kpdbZiroA_32.JAM_Set_LookUpTable(ZXC.luiListaSerlot, (int)ZXC.Kolona.prva);

   }


   #endregion InitializeTheGrid_Columns()

   #region Fld_

   public uint     Fld_KupDobCd      { get { return tbx_kupDobCd  .GetSomeRecIDField(); } set { tbx_kupDobCd      .PutSomeRecIDField(value); } }
   public string   Fld_KupDobCdAsTxt { get { return tbx_kupDobCd  .Text               ; } set { tbx_kupDobCd      .Text = value            ; } }
   public string   Fld_KupDobName    { get { return tbx_kupDobName.Text               ; } set { tbx_kupDobName    .Text = value            ; } }
   public string   Fld_KupDobTk      { get { return tbx_kupDobTk  .Text               ; } set { tbx_kupDobTk      .Text = value            ; } }
    
   public string   Fld_Primjedba     { get { return tbx_primjedba .Text               ; } set { tbx_primjedba     .Text = value            ; } }
   public string   Fld_Standard      { get { return tbx_standard  .Text               ; } set { tbx_standard      .Text = value            ; } }
   public string   Fld_VrstaRNM      { get { return tbx_vrstaRNM  .Text               ; } set { tbx_vrstaRNM      .Text = value            ; } }
   public string   Fld_VrstaRNMopis  {                                                    set { tbx_vrstaRNMopis  .Text = value            ; } }
   public string   Fld_IzlaznoSkl    { get { return tbx_izlaznoSkl.Text               ; } set { tbx_izlaznoSkl    .Text = value            ; } }
   public string   Fld_IzlaznoSklOpis{                                                    set { tbx_izlaznoSklOpis.Text = value            ; } }
   public string   Fld_UlaznoSkl     { get { return tbx_ulaznoSkl .Text               ; } set { tbx_ulaznoSkl     .Text = value            ; } }
   public string   Fld_UlaznoSklOpis {                                                    set { tbx_ulaznoSklOpis .Text = value            ; } }

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
   
   #endregion _Fld

   #region override PutSpecificsFld() GetSpecificsFld()
   
   /*protected*/public override void PutSpecificsFld()
   {
      PutSpecificsFld(mixer_rec);
   }

   /*protected*/public override void PutSpecificsFld(Mixer mixerLocal_rec)
   {
      if(CtrlOK(tbx_kupDobCd      ))   Fld_KupDobCd        = mixerLocal_rec.KupdobCD  ;
      if(CtrlOK(tbx_kupDobName    ))   Fld_KupDobName      = mixerLocal_rec.KupdobName;
      if(CtrlOK(tbx_kupDobTk      ))   Fld_KupDobTk        = mixerLocal_rec.KupdobTK  ;
      if(CtrlOK(tbx_dateTrazeniRok))   Fld_DateTrazRok     = mixerLocal_rec.DateA     ;
      if(CtrlOK(tbx_primjedba     ))   Fld_Primjedba       = mixerLocal_rec.StrE_256  ;
     
      if(CtrlOK(tbx_v1_tt         ))   Fld_V1_tt           = mixerLocal_rec.V1_tt;
      if(CtrlOK(tbx_v1_ttOpis     ))   Fld_V1_ttOpis       = ZXC.GetNameForThisCdFromManyLuiLists(mixerLocal_rec.V1_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v1_ttNum      ))   Fld_V1_ttNum        = mixerLocal_rec.V1_ttNum;
      if(CtrlOK(tbx_v2_tt         ))   Fld_V2_tt           = mixerLocal_rec.V2_tt;
      if(CtrlOK(tbx_v2_ttOpis     ))   Fld_V2_ttOpis       = ZXC.GetNameForThisCdFromManyLuiLists(mixerLocal_rec.V2_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v2_ttNum      ))   Fld_V2_ttNum        = mixerLocal_rec.V2_ttNum;
      if(CtrlOK(tbx_externLink1   ))   Fld_ExternLink1     = mixerLocal_rec.ExternLink1;
                                  
      if(CtrlOK(tbx_standard      ))   Fld_Standard        = mixerLocal_rec.StrF_64;  
      if(CtrlOK(tbx_vrstaRNM      ))   Fld_VrstaRNM        = mixerLocal_rec.StrD_32;  
      if(CtrlOK(tbx_izlaznoSkl    ))   Fld_IzlaznoSkl      = mixerLocal_rec.Konto  ;  
      if(CtrlOK(tbx_ulaznoSkl     ))   Fld_UlaznoSkl       = mixerLocal_rec.Konto2 ;  

      if(CtrlOK(tbx_vrstaRNMopis  ))   Fld_VrstaRNMopis  = ZXC.luiListaVrstaRNM .GetNameForThisCd(mixerLocal_rec.StrD_32);
      if(CtrlOK(tbx_izlaznoSklOpis))   Fld_IzlaznoSklOpis= ZXC.luiListaSkladista.GetNameForThisCd(mixerLocal_rec.Konto  );
      if(CtrlOK(tbx_ulaznoSklOpis ))   Fld_UlaznoSklOpis = ZXC.luiListaSkladista.GetNameForThisCd(mixerLocal_rec.Konto2 );

      Fld_TtOpis = "Nalog za izradu";
   }

   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd      ))  mixer_rec.KupdobCD    = Fld_KupDobCd   ;
      if(CtrlOK(tbx_kupDobName    ))  mixer_rec.KupdobName  = Fld_KupDobName ;
      if(CtrlOK(tbx_kupDobTk      ))  mixer_rec.KupdobTK    = Fld_KupDobTk   ;
                                                              
      if(CtrlOK(tbx_dateTrazeniRok))  mixer_rec.DateA       = Fld_DateTrazRok;
      if(CtrlOK(tbx_primjedba     ))  mixer_rec.StrE_256    = Fld_Primjedba  ;
                                                              
      if(CtrlOK(tbx_v1_tt         ))  mixer_rec.V1_tt       = Fld_V1_tt      ;
      if(CtrlOK(tbx_v1_ttNum      ))  mixer_rec.V1_ttNum    = Fld_V1_ttNum   ;
      if(CtrlOK(tbx_v2_tt         ))  mixer_rec.V2_tt       = Fld_V2_tt      ;
      if(CtrlOK(tbx_v2_ttNum      ))  mixer_rec.V2_ttNum    = Fld_V2_ttNum   ;
      if(CtrlOK(tbx_externLink1   ))  mixer_rec.ExternLink1 = Fld_ExternLink1;
      if(CtrlOK(tbx_vrstaRNM      ))  mixer_rec.StrD_32     = Fld_VrstaRNM   ;  
      if(CtrlOK(tbx_izlaznoSkl    ))  mixer_rec.Konto       = Fld_IzlaznoSkl ;  
      if(CtrlOK(tbx_ulaznoSkl     ))  mixer_rec.Konto2      = Fld_UlaznoSkl  ;
      if(CtrlOK(tbx_standard      ))  mixer_rec.StrF_64     = Fld_Standard   ;  

   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region PrintDocumentRecord

   public ZahtjevRNMFilterUC TheZahtjevRNMFilterUC { get; set; }
   public ZahtjevRNMFilter   TheZahtjevRNMFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.TheZahtjevRNMFilter = new ZahtjevRNMFilter(this);

      TheZahtjevRNMFilterUC        = new ZahtjevRNMFilterUC(this);
      TheZahtjevRNMFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheZahtjevRNMFilterUC.Width;

   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      ZahtjevRNMFilter mixerFilter = (ZahtjevRNMFilter)vvRptFilter;

      switch(mixerFilter.PrintNzi)
      {
         case ZahtjevRNMFilter.PrintNZIEnum.NZI: specificMixerReport = new RptX_ZahtjevRNM(new Vektor.Reports.XIZ.CR_NazlogZaIzradu(), "NALOG ZA IZRADU", mixerFilter); break;
         
         default: ZXC.aim_emsg("{0}\nPrintSomeDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintNzi); return null;
      }

      return specificMixerReport;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheZahtjevRNMFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheZahtjevRNMFilterUC;
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

public class ZahtjevRNMFilterUC : VvFilterUC
{
   #region Fieldz

   #endregion Fieldz

   #region  Constructor

   public ZahtjevRNMFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;
     
      hamperHorLine.Visible = false;

      this.Width  = hamper4buttons.Width + ZXC.QUN;
      this.Height = hamper4buttons.Bottom + ZXC.QUN;

      hamper4buttons.Visible = false;

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region Put & GetFilterFields

   private ZahtjevRNMFilter TheZahtjevRNMFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as ZahtjevRNMFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheZahtjevRNMFilter = (ZahtjevRNMFilter)_filter_data;

      if(TheZahtjevRNMFilter != null)
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

public class ZahtjevRNMFilter : VvRpt_Mix_Filter
{

   public enum PrintNZIEnum
   {
      NZI
   }

   public PrintNZIEnum PrintNzi { get; set; }

   public ZahtjevRNMDUC theDUC;

   public ZahtjevRNMFilter(ZahtjevRNMDUC _theDUC)
   {
      this.theDUC = _theDUC;
      SetDefaultFilterValues();
   }

   #region SetDefaultFilterValues()

   public override void SetDefaultFilterValues()
   {
      ZXC.VvDataBaseInfo vvDBinfo = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects;
      int projectYear            = int.Parse(vvDBinfo.ProjectYear);
      int thisYear               = DateTime.Now.Year;
      PrintNzi                   = PrintNZIEnum.NZI;
   }

   #endregion SetDefaultFilterValues()

}
