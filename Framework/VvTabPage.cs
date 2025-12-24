using System;
using System.Drawing;
using System.Windows.Forms;
using Crownwood.DotNetMagic.Controls;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using                  MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
#endif

public class VvTabPage : Crownwood.DotNetMagic.Controls.TabPage, IDisposable
{
   #region Fieldz

   public  Panel     tamponPanel_Header, panelZaUC;
   public  Panel     tamponPanel_Footer;
   public VvHamper   hampMod, hampModul, hampNazivPrjkt, hamp_F2; 
   public  VvHamper  hampAdd,hampArhiva;
   public  VvTextBox tbx_addMetaData, tbx_modMetaData, tbx_col1, tbx_col2, tbx_col3, tbx_col4, tbx_col5, tbx_col6, tbx_col7, tbx_NazivPrjkt,
                     tbx_arVer, tbx_arTS, tbx_arUID, tbx_arAction;
   public  Label     labTamPanCrta, labArhiva, labDeviza;
   private int       col1, col2, col3, col4, col5, col6, col7;

   private Point xy;

   public Point SubModul_xy
   {
      get { return xy; }
      set { xy = value; }
   }

   private uint? initialVvDataRecord_RecID;
   private VvDataRecord initialVvDataRecord;

   /*private*/ public bool thisIsFirstAppereance = true;

   #endregion Fieldz

   #region Propertiz

   /// <summary>
   /// Ako ces ikada trebati Unique Identifier za VvTabPage
   /// On je TimStamp trenutka nastanka objekta 
   /// </summary>
   public DateTime JMBG_UUID { get; private set; }

   private VvForm theVvForm;
   public  VvForm TheVvForm { get { return this.theVvForm; } }

   private VvForm.VvSubModul vvSubModul;
   public  VvForm.VvSubModul TheVvSubModul
   {
      get { return vvSubModul; }
      set {        vvSubModul = value; }
   }

   public bool IsForReport
   {
      get { return TheVvSubModul.IsReport; }
   }

   private VvUserControl vvUserControl;
   /// <summary>
   /// Generic, current VvUserControl (moze biti VvRecordUC ili VvReportUC
   /// </summary>
   public  VvUserControl TheVvUC
   {
      get { return vvUserControl; }
      set {        vvUserControl = value; }
   }

   /// <summary>
   /// TheVvUc as IVvPrintableUC
   /// </summary>
   public IVvPrintableUC TheVvPrintableUC
   {
      get { return (IVvPrintableUC)vvUserControl; }
   }

   /// <summary>
   /// TheVvUc as VvRecordUC
   /// </summary>
   public VvRecordUC TheVvRecordUC
   {
      get { return (VvRecordUC)vvUserControl; }
   }

   /// <summary>
   /// TheVvUc as VvDocumentRecordUC
   /// </summary>
   public VvDocumentRecordUC TheVvDocumentRecordUC
   {
      get { return (VvDocumentRecordUC)vvUserControl; }
   }

   /// <summary>
   /// TheVvUc as VvPolyDocumRecordUC
   /// </summary>
   public VvPolyDocumRecordUC TheVvPolyDocumRecordUC
   {
      get { return (VvPolyDocumRecordUC)vvUserControl; }
   }

   /// <summary>
   /// TheVvUc as VvDocumLikeRecordUC
   /// </summary>
   public VvDocumLikeRecordUC TheVvDocumentLikeRecordUC
   {
      get { return (VvDocumLikeRecordUC)vvUserControl; }
   }

   /// <summary>
   /// TheVvUc as VvReportUC
   /// </summary>
   public VvReportUC TheVvReportUC
   {
      // 03.06.2016: 
    //get { return (VvReportUC)vvUserControl; }
      get 
      { 
         if(vvUserControl is VvReportUC) return (VvReportUC)vvUserControl; 
         else                            return null;
      }
   }

   /// <summary>
   /// TheVvUc as VvRecLstUC
   /// </summary>
   public VvRecLstUC TheVvRecLstUC
   {
      get { return (VvRecLstUC)vvUserControl; }
   }

   /// <summary>
   /// TheVvUc as VvSifrarRecordUC
   /// </summary>
   public VvSifrarRecordUC TheVvSifrarRecordUC
   {
      get { return (VvSifrarRecordUC)vvUserControl; }
   }

   /// <summary>
   /// TheVvRecordUC's VirtualDataRecord
   /// </summary>
   public VvDataRecord TheVvDataRecord
   {
      get { return TheVvRecordUC.VirtualDataRecord; }
   }

   //PUSE:
   //private XSqlConnection dbConnection;
   //public  XSqlConnection conn
   //{
   //   get { return dbConnection; }
   //   set {        dbConnection = value; }
   //}
   public XSqlConnection TheDbConnection
   {
      get
      {
         string dbNameForThisTabPage;

         //if(this.IsForReport == false && VvDataRecord.ThisTypeIsFrom_VvProjektiDB(TheVvDataRecord.GetType()))
         if(ThisVvModulIsFrom_VvProjektiDB(this.vvSubModul.modulEnum))
         {
            dbNameForThisTabPage = TheVvForm.GetvvDB_prjktDB_name();
         }
         else
         {
            dbNameForThisTabPage = TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName;
         }

         ZXC.SetMainDbConnDatabaseName(dbNameForThisTabPage);

         return ZXC.TheMainDbConnection;
      }
   }

   private bool ThisVvModulIsFrom_VvProjektiDB(ZXC.VvModulEnum vvModulEnum)
   {
      if(vvModulEnum == ZXC.VvModulEnum.MODUL_PRJKT) return true;
      else                                           return false;
   }

   private ZXC.VvDataBaseInfo vvDatabaseInfo;
   public  ZXC.VvDataBaseInfo TheVvDatabaseInfoOn_SelectedVvTabPage
   {
      get { return vvDatabaseInfo; }
      set {        vvDatabaseInfo = value; }
   }

   private ZXC.WriteMode writeMode;
   public  ZXC.WriteMode WriteMode
   {
      get { return writeMode; }
      set {        writeMode = value; }
   }

   private ZXC.ReportMode reportMode;
   public  ZXC.ReportMode ReportMode
   {
      get { return reportMode; }
      set {        reportMode = value; }
   }

   public Panel TamponPanel_Header
   {
      get { return tamponPanel_Header; }
   }

   public Panel TamponPanel_Footer
   {
      get { return tamponPanel_Footer; }
   }

   private BackgroundWorker backgroundWorker;
   public  BackgroundWorker TheBackgroundWorker
   {
      get { return backgroundWorker; }
      set {        backgroundWorker = value; }
   }

   private bool hasUnsavedChanges;
   public  bool HasUnsavedChanges
   {
      get { return hasUnsavedChanges; }
      set { hasUnsavedChanges = value; }
   }

   public  ZXC.VvTabPageKindEnum TabPageKind { get; set; }

   public bool IsFiterVisible { get; set; }

   public bool FileIsEmpty { get; set; }

   public bool IsArhivaTabPage { get;  set; }

   public bool ArhivaTableIsNotEmpty { get; set; }

   private List<TsbEnabled> RecordTSB_EnabledStateList { get; set; }

   private int ArhivaTabPageSelectedIndex { get; set; }

   #endregion Propertiz

   #region Fld_
 
   public string Fld_AddMetaData
   {
      set { tbx_addMetaData.Text = value; }
   }

   public string Fld_ModMetaData
   {
      set { tbx_modMetaData.Text = value; }
   }

   public string Fld_Col1 { set { tbx_col1.Text = value; } }
   public string Fld_Col2 { set { tbx_col2.Text = value; } }
   public string Fld_Col3 { set { tbx_col3.Text = value; } }
   public string Fld_Col4 { set { tbx_col4.Text = value; } }
   public string Fld_Col5 { set { tbx_col5.Text = value; } }
   public string Fld_Col6 { set { tbx_col6.Text = value; } }
   public string Fld_Col7 { set { tbx_col7.Text = value; } }

   public string Fld_PrjktNaziv
   {
      set { tbx_NazivPrjkt.Text = value; }
   }

   public string Fld_Arhiva
   {
      set { tbx_arVer.Text = value; }
      get { return tbx_arVer.Text;  }
   }

   #endregion Fld_

   #region struct TsbEnabled

   private struct TsbEnabled
   {
      public string name;
      public bool   enabled;
   }
   
   #endregion struct TsbEnabled

   #region Constructor()

