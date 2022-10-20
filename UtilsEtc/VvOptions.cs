using System;
using System.Drawing;
using System.Windows.Forms;


public class VvOptions 
{

   public VvOptions()
   {
     // this.BaseFont  = new Font("Tahoma", (8.25f));
   }

   public static int CalcQUN(Control c)
   {
      Graphics g = c.CreateGraphics();
      float newQUN;
      float referenceWidth = g.MeasureString("W M", c.Font).Width;
      g.Dispose();
      newQUN = referenceWidth / 3.15F * 2.05F;
      return((int)newQUN);
   }

   private Font smallFont, baseFont, baseBoldFont, largeFont, smallsmallFont, largelargeFont;


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
         largeFont      = new Font(baseFont.FontFamily, baseFont.SizeInPoints  + ((baseFont.SizeInPoints  * 15)/100));
         smallsmallFont = new Font(baseFont.FontFamily, smallFont.SizeInPoints - ((smallFont.SizeInPoints * 15)/100));
         largelargeFont = new Font(baseFont.FontFamily, largeFont.SizeInPoints + ((largeFont.SizeInPoints * 15)/100));

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
   /// Ovi font koristi ?  (read only) (ZXC.opt.BaseFont + 1)
   /// </summary>
   public  Font LargeFont 
   { 
      get { return(largeFont); } 
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

}
