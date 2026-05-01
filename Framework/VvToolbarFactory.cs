// =============================================================================
// VvToolbarFactory.cs
//
// Faza 1b: Static klasa — sluzbeni ulazni kontrakt za buduce kreiranje i
// enable/disable menija + toolbarova na `IVvDocumentHost`-u.
//
// Stanje nakon C10 (Option B — V4 §3.1b):
//   - `ApplyWriteMode` je sluzbeni ulazni kontrakt. Tijelo FIZICKI OSTAJE u
//     `VvForm.SetVvMenuEnabledOrDisabled_RegardingWriteMode_JOB` do Faze 2g
//     zbog dubokih ovisnosti o VvForm-private stateu (aMainMenu[][],
//     aSubTopMenuItem[][], TheVvUC type-checks). Factory delegira na VvForm.
//   - `Create*` metode ostaju signature-stubovi do Faze 2g kada gradimo
//     DevExpress `Bar` objekte iz postojecih `VvMenu[]` struktura (§1.5 V4).
//   - `ApplyProductTypeFilter` — signature-stub, tijelo iz
//     `VvForm.InitalizeToolStrip_Modul` u buducem komitu po potrebi.
//
// Zasto vec sad kontrakt (a ne tek u Fazi 2g):
//   - Svi NOVI call-siteovi (business layer, Rtrans, detached VvFloatingForm)
//     moraju ici kroz `VvToolbarFactory.ApplyWriteMode(host, wm)` — NIKAD
//     direktno u VvForm. Time u Fazi 2g kada target flipa s `ToolStripItem`
//     na DX `BarButtonItem`, mijenjamo SAMO tijelo factory metoda — ne i
//     call-siteove.
//   - Vidi V4 §3.1b i §1.6.
// =============================================================================

