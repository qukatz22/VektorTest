using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

public class VvDateTimePicker : System.Windows.Forms.DateTimePicker
{

   #region DaInternet

   const int WM_ERASEBKGND = 0x14;
   SolidBrush brush;

   [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
   public override Color BackColor
   {
      get { return base.BackColor; }
      set
      {
         base.BackColor = value;
         brush = new SolidBrush(value);
      }
   }

   protected override void WndProc(ref Message m)
   {
      if (m.Msg == WM_ERASEBKGND)
      {
         using (Graphics g = Graphics.FromHdc(m.WParam))
         {
            g.FillRectangle(brush, this.ClientRectangle);
         }
         return;
      }
      base.WndProc(ref m);
   }

   protected override void Dispose(bool disposing)
   {
      if (disposing && (null != brush))
      {
         brush.Dispose();
         brush = null;
      }
      base.Dispose(disposing);
   }

   #endregion DaInternet

   #region Event DateTimePicker - TheVvTextBox

   public VvTextBox TheVvTextBox { get; set; }
   
   public void dtPicker_ValueChanged_SetDateFormat_SendValueToTextBox(object sender, EventArgs e)
   {
      VvDateTimePicker dtp = sender as VvDateTimePicker;

      if(dtp.Value == DateTimePicker.MinimumDateTime)
      {
         dtp.CustomFormat = ZXC.VvEmptyDtpFormat;
      }
      else
      {
         if(TheVvTextBox.JAM_IsForDateTimePicker_WithTimeDisplay == true)
            dtp.CustomFormat = ZXC.VvDateAndTimeFormat;
         else if(TheVvTextBox.JAM_IsForDateTimePicker_TimeOnlyDisplay == true)
         {
            dtp.CustomFormat = ZXC.VvTimeOnlyFormat;
            dtp.ShowUpDown = true; // 28.11.2017: 
         }
         else if(TheVvTextBox.JAM_IsForDateTimePicker_YearOnly == true)
            dtp.CustomFormat = ZXC.VvDateYyyyFormat;
         else
            dtp.CustomFormat = ZXC.VvDateFormat;
      }

      TheVvTextBox.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp);

      if(TheVvTextBox.JAM_twinTaker_JAM_Name.NotEmpty())
      {
         VvTextBox twinTextBox = TheVvTextBox.GetTwinVvTextBoxTaker(ZXC.TheVvForm.TheVvUC, TheVvTextBox.JAM_twinTaker_JAM_Name);

         if(twinTextBox != null) twinTextBox.Text = TheVvTextBox.Text;

         // chain reaction ako ima i treci (za sada specijalan slucaj samo na OptusnomPismu - datum odjave), sljaka samo za Dtp - VvTextBox jer se kod obicnog vvtb-a nece dignuti OnExit event! 
         if(twinTextBox.JAM_twinTaker_JAM_Name.NotEmpty())
         {
            VvTextBox twinTextBox2 = twinTextBox.GetTwinVvTextBoxTaker(ZXC.TheVvForm.TheVvUC, twinTextBox.JAM_twinTaker_JAM_Name);

            if(twinTextBox2 != null) twinTextBox2.Text = twinTextBox.Text;
         }
      }
   }

   public void dtp_KeyDown_DeleteKeyEmptiesDTP(object sender, KeyEventArgs e)
   {
      VvDateTimePicker dtp = sender as VvDateTimePicker;

      if(e.KeyCode == Keys.Delete)
      {
         dtp.Value = VvDateTimePicker.MinimumDateTime;
      }
   }

   public void dtp_KeyPress_StartTypingIfEmpty(object sender, KeyPressEventArgs e)
   {
      VvDateTimePicker dtp = sender as VvDateTimePicker;

      if(dtp.Text  == " " && 
         dtp.Value == VvDateTimePicker.MinimumDateTime &&
         char.IsDigit(e.KeyChar))
      {
         //if(TheVvTextBox.JAM_IsForDateTimePicker_WithTimeDisplay == true)
         //   vvcb.Text = DateTime.Now.ToString(ZXC.VvDateAndTimeFormat);
         //else if(TheVvTextBox.JAM_IsForDateTimePicker_YearOnly == true)
         //   vvcb.Value = DateTime.Now;
         //   //vvcb.Text = "2009";
         //   //vvcb.Text = DateTime.Now.ToString(ZXC.VvDateYyyyFormat);
         //else
         //   vvcb.Text = DateTime.Now.ToString(ZXC.VvDateFormat);

         dtp.Value = DateTime.Now;

         SendKeys.Send("{RIGHT}");
         SendKeys.Send(e.KeyChar.ToString());
      }
   }

