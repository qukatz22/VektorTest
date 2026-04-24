using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;


public class NalogListUC : /*VvRecLstUC*/VvDocumRecLstUC
{
   #region Fildz

   private RadioButton      rbtSortByBroj, rbtSortByDatum, rbtSortByVKnum,
                            rBtcurrChecked;
   private VvTextBox        tbx_dokNum, tbx_dokDate, tbx_VKnum, tbx_VK, tbx_filtVKnj,tbx_filtVKOpis, tbx_filtNapomena;
   private VvDateTimePicker dtp_datum;
   private Nalog            nalog_rec;

   #endregion Fildz

   #region Constructor

   public NalogListUC(Control parent, Nalog _nalog, VvSubModul vvSubModul) : base(parent)
   {
      this.nalog_rec = _nalog;

      this.MasterSubModulEnum = ZXC.VvSubModulEnum.NAL_F;
      this.TheSubModul        = vvSubModul;
      this.Parent.Text        = this.TheSubModul.subModul_name;

      TheFilterMembers = new System.Collections.Generic.List<VvSqlFilterMember>(); // namoj tamo di ne treba (npr Kplan) 
   }

   #endregion Constructor

   #region NalogListUC

   protected override void InitializeFindFormSpecifics()
   {
      recordSorter = Nalog.sorterDokNum;

      this.ds_nalog = new Vektor.DataLayer.DS_FindRecord.DS_findNalog();

      this.Name = "NalogListUC";
      this.Text = "NalogListUC";
   }

   #endregion NalogListUC

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
      tbx_dokNum                  = hampSpecifikum.CreateVvTextBox(1, 0, "tbx_broj", "Od naloga broj", 6, 2,0);
      tbx_dokNum.Tag              = rbtSortByBroj;
      rbtSortByBroj.Tag           = tbx_dokNum;
      this.ControlForInitialFocus = tbx_dokNum;
      tbx_dokNum.DoubleClick += new EventHandler(tbx_DoubleClick);

      rbtSortByDatum                      = hampSpecifikum.CreateVvRadioButton(0, 1, new EventHandler(rbtSortByDatum_Click), "Od datuma:", TextImageRelation.ImageAboveText);
      tbx_dokDate                         = hampSpecifikum.CreateVvTextBox    (1, 1, "tbx_datum", "Od datuma", 12, 2, 0);
      rbtSortByDatum.Tag                  = tbx_dokDate;
      tbx_dokDate.JAM_IsForDateTimePicker = true;
      
      tbx_dokDate.DoubleClick += new EventHandler(tbx_DoubleClick); 
    
      dtp_datum      = hampSpecifikum.CreateVvDateTimePicker(1, 1, "", 2, 0, tbx_dokDate);
      dtp_datum.Name = "datum";
      dtp_datum.Tag  = rbtSortByDatum;

      rbtSortByVKnum       = hampSpecifikum.CreateVvRadioButton  (0, 2, new EventHandler(rbtSortByVKnum_Click), "Od VK:", TextImageRelation.ImageAboveText);
      tbx_VK = hampSpecifikum.CreateVvTextBoxLookUp(1, 2, " tbx_VK", "Od vrste knji�enja");
      tbx_VK.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_VK.Tag           = rbtSortByVKnum;
      rbtSortByVKnum.Tag   = tbx_VK;
      tbx_VK.DoubleClick  += new EventHandler(tbx_DoubleClick);

      tbx_VK.JAM_Set_LookUpTable(ZXC.luiListaNalogTT, (int)ZXC.Kolona.prva);


      Label lb                    = hampSpecifikum.CreateVvLabel  (2, 2, "Broj:"    , System.Drawing.ContentAlignment.MiddleRight);
      tbx_VKnum                   = hampSpecifikum.CreateVvTextBox(3, 2, "tbx_TtNum", "Broj tipa transakcije - vrste knji�enja", 4);
      tbx_VKnum.JAM_CharEdits     = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_VKnum.JAM_FillCharacter = '0';

