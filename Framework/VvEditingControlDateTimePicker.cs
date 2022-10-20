//-------------------------------------------------------------------------
// VvDateTimePickerEditingControl.cs, from the MSDN topic 
//              "How to: ServerHost Controls in Windows Forms DataGridView Cells
//-------------------------------------------------------------------------
using System;
using System.Windows.Forms;

public class VvDateTimePickerEditingControl : VvDateTimePicker, IDataGridViewEditingControl
{
    DataGridView dataGridView;
    private bool valueChanged = false;
    int rowIndex;

    public VvDateTimePickerEditingControl()
    {
        this.Format = DateTimePickerFormat.Short;
        this.KeyDown += new KeyEventHandler(DateTimePickerEx_KeyDown);
     }

     private void DateTimePickerEx_KeyDown(object sender, KeyEventArgs e)
     {
        if(e.KeyCode == Keys.Delete)
        {
           EditingControlDataGridView.CurrentCell.Value = DateTime.MinValue;

           EditingControlDataGridView.EndEdit();

           ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode = true;
        }
     }

    // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
    // property.
    public object EditingControlFormattedValue
    {
        get
        {
           //return this.Value.ToShortDateString();
           //return this.Value.ToString(ZXC.VvDateFormat);
           return this.Value.ToString(ZXC.VvDateFormat.TrimEnd('.')); // bez ovoga Trim pada jerbo mu smeta tocka na kraju!!!??? 
        }
        set
        {
            String newValue = value as String;
            if(newValue != null)
            {
               this.Value = DateTime.Parse(newValue.TrimEnd('.'));
            }
        }
    }

    // Implements the 
    // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
    public object GetEditingControlFormattedValue(
        DataGridViewDataErrorContexts context)
    {
        return EditingControlFormattedValue;
    }

    // Implements the 
    // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
    public void ApplyCellStyleToEditingControl(
        DataGridViewCellStyle dataGridViewCellStyle)
    {
        this.Font = dataGridViewCellStyle.Font;
        this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
        this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
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
        switch (key & Keys.KeyCode)
        {
            case Keys.Left:
            case Keys.Up:
            case Keys.Down:
            case Keys.Right:
            case Keys.Home:
            case Keys.End:
            case Keys.PageDown:
            case Keys.PageUp:
            case Keys.OemPeriod:
            case Keys.Decimal:
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

    protected override void OnValueChanged(EventArgs eventargs)
    {
        // Notify the DataGridView that the contents of the cell
        // have changed.
        valueChanged = true;
        this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
        base.OnValueChanged(eventargs);
    }
}

public class VvDateTimePickerColumn         : DataGridViewColumn
{
   public VvDateTimePickerColumn()  : base(new VvDateTimePickerCell())
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
         // Ensure that the cell used for the template is a VvDateTimePickerCell.
         if(value != null && !value.GetType().IsAssignableFrom(typeof(VvDateTimePickerCell)))
         {
            throw new InvalidCastException("Must be a CalendarCell");
         }
         base.CellTemplate = value;
      }
   }
}

public class VvDateTimePickerCell           : DataGridViewTextBoxCell
{

   public VvDateTimePickerCell() : base()
   {
      // Use the short date format.
      //this.Style.Format = "d";
      this.Style.Format = ZXC.VvDateFormat;
   }

   public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
   {
      // Set the value of the editing thisControl to the current cell value.
      base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
      VvDateTimePickerEditingControl ctl = DataGridView.EditingControl as VvDateTimePickerEditingControl;

      //ctl.Value = (DateTime)this.Value;

      DateTime currDT;

      if(this.Value == null)
      {
         currDT = DateTime.Now;
      }
      else
      {
         currDT = ZXC.ValOr_01010001_DateTime(this.Value.ToString());
         if(currDT == DateTime.MinValue) currDT = DateTime.Now;
      }
      ctl.Value = currDT;

      //ctl.Value = ZXC.ValOrDefault_DateTime(this.Value.ToString(), /*DateTime.MinValue*/ DateTimePicker.MinimumDateTime);

      ctl.BackColor = ZXC.vvColors.vvTBoxHotOn_GotFocus_BackColor; //Color.LightSkyBlue;

      ctl.CustomFormat = ZXC.VvDateFormat;
      ctl.Format = DateTimePickerFormat.Custom;
   }

   public override Type EditType
   {
      get
      {
         // Return the type of the editing contol that VvDateTimePickerCell uses.
         return typeof(VvDateTimePickerEditingControl);
      }
   }

   public override Type ValueType
   {
      get
      {
         // Return the type of the value that VvDateTimePickerCell contains.
         return typeof(DateTime);
      }
   }

   public override object DefaultNewRowValue
   {
      get
      {
         // Use the current date and time as the default value.

         return DateTime.MinValue;
         //return "";
      }
   }
}

#region VVDateAnd_TIME_

