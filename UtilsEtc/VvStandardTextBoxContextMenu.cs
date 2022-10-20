using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

/// <summary>
/// A home-grown version of the standard right-click context menu,
/// so that applications can extend this menu and add their own
/// context items.
/// </summary>
public class VvStandardTextBoxContextMenu : ContextMenu
{
   private System.Windows.Forms.MenuItem miUndo;
   private System.Windows.Forms.MenuItem miCut;
   private System.Windows.Forms.MenuItem miCopy;
   private System.Windows.Forms.MenuItem miPaste;
   private System.Windows.Forms.MenuItem miDelete;
   private System.Windows.Forms.MenuItem miSelectAll;
   private System.Windows.Forms.MenuItem miSeparator1;
   private System.Windows.Forms.MenuItem miSeparator2;

   /// <summary>
   /// Creates a standard context menu for a text box, containing
   /// Undo, Cut, Copy, Paste... all of the usual context menu
   /// items.
   /// </summary>
   public VvStandardTextBoxContextMenu() : this(new MenuItem[0])
   { 
   }

   /// <summary>
   /// Creates a standard context menu for a text box, containing
   /// Undo, Cut, Copy, Paste... all of the usual context menu
   /// items, with additional menu items supplied by the caller
   /// that will precede the standard items in the context menu.
   /// </summary>
   /// <param name="additionalMenuItems">Menu items that should
   /// appear above the standard menu items.</param>
   /// <remarks>You can get the same effect as calling this
   /// constructor by calling the no parameter constructor
   /// and then using <see cref="Menu.MenuItemCollection.AddRange"/>
   /// to add menu items later. Just set the <see cref="MenuItem.Index"/>
   /// property of the menu items to start numbering from
   /// 0, and <see cref="Menu.MenuItemCollection.AddRange"/> will rearrange
   /// the standard menu items to follow the new ones you add.</remarks>
   /// 
   public VvStandardTextBoxContextMenu(MenuItem[] additionalMenuItems)
   {
      this.MenuItems.AddRange(additionalMenuItems);

      InitializeComponent();
   }

   #region Static constructor

   // Q remarckirao: 
   //static StandardTextBoxContextMenu()
   //{
   //   Agama.AgamaRegistry.Reg.AddControlSerialization(typeof(StandardTextBoxContextMenu),
   //   null, null, false);
   //}

   #endregion

   private void InitializeComponent()
   {
      this.miUndo       = new System.Windows.Forms.MenuItem();
      this.miSeparator1 = new System.Windows.Forms.MenuItem();
      this.miSeparator2 = new System.Windows.Forms.MenuItem();
      this.miCut        = new System.Windows.Forms.MenuItem();
      this.miCopy       = new System.Windows.Forms.MenuItem();
      this.miPaste      = new System.Windows.Forms.MenuItem();
      this.miDelete     = new System.Windows.Forms.MenuItem();
      this.miSelectAll  = new System.Windows.Forms.MenuItem();
      //
      // miUndo
      //
      this.miUndo.Text = "&Undo";
      this.miUndo.Click += new System.EventHandler(this.miUndo_Click);
      //
      // miSeparator
      //
      this.miSeparator1.Text = "-";
      this.miSeparator2.Text = "-";
      //
      // miCut
      //
      this.miCut.Text = "Cu&t";
      this.miCut.Click += new System.EventHandler(this.miCut_Click);
      //
      // miCopy
      //
      this.miCopy.Text = "&Copy";
      this.miCopy.Click += new System.EventHandler(this.miCopy_Click);
      //
      // miPaste
      //
      this.miPaste.Text = "&Paste";
      this.miPaste.Click += new System.EventHandler(this.miPaste_Click);
      //
      // miDelete
      //
      this.miDelete.Text = "&Delete";
      this.miDelete.Click += new System.EventHandler(this.miDelete_Click);
      //
      // miSelectAll
      //
      this.miSelectAll.Text = "Select &All";
      this.miSelectAll.Click += new
      System.EventHandler(this.miSelectAll_Click);

      this.MenuItems.AddRange(
      new System.Windows.Forms.MenuItem[] {
            this.miUndo,
            this.miSeparator1,
            this.miCut,
            this.miCopy,
            this.miPaste,
            this.miDelete,
            this.miSeparator2,
            this.miSelectAll
         });

      this.Popup += new EventHandler(StandardTextBoxContextMenu_Popup);
   }

   private void miUndo_Click(object sender, System.EventArgs e)
   {
      // Get the text box that the context menu was popped on
      if(this.SourceControl is TextBox)
      {
         TextBox clickedBox = (TextBox)this.SourceControl;

         if(clickedBox.CanUndo)
         {
            clickedBox.Undo();
         }
      }
   }

   private void miCut_Click(object sender, System.EventArgs e)
   {
      // Get the text box that the context menu was popped on
      if(this.SourceControl is TextBox)
      {
         TextBox clickedBox = (TextBox)this.SourceControl;

         if(clickedBox.SelectionLength > 0)
         {
            clickedBox.Cut();
         }
      }
   }

   private void miCopy_Click(object sender, System.EventArgs e)
   {
      // Get the text box that the context menu was popped on
      if(this.SourceControl is TextBox)
      {
         TextBox clickedBox = (TextBox)this.SourceControl;

         if(clickedBox.SelectionLength > 0)
         {
            clickedBox.Copy();
         }
      }
   }

   private void miPaste_Click(object sender, System.EventArgs e)
   {
      // Get the text box that the context menu was popped on
      if(this.SourceControl is TextBox)
      {
         TextBox clickedBox = (TextBox)this.SourceControl;

         //Q: if(clickedBox.SelectionLength > 0)
         {
            clickedBox.Paste();
         }
      }
   }

   [DllImport("user32.dll", CharSet = CharSet.Auto)]
   private static extern int SendMessage(System.IntPtr hWnd, int msg,
   int lParam, int wParam);
   private const int WM_CLEAR = 0x0303;

   private void miDelete_Click(object sender, System.EventArgs e)
   {
      // Get the text box that the context menu was popped on
      if(this.SourceControl is TextBox)
      {
         TextBox clickedBox = (TextBox)this.SourceControl;

         if(clickedBox.SelectionLength > 0)
         {
            SendMessage(clickedBox.Handle, WM_CLEAR, 0, 0);
         }
      }
   }

   private void miSelectAll_Click(object sender, System.EventArgs e)
   {
      // Get the text box that the context menu was popped on
      if(this.SourceControl is TextBox)
      {
         TextBox clickedBox = (TextBox)this.SourceControl;
         clickedBox.SelectAll();
      }
   }

   private void StandardTextBoxContextMenu_Popup(object sender, EventArgs e)
   {
      // Get the text box that the context menu was popped on
      if(this.SourceControl is TextBox)
      {
         TextBox clickedBox = (TextBox)this.SourceControl;

         // Enable and disable standard menu items as necessary
         bool        isSelection     = clickedBox.SelectionLength > 0;
         IDataObject clipObject      = Clipboard.GetDataObject();
         bool        textOnClipboard = clipObject.GetDataPresent(DataFormats.Text);

         this.miUndo.Enabled   = clickedBox.CanUndo;
         this.miCut.Enabled    = isSelection;
         this.miCopy.Enabled   = isSelection;
         this.miPaste.Enabled  = textOnClipboard;
         this.miDelete.Enabled = isSelection;
      }
   }
}

