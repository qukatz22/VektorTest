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
   private readonly StatusStrip statusStrip;
   private readonly ToolStripStatusLabel statusLabel;
   private BarManager dxBarManager;
   private Bar dxBar_Record;
   private Bar dxBar_Report;
   private Bar dxMenuBar;

   public VvFloatingForm()
      : this(null)
   {
   }

   public VvFloatingForm(VvTabPage sourceTabPage)
   {
      Text = sourceTabPage != null && sourceTabPage.Title.NotEmpty() ? "Detached tab - " + sourceTabPage.Title : "Detached tab";
      ShowInTaskbar = true;
      StartPosition = FormStartPosition.CenterParent;
      Width = 1024;
      Height = 768;

      statusLabel = new ToolStripStatusLabel();
      statusStrip = new StatusStrip();
      statusStrip.Items.Add(statusLabel);
      Controls.Add(statusStrip);

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

   protected override void OnActivated(EventArgs e)
   {
      base.OnActivated(e);
      ZXC.SetActiveDocumentHost(this);
   }

   protected override void OnFormClosed(FormClosedEventArgs e)
   {
      ZXC.UnregisterDocumentHost(this);
      base.OnFormClosed(e);
   }
}
