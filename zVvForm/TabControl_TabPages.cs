using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;

public /*sealed*/ partial class VvForm : DevExpress.XtraEditors.XtraForm
{

   #region InitializeWorkTabControl

   protected void InitializeWorkTabControl()
   {
      // Per V4 §2.1 + §2.2 #1: DocumentManager + TabbedView od poèetka.
      workDocumentManager = new DocumentManager();
      workDocumentManager.ContainerControl = this;

      TabbedView tv = new TabbedView((System.ComponentModel.IContainer)null);
      workDocumentManager.ViewCollection.Add(tv);
      workDocumentManager.View = tv;

      // P3-1: floating gesture je ukljuèen, ali default DX lightweight floating se cancelira u BeginFloating.
      tv.DocumentProperties.AllowFloat = true;
      tv.DocumentProperties.AllowPin   = false;

      // X gumb na tab-u; ekvivalent Crownwood ShowClose dynamic logici
      // (gasi se u ShowCloseAndArrowsDropSelectOnWorkTabControl kad nema open tab-ova).
      tv.DocumentProperties.AllowClose = false;

      // Per V4 §3.2c korak 3 (binding na C20a OnActivated/OnDeactivated metode).
      tv.DocumentActivated   += new DocumentEventHandler(TheTabControl_DocumentActivated);
      tv.DocumentDeactivated += new DocumentEventHandler(TheTabControl_DocumentDeactivated);

      // Per V4 §3.2c korak 4 (Crownwood ClosePressed + Validating arhiva-blokada -> DocumentClosing).
      tv.DocumentClosing += new DocumentCancelEventHandler(TheTabControl_DocumentClosing);
      tv.DocumentClosed  += new DocumentEventHandler      (TheTabControl_DocumentClosed);

      // Per V4 §3.2c korak 4: mouse tab-switch mora zadržati staru Validating arhiva-blokadu.
      tv.TabMouseActivating += new DocumentCancelEventHandler(TheTabControl_TabMouseActivating);

      // P3-1: intercept default DevExpress lightweight floating prije kasnijeg VvFloatingForm reparent flowa.
      tv.BeginFloating += new DocumentCancelEventHandler(TheTabControl_BeginFloating);
   }

   /// <summary>
   /// P3-1 spike: prepoznaj floating gesture, sprijeèi default DX lightweight floating,
   /// i validiraj da event nosi source Document/VvTabPage context.
   /// </summary>
   private void TheTabControl_BeginFloating(object sender, DocumentCancelEventArgs e)
   {
      e.Cancel = true;

      VvTabPage vvTabPage = e.Document != null ? e.Document.Control as VvTabPage : null;
      string title = vvTabPage != null ? vvTabPage.Title : "unknown";
      System.Diagnostics.Debug.WriteLine("P3-3 DETACH preview: BeginFloating intercepted for tab '" + title + "'. Default DX floating cancelled.");

      VvFloatingForm floatingForm = new VvFloatingForm(vvTabPage);
      floatingForm.Show(this);
   }

   private void ShowCloseAndArrowsDropSelectOnWorkTabControl(bool _showCA)
   {
      // X gumb se pojavljuje/skriva globalno preko DocumentProperties.AllowClose.
      // ShowArrows / ShowDropSelect (Crownwood) nemaju 1:1 ekvivalent — TabbedView automatski
      // upravlja overflow strijelama i ima ugraðen document-selector dropdown.
      if(TheTabControl == null) return;
      TheTabControl.DocumentProperties.AllowClose = _showCA;
   }

   /// <summary>
   /// Per V4 §3.2c korak 3: binding-target za TabbedView.DocumentActivated.
   /// Delegira u OnActivated() na VvTabPage (extracted u C20a).
   /// </summary>
   private void TheTabControl_DocumentActivated(object sender, DocumentEventArgs e)
   {
      VvTabPage vvTabPage = e.Document.Control as VvTabPage;
      if(vvTabPage != null && vvTabPage.IsInitializedForActivation) vvTabPage.OnActivated();
   }

   /// <summary>
   /// Per V4 §3.2c korak 3: binding-target za TabbedView.DocumentDeactivated.
   /// Delegira u OnDeactivated() na VvTabPage (extracted u C20a).
   /// </summary>
   private void TheTabControl_DocumentDeactivated(object sender, DocumentEventArgs e)
   {
      VvTabPage vvTabPage = e.Document.Control as VvTabPage;
      if(vvTabPage != null) vvTabPage.OnDeactivated();
   }

   /// <summary>
   /// Per V4 §3.2c korak 4: TabbedView.DocumentClosing apsorbira raniju Crownwood
   /// kombinaciju ClosePressed + VvTabPage_Validating (arhiva-blokada).
   /// e.Cancel = true zaustavlja zatvaranje (npr. unsaved data ili arhiva mode).
   /// </summary>
   private void TheTabControl_DocumentClosing(object sender, DocumentCancelEventArgs e)
   {
      VvTabPage vvTabPage = e.Document.Control as VvTabPage;
      if(vvTabPage == null) return;

      // Arhiva-blokada (preuzeto iz VvTabPage_Validating logike).
      if(vvTabPage.IsArhivaTabPage)
      {
         e.Cancel = true;
         return;
      }

      // Unsaved data dialog — ako user odustane, otkaži zatvaranje.
      if(HasTheTabPageAnyUnsavedData(vvTabPage))
      {
         e.Cancel = true;
         return;
      }
   }

   /// <summary>
   /// Per V4 §3.2c korak 4: TabbedView.TabMouseActivating zadržava staro ponašanje
   /// VvTabPage_Validating kod pokušaja odlaska s taba u arhivi.
   /// </summary>
   private void TheTabControl_TabMouseActivating(object sender, DocumentCancelEventArgs e)
   {
      if(TheTabControl == null || TheTabControl.ActiveDocument == null) return;

      VvTabPage activeTabPage = TheTabControl.ActiveDocument.Control as VvTabPage;
      if(activeTabPage == null) return;

      if(activeTabPage.IsArhivaTabPage)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Izaðite, najprije, iz Arhive.");
         e.Cancel = true;
         return;
      }

   }

   /// <summary>
   /// Post-close cleanup (raniji TheTabControl_ClosePressed body, prilagoðen za DX).
   /// TabbedView je veæ maknuo Document iz Documents kolekcije do trenutka kad ovaj event okida.
   /// </summary>
   private void TheTabControl_DocumentClosed(object sender, DocumentEventArgs e)
   {
      VvTabPage vvTabPage = e.Document.Control as VvTabPage;

      if(vvTabPage != null) vvTabPage.Dispose();

      if(this.TheTabControl.Documents.Count == 0)
      {
         ShowCloseAndArrowsDropSelectOnWorkTabControl(false);
         EnableDisable_VvReportSubModuls_onCmdPanel(false, Point.Empty);
         SetVvMenuEnabledOrDisabled_NoTabPageIsOpened();
         foreach(ToolStrip ts in tsPanel_SubModul.Controls) ts.Visible = false;
         aTopSetSubModul.Visible = false;
      }
      else
      {
         vvTabPage_GotFocus(null, EventArgs.Empty);
      }

      ZXC.FakturRec                      = null;
      ZXC.RISK_CopyToOtherDUC_inProgress = false;
      ZXC.RISK_CopyToMixerDUC_inProgress = false;
      ZXC.OffixImport_InProgress         = false;
   }

   private void CloseTabPage_CtrlW(object sender, EventArgs e)
   {
      if(TheTabControl == null || TheTabControl.ActiveDocument == null) return;
      // Programatski close — prolazi kroz isti DocumentClosing/Closed pipeline.
      TheTabControl.Controller.Close(TheTabControl.ActiveDocument);
   }

   private void CloseAllOpenTabPage(object sender, EventArgs e)
   {
      if(TheTabControl == null) return;

      // Snapshot jer Controller.Close mijenja Documents kolekciju.
      var docs = TheTabControl.Documents.ToArray();
      foreach(BaseDocument d in docs)
      {
         TheTabControl.Controller.Close(d);
      }

      // 11.03.2014: 
      VvUserControl.NullifyAllSifrars();
   }

   


   /// <summary>
   /// Return value odgovara na pitanje 'Da li treba brejknuti ovu operaciju.'
   /// </summary>
   /// <param name="vvTabPage"></param>
   /// <returns></returns>
   public bool HasTheTabPageAnyUnsavedData(VvTabPage vvTabPage)
   {
      if(vvTabPage.HasUnsavedChanges)
      {
         if(!vvTabPage.Selected) vvTabPage.Selected = true;
         DialogResult dr = MessageBox.Show("Da li da spremim podatke " + vvTabPage.Title + " [" + vvTabPage.TheVvDataRecord + "]?", "SPREMITI? (Da/Ne/Odustani)",
                                           MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
         switch(dr)
         {
            case DialogResult.Yes:
               bool validatedAndOK = SaveVvDataRecord(null, EventArgs.Empty); 
               if(validatedAndOK)     return false;
               else                   return true;

            case DialogResult.No:
               if(TheVvTabPage.TabPageKind == ZXC.VvTabPageKindEnum.RECORD_TabPage &&
                  TheVvTabPage.WriteMode   == ZXC.WriteMode.Edit)
               {
                  ReleaseLock();
               }

               return false;

            case DialogResult.Cancel: return true;

            default: ZXC.aim_emsg("DialogResult [{0}] unsupported.", dr.ToString()); return true;
         }
      }
      else // 04.08.2009: ____________________________________________________________________ 
      {
         if(TheVvTabPage.TabPageKind == ZXC.VvTabPageKindEnum.RECORD_TabPage &&
            TheVvTabPage.WriteMode   == ZXC.WriteMode.Edit)
         {
            ReleaseLock();
         }
      }

      return false;
   }

   private void ReleaseLock()
   {
      VvSQL.VvLockerInfo freshFromFile_lockerInfo = new VvSQL.VvLockerInfo(TheVvDataRecord.VirtualRecordName, TheVvDataRecord.VirtualRecID);

      if(TheVvDao.IsInLocker(TheDbConnection, freshFromFile_lockerInfo, true) && // Lock exists AND Lock is mine! 
         TheVvLockerInfo.lockID == freshFromFile_lockerInfo.lockID)
      {
         bool OK = TheVvDao.DeleteFromLocker(TheDbConnection, new VvSQL.VvLockerInfo(TheVvDataRecord.VirtualRecordName, TheVvDataRecord.VirtualRecID));

         if(!OK) ZXC.aim_emsg("Ne mogu odlockirati zapis!");
      }
   }

   #endregion InitializeWorkTabControl

   #region CreateVvTabPage

   private void CreateVvTabPage(Point xy, ZXC.VvTabPageKindEnum _tabPageKind, VvDataRecord initialVvDataRecord, uint? initialDataRecord_RecID, VvRecLstUC initialRecLstUC)
   {
      VvTabPage  vvTabPage;
      VvSubModul vvSubModul = aModuli[xy.X].aSubModuls[xy.Y];
      string     nazivTabPage;

      if(TheVvTabPage != null && TheVvTabPage.IsArhivaTabPage)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Izaðite, najprije, iz Arhive.");
         return;
      }

      if(ZXC.IsTEXTHOshop && vvSubModul.subModulEnum == ZXC.VvSubModulEnum.R_IRM)
      {
          foreach(VvTabPage tp in GetOpenVvTabPages())
         {
            // 08.07.2015: ma, jos stroze! 
          //if(tp.TheVvSubModul.subModulEnum == ZXC.VvSubModulEnum.R_IRM && tp.HasUnsavedChanges)
            if(tp.TheVvSubModul.subModulEnum == ZXC.VvSubModulEnum.R_IRM                        )
            {
             //ZXC.aim_emsg(MessageBoxIcon.Stop, "Ne možete otvarati novi IRM ekran\n\ndok imate prethodno otvoreni IRM ekran sa nespremljenim raèunom.\n\n(IRM TabPage sa crvenim uskliènikom)\n\nVratite se na taj TabPage (ekran) te usnimite raèun ili odustanite sa 'Odustani - Esc'.");
               ZXC.aim_emsg(MessageBoxIcon.Stop, "Ne možete otvarati novi IRM ekran\n\ndok imate prethodno otvoreni.\n\nKoristite onoga veæ otvorenog.");
               return;
            }
         }
      }

      ats_SubModulSet[xy.X][xy.Y].Parent = tsPanel_SubModul;

      if(tsCbxVvDataBase.SelectedItem == null)
      {
         MessageBox.Show("Odaberite, molim, projekt.");
         return;
      }

      if(vvSubModul.subModulEnum == ZXC.VvSubModulEnum.PRJ)
      {
         nazivTabPage = "PROJEKTI";//vvSubModul.subModul_shortName;
      }
      else if(vvSubModul.modulEnum == ZXC.VvModulEnum.MODUL_PRIJEM || vvSubModul.modulEnum == ZXC.VvModulEnum.MODUL_OPRANA)
      {
         nazivTabPage = vvSubModul.subModul_shortName;
      }
      else
      {
         nazivTabPage = vvSubModul.subModul_shortName + " [" + ZXC.TheVvDatabaseInfoIn_ComboBox4Projects.ProjectName + "]";
      }
      /* *                                                                                                                                                */
      /* */
      /* */ vvTabPage = new VvTabPage(this, nazivTabPage, xy, _tabPageKind, TheTabControl, initialVvDataRecord, initialDataRecord_RecID, initialRecLstUC);
      /* */
      /* *                                                                                                                                                */
 
      if(vvTabPage.TheDbConnection == null || vvTabPage.TheDbConnection.State != System.Data.ConnectionState.Open) return;

      if(TheTabControl.ActiveDocument == null) return;

      // preselili u constructor VvTabPage da izbjegnemo kokos/jaje...     vvTabPage.Selected = true;

      ShowCloseAndArrowsDropSelectOnWorkTabControl(true);

      // preselio u constructor vvTabPage-a zbog dbConnection 
      //TheTabControl.TabPages.Add(vvTabPage);


      switch(vvTabPage.TabPageKind)
      {
         case ZXC.VvTabPageKindEnum.RECORD_TabPage:
            SetSorterComboBox();
            break;

         case ZXC.VvTabPageKindEnum.REPORT_TabPage:
            SetReportComboBox((Point)vvTabPage.Tag, 0);
            break;

         case ZXC.VvTabPageKindEnum.RecLIST_TabPage:
            break;
      }

      // 25.11.2009: 
      //vvTabPage.Fld_PrjktNaziv = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects.ProjectName; // TamponPanel_Header
      vvTabPage.Fld_PrjktNaziv = vvTabPage.TheDbConnection.Database; // TamponPanel_Header

      vvTabPage.GotFocus += new EventHandler(vvTabPage_GotFocus); // potreban zbog SorterComboBoxa jerbo se inace izgubi
   }

   private void vvTabPage_GotFocus(object sender, EventArgs e)
   {
      //VvTabPage vvTabPage = sender as VvTabPage;
      //Point xy = (Point)vvTabPage.Tag;

      if(TheVvTabPage.TabPageKind == ZXC.VvTabPageKindEnum.RECORD_TabPage) SetSorterComboBox();

      if(TheVvTabPage.TabPageKind == ZXC.VvTabPageKindEnum.REPORT_TabPage) SetReportComboBox(TheVvTabPage.SubModul_xy, tsCbxReport.SelectedIndex);

      // note: vvTabPage (tj. 'sender') NIJE jednako TheVvTabPage-u u slucaju kada se upravo 'radja' novi tab page sa nekog FindDialog-a     
      // jer onaj TabPage sa kojega je otvaran Find privremeno dobije focus gasenjem find-a, a selectedTabPage od TabControle je vec novi TP 
   }

   private System.Collections.Generic.IEnumerable<VvTabPage> GetOpenVvTabPages()
   {
      if(TheTabControl == null) yield break;

      foreach(BaseDocument document in TheTabControl.Documents)
      {
         VvTabPage vvTabPage = document.Control as VvTabPage;
         if(vvTabPage != null) yield return vvTabPage;
      }
   }

   private int GetActiveVvTabPageIndex()
   {
      if(TheTabControl == null || TheTabControl.ActiveDocument == null) return -1;

      for(int i = 0; i < TheTabControl.Documents.Count; i++)
      {
         if(TheTabControl.Documents[i] == TheTabControl.ActiveDocument) return i;
      }

      return -1;
   }

   private bool IsTabControlPositionTop
   {
      get { return TheTabControl != null && TheTabControl.DocumentGroupProperties.HeaderLocation == DevExpress.XtraTab.TabHeaderLocation.Top; }
      set
      {
         if(TheTabControl == null) return;
         TheTabControl.DocumentGroupProperties.HeaderLocation = value ? DevExpress.XtraTab.TabHeaderLocation.Top : DevExpress.XtraTab.TabHeaderLocation.Bottom;
      }
   }

   #endregion CreateVvTabPage

   #region Open New Record/Report/RecList TabPage

   public void OpenNew_Record_TabPage(Point xy, uint? initialDataRecord_RecID)
   {
      CreateVvTabPage(xy, ZXC.VvTabPageKindEnum.RECORD_TabPage, null, initialDataRecord_RecID, null);
   }

   public void OpenNew_Record_TabPage_wInitialRecord(Point xy, VvDataRecord initialVvDataRecord)
   {
      CreateVvTabPage(xy, ZXC.VvTabPageKindEnum.RECORD_TabPage, initialVvDataRecord, null, null);
   }

   public void OpenNew_RecLst_TabPage(Point xy, VvRecLstUC recLstUC)
   {
      CreateVvTabPage(xy, ZXC.VvTabPageKindEnum.RecLIST_TabPage, null, null, recLstUC);
   }

   private void OpenNew_Report_TabPage(Point xy)
   {
      SetVisibilitiOfReportModulButton(xy);

      CreateVvTabPage(xy, ZXC.VvTabPageKindEnum.REPORT_TabPage, null, null, null);

      VvFormResize();

   }

   public void OpenNew_Other_TabPage(Point xy)
   {
      CreateVvTabPage(xy, ZXC.VvTabPageKindEnum.OTHER_TabPage, null, null, null);
   }

   public void SetVisibilitiOfReportModulButton(Point xy)
   {
      VvSubModul subModul = GetVvSubModulFrom_PointXY(xy);

      for(int z = 0; z < subModul.aRptSubModuls.Length; z++)
      {
         aReportModulButton[xy.X][xy.Y][z].Visible = true;

         aModulPanel[xy.X].Size = new Size(modulPanel.Width - 2 * margin4modul,
                                           aModuli[xy.X].aSubModuls.Length * aSubModulButton[xy.X][xy.Y].Height
                                           + subModul.aRptSubModuls.Length * aReportModulButton[xy.X][xy.Y][z].Height);

      }
      
      OrganizeModulButtons(xy.X, true);

#region Tree
      ExpandTreeViewModulReportNode(xy);
      //da li da druge Colapsam
#endregion Tree

   }

   #endregion OpenNew_Record_Report_TabPage

   #region SubModulSet_EnableOrDisable_TSButtonsAndTSMnItems

   public void SubModulSet_EnableOrDisable_TSButtonsAndTSMnItems()
   {
      ZXC.VvSubModulEnum vvModul = TheVvTabPage.TheVvSubModul.subModulEnum;

      switch(vvModul)
      {
         case ZXC.VvSubModulEnum.PRJ: Prjkt_SubModul_EnableOrDisable_TSButtonsAndTSMnItems(); 
                                      break;

         //default                 : ZXC.aim_emsg("POI: VvSubModulEnum {0} still undone in SubModulSet_EnableOrDisable_TSButtonsAndTSMnItems()", vvModul);
         //                          break;
      }
   }

   #endregion SubModulSet_EnableOrDisable_TSButtonsAndTSMnItems
   
   #region AnySubModulSet_EnableOrDisable_TSButtonsAndTSMnItems

   #region Prjkt_SubModul_EnableOrDisable_TSButtonsAndTSMnItems\

   public void Prjkt_SubModul_EnableOrDisable_TSButtonsAndTSMnItems()
   {
      Point xy     = (Point)TheVvTabPage.Tag;
      ToolStrip ts = ats_SubModulSet[xy.X][xy.Y];

      bool shouldDisableCreationButton;

      shouldDisableCreationButton =
        VvSQL.CHECK_DATABASE_EXISTS(TheVvTabPage.TheDbConnection, ZXC.VvDB_NameConstructor(ZXC.projectYear, ((Prjkt)TheVvDataRecord).Ticker, ((Prjkt)TheVvDataRecord).KupdobCD/*RecID*/));

      SetEnabled_SubModulSet_JOB(ts.Items["ceateDB"], !shouldDisableCreationButton);
      SetEnabled_SubModulSet_JOB(ts.Items["activDB"], shouldDisableCreationButton);
      if(ZXC.ThisIsVektorProject)
      {
         SetEnabled_SubModulSet_JOB(ts.Items["initNY"], shouldDisableCreationButton);
         SetEnabled_SubModulSet_JOB(ts.Items["amortNY"], shouldDisableCreationButton);
      }
   }

   #endregion Prjkt_SubModul_EnableOrDisable_TSButtonsAndTSMnItems

   #region SetEnabled_SubModulSet_JOB

   private void SetEnabled_SubModulSet_JOB(object item, bool enabled)
   {
      if(item.GetType() != typeof(ToolStripButton)) return;

      ToolStripButton tsb = (ToolStripButton)item;
     // ZXC.Koordinata_3D xyz = (ZXC.Koordinata_3D)tsb.Tag;
      ToolStripMenuItem mi = (ToolStripMenuItem)tsb.Tag;
      tsb.Enabled = mi.Enabled = enabled;
      //atsBtn_SubModulSet[xyz.X][xyz.Y][xyz.Z].Enabled = aSubModulSet_menuItem[xyz.X][xyz.Y][xyz.Z].Enabled = enabled;
   }

   #endregion SetEnabled_SubModulSet_JOB

   #endregion AnySubModulSet_EnableOrDisable_TSButtonsAndTSMnItems

   #region Report_SubModul_EnableOrDisable_TSButtonsAndTSMnItems VisibitiTs_Report

   public void Report_SubModul_EnableOrDisable_TSButtonsAndTSMnItems(ZXC.ReportMode reportMode, bool isReportTabPage)
   {
      ToolStrip ts = ts_Report;

      if(isReportTabPage) SetEnabled_ProgresBarAnd_DropDowns(reportMode);

      SetEnabled_ReportControls_JOB(ts.Items["GO"]          , (reportMode == ZXC.ReportMode.Working ||
                                                               reportMode == ZXC.ReportMode.Canceling) ? false : true);
      SetEnabled_ReportControls_JOB(ts.Items["Cancel"]      ,  reportMode == ZXC.ReportMode.Working    ? true : false);
      
      SetEnabled_ReportControls_JOB(ts.Items["Print"]       , (reportMode == ZXC.ReportMode.Done || 
                                                                                     !isReportTabPage) ? true : false);
      SetEnabled_ReportControls_JOB(ts.Items["Export"]      , (reportMode == ZXC.ReportMode.Done || 
                                                                                     !isReportTabPage) ? true : false);
      SetEnabled_ReportControls_JOB(ts.Items["PDF"]         , (reportMode == ZXC.ReportMode.Done || 
                                                                                     !isReportTabPage) ? true : false);
    // 15.09.2016.
    //SetEnabled_ReportControls_JOB(ts.Items["SgnPDF"]      , (reportMode == ZXC.ReportMode.Done || 
    //                                                                               !isReportTabPage) ? true : false);
    //SetEnabled_ReportControls_JOB(ts.Items["SgnPDFVisibl"], (reportMode == ZXC.ReportMode.Done || 
    //                                                                               !isReportTabPage) ? true : false);
      SetEnabled_ReportControls_JOB(ts.Items["PDFnaMail"]   , (reportMode == ZXC.ReportMode.Done ||
                                                                                     !isReportTabPage) ? true : false);
      SetEnabled_ReportControls_JOB(ts.Items["PDFManager"]  , (reportMode == ZXC.ReportMode.Done ||
                                                                                     !isReportTabPage) ? true : false);
      SetEnabled_ReportControls_JOB(ts.Items["Frst"]        , (reportMode == ZXC.ReportMode.Done || 
                                                                                     !isReportTabPage) ? true : false);
      SetEnabled_ReportControls_JOB(ts.Items["Prev"]        , (reportMode == ZXC.ReportMode.Done || 
                                                                                     !isReportTabPage) ? true : false);
      SetEnabled_ReportControls_JOB(ts.Items["Next"]        , (reportMode == ZXC.ReportMode.Done || 
                                                                                     !isReportTabPage) ? true : false);
      SetEnabled_ReportControls_JOB(ts.Items["Last"]        , (reportMode == ZXC.ReportMode.Done || 
                                                                                     !isReportTabPage) ? true : false);
      SetEnabled_ReportControls_JOB(ts.Items["Zoom100"]     , (reportMode == ZXC.ReportMode.Done || 
                                                                                     !isReportTabPage) ? true : false);
      SetEnabled_ReportControls_JOB(ts.Items["PageWidth"]   , (reportMode == ZXC.ReportMode.Done || 
                                                                                     !isReportTabPage) ? true : false);
      SetEnabled_ReportControls_JOB(ts.Items["WholePage"]   , (reportMode == ZXC.ReportMode.Done || 
                                                                                     !isReportTabPage) ? true : false);
      SetEnabled_ReportControls_JOB(ts.Items["GroupTree"]   , (reportMode == ZXC.ReportMode.Done || 
                                                                                     !isReportTabPage) ? true : false);
      SetEnabled_ReportControls_JOB(ts.Items["FindPage"]    , (reportMode == ZXC.ReportMode.Done || 
                                                                                     !isReportTabPage) ? true : false);
      SetEnabled_ReportControls_JOB(ts.Items["FindText"]    , (reportMode == ZXC.ReportMode.Done || 
                                                                                     !isReportTabPage) ? true : false);
      SetEnabled_ReportControls_JOB(ts.Items["ColseCurView"], (reportMode == ZXC.ReportMode.Done ||
                                                                                     !isReportTabPage) ? true : false);
      if(TheVvUC is VirmanDUC)
      {
         SetEnabled_ReportControls_JOB(ts.Items["ToTXTorXML"], true);
      }
      // 18.06.2019: za eRacun umetnut ovaj else if 
      else if(TheVvUC is FakturDUC && TheVvRecordUC.TheVvReport.IsForExport)
      {
         SetEnabled_ReportControls_JOB(ts.Items["ToTXTorXML"], true);
      }
      else
      {
       //SetEnabled_ReportControls_JOB(ts.Items["ToTXTorXML"], (reportMode == ZXC.ReportMode.Done && isReportTabPage &&                          TheVvReportUC.TheVvReport != null)
         SetEnabled_ReportControls_JOB(ts.Items["ToTXTorXML"], (reportMode == ZXC.ReportMode.Done && isReportTabPage && TheVvReportUC != null && TheVvReportUC.TheVvReport != null)
            ?
            TheVvReportUC.TheVvReport.IsForExport :
            false);
      }

      zoomButton_OnReport.Enabled = mi_ReportZoom.Enabled = ((reportMode == ZXC.ReportMode.Done ||
                                                                                     !isReportTabPage) ? true : false);

   }

   private void SetEnabled_ProgresBarAnd_DropDowns(ZXC.ReportMode reportMode)
   {

      if(ZXC.TheVvForm.numberOfRunningWorkers > 0 ||
             reportMode == ZXC.ReportMode.Working ||
             reportMode == ZXC.ReportMode.Canceling)
      {
         progressBar.Visible = true;
      }
      else
      {
         progressBar.Visible = false;
      }

      if(reportMode == ZXC.ReportMode.Working) progressBar.MarqueeAnimationSpeed = 50;

      tsCbxReport.Enabled = (reportMode == ZXC.ReportMode.Working ||
                             reportMode == ZXC.ReportMode.Canceling ? false : true);

      // Dear Tamara, ovo dole dize Exception kada cekas kraj izvjestaja na nekim drugom Tabpage-u 
      // if (reportMode == ZXC.ReportMode.Done || reportMode == ZXC.ReportMode.Canceling) TheVvReportUC.Label_reportInProgressMessage.Visible = false;
      // metoda dolje ispravlja ovo gore + linija 247 na VvTabPage
   }

   //private void SetLabel_reportInProgressMessageVisible(ZXC.ReportMode reportMode)
   //{
   //   if(reportMode == ZXC.ReportMode.Done || reportMode == ZXC.ReportMode.Canceling) TheVvReportUC.Label_reportInProgressMessage.Visible = false;
   //}

   /*private*/internal void SetEnabled_ReportControls_JOB(object item, bool enabled)
   {
      if(item.GetType() != typeof(ToolStripButton)) return;

      ToolStripButton tsb = (ToolStripButton)item;
      Point xy            = (Point)tsb.Tag;

      atsBtn_RecordRep[xy.X][xy.Y].Enabled = aSubTopMenuItem[xy.X][xy.Y].Enabled = enabled;
   }

   public void ToolStripReportVisible_tsBtnOnTsReportVisible(bool isOnReportUC, ZXC.ReportMode reportMode, bool openVisible)
   {
      ts_Report.Items["Open"].Visible            = openVisible;
      reportTopMenuItem.DropDownItems["Open"].Visible = false;

      ts_Report.Items["GO"].             Visible =
      ts_Report.Items["Cancel"].         Visible =
      ts_Report.Items["tsCbxReport"].       Visible =
      reportTopMenuItem.DropDownItems["GO"].Visible =
      reportTopMenuItem.DropDownItems["Cancel"].Visible = isOnReportUC;


      Report_SubModul_EnableOrDisable_TSButtonsAndTSMnItems(reportMode, isOnReportUC);
   }

   #endregion Report_SubModul_EnableOrDisable_TSButtonsAndTSMnItems

   #region VisibiltiOfTsReportAndTsSubModulSet
   
   public void VisibiltiOfTsReportAndTsSubModulSet(Point xy, bool isReportViewerTabPage)
   {
      ts_Report.        Visible = isReportViewerTabPage;
      reportTopMenuItem.Visible = isReportViewerTabPage;

      ats_SubModulSet[xy.X][xy.Y].         Visible = !isReportViewerTabPage;
      aTopMenuItem_SubModulSet[xy.X][xy.Y].Visible = !isReportViewerTabPage;

      if(isReportViewerTabPage)
      {
         ts_Report.                  Dock = DockStyle.Left;
         ats_SubModulSet[xy.X][xy.Y].Dock = DockStyle.Right;
         ts_Report.Location = new Point(3, 0); // tamtam 30.9 provremno rj da ne plese/ 25.1.2010.
      }
      else
      {
         ats_SubModulSet[xy.X][xy.Y].Dock = DockStyle.Left;
         ts_Report.                  Dock = DockStyle.Right;
         ats_SubModulSet[xy.X][xy.Y].Location = new Point(3, 0); // tamtam 30.9 provremno rj da ne plese/ 25.1.2010.

      }
   }

   #endregion VisibiltiOfTsReportAndTsSubModulSet
}