   public VvTabPage(// CONSTRUCTOR 
      VvForm _vvForm, string _title, //VvForm.VvSubModul     _vvSubModul, 
      Point _xy, ZXC.VvTabPageKindEnum _tabPageKind, Crownwood.DotNetMagic.Controls.TabControl parentTabControl, VvDataRecord _initialVvDataRecord, uint? _initialVvDataRecord_RecID, VvRecLstUC _initialRecLstUC)
      : base(_title)
   {
      this.theVvForm     = _vvForm;
      //this.TheVvSubModul = _vvSubModul;
      this.TheVvSubModul = TheVvForm.aModuli[_xy.X].aSubModuls[_xy.Y];
      this.xy            = _xy;
      this.Tag           = _xy; // 01.06.2008: ovo vjerovatno ne sluzi nicemu! istrazi kasnije!!! 
      this.TabPageKind   = _tabPageKind;
      this.IsFiterVisible = true;


      TheVvDatabaseInfoOn_SelectedVvTabPage = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects;

      JMBG_UUID = DateTime.Now;

      RecordTSB_EnabledStateList = new List<TsbEnabled>();

      IsArhivaTabPage = false;

      ArhivaTableIsNotEmpty = false;

      this.initialVvDataRecord_RecID = _initialVvDataRecord_RecID;
      this.initialVvDataRecord       = _initialVvDataRecord      ;

      InitializePanelZaUC();

      CalcTextBoxWidth();

      if(parentTabControl != null) parentTabControl.TabPages.Add(this);

      this.Selected = true;

      if(this.TabPageKind != ZXC.VvTabPageKindEnum.RECORD_TabPage_INTERACTIVE)
         this.VisibleChanged += new EventHandler(VvTabPage_VisibleChanged);

      bool localOK = true;

      switch(this.TabPageKind)
      {
         case  ZXC.VvTabPageKindEnum.RECORD_TabPage: 
         case  ZXC.VvTabPageKindEnum.RECORD_TabPage_INTERACTIVE:

            InitializeTamponPanel_Header(parentTabControl);
            InitializeTamponPanel_Footer(parentTabControl);
            panelZaUC.Location          = new Point(0, tamponPanel_Header.Bottom);
            panelZaUC.Size              = new Size(this.Width, this.Height - tamponPanel_Header.Height - tamponPanel_Footer.Height);
            tamponPanel_Footer.Location = new Point(0, panelZaUC.Bottom);

            ZXC.TheVvForm.Refresh(); Cursor.Current = Cursors.WaitCursor;

            localOK = Create_vvDataRecord_and_vvRecordUC();

            Cursor.Current = Cursors.Default;
            if(!localOK)
            {
               //if(conn != null) conn.Close();
               this.VisibleChanged -= new EventHandler(VvTabPage_VisibleChanged);
               parentTabControl.TabPages.Remove(this);
            }
            //else
            //{
            //   ArhivaTableIsNotEmpty = VvDaoBase.CountAllRecords(conn, TheVvDataRecord.VirtualRecordNameArhiva) > 0;
            //}

            break;


         case ZXC.VvTabPageKindEnum.RecLIST_TabPage:

            InitializeTamponPanel_Header(parentTabControl);
            panelZaUC.Location = new Point(0, tamponPanel_Header.Bottom);
            panelZaUC.Size     = new Size(this.Width, this.Height - tamponPanel_Header.Height);

            localOK = Create_vvRecListUC(_initialRecLstUC);

            if(!localOK)
            {
               //if(conn != null) conn.Close();
               this.VisibleChanged -= new EventHandler(VvTabPage_VisibleChanged);
               parentTabControl.TabPages.Remove(this);
            }
            break;


         case ZXC.VvTabPageKindEnum.REPORT_TabPage:
            InitializeTamponPanel_Header(parentTabControl);
            panelZaUC.Location = new Point(0, tamponPanel_Header.Bottom);
            panelZaUC.Size     = new Size(this.Width, this.Height - tamponPanel_Header.Height);
           
            Create_vvRptFilter_and_vvReportUC();
            CreateBackgroundWorker();

            break;

         case ZXC.VvTabPageKindEnum.OTHER_TabPage:
            InitializeTamponPanel_Header(parentTabControl);
            panelZaUC.Location = new Point(0, tamponPanel_Header.Bottom);
            panelZaUC.Size     = new Size(this.Width, this.Height - tamponPanel_Header.Height);
            Create_OtherUC();

            break;
      
      }
      
      WriteMode = ZXC.WriteMode.None;

      if(localOK && TabPageKind != ZXC.VvTabPageKindEnum.RECORD_TabPage_INTERACTIVE) 
         VvTabPage_VisibleChanged(null, null); // da dignemo VisibleChanged event, za SubModul ToolStrip ovoOno... 

      if(this.Visible && this.TabPageKind != ZXC.VvTabPageKindEnum.OTHER_TabPage) 
         VvHamper.ApplyVVColorAndStyleTabCntrolChange(this);

      ZXC.TheVvForm.VvPref.login.TheINITIAL_VvSubModulEnum_AsInt = (int)TheVvSubModul.subModulEnum;

      // 24.03.2015: 
      bool lan2skyConn = ZXC.IsTEXTHOsky && ZXC.vvDB_IsLocalhost == false;

      // 11.1.2011: 
      if((this.TheVvUC is ArtiklUC      ||
          this.TheVvUC is ArtiklListUC  ||
          this.TheVvUC is FakturDUC     ||
          this.TheVvUC is FakturListUC  ||
          this.TheVvUC is RiskReportUC   ) &&
          ZXC.ThisIsSkyLabProject == false &&
          lan2skyConn == false)
      {
         TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType./*Name*/Code);

         ZXC.TheVvForm.Refresh(); Cursor.Current = Cursors.WaitCursor;

         // 08.11.2016: fast program entrance
         if((ZXC.IsRipleyOrKristal && ZXC.vvDB_VvDomena == "vvTH") || (ZXC.IsTEXTHOcentrala))
         {
            goto be_fast;
         }

         /*********************************************************************************************************************************************************************/
         /****/   /* 02.07.2017: */
         /****/   /* 11.07.2017: ipak ne jer Rijeka radi do 21.00 */
         /****/   /* 13.09.2017: ipak da jer Rijekin VvJanitor cemo pomaknuti na 21.20 */
         /****/   //bool isTHshopDuringJanitorTime = false; 
         /****/
         /****/ //if(ZXC.RISK_Cache_Checked == false ||  this.TheVvUC is RiskReportUC                                   ) { ArtiklDao.CheckCache(TheDbConnection); ZXC.RISK_Cache_Checked = true; } /****/
         /****/   if(ZXC.RISK_Cache_Checked == false || (this.TheVvUC is RiskReportUC && !ZXC.IsTHshopDuringJanitorTime)) { ArtiklDao.CheckCache(TheDbConnection); ZXC.RISK_Cache_Checked = true; } /****/
         /****/
         /*********************************************************************************************************************************************************************/

         /****... automatic, this goes in SkyLab only ... or 'ruèno' as AubModulAction   RtransDao.CheckAndRepare_ZPC_Kol_And_OldMpc(TheDbConnection);                     ****/
         /****/   // 30.11.2015: 
         /****/ //if( ZXC.RISK_PrNabCij_Checked == false || this.TheVvUC is RiskReportUC)                                                     { RtransDao.CheckPrNabDokCij(TheDbConnection); ZXC.RISK_PrNabCij_Checked = true; } /****/
         /****/   if((ZXC.RISK_PrNabCij_Checked == false || this.TheVvUC is RiskReportUC) && (!ZXC.IsTEXTHOcentrala || ZXC.vvDB_IsLocalhost)) { RtransDao.CheckPrNabDokCij(TheDbConnection); ZXC.RISK_PrNabCij_Checked = true; } /****/
         /****/
         /*********************************************************************************************************************************************************************/

         // 28.10.2024: 
         /****/   if
                  (
                     (ZXC.RISK_BadMSU_Checked == false || TheVvUC is RiskReportUC) && 
                     (ZXC.IsTEXTHOcentrala    == false || ZXC.vvDB_IsLocalhost   ) && 
                     (ZXC.IsTEXTHOshop        == false                           )
                  ) 
         { 
            ArtiklDao.Check_MSU_Cache(TheDbConnection, false); ZXC.RISK_BadMSU_Checked = true; 
         } /****/

         // 02.06.2015: 
       //if(ZXC.RISK_NOTfisk_Checked == false && ZXC.CURR_prjkt_rec.IsFiskalOnline &&  this.TheVvUC is IRMDUC)
         // 14.06.2015: 
         // 30.11.2015: 
       //if(ZXC.RISK_NOTfisk_Checked == false && ZXC.CURR_prjkt_rec.IsFiskalOnline &&  this.TheVvUC is IRMDUC && ZXC.IsTEXTHOany )
         // 01.02.2024: 
       //if(ZXC.RISK_NOTfisk_Checked == false && ZXC.CURR_prjkt_rec.IsFiskalOnline &&  this.TheVvUC is IRMDUC && ZXC.IsTEXTHOshop)

         bool shouldCheck_forTH         = this.TheVvUC is IRMDUC      && ZXC.IsTEXTHOshop                          ; // TEXTHO
       //bool shouldCheck_forNoAutoFisk = this.TheVvUC is IRADUC      && ZXC.CURR_prjkt_rec.IsNoAutoFiskal == true ; // TETRAGRAM only! ... za sada 
         bool shouldCheck_forNoAutoFisk = this.TheVvUC is IRA_MPC_DUC && ZXC.CURR_prjkt_rec.IsNoAutoFiskal == true ; // TETRAGRAM only! ... za sada 

         #region Check for TH

         if(ZXC.RISK_NOTfisk_Checked == false && ZXC.CURR_prjkt_rec.IsFiskalOnline && shouldCheck_forTH)
         {
            uint NOTfisk_IRM_Count = FakturDao.CountNOTfiskalized_IRMs(TheDbConnection);

            if(NOTfisk_IRM_Count.NotZero())
            {
               // 30.01.2017: !!! BIG NEWS !!! 'ExecuteSynchronisation_SEND_then_RECEIVE' - BEFORE 'RISK_Fiskalize_AllPreviously_NOTfiskalized_JOB' 
               TheVvForm.VvForm_FormLoad_ExecuteSynchronisation_SEND_then_RECEIVE(this, EventArgs.Empty);

               ZXC.aim_emsg(MessageBoxIcon.Warning, "Otkriveni su NEFISKALIZIRANI 'IRM' raèuni!\n\nFiskalizirati æu ih automatski.");

               // 25.01.2018: u danu kada CIS uopce ne radi, ipak im treba dati mogucnost da odustanu od ReFiskalizacije 
               // jer odervajs ulazak u program moze trajati satima ...                                                  
               // pa smo u RISK_Fiskalize_AllPreviously_NOTfiskalized_JOB iako je isAutoCheck == true                    
               // omogucili da user odustane od ove akcije                                                               
               ZXC.TheVvForm.RISK_Fiskalize_AllPreviously_NOTfiskalized_JOB(this, EventArgs.Empty, true );
            }

            ZXC.RISK_NOTfisk_Checked = true;
         }

         #endregion Check for TH

         #region Check for NoAutoFisk

         if(ZXC.RISK_NOTfisk_Checked == false && ZXC.CURR_prjkt_rec.IsFiskalOnline && shouldCheck_forNoAutoFisk)
         {
            List<Faktur> theFakturList = FakturDao.GetAllPreviously_NOTfiskalizedFakturs(TheDbConnection);

            if(theFakturList.Count.NotZero())
            {
               ZXC.aim_emsg_List("NE FISKALIZIRANI raèuni!", theFakturList.Select(fak => fak./*TT_And_TtNum*/ToString()).ToList());
            }

            ZXC.RISK_NOTfisk_Checked = true;
         }

         #endregion Check for NoAutoFisk

be_fast:

         Cursor.Current = Cursors.Default;
      }

      // 19.1.2011: 
      // 23.10.2014: 
      if(TheVvUC != null) TheVvUC.SetWarningColorsAndLabel();

      // 28.10.2013: CheckForUnlinkedTranses 

