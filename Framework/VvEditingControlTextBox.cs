//-------------------------------------------------------------------------
// VvTextBoxEditingControl.cs, from the MSDN topic 
// VvTextBoxColumn.cs, from the MSDN topic 
// VvTextBoxCell.cs, from the MSDN topic 
//              "How to: ServerHost Controls in Windows Forms DataGridView Cells
//-------------------------------------------------------------------------
using System;
using System.Windows.Forms;

public class VvTextBoxEditingControl : VvTextBox, IDataGridViewEditingControl
{
   DataGridView dataGridView;
   private bool valueChanged = false;
   int rowIndex;

   public VvTextBoxEditingControl()
   {
      //Qukatz: this.Format = DateTimePickerFormat.Short;
   }

   // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
   // property.
   public object EditingControlFormattedValue
   {
      get
      {
         //Q: return this.Value.ToShortDateString();

         //if(this.JAM_FillCharacter != ' ')      this.EH_JAM_FormatFillCharacter(this, EventArgs.Empty);
         //if(this.JAM_IsNumericTextBox == true)  this.EH_JAM_FormatNiceNumber(   this, EventArgs.Empty);

         return this.Text;
      }
      set
      {
         //Q: String newValue = value as String;
         //Q: if(newValue != null)
         {
            //Q: this.Value = DateTime.Parse(newValue);
            this.Text = value.ToString();
         }
      }
   }

   // Implements the 
   // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
   public object GetEditingControlFormattedValue(
       DataGridViewDataErrorContexts context)
   {
      if(context != (DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.Display)) 
         return EditingControlFormattedValue;
      else         
         return this.Text;
   }

   // Implements the 
   // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
   public void ApplyCellStyleToEditingControl(
       DataGridViewCellStyle dataGridViewCellStyle)
   {
      //Q: this.Font = dataGridViewCellStyle.Font;
      //Q: this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
      //Q: this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
      this.Font = dataGridViewCellStyle.Font;
      //this.ForeColor = dataGridViewCellStyle.ForeColor;
      //this.BackColor = dataGridViewCellStyle.BackColor;
   }

   // Implements the IDataGridViewEditingControl.EditingControlRowIndex 
   // property.
   public int EditingControlRowIndex
   {
      get
      {
         return rowIndex;
      }
      set
      {
         rowIndex = value;
      }
   }

   // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey 
   // method.
   public bool EditingControlWantsInputKey(
       Keys key, bool dataGridViewWantsInputKey)
   {
      // Let the DateTimePicker handle the keys listed.
      switch(key & Keys.KeyCode)
      {
         case Keys.Enter:
         case Keys.Left:
         //case Keys.Up:
         //case Keys.Down:
         case Keys.Right:
         case Keys.Home:
         case Keys.End:
         //case Keys.PageDown:
         //case Keys.PageUp:
         case Keys.OemPeriod:
         case Keys.Decimal:
         // zbog nekog undocumented bug-a: start

         case Keys.Q:   // q 
         case Keys.D1:  // ! 
         case Keys.D3:  // # 
         case Keys.D4:  // $ 
         case Keys.D5:  // % 
         case Keys.D7:  // & 
         case Keys.D9:  // ( 
         case Keys.OemQuotes:   // <'> + <"> 

         // zbog nekog undocumented bug-a:   end
            return true;
         default:
            return false;
      }
   }

   // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit 
   // method.
   public void PrepareEditingControlForEdit(bool selectAll)
   {
      // No preparation needs to be done.
   }

   // Implements the IDataGridViewEditingControl
   // .RepositionEditingControlOnValueChange property.
   public bool RepositionEditingControlOnValueChange
   {
      get
      {
         return false;
      }
   }

   // Implements the IDataGridViewEditingControl
   // .EditingControlDataGridView property.
   public DataGridView EditingControlDataGridView
   {
      get
      {
         return dataGridView;
      }
      set
      {
         dataGridView = value;
      }
   }

   // Implements the IDataGridViewEditingControl
   // .EditingControlValueChanged property.
   public bool EditingControlValueChanged
   {
      get
      {
         return valueChanged;
      }
      set
      {
         valueChanged = value;
      }
   }

   // Implements the IDataGridViewEditingControl
   // .EditingPanelCursor property.
   public Cursor EditingPanelCursor
   {
      get
      {
         return base.Cursor;
      }
   }

   //protected override void OnValueChanged(EventArgs eventargs)
   //{
   //   // Notify the DataGridView that the contents of the cell
   //   // have changed.
   //   valueChanged = true;
   //   this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
   //   base.OnValueChanged(eventargs);
   //}
   protected override void OnTextChanged(EventArgs eventargs)
   {
      // Notify the DataGridView that the contents of the cell
      // have changed.
      valueChanged = true;
      this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
      base.OnTextChanged(eventargs);
   }
}

public class VvTextBoxColumn         : DataGridViewColumn
{
   public VvTextBoxColumn() : base(new VvTextBoxCell())
   {
   }

   public override DataGridViewCell CellTemplate
   {
      get
      {
         return base.CellTemplate;
      }
      set
      {
         // Ensure that the cell used for the template is a VvTextBoxCell.
         if(value != null && !value.GetType().IsAssignableFrom(typeof(VvTextBoxCell)))
         {
            throw new InvalidCastException("Must be a VvTextBoxCell");
         }
         base.CellTemplate = value;
      }
   }
}

public class VvTextBoxCell           : DataGridViewTextBoxCell
{

   public VvTextBoxCell() : base()
   {
      // Use the short date format.
      //this.Style.Format = "d";
      //Q: this.Style.Format = "dd.MM.yyyy.";
   }

   public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
   {
      // Set the value of the editing thisControl to the current cell value.
      base.InitializeEditingControl(rowIndex, initialFormattedValue,
          dataGridViewCellStyle);
      VvTextBoxEditingControl ctl =
          DataGridView.EditingControl as VvTextBoxEditingControl;
      //ctl.Value = (DateTime)this.Value;
      //Q: ctl.Value = ZXC.ValOrDefault_DateTime(this.Value.ToString(), DateTime.Now);

      //if(this.Value != null)
      //   ctl.Text = this.Value.ToString();
      //else
      //   ctl.Text = "";


   }

   public override Type EditType
   {
      get
      {
         // Return the type of the editing contol that VvTextBoxCell uses.
         return typeof(VvTextBoxEditingControl);
      }
   }

   public override Type ValueType
   {
      get
      {
         // Return the type of the value that VvTextBoxCell contains.
         //return typeof(string);
         return base.ValueType;
      }
   }

   public override object DefaultNewRowValue
   {
      get
      {
         // Use the current date and time as the default value.

         //return DateTime.Now;
         return "";
      }
   }
}
