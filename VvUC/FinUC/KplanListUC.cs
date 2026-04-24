using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

public class KplanListUC : VvRecLstUC
{
   #region Fieldz

   private RadioButton rBtSortByNaziv, rBtSortByKonto, 
                       rBtcurrChecked;
   private VvTextBox   tbx_konto, tbx_naziv;
   private CheckBox    cbx_biloGdjeUnazivu;
   private Kplan       kplan_rec;

   #endregion Fieldz

   #region Constructor

   public KplanListUC(Control parent, Kplan _kplan, VvSubModul vvSubModul) : base(parent)
   {
      this.kplan_rec = _kplan;

      this.MasterSubModulEnum = ZXC.VvSubModulEnum.KPL;
      this.TheSubModul        = vvSubModul;
      this.Parent.Text        = this.TheSubModul.subModul_name;

      Kplan.sorterNaziv.BiloGdjeU_Tekstu = Fld_BiloGdjeUnazivu = ZXC.TheVvForm.VvPref.findKplan.IsBiloGdjeUnazivu;
      if(recordSorter.SortType == Kplan.sorterNaziv.SortType) recordSorter.BiloGdjeU_Tekstu = Kplan.sorterNaziv.BiloGdjeU_Tekstu;
   }

   #endregion Constructor

   #region InitializeFindForm

   protected override void InitializeFindFormSpecifics()
   {
      recordSorter = Kplan.sorterKonto;

      this.ds_kplan = new Vektor.DataLayer.DS_FindRecord.DS_findKplan();

      this.Name        = "FindKplanForm";
      this.Text        = "FindKplanForm";
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

      colWidth = ZXC.Q4un;           sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Konto"      , colWidth, false,    "konto");
      colWidth = ZXC.QUN + ZXC.Qun2; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Tip"        , colWidth, false,    "tip"  );
      colWidth = ZXC.Q10un+ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Naziv konta", colWidth, true ,    "naziv");
      colWidth = ZXC.Q2un          ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "AGrupa"     , colWidth, false,    "anaGr");
      colWidth = ZXC.Q10un         ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Opis konta" , colWidth, false ,   "opis" );
      colWidth = ZXC.Q3un          ;                            AddDGVColum_RecID_4GridReadOnly (TheGrid, "RecID"      , colWidth, false, 0, "recID");

      grid_Width = sumOfColWidth + ZXC.QUN;
   }

   #endregion DataGridView

   #region Hamper

   protected override void CreateHamperSpecifikum()
   {
      hampSpecifikum = new VvHamper(3, 2, "", this, true, hampListaRastePada.Right + ZXC.Qun4, nextY, razmakHamp);

      hampSpecifikum.VvColWdt      = new int[] { ZXC.Q4un + ZXC.Qun8, ZXC.QUN - ZXC.Qun8, ZXC.Q5un };
      hampSpecifikum.VvSpcBefCol   = new int[] {            ZXC.Qun4,          ZXC.Qun12, ZXC.Qun12 };
      hampSpecifikum.VvRightMargin = hampSpecifikum.VvLeftMargin;

      hampSpecifikum.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN };
      hampSpecifikum.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hampSpecifikum.VvBottomMargin = hampSpecifikum.VvTopMargin;

      rBtSortByKonto         = hampSpecifikum.CreateVvRadioButton(0, 0, new EventHandler(radioButtonSortByKonto_Click), "Od konta:", TextImageRelation.ImageAboveText);
      rBtSortByKonto.Checked = true;
      rBtcurrChecked         = rBtSortByKonto;

      tbx_konto                   = hampSpecifikum.CreateVvTextBox(2, 0, "tbx_kto", "Od konta", 8);
      this.ControlForInitialFocus = tbx_konto;
      tbx_konto.JAM_CharEdits     = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_konto.DoubleClick += new EventHandler(tbx_DoubleClick);

      tbx_konto.Tag          = rBtSortByKonto;
      rBtSortByKonto.Tag     = tbx_konto;

      tbx_konto.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);


      rBtSortByNaziv     = hampSpecifikum.CreateVvRadioButton(0, 1, new EventHandler(radioButtonSortByNaziv_Click), "Od naziva:", TextImageRelation.ImageAboveText);

      cbx_biloGdjeUnazivu = hampSpecifikum.CreateVvCheckBox_OLD(1, 1, CheckBox_biloGdjeUnazivu_Click, "", RightToLeft.No);
      SetVvPrefLink_AND_ToolTipText_ForGenericBiloGdjeUnazivuCheckBox(cbx_biloGdjeUnazivu, new EventHandler(cbx_biloGdjeUnazivu_Click_SaveToVvPref));

      tbx_naziv          = hampSpecifikum.CreateVvTextBox    (2, 1, "tbx_naz", "Od naziva konta");
      tbx_naziv.DoubleClick += new EventHandler(tbx_DoubleClick);
      tbx_naziv.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

      tbx_naziv.Tag      = rBtSortByNaziv;
      rBtSortByNaziv.Tag = tbx_naziv;

      VvHamper.Open_Close_Fields_ForWriting(tbx_konto, ZXC.ZaUpis.Otvoreno,  ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_naziv, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
   }

   #endregion Hamper
   
   #region Eveniti

   void cbx_biloGdjeUnazivu_Click_SaveToVvPref(object sender, EventArgs e)
   {
      ZXC.TheVvForm.VvPref.findKplan.IsBiloGdjeUnazivu = Kplan.sorterNaziv.BiloGdjeU_Tekstu = Fld_BiloGdjeUnazivu;
   }

   private void radioButtonSortByKonto_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Kplan.sorterKonto;
   }

   private void radioButtonSortByNaziv_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = Kplan.sorterNaziv;
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

   public string Fld_FromKonto
   {
      get { return tbx_konto.Text; }
      set { tbx_konto.Text = value; }
   }

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

   #endregion Fld_
   
   #region Overriders And Specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.kplan_rec; }
      set {        this.kplan_rec = (Kplan)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.KplanDao; }
   }

   private Vektor.DataLayer.DS_FindRecord.DS_findKplan ds_kplan;

   protected override DataSet VirtualUntypedDataSet { get { return ds_kplan; } }

   protected override object[] From_IndexSegmentValues
   {
      get
      {
         switch(recordSorter.SortType)
         {
            case VvSQL.SorterType.Konto/*Code*/: return new object[] { Fld_FromKonto, 0 };
            case VvSQL.SorterType.Name: return new object[] { Fld_FromNaziv, 0 };
            default: ZXC.aim_emsg("Q42: SortType [{0}] undifajnd in property 'From_IndexSegmentValues'", recordSorter.SortType); return null;
         }
      }
   }

   #endregion Overriders and specifics

}

