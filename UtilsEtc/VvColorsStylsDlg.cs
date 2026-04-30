using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using Crownwood.DotNetMagic.Controls;
using Crownwood.DotNetMagic.Common;

public class VvColorsStylsDlg : VvDialog
{
   private Crownwood.DotNetMagic.Controls.TabControl tabControlColors;
   private Crownwood.DotNetMagic.Controls.TabPage tabPageFormStyle, tabPageTabControlStyle, tabPageTreeControlStyle;
   private Button resetButton, doneButton;
   private VvHamper hampFormStyle, hampTabCtrlOfficeStyle, hampTabCtrlColors, hampTabCtrlMediaPlayerStyle, hampTreeControlStyle;
   private int razmakHamp = ZXC.Qun4, nextX = 0, nextY = 0;
   private int tabCtrlWidth, tabCtrlHeight, dlgWidth, dlgHeight;
   public event EventHandler ResetEventHandler;
   private RadioButton[] aRBtnDxSkin;
   private RadioButton[] aRBtnOffStyle;
   private RadioButton[] aRBtnTabCtrMedPlayStyle;
   private RadioButton[] aRBtnTreeControlStyle;

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
      InitializeTabControlStyleHamper(out hampTabCtrlOfficeStyle);
      InitializeTreeControlStyleHamper(out hampTreeControlStyle);

      InitializeTabControlStyleHamper(out hampTabCtrlOfficeStyle);
      nextY = hampTabCtrlOfficeStyle.Bottom + ZXC.Qun2 + ZXC.Qun10 - ZXC.Qun12;

      InitializeTabControlMediaPlayerStyle(out hampTabCtrlMediaPlayerStyle);

      nextY = 0;
      nextX = hampTabCtrlOfficeStyle.Right;
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
      tabCtrlHeight = hampTreeControlStyle.Height + ZXC.Q2un;
      tabCtrlWidth  = hampTabCtrlOfficeStyle.Width + hampTabCtrlColors.Width + ZXC.Q2un;
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
      tabControlColors              = new Crownwood.DotNetMagic.Controls.TabControl();
      tabControlColors.Parent       = this;
      tabControlColors.Location     = new Point(0, 0);
      tabControlColors.Appearance   = VisualAppearance.MultiDocument;
      tabControlColors.PositionTop  = true;
      tabControlColors.ShowClose    = false;
      tabControlColors.HotTrack     = true;
      tabControlColors.Style        = ZXC.vvColors.vvform_VisualStyle;
      tabPageFormStyle              = new Crownwood.DotNetMagic.Controls.TabPage("FormStyle");
      tabControlColors.TabPages.Add(tabPageFormStyle);
      tabPageTabControlStyle        = new Crownwood.DotNetMagic.Controls.TabPage("TabControlStyle");
      tabControlColors.TabPages.Add(tabPageTabControlStyle);

      if(ZXC.ThisIsVektorProject)
      {
         tabPageTreeControlStyle = new Crownwood.DotNetMagic.Controls.TabPage("TreeControlStyle");
         tabControlColors.TabPages.Add(tabPageTreeControlStyle);
      }
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

   private void InitializeTabControlStyleHamper(out VvHamper tabControlStyleHamper)
   {
      tabControlStyleHamper = new VvHamper(1, 8, "OfficeStyle", tabPageTabControlStyle, true, nextX, nextY, razmakHamp);

      tabControlStyleHamper.VvColWdt      = new int[] { ZXC.Q5un };
      tabControlStyleHamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      tabControlStyleHamper.VvRightMargin = tabControlStyleHamper.VvLeftMargin;

      for (int i = 0; i < tabControlStyleHamper.VvNumOfRows; i++)
      {
         tabControlStyleHamper.VvRowHgt[i]    = ZXC.QUN;
         tabControlStyleHamper.VvSpcBefRow[i] = ZXC.Qun5;
      }
      tabControlStyleHamper.VvBottomMargin = tabControlStyleHamper.VvTopMargin;

      OfficeStyle[] aOffStyle = new OfficeStyle[] {OfficeStyle.SoftWhite   , OfficeStyle.LightWhite   , OfficeStyle.DarkWhite,
                                                   OfficeStyle.SoftEnhanced, OfficeStyle.LightEnhanced, OfficeStyle.DarkEnhanced,
                                                   OfficeStyle.Light       , OfficeStyle.Dark};

      aRBtnOffStyle = new RadioButton[aOffStyle.Length];
      
      for (int i = 0; i < aOffStyle.Length; i++)
      {
         aRBtnOffStyle[i]     = tabControlStyleHamper.CreateVvRadioButton(0, i, radBtn_TabControl, aOffStyle[i].ToString(), TextImageRelation.ImageBeforeText);
         aRBtnOffStyle[i].Tag = aOffStyle[i];

         if ((OfficeStyle)aRBtnOffStyle[i].Tag == ZXC.vvColors.tabControl_OfficeStyle)
         {
            aRBtnOffStyle[i].Checked = true;
         }
      }
   }

