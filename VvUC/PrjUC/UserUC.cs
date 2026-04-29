using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using                  MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
using System.Collections.Generic;
#endif

public class UserUC : VvSifrarRecordUC
{
   #region Fieldz

   private VvTextBox tbx_userName, tbx_passwd, tbx_passwd2, tbx_ime, tbx_prezime, tbx_email, tbx_opis, tbx_oib, tbx_recId;
   private int       nextX = 0, nextY = 0, razmakHamp = ZXC.Qun10;
   private VvHamper  hampUser;
   private User      user_rec;
   private Label     lbl_passwd2;
   private CheckBox  cbx_isSuperuser;

   private UserDao.UserCI DB_ci
   {
      get { return ZXC.UsrCI; }
   }
 
   #endregion Fieldz

   #region Constructor

   public UserUC(Control parent, User _user, VvSubModul vvSubModul)
   {
      user_rec         = _user;

      ThePrefferedRecordSorter = VirtualDataRecord.DefaultSorter;

      this.TheSubModul = vvSubModul;
      
      SuspendLayout();

      CreateTabPages(parent);
      InitializeHamperProperties(out hampUser);
      InitializeVvUserControl(parent);
      CreateDataGridView_InitializeTheGrid_ReadOnly_Columns();

      ResumeLayout();
   }

   #endregion Constructor

   #region TabPages

