using System;
using System.Drawing;
using System.Windows.Forms;

// NOTE: Intentionally in the GLOBAL namespace — matches VvForm and the rest of
// Framework/*.cs (none of which declare a namespace). Extracted from VvForm
// (was nested in zVvForm\Moduls_CommandPanel.cs and zVvForm\Menus_ToolStrips.cs)
// as part of Phase 1a / C3 of the DevExpress migration (decouple-first strategy).
//
// VvSubModul plus its two dependency structs (VvSubMenu, VvReportSubModul) are
// moved together because VvSubModul's field/constructor signatures reference them.

public struct VvSubMenu
{
   public string subMenuText;
   public ZXC.vvMenuStyleEnum vvMenuStyle;
   public string btnName;
   public bool enabledInWriteMode;
   public Icon icon;
   public Keys shortKeys;
   public EventHandler evHandler;
   public string subMenuDescription;


   public VvSubMenu(string _subMenuText, ZXC.vvMenuStyleEnum _vvMenuStyle, string _btnName, bool _enabledInWriteMode,
                    Icon _icon, Keys _shortKeys, EventHandler _evHandler,
      string _subMenuDescription)
   {
      this.subMenuText        = _subMenuText;
      this.vvMenuStyle        = _vvMenuStyle;
      this.btnName            = _btnName;
      this.enabledInWriteMode = _enabledInWriteMode;
      this.icon               = _icon;
      this.shortKeys          = _shortKeys;
      this.evHandler          = _evHandler;
      this.subMenuDescription = _subMenuDescription;
   }
}

public struct VvReportSubModul
{
   public string reportName;
   public string shortReportName;
   public ZXC.VvReportEnum reportEnum;
   public int groupNum;

   public VvReportSubModul(string _subModulName, string _cbxName, ZXC.VvReportEnum _reportEnum, int _groupNum)
   {
      this.reportEnum = _reportEnum;
      this.reportName = _subModulName;
      this.shortReportName = _cbxName;
      this.groupNum = _groupNum;

   }
}

public struct VvSubModul
{
   public string          subModul_name;

   public bool IsReport { get { return subModulKindEnum == ZXC.VvSubModulKindEnum.REPORT_MENU; } }

   public VvReportSubModul[] aRptSubModuls;
   public string          subModul_shortName;
   public string          subModul_Description;
   public Icon            modulIcon;
   public string          ts_subModulSetName;
   public VvSubMenu[]     subModulSet;

   public ZXC.VvModulEnum        modulEnum;
   public ZXC.VvSubModulEnum     subModulEnum;
   public ZXC.VvSubModulKindEnum subModulKindEnum;

   public Point xy;
   public int   groupNum;

   public VvSubModul(bool isNull) // dummy parametar. Fora posto neda parameterless constructor...
   {
      //if(isNull)
      {
         this.modulEnum            = ZXC.VvModulEnum.MODUL_UNDEF;
         this.subModulEnum         = ZXC.VvSubModulEnum.FORBIDDEN;
         this.subModulKindEnum     = ZXC.VvSubModulKindEnum.UNDEF;
         this.subModul_name        = "UNDEF";
         this.modulIcon            = null;
         this.subModul_shortName   = "UNDEF";
         this.subModul_Description = "";
         this.aRptSubModuls        = null;
         this.ts_subModulSetName   = "";
         this.subModulSet          = null;
         this.xy                   = Point.Empty;
         this.groupNum             = 0;
      }
   }

   public VvSubModul(ZXC.VvSubModulEnum wantedSubModulEnum) // znaci da ovaj subModul NEMA VvRecListUC-a vidljivog na Modul Panel-u (Mixer objekti) 
   {
      //if(isNull)
      {
         this.modulEnum = ZXC.VvModulEnum.MODUL_UNDEF;
         //this.subModulEnum         = ZXC.VvSubModulEnum.FORBIDDEN;
         /* !!! */
         this.subModulEnum = wantedSubModulEnum;
         this.subModulKindEnum = ZXC.VvSubModulKindEnum.UNDEF;
         this.subModul_name = "UNDEF";
         this.modulIcon = null;
         this.subModul_shortName = "UNDEF";
         this.subModul_Description = "";
         this.aRptSubModuls = null;
         this.ts_subModulSetName = "";
         this.subModulSet = null;
         this.xy = Point.Empty;
         this.groupNum = 0;
      }
   }

   public VvSubModul(ZXC.VvModulEnum _modulEnum,
                     ZXC.VvSubModulEnum _subModulEnum,
                     ZXC.VvSubModulKindEnum _subModulKindEnum,
                     string _subModulName,
                     Icon _modulIcon,
                     string _subModulShortName,
                     string _sLvlDescription,
                     VvReportSubModul[] _aReportSubModuls,
                     string _ts_subModulSetName,
                     VvSubMenu[] _subModulSet,
                     int _groupNum)
   {
      this.modulEnum            = _modulEnum;
      this.subModulEnum         = _subModulEnum;
      this.subModulKindEnum     = _subModulKindEnum;
      this.subModul_name        = _subModulName;
      this.modulIcon            = _modulIcon;
      this.subModul_shortName   = _subModulShortName;
      this.subModul_Description = _sLvlDescription;
      this.aRptSubModuls        = _aReportSubModuls;
      this.ts_subModulSetName   = _ts_subModulSetName;
      this.subModulSet          = _subModulSet;

      this.xy = Point.Empty; // inicijalizira se kasnije u for(X), for(Y) petljama 

      this.groupNum = _groupNum;

   }
}
