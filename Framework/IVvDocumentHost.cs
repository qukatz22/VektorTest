// =============================================================================
// IVvDocumentHost.cs
//
// Faza 1b (C8): Apstrakcija host-a dokumenta (glavna VvForm singleton, kasnije i
// per-detached VvFloatingForm u Fazi 3).
//
// Cilj: business layer i VvUserControl-i vise ne smiju gadjati ZXC.TheVvForm
// direktno — sve potrebne operacije (menu/toolbar enable, status bar, DB konekcija,
// WriteMode) ruti ju se kroz ovaj interface.
//
// NAPOMENA o tipovima (Faza 1 vs Faza 2g):
//   - U Fazi 1 members su System.Windows.Forms tipovi (ToolStrip / MenuStrip /
//     ToolStripStatusLabel) jer je Crownwood jos uvijek na mjestu, a MenuStrip i
//     ToolStrip-ovi su vanilla WinForms kontrole.
//   - U Fazi 2g (SWAP na DevExpress) ovi se retypiraju u DevExpress.XtraBars.Bar
//     / BarManager / BarStaticItem. Tada ce se i svi implementori (VvForm, kasnije
//     VvFloatingForm) promijeniti u jednom koraku.
//   - TheTabControl se NAMJERNO NE izlaze kroz interface — u Fazi 2c zamjenjujemo
//     Crownwood.TabControl → DocumentManager+TabbedView, a potrosaci interface-a
//     ne bi smjeli ovisiti o konkretnom tab modelu.
// =============================================================================

using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.XtraBars;

#if MICROSOFT
using XSqlConnection = System.Data.SqlClient.SqlConnection;
#else
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
#endif

/// <summary>
/// Apstrakcija host-a dokumenta (glavna VvForm singleton, kasnije i per-detached
/// VvFloatingForm u Fazi 3). Vidi MarkDowns\DevExpress_Migration_V4.md §2.3, §3.1b.
/// </summary>
public interface IVvDocumentHost
{
   // ---- Toolbarovi / meni (Faza 2g: retype to DevExpress.XtraBars.Bar) ----

   /// <summary>Bar s CRUD+nav gumbima (NEW, OPN, DEL, SAV, ESC, FRS, PRV, NXT, LST, FND, PRN, PRW, ARH, ...).</summary>
   ToolStrip Bar_Record { get; }

   /// <summary>Bar s report ops (GO, Print, PDF, Export, Zoom, page nav).</summary>
   ToolStrip Bar_Report { get; }

   /// <summary>2D polje per-SubModul toolbarova.</summary>
   ToolStrip[][] Bars_SubModul { get; }

   /// <summary>Glavni meni forme.</summary>
   MenuStrip TheMenuStrip { get; }

   /// <summary>DevExpress manager za meni i toolbarove host-a (Faza 2g).</summary>
   BarManager DxBarManager { get; }

   /// <summary>DevExpress Bar_Record placeholder za CRUD+nav gumbe (Faza 2g).</summary>
   Bar DxBar_Record { get; set; }

   /// <summary>DevExpress Bar_Report placeholder za report operacije (Faza 2g).</summary>
   Bar DxBar_Report { get; set; }

   /// <summary>DevExpress Bar_SubModul placeholder za per-SubModul operacije (Faza 2g / Faza 3).</summary>
   Bar DxBar_SubModul { get; set; }

   /// <summary>DevExpress menu bar placeholder glavnog menija (Faza 2g).</summary>
   Bar DxMenuBar { get; set; }

   /// <summary>Lookup DX itema po legacy ToolStrip/MenuItem imenu (Faza 2g).</summary>
   Dictionary<string, BarItem> DxBarItemsByName { get; }

   /// <summary>Status label na status stripu.</summary>
   ToolStripStatusLabel TStripStatusLabel { get; }

   // ---- State ----

   /// <summary>DB konekcija aktivnog taba / host-a.</summary>
   XSqlConnection TheDbConnection { get; }

   /// <summary>Aktivni VvTabPage u ovom host-u, ako host trenutno prikazuje VvTabPage.</summary>
   VvTabPage ActiveTabPage { get; }

   /// <summary>Aktivni VvUserControl u ovom host-u, ako host trenutno prikazuje VvUserControl.</summary>
   VvUserControl ActiveUserControl { get; }

   /// <summary>Aktivni VvRecordUC u ovom host-u, ako host trenutno prikazuje record tab.</summary>
   VvRecordUC ActiveRecordUC { get; }

   /// <summary>Aktivni VvDocumentRecordUC u ovom host-u, ako host trenutno prikazuje document record tab.</summary>
   VvDocumentRecordUC ActiveDocumentRecordUC { get; }

   // ---- Akcije ----

   /// <summary>Upisi tekst u status bar ovog host-a.</summary>
   void SetStatusText(string text);

   /// <summary>Ocisti status bar ovog host-a.</summary>
   void ClearStatusText();

   // ---- Form pristup ----

   /// <summary>Host kao WinForms Form (za MessageBox owner, ShowDialog, Invoke, ...).</summary>
   Form AsForm { get; }

   // ---- Per-host state (Faza 1e / C15) ----

   /// <summary>
   /// Per-host flag bucket (§1.14, §3.1e). Dormant kontrakt u Fazi 1 — tijela
   /// flagova i dalje zive u ZXC staticima; u Fazi 3 (VvFloatingForm) call-siteovi
   /// flipaju s ZXC.FlagName na host.PerHost.FlagName.
   /// </summary>
   VvPerHostState PerHost { get; }
}
