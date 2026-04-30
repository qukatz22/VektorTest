using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;

[System.Security.SecuritySafeCritical]
public class VvRichTextBoxToolStrip : ToolStrip
{
   #region Fieldz

   public AdvRichTextBox TheRTB { get; set; }

   public float PrefferedZoomFactor;

   private ToolStripButton tsb_Open, tsb_SaveAs, tsb_Font, tsb_FontBig, tsb_FontSml, 
                           tsb_Left, tsb_Center, tsb_Right, tsb_Justify,
                           tsb_Bold, tsb_Italic, tsb_Underline,
                           tsb_Copy, tsb_Cut, tsb_Paste, tsb_Undo, tsb_Redo,
                           tsb_Find, tsb_FindRe, tsb_SelAll;

   private ToolStripDropDownButton tsddb_indent, tsddb_hanging;
   private ToolStripTextBox        tstbx_FontSize;
   
   private ContextMenuStrip contextMenuStrip_indent, contextMenuStrip_hangingIndent;
   private int[]                   intText_Indent, intText_Hanging;
   private FontDialog     fontDialog;
   private OpenFileDialog openFileDialog;
   private SaveFileDialog saveFileDialog;
   private string         currentFile;


   #endregion Fieldz

   #region Constructor

   public VvRichTextBoxToolStrip(AdvRichTextBox rtb)
   {
      this.TheRTB = rtb;
      
      this.TheRTB.Tag = this;

      if(ZXC.ThisIsSurgerProject || ZXC.ThisIsRemonsterProject) PrefferedZoomFactor = 1.25f;
      else                                                      PrefferedZoomFactor = 1.00f;

      InitializationToolStrip();

      CreateParametar4ContextMenuStrips();

      CreateToolStripButtons();

      TheRTB.SelectionChanged += new EventHandler(TheRTB_SelectionChanged);
   }

   #endregion Constructor

   #region InitializationToolStrip()

   private void InitializationToolStrip()
   {
      this.Dock      = DockStyle.None;
      this.BackColor = ZXC.vvColors.vvTBoxReadOnly_False_BackColor;// ZXC.vvColors.vvTBoxHotOn_GotFocus_BackColor;
      this.GripStyle = ToolStripGripStyle.Hidden;
      
      this.Visible   = false;

      fontDialog     = new FontDialog();
      saveFileDialog = new SaveFileDialog();
      openFileDialog = new OpenFileDialog();
   }

   #endregion InitializationToolStrip()

   #region ToolStripButton

   private void CreateToolStripButtons()
   {
      tsb_Open    = AddToolStripButton("Open"       , "rtbx_open.ico"    , new EventHandler(TsbOpen_Click)   , "tsb_Open");
      tsb_SaveAs  = AddToolStripButton("SaveAs"     , "rtbx_save.ico"    , new EventHandler(TsbSaveAs_Click) , "tsb_SaveAs");
      
      this.Items.Add(new ToolStripSeparator());
    
      tsb_SelAll  = AddToolStripButton("SelectAll", "rtbx_lst2icon.ico", new EventHandler(TsbSelectedAll_Click), "tsb_SelAll");

      this.Items.Add(new ToolStripSeparator());
       
      tsb_Font    = AddToolStripButton("Font"       , "rtbx_font.ico"    , new EventHandler(TsbFont_Click)   , "tsb_Font");
      tsb_FontBig = AddToolStripButton("Font+"      , "rtbx_font_big.ico", new EventHandler(TsbFontBig_Click), "tsb_FontBig");
      tsb_FontSml = AddToolStripButton("Font-"      , "rtbx_font_sml.ico", new EventHandler(TsbFontSml_Click), "tsb_FontSml");

      this.Items.Add(new ToolStripSeparator());

      tstbx_FontSize      = AddToolStripTextBox("rtbx_tstbx_FontSize");
     // tstbx_FontSize.Text = ((int)(TheRTB.SelectionFont.Size)).ToString();
    
      this.Items.Add(new ToolStripSeparator());
      
      tsb_Left    = AddToolStripButton("Lijevo"     , "rtbx_left.ico"     , new EventHandler(TsbLeft_Click)    , "tsb_Left");
      tsb_Center  = AddToolStripButton("Centar"     , "rtbx_centre.ico"   , new EventHandler(TsbCentar_Click)  , "tsb_Centre");
      tsb_Right   = AddToolStripButton("Desno"      , "rtbx_right.ico"    , new EventHandler(TsbRight_Click)   , "tsb_Right");
      tsb_Justify = AddToolStripButton("Justify"    , "rtbx_justify.ico"  , new EventHandler(TsbJustify_Click) , "tsb_Justify");
      
      this.Items.Add(new ToolStripSeparator());
      
      tsb_Bold      = AddToolStripButton("Bold"     , "rtbx_bold.ico"    , new EventHandler(TsbBold_Click)     , "tsb_Bold");
      tsb_Italic    = AddToolStripButton("Italic"   , "rtbx_italic.ico"  , new EventHandler(TsbItalic_Click)   , "tsb_Italic");
      tsb_Underline = AddToolStripButton("Underline", "rtbx_underlne.ico", new EventHandler(TsbUnderline_Click), "tsb_Underline");
      
      this.Items.Add(new ToolStripSeparator());

      tsb_Copy      = AddToolStripButton("Copy"     , "rtbx_copy.ico"    , new EventHandler(TsbCopy_Click)     , "tsb_Copy");
      tsb_Cut       = AddToolStripButton("Cut"      , "rtbx_cut.ico"     , new EventHandler(TsbCut_Click)      , "tsb_Cut");
      tsb_Paste     = AddToolStripButton("Paste"    , "rtbx_paste.ico"   , new EventHandler(TsbPaste_Click)    , "tsb_Paste");
  
      this.Items.Add(new ToolStripSeparator());

      tsb_Undo      = AddToolStripButton("Undo"     , "rtbx_undo.ico"    , new EventHandler(TsbUndo_Click)     , "tsb_Undo");
      tsb_Redo      = AddToolStripButton("Redo"     , "rtbx_redo.ico"    , new EventHandler(TsbRedo_Click)     , "tsb_Redo");

      this.Items.Add(new ToolStripSeparator());

      tsb_Find      = AddToolStripButton("Find"     , "rtbx_find.ico"    , new EventHandler(TsbFind_Click)       , "tsb_Find");
      tsb_FindRe    = AddToolStripButton("FindRepla", "rtbx_findRpl.ico" , new EventHandler(TsbFindReplace_Click), "tsb_FindRe");

      this.Items.Add(new ToolStripSeparator());

      tsddb_indent  = AddToolStripDropDownButton("Komplet", "rtbx_indent.ico", contextMenuStrip_indent       , null, "tsddb_indent");
      tsddb_hanging = AddToolStripDropDownButton("Ostatak", "rtbx_hangng.ico", contextMenuStrip_hangingIndent, null, "tsddb_hanging");
      
   }

