using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraTab;
using DevExpress.LookAndFeel;

public class VvColorsStylsDlg : VvDialog
{
   private XtraTabControl tabControlColors;
   private XtraTabPage tabPageFormStyle, tabPageTabControlStyle;
   private Button resetButton, doneButton;
   private VvHamper hampFormStyle, hampTabCtrlColors;
   private int razmakHamp = ZXC.Qun4, nextX = 0, nextY = 0;
   private int tabCtrlWidth, tabCtrlHeight, dlgWidth, dlgHeight;
   public event EventHandler ResetEventHandler;
   private RadioButton[] aRBtnDxSkin;

   public VvColorsAndStyles oldValues;

   private Button btnVvTextBoxReadOnly_True_BackColor,
                  btnVvTextBoxReadOnly_True_ForeColor,
                  btnVvTextBoxReadOnly_False_BackColor,
                  btnVvTextBoxReadOnly_False_ForeColor,
                  btnVvTextBoxHotOn_GotFocus_BackColor,
                  btnVvTextBoxHotOn_GotFocus_ForeColor,
                  btnVvTextBoxHotOn_Find_BackColor,
                  btnVvTextBoxHotOn_Find_ForeColor,
                  btnDataGridCellReadOnly_True_BackColor,
                  btnDataGridCellReadOnly_True_ForeColor,
                  btnDataGridCellReadOnly_False_BackColor,
                  btnDataGridCellReadOnly_False_BackColorOdd,
                  btnDataGridCellReadOnly_False_ForeColor;

   public VvColorsStylsDlg()
   {
      InitializeTabControlColors();

      InitializeFormVisualStyleHamper(out hampFormStyle);
      nextY = 0;
      nextX = 0;
      InitializeTabControlColorsHamper(out hampTabCtrlColors);

      InitializeSize();
      tabControlColors.Size = new Size(tabCtrlWidth, tabCtrlHeight);
      this.ClientSize       = new Size(dlgWidth, dlgHeight);

      AddDoneResetButtons(out doneButton, out resetButton, dlgWidth, dlgHeight);
      doneButton.Anchor = resetButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      resetButton.Click += new System.EventHandler(this.button_Reset_Click);
   }


   # region Size

   private void InitializeSize()
   {
      tabCtrlHeight = Math.Max(hampFormStyle.Height, hampTabCtrlColors.Height) + ZXC.Q2un;
      tabCtrlWidth  = Math.Max(hampFormStyle.Width, hampTabCtrlColors.Width) + ZXC.Q2un;
      dlgWidth      = tabCtrlWidth;
      dlgHeight     = tabCtrlHeight + ZXC.QunBtnH + 2 * ZXC.QunMrgn;
   }

   # endregion Size

   #region Reset

   private void button_Reset_Click(object sender, System.EventArgs e)
   {
      if (ResetEventHandler != null)
         ResetEventHandler(this, new EventArgs());
   }

   #endregion Reset

   #region TabControl

   private void InitializeTabControlColors()
   {
      tabControlColors             = new XtraTabControl();
      tabControlColors.Parent      = this;
      tabControlColors.Location    = new Point(0, 0);
      tabControlColors.ClosePageButtonShowMode = ClosePageButtonShowMode.InAllTabPagesAndTabControlHeader;
      tabPageFormStyle             = new XtraTabPage();
      tabPageFormStyle.Text        = "FormStyle";
      tabControlColors.TabPages.Add(tabPageFormStyle);
      tabPageTabControlStyle       = new XtraTabPage();
      tabPageTabControlStyle.Text  = "Colors";
      tabControlColors.TabPages.Add(tabPageTabControlStyle);
    }

   #endregion TabControl

   #region TabPageFormStyle

