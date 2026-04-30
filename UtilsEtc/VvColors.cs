using System;
using System.Drawing;
using System.Windows.Forms;
using Crownwood.DotNetMagic.Forms;
using Crownwood.DotNetMagic.Controls;
using Crownwood.DotNetMagic.Common;

public class VvColors : DotNetMagicForm
{
   private Button[] btnColor;
   private ButtonWithStyle[] btnName;
   private Color selectedColor;
   private Crownwood.DotNetMagic.Controls.TabControl tabControlBojice;
   private Crownwood.DotNetMagic.Controls.TabPage tabPageWeb, tabPageSystem, tabPageProf, tabPageOffice;
   private Color[] aSystemColors, aProfessionalColors, aColor, aOffice2007ColorTable/*, aMediaPlayerColorTable*/;
   private String[] aTextColorsProf, aTextColorsOffice;

   public Color SelectedColor
   {
      get { return selectedColor; }
      set { selectedColor = value; }
   }

   public VvColors(Color btnColors)
   {

      this.Size      = new Size(300, 600);
      this.BackColor = Color.White;

      selectedColor = btnColors;

      #region Colors[] ....

      aSystemColors = new Color[]{
      /* 1*/   SystemColors.ActiveCaptionText, 
      /* 2*/   SystemColors.Info,
      /* 3*/   SystemColors.ControlLight,
      /* 4*/   SystemColors.ButtonFace, 
      /* 5*/   SystemColors.ActiveBorder, 
      /* 6*/   SystemColors.ButtonShadow, 
      /* 7*/   SystemColors.AppWorkspace,
      /* 8*/   SystemColors.ControlDarkDark,
      /* 9*/   SystemColors.ControlText,
      /*10*/   SystemColors.InactiveCaptionText,
      /*11*/   SystemColors.GradientInactiveCaption, 
      /*12*/   SystemColors.InactiveCaption,
      /*13*/   SystemColors.Highlight,
      /*14*/   SystemColors.HotTrack,
      /*15*/   SystemColors.GradientActiveCaption, 
      /*16*/   SystemColors.Desktop,
      /*17*/   SystemColors.ActiveCaption
                                };

      aProfessionalColors = new Color[]{
     /* 1*/   ProfessionalColors.GripLight,
     /* 2*/   ProfessionalColors.ButtonSelectedGradientBegin, 
     /* 3*/   ProfessionalColors.ButtonSelectedGradientMiddle, 
     /* 4*/   ProfessionalColors.ButtonCheckedGradientBegin, 
     /* 5*/   ProfessionalColors.ButtonSelectedGradientEnd,
     /* 6*/   ProfessionalColors.ButtonCheckedGradientMiddle,
     /* 7*/   ProfessionalColors.CheckBackground,
     /* 8*/   ProfessionalColors.ButtonPressedGradientMiddle, 
     /* 9*/   ProfessionalColors.ButtonCheckedGradientEnd,
     /*10*/   ProfessionalColors.ButtonPressedGradientBegin,
     /*11*/   ProfessionalColors.ToolStripDropDownBackground,
     /*12*/   ProfessionalColors.SeparatorLight,
     /*13*/   ProfessionalColors.ImageMarginGradientBegin,
     /*14*/   ProfessionalColors.ImageMarginGradientMiddle,
     /*15*/   ProfessionalColors.ImageMarginRevealedGradientBegin, 
     /*16*/   ProfessionalColors.MenuStripGradientEnd, 
     /*17*/   ProfessionalColors.ButtonCheckedHighlight,
     /*18*/   ProfessionalColors.ImageMarginRevealedGradientMiddle,
     /*19*/   ProfessionalColors.MenuStripGradientBegin,
     /*20*/   ProfessionalColors.ButtonPressedHighlight,
     /*21*/   ProfessionalColors.OverflowButtonGradientBegin,
     /*22*/   ProfessionalColors.ImageMarginGradientEnd, 
     /*23*/   ProfessionalColors.ImageMarginRevealedGradientEnd,
     /*24*/   ProfessionalColors.SeparatorDark, 
     /*25*/   ProfessionalColors.OverflowButtonGradientMiddle,
     /*26*/   ProfessionalColors.ToolStripBorder, 
     /*27*/   ProfessionalColors.ButtonCheckedHighlightBorder,  
     /*28*/   ProfessionalColors.GripDark, 
     /*29*/   ProfessionalColors.OverflowButtonGradientEnd, 
     /*30*/   ProfessionalColors.MenuBorder,
     /*31*/   ProfessionalColors.ButtonPressedBorder
                                    };

      aColor = new Color[] {
         /*1*/Color.Black,              /*2*/Color.White,                /*3*/Color.DimGray,
         /*4*/Color.Gray,               /*5*/Color.DarkGray,             /*6*/Color.Silver,  
         /*7*/Color.LightGray,          /*8*/Color.Gainsboro,            /*9*/Color.WhiteSmoke,
         /*10*/Color.Maroon,            /*11*/Color.DarkRed,             /*12*/Color.Red,
         /*13*/Color.Brown,             /*14*/Color.Firebrick,           /*15*/Color.IndianRed,
         /*16*/Color.Snow,              /*17*/Color.LightCoral,          /*18*/Color.RosyBrown,
         /*19*/Color.MistyRose,         /*20*/Color.Salmon,              /*21*/Color.Tomato,
         /*22*/Color.DarkSalmon,        /*23*/Color.Coral,               /*24*/Color.OrangeRed,
         /*25*/Color.LightSalmon,       /*26*/Color.Sienna,              /*27*/Color.SeaShell, 
         /*28*/Color.Chocolate,         /*29*/Color.SaddleBrown,         /*30*/Color.SandyBrown,
         /*31*/Color.PeachPuff,         /*32*/Color.Peru,                /*33*/Color.Linen,
         /*34*/Color.Bisque,            /*35*/Color.DarkOrange,          /*36*/Color.BurlyWood,
         /*37*/Color.Tan,               /*38*/Color.AntiqueWhite,        /*39*/Color.NavajoWhite,
         /*40*/Color.BlanchedAlmond,    /*41*/Color.PapayaWhip,          /*42*/Color.Moccasin,
         /*43*/Color.Orange,            /*44*/Color.Wheat,               /*45*/Color.OldLace,
         /*46*/Color.FloralWhite,       /*47*/Color.DarkGoldenrod,       /*48*/Color.Goldenrod,
         /*49*/Color.Cornsilk,          /*50*/Color.Gold,                /*51*/Color.Khaki,
         /*52*/Color.LemonChiffon,      /*53*/Color.PaleGoldenrod,       /*54*/Color.DarkKhaki,
         /*55*/Color.Beige,             /*56*/Color.LightGoldenrodYellow,/*57*/Color.Olive,
         /*58*/Color.Yellow,            /*59*/Color.LightYellow,         /*60*/Color.Ivory,
         /*61*/Color.OliveDrab,         /*62*/Color.YellowGreen,         /*63*/Color.DarkOliveGreen,
         /*64*/Color.GreenYellow,       /*65*/Color.Chartreuse,          /*66*/Color.LawnGreen,
         /*67*/Color.DarkSeaGreen,      /*68*/Color.LightGreen,          /*69*/Color.ForestGreen,
         /*70*/Color.LimeGreen,         /*71*/Color.PaleGreen,           /*72*/Color.DarkGreen,
         /*73*/Color.Green,             /*74*/Color.Lime,                /*75*/Color.Honeydew,
         /*76*/Color.SeaGreen,          /*77*/Color.MediumSeaGreen,      /*78*/Color.SpringGreen,
         /*79*/Color.MintCream,         /*80*/Color.MediumSpringGreen,   /*81*/Color.MediumAquamarine,
         /*82*/Color.Aquamarine,        /*83*/Color.Turquoise,           /*84*/Color.LightSeaGreen,
         /*85*/Color.MediumTurquoise,   /*86*/Color.DarkSlateGray,       /*87*/Color.PaleTurquoise,
         /*88*/Color.Teal,              /*89*/Color.DarkCyan,            /*90*/Color.Cyan,
         /*91*/Color.Aqua,              /*92*/Color.LightCyan,           /*93*/Color.Azure,
         /*94*/Color.DarkTurquoise,     /*95*/Color.CadetBlue,           /*96*/Color.PowderBlue,
         /*97*/Color.LightBlue,         /*98*/Color.DeepSkyBlue,         /*99*/Color.SkyBlue,
         /*100*/Color.LightSkyBlue,     /*101*/Color.SteelBlue,          /*102*/Color.AliceBlue,
         /*103*/Color.DodgerBlue,       /*104*/Color.SlateGray,          /*105*/Color.LightSlateGray,
         /*106*/Color.LightSteelBlue,   /*107*/Color.CornflowerBlue,     /*108*/Color.RoyalBlue,
         /*109*/Color.MidnightBlue,     /*110*/Color.Lavender,           /*111*/Color.Navy, 
         /*112*/Color.DarkBlue,         /*113*/Color.MediumBlue,         /*114*/Color.Blue,
         /*115*/Color.GhostWhite,       /*116*/Color.SlateBlue,          /*117*/Color.DarkSlateBlue, 
         /*118*/Color.MediumSlateBlue,  /*119*/Color.MediumPurple,       /*120*/Color.BlueViolet,
         /*121*/Color.Indigo,           /*122*/Color.DarkOrchid,         /*123*/Color.DarkViolet,
         /*124*/Color.MediumOrchid,     /*125*/Color.Thistle,            /*126*/Color.Plum,
         /*127*/Color.Violet,           /*128*/Color.Purple,             /*129*/Color.DarkMagenta,
         /*130*/Color.Magenta,          /*131*/Color.Orchid,             /*132*/Color.MediumVioletRed,
         /*133*/Color.DeepPink,         /*134*/Color.HotPink,            /*135*/Color.LavenderBlush,
         /*136*/Color.PaleVioletRed,    /*137*/Color.Crimson,            /*138*/Color.Pink, 
         /*139*/Color.LightPink   
                           };

      aOffice2007ColorTable = new Color[] { 
      /* 1*/   Office2007ColorTable.SepLightColor(VisualStyle.Office2007Silver, OfficeStyle.Dark),
      /* 2*/   Office2007ColorTable.CheckedActiveDark(VisualStyle.Office2007Blue),
      /* 3*/   Office2007ColorTable.SelectedActiveDark(VisualStyle.Office2007Blue),
      /* 4*/   Office2007ColorTable.SelectedActiveLight(VisualStyle.Office2007Blue),
      /* 5*/   Office2007ColorTable.CheckedActiveLight(VisualStyle.Office2007Blue),
      /* 6*/   Office2007ColorTable.EnhancedBackground(VisualStyle.Office2007Blue),
      /* 7*/   Office2007ColorTable.TabHighlightTextColor(VisualStyle.Office2007Blue, OfficeStyle.Dark),
      /* 8*/   Office2007ColorTable.SepLightColor(VisualStyle.Office2007Blue, OfficeStyle.Dark),
      /* 9*/   Office2007ColorTable.SoftBackground(VisualStyle.Office2007Blue),
      /*10*/   Office2007ColorTable.TitleInactiveDark(VisualStyle.Office2007Blue),
      /*11*/   Office2007ColorTable.TitleInactiveLight(VisualStyle.Office2007Blue),
      /*12*/   Office2007ColorTable.TooltipDark(VisualStyle.Office2007Blue),
      /*13*/   Office2007ColorTable.SepDarkColor(VisualStyle.Office2007Blue, OfficeStyle.SoftEnhanced),
      /*14*/   Office2007ColorTable.LightBackground(VisualStyle.Office2007Blue),
      /*15*/   Office2007ColorTable.SepLightColor(VisualStyle.Office2007Blue,OfficeStyle.DarkWhite),
      /*16*/   Office2007ColorTable.TitleActiveDark(VisualStyle.Office2007Blue),
      /*17*/   Office2007ColorTable.SepDarkColor(VisualStyle.Office2007Blue, OfficeStyle.Dark),
      /*18*/   Office2007ColorTable.DarkBackground(VisualStyle.Office2007Blue),
      /*19*/   Office2007ColorTable.SepDarkColor(VisualStyle.Office2007Blue, OfficeStyle.DarkEnhanced),
      /*20*/   Office2007ColorTable.StatusBarGripDark(VisualStyle.Office2007Blue),
      /*21*/   Office2007ColorTable.BorderColor(VisualStyle.Office2007Blue),
      /*22*/   Office2007ColorTable.IndicatorsActive(VisualStyle.Office2007Blue),
      /*23*/   Office2007ColorTable.TabButtonColor(VisualStyle.Office2007Blue, OfficeStyle.SoftEnhanced),
      /*24*/   Office2007ColorTable.StatusBarText(VisualStyle.Office2007Blue),
      /*25*/   Office2007ColorTable.TitleActiveLight(VisualStyle.Office2007Silver),
      /*26*/   Office2007ColorTable.SepLightColor(VisualStyle.Office2007Black, OfficeStyle.SoftEnhanced),
      /*27*/   Office2007ColorTable.TabInactiveTextColor(VisualStyle.Office2007Black, OfficeStyle.DarkEnhanced),
      /*28*/   Office2007ColorTable.SoftBackground(VisualStyle.Office2007Silver),
      /*29*/   Office2007ColorTable.TitleInactiveDark(VisualStyle.Office2007Silver),
      /*30*/   Office2007ColorTable.TitleActiveLight(VisualStyle.Office2007Black),
      /*31*/   Office2007ColorTable.TooltipDark(VisualStyle.Office2007Silver),
      /*32*/   Office2007ColorTable.TabInactiveTextColor(VisualStyle.Office2007Black, OfficeStyle.Dark),
      /*33*/   Office2007ColorTable.TitleInactiveLight(VisualStyle.Office2007Black),
      /*34*/   Office2007ColorTable.TitleActiveDark(VisualStyle.Office2007Silver),
      /*35*/   Office2007ColorTable.TitleInactiveLight(VisualStyle.Office2007Silver),
      /*36*/   Office2007ColorTable.SepDarkColor(VisualStyle.Office2007Silver, OfficeStyle.SoftEnhanced),
      /*37*/   Office2007ColorTable.SoftBackground(VisualStyle.Office2007Black),
      /*38*/   Office2007ColorTable.LightBackground(VisualStyle.Office2007Silver),
      /*39*/   Office2007ColorTable.SepLightColor(VisualStyle.Office2007Silver, OfficeStyle.DarkEnhanced),
      /*40*/   Office2007ColorTable.TitleActiveDark(VisualStyle.Office2007Black),
      /*41*/   Office2007ColorTable.SepDarkColor(VisualStyle.Office2007Black, OfficeStyle.SoftEnhanced),
      /*42*/   Office2007ColorTable.SepLightColor(VisualStyle.Office2007Black, OfficeStyle.DarkEnhanced),
      /*43*/   Office2007ColorTable.SepDarkColor(VisualStyle.Office2007Silver, OfficeStyle.Dark),
      /*44*/   Office2007ColorTable.DarkBackground(VisualStyle.Office2007Silver),
      /*45*/   Office2007ColorTable.TabButtonColor(VisualStyle.Office2007Black, OfficeStyle.Dark),
      /*46*/   Office2007ColorTable.SepDarkColor(VisualStyle.Office2007Silver, OfficeStyle.DarkEnhanced),
      /*47*/   Office2007ColorTable.DarkBackground(VisualStyle.Office2007Black),
      /*48*/   Office2007ColorTable.TitleBorderColorDark(VisualStyle.Office2007Silver),
      /*49*/   Office2007ColorTable.BorderColor(VisualStyle.Office2007Silver),
      /*50*/   Office2007ColorTable.IndicatorsActive(VisualStyle.Office2007Silver),
      /*51*/   Office2007ColorTable.SepDarkColor(VisualStyle.Office2007Black, OfficeStyle.DarkEnhanced),
      /*52*/   Office2007ColorTable.TabButtonColor(VisualStyle.Office2007Silver, OfficeStyle.SoftEnhanced),
      /*53*/   Office2007ColorTable.StatusBarGripDark(VisualStyle.Office2007Silver),
      /*54*/   Office2007ColorTable.IndicatorsActive(VisualStyle.Office2007Black),
      /*55*/   Office2007ColorTable.LightBackground(VisualStyle.Office2007Black),
      /*56*/   Office2007ColorTable.TabButtonColor(VisualStyle.Office2007Black, OfficeStyle.DarkEnhanced),
      /*57*/   Office2007ColorTable.TooltipTextColor(VisualStyle.Office2007Blue),
      /*58*/   Office2007ColorTable.StatusBarGripDark(VisualStyle.Office2007Black),
      /*59*/   Office2007ColorTable.BorderColor(VisualStyle.Office2007Black),
      /*60*/   Office2007ColorTable.SepDarkColor(VisualStyle.Office2007Black, OfficeStyle.Dark),
      /*61*/   Office2007ColorTable.StatusBarText(VisualStyle.Office2007Silver),
      /*62*/   Office2007ColorTable.TabButtonColor(VisualStyle.Office2007Blue, OfficeStyle.DarkWhite),
      /*63*/   Office2007ColorTable.TitleActiveTextColor(VisualStyle.Office2007Black)
                              };


      //aMediaPlayerColorTable = new Color[] { 
      ///* 1*/   MediaPlayerColorTable.SepLightColor(VisualStyle.MediaPlayerBlue, OfficeStyle.Dark),
      ///* 2*/   MediaPlayerColorTable.CheckedActiveDark(VisualStyle.Office2007Blue),
      ///* 3*/   MediaPlayerColorTable.SelectedActiveDark(VisualStyle.Office2007Blue),
      ///* 4*/   MediaPlayerColorTable.SelectedActiveLight(VisualStyle.Office2007Blue),
      ///* 5*/   MediaPlayerColorTable.CheckedActiveLight(VisualStyle.Office2007Blue),
      ///* 6*/   MediaPlayerColorTable.EnhancedBackground(VisualStyle.Office2007Blue),
      ///* 7*/   MediaPlayerColorTable.TabHighlightTextColor(VisualStyle.Office2007Blue, OfficeStyle.Dark),
      ///* 8*/   MediaPlayerColorTable.SepLightColor(VisualStyle.Office2007Blue, OfficeStyle.Dark),
      ///* 9*/   MediaPlayerColorTable.SoftBackground(VisualStyle.Office2007Blue),
      ///*10*/   MediaPlayerColorTable.TitleInactiveDark(VisualStyle.Office2007Blue),
      ///*11*/   MediaPlayerColorTable.TitleInactiveLight(VisualStyle.Office2007Blue),
      ///*12*/   MediaPlayerColorTable.TooltipDark(VisualStyle.Office2007Blue),
      ///*13*/   MediaPlayerColorTable.SepDarkColor(VisualStyle.Office2007Blue, OfficeStyle.SoftEnhanced),
      ///*14*/   MediaPlayerColorTable.LightBackground(VisualStyle.Office2007Blue),
      ///*15*/   MediaPlayerColorTable.SepLightColor(VisualStyle.Office2007Blue,OfficeStyle.DarkWhite),
      ///*16*/   MediaPlayerColorTable.TitleActiveDark(VisualStyle.Office2007Blue),
      ///*17*/   MediaPlayerColorTable.SepDarkColor(VisualStyle.Office2007Blue, OfficeStyle.Dark),
      ///*18*/   MediaPlayerColorTable.DarkBackground(VisualStyle.Office2007Blue),
      ///*19*/   MediaPlayerColorTable.SepDarkColor(VisualStyle.Office2007Blue, OfficeStyle.DarkEnhanced),
      ///*20*/   MediaPlayerColorTable.StatusBarGripDark(VisualStyle.Office2007Blue),
      ///*21*/   MediaPlayerColorTable.BorderColor(VisualStyle.Office2007Blue),
      ///*22*/   MediaPlayerColorTable.IndicatorsActive(VisualStyle.Office2007Blue),
      ///*23*/   MediaPlayerColorTable.TabButtonColor(VisualStyle.Office2007Blue, OfficeStyle.SoftEnhanced),
      ///*24*/   MediaPlayerColorTable.StatusBarText(VisualStyle.Office2007Blue),
      ///*25*/   MediaPlayerColorTable.TitleActiveLight(VisualStyle.Office2007Silver),
      ///*26*/   MediaPlayerColorTable.SepLightColor(VisualStyle.Office2007Black, OfficeStyle.SoftEnhanced),
      ///*27*/   MediaPlayerColorTable.TabInactiveTextColor(VisualStyle.Office2007Black, OfficeStyle.DarkEnhanced),
      ///*28*/   MediaPlayerColorTable.SoftBackground(VisualStyle.Office2007Silver),
      ///*29*/   MediaPlayerColorTable.TitleInactiveDark(VisualStyle.Office2007Silver),
      ///*30*/   MediaPlayerColorTable.TitleActiveLight(VisualStyle.Office2007Black),
      ///*31*/   MediaPlayerColorTable.TooltipDark(VisualStyle.Office2007Silver),
      ///*32*/   MediaPlayerColorTable.TabInactiveTextColor(VisualStyle.Office2007Black, OfficeStyle.Dark),
      ///*33*/   MediaPlayerColorTable.TitleInactiveLight(VisualStyle.Office2007Black),
      ///*34*/   MediaPlayerColorTable.TitleActiveDark(VisualStyle.Office2007Silver),
      ///*35*/   MediaPlayerColorTable.TitleInactiveLight(VisualStyle.Office2007Silver),
      ///*36*/   MediaPlayerColorTable.SepDarkColor(VisualStyle.Office2007Silver, OfficeStyle.SoftEnhanced),
      ///*37*/   MediaPlayerColorTable.SoftBackground(VisualStyle.Office2007Black),
      ///*38*/   MediaPlayerColorTable.LightBackground(VisualStyle.Office2007Silver),
      ///*39*/   MediaPlayerColorTable.SepLightColor(VisualStyle.Office2007Silver, OfficeStyle.DarkEnhanced),
      ///*40*/   MediaPlayerColorTable.TitleActiveDark(VisualStyle.Office2007Black),
      ///*41*/   MediaPlayerColorTable.SepDarkColor(VisualStyle.Office2007Black, OfficeStyle.SoftEnhanced),
      ///*42*/   MediaPlayerColorTable.SepLightColor(VisualStyle.Office2007Black, OfficeStyle.DarkEnhanced),
      ///*43*/   MediaPlayerColorTable.SepDarkColor(VisualStyle.Office2007Silver, OfficeStyle.Dark),
      ///*44*/   MediaPlayerColorTable.DarkBackground(VisualStyle.Office2007Silver),
      ///*45*/   MediaPlayerColorTable.TabButtonColor(VisualStyle.Office2007Black, OfficeStyle.Dark),
      ///*46*/   MediaPlayerColorTable.SepDarkColor(VisualStyle.Office2007Silver, OfficeStyle.DarkEnhanced),
      ///*47*/   MediaPlayerColorTable.DarkBackground(VisualStyle.Office2007Black),
      ///*48*/   MediaPlayerColorTable.TitleBorderColorDark(VisualStyle.Office2007Silver),
      ///*49*/   MediaPlayerColorTable.BorderColor(VisualStyle.Office2007Silver),
      ///*50*/   MediaPlayerColorTable.IndicatorsActive(VisualStyle.Office2007Silver),
      ///*51*/   MediaPlayerColorTable.SepDarkColor(VisualStyle.Office2007Black, OfficeStyle.DarkEnhanced),
      ///*52*/   MediaPlayerColorTable.TabButtonColor(VisualStyle.Office2007Silver, OfficeStyle.SoftEnhanced),
      ///*53*/   MediaPlayerColorTable.StatusBarGripDark(VisualStyle.Office2007Silver),
      ///*54*/   MediaPlayerColorTable.IndicatorsActive(VisualStyle.Office2007Black),
      ///*55*/   MediaPlayerColorTable.LightBackground(VisualStyle.Office2007Black),
      ///*56*/   MediaPlayerColorTable.TabButtonColor(VisualStyle.Office2007Black, OfficeStyle.DarkEnhanced),
      ///*57*/   MediaPlayerColorTable.TooltipTextColor(VisualStyle.Office2007Blue),
      ///*58*/   MediaPlayerColorTable.StatusBarGripDark(VisualStyle.Office2007Black),
      ///*59*/   MediaPlayerColorTable.BorderColor(VisualStyle.Office2007Black),
      ///*60*/   MediaPlayerColorTable.SepDarkColor(VisualStyle.Office2007Black, OfficeStyle.Dark),
      ///*61*/   MediaPlayerColorTable.StatusBarText(VisualStyle.Office2007Silver),
      ///*62*/   MediaPlayerColorTable.TabButtonColor(VisualStyle.Office2007Blue, OfficeStyle.DarkWhite),
      ///*63*/   MediaPlayerColorTable.TitleActiveTextColor(VisualStyle.Office2007Black)
      //                        };
      #endregion Colors[] ....

      #region TextOfColors

      aTextColorsProf = new String[]{
     /* 1*/   "GripLight",
     /* 2*/   "ButtonSelectedGradientBegin", 
     /* 3*/   "ButtonSelectedGradientMiddle", 
     /* 4*/   "ButtonCheckedGradientBegin", 
     /* 5*/   "ButtonSelectedGradientEnd",
     /* 6*/   "ButtonCheckedGradientMiddle",
     /* 7*/   "CheckBackground",
     /* 8*/   "ButtonPressedGradientMiddle", 
     /* 9*/   "ButtonCheckedGradientEnd",
     /*10*/   "ButtonPressedGradientBegin",
     /*11*/   "ToolStripDropDownBackground",
     /*12*/   "SeparatorLight",
     /*13*/   "ImageMarginGradientBegin",
     /*14*/   "ImageMarginGradientMiddle",
     /*15*/   "ImageMarginRevealedGradientBegin", 
     /*16*/   "MenuStripGradientEnd", 
     /*17*/   "ButtonCheckedHighlight",
     /*18*/   "ImageMarginRevealedGradientMiddle",
     /*19*/   "MenuStripGradientBegin",
     /*20*/   "ButtonPressedHighlight",
     /*21*/   "OverflowButtonGradientBegin",
     /*22*/   "ImageMarginGradientEnd", 
     /*23*/   "ImageMarginRevealedGradientEnd",
     /*24*/   "SeparatorDark", 
     /*25*/   "OverflowButtonGradientMiddle",
     /*26*/   "ToolStripBorder", 
     /*27*/   "ButtonCheckedHighlightBorder",  
     /*28*/   "GripDark", 
     /*29*/   "OverflowButtonGradientEnd", 
     /*30*/   "MenuBorder",
     /*31*/   "ButtonPressedBorder"
                                    };

      aTextColorsOffice = new String[] { 
      /* 1*/  "SepLightColor(Office2007Silver)",
      /* 2*/  "CheckedActiveDark(Office2007Blue)",
      /* 3*/   "SelectedActiveDark(Office2007Blue)",
      /* 4*/   "SelectedActiveLight(Office2007Blue)",
      /* 5*/   "CheckedActiveLight(Office2007Blue)",
      /* 6*/   "EnhancedBackground(Office2007Blue)",
      /* 7*/   "TabHighlightTextColor(Office2007Blue)",
      /* 8*/   "SepLightColor(Office2007Blue)",
      /* 9*/   "SoftBackground(Office2007Blue)",
      /*10*/   "TitleInactiveDark(Office2007Blue)",
      /*11*/   "TitleInactiveLight(Office2007Blue)",
      /*12*/   "TooltipDark(Office2007Blue)",
      /*13*/   "SepDarkColor(Office2007Blue)",
      /*14*/   "LightBackground(Office2007Blue)",
      /*15*/   "SepLightColor(Office2007Blue)",
      /*16*/   "TitleActiveDark(Office2007Blue)",
      /*17*/   "SepDarkColor(Office2007Blue)",
      /*18*/   "DarkBackground(Office2007Blue)",
      /*19*/   "SepDarkColor(Office2007Blue)",
      /*20*/   "StatusBarGripDark(Office2007Blue)",
      /*21*/   "BorderColor(Office2007Blue)",
      /*22*/   "IndicatorsActive(Office2007Blue)",
      /*23*/   "TabButtonColor(Office2007Blue)",
      /*24*/   "StatusBarText(Office2007Blue)",
      /*25*/   "TitleActiveLight(Office2007Silver)",
      /*26*/   "SepLightColor(Office2007Black)",
      /*27*/   "TabInactiveTextColor(Office2007Black)",
      /*28*/   "SoftBackground(Office2007Silver)",
      /*29*/   "TitleInactiveDark(Office2007Silver)",
      /*30*/   "TitleActiveLight(Office2007Black)",
      /*31*/   "TooltipDark(Office2007Silver)",
      /*32*/   "TabInactiveTextColor(Office2007Black)",
      /*33*/   "TitleInactiveLight(Office2007Black)",
      /*34*/   "TitleActiveDark(Office2007Silver)",
      /*35*/   "TitleInactiveLight(Office2007Silver)",
      /*36*/   "SepDarkColor(Office2007Silver)",
      /*37*/   "SoftBackground(Office2007Black)",
      /*38*/   "LightBackground(Office2007Silver)",
      /*39*/   "SepLightColor(Office2007Silver)",
      /*40*/   "TitleActiveDark(Office2007Black)",
      /*41*/   "SepDarkColor(Office2007Black)",
      /*42*/   "SepLightColor(Office2007Black)",
      /*43*/   "SepDarkColor(Office2007Silver)",
      /*44*/   "DarkBackground(Office2007Silver)",
      /*45*/   "TabButtonColor(Office2007Black)",
      /*46*/   "SepDarkColor(Office2007Silver)",
      /*47*/   "DarkBackground(Office2007Black)",
      /*48*/   "TitleBorderColorDark(Office2007Silver)",
      /*49*/   "BorderColor(Office2007Silver)",
      /*50*/   "IndicatorsActive(Office2007Silver)",
      /*51*/   "SepDarkColor(Office2007Black)",
      /*52*/   "TabButtonColor(Office2007Silver)",
      /*53*/   "StatusBarGripDark(Office2007Silver)",
      /*54*/   "IndicatorsActive(Office2007Black)",
      /*55*/   "LightBackground(Office2007Black)",
      /*56*/   "TabButtonColor(Office2007Black)",
      /*57*/   "TooltipTextColor(Office2007Blue)",
      /*58*/   "StatusBarGripDark(Office2007Black)",
      /*59*/   "BorderColor(Office2007Black)",
      /*60*/   "SepDarkColor(Office2007Black)",
      /*61*/   "StatusBarText(Office2007Silver)",
      /*62*/   "TabButtonColor(Office2007Blue)",
      /*63*/   "TitleActiveTextColor(Office2007Black)"
                              };

      #endregion TextOfColors

      InitializeTabControlBojice();
      ColorAndNameButtons(aSystemColors, tabPageSystem, null);
      ColorAndNameButtons(aProfessionalColors, tabPageProf, aTextColorsProf);
      ColorAndNameButtons(aColor, tabPageWeb, null);
      ColorAndNameButtons(aOffice2007ColorTable, tabPageOffice, aTextColorsOffice);
   }