   private ToolStripButton AddToolStripButton(string text, string icon, EventHandler eventHandler, string name)
   {
      ToolStripButton tsb = new ToolStripButton(text, new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_RTB." + icon), 16, 16).ToBitmap(), eventHandler, name);
      tsb.DisplayStyle          = ToolStripItemDisplayStyle.Image;
      tsb.ImageTransparentColor = Color.FromArgb(192, 192, 192);
      this.Items.Add(tsb);
      return tsb;
   }

   private ToolStripDropDownButton AddToolStripDropDownButton(string text, string icon, ContextMenuStrip contextMenuStrip, EventHandler eventHandler, string name)
   {
      ToolStripDropDownButton tsddb = new ToolStripDropDownButton(text, new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_RTB." + icon), 16, 16).ToBitmap(), eventHandler, name);
      tsddb.DropDown                = contextMenuStrip;
      tsddb.DisplayStyle            = ToolStripItemDisplayStyle.Image;
      tsddb.ImageTransparentColor   = Color.FromArgb(192, 192, 192);
      this.Items.Add(tsddb);
      return tsddb;

   }


   private ToolStripTextBox AddToolStripTextBox(string name)
   {
      ToolStripTextBox tstbx = new ToolStripTextBox();
      tstbx.Name             = name;
      tstbx.Size             = new Size(ZXC.Q6un + ZXC.Qun4, ZXC.QUN);
      tstbx.ReadOnly         = true;
      tstbx.Text             = "";
      this.Items.Add(tstbx);

      return tstbx;
   }

   #endregion ToolStripButton

   #region Eveniti

   private void TheRTB_SelectionChanged(object sender, EventArgs e)
   {
      if(TheRTB.SelectionFont != null)
      {
         tsb_Bold.Checked      = TheRTB.SelectionFont.Bold;
         tsb_Italic.Checked    = TheRTB.SelectionFont.Italic;
         tsb_Underline.Checked = TheRTB.SelectionFont.Underline;

         tstbx_FontSize.Text = TheRTB.SelectionFont.OriginalFontName + " " +  ((int)(TheRTB.SelectionFont.Size)).ToString();
      }
      else
      {
         tstbx_FontSize.Text = "";
      }

      tsb_Left.Checked    = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Left;
      tsb_Right.Checked   = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Right;
      tsb_Center.Checked  = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Center;
      tsb_Justify.Checked = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Justify;
   }

   private void TsbOpen_Click(object sender, System.EventArgs e)
   {
      try
      {
              OpenFile();
      }
      catch (Exception ex)
      {
          MessageBox.Show(ex.Message.ToString(), "Error");
      }
  }

   private void OpenFile()
   {
      try
      {
         openFileDialog.Title       = "RTE - Open File";
         openFileDialog.DefaultExt  = "rtf";
         openFileDialog.Filter      = "Rich Text Files|*.rtf|Text Files|*.txt|HTML Files|*.htm|All Files|*.*";
         openFileDialog.FilterIndex = 1;
         openFileDialog.FileName    = string.Empty;

         if(openFileDialog.ShowDialog() == DialogResult.OK)
         {

            if(openFileDialog.FileName == "")
            {
               return;
            }

            string strExt;
            strExt = System.IO.Path.GetExtension(openFileDialog.FileName);
            strExt = strExt.ToUpper();

            if(strExt == ".RTF")
            {
               TheRTB.LoadFile(openFileDialog.FileName, RichTextBoxStreamType.RichText);
            }
            else
            {
               System.IO.StreamReader txtReader;
               txtReader = new System.IO.StreamReader(openFileDialog.FileName);
               TheRTB.Text = txtReader.ReadToEnd();
               txtReader.Close();
               txtReader = null;
               TheRTB.SelectionStart = 0;
               TheRTB.SelectionLength = 0;
            }

            currentFile = openFileDialog.FileName;
            TheRTB.Modified = false;
            this.Text = "Editor: " + currentFile.ToString();
         }
         else
         {
            MessageBox.Show("Open File request cancelled by user.", "Cancelled");
         }
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
   }

   private void TsbSaveAs_Click(object sender, System.EventArgs e)
   {

      try
      {
         saveFileDialog.Title       = "RTE - Save File";
         saveFileDialog.DefaultExt  = "rtf";
         saveFileDialog.Filter      = "Rich Text Files|*.rtf|Text Files|*.txt|HTML Files|*.htm|All Files|*.*";
         saveFileDialog.FilterIndex = 1;

         if(saveFileDialog.ShowDialog() == DialogResult.OK)
         {

            if(saveFileDialog.FileName == "")
            {
               return;
            }

            string strExt;
            strExt = System.IO.Path.GetExtension(saveFileDialog.FileName);
            strExt = strExt.ToUpper();

            if(strExt == ".RTF")
            {
               TheRTB.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.RichText);
            }
            else
            {
               System.IO.StreamWriter txtWriter;
               txtWriter = new System.IO.StreamWriter(saveFileDialog.FileName);
               txtWriter.Write(TheRTB.Text);
               txtWriter.Close();
               txtWriter = null;
               TheRTB.SelectionStart = 0;
               TheRTB.SelectionLength = 0;
            }

            currentFile     = saveFileDialog.FileName;
            TheRTB.Modified = false;
            this.Text = "Editor: " + currentFile.ToString();
            MessageBox.Show(currentFile.ToString() + " saved.", "File Save");
         }
         else
         {
            MessageBox.Show("Save File request cancelled by user.", "Cancelled");
         }
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
   }

   private void TsbSelectedAll_Click(object sender, System.EventArgs e)
   {
      try
      {
         TheRTB.SelectAll();
      }
      catch(Exception)
      {
         MessageBox.Show("Unable to select all document content.", "RTE - Select", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
   }

   private void TsbFont_Click(object sender, System.EventArgs e)
   {
      SelectFontToolStripMenuItem_Click(this, e);
   }

   private void SelectFontToolStripMenuItem_Click(object sender, System.EventArgs e)
   {
      try
      {
         if(TheRTB.SelectionFont != null)
         {
            fontDialog.Font     = TheRTB.SelectionFont;
            tstbx_FontSize.Text = TheRTB.SelectionFont.OriginalFontName + " " +  ((int)(TheRTB.SelectionFont.Size)).ToString();
         }

         else
         {
            fontDialog.Font     = null;
            tstbx_FontSize.Text = "";
         }
         fontDialog.ShowApply = true;
         if(fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
         {
            TheRTB.SelectionFont = fontDialog.Font;
         }
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }

   }

   private void TsbFontBig_Click(object sender, System.EventArgs e)
   {
      try
      {
         if(TheRTB.SelectionFont != null)
         {
            Font currentFont = TheRTB.SelectionFont;
            float currSize = currentFont.Size;

            if(currSize > 32f) return;

            TheRTB.SelectionFont = new Font(currentFont.FontFamily, ++currSize);
            tstbx_FontSize.Text = TheRTB.SelectionFont.OriginalFontName + " " + ((int)(TheRTB.SelectionFont.Size)).ToString();
         }
         else
         {
            tstbx_FontSize.Text = "";
         }
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
   }

   private void TsbFontSml_Click(object sender, System.EventArgs e)
   {
      try
      {
         if(TheRTB.SelectionFont != null)
         {
            Font currentFont = TheRTB.SelectionFont;
            float currSize = currentFont.Size;

            if(currSize < 6f) return;

            TheRTB.SelectionFont = new Font(currentFont.FontFamily, --currSize);
            tstbx_FontSize.Text = TheRTB.SelectionFont.OriginalFontName + " " + ((int)(TheRTB.SelectionFont.Size)).ToString();
         }
         else
         {
            tstbx_FontSize.Text = "";
         }
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
   }

   private void TsbLeft_Click(object sender, System.EventArgs e)
   {
      TheRTB.SelectionAlignment = AdvRichTextBox.TextAlign.Left;
      tsb_Left.Checked    = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Left;
      tsb_Right.Checked   = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Right;
      tsb_Center.Checked  = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Center;
      tsb_Justify.Checked = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Justify;
   }

   private void TsbCentar_Click(object sender, System.EventArgs e)
   {
      TheRTB.SelectionAlignment = AdvRichTextBox.TextAlign.Center;
      tsb_Left.Checked    = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Left;
      tsb_Right.Checked   = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Right;
      tsb_Center.Checked  = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Center;
      tsb_Justify.Checked = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Justify;
   }

   private void TsbRight_Click(object sender, System.EventArgs e)
   {
      TheRTB.SelectionAlignment = AdvRichTextBox.TextAlign.Right;
      tsb_Left.Checked    = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Left;
      tsb_Right.Checked   = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Right;
      tsb_Center.Checked  = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Center;
      tsb_Justify.Checked = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Justify;
   }

   private void TsbJustify_Click(object sender, System.EventArgs e)
   {
      
      TheRTB.SelectionAlignment = AdvRichTextBox.TextAlign.Justify;
      tsb_Left.Checked    = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Left;
      tsb_Right.Checked   = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Right;
      tsb_Center.Checked  = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Center;
      tsb_Justify.Checked = TheRTB.SelectionAlignment == AdvRichTextBox.TextAlign.Justify;
   }

   
   private void TsbBold_Click(object sender, System.EventArgs e)
   {
      BoldToolStripMenuItem_Click(this, e);
   }

   private void TsbItalic_Click(object sender, System.EventArgs e)
   {
      ItalicToolStripMenuItem_Click(this, e);
   }

   private void TsbUnderline_Click(object sender, System.EventArgs e)
   {
      UnderlineToolStripMenuItem_Click(this, e);
   }
 
   private void BoldToolStripMenuItem_Click(object sender, System.EventArgs e)
   {
      try
      {
         if(TheRTB.SelectionFont != null)
         {
            System.Drawing.Font currentFont = TheRTB.SelectionFont;
            System.Drawing.FontStyle newFontStyle;

            newFontStyle = TheRTB.SelectionFont.Style ^ FontStyle.Bold;

            TheRTB.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);

         }
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
      tsb_Bold.Checked = TheRTB.SelectionFont.Bold;
   }

   private void ItalicToolStripMenuItem_Click(object sender, System.EventArgs e)
   {
      try
      {
         if(TheRTB.SelectionFont != null)
         {
            System.Drawing.Font currentFont = TheRTB.SelectionFont;
            System.Drawing.FontStyle newFontStyle;

            newFontStyle = TheRTB.SelectionFont.Style ^ FontStyle.Italic;

            TheRTB.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
         }
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
      tsb_Italic.Checked = TheRTB.SelectionFont.Italic;
   }

   private void UnderlineToolStripMenuItem_Click(object sender, System.EventArgs e)
   {
      try
      {
         if(TheRTB.SelectionFont != null)
         {
            System.Drawing.Font currentFont = TheRTB.SelectionFont;
            System.Drawing.FontStyle newFontStyle;

            newFontStyle = TheRTB.SelectionFont.Style ^ FontStyle.Underline;

            TheRTB.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
         }
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
      tsb_Underline.Checked = TheRTB.SelectionFont.Underline;
   }

   private void TsbCopy_Click(object sender, System.EventArgs e)
   {
      try
      {
         TheRTB.Copy();
      }
      catch(Exception)
      {
         MessageBox.Show("Unable to copy document content.", "RTE - Copy", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
   }

   private void TsbCut_Click(object sender, System.EventArgs e)
   {
      try
      {
         TheRTB.Cut();
      }
      catch
      {
         MessageBox.Show("Unable to cut document content.", "RTE - Cut", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
   }

   private void TsbPaste_Click(object sender, System.EventArgs e)
   {
      try
      {
         TheRTB.Paste();
      }
      catch
      {
         MessageBox.Show("Unable to copy clipboard content to document.", "RTE - Paste", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
   }

   private void TsbUndo_Click(object sender, System.EventArgs e)
   {
      try
      {
         if(TheRTB.CanUndo)
         {
            TheRTB.Undo();
         }
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
   }

   private void TsbRedo_Click(object sender, System.EventArgs e)
   {
      try
      {
         if(TheRTB.CanRedo)
         {
            TheRTB.Redo();
         }
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
   }


   private void TsbFind_Click(object sender, System.EventArgs e)
   {
      try
      {
         FormVvRichTextBoxFind frm = new FormVvRichTextBoxFind(TheRTB);
         frm.Show();
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
   }




   private void TsbFindReplace_Click(object sender, System.EventArgs e)
   {
      try
      {
         FormVvRichTextBoxFindReplace frm = new FormVvRichTextBoxFindReplace(TheRTB);
         frm.Show();
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
   }

      private void Mi_SelectionIndent_Click(object sender, EventArgs ea)
   {
      ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
      int selectionIndent    = (int)tsmi.Tag;
    
      try
      {
         TheRTB.SelectionIndent = selectionIndent;
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
   }

   private void Mi_SelectionHangingIndent_Click(object sender, EventArgs ea)
   {
      ToolStripMenuItem tsmi     = sender as ToolStripMenuItem;
      int SelectionHangingIndent = (int)tsmi.Tag;
    
      try
      {
         TheRTB.SelectionHangingIndent = SelectionHangingIndent;
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
   }

   #endregion Eveniti

   #region ContextMenuStrips4DropDownButton

   private void CreateParametar4ContextMenuStrips()
   {
      intText_Indent  = new int[] { -60, -50, -40, -30, -20, -10, 0, 10, 20, 30, 40, 50, 60 };
      intText_Hanging = new int[] {                -30, -20, -10, 0, 10, 20, 30             };

      contextMenuStrip_indent        = new ContextMenuStrip();
      contextMenuStrip_hangingIndent = new ContextMenuStrip();

      CreateContextMenuStrips( intText_Indent , contextMenuStrip_indent       , new EventHandler(Mi_SelectionIndent_Click));
      CreateContextMenuStrips( intText_Hanging, contextMenuStrip_hangingIndent, new EventHandler(Mi_SelectionHangingIndent_Click));
   }

   private void CreateContextMenuStrips(int[] intText, ContextMenuStrip contextMenuStrip, EventHandler eventHandler)
   {
      ToolStripMenuItem[] tsmi = new ToolStripMenuItem[intText.Length];
      
      for(int i = 0; i < intText.Length; i++)
      {
         tsmi[i]        = new ToolStripMenuItem(intText[i].ToString());
         tsmi[i].Tag    = intText[i];
         tsmi[i].Click += eventHandler;
         contextMenuStrip.Items.Add(tsmi[i]);
      }
   }

   #endregion ContextMenuStrips4DropDownButton

}

#region FormVvRichTextBoxFind/Replace
[System.Security.SecuritySafeCritical]
public class FormVvRichTextBoxFind : Form
{
   // local member variable to hold main form
   AdvRichTextBox TheRtb;

   // default constructor
   public FormVvRichTextBoxFind()
   {
      InitializeComponent();
   }

   // overloaded constructor - permits passing in main form
   public FormVvRichTextBoxFind(AdvRichTextBox _rtb)
   {
      InitializeComponent();
      this.TopMost = true;
      TheRtb = _rtb;
   }

   private void btnFind_Click(object sender, System.EventArgs e)
   {
      try
      {
         int StartPosition;
         StringComparison SearchType;

         if(chkMatchCase.Checked == true)
         {
            SearchType = StringComparison.Ordinal;
         }
         else
         {
            SearchType = StringComparison.OrdinalIgnoreCase;
         }

         StartPosition = TheRtb.Text.IndexOf(txtSearchTerm.Text, SearchType);

         if(StartPosition == 0)
         {
            MessageBox.Show("String: " + txtSearchTerm.Text.ToString() + " not found", "No Matches", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            return;
         }

         TheRtb.Select(StartPosition, txtSearchTerm.Text.Length);
         TheRtb.ScrollToCaret();
         ZXC.TheVvForm.Focus();
         btnFindNext.Enabled = true;
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
   }

   private void btnFindNext_Click(object sender, System.EventArgs e)
   {
      try
      {
         int StartPosition = TheRtb.SelectionStart + 2;

         StringComparison SearchType;

         if(chkMatchCase.Checked == true)
         {
            SearchType = StringComparison.Ordinal;
         }
         else
         {
            SearchType = StringComparison.OrdinalIgnoreCase;
         }

         //StartPosition = Microsoft.VisualBasic.Strings.InStr(StartPosition, TheRtb.Text, txtSearchTerm.Text, SearchType);
         StartPosition = TheRtb.Text.IndexOf(txtSearchTerm.Text, StartPosition, SearchType);

         if(StartPosition == 0 || StartPosition < 0)
         {
            MessageBox.Show("String: " + txtSearchTerm.Text.ToString() + " not found", "No Matches", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            return;
         }

         TheRtb.Select(StartPosition, txtSearchTerm.Text.Length);
         TheRtb.ScrollToCaret();
         ZXC.TheVvForm.Focus();
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
   }


   private void txtSearchTerm_TextChanged(object sender, EventArgs e)
   {
      btnFindNext.Enabled = false;
   }

   /// <summary>
   /// Required designer variable.
   /// </summary>
   private System.ComponentModel.IContainer components = null;

   /// <summary>
   /// Clean up any resources being used.
   /// </summary>
   /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
   protected override void Dispose(bool disposing)
   {
      if(disposing && (components != null))
      {
         components.Dispose();
      }
      base.Dispose(disposing);
   }

   #region Windows Form Designer generated code

   /// <summary>
   /// Required method for Designer support - do not modify
   /// the contents of this method with the code editor.
   /// </summary>
   private void InitializeComponent()
   {
    //  System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVvRichTextBoxFind));
      this.Label1         = new System.Windows.Forms.Label();
      this.txtSearchTerm = new System.Windows.Forms.TextBox();
      this.chkMatchCase  = new System.Windows.Forms.CheckBox();
      this.btnFind       = new System.Windows.Forms.Button();
      this.btnFindNext   = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // Label1
      // 
      this.Label1.AutoSize = true;
      this.Label1.Location = new System.Drawing.Point(23, 26);
      this.Label1.Name     = "Label1";
      this.Label1.Size     = new System.Drawing.Size(71, 13);
      this.Label1.TabIndex = 1;
      this.Label1.Text     = "Search Term:";
      // 
      // txtSearchTerm
      // 
      this.txtSearchTerm.Location = new System.Drawing.Point(26, 42);
      this.txtSearchTerm.Name     = "txtSearchTerm";
      this.txtSearchTerm.Size     = new System.Drawing.Size(252, 20);
      this.txtSearchTerm.TabIndex = 2;
      this.txtSearchTerm.TextChanged += new System.EventHandler(this.txtSearchTerm_TextChanged);
      // 
      // chkMatchCase
      // 
      this.chkMatchCase.AutoSize = true;
      this.chkMatchCase.Location = new System.Drawing.Point(26, 73);
      this.chkMatchCase.Name     = "chkMatchCase";
      this.chkMatchCase.Size     = new System.Drawing.Size(83, 17);
      this.chkMatchCase.TabIndex = 5;
      this.chkMatchCase.Text     = "Match Case";
      this.chkMatchCase.UseVisualStyleBackColor = true;
      // 
      // btnFind
      // 
      this.btnFind.Location = new System.Drawing.Point(311, 42);
      this.btnFind.Name     = "btnFind";
      this.btnFind.Size     = new System.Drawing.Size(75, 21);
      this.btnFind.TabIndex = 6;
      this.btnFind.Text     = "&Find";
      this.btnFind.UseVisualStyleBackColor = true;
      this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
      // 
      // btnFindNext
      // 
      this.btnFindNext.Enabled  = false;
      this.btnFindNext.Location = new System.Drawing.Point(311, 73);
      this.btnFindNext.Name     = "btnFindNext";
      this.btnFindNext.Size     = new System.Drawing.Size(75, 21);
      this.btnFindNext.TabIndex = 7;
      this.btnFindNext.Text     = "Find &Next";
      this.btnFindNext.UseVisualStyleBackColor = true;
      this.btnFindNext.Click += new System.EventHandler(this.btnFindNext_Click);
      // 
      // frmFind
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize          = new System.Drawing.Size(398, 112);
      this.Controls.Add(this.btnFindNext);
      this.Controls.Add(this.btnFind);
      this.Controls.Add(this.chkMatchCase);
      this.Controls.Add(this.txtSearchTerm);
      this.Controls.Add(this.Label1);
    //  this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "formVvRichTextBoxFind";
      this.Text = "RTE - Find Text";
      this.ResumeLayout(false);
      this.PerformLayout();

   }

   #endregion

   internal System.Windows.Forms.Label Label1;
   internal System.Windows.Forms.TextBox txtSearchTerm;
   internal System.Windows.Forms.CheckBox chkMatchCase;
   internal System.Windows.Forms.Button btnFind;
   internal System.Windows.Forms.Button btnFindNext;

}

[System.Security.SecuritySafeCritical]
public partial class FormVvRichTextBoxFindReplace : Form
{
   // member variable pointing to main form
   AdvRichTextBox TheRtb;


   // default constructor
   public FormVvRichTextBoxFindReplace()
   {
      InitializeComponent();
   }


   // overloaded constructor accepteing main form as
   // an argument
   public FormVvRichTextBoxFindReplace(AdvRichTextBox _rtb)
   {
      InitializeComponent();
      this.TopMost = true;
      TheRtb       = _rtb;
   }

   private void btnFind_Click(object sender, System.EventArgs e)
   {
      try
      {
         int StartPosition;
         StringComparison SearchType;

         if(chkMatchCase.Checked == true)
         {
            SearchType = StringComparison.Ordinal;
         }
         else
         {
            SearchType = StringComparison.OrdinalIgnoreCase;
         }

         StartPosition = TheRtb.Text.IndexOf(txtSearchTerm.Text, SearchType);

         if(StartPosition == 0)
         {
            MessageBox.Show("String: " + txtSearchTerm.Text.ToString() + " not found", "No Matches", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            return;
         }

         TheRtb.Select(StartPosition, txtSearchTerm.Text.Length);
         TheRtb.ScrollToCaret();
         ZXC.TheVvForm.Focus();
         btnFindNext.Enabled = true;
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
   }



   private void btnFindNext_Click(object sender, System.EventArgs e)
   {
      try
      {
         int StartPosition = TheRtb.SelectionStart + 2;

         StringComparison SearchType;

         if(chkMatchCase.Checked == true)
         {
            SearchType = StringComparison.Ordinal;
         }
         else
         {
            SearchType = StringComparison.OrdinalIgnoreCase;
         }

         StartPosition = TheRtb.Text.IndexOf(txtSearchTerm.Text, StartPosition, SearchType);

         if(StartPosition == 0 || StartPosition < 0)
         {
            MessageBox.Show("String: " + txtSearchTerm.Text.ToString() + " not found", "No Matches", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            return;
         }

         TheRtb.Select(StartPosition, txtSearchTerm.Text.Length);
         TheRtb.ScrollToCaret();
         ZXC.TheVvForm.Focus();
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }
   }




   private void btnReplace_Click(object sender, System.EventArgs e)
   {
      try
      {
         if(TheRtb.SelectedText.Length != 0)
         {
            TheRtb.SelectedText = txtReplacementText.Text;
         }

         int StartPosition;
         StringComparison SearchType;

         if(chkMatchCase.Checked == true)
         {
            SearchType = StringComparison.Ordinal;
         }
         else
         {
            SearchType = StringComparison.OrdinalIgnoreCase;
         }

         StartPosition = TheRtb.Text.IndexOf(txtSearchTerm.Text, SearchType);

         if(StartPosition == 0 || StartPosition < 0)
         {
            MessageBox.Show("String: " + txtSearchTerm.Text.ToString() + " not found", "No Matches", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            return;
         }

         TheRtb.Select(StartPosition, txtSearchTerm.Text.Length);
         TheRtb.ScrollToCaret();
         ZXC.TheVvForm.Focus();
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }

   }



   private void btnReplaceAll_Click(object sender, System.EventArgs e)
   {

      try
      {
         TheRtb.Rtf = TheRtb.Rtf.Replace(txtSearchTerm.Text.Trim(), txtReplacementText.Text.Trim());


         int StartPosition;
         StringComparison SearchType;

         if(chkMatchCase.Checked == true)
         {
            SearchType = StringComparison.Ordinal;
         }
         else
         {
            SearchType = StringComparison.OrdinalIgnoreCase;
         }

         StartPosition = TheRtb.Text.IndexOf(txtReplacementText.Text, SearchType);

         TheRtb.Select(StartPosition, txtReplacementText.Text.Length);
         TheRtb.ScrollToCaret();
         ZXC.TheVvForm.Focus();
      }
      catch(Exception ex)
      {
         MessageBox.Show(ex.Message.ToString(), "Error");
      }

   }

   private void txtSearchTerm_TextChanged(object sender, EventArgs e)
   {
      btnFindNext.Enabled = false;
   }

   private System.ComponentModel.IContainer components = null;

   /// <summary>
   /// Clean up any resources being used.
   /// </summary>
   /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
   protected override void Dispose(bool disposing)
   {
      if(disposing && (components != null))
      {
         components.Dispose();
      }
      base.Dispose(disposing);
   }

   #region Windows Form Designer generated code

   /// <summary>
   /// Required method for Designer support - do not modify
   /// the contents of this method with the code editor.
   /// </summary>
   private void InitializeComponent()
   {
      this.Label1 = new System.Windows.Forms.Label();
      this.txtSearchTerm = new System.Windows.Forms.TextBox();
      this.Label2 = new System.Windows.Forms.Label();
      this.txtReplacementText = new System.Windows.Forms.TextBox();
      this.chkMatchCase = new System.Windows.Forms.CheckBox();
      this.btnFind = new System.Windows.Forms.Button();
      this.btnFindNext = new System.Windows.Forms.Button();
      this.btnReplace = new System.Windows.Forms.Button();
      this.btnReplaceAll = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // Label1
      // 
      this.Label1.AutoSize = true;
      this.Label1.Location = new System.Drawing.Point(8, 18);
      this.Label1.Name = "Label1";
      this.Label1.Size = new System.Drawing.Size(71, 13);
      this.Label1.TabIndex = 6;
      this.Label1.Text = "Search Term:";
      // 
      // txtSearchTerm
      // 
      this.txtSearchTerm.Location = new System.Drawing.Point(10, 34);
      this.txtSearchTerm.Name = "txtSearchTerm";
      this.txtSearchTerm.Size = new System.Drawing.Size(321, 20);
      this.txtSearchTerm.TabIndex = 7;
      this.txtSearchTerm.TextChanged += new System.EventHandler(this.txtSearchTerm_TextChanged);
      // 
      // Label2
      // 
      this.Label2.AutoSize = true;
      this.Label2.Location = new System.Drawing.Point(8, 71);
      this.Label2.Name = "Label2";
      this.Label2.Size = new System.Drawing.Size(97, 13);
      this.Label2.TabIndex = 11;
      this.Label2.Text = "Replacement Text:";
      // 
      // txtReplacementText
      // 
      this.txtReplacementText.Location = new System.Drawing.Point(10, 87);
      this.txtReplacementText.Name = "txtReplacementText";
      this.txtReplacementText.Size = new System.Drawing.Size(320, 20);
      this.txtReplacementText.TabIndex = 12;
      // 
      // chkMatchCase
      // 
      this.chkMatchCase.AutoSize = true;
      this.chkMatchCase.Location = new System.Drawing.Point(10, 113);
      this.chkMatchCase.Name = "chkMatchCase";
      this.chkMatchCase.Size = new System.Drawing.Size(83, 17);
      this.chkMatchCase.TabIndex = 13;
      this.chkMatchCase.Text = "Match Case";
      this.chkMatchCase.UseVisualStyleBackColor = true;
      // 
      // btnFind
      // 
      this.btnFind.Location = new System.Drawing.Point(11, 149);
      this.btnFind.Name = "btnFind";
      this.btnFind.Size = new System.Drawing.Size(75, 21);
      this.btnFind.TabIndex = 14;
      this.btnFind.Text = "&Find";
      this.btnFind.UseVisualStyleBackColor = true;
      this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
      // 
      // btnFindNext
      // 
      this.btnFindNext.Enabled = false;
      this.btnFindNext.Location = new System.Drawing.Point(92, 149);
      this.btnFindNext.Name = "btnFindNext";
      this.btnFindNext.Size = new System.Drawing.Size(75, 21);
      this.btnFindNext.TabIndex = 15;
      this.btnFindNext.Text = "Find &Next";
      this.btnFindNext.UseVisualStyleBackColor = true;
      this.btnFindNext.Click += new System.EventHandler(this.btnFindNext_Click);
      // 
      // btnReplace
      // 
      this.btnReplace.Location = new System.Drawing.Point(173, 149);
      this.btnReplace.Name = "btnReplace";
      this.btnReplace.Size = new System.Drawing.Size(75, 21);
      this.btnReplace.TabIndex = 16;
      this.btnReplace.Text = "&Replace";
      this.btnReplace.UseVisualStyleBackColor = true;
      this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
      // 
      // btnReplaceAll
      // 
      this.btnReplaceAll.Location = new System.Drawing.Point(254, 149);
      this.btnReplaceAll.Name = "btnReplaceAll";
      this.btnReplaceAll.Size = new System.Drawing.Size(75, 21);
      this.btnReplaceAll.TabIndex = 17;
      this.btnReplaceAll.Text = "Replace &All";
      this.btnReplaceAll.UseVisualStyleBackColor = true;
      this.btnReplaceAll.Click += new System.EventHandler(this.btnReplaceAll_Click);
      // 
      // frmReplace
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(343, 189);
      this.Controls.Add(this.btnReplaceAll);
      this.Controls.Add(this.btnReplace);
      this.Controls.Add(this.btnFindNext);
      this.Controls.Add(this.btnFind);
      this.Controls.Add(this.chkMatchCase);
      this.Controls.Add(this.txtReplacementText);
      this.Controls.Add(this.Label2);
      this.Controls.Add(this.txtSearchTerm);
      this.Controls.Add(this.Label1);
      this.Name = "formVvRichTextBoxFindReplace";
      this.Text = "RTE - Replace Text";
      this.ResumeLayout(false);
      this.PerformLayout();

   }

   #endregion

   internal System.Windows.Forms.Label Label1;
   internal System.Windows.Forms.TextBox txtSearchTerm;
   internal System.Windows.Forms.Label Label2;
   internal System.Windows.Forms.TextBox txtReplacementText;
   internal System.Windows.Forms.CheckBox chkMatchCase;
   internal System.Windows.Forms.Button btnFind;
   internal System.Windows.Forms.Button btnFindNext;
   internal System.Windows.Forms.Button btnReplace;
   internal System.Windows.Forms.Button btnReplaceAll;

}


#endregion FormVvRichTextBoxFind/Replace

#region AdvRichTextBox

/// <summary>
/// Represents a standard <see cref="RichTextBox"/> with some
/// minor added functionality.
/// </summary>
/// <remarks>
/// AdvRichTextBox provides methods to maintain performance
/// while it is being updated. Additional formatting features
/// have also been added.
/// </remarks>
[System.Security.SecuritySafeCritical] // !!! Ovaj redak rijesava bug: 
                                       // TODO: !!!!!! 27.12.2012: odjemput ovo dize exception 'MethodAccessException' i ne mre se uc na PrjktUC!?!? 
public class AdvRichTextBox : RichTextBox
{
   /// <summary>
   /// Maintains performance while updating.
   /// </summary>
   /// <remarks>
   /// <para>
   /// It is recommended to call this method before doing
   /// any major updates that you do not wish the user to
   /// see. Remember to call EndUpdate when you are finished
   /// with the update. Nested calls are supported.
   /// </para>
   /// <para>
   /// Calling this method will prevent redrawing. It will
   /// also setup the event mask of the underlying richedit
   /// thisControl so that no events are sent.
   /// </para>
   /// </remarks>
   public void BeginUpdate()
   {
      // Deal with nested calls.
      ++updating;

      if(updating > 1)
         return;

      // Prevent the thisControl from raising any events.
      oldEventMask = SendMessage(new HandleRef(this, Handle),
                                  EM_SETEVENTMASK, 0, 0);

      // Prevent the thisControl from redrawing itself.
      SendMessage(new HandleRef(this, Handle),
                   WM_SETREDRAW, 0, 0);
   }

   /// <summary>
   /// Resumes drawing and event handling.
   /// </summary>
   /// <remarks>
   /// This method should be called every time a call is made
   /// made to BeginUpdate. It resets the event mask to it's
   /// original value and enables redrawing of the thisControl.
   /// </remarks>
   public void EndUpdate()
   {
      // Deal with nested calls.
      --updating;

      if(updating > 0)
         return;

      // Allow the thisControl to redraw itself.
      SendMessage(new HandleRef(this, Handle),
                   WM_SETREDRAW, 1, 0);

      // Allow the thisControl to raise event messages.
      SendMessage(new HandleRef(this, Handle),
                   EM_SETEVENTMASK, 0, oldEventMask);
   }

   /// <summary>
   /// Gets or sets the alignment to apply to the current
   /// selection or insertion point.
   /// </summary>
   /// <remarks>
   /// Replaces the SelectionAlignment from
   /// <see cref="RichTextBox"/>.
   /// </remarks>
   public new TextAlign SelectionAlignment
   {
      get
      {
         PARAFORMAT fmt = new PARAFORMAT();
         fmt.cbSize = Marshal.SizeOf(fmt);

         // Get the alignment.

         // TODO: !!!!!! 27.12.2012: odjemput ovo dize exception 'MethodAccessException' i ne mre se uc na PrjktUC!?!? 
         SendMessage(new HandleRef(this, Handle), EM_GETPARAFORMAT, SCF_SELECTION, ref fmt);

         // Default to Left align.
         if((fmt.dwMask & PFM_ALIGNMENT) == 0)
            return TextAlign.Left;

         return (TextAlign)fmt.wAlignment;
      }

      set
      {
         PARAFORMAT fmt = new PARAFORMAT();
         fmt.cbSize = Marshal.SizeOf(fmt);
         fmt.dwMask = PFM_ALIGNMENT;
         fmt.wAlignment = (short)value;

         // Set the alignment.

         // // TODO: !!!!!! 27.12.2012: odjemput ovo dize exception 'MethodAccessException' i ne mre se uc na PrjktUC!?!? 
         SendMessage(new HandleRef(this, Handle), EM_SETPARAFORMAT, SCF_SELECTION, ref fmt);
      }
   }

   /// <summary>
   /// This member overrides
   /// <see cref="Control"/>.OnHandleCreated.
   /// </summary>
   protected override void OnHandleCreated(EventArgs e)
   {
      base.OnHandleCreated(e);

      // Enable support for justification.
      // // TODO: !!!!!! 27.12.2012: odjemput ovo dize exception 'MethodAccessException' i ne mre se uc na PrjktUC!?!? 
      try
      {
         SendMessage(new HandleRef(this, Handle),
                      EM_SETTYPOGRAPHYOPTIONS,
                      TO_ADVANCEDTYPOGRAPHY,
                      TO_ADVANCEDTYPOGRAPHY);
      }
      catch(MethodAccessException)
      {
         // 17.02.2014: kada je UC ReadOnly dodemo ovdje. U zutome radi OK, sejva se i printa OK!? 
         //ZXC.aim_emsg(MessageBoxIcon.Warning, "Alignment did not succeed. Temporary.");
      }
   }

   private int updating = 0;
   private int oldEventMask = 0;

   // Constants from the Platform SDK.
   private const int EM_SETEVENTMASK = 1073;
   private const int EM_GETPARAFORMAT = 1085;
   private const int EM_SETPARAFORMAT = 1095;
   private const int EM_SETTYPOGRAPHYOPTIONS = 1226;
   private const int WM_SETREDRAW = 11;
   private const int TO_ADVANCEDTYPOGRAPHY = 1;
   private const int PFM_ALIGNMENT = 8;
   private const int SCF_SELECTION = 1;

   // It makes no difference if we use PARAFORMAT or
   // PARAFORMAT2 here, so I have opted for PARAFORMAT2.
   [StructLayout(LayoutKind.Sequential)]
   private struct PARAFORMAT
   {
      public int cbSize;
      public uint dwMask;
      public short wNumbering;
      public short wReserved;
      public int dxStartIndent;
      public int dxRightIndent;
      public int dxOffset;
      public short wAlignment;
      public short cTabCount;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
      public int[] rgxTabs;

      // PARAFORMAT2 from here onwards.
      public int dySpaceBefore;
      public int dySpaceAfter;
      public int dyLineSpacing;
      public short sStyle;
      public byte bLineSpacingRule;
      public byte bOutlineLevel;
      public short wShadingWeight;
      public short wShadingStyle;
      public short wNumberingStart;
      public short wNumberingStyle;
      public short wNumberingTab;
      public short wBorderSpace;
      public short wBorderWidth;
      public short wBorders;
   }

   [DllImport("user32", CharSet = CharSet.Auto)]
   [System.Security.SecuritySafeCritical]
   private static extern int SendMessage(HandleRef hWnd,
                                          int msg,
                                          int wParam,
                                          int lParam);

   [DllImport("user32", CharSet = CharSet.Auto)]
   [System.Security.SecuritySafeCritical]
   private static extern int SendMessage(HandleRef hWnd,
                                          int msg,
                                          int wParam,
                                          ref PARAFORMAT lp);

   /// <summary>
   /// Specifies how text in a <see cref="AdvRichTextBox"/> is
   /// horizontally aligned.
   /// </summary>
   public enum TextAlign
   {
      /// <summary>
      /// The text is aligned to the left.
      /// </summary>
      Left = 1,

      /// <summary>
      /// The text is aligned to the right.
      /// </summary>
      Right = 2,

      /// <summary>
      /// The text is aligned in the center.
      /// </summary>
      Center = 3,

      /// <summary>
      /// The text is justified.
      /// </summary>
      Justify = 4
   }
}

#endregion AdvRichTextBox