      if(this.TheVvUC is VvDocumentRecordUC && ZXC.DUC_UnlinkedTranses_Checked == false)
      {
         VvDocumentRecordUC theVvDocumentRecordUC = this.TheVvUC as VvDocumentRecordUC         ;
         VvDocumentRecord   theVvDocumLikeRecord  = theVvDocumentRecordUC.VirtualDocumentRecord;
         string             documentName          = theVvDocumLikeRecord.VirtualRecordName     ;
         string             transesName           = theVvDocumLikeRecord.TransRecordName       ;

         //List<VvTransRecord> unlinkedTransesList;
         List<int> unlinkedTransesRecIdList = VvDaoBase.CheckForUnlinkedTranses(TheDbConnection, documentName, transesName);

         if(unlinkedTransesRecIdList.Count.NotZero())
         {
            VvRptFilter rptFilter = new VvRptFilter();

            // PAZI!!! Za sve novododane iznimke moras dodati i u 'CheckForUnlinkedTranses_Command' !!! 
            List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);
            filterMembers.Add(new VvSqlFilterMember(/*documentName +*/ "L.recID", "NULL", " IS "/*, documentName*/)); // ovaj, dakle, blokira vec prebacene 
            filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt], "ttPIM", Faktur.TT_PIM, " != ")); // ova dva nikada namju zaglavlje
            filterMembers.Add(new VvSqlFilterMember(ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt], "ttPUM", Faktur.TT_PUM, " != ")); // ova dva nikada namju zaglavlje
            filterMembers.Add(new VvSqlFilterMember(ZXC.XtransSchemaRows[ZXC.XtrCI.t_tt], "ttTHL", "THL"        , " != ")); // ovaj    nikada nama  zaglavlje
            // PAZI!!! Za sve novododane iznimke moras dodati i u 'CheckForUnlinkedTranses_Command' !!! 
            
            rptFilter.FilterMembers = filterMembers;

            string errMessage = "";
            uint count = 0;

            switch(documentName)
            {
               case Faktur.recordName:
                  List<Rtrans> unlinkedRtransesList = new List<Rtrans>();
                  ZXC.RtransDao.LoadManyDocumentsTtranses(TheDbConnection, unlinkedRtransesList, rptFilter, "recID");
                  foreach(Rtrans rtrans_rec in unlinkedRtransesList) { errMessage += (++count).ToString() + ": " + rtrans_rec.ToShortString(); }
                  break;

               case Nalog.recordName:
                  List<Ftrans> unlinkedFtransesList = new List<Ftrans>();
                  ZXC.FtransDao.LoadManyDocumentsTtranses(TheDbConnection, unlinkedFtransesList, rptFilter, "recID");
                  foreach(Ftrans ftrans_rec in unlinkedFtransesList) { errMessage += (++count).ToString() + ": " + ftrans_rec.ToShortString(); }
                  break;

               case Mixer.recordName:
                  List<Xtrans> unlinkedXtransesList = new List<Xtrans>();
                  ZXC.XtransDao.LoadManyDocumentsTtranses(TheDbConnection, unlinkedXtransesList, rptFilter, "recID");
                  foreach(Xtrans xtrans_rec in unlinkedXtransesList) { errMessage += (++count).ToString() + ": " + xtrans_rec.ToString(); }
                  break;
            }

            DialogResult result = MessageBox.Show("Otkrivena je greška!\n\nOve stavke dokumenta NEMAJU ZAGLAVLJE.\nDa li ih želite obrisati?\nAko niste sigurni, odgovorite NE te kontaktirajte Viper.\n\n" + errMessage,
               "Potvrdite BRISANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

            if(result != DialogResult.Yes) return;

            Cursor.Current = Cursors.WaitCursor;
            VvDaoBase.DELETE_UnlinkedTranses(TheDbConnection, transesName, unlinkedTransesRecIdList);
            Cursor.Current = Cursors.Default;
         }

         ZXC.DUC_UnlinkedTranses_Checked = true;
      }

      #region VvXmlDocumentDR AUTO RECOVERY

      // delete old VvXmlDRfiles: 
      //if(TheVvUC is VvRecordUC && ZXC.OldVvXmlDRfilesCheckedAndDeletedList.Contains(TheVvDataRecord.VirtualRecordName2) == false && ZXC.VvXmlDR_LastDocumentMissing_AlertRaised == false)
      //{
      //   // ovo si pak promijenio da se vise ne brise nego gZip-a u tar file: 
      //   bool hasError = ZXC.Delete_AutoCreated_VvXmlDR_Files(TheVvDataRecord.VirtualRecordName2, ZXC.YesterdayYesterdayYesterday);
      //   ZXC.OldVvXmlDRfilesCheckedAndDeletedList.Add(TheVvDataRecord.VirtualRecordName2);
      //
      //}

      #endregion VvXmlDocumentDR AUTO RECOVERY

      // 24.01.2023: 
      this.Validating += VvTabPage_Validating;
      ArhivaTabPageSelectedIndex = -1;

   } // Constructor 

   private void VvTabPage_Validating(object sender, CancelEventArgs e)
   {
      if(IsArhivaTabPage)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Izaðite, najprije, iz Arhive.");
         TheVvForm.TheTabControl.SelectedIndex = ArhivaTabPageSelectedIndex;

         e.Cancel = true;
      }
   }


   #endregion Constructor()

   #region Create_vvDataRecord_and_vvRecordUC

   private bool Create_vvDataRecord_and_vvRecordUC()
   {
      bool IsNotEmptyTable = false;
      VvDataRecord vvDataRecord;
      string dbName/* = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects.DataBaseName*/;

      #region switch 4 arhivedDataRecord = new XyVvDataRecord();

      // next lajn iz project dependent VvDataRecord factory (virtuals or overriders are in VvForm_Q / VsrForm, ... 
      vvDataRecord = TheVvForm.CreateTheVvDataRecord_SwitchSubModulEnum(TheVvSubModul); 

      if(vvDataRecord == null) return (false);

      if(vvDataRecord.IsDocument     && ((VvDocumentRecord) vvDataRecord).VirtualTranses  == null) ZXC.aim_emsg("U bussiness constructoru, Zaboravio si 'Transes  = new List<Xtrans>()' u 'Create_vvDataRecord_and_vvRecordUC'");
      if(vvDataRecord.IsPolyDocument && ((VvPolyDocumRecord)vvDataRecord).VirtualTranses2 == null) ZXC.aim_emsg("U bussiness constructoru, Zaboravio si 'Transes2 = new List<Xtrans>()' u 'Create_vvDataRecord_and_vvRecordUC'");
      if(vvDataRecord.IsPolyDocument && ((VvPolyDocumRecord)vvDataRecord).VirtualTranses3 == null) ZXC.aim_emsg("U bussiness constructoru, Zaboravio si 'Transes3 = new List<Xtrans>()' u 'Create_vvDataRecord_and_vvRecordUC'");

      #endregion switch 4 arhivedDataRecord = new XyVvDataRecord();

      #region InitializeVvTabConnection(arhivedDataRecord.GetType())

      //if(!InitializeVvTabConnection(vvDataRecord.GetType())) return false;

      #endregion InitializeVvTabConnection(arhivedDataRecord.GetType())

      #region if(initialVvDataRecord_RecID != null) arhivedDataRecord.VvDao.SetMe_Record_byRecID()

      if(initialVvDataRecord_RecID != null) vvDataRecord.VvDao.SetMe_Record_byRecID(TheDbConnection, vvDataRecord, (uint)initialVvDataRecord_RecID, IsArhivaTabPage);

      if(initialVvDataRecord       != null) vvDataRecord = initialVvDataRecord;

      #endregion if(initialVvDataRecord_RecID != null) arhivedDataRecord.VvDao.SetMe_Record_byRecID()

      #region CheckTable(conn, dbName, arhivedDataRecord .VirtualRecordName

      dbName = VvSQL.GetDbNameForThisTableName(vvDataRecord.VirtualRecordName);

      if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, vvDataRecord.VirtualRecordName)) return false;
      if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, vvDataRecord.VirtualRecordNameArhiva)) return false;

      if(vvDataRecord.IsExtendable || vvDataRecord is Faktur) // jerbo dok tek radis vvDataRecord i vvUC, jos neznas koji je tt u pitanju 
      {
         if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, ((IVvExtendableDataRecord)vvDataRecord).ExtenderTableName)) return false;
         if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, ((IVvExtendableDataRecord)vvDataRecord).ExtenderTableNameArhiva)) return false;
      }

      if(vvDataRecord is Artikl || vvDataRecord is Faktur) 
      {
         if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, ArtStat.recordName)) return false;
      }

      if(vvDataRecord.IsDocument)
      {
         if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, ((VvDocumentRecord)vvDataRecord).TransRecordName)) return false;
         if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, ((VvDocumentRecord)vvDataRecord).TransRecordNameArhiva)) return false;

         if(vvDataRecord.IsPolyDocument)
         {
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, ((VvPolyDocumRecord)vvDataRecord).TransRecordName2)) return false;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, ((VvPolyDocumRecord)vvDataRecord).TransRecordNameArhiva2)) return false;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, ((VvPolyDocumRecord)vvDataRecord).TransRecordName3)) return false;
            if(!VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, ((VvPolyDocumRecord)vvDataRecord).TransRecordNameArhiva3)) return false;
         }
      }

      #endregion CheckTable(conn, dbName, arhivedDataRecord .VirtualRecordName

      ArhivaTableIsNotEmpty = VvDaoBase.CountAllRecords(TheDbConnection, vvDataRecord.VirtualRecordNameArhiva) > 0;

      #region switch 4 TheVvUC = new XyVvUC(panelZaUC, (XY)arhivedDataRecord, vvSubModul);

      // next lajn iz project dependent VvUserControl factory (virtuals or overriders are in VvForm_Q / VsrForm, ... 
      TheVvUC = TheVvForm.CreateTheVvUserControl_SwitchSubModulEnum(vvDataRecord, panelZaUC, TheVvSubModul);

      if(TheVvUC == null) return(false);

      #endregion switch 4 TheVvUC = new XyVvUC(panelZaUC, (XY)arhivedDataRecord, vvSubModul);

      IsNotEmptyTable = InitializeVvDataRecord(vvDataRecord);

      TheVvRecordUC.CreateRptFilterAndRptFilterUC();

      if(IsNotEmptyTable)
      {
         TheVvForm.TheVvTabPage.IsArhivaTabPage = false;

         // 31.1.2009: dok nije bilo ovog if()-a, bio bug jer je kod Dodak Novi dolazio onaj FRSREC 
         if(TabPageKind != ZXC.VvTabPageKindEnum.RECORD_TabPage_INTERACTIVE)
         {
            TheVvForm.PutFieldsActions(TheDbConnection, vvDataRecord, TheVvRecordUC);
         }

         FileIsEmpty = false;
      }
      else
      {
         FileIsEmpty = true;
      }

      return true;
   }

   private bool InitializeVvDataRecord(VvDataRecord _vvDataRecord)
   {
      if(TheVvDataRecord.VirtualRecID == 0) // Znaci otvaramo novi TabPage sos friski record u 'Create_vvDataRecord_and_vvRecordUC' nastao. Npr. 'new Nalog()' 
      {
         VvSQL.DBNavigActionType frs_lst = (TheVvDataRecord.IsDocumentLike ? VvSQL.DBNavigActionType.LST : VvSQL.DBNavigActionType.FRS);

         bool success = (TheVvDataRecord.VvDao.FrsPrvNxtLst_REC(TheDbConnection, TheVvDataRecord, frs_lst, /*TheVvDataRecord.DefaultSorter*/ TheVvRecordUC.ThePrefferedRecordSorter, IsArhivaTabPage, TheVvUC.VvNavRestrictor_TT, TheVvUC.VvNavRestrictor_SKL, TheVvUC.VvNavRestrictor_SKL2));

         // 31.10.2013: 
         if(success == false) // ako novododana 'VvNavRestrictor_SKL' logika producira prazni DUC, probaj sa ZXC.DbNavigationRestrictor.Empty 
         {
            success = (TheVvDataRecord.VvDao.FrsPrvNxtLst_REC(TheDbConnection, TheVvDataRecord, frs_lst, /*TheVvDataRecord.DefaultSorter*/ TheVvRecordUC.ThePrefferedRecordSorter, IsArhivaTabPage, TheVvUC.VvNavRestrictor_TT, ZXC.DbNavigationRestrictor.Empty, ZXC.DbNavigationRestrictor.Empty));
            if(success && TheVvUC is FakturDUC)
            {
               FakturDUC fakturDUC = TheVvUC as FakturDUC;
               string foundSkladCDGoodForThisDUC = ((Faktur)(TheVvDataRecord)).SkladCD;

               fakturDUC.dbNavigationRestrictor_SKL = new ZXC.DbNavigationRestrictor(Faktur.skladCd_colName, new string[] { foundSkladCDGoodForThisDUC });

               ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD = foundSkladCDGoodForThisDUC;
            }
         }

         return success;
      }

      return(true); // IsNotEmptyTable 
   }

   #endregion Create_vvDataRecord_and_vvRecordUC

   #region Create_vvRecListUC()

   private bool Create_vvRecListUC(VvRecLstUC initialRecLstUC)
   {
      #region switch 4 arhivedDataRecord = new XyVvDataRecord();

      // next lajn iz project dependent VvDataRecord factory (virtuals or overriders are in VvForm_Q / VsrForm, ... 
      VvDataRecord vvDataRecord = TheVvForm.CreateTheVvDataRecord_SwitchSubModulEnum(TheVvSubModul);

      if(vvDataRecord == null) return (false);

      if(vvDataRecord.IsDocument     && ((VvDocumentRecord) vvDataRecord).VirtualTranses  == null) ZXC.aim_emsg("Zaboravio si 'Transes  = new List<Xtrans>()' u 'Create_vvDataRecord_and_vvRecordUC'");
      if(vvDataRecord.IsPolyDocument && ((VvPolyDocumRecord)vvDataRecord).VirtualTranses2 == null) ZXC.aim_emsg("Zaboravio si 'Transes2 = new List<Xtrans>()' u 'Create_vvDataRecord_and_vvRecordUC'");
      if(vvDataRecord.IsPolyDocument && ((VvPolyDocumRecord)vvDataRecord).VirtualTranses3 == null) ZXC.aim_emsg("Zaboravio si 'Transes3 = new List<Xtrans>()' u 'Create_vvDataRecord_and_vvRecordUC'");

      #endregion switch 4 arhivedDataRecord = new XyVvDataRecord();

      // next lajn iz project dependent VvRecLstUC factory (virtuals or overriders are in VvForm_Q / VsrForm, ... 
      TheVvUC = TheVvForm.CreateTheVvRecLstUC_SwitchSubModulEnum(TheVvSubModul, vvDataRecord, initialRecLstUC, panelZaUC);

      if(TheVvUC == null) return false;
      else                return true;
   }

   public VvRecLstUC InitialRecLstUCFromVvFindDialog(VvRecLstUC initialRecLstUC)
   {
      TheVvUC = initialRecLstUC;
      TheVvRecLstUC.InitializeVvUserControl(panelZaUC);
      TheVvRecLstUC.CalculateLocationAndSize();

      return ((VvRecLstUC)TheVvUC);
   }

   #endregion Create_vvRecListUC()

   #region Create_vvRptFilter_and_vvReportUC

   private void Create_vvRptFilter_and_vvReportUC()
   {
      VvRptFilter vvRptFilter;

      // next lajn iz project dependent VvReportUC factory (virtuals or overriders are in VvForm_Q / VsrForm, ... 
      TheVvUC = TheVvForm.CreateTheVvReportUC_SwitchSubModulEnum(out vvRptFilter, panelZaUC, TheVvSubModul);

      if(TheVvUC == null) return;

      ((VvReportUC)vvUserControl).PutFilterFields(vvRptFilter);
   }

   public void PaliGasiDirtyFlag(bool pali)
   {
      this.HasUnsavedChanges = pali;

      if(pali)
      {
         //this.Image = vvForm.imglst.Images[2];
         this.Image = VvIco.Dirty/*new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.dirty.ico"))*/.ToBitmap();
         //this.Icon = new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.dirty.ico"));
         // new Bitmap(GetType(), "Vektor.Icons.Toolstrip.DEL_1.BMP");//TheVvForm.imglst.Images[2];
      }
      else
      {
         this.Image = null;
      }
   }

   #endregion Create_vvRptFilter_and_vvReportUC

   #region Create_OtherUC

   private void Create_OtherUC()
   { 
    //UserControl   otherUc;
    //VvUserControl otherUc;
      
      if(TheVvSubModul.subModulEnum == ZXC.VvSubModulEnum.SIN      ) TheVvUC = new SIN_UC           (panelZaUC, vvSubModul); 
      if(TheVvSubModul.subModulEnum == ZXC.VvSubModulEnum.R_FUG_PTG) TheVvUC = new FUG_PTG_UC       (panelZaUC, vvSubModul);
      
      if(TheVvSubModul.subModulEnum == ZXC.VvSubModulEnum.R_PCKinf_PTG)
      {
         string artiklCD;
         string skladCD = ZXC.PTG_ZNJ/*"ZNJ"*/;

         artiklCD = PCK_ArtiklList_UC.GetFirstActivePCKartiklCD(TheDbConnection, skladCD, "");

         TheVvUC = new PCK_ArtiklList_UC(panelZaUC, artiklCD, skladCD/*, PCK_ArtiklList_Caller.SubModulAction*/);

       //List<PCK_Artikl> PCK_ArtikList      = RtranoDao.Get_PCK_ArtiklList_ByPCK_Baza_AndSklad(TheDbConnection, TheVvUC.Get_Artikl_FromVvUcSifrar(artiklCD), skladCD, ZXC.PCK_Info_Kind.OvaBazaOnly, false, false);
       //List<PCK_Artikl> PCK_SviArtikliList = RtranoDao.Get_PCK_ArtiklList_ByPCK_Baza_AndSklad(TheDbConnection, null                                       , skladCD, ZXC.PCK_Info_Kind.SveBazeOnly, false, false);
         List<PCK_Artikl> PCK_ArtikList      = RtranoDao.Get_PCK_ArtiklList_ByPCK_Baza_AndSklad(TheDbConnection, TheVvUC.Get_Artikl_FromVvUcSifrar(artiklCD), skladCD, ZXC.PCK_Info_Kind.OvaBazaOnly, ""    , ""  );
         List<PCK_Artikl> PCK_SviArtikliList = RtranoDao.Get_PCK_ArtiklList_ByPCK_Baza_AndSklad(TheDbConnection, null                                       , skladCD, ZXC.PCK_Info_Kind.SveBazeOnly, ""    , ""  );

         ((PCK_ArtiklList_UC)TheVvUC).PutDgvFields(PCK_ArtikList, PCK_SviArtikliList, artiklCD, skladCD);
      }

      // IZLAZNI RACUNI 
      if(TheVvSubModul.subModulEnum == ZXC.VvSubModulEnum.R_F2I)
      {
         TheVvUC = new F2_Izlaz_UC(panelZaUC, vvSubModul);

         // preselio u constructor UC-a 
       //Vv_eRacun_HTTP.Load_IRn_FakturList   ((F2_Izlaz_UC)TheVvUC       );
       //Vv_eRacun_HTTP.QueryOutbox_TRN_Or_DPS((F2_Izlaz_UC)TheVvUC, false);
       //Vv_eRacun_HTTP.QueryOutbox_TRN_Or_DPS((F2_Izlaz_UC)TheVvUC, true );

      }

      // ULAZNI RACUNI 
      if(TheVvSubModul.subModulEnum == ZXC.VvSubModulEnum.R_F2U) 
      {
         TheVvUC = new F2_Ulaz_UC(panelZaUC, vvSubModul);

         // preselio u constructor UC-a 
       //  Vv_eRacun_HTTP.Load_URn_FakturList((F2_Ulaz_UC)TheVvUC);
       ////Vv_eRacun_HTTP.QueryInbox_DPS     ((F2_Ulaz_UC)TheVvUC);
       //  Vv_eRacun_HTTP.QueryInbox_DPS     ((F2_Ulaz_UC)TheVvUC);
      }

   }

   #endregion Create_OtherUC

   #region VvTabPage_VisibleChanged

   public void VvTabPage_VisibleChanged(object sender, EventArgs e)
   {
      if(this.Visible == false) // ovaj, dakle, upravo GUBI visibility (napustamo ga, vec je prije otvoren) 
      {
         if(IsArhivaTabPage) ArhivaTabPageSelectedIndex = TheVvForm.TheTabControl.SelectedIndex;
         else                ArhivaTabPageSelectedIndex = -1;

         thisIsFirstAppereance = false;

         GetTSB_EnabledStateSnapshot();

         return;
      }
      else
      {
         TheVvForm.TH_CheckAndForceFiskalization(); 

         // 15.12.2011 
         ZXC.TheVvForm.VvPref.login.TheINITIAL_VvSubModulEnum_AsInt = (int)TheVvSubModul.subModulEnum;

         //13.04.2012. za FullScreen
         if(this.TabPageKind == ZXC.VvTabPageKindEnum.RECORD_TabPage)
         {
            if(theVvForm.ts_Record.Visible == false) theVvForm.ts_Record.Visible = true;
         }
         else if(this.TabPageKind == ZXC.VvTabPageKindEnum.REPORT_TabPage)
         {
            if(theVvForm.ts_Record.Visible == true && ZXC.TheVvForm.menuStrip.Visible == false) theVvForm.ts_Record.Visible = false;
         }
      }

      if(!thisIsFirstAppereance)
      {
         PutTSB_EnabledStateSnapshot();
         ChangeVisibilitiOfToolStripAndMenuItem_SubModulSet(); // postavlja vidljivost ts_SubModulSet
         if(this.TabPageKind == ZXC.VvTabPageKindEnum.RECORD_TabPage) TheVvForm.SetVvSubModulSetMenuEnabledOrDisabled_RegardingWriteMode(this.WriteMode); // zbog LGO smao ovaj Kind
         TheVvForm.EnableDisable_VvReportSubModuls_onCmdPanel(IsForReport, SubModul_xy);

         if(this.TabPageKind == ZXC.VvTabPageKindEnum.REPORT_TabPage) // zato kad se vracas na izvjTabPage da se vrate izvjestaji
         {
            TheVvForm.SetVisibilitiOfReportModulButton(SubModul_xy);
            TheVvForm.SetReportComboBox(SubModul_xy, TheVvForm.tsCbxReport.SelectedIndex);
         }
         // 2.2.2011: REFRESH-aj record na got visibility 
         else if(this.TabPageKind == ZXC.VvTabPageKindEnum.RECORD_TabPage && writeMode == ZXC.WriteMode.None) 
         {
            // 21.01.2014: 
          //TheVvForm.WhenRecordInDBHasChangedAction();
            if(((VvInnerTabPage)(this.TheVvRecordUC.TheTabControl.SelectedTab)).TheInnerTabPageKindEnum == ZXC.VvInnerTabPageKindEnum.ReportViewer_TabPage == false)
            {
               TheVvForm.WhenRecordInDBHasChangedAction();
            }

            TheVvForm.SetSorterComboBox();
         }

         if(ZXC.VvDataBaseInfoInUse.DataBaseName != this.vvDatabaseInfo.DataBaseName)
         {
            VvUserControl.NullifyAllSifrars();
            ZXC.VvDataBaseInfoInUse = this.vvDatabaseInfo;
         }

         //10.12.2021. da kad se vratimo na FUG njegovi tsButtoni budu eneblirani kako treba
         if(TheVvUC is FUG_PTG_UC) ((FUG_PTG_UC)TheVvUC).ThePolyGridTabControl_SelectionChanged(null, null, ((FUG_PTG_UC)TheVvUC).ThePolyGridTabControl.SelectedTab);

         //02.02.2022. da kad se vratimo na faktur koji ima zoom taj zoom ostane zoom a ne da se smanji
         if(TheVvUC is FakturExtDUC) ((FakturExtDUC)TheVvUC).TheTabControl_SelectionChanged_Zoom(null, null, ((FakturExtDUC)TheVvUC).TheTabControl.SelectedTab); ;

         
         if(TheVvUC is F2_Izlaz_UC) ((F2_Izlaz_UC)TheVvUC).Refresh_FIR(null, null, ((F2_Izlaz_UC)TheVvUC).TheVvTabPage.TheVvForm.TheTabControl.SelectedTab);
         if(TheVvUC is F2_Ulaz_UC ) ((F2_Ulaz_UC )TheVvUC).Refresh_FUR(null, null, ((F2_Ulaz_UC )TheVvUC).TheVvTabPage.TheVvForm.TheTabControl.SelectedTab);
         
         return;
      }

      // ovo dole se znaci dogada na PRVO POJAVLJIVANJE tab-a 

      ChangeVisibilitiOfToolStripAndMenuItem_SubModulSet(); // postavlja vidljivost ts_SubModulSet

      TheVvForm.SubModulSet_EnableOrDisable_TSButtonsAndTSMnItems();
      TheVvForm.EnableDisable_VvReportSubModuls_onCmdPanel(IsForReport, SubModul_xy);

      switch (this.TabPageKind)
      {
         case ZXC.VvTabPageKindEnum.RECORD_TabPage:
               TheVvForm.SetVvMenuEnabledOrDisabled_RegardingWriteMode(this.WriteMode);
               // TODO: Tamara, cemu sluzi ovaj SetDirtyFlag? 
               TheVvForm.SetDirtyFlag("VvTabPage_VisibleChanged");
               TheVvForm.SetSorterComboBox();
            break;

         case ZXC.VvTabPageKindEnum.RecLIST_TabPage:
              TheVvForm.SetVvMenuEnabledOrDisabled_NoTabPageIsOpened();
              break;

         case ZXC.VvTabPageKindEnum.REPORT_TabPage:
              TheVvForm.SetVvMenuEnabledOrDisabled_FilterTabPageIsOpened(ZXC.WriteMode.None, true);
               //07.04.2012:
              //TheVvReportUC.CalcTheFilterPanelWidth();
              if(TheVvReportUC != null) TheVvReportUC.CalcTheFilterPanelWidth();
              TheVvForm.OrganizeModulButtons(SubModul_xy.X, true);
              TheVvForm.SetReportComboBox(SubModul_xy, TheVvForm.tsCbxReport.SelectedIndex);
              break;

         case ZXC.VvTabPageKindEnum.OTHER_TabPage:
              TheVvForm.SetVvMenuEnabledOrDisabled_NoTabPageIsOpened();
              break;

      }

      ZXC.VvDataBaseInfoInUse = this.vvDatabaseInfo;

   }

   private void PutTSB_EnabledStateSnapshot()
   {
      foreach(ToolStripItem tsItem in TheVvForm.ts_Record.Items)
      {
         if(!(tsItem is ToolStripButton)) continue;

         tsItem.Enabled = RecordTSB_EnabledStateList.SingleOrDefault(tsbe => tsbe.name == tsItem.Name).enabled; // qLinq 
      }
   }

   private void GetTSB_EnabledStateSnapshot()
   {
      RecordTSB_EnabledStateList.Clear();

      foreach(ToolStripItem tsItem in TheVvForm.ts_Record.Items)
      {
         if(!(tsItem is ToolStripButton)) continue;

         TsbEnabled tsbe = new TsbEnabled();

         tsbe.name = tsItem.Name;
         tsbe.enabled = tsItem.Enabled;

         RecordTSB_EnabledStateList.Add(tsbe);
      }
   }

   public void ChangeVisibilitiOfToolStripAndMenuItem_SubModulSet()
   {
      ToolStrip ts_Set           = TheVvForm.ats_SubModulSet[SubModul_xy.X][SubModul_xy.Y];
      ToolStripMenuItem tsmi_Set = TheVvForm.aTopMenuItem_SubModulSet[SubModul_xy.X][SubModul_xy.Y];
      TheVvForm.aTopSetSubModul  = tsmi_Set;

      if((this.TabPageKind == ZXC.VvTabPageKindEnum.RECORD_TabPage && this.vvUserControl != null &&
         ((VvRecordUC)this.vvUserControl).TheTabControl != null    &&
         ((VvInnerTabPage)((VvRecordUC)this.vvUserControl).TheTabControl.SelectedTab).TheInnerTabPageKindEnum == ZXC.VvInnerTabPageKindEnum.ReportViewer_TabPage &&
         ((VvInnerTabPage)((VvRecordUC)this.vvUserControl).TheTabControl.SelectedTab).Visible == true)
             || this.TabPageKind == ZXC.VvTabPageKindEnum.REPORT_TabPage)
      {
         TheVvForm.ts_Report.Visible         = true;
         TheVvForm.ts_Report.Dock            = DockStyle.Left;
         TheVvForm.ts_Report.Parent          = TheVvForm.tsPanel_SubModul;
         TheVvForm.reportTopMenuItem.Visible = true;
         TheVvForm.ts_Report.Location        = new Point(3, 0); // tamtam 30.9 provremno rj da ne plese

         if(this.TabPageKind == ZXC.VvTabPageKindEnum.REPORT_TabPage)
         {
            TheVvForm.ToolStripReportVisible_tsBtnOnTsReportVisible(true, ReportMode, false);
         }
         else
         {
            TheVvForm.ToolStripReportVisible_tsBtnOnTsReportVisible(false, ReportMode, false);
         }

         for(int i = 0; i < TheVvForm.NumOfModulLevel; ++i)
         {
            for(int j = 0; j < TheVvForm.NumOfSubModulLevel(i); j++)
            {
               ToolStripMenuItem tsmi = TheVvForm.aTopMenuItem_SubModulSet[i][j];
               ToolStrip           ts = TheVvForm.ats_SubModulSet[i][j];

               tsmi.Visible = ts.Visible = false;
               ts.Dock      = DockStyle.Right;
            }
         }
         ts_Set.Visible = false;
      }
      else
      {
         tsmi_Set.Visible = true;
         ts_Set.Visible   = true;
         ts_Set.Dock      = DockStyle.Left;
         ts_Set.Location  = new Point(3, 0);// tamtam 30.9 provremno rj da ne plese

         for(int i = 0; i < TheVvForm.NumOfModulLevel; ++i)
         {
            for(int j = 0; j < TheVvForm.NumOfSubModulLevel(i); j++)
            {
               ToolStripMenuItem tsmi = TheVvForm.aTopMenuItem_SubModulSet[i][j];
               ToolStrip           ts = TheVvForm.ats_SubModulSet[i][j];
               if(tsmi != tsmi_Set)
               {
                  tsmi.Visible = ts.Visible = false;
                  ts.Dock      = DockStyle.Right;
               }
            }
         }

         TheVvForm.ts_Report.Visible = false;
         TheVvForm.ts_Report.Dock    = DockStyle.Right;

         TheVvForm.reportTopMenuItem.Visible = false;

         // ovo je zbog btna "LGO" pod pretpostavkom da je uvijek 1. i mu je tekst "LGO"
         if(this.TabPageKind == ZXC.VvTabPageKindEnum.RecLIST_TabPage)
         {
            ToolStripButton tsb = TheVvForm.atsBtn_SubModulSet[SubModul_xy.X][SubModul_xy.Y][0];
            if(tsb.Text == "LGO" && TheVvRecLstUC.SelectedRecID < 1) tsb.Enabled = false;
            else                                                     tsb.Enabled = true;
         }

         if(ts_Set.Items["StartLink"] != null)
         {
            if(ZXC.RISK_CopyToOtherDUC_inProgress  == true ||
               ZXC.MIXER_CopyToOtherDUC_inProgress == true) ts_Set.Items["StartLink"].Text = "EndLink";
            else                                            ts_Set.Items["StartLink"].Text = "StartLink";
         }
      }
   }

   #endregion VvTabPage_VisibleChanged

   #region TamponPanel_Header .. InitializeTamponPanel_Footer .. PanelZaUC
 
   private void CalcTextBoxWidth()
   {
      switch (this.TabPageKind)
      { 
         case ZXC.VvTabPageKindEnum.RecLIST_TabPage:
         case ZXC.VvTabPageKindEnum.REPORT_TabPage:
               col1 = col2 = col3 = col4 = 0;
               break;

         case ZXC.VvTabPageKindEnum.RECORD_TabPage:
            
            switch (TheVvSubModul.subModulEnum)
             {
               case ZXC.VvSubModulEnum.PRJ:
               case ZXC.VvSubModulEnum.KID:
                  col1 = ZXC.Q3un + ZXC.Qun10;
                  col2 = ZXC.Q3un + ZXC.Qun10;
                  col3 = ZXC.Q10un * 2;
                  break;

               case ZXC.VvSubModulEnum.NAL_F:
               case ZXC.VvSubModulEnum.NAL_O:
               case ZXC.VvSubModulEnum.NAL_M:
               case ZXC.VvSubModulEnum.NAL_P:
               case ZXC.VvSubModulEnum.AMO:
               case ZXC.VvSubModulEnum.PLA:
               case ZXC.VvSubModulEnum.PLA_2014:
               case ZXC.VvSubModulEnum.PLA_2024:
               case ZXC.VvSubModulEnum.PLA_NP:
                  col1 = ZXC.Q4un;
                  col2 = ZXC.Q4un;
                  col3 = ZXC.Q2un;
                  break;

               case ZXC.VvSubModulEnum.KPL:
                  col1 = ZXC.Q3un + ZXC.Qun10;
                  col2 = ZXC.QUN;
                  col3 = ZXC.Q10un;
                  break;

               case ZXC.VvSubModulEnum.PRV:
                  col1 = ZXC.Q4un;
                  col2 = ZXC.Q3un + ZXC.Qun10;
                  col3 = ZXC.Q6un;
                  col4 = ZXC.Q9un;
                  break;

               case ZXC.VvSubModulEnum.ART:
               case ZXC.VvSubModulEnum.OSR:
                  col1 = ZXC.Q6un;
                  col2 = ZXC.Q10un + ZXC.Q5un;
                  break;

               case ZXC.VvSubModulEnum.PER:
                  col1 = ZXC.Q3un + ZXC.Qun10;
                  col2 = ZXC.Q10un;
                  col3 = ZXC.Q6un;
                  break;

               case ZXC.VvSubModulEnum.Vsr_PRM:
                  col1 = ZXC.Q3un + ZXC.Qun10;
                  col2 = ZXC.Q4un;
                  col3 = ZXC.Q10un;
                  break;
    
               case ZXC.VvSubModulEnum.Vsr_ANK:
                  col1 = ZXC.Q4un;
                  col2 = ZXC.Q4un + ZXC.Qun8;
                  col3 = ZXC.Q10un + ZXC.Q3un;
                  break;
              
               case ZXC.VvSubModulEnum.Vrm_DBT:
                  col1 = ZXC.Q5un;
                  col2 = ZXC.Q10un;
                  col3 = ZXC.Q4un;
                  col4 = ZXC.Q2un;
                  col5 = ZXC.Q7un;
                  col6 = ZXC.Q2un;
                  col7 = ZXC.Q2un;
                  break;

               case ZXC.VvSubModulEnum.R_IRA:
               case ZXC.VvSubModulEnum.R_IRA_MPC:
               case ZXC.VvSubModulEnum.R_IRP:
               case ZXC.VvSubModulEnum.R_IFA:
               case ZXC.VvSubModulEnum.R_IFAdev:
               case ZXC.VvSubModulEnum.R_IRM:
               case ZXC.VvSubModulEnum.R_IRM_2:
                  col1 = ZXC.Q4un;
                  col2 = ZXC.Q4un;
                  col3 = ZXC.Q10un;
                  col4 = ZXC.Q2un;
                  col5 = ZXC.Q2un;
                  col6 = ZXC.Q3un;
                  col7 = ZXC.Q3un;
                  break;
               case ZXC.VvSubModulEnum.R_UFA:
               case ZXC.VvSubModulEnum.R_UPA:
               case ZXC.VvSubModulEnum.R_UFM:
               case ZXC.VvSubModulEnum.R_UFAdev:
               case ZXC.VvSubModulEnum.R_URA:
               case ZXC.VvSubModulEnum.R_URA_SVD:
               case ZXC.VvSubModulEnum.R_URP:
               case ZXC.VvSubModulEnum.R_KLK:
               case ZXC.VvSubModulEnum.R_KKM:
               case ZXC.VvSubModulEnum.R_UPM:
               case ZXC.VvSubModulEnum.R_KLK_2:
               case ZXC.VvSubModulEnum.R_KLD:
               case ZXC.VvSubModulEnum.R_PRI:
               case ZXC.VvSubModulEnum.R_PRI_P:
               case ZXC.VvSubModulEnum.R_PRIdev:
               case ZXC.VvSubModulEnum.R_PRI_POT:
               case ZXC.VvSubModulEnum.R_POU:
               case ZXC.VvSubModulEnum.R_POI:
               case ZXC.VvSubModulEnum.R_CJ:
               case ZXC.VvSubModulEnum.R_CJK:
               case ZXC.VvSubModulEnum.R_IZM:
               case ZXC.VvSubModulEnum.R_IZM_2:
               case ZXC.VvSubModulEnum.R_MSI:
               case ZXC.VvSubModulEnum.R_MSI_2:
               case ZXC.VvSubModulEnum.R_OPN:
               case ZXC.VvSubModulEnum.R_PIZ:
               case ZXC.VvSubModulEnum.R_PIM:
               case ZXC.VvSubModulEnum.R_NOR:
               case ZXC.VvSubModulEnum.R_BOR:
               case ZXC.VvSubModulEnum.R_PIZ_P:
               case ZXC.VvSubModulEnum.R_PST:
               case ZXC.VvSubModulEnum.R_SKO:
               case ZXC.VvSubModulEnum.R_STU:
               case ZXC.VvSubModulEnum.R_STI:
               case ZXC.VvSubModulEnum.R_PON:
               case ZXC.VvSubModulEnum.R_PON_MPC:
               case ZXC.VvSubModulEnum.R_OPN_MPC:
               case ZXC.VvSubModulEnum.R_PNM:
               case ZXC.VvSubModulEnum.R_RVI:
               case ZXC.VvSubModulEnum.R_RVU:
               case ZXC.VvSubModulEnum.R_UOD:
               case ZXC.VvSubModulEnum.R_UPV:
               case ZXC.VvSubModulEnum.R_ZPC:
               case ZXC.VvSubModulEnum.R_RNP:
               case ZXC.VvSubModulEnum.R_RNM:
               case ZXC.VvSubModulEnum.R_RNS:
               case ZXC.VvSubModulEnum.R_RNZ:
               case ZXC.VvSubModulEnum.R_PRJ:
               case ZXC.VvSubModulEnum.R_PRI_bc:
               case ZXC.VvSubModulEnum.R_VMI:
               case ZXC.VvSubModulEnum.R_VMI_2:
               case ZXC.VvSubModulEnum.R_MVI:
               case ZXC.VvSubModulEnum.R_MVI_2:
               case ZXC.VvSubModulEnum.R_MMI:
               case ZXC.VvSubModulEnum.R_URM:
               case ZXC.VvSubModulEnum.R_URM_2:
               case ZXC.VvSubModulEnum.R_URM_D:
               case ZXC.VvSubModulEnum.R_PSM:
               case ZXC.VvSubModulEnum.R_INM:
               case ZXC.VvSubModulEnum.R_UPL:
               case ZXC.VvSubModulEnum.R_ISP:
               case ZXC.VvSubModulEnum.R_BUP:
               case ZXC.VvSubModulEnum.R_BIS:
               case ZXC.VvSubModulEnum.R_ABU:
               case ZXC.VvSubModulEnum.R_ABI:
               case ZXC.VvSubModulEnum.R_KIZ:
               case ZXC.VvSubModulEnum.R_PIK:
               case ZXC.VvSubModulEnum.R_TRI:
               case ZXC.VvSubModulEnum.R_NRM:
               case ZXC.VvSubModulEnum.R_WYR:

                  col1 = ZXC.Q5un;
                  col2 = ZXC.Q4un;
                  col3 = ZXC.Q10un;
                  break;


               default:
                  col1 = ZXC.Q4un;
                  col2 = ZXC.Q4un;
                  col3 = ZXC.Q10un;
                  break;

             }
             break;
      }
   }

   private void InitializeTamponPanel_Header(Control _parentVvTp)
   {
      tamponPanel_Header = new Panel();

      tamponPanel_Header.Parent    = this;
      tamponPanel_Header.Location  = new Point(0, 0);
      tamponPanel_Header.Size      = new Size(this.Width, ZXC.Q2un);
      tamponPanel_Header.Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      tamponPanel_Header.BackColor = ZXC.vvColors.tamponPanel_BackColor;

      CreateHamperModul();
      CreateHamperNaziv();

      CreateLabelaArhiva();
      CreateLabelaDeviza();

      labTamPanCrta           = new Label();
      labTamPanCrta.Parent    = this.tamponPanel_Header;
      labTamPanCrta.Location  = new Point(0, tamponPanel_Header.Height - ZXC.Qun4);
      labTamPanCrta.Size      = new Size(this.tamponPanel_Header.Width, ZXC.Qun10*2);
      labTamPanCrta.Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      labTamPanCrta.BackColor = ZXC.vvColors.tamponPanel_Crta;
      labTamPanCrta.Tag       = ZXC.vvColors.tamponPanel_Crta;
   }

   private void CreateLabelaArhiva()
   {
      labArhiva           = new Label();
      labArhiva.Parent    = tamponPanel_Header;
      labArhiva.Text      = "ARHIVA";
      labArhiva.Location  = new Point(hampModul.Right, ZXC.Qun10 + ZXC.Qun4);
      labArhiva.Size      = new Size(tamponPanel_Header.Width - hampModul.Width - hampNazivPrjkt.Width - ZXC.Qun2, ZXC.QUN);
      labArhiva.AutoSize  = false;
      labArhiva.Font      = ZXC.vvFont.LargeBoldFont;
      labArhiva.Visible   = false;
      labArhiva.ForeColor = Color.Red;
      labArhiva.TextAlign = ContentAlignment.MiddleCenter;
      labArhiva.Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
   }

   private void CreateHamperModul()
   { 
      hampModul = new VvHamper(7, 1, "", tamponPanel_Header, false, ZXC.Qun10, ZXC.Qun10, ZXC.Qun10);

      hampModul.VvColWdt      = new int[] {     col1,    col2,      col3,      col4,     col5,      col6,     col7 };
      hampModul.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 , ZXC.Qun4, ZXC.Qun4 , ZXC.Qun4 };
      hampModul.VvRightMargin = hampModul.VvLeftMargin;

      hampModul.VvRowHgt       = new int[] { ZXC.QUN };
      hampModul.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hampModul.VvBottomMargin = hampModul.VvTopMargin;

      tbx_col1 = hampModul.CreateVvTextBox(0, 0, "tbx_col1", "");
      tbx_col2 = hampModul.CreateVvTextBox(1, 0, "tbx_col2", "");
      tbx_col3 = hampModul.CreateVvTextBox(2, 0, "tbx_col3", "");
      tbx_col4 = hampModul.CreateVvTextBox(3, 0, "tbx_col4", "");
      tbx_col5 = hampModul.CreateVvTextBox(4, 0, "tbx_col5", "");
      tbx_col6 = hampModul.CreateVvTextBox(5, 0, "tbx_col6", "");
      tbx_col7 = hampModul.CreateVvTextBox(6, 0, "tbx_col7", "");

      tbx_col1.JAM_ReadOnly = 
      tbx_col2.JAM_ReadOnly = 
      tbx_col3.JAM_ReadOnly = 
      tbx_col4.JAM_ReadOnly = 
      tbx_col5.JAM_ReadOnly = 
      tbx_col6.JAM_ReadOnly =
      tbx_col7.JAM_ReadOnly = true;

      //tamtam***** nekak drukcije s tim tagom nekaj zmislii
      tbx_col1.Tag = tbx_col2.Tag = tbx_col3.Tag = tbx_col4.Tag = tbx_col5.Tag = tbx_col6.Tag = tbx_col7.Tag = ZXC.vvColors.tamponHeaderLeftTbx_BackColor;
      tbx_col4.JAM_ForeColor = Color.Red;
      tbx_col4.Font = ZXC.vvFont.BaseBoldFont;
      tbx_col4.TextAlign = HorizontalAlignment.Center;
      tbx_col5.TextAlign = HorizontalAlignment.Center;
    //tbx_col5.Tag = tbx_col6.Tag = tbx_col7.Tag = Color.LavenderBlush;

      VvHamper.Open_Close_Fields_ForWriting(hampModul, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.TamponPanel_HeaderLeft);
   }

   private void CreateLabelaDeviza()
   {
      labDeviza           = new Label();
      labDeviza.Parent    = tamponPanel_Header;
      labDeviza.Text      = "Iznosi u devizi";
      labDeviza.Location  = new Point(hampModul.Right, ZXC.Qun10 + ZXC.Qun4);
      labDeviza.Size      = new Size(tamponPanel_Header.Width - hampModul.Width - hampNazivPrjkt.Width - ZXC.Qun2, ZXC.QUN);
      labDeviza.AutoSize  = false;
      labDeviza.Font      = ZXC.vvFont.LargeBoldFont;
      labDeviza.Visible   = false;
      labDeviza.ForeColor = Color.DarkGreen;
      labDeviza.TextAlign = ContentAlignment.MiddleCenter;
      labDeviza.Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
   }

   private void CreateHamperNaziv()
   {
      hampNazivPrjkt = new VvHamper(1, 1, "", tamponPanel_Header, false);

      hampNazivPrjkt.VvColWdt      = new int[] { ZXC.Q10un };
      hampNazivPrjkt.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hampNazivPrjkt.VvRightMargin = hampNazivPrjkt.VvLeftMargin;

      hampNazivPrjkt.VvRowHgt       = new int[] { ZXC.QUN };
      hampNazivPrjkt.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hampNazivPrjkt.VvBottomMargin = hampNazivPrjkt.VvTopMargin;

      tbx_NazivPrjkt              = hampNazivPrjkt.CreateVvTextBox(0, 0, "tbx_NazivPrjkt", "");
      tbx_NazivPrjkt.JAM_ReadOnly = true;
      tbx_NazivPrjkt.Tag          = "AddPrjktColor";//"AddModColor";
      tbx_NazivPrjkt.Font         = ZXC.vvFont.BaseBoldFont;
      tbx_NazivPrjkt.TextAlign    = HorizontalAlignment.Right;

      hampNazivPrjkt.Location = new Point(tamponPanel_Header.Width - hampNazivPrjkt.Width - ZXC.Qun5, ZXC.Qun10);
      hampNazivPrjkt.Anchor   = AnchorStyles.Top | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(hampNazivPrjkt, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.TamponPanel_HeaderPrjkt);

   }

   private void InitializeTamponPanel_Footer(Crownwood.DotNetMagic.Controls.TabControl parentVvTp)
   {
      tamponPanel_Footer           = new Panel();
      tamponPanel_Footer.Parent    = this;
      tamponPanel_Footer.Anchor    = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      tamponPanel_Footer.BackColor = ZXC.vvColors.tamponPanel_BackColor;

      CreateHamperAdd();
      CreateHamperMod();

      CreateVvTextBoxArhiva();

   }

   private void CreateHamperAdd()
   {
      hampAdd = new VvHamper(1, 1, "", tamponPanel_Footer, false);

      hampAdd.VvColWdt      = new int[] { ZXC.Q10un+ ZXC.Q2un };
      hampAdd.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hampAdd.VvRightMargin = hampAdd.VvLeftMargin;

      hampAdd.VvRowHgt       = new int[] { ZXC.QUN };
      hampAdd.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hampAdd.VvBottomMargin = hampAdd.VvTopMargin;

      tbx_addMetaData = hampAdd.CreateVvTextBox(0, 0, "tbxaddTSU_UID", "");

      tbx_addMetaData.JAM_ReadOnly = true;
      tbx_addMetaData.Tag          = "AddModColor";
      tbx_addMetaData.TextAlign    =  HorizontalAlignment.Left;

      tamponPanel_Footer.Size = new Size(this.Width, hampAdd.Height + ZXC.Qun5);

      hampAdd.Location = new Point(ZXC.Qun5, ZXC.Qun10);
      hampAdd.Anchor   = AnchorStyles.Top | AnchorStyles.Left;
      
      VvHamper.Open_Close_Fields_ForWriting(hampAdd, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.TamponPanel_Footer);
   }

   private void CreateHamperMod()
   {
      hampMod = new VvHamper(1, 1, "", tamponPanel_Footer, false);

      hampMod.VvColWdt      = new int[] { ZXC.Q10un + ZXC.Q2un };
      hampMod.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hampMod.VvRightMargin = hampMod.VvLeftMargin;

      hampMod.VvRowHgt       = new int[] { ZXC.QUN };
      hampMod.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hampMod.VvBottomMargin = hampMod.VvTopMargin;

      tbx_modMetaData              = hampMod.CreateVvTextBox(0, 0, "tbxmodTS_UID", "");
      tbx_modMetaData.JAM_ReadOnly = true;
      tbx_modMetaData.Tag          = "AddModColor";
      tbx_modMetaData.TextAlign    = HorizontalAlignment.Right;

      hampMod.Location = new Point(tamponPanel_Footer.Width - hampMod.Width - ZXC.Qun5, ZXC.Qun10);
      hampMod.Anchor   = AnchorStyles.Top | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(hampMod, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.TamponPanel_Footer);
   }

   private void CreateVvTextBoxArhiva()
   {
      tbx_arVer              = new VvTextBox();
      tbx_arVer.Parent       = tamponPanel_Footer;
      tbx_arVer.Location     = new Point(hampAdd.Right, ZXC.Qun10 + ZXC.Qun4);
      tbx_arVer.AutoSize     = false;
      tbx_arVer.Font         = ZXC.vvFont.BaseFont;
      tbx_arVer.Size         = new Size (tamponPanel_Footer.Width - hampMod.Width - hampAdd.Right - ZXC.Qun5, ZXC.QUN);
      tbx_arVer.JAM_ReadOnly = true;
      tbx_arVer.Tag          = "Arhiva";
      tbx_arVer.TextAlign    = HorizontalAlignment.Center;
      tbx_arVer.Anchor       = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
    
      VvHamper.Open_Close_Fields_ForWriting(tbx_arVer, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.TamponPanel_Footer);
   }

   public void InitializePanelZaUC()
   {
      panelZaUC        = new Panel();
      panelZaUC.Parent = this;
      panelZaUC.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
      panelZaUC.Name   = "panelZaUC";
   }

   #endregion TamponPanel_Header .. InitializeTamponPanel_Footer .. PanelZaUC

   #region implementing Dispose (cleanup 4 unmenidzd sorsis (dbConnecgtion))

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

            //27.4.2009: after huge dbConnection reorganization
            //if(this.conn != null) this.conn.Close();

            if(this.IsForReport && TheVvReportUC != null && TheVvReportUC.TheVvReport != null && TheVvReportUC.TheVvReport.VirtualUntypedDataSet != null)
            {
               TheVvReportUC.TheVvReport.VirtualReportDocument.Close();

               foreach(System.Data.DataTable dt in TheVvReportUC.TheVvReport.VirtualUntypedDataSet.Tables)
               {
                  dt.Dispose();
               }

               TheVvReportUC.TheVvReport.VirtualUntypedDataSet.Dispose();
            }
            // ovi elsovi dodanui 27.12.2010: 
            else if(this.IsForReport && TheVvReportUC != null && TheVvReportUC.TheVvReport != null)
            {
               TheVvReportUC.TheVvReport.VirtualReportDocument.Close();
            }
            else if(this.TabPageKind == ZXC.VvTabPageKindEnum.RECORD_TabPage && TheVvRecordUC != null && TheVvRecordUC.TheVvReport != null)
            {
               TheVvRecordUC.TheVvReport.VirtualReportDocument.Close();
            }

            //===================================================

            // 04.06.2025:
            if(this.TheVvUC is PCK_ArtiklList_UC)
            {
               (this.TheVvUC as PCK_ArtiklList_UC).AutoRefreshTimer.Dispose();
            }

            this.alreadyDisposed = true;
         }
         finally
         {
            // Call Dispose on your base class.
            base.Dispose(disposing);
         }
      }
   }


   #endregion implementing Dispose (cleanup 4 unmenidzd sorsis (dbConnecgtion))

   #region BackgroundWorker

   private void CreateBackgroundWorker()
   {
      this.TheBackgroundWorker = new System.ComponentModel.BackgroundWorker();

      this.TheBackgroundWorker.WorkerReportsProgress = true;
      this.TheBackgroundWorker.WorkerSupportsCancellation = true;
      this.TheBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
      this.TheBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
      this.TheBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);

   }

   private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
   {
      // Handle the case where an exception was thrown.
      if(e.Error != null)
      {
         MessageBox.Show(e.Error.Message);
      }
      // 29.04.2010: NTR fin izvjestaji uvijek ulaze ovdje jer im je c.Canceled = true.
      // Do daljnjega, ovako 'pjeske' zjebemo...                                       
      //else if(e.Cancelled)                                                           


      // 10.10.2011: ovaj else if za 'Cancel' slucaj je potpuno nedorecen, do dalnjega izjednacavam Cancel i Finished 
      else if(/*e.Cancelled && (TheVvReportUC.TheVvReport is VvFinNtrReport) == false*/ false)
      {
         //MessageBox.Show("CANCELED");
         this.ReportMode = ZXC.ReportMode.Fresh;
      }
      else
      {
         // the operation succeeded.
          //MessageBox.Show("FINISHED");
         this.ReportMode = ZXC.ReportMode.Done;
         TheVvReportUC.Label_reportInProgressMessage.Visible = false;
      }

      TheVvForm.numberOfRunningWorkers -= 1;
      TheVvForm.Report_SubModul_EnableOrDisable_TSButtonsAndTSMnItems(ReportMode, true);

      // 25.3.2011. za one kojima zelimo odmah vidjeti groupTree
    //  if(TheVvReportUC.TheVvReport is RptR_Dugovanja || TheVvReportUC.TheVvReport is RptR_Potrazivanja)
      if(TheVvReportUC.TheVvReport != null && 
         (TheVvReportUC.TheVvReport.VirtualReportDocument is Vektor.Reports.RIZ.CR_DugPotInRisk     || 
          TheVvReportUC.TheVvReport.VirtualReportDocument is Vektor.Reports.RIZ.CR_DugPotInRisk_Kum ||
          TheVvReportUC.TheVvReport.VirtualRptFilter.NeedsGroupTree == true )) //10.04.2012. svi grupirani izvj trebaju GroupTree
      {
         ZXC.TheVvForm.TheVvReportUC.VirtualReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.GroupTree;
      }

      // 27.11.2016: 
      if(TheVvReportUC.TheVvReport != null)
      {
         TheVvReportUC.TheVvReport.TheReportLists_Clear();
      }
   }

   private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
   {
      // This method will run on a thread other than the UI thread.
      // Be sure not to manipulate any Windows Forms controls created
      // on the UI thread from this method.
      BackgroundWorker worker = sender as BackgroundWorker;
      ZXC.Koordinata_3D xyz = (ZXC.Koordinata_3D)e.Argument;

      TheVvForm.numberOfRunningWorkers += 1;

      e.Result = TheVvForm.CreateVvReport(xyz, worker, e);

      //if(worker.CancellationPending)...11.01.2010:
      if(worker.CancellationPending || (e.Result as int?)== -1)
      {
         e.Cancel = true;
      }
   }

   private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
   {
      // sender is BackgroundWorker 

      TheVvForm.progressBar.Visible = true;
   }

   #endregion BackgroundWorker

}

