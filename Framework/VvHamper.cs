using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Crownwood.DotNetMagic.Common;
using System.Linq;
using System.Collections.Generic;

public class VvHamper: Panel
{
   #region fieldz

   GroupBox grBox;
   int[]    aiCol, aiRow, aiSpcBefCol, aiSpcBefRow;
   int[]    aiXxCol, aiYyRow;
   int      iBottomMargin, iRightMargin, minHamperWidth, minHamperHeight;
   int      numOfCols, numOfRows;
   bool     showCellsBounds, showHamperBounds, havingGroupBox;
   bool[,]  reservedLocations;

   bool isDUMMY;

   #endregion fieldz

   #region Constructors

   public VvHamper() : base()
   {
   }

   public VvHamper(int numberOfCols, int numberOfRows, string text, Control parent, bool oceNeceGrupBoks)
   {
      InitializeVvHamper(numberOfCols, numberOfRows, text, parent, oceNeceGrupBoks);
   }

   public VvHamper(int numberOfCols, int numberOfRows, string text, Control parent, bool oceNeceGrupBoks,
      int _nextX, int _nextY, int _razmak)
   {
      InitializeVvHamper(numberOfCols, numberOfRows, text, parent, oceNeceGrupBoks);
     
      if(this.VvHavingGroupBox)
      {
         this.Location = new Point(_nextX + _razmak, _nextY + _razmak); 
      }
      else
      {
         this.Location = new Point(_nextX +        0, _nextY +        0); 
      }
   }

   public void InitializeVvHamper(int numberOfCols, int numberOfRows, string text, Control parent, bool oceNeceGrupBoks)
   {
      this.VvNumOfCols = numberOfCols;
      this.VvNumOfRows = numberOfRows;

      this.Parent            = parent;
      this.aiCol             = new int[numberOfCols];
      this.aiRow             = new int[numberOfRows];
      this.aiSpcBefCol       = new int[numberOfCols];
      this.aiSpcBefRow       = new int[numberOfRows];
      this.aiXxCol           = new int[numberOfCols];
      this.aiYyRow           = new int[numberOfRows];
      this.reservedLocations = new bool[numberOfCols, numberOfRows];

      this.Text = text;

      this.havingGroupBox = oceNeceGrupBoks;
      if(this.VvHavingGroupBox == true) 
      {
         this.VvGroupBox           = new GroupBox();
         this.VvGroupBox.Parent    = this;
         this.VvGroupBox.Location  = new Point(0, 0);
         this.VvGroupBox.Text      = text;
         this.VvGroupBox.FlatStyle = FlatStyle.System;
         this.VvGroupBox.Font      = ZXC.vvFont.SmallSmallFont;// new Font(this.Font.FontFamily, this.Font.SizeInPoints-1);
      }
   }

   #endregion Constructors
   
   #region VvLabel

   public Label CreateVvLabel(int col, int row, string text, ContentAlignment txtAlign)
   {
      return PrivateCore_CreateVvLabel(col, row, text, 0, 0, txtAlign, 2);
   }
   public Label CreateVvLabel(int col, int row, string text, ContentAlignment txtAlign, int pomak)// 11.12.2013. tembo dialog
   {
      return PrivateCore_CreateVvLabel(col, row, text, 0, 0, txtAlign, pomak);
   }

   public Label CreateVvLabel(ZXC.Kolona col, ZXC.Redak row, string text, ContentAlignment txtAlign)
   {
      return PrivateCore_CreateVvLabel((int)col, (int)row, text, 0, 0, txtAlign, 2);
   }

   public Label CreateVvLabel(int col, int row, string text, int numOfMergedWidths, int numOfMergedHeights, ContentAlignment txtAlign)
   {
      return PrivateCore_CreateVvLabel(col, row, text, numOfMergedWidths, numOfMergedHeights, txtAlign, 2);
   }

   public Label CreateVvLabel(ZXC.Kolona col, ZXC.Redak row, string text, int numOfMergedWidths, int numOfMergedHeights, ContentAlignment txtAlign)
   {
      return PrivateCore_CreateVvLabel((int)col, (int)row, text, numOfMergedWidths, numOfMergedHeights, txtAlign, 2);
   }
  

   private Label PrivateCore_CreateVvLabel(int col, int row, string text, int numOfMergedWidths, int numOfMergedHeights, ContentAlignment txtAlign, int pomak)
   {
      Label  label    = new Label();
      label.TextAlign = txtAlign;
      label.Text      = (this.VvIsMigrateable ? text.Substring(1) : text);
      label.FlatStyle = FlatStyle.Flat;
      //      rptLabel.Font        = new Font(rptLabel.Font.FontFamilyName, rptLabel.Font.SizeInPoints-1);
      label.Font        = ZXC.vvFont.SmallFont;

     // CreateAnyVvControl(label, col, numOfMergedWidths, row, numOfMergedHeights, 1);
     // CreateAnyVvControl(label, col, numOfMergedWidths, row, numOfMergedHeights, 2);
      // 11.12.2013. tembo dialog
      CreateAnyVvControl(label, col, numOfMergedWidths, row, numOfMergedHeights, pomak); 

      return label;
   }
   
   #endregion VvLabel

   # region VvTextBox

   public VvTextBox CreateVvTextBox(int col, int row, string name, string statusText)
   {
      return PrivateCore_CreateVvTextBox(name, col, row, statusText, 0, 0, 0);
   }

   public VvTextBox CreateVvTextBox(int col, int row, string name, string statusText, int maxLength)
   {
      return PrivateCore_CreateVvTextBox(name, col, row, statusText, maxLength, 0, 0);
   }

   public VvTextBox CreateVvTextBox(ZXC.Kolona col, ZXC.Redak row, string name, string statusText)
   {
      return PrivateCore_CreateVvTextBox(name, (int)col, (int)row, statusText, 0, 0, 0);
   }

   public VvTextBox CreateVvTextBox(ZXC.Kolona col, ZXC.Redak row, string name, string statusText, int maxLength)
   {
      return PrivateCore_CreateVvTextBox(name, (int)col, (int)row, statusText, maxLength, 0, 0);
   }

   public VvTextBox CreateVvTextBox(int col, int row, string name, string statusText, int maxLength, int numOfMergedWidths, int numOfMergedHeights)
   {
      return PrivateCore_CreateVvTextBox(name, col, row, statusText, maxLength, numOfMergedWidths, numOfMergedHeights);
   }

   public VvTextBox CreateVvTextBox(ZXC.Kolona col, ZXC.Redak row, string name, string statusText, int maxLength, int numOfMergedWidths, int numOfMergedHeights)
   {
      return PrivateCore_CreateVvTextBox(name, (int)col, (int)row, statusText, maxLength, numOfMergedWidths, numOfMergedHeights);
   }

   private VvTextBox PrivateCore_CreateVvTextBox(string name, int col, int row, string statusText, int maxLength, int numOfMergedWidths, int numOfMergedHeights)
   {
      VvTextBox tBox      = new VvTextBox();
      tBox.AutoSize       = false;
      tBox.TextAlign      = HorizontalAlignment.Left;
      tBox.MaxLength      = maxLength;
      tBox.Text           = "";
      tBox.BorderStyle    = BorderStyle.Fixed3D;
      tBox.Font           = ZXC.vvFont.BaseFont;
      tBox.Name           = name;
      tBox.JAM_StatusText = statusText;
      
      CreateAnyVvControl(tBox, col, numOfMergedWidths, row, numOfMergedHeights, 0);

      return tBox;
   }

   # endregion VvTextBox

   #region VvTextBoxLookUp

   public VvTextBox CreateVvTextBoxLookUp(int col, int row, string name, string statusText)
   {
      return PrivateCore_CreateVvTextBoxLookUp(name, col, row, statusText, 0, 0, 0);
   }

   public VvTextBox CreateVvTextBoxLookUp(int col, int row, string name, string statusText, int maxLength)
   {
      return PrivateCore_CreateVvTextBoxLookUp(name, col, row, statusText, maxLength, 0, 0);
   }

   public VvTextBox CreateVvTextBoxLookUp(string name, ZXC.Kolona col, ZXC.Redak row, string statusText)
   {
      return PrivateCore_CreateVvTextBoxLookUp(name, (int)col, (int)row, statusText, 0, 0, 0);
   }

   public VvTextBox CreateVvTextBoxLookUp(string name, ZXC.Kolona col, ZXC.Redak row, string statusText, int maxLength)
   {
      return PrivateCore_CreateVvTextBoxLookUp(name, (int)col, (int)row, statusText, maxLength, 0, 0);
   }

   public VvTextBox CreateVvTextBoxLookUp(string name, int col, int row, string statusText, int maxLength, int numOfMergedWidths, int numOfMergedHeights)
   {
      return PrivateCore_CreateVvTextBoxLookUp(name, col, row, statusText, maxLength, numOfMergedWidths, numOfMergedHeights);
   }

   public VvTextBox CreateVvTextBoxLookUp(string name, ZXC.Kolona col, ZXC.Redak row, string statusText, int maxLength, int numOfMergedWidths, int numOfMergedHeights)
   {
      return PrivateCore_CreateVvTextBoxLookUp(name, (int)col, (int)row, statusText, maxLength, numOfMergedWidths, numOfMergedHeights);
   }

   private VvTextBox PrivateCore_CreateVvTextBoxLookUp(string name, int col, int row, string statusText, int maxLength, int numOfMergedWidths, int numOfMergedHeights)
   {
      VvTextBox tbxLookUp = VvTextBox.VvTextBoxLookUpFactory(name, statusText, maxLength, numOfMergedHeights);

      CreateAnyVvControl(tbxLookUp, col, numOfMergedWidths, row, numOfMergedHeights, 0);

      return tbxLookUp;
   }

   //public Button BtnChangeLookUpSelection
   //{
   //   get { return vvLookUpButton; }
   //}

   #endregion VvTextBoxLui

   #region RichTextBox

   public AdvRichTextBox CreateVvRichTextBox(int col, int row, string name, string text)
   {
      return PrivateCore_CreateVvRichTextBox(name, col, row, text, 0, 0, 0);
   }

   public AdvRichTextBox CreateVvRichTextBox(int col, int row, string name, string text, int maxLength)
   {
      return PrivateCore_CreateVvRichTextBox(name, col, row, text, maxLength, 0, 0);
   }

   public AdvRichTextBox CreateVvRichTextBox(ZXC.Kolona col, ZXC.Redak row, string name, string text)
   {
      return PrivateCore_CreateVvRichTextBox(name, (int)col, (int)row, text, 0, 0, 0);
   }

   public AdvRichTextBox CreateVvRichTextBox(ZXC.Kolona col, ZXC.Redak row, string name, string text, int maxLength)
   {
      return PrivateCore_CreateVvRichTextBox(name, (int)col, (int)row, text, maxLength, 0, 0);
   }

   public AdvRichTextBox CreateVvRichTextBox(int col, int row, string name, string text, int maxLength, int numOfMergedWidths, int numOfMergedHeights)
   {
      return PrivateCore_CreateVvRichTextBox(name, col, row, text, maxLength, numOfMergedWidths, numOfMergedHeights);
   }

   public AdvRichTextBox CreateVvRichTextBox(ZXC.Kolona col, ZXC.Redak row, string name, string text, int maxLength, int numOfMergedWidths, int numOfMergedHeights)
   {
      return PrivateCore_CreateVvRichTextBox(name, (int)col, (int)row, text, maxLength, numOfMergedWidths, numOfMergedHeights);
   }

   private AdvRichTextBox PrivateCore_CreateVvRichTextBox(string name, int col, int row, string text, int maxLength, int numOfMergedWidths, int numOfMergedHeights)
   {
      AdvRichTextBox rtBox = new AdvRichTextBox();
      rtBox.AutoSize       = false;
      rtBox.MaxLength      = maxLength;
      rtBox.Text           = text;
      rtBox.BorderStyle    = BorderStyle.Fixed3D;
      rtBox.Font           = ZXC.vvFont.RTBFont;
      rtBox.Name           = name;

      int firstLineOffset = 50;
      rtBox.SelectionAlignment     =  AdvRichTextBox.TextAlign.Justify;
      rtBox.SelectionIndent        =  firstLineOffset;
      rtBox.SelectionHangingIndent = -firstLineOffset;

      CreateAnyVvControl(rtBox, col, numOfMergedWidths, row, numOfMergedHeights, 0);

      //if(ZXC.ThisIsSurgerProject)
      //{
      //   //rtBox.ZoomFactor = 1.5f;
      //   rtBox.Font = new Font("Times New Roman", 10f);
      //}

      return rtBox;
   }

   #endregion RichTextBox

   #region VvRichTextBoxToolStrip

   public VvRichTextBoxToolStrip CreateVvRichTextBoxToolStrip(int col, int row, string name, AdvRichTextBox rtb)
   {
      return PrivateCore_CreateVvRichTextBoxToolStrip(name, col, row, rtb, 0, 0);
   }

   public VvRichTextBoxToolStrip CreateVvRichTextBoxToolStrip(ZXC.Kolona col, ZXC.Redak row, string name, AdvRichTextBox rtb)
   {
      return PrivateCore_CreateVvRichTextBoxToolStrip(name, (int)col, (int)row, rtb, 0, 0);
   }

   public VvRichTextBoxToolStrip CreateVvRichTextBoxToolStrip(int col, int row, string name, AdvRichTextBox rtb, int numOfMergedWidths, int numOfMergedHeights)
   {
      return PrivateCore_CreateVvRichTextBoxToolStrip(name, col, row, rtb, numOfMergedWidths, numOfMergedHeights);
   }

   public VvRichTextBoxToolStrip CreateVvRichTextBoxToolStrip(ZXC.Kolona col, ZXC.Redak row, string name, AdvRichTextBox rtb, int numOfMergedWidths, int numOfMergedHeights)
   {
      return PrivateCore_CreateVvRichTextBoxToolStrip(name, (int)col, (int)row, rtb, numOfMergedWidths, numOfMergedHeights);
   }

   private VvRichTextBoxToolStrip PrivateCore_CreateVvRichTextBoxToolStrip(string name, int col, int row, AdvRichTextBox rtb, int numOfMergedWidths, int numOfMergedHeights)
   {
      VvRichTextBoxToolStrip ts = new VvRichTextBoxToolStrip(rtb);
      ts.Name                   = name;


      CreateAnyVvControl(ts, col, numOfMergedWidths, row, numOfMergedHeights, 0);

      return ts;
   }

   
   
