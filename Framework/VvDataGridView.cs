using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

public class VvDataGridView : DataGridView
{
   #region Original DataGridViewEnter sa neke news grupe

   //This override causes the DataGridView to use the enter key in a similar way as
   //the tab key


   //protected override bool ProcessDialogKey(Keys keyData)
   //{
   //   Keys key = (keyData & Keys.KeyCode);

   //      if(key == Keys.Enter)
   //   {
   //      /* Q: */ SendKeys.Send("{TAB}"); return true;
   //      //return this.ProcessRightKey(keyData);
   //   }
   //   return base.ProcessDialogKey(keyData);
   //}

   //// some test: kada ovdje uopce ulazi?! 
   //protected override bool ProcessDialogChar(char charCode)
   //{
   //   ZXC.aim_emsg("Udjosmo u DataGridViewEnter.ProcessDialogChar! (override)");
   //   return base.ProcessDialogChar(charCode);
   //}

   #region what?
   // Koji je ovo ku*ac?
   //public new bool ProcessRightKey(Keys keyData)
   //{
   //   Keys key = (keyData & Keys.KeyCode);


   //   if(key == Keys.Enter)
   //   {
   //      if((base.CurrentCell.ColumnIndex == (base.ColumnCount - 1)) && (base.CurrentCell.RowIndex == (base.RowCount - 1)))
   //      {

   //         //This causes the last cell to be checked for errors
   //         base.EndEdit();
   //         //((BindingSource)base.DataSource).AddNew();
   //         base.CurrentCell = base.Rows[base.RowCount - 1].Cells[1];
   //         return true;
   //      }


   //      if((base.CurrentCell.ColumnIndex == (base.ColumnCount - 1)) && (base.CurrentCell.RowIndex + 1 != base.NewRowIndex))
   //      {
   //         // Cells[0] je invisible cell za prjktKupdobCD, stoga ju treba skip-ati 
   //         //base.CurrentCell = base.Rows[base.CurrentCell.RowIndex + 1].Cells[0];
   //         base.CurrentCell = base.Rows[base.CurrentCell.RowIndex + 1].Cells[1];
   //         return true;
   //      }
   //      return base.ProcessRightKey(keyData);
   //   }
   //   return base.ProcessRightKey(keyData);
   //}
   #endregion what?

   // Be adviced: ovo ProcessDataGridViewKey() trza samo na sljedece tipke:

   //'A'
   //'0'
   //'C'
   //ESC
   //Enter
   //Space
   //Tab
   //Up Arrow
   //Down -||-
   //Left -||-
   //Right -||-
   //PgUp
   //PgDn
   //Home
   //End
   //Insert
   //Delete

   //protected override bool ProcessDataGridViewKey(KeyEventArgs e)
   //{
   //   if(e.KeyCode == Keys.Enter)
   //   {
   //      SendKeys.Send("{TAB}"); return true;
   //   }
   //   return base.ProcessDataGridViewKey(e);
   //}

   #region New Process...

   protected override bool ProcessDialogKey(Keys keyData)
   {
      if(NavKey(keyData))
         return true;        //handled
      return base.ProcessDialogKey(keyData);
   }

   protected override bool ProcessDataGridViewKey(KeyEventArgs e)
   {
      if(NavKey(e.KeyData))
         return true;        //handled
      return base.ProcessDataGridViewKey(e);
   }