   void radBtn_TabControl(object sender, EventArgs e)
   {
      RadioButton rBtn = sender as RadioButton;
      tabControlColors.OfficeStyle        = (OfficeStyle)rBtn.Tag;
      ZXC.vvColors.tabControl_OfficeStyle = (OfficeStyle)rBtn.Tag;

      VvHamper.SetUpVisualStyle(ZXC.vvColors.vvform_VisualStyle);
      // C20b: vidi komentar na liniji 157 (TabbedView nije Control; Faza 2i preuzima).
      //VvHamper.ApplyVVColorAndStyleTabCntrolChange(ZXC.TheVvForm.TheTabControl);
   }

   private void InitializeTabControlMediaPlayerStyle(out VvHamper hampTabCtrlMediaPlayerStyle)
   {
      hampTabCtrlMediaPlayerStyle = new VvHamper(1, 8, "MediaPlayerStyle", tabPageTabControlStyle, true, nextX, nextY, razmakHamp);

      hampTabCtrlMediaPlayerStyle.VvColWdt      = new int[] { ZXC.Q5un };
      hampTabCtrlMediaPlayerStyle.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hampTabCtrlMediaPlayerStyle.VvRightMargin = hampTabCtrlMediaPlayerStyle.VvLeftMargin;

      for(int i = 0; i < hampTabCtrlMediaPlayerStyle.VvNumOfRows; i++)
      {
         hampTabCtrlMediaPlayerStyle.VvRowHgt[i] = ZXC.QUN;
         hampTabCtrlMediaPlayerStyle.VvSpcBefRow[i] = ZXC.Qun5;
      }
      hampTabCtrlMediaPlayerStyle.VvBottomMargin = hampTabCtrlMediaPlayerStyle.VvTopMargin;

      MediaPlayerStyle[] aMediaPlayerStyle = new MediaPlayerStyle[] { MediaPlayerStyle.Dark        , MediaPlayerStyle.DarkEnhanced , MediaPlayerStyle.DarkWhite,
                                                                      MediaPlayerStyle.Light       , MediaPlayerStyle.LightEnhanced, MediaPlayerStyle.LightWhite,
                                                                      MediaPlayerStyle.SoftEnhanced, MediaPlayerStyle.SoftWhite};

      aRBtnTabCtrMedPlayStyle = new RadioButton[aMediaPlayerStyle.Length];

      for(int i = 0; i < aMediaPlayerStyle.Length; i++)
      {
         aRBtnTabCtrMedPlayStyle[i]     = hampTabCtrlMediaPlayerStyle.CreateVvRadioButton(0, i, radBtn_TabControlMedia, aMediaPlayerStyle[i].ToString(), TextImageRelation.ImageBeforeText);
         aRBtnTabCtrMedPlayStyle[i].Tag = aMediaPlayerStyle[i];

         if((MediaPlayerStyle)aRBtnTabCtrMedPlayStyle[i].Tag == ZXC.vvColors.tabControl_MediaPlayerStyle)
         {
            aRBtnTabCtrMedPlayStyle[i].Checked = true;
         }
      }
   }