public class VvInnerTabPage : Crownwood.DotNetMagic.Controls.TabPage, IDisposable
{
   public ZXC.VvInnerTabPageKindEnum TheInnerTabPageKindEnum { get; set; }

   public VvInnerTabPage(string _title, string _name, ZXC.VvInnerTabPageKindEnum _theInnerTabPageKindEnum)
   {
      this.AutoScroll              = true;  // mora biti da ga TheG ne dobije
      this.BackColor               = ZXC.vvColors.userControl_BackColor;

      this.Title                   = _title;
      this.Name                    = _name.IsEmpty() ? _title : _name;

      this.TheInnerTabPageKindEnum = _theInnerTabPageKindEnum;
   }
}

public class VvAddInteractiveSifrarRecordDlg : Crownwood.DotNetMagic.Forms.DotNetMagicForm
{
   #region Fieldz

   private ToolStrip         ts_AddSifrar;
   private ToolStripButton   tsb_Save, tsb_Esc;
   private MenuStrip         menuStrip;
   private ToolStripMenuItem tsMi_datoteka, tsMi_Save, tsMi_Esc;
   
   private Crownwood.DotNetMagic.Controls.TabControl tabControl;

   #endregion Fieldz

   #region Propertiz

   private VvTabPage TheVvTabPage { get; set; }

