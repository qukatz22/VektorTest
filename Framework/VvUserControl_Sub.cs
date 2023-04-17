using System.Drawing;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Windows.Forms;
using System;
using System.Data;
using Crownwood.DotNetMagic.Controls;
using System.Linq;
using System.ComponentModel;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using MySql.Data.MySqlClient;
using XSqlConnection  = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand     = MySql.Data.MySqlClient.MySqlCommand;
using XSqlDataReader  = MySql.Data.MySqlClient.MySqlDataReader;
using XSqlDataAdapter = MySql.Data.MySqlClient.MySqlDataAdapter;
using System.Collections.Generic;
#endif

public interface IVvPrintableUC
{
   CrystalReportViewer VirtualReportViewer
   {
      get;
      //set;
   }
}

public interface IVvRecordAssignableUC
{
   VvDataRecord VirtualDataRecord
   {
      get;
      set;
   }

   VvDaoBase TheVvDao
   {
      get;
   }
}

public abstract  class VvReportUC : VvUserControl, IVvPrintableUC
{
   protected delegate void SetTextCallback(string text);
   private delegate void SetReportSourceCallback(ReportDocument report);
   private delegate void SetDisplayGroupTreeCallback(bool shouldDisplay);

   public VvReport TheVvReport { get; set; }
   public CrystalReportViewer TheReportViewer { get; set; }
   public Label Label_reportInProgressMessage { get; set; }
   public Panel TheFilterPanel                { get; set; }
   public int   TheFilterPanel_Width          { get; set; }
   public Panel TheFilterPanelBottom          { get; set; }
   public int   TheFilterPanelBottom_Height   { get; set; }

   public abstract void PutFilterFields(VvRptFilter _filter_data);

   public abstract void GetFilterFields();

   public abstract void AddFilterMemberz();

   public VvReportUC()
   {
      SuspendLayout();

      CreateVirtualReportViewer();
      CreateTheFilterPanelBootom();
      CreateTheFilterPanel();

      ResumeLayout();
   }


   #region CreateVirtualReportViewer_Label_reportInProgressMessage

   public void CreateVirtualReportViewer()
   {
      this.TheReportViewer                  = new CrystalReportViewer();
      this.TheReportViewer.Parent           = this;
      this.TheReportViewer.Dock             = DockStyle.Fill;
      //this.TheReportViewer.DisplayGroupTree = false;
      this.TheReportViewer.ToolPanelView    = ToolPanelViewType.None;
      this.TheReportViewer.DisplayToolbar   = false;
      this.TheReportViewer.DisplayStatusBar = true;

      // Kada ti intellisense bude dao upotrebu ClickPage i DoubleClickPage eventa. Znat ces da 
      // KONACNO imas neku novu verziju CrystalReportsa koja moze to... 
      //this.TheReportViewer.ClickPage += new EventHandler();
      //this.TheReportViewer.DoubleClickPage += new EventHandler();
      //this.TheReportViewer.ClickPage += new PageMouseEventHandler(TheReportViewer_ClickPage);
      this.TheReportViewer.DoubleClickPage += new PageMouseEventHandler(TheReportViewer_DoubleClickPage);

      Label_reportInProgressMessage             = new Label();
      Label_reportInProgressMessage.Parent      = this.TheReportViewer;
      Label_reportInProgressMessage.Text        = "Izvjestaj u izradi. Ako zelite odustati pritisnite ESC.";
      Label_reportInProgressMessage.AutoSize    = true;
      Label_reportInProgressMessage.Font        = new Font("Tahoma", 15);
      Label_reportInProgressMessage.ForeColor   = Color.Red;
      Label_reportInProgressMessage.Visible     = false;

   }

   //public PageMouseEventHandler TheReportViewer_ClickPage { get; set; }
   internal void TheReportViewer_DoubleClickPage(object sender, PageMouseEventArgs e)
   {
#if DEBUG
      ZXC.aim_emsg("{0}: [{1}]   Text: [{2}]",
         e.ObjectInfo.ObjectType,
         e.ObjectInfo.Name,
         e.ObjectInfo.Text);

      switch(e.ObjectInfo.ObjectType)
      {
         case ObjectType.DatabaseField: break;
         case ObjectType.FormulaField : break;
         case ObjectType.SummaryField : break;
         case ObjectType.Text         : break;

      }
#endif
   }

   #endregion CreateVirtualReportViewer_Label_reportInProgressMessage

   #region CreateTheFilterPanel

   private void CreateTheFilterPanel()
   {
      TheFilterPanel            = new Panel();
      TheFilterPanel.Parent     = this;
      TheFilterPanel.Dock       = ZXC.TheVvForm.modulPanel.Dock;
      TheFilterPanel.BackColor  = ZXC.vvColors.userControl_BackColor;
      TheFilterPanel.Name       = "FilterPanel";
      TheFilterPanel.AutoScroll = true;
      TheFilterPanel.Height     = this.Height;
   }

   public void CalcTheFilterPanelWidth()
   {
      if(TheFilterPanel.VerticalScroll.Visible)
      {
         TheFilterPanel.Width = TheFilterPanel_Width + ZXC.QUN;
      }
      else
         TheFilterPanel.Width = TheFilterPanel_Width;
   }

   
   public void CreateTheFilterPanelBootom()
   {
      TheFilterPanelBottom            = new Panel();
      TheFilterPanelBottom.Parent     = this;
      TheFilterPanelBottom.Dock       = DockStyle.Bottom;
      TheFilterPanelBottom.BackColor  = ZXC.vvColors.userControl_BackColor;
      TheFilterPanelBottom.Name       = "FilterPanelBottom";
      TheFilterPanelBottom.AutoScroll = true;
      TheFilterPanelBottom.Width      = this.Width;
      TheFilterPanelBottom.Tag        = true;
      TheFilterPanelBottom.Height     = 0;
      TheFilterPanelBottom.Visible    = false;
   }

   #endregion CreateTheFilterPanel

   #region SetReportSource_ThreadSafe And SetDisplayGroupTree_ThreadSafe

   public void SetReportSource_ThreadSafe(ReportDocument reportDocument)
   {
      // InvokeRequired required compares the thread ID of the
      // calling thread to the thread ID of the creating thread.
      // If these threads are different, it returns true.
      if(this.TheReportViewer.InvokeRequired)
      {
         SetReportSourceCallback callback = new SetReportSourceCallback(SetReportSource_ThreadSafe);
         this.Invoke(callback, new object[] { reportDocument });
      }
      else
      {
         this.TheReportViewer.ReportSource = reportDocument;

         if(ZXC.TheVvForm.VvPref.reportPrefs.ZoomFactor.IsZero()) ZXC.TheVvForm.VvPref.reportPrefs.ZoomFactor = 1;
         this.TheReportViewer.Zoom(/*this.VirtualRptFilter.PrintZoom*/ZXC.TheVvForm.VvPref.reportPrefs.ZoomFactor);
      }
   }

   public void SetDisplayGroupTree_ThreadSafe(bool shouldDisplay)
   {
      // InvokeRequired required compares the thread ID of the
      // calling thread to the thread ID of the creating thread.
      // If these threads are different, it returns true.
      if(this.TheReportViewer.InvokeRequired)
      {
         SetDisplayGroupTreeCallback callback = new SetDisplayGroupTreeCallback(SetDisplayGroupTree_ThreadSafe);
         this.Invoke(callback, new object[] { shouldDisplay });
      }
      else
      {
          //this.TheReportViewer.DisplayGroupTree = shouldDisplay;
          if(shouldDisplay) TheReportViewer.ToolPanelView = ToolPanelViewType.GroupTree;
          else              TheReportViewer.ToolPanelView = ToolPanelViewType.None;

      }
   }

   #endregion SetReportSource_ThreadSafe And SetDisplayGroupTree_ThreadSafe

   #region IVvPrintableUC Members

   public CrystalReportViewer VirtualReportViewer
   {
      get
      {
         return this.TheReportViewer;
      }
      set
      {
         this.TheReportViewer = value;
      }
   }

   public abstract void ResetRptFilterRbCbControls();

   #endregion



}

public abstract  class VvRecordUC : VvUserControl, IVvRecordAssignableUC, IVvPrintableUC
{

   public VvRecordUC()
   {
      SuspendLayout();

      CreateTheTabControl();
      //CreatePanelForFilter();

      // 05.02.2018: dodan if() 
      if(ZXC.LoadCrystalReports_HasErrors == false)
      {
         CreateTheCrystalViewer();
      }

      CreatePanelForFilter();

      ResumeLayout();
   }

   public abstract VvDataRecord VirtualDataRecord
   {
      get;
      set;
   }

   public abstract void PutFields(VvDataRecord vvDatarecord);
   public virtual  void PutFields(VvDataRecord vvDatarecord, bool isCopying) {}
   
   public virtual  void PutDefaultDUCfields() {}

   public virtual  bool IsOkToInitiateThisAction(ZXC.WriteMode writeMode) { return true; }

   public virtual void CleanUniqueFieldsOnCopyFromOtherRecord() { }

   public abstract VvDaoBase TheVvDao
   {
      get;
   }

   // od 2009: 
   public VvSQL.RecordSorter ThePrefferedRecordSorter { get; set; }

   public TextBox tbx_DummyForDefaultFocus;

   public VvSQL.VvLockerInfo lockerInfo;

   public virtual VvFindDialog CreateVvFindDialog()
   {
      ZXC.aim_emsg("Zis one UC's VvFindDialog still not overriden!");

      return null;
   }

   #region VvMigrator

   /*protected*/internal virtual List<VvMigrator> MigratorList                { get { return null; } }
   protected virtual Control          MigratorRightParentA        { get { return null; } }
   protected virtual Control          MigratorRightParentB        { get { return null; } }
   protected virtual Panel            MigratorLeftParentA         { get { return null; } }
   protected virtual Panel            MigratorLeftParentB         { get { return null; } }
   protected virtual Control          ControlUnderMigLeftParentA  { get { return null; } }
   protected virtual Control          ControlUnderMigLeftParentB  { get { return null; } }

   protected int xA, yA, restXA, xB, yB, restXB;

   protected bool IsParent_A(VvMigrator migrator)
   {
      //Control hamperParent = migrator.TheHamper.Parent;

      //return (hamperParent == MigratorLeftParentA ||
      //        hamperParent == MigratorRightParentA);

      // glupo, ali za sada...
      return migrator.MigName.StartsWith("A");
   }

   protected bool IsParent_B(VvMigrator migrator)
   {
      return IsParent_A(migrator) == false;
   }

   protected void PutMigratorsStates(Control control)
   {
      string migratorName;

      if(control is CheckBox && control.Tag is VvHamper)
      {
         migratorName = control.Name;

         VvMigrator migrator = null;

         // 22.02.2016: 
         if(MigratorList != null)
         {
            migrator = MigratorList.SingleOrDefault(mig => mig.MigName == migratorName);
         }

         if(migrator != null)
         {
            ((CheckBox)control).Checked = migrator.IsMigrated;
         }
         else
         {
            ((CheckBox)control).Checked = false;
         }
      }

      foreach(Control childControl in control.Controls)
      {
         PutMigratorsStates(childControl);
      }
   }

   private void GetMigratorsStates(Control control)
   {
      string migratorName;

      if(control is CheckBox && control.Tag is VvHamper) // dakle svi CheckBox-ovi koji sluze Migrator definiciji moraju u Tag-u imati VvHamper-a! 
      {
         migratorName = control.Name;

         VvMigrator migrator = MigratorList.SingleOrDefault(mig => mig.MigName == migratorName);

         if(migrator != null)
         {
            migrator.IsMigrated = ((CheckBox)control).Checked;
         }
         else
         {
            VvHamper hamper = (Control)control.Tag as VvHamper;

            MigratorList.Add(new VvMigrator(hamper));
         }
      }

      foreach(Control childControl in control.Controls)
      {
         GetMigratorsStates(childControl);
      }
   }

   public void CbxMigrator_CheckedChanged_GetMigratorsStates(object sender, EventArgs e)
   {
      if(ZXC.TheVvForm.PutFieldsInProgress == false)
      {
         GetMigratorsStates(MigratorRightParentA);
         if(MigratorRightParentB != null) GetMigratorsStates(MigratorRightParentB);
      }
   }

   public void CbxMigrator_CheckedChanged_ChangeParent(object sender, EventArgs e)
   {
      CheckBox   cbx          = sender as CheckBox;
      VvHamper   migratorHamp = (VvHamper)cbx.Tag;
      VvHamper   parentHamp   = (VvHamper)cbx.Parent;
      VvMigrator migrator     = MigratorList.Single(mig => mig.MigName == migratorHamp.Name);

      if(cbx.Checked) // Hamper se seli sa drugog TabPage-a na prvi 
      {

         if(IsParent_A(migrator)) migratorHamp.Parent = MigratorLeftParentA;
         else                     migratorHamp.Parent = MigratorLeftParentB;

         parentHamp.VvColWdt[0] = ZXC.Q7un;
         parentHamp.Width = ZXC.Q7un;
         string text            = migratorHamp.Name.TrimEnd(':').Substring(1);
         cbx.Text               = text;

         if(cbx.Name == "AOsobaA:") cbx.Text = "Komerc.:"; //20.11.2013. 
      }
      else // Hamper se vraca sa prvog TabPage-a na drugi 
      {
         
         if(IsParent_A(migrator)) migratorHamp.Parent = MigratorRightParentA;
         else                     migratorHamp.Parent = MigratorRightParentB;

         migratorHamp.Location = migratorHamp.VvInitialHamperLocation;

         parentHamp.VvColWdt[0] = ZXC.QUN;
         parentHamp.Width       = ZXC.QUN;
         cbx.Text = "";
      }

      xA = yA = xB = yB = 0;
      restXA = MigratorLeftParentA.Width;
      
      ReorganizeMigratorHampersLocations(MigratorRightParentA);

      if(MigratorLeftParentB != null)
      {
         restXB = MigratorLeftParentB.Width;
         ReorganizeMigratorHampersLocations(MigratorRightParentB);
      }

      if(MigratorLeftParentA.Controls.Count == 0)
      {
         if(ControlUnderMigLeftParentA != null) ControlUnderMigLeftParentA.Location = new Point(ZXC.Qun4, MigratorLeftParentA.Top + ZXC.QUN);
         MigratorLeftParentA.SendToBack();
      }
      if(MigratorLeftParentB != null && MigratorLeftParentB.Controls.Count == 0)
      {
         if(ControlUnderMigLeftParentB != null) ControlUnderMigLeftParentB.Location = new Point(ZXC.Qun4, MigratorLeftParentB.Top);
      }
   }

   private void ReorganizeMigratorHampersLocations(Control control)
   {
      //foreach(Control control in panel_Migrators.Controls)
      //{
      //   if(control is VvHamper && ((VvHamper)control).VvIsMigrateable == true)
      //   {
      //      CalcMigratorLocation((VvHamper)control);
      //   }
      //}
      if(control is CheckBox && control.Tag is VvHamper && ((CheckBox)control).Checked == true)
      {
         VvMigrator migrator = MigratorList.Single(mig => mig.MigName == ((VvHamper)(control.Tag)).Name);

         if(IsParent_A(migrator)) CalcMigratorLocation((VvHamper)control.Tag, xA, ref yA, ref restXA, MigratorLeftParentA);
         if(IsParent_B(migrator)) CalcMigratorLocation((VvHamper)control.Tag, xB, ref yB, ref restXB, MigratorLeftParentB);
      }

      foreach(Control childControl in control.Controls)
      {
         ReorganizeMigratorHampersLocations(childControl);
      }
   }

