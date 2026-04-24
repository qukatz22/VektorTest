// =============================================================================
// VvToolbarFactory.cs
//
// Faza 1b (C9): Skeleton static klase za buduce kreiranje i enable/disable
// menija + toolbarova na `IVvDocumentHost`-u.
//
// Stanje u Fazi 1 (C9):
//   - Samo signature-stubovi. Implementacije ostaju NotImplementedException.
//   - `ApplyWriteMode` ce u C10 preuzeti tijelo iz
//     `VvForm.SetVvMenuEnabledOrDisabled_RegardingWriteMode_JOB` (bit-identicno).
//   - `Create*` metode ostaju prazne do Faze 2g kada gradimo DevExpress `Bar`
//     objekte iz postojecih `VvMenu[]` struktura (§1.5 V4).
//
// Zasto stubovi vec sad:
//   - Svaki buduci call-site koji treba enable/disable gumba mogao bi ODMAH
//     pozvati `VvToolbarFactory.ApplyWriteMode(host, wm)` kao indirect wrapper
//     oko postojece `VvForm.SetVvMenuEnabledOrDisabled_RegardingWriteMode`.
//     Time u Fazi 2g kada target prelazi s `ToolStripItem` na DX `BarButtonItem`,
//     mijenjamo SAMO tijelo factory metoda — ne i call-siteove.
//   - Vidi V4 §3.1b i §1.6.
// =============================================================================

using System;

/// <summary>
/// Static factory za izgradnju i upravljanje menija/toolbara na
/// <see cref="IVvDocumentHost"/>-u. Vidi MarkDowns\DevExpress_Migration_V4.md §3.1b.
/// </summary>
public static class VvToolbarFactory
{
   // =========================================================================
   // Create* — gradnja menija i toolbarova iz VvMenu[] struktura.
   // Implementira se u Fazi 2g (kada `IVvDocumentHost` primi DevExpress BarManager).
   // U Fazi 1 su stubovi; VvForm jos uvijek gradi svoje toolbarove internim kodom.
   // =========================================================================

   /// <summary>
   /// Izgradi Bar_Record (CRUD + nav gumbi) na host-u. Faza 2g.
   /// </summary>
   public static void CreateBar_Record(IVvDocumentHost host)
   {
      if (host == null) throw new ArgumentNullException(nameof(host));
      throw new NotImplementedException("VvToolbarFactory.CreateBar_Record: Faza 2g.");
   }

   /// <summary>
   /// Izgradi Bar_Report (report ops) na host-u. Faza 2g.
   /// </summary>
   public static void CreateBar_Report(IVvDocumentHost host)
   {
      if (host == null) throw new ArgumentNullException(nameof(host));
      throw new NotImplementedException("VvToolbarFactory.CreateBar_Report: Faza 2g.");
   }

   /// <summary>
   /// Izgradi per-SubModul toolbare na host-u. Faza 2g.
   /// </summary>
   public static void CreateBar_SubModul(IVvDocumentHost host)
   {
      if (host == null) throw new ArgumentNullException(nameof(host));
      throw new NotImplementedException("VvToolbarFactory.CreateBar_SubModul: Faza 2g.");
   }

   /// <summary>
   /// Izgradi glavni meni na host-u. Faza 2g.
   /// </summary>
   /// <param name="isDetached">
   /// True = reducirani meni za detached <c>VvFloatingForm</c> (npr. bez "Nova SubModul tab").
   /// False = puni meni za glavnu <c>VvForm</c>. Koristi se tek u Fazi 3a.
   /// </param>
   public static void CreateMenuBar(IVvDocumentHost host, bool isDetached = false)
   {
      if (host == null) throw new ArgumentNullException(nameof(host));
      throw new NotImplementedException("VvToolbarFactory.CreateMenuBar: Faza 2g.");
   }

   // =========================================================================
   // ApplyWriteMode — single extraction point za sva WriteMode pravila.
   // Faza 1b C10: tijelo se preuzima iz
   //   VvForm.SetVvMenuEnabledOrDisabled_RegardingWriteMode_JOB
   // bit-identicno (ukljucujuci svih 7 specijalnih case-ova iz V4 §1.6:
   // IsTEXTHOshop, IsPCTOGO × 4 varijante, KDCDUC, IsSvDUH_ZAHonly × 2).
   // =========================================================================

   /// <summary>
   /// Primjeni WriteMode na sve relevantne menu/toolbar gumbe host-a.
   /// Faza 1b C10: placeholder koji delegira na postojecu VvForm metodu.
   /// </summary>
   public static void ApplyWriteMode(IVvDocumentHost host, ZXC.WriteMode writeMode)
   {
      if (host == null) throw new ArgumentNullException(nameof(host));

      // Faza 1b C10: privremeno delegiramo na postojecu VvForm implementaciju
      // dok god je host VvForm singleton. U C10 se tijelo seli ovdje.
      VvForm vvForm = host as VvForm;
      if (vvForm != null)
      {
         vvForm.SetVvMenuEnabledOrDisabled_RegardingWriteMode(writeMode);
         return;
      }

      // Za detached VvFloatingForm (Faza 3): nema fallbacka — mora imati factory impl.
      throw new NotImplementedException(
         "VvToolbarFactory.ApplyWriteMode: tijelo se seli iz " +
         "VvForm.SetVvMenuEnabledOrDisabled_RegardingWriteMode_JOB u C10.");
   }

   /// <summary>
   /// Primjeni product-site filtriranje na ts_Modul gumbe (Surger / Remonster / ostali).
   /// Faza 1b C10: tijelo iz <c>VvForm.InitalizeToolStrip_Modul</c>.
   /// </summary>
   public static void ApplyProductTypeFilter(IVvDocumentHost host)
   {
      if (host == null) throw new ArgumentNullException(nameof(host));
      throw new NotImplementedException(
         "VvToolbarFactory.ApplyProductTypeFilter: tijelo se seli iz " +
         "VvForm.InitalizeToolStrip_Modul u C10/C11 (po potrebi).");
   }
}
