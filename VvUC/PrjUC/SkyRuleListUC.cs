using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

public class SkyRuleListUC : VvRecLstUC
{
   #region Fieldz

   private Vektor.DataLayer.DS_FindRecord.DS_findSkyRule ds_skyRule;
    
   private RadioButton rBtSortByUserName, rBtSortByPrjkt,
                       rBtcurrChecked;
   private VvTextBox   tbx_userName, tbx_prjkt, 
                       tbx_filterUserName     ,
                       tbx_fliterPrjktID, tbx_fliterPrjktTicker, tbx_fliterPrjktNaziv;
   private SkyRule     skyRule_rec;
   
   #endregion Fieldz

   #region Constructor

   public SkyRuleListUC(Control parent, SkyRule _skyRule, VvForm.VvSubModul vvSubModul): base(parent)
   {
      this.skyRule_rec = _skyRule;

      this.MasterSubModulEnum = ZXC.VvSubModulEnum.SKY;
      this.TheSubModul        = vvSubModul;
      this.Parent.Text        = this.TheSubModul.subModul_name;

      TheFilterMembers = new System.Collections.Generic.List<VvSqlFilterMember>(); // namoj tamo di ne treba (npr Kplan) 
   }

   #endregion Constructor

   #region InitializeFindForm

   protected override void InitializeFindFormSpecifics()
   {
      recordSorter = SkyRule.sorterByCode;

      this.ds_skyRule = new Vektor.DataLayer.DS_FindRecord.DS_findSkyRule();
      

      this.Name = "FindSkyRuleForm";
      this.Text = "FindSkyRuleForm";
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

      //rBtSortByUserName         = hampSpecifikum.CreateVvRadioButton(0, 0, new EventHandler(radioButtonSortByUserName_Click), "Od user name:", TextImageRelation.ImageAboveText);
      //rBtSortByUserName.Checked = true;
      //rBtcurrChecked            = rBtSortByUserName;

      //tbx_userName                = hampSpecifikum.CreateVvTextBox(1, 0, "tbx_userName", "Od user name");
      //this.ControlForInitialFocus = tbx_userName;
      //tbx_userName.DoubleClick   += new EventHandler(tbx_DoubleClick);
      //tbx_userName.TextChanged   += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

      //tbx_userName.Tag      = rBtSortByUserName;
      //rBtSortByUserName.Tag = tbx_userName;

      //rBtSortByPrjkt          = hampSpecifikum.CreateVvRadioButton(0, 1, new EventHandler(radioButtonSortByPrjkt_Click), "Od ProjTicker:", TextImageRelation.ImageAboveText);
      //tbx_prjkt               = hampSpecifikum.CreateVvTextBox(1, 1, "tbx_Prjkt", "Od projekta - ticker");
      //tbx_prjkt.JAM_CharacterCasing = CharacterCasing.Upper;
      //tbx_prjkt.DoubleClick  += new EventHandler(tbx_DoubleClick);
      //tbx_prjkt.TextChanged  += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

      //tbx_prjkt.Tag      = rBtSortByPrjkt;
      //rBtSortByPrjkt.Tag = tbx_prjkt;

      //VvHamper.Open_Close_Fields_ForWriting(tbx_userName, ZXC.ZaUpis.Otvoreno,  ZXC.ParentControlKind.VvFindDialog);
      //VvHamper.Open_Close_Fields_ForWriting(tbx_prjkt,    ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
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

      //Label lblFilterUser = hampFilter.CreateVvLabel  (0, 0, "Izlistaj samo user name:", System.Drawing.ContentAlignment.MiddleRight);
      //tbx_filterUserName  = hampFilter.CreateVvTextBox(1, 0, "tbx_filterUserName", "");

      //Label lblFilterPrjkt                = hampFilter.CreateVvLabel  (0, 1, "Izlistaj samo za projekt:", System.Drawing.ContentAlignment.MiddleRight);
      //tbx_fliterPrjktID                   = hampFilter.CreateVvTextBox(1, 1, "tbx_fliterPrjktID", "Sifra projekta");
      //tbx_fliterPrjktID.JAM_CharEdits     = ZXC.JAM_CharEdits.DigitsOnly;
      //tbx_fliterPrjktTicker               = hampFilter.CreateVvTextBox(2, 1, "tbx_fliterPrjktTicker", "Ticker projekta");
      //tbx_fliterPrjktTicker.JAM_CharacterCasing = CharacterCasing.Upper;
      //tbx_fliterPrjktNaziv                = hampFilter.CreateVvTextBox(3, 1, "tbx_fliterPrjktNaziv", "Naziv projekta");

      ////==============================================================================================================================
      
      //tbx_filterUserName.JAM_SetAutoCompleteData(User.recordName , User.sorterUserName.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_User_sorterUserName), null);

      //tbx_fliterPrjktID    .JAM_SetAutoCompleteData(Prjkt.recordName, Prjkt.sorterKCD.SortType     , new EventHandler(OnVvTBEnter_SetAutocmplt_Prjkt_sorterSifra), new EventHandler(AnyPrjktTextBoxLeave));
      //tbx_fliterPrjktTicker.JAM_SetAutoCompleteData(Prjkt.recordName, Prjkt.sorterTicker.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Prjkt_sorterTicker) , new EventHandler(AnyPrjktTextBoxLeave));
      //tbx_fliterPrjktNaziv .JAM_SetAutoCompleteData(Prjkt.recordName, Prjkt.sorterNaziv.SortType  , new EventHandler(OnVvTBEnter_SetAutocmplt_Prjkt_sorterNaziv), new EventHandler(AnyPrjktTextBoxLeave));

      ////==============================================================================================================================

      //VvHamper.Open_Close_Fields_ForWriting(hampFilter, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

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
      //this.recordSorter = SkyRule.sorterByUserName;
   }

