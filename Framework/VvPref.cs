using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Windows.Forms;

public class VvPref
{
   #region Fieldz

   private static XmlSerializer xmlSer       = new XmlSerializer(typeof(VvPref));
   public  static string        fileName     = @"VvPreferences"     + ".xml";
   public  static string        fileName_Bkp = @"VvPreferences_Bkp" + ".xml";
   public  static string        fileName_Prev= @"VvPreferences_Prev"+ ".xml";
   
   #endregion Fieldz

   #region Constructor

   public VvPref()
   {
      //this.login.serverHost  = "";
      //this.login.userName    = "";
      //this.login.password    = "";
      //this.login.ProjectYear = "";

      //this.login.TheINITIAL_VvSubModulEnum = ZXC.VvSubModulEnum.UNDEF;
   }

   #endregion Constructor

   #region Methods

   public static string FilePath
   {
      get
      {
         try
         {
            return ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\" + fileName;
         }
         catch(Exception ex)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "get_FilePath problem:\n\n" + ZXC.VvExceptionDetails(ex));
            return "";
         }
      }
   }

   public static string FilePath_Bkp
   {
      get
      {
         return ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\" + fileName_Bkp;
      }
   }

   public static string FilePath_Prev
   {
      get
      {
         string theFilename = ZXC.ThisIsHektorProject ? fileName_Prev.Replace("Prev", DateTime.Now.ToString(ZXC.VvTimeStampFormat4FileName)) : fileName_Prev;
         return ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\" + theFilename;
      }
   }

   private static bool DownLoaded_VvPreferences_SvDUH_ZAHonly = false;

   public static VvPref Load_VvPreferences_FromXmlFile()
   {
      VvPref thisFromXml = null;

      try
      {
         using(StreamReader sr = new StreamReader(FilePath))
         {
            thisFromXml = ((VvPref)xmlSer.Deserialize(sr));
            sr.Close();
         }
      }
      catch(FileNotFoundException)
      {
         // Za SVD zahtjevnice users, za ostale normalne SVD usere morati ces rucno kopirati XML-ove kod instalacije novih klijenta 
         if(/*ZXC.IsSvDUH_ZAHonly &&*/ ZXC.IsVektor_SVD_Deployment && DownLoaded_VvPreferences_SvDUH_ZAHonly == false)
         {
            ZXC.aim_emsg("FilePath: [" + FilePath + "]");
            return DownLoad_VvPreferences_SvDUH_ZAHonly();
         }

         if(!ZXC.TheVvForm.TheVvApplicationDeployment_IsFirstRun)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Inicijalizacija datoteke: [" + FilePath + "]!");
         }
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Load_VvPreferences_FromXmlFile PROBLEM:\n\n" + ZXC.VvExceptionDetails(ex));
      }

      // upali ak bu trebalo i kad svvatis uvidom kod njih kako izgleda login forma ak je nedobra 
      if(/*ZXC.IsSvDUH_ZAHonly &&*/ ZXC.IsVektor_SVD_Deployment && DownLoaded_VvPreferences_SvDUH_ZAHonly == false)
      {
       //if(thisFromXml.login.serverHost.IsEmpty())
         if(thisFromXml.login.ServerHostDecrypted.StartsWith(/*"APOTEKA1"*/ZXC.SVD_serverName) == false)
         {
            if(Environment.MachineName == "RIPLEY7" ||                     // nemoj 'kod nas' 
               Environment.MachineName == "RIPLEY22" ||                   // nemoj 'kod nas' 
               Environment.MachineName == "VVKRISTAL" ||                   // nemoj 'kod nas' 
               Environment.MachineName == "VVKRISTAL-NEW" ||               // nemoj 'kod nas' 
               Environment.MachineName == "QWHICHKEY") return thisFromXml; // nemoj 'kod nas' 
            {
               ZXC.aim_emsg("ServerHostDecrypted: [" + thisFromXml.login.ServerHostDecrypted + "]");
               return DownLoad_VvPreferences_SvDUH_ZAHonly();
            }
         }
      }

      return thisFromXml;
   }

   private static VvPref DownLoad_VvPreferences_SvDUH_ZAHonly()
   {
      System.Net.WebClient client = new System.Net.WebClient();

      string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\" + fileName;

      try
      {
         client.DownloadFile("http://www.viper.hr/vektorSVD/VvPreferences.xml", fName);
         ZXC.aim_emsg(MessageBoxIcon.Information, "VvPreferences.xml: Download completed.");
      }
      catch(Exception ex)
      {
         //ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "DownLoad_VvPreferences_SvDUH_ZAHonly PROBLEM:\n\n" + ZXC.VvExceptionDetails(ex));
      }

      DownLoaded_VvPreferences_SvDUH_ZAHonly = true;

      // go back and do it again: 
      return Load_VvPreferences_FromXmlFile();
   }

   public static VvPref Load_VvPreferences_FromXmlFile_Bkp()
   {
      VvPref thisFromXml = null;

      try
      {
         using(StreamReader sr = new StreamReader(FilePath_Bkp))
         {
            thisFromXml = ((VvPref)xmlSer.Deserialize(sr));
            sr.Close();
         }
      }
      //catch(FileNotFoundException)
      //{
      //   if(!ZXC.TheVvForm.TheVvApplicationDeployment_IsFirstRun)
      //   {
      //      ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Inicijalizacija datoteke: [" + FilePath + "]!");
      //   }
      //}
      catch(Exception ex)
      {
         System.Windows.Forms.MessageBox.Show("Load_VvPreferences_FromXmlFile PROBLEM:\n\n" + ex.Message);
      }

      return thisFromXml;
   }

   public bool Save_VvPreferences_ToXmlFile()
   {
      // 04.01.2017: start
      if(File.Exists(FilePath_Prev))
      {
         File.Delete(FilePath_Prev);
      }

      if(File.Exists(FilePath))
      {
         File.Move(FilePath, FilePath_Prev);
      }
      // 04.01.2017: end


      return Save_VvPreferences_ToXmlFile_JOB(FilePath);
   }

   public bool Save_VvPreferences_ToXmlFile_Bkp()
   {
      return Save_VvPreferences_ToXmlFile_JOB(FilePath_Bkp);
   }

   /*public*/
   private bool Save_VvPreferences_ToXmlFile_JOB(string _filePath)
   {
      bool success = false;

      // 16.05.2024: CurrentTetragramUser_CityDependent
      if(ZXC.IsTETRAGRAMdomena)
      {
         VvLookUpItem skladLUI = ZXC.GetTetragram_PreferredSkladCD_LookUpItem();

         if(skladLUI != null)
         {
            this.findArtikl.LastUsedSkladCD = skladLUI.Cd;
         }
      }

      try
      {
         using(StreamWriter sw = new StreamWriter(_filePath))
         {
            xmlSer.Serialize(sw, this);
            sw.Close();
            success = true;
         }
      }
      catch(Exception ex)
      {
         System.Windows.Forms.MessageBox.Show("Save_VvPreferences_ToXmlFile PROBLEM:\n\n" + ex.Message);
      }

      return success;
   }

   #endregion Methods

   #region Structures

   public struct SVDZAHPrefs
   {
      public List<string> userNameHistory;
      /// <summary>
      /// Decryptiran, decoded, desifrirani userNameHistory
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public List<string> UserNameHistoryDecrypted
      {
         get
         {
          //return                            userNameHistory.Count.NotZero() ? GetUserNameHistoryDecrypted(userNameHistory) : /*null*/ new List<string>();
            return userNameHistory != null && userNameHistory.Count.NotZero() ? GetUserNameHistoryDecrypted(userNameHistory) : /*null*/ new List<string>();
         }
      }

      private List<string> GetUserNameHistoryDecrypted(List<string> userNameHistory)
      {
         List<string> decryptedArray = new List<string>(userNameHistory.Count);

         foreach(string host in userNameHistory)
         {
            decryptedArray.Add(VvAES.DecryptData(host, ZXC.vv_Login_AES_key));
         }

         return decryptedArray;
      }
   }

   public struct LoginPrefs
   {
      public string serverHost;
      public string vvDomena;
      public string userName;
      public string password;
      public string projectYear;

      public List<string> serverHostHistory;
      public List<string> vvDomenaHistory;
      public List<string> userNameHistory;

      public bool rememberServerHost;
      public bool rememberVvDomena;
      public bool rememberUserName;
      public bool rememberPassword;
      public bool rememberYear;
      public bool rememberLastUsed_SubModul;
      public bool rememberLastUsed_Projekt;

      public uint   rootPrjktKCD;
      public uint   initialPrjktKCD;
      public string initialPrjktTicker;

      private ZXC.VvSubModulEnum    INITIAL_VvSubModulEnum;
      [System.Xml.Serialization.XmlIgnore] // TheINITIAL_VvSubModulEnum za koristenje u programu, NE sprema se u fajl 
      public ZXC.VvSubModulEnum TheINITIAL_VvSubModulEnum
      {
         get {               return INITIAL_VvSubModulEnum; }
         set {                      INITIAL_VvSubModulEnum = value; }
      }
      public  int                TheINITIAL_VvSubModulEnum_AsInt // za spremiti ga u xml file, jerbo izgleda samo primitve types idu biti serializirani? (njet perfektli siguran) 
      {
         get {          return (int)INITIAL_VvSubModulEnum; }
         set {                      INITIAL_VvSubModulEnum = (ZXC.VvSubModulEnum)value; }
      }

      /// <summary>
      /// Cryptiran, encoded, sifrirani password
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string PasswdEncodedAsInFile
      {
         get { return this.password; }
         set {        this.password = value; }
      }

      /// <summary>
      /// Decryptiran, decoded, desifrirani password
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string PasswdDecrypted
      {
         get
         {
            return (password.NotEmpty()) ? VvAES.DecryptData(password, ZXC.vv_Login_AES_key) : "";
         }
      }

      /// <summary>
      /// Cryptiran, encoded, sifrirani userName
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string UserNameEncodedAsInFile
      {
         get { return this.userName; }
         set {        this.userName = value; }
      }

      /// <summary>
      /// Decryptiran, decoded, desifrirani userName
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string UserNameDecrypted
      {
         get
         {
            return userName.NotEmpty() ? VvAES.DecryptData(userName, ZXC.vv_Login_AES_key) : "";
         }
      }

      /// <summary>
      /// Decryptiran, decoded, desifrirani userNameHistory
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public List<string> UserNameHistoryDecrypted
      {
         get
         {
            return userNameHistory.Count.NotZero() ? GetUserNameHistoryDecrypted(userNameHistory) : /*null*/ new List<string>();
         }
      }

      private List<string> GetUserNameHistoryDecrypted(List<string> userNameHistory)
      {
         List<string> decryptedArray = new List<string>(userNameHistory.Count);

         foreach(string host in userNameHistory)
         {
            decryptedArray.Add(VvAES.DecryptData(host, ZXC.vv_Login_AES_key));
         }

         return decryptedArray;
      }

      /// <summary>
      /// Cryptiran, encoded, sifrirani serverHost
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string ServerHostEncodedAsInFile
      {
         get { return this.serverHost; }
         set {        this.serverHost = value; }
      }

      /// <summary>
      /// Decryptiran, decoded, desifrirani serverHost
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string ServerHostDecrypted
      {
         get
         {
            return serverHost.NotEmpty() ? VvAES.DecryptData(serverHost, ZXC.vv_Login_AES_key) : "";
         }
      }

      /// <summary>
      /// Decryptiran, decoded, desifrirani serverHostHistory
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public List<string> ServerHostHistoryDecrypted
      {
         get
         {
            return serverHostHistory.Count.NotZero() ? GetHostHistoryDecrypted(serverHostHistory) : /*null*/ new List<string>();
         }
      }

      private List<string> GetHostHistoryDecrypted(List<string> serverHostHistory)
      {
         List<string> decryptedArray = new List<string>(serverHostHistory.Count);

         foreach(string host in serverHostHistory)
         {
            decryptedArray.Add(VvAES.DecryptData(host, ZXC.vv_Login_AES_key));
         }

         return decryptedArray;
      }

      /// <summary>
      /// Cryptiran, encoded, sifrirani vvDomena
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string VvDomenaEncodedAsInFile
      {
         get { return this.vvDomena; }
         set {        this.vvDomena = value; }
      }

      /// <summary>
      /// Decryptiran, decoded, desifrirani vvDomena
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string VvDomenaDecrypted
      {
         get
         {
            return vvDomena.NotEmpty() ? VvAES.DecryptData(vvDomena, ZXC.vv_Login_AES_key) : "";
         }
      }

      /// <summary>
      /// Decryptiran, decoded, desifrirani vvDomenaHistory
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public List<string> VvDomenaHistoryDecrypted
      {
         get
         {
            return vvDomenaHistory.Count.NotZero() ? GetVvDomenaHistoryDecrypted(vvDomenaHistory) : /*null*/ new List<string>();
         }
      }

      private List<string> GetVvDomenaHistoryDecrypted(List<string> vvDomenaHistory)
      {
         List<string> decryptedArray = new List<string>(vvDomenaHistory.Count);

         foreach(string host in vvDomenaHistory)
         {
            decryptedArray.Add(VvAES.DecryptData(host, ZXC.vv_Login_AES_key));
         }

         return decryptedArray;
      }

      /// <summary>
      /// Cryptiran, encoded, sifrirani ProjectYear
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string ProjectYearEncodedAsInFile
      {
         get { return this.projectYear; }
         set {        this.projectYear = value; }
      }

      /// <summary>
      /// Decryptiran, decoded, desifrirani ProjectYear
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string ProjectYearDecrypted
      {
         get
         {
            return projectYear.NotEmpty() ? VvAES.DecryptData(projectYear, ZXC.vv_Login_AES_key) : "";
         }
      }
   }

   public struct ZapIzvodPrefs
   {
      private string directoryName;

      public string DirectoryName
      {
         get { return directoryName.NotEmpty() ? directoryName : @".\"; }
         set { directoryName = value; }
      }
      public string KontoZiro     { get; set; }
      public string NepozDug      { get; set; }
      public string NepozPot      { get; set; }
      public bool   IsPrmPrvoKnj  { get; set; }
      public bool   IsPrmUJedRed  { get; set; }
      public bool   IsAutoSave    { get; set; }
      public bool   IsQuickLoad   { get; set; }

   }

   public struct EksternLinksPrefs
   {
      private string directoryName;

      public string DirectoryName
      {
         get { return directoryName.NotEmpty() ? directoryName : @".\"; }
         set {        directoryName = value; }
      }

    //public string SomeStringProperty    { get; set; }
    //public bool   SomeBooleanProperty   { get; set; }

   }

   public struct LoadPopratPrefs
   {
      private string directoryName;
      public string DirectoryName
      {
         get { return directoryName.NotEmpty() ? directoryName : @".\"; }
         set {        directoryName = value; }
      }

    //public string SomeStringProperty    { get; set; }
    //public bool   SomeBooleanProperty   { get; set; }

   }

   public struct eRacun_Incoming_Prefs
   {
      private string directoryName;
      public string DirectoryName
      {
         get { return directoryName.NotEmpty() ? directoryName : /*@".\"*/VvForm.GetLocalDirectoryForVvFile("_ eRacun ULAZNI"); }
         set { directoryName = value; }
      }

      //public string SomeStringProperty    { get; set; }
      //public bool   SomeBooleanProperty   { get; set; }

   }

   public struct eRacun_Outgoing_Prefs
   {
      private string directoryName;
      public string DirectoryName
      {
         get { return directoryName.NotEmpty() ? directoryName : /*@".\"*/VvForm.GetLocalDirectoryForVvFile("_ eRacun IZLAZNI"); }
         set { directoryName = value; }
      }

      //public string SomeStringProperty    { get; set; }
      //public bool   SomeBooleanProperty   { get; set; }

   }

   public struct CalcOtpadPrefs
   {
      public decimal OtpadCij     { get; set; }
      public decimal PiljvCij     { get; set; }
      public decimal UdioPuO      { get; set; }
   }

   public struct ReportsPrefs
   {
      public int ZoomFactor { get; set; }

      public string RiskTT     { get; set; }
      public bool   IsPrihodTT { get; set; }
      public bool   IsRashodTT { get; set; }
      public string MacroName  { get; set; }
   }

   public struct LoadRemonsterExcelPrefs
   {
      private string directoryName;
      public  string DirectoryName
      {
         get { return directoryName.NotEmpty() ? directoryName : @".\"; }
         set {        directoryName = value; }
      }

      public string SheetName { get; set; }

      private string uplateDirectoryName;
      public  string UplateDirectoryName
      {
         get { return uplateDirectoryName.NotEmpty() ? uplateDirectoryName : @".\"; }
         set {        uplateDirectoryName = value; }
      }

      public string UplateSheetName { get; set; }

      private string bankeDirectoryName;
      public  string BankeDirectoryName
      {
         get { return bankeDirectoryName.NotEmpty() ? bankeDirectoryName : @".\"; }
         set {        bankeDirectoryName = value; }
      }

      public string BankeSheetName { get; set; }

   }

   public struct LoadPNRExcelPrefs
   {
      private string directoryName;
      public  string DirectoryName
      {
         get { return directoryName.NotEmpty() ? directoryName : @".\"; }
         set {        directoryName = value; }
      }

      public string SheetName { get; set; }

   }

   public struct FindKplanPrefs
   {
      public bool IsBiloGdjeUnazivu   { get; set; }
   }

   public struct FindKupdobPrefs
   {
      public bool IsBiloGdjeUnazivu   { get; set; }
   }

   public struct FindPrijemPrefs
   {
      public bool IsBiloGdjePrezimenu { get; set; }
   }

   public struct FindOperAnaPrefs
   {
      public bool IsBiloGdjePrezimenu { get; set; }
   }

   public struct VVColChooserStates
   {
      public string ColumnName    { get; set; }
      public bool   VisibleInRed  { get; set; }
      public bool   VisibleInBlue { get; set; }

      public VVColChooserStates(string colName, bool inRed, bool inBlue) : this() // bez ovoga : this() skace Compiler Error CS0843 
      {
         this.ColumnName    = colName;
         this.VisibleInRed  = inRed;
         this.VisibleInBlue = inBlue;
      }
   }

   public struct PlacaDucPrefs
   {
      public List<VVColChooserStates> ColChooserStates;
   }
   public struct PlacaNPDucPrefs
   {
      public List<VVColChooserStates> ColChooserStates;
   }
   public struct Placa2014DucPrefs
   {
      public List<VVColChooserStates> ColChooserStates;
   }
   public struct PlacaOd2024DucPrefs
   {
      public List<VVColChooserStates> ColChooserStates;
   }

   public struct LoginLinuxPrefs
   {
      public string serverHost;
      public string userName;
      public string password;
      public string RemoteDirectory { get; set; }

      //public uint initialPrjktKCD;

      /// <summary>
      /// Cryptiran, encoded, sifrirani password
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string PasswdEncodedAsInFile
      {
         get { return this.password; }
         set { this.password = value; }
      }

      /// <summary>
      /// Decryptiran, decoded, desifrirani password
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string PasswdDecrypted
      {
         get
         {
            return (password.NotEmpty()) ? VvAES.DecryptData(password, ZXC.vv_Login_AES_key) : "";
         }
      }

      /// <summary>
      /// Cryptiran, encoded, sifrirani userName
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string UserNameEncodedAsInFile
      {
         get { return this.userName; }
         set { this.userName = value; }
      }

      /// <summary>
      /// Decryptiran, decoded, desifrirani userName
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string UserNameDecrypted
      {
         get
         {
            return userName.NotEmpty() ? VvAES.DecryptData(userName, ZXC.vv_Login_AES_key) : "";
         }
      }

      /// <summary>
      /// Cryptiran, encoded, sifrirani serverHost
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string ServerHostEncodedAsInFile
      {
         get { return this.serverHost; }
         set { this.serverHost = value; }
      }

      /// <summary>
      /// Decryptiran, decoded, desifrirani serverHost
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string ServerHostDecrypted
      {
         get
         {
            return serverHost.NotEmpty() ? VvAES.DecryptData(serverHost, ZXC.vv_Login_AES_key) : "";
         }
      }

   }

   public struct FindArtiklPrefs
   {
      public bool   IsBiloGdjeUnazivu { get; set; }
      public bool   IsFindBy_naziv    { get; set; }
      public bool   IsFindBy_sifra    { get; set; }
      public bool   IsFindBy_barCode  { get; set; }
      public bool   IsWKolOnly        { get; set; }
      public bool   IsStatus          { get; set; }
      public string LastUsedSkladCD   { get; set; }

      public bool IsFindBy_naziv2 { get; set; }
      public bool IsFindBy_sifra2 { get; set; }

   }

   public struct FindFakturPrefs
   {
      public bool   IsBiloGdjeUnazivu { get; set; }
      public bool   IsFindBy_ttNum    { get; set; }
      public bool   IsFindBy_dokDate  { get; set; }
      public bool   IsFindBy_kpdbName { get; set; }
   }

   public struct FindRtransPrefs
   {
      public bool   IsBiloGdjeUnazivu { get; set; }
      public bool   IsFindBy_ttNum    { get; set; }
      public bool   IsFindBy_dokDate  { get; set; }
      public bool   IsFindBy_kpdbName { get; set; }

      public bool   IsFindBy_naziv    { get; set; }
      public bool   IsFindBy_sifra    { get; set; }
      public bool   IsFindBy_serlot   { get; set; }
   }

   public struct FindFakturKupdobPrefs
   {
      public bool IsBiloGdjeUnazivu { get; set; }
   }

   public struct FindKcdXtransPrefs
   {
      public bool IsBiloGdjeUnazivu { get; set; }
   }

   public struct ArtiklUcPrefs
   {
      public List<VvMigrator> MigratorStates;
   }

   public struct FakturUFADUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturUPADUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }

   public struct FakturUFMDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturURADUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturURMDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
      //public ushort MalopCalcKind { get; set; }
   }

   public struct FakturPrimkaDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturKalkDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturKKMDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturPovMalDobDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }

   
   public struct FakturIFADUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturIRADUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturIzdatnicaDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }

   public enum POSprinterKind
   {
      CLASSIC,
      BIXOLON01,
      BIXOLON02,
      EPSON01,
      EPSON02,
      POS80
   }

   public struct FakturIRMDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
      public bool isPOSprint;
      public POSprinterKind posPrintKind;
   }
   public struct FakturIRMDUC2Prefs
   {
      public List<VvMigrator> MigratorStates;
      public bool isPOSprint;
   }

   public struct FakturOdobrKupDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturPovKupDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturOdobrDobDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturPovDobDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturObavPonDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturPonudaDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturPonMalDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturNarDobDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturNRMDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturNarDobUvDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturNarDobUslDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturNarKupDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturBlgUplDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturBlgIspDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturBlgUplMDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturBlgIspMDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturReversDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturPovReversaDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturRNPDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturRNMDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturRNSDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturRNZDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturPRJDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturKIZDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturPIKDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturIRPDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturURPDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturWYRDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturCjKupcaDUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }
   public struct FakturZAH_SVD_DUCPrefs
   {
      public List<VvMigrator> MigratorStates;
   }

   public struct VvMailData
   {
      public string passwd;

      /// <summary>
      /// Cryptiran, encoded, sifrirani userName
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string EmailFromPasswdEncodedAsInFile
      {
         get { return this.passwd; }
         set {        this.passwd = value; }
      }

      /// <summary>
      /// Decryptiran, decoded, desifrirani userName
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public string EmailFromPasswdDecrypted
      {
         get
         {
            return passwd.NotEmpty() ? VvAES.DecryptData(passwd, ZXC.vv_Login_AES_key) : "";
         }
      }

      public string MailHost             { get; set; }
      public string EmailFromAddress     { get; set; }
      public string EmailFromDisplayName { get; set; }
      public bool   PoslajiKopijuSebi    { get; set; }
      public int    Port                 { get; set; }
      public string UserName             { get; set; }
      public bool   UseSSL               { get; set; }

      public const string DeaultVvPDFdirectoryName = "_ PDF i Export Datoteke";

      private string directoryName;
      public  string DirectoryName
      {
         get { return directoryName.NotEmpty() ? directoryName : /*@".\"*/VvForm.GetLocalDirectoryForVvFile(DeaultVvPDFdirectoryName/*"_ PDF i Export Datoteke"*/); }
         set { directoryName = value; }
      }

      public bool CloseAfter_Spremi      { get; set; }
      public bool CloseAfter_SpremiKao   { get; set; }
      public bool CloseAfter_Pogledaj    { get; set; }
      public bool CloseAfter_SaljiMailom { get; set; }
    //public bool UseOutlook2010         { get; set; }
      public uint SignatureModeUint      { get; set; } // 0 - NO signature, 1 - Normal (unvisible) Signature, 2 - Visible Signature 
      public uint MailClientModeUint     { get; set; } // 0 - NO_Outlook_VektorDialog, 1 - OutlookAccount_OutlookDialog, 2 - OutlookAccount_VektorDialog 

      /// <summary>
      /// Decryptiran, decoded, desifrirani userName
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public ZXC.PdfSignatureKind SignatureMode
      {
         get
         {
            switch(SignatureModeUint)
            {
               default:
               case 0 : return ZXC.PdfSignatureKind.NO_SIGNATURE  ;

               case 1 : return ZXC.PdfSignatureKind.SIGN_UNVISIBLE;
               case 2 : return ZXC.PdfSignatureKind.SIGN_VISIBLE  ;
            }
         }
      }

      /// <summary>
      /// Decryptiran, decoded, desifrirani userName
      /// </summary>
      [System.Xml.Serialization.XmlIgnore]
      public ZXC.MailClientKind MailClientMode
      {
         get
         {
            switch(MailClientModeUint)
            {
               default:
               case 0: return ZXC.MailClientKind.NO_Outlook_VektorDialog;

               case 1: return ZXC.MailClientKind.OutlookAccount_OutlookDialog;
               case 2: return ZXC.MailClientKind.OutlookAccount_VektorDialog;
            }
         }
      }

   }

   public struct HektorPrefs
   {
      public DateTime DateOd                              { get; set; }
      public DateTime DateDo                              { get; set; }

      public List<string> TrosakKontaList                 { get; set; }
      public int          Skip_TR_MtrosCd                 { get; set; }
                                                          
      public List<int>    Skip_RN_MtrosCdList             { get; set; }
      public int          NotUslArtCD                     { get; set; }

      // Be ADVICED!!!                                                    
      // bez ovih 'XmlIgnore' neki TH Shopovi ne mogu uci u program       
      // 'VvPref type initializer threw exception'                        
      // nakon sati dabagiranja zasto neki mogu! a neki ne mogu!          
      // nisam uspio skuziti razlog. Jedino suziti izvor problema na ove  
      // dvije Liste<VvUtilDataPackage>                                   
      // probao sam cak i struct promjeniti u class i dodati parameterless
      // constructor, ali NIJE pomoglo                                    
      // rjesenje je, dokle i ako ikada ne skuzis, da se ovi XmlIgnore-i  
      // micu kada ides deployati Haktor, a za Vektor ih ostavi upaljene  

      // ... kasnije ... 02.01.2017: otkrio si slucajno da to ima veze s verzijom .NET framevorka:     
      // .NET 4.5 radi ziher, a mozda ce i 4.0? Ak' ne, sibaj dalje sa ova 2 XmlIgnore-a nekomentirana 

#if VektorXP
      [System.Xml.Serialization.XmlIgnore]
#endif
      public List<ZXC.VvUtilDataPackage> MtrosDataList    { get; set; }

#if VektorXP
      [System.Xml.Serialization.XmlIgnore]
#endif
      public List<ZXC.VvUtilDataPackage> PerCD_MtrCD_List { get; set; }
   }

   #endregion Structures

   #region Serializible Fields And Propertiz

   // primjer kada NE zelis da ti neki Property bude serializible
   //[System.Xml.Serialization.XmlIgnore]
   //public bool HasEdits
   //{
   //   get { return _modified; }
   //   set { _modified = value; }
   //}




   // PAZI!!! ovdje trebas koristiti public field-ove. A ako koristis Property: 
   // 'code generates error CS1612'
   //Error Message :
   //Cannot modify the return value of 'expression' because it is not a variable
   //An attempt was made to modify a value type that was the result of an intermediate expression. Because the value is not persisted, the value will be unchanged.
   //To resolve this error, store the result of the expression in an intermediate value, or use a reference type for the intermediate expression.
      

   public LoginPrefs               login;
   public SVDZAHPrefs              SVDZAH;
   public ZapIzvodPrefs            zapIzvod;
   public FindKplanPrefs           findKplan;
   public FindKupdobPrefs          findKupdob;
   public FindPrijemPrefs          findPrijem;
   public FindOperAnaPrefs         findOperana;
   public PlacaDucPrefs            placaDUC;
   public Placa2014DucPrefs        placa2014DUC;
   public PlacaOd2024DucPrefs      placaOd2024DUC;
   public PlacaNPDucPrefs          placaNPDUC;
   public ArtiklUcPrefs            artiklUC;
   public FindArtiklPrefs          findArtikl;
   public FindFakturPrefs          findFaktur;
   
   public FindRtransPrefs          findRtrans;
   public FindKcdXtransPrefs       findKcdXtrans;

   public FakturUFADUCPrefs        fakturURaDUC;
   public FakturUPADUCPrefs        fakturUPaDUC;
   public FakturUFMDUCPrefs        fakturUFMDUC;
   public FakturURADUCPrefs        fakturURbDUC;
   public FakturURMDUCPrefs        fakturURmDUC;
   public FakturPrimkaDUCPrefs     fakturPrimkaDUC;
   public FakturKalkDUCPrefs       fakturKalkDUC;
   public FakturPovMalDobDUCPrefs  fakturPovDobMalDUC;
   public FakturKKMDUCPrefs        fakturKKMDUC;
   public FakturIFADUCPrefs        fakturIRaDUC;
   public FakturIRADUCPrefs        fakturIRbDUC;
   public FakturIzdatnicaDUCPrefs  fakturIzdatnicaDUC;
   public FakturIRMDUCPrefs        fakturIRMDUC;
   public FakturIRMDUC2Prefs       fakturIRMDUC2;
   public FakturOdobrKupDUCPrefs   fakturOdobrKupDUC;
   public FakturPovKupDUCPrefs     fakturPovKupDUC;
   public FakturOdobrDobDUCPrefs   fakturOdobrDobDUC;
   public FakturPovDobDUCPrefs     fakturPovDobDUC;
   public FakturObavPonDUCPrefs    fakturObavPonDUC;
   public FakturPonudaDUCPrefs     fakturPonudaDUC;
   public FakturPonMalDUCPrefs     fakturPonMalDUC;
   public FakturNarDobDUCPrefs     fakturNarDobDUC;
   public FakturNRMDUCPrefs        fakturNRMDUC;
   public FakturNarDobUvDUCPrefs   fakturNarDobUvDUC;
   public FakturNarDobUslDUCPrefs  fakturNarDobUslDUC;
   public FakturNarKupDUCPrefs     fakturNarKupDUC;
   public FakturBlgUplDUCPrefs     fakturBlgUplDUC;
   public FakturBlgIspDUCPrefs     fakturBlgIspDUC;
   public FakturBlgUplMDUCPrefs    fakturBlgUplMDUC;
   public FakturBlgIspMDUCPrefs    fakturBlgIspMDUC;
   public FakturReversDUCPrefs     fakturReversDUC;
   public FakturPovReversaDUCPrefs fakturPovRevDUC;
   public FindFakturKupdobPrefs    findFakturKupdob;
   public FakturRNPDUCPrefs        fakturRNpDUC;
   public FakturRNMDUCPrefs        fakturRNmDUC;
   public FakturRNSDUCPrefs        fakturRNsDUC;
   public FakturRNZDUCPrefs        fakturRNzDUC;
   public FakturPRJDUCPrefs        fakturPRjDUC;
   public FakturKIZDUCPrefs        fakturKIZDUC;
   public FakturPIKDUCPrefs        fakturPIKDUC;
   public FakturIRPDUCPrefs        fakturIRPDUC;
   public FakturURPDUCPrefs        fakturURPDUC;
   public FakturWYRDUCPrefs        fakturWYRDUC;
   public FakturCjKupcaDUCPrefs    fakturCjKupcaDUC;
   public FakturZAH_SVD_DUCPrefs   fakturZAH_SVD_DUC;

   public LoadRemonsterExcelPrefs loadRemonsterExcelPrefs;
   public LoadPNRExcelPrefs       loadPNRExcelPrefs;

   public LoginLinuxPrefs  loginLinux;

   public EksternLinksPrefs eksternLinks1;
   public EksternLinksPrefs eksternLinks2;

   public ReportsPrefs      reportPrefs;

   public LoadPopratPrefs   loadPopratPrefs;

   public CalcOtpadPrefs    calcOtpadPrefs;

   public VvMailData        vvMailData;

   //[System.Xml.Serialization.XmlIgnore]
   public HektorPrefs hektorPrefs;

   public eRacun_Incoming_Prefs eRacun_Ulaz_Prefs;
   public eRacun_Outgoing_Prefs eRacun_Izlaz_Prefs;

   #endregion Serializible Propertiz

}