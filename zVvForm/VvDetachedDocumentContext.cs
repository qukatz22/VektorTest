using System;

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
}