   private void CalcMigratorLocation(VvHamper migratorHamp, int x, ref int y, ref int restX, Control parentLeft)
   {
      int big = 0;

      if(restX + ZXC.Qun4 > migratorHamp.Width)
      {
         if(parentLeft.Controls.Count <= 1)
         {
            x = y = 0;
            restX = parentLeft.Width;
            big   = 0;
         }
         else
         {
            big = CalcBigHamper(parentLeft);
         }
         x      = parentLeft.Width - restX;
         restX -= (migratorHamp.Width - ZXC.Qun4);

      }
      else
      {
         x      = 0;
         restX  = parentLeft.Width;
         restX -= (migratorHamp.Width - ZXC.Qun4);

         big = CalcBigHamper(parentLeft);

         if(y == 0) y = big;

         if(parentLeft == MigratorLeftParentA) y += (migratorHamp.Height/* - ZXC.Qun8*/);
         else                                  y += (migratorHamp.Height - ZXC.Qun8);
      }
      
      migratorHamp.Location = new Point(x, y);

      if(parentLeft == MigratorLeftParentA) // 14.10. ovo je da bude fiksna velicina B radi boje back ...
      {
         if(y == 0) parentLeft.Height = y + migratorHamp.Height + big;
         else       parentLeft.Height = y + migratorHamp.Height;
      }

      if(parentLeft == MigratorLeftParentA)
      {
         if(ControlUnderMigLeftParentA != null) ControlUnderMigLeftParentA.Location = new Point(ZXC.Qun4, parentLeft.Bottom + ZXC.QunMrgn);
         
      }
      else
      {
         if(ControlUnderMigLeftParentB != null) ControlUnderMigLeftParentB.Location = new Point(0, parentLeft.Bottom + ZXC.QunMrgn);
      }
   }

   private int CalcBigHamper(Control parentLeft)
   {
      int bigHamp = 0;

      foreach(VvHamper hamper in parentLeft.Controls)
      {
         if(hamper.VvNumOfRows > 1) bigHamp = (hamper.VvNumOfRows - 1) * (hamper.VvRowHgt[0] + hamper.VvSpcBefRow[0] + ZXC.Qun8);
      }
      return bigHamp;
   }

