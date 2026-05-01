using System;
using System.Collections.Generic;
using System.Data;
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

      Text = detachedContext != null && detachedContext.Title.NotEmpty() ? "Detached tab - " + detachedContext.Title : "Detached tab";
      ShowInTaskbar = true;
      StartPosition = FormStartPosition.CenterParent;
      Width = 1024;
      Height = 768;

      statusLabel = new ToolStripStatusLabel();
      statusStrip = new StatusStrip();
      statusStrip.Items.Add(statusLabel);
      Controls.Add(statusStrip);

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
      internal set { dxBar_Record = value; }
   }

   public Bar DxBar_Report
   {
      get { return dxBar_Report; }
      internal set { dxBar_Report = value; }
   }

   public Bar DxMenuBar
   {
      get { return dxMenuBar; }
      internal set { dxMenuBar = value; }
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

   private void DetachContent()
   {
      VvUserControl hostedUserControl = detachedContext.HostedUserControl;
      hostedUserControl.Parent = null;
      hostedUserControl.Dock = DockStyle.Fill;
      Controls.Add(hostedUserControl);
      Controls.SetChildIndex(hostedUserControl, 0);
      statusStrip.Dock = DockStyle.Bottom;
   }

   private void ReattachContent()
   {
      if(reattached || detachedContext == null) return;

      VvUserControl hostedUserControl = detachedContext.HostedUserControl;
      hostedUserControl.Parent = null;
      hostedUserControl.Dock = DockStyle.Fill;
      detachedContext.SourceTabPage.panelZaUC.Controls.Add(hostedUserControl);
      reattached = true;
   }

   protected override void OnActivated(EventArgs e)
   {
      base.OnActivated(e);
      ZXC.SetActiveDocumentHost(this);
   }

   protected override void OnFormClosed(FormClosedEventArgs e)
   {
      ReattachContent();
      ZXC.UnregisterDocumentHost(this);
      base.OnFormClosed(e);
   }
}