   bool NavKey(Keys keyData)
   {
      if(this.CurrentCell == null) return false;

      if(IsDgvFind == true) return false; // da strelice rade gore dole ...

      int col = this.CurrentCell.ColumnIndex;
      int row = this.CurrentCell.RowIndex;
      switch(keyData)
      {
         case Keys.Down:
            col--; row++;
            goto case Keys.Right;    //fall through
         case Keys.Right:
         case Keys.Tab:
         case Keys.Enter:
            while(row < this.Rows.Count)
            {
               while(++col < this.Columns.Count)
               {
                  if(!this[col, row].ReadOnly && this[col, row].Visible)
                  {
                     try
                     {
                        this.CurrentCell = this[col, row];
                        return true;
                     }
                     catch(InvalidOperationException) { return true; }
                  }
               }
               col = -1;
               row++;
            }
            return true;

         case Keys.Up:
            col++; row--;
            goto case Keys.Left;    //fall through
         case Keys.Left:
         case Keys.Tab | Keys.Shift:
         case Keys.Enter | Keys.Shift:
            while(row >= 0)
            {
               while(--col >= 0)
               {
                  if(!this[col, row].ReadOnly && this[col, row].Visible)
                  {
                     try
                     {
                        this.CurrentCell = this[col, row];
                        return true;
                     }
                     catch(InvalidOperationException) { return true; }
                  }
               }
               col = this.Columns.Count;
               row--;
            }
            return true;
      }
      return false;
   }

   #endregion New Process...

   public void UpdateVvLookUpItem_OnCellClick(object sender, DataGridViewCellEventArgs e)
   {
      if(Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Alt)
      {
         UpdateVvLookUpItem_4DataGridViewEnter_Core(this.Columns[this.CurrentCell.ColumnIndex].Tag as VvTextBox);
      }
   }

   public void UpdateVvDataRecord_OnCellClick(object sender, DataGridViewCellEventArgs e)
   {
      if(Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Alt)
      {
         UpdateVvDataRecord_4DataGridViewEnter_Core();
      }
   }

   public void UpdateVvLookUpItem_OnKeyDown(object sender, KeyEventArgs e)
   {
      if(VvUserControl.TriggerKey_ForUpdateVvDataRecord(e) == true) UpdateVvLookUpItem_4DataGridViewEnter_Core(this.Columns[this.CurrentCell.ColumnIndex].Tag as VvTextBox);
   }

   public void UpdateVvDataRecord_OnKeyDown(object sender, KeyEventArgs e)
   {
      if(VvUserControl.TriggerKey_ForUpdateVvDataRecord(e) == true) UpdateVvDataRecord_4DataGridViewEnter_Core();
   }

   public void UpdateVvLookUpItem_4DataGridViewEnter_Core(VvTextBox vvtb)
   {

      if(ReadOnly == false && vvtb != null && vvtb.JAM_IsVvLookUp)
      {
         // 23.02.2016: 
         if(vvtb.JAM_IsInDataGridView) this.BeginEdit(false);

         VvLookUpItem lui = VvUserControl.BtnChangeLookUpSelection_Click(vvtb, EventArgs.Empty);

         this.BeginEdit(false);

         VvTextBoxEditingControl vvtbEC = EditingControl as VvTextBoxEditingControl;

         string findResult = vvtb.Text;

         // 23.02.2016: 
       //if(findResult.NotEmpty())
         if(findResult.NotEmpty() && lui != null)
         {
            //CurrentCell.Value = findResult;
            vvtbEC.EditingControlFormattedValue = findResult;
            //EndEdit();

            //SetEventualLookupDataTakers(vvtb); preseljeno 

            // 23.02.2016: ??? !!! ??? komentirao ovo dole, jer je na gridu isao predaleko 
            //SendKeys.Send("{TAB}");
         }
      }
   }