   private void CreateTabPages(Control _parent)
   {
      TheTabControl.TabPages.Add(CreateVvInnerTabPages("Matični", "", ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
     
      VvTabPage vvTabPage = (VvTabPage)(_parent.Parent);

      if(vvTabPage.TabPageKind != ZXC.VvTabPageKindEnum.RECORD_TabPage_INTERACTIVE)
         TheTabControl.TabPages.Add(CreateVvInnerTabPages(prvlg_TabPageName, "", ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage));
   }

   #endregion TabPages
   
   #region hamper
   
   private void InitializeHamperProperties(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 5, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q8un, ZXC.Q4un, ZXC.Q8un, ZXC.Q5un, ZXC.Q10un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Q2un, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,  };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.QUN, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      //Label lbl_userName, lbl_ime, lbl_prezime, lbl_email, lbl_opis, lbl_passwd;

                     hamper.CreateVvLabel  (0, 0, "UserName:", ContentAlignment.MiddleRight);
      tbx_userName = hamper.CreateVvTextBox(1, 0, "tbx_username", "User name", GetDB_ColumnSize(DB_ci.userName));
      tbx_userName.JAM_FieldExitWithValidationMethod += new System.ComponentModel.CancelEventHandler(DisableRootUsername);
    
      cbx_isSuperuser = hamper.CreateVvCheckBox_OLD(1, 1, null, "Superuser", RightToLeft.No );

                   hamper.CreateVvLabel  (0, 2, "Password:", ContentAlignment.MiddleRight);
      tbx_passwd = hamper.CreateVvTextBox(1, 2, "tbx_passwd", "Password", GetDB_ColumnSize(DB_ci.password));
      tbx_passwd.MaxLength = 16; // !!! 
#if(DEBUG)
      // Kada hoces vidjeti userov password, samo komentiraj dolnju liniju. 
      //tbx_passwd.JAM_IsPassword   = true;
#else
      tbx_passwd.JAM_IsPassword = true;
#endif
      tbx_passwd.JAM_DataRequired = true;
      tbx_passwd.JAM_PasswdField_TextChanged_Method = new EventHandler(tbx_passwd_TextChanged);

      lbl_passwd2 = hamper.CreateVvLabel  (0, 3, "Ponovljeni password:", ContentAlignment.MiddleRight);
      tbx_passwd2 = hamper.CreateVvTextBox(1, 3, "tbx_passwd2", "Ponovljeni password", GetDB_ColumnSize(DB_ci.password));
      lbl_passwd2.Tag     = tbx_passwd2.Tag     = "Visible_ only_ZXC.ZaUpis.Otvoreno";
      lbl_passwd2.Visible = tbx_passwd2.Visible = false;
      tbx_passwd2.JAM_IsPassword = true;


                hamper.CreateVvLabel  (2, 0, "Ime:", ContentAlignment.MiddleRight);
      tbx_ime = hamper.CreateVvTextBox(3, 0, "tbx_ime", "Ime", GetDB_ColumnSize(DB_ci.ime));

                    hamper.CreateVvLabel  (2, 1, "Prezime:", ContentAlignment.MiddleRight);
      tbx_prezime = hamper.CreateVvTextBox(3, 1, "tbx_username", "Prezime", GetDB_ColumnSize(DB_ci.prezime));

                  hamper.CreateVvLabel  (2, 2, "E-mail:", ContentAlignment.MiddleRight);
      tbx_email = hamper.CreateVvTextBox(3, 2, "tbx_email", "E-mail", GetDB_ColumnSize(DB_ci.email));

                 hamper.CreateVvLabel  (4, 0, "Opis:", ContentAlignment.MiddleRight);
      tbx_opis = hamper.CreateVvTextBox(5, 0, "tbx_opis", "Opis", GetDB_ColumnSize(DB_ci.opis), 0, 2);
      tbx_opis.Multiline = true;

                hamper.CreateVvLabel  (2, 3, "OIB:", ContentAlignment.MiddleRight);
      tbx_oib = hamper.CreateVvTextBox(3, 3, "tbx_oib", "OIB", /*GetDB_ColumnSize(DB_ci.oib)*/11);
      tbx_oib.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_oib.JAM_FieldExitWithValidationMethod += new System.ComponentModel.CancelEventHandler(CheckOIB_Field);

                  hamper.CreateVvLabel  (4, 3, "OznOperatera:", ContentAlignment.MiddleRight);
      tbx_recId = hamper.CreateVvTextBox(5, 3, "tbx_opis"     , "Oznaka operatera", 3);
      tbx_recId.JAM_ReadOnly = true;

      this.ControlForInitialFocus = tbx_userName;

      this.Validating += new System.ComponentModel.CancelEventHandler(UserUC_Validating);
   }
  
   #endregion hamper

   #region DataGridView

   private void CreateDataGridView_InitializeTheGrid_ReadOnly_Columns()
   {
      aTransesGrid[0]      = CreateDataGridView_ReadOnly(TheTabControl.TabPages[prvlg_TabPageName], "Privilegije_User");
      aTransesGrid[0].Dock = DockStyle.Fill;
      int minGridWIdth     = InitializeTheGrid_ReadOnly_Columns();

      aTransesGrid[0].DoubleClick += new EventHandler(theFIRST_TransGrid_DoubleClick);
      aTransesGrid[0].KeyPress += new KeyPressEventHandler(theFIRST_TransGrid_KeyPress);
   }
   
   protected override void theFIRST_TransGrid_DoubleClick(object sender, EventArgs e)
   {
      base.OpenNew_Record_TabPage_OnDoubleClick(ZXC.VvSubModulEnum.PRV, SelectedRecIDIn_FIRST_TransGrid);
   }

   private int InitializeTheGrid_ReadOnly_Columns()
   {
      int sumOfColWidth = 0, colWidth;

      sumOfColWidth += aTransesGrid[0].RowHeadersWidth;
      colWidth = ZXC.Q4un;                            AddDGVColum_RecID_4GridReadOnly  (aTransesGrid[0], "RecID"            , colWidth, true, 6);
      colWidth = ZXC.Q4un; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly(aTransesGrid[0], "Projekt ID"       , colWidth, true, 6);
      colWidth = ZXC.Q4un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly (aTransesGrid[0], "ProjTicker"       , colWidth, false);
      colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly (aTransesGrid[0], ""                 , colWidth, false);
      colWidth = ZXC.Q7un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly (aTransesGrid[0], "Privilegija"      , colWidth, false);
      colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly (aTransesGrid[0], ""                 , colWidth, false);
      colWidth = ZXC.Q7un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly (aTransesGrid[0], "Doseg privilegije", colWidth, true);
      colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly (aTransesGrid[0], ""                 , colWidth, false);
      colWidth = ZXC.Q7un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly (aTransesGrid[0], "Dokument"         , colWidth, false);

      return sumOfColWidth;
   }

   #endregion DataGridView

   #region Filter

   public override void CreateRptFilterAndRptFilterUC()
   {
      ThePrvlgFilter = new VvRpt_Prvlg_Filter();

      ThePrvlgFilterUC        = new PrvlgFilterUC(this);
      ThePrvlgFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = ThePrvlgFilterUC.Width;
   }

   #endregion Filter

   #region UserUC_Validating

   void UserUC_Validating(object sender, System.ComponentModel.CancelEventArgs e)
   {
      if(tbx_passwd2.Visible == false) return;

      if(Fld_Passwd != Fld_Passwd2)
      {
         ZXC.RaiseErrorProvider(tbx_passwd2, "Passwordi se ne podudaraju!");
         e.Cancel = true;
      }
      else Fld_Passwd2 = "";
   }

   void tbx_passwd_TextChanged(object sender, EventArgs e)
   {
      if(lbl_passwd2.Visible == true && tbx_passwd2.Visible == true) return;

      lbl_passwd2.Visible = tbx_passwd2.Visible = true;
      Fld_Passwd2         = "";
   }

   #endregion UserUC_Validating

   #region DisableRootUsername

   void DisableRootUsername(object sender, System.ComponentModel.CancelEventArgs e)
   {
      if(tbx_userName.Text == ZXC.vvDB_systemSuperUserName /*||
         tbx_userName.Text == ZXC.vvDB_programSuperUserName*/)
      {
         ZXC.RaiseErrorProvider((Control)sender, "Nedozvoljeni UserName.");
         e.Cancel = true;
      }
   }

   #endregion DisableRootUsername

   #region PutFields(), GetFields()

   #region Fld_XY

   public string Fld_UserName { get { return tbx_userName   .Text;   }  set { tbx_userName   .Text    = value; } }
   public string Fld_Passwd   { get { return tbx_passwd     .Text;   }  set { tbx_passwd     .Text    = value; } }
   public string Fld_Passwd2  { get { return tbx_passwd2    .Text;   }  set { tbx_passwd2    .Text    = value; } }
   public string Fld_Ime      { get { return tbx_ime        .Text;   }  set { tbx_ime        .Text    = value; } }
   public string Fld_Prezime  { get { return tbx_prezime    .Text;   }  set { tbx_prezime    .Text    = value; } }
   public string Fld_Email    { get { return tbx_email      .Text;   }  set { tbx_email      .Text    = value; } }
   public string Fld_Opis     { get { return tbx_opis       .Text;   }  set { tbx_opis       .Text    = value; } }
   public bool   Fld_isSuper  { get { return cbx_isSuperuser.Checked;}  set { cbx_isSuperuser.Checked = value; } }
   public string Fld_Oib      { get { return tbx_oib        .Text;   }  set { tbx_oib        .Text    = value; } }
   public string Fld_RecId    {                                         set { tbx_recId      .Text    = value; } }

   #endregion Fld_XY

   #region Classic PutFileds(), GetFields()

   public override void PutFields(VvDataRecord user)
   {
      user_rec = (User)user;
 
      if (user_rec != null)
      {
         PutMetaFileds(user_rec.AddUID, user_rec.AddTS, user_rec.ModUID, user_rec.ModTS, user_rec.RecID, user_rec.LanSrvID, user_rec.LanRecID);
         PutIdentityFields(user_rec.UserName, user_rec.Ime, user_rec.Prezime, "");

         Fld_UserName = user_rec.UserName;
         Fld_Passwd   = user_rec.PasswdDecrypted;
         Fld_Ime      = user_rec.Ime;
         Fld_Prezime  = user_rec.Prezime;
         Fld_Email    = user_rec.Email;
         Fld_Opis     = user_rec.Opis;
         Fld_isSuper  = user_rec.IsSuper;
         Fld_Oib      = user_rec.Oib;
         Fld_RecId    = user_rec.RecID.ToString("000");

         InitializeFilterUCFields();

         recordReportLoaded = false;
         DecideIfShouldLoad_VvReport(null, null, null);

         aTransesLoaded[0] = false; // ovdje treba nulirati sve postojece 'xyLoaded' varijable
         DecideIfShouldLoad_TransDGV(null, null, null);
      }
   }

   public override void GetFields(bool fuse)
   {
      if(user_rec == null) user_rec = new User();

      user_rec.UserName = Fld_UserName;

      user_rec.PasswdEncodedAsInFile = ZXC.EncryptThis_UserUC_Password(Fld_Passwd, Fld_UserName);

      if(user_rec.PasswdEncodedAsInFile.Length > GetDB_ColumnSize(DB_ci.password))
      {
         ZXC.RaiseErrorProvider(tbx_passwd, "Password PREDUGACAK!");
         user_rec.PasswdEncodedAsInFile = Fld_Passwd = "";
      }


      user_rec.Ime     = Fld_Ime;
      user_rec.Prezime = Fld_Prezime;
      user_rec.Email   = Fld_Email;
      user_rec.Opis    = Fld_Opis;
      user_rec.IsSuper = Fld_isSuper;
      user_rec.Oib     = Fld_Oib ;

   }

   #endregion Classic PutFileds(), GetFields()

   #region Put Trans DGV Fileds

   private const string prvlg_TabPageName = "Privilegije";

   private void InitializeFilterUCFields()
   {
      SetFilterRecordDependentDefaults(); // filter.userName = user_rec.UserName (punimo bussiness od filtera, ne UC)
      ThePrvlgFilterUC.PutFilterFields(ThePrvlgFilter);
   }

   public override void DecideIfShouldLoad_TransDGV(VvInnerTabControl sender, VvInnerTabPage oldPage, VvInnerTabPage newPage)
   {
      ZXC.VvInnerTabPageKindEnum innerTabPageKind = ((VvInnerTabPage)TheTabControl.SelectedTab).TheInnerTabPageKindEnum;

      if(aTransesLoaded[0] == false &&
         innerTabPageKind == ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage)
      {
         LoadRecordList_AND_PutTransDgvFields();
      }
   }

   public override void LoadRecordList_AND_PutTransDgvFields()
   {
      int rowIdx, idxCorrector;
      VvLookUpLista documNamesList;

      ThePrvlgFilterUC.GetFilterFields();
      ThePrvlgFilterUC.AddFilterMemberz(ThePrvlgFilter, null);

      if(user_rec.Privileges == null) user_rec.Privileges = new List<Prvlg>();
      else                            user_rec.Privileges.Clear();

      VvDaoBase.LoadGenericVvDataRecordList<Prvlg>(TheDbConnection, user_rec.Privileges, ThePrvlgFilter.FilterMembers, "prjktTick, recID");

      aTransesLoaded[0] = true;

      aTransesGrid[0].Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(aTransesGrid[0]);

      foreach(Prvlg prvlg_rec in user_rec.Privileges)
      {
         aTransesGrid[0].Rows.Add();

         rowIdx = aTransesGrid[0].RowCount - idxCorrector;

         aTransesGrid[0][0, rowIdx].Value = prvlg_rec.RecID;
         aTransesGrid[0][1, rowIdx].Value = prvlg_rec.PrjktID;
         aTransesGrid[0][2, rowIdx].Value = prvlg_rec.PrjktTick;
         aTransesGrid[0][3, rowIdx].Value = prvlg_rec.PrvlgType;
         aTransesGrid[0][4, rowIdx].Value = ZXC.luiListaPrvlgType.GetNameForThisCd(prvlg_rec.PrvlgType);
         aTransesGrid[0][5, rowIdx].Value = prvlg_rec.PrvlgScope;
         aTransesGrid[0][6, rowIdx].Value = ZXC.luiListaPrvlgScope.GetNameForThisCd(prvlg_rec.PrvlgScope);
         aTransesGrid[0][7, rowIdx].Value = prvlg_rec.DocumType;

         if(PrvlgUC.IsThis_PrvlgScopeCd_OneDocumScope(prvlg_rec.PrvlgScope))
         {
            documNamesList = ZXC.TheVvForm.DocumentTypeLookUpListaForThisModulEnum(PrvlgUC.ModulFromPrvlgScopeCd_asList(prvlg_rec.PrvlgScope));
            aTransesGrid[0][8, rowIdx].Value = documNamesList.GetNameForThisCd(prvlg_rec.DocumType);
         }
         else
         {
            aTransesGrid[0][8, rowIdx].Value = "";
         }
         aTransesGrid[0].Rows[rowIdx].HeaderCell.Value = (user_rec.Privileges.Count - rowIdx).ToString();

      }
   }
   
   #endregion Put Trans DGV Fileds

   #endregion PutFields(), GetFields()

   #region  Overriders And Specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.user_rec; }
      set { this.user_rec = (User)value; }
   }