   private VvRecordUC TheVvRecordUC 
   { 
      get 
      {
         return TheVvTabPage.TheVvRecordUC;
      } 
   }

   private VvDataRecord TheVvDataRecord
   {
      get { return TheVvRecordUC.VirtualDataRecord; }
      //set { TheVvRecordUC.VirtualDataRecord = value; }
   }

   private XSqlConnection TheDbConnection
   {
      get { return TheVvTabPage.TheDbConnection; }
   }

   private IVvDao TheVvDao
   {
      get { return TheVvDataRecord.VvDao; }
   }

   #endregion Propertiz

   #region Constructor

   public VvAddInteractiveSifrarRecordDlg(VvTabPage vvTabPage)
   {
      TheVvTabPage = vvTabPage;
           
      this.Font        = ZXC.vvFont.BaseFont;
      this.Style       = ZXC.vvColors.vvform_VisualStyle;
      this.BackColor   = ZXC.vvColors.userControl_BackColor;
      this.MaximizeBox = false;

      SuspendLayout();
     
      CreateToolStripAndMenuStrip();

      CreateTabControlAndAddVvabPage();
   
      ResumeLayout();
     
      CalcLocationAndSize();

   }

   #endregion Constructor

   #region Methodz

   private void CreateTabControlAndAddVvabPage()
   {
      tabControl                  = new Crownwood.DotNetMagic.Controls.TabControl();
      tabControl.Parent           = this;
      tabControl.OfficeStyle      = ZXC.vvColors.tabControl_OfficeStyle;
      tabControl.Style            = ZXC.vvColors.vvform_VisualStyle;
      tabControl.HideTabsMode     = HideTabsModes.HideAlways;
      tabControl.MediaPlayerStyle = ZXC.vvColors.tabControl_MediaPlayerStyle;

      tabControl.TabPages.Add(TheVvTabPage);
   }