   void radBtn_TabControlMedia(object sender, EventArgs e)
   {
      RadioButton rBtn                        = sender as RadioButton;
      tabControlColors.MediaPlayerStyle       = (MediaPlayerStyle)rBtn.Tag;
      ZXC.vvColors.tabControl_MediaPlayerStyle = (MediaPlayerStyle)rBtn.Tag;

      VvHamper.SetUpVisualStyle(ZXC.vvColors.vvform_VisualStyle);
      // C20b: vidi komentar na liniji 157 (TabbedView nije Control; Faza 2i preuzima).
      //VvHamper.ApplyVVColorAndStyleTabCntrolChange(ZXC.TheVvForm.TheTabControl);
   }

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

   #region TabPageTreeControlStyle

   private void InitializeTreeControlStyleHamper(out VvHamper hampTreeControlStyle)
   {
      hampTreeControlStyle = new VvHamper(1, 19, "TreeControlStyles", tabPageTreeControlStyle, true, nextX, nextY, razmakHamp);

      hampTreeControlStyle.VvColWdt      = new int[] { ZXC.Q7un };
      hampTreeControlStyle.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hampTreeControlStyle.VvRightMargin = hampTreeControlStyle.VvLeftMargin;

      for(int i = 0; i < hampTreeControlStyle.VvNumOfRows; i++)
      {
         hampTreeControlStyle.VvRowHgt[i] = ZXC.QUN;
         hampTreeControlStyle.VvSpcBefRow[i] = ZXC.Qun5;
      }
      hampTreeControlStyle.VvBottomMargin = hampTreeControlStyle.VvTopMargin;

      TreeControlStyles[] aTreeControlStyle = new TreeControlStyles[] {
                                                                        TreeControlStyles.StandardPlain         ,
                                                                        TreeControlStyles.StandardThemed        ,
                                                                        TreeControlStyles.Explorer              ,
                                                                        TreeControlStyles.Navigator             ,
                                                                        TreeControlStyles.Group                 ,
                                                                        TreeControlStyles.GroupOfficeLight      ,
                                                                        TreeControlStyles.GroupOfficeDark       ,
                                                                       // TreeControlStyles.List                  ,
                                                                        TreeControlStyles.GroupOfficeBlueLight  ,
                                                                        TreeControlStyles.GroupOfficeBlueDark   ,
                                                                        TreeControlStyles.GroupOfficeSilverLight,
                                                                        TreeControlStyles.GroupOfficeSilverDark ,
                                                                        TreeControlStyles.GroupOfficeBlackLight ,
                                                                        TreeControlStyles.GroupOfficeBlackDark  ,
                                                                        TreeControlStyles.GroupMediaBlueLight   ,
                                                                        TreeControlStyles.GroupMediaBlueDark    ,
                                                                        TreeControlStyles.GroupMediaOrangeLight ,
                                                                        TreeControlStyles.GroupMediaOrangeDark  ,
                                                                        TreeControlStyles.GroupMediaPurpleLight ,
                                                                        TreeControlStyles.GroupMediaPurpleDark 
      };

      aRBtnTreeControlStyle = new RadioButton[aTreeControlStyle.Length];

      for(int i = 0; i < aTreeControlStyle.Length; i++)
      {
         aRBtnTreeControlStyle[i] = hampTreeControlStyle.CreateVvRadioButton(0, i, radBtn_aTreeControlStyle, aTreeControlStyle[i].ToString(), TextImageRelation.ImageBeforeText);
         aRBtnTreeControlStyle[i].Tag = aTreeControlStyle[i];

         if((TreeControlStyles)aRBtnTreeControlStyle[i].Tag == ZXC.vvColors.treeControlStyle)
         {
            aRBtnTreeControlStyle[i].Checked = true;
         }
      }

   }

   void radBtn_aTreeControlStyle(object sender, EventArgs e)
   {
      RadioButton rBtn              = sender as RadioButton;
      ZXC.vvColors.treeControlStyle = (TreeControlStyles)rBtn.Tag;

      VvHamper.SetUpVisualStyle(ZXC.vvColors.vvform_VisualStyle);
      VvHamper.ApplyVVColorAndStyleTreeControl(ZXC.TheVvForm.TreeView_Modul);
   }

   #endregion TabPageTreeControlStyle

}