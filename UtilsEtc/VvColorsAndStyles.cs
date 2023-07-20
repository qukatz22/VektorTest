using System;
using System.Drawing;
using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Controls;


public struct VvColorsAndStyles
{
   public VisualStyle vvform_VisualStyle;
   public OfficeStyle tabControl_OfficeStyle;
   public bool        enableHeadersVisualStylesForDataGrid;

   public Color   modulPanel_BackColor, splitter_BackColor, 
                  userControl_BackColor,

                  hamperOnReportFilter_BackColor,

                  hamperRules_BackColor,

                  tamponPanel_BackColor, tamponPanel_ForeColor, tamponPanel_Crta,
                  tamponHeaderLeftTbx_BackColor,
                  tamponHeaderLeftTbx__ForeColor,
      
                  vvTBoxReadOnly_True_BackColor,
                  vvTBoxReadOnly_True_ForeColor,
                  vvTBoxReadOnly_False_BackColor,
                  vvTBoxReadOnly_False_ForeColor,
                  vvTBoxHotOn_GotFocus_BackColor,
                  vvTBoxHotOn_GotFocus_ForeColor,
                  vvTBoxHotOn_Find_BackColor,
                  vvTBoxHotOn_Find_ForeColor,

                  dataGridCellReadOnly_True_BackColor,
                  dataGridCellReadOnly_True_ForeColor,
                  dataGridCellReadOnly_False_BackColor,
                  dataGridCellReadOnly_False_BackColorOdd,
                  dataGridCellReadOnly_False_ForeColor,
                  
                  dataGrid_BackgroundColor,
                  dataGrid_GridColor,
                  dataGridColumnHeaders_BackColor,
                  dataGridColumnHeaders_ForeColor,
                  dataGridRowHeaders_BackColor,
                  dataGridRowHeaders_ForeColor,

                  dataGridColumnHeaders_BackColor_Red,
                  dataGridColumnHeaders_BackColor_Blue,

                  tsPanel_BackColor,
                  mouseOverModulButton,
      
                  modulButton_BackColor,
                  modulButton_GradientColor,
                  modulButton_ForeColor,

                  reportModulButton_BackColor,

                  vvPanel4TBoxResultBox_BackColor,
                  vvTBoxResultBox_True_BackColor,
                  vvTBoxResultBox_True_ForeColor,

                  tabPage4TheG_BackColor,
                  tabPage4TheG2_BackColor,
                  tabPage4TheG3_BackColor,
                  clr_RAM_PTG, clr_HDD_PTG, clr_PCK_PTG;

   public int  modulPanel_BackColorArgb, splitter_BackColorArgb,
               userControl_BackColorArgb,
               
               hamperOnReportFilter_BackColorArgb,

               tamponPanel_BackColorArgb, tamponPanel_ForeColorArgb, tamponPanel_CrtaArgb,
               tamponHeaderLeftTbx__BackColorArgb,
               tamponHeaderLeftTbx__ForeColorArgb,

               vvTBoxReadOnly_True_BackColorArgb,
               vvTBoxReadOnly_True_ForeColorArgb,
               vvTBoxReadOnly_False_BackColorArgb,
               vvTBoxReadOnly_False_ForeColorArgb,
               vvTBoxHotOn_GotFocus_BackColorArgb,
               vvTBoxHotOn_GotFocus_ForeColorArgb,
               vvTBoxHotOn_Find_BackColorArgb,
               vvTBoxHotOn_Find_ForeColorArgb,

               dataGridCellReadOnly_True_BackColorArgb,
               dataGridCellReadOnly_True_ForeColorArgb,
               dataGridCellReadOnly_False_BackColorArgb,
               dataGridCellReadOnly_False_BackColorOddArgb,
               dataGridCellReadOnly_False_ForeColorArgb,

               dataGrid_BackgroundColorArgb,
               dataGrid_GridColorArgb,
               dataGridColumnHeaders_BackColorArgb,
               dataGridColumnHeaders_ForeColorArgb,
               dataGridRowHeaders_BackColorArgb,
               dataGridRowHeaders_ForeColorArgb,

                mouseOverCmdButtArgb;

   public TreeControlStyles treeControlStyle;
   public MediaPlayerStyle  tabControl_MediaPlayerStyle;
}