   public void dtp_Validating_SetDateRequired(object sender, CancelEventArgs e)
   {
      VvDateTimePicker dtp = sender as VvDateTimePicker;

      if(dtp.Value == DateTimePicker.MinimumDateTime)
      {
         ZXC.RaiseErrorProvider(dtp, "Polje mora sadrzavati neki podatak!");
         e.Cancel = true;
      }
   }

   public void dtPicker_DropDown_SetDateToday(object sender, EventArgs e)
   {
      VvDateTimePicker dtp = sender as VvDateTimePicker;

      if(dtp.Value == VvDateTimePicker.MinimumDateTime)
      {
         dtp.Value = DateTime.Now;
         SendKeys.Send("{ESC}");
         SendKeys.Send("%{DOWN}");
      }
   }

   public void dtp_LostFocus_ShowTextBox(object sender, EventArgs e)
   {
      VvDateTimePicker dtp = sender as VvDateTimePicker;

      if(TheVvTextBox.JAM_DataRequired == true && dtp.Value == DateTimePicker.MinimumDateTime) return; // ... a dtp_Validating_SetDateRequired() ce obaviti ostalo 
      
      dtp.SendToBack();
      dtp.Visible          = false;
      TheVvTextBox.Visible = true;
   }

   public void theVvTextBox_GotFocus_ShowDTP(object sender, EventArgs e)
   {
      VvTextBox tbx = sender as VvTextBox;
      VvDateTimePicker dtp = (VvDateTimePicker)tbx.Tag;

      if(tbx.ReadOnly) return;

      dtp.Visible = true;
      dtp.Focus();
      TheVvTextBox.Visible = false;

   }

   #endregion Event DateTimePicker - TheVvTextBox

   // 02.05.2016: 
   #region 02.05.2016: Try Validate like VvTextBox

   private bool editInProgress = false;
   public /*private*/ void EH_JAM_Validating(object sender, System.ComponentModel.CancelEventArgs e)
   {
      //...JAM_MustTabOutBeforeSubmit.............................................................
      if(this.JAM_MustTabOutBeforeSubmit == true)
      {

         if(editInProgress)
         {
            VvDateTimePicker dtp = sender as VvDateTimePicker;

            ZXC.RaiseErrorProvider(dtp.TheVvTextBox, "DokDate TAB OUT NEEDED!\n\nMolim, potvrdite DATUM DOKUMENTA izlaskom iz polja tipkom <Tab> ili <Enter> ili mišem.");
            e.Cancel = true;
         }

      } //----------------------------------------------------------------------------------------

      return;
   }

   public bool JAM_MustTabOutBeforeSubmit { get; set; }

   public void OnEnter_BeginEdit(object sender, EventArgs e)
   {
      editInProgress = true;
   }

   public void OnLeave_EndEdit(object sender, EventArgs e)
   {
      editInProgress = false;
   }

 //public bool JAM_ShouldValidateOnExit
 //{
 //   get
 //   {
 //      //if(JAM_IsInDataGridView == true) return false;
 //
 //      // ovdje u OR-u navedi sve razloge kada je SHOULD true 
 //      if(JAM_MustTabOutBeforeSubmit == true /*||
 //         JAM_FieldExitWithValidationMethod != null*/)
 //      {
 //         return true;
 //      }
 //      else
 //      {
 //         return false;
 //      }
 //   }
 //}

   #endregion Try Validate like VvTextBox

}

public class VvCheckBox : System.Windows.Forms.CheckBox
{
   public VvTextBox TheVvTextBox { get; set; }

   public string TheFalseText { get; set; }
   public string TheTrueText  { get; set; }

   // TODO: vidi mozes li umjesto 'X' dobiti checkMark  
   public const string TheDefaultFalseText = "";
   public const string TheDefaultTrueText  = "X";

   // TODO: nebude ti ovo radilo dok ne napravis VvCheckBox.TakeJAM_Members...                              
   // jerbo ti je CheckBox kao EditingControl jedan jedini, pa bi tu morao kao i za VvTextBox               
   // ali buduci da ti na PlacaDUC-u svi chkBoxovi rade slicno (dizu CalcTransResults), mozes ovo i kasnije 
   // do tada notaBene da nemozes nekima reci da jesu a nekima da nisu 'shouldCalcTrans'                    
   private bool shouldCalcTrans;
   /// <summary>
   /// If tru, should raise CalcTransResults
   /// </summary>
   public bool JAM_ShouldCalcTrans
   {
      get { return shouldCalcTrans; }
      set { shouldCalcTrans = value; }
   }

