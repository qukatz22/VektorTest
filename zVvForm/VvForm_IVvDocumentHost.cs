// =============================================================================
// VvForm_IVvDocumentHost.cs
//
// Faza 1b (C8): VvForm implementira IVvDocumentHost.
//
// Vecinom explicit interface implementation koji delegira na postojece publicne
// memb ere (menuStrip, ts_Record, ts_Report, ats_SubModulSet, TStripStatusLabel,
// TheDbConnection). Postojeci call-site-ovi koriste izravno ime polja ili property-a —
// njih NE diramo u Fazi 1. Kroz interface tok ide samo kada se ide preko
// ZXC.ActiveDocumentHost (npr. delegati statusa, buduce factory metode).
// =============================================================================

using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.XtraBars;

#if MICROSOFT
using XSqlConnection = System.Data.SqlClient.SqlConnection;
#else
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
#endif

public partial class VvForm : IVvDocumentHost
{
   private BarManager dxBarManager;
   private Bar dxBar_Record;
   private Bar dxBar_Report;
   private Bar dxBar_SubModul;
   private Bar dxMenuBar;
   private Dictionary<string, BarItem> dxBarItemsByName;

   // ---- Toolbarovi / meni (Faza 2g: retype to DevExpress.XtraBars.Bar) ----

   ToolStrip        IVvDocumentHost.Bar_Record       { get { return ts_Record;       } }
   ToolStrip        IVvDocumentHost.Bar_Report       { get { return ts_Report;       } }
   ToolStrip[][]    IVvDocumentHost.Bars_SubModul    { get { return ats_SubModulSet; } }
   MenuStrip        IVvDocumentHost.TheMenuStrip     { get { return menuStrip;       } }

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
      get
      {
         if(dxBarItemsByName == null)
         {
            dxBarItemsByName = new Dictionary<string, BarItem>();
         }

         return dxBarItemsByName;
      }
   }

   // TStripStatusLabel vec postoji kao public property → implicitna implementacija.
   // TheDbConnection   vec postoji kao public property → implicitna implementacija.

   // ---- Akcije (status bar) ----

   void IVvDocumentHost.SetStatusText(string text)
   {
      if (TStripStatusLabel == null) return;
      TStripStatusLabel.Text = text;
      TStripStatusLabel.Invalidate();
      TStripStatusLabel.Owner?.Update();
      TStripStatusLabel.Owner?.Refresh();
   }

   void IVvDocumentHost.ClearStatusText()
   {
      if (TStripStatusLabel == null) return;
      TStripStatusLabel.Text = "";
   }

   // ---- Form pristup ----

   Form IVvDocumentHost.AsForm { get { return this; } }

   // ---- Per-host state (Faza 1e / C15) ----

   private readonly VvPerHostState _perHost = new VvPerHostState();
   VvPerHostState IVvDocumentHost.PerHost { get { return _perHost; } }
}
