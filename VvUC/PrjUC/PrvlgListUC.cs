using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

public class PrvlgListUC : VvRecLstUC
{
   #region Fieldz

   private Vektor.DataLayer.DS_FindRecord.DS_findPrvlg ds_prvlg;
 
   private RadioButton rBtSortByUserName, rBtSortByPrjkt,
                       rBtcurrChecked;
   private VvTextBox   tbx_userName, tbx_prjkt, 
                       tbx_filterUserName     ,
                       tbx_fliterPrjktID, tbx_fliterPrjktTicker, tbx_fliterPrjktNaziv;
   private Prvlg       prvlg_rec;
   
   #endregion Fieldz

   #region Constructor

   public PrvlgListUC(Control parent, Prvlg _prvlg, VvSubModul vvSubModul): base(parent)
   {
      this.prvlg_rec = _prvlg;

      this.MasterSubModulEnum = ZXC.VvSubModulEnum.PRV;
      this.TheSubModul        = vvSubModul;
      this.Parent.Text        = this.TheSubModul.subModul_name;

      TheFilterMembers = new System.Collections.Generic.List<VvSqlFilterMember>(); // namoj tamo di ne treba (npr Kplan) 
   }

   #endregion Constructor

   #region InitializeFindForm

   protected override void InitializeFindFormSpecifics()
   {
      recordSorter = Prvlg.sorterByUserName;

      this.ds_prvlg = new Vektor.DataLayer.DS_FindRecord.DS_findPrvlg();

      this.Name = "FindPrivilegijeForm";
      this.Text = "FindPrivilegijeForm";
   }

   #endregion InitializeFindForm
   
   #region CreateHamperSpecifikum

   protected override void CreateHamperSpecifikum()
   {
      hampSpecifikum  = new VvHamper(2, 2, "", this, true, hampListaRastePada.Right + ZXC.Qun4, nextY, razmakHamp);

      hampSpecifikum.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q5un };
      hampSpecifikum.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hampSpecifikum.VvRightMargin = hampSpecifikum.VvLeftMargin;