   public void SetEventualLookupDataTakers(VvTextBox vvtb)
   {
      int rowIdx = this.CurrentCell.RowIndex;

      VvLookUpItem chosenLui = vvtb.JAM_ChosenLookUpItem;

      if(vvtb.JAM_lui_CdTaker_JAM_Name.NotEmpty())
      {
         if(chosenLui != null)
         {
            PutCell(vvtb.JAM_lui_CdTaker_JAM_Name, rowIdx, chosenLui.Cd);
         }
      }

      if(vvtb.JAM_lui_NameTaker_JAM_Name.NotEmpty())
      {
         if(chosenLui != null)
         {
            PutCell(vvtb.JAM_lui_NameTaker_JAM_Name, rowIdx, chosenLui.Name);
         }
      }

      if(vvtb.JAM_lui_NumberTaker_JAM_Name.NotEmpty())
      {
         if(chosenLui != null)
         {
            PutCell(vvtb.JAM_lui_NumberTaker_JAM_Name, rowIdx, chosenLui.Number);
         }
      }

      if(vvtb.JAM_lui_NumberTaker2_JAM_Name.NotEmpty())
      {
         if(chosenLui != null)
         {
            PutCell(vvtb.JAM_lui_NumberTaker2_JAM_Name, rowIdx, chosenLui.Number2);
         }
      }

      if(vvtb.JAM_lui_FlagTaker_JAM_Name.NotEmpty())
      {
         if(chosenLui != null)
         {
            PutCell(vvtb.JAM_lui_FlagTaker_JAM_Name, rowIdx, chosenLui.Flag);
         }
      }

      if(vvtb.JAM_lui_IntegerTaker_JAM_Name.NotEmpty())
      {
         if(chosenLui != null)
         {
            PutCell(vvtb.JAM_lui_IntegerTaker_JAM_Name, rowIdx, chosenLui.Integer);
         }
      }

      if(vvtb.JAM_lui_UintegerTaker_JAM_Name.NotEmpty())
      {
         if(chosenLui != null)
         {
            PutCell(vvtb.JAM_lui_UintegerTaker_JAM_Name, rowIdx, chosenLui.Uinteger);
         }
      }

      if(vvtb.JAM_lui_DateTaker_JAM_Name.NotEmpty())
      {
         if(chosenLui != null)
         {
            PutCell(vvtb.JAM_lui_DateTaker_JAM_Name, rowIdx, chosenLui.DateT);
         }
      }

   }


   // Ovdje dolazimo SAMO na Ctrl + Space ili F, a ne i nakon F2 pa ClickButton... !!! !!! !!! !!
   // F2 ... dolazimo na 
   //           UpdateVvDataRecord_4VvTextBox_________Core() u VvTextBox.cs -u 
   private void UpdateVvDataRecord_4DataGridViewEnter_Core()
   {
      int       currCol = this.CurrentCell.ColumnIndex;
      VvTextBox vvtb    = this.Columns[currCol].Tag as VvTextBox;

      if(ReadOnly == false && vvtb != null && (vvtb.JAM_IsAutocomplete || vvtb.JAM_IsOtsInfo))
      {
         this.BeginEdit(false);

         VvTextBoxEditingControl vvtbEC = EditingControl as VvTextBoxEditingControl;

         List<OtsTipBrGroupInfo> choosenOtsList = null;


         /* !!! !!! !!! !!! !!! !!! !!! !!! !!! !!! !!! !!! !!! !!! !!! !!! !!! */
         object findResult = null;

         if(vvtb.JAM_IsOtsInfo)
         {
            choosenOtsList = VvUserControl.UpdateOtsInfo(vvtb);

            ZXC.TheVvForm.DumpChosenOtsList_OnNalogDUC(choosenOtsList);
         }
         /* !!! !!! !!! !!! !!! !!! !!! !!! !!! !!! !!! !!! !!! !!! !!! !!! !!! */
         else
         {
            findResult = VvTextBox.UpdateVvDataRecord_And_SetSifrarAndAutocomplete_Core(vvtb);

            if(findResult != null)
            {
               //CurrentCell.Value = findResult;
               vvtbEC.EditingControlFormattedValue = findResult;
               //EndEdit();
               SendKeys.Send("{TAB}");
            }
         }
      }
   }

   #endregion Original DataGridViewEnter sa neke news grupe

   #region PutCell, GetCell

   public int IdxForColumn(string colName)
   {
      if(ColExists(colName)) return this.Columns[colName].Index;
      else                   return -1;
   }

   private bool ColExists(string colName)
   {
      return this.Columns.Contains(colName);
   }

   public bool CI_OK(int colIndex)
   {
      return colIndex >= 0; // else it is '-1'... meaning this column daznt egzist in this 
   }

   public void PutCell(int colIdx, int rowIdx, object value)
   {
      if(CI_OK(colIdx))
      {
         this[colIdx, rowIdx].Value = value;
      }
   }
   public void PutCell(string colName, int rowIdx, object value)
   {
      PutCell(IdxForColumn(colName), rowIdx, value);
   }