      VvHamper.Open_Close_Fields_ForWriting(tbx_dokNum , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_dokDate, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(dtp_datum  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_VK     , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_VKnum  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
   }

   protected override void CreateHamperFilter()
   {
      CreateHamperOpenFilter();

      hampFilter = new VvHamper(3, 3, "", this, true, hampOpenFilter.Left, hampOpenFilter.Top, razmakHamp);

      hampFilter.VvColWdt      = new int[] { ZXC.Q10un, ZXC.Q2un, ZXC.Q6un};
      hampFilter.VvSpcBefCol   = new int[] { ZXC.Qun2, ZXC.Qun4, ZXC.Qun4};
      hampFilter.VvRightMargin = hampFilter.VvLeftMargin;

      hampFilter.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN };
      hampFilter.VvSpcBefRow    = new int[] { ZXC.Qun2, ZXC.Qun4, ZXC.Qun4};
      hampFilter.VvBottomMargin = hampFilter.VvTopMargin;

      Label lblFilter = hampFilter.CreateVvLabel(0, 0, "Izlistaj samo one koji se odnose na:", System.Drawing.ContentAlignment.MiddleRight);
      Label lblvk     = hampFilter.CreateVvLabel(0, 1, "Vrstu knji�enja:"                    , System.Drawing.ContentAlignment.MiddleRight);
      Label lblnapom  = hampFilter.CreateVvLabel(0, 2, "Napomenu:"                           , System.Drawing.ContentAlignment.MiddleRight);

      tbx_filtVKnj = hampFilter.CreateVvTextBoxLookUp(1, 1, "tbx_filtVKnj", "Filtriraj prema zadanoj vrsti knji�enja");
      tbx_filtVKnj.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_filtVKOpis              = hampFilter.CreateVvTextBox(2, 1, "tbx_filtVKOpis", "");
      tbx_filtVKOpis.JAM_ReadOnly = true;

      tbx_filtVKnj.JAM_Set_LookUpTable(ZXC.luiListaNalogTT, (int)ZXC.Kolona.prva);
      tbx_filtVKnj.JAM_lui_NameTaker_JAM_Name = tbx_filtVKOpis.JAM_Name;

      tbx_filtNapomena = hampFilter.CreateVvTextBox(1, 2, "tbx_filtVKnj", "Filtriraj prema napomeni", 30, 1, 0);

      VvHamper.Open_Close_Fields_ForWriting(tbx_filtVKnj    , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_filtVKOpis  , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_filtNapomena, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      hampFilter.Visible = false;
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

      colWidth = colSif6Width;                            AddDGVColum_RecID_4GridReadOnly   (TheGrid, "RecID"   , colWidth, false, 0, "recID");
      colWidth = colSif6Width; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (TheGrid, "Nalog"   , colWidth, true , 6, "dokNum");
      colWidth = colDateWidth; sumOfColWidth += colWidth; AddDGVColum_DateTime_4GridReadOnly(TheGrid, "Datum"   , colWidth,           "dokDate");
      colWidth = ZXC.Q2un    ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "VK"      , colWidth, false ,   "tt");
      colWidth = ZXC.Q3un    ; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (TheGrid, "Broj Vk" , colWidth, true , 4, "ttNum");
      colWidth = ZXC.Q10un   ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (TheGrid, "Napomena", colWidth, true    , "napomena");

      grid_Width = sumOfColWidth + ZXC.QUN;
   }


   #endregion DataGridView

   #region Eveniti

   private void rbtSortByBroj_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Nalog.sorterDokNum;

      //VvHamper.Open_Close_Fields_ForWriting(dtp_datum, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      //VvHamper.Open_Close_Fields_ForWriting(tbx_VKnum, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
   }

   private void rbtSortByDatum_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Nalog.sorterDokDate;

      //VvHamper.Open_Close_Fields_ForWriting(dtp_datum, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      //VvHamper.Open_Close_Fields_ForWriting(tbx_VKnum, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
   }

   private void rbtSortByVKnum_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Nalog.sorterTtNum;

      //VvHamper.Open_Close_Fields_ForWriting(dtp_datum, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_VKnum, ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvFindDialog);
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
  
   public string Fld_FromTt
   {
      get { return tbx_VK.Text; }
      set {        tbx_VK.Text = value; }
   }

   public uint Fld_FromTtNum
   {
      get { return ZXC.ValOrZero_UInt(tbx_VKnum.Text); }
      set { tbx_VKnum.Text = value.ToString("0000"); }
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

   public string Fld_FilterTipTran
   {
      get { return tbx_filtVKnj.Text; }
      set {        tbx_filtVKnj.Text = value; }
   }
   #endregion Fld_

   #region Overriders and specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.nalog_rec; }
      set {        this.nalog_rec = (Nalog)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.NalogDao; }
   }

   private Vektor.DataLayer.DS_FindRecord.DS_findNalog ds_nalog;

   protected override DataSet VirtualUntypedDataSet { get { return ds_nalog; } }

   protected override object[] From_IndexSegmentValues
   {
      get
      {
         switch (recordSorter.SortType)
         {
            case VvSQL.SorterType.DokNum:  return new object[] { Fld_FromDokNum,                  0 };
            case VvSQL.SorterType.DokDate: return new object[] { Fld_FromDokDate, Fld_FromDokNum, 0 };
            case VvSQL.SorterType.TtNum:   return new object[] { Fld_FromTt,      Fld_FromTtNum,  0 };

            default: ZXC.aim_emsg("Q42: SortType [{0}] undifajnd in property 'From_IndexSegmentValues'", recordSorter.SortType); return null;
         }
      }
   }

   #region AddFilterMemberz()

   /// <summary>
   /// get { return ZXC.NalogDao.TheSchemaTable.Rows; }
   /// </summary>
   private DataRowCollection NalogSchemaRows
   {
      get { return ZXC.NalogDao.TheSchemaTable.Rows; }
   }

   /// <summary>
   ///  get { return ZXC.NalogDao.CI; }
   /// </summary>
   private NalogDao.NalogCI NalCI
   {
      get { return ZXC.NalogDao.CI; }
   }

   public override void AddFilterMemberz()
   {
      string  text;
      DataRow drSchema;

      this.TheFilterMembers.Clear();

      // Fld_FilterNapomena                                                                                                                                          

      drSchema = NalogSchemaRows[NalCI.napomena];
      text     = Fld_FilterNapomena;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "Napomena", text + "%"," LIKE "));
      }

      // Fld_FilterTT                                                                                                                                            

      drSchema = NalogSchemaRows[NalCI.tt];
      text     = Fld_FilterTipTran;

      if(text.NotEmpty())
      {
         this.TheFilterMembers.Add(new VvSqlFilterMember(drSchema, "VrstaKnj", text, " = "));
      }


   }

   #endregion AddFilterMemberz()

   protected override VvTextBox VvTbx_Virtual_TT       { get { return this.tbx_VK; } }
   protected override VvTextBox VvTbx_VirtualFilter_TT { get { return this.tbx_filtVKnj; } set { this.tbx_filtVKnj = value; } }

   #endregion Overriders and specifics

}
