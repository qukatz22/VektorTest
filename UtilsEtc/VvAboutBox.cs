using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Configuration;
using System.Deployment.Application;
using Crownwood.DotNetMagic.Forms;
using System.Net;

public class VvAboutBox : DotNetMagicForm
{
   #region Fieldz

   private VvHamper  hamp_Assembly, hamp_Application, hamp_Buttons;
   
   private VvTextBox tbx_ProductName, /*tbx_Version,*/ tbx_Copyright, tbx_CompanyName,
                     tbx_ServerHost, tbx_UserName, tbx_Year, tbx_VvDomena,
                     tbx_ApplicationVersion, tbx_ApplicationStartupPath, tbx_UserConfigFilePath,
                     tbx_VvPrefFilePath, tbx_VvEnvironDescrFilePath, tbx_AssemblyVersion,
                     tbx_DataDirectory, tbx_IsFirstRun, tbx_TimeOfLastUpdateCheck,
                     tbx_UpdatedApplicationFullName, tbx_UpdatedVersion, tbx_UpdateLocation, tbx_TargetFramework;
   
   private Button    okButton, ChkUpdateBtn;
   private int       nextX, nextY;
   
   #endregion Fieldz

   #region Constructor

   public VvAboutBox()
   {
      InitializeVvAboutBox();
      CreateHampers();

      CalcSize();

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
   }


   #endregion Constructor

   #region VvAboutBox

   private void InitializeVvAboutBox()
   {
      this.Style = ZXC.vvColors.vvform_VisualStyle;
      this.Text  = String.Format("About {0} {1}", AssemblyCompany, AssemblyTitle);

      nextX = nextY = ZXC.Qun4;

      this.Resize += new EventHandler(VvAboutBox_Resize);
   }
   
   private void CalcSize()
   {
      this.Size        = new Size(hamp_Application.Width + 2 * ZXC.QunMrgn, hamp_Buttons.Bottom + 4 * ZXC.QunMrgn);
      this.MinimumSize = new Size(hamp_Assembly.Width    + 2 * ZXC.QunMrgn, hamp_Buttons.Bottom + 4 * ZXC.QunMrgn);
   }

   void VvAboutBox_Resize(object sender, EventArgs e)
   {
      hamp_Application.VvColWdt[1] = this.Width - ZXC.Q2un - hamp_Application.VvColWdt[0];
      hamp_Buttons.VvColWdt[1]     = this.Width - ZXC.Q2un - hamp_Buttons.VvColWdt[0] - hamp_Buttons.VvColWdt[2];
      
      hamp_Application.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
      hamp_Buttons.Anchor     = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
   }

   #endregion VvAboutBox

   #region Hampers

   private void CreateHampers()
   {
      Initialize_hamperAssembly(out hamp_Assembly);
      nextY = hamp_Assembly.Bottom;
      Initialize_hamperApplication(out hamp_Application);
      nextY = hamp_Application.Bottom;
      Initialize_hamperButtons(out hamp_Buttons);
   }


   private void Initialize_hamperAssembly(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 4, "", this, true, nextX, nextY, ZXC.Qun4);

      hamper.VvColWdt      = new int[] { ZXC.Q8un, ZXC.Q10un, ZXC.Q4un, ZXC.Q7un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
   
      Label labelProductName        = hamper.CreateVvLabel(0, 0, "ProductName:"       , ContentAlignment.MiddleRight);
      Label labelApplicationVersion = hamper.CreateVvLabel(0, 1, "ApplicationVersion:", ContentAlignment.MiddleRight);
      Label labelCopyright          = hamper.CreateVvLabel(0, 2, "Copyright:"         , ContentAlignment.MiddleRight);
      Label labelCompanyName        = hamper.CreateVvLabel(0, 3, "CompanyName:"       , ContentAlignment.MiddleRight);

      hamper.CreateVvLabel(2, 0, "ServerHost:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 1, "VvDomena:"  , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 2, "UserName:"  , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 3, "Godina:"    , ContentAlignment.MiddleRight);

      tbx_ProductName        = hamper.CreateVvTextBox(1, 0, "tbx_ProductName"       , "");
      tbx_ApplicationVersion = hamper.CreateVvTextBox(1, 1, "tbx_ApplicationVersion", "");
      tbx_Copyright          = hamper.CreateVvTextBox(1, 2, "tbx_Copyright"         , "");
      tbx_CompanyName        = hamper.CreateVvTextBox(1, 3, "tbx_CompanyName"       , "");

      tbx_ServerHost = hamper.CreateVvTextBox(3, 0, "tbx_ServerHost", "");
      tbx_VvDomena   = hamper.CreateVvTextBox(3, 1, "tbx_VvDomena"  , "");
      tbx_UserName   = hamper.CreateVvTextBox(3, 2, "tbx_UserName"  , "");
      tbx_Year       = hamper.CreateVvTextBox(3, 3, "tbx_Year"      , "");

      tbx_ProductName       .JAM_ReadOnly =
      tbx_ApplicationVersion.JAM_ReadOnly =
      tbx_Copyright         .JAM_ReadOnly =
      tbx_CompanyName       .JAM_ReadOnly = 
      tbx_ServerHost        .JAM_ReadOnly =
      tbx_VvDomena          .JAM_ReadOnly =
      tbx_UserName          .JAM_ReadOnly =
      tbx_Year              .JAM_ReadOnly = true;

      tbx_ProductName       .Text = AssemblyProduct;
      tbx_ApplicationVersion.Text = VvProgramVersion;           
      tbx_Copyright         .Text = AssemblyCopyright;
      tbx_CompanyName       .Text = AssemblyCompany;

      tbx_ServerHost.Text = ZXC.vvDB_Server  ;
      tbx_VvDomena  .Text = ZXC.vvDB_VvDomena; 
      tbx_UserName  .Text = ZXC.vvDB_User ;
      tbx_Year      .Text = ZXC.projectYear;

   }

   private void Initialize_hamperApplication(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 12, "", this, true, nextX, nextY, ZXC.Qun4);

      hamper.VvColWdt      = new int[] { ZXC.Q8un, 2*ZXC.Q10un + ZXC.QUN + ZXC.Qun4 + ZXC.Qun8};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
      Label labelTargetFramework            = hamper.CreateVvLabel(0,  0, "TargetFramework:"           , ContentAlignment.MiddleRight);
      Label labelAssemblyVersion            = hamper.CreateVvLabel(0,  1, "AssemblyVersion:"           , ContentAlignment.MiddleRight);
      Label labelApplicationStartupPath     = hamper.CreateVvLabel(0,  2, "ApplicationStartupPath:"    , ContentAlignment.MiddleRight);
      Label labelUserConfigFilePath         = hamper.CreateVvLabel(0,  3, "UserConfigFilePath:"        , ContentAlignment.MiddleRight);
      Label labelVvPrefFilePath             = hamper.CreateVvLabel(0,  4, "VvPrefFilePath:"            , ContentAlignment.MiddleRight);
      Label labelVvEnvironDescrFilePath     = hamper.CreateVvLabel(0,  5, "VvEnvironDescrFilePath:"    , ContentAlignment.MiddleRight);
      Label labelDataDirectory              = hamper.CreateVvLabel(0,  6, "DataDirectory:"             , ContentAlignment.MiddleRight);    
      Label labelIsFirstRun                 = hamper.CreateVvLabel(0,  7, "IsFirstRun:"                , ContentAlignment.MiddleRight);    
      Label labelTimeOfLastUpdateCheck      = hamper.CreateVvLabel(0,  8, "TimeOfLastUpdateCheck:"     , ContentAlignment.MiddleRight);
      Label labelUpdatedApplicationFullName = hamper.CreateVvLabel(0,  9, "UpdatedApplicationFullName:", ContentAlignment.MiddleRight);
      Label labelUpdatedVersion             = hamper.CreateVvLabel(0, 10, "UpdatedVersion:"            , ContentAlignment.MiddleRight);
      Label labelUpdateLocation             = hamper.CreateVvLabel(0, 11, "UpdateLocation:"            , ContentAlignment.MiddleRight);

      tbx_TargetFramework            = hamper.CreateVvTextBox(1,  0, "tbx_TargetFramework           ", "");
      tbx_AssemblyVersion            = hamper.CreateVvTextBox(1,  1, "tbx_AssemblyVersion           ", "");
      tbx_ApplicationStartupPath     = hamper.CreateVvTextBox(1,  2, "tbx_ApplicationStartupPath    ", "");
      tbx_UserConfigFilePath         = hamper.CreateVvTextBox(1,  3, "tbx_UserConfigFilePath        ", "");
      tbx_VvPrefFilePath             = hamper.CreateVvTextBox(1,  4, "tbx_VvPrefFilePath            ", "");
      tbx_VvEnvironDescrFilePath     = hamper.CreateVvTextBox(1,  5, "tbx_VvEnvironDescrFilePath    ", "");
      tbx_DataDirectory              = hamper.CreateVvTextBox(1,  6, "tbx_DataDirectory             ", "");
      tbx_IsFirstRun                 = hamper.CreateVvTextBox(1,  7, "tbx_IsFirstRun                ", "");
      tbx_TimeOfLastUpdateCheck      = hamper.CreateVvTextBox(1,  8, "tbx_TimeOfLastUpdateCheck     ", "");
      tbx_UpdatedApplicationFullName = hamper.CreateVvTextBox(1,  9, "tbx_UpdatedApplicationFullName", "");
      tbx_UpdatedVersion             = hamper.CreateVvTextBox(1, 10, "tbx_UpdatedVersion            ", "");
      tbx_UpdateLocation             = hamper.CreateVvTextBox(1, 11, "tbx_UpdateLocation            ", "");

      tbx_ApplicationStartupPath    .JAM_ReadOnly =
      tbx_UserConfigFilePath        .JAM_ReadOnly =
      tbx_VvPrefFilePath            .JAM_ReadOnly =
      tbx_VvEnvironDescrFilePath    .JAM_ReadOnly =
      tbx_TargetFramework           .JAM_ReadOnly =
      tbx_AssemblyVersion           .JAM_ReadOnly =
      tbx_DataDirectory             .JAM_ReadOnly =
      tbx_IsFirstRun                .JAM_ReadOnly =
      tbx_TimeOfLastUpdateCheck     .JAM_ReadOnly =
      tbx_UpdatedApplicationFullName.JAM_ReadOnly =
      tbx_UpdatedVersion            .JAM_ReadOnly =
      tbx_UpdateLocation            .JAM_ReadOnly = true;

      tbx_TargetFramework           .Text = TargetFramework;     
      tbx_ApplicationStartupPath    .Text = ApplicationStartupPath;
      tbx_UserConfigFilePath        .Text = UserConfigFilePath;         
      tbx_VvPrefFilePath            .Text = VvPref.FilePath;            
      tbx_VvEnvironDescrFilePath    .Text = VvForm.EnvDesXmlserFilePath;
      tbx_AssemblyVersion           .Text = AssemblyVersion;            
      tbx_DataDirectory             .Text = VvApplicationDeployment == null ? "" : VvApplicationDeployment.DataDirectory;                                          
      tbx_IsFirstRun                .Text = VvApplicationDeployment == null ? "" : VvApplicationDeployment.IsFirstRun.ToString();                                  
      tbx_TimeOfLastUpdateCheck     .Text = VvApplicationDeployment == null ? "" : VvApplicationDeployment.TimeOfLastUpdateCheck.ToString(ZXC.VvDateAndTimeFormat);
      tbx_UpdatedApplicationFullName.Text = VvApplicationDeployment == null ? "" : VvApplicationDeployment.UpdatedApplicationFullName  ;                           
      tbx_UpdatedVersion            .Text = VvApplicationDeployment == null ? "" : VvApplicationDeployment.UpdatedVersion.ToString();                              
      tbx_UpdateLocation            .Text = VvApplicationDeployment == null ? "" : VvApplicationDeployment.UpdateLocation.ToString();

      tbx_ApplicationStartupPath    .Anchor =
      tbx_UserConfigFilePath        .Anchor =
      tbx_VvPrefFilePath            .Anchor =
      tbx_VvEnvironDescrFilePath    .Anchor =
      tbx_AssemblyVersion           .Anchor =
      tbx_TargetFramework           .Anchor =
      tbx_DataDirectory             .Anchor =
      tbx_IsFirstRun                .Anchor =
      tbx_TimeOfLastUpdateCheck     .Anchor =
      tbx_UpdatedApplicationFullName.Anchor =
      tbx_UpdatedVersion            .Anchor =
      tbx_UpdateLocation            .Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

   }

