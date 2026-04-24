using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

public class UserListUC : VvRecLstUC
{
   #region Fieldz

   private RadioButton rBtSortByUserName, rBtSortByPrezime,
                       rBtcurrChecked;
   private VvTextBox   tbx_userName, tbx_prezime;
   private User        user_rec;

   #endregion Fieldz

   #region Constructor

   public UserListUC(Control parent, User _user, VvSubModul vvSubModul) : base(parent)
   {
      this.user_rec = _user;

      this.MasterSubModulEnum = ZXC.VvSubModulEnum.USR;
      this.TheSubModul        = vvSubModul;
      this.Parent.Text        = this.TheSubModul.subModul_name;
   }

   #endregion Constructor

   #region InitializeFindForm

   protected override void InitializeFindFormSpecifics()
   {
      recordSorter = User.sorterUserName;

      this.ds_user = new Vektor.DataLayer.DS_FindRecord.DS_findUser();

      this.Name = "UserListUC";
      this.Text = "UserListUC";
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

      colWidth = ZXC.Q5un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly       (TheGrid, "UserName"   , colWidth, false   , "userName");
      colWidth = ZXC.Q5un; sumOfColWidth += colWidth; AddDGVColum_RecID_4GridReadOnly_Visible(TheGrid, "OznOp/RecId", colWidth, true,  3, "recID"   );
      colWidth = ZXC.Q7un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly       (TheGrid, "Prezime"    , colWidth, false   , "prezime" );
      colWidth = ZXC.Q5un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly       (TheGrid, "Ime"        , colWidth, false   , "ime"     );
      colWidth = ZXC.Q5un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly       (TheGrid, "OIB"        , colWidth, false   , "oib"     );
      colWidth = ZXC.Q7un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly       (TheGrid, "Password", colWidth, true, "password");

      grid_Width = sumOfColWidth + ZXC.QUN;
    }

   #endregion DataGridView

   #region Hamper

   protected override void CreateHamperSpecifikum()
   {
      hampSpecifikum = new VvHamper(2, 2, "", this, true, hampListaRastePada.Right + ZXC.Qun4, nextY, razmakHamp);

      hampSpecifikum.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q5un };
      hampSpecifikum.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hampSpecifikum.VvRightMargin = hampSpecifikum.VvLeftMargin;

      hampSpecifikum.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN };
      hampSpecifikum.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hampSpecifikum.VvBottomMargin = hampSpecifikum.VvTopMargin;

      rBtSortByUserName = hampSpecifikum.CreateVvRadioButton(0, 0, new EventHandler(radioButtonSortByUserName_Click), "Od user name:", TextImageRelation.ImageAboveText);
      rBtSortByUserName.Checked = true;
      rBtcurrChecked = rBtSortByUserName;

      tbx_userName = hampSpecifikum.CreateVvTextBox(1, 0, "tbx_userName", "Od user name");
      this.ControlForInitialFocus = tbx_userName;
      tbx_userName.DoubleClick += new EventHandler(tbx_DoubleClick);
      tbx_userName.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

      tbx_userName.Tag      = rBtSortByUserName;
      rBtSortByUserName.Tag = tbx_userName;

      rBtSortByPrezime = hampSpecifikum.CreateVvRadioButton(0, 1, new EventHandler(radioButtonSortByPrezime_Click), "Od prezimena:", TextImageRelation.ImageAboveText);
      tbx_prezime      = hampSpecifikum.CreateVvTextBox    (1, 1, "tbx_prezime", "Od prezimena");
      tbx_prezime.DoubleClick += new EventHandler(tbx_DoubleClick);
      tbx_prezime.TextChanged += new EventHandler(FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action);

      tbx_prezime.Tag      = rBtSortByPrezime;
      rBtSortByPrezime.Tag = tbx_prezime;

      VvHamper.Open_Close_Fields_ForWriting(tbx_userName, ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_prezime , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
   }

   #endregion Hamper

   #region Eveniti

   private void radioButtonSortByUserName_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = User.sorterUserName;
   }

   private void radioButtonSortByPrezime_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt   = sender as RadioButton;
      rBtcurrChecked    = OpenClose_VvTextBoxOnFindDialog(rbt, rBtcurrChecked);
      this.recordSorter = User.sorterPrezime;
   }

   private void tbx_DoubleClick(object sender, System.EventArgs e)
   {
      VvTextBox vvTb  = sender as VvTextBox;
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

   private string Fld_FromPrezime
   {
      get { return tbx_prezime.Text; }
      set {        tbx_prezime.Text = value; }
   }

   #endregion Fld_

   #region Overriders And Specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.user_rec; }
      set {        this.user_rec = (User)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.UserDao; }
   }

   private Vektor.DataLayer.DS_FindRecord.DS_findUser ds_user;

   protected override DataSet VirtualUntypedDataSet { get { return ds_user; } }

   protected override object[] From_IndexSegmentValues
   {
      get
      {
         switch (recordSorter.SortType)
         {
            case VvSQL.SorterType.Person: return new object[] { Fld_FromPrezime, Fld_FromUserName, 0 };
            case VvSQL.SorterType.Code:   return new object[] { Fld_FromUserName,                  0 };

            default: ZXC.aim_emsg("Q42: SortType [{0}] undifajnd in property 'From_IndexSegmentValues'", recordSorter.SortType); return null;
         }
      }
   }

   #endregion Overriders and specifics

}