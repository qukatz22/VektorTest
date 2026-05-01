using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;

#if MICROSOFT
using XSqlConnection = System.Data.SqlClient.SqlConnection;
#else
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
#endif

/// <summary>
/// Phase 3 DETACH top-level host skeleton. Full tab reparenting is added in later P3 slices.
/// </summary>
internal sealed class VvFloatingForm : XtraForm, IVvDocumentHost
{
   private readonly VvPerHostState perHost = new VvPerHostState();
   private readonly Dictionary<string, BarItem> dxBarItemsByName = new Dictionary<string, BarItem>();
   private readonly VvDetachedDocumentContext detachedContext;
   private readonly StatusStrip statusStrip;
   private readonly ToolStripStatusLabel statusLabel;
   private bool reattached;
   private BarManager dxBarManager;
   private Bar dxBar_Record;
   private Bar dxBar_Report;
   private Bar dxMenuBar;

   public bool IsReattached
   {
      get { return reattached; }
   }

   public VvFloatingForm()
      : this((VvDetachedDocumentContext)null)
   {
   }

   public VvFloatingForm(VvTabPage sourceTabPage)
      : this(sourceTabPage != null ? new VvDetachedDocumentContext(sourceTabPage.TheVvForm, sourceTabPage) : null)
   {
   }

   public VvFloatingForm(VvDetachedDocumentContext detachedContext)
   {
      this.detachedContext = detachedContext;

      Text = GetDetachedTitle(detachedContext);
      ShowInTaskbar = true;
      StartPosition = FormStartPosition.CenterParent;
      Width = 1024;
      Height = 768;
      Icon = GetDetachedIcon(detachedContext);

      statusLabel = new ToolStripStatusLabel();
      statusStrip = new StatusStrip();
      statusStrip.Items.Add(statusLabel);
      Controls.Add(statusStrip);

      InitializeDetachedBars();

      if(detachedContext != null)
      {
         DetachContent();
      }

      ZXC.RegisterDocumentHost(this);
   }

   ToolStrip IVvDocumentHost.Bar_Record { get { return null; } }
   ToolStrip IVvDocumentHost.Bar_Report { get { return null; } }
   ToolStrip[][] IVvDocumentHost.Bars_SubModul { get { return null; } }
   MenuStrip IVvDocumentHost.TheMenuStrip { get { return null; } }

   public BarManager DxBarManager
   {
      get
      {
         if(dxBarManager == null)
         {
            dxBarManager = new BarManager();
            dxBarManager.Form = this;
         }

         return dxBarManager;
      }
   }

   public Bar DxBar_Record
   {
      get { return dxBar_Record; }
      set { dxBar_Record = value; }
   }

   public Bar DxBar_Report
   {
      get { return dxBar_Report; }
      set { dxBar_Report = value; }
   }

   public Bar DxMenuBar
   {
      get { return dxMenuBar; }
      set { dxMenuBar = value; }
   }

   public Dictionary<string, BarItem> DxBarItemsByName
   {
      get { return dxBarItemsByName; }
   }

   public ToolStripStatusLabel TStripStatusLabel
   {
      get { return statusLabel; }
   }

   public XSqlConnection TheDbConnection
   {
      get { return ZXC.TheVvForm != null ? ZXC.TheVvForm.TheDbConnection : null; }
   }

   public VvPerHostState PerHost
   {
      get { return perHost; }
   }

   Form IVvDocumentHost.AsForm { get { return this; } }

   void IVvDocumentHost.SetStatusText(string text)
   {
      statusLabel.Text = text;
      statusLabel.Invalidate();
      statusLabel.Owner.Update();
      statusLabel.Owner.Refresh();
   }

   void IVvDocumentHost.ClearStatusText()
   {
      statusLabel.Text = "";
   }

   private void InitializeDetachedBars()
   {
      VvToolbarFactory.CreateMenuBar(this, true);
      VvToolbarFactory.CreateBar_Record(this);
      VvToolbarFactory.CreateBar_Report(this);
      PopulateDetachedBars();
      DxBarManager.ForceInitialize();
   }

   private void PopulateDetachedBars()
   {
      BarButtonItem closeItem = VvToolbarFactory.CreateStaticChildItem(this, "Zatvori detached tab", DetachedCloseItem_Click);
      BarButtonItem titleItem = VvToolbarFactory.CreateStaticChildItem(this, Text, null);
      titleItem.Enabled = false;

      if(DxMenuBar != null)
      {
         DxMenuBar.AddItem(closeItem);
      }

      if(DxBar_Record != null)
      {
         DxBar_Record.AddItem(titleItem);
      }
   }