   public object GetCellValueAsObject(int colIdx, int rowIdx, bool isDirtyFlagging)
   {
      // 14.04.2016: 
    //if(isDirtyFlagging                                                   ) return this[colIdx, rowIdx].EditedFormattedValue;
      if(isDirtyFlagging && CurrentCell != null && CurrentCell.IsInEditMode) return this[colIdx, rowIdx].EditedFormattedValue;
      else                                                                   return this[colIdx, rowIdx].               Value;
   }

   private object GetObjectCell(int colIdx, int rowIdx, bool isDirtyFlagging)
   {
      if(colIdx < 0) return null;

      object value = GetCellValueAsObject(colIdx, rowIdx, isDirtyFlagging);

      if(value != null && value.ToString().Length > 0) return value;
      else                                             return null;
   }

   public uint GetUint32Cell(int colIdx, int rowIdx, bool isDirtyFlagging)
   {
      if(colIdx < 0) return 0;

      object value = GetCellValueAsObject(colIdx, rowIdx, isDirtyFlagging);

      if(value == null) return 0;
      else              return ZXC.ValOrZero_UInt(value.ToString());
   }
   public uint GetUint32Cell(string colName, int rowIdx, bool isDirtyFlagging)
   {
      return GetUint32Cell(IdxForColumn(colName), rowIdx, isDirtyFlagging);
   }

   public decimal GetDecimalCell(int colIdx, int rowIdx, bool isDirtyFlagging)
   {
      return GetDecimalCell(colIdx, rowIdx, isDirtyFlagging, false);
   }

   public decimal GetDecimalCell(int colIdx, int rowIdx, bool isDirtyFlagging, bool isForPercent)
   {
      if(colIdx < 0) return 0;

      object value = GetCellValueAsObject(colIdx, rowIdx, isDirtyFlagging);

      if(value == null) return 0;
      else              return ZXC.ValOrZero_Decimal(value.ToString(), 2, isForPercent); // ovo '2' popravi kasnije na univerzalni br decimala!!!
   }
   public decimal GetDecimalCell(string colName, int rowIdx, bool isDirtyFlagging)
   {
      return GetDecimalCell(IdxForColumn(colName), rowIdx, isDirtyFlagging);
   }

   public string GetStringCell(int colIdx, int rowIdx, bool isDirtyFlagging)
   {
      if(colIdx < 0) return "";

      object value = GetCellValueAsObject(colIdx, rowIdx, isDirtyFlagging);

      if(value == null) return "";
      else              return (value.ToString());
   }
   public string GetStringCell(string colName, int rowIdx, bool isDirtyFlagging)
   {
      return GetStringCell(IdxForColumn(colName), rowIdx, isDirtyFlagging);
   }

   public DateTime GetDateCell(int colIdx, int rowIdx, bool isDirtyFlagging)
   {
      if(colIdx < 0) return DateTime.MinValue;

      object value = GetCellValueAsObject(colIdx, rowIdx, isDirtyFlagging);

      if(value == null) return DateTime.MinValue;
      else              return ZXC.ValOr_01010001_DateTime(value.ToString());
   }
   public DateTime GetDateCell(string colName, int rowIdx, bool isDirtyFlagging)
   {
      return GetDateCell(IdxForColumn(colName), rowIdx, isDirtyFlagging);
   }


   public bool GetBoolCell(int colIdx, int rowIdx, bool isDirtyFlagging)
   {
      if(colIdx < 0) return false;

      object value = GetCellValueAsObject(colIdx, rowIdx, isDirtyFlagging);

      if(value == null) return false;
      else              return (bool)value;
   }
   public bool GetBoolCell(string colName, int rowIdx, bool isDirtyFlagging)
   {
      return GetBoolCell(IdxForColumn(colName), rowIdx, isDirtyFlagging);
   }

   public void ClearRowContent(int rowIdx)
   {
      for(int i = 0; i < this.ColumnCount; i++)
      {
         if(this.Columns[i] is DataGridViewCheckBoxColumn) { this.Rows[rowIdx].Cells[i].Value = false; continue; }

         if(this.Columns[i] is VvCheckBoxColumn && ((VvCheckBoxColumn)this.Columns[i]).VvSupressClearingOnClearAllRowValues) continue;

         this.Rows[rowIdx].Cells[i].Value = "";
      }
   }

