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

#if MICROSOFT
using XSqlConnection = System.Data.SqlClient.SqlConnection;
#else
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
#endif

public partial class VvForm : IVvDocumentHost
{
   // ---- Toolbarovi / meni (Faza 2g: retype to DevExpress.XtraBars.Bar) ----

   ToolStrip        IVvDocumentHost.Bar_Record       { get { return ts_Record;       } }
   ToolStrip        IVvDocumentHost.Bar_Report       { get { return ts_Report;       } }
   ToolStrip[][]    IVvDocumentHost.Bars_SubModul    { get { return ats_SubModulSet; } }
   MenuStrip        IVvDocumentHost.TheMenuStrip     { get { return menuStrip;       } }

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
}