   public override VvSifrarRecord VirtualSifrarRecord
   {
      get { return this.VirtualDataRecord as VvSifrarRecord; }
      set {        this.VirtualDataRecord = (User)value;     }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.UserDao; }
   }

   #region VvFindDialog

   public override VvFindDialog CreateVvFindDialog()
   {
      return CreateFind_User_Dialog();
   }

   public static VvFindDialog CreateFind_User_Dialog()
   {
      VvSubModul vvSubModul   = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsUSR);
      VvDataRecord      vvDataRecord = ZXC.TheVvForm.CreateTheVvDataRecord_SwitchSubModulEnum(vvSubModul);

      VvFindDialog vvFindDialog = new VvFindDialog();
      VvRecLstUC   vvRecListUC  = new UserListUC(vvFindDialog, (User)vvDataRecord, vvSubModul);
      vvFindDialog.TheRecListUC = vvRecListUC;

      return vvFindDialog;
   }

   #endregion VvFindDialog

   #region PrintSifrarRecord

   public VvRpt_Prvlg_Filter ThePrvlgFilter{ get; set;}

   public PrvlgFilterUC ThePrvlgFilterUC { get; set; }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.ThePrvlgFilter;
      }
   }
   
   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.ThePrvlgFilterUC;
      }
   }
   
   public override void SetFilterRecordDependentDefaults()
   {
      this.ThePrvlgFilter.UserName = user_rec.UserName;
   }

