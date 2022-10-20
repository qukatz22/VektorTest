using System;
using System.Drawing;
using System.Windows.Forms;


public class VvFont 
{

   public VvFont()
   {
     // this.BaseFont  = new Font("Tahoma", (8.25f));
   }

   public static int CalcQUN(Control c)
   {
      Graphics g = c.CreateGraphics();
      float newQUN;
      float referenceWidth = g.MeasureString("W M", c.Font).Width;
      g.Dispose();
      //newQUN = referenceWidth / 3.15F * 2.05F;
      newQUN = referenceWidth / 3.15F * 2.15F;
      
      return((int)newQUN);
   }

   private Font smallFont, baseFont, baseBoldFont, largeFont, largeBoldFont, smallBoldFont, smallsmallFont, largelargeFont, toolStripBtnFont,
                rtbFont;


   /// <summary>
   /// Ovi font koriste TextBox-ovi (i neke labele, ...)
   /// </summary>
   public  Font BaseFont 
   { 
      get { return(baseFont); } 
      set
      {
         baseFont       = value;
         baseBoldFont   = new Font(baseFont, baseFont.Style | FontStyle.Bold);
         smallFont      = new Font(baseFont.FontFamily, baseFont.SizeInPoints  - ((baseFont.SizeInPoints  * 15)/100));
         smallBoldFont  = new Font(smallFont, baseFont.Style | FontStyle.Bold);
         largeFont      = new Font(baseFont.FontFamily, baseFont.SizeInPoints + ((baseFont.SizeInPoints * 15) / 100));
         largeBoldFont  = new Font(largeFont, baseFont.Style | FontStyle.Bold);
         smallsmallFont = new Font(baseFont.FontFamily, smallFont.SizeInPoints - ((smallFont.SizeInPoints * 15)/100));
         largelargeFont = new Font(baseFont.FontFamily, largeFont.SizeInPoints + ((largeFont.SizeInPoints * 15)/100));
         
         toolStripBtnFont = smallsmallFont;
         rtbFont          = new Font("Arial", 9f);
      }
   }
   /// <summary>
   /// BaseFont pa BOLD-an
   /// </summary>
   public Font BaseBoldFont
   {
      get { return baseBoldFont; }
      set { baseBoldFont = value; }
   }
   /// <summary>
   /// Ovi font koriste Label-e (read only) (ZXC.opt.BaseFont - 1)
   /// </summary>
   public  Font SmallFont 
   { 
      get { return(smallFont); } 
   }
   /// <summary>
   /// Ovi font koriste Label-e (read only) (ZXC.opt.BaseFont - 1)
   /// </summary>
   public Font SmallBoldFont
   {
      get { return (smallBoldFont); }
   }

   /// <summary>
   /// Ovi font koristi ?  (read only) (ZXC.opt.BaseFont + 1)
   /// </summary>
   public  Font LargeFont 
   { 
      get { return(largeFont); } 
   }
   /// <summary>
   /// Ovi font koristi ?  (read only) (ZXC.opt.BaseFont + 1)
   /// </summary>
   public Font LargeBoldFont
   {
      get { return (largeBoldFont); }
   }
   /// <summary>
   /// Ovi font koriste Label-e (read only) (ZXC.opt.BaseFont - 2)
   /// </summary>
   public  Font SmallSmallFont 
   { 
      get { return(smallsmallFont); } 
   }
   /// <summary>
   /// Ovi font koristi ?  (read only) (ZXC.opt.BaseFont + 2)
   /// </summary>
   public  Font LargeLargeFont 
   { 
      get { return(largelargeFont); } 
   }

    /// <summary>
   /// Ovi font koristi ToolStriButtoni ispod Icon-e  (read only) (ZXC.opt.BaseFont - 2)
   /// </summary>
   public Font ToolStripBtnFont 
   {
      get { return (toolStripBtnFont); } 
   }

   /// <summary>
   /// Ovi font koristi VvRichTextBox  
   /// </summary>
   public Font RTBFont
   {
      get { return (rtbFont); }
   }

   
}

class NiceEnumStr : Attribute
{
   public string Text;
   public NiceEnumStr(string text)
   {
      Text = text;
   }
}

