using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

public class KDC_ListUC : VvRecLstUC
{
   #region Fieldz

   private RadioButton rBtSortByNaziv, rBtcurrChecked;
   private VvTextBox   tbx_naziv, tbx_TT, tbx_TtNum  ;
   private CheckBox    cbx_biloGdjeUnazivu;
   private Xtrans      kdcXtrans_rec;

   #endregion Fieldz

   #region Constructor

   public KDC_ListUC(Control parent, Xtrans _kdcXtrans, VvSubModul vvSubModul) : base(parent)
   {
      this.kdcXtrans_rec = _kdcXtrans;

      this.MasterSubModulEnum = ZXC.VvSubModulEnum.UNDEF; // ?! 
      this.TheSubModul        = vvSubModul;
      this.Parent.Text        = this.TheSubModul.subModul_name;

      Xtrans.sorterKDCnaziv.BiloGdjeU_Tekstu = Fld_BiloGdjeUnazivu = ZXC.TheVvForm.VvPref.findKcdXtrans.IsBiloGdjeUnazivu;
      if(recordSorter.SortType == Xtrans.sorterKDCnaziv.SortType) recordSorter.BiloGdjeU_Tekstu = Xtrans.sorterKDCnaziv.BiloGdjeU_Tekstu;

      TheFilterMembers = new System.Collections.Generic.List<VvSqlFilterMember>(); // namoj tamo di ne treba (npr Kplan) 

   }

   #endregion Constructor

   #region InitializeFindForm

   protected override void InitializeFindFormSpecifics()
   {
      recordSorter = Xtrans.sorterKDCnaziv;

      this.ds_kdcXtrans = new Vektor.DataLayer.DS_FindRecord.DS_findKdcXtrans(); 

      this.Name        = "FindKDCForm";
      this.Text        = "FindKDCForm";
   }

   #endregion InitializeFindForm

   #region DataGridView

   protected override void CreateDataGridViewColumn()
   {
      int sumOfColWidth = 0, colWidth;

      sumOfColWidth += TheGrid.RowHeadersWidth;

      if(IsArhivaTabPage)
      {
         AddDGV_ArhivaColumns(ref sumOfColWidth);
      }

      colWidth = ZXC.Q9un ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Ime i prezime", colWidth, false, "t_kpdbNameA_50");
      colWidth = ZXC.Q9un ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Telefon"      , colWidth, false, "t_kpdbUlBrA_32"  );
      colWidth = ZXC.Q10un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "e-mail"       , colWidth, true , "t_vezniDokA_64");
      colWidth = ZXC.Q7un ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Funkcija"     , colWidth, false, "t_kpdbZiroA_32");
      colWidth = ZXC.Q3un ;                            AddDGVColum_RecID_4GridReadOnly (TheGrid, "RecID"        , colWidth, false, 0, "recID");