   #region InitializeTabControlBojice

   private void InitializeTabControlBojice()
   {
      tabControlBojice                  = new Crownwood.DotNetMagic.Controls.TabControl();
      tabControlBojice.Parent           = this;
      tabControlBojice.Location         = new Point(0, 0);
      tabControlBojice.Size             = new Size(this.Width - 10, this.Height - 30);
      tabControlBojice.Anchor           = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      tabControlBojice.Appearance       = VisualAppearance.MultiDocument;
      tabControlBojice.PositionTop      = true;
      tabControlBojice.ShowClose        = false;
      tabControlBojice.Style            = ZXC.vvColors.vvform_VisualStyle;
      tabControlBojice.OfficeStyle      = ZXC.vvColors.tabControl_OfficeStyle;
      tabControlBojice.MediaPlayerStyle = ZXC.vvColors.tabControl_MediaPlayerStyle;

      tabPageSystem = new Crownwood.DotNetMagic.Controls.TabPage("System");
      tabControlBojice.TabPages.Add(tabPageSystem);
      tabPageProf = new Crownwood.DotNetMagic.Controls.TabPage("Professional");
      tabControlBojice.TabPages.Add(tabPageProf);
      tabPageWeb = new Crownwood.DotNetMagic.Controls.TabPage("Web");
      tabControlBojice.TabPages.Add(tabPageWeb);
      tabPageOffice = new Crownwood.DotNetMagic.Controls.TabPage("Office2007");
      tabControlBojice.TabPages.Add(tabPageOffice);

      tabPageSystem.AutoScroll = tabPageProf.AutoScroll = tabPageWeb.AutoScroll = tabPageOffice.AutoScroll = true;
      tabPageSystem.BackColor = tabPageProf.BackColor = tabPageWeb.BackColor = tabPageOffice.BackColor = Color.White;
   }