   public int GetIntCell(int colIdx, int rowIdx, bool isDirtyFlagging)
   {
      if(colIdx < 0) return 0;

      object value = GetCellValueAsObject(colIdx, rowIdx, isDirtyFlagging);

      if(value == null) return 0;
      else              return ZXC.ValOrZero_Int(value.ToString());
   }
   public int GetIntCell(string colName, int rowIdx, bool isDirtyFlagging)
   {
      return GetIntCell(IdxForColumn(colName), rowIdx, isDirtyFlagging);
   }

   #endregion PutCell, GetCell

   #region Create VvTextBox ColumnTemplate And VvTextBox Column And CreateCalendarColumn

   public VvTextBox CreateVvTextBoxFor_String_ColumnTemplate(string name, VvDaoBase vvDao, int dbColIdx, string statusText)
   {
      return CreateVvTextBoxFor_JOB_ColumnTemplate(typeof(string), name, vvDao, dbColIdx, statusText, 0, false, false);
   }

   public VvTextBox CreateVvTextBoxFor_LookUp_ColumnTemplate(string name, VvDaoBase vvDao, int dbColIdx, string statusText)
   {
      return CreateVvTextBoxFor_JOB_ColumnTemplate(typeof(string), name, vvDao, dbColIdx, statusText, 0, false, true);
   }

   public VvTextBox CreateVvTextBoxFor_Decimal_ColumnTemplate(int numberOfDecimalPlaces, string name, VvDaoBase vvDao, int dbColIdx, string statusText)
   {
      return CreateVvTextBoxFor_JOB_ColumnTemplate(typeof(decimal), name, vvDao, dbColIdx, statusText, numberOfDecimalPlaces, false, false);
   }

   public VvTextBox CreateVvTextBoxFor_Integer_ColumnTemplate(bool oce_nece_leadingZero, string name, VvDaoBase vvDao, int dbColIdx, string statusText)
   {
      return CreateVvTextBoxFor_JOB_ColumnTemplate(typeof(int), name, vvDao, dbColIdx, statusText, 0, oce_nece_leadingZero, false);
   }

   public VvTextBox CreateVvTextBoxFor_DateTime_ColumnTemplate(string name, VvDaoBase vvDao, int dbColIdx, string statusText)
   {
      return CreateVvTextBoxFor_JOB_ColumnTemplate(typeof(DateTime), name, vvDao, dbColIdx, statusText, 0, false, false);
   }

   /// <summary>
   /// Ako je dbColIdx negativan, znaci da col ne postoji u datoteci, pa je abs(dbColIdx) zapravo maxColLength 
   /// </summary>
   /// <param name="valueType"></param>
   /// <param name="name"></param>
   /// <param name="vvDao"></param>
   /// <param name="dbColIdx"></param>
   /// <param name="statusText"></param>
   /// <param name="numberOfDecimalPlaces"></param>
   /// <param name="oce_nece_leadingZero"></param>
   /// <returns></returns>
   private VvTextBox CreateVvTextBoxFor_JOB_ColumnTemplate(Type valueType, string name, VvDaoBase vvDao, int dbColIdx, string statusText, int numberOfDecimalPlaces, bool oce_nece_leadingZero, bool isForLookUp)
   {
      VvTextBox vvTextBox;

      if(isForLookUp == true)
      {
         vvTextBox = VvTextBox.VvTextBoxLookUpFactory(name, "", 0, 0);
      }
      else
      {
         vvTextBox = new VvTextBox();
      }

      vvTextBox.Name = name;
      vvTextBox.JAM_StatusText = statusText;
      vvTextBox.JAM_IsInDataGridView = true;
      vvTextBox.JAM_ValueType = valueType;

      if(dbColIdx < 0) // Ako je dbColIdx negativan, znaci da col ne postoji u datoteci, pa je abs(dbColIdx) zapravo maxColLength 
      {
         vvTextBox.MaxLength = Math.Abs(dbColIdx);
      }
      else
      {
         vvTextBox.MaxLength = vvDao.GetSchemaColumnSize(dbColIdx);
      }

      if(valueType == typeof(decimal))
      {
         vvTextBox.JAM_MarkAsNumericTextBox(numberOfDecimalPlaces, true);
      }

      if(valueType == typeof(int) || valueType == typeof(uint))
      {
         vvTextBox.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

         if(oce_nece_leadingZero == true) vvTextBox.JAM_FillCharacter = '0';
      }

      return vvTextBox;
   }