   public VvHamper CreateCbxhamper4Migrators(VvHamper migratorHamper, int _nextX, int _nextY, int tabNum, bool hasParent)
   {
      VvHamper hamper;

      if(hasParent)
      {
         hamper = new VvHamper(1, 1, "", TheTabControl.TabPages[tabNum], false, _nextX, _nextY, 0);
      }
      else
      {
         hamper = new VvHamper(1, 1, "", null, false);
      }
       
      
      hamper.VvColWdt      = new int[] { ZXC.Q7un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      CheckBox cbx = hamper.CreateVvCheckBox_OLD(0, 0, null, ""/*migratorHamper.Name*/, System.Windows.Forms.RightToLeft.No);
      cbx.Name     = migratorHamper.Name;
      cbx.Tag      = migratorHamper;
      cbx.TabStop  = false;

      cbx.CheckedChanged += new EventHandler(CbxMigrator_CheckedChanged_GetMigratorsStates);
      cbx.CheckedChanged += new EventHandler(CbxMigrator_CheckedChanged_ChangeParent);

      if(cbx.Checked) hamper.VvColWdt = new int[] { ZXC.Q7un };
      else            hamper.VvColWdt = new int[] { ZXC.QUN };

      Control parent;
      if(migratorHamper.Name.StartsWith("A")) parent = MigratorRightParentA;
      else                                    parent = MigratorRightParentB;

      if(hasParent) hamper.Parent = parent;
      else          hamper.Parent = null;

      return hamper;
   }

   #endregion VvMigrator

   //protected static VvFindDialog CreateVvFindDialogAndUC(XSqlConnection dbConnection, VvRecListUC vvRecListUC)
   //{
   //   return new VvFindDialog(dbConnection, vvRecListUC);
   //}

   #region Create_DummyForDefaultFocus

   public void Create_DummyForDefaultFocus()
   {
      tbx_DummyForDefaultFocus = new TextBox();
      tbx_DummyForDefaultFocus.Parent = this;
      tbx_DummyForDefaultFocus.Name = "tbx_DummyForDefaultFocus";
      tbx_DummyForDefaultFocus.Location = Point.Empty;
      tbx_DummyForDefaultFocus.Size = Size.Empty;
      tbx_DummyForDefaultFocus.TabStop = false;
   }

   #endregion Create_DummyForDefaultFocus

   #region GetDB_ColumnSize()

   /// <summary>
   /// Get from DATABASE column size for DOCUMENT record (npr Nalog)
   /// </summary>
   /// <param name="columnIndex"></param>
   /// <returns></returns>
   protected int GetDB_ColumnSize(int columnIndex)
   {
      //return (int)TheVvDao.TheSchemaTable.Rows[columnIndex]["ColumnSize"];
      return TheVvDao.GetSchemaColumnSize(columnIndex);
   }

   protected int GetDB_ColSize_namedDao(VvDaoBase _theVvDao, int columnIndex)
   {
      //return (int)TheVvDao.TheSchemaTable.Rows[columnIndex]["ColumnSize"];
      return _theVvDao.GetSchemaColumnSize(columnIndex);
   }

   #endregion GetDB_ColumnSize()

   #region PutMetaFileds_FormatMetaFields_PutIdentityFields

   protected void PutMetaFileds(string addUID, DateTime addTS, string modUID, DateTime modTS, uint recID, uint lanSrvID, uint lanRecID)
   {
#if (WWWDEMO)
      if(addUID.NotEmpty()) addUID = "userA_01";
      if(modUID.NotEmpty()) modUID = "userM_02";
#endif
      if(addTS == modTS) TheVvTabPage.Fld_ModMetaData = "";
      else               TheVvTabPage.Fld_ModMetaData = FormatMetaFields(modUID, modTS);

      TheVvTabPage.Fld_AddMetaData = FormatMetaFields(addUID, addTS);

      // SkyNews 
    //toolTip.SetToolTip(TheVvTabPage.tbx_addMetaData, "RecID: " + recID.ToString("000000"));
    //toolTip.SetToolTip(TheVvTabPage.tbx_modMetaData, "RecID: " + recID.ToString("000000"));
      toolTip.SetToolTip(TheVvTabPage.tbx_addMetaData, "RecID: " + recID.ToString("000000") + " LanSrvID: " + lanSrvID.ToString("00") + " LanRecID: " + lanRecID.ToString("000000"));
      toolTip.SetToolTip(TheVvTabPage.tbx_modMetaData, "RecID: " + recID.ToString("000000") + " LanSrvID: " + lanSrvID.ToString("00") + " LanRecID: " + lanRecID.ToString("000000"));
   }

   private string FormatMetaFields(string uid, DateTime ts)
   {
//#if (WWWDEMO)
//      return "[" + /*uid*/"user_1" + " / " + ts + "]";
//#endif
      return "[" + uid + " / " + ts + "]";
   }

   protected void PutIdentityFields(string col1, string col2, string col3, string col4)//, string prjkt)
   {
      TheVvTabPage.Fld_Col1 = col1;
      TheVvTabPage.Fld_Col2 = col2;
      TheVvTabPage.Fld_Col3 = col3;
      TheVvTabPage.Fld_Col4 = col4;
      //TheVvTabPage.Fld_PrjktNaziv = prjkt;

   }
   protected void PutIdentityFields_5Col(string col1, string col2, string col3, string col4, string col5)
   {
      TheVvTabPage.Fld_Col1 = col1;
      TheVvTabPage.Fld_Col2 = col2;
      TheVvTabPage.Fld_Col3 = col3;
      TheVvTabPage.Fld_Col4 = col4;
      TheVvTabPage.Fld_Col5 = col5;
      //TheVvTabPage.Fld_PrjktNaziv = prjkt;

   }
   protected void PutIdentityFields_7Col(string col1, string col2, string col3, string col4, string col5, string col6, string col7)
   {
      TheVvTabPage.Fld_Col1 = col1;
      TheVvTabPage.Fld_Col2 = col2;
      TheVvTabPage.Fld_Col3 = col3;
      TheVvTabPage.Fld_Col4 = col4;
      TheVvTabPage.Fld_Col5 = col5;
      TheVvTabPage.Fld_Col6 = col6;
      TheVvTabPage.Fld_Col7 = col7;
   }

   #endregion PutMetaFileds_FormatMetaFields_PutIdentityFields

   #region TheTabControl

   public Crownwood.DotNetMagic.Controls.TabControl TheTabControl { get; set; }

   public void CreateTheTabControl()
   {
      TheTabControl                  = new Crownwood.DotNetMagic.Controls.TabControl();
      TheTabControl.Parent           = this;
      TheTabControl.Dock             = DockStyle.Fill;
      TheTabControl.Appearance       = VisualAppearance.MultiDocument;
      TheTabControl.ShowArrows       = false;
      TheTabControl.ShowClose        = false;
      TheTabControl.HotTrack         = true;
      TheTabControl.Style            = ZXC.vvColors.vvform_VisualStyle;
      TheTabControl.OfficeStyle      = ZXC.vvColors.tabControl_OfficeStyle;
      TheTabControl.MediaPlayerStyle = ZXC.vvColors.tabControl_MediaPlayerStyle;

      TheTabControl.SelectionChanged += new SelectTabHandler(DecideIfShouldLoad_TransDGV);
      TheTabControl.SelectionChanged += new SelectTabHandler(DecideIfShouldLoad_VvReport);
      TheTabControl.SelectionChanged += new SelectTabHandler(SetVvMenuEnabledOrDisabled_HasFilter_FilterVisibiliti);
      TheTabControl.SelectionChanged += new SelectTabHandler(Changed_SubModulSet);
      TheTabControl.SelectionChanged += new SelectTabHandler(Changed_HamperPrintDocVisibiliti);
   }

   public virtual void DecideIfShouldLoad_TransDGV(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   {
      // mejbi bu overriden, mejbi not 
      // npr DUC-evi ju ne trebaju jer nemaju innerTabPage 'Transakcije' 
   }

   public virtual void LoadRecordList_AND_PutTransDgvFields()
   {
      // mejbi bu overriden, mejbi not 
      // npr DUC-evi ju ne trebaju jer nemaju innerTabPage 'Transakcije' 
   }

   private void SetVvMenuEnabledOrDisabled_HasFilter_FilterVisibiliti(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   {
      if(TheVvTabPage != null && TheVvTabPage.IsArhivaTabPage) return;

      if(((VvInnerTabPage)newPage).TheInnerTabPageKindEnum == ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage ||
         ((VvInnerTabPage)newPage).TheInnerTabPageKindEnum == ZXC.VvInnerTabPageKindEnum.ReportViewer_TabPage)
      {
         ZXC.TheVvForm.SetVvMenuEnabledOrDisabled_FilterTabPageIsOpened(ZXC.TheVvForm.TheVvTabPage.WriteMode, false);

         if(ZXC.TheVvForm.TheVvTabPage.IsFiterVisible)
         {
            ThePanelForFilterUC_PrintTemplateUC.Visible = true;
            CalcThePanelForFilterUCOnRecordUCWidth();
         }
         else ThePanelForFilterUC_PrintTemplateUC.Visible = false;
      }
      else
      {
         if(Form.ActiveForm is VvForm) // zato da tsBtn na ts_record-u ostanu disebliran/enablitrani kak su bili prije pozivanja dlg
         {
            ZXC.TheVvForm.SetVvMenuEnabledOrDisabled_RegardingWriteMode(ZXC.TheVvForm.TheVvTabPage.WriteMode);
            // TODO: Tamara, cemu sluzi ovaj SetDirtyFlag? 
            ZXC.TheVvForm.SetDirtyFlag("SetVvMenuEnabledOrDisabled_HasFilter_FilterVisibiliti");
         }

         ThePanelForFilterUC_PrintTemplateUC.Visible = false;
      }
   }

   private void Changed_SubModulSet(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   {
      Point xy = ZXC.TheVvForm.TheVvTabPage.SubModul_xy;

      if(((VvInnerTabPage)newPage).TheInnerTabPageKindEnum == ZXC.VvInnerTabPageKindEnum.ReportViewer_TabPage)
      {
         ZXC.TheVvForm.ts_Report.Parent = ZXC.TheVvForm.tsPanel_SubModul;
         ZXC.TheVvForm.VisibiltiOfTsReportAndTsSubModulSet(xy, true);
         ZXC.TheVvForm.ToolStripReportVisible_tsBtnOnTsReportVisible(false, ZXC.TheVvForm.TheVvTabPage.ReportMode, false);
      }
      else
      {
         if(Form.ActiveForm is VvForm) // yato da ostane isti ts_subModulSet koji je bio prije otvaranja dlg
            ZXC.TheVvForm.VisibiltiOfTsReportAndTsSubModulSet(xy, false);
      }
   }


   public Crownwood.DotNetMagic.Controls.TabPage CreateVvInnerTabPages(string title, string name, ZXC.VvInnerTabPageKindEnum _vvInnerTabPageKindEnum)
   {
      VvInnerTabPage vvInerTapPage = new VvInnerTabPage(title, name, _vvInnerTabPageKindEnum);
      return vvInerTapPage;
   }

   public void Changed_HamperPrintDocVisibiliti(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   {
      if(TheVvTabPage != null && TheVvTabPage.IsArhivaTabPage) return;

      if(((VvInnerTabPage)newPage).TheInnerTabPageKindEnum == ZXC.VvInnerTabPageKindEnum.ReportViewer_TabPage)
      {
         if((this.VirtualFilterUC) != null) (this.VirtualFilterUC).hamperPrintDoc.Visible = true;
      }
      else
      {
         if((this.VirtualFilterUC) != null) (this.VirtualFilterUC).hamperPrintDoc.Visible = false;
      }
   }


   #endregion TheTabControl

   #region ThePanelForFilterUC_PrintTemplateUC

   public Panel ThePanelForFilterUC_PrintTemplateUC { get; set; }

   public void CreatePanelForFilter()
   {
      ThePanelForFilterUC_PrintTemplateUC            = new Panel();
      ThePanelForFilterUC_PrintTemplateUC.Parent     = this;
      ThePanelForFilterUC_PrintTemplateUC.Dock       = ZXC.TheVvForm.modulPanel.Dock;
      
      ThePanelForFilterUC_PrintTemplateUC.BackColor  = ZXC.vvColors.userControl_BackColor;
      ThePanelForFilterUC_PrintTemplateUC.Name       = "FilterPanelInner";
      ThePanelForFilterUC_PrintTemplateUC.AutoScroll = true;
      ThePanelForFilterUC_PrintTemplateUC.Visible    = false;
   }


   public void CalcThePanelForFilterUCOnRecordUCWidth()
   {
      if(ThePanelForFilterUC_PrintTemplateUC.VerticalScroll.Visible)
         ThePanelForFilterUC_PrintTemplateUC.Width = ThePanelForFilterUC_PrintTemplateUC.Width + ZXC.QUN;
      else
         ThePanelForFilterUC_PrintTemplateUC.Width = ThePanelForFilterUC_PrintTemplateUC.Width;
   }

   #endregion ThePanelForFilterUC_PrintTemplateUC

   #region PrintRecord

   #region  TheCrystalViewer

   public CrystalReportViewer TheReportViewer { get; set; }

   public void CreateTheCrystalViewer()
   {
      TheReportViewer                  = new CrystalReportViewer();
      TheReportViewer.Dock             = DockStyle.Fill;
     // TheReportViewer.DisplayGroupTree = false;
      TheReportViewer.ToolPanelView    = ToolPanelViewType.None;
      TheReportViewer.DisplayToolbar   = false;
      TheReportViewer.DisplayStatusBar = true;

      //if(this is FakturDUC)
      //{ 
      //   Splitter spliter  = new Splitter();
      //   spliter.Parent    = this;
      //   spliter.Dock      = DockStyle.Bottom;
      //   spliter.MinExtra  = ZXC.Q10un;  
      //   spliter.MinSize   = ZXC.Q3un; 
      //   spliter.Width     = ZXC.Qun10;
      //   spliter.BackColor = ZXC.vvColors.modulPanel_BackColor;

      //}
   }

   #endregion  TheCrystalViewer

   #region IVvPrintableUC Members

   public CrystalReportViewer VirtualReportViewer
   {
      get
      {
         return this.TheReportViewer;
      }
      set
      {
         this.TheReportViewer = value;
      }
   }

   #endregion

   public bool recordReportLoaded;

   //27.12.2010:
   public VvReport TheVvReport { get; set; }

   //public virtual VvReport VirtualReport
   //{
   //   get { return null; }
   //   //set { }
   //}

   public virtual string VirtualReportName
   {
      get { return null; }
      set { }
   }

   public virtual VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      return null;
   }

   public virtual VvFilterUC VirtualFilterUC
   {
      get { return null; }
      //set { }
   }

   public virtual void CreateRptFilterAndRptFilterUC()
   {
   }

   public virtual void SetFilterRecordDependentDefaults()
   {

   }

   // Tu dodjemo na 2 nacina:                          
   // 1. PutFields (overriden-ani) od pojedinih UC-ova 
   // 2. TheTabControl.SelectionChanged                
   public void DecideIfShouldLoad_VvReport(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   {
      ZXC.VvInnerTabPageKindEnum innerTabPageKind = ((VvInnerTabPage)TheTabControl.SelectedTab).TheInnerTabPageKindEnum;

      if(recordReportLoaded == false &&
         innerTabPageKind == ZXC.VvInnerTabPageKindEnum.ReportViewer_TabPage)
      {
         ShowRecordReportPreview_Or_QuickPrintRecord(true);
      }

   }

   // Tu dodjemo na 4 nacina.            
   // 1. QuickPrintRecord_Button_OnClick,
   // 2. PrintPreviewMenu_Button_OnClick 
   // 3. ButtonGO_OnClick                
   // 4. DecideIfShouldLoad_VvReport     
   public int ShowRecordReportPreview_Or_QuickPrintRecord(bool isPreview)
   {
      int norr = -1;
      //VvReport vvReport;

      Cursor.Current = Cursors.WaitCursor;
                      // dodala 1.7.2011. zbog RasterDuc-aa koji nema printanje samog sebe
      if(isPreview && VirtualFilterUC !=null) VirtualFilterUC.GetFilterFields();

      SetReportAndReportName_ForThisInnerTabPage(TheTabControl.SelectedTab.Name);

      //21.10.2010: 
      // Googlaj "Load Report Failed"
      // HKEY_LOCAL_MACHINE\SOFTWARE\Business Objects\10.5\Report Application Server\InProcServer - PrintJobnLimit je 75, 
      // pa prije dok nisi ovako na pocetku cistio stare unmanaged resources skakao bi ti Exception nakon 75-og uzastopnog pozivanja Report-a 
      //===================================================
      if(TheVvReport != null)
      {
         TheVvReport.VirtualReportDocument.Close();
      }
      //===================================================

      TheVvReport = CreateVvRecordReport(VirtualReportName, VirtualRptFilter);

      if(TheVvReport == null) return -1;  //poziv reporta koji !IsFilterWellFormed() sa notReportUc-a vraca null

      if(VirtualRptFilter.PrnTemplatesAreExtern)
      {
         if(TheVvReport.LoadReportFromRptFile() == false) // error occurred! 
         {
            recordReportLoaded = true; // supress further shit.
            return norr;
         }
      }

      VirtualRptFilter.IsPopulatingTransDGV = false;

      VirtualFilterUC.AddFilterMemberz(VirtualRptFilter, /*vvReport*/ null);

      norr = TheVvReport.FillDataSet_And_SetDataSource(null);

      if(isPreview) // ShowReportPreview
      {
         TheReportViewer.ReportSource = TheVvReport.VirtualReportDocument;
         //TheReportViewer.Zoom(1);
         if(ZXC.TheVvForm.VvPref.reportPrefs.ZoomFactor.IsZero()) ZXC.TheVvForm.VvPref.reportPrefs.ZoomFactor = 1;

         TheReportViewer.Zoom(/*VirtualRptFilter.PrintZoom*/ ZXC.TheVvForm.VvPref.reportPrefs.ZoomFactor);


      }
      else // QuickPrintRecord 
      {
         TheVvReport.VirtualReportDocument.PrintToPrinter(1, false, 0, 0);

         //06.10.2015: 
         if(ZXC.RRD.Dsc_IsPrintOTSafterIRA && TheVvReport is RptR_IRA && 
            (((Faktur)TheVvTabPage.TheVvDataRecord).TT == Faktur.TT_IFA ||
             ((Faktur)TheVvTabPage.TheVvDataRecord).TT == Faktur.TT_IRA)) TheVvTabPage.TheVvForm.AfterPrintIRA_PrintKupdobsOTS((Faktur)TheVvTabPage.TheVvDataRecord);

         // 08.07.2019: 
         if(this is FakturExtDUC && ZXC.RRD.Dsc_IsPamtiPrintDate) // Faktur RWTREC: Datum Zadnjeg Printa 
         {
            FakturExtDUC theDUC     = (this as FakturExtDUC);
            Faktur       faktur_rec = theDUC.faktur_rec;

            theDUC.TheVvTabPage.TheVvForm.BeginEdit(faktur_rec);

            faktur_rec.DateX = VvSQL.GetServer_DateTime_Now(TheDbConnection);

            bool rwtOK = faktur_rec.VvDao.RWTREC(TheDbConnection, faktur_rec, false, true, false, false, true/* !!! */ ); // zbog implicitne synchronizacije ovaj rwtrec ne zelimo u Insert_LAN_LogEntry 

            theDUC.TheVvTabPage.TheVvForm.EndEdit(faktur_rec);

          //if(rwtOK) WhenRecordInDBHasChangedAction(); // RRDREC 
            if(rwtOK) theDUC.Fld_DateX = faktur_rec.DateX;

         } // Faktur RWTREC: Datum Zadnjeg Printa 

      }

      recordReportLoaded = true;

      Cursor.Current = Cursors.Default;

      return norr;
   }

   // znaci defaultna implementacije je da metoda ne radi nista 
   // A po potrebi ga neki RecListUC - prejebe 
   protected virtual void SetReportAndReportName_ForThisInnerTabPage(string selectedTabName)
   {
   }


   #endregion PrintRecord

   public virtual bool IsSomeCrutialFieldIrregularyChanged()
   {
      return false;
   }

   // user & person 
   protected void CheckOIB_Field(object sender, System.ComponentModel.CancelEventArgs e)
   {
      if(sender is VvTextBox == false) throw new Exception("CheckOIB_Field: sender nije VvTextBox.");

      VvTextBox vvtb = sender as VvTextBox;

      if(ZXC.IsBadOib(vvtb.Text, true))
      {
         ZXC.RaiseErrorProvider((Control)sender, "Neispravan OIB!");
         e.Cancel = true;
      }
   }

   // kupdob
   protected void CheckOIB_Field_Kupdob(object sender, System.ComponentModel.CancelEventArgs e)
   {
      if(sender is VvTextBox == false) throw new Exception("CheckOIB_Field: sender nije VvTextBox.");

      VvTextBox vvtb = sender as VvTextBox;

      if(this is KupdobUC == false) throw new Exception("CheckOIB_Field_Kupdob: this nije KupdobUC.");

      KupdobUC kupdobUC = this as KupdobUC;

      if(kupdobUC.kupdob_rec.IsHRVATSKA == false) return;

      if(ZXC.IsBadOib(vvtb.Text, true))
      {
         ZXC.RaiseErrorProvider((Control)sender, "Neispravan OIB!");
         e.Cancel = true;
      }
   }

}

public abstract  class VvRecLstUC : VvUserControl, IVvRecordAssignableUC
{
   #region Common Controlz

   public /*protected*/ RadioButton rbt_Descending, rbt_Ascending;
   protected Button button_prevPage, button_nextPage, btn_PlusMinusUtil; 
   public    Button btn_PlusMinusFilter;
   public    Button button_GO, button_Reset;
   protected CheckBox checkBox_shouldLimit;
   public    VvTextBox tbx_limitNum, tbx_filColumn;
   protected Label labelMin, labelMax, labelPositionInfo;
   protected HScrollBar scrollBarNorrWoLimit;
   public    GroupBox grBoxLimitiraj;
   public    VvHamper hampListaRastePada, hampSpecifikum, hampIzlistaj, hampButtons, hampFilter, hampOpenFilter, hampOpenUtil, hampUtil, hampFillColumn;
   protected int grid_Width;
   protected Label lblCrta;
   protected int nextX = ZXC.QunMrgn, nextY = 0, razmakHamp = ZXC.Qun10, nextY4grBox;
   
   protected Button btnUtil_del, btnUtil_copyIn, btnUtil_copyOut;

   // news: 22.10.2010 DataGridViewFind ---> VvDataGridViewFind
   private VvDataGridViewFind dataGridView;
   public VvDataGridViewFind TheGrid
   {
      get { return dataGridView; }
   }

   private ZXC.VvSubModulEnum masterSubModulEnum;
   public ZXC.VvSubModulEnum MasterSubModulEnum
   {
      get { return masterSubModulEnum; }
      set { masterSubModulEnum = value; }
   }

   #endregion Common Controlz

   #region Constructor

   public VvRecLstUC(Control _parent)
   {
      SuspendLayout();

      this.Parent = _parent;

      this.ToolTipBiloGdjeUnazivu = new ToolTip();

      //if(_parent is Panel) InitializeVvUserControl(this.Parent);
      //else                 this.ParentControlKind = ZXC.ParentControlKind.VvFindDialog;

      if(_parent is Panel)
      {
         if(_parent.Name == "FilterPanelBottom") this.ParentControlKind = ZXC.ParentControlKind.VvReportUC;
         else                                    InitializeVvUserControl(this.Parent);
      }
      else
      {
         this.ParentControlKind = ZXC.ParentControlKind.VvFindDialog;
      }

      CreateHamperRastePada();
      InitializeFindFormSpecifics();
      CreateHamperSpecifikum();
      dataGridView = CreateDataGridView_ReadOnly(this, "RecListGrid");

      CreateDataGridViewColumn();
      CreateHamperIzlistaj();
      CreateHamperFilter();
      CreateHamperFillCol();

      if(hampFilter == null) nextY4grBox = hampListaRastePada.Bottom;
      else                   nextY4grBox = hampOpenFilter.Bottom;

      if(ZXC.CURR_userName == ZXC.vvDB_systemSuperUserName  ||
         ZXC.CURR_userName == ZXC.vvDB_programSuperUserName || ZXC.CURR_user_rec.IsSuper )
      {
         CreateHamperUtil();
         nextY4grBox = hampOpenUtil.Bottom;
      }
      //CreateHamperOpenUtil(); 

      // CreateGroupBoxLimit(GetNextY_ForGroupBoxLimit());
      CreateGroupBoxLimit(nextY4grBox);

      InitializeDataGridView();
      CalculateLocationAndSize();

      this.Resize += new EventHandler(VvRecListUC_Resize);

      InitializeDataAdapter();

      GroupBoxLimit_CalclimitNum();

      //InitializeStartValue(); // duplicirano sa VvFindDialog_Load(); 

      TheFillColumn = TheGrid.Columns.Cast<DataGridViewColumn>().OfType<DataGridViewTextBoxColumn>().SingleOrDefault(dgvCol => dgvCol.AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill);
      if(btn_PlusMinusFilter != null)
      {
         btn_PlusMinusFilter.PerformClick();
         btn_PlusMinusFilter.PerformClick();
      }
      
      ResumeLayout();

      NeedsAutoRefresh = false;

      #region Auto Refresh Via Timer Tick

      //NeedsAutoRefresh = true; // todo odremarkiraj ovo kada slozis uvjet 

      if(NeedsAutoRefresh) 
      {
         int seconds = 4; // todo 

         AutoRefreshTimer = new System.Windows.Forms.Timer();
         AutoRefreshTimer.Interval = seconds * 1000; // milliseconds 
         AutoRefreshTimer.Tick += new System.EventHandler(autoRefreshTimer_Tick);
         AutoRefreshTimer.Start();
      }

      #endregion Auto Refresh Via Timer Tick
   }

   private void autoRefreshTimer_Tick(object sender, EventArgs e)
   {
    //button_Go_Prev_Next_Action(this, EventArgs.Empty);
      button_GO_Click           (this, EventArgs.Empty);
   }
   private void VvRecListUC_Resize(object sender, EventArgs e)
   {
      CalcFillColumnHamperSizeAndLeftanchor();
   }

   private void CalcFillColumnHamperSizeAndLeftanchor()
   {
      hampFillColumn.VvColWdt[0] = TheGrid.Width;
      hampFillColumn.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
   }

   #endregion Constructor

   #region Common Propertiz

   public abstract VvDataRecord VirtualDataRecord
   {
      get;
      set;
   }

   public abstract VvDaoBase TheVvDao
   {
      get;
   }

   #region Some Fieldz

   private BindingSource bindingSource;
   protected int pageCount;
   public VvSQL.RecordSorter recordSorter;
   public /*protected*/ VvSQL.OrderDirectEnum asc_or_desc = VvSQL.OrderDirectEnum.ASC;
   private bool norrDone;
   private int theNorrWoLimitStorage;

   #endregion Some Fieldz

   public int SelectedRecID
   {
      get
      {
         if(TheGrid.CurrentRow != null)
            return int.Parse(TheGrid.CurrentRow.Cells["recID"].Value.ToString());
         else
            return -1;
      }
   }

   protected abstract DataSet VirtualUntypedDataSet
   {
      get;
   }

   /// <summary>
   /// get { return VirtualUntypedDataSet.Tables[0]; }
   /// </summary>
   protected DataTable TheDataTable
   {
      get { return VirtualUntypedDataSet.Tables[0]; }
   }

   protected abstract object[] From_IndexSegmentValues
   {
      get;
   }

   private XSqlDataAdapter dAdapter_my;
   protected XSqlDataAdapter TheDataAdapter
   {
      get { return dAdapter_my; }
      set { dAdapter_my = value; }
   }

   private XSqlCommand selectCommand;
   public XSqlCommand TheSelectCommand
   {
      get { return selectCommand; }
      set { selectCommand = value; }
   }

   protected int NorrWoLimit
   {
      get
      {
         if(ZXC.IsSvDUH_ZAHonly) return ArtiklSifrar.Count;

         if(norrDone == false)
         {
            theNorrWoLimitStorage = VvSQL.CountGTEREC_Rows(TheDbConnection, recordSorter, From_IndexSegmentValues, TheFilterMembers, asc_or_desc, this);
            norrDone = true;
         }
         return theNorrWoLimitStorage;
      }
   }

   /*private*/public int SelectedRowIndex
   {
      get 
      {
         if(TheGrid.CurrentRow == null) return -1;
         
         return TheGrid.CurrentRow.Index; 
      }

      //get { return dataGrid1.CurrentRowIndex; }
      //set
      //{
      //   dataGrid1.CurrentRowIndex = value;
      //   previousSelectedRowIndex = value;
      //}
   }

   protected int PageSize
   {
      get
      {
         return (Int32.Parse(tbx_limitNum.Text));
      }
   }

   protected int PageSize4Command
   {
      get
      {
         if(PaggingTurnedOn == true) return (Int32.Parse(tbx_limitNum.Text));
         else return 0;
      }
   }

   protected int Offset
   {
      get
      {
         //return (pageCount * PageSize) ;
         return scrollBarNorrWoLimit.Value;
      }
      set
      {
         if(value >= scrollBarNorrWoLimit.Minimum)
            scrollBarNorrWoLimit.Value = value;
         else
            scrollBarNorrWoLimit.Value = scrollBarNorrWoLimit.Minimum;
      }
   }
   protected bool PaggingTurnedOn
   {
      get { return checkBox_shouldLimit.Checked; }
      set { checkBox_shouldLimit.Checked = value; }
   }

   private System.Collections.Generic.List<VvSqlFilterMember> filterMembers;
   public System.Collections.Generic.List<VvSqlFilterMember> TheFilterMembers
   {
      get { return filterMembers; }
      set { filterMembers = value; }
   }

   protected bool IsArhivaTabPage
   {
      get { return ZXC.TheVvForm.TheVvTabPage.IsArhivaTabPage; }
   }

   protected ToolTip ToolTipBiloGdjeUnazivu { get; set; }
   protected const string ToolTipTextBiloGdjeUnazivu = "Tekst ne mora biti na pocetku.\nTrazi na bilo kojoj poziciji.";

   public bool Supress_ImaLiIjedan_StartField_Neprazan_Action { get; set; }

   public DataGridViewColumn TheFillColumn { get; set; }

   public bool NeedsAutoRefresh { get; set; }

   private System.Windows.Forms.Timer AutoRefreshTimer;

   #endregion Common Propertiz

   #region Common Eventz

   //  public Control ControlForInitialFocus;

   private void radioButtonAscending_Click(object sender, System.EventArgs e)
   {
      this.asc_or_desc = VvSQL.OrderDirectEnum.ASC;
   }

   private void radioButtonDescending_Click(object sender, System.EventArgs e)
   {
      this.asc_or_desc = VvSQL.OrderDirectEnum.DESC;
   }

   private void button_GO_Click(object sender, System.EventArgs e)
   {
      //if(Smo upravo stisnuli Enter u VvTextBoxu koji izaziva izlistavanje cim pocnes pisat po njemu. Mozda uspijes smisliti kasnije)
      //{
      //   TheGrid_DoubleClick(sender, e);
      //   return;
      //}

      Offset = 0;
      button_Go_Prev_Next_Action(sender, e);
      TheGridFocused();

      if(this.Parent is VvFindDialog) EnableDisable_button_Apply();
   }

   private void EnableDisable_button_Apply()
   {
      VvFindDialog theFindDialog = ((VvFindDialog)this.Parent);

      if((ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(this.MasterSubModulEnum)).xy == ZXC.TheVvForm.TheVvTabPage.SubModul_xy)
      {
         theFindDialog.button_Apply.Enabled = true;
      }
      else if(this is ArtiklListUC)
      {
         if(ZXC.TheVvForm.TheVvUC is FakturDUC || ZXC.TheVvForm.TheVvUC is PmvDUC || ZXC.TheVvForm.TheVvUC is PredNrdDUC)
         {
            if(ZXC.TheVvForm.TheVvUC is FakturDUC)
            {
               if(true/*08.04.2012 isOnGrid*/)
               {
                  theFindDialog.button_Apply.Enabled = true;

                  theFindDialog.ApplyEventHandler -= new EventHandler(this.ApplyEventHandlerIn_FakturDUC_ForOneOrManyArtikls);
                  theFindDialog.ApplyEventHandler += new EventHandler(this.ApplyEventHandlerIn_FakturDUC_ForOneOrManyArtikls);
               }
               //else
               //{
               //   theFindDialog.button_Apply.Enabled = false;
               //}
            }

            else if(ZXC.TheVvForm.TheVvUC is PmvDUC)
            {
               if(true/*08.04.2012 isOnGrid*/)
               {
                  theFindDialog.button_Apply.Enabled = true;

                  theFindDialog.ApplyEventHandler -= new EventHandler(this.ApplyEventHandlerIn_PmvDUC_ForOneOrManyArtikls);
                  theFindDialog.ApplyEventHandler += new EventHandler(this.ApplyEventHandlerIn_PmvDUC_ForOneOrManyArtikls);
               }
               //else
               //{
               //   theFindDialog.button_Apply.Enabled = false;
               //}
            }
            else if(ZXC.TheVvForm.TheVvUC is PredNrdDUC)
            {
               if(true/*08.04.2012 isOnGrid*/)
               {
                  theFindDialog.button_Apply.Enabled = true;

                  theFindDialog.ApplyEventHandler -= new EventHandler(this.ApplyEventHandlerIn_PredNrdDUC_ForOneOrManyArtikls);
                  theFindDialog.ApplyEventHandler += new EventHandler(this.ApplyEventHandlerIn_PredNrdDUC_ForOneOrManyArtikls);
               }
               //else
               //{
               //   theFindDialog.button_Apply.Enabled = false;
               //}
            }

         }
         else
         {
            theFindDialog.button_Apply.Enabled = false;
         }
      }
      else
      {
         theFindDialog.button_Apply.Enabled = false;
      }
   }

   private void ApplyEventHandlerIn_FakturDUC_ForOneOrManyArtikls(object sender, EventArgs e)
   {
      VvFindDialog dlg = (VvFindDialog)sender;

      Artikl artikl_rec;
      
      FakturDUC theDUC = ZXC.TheVvForm.TheVvUC as FakturDUC;

      int rowIdx = theDUC.TheG.CurrentRow.Index;
      int artiklRecID;
      bool isFromArtiklName = theDUC.TheG.CurrentCell.ColumnIndex == theDUC.DgvCI.iT_artiklName;

      // 28.10.2019: 
    //foreach(DataGridViewRow row in TheGrid.SelectedRows)
      foreach(DataGridViewRow row in TheGrid.SelectedRows.Cast<DataGridViewRow>().ToList().OrderBy(r => r.Index))
      {
         artiklRecID = int.Parse(row.Cells["recID"].Value.ToString());
         artikl_rec = ArtiklSifrar.SingleOrDefault(artikl => artikl.RecID == artiklRecID);
         if(artikl_rec == null) continue;

         //theDUC.TheG.Rows.Insert(++rowIdx, 1);
         theDUC.TheG.Rows.Add();
         rowIdx = theDUC.TheG.RowCount - 2;

         theDUC.TheG.PutCell(theDUC.DgvCI.iT_artiklCD  , rowIdx, artikl_rec.ArtiklCD);
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_artiklName, rowIdx, artikl_rec.ArtiklName);

         theDUC.TheG.CurrentCell = theDUC.TheG[isFromArtiklName ? theDUC.DgvCI.iT_artiklName : theDUC.DgvCI.iT_artiklCD, rowIdx];
         theDUC.TheG.BeginEdit(false);
         theDUC.AnyArtiklTextBox_OnGrid_Leave(theDUC.TheG.EditingControl, e);
         theDUC.TheG.EndEdit();
         ZXC.TheVvForm.SetDirtyFlag(sender);

      }
   }

   private void ApplyEventHandlerIn_PmvDUC_ForOneOrManyArtikls(object sender, EventArgs e)
   {
      VvFindDialog dlg = (VvFindDialog)sender;

      Artikl artikl_rec;
      
      PmvDUC theDUC = ZXC.TheVvForm.TheVvUC as PmvDUC;

      int rowIdx = theDUC.TheG.CurrentRow.Index;
      int artiklRecID;
      bool isFromArtiklName = theDUC.TheG.CurrentCell.ColumnIndex == theDUC.DgvCI.iT_artiklName;

      foreach(DataGridViewRow row in TheGrid.SelectedRows)
      {
         artiklRecID = int.Parse(row.Cells["recID"].Value.ToString());
         artikl_rec = ArtiklSifrar.SingleOrDefault(artikl => artikl.RecID == artiklRecID);
         if(artikl_rec == null) continue;

         //theDUC.TheG.Rows.Insert(++rowIdx, 1);
         theDUC.TheG.Rows.Add();
         rowIdx = theDUC.TheG.RowCount - 2;

         theDUC.TheG.PutCell(theDUC.DgvCI.iT_artiklCD  , rowIdx, artikl_rec.ArtiklCD);
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_artiklName, rowIdx, artikl_rec.ArtiklName);

         theDUC.TheG.CurrentCell = theDUC.TheG[isFromArtiklName ? theDUC.DgvCI.iT_artiklName : theDUC.DgvCI.iT_artiklCD, rowIdx];
         theDUC.TheG.BeginEdit(false);
         theDUC.AnyArtiklTextBox_OnGrid_Leave(theDUC.TheG.EditingControl, e);
         theDUC.TheG.EndEdit();
         ZXC.TheVvForm.SetDirtyFlag(sender);

      }
   }

   private void ApplyEventHandlerIn_PredNrdDUC_ForOneOrManyArtikls(object sender, EventArgs e)
   {
      VvFindDialog dlg = (VvFindDialog)sender;

      Artikl artikl_rec;
      
      PredNrdDUC theDUC = ZXC.TheVvForm.TheVvUC as PredNrdDUC;

      int rowIdx = theDUC.TheG.CurrentRow.Index;
      int artiklRecID;
      bool isFromArtiklName = theDUC.TheG.CurrentCell.ColumnIndex == theDUC.DgvCI.iT_artiklName;

      foreach(DataGridViewRow row in TheGrid.SelectedRows)
      {
         artiklRecID = int.Parse(row.Cells["recID"].Value.ToString());
         artikl_rec = ArtiklSifrar.SingleOrDefault(artikl => artikl.RecID == artiklRecID);
         if(artikl_rec == null) continue;

         //theDUC.TheG.Rows.Insert(++rowIdx, 1);
         theDUC.TheG.Rows.Add();
         rowIdx = theDUC.TheG.RowCount - 2;

         theDUC.TheG.PutCell(theDUC.DgvCI.iT_artiklCD  , rowIdx, artikl_rec.ArtiklCD);
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_artiklName, rowIdx, artikl_rec.ArtiklName);

         theDUC.TheG.CurrentCell = theDUC.TheG[isFromArtiklName ? theDUC.DgvCI.iT_artiklName : theDUC.DgvCI.iT_artiklCD, rowIdx];
         theDUC.TheG.BeginEdit(false);
         theDUC.AnyArtiklTextBox_OnGrid_Leave(theDUC.TheG.EditingControl, e);
         theDUC.TheG.EndEdit();
         ZXC.TheVvForm.SetDirtyFlag(sender);

      }
   }



   private void button_nextPage_Click(object sender, System.EventArgs e)
   {
      if(Offset + PageSize < NorrWoLimit)
      {
         Offset += PageSize;
         button_Go_Prev_Next_Action(sender, e);
      }
   }
   private void button_prevPage_Click(object sender, System.EventArgs e)
   {
      if(Offset /*- PageSize*/ > 0)
      {
         Offset -= PageSize;
         button_Go_Prev_Next_Action(sender, e);
      }
   }

   private void checkBox_shouldLimit_Click(object sender, System.EventArgs e)
   {
      //int norr = (ds_kplan.kplan.Count);
      int norr = VirtualUntypedDataSet.Tables[0].Rows.Count;

      if((sender as CheckBox).Checked == true)
      {
         this.tbx_limitNum.Enabled =
         this.labelMin.Enabled =
         this.labelMax.Enabled =
         this.labelPositionInfo.Enabled =
         this.scrollBarNorrWoLimit.Enabled = true;

         if(norr > 0)
         {
            this.button_prevPage.Enabled = this.button_nextPage.Enabled = true;
         }
         else
         {
            this.button_prevPage.Enabled = this.button_nextPage.Enabled = false;
         }
      }
      else
      {
         this.tbx_limitNum.Enabled =
         this.labelMin.Enabled =
         this.labelMax.Enabled =
         this.labelPositionInfo.Enabled =
         this.scrollBarNorrWoLimit.Enabled = false;

         this.button_prevPage.Enabled = this.button_nextPage.Enabled = false;
      }
   }

   private void tbx_limitNum_TextChanged(object sender, System.EventArgs e)
   {
      TextBox tbx = sender as TextBox;
      button_prevPage.Text = "<<<   &-" + (tbx.Text);
      button_nextPage.Text = "&+" + (tbx.Text) + "   >>>";
   }

   private void scrollBarNorrWoLimit_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
   {
      pageCount = scrollBarNorrWoLimit.Value / PageSize;
      button_Go_Prev_Next_Action(sender, e);
      /*Console.WriteLine("Off: {3} Val: {0} min {1} - max {2}",
         scrollBarNorrWoLimit.Value.ToString(),
         scrollBarNorrWoLimit.Minimum.ToString(),
         scrollBarNorrWoLimit.Maximum.ToString(),
         Offset);*/
   }

   private void TheGrid_Click(object sender, EventArgs e)
   {
      if(SelectedRecID.IsZeroOrNegative() || this.Parent is VvFindDialog == false) return;

      if((ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(this.MasterSubModulEnum)).xy == ZXC.TheVvForm.TheVvTabPage.SubModul_xy)
      {
         ((VvFindDialog)this.Parent).button_Apply.PerformClick();
      }
   
      DisplayFullFillText(SelectedRowIndex);
   }

   public void TheGrid_DoubleClick(object sender, EventArgs e)
   {
      if(SelectedRecID > 0)
      {
         if(this.Parent is VvFindDialog)
         {
            if(sender == null) // znaci pozvan sa VvFindDialog.AddSifrarRecord_Click()
            {
               ((VvFindDialog)this.Parent).button_OK.Enabled = true;
            }

            ((VvFindDialog)this.Parent).button_OK.PerformClick();
         }
         else
         {
            Point xy = Point.Empty;
            uint? recIDforOpenNewRecordTabPage = null;

            if(this is FakturListUC)
            {
               string tt;
               string ttZadani = ((FakturListUC)this).Fld_FilterTT;
               
               if(ttZadani.NotEmpty())  tt = ttZadani;
               else                     tt = this.dataGridView.CurrentRow.Cells["tt"].Value.ToString();               
               
               xy = ZXC.TheVvForm.GetSubModulXY(ZXC.TheVvForm.GetVvSubModulEnumFrom_SubModulShortName(tt));

               recIDforOpenNewRecordTabPage = (uint?)SelectedRecID;

            } // if(this is FakturListUC) 

            // 17.03.2016: 
            else if(this is RtransListUC)
            {
               string selectedTT       = ((RtransListUC)this).SelectedTT      ;
               uint   selectedParentID = ((RtransListUC)this).SelectedParentID;
               bool rtrFound = selectedTT.NotEmpty() && selectedParentID.NotZero();

               if(rtrFound)
               {
                  xy = ZXC.TheVvForm.GetSubModulXY(ZXC.TheVvForm.GetVvSubModulEnumFrom_SubModulShortName(selectedTT));
                  
                  recIDforOpenNewRecordTabPage = (uint?)selectedParentID;
               }

            } // else if(this is RtransListUC) 

            else
            {
               xy = ZXC.TheVvForm.GetSubModulXY(this.MasterSubModulEnum);
               //ZXC.TheVvForm.OpenNew_Record_TabPage(xy, (uint?)SelectedRecID);

               recIDforOpenNewRecordTabPage = (uint?)SelectedRecID;
            }

            ZXC.TheVvForm.OpenNew_Record_TabPage(xy, recIDforOpenNewRecordTabPage /*(uint?)SelectedRecID*/);

         } // else - parent is NOT VvFindDialog nego neki dokirani RecListUC 

      }
   }

   void TheGrid_KeyPress(object sender, KeyPressEventArgs e)
   {
      if(e.KeyChar == (char)Keys.Enter)
      {
         TheGrid_DoubleClick(sender, (EventArgs)e);
      }
   }
   void TheGrid_KeyDown(object sender, KeyEventArgs e)
   {
      if(e.Control && e.KeyCode == Keys.Right)
      {
         button_nextPage.PerformClick();
      }
      if(e.Control && e.KeyCode == Keys.Left)
      {
         button_prevPage.PerformClick();
      }

      if(e.KeyCode == Keys.Down)
      {
         DisplayFullFillText(SelectedRowIndex + 1);
      }
      if(e.KeyCode == Keys.Up)
      {
         DisplayFullFillText(SelectedRowIndex - 1);
      }
   }

   private void button_Reset_Click(object sender, System.EventArgs e)
   {
      VvHamper.ClearFieldContents(hampSpecifikum);
      if(hampFilter != null)
      {
         VvHamper.ClearFieldContents(hampFilter);
         SetListFilterRecordDependentDefaults();
      }

      RadioButton rbt = (RadioButton)this.ControlForInitialFocus.Tag;
      rbt.PerformClick();
      //rbt_Ascending.Checked = true;
      rbt_Ascending.PerformClick();
   }

   protected void FindSifrarTextBox_TextChanged_PERFORM_button_Go_Prev_Next_Action(object sender, EventArgs e)
   {
      // 08.04.2018: 
      if(Should_SVD_speedUp) return;

      VvTextBox vvTB = sender as VvTextBox;

      if(vvTB.Text.Length > 1)
      {
         button_Go_Prev_Next_Action(sender, e);
      }

   }

   protected void CheckBox_biloGdjeUnazivu_Click(object sender, System.EventArgs e)
   {
      CheckBox cb = sender as CheckBox;

      this.recordSorter.BiloGdjeU_Tekstu = cb.Checked;
   }

   #endregion Common Eventz

   #region Common Methods

   #region Abstracts & Virtuals

   protected abstract void InitializeFindFormSpecifics();
   protected virtual void CreateHamperFilter() { }
   /*protected*/
   public virtual void AddFilterMemberz() { }

   protected abstract void CreateHamperSpecifikum();

   protected abstract void CreateDataGridViewColumn();

   //CiaoBaby
   //protected virtual bool PerformAdditionalDataSetOperation() { return true; }
   //public virtual bool IsRemoveUnapropriatesNeeded { get { return false; } }

   #endregion Abstracts & Virtuals

   #region TheG

   protected void InitializeDataGridView()
   {
      TheGrid.DoubleClick += new EventHandler        (TheGrid_DoubleClick);
      TheGrid.KeyPress    += new KeyPressEventHandler(TheGrid_KeyPress);
      TheGrid.KeyDown     += new KeyEventHandler     (TheGrid_KeyDown);

      // 11.11.2011: 
      TheGrid.Click       += new EventHandler(TheGrid_Click);

      bindingSource            = new BindingSource();
      bindingSource.DataMember = TheDataTable.TableName;
      bindingSource.DataSource = VirtualUntypedDataSet;
      TheGrid.DataSource       = bindingSource;

      // NotaBene: brez ovoga dole se raisne exception kada sa FindDialoga 'dockiras' na TabPage pa predjes misom preko TheG-a! 
      TheGrid.ShowCellToolTips = false;
   }

   protected void TheGridFocused()
   {
      if(!TheGrid.Focused)
      {
         TheGrid.Select();
         TheGrid.Focus();
      }
   }

   protected DataGridViewTextBoxColumn Create_DataGridViewTextBoxColumn_4FindForm(string dataPropertyame, string headerText, int colWidth, ref int sumOfColWidth)
   {
      DataGridViewTextBoxColumn colText = new DataGridViewTextBoxColumn();

      colText.DataPropertyName = dataPropertyame;
      colText.Name = dataPropertyame;
      colText.HeaderText = headerText;
      colText.Width = colWidth;
      colText.ReadOnly = true;

      TheGrid.Columns.Add(colText);

      sumOfColWidth += colText.Width;

      return colText;
   }

   protected DataGridViewCheckBoxColumn Create_DataGridViewCheckBoxColumn_4FindForm(string dataPropertyame, string headerText, int colWidth, ref int sumOfColWidth)
   {
      DataGridViewCheckBoxColumn colCbox = new DataGridViewCheckBoxColumn();

      colCbox.DataPropertyName = dataPropertyame;
      colCbox.Name = dataPropertyame;
      colCbox.HeaderText = headerText;
      colCbox.Width = colWidth;
      colCbox.ReadOnly = true;

      TheGrid.Columns.Add(colCbox);

      sumOfColWidth += colCbox.Width;

      return colCbox;
   }

   #endregion TheG

   #region Hamperz

   protected void CreateHamperRastePada()
   {
      hampListaRastePada = new VvHamper(1, 2, "", this, true, nextX, nextY, razmakHamp);

      hampListaRastePada.VvColWdt      = new int[] { ZXC.Q4un + ZXC.Qun4};
      hampListaRastePada.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hampListaRastePada.VvRightMargin = hampListaRastePada.VvLeftMargin;

      hampListaRastePada.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN };
      hampListaRastePada.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hampListaRastePada.VvBottomMargin = hampListaRastePada.VvTopMargin;

      rbt_Ascending = hampListaRastePada.CreateVvRadioButton(0, 0, new EventHandler(radioButtonAscending_Click), "Lista raste", TextImageRelation.ImageAboveText);
      rbt_Ascending.Checked = true;
      rbt_Descending = hampListaRastePada.CreateVvRadioButton(0, 1, new EventHandler(radioButtonDescending_Click), "Lista pada", TextImageRelation.ImageAboveText);
   }

   protected void CreateHamperIzlistaj()
   {
      hampIzlistaj = new VvHamper(1, 2, "", this, false);

      hampIzlistaj.VvColWdt      = new int[] { ZXC.QunBtnW };
      hampIzlistaj.VvSpcBefCol   = new int[] { ZXC.Qun8 };
      hampIzlistaj.VvRightMargin = hampIzlistaj.VvLeftMargin;

      hampIzlistaj.VvRowHgt       = new int[] { ZXC.QunBtnH, ZXC.QunBtnH };
      hampIzlistaj.VvSpcBefRow    = new int[] { ZXC.Qun2, ZXC.Qun4 };
      hampIzlistaj.VvBottomMargin = hampIzlistaj.VvTopMargin;

      button_GO = hampIzlistaj.CreateVvButton(0, 0, new EventHandler(button_GO_Click), "Izlistaj");
      button_Reset = hampIzlistaj.CreateVvButton(0, 1, new EventHandler(button_Reset_Click), "Reset");
      hampIzlistaj.Anchor = AnchorStyles.Top | AnchorStyles.Right;

      if(this.Parent is VvFindDialog) ((VvFindDialog)this.Parent).AcceptButton = button_GO;

   }

   protected void CreateHamperOpenFilter()
   {
      hampOpenFilter = new VvHamper(2, 1, "", this, false, nextX, hampListaRastePada.Bottom + ZXC.Qun5, razmakHamp);
       
      hampOpenFilter.VvColWdt      = new int[] { ZXC.QUN, ZXC.Q3un - ZXC.Qun2 };
      hampOpenFilter.VvSpcBefCol   = new int[] { 0, ZXC.Qun4 };
      hampOpenFilter.VvRightMargin = hampOpenFilter.VvLeftMargin;

      hampOpenFilter.VvRowHgt       = new int[] { ZXC.QUN };
      hampOpenFilter.VvSpcBefRow    = new int[] { 0 };
      hampOpenFilter.VvBottomMargin = hampOpenFilter.VvTopMargin;

      btn_PlusMinusFilter = hampOpenFilter.CreateVvButton(0, 0, new EventHandler(btn_PlusMinusFilter_Click), "+");
      Label lblFilter     = hampOpenFilter.CreateVvLabel(1, 0, "Filteri", ContentAlignment.MiddleLeft);

      lblCrta           = new Label();
      lblCrta.Parent    = this;
      lblCrta.Location  = new Point(hampOpenFilter.Right, hampOpenFilter.Top + ZXC.Qun4 + ZXC.Qun5);
      lblCrta.Size      = new Size(ZXC.Q10un * 2, 1);
      lblCrta.BackColor = Color.LightGray;
   }

   protected void CreateHamperUtil()
   {
      CreateHamperOpenUtil();

      hampUtil = new VvHamper(3, 1, "", this, true, hampOpenUtil.Left, hampOpenUtil.Top, razmakHamp);

      hampUtil.VvColWdt      = new int[] { ZXC.QunBtnW, ZXC.QunBtnW, ZXC.QunBtnW };
      hampUtil.VvSpcBefCol   = new int[] { ZXC.Q6un, ZXC.Q4un, ZXC.Q4un };
      hampUtil.VvRightMargin = hampUtil.VvLeftMargin;

      hampUtil.VvRowHgt       = new int[] { ZXC.QunBtnH };
      hampUtil.VvSpcBefRow    = new int[] { ZXC.Qun2 + ZXC.Qun4 };
      hampUtil.VvBottomMargin = hampUtil.VvTopMargin;

      btnUtil_del     = hampUtil.CreateVvButton(0, 0, new EventHandler(btnUtil_del_Click)    , "Briši");
      btnUtil_copyIn  = hampUtil.CreateVvButton(1, 0, new EventHandler(btnUtil_copyIn_Click) , "KopirajIn");
      btnUtil_copyOut = hampUtil.CreateVvButton(2, 0, new EventHandler(btnUtil_copyOut_Click), "KopirajOut");

      hampUtil.Visible = false;
   }

   protected void CreateHamperOpenUtil()
   {
      int y;
      if(hampOpenFilter == null) y = hampListaRastePada.Bottom + ZXC.Qun5;
      else y = hampOpenFilter.Bottom + ZXC.Qun5;

      hampOpenUtil = new VvHamper(2, 1, "", this, false, nextX, y, razmakHamp);

      hampOpenUtil.VvColWdt      = new int[] { ZXC.QUN, ZXC.Q3un - ZXC.Qun2 };
      hampOpenUtil.VvSpcBefCol   = new int[] { 0, ZXC.Qun4 };
      hampOpenUtil.VvRightMargin = hampOpenUtil.VvLeftMargin;

      hampOpenUtil.VvRowHgt       = new int[] { ZXC.QUN };
      hampOpenUtil.VvSpcBefRow    = new int[] { 0 };
      hampOpenUtil.VvBottomMargin = hampOpenUtil.VvTopMargin;

      btn_PlusMinusUtil = hampOpenUtil.CreateVvButton(0, 0, new EventHandler(btn_PlusMinusUtil_Click), "+");

      Label lblFilter = hampOpenUtil.CreateVvLabel(1, 0, "Util", ContentAlignment.MiddleLeft);


      lblCrta           = new Label();
      lblCrta.Parent    = this;
      lblCrta.Location  = new Point(hampOpenUtil.Right, hampOpenUtil.Top + ZXC.Qun4 + ZXC.Qun5);
      lblCrta.Size      = new Size(ZXC.Q10un * 2, 1);
      lblCrta.BackColor = Color.LightGray;

   }

   public void btn_PlusMinusFilter_Click(object sender, System.EventArgs e)
   {
      Button btn = sender as Button;

      if(btn.Text == "+")
      {
         btn.Text = "-";
         hampFilter.Visible = true;
         lblCrta.Visible = false;

         if(hampOpenUtil != null)
         {
            hampOpenUtil.Location = hampUtil.Location = new Point(nextX, hampFilter.Bottom);

            if(hampUtil.Visible) grBoxLimitiraj.Location = new Point(nextX, hampUtil.Bottom);
            else grBoxLimitiraj.Location = new Point(nextX, hampOpenUtil.Bottom);
         }
         else
         {
            grBoxLimitiraj.Location = new Point(nextX, hampFilter.Bottom);
         }
      }
      else
      {
         btn.Text = "+";
         hampFilter.Visible = false;
         lblCrta.Visible = true;

         if(hampOpenUtil != null)
         {
            hampOpenUtil.Location = hampUtil.Location = new Point(nextX, hampOpenFilter.Bottom);

            if(hampUtil.Visible) grBoxLimitiraj.Location = new Point(nextX, hampUtil.Bottom);
            else grBoxLimitiraj.Location = new Point(nextX, hampOpenUtil.Bottom);
         }
         else
         {
            grBoxLimitiraj.Location = new Point(nextX, hampOpenFilter.Bottom);
         }
      }

      OvisniciOHeight(grBoxLimitiraj.Bottom);
   }

   public void btn_PlusMinusUtil_Click(object sender, System.EventArgs e)
   {
      Button btn = sender as Button;

      if(btn.Text == "+")
      {
         btn.Text = "-";
         hampUtil.Visible = true;
         lblCrta.Visible = false;

         grBoxLimitiraj.Location = new Point(nextX, hampUtil.Bottom);
      }
      else
      {
         btn.Text = "+";
         hampUtil.Visible = false;
         lblCrta.Visible = true;

         grBoxLimitiraj.Location = new Point(nextX, hampOpenUtil.Bottom);
      }

      OvisniciOHeight(grBoxLimitiraj.Bottom);

   }

   private void CreateHamperFillCol()
   {
      hampFillColumn               = new VvHamper(1, 1, "", this, false, dataGridView.Left, dataGridView.Bottom + ZXC.Qun8, 0);

      hampFillColumn.VvColWdt      = new int[] { dataGridView.Width};
      hampFillColumn.VvSpcBefCol   = new int[] { 0};
      hampFillColumn.VvRightMargin = hampFillColumn.VvLeftMargin;
      hampFillColumn.VvRowHgt       = new int[] { ZXC.QUN };
      hampFillColumn.VvSpcBefRow    = new int[] { 0 };
      hampFillColumn.VvBottomMargin = hampFillColumn.VvTopMargin;

      tbx_filColumn = hampFillColumn.CreateVvTextBox(0, 0, "tbx_filColumn","");
      tbx_filColumn.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
      tbx_filColumn.JAM_ReadOnly = true;
   }

   public string Fld_FillColumn { get { return tbx_filColumn.Text; } set { tbx_filColumn.Text = value; } }

   #endregion Hamperz

   #region UtilEvent

   // Watch! Ovdje NE provjeravas PRIVILEGIJE. Assuming da se ovaj 'Util' GroupBox otvara samo SuperUser-u koji ionako moze sve. 

   #region DELETE RECORDS

   private void btnUtil_del_Click(object sender, System.EventArgs e)
   {
      if(TheGrid.RowCount < 1)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nemam što obrisati, izlisatno je nula zapisa!");
         return;
      }

      string message = "Da li zaista zelite nepovratno obrisati " + TheGrid.RowCount.ToString() + " prikazanih zapisa tablice " + TheDataTable.TableName + "!?";
      DialogResult result = MessageBox.Show(message, "Potvrdite BRISANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(result != DialogResult.Yes) return;

      result = MessageBox.Show("Zelite li odustati?!", "Potvrdite BRISANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(result != DialogResult.No) return;

      //-------- Here we go! --------------------------------------------------------------------------------------------------------------------------------- 
      //-------- Here we go! --------------------------------------------------------------------------------------------------------------------------------- 
      //-------- Here we go! --------------------------------------------------------------------------------------------------------------------------------- 

      VvDataRecord theArhivedVvDataRecord;
      VvSQL.VvLockerInfo lockerInfo;
      uint recID;
      bool OK;

      Cursor.Current = Cursors.WaitCursor;

      for(int rIdx = 0; rIdx < TheGrid.RowCount; ++rIdx)
      {
         recID = ZXC.ValOrZero_UInt(TheGrid["recID", rIdx].Value.ToString());

         OK = ZXC.TheVvForm.TheVvDao.SetMe_Record_byRecID(TheDbConnection, VirtualDataRecord, recID, false);

         if(!OK) continue;

         // 18.05.2022: zamjenili mjesta LoadTranses i LoadExtender tako da LoadExtender dode prije LoadTranses jer nekad calcTranses treba TheEx 
         if(VirtualDataRecord.IsExtendable == true) VirtualDataRecord.VvDao.LoadExtender(TheDbConnection,                   VirtualDataRecord, false);
         if(VirtualDataRecord.IsDocument   == true) VirtualDataRecord.VvDao.LoadTranses (TheDbConnection, (VvDocumentRecord)VirtualDataRecord, false);

         lockerInfo = new VvSQL.VvLockerInfo(VirtualDataRecord.VirtualRecordName, VirtualDataRecord.VirtualRecID);

         // 09.09.2009: ovo dole komentirao jer radi probleme kada brises DocumentRecord (sa ostalima, e.g. sifrarima - radi dobro)
         // TODO: mozda da prek nekog ljeta provjeris i usavrsis? 
         //OK = TheVvDao.IsInDBUnchanged(conn, VirtualDataRecord); // da li je u fajlu jos uvijek isti? 

         if(OK && IsSomeoneElseAllreadyEditingThisRecord(TheDbConnection, (IVvDao)TheVvDao, lockerInfo)) return;

         if(OK && TheVvDao.IsThisRecordInSomeRelation(ZXC.PrivilegedAction.DELREC, TheDbConnection, VirtualDataRecord)) return; // ====== Check for CROSS-TABLE integrity violation 

         // ====== User 4 MySQL additions START =============================
         if(OK && VirtualDataRecord.VirtualRecordName == User.recordName)
         {
            OK = VvSQL.DropUser(TheDbConnection, VirtualDataRecord as User);
         }
         // ====== User 4 MySQL additions END   =============================

         if(OK)
         {
            OK = TheVvDao.DELREC(TheDbConnection, VirtualDataRecord, false);
         }

         if(OK)
         {  //                                                                                                                              
            /* */ theArhivedVvDataRecord = VirtualDataRecord.CreateArhivedDataRecord(TheDbConnection, "BRISANJE");                     //   
            /* */                                                                                                                      //   
            /* */ bool arOK = theArhivedVvDataRecord.VvDao.ADDREC(TheDbConnection, theArhivedVvDataRecord, true, true, false, false);  //   
         }  //                                                                                                                              

      } // foreach 

      button_GO.PerformClick(); // refresh DGV 

      Cursor.Current = Cursors.Default;
   }

   private bool IsSomeoneElseAllreadyEditingThisRecord(XSqlConnection conn, IVvDao vvDao, VvSQL.VvLockerInfo lockerInfo)
   {
      if(vvDao.IsInLocker(conn, lockerInfo, false))
      {
         ZXC.aim_emsg(
            "Zapis je u procesu izmjene od strane korisnika <" +
            lockerInfo.editorUID + "@" + "(" +
            lockerInfo.clientMachineName + ":" +
            lockerInfo.clientUserName +
            ")>. Pricekajte, molim, obradu.");
         return true;
      }

      return false;
   }

   #endregion DELETE RECORDS

   #region COPY RECORDS

   private void btnUtil_copyIn_Click(object sender, System.EventArgs e)
   {
      // 23.11.2012: 
    //if(VvSQL.GetDbNameForThisTableName(VirtualDataRecord.VirtualRecordName) == TheVvTabPage.TheVvForm.GetvvDB_prjktDB_name() /*ZXC.vvDB_prjktDB_Name*/)
      if(VvSQL.GetDbNameForThisTableName(VirtualDataRecord.VirtualRecordName) == ZXC.TheVvForm.GetvvDB_prjktDB_name() /*ZXC.vvDB_prjktDB_Name*/)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Kopiranje ne radi za datoteku {0}!", VirtualDataRecord.VirtualRecordName);
         return;
      }

      ExecuteMassCopyOfRecords(TheDbConnection, true, true, true);
   }

   private void btnUtil_copyOut_Click(object sender, System.EventArgs e)
   {
      if(VvSQL.GetDbNameForThisTableName(VirtualDataRecord.VirtualRecordName) == ZXC.VvDB_prjktDB_Name)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Kopiranje ne radi za datoteku {0}!", VirtualDataRecord.VirtualRecordName);
         return;
      }

      // 03.10.2013: 
      string origServerName = ZXC.vvDB_Server;
      string origVvDomena   = ZXC.vvDB_VvDomena;

      LoginForm lf = new LoginForm(true);
      
      lf.ShowDialog();

      if(lf.DialogResult == DialogResult.Cancel) return;

      if(lf.Fld_UseThisPrjkt.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Morate zadati sifru projekta.");
         return;
      }

      //-------------- Here we go! ------------------------------------------------ 

      //ZXC.VvDataBaseInfo dbi = (ZXC.VvDataBaseInfo)lf.combbx_prjkt.SelectedItem;

      XSqlConnection conn = lf.TheUtilConnection;

      string theTicker = ZXC.KupdobDao.GetTickerForKupdobCD(conn, lf.Fld_UseThisPrjkt, true);

      conn.ChangeDatabase(ZXC.VvDB_NameConstructor(lf.Fld_ProjectYear.ToString("yyyy"), theTicker, lf.Fld_UseThisPrjkt));

      // 03.10.2013: 
      ZXC.vvDB_Server   = origServerName;
      ZXC.vvDB_VvDomena = origVvDomena  ;

      ExecuteMassCopyOfRecords(conn, lf.Fld_WeWantOrigDokNum, lf.Fld_WeWantOrigTtNum, false);

   }