   #endregion VvRichTextBoxToolStrip

   #region VvComboBox

   public ComboBox CreateVvComboBox(int col, int row, string text, string name, ComboBoxStyle cBoxStyle)
   {
      return PrivateCore_CreateVvComboBox(col, row, text, 0, 0, name, cBoxStyle);
   }

   public ComboBox CreateVvComboBox(ZXC.Kolona col, ZXC.Redak row, string text, string name, ComboBoxStyle cBoxStyle)
   {
      return PrivateCore_CreateVvComboBox((int)col, (int)row, text, 0, 0, name, cBoxStyle);
   }

   public ComboBox CreateVvComboBox(int col, int row, string text, int numOfMergedWidths, int numOfMergedHeights, string name, ComboBoxStyle cBoxStyle)
   {
      return PrivateCore_CreateVvComboBox(col, row, text, numOfMergedWidths, numOfMergedHeights, name, cBoxStyle);
   }

   public ComboBox CreateVvComboBox(ZXC.Kolona col, ZXC.Redak row, string text, int numOfMergedWidths, int numOfMergedHeights, string name, ComboBoxStyle cBoxStyle)
   {
      return PrivateCore_CreateVvComboBox((int)col, (int)row, text, numOfMergedWidths, numOfMergedHeights, name, cBoxStyle);
   }

   private ComboBox PrivateCore_CreateVvComboBox(int col, int row, string text, int numOfMergedWidths, int numOfMergedHeights, string name, ComboBoxStyle cBoxStyle)
   {
      ComboBox cBox      = new ComboBox();
      cBox.Text          = text;
      cBox.Name          = name;
      cBox.DropDownStyle = cBoxStyle;
      cBox.BackColor     = ZXC.vvColors.vvTBoxReadOnly_False_BackColor;
      cBox.Font          = ZXC.vvFont.BaseFont;
       
      CreateAnyVvControl(cBox, col, numOfMergedWidths, row, numOfMergedHeights, 0);

      return cBox;

      //ComboBox NAPOMENA
      //ComboBoxStyle.DropDown     - The text portion is editable;
      //ComboBoxStyle.DropDownList - The user cannot directly edit the text portion, The list displays only if AutoCompleteMode is Suggest or SuggestAppend.;
   }

   #endregion VvComboBox

   #region VvButton

   public Button CreateVvButton(int col, int row, EventHandler eh, string text)
   {
      return PrivateCore_CreateVvButton(col, row, eh, text, 0, 0);
   }

   public Button CreateVvButton(ZXC.Kolona col, ZXC.Redak row, EventHandler eh, string text)
   {
      return PrivateCore_CreateVvButton((int)col, (int)row, eh, text, 0, 0);
   }

   public Button CreateVvButton(int col, int row, EventHandler eh, string text, int numOfMergedWidths, int numOfMergedHeights)
   {
      return PrivateCore_CreateVvButton(col, row, eh, text, numOfMergedWidths, numOfMergedHeights);
   }

   public Button CreateVvButton(ZXC.Kolona col, ZXC.Redak row, EventHandler eh, string text, int numOfMergedWidths, int numOfMergedHeights)
   {
      return PrivateCore_CreateVvButton((int)col, (int)row, eh, text, numOfMergedWidths, numOfMergedHeights);
   }

   private Button PrivateCore_CreateVvButton(int col, int row, EventHandler eh, string text, int numOfMergedWidths, int numOfMergedHeights)
   {
      Button button = new Button();
      button.Text   = text;
      if (eh != null)   button.Click += new EventHandler(eh);
   //   button.Font = ZXC.opt.BaseFont;// SystemFonts.MessageBoxFont;// ZXC.opt.SmallFont;
      button.Font = ZXC.vvFont.SmallFont;
      button.FlatStyle = FlatStyle.System;

      CreateAnyVvControl(button, col, numOfMergedWidths, row, numOfMergedHeights, 0);

      return button;
   }

   #endregion VvButton

   #region VvRadioButton
   
   public RadioButton CreateVvRadioButton(int col, int row, EventHandler eh, string text, TextImageRelation textImageRelation)
   {
      return PrivateCore_CreateVvRadioButton(col, row, eh, text, 0, 0, textImageRelation);
   }

   public RadioButton CreateVvRadioButton(ZXC.Kolona col, ZXC.Redak row, EventHandler eh, string text, TextImageRelation textImageRelation)
   {
      return PrivateCore_CreateVvRadioButton((int)col, (int)row, eh, text, 0, 0, textImageRelation);
   }

   public RadioButton CreateVvRadioButton(int col, int row, EventHandler eh, string text, int numOfMergedWidths, int numOfMergedHeights, TextImageRelation textImageRelation)
   {
      return PrivateCore_CreateVvRadioButton(col, row, eh, text, numOfMergedWidths, numOfMergedHeights, textImageRelation);
   }

   public RadioButton CreateVvRadioButton(ZXC.Kolona col, ZXC.Redak row, EventHandler eh, string text, int numOfMergedWidths, int numOfMergedHeights, TextImageRelation textImageRelation)
   {
      return PrivateCore_CreateVvRadioButton((int)col, (int)row, eh, text, numOfMergedWidths, numOfMergedHeights, textImageRelation);
   }

   private RadioButton PrivateCore_CreateVvRadioButton(int col, int row, EventHandler eh, string text, int numOfMergedWidths, int numOfMergedHeights, TextImageRelation textImageRelation)
   {
      RadioButton radioButton           = new RadioButton();
      radioButton.Text                  = text;
      if(eh != null) radioButton.Click += new EventHandler(eh);
      radioButton.TextImageRelation     = textImageRelation;
      radioButton.Name                  = text;
      radioButton.Font                  = ZXC.vvFont.SmallFont;

      CreateAnyVvControl(radioButton, col, numOfMergedWidths, row, numOfMergedHeights, 0);

      return radioButton;
   }

   public ushort GetCheckedRadioButtonAsUshort()
   {
      RadioButton checkedRadioButton = this.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Checked);