public class VvDateAnd_TIME_PickerEditingControl : VvDateTimePicker, IDataGridViewEditingControl
{
    DataGridView dataGridView;
    private bool valueChanged = false;
    int rowIndex;

    public VvDateAnd_TIME_PickerEditingControl()
    {
        this.Format = DateTimePickerFormat.Short;
        this.KeyDown += new KeyEventHandler(DateTimePickerEx_KeyDown);
     }

     private void DateTimePickerEx_KeyDown(object sender, KeyEventArgs e)
     {
        if(e.KeyCode == Keys.Delete)
        {
           EditingControlDataGridView.CurrentCell.Value = DateTime.MinValue;

           EditingControlDataGridView.EndEdit();

           ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode = true;
        }
     }

    // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
    // property.
    public object EditingControlFormattedValue
    {
        get
        {
           //return this.Value.ToShortDateString();
           //return this.Value.ToString(ZXC.VvDateFormat);
           return this.Value.ToString(ZXC.VvDateAndTimeFormat.TrimEnd('.')); // bez ovoga Trim pada jerbo mu smeta tocka na kraju!!!??? 
        }
        set
        {
            String newValue = value as String;
            if(newValue != null)
            {
               this.Value = DateTime.Parse(newValue.TrimEnd('.'));
            }
        }
    }

    // Implements the 
    // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
    public object GetEditingControlFormattedValue(
        DataGridViewDataErrorContexts context)
    {
        return EditingControlFormattedValue;
    }

    // Implements the 
    // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
    public void ApplyCellStyleToEditingControl(
        DataGridViewCellStyle dataGridViewCellStyle)
    {
        this.Font = dataGridViewCellStyle.Font;
        this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
        this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
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
        switch (key & Keys.KeyCode)
        {
            case Keys.Left:
            case Keys.Up:
            case Keys.Down:
            case Keys.Right:
            case Keys.Home:
            case Keys.End:
            case Keys.PageDown:
            case Keys.PageUp:
            case Keys.OemPeriod:
            case Keys.Decimal:
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

    protected override void OnValueChanged(EventArgs eventargs)
    {
        // Notify the DataGridView that the contents of the cell
        // have changed.
        valueChanged = true;
        this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
        base.OnValueChanged(eventargs);
    }
}

public class VvDateAnd_TIME_PickerColumn         : DataGridViewColumn
{
   public VvDateAnd_TIME_PickerColumn()  : base(new VvDateAnd_TIME_PickerCell())
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
         // Ensure that the cell used for the template is a VvDateAnd_TIME_PickerCell.
         if(value != null && !value.GetType().IsAssignableFrom(typeof(VvDateAnd_TIME_PickerCell)))
         {
            throw new InvalidCastException("Must be a CalendarCell");
         }
         base.CellTemplate = value;
      }
   }
}

public class VvDateAnd_TIME_PickerCell           : DataGridViewTextBoxCell
{

   public VvDateAnd_TIME_PickerCell() : base()
   {
      // Use the short date format.
      this.Style.Format = "d";
      //this.Style.Format = ZXC.VvDateAndTimeFormat;
   }

   public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
   {
      // Set the value of the editing thisControl to the current cell value.
      base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
      VvDateAnd_TIME_PickerEditingControl ctl = DataGridView.EditingControl as VvDateAnd_TIME_PickerEditingControl;

      //ctl.Value = (DateTime)this.Value;

      DateTime currDT;

      if(this.Value == null)
      {
         currDT = DateTime.Now;
      }
      else
      {
         currDT = ZXC.ValOr_01010001_DateTime(this.Value.ToString());
         if(currDT == DateTime.MinValue) currDT = DateTime.Now;
      }
      ctl.Value = currDT;

      //ctl.Value = ZXC.ValOrDefault_DateTime(this.Value.ToString(), /*DateTime.MinValue*/ DateTimePicker.MinimumDateTime);

      ctl.BackColor = ZXC.vvColors.vvTBoxHotOn_GotFocus_BackColor; //Color.LightSkyBlue;

      ctl.CustomFormat = ZXC.VvDateAndTimeFormat;
      ctl.Format = DateTimePickerFormat.Custom;
   }

   public override Type EditType
   {
      get
      {
         // Return the type of the editing contol that VvDateAnd_TIME_PickerCell uses.
         return typeof(VvDateAnd_TIME_PickerEditingControl);
      }
   }

   public override Type ValueType
   {
      get
      {
         // Return the type of the value that VvDateAnd_TIME_PickerCell contains.
         return typeof(DateTime);
      }
   }

   public override object DefaultNewRowValue
   {
      get
      {
         // Use the current date and time as the default value.

         return DateTime.MinValue;
         //return "";
      }
   }
}

public class VvDate_TimeOnly_PickerEditingControl : VvDateTimePicker, IDataGridViewEditingControl
{
    DataGridView dataGridView;
    private bool valueChanged = false;
    int rowIndex;

