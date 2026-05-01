using System;
using System.Windows.Forms;

/// <summary>
/// Phase 3 DETACH context for moving one tab content instance between hosts.
/// </summary>
internal sealed class VvDetachedDocumentContext
{
   public VvDetachedDocumentContext(VvForm sourceForm, VvTabPage sourceTabPage)
   {
      if(sourceForm == null) throw new ArgumentNullException("sourceForm");
      if(sourceTabPage == null) throw new ArgumentNullException("sourceTabPage");
      if(sourceTabPage.TheVvUC == null) throw new InvalidOperationException("Detach nije moguć jer source tab nema VvUserControl.");
      if(sourceTabPage.IsDetached) throw new InvalidOperationException("Detach nije moguć jer je tab već detached.");

      SourceForm = sourceForm;
      SourceTabPage = sourceTabPage;
      HostedUserControl = sourceTabPage.TheVvUC;
      Title = sourceTabPage.Title;
   }

   public VvForm SourceForm { get; private set; }
   public VvTabPage SourceTabPage { get; private set; }
   public VvUserControl HostedUserControl { get; private set; }
   public string Title { get; private set; }

   public static bool CanDetach(VvTabPage sourceTabPage, out string reason)
   {
      reason = null;

      if(sourceTabPage == null)
      {
         reason = "source tab nije dostupan";
         return false;
      }

      if(sourceTabPage.IsDetached)
      {
         reason = "tab je već detached";
         return false;
      }

      if(sourceTabPage.IsArhivaTabPage)
      {
         reason = "tab je u Arhivi";
         return false;
      }

      if(sourceTabPage.TheVvUC == null)
      {
         reason = "tab nema VvUserControl";
         return false;
      }

      if(sourceTabPage.TheVvUC.IsDisposed || sourceTabPage.TheVvUC.Disposing)
      {
         reason = "VvUserControl se zatvara";
         return false;
      }

      return true;
   }

   public bool ShouldCancelClose()
   {
      if(SourceTabPage.IsArhivaTabPage)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Izađite, najprije, iz Arhive.");
         return true;
      }

      return SourceForm.HasTheTabPageAnyUnsavedData(SourceTabPage);
   }
}
