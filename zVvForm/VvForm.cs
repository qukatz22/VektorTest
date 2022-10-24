using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using Crownwood.DotNetMagic.Controls;
using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Forms;
using System.Deployment.Application;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

public /*sealed*/ partial class VvForm : DotNetMagicForm
{

   #region Main

   //static System.Threading.Mutex singleton = new System.Threading.Mutex(true, "My App Name");

   [STAThread]
   public static void Main(string[] args)
   {
      ZXC.MainArgs = args;
      Application.EnableVisualStyles();
      //Application.DoEvents();


      Application.SetCompatibleTextRenderingDefault(false);

      // 18.10.2021: pali / gasi if !DEBUG ako oces da ti Exception stane u kodu 
      // !!! ALI POSLIJE VRATI 
#if !DEBUG
      Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(VvApplication_ThreadException);
#endif
      Application.Run(new VvForm());
   }

   private static int GetThisApplicationRunningInstancesCount()
   {
      Process process = Process.GetCurrentProcess();

      string procName1 = Process.GetCurrentProcess().ProcessName;
    //string procName2 = Process.GetCurrentProcess().MainModule.ModuleName;

      Process[] processes1 = Process.GetProcessesByName(procName1);
    //Process[] processes2 = Process.GetProcessesByName(procName2);

      return processes1.Length;
   }

   public static void VvApplication_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
   {
      //ZXC.aim_emsg(MessageBoxIcon.Exclamation, e.Exception.Message);

      //Clipboard.SetText(e.Exception.StackTrace);

      ZXC.aim_log(e.Exception.Message   );
      ZXC.aim_log(e.Exception.StackTrace);

      #region EXCEPT this Exception!!!

      // FUSE ako zelis suspregnuti javljanje nekog konkretnog Exceptiona 
      if(false/*e.Exception.Message.ToLower().Contains("has been thrown by the target of an invocation")*/) // "Exception has been thrown by the target of an invocation" 
      {
         return; // !!! 
      }

      #endregion EXCEPT this Exception!!!

      #region ExceptionIntroForm

      if(Environment.MachineName == "RIPLEY7"   ||
         Environment.MachineName == "RIPLEY22"  ||
         Environment.MachineName == "VVKRISTAL" ||
         Environment.MachineName == "VVKRISTAL-NEW" ||
         Environment.MachineName == "QWHICHKEY"  ) 
      {
         Timer timer = new Timer();
         timer.Interval = 1000;
         timer.Tick += new EventHandler(ExceptionIntro_timer_Tick);
         timer.Start();
      }

      #endregion ExceptionIntroForm

      VvExceptionDlg exDlg = new VvExceptionDlg(e.Exception.Message, e.Exception.StackTrace,
         e.Exception.InnerException != null ? e.Exception.InnerException.Message : "no inner ex", e.Exception.InnerException != null ? e.Exception.InnerException.StackTrace : "");

      DialogResult dlgResult = exDlg.ShowDialog();

      if(dlgResult == DialogResult.Abort)
      {
         System.Environment.Exit(0);
      }

      exDlg.Dispose();

      return;
   }

   static void ExceptionIntro_timer_Tick(object sender, EventArgs e)
   {
      Timer timer = sender as Timer;
      
      timer.Stop();

      #region ExceptionIntroForm

      Form exceptionIntroForm = new Form();
      exceptionIntroForm.StartPosition = FormStartPosition.CenterScreen;
      exceptionIntroForm.Size = new System.Drawing.Size(300, 300);
      exceptionIntroForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      exceptionIntroForm.ControlBox = false;
      exceptionIntroForm.Text = "OOPS!!!";
      exceptionIntroForm.BackgroundImage = new Bitmap(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.oops.png"));
      exceptionIntroForm.BackColor = Color.White;
      exceptionIntroForm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      exceptionIntroForm.Opacity = /*0.85*/1.00;
      //exceptionIntroForm.Icon = this.Icon;


      #endregion ExceptionIntroForm

      exceptionIntroForm.Show();
      System.Threading.Thread.Sleep(3000);
      exceptionIntroForm.Close();
   }

   #endregion Main

   public VvForm()
   {
      ZXC.InitializeApplication_InProgress = true;

      if(InitializeApplication(/*false,*/ false) == false)
      {
         while(InitializeApplication(/*true,*/ true) != true);
      } 

      ZXC.InitializeApplication_InProgress = false;

   }

   protected static bool loginExceptionRised = false;

   public virtual bool Initialize_Hektor_Application(bool isRecconection)
   {
      throw new Exception("Shouldn't hektorize.");
   }

   /*private*/public bool InitializeApplication(/*bool forceLoginForm,*/ bool isRecconection)
   {
      // !!! 
      if(ZXC.ThisIsHektorProject) return Initialize_Hektor_Application(isRecconection);
      
      SuspendLayout();

      // ________ here you can TestSomething(); ________ 

      // 03.05.2016: Probati cu ugasiti ovaj try-catch-finally blok
      // jer je dodan ThreadExceptionEventHandler(VvApplication_ThreadException) u Main()

      // 03.05.2016: komentirano: try
      // 03.05.2016: komentirano: {

         // 04.12.2013: MAJOR FACKUP?!?! na Ripley7 mi u debug-u neda uci u program nakon dodavanja 2 nova PlacaDUC-a 
       //this.Icon = new Icon(GetManifestResourceStream("Vektor.Icons.Vektor.ico"));
         this.Icon = CreateIconSafely(GetManifestResourceStream("Vektor.Icons.Vektor.ico"));

         progressForm = CreateWaitingForConnectionForm(/*this.Icon*/);

         if(isRecconection == false || loginExceptionRised == true)
         {
            ZXC.SetStatusText("InitializeVvColorsAndStyles"); InitializeVvColorsAndStyles();
            ZXC.SetStatusText("InitializeVvForm"); InitializeVvForm();
            ZXC.SetStatusText("InitializeVvPreferences"); InitializeVvPreferences();
            ZXC.SetStatusText("InitializeCultureAndNumberFormatInfo"); InitializeCultureAndNumberFormatInfo();

            ZXC.SetStatusText("InitializeVvLuiLists"); InitializeVvLuiLists();
            ZXC.SetStatusText("Initialize_kamtbl_rec"); Initialize_kamtbl_rec();

            ZXC.ClearStatusText();
         }

         GetLoginData(/*forceLoginForm*/);

         // 20.06.2022: 
         if(ZXC.IsSvDUHdomena == true)
         {
            // ponovo, da kad je nakon GetLoginData() ZXC.IsSvDUHdomena postala 'true' 
            // obavi ZXC.luiListaSkladista sa IsYearDependent = true                   
            InitializeVvLuiLists_ViperProjectDependent();
         }

         if(ZXC.ThisIsHektorProject == false)
         {
            CheckPrjktsDatabasesExists();
         }

         if(isRecconection == false || loginExceptionRised == true)
         {
/* ZXC.SetStatusText("InitializeVvUserControlList");                */ if(!ZXC.ThisIsHektorProject) InitializeVvUserControlList();
/* ZXC.SetStatusText("InitializeVvModul");                          */ if(!ZXC.ThisIsHektorProject) InitializeVvModul();
/* ZXC.SetStatusText("InitializeVvMenu");                           */ if(!ZXC.ThisIsHektorProject) InitializeVvMenu();
/* ZXC.SetStatusText("InitializeWorkTabControl");                   */ if(!ZXC.ThisIsHektorProject) InitializeWorkTabControl();
/* ZXC.SetStatusText("InitializeSplitterAndModulPanel");            */ if(!ZXC.ThisIsHektorProject) InitializeSplitterAndModulPanel();
/* ZXC.SetStatusText("InitializeModulButtonAndModulMenuItem");      */ if(!ZXC.ThisIsHektorProject) InitializeModulButtonAndModulMenuItem();
/* ZXC.SetStatusText("InitializeSubModulBtnAndSubModulMenuItem");   */ if(!ZXC.ThisIsHektorProject) InitializeSubModulBtnAndSubModulMenuItem();
/* ZXC.SetStatusText("InitializeSubModulSet_TOOLSTRIP_MenuItem");   */ if(!ZXC.ThisIsHektorProject) InitializeSubModulSet_TOOLSTRIP_MenuItem();
/* ZXC.SetStatusText("InitializeToolStrip_Report");                 */ if(!ZXC.ThisIsHektorProject) InitializeToolStrip_Report();
/* ZXC.SetStatusText("InitializeReportModulBtnAndReportlMenuItem"); */ if(!ZXC.ThisIsHektorProject) InitializeReportModulBtnAndReportlMenuItem();
/* ZXC.SetStatusText("InitializeToolStripPanels");                  */ if(!ZXC.ThisIsHektorProject) InitializeToolStripPanels();
/* ZXC.SetStatusText("InitalizeToolStrip_Modul");                   */ if(!ZXC.ThisIsHektorProject) InitalizeToolStrip_Modul();
/* ZXC.SetStatusText("InitalizeToolStrip_Record");                  */ if(!ZXC.ThisIsHektorProject) InitalizeToolStrip_Record();
/* ZXC.SetStatusText("InitializeMainMenu");                         */ if(!ZXC.ThisIsHektorProject) InitializeMainMenu();
/* ZXC.SetStatusText("InitializeStatusStrip");                      */ if(!ZXC.ThisIsHektorProject) InitializeStatusStrip();
/* ZXC.SetStatusText("InitializeTimer");                            */ if(!ZXC.ThisIsHektorProject) InitializeTimer();

//ZXC.ClearStatusText();
         }

         if(ZXC.ThisIsHektorProject) goto thisIsHektorProject_label;

         if(InitializeWorkingProject() == false) return false;

         if(Getvv_PRODUCT_name() == ZXC.vv_PRODUCT_Name)
         {
            /*ZXC.SetStatusText("InitializeWorkingDaysList_ForYear");*/ InitializeWorkingDaysList_ForYear();
         }

         // 17.11.2015: 
         if(isRecconection) ZXC.root_prjkt_rec_loaded = false;

         ZXC.SetStatusText("InitializeVvDao");

         // 12.10.2017: 
         InitializeRiskTtInfo(); // po novome, ide prije 'InitializeVvDao()'

         if(ZXC.ThisIsHektorProject == false)
         {
            InitializeVvDao();
         }
         ZXC.ClearStatusText();

         // 12.10.2017: jebisimaterkurvoglupa 
         // 12.10.2017: preselili 'InitializeRiskTtInfo()' prije 'InitializeVvDao()' !!! 
       //InitializeRiskTtInfo();

         if(Getvv_PRODUCT_name() == ZXC.vv_PRODUCT_Name && ZXC.ThisIsHektorProject == false)
         {
            //13.02.2018: idemo LoadDevTec via async
            ZXC.SetStatusText("LoadDevTec_Async");
            LoadDevTec();
            ZXC.SetStatusText("InitializeCrystalReports_LoadDLL_ForDummyReport_InTheBackGround");
            InitializeCrystalReports_LoadDLL_ForDummyReport_InTheBackGround();

         }


      // 32.01.2017: prestao checkirati XP verziju 
      //if(!ZXC.IsTEXTHOany && !ZXC.IsRipley7 && ZXC.ThisIsHektorProject == false)
      //{
      //   int major, minor;
      //
      //   string winVersion = VvAboutBox.GetWindowsVersion(out major, out minor);
      //
      //   bool isTooOldWindows = major < 6;
      //
      //   if(isTooOldWindows)
      //   {
      //      VvMailClient mailClient = new VvMailClient();
      //
      //      mailClient.SendMail_SUPPORT(false, "WinVer " + winVersion, "Windows version is too old?\n\n" + winVersion, " XP?");
      //   }
      //}

      // 07.05.2019: ___start___                                                                                       

      if(ZXC.ThisIsVektorProject && Getvv_PRODUCT_name() != ZXC.vv_SKYLAB_PRODUCT_Name)
      { 
         CHECK_is_VvXmlDR_LastDocumentMissing_ShitHappendDetection()   ; // !!!                                           

         ZXC.Delete_OldNamingConvention_AutoCreated_VvXmlDR_Directory(); // pobrisi ovo jednog dana, recimo na jesen 2019 

         Cleanup_VvXmlDR_ToTarGZ()                                     ; // !!!                                           

         Cleanup_VektorLog_ToTarGZ()                                   ; // !!!                                           

         //Cleanup_Fiskal_RnOxml_RnZxml_ToTarGZ("RnZ_", VvForm.GetLocalDirectoryForVvFile(@"FiskXML Zahtjev")); // !!!    

         //Cleanup_Fiskal_RnOxml_RnZxml_ToTarGZ("RnO_", VvForm.GetLocalDirectoryForVvFile(@"FiskXML Odgovor")); // !!!    
      }

      // 07.05.2019: ___end___                                                                                         

      // 09.07.2019: ___start___   Provjera Printa IRA / IFA (Tembo)                                                   

      if(ZXC.ThisIsVektorProject && Getvv_PRODUCT_name() != ZXC.vv_SKYLAB_PRODUCT_Name && ZXC.RRD.Dsc_IsPamtiPrintDate)
      {
         // SELECT * FROM faktur f      
         // LEFT JOIN     faktEx x      
         // ON f.RecID = x.FakturRecID  
         //                             
         // WHERE TT IN('IFA', 'IRA')   
         //                             
         // AND DokDate >= '2018-07-10' 
         // AND DateX   <  '1968-01-12' 

         List<Faktur> IFA_IRA_NotPrinted_FakturList = new List<Faktur>();

         string[] array_IFA_IRA_TT = new string[] {
            Faktur.TT_IFA,
            Faktur.TT_IRA,
         };

         List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

         filterMembers.Add(new VvSqlFilterMember("tt", TtInfo.GetSql_IN_Clause(array_IFA_IRA_TT)   ,                            " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!
         filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate], "dokDate", ZXC.IFA_IRA_PrintDate_ERA, " >= "));
         filterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.dateX  ], "dateX"  , ZXC.MySQL_MIN_timpestamp , " <= "));

         VvDaoBase.LoadGenericVvDataRecordList<Faktur>(TheDbConnection, IFA_IRA_NotPrinted_FakturList, filterMembers, "", "dokDate, ttSort, ttNum", true);

         if(IFA_IRA_NotPrinted_FakturList.NotEmpty())
         {
            ZXC.aim_emsg_List("NE odprintane IFA-e / IRA-e", IFA_IRA_NotPrinted_FakturList.Select(fak => fak.TT_And_TtNum).ToList());

            ZXC.CurrentForm = this; // bez ovoga ide u exception kod ucitaj izvod 
         }

      } // if(ZXC.ThisIsVektorProject && Getvv_PRODUCT_name() != ZXC.vv_SKYLAB_PRODUCT_Name && ZXC.RRD.Dsc_IsPamtiPrintDate) 

      // 09.07.2019: ___end___                                                                                         

      thisIsHektorProject_label:

      Cursor.Current = Cursors.Default;

      // 03.05.2016: komentirano: }
      // 03.05.2016: komentirano: catch(Exception e)
      // 03.05.2016: komentirano: {
      // 03.05.2016: komentirano:    VvSQL.ReportGeneric_DB_Error("VvForm Constructor", "GRESKA: " + e.ToString(), System.Windows.Forms.MessageBoxButtons.OK);
      // 03.05.2016: komentirano:    
      // 03.05.2016: komentirano:    loginExceptionRised = true;
      // 03.05.2016: komentirano: 
      // 03.05.2016: komentirano:    return false;
      // 03.05.2016: komentirano: }
      // 03.05.2016: komentirano: finally
      // 03.05.2016: komentirano: {

        CloseForm_ThreadSafe(progressForm);

      // 03.05.2016: komentirano: }

      // 16.11.2015: 
      // 15.08.2016: 
      // 07.09.2016: 
      //if(ZXC.ThisIsVektorProject                                                                                  ) VvDaoBase.Send_MBF_Data_ToSKY(); // uncomment this 
      //if(ZXC.ThisIsVektorProject && Getvv_PRODUCT_name() != ZXC.vv_SKYLAB_PRODUCT_Name                            ) VvDaoBase.Send_MBF_Data_ToSKY(); // uncomment this 
      // 13.11.2016: privremeno suspendirao dokle ne zavrsimo migraciju na novi SkyLab 79.143. ... 
      //if(ZXC.ThisIsVektorProject && Getvv_PRODUCT_name() != ZXC.vv_SKYLAB_PRODUCT_Name && ZXC.IsTEXTHOany == false && ZXC.ThisIsHektorProject == false                                    ) VvDaoBase.Send_MBF_Data_ToSKY(); // uncomment this 
      if(ZXC.ThisIsVektorProject && Getvv_PRODUCT_name() != ZXC.vv_SKYLAB_PRODUCT_Name && ZXC.IsTEXTHOany == false && ZXC.ThisIsHektorProject == false && ZXC.ThisIsSurgerProject == false) VvDaoBase.Process_MBF_Data(); // uncomment this 

      ResumeLayout();

      return true;
   }



   #region Fieldz

   private LoginForm loginForm;
   private Crownwood.DotNetMagic.Controls.TabControl workTabControl;

   private Splitter spliterModulPanelTabControl;

   #region ModulPanel

   public  Panel                 modulPanel;
   private Panel[]               aModulPanel;
   private TitleBar[]            aModulButton;
   public  ButtonWithStyle[][]   aSubModulButton;
   private ButtonWithStyle[][][] aReportModulButton;

   private int modulButtonsHeight;// = 22;
   private int activeModulButtonIndex, actxI, actxJ;
   private int margin4modul;

   #endregion ModulPanel

   #region Menus_ToolStrips

   public MenuStrip                menuStrip;
   private ToolStripMenuItem       mi_ViewModulPanelRight, mi_ViewModulPanelLeft, mi_YesNoViewModulPanel,
                                   mi_ModulPanelRightOnContexMenu, mi_ModulPanelLeftOnContexMenu,
                                   mi_UpDownTabPage,
                                   miSub_RecordToolStripVisible, miSub_ModulToolStripVisible,
                                   mi_IconeSize, mi_DBkonekcija, mi_ScalingFont;
   public ToolStripMenuItem        mi_Filter;
   public ToolStripMenuItem        miSub_SubModulSetToolStripVisible, mi_ReportZoom,
                                   aTopSetSubModul, reportTopMenuItem;

   private ToolStripMenuItem[]     aModulMenuItem, aTopMenuItem;

   public ToolStripMenuItem[][]    aSubModulMenuItem, aSubTopMenuItem,
                                   aMenuItem4ContexMenu_Modul, aMenuItem4ContexMenu_Record,
                                   aMenuItem4ContexMenu_Report,
                                   aTopMenuItem_SubModulSet;

   private ToolStripMenuItem[][][] aSubModulSet_menuItem, aMenuItem4ContexMenu_SubModulSet;

   private ToolStripMenuItem[][][] aReportModulMenuItem;

   //private ToolStripSeparator      buttonSeparator;
   private ToolStripSeparator[][]  aSeparatorMenuItem;

   private ToolStripPanel          tsPanel_Record, tsPanel_Modul;
   public  ToolStripPanel          tsPanel_SubModul;
   private ToolStrip               ts_Modul;
   public  ToolStrip               ts_Record, ts_Report;

   public  ToolStrip[][]            ats_SubModulSet;

   public ToolStripButton tsBtn_UpDownTPage, tsBtn_ActivPrjkt, tsBtn_Filter, tsb_startLink, tsb_endLink;
   private ToolStripButton[][]     atsBtn_RecordRep, atsBtn_SubModul;
   public  ToolStripButton[][][]   atsBtn_SubModulSet, atsBtn_ReportViewerOnRecord;
   private ToolStripDropDownButton addRemoveBtn_onToolsStripModul, addRemoveBtn_onToolStripRecord,
                                   zoomButton_OnReport;
   private ToolStripDropDownButton[][] addRemoveBtn_onToolStripSubModulSet;

   private ToolStripSplitButton tsSplitBtn_ViewModulPanel;

   private ContextMenuStrip     contextMenuStrip_LeftRightModulPanel,
                                contextxMenuStrip4AddRemoveBtn_OnModulToolStrip,
                                contextxMenuStrip4AddRemoveBtn_OnRecordToolStrip;
   private ContextMenuStrip[][] ctxMenuStr4AddRmvBtn_OnSubModulSetts;
   
   public ToolStripComboBox tsCbxVvDataBase, tsCbxSorterType, tsCbxReport;

   private StatusStrip          statusStrip;
   private ToolStripStatusLabel tStripStatusLabel, ssDate, ssTime;

   private int numOfXModul = 0;

   public VvEnvironmentDescriptor.VvToolStripItem_State[] VvTsiRecordDefaultStates { get; set; }
   public VvEnvironmentDescriptor.VvToolStripItem_State[] VvTsiModulDefaultStates { get; set; }

   #endregion Menus_ToolStrips

   protected VvEnvironmentDescriptor vvEnvironmentDescriptor;
   private XmlSerializer envDesXmlser = new XmlSerializer(typeof(VvEnvironmentDescriptor));

   private ToolStripMenuItem mi_SrednjeIcone;
   private int   iconSize       = 32;
   private int   isImageAndText = 1;

   public ToolStripProgressBar progressBar;

   public int  numberOfRunningWorkers = 0;
   public bool quickPrintInitiated = false;

   // 10.04.2013. Q CHECK ovaj statusTextBackup je bio null, pa je iz toga proiylazilo i statusLabelText null i mislim  da je to smetalo za skakanje ekrana
   // kad sam maknula statusStrip onda nis nije skakalo i sad kad se statusTextBackup napuni sa necime isto ne skace, zapravo nisam sigurna cemu to sluzi
 //public string statusTextBackup;
   public string statusTextBackup = "...";

   public VvPref VvPref { get; set; }

   #endregion Fieldz

   /// <summary>
   /// This will create a Application Reference file on the users desktop
   /// if they do not already have one when the program is loaded.
   //    If not debugging in visual studio check for Application Reference
   //    #if (!debug)
   //        CheckForShortcut();
   //    #endif
   /// </summary
   void CheckForShortcut()
   {
      ApplicationDeployment ad = null;

      try
      {
         ad = ApplicationDeployment.CurrentDeployment;
      }
      catch 
      {
         return;  
      }

      if(ad.IsFirstRun)
      {
         // Q: 
         //Assembly code = Assembly.GetExecutingAssembly();
         Assembly code = Assembly.GetEntryAssembly();

         string company     = string.Empty;
         string productName = string.Empty;

         if(Attribute.IsDefined(code, typeof(AssemblyCompanyAttribute)))
         {
            AssemblyCompanyAttribute ascompany = (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(code, typeof(AssemblyCompanyAttribute));
            company = ascompany.Company;
         }

         if(Attribute.IsDefined(code, typeof(AssemblyDescriptionAttribute)))
         {
            AssemblyProductAttribute asproduct = (AssemblyProductAttribute)Attribute.GetCustomAttribute(code, typeof(AssemblyProductAttribute));
            productName = asproduct.Product;
         }

         if(company != string.Empty && productName != string.Empty)
         {
            string desktopPath = string.Empty;

            desktopPath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "\\", productName, ".appref-ms");

            string shortcutName = string.Empty;

            shortcutName = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "\\", company, "\\", productName, ".appref-ms");

            System.IO.File.Copy(shortcutName, desktopPath, true);

            //ZXC.aim_emsg("Kreirao Desktop Shortcut: shcutName [{0}]   dsktpPath [{1}]", shortcutName, desktopPath);
         }
         else
         {
            ZXC.aim_emsg("Shortcut problem: company [{0}] product [{1}]", company, productName);
         }
      }
   }

}