   private void radioButtonSortByPrjkt_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      //this.recordSorter = SkyRule.sorterByPrjktTicker;
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

      colWidth = ZXC.Q4un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Record"     , colWidth, false, "record");
      colWidth = ZXC.Q4un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "DocumTT"    , colWidth, false, "documTT");
      colWidth = ZXC.Q4un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "BirthLoc"   , colWidth, false, "birthLoc");
      colWidth = ZXC.Q4un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Skl1kind"   , colWidth, false, "skl1kind");
      colWidth = ZXC.Q4un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Skl2kind"   , colWidth, false, "skl2kind");
      colWidth = ZXC.Q4un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "CentOPS"    , colWidth, false, "centOPS");
      colWidth = ZXC.Q4un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "ShopOPS"    , colWidth, false, "shopOPS");
      colWidth = ZXC.Q6un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "ShopRCVkind", colWidth, false, "shopRCVkind");
      colWidth = ZXC.Q7un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Opis"       , colWidth, true , "opis");
      colWidth = ZXC.Q3un           ;                            AddDGVColum_RecID_4GridReadOnly (TheGrid, "RecID"      , colWidth, false,  0, "recID");

      grid_Width = sumOfColWidth + ZXC.QUN;

  //    TheGrid.CellFormatting += new DataGridViewCellFormattingEventHandler(TheGrid_CellFormatting_GetUnboundValues);
   }

   //void TheGrid_CellFormatting_GetUnboundValues(object sender, DataGridViewCellFormattingEventArgs e)
   //{
   //   DataGridView       dgv    = sender as DataGridView;
   //   DataGridViewColumn column = dgv.Columns[e.ColumnIndex];

   //   if(column.Name != "UnboundLUI_skyRuleScope" &&
   //      column.Name != "UnboundLUI_skyRuleType"  &&
   //      column.Name != "UnboundLUI_documType") return;

   //   string luiCd        = (string)(dgv[e.ColumnIndex - 1, e.RowIndex].Value);
   //   string skyRuleScopeCd = (string)(dgv["skyRuleScope",      e.RowIndex].Value);

   //   switch(column.Name)
   //   {
   //      case "UnboundLUI_skyRuleScope": e.Value = ZXC.luiListaSkyRuleScope.GetNameForThisCd(luiCd); break;
   //      case "UnboundLUI_skyRuleType" : e.Value = ZXC.luiListaSkyRuleType.GetNameForThisCd (luiCd); break;
   //      case "UnboundLUI_documType" : e.Value = GetUnboundValues_DocumentTypeName(luiCd, skyRuleScopeCd); break;
   //   }
   //}

   //private object GetUnboundValues_DocumentTypeName(string luiCD, string skyRuleScopeCd)
   //{
   //   if(SkyRuleUC.IsThis_SkyRuleScopeCd_OneDocumScope(skyRuleScopeCd))
   //   {
   //      VvLookUpLista documNamesList = ZXC.TheVvForm.DocumentTypeLookUpListaForThisModulEnum(SkyRuleUC.ModulFromSkyRuleScopeCd_asList(skyRuleScopeCd));
   //      return documNamesList.GetNameForThisCd(luiCD);
   //   }
   //   else
   //   {
   //      return  "";
   //   }
   //}

   #endregion DataGridView

   #region Overriders And Specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.skyRule_rec; }
      set {        this.skyRule_rec = (SkyRule)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.SkyRuleDao; }
   }

   protected override DataSet VirtualUntypedDataSet { get { return ds_skyRule; } }

   protected override object[] From_IndexSegmentValues
   {
      get
      {
         switch (recordSorter.SortType)
         {
          //case VvSQL.SorterType.Person: return new object[] { Fld_FromUserName,    Fld_FromPrjktTicker, 0, 0 };
          //case VvSQL.SorterType.Ticker: return new object[] { Fld_FromPrjktTicker, Fld_FromUserName,    0, 0 };

          //case VvSQL.SorterType.Code: return new object[] { Fld_LanServerID, Fld_PrjktID, Fld_Record, Fld_DocumTT, Fld_SkladCD, 0, 0 }; // Tamara TODO: !!!!!!!!!!!! 
            case VvSQL.SorterType.Code: return new object[] { 0              , 0          , ""        , ""         , ""         , 0, 0 }; 
            default: ZXC.aim_emsg("Q42: SortType [{0}] undifajnd in property 'From_IndexSegmentValues'", recordSorter.SortType); return null;
         }
      }
   }

   #endregion Overriders and specifics
  
   #region AddFilterMemberz()

   private DataRowCollection SkyRuleSchemaRows
   {
      get { return ZXC.SkyRuleDao.TheSchemaTable.Rows; }
   }

  
   private SkyRuleDao.SkyRuleCI SkyRuleCI
   {
      get { return ZXC.SkyRuleDao.CI; }
   }

   public override void AddFilterMemberz()
   {
      string text;
      DataRow drSchema;

      this.TheFilterMembers.Clear();

      //// Fld_FilterUserName                                                                                                                                          

      //drSchema = SkyRuleSchemaRows[SkyRuleCI.userName];
      //text     = Fld_FilterUsername;

      //if(text.NotEmpty())
      //{
      //   this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Username", text + "%", " LIKE "));
      //}

      //// Fld_FilterPrjktTicker                                                                                                                                            

      //drSchema = SkyRuleSchemaRows[SkyRuleCI.prjktTick];
      //text     = Fld_FilterPrjktTicker;

      //if(text.NotEmpty())
      //{
      //   this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "PrjktTick", text + "%", " LIKE "));
      //}


   }

  #endregion AddFilterMemberz()

}