   public VvTextBoxColumn CreateVvTextBoxColumn(VvTextBox vvTextBox, VvDaoBase vvDao, int dbColIdx, string headerText, int _width)
   {
      return CreateVvTextBoxColumn(vvTextBox, vvDao, vvDao.GetSchemaColumnName(dbColIdx), headerText, _width);
   }

   public VvTextBoxColumn CreateVvTextBoxColumn(VvTextBox vvTextBox, VvDaoBase vvDao, string colName, string headerText, int _width)
   {
      VvTextBoxColumn colVvText = new VvTextBoxColumn();

      colVvText.Tag        = vvTextBox;
      colVvText.Name       = colName;
      colVvText.HeaderText = headerText;

      colVvText.ValueType = vvTextBox.JAM_ValueType;

      if(_width != 0) colVvText.Width = _width;

      colVvText.SortMode = DataGridViewColumnSortMode.Automatic;

      if(vvTextBox.JAM_IsNumericTextBox)
      {
         colVvText.DefaultCellStyle.Alignment      = DataGridViewContentAlignment.MiddleRight;
         colVvText.HeaderCell.Style.Alignment      = DataGridViewContentAlignment.MiddleRight;
         colVvText.DefaultCellStyle.Format         = VvUserControl.GetDgvCellStyleFormat_Number(vvTextBox.JAM_NumberOfDecimalPlaces, true, vvTextBox.JAM_IsForPercent);
         colVvText.DefaultCellStyle.FormatProvider = VvUserControl.GetDgvCellStyleFormatProvider(vvTextBox.JAM_NumberOfDecimalPlaces);

         colVvText.SortMode = DataGridViewColumnSortMode.NotSortable; // !!! // inace 'DataGridViewContentAlignment.MiddleRight' ne sljaka dobro

      } // if(vvTextBox.JAM_IsNumericTextBox) 

      if(vvTextBox.JAM_FillCharacter == '0')
      {
         colVvText.DefaultCellStyle.Format = VvUserControl.GetDgvCellStyleFormat_ZeroFillInt(vvTextBox.MaxLength, true);
      }

      if(vvTextBox.JAM_ReadOnly == true)
      {
         colVvText.ReadOnly = true;
      }

      // ?? koji mi_Open je ovo a?
      if(vvTextBox.JAM_DataRequired == true)
      {
         DataGridViewCell cell = new VvTextBoxCell();

         //cell.val
         colVvText.CellTemplate = cell;
      }

      if(vvTextBox.JAM_StatusText.NotEmpty())
      {
         colVvText.ToolTipText = vvTextBox.JAM_StatusText;
      }

      //_grid.Columns.Add(colVvText);
      this.Columns.Add(colVvText);

      return colVvText;
   }

   public VvDateTimePickerColumn CreateCalendarColumn(VvDaoBase vvDao, int dbColIdx, string headerText, int _width)
   {
      VvDateTimePickerColumn colDate = new VvDateTimePickerColumn();

      if(dbColIdx > 0) colDate.Name       = vvDao.GetSchemaColumnName(dbColIdx);
      colDate.HeaderText = headerText;
      colDate.ValueType  = typeof(DateTime);

      if(_width != 0) colDate.Width = _width;

      this.Columns.Add(colDate);

      return colDate;
   }

