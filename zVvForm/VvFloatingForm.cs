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
   private Bar dxBar_SubModul;
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

   public Bar DxBar_SubModul
   {
      get { return dxBar_SubModul; }
      set { dxBar_SubModul = value; }
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

   public VvTabPage ActiveTabPage
   {
      get { return detachedContext != null ? detachedContext.SourceTabPage : null; }
   }

   public VvUserControl ActiveUserControl
   {
      get { return detachedContext != null ? detachedContext.HostedUserControl : null; }
   }

   public VvRecordUC ActiveRecordUC
   {
      get
      {
         if(detachedContext == null || detachedContext.HostedUserControl == null) return null;
         return detachedContext.HostedUserControl as VvRecordUC;
      }
   }

   public VvDocumentRecordUC ActiveDocumentRecordUC
   {
      get
      {
         if(detachedContext == null || detachedContext.HostedUserControl == null) return null;
         return detachedContext.HostedUserControl as VvDocumentRecordUC;
      }
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
      VvToolbarFactory.CreateBar_SubModul(this);
      VvToolbarFactory.CreateBar_Report(this);
      PopulateDetachedBars();
      ApplyDetachedWriteMode();
      DxBarManager.ForceInitialize();
   }

   private void ApplyDetachedWriteMode()
   {
      if(detachedContext == null || detachedContext.SourceTabPage == null) return;

      VvToolbarFactory.ApplyWriteMode(this, detachedContext.WriteModeAtDetach);
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

      if(DxBar_SubModul != null)
      {
         BarButtonItem subModulTitleItem = VvToolbarFactory.CreateStaticChildItem(this, "SubModul toolbar", null);
         subModulTitleItem.Enabled = false;
         DxBar_SubModul.AddItem(subModulTitleItem);
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
      string writeModeName = detachedContext.WriteModeAtDetach.ToString();

      if(modulName.IsEmpty()) modulName = "Modul";
      if(subModulName.IsEmpty()) subModulName = detachedContext.Title;
      if(subModulName.IsEmpty()) subModulName = "SubModul";
      if(detachedContext.IsArhivaAtDetach) writeModeName = "Arhiva / " + writeModeName;

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
      hostedUserControl.DocumentHost = this;
      hostedUserControl.TheVvTabPage = detachedContext.SourceTabPage;
      WireActiveHostRouting(hostedUserControl);
      Controls.Add(hostedUserControl);
      Controls.SetChildIndex(hostedUserControl, 0);
      statusStrip.Dock = DockStyle.Bottom;
   }

   private void ReattachContent()
   {
      if(reattached || detachedContext == null) return;

      string recoveryMessage;
      if(!TryReattachContentCore(out recoveryMessage))
      {
         RecoverDetachedContent(recoveryMessage);
      }

      reattached = true;
   }

   private bool TryReattachContentCore(out string recoveryMessage)
   {
      recoveryMessage = null;

      if(!CanReattachContent(out recoveryMessage)) return false;

      try
      {
         ReattachContentCore();
         return true;
      }
      catch(ObjectDisposedException ex)
      {
         recoveryMessage = ex.Message;
         return false;
      }
      catch(InvalidOperationException ex)
      {
         recoveryMessage = ex.Message;
         return false;
      }
      catch(ArgumentException ex)
      {
         recoveryMessage = ex.Message;
         return false;
      }
   }

   private bool CanReattachContent(out string recoveryMessage)
   {
      recoveryMessage = null;

      if(detachedContext == null)
      {
         recoveryMessage = "Detached context nije dostupan.";
         return false;
      }

      if(detachedContext.SourceForm == null || detachedContext.SourceForm.IsDisposed || detachedContext.SourceForm.Disposing)
      {
         recoveryMessage = "Glavna forma više nije dostupna.";
         return false;
      }

      if(detachedContext.SourceTabPage == null || detachedContext.SourceTabPage.IsDisposed || detachedContext.SourceTabPage.Disposing)
      {
         recoveryMessage = "Source tab više nije dostupan.";
         return false;
      }

      if(detachedContext.SourceTabPage.panelZaUC == null || detachedContext.SourceTabPage.panelZaUC.IsDisposed || detachedContext.SourceTabPage.panelZaUC.Disposing)
      {
         recoveryMessage = "Source panel više nije dostupan.";
         return false;
      }

      if(detachedContext.HostedUserControl == null || detachedContext.HostedUserControl.IsDisposed || detachedContext.HostedUserControl.Disposing)
      {
         recoveryMessage = "Detached VvUserControl više nije dostupan.";
         return false;
      }

      return true;
   }

   private void ReattachContentCore()
   {
      VvUserControl hostedUserControl = detachedContext.HostedUserControl;
      hostedUserControl.Parent = null;
      hostedUserControl.Dock = DockStyle.Fill;
      hostedUserControl.DocumentHost = detachedContext.SourceForm;
      hostedUserControl.TheVvTabPage = detachedContext.SourceTabPage;
      UnwireActiveHostRouting(hostedUserControl);
      detachedContext.SourceTabPage.panelZaUC.Controls.Add(hostedUserControl);
      detachedContext.SourceTabPage.IsDetached = false;
      detachedContext.SourceTabPage.Selected = true;
      ZXC.SetActiveDocumentHost(detachedContext.SourceForm);
   }

   private void RecoverDetachedContent(string recoveryMessage)
   {
      VvUserControl hostedUserControl = detachedContext != null ? detachedContext.HostedUserControl : null;

      UnwireActiveHostRouting(hostedUserControl);

      if(hostedUserControl != null && !hostedUserControl.IsDisposed && !hostedUserControl.Disposing)
      {
         Controls.Remove(hostedUserControl);
         hostedUserControl.Dispose();
      }

      if(detachedContext != null && detachedContext.SourceTabPage != null && !detachedContext.SourceTabPage.IsDisposed)
      {
         detachedContext.SourceTabPage.IsDetached = false;
      }

      if(!recoveryMessage.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Detached prozor se zatvara, ali sadržaj nije moguće vratiti u glavni prozor.\n\n" + recoveryMessage);
      }
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

   protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
   {
      ZXC.SetActiveDocumentHost(this);
      return base.ProcessCmdKey(ref msg, keyData);
   }

   protected override void OnFormClosing(FormClosingEventArgs e)
   {
      if(detachedContext != null && !reattached)
      {
         string recoveryMessage;
         if(!TryReattachContentCore(out recoveryMessage))
         {
            RecoverDetachedContent(recoveryMessage);
            reattached = true;
            base.OnFormClosing(e);
            return;
         }

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