/* report
   public RptF_KKonta TheRptF_KKonta { get; set; }

   public override VvReport VirtualReport
   {
      get
      {
         return this.TheRptF_KKonta;
      }
   }

   public override string VirtualReportName
   {
      get
      {
         return "KARTICA KONTA";
      }
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      return new RptF_KKonta(reportName, (VvRpt_Fin_Filter)vvRptFilter);
   }
   */


   #endregion PrintSifrarRecord
  
   #region Update_VvDataRecord (Legacy naming convention)

   /// <summary>
   /// 'FindVvDataRecord' procedura. Inicirana:
   /// 1. Context menu (Mouse right click)
   /// 2. Mouse click (Ctrl ili Alt click)
   /// 3. Keyboard initiated (Ctrl/Alt + F/Space)
   /// </summary>
   /// <param name="startValue"></param>
   /// <returns></returns>
   public static string Update_User(object startValue)
   {
      User user_rec = new User();
      UserListUC userListUC;
      XSqlConnection dbConnection = ZXC.TheVvForm.TheDbConnection;

      VvFindDialog dlg = CreateFind_User_Dialog();

      userListUC = (UserListUC)(dlg.TheRecListUC);


      userListUC.Fld_FromUserName = startValue.ToString();

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         if(!ZXC.UserDao.SetMe_Record_byRecID(dbConnection, user_rec, (uint)dlg.SelectedRecID, false)) return null;
      }
      else
      {
         user_rec = null;
      }

      if(dlg.SelectionIsNewlyAddedRecord == true) ZXC.ShouldForceSifrarRefreshing = true;

      dlg.Dispose();

      if(user_rec != null) return user_rec.UserName;
      else return null;
   }

   #endregion Update_VvDataRecord (Legacy naming convention)

   public override Size ThisUcSize
   {
      get
      {
         return new Size(hampUser.Right + ZXC.QunMrgn, hampUser.Bottom + ZXC.QunMrgn);
      }
   }

   #region PutNew_Sifra_Field

   public override void PutNew_Sifra_Field(uint newSifra)
   {
      // dummy for UserUC 
   }

   #endregion PutNew_Sifra_Field

   #endregion  Overriders And Specifics

}