   public VvDateTimePickerColumn CreateCalendarColumn_R(VvDaoBase vvDao, string name, string headerText, int _width)
   {
      VvDateTimePickerColumn colDate = new VvDateTimePickerColumn();

      if(name.NotEmpty()) colDate.Name = name;
      colDate.HeaderText = headerText;
      colDate.ValueType  = typeof(DateTime);

      if(_width != 0) colDate.Width = _width;

      this.Columns.Add(colDate);

      return colDate;
   }


   public VvDateAnd_TIME_PickerColumn CreateCalendarAnd_TIMEColumn(VvDaoBase vvDao, int dbColIdx, string headerText, int _width)
   {
      VvDateAnd_TIME_PickerColumn colDate = new VvDateAnd_TIME_PickerColumn();

      if(dbColIdx > 0) colDate.Name       = vvDao.GetSchemaColumnName(dbColIdx);
      colDate.HeaderText = headerText;
      colDate.ValueType  = typeof(DateTime);

      if(_width != 0) colDate.Width = _width;

      this.Columns.Add(colDate);

      return colDate;
   }

   public VvDate_TimeOnly_PickerColumn CreateCalendar_TimeOnly_Column(VvDaoBase vvDao, int dbColIdx, string headerText, int _width)
   {
      VvDate_TimeOnly_PickerColumn colDate = new VvDate_TimeOnly_PickerColumn();

      if(dbColIdx > 0) colDate.Name = vvDao.GetSchemaColumnName(dbColIdx);
      colDate.HeaderText = headerText;
      colDate.ValueType = typeof(DateTime);

      if(_width != 0) colDate.Width = _width;

      this.Columns.Add(colDate);

      return colDate;
   }

   public /*DataGridViewCheckBoxColumn*/ VvCheckBoxColumn CreateVvCheckBoxColumn(VvCheckBox vvCheckBox, VvDaoBase vvDao, int dbColIdx, string headerText, int _width)
   {
      //DataGridViewCheckBoxColumn colCbox = new DataGridViewCheckBoxColumn();
      VvCheckBoxColumn colCbox = new VvCheckBoxColumn();

      colCbox.Tag = vvCheckBox;

           if(dbColIdx == 111) colCbox.Name = "T_isProductLine";
      else if(dbColIdx == 222) colCbox.Name = "T_selection";
      else                     colCbox.Name = vvDao.GetSchemaColumnName(dbColIdx);
      
      colCbox.HeaderText = headerText;
      colCbox.ValueType = typeof(bool);
      colCbox.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
     // colCbox.DefaultCellStyle.Font      = ZXC.vvFont.BaseBoldFont;

      if(_width != 0) colCbox.Width = _width;

      this.Columns.Add(colCbox);

      return colCbox;
   }

   public DataGridViewCheckBoxColumn CreateClassicCheckBoxColumn(CheckBox classicCheckBox, VvDaoBase vvDao, int dbColIdx, string headerText, int _width)
   {
      DataGridViewCheckBoxColumn colCbox = new DataGridViewCheckBoxColumn();

      colCbox.Tag = classicCheckBox;

           if(dbColIdx == 111) colCbox.Name = "T_isProductLine";
      else if(dbColIdx == 222) colCbox.Name = "T_selection";
      else if(dbColIdx == 333) colCbox.Name = "T_skip";
      else                     colCbox.Name = vvDao.GetSchemaColumnName(dbColIdx);

      colCbox.HeaderText = headerText;
      colCbox.ValueType = typeof(bool);
      colCbox.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
      // colCbox.DefaultCellStyle.Font      = ZXC.vvFont.BaseBoldFont;

      if(_width != 0) colCbox.Width = _width;

      this.Columns.Add(colCbox);

      return colCbox;
   }

   public DataGridViewTextBoxColumn CreateScrollColumn(string name, int _width)
   {
      DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();

      col.Name = name;
      col.HeaderText = "";
      col.ValueType = typeof(string);
      col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
      col.ReadOnly = true;

      if(_width != 0) col.Width = _width;

      this.Columns.Add(col);

      return col;

   
   }

   #endregion Create VvTextBox ColumnTemplate And VvTextBox Column

   #region Additional Propertiz