   private static string GetDetachedTitle(VvDetachedDocumentContext detachedContext)
   {
      if(detachedContext == null || detachedContext.SourceTabPage == null)
      {
         return "Vektor — detached tab";
      }

      VvTabPage sourceTabPage = detachedContext.SourceTabPage;
      string modulName = GetModulName(detachedContext.SourceForm, sourceTabPage.TheVvSubModul.modulEnum);
      string subModulName = sourceTabPage.TheVvSubModul.subModul_name;
      string writeModeName = sourceTabPage.WriteMode.ToString();

      if(modulName.IsEmpty()) modulName = "Modul";
      if(subModulName.IsEmpty()) subModulName = detachedContext.Title;
      if(subModulName.IsEmpty()) subModulName = "SubModul";

      return string.Format("Vektor — {0} / {1} — {2}", modulName, subModulName, writeModeName);
   }

   private static string GetModulName(VvForm sourceForm, ZXC.VvModulEnum modulEnum)
   {
      if(sourceForm == null || sourceForm.aModuli == null) return string.Empty;

      VvForm.VvModul modul = sourceForm.aModuli.FirstOrDefault(vvModul => vvModul.modulEnum == modulEnum);

      return modul.modul_name;
   }

   private static Icon GetDetachedIcon(VvDetachedDocumentContext detachedContext)
   {
      if(detachedContext != null && detachedContext.SourceForm != null && detachedContext.SourceForm.Icon != null)
      {
         return detachedContext.SourceForm.Icon;
      }

      return ZXC.TheVvForm != null ? ZXC.TheVvForm.Icon : null;
   }

   private void DetachedCloseItem_Click(object sender, EventArgs e)
   {
      Close();
   }

   private void DetachContent()
   {
      VvUserControl hostedUserControl = detachedContext.HostedUserControl;
      detachedContext.SourceTabPage.IsDetached = true;
      hostedUserControl.Parent = null;
      hostedUserControl.Dock = DockStyle.Fill;
      WireActiveHostRouting(hostedUserControl);
      Controls.Add(hostedUserControl);
      Controls.SetChildIndex(hostedUserControl, 0);
      statusStrip.Dock = DockStyle.Bottom;
   }

   private void ReattachContent()
   {
      if(reattached || detachedContext == null) return;

      ReattachContentCore();
      reattached = true;
   }

   private void ReattachContentCore()
   {
      VvUserControl hostedUserControl = detachedContext.HostedUserControl;
      hostedUserControl.Parent = null;
      hostedUserControl.Dock = DockStyle.Fill;
      UnwireActiveHostRouting(hostedUserControl);
      detachedContext.SourceTabPage.panelZaUC.Controls.Add(hostedUserControl);
      detachedContext.SourceTabPage.IsDetached = false;
      detachedContext.SourceTabPage.Selected = true;
      ZXC.SetActiveDocumentHost(detachedContext.SourceForm);
   }

   private void WireActiveHostRouting(Control control)
   {
      if(control == null) return;

      control.Enter += DetachedControl_ActivateHost;
      control.GotFocus += DetachedControl_ActivateHost;
      control.MouseDown += DetachedControl_ActivateHost;

      foreach(Control child in control.Controls)
      {
         WireActiveHostRouting(child);
      }
   }

   private void UnwireActiveHostRouting(Control control)
   {
      if(control == null) return;

      control.Enter -= DetachedControl_ActivateHost;
      control.GotFocus -= DetachedControl_ActivateHost;
      control.MouseDown -= DetachedControl_ActivateHost;

      foreach(Control child in control.Controls)
      {
         UnwireActiveHostRouting(child);
      }
   }

   private void DetachedControl_ActivateHost(object sender, EventArgs e)
   {
      ZXC.SetActiveDocumentHost(this);
   }

   protected override void OnActivated(EventArgs e)
   {
      base.OnActivated(e);
      ZXC.SetActiveDocumentHost(this);
   }

   protected override void OnFormClosing(FormClosingEventArgs e)
   {
      if(detachedContext != null && !reattached)
      {
         ReattachContentCore();

         if(detachedContext.ShouldCancelClose())
         {
            e.Cancel = true;
            detachedContext.SourceTabPage.IsDetached = true;
            DetachContent();
            ZXC.SetActiveDocumentHost(this);
            base.OnFormClosing(e);
            return;
         }

         reattached = true;
      }

      base.OnFormClosing(e);
   }

   protected override void OnFormClosed(FormClosedEventArgs e)
   {
      ZXC.UnregisterDocumentHost(this);
      base.OnFormClosed(e);
   }
}