   private void InitializeFormVisualStyleHamper(out VvHamper formVisualStyleHamper)
   {
      string[] dxSkinNames = new string[] { "Office 2019 Colorful", "Office 2019 Black", "The Bezier", "Visual Studio 2013 Light", "Office 2007 Silver" };
      string currentSkinName = VvForm.GetDxSkinNameFromEnvironment(ZXC.vvColors);

      formVisualStyleHamper = new VvHamper(1, dxSkinNames.Length, "", tabPageFormStyle, false, nextX + ZXC.Qun4, nextY + ZXC.Qun4, razmakHamp);

      formVisualStyleHamper.VvColWdt      = new int[] { ZXC.Q6un};
      formVisualStyleHamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      formVisualStyleHamper.VvRightMargin = formVisualStyleHamper.VvLeftMargin;

      for (int i = 0; i < formVisualStyleHamper.VvNumOfRows; i++)
      {
         formVisualStyleHamper.VvRowHgt[i]    = ZXC.QUN;
         formVisualStyleHamper.VvSpcBefRow[i] = ZXC.Qun5 ;
      }
      formVisualStyleHamper.VvBottomMargin = formVisualStyleHamper.VvTopMargin;

      aRBtnDxSkin = new RadioButton[dxSkinNames.Length];

      for (int i = 0; i < dxSkinNames.Length; i++)
      {
         aRBtnDxSkin[i]     = formVisualStyleHamper.CreateVvRadioButton(0, i, RadioBtn_FormStyle, dxSkinNames[i], TextImageRelation.ImageBeforeText);
         aRBtnDxSkin[i].Tag = dxSkinNames[i];

         if (currentSkinName == (string)aRBtnDxSkin[i].Tag)
         {
            aRBtnDxSkin[i].Checked = true;
         }
      }
   }

   void RadioBtn_FormStyle(object sender, EventArgs e)
   {
      RadioButton rBtn = sender as RadioButton;

      string skinName = (string)rBtn.Tag;

      ZXC.vvColors.DxSkinName = skinName;
      VvForm.ApplyDxSkin(skinName);
      // C20b: TabbedView nije Control — VvHamper.ApplyVVColorAndStyleTabCntrolChange ne može primiti njega.
      // TODO Faza 2i (V4 §3.2j): kompletan VvColors → SkinStyle refactor preuzima ovu logiku.
      // Skin engine (Office 2019 Colorful, postavljen u C18) auto-renderira boje glavnog tab kontejnera.
      //VvHamper.ApplyVVColorAndStyleTabCntrolChange(ZXC.TheVvForm.TheTabControl);
      VvHamper.ApplyVVColorAndStyleChangeOkolina(ZXC.TheVvForm.modulPanel);
      VvHamper.ApplyVVColorAndStyleVvForm(ZXC.TheVvForm);
   }
   
   #endregion TabPageFormStyle

   #region TabPageTabControlStyle

