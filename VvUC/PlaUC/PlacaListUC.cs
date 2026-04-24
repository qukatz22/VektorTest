using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

public class PlacaListUC : /*VvRecLstUC*/VvDocumRecLstUC
{
   #region Fieldz

   private RadioButton      rbtSortByBroj, rbtSortByDatum, rbtSortByTTnum,
                            rBtcurrChecked;
   private VvTextBox        tbx_dokNum, tbx_dokDate, tbx_TTnum, tbx_TT, tbx_filtTT,tbx_filtTTOpis, tbx_filtNapomena,
                            tbx_filtMMYYYY, tbx_filtVrstaObr, tbx_filtmtros_cd, tbx_filtmtros_tk, tbx_filtmtros_Naziv,
                            tbx_filtRSmID;
   private VvDateTimePicker dtp_datum;
   private Placa            placa_rec;

   #endregion Fieldz

   #region Constructor

   public PlacaListUC(Control parent, Placa _placa, VvSubModul vvSubModul) : base(parent)
   {
      this.placa_rec = _placa;

      this.MasterSubModulEnum = ZXC.VvSubModulEnum.PLA;
      this.TheSubModul        = vvSubModul;
      this.Parent.Text        = this.TheSubModul.subModul_name;

      TheFilterMembers = new System.Collections.Generic.List<VvSqlFilterMember>(); // namoj tamo di ne treba (npr Kplan) 
   }
   
   #endregion Constructor

   #region PlacaListUC

   protected override void InitializeFindFormSpecifics()
   {
      recordSorter = Placa.sorterDokNum;

      this.ds_placa = new Vektor.DataLayer.DS_FindRecord.DS_findPlaca();

      this.Name = "PlacaListUC";
      this.Text = "PlacaListUC";
   }

   #endregion PlacaListUC

   #region Hamper-i

   protected override void CreateHamperSpecifikum()
   {
      hampSpecifikum = new VvHamper(4, 3, "", this, true, hampListaRastePada.Right + ZXC.Qun4, nextY, razmakHamp);

      hampSpecifikum.VvColWdt      = new int[] { ZXC.Q4un + ZXC.Qun2, ZXC.Q2un + ZXC.Qun2, ZXC.Q2un - ZXC.Qun5, ZXC.Q2un + ZXC.Qun2 };
      hampSpecifikum.VvSpcBefCol   = new int[] {            ZXC.Qun8,            ZXC.Qun8,            ZXC.Qun8, ZXC.Qun8 };
      hampSpecifikum.VvRightMargin = hampSpecifikum.VvLeftMargin;

      hampSpecifikum.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hampSpecifikum.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hampSpecifikum.VvBottomMargin = hampSpecifikum.VvTopMargin;

      rbtSortByBroj               = hampSpecifikum.CreateVvRadioButton(0, 0, new EventHandler(rbtSortByBroj_Click), "Od broja:", TextImageRelation.ImageAboveText);
      rbtSortByBroj.Checked       = true;
      tbx_dokNum                  = hampSpecifikum.CreateVvTextBox(1, 0, "tbx_broj", "", 6, 2,0);
      tbx_dokNum.Tag              = rbtSortByBroj;
      rbtSortByBroj.Tag           = tbx_dokNum;
      this.ControlForInitialFocus = tbx_dokNum;
      tbx_dokNum.DoubleClick += new EventHandler(tbx_DoubleClick);

      rbtSortByDatum                      = hampSpecifikum.CreateVvRadioButton(0, 1, new EventHandler(rbtSortByDatum_Click), "Od datuma:", TextImageRelation.ImageAboveText);
      tbx_dokDate                         = hampSpecifikum.CreateVvTextBox    (1, 1, "tbx_datum", "", 12, 2, 0);
      rbtSortByDatum.Tag                  = tbx_dokDate;
      tbx_dokDate.JAM_IsForDateTimePicker = true;
      tbx_dokDate.DoubleClick += new EventHandler(tbx_DoubleClick); 
    
      dtp_datum         = hampSpecifikum.CreateVvDateTimePicker(1, 1, "", 2, 0, tbx_dokDate);
      dtp_datum.Name    = "datum";
      dtp_datum.Tag = rbtSortByDatum;

      rbtSortByTTnum       = hampSpecifikum.CreateVvRadioButton  (0, 2, new EventHandler(rbtSortByVKnum_Click), "Od TT:", TextImageRelation.ImageAboveText);
      tbx_TT = hampSpecifikum.CreateVvTextBoxLookUp(1, 2, " tbx_TT", "");
      tbx_TT.JAM_CharacterCasing  = CharacterCasing.Upper;
      tbx_TT.Tag           = rbtSortByTTnum;
      rbtSortByTTnum.Tag   = tbx_TT;
      tbx_TT.DoubleClick  += new EventHandler(tbx_DoubleClick);

      tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaPlacaTT, (int)ZXC.Kolona.prva);