using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;

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

      if(host.DxBar_Record == null)
      {
         host.DxBar_Record = CreateBar(host.DxBarManager, "Record", false);
      }
   }

   /// <summary>
   /// Izgradi Bar_Report (report ops) na host-u. Faza 2g.
   /// </summary>
   public static void CreateBar_Report(IVvDocumentHost host)
   {
      if (host == null) throw new ArgumentNullException(nameof(host));

      if(host.DxBar_Report == null)
      {
         host.DxBar_Report = CreateBar(host.DxBarManager, "Report", false);
      }
   }

   /// <summary>
   /// Izgradi per-SubModul toolbare na host-u. Faza 2g.
   /// </summary>
   public static void CreateBar_SubModul(IVvDocumentHost host)
   {
      if (host == null) throw new ArgumentNullException(nameof(host));

      host.DxBarManager.ForceInitialize();
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

      if(host.DxMenuBar == null)
      {
         host.DxMenuBar = CreateBar(host.DxBarManager, isDetached ? "Detached menu" : "Main menu", true);
      }
   }

   private static Bar CreateBar(BarManager barManager, string barName, bool isMainMenu)
   {
      if(barManager == null) throw new ArgumentNullException(nameof(barManager));

      Bar bar = new Bar(barManager, barName);
      barManager.Bars.Add(bar);
      bar.Visible = true;

      if(isMainMenu)
      {
         bar.OptionsBar.MultiLine = true;
         bar.OptionsBar.UseWholeRow = true;
         barManager.MainMenu = bar;
      }

      return bar;
   }

   public static BarButtonItem CreateButtonItem(IVvDocumentHost host, VvSubMenu subMenu)
   {
      if(host == null) throw new ArgumentNullException(nameof(host));

      BarButtonItem item = new BarButtonItem(host.DxBarManager, subMenu.subMenuText);
      item.Name = subMenu.btnName;
      item.Caption = subMenu.subMenuText;
      item.Enabled = subMenu.enabledInWriteMode == false;
      item.Hint = (subMenu.subMenuDescription.IsEmpty() || subMenu.subMenuDescription == "Description: ") ? subMenu.subMenuText : subMenu.subMenuDescription;

      if(subMenu.icon != null)
      {
         item.ImageOptions.Image = new Icon(subMenu.icon, 16, 16).ToBitmap();
      }

      if(subMenu.evHandler != null)
      {
         item.ItemClick += delegate(object sender, ItemClickEventArgs e)
         {
            subMenu.evHandler(sender, EventArgs.Empty);
         };
      }

      ApplyShortcut(item, subMenu.shortKeys);
      RegisterItem(host, item);

      return item;
   }

   public static BarSubItem CreateSubItem(IVvDocumentHost host, VvSubMenu subMenu)
   {
      if(host == null) throw new ArgumentNullException(nameof(host));

      BarSubItem item = new BarSubItem(host.DxBarManager, subMenu.subMenuText);
      item.Name = subMenu.btnName.IsEmpty() ? subMenu.subMenuText : subMenu.btnName;
      item.Caption = subMenu.subMenuText;
      item.Enabled = subMenu.enabledInWriteMode == false;
      item.Hint = (subMenu.subMenuDescription.IsEmpty() || subMenu.subMenuDescription == "Description: ") ? subMenu.subMenuText : subMenu.subMenuDescription;

      if(subMenu.icon != null)
      {
         item.ImageOptions.Image = new Icon(subMenu.icon, 16, 16).ToBitmap();
      }

      ApplyShortcut(item, subMenu.shortKeys);
      RegisterItem(host, item);

      return item;
   }

   public static BarButtonItem CreateStaticChildItem(IVvDocumentHost host, string text, EventHandler eventHandler)
   {
      if(host == null) throw new ArgumentNullException(nameof(host));

      BarButtonItem item = new BarButtonItem(host.DxBarManager, text);
      item.Name = text;
      item.Caption = text;

      if(eventHandler != null)
      {
         item.ItemClick += delegate(object sender, ItemClickEventArgs e)
         {
            eventHandler(sender, EventArgs.Empty);
         };
      }

      RegisterItem(host, item);

      return item;
   }

   public static BarEditItem CreateComboItem(IVvDocumentHost host, string name, string caption, int width)
   {
      if(host == null) throw new ArgumentNullException(nameof(host));

      RepositoryItemComboBox repositoryItem = new RepositoryItemComboBox();
      repositoryItem.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
      host.DxBarManager.RepositoryItems.Add(repositoryItem);

      BarEditItem item = new BarEditItem(host.DxBarManager, repositoryItem);
      item.Name = name;
      item.Caption = caption;
      item.EditWidth = width;

      RegisterItem(host, item);

      return item;
   }

   private static void RegisterItem(IVvDocumentHost host, BarItem item)
   {
      if(item == null || item.Name.IsEmpty()) return;

      host.DxBarItemsByName[item.Name] = item;
   }

   private static void ApplyShortcut(BarItem item, Keys shortcutKeys)
   {
      if(item == null || shortcutKeys == Keys.None) return;

      item.ItemShortcut = new BarShortcut(shortcutKeys);
   }

   // =========================================================================
   // ApplyWriteMode — single extraction point za sva WriteMode pravila.
   //
   // C10 Option B decision (V4 §3.1b / §1.6):
   //   Tijelo FIZICKI OSTAJE u `VvForm.SetVvMenuEnabledOrDisabled_RegardingWriteMode_JOB`
   //   tijekom cijele Faze 1. Razlog — duboke ovisnosti o VvForm-private stateu
   //   (aMainMenu[][], aSubTopMenuItem[][], TheVvUC type-checks, TheVvTabPage
   //   .ArhivaTableIsNotEmpty) — premjestaj u factory trazio bi interface-pollution
   //   (IVvDocumentHost bi morao izlagati te clanove) sto se kosi s atomic-commit
   //   principom Faze 1b.
   //
   //   Ova metoda je SLUZBENI ULAZNI KONTRAKT. Svi NOVI call-siteovi (business
   //   layer, Rtrans, detached VvFloatingForm iz Faze 3) ulaze kroz
   //   `VvToolbarFactory.ApplyWriteMode(host, wm)` — NIKAD direktno u VvForm
   //   metodu. Postojeci VvForm-interni pozivi ostaju direktni (ne dira se bit).
   //
   //   Fizicki premjestaj tijela: Faza 2g, kad target flipa s `ToolStripButton`
   //   na DX `BarButtonItem`. Tada `_JOB` nestaje s VvForma i seli se ovdje u
   //   retypiranom obliku.
   // =========================================================================

   /// <summary>
   /// Primjeni WriteMode na sve relevantne menu/toolbar gumbe host-a. Ovo je
   /// sluzbeni ulazni kontrakt za enable/disable logiku — svi NOVI call-siteovi
   /// moraju ici kroz ovu metodu. Vidi V4 §3.1b (Option B @ C10).
   /// </summary>
   public static void ApplyWriteMode(IVvDocumentHost host, ZXC.WriteMode writeMode)
   {
      if (host == null) throw new ArgumentNullException(nameof(host));

      // Faza 1b C10: VvForm je jedini IVvDocumentHost u sistemu. Delegiramo na
      // postojecu metodu (§1.6 single extraction point ostaje na VvFormu).
      VvForm vvForm = host as VvForm;
      if (vvForm != null)
      {
         vvForm.SetVvMenuEnabledOrDisabled_RegardingWriteMode(writeMode);
         return;
      }

      // Faza 3 (detach): VvFloatingForm mora imati vlastitu implementaciju
      // — ili kroz premjesteno tijelo u C10(Faza 2g) ili kroz poseban factory
      // path za DX BarManager. Do tada ne smije se pojaviti ne-VvForm host.
      throw new NotImplementedException(
         "VvToolbarFactory.ApplyWriteMode: host tipa " + host.GetType().Name +
         " nije podrzan u Fazi 1b. Detach (Faza 3) ili Faza 2g ce dodati put.");
   }

   /// <summary>
   /// Primjeni product-site filtriranje na ts_Modul gumbe (Surger / Remonster / ostali).
   /// Signature-stub — tijelo iz <c>VvForm.InitalizeToolStrip_Modul</c> seli se u
   /// Fazi 2g kada target flipa na DX `BarItem`. Do tada VvForm gradi svoj ts_Modul interno.
   /// </summary>
   public static void ApplyProductTypeFilter(IVvDocumentHost host)
   {
      if (host == null) throw new ArgumentNullException(nameof(host));
      throw new NotImplementedException(
         "VvToolbarFactory.ApplyProductTypeFilter: tijelo se seli u Fazi 2g. " +
         "Do tada koristi VvForm.InitalizeToolStrip_Modul interno.");
   }
}