      hampSpecifikum.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN };
      hampSpecifikum.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hampSpecifikum.VvBottomMargin = hampSpecifikum.VvTopMargin;

      rBtSortByUserName         = hampSpecifikum.CreateVvRadioButton(0, 0, new EventHandler(radioButtonSortByUserName_Click), "Od user name:", TextImageRelation.ImageAboveText);
      rBtSortByUserName.Checked = true;
      rBtcurrChecked            = rBtSortByUserName;

      tbx_userName                = hampSpecifikum.CreateVvTextBox(1, 0, "tbx_userName", "Od user name");
      this.ControlForInitialFocus = tbx_userName;
      tbx_userName.DoubleClick   += new EventHandler(tbx_DoubleClick);
      tbx_userName.TextChanged   += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

      tbx_userName.Tag      = rBtSortByUserName;
      rBtSortByUserName.Tag = tbx_userName;

      rBtSortByPrjkt          = hampSpecifikum.CreateVvRadioButton(0, 1, new EventHandler(radioButtonSortByPrjkt_Click), "Od ProjTicker:", TextImageRelation.ImageAboveText);
      tbx_prjkt               = hampSpecifikum.CreateVvTextBox(1, 1, "tbx_Prjkt", "Od projekta - ticker");
      tbx_prjkt.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_prjkt.DoubleClick  += new EventHandler(tbx_DoubleClick);
      tbx_prjkt.TextChanged  += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

      tbx_prjkt.Tag      = rBtSortByPrjkt;
      rBtSortByPrjkt.Tag = tbx_prjkt;

      VvHamper.Open_Close_Fields_ForWriting(tbx_userName, ZXC.ZaUpis.Otvoreno,  ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_prjkt,    ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
   }

   #endregion CreateHamperSpecifikum

   #region CreateHamperFilter

   protected override void CreateHamperFilter()
   {
      CreateHamperOpenFilter();

      hampFilter = new VvHamper(4, 2, "", this, true, hampOpenFilter.Left, hampOpenFilter.Top, razmakHamp);

      hampFilter.VvColWdt      = new int[] { ZXC.Q8un, ZXC.Q4un, ZXC.Q4un, ZXC.Q8un };
      hampFilter.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hampFilter.VvRightMargin = hampFilter.VvLeftMargin;

      hampFilter.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN };
      hampFilter.VvSpcBefRow    = new int[] { ZXC.Qun2, ZXC.Qun4 };
      hampFilter.VvBottomMargin = hampFilter.VvTopMargin;

      Label lblFilterUser = hampFilter.CreateVvLabel  (0, 0, "Izlistaj samo user name:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_filterUserName  = hampFilter.CreateVvTextBox(1, 0, "tbx_filterUserName", "");

      Label lblFilterPrjkt                = hampFilter.CreateVvLabel  (0, 1, "Izlistaj samo za projekt:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_fliterPrjktID                   = hampFilter.CreateVvTextBox(1, 1, "tbx_fliterPrjktID", "Sifra projekta");
      tbx_fliterPrjktID.JAM_CharEdits     = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_fliterPrjktTicker               = hampFilter.CreateVvTextBox(2, 1, "tbx_fliterPrjktTicker", "Ticker projekta");
      tbx_fliterPrjktTicker.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_fliterPrjktNaziv                = hampFilter.CreateVvTextBox(3, 1, "tbx_fliterPrjktNaziv", "Naziv projekta");

      //==============================================================================================================================
      
      tbx_filterUserName.JAM_SetAutoCompleteData(User.recordName , User.sorterUserName.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_User_sorterUserName), null);

      tbx_fliterPrjktID    .JAM_SetAutoCompleteData(Prjkt.recordName, Prjkt.sorterKCD.SortType     , new EventHandler(OnVvTBEnter_SetAutocmplt_Prjkt_sorterSifra), new EventHandler(AnyPrjktTextBoxLeave));
      tbx_fliterPrjktTicker.JAM_SetAutoCompleteData(Prjkt.recordName, Prjkt.sorterTicker.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Prjkt_sorterTicker) , new EventHandler(AnyPrjktTextBoxLeave));
      tbx_fliterPrjktNaziv .JAM_SetAutoCompleteData(Prjkt.recordName, Prjkt.sorterNaziv.SortType  , new EventHandler(OnVvTBEnter_SetAutocmplt_Prjkt_sorterNaziv), new EventHandler(AnyPrjktTextBoxLeave));

      //==============================================================================================================================

      VvHamper.Open_Close_Fields_ForWriting(hampFilter, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      hampFilter.Visible = false;
   }

   void AnyPrjktTextBoxLeave(object sender, EventArgs e)
   {
      if(this.isPopulatingSifrar) return;

      //if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Prjkt   prjkt_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;
         prjkt_rec         = PrjktSifrar.Find(this.FoundInSifrar<Prjkt>);

         if (prjkt_rec != null && tb.Text != "")
         {
            Fld_FilterPrjktID      = prjkt_rec.KupdobCD/*RecID*/;
            Fld_FilterPrjktTicker  = prjkt_rec.Ticker;
            Fld_FilterPrjktNaziv   = prjkt_rec.Naziv;
         }
         else
         {
            Fld_FilterPrjktTicker = Fld_FilterPrjktIDAsTxt  = Fld_FilterPrjktNaziv = "";
         }
      }
   }

   #endregion CreateHamperFilter

   #region Eveniti

   private void radioButtonSortByUserName_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Prvlg.sorterByUserName;
   }

   private void radioButtonSortByPrjkt_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Prvlg.sorterByPrjktTicker;
   }

   private void tbx_DoubleClick(object sender, System.EventArgs e)
   {
      VvTextBox vvTb = sender as VvTextBox;
      RadioButton rbt = (RadioButton)vvTb.Tag;

      if (!rbt.Checked)
         rbt.PerformClick();
   }

   #endregion Eveniti

   #region Fld_

   public string Fld_FromUserName
   {
      get { return tbx_userName.Text; }
      set {        tbx_userName.Text = value; }
   }

   private string Fld_FromPrjktTicker
   {
      get { return tbx_prjkt.Text; }
      set {        tbx_prjkt.Text = value; }
   }
   public string Fld_FilterUsername
   {
      get { return tbx_filterUserName.Text; }
      set {        tbx_filterUserName.Text = value; }
   }

   public uint Fld_FilterPrjktID
   {
      get { return ZXC.ValOrZero_UInt(tbx_fliterPrjktID.Text); }
      set {                           tbx_fliterPrjktID.Text = value.ToString("000000"); }
   }
   public string Fld_FilterPrjktIDAsTxt
   {
      get { return tbx_fliterPrjktID.Text; }
      set {        tbx_fliterPrjktID.Text = value; }
   }
   public string Fld_FilterPrjktTicker
   {
      get { return tbx_fliterPrjktTicker.Text; }
      set {        tbx_fliterPrjktTicker.Text = value; }
   }
   public string Fld_FilterPrjktNaziv
   {
      get { return tbx_fliterPrjktNaziv.Text; }
      set {        tbx_fliterPrjktNaziv.Text = value; }
   }

   #endregion Fld_

   #region DataGridView

   protected override void CreateDataGridViewColumn()
   {
      int sumOfColWidth = 0, colWidth;

      sumOfColWidth += TheGrid.RowHeadersWidth;

      if(IsArhivaTabPage)
      {
         AddDGV_ArhivaColumns(ref sumOfColWidth);
      }

      colWidth = ZXC.Q4un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly (TheGrid, "UserName"  , colWidth, true    , "userName");
      colWidth = ZXC.Q4un - ZXC.Qun4; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly(TheGrid, "Projekt ID", colWidth, true , 6, "prjktId");
      colWidth = ZXC.Q4un - ZXC.Qun4; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly (TheGrid, "ProjTicker", colWidth, false   , "prjktTick");
      colWidth = ZXC.Q2un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly (TheGrid, ""          , colWidth, false   , "prvlgType");
      // 'unbound' 
      colWidth = ZXC.Q7un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Privilegija", colWidth, false   , "UnboundLUI_prvlgType");
      colWidth = ZXC.Q2un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, ""           , colWidth, false   , "prvlgScope");
      // 'unbound' 
      colWidth = ZXC.Q5un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Doseg"      , colWidth, false   , "UnboundLUI_prvlgScope");
      colWidth = ZXC.Q2un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, ""           , colWidth, false   , "documType");
      colWidth = ZXC.Q7un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Dokument"   , colWidth, false   , "UnboundLUI_documType");
      colWidth = ZXC.Q3un           ;                            AddDGVColum_RecID_4GridReadOnly (TheGrid, "RecID"      , colWidth, false, 0, "recID");

      grid_Width = sumOfColWidth + ZXC.QUN;

      TheGrid.CellFormatting += new DataGridViewCellFormattingEventHandler(TheGrid_CellFormatting_GetUnboundValues);
   }

   void TheGrid_CellFormatting_GetUnboundValues(object sender, DataGridViewCellFormattingEventArgs e)
   {
      DataGridView       dgv    = sender as DataGridView;
      DataGridViewColumn column = dgv.Columns[e.ColumnIndex];

      if(column.Name != "UnboundLUI_prvlgScope" &&
         column.Name != "UnboundLUI_prvlgType"  &&
         column.Name != "UnboundLUI_documType") return;

      string luiCd        = (string)(dgv[e.ColumnIndex - 1, e.RowIndex].Value);
      string prvlgScopeCd = (string)(dgv["prvlgScope",      e.RowIndex].Value);

      switch(column.Name)
      {
         case "UnboundLUI_prvlgScope": e.Value = ZXC.luiListaPrvlgScope.GetNameForThisCd(luiCd); break;
         case "UnboundLUI_prvlgType" : e.Value = ZXC.luiListaPrvlgType.GetNameForThisCd (luiCd); break;
         case "UnboundLUI_documType" : e.Value = GetUnboundValues_DocumentTypeName(luiCd, prvlgScopeCd); break;
      }
   }

   private object GetUnboundValues_DocumentTypeName(string luiCD, string prvlgScopeCd)
   {
      if(PrvlgUC.IsThis_PrvlgScopeCd_OneDocumScope(prvlgScopeCd))
      {
         VvLookUpLista documNamesList = ZXC.TheVvForm.DocumentTypeLookUpListaForThisModulEnum(PrvlgUC.ModulFromPrvlgScopeCd_asList(prvlgScopeCd));
         return documNamesList.GetNameForThisCd(luiCD);
      }
      else
      {
         return  "";
      }
   }

   #endregion DataGridView

   #region Overriders And Specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.prvlg_rec; }
      set {        this.prvlg_rec = (Prvlg)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.PrvlgDao; }
   }

   protected override DataSet VirtualUntypedDataSet { get { return ds_prvlg; } }

   protected override object[] From_IndexSegmentValues
   {
      get
      {
         switch (recordSorter.SortType)
         {
            case VvSQL.SorterType.Person: return new object[] { Fld_FromUserName,    Fld_FromPrjktTicker, 0, 0 };
            case VvSQL.SorterType.Ticker: return new object[] { Fld_FromPrjktTicker, Fld_FromUserName,    0, 0 };

            default: ZXC.aim_emsg("Q42: SortType [{0}] undifajnd in property 'From_IndexSegmentValues'", recordSorter.SortType); return null;
         }
      }
   }

   #endregion Overriders and specifics
  
   #region AddFilterMemberz()

   private DataRowCollection PrvlgSchemaRows
   {
      get { return ZXC.PrvlgDao.TheSchemaTable.Rows; }
   }

  
   private PrvlgDao.PrvlgCI PrvlgCI
   {
      get { return ZXC.PrvlgDao.CI; }
   }

   public override void AddFilterMemberz()
   {
      string text;
      DataRow drSchema;

      this.TheFilterMembers.Clear();

      // Fld_FilterUserName                                                                                                                                          

      drSchema = PrvlgSchemaRows[PrvlgCI.userName];
      text     = Fld_FilterUsername;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Username", text + "%", " LIKE "));
      }

      // Fld_FilterPrjktTicker                                                                                                                                            

      drSchema = PrvlgSchemaRows[PrvlgCI.prjktTick];
      text     = Fld_FilterPrjktTicker;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "PrjktTick", text + "%", " LIKE "));
      }


   }

  #endregion AddFilterMemberz()

}