using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Deployment.Application;
using System.Net.Mail;
using System.Data.OleDb;
using System.Linq;
using com.handpoint.api;
using System.Numerics;
using Newtonsoft.Json;
using CrystalDecisions.CrystalReports.Engine;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;


#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlErrorCode  = MySql.Data.MySqlClient.MySqlErrorCode;
using XSqlException  = MySql.Data.MySqlClient.MySqlException;
using Vektor.Reports.PIZ;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
#endif

public /*sealed*/ partial class VvForm : DevExpress.XtraEditors.XtraForm, Events.Required, Events.Status
{
   #region Test Something

   private void TestSomethingOld2()
   {
      string directoryPath = @"E:\0_DOWNLOAD\VIPER_Izvodi08"                      ;
      string fName         = directoryPath + @"\" + "00885193000_045_20080331.txt";

      //string[] lines = VvImpExp.GetFileLinesAsStringArray(@"E:\0_DOWNLOAD\VIPER_Izvodi08\00885193000_045_20080331.txt");

      //ZapIzvod zi = new ZapIzvod(fName);

      //if(zi.BadData) zi = null;

      //uint izvodNum = ZapIzvod.GetIzvodNumFromFile(fName);

      //string[] fNames = VvImpExp.GetAllFileNamesInDirectory(directoryPath);

      uint izvodNumWeWant = TheVvDao.GetNextTtNum(TheDbConnection, "IZ", null);

      string suggestedIzvodFileName = ZapIzvod.GetIzvodFileName_ContainingIzvodNumWeWant(directoryPath, izvodNumWeWant);

      ZXC.aim_emsg("Suggested Izvod FileName would be\n\n{0}", suggestedIzvodFileName);
   }

   static bool RwtNalogTtNum(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      bool OK = true;
      Nalog nalog_rec = vvDataRecord as Nalog;

      nalog_rec.BeginEdit();

      nalog_rec.TtNum = nalog_rec.VvDao.GetNextTtNum(ZXC.TheVvForm.TheDbConnection, nalog_rec.TT, null);

      Console.WriteLine("[{0,6}] [{1}] [{2}]", nalog_rec.DokNum, nalog_rec.TT, nalog_rec.TtNum);

      OK = vvDataRecord.VvDao.RWTREC(ZXC.TheVvForm.TheDbConnection, nalog_rec, false, true);

      nalog_rec.EndEdit();

      return OK;
   }

   #endregion Test Something

   #region The's

   //================================== The's ===============

   /// <summary>
   /// Current, selected TAB
   /// </summary>
   public VvTabPage TheVvTabPage
   {
      get 
      {
         IVvDocumentHost activeHost = ZXC.LastActiveDocumentHost as IVvDocumentHost;
         if(activeHost != null && !ReferenceEquals(activeHost, this) && activeHost.ActiveTabPage != null) return activeHost.ActiveTabPage;

         return ActiveTabPage; 
      }
   }

   /// <summary>
   /// Generic, current VvUserControl (moze biti VvRecordUC ili VvReportUC
   /// </summary>
   public VvUserControl TheVvUC
   {
      get
      {
         IVvDocumentHost activeHost = ZXC.LastActiveDocumentHost as IVvDocumentHost;
         if(activeHost != null && !ReferenceEquals(activeHost, this) && activeHost.ActiveUserControl != null) return activeHost.ActiveUserControl;

         return ActiveUserControl;
      }
   }

   /// <summary>
   /// Current VvUserControl as IVvPrintableUC ili
   /// Neka druga Form-a (npr PrintPreviewIzvod)
   /// </summary>
   public IVvPrintableUC TheVvPrintableUC
   {
      get 
      {
         // Ovdje ce ti se zavrtiti u glavi kada budes ponovno probao naci rjesenje kako ovo natjerati da radi 
         // i sa FindText na ReportViewer-u koji je na ZapIzvodPreview-ju!  

         if(Form.ActiveForm is VvForm || 
            Form.ActiveForm == null   ||
            Form.ActiveForm is VvReportFindPageOrTextDlg) return(ZXC.TheVvForm.TheVvTabPage.TheVvPrintableUC);
         else                                             return(VvUserControl.GetIVvPrintableUC(Form.ActiveForm)); // npr ZapIzvodPreview 
      }
   }

   /// <summary>
   /// Current VvUserControl as VvRecordUC
   /// </summary>
   public VvRecordUC TheVvRecordUC
   {
      get { return (TheVvTabPage.TheVvRecordUC); }
   }

   public VvRecordUC ActiveRecordUC
   {
      get
      {
         VvTabPage activeTabPage = TheVvTabPage;
         return activeTabPage != null ? activeTabPage.TheVvUC as VvRecordUC : null;
      }
   }

   /// <summary>
   /// Current VvUserControl as VvDocumentRecordUC
   /// </summary>
   public VvDocumentRecordUC TheVvDocumentRecordUC
   {
      get { return (TheVvTabPage.TheVvDocumentRecordUC); }
   }

   public VvDocumentRecordUC ActiveDocumentRecordUC
   {
      get
      {
         VvTabPage activeTabPage = TheVvTabPage;
         return activeTabPage != null ? activeTabPage.TheVvUC as VvDocumentRecordUC : null;
      }
   }

   /// <summary>
   /// Current VvUserControl as VvPolyDocumRecordUC
   /// </summary>
   public VvPolyDocumRecordUC TheVvPolyDocumRecordUC
   {
      get { return (TheVvTabPage.TheVvPolyDocumRecordUC); }
   }

   /// <summary>
   /// Current VvUserControl as VvDocumLikeRecordUC
   /// </summary>
   public VvDocumLikeRecordUC TheVvDocumentLikeRecordUC
   {
      get { return (TheVvTabPage.TheVvDocumentLikeRecordUC); }
   }

   /// <summary>
   /// Current VvUserControl as VvSifrarRecordUC
   /// </summary>
   public VvSifrarRecordUC TheVvSifrarRecordUC
   {
      get { return (TheVvTabPage.TheVvSifrarRecordUC); }
   }

   /// <summary>
   /// get { return (TheVvRecordUC.ucList); }
   /// set {         TheVvRecordUC.ucList = value; }
   /// </summary>
   public VvSQL.VvLockerInfo TheVvLockerInfo
   {
      get { return (TheVvRecordUC.lockerInfo); }
      set {         TheVvRecordUC.lockerInfo = value; }
   }
   
   /// <summary>
   /// Current VvUserControl as VvReportUC
   /// </summary>
   public VvReportUC TheVvReportUC
   {
      get { return (TheVvTabPage.TheVvReportUC); }
   }

   public RiskReportUC TheVvPDFreporterReportUC { get; set; }

   /// <summary>
   /// Current VvUserControl as VvRecLstUC
   /// </summary>
   public VvRecLstUC TheVvRecLstUC
   {
      get { return (TheVvTabPage.TheVvRecLstUC); }
   }

   /// <summary>
   /// Current VvDataRecord
   /// </summary>
   public VvDataRecord TheVvDataRecord
   {
      get { return ((IVvRecordAssignableUC)TheVvUC).VirtualDataRecord; }
      set {        ((IVvRecordAssignableUC)TheVvUC).VirtualDataRecord = value; }
   }

   /// <summary>
   /// Curent FilterPanel on VvReportUC
   /// </summary>
   public Panel TheVvFilterPanel_OnReportUC
   {
      get { return ((VvReportUC)TheVvUC).TheFilterPanel; }
      set {        ((VvReportUC)TheVvUC).TheFilterPanel = value; }
   }

   /// <summary>
   /// Curent FilterPanel on VvRecordUC
   /// </summary>
   public Panel TheVvFilterPanel_OnRecordUC
   {
      get { return ((VvRecordUC)TheVvUC).ThePanelForFilterUC_PrintTemplateUC; }
      set { ((VvRecordUC)TheVvUC).ThePanelForFilterUC_PrintTemplateUC = value; }
   }

   /// <summary>
   /// VvDataAccessObject for current VvDataRecord
   /// </summary>
   public IVvDao TheVvDao
   {
      get { return TheVvDataRecord.VvDao; }
   }

   /// <summary>
   /// Current VvTabPage's MySqlConnection
   /// </summary>
   public XSqlConnection TheDbConnection
   {
      // 02.01.2015: 
    //get { return                                                  TheVvTabPage.TheDbConnection; }
      get { return TheVvTabPage == null ? ZXC.TheMainDbConnection : TheVvTabPage.TheDbConnection; }
   }

   /// <summary>
   /// Glavni tab kontejner na VvFormu. Per V4 §2.1 + §2.2 #1 (post-C20b):
   /// `DocumentManager` + `TabbedView` od Faze 2 odmah (NE XtraTabControl).
   /// `workDocumentManager` (deklariran u zVvForm\VvForm.cs Fieldz region) drži
   /// DocumentManager komponentu; ovaj getter vraća njen `View as TabbedView`.
   /// AllowFloating je u Fazi 2 isključen — tek u Fazi 3 (DETACH) se uključuje.
   /// </summary>
   public TabbedView TheTabControl
   {
      get 
      {
         if(workDocumentManager == null) return null;
         return                            workDocumentManager.View as TabbedView; 
      }
      // Setter zadržan ali no-op: TabbedView se kreira interno u InitializeWorkTabControl();
      // nijedan postojeći call-site ne assigna TheTabControl izvana.
      set { /* no-op u DX modelu */ }
   }

   /// <summary>
   /// Current VvTabPage's Is Arhiva Is NOT Arhiva podatak
   /// </summary>
   public bool IsArhivaTabPage
   {
      get { return TheVvTabPage.IsArhivaTabPage; }
   }

   public VvDataRecord TheArhivedVvDataRecord { get; set; }

   public List<VvTransRecord> TheArhivedVvTransRecords;

   public uint RecID_BeforeArhivaEntered { get; set; }

   public ApplicationDeployment TheVvApplicationDeployment
   {
      get
      {
         if(ApplicationDeployment.IsNetworkDeployed == false) return null;

         ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

         return ad;
      }
   }

   public bool TheVvApplicationDeployment_IsFirstRun
   {
      get
      {
         return TheVvApplicationDeployment == null ? false : TheVvApplicationDeployment.IsFirstRun;
      }
   }

   public bool VvFlag_AllowGridAddOrDeleteRowsIsChanging  { get; set; }
   public bool VvFlag_PretendDgvCurrentCellIsInEditMode   { get; set; }
   public bool VvFlag_RowsAreAddingOrDeletingOrBoth       { get; set; }

   public bool PutFieldsInProgress { get; set; }

   Hapi TheHapi { get; set; }

   Device TheM2PayDevice { get; set; }

   //================================== The's ===============

   #endregion The's

   #region GetLoginData()

   #region ShowProgressForm

   protected Form ProgressForm { get; set; }

   public Form CreateWaitingForConnectionForm(/*Icon icon*/)
   {
      Form progressForm = new Form();
    //progressForm.StartPosition = FormStartPosition.CenterScreen;
      progressForm.Location = new Point(606, 606);
      progressForm.Size = new Size(400, 400);
      //progressForm.ClientSize = new Size(400, 400);
      progressForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      progressForm.ControlBox = false;
      progressForm.Text = "Molim pričekajte. Konektiranje na bazu podataka u tijeku.";
      //progressForm.BackgroundImage = new Icon(this.Icon, 256, 256).ToBitmap();
      progressForm.BackgroundImage = new Bitmap(GetManifestResourceStream("Vektor.Icons.app_intro3_400.png"));
      progressForm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      progressForm.Opacity = /*0.85*/1.00;
      progressForm.Icon = this.Icon;

      ProgressBar progBar = new ProgressBar();
      progBar.Parent = progressForm;
      progBar.Location = new Point(10, 10);
      progBar.MarqueeAnimationSpeed = 50;
      progBar.Maximum = 300;
      progBar.Name = "progBar";
      progBar.Size = new Size(progressForm.ClientSize.Width - progBar.Location.X, 20);
      progBar.Step = 5;
      progBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
      progBar.Value = 100;

      Cursor.Current = Cursors.WaitCursor;

      return progressForm;
   }

   public void ShowProgressForm_BackgroundWorker()
   {
      BackgroundWorker pfBW = new BackgroundWorker();

      pfBW.WorkerReportsProgress = false;
      pfBW.WorkerSupportsCancellation = false;

      pfBW.DoWork += new DoWorkEventHandler(pfBW_DoWork);
      pfBW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(pfBW_RunWorkerCompleted);

      pfBW.RunWorkerAsync(this);

   }

   private void pfBW_DoWork(object sender, DoWorkEventArgs e)
   {
      // This method will run on a thread other than the UI thread.
      // Be sure not to manipulate any Windows Forms controls created
      // on the UI thread from this method.
      BackgroundWorker worker = sender as BackgroundWorker;
      VvForm theVvForm = (VvForm)e.Argument;

      ProgressForm.ShowDialog();

   }

   private void pfBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
   {
      // Handle the case where an exception was thrown.
      if(e.Error != null)
      {
         MessageBox.Show(e.Error.Message);
      }
      else if(e.Cancelled)
      {
         MessageBox.Show("CANCELED");
      }
      else
      {
         //MessageBox.Show("FINISHED");
      }
   }

   private delegate void CloseForm_CallBack  (Form theForm);
   public static    void CloseForm_ThreadSafe(Form theForm)
   {
      if(theForm.InvokeRequired) theForm.Invoke(new CloseForm_CallBack(CloseForm_ThreadSafe), new object[] { theForm });
      else                       theForm.Close();
   }

   private delegate void FormVisible_CallBack  (Form theForm, bool visible);
   public static    void SetFormVisibility_ThreadSafe(Form theForm, bool visible)
   {
      if(theForm.InvokeRequired) theForm.Invoke(new FormVisible_CallBack(SetFormVisibility_ThreadSafe), new object[] { theForm, visible });
      else                       theForm.Visible = visible;
   }

   #endregion ShowProgressForm

   internal virtual void GetLoginData_ExecuteSomething_ThenExit()
   {
      bool OK, OK2 = true;

      ///* ZDRAVKOC */if(ProgressForm.Visible) SetFormVisibility_ThreadSafe(ProgressForm, false);

      Cursor.Current = Cursors.WaitCursor;

      //progressForm.Show/*Dialog*/(this);

      ZXC.vvDB_Server   = ZXC.MainArgs[0];                                                 // loginForm.Fld_ServerHost; 
      ZXC.vvDB_VvDomena = ZXC.MainArgs[1].ToLower() == "empty" ? "" : ZXC.MainArgs[1];     // loginForm.Fld_VvDomena  ; 
      ZXC.vvDB_User     = ZXC.MainArgs[2];                                                 // loginForm.Fld_UserName  ; 
      ZXC.vvDB_Password = ZXC.EncryptThis_UserUC_Password(ZXC.MainArgs[3], ZXC.vvDB_User); // ZXC.EncryptThis_UserUC_Password(loginForm.Fld_Password, loginForm.Fld_UserName);

      ZXC.projectYear         = ZXC.MainArgs[4]; // loginForm.Fld_ProjectYear.ToString(ZXC.VvDateYyyyFormat);
      ZXC.projectYearFirstDay = new DateTime(int.Parse(ZXC.projectYear), 1, 1);
      ZXC.projectYearLastDay  = new DateTime(int.Parse(ZXC.projectYear), 12, 31, 23, 59, 59);
      ZXC.prevYearLastDay     = ZXC.projectYearFirstDay - ZXC.OneDaySpan;
      ZXC.prevYearDecembar    = new DateTime(int.Parse(ZXC.projectYear) - 1, 12, 1);

      ZXC.nextYearFirstDay    = (ZXC.projectYearLastDay + ZXC.OneDaySpan).Date;

      ZXC.initialPrjktKCD     = ZXC.ValOrZero_UInt(ZXC.MainArgs[5]); // loginForm.Fld_UseThisPrjkt;

      VvPref.login.initialPrjktKCD = ZXC.initialPrjktKCD;

      OK = VvSQL.TestAnd_CREATE_ThePrjktDB_Connection_4ZXC();

      
      if(!OK && ZXC.ThisIsSkyLabProject) System.Environment.Exit(0);


      // 03.07.2014: 
      if(OK) ZXC.vvDB_ServerID = VvSQL.Get_Sql_ServerID(ZXC.TheMainDbConnection);

      if(OK) OK2 = VvSQL.CheckDatabase(ZXC.TheMainDbConnection, GetvvDB_prjktDB_name(), false);

      if(!OK2)
      {
         //Application.Exit();
         System.Environment.Exit(0);
      }

      if(OK)
      {
         ZXC.VvDataBaseInfo? vvDBinfoOfMainPrjkt = VvSQL.GetVvDataBaseInfoForKupdobCD(ZXC.PrjConnection, ZXC.projectYear, ZXC.initialPrjktKCD);

         if(vvDBinfoOfMainPrjkt != null)
         {
            ZXC.initialPrjktTicker = ((ZXC.VvDataBaseInfo)vvDBinfoOfMainPrjkt).ProjectName;

            VvPref.login.initialPrjktTicker = ZXC.initialPrjktTicker;
         }
         else // TODO: mainPrjkt database doesn't exists! Ask for ticker and create database
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Za šifru [{0}] ne postoji DataBase!\n\nZadajte šifru projekta koji postoji u zadanoj projektnoj godini [{1}].", ZXC.initialPrjktKCD, ZXC.projectYear);
            //Application.Exit();
            //System.Environment.Exit(0);
            OK = false;

            // 09.04.2014: start ____________ 
            if(tsCbxVvDataBase == null) tsCbxVvDataBase = new ToolStripComboBox();
            // 01.01.2022:
            //ZXC.lowestPrjktCdDBI = RefreshDataBasesInfo();
            if(ZXC.ThisIsSkyLabProject) ZXC.lowestPrjktCdDBI = new ZXC.VvDataBaseInfo(ZXC.projectYear, "TEXTHO", 69);
            else                        ZXC.lowestPrjktCdDBI = RefreshDataBasesInfo();
            loginForm.rbt_lastUsedPrjkt.Checked = false;
            loginForm.rbt_useThisPrjkt.Checked = true;
            loginForm.Fld_UseThisPrjkt = ZXC.lowestPrjktCdDBI.ProjectKcdAsUInt;
            // 09.04.2014: end ____________ 
         }
      }

      if(!OK) loginForm.VvPerformOkButtonClick = false;

   }

   protected void GetLoginData(/*bool forceLoginForm*/)
   {
      // 12.01.2015: 
      if(ZXC.MainArgs.Length >= 6)
      {
         //ZXC.aim_emsg("{0} {1} {2} {3} {4} {5} {6}", ZXC.MainArgs[0], ZXC.MainArgs[1], ZXC.MainArgs[2], ZXC.MainArgs[3], ZXC.MainArgs[4], ZXC.MainArgs[5], ZXC.MainArgs[6]);
         GetLoginData_ExecuteSomething_ThenExit();
         return;
      }

      // 03.05.2017: if SkyLab or Janitor 
      // AND NO arguments ... exit!       
      if((ZXC.ThisIsSkyLabProject || ZXC.ThisIsJanitorProject) &&
          ZXC.MainArgs.Length.IsZero())
      {
         System.Windows.Forms.MessageBox.Show("Taskscheduler only.", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
         System.Environment.Exit(0);
      }

      bool OK, OK2 = true;

      loginForm = new LoginForm(false);

#if(DEBUG)

      //// TODO: !!!!!!!!!!!!!!!! DELLME LATTER: 
      //DateTime Date31122014 = new DateTime(2014, 12, 31);
      //for(DateTime currDate = ZXC.THcycle_ZeroDate; currDate <= Date31122014; currDate += ZXC.OneDaySpan)
      //{
      //   ZXC.TH_CycleMoment theMoment = ZXC.GetTH_CycleMoment(currDate, "14M5");
      //}

    //if(forceLoginForm == false)
    //{
    //   //loginForm.VvPerformOkButtonClick = true;
    //}

      // stavi ovo u watch debuggera kada zelis saznati user login password: 
      // "Jnv0PFH4VrpLl06/ppIyjg==" npr daje "qweqwe"
      // Dakle zamijeni "Jnv0PFH4VrpLl06/ppIyjg==" sa onim sto ti da QueryBrowser sa copy field content kolone password u User tablici 
      // VvAES.DecryptData("Jnv0PFH4VrpLl06/ppIyjg==", ZXC.vv_User_AES_key)
      //VvAES.DecryptData("dY3gG2+aPvymdiKI648JNA==", ZXC.vv_User_AES_key);



      //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

      //ZXC.aim_emsg(" [{0}] [{1}]", Application.ProductVersion, ZXC.VvProgramVersion);

      //if(Vektor.Properties.Settings.Default.ZoviUpgrade)
      //{
      //   Vektor.Properties.Settings.Default.Upgrade();
      //   Vektor.Properties.Settings.Default.ZoviUpgrade = false;
      //   Vektor.Properties.Settings.Default.Save();
      //}

      loginForm.labelDebug.Font = ZXC.vvFont.LargeLargeFont;
      loginForm.labelDebug.Text = "DEBUG";

#endif

      do
       {
          ///* ZDRAVKOC */if(ProgressForm != null && ProgressForm.Visible && ZXC.IsSkipSplash == false) SetFormVisibility_ThreadSafe(ProgressForm, false);

         loginForm.ShowDialog();

         if(loginForm.DialogResult == DialogResult.Cancel) System.Environment.Exit(0);

         Cursor.Current = Cursors.WaitCursor;

         ZXC.IsSkipSplash = loginForm.Fld_SkipSplash;

         ///* ZDRAVKOC */ if(ZXC.IsSkipSplash == false) ShowProgressForm_BackgroundWorker();

         ZXC.vvDB_Server   = loginForm.Fld_ServerHost; //if(!ZXC.NotEmpty(ZXC.vvDB_Server)) ZXC.vvDB_Server = "localhost";
         ZXC.vvDB_VvDomena = loginForm.Fld_VvDomena;
         ZXC.vvDB_User     = loginForm.Fld_UserName;

         if(ZXC.ThisIsHektorProject == false)
         {
            ZXC.vvDB_Password = ZXC.EncryptThis_UserUC_Password(loginForm.Fld_Password, loginForm.Fld_UserName);
         }
         else // ZXC.ThisIsHektorProject 
         {
            ZXC.vvDB_Password = loginForm.Fld_Password;
         }

         ZXC.projectYear         = loginForm.Fld_ProjectYear.ToString(ZXC.VvDateYyyyFormat);
         ZXC.ogYY                = loginForm.Fld_ProjectYear.ToString(ZXC.VvDateYYFormat  );
         ZXC.projectYearFirstDay = new DateTime(int.Parse(ZXC.projectYear)    ,  1,  1);
         ZXC.projectYearLastDay  = new DateTime(int.Parse(ZXC.projectYear)    , 12, 31, 23, 59, 59);
         ZXC.prevYearFirstDay    = new DateTime(int.Parse(ZXC.projectYear) - 1,  1,  1);
         ZXC.prevYearLastDay     = ZXC.projectYearFirstDay - ZXC.OneDaySpan;
         ZXC.prevYearDecembar    = new DateTime(int.Parse(ZXC.projectYear) - 1, 12,  1);
         ZXC.projectPrevYear     = ZXC.prevYearLastDay.ToString(ZXC.VvDateYyyyFormat);
         ZXC.nextYearFirstDay    = (ZXC.projectYearLastDay + ZXC.OneDaySpan).Date;
         ZXC.projectYearAsInt    = ZXC.ValOrZero_Int(ZXC.projectYear);

         ZXC.eRacuniDIR = "_ eRacuni " + ZXC.projectYear;

         if(loginForm.Fld_ShouldUse_LastUsedPrjkt == true)
            ZXC.initialPrjktKCD = loginForm.Fld_LastUsedPrjkt;
         else
            ZXC.initialPrjktKCD = loginForm.Fld_UseThisPrjkt;

         VvPref.login.initialPrjktKCD = ZXC.initialPrjktKCD;

         loginForm.ComboBoxHost_AddEventuallyNewDataSourceMember();
         loginForm.ComboBoxVvDomena_AddEventuallyNewDataSourceMember();
         loginForm.ComboBoxUser_AddEventuallyNewDataSourceMember();

         OK = VvSQL.TestAnd_CREATE_ThePrjktDB_Connection_4ZXC();

         // 01.12.2022: 
         if(ZXC.IsSvDUHdomena && ZXC.IsSVD_ZAH_ODOBRAVATELJ_UserName(ZXC.vvDB_User))
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Ovaj UserName nije predviđen za ulazak u program, već samo za AUTORIZACIJU ZAHTJEVNICA.");
            OK = false;
         }

         // 26.10.2021: check SvDUH_ZAHonly params ... START ... 
         bool shouldCheck_SvDUH_ZAHonly_params = ZXC.IsSVD_ZAH_UserName(ZXC.vvDB_User) && ZXC.IsVektor_SVD_Deployment;

         if(!OK && shouldCheck_SvDUH_ZAHonly_params)
         {
            if(ZXC.vvDB_Server   != ZXC.SVD_serverName ||
               ZXC.vvDB_VvDomena != ZXC.SVD_vvDomena)
            { 
               ZXC.aim_emsg(MessageBoxIcon.Stop, "U polju 'ServerHost:' treba pisati '{0}'\n\r\n\ra u polju 'VvDomena:' treba pisati '{1}'", ZXC.SVD_serverName, ZXC.SVD_vvDomena);

               loginForm.Fld_ServerHostHistory = new List<string> { ZXC.SVD_serverName };
               loginForm.Fld_ServerHost = ZXC.SVD_serverName;

               loginForm.Fld_VvDomenaHistory = new List<string> { ZXC.SVD_vvDomena };
               loginForm.Fld_VvDomena   = ZXC.SVD_vvDomena  ;

               //ZXC.TheVvForm.VvPref.login.ServerHostEncodedAsInFile = "uULMi695I19uib/+jIgJ5Q=="; //VvAES.DecryptData(ZXC.SVD_serverName, ZXC.vv_Login_AES_key);
               //ZXC.TheVvForm.VvPref.login.VvDomenaEncodedAsInFile   = "uULMi695I19uib/+jIgJ5Q=="; //VvAES.DecryptData(ZXC.SVD_vvDomena  , ZXC.vv_Login_AES_key);
            }
         }
         // 26.10.2021: check SvDUH_ZAHonly params ...  END  ... 


         if(ZXC.ThisIsHektorProject) goto thisIsHektorProject_label;
         // 03.07.2014: 
         if(OK) ZXC.vvDB_ServerID = VvSQL.Get_Sql_ServerID(ZXC.TheMainDbConnection);

         if(OK) OK2 = VvSQL.CheckDatabase(ZXC.TheMainDbConnection, GetvvDB_prjktDB_name(), false);

         if(!OK2)
         {
            //Application.Exit();
            System.Environment.Exit(0);
         }

         if(OK)
         {
            ZXC.VvDataBaseInfo? vvDBinfoOfMainPrjkt = VvSQL.GetVvDataBaseInfoForKupdobCD(ZXC.PrjConnection, ZXC.projectYear, ZXC.initialPrjktKCD);

            if(vvDBinfoOfMainPrjkt != null)
            {
               ZXC.initialPrjktTicker = ((ZXC.VvDataBaseInfo)vvDBinfoOfMainPrjkt).ProjectName;

               VvPref.login.initialPrjktTicker = ZXC.initialPrjktTicker;
            }
            else // TODO: mainPrjkt database doesn't exists! Ask for ticker and create database
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Za šifru [{0}] ne postoji DataBase!\n\nZadajte šifru projekta koji postoji u zadanoj projektnoj godini [{1}].", ZXC.initialPrjktKCD, ZXC.projectYear);
               //Application.Exit();
               //System.Environment.Exit(0);
               OK = false;

               // 09.04.2014: start ____________ 
               if(tsCbxVvDataBase == null) tsCbxVvDataBase = new ToolStripComboBox();
               ZXC.lowestPrjktCdDBI = RefreshDataBasesInfo();
               loginForm.rbt_lastUsedPrjkt.Checked = false;
               loginForm.rbt_useThisPrjkt.Checked  = true;
               loginForm.Fld_UseThisPrjkt = ZXC.lowestPrjktCdDBI.ProjectKcdAsUInt;
               // 09.04.2014: end ____________ 
            }
         }

         if(!OK) loginForm.VvPerformOkButtonClick = false;

      } while(!OK);

thisIsHektorProject_label:

      loginForm.Close();

      //this.Text = Getvv_PRODUCT_name() + " " + ZXC.projectYear + " " + ZXC.vvDB_User + "@" + ZXC.vvDB_Server + " / MachineName: " + Environment.MachineName + " / Site: " + ZXC.VektorSite;

#if(!DEBUG)
      if(ZXC.projectYearFirstDay.Year != DateTime.Now.Year && ZXC.ThisIsHektorProject == false) 
         if(Getvv_PRODUCT_name() != ZXC.vv_REMONSTER_PRODUCT_Name) 
            if(ZXC.IsManyYearDB == false) 
               ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE: Odabrali ste rad u godini koja nije tekuća.");
#endif
   }

   #endregion GetLoginData()

   #region GetManifestResourceStream for icons

   public Stream GetManifestResourceStream(string iconFilePath)
   {
      return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(iconFilePath);
   }

   #endregion GetManifestResourceStream for icons

   #region InitializeCrystalReports_LoadDLL_ForDummyReport_InTheBackGround

   // 12.05.2025. vidi opasku u ZXC.cs                                                                          
   //public void LoadDevTec()
   //{
   //   // 03.01.2023: privremeno ugasili ... TODO!!! 
   //   if(false/*ZXC.CURR_prjkt_rec.IsDevizno*/)
   //   {
   //      // Ovo je trouble maker: 
   //      DevTecDao.CheckAndDownloadMissingDevTec(ZXC.PrjConnection);
   //   }
   //
   //}

   public void InitializeCrystalReports_LoadDLL_ForDummyReport_InTheBackGround()
   {
      bool CRinstallationIsInitiated = false;

      #region Novi nacin detekcije neimanja kristala ... UGASITI KASNIJE!!!

      // 24.10.2022: ... ali kasnije ugasi ovo jednog dana kada 'novi' kristali legnu svugdje 
      
      bool hasAppropriateCRruntime = ZXC.HasAppropriateCRruntime();

      if(ZXC.IsRipleyOrKristal) hasAppropriateCRruntime = true;

      if(hasAppropriateCRruntime == false)
      {
         ZXC.LoadCrystalReports_HasErrors = true;

         VvUpdateCrystalReportsDlg dlg = new VvUpdateCrystalReportsDlg();

         if(dlg.ShowDialog() != DialogResult.OK)
         {
            dlg.Dispose();
            ZXC.aim_emsg(MessageBoxIcon.Warning, "Instalacija CrystalReports-a je otkazana.\n\nIzvješća neće biti moguće prikazati.\n\nKontaktirajte support!");
            return;
         }

         /*ZXC.*/CRinstallationIsInitiated = VvInstal_CRforVV_runtime(); // !!!!!!!!!!!!!!! ##################################################

         if(/*ZXC.*/CRinstallationIsInitiated)
         {
            //ZXC.aim_emsg(MessageBoxIcon.Information, "Update CrystalReports-a je iniciran.\n\nTEK KADA INSTALACIJA ZAVRŠI\n\nklikinite na OK button\n\nšto će izazvati ponovno pokretanje programa.");
            //Application.Restart();
            //ZXC.aim_emsg(MessageBoxIcon.Information, "Update CrystalReports-a je iniciran.\n\nTEK KADA INSTALACIJA ZAVRŠI\n\nponovno pokrenite program.");
            System.Environment.Exit(0);
         }
         else
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "Instalacija CrystalReports-a nije uspjela.\n\nIzvješća neće biti moguće prikazati.\n\nKontaktirajte support!");
         }
      }

      #endregion Novi nacin detekcije neimanja kristala ... UGASITI KASNIJE!!!

      if(/*ZXC.*/CRinstallationIsInitiated == false)
      { 
         BackgroundWorker theBW = new BackgroundWorker();

         theBW.WorkerReportsProgress      = false;
         theBW.WorkerSupportsCancellation = false;

         theBW.DoWork             += new DoWorkEventHandler(theBW_DoWork);
         theBW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(theBW_RunWorkerCompleted);

         theBW.RunWorkerAsync(this);
      }
   }

   private void theBW_DoWork(object sender, DoWorkEventArgs e)
   {
      // This method will run on a thread other than the UI thread.
      // Be sure not to manipulate any Windows Forms controls created
      // on the UI thread from this method.
      BackgroundWorker worker = sender as BackgroundWorker;
      VvForm theVvForm = (VvForm)e.Argument;

      // 14.02.2018 prebaceno u async verziju 
      //// CheckAndDownloadMissingDevTec 
      //if(ZXC.CURR_prjkt_rec.IsDevizno)
      //{
      //   // Ovo je trouble maker: 
      //   DevTecDao.CheckAndDownloadMissingDevTecAsync(ZXC.PrjConnection);
      //}

      try
      {
         Vektor.Reports.KIZ.CR_EmptyDummy cr_EmptyDummy = new Vektor.Reports.KIZ.CR_EmptyDummy();
         cr_EmptyDummy.Load();
         //throw new Exception("qweqwe");
      }
      catch(Exception ex)
      {
         ZXC.LoadCrystalReports_HasErrors = true;

         //VvUpdateCrystalReportsDlg dlg = new VvUpdateCrystalReportsDlg();
         //
         //if(dlg.ShowDialog() != DialogResult.OK)
         //{
         //   dlg.Dispose();
         //   ZXC.aim_emsg(MessageBoxIcon.Warning, "Instalacija CrystalReports-a je otkazana.\n\nIzvješća neće biti moguće prikazati.\n\nKontaktirajte support!");
         //   return;
         //}
         //
         //bool installationIsInitiated;
         //
         //installationIsInitiated = VvInstal_CRforVV_runtime();
         //
         //if(installationIsInitiated)
         //{
         // //ZXC.aim_emsg(MessageBoxIcon.Information, "Update CrystalReports-a je iniciran.\n\nTEK KADA INSTALACIJA ZAVRŠI\n\nklikinite na OK button\n\nšto će izazvati ponovno pokretanje programa.");
         // //Application.Restart();
         //   ZXC.aim_emsg(MessageBoxIcon.Information, "Update CrystalReports-a je iniciran.\n\nTEK KADA INSTALACIJA ZAVRŠI\n\nponovno pokrenite program.");
         //   Application.Exit();
         //}
         //else
         //{
         //   ZXC.aim_emsg(MessageBoxIcon.Warning, "Instalacija CrystalReports-a nije uspjela.\n\nIzvješća neće biti moguće prikazati.\n\nKontaktirajte support!");
         //}
      }
   }

   private bool VvInstal_CRforVV_runtime()
   {
      bool is64BitOperatingSystem = System.Environment.Is64BitOperatingSystem;
      //bool isIs64BitProcess       = System.Environment.Is64BitProcess        ;

      //ZXC.aim_emsg(MessageBoxIcon.Information, "Aplikacija će biti zatvorena.\n\nTEK NAKON ZAVRŠENE INSTALACIJE CrystalReports-a\n\nuđite ponovno u program.");

      //string CRRuntimeFile = is64BitOperatingSystem ? @"http://www.viper.hr/vektor/CRRuntime_64bit_13_0_22.msi" :
      //                                                @"http://www.viper.hr/vektor/CRRuntime_32bit_13_0_22.msi" ;

      //string CRRuntimeFile = @"https://origin-az.softwaredownloads.sap.com/public/file/0020000000661872022"; // CR13SP32MSI64_0-80007712.MSI

      string CRRuntimeFile = is64BitOperatingSystem ? @"https://origin-az.softwaredownloads.sap.com/public/file/0020000000661872022" : // "CR for Visual Studio SP32 CR Runtime 64-bit MSI"
                                                      @"https://origin-az.softwaredownloads.sap.com/public/file/0020000000661572022" ; // "CR for Visual Studio SP32 CR Runtime 32-bit MSI"
                                                                                                             
      try
      {
         System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
         //int   elapsedTime = 0    ;
         //bool eventHandled = false;

         myProcess.StartInfo.FileName = CRRuntimeFile;
         //myProcess.StartInfo.Verb           = "Open"           ;
         //myProcess.StartInfo.CreateNoWindow = true             ;
         //myProcess.EnableRaisingEvents      = true             ;
         //myProcess.Exited += new EventHandler(myProcess_Exited);
         myProcess.Start();

         // neuspjeh #1 
         //void myProcess_Exited(object sender, EventArgs e)
         //{
         //   eventHandled = true;
         //   ZXC.aim_emsg(MessageBoxIcon.Information, "Installer Exit Code: {0}", myProcess.ExitCode);
         //}
         //
         //// Wait for Exited event, but not more than 60 x 10 seconds.
         //const int Milliseconds_SLEEP_AMOUNT = 1000;
         //while(!eventHandled)
         //{
         //   elapsedTime += Milliseconds_SLEEP_AMOUNT;
         //   if(elapsedTime > (60000 * 10)) // 10 minuta 
         //   {
         //      break;
         //   }
         //   System.Threading.Thread.Sleep(Milliseconds_SLEEP_AMOUNT);
         //}

         // neuspjeh #2 
         //while(myProcess.HasExited == false)
         //{
         //   //indicate progress to user
         //   Application.DoEvents();
         //   System.Threading.Thread.Sleep(Milliseconds_SLEEP_AMOUNT);
         //}
         //MessageBox.Show("done installing");

         // neuspjeh #3 
         //Type type = Type.GetTypeFromProgID("WindowsInstaller.Installer");
         //WindowsInstaller.Installer installer = (WindowsInstaller.Installer)Activator.CreateInstance(type);
         //installer.InstallProduct(CRRuntimeFile);
      }
      catch(Exception ex2)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, ex2.Message);
         return false;
      }

      return true;
   }

   private void theBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
   {
      // Handle the case where an exception was thrown.
      if(e.Error != null)
      {
         MessageBox.Show(e.Error.Message);
      }
      else if(e.Cancelled)
      {
         MessageBox.Show("CANCELED");
      }
      else
      {
         //MessageBox.Show("FINISHED");
      }
   }

   #endregion InitializeCrystalReports_LoadDLL_ForDummyReport_InTheBackGround

   #region CreateVvReport, PrintPreviewMenu_Button_OnClick, QuickPrintMenu_Button_OnClick

   // Ovdje se dolazi click-om na SubModulPanel - doticni izvj, ToolStripMenuItem - buttonGO ili klasican MenuItem - doticni izvj 
   // stvar se vrti u svom thread-u !!! 
   public int CreateVvReport(ZXC.Koordinata_3D xyz, BackgroundWorker worker, DoWorkEventArgs e)
   {
      int norr = -1;
      VvReport vvReport;
      VvReportSubModul vvReportSubModul = aModuli[xyz.X].aSubModuls[xyz.Y].aRptSubModuls[xyz.Z];
      string reportName = vvReportSubModul.reportName;

      // tu ga sacuvamo jer u nekom drugom thread-u The... nije vise ovaj current.       
      // Npr pustio si da curi izvjestaj pa otvorio KplanUC gdje cekas da ovaj zavrsi... 
      VvReportUC vvReportUC = TheVvReportUC;

      //21.10.2010: 
      // Googlaj "Load Report Failed"
      // HKEY_LOCAL_MACHINE\SOFTWARE\Business Objects\10.5\Report Application Server\InProcServer - PrintJobnLimit je 75, 
      // pa prije dok nisi ovako na pocetku cistio stare unmanaged resources skakao bi ti Exception nakon 75-og uzastopnog pozivanja Report-a 
      //===================================================
      if(vvReportUC.TheVvReport != null)
      {
         vvReportUC.TheVvReport.VirtualReportDocument.Close();

         if(vvReportUC.TheVvReport.VirtualUntypedDataSet != null)
         {
            foreach(System.Data.DataTable dt in vvReportUC.TheVvReport.VirtualUntypedDataSet.Tables)
            {
               dt.Dispose();
            }

            vvReportUC.TheVvReport.VirtualUntypedDataSet.Dispose();
         }
      }
      //===================================================

      vvReportUC.SetReportSource_ThreadSafe(null);

      vvReportUC.GetFields(false);

      // next lajn iz project dependent VvReport factory (virtuals or overriders are in VvForm_Q / VsrForm, ... 
      vvReport = CreateTheVvReport_SwitchReportEnum(vvReportSubModul, reportName, vvReportUC);

      if(vvReport == null) return (-1);

      vvReportUC.TheVvReport = vvReport;

      vvReportUC.AddFilterMemberz();

      /*============*/
      /*============*/
      /*============*/
      //========
      //====

      if(!worker.CancellationPending)
      {
         norr = vvReport.FillDataSet_And_SetDataSource(worker);
      }

      //if(norr < 1) return (norr);

      if(!worker.CancellationPending)
      {
         vvReportUC.SetReportSource_ThreadSafe(vvReport.VirtualReportDocument);
      }

      //====
      //========
      /*============*/
      /*============*/
      /*============*/

      vvReportUC.TheReportViewer.EnableDrillDown = vvReport.EnableDrillDown;

      vvReportUC.SetDisplayGroupTree_ThreadSafe(vvReport.EnableGroupTree);

      // QuickPrint!!! 
      if(this.quickPrintInitiated)
      {
         TheVvReportUC.TheVvReport.VirtualReportDocument.PrintToPrinter(1, false, 0, 0);

         this.quickPrintInitiated = false;
      }

      return norr;
   }

   private void QuickPrintMenu_Button_OnClick(object sender, EventArgs e)
   {
      if(TheVvUC is VvRecordUC)
      {
         // 29.12.2010:
         if(TheVvRecordUC is FakturDUC)
         {
            // 20.05.2015: TH hoce 'kopija racuna' 
            bool isFrom_SaveRecord_OnClick = (sender.ToString() == "SaveRecord_OnClick");
            if(ZXC.IsTEXTHOshop && isFrom_SaveRecord_OnClick == false && TheVvRecordUC is IRMDUC) // THshop + IRM + rucno inicirani print racuna 
            {
               (TheVvRecordUC as FakturDUC).faktur_rec.Napomena = "KOPIJA RN-a! " + (TheVvRecordUC as FakturDUC).faktur_rec.Napomena;
            }

            (TheVvRecordUC as FakturDUC).LoadDscLuiList_And_PutFilterFields(0);
          //if(((FakturDocFilter)(TheVvRecordUC as FakturDUC).VirtualRptFilter).PFD == null) return;
            if(((FakturDocFilter)(TheVvRecordUC as FakturDUC).VirtualRptFilter).PFD == null && (TheVvRecordUC is URMDUC            ) == false && 
                                                                                               (TheVvRecordUC is KalkulacijaMpDUC  ) == false &&
                                                                                               (TheVvRecordUC is MedjuSkladVMIuDUC ) == false &&
                                                                                               (TheVvRecordUC is UGODUC            ) == false &&
                                                                                             //(TheVvRecordUC is POT_DUC           ) == false &&
                                                                                               (TheVvRecordUC is PocetnoStanjeMPDUC) == false  ) return; 

         }

         TheVvRecordUC.ShowRecordReportPreview_Or_QuickPrintRecord(false);
      }
      else if(TheVvUC is VvReportUC)
      {
         this.quickPrintInitiated = true;
         
         ReportWanted_Click_FromReportUC(sender, e); // force preview first! 

         //while(TheVvTabPage.TheBackgroundWorker.IsBusy) continue;

         //TheVvReportUC.TheVvReport.VirtualReportDocument.PrintToPrinter(1, false, 0, 0);
      }
   }

   private void PrintPreviewMenu_Button_OnClick(object sender, EventArgs e)
   {

      if(TheVvUC is VvRecordUC)
      {
         int norr = -1;

          VvInnerTabControl tabCtrl = TheVvRecordUC.TheTabControl;
          VvInnerTabPage    tabPage;

         if(tabCtrl.TabPages.Contains(tabCtrl.TabPages["PredPrint"]) == false)
         {
            // 29.12.2010:
            if(TheVvRecordUC is FakturDUC)
            {
               bool isRNP_Analiza                                                             = // i lokalna i RptFilter-ova varijabla 
               ((FakturDocFilter)(TheVvRecordUC as FakturDUC).VirtualRptFilter).IsRNP_Analiza = sender.ToString() == FakturDUC.RNP_Analiza;

               // 20.05.2015: TH hoce 'kopija racuna' 
               bool isFrom_SaveRecord_OnClick = (sender.ToString() == "SaveRecord_OnClick");
               if((ZXC.IsTEXTHOany || ZXC.CURR_prjkt_rec.Ticker == "QQTEXT") && isFrom_SaveRecord_OnClick == false && TheVvRecordUC is IRMDUC) // THshop + IRM + rucno inicirani print racuna 
               {
                  (TheVvRecordUC as FakturDUC).faktur_rec.Napomena = "KOPIJA RN-a! " + (TheVvRecordUC as FakturDUC).faktur_rec.Napomena;
               }


               (TheVvRecordUC as FakturDUC).LoadDscLuiList_And_PutFilterFields(0);
              // if(((FakturDocFilter)(TheVvRecordUC as FakturDUC).VirtualRptFilter).PFD == null) return; 16.12.2011. KLK i URM su drugaciji
               if(((FakturDocFilter)(TheVvRecordUC as FakturDUC).VirtualRptFilter).PFD == null && (TheVvRecordUC is URMDUC            ) == false && 
                                                                                                  (TheVvRecordUC is KalkulacijaMpDUC  ) == false &&
                                                                                                  (TheVvRecordUC is MedjuSkladVMIuDUC ) == false &&
                                                                                                  (TheVvRecordUC is MedjuSkladVMI2DUC ) == false &&
                                                                                                  (TheVvRecordUC is NivelacijaDUC     ) == false &&
                                                                                                  (TheVvRecordUC is PIZpDUC           ) == false &&
                                                                                                  (TheVvRecordUC is ProizvodnjaDUC    ) == false &&
                                                                                                  (TheVvRecordUC is MedjuSkladDUC     ) == false &&
                                                                                                  (TheVvRecordUC is MedjuSklad2DUC    ) == false &&
                                                                                                  (TheVvRecordUC is TransformDUC      ) == false &&
                                                                                                  (TheVvRecordUC is MedjuSkladMVIDUC  ) == false &&
                                                                                                  (TheVvRecordUC is MedjuSkladMVI2DUC ) == false &&
                                                                                                  //14.04.2015: 
                                                                                                   isRNP_Analiza                        == false &&
                                                                                                  (TheVvRecordUC is RNMDUC            ) == false &&
                                                                                                  (TheVvRecordUC is RNZDUC            ) == false &&
                                                                                                  (TheVvRecordUC is UGODUC            ) == false &&
                                                                                                  (TheVvRecordUC is ZAH_SVD_DUC       ) == false &&
                                                                                                  (TheVvRecordUC is UGNorAUN_PTG_DUC  ) == false &&
                                                                                                //(TheVvRecordUC is POT_DUC           ) == false &&
                                                                                                  (TheVvRecordUC is PocetnoStanjeMPDUC) == false  ) return; 
            }



            tabPage = tabCtrl.TabPages.Add((TheVvRecordUC).CreateVvInnerTabPages("PredPrint", "", ZXC.VvInnerTabPageKindEnum.ReportViewer_TabPage));
            TheVvRecordUC.TheReportViewer.Parent = tabPage;
         }

         
         /*============*/
         //========
         //====

         norr = TheVvRecordUC.ShowRecordReportPreview_Or_QuickPrintRecord(true);

         //====
         //========
         /*============*/


         // namjerno zadnja stvar metode da ne digne selectedTabChanged prije  
         // ShowRecordReportPreview_Or_QuickPrintRecord()                            
         tabCtrl.TabPages["PredPrint"].Selected = true;
      }
      else if(TheVvUC is VvReportUC)
      {
         ReportWanted_Click_FromReportUC(sender, e);
      }

   }

   private void HidePredPrintTabPage()
   {
          VvInnerTabControl tabCtrl = TheVvRecordUC.TheTabControl;

         if(tabCtrl.TabPages.Contains(tabCtrl.TabPages["PredPrint"]) == true)
         {
            tabCtrl.TabPages.Remove(tabCtrl.TabPages["PredPrint"]);
         }
   }

   #endregion CreateVvReport, QuickPrintMenu_Button_OnClick, PrintPreviewMenu_Button_OnClick

   #region ComboBox4Project

   // 07.11.2012: 
   private static bool prjktNazivAppended = false;
   private void tsCbxVvDataBase_DropDown_AppendPrjktNaziv(object sender, EventArgs e)
   {
      if(prjktNazivAppended == false)
      {
         prjktNazivAppended = true;

         //VvDaoBase.LoadGenericVvDataRecordList<Prjkt>(ZXC.TheMainDbConnection, VvUserControl.PrjktSifrar, null, "");
         ZXC.VvDataBaseInfo dbi;

         //ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Prjkt>(null, VvSQL.SorterType.Name);
         List<Prjkt> prjktList = new List<Prjkt>();
         bool OK = VvDaoBase.LoadGenericVvDataRecordList<Prjkt>(ZXC.PrjConnection, prjktList, null, "");

         for(int i=0; i < tsCbxVvDataBase.Items.Count; ++i)
         {
            dbi = (ZXC.VvDataBaseInfo)tsCbxVvDataBase.Items[i];
            uint prjktCode = dbi.ProjectKcdAsUInt;
          //Prjkt prjkt_rec = VvUserControl.PrjktSifrar.SingleOrDefault(prj => prj.KupdobCD == prjktCode);
            Prjkt prjkt_rec = prjktList.SingleOrDefault(prj => prj.KupdobCD == prjktCode);
            dbi.prjktNaziv = prjkt_rec.Naziv;

            tsCbxVvDataBase.Items.RemoveAt(i);
            tsCbxVvDataBase.Items.Insert(i, dbi);
         }
      }
   }

   public bool InitializeComboBox4Project(string trySelectThisTicker, uint withThisRecID, bool isVirgin)
   {
      int initialIdx;

      // TODO: !!!!!!!!!!!!!!!!!! MAAJOR FAKAP!
      //if(this.tsCbxVvDataBase == null) return true;

      this.tsCbxVvDataBase.Sorted = true;

      ZXC.lowestPrjktCdDBI = RefreshDataBasesInfo();

      initialIdx = tsCbxVvDataBase.FindString(trySelectThisTicker);

      if(initialIdx < 0)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ne postoji DB projekt <" + ZXC.VvDB_NameConstructor(ZXC.projectYear, trySelectThisTicker, withThisRecID) + ">!");

         return false;

         // PUSE: 
         //tsCbxVvDataBase.Items.Add(new ZXC.VvDataBaseInfo(ZXC.projectYear, trySelectThisTicker/*ZXC.copyrightName*/, withThisRecID /*0*/));
         //initialIdx = tsCbxVvDataBase.FindString(trySelectThisTicker/*ZXC.copyrightName*/);

         //if(initialIdx < 0)
         //{
         //   ZXC.aim_emsg("Ne mogu inicijalizirati bazu podataka <{0}>!", trySelectThisTicker/*ZXC.copyrightName*/);
         //   System.Environment.Exit(0);
         //}
      }

      tsCbxVvDataBase.SelectedIndex = initialIdx;
         
      ZXC.TheVvDatabaseInfoIn_ComboBox4Projects = (ZXC.VvDataBaseInfo)tsCbxVvDataBase.SelectedItem;

      // Ok, ok. Be adviced: probao si vec nekoliko puta 'SelectedIndexChanged' staviti prije 'SelectedIndex = initialIdx'
      // ali NE IDE! Prestyani se truditi. Trust me/you. 
      if(isVirgin == true) tsCbxVvDataBase.SelectedIndexChanged += new System.EventHandler(tsCbxVvDataBase_SelectedIndexChanged);

      tsCbxVvDataBase.AutoCompleteMode   = AutoCompleteMode.SuggestAppend;
      tsCbxVvDataBase.AutoCompleteSource = AutoCompleteSource.ListItems;

      tsCbxVvDataBaseCurrentlySelectedIdx = tsCbxVvDataBase.SelectedIndex;

      return true;
   }

   private int tsCbxVvDataBaseCurrentlySelectedIdx;

   void tsCbxVvDataBase_SelectedIndexChanged(object sender, EventArgs e)
   {
      // 26.11.2019: start
      bool imaOtvorenTabPage = TheTabControl.Documents.Count > 0;

      if(imaOtvorenTabPage)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Molim, prvo zatvorite sve otvorene TabPage-ove.");

         tsCbxVvDataBase.SelectedIndex = tsCbxVvDataBaseCurrentlySelectedIdx; // go back 

         return;
      }
      else
      {
         tsCbxVvDataBaseCurrentlySelectedIdx = tsCbxVvDataBase.SelectedIndex;
      }
      // 26.11.2019: end 

      ZXC.TheVvDatabaseInfoIn_ComboBox4Projects = (ZXC.VvDataBaseInfo)tsCbxVvDataBase.SelectedItem;

      SetMe_CURR_prjkt_rec(ZXC.TheVvDatabaseInfoIn_ComboBox4Projects);

      ZXC.TheVvForm.VvPref.login.initialPrjktKCD    = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects.ProjectKcdAsUInt;
      ZXC.TheVvForm.VvPref.login.initialPrjktTicker = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects.ProjectName;

      // 12.05.2025. vidi opasku u ZXC.cs                                                                          
      // 03.01.2023: privremeno ugasili ... TODO!!! 
      //if(false/*ZXC.CURR_prjkt_rec.IsDevizno*/)
      //{
      //   DevTecDao.CheckAndDownloadMissingDevTec(ZXC.PrjConnection);
      //}

      // 12.5.2011: 
      VvUserControl.NullifyAllSifrars();

      // 12.12.2011:
      VvDaoBase.ForExistingPugTablesOnly_CheckTableVersion_AndCatchUpIfNeeded(/*TheDbConnection*/ ZXC.PrjConnection, ZXC.TheVvDatabaseInfoIn_ComboBox4Projects.DataBaseName);

      // 08.02.2012: 
      ZXC.ResetAll_GlobalStatusVariables();
   }

   private void SetMe_CURR_prjkt_rec(ZXC.VvDataBaseInfo vvDBI) // vvDBI je ZXC.TheVvDatabaseInfoIn_ComboBox4Projects 
   {
      bool found;
      uint prjktKupdobCD = uint.Parse(vvDBI.ProjectCode);

    //found = ZXC.PrjktDao.SetMe_Record_byRecID           (ZXC.PrjConnection, ZXC.CURR_prjkt_rec, prjktKupdobCD,                                            false);
      found = ZXC.PrjktDao.SetMe_Record_bySomeUniqueColumn(ZXC.PrjConnection, ZXC.CURR_prjkt_rec, prjktKupdobCD, ZXC.KupdobSchemaRows[ZXC.KpdbCI.kupdobCD], false);

      if(found) ZXC.PrvlgDao.Load_ZXC_Privileges(ZXC.PrjConnection, ZXC.CURR_prjkt_rec.KupdobCD/*RecID*/, ZXC.CURR_userName);

      if(Getvv_PRODUCT_name() == ZXC.vv_VEKTOR_PRODUCT_Name || Getvv_PRODUCT_name() == ZXC.vv_SKYLAB_PRODUCT_Name)
      {
         if(found && ZXC.IsSkyEnvironment) ZXC.SkyRuleDao.Load_SkyRules(ZXC.PrjConnection);

         if(found && (ZXC.IsTEXTHOany || ZXC.IsTEXTHOany2))
         {
            TH_PriceRuleForCycleMoment.Init_TH_PriceRuleList();
            TH_PriceRuleForCycleMoment.Init_TH_Calendar     ();
         }
      }

      if(found)
      {
         if(vvDBI.ProjectName != ZXC.CURR_prjkt_rec.Ticker)
            ZXC.aim_emsg("UPOZORENJE: DataBaseName ticker [{0}] nije jednaka tickeru u Projekt datoteci[{1}]!\n\nPreporuka je da ispravite (izjednačite) ticker u datoteci Projekta na [{0}]", vvDBI.ProjectName, ZXC.CURR_prjkt_rec.Ticker);
      }
      else // wanted prjktKupdobCD not found in Prjkt database! 
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Za traženi database\n\nticker: {0}, KCD: {1}, u godini: {2}, \n\nne postoji odgovarajuća kartica u datoteci 'Projekti'.\n\nKREIRATI cu je automatski.", 
            vvDBI.ProjectName, vvDBI.ProjectCode, vvDBI.ProjectYear);

         // --- new in 2009: ---------, ma ovo ti je jedan veliki TODO: 
         Prjkt initialPrjkt    = new Prjkt(prjktKupdobCD);
         initialPrjkt.KupdobCD = prjktKupdobCD;
         initialPrjkt.Naziv    = 
         initialPrjkt.Ticker   = vvDBI.ProjectName;
         initialPrjkt.Napom1   = "Automatski kreiran iz 'SetMe_CURR_prjkt_rec()'";
         ZXC.PrjktDao.ADDREC(ZXC.PrjConnection, initialPrjkt);
         // --- new in 2009  ---------

         InitializeComboBox4Project(initialPrjkt.Ticker, initialPrjkt.KupdobCD, false);
      }

   }

   //public void RefreshDataBasesInfo_PUSE()
   //{
   //   string sPattern = /*ZXC.vvDB_prefix*/GetvvDB_prefix() + "2*";
   //   string[] directories;
   //   ZXC.VvDataBaseInfo dbi;

   //   try
   //   {
   //      directories = System.IO.Directory.GetDirectories(ZXC.vvDB_path, sPattern);
   //   }
   //   catch (Exception e)
   //   {
   //      VvSQL.ReportGeneric_DB_Error("RefreshDataBasesInfo", "Zadani PATH: [" + ZXC.vvDB_path + "] " + e.ToString(), System.Windows.Forms.MessageBoxButtons.OK);
   //      return;
   //   }

   //   tsCbxVvDataBase.Items.Clear();

   //   for(int i = 0; i < directories.Length; ++i)
   //   {
   //      dbi = new ZXC.VvDataBaseInfo(directories[i]);

   //      if(dbi.ProjectYear != ZXC.ProjectYear) continue; // !!! OVO TI JE TEMELJ LOGIKE RADA SA PROJEKTIMA! 

   //      tsCbxVvDataBase.Items.Add(dbi);
   //   }

   //}

   public ZXC.VvDataBaseInfo RefreshDataBasesInfo()
   {
      List<string> dbNames;
      ZXC.VvDataBaseInfo dbi;
      List<uint> prjktCdOfThisYearList = new List<uint>();

      dbNames = VvSQL.GetVVDatabaseNamesList(ZXC.PrjConnection, ZXC.vvDB_www_preffix + GetvvDB_prefix());

      tsCbxVvDataBase.Items.Clear();

      foreach(string currDbName in dbNames)
      {
         dbi = new ZXC.VvDataBaseInfo(currDbName);

         if(dbi.ProjectYear != ZXC.projectYear) continue; // !!! OVO TI JE TEMELJ LOGIKE RADA SA PROJEKTIMA! 

         tsCbxVvDataBase.Items.Add(dbi);

         prjktCdOfThisYearList.Add(dbi.ProjectKcdAsUInt);

      }

      uint minPrjktCD = prjktCdOfThisYearList.Min();

      return tsCbxVvDataBase.Items.Cast<ZXC.VvDataBaseInfo>().Single(theDbi => theDbi.ProjectKcdAsUInt == minPrjktCD);

   }

   #endregion ComboBox4Project

   #region InitializeWorkingProject

   private bool InitializeWorkingProject()
   {
      if(VvPref.login.rememberLastUsed_Projekt == true && 
         VvPref.login.initialPrjktTicker.NotEmpty()    && 
         VvPref.login.initialPrjktKCD.NotZero())
      {
         return InitializeComboBox4Project(VvPref.login.initialPrjktTicker, VvPref.login.initialPrjktKCD, true);
      }
      else
      {
         return InitializeComboBox4Project(ZXC.initialPrjktTicker, ZXC.initialPrjktKCD, true);
      }

   }

   #endregion InitializeWorkingProject

   #region InitializeVvDao

   private /*bool*/ void CheckPrjktsDatabasesExists()
   {
      //bool OK;

      string dbName = GetvvDB_prjktDB_name();

      // so far, so good! _________ 

      // Logic: kod startanja programa check i po potrebi create:     
      // - DataBase 'vvektor', PUG DB za: ZXC.initialPrjktTicker   
      // - za DataBase 'vvektor': prjkt, user                      
      // - za PUG: sve u PUG_RecordList                               
      // I TO S OBZIROM na DB ZXC.initialPrjktTicker (GlavniKorisnik) 
      // kompletni PUG ti treba zbog GetTableSchema                   
      // Added in 2009: ovo za 'PUG' ti obavlja InitializeVvDao() za dbName koji dolazi iz GetLoginData() koji inicijaliziše ZXC.projectYear, ZXC.initialPrjktKCD, ZXC.initialPrjktTicker 

      try
      {
         ZXC.PrjConnection.ChangeDatabase(dbName);
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "ChangeDatabase Exception.\n\nDataBase [{0}] nedostupna!\n\nException message: {1}", dbName, ex.ToString());
         //Application.Exit();
         System.Environment.Exit(0);
      }

      VvSQL.CheckTableVersion_AndCatchUpIfNeeded(ZXC.PrjConnection, dbName, Prjkt.recordName);
      VvSQL.CheckTableVersion_AndCatchUpIfNeeded(ZXC.PrjConnection, dbName, Prjkt.recordNameArhiva);

      VvSQL.CheckTableVersion_AndCatchUpIfNeeded(ZXC.PrjConnection, dbName, User.recordName);
      VvSQL.CheckTableVersion_AndCatchUpIfNeeded(ZXC.PrjConnection, dbName, User.recordNameArhiva);

      VvSQL.CheckTableVersion_AndCatchUpIfNeeded(ZXC.PrjConnection, dbName, Prvlg.recordName);
      VvSQL.CheckTableVersion_AndCatchUpIfNeeded(ZXC.PrjConnection, dbName, Prvlg.recordNameArhiva);

      if(Getvv_PRODUCT_name() == ZXC.vv_PRODUCT_Name || Getvv_PRODUCT_name() == ZXC.vv_SKYLAB_PRODUCT_Name)
      {
         VvSQL.CheckTableVersion_AndCatchUpIfNeeded(ZXC.PrjConnection, dbName, DevTec2.recordName);
         VvSQL.CheckTableVersion_AndCatchUpIfNeeded(ZXC.PrjConnection, dbName, DevTec2.recordNameArhiva);

         VvSQL.CheckTableVersion_AndCatchUpIfNeeded(ZXC.PrjConnection, dbName, Htrans2.recordName);
         VvSQL.CheckTableVersion_AndCatchUpIfNeeded(ZXC.PrjConnection, dbName, Htrans2.recordNameArhiva);

         VvSQL.CheckTableVersion_AndCatchUpIfNeeded(ZXC.PrjConnection, dbName, SkyRule.recordName);
         VvSQL.CheckTableVersion_AndCatchUpIfNeeded(ZXC.PrjConnection, dbName, SkyRule.recordNameArhiva);

      }

      VvSQL.CheckTableVersion_AndCatchUpIfNeeded(ZXC.PrjConnection, dbName, ZXC.vvDB_ucListTableName);

      return /*OK*/;
   }

   private void InitializeVvDao()
   {
      // conn treba za GetTableSchema
      string dbName;

      // Some test --------------------- START 
      
      // 09.02.2016: 
      ZXC.programStartedOnDateTime = VvSQL.GetServer_DateTime_Now(ZXC.PrjConnection);

      TimeSpan timeSpan = /*VvSQL.GetServer_DateTime_Now(ZXC.PrjConnection)*/DateTime.Now - ZXC.programStartedOnDateTime;
      int diffInMinutes = (int)timeSpan.TotalMinutes;
      if(Math.Abs(diffInMinutes) > 5)
         ZXC.aim_emsg("Upozorenje: Vrijeme na clientu (Vaše računalo): {0} vs {1} vrijeme na serveru.",
            DateTime.Now, VvSQL.GetServer_DateTime_Now(ZXC.PrjConnection));
      // Some test --------------------- END   


    // TODO: ovo ti ipak otvara sve tablice nepotrebno za zadnje koristeni prjkt, trebalo bi preko ROOT_Prjkt_rec-a! 
    //dbName = ZXC.VvDB_NameConstructor(ZXC.projectYear, ZXC.initialPrjktTicker, ZXC.initialPrjktKCD); // GetLoginData() koji inicijaliziše ZXC.projectYear, ZXC.initialPrjktKCD, ZXC.initialPrjktTicker

    // 30.3.2011: ovo bi trebalo rijesiti ovaj gornji TODO 
      dbName = ZXC.VvDB_NameConstructor(ZXC.projectYear, ZXC.lowestPrjktCdDBI.ProjectName, ZXC.lowestPrjktCdDBI.ProjectKcdAsUInt); // GetLoginData() koji inicijaliziše ZXC.projectYear, a ZXC.ROOT_Ticker i ZXC.ROOT_PrjktCd su ti zapravo najniza sifra prijekta u godini

      VvSQL.CheckDatabase(ZXC.PrjConnection, dbName, false);

      // next lajn iz project dependent VvDao factory (virtuals or overriders are in VvForm_Q / VsrForm, ... 
      // DAKLE!!! Ovo je VvDao Factory 
      ViperProjectDependent_DataAccessObjects(ZXC.PrjConnection, dbName);

      // 12.12.2011: 
      //VvDaoBase.ForExistingPugTablesOnly_CheckTableVersion_AndCatchUpIfNeeded(ZXC.PrjConnection, ZXC.TheVvDatabaseInfoIn_ComboBox4Projects.DataBaseName);
      // qqq OVO '12.12.2011:' MOZDA TEBA UBITI!!! 
      // qqq CheckTableVersion_AndCatchUpIfNeeded je vec sadrzan 2cm iznad u ViperProjectDependent_DataAccessObjects (za sve, bez obzira postoji ili ne postoji. - inizijalizacija)
      // 30.06.2014: evo, ubio.

      // 16.08.2014: evo, idijote, vratio. Database nije isti kod prvog i drugog ForExistingPugTablesOnly_CheckTableVersion_AndCatchUpIfNeeded. 
      // Prvi je najnizi a drugi aktualni prjktCD u kojega se ulazi. 
      if(dbName != ZXC.TheVvDatabaseInfoIn_ComboBox4Projects.DataBaseName)
      {
         VvDaoBase.ForExistingPugTablesOnly_CheckTableVersion_AndCatchUpIfNeeded(ZXC.PrjConnection, ZXC.TheVvDatabaseInfoIn_ComboBox4Projects.DataBaseName);
      }

      ZXC.PrjktDao  = PrjktDao .Instance(ZXC.PrjConnection, GetvvDB_prjktDB_name()/*ZXC.vvDB_prjktDB_Name*/, ZXC.KupdobDao);
      ZXC.UserDao   = UserDao  .Instance(ZXC.PrjConnection, GetvvDB_prjktDB_name()/*ZXC.vvDB_prjktDB_Name*/);
      ZXC.PrvlgDao  = PrvlgDao .Instance(ZXC.PrjConnection, GetvvDB_prjktDB_name()/*ZXC.vvDB_prjktDB_Name*/);
      
      if(Getvv_PRODUCT_name() == ZXC.vv_PRODUCT_Name || Getvv_PRODUCT_name() == ZXC.vv_SKYLAB_PRODUCT_Name)
      {
         ZXC.DevTecDao = DevTecDao.Instance(ZXC.PrjConnection, GetvvDB_prjktDB_name()/*ZXC.vvDB_prjktDB_Name*/);
         ZXC.HtransDao = HtransDao.Instance(ZXC.PrjConnection, GetvvDB_prjktDB_name()/*ZXC.vvDB_prjktDB_Name*/);

         ZXC.SkyRuleDao=SkyRuleDao.Instance(ZXC.PrjConnection, GetvvDB_prjktDB_name()/*ZXC.vvDB_prjktDB_Name*/);
      }

      #region ZXC.VektorSite

      try
      {
         ZXC.VvDeploymentSite = (ZXC.VektorSiteEnum)Enum.Parse(typeof(ZXC.VektorSiteEnum), ZXC.ROOT_Ticker, true);
      }
      catch(Exception)
      {
         ZXC.VvDeploymentSite = ZXC.VektorSiteEnum.UNKNOWN;
      }

      this.Text = 
         Getvv_PRODUCT_name() + " " + ZXC.projectYear + " " + ZXC.vvDB_User + "@" + ZXC.vvDB_Server +
         (ZXC.vvDB_VvDomena.IsEmpty() ? "" : "." + ZXC.vvDB_VvDomena) + 
         (ZXC.vvDB_ServerID.IsZero () ? "" : "." + ZXC.vvDB_ServerID) + 
         " / MachineName: " + Environment.MachineName + " / Site: " + ZXC.VvDeploymentSite + " (" + ZXC.ROOT_Ticker + ") AppVer: " +
         VvAboutBox.VvProgramVersion;

      #endregion ZXC.VektorSite

      #region GetApplicationRunningInstanceCount 25.08.2016

      ZXC.ThisApplicationRunningInstancesCount = GetThisApplicationRunningInstancesCount();

      // 18.04.2018: 
    //if(ZXC.IsTEXTHOshop && ZXC.ThisApplicationRunningInstancesCount > 1)
      if(ZXC.IsTEXTHOshop && ZXC.ThisApplicationRunningInstancesCount > 1 && Environment.MachineName != "RIPLEY7" && Environment.MachineName != "RIPLEY22")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Nedozvoljeno pokretanje nove (dodatne) instance programa!");
         System.Environment.Exit(0);
      }

      #endregion GetApplicationRunningInstanceCount 25.08.2016

      ZXC.CURR_prjkt_rec = new Prjkt();
      SetMe_CURR_prjkt_rec(ZXC.TheVvDatabaseInfoIn_ComboBox4Projects);

      ZXC.CURR_user_rec = new User();
      ZXC.UserDao.SetMe_Record_byUserName(ZXC.PrjConnection, ZXC.CURR_user_rec, ZXC.CURR_userName);

      #region password discrepancy

      // 02.12.2022: 
    //if(/*OK*/true) // ako smo do ovdje došli - zadani password je MySQL-u ok. let's check nijelinedajbože drugačiji u našoj User tablici 
         // 23.09.2024: 
      if(/*OK*/ZXC.CURR_userName != "root") // ako smo do ovdje došli - zadani password je MySQL-u ok. let's check nijelinedajbože drugačiji u našoj User tablici 
         {
         if(ZXC.CURR_user_rec != null && ZXC.CURR_user_rec.PasswdEncodedAsInFile != ZXC.vvDB_Password) // password discrepancy occured! 
         {
            BeginEdit(ZXC.CURR_user_rec);

            ZXC.CURR_user_rec.PasswdEncodedAsInFile = ZXC.vvDB_Password;

            bool rwtUserOK = ZXC.UserDao.RWTREC(ZXC.PrjConnection, ZXC.CURR_user_rec);

            if(!rwtUserOK) CancelEdit(ZXC.CURR_user_rec);
            else           EndEdit   (ZXC.CURR_user_rec);
         }
      }

      #endregion password discrepancy


      #region Initialize 'NoNeedForZaliha_ArtiklTSs' array from VvLookUpList

      if(Getvv_PRODUCT_name() == ZXC.vv_PRODUCT_Name || Getvv_PRODUCT_name() == ZXC.vv_SKYLAB_PRODUCT_Name)
      {

         ZXC.luiListaArtiklTS.LazyLoad();

         ZXC.NoNeedForZaliha_ArtiklTSs        = ZXC.luiListaArtiklTS.Where(l => l.Flag == true                                  ).Select(lui => lui.Cd).ToArray();
         // 10.05.2017: Tembo UDG additions ... 15.05. ipak NE 
       //ZXC.NoNeedForZaliha_andUDP_ArtiklTSs = ZXC.luiListaArtiklTS.Where(l => l.Flag == true || l.Cd == "UDP"                 ).Select(lui => lui.Cd).ToArray();
       //ZXC.NoNeedForZaliha_andUDP_ArtiklTSs = ZXC.luiListaArtiklTS.Where(l => l.Flag == true || l.Cd == "UDP" || l.Cd == "UDG").Select(lui => lui.Cd).ToArray();
         ZXC.IsMinusOK_or_UDP_ArtiklTS_array  = ZXC.luiListaArtiklTS.Where(l => l.Flag == true || l.Cd == "UDP"                 ).Select(lui => lui.Cd).ToArray();
      }

      #endregion Initialize 'NoNeedForZaliha_ArtiklTSs' array from VvLookUpList

      // 11.02.2015: 
      // 30.11.2015: 
    //if (ZXC.ThisIsSkyLabProject == false && ZXC.IsTEXTHOany      == true                                                                         ) SqlSomeCheckQuery("PROGRAM LOAD", EventArgs.Empty);
      //  14.08.2016:                                                                                                                              
    //if (ZXC.ThisIsSkyLabProject == false && ZXC.IsTEXTHOany      == true  && ZXC.vvDB_IsLocalhost                                                ) SqlSomeCheckQuery("PROGRAM LOAD", EventArgs.Empty);
      //  14.11.2016:                                                                                                                              
    //if (ZXC.ThisIsSkyLabProject == false && ZXC.IsTEXTHOany      == true  && ZXC.vvDB_IsLocalhost && ZXC.IsTEXTHOsky == false                    ) SqlSomeCheckQuery("PROGRAM LOAD", EventArgs.Empty);
    //if (ZXC.ThisIsSkyLabProject == false && ZXC.IsTEXTHOcentrala == false && ZXC.vvDB_IsLocalhost && ZXC.IsTEXTHOsky == false && ZXC.IsNotRipley7) SqlSomeCheckQuery("PROGRAM LOAD", EventArgs.Empty);
      if((ZXC.ThisIsSkyLabProject == false && ZXC.IsTEXTHOcentrala == false && ZXC.vvDB_IsLocalhost && ZXC.IsTEXTHOsky == false && ZXC.IsRipleyOrKristal == false) ||
         (ZXC.IsSvDUH && ZXC.CURR_userName == "roman"))                                                                                              SqlSomeCheckQuery("PROGRAM LOAD", EventArgs.Empty);

      ZXC.RRD = new RiskRulesDsc(ZXC.dscLuiLst_riskRules);
      ZXC.KSD = new KtoShemaDsc (ZXC.dscLuiLst_KtoShema );

      ZXC.VvUTF8Encoding_noBOM = new System.Text.UTF8Encoding(false);

      ZXC.IsVELEFORM = (ZXC.CURR_prjkt_rec.Prezime.ToUpper().StartsWith("SINKOVI") ||
                        ZXC.CURR_prjkt_rec.Ticker == "MICROF"                      ||
                        ZXC.CURR_prjkt_rec.Ticker == "VELEFO"                      ||
                        ZXC.CURR_prjkt_rec.Ticker == "VELOBR"                       );

    //if(ZXC.CURR_prjkt_rec.IsFiskalOnline                         ) 
      if(false/*ZXC.CURR_prjkt_rec.IsFiskalOnline && ZXC.IsRipleyOrKristal*/)
      {
         try
         {
            if(timeSpan.TotalSeconds < 0)

               ZXC.aim_emsg("FISKALIZACIJA: Pomaknite vrijeme vašeg računala unaprijed,\n\ntako da bude kasnije od vremena servera.\n\n...bar {2} sek.\n\nVrijeme na clientu (Vaše računalo):\n\n{0} vs\n\n{1} vrijeme na serveru.",
                  DateTime.Now, VvSQL.GetServer_DateTime_Now(ZXC.PrjConnection), (-1.00D * (timeSpan.TotalSeconds - 1)).ToString0Vv());

            int daysToFiskCertExpyre = (ZXC.CURR_prjkt_rec.FiskalCertifikat_ExpireD - ZXC.programStartedOnDateTime).Days;

            if(ZXC.CURR_prjkt_rec.FiskalCertifikat != null && daysToFiskCertExpyre < 30)
            {
               ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE!\n\r\n\rFiskalni certifikat ističe za {0} dana.\n\n{1}", daysToFiskCertExpyre.ToString(), ZXC.CURR_prjkt_rec.FiskalCertifikat.ToString());
            }
         }
         catch(Exception ex)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "CheckFiskal error\n\r\n\r{0}", ex.Message);
            ZXC.aim_emsg_VvException(ex);
         }

      } // if(ZXC.CURR_prjkt_rec.IsFiskalOnline)

      // 13.04.2018: 
      if(ZXC.IsTEMBO) ZXC.MotVoziloGrCD = "SKU";
      else            ZXC.MotVoziloGrCD = "MOT";

   }

   private void InitializeWorkingDaysList_ForYear()
   {
      if(Getvv_PRODUCT_name() != ZXC.vv_PRODUCT_Name) return;

    //ZXC.SetWorkingDaysList_ForYear(ZXC.projectYearFirstDay.Year);
      ZXC.SetWorkingDaysList_ForYear(                            );
   }

   #endregion InitializeVvDao

   #region Check Privileges & OcuNecuVvUc

   private bool HasNotEnoughPrivilegesForThisAction(ZXC.PrivilegedAction privilegedAction, VvSubModul subModul, string documType)
   {
      return HasNotEnoughPrivilegesForThisAction(privilegedAction, subModul, documType, false);
   }

   private bool HasNotEnoughPrivilegesForThisAction(ZXC.PrivilegedAction privilegedAction, VvSubModul subModul, string documType, bool isSaving)
   {
      #region TH 2018 News 'Zakljucaj SVE Prethodne Godine'
      
      // 07.09.2023: poslovođe imaju IsSuper == true da bi mogli nesto, ali im zelimo zabraniti ADDREC mogucnost da ne bi nest zasrali 
    //if(ZXC.IsTEXTHOany && privilegedAction == ZXC.PrivilegedAction.ADDREC && ZXC.CurrUserNameIs_NOT_Superuser_ButHasSuperuserPrivileges)
      if(ZXC.IsTEXTHOany && privilegedAction == ZXC.PrivilegedAction.ADDREC && ZXC.Curr_TH_userName_Nema_ADD_privileges && !ZXC.RISK_PromjenaNacPlac_inProgress)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Korisnik sa super user privilegijama ne smije dodavati nove zapise!");
         return true;
      }

      if(ZXC.IsTEXTHOany && privilegedAction != ZXC.PrivilegedAction.ENTER_SUBMODUL && ZXC.projectYearAsInt < ZXC.TH_FirstOkYear)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Nedozvoljena promjena podataka prethodnih godina!");

         return true; // Odi van, NE daj mu! 
      }

      #endregion TH 2018 News 'Zakljucaj SVE Prethodne Godine'

      #region SkyNews

      if(ZXC.IsSkyEnvironment && 
         privilegedAction != ZXC.PrivilegedAction.ENTER_SUBMODUL && 
       //privilegedAction != ZXC.PrivilegedAction.ADDREC         && ... tu ne znas jel bu ADDRECal dobro ili lose skladiste/TT, ali ne znas niti da li je tek usao ili usnimava ADDREC akciju... pa ostavi bez ove provjere. 
         (privilegedAction != ZXC.PrivilegedAction.ADDREC || TheVvTabPage.WriteMode == ZXC.WriteMode.Add)  && 
         TheVvUC != null && TheVvUC is IVvRecordAssignableUC && TheVvDataRecord != null                    && 
         TheVvDataRecord.SkyRuleDeniesWrite_ThisIsReceiveOnly == true)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "SkyRule \n{0}\nNe dozvoljava promjenu ovoga zapisa.", TheVvDataRecord.SkyRule);
         return true;
      }

      // ako je tek usao u dodaj ili ispravi, ne znas jos kako bude pri sejvanju. 
      // npr TT_MSI moze biri centralin, shopov, ... 
      string pressumedTT = (ZXC.IsSkyEnvironment && TheVvTabPage != null && TheVvUC is FakturDUC ? (TheVvUC as FakturDUC).dbNavigationRestrictor_TT.RestrictedValues[0] : "");
      bool isJustInitiatedAndTTisForMultipleDUCs = (ZXC.IsSkyEnvironment && isSaving == false && ZXC.CURR_SkyRules.Count(sr => sr.DocumTT == pressumedTT) > 1);
      // 11.05.2015: 
      if(ZXC.IsSkyEnvironment && isJustInitiatedAndTTisForMultipleDUCs == false && TheVvTabPage != null && TheVvUC != null && TheVvUC is IVvRecordAssignableUC && TheVvDataRecord != null && TheVvDataRecord.SkyRule != null && (
         (ZXC.IsTEXTHOcentrala && privilegedAction == ZXC.PrivilegedAction.ADDREC && TheVvDataRecord.SkyRule.CentCanADD == false) ||
         (ZXC.IsTEXTHOcentrala && privilegedAction == ZXC.PrivilegedAction.RWTREC && TheVvDataRecord.SkyRule.CentCanRWT == false) ||
         (ZXC.IsTEXTHOcentrala && privilegedAction == ZXC.PrivilegedAction.DELREC && TheVvDataRecord.SkyRule.CentCanDEL == false) ||
                                                          
         (ZXC.IsTEXTHOshop     && privilegedAction == ZXC.PrivilegedAction.ADDREC && TheVvDataRecord.SkyRule.ShopCanADD == false) ||
         (ZXC.IsTEXTHOshop     && privilegedAction == ZXC.PrivilegedAction.RWTREC && TheVvDataRecord.SkyRule.ShopCanRWT == false) ||
         (ZXC.IsTEXTHOshop     && privilegedAction == ZXC.PrivilegedAction.DELREC && TheVvDataRecord.SkyRule.ShopCanDEL == false)))
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "SkyRule \n{0}\nNe dozvoljava promjenu ovoga zapisa.", TheVvDataRecord.SkyRule);
         return true;
      }

      if(ZXC.IsSkyEnvironment && privilegedAction != ZXC.PrivilegedAction.ENTER_SUBMODUL && TheVvUC is FakturDUC && (privilegedAction == ZXC.PrivilegedAction.RWTREC || privilegedAction == ZXC.PrivilegedAction.DELREC))
      {
         FakturDUC theDUC = TheVvUC as FakturDUC;

         uint skl1 = theDUC.Fld_SkladCD .NotEmpty() ? ZXC.ValOrZero_UInt(theDUC.Fld_SkladCD .Substring(0, 2)) : 0;
         uint skl2 = theDUC.Fld_SkladCD2.NotEmpty() ? ZXC.ValOrZero_UInt(theDUC.Fld_SkladCD2.Substring(0, 2)) : 0;

         if(TheVvDataRecord.SkyRule != null && privilegedAction == ZXC.PrivilegedAction.DELREC)
         {
            if(ZXC.ThisLanServerKind == ZXC.LanSrvKind.CENT                                                             && TheVvDataRecord.SkyRule.CentCanDEL) return false;
            if(ZXC.ThisLanServerKind == ZXC.LanSrvKind.SHOP && (skl1 == ZXC.vvDB_ServerID || skl2 == ZXC.vvDB_ServerID) && TheVvDataRecord.SkyRule.ShopCanDEL) return false;
         }
         if(TheVvDataRecord.SkyRule != null && privilegedAction == ZXC.PrivilegedAction.RWTREC)
         {
            if(ZXC.ThisLanServerKind == ZXC.LanSrvKind.CENT                                                             && TheVvDataRecord.SkyRule.CentCanRWT) return false;
            if(ZXC.ThisLanServerKind == ZXC.LanSrvKind.SHOP && (skl1 == ZXC.vvDB_ServerID || skl2 == ZXC.vvDB_ServerID) && TheVvDataRecord.SkyRule.ShopCanRWT) return false;
         }

         //if(skl1 != ZXC.vvDB_ServerID && skl2 != ZXC.vvDB_ServerID)
         if(TheVvDataRecord.SkyRule != null) // 04.02.2015 dodan ovaj if, prije bilo bez ikojeg if-a 
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Server\n{0}\nNe može " + (privilegedAction == ZXC.PrivilegedAction.RWTREC ? "ispravljati" : "brisati") + " ovaj zapis.", ZXC.vvDB_ServerID);
            return true;
         }

      } // if(ZXC.IsSkyEnvironment && privilegedAction != ZXC.PrivilegedAction.ENTER_SUBMODUL && TheVvUC is FakturDUC && (privilegedAction == ZXC.PrivilegedAction.RWTREC || privilegedAction == ZXC.PrivilegedAction.DELREC))

      bool isThisRecordInSkyRules = (ZXC.IsSkyEnvironment && privilegedAction != ZXC.PrivilegedAction.ENTER_SUBMODUL && ZXC.CURR_SkyRules.Any(sr => sr.Record == TheVvDataRecord.VirtualRecordName));

    //if(ZXC.IsSkyEnvironment && privilegedAction != ZXC.PrivilegedAction.ENTER_SUBMODUL && TheVvUC is FakturDUC   && privilegedAction == ZXC.PrivilegedAction.ADDREC)
      if(ZXC.IsSkyEnvironment && privilegedAction != ZXC.PrivilegedAction.ENTER_SUBMODUL && isThisRecordInSkyRules && privilegedAction == ZXC.PrivilegedAction.ADDREC)
      {
         if(TheVvUC is FakturDUC)
         {
            FakturDUC theDUC = TheVvUC as FakturDUC;

            string thisDucTT = theDUC.dbNavigationRestrictor_TT.RestrictedValues.Count().NotZero() && theDUC.dbNavigationRestrictor_TT.RestrictedValues[0] == null ? "" : theDUC.VvNavRestrictor_TT.RestrictedValues[0];

            bool thisFakturTt_and_thisBirthLoc_IsInSkyRule = ZXC.CURR_SkyRules.Any(sr => sr.Record == Faktur.recordName && sr.DocumTT == thisDucTT && sr.BirthLoc == ZXC.ThisLanServerKind);
            if(thisFakturTt_and_thisBirthLoc_IsInSkyRule == false) return false; // npr PIZ, INV centrale, ... 

            SkyRule skyRule_fakturRecord_thisDucTT_birthLocIsMy = ZXC.CURR_SkyRules

               .FirstOrDefault(sr => sr.Record == Faktur.recordName && sr.DocumTT == thisDucTT && sr.BirthLoc == ZXC.ThisLanServerKind); // lociranje skyRule samo na osnovi prva 3 param... jer je za saznavanje CanXYZ bool-ova dovoljno (FirstOrDefault zbog MSI koji je 2 put u rulama)

            if(ZXC.ThisLanServerKind == ZXC.LanSrvKind.CENT && skyRule_fakturRecord_thisDucTT_birthLocIsMy != null && skyRule_fakturRecord_thisDucTT_birthLocIsMy.CentCanADD) return false;
            if(ZXC.ThisLanServerKind == ZXC.LanSrvKind.SHOP && skyRule_fakturRecord_thisDucTT_birthLocIsMy != null && skyRule_fakturRecord_thisDucTT_birthLocIsMy.ShopCanADD) return false;

            ZXC.aim_emsg(MessageBoxIcon.Error, "Server tipa\n{0}\nNe može dodavati ovaj zapis.", ZXC.ThisLanServerKind);
            return true;
         } // if(TheVvUC is FakturDUC) 

         if(TheVvUC is ArtiklUC)
         {
            SkyRule skyRule_artiklRecord = ZXC.CURR_SkyRules.Single(sr => sr.Record == Artikl.recordName);

            if(ZXC.ThisLanServerKind == ZXC.LanSrvKind.CENT && skyRule_artiklRecord.CentCanADD) return false;
            if(ZXC.ThisLanServerKind == ZXC.LanSrvKind.SHOP && skyRule_artiklRecord.ShopCanADD) return false;
         
            ZXC.aim_emsg(MessageBoxIcon.Error, "Server tipa\n{0}\nNe može dodavati ovaj zapis.", ZXC.ThisLanServerKind);
            return true;
         } //if(TheVvUC is ArtiklUC) 

      } // if(privilegedAction != ZXC.PrivilegedAction.ENTER_SUBMODUL && TheVvUC is FakturDUC && privilegedAction == ZXC.PrivilegedAction.ADDREC) 

      #endregion SkyNews

      #region 2017 News: do not touch RISK from PG YEARS

      if(ZXC.CURR_prjkt_rec./*ShouldPrevYearLock_RISK*/ShouldPeriodLock        && // u Prjkt je upaljeno 'ne daj mijenjati RISK iz proslih godina ... NE nego zakljucajPrethodniMjesec
         ZXC.projectYearFirstDay.Year   != DateTime.Now.Year                   && // usli smo u neku prethodnu, a ne tekucu, godinu               
         ZXC.TodayIsInFirst20YearDays   == false                               && // dozvoljavamo jos u prvih 20 dana siječnja                    
         subModul.modulEnum             == ZXC.VvModulEnum.MODUL_RAS           && // radimo na modulu RISK                                        
         privilegedAction               != ZXC.PrivilegedAction.ENTER_SUBMODUL && // action is WRITE action (ADD or RWT or DEL)                   
         ZXC.CurrUserHasSuperPrivileges == false                               && // CurrUser NIJE superuser niti root niti IsSuper               
       //ZXC.IsRNMnotRNP                == true                                 ) // Ovo je, valjda, Metaflex                                     
       //TODO izuzetci, kada pocnui svi zvati da ne mogu ... !!! a tada im reci da mogu kao superuser, ako bas moraju                             
         true) // samo za laksu sintaksu
      {
         //ZXC.IssueAccessDeniedMessage(privilegedAction, subModul, documType); 

         ZXC.aim_emsg(MessageBoxIcon.Stop, "Nedozvoljena promjena RISK podatka prethodnih godina!");

         return true; // Odi van, NE daj mu! 
      }

      #endregion 2017 News: do not touch RISK from PG YEARS

      #region SvDUH Close Prev Month IZD Write (except 'roman')
      if(ZXC.IsSvDUH && privilegedAction != ZXC.PrivilegedAction.ENTER_SUBMODUL && TheVvUC is IZD_SVD_DUC && 
         ZXC.CURR_userName != "roman" &&
         ZXC.CURR_userName != ZXC.vvDB_programSuperUserName && 
         ZXC.CURR_userName != "brankica") // I jesmo SvDUH, i jesmo na IZD_SVD_DUV i NISMO 'roman' i NISMO 'brankica' 
      {
         if(privilegedAction == ZXC.PrivilegedAction.RWTREC || privilegedAction == ZXC.PrivilegedAction.DELREC)
         {
            if((TheVvUC as IZD_SVD_DUC).Fld_DokDate.IsFromTodayMMYYYY() == false)
            { 
               ZXC.aim_emsg(MessageBoxIcon.Stop, "Nedozvoljena promjena IZD-atnice iz prethodnih mjeseci!");

               return true; // Odi van, NE daj mu! 
            }
         }
      }

      if(
         (privilegedAction == ZXC.PrivilegedAction.ADDREC || privilegedAction == ZXC.PrivilegedAction.RWTREC || privilegedAction == ZXC.PrivilegedAction.DELREC)
         && TheVvUC is ZAH_SVD_DUC && !ZXC.IsSvDUH_ZAHonly
        )
      {
         // 05.09.2022: odlucili da ne samo upozoravamo vec i nedamo mijenjati ZAH NEzahtjevnicarima 
       //ZXC.aim_emsg(MessageBoxIcon.Warning, "Upozorenje!\n\r\n\rDokument 'ZAH' bi trebali mijenjati samo odjeli!?");
       ////return true; // Odi van, NE daj mu! 

         ZXC.aim_emsg(MessageBoxIcon.Error, "Greška!\n\r\n\rDokument 'ZAH' mijenjaju samo odjeli.");
         return true; // Odi van, NE daj mu! 

      }

      #endregion SvDUH Close Prev Month IZD Write (except 'roman')

      #region Classic

      if(ZXC.CURR_prjkt_rec.IsAuthn == false)                return false; // dis projekt daznt check forprivileges 

      if(ZXC.CURR_userName == ZXC.vvDB_systemSuperUserName  ||
         ZXC.CURR_userName == ZXC.vvDB_programSuperUserName || ZXC.CURR_user_rec.IsSuper) return false; // root rules!                           
      //else // user is NOT 'root' 
      //{
      //   // Rule overrider: ako user nije 'root', nedamo mu niti prismrditi u cio Modul 'Prjkt' 
      //   // Overrider overridera: promjena plana, obicnim userima das da gledaju a mozda i SAMO DODAJE projekte a 
      //   // User i Prvlg operacije zastiti sa 'VvUcOvuNecu' forom. 
         
      //   if(privilegedAction == ZXC.PrivilegedAction.ENTER_SUBMODUL && subModul.modulEnum == ZXC.VvModulEnum.MODUL_PRJKT)
      //   {
      //      IssueAccessDeniedMessage(privilegedAction, subModul, documType);
      //      return true;
      //   }
      //}

      // TT Privilegije   Sifra 
      //                         
      // NEOGRANICENO         0  
      // SAMO DODAJE          1  
      // SAMO GLEDA           2  
      //                         

      char wantedModulAsChar        = ((int)subModul.modulEnum       ).ToString()[0];
      char wantedSubModulKindAsChar = ((int)subModul.subModulKindEnum).ToString()[0];
      
      // ######################################################################################### 
      // ######### LOOP START #################################################################### 
      // ######################################################################################### 

      foreach(Prvlg prvlg in ZXC.CURR_Privileges)
      {
         #region Scope 'Program'

         if(prvlg.PrvlgScope.Length == 1) // Scope 0 - cio 'Program' 
         {
            if(PrivilegeType_Covers_PrivilegeAction(prvlg.PrvlgType, privilegedAction) == true) return false;
            else                                                                                continue;
         } 

         else // za scope detaljniji od '0' preskoci privilegije koje se na odnose na vvModulEnum iz parametra 
         {
            if(prvlg.PrvlgScope[1] != wantedModulAsChar) continue;    // npr '011 - SubM Sifrar SKLAD' na drugom mjestu ima pripadnost modulu 'MODUL_RAS' 
         }

         #endregion Scope 'Program'

         // sada smo dakle na Privilegiji kojoj scope nije '0 - program' i koja se odnosi na trzeni modul (MODUL_RAS, MODUL_FIN, ...) 

         #region Scope 'Modul'

         // Scope 'Modul' 
         if(prvlg.PrvlgScope.Length == 2) // Scopes: '01', '02', '03', ... meaning cio modul (MODUL_RAS, MODUL_FIN, ...) 
         {
            if(PrivilegeType_Covers_PrivilegeAction(prvlg.PrvlgType, privilegedAction) == true) return false;
            else                                                                                continue;

         } // if(prvlg.PrvlgScope == "0") 

         else // za scope detaljniji od '??' (dvoznamenkasti), preskoci privilegije koje se na odnose na vvModulKindEnum iz parametra 
         {
            if(prvlg.PrvlgScope[2] != wantedSubModulKindAsChar) continue;    // npr '011 - SubM Sifrar SKLAD' na trecem mjestu ima pripadnost kind-u 'SIFRAR' 
         }

         #endregion Scope 'Modul'

         // sada smo dakle na Privilegiji kojoj scope nije 'program' niti 'modul' i koja se odnosi na trzeni Kind (SIFRAR, DOKUMENT, REPORT_MENU) 

         #region Scope 'SubModulKind' (SIFRAR=1, REPORT_MENU=2, DOCUMENT=3)

         // Scope 'SubModul' (SIFRAR=1, DOKUMENT=2, REPORT_MENU=3) 
         if(prvlg.PrvlgScope.Length == 3) // Scopes: '011', '012', '013', ... meaning cio modul (Sifrar SKLAD-011, Izvj MODUL_RAS-013, Document MODUL_RAS-012) 
         {
            if(PrivilegeType_Covers_PrivilegeAction(prvlg.PrvlgType, privilegedAction) == true) return false;
            else                                                                                continue;

         } // if(prvlg.PrvlgScope == "0") 

         else // sada preskoci krivi documType
         {
            if(documType.NotEmpty())
            {
               if(prvlg.DocumType != documType) continue;    // npr 'TM - Temeljnica' 
            }
            else if(privilegedAction == ZXC.PrivilegedAction.ENTER_SUBMODUL ||
                    privilegedAction == ZXC.PrivilegedAction.ADDREC) // documType je "" pa ga pusti ENTER_SUBMODUL ili da inicira ADDREC (sto jos uvijek ne znaci da ce ga uspjeti sejvati) 
            {
               return false;
            }
         }

         #endregion Scope 'SubModulKind' (SIFRAR=1, REPORT_MENU=2, DOCUMENT=3)

         // ovo sad nema biti kaj drugo doli 4znamenkasti scope sa dobrim modulom i subModulKind-om i
         // dobrim documType-om pa ga moramo provjeriti da li PrivilegeType_Covers_PrivilegeAction   

         if(PrivilegeType_Covers_PrivilegeAction(prvlg.PrvlgType, privilegedAction) == true) return false;

      } // foreach(Prvlg prvlg in ZXC.CURR_Privileges)

      // ######################################################################################### 
      // ######### LOOP end   #################################################################### 
      // ######################################################################################### 

      ZXC.IssueAccessDeniedMessage(privilegedAction, subModul, documType);

      #endregion Classic

      return true;
   }

   private bool PrivilegeType_Covers_PrivilegeAction(string prvlgType, ZXC.PrivilegedAction privilegedAction)
   {
      // Tu smo dosli nakon provjere Scope-a. Dakle Scope je dobar, a sada provjeri da li 
      // promatrani tt privilegije pokriva zahtijevanu privilegedAkciju                  

      if(prvlgType == "0") // Privilegija 'NEOGRANICENO' 
      { 
         return true; 
      }

      if(prvlgType == "1") // Privilegija 'SAMO DODAJE'  
      {
         if(privilegedAction == ZXC.PrivilegedAction.ADDREC         ||
            privilegedAction == ZXC.PrivilegedAction.ENTER_SUBMODUL  ) return true;
      }

      if(prvlgType == "2") // Privilegija 'SAMO GLEDA'   
      {
         if(privilegedAction == ZXC.PrivilegedAction.ENTER_SUBMODUL) return true;
      }

      //ZXC.aim_emsg("KLO: PrivilegeType [{0}] and action [{1} UNMATCHED!]", prvlgType, privilegedAction.ToString());
      return false;
   }

   protected void InitializeVvUserControlList()
   {
      if(ZXC.VvUserControlList == null) ZXC.VvUserControlList = new List<VvSQL.VvUcListMember>();
      else                              ZXC.VvUserControlList.Clear();

      VvDaoBase.LoadVvUserControlList(ZXC.VvUserControlList);
   }

   protected bool OcuNecuVvUc(XSqlConnection tempConnection, string enumName)
   {
      if(ZXC.IsSvDUH_ZAHonly                                 && 
         enumName != ZXC.VvSubModulEnum.R_ZAH_SVD.ToString() &&
         enumName != ZXC.VvSubModulEnum.R_IZD_SVD.ToString() && 
         enumName != ZXC.VvSubModulEnum.ART      .ToString() && 
         enumName != ZXC.VvSubModulEnum.LsART    .ToString() && 
         enumName != ZXC.VvSubModulEnum.LsFAK    .ToString() && 
         enumName != ZXC.   VvModulEnum.MODUL_RAS.ToString() &&
         enumName != "GROUP10"                                 ) return false;

      //return true;
      bool ocu = true, foundInDB;

      if(enumName == "SEPARATOR") return true;

      // 09.07.2021: 
    //if(enumName.StartsWith("GROUP")) return true;
      if(enumName == "GROUP10" && ZXC.IsSvDUHdomena     == false) return false;
    //if(enumName == "GROUP11" && ZXC.IsPCTOGOdomena    == false) return false; zastitari su group10 pa sam njih stavila na 11
      if(enumName == "GROUP12" && ZXC.IsPCTOGOdomena    == false) return false;
      if(enumName == "GROUP13" && ZXC.IsTETRAGRAMdomena == false) return false;
    //if(enumName == "GROUP14" && /*(ZXC.initialPrjktTicker.StartsWith("VIPER") == false)*/ZXC.IsRipleyOrKristal == false) return false; 01.01.2026. dajemo svima
      if(enumName.StartsWith("GROUP")) return true;


      //VvSQL.VvUcListMember ucMember = new VvSQL.VvUcListMember();
      //VvSQL.VvUcListMember ucMember = ZXC.VvUserControlList.SingleOrDefault(uc => uc.enumName == enumName);
      VvSQL.VvUcListMember ucMember = ZXC.VvUserControlList.FirstOrDefault(uc => uc.enumName == enumName);

      //foundInDB = VvDaoBase.UcEnumNameIsIn_VvUserControlList(tempConnection, ref ucMember, enumName);
      foundInDB = (ucMember.recID.NotZero());

      // necu ovaj UC jer: uopce nije naveden u listi UC-ova za ovog korisnika 
      if(foundInDB == false) return false;

      // necu ovaj UC jer: je UC namijenjen samo root useru a ovaj to nije 
      if(ucMember.rootOnly == true && ZXC.CURR_userName != ZXC.vvDB_systemSuperUserName && ZXC.CURR_userName != ZXC.vvDB_programSuperUserName /*&& !ZXC.CURR_user_rec.IsSuper*/) return false;

      // 29.02.2016: pobeglo bez 'valentina' pa moramo 'vako: 
      if(ucMember.enumName == ZXC.VvSubModulEnum.R_WYR         .ToString() ||
         ucMember.enumName == ZXC.VvReportEnum  .RIZ_OPZSTAT1  .ToString() ||
         ucMember.enumName == ZXC.VvReportEnum  .RIZ_OPZSTAT1_S.ToString() ||
         ucMember.enumName == ZXC.VvSubModulEnum.R_RNM         .ToString() ||
         ucMember.enumName == ZXC.VvSubModulEnum.R_PIP         .ToString()  )
      {
         if(ZXC.IsTEXTHOany2) ucMember.okLogin1 = "valentina";
      }

      // necu ovaj UC jer: je UC namijenjen samo eksplicitno imenovanim userima ili root useru a ovaj nije niti taj eksplicitni a niti root. 
      if(ucMember.okLogin1.NotEmpty() ||
         ucMember.okLogin2.NotEmpty() ||
         ucMember.okLogin3.NotEmpty() ||
         ucMember.okLogin4.NotEmpty())
      {
         if(ZXC.CURR_userName != ZXC.vvDB_systemSuperUserName &&
            ZXC.CURR_userName != ZXC.vvDB_programSuperUserName &&
            //ZXC.CURR_user_rec.IsSuper == false && 
            ZXC.CURR_userName != ucMember.okLogin1 &&
            ZXC.CURR_userName != ucMember.okLogin2 &&
            ZXC.CURR_userName != ucMember.okLogin3 &&
            ZXC.CURR_userName != ucMember.okLogin4) 
            
            return false;
      }

      // necu ovaj UC jer: je UC namijenjen svima osim eksplicitno imenovanim userima a ovaj je taj eksplicitni. 
      if(ZXC.CURR_userName == ucMember.stopLogin1 ||
         ZXC.CURR_userName == ucMember.stopLogin2 ||
         ZXC.CURR_userName == ucMember.stopLogin3 ||
         ZXC.CURR_userName == ucMember.stopLogin4)
      {
         return false;
      }

      // 09.07.2021: pokusaj ...
    //if(ZXC.IsSvDUHdomena == false && SVD_SubModulEnumNames.Contains(ucMember.enumName) == true) return false;

      return ocu;
   }

 //private ZXC.VvSubModulEnum[] SVD_SubModulEnums = { ZXC.VvSubModulEnum.R_IZD_SVD, ZXC.VvSubModulEnum.R_URA_SVD, ZXC.VvSubModulEnum.R_NRD_SVD };
 //private string[] SVD_SubModulEnumNames = { ZXC.VvSubModulEnum.R_IZD_SVD.ToString(), ZXC.VvSubModulEnum.R_URA_SVD.ToString(), ZXC.VvSubModulEnum.R_NRD_SVD.ToString() };

   #endregion Check Privileges & OcuNecuVvUc

   #region AddAndGetNewVvSifrarRecordInteractive

   public VvSifrarRecord AddAndGetNewVvSifrarRecordInteractive(VvRecLstUC theRecLstUC/*ZXC.VvSubModulEnum wantedSubModulEnum*/)
   {
      ZXC.VvSubModulEnum wantedSubModulEnum = theRecLstUC.MasterSubModulEnum;
      Point xy = GetSubModulXY(wantedSubModulEnum);

      VvTabPage localVvTabPage = new VvTabPage(this, "", xy, ZXC.VvTabPageKindEnum.RECORD_TabPage_INTERACTIVE, null,                null, null, null);

      #region Added 12.06.2010
      
      ////kad je ponovni ulazak onda se izgubi ZXC.TheGlobalVvDataRecord 
      
      //if(ZXC.TheGlobalVvDataRecord == null)
      //{
      //   ZXC.aim_emsg(MessageBoxIcon.Stop, "Nemogu dva puta.");
      //   return null;
      //}

      #endregion Added 12.06.2010

      if(ZXC.TheGlobalVvDataRecord != null)
      {
         localVvTabPage.TheVvRecordUC.PutFields(ZXC.TheGlobalVvDataRecord); // ovoga Global ti puni u LoadIzvodUC.AddNew_OR_PairOld_OR_ChooseOneOfMany();
      }

      //------------------------------------------------------- 
      if(localVvTabPage.TheVvDataRecord.IsAutoSifra) // VvSifrarDataRecord-i 
      {
         uint   newSifra;
         string sifraColName = localVvTabPage.TheVvSifrarRecordUC.VirtualSifrarRecord.SifraColName;

         if(localVvTabPage.TheVvDataRecord.IsStringAutoSifra) /* sifCD je 'string' */ 
            newSifra = localVvTabPage.TheVvDataRecord.VvDao.GetNextSifra_String(TheDbConnection, localVvTabPage.TheVvDataRecord.VirtualRecordName, sifraColName);
         else                                                 /* sifCD je 'uint'   */
            // 09.01.2015: 
          //newSifra = localVvTabPage.TheVvDataRecord.VvDao.GetNextSifra_Uint(TheDbConnection, localVvTabPage.TheVvDataRecord.VirtualRecordName, sifraColName,                TheVvDataRecord.UintSifraRootNum,                TheVvDataRecord.UintSifraBaseFactor);
            newSifra = localVvTabPage.TheVvDataRecord.VvDao.GetNextSifra_Uint(TheDbConnection, localVvTabPage.TheVvDataRecord.VirtualRecordName, sifraColName, localVvTabPage.TheVvDataRecord.UintSifraRootNum, localVvTabPage.TheVvDataRecord.UintSifraBaseFactor);

         localVvTabPage.TheVvSifrarRecordUC.PutNew_Sifra_Field(newSifra);
      }
      //------------------------------------------------------- 

      ZXC.TheGlobalVvDataRecord = null;

      // 19.01.2013: _____ START ________________________ 
      if(theRecLstUC is ArtiklListUC && ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.DUCATI) // TODO: iz rulsa 
      {
         ArtiklListUC artiklListUC = theRecLstUC as ArtiklListUC;

         if(artiklListUC.SelectedRowIndex.IsZeroOrPositive())
         {
            string artiklCD_ofTemplate = artiklListUC.TheGrid.GetStringCell("artiklCD", artiklListUC.SelectedRowIndex, false);
            if(artiklCD_ofTemplate.NotEmpty())
            {
               Artikl templateArtikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(a => a.ArtiklCD == artiklCD_ofTemplate);
               if(templateArtikl_rec != null)
               {
                  ArtiklUC artiklUC = localVvTabPage.TheVvSifrarRecordUC as ArtiklUC;
                  artiklUC.PutFields(templateArtikl_rec);
                  artiklUC.CleanUniqueFieldsOnCopyFromOtherRecord();
               }
            }
         }

         if(artiklListUC.Fld_FromArtiklCD.NotEmpty())
         {
            localVvTabPage.TheVvSifrarRecordUC.PutNew_Sifra_Field(artiklListUC.Fld_FromArtiklCD);
         }
      }
      // 19.01.2013: _____ END   ________________________ 

      // 19.01.2013: _____ START ________________________ 
      if(theRecLstUC is ArtiklListUC && ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.PCTOGO)  
      {
         ArtiklListUC artiklListUC = theRecLstUC as ArtiklListUC;

         string targetNewArtiklCD = artiklListUC.Fld_FromArtiklCD;

         bool from_new_PCK_artiklCD  /* = false*/; // iz nove, nepostojece sifre, zadana je željena, ciljana NOVA šifra 
         bool from_old_PCK_artikl_rec/* = false*/; // iz postojeceg, slicnog                                            

         Artikl artiklPremaUpisanojSifri = artiklListUC.Fld_FromArtiklCD.NotEmpty() ? TheVvUC.Get_Artikl_FromVvUcSifrar(artiklListUC.Fld_FromArtiklCD) : null;

         if(targetNewArtiklCD.NotEmpty() && artiklPremaUpisanojSifri == null) from_new_PCK_artiklCD = true ; // zadana je željena, ciljana NOVA šifra 
         else                                                                 from_new_PCK_artiklCD = false;

         from_old_PCK_artikl_rec = !from_new_PCK_artiklCD;

         if(from_new_PCK_artiklCD)
         {
            string  newArtiklCD          = artiklListUC.Fld_FromArtiklCD;
            string  newArtiklCD_PCK_base = Artikl.Get_PCK_BazaCD(newArtiklCD);

            (decimal newPCK_RAM, decimal newPCK_HDD) = Artikl.Get_PTG_RAM_HDD_From_ArtiklCD(newArtiklCD);

            Artikl templateArtikl_rec = VvUserControl.ArtiklSifrar.LastOrDefault(a => a.PCK_BazaCD == newArtiklCD_PCK_base);

            if(templateArtikl_rec != null)
            {
               ArtiklUC artiklUC = localVvTabPage.TheVvSifrarRecordUC as ArtiklUC;

               Artikl newArtikl_rec = templateArtikl_rec.MakeDeepCopy();

               newArtikl_rec.PCK_RAM = newPCK_RAM;
               newArtikl_rec.PCK_HDD = newPCK_HDD;

               newArtikl_rec.ArtiklCD   = newArtikl_rec.New_ArtiklCD_From_PCK_base_RAM_HDD;
               newArtikl_rec.ArtiklName = ZXC.ModifyPCK_ArtiklName(templateArtikl_rec.ArtiklName, newPCK_RAM, newPCK_HDD, templateArtikl_rec.ZapreminaJM, templateArtikl_rec.DuljinaJM);

               artiklUC.PutFields(newArtikl_rec);
               artiklUC.tbx_artiklName.Select();

               //artiklUC.CleanUniqueFieldsOnCopyFromOtherRecord();
            }
            else // ? 
            { 
            }

         } // if(fromPCKbase) 

         if(from_old_PCK_artikl_rec)
         {
            if(artiklListUC.SelectedRowIndex.IsZeroOrPositive())
            {
               string artiklCD_ofTemplate = artiklListUC.TheGrid.GetStringCell("artiklCD", artiklListUC.SelectedRowIndex, false);
               if(artiklCD_ofTemplate.NotEmpty())
               {
                  Artikl templateArtikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(a => a.ArtiklCD == artiklCD_ofTemplate);

                  if(templateArtikl_rec != null)
                  {
                     ArtiklUC artiklUC = localVvTabPage.TheVvSifrarRecordUC as ArtiklUC;
                     artiklUC.PutFields(templateArtikl_rec);
                     artiklUC.tbx_artiklName.Select();

                     // ne za pctogo: 
                     //artiklUC.CleanUniqueFieldsOnCopyFromOtherRecord();
                  }
               }
            }
         } // if(fromTheList) 

      } // if(theRecLstUC is ArtiklListUC && ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.PCTOGO)   

      // 19.01.2013: _____ END   ________________________ 



      VvAddInteractiveSifrarRecordDlg theDlg = new VvAddInteractiveSifrarRecordDlg(localVvTabPage);

      if(theDlg.ShowDialog() != DialogResult.OK) return null;

      return (VvSifrarRecord)localVvTabPage.TheVvDataRecord;
   }

   #endregion AddAndGetNewVvSifrarRecordInteractive

   #region DumpChosenOtsList_OnNalogDUC

   public void DumpChosenOtsList_OnNalogDUC(List<OtsTipBrGroupInfo> choosenOtsList)
   {
      //ZXC.aim_emsg("CurForm je [{0}]", ZXC.CurrentForm);

      if(choosenOtsList == null || choosenOtsList.Count == 0) return;

      NalogDUC    theNalogDUC    = null;
      LoadIzvodUC theLoadIzvodUC = null;

      // 01.04.2026: 
    //if(ZXC.CurrentForm is VvForm     ) 
      if(ZXC.LoadIzvodDLG_isON == false)
      {
         theNalogDUC = (NalogDUC)ZXC.TheVvForm.TheVvDocumentRecordUC;

         VvTextBoxEditingControl vvtbEC = theNalogDUC.TheG.EditingControl as VvTextBoxEditingControl;

         if(choosenOtsList.Count == 1)
         {
            OtsTipBrGroupInfo otsInfo = choosenOtsList[0];

            vvtbEC.EditingControlFormattedValue = otsInfo.TipBr;

            int currRowIdx = vvtbEC.EditingControlRowIndex;

            theNalogDUC.TheG.PutCell(theNalogDUC.DgvCI.iT_fakRecID , currRowIdx, otsInfo.FakRecID);
            theNalogDUC.TheG.PutCell(theNalogDUC.DgvCI.iT_fakYear  , currRowIdx, otsInfo.FakYear );
            theNalogDUC.TheG.PutCell(theNalogDUC.DgvCI.iT_projektCD, currRowIdx, otsInfo.ProjektCD);
            theNalogDUC.TheG.PutCell(theNalogDUC.DgvCI.iT_mtros_cd , currRowIdx, otsInfo.MtrosCD);
            theNalogDUC.TheG.PutCell(theNalogDUC.DgvCI.iT_mtros_tk , currRowIdx, otsInfo.MtrosTK);
            theNalogDUC.TheG.PutCell(theNalogDUC.DgvCI.iT_valuta   , currRowIdx, otsInfo.OpenDokumentValuta);

            int moneyColIdx = otsInfo.IsKupac ? theNalogDUC.DgvCI.iT_pot : theNalogDUC.DgvCI.iT_dug;

            theNalogDUC.TheG.PutCell(moneyColIdx, currRowIdx, otsInfo.UkSaldo);
         }
         else
         {
            ((NalogDUC)TheVvDocumentRecordUC).DumpChosenOtsList(choosenOtsList);
         }
      }
      // 01.04.2026: 
    //else // LoadIzvodDLG 
      else if(ZXC.LoadIzvodDLG_isON == true)
      {
         LoadIzvodDLG dlg = Application.OpenForms.OfType<LoadIzvodDLG>().FirstOrDefault();
         if(dlg != null)
         {
             theLoadIzvodUC = dlg.TheUC;
         }
         else
         {
            ZXC.aim_emsg(MessageBoxIcon.Error,"theLoadIzvodUC is null");
            return;
         }

         VvTextBoxEditingControl vvtbEC = theLoadIzvodUC.TheGrid.EditingControl as VvTextBoxEditingControl;

         if(choosenOtsList.Count == 1)
         {
            OtsTipBrGroupInfo otsInfo = choosenOtsList[0];

            vvtbEC.EditingControlFormattedValue = otsInfo.TipBr;


            theLoadIzvodUC.Fld_DGV_FakRecID  = otsInfo.FakRecID;
            theLoadIzvodUC.Fld_DGV_FakYear   = otsInfo.FakYear ;
            theLoadIzvodUC.Fld_DGV_ProjektCD = otsInfo.ProjektCD;
            theLoadIzvodUC.Fld_DGV_MtrosCd   = otsInfo.MtrosCD;
            theLoadIzvodUC.Fld_DGV_MtrosTK   = otsInfo.MtrosTK;
            theLoadIzvodUC.Fld_DGV_Valuta    = otsInfo.OpenDokumentValuta;

            if(otsInfo.IsKupac)
               theLoadIzvodUC.Fld_DGV_Pot = otsInfo.UkSaldo;
            else
               theLoadIzvodUC.Fld_DGV_Dug = otsInfo.UkSaldo;
         }
         else
         {
            vvtbEC.EditingControlFormattedValue = string.Format("MULTI{0}", choosenOtsList.Count.ToString("000"));

            theLoadIzvodUC.OtsInfoSelectionList = choosenOtsList;
         }
      }
      ZXC.DumpChosenOtsList_OnNalogDUC_InProgress = true;
      SendKeys.Send("{TAB}");
   }

   #endregion DumpChosenOtsList_OnNalogDUC

   #region Implementing Dispose (cleanup 4 unmenidzd sorsis (dbConnecgtion))

   private bool alreadyDisposed = false;

   #region Kako bi bilo da nije derived od allredi disposable
   //// finalizer: Call the virtual Dispose method 
   //~VvTabPage()
   //{
   //   Dispose(false);
   //}

   //// Implementaton of IDisposable.   
   //// Call the virtual Dispose method.
   //// Supress Finalization.           
   //public void Dispose()
   //{
   //   Dispose(true);
   //   GC.SuppressFinalize(true);
   //}

   //// Virtual Dispose method 
   //protected virtual void Dispose(bool isDisposing)
   //{
   //   // Don't Dispose more then once 
   //   if(alreadyDisposed) 
   //   {
   //      return;
   //   }
   //   if(isDisposing)
   //   {
   //      // TO DO: free managed resources here. 
   //   }
   //   // TO DO: free unmanaged resources here.  
   //   /* */
   //   // tu bre sad pisi Close, Kill, Riliz...
   //   /* */

   //   // Set disposed flag. 
   //   alreadyDisposed = true;
   //}

   #endregion

   protected override void Dispose(bool disposing)
   {
      if(!this.alreadyDisposed)
      {
         try
         {
            if(disposing)
            {
               // Release the managed resources you added in
               // this derived class here.

               //addedManaged.Dispose();
            }
            // Release the native unmanaged resources you added
            // in this derived class here.

            //===================================================

            if(ZXC.TheMainDbConnection   != null) ZXC.TheMainDbConnection  .Close();
            if(ZXC.theSecondDbConnection != null) ZXC.theSecondDbConnection.Close(); // nemoj tu pozivaty propertyy nego koristi varijablu (malo slovo)
            if(ZXC.theThirdDbConnection  != null) ZXC.theThirdDbConnection .Close(); // nemoj tu pozivaty propertyy nego koristi varijablu (malo slovo)
            if(ZXC.theSkyDbConnection    != null) ZXC.theSkyDbConnection   .Close(); // nemoj tu pozivaty propertyy nego koristi varijablu (malo slovo)
            if(ZXC.theMbfDbConnection    != null) ZXC.theMbfDbConnection   .Close(); // nemoj tu pozivaty propertyy nego koristi varijablu (malo slovo)

            //===================================================

            this.alreadyDisposed = true;
         }
         finally
         {
            // Call Dispose on your base class.
            base.Dispose(disposing);
         }
      }
   }

   #endregion Implementing Dispose (cleanup 4 unmenidzd sorsis (dbConnecgtion))

   #region Util Methods

   public void SetDirtyFlag(object vvSender)
   {
      string vvSenderAsText = vvSender.ToString();

      bool hasChanges, transHasChanges, docOrKarticaHasChanges, polyTransHasChanges, extenderHasChanges;

      if(TheVvTabPage == null) return; // npr kod VvLoginForm-e 

      if(TheVvTabPage.WriteMode != ZXC.WriteMode.None)
      {
         TheVvUC.GetFields(true);
         docOrKarticaHasChanges = TheVvDataRecord.EditedHasChanges();

         if(TheVvDataRecord.IsExtendable && TheVvDataRecord.VirtualExtenderRecord.EditedHasChanges()) extenderHasChanges = true;
         else                                                                                         extenderHasChanges = false;

         if(TheVvDataRecord.IsDocument && ((VvDocumentRecord)TheVvDataRecord).EditedTransesHaveChanges()) transHasChanges = true;
         else                                                                                             transHasChanges = false;

         if(TheVvDataRecord.IsPolyDocument)
         {
            VvPolyDocumRecord vvPolyDocumRecord = (VvPolyDocumRecord)TheVvDataRecord;

            if(vvPolyDocumRecord.EditedTransesHaveChanges2() || vvPolyDocumRecord.EditedTransesHaveChanges3()) polyTransHasChanges = true;
            else                                                                                               polyTransHasChanges = false;
         }
         else
         {
            polyTransHasChanges = false;
         }

         hasChanges = docOrKarticaHasChanges || transHasChanges || polyTransHasChanges || extenderHasChanges;

         SetVvMenuEnabledOrDisabled_Explicitly("ESC", true);

         #region KOP Logic Additions

         // 16.11.2021: pri ADD operaciji, treba li onemoguciti sejvanja zaglavlja KOP-a jerbo nema stavaka: 

         bool isKOP = (TheVvDataRecord is Mixer) && (TheVvDataRecord as Mixer).TT == Mixer.TT_KOP; bool isNotKOP = !isKOP;
         if(isKOP && TheVvTabPage.WriteMode == ZXC.WriteMode.Add)
         {
            if((TheVvDataRecord as Mixer).Transes.Count.IsZero() && docOrKarticaHasChanges)
            {
               hasChanges = false;
            }
         }

         #endregion KOP Logic Additions

         TheVvTabPage.PaliGasiDirtyFlag(hasChanges);

         SetVvMenuEnabledOrDisabled_Explicitly("SAV", hasChanges); 
         SetVvMenuEnabledOrDisabled_Explicitly("SAS", hasChanges);
         SetVvMenuEnabledOrDisabled_Explicitly("SAK", hasChanges);

         if(TheVvTabPage.WriteMode != ZXC.WriteMode.Edit) SetVvMenuEnabledOrDisabled_Explicitly("SAS", false);
      }
      else // WriteMode OFF 
      {
         TheVvTabPage.PaliGasiDirtyFlag(false);

         SetVvMenuEnabledOrDisabled_Explicitly("ESC", false);
         SetVvMenuEnabledOrDisabled_Explicitly("SAV", false);
         SetVvMenuEnabledOrDisabled_Explicitly("SAS", false);
         SetVvMenuEnabledOrDisabled_Explicitly("SAK", false);
      }
   }

   private bool LockThisRecordForEdit(VvSQL.VvLockerInfo lockerInfo)
   {
      bool OK;

    //ucList.editorUID         = VvSQL.Get_Sql_UserID(conn, false);
      lockerInfo.editorUID         = ZXC.vvDB_User;
      lockerInfo.clientMachineName = Environment.MachineName;
      lockerInfo.clientUserName    = Environment.UserName /*ZXC.vvDB_User*/;

      OK = TheVvDao.InsertInLocker(TheDbConnection, lockerInfo);

      return OK;
   }

   private bool IsSomeoneElseAllreadyEditingThisRecord(VvSQL.VvLockerInfo lockerInfo)
   {
      if(TheVvDao.IsInLocker(TheDbConnection, lockerInfo, false))
      {
         TimeSpan timeSpan       = VvSQL.GetServer_DateTime_Now(TheDbConnection) - lockerInfo.inEditTS;
         int      elapsedSeconds = (int)timeSpan.TotalSeconds;
         int      timeOutSeconds = ZXC.vvLockTimeoutSeconds - elapsedSeconds;
         TimeSpan toTimeSpan     = new TimeSpan(timeOutSeconds * TimeSpan.TicksPerSecond);

         if(lockerInfo.editorUID         == ZXC.vvDB_User &&
            lockerInfo.clientMachineName == Environment.MachineName &&
            lockerInfo.clientUserName    == Environment.UserName)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop,
               "Zapis je u procesu izmjene na nekom prethodno otvorenom TabPage-u na Vašem računalu.\n\nUsnimite ili odustanite od promjene na tom prethodno otvorenom TabPage-u,\n\nkako bi 'otključali' zapis." +
               "\n\nTimeout in {0}", toTimeSpan);
         }
         else
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop,
               "Zapis je u procesu izmjene od strane korisnika <" +
               lockerInfo.editorUID + "@" + "(" +
               lockerInfo.clientMachineName + ":" +
               lockerInfo.clientUserName +
               ")>. Pricekajte, molim, obradu." +
               "\n\nTimeout in {0}", toTimeSpan);
         }
         return true;
      }

      return false;
   }

   private void OpenForWriteActions(ZXC.WriteMode writeMode)
   {
      TheVvUC.ControlForInitialFocus.Select();

      // 2.2.2011: reset warning colors on new record 
      if(TheVvUC is FakturDUC && writeMode == ZXC.WriteMode.Add) // 19.09.2023. ovdje dodemo sa editom kada radimo rtrano!!!!
      {
         TheVvTabPage.labDeviza.Visible = false;
         VvHamper.ApplyVVColorAndStyleTabCntrolChange(TheVvTabPage);
      }

      // 2.7.2010:
      if(TheVvUC.ControlForInitialFocus is VvTextBox)
      {
         VvTextBox vvTB = TheVvUC.ControlForInitialFocus as VvTextBox;

         vvTB.BeginEdit();
      }

      // tamara dodala 17.05.2011. zbog internih FakturDuc koji nemaju partnera nego im je dtp ControlForInitialFocus
      if(TheVvUC.ControlForInitialFocus is VvDateTimePicker)
      {
         VvDateTimePicker vvDtp = TheVvUC.ControlForInitialFocus as VvDateTimePicker;

         vvDtp.Visible = true;
         vvDtp.Focus();
         vvDtp.TheVvTextBox.Visible = false;
         SendKeys.Send("{RIGHT}");
      }
      // 16.12.2011: za IRM
      if(TheVvUC.ControlForInitialFocus is VvDataGridView)
      {
         if(writeMode == ZXC.WriteMode.Add ) SendKeys.Send("{TAB}");
      }

      TheVvTabPage.WriteMode = writeMode;
      if(writeMode == ZXC.WriteMode.Edit) BeginEdit(TheVvDataRecord); // save backupData in kejs of cancel 
      VvHamper.Open_Close_Fields_ForWriting(TheVvTabPage, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvRecordUC);
      SetVvMenuEnabledOrDisabled_RegardingWriteMode(writeMode);

      HidePredPrintTabPage();
      
      //SetDirtyFlag(writeMode);
      SetDirtyFlagDependentAppereance_NothingNewYet();

      // upali ovo ako pozelis da ti ControlForInitialFocus ne bude tamno-plavo-selektirano
      // to si nekada htio iz nekog razloga!? 
      //if(TheVvUC.ControlForInitialFocus is TextBox) 
      //{
      //   ((TextBox)(TheVvUC.ControlForInitialFocus)).DeselectAll();
      //}
      if(TheVvUC.ControlForInitialFocus is VvTextBox)
      {
         TheVvUC.ControlForInitialFocus.BackColor = ZXC.vvColors.vvTBoxHotOn_GotFocus_BackColor;

         VvTextBox vvTB = TheVvUC.ControlForInitialFocus as VvTextBox;

         if(TheVvUC is RNZDUC) // 08.10.2017: jer RNZu ide person kao prvi tb 
         {
            TheVvUC.SetSifrarAndAutocomplete<Person>(vvTB, VvSQL.SorterType.Person);
            vvTB.EH_HotOn_GotFocus(null, EventArgs.Empty);
         }
         // 07.10.2016: ovdje treba navesti svaki duc kojem je prvi TexBox koji dobiva focus Kupdob TextBox 
       //if(TheVvUC is FakturExtDUC                            )
         else if(TheVvUC is FakturExtDUC || TheVvUC is ZahtjevRNMDUC || TheVvUC is UgovoriDUC || TheVvUC is NazivArtiklaZaKupcaDUC)
         {
            TheVvUC.SetSifrarAndAutocomplete<Kupdob>(vvTB, VvSQL.SorterType.Name  );
            vvTB.EH_HotOn_GotFocus(null, EventArgs.Empty);
         }
      }

      // 06.03.2018: 
      TheVvUC.OpenCloseForWriting_AdditionalAction_UCspecific(writeMode, false);

   }

   private void CloseForWriteActions(bool isESC)
   {
      if(isESC) CancelEdit(TheVvDataRecord);
      else      EndEdit   (TheVvDataRecord);

      TheVvTabPage.WriteMode = ZXC.WriteMode.None;

      VvHamper.Open_Close_Fields_ForWriting(TheVvTabPage, ZXC.ZaUpis.Zatvoreno, isESC, ZXC.ParentControlKind.VvRecordUC);

      SetVvMenuEnabledOrDisabled_RegardingWriteMode(ZXC.WriteMode.None);

      SetDirtyFlag("CloseForWriteActions");

      PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);

      this.TStripStatusLabel.Text = this.statusTextBackup;

      TheVvRecordUC.tbx_DummyForDefaultFocus.Focus();

      // 06.03.2018: 
      TheVvUC.OpenCloseForWriting_AdditionalAction_UCspecific(ZXC.WriteMode.None, isESC);

   }

   internal /*private*/ void WhenRecordInDBHasChangedAction()
   {
      if(!TheVvDao.SetMe_Record_byRecID(TheDbConnection, TheVvDataRecord, TheVvDataRecord.VirtualRecID, false, TheVvTabPage.FileIsEmpty)) // probaj osvjeziti record 
      {
         TakeNewPositionAndPutFields(); // record vise ne postoji pa idemo nekam 
      }
      else
      {
         PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);
      }
   }

   #region BeginEndCancel Edit

   internal void BeginEdit(VvDataRecord vvDataRecord)
   {
      vvDataRecord.BeginEdit();

      if(vvDataRecord.IsExtendable)
      {
         vvDataRecord.VirtualExtenderRecord.BeginEdit();
      }

      if(vvDataRecord.IsDocument)
      {
         foreach(VvTransRecord trans_rec in ((VvDocumentRecord)vvDataRecord).VirtualTranses)
         {
            trans_rec.BeginEdit();
         }

         if(vvDataRecord.IsPolyDocument)
         {
            foreach(VvTransRecord trans_rec in ((VvPolyDocumRecord)vvDataRecord).VirtualTranses2)
            {
               trans_rec.BeginEdit();
            }
            foreach(VvTransRecord trans_rec in ((VvPolyDocumRecord)vvDataRecord).VirtualTranses3)
            {
               trans_rec.BeginEdit();
            }
         } // if(vvDataRecord.IsPolyDocument) 

      } // if(vvDataRecord.IsDocument) 
   }

   public void EndEdit(VvDataRecord vvDataRecord)
   {
      vvDataRecord.EndEdit();

      if(vvDataRecord.IsExtendable)
      {
         vvDataRecord.VirtualExtenderRecord.EndEdit();
      }

      if(vvDataRecord.IsDocument)
      {
         foreach(VvTransRecord trans_rec in ((VvDocumentRecord)vvDataRecord).VirtualTranses)
         {
            trans_rec.EndEdit();
         }

         if(vvDataRecord.IsPolyDocument)
         {
            foreach(VvTransRecord trans_rec in ((VvPolyDocumRecord)vvDataRecord).VirtualTranses2)
            {
               trans_rec.EndEdit();
            }
            foreach(VvTransRecord trans_rec in ((VvPolyDocumRecord)vvDataRecord).VirtualTranses3)
            {
               trans_rec.EndEdit();
            }
         } // if(vvDataRecord.IsPolyDocument) 

      } // if(vvDataRecord.IsDocument) 
   }

   internal void CancelEdit(VvDataRecord vvDataRecord)
   {
      vvDataRecord.CancelEdit();

      if(vvDataRecord.IsExtendable)
      {
         vvDataRecord.VirtualExtenderRecord.CancelEdit();
      }

      if(vvDataRecord.IsDocument)
      {
         foreach(VvTransRecord trans_rec in ((VvDocumentRecord)vvDataRecord).VirtualTranses)
         {
            trans_rec.CancelEdit();
         }

         if(TheVvDocumentRecordUC.TheG.CurrentCell != null && TheVvDocumentRecordUC.TheG.CurrentCell.IsInEditMode)
         {
            TheVvDocumentRecordUC.TheG.EndEdit();
         }

         if(vvDataRecord.IsPolyDocument)
         {
            foreach(VvTransRecord trans_rec in ((VvPolyDocumRecord)vvDataRecord).VirtualTranses2)
            {
               trans_rec.CancelEdit();
            }
            if(TheVvPolyDocumRecordUC.TheG2             != null && 
               TheVvPolyDocumRecordUC.TheG2.CurrentCell != null && 
               TheVvPolyDocumRecordUC.TheG2.CurrentCell.IsInEditMode)
            {
               TheVvPolyDocumRecordUC.TheG2.EndEdit();
            }

            foreach(VvTransRecord trans_rec in ((VvPolyDocumRecord)vvDataRecord).VirtualTranses3)
            {
               trans_rec.CancelEdit();
            }
            if(TheVvPolyDocumRecordUC.TheG3             != null &&
               TheVvPolyDocumRecordUC.TheG3.CurrentCell != null && 
               TheVvPolyDocumRecordUC.TheG3.CurrentCell.IsInEditMode)
            {
               TheVvPolyDocumRecordUC.TheG3.EndEdit();
            }

         } // if(vvDataRecord.IsPolyDocument) 

      } // if(vvDataRecord.IsDocument) 
   }

   #endregion BeginEndCancel Edit 

   private string TheVvDocumDataRecordDocumType { get { return TheVvDataRecord.IsDocumentLike ? ((VvDocumLikeRecord)TheVvDataRecord).VirtualTT : ""; } }

   private void TakeNewPositionAndPutFields()
   {
      bool OK;

      OK = TheVvDao.FrsPrvNxtLst_REC(TheDbConnection, TheVvDataRecord, VvSQL.DBNavigActionType.PRV, /*TheVvDataRecord.DefaultSorter*/ TheVvRecordUC.ThePrefferedRecordSorter, IsArhivaTabPage, TheVvUC.VvNavRestrictor_TT, TheVvUC.VvNavRestrictor_SKL, TheVvUC.VvNavRestrictor_SKL2);

      if(!OK)
      {
         OK = TheVvDao.FrsPrvNxtLst_REC(TheDbConnection, TheVvDataRecord, VvSQL.DBNavigActionType.NXT, /*TheVvDataRecord.DefaultSorter*/ TheVvRecordUC.ThePrefferedRecordSorter, IsArhivaTabPage, TheVvUC.VvNavRestrictor_TT, TheVvUC.VvNavRestrictor_SKL, TheVvUC.VvNavRestrictor_SKL2);
      }

      if(OK)
      {
         PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);
      }
      else
      {
         TheVvTabPage.FileIsEmpty = true;
         //VvHamper.ClearFieldContents(TheVvUC);
         ClearAllVvUcFields();
      }
   }

   public void PutFieldsActions(XSqlConnection _dbConnection, VvDataRecord _vvDataRecord, VvRecordUC _vvRecordUC)
   {
      if(TheVvTabPage.FileIsEmpty) return;

      // 13.05.2019: opkoljeno if()-om, jer ako JE AlertRised tu dolazimo sa vec napunjenim VvDataRecord-om 
      if(ZXC.VvXmlDR_LastDocumentMissing_AlertRaised == false)
      {
         // 18.05.2022: zamjenili mjesta LoadTranses i LoadExtender tako da LoadExtender dode prije LoadTranses jer nekad calcTranses treba TheEx 
         if(_vvDataRecord.IsExtendable == true) _vvDataRecord.VvDao.LoadExtender(_dbConnection,                   _vvDataRecord, IsArhivaTabPage);
         else                                   _vvDataRecord.VirtualExtenderRecord = null;
         if(_vvDataRecord.IsDocument   == true) _vvDataRecord.VvDao.LoadTranses (_dbConnection, (VvDocumentRecord)_vvDataRecord, IsArhivaTabPage);

         // 17.05.2022:
         if(TheVvUC is UGNorAUN_PTG_DUC)
         {
            // 17.02.2025: treba li ovo mozda i za DIZ, PVR, ZIZ?! 
            (TheVvUC as UGNorAUN_PTG_DUC).PtgUgovor_rec.TakeDataFromFaktur(_vvDataRecord as Faktur);
         }
      }
      // 13.10.2022: 
      else // SHIT HAPPEND pa necemo extender i transove 
      {
         ZXC.aim_emsg(MessageBoxIcon.Information, "LoadExtender & LoadTranses omitted zbog VvXmlDR_LastDocumentMissing_AlertRaised");
      }

      this.PutFieldsInProgress = true;
      _vvRecordUC.PutFields(_vvDataRecord); // This is it!!! 
      this.PutFieldsInProgress = false;

      Arhiva_EnableOrDisable_TSButtonsAndTSMnItems(_dbConnection, _vvDataRecord);

      SubModulSet_EnableOrDisable_TSButtonsAndTSMnItems();

      // 31.10.2012:
      if(ZXC.RepairMissingFakturEx_InProgress)
      {
         ZXC.RepairMissingFakturEx_InProgress = false;
         WhenRecordInDBHasChangedAction();
      }

#if DEBUG
      //ZXC.aim_emsg(TheVvDataRecord.SkyOperation.ToString());
#endif

      // 11.12.2017: 
      if(ZXC.IsTEXTHOany && TheVvUC is NivelacijaDUC)
      {
         NivelacijaDUC zpcDUC = TheVvUC as NivelacijaDUC;
         int dupErrCount;

         do
         {
            dupErrCount = zpcDUC.faktur_rec.TransDuplicatesCount();

            if(dupErrCount.NotZero())
            {
               DialogResult result = MessageBox.Show("Da li zelite izbrisati " + dupErrCount + " E_RtransDuplicates?",
                  "Ovaj ZPC ima E_RtransDuplicates?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

               if(result == DialogResult.Yes)
               {
                  int nora = VvDaoBase.SqlDeleteDuplicateTranses(TheDbConnection, ZXC.MySqlCheck_Kind.E_Rtrans_duplicates, zpcDUC.faktur_rec.RecID);
                  ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo.\n\nPobrisao " + nora + " duplikata.");
                  WhenRecordInDBHasChangedAction();
               }
               else dupErrCount = 0;
            }
         } while(dupErrCount.NotZero());
      }
   }

   private bool ForbiddenDocument_TT()
   {
      if( TheVvDataRecord.IsDocumentLike        == false || 
         (TheVvRecordUC is VvDocumLikeRecordUC) == false ) return false;

      bool isForbidden = false;
      
      // TODO: is it forbidden? 

      return isForbidden;
   }

   #endregion Util Methods

   #region Arhiva Buttons And Fields

   private void Arhiva_EnableOrDisable_TSButtonsAndTSMnItems(XSqlConnection _dbConnection, VvDataRecord _vvDataRecord)
   {
      // Ekran aktivnih ('normalnih') rekorda a (IsArhivable && VirtualAddTS != VirtualModTS) == true 
      if(_vvDataRecord.HasSenseCheckingForArhivaVersionExistance && IsArhivaTabPage == false)
      {
         _vvDataRecord.VvArhivedVersionsCount = _vvDataRecord.VvDao.GetVvArhivedVersionsCount(_dbConnection, _vvDataRecord);

         if(_vvDataRecord.VvArhivedVersionsCount > 0)  EnableOrDisable_PRV_NXT_ArhivaButtons(true,  false);
         else                                          EnableOrDisable_PRV_NXT_ArhivaButtons(false, false);

         uint activeRecordVersion = VvDaoBase.GetNextArhivaRecordVersion(_dbConnection, _vvDataRecord.VirtualRecordNameArhiva, _vvDataRecord.VirtualRecID);

         if(activeRecordVersion > 1)
            TheVvTabPage.Fld_Arhiva = " Ver: (" + activeRecordVersion + ")";
         else
            TheVvTabPage.Fld_Arhiva = "";
      }
      // Ekran aktivnih ('normalnih') rekorda a (IsArhivable && VirtualAddTS != VirtualModTS) == false 
      else if(IsArhivaTabPage == false)
      {
         EnableOrDisable_PRV_NXT_ArhivaButtons(false, false);

         TheVvTabPage.Fld_Arhiva = "";
      }
      // Ekran arhiviranih rekorda 
      else 
      {
         PutArhivaFields(_vvDataRecord);

         bool enablePRV = _vvDataRecord.VvDao.DoesExists_PRV_NXT_ArhivedVersions(_dbConnection, _vvDataRecord, VvSQL.DBNavigActionType.PRV);
         bool enableNXT = _vvDataRecord.VvDao.DoesExists_PRV_NXT_ArhivedVersions(_dbConnection, _vvDataRecord, VvSQL.DBNavigActionType.NXT);

         if(enableNXT == false) // vidi ima li ga kao aktivnoga 
         {
            if(_vvDataRecord.VvDao.DoesExists_ActiveVersion(_dbConnection, _vvDataRecord)) enableNXT = true;
         }

         EnableOrDisable_PRV_NXT_ArhivaButtons(enablePRV, enableNXT);

      }

      // Ovo gore je za PRV_NXT logic, a ovo dole za 'glavni' arhiva button (onaj bez strelica) 
      if(_vvDataRecord.IsArhivable == false || IsArhivaTabPage == true)
      {
         EnableOrDisable_ArhivaButton(false);
      }
      else if(IsArhivaTabPage == false)
      {
         if(TheVvTabPage.ArhivaTableIsNotEmpty) EnableOrDisable_ArhivaButton(true);
      }
   }

   private void PutArhivaFields(VvDataRecord _vvDataRecord)
   {
      TheVvTabPage.Fld_Arhiva = "[ (Ver: " +_vvDataRecord.TheArhivaData._recVer + ") " + _vvDataRecord.TheArhivaData._arAction + " / " + _vvDataRecord.TheArhivaData._arTS + " / " + _vvDataRecord.TheArhivaData._arUID + "]";
   }

   private void EnableOrDisable_PRV_NXT_ArhivaButtons(bool enableArOld, bool enableArNew)
   {
      ts_Record.Items["AOLD"].Enabled = aTopMenuItem[0].DropDown.Items["AOLD"].Enabled = enableArOld;
      ts_Record.Items["ANEW"].Enabled = aTopMenuItem[0].DropDown.Items["ANEW"].Enabled = enableArNew;
   }

   private void EnableOrDisable_ArhivaButton(bool enabled)
   {
      ts_Record.Items["ARH"].Enabled = aTopMenuItem[0].DropDown.Items["ARH"].Enabled = enabled;
   }
   
   #endregion Arhiva Buttons And Fields

   #region Virtual Methods (Other projects are overriders)

   // Daj provjeri jel ti ove 'kada ti treba vvDB_name prije constructora VvForma' metode jos trebaju, 
   // buduci da si promijenio raspored kada se zove InitializeVvForm() u VvForm.cs 

   public virtual string Get_MyDocumentsLocation_ProjectAndUser_Dependent(bool isUserDependent)
   {
      return GetMyDocumentsLocation(ZXC.vv_PRODUCT_Name, isUserDependent);
   }

   public static string GetMyDocumentsLocation(string vv_PRODUCT_Name, bool isUserDependent)
   {
      string theDirectory;
      string myDocumentsDirectory;

      // 10.12.2016: dodan try-catch 

      try
      {
         myDocumentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "ERROR:\n\n GetMyDocumentsLocation - [0]", ex.Message);
         myDocumentsDirectory = @"C:\";
      }

      theDirectory =
         myDocumentsDirectory +
         @"\Viper.NET\" +
         /*ZXC.*/vv_PRODUCT_Name +
         (isUserDependent ? @"\" + ZXC.vvDB_User : "");

      if(!Directory.Exists(theDirectory))
      {
         CreateDirectoryInMyDocuments(theDirectory);
      }

      return theDirectory;
   }

   public static void CreateDirectoryInMyDocuments(string theDirectory)
   {
      try { DirectoryInfo di = Directory.CreateDirectory(theDirectory); }
      catch(Exception e) { ZXC.aim_emsg("Ne mogu kreirati directory: [{0}]\n\n{1}", theDirectory, e.ToString()); }
      finally { }
   }

   // C6 (Phase 1a): ove metode SAMO bounceaju na ZXC (nema overrida u projektu).
   // Zadrzane jer ih VvForm partial fileovi jos uvijek internall koriste; TODO Fazu 1f: zamijeniti sve interne pozive izravnim ZXC pristupom i onda ih ukloniti.
   public virtual string GetvvDB_prjktDB_name()
   {
      return ZXC.VvDB_prjktDB_Name;
   }

   public virtual string Getvv_PRODUCT_name()
   {
      return ZXC.vv_PRODUCT_Name;
   }

   public virtual string GetvvDB_prefix()
   {
      return ZXC.vvDB_prefix;
   }

   protected virtual void ViperProjectDependent_DataAccessObjects(XSqlConnection tempConnection, string dbName)
   {
      foreach(string vvDataRecordName in ZXC.VvPUG_RecordNamesList)
      {

//#if(!DEBUG) // Znaci ovo je _RELEASE_ 
//         if(vvDataRecordName == Mixer.recordName ||
//            vvDataRecordName == Mixer.recordNameArhiva ||
//            vvDataRecordName == Xtrans.recordName ||
//            vvDataRecordName == Xtrans.recordNameArhiva
//         ) continue;
//#else
//         if(ZXC.initialPrjktKCD != 1) continue; // Dok Razvijamo, SkladRec radimo samo na Projektu '000001' 
//#endif

         VvSQL.CheckTableVersion_AndCatchUpIfNeeded(tempConnection, dbName, vvDataRecordName);
      }

      ZXC.KplanDao  = KplanDao .Instance(tempConnection, dbName);
      ZXC.NalogDao  = NalogDao .Instance(tempConnection, dbName);
      ZXC.FtransDao = FtransDao.Instance(tempConnection, dbName);
      ZXC.KupdobDao = KupdobDao.Instance(tempConnection, dbName);
      ZXC.OsredDao  = OsredDao .Instance(tempConnection, dbName);
      ZXC.AmortDao  = AmortDao .Instance(tempConnection, dbName);
      ZXC.AtransDao = AtransDao.Instance(tempConnection, dbName);
      ZXC.PersonDao = PersonDao.Instance(tempConnection, dbName);
      ZXC.PlacaDao  = PlacaDao .Instance(tempConnection, dbName);
      ZXC.PtransDao = PtransDao.Instance(tempConnection, dbName);
      ZXC.PtraneDao = PtraneDao.Instance(tempConnection, dbName);
      ZXC.PtranoDao = PtranoDao.Instance(tempConnection, dbName);

      ZXC.ArtiklDao = ArtiklDao .Instance(tempConnection, dbName);
      ZXC.ArtStatDao= ArtStatDao.Instance(tempConnection, dbName);
      ZXC.FakturDao = FakturDao .Instance(tempConnection, dbName);
      ZXC.FaktExDao = FaktExDao .Instance(tempConnection, dbName);
      ZXC.RtransDao = RtransDao .Instance(tempConnection, dbName);
      ZXC.RtranoDao = RtranoDao .Instance(tempConnection, dbName);

//#if(DEBUG) // Znaci ovo je _DEBUG_
      ZXC.MixerDao  = MixerDao .Instance(tempConnection, dbName);
      ZXC.XtransDao = XtransDao.Instance(tempConnection, dbName);
      ZXC.XtranoDao = XtranoDao.Instance(tempConnection, dbName);
//#endif
   }

   public virtual VvUserControl CreateTheVvUserControl_SwitchSubModulEnum(VvDataRecord vvDataRecord, Control panelZaUC, VvSubModul vvSubModul)
   {
      VvUserControl theVvUC = null;

      switch(vvSubModul.subModulEnum)
      {
         case ZXC.VvSubModulEnum.PRJ         : theVvUC = new PrjktUC             (panelZaUC, (Prjkt)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.USR         : theVvUC = new UserUC              (panelZaUC, (User)   vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.PRV         : theVvUC = new PrvlgUC             (panelZaUC, (Prvlg)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.KID         : theVvUC = new KupdobUC            (panelZaUC, (Kupdob) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.KPL         : theVvUC = new KplanUC             (panelZaUC, (Kplan)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.NAL_F       : theVvUC = new NalogFirmaDUC       (panelZaUC, (Nalog)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.NAL_O       : theVvUC = new NalogObrtDUC        (panelZaUC, (Nalog)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.NAL_M       : theVvUC = new NalogMtrDUC         (panelZaUC, (Nalog)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.NAL_P       : theVvUC = new NalogProjektDUC     (panelZaUC, (Nalog)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.OSR         : theVvUC = new OsredUC             (panelZaUC, (Osred)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.AMO         : theVvUC = new AmortDUC            (panelZaUC, (Amort)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.PER         : theVvUC = new PersonUC            (panelZaUC, (Person) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.PLA         : theVvUC = new PlacaDUC            (panelZaUC, (Placa)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.PLA_2014    : theVvUC = new Placa2014DUC        (panelZaUC, (Placa)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.PLA_2024    : theVvUC = new PlacaOd2024DUC      (panelZaUC, (Placa)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.PLA_NP      : theVvUC = new PlacaNPDUC          (panelZaUC, (Placa)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.DTEC        : theVvUC = new DevTecUC            (panelZaUC, (DevTec2) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.ART         : theVvUC = new ArtiklUC            (panelZaUC, (Artikl) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_UFA       : theVvUC = new UFADUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_UPA       : theVvUC = new UPADUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_UFM       : theVvUC = new UFMDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_URA       : theVvUC = new URADUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_URA_SVD   : theVvUC = new URA_SVD_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_URP       : theVvUC = new URPDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_URM       : theVvUC = new URMDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_URM_2     : theVvUC = new URMDUC_2            (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_URM_D     : theVvUC = new URMDUC_Dev          (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IFA       : theVvUC = new IFADUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IRM       : theVvUC = new IRMDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IRM_2     : theVvUC = new IRMDUC_2            (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IZM       : theVvUC = new IZMDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IZM_2     : theVvUC = new IZMDUC_2            (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_MSI       : theVvUC = new MedjuSkladDUC       (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_MSI_2     : theVvUC = new MedjuSklad2DUC      (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_VMI       : theVvUC = new MedjuSkladVMIuDUC   (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_VMI_2     : theVvUC = new MedjuSkladVMI2DUC   (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_MVI       : theVvUC = new MedjuSkladMVIDUC    (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_MVI_2     : theVvUC = new MedjuSkladMVI2DUC   (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_MMI       : theVvUC = new MedjuSkladMMIDUC    (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PIZ       : theVvUC = new ProizvodnjaDUC      (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PIM       : theVvUC = new PIMDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_NOR       : theVvUC = new NORDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_BOR       : theVvUC = new BORDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PIZ_P     : theVvUC = new PIZpDUC             (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_CJ        : theVvUC = new CjenikDUC           (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_CJK       : theVvUC = new CjenikKupca_DUC     (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PST       : theVvUC = new PocetnoStanjeDUC    (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PSM       : theVvUC = new PocetnoStanjeMPDUC  (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_INV       : theVvUC = new InventuraDUC        (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_INM       : theVvUC = new InventuraMPDUC      (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PRI       : theVvUC = new PrimkaVpDUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PRI_P     : theVvUC = new PRIpDUC             (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_KLK       : theVvUC = new KalkulacijaMpDUC    (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_KKM       : theVvUC = new KKMDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_UPM       : theVvUC = new PovratDobMalDUC     (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_KLK_2     : theVvUC = new KalkulacijaMpDUC_2  (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_KLD       : theVvUC = new KalkulacijaMpDUC_Dev(panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_SKO       : theVvUC = new SkladOnlyDUC        (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IOD       : theVvUC = new OdobrKupcuDUC       (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IPV       : theVvUC = new PovratKupcaDUC      (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_TMK       : theVvUC = new KorTemeljnicaDUC    (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_ZPC       : theVvUC = new NivelacijaDUC       (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IRA       : theVvUC = new IRADUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IRA_2     : theVvUC = new IRADUC_2            (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IRA_MPC   : theVvUC = new IRA_MPC_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IRP       : theVvUC = new IRPDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IZD       : theVvUC = new IzdatnicaDUC        (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IZD_SVD   : theVvUC = new IZD_SVD_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IZD_MPC   : theVvUC = new IZD_MPC_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_ZAH_SVD   : theVvUC = new ZAH_SVD_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_UOD       : theVvUC = new OdobrDobavDUC       (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_UPV       : theVvUC = new PovratDobaDUC       (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IMT       : theVvUC = new IzdatnicaNaMjTRrDUC (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_OPN       : theVvUC = new ObvezPonudaDUC      (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PON       : theVvUC = new PonudaDUC           (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PON_MPC   : theVvUC = new PON_MPC_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_OPN_MPC   : theVvUC = new OPN_MPC_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PNM       : theVvUC = new PonMalDUC           (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_NRD       : theVvUC = new NarudzbaDobavDUC    (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_NRD_SVD   : theVvUC = new NRD_SVD_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_NRU       : theVvUC = new NarudzDobUvozDUC    (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_NRS       : theVvUC = new NarudzDobUslugaDUC  (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_NRK       : theVvUC = new NarudzbaKupcaDUC    (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_NRM       : theVvUC = new NRMDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_UPL       : theVvUC = new BlgUplatDUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_ISP       : theVvUC = new BlgIsplatDUC        (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_BUP       :
         case ZXC.VvSubModulEnum.R_ABU       : theVvUC = new BlgUplat_M_DUC      (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_BIS       : 
         case ZXC.VvSubModulEnum.R_ABI       : theVvUC = new BlgIsplat_M_DUC     (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
//       case ZXC.VvSubModulEnum .R_STU      : theVvUC = new StornoUlazaDUC      (panelZaUC, (Faktur)vvDataRecord, vvSubModul); break;
//       case ZXC.VvSubModulEnum .R_STI      : theVvUC = new StornoIzlazaDUC     (panelZaUC, (Faktur)vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_RVI       : theVvUC = new ReversDUC           (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_RVU       : theVvUC = new PovReversaDUC       (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_RNP       : theVvUC = new RNPDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_RNM       : theVvUC = new RNMDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_RNS       : theVvUC = new RNSDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_RNZ       : theVvUC = new RNZDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PRJ       : theVvUC = new PRJDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_UGO       : theVvUC = new UGODUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_UGO_PTG   : theVvUC = new UGO_PTG_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_ANU_PTG   : theVvUC = new ANU_PTG_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_KUG_PTG   : theVvUC = new KUG_PTG_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_A1_KUG_PTG: theVvUC = new A1_KUG_PTG_DUC      (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_A1_ANU_PTG: theVvUC = new A1_ANU_PTG_DUC      (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_DOD_PTG   : theVvUC = new DIZ_PTG_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PVR_PTG   : theVvUC = new PVR_PTG_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
       //case ZXC.VvSubModulEnum.R_PVD_PTG   : theVvUC = new PVD_PTG_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_ZIZ_PTG   : theVvUC = new ZIZ_PTG_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_MOD_PTG   : theVvUC = new MOD_PTG_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PRI_PTG   : theVvUC = new PRI_PTG_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_URA_PTG   : theVvUC = new URA_PTG_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IRA_PTG   : theVvUC = new IRA_PTG_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IZD_PTG   : theVvUC = new IZD_PTG_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_MSI_PTG   : theVvUC = new MSI_PTG_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PST_PTG   : theVvUC = new PST_PTG_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
       //case ZXC.VvSubModulEnum.R_KOP_PTG   : theVvUC = new KOP_PTG_DUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PR_DOD_PTG: theVvUC = new PRN_DIZ_PTG_DUC     (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PRI_bc    : theVvUC = new PrimkaBcDUC         (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PRI_POT   : theVvUC = new POT_DUC             (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_POU       : theVvUC = new POU_DUC             (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_ZAR       : theVvUC = new ZAR_DUC             (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_POI       : theVvUC = new POI_DUC             (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PPR       : theVvUC = new PredatUProizDUC     (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_POV       : theVvUC = new PovratInterDUC      (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PIP       : theVvUC = new PIPDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PRIdev    : theVvUC = new PrimkaDevDUC        (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_UFAdev    : theVvUC = new UFAdevDUC           (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_IFAdev    : theVvUC = new IFAdevDUC           (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_KIZ       : theVvUC = new KIZDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_PIK       : theVvUC = new PIKDUC              (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_TRI       : theVvUC = new TransformDUC        (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.R_WYR       : theVvUC = new WYRNDUC             (panelZaUC, (Faktur) vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_VIR       : theVvUC = new VirmanDUC           (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_RAS       : theVvUC = new RasterDUC           (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_RAS_B     : theVvUC = new RasterBDUC          (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_PNT       : theVvUC = new PutNalTuzDUC        (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_PNI       : theVvUC = new PutNalInoDUC        (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_PNL       : theVvUC = new LokoVoznjaDUC       (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_PNR       : theVvUC = new PutRadListDUC       (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_ZAH       : theVvUC = new ZahtjeviDUC         (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_ZAH_RNM   : theVvUC = new ZahtjevRNMDUC       (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_SMD       : theVvUC = new SmdDUC              (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_EVD       : theVvUC = new EvidencijaDUC       (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_URZ       : theVvUC = new UrudzbeniDUC        (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_RUG       : theVvUC = new UgovoriDUC          (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_RVR       : theVvUC = new RvrDUC              (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_RVR2      : theVvUC = new RvrDUC              (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_MVR       : theVvUC = new RvrMjesecDUC        (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_AVR       : theVvUC = new AvrDUC              (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_IRV       : theVvUC = new InterniRvrDUC       (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_EXC       : theVvUC = new ExtCjeniciDUC       (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_PMV       : theVvUC = new PmvDUC              (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.GFI_TSI     : theVvUC = new GFI_TSI_DUC         (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.STAT_NPF    : theVvUC = new Statistika_NPF_DUC  (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.PLAN        : theVvUC = new PlanDUC             (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_GST       : theVvUC = new KnjigaGostijuDUC    (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_ZLJ       : theVvUC = new ZLJ_DUC             (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_ZPG       : theVvUC = new ZPG_DUC             (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_RGC       : theVvUC = new RegistarCijeviDUC   (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_BMW       : theVvUC = new BmwDUC              (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_PNA       : theVvUC = new PredNrdDUC          (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.SKY         : theVvUC = new SkyRuleUC           (panelZaUC, (SkyRule)vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_NAK       : theVvUC = new NazivArtiklaZaKupcaDUC (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_KOP       : theVvUC = new KOP_PTG_DUC         (panelZaUC, (Mixer)  vvDataRecord, vvSubModul); break;
         case ZXC.VvSubModulEnum.X_KDC       : theVvUC = new KDCDUC              (panelZaUC, (Mixer)vvDataRecord, vvSubModul); break;

         default:
            ZXC.aim_emsg("switch 4 TheVvUC = new XyVvUC(panelZaUC, (XY)vvDataRecord, vvSubModul); Modul {0} nedovrsen!", vvSubModul.subModulEnum);
            break;
      }

      return theVvUC;
   }

   public virtual VvDataRecord CreateTheVvDataRecord_SwitchSubModulEnum(VvSubModul vvSubModul)
   {
      VvDataRecord vvDataRecord = null;

      switch(vvSubModul.subModulEnum)
      {
         case ZXC.VvSubModulEnum.PRJ:
         case ZXC.VvSubModulEnum.LsPRJ: vvDataRecord = new Prjkt(); break;
         case ZXC.VvSubModulEnum.USR:
         case ZXC.VvSubModulEnum.LsUSR: vvDataRecord = new User(); break;
         case ZXC.VvSubModulEnum.PRV:
         case ZXC.VvSubModulEnum.LsPRV: vvDataRecord = new Prvlg(); break;
         case ZXC.VvSubModulEnum.KID:
         case ZXC.VvSubModulEnum.LsKID: vvDataRecord = new Kupdob(); break;
         case ZXC.VvSubModulEnum.KPL:
         case ZXC.VvSubModulEnum.LsKPL: vvDataRecord = new Kplan(); break;
         case ZXC.VvSubModulEnum.NAL_F:
         case ZXC.VvSubModulEnum.NAL_O:
         case ZXC.VvSubModulEnum.NAL_M:
         case ZXC.VvSubModulEnum.NAL_P:
         case ZXC.VvSubModulEnum.LsNAL: vvDataRecord = new Nalog();  /*((Nalog)vvDataRecord).Transes = new List<Ftrans>();*/ break;
         case ZXC.VvSubModulEnum.OSR:
         case ZXC.VvSubModulEnum.LsOSR: vvDataRecord = new Osred(); break;
         case ZXC.VvSubModulEnum.AMO:
         case ZXC.VvSubModulEnum.LsAMO: vvDataRecord = new Amort();  /*((Amort)vvDataRecord).Transes = new List<Atrans>();*/ break;
         case ZXC.VvSubModulEnum.PER:
         case ZXC.VvSubModulEnum.LsPER: vvDataRecord = new Person(); break;
         case ZXC.VvSubModulEnum.PLA:
         case ZXC.VvSubModulEnum.PLA_2014:
         case ZXC.VvSubModulEnum.PLA_2024:
         case ZXC.VvSubModulEnum.PLA_NP:
         case ZXC.VvSubModulEnum.LsPLA: vvDataRecord = new Placa(); break;
         case ZXC.VvSubModulEnum.DTEC:
         case ZXC.VvSubModulEnum.LsDTEC: vvDataRecord = new DevTec2(); break;
         case ZXC.VvSubModulEnum.ART:
         case ZXC.VvSubModulEnum.LsART: vvDataRecord = new Artikl(); break;
         case ZXC.VvSubModulEnum.LsRTR: vvDataRecord = new Rtrans(); break;
         case ZXC.VvSubModulEnum.LsRTO: vvDataRecord = new Rtrano(); break;
         case ZXC.VvSubModulEnum.R_UFA:
         case ZXC.VvSubModulEnum.R_UPA:
         case ZXC.VvSubModulEnum.R_UFM:
         case ZXC.VvSubModulEnum.R_URA:
         case ZXC.VvSubModulEnum.R_URA_SVD:
         case ZXC.VvSubModulEnum.R_URP:
         case ZXC.VvSubModulEnum.R_URM:
         case ZXC.VvSubModulEnum.R_URM_2:
         case ZXC.VvSubModulEnum.R_URM_D:
         case ZXC.VvSubModulEnum.R_MSI:
         case ZXC.VvSubModulEnum.R_MSI_2:
         case ZXC.VvSubModulEnum.R_VMI:
         case ZXC.VvSubModulEnum.R_VMI_2:
         case ZXC.VvSubModulEnum.R_MVI:
         case ZXC.VvSubModulEnum.R_MVI_2:
         case ZXC.VvSubModulEnum.R_MMI:
         case ZXC.VvSubModulEnum.R_PIZ:
         case ZXC.VvSubModulEnum.R_PIM:
         case ZXC.VvSubModulEnum.R_NOR:
         case ZXC.VvSubModulEnum.R_BOR:
         case ZXC.VvSubModulEnum.R_PIZ_P:
         case ZXC.VvSubModulEnum.R_CJ:
         case ZXC.VvSubModulEnum.R_CJK:
         case ZXC.VvSubModulEnum.R_IFA:
         case ZXC.VvSubModulEnum.R_IRM:
         case ZXC.VvSubModulEnum.R_IRM_2:
         case ZXC.VvSubModulEnum.R_IZM:
         case ZXC.VvSubModulEnum.R_IZM_2:
         case ZXC.VvSubModulEnum.R_PRI:
         case ZXC.VvSubModulEnum.R_PRI_P:
         case ZXC.VvSubModulEnum.R_KLK:
         case ZXC.VvSubModulEnum.R_KKM:
         case ZXC.VvSubModulEnum.R_UPM:
         case ZXC.VvSubModulEnum.R_KLK_2:
         case ZXC.VvSubModulEnum.R_KLD:
         case ZXC.VvSubModulEnum.R_PST:
         case ZXC.VvSubModulEnum.R_PSM:
         case ZXC.VvSubModulEnum.R_INV:
         case ZXC.VvSubModulEnum.R_INM:
         case ZXC.VvSubModulEnum.R_SKO:
         case ZXC.VvSubModulEnum.R_IOD:
         case ZXC.VvSubModulEnum.R_IPV:
         case ZXC.VvSubModulEnum.R_TMK:
         case ZXC.VvSubModulEnum.R_ZPC:
         case ZXC.VvSubModulEnum.R_IRA:
         case ZXC.VvSubModulEnum.R_IRA_2:
         case ZXC.VvSubModulEnum.R_IRP:
         case ZXC.VvSubModulEnum.R_IZD:
         case ZXC.VvSubModulEnum.R_IZD_SVD:
         case ZXC.VvSubModulEnum.R_IZD_MPC:
         case ZXC.VvSubModulEnum.R_ZAH_SVD:
         case ZXC.VvSubModulEnum.R_UOD:
         case ZXC.VvSubModulEnum.R_UPV:
         case ZXC.VvSubModulEnum.R_IMT:
         case ZXC.VvSubModulEnum.R_OPN:
         case ZXC.VvSubModulEnum.R_PON:
         case ZXC.VvSubModulEnum.R_PNM:
         case ZXC.VvSubModulEnum.R_NRD:
         case ZXC.VvSubModulEnum.R_NRD_SVD:
         case ZXC.VvSubModulEnum.R_NRU:
         case ZXC.VvSubModulEnum.R_NRS:
         case ZXC.VvSubModulEnum.R_NRK:
         case ZXC.VvSubModulEnum.R_NRM:
         case ZXC.VvSubModulEnum.R_UPL:
         case ZXC.VvSubModulEnum.R_ISP:
         case ZXC.VvSubModulEnum.R_BUP:
         case ZXC.VvSubModulEnum.R_BIS:
         case ZXC.VvSubModulEnum.R_ABU:
         case ZXC.VvSubModulEnum.R_ABI:
         case ZXC.VvSubModulEnum.R_STU:
         case ZXC.VvSubModulEnum.R_STI:
         case ZXC.VvSubModulEnum.R_RVI:
         case ZXC.VvSubModulEnum.R_RVU:
         case ZXC.VvSubModulEnum.R_RNP:
         case ZXC.VvSubModulEnum.R_RNM:
         case ZXC.VvSubModulEnum.R_RNS:
         case ZXC.VvSubModulEnum.R_RNZ:
         case ZXC.VvSubModulEnum.R_PRJ:
         case ZXC.VvSubModulEnum.R_UGO:
         case ZXC.VvSubModulEnum.R_UGO_PTG:
         case ZXC.VvSubModulEnum.R_ANU_PTG:
         case ZXC.VvSubModulEnum.R_KUG_PTG:
         case ZXC.VvSubModulEnum.R_A1_KUG_PTG:
         case ZXC.VvSubModulEnum.R_A1_ANU_PTG:
         case ZXC.VvSubModulEnum.R_DOD_PTG:
         case ZXC.VvSubModulEnum.R_PVR_PTG:
       //case ZXC.VvSubModulEnum.R_PVD_PTG:
         case ZXC.VvSubModulEnum.R_ZIZ_PTG:
         case ZXC.VvSubModulEnum.R_MOD_PTG:
         case ZXC.VvSubModulEnum.R_URA_PTG:
         case ZXC.VvSubModulEnum.R_IRA_PTG:
         case ZXC.VvSubModulEnum.R_PRI_PTG:
         case ZXC.VvSubModulEnum.R_IZD_PTG:
         case ZXC.VvSubModulEnum.R_MSI_PTG:
         case ZXC.VvSubModulEnum.R_PST_PTG:
       //case ZXC.VvSubModulEnum.R_KOP_PTG:
         case ZXC.VvSubModulEnum.R_PR_DOD_PTG:
         case ZXC.VvSubModulEnum.R_PRI_bc:
         case ZXC.VvSubModulEnum.R_PPR:
         case ZXC.VvSubModulEnum.R_POV:
         case ZXC.VvSubModulEnum.R_PIP:
         case ZXC.VvSubModulEnum.R_IFAdev:
         case ZXC.VvSubModulEnum.R_UFAdev:
         case ZXC.VvSubModulEnum.R_PRIdev:
         case ZXC.VvSubModulEnum.R_KIZ:
         case ZXC.VvSubModulEnum.R_PIK:
         case ZXC.VvSubModulEnum.R_TRI:
         case ZXC.VvSubModulEnum.R_WYR:
         case ZXC.VvSubModulEnum.R_PON_MPC:
         case ZXC.VvSubModulEnum.R_OPN_MPC:
         case ZXC.VvSubModulEnum.R_IRA_MPC:
         case ZXC.VvSubModulEnum.R_PRI_POT:
         case ZXC.VvSubModulEnum.R_ZAR:
         case ZXC.VvSubModulEnum.R_POU:
         case ZXC.VvSubModulEnum.R_POI:
         case ZXC.VvSubModulEnum.LsFAK: vvDataRecord = new Faktur(); /*((Faktur)vvDataRecord).TheFaktExRecord = new FaktEx();*/ break;
         case ZXC.VvSubModulEnum.X_VIR:
         case ZXC.VvSubModulEnum.X_RAS:
         case ZXC.VvSubModulEnum.X_RAS_B:
         case ZXC.VvSubModulEnum.X_PNT:
         case ZXC.VvSubModulEnum.X_PNI:
         case ZXC.VvSubModulEnum.X_PNL:
         case ZXC.VvSubModulEnum.X_PNR:
         case ZXC.VvSubModulEnum.X_ZAH:
         case ZXC.VvSubModulEnum.X_ZAH_RNM:
         case ZXC.VvSubModulEnum.X_SMD:
         case ZXC.VvSubModulEnum.X_EVD:
         case ZXC.VvSubModulEnum.X_URZ:
         case ZXC.VvSubModulEnum.X_RUG:
         case ZXC.VvSubModulEnum.X_RGC:
         case ZXC.VvSubModulEnum.X_EXC:
         case ZXC.VvSubModulEnum.X_RVR:
         case ZXC.VvSubModulEnum.X_RVR2:
         case ZXC.VvSubModulEnum.X_MVR:
         case ZXC.VvSubModulEnum.X_AVR:
         case ZXC.VvSubModulEnum.X_IRV:
         case ZXC.VvSubModulEnum.X_PMV:
         case ZXC.VvSubModulEnum.X_GST:
         case ZXC.VvSubModulEnum.X_ZLJ:
         case ZXC.VvSubModulEnum.X_ZPG:
         case ZXC.VvSubModulEnum.X_NAK:
         case ZXC.VvSubModulEnum.GFI_TSI:
         case ZXC.VvSubModulEnum.STAT_NPF:
         case ZXC.VvSubModulEnum.PLAN:
         case ZXC.VvSubModulEnum.X_BMW:
         case ZXC.VvSubModulEnum.X_PNA:
         case ZXC.VvSubModulEnum.X_KOP:
         case ZXC.VvSubModulEnum.X_KDC:
         case ZXC.VvSubModulEnum.LsMIX: vvDataRecord = new Mixer(); break;
         case ZXC.VvSubModulEnum.LsKDC: vvDataRecord = new Xtrans(); break;
         case ZXC.VvSubModulEnum.SKY:
         case ZXC.VvSubModulEnum.LsSKY: vvDataRecord = new SkyRule(); break;

         default:
            if(vvSubModul.subModulEnum == ZXC.VvSubModulEnum.FORBIDDEN)
            {
               ZXC.aim_emsg("switch 4 vvDataRecord = new VvDataRecord(); Modul {0} nedovrsen!", vvSubModul.subModulEnum);
            }
            break;
      }

      return vvDataRecord;
   }

   public virtual VvRecLstUC CreateTheVvRecLstUC_SwitchSubModulEnum(VvSubModul vvSubModul, VvDataRecord vvDataRecord, VvRecLstUC initialRecLstUC, Panel panelZaUC)
   {
      VvRecLstUC   theVvUC      = null;

      string dbName = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects.DataBaseName;

      switch (vvSubModul.subModulEnum)
      {
         case ZXC.VvSubModulEnum.LsPRJ:
          //if(!TheVvTabPage.InitializeVvTabConnection(typeof(Prjkt)))                             return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, GetvvDB_prjktDB_name(), Prjkt.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else                        theVvUC = new PrjktListUC(panelZaUC, (Prjkt)vvDataRecord, vvSubModul);
            break;

         case ZXC.VvSubModulEnum.LsUSR:
          //if(!TheVvTabPage.InitializeVvTabConnection(typeof(User)))                              return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, GetvvDB_prjktDB_name(), User.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else                        theVvUC = new UserListUC(panelZaUC, (User)vvDataRecord, vvSubModul);
            break;

         case ZXC.VvSubModulEnum.LsPRV:
          //if(!TheVvTabPage.InitializeVvTabConnection(typeof(Prvlg)))                             return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, GetvvDB_prjktDB_name(), Prvlg.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else                        theVvUC = new PrvlgListUC(panelZaUC, (Prvlg)vvDataRecord, vvSubModul);
            break;
         case ZXC.VvSubModulEnum.LsSKY:
            //if(!TheVvTabPage.InitializeVvTabConnection(typeof(SkyRule)))                             return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, GetvvDB_prjktDB_name(), SkyRule.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else theVvUC = new SkyRuleListUC(panelZaUC, (SkyRule)vvDataRecord, vvSubModul);
            break;

         case ZXC.VvSubModulEnum.LsKPL:
          //if(!TheVvTabPage.InitializeVvTabConnection(typeof(Kplan)))              return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, Kplan.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else                        theVvUC = new KplanListUC(panelZaUC, (Kplan)vvDataRecord, vvSubModul);
            break;

         case ZXC.VvSubModulEnum.LsNAL:
          //if(!TheVvTabPage.InitializeVvTabConnection(typeof(Nalog)))             return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, Nalog.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else                        theVvUC = new NalogListUC(panelZaUC, (Nalog)vvDataRecord, vvSubModul);
            break;

         case ZXC.VvSubModulEnum.LsOSR:
          //if(!TheVvTabPage.InitializeVvTabConnection(typeof(Osred)))             return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, Osred.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else                        theVvUC = new OsredListUC(panelZaUC, (Osred)vvDataRecord, vvSubModul);
            break;
        
         case ZXC.VvSubModulEnum.LsAMO:
          //if(!TheVvTabPage.InitializeVvTabConnection(typeof(Amort)))             return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, Amort.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else                        theVvUC = new AmortListUC(panelZaUC, (Amort)vvDataRecord, vvSubModul);
            break;

         case ZXC.VvSubModulEnum.LsKID:
          //if(!TheVvTabPage.InitializeVvTabConnection(typeof(Kupdob)))             return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, Kupdob.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else                        theVvUC = new KupdobListUC(panelZaUC, (Kupdob)vvDataRecord, vvSubModul);
            break;
        
         case ZXC.VvSubModulEnum.LsPER:
            //if(!TheVvTabPage.InitializeVvTabConnection(typeof(Osred)))             return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, Person.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else                        theVvUC = new PersonListUC(panelZaUC, (Person)vvDataRecord, vvSubModul);
            break;

         case ZXC.VvSubModulEnum.LsPLA:
            //if(!TheVvTabPage.InitializeVvTabConnection(typeof(Osred)))             return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, Placa.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else                        theVvUC = new PlacaListUC(panelZaUC, (Placa)vvDataRecord, vvSubModul);
            break;
        
         case ZXC.VvSubModulEnum.LsDTEC:
          //if(!TheVvTabPage.InitializeVvTabConnection(typeof(Kplan)))              return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, GetvvDB_prjktDB_name(), DevTec2.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else                        theVvUC = new DevTecListUC(panelZaUC, (DevTec2)vvDataRecord, vvSubModul);
            break;

         case ZXC.VvSubModulEnum.LsART:
            //if(!TheVvTabPage.InitializeVvTabConnection(typeof(Kplan)))              return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, Artikl.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else                        theVvUC = new ArtiklListUC(panelZaUC, (Artikl)vvDataRecord, vvSubModul);
            break;

         case ZXC.VvSubModulEnum.LsRTR:
            //if(!TheVvTabPage.InitializeVvTabConnection(typeof(Kplan)))              return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, Rtrans.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else                        theVvUC = new RtransListUC(panelZaUC, (Rtrans)vvDataRecord, vvSubModul);
            break;

         case ZXC.VvSubModulEnum.LsRTO:
            //if(!TheVvTabPage.InitializeVvTabConnection(typeof(Kplan)))              return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, Rtrano.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else                        theVvUC = new RtranoListUC(panelZaUC, (Rtrano)vvDataRecord, vvSubModul);
            break;

         case ZXC.VvSubModulEnum.LsFAK:
            //if(!TheVvTabPage.InitializeVvTabConnection(typeof(Kplan)))              return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, Faktur.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else                        theVvUC = new FakturListUC(panelZaUC, (Faktur)vvDataRecord, vvSubModul, new VvSubModul(true));
            break;

         case ZXC.VvSubModulEnum.LsMIX:
            //if(!TheVvTabPage.InitializeVvTabConnection(typeof(Kplan)))              return null;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, Faktur.recordName)) return null;

            if(initialRecLstUC != null) theVvUC = TheVvTabPage.InitialRecLstUCFromVvFindDialog(initialRecLstUC);
            else                        theVvUC = new MixerListUC(panelZaUC, (Mixer)vvDataRecord, vvSubModul, new VvSubModul(true));
            break;

         default:
            ZXC.aim_emsg("Modul {0} nedovrsen!", vvSubModul.subModulEnum);
            break;
      }

      return theVvUC;
   }

   public virtual VvReportUC    CreateTheVvReportUC_SwitchSubModulEnum(out VvRptFilter vvRptFilter, Control panelZaUC, VvSubModul vvSubModul)
   {
      VvReportUC theVvUC = null;
      vvRptFilter = null;

      switch(vvSubModul.subModulEnum)
      {
         case ZXC.VvSubModulEnum.FIZ:
            //if(!TheVvTabPage.InitializeVvTabConnection(typeof(VvRpt_Fin_Filter))) return null;
            vvRptFilter = new VvRpt_Fin_Filter();
            theVvUC = new FinReportUC(panelZaUC, (VvRpt_Fin_Filter)vvRptFilter, vvSubModul);
            break;

         case ZXC.VvSubModulEnum.OIZ:
            //if(!TheVvTabPage.InitializeVvTabConnection(typeof(VvRpt_Osred_Filter))) return null;
            vvRptFilter = new VvRpt_Osred_Filter();
            theVvUC = new AmoReportUC(panelZaUC, (VvRpt_Osred_Filter)vvRptFilter, vvSubModul);
            break;

         case ZXC.VvSubModulEnum.KIZ:
            //if(!TheVvTabPage.InitializeVvTabConnection(typeof(VvRpt_Kupdob_Filter))) return null;
            vvRptFilter = new VvRpt_Kupdob_Filter();
            theVvUC = new KupdobReportUC(panelZaUC, (VvRpt_Kupdob_Filter)vvRptFilter, vvSubModul);
            break;

         case ZXC.VvSubModulEnum.PIZ:
            //if(!TheVvTabPage.InitializeVvTabConnection(typeof(VvRpt_Kupdob_Filter))) return null;
            vvRptFilter = new VvRpt_Placa_Filter();
            theVvUC     = new PlacaReportUC(panelZaUC, (VvRpt_Placa_Filter)vvRptFilter, vvSubModul);
            break;
        
         case ZXC.VvSubModulEnum.PRIZ:
            //if(!TheVvTabPage.InitializeVvTabConnection(typeof(VvRpt_Kupdob_Filter))) return null;
            vvRptFilter = new VvRpt_Kupdob_Filter();
            theVvUC     = new KupdobReportUC(panelZaUC, (VvRpt_Kupdob_Filter)vvRptFilter, vvSubModul);
            break;
         case ZXC.VvSubModulEnum.RIZ:
            //if(!TheVvTabPage.InitializeVvTabConnection(typeof(VvRpt_Kupdob_Filter))) return null;
            vvRptFilter = new VvRpt_RiSk_Filter();
            theVvUC     = new RiskReportUC(panelZaUC, (VvRpt_RiSk_Filter)vvRptFilter, vvSubModul);
            break;

         case ZXC.VvSubModulEnum.XIZ_P:
         case ZXC.VvSubModulEnum.XIZ_Z:
         case ZXC.VvSubModulEnum.XIZ_S:
         case ZXC.VvSubModulEnum.XIZ_E:
         case ZXC.VvSubModulEnum.XIZ_R:
         case ZXC.VvSubModulEnum.XIZ_G:
         case ZXC.VvSubModulEnum.XIZ_B:
            //if(!TheVvTabPage.InitializeVvTabConnection(typeof(VvRpt_Kupdob_Filter))) return null;
            vvRptFilter = new VvRpt_Mix_Filter();
            theVvUC     = new MixerReportUC(panelZaUC, (VvRpt_Mix_Filter)vvRptFilter, vvSubModul);
            break;
  
         default:
            ZXC.aim_emsg("Modul IZVJESTAJA {0} nedovrsen!", vvSubModul.subModulEnum);
            break;
      }

      return theVvUC;
   }

   public virtual string Create_table_definition(string dbName, string recordName)
   {
      return VvSQL.Create_table_definition(dbName, recordName);
   }

   public virtual uint GetLastNewestCurrentTableVersionForMetaData(string recordName)
   {
      return VvSQL.GetLastNewestCurrentTableVersionForMetaData(recordName);
   }

   public virtual string Alter_table_definition(string recordName, uint catchingVersion)
   {
      return VvSQL.Alter_table_definition(recordName, catchingVersion);
   }

   public virtual string GetALTER_TABLE_AddVvUserControl_Command(uint catchingVersion)
   {
      return VvUcList_AddNew.AddNewVvUserControl_CommandText_VEKTOR(catchingVersion);
   }

   public virtual LookUpItem_ListView_Dialog Create_LookUpItem_ListView_Dialog_ProjectDependent(VvTextBox vvtb)
   {
      LookUpItem_ListView_Dialog dlg = null;

      switch(vvtb.JAM_LookUpTableTitle)
      {
         case ZXC.luiListaPorPla_Name           : ZXC.luiListaPorPla           .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPorPla,            (Control)vvtb); break;

         case ZXC.luiListaNalogTT_Name          : ZXC.luiListaNalogTT          .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaNalogTT,           (Control)vvtb); break;
         case ZXC.luiListaSkladista_Name        : ZXC.luiListaSkladista        .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaSkladista,         (Control)vvtb); break;
         case ZXC.luiListaOpcina_Name           : ZXC.luiListaOpcina           .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaOpcina,            (Control)vvtb); break;
         case ZXC.luiListaIspostava_Name        : ZXC.luiListaIspostava        .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaIspostava,         (Control)vvtb); break;
         case ZXC.luiListaZupanija_Name         : ZXC.luiListaZupanija         .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaZupanija,          (Control)vvtb); break;
         case ZXC.luiListaPostaZupan_Name       : ZXC.luiListaPostaZupan       .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPostaZupan,        (Control)vvtb); break;
         case ZXC.luiListaDjelat_Name           : ZXC.luiListaDjelat           .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaDjelat,            (Control)vvtb); break;
         case ZXC.luiListaGrupaPartnera_Name    : ZXC.luiListaGrupaPartnera    .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaGrupaPartnera,     (Control)vvtb); break;
         case ZXC.luiListaGodina_Name           : ZXC.luiListaGodina           .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaGodina,            (Control)vvtb); break;
         case ZXC.luiListaPrvlgScope_Name       : ZXC.luiListaPrvlgScope       .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPrvlgScope,        (Control)vvtb); break;
         case ZXC.luiListaPrvlgType_Name        : ZXC.luiListaPrvlgType        .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPrvlgType,         (Control)vvtb); break;
         case ZXC.luiListaFakturType_Name       : ZXC.luiListaFakturType       .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaFakturType,        (Control)vvtb); break;
         case ZXC.luiListaAmortTT_Name          : ZXC.luiListaAmortTT          .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaAmortTT,           (Control)vvtb); break;
         case ZXC.luiListaPersonTS_Name         : ZXC.luiListaPersonTS         .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPersonTS,          (Control)vvtb); break;
         case ZXC.luiListaStrSprema_Name        : ZXC.luiListaStrSprema        .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaStrSprema,         (Control)vvtb); break;
         case ZXC.luiListaRadnoMjesto_Name      : ZXC.luiListaRadnoMjesto      .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaRadnoMjesto,       (Control)vvtb); break;
         case ZXC.luiListaPlacaTT_Name          : ZXC.luiListaPlacaTT          .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPlacaTT,           (Control)vvtb); break;
         case ZXC.luiListaFondSati_NOR_Name     : ZXC.luiListaFondSati_NOR     .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaFondSati_NOR,      (Control)vvtb); break;
         case ZXC.luiListaFondSati_TRG_Name     : ZXC.luiListaFondSati_TRG     .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaFondSati_TRG,      (Control)vvtb); break;
         case ZXC.luiListaPlacaVrstaObr_Name    : ZXC.luiListaPlacaVrstaObr    .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPlacaVrstaObr    , (Control)vvtb); break;
         case ZXC.luiListaVrstaRadaEVR_Name     : ZXC.luiListaVrstaRadaEVR     .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaVrstaRadaEVR     , (Control)vvtb); break;
         case ZXC.luiListaOsnovaOsigRsm_Name    : ZXC.luiListaOsnovaOsigRsm    .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaOsnovaOsigRsm    , (Control)vvtb); break;
         case ZXC.luiListaStazBRsm_Name         : ZXC.luiListaStazBRsm         .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaStazBRsm         , (Control)vvtb); break;
         case ZXC.luiListaDeviza_Name           : ZXC.luiListaDeviza           .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaDeviza           , (Control)vvtb); break;
         case ZXC.luiListaGrupa1Artikla_Name    : ZXC.luiListaGrupa1Artikla    .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaGrupa1Artikla    , (Control)vvtb); break;
         case ZXC.luiListaGrupa2Artikla_Name    : ZXC.luiListaGrupa2Artikla    .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaGrupa2Artikla    , (Control)vvtb); break;
         case ZXC.luiListaGrupa3Artikla_Name    : ZXC.luiListaGrupa3Artikla    .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaGrupa3Artikla    , (Control)vvtb); break;
         case ZXC.luiListaPdvKat_Name           : ZXC.luiListaPdvKat           .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPdvKat           , (Control)vvtb); break;
         case ZXC.luiListaArtiklTS_Name         : ZXC.luiListaArtiklTS         .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaArtiklTS         , (Control)vvtb); break;
         case ZXC.luiListaRiskZiroRn_Name       : ZXC.luiListaRiskZiroRn       .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaRiskZiroRn       , (Control)vvtb); break;
         case ZXC.luiListaRiskVrstaPl_Name      : ZXC.luiListaRiskVrstaPl      .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaRiskVrstaPl      , (Control)vvtb); break;
         case ZXC.luiListaRiskCjenik_Name       : ZXC.luiListaRiskCjenik       .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaRiskCjenik       , (Control)vvtb); break;
         case ZXC.luiListaRiskStatus_Name       : ZXC.luiListaRiskStatus       .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaRiskStatus       , (Control)vvtb); break;
         case ZXC.luiListaRiskPredefs_Name      : ZXC.luiListaRiskPredefs      .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaRiskPredefs      , (Control)vvtb); break;
         case ZXC.luiListaRiskTipOtprem_Name    : ZXC.luiListaRiskTipOtprem    .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaRiskTipOtprem    , (Control)vvtb); break;
         case ZXC.luiListaRiskJezikHrv_Name     : ZXC.luiListaRiskJezikHrv     .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaRiskJezikHrv     , (Control)vvtb); break;
         case ZXC.luiListaRiskJezikEng_Name     : ZXC.luiListaRiskJezikEng     .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaRiskJezikEng     , (Control)vvtb); break;
         case ZXC.luiListaRiskJezikC_Name       : ZXC.luiListaRiskJezikC       .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaRiskJezikC       , (Control)vvtb); break;
         case ZXC.luiListaRiskJezikD_Name       : ZXC.luiListaRiskJezikD       .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaRiskJezikD       , (Control)vvtb); break;
         case ZXC.luiListaRiskVodPrjkt_Name     : ZXC.luiListaRiskVodPrjkt     .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaRiskVodPrjkt     , (Control)vvtb); break;
         case ZXC.luiListaMixerType_Name        : ZXC.luiListaMixerType        .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaMixerType        , (Control)vvtb); break;
         case ZXC.luiListaMixerZadatak_Name     : ZXC.luiListaMixerZadatak     .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaMixerZadatak     , (Control)vvtb); break;
         case ZXC.luiListaMixerOdrediste_Name   : ZXC.luiListaMixerOdrediste   .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaMixerOdrediste   , (Control)vvtb); break;
         case ZXC.luiListaMixerVozilo_Name      : ZXC.luiListaMixerVozilo      .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaMixerVozilo      , (Control)vvtb); break;
         case ZXC.luiListaMixerRelacija_Name    : ZXC.luiListaMixerRelacija    .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaMixerRelacija    , (Control)vvtb); break;
         case ZXC.luiListaMixTypePutNal_Name    : ZXC.luiListaMixTypePutNal    .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaMixTypePutNal    , (Control)vvtb); break;
         case ZXC.luiListaMixTypeZahtjev_Name   : ZXC.luiListaMixTypeZahtjev   .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaMixTypeZahtjev   , (Control)vvtb); break;
         case ZXC.luiListaMixTypeEvidencija_Name: ZXC.luiListaMixTypeEvidencija.LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaMixTypeEvidencija, (Control)vvtb); break;
         case ZXC.luiListaMixDnevnice_Name      : ZXC.luiListaMixDnevnice      .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaMixDnevnice      , (Control)vvtb); break;
         case ZXC.luiListaMixRadVrijemeRVR_Name : ZXC.luiListaMixRadVrijemeRVR .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaMixRadVrijemeRVR , (Control)vvtb); break;
         case ZXC.luiListaMixRadVrijemRVR2_Name : ZXC.luiListaMixRadVrijemRVR2 .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaMixRadVrijemRVR2 , (Control)vvtb); break;
         case ZXC.luiListaKplanKlase_Name       : ZXC.luiListaKplanKlase       .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaKplanKlase       , (Control)vvtb); break;
         case ZXC.luiListaMix_GFI_TSI_Name      : ZXC.luiListaMix_GFI_TSI      .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaMix_GFI_TSI      , (Control)vvtb); break;
         case ZXC.luiListaMix_Statist_NPF_Name  : ZXC.luiListaMix_Statist_NPF  .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaMix_Statist_NPF  , (Control)vvtb); break;
         case ZXC.luiListaTSI_Podaci_Name       : ZXC.luiListaTSI_Podaci       .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaTSI_Podaci       , (Control)vvtb); break;
         case ZXC.luiListaGFI_Bilanca_Name      : ZXC.luiListaGFI_Bilanca      .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaGFI_Bilanca      , (Control)vvtb); break;
         case ZXC.luiListaGFI_RDG_Name          : ZXC.luiListaGFI_RDG          .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaGFI_RDG          , (Control)vvtb); break;
         case ZXC.luiListaGFI_PodDop_Name       : ZXC.luiListaGFI_PodDop       .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaGFI_PodDop       , (Control)vvtb); break;
         case ZXC.luiListaGFI_NT_I_Name         : ZXC.luiListaGFI_NT_I         .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaGFI_NT_I         , (Control)vvtb); break;
         case ZXC.luiListaGFI_NT_D_Name         : ZXC.luiListaGFI_NT_D         .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaGFI_NT_D         , (Control)vvtb); break;
         case ZXC.luiListaRtranoGr_Name         : ZXC.luiListaRtranoGr         .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaRtranoGr         , (Control)vvtb); break;
         case ZXC.luiListaNacIspl_Name          : ZXC.luiListaNacIspl          .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaNacIspl          , (Control)vvtb); break;
         case ZXC.luiListaNeoporPrim_Name       : ZXC.luiListaNeoporPrim       .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaNeoporPrim       , (Control)vvtb); break;
         case ZXC.luiListaStjecatelj_Name       : ZXC.luiListaStjecatelj       .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaStjecatelj       , (Control)vvtb); break;
         case ZXC.luiListaPrimDoh_Name          : ZXC.luiListaPrimDoh          .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPrimDoh          , (Control)vvtb); break;
         case ZXC.luiListaPocKraj_Name          : ZXC.luiListaPocKraj          .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPocKraj          , (Control)vvtb); break;
         case ZXC.luiListaVrstaIzvjJoppd_Name   : ZXC.luiListaVrstaIzvjJoppd   .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaVrstaIzvjJoppd   , (Control)vvtb); break;
         case ZXC.luiListaBrojSobe_Name         : ZXC.luiListaBrojSobe         .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaBrojSobe         , (Control)vvtb); break;
         case ZXC.luiListaMixRadVrijemeMVR_Name : ZXC.luiListaMixRadVrijemeMVR .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaMixRadVrijemeMVR , (Control)vvtb); break;
         case ZXC.luiListaDrzave_Name           : ZXC.luiListaDrzave           .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaDrzave           , (Control)vvtb); break;
         case ZXC.luiListaGranPrijelaz_Name     : ZXC.luiListaGranPrijelaz     .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaGranPrijelaz     , (Control)vvtb); break;
         case ZXC.luiListaVrstaGosta_Name       : ZXC.luiListaVrstaGosta       .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaVrstaGosta       , (Control)vvtb); break;
         case ZXC.luiListaStatusGosta_Name      : ZXC.luiListaStatusGosta      .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaStatusGosta      , (Control)vvtb); break;
         case ZXC.luiListaVrstaPutIsprave_Name  : ZXC.luiListaVrstaPutIsprave  .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaVrstaPutIsprave  , (Control)vvtb); break;
         case ZXC.luiListaTipObjekta_Name       : ZXC.luiListaTipObjekta       .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaTipObjekta       , (Control)vvtb); break;
         case ZXC.luiLista_PlanTT_Name          : ZXC.luiLista_PlanTT          .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiLista_PlanTT          , (Control)vvtb); break;
         case ZXC.luiListaPozicijePlana_Name    : ZXC.luiListaPozicijePlana    .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPozicijePlana    , (Control)vvtb); break;
         case ZXC.luiListaPozicijePlanaPLN_Name : ZXC.luiListaPozicijePlanaPLN .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPozicijePlanaPLN , (Control)vvtb); break;
         case ZXC.luiListaPozicijePlanaRLZ_Name : ZXC.luiListaPozicijePlanaRLZ .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPozicijePlanaRLZ , (Control)vvtb); break;
         case ZXC.luiListaPozicijePlanaPBN_Name : ZXC.luiListaPozicijePlanaPBN .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPozicijePlanaPBN , (Control)vvtb); break;
         case ZXC.luiListaFinFond_Name          : ZXC.luiListaFinFond          .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaFinFond          , (Control)vvtb); break;
         case ZXC.luiLista_PR_RAS_NPF_Name      : ZXC.luiLista_PR_RAS_NPF      .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiLista_PR_RAS_NPF      , (Control)vvtb); break;
         case ZXC.luiLista_S_PR_RAS_NPF_Name    : ZXC.luiLista_S_PR_RAS_NPF    .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiLista_S_PR_RAS_NPF    , (Control)vvtb); break;
         case ZXC.luiLista_BIL_NPF_Name         : ZXC.luiLista_BIL_NPF         .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiLista_BIL_NPF         , (Control)vvtb); break;
         case ZXC.luiLista_G_PR_IZ_NPF_Name     : ZXC.luiLista_G_PR_IZ_NPF     .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiLista_G_PR_IZ_NPF     , (Control)vvtb); break;
         case ZXC.luiListaKlasifikALL_Name      : ZXC.luiListaKlasifikALL      .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaKlasifikALL      , (Control)vvtb); break;
         case ZXC.luiListaKlasifikJOB_Name      : ZXC.luiListaKlasifikJOB      .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaKlasifikJOB      , (Control)vvtb); break;
         case ZXC.luiListaVrstaUgovora_Name     : ZXC.luiListaVrstaUgovora     .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaVrstaUgovora     , (Control)vvtb); break;
         case ZXC.luiListaSerlot_Name           : ZXC.luiListaSerlot           .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaSerlot           , (Control)vvtb); break;
         case ZXC.luiListaVrstaRNM_Name         : ZXC.luiListaVrstaRNM         .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaVrstaRNM         , (Control)vvtb); break;
         case ZXC.luiListaOutlookItems_Name     : ZXC.luiListaOutlookItems     .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaOutlookItems     , (Control)vvtb); break;
         case ZXC.luiListaMixTypeZastitari_Name : ZXC.luiListaMixTypeZastitari .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaMixTypeZastitari , (Control)vvtb); break;
         case ZXC.luiListaeRacPoslProc_Name     : ZXC.luiListaeRacPoslProc     .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaeRacPoslProc     , (Control)vvtb); break;
         case ZXC.luiListaFakRptUniFilter_Name  : ZXC.luiListaFakRptUniFilter  .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaFakRptUniFilter  , (Control)vvtb); break;
         case ZXC.luiListaPTG_NajamNaRok_Name   : ZXC.luiListaPTG_NajamNaRok   .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPTG_NajamNaRok   , (Control)vvtb); break;
         case ZXC.luiListaPTG_VrstaNajma_Name   : ZXC.luiListaPTG_VrstaNajma   .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPTG_VrstaNajma   , (Control)vvtb); break;
         case ZXC.luiListaPTG_DanZaFaktur_Name  : ZXC.luiListaPTG_DanZaFaktur  .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPTG_DanZaFaktur  , (Control)vvtb); break;
         case ZXC.luiListaPTG_SlanjeRacuna_Name : ZXC.luiListaPTG_SlanjeRacuna .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPTG_SlanjeRacuna , (Control)vvtb); break;
         case ZXC.luiListaPTG_OsigPlacanja_Name : ZXC.luiListaPTG_OsigPlacanja .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPTG_OsigPlacanja , (Control)vvtb); break;
         case ZXC.luiListaGeonomenklatura_Name  : ZXC.luiListaGeonomenklatura  .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaGeonomenklatura  , (Control)vvtb); break;
         case ZXC.luiListaIncoterms_Name        : ZXC.luiListaIncoterms        .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaIncoterms        , (Control)vvtb); break;
         case ZXC.luiListaIntrastVrPosla_Name   : ZXC.luiListaIntrastVrPosla   .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaIntrastVrPosla   , (Control)vvtb); break;
         case ZXC.luiListaIntrastVrProm_Name    : ZXC.luiListaIntrastVrProm    .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaIntrastVrProm    , (Control)vvtb); break;
         case ZXC.luiListaIntrastIsporuka_Name  : ZXC.luiListaIntrastIsporuka  .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaIntrastIsporuka  , (Control)vvtb); break;
         case ZXC.luiListaPCKpricesPerGB_Name   : ZXC.luiListaPCKpricesPerGB   .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaPCKpricesPerGB   , (Control)vvtb); break;
         case ZXC.luiListaZIZ_TT_Name           : ZXC.luiListaZIZ_TT           .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaZIZ_TT           , (Control)vvtb); break;
         case ZXC.luiListaKPD2025_Name          : ZXC.luiListaKPD2025          .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaKPD2025          , (Control)vvtb); break;
         case ZXC.luiListaKodTipaEracuna_Name   : ZXC.luiListaKodTipaEracuna   .LazyLoad(); dlg = new LookUpItem_ListView_Dialog(ZXC.luiListaKodTipaEracuna   , (Control)vvtb); break;

         default: ZXC.aim_emsg("LookUpTable [{0}] undone in VvUserControl.UpdateVvLookUpItem()!!!", vvtb.JAM_LookUpTableTitle); break;

      } //switch(vvtb.JAM_LookUpTableTitle)

      return dlg;
   }

   public virtual VvLookUpLista Set_JAM_LookUpTable_ProjectDependent(string JAM_LookUpTableTitle)
   {
      VvLookUpLista luList = null;

      switch(JAM_LookUpTableTitle)
      {
         case ZXC.luiListaNalogTT_Name          : luList = ZXC.luiListaNalogTT          ; break;
         case ZXC.luiListaSkladista_Name        : luList = ZXC.luiListaSkladista        ; break;
         case ZXC.luiListaOpcina_Name           : luList = ZXC.luiListaOpcina           ; break;
         case ZXC.luiListaIspostava_Name        : luList = ZXC.luiListaIspostava        ; break;
         case ZXC.luiListaZupanija_Name         : luList = ZXC.luiListaZupanija         ; break;
         case ZXC.luiListaPostaZupan_Name       : luList = ZXC.luiListaPostaZupan       ; break;
         case ZXC.luiListaDjelat_Name           : luList = ZXC.luiListaDjelat           ; break;
         case ZXC.luiListaGrupaPartnera_Name    : luList = ZXC.luiListaGrupaPartnera    ; break;
         case ZXC.luiListaGodina_Name           : luList = ZXC.luiListaGodina           ; break;
         case ZXC.luiListaPrvlgScope_Name       : luList = ZXC.luiListaPrvlgScope       ; break;
         case ZXC.luiListaPrvlgType_Name        : luList = ZXC.luiListaPrvlgType        ; break;
         case ZXC.luiListaFakturType_Name       : luList = ZXC.luiListaFakturType       ; break;
         case ZXC.luiListaAmortTT_Name          : luList = ZXC.luiListaAmortTT          ; break;
         case ZXC.luiListaPersonTS_Name         : luList = ZXC.luiListaPersonTS         ; break;
         case ZXC.luiListaStrSprema_Name        : luList = ZXC.luiListaStrSprema        ; break;
         case ZXC.luiListaRadnoMjesto_Name      : luList = ZXC.luiListaRadnoMjesto      ; break;
         case ZXC.luiListaPlacaTT_Name          : luList = ZXC.luiListaPlacaTT          ; break;
         case ZXC.luiListaFondSati_NOR_Name     : luList = ZXC.luiListaFondSati_NOR     ; break;
         case ZXC.luiListaFondSati_TRG_Name     : luList = ZXC.luiListaFondSati_TRG     ; break;
         case ZXC.luiListaPlacaVrstaObr_Name    : luList = ZXC.luiListaPlacaVrstaObr    ; break;
         case ZXC.luiListaVrstaRadaEVR_Name     : luList = ZXC.luiListaVrstaRadaEVR     ; break;
         case ZXC.luiListaOsnovaOsigRsm_Name    : luList = ZXC.luiListaOsnovaOsigRsm    ; break;
         case ZXC.luiListaStazBRsm_Name         : luList = ZXC.luiListaStazBRsm         ; break;
         case ZXC.luiListaDeviza_Name           : luList = ZXC.luiListaDeviza           ; break;
         case ZXC.luiListaGrupa1Artikla_Name    : luList = ZXC.luiListaGrupa1Artikla    ; break;
         case ZXC.luiListaGrupa2Artikla_Name    : luList = ZXC.luiListaGrupa2Artikla    ; break;
         case ZXC.luiListaGrupa3Artikla_Name    : luList = ZXC.luiListaGrupa3Artikla    ; break;
         case ZXC.luiListaPdvKat_Name           : luList = ZXC.luiListaPdvKat           ; break;
         case ZXC.luiListaArtiklTS_Name         : luList = ZXC.luiListaArtiklTS         ; break;
         case ZXC.luiListaRiskZiroRn_Name       : luList = ZXC.luiListaRiskZiroRn       ; break;
         case ZXC.luiListaRiskVrstaPl_Name      : luList = ZXC.luiListaRiskVrstaPl      ; break;
         case ZXC.luiListaRiskCjenik_Name       : luList = ZXC.luiListaRiskCjenik       ; break;
         case ZXC.luiListaRiskStatus_Name       : luList = ZXC.luiListaRiskStatus       ; break;
         case ZXC.luiListaRiskPredefs_Name      : luList = ZXC.luiListaRiskPredefs      ; break;
         case ZXC.luiListaRiskTipOtprem_Name    : luList = ZXC.luiListaRiskTipOtprem    ; break;
         case ZXC.dscLuiLst_IRA_1_Name          : luList = ZXC.dscLuiLst_IRA_1          ; break;
         case ZXC.dscLuiLst_IRA_2_Name          : luList = ZXC.dscLuiLst_IRA_2          ; break;
         case ZXC.dscLuiLst_IRA_3_Name          : luList = ZXC.dscLuiLst_IRA_3          ; break;
         case ZXC.dscLuiLst_IRA_4_Name          : luList = ZXC.dscLuiLst_IRA_4          ; break;
         case ZXC.dscLuiLst_IRA_5_Name          : luList = ZXC.dscLuiLst_IRA_5          ; break;
         case ZXC.dscLuiLst_PON_Name            : luList = ZXC.dscLuiLst_PON            ; break;
         case ZXC.dscLuiLst_PON_2_Name          : luList = ZXC.dscLuiLst_PON_2          ; break;
         case ZXC.dscLuiLst_PON_3_Name          : luList = ZXC.dscLuiLst_PON_3          ; break;
         case ZXC.dscLuiLst_PON_4_Name          : luList = ZXC.dscLuiLst_PON_4          ; break;
         case ZXC.dscLuiLst_PNM_Name            : luList = ZXC.dscLuiLst_PNM            ; break;
         case ZXC.dscLuiLst_PNM_2_Name          : luList = ZXC.dscLuiLst_PNM_2          ; break;
         case ZXC.dscLuiLst_PNM_3_Name          : luList = ZXC.dscLuiLst_PNM_3          ; break;
         case ZXC.dscLuiLst_PNM_4_Name          : luList = ZXC.dscLuiLst_PNM_4          ; break;
         case ZXC.dscLuiLst_XYZ_Name            : luList = ZXC.dscLuiLst_XYZ            ; break;
         case ZXC.dscLuiLst_URA_Name            : luList = ZXC.dscLuiLst_URA            ; break;  
         case ZXC.dscLuiLst_UFA_Name            : luList = ZXC.dscLuiLst_UFA            ; break;  
         case ZXC.dscLuiLst_UFM_Name            : luList = ZXC.dscLuiLst_UFM            ; break;  
         case ZXC.dscLuiLst_PRI_Name            : luList = ZXC.dscLuiLst_PRI            ; break;  
         case ZXC.dscLuiLst_KLK_Name            : luList = ZXC.dscLuiLst_KLK            ; break;  
         case ZXC.dscLuiLst_KKM_Name            : luList = ZXC.dscLuiLst_KKM            ; break;  
         case ZXC.dscLuiLst_IFA_Name            : luList = ZXC.dscLuiLst_IFA            ; break;  
         case ZXC.dscLuiLst_IFA_2_Name          : luList = ZXC.dscLuiLst_IFA_2          ; break;  
         case ZXC.dscLuiLst_IFA_3_Name          : luList = ZXC.dscLuiLst_IFA_3          ; break;  
         case ZXC.dscLuiLst_IFA_4_Name          : luList = ZXC.dscLuiLst_IFA_4          ; break;  
         case ZXC.dscLuiLst_IZD_Name            : luList = ZXC.dscLuiLst_IZD            ; break;  
         case ZXC.dscLuiLst_IRM_Name            : luList = ZXC.dscLuiLst_IRM            ; break;  
         case ZXC.dscLuiLst_IRM_2_Name          : luList = ZXC.dscLuiLst_IRM_2          ; break;  
         case ZXC.dscLuiLst_IRM_3_Name          : luList = ZXC.dscLuiLst_IRM_3          ; break;  
         case ZXC.dscLuiLst_IRM_4_Name          : luList = ZXC.dscLuiLst_IRM_4          ; break;  
         case ZXC.dscLuiLst_UOD_Name            : luList = ZXC.dscLuiLst_UOD            ; break;  
         case ZXC.dscLuiLst_UPV_Name            : luList = ZXC.dscLuiLst_UPV            ; break;  
         case ZXC.dscLuiLst_UPM_Name            : luList = ZXC.dscLuiLst_UPM            ; break;  
         case ZXC.dscLuiLst_IOD_Name            : luList = ZXC.dscLuiLst_IOD            ; break;  
         case ZXC.dscLuiLst_IPV_Name            : luList = ZXC.dscLuiLst_IPV            ; break;  
         case ZXC.dscLuiLst_NRD_Name            : luList = ZXC.dscLuiLst_NRD            ; break;  
         case ZXC.dscLuiLst_NRM_Name            : luList = ZXC.dscLuiLst_NRM            ; break;  
         case ZXC.dscLuiLst_NRU_Name            : luList = ZXC.dscLuiLst_NRU            ; break;  
         case ZXC.dscLuiLst_NRS_Name            : luList = ZXC.dscLuiLst_NRS            ; break;  
         case ZXC.dscLuiLst_NRK_Name            : luList = ZXC.dscLuiLst_NRK            ; break;  
         case ZXC.dscLuiLst_STU_Name            : luList = ZXC.dscLuiLst_STU            ; break;  
         case ZXC.dscLuiLst_STI_Name            : luList = ZXC.dscLuiLst_STI            ; break;  
         case ZXC.dscLuiLst_RVI_Name            : luList = ZXC.dscLuiLst_RVI            ; break;  
         case ZXC.dscLuiLst_RVU_Name            : luList = ZXC.dscLuiLst_RVU            ; break;  
         case ZXC.dscLuiLst_UPL_Name            : luList = ZXC.dscLuiLst_UPL            ; break;  
         case ZXC.dscLuiLst_ISP_Name            : luList = ZXC.dscLuiLst_ISP            ; break;  
         case ZXC.dscLuiLst_BUP_Name            : luList = ZXC.dscLuiLst_BUP            ; break;  
         case ZXC.dscLuiLst_BIS_Name            : luList = ZXC.dscLuiLst_BIS            ; break;  
         case ZXC.dscLuiLst_PST_Name            : luList = ZXC.dscLuiLst_PST            ; break;  
         case ZXC.dscLuiLst_INV_Name            : luList = ZXC.dscLuiLst_INV            ; break;  
         case ZXC.dscLuiLst_CJE_Name            : luList = ZXC.dscLuiLst_CJE            ; break;  
         case ZXC.dscLuiLst_PPR_Name            : luList = ZXC.dscLuiLst_PPR            ; break;  
         case ZXC.dscLuiLst_PIP_Name            : luList = ZXC.dscLuiLst_PIP            ; break;  
         case ZXC.dscLuiLst_POV_Name            : luList = ZXC.dscLuiLst_POV            ; break;  
         case ZXC.dscLuiLst_MSI_Name            : luList = ZXC.dscLuiLst_MSI            ; break;  
         case ZXC.dscLuiLst_IZM_Name            : luList = ZXC.dscLuiLst_IZM            ; break;  
         case ZXC.dscLuiLst_IZM_2_Name          : luList = ZXC.dscLuiLst_IZM_2          ; break;  
         case ZXC.dscLuiLst_KIZ_Name            : luList = ZXC.dscLuiLst_KIZ            ; break;  
         case ZXC.dscLuiLst_PIK_Name            : luList = ZXC.dscLuiLst_PIK            ; break;  
         case ZXC.dscLuiLst_INM_Name            : luList = ZXC.dscLuiLst_INM            ; break;  
         case ZXC.dscLuiLst_KtoShema_Name       : luList = ZXC.dscLuiLst_KtoShema       ; break;  
         case ZXC.dscLuiLst_KtoShemaPlaca_Name  : luList = ZXC.dscLuiLst_KtoShemaPlaca  ; break;  
         case ZXC.luiListaRiskVodPrjkt_Name     : luList = ZXC.luiListaRiskVodPrjkt     ; break;
         case ZXC.luiListaMixerType_Name        : luList = ZXC.luiListaMixerType        ; break;
         case ZXC.luiListaMixerZadatak_Name     : luList = ZXC.luiListaMixerZadatak     ; break;
         case ZXC.luiListaMixerOdrediste_Name   : luList = ZXC.luiListaMixerOdrediste   ; break;
         case ZXC.luiListaMixerVozilo_Name      : luList = ZXC.luiListaMixerVozilo      ; break;
         case ZXC.luiListaMixerRelacija_Name    : luList = ZXC.luiListaMixerRelacija    ; break;
         case ZXC.luiListaMixTypePutNal_Name    : luList = ZXC.luiListaMixTypePutNal    ; break;
         case ZXC.luiListaMixTypeZahtjev_Name   : luList = ZXC.luiListaMixTypeZahtjev   ; break;
         case ZXC.luiListaMixDnevnice_Name      : luList = ZXC.luiListaMixDnevnice      ; break;
         case ZXC.luiListaMixTypeEvidencija_Name: luList = ZXC.luiListaMixTypeEvidencija; break;
         case ZXC.luiListaMixRadVrijemeRVR_Name : luList = ZXC.luiListaMixRadVrijemeRVR ; break;
         case ZXC.luiListaMixRadVrijemRVR2_Name : luList = ZXC.luiListaMixRadVrijemRVR2 ; break;
         case ZXC.dscLuiLst_riskRules_Name      : luList = ZXC.dscLuiLst_riskRules      ; break;  
         case ZXC.luiListaKplanKlase_Name       : luList = ZXC.luiListaKplanKlase       ; break;  
         case ZXC.luiListaMix_GFI_TSI_Name      : luList = ZXC.luiListaMix_GFI_TSI      ; break;  
         case ZXC.luiListaMix_Statist_NPF_Name  : luList = ZXC.luiListaMix_Statist_NPF  ; break;  
         case ZXC.luiListaTSI_Podaci_Name       : luList = ZXC.luiListaTSI_Podaci       ; break;  
         case ZXC.luiListaGFI_Bilanca_Name      : luList = ZXC.luiListaGFI_Bilanca      ; break;  
         case ZXC.luiListaGFI_RDG_Name          : luList = ZXC.luiListaGFI_RDG          ; break;  
         case ZXC.luiListaGFI_PodDop_Name       : luList = ZXC.luiListaGFI_PodDop       ; break;  
         case ZXC.luiListaGFI_NT_I_Name         : luList = ZXC.luiListaGFI_NT_I         ; break;  
         case ZXC.luiListaGFI_NT_D_Name         : luList = ZXC.luiListaGFI_NT_D         ; break;  
         case ZXC.luiListaRtranoGr_Name         : luList = ZXC.luiListaRtranoGr         ; break;  
         case ZXC.luiListaNacIspl_Name          : luList = ZXC.luiListaNacIspl          ; break;
         case ZXC.luiListaNeoporPrim_Name       : luList = ZXC.luiListaNeoporPrim       ; break;
         case ZXC.luiListaStjecatelj_Name       : luList = ZXC.luiListaStjecatelj       ; break;
         case ZXC.luiListaPrimDoh_Name          : luList = ZXC.luiListaPrimDoh          ; break;
         case ZXC.luiListaPocKraj_Name          : luList = ZXC.luiListaPocKraj          ; break;
         case ZXC.luiListaVrstaIzvjJoppd_Name   : luList = ZXC.luiListaVrstaIzvjJoppd   ; break;
         case ZXC.luiListaBrojSobe_Name         : luList = ZXC.luiListaBrojSobe         ; break;
         case ZXC.luiListaMixRadVrijemeMVR_Name : luList = ZXC.luiListaMixRadVrijemeMVR ; break;
         case ZXC.luiListaDrzave_Name           : luList = ZXC.luiListaDrzave           ; break;
         case ZXC.luiListaGranPrijelaz_Name     : luList = ZXC.luiListaGranPrijelaz     ; break;
         case ZXC.luiListaVrstaGosta_Name       : luList = ZXC.luiListaVrstaGosta       ; break;
         case ZXC.luiListaStatusGosta_Name      : luList = ZXC.luiListaStatusGosta      ; break;
         case ZXC.luiListaVrstaPutIsprave_Name  : luList = ZXC.luiListaVrstaPutIsprave  ; break;
         case ZXC.luiListaTipObjekta_Name       : luList = ZXC.luiListaTipObjekta       ; break;
         case ZXC.luiLista_PlanTT_Name          : luList = ZXC.luiLista_PlanTT          ; break;  
         case ZXC.luiListaPozicijePlana_Name    : luList = ZXC.luiListaPozicijePlana    ; break;  
         case ZXC.luiListaPozicijePlanaPLN_Name : luList = ZXC.luiListaPozicijePlanaPLN ; break;  
         case ZXC.luiListaPozicijePlanaRLZ_Name : luList = ZXC.luiListaPozicijePlanaRLZ ; break;  
         case ZXC.luiListaPozicijePlanaPBN_Name : luList = ZXC.luiListaPozicijePlanaPBN ; break;  
         case ZXC.luiListaFinFond_Name          : luList = ZXC.luiListaFinFond          ; break;  
         case ZXC.luiLista_PR_RAS_NPF_Name      : luList = ZXC.luiLista_PR_RAS_NPF      ; break;  
         case ZXC.luiLista_S_PR_RAS_NPF_Name    : luList = ZXC.luiLista_S_PR_RAS_NPF    ; break;  
         case ZXC.luiLista_BIL_NPF_Name         : luList = ZXC.luiLista_BIL_NPF         ; break;  
         case ZXC.luiLista_G_PR_IZ_NPF_Name     : luList = ZXC.luiLista_G_PR_IZ_NPF     ; break;  
         case ZXC.luiListaKlasifikALL_Name      : luList = ZXC.luiListaKlasifikALL      ; break;  
         case ZXC.luiListaKlasifikJOB_Name      : luList = ZXC.luiListaKlasifikJOB      ; break;  
         case ZXC.luiListaVrstaUgovora_Name     : luList = ZXC.luiListaVrstaUgovora     ; break;
         case ZXC.luiListaSerlot_Name           : luList = ZXC.luiListaSerlot           ; break;
         case ZXC.luiListaVrstaRNM_Name         : luList = ZXC.luiListaVrstaRNM         ; break;
         case ZXC.luiListaOutlookItems_Name     : luList = ZXC.luiListaOutlookItems     ; break;
         case ZXC.luiListaMixTypeZastitari_Name : luList = ZXC.luiListaMixTypeZastitari ; break;
         case ZXC.luiListaeRacPoslProc_Name     : luList = ZXC.luiListaeRacPoslProc     ; break;  
         case ZXC.luiListaFakRptUniFilter_Name  : luList = ZXC.luiListaFakRptUniFilter  ; break;
         case ZXC.luiListaPTG_NajamNaRok_Name   : luList = ZXC.luiListaPTG_NajamNaRok   ; break;  
         case ZXC.luiListaPTG_VrstaNajma_Name   : luList = ZXC.luiListaPTG_VrstaNajma   ; break;  
         case ZXC.luiListaPTG_DanZaFaktur_Name  : luList = ZXC.luiListaPTG_DanZaFaktur  ; break;  
         case ZXC.luiListaPTG_OsigPlacanja_Name : luList = ZXC.luiListaPTG_OsigPlacanja ; break;  
         case ZXC.luiListaGeonomenklatura_Name  : luList = ZXC.luiListaGeonomenklatura  ; break;  
         case ZXC.luiListaIncoterms_Name        : luList = ZXC.luiListaIncoterms        ; break;  
         case ZXC.luiListaIntrastVrPosla_Name   : luList = ZXC.luiListaIntrastVrPosla   ; break;  
         case ZXC.luiListaIntrastVrProm_Name    : luList = ZXC.luiListaIntrastVrProm    ; break;  
         case ZXC.luiListaIntrastIsporuka_Name  : luList = ZXC.luiListaIntrastIsporuka  ; break;  
         case ZXC.luiListaPCKpricesPerGB_Name   : luList = ZXC.luiListaPCKpricesPerGB   ; break;  
         case ZXC.luiListaZIZ_TT_Name           : luList = ZXC.luiListaZIZ_TT           ; break;  
         case ZXC.luiListaKPD2025_Name          : luList = ZXC.luiListaKPD2025          ; break;
         case ZXC.luiListaKodTipaEracuna_Name   : luList = ZXC.luiListaKodTipaEracuna   ; break;  

         default: ZXC.aim_emsg("LookUpTable [{0}] undone in VvTextBox.OnExitSetLookUp_Name()!!!", JAM_LookUpTableTitle); break;
      }

      return luList;
   }

   protected virtual void OpenMenu_ProjectSpecific_Actions()
   {
      // DUMMY for Vektor 

      // 02.05.2016: Za namjerno rucno omogucavanje utjecaja na Faktur sume via Fld_IsManuallyRed: 
      if((TheVvUC is FakturExtDUC) && ((TheVvDataRecord as Faktur).Has_TrnSum_vs_S_Sum_Discrepancy.NotEmpty()))
      {
         string message = "Dokument je 'crven' - ima razliku između suma stavaka sa sumom upisanom u TAB 'Sume'.\n\nDa li zelite zadržati takvo stanje tj. postaviti oznaku\n\n'Ručna korekcija'";
         DialogResult result = MessageBox.Show(message, "Potvrdite Ručnu korekciju", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

         if(result == DialogResult.Yes)
         {
            (TheVvUC as FakturExtDUC).Fld_IsManuallyRed = true;
         }
         else
         {
            (TheVvUC as FakturExtDUC).Fld_IsManuallyRed = false;
         }
      }
   }

   public virtual VvLookUpLista DocumentTypeLookUpListaForThisModulEnum(ZXC.VvModulEnum modulEnum)
   {
      switch(modulEnum)
      {
         case ZXC.VvModulEnum.MODUL_RAS:   return ZXC.luiListaFakturType;
         case ZXC.VvModulEnum.MODUL_FIN:   return ZXC.luiListaNalogTT;
         case ZXC.VvModulEnum.MODUL_OSR:   return ZXC.luiListaAmortTT;
         case ZXC.VvModulEnum.MODUL_PLA:   return ZXC.luiListaPlacaTT;
        // case ZXC.VvModulEnum.MODUL_OTHER: return ZXC.luiListaMixerType;

         default: ZXC.aim_emsg("yoyo undone"); return null;
      }
   }

   public virtual int GetDebitSqlCommand_ProjectDependent(VvReport vvReport)
   {
      // dummy for Vektor 
      return -1;
   }

   public virtual int GetDtranuSqlCommand_ProjectDependent(VvReport vvReport)
   {
      // dummy for Vektor 
      return -1;
   }

   public virtual DataRow GetCommonDrSchema_DocumentRecord_DokNum()
   {
      return ZXC.NalogSchemaRows[ZXC.NalCI.dokNum];
   }

   public virtual string SuggestedPdfExportFileName(string defaultExt, bool isForSigning)
   {
      string fileName = "";

      if(TheVvUC.IsOnReportTabPage)
      {
       //return "TODO filename " + "@_" + DateTime.Now.ToString(ZXC.VvTimeStampFormat4FileName);
       //15.05.2023.
       //fileName = tsCbxReport.SelectedItem + " @_" + DateTime.Now.ToString(ZXC.VvTimeStampFormat4FileName);
         if(TheVvUC is FinReportUC)
         {
          //fileName = TheVvReportUC.TheVvReport.reportName;
            fileName = tsCbxReport.SelectedItem.ToString();
         }
         else
         {
            fileName = tsCbxReport.SelectedItem + " @_" + DateTime.Now.ToString(ZXC.VvTimeStampFormat4FileName);
         }
      }

      else
      {
         string recodName = TheVvDataRecord.VirtualRecordName;
         switch(recodName)
         {
            case Faktur.recordName:
               Faktur faktur_rec = TheVvDataRecord as Faktur;
               string docName = ZXC.luiListaFakturType.GetNameForThisCd(faktur_rec.TT);
               //05.02.2015. ne radi export faktura bez exta zabog kupdob=null
               //fileName = faktur_rec.TT_And_TtNum + " [" + /*docName*/faktur_rec.KupdobName + "]";
               if(faktur_rec.IsExtendable) fileName = faktur_rec.TT_And_TtNum + " [" + /*docName*/faktur_rec.KupdobName + "]";
               else                        fileName = faktur_rec.TT_And_TtNum;

               // 12.09.2019: roman zatrazio 
               if(ZXC.IsSvDUH && faktur_rec.IsExtendable)
               {
                  fileName = faktur_rec.KupdobName + "-" + faktur_rec.TT_And_TtNum;
               }

               break;

            default: fileName = recodName + " " + TheVvDataRecord.ToString().Replace(":", "") + " @_" + DateTime.Now.ToString(ZXC.VvTimeStampFormat4FileName);

               break;
         }
      }

      return fileName.IsEmpty() ? fileName : fileName + (isForSigning ? SignedPDFextension : "") + "." + defaultExt;
   }

   #endregion Virtual Methods (Other projects are overriders)

   #region CreateTheVvReport_SwitchReportEnum

   public virtual VvReport CreateTheVvReport_SwitchReportEnum(VvReportSubModul vvReportSubModul, string reportName, VvReportUC vvReportUC)
   {
      #region Init RISK Stuff

      VvReport vvReport = null;

      CrystalDecisions.CrystalReports.Engine.ReportDocument repDoc;
      VvRpt_RiSk_Filter theRiskFilter = null;
      string artiklGR = "";
      string fakturGR = "";
      bool isArtiklGR = false;
      bool isFakturGR = false;

      if(vvReportUC is RiskReportUC)
      {
         theRiskFilter = ((RiskReportUC)vvReportUC).TheRptFilter;
         artiklGR = theRiskFilter.GrupiranjeArtikla;
         fakturGR = theRiskFilter.GrupiranjeDokum;
         isArtiklGR = artiklGR.NotEmpty();
         isFakturGR = fakturGR.NotEmpty();
      }

      #endregion Init RISK Stuff

      switch(vvReportSubModul.reportEnum)
      {
         // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-= 

         #region FIZ

         case ZXC.VvReportEnum.FIZ_BilancaS:            vvReport = new RptF_Bilanca             (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_BilancaU:            vvReport = new RptF_BilancaU            (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_BilancaSubKlas:      vvReport = new RptF_BilancaSubKlas      (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_BilancaUNeprof:      vvReport = new RptF_BilancaUsNeprof     (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_Dnevnik:             vvReport = new RptF_Dnevnik             (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_KKonta_ALL:          vvReport = new RptF_KKonta_ALL          (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_KKontaGroup_ALL:     vvReport = new RptF_KKontaGroup_ALL     (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_KKontaGroup_ALL_Exp: vvReport = new RptF_KKontaGroup_ALL_Exp (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_Dnevnik_Exp:         vvReport = new RptF_Dnevnik_Exp         (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;

         case ZXC.VvReportEnum.FIZ_KKonta:
            if(!RptF_KKonta.IsFilterWellFormed(((FinReportUC)vvReportUC))) vvReport = null;
            else                                                           vvReport = new RptF_KKonta(reportName, ((FinReportUC)vvReportUC).TheRptFilter);
            break;

         case ZXC.VvReportEnum.FIZ_OTS:
            if(!RptF_OtvoreneStavke.IsFilterWellFormed(((FinReportUC)vvReportUC))) vvReport = null;
            else                                                                   vvReport = new RptF_OtvoreneStavke(reportName, ((FinReportUC)vvReportUC).TheRptFilter);
            break;

         case ZXC.VvReportEnum.FIZ_OTS_Kum:
            if(!RptF_OtvoreneStavke_Kum.IsFilterWellFormed(((FinReportUC)vvReportUC))) vvReport = null;
            else                                                                       vvReport = new RptF_OtvoreneStavke_Kum(reportName, ((FinReportUC)vvReportUC).TheRptFilter);
            break;

         case ZXC.VvReportEnum.FIZ_AnaSC:
            if(!RptF_AnalitikaSKonta.IsFilterWellFormed(((FinReportUC)vvReportUC))) vvReport = null;
            else                                                                    vvReport = new RptF_AnalitikaSKonta(reportName, ((FinReportUC)vvReportUC).TheRptFilter);
            break;

         case ZXC.VvReportEnum.FIZ_APT_K1:
            vvReport = new RptF_AnalitikaProrTroska(reportName, ((FinReportUC)vvReportUC).TheRptFilter);
          //((FinReportUC)vvReportUC).TheRptFilter.VrstaKnj = "IZ";
            break;

         case ZXC.VvReportEnum.FIZ_AnaSCL:
            if(!RptF_AnalitikaSKonta_L.IsFilterWellFormed(((FinReportUC)vvReportUC))) vvReport = null;
            else                                                                      vvReport = new RptF_AnalitikaSKonta_L(reportName, ((FinReportUC)vvReportUC).TheRptFilter);
            break;

         case ZXC.VvReportEnum.FIZ_SinSC:
            if(!RptF_SintetikaSKonta.IsFilterWellFormed(((FinReportUC)vvReportUC))) vvReport = null;
            else                                                                    vvReport = new RptF_SintetikaSKonta(reportName, ((FinReportUC)vvReportUC).TheRptFilter);
            break;

         case ZXC.VvReportEnum.FIZ_AnaMT:
            if(!RptF_AnalitikaSKontaMT.IsFilterWellFormed(((FinReportUC)vvReportUC))) vvReport = null;
            else                                                                      vvReport = new RptF_AnalitikaSKontaMT(reportName, ((FinReportUC)vvReportUC).TheRptFilter);
            break;

         case ZXC.VvReportEnum.FIZ_SinMT:
            if(!RptF_SintetikaSKontaMT.IsFilterWellFormed(((FinReportUC)vvReportUC))) vvReport = null;
            else                                                                     vvReport = new RptF_SintetikaSKontaMT(reportName, ((FinReportUC)vvReportUC).TheRptFilter);
            break;

         case ZXC.VvReportEnum.FIZ_RekapNal:      vvReport = new RptF_RekapNal     (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_KPlan:         vvReport = new RptF_KontniPlan   (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_KPI:           vvReport = new RptF_KPI          (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_KPI_orig:      vvReport = new RptF_KPI_orig     (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_PPI:           vvReport = new RptNTR_PPI        (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_NTR_BilancaMP: vvReport = new RptNTR_BilancaMP  (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_NTR_TSIPod:    vvReport = new RptNTR_TSIPOD     (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_NTR_RDiGMP:    vvReport = new RptNTR_RDiG       (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_NTR_ObrPD:     vvReport = new RptNTR_ObrPD      (reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_KplanExport:   vvReport = new RptF_KplanZaExport(reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;

         //18.02.2018. ova knjiga ura se ne upotrebljava
         //case ZXC.VvReportEnum.FIZ_KnjigaURA: vvReport = new RptF_KnjigaURA(reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.FIZ_KnjigaURA: vvReport = new RptNTR_PPI2(reportName, ((FinReportUC)vvReportUC).TheRptFilter); break;

         case ZXC.VvReportEnum.FIZ_PlanPG_Plan1:
         case ZXC.VvReportEnum.FIZ_ThePln_Reali:
         case ZXC.VvReportEnum.FIZ_Pl1_ThePln_Reali:
         case ZXC.VvReportEnum.FIZ_ThePln_Njv_Reali: vvReport = new RptF_PlanAndRealizacija(reportName, ((FinReportUC)vvReportUC).TheRptFilter, vvReportSubModul.reportEnum); break;
         case ZXC.VvReportEnum.FIZ_ThePln_Money    : vvReport = new RptF_PlanAndRealizNovac(reportName, ((FinReportUC)vvReportUC).TheRptFilter, vvReportSubModul.reportEnum); break;
         case ZXC.VvReportEnum.FIZ_Plan_PBN        : vvReport = new RptF_PlanAndRealiz_PBN (reportName, ((FinReportUC)vvReportUC).TheRptFilter, vvReportSubModul.reportEnum); break;

         #endregion FIZ

         // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-= 

         #region AIZ

         case ZXC.VvReportEnum.AIZ_PopisDI: vvReport = new RptO_PopisDI(reportName, ((AmoReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.AIZ_PopisDI_DD: vvReport = new RptO_PopisDI_DD(reportName, ((AmoReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.AIZ_Old_PopisDI_DD: vvReport = new RptO_ListaPoKontu(reportName, ((AmoReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.AIZ_RekapDok: vvReport = new RptO_RekapAmo(reportName, ((AmoReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.AIZ_RekapDok_DD: vvReport = new RptO_RekapAmo_DD(reportName, ((AmoReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.AIZ_Dnevnik: vvReport = new RptO_DnevnikAM(reportName, ((AmoReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.AIZ_Osred: vvReport = new RptO_KartOsred(reportName, ((AmoReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.AIZ_InvList: vvReport = new RptO_InvLista(reportName, ((AmoReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.AIZ_InvDiff: vvReport = new RptO_InvDiff(reportName, ((AmoReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.AIZ_ObrazacDI: vvReport = new RptO_ObrazacDI(reportName, ((AmoReportUC)vvReportUC).TheRptFilter); break;

         #endregion AIZ

         // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-= 

         #region KIZ

         case ZXC.VvReportEnum.KIZ_Lista: vvReport = new RptK_GeneralList(reportName, ((KupdobReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.KIZ_Kontakti: vvReport = new RptK_PartnerKontakti(reportName, ((KupdobReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.KIZ_Faktur: vvReport = new RptK_PartnerFaktur(reportName, ((KupdobReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.KIZ_GrupTip: vvReport = new RptK_PartnerGrupTip(reportName, ((KupdobReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.KIZ_Rabat: vvReport = new RptK_PartnerRabat(reportName, ((KupdobReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.KIZ_Ziro: vvReport = new RptK_PartnerZiro(reportName, ((KupdobReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.KIZ_Sifre: vvReport = new RptK_PartnerSifre(reportName, ((KupdobReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.KIZ_VizitKarta: vvReport = new RptK_VizitKarta(reportName, ((KupdobReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.KIZ_VizitKaProsirena: vvReport = new RptK_VizitKaProsireno(reportName, ((KupdobReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.KIZ_PartnerExport: vvReport = new RptK_PartnerZaExport(reportName, ((KupdobReportUC)vvReportUC).TheRptFilter); break;

         case ZXC.VvReportEnum.PRIZ_Lista: vvReport = new RptPrj_GeneralList(reportName, ((KupdobReportUC)vvReportUC).TheRptFilter); break;

         #endregion KIZ

         // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-= 

         #region PIZ

         case ZXC.VvReportEnum.PIZ_RekapPlaca: vvReport = new VvPlacaReport(new CR_RekapitulacijaPlacA(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;
         
         case ZXC.VvReportEnum.PIZ_APT_K2    : vvReport = new VvPlacaReport(new CR_TranspKat2Pl(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;
         
         case ZXC.VvReportEnum.PIZ_RekapPerson: vvReport = new VvPlacaReport(new CR_RekapPerson(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;

         case ZXC.VvReportEnum.PIZ_RekapBruta: vvReport = new VvPlacaReport(new CR_RekapitulacijaBruta(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.PIZ_RAD1: vvReport = new RptP_RAD1(new CR_RAD1(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;

         case ZXC.VvReportEnum.PIZ_RAD1_G: vvReport = new RptP_RAD1_G(new CR_RAD_1_G(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;

         case ZXC.VvReportEnum.PIZ_ObracunPlace: vvReport = new VvPlacaReport(new CR_ObracunPlace_2(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            ((VvPlacaReport)vvReport).VvLet_TT_RRiPP_Only = true;
            break;
         case ZXC.VvReportEnum.PIZ_IsplatnaLista: vvReport = new VvPlacaReport(new CR_IsplatnaLista_New(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            ((VvPlacaReport)vvReport).VvLet_TT_RRiPP_Only = true;
            break;

       //case ZXC.VvReportEnum.PIZ_IsplatListOP: vvReport = new VvPlacaReport(new CR_IsplatnaListaObracunOP(), new ZXC.VvRptExternTblChooser_Placa(true, true, false, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
         case ZXC.VvReportEnum.PIZ_IsplatListOP:
            if(int.Parse(ZXC.projectYear) < 2024)  vvReport = new VvPlacaReport(new CR_IsplatnaListaObracunOP(), new ZXC.VvRptExternTblChooser_Placa(true, true, false, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            else                                   vvReport = new VvPlacaReport(new CR_IsplatnaListaOstPriPN() , new ZXC.VvRptExternTblChooser_Placa(true, true, false, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            ((VvPlacaReport)vvReport).VvLet_TT_RRiPP_Only = true;
            break;
         case ZXC.VvReportEnum.PIZ_ObrazacIP1: if(ZXC.TheVvForm.BadGuys("PIZ_ObrazacIP1")) break;
            vvReport = new VvPlacaReport(new CR_ObrazacIP1(), new ZXC.VvRptExternTblChooser_Placa(true, true, true, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            ((VvPlacaReport)vvReport).VvLet_TT_RRiPP_Only = true;
            break;
         case ZXC.VvReportEnum.PIZ_ObrazacIP1_v2: if(ZXC.TheVvForm.BadGuys("PIZ_ObrazacIP1_v2")) break;
            vvReport = new VvPlacaReport(new CR_ObrazacIP1_1023(), new ZXC.VvRptExternTblChooser_Placa(true, true, true, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            ((VvPlacaReport)vvReport).VvLet_TT_RRiPP_Only = true;
            break;

         case ZXC.VvReportEnum.PIZ_ObrazacNP1: if(ZXC.TheVvForm.BadGuys("PIZ_ObrazacNP1")) break;
            //01.12.2015.
            //   vvReport = new VvPlacaReport(new CR_ObrazacNP1()          , new ZXC.VvRptExternTblChooser_Placa(true, true, true, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            vvReport = new VvPlacaReport(new CR_ObrazacNP1_Obv011015(), new ZXC.VvRptExternTblChooser_Placa(true, true, true, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            ((VvPlacaReport)vvReport).VvLet_TT_RRiPP_Only = true;
            break;

         //case ZXC.VvReportEnum.PIZ_ObrazacID:   if(int.Parse(ZXC.projectYear) < 2011) vvReport = new VvPlacaReport(new CR_IDObrazac_Subreport(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
         //                                       else                                  vvReport = new VvPlacaReport(new CR_IDobrazac2011()      , new ZXC.VvRptExternTblChooser_Placa(false, false, false, true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
         case ZXC.VvReportEnum.PIZ_ObrazacID: if(int.Parse(ZXC.projectYear) < 2011) vvReport = new RptP_ObrazacID(new CR_IDObrazac_Subreport(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
                                              else                                  vvReport = new RptP_ObrazacID(new CR_IDobrazac2011(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);

            ((VvPlacaReport)vvReport).VvLet_TT_RRiPP_Only = true;
            break;

         case ZXC.VvReportEnum.PIZ_ObrazacDNR: vvReport = new VvPlacaReport(new CR_DNRobrazac(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            ((VvPlacaReport)vvReport).VvLet_TT_RRiPP_Only = true;
            break;

         case ZXC.VvReportEnum.PIZ_ObracunDrDoh: vvReport = new VvPlacaReport(new CR_ObracunDrDoh(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, false, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            ((VvPlacaReport)vvReport).VvLet_TT_AHiUGiNOiK4_Only = true;
            break;
         case ZXC.VvReportEnum.PIZ_IsplListaDrDoh: vvReport = new VvPlacaReport(new CR_IsplListaDrDoh(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, false, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            ((VvPlacaReport)vvReport).VvLet_TT_AHiUGiNOiK4_Only = true;
            break;
         case ZXC.VvReportEnum.PIZ_ObrazacIDD: //vvReport = new VvPlacaReport  (new CR_IDDobrazac(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, false, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            vvReport = new RptP_ObrazacIDD(new CR_IDDobrazac(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, false, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            ((VvPlacaReport)vvReport).VvLet_TT_AHiUGiNOiK4_Only = true;
            break;
         case ZXC.VvReportEnum.PIZ_PotvrdaDrDoh: vvReport = new VvPlacaReport(new CR_PotvrdaDrugiDoh(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, false, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            ((VvPlacaReport)vvReport).VvLet_TT_AHiUGiNOiK4_Only = true;
            break;

         case ZXC.VvReportEnum.PIZ_ObrazacIPP: vvReport = new VvPlacaReport(new CR_IPPobrazac(), new ZXC.VvRptExternTblChooser_Placa(false, false, false, true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;

         case ZXC.VvReportEnum.PIZ_ObrazacRSm: vvReport = new RptP_ObrazRSm(new CR_RSmObrazac(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;
         //  case ZXC.VvReportEnum.PIZ_ObrazacID1   : vvReport = new RptP_ObrazacID1(new CR_ID1obrazac()       , new ZXC.VvRptExternTblChooser_Placa(true , false, false, false, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.PIZ_ObrazacID1: vvReport = new RptP_ObrazacID1(new CR_ID1Obrazac2013(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, false, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;
         //case ZXC.VvReportEnum.PIZ_ObrazacIP    : vvReport = new RptP_IP_Kartica(new CR_IPobrazac()          , new ZXC.VvRptExternTblChooser_Placa(true , false, false, true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.PIZ_ObrazacIP: vvReport = new RptP_IP_Kartica(new CR_IPobrazac2013(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;

         case ZXC.VvReportEnum.PIZ_KonacniObrPor: vvReport = new RptP_GodObrPoreza(new CR_GodObrPoreza(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            ((VvPlacaReport)vvReport).VvLet_TT_RRiPP_Only = true;
            break;


         //case ZXC.VvReportEnum.PIZ_VirmaniPlaca: vvReport = new RptP_Virmani(new CR_VirmaniPlaca(), new ZXC.VvRptExternTblChooser_Placa(true, true, true, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;
         //case ZXC.VvReportEnum.PIZ_VirmaniPlaca: vvReport = new RptP_Virmani(new CR_Virmani_New(), new ZXC.VvRptExternTblChooser_Placa(true, true, true, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.PIZ_VirmaniPlaca: vvReport = new RptP_Virmani(new CR_Virman_Hub3A(), new ZXC.VvRptExternTblChooser_Placa(true, true, true, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;
         case ZXC.VvReportEnum.PIZ_ZbrojniNalog: vvReport = new RptP_Virmani(new CR_ZbrojniNalogZaPr(), new ZXC.VvRptExternTblChooser_Placa(true, true, true, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;

         case ZXC.VvReportEnum.ZNP_Pogled: vvReport = new RptP_SEPA(new CR_SepaPain00100103(), new ZXC.VvRptExternTblChooser_Placa(true, true, true, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;

         //case ZXC.VvReportEnum.ZNP_Pogled: vvReport = null; break;

         case ZXC.VvReportEnum.PIZ_ListaZaBanku:

            if(!RptP_ListaBanka.IsFilterWellFormed(((PlacaReportUC)vvReportUC)))
               vvReport = null;
            else
               vvReport = new RptP_ListaBanka(new CR_ListaZaBanku(), new ZXC.VvRptExternTblChooser_Placa(true, true, true, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);

            break;

         //case ZXC.VvReportEnum.PIZ_ObrazacID     : vvReport = new VvPlacaReport(new CR_IDobrazac()          , new ZXC.VvRptExternTblChooser_Placa(false, false,  true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter); break;

         case ZXC.VvReportEnum.PIZ_ListaRadnika: vvReport = new VvPlacaReport(new CR_IsplatnaLista_080710(), new ZXC.VvRptExternTblChooser_Placa(true, true, true, true, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            ((VvPlacaReport)vvReport).VvLet_TT_RRiPP_Only = true;
            break;

         case ZXC.VvReportEnum.PIZ_ListaObustava:
            vvReport = new VvPlacaReport(new CR_ListaObustava(), new ZXC.VvRptExternTblChooser_Placa(false, false, true, false, true), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            ((VvPlacaReport)vvReport).DajSamoZbirneObustave = true;
            break;

         case ZXC.VvReportEnum.PIZ_JOPPD:
            //if(ZXC.TheVvForm.BadGuys("JOPPD")) break;

            if(Convert.ToInt16((((PlacaReportUC)vvReportUC).TheRptFilter).RSmID) < 15059) /*||(((PlacaReportUC)vvReportUC).TheRptFilter).RSmID < 15059*/
                                                                                          //if((((PlacaReportUC)vvReportUC).TheRptFilter).NaDan < new DateTime(2015, 02, 28) /*||(((PlacaReportUC)vvReportUC).TheRptFilter).RSmID < 15059*/)
               vvReport = new RptP_JOPPD(new CR_JOPPD(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            else
               vvReport = new RptP_JOPPD(new CR_JOPPD_28022015(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            break;

         case ZXC.VvReportEnum.PIZ_PotpisnaLista: vvReport = new VvPlacaReport(new CR_PotpisnaListaPL(), new ZXC.VvRptExternTblChooser_Placa(true, true, false, false, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            break;

         case ZXC.VvReportEnum.PIZ_RekapNeoporPri:
            vvReport = new RptP_NeoporezPrimici(new CR_RekapNeoporezPrim(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, true, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            break;
        
         case ZXC.VvReportEnum.PIZ_PersonMatPodaci: vvReport = new RptP_PersonMaticniPodaci(new CR_PersonMaticni(), new ZXC.VvRptExternTblChooser_Placa(true, false, false, false, false), reportName, ((PlacaReportUC)vvReportUC).TheRptFilter);
            
            ((PlacaReportUC)vvReportUC).TheRptFilter.UGodiniChecked = true;
            ((PlacaReportUC)vvReportUC).TheRptFilter.RSmIDChecked   = false;

            break;


         #endregion PIZ

         #region RIZ

         #region REALIZACIJA

         case ZXC.VvReportEnum.RIZ_RekapIRMasBlagIzvj:
            vvReport = new RptR_RekapIRMasBlagIzvj(fakturGR, new Vektor.Reports.RIZ.CR_RekapIRMasBlagIzvj(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                       false, // ArtiklWithArtstat         
                                       false, // ArtStat        
                                       true, // Faktur         
                                       false, // Rtrans         
                                              /*false*/true, // Kupdob         
                                       false, // Prjkt          
                                       false, // Rtrans4ruc     
                                       false);// Artikl         

            //theRiskFilter.AnalitSintet  = "S";
            theRiskFilter.TT = Faktur.TT_IRM;
            VvLookUpItem theLui = ZXC.luiListaRiskVrstaPl.FirstOrDefault(lui => lui.Flag == true); // probaj naci prvi lui sa IsNPcash flagom  
            theRiskFilter.NacinPlacanja = (theLui == null ? "NPerror" : theLui.Cd);

            break;

         case ZXC.VvReportEnum.RIZ_Rekap_TH_DjelatRabat:
         case ZXC.VvReportEnum.RIZ_Rekap_TH_DjelatRabat_S:
            vvReport = new RptR_Rekap_TH_DjelatRabat(fakturGR, new Vektor.Reports.RIZ.CR_Rekap_TH_DjelatRabat(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                       false, // ArtiklWithArtstat         
                                       false, // ArtStat        
                                       true, // Faktur         
                                       false, // Rtrans         
                                              /*false*/true, // Kupdob         
                                       false, // Prjkt          
                                       false, // Rtrans4ruc     
                                       false);// Artikl         

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_Rekap_TH_DjelatRabat) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";
            theRiskFilter.IsPrihodTT = false;
            theRiskFilter.IsRashodTT = false;
            theRiskFilter.TT = Faktur.TT_IRM;
            theRiskFilter.FuseStr3 = "R";
            break;

         case ZXC.VvReportEnum.RIZ_RekapFaktur:
         case ZXC.VvReportEnum.RIZ_RekapFaktur_S:
            vvReport = new RptR_RekapFaktur(fakturGR, /*new Vektor.Reports.RIZ.CR_MalopRekapDok(),*/ reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                       false, // ArtiklWithArtstat         
                                       false, // ArtStat        
                                       true, // Faktur         
                                       false, // Rtrans         
                                              /*false*/true, // Kupdob         
                                       false, // Prjkt          
                                       false, // Rtrans4ruc     
                                       false);// Artikl         
            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_RekapFaktur) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";

            break;


         case ZXC.VvReportEnum.RIZ_Komparacija:
            if(!RptR_RekapCompare.IsFilterWellFormed(((RiskReportUC)vvReportUC))) vvReport = null;
            else vvReport = new RptR_RekapCompare(fakturGR, new Vektor.Reports.RIZ.CR_Komparcija(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                       false, // ArtiklWithArtstat         
                                       false, // ArtStat        
                                       true, // Faktur         
                                       false, // Rtrans         
                                              /*false*/true, // Kupdob         
                                       false, // Prjkt          
                                       false, // Rtrans4ruc     
                                       false);// Artikl         

            theRiskFilter.AnalitSintet = "S";
            break;

         case ZXC.VvReportEnum.RIZ_RUC_Ira:
         case ZXC.VvReportEnum.RIZ_RUC_Ira_S:
              vvReport = new RptR_Ira_Ruc(fakturGR, new Vektor.Reports.RIZ.CR_RUC_IRA_Light(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                   false, // ArtiklWithArtstat         
                                                   false, // ArtStat        
                                                   true, // Faktur         
                                                   false, // Rtrans         
                                          /*false*/true, // Kupdob         
                                                   false, // Prjkt          
                                                   true, // Rtrans4ruc     
                                                   false);// Artikl         

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_RUC_Ira) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";
            break;

         case ZXC.VvReportEnum.RIZ_RUC_Ira_Rtrans:
           vvReport = new RptR_Ira_Ruc_Rtrans(fakturGR, new Vektor.Reports.RIZ.CR_RUC_Rtrans(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                   false, // ArtiklWithArtstat         
                                                   false, // ArtStat        
                                                   true, // Faktur         
                                                   false, // Rtrans         
                                          /*false*/true, // Kupdob         
                                                   false, // Prjkt          
                                                   true, // Rtrans4ruc     
                                                         /*false*/true);// Artikl         
            theRiskFilter.AnalitSintet = "A";
            break;

         case ZXC.VvReportEnum.RIZ_RUC_Provizija_A:
         case ZXC.VvReportEnum.RIZ_RUC_Provizija_S:
              ReportDocument rptDocProvKom =  new Vektor.Reports.RIZ.CR_ProvizijaKomerc();
              ReportDocument rptDocTgZAR   =  new Vektor.Reports.RIZ.CR_RekapFaktur_ZAR();
              ReportDocument rptDoc = ZXC.IsTETRAGRAMdomena ? rptDocTgZAR : rptDocProvKom;
          //vvReport = new RptR_Ira_Ruc_Rtrans(fakturGR, new Vektor.Reports.RIZ.CR_ProvizijaKomerc(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
            vvReport = new RptR_Ira_Ruc_Rtrans(fakturGR, rptDoc, reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                   false, // ArtiklWithArtstat         
                                                   false, // ArtStat        
                                                   true, // Faktur         
                                                   false, // Rtrans         
                                          /*false*/true, // Kupdob         
                                                   false, // Prjkt          
                                                   true, // Rtrans4ruc     
                                                         /*false*/true);// Artikl         
            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_RUC_Provizija_A) theRiskFilter.AnalitSintet = "A";
            else                                                                    theRiskFilter.AnalitSintet = "S";

            if(ZXC.IsTETRAGRAMdomena)
            { 
               theRiskFilter.TT = "ZAR";
            }
            else 
            {
               //12.10.2020. ako je upaljen ovaj IsPrihodTT onda ne ulaze u obzir YRNovi proslih godina
               theRiskFilter.IsPrihodTT = false;
               theRiskFilter.IsRashodTT = false;
               theRiskFilter.TT = "";
            }
            break;

         case ZXC.VvReportEnum.RIZ_RekapRN:
         case ZXC.VvReportEnum.RIZ_RekapRN_S:
            vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_RekapitulacijaRNPS(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                 false, // ArtiklWithArtstat         
                                                 false, // ArtStat        
                                                 true, // Faktur         
                                                 false, // Rtrans         
                                                 false, // Kupdob         
                                                 false, // Prjkt          
                                                 false, // Rtrans4ruc     
                                                 false);// Artikl         

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_RekapRN) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";
            break;

         case ZXC.VvReportEnum.RIZ_KPM: vvReport = new RptR_KnjigaPopisa(new Vektor.Reports.RIZ.CR_KnjgaPopisa_MP(), reportName, theRiskFilter); break;

         case ZXC.VvReportEnum.RIZ_BlgDnevnik: vvReport = new RptR_BLAG(new Vektor.Reports.RIZ.CR_BlgDnevnik(), reportName, theRiskFilter); break;

         case ZXC.VvReportEnum.RIZ_IFAraster:
         case ZXC.VvReportEnum.RIZ_IFArasterB:

            ushort subDsc = (ushort)(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_IFAraster ? 0 : 2);

            if(ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.TEMBO || ZXC.CURR_prjkt_rec.Ticker.StartsWith("TEMBO")) // TEMBO podmetni devizni dsc (br 4) 
            {
               subDsc = (ushort)(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_IFAraster ? 0 : 4);
            }

            if(!RptR_PrnManyFak.IsFilterWellFormed(((RiskReportUC)vvReportUC)))
            {
               vvReport = null;
            }
            else
            {
               VvRpt_RiSk_Filter riskFilter = theRiskFilter;

               if(ZXC.TtInfo(riskFilter.TT).IsExtendableTT == false) // interni dokumenti 
               {
                  vvReport = new RptR_PrnManyFak(new Vektor.Reports.RIZ.CR_FaktInterna(), reportName, theRiskFilter, 0);
               }
               else
               {
                  if(ZXC.TtInfo(riskFilter.TT).IsBlagajnaTT) // blagajna
                  {
                     vvReport = new RptR_PrnManyFak(new Vektor.Reports.RIZ.CR_UplIspl(), reportName, theRiskFilter, 0);
                  }
                  else // externi dokumenti (IFA, IRA, UFA, URA, ...)
                  {
                     //vvReport = new RptR_PrnManyFak(new Vektor.Reports.RIZ.CR_PrnManyFak(), reportName, theRiskFilter);
                     vvReport = new RptR_PrnManyFak(new Vektor.Reports.RIZ.CR_PrnManyFak_ArtiklLight(), reportName, theRiskFilter, subDsc);
                  }
               }
            }
            break;

         case ZXC.VvReportEnum.RIZ_Rekap_FISK_Faktur:
            vvReport = new RptR_Rekap_FISK_Faktur(reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                 false,  // ArtiklWithArtstat         
                                                 false,  // ArtStat        
                                                 true,  // Faktur         
                                                 false,  // Rtrans         
                                                 false,  // Kupdob         
                                                 false,  // Prjkt          
                                                 false,  // Rtrans4ruc     
                                                 false); // Artikl   
            break;

          case ZXC.VvReportEnum.RIZ_RekapFakturWKupdob:
            if(!ZXC.IsTETRAGRAM_ANY) vvReport = null;
            else                     vvReport = new RptR_Rekap_SPN1_Faktur(reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                false, // ArtiklWithArtstat         
                                                false, // ArtStat        
                                                true, // Faktur         
                                                false, // Rtrans         
                                               /*false*/true, // Kupdob         
                                                false, // Prjkt          
                                                false, // Rtrans4ruc     
                                                false);// Artikl         

            break;


         case ZXC.VvReportEnum.RIZ_Rekap_MER_STATUS:

            if(theRiskFilter.TT != Faktur.TT_IFA && theRiskFilter.TT != Faktur.TT_IRA && theRiskFilter.TT != Faktur.TT_IRM)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Zadajte TT izlaznih eRačuna.");
               vvReport = null;
               break;
            }

            vvReport = new RptR_Rekap_MER_STATUS(fakturGR, new Vektor.Reports.RIZ.CR_Rekap_MER_Status(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                 false,  // ArtiklWithArtstat         
                                                 false,  // ArtStat        
                                                 true,  // Faktur         
                                                 false,  // Rtrans         
                                                         /*false*/ true,  // Kupdob         
                                                 false,  // Prjkt          
                                                 false,  // Rtrans4ruc     
                                                 false); // Artikl   
            break;

         case ZXC.VvReportEnum.RIZ_Rekap_PIX:
            theRiskFilter.TT = Faktur.TT_PIX;
            vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_Rekapitulacija_PIX(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                 false, // ArtiklWithArtstat         
                                                 false, // ArtStat        
                                                 true, // Faktur         
                                                 false, // Rtrans         
                                                 false, // Kupdob         
                                                 false, // Prjkt          
                                                 false, // Rtrans4ruc     
                                                 false);// Artikl         

            break;

         case ZXC.VvReportEnum.RIZ_Rekap_IRAvsFtrans:
         case ZXC.VvReportEnum.RIZ_Rekap_IRMvsFtrans:

            bool isIRA = vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_Rekap_IRAvsFtrans;

            if(!isIRA) theRiskFilter.TT = Faktur.TT_IRM;

            if(!RptR_Rekap_IRMvsFtrans.IsFilterWellFormed(isIRA, ((RiskReportUC)vvReportUC))) vvReport = null;
            else vvReport = new RptR_Rekap_IRMvsFtrans(isIRA,
                                                 reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                 false,  // ArtiklWithArtstat         
                                                 false,  // ArtStat        
                                                 true,  // Faktur         
                                                 false,  // Rtrans         
                                                 false,  // Kupdob         
                                                 false,  // Prjkt          
                                                 true,  // Rtrans4ruc     
                                                 false); // Artikl   
            break;

         case ZXC.VvReportEnum.RIZ_Rekap_RNM:
         case ZXC.VvReportEnum.RIZ_Rekap_RNM_S:
            //bool isPipArtiklRekap = false;

            if(ZXC.CURR_userName != ZXC.vvDB_programSuperUserName) break;
            theRiskFilter.TT = Faktur.TT_RNM;
            vvReport = new RptR_Rekap_RNM(/*isPipArtiklRekap*/ false, reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                 false, // ArtiklWithArtstat         
                                                 false, // ArtStat        
                                                 true, // Faktur         
                                                 true, // Rtrans         
                                                 false, // Kupdob         
                                                 false, // Prjkt          
                                                 false, // Rtrans4ruc     
                                                 false);// Artikl         

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_Rekap_RNM) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";

            break;

         case ZXC.VvReportEnum.RIZ_RNM_Proizvodi:
         case ZXC.VvReportEnum.RIZ_RNM_Proizvodi_S:

            if(ZXC.CURR_userName != ZXC.vvDB_programSuperUserName) break;

            theRiskFilter.TT = Faktur.TT_RNM;
            vvReport = new RptR_Rekap_RNM(/*isPipArtiklRekap*/ true, reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                 false, // ArtiklWithArtstat         
                                                 false, // ArtStat        
                                                 true, // Faktur         
                                                 true, // Rtrans         
                                                 false, // Kupdob         
                                                 false, // Prjkt          
                                                 false, // Rtrans4ruc     
                                                 false);// Artikl         

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_RNM_Proizvodi) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";

            break;

         case ZXC.VvReportEnum.XIZ_ZOBJ:

            theRiskFilter.TT = Faktur.TT_RNZ;
            theRiskFilter.AnalitSintet = "A";
            vvReport = new RptR_Rekap_RNZ_PoslJed(reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                 false, // ArtiklWithArtstat         
                                                 false, // ArtStat        
                                                 true, // Faktur         
                                                 false, // Rtrans         
                                                 false, // Kupdob         
                                                 false, // Prjkt          
                                                 false, // Rtrans4ruc     
                                                 false);// Artikl         
            break;

         //case ZXC.VvReportEnum.RIZ_Rekap_RNZ:
         case ZXC.VvReportEnum.RIZ_Rekap_RNZ:
            theRiskFilter.TT = Faktur.TT_RNZ;
            vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_Rekapitulacija_RNZ(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                 false, // ArtiklWithArtstat         
                                                 false, // ArtStat        
                                                 true, // Faktur         
                                                 false, // Rtrans         
                                                 false, // Kupdob         
                                                 false, // Prjkt          
                                                 false, // Rtrans4ruc     
                                                 false);// Artikl         

            break;

         case ZXC.VvReportEnum.SVD_FinIzlazByAgr:
         case ZXC.VvReportEnum.SVD_FinIzlazByAgr_S:

            bool IsANA;
            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.SVD_FinIzlazByAgr) { IsANA = true; theRiskFilter.AnalitSintet = "A"; }
            else { IsANA = false; theRiskFilter.AnalitSintet = "S"; }

            theRiskFilter.IsPrihodTT = theRiskFilter.IsRashodTT = false;
            theRiskFilter.TT = Faktur.TT_IZD;

            if(!ZXC.IsSvDUH) vvReport = null;
            else vvReport = new RptR_SVD_FinIzlaz(/* isByAgr */ true, IsANA, new Vektor.Reports.RIZ.CR_SVD_FinIzlaz(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                          false, // ArtiklWithArtstat         
                          false, // ArtStat        
                          false, // Faktur         
                          false, // Rtrans         
                          false, // Kupdob         ... kupi se direkt u RptR_SVD_FinIzlaz 
                          false, // Prjkt          
                          true, // Rtrans4ruc     
                          false);// Artikl         
            break;

         case ZXC.VvReportEnum.SVD_NEW_4KNJ_S:

            theRiskFilter.IsPrihodTT = theRiskFilter.IsRashodTT = false;
            theRiskFilter.TT = Faktur.TT_IZD;

            if(!ZXC.IsSvDUH) vvReport = null;
            else vvReport = new RptR_SVD_FinIzlaz(/* isByAgr */ true, false, new Vektor.Reports.RIZ.CR_SVD_FinIzlaz_4Knj(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                          false, // ArtiklWithArtstat         
                          false, // ArtStat        
                          false, // Faktur         
                          false, // Rtrans         
                          false, // Kupdob         ... kupi se direkt u RptR_SVD_FinIzlaz 
                          false, // Prjkt          
                          true, // Rtrans4ruc     
                          false);// Artikl         
            break;

         case ZXC.VvReportEnum.SVD_ArtikliKlinInv_Exp:

            theRiskFilter.IsPrihodTT = theRiskFilter.IsRashodTT = false;
            theRiskFilter.TT = Faktur.TT_IZD;

            if(!ZXC.IsSvDUH) vvReport = null;
            else vvReport = new RptR_SVD_FinIzlaz(/* isByAgr */ true, true, new Vektor.Reports.RIZ.CR_SVD_Artikli_KlinikaInv_Exp(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                          false, // ArtiklWithArtstat         
                          false, // ArtStat        
                          false, // Faktur         
                          false, // Rtrans         
                          false, // Kupdob         ... kupi se direkt u RptR_SVD_FinIzlaz 
                          false, // Prjkt          
                          true, // Rtrans4ruc     
                          false);// Artikl         
            break;


         case ZXC.VvReportEnum.SVD_FinIzlazBySkl:
         case ZXC.VvReportEnum.SVD_FinIzlazBySkl_S:

            //bool IsANA;
            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.SVD_FinIzlazBySkl) { IsANA = true; theRiskFilter.AnalitSintet = "A"; }
            else { IsANA = false; theRiskFilter.AnalitSintet = "S"; }

            theRiskFilter.IsPrihodTT = theRiskFilter.IsRashodTT = false;
            theRiskFilter.TT = Faktur.TT_IZD;

            if(!ZXC.IsSvDUH) vvReport = null;
            else vvReport = new RptR_SVD_FinIzlaz(/* isByAgr */ false, IsANA, new Vektor.Reports.RIZ.CR_SVD_FinIzlazGrSkl(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                           false, // ArtiklWithArtstat         
                           false, // ArtStat        
                           false, // Faktur         
                           false, // Rtrans         
                           false, // Kupdob         ... kupi se direkt u RptR_SVD_FinIzlaz 
                           false, // Prjkt          
                           true, // Rtrans4ruc     
                           false);// Artikl         
            break;

         case ZXC.VvReportEnum.SVD_TopListaPartnera:

            theRiskFilter.AnalitSintet = "A";

            if(!ZXC.IsSvDUH) vvReport = null;
            else vvReport = new RptR_SVD_FinIzlaz(/* isByAgr */ true, true, new Vektor.Reports.RIZ.CR_SVD_TopListPart(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                           false, // ArtiklWithArtstat         
                           false, // ArtStat        
                           false, // Faktur         
                           false, // Rtrans         
                           false, // Kupdob         ... kupi se direkt u RptR_SVD_FinIzlaz 
                           false, // Prjkt          
                           true, // Rtrans4ruc     
                           false);// Artikl         
            break;


         case ZXC.VvReportEnum.SVD_PlanRealizUGO:
         case ZXC.VvReportEnum.SVD_PlanRealizUGO_S:

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.SVD_PlanRealizUGO) { IsANA = true; theRiskFilter.AnalitSintet = "A"; }
            else { IsANA = false; theRiskFilter.AnalitSintet = "S"; }

            theRiskFilter.IsPrihodTT = theRiskFilter.IsRashodTT = false;
            //theRiskFilter.TT         = Faktur.TT_URA;
            //theRiskFilter.DatumOd    = ZXC.projectYearFirstDay; // ali samo za URA-e, za UGO-e ide bez ikakvog dateOD-a 
            theRiskFilter.TT = Faktur.TT_UGO;
            theRiskFilter.DatumOd = DateTime.MinValue;

            if(!ZXC.IsSvDUH) vvReport = null;
            else vvReport = new RptR_Rekap_SVD_PlanRealizUGO(/*fakturGR,*/ new Vektor.Reports.RIZ.CR_SVD_PlanRealizUGO(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                     false, // ArtiklWithArtstat         
                                     false, // ArtStat        
                                     true, // Faktur         
                                     true, // Rtrans         
                                     false, // Kupdob         
                                     false, // Prjkt          
                                     false, // Rtrans4ruc     
                                     false);// Artikl         

            break;


         case ZXC.VvReportEnum.SVD_URA_4Knjigovod:
         case ZXC.VvReportEnum.SVD_URA_4Knjigovod_S:

            if(!ZXC.IsSvDUH) vvReport = null;
            else vvReport = new RptR_SVD_URA4Knjigovodstvo(fakturGR, new Vektor.Reports.RIZ.CR_SVD_URA4Knjigovodstvo(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                           false, // ArtiklWithArtstat         
                           false, // ArtStat        
                           true, // Faktur         
                           false, // Rtrans         
                           false, // Kupdob         
                           false, // Prjkt          
                           false, // Rtrans4ruc     
                           false);// Artikl         

            theRiskFilter.TT = Faktur.TT_URA;

            // 24.12.2019: ugaslili, a niti ne znamo zasto smo ikada upalili?
            //theRiskFilter.DatumOd = ZXC.projectYearFirstDay;

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.SVD_URA_4Knjigovod) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";
            break;

         #endregion REALIZACIJA

         #region SKLADISTE

         case ZXC.VvReportEnum.RIZ_StanjeSklad_A:
         case ZXC.VvReportEnum.RIZ_StanjeSklad_A_S:
            // 11.03.2019.samo po potrebi za export SvDuh ALMP, potrebno je jednom godisnje i mi to radimo pa nije zaseban rpt vec samo cr      
            //vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_SVD_ALMP()     , reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
            vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_StanjeSklad_A(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                                  true, // ArtiklWithArtstat         
                                                  false, // ArtStat        
                                                  false, // Faktur         
                                                  false, // Rtrans         
                                                  false, // Kupdob         
                                                  false, // Prjkt          
                                                  false, // Rtrans4ruc     
                                                  false);// Artikl         

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_StanjeSklad_A) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";
            break;

         case ZXC.VvReportEnum.RIZ_StanjeSklad_AP:
         case ZXC.VvReportEnum.RIZ_StanjeSklad_AP_S:
            vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_StanjeSklad_AP(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                                  true, // ArtiklWithArtstat         
                                                  false, // ArtStat        
                                                  false, // Faktur         
                                                  false, // Rtrans         
                                                  false, // Kupdob         
                                                  false, // Prjkt          
                                                  false, // Rtrans4ruc     
                                                  false);// Artikl         

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_StanjeSklad_AP) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";
            break;

         case ZXC.VvReportEnum.RIZ_StanjeSklad_B:
         case ZXC.VvReportEnum.RIZ_StanjeSklad_B_S:
            vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_StanjeSklad_B(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                                  true, // ArtiklWithArtstat         
                                                  true, // ArtStat        
                                                  false, // Faktur         
                                                  false, // Rtrans         
                                                  false, // Kupdob         
                                                  false, // Prjkt          
                                                  false, // Rtrans4ruc     
                                                  false);// Artikl         

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_StanjeSklad_B) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";
            break;

         case ZXC.VvReportEnum.RIZ_LagerLista:
         case ZXC.VvReportEnum.RIZ_LagerLista_S:
            vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_LagerLista(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                                  true, // ArtiklWithArtstat         
                                                  false, // ArtStat        
                                                  false, // Faktur         
                                                  false, // Rtrans         
                                                  false, // Kupdob         
                                                  false, // Prjkt          
                                                  false, // Rtrans4ruc     
                                                  false);// Artikl         

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_LagerLista) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";
            break;

         case ZXC.VvReportEnum.RIZ_LagerLista_Kol_A:
            vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_LagerLista_Kol(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                                  true, // ArtiklWithArtstat         
                                                  false, // ArtStat        
                                                  false, // Faktur         
                                                  false, // Rtrans         
                                                  false, // Kupdob         
                                                  false, // Prjkt          
                                                  false, // Rtrans4ruc     
                                                  false);// Artikl         

            theRiskFilter.AnalitSintet = "A";
            break;

         case ZXC.VvReportEnum.RIZ_LagerLista_OP_A:
         case ZXC.VvReportEnum.RIZ_LagerLista_OP_S:
            vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_LagerLista_OP(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                                  true, // ArtiklWithArtstat         
                                                  false, // ArtStat        
                                                  false, // Faktur         
                                                  false, // Rtrans         
                                                  false, // Kupdob         
                                                  false, // Prjkt          
                                                  false, // Rtrans4ruc     
                                                  false);// Artikl         

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_LagerLista_OP_A) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";
            break;

         case ZXC.VvReportEnum.RIZ_LagerLista_OPSkl_A:
         case ZXC.VvReportEnum.RIZ_LagerLista_OPSkl_S:
            vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_LagerListaPodJM_SvaSkl(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                                  true, // ArtiklWithArtstat         
                                                  false, // ArtStat        
                                                  false, // Faktur         
                                                  false, // Rtrans         
                                                  false, // Kupdob         
                                                  false, // Prjkt          
                                                  false, // Rtrans4ruc     
                                                  false);// Artikl         

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_LagerLista_OPSkl_A) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";
            break;



         case ZXC.VvReportEnum.RIZ_StanjeSklad_Kol: vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_StanjeSklad_K(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                                  true, // ArtiklWithArtstat         
                                                  false, // ArtStat        
                                                  false, // Faktur         
                                                  false, // Rtrans         
                                                  false, // Kupdob         
                                                  false, // Prjkt          
                                                  false, // Rtrans4ruc     
                                                  false);// Artikl         

            break;


         case ZXC.VvReportEnum.RIZ_SkladBilten:

            // 03.04.2019: 
            if(ZXC.IsTEMBO)
            {
               vvReport = new RptR_Artikl2BCterminal(reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl);

               theRiskFilter.Artikl_TS = "ROB";
               theRiskFilter.SkladCD = "VPSK";
            }
            else if(ZXC.vvDB_VvDomena == "vvMN")
            {
               vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_LagreEtikete(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                       true, // ArtiklWithArtstat         
                                       false, // ArtStat        
                                       false, // Faktur         
                                       false, // Rtrans         
                                       false, // Kupdob         
                                       false, // Prjkt          
                                       false, // Rtrans4ruc     
                                       false);// Artikl         
            }
            else
            {
               vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_SkladBilten(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                       false, // ArtiklWithArtstat         
                                       false, // ArtStat        
                                       false, // Faktur         
                                       false, // Rtrans         
                                       false, // Kupdob         
                                       false, // Prjkt          
                                       false, // Rtrans4ruc     
                                       true);// Artikl         

               theRiskFilter.SviArtikli = true; // na dan 16.09.2016 ovo je jedino mjesto gdje se koristi ova bussiness varijabla. Nema je uopce na RiskReportUC-u 
            }

            theRiskFilter.AnalitSintet = "A";

            break;

         case ZXC.VvReportEnum.RIZ_InventurnaLista:
            vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_InventurnaLista(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                                  true, // ArtiklWithArtstat         
                                                  false, // ArtStat        
                                                  false, // Faktur         
                                                  false, // Rtrans         
                                                  false, // Kupdob         
                                                  false, // Prjkt          
                                                  false, // Rtrans4ruc     
                                                  false);// Artikl         
            theRiskFilter.AnalitSintet = "A";
            break;

         case ZXC.VvReportEnum.RIZ_InventurneRazlike:
         case ZXC.VvReportEnum.RIZ_InventurnoStanje:
         case ZXC.VvReportEnum.RIZ_InventurneRazlike_S:
         case ZXC.VvReportEnum.RIZ_InventurnoStanje_S:
            vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_InventurneRazlike(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                                   true, // ArtiklWithArtstat         
                                                   false, // ArtStat        
                                                   false, // Faktur         
                                                   false, // Rtrans         
                                                   false, // Kupdob         
                                                   false, // Prjkt          
                                                   false, // Rtrans4ruc     
                                                   false);// Artikl         
                                                          //theRiskFilter.AnalitSintet = "A";
                                                          //if(ZXC.IsTEXTHOany) theRiskFilter.DatumDo = ZXC.projectYearLastDay ;
            if(ZXC.IsTEXTHOany) theRiskFilter.DatumDo = ZXC.TexthoInventuraDate;

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_InventurneRazlike ||
               vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_InventurnoStanje) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";


            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_InventurneRazlike ||
               vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_InventurneRazlike_S) theRiskFilter.IsTrue = true;
            else theRiskFilter.IsTrue = false;

            break;

         case ZXC.VvReportEnum.RIZ_KretanjeSklad_S:

            if(!RptR_KretanjeSklad.IsFilterWellFormed(((RiskReportUC)vvReportUC))) vvReport = null;
            else vvReport = new RptR_KretanjeSklad(reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl);
            break;

         case ZXC.VvReportEnum.RIZ_StanjeSkladPoPRJ:

            if(!RptR_StanjeSkladPoPRJ.IsFilterWellFormed(((RiskReportUC)vvReportUC))) vvReport = null;
            else vvReport = new RptR_StanjeSkladPoPRJ(reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl);
            break;

         case ZXC.VvReportEnum.RIZ_StanjeReversa:

            if(!RptR_StanjeReversa.IsFilterWellFormed(((RiskReportUC)vvReportUC))) vvReport = null;
            else vvReport = new RptR_StanjeReversa(reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl);
            break;

         case ZXC.VvReportEnum.RIZ_PrometRazdoblja:
         case ZXC.VvReportEnum.RIZ_PrometRazdoblja_S:

            if(theRiskFilter.DatumOd == ZXC.projectYearFirstDay)
            {
               ZXC.aim_emsg(MessageBoxIcon.Warning, "Promet Razdoblja nema smisla tražiti za razdobljeOD koje je 01.01. tekuće godine.\n\nOdaberite izvještaj 'Stanje Skladišta'.");
               vvReport = null; break;
            }

            vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_PrometRazdoblja(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                                  true, // ArtiklWithArtstat         
                                                  false, // ArtStat        
                                                  false, // Faktur         
                                                  false, // Rtrans         
                                                  false, // Kupdob         
                                                  false, // Prjkt          
                                                  false, // Rtrans4ruc     
                                                  false);// Artikl         

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_PrometRazdoblja) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";

          //theRiskFilter.SviArtikli = true; // na dan 16.06.2025 ovo je drugo mjesto gdje se koristi ova bussiness varijabla. Nema je uopce na RiskReportUC-u 

            break;

         case ZXC.VvReportEnum.RIZ_PrometRazdoblja_OP_A:
         case ZXC.VvReportEnum.RIZ_PrometRazdoblja_OP_S:

            if(theRiskFilter.DatumOd == ZXC.projectYearFirstDay)
            {
               ZXC.aim_emsg(MessageBoxIcon.Warning, "Promet Razdoblja nema smisla tražiti za razdobljeOD koje je 01.01. tekuće godine.\n\nOdaberite izvještaj 'Stanje Skladišta'.");
               vvReport = null; break;
            }

            vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_PrometRazdoblja_OP(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                                  true, // ArtiklWithArtstat         
                                                  false, // ArtStat        
                                                  false, // Faktur         
                                                  false, // Rtrans         
                                                  false, // Kupdob         
                                                  false, // Prjkt          
                                                  false, // Rtrans4ruc     
                                                  false);// Artikl         

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_PrometRazdoblja_OP_A) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";
            break;



         case ZXC.VvReportEnum.RIZ_PrometArtikla:
         case ZXC.VvReportEnum.RIZ_PrometArtikla_S:

            bool needsKpdbList = theRiskFilter.GrupiranjeDokum == "KupdobName" ||
                                 theRiskFilter.GrupiranjeDokum == "MtrosName";

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_PrometArtikla) theRiskFilter.AnalitSintet = "A";
            else theRiskFilter.AnalitSintet = "S";

            //if(rbt_grupUser   .Checked) return "AddUID"    ;
            //if(rbt_grupPartner.Checked) return "KupdobName"; NO FakturList needed! KupdobSifrar has it 
            //if(rbt_grupMjTros .Checked) return "MtrosName" ; NO FakturList needed! KupdobSifrar has it 
            //if(rbt_grupValuta .Checked) return "DevName"   ;
            //if(rbt_grupProjekt.Checked) return "ProjektCD" ;
            //if(rbt_grupNacPl  .Checked) return "NacPlac"   ;
            //if(rbt_grupTT     .Checked) return "TT"        ; NO FakturList needed! rtrans_rec has it 
            //if(rbt_grupMonth  .Checked) return "DokMonth"  ; NO FakturList needed! rtrans_rec has it 
            //if(rbt_grupSkladCD.Checked) return "SkladCD"   ; NO FakturList needed! rtrans_rec has it 
              
                 if(theRiskFilter.IsFor_SVD_CheckUgovor)                                                                                           repDoc = new Vektor.Reports.RIZ.CR_SVD_PrmArt_Ulaz();
            else if(ZXC.TtInfo(theRiskFilter.TT).IsMalopFin_U || theRiskFilter.IsMalopUlazForPrmArtTT == true)                                     repDoc = new Vektor.Reports.RIZ.CR_PrometArtikla_GrMalopU();//dodano 29.08.2023.
            else if(ZXC.TtInfo(theRiskFilter.TT).IsRashodTT   || theRiskFilter.IsVelepUlazForPrmArtTT == true || theRiskFilter.IsRashodTT == true) repDoc = new Vektor.Reports.RIZ.CR_PrometArtikla_GrVelepU();//dodano 17.06.2025.
            else                                                                                                                                   repDoc = new Vektor.Reports.RIZ.CR_PrometArtikla_Grup();

            //            if(isArtiklGR || isFakturGR) repDoc = new Vektor.Reports.RIZ.CR_PrometArtikla_Grup();
            //            else                         repDoc = new Vektor.Reports.RIZ.CR_PrometArtikla     ();

            if(!RptR_PrometArtikla.IsFilterWellFormed(((RiskReportUC)vvReportUC))) vvReport = null;
                                                                                   // 16.06.2025: 
                                                                                   // 26.06.2025: vracamo na staro
          //else                                                                   vvReport = new RptR_PrometArtikla(false, artiklGR, fakturGR, repDoc, reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
          //else                                                                   vvReport = new RptR_PrometArtikla(false, artiklGR, fakturGR, repDoc, reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Rtrans,
            else                                                                   vvReport = new RptR_PrometArtikla(false, artiklGR, fakturGR, repDoc, reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,

                                                    false, // ArtiklWithArtstat         
                                                    false, // ArtStat        
  RptR_PrometArtikla.
  IsFakGrDataInFakturBussiness(fakturGR) || needsKpdbList, // Faktur         
                                                    false, // Rtrans         
                                            needsKpdbList, // Kupdob         
                                                    false, // Prjkt          
                                                     true, // Rtrans4ruc     
                                             //isArtiklGR);// Artikl         
        theRiskFilter.IsFor_SVD_CheckUgovor || isArtiklGR);// Artikl         

            
            break;

         case ZXC.VvReportEnum.SVD_PrmArt4Nabava:

            repDoc = new Vektor.Reports.RIZ.CR_SVD_PrmArt4Nabava();
          //23.12.2022. - ovo makni ako hoces bez grupa po klinikama!!!
            theRiskFilter.GrupiranjeDokum = fakturGR = "SVDklinika";

            if(!RptR_PrometArtikla.IsFilterWellFormed(((RiskReportUC)vvReportUC))) vvReport = null;
            else vvReport = new RptR_PrometArtikla(false, artiklGR, fakturGR, repDoc, reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                    false, // ArtiklWithArtstat         
                                                    false, // ArtStat        
                                                    true , // Faktur         
                                                    false, // Rtrans         
                                                     true, // Kupdob         
                                                    false, // Prjkt          
                                                     true, // Rtrans4ruc     
                                                           //isArtiklGR);// Artikl         
                                                     true);// Artikl         

            break;

         case ZXC.VvReportEnum.RIZ_PrometArtikla_OP_A:
         case ZXC.VvReportEnum.RIZ_PrometArtikla_OP_S:

            needsKpdbList = theRiskFilter.GrupiranjeDokum == "KupdobName" ||
                            theRiskFilter.GrupiranjeDokum == "MtrosName";

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_PrometArtikla_OP_A) theRiskFilter.AnalitSintet = "A";
            else                                                                       theRiskFilter.AnalitSintet = "S";

          //if(ZXC.IsTETRAGRAM_ANY) repDoc = new Vektor.Reports.RIZ.CR_IMPRizj(); 
          //else                    repDoc = new Vektor.Reports.RIZ.CR_PrometArtikla_Grup();
            if(ZXC.IsTETRAGRAM_ANY && (vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_PrometArtikla_OP_A)) repDoc = new Vektor.Reports.RIZ.CR_IMPRizj(); 
            else                                                                                                repDoc = new Vektor.Reports.RIZ.CR_PrometArtikla_Grup();


            if(!RptR_PrometArtikla.IsFilterWellFormed(((RiskReportUC)vvReportUC))) vvReport = null;
            else                                                                   vvReport = new RptR_PrometArtikla(true, artiklGR, fakturGR, repDoc, reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                    false, // ArtiklWithArtstat         
                                                    false, // ArtStat        
  RptR_PrometArtikla.
  IsFakGrDataInFakturBussiness(fakturGR) || needsKpdbList, // Faktur         
                                                    false, // Rtrans         
                                            needsKpdbList, // Kupdob         
                                                    false, // Prjkt          
                                                    true, // Rtrans4ruc     
                                               isArtiklGR);// Artikl         


            break;





         case ZXC.VvReportEnum.RIZ_ProdajaPoDobav_A:
         case ZXC.VvReportEnum.RIZ_ProdajaPoDobav_S:

            theRiskFilter.IsPrihodTT = true;

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_ProdajaPoDobav_A)  theRiskFilter.AnalitSintet = "A";
            else                                                                      theRiskFilter.AnalitSintet = "S";
      
            repDoc = new Vektor.Reports.RIZ.CR_ProdajaPoDobavljacu();

            vvReport = new RptR_ProdajaPoDobav(false, artiklGR, fakturGR, repDoc, reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                    false, // ArtiklWithArtstat         
                                                    false, // ArtStat        
                                                    false, // Faktur         
                                                    false, // Rtrans         
                                                    false, // Kupdob         
                                                    false, // Prjkt          
                                                    true , // Rtrans4ruc     
                                               isArtiklGR);// Artikl         

            break;

         case ZXC.VvReportEnum.RIZ_ProdajaPoDobav_B_A:
         case ZXC.VvReportEnum.RIZ_ProdajaPoDobav_B_S:

            theRiskFilter.IsPrihodTT = true;

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_ProdajaPoDobav_B_A) theRiskFilter.AnalitSintet = "A";
            else                                                                       theRiskFilter.AnalitSintet = "S";

            repDoc = new Vektor.Reports.RIZ.CR_ProdajaPoDobavljacu_B();

            vvReport = new RptR_ProdajaPoDobav(true, artiklGR, fakturGR, repDoc, reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                    false, // ArtiklWithArtstat         
                                                    false, // ArtStat        
                                                    true , // Faktur         
                                                    false, // Rtrans         
                                                    false, // Kupdob         
                                                    false, // Prjkt          
                                                    true , // Rtrans4ruc     
                                               isArtiklGR);// Artikl         

            break;


         case ZXC.VvReportEnum.RIZ_PromStSkladDobav_A:
         case ZXC.VvReportEnum.RIZ_PromStSkladDobav_S:

            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_PromStSkladDobav_A) theRiskFilter.AnalitSintet = "A";
            else                                                                       theRiskFilter.AnalitSintet = "S";

            repDoc = new Vektor.Reports.RIZ.CR_StanjeSkladPoDob();

            vvReport = new RptR_StanjePoDobav(artiklGR, fakturGR, repDoc, reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                    false, // ArtiklWithArtstat         
                                                    false, // ArtStat        
                                                    false, // Faktur         
                                                    false, // Rtrans         
                                                    false, // Kupdob         
                                                    false, // Prjkt          
                                                    true , // Rtrans4ruc     
                                               isArtiklGR);// Artikl         

            break;

         case ZXC.VvReportEnum.RIZ_StanjeSklad_AMB:
         case ZXC.VvReportEnum.RIZ_StanjeSklad_AMB_S: 
            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_StanjeSklad_AMB) theRiskFilter.AnalitSintet = "A";
            else                                                                     theRiskFilter.AnalitSintet = "S";

            vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_StanjeSklad_AMB(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                      true, // ArtiklWithArtstat         
                                      false, // ArtStat        
                                      false, // Faktur         
                                      false, // Rtrans         
                                      false, // Kupdob         
                                      false, // Prjkt          
                                      false, // Rtrans4ruc     
                                      false);// Artikl         

            break;

         case ZXC.VvReportEnum.RIZ_StanjeSklad_OP_A:
         case ZXC.VvReportEnum.RIZ_StanjeSklad_OP_S: 
            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_StanjeSklad_OP_A) theRiskFilter.AnalitSintet = "A";
            else                                                                     theRiskFilter.AnalitSintet = "S";

            vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_StanjeSklad_OP(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                      true, // ArtiklWithArtstat         
                                      false, // ArtStat        
                                      false, // Faktur         
                                      false, // Rtrans         
                                      false, // Kupdob         
                                      false, // Prjkt          
                                      false, // Rtrans4ruc     
                                      false);// Artikl         

            break;

         case ZXC.VvReportEnum.SVD_PrmStSkl_ProsWeek:

            if(!ZXC.IsSvDUH) vvReport = null;
            else             vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_SVD_PrmtStanjSklad(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl,
                                                  true, // ArtiklWithArtstat         
                                                  false, // ArtStat        
                                                  false, // Faktur         
                                                  false, // Rtrans         
                                                  false, // Kupdob         
                                                  false, // Prjkt          
                                                  false, // Rtrans4ruc     
                                                  false);// Artikl         

            theRiskFilter.AnalitSintet = "A";

            break;

         case ZXC.VvReportEnum.RIZ_TemboWebShopExport:

            vvReport = new RptR_TemboWebShopExport(new Vektor.Reports.RIZ.CR_TemboWebShopExport(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl);
            break;

         case ZXC.VvReportEnum.RIZ_Jeftinije_hr_Export:
         
            vvReport = new RptR_Jeftinije_hr_Export(new Vektor.Reports.RIZ.CR_Jeftinije_hr_Export(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl);
            break;

         case ZXC.VvReportEnum.SVD_HALMED_Potrosnja:

            vvReport = new RptR_SVD_HALMED_Potrosnja(new Vektor.Reports.RIZ.CR_SVD_ALMP(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl);

            theRiskFilter.SkladCD = ""; // !!! jer trazimo sumarne izlaze sa SVIH skladista 

            vvReport.IsForExport = true;

            break;

         case ZXC.VvReportEnum.SVD_HALMED_Provjera:

            vvReport = new RptR_SVD_HALMED_Potrosnja(new Vektor.Reports.RIZ.CR_SVD_HALMED(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Artikl);

            theRiskFilter.SkladCD = ""; // !!! jer trazimo sumarne izlaze sa SVIH skladista 

            vvReport.IsForExport = false;

            break;

         #endregion SKLADISTE

         #region PDV, PPMV, PNP

         case ZXC.VvReportEnum.RIZ_PDV_URA      : vvReport = new RptR_PDV_Knjiga  (new Vektor.Reports.RIZ.CR_KnjigaURA()   , reportName, theRiskFilter, true); break;
         case ZXC.VvReportEnum.RIZ_PDV_IRA      : vvReport = new RptR_PDV_Knjiga  (new Vektor.Reports.RIZ.CR_KnjigaIRA()   , reportName, theRiskFilter, false); break;
         case ZXC.VvReportEnum.RIZ_PDV_PDV      : vvReport = new RptR_PDV_PDV     (new Vektor.Reports.RIZ.CR_PDVobrazac()  , reportName, theRiskFilter, false); break;
         case ZXC.VvReportEnum.RIZ_PDV_PDVk     : vvReport = new RptR_PDV_PDV     (new Vektor.Reports.RIZ.CR_PDVobrazac()  , reportName, theRiskFilter, true); break;

         case ZXC.VvReportEnum.RIZ_PDV_URA_2012 : vvReport = new RptR_PDV_Knjiga(new Vektor.Reports.RIZ.CR_KnjigaURA_2012() , reportName, theRiskFilter, true) ; break;
         case ZXC.VvReportEnum.RIZ_PDV_IRA_2012 : vvReport = new RptR_PDV_Knjiga(new Vektor.Reports.RIZ.CR_KnjigaIRA_2012() , reportName, theRiskFilter, false); break;
         case ZXC.VvReportEnum.RIZ_PDV_PDV_2012 : vvReport = new RptR_PDV_PDV   (new Vektor.Reports.RIZ.CR_PDVobrazac_2012(), reportName, theRiskFilter, false); break;
         case ZXC.VvReportEnum.RIZ_PDV_PDVk_2012: vvReport = new RptR_PDV_PDV   (new Vektor.Reports.RIZ.CR_PDVobrazac_2012(), reportName, theRiskFilter, true) ; break;

         case ZXC.VvReportEnum.RIZ_PDV_URA_EU : vvReport = new RptR_PDV_Knjiga(new Vektor.Reports.RIZ.CR_KnjigaURA_EU()    , reportName, theRiskFilter, true) ; break;
         case ZXC.VvReportEnum.RIZ_PDV_IRA_EU : vvReport = new RptR_PDV_Knjiga(new Vektor.Reports.RIZ.CR_KnjigaIRA_EU()    , reportName, theRiskFilter, false); break;

         case ZXC.VvReportEnum.RIZ_ObrazacPDV_MU: vvReport = new RptR_PDV_Knjiga(new Vektor.Reports.RIZ.CR_PDV_MU(), reportName, theRiskFilter, true ); theRiskFilter.PdvKnjiga = ZXC.PdvKnjigaEnum.PDV_RUC; break;
         case ZXC.VvReportEnum.RIZ_ObrazacPDV_MI: vvReport = new RptR_PDV_Knjiga(new Vektor.Reports.RIZ.CR_PDV_MI(), reportName, theRiskFilter, false); theRiskFilter.PdvKnjiga = ZXC.PdvKnjigaEnum.PDV_RUC; break;


         //case ZXC.VvReportEnum.RIZ_PDV_PDV_EU : vvReport = new RptR_PDV_PDV   (new Vektor.Reports.RIZ.CR_PDVorazac_EU2013(), reportName, theRiskFilter, false); break;
         // od 01.01.2014. prosiren pdv obrazac
         case ZXC.VvReportEnum.RIZ_PDV_PDV_EU :
          //if(theRiskFilter.DatumOd >= ZXC.PdvEU_EraDate && theRiskFilter.DatumOd < ZXC.Date01012014) vvReport = new RptR_PDV_PDV(new Vektor.Reports.RIZ.CR_PDVorazac_EU2013(), reportName, theRiskFilter, false);
          //if(theRiskFilter.DatumOd < ZXC.Date01012014) vvReport = new RptR_PDV_PDV(new Vektor.Reports.RIZ.CR_PDVorazac_EU2013(), reportName, theRiskFilter, false);
          //else                                         vvReport = new RptR_PDV_PDV(new Vektor.Reports.RIZ.CR_PDVobrazac_2014(), reportName, theRiskFilter, false);
                 if(theRiskFilter.DatumOd <  ZXC.Date01012014 ) vvReport = new RptR_PDV_PDV(new Vektor.Reports.RIZ.CR_PDVorazac_EU2013(), reportName, theRiskFilter, false);
            else if(theRiskFilter.DatumOd >= ZXC.Date01012014 && 
                    theRiskFilter.DatumOd < ZXC.Date01012023  ) vvReport = new RptR_PDV_PDV(new Vektor.Reports.RIZ.CR_PDVobrazac_2014(), reportName, theRiskFilter, false);
               else /*od 01.01.2023.*/                          vvReport = new RptR_PDV_PDV(new Vektor.Reports.RIZ.CR_PDVobrazac_2023(), reportName, theRiskFilter, false);
            break;

       //case ZXC.VvReportEnum.RIZ_PDV_PDVk_2013: vvReport = new RptR_PDV_PDV(new Vektor.Reports.RIZ.CR_PDVorazac_EU2013(), reportName, theRiskFilter, true); break;
         case ZXC.VvReportEnum.RIZ_PDV_PDVk_2013: 
            if(theRiskFilter.DatumOd < ZXC.Date01012014) vvReport = new RptR_PDV_PDV(new Vektor.Reports.RIZ.CR_PDVorazac_EU2013(), reportName, theRiskFilter, true); 
            else                                         vvReport = new RptR_PDV_PDV(new Vektor.Reports.RIZ.CR_PDVobrazac_2014() , reportName, theRiskFilter, true); 
            break;
            
            
            

       //case ZXC.VvReportEnum.RIZ_ObrazacPPO: vvReport = new RptR_PDV_PPO(new Vektor.Reports.RIZ.CR_Obrazac_PPO(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
       //                             false, // ArtiklWithArtstat         
       //                             false, // ArtStat        
       //                             true , // Faktur         
       //                             false, // Rtrans         
       //                             false, // Kupdob         
       //                             false, // Prjkt          
       //                             false, // Rtrans4ruc     
       //                             false);// Artikl         
       //                             break;
         case ZXC.VvReportEnum.RIZ_ObrazacPPO: vvReport = new RptR_PDV_PPO(new Vektor.Reports.RIZ.CR_Obrazac_PPO(), reportName, theRiskFilter);
                                      break;


         case ZXC.VvReportEnum.RIZ_ObrazacPDVS_EU_A:
         case ZXC.VvReportEnum.RIZ_ObrazacPDVS_EU  : vvReport = new RptR_EU_PdvGEOkind(new Vektor.Reports.RIZ.CR_Obrazac_PDV_S(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                      false, // ArtiklWithArtstat         
                                      false, // ArtStat        
                                      true , // Faktur         
                                      false, // Rtrans         
                                      false, // Kupdob         
                                      false, // Prjkt          
                                      false, // Rtrans4ruc     
                                      false, // Artikl         

                                      false); // RptR_EU_PdvGEOkind ___ONLY!___: some TTs 
            
                                      if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_ObrazacPDVS_EU_A) theRiskFilter.AnalitSintet = "A";
                                      else                                                                     theRiskFilter.AnalitSintet = "S";

                                      break;

         case ZXC.VvReportEnum.RIZ_ObrazacZP_EU_A:
         case ZXC.VvReportEnum.RIZ_ObrazacZP_EU  : vvReport = new RptR_EU_PdvGEOkind(new Vektor.Reports.RIZ.CR_Obrazac_ZP(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                      false, // ArtiklWithArtstat         
                                      false, // ArtStat        
                                      true , // Faktur         
                                      false, // Rtrans         
                                      false, // Kupdob         
                                      false, // Prjkt          
                                      false, // Rtrans4ruc     
                                      false, // Artikl         

                                      true); // RptR_EU_PdvGEOkind ___ONLY!___: some TTs 

                                      if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_ObrazacZP_EU_A) theRiskFilter.AnalitSintet = "A";
                                      else                                                                   theRiskFilter.AnalitSintet = "S";
                                     
                                      break;

          case ZXC.VvReportEnum.RIZ_PPMV_Prilog9: 
            vvReport = new RptR_PPMV_Prilog9(reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
                                                 false, // ArtiklWithArtstat         
                                                 false, // ArtStat        
                                                 true , // Faktur         
                                                 false, // Rtrans         
                                                 false, // Kupdob         
                                                 false, // Prjkt          
                                                 false, // Rtrans4ruc     
                                                 /*true*/ false);// Artikl         .. se treba join-ati sa PrjArtCD a NE sa rtransom 
            //vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_Ppmv_Prilog9(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
            //                                      false, // ArtiklWithArtstat         
            //                                      false, // ArtStat        
            //                                      true,  // Faktur         
            //                                      false, // Rtrans         
            //                                      false, // Kupdob         
            //                                      false, // Prjkt          
            //                                      false, // Rtrans4ruc     
            //                                      true); // Artikl         
            break;

          case ZXC.VvReportEnum.RIZ_Rekap_PNP_Rtrans:
            theRiskFilter.IsPrihodTT = true; // !!! 
            vvReport = new RptR_Rekap_PNP_Rtrans(reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur);
            break;

          case ZXC.VvReportEnum.RIZ_IntrastatUlaz : vvReport = new RptR_Intrastat(new Vektor.Reports.RIZ.CR_Intrastat(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,

                                      false, // ArtiklWithArtstat         
                                      false, // ArtStat        
                                      true , // Faktur         
                                      true , // Rtrans         
                                      true , // Kupdob         
                                      false, // Prjkt          
                                      false, // Rtrans4ruc     
                                      true , // Artikl         

                                      true); // isUlaz 
            theRiskFilter.TT = Faktur.TT_URA;
            
            break;

          case ZXC.VvReportEnum.RIZ_IntrastatIzlaz : vvReport = new RptR_Intrastat(new Vektor.Reports.RIZ.CR_Intrastat(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,

                                      false, // ArtiklWithArtstat         
                                      false, // ArtStat        
                                      true , // Faktur         
                                      true , // Rtrans         
                                      true , // Kupdob         
                                      false, // Prjkt          
                                      false, // Rtrans4ruc     
                                      true , // Artikl         

                                      false); // isUlaz 
            theRiskFilter.TT = Faktur.TT_IRA;

            break;

         #endregion PDV, PPMV, PNP

         #region IOS

         case ZXC.VvReportEnum.RIZ_OPZSTAT1:
         case ZXC.VvReportEnum.RIZ_OPZSTAT1_S:

            if(!RptR_OPZ_Stat1.IsFilterWellFormed(((RiskReportUC)vvReportUC))) vvReport = null;
            else 
            {
                vvReport = new RptR_OPZ_Stat1   (new Vektor.Reports.RIZ.CR_OPZ_STAT_1(), reportName, theRiskFilter, true);
                if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_OPZSTAT1) theRiskFilter.AnalitSintet = "A";
                else                                                             theRiskFilter.AnalitSintet = "S";
                theRiskFilter.IsOtsDospOnly = true;
            }
            break;

         case ZXC.VvReportEnum.RIZ_DUGOVANJA:
         case ZXC.VvReportEnum.RIZ_DUGOVANJA_S: 
                vvReport = new RptR_Dugovanja   (new Vektor.Reports.RIZ.CR_DugPotInRisk()     , reportName, theRiskFilter, false);
                if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_DUGOVANJA) theRiskFilter.AnalitSintet = "A";
                else                                                              theRiskFilter.AnalitSintet = "S";
                break;

         case ZXC.VvReportEnum.RIZ_DUGOVANJA_Kum:
                vvReport = new RptR_Dugovanja   (new Vektor.Reports.RIZ.CR_DugPotInRisk_Kum() , reportName, theRiskFilter, false);
                if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_DUGOVANJA_Kum) theRiskFilter.AnalitSintet = "A";
                else                                                                  theRiskFilter.AnalitSintet = "S";
                break;

         case ZXC.VvReportEnum.RIZ_POTRAZIVANJA:
         case ZXC.VvReportEnum.RIZ_POTRAZIVANJA_S: 
            vvReport = new RptR_Potrazivanja(new Vektor.Reports.RIZ.CR_DugPotInRisk()     , reportName, theRiskFilter, true );
            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_POTRAZIVANJA) theRiskFilter.AnalitSintet = "A";
            else                                                                 theRiskFilter.AnalitSintet = "S";
            break;

         case ZXC.VvReportEnum.RIZ_POTRAZIVANJA_Kum:
            vvReport = new RptR_Potrazivanja(new Vektor.Reports.RIZ.CR_DugPotInRisk_Kum() , reportName, theRiskFilter, true );
            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_POTRAZIVANJA_Kum) theRiskFilter.AnalitSintet = "A";
            else                                                                     theRiskFilter.AnalitSintet = "S";
            break;
        
         case ZXC.VvReportEnum.RIZ_KarticaKupca:
         case ZXC.VvReportEnum.RIZ_KarticaKupca_S: 
            vvReport = new RptR_KarticaKupca(new Vektor.Reports.RIZ.CR_KarticaPartnera(), reportName, theRiskFilter, true );
            if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_KarticaKupca) theRiskFilter.AnalitSintet = "A";
            else                                                                 theRiskFilter.AnalitSintet = "S";
            break;

         case ZXC.VvReportEnum.RIZ_KarticaDobav:
         case ZXC.VvReportEnum.RIZ_KarticaDobav_S: 
                vvReport = new RptR_KarticaDobav(new Vektor.Reports.RIZ.CR_KarticaPartnera(), reportName, theRiskFilter, false);
                if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_KarticaDobav) theRiskFilter.AnalitSintet = "A";
                else                                                                 theRiskFilter.AnalitSintet = "S";
                break;
     
         case ZXC.VvReportEnum.RIZ_ObrKamataKupac:
         case ZXC.VvReportEnum.RIZ_ObrKamataKupac_S:
                if(!RptR_Kamate_Kartica.IsFilterWellFormed(((RiskReportUC)vvReportUC))) vvReport = null;
                else vvReport = new RptR_KamateKupca(new Vektor.Reports.RIZ.CR_ObracunKamata(), reportName, theRiskFilter, true);
                if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_ObrKamataKupac) theRiskFilter.AnalitSintet = "A";
                else                                                                   theRiskFilter.AnalitSintet = "S";
                break;

         case ZXC.VvReportEnum.RIZ_ObrKamataDobav:
         case ZXC.VvReportEnum.RIZ_ObrKamataDobav_S: 
                if(!RptR_Kamate_Kartica.IsFilterWellFormed(((RiskReportUC)vvReportUC))) vvReport = null;
                else vvReport = new RptR_KamateDobav(new Vektor.Reports.RIZ.CR_ObracunKamata(), reportName, theRiskFilter, false);
                if(vvReportSubModul.reportEnum == ZXC.VvReportEnum.RIZ_ObrKamataDobav) theRiskFilter.AnalitSintet = "A";
                else                                                                   theRiskFilter.AnalitSintet = "S";
                break;
    
         
         case ZXC.VvReportEnum.RIZ_TopDugovanja    : vvReport = new RptR_Dugovanja   (new Vektor.Reports.RIZ.CR_OtsTopLista()      , reportName, theRiskFilter, false); break;
         case ZXC.VvReportEnum.RIZ_TopPotraziv     : vvReport = new RptR_Potrazivanja(new Vektor.Reports.RIZ.CR_OtsTopLista()      , reportName, theRiskFilter, true ); break;
         case ZXC.VvReportEnum.RIZ_DobavDospjeca   : vvReport = new RptR_Dugovanja   (new Vektor.Reports.RIZ.CR_DospjPoRazd()      , reportName, theRiskFilter, false); break;
         case ZXC.VvReportEnum.RIZ_KupciDospjeca   : vvReport = new RptR_Potrazivanja(new Vektor.Reports.RIZ.CR_DospjPoRazd()      , reportName, theRiskFilter, true ); break;
         case ZXC.VvReportEnum.RIZ_Kompenzacija    : if(!RptR_Kompenzacija.IsFilterWellFormed(((RiskReportUC)vvReportUC))) vvReport = null;
                                                     else                                                                  vvReport = new RptR_Kompenzacija(new Vektor.Reports.RIZ.CR_Kompenzacija(), reportName, theRiskFilter, true); break;

         #endregion IOS


         case ZXC.VvReportEnum.RIZ_Knjizenja: vvReport = new RptR_KnjigaPopisa(new Vektor.Reports.RIZ.CR_KnjizenjaFromRisk(), reportName, theRiskFilter); break;
         //vvReport = new RptR_StandardRiskReport(new Vektor.Reports.RIZ.CR_KontrolaPrometaArt(), reportName, theRiskFilter, ZXC.RIZ_FilterStyle.Faktur,
         //                            false, // ArtiklWithArtstat         
         //                            false, // ArtStat        
         //                            false, // Faktur         
         //                            true , // Rtrans         
         //                            false, // Kupdob         
         //                            false, // Prjkt          
         //                            false, // Rtrans4ruc     
         //                            false);// Artikl         

         case ZXC.VvReportEnum.GROUP1:
         case ZXC.VvReportEnum.GROUP2:
         case ZXC.VvReportEnum.GROUP3:
         case ZXC.VvReportEnum.GROUP4:
         case ZXC.VvReportEnum.GROUP5:
         case ZXC.VvReportEnum.GROUP6:
         case ZXC.VvReportEnum.GROUP7:
         case ZXC.VvReportEnum.GROUP8:
         case ZXC.VvReportEnum.GROUP9:    ZXC.aim_emsg("Molim, odaberite izvještaj"); break;

         #endregion RIZ

         #region XIZ

         case ZXC.VvReportEnum.XIZ_RekapPutNalLoko: vvReport = new RptX_StandardMixerReport(new Vektor.Reports.XIZ.CR_GrupiraniPutNal(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.PutNal, 
                                                true,  //Mixer_List     
                                                true,  //Xtrans_List    
                                                true,  //Xtrano_List    
                                      /* !!! */ true,  // needsXtrSO_Join_Mixer !!!
                                                true , //Kupdob_List_   
                                                false, //Artikl_List_   
                                                false);//Prjkt
            break;
         case ZXC.VvReportEnum.XIZ_RekapZahtjev: vvReport = new RptX_StandardMixerReport(new Vektor.Reports.XIZ.CR_GrupiraniZahtjevi(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev, 
                                               true,  // Mixer_List  
                                               false, //Xtrans_List 
                                               false, //Xtrano_List 
                                               false, // needsXtrSO_Join_Mixer
                                               false, //Kupdob_List   
                                               false, //Artikl_List   
                                               false);//Prjkt
            break;
         
         case ZXC.VvReportEnum.XIZ_RekapSMD   :
            if(((MixerReportUC)vvReportUC).TheRptFilter.GrupiranjeSMD == "PersonCD")            
               vvReport = new RptX_StandardMixerReport(new Vektor.Reports.XIZ.CR_SMDgrupiranPoPersonu(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                               true,  // Mixer_List  
                                               true,  //Xtrans_List 
                                               false, //Xtrano_List 
                                               true,  // needsXtrSO_Join_Mixer
                                               false, //Kupdob_List   
                                               false, //Artikl_List   
                                               false);//Prjkt
            else 
               vvReport = new RptX_StandardMixerReport(new Vektor.Reports.XIZ.CR_GrupiraniSMD(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                               true,  // Mixer_List  
                                               true,  //Xtrans_List 
                                               false, //Xtrano_List 
                                               true,  // needsXtrSO_Join_Mixer
                                               false, //Kupdob_List   
                                               false, //Artikl_List   
                                               false);//Prjkt
            break;
     
       case ZXC.VvReportEnum.XIZ_RekapEVD: vvReport = new RptX_StandardMixerReport(new Vektor.Reports.XIZ.CR_GrupiraneEvidencije(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                            true,  // Mixer_List  
                                            false, //Xtrans_List 
                                            false, //Xtrano_List 
                                            false, // needsXtrSO_Join_Mixer
                                            false, //Kupdob_List   
                                            false, //Artikl_List   
                                            false);//Prjkt
            break;

         
         case ZXC.VvReportEnum.XIZ_UrdzZap: vvReport = new RptX_StandardMixerReport(new Vektor.Reports.XIZ.CR_UrudzbeniZap(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                            true,  // Mixer_List  
                                            false, //Xtrans_List 
                                            false, //Xtrano_List 
                                            false, // needsXtrSO_Join_Mixer
                                            false, //Kupdob_List   
                                            false, //Artikl_List   
                                            false);//Prjkt
            ((MixerReportUC)vvReportUC).TheRptFilter.TT = Mixer.TT_URZ;
            break;

         case ZXC.VvReportEnum.XIZ_RegUgv: vvReport = new RptX_StandardMixerReport(new Vektor.Reports.XIZ.CR_RegUgovora(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                            true,  // Mixer_List  
                                            false, //Xtrans_List 
                                            false, //Xtrano_List 
                                            false, // needsXtrSO_Join_Mixer
                                            false, //Kupdob_List   
                                            false, //Artikl_List   
                                            false);//Prjkt
            ((MixerReportUC)vvReportUC).TheRptFilter.TT = Mixer.TT_RUG;

            break;
        
         case ZXC.VvReportEnum.XIZ_RekapEVN: vvReport = new RptX_StandardMixerReport(new Vektor.Reports.XIZ.CR_GrupEvdNacrta(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                            true,  // Mixer_List  
                                            false, //Xtrans_List 
                                            false, //Xtrano_List 
                                            false, // needsXtrSO_Join_Mixer
                                            false, //Kupdob_List   
                                            false, //Artikl_List   
                                            false);//Prjkt
            break;

         case ZXC.VvReportEnum.XIZ_RVR:
            bool isBef01042015 = ((MixerReportUC)vvReportUC).TheRptFilter.DatumOd < ZXC.Date01042015;
            CrystalDecisions.CrystalReports.Engine.ReportDocument repDoc1 = new Vektor.Reports.XIZ.CR_EvidRadnogVremena();
            CrystalDecisions.CrystalReports.Engine.ReportDocument repDoc2 = new Vektor.Reports.XIZ.CR_EvRadVrem_od28032015();

            vvReport = new RptX_EvidencijaRadnogVremena(isBef01042015 ? repDoc1 : repDoc2, reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
        //                              vvReport = new RptX_EvidencijaRadnogVremena(new Vektor.Reports.XIZ.CR_EvidRadnogVremena(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                            false, // Mixer_List  
                                            true , //Xtrans_List 
                                            false, //Xtrano_List 
                                            false, // needsXtrSO_Join_Mixer
                                            false, //Kupdob_List   
                                            false, //Artikl_List   
                                            false);//Prjkt

            break;
         case ZXC.VvReportEnum.XIZ_MVR:
            if(!RptX_EvidencijaRadnogVremena_Mjesecna.IsFilterWellFormed(((MixerReportUC)vvReportUC))) vvReport = null;
            else             
            vvReport = new RptX_EvidencijaRadnogVremena_Mjesecna(new Vektor.Reports.XIZ.CR_MVR_EvidRadVr(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                            false, // Mixer_List  
                                            true , //Xtrans_List 
                                            false, //Xtrano_List 
                                            false, // needsXtrSO_Join_Mixer
                                            false, //Kupdob_List   
                                            false, //Artikl_List   
                                            true); //Prjkt
            break;

         case ZXC.VvReportEnum.XIZ_IRV:
            if(((MixerReportUC)vvReportUC).TheRptFilter.ProjektCd.NotEmpty())
               vvReport = new RptX_StandardMixerReport(new Vektor.Reports.XIZ.CR_IRVpoProjektCd(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                               false, // Mixer_List  
                                               true, //Xtrans_List 
                                               false, //Xtrano_List 
                                               false, // needsXtrSO_Join_Mixer
                                               false, //Kupdob_List   
                                               false, //Artikl_List   
                                               false);//Prjkt

            else
               vvReport = new RptX_StandardMixerReport(new Vektor.Reports.XIZ.CR_InterniRvrI(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                            true , // Mixer_List  
                                            true , //Xtrans_List 
                                            false, //Xtrano_List 
                                            false, // needsXtrSO_Join_Mixer
                                            false, //Kupdob_List   
                                            false, //Artikl_List   
                                            false);//Prjkt
            break;

         case ZXC.VvReportEnum.XIZ_PriBor: vvReport = new RptX_PriBor(new Vektor.Reports.XIZ.CR_PBSMUP(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                            true , // Mixer_List  
                                            false, //Xtrans_List 
                                            false, //Xtrano_List 
                                            false, // needsXtrSO_Join_Mixer
                                            false, //Kupdob_List   
                                            false, //Artikl_List   
                                            true); //Prjkt
            break;

         case ZXC.VvReportEnum.XIZ_GST:
            vvReport = new RptX_StandardMixerReport(new Vektor.Reports.XIZ.CR_KnjigaGostiju(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                               true, // Mixer_List  
                                               false, //Xtrans_List 
                                               false, //Xtrano_List 
                                               false, // needsXtrSO_Join_Mixer
                                               false, //Kupdob_List   
                                               false, //Artikl_List   
                                               true); //Prjkt

            ((MixerReportUC)vvReportUC).TheRptFilter.IsKnjigaDomaca = true;
            break;

         case ZXC.VvReportEnum.XIZ_GST_STR:
            vvReport = new RptX_StandardMixerReport(new Vektor.Reports.XIZ.CR_KnjigaGostiju(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                               true, // Mixer_List  
                                               false, //Xtrans_List 
                                               false, //Xtrano_List 
                                               false, // needsXtrSO_Join_Mixer
                                               false, //Kupdob_List   
                                               false, //Artikl_List   
                                               true); //Prjkt
            ((MixerReportUC)vvReportUC).TheRptFilter.IsKnjigaDomaca = false;
            break;

         case ZXC.VvReportEnum.XIZ_TZNoc: vvReport = new RptX_NocenjaInfo(new Vektor.Reports.XIZ.CR_nocenjaInfoTZ(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                            true, // Mixer_List  
                                            false, //Xtrans_List 
                                            false, //Xtrano_List 
                                            false, // needsXtrSO_Join_Mixer
                                            false, //Kupdob_List   
                                            false, //Artikl_List   
                                            true); //Prjkt
            break;
         case ZXC.VvReportEnum.XIZ_TZ_DolN: vvReport = new RptX_NocenjaInfo(new Vektor.Reports.XIZ.CR_StranaNocenajTZ(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                            true, // Mixer_List  
                                            false, //Xtrans_List 
                                            false, //Xtrano_List 
                                            false, // needsXtrSO_Join_Mixer
                                            false, //Kupdob_List   
                                            false, //Artikl_List   
                                            true); //Prjkt
            break;

         case ZXC.VvReportEnum.XIZ_ZPG: vvReport = new RptX_StandardMixerReport(new Vektor.Reports.XIZ.CR_Rekap_ZPG(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                true, // Mixer_List  
                                false, //Xtrans_List 
                                false, //Xtrano_List 
                                false, // needsXtrSO_Join_Mixer
                                false, //Kupdob_List   
                                false, //Artikl_List   
                                false); //Prjkt
            ((MixerReportUC)vvReportUC).TheRptFilter.TT = Mixer.TT_ZPG;

            break;

         case ZXC.VvReportEnum.XIZ_ZLJ: vvReport = new RptX_StandardMixerReport(new Vektor.Reports.XIZ.CR_Rekap_ZLJ(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                true, // Mixer_List  
                                false, //Xtrans_List 
                                false, //Xtrano_List 
                                false, // needsXtrSO_Join_Mixer
                                false, //Kupdob_List   
                                false, //Artikl_List   
                                false); //Prjkt
            ((MixerReportUC)vvReportUC).TheRptFilter.TT = Mixer.TT_ZLJ;

            break;

         case ZXC.VvReportEnum.XIZ_ZUG: vvReport = new RptX_StandardMixerReport(new Vektor.Reports.XIZ.CR_Rekap_ZUG(), reportName, ((MixerReportUC)vvReportUC).TheRptFilter, ZXC.MIX_FilterStyle.Zahtjev,
                                true, // Mixer_List  
                                false, //Xtrans_List 
                                false, //Xtrano_List 
                                false, // needsXtrSO_Join_Mixer
                                true , //Kupdob_List   
                                false, //Artikl_List   
                                false); //Prjkt
            ((MixerReportUC)vvReportUC).TheRptFilter.TT = Mixer.TT_RUG;

            break;


         #endregion XIZ

         default: ZXC.aim_emsg("Report {0} nedovrsen! A ak je Pogled_ZNP onda ce se premjestit kad za to dojde vrijeme", vvReportSubModul.reportEnum);
            break;
      }

      return vvReport;
   }

   #endregion CreateTheVvReport_SwitchReportEnum

   #region ReportNewClient

   private void ReportNewClient()
   {
      bool dummy = false;

      if(dummy) return; // ToDo: uvjet jel Vergin Client 

      MailMessage message = new MailMessage(ZXC.vvDB_Server + "." + ZXC.VvDeploymentSite + "@t-com.hr", "viper@zg.htnet.hr");

      message.Subject = "NewVvClient";

      SmtpClient client = new SmtpClient("mail.htnet.hr");

      try
      {
         client.Send(message);
      }
      catch
      {
         // be silent 
      }
   }

   #endregion ReportNewClient

   #region Check For Minus

#if CheckForMinus_OLD

   private bool CheckForMinus_OLD(ZXC.WriteMode documentWriteMode)
   {
      if(ShouldCheckMinusForTheVvDataRecord == false) return false;

      // Here we go! Znaci ovo je FakturDUC i mixer_rec.TtInfo.IsFinKol_TT == true 

      FakturDUC fakturDUC  = TheVvUC         as FakturDUC;
      Faktur    faktur_rec = TheVvDataRecord as Faktur;

      bool minusOccured = false, skladCD_Changed = false, dateChangedInPlus = false;

      decimal deltaKol, kolStBeforeThisChange, /*previousDataLayerSameArtiklDeltaKol,*/ previousNonDataLayerSameArtiklDeltaKol;

      string artiklCD, skladCD, artiklName;

      ArtStat artstat_rec;
      Artikl  artikl_rec;

      List<ZXC.VvUtilDataPackage> artiklInMinusInfoList    = new List<ZXC.VvUtilDataPackage>();
      List<ZXC.VvUtilDataPackage> previousRtransesInfoList = new List<ZXC.VvUtilDataPackage>();

      Rtrans getCacheBeforeThisRtrans_rec = null;

      ZXC.WriteMode thisRtransWriteMode = documentWriteMode; // Za ADDREC Faktur document i DELREC Faktur document, za RWTREC Faktur moras dole u petlji analiticki... 

      // nepoznat datum i razlog ove promjene (Transes --> TrnNonDel) komentar pisan 03.10.2016!     
    //foreach(Rtrans rtrans_rec in faktur_rec.Transes      )                                         
      // 26.10.2015: !!! TRI dozvojavao minus!!!! pa TrnNonDel --> TrnNonDel_ALL                     
    //foreach(Rtrans rtrans_rec in faktur_rec.TrnNonDel    )                                         
      // 03.10.2016: !!! ipak Transes jer inace, brisanje redka npr URA-e dozvoljava odlazak u minus 
    //foreach(Rtrans rtrans_rec in faktur_rec.TrnNonDel_ALL)                                         
      foreach(Rtrans rtrans_rec in faktur_rec.Transes      ) 
      {
         getCacheBeforeThisRtrans_rec = rtrans_rec.MakeDeepCopy();

         artiklCD   = rtrans_rec.T_artiklCD  ;
         artiklName = rtrans_rec.T_artiklName;
         skladCD    = rtrans_rec.T_skladCD   ;

         artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artiklCD);

         if(artiklCD.IsEmpty() ||  artikl_rec == null || artikl_rec.IsAllowMinus || artikl_rec.IsMinusOK || artikl_rec.IsGlass || artikl_rec.IsFKZ) continue;

         if(documentWriteMode == ZXC.WriteMode.Edit) // overriding some ADDREC, DELREC logic 
         {
            thisRtransWriteMode = rtrans_rec.SaveTransesWriteMode;

            if(thisRtransWriteMode == ZXC.WriteMode.Edit)
            {
               getCacheBeforeThisRtrans_rec.T_skladCD   = rtrans_rec.T_BCKPskladCD  ;
               getCacheBeforeThisRtrans_rec.T_ttNum     = rtrans_rec.T_BCKPttNum    ;
               getCacheBeforeThisRtrans_rec.T_serial    = rtrans_rec.T_BCKPserial   ;
               getCacheBeforeThisRtrans_rec.T_skladDate = rtrans_rec.T_BCKPskladDate;
            }

            if(thisRtransWriteMode == ZXC.WriteMode.Edit)
            {
               artiklCD   = rtrans_rec.T_BCKPartiklCD;
               artiklName = ""                       ;
               skladCD    = rtrans_rec.T_BCKPskladCD ;
            }
            else
            {
               artiklCD   = rtrans_rec.T_artiklCD  ;
               artiklName = rtrans_rec.T_artiklName;
               skladCD    = rtrans_rec.T_skladCD   ;
            }

            if(thisRtransWriteMode == ZXC.WriteMode.Edit && rtrans_rec.T_BCKPskladCD != rtrans_rec.T_skladCD) // U 'Ispravi' je promijenjeno skladiste 
            {
               skladCD_Changed = true;

               // 13.02.2023: 
             //thisRtransWriteMode = ZXC.WriteMode.Delete;
               thisRtransWriteMode = ZXC.WriteMode.Add   ;

               skladCD             = rtrans_rec.T_skladCD;
            }

            if(thisRtransWriteMode == ZXC.WriteMode.Edit && rtrans_rec.T_BCKPskladDate < rtrans_rec.T_skladDate) // U 'Ispravi' je 'povećan' datum 
            {
               dateChangedInPlus = true;
            }

         } // if(documentWriteMode == ZXC.WriteMode.Edit) // overriding some ADDREC, DELREC logic 

         if(thisRtransWriteMode == ZXC.WriteMode.None) continue; // Pri RWTREC stavke koje nisu mijenjane preskoci 

         // 13.02.2023: 
       //artstat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, artiklCD, skladCD           );
         // 14.02.2023:                                                                       
       //artstat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, rtrans_rec                  );
         artstat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, getCacheBeforeThisRtrans_rec);

         if(artstat_rec != null) kolStBeforeThisChange = artstat_rec.StanjeKol/*Free*/; // ovo 'Free' smo ugasili u 2023 da lakse shvatimo sto radimo, a nitko ne koristi rezervacije 
         else                    kolStBeforeThisChange =                             0;

         previousNonDataLayerSameArtiklDeltaKol = previousRtransesInfoList.Where(prev => prev.TheStr1 == artiklCD && prev.TheBool == false).Sum(prev => prev.TheDecimal); // false - nije u data layeru 
       //previousDataLayerSameArtiklDeltaKol    = previousRtransesInfoList.Where(prev => prev.TheStr1 == artiklCD && prev.TheBool == true ).Sum(prev => prev.TheDecimal); // true  -   je u data layeru 

         kolStBeforeThisChange += previousNonDataLayerSameArtiklDeltaKol; // za visestruke pojave istog artikla, ma nebu mene nitko je'al! 

         if(dateChangedInPlus) // datum je pomaknut na novije pa treba iz prošlosti poništiti samog sebe da ge ne racuna uduplo 
         {
            kolStBeforeThisChange -= rtrans_rec.GetDeltaKol2023_BCKP(thisRtransWriteMode);
         }

         // 13.02.2023: 
     //if(rtrans_rec.AfterThisRtransMinusWillBeWorse        (thisRtransWriteMode, kolStBeforeThisChange, out deltaKol) == true)
       if(rtrans_rec.BudeLiOvajRedakRobneKarticeIskazaoMinus(thisRtransWriteMode, kolStBeforeThisChange, out deltaKol) == true)
       {
          minusOccured = true;
          
          artiklInMinusInfoList.Add(new ZXC.VvUtilDataPackage(artiklCD, skladCD, kolStBeforeThisChange + deltaKol) { TheStr3 = artiklName } );
       }

         previousRtransesInfoList.Add(new ZXC.VvUtilDataPackage(artiklCD, "", deltaKol, rtrans_rec.T_recID.NotZero())); // bool odgovara na pitanje "ima li ga u data layeru" 

   #region On skladCD change - new skladCD pass

         if(skladCD_Changed)
         {
            // 13.02.2023: 
          //thisRtransWriteMode = ZXC.WriteMode.Add   ;
            thisRtransWriteMode = ZXC.WriteMode.Delete;

            artiklCD = getCacheBeforeThisRtrans_rec.T_artiklCD;
            skladCD  = getCacheBeforeThisRtrans_rec.T_skladCD ;

            // 13.02.2023: 
          //artstat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, artiklCD, skladCD);
            artstat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, getCacheBeforeThisRtrans_rec);

            if(artstat_rec != null) kolStBeforeThisChange = artstat_rec.StanjeKolFree;
            else                    kolStBeforeThisChange =                         0;

            // 13.02.2023: 
          //if(rtrans_rec                  .AfterThisRtransMinusWillBeWorse        (thisRtransWriteMode, kolStBeforeThisChange, out deltaKol) == true)
            if(getCacheBeforeThisRtrans_rec.BudeLiOvajRedakRobneKarticeIskazaoMinus(thisRtransWriteMode, kolStBeforeThisChange, out deltaKol) == true)
            {
               minusOccured = true;
               
               artiklInMinusInfoList.Add(new ZXC.VvUtilDataPackage(artiklCD, skladCD, kolStBeforeThisChange + deltaKol));
            }
         }

   #endregion On skladCD change - new skladCD pass

#if CheckForMinus23

         // NOVO 2023: !!! 

         if(thisRtransWriteMode == ZXC.WriteMode.Edit  ) kolStBeforeThisChange += rtrans_rec.GetDeltaKol2023_BCKP(ZXC.WriteMode.Edit  ); // +thisRtransBCKPkol 
         if(thisRtransWriteMode == ZXC.WriteMode.Delete) kolStBeforeThisChange -= rtrans_rec.GetDeltaKol         (ZXC.WriteMode.Delete); // -thisRtransKol     

         if(ImaLiIgdjeNakonOvog_ADD_RWT_DEL_PojavaMinusa(getCacheBeforeThisRtrans_rec, thisRtransWriteMode, kolStBeforeThisChange))
         {
            minusOccured = true;
         
            artiklInMinusInfoList.Add(new ZXC.VvUtilDataPackage(artiklCD, skladCD, kolStBeforeThisChange + deltaKol));
         }
#endif

      } // foreach(Rtrans rtrans_rec in faktur_rec.Transes) 

      if(minusOccured == true)
      {
         // Na IRM-ovima niti ne javljaj minuse
         if(ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.DENY_VEL_ALLOW_MAL && faktur_rec.TT == Faktur.TT_IRM) return false;
         if(ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.ALOW_ALL_NO_MSG                                     ) return false;

         IssueActionRefusedBecauseOfMinusMessage(documentWriteMode, ZXC.CURR_prjkt_rec.MinusPolicy, faktur_rec.TtInfo.IsMalopTT, artiklInMinusInfoList);

         if(ZXC.CURR_prjkt_rec.IsUnacceptableMinus(faktur_rec.TtInfo.IsMalopTT) == true) return true; // !!! 
      }

      return false;
   }


   private bool ImaLiIgdjeNakonOvog_ADD_RWT_DEL_PojavaMinusa(Rtrans ADD_RWT_DEL_rtrans_rec, ZXC.WriteMode thisRtransWriteMode, decimal kolStBeforeThisChange)
   {
   #region Init

      //bool minusOccured = false;
   
      List<Rtrans> listaIzmjenjenihRedaka = new List<Rtrans>();
   
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(4); 

      DataRowCollection  rtrSch = ZXC.RtransSchemaRows; 
      RtransDao.RtransCI rtrCI  = ZXC.RtrCI           ;

      decimal KolSt_odsada;
      decimal delta_KolSt ;

      bool isADDed         = thisRtransWriteMode == ZXC.WriteMode.Add                                                                                 ;

      bool isEDITed        = thisRtransWriteMode == ZXC.WriteMode.Edit                                                                                ;
      bool hasEDITed_kol   = thisRtransWriteMode == ZXC.WriteMode.Edit && ADD_RWT_DEL_rtrans_rec.T_kol       != ADD_RWT_DEL_rtrans_rec.T_BCKPkol      ;
      bool hasEDITed_date  = thisRtransWriteMode == ZXC.WriteMode.Edit && ADD_RWT_DEL_rtrans_rec.T_skladDate != ADD_RWT_DEL_rtrans_rec.T_BCKPskladDate;
      bool hasEDITed_sklad = thisRtransWriteMode == ZXC.WriteMode.Edit && ADD_RWT_DEL_rtrans_rec.T_skladCD   != ADD_RWT_DEL_rtrans_rec.T_BCKPskladCD  ;

      bool isDELETEed      = thisRtransWriteMode == ZXC.WriteMode.Delete                                                                              ;

   #endregion init

   #region Za ADD_RWT_DEL_rtrans_rec: GetDeltaKol & provjera KolSt_odsada

      delta_KolSt = ADD_RWT_DEL_rtrans_rec.GetDeltaKol(thisRtransWriteMode);

      KolSt_odsada = kolStBeforeThisChange + delta_KolSt;

      if(KolSt_odsada.IsNegative()) return true;

   #endregion Za ADD_RWT_DEL_rtrans_rec: GetDeltaKol & provjera KolSt_odsada

   #region Get robna kartica segment 

      filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.t_artiklCD  ], false, "theArtiklCD", ADD_RWT_DEL_rtrans_rec.T_artiklCD, "", "",  " = " , "", "R"));
      filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.t_skladCD   ], false, "theSklCD"   , ADD_RWT_DEL_rtrans_rec.T_skladCD , "", "",  " = " , "", "R"));

      //filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.tt]     , ZXC.FM_OR_Enum.OPEN_OR , false, "TT1"    , Faktur.TT_UGN, "", "", "  = ", ""));
      //filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.tt]     , ZXC.FM_OR_Enum.NONE    , false, "TT2"    , Faktur.TT_AUN, "", "", "  = ", ""));
      //filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.tt]     , ZXC.FM_OR_Enum.CLOSE_OR, false, "TT2"    , Faktur.TT_AUN, "", "", "  = ", ""));

      RtransDao.GetRtransWithArtstatList(TheDbConnection, listaIzmjenjenihRedaka, "", filterMembers, Rtrans.artiklOrderBy_ASC.Replace("t_", "R.t_"));

      listaIzmjenjenihRedaka.RemoveAll
      (rtr =>
         (                                                                                                                                                       rtr.T_skladDate <= ADD_RWT_DEL_rtrans_rec.DatumPocetkaPromjeneRobnojKartici) ||
         (                                                                                                    rtr.T_ttSort <= ADD_RWT_DEL_rtrans_rec.T_ttSort && rtr.T_skladDate == ADD_RWT_DEL_rtrans_rec.DatumPocetkaPromjeneRobnojKartici) ||
         (                                                   rtr.T_ttNum <= ADD_RWT_DEL_rtrans_rec.T_ttNum && rtr.T_ttSort == ADD_RWT_DEL_rtrans_rec.T_ttSort && rtr.T_skladDate == ADD_RWT_DEL_rtrans_rec.DatumPocetkaPromjeneRobnojKartici) ||
         (rtr.T_serial <= ADD_RWT_DEL_rtrans_rec.T_serial && rtr.T_ttNum == ADD_RWT_DEL_rtrans_rec.T_ttNum && rtr.T_ttSort == ADD_RWT_DEL_rtrans_rec.T_ttSort && rtr.T_skladDate == ADD_RWT_DEL_rtrans_rec.DatumPocetkaPromjeneRobnojKartici)
      );

   #endregion Get robna kartica segment 

      // retciNakonMene je segment robne kartice nakon ADD_RWT_DEL_rtrans_rec pa do kraja 
      foreach(Rtrans redakRobneKartice in listaIzmjenjenihRedaka)
      {
         KolSt_odsada = redakRobneKartice.A_StanjeKol + delta_KolSt;

         if(KolSt_odsada.IsNegative()) return true;
      }

   #region Final

      return /*minusOccured*/ false;

   #endregion final

   }

   private void IssueActionRefusedBecauseOfMinusMessage(ZXC.WriteMode writeMode, ZXC.MinusPolicy minusPolicy, bool isMalopTT, List<ZXC.VvUtilDataPackage> artiklInMinusInfoList)
   {
      string errmsg         = "";
      string minusPolicyStr = "";

      string glagol = writeMode == ZXC.WriteMode.Delete ? "Brisanje" : "Usnimavanje";

      switch(minusPolicy)
      {
         case ZXC.MinusPolicy.DENY_ALL: 
            errmsg         = "AKCIJA ODBIJENA";
            minusPolicyStr = "Ne dozvoli minus";
            break;

         case ZXC.MinusPolicy.ALLOW_ALL: 
            errmsg         = "UPOZORENJE";
            minusPolicyStr = "Dozvoli minus";
            break;

         case ZXC.MinusPolicy.DENY_VEL_ALLOW_MAL:
            if(isMalopTT)
            {
               errmsg         = "UPOZORENJE";
               minusPolicyStr = "Dozvoli minus";
            }
            else // VELEP 
            {
               errmsg         = "AKCIJA ODBIJENA";
               minusPolicyStr = "Ne dozvoli minus";
            }
            break;

      }

      string artikls = "";

      if(artiklInMinusInfoList != null) foreach(ZXC.VvUtilDataPackage artiklInfo in artiklInMinusInfoList)
      {
         artikls += "Novonastalo stanje artikla\n" + artiklInfo.TheStr1 + " " + artiklInfo.TheStr3 + "\nna skladištu " + artiklInfo.TheStr2 + " bilo bi: " + artiklInfo.TheDecimal.ToStringVv() + "\n\n";
      }

      ZXC.aim_emsg(MessageBoxIcon.Error, 
         "{1}!\n\n{0} ovog dokumenta proizvest ce ili povećati negativno količinsko stanje nekog artikla sa ovog dokumenta.\n\n'Politika Minusa' je: {2}.\n\nArtikli:\n\n{3}", glagol, errmsg, minusPolicyStr, artikls);
   }
#endif

   #region CheckForMinus_NEW
   private bool CheckForMinus/*_NEW*/(ZXC.WriteMode documentWriteMode)
   {
      #region Init

      if(ShouldCheckMinusForTheVvDataRecord == false) return false;

      Faktur faktur_rec = TheVvDataRecord as Faktur;

      bool isMinusBorn = false;

      Rtrans firstMinusRtrans_rec;
      Artikl artikl_rec;

      List<ZXC.VvUtilDataPackage> artiklInMinusInfoList = new List<ZXC.VvUtilDataPackage>();
      List<Rtrans>                firstMinusRtransList  = new List<Rtrans>               ();

      #endregion Init

      #region Sint Same Artikls With Same WriteMode

      List<Rtrans> theRtransList = faktur_rec.Transes.GroupBy(rtr => rtr.T_artiklCD + rtr.SaveTransesWriteMode.ToString())
         .Select(rtrGR => new Rtrans()
         { 
            backupData = rtrGR.Last().backupData,

            T_artiklCD           = rtrGR.First().T_artiklCD          ,
            T_artiklName         = rtrGR.First().T_artiklName        ,
            SaveTransesWriteMode = rtrGR.First().SaveTransesWriteMode,
            T_skladDate          = rtrGR.First().T_skladDate         ,
            T_TT                 = rtrGR.First().T_TT                ,
            T_ttNum              = rtrGR.First().T_ttNum             ,
            T_ttSort             = rtrGR.First().T_ttSort            ,
            T_skladCD            = rtrGR.First().T_skladCD           ,
          //T_BCKPskladDate      = rtrGR.First().T_BCKPskladDate     ,
          //T_BCKPskladCD        = rtrGR.First().T_BCKPskladCD       ,
            
            T_serial             = rtrGR.Last().T_serial             ,
            A_UkPstKol           = rtrGR.Last().A_UkPstKol           , // ovi daju A_StanjeKol 
            A_UkUlazKol          = rtrGR.Last().A_UkUlazKol          , // ovi daju A_StanjeKol 
            A_UkIzlazKol         = rtrGR.Last().A_UkIzlazKol         , // ovi daju A_StanjeKol 

            T_BCKPkol            = rtrGR.Sum  (r => r.T_BCKPkol)     ,
            T_kol                = rtrGR.Sum  (r => r.T_kol)         ,

         }).ToList();

      if(documentWriteMode == ZXC.WriteMode.Delete) theRtransList.ForEach(rtr => rtr.SaveTransesWriteMode = ZXC.WriteMode.Delete);

      #endregion Sint Same Artikls With Same WriteMode

      // main loop 

      foreach(Rtrans rtrans in theRtransList)
      {
         #region Skipaj nepotrebne

         if(rtrans.SaveTransesWriteMode == ZXC.WriteMode.None) continue; 

         artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == rtrans.T_artiklCD);

         if(rtrans.T_artiklCD.IsEmpty() || artikl_rec == null || artikl_rec.IsAllowMinus || artikl_rec.IsMinusOK || artikl_rec.IsGlass || artikl_rec.IsFKZ) continue;

         #endregion Skipaj nepotrebne

         firstMinusRtrans_rec = null;

         switch(rtrans.SaveTransesWriteMode)
         {
            case ZXC.WriteMode.Add   : firstMinusRtrans_rec = CheckForMinusJOB_ADD(rtrans); break;
            case ZXC.WriteMode.Delete: firstMinusRtrans_rec = CheckForMinusJOB_DEL(rtrans); break;
            case ZXC.WriteMode.Edit  : firstMinusRtrans_rec = CheckForMinusJOB_RWT(rtrans); break;
         }

         if(firstMinusRtrans_rec != null)
         {
            isMinusBorn = true;

            artiklInMinusInfoList.Add(new ZXC.VvUtilDataPackage
               (firstMinusRtrans_rec.T_artiklCD, 
                firstMinusRtrans_rec.T_skladCD, 
                firstMinusRtrans_rec.TmpDecimal
               ) 
            { TheStr3 = firstMinusRtrans_rec.T_artiklName });

            firstMinusRtransList.Add(firstMinusRtrans_rec);
         }

      } // foreach(Rtrans rtrans in theRtransList) 

      // end of main loop

      if(isMinusBorn)
      {
         // Na IRM-ovima niti ne javljaj minuse
         if(ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.DENY_VEL_ALLOW_MAL && faktur_rec.TT == Faktur.TT_IRM) return false;
         if(ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.ALOW_ALL_NO_MSG                                     ) return false;

         IssueActionRefusedBecauseOfMinusMessage_NEW(documentWriteMode, ZXC.CURR_prjkt_rec.MinusPolicy, faktur_rec.TtInfo.IsMalopTT, firstMinusRtransList);

         if(ZXC.CURR_prjkt_rec.IsUnacceptableMinus(faktur_rec.TtInfo.IsMalopTT) == true) return true; // !!! 
      }

      return false;
   }

   private Rtrans CheckForMinusJOB_RWT(Rtrans EDITedRtrans)
   {
      bool hasEDITed_kol    = EDITedRtrans.T_kol       != EDITedRtrans.T_BCKPkol      ;
      bool hasEDITed_date   = EDITedRtrans.T_skladDate != EDITedRtrans.T_BCKPskladDate;
      bool hasEDITed_sklad  = EDITedRtrans.T_skladCD   != EDITedRtrans.T_BCKPskladCD  ;
      bool hasEDITed_artikl = EDITedRtrans.T_artiklCD  != EDITedRtrans.T_BCKPartiklCD ;

      if(hasEDITed_sklad )                return CheckForMinus_RWT_EDITed_Sklad          (EDITedRtrans);
    //if(hasEDITed_artikl)                return CheckForMinus_RWT_EDITed_Artikl         (EDITedRtrans); ove se ne moze dogotiti. kad u ispravi zamjenis artikl stavke radjaju se 1 delete i 1 add. edit-a nema
      if(hasEDITed_kol || hasEDITed_date) return CheckForMinus_RWT_EDITed_Kol_or_and_Date(EDITedRtrans);

      return /*false*/ null;
   }

   private Rtrans CheckForMinus_RWT_EDITed_Kol_or_and_Date(Rtrans eDITedRtrans)
   {
      // Ako smo dosli do ovdje, znaci da su orig sklad i artikl ostali nepromijenjeni 
       
      Rtrans newValues_rtrans = eDITedRtrans.MakeDeepCopy();

      Rtrans oldValues_rtrans = eDITedRtrans.MakeDeepCopy();

      oldValues_rtrans.RestoreBackupData();

      List<Rtrans> sviRetciRobneKartice = GetCijelaRobnaKartica(eDITedRtrans);

      int idxOf_OLD_Rtrans = sviRetciRobneKartice.IndexOf(oldValues_rtrans);

      sviRetciRobneKartice[idxOf_OLD_Rtrans].CurrentData = eDITedRtrans.CurrentData;

      sviRetciRobneKartice = sviRetciRobneKartice.OrderBy(rtr => rtr.T_skladDate).ThenBy(rtr => rtr.T_ttSort).ThenBy(rtr => rtr.T_ttNum).ThenBy(rtr => rtr.T_serial).ToList();

      int idxOf_NEW_Rtrans = sviRetciRobneKartice.IndexOf(newValues_rtrans);

      int startIdxOf_Revalor = Math.Min(idxOf_OLD_Rtrans, idxOf_NEW_Rtrans);

      decimal lastCorrectKolSt = 0.00M;

      if(startIdxOf_Revalor.IsPositive())
      {
         lastCorrectKolSt = sviRetciRobneKartice[startIdxOf_Revalor - 1].A_StanjeKol;
      }

      decimal cumulativKolSt = lastCorrectKolSt;

      sviRetciRobneKartice.RemoveRange(0, startIdxOf_Revalor);

      foreach(Rtrans redakRobneKartice in sviRetciRobneKartice)
      {
         cumulativKolSt += (redakRobneKartice.TtInfo.IsFinKol_I ? -1.00M * redakRobneKartice.R_kol : redakRobneKartice.R_kol);

         if(cumulativKolSt.IsNegative())
         {
            redakRobneKartice.TmpDecimal = cumulativKolSt;

            return /*true*/redakRobneKartice;
         }
      }

      return /*false*/null;
   }

   private Rtrans CheckForMinus_RWT_EDITed_Sklad(Rtrans eDITedRtranswChangedSklad)
   {
      Rtrans firstMinusRtrans_rec;

      Rtrans newSklad_ADDrtrans = eDITedRtranswChangedSklad.MakeDeepCopy();

      Rtrans oldSklad_DELrtrans = eDITedRtranswChangedSklad.MakeDeepCopy();
      oldSklad_DELrtrans.RestoreBackupData();

      /*bool isMinusBornByADD*/firstMinusRtrans_rec = CheckForMinusJOB_ADD(newSklad_ADDrtrans);

      if(firstMinusRtrans_rec != null) return firstMinusRtrans_rec;

      /*bool isMinusBornByDEL*/firstMinusRtrans_rec = CheckForMinusJOB_DEL(oldSklad_DELrtrans);

      if(firstMinusRtrans_rec != null) return firstMinusRtrans_rec;

      //return isMinusBornByADD || isMinusBornByDEL;
      return null;
   }

   private Rtrans CheckForMinusJOB_DEL(Rtrans DELETEdRtrans)
   {
      // PAZI! Ovdje dolazima na vise nacina:                       
      // 1. Brisanje cijelog dokumenta                              
      // 2. Brisanje jedne stavke dokumenta                         
      // 3. Zamjena artikla na retku (tada ide ovaj T_BCKPartiklCD) 
      // 4. Promjena skladista dokumenta                            

      // empirijski primjecen 'fenomen' (aka bug) da kod zamjene artikla redka kod stvara retku NE 1 editWriteMode nego se 
      // akcija razdvaja na 2 rtrans-a; prvi (stari orginal) dobije delete a drugi (novi) dobije add akciju                
      // pri tome se, iz sad vise nepoznatog razloga, u t_artiklCD-u onog prvog deletanog nađe artCD onog novog,           
      // orig artCD se ipak moze nac u t_BCKPartiklCD pa ga OVDJE treba restore-ati                                        
      if(DELETEdRtrans.T_BCKPartiklCD.NotEmpty())
      {
         DELETEdRtrans.T_artiklCD = DELETEdRtrans.T_BCKPartiklCD;
      }

      decimal deltakolOfThisDELETEd = DELETEdRtrans.TtInfo.IsFinKol_I ? -1.00M * DELETEdRtrans.R_kol : DELETEdRtrans.R_kol;

      List<Rtrans> listaIzmjenjenihRedaka = GetRobnaKarticaSegment(DELETEdRtrans);

      decimal newKolSt_ovogRetka;

      foreach(Rtrans redakRobneKartice in listaIzmjenjenihRedaka)
      {
         newKolSt_ovogRetka = redakRobneKartice.A_StanjeKol - deltakolOfThisDELETEd;

         if(newKolSt_ovogRetka.IsNegative())
         {
            redakRobneKartice.TmpDecimal = newKolSt_ovogRetka;

            return redakRobneKartice;
         }
      }

      return null;
   }

   private Rtrans CheckForMinusJOB_ADD(Rtrans ADDedRtrans)
   {
      decimal kolStBeforeThisNewlyADDed = GetPreviousKolSt(ADDedRtrans);

      decimal ADDed_delta_Kol = ADDedRtrans.TtInfo.IsFinKol_I ? -1.00M * ADDedRtrans.R_kol : ADDedRtrans.R_kol;

      decimal newKolSt_ovogRetka = kolStBeforeThisNewlyADDed + ADDed_delta_Kol;

      if(newKolSt_ovogRetka.IsNegative())
      {
         ADDedRtrans.TmpDecimal = newKolSt_ovogRetka;

         return ADDedRtrans;
      }

      List<Rtrans> listaIzmjenjenihRedaka = GetRobnaKarticaSegment(ADDedRtrans);

      foreach(Rtrans redakRobneKartice in listaIzmjenjenihRedaka)
      {
         newKolSt_ovogRetka = redakRobneKartice.A_StanjeKol + ADDed_delta_Kol;

         if(newKolSt_ovogRetka.IsNegative())
         {
            redakRobneKartice.TmpDecimal = newKolSt_ovogRetka;

            return redakRobneKartice;
         }
      }

      return null;
   }

   private decimal GetPreviousKolSt(Rtrans justBeforeThisRtrans)
   {
      decimal previousKolSt;
      ArtStat artstat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, justBeforeThisRtrans); // getCacheBeforeThisRtrans_rec 

      if(artstat_rec != null) previousKolSt = artstat_rec.StanjeKol/*Free*/; // ovo 'Free' smo ugasili u 2023 da lakse shvatimo sto radimo, a nitko ne koristi rezervacije 
      else                    previousKolSt = 0;

      return previousKolSt;
   }

   List<Rtrans> GetRobnaKarticaSegment(Rtrans rtrans)
   {
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      DataRowCollection  rtrSch = ZXC.RtransSchemaRows; 
      RtransDao.RtransCI rtrCI  = ZXC.RtrCI           ;

      filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.t_artiklCD], false, "theArtiklCD", rtrans.T_artiklCD, "", "",  " = " , "", "R"));
      filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.t_skladCD ], false, "theSklCD"   , rtrans.T_skladCD , "", "",  " = " , "", "R"));

      List<Rtrans> listaIzmjenjenihRedaka = new List<Rtrans>();

      RtransDao.GetRtransWithArtstatList(TheDbConnection, listaIzmjenjenihRedaka, "", filterMembers, Rtrans.artiklOrderBy_ASC.Replace("t_", "R.t_"));

      listaIzmjenjenihRedaka.RemoveAll(rtr => rtr.TtInfo.IsFinKol_TT == false);

      listaIzmjenjenihRedaka.RemoveAll
      (rtr =>
         (                                                                                                       rtr.T_skladDate <  rtrans.DatumPocetkaPromjeneRobnojKartici) ||
         (                                                                    rtr.T_ttSort <  rtrans.T_ttSort && rtr.T_skladDate == rtrans.DatumPocetkaPromjeneRobnojKartici) ||
         (                                   rtr.T_ttNum <  rtrans.T_ttNum && rtr.T_ttSort == rtrans.T_ttSort && rtr.T_skladDate == rtrans.DatumPocetkaPromjeneRobnojKartici) ||
         (rtr.T_serial <= rtrans.T_serial && rtr.T_ttNum == rtrans.T_ttNum && rtr.T_ttSort == rtrans.T_ttSort && rtr.T_skladDate == rtrans.DatumPocetkaPromjeneRobnojKartici)
      );

      return listaIzmjenjenihRedaka;
   }

   List<Rtrans> GetCijelaRobnaKartica(Rtrans rtrans)
   {
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      DataRowCollection  rtrSch = ZXC.RtransSchemaRows; 
      RtransDao.RtransCI rtrCI  = ZXC.RtrCI           ;

      filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.t_artiklCD ], false, "theArtiklCD", rtrans.T_artiklCD       , "", "",  "  = " , "", "R"));
      filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.t_skladCD  ], false, "theSklCD"   , rtrans.T_skladCD        , "", "",  "  = " , "", "R"));
      filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.t_skladDate], false, "theDateOd"  , ZXC.projectYearFirstDay , "", "",  " >= " , "", "R"));

      List<Rtrans> sviRetciRobneKartice = new List<Rtrans>();

      RtransDao.GetRtransWithArtstatList(TheDbConnection, sviRetciRobneKartice, "", filterMembers, Rtrans.artiklOrderBy_ASC.Replace("t_", "R.t_"));

      sviRetciRobneKartice.RemoveAll(rtr => rtr.TtInfo.IsFinKol_TT == false);

      return sviRetciRobneKartice;
   }

   #endregion CheckForMinus_NEW

   private void IssueActionRefusedBecauseOfMinusMessage_NEW(ZXC.WriteMode writeMode, ZXC.MinusPolicy minusPolicy, bool isMalopTT, List<Rtrans> firstMinusRtransList)
   {
      string msgKind         = "";
      string minusPolicyStr = "";

      string glagol = writeMode == ZXC.WriteMode.Delete ? "Brisanje" : "Usnimavanje";

      switch(minusPolicy)
      {
         case ZXC.MinusPolicy.DENY_ALL: 
            msgKind        = "AKCIJA ODBIJENA";
            minusPolicyStr = "Ne dozvoli minus";
            break;

         case ZXC.MinusPolicy.ALLOW_ALL: 
            msgKind        = "UPOZORENJE";
            minusPolicyStr = "Dozvoli minus";
            break;

         case ZXC.MinusPolicy.DENY_VEL_ALLOW_MAL:
            if(isMalopTT)
            {
               msgKind        = "UPOZORENJE";
               minusPolicyStr = "Dozvoli minus";
            }
            else // VELEP 
            {
               msgKind        = "AKCIJA ODBIJENA";
               minusPolicyStr = "Ne dozvoli minus";
            }
            break;

      }

      string artikls = "";

      if(firstMinusRtransList != null) foreach(Rtrans firstMinusRtrans in firstMinusRtransList)
      {
         artikls += 
               "Novonastalo stanje artikla\n" + firstMinusRtrans.T_artiklCD + " " + firstMinusRtrans.T_artiklName + "\nna skladištu " + firstMinusRtrans.T_skladCD + " bilo bi: " + 
               firstMinusRtrans.TmpDecimal.ToStringVv() + "\n\n" + 
               "sadržaj retka koji bi nakon ove promjene iskazao minus: \n" + firstMinusRtrans.T_skladDate.ToString(ZXC.VvDateAndTimeFormat) + " " + 
               Faktur.Set_TT_And_TtNum(firstMinusRtrans.T_TT, firstMinusRtrans.T_ttNum) + " " +
               "ulaz/izlaz kol: " + firstMinusRtrans.R_kol.ToStringVv() + " stanje: " + firstMinusRtrans./*A_StanjeKol*/TmpDecimal.ToStringVv() + "\n\n";
      }
      ZXC.aim_emsg(MessageBoxIcon.Error, 
         "{1}!\n\n{0} ovog dokumenta proizvest će ili povećati negativno količinsko stanje nekog artikla sa ovog dokumenta.\n\n'Politika Minusa' je: {2}.\n\nArtikli:\n\n{3}", glagol, msgKind, minusPolicyStr, artikls);

      // =================================================================================================== 

      List<Rtrans> sviRetciRobneKartice     = new List<Rtrans>();
      List<string> sviRetciRobneKartice_str = new List<string>();

      //string redakRobneKartice_str;

      List<VvReportSourceUtil> messageList            ;
      VvReportSourceUtil       redakRobneKartice4DLG  ;
      VvMessageBoxDLG          robnaKarticaInMinus_dlg;
      Kupdob                   kupdob_rec             ;

      foreach(Rtrans firstMinusRtrans in firstMinusRtransList)
      {
         sviRetciRobneKartice = GetCijelaRobnaKartica(firstMinusRtrans);

         messageList = new List<VvReportSourceUtil>();

         foreach(Rtrans redakRobneKartice in sviRetciRobneKartice)
         {
            kupdob_rec = TheVvUC.Get_Kupdob_FromVvUcSifrar(redakRobneKartice.T_kupdobCD);

            redakRobneKartice4DLG           = new VvReportSourceUtil();
            redakRobneKartice4DLG.TheDate   = redakRobneKartice.T_skladDate;
            redakRobneKartice4DLG.String1   = Faktur.Set_TT_And_TtNum(redakRobneKartice.T_TT, redakRobneKartice.T_ttNum);
            redakRobneKartice4DLG.KupdobCD  = redakRobneKartice.T_kupdobCD;
            redakRobneKartice4DLG.TheMoney  = redakRobneKartice.TtInfo.IsFinKol_I ? 0.00M : redakRobneKartice.T_kol;
            redakRobneKartice4DLG.TheMoney2 = redakRobneKartice.TtInfo.IsFinKol_I ? redakRobneKartice.T_kol : 0.00M;
            redakRobneKartice4DLG.TheSaldo  = redakRobneKartice.A_StanjeKol;
            redakRobneKartice4DLG.IsNekakav = redakRobneKartice4DLG.String1 == (((FakturDUC)ZXC.TheVvForm.TheVvUC).Fld_TT + "-" + ((FakturDUC)ZXC.TheVvForm.TheVvUC).Fld_TtNum.ToString());

            if(kupdob_rec != null) redakRobneKartice4DLG.KupdobName = kupdob_rec.Naziv;
            else                   redakRobneKartice4DLG.KupdobName = "";

            messageList.Add(redakRobneKartice4DLG);

         } // foreach(Rtrans redakRobneKartice in sviRetciRobneKartice) 

         robnaKarticaInMinus_dlg = new VvMessageBoxDLG(false, ZXC.VvmBoxKind.RobnaKartica);
         robnaKarticaInMinus_dlg.TheUC.PutDgvFields_RobnaKartica(messageList);
         robnaKarticaInMinus_dlg.Text = "Robna Kartica artikla " + firstMinusRtrans.T_artiklCD + " " + firstMinusRtrans.T_artiklName + " na sklad. " + firstMinusRtrans.T_skladCD + " prije ove promjene."; 
         robnaKarticaInMinus_dlg.ShowDialog();
         robnaKarticaInMinus_dlg.Dispose();

      } // foreach(Rtrans firstMinusRtrans in firstMinusRtransList) 
   }

   public bool ShouldCheckMinusForTheVvDataRecord
   {
      get
      {
         // ipak NE, jerbo SaveTransesWriteMode zbunimo zbog fakta da je rtrans uvijek ADD iako rtrano moze biti bilosto od ADD/RWT/DEL 
       //if(TheVvUC is MOD_PTG_DUC) return true;

         if(TheVvUC is FakturDUC == false ||
            TheVvDataRecord is Faktur == false) return false;

         // Ako smo dosli do ovdje, znaci ovo je FakturDUC i mixer_rec

         FakturDUC fakturDUC = TheVvUC as FakturDUC;

         // Ne tu, nego caller naka vodi brigu sta ce u lucaju minusa 
         //if(fakturDUC.MinusPolicyIsStricktly == false) return false;

         Faktur faktur_rec = TheVvDataRecord as Faktur;

         if(faktur_rec.TtInfo.IsFinKol_TT == false) return false;

         // 18.12.2013 BIJO BUG! 
         //if(faktur_rec.TtInfo.IsNivelacijaZPC == false) return false;
         if(faktur_rec.TtInfo.IsNivelacijaZPC == true) return false;

         // Ok. Now we should check indeed... 

         // 07.09.2022: privremeno SVD Roman Donacije DELLMELATTER 
         // evo, deletao
         //if(ZXC.IsSvDUH_donSkl(faktur_rec.SkladCD)) return false;

         return true;
      }
   }

   #endregion Check For Minus

   #region Create Image & Icon Safely

   private Image CreateImageSafely(Icon origIcon, int iconSquareSize)
   {
      Icon theIcon;
      Image theImage;
      try
      {
         // 04.11.2022: 
       //theIcon = new Icon(origIcon, iconSquareSize, iconSquareSize);
       //theImage = theIcon.ToBitmap();
         theImage = origIcon.ToBitmap();
      }
      catch
      {
         theImage = null;
      }

      return theImage;
   }

   // 03.11.2022: 
 //private Image CreateImageSafely(Icon origIcon  , int iconWidth, int iconHeight  )
   private Image CreateImageSafely(Icon origIcon/*, int iconWidth, int iconHeight*/)
   {
      Icon theIcon;
      Image theImage;
      try
      {
         // 04.11.2022: 
       //theIcon = new Icon(origIcon, iconSize, iconSize);
       //theImage = theIcon.ToBitmap();
         theImage = origIcon.ToBitmap();
      }
      catch
      {
         theImage = null;
      }

      return theImage;
   }

   protected Icon CreateIconSafely(System.IO.Stream stream)
   {
      Icon theIcon;
      try
      {
         theIcon = new Icon(stream);
      }
      catch
      {
         theIcon = null;
      }

      return theIcon;
   }

   #endregion Create Image & Icon Safely

   #region Offix like button actions (ADDREC, RWTREC, DELREC, FIND, SAVE, ...)

   #region ADD, EDIT, DELETE

   public static bool IsProgramNOT_StartedToday()
   {
      bool programIsNOT_StartedToday = DateTime.Now.DayOfYear != ZXC.programStartedOnDateTime.DayOfYear; // da li je u programu 'od jucer' ili jos od prije... 

      if(programIsNOT_StartedToday)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Datum ulaska u program [{0}] je stariji od 'danas'.\n\nIzađite, pa ponovno uđite u program.", ZXC.programStartedOnDateTime.ToString(ZXC.VvDateFormat));
         return true;
      }

      // 13.09.2017: @TH Shop VvJanitorIsInProgress, go out! start 
      if(ZXC.IsTHshopDuringJanitorTime)
      {
         ZXC.IssueTHshopDuringJanitorTimeMessage();
         return true;
      }
      // 13.09.2017: @TH Shop VvJanitorIsInProgress, go out! end 

      return false;
   }

   /* */
   /* ADD RECORD */
   /* */

   public void NewRecord_OnClick(object sender, EventArgs e)
   {
      if(BadGuys(VvSQL.DB_RW_ActionType.ADD)) return;

      if(HasNotEnoughPrivilegesForThisAction(ZXC.PrivilegedAction.ADDREC, TheVvUC.TheSubModul, "")) { ZXC.FakturRec = null; return; }

      if(TheVvRecordUC.IsOkToInitiateThisAction(ZXC.WriteMode.Add) == false)                        { ZXC.FakturRec = null; return; }

      // 08.04.2016: 
      if(IsProgramNOT_StartedToday()) return;

      // 18.02.2019: VvXmlDocumentDR AUTO RECOVERY
      if(ZXC.VvXmlDR_LastDocumentMissing_AlertRaised && TheVvRecordUC is FakturDUC && (TheVvRecordUC as FakturDUC).faktur_rec.IsFiskalDutyFaktur)
      {
         if(ZXC.CurrUserHasSuperPrivileges)
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "VvXmlDRfilesAlertRaised!\n\nOperacija 'Novi' dozvoljena ISKLJUČIVO za popunjavanje 'rupe' u brojevima računa\n\nsa praznim IRM-om!");
         }
         else
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "VvXmlDRfilesAlertRaised!\n\nKONTAKTIRAJTE SUPPORT!");
            return;
         }
      }

      if(sender is VvDataRecord) // Kopiraj button 
      {
         if(ForbiddenDocument_TT()) return;

         OpenForWriteActions(ZXC.WriteMode.Edit);

         #region RISK Copy to other DUC

         if(ZXC.RISK_CopyToOtherDUC_inProgress)
         {
            ClearAllVvUcFields();
            
            // 24.05.2013: 
            // Zbog RwtrecFeedback ne smijes mijenjati ZXC.FakturRec
            //Faktur fakturCopied_rec = ZXC.FakturRec.MakeDeepCopy(); ... idijote, ovo ne Clona transove! 
            Faktur fakturNEW_rec = null;
            Mixer  mixerNEW_rec  = null;

            if(ZXC.RISK_CopyToMixerDUC_inProgress) mixerNEW_rec  = (Mixer )(ZXC.MixerRec .CreateNewRecordAndCloneItComplete());
            else                                   fakturNEW_rec = (Faktur)(ZXC.FakturRec.CreateNewRecordAndCloneItComplete());

            #region Check Unappropriate ZTR existance

            if(fakturNEW_rec != null && ZXC.FakturRec.IsZtrPresent && Rtrans.CheckZtrColExists() == false) // kod npr. kopiranja devizne primke u UFA-u, a netko je vec na PRI zalijepio ztr koji ne smiju doci na UFA-u
            {
             //ZXC.FakturRec   .S_ukZavisni = 0.00M;
               fakturNEW_rec.S_ukZavisni = 0.00M;
            }

            #endregion Check Unappropriate ZTR existance

            #region Clear Unappropriate Ponuda Data

            if(fakturNEW_rec != null && ZXC.FakturRec.TtInfo.IsPonudaTT)
            {
             //ZXC.FakturRec.PonudDate = DateTime.MinValue;
             //ZXC.FakturRec.RokPonude = 0;
               fakturNEW_rec.PonudDate = DateTime.MinValue; // PAZI! Kod PPMV IRMa ovo je datum uplate ppmva 
               fakturNEW_rec.RokPonude = 0;
            }

            #endregion Clear Unappropriate Ponuda Data

            #region RNM to PPR or PIP

            bool documentSumIsMissing = false;

            if(fakturNEW_rec != null && fakturNEW_rec.TT == Faktur.TT_RNM &&
               (TheVvUC is PredatUProizDUC ||
                TheVvUC is PIPDUC          ))
            {
               NewRecordEventArgs nrea = (e as NewRecordEventArgs);

               decimal newKolSum = 0M;

               fakturNEW_rec.OsobaX    = "";
               fakturNEW_rec.ProjektCD = fakturNEW_rec.TT_And_TtNum;

               fakturNEW_rec.DokDate = /*ZXC.FakturRec.DokDate =*/ VvSQL.GetServer_DateTime_Now(TheDbConnection);

               if(TheVvUC is PredatUProizDUC)
               {
                  fakturNEW_rec.Transes.RemoveAll(rtr => rtr.T_TT != Faktur.TT_RNM);

                  fakturNEW_rec.SkladCD2 = "";

                  ArtStat artStat_rec;

                  foreach(Rtrans PPRrtrans_rec in fakturNEW_rec.Transes)
                  {
                     PPRrtrans_rec.T_skladDate = fakturNEW_rec.DokDate;

                     artStat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, PPRrtrans_rec);

                     PPRrtrans_rec.T_kol = -1M * FakturDUC.GetDiffKol_PlanVsRealizacijaPIPR(PPRrtrans_rec.T_artiklCD, (nrea.RecordUC as IVvRealizableFakturDUC).RealizRtrList_AllYears, PPRrtrans_rec.T_kol);
                     newKolSum += PPRrtrans_rec.T_kol;

                     if(artStat_rec != null)
                     {
                        PPRrtrans_rec.T_cij = artStat_rec.PrNabCij;
                      //(TheVvUC as PredatUProizDUC).TheG.PutCell((TheVvUC as PredatUProizDUC).DgvCI.iT_cij, PPRrtrans_rec.T_serial, PPRrtrans_rec.T_cij);
                      //(TheVvUC as PredatUProizDUC).
                     }
                  }
                  if(newKolSum.IsZero()) ZXC.aim_emsg(MessageBoxIcon.Warning, "Upozorenje.\n\nZa RNM su već utrošene sve planirane PPR količine?!");

                  documentSumIsMissing = true; // ! (RNM nema s_ukKC, etc, ...) 
               }
               if(TheVvUC is PIPDUC)
               {
                  fakturNEW_rec.Transes.RemoveAll(rtr => rtr.T_TT != Faktur.TT_RNU);
                  fakturNEW_rec.SkladCD  = fakturNEW_rec.SkladCD2;
                  fakturNEW_rec.SkladCD2 = "";

                  foreach(Rtrans PIPrtrans_rec in fakturNEW_rec.Transes)
                  {
                     PIPrtrans_rec.T_kol = -1M * FakturDUC.GetDiffKol_PlanVsRealizacijaPIPR(PIPrtrans_rec.T_artiklCD, (nrea.RecordUC as IVvRealizableFakturDUC).RealizRtrList_AllYears, PIPrtrans_rec.T_kol);
                     newKolSum += PIPrtrans_rec.T_kol;
                  }
                  if(newKolSum.IsZero()) ZXC.aim_emsg(MessageBoxIcon.Warning, "Upozorenje.\n\nZa RNM su već proizvedene sve planirane PIP količine?!");
               }
            } // PPR / PIP 

            #endregion RNM to PPR or PIP

            #region SVD URA to IZD ... Set PrNabCij

            // 25.01.2021: zapravo, kad god i sto god kopirali u IZD_SVD_DUC; treba refresh-ati PrNabCij!? 
          //if(fakturCopied_rec != null && fakturCopied_rec.TT == Faktur.TT_URA && TheVvUC is IZD_SVD_DUC)
            if(fakturNEW_rec    != null                                        && (TheVvUC is IZD_SVD_DUC || TheVvUC is ZAH_SVD_DUC))
            {
               fakturNEW_rec.DokDate = VvSQL.GetServer_DateTime_Now(TheDbConnection);

               if(fakturNEW_rec.SkladCD.IsEmpty()) fakturNEW_rec.SkladCD = "10";

               ArtStat artStat_rec;

               foreach(Rtrans IZDorZAHrtrans_rec in fakturNEW_rec.Transes)
               {
                  IZDorZAHrtrans_rec.T_skladDate = fakturNEW_rec.DokDate;

                  if(TheVvUC is ZAH_SVD_DUC) IZDorZAHrtrans_rec.T_ttSort = ZXC.TtInfo(Faktur.TT_ZAH).TtSort;
                  else                       IZDorZAHrtrans_rec.T_ttSort = ZXC.TtInfo(Faktur.TT_IZD).TtSort;

                  artStat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, IZDorZAHrtrans_rec);

                  IZDorZAHrtrans_rec.T_pdvSt = 0.00M;
                  if(artStat_rec != null)
                  {
                     IZDorZAHrtrans_rec.T_cij = artStat_rec.PrNabCij;
                   //(TheVvUC as PredatUProizDUC).TheG.PutCell((TheVvUC as PredatUProizDUC).DgvCI.iT_cij, PPRrtrans_rec.T_serial, PPRrtrans_rec.T_cij);
                   //(TheVvUC as PredatUProizDUC).
                  }
               }
            }

            // kod nastajanja nove IZD ili ZAH; ocisti odobravatelja: 
            if(fakturNEW_rec != null && (TheVvUC is IZD_SVD_DUC || TheVvUC is ZAH_SVD_DUC)) fakturNEW_rec./*OdgvPersName*/Napomena2 = "";

            // kod nastajanja nove IZD iz IZD; ocisti pacijenta: 
            if(fakturNEW_rec != null && fakturNEW_rec.TT == Faktur.TT_IZD && TheVvUC is IZD_SVD_DUC)
            {
               fakturNEW_rec.PersonName =
               fakturNEW_rec.OpciAlabel =
               fakturNEW_rec.OpciAvalue = "";
            }

            // kod nastajanja nove ZAH iz ZAH; ocisti pacijenta: 
            if(fakturNEW_rec != null && fakturNEW_rec.TT == Faktur.TT_ZAH && TheVvUC is ZAH_SVD_DUC)
            {
               fakturNEW_rec.PersonName =
               fakturNEW_rec.OpciAlabel =
               fakturNEW_rec.OpciAvalue = "";
            }

            #endregion SVD URA to IZD ... Set PrNabCij

            #region SVD UGO to NRD ... Set ORG and COP

            if(fakturNEW_rec != null && fakturNEW_rec.TT == Faktur.TT_UGO && TheVvUC is NRD_SVD_DUC)
            {
               fakturNEW_rec.DokDate = VvSQL.GetServer_DateTime_Now(TheDbConnection);

               ArtStat artStat_rec;

               foreach(Rtrans NRDrtrans_rec in fakturNEW_rec.Transes)
               {
                  NRDrtrans_rec.T_skladDate = fakturNEW_rec.DokDate;

                  NRDrtrans_rec.T_ttSort = ZXC.TtInfo(Faktur.TT_NRD).TtSort;

                  decimal theORG = FakturDUC.Get_SVD_theORG(TheDbConnection, NRDrtrans_rec.T_artiklCD, false);
                  NRDrtrans_rec.T_doCijMal = theORG; // ORG 
                  NRDrtrans_rec.T_kol      = 0M    ;
               }
            }

            #endregion SVD UGO to NRD ... Set ORG and COP

            #region Set eventual missing T_isIrmUsluga

            if(fakturNEW_rec != null && TheVvUC is FakturDUC &&
                 (TheVvUC as FakturDUC).TheG.Columns["t_isIrmUslug"] != null &&
                 (TheVvUC as FakturDUC).TheG.Columns["t_isIrmUslug"].Visible
               ) // if() 
            {
               foreach(Rtrans rtrans_rec in fakturNEW_rec.Transes)
               {
                  Artikl artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(a => a.ArtiklCD == rtrans_rec.T_artiklCD);

                  if(artikl_rec != null)
                  {
                     rtrans_rec.T_isIrmUsluga = artikl_rec.IsMinusOK_or_UDP_Artikl; // pravilo isto kao i u AnyArtiklTextBox_OnGrid_Leave 

                     //(TheVvUC as PredatUProizDUC).TheG.PutCell((TheVvUC as PredatUProizDUC).DgvCI.iT_cij, PPRrtrans_rec.T_serial, PPRrtrans_rec.T_cij);
                     //(TheVvUC as PredatUProizDUC).
                  }
               }
            }

            #endregion Set eventual missing T_isIrmUsluga

            #region Clear eventual T_kol2 from TT_OPN

            // 22.11.2016: 
            if(fakturNEW_rec != null && TheVvUC is FakturDUC && fakturNEW_rec.TT == Faktur.TT_OPN) // if() 
            {
               fakturNEW_rec.Transes.ForEach(rtr => rtr.T_kol2 = 0M);
            }

            #endregion Clear eventual T_kol2 from TT_OPN

            #region UFA or URA to ISP ___ AND ___ IFA or IRA to UPL

            // tu, zasada, ne radimo razliku da li Uplat/Isplat kreiramo via genericka KopirajU ili ciljanom submodul akcijom (npr. UFA to ISP) 

          //bool is4Isplatnica = fakturNEW_rec != null &&  TheVvUC is BlgIsplatDUC                                && fakturNEW_rec.TT != Faktur.TT_ISP; // ovaj zadnji != sluzi da razdvojimo 'Kopiraj' od 'Kopiraj U'. kod kopiraj NE zelimo uci u if 
            bool is4Isplatnica = fakturNEW_rec != null && (TheVvUC is BlgIsplatDUC || TheVvUC is BlgIsplat_M_DUC) && fakturNEW_rec.TT != Faktur.TT_ISP; // ovaj zadnji != sluzi da razdvojimo 'Kopiraj' od 'Kopiraj U'. kod kopiraj NE zelimo uci u if 

            if(is4Isplatnica) 
            {
               fakturNEW_rec.VezniDok = "Isplata po računu " + fakturNEW_rec.TT_And_TtNum; // ili prva stavka?! ne znam

               decimal cashPonder = 1.00M;
               bool shouldPonder  = false;

               if(fakturNEW_rec.R_IsNpMix && fakturNEW_rec.R_ukKCRP_cash.NotZero())
               {
                  shouldPonder = true;
                  cashPonder   = ZXC.DivSafe(fakturNEW_rec.R_ukKCRP_cash, fakturNEW_rec.S_ukKCRP);
               }

               foreach(Rtrans ISPrtrans_rec in fakturNEW_rec.Transes)
               {
                  // nota bene, ukoliko prije sejvanja trebas npr artsat record (vidi gore za 'SVD URA to IZD') 
                  // moras rucno postaviti T_skladDate i T_ttSort                                               

                  ISPrtrans_rec.T_pdvSt = 0.00M                     ;
                //ISPrtrans_rec.T_cij   = ISPrtrans_rec.R_CIJ_KCRP  ;
                  if(shouldPonder)
                  {
                     ISPrtrans_rec.T_cij = ISPrtrans_rec.R_CIJ_KCRP * cashPonder;
                   //ISPrtrans_rec.CalcTransResults(null); NE jer kad ide CalcTrans_VELEP_Results_ByMPC onda zjebe 
                  }
                  else
                  {
                     ISPrtrans_rec.T_cij = ISPrtrans_rec.R_CIJ_KCRP;
                  }

                  ISPrtrans_rec.T_jedMj = fakturNEW_rec.TT_And_TtNum;
                  ISPrtrans_rec.T_konto = fakturNEW_rec.Konto.NotEmpty() ? fakturNEW_rec.Konto : ZXC.KSD.Dsc_RKto_Dobav;
               }
            }

          //bool is4Uplatnica = fakturNEW_rec != null &&  TheVvUC is BlgUplatDUC                               && fakturNEW_rec.TT != Faktur.TT_UPL; // ovaj zadnji != sluzi da razdvojimo 'Kopiraj' od 'Kopiraj U'. kod kopiraj NE zelimo uci u if 
            bool is4Uplatnica = fakturNEW_rec != null && (TheVvUC is BlgUplatDUC || TheVvUC is BlgUplat_M_DUC) && fakturNEW_rec.TT != Faktur.TT_UPL; // ovaj zadnji != sluzi da razdvojimo 'Kopiraj' od 'Kopiraj U'. kod kopiraj NE zelimo uci u if 

            if(is4Uplatnica)
            {
             //29.12.2021. Romana hoce u VezDok(svrha) napomenu sa racuna
             //fakturNEW_rec.VezniDok = "Uplata po računu " + fakturNEW_rec.TT_And_TtNum;
               if(ZXC.KSD.Dsc_IsIFAtoUPL_napomena)
               {
                  fakturNEW_rec.VezniDok = fakturNEW_rec.Napomena                          ;
                  fakturNEW_rec.Napomena = "Uplata po računu " + fakturNEW_rec.TT_And_TtNum;
               }
               else
               {
                  fakturNEW_rec.VezniDok = "Uplata po računu " + fakturNEW_rec.TT_And_TtNum;
               }

               decimal cashPonder = 1.00M;
               bool shouldPonder  = false;

               if(fakturNEW_rec.R_IsNpMix && fakturNEW_rec.R_ukKCRP_cash.NotZero())
               {
                  shouldPonder = true;
                  cashPonder = ZXC.DivSafe(fakturNEW_rec.R_ukKCRP_cash, fakturNEW_rec.S_ukKCRP);
               }

               foreach(Rtrans UPLrtrans_rec in fakturNEW_rec.Transes)
               {
                  // nota bene, ukoliko prije sejvanja trebas npr artsat record (vidi gore za 'SVD URA to IZD') 
                  // moras rucno postaviti T_skladDate i T_ttSort                                               

                  UPLrtrans_rec.T_pdvSt = 0.00M;
                //ISPrtrans_rec.T_cij   = ISPrtrans_rec.R_CIJ_KCRP             ;
                  if(shouldPonder)
                  {
                     UPLrtrans_rec.T_cij = UPLrtrans_rec.R_CIJ_KCRP * cashPonder;
                   //ISPrtrans_rec.CalcTransResults(null); NE jer kad ide CalcTrans_VELEP_Results_ByMPC onda zjebe 
                  }
                  else
                  {
                     UPLrtrans_rec.T_cij = UPLrtrans_rec.R_CIJ_KCRP;
                  }

                  UPLrtrans_rec.T_jedMj = fakturNEW_rec.TT_And_TtNum;
                  UPLrtrans_rec.T_konto = fakturNEW_rec.Konto.NotEmpty() ? fakturNEW_rec.Konto : ZXC.KSD.Dsc_RKto_Kupca;
               }
            }

            #endregion UFA or URA to ISP

            #region CopyTO some PDV TT ... set PDV_Knjiga

            // 30.03.2021: poboljsavamo logiku Fld_PdvKnjiga
            // ALI ODUSTAJEMO 
            //if(fakturNEW_rec != null && TheVvUC is FakturExtDUC && (TheVvUC as FakturDUC).Default_TtInfo.IsUlazniPdvTT) // PDV ovo ono 
            //{
            //   Kupdob kupdob_rec = TheVvUC.Get_Kupdob_FromVvUcSifrar(fakturNEW_rec.KupdobCD);
            //
            //   if(kupdob_rec != null)
            //   {
            //      (TheVvUC as FakturExtDUC).PutAllKupdobFields(kupdob_rec); // ovaj PutAllKupdobFields rijesi pdv polja a prek adresara 
            //   }
            //}

            #endregion CopyTO some PDV TT ... set PDV_Knjiga

            #region isPTG_KUG_to_AUN || isPTG_UGNorAUN_to_DOD 

            bool isPTG_KUG_to_AUN      = fakturNEW_rec != null && TheVvUC is ANU_PTG_DUC && fakturNEW_rec.TT == Faktur.TT_KUG                                      ;
            bool isPTG_UGNorAUN_to_DOD = fakturNEW_rec != null && TheVvUC is DIZ_PTG_DUC &&(fakturNEW_rec.TT == Faktur.TT_UGN || fakturNEW_rec.TT == Faktur.TT_AUN);

            if(isPTG_KUG_to_AUN || isPTG_UGNorAUN_to_DOD)
            {
               fakturNEW_rec.DokDate = VvSQL.GetServer_DateTime_Now(TheDbConnection);
            }

            if(TheVvUC is FakturDUC && (TheVvUC as FakturDUC).IsPTG_UgAnDod_DUC)
            {
               fakturNEW_rec.InvokeTransClear2();
            }

            #endregion isPTG_KUG_to_AUN || isPTG_UGNorAUN_to_DOD

            #region Tetragram IZD_MPC to IRA_MPC

            if(fakturNEW_rec != null && fakturNEW_rec.TT == Faktur.TT_IZD && TheVvUC is IRA_MPC_DUC)
            {
               fakturNEW_rec.Transes.ForEach(rtr => rtr.T_artiklCD = "");
            }

            #endregion Tetragram IZD_MPC to IRA_MPC

            #region Tetragram PON_MPC to ZAR

            if(fakturNEW_rec != null && fakturNEW_rec.TT == Faktur.TT_PON && TheVvUC is ZAR_DUC)
            {
               fakturNEW_rec.Transes.ForEach(rtr => rtr.T_pdvColTip = ZXC.PdvKolTipEnum.UMJETN);

               VvLookUpItem ZAR_SKL_lui = ZXC.luiListaSkladista.SingleOrDefault(lui => lui.Cd.StartsWith("Z-ZA-") && lui.Integer != 20);
               if(ZAR_SKL_lui != null)
               {
                  fakturNEW_rec.SkladCD = ZAR_SKL_lui.Cd;
               }
               else
               {
                  ZXC.aim_emsg(MessageBoxIcon.Error, "Nema ZAR_SKL_lui!?");
               }
            }

            #endregion Tetragram IZD_MPC to IRA_MPC

            #region Clear ALL F2 Fields, on RISK_FinalRn clear Rtranses

            if(fakturNEW_rec != null)
            {
               /*198 */    fakturNEW_rec.F2_ElectronicID   = 0;
               /*199 */    fakturNEW_rec.F2_StatusCD       = 0;
               /*200 */    fakturNEW_rec.F2_ArhRecID       = 0;
               /*201 */    fakturNEW_rec.F2_IsFisk         = 0;
               /*202 */    fakturNEW_rec.F2_SentTS         = DateTime.MinValue;
               /*203 */    fakturNEW_rec.F2_IsRejected     = 0;
               /*204 */    fakturNEW_rec.F2_IsMarkAsPaid   = 0;
             ///*205 */    fakturNEW_rec.F2_R1kind         = /*false*/0;
               /*206 */    fakturNEW_rec.F2_PrvFakYYiRecID = 0;
               /*207 */    fakturNEW_rec.F2_IsEizvj        = 0;
            }

            if(ZXC.RISK_FinalRn_inProgress)
            {
               fakturNEW_rec.Napomena = "KONAČNI rn ";

               fakturNEW_rec.InvokeTransClear();
            }

            #endregion Clear ALL F2 Fields, on RISK_FinalRn clear Rtranses

            PutFieldsInProgress = true;
            // 24.05.2013: 
            // Zbog RwtrecFeedback ne smijes mijenjati ZXC.FakturRec
          //TheVvRecordUC.PutFields(ZXC.FakturRec,    true);
            if(ZXC.RISK_CopyToMixerDUC_inProgress) TheVvRecordUC.PutFields(mixerNEW_rec , true);
            else                                   TheVvRecordUC.PutFields(fakturNEW_rec, true);

            // 03.03.2016: ciau nona Francesca 
            if(documentSumIsMissing) TheVvDocumentRecordUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();

            PutFieldsInProgress = false;

            TheVvRecordUC.PutDefaultDUCfields();

            // 13.07.2015
            // by vvDelf - OVO JE BILO TRICKY
            // vjerojatno se ne treba zvati, ali nije sigurno!
            // TheVvDocumentRecordUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
          
            TheVvTabPage.Fld_PrjktNaziv = TheVvTabPage.TheDbConnection.Database; // TamponPanel_Header

         }

         if(TheVvUC is UgovoriDUC && ZXC.MIXER_CopyToOtherDUC_inProgress)
         {
            ClearAllVvUcFields();
         
            TheVvRecordUC.PutDefaultDUCfields();
         
            Mixer mixerCopied_rec = (Mixer)(ZXC.MixerRec.CreateNewRecordAndCloneItComplete());
         
            (TheVvUC as UgovoriDUC).PutUgovoriFieldsFromUrudzbeniData(mixerCopied_rec);
         
            TheVvDocumentRecordUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
         }

         #endregion RISK Copy to other DUC

         TheVvTabPage.WriteMode = ZXC.WriteMode.Add;

         if(TheVvRecordUC is VvDocumLikeRecordUC && !ZXC.RISK_CopyToOtherDUC_inProgress)
         {
            if(TheVvRecordUC is FakturDUC)
            {
               FakturDUC theFakturDUC = TheVvRecordUC as FakturDUC;
               theFakturDUC.SetVezaFields(theFakturDUC.faktur_rec);
            }

            string eventualSkladCD = (TheVvRecordUC is FakturDUC ? ((FakturDUC)TheVvRecordUC).Fld_SkladCD : null);

            uint newTTnum = TheVvDao.GetNextTtNum(TheDbConnection, ((VvDocumLikeRecord)TheVvDataRecord).VirtualTT, eventualSkladCD);

            TheVvDocumentLikeRecordUC.Put_NewTT_Num(newTTnum);
         }
         if(TheVvRecordUC is VvDocumentRecordUC )
         {
            ((VvDocumentRecord)TheVvDataRecord).InvokeTransClear(); // Za CalcResults: da iskopirani recordov business ima transove ishodisnog, ali bez REcID-ova 

            TheVvDocumentRecordUC.TheG.ClearDGV_RecIdColumn();

            if(TheVvRecordUC is VvPolyDocumRecordUC)
            {
               ((VvPolyDocumRecord)TheVvDataRecord).InvokeTransClear2(); // Za CalcResults: da iskopirani recordov business ima transove ishodisnog, ali bez REcID-ova 
               ((VvPolyDocumRecord)TheVvDataRecord).InvokeTransClear3(); // Za CalcResults: da iskopirani recordov business ima transove ishodisnog, ali bez REcID-ova 

               if(TheVvPolyDocumRecordUC.TheG2 != null) TheVvPolyDocumRecordUC.TheG2.ClearDGV_RecIdColumn();
               if(TheVvPolyDocumRecordUC.TheG3 != null) TheVvPolyDocumRecordUC.TheG3.ClearDGV_RecIdColumn();

               // 05.11.2024: 
               if(TheVvRecordUC is FakturDUC && (TheVvRecordUC as FakturDUC).IsPTG_DUC_wRtrano)
               {
                  if(TheVvPolyDocumRecordUC.TheG2 != null) TheVvPolyDocumRecordUC.TheG2.Rows.Clear();
               }
            }
         }

         // 19.01.2013: 
         TheVvRecordUC.CleanUniqueFieldsOnCopyFromOtherRecord();

         // 17.09.2014:
         TheVvUC.OnCopyRecordPutSomeSpecificFields();

         TheVvUC.GetFields(false); // Za CalcResults: da iskopirani recordov business ima transove ishodisnog, ali bez REcID-ova 

      } // if(sender is VvDataRecord) // Kopiraj button 

      else // NIJE Kopiraj 
      {
         ClearAllVvUcFields();

         OpenForWriteActions(ZXC.WriteMode.Add);

         TheVvRecordUC.PutDefaultDUCfields();
      }
      
      if(TheVvDataRecord.IsDocumentLike)
      {
         uint     dokNum  = TheVvDao.GetNextDokNum(TheDbConnection, TheVvDataRecord.VirtualRecordName);
       //uint     ttNum   = TheVvDao.GetLastVkNum_DaoAction (conn, (VvDocumentRecord)TheVvDataRecord);
         DateTime dokDate;

         // 14.09.2011: 
         #region RISK_CopyToOtherDUC_inProgress ADDITIONS

         if(ZXC.RISK_CopyToOtherDUC_inProgress && TheVvUC is FakturDUC)
         {
            FakturDUC fakturDUC = TheVvUC as FakturDUC;

            dokDate = ZXC.FakturRec.DokDate;

            // 08.02.2023: gore se jebes mudro postaviti fakturNEW_rec.DokDate a onda bi ga ovdje pregazio 
            // ovo sad pokusava ispraviti taj bug. na znaci da si uspio. vrijemecepokazati                 
            DateTime newWantedDokDate = fakturDUC.Fld_DokDate;
            if(newWantedDokDate.NotEmpty())
            {
               dokDate = newWantedDokDate;
            }

            // 10.01.2013: 
            if(fakturDUC != null && Faktur.IsFiskalDutyTT(fakturDUC.Fld_TT)) // ako je fakturDUC null, onda je RISK2MIXER 
            {
               dokDate = VvSQL.GetServer_DateTime_Now(TheDbConnection);
            }

            // 16.02.2017: 
            if(fakturDUC.Fld_TT == Faktur.TT_PIP || fakturDUC.Fld_TT == Faktur.TT_PPR)
            {
               dokDate = VvSQL.GetServer_DateTime_Now(TheDbConnection);
            }

            // 11.04.2018: SVD
            if(fakturDUC.Fld_TT == Faktur.TT_IZD || fakturDUC.Fld_TT == Faktur.TT_ZAH || ZXC.TtInfo(fakturDUC.Fld_TT).IsPTGFaktur_UgAnDodTT)
            {
               dokDate = VvSQL.GetServer_DateTime_Now(TheDbConnection);
            }

            #region if isDevizniDok - RefreshKunskaCijena

            // 10.05.2016: 

            if(ZXC.FakturRec.DevName.NotEmpty() && fakturDUC != null) // this is Devizni faktur_rec 
            {
               decimal oldKnCij;

               for(int rowIdx = 0; rowIdx < fakturDUC.TheG.RowCount - 1; ++rowIdx)
               {
                  oldKnCij = fakturDUC.TheG.GetDecimalCell(fakturDUC.DgvCI.iT_cij, rowIdx, false);

                  fakturDUC.TheG.PutCell(fakturDUC.DgvCI.iT_cij, rowIdx, ZXC.RefreshedKunskaCijena(oldKnCij, ZXC.FakturRec.DevTecaj, ZXC.DevTecDao.GetHnbTecaj(ZXC.FakturRec.DevNameAsEnum, dokDate)));
               }
            }

            #endregion if isDevizniDok - RefreshKunskaCijena

            #region if PSM to ZPC - Set ZPC specific columns

            if(fakturDUC != null && ZXC.FakturRec.TT == Faktur.TT_PSM && fakturDUC is NivelacijaDUC) 
            {
               decimal psmMPC;

               dokDate = fakturDUC.Fld_DokDate = new DateTime(ZXC.projectYearAsInt, 01, 02);

               for(int rowIdx = 0; rowIdx < fakturDUC.TheG.RowCount - 1; ++rowIdx)
               {
                  psmMPC = fakturDUC.TheG.GetDecimalCell(fakturDUC.DgvCI.iT_cij_MSK, rowIdx, false);
                  
                  fakturDUC.TheG.PutCell(fakturDUC.DgvCI.iT_doCijMal, rowIdx, psmMPC       );
                  fakturDUC.TheG.PutCell(fakturDUC.DgvCI.iT_noCijMal, rowIdx, psmMPC.Ron2());
                //fakturDUC.TheG.PutCell(fakturDUC.DgvCI.iT_mrzSt   , rowIdx,              );

               }
            }

            #endregion if PSM to ZPC - Set ZPC specific columns

            if(fakturDUC is FakturExtDUC)
            {
               FakturExtDUC fakturExtDUC =  fakturDUC as FakturExtDUC;

               fakturExtDUC.Fld_DospDate = /*DateTime.Now*/dokDate + new TimeSpan(fakturExtDUC.Fld_RokPlac, 0, 0, 0); // da ispravi Fld_DospDate na osnovi Fld_RokPlac sas orginala 
               fakturExtDUC.Fld_PdvDate  = dokDate;

               if(ZXC.IsTEXTHOshop && fakturDUC.Fld_TT == Faktur.TT_MVI)
               {
                  // 05.02.2015: 
                //fakturDUC.Fld_SkladCD2   = "12BPS";
                //fakturDUC.Fld_Sklad2Opis = "SKLADIŠTE POVRATA";
                //fakturDUC.Fld_SkladBR2   = 13;
                  if(ZXC.GetTEXTHO_Regija(/*fakturDUC.Fld_SkladCD2*/ ZXC.vvDB_ServerID) == 1)
                  {
                     fakturDUC.Fld_SkladCD2   = "12BPS";
                     fakturDUC.Fld_Sklad2Opis = "SKLADIŠTE POVRATA";
                     fakturDUC.Fld_SkladBR2   = 13;
                  }
                  else if(ZXC.GetTEXTHO_Regija(/*fakturDUC.Fld_SkladCD2*/ ZXC.vvDB_ServerID) == 2)
                  {
                     fakturDUC.Fld_SkladCD2   = "12BP2";
                     fakturDUC.Fld_Sklad2Opis = "SLAVONIJE POVSK";
                     fakturDUC.Fld_SkladBR2   = 11;
                  }
               }
            }

         } // if(ZXC.RISK_CopyToOtherDUC_inProgress) 

         #endregion RISK_CopyToOtherDUC_inProgress ADDITIONS

         else
         {
/* !!! */   // !!! VAZNO !!!                                                                                               
/* !!! */   // 02.01.2015: ako je if(faktur_rec.IsFiskalDutyFaktur_ONLINE && TheVvTabPage.WriteMode == ZXC.WriteMode.Add)  
/* !!! */   // tada se u FakturDUC_Q.cs: FakturDUC_Validating PREJEBAVA                                                    
/* !!! */   // Fld_DokDate = VvSQL.GetServer_DateTime_Now(TheDbConnection);                                                

            dokDate = VvSQL.GetServer_DateTime_Now(TheDbConnection);

          //if(dokDate > ZXC.projectYearLastDay                             ) dokDate = ZXC.projectYearLastDay;
            if(dokDate > ZXC.projectYearLastDay && ZXC.IsManyYearDB == false) dokDate = ZXC.projectYearLastDay;

            // 07.06.2018: RISK_FiskParagon_InProgress 
            if(ZXC.RISK_FiskParagon_InProgress)
            {
               FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

               theDUC.Prev_ParagonIRM_Date += ZXC.OneMinuteSpan;

               dokDate = theDUC.Prev_ParagonIRM_Date;
               theDUC.tbx_DokDate.JAM_ReadOnly = theDUC.tbx_TtNum.JAM_ReadOnly = true;
            }

/* !!! */   // !!! VAZNO !!!                                                                                               
/* !!! */   // 02.01.2015: ako je if(faktur_rec.IsFiskalDutyFaktur_ONLINE && TheVvTabPage.WriteMode == ZXC.WriteMode.Add)  
/* !!! */   // tada se u FakturDUC_Q.cs: FakturDUC_Validating PREJEBAVA                                                    
/* !!! */   // Fld_DokDate = VvSQL.GetServer_DateTime_Now(TheDbConnection);                                                
         }

         TheVvDocumentLikeRecordUC.Put_NewDocum_NumAndDateFields(dokNum, dokDate);

      } // if(TheVvDataRecord.IsDocumentLike)

      if(TheVvDataRecord.IsAutoSifra) // VvSifrarDataRecord-i 
      {
         uint newSifra;
         string sifraColName = TheVvSifrarRecordUC.VirtualSifrarRecord.SifraColName;

         if(TheVvDataRecord.IsStringAutoSifra) /* sifCD je 'string' */ newSifra = TheVvDao.GetNextSifra_String(TheDbConnection, TheVvDataRecord.VirtualRecordName, sifraColName);
         else                                  /* sifCD je 'uint'   */ newSifra = TheVvDao.GetNextSifra_Uint  (TheDbConnection, TheVvDataRecord.VirtualRecordName, sifraColName, TheVvDataRecord.UintSifraRootNum, TheVvDataRecord.UintSifraBaseFactor);

         TheVvSifrarRecordUC.PutNew_Sifra_Field(newSifra);
      }

      // added 21.3.2009: 
      if(sender is VvDataRecord == false) // znaci, NIJE 'Kopiraj button' 
      {
         TheVvUC.GetFields(false); BeginEdit(TheVvDataRecord); SetDirtyFlagDependentAppereance_NothingNewYet();
      }
      // 29.12.2010: 
      else
      {
         SetDirtyFlagDependentAppereance_EnableSave();
      }

      #region Check CIS Status additions

      // 10.01.2013: if(IsFisklaDuty) Check CIS Status 
      // 16.07.2015: ubio skroz 
      //if(TheVvRecordUC is FakturDUC)
      //{
      //   FakturDUC fakturDUC = TheVvUC as FakturDUC;
      // //bool isNpCash = false;
      //   string nacPlac = "";
      // //if(fakturDUC is FakturExtDUC) isNpCash = (fakturDUC as FakturExtDUC).Fld_IsNpCash;
      //   if(fakturDUC is FakturExtDUC) nacPlac  = (fakturDUC as FakturExtDUC).Fld_NacPlac ;
      //   if(Faktur.IsFiskalDutyTT_ONLINE(fakturDUC.Fld_TT, /*isNpCash*/nacPlac))
      //   {
      //      // 19.05.2015: 
      //      if(ZXC.NO_SKY_Connection)
      //      {
      //         TheVvTabPage.labTamPanCrta.BackColor = Color.Black; 
      //         return;
      //      }
      //
      //      Raverus.FiskalizacijaDEV.PopratneFunkcije.ServiceStatusEnum status = Raverus.FiskalizacijaDEV.PopratneFunkcije.ServiceStatusEnum.unknown;
      //      
      //      try
      //      {
      //         status = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.DohvatiStatusCisServisa();
      //      }
      //      catch(Exception ex)
      //      {
      //         ZXC.aim_emsg(MessageBoxIcon.Error, "NE MOGU USPOSTAVITI VEZU SA CIS-om\n\r\n\r" + ex.Message);
      //      }
      //
      //      if(status == Raverus.FiskalizacijaDEV.PopratneFunkcije.ServiceStatusEnum.green || 
      //         status == Raverus.FiskalizacijaDEV.PopratneFunkcije.ServiceStatusEnum.yellow)
      //         TheVvTabPage.labTamPanCrta.BackColor = Color.Green;
      //      else
      //         TheVvTabPage.labTamPanCrta.BackColor = Color.Black;
      //   }
      //
      //}

      #endregion Check CIS Status additions

      #region test 4 GetNextRecID
#if DEBUG
      //string dbName = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects.DataBaseName;
      //string tableName = TheVvDataRecord.VirtualRecordName;
      //uint nextRecID = VvSQL.Get_Sql_NextAutoIncrementRecID(TheDbConnection, tableName);
      //ZXC.aim_emsg("Next recID is : {0}", nextRecID);
#endif
      #endregion test 4 GetNextRecID

   }

   private void SetDirtyFlagDependentAppereance_NothingNewYet()
   {
      TheVvTabPage.PaliGasiDirtyFlag(false);

      SetVvMenuEnabledOrDisabled_Explicitly("ESC", true);
      SetVvMenuEnabledOrDisabled_Explicitly("SAV", false);
      SetVvMenuEnabledOrDisabled_Explicitly("SAS", false);
      SetVvMenuEnabledOrDisabled_Explicitly("SAK", false);
   }

   private void SetDirtyFlagDependentAppereance_EnableSave()
   {
      TheVvTabPage.PaliGasiDirtyFlag(false);

      SetVvMenuEnabledOrDisabled_Explicitly("ESC", true);
      SetVvMenuEnabledOrDisabled_Explicitly("SAV", true);
      SetVvMenuEnabledOrDisabled_Explicitly("SAS", true);
      SetVvMenuEnabledOrDisabled_Explicitly("SAK", true);
   }

   private void ClearAllVvUcFields()
   {
      if(TheVvDataRecord.IsDocument)
      {
         ((VvDocumentRecord)TheVvDataRecord).InvokeTransClear();

         TheVvDocumentRecordUC.TheG.Rows.Clear();

         if(TheVvDataRecord.IsPolyDocument)
         {
            ((VvPolyDocumRecord)TheVvDataRecord).InvokeTransClear2();

            if(TheVvPolyDocumRecordUC.TheG2 != null) TheVvPolyDocumRecordUC.TheG2.Rows.Clear();

            ((VvPolyDocumRecord)TheVvDataRecord).InvokeTransClear3();

            if(TheVvPolyDocumRecordUC.TheG3 != null) TheVvPolyDocumRecordUC.TheG3.Rows.Clear();
         } // if(TheVvDataRecord.IsPolyDocument) 
      }

      if(TheVvDataRecord.IsSifrar)
      {
         foreach(DataGridView dgv in TheVvSifrarRecordUC.aTransesGrid)
         {
            ((VvSifrarRecord)TheVvDataRecord).InvokeTransClear();

            if(dgv != null) dgv.Rows.Clear();
         }
      }

      VvHamper.ClearFieldContents(TheVvUC);
      VvHamper.ClearFieldContents(TheVvTabPage.TamponPanel_Footer);
      VvHamper.ClearFieldContents(TheVvTabPage.TamponPanel_Header);

   }

   private void DupCopyMenu_Button_OnClick(object sender, EventArgs e)
   {
      // 15.01.2026: 
      if(ZXC.RISK_CopyToOtherDUC_inProgress || ZXC.RISK_CopyToMixerDUC_inProgress || ZXC.MIXER_CopyToOtherDUC_inProgress)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Morate prvo usnimiti neku ranije započetu akciju kopiranja.");
         return;
      }

      if(BadGuys(VvSQL.DB_RW_ActionType.ADD)) return;

      // 08.04.2016: 
      if(IsProgramNOT_StartedToday()) return;

      // 2026: 
      ZXC.DupCopyMenu_inProgress = true;

      // 12.01.2017: 
      if(TheVvDataRecord is Faktur && (TheVvDataRecord as Faktur).Is_STORNO)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ne dozvoljava se kopiranje iz STORNO dokumenta.");
         return;
      }

      if(IsArhivaTabPage) ToggleArhivaVisualApperiance(false);

      else if(TheVvRecordUC is FakturDUC)
      {
         // ZXC.FakturRec = fakturToBeCopied_rec
         ZXC.FakturRec = (Faktur)(TheVvDataRecord.CreateNewRecordAndCloneItComplete());
         ZXC.RISK_CopyToOtherDUC_inProgress = true;
      }

      NewRecord_OnClick(TheVvDataRecord, EventArgs.Empty);

      // 20.09.2022: RISK_PromjenaNacPlac_inProgress additions
      #region RISK_PromjenaNacPlac_inProgress

      if(ZXC.RISK_PromjenaNacPlac_inProgress == true)
      {
         FakturDUC theDUC = TheVvDocumentRecordUC as FakturDUC;

         theDUC.Fld_VezniDok = /*theDUC.faktur_rec*/ZXC.FakturRec.TtNumFiskal;

         string stornoStr   = "NoviNP za " + theDUC.faktur_rec.BackupData._tt + "-" + theDUC.faktur_rec.BackupData._ttNum + "/";
         string oldNapomena = theDUC.faktur_rec.Napomena;

         if(oldNapomena.Length + stornoStr.Length <= theDUC.tbx_Napomena.MaxLength) theDUC.Fld_Napomena  = stornoStr + oldNapomena;
         else                                                                       theDUC.Fld_Napomena  = stornoStr + oldNapomena.Substring(0, oldNapomena.Length - stornoStr.Length);
      }

      #endregion RISK_PromjenaNacPlac_inProgress

      #region RISK_FinalRn_inProgress

      if(ZXC.RISK_FinalRn_inProgress == true)
      {
         FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

       //theDUC.Fld_eRproc = (ZXC.VvUBL_PolsProcEnum)Enum.Parse(typeof(ZXC.VvUBL_PolsProcEnum), wanted_eRproc);
         theDUC.Fld_eRproc =  ZXC.VvUBL_PolsProcEnum.P11; 
         VvLookUpItem lui = ZXC.luiListaeRacPoslProc.GetLuiForThisCd(/*theDUC.Fld_eRproc.ToString()*/ "11");
         if(lui != null) theDUC.Fld_eRprocOpis = lui.Name;

         lui = ZXC.luiListaKodTipaEracuna.GetLuiForThisCd("380");
         theDUC.Fld_StatusCD   = lui != null ? lui.Cd   : "";
         theDUC.Fld_StatusOpis = lui != null ? lui.Name : "";

         // !!! !!! !!! 
       //theDUC.Fld_F2_PrvFakRecID = ZXC.FakturRec.               RecID; // referenca na prethodni dokument 
         theDUC.Fld_F2_PrvFakRecID = ZXC.FakturRec.GetFaktur_YYandRecID; // referenca na prethodni dokument 

      }

      #endregion RISK_FinalRn_inProgress

      ZXC.DupCopyMenu_inProgress = false;
   }

   /* */
   /* EDIT RECORD */
   /* */

   /*private*/
   public void EditRecord_OnClick(object sender, EventArgs e)
   {
      if(BadGuys(VvSQL.DB_RW_ActionType.RWT)) return;

      // 08.04.2016: 
      if(IsProgramNOT_StartedToday()) return;

      // 24.01.2014: 
      if(TheVvTabPage.WriteMode != ZXC.WriteMode.None) return;

      if(TheVvDataRecord.VirtualIsLocked) { ZXC.aim_emsg(MessageBoxIcon.Information, "Akcija odbijena.\n\nZapis je trajno zaključan."); return; }

      if(ForbiddenDocument_TT()) return;

      if(HasNotEnoughPrivilegesForThisAction(ZXC.PrivilegedAction.RWTREC, TheVvUC.TheSubModul, this.TheVvDocumDataRecordDocumType)) return;

      if(TheVvRecordUC.IsOkToInitiateThisAction(ZXC.WriteMode.Edit) == false) return;

      // 09.02.2016: 
      if(TheVvRecordUC is VvDocumLikeRecordUC)
      {
         if(VvDaoBase.IsDocumentFromLockedPeriod(((VvDocumLikeRecord)TheVvDataRecord).VirtualDokDate, false)) return;
      }

      bool OK;

      Cursor.Current = Cursors.WaitCursor;

      OK = TheVvDao.IsInDBUnchanged(TheDbConnection, TheVvDataRecord.CreateNewRecordAndCloneItComplete()); // da li je u fajlu jos uvijek isti? 

      Cursor.Current = Cursors.Default;
      
      TheVvLockerInfo = new VvSQL.VvLockerInfo(TheVvDataRecord.VirtualRecordName, TheVvDataRecord.VirtualRecID);

      if(OK && IsSomeoneElseAllreadyEditingThisRecord(TheVvLockerInfo)) return;

      if(OK)
      {
         //   
         //                                                                                                         
         /* */ TheArhivedVvDataRecord = TheVvDataRecord.CreateArhivedDataRecord(TheDbConnection, "ISPRAVAK"); //    
         //                                                                                                         
         //   

         OpenForWriteActions(ZXC.WriteMode.Edit);

         // --- Let's lock this succker for editing? --- start ---

         OK = LockThisRecordForEdit(TheVvLockerInfo);

         if(!OK) ZXC.aim_emsg("Ne mogu zakljucati zapis!");

         // --- Let's lock this succker for editing? ---  end  ---

         OpenMenu_ProjectSpecific_Actions();

      }
      else // iz nekog razloga record u db vise nije isti 
      {
         WhenRecordInDBHasChangedAction();
      }

   }

   /* */
   /* DELETE RECORD */
   /* */

   private void DeleteRecord_OnClick(object sender, EventArgs e)
   {
      if(BadGuys(VvSQL.DB_RW_ActionType.DEL)) return;

      // 08.04.2016: 
      if(IsProgramNOT_StartedToday()) return;

      if(TheVvDataRecord.VirtualIsLocked) { ZXC.aim_emsg(MessageBoxIcon.Information, "Akcija odbijena.\n\nZapis je trajno zaključan."); return; }

      if(ForbiddenDocument_TT()) return;

      if(HasNotEnoughPrivilegesForThisAction(ZXC.PrivilegedAction.DELREC, TheVvUC.TheSubModul, this.TheVvDocumDataRecordDocumType)) return;

      if(TheVvRecordUC.IsOkToInitiateThisAction(ZXC.WriteMode.Delete) == false) return;

      if(CheckForMinus(ZXC.WriteMode.Delete)) return;

      // 09.02.2016: 
      if(TheVvRecordUC is VvDocumLikeRecordUC)
      {
         if(VvDaoBase.IsDocumentFromLockedPeriod(((VvDocumLikeRecord)TheVvDataRecord).VirtualDokDate, false)) return;
      }

      // 2026: 


      bool OK;
      TheVvLockerInfo = new VvSQL.VvLockerInfo(TheVvDataRecord.VirtualRecordName, TheVvDataRecord.VirtualRecID);

      DialogResult result = MessageBox.Show("Da li zaista zelite obrisati stavku \'" + TheVvDataRecord.ToString() + "\'",
         "Potvrdite BRISANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if (result != DialogResult.Yes) return;

      Cursor.Current = Cursors.WaitCursor;

      OK = TheVvDao.IsInDBUnchanged(TheDbConnection, TheVvDataRecord); // da li je u fajlu jos uvijek isti? 

      Cursor.Current = Cursors.Default;

      if(OK && IsSomeoneElseAllreadyEditingThisRecord(TheVvLockerInfo)) return;

      if(OK && TheVvDao.IsThisRecordInSomeRelation(ZXC.PrivilegedAction.DELREC, TheDbConnection, TheVvDataRecord)) return; // ====== Check for CROSS-TABLE integrity violation 

      // ====== User 4 MySQL additions START =============================
      // 10.10.2022: SKIP on SVDZAH ODOBRAVATELJ 
    //if(OK && TheVvDataRecord.VirtualRecordName == User.recordName                                                                            )
      if(OK && TheVvDataRecord.VirtualRecordName == User.recordName && !ZXC.IsSVD_ZAH_ODOBRAVATELJ_UserName((TheVvDataRecord as User).UserName))
      {
         OK = VvSQL.DropUser(TheDbConnection, TheVvDataRecord as User);
      }
      // ====== User 4 MySQL additions END   =============================

      if(OK)
      {
         OK = TheVvDao.DELREC(TheDbConnection, TheVvDataRecord, false);
      }

      #region VvXmlDocumentDR AUTO RECOVERY - OnDelete invalidate '$_'

      // 04.10.2019: observacija: 
      // pri izradi Restore_FromVvXmlDR_OnClick smo skuzili da ovo rjesenje 'if(/*deletedWasMaxTtNum*/true) // bezuvjetno! ' 
      // a zbog situacije 'Roman' (ADD IZD-3, ADD IZD-4, DEL IZD-3, rename IZD-4 to IZD-3) 
      // je BUG kada imas recored dodadn u sijecnju a brises ga na 02.12. (a pak gdje je backup od 01.12.)
      // treba pomiriti te dvije stvari 
      // ... 
      // 10.10.2019: Pomirio si. Search pattern za Restore_FromVvXmlDR_OnClick si modificirao tako da bude 
      // UNIJA '$' i '#' 



      // 11.04.2019: 
      if(OK && TheVvUC is VvDocumentRecordUC) 
      {
         //string eventualSkladCD = (TheVvUC is FakturDUC ? ((FakturDUC)TheVvUC).Fld_SkladCD : null);
         //// ako je da je deletedWasMaxTtNum; tada se upravo 'oslobodio' njegov ttNum za nextTtNum 
         //uint nextTtNum = TheVvDao.GetNextTtNum(TheDbConnection, ((VvDocumLikeRecord)TheVvDataRecord).VirtualTT, eventualSkladCD);
         //bool deletedWasMaxTtNum = (nextTtNum == ((VvDocumLikeRecord)TheVvDataRecord).VirtualTTnum);

         if(/*deletedWasMaxTtNum*/true) // bezuvjetno! 
         {
            FileInfo[] fiArray = ZXC.GetAutoCreated_VvXmlDocumentDR_FileInfoArray_ForThisRecord(TheVvDataRecord, /*nextTtNum*/ ((VvDocumLikeRecord)TheVvDataRecord).VirtualTTnum);

            // Rename files; rename '$_' preffix to '#_' preffix 
            foreach(FileInfo fi in fiArray)
            {
               File.Move(fi.FullName, fi.FullName.Replace(VvDataRecord.Auto_vvXmlDR_preffix, VvDataRecord.Auto_vvXmlDR_preffixInvalidated));
            }

         } // if(deletedWasMaxTtNum) 
      }

      #endregion VvXmlDocumentDR AUTO RECOVERY - OnDelete invalidate '$_'

      // 10.07.2015: 
    //if(OK && ZXC.IsSkyEnvironment) VvDaoBase.SendWriteOperationToSKY(TheDbConnection           , TheVvDataRecord, VvSQL.DB_RW_ActionType.DEL, true);
      if(OK && ZXC.IsSkyEnvironment)
      {
         ZXC.UseSecondDbConnection(
            () => ZXC.TheSecondDbConn_SameDB,
            secondDbConn => VvDaoBase.SendWriteOperationToSKY(secondDbConn, TheVvDataRecord, VvSQL.DB_RW_ActionType.DEL, true));
      }

      if(OK)
      {
         //   
         //                                                                                                                             
         /* */ TheArhivedVvDataRecord = TheVvDataRecord.CreateArhivedDataRecord(TheDbConnection, "BRISANJE");                      //   
         /* */                                                                                                                     //   
         /* */ bool arOK = TheArhivedVvDataRecord.VvDao.ADDREC(TheDbConnection, TheArhivedVvDataRecord, true, true, false, false); //   
         //                                                                                                                             
                                                                                                                                   //   
         // 23.02.2024: 
         #region SynchronizeArtiklSifrar

         if(ZXC.Should_SynchronizeArtiklSifrar && TheVvDataRecord is Artikl) // IsTETRAGRAM_ANY 
         {
            bool syncArtiklOK = ZXC.ArtiklDao.SynchronizeArtiklSifrar(TheDbConnection, ZXC.WriteMode.Delete, TheVvDataRecord as Artikl);

            if(syncArtiklOK) ZXC.aim_emsg(MessageBoxIcon.Information, "Artikl sam izbrisao i u šifrarniku 'partner' firme.");
         }

         // ili ipak za DELREC ne 
       //if(OK && TheVvDataRecord.IsSifrar && ZXC.projectYearFirstDay.Year != DateTime.Now.Year && TheVvDataRecord.IsPrjkt_NonPUG_DataRecord == false) // SynchronizeSifrar PG ---> NY
       //{
       //   bool syncSifrarOK = TheVvDao.SynchronizeVvSifrarRecord_PG_NY(TheDbConnection, ZXC.WriteMode.Delete, TheVvDataRecord as VvSifrarRecord);
       //
       //   if(syncSifrarOK) ZXC.aim_emsg(MessageBoxIcon.Information, "Artikl sam obrisao i u šifrarniku '{0}' godine.", DateTime.Now.Year.ToString());
       //}

         #endregion SynchronizeArtiklSifrar

         TakeNewPositionAndPutFields();

      }
      else // iz nekog razloga nismo nista pobrisali
      {
         WhenRecordInDBHasChangedAction();
      }

   }

   #endregion ADD, EDIT, DELETE

   #region SAVE, ESCAPE, FIND

   //=================================================================
   //=============   SAVE   ==========================================
   //=================================================================


   /* */
   /* SAVE_AS RECORD */
   /* */

   private void SaveRecordAs_OnClick(object sender, EventArgs e)
   {
      TheVvTabPage.WriteMode = ZXC.WriteMode.Add;
      SaveRecord_OnClick(sender, e);
      TheVvRecordUC.tbx_DummyForDefaultFocus.Focus();
   }

   /* */
   /* SAVE RECORD */
   /* */

   public void SaveRecord_OnClick_M2PAY(object sender, EventArgs e)
   {
      SaveRecord_OnClick("F9_M2PAY", e);
   }
   
   public void SaveRecord_OnClick(object sender, EventArgs e)
   {
      Cursor.Current = Cursors.WaitCursor;

      bool OK = SaveVvDataRecord(sender, e);

      Cursor.Current = Cursors.Default;

      #region IRM utilities 

      if(OK && ZXC.RRD.Dsc_IsIrmQuickPrint)
      {
         if(TheVvUC is IRMDUC || TheVvUC is IRMDUC_2)
         {
            ZXC.SetStatusText("IRM Quick Print in progress");
            QuickPrintMenu_Button_OnClick("SaveRecord_OnClick", EventArgs.Empty);
            ZXC.ClearStatusText();
         }
      }

      if(OK && ZXC.RRD.Dsc_IsRetMoneyCalc)
      {
         if(TheVvUC is IRMDUC || TheVvUC is IRMDUC_2)
         {
            FakturExtDUC fakturDUC = TheVvUC as FakturExtDUC;
            if(fakturDUC.Fld_IsNpCash)
            {
               RISK_RetMoneyCalc("SaveRecord_OnClick", EventArgs.Empty);
            }
         }
      }

    //if(OK && ZXC.RRD.Dsc_IsIrmQuickPrint)
    //{
    //   if(TheVvUC is IRMDUC || TheVvUC is IRMDUC_2)
    //   {
    //      ZXC.SetStatusText("IRM Quick Print in progress");
    //      QuickPrintMenu_Button_OnClick("SaveRecord_OnClick", EventArgs.Empty);
    //      ZXC.ClearStatusText();
    //   }
    //}

      if(OK && ZXC.RRD.Dsc_IsIrmQuickPrint)
      {
         if(TheVvUC is IRMDUC || TheVvUC is IRMDUC_2)
         {
            NewRecord_OnClick("SaveRecord_OnClick", EventArgs.Empty);
         }
      }

      #endregion IRM utilities

   }

   private bool SaveVvDataRecord(object sender, EventArgs e)
   {
      TheVvUC.GetFields(false);

      bool OK = true;

      // 21.01.2015: 
      ZXC.RISK_SaveVvDataRecord_inProgress = true;

      // 5.3.2009: 
      //if(this.ValidateChildren() == false) return false;
      //if(this.ValidateChildren(ValidationConstraints.Visible) == false) return false;
      //if(TheVvUC.ValidateChildren() == false) return false;
      // 9.4.2009.: ponovo vratio, jer se ne dize UserControl.Validating kod sejvanja. To be continued... 
      if(this.ValidateChildren() == false) { ZXC.RISK_SaveVvDataRecord_inProgress = false; return false; } // 05.01.2018: dodan 'ZXC.RISK_SaveVvDataRecord_inProgress = false' 

      // 27.11.2015: zbog ubrzanja rada, PutProduktCijenaAndIznos_OR_CheckKOLbalance() se NE odvija u zutome 
      // pa ga treba pozvati na kraju prije sejvanja 
      // 25.02.2016: 
    //bool isSavingProizvodnja = TheVvUC is FakturDUC && ((FakturDUC)(TheVvUC)).faktur_rec.TtInfo.HasSplitTT;
      // 28.04.2016: 
    //if(TheVvUC.NeedsProizvodCIJ                     ) 
      if(TheVvUC.NeedsProizvodCIJ || TheVvUC is RNMDUC) // bio bug kada se sejva RNM i pri tome promjenis proizvod/sirovina checkbox netom prije sejvanja 
      {
         ((FakturDUC)(TheVvUC)).PutProduktCijenaAndIznos_OR_CheckKOLbalance();
      }


      /* */ //new from 04.03.2009: 
      /* */
      /* */
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.Add)
      {
         TheVvDataRecord.Memset0(0); // da obrise i eventualno 'nevidljiva' polja nekom UC-u 

         // from 08.08.2009: ovaj GetFields je povucen u ovaj if, tj samo kada je ADDREC se radi GetFields zbog ovog Memset(0), 
         // odervajs zbog brzine je za ostale slucajeve ovaj finalni GetFields izbacen (prije je isao uvijek.                   

         //TheVvUC.GetFields(false);

         // 09.09.2009: Jos novije!!! trans_rec-ova polja koja ovise o zaglavlju (dokNum, dokDate, ...) ti se 
         // NE refresh-aju! i u DGV-u kod promjene istih na zaglavlju.
         // Pa je ovaj 'FINALNI' i kompletni GetFields ipak obaviti uvijek, te sam ga opet maknuo izvan if-a 
      }

      // 29.04.2016: pokusaj onemogucavanja nastanka nezeljeno 'crvenih' dokumenata
      // ziheraski, jos jemput 
      if(TheVvUC is FakturDUC)
      {
         (TheVvDataRecord as Faktur).TT         = (TheVvUC as FakturDUC).Fld_TT; // za tocan TrnNonDel, jer ga onaj gore Memset0 zhebe  

         if(TheVvUC is FakturExtDUC) 
         {
            (TheVvUC as FakturExtDUC).GetFaktExtFields();
         }

         // dont / may intervent bool se odnosi da li program smije popeglati krivu sumu - ukloniti crvenilo 
         bool dontIntervent = ((TheVvUC is FakturExtDUC) && (TheVvUC as FakturExtDUC).Fld_IsManuallyRed == true);
         bool mayIntervent  = !dontIntervent;

         // 13.03.2023: (kasnije): ipak NE, nego smo ovo rjesili u 'PutTransSumToDocumentSumFields_Ext' 
         ////13.03.2023: gabi th pojava 0,01 razlike pri raspoređivanju ztr-a 
         //if((TheVvDataRecord as Faktur).IsZtrPresent)
         //{
         //   mayIntervent = false; // oj programu, nemoj ti nist intervenirati. pusti da bude kako je neko rucno napisao 
         //}

         if(mayIntervent)
         {
            (TheVvUC as FakturDUC).PutTransSumToDocumentSumFields(); // 19.04.2023. observacija: NE DIRAJ OVO, ne gasi jer treba 
         }

         // 25.11.2016: 
         if((TheVvUC as FakturDUC).faktur_rec.TtInfo.IsBlagajnaTT)
         {
            VvLookUpItem theLui = ZXC.luiListaRiskVrstaPl.FirstOrDefault(lui => lui.Flag == true); // probaj naci prvi lui sa IsNPcash flagom  

            (TheVvUC as FakturDUC).faktur_rec.NacPlac = (theLui == null ? "NPerror" : theLui.Cd);
            (TheVvUC as FakturDUC).faktur_rec.IsNpCash = true;
         }

         // SvDUH UGOvori; Get & Save UGO_Ostvareno_inOLDyears
         // 02.07.2018: gasimo ovo, jer vise ne treba 
       //if(ZXC.IsSvDUH && TheVvUC is UGODUC && (TheVvUC as FakturDUC).Fld_DokDate < ZXC.projectYearFirstDay) // ako je ugovor iz bilo koje prethodne a ne ove godine 
         if(false                                                                                           ) // ako je ugovor iz bilo koje prethodne a ne ove godine 
         {
            decimal UGO_Ostvareno_inOLDyears = 0M;
            string t_artiklCD;

            for(int rowIdx = 0; rowIdx < (TheVvUC as FakturDUC).TheG.RowCount - 1; ++rowIdx)
            {
               t_artiklCD = (TheVvUC as FakturDUC).TheG.GetStringCell((TheVvUC as FakturDUC).DgvCI.iT_artiklCD, rowIdx, false);

               UGO_Ostvareno_inOLDyears += RtransDao.Get_UGO_Ostvareno_For_Artikl_inOLDyears(TheDbConnection, t_artiklCD, (TheVvUC as FakturExtDUC).Fld_KupdobCd, (TheVvUC as FakturDUC).Fld_DokDate, (TheVvUC as FakturExtDUC).Fld_DospDate);
            }

            ZXC.aim_emsg(MessageBoxIcon.Information, "Za period i dobavljača ovog Ugovora sam u prošlim godinama izračunao i u polje 'Ostvareno' upisao\n\n{0} kn.",
               UGO_Ostvareno_inOLDyears.ToStringVv());

            (TheVvUC as FakturExtDUC).Fld_someMoney = UGO_Ostvareno_inOLDyears;
         }

      } // if(TheVvUC is FakturDUC) 

      // 29.12.2025: dodao jos jedan GetFields na vrh, prije validacije ... God help us! 
      TheVvUC.GetFields(false);

      #region On PTG PCK Artikl set New_ArtiklCD_From_PCK_base_RAM_HDD

      if(ZXC.IsPCTOGO && TheVvUC is ArtiklUC) // Sredi PCK ArtiklCD 
      {
         ArtiklUC theVvUC  = TheVvUC as ArtiklUC      ;
         Artikl artikl_rec = TheVvDataRecord as Artikl;

         if(artikl_rec.TS == ZXC.PCK_TS)
         {
            artikl_rec.ArtiklCD   = theVvUC.Fld_ArtiklCd   = artikl_rec.New_ArtiklCD_From_PCK_base_RAM_HDD     ;
            artikl_rec.ArtiklName = theVvUC.Fld_ArtiklName = artikl_rec.New_ArtiklName_From_OldPCK_name_RAM_HDD;
         }
      }

      #endregion On PTG PCK Artikl set New_ArtiklCD_From_PCK_base_RAM_HDD

      // 23.08.2011: 
      if(CheckForMinus(TheVvTabPage.WriteMode)) { ZXC.RISK_SaveVvDataRecord_inProgress = false; return false; } // 05.01.2018: dodan 'ZXC.RISK_SaveVvDataRecord_inProgress = false' 

      #region ADDREC

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.Add) //////////////////////////////////////////////////////////////////////////////////////////////
      {
         #region CheckTtNum_Slijednost

         // 27.12.2012: 
         if(TheVvUC is FakturDUC)
         {
            Faktur faktur_rec = (Faktur)TheVvDataRecord;
          //if(faktur_rec.IsFiskalDutyFaktur)                                                                                           // Od 1.1.2013 & IRM, IRA, IFA, IOD, IPV 
            // 27.08.2015: neka provjerava samo jedamput a ukoliko otkrije problem, javlja stalno                                                                                
          //if(faktur_rec.IsFiskalDutyFaktur && ZXC.CURR_prjkt_rec.IsNoTtNumChk == false                                              ) // Od 1.1.2013 & IRM, IRA, IFA, IOD, IPV 
            if(faktur_rec.IsFiskalDutyFaktur && ZXC.CURR_prjkt_rec.IsNoTtNumChk == false && ZXC.RISK_TtNum_Slijednost_Checked == false) // Od 1.1.2013 & IRM, IRA, IFA, IOD, IPV 
            {
               DateTime dateToCheck = faktur_rec.DokDate;
               bool isDateXDateIzd = ZXC.RRD.Dsc_IsDateXDateIzd == true && faktur_rec.TT != Faktur.TT_IRM;
               if(isDateXDateIzd) dateToCheck = faktur_rec.DateX; // Na IRA, IFA, ... DateX je 'datum izdavanja racuna' 

               ZXC.SetStatusText("Check TtNum Sljednost in progress");

               List<ZXC.VvUtilDataPackage> badList = FakturDao.CheckTtNum_Slijednost(TheDbConnection,
                  faktur_rec.SkladCD,
                  faktur_rec.TT,
                  faktur_rec.TtNum,
                  dateToCheck,
                  /*ZXC.RRD.Dsc_IsDateXDateIzd*/ isDateXDateIzd);

               #region Some TH krpanje 

               // 27.04.2020: TH 36 Zabok u 17:02 vraca sistemsko vrijeme na 16:52 - 10 min unazad ___ START ___ 

               if(ZXC.IsTEXTHOshop && faktur_rec.SkladCD == "36M2" && dateToCheck.Date == ZXC.Date27042020)
               {
                  badList.RemoveAll(udp => udp.TheDate == ZXC.Date27042020);
               }

               // 27.04.2020: TH 36 Zabok u 17:02 vraca sistemsko vrijeme na 16:52 - 10 min unazad ___  END  ___ 

               #endregion Some TH krpanje 

               int count = 0, maxCount = 10;
               if(badList.Count.NotZero()) // znaci, IMA problema 
               {
                  ZXC.RISK_TtNum_Slijednost_Checked = false; // ako se otkrije nekonzist. neka i dalje provjerava i javlja. 

                  string errMessage = "";

                  foreach(ZXC.VvUtilDataPackage udp in badList)
                  {
                     if(++count <= maxCount)
                     {
                        errMessage += "Rn [" + udp.TheDate.ToString(ZXC.VvDateAndTimeFormat) + "] broj [" + udp.TheUint + "] treba biti [" + udp.TheInt + "]\n";
                     }
                     else
                     {
                        errMessage += "\n... i još [" + (badList.Count - count + 1).ToString() + "] računa ...";
                        break;
                     }
                  }

                  //ZXC.aim_emsg(MessageBoxIcon.Error, errMessage);
                     MessageBox.Show(errMessage, "NEKONZISTENTNOST SLIJEDA REDNIH BROJEVA DOKUMENTA!", MessageBoxButtons.OK, MessageBoxIcon.Error);
               } // if(badList.Count.NotZero()) // znaci, IMA problema 
               else // znaci, NEMA problema 
               {
                  ZXC.RISK_TtNum_Slijednost_Checked = true;
               }

               ZXC.ClearStatusText();
            }
         
         }

         #endregion CheckTtNum_Slijednost

         // ====== User 4 MySQL additions START =============================


         // 10.10.2022: SKIP on SVDZAH ODOBRAVATELJ 
       //if(TheVvDataRecord.VirtualRecordName == User.recordName)
         if(TheVvDataRecord.VirtualRecordName == User.recordName && !ZXC.IsSVD_ZAH_ODOBRAVATELJ_UserName((TheVvDataRecord as User).UserName))
         {
            OK = VvSQL.CreateUser(TheDbConnection, TheVvDataRecord as User);
         }
         // ====== User 4 MySQL additions END   =============================

         if(TheVvDataRecord.IsDocumentLike)
         {
            if(HasNotEnoughPrivilegesForThisAction(ZXC.PrivilegedAction.ADDREC, TheVvUC.TheSubModul, ((VvDocumLikeRecord)TheVvDataRecord).VirtualTT, true))
            {
               { ZXC.RISK_SaveVvDataRecord_inProgress = false; return false; } // 05.01.2018: dodan 'ZXC.RISK_SaveVvDataRecord_inProgress = false' 
            }
         }

         #region M2PAY

         bool is_F9_M2PAYsaveKartica = sender is string && sender.ToString() == "F9_M2PAY";
         
         bool m2pAuthorizationNeeded = false;
         
         Faktur m2p_faktur_rec = null;

         if(TheVvDataRecord is Faktur)
         {
            m2p_faktur_rec = TheVvDataRecord as Faktur;

            m2pAuthorizationNeeded = Is_M2P_AuthorizationNeeded(m2p_faktur_rec);
         }

         if(m2pAuthorizationNeeded && !is_F9_M2PAYsaveKartica)
         {
            DialogResult result = MessageBox.Show("Da li zaista želite spremiti ovaj račun BEZ AUTORIZACIJE?!",
               "KARTIČNO PLAĆANJE BEZ AUTORIZACIJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if(result != DialogResult.Yes)
            {
               ZXC.RISK_SaveVvDataRecord_inProgress = false;
               return false;
            }
         }

       //if(m2pAuthorizationNeeded)
       //if(m2pAuthorizationNeeded || is_F9_M2PAYsaveKartica)
         if(TheVvUC.IsM2PAY_DUC &&    is_F9_M2PAYsaveKartica) // Najnovija odluka; u autorizacijuIterecenje krecemo samo po eksplicitnom odabiru 'AutorizKart i Spremi'
         {
            //M2PAY_DirectConnect(false);

            if(ZXC.M2PAY_Device_Connected == false) 
            { 
               ZXC.aim_emsg(MessageBoxIcon.Stop, "M2PAY uređaj nije spojen!"); 
               ZXC.RISK_SaveVvDataRecord_inProgress = false;
               return false; 
            }

            M2P_Statusdlg = new VvM2PayStatusDlg();
            M2P_Statusdlg.lbl_m2PayStatus.Text = "Start autorizacije ...";

            ZXC.M2PAY_AuthorizationStatus = com.handpoint.api.FinancialStatus.UNDEFINED;

            int moneyAsInteger = (int)(m2p_faktur_rec.S_ukKCRP * 100);

            OperationStartResult result;

            if(m2p_faktur_rec.Is_STORNO)
            {
               TransactionResult transactionResult = M2P_GetLast_TransactionResultFrom_Xtrano(TheDbConnection, ZXC.FakturRec.TtNum);

               if(transactionResult == null)
               {
                  ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu isčitati M2P TransactionResult");
                  ZXC.RISK_SaveVvDataRecord_inProgress = false;
                  return false;
               }
             
               M2P_Statusdlg = new VvM2PayStatusDlg();
               M2P_Statusdlg.lbl_m2PayStatus.Text = "Start POVRATA sredstava na karticu po plaćanja ovog računa...";

               bool isSaleReversal = m2p_faktur_rec.DokDate.Date == DateTime.Today.Date;
               bool isRefund       = !isSaleReversal;

               if(isRefund) result = TheHapi.Refund      (transactionResult.TotalAmount, transactionResult.Currency, transactionResult.TransactionId); // Voila! ####################################################################################################### 
               else         result = TheHapi.SaleReversal(transactionResult.TotalAmount, transactionResult.Currency, transactionResult.TransactionId); // Voila! ####################################################################################################### 
            }
            else
            {
               result = TheHapi.Sale(new BigInteger(moneyAsInteger /*1000*/ /*37.84*/ ), Currency.EUR); // Voila! ####################################################################################################### 
            }

            #region Autorizacija i TEREĆENJE je otkazano ili odbijeno 

            if(result.OperationStarted == true)                             
            { 
               M2P_Statusdlg.ShowDialog();                                                                            
            }
            else                                                            
            { 
               ZXC.aim_emsg(MessageBoxIcon.Error, "Start autorizacije nije uspio. Pokušajte ponovno."); 
               ZXC.RISK_SaveVvDataRecord_inProgress = false;
               return false; 
            }

            if(ZXC.M2PAY_AuthorizationStatus != FinancialStatus.AUTHORISED) 
            { 
               ZXC.aim_emsg(MessageBoxIcon.Error, "AUTORIZACIJA NIJE USPJELA!.");
               ZXC.RISK_SaveVvDataRecord_inProgress = false;
               return false; 
            }

            #endregion Autorizacija i TEREĆENJE je otkazano ili odbijeno 

            FakturExtDUC theDUC = (TheVvUC as FakturExtDUC);

            string vvNacPlac_from_M2PnacPlac = "NEDEFINIRANO!";

            if(theDUC.M2P_TransactionResult != null) vvNacPlac_from_M2PnacPlac = Get_vvNacPlac_from_M2PnacPlac(theDUC.M2P_TransactionResult.CardSchemeName); // SAMO ako je TEXTHO, ostalima ostavljamo kak je m2p vratio                        

            if(vvNacPlac_from_M2PnacPlac.NotEmpty())
            {
               m2p_faktur_rec.NacPlac  = theDUC.Fld_NacPlac  = vvNacPlac_from_M2PnacPlac; // business i fld 
               m2p_faktur_rec.IsNpCash = theDUC.Fld_IsNpCash = false                    ; // business i fld 

               if(ZXC.luiListaRiskVrstaPl.Select(lui => lui.Cd).Contains(vvNacPlac_from_M2PnacPlac) == false)
               {
                  ADD_NEW_NacPlac_FromM2PAY(vvNacPlac_from_M2PnacPlac);
               }
            }

         }

         #endregion M2PAY

         // the ADDREC is here _________________________________________________________________________________________________________________ 
         if(OK)
         {
            //// 14.02.2019: VvXmlDR RECOVERY + VvXmlDocumentDR AUTO RECOVERY 
            //TheVvDataRecord.SaveSerialized_VvDataRecord_ToXmlFile_AUTOMATICALLY();

            //ZXC.SetStatusText("TheVvDao.ADDREC");
            OK = TheVvDao.ADDREC(TheDbConnection, TheVvDataRecord); // VOILA ################################################################### 
                                                                    //ZXC.ClearStatusText();

         }

         // 23.02.2024: 
         #region SynchronizeArtiklSifrar

         if(OK && ZXC.Should_SynchronizeArtiklSifrar && TheVvDataRecord is Artikl) // IsTETRAGRAM_T1_OR_T2 
         {
            bool syncArtiklOK = ZXC.ArtiklDao.SynchronizeArtiklSifrar(TheDbConnection, ZXC.WriteMode.Add, TheVvDataRecord as Artikl);

            if(syncArtiklOK) ZXC.aim_emsg(MessageBoxIcon.Information, "Artikl sam dodao i u šifrarniku 'partner' firme.");
         }

         // 17.03.2026: 
       //if(OK && ZXC.IsSifrar_And_WeAreInPGyear(TheVvDataRecord)                                ) // SynchronizeSifrar PG ---> NY
         if(OK && ZXC.IsSifrar_And_WeAreInPGyear(TheVvDataRecord) && !(TheVvDataRecord is Osred) ) // SynchronizeSifrar PG ---> NY
         {
            bool syncSifrarOK = TheVvDao.SynchronizeVvSifrarRecord_PG_NY(TheDbConnection, ZXC.WriteMode.Add, TheVvDataRecord as VvSifrarRecord);

            if(syncSifrarOK) ZXC.aim_emsg(MessageBoxIcon.Information, "Šifrar sam dodao i u šifrarnik '{0}' godine.", DateTime.Now.Year.ToString());
         }

         #endregion SynchronizeArtiklSifrar

         if(OK && TheVvTabPage.FileIsEmpty == true) TheVvTabPage.FileIsEmpty = false;

         if(OK && ZXC.VvXmlDR_LastDocumentMissing_AlertRaised == true) ZXC.VvXmlDR_LastDocumentMissing_AlertRaised = false;

         if(OK) CloseForWriteActions(false);

         if(!OK && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && TheVvDataRecord.IsDocumentLike) // 1062! 
         //Error: 1062 SQLSTATE: 23000 (ER_DUP_ENTRY) 
         //Message: Duplicate entry '%s' for key %d
         {
            AskForAndObtainNewDokNumAndDocumTT();
         }

         #region M2PAY ... After ADDREC Faktur, refresh & RWTREC m2pResultXtrano_rec

         bool shouldRWTREC_M2Pxtrano = is_F9_M2PAYsaveKartica ||
                                       (TheVvUC is FakturExtDUC && (TheVvUC as FakturExtDUC).M2P_TransactionResult != null);

         if(shouldRWTREC_M2Pxtrano)
         {
            Faktur faktur_rec = TheVvDataRecord as Faktur;

            FakturExtDUC theDUC = TheVvUC as FakturExtDUC;

            List<Xtrano> M2Pay_TransactionResult_xtranoList = M2P_GetTransactionResultListFrom_Xtrano(TheDbConnection, theDUC.M2P_Xtrano_Current_TtNum);

            M2Pay_TransactionResult_xtranoList.RemoveAll(rto => rto.T_parentID.NotZero()); // makni eventualne prethodno validirano spojene xtrano-e s faktur-om 

            foreach(Xtrano initialXtrano_rec in M2Pay_TransactionResult_xtranoList)
            {
               initialXtrano_rec.BeginEdit();

               initialXtrano_rec.T_ttNum    = faktur_rec.TtNum;
               initialXtrano_rec.T_parentID = faktur_rec.RecID;

               if(initialXtrano_rec != null)
               {
                  ZXC.XtranoDao.RWTREC(TheDbConnection, initialXtrano_rec);
               }

               initialXtrano_rec.EndEdit();

               theDUC.M2P_Xtrano_Current_TtNum = faktur_rec.TtNum;
            }
         }

         #endregion M2PAY ... After ADDREC Faktur, refresh & RWTREC m2pResultXtrano_rec

         #region RISK_CopyToSomeOtherTT_inProgress

         if(OK && ZXC.RISK_CopyToOtherDUC_inProgress)
         {
            if(ZXC.MIXER_CopyToOtherDUC_inProgress) // kopirali smo Mixer 2 Risk 
            {
               RwtrecLinkFeedback_MIXER();
            }
            else
            {
               if(ZXC.IsSkyEnvironment == false) RwtrecLinkFeedback_FAKTUR();

               // 01.09.2022: RISK_CopySVD_ZAH_to_IZD feedback (eventualno zatvaranje ZAH-a) 
               if(ZXC.IsSvDUH && TheVvUC is IZD_SVD_DUC)
               {
                  FakturDUC theDUC = TheVvUC as IZD_SVD_DUC;
                  Faktur IZDfaktur_rec = theDUC.faktur_rec;
                  if(IZDfaktur_rec.V1_tt == Faktur.TT_ZAH) // ovo je malo mrsavi kriterij
                  {

                     #region Rwtrec Feedback 

                     Faktur ZAHfaktur_rec = new Faktur();

                     bool ZAHfaktur_recFound = FakturDao.SetMeFaktur(TheDbConnection, ZAHfaktur_rec, IZDfaktur_rec.V1_tt, IZDfaktur_rec.V1_ttNum, false);

                     if(ZAHfaktur_recFound)
                     {
                        ZAHfaktur_rec.VvDao.LoadTranses(TheDbConnection, ZAHfaktur_rec, false);

                        bool everyZAHrtrans_has_IZDrtrans = Get_has_everyZAHrtrans_IZDrtrans(ZAHfaktur_rec);

                        if(everyZAHrtrans_has_IZDrtrans)
                        {
                           BeginEdit(ZAHfaktur_rec);

                           ZAHfaktur_rec.StatusCD = "D";

                           bool rwtOK = ZAHfaktur_rec.VvDao.RWTREC(TheDbConnection, ZAHfaktur_rec, false, true, false, false, true/* !!! */ ); // zbog implicitne synchronizacije ovaj rwtrec ne zelimo u Insert_LAN_LogEntry 

                           EndEdit(ZAHfaktur_rec);

                           //if(rwtOK) WhenRecordInDBHasChangedAction(); // RRDREC 

                        }

                     } // if(ZAHfaktur_recFound)

                     #endregion Rwtrec Feedback 

                  }
               }
            }

            EndLinkToOtherDocumActions();
         }

         // bikoz of some other uses... 
         ZXC.LoadPoprat_InProgress = false;

         #endregion RISK_CopyToSomeOtherTT_inProgress

         #region FISKALIZE

         if(OK && TheVvRecordUC is FakturExtDUC)
         {
            Faktur faktur_rec = (Faktur)(TheVvDataRecord);

            // 26.01.2026: 
          //if(                   Faktur.IsFiskalDutyTT_ONLINE(faktur_rec.TT, faktur_rec.NacPlac, faktur_rec.NacPlac2)/*&& ZXC.Vv_ThisClientHasFiskalConnection*/ && ZXC.CURR_prjkt_rec.IsNoAutoFiskal == false) // ipak bez Vv_ThisClientHasFiskalConnection                                  )
            if(faktur_rec.IsF1 && Faktur.IsFiskalDutyTT_ONLINE(faktur_rec.TT, faktur_rec.NacPlac, faktur_rec.NacPlac2)/*&& ZXC.Vv_ThisClientHasFiskalConnection*/ && ZXC.CURR_prjkt_rec.IsNoAutoFiskal == false) // ipak bez Vv_ThisClientHasFiskalConnection                                  )
            {
               ZXC.SetStatusText("FISKALIZIRAM");

               // 19.02.2019: dodan if() jer FiskJIR moze vec postojati ako smo tu dosli iz auto recovery-a recorda koji je vec fiskaliziran (a koji moze i ne mora imati JIR!) 
             //                                 RISK_Fiskalize_RACUN(false, e); // Ovaj, dakle, sadrzi RWTREC! 
               if(faktur_rec.FiskJIR.IsEmpty()) RISK_Fiskalize_RACUN(false, e); // Ovaj, dakle, sadrzi RWTREC! 

               ZXC.ClearStatusText();
            }
         }

         #endregion FISKALIZE

         // 19.02.2019: 
       //if(OK && ZXC.IsSkyEnvironment )             VvDaoBase.SendWriteOperationToSKY(ZXC.TheSecondDbConn_SameDB, TheVvDataRecord, VvSQL.DB_RW_ActionType.ADD, true);
         if(OK && ZXC.IsSkyEnvironment )
         {  // NE sinhroniziraj ako je VvXmlDRfilesAlertRaised i daj ga MarkAsSENDed 
            if(ZXC.VvXmlDR_LastDocumentMissing_AlertRaised == false)
            {
               ZXC.UseSecondDbConnection(
                  () => ZXC.TheSecondDbConn_SameDB,
                  secondDbConn => VvDaoBase.SendWriteOperationToSKY(secondDbConn, TheVvDataRecord, VvSQL.DB_RW_ActionType.ADD, true)); // normal defautl 

               // 13.02.2024: neki je BUG kada je ADDREC Faktur bussinessa gdje ima ZTR-a i SendWriteOperationToSKY 
               if(TheVvDataRecord is Faktur && (TheVvDataRecord as Faktur).IsZtrPresent) // zbog drkanja po faktur biznisu zbog ZTR-a nekaj se zjebe pa treba refresh-ati record 
               { 
                  WhenRecordInDBHasChangedAction();
               }

            }
            else
            {
               MarkADDactionAs_SENDed_ToSKY(null, EventArgs.Empty); // disable SEND2SKY, a ako treba iniciraj rucno preko 'Un_MarkADDactionAs_SENDed_ToSKY()' 
            }
         }

         #region Cash fakturs to blagajna

         bool isCashFakturToBlagajna = ZXC.RRD.Dsc_IsCashFakturToBlagajna && TheVvDataRecord is Faktur && (TheVvDataRecord as Faktur).TtInfo.IsCash2BlagajnaTT && (TheVvDataRecord as Faktur).R_IsNpCashAny;

         if(OK && isCashFakturToBlagajna)
         {
            //Faktur cashFaktur_rec = TheVvDataRecord as Faktur;

            //ZXC.VvSubModulEnum targetSubModulEnum = cashFaktur_rec.TtInfo.IsFinKol_U ? ZXC.VvSubModulEnum./*R_BIS*/R_ABI : ZXC.VvSubModulEnum./*R_BUP*/R_ABU;

            //RISK_CopyToSomeOtherTT(targetSubModulEnum, /*new NewRecordEventArgs(ZXC.FakturRec, (RNMDUC)TheVvUC, true)*/ EventArgs.Empty);

            (string ABU_ABI_tt, uint ABU_ABI_ttNum) = AddCashFakturToBlagajna((Faktur)TheVvDataRecord.CreateNewRecordAndCloneItComplete());

            #region Rwtrec Feedback CashFakturu

            Faktur cashFaktur_rec = TheVvDataRecord as Faktur;

            BeginEdit(cashFaktur_rec);

            string v1_tt    = ABU_ABI_tt   ;
            uint   v1_ttNum = ABU_ABI_ttNum;

            switch(cashFaktur_rec.FirstEmptyVezaLine)
            {
               case 1: cashFaktur_rec.V1_tt    = v1_tt;
                       cashFaktur_rec.V1_ttNum = v1_ttNum;
                       break;
               case 2: cashFaktur_rec.V2_tt    = v1_tt;
                       cashFaktur_rec.V2_ttNum = v1_ttNum;
                       break;
               case 3: cashFaktur_rec.V3_tt    = v1_tt;
                       cashFaktur_rec.V3_ttNum = v1_ttNum;
                       break;
               case 4: cashFaktur_rec.V4_tt    = v1_tt;
                       cashFaktur_rec.V4_ttNum = v1_ttNum;
                       break;

               default: break;
            }

            cashFaktur_rec.VvDao.RWTREC(TheDbConnection, cashFaktur_rec, false, true, false);

            EndEdit(cashFaktur_rec);

            PutFieldsActions(TheDbConnection, cashFaktur_rec, TheVvRecordUC);

            #endregion Rwtrec Feedback CashFakturu

         }

         #endregion Cash fakturs to blagajna

      } // if(ZXC.WriteMode.Add) 

      #endregion ADDREC

      #region RWTREC

      else if(TheVvTabPage.WriteMode == ZXC.WriteMode.Edit) //////////////////////////////////////////////////////////////////////////////////////////////
      {
         VvSQL.VvLockerInfo freshFromFile_lockerInfo = new VvSQL.VvLockerInfo(TheVvDataRecord.VirtualRecordName, TheVvDataRecord.VirtualRecID);

         if(TheVvDao.IsInLocker(TheDbConnection, freshFromFile_lockerInfo, true) && // Lock exists AND Lock is mine! 
            TheVvLockerInfo.lockID == freshFromFile_lockerInfo.lockID)
         {
            // ====== Check for CROSS-TABLE integrity violation START ====================
            if(TheVvDataRecord.IsSomeOfPossibleForeignKeyFieldsChanged)
            {
               if(TheVvDao.IsThisRecordInSomeRelation(ZXC.PrivilegedAction.RWTREC, TheDbConnection, TheVvDataRecord)) { ZXC.RISK_SaveVvDataRecord_inProgress = false; return false; } // 05.01.2018: dodan 'ZXC.RISK_SaveVvDataRecord_inProgress = false' 
            }
            // ====== Check for CROSS-TABLE integrity violation END ======================

            // ====== User 4 MySQL additions START =============================

            // 10.10.2022: SKIP on SVDZAH ODOBRAVATELJ 
          //if(TheVvDataRecord.VirtualRecordName == User.recordName)
            if(TheVvDataRecord.VirtualRecordName == User.recordName && !ZXC.IsSVD_ZAH_ODOBRAVATELJ_UserName((TheVvDataRecord as User).UserName))
            {
               OK = VvSQL.RenameUserAndSetPassword(TheDbConnection, TheVvDataRecord as User);
            }
            // ====== User 4 MySQL additions END   =============================

            if(TheVvDataRecord.IsDocumentLike)
            {
               if(HasNotEnoughPrivilegesForThisAction(ZXC.PrivilegedAction.RWTREC, TheVvUC.TheSubModul, ((VvDocumLikeRecord)TheVvDataRecord).VirtualTT, true))
               {
                  { ZXC.RISK_SaveVvDataRecord_inProgress = false; return false; } // 05.01.2018: dodan 'ZXC.RISK_SaveVvDataRecord_inProgress = false' 
               }
            }

            #region Is any of SHOULD NOT CHANGE field changed (npr. za Fiskalizirani racun)
            // 21.01.2013:
            if(TheVvRecordUC.IsSomeCrutialFieldIrregularyChanged())
            {
               { ZXC.RISK_SaveVvDataRecord_inProgress = false; return false; } // 05.01.2018: dodan 'ZXC.RISK_SaveVvDataRecord_inProgress = false' 
            }

            #endregion Is any of SHOULD NOT CHANGE field changed (npr. za Fiskalizirani racun)

            if(OK)
            {
               OK = TheVvDao.RWTREC(TheDbConnection, TheVvDataRecord);

               // 23.02.2024: 
               #region SynchronizeArtiklSifrar

               if(OK && ZXC.Should_SynchronizeArtiklSifrar && TheVvDataRecord is Artikl) // IsTETRAGRAM_ANY 
               {
                  bool syncArtiklOK = ZXC.ArtiklDao.SynchronizeArtiklSifrar(TheDbConnection, ZXC.WriteMode.Edit, TheVvDataRecord as Artikl);

                  if(syncArtiklOK) ZXC.aim_emsg(MessageBoxIcon.Information, "Artikl sam ispravio i u šifrarniku 'partner' firme.");
               }

               if(OK && ZXC.IsSifrar_And_WeAreInPGyear(TheVvDataRecord)) // SynchronizeSifrar PG ---> NY
               {
                  bool syncSifrarOK = TheVvDao.SynchronizeVvSifrarRecord_PG_NY(TheDbConnection, ZXC.WriteMode.Edit, TheVvDataRecord as VvSifrarRecord);

                  if(syncSifrarOK) ZXC.aim_emsg(MessageBoxIcon.Information, "Šifrar sam ispravio i u šifrarniku '{0}' godine.", DateTime.Now.Year.ToString());
               }

               #endregion SynchronizeArtiklSifrar

               #region VvXmlDocumentDR AUTO RECOVERY - OnTtNumChanged rename filenames

               // 11.04.2019: 
               if(OK && TheVvUC is VvDocumentRecordUC)
               {
                  uint OLD_ttNum = ((VvDocumLikeRecord)TheVvDataRecord).VirtualTTnum_Bkp;
                  uint NEW_ttNum = ((VvDocumLikeRecord)TheVvDataRecord).VirtualTTnum    ;

                  bool ttNumChanged = OLD_ttNum != NEW_ttNum;

                  if(ttNumChanged) 
                  {
                     FileInfo[] fiArray = ZXC.GetAutoCreated_VvXmlDocumentDR_FileInfoArray_ForThisRecord(TheVvDataRecord, OLD_ttNum /*((VvDocumLikeRecord)TheVvDataRecord).VirtualTTnum*/);

                     // Rename files; rename OLD_ttNum to NEW_ttNum 
                     foreach(FileInfo fi in fiArray)
                     {
                        // 27.05.2025: 
                      //File.Move(fi.FullName, fi.FullName.Replace(      OLD_ttNum.ToString(),             NEW_ttNum.ToString()      ));
                        File.Move(fi.FullName, fi.FullName.Replace("_" + OLD_ttNum.ToString() + "_", "_" + NEW_ttNum.ToString() + "_"));
                     }

                  } // if(deletedWasMaxTtNum) 
               }

               #endregion VvXmlDocumentDR AUTO RECOVERY - OnTtNumChanged rename filenames


               // 10.07.2015: 
             //if(OK && ZXC.IsSkyEnvironment) VvDaoBase.SendWriteOperationToSKY(TheDbConnection           , TheVvDataRecord, VvSQL.DB_RW_ActionType.RWT, true);
               if(OK && ZXC.IsSkyEnvironment)
               {
                  ZXC.UseSecondDbConnection(
                     () => ZXC.TheSecondDbConn_SameDB,
                     secondDbConn => VvDaoBase.SendWriteOperationToSKY(secondDbConn, TheVvDataRecord, VvSQL.DB_RW_ActionType.RWT, true));
               }
            }

            if(OK)
            {
               bool isKOParhiva = (TheArhivedVvDataRecord is Mixer) && (TheArhivedVvDataRecord as Mixer).TT == Mixer.TT_KOP; bool isNotKOParhiva = !isKOParhiva;
               //   
               //                                                                                                                                     
               // NOTA BENE! ostavi ADDREC paramtere true, true, false, false. ShouldRefrweshData MORA BITI true.                                     
               //                                                                                                                                     
               // 10.11.2021: postavljen if() da ne arhivira Mixer.TT_KOP                                                                             
               /* */ bool arOK; if(isNotKOParhiva) arOK = TheArhivedVvDataRecord.VvDao.ADDREC(TheDbConnection, TheArhivedVvDataRecord, true, true, false, false); /* */   
               //                                                                                                                                     
               //                                                                                                                                     
               //   

               // 2.2.2011: 
               OK = TheVvDao.RwtrecAditionalAction(TheDbConnection, TheVvDataRecord);
               if(OK) WhenRecordInDBHasChangedAction(); // refresh record view 

               CloseForWriteActions(false);

               OK = TheVvDao.DeleteFromLocker(TheDbConnection, new VvSQL.VvLockerInfo(TheVvDataRecord.VirtualRecordName, TheVvDataRecord.VirtualRecID));

               if(!OK) ZXC.aim_emsg("Ne mogu odlockirati zapis!");

               // 06.02.2013: if(Prjkt) refresh ZXC.curr_prjkt_rec 
               if(TheVvDataRecord.VirtualRecordName == Prjkt.recordName)
               {
                  ZXC.CURR_prjkt_rec = ((Prjkt)TheVvDataRecord).MakeDeepCopy();
               }

               // 06.01.2015: if(SkyRule) refresh ZXC.curr_prjkt_rec 
               if(ZXC.IsSkyEnvironment && TheVvDataRecord.VirtualRecordName == SkyRule.recordName)
               {
                  ZXC.SkyRuleDao.Load_SkyRules(ZXC.PrjConnection);
               }

            }
         }
         else // Lock doesn't exists OR Lock isn't mine anymore! 
         {
            ZXC.aim_emsg("Nakon isteka vremena Vaše rezervacije ovog zapisa ({0} min), zapis je izmijenjen ili pobrisan od strane nekog drugog korisnika.\nVaše izmjene neæe biti usnimljene!", ZXC.vvLockTimeoutSeconds / 60);

            CloseForWriteActions(false);

            WhenRecordInDBHasChangedAction();
         }

         if(TheVvDataRecord is Prjkt) SetMe_CURR_prjkt_rec(ZXC.TheVvDatabaseInfoIn_ComboBox4Projects);

         // 2026: 
         if(TheVvDataRecord is User) TheVvDao.SetMe_Record_byRecID(TheDbConnection, ZXC.CURR_user_rec, TheVvDataRecord.VirtualRecID, false, false);

         #region treba li DELETE zaglavlja KOP-a

         // 10.11.2021: treba li DELETE zaglavlja KOP-a: 

         bool isKOP = (TheVvDataRecord is Mixer) && (TheVvDataRecord as Mixer).TT == Mixer.TT_KOP; bool isNotKOP = !isKOP;

         if(isKOP)
         {
            bool should_DELETE_KOP = (TheVvDataRecord as Mixer).Transes.Any(x => x.T_isXxx) == false; // nije ostao nijedan KOP data layer 

            if(should_DELETE_KOP)
            {
               (TheVvDataRecord as Mixer).Transes.Clear();

               OK = TheVvDao.DELREC(TheDbConnection, TheVvDataRecord, false);

               if(!OK) ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu obrisati KOP\n\r", TheVvDataRecord);
               else    TakeNewPositionAndPutFields();
            }
         }

         #endregion treba li DELETE zaglavlja KOP-a

         #region CheckCache new in 2023

         // 18.05.2023: start ... ON RWTREC TWIN rtransa MSU komponenta ne dobije cache od prve, pa ide ovo debilno rjesenje ... nis etdismoment dovoljno pametan za bolje :-( 

         if((TheVvDataRecord is Faktur) && (TheVvDataRecord as Faktur).TtInfo.HasTwinTT)
         {
            ArtiklDao.CheckCache(TheDbConnection, true);
         }
         
         #endregion CheckCache new in 2023

      } // else if(TheVvTabPage.WriteMode == ZXC.WriteMode.Edit)

      #endregion RWTREC

      #region OPN - CLOSE Obvezujuca Ponuda Kol2
      // 11.03.2016: 

    //if(TheVvUC is IRADUC || TheVvUC is IzdatnicaDUC || TheVvUC is IRA_MPC_DUC)
      if(TheVvUC is FakturDUC && (TheVvUC as FakturDUC).CouldClose_OPN)
      {
         uint opnTtNum;

         if((opnTtNum = ((FakturDUC)TheVvUC).VezaTtNumForTT(Faktur.TT_OPN)).NotZero())
         {
            DialogResult result = MessageBox.Show("Da li želite zatvoriti obvezujuću ponudu\n\n(osloboditi rezervaciju) po dokumentu\n\nOPN - " + opnTtNum.ToString(),
               "Potvrdite OPN zatvaranje?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(result == DialogResult.Yes) // here we go! 
            {
               Faktur opnFaktur_rec = new Faktur();

               bool found = FakturDao.SetMeFaktur(TheDbConnection, opnFaktur_rec, Faktur.TT_OPN, opnTtNum, true);

               if(found)
               {
                  decimal prevT_kol2;
                  decimal ovaIsporuka;
                  VvDataRecord arhivedOpnFaktur_rec;
                  Rtrans opnRtrans;
                  bool touched = false;
                  string message = "Zatvorene količine obvezujuće ponue:\n\n";

                  opnFaktur_rec.VvDao.LoadTranses(TheDbConnection, opnFaktur_rec, false);

                  arhivedOpnFaktur_rec = opnFaktur_rec.CreateArhivedDataRecord(TheDbConnection, "Save IRA");

                  BeginEdit(opnFaktur_rec);

                  foreach(Rtrans iraRtrans_rec in ((FakturDUC)TheVvUC).faktur_rec.TrnNonDel)
                  {
                     opnRtrans = opnFaktur_rec.Transes.FirstOrDefault(rtr => rtr.T_artiklCD == iraRtrans_rec.T_artiklCD);

                     if(opnRtrans != null)
                     {
                        touched = true;

                        prevT_kol2 = opnRtrans.T_kol2;

                                     /* neisporuceno */            /* ova isporuka */   /* isporuceno */
                        if((opnRtrans.T_kol - opnRtrans.T_kol2) >= iraRtrans_rec.T_kol) ovaIsporuka = iraRtrans_rec.T_kol;
                        else                                                            ovaIsporuka = (opnRtrans.T_kol - opnRtrans.T_kol2);

                        opnRtrans.T_kol2 += ovaIsporuka;

                        opnRtrans.SaveTransesWriteMode = ZXC.WriteMode.Edit; // ! 

                        message += string.Format("[{0}] [{1}]\nponuda: {2} preth. ispor.: {3} ova ispor.: {4} neispor.: {5}\n\n", 
                           opnRtrans.T_artiklCD, opnRtrans.T_bak_recID,
                           opnRtrans.T_kol.ToStringVv(), prevT_kol2.ToStringVv(), ovaIsporuka.ToStringVv(), (opnRtrans.T_kol - opnRtrans.T_kol2).ToStringVv());
                     }
                     
                  } // foreach(Rtrans iraRtrans_rec in ((FakturDUC)TheVvUC).faktur_rec.TrnNonDel)

                  if(touched && OK)
                  {
                     OK = opnFaktur_rec.VvDao.RWTREC(TheDbConnection, opnFaktur_rec);

                     if(OK) arhivedOpnFaktur_rec.VvDao.ADDREC(TheDbConnection, arhivedOpnFaktur_rec, true, true, false, false);

                     ZXC.aim_emsg(MessageBoxIcon.Information, message);
                  }

                  EndEdit(opnFaktur_rec);

               } // if(found)

               else
               {
                  ZXC.aim_emsg(MessageBoxIcon.Error, "Nema dokumenta OPN - {0}!\n\nNema se što zatvoriti.", opnTtNum.ToString());
               }
            }

         } // if((opnTtNum = ((FakturDUC)TheVvUC).VezaTtNumForTT(Faktur.TT_OPN)).NotZero()) 

      } // if(TheVvUC is IRADUC || TheVvUC is IzdatnicaDUC) 

      #endregion OPN - CLOSE Obvezujuca Ponuda Kol2

      // 02.10.2024: ovo ugasio a implementirao VvDaoBase.SynchronizeVvSifrarRecord_PG_NY 

      //// 09.01.2017: upozori ako unosi sifrar u stare godine 
      //if(TheVvDataRecord.IsSifrar && ZXC.projectYearFirstDay.Year != DateTime.Now.Year)
      //{
      //   ZXC.aim_emsg(MessageBoxIcon.Warning, "Unosite element šifrara u podatke godine koja NIJE 'tekuća'.\n\nUkoliko je 'Nova Godina' već otvorena,\n\nmorat ćete unos ponoviti i u novoj godini!");
      //}

      ZXC.RISK_SaveVvDataRecord_inProgress = false;

      if(ZXC.RtransDao != null) ZXC.RtransDao.ResetRecIDinfoList();  // vvDelf 17.06.2015

      // 27.11.2015: 
      if(TheVvUC.NeedsProizvodCIJ)
      {
         ReRead_OnClick(null, EventArgs.Empty);
      }

      if(OK && New_FromVvXmlDR_InProgress == true) { New_FromVvXmlDR_InProgress = false; }

      #region PTG Additions

      if(ZXC.KOPfromFUG_InProgress)  { CloseKOP_ShowFUGandIzlistaj(); }
      if(ZXC.KOPfromUGAN_InProgress) { CloseKOP_JOB(); }

      if(TheVvUC is MOD_PTG_DUC) // Prenesi Rtrano i na Rtrans 
      {
         // ovo da ili ne? 14.02.2025:
         //WhenRecordInDBHasChangedAction(); 

         (TheVvUC as MOD_PTG_DUC).SintRtranoToRtransOnMOD(this);
      }

      if(TheVvUC is PVR_PTG_DUC) // Prenesi Rtrano i na Rtrans 
      {
         (TheVvUC as PVR_PTG_DUC).SintRtranoToRtransOnPVR(this);
      }

      if(TheVvUC is ZIZ_PTG_DUC) // Prenesi Rtrano i na Rtrans 
      {
         (TheVvUC as ZIZ_PTG_DUC).SintRtranoToRtransOnZIZ(this);
      }

      #endregion PTG Additions

      return OK; // validated and localOK 

   } // private bool SaveVvDataRecord(object sender, EventArgs e)

   private (string ABU_ABI_tt, uint ABU_ABI_ttNum) AddCashFakturToBlagajna(Faktur cashFaktur_rec)
   {
      bool is4Isplatnica     = cashFaktur_rec.TtInfo.IsFinKol_U;
      string targetTT        = (is4Isplatnica ? Faktur./*TT_BIS*/TT_ABI : Faktur./*TT_BUP*/TT_ABU);
      string targetVezniText = (is4Isplatnica ? "Isplata po računu " : "Uplata po računu ");

      #region Set TransesSum for eventual cashPonder

      decimal cashPonder;

      if(cashFaktur_rec.R_IsNpMix && cashFaktur_rec.R_ukKCRP_cash.NotZero()) cashPonder = ZXC.DivSafe(cashFaktur_rec.R_ukKCRP_cash, cashFaktur_rec.S_ukKCRP);
      else                                                                   cashPonder = 1.00M;

      foreach(Rtrans cashRtrans_rec in cashFaktur_rec.Transes)
      {
         cashRtrans_rec.T_pdvSt = 0.00M;
         cashRtrans_rec.T_cij   = cashRtrans_rec.R_CIJ_KCRP * cashPonder;

         cashRtrans_rec.T_TT     = targetTT; // da sprijeci CalcTrans_VELEP_Results_ByMPC 
         cashRtrans_rec.T_wanted = 0M      ; // da sprijeci CalcTrans_VELEP_Results_ByMPC 

         cashRtrans_rec.CalcTransResults(null);
      }

      cashFaktur_rec.TakeTransesSumToDokumentSum(true);

      #endregion 

      Faktur faktur_ABU_ABI_rec = (Faktur)(cashFaktur_rec.CreateNewRecordAndCloneItComplete());

      faktur_ABU_ABI_rec.InvokeTransClear();

      ushort line = 0;

      faktur_ABU_ABI_rec.SkladCD = faktur_ABU_ABI_rec.SkladCD.Replace("RE", "GL"); // !!! da obadva IRA skl iste sljednosti odu na glavno skladiste grada 
      faktur_ABU_ABI_rec.SkladCD = faktur_ABU_ABI_rec.SkladCD.Replace("ZA", "GL"); // !!! da i ZAR      skl                 ode na glavno skladiste grada 

      faktur_ABU_ABI_rec.TtNum        = 0;
      faktur_ABU_ABI_rec.TT           = targetTT;
      faktur_ABU_ABI_rec.VezniDok     = targetVezniText + cashFaktur_rec.TT_And_TtNum;
      faktur_ABU_ABI_rec.V1_tt        = cashFaktur_rec.TT   ;
      faktur_ABU_ABI_rec.V1_ttNum     = cashFaktur_rec.TtNum;
      faktur_ABU_ABI_rec.S_ukKCRP_NP1 = 0M;
      faktur_ABU_ABI_rec.NacPlac      = 
      faktur_ABU_ABI_rec.NacPlac2     = "";

      Rtrans rtrans_ABU_ABI_rec;

      for(int i = 0; i < cashFaktur_rec.Transes.Count; ++i)
      {
         rtrans_ABU_ABI_rec = (Rtrans)(cashFaktur_rec.Transes[i].CreateNewRecordAndCloneItComplete());

       //rtrans_BUP_BIS_rec.T_pdvSt = 0.00M;
       //rtrans_BUP_BIS_rec.T_cij   = cashFaktur_rec.Transes[i].R_CIJ_KCRP * cashPonder;
         rtrans_ABU_ABI_rec.T_jedMj = cashFaktur_rec.TT_And_TtNum;
         rtrans_ABU_ABI_rec.T_konto = cashFaktur_rec.Konto.NotEmpty() ? faktur_ABU_ABI_rec.Konto : (is4Isplatnica ? ZXC.KSD.Dsc_RKto_Dobav : ZXC.KSD.Dsc_RKto_Kupca);

         rtrans_ABU_ABI_rec.CalcTransResults(null);

         FakturDao.AutoSetFaktur(TheDbConnection, ref line, faktur_ABU_ABI_rec, rtrans_ABU_ABI_rec);
      }

      return (faktur_ABU_ABI_rec.TT, faktur_ABU_ABI_rec.TtNum);
   }

   private void AskForAndObtainNewDokNumAndDocumTT()
   {

      uint     dokNum = TheVvDao.GetNextDokNum(TheDbConnection, TheVvDataRecord.VirtualRecordName);
      uint     ttNum  = TheVvDao.GetNextTtNum (TheDbConnection, ((VvDocumLikeRecord)TheVvDataRecord).VirtualTT, null);

      #region FISKALIZE

      DateTime freshDokDateTime = DateTime.MinValue;

      bool shouldGiveFreshDokDateTime = false;

      if(TheVvRecordUC is FakturDUC)
      {

         Faktur faktur_rec = (Faktur)(TheVvDataRecord);

         // 06.05.2013: 
       //if(Faktur.IsFiskalDutyTT_ONLINE(faktur_rec.TT, faktur_rec.IsNpCash))
         if(faktur_rec.IsFiskalDutyFaktur)
         {
            shouldGiveFreshDokDateTime = true;
                      freshDokDateTime = VvSQL.GetServer_DateTime_Now(TheDbConnection);
         }
      }

      #endregion FISKALIZE


      string message = string.Format("Broj dokumenta [{0}] i/ili broj tipa transakcije [{1} - {2}] je u datoteci već zauzet!\n\n" +
                                     "Mogu li u Vaše ime predložiti nove podatke:\n\n" +
                                     "Broj dokumenta [{3}] i/ili broj tipa transakcije [{1} - {4}]", 
                                          ((VvDocumLikeRecord)TheVvDataRecord).VirtualDokNum,
                                          ((VvDocumLikeRecord)TheVvDataRecord).VirtualTT,
                                          ((VvDocumLikeRecord)TheVvDataRecord).VirtualTTnum,
                                          dokNum, ttNum);

      DialogResult result = MessageBox.Show(message, "Potvrdite novi broj dokumenta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

      if(result != DialogResult.Yes) return;

      if(shouldGiveFreshDokDateTime) TheVvDocumentLikeRecordUC.Put_NewDocum_NumAndDateFields(dokNum, freshDokDateTime);
      else                           TheVvDocumentLikeRecordUC.Put_NewDocum_NumAndDateFields(dokNum, ((VvDocumLikeRecord)TheVvDataRecord).VirtualDokDate);

      TheVvDocumentLikeRecordUC.Put_NewTT_Num(ttNum);
   }

   /* */
   /* ESCAPE */
   /* */
   public void EscapeAction_OnClick(object sender, EventArgs e)
   {
      // 19.02.2019: 
      if(New_FromVvXmlDR_InProgress == true)
      {
         TheVvDataRecord = prevTheVvDataRecord.CreateNewRecordAndCloneItComplete(); // perform RESTORE 

         New_FromVvXmlDR_InProgress = false;
      }

      if(IsArhivaTabPage)
      {
         ExitArhiva();
         return;
      }
      
      if(HasTheTabPageAnyUnsavedData(TheVvTabPage)) return;

      // 29.09.2017: 
      if(ZXC.TH_Should_ESC_DRW_Log)
      {
         TheVvUC.GetFields(false);
         //bool OK = 
         XtransDao.Addrec_ESC_DRW_Log(TheDbConnection, "ESC", TheVvDataRecord as Faktur, -1);
      }

      ZXC.FakturRec                      = null;
      ZXC.RISK_CopyToOtherDUC_inProgress = false;
      ZXC.RISK_CopyToMixerDUC_inProgress = false;
      ZXC.OffixImport_InProgress         = false;

      CloseForWriteActions(true);

      if(TheVvUC is FakturDUC && ((FakturDUC)TheVvUC).IsShowingConvertedMoney) RISK_ToggleKnDeviza(null, EventArgs.Empty);

      WhenRecordInDBHasChangedAction();

      if(ZXC.KOPfromFUG_InProgress ) { CloseKOP_ShowFUGandIzlistaj(); }
      if(ZXC.KOPfromUGAN_InProgress) { CloseKOP_JOB(); }

   }

   private void CloseKOP_ShowFUGandIzlistaj()
   {
      CloseKOP_JOB();

      ZXC.KOPfromFUG_InProgress = false;

      VvSubModul FUGvvSubModul = GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.R_FUG_PTG);

      VvTabPage FUGtabPage = TheTabControl.Documents.Select(d => d.Control as VvTabPage).Where(p => p != null)
         .FirstOrDefault(tab => tab.SubModul_xy == FUGvvSubModul.xy);

      if(FUGtabPage != null) FUGtabPage.Selected = true;

      RISK_PTG_Izlistaj_TheG1_RateZaFak(null, EventArgs.Empty);
   }

   private void CloseKOP_JOB()
   {
      VvSubModul KOPvvSubModul = GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.X_KOP);

      VvTabPage KOPtabPage = TheTabControl.Documents.Select(d => d.Control as VvTabPage).Where(p => p != null)
         .FirstOrDefault(tab => tab.SubModul_xy == KOPvvSubModul.xy);

      if(KOPtabPage != null)
      {
         KOPtabPage.Dispose();

         var kopDoc = TheTabControl.Documents.FirstOrDefault(d => d.Control == KOPtabPage);
         if(kopDoc != null) this.TheTabControl.Controller.Close(kopDoc);

         if(ZXC.KOPfromUGAN_TabIndex >= 0 && ZXC.KOPfromUGAN_TabIndex < TheTabControl.Documents.Count)
            TheTabControl.Controller.Activate(TheTabControl.Documents[ZXC.KOPfromUGAN_TabIndex]);

         ZXC.KOPfromUGAN_InProgress = false;
         ZXC.KOPfromUGAN_TabIndex   =     0;
      }
   }

   private void ExitArhiva()
   {
      bool OK;

      ToggleArhivaVisualApperiance(false);

      OK = TheVvDao.SetMe_Record_byRecID(TheDbConnection, TheVvDataRecord, RecID_BeforeArhivaEntered, false, true);

      if(OK) PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);
      else   TakeNewPositionAndPutFields();

   }

   /* */
   /* FIND RECORD */
   /* */
   public void FindRecord_OnClick(object sender, EventArgs e)
   {
      //30.3.2011:
      if(TheVvTabPage.WriteMode != ZXC.WriteMode.None) return;

      VvFindDialog dlg = TheVvRecordUC.CreateVvFindDialog();

      dlg.ApplyEventHandler += new System.EventHandler(this.ApplyEventHandlerInFindRecord);
      if(dlg.ShowDialog() == DialogResult.OK)
      {
         if(!TheVvDao.SetMe_Record_byRecID(TheDbConnection, TheVvDataRecord, (uint)dlg.SelectedRecID, IsArhivaTabPage)) return;

         PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);
      }

      dlg.Dispose();
   }

   private void RISK_FindSerLot(object sender, EventArgs e) 
   {
    //VvFindDialog dlg = new VvFindDialog();
    //dlg.TheRecListUC = new RtransListUC(dlg, new Rtrans(), ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsRTR));
    //
    //if(dlg.ShowDialog() == DialogResult.OK)
    //{
    //   string selectedTT       = ((RtransListUC)dlg.TheRecListUC).SelectedTT      ;
    //   uint   selectedParentID = ((RtransListUC)dlg.TheRecListUC).SelectedParentID;
    //   bool rtrFound = selectedTT.NotEmpty() && selectedParentID.NotZero();
    //
    //   if(rtrFound)
    //   {
    //      Point xy = GetSubModulXY(GetVvSubModulEnumFrom_SubModulShortName(selectedTT     ));
    //      ZXC.TheVvForm.OpenNew_Record_TabPage(xy,                         selectedParentID);
    //   }
    //}
    //dlg.Dispose();

    VvFindDialog dlg = new VvFindDialog();

    if(ZXC.IsPCTOGO)
    { 
       dlg.TheRecListUC = new RtranoListUC(dlg, new Rtrano(), ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsRTO));
    }
    else
    {
       dlg.TheRecListUC = new RtransListUC(dlg, new Rtrans(), ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsRTR));
    }

    if(dlg.ShowDialog() == DialogResult.OK)
    {
       string selectedTT       = ((RtransListUC)dlg.TheRecListUC).SelectedTT      ;
       uint   selectedParentID = ((RtransListUC)dlg.TheRecListUC).SelectedParentID;
       bool rtrFound = selectedTT.NotEmpty() && selectedParentID.NotZero();
    
       if(rtrFound)
       {
          Point xy = GetSubModulXY(GetVvSubModulEnumFrom_SubModulShortName(selectedTT     ));
          ZXC.TheVvForm.OpenNew_Record_TabPage(xy,                         selectedParentID);
       }
    }
    dlg.Dispose();

   }

   public void ApplyEventHandlerInFindRecord(object sender, EventArgs e)
   {
      VvFindDialog dlg = (VvFindDialog)sender;

      if(!TheVvDao.SetMe_Record_byRecID(TheDbConnection, TheVvDataRecord, (uint)dlg.SelectedRecID, IsArhivaTabPage)) return;

      PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);
   }

   public VvSQL.RecordSorter ActiveSorter
   {
      get { return (VvSQL.RecordSorter)tsCbxSorterType.SelectedItem; }
   }

   #endregion Save, Escape, Find

   #region Navigation (REREAD, FIRST, PREV, NEXT, LAST)

   /* */
   /* REREAD RECORD */
   /* */
   /*private*/internal void ReRead_OnClick(object sender, EventArgs e)
   {
      WhenRecordInDBHasChangedAction();

      ////// TODO15: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! DELLME IMEDIATELLY
      //
      //BMW 
      //    //                  D     F        F       W    W    D     F           F                    
      //    theBMW = new BMW(42000, 466650M, 272278M, 03, 2015, 149, false, new DateTime(2018, 03, 31)); // 530xd 3 godine 
      //    theBMW = new BMW(35900, 526415M, 272278M, 04, 2015, 154, false, new DateTime(2018, 03, 31)); // 530xd 2 godine 
      //    theBMW = new BMW(44000, 526415M,  20000M, 03, 2016, 154, false, new DateTime(2018, 03, 31)); // 535xd 2 godine 
      //    theBMW = new BMW(44000, 526415M, 272278M, 03, 2015, 154, false, new DateTime(2018, 03, 31)); // 535xd 3 godine 
      //    theBMW = new BMW(62000, 515900M, 288888M, 03, 2017, 139, false, new DateTime(2018, 03, 31)); // 530xd 1 godina novi
      //
      //    theBMW = new BMW(44000, 526415M, 272278M, 03, 2016, 154, false, new DateTime(2018, 03, 31)); // 535xd 2 godine 
      //    theBMW = new BMW(44000, 466650M, 272278M, 03, 2016, 149, false, new DateTime(2018, 03, 31)); // 530xd 2 godine 
      //    theBMW = new BMW(44000, 434137M, 272278M, 03, 2016, 148, false, new DateTime(2018, 03, 31)); // 335xd 2 godine 

      //if(TheVvUC != null && TheVvUC is VvRecordUC)
      //{
      //   VvHamper.Unhide_PasswordTextBox_Text(TheVvUC);
      //}

      //VvInstal_CRforVV_runtime();

#if DEBUG

      //BarcodeLib.Barcode barcode = new BarcodeLib.Barcode(/*"30583306", BarcodeLib.TYPE.EAN8*/);
      //Image barcodeImage = barcode.Encode(BarcodeLib.TYPE.EAN13, "0000046100115", Color.DarkBlue, Color.LightBlue, 500, 200);

      //System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CurrentUICulture;

      // ::: Testno slanje eRačuna  možete obaviti u demo okruženju na sljedećoj poveznici: http://demo.moj-eracun.hr/hr/. 
      // ::: korisnički podatci za pristup demo servisu ( http://demo.moj-eracun.hr) su : 
      // ::: 
      // ::: Korisnički ID: 6072
      // ::: Lozinka: eCMH8Jhm
      // ::: Lozinka: buV733eX
      // ::: SoftwareID: Test-001 
      // ::: 
      // ::: Molim Vas da prilikom testiranja koristite vlastite mail adrese za zaprimanje eRačuna.

      // ::: https://www.moj-eracun.hr/hr/Manual/Stable/Api

      //  string merXmlStr  = "qwerty_qweqwe_zxc";
      //  string jsonString = Vv_Http_Web_request.VvMER_JsonMessage_Serializator(6072, "buV733eX", "60042587515", "", "Test-001", merXmlStr);
      ////string webAddress = @"http://demo.moj-eracun.hr/hr/";
      //  string webAddress = @"https://demo.moj-eracun.hr/apis/v2/send";
      //  Vv_Http_Web_request.Vv_PostJson(webAddress, jsonString);

      //ZXC.Check_TH_SKL_ShopWeekKind();

#if stariPrimjer
      PTG_Ugovor ugan_rec1 = new PTG_Ugovor();
                 ugan_rec1.DokDate = new DateTime(2021, 02, 01);
                 ugan_rec1.S_ukKCR = 1000.00M;
                 ugan_rec1.PTG_BrojRata = 12;
                 ugan_rec1.PTG_DanFakturiranja = ZXC.PTG_DanFakturiranjaEnum.NaDanUgovora;

      PTG_Ugovor ugan_rec2 = new PTG_Ugovor();
                 ugan_rec2.DokDate = new DateTime(2021, 02, 24);
                 ugan_rec2.S_ukKCR = 1000.00M;
                 ugan_rec2.PTG_BrojRata = 12;
                 ugan_rec2.PTG_DanFakturiranja = ZXC.PTG_DanFakturiranjaEnum.NaDanUgovora;

      PTG_Ugovor ugan_rec3 = new PTG_Ugovor();
                 ugan_rec3.DokDate = new DateTime(2021, 03, 31);
                 ugan_rec3.S_ukKCR = 1000.00M;
                 ugan_rec3.PTG_BrojRata = 12;
                 ugan_rec3.PTG_DanFakturiranja = ZXC.PTG_DanFakturiranjaEnum.NaDanUgovora;

      PTG_Ugovor ugan_rec4 = new PTG_Ugovor();
                 ugan_rec4.DokDate = new DateTime(2021, 02, 01);
                 ugan_rec4.S_ukKCR = 1000.00M;
                 ugan_rec4.PTG_BrojRata = 12;
                 ugan_rec4.PTG_DanFakturiranja = ZXC.PTG_DanFakturiranjaEnum.ZadnjiDanMjeseca;

      PTG_Ugovor ugan_rec5 = new PTG_Ugovor();
                 ugan_rec5.DokDate = new DateTime(2021, 02, 24);
                 ugan_rec5.S_ukKCR = 1000.00M;
                 ugan_rec5.PTG_BrojRata = 12;
                 ugan_rec5.PTG_DanFakturiranja = ZXC.PTG_DanFakturiranjaEnum.ZadnjiDanMjeseca;

      PTG_Ugovor ugan_rec6 = new PTG_Ugovor();
                 ugan_rec6.DokDate = new DateTime(2021, 02, 28);
                 ugan_rec6.S_ukKCR = 1000.00M;
                 ugan_rec6.PTG_BrojRata = 12;
                 ugan_rec6.PTG_DanFakturiranja = ZXC.PTG_DanFakturiranjaEnum.ZadnjiDanMjeseca;

      PTG_Ugovor ugan_rec7 = new PTG_Ugovor();
                 ugan_rec7.DokDate = new DateTime(2021, 02, 01);
                 ugan_rec7.S_ukKCR = 1000.00M;
                 ugan_rec7.PTG_BrojRata = 12;
                 ugan_rec7.PTG_DanFakturiranja = ZXC.PTG_DanFakturiranjaEnum.PrviDanMjeseca;

      PTG_Ugovor ugan_rec8 = new PTG_Ugovor();
                 ugan_rec8.DokDate = new DateTime(2021, 02, 24);
                 ugan_rec8.S_ukKCR = 1000.00M;
                 ugan_rec8.PTG_BrojRata = 12;
                 ugan_rec8.PTG_DanFakturiranja = ZXC.PTG_DanFakturiranjaEnum.PrviDanMjeseca;

      PTG_Ugovor ugan_rec9 = new PTG_Ugovor();
                 ugan_rec9.DokDate = new DateTime(2021, 03, 31);
                 ugan_rec9.S_ukKCR = 1000.00M;
                 ugan_rec9.PTG_BrojRata = 12;
                 ugan_rec9.PTG_DanFakturiranja = ZXC.PTG_DanFakturiranjaEnum.PrviDanMjeseca;

      PTG_OtplatniPlan op1 = new PTG_OtplatniPlan(TheDbConnection, ugan_rec1);
      PTG_OtplatniPlan op2 = new PTG_OtplatniPlan(TheDbConnection, ugan_rec2);
      PTG_OtplatniPlan op3 = new PTG_OtplatniPlan(TheDbConnection, ugan_rec3);
      PTG_OtplatniPlan op4 = new PTG_OtplatniPlan(TheDbConnection, ugan_rec4);
      PTG_OtplatniPlan op5 = new PTG_OtplatniPlan(TheDbConnection, ugan_rec5);
      PTG_OtplatniPlan op6 = new PTG_OtplatniPlan(TheDbConnection, ugan_rec6);
      PTG_OtplatniPlan op7 = new PTG_OtplatniPlan(TheDbConnection, ugan_rec7);
      PTG_OtplatniPlan op8 = new PTG_OtplatniPlan(TheDbConnection, ugan_rec8);
      PTG_OtplatniPlan op9 = new PTG_OtplatniPlan(TheDbConnection, ugan_rec9);

#endif

      //PTG_Ugovor       ugan_rec = new PTG_Ugovor(((Faktur)TheVvDataRecord));
      //ugan_rec.LoadOtplatniPlan(TheDbConnection);
      //PTG_OtplatniPlan opThis   = new PTG_OtplatniPlan(TheDbConnection, ugan_rec);

#endif

      #region AUTO ADD SVD odjeli as USER

      //if(ZXC.IsSvDUH && ZXC.CURR_userName == ZXC.vvDB_programSuperUserName)
      //{
      //   TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Ticker);
      //   User user_rec; Prvlg prvlg_rec; int count = 0; int odjCD = 101;
      //   bool myOK, vvOK;
      //   foreach(Kupdob kupdob_rec in VvUserControl.KupdobSifrar.OrderBy(k => k.Ticker).Where(k => k.IsMtr == true))
      //   {
      //      //if(++count > 2) break;
      //
      //      user_rec = new User();
      //      user_rec.UserName = kupdob_rec.Ticker;
      //      user_rec.PasswdEncodedAsInFile = ZXC.EncryptThis_UserUC_Password(kupdob_rec.Ticker, kupdob_rec.Ticker);
      //      user_rec.Email   = ZXC.LenLimitedStr(kupdob_rec.Email, ZXC.UserDao.GetSchemaColumnSize(ZXC.UsrCI.email));
      //      user_rec.Ime     = ZXC.LenLimitedStr(kupdob_rec.Naziv, ZXC.UserDao.GetSchemaColumnSize(ZXC.UsrCI.ime));
      //      user_rec.Prezime = ZXC.LenLimitedStr(kupdob_rec.Url  , ZXC.UserDao.GetSchemaColumnSize(ZXC.UsrCI.prezime));
      //      user_rec.Oib     = odjCD++.ToString("000");
      //
      //      myOK = VvSQL.CreateUser(TheDbConnection, user_rec);
      //
      //      if(!myOK) break;
      //
      //      vvOK = TheVvDao.ADDREC(TheDbConnection, user_rec); // VOILA ################################################################### 
      //
      //      if(!vvOK) break;
      //
      //      prvlg_rec = new Prvlg();
      //      prvlg_rec.UserName   = user_rec.UserName;
      //      prvlg_rec.PrjktID    = ZXC.CURR_prjkt_rec.KupdobCD;
      //      prvlg_rec.PrjktTick  = ZXC.CURR_prjkt_rec.Ticker  ;
      //      prvlg_rec.PrvlgType  = "0";
      //      prvlg_rec.PrvlgScope = "012X";
      //      prvlg_rec.DocumType  = Faktur.TT_ZAH;
      //
      //      vvOK = TheVvDao.ADDREC(TheDbConnection, prvlg_rec); // VOILA ################################################################### 
      //
      //      prvlg_rec = new Prvlg();
      //      prvlg_rec.UserName   = user_rec.UserName;
      //      prvlg_rec.PrjktID    = ZXC.CURR_prjkt_rec.KupdobCD;
      //      prvlg_rec.PrjktTick  = ZXC.CURR_prjkt_rec.Ticker  ;
      //      prvlg_rec.PrvlgType  = "2";
      //      prvlg_rec.PrvlgScope = "012X";
      //      prvlg_rec.DocumType  = Faktur.TT_IZD;
      //
      //      vvOK = TheVvDao.ADDREC(TheDbConnection, prvlg_rec); // VOILA ################################################################### 
      //
      //   }
      //}

      #endregion AUTO ADD SVD odjeli as USER

      //List<PTG_Rata> The_Rata_List_ZaFakturiranje = PTG_Ugovor.Get_PTG_Rata_List_ZaFakturiranje(TheDbConnection, DateTime.Now, PTG_Ugovor.PTG_FakturiranjeKind.Grupno);


#if DEBUG

      //List<VvUtilDataPackage> installs = ZXC.VvGetInstalledPrograms();
      //bool hasAppropriateCRruntime = installs.Any(prg => 
      //   prg.TheStr1 == @"{36B0EEF1-E0B2-40FC-BEE5-F036E51D7540}" ||
      //   prg.TheStr1 == @"{4D5EEA90-E0C2-4130-8FC2-F468998AADA3}");

      //
      //      List<Faktur> theFakturList = new List<Faktur>();
      //
      //      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
      //      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt], "theTT", Faktur.TT_IFA, " = "));
      //
      //      //rptFilter.FilterMembers = filterMembers;
      //
      //      VvDaoBase.LoadGenericVvDataRecordList<Faktur>(TheDbConnection, theFakturList, filterMembers, "", "dokDate, ttSort, ttNum", true);
      //      foreach(Faktur faktur_rec in theFakturList)
      //      {
      //         faktur_rec.VvDao.LoadTranses(TheDbConnection, faktur_rec, false);
      //      }
      //
      //      VvRiskReport.FakturList_To_PDF(theFakturList);
      //

      //ZXC.aim_emsg(((Artikl)TheVvDataRecord).TheAsEx.HasUselessPST.ToString());

      //decimal najsKuna;
      //decimal najsEuroizKuna;
      //decimal najsEuroizEura;
      //
      //najsKuna = ZXC.NiceEURo_ReadyKuna( 5M); najsEuroizKuna = ZXC.EURiIzKuna_HRD_(najsKuna); najsEuroizEura = ZXC.NiceEURo_0_50(0.10M);
      //najsKuna = ZXC.NiceEURo_ReadyKuna(10M); najsEuroizKuna = ZXC.EURiIzKuna_HRD_(najsKuna); najsEuroizEura = ZXC.NiceEURo_0_50(0.20M);
      //najsKuna = ZXC.NiceEURo_ReadyKuna(15M); najsEuroizKuna = ZXC.EURiIzKuna_HRD_(najsKuna); najsEuroizEura = ZXC.NiceEURo_0_50(0.30M);
      //najsKuna = ZXC.NiceEURo_ReadyKuna(20M); najsEuroizKuna = ZXC.EURiIzKuna_HRD_(najsKuna); najsEuroizEura = ZXC.NiceEURo_0_50(0.40M);
      //najsKuna = ZXC.NiceEURo_ReadyKuna(25M); najsEuroizKuna = ZXC.EURiIzKuna_HRD_(najsKuna); najsEuroizEura = ZXC.NiceEURo_0_50(0.50M);
      //najsKuna = ZXC.NiceEURo_ReadyKuna(30M); najsEuroizKuna = ZXC.EURiIzKuna_HRD_(najsKuna); najsEuroizEura = ZXC.NiceEURo_0_50(0.60M);
      //najsKuna = ZXC.NiceEURo_ReadyKuna(35M); najsEuroizKuna = ZXC.EURiIzKuna_HRD_(najsKuna); najsEuroizEura = ZXC.NiceEURo_0_50(0.70M);
      //najsKuna = ZXC.NiceEURo_ReadyKuna(40M); najsEuroizKuna = ZXC.EURiIzKuna_HRD_(najsKuna); najsEuroizEura = ZXC.NiceEURo_0_50(0.80M);
      //najsKuna = ZXC.NiceEURo_ReadyKuna(45M); najsEuroizKuna = ZXC.EURiIzKuna_HRD_(najsKuna); najsEuroizEura = ZXC.NiceEURo_0_50(0.90M);
      //najsKuna = ZXC.NiceEURo_ReadyKuna(50M); najsEuroizKuna = ZXC.EURiIzKuna_HRD_(najsKuna); najsEuroizEura = ZXC.NiceEURo_0_50(1.25M);
      //najsKuna = ZXC.NiceEURo_ReadyKuna(55M); najsEuroizKuna = ZXC.EURiIzKuna_HRD_(najsKuna); najsEuroizEura = ZXC.NiceEURo_0_50(1.50M);
      //najsKuna = ZXC.NiceEURo_ReadyKuna(60M); najsEuroizKuna = ZXC.EURiIzKuna_HRD_(najsKuna); najsEuroizEura = ZXC.NiceEURo_0_50(1.75M);
      //
      //decimal kune = VvSkyLab.Kune_From_KunskiArtiklName("AC0012"  );
      //decimal euri = VvSkyLab.Kune_From_KunskiArtiklName("AC012,34");
      //
      //decimal niceEuro2;
      //niceEuro2 = ZXC.RoundUpToNearest(1.00M, 0.02M); // 1
      //niceEuro2 = ZXC.RoundUpToNearest(1.01M, 0.02M); // 1.5
      //niceEuro2 = ZXC.RoundUpToNearest(1.49M, 0.02M); // 1.5
      //niceEuro2 = ZXC.RoundUpToNearest(1.50M, 0.02M); // 1.5
      //niceEuro2 = ZXC.RoundUpToNearest(1.51M, 0.02M); // 2
      //niceEuro2 = ZXC.RoundUpToNearest(1.99M, 0.02M); // 2
      //niceEuro2 = ZXC.RoundUpToNearest(2.00M, 0.02M); // 2
      //niceEuro2 = ZXC.RoundUpToNearest(2.01M, 0.02M); // 2.5
      //niceEuro2 = ZXC.RoundUpToNearest(2.49M, 0.02M); // 2.5
      //niceEuro2 = ZXC.RoundUpToNearest(2.50M, 0.02M); // 2.5
      //niceEuro2 = ZXC.RoundUpToNearest(2.51M, 0.02M); // 3

      //najsEuroizEura = ZXC.NiceEURo(0.10M, 0.50M);
      //najsEuroizEura = ZXC.NiceEURo(0.20M, 0.50M);
      //najsEuroizEura = ZXC.NiceEURo(0.30M, 0.50M);
      //najsEuroizEura = ZXC.NiceEURo(0.40M, 0.50M);
      //najsEuroizEura = ZXC.NiceEURo(0.50M, 0.50M);
      //najsEuroizEura = ZXC.NiceEURo(0.60M, 0.50M);
      //najsEuroizEura = ZXC.NiceEURo(0.70M, 0.50M);
      //najsEuroizEura = ZXC.NiceEURo(0.80M, 0.50M);
      //najsEuroizEura = ZXC.NiceEURo(0.90M, 0.50M);
      //najsEuroizEura = ZXC.NiceEURo(1.25M, 0.50M);
      //najsEuroizEura = ZXC.NiceEURo(1.50M, 0.50M);
      //najsEuroizEura = ZXC.NiceEURo(1.75M, 0.50M);

      //string poruka = VvDaoBase.Get_TABLE_Create_Time(TheDbConnection, Artikl.recordName).ToString(ZXC.VvDateAndTimeFormat);
      //ZXC.aim_emsg(MessageBoxIcon.Asterisk, "ovo bu poruka: {0}", poruka);
      //ZXC.aim_emsg(                         "ovo bu poruka: {0}", poruka);
      //ZXC.aim_emsg(MessageBoxIcon.Asterisk,                       poruka);
      //ZXC.aim_emsg(                                               poruka);

      //ZXC.aim_emsg_List("Naslov", new List<string> { poruka });

      //        decimal a1, b1, b2, pdvSt = 25.00M; List<string> msgList = new List<string>(); int badCount=0;
      //      decimal theOsn, thePdv, theUk;
      //    //  for(decimal c = 0.02M; c <=  10.00M; c = c + 0.05M)
      //        for(decimal c = 0.01M; c <= 100.00M; c = c + 0.01M)
      //        {
      //            Rtrans rtrans = new Rtrans()
      //            {
      //               T_rbt1St = 50M,
      //               T_pdvSt  = 25M,
      //               T_kol    = 1M,
      //               T_cij    = c,
      //               T_skladDate = ZXC.Date01042023,
      //               T_TT = Faktur.TT_IRM
      //            };
      //
      //            rtrans.CalcTransResults(null);
      //
      //         theOsn = rtrans.R_KCR;
      //         thePdv = rtrans.R_pdv;
      //         theUk  = rtrans.R_KCRP;
      //
      //         //b1 = ZXC.VvGet_25_from_125( c, pdvSt).Ron2();
      //         //a1 = c - b1;
      //         b2 = ZXC.VvGet_25_of_100  (theOsn, pdvSt).Ron2();
      //         //
      //         if(theOsn + thePdv != theUk)
      //         {
      //            badCount++;
      //            msgList.Add(String.Format("!!!!!!!!!!   a1 {0} + b2 {1} != c  {2}", theOsn.ToStringVv(), thePdv.ToStringVv(), theUk.ToStringVv()));
      //         }
      //         //else
      //         //{
      //         //   msgList.Add(String.Format("ok a1 {0} + b2 {1} = c  {2}", theOsn.ToStringVv(), thePdv.ToStringVv(), theUk.ToStringVv()));
      //         //
      //         //}
      //         if(thePdv.Ron2() != b2)
      //         {
      //            msgList.Add(String.Format("   thePdv {0} != b2 {1}", thePdv.ToStringVv(), b2.ToStringVv()));
      //         }
      //         else
      //         {
      //            msgList.Add(String.Format("ok {0} {1} ", thePdv.Ron2().ToStringVv(), b2.ToStringVv()));
      //         }
      //
      //      }
      //
      //      ZXC.aim_emsg_List(String.Format
      //           (String.Format("{0} of {1}", badCount.ToString(), msgList.Count.ToString())), 
      //           msgList);

      //////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
      //decimal a1, b1, b2, pdvSt = 25.00M; List<string> msgList = new List<string>(); int badCount = 0;
      //decimal thePdv, theUk, the25;
      //decimal thePdvSS=0.00M, theOsnSS=0.00M, the25SS=0.00M;
      //for(decimal theOsn = 10.00M; theOsn <= 20.00M; theOsn = theOsn + 0.01M)
      //{
      //   thePdv = ZXC.VvGet_25_of_100(theOsn, pdvSt).Ron2();
      //   the25  = ZXC.VvGet_25_of_100(theOsn, pdvSt)       ;
      //
      //   bool manjePlaceno;
      //
      //   if(thePdv == the25) continue;
      //
      //   theOsnSS += theOsn;
      //   thePdvSS += thePdv;
      //   the25SS  += the25 ;
      //
      //   manjePlaceno = thePdv < the25;
      //
      //   msgList.Add(String.Format("{0} {1} {2} = {3} {4} {5}", theOsn.ToString(), thePdv.ToString(), the25.ToString(),
      //      (theOsn + thePdv).ToString(), manjePlaceno ? "!" : " ", (thePdv - the25).ToString()));
      //
      //}
      //
      //ZXC.aim_emsg(String.Format("{0} {1} {2}", theOsnSS.ToString(), thePdvSS.ToString(), the25SS.ToString()));
      //
      //ZXC.aim_emsg_List("", msgList);

      //List<string> sernoList = MixerDao.GetDistinctRtranoSernoForArtiklAndSklad(TheDbConnection, "HP Z600", "ZNJ");
      //
      //List<PCK_ArtiklInfo_Line> PCK_ArtiklInfo_List = /*PCK_SernoInfo_Dao*/RtranoDao.Get_PCK_ArtiklInfo_List_ForArtiklAndSklad(TheDbConnection, "HP Z600", "ZNJ");



      //Hapi theHapi;
      //string sharedSecret = "0102030405060708091011121314151617181920212223242526272829303132";
      //theHapi = HapiFactory.GetAsyncInterface(this).DefaultSharedSecret(sharedSecret);
      //// The api is now initialized. Yay! we've even set a default shared secret!
      //// The shared secret is a unique string shared between the card reader and your mobile application. 
      //// It prevents other people to connect to your card reader.
      //
      //// Subscribe to the status notifications
      //theHapi.AddStatusNotificationEventHandler(this);

      // VvPDFreporter theReporter = new VvPDFreporter(TheDbConnection, "VvPartneri", ZXC.projectYearFirstDay, new DateTime(2024, 5, 31), @"D:\");

      //List<Rtrano> UGAN_RtranoList = RtranoDao.Get_UGAN_RtranoList(TheDbConnection,     11, false);
      //             UGAN_RtranoList = RtranoDao.Get_UGAN_RtranoList(TheDbConnection, 100011, true );
      //             UGAN_RtranoList = RtranoDao.Get_UGAN_RtranoList(TheDbConnection,     11, true );
      //             UGAN_RtranoList = RtranoDao.Get_UGAN_RtranoList(TheDbConnection, 100011, false);

      // 14.08.2025: plus hosting inzistira na SSL/TLS ... 
      //EmailHelper_GPT.SendEmail(new EmailMessageData_GPT 
      //{
      //SmtpServer = ZXC.ViperMailHost      ,
      //SmtpPort      = 465                 ,
      //SmtpUser   = ZXC.SkyLabEmailAddress ,
      //SmtpPass   = ZXC.SkyLabEmailPassword,
      //                                    
      //FromAddress = ZXC.SkyLabEmailAddress ,
      //
      //ToAddresses = new List<string> { "viper@zg.htnet.hr"},
      //   
      //Subject     = "Test Email"           ,
      //Body        = "Hello! This is a test email sent via C# using SSL/TLS.",
      //});

      //using(var httpClient = new System.Net.Http.HttpClient { BaseAddress = new Uri(/*"https://your-api-domain.com"*/ /*"https://www.moj-eracun.hr"*/ "https://www-demo.moj-eracun.hr") })
      //{
      //   var service = new MER_ApiClient.MER_Check_AMS_OIB_ApiService(httpClient);
      //
      //   var request = new MER_ApiClient.MER_Check_AMS_OIB_Request("04192765979");
      //
      //   service.MER_CheckI_AMS_OIB(request);
      //}

      //VvMER_Response_Data_AllActions vvMER_Response_Data_Ping = Vv_eRacun_HTTP.VvMER_WebService_Ping();

      //VvMER_Response_Data_AllActions vvPND_Response_Data_Ping = Vv_eRacun_HTTP.VvPND_WebService_Ping();



      //VvMER_Response_Data_AllActions response_Data_CheckAMS;
      //switch(ZXC.F2_TheProvider)
      //{
      //   case ZXC.F2_Provider_enum.MER:
      //      
      //WebApiResult<VvMER_Response_Data_AllActions> webApiResult = Vv_eRacun_HTTP.VvMER_WebService_CheckAMS("60042587515"); // Viper      
      //WebApiResult<VvMER_Response_Data_AllActions> webApiResult = Vv_eRacun_HTTP.VvMER_WebService_CheckAMS("04192765979"); // Delovski   
      //WebApiResult<VvMER_Response_Data_AllActions> webApiResult = Vv_eRacun_HTTP.VvMER_WebService_CheckAMS("62781739468"); // Pondi      
      //WebApiResult<VvMER_Response_Data_AllActions> webApiResult = Vv_eRacun_HTTP.VvMER_WebService_CheckAMS("42889250808"); // Moj eRacun 

      //Kupdob kupdob_rec = TheVvUC.Get_Kupdob_FromVvUcSifrar(104);
      //ZXC.AMSstatus AMSstatus = KupdobDao.RefreshKupdob_AMSstatus(TheDbConnection, kupdob_rec); // NOVO 

      //      break;
      //
      //   case ZXC.F2_Provider_enum.PND:
      //      
      //      response_Data_CheckAMS = Vv_eRacun_HTTP.VvPND_WebService_CheckAMS("12345677777"); // Viper      
      //      response_Data_CheckAMS = Vv_eRacun_HTTP.VvPND_WebService_CheckAMS("60042587515"); // Viper      
      //      response_Data_CheckAMS = Vv_eRacun_HTTP.VvPND_WebService_CheckAMS("04192765979"); // Delovski   
      //      response_Data_CheckAMS = Vv_eRacun_HTTP.VvPND_WebService_CheckAMS("62781739468"); // Pondi      
      //      response_Data_CheckAMS = Vv_eRacun_HTTP.VvPND_WebService_CheckAMS("42889250808"); // Moj eRacun 
      //      break;
      //
      //}



      //Xtrano xtrano_rec = new Xtrano();
      //xtrano_rec.VvDao.SetMe_Record_byRecID(TheDbConnection, xtrano_rec, 1, false);
      //
      //List<byte[]> pdfBytesList    = xtrano_rec.F2_Get_PDF_Bytes_List();
      //List<string> pdfFileNameList = xtrano_rec.F2_GetPdfFilenamesFrom_eRacun();
      //
      //byte[] pdfBytes = pdfBytesList[0]; // your PDF byte array
      //
      //string filename = pdfFileNameList[0];
      //string dirame   = VvPref.eRacun_Izlaz_Prefs.DirectoryName;
      //string fullPath = Path.Combine(dirame, filename);
      //File.WriteAllBytes(fullPath, pdfBytes);

      // OVO je za PDFiumViewer testiranje ... ali si odustao od toga 
      //using(var stream = new MemoryStream(pdfBytes))
      //{
      //   var pdfDocument = PdfiumViewer.PdfDocument.Load(stream);
      //   var pdfViewer = new PdfiumViewer.PdfViewer();
      //   pdfViewer.Document = pdfDocument;
      //   pdfViewer.Dock = DockStyle.Fill;
      //   this.Controls.Add(pdfViewer); // 'this' is your Form or UserControl
      //}

      //uint YYandRecID = ZXC.Get_YYandRecID(2097, 7654321);
      //(int year, uint recID) = ZXC.GetYearAndRecIDFrom_YYandRecID(YYandRecID);


      //string xmlFilePath = @"C:\temp\eRacun_ValidationError_20260116_093716.xml";
      //Vv_eRacun_HTTP. ExtractEmbeddedPdfsFromXmlFile(xmlFilePath);
#endif

      // FILIPIDIPI 

   } // ReRead_OnClick 

   /* */
   /* FIRST RECORD */
   /* */
   private void FirstRecord_OnClick(object sender, EventArgs e)
   {
      bool OK = TheVvDao.FrsPrvNxtLst_REC(TheDbConnection, TheVvDataRecord, VvSQL.DBNavigActionType.FRS, ActiveSorter, IsArhivaTabPage, TheVvUC.VvNavRestrictor_TT, TheVvUC.VvNavRestrictor_SKL, TheVvUC.VvNavRestrictor_SKL2);

      if(OK) PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);
   }

   /* */
   /* PREV RECORD */
   /* */
   private void PrevRecord_OnClick(object sender, EventArgs e)
   {
      bool OK = TheVvDao.FrsPrvNxtLst_REC(TheDbConnection, TheVvDataRecord, VvSQL.DBNavigActionType.PRV, ActiveSorter, IsArhivaTabPage, TheVvUC.VvNavRestrictor_TT, TheVvUC.VvNavRestrictor_SKL, TheVvUC.VvNavRestrictor_SKL2);

      if(OK) PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);
   }

   /* */
   /* NEXT RECORD */
   /* */
   private void NextRecord_OnClick(object sender, EventArgs e)
   {
      bool OK = TheVvDao.FrsPrvNxtLst_REC(TheDbConnection, TheVvDataRecord, VvSQL.DBNavigActionType.NXT, ActiveSorter, IsArhivaTabPage, TheVvUC.VvNavRestrictor_TT, TheVvUC.VvNavRestrictor_SKL, TheVvUC.VvNavRestrictor_SKL2);

      if(OK) PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);
   }

   /* */
   /* LAST RECORD */
   /* */
   /*private*/
   public void LastRecord_OnClick(object sender, EventArgs e)
   {
      bool OK = TheVvDao.FrsPrvNxtLst_REC(TheDbConnection, TheVvDataRecord, VvSQL.DBNavigActionType.LST, ActiveSorter, IsArhivaTabPage, TheVvUC.VvNavRestrictor_TT, TheVvUC.VvNavRestrictor_SKL, TheVvUC.VvNavRestrictor_SKL2);

      if(OK) PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);

#if DEBUG
      // Put some test method call down there: ============================ 

      //decimal theTecaj = ZXC.DevTecDao.GetTecaj(conn, ZXC.BankaEnum.HNB, ZXC.ValutaNameEnum.EUR, ZXC.TipTecajaEnum.SREDNJI, new DateTime(2010, 5, 3));

      /*
    //KtoShemaDsc ktoSchemaDsc = new KtoShemaDsc(ZXC.dscLuiLst_KtoShema);
      KtoShemaDsc ktoSchemaDsc = ZXC.KSD;
      ktoSchemaDsc.Dsc_IrmGroupByNacPlac = true;
      ktoSchemaDsc.Dsc_IrmSumMonthly     = false;
      List<Faktur> delme1;
      List<Rtrans> delme2;
      FakturDao.GetNeprebaceniFakturAndRtrans2NalogLists(conn, out delme1, out delme2, ktoSchemaDsc, ZXC.Faktur2NalogSetEnum.IZLAZNI_VP, ZXC.Faktur2NalogTimeRuleEnum.DoIt_For_AddTS_NotToday, ZXC.projectYearFirstDay, ZXC.projectYearLastDay);
      */
      //System.Diagnostics.Process.Start("notepad.exe", @"E:\SifreDjelatnosti.txt");
      //System.Diagnostics.Process.Start(@"E:\0_UPLOAD\SvetiDuh\ALMP za 2007 sa ORG BOP COP.xls");

      //TestSomething();

      //Nalog vvDocument_rec = new Nalog(); bool isArhiva = false;
      //vvDocument_rec.VvDao.SetMe_VvDocumentRecord_byTtAndTtNum(TheDbConnection, vvDocument_rec, "IZ", 2, isArhiva, false);
      //vvDocument_rec.VvDao.LoadTranses(TheDbConnection, vvDocument_rec, isArhiva);

#region XML Load from file TEST

      //System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument(); //* create an xml document object.
      //xmlDoc.Load(@"E:\0_UPLOAD\PPUK_VGR\Popratnice 2013\15022013 - 15022013.xml"); //* load the XML document from the specified file.

      //foreach(var stuff in xmlDoc["Popratnice"])
      //{
      //}

      //XDocument xDoc2 = XDocument.Parse(xmlDoc.InnerXml);



      // stackoverflow: Read typed objects from XML using known XSD
      //      You need to do two steps:
      //1) Take your XML schema file and run it through the xsd.exe utility (which comes with the Windows SDK - it's in C:\Program Files\Microsoft SDKs\Windows\v6.0A\Bin\ or some similar path. This can turn the XSD file into a C# class:
      //xsd /c yourfile.xml
      //xsd /c yourfile.xsd
      //This should give you a file yourfile.cs which contains a class representing that XML schema.
      //2) Now, armed with that C# class, you should be able to just deserializing the XML file into an instance of your new object:
      //XmlSerializer ser = new XmlSerializer(typeof(foo));
      //string filename = Path.Combine(FilePath, "SimpleFields.xml");
      //foo myFoo = ser.Deserialize(new FileStream(filename, FileMode.Open)) as foo;
      //if (myFoo != null)
      //{
      //   // do whatever you want with your "foo"
      //}
      //That's about as simple as it gets! :-) 






      //string xmlFullPathFileName = @"E:\0_UPLOAD\PPUK_VGR\Popratnice 2013\01012012 - 31122012.xml";
      //string txtFullPathFileName = @"E:\0_UPLOAD\PPUK_VGR\Popratnice 2013\PPUK_TRUPCI.txt";

      ///* byQ */
      //HrSumePopratnica thePopratnicaByQ = new HrSumePopratnica(xmlFullPathFileName);

      ///* byDotNet */
      //XmlSerializer ser = new XmlSerializer(typeof(Popratnice));

      ///* byDotNet */
      //FileStream fs = new FileStream(fullPathName, FileMode.Open);
      //Popratnice xmlPoprat = ser.Deserialize(fs)) as Popratnice;
      //fs.Close();

      ///* byQ */
      //List<string> vvArtiklListDistinct = thePopratnica.StavkeList.OrderBy(st => st.VvArtiklName).Select(st => st.VvArtiklName).Distinct().ToList();

      //List<PopratniceZaglavljaStavke> stavkeList = new List<PopratniceZaglavljaStavke>();
      //foreach(PopratniceZaglavlja zaglavlje in xmlPoprat.Items)
      //{
      //   foreach(PopratniceZaglavljaStavke stavka in zaglavlje.Stavke)
      //   {
      //      stavkeList.Add(stavka);
      //   }
      //}

      //List<string> vvArtiklListDistinct = stavkeList.OrderBy(r => r.VvArtiklForExport).Select(r => r.VvArtiklForExport).Distinct().ToList();

      //using(StreamWriter sw = new StreamWriter(txtFullPathFileName, false, System.Text.Encoding.GetEncoding(1250)))
      //{
      //   foreach(string trupacVvName in vvArtiklListDistinct)
      //   {
      //      sw.WriteLine(trupacVvName);
      //   }
      //}

#endregion XML Load from file TEST

      //ZXC.aim_emsg(VvSQL.Get_Sql_ServerName(TheDbConnection));

      //System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
      //System.Drawing.Printing.PaperSize paper = new System.Drawing.Printing.PaperSize();

#if Doccnije

      //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> 
      FakturDUC theDUC = TheVvUC as FakturDUC;
      new ArtiklInfoExporter(theDUC.faktur_rec.Transes, @"E:", VvSQL.GetServer_DateTime_Now(TheDbConnection)).Save_ArtStatXmlExporter_ToXmlFile();
      //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> 

#endif

      // qweqwe ... tu isprobavaj ... 

#endif
      //string theHost;
      //theHost = "www.qw1ws44r.com"; ZXC.aim_emsg("{0}: {1}", theHost, ZXC.VvCC_DnsTest_ForHost(theHost));
      //theHost = "www.google.com"; ZXC.aim_emsg("{0}: {1}", theHost, ZXC.VvCC_DnsTest_ForHost(theHost));
      //theHost = ZXC.CURR_prjkt_rec.SkySrvrHostDecrypted; ZXC.aim_emsg("{0}: {1}", theHost, ZXC.VvCC_DnsTest_ForHost(theHost));
      //theHost = "";
      //ZXC.aim_emsg("Vv_ThisClientHasInternetConnection; {0}", ZXC.Vv_ThisClientHasInternetConnection);
      //ZXC.aim_emsg("Vv_ThisClientHasSkyLabConnection  ; {0}", ZXC.Vv_ThisClientHasSkyLabConnection);
      //ZXC.aim_emsg("Vv_ThisClientHasFiskalConnection  ; {0}", ZXC.Vv_ThisClientHasFiskalConnection);

      //foreach(VvLookUpItem sklLUI in ZXC.luiListaSkladista) ZXC.aim_emsg("sklad [{0}] is5 {1} is2PON {2} is2SRI {3}", sklLUI.Cd, ZXC.IsTH_5WeekShop(sklLUI.Cd), ZXC.IsTH_2Week_PON_Shop(sklLUI.Cd), ZXC.IsTH_2Week_SRI_Shop(sklLUI.Cd));

      //wZXC.aim_emsg(DateTime.Today.ToString(ZXC.VvDateYyyyMmDdMySQLFormat));


      //TheVvDataRecord.SaveSerialized_VvDataRecord_ToXmlFile(@"D:\Pero3.XML");
      //VvDataRecord theVvDataRecord = TheVvDataRecord.Deserialize_VvDataRecord_FromXmlFile(@"D:\Pero3.XML");
   }

#endregion Navigation (First, Prev, Next, Last)

   #region TestSomething

   public virtual void TestSomething()
   {
      string dataSource2003 = @"D:\MyExcel.xls";
      string dataSource2007 = @"D:\MyExcel.xlsx";
      string dataSourceVIP = @"D:\VIP - test.xls";

      //http://www.connectionstrings.com/ 

      string connectionString03 = @"Provider=Microsoft.Jet.OLEDB.4.0;  Data Source=" + dataSource2003 + @"; Extended Properties=""Excel 8.0;HDR=Yes;IMEX=1""";
      string connectionString07 = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + dataSource2007 + @"; Extended Properties=""Excel 12.0 Xml;HDR=YES;IMEX=1""";
      string connectionStringVIP = @"Provider=Microsoft.Jet.OLEDB.4.0;  Data Source=" + dataSourceVIP + @"; Extended Properties=""Excel 8.0;HDR=Yes;IMEX=1""";
      string connectionString2012 = @"Provider=Microsoft.Jet.OLEDB.4.0;  Data Source=" + dataSource2007 + @"; Extended Properties=""Excel 8.0;HDR=Yes;IMEX=1""";

      //using(OleDbConnection connection = new OleDbConnection(connectionString2012))
      //{
      //   using(OleDbCommand command = connection.CreateCommand())
      //   {
      //      // Qwerty$ comes from the name of the worksheet
      //      //command.CommandText = "SELECT * FROM [Qwerty$]";
      //      command.CommandText = "SELECT * FROM [Sheet1$]";

      //      connection.Open();

      //      command.CommandText = "Update [Sheet1$] set Name = 'Qukatz' where CD=1";
      //      command.ExecuteNonQuery();
      //   }
      //}
      try
      {

         System.Data.OleDb.OleDbConnection MyConnection;

         System.Data.OleDb.OleDbCommand myCommand = new System.Data.OleDb.OleDbCommand();

         string sql = null;

         MyConnection = new System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dataSource2007 + ";Extended Properties=Excel 8.0;");

         MyConnection.Open();

         myCommand.Connection = MyConnection;

         sql = "Update [Sheet1$] set Name = 'rijec' where CD=1";

         myCommand.CommandText = sql;

         myCommand.ExecuteNonQuery();

         MyConnection.Close();

      }

      catch(Exception ex)
      {

         MessageBox.Show(ex.ToString());

      }
   }

   public virtual void TestSomethingOLD3()
   {
      string dataSource2003 = @"D:\MyExcel.xls";
      string dataSource2007 = @"D:\MyExcel.xlsx";
      string dataSourceVIP  = @"D:\VIP - test.xls";
      
      //http://www.connectionstrings.com/ 

      string connectionString03  = @"Provider=Microsoft.Jet.OLEDB.4.0;  Data Source=" + dataSource2003 + @"; Extended Properties=""Excel 8.0;HDR=Yes;IMEX=1""";
      string connectionString07  = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + dataSource2007 + @"; Extended Properties=""Excel 12.0 Xml;HDR=YES;IMEX=1""";
      string connectionStringVIP = @"Provider=Microsoft.Jet.OLEDB.4.0;  Data Source=" + dataSourceVIP  + @"; Extended Properties=""Excel 8.0;HDR=Yes;IMEX=1""";

      using(OleDbConnection connection = new OleDbConnection(connectionStringVIP))
      {
         using(OleDbCommand command = connection.CreateCommand())
         {
            // Qwerty$ comes from the name of the worksheet
            //command.CommandText = "SELECT * FROM [Qwerty$]";
            command.CommandText = "SELECT * FROM [Sheet1$]";

            connection.Open();

            using(OleDbDataReader reader = command.ExecuteReader())
            {
               while(reader.Read())
               {
                  //FillDebitFromExcelDataReader(reader);
               }
            }
         }
      }      
   }

   #endregion TestSomething

   #endregion Offix like button actions (ADDREC, RWTREC, DELREC, FIND, SAVE, ...)

   #region RISK Specials

   internal bool BadGuys(object sender)
   {
    //if(ZXC.IsSvDUH_ZAHonly)
    //{
    //   //ZXC.aim_emsg(MessageBoxIcon.Warning, "Privremeno nedostupno.");
    //   return true;
    //}

      if(sender.ToString() != "JOPPD"          &&
         sender.ToString() != "PIZ_ObrazacIP1" &&
         sender.ToString() != "PIZ_ObrazacNP1" &&
         sender.ToString() != "PIZ_RAD1"       &&
         sender.ToString() != "PIZ_RAD1_G"
         )
      {
         return false;
      }

      if(
         ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.AGSJAJ || ZXC.CURR_prjkt_rec.Ticker.StartsWith("AGSJAJ") ||
         ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.LIKUM  || ZXC.CURR_prjkt_rec.Ticker.StartsWith("LIKUM" )
      )
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Kriva godina.");
         return true;
      }

      return false;
   }

   internal void ReportArtiklMinusManager(ArtStat artStat, bool isSklKolOnly)
   {
      //throw new NotImplementedException();
   }

   internal void ReportInternCijenaDiscrepancyManager(ArtStat artStat, decimal shouldBeCij, decimal t_cij)
   {
      // TODO: !!! vidi-razmisli kada ovo javljati!?
      //ZXC.aim_emsg(MessageBoxIcon.Warning, "{0} DokCij: {1} PrNabCij: {2}", artStat.ToString(), t_cij.ToStringVv(), /*artStat.PrNabCij*/shouldBeCij.ToStringVv());
   }

   internal void InitializeRiskTtInfo()
   {
      ZXC.TtInfoArray = new TtInfo[]
      {

         //         TheTT             TtSort   IsSklCdInTtNum   isFinKol_U   IsFinKol_I  DefaultSubModulXY           IsPreDef
         new TtInfo(Faktur.TT_WRN,   -02,      false,           false,       false,      ZXC.VvSubModulEnum.R_WYR),
         new TtInfo(Faktur.TT_YRN,   -01,      false,           false,       false,      ZXC.VvSubModulEnum.R_WYR),

         new TtInfo(Faktur.TT_ZPC, 

            (short)(ZXC.projectYearAsInt < 2022 ? 32 : 1)
                                        ,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_ZPC),

         new TtInfo(Faktur.TT_CJ_VP1, 01,      false,           false,       false,      ZXC.VvSubModulEnum.R_CJ,  true),
         new TtInfo(Faktur.TT_CJ_VP2, 02,      false,           false,       false,      ZXC.VvSubModulEnum.R_CJ,  true),
         new TtInfo(Faktur.TT_CJ_MP , 03,      false,           false,       false,      ZXC.VvSubModulEnum.R_CJ,  true),
         new TtInfo(Faktur.TT_CJ_DE , 04,      false,           false,       false,      ZXC.VvSubModulEnum.R_CJ,  true), // isti TtSort 
         new TtInfo(Faktur.TT_CKP   , 04,      false,           false,       false,      ZXC.VvSubModulEnum.R_CJK, false), // isti TtSort 
         new TtInfo(Faktur.TT_CJ_MK , 05,      false,           false,       false,      ZXC.VvSubModulEnum.R_CJ,  true),
         new TtInfo(Faktur.TT_CJ_RB1, 06,      false,           false,       false,      ZXC.VvSubModulEnum.R_CJ,  true),
         new TtInfo(Faktur.TT_CJ_RB2, 07,      false,           false,       false,      ZXC.VvSubModulEnum.R_CJ,  true),
         new TtInfo(Faktur.TT_CJ_MRZ, 08,      false,           false,       false,      ZXC.VvSubModulEnum.R_CJ,  true),

         // 18.12.2015: uvodimo po prvi put da vise od 1 TT-a moze imati isti ttSort (npr. INV, INM) 
         new TtInfo(Faktur.TT_INV,/*78*/ 09,      true ,           false,       false,      ZXC.VvSubModulEnum.R_INV),
         new TtInfo(Faktur.TT_INM,/*83*/ 09,      true ,           false,       false,      ZXC.VvSubModulEnum.R_INM),

       //new TtInfo(Faktur.TT_WFA,    09,      false,           false,       false,      ZXC.VvSubModulEnum.R_UFA),
       //new TtInfo(Faktur.TT_WRA,    09,      false,           false,       false,      ZXC.VvSubModulEnum.R_URA),
       //new TtInfo(Faktur.TT_WRM,    09,      false,           false,       false,      ZXC.VvSubModulEnum.R_URM),
       //new TtInfo(Faktur.TT_YFA,    09,      false,           false,       false,      ZXC.VvSubModulEnum.R_IFA),
       //new TtInfo(Faktur.TT_YRA,    09,      false,           false,       true ,      ZXC.VvSubModulEnum.R_IRA),
       //new TtInfo(Faktur.TT_YRM,    09,      false,           false,       true ,      ZXC.VvSubModulEnum.R_IRM),

         new TtInfo(Faktur.TT_PST,    10,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_PST),
         new TtInfo(Faktur.TT_PSM,    11,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_PSM),
         new TtInfo(Faktur.TT_UFA,    12,      false,   false/*true*/,       false,      ZXC.VvSubModulEnum.R_UFA),
         new TtInfo(Faktur.TT_UFM,    13,      false,   false/*true*/,       false,      ZXC.VvSubModulEnum.R_UFM),
         new TtInfo(Faktur.TT_URA,    14,      false,           true ,       false,      ZXC.VvSubModulEnum.R_URA),
         new TtInfo(Faktur.TT_URM,    15,      false,           true ,       false,      ZXC.VvSubModulEnum.R_URM),
       //new TtInfo(Faktur.TT_STU,    15,      false,           true ,       false,      ZXC.VvSubModulEnum.R_STU),
         new TtInfo(Faktur.TT_PRI,    16,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_PRI),
         new TtInfo(Faktur.TT_POU,    17,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_POU),
         new TtInfo(Faktur.TT_RVU,    17,      true ,   false/*true*/,       false,      ZXC.VvSubModulEnum.R_RVU),
         new TtInfo(Faktur.TT_POT,    18,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_PRI_POT),
         new TtInfo(Faktur.TT_KLK,    18,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_KLK),
         new TtInfo(Faktur.TT_KUL,    19,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_KIZ),
         new TtInfo(Faktur.TT_MSU,    20,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_MSI),
         new TtInfo(Faktur.TT_VMU,    21,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_VMI),
         new TtInfo(Faktur.TT_PUL,    22,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_PIZ),
         new TtInfo(Faktur.TT_PUK,    23,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_PIK),
         new TtInfo(Faktur.TT_SKU,    24,      true ,           false,       false,      ZXC.VvSubModulEnum.R_SKO),
         new TtInfo(Faktur.TT_PUX,    25,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_PIZ_P),
         new TtInfo(Faktur.TT_IOD,    26,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_IOD),
         new TtInfo(Faktur.TT_KKM,    27,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_KKM),
         new TtInfo(Faktur.TT_IPV,    28,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_IPV),
         new TtInfo(Faktur.TT_POV,    29,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_POV),
         new TtInfo(Faktur.TT_TMU,    30,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_TMK),
         new TtInfo(Faktur.TT_PUM,    31,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_PIM),
       //new TtInfo(Faktur.TT_ZPC,    32,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_ZPC),
         new TtInfo(Faktur.TT_NIV,    33,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_ZPC),
         new TtInfo(Faktur.TT_NUV,    33,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_ZPC),
         new TtInfo(Faktur.TT_NUP,    33,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_URA), // news in 2023 
         new TtInfo(Faktur.TT_TRM,    34,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_TRI),
         new TtInfo(Faktur.TT_MMU,    35,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_MMI),
         new TtInfo(Faktur.TT_MVU,    36,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_MVI),
/*!ttSo*/new TtInfo(Faktur.TT_ZKC,    37,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_ZPC), // isti ttSort !!! 
/*!ttSo*/new TtInfo(Faktur.TT_PIP,    37,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_PIP), // isti ttSort !!! 

                                                                                  
         new TtInfo(Faktur.TT_PIK,    38,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_PIK),
         new TtInfo(Faktur.TT_KIZ,    39,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_KIZ),
         new TtInfo(Faktur.TT_MSI,    40,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_MSI),
         new TtInfo(Faktur.TT_VMI,    41,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_VMI),
         new TtInfo(Faktur.TT_PIZ,    42,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_PIZ),
         new TtInfo(Faktur.TT_PIX,    43,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_PIZ_P),
         new TtInfo(Faktur.TT_IFA,    44,      false,           false,false/*true*/,     ZXC.VvSubModulEnum.R_IFA),
         new TtInfo(Faktur.TT_PIM,    45,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_PIM),
         new TtInfo(Faktur.TT_IRA,    46,      false,           false,       true ,      ZXC.VvSubModulEnum.R_IRA),
         new TtInfo(Faktur.TT_TRI,    47,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_TRI),
       //new TtInfo(Faktur.TT_IZD,    48,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_IZD),
         new TtInfo(Faktur.TT_IZD,    48,      true ,           false,       true ,      ZXC.IsSvDUHdomena ? ZXC.VvSubModulEnum.R_IZD_SVD : ZXC.VvSubModulEnum.R_IZD),
         new TtInfo(Faktur.TT_POI,    49,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_POI),
         new TtInfo(Faktur.TT_RVI,    49,      true ,           false,false/*true*/,     ZXC.VvSubModulEnum.R_RVI),
         new TtInfo(Faktur.TT_IRM,    50,      false,/*true!?!*/false,       true ,      ZXC.VvSubModulEnum.R_IRM),
         new TtInfo(Faktur.TT_IZM,    51,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_IZM),
         new TtInfo(Faktur.TT_SKI,    52,      true ,           false,       false,      ZXC.VvSubModulEnum.R_SKO),
         new TtInfo(Faktur.TT_UOD,    54,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_UOD),
         new TtInfo(Faktur.TT_UPV,    56,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_UPV),
       //new TtInfo(Faktur.TT_IMM,      ,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_IMM),
         new TtInfo(Faktur.TT_IMT,    58,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_IMT),
         new TtInfo(Faktur.TT_PPR,    59,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_PPR),
         new TtInfo(Faktur.TT_TMI,    60,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_TMK),
         new TtInfo(Faktur.TT_UPM,    61,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_UPM),
         new TtInfo(Faktur.TT_MMI,    62,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_MMI),
         new TtInfo(Faktur.TT_MVI,    63,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_MVI),
                                                                                  
         new TtInfo(Faktur.TT_OPN,    70,      false,           false,       false,      ZXC.VvSubModulEnum.R_OPN),
         new TtInfo(Faktur.TT_PON,    72,      false,           false,       false,      ZXC.VvSubModulEnum.R_PON),
         new TtInfo(Faktur.TT_PNM,    73,      false,           false,       false,      ZXC.VvSubModulEnum.R_PNM),
         new TtInfo(Faktur.TT_NRD,    74,      false,           false,       false,      ZXC.VvSubModulEnum.R_NRD),
         new TtInfo(Faktur.TT_NRU,    75,      false,           false,       false,      ZXC.VvSubModulEnum.R_NRU),
         new TtInfo(Faktur.TT_NRK,    76,      false,           false,       false,      ZXC.VvSubModulEnum.R_NRK),
         new TtInfo(Faktur.TT_NRS,    77,      false,           false,       false,      ZXC.VvSubModulEnum.R_NRS),
       //new TtInfo(Faktur.TT_INV,    78,      true ,           false,       false,      ZXC.VvSubModulEnum.R_INV),
         new TtInfo(Faktur.TT_NRM,    79,      false,           false,       false,      ZXC.VvSubModulEnum.R_NRM),

         new TtInfo(Faktur.TT_RNP,    80,      false /* ?! */,  false,       false,      ZXC.VvSubModulEnum.R_RNP    ),
         new TtInfo(Faktur.TT_RNS,    81,      false /* ?! */,  false,       false,      ZXC.VvSubModulEnum.R_RNS    ),
/*!ttSo*/new TtInfo(Faktur.TT_PRJ,    82,      false /* ?! */,  false,       false,      ZXC.VvSubModulEnum.R_PRJ    ), // isti ttSort !!! 
/*!ttSo*/new TtInfo(Faktur.TT_RNM,    82,      false /* ?! */,  false,       false,      ZXC.VvSubModulEnum.R_RNM    ), // isti ttSort !!! 
/*!ttSo*/new TtInfo(Faktur.TT_UGO,    82,      false /* ?! */,  false,       false,      ZXC.VvSubModulEnum.R_UGO    ), // isti ttSort !!! 

// PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG 
// PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG 
// PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG 

         new TtInfo(Faktur.TT_KUG,    82,      false /* ?! */,  false,       false,      ZXC.VvSubModulEnum.R_KUG_PTG), // isti ttSort !!! 
         
         new TtInfo(Faktur.TT_AU2,    34,      false /* ?! */,  true ,       false,      ZXC.VvSubModulEnum.R_ANU_PTG), // isti ttSort !!! 
         new TtInfo(Faktur.TT_UG2,    34,      false /* ?! */,  true ,       false,      ZXC.VvSubModulEnum.R_UGO_PTG), // isti ttSort !!! 
         new TtInfo(Faktur.TT_DI2,    34,      false,           true ,       false,      ZXC.VvSubModulEnum.R_DOD_PTG),
         new TtInfo(Faktur.TT_PV2,    34,      false /* ?! */,  true ,       false,      ZXC.VvSubModulEnum.R_PVR_PTG), // isti ttSort !!! 
       //new TtInfo(Faktur.TT_PD2,    34,      false /* ?! */,  true ,       false,      ZXC.VvSubModulEnum.R_PVD_PTG), // isti ttSort !!! 
         new TtInfo(Faktur.TT_MOU,  /*34*/85,  false /* ?! */,  true ,       false,      ZXC.VvSubModulEnum.R_MOD_PTG), // isti ttSort !!! 
         
         new TtInfo(Faktur.TT_MOD,    85,      true  /* ?! */,  false,       false,      ZXC.VvSubModulEnum.R_MOD_PTG), // isti ttSort !!! 
         new TtInfo(Faktur.TT_MOC,    85,      false /* ?! */,  false,       false,      ZXC.VvSubModulEnum.R_MOD_PTG), // isti ttSort !!! 
         new TtInfo(Faktur.TT_MOS,    85,      false /* ?! */,  false,       false,      ZXC.VvSubModulEnum.R_MOD_PTG), // isti ttSort !!! 
         
         new TtInfo(Faktur.TT_AUN,    91,      false /* ?! */,  false,       true ,      ZXC.VvSubModulEnum.R_ANU_PTG), // isti ttSort !!! 
         new TtInfo(Faktur.TT_UGN,    91,      false /* ?! */,  false,       true ,      ZXC.VvSubModulEnum.R_UGO_PTG), // isti ttSort !!! 
         new TtInfo(Faktur.TT_DIZ,    91,      false,           false,       true ,      ZXC.VvSubModulEnum.R_DOD_PTG),
         new TtInfo(Faktur.TT_PVR,    91,      false /* ?! */,  false,       true ,      ZXC.VvSubModulEnum.R_PVR_PTG), // isti ttSort !!! 
       //new TtInfo(Faktur.TT_PVD,    91,      false /* ?! */,  false,       true ,      ZXC.VvSubModulEnum.R_PVD_PTG), // isti ttSort !!! 
         new TtInfo(Faktur.TT_MOI,  /*91*/85,  false /* ?! */,  false,       true ,      ZXC.VvSubModulEnum.R_MOD_PTG), // isti ttSort !!! 

         new TtInfo(Faktur.TT_MPI,    40,      true ,           false,       true ,      ZXC.VvSubModulEnum.R_MSI_PTG),
         new TtInfo(Faktur.TT_MPU,    20,      true ,           true ,       false,      ZXC.VvSubModulEnum.R_MSI_PTG),

         new TtInfo(Faktur.TT_ZIZ,    93,      false,           false,       true ,      ZXC.VvSubModulEnum.R_ZIZ_PTG),
         new TtInfo(Faktur.TT_ZI2,    21,      false,           true ,       false,      ZXC.VvSubModulEnum.R_ZIZ_PTG),
       //new TtInfo(Faktur.TT_ZUL,    22,      false,           true ,       false,      ZXC.VvSubModulEnum.R_ZIZ_PTG), // ili ??? !!!
       //new TtInfo(Faktur.TT_ZU2,    92,      false,           false,       true ,      ZXC.VvSubModulEnum.R_ZIZ_PTG), // ili ??? !!!
         new TtInfo(Faktur.TT_ZU2,    22,      false,           true ,       false,      ZXC.VvSubModulEnum.R_ZIZ_PTG), // ili ??? !!!
         new TtInfo(Faktur.TT_ZUL,    92,      false,           false,       true ,      ZXC.VvSubModulEnum.R_ZIZ_PTG), // ili ??? !!!

// PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG 
// PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG 
// PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG PTG 

       //new TtInfo(Faktur.TT_INM,    83,      true ,           false,       false,      ZXC.VvSubModulEnum.R_INM),
         new TtInfo(Faktur.TT_UPA,    84,      false,   false/*true*/,       false,      ZXC.VvSubModulEnum.R_UPA),
/*!ttSo*/new TtInfo(Faktur.TT_RNU,    84,      false /* ?! */,  false,       false,      ZXC.VvSubModulEnum.R_RNM), // isti ttSort !!! 
         new TtInfo(Faktur.TT_ZAH,    84,      false,           false,       false,      ZXC.VvSubModulEnum.R_ZAH_SVD),

         new TtInfo(Faktur.TT_BOR,    85,      false,           false,       false,      ZXC.VvSubModulEnum.R_BOR),
         new TtInfo(Faktur.TT_RNZ,    85,      false,           false,       false,      ZXC.VvSubModulEnum.R_RNZ),

         new TtInfo(Faktur.TT_UPL   , 90,      false,           false,       false,      ZXC.VvSubModulEnum.R_UPL),
         new TtInfo(Faktur.TT_BUP   , 91,      true ,           false,       false,      ZXC.VvSubModulEnum.R_BUP),
         new TtInfo(Faktur.TT_ABU   , 91,      true ,           false,       false,      ZXC.VvSubModulEnum.R_ABU),
         new TtInfo(Faktur.TT_ISP   , 92,      false,           false,       false,      ZXC.VvSubModulEnum.R_ISP),
         new TtInfo(Faktur.TT_NOR   , 93,      false,           false,       false,      ZXC.VvSubModulEnum.R_NOR), 
         new TtInfo(Faktur.TT_BIS   , 94,      true ,           false,       false,      ZXC.VvSubModulEnum.R_BIS),
         new TtInfo(Faktur.TT_ABI   , 94,      true ,           false,       false,      ZXC.VvSubModulEnum.R_ABI),
       //new TtInfo(Faktur.TT_NOM   , 94,      false,           false,       false,      ZXC.VvSubModulEnum.R_NOM), 
         new TtInfo(Faktur.TT_ZAR   , 85,      true ,           false,       false,      ZXC.VvSubModulEnum.R_ZAR),

      };

      ZXC.RiskTT = ZXC.TtInfoArray.ToDictionary(tt => tt.TheTT);

   }

   #endregion RISK Specials

   #region SaveTo & NewFrom VvXmlDR 

   private void Save_ToVvXmlDR_OnClick(object sender, EventArgs e)
   {
#region FileDialog

      SaveFileDialog saveFileDialog = new SaveFileDialog();

      saveFileDialog.InitialDirectory = ZXC.VvSerializedDR_DirectoryName;

      saveFileDialog.Title = "Kreiranje XML datoteke 'Serialized " + TheVvDataRecord.VirtualRecordName2 + " - Vektor Data Record'";
      saveFileDialog.Filter = "XML Serialized " + TheVvDataRecord.VirtualRecordName2 + "|" + TheVvDataRecord.Suggested_vvXmlDR_fileName_root + "*.xml|Sve Datoteke (*.*)|*.*";
      //saveFileDialog.Filter           = "(*" + thisDocumentID + "*)|" + "*" + thisDocumentID + "*" + "|All files (*.*)|*.*";
      saveFileDialog.FilterIndex = 1;
      saveFileDialog.RestoreDirectory = true;
      saveFileDialog.DefaultExt = "xml";

    //string suggestedFileName = faktur_rec.TT_And_TtNum + "-" + faktur_rec.DokDate_DDMMYY + "-" + faktur_rec.KupdobTK/*KupdobName*/;
    //string suggestedFileName = "vv" + TheVvDataRecord.VirtualRecordName2 + "_" + TheVvDataRecord.VirtualIDstring;
      string suggestedFileName = TheVvDataRecord.Suggested_vvXmlDR_fileName;

      saveFileDialog.FileName = suggestedFileName.IsEmpty() ? "" : suggestedFileName + "." + saveFileDialog.DefaultExt;

      Cursor.Current = Cursors.Default;

      if(saveFileDialog.ShowDialog() != DialogResult.OK)
      {
         saveFileDialog.Dispose(); // !!! 
         Cursor.Current = Cursors.Default;
         return;
      }

      Cursor.Current = Cursors.WaitCursor;

      string fullPath_XML_FileName = saveFileDialog.FileName;
      System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(fullPath_XML_FileName);

      string fileNameOnly = dInfo.Name;
      string directoryName = fullPath_XML_FileName.Substring(0, fullPath_XML_FileName.Length - (fileNameOnly.Length + 1));
    //ZXC.VvSerializedDR_DirectoryName = directoryName;

      saveFileDialog.Dispose(); // !!! 

#endregion FileDialog

      TheVvDataRecord.SaveSerialized_VvDataRecord_ToXmlFile(fullPath_XML_FileName, false);

   }

   private bool New_FromVvXmlDR_InProgress = false;
   private VvDataRecord prevTheVvDataRecord;
   private void New_FromVvXmlDR_OnClick(object sender, EventArgs e)
   {
#region FileDialog

      OpenFileDialog openFileDialog   = new OpenFileDialog();
      openFileDialog.InitialDirectory = ZXC.VvSerializedDR_DirectoryName;
      openFileDialog.Filter           = "XML Serialized " + TheVvDataRecord.VirtualRecordName2 + "|*" + TheVvDataRecord.Suggested_vvXmlDR_fileName_root + "*.xml|Sve Datoteke (*.*)|*.*";
    //openFileDialog.Filter           = "(*" + thisDocumentID + "*)|" + "*" + thisDocumentID + "*" + "|All files (*.*)|*.*";
      openFileDialog.FilterIndex      = 1;
      openFileDialog.RestoreDirectory = true;

      if(openFileDialog.ShowDialog() != DialogResult.OK)
      {
         openFileDialog.Dispose(); // !!! 
         return;
      }

      string fullPathName            = openFileDialog.FileName;
      System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(fullPathName);

      string fileName = dInfo.Name;
      string directoryName = fullPathName.Substring(0, fullPathName.Length - (fileName.Length + 1));
    //ZXC.VvSerializedDR_DirectoryName = directoryName;

      openFileDialog.Dispose(); // !!! 

      Cursor.Current = Cursors.WaitCursor;
      //SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}");

#endregion FileDialog

      prevTheVvDataRecord = TheVvDataRecord.CreateNewRecordAndCloneItComplete(); // make BACKUP 

      /*VvDataRecord daXmlVvDataRecord*/TheVvDataRecord = TheVvDataRecord.Deserialize_VvDataRecord_FromXmlFile(fullPathName);

      TheVvRecordUC.PutFields(/*daXmlVvDataRecord*/TheVvDataRecord);

      //OpenForWriteActions(ZXC.WriteMode.Add);

      TheVvTabPage.WriteMode = ZXC.WriteMode.Add;
      VvHamper.Open_Close_Fields_ForWriting(TheVvTabPage, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvRecordUC);
      SetVvMenuEnabledOrDisabled_Explicitly("SAV", true);
      SetVvMenuEnabledOrDisabled_Explicitly("ESC", true);

      New_FromVvXmlDR_InProgress = true;

      //NewRecord_OnClick("New_FromVvXml_OnClick"/*daXmlVvDataRecord*/, EventArgs.Empty);
   }

   private string Get_NewestVvXmlDR_FullPathFileName(IEnumerable<ZXC.VvUtilDataPackage> thisRecID_Files, string expected_vvXmlDR_recovery_directory)
   {
      //throw new NotImplementedException();
      string cleanFileName =  thisRecID_Files.Last().TheStr1;
      return Path.Combine(expected_vvXmlDR_recovery_directory, cleanFileName);
   }

   private ZXC.VvUtilDataPackage Set_VvXmlDR_FileInfo_UDP(string fileName)
   {
      // int      recID            TheInt  
      // string   fileName         TheStr1 
      // string   actionType       TheStr2 
      // string   recordName       TheStr3 
      // DateTime dateFromFileName TheDate 

      ZXC.VvUtilDataPackage fileInfo = new ZXC.VvUtilDataPackage();

      fileInfo.TheStr1 = fileName;

      string[] splitters = fileName.Split("_".ToCharArray());

      if(splitters.Length > 1) fileInfo.TheInt  =               ZXC.ValOrZero_Int(splitters[1])                           ; // recID      
      if(splitters.Length > 2) fileInfo.TheStr2 =                                 splitters[2]                            ; // actionType 
      if(splitters.Length > 3) fileInfo.TheStr3 = splitters[3].StartsWith("vv") ? splitters[3].Replace("vv", "") : "error"; // recordName 

      if(splitters.Length > 2)
      {
         fileInfo.TheDate = ZXC.ValOr_01010001_DateTime_Import_yyyyMMdd_HHmmss_Format(splitters[splitters.Length - 2] + "_" + splitters[splitters.Length - 1].Replace(".xml", ""));
      }

      if(fileInfo.TheInt.IsZero() || fileInfo.TheStr3 == "error" || fileInfo.TheDate.IsEmpty()) return new ZXC.VvUtilDataPackage();

      return fileInfo;
   }

   private ZXC.VvUtilDataPackage Set_RnX_FileInfo_UDP(string fileName)
   {
      // string   vvTD TheStr2 
      // string   vvid TheStr3 
      // DateTime vvTS TheDate 

      ZXC.VvUtilDataPackage fileInfo = new ZXC.VvUtilDataPackage();

      fileInfo.TheStr1 = fileName;

      string[] splitters = fileName.Split("_".ToCharArray());

      if(splitters.Length > 0) fileInfo.TheStr2 =                                 splitters[0]                            ; // vvTD 
      if(splitters.Length > 1) fileInfo.TheStr3 =                                 splitters[1]                            ; // vvid 

      if(splitters.Length > 2)
      {
         fileInfo.TheDate = ZXC.ValOr_01010001_DateTime_Import_yyyyMMdd_HHmmss_Format(splitters[2] + "_" + splitters[3].Replace(".xml", ""));
      }

      if(fileInfo.TheDate.IsEmpty()) return new ZXC.VvUtilDataPackage();

      return fileInfo;
   }

   private ZXC.VvUtilDataPackage Set_VektorLog_FileInfo_UDP(string fileName)
   {
      // DateTime vvTS TheDate 

      ZXC.VvUtilDataPackage fileInfo = new ZXC.VvUtilDataPackage();

      fileInfo.TheStr1 = fileName;

      string[] splitters = fileName.Split("_".ToCharArray());

      if(splitters.Length > 0)
      {
         fileInfo.TheDate = ZXC.ValOr_01010001_DateTime_Import_yyyyMMdd_HHmmss_Format(splitters[1] + "_" + splitters[2].Replace(".TXT", ""));
      }

      if(fileInfo.TheDate.IsEmpty()) return new ZXC.VvUtilDataPackage();

      return fileInfo;
   }

   private void CHECK_is_VvXmlDR_LastDocumentMissing_ShitHappendDetection()
   {
      if(ZXC.IsPCTOGO) return; // Nemoj ovo raditi za PCTOGO 

      ZXC.SetMainDbConnDatabaseName(VvSQL.GetDbNameForThisTableName(Faktur.recordName)); // ?! jel' bu ovo kaj zasralo?! 

      // *** // // tek 01.07.2021: 
      /**/ bool fakturTableExists = VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, TheDbConnection.Database, Faktur.recordName);
      /**/
      /**/ if(fakturTableExists == false) return;
      // *** //

      if(ZXC.IsTEXTHOshop /*&& TheVvUC is IRMDUC*/)
      {
         List<ZXC.VvUtilDataPackage> PrihodTT_Skladista_InUse_List = FakturDao.GetPrihodTT_Skladista_InUse(TheDbConnection);

         PrihodTT_Skladista_InUse_List.RemoveAll(udp => udp.TheStr1 != Faktur.TT_IRM);

         // 01.01.2020: !!! idijote !!! 
         if(PrihodTT_Skladista_InUse_List.IsEmpty()) return;

         string tt      = Faktur.TT_IRM;
         string skladCD = PrihodTT_Skladista_InUse_List.First().TheStr2;

         uint nextTtNum_LAN = ZXC.FakturDao.GetNextTtNum(    TheDbConnection   , tt, skladCD);

         // 04.12.2020: TH nemre uci u program ako je skylab down pa opkoljavamo sa try / catch
         uint nextTtNum_SKY = nextTtNum_LAN;
         try
         {
            /*uint*/ nextTtNum_SKY = ZXC.FakturDao.GetNextTtNum(ZXC.TheSkyDbConnection, tt, skladCD);
         }
         catch // meaning; SKY is DOWN! 
         {
            // ostavi da su jednaki nextTtNum_SKY = nextTtNum_LAN
         }
       //bool shitHappend = nextTtNum_LAN != nextTtNum_SKY;
         bool shitHappend = nextTtNum_LAN  < nextTtNum_SKY;

         // 02.10.2020:
       //if(shitHappend                     ) // !!! ALERT !!!  ALERT !!!  ALERT !!! 
         if(shitHappend && !ZXC.ThisIsQUKATZ) // !!! ALERT !!!  ALERT !!!  ALERT !!! 
         {
            ZXC.VvXmlDR_LastDocumentMissing_AlertRaised = true; 

            string message = "UPOZORENJE!\n\nVAŽNO!\n\nKONTAKTIRAJTE SUPPORT!\n\nNE DODAJTE NOVE DOKUMENTE!\n\n\n\nServer računalo je vjerovatno ostalo bez napajanja\nte je uslijed toga zadnje dodan dokument nestao.\n\n" +
               /*((VvDocumLikeRecord)existingVvDataRecord).VirtualTT*/Faktur.TT_IRM + "-" + /*((VvDocumLikeRecord)existingVvDataRecord).VirtualTTnum*/nextTtNum_LAN;

            ZXC.aim_emsg(MessageBoxIcon.Stop, message);
            ZXC.aim_emsg(MessageBoxIcon.Warning, "KONTAKTIRAJTE SUPPORT ODMAH!\n\nnextTtNum_LAN: {0}\n\nnextTtNum_SKY: {1}", nextTtNum_LAN, nextTtNum_SKY);
         }
      }

      else // NOT TEXTHOshop 
      {
         List<ZXC.VvUtilDataPackage> PrihodTT_Skladista_InUse_List = FakturDao.GetPrihodTT_Skladista_InUse(TheDbConnection);

         foreach(ZXC.VvUtilDataPackage udp in PrihodTT_Skladista_InUse_List)
         {
            string tt      = udp.TheStr1;
            string skladCD = udp.TheStr2;

            Faktur faktur_rec = new Faktur() { TT = tt, SkladCD = skladCD };

            uint nextTtNum = ZXC.FakturDao.GetNextTtNum(TheDbConnection, tt, skladCD);

            bool xmlReadOK;

            VvDataRecord existingVvDataRecord = ZXC.AutoCreated_VvXmlDocumentDR_Exists(faktur_rec, out xmlReadOK, nextTtNum /*- 1*/); // ovo '- 1' ti je za isprobavanje 

          //bool shitHappend1 = existingVvDataRecord != null;
            bool shitHappend1 = existingVvDataRecord != null && ((Faktur)existingVvDataRecord).TT == tt && ((Faktur)existingVvDataRecord).SkladCD == skladCD;

            // 13.10.2022: 
          //if(shitHappend1              ) // !!! ALERT !!!  ALERT !!!  ALERT !!! 
            if(shitHappend1 || !xmlReadOK) // !!! ALERT !!!  ALERT !!!  ALERT !!! 
            {
               ZXC.VvXmlDR_LastDocumentMissing_AlertRaised = true;

#region Da smo na FakturDUC-u onda bi islo ovo:
               if(xmlReadOK)
               { 
                  OpenNew_Record_TabPage_wInitialRecord(GetSubModulXY(FakturDUC.GetVvSubModulEnum_ForTT(tt)), existingVvDataRecord);

                  TheVvDataRecord = existingVvDataRecord;

                  TheVvRecordUC.PutFields(TheVvDataRecord);
                  
                  TheVvTabPage.WriteMode = ZXC.WriteMode.Add;
                  TheVvTabPage.thisIsFirstAppereance = true;

                  VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvRecordUC);
                  SetVvMenuEnabledOrDisabled_Explicitly("SAV", true);
                  SetVvMenuEnabledOrDisabled_Explicitly("ESC", true);
               }
#endregion Da smo na FakturDUC-u onda bi islo ovo:

               if(xmlReadOK)
               {
                  string message = "UPOZORENJE!\n\nVAŽNO!\n\nKONTAKTIRAJTE SUPPORT!\n\nNE DODAJTE NOVE DOKUMENTE!\n\n\n\nServer računalo je vjerovatno ostalo bez napajanja\nte je uslijed toga zadnje dodan dokument nestao.\n\n" +
                     ((VvDocumLikeRecord)existingVvDataRecord).VirtualTT + "-" + ((VvDocumLikeRecord)existingVvDataRecord).VirtualTTnum;

                  ZXC.aim_emsg(MessageBoxIcon.Stop, message);
                  ZXC.aim_emsg(MessageBoxIcon.Warning, "Na ekranu je prikazan 'recovery' (spašen) dokument te ga se po potrebi može usnimiti, ali tek nakon što\n\nKONTAKTIRATE SUPPORT ODMAH!");
               }
               else
               {
                  string message = "UPOZORENJE!\n\nVAŽNO!\n\nKONTAKTIRAJTE SUPPORT!\n\nNE DODAJTE NOVE DOKUMENTE!\n\n\n\nServer računalo je vjerovatno ostalo bez napajanja\nte je uslijed toga zadnje dodan dokument nestao.\n\n" +
                     "XML datoteka zadnjeg zapisa je oštećena - nečitljiva!!!";

                  ZXC.aim_emsg(MessageBoxIcon.Stop, message);
               }
            } // if(existingVvDataRecord != null) // !!! ALERT !!!  ALERT !!!  ALERT !!! 

         } // foreach(ZXC.VvUtilDataPackage udp in PrihodTT_Skladista_InUse_List) 

      } // NOT TEXTHOshop: 

   }

   private void Cleanup_VvXmlDR_ToTarGZ()
   {
      if(ZXC.VvXmlDR_LastDocumentMissing_AlertRaised) return;

      string gZipPreffix      = "VvXmlDR_";
      string tarDirectoryName = ZXC.VvSerializedDR_DirectoryName;

      List<string> autoCreated_VvXmlDR_fileNames = ZXC.GetFileNames_AutoCreated_VvXmlDR_ToAdd_ToTargzip(tarDirectoryName);

      if(autoCreated_VvXmlDR_fileNames.IsEmpty()) return;

      List<ZXC.VvUtilDataPackage> fileInfo_UDP_List = new List<ZXC.VvUtilDataPackage>();

      ZXC.VvUtilDataPackage fileInfo_UDP;

      // create fileInfo_UDP_List by parsing fileNames 
      foreach(string fullPathFileName in autoCreated_VvXmlDR_fileNames)
      {
         // int      recID            TheInt  
         // string   fileName         TheStr1 
         // string   actionType       TheStr2 
         // string   recordName       TheStr3 
         // DateTime dateFromFileName TheDate 

         fileInfo_UDP = Set_VvXmlDR_FileInfo_UDP(fullPathFileName.Replace(tarDirectoryName + @"\", ""));

         // do it only for 'yesterday' and older files. Do not for today.
         if(fileInfo_UDP.TheInt.NotZero() && fileInfo_UDP.TheDate.Date < DateTime.Today.Date)
         {
            fileInfo_UDP_List.Add(fileInfo_UDP);
         }
      }

      string tarFilename        ;
      string tarFilenameFullPath;

      var fileInfo_UDP_DateGroups = fileInfo_UDP_List.GroupBy(fInfo => fInfo.TheDate.Date);
      List<string> oneDayfileInfo_fileName_List;

      // do the job for each date 
      foreach(/*List<ZXC.VvUtilDataPackage>*/ var oneDayfileInfo_UDP_List in fileInfo_UDP_DateGroups)
      {
         tarFilename         = gZipPreffix + ZXC.PUG_ID + "_" + oneDayfileInfo_UDP_List.Key.ToString(ZXC.VvDateYyyyMmDdFormat) + ".tar.gz";
         tarFilenameFullPath = Path.Combine(tarDirectoryName, tarFilename);

         oneDayfileInfo_fileName_List = oneDayfileInfo_UDP_List.Select(udp => Path.Combine(tarDirectoryName, udp.TheStr1)).ToList();

         if(File.Exists(tarFilenameFullPath) == false) // !!! 
         {
            ZXC.CreateAndAddFilesToTarThenDeleteFiles(tarFilenameFullPath, oneDayfileInfo_fileName_List);

            ZXC.aim_log("AutoCreated_VvXmlDR_ToTargzip: file {0} - {1} records.", tarFilenameFullPath, oneDayfileInfo_fileName_List.Count);
         }
      }

      ZXC.aim_log("AutoCreated_VvXmlDR_ToTargzip: total {0} records.", fileInfo_UDP_List.Count);

      //ZXC.ExtractTGZ(targzFileName); primjer za Unzip-anje! 

   }

   private void Cleanup_VektorLog_ToTarGZ()
   {
      string gZipPreffix      = ZXC.vv_PRODUCT_Name + "Log_";
      string tarDirectoryName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false);

      List<string> autoCreated_VektorLog_fileNames = ZXC.GetFileNames_AutoCreated_VektorLog_ToAdd_ToTargzip(gZipPreffix, tarDirectoryName);

      if(autoCreated_VektorLog_fileNames.IsEmpty()) return;

      List<ZXC.VvUtilDataPackage> fileInfo_UDP_List = new List<ZXC.VvUtilDataPackage>();

      ZXC.VvUtilDataPackage fileInfo_UDP;

      // create fileInfo_UDP_List by parsing fileNames 
      foreach(string fullPathFileName in autoCreated_VektorLog_fileNames)
      {
         // DateTime dateFromFileName TheDate 

         fileInfo_UDP = Set_VektorLog_FileInfo_UDP(fullPathFileName.Replace(tarDirectoryName + @"\", ""));

         // do it only for 'yesterday' and older files. Do not for today.
         if(fileInfo_UDP.TheDate.NotEmpty() && fileInfo_UDP.TheDate.Date < DateTime.Today.Date)
         {
            fileInfo_UDP_List.Add(fileInfo_UDP);
         }
      }

      string tarFilename        ;
      string tarFilenameFullPath;

      var fileInfo_UDP_DateGroups = fileInfo_UDP_List.GroupBy(fInfo => fInfo.TheDate.Date);
      List<string> oneDayfileInfo_fileName_List;

      // do the job for each date 
      foreach(/*List<ZXC.VvUtilDataPackage>*/ var oneDayfileInfo_UDP_List in fileInfo_UDP_DateGroups)
      {
         tarFilename         = gZipPreffix + ZXC.PUG_ID + "_" + oneDayfileInfo_UDP_List.Key.ToString(ZXC.VvDateYyyyMmDdFormat) + ".tar.gz";
         tarFilenameFullPath = Path.Combine(tarDirectoryName, tarFilename);

         oneDayfileInfo_fileName_List = oneDayfileInfo_UDP_List.Select(udp => Path.Combine(tarDirectoryName, udp.TheStr1)).ToList();

         if(File.Exists(tarFilenameFullPath) == false) // !!! 
         {
            ZXC.CreateAndAddFilesToTarThenDeleteFiles(tarFilenameFullPath, oneDayfileInfo_fileName_List);

            ZXC.aim_log("AutoCreated_VektorLog_ToTargzip: file {0} - {1} records.", tarFilenameFullPath, oneDayfileInfo_fileName_List.Count);
         }
      }

      ZXC.aim_log("AutoCreated_VektorLog_ToTargzip: total {0} records.", fileInfo_UDP_List.Count);

      //ZXC.ExtractTGZ(targzFileName); primjer za Unzip-anje! 

   }

   // 20.05.2019: !!! vvTH_VvJanitor ode u KRIVI directory u metodi: VvForm.GetLocalDirectoryForVvFile(@"FiskXML Zahtjev"));
   public void Cleanup_Fiskal_RnOxml_RnZxml_ToTarGZ(string gZipPreffix, string tarDirectoryName)
   {
      List<string> autoCreated_VvXmlRnX_fileNames = ZXC.GetFileNames_AutoCreated_VvXml_RnX_IRM_ToAdd_ToTargzip(tarDirectoryName, gZipPreffix, /*DateTime.Today.Day*/ZXC.Yesterday.Day);

      if(autoCreated_VvXmlRnX_fileNames.IsEmpty()) return;

      List<ZXC.VvUtilDataPackage> fileInfo_UDP_List = new List<ZXC.VvUtilDataPackage>();

      ZXC.VvUtilDataPackage fileInfo_UDP;

      // create fileInfo_UDP_List by parsing fileNames 
      foreach(string fullPathFileName in autoCreated_VvXmlRnX_fileNames)
      {
         // string   vvTD TheStr2 
         // string   vvid TheStr3 
         // DateTime vvTS TheDate 

         fileInfo_UDP = Set_RnX_FileInfo_UDP(fullPathFileName.Replace(tarDirectoryName + @"\", ""));

         // do it only for 'yesterday' and older files. Do not for today.
         if(fileInfo_UDP.TheDate.NotEmpty() && fileInfo_UDP.TheDate.Date < DateTime.Today.Date)
         {
            fileInfo_UDP_List.Add(fileInfo_UDP);
         }
      }

      string tarFilename        ;
      string tarFilenameFullPath;

      var fileInfo_UDP_DateGroups = fileInfo_UDP_List.GroupBy(fInfo => fInfo.TheDate.Date);
      List<string> oneDayfileInfo_fileName_List;

      // do the job for each date 
      foreach(/*List<ZXC.VvUtilDataPackage>*/ var oneDayfileInfo_UDP_List in fileInfo_UDP_DateGroups)
      {
         tarFilename         = gZipPreffix + ZXC.PUG_ID + "_" + oneDayfileInfo_UDP_List.Key.ToString(ZXC.VvDateYyyyMmDdFormat) + ".tar.gz";
         tarFilenameFullPath = Path.Combine(tarDirectoryName, tarFilename);

         oneDayfileInfo_fileName_List = oneDayfileInfo_UDP_List.Select(udp => Path.Combine(tarDirectoryName, udp.TheStr1)).ToList();

         if(File.Exists(tarFilenameFullPath) == false) // !!! 
         {
            ZXC.CreateAndAddFilesToTarThenDeleteFiles(tarFilenameFullPath, oneDayfileInfo_fileName_List);
         }

         ZXC.aim_log("AutoCreated_VvXml{2}ToTargzip: file {0} - {1} records.", tarFilenameFullPath, oneDayfileInfo_fileName_List.Count, gZipPreffix);
      }

      ZXC.aim_log("AutoCreated_VvXml{1}ToTargzip: total {0} records.", fileInfo_UDP_List.Count, gZipPreffix);

   }

   private void Restore_FromVvXmlDR_OnClick(object sender, EventArgs e)
   {
#region BlaBla

      VvBackup_FromVvXmlDRDlg dlg = new VvBackup_FromVvXmlDRDlg();

      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      DateTime lastBackup_DateAndTIme = dlg.Fld_DateTime;

      dlg.Dispose();

#endregion BlaBla

      string expected_vvXmlDR_recovery_directory = Path.Combine(ZXC.VvSerializedDR_DirectoryName, "vvRecovery");

      ShowRecoveryDirectoryFullInfo(lastBackup_DateAndTIme, expected_vvXmlDR_recovery_directory);

      string currentVvRecordUC_Type = TheVvDataRecord.GetType().Name;

    //string searchPattern1 = VvDataRecord.Auto_vvXmlDR_preffix + "*vv*_"                               + ZXC.CURR_prjkt_rec.Ticker + "_*.xml";
    //string searchPattern1 = VvDataRecord.Auto_vvXmlDR_preffix + "*vv*_"                               + ZXC.PUG_ID                + "_*.xml";
      string searchPattern1 = VvDataRecord.Auto_vvXmlDR_preffix + "*vv" + currentVvRecordUC_Type + "*_" + ZXC.PUG_ID                + "_*.xml";

      string searchPattern2 = searchPattern1.Replace(VvDataRecord.Auto_vvXmlDR_preffix, VvDataRecord.Auto_vvXmlDR_preffixInvalidated);

      if(Directory.Exists(expected_vvXmlDR_recovery_directory) == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Directory\n\n[{0}]\n\nne postoji.\n\nKreirajte directory i u njega napunite sve VvXmlDR datoteke na raspolaganju.\n\n(Unija sa svih client-a.)", expected_vvXmlDR_recovery_directory);
         return;
      }

      DirectoryInfo diInfo = new DirectoryInfo(expected_vvXmlDR_recovery_directory); // D:\MyDocuments\Viper.NET\Vektor\VvSerializedDR\vvRecovery 

      FileInfo[] fiArray1 = diInfo.GetFiles(searchPattern1);

      // !!! 'CreationTime' ti se sjebe kod copy paste, pa moras preko DateModified tj 'LastWriteTime' 
      List<string> fileNames = fiArray1.Where(fi => fi./*CreationTime*/LastWriteTime > lastBackup_DateAndTIme).Select(fi => fi.FullName).ToList();

      // searchPattern2 Addition: 
      FileInfo[] fiArray2 = diInfo.GetFiles(searchPattern2);
      fileNames.AddRange(fiArray2.Where(fi => fi./*CreationTime*/LastWriteTime > lastBackup_DateAndTIme).Select(fi => fi.FullName).ToList());


      if(fileNames.IsEmpty()) return; // der'z notin tu du 

      ZXC.Restore_FromVvXmlDR_InProgress = true;

      VvDataRecord theArhivedVvDataRecord;

      List<ZXC.VvUtilDataPackage> fileInfo_UDP_List = new List<ZXC.VvUtilDataPackage>();

      ZXC.VvUtilDataPackage fileInfo_UDP;

      if(TheVvUC is FakturDUC) ZXC.RISK_DisableCacheTemporarily = true;

      foreach(string fullPathFileName in fileNames)
      {
         string fileName_clean = fullPathFileName.Replace(expected_vvXmlDR_recovery_directory + @"\", "");

         fileInfo_UDP = Set_VvXmlDR_FileInfo_UDP(/*fullPathFileName*/ fileName_clean);

         if(fileInfo_UDP.TheInt.NotZero())
         {
            fileInfo_UDP_List.Add(fileInfo_UDP);
         }
      }

      string newestVvXmlDR_FullPathFileName;
      List<ZXC.DBactionForSrvRecID> vvDR_RecIDactions;
      bool isADD, isRWT, isDEL;

      var vvDR_RecIDs_DistinctList = fileInfo_UDP_List.Select(udp => udp.TheInt).Distinct().OrderBy(recID => recID); // sort by RecID! 

      vvDR_RecIDactions = new List<ZXC.DBactionForSrvRecID>();

      foreach(uint vvDR_recID in vvDR_RecIDs_DistinctList)
      {
         var thisRecID_Files = fileInfo_UDP_List.Where(udp => udp.TheInt == vvDR_recID);

         isADD = thisRecID_Files.Any(udp => udp.TheStr2 == /*VvSQL.DB_RW_ActionType.*/"ADD");
         isRWT = thisRecID_Files.Any(udp => udp.TheStr2 == /*VvSQL.DB_RW_ActionType.*/"RWT");
         isDEL = thisRecID_Files.Any(udp => udp.TheStr2 == /*VvSQL.DB_RW_ActionType.*/"DEL");

         VvSQL.DB_RW_ActionType actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.NONE;

         if( isADD && !isRWT && !isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.ADD ;
         if( isADD &&  isRWT && !isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.ADD ;
         if(!isADD &&  isRWT && !isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.RWT ;
         if(!isADD &&  isRWT &&  isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.DEL ;
         if(!isADD && !isRWT &&  isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.DEL ;
         if( isADD && !isRWT &&  isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.NONE;
         if( isADD &&  isRWT &&  isDEL) actionToTakeOnSyncTaker = VvSQL.DB_RW_ActionType.NONE;

         newestVvXmlDR_FullPathFileName = Get_NewestVvXmlDR_FullPathFileName(thisRecID_Files, expected_vvXmlDR_recovery_directory);

         if(actionToTakeOnSyncTaker != VvSQL.DB_RW_ActionType.NONE)
            {
             //vvDR_RecIDactions.Add(new ZXC.DBactionForSrvRecID() { recID = vvDR_recID, action = actionToTakeOnSyncTaker                                                            } );
               vvDR_RecIDactions.Add(new ZXC.DBactionForSrvRecID() { recID = vvDR_recID, action = actionToTakeOnSyncTaker, VvXmlDR_FullPathFileName = newestVvXmlDR_FullPathFileName } );
            }

      } // foreach(uint vvDR_recID in vvDR_RecIDs_DistinctList)

      VvDataRecord deserialized_vvDataRecord;

      foreach(ZXC.DBactionForSrvRecID recIDaction in vvDR_RecIDactions)
      {
         deserialized_vvDataRecord = TheVvDataRecord.Deserialize_VvDataRecord_FromXmlFile(recIDaction.VvXmlDR_FullPathFileName);

         deserialized_vvDataRecord.TurnNullValuesToEmptyString(); // !!! 

         switch(recIDaction.action)
         {
            case VvSQL.DB_RW_ActionType.ADD: // ####################################################################################################################### 

               deserialized_vvDataRecord.VvDao.ADDREC(TheDbConnection, deserialized_vvDataRecord);

               break;

            case VvSQL.DB_RW_ActionType.RWT: // ####################################################################################################################### 

#region deserialized_vvDataRecord.TakeInBackupData_CurrentDataFrom_Complete(existingVvDataRecord)

               VvDataRecord existingVvDataRecord = deserialized_vvDataRecord.VvDataRecordFactory();

               deserialized_vvDataRecord.VvDao.SetMe_Record_byRecID_Complete(TheDbConnection, deserialized_vvDataRecord.VirtualRecID, existingVvDataRecord);

               deserialized_vvDataRecord.TakeInBackupData_CurrentDataFrom_Complete(existingVvDataRecord);

#endregion deserialized_vvDataRecord.TakeInBackupData_CurrentDataFrom_Complete(existingVvDataRecord)

#region if(IsDocument): DELREC foreach existingVvDataRecord Transes, Transes2, Transes3 

               // da bi klasicni RWTREC pak ADDREC-ao sve sto najde u XML-u 

               if(existingVvDataRecord.IsDocument)
               {
                  foreach(VvTransRecord trans_rec in ((VvDocumentRecord)existingVvDataRecord).VirtualTranses)
                  {
                     trans_rec.VvDao.DELREC(TheDbConnection, trans_rec, true);
                  }

                  if(existingVvDataRecord.IsPolyDocument)
                  {
                     foreach(VvTransRecord trans_rec in ((VvPolyDocumRecord)existingVvDataRecord).VirtualTranses2)
                     {
                        trans_rec.VvDao.DELREC(TheDbConnection, trans_rec, true);
                     }
                     foreach(VvTransRecord trans_rec in ((VvPolyDocumRecord)existingVvDataRecord).VirtualTranses3)
                     {
                        trans_rec.VvDao.DELREC(TheDbConnection, trans_rec, true);
                     }

                  } // if(existingVvDataRecord.IsPolyDocument) 

               } // if(existingVvDataRecord.IsDocument) 

#endregion if(IsDocument): DELREC foreach existingVvDataRecord Transes, Transes2, Transes3 

#region set SaveTransesWriteMode = ADD foreach deserialized_vvDataRecord.Transes

               if(deserialized_vvDataRecord.IsDocument)
               {
                  // Za XML-ove nastale prije 22.10.2019, koji su imali i SaveTransesWriteMode.DEL u sebi                     
                  // od ovog datuma u SaveSerialized_VvDataRecord_ToXmlFile_AUTOMATICALLY idu samo not DEL                    
                  // ali ovo pomaze SAMO ZA notFaktur recorde. Faktur ide via FakturType koji nema uopce SaveTransesWriteMode 
                  ((VvDocumentRecord)deserialized_vvDataRecord).DiscardPreviouslyDeletedTranses();
                  foreach(VvTransRecord trans_rec in ((VvDocumentRecord)deserialized_vvDataRecord).VirtualTranses)
                  {
                     trans_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;
                  }

                  if(deserialized_vvDataRecord.IsPolyDocument)
                  {
                     ((VvPolyDocumRecord)deserialized_vvDataRecord).DiscardPreviouslyDeletedTranses2();
                     foreach(VvTransRecord trans_rec in ((VvPolyDocumRecord)deserialized_vvDataRecord).VirtualTranses2)
                     {
                        trans_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;
                     }

                     ((VvPolyDocumRecord)deserialized_vvDataRecord).DiscardPreviouslyDeletedTranses3();
                     foreach(VvTransRecord trans_rec in ((VvPolyDocumRecord)deserialized_vvDataRecord).VirtualTranses3)
                     {
                        trans_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;
                     }

                  } // if(deserialized_vvDataRecord.IsPolyDocument) 

               } // if(deserialized_vvDataRecord.IsDocument) 

#endregion set SaveTransesWriteMode = ADD foreach deserialized_vvDataRecord.Transes

               deserialized_vvDataRecord.VvDao.RWTREC(TheDbConnection, deserialized_vvDataRecord); // !!! IsJustDeserializedFromXML ... za VvDaoBase.RwtrecDocument_AddOrRwtOrDelTranses() 

#region Arhiva

               theArhivedVvDataRecord = existingVvDataRecord.CreateArhivedDataRecord(TheDbConnection, "ISPRAVAK");
               theArhivedVvDataRecord.VvDao.ADDREC(TheDbConnection, theArhivedVvDataRecord, true, true, false, false);

#endregion Arhiva

               break;

            case VvSQL.DB_RW_ActionType.DEL: // ####################################################################################################################### 

               deserialized_vvDataRecord.VvDao.DELREC(TheDbConnection, deserialized_vvDataRecord, true);

#region Arhiva

               theArhivedVvDataRecord = deserialized_vvDataRecord.CreateArhivedDataRecord(TheDbConnection, "BRISANJE");            
               theArhivedVvDataRecord.VvDao.ADDREC(TheDbConnection, theArhivedVvDataRecord, true, true, false, false);

#endregion Arhiva

               break;
         }

      } // foreach(ZXC.DBactionForSrvRecID recIDaction in vvDR_RecIDactions)

      if(TheVvUC is FakturDUC) ZXC.RISK_DisableCacheTemporarily = false;

      ZXC.Restore_FromVvXmlDR_InProgress = false;
   }

   private void ShowRecoveryDirectoryFullInfo(DateTime lastBackup_DateAndTIme, string expected_vvXmlDR_recovery_directory)
   {
      string searchPattern1 = VvDataRecord.Auto_vvXmlDR_preffix + "*vv*_"                               + ZXC.PUG_ID                + "_*.xml";

      DirectoryInfo diInfo = new DirectoryInfo(expected_vvXmlDR_recovery_directory); // D:\MyDocuments\Viper.NET\Vektor\VvSerializedDR\vvRecovery 

      FileInfo[] fiArray1 = diInfo.GetFiles(searchPattern1);

      // !!! 'CreationTime' ti se sjebe kod copy paste, pa moras preko DateModified tj 'LastWriteTime' 
      List<string> fileNames = fiArray1.Where(fi => fi./*CreationTime*/LastWriteTime > lastBackup_DateAndTIme).Select(fi => fi.FullName).ToList();

      if(fileNames.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Nema XML datoteka po zadanom kriteriju.");
         return; // der'z notin tu du 
      }
      ZXC.Restore_FromVvXmlDR_InProgress = true;

      List<ZXC.VvUtilDataPackage> fileInfo_UDP_List = new List<ZXC.VvUtilDataPackage>();
      ZXC.VvUtilDataPackage fileInfo_UDP;

      foreach(string fullPathFileName in fileNames)
      {
         string fileName_clean = fullPathFileName.Replace(expected_vvXmlDR_recovery_directory + @"\", "");

         fileInfo_UDP = Set_VvXmlDR_FileInfo_UDP(/*fullPathFileName*/ fileName_clean);

         if(fileInfo_UDP.TheInt.NotZero())
         {
            fileInfo_UDP_List.Add(fileInfo_UDP);
         }
      }

      // ====================================================================================================== 

      var groupsByrecord = fileInfo_UDP_List.GroupBy(qwe => qwe.TheStr3);
      int recIDs_Count;
      List<ZXC.NameAndDecimal_CommonStruct> recordsCountList = new List<ZXC.NameAndDecimal_CommonStruct>();
      foreach(var recordGR in groupsByrecord)
      {
         recIDs_Count = recordGR.Select(udp => udp.TheInt).Distinct().Count();
         recordsCountList.Add(new ZXC.NameAndDecimal_CommonStruct(recordGR.Key, 0M, (uint)recIDs_Count));
      }
      int xmlFilesCount = fileInfo_UDP_List.Count;

      List<string> messages = new List<string>();
      
      foreach(var record in recordsCountList)
      {
         messages.Add(record.TheName + ": " + record.TheUint + " recorda. (distinktivnih RecID-ova)");
      }

      ZXC.aim_emsg_List(string.Format("{0} Xml datoteka u vvRecovery directoryju.", xmlFilesCount), messages);
   }

   #endregion SaveTo & NewFrom VvXmlDR 

   #region VvM2Pay

   internal bool Is_M2P_AuthorizationNeeded(Faktur faktur_rec)
   {
      return ZXC.RRD.Dsc_IsM2PAY && faktur_rec.TtInfo.IsPrihodTT && faktur_rec.IsNacPlacKartica;
   }

   public bool InitHApi(bool forceEvenIfInitialized)
   {
      if(forceEvenIfInitialized == false)
      {
         if(ZXC.M2PAY_API_Initialized)
         {
            return true; // do nothing. M2PAY API is already Initialized 
         }
      }

    //string sharedSecret = "0102030405060708091011121314151617181920212223242526272829303132";
    //string sharedSecret = "1BBFFC6D1CFE1EC980584A61F802D362D7A193CD530E7993DD95F63C433660BC"; // byQ 
      string sharedSecret = ZXC.CURR_prjkt_rec.M2PshaSecDecrypted;
    //string apikey = "This-is-my-api-key-provided-by-Handpoint";
    //string apikey = "BBBXYM8-4A443TX-PTNPERE-DXVRESD"; // byQ
      string apikey = ZXC.CURR_prjkt_rec.M2PapikeyDecrypted;

      if(sharedSecret.IsEmpty() || apikey.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "SharedSecret [{0}] i/ili ApiKey [{1}] su prazni!", sharedSecret, apikey);
          return false;
      }

      try
      {
         TheHapi = HapiFactory.GetAsyncInterface(this, new HandpointCredentials(sharedSecret, apikey));
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg_VvException(ex);
         return false;
      }

      //TheHapi.SetLocale(SupportedLocales.hr_HR);

      ZXC.M2PAY_API_Initialized = true;

      return true;
   }

   internal void M2PAY_DirectConnect(bool forceEvenIfConnected)
   {
      if(forceEvenIfConnected == false)
      {
         if(ZXC.M2PAY_Device_Connected == true)
         {
            return;
         }
      }

    //string m2pSerno = "1240254696";
    //string m2pModel = "PAXA80"    ;
      string m2pSerno = ZXC.CURR_prjkt_rec.M2Pserno;
      string m2pModel = ZXC.CURR_prjkt_rec.M2Pmodel;

      string m2pAddress = m2pSerno + "-" + m2pModel;

      bool initOK = InitHApi(forceEvenIfConnected); // if(ZXC.M2PAY_API_Initialized) return; 

      if(initOK == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Ne mogu Direct Connect jer je InitHApi false!");
         return;
      }

    //Device device       = new Device("VvCloudDevice", m2pAddress, "", ConnectionMethod.CLOUD);
      this.TheM2PayDevice = new Device("VvCloudDevice", m2pAddress, "", ConnectionMethod.CLOUD);

      // new Device("name", "address", "port (optional)", ConnectionMethod);
      // The address always has to be written in UPPER CASE
      // It is the composition of the serial number and model ot the payment terminal.
      // Example for a PAX A920 device: serial_number - model  -> 0821032395-PAXA920

      bool connectResult = TheHapi.Connect(/*device*/this.TheM2PayDevice);
   }

   public void M2PAY_Disconnect()
   {
      TheHapi.Disconnect();

      ZXC.M2PAY_Device_Connected = false;
   }

#if BUMO_PREKO_XMLa

   private void Save_M2Pay_TransactionResult_ToXML(string fileName, TransactionResult result)
   {
      XmlSerializer serializer = new XmlSerializer(typeof(TransactionResult));

      //using(StreamWriter sw = new StreamWriter(fileName))
      //{
      //   resultXmlser.Serialize(sw, result);
      //   sw.Close();
      //}

      //XNode node = JsonConvert.DeserializeXNode(result.ToJSON(), "Root");

      XmlWriterSettings settings = new XmlWriterSettings();
      
      settings.Indent      = true;
      settings.IndentChars = ("   ");
      settings.Encoding    = ZXC.VvUTF8Encoding_noBOM;

      using(XmlWriter writer = XmlWriter.Create(fileName, settings))
      {
         serializer.Serialize(writer, result);
         writer.Close();
      }
   }

   public TransactionResult Load_M2Pay_TransactionResult_FromXML(string fileName)
   {
      bool success = false;

      TransactionResult result       = new TransactionResult();
      XmlSerializer     resultXmlser = new XmlSerializer(typeof(TransactionResult));

      try
      {
         using(StreamReader sr = new StreamReader(fileName))
         {
            result = ((TransactionResult)resultXmlser.Deserialize(sr));
            sr.Close();
            success = true;
         }
      }
      catch(FileNotFoundException)
      {
      }

      return result;
   }

#endif

   #region TransactionResult to & from Xtrano data layer
   private string Set_compressed_JSON_ResultString(TransactionResult result)
   {
      string resultAsJSON                 = result.ToJSON();
      string compressed_JSON_ResultString = VvStringCompressor.CompressString  (resultAsJSON);

      return compressed_JSON_ResultString;
   }

   private TransactionResult Load_M2Pay_TransactionResult_FromJSON(string compressed_JSON_ResultString)
   {
      string decompressedResultString          = VvStringCompressor.DecompressString(compressed_JSON_ResultString);

    //TransactionResult resultDeserializedJSON = null;
      TransactionResult resultDeserializedJSON = new TransactionResult();

      try
      {
         resultDeserializedJSON = JsonConvert.DeserializeObject<TransactionResult>(decompressedResultString);
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Load_M2Pay_TransactionResult_FromJSON PROBLEM:\n\n" + ex.Message);
      }
      //TransactionResult resultDeserializedJSON = new TransactionResult()
      //{
      //   Currency = Currency.EUR,
      //   FinStatus = FinancialStatus.AUTHORISED
      //};

      return resultDeserializedJSON;
   }

   private Xtrano M2P_SetXtranoFrom_TransactionResult(TransactionResult result)
   {
      if(TheVvDataRecord is Faktur == false) return null;

      Faktur faktur_rec = TheVvDataRecord as Faktur;

      string compressed_JSON_ResultString = Set_compressed_JSON_ResultString(result);

      //TransactionResult result3 = Load_M2Pay_TransactionResult_FromJSON(compressed_JSON_ResultString);

      string JSON4xtrano = ZXC.LenLimitedStr(compressed_JSON_ResultString, ZXC.XtranoDao.GetSchemaColumnSize(ZXC.XtoCI.t_opis_128));

      Xtrano resultXtrano_rec = new Xtrano() 
      { 
         T_opis_128 = JSON4xtrano        ,

         T_TT       = Mixer.TT_M2P       ,
       //T_parentID = faktur_rec.RecID   , // NE! Nemas jos faktur_rec.RecID u ovom trenutku 
         T_dokDate  = faktur_rec.DokDate ,
         T_ttNum    = faktur_rec.TtNum   ,
         T_dokNum   = faktur_rec.DokNum  ,
         T_serial   = 1                  ,
         T_moneyA   = faktur_rec.S_ukKCRP,
         T_konto    = ""                 , // fuse 
         T_devName  = faktur_rec.DevName  
      };

      return resultXtrano_rec;
   }

   internal List<Xtrano> M2P_GetTransactionResultListFrom_Xtrano(XSqlConnection conn, /*Faktur faktur_rec*/ uint TtNum)
   {
      //Xtrano xtrano_rec = new Xtrano();
      //// PAZI! ovaj moze vratiti krivoga, ako ih ima vise. Nemas UNIQUE (npr. imamo vise pokusaja+resultova autorizacije)                                                 
      //xtrano_rec.VvDao.SetMe_VvTransRecord_byTtAndTtNum(conn, xtrano_rec, /*faktur_rec.TT*/Mixer.TT_M2P, faktur_rec.TtNum /*, bool isArhiva*/, /*shouldBeSilent*/ false); 

      List<Xtrano> M2Pay_TransactionResult_xtranoList = new List<Xtrano>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      filterMembers.Add(new VvSqlFilterMember(ZXC.XtranoSchemaRows[ZXC.XtoCI.t_tt]      , "elTt"       , Mixer.TT_M2P        , " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.XtranoSchemaRows[ZXC.XtoCI.t_ttNum]   , "elTtNum"    , /*faktur_rec.*/TtNum, " = "));
    //filterMembers.Add(new VvSqlFilterMember(ZXC.XtranoSchemaRows[ZXC.XtoCI.t_parentID], "fakturRecID", faktur_rec.RecID    , " = ")); 

      VvDaoBase.LoadGenericVvDataRecordList<Xtrano>(conn, M2Pay_TransactionResult_xtranoList, filterMembers, "recID DESC");

      return M2Pay_TransactionResult_xtranoList;
   }

   internal TransactionResult M2P_GetLast_TransactionResultFrom_Xtrano(XSqlConnection conn, /*Faktur faktur_rec*/ uint TtNum)
   {
      List<Xtrano> M2Pay_TransactionResult_xtranoList = M2P_GetTransactionResultListFrom_Xtrano(conn, /*faktur_rec.*/TtNum);

      Xtrano xtrano_rec = M2Pay_TransactionResult_xtranoList.FirstOrDefault();

      if(xtrano_rec == null) return null; // nema M2Pay_TransactionResut a 

      TransactionResult result = Load_M2Pay_TransactionResult_FromJSON(xtrano_rec.T_opis_128);

      return result;
   }

   #endregion TransactionResult to & from Xtrano data layer

   #region Event Handlers

   public void DeviceDiscoveryFinished_ORIG(List<Device> devices)
   {
      foreach(Device device in devices)
      {
         if(device.Name != null)
         {
            //if(device.Name.Equals("CloudDevice"))
            if(device.Name.Equals("1240254696"))

            {
               this.TheM2PayDevice = device;
               TheHapi.Connect(this.TheM2PayDevice);
            }
         }
      }
   }

   public void DeviceDiscoveryFinished(List<Device> devices)
   {
      var msgList = new List<string>();

      foreach(Device device in devices)
      {
         if(device.Name != null)
         {
            msgList.Add(String.Format("ID: [{0}]      Address: [{1}]      Name: [{2}]", device.Id, device.Address, device.Name));
         }
      }
      ZXC.aim_emsg_List(string.Format("Discovered {0} M2PAY devices.", msgList.Count), msgList);
   }

   public TransactionResult GetTransactionStatus(String transactionReference)
   {
      TransactionResult result = TheHapi.GetTransactionStatus(transactionReference);
      return result;
   }

   public void ConnectionStatusChanged(ConnectionStatus status, Device device)
   {
      string theMessage = ("*** ConnectionStatus *** " + status);

      Console.WriteLine(theMessage);

    //VvHamper.Set_ControlText_ThreadSafe((TheVvUC as FakturExtDUC).tbx_fiskPrgBr, theMessage);

      Color theColor = Color.White;
      switch(status)
      {
         case ConnectionStatus.Disconnected: theColor = Color.Red   ; ZXC.M2PAY_Device_Connected = false; break;
         case ConnectionStatus.Initializing: theColor = Color.Orange;                                     break;
         case ConnectionStatus.Connecting  : theColor = Color.Yellow;                                     break;
         case ConnectionStatus.Connected   : theColor = Color.Green ; ZXC.M2PAY_Device_Connected = true ; break;
      }

      VvHamper.Set_ControlBackColor_ThreadSafe((TheVvUC as FakturExtDUC).m2PayConectedLabel, theColor);

   }

   public void CurrentTransactionStatus(StatusInfo info, Device device)
   {
    //string theMessage = ("*** CurrentTransactionStatus *** " + info.Status.ToString());
      string theMessage = ("Status: " + info.Status.ToString());

      Console.WriteLine(theMessage);

    //VvHamper.Set_ControlText_ThreadSafe(M2P_Statusdlg.tbx_m2PayStatus, theMessage);
      VvHamper.Set_ControlText_ThreadSafe(M2P_Statusdlg.lbl_m2PayStatus, theMessage);
      //this.TStripStatusLabel.Text = theMessage;
   }

   public void EndOfTransaction(TransactionResult result, Device device)
   {
      ZXC.M2PAY_AuthorizationStatus = result.FinStatus;

      //string theMessage = ("*** EndOfTransaction *** " + result.ToJSON());
      string theMessage = ("Gotovo: " + result.FinStatus + " / " + result.StatusMessage);
      
      Console.WriteLine(theMessage);
      
    //VvHamper.Set_ControlText_ThreadSafe(M2P_Statusdlg.tbx_m2PayStatus, theMessage);
      VvHamper.Set_ControlText_ThreadSafe(M2P_Statusdlg.lbl_m2PayStatus, theMessage);

      System.Threading.Thread.Sleep(/*2000*/ZXC.RRD.Dsc_M2P_TimeOutSeconds * 1000); 

      // 20.09.2024: preselio dole, nakon Xtrano ADDREC-a 
    //CloseForm_ThreadSafe(M2P_Statusdlg);

      #region Save Transaction Result

      Xtrano m2pResultXtrano_rec = M2P_SetXtranoFrom_TransactionResult(result); // Tu ide .ToJason() 

      if(m2pResultXtrano_rec != null)
      {
         bool OK = ZXC.XtranoDao.ADDREC(TheDbConnection, m2pResultXtrano_rec, false, false, false, false);
      }

      #endregion Save Transaction Result

      FakturExtDUC theDUC = (TheVvUC as FakturExtDUC);

      theDUC.M2P_TransactionResult    = result;
      theDUC.M2P_Xtrano_Current_TtNum = m2pResultXtrano_rec.T_ttNum;

      CloseForm_ThreadSafe(M2P_Statusdlg); // ovo izazove nastavak izvođenja linija koda nakon M2P_Statusdlg.ShowDialog() u Save-u      
                                           // pa dok je bilo prije ').M2P_TransactionResult = result;' neki put se punjenje vrijednosti 
                                           // M2P_TransactionResult-a događalo prekasno, nakon što si ga u Save'u isao konzumirati      

      theDUC.Fld_M2PayStatus = Get_M2P_TransactionStatusMessage(result);
   }

   public void HardwareStatusChanged(HardwareStatus status, ConnectionMethod hardware)
   {
      //Ignore
   }

   public void SignatureRequired(SignatureRequest request, Device device)
   {
      //Ignore
   }


   #endregion Event Handlers

   #region SubModulActions

   // M2P 1 
   private void RISK_M2PAY_DirectConnect/*_ORIG*/(object sender, EventArgs e)
   {
      if(ZXC.RRD.Dsc_IsM2PAY == false) return;  

      M2PAY_DirectConnect(true);
   }

   private void RISK_M2PAY_DiscoverDevices_REFUND(object sender, EventArgs e)
   {
      if(ZXC.RRD.Dsc_IsM2PAY == false) return;

      TransactionResult transactionResult = (TheVvUC as FakturExtDUC).M2P_TransactionResult;

      if(transactionResult == null)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu isčitati M2P TransactionResult");
         return;
      }

      if(InitHApi(false) == false) return;

      if(ZXC.TryBeginM2PayTransaction(GetM2PayTransactionOwnerText()) == false) return;

      try
      {
         M2P_Statusdlg = new VvM2PayStatusDlg();
         M2P_Statusdlg.lbl_m2PayStatus.Text = "Start POVRATA plaćanja ovog računa...";

         ZXC.M2PAY_AuthorizationStatus = com.handpoint.api.FinancialStatus.UNDEFINED;


       //int moneyAsInteger = (TheVvDataRecord as Faktur).M2P_moneyAsInteger;

         OperationStartResult result = TheHapi.Refund      (transactionResult.TotalAmount, transactionResult.Currency, transactionResult.TransactionId); // Voila! ####################################################################################################### 
       //OperationStartResult result = TheHapi.Refund      (transactionResult.TotalAmount, transactionResult.Currency                                 ); // Voila! ####################################################################################################### 
       //OperationStartResult result = TheHapi.SaleReversal(transactionResult.TotalAmount, transactionResult.Currency, transactionResult.TransactionId); // Voila! ####################################################################################################### 

         if(result.OperationStarted == true) { M2P_Statusdlg.ShowDialog();                                                                                             }
         else                                { ZXC.aim_emsg(MessageBoxIcon.Error, "Start autorizacije nije uspio. Provjerite terminal te pokušajte ponovno."); return; }

      }
      finally
      {
         ZXC.EndM2PayTransaction();
      }

   }

   // M2P 2 
   private void RISK_M2PAY_DiscoverDevices/*_ORIG*/(object sender, EventArgs e)
   {
      if(ZXC.RRD.Dsc_IsM2PAY == false) return;

      InitHApi(false);

      //TheHapi.Disconnect();

      TheHapi.SearchDevices(ConnectionMethod.CLOUD);
   }

   private VvM2PayStatusDlg M2P_Statusdlg;
   private void RISK_M2PAY_DiscoverDevices_NEW(object sender, EventArgs e)
   {
      if(ZXC.RRD.Dsc_IsM2PAY == false) return;

      M2P_Statusdlg = new VvM2PayStatusDlg();
         
      if(M2P_Statusdlg.ShowDialog() != DialogResult.OK)
      {
         M2P_Statusdlg.Dispose();
         return;
      }

      M2P_Statusdlg.Dispose();
   }

   // M2P 3 
   private void RISK_M2PAY_Authorize(object sender, EventArgs e)
   {
      if(ZXC.RRD.Dsc_IsM2PAY == false) return;

      InitHApi(false); // ne brini, nece se nepotrebno ponavljati jer checkirash ZXCZXC.M2PAY_API_Initialized 

      bool m2pAuthorizationNeeded = false;

      Faktur m2p_faktur_rec = null;

      if(TheVvDataRecord is Faktur)
      {
         m2p_faktur_rec = TheVvDataRecord as Faktur;

         m2pAuthorizationNeeded = Is_M2P_AuthorizationNeeded(m2p_faktur_rec);
      }

      if(!m2pAuthorizationNeeded) return;

      if(ZXC.M2PAY_API_Initialized == false) return;

      if(ZXC.TryBeginM2PayTransaction(GetM2PayTransactionOwnerText()) == false) return;

      try
      {

      //if(ZXC.M2PAY_Device_Connected == false) { ZXC.aim_emsg(MessageBoxIcon.Stop, "M2PAY uređaj nije spojen!"); return ; }

         M2P_Statusdlg = new VvM2PayStatusDlg();
         M2P_Statusdlg.lbl_m2PayStatus.Text = "Start autorizacije plaćanja ovog računa...";

         ZXC.M2PAY_AuthorizationStatus = com.handpoint.api.FinancialStatus.UNDEFINED;

       //int moneyAsInteger =       m2p_faktur_rec.M2P_moneyAsInteger;
         int moneyAsInteger = (int)(m2p_faktur_rec.S_ukKCRP * 100);

         OperationStartResult result = TheHapi.Sale(new BigInteger(moneyAsInteger /*1000*/ /*37.84*/ ), Currency.EUR); // Voila! ####################################################################################################### 

         // Let´s start our first transaction for 10 euros                
         // The amount should always be in the minor unit of the currency 

         if(result.OperationStarted == true) { M2P_Statusdlg.ShowDialog(); }
         else                                { ZXC.aim_emsg(MessageBoxIcon.Error, "Start autorizacije nije uspio. Provjerite terminal te pokušajte ponovno."); return ; }
      }
      finally
      {
         ZXC.EndM2PayTransaction();
      }
   }

   private void Swap_EnableDisable_M2PAY(object sender, EventArgs e)
   {
      ZXC.RRD.Dsc_IsM2PAY = !ZXC.RRD.Dsc_IsM2PAY;

      ZXC.aim_emsg(MessageBoxIcon.Information, "M2PAY iz now " + ZXC.RRD.Dsc_IsM2PAY.ToString());
   }

   /*private*/
   public void RISK_M2P_ShowResults(object sender, EventArgs e)
   {
      Faktur faktur_rec = (Faktur)(TheVvDataRecord);
      FakturDUC theDUC  = TheVvDocumentRecordUC as FakturDUC;

      List<Xtrano> M2Pay_TransactionResult_xtranoList = M2P_GetTransactionResultListFrom_Xtrano(TheDbConnection, faktur_rec.TtNum);

      List<TransactionResult> transactionResultList = new List<TransactionResult>();

      if(M2Pay_TransactionResult_xtranoList.NotEmpty())
      {
         M2Pay_TransactionResult_xtranoList.ForEach(xto => transactionResultList.Add(Load_M2Pay_TransactionResult_FromJSON(xto.T_opis_128)));
      }

      List<string> theList = new List<string>();

      transactionResultList.ForEach(result => theList.Add(String.Format("Status: {0}  {1}  {2}  {3}  {4}  {5} {6}",
                                                         result.EftTimestamp.ToShortDateString(),
                                                       //result.EftTimestamp.ToLongTimeString(),
                                                         result.EftTimestamp.ToLocalTime().ToLongTimeString(),
                                                         GetHR_M2P_FinStatus(result.FinStatus/*.ToString()*/),
                                                         GetHR_M2P_Type     (result.Type     /*.ToString()*/),
                                                         result.AuthorisationCode,
                                                         result.CardSchemeName,
                                                         result.MaskedCardNumber
                                                         )));

      ZXC.aim_emsg_List("M2PAY Transaction Results", theList);
   }

   public void RISK_M2P_UNDO_Authorize(object sender, EventArgs e)
   {
      DialogResult dlgResult = MessageBox.Show("Da li zaista želite STORNIRATI ovu kartičnu naplatu te izvršiti POVRAT na karticu?!",
         "Potvrdite POVRAT sredstava ovog kartičnog plaćanja?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(dlgResult != DialogResult.Yes) return;

      if(ZXC.M2PAY_Device_Connected == false) { ZXC.aim_emsg(MessageBoxIcon.Stop, "M2PAY uređaj nije spojen!"); return; }

      if(ZXC.M2PAY_API_Initialized == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "M2PAY API nije inicijaliziran!");
         return;
      }

      Faktur m2p_faktur_rec = TheVvDataRecord as Faktur;
      OperationStartResult result;

      TransactionResult transactionResult = M2P_GetLast_TransactionResultFrom_Xtrano(TheDbConnection, /*ZXC.FakturRec*/m2p_faktur_rec.TtNum);

      if(transactionResult == null)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu isčitati M2P TransactionResult");
         return;
      }

      if(ZXC.TryBeginM2PayTransaction(GetM2PayTransactionOwnerText()) == false) return;

      try
      {
         ZXC.M2PAY_AuthorizationStatus = com.handpoint.api.FinancialStatus.UNDEFINED;
      
         M2P_Statusdlg = new VvM2PayStatusDlg();
         M2P_Statusdlg.lbl_m2PayStatus.Text = "Start POVRATA sredstava na karticu po plaćanja ovog računa...";

         bool isSaleReversal = m2p_faktur_rec.DokDate.Date == DateTime.Today.Date;
         bool isRefund       = !isSaleReversal;

         if(isRefund) result = TheHapi.Refund      (transactionResult.TotalAmount, transactionResult.Currency, transactionResult.TransactionId); // Voila! ####################################################################################################### 
         else         result = TheHapi.SaleReversal(transactionResult.TotalAmount, transactionResult.Currency, transactionResult.TransactionId); // Voila! ####################################################################################################### 

         if(result.OperationStarted == true) { M2P_Statusdlg.ShowDialog();                                                                            }
         else                                { ZXC.aim_emsg(MessageBoxIcon.Error, "Start autorizacije nije uspio. Pokušajte ponovno."); return; }

         if(ZXC.M2PAY_AuthorizationStatus != FinancialStatus.AUTHORISED)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "AUTORIZACIJA NIJE USPJELA!."); 
         
            return;
         }

      }
      finally
      {
         ZXC.EndM2PayTransaction();
      }

   }

   private string GetM2PayTransactionOwnerText()
   {
      string tabTitle = TheVvTabPage != null ? TheVvTabPage.Title : string.Empty;

      return tabTitle.IsEmpty() ? "aktivnom prozoru" : string.Format("prozoru '{0}'", tabTitle);
   }

   #endregion SubModulActions

   #region Utils

   public string GetHR_M2P_FinStatus(/*string*/ FinancialStatus m2p_finStatus)   
   {
    //switch(m2p_finStatus)  //UNDEFINED AUTHORISED DECLINED PROCESSED FAILED CANCELLED PARTIAL_APPROVAL
    //{
    //   case "AUTHORISED": return "ODOBRENO"  ;
    //   case "CANCELLED" : return "OTKAZANO"  ;
    //   case "DECLINED"  : return "ODBIJENO"  ;
    //   case "FAILED"    : return "NEUSPJEŠNO";
    //   default          : return m2p_finStatus;
    //}
      switch(m2p_finStatus)  //UNDEFINED AUTHORISED DECLINED PROCESSED FAILED CANCELLED PARTIAL_APPROVAL
      {
         case FinancialStatus.AUTHORISED: return "ODOBRENO"  ;
         case FinancialStatus.CANCELLED : return "OTKAZANO"  ;
         case FinancialStatus.DECLINED  : return "ODBIJENO"  ;
         case FinancialStatus.FAILED    : return "NEUSPJEŠNO";

         default                        : return m2p_finStatus.ToString();
      }
   } 

   public string GetHR_M2P_Type(/*string*/TransactionType m2p_type)   
   {
    //switch(m2p_type)  //UNDEFINED AUTHORISED DECLINED PROCESSED FAILED CANCELLED PARTIAL_APPROVAL
    //{
    //   case "SALE"     : return "PRODAJA"       ;
    //   case "REFUND"   : return "POVRAT"        ;
    //   case "VOID_SALE": return "STORNO PRODAJE";
    //   default         : return m2p_type        ;
    //}
      switch(m2p_type)  //UNDEFINED AUTHORISED DECLINED PROCESSED FAILED CANCELLED PARTIAL_APPROVAL
      {
         case TransactionType.SALE      : return "PRODAJA"       ;
         case TransactionType.REFUND    : return "POVRAT"        ;
         case TransactionType.VOID_SALE : return "STORNO PRODAJE";

         default                        : return m2p_type.ToString();
      }
   }

   private void ADD_NEW_NacPlac_FromM2PAY(string vvNacPlac_from_M2PnacPlac)
   {
      VvLookUpItem newLui = new VvLookUpItem()
      { 
         Cd    = vvNacPlac_from_M2PnacPlac,
         DateT = DateTime.Now.Date        ,
      };

      ZXC.luiListaRiskVrstaPl.Add(newLui);

      VvDaoBase.SaveLookUpListToSqlTable(ZXC.luiListaRiskVrstaPl);
   }

   private string Get_vvNacPlac_from_M2PnacPlac(string M2P_cardSchemeName)
   {
      //if(lukap liksta ne sadrzi M2P_cardSchemeName, dodaj u luk ap) TODO 

      // Dakle, ovdje eventualno modificiramo M2Pay string sa vaktorovim NacPlac stringom 
      // SAMO ako je TEXTHO, ostalima ostavljamo kak je m2p vratio                        
      if(ZXC.IsTEXTHOshop == false) return M2P_cardSchemeName;

      string M2P_cardSchemeName_ToUpper = M2P_cardSchemeName.ToUpper();

           if(M2P_cardSchemeName_ToUpper.Contains("VISA"  )) return "VISA"       ;
      else if(M2P_cardSchemeName_ToUpper.Contains("MASTER")) return "MASTER CARD";
      else return M2P_cardSchemeName;
   }

   internal string Get_M2P_TransactionStatusMessage(TransactionResult result)
   {
      return GetHR_M2P_Type(result.Type) + ": " + GetHR_M2P_FinStatus(result.FinStatus);
   }

   #endregion Utils

   #endregion VvM2Pay

}