      Label lb                    = hampSpecifikum.CreateVvLabel  (2, 2, "Broj:"    , System.Drawing.ContentAlignment.MiddleRight);
      tbx_TTnum                   = hampSpecifikum.CreateVvTextBox(3, 2, "tbx_TtNum", "", 4);
      tbx_TTnum.JAM_CharEdits     = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_TTnum.JAM_FillCharacter = '0';

      VvHamper.Open_Close_Fields_ForWriting(tbx_dokNum , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_dokDate, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(dtp_datum  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_TT     , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_TTnum  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
   }

   protected override void CreateHamperFilter()
   {
      CreateHamperOpenFilter();

      hampFilter = new VvHamper(10, 3, "", this, true, hampOpenFilter.Left, hampOpenFilter.Top, razmakHamp);

      hampFilter.VvColWdt      = new int[] { ZXC.Q10un, ZXC.Q2un, ZXC.QUN , ZXC.Q3un, ZXC.Q2un, ZXC.Q6un, ZXC.Q3un , ZXC.Q3un, ZXC.Q5un, ZXC.Q2un};
      hampFilter.VvSpcBefCol   = new int[] { ZXC.Qun2 , ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 , ZXC.Qun4, ZXC.Qun4, ZXC.Qun4} ;
      hampFilter.VvRightMargin = hampFilter.VvLeftMargin;

      hampFilter.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN };
      hampFilter.VvSpcBefRow    = new int[] { ZXC.Qun2, ZXC.Qun4, ZXC.Qun4};
      hampFilter.VvBottomMargin = hampFilter.VvTopMargin;

      Label lbl_filtMMYYYY, lbl_filtVrstaObr, lbl_filtmtros, lbl_filtRSmID, lbl_Filter, lbl_filtTt, lbl_filtNapom; 

      lbl_Filter     = hampFilter.CreateVvLabel(0, 0, "Izlistaj samo one koji se odnose na:", System.Drawing.ContentAlignment.MiddleRight);

      lbl_filtTt = hampFilter.CreateVvLabel        (0, 1, "TT:"       , System.Drawing.ContentAlignment.MiddleRight);
      tbx_filtTT = hampFilter.CreateVvTextBoxLookUp(1, 1, "tbx_filtTT", "Odabir vrste place");
      tbx_filtTT.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_filtTTOpis              = hampFilter.CreateVvTextBox(2, 1, "tbx_filtTTOpis", "", 32, 2, 0);
      tbx_filtTTOpis.JAM_ReadOnly = true;
      tbx_filtTT.JAM_Set_LookUpTable(ZXC.luiListaPlacaTT, (int)ZXC.Kolona.prva);
      tbx_filtTT.JAM_lui_NameTaker_JAM_Name = tbx_filtTTOpis.JAM_Name;


      lbl_filtMMYYYY = hampFilter.CreateVvLabel        (0, 2, "Za mjesec:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filtMMYYYY = hampFilter.CreateVvTextBoxLookUp("tbx_filtMMYYYY",1, 2,  "Za mjesec i godinu", 6, 1, 0);
      tbx_filtMMYYYY.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      // 14.10.2010:
      //VvLookUpLista fondSatiLista = ZXC.CURR_prjkt_rec.IsTrgRs ? ZXC.luiListaFondSati_TRG : ZXC.luiListaFondSati_NOR;
      VvLookUpLista fondSatiLista = ZXC.luiListaFondSati_NOR;
      tbx_filtMMYYYY.JAM_Set_LookUpTable(fondSatiLista, (int)ZXC.Kolona.prva);

      lbl_filtRSmID = hampFilter.CreateVvLabel  (5, 0, "R-Sm Identifikator:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filtRSmID = hampFilter.CreateVvTextBox(6, 0, "tbx_filtRSmID", "R-Sm Identifikator");
      tbx_filtRSmID.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      lbl_filtVrstaObr = hampFilter.CreateVvLabel        (7, 0, "Vrsta Obračuna Za R-Sm:", 1, 0, System.Drawing.ContentAlignment.MiddleRight);
      tbx_filtVrstaObr = hampFilter.CreateVvTextBoxLookUp(9, 0, "tbx_filtVrstaObr", "Vrsta Obračuna Za R-Sm");
      tbx_filtVrstaObr.JAM_Set_LookUpTable(ZXC.luiListaPlacaVrstaObr, (int)ZXC.Kolona.prva);

      lbl_filtmtros       = hampFilter.CreateVvLabel  (5, 1, "Mjesto Troška:"  , System.Drawing.ContentAlignment.MiddleRight);
      tbx_filtmtros_cd    = hampFilter.CreateVvTextBox(6, 1, "tbx_filtmtros_cd", "Šifra mjesta troška");
      tbx_filtmtros_tk    = hampFilter.CreateVvTextBox(7, 1, "tbx_filtmtros_tk", "Tiker mjesta troška");
      tbx_filtmtros_Naziv = hampFilter.CreateVvTextBox(8, 1, "tbx_mtros_naziv" , "Naziv mjesta troška", 32, 1, 0);

      tbx_filtmtros_cd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_filtmtros_tk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_filtmtros_cd   .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType    , ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_filtmtros_tk   .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_filtmtros_Naziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyMtrosTextBoxLeave));

      lbl_filtNapom    = hampFilter.CreateVvLabel  (5, 2, "Napomena:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filtNapomena = hampFilter.CreateVvTextBox(6, 2, "tbx_filtNapomena", "", 32, 3, 0);

      VvHamper.Open_Close_Fields_ForWriting(tbx_filtTT         , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_filtTTOpis     , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_filtMMYYYY     , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_filtVrstaObr   , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_filtmtros_cd   , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_filtmtros_tk   , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_filtmtros_Naziv, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_filtRSmID      , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_filtNapomena   , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      hampFilter.Visible = false;
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
            Fld_FilterMtrosCd    = kupdob_rec.KupdobCD/*RecID*/;
            Fld_FilterMtrosTk    = kupdob_rec.Ticker;
            Fld_FilterMtrosNaziv = kupdob_rec.Naziv;
         }
         else
         {
            Fld_FilterMtrosCdAsTxt = Fld_FilterMtrosTk = Fld_FilterMtrosNaziv = "";
         }
      }
   }