   private void CalcLocationAndSize()
   {
      TheVvTabPage.Size = new Size(TheVvTabPage.TheVvUC.ThisUcSize.Width, TheVvTabPage.TheVvUC.ThisUcSize.Height +
                                                                          TheVvTabPage.TamponPanel_Footer.Height +
                                                                          TheVvTabPage.TamponPanel_Header.Height);

      TheVvTabPage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

      tabControl.Location = new Point(0, ts_AddSifrar.Height + menuStrip.Height);

      tabControl.Size = TheVvTabPage.Size;
      tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

      this.ClientSize = new Size(tabControl.Width, tabControl.Height + ts_AddSifrar.Height + menuStrip.Height);
      this.MinimumSize = this.Size;
   }
  

   #endregion Methodz

   #region ToolStrip_MenuStrip

   private void CreateToolStripAndMenuStrip()
   {
      ts_AddSifrar           = new ToolStrip();
      ts_AddSifrar.Parent    = this;
      ts_AddSifrar.Dock      = DockStyle.Top;
      ts_AddSifrar.GripStyle = ToolStripGripStyle.Hidden;

      tsb_Save = new ToolStripButton("Spremi"  , VvIco.Sav32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.sav.ico")), 32, 32)*/.ToBitmap(), new EventHandler(SaveNewSifrarRecord_Button_Click), "Save");
      tsb_Esc  = new ToolStripButton("Odustani", VvIco.Esc32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.esc.ico")), 32, 32)*/.ToBitmap(), new EventHandler(Esc_Button_Click), "Esc");
      
      tsb_Save.ImageScaling      = tsb_Esc.ImageScaling = ToolStripItemImageScaling.None;
      tsb_Save.DisplayStyle      = tsb_Esc.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
      tsb_Save.TextImageRelation = tsb_Esc.TextImageRelation = TextImageRelation.ImageAboveText;

      ts_AddSifrar.Items.Add(tsb_Save);
      ts_AddSifrar.Items.Add(tsb_Esc);

      menuStrip        = new MenuStrip();
      menuStrip.Parent = this;

      tsMi_datoteka = new ToolStripMenuItem("Datoteka");
      tsMi_Save     = new ToolStripMenuItem("Spremi", null, new EventHandler(SaveNewSifrarRecord_Button_Click), Keys.Control | Keys.S);
      tsMi_Esc      = new ToolStripMenuItem("Odustani", null, new EventHandler(Esc_Button_Click)              , Keys.None          );

      tsMi_datoteka.DropDown.Items.Add(tsMi_Save);
      tsMi_datoteka.DropDown.Items.Add(tsMi_Esc);
      menuStrip.Items.Add(tsMi_datoteka);

   }

   #endregion ToolStrip_MenuStrip

   #region btns_Click

   private void SaveNewSifrarRecord_Button_Click(object sender, EventArgs e)
   {
      bool OK = SaveVvSifrarRecord();

      if(!OK) return;

      this.DialogResult = DialogResult.OK;

      this.Close();
   }

   private bool SaveVvSifrarRecord()
   {
      bool OK = true;

      if(this.ValidateChildren() == false) return false;

      TheVvTabPage.TheVvUC.GetFields(false);

      // ====== User 4 MySQL additions START =============================
      if(TheVvDataRecord.VirtualRecordName == User.recordName)
      {
         OK = VvSQL.CreateUser(TheDbConnection, TheVvDataRecord as User);
      }
      // ====== User 4 MySQL additions END   =============================


      //10.11.2023. da i ovdje kod spremanja naoravi pck artiklCd
      if(ZXC.IsPCTOGO && TheVvTabPage.TheVvUC is ArtiklUC) // Sredi PCK ArtiklCD 
      {
         ArtiklUC theVvUC  = TheVvTabPage.TheVvUC as ArtiklUC;
         Artikl artikl_rec = TheVvDataRecord as Artikl;
         if(artikl_rec.TS == ZXC.PCK_TS)
         {
            artikl_rec.ArtiklCD   = theVvUC.Fld_ArtiklCd   = artikl_rec.New_ArtiklCD_From_PCK_base_RAM_HDD     ;
            artikl_rec.ArtiklName = theVvUC.Fld_ArtiklName = artikl_rec.New_ArtiklName_From_OldPCK_name_RAM_HDD;
         }
      }


      if(OK)
      {
         OK = TheVvDao.ADDREC(TheDbConnection, TheVvDataRecord);
      }

      return OK;
   }

   private void Esc_Button_Click(object sender, EventArgs e)
   {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
   }

   #endregion btns_Click

}