   public DataGridView TheLinkedGrid_ColC  { get; set; } // column chooser Cbx link 
   public DataGridView TheLinkedGrid_ColC2 { get; set; } // column chooser     link 
   public DataGridView TheLinkedGrid_Sum   { get; set; } // sum theGrid1       link 

   public bool ThisIsOneRowFixedGrid { get; set; }

   public int TheSumOfPreferredWidths { get; set; }

   public virtual bool IsDgvFind { get { return (false); } }

   #endregion Additional Propertiz

   #region ClearDGV_RecIdColumn

   public void ClearDGV_RecIdColumn()
   {
      int cIdx = IdxForColumn("T_recID");

      if(cIdx < 0) throw new Exception("Ova DGV [" + this + "]\n\nNEMA column 'T_recID'");

      for(int rIdx = 0; rIdx < this.RowCount - 1; ++rIdx)
      {
         PutCell(cIdx, rIdx, 0);
      }
   }

   #endregion ClearDGV_RecIdColumn

   public DataGridViewRow CloneWithValues(DataGridViewRow row)
   {
      DataGridViewRow clonedRow = (DataGridViewRow)row.Clone();
      for(Int32 index = 0; index < row.Cells.Count; index++)
      {
         if(row.Cells[index].Visible)
            clonedRow.Cells[index].Value = row.Cells[index].Value;
      }
      return clonedRow;
   }

   public void SetDgvRowColor(int rowIdx, System.Drawing.Color backColor, System.Drawing.Color foreColor )
   {
      foreach(DataGridViewTextBoxCell tbxCell in this.Rows[rowIdx].Cells)
      {
         tbxCell.Style.BackColor = backColor;
         tbxCell.Style.ForeColor = foreColor;
      }
   }

 //internal int GrouppingColor(int grColIdx)
   internal void GrouppingColor(int grColIdx)
   {
      if(this.Rows.Count.IsZero()) return/* 0*/;

      bool thisIsNewGR = false;

      System.Drawing.Color theColor = System.Drawing.Color.White;
      System.Drawing.Color color1   = System.Drawing.Color.White;
      System.Drawing.Color color2   = System.Drawing.Color.LightGray;

    //int grCount = 0;

      string currGrID = this.GetStringCell(grColIdx, 0, false);

    //for(int rIdx = 0; rIdx < RowCount - 1; ++rIdx)
      for(int rIdx = 0; rIdx < RowCount    ; ++rIdx)
      {
         thisIsNewGR = GetStringCell(grColIdx, rIdx, false) != currGrID;

         if(thisIsNewGR)
         {
            theColor = ToggleColors(theColor, color1, color2);
            this.Rows[rIdx].DefaultCellStyle.BackColor = theColor;
            currGrID = this.GetStringCell(grColIdx, rIdx, false);
         }
         else
         {
            if(rIdx.NotZero()) this.Rows[rIdx].DefaultCellStyle.BackColor = this.Rows[rIdx-1].DefaultCellStyle.BackColor;
         }
      }

    //return grCount;
   }

   private System.Drawing.Color ToggleColors(System.Drawing.Color currColor, System.Drawing.Color color1, System.Drawing.Color color2)
   {
      if(currColor == color1) return color2;
      else                    return color1;
   }

   internal void ColorNegativValue(int colIdx)
   {
      if(this.Rows.Count.IsZero()) return;

      System.Drawing.Color colorPozitiv = System.Drawing.Color.Black;
      System.Drawing.Color colorNegativ = System.Drawing.Color.Red;

      decimal currNum;

      for(int rIdx = 0; rIdx < RowCount; ++rIdx)
      {
         currNum = this.GetDecimalCell(colIdx, rIdx, false);

         if(currNum.IsNegative())
         {
            this.Rows[rIdx].DefaultCellStyle.ForeColor = colorNegativ;
         }
         else
         {
            this.Rows[rIdx].DefaultCellStyle.ForeColor = colorPozitiv;
         }
      }
   }

   public int VvEffectiveRowCount
   {
      get 
      {
         if(this.AllowUserToAddRows == true) return this.RowCount - 1;
         else                                return this.RowCount    ; 
      }
   }
}

