using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

public class VvEnvironmentDescriptor
{
   private bool[] miChecked;
   private bool   miRadnaOkolinaChecked;
   private bool   miSubRecordChecked;
   private bool   miSubCmdChecked;
   private Point  vvFormLocation;
   private Size   vvFormSize;
   private bool   topBottomTabControl;
   private bool   toolStripModulVisible;
   private bool   toolStripRecordVisible;
   private float  fontSizeInPoints;
   private string fontFamilyName;
   private Point  iconSizeAndText;
   private bool   isVvFormMaximized;
   private Point  ts_RecordLocation;
   private Point  ts_ModulLocation;
   private DockStyle         leftRightModulPanel;
   private VvColorsAndStyles vvColorsAndStylesEnv;

   public Point Ts_ModulLocation
   {
      get { return ts_ModulLocation; }
      set { ts_ModulLocation = value; }
   }

   public Point Ts_RecordLocation
   {
      get { return ts_RecordLocation; }
      set { ts_RecordLocation = value; }
   }

   public bool IsVvFormMaximized
   {
      get { return isVvFormMaximized; }
      set { isVvFormMaximized = value; }
   }
   
   
   public Point IconSizeAndText
   {
      get { return iconSizeAndText; }
      set { iconSizeAndText = value; }
   }

   private Color nekiColor;

   public Color NekiColor
   {
      get { return nekiColor; }
      set { nekiColor = value; }
   }

   public string FontFamilyName
   {
      get { return fontFamilyName; }
      set { fontFamilyName = value; }
   }

   public float FontSizeInPoints
   {
      get { return fontSizeInPoints; }
      set { fontSizeInPoints = value; }
   }
 
   public VvColorsAndStyles VvColorsAndStylesEnv
   {
      get { return vvColorsAndStylesEnv; }
      set { vvColorsAndStylesEnv = value; }
   }

   public bool MiRadnaOkolinaChecked
   {
      get { return miRadnaOkolinaChecked; }
      set { miRadnaOkolinaChecked = value; }
   }

   public bool ToolStripRecordVisible
   {
      get { return toolStripRecordVisible; }
      set { toolStripRecordVisible = value; }
   }

   public bool ToolStripModulVisible
   {
      get { return toolStripModulVisible; }
      set { toolStripModulVisible = value; }
   }

   public DockStyle LeftRightModulPanel
   {
      get { return leftRightModulPanel; }
      set { leftRightModulPanel = value; }
   }

   public bool TopBottomTabControl
   {
      get { return topBottomTabControl; }
      set { topBottomTabControl = value; }
   }

   public Size VvFormSize
   {
      get { return vvFormSize; }
      set { vvFormSize = value; }
   }

   public Point VvFormLocation
   {
      get { return vvFormLocation; }
      set { vvFormLocation = value; }
   } 

   public bool MiSubModulChecked
   {
      get { return miSubCmdChecked; }
      set { miSubCmdChecked = value; }
   }

   public bool MiSubRecordChecked
   {
      get { return miSubRecordChecked; }
      set { miSubRecordChecked = value; }
   }

   public bool[] MiChecked
   {
      get { return miChecked; }
      set { miChecked = value; }
   }


   // ---------------------- new era 

   public struct VvToolStripItem_State
   {
      public string name;
      public bool avaiable;
      public bool visible;

      public VvToolStripItem_State(string _name, bool _available, bool _visible)
      {
         this.name = _name;
         this.avaiable = _available;
         this.visible = _visible;
      }
   }

   //public VvToolStripItem_State[] TsRecordItems { get; set; }
   //public VvToolStripItem_State[] TsModulItems  { get; set; }
   public List<VvToolStripItem_State> TsRecordItems { get; set; }
   public List<VvToolStripItem_State> TsModulItems { get; set; }
}