   private void Initialize_hamperButtons(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", this, true, nextX, nextY, ZXC.Qun4);

      hamper.VvColWdt      = new int[] { 2*ZXC.QunBtnW, hamp_Application.Width - 3*ZXC.QunBtnW -ZXC.QUN,ZXC.QunBtnW};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4   ,                                       0, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QunBtnH;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      ChkUpdateBtn = hamper.CreateVvButton(0, 0, new EventHandler(ChkUpdateBtn_Click), "Check For Updates");
     
      okButton     = hamper.CreateVvButton(2, 0, new EventHandler(OkBtn_Click)       , "OK");
      okButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
   }

   private void OkBtn_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Hampers

   #region Assembly Attribute Accessors

   public string TargetFramework
   {
      get
      {
         var targetFw = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(System.Runtime.Versioning.TargetFrameworkAttribute), false);
         return ((System.Runtime.Versioning.TargetFrameworkAttribute)((System.Runtime.Versioning.TargetFrameworkAttribute[])targetFw)[0]).FrameworkDisplayName;
      }
   }
   
   public string AssemblyTitle
   {
      get
      {
         object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
         if(attributes.Length > 0)
         {
            AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
            if(titleAttribute.Title != "")
            {
               return titleAttribute.Title;
            }
         }
         return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
      }
   }

   public string AssemblyVersion
   {
      get
      {
         return Assembly.GetExecutingAssembly().GetName().Version.ToString();
      }
   }

   public string AssemblyDescription
   {
      get
      {
         object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
         if(attributes.Length == 0)
         {
            return "";
         }
         return ((AssemblyDescriptionAttribute)attributes[0]).Description;
      }
   }

   public string AssemblyProduct
   {
      get
      {
         object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
         if(attributes.Length == 0)
         {
            return "";
         }

         #region Qukatz Addendum

         object[] attributesEntry = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);

         #endregion Qukatz Addendum

         return ((AssemblyProductAttribute)attributes     [0]).Product + " [" + 
                ((AssemblyProductAttribute)attributesEntry[0]).Product + "]";
      }
   }

   public string AssemblyCopyright
   {
      get
      {
         object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
         if(attributes.Length == 0)
         {
            return "";
         }
         return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
      }
   }

   public string AssemblyCompany
   {
      get
      {
         object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
         if(attributes.Length == 0)
         {
            return "";
         }
         return ((AssemblyCompanyAttribute)attributes[0]).Company;
      }
   }
 
   #endregion

   #region Q's

   private static ApplicationDeployment VvApplicationDeployment
   {
      get
      {
         if(ApplicationDeployment.IsNetworkDeployed == false) return null;
         
         ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

         return ad;
      }
   }

   public static string VvProgramVersion
   {
      get
      {
         if(VvApplicationDeployment == null) return "App isn't network deployed!";

         return VvApplicationDeployment.CurrentVersion.ToString();
      }
   }

   private string UserConfigFilePath
   {
      get
      {
         return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
      }
   }

   private string ApplicationStartupPath
   {
      get
      {
         return Application.StartupPath;
      }
   }


   // https://msdn.microsoft.com/en-us/library/windows/desktop/ms724832(v=vs.85).aspx
   // Operating system
   // 
   // Version number
   // 
   // Windows 10                 10.0* 
   // Windows Server 2016        10.0* 
   // Windows 8.1                6.3* 
   // Windows Server 2012 R2     6.3* 
   // Windows 8                  6.2 
   // Windows Server 2012        6.2 
   // Windows 7                  6.1 
   // Windows Server 2008 R2     6.1 
   // Windows Server 2008        6.0 
   // Windows Vista              6.0 
   // Windows Server 2003 R2     5.2 
   // Windows Server 2003        5.2 
   // Windows XP 64-Bit Edition  5.2 
   // Windows XP                 5.1 
   // Windows 2000               5.0 
   // 
   // * For applications that have been manifested for Windows 8.1 or Windows 10. Applications not manifested for Windows 8.1 or Windows 10 will return the Windows 8 OS version value (6.2). To manifest your applications for Windows 8.1 or Windows 10, refer to Targeting your application for Windows.

   public static string GetWindowsVersion(out int major, out int minor)
   {
      Version theVersion = Environment.OSVersion.Version;

      major = theVersion.Major;
      minor = theVersion.Minor;

      return theVersion.ToString();
   }

   #endregion Q's

   #region InstallUpdateSyncWithInfo

   public static void ChkUpdateBtn_Click(object sender, EventArgs e)
   {
      InstallUpdateSyncWithInfo();
   }

   private static void InstallUpdateSyncWithInfo()
   {
      UpdateCheckInfo info = null;

      if(ApplicationDeployment.IsNetworkDeployed)
      {
         ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

         try
         {
            info = ad.CheckForDetailedUpdate();

         }
         catch(DeploymentDownloadException dde)
         {
            ZXC.aim_emsg("The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
            return;
         }
         catch(InvalidDeploymentException ide)
         {
            ZXC.aim_emsg("Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message);
            return;
         }
         catch(InvalidOperationException ioe)
         {
            ZXC.aim_emsg("This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
            return;
         }

         if(info.UpdateAvailable)
         {
            Boolean doUpdate = true;

            if(!info.IsUpdateRequired)
            {
               DialogResult dr = MessageBox.Show("Nova verzija aplikacije je dostupna. Želite li obnoviti sada?", "Update Available", MessageBoxButtons.OKCancel);
               if(!(DialogResult.OK == dr))
               {
                  doUpdate = false;
               }
            }
            else
            {
               // Display a message that the app MUST reboot. Display the minimum required version.
               ZXC.aim_emsg("This application has detected a mandatory update from your current " +
                   "version to version " + info.MinimumRequiredVersion.ToString() +
                   ". The application will now install the update and restart.");
            }

            if(doUpdate)
            {
               try
               {
                  ad.Update();
                  ZXC.aim_emsg("Aplikacija je obnovljena, te će biti restartana.");
                  Application.Restart();
               }
               catch(DeploymentDownloadException dde)
               {
                  ZXC.aim_emsg("Cannot install the latest version of the application. \n\nPlease check your network connection, or try again later. Error: " + dde);
                  return;
               }
            }
         } // if(info.UpdateAvailable) 
         else
         {
            ZXC.aim_emsg("Nema nove verzije.");
         }
      } // if(ApplicationDeployment.IsNetworkDeployed)
      else
      {
         ZXC.aim_emsg("Zis App isn't NetworkDeployed!");
      }
   }

   public static void CheckForAndInstallUpdateSilent()
   {
      UpdateCheckInfo info = null;

      if(ApplicationDeployment.IsNetworkDeployed)
      {
         ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

         try
         {
            info = ad.CheckForDetailedUpdate();

         }
         catch(DeploymentDownloadException dde)
         {
            ZXC.aim_emsg("The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
            return;
         }
         catch(InvalidDeploymentException ide)
         {
            ZXC.aim_emsg("Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message);
            return;
         }
         catch(InvalidOperationException ioe)
         {
            ZXC.aim_emsg("This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
            return;
         }

         if(info.UpdateAvailable)
         {
            Boolean doUpdate = true;

          //if(!info.IsUpdateRequired)
          //{
          //   DialogResult dr = MessageBox.Show("Nova verzija aplikacije je dostupna. Želite li obnoviti sada?", "Update Available", MessageBoxButtons.OKCancel);
          //   if(!(DialogResult.OK == dr))
          //   {
          //      doUpdate = false;
          //   }
          //}
          //else
          //{
          //   // Display a message that the app MUST reboot. Display the minimum required version.
          //   ZXC.aim_emsg("This application has detected a mandatory update from your current " +
          //       "version to version " + info.MinimumRequiredVersion.ToString() +
          //       ". The application will now install the update and restart.");
          //}

            if(doUpdate)
            {
               try
               {
                  ad.Update();
                  ZXC.aim_emsg("Aplikacija je obnovljena, te će biti restartana.");
                  Application.Restart();
               }
               catch(DeploymentDownloadException dde)
               {
                  ZXC.aim_emsg("Cannot install the latest version of the application. \n\nPlease check your network connection, or try again later. Error: " + dde);
                  return;
               }
            }
         } // if(info.UpdateAvailable) 
         else
         {
            ZXC.aim_emsg("There's no new application update.");
         }
      } // if(ApplicationDeployment.IsNetworkDeployed)
      else
      {
         ZXC.aim_emsg("Zis App isn't NetworkDeployed!");
      }
   }

   #endregion InstallUpdateSyncWithInfo

   public static void CheckSpeed(object sender, EventArgs e)
   {
      int iterrations = 20;

      decimal[] speedsKB = new decimal[iterrations];
      decimal[] speedsMB = new decimal[iterrations];

      for(int i = 0; i < iterrations; i++)
      {
         decimal DLtestFileSize_KB = 102.40M; //Size of File in KB.
         decimal DLtestFileSize_MB =   0.10M; //Size of File in MB.

         WebClient client = new WebClient();

         DateTime startTime = DateTime.Now;

         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\DLtest" + /*"_" + startTime.ToString(ZXC.VvDateYyyyMmDdFormat) + */".txt";

         client.DownloadFile("http://www.viper.hr/DLtest.txt", fName);

         DateTime endTime = DateTime.Now;

         decimal diffSeconds = (decimal)(endTime - startTime).TotalSeconds;

         if(diffSeconds.IsZero()) diffSeconds = 0.01M;

         speedsKB[i] = Math.Round((DLtestFileSize_KB / diffSeconds));
         speedsMB[i] = Math.Round((DLtestFileSize_MB / diffSeconds));

      }

      ZXC.aim_emsg(MessageBoxIcon.Information, "Download Speed:\n\r{0} KB/s\n\r{1} MB/s", speedsKB.Average(), speedsMB.Average());
      ZXC.aim_log (                            "Download Speed: {0} KB/s {1} MB/s"      , speedsKB.Average(), speedsMB.Average());

   }
}

public class VvMessageBoxDLG :  VvDialog
{
   public  VvMessageBox_UC TheUC { get; set; }
   public Button okButton, cancelButton, sendTosupportButton;
   private int dlgWidth, dlgHeight;
   internal string TextForSupportMailFromAddition { get; set; }
   internal string TextForSupportMailBody { get; set; }

   public VvMessageBoxDLG(bool _smallFont, ZXC.VvmBoxKind vvmBoxKind)
   {
      ZXC.CurrentForm = this;

      TheUC = new VvMessageBox_UC(this, _smallFont, this is VvMessageBoxForm, vvmBoxKind);

      SuspendLayout();

      this.Font        = ZXC.vvFont.BaseFont;
      this.Style       = ZXC.vvColors.vvform_VisualStyle;
      this.BackColor   = ZXC.vvColors.userControl_BackColor;
    //this.StartPosition = FormStartPosition.CenterScreen;

      this.Text          = "MessageBox";

      TheUC.Parent   = this;
      TheUC.Location = new Point(ZXC.Qun8, ZXC.Qun8);
      
      dlgWidth  = TheUC.Width;
      dlgHeight = TheUC.Height + ZXC.QunMrgn * 2 + ZXC.QunBtnH;

      this.MaximizeBox = true;

      this.ClientSize = new Size(dlgWidth, dlgHeight);

      if(vvmBoxKind == ZXC.VvmBoxKind.F2_SEND_candidates || vvmBoxKind == ZXC.VvmBoxKind.F2_MAP_candidates)
      {
         AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
         okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      }
      else if(vvmBoxKind == ZXC.VvmBoxKind.F2_webApiResults || vvmBoxKind == ZXC.VvmBoxKind.aim_emsg_List)
      {
         AddSendCancelButtons(out sendTosupportButton, out cancelButton, dlgWidth, dlgHeight);
         sendTosupportButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
         sendTosupportButton.Click += new EventHandler(MailToSupport_Click);
      }
      else
      {
         AddOkButton(out okButton, dlgWidth, dlgHeight);
         okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      }

      this.AcceptButton = okButton;

      TheUC.Anchor         =  AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      TheUC.TheGrid.Anchor =  AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

      if(vvmBoxKind == ZXC.VvmBoxKind.BarCodeInfo)
      {
         this.StartPosition = FormStartPosition.Manual;
         this.Location = new Point(ZXC.TheVvForm.Right - dlgWidth - ZXC.QUN, 0);
      }
      else
      {
         this.StartPosition = FormStartPosition.CenterScreen;
      }

      ResumeLayout();

   }
   
   private void MailToSupport_Click(object sender, EventArgs e)
   {
     VvMailClient mailClient = new VvMailClient();
     
   //mailClient.EmailFromPasswd      = ZXC.SkyLabEmailPassword;
   //mailClient.MailHost             = ZXC.ViperMailHost      ;
   //mailClient.EmailFromAddress     = 
   //mailClient.EmailFromUserName    = ZXC.SkyLabEmailAddress ;
   //mailClient.EmailFromDisplayName = ZXC.VvDeploymentSite + " VvException";
   
   //mailClient.PortNo               = Fld_Port               ;
   //mailClient.EnableSSL            = Fld_UseSSL             ;
   //mailClient.IsCcToMySelf         = Fld_PoslajiKopijuSebi  ;
   
     mailClient.SetVvSupportMailData((TextForSupportMailFromAddition.IsEmpty() ? " VvException" : TextForSupportMailFromAddition));
   
     mailClient.MessageSubject = this.Text;
   
     mailClient.MessageBody = String.Format("{0}\n\rProjekt/Year: {1}\n\r{2}\n\r~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\r", 
        ZXC.TheVvForm.Text, 
        ZXC.CURR_prjkt_rec.ToString() + " - " + ZXC.projectYear,
        ZXC.TheVvForm.TheVvUC.ToString());
   
     mailClient.MessageBody += TextForSupportMailBody;
   
     mailClient.SendMail_Normal(true, ZXC.VektorEmailAddress);
   }

}

public class VvMessageBox_UC : UserControl
{
   #region Fieldz

   public VvDataGridView TheGrid { get; set; }

   private VvTextBox vvtb_message,
                     vvtb_barkod, vvtb_kol, vvtb_artiklCd, vvtb_artiklName, vvtb_tserial, vvtb_status,
                     vvtb_datum, vvtb_tipBr, vvtb_partner, vvtb_ulaz, vvtb_izlaz, vvtb_stanje, vvtb_tmpMinus,
                     vvtb_iznos, tbx_numOfFirstLinesOnly_Send, tbx_numOfFirstLinesOnly_MAP,
                     vvtb_datumUpl, vvtb_iznosUpl, vvtb_tipUpl, vvtb_opis, vvtb_nalRed, vvtb_konto;

   private CheckBox  cbx_IsAutoSend, cbx_isAutoMAP,cbx_skip ;
   private VvTextBoxColumn colVvText;
   private DataGridViewTextBoxColumn colScrol;
   private DataGridViewCheckBoxColumn colCbxClassic;
   int colWidth = 0;
   private bool smallFont;
   private bool isMultiColumn;
   private ZXC.VvmBoxKind vvmBoxKind;
   private VvHamper hamper_F2_SEND_candidates, hamper_F2_MAP_candidates, hamper_F2_SEND_stop, hamper_F2_MAP_stop;

   #endregion Fieldz

   #region Constructor

   public VvMessageBox_UC(Control _parent, bool _smallFont, bool _isMultiColumn, ZXC.VvmBoxKind _vvmBoxKind)
   {
      this.SuspendLayout();

      this.Parent = _parent;

      this.smallFont     = _smallFont    ;
      this.isMultiColumn = _isMultiColumn;
      this.vvmBoxKind    = _vvmBoxKind   ;

      colWidth = ZXC.Q10un * 4 + ZXC.Q3un;

      if(vvmBoxKind == ZXC.VvmBoxKind.F2_SEND_candidates)
      {
         CreateHamper_F2_SEND_stop      (out hamper_F2_SEND_stop);
         CreateHamper_F2_SEND_candidates(out hamper_F2_SEND_candidates);
      }
      if(vvmBoxKind == ZXC.VvmBoxKind.F2_MAP_candidates)
      {
         CreateHamper_F2_MAP_stop      (out hamper_F2_MAP_stop      );
         CreateHamper_F2_MAP_candidates(out hamper_F2_MAP_candidates);
      }

      CreateTheGrid();
      CalcLocationAndSize(vvmBoxKind);
      //PutDgvFields();

      this.ResumeLayout();

      SetColumnIndexes();
   }

   #endregion Constructor

   #region CalcLocationAndSize

   private void CalcLocationAndSize(ZXC.VvmBoxKind vvmBoxKind)
   {
           if(vvmBoxKind == ZXC.VvmBoxKind.BarCodeInfo ) this.Size = new Size(TheGrid.Width + 2 * ZXC.QunMrgn, ZXC.Q10un * 2);
      else if(vvmBoxKind == ZXC.VvmBoxKind.RobnaKartica) this.Size = new Size(TheGrid.Width + 2 * ZXC.QunMrgn, ZXC.Q10un * 2);
      else                                               this.Size = new Size(TheGrid.Width + 2 * ZXC.QunMrgn, ZXC.Q10un * 3);

      TheGrid.Height = this.Size.Height - ZXC.Q2un;

      if(vvmBoxKind == ZXC.VvmBoxKind.F2_SEND_candidates)
      { 
         this.Size      = new Size(TheGrid.Width + 2 * ZXC.QunMrgn, ZXC.Q10un * 4);
         TheGrid.Height = this.Size.Height - ZXC.QUN - hamper_F2_SEND_stop.Height - ZXC.Qun4;
         hamper_F2_SEND_stop      .Location = new Point(ZXC.QunMrgn, TheGrid.Bottom + ZXC.Qun2 );
         hamper_F2_SEND_candidates.Location = new Point(hamper_F2_SEND_stop.Right, TheGrid.Bottom);

         hamper_F2_SEND_stop      .Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
         hamper_F2_SEND_candidates.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      }
      if(vvmBoxKind == ZXC.VvmBoxKind.F2_MAP_candidates)
      { 
         this.Size      = new Size(TheGrid.Width + 2 * ZXC.QunMrgn, ZXC.Q10un * 4);
         TheGrid.Height = this.Size.Height - ZXC.QUN - hamper_F2_MAP_candidates.Height;
         hamper_F2_MAP_stop      .Location = new Point(ZXC.QunMrgn, this.Size.Height - ZXC.QUN);
         hamper_F2_MAP_candidates.Location = new Point(ZXC.Q10un*3 + ZXC.Q2un, TheGrid.Bottom);

         hamper_F2_MAP_stop      .Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
         hamper_F2_MAP_candidates.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      }

   }

   #endregion CalcLocationAndSize

   #region Hampers

   private void CreateHamper_F2_SEND_candidates(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", this, false, ZXC.QunMrgn, ZXC.QunMrgn, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q2un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] {  ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                                     hamper.CreateVvLabel  (0, 0, "Pošalji samo prvih:", ContentAlignment.MiddleRight);
      tbx_numOfFirstLinesOnly_Send = hamper.CreateVvTextBox(1, 0, "tbx_numOfFirstLinesOnly", "NumOfFirstLinesOnly");
      tbx_numOfFirstLinesOnly_Send.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
                                     hamper.CreateVvLabel  (2, 0, " računa", ContentAlignment.MiddleLeft);

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvOtherUC);
   }
      
   private void CreateHamper_F2_SEND_stop(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 2, "", this, false, ZXC.QunMrgn, ZXC.QunMrgn, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q10un*2};
      hamper.VvSpcBefCol   = new int[] {  ZXC.Qun4   };
      hamper.VvRightMargin = hamper.VvLeftMargin;

    //hamper.VvRowHgt       = new int[] { ZXC.QUN-ZXC.Qun4, ZXC.QUN - ZXC.Qun4};
      hamper.VvRowHgt       = new int[] { ZXC.QUN-ZXC.Qun8, ZXC.QUN - ZXC.Qun8 };
      hamper.VvSpcBefRow    = new int[] { 0      , 0       };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbx_IsAutoSend = hamper.CreateVvCheckBox_OLD(0, 0, null, "Ubuduće, Automatski pošalji eRačune", RightToLeft.No);
                       hamper.CreateVvLabel(0,1, "     (Prilikom otvaranja FIR-a, a uz dodatnu potvrdu)", ContentAlignment.MiddleLeft);

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvOtherUC);
   }

   private void CreateHamper_F2_MAP_candidates(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", this, false, ZXC.QunMrgn, ZXC.QunMrgn, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q10un, ZXC.Q2un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] {  ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                                    hamper.CreateVvLabel  (0, 0, "Pošalji samo prvih:", ContentAlignment.MiddleRight);
      tbx_numOfFirstLinesOnly_MAP = hamper.CreateVvTextBox(1, 0, "tbx_numOfFirstLinesOnly_MAP", "NumOfFirstLinesOnly");
      tbx_numOfFirstLinesOnly_MAP.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
                                    hamper.CreateVvLabel  (2, 0, " uplata", ContentAlignment.MiddleLeft);

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvOtherUC);
   }
      
   private void CreateHamper_F2_MAP_stop(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", this, false, ZXC.QunMrgn, ZXC.QunMrgn, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q10un*3 };
      hamper.VvSpcBefCol   = new int[] {  ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { 0 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbx_isAutoMAP = hamper.CreateVvCheckBox_OLD(0, 0, null, "Ubuduće, Automatski prijavi uplate (prilikom otvaranja FIR-a, a uz dodatnu potvrdu)", RightToLeft.No);

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvOtherUC);
   }

   #endregion Hampers

   #region TheGrid

   private void CreateTheGrid()
   {
      TheGrid          = new VvDataGridView();
      TheGrid.Parent   = this;
      TheGrid.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);
    //TheGrid.ReadOnly = true;
      TheGrid.ReadOnly = (vvmBoxKind == ZXC.VvmBoxKind.F2_SEND_candidates || vvmBoxKind == ZXC.VvmBoxKind.F2_MAP_candidates) ? false : true;

      TheGrid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
      TheGrid.AutoGenerateColumns                  = false;

      TheGrid.RowHeadersBorderStyle = TheGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
      TheGrid.ColumnHeadersHeight   = ZXC.QUN;
      TheGrid.RowTemplate.Height    = ZXC.Q2un;
      TheGrid.RowHeadersWidth       = ZXC.Q3un;
      TheGrid.Height                = TheGrid.ColumnHeadersHeight + TheGrid.RowTemplate.Height;

      //VvHamper.ApplyVVColorAndStyleTabCntrolChange(TheGrid);
      //VvHamper.Open_Close_Fields_ForWriting(TheGrid, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);

      TheGrid.AllowUserToAddRows       =
      TheGrid.AllowUserToDeleteRows    =
      TheGrid.AllowUserToOrderColumns  =
    //TheGrid.AllowUserToResizeColumns =
      TheGrid.AllowUserToResizeRows    = false;

      TheGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

      TheGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.PowderBlue;
      TheGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.DarkSlateGray;
      TheGrid.RowHeadersDefaultCellStyle.BackColor    = Color.PowderBlue; //Color.FloralWhite;
      TheGrid.RowHeadersDefaultCellStyle.ForeColor    = Color.DarkSlateGray;

      TheGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.Azure;

      if(smallFont)
      { 
         TheGrid.ColumnHeadersDefaultCellStyle.Font = 
         TheGrid.RowsDefaultCellStyle         .Font = 
         TheGrid.RowHeadersDefaultCellStyle   .Font = ZXC.vvFont.SmallFont;
      }

      if(vvmBoxKind == ZXC.VvmBoxKind.BarCodeInfo)
      {
         CreateMultiColumn_Barkod(TheGrid);
         TheGrid.Width  = ZXC.Q10un * 6 + ZXC.Q2un + TheGrid.RowHeadersWidth + ZXC.QUN + ZXC.Qun4 - ZXC.Q2un;
         TheGrid.Height = this.Size.Height - ZXC.QUN;
      }
      else if(vvmBoxKind == ZXC.VvmBoxKind.RobnaKartica)
      {
         CreateMultiColumn_Minus(TheGrid);
         TheGrid.Width  = ZXC.Q10un * 3 + ZXC.Q7un - ZXC.Qun2 + TheGrid.RowHeadersWidth;
         TheGrid.Height = this.Size.Height - ZXC.QUN;
      }
      else if(vvmBoxKind == ZXC.VvmBoxKind.F2_SEND_candidates)
      {
         CreateMultiColumn_F2_SEND_candidates(TheGrid);
         TheGrid.Width  = ZXC.Q10un * 3 + ZXC.Qun4 + TheGrid.RowHeadersWidth;
         TheGrid.Height = this.Size.Height - ZXC.QUN - hamper_F2_SEND_candidates.Height;
      }
      else if(vvmBoxKind == ZXC.VvmBoxKind.F2_MAP_candidates)
      {
         CreateMultiColumn_F2_MAP_candidates(TheGrid);
         TheGrid.Width  = ZXC.Q10un * 5 + ZXC.Q7un + ZXC.Qun4 + TheGrid.RowHeadersWidth;
         TheGrid.Height = this.Size.Height - ZXC.QUN - hamper_F2_MAP_candidates.Height;
      }
      else
      {
         CreateColumn(TheGrid);
         TheGrid.Width = colWidth + TheGrid.RowHeadersWidth + ZXC.QUN + ZXC.Qun4;
         TheGrid.Height = this.Size.Height - ZXC.QUN;
      }

      // micanje tamnoplavog polja iz datagrida 
      //
      TheGrid.TabStop = false;
      TheGrid.ClearSelection();
      //
      //                                         
   }

   #endregion TheGrid

   #region TheGridColumn

   private void CreateColumn(VvDataGridView theGrid)
   {
      vvtb_message = theGrid.CreateVvTextBoxFor_String_ColumnTemplate ("vvtb_message", null, -12, "Poruke");
      colVvText    = theGrid.CreateVvTextBoxColumn(vvtb_message, null, "R_message", "Poruke:", colWidth);
      vvtb_message.JAM_ReadOnly = true;
      //vvtb_message.Multiline = true;
      //vvtb_message.ScrollBars = ScrollBars.Vertical;
      colVvText.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);
      colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
   }

   private void CreateMultiColumn_Barkod(VvDataGridView theGrid)
   {
      CreateColumn_barkod    (theGrid, "Barkod"      , ZXC.Q5un              );
      CreateColumn_kol       (theGrid, "Kol"         , ZXC.Q3un              );
      CreateColumn_artiklCd  (theGrid, "ArtiklCD"    , ZXC.Q7un              );
      CreateColumn_artiklName(theGrid, "Artikl naziv", ZXC.Q10un*2 + ZXC.Q2un);
      CreateColumn_tserial   (theGrid, "Red"         , ZXC.Q2un + ZXC.Qun8   );
      CreateColumn_status    (theGrid, "Status"      , ZXC.Q4un              );
      CreateColumn_message   (theGrid, "Poruka"      , ZXC.Q10un + ZXC.Q7un  );

      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);
   }

   private void CreateMultiColumn_Minus(VvDataGridView theGrid)
   {
      CreateColumn_datum   (theGrid, "Datum"            , ZXC.Q5un            );
      CreateColumn_tipBr   (theGrid, "Dokument"         , ZXC.Q5un            );
      CreateColumn_partner (theGrid, "Partner"          , ZXC.Q10un + ZXC.Q3un);
      CreateColumn_ulaz    (theGrid, "Ulaz"             , ZXC.Q4un            );
      CreateColumn_izlaz   (theGrid, "Izlaz"            , ZXC.Q4un            );
      CreateColumn_stanje  (theGrid, "Stanje"           , ZXC.Q4un            );
      
      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);
   }

   private void CreateMultiColumn_F2_SEND_candidates(VvDataGridView theGrid)
   {
      CreateColumn_skip   (theGrid, "Preskoči"    , ZXC.Q3un            );

      CreateColumn_tipBr  (theGrid, "TipBr"   , ZXC.Q5un            );
      CreateColumn_datum  (theGrid, "Datum"   , ZXC.Q5un            );
      CreateColumn_partner(theGrid, "Partner" , ZXC.Q10un + ZXC.Q5un);
      CreateColumn_iznos  (theGrid, "Iznos"   , ZXC.Q4un            );
      
      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);
   }

   private void CreateMultiColumn_F2_MAP_candidates(VvDataGridView theGrid)
   {
      CreateColumn_skip   (theGrid, "Preskoči"    , ZXC.Q3un            );

      CreateColumn_tipBr   (theGrid, "TipBr"      , ZXC.Q5un            );
      CreateColumn_datum   (theGrid, "DatumRn"    , ZXC.Q5un            );
      CreateColumn_partner (theGrid, "Partner"    , ZXC.Q10un + ZXC.Q5un);
      CreateColumn_iznos   (theGrid, "IznosRn"    , ZXC.Q4un            );
      CreateColumn_datumUpl(theGrid, "DatumUpl"   , ZXC.Q4un            );
      CreateColumn_nalRed  (theGrid, "Nalog/Red"  , ZXC.Q5un            );
      CreateColumn_konto   (theGrid, "Konto"      , ZXC.Q3un            );
      CreateColumn_opis    (theGrid, "Opis uplate", ZXC.Q7un            );
      CreateColumn_iznosUpl(theGrid, "IznosUpl"   , ZXC.Q4un            );
      CreateColumn_tipUpl  (theGrid, "TipUpl"     , ZXC.Q3un            );

      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);
   }

   #region barkodColumns

   private void CreateColumn_barkod(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_barkod = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_barkod", null, -12, header);
      colVvText    = theGrid.CreateVvTextBoxColumn(vvtb_barkod, null, "R_barkod", header, colWidth);
      vvtb_barkod.JAM_ReadOnly = true;
   }

   private void CreateColumn_kol(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_kol = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb_kol", null, -12, header);
//    vvtb_kol = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_kol", null, -12, header);
      colVvText    = theGrid.CreateVvTextBoxColumn(vvtb_kol, null, "R_kol", header, colWidth);
      vvtb_kol.JAM_ReadOnly = true;
   }

   private void CreateColumn_artiklCd(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_artiklCd = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_artiklCd", null, -12, header);
      colVvText    = theGrid.CreateVvTextBoxColumn(vvtb_artiklCd, null, "R_artiklCd", header, colWidth);
      vvtb_artiklCd.JAM_ReadOnly = true;
   }

   private void CreateColumn_artiklName(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_artiklName = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_artiklName", null, -12, header);
      colVvText       = theGrid.CreateVvTextBoxColumn(vvtb_artiklName, null, "R_artiklName", header, colWidth);
      vvtb_artiklName.JAM_ReadOnly = true;
   }

   private void CreateColumn_tserial(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_tserial = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate(false, "vvtb_tserial", null, -12, header);
    //vvtb_tserial = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (       "vvtb_tserial", null, -12, header);
      colVvText    = theGrid.CreateVvTextBoxColumn(vvtb_tserial, null, "R_tserial", header, colWidth);
      vvtb_tserial.JAM_ReadOnly = true;
   }

   private void CreateColumn_status(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_status = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_status", null, -12, header);
      colVvText    = theGrid.CreateVvTextBoxColumn(vvtb_status, null, "R_status", header, colWidth);
      vvtb_status.JAM_ReadOnly = true;
   }

   private void CreateColumn_message(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_message = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_message", null, -12, header);
      colVvText    = theGrid.CreateVvTextBoxColumn(vvtb_message, null, "R_message", header, colWidth);
      vvtb_message.JAM_ReadOnly = true;
      colVvText.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
   }

   #endregion barkodColumns

   #region minus Columns

   private void CreateColumn_datum(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_datum = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_datum", null, -12, header);
      colVvText = theGrid.CreateVvTextBoxColumn(vvtb_datum, null, "R_datum", header, colWidth);
      vvtb_datum.JAM_ReadOnly = true;
      colVvText.ReadOnly = true;

   }

   private void CreateColumn_tipBr(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_tipBr = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_tipBr", null, -12, header);
      colVvText = theGrid.CreateVvTextBoxColumn(vvtb_tipBr, null, "R_tipBr", header, colWidth);
      vvtb_tipBr.JAM_ReadOnly = true;
      colVvText.ReadOnly = true;
   }
   private void CreateColumn_partner(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_partner = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_partner", null, -12, header);
      colVvText = theGrid.CreateVvTextBoxColumn(vvtb_partner, null, "R_partner", header, colWidth);
      vvtb_partner.JAM_ReadOnly = true;
      colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      colVvText.ReadOnly = true;
   }

   private void CreateColumn_ulaz(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_ulaz = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb_ulaz", null, -12, header);
      colVvText = theGrid.CreateVvTextBoxColumn(vvtb_ulaz, null, "R_ulaz", header, colWidth);
      vvtb_ulaz.JAM_ReadOnly = true;
      colVvText.ReadOnly = true;
   }

   private void CreateColumn_izlaz(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_izlaz = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb_izlaz", null, -12, header);
      colVvText = theGrid.CreateVvTextBoxColumn(vvtb_izlaz, null, "R_izlaz", header, colWidth);
      vvtb_izlaz.JAM_ReadOnly = true;
      colVvText.ReadOnly = true;
   }
   private void CreateColumn_stanje(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_stanje = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb_stanje", null, -12, header);
      colVvText = theGrid.CreateVvTextBoxColumn(vvtb_stanje, null, "R_stanje", header, colWidth);
      colVvText.DefaultCellStyle.Format = VvUserControl.GetDgvCellStyleFormat_Number(vvtb_stanje.JAM_NumberOfDecimalPlaces, false, false); // da prikaze 0.00
      vvtb_stanje.JAM_ReadOnly = true;
      colVvText.ReadOnly = true;
   }

   #endregion minus Columns

   #region F2_SEND_candidates Columns

   private void CreateColumn_iznos(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_iznos = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb_iznos", null, -12, header);
      colVvText  = theGrid.CreateVvTextBoxColumn(vvtb_iznos, null, "R_iznos", header, colWidth);
      colVvText.DefaultCellStyle.Format = VvUserControl.GetDgvCellStyleFormat_Number(vvtb_iznos.JAM_NumberOfDecimalPlaces, false, false); // da prikaze 0.00
      vvtb_iznos.JAM_ReadOnly = true;
      colVvText.ReadOnly = true;
   }

   #endregion F2_SEND_candidates Columns

   #region F2_MAP_candidates Columns

   private void CreateColumn_skip(VvDataGridView theGrid, string _colHeader, int _width)
   {
      cbx_skip      = new CheckBox();
      colCbxClassic = theGrid.CreateClassicCheckBoxColumn(cbx_skip, null, 333, _colHeader, _width);
   }

   private void CreateColumn_datumUpl(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_datumUpl = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_datumUpl", null, -12, header);
      colVvText     = theGrid.CreateVvTextBoxColumn(vvtb_datumUpl, null, "R_datumUpl", header, colWidth);
      vvtb_datumUpl.JAM_ReadOnly = true;
      colVvText.ReadOnly = true;
   }

   private void CreateColumn_iznosUpl(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_iznosUpl = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb_iznosUpl", null, -12, header);
      colVvText     = theGrid.CreateVvTextBoxColumn(vvtb_iznosUpl, null, "R_iznosUpl", header, colWidth);
      colVvText.DefaultCellStyle.Format = VvUserControl.GetDgvCellStyleFormat_Number(vvtb_iznosUpl.JAM_NumberOfDecimalPlaces, false, false); // da prikaze 0.00
      vvtb_iznosUpl.JAM_ReadOnly = true;
      colVvText.ReadOnly = true;

   }

   private void CreateColumn_tipUpl(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_tipUpl = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_tipUpl", null, -12, header);
      colVvText   = theGrid.CreateVvTextBoxColumn(vvtb_tipUpl, null, "R_tipUpl", header, colWidth);
      vvtb_tipUpl.JAM_ReadOnly = true;
      colVvText.ReadOnly = true;
      colVvText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
   }

   private void CreateColumn_opis(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_opis = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_opis", null, -12, header);
      colVvText = theGrid.CreateVvTextBoxColumn(vvtb_opis, null, "R_opis", header, colWidth);
      vvtb_opis.JAM_ReadOnly = true;
      colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      colVvText.ReadOnly = true;

   }

   private void CreateColumn_nalRed(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_nalRed = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_nalRed", null, -12, header);
      colVvText   = theGrid.CreateVvTextBoxColumn(vvtb_nalRed, null, "R_nalRed", header, colWidth);
      vvtb_nalRed.JAM_ReadOnly = true;
      colVvText.ReadOnly = true;

   }

   private void CreateColumn_konto(VvDataGridView theGrid, string header, int colWidth)
   {
      vvtb_konto = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_konto", null, -12, header);
      colVvText = theGrid.CreateVvTextBoxColumn(vvtb_konto, null, "R_konto", header, colWidth);
      vvtb_konto.JAM_ReadOnly = true;
      colVvText.ReadOnly = true;

   }

   #endregion F2_MAP_candidates Columns

   #endregion TheGridColumn

   #region SetColumnIndexes()

   private Message_colIdx ci;
   public Message_colIdx DgvCI { get { return ci; } }
   public struct Message_colIdx
   {
      internal int iT_message   ;
      internal int iT_barkod    ;
      internal int iT_kol       ;
      internal int iT_artiklCd  ;
      internal int iT_artiklName;
      internal int iT_tserial   ;
      internal int iT_status    ;

      internal int iT_datum     ;
      internal int iT_tipBr     ;
      internal int iT_partner   ;
      internal int iT_ulaz      ;
      internal int iT_izlaz     ;
      internal int iT_stanje    ;
      
      internal int iT_iznos     ;
      internal int iT_iznosUpl  ;
      internal int iT_datumUpl  ;
      internal int iT_opis      ;
      internal int iT_nalRed    ;
      internal int iT_konto     ;
      internal int iT_tipUpl    ;
      internal int iT_shouldS   ;
      internal int iT_ftrRecID  ;
   }

   private void SetColumnIndexes()
   {
      ci = new Message_colIdx();

      ci.iT_message    = TheGrid.IdxForColumn("R_message"   );
      ci.iT_barkod     = TheGrid.IdxForColumn("R_barkod"    );
      ci.iT_kol        = TheGrid.IdxForColumn("R_kol"       );
      ci.iT_artiklCd   = TheGrid.IdxForColumn("R_artiklCd"  );
      ci.iT_artiklName = TheGrid.IdxForColumn("R_artiklName");
      ci.iT_tserial    = TheGrid.IdxForColumn("R_tserial"   );
      ci.iT_status     = TheGrid.IdxForColumn("R_status"    );

      ci.iT_datum    = TheGrid.IdxForColumn("R_datum")  ;
      ci.iT_tipBr    = TheGrid.IdxForColumn("R_tipBr")  ;
      ci.iT_partner  = TheGrid.IdxForColumn("R_partner");
      ci.iT_ulaz     = TheGrid.IdxForColumn("R_ulaz")   ;
      ci.iT_stanje   = TheGrid.IdxForColumn("R_stanje") ;
      ci.iT_izlaz    = TheGrid.IdxForColumn("R_izlaz")  ;
      
      ci.iT_iznos    = TheGrid.IdxForColumn("R_iznos")  ;

      ci.iT_iznosUpl = TheGrid.IdxForColumn("R_iznosUpl");
      ci.iT_datumUpl = TheGrid.IdxForColumn("R_datumUpl");
      ci.iT_opis     = TheGrid.IdxForColumn("R_opis"    );
      ci.iT_nalRed   = TheGrid.IdxForColumn("R_nalRed"  );
      ci.iT_konto    = TheGrid.IdxForColumn("R_konto"   );
      ci.iT_tipUpl   = TheGrid.IdxForColumn("R_tipUpl"  );
      ci.iT_shouldS  = TheGrid.IdxForColumn("R_shouldS");
      ci.iT_ftrRecID = TheGrid.IdxForColumn("R_ftrRedID"); // TAMARA TODO dodati kolonu 

   }

   #endregion SetColumnIndexes()

   #region PutDgvFields

   public void PutDgvFields(List<string> messageList)
   {
      int rowIdx;

      TheGrid.Rows.Clear();

      for(rowIdx = 0; rowIdx < messageList.Count; ++rowIdx)
      {
         TheGrid.Rows.Add();

         TheGrid.PutCell(ci.iT_message, rowIdx, messageList[rowIdx]);

         TheGrid.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();

      }
      TheGrid.ClearSelection();
   }

   public void PutDgvFields_BarCodeInfo(List<VvReportSourceUtil> messageList)
   {
      int rowIdx;

      TheGrid.Rows.Clear();

      for(rowIdx = 0; rowIdx < messageList.Count; ++rowIdx)
      {
         TheGrid.Rows.Add();

         TheGrid.PutCell(ci.iT_barkod    , rowIdx, messageList[rowIdx].TheCD       );
         TheGrid.PutCell(ci.iT_kol       , rowIdx, messageList[rowIdx].Kol         );
         TheGrid.PutCell(ci.iT_artiklCd  , rowIdx, messageList[rowIdx].ArtiklGrCD  );
         TheGrid.PutCell(ci.iT_artiklName, rowIdx, messageList[rowIdx].ArtiklGrName);
         TheGrid.PutCell(ci.iT_tserial   , rowIdx, messageList[rowIdx].Count       );
         TheGrid.PutCell(ci.iT_status    , rowIdx, messageList[rowIdx].DevName     );
         TheGrid.PutCell(ci.iT_message   , rowIdx, messageList[rowIdx].KupdobName  );
         
         switch(messageList[rowIdx].DevName)
         {
            case "NEPOZNAT": TheGrid.SetDgvRowColor(rowIdx, Color.White, Color.Red       ); break;
            case "NEMA GA" : TheGrid.SetDgvRowColor(rowIdx, Color.White, Color.DarkViolet); break;
            case "MINUS"   : TheGrid.SetDgvRowColor(rowIdx, Color.White, Color.Green     ); break;
            case "NEPOTPUN": TheGrid.SetDgvRowColor(rowIdx, Color.White, Color.DarkBlue  ); break;
            default        : TheGrid.SetDgvRowColor(rowIdx, Color.White, Color.Black     ); break;
         }

         TheGrid.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();
      }

      TheGrid.ClearSelection();
   }

   public void PutDgvFields_RobnaKartica(List<VvReportSourceUtil> messageList)
   {
      int rowIdx;

      TheGrid.Rows.Clear();

      for(rowIdx = 0; rowIdx < messageList.Count; ++rowIdx)
      {
         TheGrid.Rows.Add();

         TheGrid.PutCell(ci.iT_datum   , rowIdx, messageList[rowIdx].TheDate.ToString(ZXC.VvDateFormat));
         TheGrid.PutCell(ci.iT_tipBr   , rowIdx, messageList[rowIdx].String1                           );
         TheGrid.PutCell(ci.iT_partner , rowIdx, messageList[rowIdx].KupdobName                        );
         TheGrid.PutCell(ci.iT_ulaz    , rowIdx, messageList[rowIdx].TheMoney                          );
         TheGrid.PutCell(ci.iT_izlaz   , rowIdx, messageList[rowIdx].TheMoney2                         );
         TheGrid.PutCell(ci.iT_stanje  , rowIdx, messageList[rowIdx].TheSaldo                          );

              if(messageList[rowIdx].IsNekakav            ) TheGrid.SetDgvRowColor(rowIdx, Color.Beige    , Color.Black);
         else if(messageList[rowIdx].TheSaldo.IsZero()    ) TheGrid.SetDgvRowColor(rowIdx, Color.AliceBlue, Color.Black);
         else                                               TheGrid.SetDgvRowColor(rowIdx, Color.White, Color.Black);

         TheGrid.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();
      }

      TheGrid.ClearSelection();
   }

   public void PutDgvFields_F2_SEND_candidates(List<VvReportSourceUtil> messageList)
   {
      int rowIdx;

      TheGrid.Rows.Clear();

      for(rowIdx = 0; rowIdx < messageList.Count; ++rowIdx)
      {
         TheGrid.Rows.Add();

         TheGrid.PutCell(ci.iT_datum   , rowIdx, messageList[rowIdx].DevName   );
         TheGrid.PutCell(ci.iT_tipBr   , rowIdx, messageList[rowIdx].TheCD     );
         TheGrid.PutCell(ci.iT_partner , rowIdx, messageList[rowIdx].KupdobName);
         TheGrid.PutCell(ci.iT_iznos   , rowIdx, messageList[rowIdx].TheMoney  );

         TheGrid.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();
      }

      TheGrid.ClearSelection();
   }

   public void PutDgvFields_F2_MAP_candidates(List<VvReportSourceUtil> messageList)
   {
      int rowIdx;

      TheGrid.Rows.Clear();

      for(rowIdx = 0; rowIdx < messageList.Count; ++rowIdx)
      {
         TheGrid.Rows.Add();

         TheGrid.PutCell(ci.iT_datum   , rowIdx, messageList[rowIdx].DevName   );
         TheGrid.PutCell(ci.iT_tipBr   , rowIdx, messageList[rowIdx].TheCD     );
         TheGrid.PutCell(ci.iT_partner , rowIdx, messageList[rowIdx].KupdobName);
         TheGrid.PutCell(ci.iT_iznos   , rowIdx, messageList[rowIdx].TheMoney  );
         TheGrid.PutCell(ci.iT_datumUpl, rowIdx, messageList[rowIdx].String1   );
         TheGrid.PutCell(ci.iT_iznosUpl, rowIdx, messageList[rowIdx].TheMoney2 );
         TheGrid.PutCell(ci.iT_tipUpl  , rowIdx, messageList[rowIdx].String2   );
         TheGrid.PutCell(ci.iT_konto   , rowIdx, messageList[rowIdx].String5   );
         TheGrid.PutCell(ci.iT_opis    , rowIdx, messageList[rowIdx].String4   );
         TheGrid.PutCell(ci.iT_nalRed  , rowIdx, messageList[rowIdx].String3   );

         TheGrid.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();
      }

      TheGrid.ClearSelection();
   }


   //private void PutDgvLineFields(int rowIdx)
   //{
   //  if(rowIdx == 0)TheGrid.PutCell(ci.iT_message , rowIdx, "Servis e-Račun za državu je internetski servis koji omogućuje razmjenu strukturiranih elektroničkih računa.Servis je u potpunosti usklađen sa zakonskom regulativom.Servis omogućuje upravljanje poslovnim procesom izdavanja, zaprimanja, prihvaćanja ili odbijanja zaprimljenog računa te arhiviranja istog na brži i sigurniji način." );
   //  if(rowIdx == 1)TheGrid.PutCell(ci.iT_message , rowIdx, "Korisnici web aplikacije trebaju imati digitalne certifikate na kripto uređaju za zaprimanje računa te njihov prihvat ili odbijanje.Za postupak likvidature korisnici mogu koristiti digitalne certifikate na kripto uređaju ili poslovne soft certifikate.Da bi korisnik mogao zaprimiti račun putem web aplikacije, barem jedna osoba treba imati certifikate na kripto uređaju kojoj će se dodijeliti pravo za Pregled i prihvat računa." );
   //  if(rowIdx == 2)TheGrid.PutCell(ci.iT_message , rowIdx, "Što je web aplikacija e-Računa za državu?");
   //  if(rowIdx == 3)TheGrid.PutCell(ci.iT_message , rowIdx, "Niste obvezni koristiti likvidaturu putem web aplikacije e-Račun za državu. Servis e-Račun za državu je internetski servis koji omogućuje razmjenu strukturiranih elektroničkih računa.Servis je u potpunosti usklađen sa zakonskom regulativom.Servis omogućuje upravljanje poslovnim procesom izdavanja, zaprimanja, prihvaćanja ili odbijanja zaprimljenog računa te arhiviranja istog na brži i sigurniji način.");
   //  if(rowIdx == 4)TheGrid.PutCell(ci.iT_message , rowIdx, "" );
   //  if(rowIdx == 5)TheGrid.PutCell(ci.iT_message , rowIdx, "" );
   //  if(rowIdx == 6)TheGrid.PutCell(ci.iT_message , rowIdx, "" );
   //  if(rowIdx == 7)TheGrid.PutCell(ci.iT_message , rowIdx, "" );
   //  if(rowIdx == 8)TheGrid.PutCell(ci.iT_message , rowIdx, "" );
   //  if(rowIdx == 9)TheGrid.PutCell(ci.iT_message , rowIdx, "" );
   //}

   #endregion PutDgvFields

   #region Fld
   
   public bool Fld_IsAutoSend               { get { return cbx_IsAutoSend.Checked; } set { cbx_IsAutoSend.Checked = value; } }
   
   public int  Fld_NumOfFirstLinesOnly_Send { get { return ZXC.ValOrZero_Int(tbx_numOfFirstLinesOnly_Send.Text); } set { tbx_numOfFirstLinesOnly_Send.Text = value.ToString(); } }
      
   public bool Fld_IsAutoMAP                { get { return cbx_isAutoMAP.Checked; } set { cbx_isAutoMAP.Checked = value; } }
   
   public int  Fld_NumOfFirstLinesOnly_MAP  { get { return ZXC.ValOrZero_Int(tbx_numOfFirstLinesOnly_MAP.Text); } set { tbx_numOfFirstLinesOnly_MAP.Text = value.ToString(); } }

   #endregion Fld
}

public class VvMessageBoxForm : VvMessageBoxDLG
{
   public VvMessageBoxForm(bool _smallFont, ZXC.VvmBoxKind vvmBoxKind) : base(_smallFont, vvmBoxKind)
   {
     
      this.okButton.Click += OkButton_Click;
      this.FormClosing += new FormClosingEventHandler(VvMessageBoxFormClosing);
   }

   private void OkButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   void VvMessageBoxFormClosing(object sender, FormClosingEventArgs e)
   {
      if(ZXC.TheVvForm.TheVvUC is IVvRecordAssignableUC)
      {
         ZXC.TheVvForm.ReRead_OnClick(null, EventArgs.Empty); // refresh record 
      }
   }
}