   private void InitializeTabControlColorsHamper(out VvHamper tabControlColorsHamper)
   {
      tabControlColorsHamper = new VvHamper(2, 13, "", tabPageTabControlStyle, true, nextX, nextY, razmakHamp);

      tabControlColorsHamper.VvColWdt      = new int[] { ZXC.Q9un + ZXC.QUN, ZXC.Q2un };
      tabControlColorsHamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      tabControlColorsHamper.VvRightMargin = tabControlColorsHamper.VvLeftMargin;

      for (int i = 0; i < tabControlColorsHamper.VvNumOfRows; i++)
      {
         tabControlColorsHamper.VvRowHgt[i]    = ZXC.QunBtnH;
         tabControlColorsHamper.VvSpcBefRow[i] = ZXC.Qun5;
      }
      tabControlColorsHamper.VvBottomMargin = tabControlColorsHamper.VvTopMargin;

      btnVvTextBoxReadOnly_True_BackColor           = tabControlColorsHamper.CreateVvButton(1,  0, Btn_Color, "");
      btnVvTextBoxReadOnly_True_ForeColor           = tabControlColorsHamper.CreateVvButton(1,  1, Btn_Color, "");
      btnVvTextBoxReadOnly_False_BackColor          = tabControlColorsHamper.CreateVvButton(1,  2, Btn_Color, "");
      btnVvTextBoxReadOnly_False_ForeColor          = tabControlColorsHamper.CreateVvButton(1,  3, Btn_Color, "");
      btnVvTextBoxHotOn_GotFocus_BackColor          = tabControlColorsHamper.CreateVvButton(1,  4, Btn_Color, "");
      btnVvTextBoxHotOn_GotFocus_ForeColor          = tabControlColorsHamper.CreateVvButton(1,  5, Btn_Color, "");
      btnVvTextBoxHotOn_Find_BackColor              = tabControlColorsHamper.CreateVvButton(1,  6, Btn_Color, "");
      btnVvTextBoxHotOn_Find_ForeColor              = tabControlColorsHamper.CreateVvButton(1,  7, Btn_Color, ""); 
      btnDataGridCellReadOnly_True_BackColor        = tabControlColorsHamper.CreateVvButton(1,  8, Btn_Color, "");
      btnDataGridCellReadOnly_True_ForeColor        = tabControlColorsHamper.CreateVvButton(1,  9, Btn_Color, "");
      btnDataGridCellReadOnly_False_BackColor       = tabControlColorsHamper.CreateVvButton(1, 10, Btn_Color, "");
      btnDataGridCellReadOnly_False_BackColorOdd    = tabControlColorsHamper.CreateVvButton(1, 11, Btn_Color, "");
      btnDataGridCellReadOnly_False_ForeColor       = tabControlColorsHamper.CreateVvButton(1, 12, Btn_Color, "");

      btnVvTextBoxReadOnly_True_BackColor.BackColor     = ZXC.vvColors.vvTBoxReadOnly_True_BackColor;
      btnVvTextBoxReadOnly_True_ForeColor.BackColor     = ZXC.vvColors.vvTBoxReadOnly_True_ForeColor;
      btnVvTextBoxReadOnly_False_BackColor.BackColor    = ZXC.vvColors.vvTBoxReadOnly_False_BackColor;
      btnVvTextBoxReadOnly_False_ForeColor.BackColor    = ZXC.vvColors.vvTBoxReadOnly_False_ForeColor;
      btnVvTextBoxHotOn_GotFocus_BackColor.BackColor    = ZXC.vvColors.vvTBoxHotOn_GotFocus_BackColor;
      btnVvTextBoxHotOn_GotFocus_ForeColor.BackColor    = ZXC.vvColors.vvTBoxHotOn_GotFocus_ForeColor;
      btnVvTextBoxHotOn_Find_BackColor.BackColor        = ZXC.vvColors.vvTBoxHotOn_Find_BackColor;
      btnVvTextBoxHotOn_Find_ForeColor.BackColor        = ZXC.vvColors.vvTBoxHotOn_Find_ForeColor;

      btnDataGridCellReadOnly_True_BackColor.BackColor     = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;
      btnDataGridCellReadOnly_True_ForeColor.BackColor     = ZXC.vvColors.dataGridCellReadOnly_True_ForeColor;
      btnDataGridCellReadOnly_False_BackColor.BackColor    = ZXC.vvColors.dataGridCellReadOnly_False_BackColor;
      btnDataGridCellReadOnly_False_BackColorOdd.BackColor = ZXC.vvColors.dataGridCellReadOnly_False_BackColorOdd;
      btnDataGridCellReadOnly_False_ForeColor.BackColor    = ZXC.vvColors.dataGridCellReadOnly_False_ForeColor;
  
      Label lblVvTextBoxReadOnly_True_BackColor        = tabControlColorsHamper.CreateVvLabel(0,  0, "TBox_ReadOnly_TRUE back"         , ContentAlignment.MiddleLeft);
      Label lblVvTextBoxReadOnly_True_ForeColor        = tabControlColorsHamper.CreateVvLabel(0,  1, "TBox_ReadOnly_TRUE fore"         , ContentAlignment.MiddleLeft);
      Label lblVvTextBoxReadOnly_False_BackColor       = tabControlColorsHamper.CreateVvLabel(0,  2, "TBox_ReadOnly_FALSE back"        , ContentAlignment.MiddleLeft);
      Label lblVvTextBoxReadOnly_False_ForeColor       = tabControlColorsHamper.CreateVvLabel(0,  3, "TBox_ReadOnly_FALSE fore"        , ContentAlignment.MiddleLeft);
      Label lblVvTextBoxHotOn_GotFocus_BackColor       = tabControlColorsHamper.CreateVvLabel(0,  4, "TBox_GotFocus back"              , ContentAlignment.MiddleLeft);
      Label lblVvTextBoxHotOn_GotFocus_ForeColor       = tabControlColorsHamper.CreateVvLabel(0,  5, "TBox_GotFocus fore"              , ContentAlignment.MiddleLeft);
      Label lblVvTextBoxHotOn_Find_BackColor           = tabControlColorsHamper.CreateVvLabel(0,  6, "TBox_Find back"                  , ContentAlignment.MiddleLeft);
      Label lblVvTextBoxHotOn_Find_ForeColor           = tabControlColorsHamper.CreateVvLabel(0,  7, "TBox_Find fore"                  , ContentAlignment.MiddleLeft);
      Label lblDataGridCellReadOnly_True_BackColor     = tabControlColorsHamper.CreateVvLabel(0,  8, "DATAGRID_ReadOnly_TRUE back"     , ContentAlignment.MiddleLeft);
      Label lblDataGridCellReadOnly_True_ForeColor     = tabControlColorsHamper.CreateVvLabel(0,  9, "DATAGRID_ReadOnly_TRUE fore"     , ContentAlignment.MiddleLeft);
      Label lblDataGridCellReadOnly_False_BackColor    = tabControlColorsHamper.CreateVvLabel(0, 10, "DATAGRID_ReadOnly_FALSE back"    , ContentAlignment.MiddleLeft);
      Label lblDataGridCellReadOnly_False_BackColorOdd = tabControlColorsHamper.CreateVvLabel(0, 11, "DATAGRID_ReadOnly_FALSE back Odd", ContentAlignment.MiddleLeft);
      Label lblDataGridCellReadOnly_False_ForeColor    = tabControlColorsHamper.CreateVvLabel(0, 12, "DATAGRID_ReadOnly_FALSE fore"    , ContentAlignment.MiddleLeft);
      ButtonFlStyle(tabControlColorsHamper);

   }