      if(checkedRadioButton == null) // mozda je GroupBox izmedju hampera i radioButtona
      {
         GroupBox gb = this.Controls.OfType<GroupBox>().FirstOrDefault();
         if(gb != null)
         {
            checkedRadioButton = gb.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Checked);
         }
      }

      return checkedRadioButton != null ? (ushort)checkedRadioButton.Tag : (ushort)0;
   }

   public void SetCheckedStateOfRadioButtonViaUshort(ushort ushortValue)
   {
      /*IEnumerable*/List<RadioButton> radioButtonList = this.Controls.OfType<RadioButton>().ToList();
      if(radioButtonList == null || radioButtonList.Count().IsZero()) // mozda je GroupBox izmedju hampera i radioButtona
      {
         GroupBox gb = this.Controls.OfType<GroupBox>().FirstOrDefault();
         if(gb != null)
         {
            radioButtonList = gb.Controls.OfType<RadioButton>().ToList();
         }
      }

      if(radioButtonList == null || radioButtonList.Count().IsZero()) return; // nismo uspjeli naci radioButtone 

      radioButtonList.ForEach(rb => rb.Checked = false); // programska promjena checkiranosti radioButtone NE odcheckirava prethodno checkiranoga! 
      radioButtonList.ForEach(rb => rb.Font = ZXC.vvFont.SmallFont); 

      // here we go 
      RadioButton radioButtonHavingThisUshortValueAsTag = this.Controls.OfType<RadioButton>().FirstOrDefault(x => (ushort)x.Tag == ushortValue);

      if(radioButtonHavingThisUshortValueAsTag == null) // mozda je GroupBox izmedju hampera i radioButtona
      {
         GroupBox gb = this.Controls.OfType<GroupBox>().FirstOrDefault();
         if(gb != null)
         {
            radioButtonHavingThisUshortValueAsTag = gb.Controls.OfType<RadioButton>().FirstOrDefault(x => (ushort)x.Tag == ushortValue);
         }
      }

      if(radioButtonHavingThisUshortValueAsTag != null)
      {
         radioButtonHavingThisUshortValueAsTag.Checked = true;
         radioButtonHavingThisUshortValueAsTag.Font    = ZXC.vvFont.BaseBoldFont;
      }
   }

   public void SetCheckedStateOfCheckBox(CheckBox cbx)
   {
      cbx.Font = cbx.Checked ? ZXC.vvFont.BaseBoldFont : ZXC.vvFont.SmallFont;
   }

   #endregion VvRadioButton

   #region VvCheckBox OLD

   internal static void SetVvPrefLink_AND_ToolTipText_ForGenericBiloGdjeUnazivuCheckBox(CheckBox cbx_biloGdjeUnazivu, EventHandler eventHandler_OnClick_SaveToVvPref, ToolTip ToolTipBiloGdjeUnazivu, string ToolTipTextBiloGdjeUnazivu)
   {
      cbx_biloGdjeUnazivu.Click += new EventHandler(eventHandler_OnClick_SaveToVvPref);
      ToolTipBiloGdjeUnazivu.SetToolTip(cbx_biloGdjeUnazivu, ToolTipTextBiloGdjeUnazivu);
   }
  
   public CheckBox CreateVvCheckBox_OLD(int col, int row, EventHandler eh, string text, RightToLeft txtRightLeft)
   {
      return PrivateCore_CreateVvCheckBox_OLD(col, row, eh, 0, 0, text, txtRightLeft, false);
   }

   public CheckBox CreateVvCheckBox_OLD(int col, int row, EventHandler eh, string text, RightToLeft txtRightLeft, bool defaultValueIsTrue)
   {
      return PrivateCore_CreateVvCheckBox_OLD(col, row, eh, 0, 0, text, txtRightLeft, defaultValueIsTrue);
   }

   public CheckBox CreateVvCheckBox_OLD(ZXC.Kolona col, ZXC.Redak row, EventHandler eh, string text, RightToLeft txtRightLeft)
   {
      return PrivateCore_CreateVvCheckBox_OLD((int)col, (int)row, eh, 0, 0, text, txtRightLeft, false);
   }

   public CheckBox CreateVvCheckBox_OLD(int col, int row, EventHandler eh, int numOfMergedWidths, int numOfMergedHeights, string text, RightToLeft txtRightLeft)
   {
      return PrivateCore_CreateVvCheckBox_OLD(col, row, eh, numOfMergedWidths, numOfMergedHeights, text, txtRightLeft, false);
   }

   public CheckBox CreateVvCheckBox_OLD(ZXC.Kolona col, ZXC.Redak row, EventHandler eh, int numOfMergedWidths, int numOfMergedHeights, string text, RightToLeft txtRightLeft)
   {
      return PrivateCore_CreateVvCheckBox_OLD((int)col, (int)row, eh, numOfMergedWidths, numOfMergedHeights, text, txtRightLeft, false);
   }

   private CheckBox PrivateCore_CreateVvCheckBox_OLD(int col, int row, EventHandler eh, int numOfMergedWidths, int numOfMergedHeights, string text, RightToLeft txtRightLeft, bool defaultValueIsTrue)
   {

      CheckBox chckBox              = new CheckBox();
      if(eh != null) chckBox.Click += new EventHandler(eh);
      chckBox.Font                  = ZXC.vvFont.SmallFont;
      //      chckBox.FlatStyle = FlatStyle.System;
      chckBox.Text                  = text;
      chckBox.RightToLeft           = txtRightLeft;

      if(defaultValueIsTrue)
      {
         chckBox.Tag = true;
      }
      
      CreateAnyVvControl(chckBox, col, numOfMergedWidths, row, numOfMergedHeights, 0);

      return chckBox;
   }

   #endregion VvCheckBox OLD

   #region VvListBox

   public ListBox CreateVvListBox(int col, int row, string text)
   {
      return PrivateCore_CreateVvListBox(col, row, text, 0, 0);
   }

   public ListBox CreateVvListBox( ZXC.Kolona col, ZXC.Redak row, string text)
   {
      return PrivateCore_CreateVvListBox((int)col, (int)row, text, 0, 0);
   }

   public ListBox CreateVvListBox(int col, int row, string text, int numOfMergedWidths, int numOfMergedHeights)
   {
      return PrivateCore_CreateVvListBox(col, row, text, numOfMergedWidths, numOfMergedHeights);
   }

   public ListBox CreateVvListBox(ZXC.Kolona col, ZXC.Redak row, string text, int numOfMergedWidths, int numOfMergedHeights)
   {
      return PrivateCore_CreateVvListBox((int)col, (int)row, text, numOfMergedWidths, numOfMergedHeights);
   }

   private ListBox PrivateCore_CreateVvListBox(int col, int row, string text, int numOfMergedWidths, int numOfMergedHeights)
   {
      ListBox lBox = new ListBox();
      lBox.Text    = text;

      CreateAnyVvControl(lBox, col, numOfMergedWidths, row, numOfMergedHeights, 0);

      return lBox;
   }

   #endregion VvListBox

   #region VvDateTimePicker

   public VvDateTimePicker CreateVvDateTimePicker(int col, int row, string text, VvTextBox twinTbx)
   {
      return PrivateCore_CreateVvDateTimePicker(col, row, text, 0, 0, twinTbx);
   }

   public VvDateTimePicker CreateVvDateTimePicker(ZXC.Kolona col, ZXC.Redak row, string text, VvTextBox twinTbx)
   {
      return PrivateCore_CreateVvDateTimePicker((int)col, (int)row, text, 0, 0, twinTbx);
   }

   public VvDateTimePicker CreateVvDateTimePicker(int col, int row, string text, int numOfMergedWidths, int numOfMergedHeights, VvTextBox twinTbx)
   {
      return PrivateCore_CreateVvDateTimePicker(col, row, text, numOfMergedWidths, numOfMergedHeights, twinTbx);
   }

   public VvDateTimePicker CreateVvDateTimePicker(ZXC.Kolona col, ZXC.Redak row, string text, int numOfMergedWidths, int numOfMergedHeights, VvTextBox twinTbx)
   {
      return PrivateCore_CreateVvDateTimePicker((int)col, (int)row, text, numOfMergedWidths, numOfMergedHeights, twinTbx);
   }

   private VvDateTimePicker PrivateCore_CreateVvDateTimePicker(int col, int row, string text, int numOfMergedWidths, int numOfMergedHeights, VvTextBox twinTbx)
   {

      VvDateTimePicker dtPicker = new VvDateTimePicker();
      dtPicker.BackColor        = ZXC.vvColors.vvTBoxHotOn_GotFocus_BackColor; 
      dtPicker.CustomFormat     = ZXC.VvDateFormat;
      dtPicker.Format           = DateTimePickerFormat.Custom;
      dtPicker.Font             = ZXC.vvFont.BaseFont;

      if(twinTbx != null)
      {
         dtPicker.TheVvTextBox     = twinTbx;
         dtPicker.TheVvTextBox.Tag = dtPicker;
         dtPicker.ValueChanged += new EventHandler(dtPicker.dtPicker_ValueChanged_SetDateFormat_SendValueToTextBox);

         dtPicker.Value = DateTimePicker.MinimumDateTime;

         dtPicker.TheVvTextBox.GotFocus += new EventHandler(dtPicker.theVvTextBox_GotFocus_ShowDTP);

         dtPicker.DropDown  += new EventHandler(dtPicker.dtPicker_DropDown_SetDateToday);
         dtPicker.LostFocus += new EventHandler(dtPicker.dtp_LostFocus_ShowTextBox);
         dtPicker.KeyPress  += new KeyPressEventHandler(dtPicker.dtp_KeyPress_StartTypingIfEmpty);
         dtPicker.KeyDown   += new KeyEventHandler(dtPicker.dtp_KeyDown_DeleteKeyEmptiesDTP);
         
         if(twinTbx.JAM_DataRequired == true)
         {
            dtPicker.Validating += new CancelEventHandler(dtPicker.dtp_Validating_SetDateRequired);
         }

         dtPicker.Visible              = false;
         dtPicker.TheVvTextBox.Visible = true;
         dtPicker.TheVvTextBox.TextAlign = HorizontalAlignment.Left;
       }
      // 23.02.2009: jel' ovo u redu? !
      //vvCheckBox.MinDate = ZXC.projectYearFirstDay;
      //vvCheckBox.MaxDate = ZXC.projectYearLastDay;
      // 24.2.2009: NIJE! 

      // 05.12.2017: ?!? start 
      if(twinTbx != null && dtPicker.TheVvTextBox.JAM_IsForDateTimePicker_WithTimeDisplay == true)
                            dtPicker.CustomFormat = ZXC.VvDateAndTimeFormat;
      else if(twinTbx != null && dtPicker.TheVvTextBox.JAM_IsForDateTimePicker_TimeOnlyDisplay == true)
      {
         dtPicker.CustomFormat = ZXC.VvTimeOnlyFormat;
         dtPicker.ShowUpDown = true; // 28.11.2017: 
      }
      else if(twinTbx != null && dtPicker.TheVvTextBox.JAM_IsForDateTimePicker_YearOnly == true)
         dtPicker.CustomFormat = ZXC.VvDateYyyyFormat;
    //else ... ovaj else ugasen tek naknadno jer ovdje CustomFormat treba ostati " " 
    //   dtPicker.CustomFormat = ZXC.VvDateFormat;

      // 05.12.2017: ?!? end 

      CreateAnyVvControl(dtPicker, col, numOfMergedWidths, row, numOfMergedHeights, 0);
      
      return dtPicker;
   }



   #endregion VvDateTimePicker
  
   #region VvCheckBox

   public VvCheckBox CreateVvCheckBox(int col, int row, string text, VvTextBox twinTbx, string falseText, string trueText)
   {
      return PrivateCore_CreateVvCheckBox(col, row, text, 0, 0, twinTbx, RightToLeft.No, falseText, trueText);
   }

   public VvCheckBox CreateVvCheckBox(ZXC.Kolona col, ZXC.Redak row, string text, VvTextBox twinTbx)
   {
      return PrivateCore_CreateVvCheckBox((int)col, (int)row, text, 0, 0, twinTbx, RightToLeft.No, "", "X");
   }

   public VvCheckBox CreateVvCheckBox(int col, int row, string text, int numOfMergedWidths, int numOfMergedHeights, VvTextBox twinTbx)
   {
      return PrivateCore_CreateVvCheckBox(col, row, text, numOfMergedWidths, numOfMergedHeights, twinTbx, RightToLeft.No, "", "X");
   }

   public VvCheckBox CreateVvCheckBox(ZXC.Kolona col, ZXC.Redak row, string text, int numOfMergedWidths, int numOfMergedHeights, VvTextBox twinTbx)
   {
      return PrivateCore_CreateVvCheckBox((int)col, (int)row, text, numOfMergedWidths, numOfMergedHeights, twinTbx, RightToLeft.No, "", "X");
   }

   private VvCheckBox PrivateCore_CreateVvCheckBox(int col, int row, string text, int numOfMergedWidths, int numOfMergedHeights, VvTextBox twinTbx, RightToLeft txtRightLeft, string falseText, string trueText)
   {
      VvCheckBox vvCheckBox       = new VvCheckBox(falseText, trueText);
      vvCheckBox.BackColor        = ZXC.vvColors.vvTBoxHotOn_GotFocus_BackColor; 
      vvCheckBox.Font             = ZXC.vvFont.SmallFont;
      vvCheckBox.Text             = text;
      vvCheckBox.RightToLeft      = txtRightLeft;

      if(twinTbx != null)
      {
         vvCheckBox.TheVvTextBox     = twinTbx;
         /* new!*/
         //vvCheckBox.TheVvTextBox.Font = new System.Drawing.Font("Comic Sans MS", this.Font.Size, FontStyle.Bold);
         vvCheckBox.TheVvTextBox.Tag = vvCheckBox;
         vvCheckBox.CheckedChanged += new EventHandler(vvCheckBox.vvCheckBox_CheckChanged_SendValueToTextBox);

         vvCheckBox.Checked = false;

         vvCheckBox.TheVvTextBox.MouseHover += new EventHandler(vvCheckBox.theVvTextBox_GotFocus_ShowCheckBox);
         vvCheckBox.TheVvTextBox.GotFocus   += new EventHandler(vvCheckBox.theVvTextBox_GotFocus_ShowCheckBox);

         vvCheckBox.LostFocus  += new EventHandler(vvCheckBox.checkBox_LostFocus_ShowTextBox);
         vvCheckBox.MouseLeave += new EventHandler(vvCheckBox.checkBox_LostFocus_ShowTextBox);
         //if(twinTbx.JAM_DataRequired == true)
         //{
         //   vvCheckBox.Validating += new CancelEventHandler(vvCheckBox.dtp_Validating_SetDateRequired);
         //}

         vvCheckBox.Visible              = false;
         vvCheckBox.TheVvTextBox.Visible = true;
         //vvCheckBox.TheVvTextBox.TextAlign = HorizontalAlignment.Left;
       }

      CreateAnyVvControl(vvCheckBox, col, numOfMergedWidths, row, numOfMergedHeights, 0);
      
      return vvCheckBox;
   }

   #endregion VvCheckBox
  
   #region CreateAnyVvControl

   private void CreateAnyVvControl(Control control, int col,int numOfMergedWidths, int row, int numOfMergedHeights, int pomak)
   {
      control.Parent = this.VvControlToBeParent;
     // privremeno DateTimePicker - textbox
     // CheckLocations(thisControl, col, numOfMergedWidths, row, numOfMergedHeights);

      control.Location = new Point(this.VvXxOfCol[col],          // X - coordinate 
                                   this.VvYyOfRow[row] + pomak); // Y - coordinate 

      ReserveLocations(control, col, numOfMergedWidths, row, numOfMergedHeights);

      if (control is Label) // tam 19.9. 4labVK na NalogDuc
      {
         control.Size = new Size(CalcMergedWidths (col, numOfMergedWidths),           // width  
                                 CalcMergedHeights(row, numOfMergedHeights) -(2 * pomak)); // height ('-1' je za onaj gore '+1' offset)
      }
      else
      {
         control.Size = new Size(CalcMergedWidths (col, numOfMergedWidths),           // width  
                                 CalcMergedHeights(row, numOfMergedHeights) - pomak); // height ('-1' je za onaj gore '+1' offset)
      }
   }

   #endregion CreateAnyVvControl

   #region Calc ..., Check ..., Reserve ...

   private void Calc_LocationAndSizes() 
   {
      int i, j, prevPixels, offset4text;

      //if (this.VvHavingGroupBox == true) offset4text = this.Font.Height * 4 / 5; 
      //19.09.07.tam kad smo uveli "novi" font

      if (this.VvHavingGroupBox == true) offset4text = ZXC.vvFont.SmallSmallFont.Height * 4 / 5;
      else                               offset4text = 0;

      this.minHamperWidth = this.minHamperHeight = 0;

      //_________________________________________________________
      for(i=0; i < VvNumOfCols; ++i)
      {
         for(j=0, prevPixels=0; j < i; ++j)
         {
            prevPixels += (VvSpcBefCol[j] + VvColWdt[j]);
         }
         aiXxCol[i] = prevPixels + VvSpcBefCol[i];
                                 
         this.minHamperWidth += (VvSpcBefCol[i] + VvColWdt[i]);
      }
      this.minHamperWidth += VvRightMargin;
      //_________________________________________________________
      this.minHamperHeight += offset4text;
      for(i=0; i < VvNumOfRows; ++i)
      {
         for(j=0, prevPixels=offset4text; j < i; ++j)
         {
            prevPixels += (VvSpcBefRow[j] + VvRowHgt[j]);
         }
         aiYyRow[i] = prevPixels + VvSpcBefRow[i];

         this.minHamperHeight += (VvSpcBefRow[i] + VvRowHgt[i]);
      }
      this.minHamperHeight += VvBottomMargin;
      //_________________________________________________________

      this.ClientSize        = new Size(this.minHamperWidth, this.minHamperHeight);
      if(this.VvGroupBox != null) this.VvGroupBox.Size = this.Size; 
   }

   int CalcMergedWidths(int col, int numOfMergedWidths)
   {
      int width;

      width = this.VvColWdt[col];
      for(int i=1; i <= numOfMergedWidths; ++i)
      {
         width += this.VvSpcBefCol[col+i] + this.VvColWdt[col+i];
      }

      return(width);
   }

   int CalcMergedHeights(int row, int numOfMergedHeights)
   {
      int height;

      height = this.VvRowHgt[row];
      for(int i=1; i <= numOfMergedHeights; ++i)
      {
         height += this.VvSpcBefRow[row+i] + this.VvRowHgt[row+i];
      }

      return(height);
   } 
   
   private void CheckLocations(Control control, int col, int numOfMergedWidths, int row, int numOfMergedHeights)
   {
      Point hamperKoordinata = new Point(col, row);

      if(col + numOfMergedWidths  > this.VvNumOfCols-1) throw new ArgumentOutOfRangeException("col", col + numOfMergedWidths,  String.Format("Smijes {0} a otisao si previse DESNO", this.VvNumOfCols-1));
      if(row + numOfMergedHeights > this.VvNumOfRows-1) throw new ArgumentOutOfRangeException("row", row + numOfMergedHeights, String.Format("Smijes {0} a otisao si previse DOLE",  this.VvNumOfRows-1));

      if(this.reservedLocations[col, row] == true)
         throw new ArgumentOutOfRangeException("Lokacija", hamperKoordinata, "Lokacija (hamper koordinata) vec ZAUZETA.");
   }

   private void ReserveLocations(Control control, int col, int numOfMergedWidths, int row, int numOfMergedHeights)
   {
      for(int rX = col; rX <= col+numOfMergedWidths;  ++rX) this.reservedLocations[rX,  row] = true;
      for(int rY = row; rY <= row+numOfMergedHeights; ++rY) this.reservedLocations[col, rY ] = true;
   }

   #endregion Calc ..., Check ..., Reserve ...
 
   #region properties
   /// <summary>
   /// Vv: Niz integera npr. { 10, 60, 10 } ... znaci tri kolone sirine 10, 60 i 10.
   /// </summary>
   public int[] VvColWdt
   {
      get { return aiCol;  }
      set 
      { 
         if (value.Length != this.VvNumOfCols) throw new ArgumentOutOfRangeException("VvColWdt");
         aiCol       = value; 
         Calc_LocationAndSizes();
      }
   } 
   /// <summary>
   /// Vv: Niz integera npr. { 20, 25, 20 } ... znaci tri retka visina 20, 25 i 20.
   /// </summary>
   public int[] VvRowHgt
   {
      get { return aiRow;  }
      set 
      { 
         if (value.Length != this.VvNumOfRows) throw new ArgumentOutOfRangeException("VvRowHgt");
         aiRow       = value; 
         Calc_LocationAndSizes();
      }
   }
   public int[] VvSpcBefCol
   {
      get { return aiSpcBefCol;  }
      set 
      { 
         if (value.Length != this.VvNumOfCols) throw new ArgumentOutOfRangeException("VvSpcBefCol");
         aiSpcBefCol = value;
         Calc_LocationAndSizes();
      }
   } 
   public int[] VvSpcBefRow
   {
      get { return aiSpcBefRow;  }
      set 
      { 
         if (value.Length != this.VvNumOfRows) throw new ArgumentOutOfRangeException("VvSpcBefRow");
         aiSpcBefRow = value;
         Calc_LocationAndSizes();
      }
   }

   /// <summary>
   /// Vv: Ljeva margina (Read Only) (zapravo je VvSpcBefCol[0])
   /// </summary>
   public int VvLeftMargin
   {
      get { return VvSpcBefCol[0];  }
   } 
   /// <summary>
   /// Vv: Gornja margina (Read Only) (zapravo je VvSpcBefRow[0])
   /// </summary>
   public int VvTopMargin
   {
      get { return VvSpcBefRow[0];  }
   } 
   /// <summary>
   /// Vv: Desna margina
   /// </summary>
   public int VvRightMargin
   {
      get { return iRightMargin;  }
      set { iRightMargin = value; Calc_LocationAndSizes(); }
   } 
   /// <summary>
   /// Vv: Donja margina
   /// </summary>
   public int VvBottomMargin
   {
      get { return iBottomMargin;  }
      set { iBottomMargin = value; Calc_LocationAndSizes(); }
   } 
   /// <summary>
   /// Vv: X koordinata kolone[thisTakesDataFromColIdx]  (Read Only)
   /// </summary>
   public int[] VvXxOfCol
   {
      get { return aiXxCol;  }
   } 
   /// <summary>
   /// Vv: Y koordinata retka[rowIdx]  (Read Only)
   /// </summary>
   public int[] VvYyOfRow
   {
      get { return aiYyRow;  }
   } 
   /// <summary>
   /// Vv: ajebote, pa broj kolona na panelu
   /// </summary>
   public int VvNumOfCols
   {
      get { return numOfCols;  }
      set { numOfCols = value; }
   } 
   /// <summary>
   /// Vv: ajebote, pa broj redova na panelu
   /// </summary>
   public int VvNumOfRows
   {
      get { return numOfRows;  }
      set { numOfRows = value; }
   } 
   /// <summary>
   /// Vv: Min width u pixelima (Read Only)
   /// </summary>
   public int VvMinHamperWidth
   {
      get { return minHamperWidth;  }
   } 
   /// <summary>
   /// Vv: Min Y Size u pixelima (Read Only) 
   /// </summary>
   public int VvMinHamperHeight
   {
      get { return minHamperHeight;  }
   } 
   /// <summary>
   /// Vv: Calculated ClientSize u pixelima (Read Only) 
   /// </summary>
   public Size VvCalculatedClientSize
   {
      get { return new Size(minHamperWidth, minHamperHeight); }
   } 
   /// <summary>
   /// Vv: oce / nece gledati crvene kvadratice (juzful at the design time)
   /// </summary>
   public bool VvShowCellsBounds
   {
      get { return showCellsBounds;  }
      set { showCellsBounds = value; }
   } 
   /// <summary>
   /// Vv: oce / nece gledati krizic / kvadratic (juzful at the design time)
   /// </summary>
   public bool VvShowHamperBounds
   {
      get { return showHamperBounds;  }
      set { showHamperBounds = value; }
   }
   /// <summary>
   /// Vv: oce / nece gledati okvir i tekst da GroupBox (rid onli)
   /// </summary>
   public bool VvHavingGroupBox
   {
      get { return havingGroupBox;  }
      // set { havingGroupBox = value; }
   }
   /// <summary>
   /// Vv: GroupBox koji opasuje panel
   /// </summary>
   public GroupBox VvGroupBox
   {
      get { return grBox;  }
      set { grBox = value; }
   }
   /// <summary>
   /// Vv: Container kao parent (this ako nema GroupBox-a)
   /// </summary>
   public Control VvControlToBeParent
   {
      get 
      { 
         if(VvHavingGroupBox == true) return VvGroupBox;
         else                         return this;
      }
   }

   public bool IsDUMMY
   {
      get { return isDUMMY; }
      set { isDUMMY = value; }
   }

   
   #endregion properties

   #region VvShowCellsBounds/VvShowHamperBounds
 
   protected override void OnPaint(PaintEventArgs pea)
   {
      Calc_LocationAndSizes();

      Graphics grfx = pea.Graphics;

      base.OnPaint(pea);

      if(this.VvShowCellsBounds == true) 
      {
         for(int col=0; col < this.VvNumOfCols; ++col)
            for(int row=0; row < this.VvNumOfRows; ++row)
               ZXC.DrawRectangleNonWeird(grfx, ZXC.penRed, this.VvXxOfCol[col], this.VvYyOfRow[row], this.VvColWdt[col], this.VvRowHgt[row]);
      }
      if(this.VvShowHamperBounds == true) 
      {
         grfx.DrawLine(ZXC.penGreen, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
         grfx.DrawLine(ZXC.penGreen, this.ClientSize.Width - 1, 0, 0, this.ClientSize.Height - 1);
         grfx.DrawLine(ZXC.penGreen, 0, 0, this.ClientSize.Width - 1, 0);
         grfx.DrawLine(ZXC.penGreen, 0, this.ClientSize.Height - 1, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
         grfx.DrawLine(ZXC.penGreen, 0, 0, 0, this.ClientSize.Height - 1);
         grfx.DrawLine(ZXC.penGreen, this.ClientSize.Width - 1, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
      }
   }

   public void VvEnableVisualDebugging()
   {
      foreach(Control hamperCtrl in this.Controls) 
      {
         hamperCtrl.Visible = false;
      }
      this.VvShowCellsBounds  = true;
      this.VvShowHamperBounds = true;
   }

   public void VvEnableVisual4CellsOnly()
   {
      foreach(Control hamperCtrl in this.Controls) 
      {
         hamperCtrl.Visible = false;
      }
      this.VvShowCellsBounds  = true;
   }

   #endregion VvShowCellsBounds/VvShowHamperBounds

   #region Open_Close_Fields_ForWriting

   public static void Open_Close_Fields_ForWriting(Control thisControl, ZXC.ZaUpis otvorenoIliZatvoreno, ZXC.ParentControlKind parentControlKind)
   {
      Open_Close_Fields_ForWriting(thisControl, otvorenoIliZatvoreno, false, parentControlKind);
   }

   public static void Open_Close_Fields_ForWriting(Control thisControl, ZXC.ZaUpis otvorenoIliZatvoreno, bool isESC, ZXC.ParentControlKind parentControlKind)
   {
      #region VvTextBox

      if(thisControl is VvTextBox)
      {

         VvTextBox vvTextBox = thisControl as VvTextBox;

         // upali ovo ako pozelis da ti ControlForInitialFocus ne bude tamno-plavo-selektirano
         // to si nekada htio iz nekog razloga!? 
         //vvTextBox.DeselectAll();
      
         #region JAM_ReadOnly

         if(vvTextBox.JAM_ReadOnly == true) // Ovo je polje vjecno ReadOnly! 
         {
            vvTextBox.ReadOnly = true;

            if(vvTextBox.JAM_IsVvLookUp)
            { 
              
            }

            if (((VvTextBox)thisControl).Tag is Color)
            {
               //if (parentControlKind == ZXC.ParentControlKind.TamponPanel_HeaderLeft) vvTextBox.BackColor = ZXC.vvColors.tamponHeaderLeftTbx_BackColor;
               //else 
               vvTextBox.BackColor = (Color)vvTextBox.Tag;
            }
            else
            {
               vvTextBox.BackColor = ZXC.vvColors.vvTBoxReadOnly_True_BackColor; 
            }

            if(vvTextBox.Tag is string && (string)vvTextBox.Tag == "AddModColor")
            {
               vvTextBox.BackColor = ZXC.vvColors.tamponPanel_BackColor;
               vvTextBox.ForeColor = ZXC.vvColors.userControl_BackColor;
               vvTextBox.BorderStyle = BorderStyle.None;
            }
            else if(vvTextBox.Tag is string && (string)vvTextBox.Tag == "AddPrjktColor")
            {
               vvTextBox.BackColor   = ZXC.vvColors.tamponPanel_BackColor;
               vvTextBox.ForeColor   = Color.OrangeRed;
               vvTextBox.BorderStyle = BorderStyle.None;
            }

            else if(vvTextBox.Tag is string && (string)vvTextBox.Tag == "Arhiva")
            {
               vvTextBox.BackColor   = ZXC.vvColors.tamponPanel_BackColor;
               vvTextBox.BorderStyle = BorderStyle.None;
               vvTextBox.ForeColor   = Color.Gold;

            }
            else if(vvTextBox.Tag is string && (string)vvTextBox.Tag == "ForeColorColor.Red")
            {
               vvTextBox.BackColor = ZXC.vvColors.vvTBoxReadOnly_True_BackColor;
               vvTextBox.ForeColor = Color.Red;
            }
            else
            {
               // ako hocs posebno dati foreColor na tamponu - kad hoces da neik tbx ima uvijek istu boju
               if(vvTextBox.JAM_ForeColor == null) vvTextBox.ForeColor = ZXC.vvColors.vvTBoxReadOnly_True_ForeColor;
               
               vvTextBox.BorderStyle = BorderStyle.Fixed3D;
            }
               vvTextBox.TabStop     = false;
         }

         #endregion JAM_ReadOnly

         else if(vvTextBox.JAM_ResultBox == true)
         {
            vvTextBox.BackColor = ZXC.vvColors.vvTBoxResultBox_True_BackColor;
            vvTextBox.ForeColor = ZXC.vvColors.vvTBoxResultBox_True_ForeColor;
        
            if((VvTextBox)thisControl != null && ((VvTextBox)thisControl).JAM_ForeColor != Color.Empty)
            {
               ((VvTextBox)thisControl).ForeColor = ((VvTextBox)thisControl).JAM_ForeColor;
            }

            if((VvTextBox)thisControl != null && ((VvTextBox)thisControl).JAM_BackColor != Color.Empty)
            {
               ((VvTextBox)thisControl).BackColor = ((VvTextBox)thisControl).JAM_BackColor;
            }

            vvTextBox.TabStop = false;
         }
         else
         {
            /*                                                                                       */
            /* */ vvTextBox.OnOpenOrCloseForEditActions(otvorenoIliZatvoreno, parentControlKind); /* */
            /*                                                                                       */

            if(vvTextBox.JAM_WriteOnly == true)
            {
               vvTextBox.BackColor = ZXC.vvColors.vvTBoxReadOnly_False_BackColor;
               vvTextBox.ForeColor = ZXC.vvColors.vvTBoxReadOnly_False_ForeColor;
            }
            if(((VvTextBox)thisControl).Tag is String && ((string)((VvTextBox)thisControl).Tag == "Visible_ only_ZXC.ZaUpis.Otvoreno"))
            {
               //((VvTextBox)thisControl).Visible = (otvorenoIliZatvoreno == ZXC.ZaUpis.Otvoreno);
               ((VvTextBox)thisControl).Visible = false;
            }
            if((VvTextBox)thisControl != null && ((VvTextBox)thisControl).JAM_ForeColor != Color.Empty)
            {
               ((VvTextBox)thisControl).ForeColor = ((VvTextBox)thisControl).JAM_ForeColor;
            }
            if((VvTextBox)thisControl != null && ((VvTextBox)thisControl).JAM_BackColor != Color.Empty)
            {
               ((VvTextBox)thisControl).BackColor = ((VvTextBox)thisControl).JAM_BackColor;
            }

         }
      }

      #endregion VvTextBox

      #region RichTextBox

      if(thisControl is AdvRichTextBox)
      {

         AdvRichTextBox richTextBox = thisControl as AdvRichTextBox;

         richTextBox.DeselectAll();

         richTextBox.ReadOnly = (otvorenoIliZatvoreno == ZXC.ZaUpis.Zatvoreno ? true : false);

         if(otvorenoIliZatvoreno == ZXC.ZaUpis.Otvoreno)
         {
            richTextBox.BackColor = ZXC.vvColors.vvTBoxReadOnly_False_BackColor; //ProfessionalColors.MenuItemSelectedGradientBegin;
            richTextBox.ForeColor = ZXC.vvColors.vvTBoxReadOnly_False_ForeColor;

            if(parentControlKind == ZXC.ParentControlKind.VvRecordUC)
            {
               richTextBox.TextChanged += new EventHandler(richTextBox_TextChanged);
            }
         }
         else if(otvorenoIliZatvoreno == ZXC.ZaUpis.Zatvoreno)
         {
            richTextBox.BackColor = ZXC.vvColors.vvTBoxReadOnly_True_BackColor;//SystemColors.ControlLightLight;
            richTextBox.ForeColor = ZXC.vvColors.vvTBoxReadOnly_True_ForeColor;

            if(parentControlKind == ZXC.ParentControlKind.VvRecordUC)
            {
               richTextBox.TextChanged -= new EventHandler(richTextBox_TextChanged);
            }
         }
      }

      #endregion RichTextBox

      #region VvRichTextBoxToolStrip

      if(thisControl is VvRichTextBoxToolStrip)
      {
         ((VvRichTextBoxToolStrip)thisControl).Visible = (otvorenoIliZatvoreno == ZXC.ZaUpis.Otvoreno);
      }

      #endregion VvRichTextBoxToolStrip

      #region Label

      if(thisControl is Label &&  ((Label)thisControl).Tag is String)// zato jer ima colorTag za labCrtu// Visible_ only_ZXC.ZaUpis.Otvoreno
      {
         if((string)((Label)thisControl).Tag == "Visible_ only_ZXC.ZaUpis.Otvoreno")
                      ((Label)thisControl).Visible = false;
      }

      #endregion Label

      #region CheckBox

      if(thisControl is CheckBox)
      {
         ((CheckBox)thisControl).AutoCheck = (otvorenoIliZatvoreno == ZXC.ZaUpis.Otvoreno);

         if(parentControlKind == ZXC.ParentControlKind.VvRecordUC)
         {
            if(otvorenoIliZatvoreno == ZXC.ZaUpis.Otvoreno) ((CheckBox)thisControl).CheckStateChanged += new EventHandler(CheckBox_CheckStateChanged);
            else                                            ((CheckBox)thisControl).CheckStateChanged -= new EventHandler(CheckBox_CheckStateChanged);

            //if(thisControl is VvCheckBox && thisControl.Tag is VvHamper)
            //{
            //   if(otvorenoIliZatvoreno == ZXC.ZaUpis.Otvoreno) ((CheckBox)thisControl).CheckStateChanged += new EventHandler(VvRecordUC/*ZXC.TheVvForm.TheVvRecordUC*/.CbxMigrator_CheckedChanged);
            //   else                                            ((CheckBox)thisControl).CheckStateChanged -= new EventHandler(VvRecordUC/*ZXC.TheVvForm.TheVvRecordUC*/.CbxMigrator_CheckedChanged);
            //}
         }
       
         if(thisControl is VvCheckBox && thisControl.Tag is VvHamper) ((CheckBox)thisControl).AutoCheck = true;
      }

      #endregion CheckBox

      #region Button

      if(thisControl is Button)
      {
         if(thisControl.Parent is VvTextBox)
         {
            if(otvorenoIliZatvoreno == ZXC.ZaUpis.Zatvoreno && ((VvTextBox)(thisControl.Parent)).JAM_WriteOnly == false)
            {
               ((Button)thisControl).Visible = false;
            }
            else
            {
               if(((VvTextBox)thisControl.Parent).ReadOnly == false)
               {
                  ((Button)thisControl).Visible = true;
               }
            }
         }

         if(thisControl.Name.StartsWith("btn_goExLink"))
         {
            thisControl.Visible = (otvorenoIliZatvoreno == ZXC.ZaUpis.Otvoreno ? true : false);
         }
      }

      #endregion Button

      #region RadioButton

      if(thisControl is RadioButton)
      {
        // if(((RadioButton)thisControl).Parent.Parent is VvFilterUC) //parentControlKind == ZXC.ParentControlKind.VvReportUC) ovak ne slajka jer mu kao parentKind dojde VvrecordUC,
        //                                                             ali onda ne sljaka kad je na VvReportUc a nije na VVFilterUC pa treba uzet u obzir i jedno ili drugo         
         if(((RadioButton)thisControl).Parent.Parent is VvFilterUC || parentControlKind == ZXC.ParentControlKind.VvReportUC) 
         {
           ((RadioButton)thisControl).AutoCheck = true;
           ((RadioButton)thisControl).Enabled   = true;
         }
         else
         {
            ((RadioButton)thisControl).AutoCheck = (otvorenoIliZatvoreno == ZXC.ZaUpis.Otvoreno);

            if(parentControlKind == ZXC.ParentControlKind.VvRecordUC)
            {
               if(otvorenoIliZatvoreno == ZXC.ZaUpis.Otvoreno) ((RadioButton)thisControl).CheckedChanged += new EventHandler(RadioButton_CheckChanged);
               else                                            ((RadioButton)thisControl).CheckedChanged -= new EventHandler(RadioButton_CheckChanged);
            }
         }
      } // if(thisControl is RadioButton)

      #endregion RadioButton

      #region VvDateTimePicker

      if(thisControl is VvDateTimePicker)
      {
         if(parentControlKind == ZXC.ParentControlKind.VvRecordUC)
         {
            if(otvorenoIliZatvoreno == ZXC.ZaUpis.Zatvoreno)
            {
               //           ((VvDateTimePicker)thisControl).Visible = false;
               ((VvDateTimePicker)thisControl).TextChanged -= new EventHandler(DateTimePickerEx_TextChanged);
            }
            else
            {
               //if((VvDateTimePicker)thisControl).parentControlKind == ZXC.ParentControlKind.VvReportUC)
               if(((VvDateTimePicker)thisControl).Tag is VvTextBox && ((VvTextBox)((VvDateTimePicker)thisControl).Tag).JAM_WriteOnly == true)
               {
                  //              ((VvDateTimePicker)thisControl).Visible                  = false;
                  ((VvDateTimePicker)thisControl).TextChanged -= new EventHandler(DateTimePickerEx_TextChanged);
               }
               else
               {
                  //             ((VvDateTimePicker)thisControl).Visible = true;
                  ((VvDateTimePicker)thisControl).TextChanged += new EventHandler(DateTimePickerEx_TextChanged);
               }
            }
         } // if(parentControlKind == ZXC.ParentControlKind.VvRecordUC)

      } // if(thisControl is VvDateTimePicker)

      #endregion VvDateTimePicker

      #region ComboBox

      if(thisControl is ComboBox)
      {
         if(otvorenoIliZatvoreno == ZXC.ZaUpis.Zatvoreno)
         {
            ((ComboBox)thisControl).Visible      = false;

            if(parentControlKind == ZXC.ParentControlKind.VvRecordUC)
            {
               ((ComboBox)thisControl).TextChanged -= new EventHandler(ComboBox_TextChanged);
            }

         }
         else
         {
            ((ComboBox)thisControl).Visible      = true;

            if(parentControlKind == ZXC.ParentControlKind.VvRecordUC)
            {
               ((ComboBox)thisControl).TextChanged += new EventHandler(ComboBox_TextChanged);
            }
         }
      } // if(thisControl is ComboBox)

      #endregion ComboBox

      #region DataGridView

      if(thisControl is DataGridView)
      {
         ((DataGridView)thisControl).ReadOnly = otvorenoIliZatvoreno == ZXC.ZaUpis.Zatvoreno ? true : false;

         //if ((string)((DataGridView)thisControl).Tag == "ROnly" || thisControl is DataGridViewFind)
         if (((DataGridView)thisControl).Name.StartsWith("SUM") || thisControl is VvDataGridViewFind)
         {
            ((DataGridView)thisControl).DefaultCellStyle.BackColor                = ZXC.vvColors.dataGridCellReadOnly_True_BackColor; //SystemColors.ControlLightLight;
            ((DataGridView)thisControl).DefaultCellStyle.ForeColor                = ZXC.vvColors.dataGridCellReadOnly_True_ForeColor;
            ((DataGridView)thisControl).ReadOnly = true;
            ((DataGridView)thisControl).TabStop  = false;

            ((DataGridView)thisControl).HorizontalScrollingOffset = 0;

            if(thisControl.Parent is VvInnerTabPage && ((VvInnerTabPage)thisControl.Parent).TheInnerTabPageKindEnum == ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage)
            {
               ((DataGridView)thisControl).ClearSelection();
            }
         }
         else if((string)((DataGridView)thisControl).Tag == "CheckBox" || (string)((DataGridView)thisControl).Tag == "Cooser2")
         {
            DataGridView dgv = thisControl as DataGridView;
            dgv.ReadOnly     = false;

            foreach(DataGridViewColumn col in dgv.Columns)
            {
               if(col.Name == "scrol") col.ReadOnly = true;
               else                    col.ReadOnly = false;
            }
         }
         
    #region "PRIVREMENO" koje se nigdje ne poziva ...
         // "PRIVREMENO"
         else if((string)((DataGridView)thisControl).Tag == "PRIVREMENO")
         {
            /*Vv*/DataGridView dgv = thisControl as /*Vv*/DataGridView;

            if(otvorenoIliZatvoreno == ZXC.ZaUpis.Zatvoreno)
            {
               thisControl.AllowDrop = false; // za DragAndDrop row reoredering 

               //dgv.CellClick -= new DataGridViewCellEventHandler(dgv.UpdateVvDataRecord_OnCellClick);
               //dgv.CellClick -= new DataGridViewCellEventHandler(dgv.UpdateVvLookUpItem_OnCellClick);
               //dgv.KeyDown -= new KeyEventHandler(dgv.UpdateVvDataRecord_OnKeyDown);
               //dgv.KeyDown -= new KeyEventHandler(dgv.UpdateVvLookUpItem_OnKeyDown);

               ZXC.TheVvForm.VvFlag_AllowGridAddOrDeleteRowsIsChanging = true;
               dgv.AllowUserToDeleteRows = false;
               dgv.AllowUserToAddRows = false;
               ZXC.TheVvForm.VvFlag_AllowGridAddOrDeleteRowsIsChanging = false;
            }
            else
            {
               thisControl.AllowDrop = true;

               //dgv.CellClick += new DataGridViewCellEventHandler(dgv.UpdateVvDataRecord_OnCellClick);
               //dgv.CellClick += new DataGridViewCellEventHandler(dgv.UpdateVvLookUpItem_OnCellClick);
               //dgv.KeyDown += new KeyEventHandler(dgv.UpdateVvDataRecord_OnKeyDown);
               //dgv.KeyDown += new KeyEventHandler(dgv.UpdateVvLookUpItem_OnKeyDown);

               //if(dgv.ThisIsOneRowFixedGrid == false)
               //{
                  ZXC.TheVvForm.VvFlag_AllowGridAddOrDeleteRowsIsChanging = true;
                  dgv.AllowUserToDeleteRows = true;
                  dgv.AllowUserToAddRows = true;
                  ZXC.TheVvForm.VvFlag_AllowGridAddOrDeleteRowsIsChanging = false;
               //}

            }

            VvTextBox vvtb;
            foreach(DataGridViewColumn col in dgv.Columns)
            {
               vvtb = col.Tag as VvTextBox;

               if(vvtb != null && vvtb.JAM_ReadOnly == true)
               {
                  col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor; //SystemColors.ControlLightLight;
                  col.DefaultCellStyle.ForeColor = ZXC.vvColors.dataGridCellReadOnly_True_ForeColor;
                  col.ReadOnly = true;
                  //col.TabStop = false;
               }

               else if(vvtb != null && vvtb.JAM_WriteOnly == true)
               {
                  throw new Exception("Ovo još nismo napravili !!!");
               }
               else
               {
                  if(otvorenoIliZatvoreno == ZXC.ZaUpis.Zatvoreno)
                  {
                     col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor; //SystemColors.ControlLightLight;
                     col.DefaultCellStyle.ForeColor = ZXC.vvColors.dataGridCellReadOnly_True_ForeColor;
                  }
                  else
                  {
                     col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_False_BackColor;
                     col.DefaultCellStyle.ForeColor = ZXC.vvColors.dataGridCellReadOnly_False_ForeColor;
                  }
               }
            } // foreach column... 

         }
         // "PRIVREMENO" 
  #endregion "PRIVREMENO" koje se nigdje ne poziva ...

         else
         {
            VvDataGridView dgv = thisControl as VvDataGridView;

           // if(dgv.Name == "")

           if(otvorenoIliZatvoreno == ZXC.ZaUpis.Zatvoreno)
           {
               thisControl.AllowDrop = false; // za DragAndDrop row reoredering 

               dgv.CellClick -= new DataGridViewCellEventHandler(dgv.UpdateVvDataRecord_OnCellClick);
               dgv.CellClick -= new DataGridViewCellEventHandler(dgv.UpdateVvLookUpItem_OnCellClick);
               //theGrid1.CellMouseDoubleClick -= new DataGridViewCellMouseEventHandler(theGrid1.UpdateVvDataRecord_OnCellMouseDoubleClick);
               dgv.KeyDown -= new KeyEventHandler(dgv.UpdateVvDataRecord_OnKeyDown);
               dgv.KeyDown -= new KeyEventHandler(dgv.UpdateVvLookUpItem_OnKeyDown);

               ZXC.TheVvForm.VvFlag_AllowGridAddOrDeleteRowsIsChanging = true;
               dgv.AllowUserToDeleteRows = false;
               dgv.AllowUserToAddRows    = false;
               ZXC.TheVvForm.VvFlag_AllowGridAddOrDeleteRowsIsChanging = false; 
            }
            else
            {
               thisControl.AllowDrop = true;

               dgv.CellClick += new DataGridViewCellEventHandler(dgv.UpdateVvDataRecord_OnCellClick);
               dgv.CellClick += new DataGridViewCellEventHandler(dgv.UpdateVvLookUpItem_OnCellClick);
               //theGrid1.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(theGrid1.UpdateVvDataRecord_OnCellMouseDoubleClick);
               dgv.KeyDown += new KeyEventHandler(dgv.UpdateVvDataRecord_OnKeyDown);
               dgv.KeyDown += new KeyEventHandler(dgv.UpdateVvLookUpItem_OnKeyDown);

               if(dgv.ThisIsOneRowFixedGrid == false)
               {
                  ZXC.TheVvForm.VvFlag_AllowGridAddOrDeleteRowsIsChanging = true; 
                  dgv.AllowUserToDeleteRows = true;
                  dgv.AllowUserToAddRows = true;
                  ZXC.TheVvForm.VvFlag_AllowGridAddOrDeleteRowsIsChanging = false;
               }

            }

            VvTextBox vvtb;
            foreach(DataGridViewColumn col in dgv.Columns)
            {
               vvtb = col.Tag as VvTextBox;

               if(vvtb != null && vvtb.JAM_ReadOnly == true)
               {
                  col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor; //SystemColors.ControlLightLight;
                  col.DefaultCellStyle.ForeColor = ZXC.vvColors.dataGridCellReadOnly_True_ForeColor;
                  col.ReadOnly = true;
                  //col.TabStop = false;
               }

               else if(vvtb != null && vvtb.JAM_WriteOnly == true)
               {
                  throw new Exception("Ovo još nismo napravili !!!");
               }
               else if(vvtb == null && col.Name == "scrol")
               {
                  col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor; //SystemColors.ControlLightLight;
                  col.DefaultCellStyle.ForeColor = ZXC.vvColors.dataGridCellReadOnly_True_ForeColor;
                  col.ReadOnly = true;

                  if((string)dgv.Tag == Mixer.TT_AVR)
                  {
                     col.DefaultCellStyle.BackColor = Color.Indigo;
                  }

               }
               // 16.09.2014.
             //else if(dgv.Parent != null && (string)dgv.Tag == Mixer.TT_MVR) 16.11.2017. dodan i AVR
               else if(dgv.Parent != null && ((string)dgv.Tag == Mixer.TT_MVR || (string)dgv.Tag == Mixer.TT_AVR))
               {
                       if(vvtb != null && col.HeaderText.Contains("NE"))      col.DefaultCellStyle.BackColor = Color.FromArgb(207, 183, 194);
                  else if(vvtb != null && col.HeaderText.Contains("SU"))      col.DefaultCellStyle.BackColor = Color.FromArgb(223, 207, 214);
                  else if(vvtb != null && col.Name      .StartsWith("t_str")) col.DefaultCellStyle.BackColor = Color.FromArgb(207, 214, 223);
                  else
                  {
                     if(otvorenoIliZatvoreno == ZXC.ZaUpis.Zatvoreno) col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;
                     else                                             col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_False_BackColor;
                  }
             
                  // Qukatz: isPraznik 
                  if(vvtb != null && col.HeaderText.EndsWith("*")) col.DefaultCellStyle.BackColor = Color.FromArgb(217, 139, 217 /*253, 185, 253*/);
               }
               else
               {
                  if(otvorenoIliZatvoreno == ZXC.ZaUpis.Zatvoreno)
                  {
                     col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor; //SystemColors.ControlLightLight;
                     col.DefaultCellStyle.ForeColor = ZXC.vvColors.dataGridCellReadOnly_True_ForeColor;
                  }
                  else
                  {

                     col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_False_BackColor;
                     col.DefaultCellStyle.ForeColor = ZXC.vvColors.dataGridCellReadOnly_False_ForeColor;
                  }
               }

               if(vvtb != null && vvtb.JAM_BackColor != Color.Empty)
               {
                  col.DefaultCellStyle.BackColor = vvtb.JAM_BackColor;
               }

               if(vvtb != null && vvtb.JAM_ForeColor != Color.Empty)
               {
                  col.DefaultCellStyle.ForeColor = vvtb.JAM_ForeColor;
               }

               if(vvtb == null && col is VvCheckBoxColumn && ((VvCheckBoxColumn)(col)).VvSupressClearingOnClearAllRowValues)
               {
                  for(int i = 0; i < dgv.Rows.Count; i++)
                  {
                     if(dgv.Rows[i].Cells[col.Name].Value != null && dgv.Rows[i].Cells[col.Name].Value.ToString() == "X")
                     {
                        foreach(DataGridViewTextBoxCell tbxCell in dgv.Rows[i].Cells)
                        {
                           tbxCell.Style.BackColor = Color.PaleGreen;
                        }
                     }

                  }
               }

            } // foreach column... 

         } // DataGridView.Tag != "ROnly" 

      } //if(thisControl is DataGridView)

      #endregion DataGridView


      if(parentControlKind == ZXC.ParentControlKind.VvRecordUC &&
         thisControl is VvDateTimePicker == false) //ovaj drugi uvjet dodan 28.09.2010
      {
         if(otvorenoIliZatvoreno == ZXC.ZaUpis.Otvoreno)
         {
            thisControl.KeyPress += new KeyPressEventHandler(PerformEscape_OnEscKeyPress);
         }
         else
         {
            thisControl.KeyPress -= new KeyPressEventHandler(PerformEscape_OnEscKeyPress);
         }
      }

      // REKURZIJA 
      foreach (Control childControl in thisControl.Controls)
      {
         Open_Close_Fields_ForWriting(childControl, otvorenoIliZatvoreno, parentControlKind);
      }

   }

   static void richTextBox_TextChanged(object sender, EventArgs e)
   {
      ZXC.TheVvForm.SetDirtyFlag(sender);
   }

   public static void CheckBox_CheckStateChanged(object sender, EventArgs e)
   {
      ZXC.TheVvForm.SetDirtyFlag(sender);
   }

   public static void RadioButton_CheckChanged(object sender, EventArgs e)
   {
      ZXC.TheVvForm.SetDirtyFlag(sender);
   }

   public static void DateTimePickerEx_TextChanged(object sender, EventArgs e)
   {
      ZXC.TheVvForm.SetDirtyFlag(sender);
   }

   public static void ComboBox_TextChanged(object sender, EventArgs e)
   {
      ZXC.TheVvForm.SetDirtyFlag(sender);
   }

   public static void PerformEscape_OnEscKeyPress(object sender, KeyPressEventArgs e)
   {
      if(e.KeyChar == (char)Keys.Escape)
      {
         // MessageBox.Show("esc " + sender.ToString());

         if(Form.ActiveForm is VvForm)
            ZXC.TheVvForm.EscapeAction_OnClick(null, EventArgs.Empty);
         else
            Form.ActiveForm.Close();
      }
   }

   public static void ClearFieldContents(Control thisControl)
   {
      if(thisControl is VvTextBox)
      {
         if(((VvTextBox)thisControl).Tag is string && (string)((VvTextBox)thisControl).Tag == "AddPrjktColor")
         { }
         else
         {
            ((VvTextBox)thisControl).Text = "";
         }
      }

      if(thisControl is AdvRichTextBox)
      {
         AdvRichTextBox rtb = thisControl as AdvRichTextBox;

         rtb.Clear();
         rtb.ZoomFactor = 1.00f;
         rtb.ZoomFactor = ((VvRichTextBoxToolStrip)(rtb.Tag)).PrefferedZoomFactor;

      }

      if(thisControl is VvDateTimePicker)
      {
         ((VvDateTimePicker)thisControl).Value = VvDateTimePicker.MinimumDateTime;
      }

      if(thisControl is CheckBox)
      {
         CheckBox cb = (CheckBox)thisControl;

         if(thisControl.Tag is VvHamper)
         {
            ((CheckBox)thisControl).Checked = ((CheckBox)thisControl).Checked;
         }
         else
         {
            if(cb.Tag is bool && ((bool)cb.Tag) == true)
               cb.Checked = true;
            else
               cb.Checked = false;
         }
         // 10.03.2014: 
         cb.Font = ZXC.vvFont.SmallFont;

      }

      if(thisControl is ComboBox)
      {
         ((ComboBox)thisControl).SelectedIndex = -1;
      }

      if(thisControl is RadioButton)
      {
         RadioButton rbt = (RadioButton)thisControl;
         bool isDefault;

         if(rbt.Tag == null ||
            !(rbt.Tag is bool)) 
            isDefault = false;
         else                
            isDefault = (bool)rbt.Tag;

         // Dakle, stavis rButtonu u Tag 'true', onome kojega zelis da bude default-an na ClearFieldContents 
         rbt.Checked = isDefault;

         // 10.03.2014: 
         rbt.Font = ZXC.vvFont.SmallFont;
      }

      //if(thisControl.GetType() == typeof(Button))
      //{
      //}

      foreach(Control childControl in thisControl.Controls)
      {
         ClearFieldContents(childControl);
      }
   }

   #endregion Open_Close_Fields_ForWriting

   #region VvFieldsEnabled_Disabled

   public static void Enable_Disable_Fields_ForWriting(Control thisControl, bool isEnable)
   {
      thisControl.Enabled = isEnable;

      // REKURZIJA 
      foreach(Control childControl in thisControl.Controls)
      {
         Enable_Disable_Fields_ForWriting(childControl, isEnable);
      }

   }

   public static void Unhide_PasswordTextBox_Text(Control thisControl)
   {
      if(thisControl is VvTextBox)
      {
         if((thisControl as VvTextBox).JAM_IsPassword) (thisControl as VvTextBox).UseSystemPasswordChar = false;
      }

      // REKURZIJA 
      foreach(Control childControl in thisControl.Controls)
      {
         Unhide_PasswordTextBox_Text(childControl);
      }

   }

   public static void SetChkBoxRadBttnAutoCheck(Control thisControl, bool isAutoCheck)
   {
      if(thisControl is CheckBox)
      {
         if(   ((CheckBox)thisControl).Parent.Parent               is VvFilterUC
            || ((CheckBox)thisControl).Parent.Parent.Parent.Parent is VvFilterUC  // faktur ubacen tab control pa .....
            || (thisControl is VvCheckBox && (((VvCheckBox)thisControl).TheVvTextBox != null))
            || ((CheckBox)thisControl).Tag is VvHamper) 
         {
            ((CheckBox)thisControl).AutoCheck = true;
         }
         else
         {
            ((CheckBox)thisControl).AutoCheck = isAutoCheck;
         }
      }

      if(thisControl is RadioButton)
      {
         if(((RadioButton)thisControl).Parent.Parent is VvFilterUC || ((RadioButton)thisControl).Parent.Parent.Parent.Parent is VvFilterUC) //mije radilo na Personu
         {
            ((RadioButton)thisControl).AutoCheck = true;
         }
         else
         {
            ((RadioButton)thisControl).AutoCheck = isAutoCheck;
         }
      }

      // REKURZIJA 
      foreach(Control childControl in thisControl.Controls)
      {
         SetChkBoxRadBttnAutoCheck(childControl, isAutoCheck);
      }
   }

   #endregion VvFieldsEnabled_Disabled

   #region AttachEscPressForEachControl

   public static void AttachEscPressForEachControl(Control thisControl)
   {
      if(thisControl is VvDateTimePicker == false) //ovaj uvjet dodan 28.09.2010
         thisControl.KeyPress += new KeyPressEventHandler(PerformEscape_OnEscKeyPress_NotOnVvForm);

      // REKURZIJA 
      foreach(Control childControl in thisControl.Controls)
      {
         AttachEscPressForEachControl(childControl);
      }
   }

   public static void PerformEscape_OnEscKeyPress_NotOnVvForm(object sender, KeyPressEventArgs e)
   {
      if(e.KeyChar == (char)Keys.Escape)
      {
         Form.ActiveForm.Close();
      }
   }

   #endregion AttachEscPressForEachControl

   #region ColorsAndStyles

   #region SetUpVisualStyle

   public static void SetUpVisualStyle(Crownwood.DotNetMagic.Common.VisualStyle visStyle)
   {
      switch (visStyle)
      {
         case VisualStyle.MediaPlayerBlue:
         case VisualStyle.MediaPlayerOrange:
         case VisualStyle.MediaPlayerPurple:

            StartPostavKaoMediaPlayer(visStyle);

            break;

         case VisualStyle.Office2007Blue:
         case VisualStyle.Office2007Silver:
         case VisualStyle.Office2007Black:
            ZXC.vvColors.userControl_BackColor          = Office2007ColorTable.SoftBackground(visStyle);
            ZXC.vvColors.modulPanel_BackColor           = Office2007ColorTable.LightBackground(visStyle);
            ZXC.vvColors.splitter_BackColor             = Office2007ColorTable.SoftBackground(visStyle);
            ZXC.vvColors.tamponPanel_BackColor          = Office2007ColorTable.BorderColor(visStyle);
            ZXC.vvColors.tamponPanel_ForeColor          = Color.White;
            ZXC.vvColors.tamponPanel_Crta               = Color.White;
            
            ZXC.vvColors.tamponHeaderLeftTbx_BackColor  = ZXC.vvColors.userControl_BackColor;
            ZXC.vvColors.hamperOnReportFilter_BackColor = Office2007ColorTable.SoftBackground(visStyle);
           
            ZXC.vvColors.tsPanel_BackColor = Office2007ColorTable.SoftBackground(visStyle);

            ZXC.vvColors.enableHeadersVisualStylesForDataGrid = false;
            ZXC.vvColors.dataGrid_GridColor              = Office2007ColorTable.SoftBackground(visStyle);
            ZXC.vvColors.dataGridColumnHeaders_BackColor = Office2007ColorTable.LightBackground(visStyle);
            ZXC.vvColors.dataGridColumnHeaders_ForeColor = Office2007ColorTable.StatusBarText(visStyle);
            ZXC.vvColors.dataGridRowHeaders_BackColor    = Office2007ColorTable.LightBackground(visStyle);
            ZXC.vvColors.dataGridRowHeaders_ForeColor    = Office2007ColorTable.StatusBarText(visStyle);
        
            ZXC.vvColors.modulButton_BackColor           = Office2007ColorTable.LightBackground(visStyle);
            ZXC.vvColors.modulButton_ForeColor           = Office2007ColorTable.EnhancedBackground(visStyle);
            ZXC.vvColors.modulButton_GradientColor       = Office2007ColorTable.SoftBackground(visStyle);
            
            ZXC.vvColors.reportModulButton_BackColor     = Office2007ColorTable.BorderColor(visStyle);
            
            break;

         case VisualStyle.Office2003:

            ZXC.vvColors.tamponPanel_BackColor = SystemColors.InactiveCaption;
            ZXC.vvColors.userControl_BackColor = SystemColors.ControlLightLight;
            ZXC.vvColors.modulPanel_BackColor  = ProfessionalColors.ImageMarginRevealedGradientBegin;
            ZXC.vvColors.splitter_BackColor    = SystemColors.Desktop;
            ZXC.vvColors.tamponPanel_ForeColor = Color.White;
            ZXC.vvColors.tamponPanel_Crta      = Color.White;
            ZXC.vvColors.tsPanel_BackColor     = ProfessionalColors.ImageMarginRevealedGradientBegin;

            ZXC.vvColors.hamperOnReportFilter_BackColor = SystemColors.ControlLightLight;
            ZXC.vvColors.tamponHeaderLeftTbx_BackColor  = SystemColors.ControlLightLight;

            ZXC.vvColors.enableHeadersVisualStylesForDataGrid = false;
            ZXC.vvColors.dataGrid_GridColor              = ControlPaint.LightLight(SystemColors.InactiveCaptionText);
            ZXC.vvColors.dataGridColumnHeaders_BackColor = ProfessionalColors.ToolStripGradientBegin;
            ZXC.vvColors.dataGridColumnHeaders_ForeColor = SystemColors.Desktop;
            ZXC.vvColors.dataGridRowHeaders_BackColor    = ProfessionalColors.ToolStripGradientBegin;
            //ZXC.vvColors.dataGridRowHeaders_BackColor    = ProfessionalColors.;
            
            break;

         case VisualStyle.IDE2005:
            ZXC.vvColors.tamponPanel_BackColor = SystemColors.ControlDarkDark;
            ZXC.vvColors.userControl_BackColor = SystemColors.ControlLightLight;
            ZXC.vvColors.modulPanel_BackColor  = SystemColors.ControlLightLight;
            ZXC.vvColors.splitter_BackColor    = SystemColors.Desktop;
            ZXC.vvColors.tamponPanel_ForeColor = Color.White;
            ZXC.vvColors.tamponPanel_Crta      = Color.White;
            ZXC.vvColors.tsPanel_BackColor     = SystemColors.ControlLight;

            ZXC.vvColors.hamperOnReportFilter_BackColor = SystemColors.ControlLightLight;
            ZXC.vvColors.tamponHeaderLeftTbx_BackColor  = SystemColors.ControlLightLight;

            ZXC.vvColors.enableHeadersVisualStylesForDataGrid = true;
            ZXC.vvColors.dataGrid_GridColor       = SystemColors.ControlLight;
            
            break;

         case VisualStyle.Plain:
            ZXC.vvColors.tamponPanel_BackColor = SystemColors.ControlDark;
            ZXC.vvColors.userControl_BackColor = SystemColors.ControlLightLight;
            ZXC.vvColors.modulPanel_BackColor  = SystemColors.Control;
            ZXC.vvColors.splitter_BackColor    = SystemColors.Desktop;
            ZXC.vvColors.tamponPanel_ForeColor = Color.White;
            ZXC.vvColors.tamponPanel_Crta      = Color.White;
            ZXC.vvColors.tsPanel_BackColor     = SystemColors.ControlLight;

            ZXC.vvColors.hamperOnReportFilter_BackColor = SystemColors.ControlLightLight;
            ZXC.vvColors.tamponHeaderLeftTbx_BackColor  = SystemColors.ControlLightLight;

            ZXC.vvColors.enableHeadersVisualStylesForDataGrid = true;
            ZXC.vvColors.dataGrid_GridColor                   = SystemColors.ControlLight;

            break;
         default:
            break;
      }

      //ZXC.vvColors.dataGrid_BackgroundColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;

   }

   public static void StartPostavKaoMediaPlayer(VisualStyle visStyle)
   {
      ZXC.vvColors.userControl_BackColor          =         //MediaPlayerColorTable.SoftBackground(visStyle);
      ZXC.vvColors.hamperOnReportFilter_BackColor =         //MediaPlayerColorTable.SoftBackground(visStyle);
      ZXC.vvColors.tsPanel_BackColor              =         //MediaPlayerColorTable.SoftBackground(visStyle);
      ZXC.vvColors.splitter_BackColor             = Office2007ColorTable.SoftBackground(VisualStyle.Office2007Black);        //MediaPlayerColorTable.SoftBackground(visStyle);

      ZXC.vvColors.modulPanel_BackColor  = MediaPlayerColorTable.LightBackground(visStyle);
      
      ZXC.vvColors.tamponPanel_BackColor = MediaPlayerColorTable.BorderColor(visStyle);
      ZXC.vvColors.tamponPanel_ForeColor = Color.White;
      ZXC.vvColors.tamponPanel_Crta      = Color.White;

      ZXC.vvColors.tamponHeaderLeftTbx_BackColor  = ZXC.vvColors.userControl_BackColor;

      
      ZXC.vvColors.enableHeadersVisualStylesForDataGrid = false;
      ZXC.vvColors.dataGrid_GridColor              = MediaPlayerColorTable.SoftBackground(visStyle);
      ZXC.vvColors.dataGridColumnHeaders_BackColor = MediaPlayerColorTable.LightBackground(visStyle);
      ZXC.vvColors.dataGridColumnHeaders_ForeColor = MediaPlayerColorTable.StatusBarText(visStyle);
      ZXC.vvColors.dataGridRowHeaders_BackColor    = MediaPlayerColorTable.LightBackground(visStyle);
      ZXC.vvColors.dataGridRowHeaders_ForeColor    = MediaPlayerColorTable.StatusBarText(visStyle);

      ZXC.vvColors.modulButton_BackColor     = MediaPlayerColorTable.LightBackground(visStyle);
      ZXC.vvColors.modulButton_ForeColor     = MediaPlayerColorTable.EnhancedBackground(visStyle);
      ZXC.vvColors.modulButton_GradientColor = MediaPlayerColorTable.SoftBackground(visStyle);

      ZXC.vvColors.reportModulButton_BackColor = MediaPlayerColorTable.BorderColor(visStyle);
   }

   #endregion SetUpVisualStyle

   public static void ApplyVVColorAndStyleVvForm(Control thisControl)   // toolStripPanel
   {
      if (thisControl is ToolStripPanel)
      {
         ((ToolStripPanel)thisControl).BackColor = ZXC.vvColors.tsPanel_BackColor;
      }
      if(thisControl is ToolStrip)
      {
         if(thisControl is MenuStrip) { }
         else
         {
            ((ToolStrip)thisControl).BackColor = ZXC.vvColors.tsPanel_BackColor;
         }
      }

      foreach (Control childControl in thisControl.Controls)
      {
         ApplyVVColorAndStyleVvForm(childControl);
      }
   }

   public static void ApplyVVColorAndStyleChangeOkolina(Control thisControl) // cmdPanel 
   {
      if (thisControl is Panel)
      {
         ((Panel)thisControl).BackColor = ZXC.vvColors.modulPanel_BackColor;
      }
      if (thisControl is Splitter)
      {
         ((Splitter)thisControl).BackColor = ZXC.vvColors.splitter_BackColor;
      }
      if (thisControl is Crownwood.DotNetMagic.Controls.TitleBar)
      {
         ((Crownwood.DotNetMagic.Controls.TitleBar)thisControl).Style               = ZXC.vvColors.vvform_VisualStyle;
         ((Crownwood.DotNetMagic.Controls.TitleBar)thisControl).BackColor           = ZXC.vvColors.modulButton_BackColor;
         ((Crownwood.DotNetMagic.Controls.TitleBar)thisControl).ForeColor           = ZXC.vvColors.modulButton_ForeColor;
         ((Crownwood.DotNetMagic.Controls.TitleBar)thisControl).GradientActiveColor = ZXC.vvColors.modulButton_GradientColor;

      }
      if (thisControl is Crownwood.DotNetMagic.Controls.ButtonWithStyle)
      {
         ((Crownwood.DotNetMagic.Controls.ButtonWithStyle)thisControl).Style = ZXC.vvColors.vvform_VisualStyle;

         if (((Crownwood.DotNetMagic.Controls.ButtonWithStyle)thisControl).Tag.GetType() == typeof(ZXC.Koordinata_3D))
         {
            ((Crownwood.DotNetMagic.Controls.ButtonWithStyle)thisControl).BackColor = ZXC.vvColors.reportModulButton_BackColor;
         }
      }

      foreach (Control childControl in thisControl.Controls)
      {
         ApplyVVColorAndStyleChangeOkolina(childControl);
      }
   }
  
   public static void ApplyVVColorAndStyleTreeControl(Control thisControl) 
   {
      if(thisControl is Crownwood.DotNetMagic.Controls.TreeControl)
      {
         ((Crownwood.DotNetMagic.Controls.TreeControl)thisControl).SetTreeControlStyle(ZXC.vvColors.treeControlStyle);
      }
   
      foreach(Control childControl in thisControl.Controls)
      {
         ApplyVVColorAndStyleChangeOkolina(childControl);
      }
   }

   public static void ApplyVVColorAndStyleTabCntrolChange(Control thisControl)  // TabControl
   {
      #region TabControl

      if (thisControl is Crownwood.DotNetMagic.Controls.TabControl)
      {
         ((Crownwood.DotNetMagic.Controls.TabControl)thisControl).OfficeStyle      = ZXC.vvColors.tabControl_OfficeStyle;
         ((Crownwood.DotNetMagic.Controls.TabControl)thisControl).Style            = ZXC.vvColors.vvform_VisualStyle;
         ((Crownwood.DotNetMagic.Controls.TabControl)thisControl).MediaPlayerStyle = ZXC.vvColors.tabControl_MediaPlayerStyle;
      }
      
      #endregion TabControl

      #region TabPage

      if (thisControl is Crownwood.DotNetMagic.Controls.TabPage)
      {
         if(thisControl.Tag is Color)
            ((Crownwood.DotNetMagic.Controls.TabPage)thisControl).BackColor = (Color)thisControl.Tag;
         else
         ((Crownwood.DotNetMagic.Controls.TabPage)thisControl).BackColor = ZXC.vvColors.userControl_BackColor;
      }

      #endregion TabPage

      #region VvTabPage

      if(thisControl is VvTabPage)
      {
         //if (!((VvTabPage)thisControl).IsForReport)
         if(((VvTabPage)thisControl).TabPageKind == ZXC.VvTabPageKindEnum.RECORD_TabPage)
         {
            if(((VvTabPage)thisControl).IsArhivaTabPage)
            {
               ((VvTabPage)thisControl).TamponPanel_Header.BackColor = Color.Gold;// AntiqueWhite;
               ((VvTabPage)thisControl).TamponPanel_Footer.BackColor = Color.Gold;
            }
            else if(((VvTabPage)thisControl).TheVvUC is FakturDUC && ((FakturDUC)((VvTabPage)thisControl).TheVvUC).IsShowingConvertedMoney ||
                    ((VvTabPage)thisControl).TheVvUC is NalogDUC  && ((NalogDUC) ((VvTabPage)thisControl).TheVvUC).IsShowingConvertedMoney)
            {
               ((VvTabPage)thisControl).TamponPanel_Header.BackColor = Color.GreenYellow;
               ((VvTabPage)thisControl).TamponPanel_Footer.BackColor = Color.GreenYellow;
            }
            else
            {
               ((VvTabPage)thisControl).TamponPanel_Header.BackColor = ZXC.vvColors.tamponPanel_BackColor;
               ((VvTabPage)thisControl).TamponPanel_Footer.BackColor = ZXC.vvColors.tamponPanel_BackColor;
            }
            //((VvTabPage)thisControl).labTamPanCrta.BackColor = ZXC.vvColors.tamponPanel_Crta;

         }
         else
         {
            ((VvTabPage)thisControl).TamponPanel_Header.BackColor = ZXC.vvColors.tamponPanel_BackColor;
         }

            ((VvTabPage)thisControl).TheVvUC.BackColor = ZXC.vvColors.userControl_BackColor;
            ((VvTabPage)thisControl).labTamPanCrta.BackColor = ZXC.vvColors.tamponPanel_Crta;


      }

      if(thisControl is Label)
      {
         if(thisControl.Tag is Color) ((Label)thisControl).BackColor = (Color)thisControl.Tag;
       //else ((Label)thisControl).BackColor = ZXC.vvColors.userControl_BackColor; // 6.12.2012. bey ovoga jer farba sve labele a treba nam samo labTamPanCrta i one koje imaju TagColor
      }

      #endregion VvTabPage

      #region Panel

      if(thisControl is Panel && (thisControl.Name == "FilterPanel" || thisControl.Name == "FilterPanelInner"))
      {
         ((Panel)thisControl).BackColor = ZXC.vvColors.userControl_BackColor;
      }

      #endregion Panel

      #region VvTextBox

      if (thisControl is VvTextBox)
      {
         if(((VvTextBox)thisControl).ReadOnly)
         {
            if (((VvTextBox)thisControl).Tag is Color)
            {
               //tamtam*************************************************************************************************
               //provjeri koliko ih ima tag colors
               //((VvTextBox)thisControl).BackColor = ZXC.vvColors.tamponHeaderLeftTbx_BackColor;
               ((VvTextBox)thisControl).BackColor = (Color)((VvTextBox)thisControl).Tag;
               ((VvTextBox)thisControl).ForeColor = ZXC.vvColors.vvTBoxReadOnly_True_ForeColor;
            }
            else if (((VvTextBox)thisControl).Tag is string && (string)((VvTextBox)thisControl).Tag == "AddModColor")
            {
               ((VvTextBox)thisControl).BackColor = ZXC.vvColors.tamponPanel_BackColor;
               ((VvTextBox)thisControl).ForeColor = ZXC.vvColors.userControl_BackColor;
            }
            else if(((VvTextBox)thisControl).Tag is string && (string)((VvTextBox)thisControl).Tag == "AddPrjktColor")
            {
               ((VvTextBox)thisControl).BackColor = ZXC.vvColors.tamponPanel_BackColor;
               ((VvTextBox)thisControl).ForeColor = Color.OrangeRed;
            }

            else if(((VvTextBox)thisControl).Tag is string && (string)((VvTextBox)thisControl).Tag == "Arhiva")
            {
               ((VvTextBox)thisControl).BackColor   = ZXC.vvColors.tamponPanel_BackColor;
               ((VvTextBox)thisControl).BorderStyle = BorderStyle.None;
               ((VvTextBox)thisControl).ForeColor   = Color.Gold;
            }
            else if(((VvTextBox)thisControl).Tag is string && (string)((VvTextBox)thisControl).Tag == "Fiskal")
            {
               FakturExtDUC theDUC = (FakturExtDUC)ZXC.TheVvForm.TheVvDocumentRecordUC;

               if(theDUC != null && Faktur.IsFiskalDutyTT_ONLINE(theDUC.Fld_TT, theDUC.Fld_NacPlac, theDUC.Fld_NacPlac2))
               {
                 if(((VvTextBox)thisControl).IsEmpty()) ((VvTextBox)thisControl).BackColor = Color.Red;
                 else                                   ((VvTextBox)thisControl).BackColor = ZXC.vvColors.vvTBoxReadOnly_True_BackColor;
               }
               else
               {
                  ((VvTextBox)thisControl).BackColor = ZXC.vvColors.vvTBoxReadOnly_True_BackColor;
               }
            }

            else
            {
               ((VvTextBox)thisControl).BackColor = ZXC.vvColors.vvTBoxReadOnly_True_BackColor;
               ((VvTextBox)thisControl).ForeColor = ZXC.vvColors.vvTBoxReadOnly_True_ForeColor;
            }

            if(((VvTextBox)thisControl).JAM_ResultBox)
            {
               ((VvTextBox)thisControl).BackColor = ZXC.vvColors.vvTBoxResultBox_True_BackColor;
               ((VvTextBox)thisControl).ForeColor = ZXC.vvColors.vvTBoxResultBox_True_ForeColor;
            }
            
            if((VvTextBox)thisControl != null && ((VvTextBox)thisControl).JAM_ForeColor != Color.Empty)
            {
               ((VvTextBox)thisControl).ForeColor = ((VvTextBox)thisControl).JAM_ForeColor;
            }
            if((VvTextBox)thisControl != null && ((VvTextBox)thisControl).JAM_BackColor != Color.Empty)
            {
               ((VvTextBox)thisControl).BackColor = ((VvTextBox)thisControl).JAM_BackColor;
            }
         }

         else if((VvTextBox)thisControl != null && ((VvTextBox)thisControl).JAM_ForeColor != Color.Empty)
         {
            ((VvTextBox)thisControl).ForeColor = ((VvTextBox)thisControl).JAM_ForeColor;
         }
         else
         {
   
            ((VvTextBox)thisControl).BackColor = ZXC.vvColors.vvTBoxReadOnly_False_BackColor;
            ((VvTextBox)thisControl).ForeColor = ZXC.vvColors.vvTBoxReadOnly_False_ForeColor;
         }
      }

      #endregion VvTextBox

      #region DataGridView

      if(thisControl is DataGridView)
      {

         ((DataGridView)thisControl).EnableHeadersVisualStyles = ZXC.vvColors.enableHeadersVisualStylesForDataGrid;
         ((DataGridView)thisControl).BackgroundColor           = ZXC.vvColors.dataGrid_BackgroundColor;
         ((DataGridView)thisControl).GridColor                 = ZXC.vvColors.dataGrid_GridColor;
         ((DataGridView)thisControl).ColumnHeadersDefaultCellStyle.ForeColor = ZXC.vvColors.dataGridColumnHeaders_ForeColor;
         ((DataGridView)thisControl).RowHeadersDefaultCellStyle.BackColor    = ZXC.vvColors.dataGridRowHeaders_BackColor;
         ((DataGridView)thisControl).RowHeadersDefaultCellStyle.ForeColor    = ZXC.vvColors.dataGridRowHeaders_ForeColor;

         ((DataGridView)thisControl).ColumnHeadersDefaultCellStyle.Font = ZXC.vvFont.BaseFont;
         ((DataGridView)thisControl).DefaultCellStyle.Font              = ZXC.vvFont.BaseFont;
         ((DataGridView)thisControl).RowHeadersDefaultCellStyle.Font    = ZXC.vvFont.BaseFont;

         if(((DataGridView)thisControl).Name == "Ptrans Grid") // kad je pocetna plava tablica
         {
            ((DataGridView)thisControl).ColumnHeadersDefaultCellStyle.BackColor = ZXC.vvColors.dataGridColumnHeaders_BackColor_Blue;
         }
         else
         {
            ((DataGridView)thisControl).ColumnHeadersDefaultCellStyle.BackColor = ZXC.vvColors.dataGridColumnHeaders_BackColor;
         }

         if(((DataGridView)thisControl).ReadOnly)
         {
            ((DataGridView)thisControl).DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;
            ((DataGridView)thisControl).DefaultCellStyle.ForeColor = ZXC.vvColors.dataGridCellReadOnly_True_ForeColor;
            ((DataGridView)thisControl).BackgroundColor            = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;

          //if((((DataGridView)thisControl).Name == FakturExtDUC.ptgOpl_TabPageName) ||
          //   (((DataGridView)thisControl).Name == FakturExtDUC.ptgDod_TabPageName) ||
          //   (((DataGridView)thisControl).Name == FakturExtDUC.ptgUna_Ana_TabPageName)
          //   )
          //{
          //   ((DataGridView)thisControl).ColumnHeadersDefaultCellStyle.Font = ZXC.vvFont.SmallSmallFont;
          //   ((DataGridView)thisControl).DefaultCellStyle.Font              = ZXC.vvFont.SmallFont;
          //   ((DataGridView)thisControl).RowHeadersDefaultCellStyle.Font    = ZXC.vvFont.SmallFont;
          //}
         }
         else
         {

            ((DataGridView)thisControl).DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_False_BackColor;
            ((DataGridView)thisControl).DefaultCellStyle.ForeColor = ZXC.vvColors.dataGridCellReadOnly_False_ForeColor;
         }


         VvTextBox vvtb;
         VvDataGridView dgv = thisControl as VvDataGridView;
         if(dgv != null)
         {
            foreach(DataGridViewColumn col in dgv.Columns)
            {
               vvtb = col.Tag as VvTextBox;

               if(vvtb == null && col is VvCheckBoxColumn && ((VvCheckBoxColumn)(col)).VvSupressClearingOnClearAllRowValues)
               {
                  for(int i = 0; i < dgv.Rows.Count; i++)
                  {
                     if(dgv.Rows[i].Cells[col.Name].Value != null && dgv.Rows[i].Cells[col.Name].Value.ToString() == "X")
                     {
                        foreach(DataGridViewTextBoxCell tbxCell in dgv.Rows[i].Cells)
                        {
                           tbxCell.Style.BackColor = Color.PaleGreen;
                        }
                     }
                  }
               }

               //if(vvtb != null && col.HeaderText.Contains("NE"))
               //   col.DefaultCellStyle.BackColor = Color.Bisque;
               //else
               //{
               //   if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;
               //   else                                                           col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_False_BackColor;
               //}


            }
         }
      }

      #endregion DataGridView

      foreach (Control childControl in thisControl.Controls)
      {
         ApplyVVColorAndStyleTabCntrolChange(childControl);
      }
   }

   #endregion ColorsAndStyles

   #region SetControlText_ThreadSafe()

   private delegate void Set_ControlText_CallBack  (Control theControl, string theText);
   private delegate void Set_ControlBackColor_CallBack  (Control theControl, Color theColor);
   public  static   void Set_ControlText_ThreadSafe(Control theControl, string theText)
   {
      // InvokeRequired required compares the thread ID of the
      // calling thread to the thread ID of the creating thread.
      // If these threads are different, it returns true.

      if(theControl.InvokeRequired)
      {
         Set_ControlText_CallBack d = new Set_ControlText_CallBack(Set_ControlText_ThreadSafe);
         try 
         { 
            theControl.Parent.Invoke(d, new object[] { theControl, theText }); 
         }
         catch(ObjectDisposedException) { } // ovi se Exception stalno dize pa ga moram ovako krotiti.
      }
      else
      {
         theControl.Text = theText;
      }
   }

   public  static   void Set_ControlBackColor_ThreadSafe(Control theControl, Color theColor)
   {
      // InvokeRequired required compares the thread ID of the
      // calling thread to the thread ID of the creating thread.
      // If these threads are different, it returns true.

      if(theControl.InvokeRequired)
      {
         Set_ControlBackColor_CallBack d = new Set_ControlBackColor_CallBack(Set_ControlBackColor_ThreadSafe);
         try 
         { 
            theControl.Parent.Invoke(d, new object[] { theControl, theColor }); 
         }
         catch(ObjectDisposedException) { } // ovi se Exception stalno dize pa ga moram ovako krotiti.
      }
      else
      {
         theControl.BackColor = theColor;
      }
   }
   private delegate string Get_ControlText_CallBack  (Control theControl);
   public  static   string Get_ControlText_ThreadSafe(Control theControl)
   {
      // InvokeRequired required compares the thread ID of the
      // calling thread to the thread ID of the creating thread.
      // If these threads are different, it returns true.

      string text = String.Empty;

      if(theControl.InvokeRequired)
      {
         Get_ControlText_CallBack d = new Get_ControlText_CallBack(Get_ControlText_ThreadSafe);
         try 
         { 
            text = (string)theControl.Parent.Invoke(d, new object[] { theControl }); 
         }
         catch(ObjectDisposedException) { } // ovi se Exception stalno dize pa ga moram ovako krotiti.

         return text;
      }
      else
      {
         return theControl.Text;
      }
   }

   private delegate object Get_ComboBoxSelectedItem_CallBack           (ComboBox theComboBox);
   public static    object Get_ComboBoxSelectedItem_CallBack_ThreadSafe(ComboBox theComboBox)
   {
      // InvokeRequired required compares the thread ID of the
      // calling thread to the thread ID of the creating thread.
      // If these threads are different, it returns true.

      object selectedItem = null;

      if(theComboBox.InvokeRequired)
      {
         Get_ComboBoxSelectedItem_CallBack d = new Get_ComboBoxSelectedItem_CallBack(Get_ComboBoxSelectedItem_CallBack_ThreadSafe);
         try 
         {
            selectedItem = theComboBox.Parent.Invoke(d, new object[] { theComboBox }); 
         }
         catch(ObjectDisposedException) { } // ovi se Exception stalno dize pa ga moram ovako krotiti.

         return selectedItem;
      }
      else
      {
         return theComboBox.SelectedItem;
      }
   }

   private delegate int Get_ToolStripComboBoxSelectedIndex_CallBack(ToolStripComboBox theComboBox);
   public static int Get_ToolStripComboBoxSelectedIndex_ThreadSafe(ToolStripComboBox theComboBox)
   {
      // InvokeRequired required compares the thread ID of the
      // calling thread to the thread ID of the creating thread.
      // If these threads are different, it returns true.

      int selectedIndex = 0;

      if(theComboBox.ComboBox.InvokeRequired)
      {
         Get_ToolStripComboBoxSelectedIndex_CallBack d = new Get_ToolStripComboBoxSelectedIndex_CallBack(Get_ToolStripComboBoxSelectedIndex_ThreadSafe);
         try
         {
            selectedIndex = (int)theComboBox.ComboBox.Parent.Invoke(d, new object[] { theComboBox });
         }
         catch(ObjectDisposedException) { } // ovi se Exception stalno dize pa ga moram ovako krotiti.

         return selectedIndex;
      }
      else
      {
         return theComboBox.SelectedIndex;
      }
   }

   private delegate void Set_ComboBoxSelectedItem_CallBack (ToolStripComboBox theComboBox, Object theItem );
   private delegate void Set_ComboBoxSelectedIndex_CallBack(ToolStripComboBox theComboBox, int    theIndex);
   public static void Set_ComboBoxSelectedItem_ThreadSafe(ToolStripComboBox theComboBox, Object theItem)
   {
      // InvokeRequired required compares the thread ID of the
      // calling thread to the thread ID of the creating thread.
      // If these threads are different, it returns true.

      if(theComboBox.ComboBox.InvokeRequired)
      {
         Set_ComboBoxSelectedItem_CallBack d = new Set_ComboBoxSelectedItem_CallBack(Set_ComboBoxSelectedItem_ThreadSafe);
         try
         {
            theComboBox.ComboBox.Parent.Invoke(d, new object[] { theComboBox, theItem });
         }
         catch(ObjectDisposedException) { } // ovi se Exception stalno dize pa ga moram ovako krotiti.
      }
      else
      {
         theComboBox.SelectedItem = theItem;
      }
   }

   public static void Set_ComboBoxSelectedIndex_ThreadSafe(ToolStripComboBox theComboBox, int theIndex)
   {
      // InvokeRequired required compares the thread ID of the
      // calling thread to the thread ID of the creating thread.
      // If these threads are different, it returns true.

      if(theComboBox.ComboBox.InvokeRequired)
      {
         Set_ComboBoxSelectedIndex_CallBack d = new Set_ComboBoxSelectedIndex_CallBack(Set_ComboBoxSelectedIndex_ThreadSafe);
         try
         {
            theComboBox.ComboBox.Parent.Invoke(d, new object[] { theComboBox, theIndex });
         }
         catch(ObjectDisposedException) { } // ovi se Exception stalno dize pa ga moram ovako krotiti.
      }
      else
      {
         theComboBox.SelectedItem = theIndex;
      }
   }

   private delegate void Set_DateTimePicker_CallBack  (VvDateTimePicker theDTP, DateTime theDate);
   public  static   void Set_DateTimePicker_ThreadSafe(VvDateTimePicker theDTP, DateTime theDate)
   {
      // InvokeRequired required compares the thread ID of the
      // calling thread to the thread ID of the creating thread.
      // If these threads are different, it returns true.

      if(theDTP.InvokeRequired)
      {
         Set_DateTimePicker_CallBack d = new Set_DateTimePicker_CallBack(Set_DateTimePicker_ThreadSafe);
         try 
         { 
            theDTP.Parent.Invoke(d, new object[] { theDTP, theDate }); 
         }
         catch(ObjectDisposedException) { } // ovi se Exception stalno dize pa ga moram ovako krotiti.
      }
      else
      {
         theDTP.Value = theDate;
      }
   }
   private delegate object Get_DateTimePicker_CallBack           (VvDateTimePicker theDTP);
   public static    object Get_DateTimePicker_CallBack_ThreadSafe(VvDateTimePicker theDTP)
   {
      // InvokeRequired required compares the thread ID of the
      // calling thread to the thread ID of the creating thread.
      // If these threads are different, it returns true.

      object dtpValue = null;

      if(theDTP.InvokeRequired)
      {
         Get_DateTimePicker_CallBack d = new Get_DateTimePicker_CallBack(Get_DateTimePicker_CallBack_ThreadSafe);
         try 
         {
            dtpValue = theDTP.Parent.Invoke(d, new object[] { theDTP }); 
         }
         catch(ObjectDisposedException) { } // ovi se Exception stalno dize pa ga moram ovako krotiti.

         return dtpValue;
      }
      else
      {
         return theDTP.Value;
      }
   }

   //private delegate void Set_TStripStatusLabel_CallBack(ToolStripStatusLabel theDTP, string theText);
   //public  static   void Set_TStripStatusLabel_ThreadSafe(ToolStripStatusLabel theTSSL, string theText)
   //{
   //   // InvokeRequired required compares the thread ID of the
   //   // calling thread to the thread ID of the creating thread.
   //   // If these threads are different, it returns true.
   //
   //   if(theTSSL.InvokeRequired)
   //   {
   //      Set_TStripStatusLabel_CallBack d = new Set_TStripStatusLabel_CallBack(Set_TStripStatusLabel_ThreadSafe);
   //      try 
   //      { 
   //         theTSSL. Parent.Invoke(d, new object[] { theTSSL, theText }); 
   //      }
   //      catch(ObjectDisposedException) { } // ovi se Exception stalno dize pa ga moram ovako krotiti.
   //   }
   //   else
   //   {
   //      theTSSL.Text = theText;
   //   }
   //}

   #endregion SetControlText_ThreadSafe()

   #region Create_Label4PrisilniScrollZbogGrida

   public static Label Create_Label4PrisilniScrollZbogGrida(Control grid, int minGridWidth)
   {
      // mora biti na istom parentu kao i TheG da TheG ne dobije horizontalni scroll
      Label label    = new Label(); 
      label.Parent   = grid.Parent;
      label.Size     = new Size(2, 2);
      label.Location = new Point(minGridWidth + grid.Location.X + ZXC.QUN*2, grid.Bottom + 1);
      label.Anchor   = AnchorStyles.Left | AnchorStyles.Bottom;

      return label;
   }

   #endregion Create_Label4PrisilniScrollZbogGrida

   #region HasFilterUC_AsChild_ShouldDisableWriteButtons

   public static bool HasFilterUC_AsChild_ShouldDisableWriteButtons(Control parentControl)
   {
      if(IsFilterUC(parentControl)&& parentControl.Visible == true) return true;

      foreach(Control childControl in parentControl.Controls)
      {
         if(HasFilterUC_AsChild_ShouldDisableWriteButtons(childControl)) return true;
      }

      return false;
   }

   private static bool IsFilterUC(Control control)
   {
      if(control is FinFilterUC) return true;
      if(control is PrvlgFilterUC) return true;

      return false;
   }

   #endregion HasFilterUC_AsChild_ShouldDisableWriteButtons

   #region DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda

   public static void DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(VvHamper hamper, int maxHampWidth, int razmakIzmjedjuHampera)
   {
      // pazi ovo nije pravi width vec mali trtmrt samo da se lijepo desno poravnaju hamperi ; 
      // to je right NAJDESNIJEG hampera - razmak.. koji je ujedno lijevi pomak 

      int hamperWidth = hamper.Right - razmakIzmjedjuHampera;

         if(hamperWidth >= maxHampWidth) hamper.VvColWdt[hamper.VvNumOfCols - 1] -= (hamperWidth - maxHampWidth);
         else                            hamper.VvColWdt[hamper.VvNumOfCols - 1] += (maxHampWidth - hamperWidth);

   }

   #endregion DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda

   #region Properties4HamperOnFilterUC

   public static void HamperStyling(VvHamper hamper)
   {
      hamper.BorderStyle = BorderStyle.FixedSingle;
   }
   
   #endregion Properties4HamperOnFilterUC

   #region Migrator Propertiz

   public Point VvInitialHamperLocation { get; set; }

   public bool  VvIsMigrateable         { get; set; }

   // TODO: pametnije. 
   public Control VvGetChildControlByName(string childControlName)
   {
      return this.Controls[childControlName];
   }

   public override string ToString()
   {
      return this.Name + "-" + base.ToString();
   }
   #endregion Migrator Propertiz


   internal static void AddLabelLine(VvHamper hamper)
   {
      hamper.VvRowHgt   [hamper.VvNumOfRows - 1] = ZXC.Qun10;
      hamper.VvSpcBefRow[hamper.VvNumOfRows - 1] = ZXC.Qun8;
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbCrta     = hamper.CreateVvLabel(0, hamper.VvNumOfRows - 1, "", ContentAlignment.MiddleLeft);
      lbCrta.Size      = new System.Drawing.Size(ZXC.Q10un + ZXC.Q5un, ZXC.Qun12);
      lbCrta.BackColor = Color.DarkGray;
   }

}