      grid_Width = sumOfColWidth + ZXC.QUN;
   }

   #endregion DataGridView

   #region Hamper

   protected override void CreateHamperSpecifikum()
   {
      hampSpecifikum = new VvHamper(5, 2, "", this, true, hampListaRastePada.Right + ZXC.Qun4, nextY, razmakHamp);
      
      hampSpecifikum.VvColWdt      = new int[] { ZXC.Q4un + ZXC.Qun8, ZXC.QUN - ZXC.Qun8, ZXC.Q7un , ZXC.Q2un, ZXC.Q4un };
      hampSpecifikum.VvSpcBefCol   = new int[] {            ZXC.Qun4,          ZXC.Qun12, ZXC.Qun12, ZXC.Q3un, ZXC.Qun12};
      hampSpecifikum.VvRightMargin = hampSpecifikum.VvLeftMargin;
      
      hampSpecifikum.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN };
      hampSpecifikum.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hampSpecifikum.VvBottomMargin = hampSpecifikum.VvTopMargin;
      

      rBtSortByNaziv     = hampSpecifikum.CreateVvRadioButton(0, 0, new EventHandler(radioButtonSortByNaziv_Click), "Od naziva:", TextImageRelation.ImageAboveText);
      rBtSortByNaziv.Checked = true;
      rBtcurrChecked = rBtSortByNaziv;

      cbx_biloGdjeUnazivu = hampSpecifikum.CreateVvCheckBox_OLD(1, 0, CheckBox_biloGdjeUnazivu_Click, "", RightToLeft.No);
      SetVvPrefLink_AND_ToolTipText_ForGenericBiloGdjeUnazivuCheckBox(cbx_biloGdjeUnazivu, new EventHandler(cbx_biloGdjeUnazivu_Click_SaveToVvPref));
      
      tbx_naziv          = hampSpecifikum.CreateVvTextBox    (2, 0, "tbx_naz", "Od naziva konta");
      this.ControlForInitialFocus = tbx_naziv;
      tbx_naziv.DoubleClick += new EventHandler(tbx_DoubleClick);
      tbx_naziv.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);
      
      tbx_naziv.Tag      = rBtSortByNaziv;
      rBtSortByNaziv.Tag = tbx_naziv;
      

      tbx_TT    = hampSpecifikum.CreateVvTextBox(3, 0, "tbx_TT"   , "KDC");
      tbx_TtNum = hampSpecifikum.CreateVvTextBox(4, 0, "tbx_TtNum", "TtNum");
      tbx_TT   .JAM_ReadOnly = true;
      tbx_TtNum.JAM_ReadOnly = true;

      VvHamper.Open_Close_Fields_ForWriting(tbx_TT   , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_TtNum, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_naziv, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
   }

   #endregion Hamper
   
   #region Eveniti

   void cbx_biloGdjeUnazivu_Click_SaveToVvPref(object sender, EventArgs e)
   {
      ZXC.TheVvForm.VvPref.findKcdXtrans.IsBiloGdjeUnazivu = Xtrans.sorterKDCnaziv.BiloGdjeU_Tekstu = Fld_BiloGdjeUnazivu;
   }

   //private void radioButtonSortByKonto_Click(object sender, System.EventArgs e)
   //{
   //   RadioButton rbt   = sender as RadioButton;
   //   rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
   //   this.recordSorter = Kplan.sorterKonto;
   //}
   //
   private void radioButtonSortByNaziv_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Xtrans.sorterKDCnaziv;
   }
   
   private void tbx_DoubleClick(object sender, System.EventArgs e)
   {
      VvTextBox vvTb  = sender as VvTextBox;
      RadioButton rbt = (RadioButton)vvTb.Tag;
   
      if(! rbt.Checked)
         rbt.PerformClick();
   }

   #endregion Eveniti

   #region Fld_

   public string Fld_FromNaziv
   {
      get { return tbx_naziv.Text; }
      set { tbx_naziv.Text = value; }
   }

   public bool Fld_BiloGdjeUnazivu
   {
      get { return cbx_biloGdjeUnazivu.Checked; }
      set {        cbx_biloGdjeUnazivu.Checked = value; }
   }

   public string Fld_TT
   {
      get { return tbx_TT.Text; }
      set {        tbx_TT.Text = value; }
   }

   public uint Fld_TtNum
   {
      get { return tbx_TtNum.GetSomeRecIDField(); }
      set {        tbx_TtNum.PutSomeRecIDField(value); }
   }

   #endregion Fld_

   #region Overriders And Specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.kdcXtrans_rec; }
      set {        this.kdcXtrans_rec = (Xtrans)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.XtransDao; }
   }

   private Vektor.DataLayer.DS_FindRecord.DS_findKdcXtrans ds_kdcXtrans;

   protected override DataSet VirtualUntypedDataSet { get { return ds_kdcXtrans; } }

   protected override object[] From_IndexSegmentValues
   {
      get
      {
         switch(recordSorter.SortType)
         {
            case VvSQL.SorterType.KCDnaziv: return new object[] { Fld_FromNaziv, 0 };
          //case VvSQL.SorterType.Name: return new object[] { Fld_FromNaziv, 0 };
            default: ZXC.aim_emsg("Q42: SortType [{0}] undifajnd in property 'From_IndexSegmentValues'", recordSorter.SortType); return null;
         }
      }
   }

   public override void AddFilterMemberz()
   {
      //if(TheFilterMembers.IsEmpty())
      //{
      //   TheFilterMembers = new System.Collections.Generic.List<VvSqlFilterMember>(2);
      //}

      this.TheFilterMembers.Clear();

    //DataRow drSchema;
   
      DataRowCollection  XtrSch = ZXC.XtransDao.TheSchemaTable.Rows;
      XtransDao.XtransCI XtrCI  = ZXC.XtransDao.CI;
   
      TheFilterMembers.Add(new VvSqlFilterMember(XtrSch[XtrCI.t_tt   ], "theTT"   , Fld_TT   , " = ")); // uvijek Mixer.TT_KDC 
      TheFilterMembers.Add(new VvSqlFilterMember(XtrSch[XtrCI.t_ttNum], "theTTnum", Fld_TtNum, " = ")); // ovo je kupdobCD     
   }

   public uint KupdobCD { get; set; }

   #endregion Overriders and specifics

}