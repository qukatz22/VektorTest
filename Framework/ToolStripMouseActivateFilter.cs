using System;
using System.Windows.Forms;

/// <summary>
/// NativeWindow subclass koja na ToolStrip/MenuStrip kontrolama pretvara
/// MA_ACTIVATEANDEAT odgovor na WM_MOUSEACTIVATE u MA_ACTIVATE — i analogno
/// MA_NOACTIVATEANDEAT u MA_NOACTIVATE. Cilj: eliminirati standardni WinForms
/// "first click eaten" pattern kada je vlasnicka forma neaktivna (npr. fokus
/// drzi VvFloatingForm), tako da prvi klik na ToolStripButton odmah izvrsi
/// komandu umjesto da se potrosi samo na aktivaciju forme.
///
/// Primjenjuje se rekurzivno preko <see cref="Attach(Control)"/> i automatski
/// re-hookira kad se djeca dodaju kasnije (HandleCreated / ControlAdded).
/// Filter ne mijenja document host routing — `_lastActiveDocumentHost` ostaje
/// onaj koji je zadnje stvarno fokusiran kroz MouseDown na sadrzaju.
/// </summary>
internal static class ToolStripMouseActivateFilter
{
   private const int WM_MOUSEACTIVATE   = 0x0021;
   private const int MA_ACTIVATE        = 1;
   private const int MA_ACTIVATEANDEAT  = 2;
   private const int MA_NOACTIVATE      = 3;
   private const int MA_NOACTIVATEANDEAT= 4;

   public static void Attach(Control root)
   {
      if(root == null) return;

      AttachRecursive(root);
   }

   private static void AttachRecursive(Control control)
   {
      if(control == null) return;

      if(control is ToolStrip)
      {
         Filter f = new Filter();
         if(control.IsHandleCreated) f.AssignHandle(control.Handle);
         else                        control.HandleCreated += (s, e) => f.AssignHandle(((Control)s).Handle);
         control.HandleDestroyed += (s, e) => { try { f.ReleaseHandle(); } catch { } };
      }

      foreach(Control child in control.Controls)
      {
         AttachRecursive(child);
      }

      control.ControlAdded += (s, e) => AttachRecursive(e.Control);
   }

   private sealed class Filter : NativeWindow
   {
      protected override void WndProc(ref Message m)
      {
         base.WndProc(ref m);

         if(m.Msg == WM_MOUSEACTIVATE)
         {
            int result = m.Result.ToInt32();
            if(result == MA_ACTIVATEANDEAT)        m.Result = (IntPtr)MA_ACTIVATE;
            else if(result == MA_NOACTIVATEANDEAT) m.Result = (IntPtr)MA_NOACTIVATE;
         }
      }
   }
}