   private void ExecuteMassCopyOfRecords(XSqlConnection conn, bool weWantOrigDokNum, bool weWantOrigTtNum, bool isCopyIN)
   {
      uint recID, okCount=0;
      bool OK;

      bool isCopyOUT = !isCopyIN;

      string message = "Da li zaista zelite umnožiti " + TheGrid.RowCount.ToString() + " prikazanih zapisa tablice [" + TheDataTable.TableName + "]\n\n" +
                       " U database: [" + conn.Database + "]!?";

      DialogResult result = MessageBox.Show(message, "Potvrdite KOPIRANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(result != DialogResult.Yes) return;

      #region Person 2 Kupdob additions

      // 23.11.2012: Person 2 Kupdob additions 
      if(VirtualDataRecord is Person)
      {
         result = MessageBox.Show("KopirajIN ove æe RADNIKE kopirati u datoteku PARTNERa\n\numjesto u RADNIKe!\n\nNASTAVITI?", "IZ RADNIKA U PARTNERE!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

         if(result != DialogResult.Yes) return;
      }

      #endregion Person 2 Kupdob additions

      result = MessageBox.Show("Zelite li odustati?!", "Potvrdite KOPIRANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(result != DialogResult.No) return;

      //-------- Here we go! --------------------------------------------------------------------------------------------------------------------------------- 
      //-------- Here we go! --------------------------------------------------------------------------------------------------------------------------------- 
      //-------- Here we go! --------------------------------------------------------------------------------------------------------------------------------- 

      Cursor.Current = Cursors.WaitCursor;

      ZXC.CopyOut_InProgress = true;

      for(int rIdx = 0; rIdx < TheGrid.RowCount; ++rIdx)
      {
         recID = ZXC.ValOrZero_UInt(TheGrid["recID", rIdx].Value.ToString());

         OK = ZXC.TheVvForm.TheVvDao.SetMe_Record_byRecID(TheDbConnection, VirtualDataRecord, recID, false);

         if(!OK) continue;

         // 18.05.2022: zamjenili mjesta LoadTranses i LoadExtender tako da LoadExtender dode prije LoadTranses jer nekad calcTranses treba TheEx 
         if(VirtualDataRecord.IsExtendable == true) VirtualDataRecord.VvDao.LoadExtender(TheDbConnection,                   VirtualDataRecord, false);
         if(VirtualDataRecord.IsDocument   == true) VirtualDataRecord.VvDao.LoadTranses (TheDbConnection, (VvDocumentRecord)VirtualDataRecord, false);

         if(OK)
         {
            OK = EnsureNonDuplicateKeys(conn, VirtualDataRecord, weWantOrigDokNum, weWantOrigTtNum);
         }

         if(OK)
         {
            // 23.11.2012: Person 2 Kupdob additions 
            if(VirtualDataRecord is Person)
            {
               Kupdob kupdob_rec = PersonDao.CreateKupdobFromPerson(TheDbConnection, (Person)(VirtualDataRecord));
               OK = TheVvDao.ADDREC(conn, kupdob_rec, false, false, false, false);
            }

            #region Tmp Tembo Util
#if Njett
            else if(VirtualDataRecord is Kupdob && (ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.TEMBO || ZXC.CURR_prjkt_rec.Ticker.StartsWith("TEMBO")))
            {
               Kupdob kupdob_rec = VirtualDataRecord as Kupdob;

               bool isCitroen = kupdob_rec.Tip.Contains('C');
               bool isPeugeot = kupdob_rec.Tip.Contains('P');
               string grupaID = isCitroen ? "C" : isPeugeot ? "P" : "X";

               uint    CentrID  ;   
               string  CentrTick;

               if(isCitroen)
               {
                  CentrID   = 12;   
                  CentrTick = "CITR12";
               }
               else if(isPeugeot)
               {
                  CentrID   = 510;   
                  CentrTick = "PEUG10";
               }
               else 
               {
                  CentrID   = 0;
                  CentrTick = "";
               }

               kupdob_rec.Naziv = ZXC.LenLimitedStr(kupdob_rec.Naziv + " " + grupaID, ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KpdbCI.naziv));
               kupdob_rec.Tip = "";

               kupdob_rec.CentrID   = CentrID  ;
               kupdob_rec.CentrTick = CentrTick;

               uint newSifra;
               string sifraColName = "kupdobCD";
               newSifra = kupdob_rec.VvDao.GetNextSifra_Uint(TheDbConnection, VirtualDataRecord.VirtualRecordName, sifraColName, 0, 0);

               kupdob_rec.KupdobCD = newSifra;

               int tkLen = kupdob_rec.Ticker.Length;
               if(tkLen <= 4)      kupdob_rec.Ticker = kupdob_rec.Ticker + "_" + grupaID;
               else if(tkLen == 5) kupdob_rec.Ticker = kupdob_rec.Ticker + grupaID;
               else                kupdob_rec.Ticker = kupdob_rec.Ticker.Substring(0, 5) + grupaID;

               OK = TheVvDao.ADDREC(conn, kupdob_rec, false, false, false, false);
            }
#endif
            #endregion Tmp Tembo Util

            else
            {
               // 25.02.2016: bez ovoga, bijo BUG! start 
               if(VirtualDataRecord.IsDocument)
               {
                  if(weWantOrigDokNum == false)
                  {
                     (VirtualDataRecord as VvDocumentRecord).VirtualTranses.ForEach(trn => trn.VirtualT_dokNum = (VirtualDataRecord as VvDocumentRecord).VirtualDokNum);
                  }
                  if(weWantOrigTtNum == false)
                  {
                     (VirtualDataRecord as VvDocumentRecord).VirtualTranses.ForEach(trn => trn.VirtualT_ttNum = (VirtualDataRecord as VvDocumentRecord).VirtualTTnum);
                  }

                  //29.12.2017: TEXTHO nulti ZPC DokDate handling 
                  if(ZXC.IsTEXTHOany && VirtualDataRecord is Faktur && (VirtualDataRecord as Faktur).TT == Faktur.TT_ZPC)
                  {
                     (VirtualDataRecord as Faktur).DokDate = ZXC.nextYearFirstDay;
                     (VirtualDataRecord as Faktur).Transes.ForEach(trn => trn.T_skladDate = (VirtualDataRecord as Faktur).DokDate);
                  }

               }
               // 25.02.2016: bez ovoga, bijo BUG! end 

               #region HRD 2022 ---> 2023

               bool isCopyOUT_2022to2023 = isCopyOUT && ZXC.projectYearAsInt == 2022 && conn.Database.Contains("2023");

               if(isCopyOUT_2022to2023)
               {
                  #region Faktur

                  if(VirtualDataRecord is Faktur)
                  {
                     Faktur faktur_rec = VirtualDataRecord as Faktur;

                     bool wasEURfaktur = faktur_rec.DevName.ToUpper() == "EUR";

                     #region TT_CKP - Cjenik KUPCA (kunski ili devizni) 

                     if(faktur_rec.TT == Faktur.TT_CKP) // Cjenik KUPCA (kunski ili devizni) 
                     {
                        if(wasEURfaktur)
                        {
                           decimal euroT_cij, tecaj;

                           DateTime cjenikDokDate = faktur_rec.DokDate;

                           foreach(Rtrans rtrans in faktur_rec.Transes)
                           {
                              tecaj = ZXC.DevTecDao.GetHnbTecaj(ZXC.ValutaNameEnum.EUR, cjenikDokDate);

                              euroT_cij = ZXC.DivSafe(rtrans.T_cij, tecaj).Ron2();

                              rtrans.T_cij = euroT_cij;
                           }

                           faktur_rec.DevName = "";

                        } // was EURfaktur

                        else // was NOT EURfaktur
                        {
                           faktur_rec.Transes.ForEach(trn => trn.T_cij = ZXC.EURiIzKuna_HRD_(trn.T_cij));
                        }
                     }

                     #endregion TT_CKP - Cjenik KUPCA (kunski ili devizni) 

                     #region TT_CJ_VP1i2

                     if(faktur_rec.TT == Faktur.TT_CJ_VP1 || faktur_rec.TT == Faktur.TT_CJ_VP2) 
                     {
                        faktur_rec.Transes.ForEach(trn => trn.T_cij = ZXC.EURiIzKuna_HRD_(trn.T_cij));
                     }

                     #endregion TT_CJ_VP1i2

                     #region TT_ZAH

                     if(faktur_rec.TT == Faktur.TT_ZAH)
                     {
                      //faktur_rec.                           Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB(null);
                      //faktur_rec.Transes.ForEach(rtr => rtr.Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB(null));
                        faktur_rec.Transes.ForEach(rtr => rtr.T_cij = ZXC.DivSafe(rtr.T_cij, ZXC.HRD_tecaj)/*.Ron2()*/);
                        faktur_rec.Transes.ForEach(rtr => rtr.CalcTransResults(faktur_rec));
                        faktur_rec.TakeTransesSumToDokumentSum(true);
                     }

                     #endregion TT_ZAH

                     #region PRJs, UGO, ...

                     if(faktur_rec.TtInfo.IsProjektTT)
                     {
                        faktur_rec.SomeMoney = faktur_rec.SomeMoney.EURiIzKuna_HRD_();
                        faktur_rec.Decimal01 = faktur_rec.Decimal01.EURiIzKuna_HRD_();
                        faktur_rec.Decimal02 = faktur_rec.Decimal02.EURiIzKuna_HRD_();

                        if(faktur_rec.TT == Faktur.TT_UGO) // SVD 
                        {
                         //faktur_rec.Transes.ForEach(rtr => rtr.Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB(null)    );
                           faktur_rec.Transes.ForEach(rtr => rtr.T_cij = ZXC.DivSafe(rtr.T_cij, ZXC.HRD_tecaj)/*.Ron2()*/);
                           faktur_rec.Transes.ForEach(rtr => rtr.CalcTransResults(faktur_rec));
                           faktur_rec.TakeTransesSumToDokumentSum(true);
                        }
                        else // ovi nemaju Rtrans stavaka 
                        {
                           faktur_rec.Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB(null);
                        }
                     }

                     #endregion PRJs, UGO, ...

                  } // if(VirtualDataRecord is Faktur)

                  #endregion Faktur

                  #region Mixer

                  if(VirtualDataRecord is Mixer)
                  {
                     Mixer mixer_rec = VirtualDataRecord as Mixer;

                     // SVIMA, bez obzira na TT!?: 
                     mixer_rec.MoneyA = ZXC.EURiIzKuna_HRD_(mixer_rec.MoneyA);

                     if(mixer_rec.TT == Mixer.TT_RASTERF)
                     {
                        mixer_rec.Transes.ForEach(trn => trn.T_moneyA = ZXC.EURiIzKuna_HRD_(trn.T_moneyA));
                     }

                     bool wasEURmixer = mixer_rec.DevName.ToUpper() == "EUR"; 

                     if(mixer_rec.TT == Mixer.TT_RASTERB)
                     {
                        if(wasEURmixer) // DO NOTHING. data layer veæ je u EURima 
                        {
                           //decimal euroT_cij, tecaj;
                           //
                           //DateTime cjenikDokDate = mixer_rec.DokDate;
                           //
                           //foreach(Xtrans xtrans in mixer_rec.Transes)
                           //{
                           //   tecaj = ZXC.DevTecDao.GetHnbTecaj(ZXC.ValutaNameEnum.EUR, cjenikDokDate);
                           //
                           //   euroT_cij = ZXC.DivSafe(xtrans.T_moneyA, tecaj).Ron2();
                           //   xtrans.T_moneyA = euroT_cij;
                           //
                           //   euroT_cij = ZXC.DivSafe(xtrans.T_moneyB, tecaj).Ron2();
                           //   xtrans.T_moneyB = euroT_cij;
                           //
                           //   euroT_cij = ZXC.DivSafe(xtrans.T_moneyC, tecaj).Ron2();
                           //   xtrans.T_moneyC = euroT_cij;
                           //
                           //}
                           //

                           mixer_rec.DevName = "";

                        } // was EURfaktur

                        else // was NOT EURfaktur
                        {
                           foreach(Xtrans xtrans in mixer_rec.Transes)
                           {
                              xtrans.T_moneyA = ZXC.EURiIzKuna_HRD_(xtrans.T_moneyA);
                              xtrans.T_moneyB = ZXC.EURiIzKuna_HRD_(xtrans.T_moneyB);
                              xtrans.T_moneyC = ZXC.EURiIzKuna_HRD_(xtrans.T_moneyC);
                           }
                        }

                     } // if(mixer_rec.TT == Mixer.TT_RASTERB) 

                  } // if(VirtualDataRecord is Mixer) 

                  #endregion Mixer

                  #region Placa

                  if(VirtualDataRecord is Placa)
                  {
                     Placa placa_rec = VirtualDataRecord as Placa;

                     placa_rec.                            Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB(null) ;
                     placa_rec.Transes .ForEach(rtr => rtr.Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB(null));
                   //placa_rec.Transes2.ForEach(rtr => rtr.Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB(null));
                     placa_rec.Transes3.ForEach(rtr => rtr.Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB(null));
                  }

                  #endregion Placa

               }

               #endregion HRD 2022 ---> 2023

               OK = TheVvDao.ADDREC(conn, VirtualDataRecord, /*false 02.01.2012: */VirtualDataRecord.IsDocument, false, false, false);
            }
         }

         if(OK) okCount++;

      } // for(int rIdx = 0; rIdx < TheGrid.RowCount; ++rIdx)

      button_GO.PerformClick(); // refresh DGV 

      ZXC.CopyOut_InProgress = false;

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo. Dodao {0} novih {1}", okCount, VirtualDataRecord is Person ? "KUPDOBa" : VirtualDataRecord.VirtualRecordName);

      // 09.01.2012:
      if(VirtualDataRecord is Faktur) ZXC.aim_emsg(MessageBoxIcon.Exclamation, "NE ZABORAVITE ruèno rekreirati Artstat CACHE!");
   }

   /*private*/ internal static bool EnsureNonDuplicateKeys(XSqlConnection conn, VvDataRecord VirtualDataRecord, bool weWantOrigDokNum, bool weWantOrigTtNum)
   {
      // to speed up: provjeri da li je target tablica prazna. Ako je, odma return-aj iz ovoga, jerbo je sigurno non duplicate! 

      #region 02.10.2012 we want new dokNum or ttNum

      if(VirtualDataRecord.IsDocumentLike != true) return true;

      if(weWantOrigDokNum == false)
      {
         uint newDokNum = /*TheVvDao*/ZXC.FakturDao.GetNextDokNum(conn, VirtualDataRecord.VirtualRecordName);
         ((VvDocumLikeRecord)VirtualDataRecord).VirtualDokNum = newDokNum;
      }

      if(weWantOrigTtNum == false)
      {
         string olfaSkladCD = "";

         // 30.11.2022:
         if(VirtualDataRecord is Faktur && (VirtualDataRecord as Faktur).TT == Faktur.TT_ZAH)
         {
            olfaSkladCD = (VirtualDataRecord as Faktur).KupdobTK;
         }

       //uint newTtNum = /*TheVvDao*/ZXC.FakturDao.GetNextTtNum(conn, ((VvDocumLikeRecord)VirtualDataRecord).VirtualTT, ""         );
         uint newTtNum = /*TheVvDao*/ZXC.FakturDao.GetNextTtNum(conn, ((VvDocumLikeRecord)VirtualDataRecord).VirtualTT, olfaSkladCD);

         // 05.12.2022: 
         if(VirtualDataRecord is Mixer)
         {
            newTtNum = /*TheVvDao*/ZXC.MixerDao.GetNextTtNum(conn, ((VvDocumLikeRecord)VirtualDataRecord).VirtualTT, "");
         }

         ((VvDocumLikeRecord)VirtualDataRecord).VirtualTTnum = newTtNum;
      }

      #endregion 02.10.2012 we want new dokNum or ttNum

      return true;
   }

   #endregion COPY RECORDS

   #endregion UtilEvent

   #region GroupBoxLimit

   protected void CreateGroupBoxLimit(int y)
   {
      grBoxLimitiraj = new GroupBox();
      grBoxLimitiraj.Parent = this;
      grBoxLimitiraj.Location = new Point(ZXC.QunMrgn, y);

      checkBox_shouldLimit = new CheckBox();
      checkBox_shouldLimit.Parent = grBoxLimitiraj;
      checkBox_shouldLimit.Location = new Point(ZXC.Qun4, ZXC.QunMrgn);
      checkBox_shouldLimit.Size = new Size(ZXC.Q6un - ZXC.Qun4 - ZXC.Qun12 + ZXC.Qun10, ZXC.QUN);
      checkBox_shouldLimit.Text = "Limitiraj listu na:";
      checkBox_shouldLimit.RightToLeft = RightToLeft.No;
      checkBox_shouldLimit.TextAlign = ContentAlignment.MiddleRight;
      checkBox_shouldLimit.Font = ZXC.vvFont.SmallFont;
      checkBox_shouldLimit.Click += new EventHandler(checkBox_shouldLimit_Click);

      tbx_limitNum = new VvTextBox();
      tbx_limitNum.Parent = grBoxLimitiraj;
      tbx_limitNum.AutoSize = false;
      tbx_limitNum.Location = new Point(checkBox_shouldLimit.Right, ZXC.QunMrgn);
      tbx_limitNum.Font = ZXC.vvFont.BaseFont;
      tbx_limitNum.Size = new Size(ZXC.Q2un + ZXC.Qun2, ZXC.QUN);
      tbx_limitNum.Enabled = false;
      tbx_limitNum.MaxLength = 6;
      tbx_limitNum.Name = "tbx_limitNum";
      tbx_limitNum.Text = "12";
      tbx_limitNum.TextAlign = HorizontalAlignment.Left;
      tbx_limitNum.TextChanged += new System.EventHandler(this.tbx_limitNum_TextChanged);
      tbx_limitNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_limitNum.JAM_MarkAsNumericTextBox(0, false, 1, decimal.MinValue, true);

      //tbx_limitNum.JAM_FieldExitWithValidationMethod = new System.ComponentModel.CancelEventHandler(TestingMyValue);

      VvHamper.Open_Close_Fields_ForWriting(tbx_limitNum, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

      Label labelRed = new Label();
      labelRed.Parent = grBoxLimitiraj;
      labelRed.Location = new Point(tbx_limitNum.Right, ZXC.QunMrgn);
      labelRed.Size = new Size(ZXC.Q2un, ZXC.QUN);
      labelRed.Name = "labelRed";
      labelRed.Text = "red.";
      labelRed.Font = ZXC.vvFont.SmallFont;
      labelRed.TextAlign = ContentAlignment.MiddleLeft;

      button_prevPage = new Button();
      button_prevPage.Parent = grBoxLimitiraj;
      button_prevPage.Size = new Size(ZXC.QunBtnW, ZXC.QunBtnH);
      button_prevPage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      button_prevPage.Enabled = false;
      button_prevPage.Name = "button_prevPage";
      button_prevPage.TabIndex = 1;
      button_prevPage.Text = "<-&--";
      button_prevPage.Font = ZXC.vvFont.SmallFont;
      button_prevPage.FlatStyle = FlatStyle.System;
      button_prevPage.Click += new System.EventHandler(this.button_prevPage_Click);

      button_nextPage = new Button();
      button_nextPage.Parent = grBoxLimitiraj;
      button_nextPage.Location = new Point(grBoxLimitiraj.Width - ZXC.QunBtnW - ZXC.Qun8, ZXC.QunMrgn);
      button_prevPage.Location = new Point(button_nextPage.Left - ZXC.QunBtnW - 2 * ZXC.Qun4, ZXC.QunMrgn);
      button_nextPage.Size = new Size(ZXC.QunBtnW, ZXC.QunBtnH);
      button_nextPage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      button_nextPage.Enabled = false;
      button_nextPage.Name = "button_nextPage";
      button_nextPage.Text = "-&+->";
      button_nextPage.Font = ZXC.vvFont.SmallFont;
      button_nextPage.FlatStyle = FlatStyle.System;
      button_nextPage.Click += new System.EventHandler(this.button_nextPage_Click);

      labelMin = new Label();
      labelMin.Parent = grBoxLimitiraj;
      labelMin.Size = new Size(ZXC.Q2un, ZXC.QUN);
      labelMin.Location = new Point(ZXC.Qun4, labelRed.Bottom + ZXC.Qun2);
      labelMin.Name = "labelMin";
      labelMin.Text = "[";
      labelMin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      labelMin.Font = ZXC.vvFont.SmallSmallFont;

      labelPositionInfo = new Label();
      labelPositionInfo.Parent = grBoxLimitiraj;
      labelPositionInfo.Size = new Size(ZXC.Q4un, ZXC.QUN);
      labelPositionInfo.Location = new Point((grBoxLimitiraj.Width - labelPositionInfo.Width) / 2, labelRed.Bottom + ZXC.Qun2);
      labelPositionInfo.Name = "labelPositionInfo";
      labelPositionInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      labelPositionInfo.Text = "[   ]";
      labelPositionInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      labelPositionInfo.Font = ZXC.vvFont.SmallSmallFont;

      labelMax = new Label();
      labelMax.Parent = grBoxLimitiraj;
      labelMax.Size = new Size(ZXC.Q3un, ZXC.QUN);
      labelMax.Location = new Point(grBoxLimitiraj.Right - labelMax.Width - ZXC.QunMrgn - ZXC.Qun4, labelRed.Bottom + ZXC.Qun2);
      labelMax.Name = "labelMax";
      labelMax.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      labelMax.Text = "]";
      labelMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      labelMax.Font = ZXC.vvFont.SmallSmallFont;

      scrollBarNorrWoLimit = new HScrollBar();
      scrollBarNorrWoLimit.Parent = grBoxLimitiraj;
      scrollBarNorrWoLimit.Location = new Point(ZXC.Qun4, labelMax.Bottom + ZXC.Qun5);
      scrollBarNorrWoLimit.Width = grBoxLimitiraj.Width - 2 * ZXC.Qun4;
      scrollBarNorrWoLimit.Name = "scrollBarNorrWoLimit";
      scrollBarNorrWoLimit.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      scrollBarNorrWoLimit.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollBarNorrWoLimit_Scroll);

      grBoxLimitiraj.Height = labelMax.Bottom + ZXC.Qun5 + scrollBarNorrWoLimit.Height + ZXC.Qun2 - ZXC.Qun8;
      grBoxLimitiraj.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
   }

   private bool Should_SVD_speedUp
   {
      get
      {
       //return ZXC.IsSvDUH && this is ArtiklListUC &&  ZXC.TheVvForm.TheVvUC is IZD_SVD_DUC                                          &&
         return ZXC.IsSvDUH && this is ArtiklListUC && (ZXC.TheVvForm.TheVvUC is IZD_SVD_DUC || ZXC.TheVvForm.TheVvUC is ZAH_SVD_DUC) &&
                ZXC.TheVvForm.VvPref.findArtikl.IsStatus && // 'prikazi stanje' 
                ZXC.TheVvForm.TheVvUC.TheVvTabPage.WriteMode != ZXC.WriteMode.None; // samo u zutome 
      }
   }

   private void GroupBoxLimit_CalclimitNum()
   {
      button_prevPage.Text = "<--" + Int32.Parse(tbx_limitNum.Text) + "--";
      button_nextPage.Text = "--" + Int32.Parse(tbx_limitNum.Text) + "-->";

      //*********** tam >>>>>>>>>>>>> zbog 1 tabPage-a Height razlicit je zbog ts_ipa jer ga racuna prije
      //int preferredPageSize = (int)Math.Floor((decimal)((TheG.Height - 2 - TheG.ColumnHeadersHeight) / TheG.RowTemplate.Height));
      //tbx_limitNum.Text = preferredPageSize.ToString();

      int gridHeight;
      if(this.Parent is Panel && ZXC.TheVvForm.miSub_SubModulSetToolStripVisible.Checked
                              && ZXC.TheVvForm.TheTabControl.TabPages.Count <= 1)
         gridHeight = TheGrid.Height - ZXC.TheVvForm.ts_Record.Height;
      else gridHeight = TheGrid.Height;

      int preferredPageSize;

      // 08.04.2018: 
    //preferredPageSize = (int)Math.Floor((decimal)((gridHeight - 2 - TheGrid.ColumnHeadersHeight) / TheGrid.RowTemplate.Height));

      if(Should_SVD_speedUp && ZXC.RRD.Dsc_FISK_TimeOutSeconds.NotZero()) preferredPageSize = ZXC.RRD.Dsc_FISK_TimeOutSeconds; // da ne dodajemo novi dsc_ 
      else                                                                preferredPageSize = (int)Math.Floor((decimal)((gridHeight - 2 - TheGrid.ColumnHeadersHeight) / TheGrid.RowTemplate.Height));

      tbx_limitNum.Text = preferredPageSize.ToString();

      //*********** tam >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>


      // enforcing paqing 
      PaggingTurnedOn = true;
      this.tbx_limitNum.Enabled = true;

      // 26.09.2018: pokusaj da Roman ima neogranicene retke 
      if(ZXC.IsSvDUH && Should_SVD_speedUp == false)
      {
         PaggingTurnedOn = false;

      }
   }
   //private void TestingMyValue(Object sender, System.ComponentModel.CancelEventArgs e)
   //{
   //   VvTextBox vvtb = sender as VvTextBox;
   //   int vvtbValue;

   //   int.TryParse(vvtb.Text, out vvtbValue);

   //   if(vvtbValue < 1)
   //   {
   //      ZXC.aim_emsg("nebudobro");
   //      e.Cancel = true;
   //   }
   //}

   #endregion GroupBoxLimit

   #region ImaLiIjedan_StartField_Neprazan, SetVvPrefLink_AND_ToolTipText_ForGenericBiloGdjeUnazivuCheckBox

   private bool ImaLiIjedan_StartField_Neprazan(Control thisControl)
   {
      if(this.Supress_ImaLiIjedan_StartField_Neprazan_Action == true) return false;

      bool imaLiIjedanNeprazan = false;

      foreach(Control ctrl in thisControl.Controls)
      {
         if(ctrl is VvTextBox && ((VvTextBox)ctrl).IsNonEmpty())
         {
            VvTextBox vvtb = ctrl as VvTextBox;

            if(vvtb.JAM_ValueType == typeof(int) && vvtb.Text == "0") continue;

            imaLiIjedanNeprazan = true;

            if(ctrl.Tag is RadioButton)
            {
               RadioButton rbt = (RadioButton)ctrl.Tag;
               rbt.Checked = true;
               rbt.PerformClick();
            }
            else
            {
               imaLiIjedanNeprazan = false; // 26.10.2010. sklad na artiklFind-u dize da je naprazan
            }

            //TheGridFocused();
            //vvtb.Select();

            break;
         }
      }

      return imaLiIjedanNeprazan;
   }

   public void SetVvPrefLink_AND_ToolTipText_ForGenericBiloGdjeUnazivuCheckBox(CheckBox cbx_biloGdjeUnazivu, EventHandler cbx_biloGdjeUnazivu_Click_SaveToVvPref)
   {
      VvHamper.SetVvPrefLink_AND_ToolTipText_ForGenericBiloGdjeUnazivuCheckBox(cbx_biloGdjeUnazivu, new EventHandler(cbx_biloGdjeUnazivu_Click_SaveToVvPref), ToolTipBiloGdjeUnazivu, ToolTipTextBiloGdjeUnazivu);
   }

   #endregion ImaLiIjedan_StartField_Neprazan

   #region CalculateLocationAndSize

   /*protected*/
   public void CalculateLocationAndSize()
   {
      if(this.Parent is VvFindDialog)
      {
         // 16.03.2016. dodano za VvFindRtransDlg koji nema hampFilter
         if(this is RtransListUC)
         {
            this.Width = hampSpecifikum.Right + hampIzlistaj.Width + ZXC.QUN + ZXC.Qun2;
            TheGrid.Width = this.Width - 2 * ZXC.QunMrgn;
         }
         else if(this is KDC_ListUC)
         {
            this.Width = grid_Width + 2 * ZXC.QUN;
            TheGrid.Width = grid_Width;
         }
         else
         {
            if(grid_Width > (hampSpecifikum.Right + hampIzlistaj.Width + ZXC.Qun2))
            {
               this.Width = grid_Width + 2 * ZXC.QUN;
               TheGrid.Width = grid_Width;
            }
            else if(hampFilter.Right > (hampSpecifikum.Right + hampIzlistaj.Width + ZXC.Qun2) && hampFilter.Right > grid_Width)
            {
               this.Width = hampFilter.Right + ZXC.QUN + ZXC.Qun2;
               TheGrid.Width = this.Width - 2 * ZXC.QunMrgn;
            }
            else
            {
               this.Width = hampSpecifikum.Right + hampIzlistaj.Width + ZXC.QUN + ZXC.Qun2;
               TheGrid.Width = this.Width - 2 * ZXC.QunMrgn;
            }
         }
      }
      else
      {
         TheGrid.Width = this.Width - 2 * ZXC.QunMrgn;
      }

      OvisniciOHeight(grBoxLimitiraj.Bottom);

      grBoxLimitiraj.Width = TheGrid.Width;
      hampIzlistaj.Location = new Point(TheGrid.Right - hampIzlistaj.Width, nextY);

      if(this.Parent is VvFindDialog)
      {
         this.Parent.Location = new Point(SystemInformation.WorkingArea.Width - this.Width, 0);
      }
      
      //      if(this.Width > ZXC.TheVvForm.Width)
      //      {
      ////         ZXC.aim_emsg("Veliki datagrid ili hamperSpecifikum"); // a veliki je sigurno kad ima arhivu
      //         this.Parent.Width = ZXC.TheVvForm.Width;
      //         this.Width        = ZXC.TheVvForm.Width;
      //      }
   }

   public void OvisniciOHeight(int grBoxLimitBottom)
   {
      int heightRecListUC;

      // 15.04.2022: 
      if(this is KDC_ListUC)
      {
         this.Parent.Height = SystemInformation.WorkingArea.Height;

         heightRecListUC = this.Parent.ClientSize.Height - 2 * ZXC.QunBtnH - ZXC.QunMrgn;
         this.Parent.Width = this.Width;

         this.Parent.MinimumSize =
            new Size(hampIzlistaj.Width + ZXC.Qun2 + ZXC.QUN,
                     grBoxLimitBottom + TheGrid.ColumnHeadersHeight + 2 * TheGrid.RowTemplate.Height + ZXC.Q2un + ZXC.Qun2 + hampFillColumn.Height);

         this.Height = heightRecListUC; // mora bti ovdje zbog anhora
         this.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
      }
      else if(this.Parent is VvFindDialog)
      {
         // 13.10.2010: preseljeno u VvFindDialog.cs
         //this.Parent.Height = 725;
         //this.Parent.Location = new Point(SystemInformation.WorkingArea.Width - this.Width, 0);  15.10. - skace pa je premjesteno gore
         this.Parent.Height    = SystemInformation.WorkingArea.Height;

         heightRecListUC = this.Parent.ClientSize.Height - 2 * ZXC.QunBtnH - ZXC.QunMrgn;
         this.Parent.Width = this.Width;


         this.Parent.MinimumSize =
            new Size(hampSpecifikum.Right + hampIzlistaj.Width + ZXC.Qun2 + ZXC.QUN,
                     grBoxLimitBottom + TheGrid.ColumnHeadersHeight + 2 * TheGrid.RowTemplate.Height + ZXC.Q2un + ZXC.Qun2 + hampFillColumn.Height);

         this.Height = heightRecListUC; // mora bti ovdje zbog anhora
         this.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
      }
      else
      {
         heightRecListUC = this.Parent.Height;

         VvHamper.Create_Label4PrisilniScrollZbogGrida(TheGrid, grid_Width);

         this.Height = heightRecListUC;
         this.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
      }

      TheGrid.Height = heightRecListUC - grBoxLimitBottom - 2 * ZXC.QunMrgn - 2;
      TheGrid.Location = new Point(ZXC.QunMrgn, grBoxLimitBottom + ZXC.Qun4 + ZXC.Qun8);
      TheGrid.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

      hampFillColumn.Location = new Point(TheGrid.Left, TheGrid.Bottom + ZXC.Qun8);
      CalcFillColumnHamperSizeAndLeftanchor();
   }

   #endregion CalculateLocationAndSize

   #region OpenClose_VvTextBoxOnFindDialog

   protected RadioButton OpenClose_VvTextBoxOnFindDialog(RadioButton rbt, RadioButton rBtcurrChecked)
   {
      VvTextBox vvTb = (VvTextBox)rbt.Tag;

      if(rbt != rBtcurrChecked)
      {
         VvHamper.Open_Close_Fields_ForWriting(vvTb, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);
      }
      
      if(vvTb.Tag is VvDateTimePicker)
      {
         VvDateTimePicker dtp = (VvDateTimePicker)vvTb.Tag;
         dtp.Visible = true;
         dtp.Focus();
         vvTb.Visible = false;
      }
      else
      {
         vvTb.Focus();
         vvTb.SelectAll();
      }

      foreach(Control ctrl in hampSpecifikum.VvControlToBeParent.Controls)
      {
         if(ctrl is VvTextBox && ctrl != vvTb)
         {
            VvTextBox vvTbox = ctrl as VvTextBox;

            VvHamper.Open_Close_Fields_ForWriting(vvTbox, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
         }
      }
      return rbt;
   }

   #endregion OpenClose_VvTextBoxOnFindDialog

   #region Q's

   private void InitializeDataAdapter()
   {
      TheDataAdapter = new XSqlDataAdapter(TheSelectCommand);
   }

   protected string ColumnNames
   {
      get 
      {
         string extendedColumnNamesList = "";

         if(this is ArtiklListUC) extendedColumnNamesList = ArtiklDao.ArtStatColumnsForArtiklGterecCommand(this as ArtiklListUC);

         if(this is FakturListUC) extendedColumnNamesList = VvSQL.GetAllDataTableColumnNames_4Select(TheDataTable, "ext", IsArhivaTabPage, "ext_");

         if(this is RtransListUC) extendedColumnNamesList = "naziv as ext_kpdbName ";

         return 
            VvSQL.GetAllDataTableColumnNames_4Select(TheDataTable, TheDataTable.TableName, /*false*/"ext_", IsArhivaTabPage) +

            (extendedColumnNamesList.NotEmpty() ? ", " : "") + extendedColumnNamesList; 
      }
   }

   protected void EnableOrDisable_Pagers()
   {
      this.button_prevPage.Enabled = PrevPageExists();
      this.button_nextPage.Enabled = NextPageExists();
   }

   protected bool NextPageExists()
   {
      if(PaggingTurnedOn == false) return false;

      return ((Offset + PageSize < NorrWoLimit));
   }

   protected bool PrevPageExists()
   {
      if(PaggingTurnedOn == false) return false;

      return (Offset /*- PageSize*/ > 0);
   }

   private void button_Go_Prev_Next_Action(object sender, System.EventArgs e)
   {
      Cursor.Current = Cursors.WaitCursor;

      norrDone = false;

      AddFilterMemberz();

      //CiaoBaby
      //if(IsRemoveUnapropriatesNeeded) PaggingTurnedOn = false;

      TheSelectCommand = VvSQL.GTEREC_Command(TheDbConnection,
                                              ColumnNames,
                                              From_IndexSegmentValues,
                                              TheFilterMembers,
                                              asc_or_desc,
                                              recordSorter,
                                              Offset,
                                              PageSize4Command,
                                              false,
                                              IsArhivaTabPage,
                                              this);

      VirtualUntypedDataSet.Clear();
      VirtualUntypedDataSet.SchemaSerializationMode = SchemaSerializationMode.ExcludeSchema;

      // ...sa neke news grupe: <http://tinyurl.com/dlpvo>
      //(bez ovoga seed pamti i ne krece od 1!)
      // ovo mozda ne bu radilo u .NET frameworku 2.0 a Sceppa na nekoj grupi kaze da uzmas tbl.Clone() da sacuvas rigigigi pa vratis na pocetak kada oces ispocetka...

      // puse: ali je dobro znati. 
      //TheDataTable.Columns["rowNum"].AutoIncrementStep = -1;
      //TheDataTable.Columns["rowNum"].AutoIncrementSeed = -1;
      //TheDataTable.Columns["rowNum"].AutoIncrementStep =  1;
      //TheDataTable.Columns["rowNum"].AutoIncrementSeed =  1;

      TheDataAdapter.SelectCommand = TheSelectCommand;


      TheDataAdapter.Fill(TheDataTable/*, "Kplan"*/); // moras dati srcTableName ili ti misli da se zove "Table"! 

      // news: 22.10.2010. 
      //PerformAdditionalDataSetOperation();

      if(this.Parent is VvFindDialog)
      {
         VvFindDialog dlg = this.Parent as VvFindDialog;

         if(SelectedRecID < 1)
         {
            dlg.button_OK.Enabled = dlg.button_Apply.Enabled = false;
            this.button_prevPage.Enabled = this.button_nextPage.Enabled = false;
         }
         else
         {
            dlg.button_OK.Enabled = dlg.button_Apply.Enabled = true;

         } // (SelItem != null)  
      }
      else // ovo je zbog btna "LGO" pod pretpostavkom da je uvijek 1.
      {
         Point xy = this.TheVvTabPage.SubModul_xy;
         if(SelectedRecID < 1)
         {
            ZXC.TheVvForm.atsBtn_SubModulSet[xy.X][xy.Y][0].Enabled = false;
         }
         else
         {
            ZXC.TheVvForm.atsBtn_SubModulSet[xy.X][xy.Y][0].Enabled = true;
         }
      }

      // scrollBar je zapravo offset 
      scrollBarNorrWoLimit.Maximum = (NorrWoLimit - 1);
      scrollBarNorrWoLimit.Minimum = 0;
      scrollBarNorrWoLimit.SmallChange = 1;
      scrollBarNorrWoLimit.LargeChange = PageSize;

      labelMin.Text = String.Format("{0}", scrollBarNorrWoLimit.Minimum + 1);
      labelMax.Text = String.Format("{0}", scrollBarNorrWoLimit.Maximum + 1);
      labelPositionInfo.Text = String.Format("Prikazani red. [ {0} - {1} ]", Offset + 1,
                                 PaggingTurnedOn == false ? scrollBarNorrWoLimit.Maximum + 1 :
                                 Offset + PageSize < NorrWoLimit ? Offset + PageSize : NorrWoLimit);

      EnableOrDisable_Pagers();

      VvDocumentRecordUC.RenumerateLineNumbers(TheGrid);

      Cursor.Current = Cursors.Default;

      DisplayFullFillText(SelectedRowIndex);
   }

   private void DisplayFullFillText(int rowIdx)
   {
      if(rowIdx.IsNegative() || TheFillColumn.Index.IsNegative()) return;

      if(rowIdx > TheGrid.RowCount-1) return;

      Fld_FillColumn = TheGrid[TheFillColumn.Index, rowIdx].Value.ToString();
   }

   public void InitializeStartValue()
   {
      // 19.10.2011: Nono Lorenzo ciao, ma dove che sei.
      // Malo cemo eksperimentirati sto kaze trziste, pa neka odmah na otvaranje FindDialog-a idu neki podaci
    //if(ImaLiIjedan_StartField_Neprazan(hampSpecifikum.VvControlToBeParent))
    //if(true)
      if(ImaLiIjedan_StartField_Neprazan(hampSpecifikum.VvControlToBeParent) || true) // tako da ako ima ijedanNeprazan da ide prema tom fld-u, a ne prema default 
      {
         bool OK_TO_GO = (this is ArtiklListUC                             == false) || 
                         (this as ArtiklListUC).Fld_IsShowSomeOfStatusData == false  || // dakle, NE zelimo PerformClick ukoliko je 'prikazi KolSt' checkiran 
                         ControlForInitialFocus.Text.NotEmpty()                       ; // a ControlForInitialFocus.Text je prazan 
         // 08.04.2018:  
       //             button_GO.PerformClick();
         if(OK_TO_GO) button_GO.PerformClick();

         if(ControlForInitialFocus != null) this.ControlForInitialFocus.Select();

         // 08.04.2018: SvDUH SpeedUP 
         if(ControlForInitialFocus.Text.NotEmpty()) // dosli smo iz Ctrl + Space ili Ctrl +F 
         {
            TheGridFocused();
         }

      }
      else
      {
         if(ControlForInitialFocus != null) this.ControlForInitialFocus.Select();
      }
   }

   #endregion Q's

   #region AddDGV_ArhivaColumns

   protected void AddDGV_ArhivaColumns(ref int sumOfColWidth)
   {
      int colWidth;

      colWidth = ZXC.Q4un; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly(TheGrid, "OrigRecID", colWidth, true, 5, "origRecID");
      colWidth = ZXC.Q2un; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly(TheGrid, "Ver", colWidth, true, 4, "recVer");
      colWidth = ZXC.Q3un + ZXC.Qun2;
      sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "Akcija", colWidth, false, "arAction");
      colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "ArUID", colWidth, false, "arUID");
      colWidth = ZXC.Q7un; sumOfColWidth += colWidth; AddDGVColum_DateTime_4GridReadOnly(TheGrid, "Vrijeme arhive", colWidth, "arTS");
      colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "addUID", colWidth, false, "addUID");
      colWidth = ZXC.Q7un; sumOfColWidth += colWidth; AddDGVColum_DateTime_4GridReadOnly(TheGrid, "addTS (Dodano)", colWidth, "addTS");
      colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(TheGrid, "modUID", colWidth, false, "modUID");
      colWidth = ZXC.Q7un; sumOfColWidth += colWidth; AddDGVColum_DateTime_4GridReadOnly(TheGrid, "modTS (Mijenjano)", colWidth, "modTS");

      //return colWidth;
   }

   #endregion AddDGV_ArhivaColumns

   public virtual void SetListFilterRecordDependentDefaults()
   {

   }

   #endregion Common Methods

   #region overrideVvUC

   public override void GetFields(bool fuse)
   {
      // notin to do;
   }

   #endregion overrideVvUC

   #region implementing Dispose

   private bool alreadyDisposed = false;

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

            TheDataTable.Dispose();
            VirtualUntypedDataSet.Dispose();

            if(NeedsAutoRefresh && AutoRefreshTimer != null) AutoRefreshTimer.Stop();// Dispose();

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

   #endregion implementing Dispose

}

public abstract  class VvDocumRecLstUC : VvRecLstUC
{
   public VvDocumRecLstUC(Control _parent) : base(_parent)
   {
      VvTbx_Virtual_TT.JAM_PairThisWithTwin(VvTbx_VirtualFilter_TT);
   }

   protected abstract VvTextBox VvTbx_Virtual_TT
   {
      get;
   }

   protected abstract VvTextBox VvTbx_VirtualFilter_TT
   {
      get;
      set;
   }

}

public           class VvOtherUC  : VvUserControl
{

   public override void GetFields(bool fuse)
   {
      throw new NotImplementedException();
   }

}