   #endregion Hamper-i

   #region DataGridView

   protected override void CreateDataGridViewColumn()
   {
      int sumOfColWidth = 0, colWidth;
      int colDateWidth = ZXC.Q4un + ZXC.Qun4;
      int colSif6Width = ZXC.Q3un + ZXC.Qun8;

      sumOfColWidth += TheGrid.RowHeadersWidth;

      if(IsArhivaTabPage)
      {
         AddDGV_ArhivaColumns(ref sumOfColWidth);
      }

      colWidth = colSif6Width;                                   AddDGVColum_RecID_4GridReadOnly   (TheGrid, "RecID"     , colWidth, false, 0, "recID");
      colWidth = ZXC.Q3un           ; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (TheGrid, "BrDok"     , colWidth, true , 6, "dokNum");
      colWidth = colDateWidth       ; sumOfColWidth += colWidth; AddDGVColum_DateTime_4GridReadOnly(TheGrid, "Datum"     , colWidth,           "dokDate");
      colWidth = ZXC.Q2un - ZXC.Qun2; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "TT"        , colWidth, false,    "tt");
      colWidth = ZXC.Q2un + ZXC.Qun2; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (TheGrid, "TTBr"      , colWidth, true , 4, "ttNum");
      colWidth = ZXC.Q3un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "ZaMjGod"   , colWidth, false,    "mmyyyy");
      colWidth = ZXC.Q2un           ; sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly (TheGrid, "Sati"      , colWidth,        0, "fondSati");
      colWidth = ZXC.Q3un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "RSm ID"    , colWidth, false,    "rSm_ID");
      colWidth = ZXC.Q3un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "VrsObr"    , colWidth, false,    "vrstaObr");
      colWidth = ZXC.Q3un + ZXC.Qun2; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (TheGrid, "MTr_Sifra" , colWidth, true , 6, "mtros_cd");
      colWidth = ZXC.Q3un + ZXC.Qun2; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "MTr_Tiker" , colWidth, false,    "mtros_tk");
      colWidth = ZXC.Q10un          ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Napomena", colWidth, true    , "napomena");

      grid_Width = sumOfColWidth + ZXC.QUN;
   }

   #endregion DataGridView

   #region Eveniti

   private void rbtSortByBroj_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Placa.sorterDokNum;

      //VvHamper.Open_Close_Fields_ForWriting(dtp_datum, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      //VvHamper.Open_Close_Fields_ForWriting(tbx_TTnum, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
   }

   private void rbtSortByDatum_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Placa.sorterDokDate;

      //VvHamper.Open_Close_Fields_ForWriting(dtp_datum, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      //VvHamper.Open_Close_Fields_ForWriting(tbx_TTnum, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
   }

   private void rbtSortByVKnum_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Placa.sorterTtNum;

      //VvHamper.Open_Close_Fields_ForWriting(dtp_datum, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_TTnum, ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvFindDialog);
   }
   
   private void tbx_DoubleClick(object sender, System.EventArgs e)
   {
      VvTextBox vvTb = sender as VvTextBox;
      RadioButton rbt;

      if(vvTb.Tag is DateTimePicker)
         rbt = (RadioButton)((DateTimePicker)vvTb.Tag).Tag;
      else
         rbt = (RadioButton)vvTb.Tag;

      if(!rbt.Checked) rbt.PerformClick();
   }

   #endregion Eveniti

   #region Fld_

   public uint Fld_FromDokNum
   {
      get { return ZXC.ValOrZero_UInt(tbx_dokNum.Text); }
      set {                           tbx_dokNum.Text = value.ToString(); }
   }

   public string Fld_FromDatumAsTxt
   {
      get { return tbx_dokDate.Text; }
      set {        tbx_dokDate.Text = value; }
   }
  
   public string Fld_FromTT
   {
      get { return tbx_TT.Text; }
      set {        tbx_TT.Text = value; }
   }

   public uint Fld_FromTtNum
   {
      get { return ZXC.ValOrZero_UInt(tbx_TTnum.Text); }
      set { tbx_TTnum.Text = value.ToString("0000"); }
   }

   public DateTime Fld_FromDokDate
   {
      get
      {
         return dtp_datum.Value;
      }
      set
      {
         dtp_datum.Value    = value;
         Fld_FromDatumAsTxt = value.ToString(dtp_datum.CustomFormat);
      }
   }

   public string Fld_FilterNapomena
   {
      get { return tbx_filtNapomena.Text; }
      set {        tbx_filtNapomena.Text = value; }
   }

   public string Fld_FilterTT
   {
      get { return tbx_filtTT.Text; }
      set {        tbx_filtTT.Text = value; }
   }

   public string Fld_FilterMMYYYY
   {
      get { return tbx_filtMMYYYY.Text; }
      set {        tbx_filtMMYYYY.Text = value; }
   }

   public string Fld_FilterVrstaObr
   {
      get { return tbx_filtVrstaObr.Text; }
      set {        tbx_filtVrstaObr.Text = value; }
   }

   public uint Fld_FilterMtrosCd
   {
      get { return tbx_filtmtros_cd.GetSomeRecIDField(); }
      set {        tbx_filtmtros_cd.PutSomeRecIDField(value); }
   }

   public string Fld_FilterMtrosCdAsTxt
   {
      get { return tbx_filtmtros_cd.Text;         }
      set {        tbx_filtmtros_cd.Text = value; }
   }

   public string Fld_FilterMtrosTk
   {
      get { return tbx_filtmtros_tk.Text; }
      set {        tbx_filtmtros_tk.Text = value; }
   }

   public string Fld_FilterMtrosNaziv
   {
      get { return tbx_filtmtros_Naziv.Text; }
      set {        tbx_filtmtros_Naziv.Text = value; }
   }

   public string Fld_FilterRSm_ID
   {
      get { return tbx_filtRSmID.Text; }
      set {        tbx_filtRSmID.Text = value; }
   }

   #endregion Fld_

   #region Overriders and specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.placa_rec; }
      set {        this.placa_rec = (Placa)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.PlacaDao; }
   }

   private Vektor.DataLayer.DS_FindRecord.DS_findPlaca ds_placa;
   
   protected override DataSet VirtualUntypedDataSet { get { return ds_placa; } }

   protected override object[] From_IndexSegmentValues
   {
      get
      {
         switch (recordSorter.SortType)
         {
            case VvSQL.SorterType.DokNum:  return new object[] { Fld_FromDokNum,                 0 };
            case VvSQL.SorterType.DokDate: return new object[] { Fld_FromDokDate, Fld_FromDokNum, 0 };
            case VvSQL.SorterType.TtNum:   return new object[] { Fld_FromTT,      Fld_FromTtNum,  0 };

            default: ZXC.aim_emsg("Q42: SortType [{0}] undifajnd in property 'From_IndexSegmentValues'", recordSorter.SortType); return null;
         }
      }
   }

   #region AddFilterMemberz()

   /// <summary>
   /// get { return ZXC.PlacaDao.TheSchemaTable.Rows; }
   /// </summary>
   private DataRowCollection PlacaSchemaRows
   {
      get { return ZXC.PlacaDao.TheSchemaTable.Rows; }
   }

   /// <summary>
   ///  get { return ZXC.PlacaDao.CI; }
   /// </summary>
   private PlacaDao.PlacaCI PlaCI
   {
      get { return ZXC.PlacaDao.CI; }
   }

   public override void AddFilterMemberz()
   {
      string  text;
      DataRow drSchema;
      uint    num;

      this.TheFilterMembers.Clear();

      // Fld_FilterNapomena                                                                                                                                          

      drSchema = PlacaSchemaRows[PlaCI.napomena];
      text     = Fld_FilterNapomena;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Napomena", text + "%", " LIKE "));
      }

      // Fld_FilterTT                                                                                                                                            

      drSchema = PlacaSchemaRows[PlaCI.tt];
      text     = Fld_FilterTT;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "TT", text, " = "));
      }

      //  Fld_FilterMMYYYY                                                                                                             

      drSchema = PlacaSchemaRows[PlaCI.mmyyyy];
      text     = Fld_FilterMMYYYY;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "MMYYYY", text, " = "));
      }

      //  Fld_FilterVrstaObr                                                                                                                       

      drSchema = PlacaSchemaRows[PlaCI.vrstaObr];
      text     = Fld_FilterVrstaObr;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "VrstaObr", text, " = "));
      }

      //  Fld_FilterMtrosCd

      drSchema = PlacaSchemaRows[PlaCI.mtros_cd];
      num      = Fld_FilterMtrosCd;

      if(num != 0)
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Mtros", num, " = "));
      }

      //  Fld_FilterRSm_ID
      
      drSchema = PlacaSchemaRows[PlaCI.rSm_ID];
      text     = Fld_FilterRSm_ID;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "RSmID", text, " = "));
      }
   }

   #endregion AddFilterMemberz()

   protected override VvTextBox VvTbx_Virtual_TT       { get { return this.tbx_TT; } }
   protected override VvTextBox VvTbx_VirtualFilter_TT { get { return this.tbx_filtTT; } set { this.tbx_filtTT = value; } }

   #endregion Overriders and specifics

}