    public VvDate_TimeOnly_PickerEditingControl()
    {
        this.Format = DateTimePickerFormat.Short;
        this.KeyDown += new KeyEventHandler(DateTimePickerEx_KeyDown);

       // 28.11.2017:
        this.ShowUpDown = true;
     }

     private void DateTimePickerEx_KeyDown(object sender, KeyEventArgs e)
     {
        if(e.KeyCode == Keys.Delete)
        {
           EditingControlDataGridView.CurrentCell.Value = DateTime.MinValue;

           EditingControlDataGridView.EndEdit();

           ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode = true;
        }
     }

    // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
    // property.
    public object EditingControlFormattedValue
    {
        get
        {
           //return this.Value.ToShortDateString();
           //return this.Value.ToString(ZXC.VvDateFormat);
           return this.Value.ToString(ZXC.VvTimeOnlyFormat.TrimEnd('.')); // bez ovoga Trim pada jerbo mu smeta tocka na kraju!!!??? 
        }
        set
        {
            String newValue = value as String;
            if(newValue != null)
            {
               this.Value = DateTime.Parse(newValue.TrimEnd('.'));
            }
        }
    }

    // Implements the 
    // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
    public object GetEditingControlFormattedValue(
        DataGridViewDataErrorContexts context)
    {
        return EditingControlFormattedValue;
    }

    // Implements the 
    // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
    public void ApplyCellStyleToEditingControl(
        DataGridViewCellStyle dataGridViewCellStyle)
    {
        this.Font = dataGridViewCellStyle.Font;
        this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
        this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
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
        switch (key & Keys.KeyCode)
        {
            case Keys.Left:
            case Keys.Up:
            case Keys.Down:
            case Keys.Right:
            case Keys.Home:
            case Keys.End:
            case Keys.PageDown:
            case Keys.PageUp:
            case Keys.OemPeriod:
            case Keys.Decimal:
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

    protected override void OnValueChanged(EventArgs eventargs)
    {
        // Notify the DataGridView that the contents of the cell
        // have changed.
        valueChanged = true;
        this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
        base.OnValueChanged(eventargs);
    }
}

public class VvDate_TimeOnly_PickerColumn : DataGridViewColumn
{
   public VvDate_TimeOnly_PickerColumn()  : base(new VvDate_TimeOnly_PickerCell())
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
         // Ensure that the cell used for the template is a VvDateAnd_TIME_PickerCell.
         if(value != null && !value.GetType().IsAssignableFrom(typeof(VvDate_TimeOnly_PickerCell)))
         {
            throw new InvalidCastException("Must be a CalendarCell");
         }
         base.CellTemplate = value;
      }
   }
}

public class VvDate_TimeOnly_PickerCell           : DataGridViewTextBoxCell
{

   public VvDate_TimeOnly_PickerCell() : base()
   {
      // Use the short date format.
      this.Style.Format = "d";
      //this.Style.Format = ZXC.VvDateAndTimeFormat;
   }

   public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
   {
      // Set the value of the editing thisControl to the current cell value.
      base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
      VvDate_TimeOnly_PickerEditingControl editingControl = DataGridView.EditingControl as VvDate_TimeOnly_PickerEditingControl;

      //ctl.Value = (DateTime)this.Value;

      DateTime currDT;

      if(this.Value == null)
      {
         currDT = DateTime.Now;
      }
      else
      {
       //currDT = ZXC.ValOr_01010001_DateTime(this.Value.ToString());
         currDT = ZXC.ValOr_01011753_DateTime(ZXC.ValOr_01010001_DateTime(this.Value.ToString()));
         if(currDT == DateTime.MinValue) currDT = DateTime.Now;
      }
      editingControl.Value = currDT;

      //ctl.Value = ZXC.ValOrDefault_DateTime(this.Value.ToString(), /*DateTime.MinValue*/ DateTimePicker.MinimumDateTime);

      editingControl.BackColor = ZXC.vvColors.vvTBoxHotOn_GotFocus_BackColor; //Color.LightSkyBlue;

      editingControl.CustomFormat = ZXC.VvTimeOnlyFormat;
      editingControl.Format = DateTimePickerFormat.Custom;
   }

   public override Type EditType
   {
      get
      {
         // Return the type of the editing contol that VvDateAnd_TIME_PickerCell uses.
         return typeof(VvDate_TimeOnly_PickerEditingControl);
      }
   }

   public override Type ValueType
   {
      get
      {
         // Return the type of the value that VvDateAnd_TIME_PickerCell contains.
         return typeof(DateTime);
      }
   }

   public override object DefaultNewRowValue
   {
      get
      {
         // Use the current date and time as the default value.

         return DateTime.MinValue;
         //return "";
      }
   }
}

#endregion VVDateAnd_TIME_