   #endregion InitializeTabControlBojice

   #region ColorAndNameButtons

   private void ColorAndNameButtons(Color[] _aColors, Crownwood.DotNetMagic.Controls.TabPage _tabPage, String[] _aText)
   {
      btnName = new ButtonWithStyle[_aColors.Length];
      btnColor = new Button[_aColors.Length];

      for (int i = 0; i < _aColors.Length; i++)
      {
         btnName[i]            = new ButtonWithStyle();
         btnName[i].Parent     = _tabPage;
         btnName[i].Size       = new Size(_tabPage.Width - 50, 20);
         btnName[i].BackColor  = Color.White;
         btnName[i].Location   = new Point(0, i * btnName[i].Height + 5);
         btnName[i].Tag        = _aColors[i];
         btnName[i].TextAlignH = StringAlignment.Far;
         btnName[i].Click += new EventHandler(btnColor_Click);
         
         if (_aText == null)
         {
            btnName[i].Text = _aColors[i].Name; //(i + 1).ToString() + "  " + aSystemColors[i].ToArgb().ToString();
         }
         else
         {
            btnName[i].Text = _aText[i]; //(i + 1).ToString() + "   " + aProfessionalColors[i].ToArgb().ToString();
         }

         btnColor[i]           = new Button();
         btnColor[i].Parent    = btnName[i];
         btnColor[i].Location  = new Point(3, 3);
         btnColor[i].Size      = new Size(20, 14);
         btnColor[i].FlatStyle = FlatStyle.Flat;
         btnColor[i].BackColor = _aColors[i];
         btnColor[i].Tag       = _aColors[i];
         btnColor[i].Click += new EventHandler(btnColor_Click);

         if (btnColor[i].BackColor.ToArgb() == selectedColor.ToArgb())
         {
            btnName[i].BackColor = ProfessionalColors.ButtonSelectedGradientMiddle;
            btnName[i].Select();
            _tabPage.Selected = true;
         }
      }
   }

   #endregion ColorAndNameButtons

   #region btnColor_Click
   void btnColor_Click(object sender, EventArgs e)
   {
      if (sender is Button)
      {
         Button btn = sender as Button;
         selectedColor = btn.BackColor;
      }
      else
      {
         ButtonWithStyle btn = sender as ButtonWithStyle;
         Color bc = (Color)btn.Tag;
         selectedColor = bc;
      }
      this.Close();
   }
   #endregion btnColor_Click

}