   private void ButtonFlStyle(Control ctrl)
   {
      if (ctrl is Button)
      {
         ((Button)ctrl).FlatStyle = FlatStyle.Flat;
      }
      foreach (Control childControl in ctrl.Controls)
      {
         ButtonFlStyle(childControl);
      }
   }

   #endregion TabPageTabControlStyle

   #region Btn_Color
   
   void Btn_Color(object sender, EventArgs e)
   {
      Button btnColors = sender as Button;

      VvColors br = new VvColors(btnColors.BackColor);

      br.ShowDialog();
      btnColors.BackColor = br.SelectedColor;

      ZXC.vvColors.vvTBoxReadOnly_True_BackColor           = btnVvTextBoxReadOnly_True_BackColor.BackColor;
      ZXC.vvColors.vvTBoxReadOnly_True_ForeColor           = btnVvTextBoxReadOnly_True_ForeColor.BackColor;
      ZXC.vvColors.vvTBoxReadOnly_False_BackColor          = btnVvTextBoxReadOnly_False_BackColor.BackColor;
      ZXC.vvColors.vvTBoxReadOnly_False_ForeColor          = btnVvTextBoxReadOnly_False_ForeColor.BackColor;
      ZXC.vvColors.vvTBoxHotOn_GotFocus_BackColor          = btnVvTextBoxHotOn_GotFocus_BackColor.BackColor ;
      ZXC.vvColors.vvTBoxHotOn_GotFocus_ForeColor          = btnVvTextBoxHotOn_GotFocus_ForeColor.BackColor ;
      ZXC.vvColors.vvTBoxHotOn_Find_BackColor              = btnVvTextBoxHotOn_Find_BackColor.BackColor;
      ZXC.vvColors.vvTBoxHotOn_Find_ForeColor              = btnVvTextBoxHotOn_Find_ForeColor.BackColor;
      ZXC.vvColors.dataGridCellReadOnly_True_BackColor     = btnDataGridCellReadOnly_True_BackColor.BackColor;
      ZXC.vvColors.dataGridCellReadOnly_True_ForeColor     = btnDataGridCellReadOnly_True_ForeColor.BackColor;
      ZXC.vvColors.dataGridCellReadOnly_False_BackColor    = btnDataGridCellReadOnly_False_BackColor.BackColor;
      ZXC.vvColors.dataGridCellReadOnly_False_BackColorOdd = btnDataGridCellReadOnly_False_BackColorOdd.BackColor;
      ZXC.vvColors.dataGridCellReadOnly_False_ForeColor    = btnDataGridCellReadOnly_False_ForeColor.BackColor;
   
      ZXC.vvColors.dataGrid_BackgroundColor             = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;

      // C20b: vidi komentar na liniji 157 (TabbedView nije Control; Faza 2i preuzima).
      //VvHamper.ApplyVVColorAndStyleTabCntrolChange(ZXC.TheVvForm.TheTabControl);

      br.Dispose();
   }

   #endregion Btn_Color

}