   public void vvCheckBox_CheckChanged_SendValueToTextBox(object sender, EventArgs e)
   {
      VvCheckBox vvcb = sender as VvCheckBox;

      TheVvTextBox.Text = ZXC.ValOrEmpty_CheckBoxChecked_AsText(vvcb);

      if(TheVvTextBox.JAM_twinTaker_JAM_Name.NotEmpty())
      {
         VvTextBox twinTextBox = TheVvTextBox.GetTwinVvTextBoxTaker(ZXC.TheVvForm.TheVvUC, TheVvTextBox.JAM_twinTaker_JAM_Name);

         if(twinTextBox != null) twinTextBox.Text = TheVvTextBox.Text;

         // chain reaction ako ima i treci (za sada specijalan slucaj samo na OptusnomPismu - datum odjave), sljaka samo za Dtp - VvTextBox jer se kod obicnog vvtb-a nece dignuti OnExit event! 
         if(twinTextBox.JAM_twinTaker_JAM_Name.NotEmpty())
         {
            VvTextBox twinTextBox2 = twinTextBox.GetTwinVvTextBoxTaker(ZXC.TheVvForm.TheVvUC, twinTextBox.JAM_twinTaker_JAM_Name);

            if(twinTextBox2 != null) twinTextBox2.Text = twinTextBox.Text;
         }
      }
   }

   public void checkBox_LostFocus_ShowTextBox(object sender, EventArgs e)
   {
      VvCheckBox vvcb = sender as VvCheckBox;

      SuspendLayout();

      vvcb.SendToBack();
      vvcb.Visible         = false;
      TheVvTextBox.Visible = true;

      ResumeLayout();
   }

   public void theVvTextBox_GotFocus_ShowCheckBox(object sender, EventArgs e)
   {
      VvTextBox  tbx  = sender as VvTextBox;
      VvCheckBox vvcb = (VvCheckBox)tbx.Tag;

      if(tbx.ReadOnly) return;

      SuspendLayout();

      vvcb.Visible = true;
      vvcb.Focus();
      TheVvTextBox.Visible = false;

      ResumeLayout();
   }

   public override string ToString()
   {
           if(this.Checked == false) return this.TheFalseText;
      else if(this.Checked == true ) return this.TheTrueText;
      else                           return base.ToString();
   }

   public VvCheckBox()
   {
      this.TheFalseText = TheDefaultFalseText;
      this.TheTrueText  = TheDefaultTrueText;
   }

   public VvCheckBox(string falseText, string trueText)
   {
      this.TheFalseText = falseText;
      this.TheTrueText  = trueText;
   }

   public static bool GetBool4String(string theString)
   {
           if(theString == VvCheckBox.TheDefaultFalseText ||
              theString == bool      .FalseString          ) return false;
      else if(theString == VvCheckBox.TheDefaultTrueText  ||
              theString == bool      .TrueString           ) return true;

      else throw new Exception("GetBool4String: theString[" + theString + "] is najder falseText[" + VvCheckBox.TheDefaultFalseText + "] najder trueText[" + VvCheckBox.TheDefaultTrueText + "]!");
   }

   public static bool GetBool4String(string theString, string falseText, string trueText)
   {
           if(theString == falseText) return false;
      else if(theString == trueText)  return true;

      else throw new Exception("GetBool4String: theString[" + theString + "] is najder falseText[" + falseText + "] najder trueText[" + trueText + "]!");
   }

   public static string GetString4Bool(bool theBool)
   {
      if(theBool == true) return TheDefaultTrueText;
      else                return TheDefaultFalseText;
   }

   public static string GetString4Bool(bool theBool, string falseText, string trueText)
   {
      if(theBool == true) return trueText;
      else                return falseText;
   }

}

public class VvCheckBoxBoolFormat : IFormatProvider, ICustomFormatter
{
   public object GetFormat(Type formatType)
   {
      if(formatType == typeof(ICustomFormatter))
         return this;
      else
         return null;
   }

   public string Format(string fmt, object arg, IFormatProvider formatProvider)
   {
      return "Y";
      //// Convert argument to a string
      //string result = arg.ToString();

      //// Add hyphens for formatting code "H"
      //if(!String.IsNullOrEmpty(fmt) && fmt.ToUpper() == "H")
      //   return result.Substring(0, 5) + "-" + result.Substring(5, 3) + "-" + result.Substring(8);
      //// Return string representation of argument for any other formatting code
      //else
      //   return result;
   }
}


