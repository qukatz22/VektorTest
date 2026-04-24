using System;
using System.Drawing;
using System.Windows.Forms;
using Crownwood.DotNetMagic.Controls;
using System.Collections.Generic;
using CrystalDecisions.Windows.Forms;
using System.Linq;
//using ikvm.lang;

public interface IVvHasSumInDataLayerDocumentRecordUC
{
   void PutTransSumToDocumentSumFields();
}

public abstract class VvDocumLikeRecordUC : VvRecordUC
{
   #region Common Propertiz

   public abstract VvDocumentRecord VirtualDocumentRecord
   {
      get;
      set;
   }

   public abstract void Put_NewDocum_NumAndDateFields(uint dokNum, DateTime dokDate);

   /// <summary>
   /// Samo za Kopiraj MenuButtonOnClick! 
   /// </summary>
   /// <param name="ttNum"></param>
   public abstract void Put_NewTT_Num(uint ttNum);


   #endregion Common Propertiz
}

public abstract class VvDocumentRecordUC  : VvDocumLikeRecordUC
{

   #region Abstract Methods

   // TODO: !!!!!!!! 
   public abstract VvTransRecord GetDgvLineFields(int rIdx, bool dirtyFlagging, uint[] recIDtable);

   public /*abstract*/ virtual void PutDgvLineResultsFields(VvTransRecord trans_rec, int rowIdx, bool passPtrResultsToZaglavljeTranses) { }

   public abstract void PutDgvLineFields(VvTransRecord trans_rec, int rowIdx, bool skipRecID_andSerial_Columns);

   public abstract void PutDgvTransSumFields();

   #endregion Abstract Methods

   #region Common Propertiz

   public abstract VvDaoBase TheVvDaoTrans
   {
      get;
   }

   public   DataGridView TheColChooserGrid  { get; set; }
   //14.3.2011: ? public   DataGridView TheColChooserGrid2 { get; set; }
   public VvDataGridView TheG               { get; set; }
   public VvDataGridView TheSumGrid         { get; set; }
   
   protected List<VvTransRecord> VirtualTranses { get { return this.VirtualDocumentRecord.VirtualTranses; } }

   public const string AlwaysInvinsibleStr = "ThisIsAlwaysInvinsibleColumn";

   public abstract VvLookUpLista TheTtLookUpList { get; }

   #endregion Common Propertiz

   #region Transes Utils

   protected void MarkTransesToDelete(uint[] dgvRecIDtable)
   {
      MarkTransesToDelete_JOB(dgvRecIDtable, VirtualTranses);
   }

   protected void MarkTransesToDelete_JOB(uint[] dgvRecIDtable, List<VvTransRecord> _virtualTranses)
   {
      foreach(VvTransRecord trans_rec in _virtualTranses)
      {
         if(!dgvRecIDtable.Contains(trans_rec.VirtualRecID)) // LINQ 
         {
            trans_rec.SaveTransesWriteMode = ZXC.WriteMode.Delete;
         }
      }
   }

   #endregion Transes Utils

   #region TheG
   
   #region CreateVvDataGridView

   protected VvDataGridView CreateVvDataGridView(Control _parent, string _name)
   {
      VvDataGridView _theGrid = new VvDataGridView();

      _theGrid.Parent = _parent;
      _theGrid.Name   = _name;

      //==================================================================

      VvHamper.ApplyVVColorAndStyleTabCntrolChange(_theGrid);
      // _theGrid.BackgroundColor = SystemColors.ControlLight;
      _theGrid.AutoGenerateColumns = false;
      //_theGrid.CellBorderStyle = DataGridViewCellBorderStyle.Sunken;

      _theGrid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

      _theGrid.RowHeadersBorderStyle = _theGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
      _theGrid.ColumnHeadersHeight   = ZXC.QUN;
      _theGrid.RowTemplate.Height    = ZXC.QUN;

      //_theGrid.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
      _theGrid.RowHeadersWidth       = ZXC.Q3un + ZXC.Qun4;
      _theGrid.AllowUserToResizeRows = false;

      //==================================================================


      // Za <DragAndDrop Row Reordering> START 
      //_theGrid.AllowDrop = true;

      _theGrid.MouseMove += new MouseEventHandler(grid_MouseMove);
      _theGrid.MouseDown += new MouseEventHandler(grid_MouseDown);
      _theGrid.DragOver += new DragEventHandler(grid_DragOver);
      _theGrid.DragDrop += new DragEventHandler(grid_DragDrop);

      // Za <DragAndDrop Row Reordering> END   

      _theGrid.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(Grid_EditingControlShowing);
      _theGrid.CellBeginEdit += new DataGridViewCellCancelEventHandler(grid_CellBeginEdit_AtachJAM_EventHandlers);
      _theGrid.CellEndEdit += new DataGridViewCellEventHandler(grid_CellEndEdit_DetachJAM_EventHandlers);
      //_theGrid.CellValidating += new DataGridViewCellValidatingEventHandler(grid_CellValidating);
      //_theGrid.CellValidated += new DataGridViewCellEventHandler(grid_CellValidated);
      _theGrid.CellEnter += new DataGridViewCellEventHandler(grid_CellEnter_CopyPrevRowValue);
      _theGrid.CellEnter += new DataGridViewCellEventHandler(grid_CellEnter_SetStatusText);
      _theGrid.CellLeave += new DataGridViewCellEventHandler(grid_CellLeave_RestoreStatusText);

      _theGrid.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged);
      _theGrid.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged_NewColWidth);

      _theGrid.AllowUserToOrderColumns = true;
      /* */
      /* T_recID */
      /* */

      T_recID_CreateColumn(_theGrid);

      /* */
      /* T_serial */
      /* */

      T_serial_CreateColumn(_theGrid);



      _theGrid.RowsAdded += new DataGridViewRowsAddedEventHandler(grid_RowsAdded);
      _theGrid.RowsRemoved += new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);
      _theGrid.Sorted += new EventHandler(grid_Sorted);

      _theGrid.CellValidating += new DataGridViewCellValidatingEventHandler(_theGrid_CellValidating_SetDirtyFlag);

      _theGrid.CellEndEdit += new DataGridViewCellEventHandler(grid_CellEndEdit_CalcDocument);

      //_theGrid.CellEndEdit += new DataGridViewCellEventHandler(grid_CellEndEdit_GoToNextBarCodeColumn);

      _theGrid.CellFormatting += new DataGridViewCellFormattingEventHandler(grid_CellFormatting_FormatVvDateTime);

      _theGrid.Validating += new System.ComponentModel.CancelEventHandler(grid_Validating);

      //_theGrid.ScrollBars = ScrollBars.Vertical; !!!!! 15.12.2011 

      return _theGrid;
   }

   private void T_recID_CreateColumn(VvDataGridView _theGrid)
   {
      CreateAllwaysInvisibleDataGridViewColumn(_theGrid, "t_recID");
   }
   
   private void T_serial_CreateColumn(VvDataGridView _theGrid)
   {
      CreateAllwaysInvisibleDataGridViewColumn(_theGrid, "t_serial");
   }

   /*private*/ protected void CreateAllwaysInvisibleDataGridViewColumn(VvDataGridView _theGrid, string name)
   {
      DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
      col.Name    = name;

      //if(col.Name == "t_fakRecID" || col.Name == "t_otsKind")
      //{
      //   col.Visible = true;
      //}

      //else
      {
         col.Visible = false;
         col.Tag = VvDocumentRecordUC.AlwaysInvinsibleStr;
      }


      _theGrid.Columns.Add(col);
   }

   #endregion CreateVvDataGridView

   #region Event Handlers

   private static VvTextBox prevVvTextBox;

   static void JAM_VvTextBoxCell_Validating(object sender, DataGridViewCellValidatingEventArgs e)
   {
      DataGridView dgv = sender as DataGridView;

      VvTextBox vvTextBox = dgv.EditingControl as VvTextBox;

      e.Cancel = vvTextBox.JAM_ValidatingActions();
   }

   public static void grid_CellBeginEdit_AtachJAM_EventHandlers(object sender, DataGridViewCellCancelEventArgs e)
   {
      DataGridView dgv = sender as DataGridView;

      if(dgv[e.ColumnIndex, e.RowIndex] is VvTextBoxCell)
      {
         dgv.CellValidating += new DataGridViewCellValidatingEventHandler(JAM_VvTextBoxCell_Validating);

         VvTextBoxCell vvTBcell  = dgv[e.ColumnIndex, e.RowIndex] as VvTextBoxCell;
         VvTextBox     vvTextBox = vvTBcell.OwningColumn.Tag      as VvTextBox;

         if(vvTextBox.JAM_FieldExitWithValidationMethod != null)
         {
            dgv.CellValidating += new DataGridViewCellValidatingEventHandler(vvTextBox.JAM_FieldExitWithValidationMethod);
         }
      }
      //theGrid1[e.ColumnIndex, e.RowIndex].Style.ApplyStyle(theGrid1[e.ColumnIndex, e.RowIndex].InheritedStyle);
   }

   public static void grid_CellEndEdit_DetachJAM_EventHandlers(object sender, DataGridViewCellEventArgs e)
   {
      DataGridView dgv = sender as DataGridView;

      if(dgv[e.ColumnIndex, e.RowIndex] is VvTextBoxCell)
      {
         dgv.CellValidating -= new DataGridViewCellValidatingEventHandler(JAM_VvTextBoxCell_Validating);

         VvTextBoxCell vvTBcell  = dgv[e.ColumnIndex, e.RowIndex] as VvTextBoxCell;
         VvTextBox     vvTextBox = vvTBcell.OwningColumn.Tag      as VvTextBox;

         if(vvTextBox.JAM_FieldExitWithValidationMethod != null)
         {
            dgv.CellValidating -= new DataGridViewCellValidatingEventHandler(vvTextBox.JAM_FieldExitWithValidationMethod);
         }
      }
      //theGrid1[e.ColumnIndex, e.RowIndex].Style.ApplyStyle(theGrid1[e.ColumnIndex, e.RowIndex].InheritedStyle);
      //theGrid1[e.ColumnIndex, e.RowIndex].Value = theGrid1[e.ColumnIndex, e.RowIndex].Value;
   }

   void _theGrid_CellValidating_SetDirtyFlag(object sender, DataGridViewCellValidatingEventArgs e)
   {

      VvDataGridView dgv = sender as VvDataGridView;

      if(dgv.IsCurrentCellInEditMode                            == false &&
         ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode == false) return; // VvDateTimePicker kolone prelaskom TAB-om kroz njih    
                                                                                  // dizu CellValidatedEvent iz meni nepoznatog razloga!!! 

      VvTextBox vvtb = dgv.Columns[e.ColumnIndex].Tag as VvTextBox;

      // 21.10.2011:
      //if(vvtb != null) return; // znaci ovo je VvTExtBoxColumn kojoj ne treba '_theGrid_CellValidated_SetDirtyFlag()' 
      //                         // ovo dole treba za sve ostale (DateTimePicker, CheckBox, CpmboBox, ...)              
      if(dgv.EditingControl is VvTextBox) return;


      ZXC.TheVvForm.SetDirtyFlag(sender);
   }

   void grid_CellEndEdit_CalcDocument(object sender, DataGridViewCellEventArgs e)
   {
      if(e.ColumnIndex < 0) return;

      VvDataGridView dgv = sender as VvDataGridView;

      VvTextBox vvtb = dgv.Columns[e.ColumnIndex].Tag as VvTextBox;

      if(vvtb != null)
      {
         if(vvtb.JAM_ShouldCalcTrans)
         {
            if(VirtualDocumentRecord.IsOneTransChangeShouldRecalcOtherAllTranses == true)
            {
               GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
            }
            else
            {
               GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(dgv.CurrentRow.Index);
            }
         }
         else if(vvtb.JAM_ShouldSumGrid)
         {
            PutDgvTransSumFields();
         }
      }

      VvCheckBox vvcb = dgv.Columns[e.ColumnIndex].Tag as VvCheckBox;

      if(vvcb != null && vvcb.JAM_ShouldCalcTrans)
      {
         GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(dgv.CurrentRow.Index);
      }
   }

   //void grid_CellEndEdit_GoToNextBarCodeColumn(object sender, DataGridViewCellEventArgs e)
   //{
   //   if(e.ColumnIndex < 0) return;

   //   VvDataGridView dgv = sender as VvDataGridView;

   //   VvTextBox vvtb = dgv.Columns[e.ColumnIndex].Tag as VvTextBox;

   //   if(vvtb != null && vvtb.JAM_IsOnEndEditJump2NextRow)
   //   {
   //      //dgv[e.ColumnIndex, e.RowIndex + 1].Selected = true;
   //      //dgv.BeginEdit(false);

   //      //dgv.CurrentCell = dgv[e.ColumnIndex, e.RowIndex + 1];
   //      //dgv.BeginEdit(false);

   //      for(int currColIdx = e.ColumnIndex + 1; currColIdx < dgv.Columns.Count; ++currColIdx)
   //      {
   //         if(dgv.Columns[currColIdx].Tag == null) continue;

   //         if(dgv[currColIdx, e.RowIndex].Visible == true && ((VvTextBox)dgv.Columns[currColIdx].Tag).JAM_ReadOnly == false) SendKeys.Send("{TAB}");
   //      }
   //   }
   //}

   // 24.08.2015: 
 //public void GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS()
 //{
 //   for(int rIdx = 0; rIdx < TheG.RowCount - 1; ++rIdx)
 //   {
 //      GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(rIdx);
 //   }
 //}
   public void GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS()
   {
      if(ZXC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS_inProgress == true) return;

      ZXC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS_inProgress = true;

      for(int rIdx = 0; rIdx < TheG.RowCount -        1; ++rIdx)
      {
         GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(rIdx);
      }

      ZXC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS_inProgress = false;
   }

   public void GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(int rowIdx)
   {
      VvTransRecord trans_rec = GetDgvLineFields(rowIdx, false, null);

      trans_rec.CalcTransResults(VirtualDocumentRecord);

      PutDgvLineResultsFields(trans_rec, rowIdx, true);

      PutDgvTransSumFields();

      if(this is IVvHasSumInDataLayerDocumentRecordUC)
      {
         ((IVvHasSumInDataLayerDocumentRecordUC)this).PutTransSumToDocumentSumFields();
      }

      ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode = true;
      ZXC.TheVvForm.SetDirtyFlag("GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds");
   }

   public static void Grid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
   {
      Control      control  = e.Control;
      DataGridView dgv      = sender as DataGridView;

      if(control is VvTextBox)
      {
         VvTextBox vvtbTalon     = dgv.Columns[dgv.CurrentCell.ColumnIndex].Tag as VvTextBox;
         VvTextBox currVvTextBox = dgv.EditingControl                           as VvTextBox;

         if(prevVvTextBox != null) 
            prevVvTextBox.OnOpenOrCloseForEditActions(ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvRecordUC);

         currVvTextBox.TakeJAM_MembersFrom(vvtbTalon);
         currVvTextBox.OnOpenOrCloseForEditActions(ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvRecordUC);

         //5.10.2010:
         if(vvtbTalon.JAM_IsForPercent == true)
         {
            dgv.EditingControl.Text = dgv.EditingControl.Text.TrimEnd('%');
         }
         prevVvTextBox = currVvTextBox;
      }
      //else if(thisControl is VvDateTimePicker)
      //{
      //   VvDateTimePicker vvcb = theGrid1.EditingControl as VvDateTimePicker;
      //   //vvcb.ValueChanged += new EventHandler(dtp_ValueChanged);
      //   vvcb.KeyDown += new KeyEventHandler(dtp_KeyDown_DeleteKeyEmptiesDTP);
      //}

      //else if(thisControl is CheckBox)
      //{
      //   CheckBox cb = thisControl as CheckBox;
      //}
   }

   // ovo je kad se scrol pojavi da sume "pobjegnu" lijevo ....
   void grid_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
   {
      VvDataGridView theGrid    = sender as VvDataGridView;
      DataGridView   theSumGrid = theGrid.TheLinkedGrid_Sum;

      int sumNalog = theGrid.RowHeadersWidth;
      int sumSum   = theSumGrid.RowHeadersWidth /*+ 1*/;

      for(int i = 0; i < theGrid.Columns.Count; i++)
      {
         sumNalog += theGrid.Columns[i].Width;
      }

      for(int i = 0; i < theSumGrid.Columns.Count; i++)
      {
         sumSum += theSumGrid.Columns[i].Width;
      }

      if((sumSum > sumNalog) & (theSumGrid.Width == theGrid.Width))
      {
         theSumGrid.Width = theGrid.Width - (sumSum - sumNalog);
      }
      // Tamara, ovaj dole else ti je bas cool. TODO: 
      else if((sumSum == sumNalog) & theSumGrid.Width < theGrid.Width)
      {
         theGrid.Width = theGrid.Width;
         theSumGrid.Width = theSumGrid.Width;
      }
      else
      {
         theSumGrid.Width = theGrid.Width;
      }
   }

   void grid_ColumnWidthChanged_NewColWidth(object sender, DataGridViewColumnEventArgs e)
   {
      //VvDataGridView theGrid = sender as VvDataGridView;
      
      //int newColWidth = 0;

      //foreach(DataGridViewColumn col in theGrid.Columns)
      //{
      //   if(col.AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
      //   {
      //    //  this.PutColWidth();
      //      newColWidth = col.Width;
      //      col.Width = col.Width;
      //   }
      //}
   }

   protected void grid_ColumnWidthChanged_SumGrid(object sender, DataGridViewColumnEventArgs e)
   {
      VvDataGridView theGrid    = sender as VvDataGridView;
      DataGridView   theSumGrid = theGrid.TheLinkedGrid_Sum;

      for(int i = 0; i < theGrid.Columns.Count; i++)
      {
         theSumGrid.Columns[i].Width = theGrid.Columns[i].Width;
      }
   }

   protected void grid_ColumnWidthChanged_ChBoxGrid(object sender, DataGridViewColumnEventArgs e)
   {
      VvDataGridView theGrid     = sender as VvDataGridView;
      DataGridView   theColCGrid = theGrid.TheLinkedGrid_ColC;

      for(int i = 0; i < theGrid.Columns.Count; i++)
      {
         theColCGrid.Columns[i].Width = theGrid.Columns[i].Width;
      }
   }

   public static void grid_Validating(object sender, System.ComponentModel.CancelEventArgs e)
   {
      // 25.3.2015: 
      if(ZXC.TheVvForm.TheVvTabPage == null) return;
      // 10.3.2011: 
      if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None ||
         ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.Delete) return;

      VvDataGridView dgv = sender as VvDataGridView;

      // 03.02.2012: 
      if(dgv.Visible == false) return;

      foreach(DataGridViewColumn column in dgv.Columns)
      {
         if(column is VvTextBoxColumn && ((VvTextBox)column.Tag).JAM_DataRequired == true)
         {
            foreach(DataGridViewRow row in dgv.Rows)
            {
               if(row.IsNewRow) break;

               if(dgv.GetStringCell(column.Index, row.Index, false).NotEmpty()) continue;

               ZXC.RaiseErrorProvider(dgv, string.Format("Redak [{0}] podatak [{1}] ne moze biti prazan!", row.Index + 1, column.HeaderText));

               e.Cancel = true;
            }
         }
      }
   }

   public static void grid_CellFormatting_FormatVvDateTime(object sender, DataGridViewCellFormattingEventArgs e)
   {
      DataGridView dgv = sender as DataGridView;
      DataGridViewColumn column = dgv.Columns[e.ColumnIndex];

      if(column.ValueType == typeof(DateTime) && e.Value != null)
      {
         DateTime dateTime = ZXC.ValOr_01010001_DateTime(e.Value.ToString());

         if(dateTime == DateTime.MinValue) e.CellStyle.Format = "' '";
         else
         {
            VvTextBox vvtb = column.Tag as VvTextBox;

                 if(vvtb != null && vvtb.JAM_IsForDateTimePicker_WithTimeDisplay) e.CellStyle.Format = ZXC.VvDateAndTimeFormat;
            else if(vvtb != null && vvtb.JAM_IsForDateTimePicker_TimeOnlyDisplay) e.CellStyle.Format = ZXC.VvTimeOnlyFormat;
            else                                                                  e.CellStyle.Format = ZXC.VvDateFormat;
         }

         //#region Frigoterm Debugging

         //bool frigoDebug = ((ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.FRIGOT || 
         //                    ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.VIPER) && ZXC.vvDB_User == "superuser");

         //if(frigoDebug) 
         //   ZXC.aim_emsg("e.Value {0}\ndateTime {1}\ne.CellStyle.Format {2}\nisMinDate {3}\n dateTime {4}\nminValue {5}", 
         //                 e.Value,     dateTime,     e.CellStyle.Format, (dateTime == DateTime.MinValue), 
         //                 dateTime.ToLongDateString(), DateTime.MinValue.ToLongDateString());

         //#endregion Frigoterm Debugging

      }

   }

   /*protected*/ public void grid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
   {
      VvDataGridView theGrid = sender as VvDataGridView;
      bool shouldDirtyFlagging;

      if(TheVvTabPage.TheVvForm.VvFlag_AllowGridAddOrDeleteRowsIsChanging == true)
         shouldDirtyFlagging = false;
      else
         shouldDirtyFlagging = true;

      TheVvTabPage.TheVvForm.VvFlag_RowsAreAddingOrDeletingOrBoth = true;

      RenumerateLineNumbers(theGrid, e.RowIndex - 1);

      if(shouldDirtyFlagging) ZXC.TheVvForm.SetDirtyFlag("grid_RowsAdded row.idx = " + e.RowIndex.ToString() + " ...pa onda RenumerateLineNumbersFrom");

      ZXC.TheVvForm.VvFlag_RowsAreAddingOrDeletingOrBoth = false;

      UpdateLineCount(theGrid);
   }

   /*protected*/ public void grid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
   {
      VvDataGridView theGrid = sender as VvDataGridView;
      bool shouldDirtyFlagging;

      if(TheVvTabPage.TheVvForm.VvFlag_AllowGridAddOrDeleteRowsIsChanging == true)
         shouldDirtyFlagging = false;
      else
         shouldDirtyFlagging = true;

      TheVvTabPage.TheVvForm.VvFlag_RowsAreAddingOrDeletingOrBoth = true;

      RenumerateLineNumbers(theGrid, e.RowIndex);

      if(shouldDirtyFlagging) ZXC.TheVvForm.SetDirtyFlag("grid_RowsRemoved" + " ...pa onda RenumerateLineNumbersFrom");

      // 15.04.2013: preselio par cm nize 
      //ZXC.TheVvForm.VvFlag_RowsAreAddingOrDeletingOrBoth = false;

      UpdateLineCount(theGrid);

      // ovaj if dodan 5.12.2010 mozda bude bug? bez if-a smo u PutDgvTransSumFields isli odavdje pri otvaranju nekog novog DUC-a (nepotrebno?!) sto je stvaralo neke probleme...
      if(TheVvTabPage.TheVvForm.VvFlag_AllowGridAddOrDeleteRowsIsChanging == false)
      {
         PutDgvTransSumFields();

         if(this is IVvHasSumInDataLayerDocumentRecordUC)
         {
            ((IVvHasSumInDataLayerDocumentRecordUC)this).PutTransSumToDocumentSumFields();
         }
      }
      // 15.04.2013: 
      ZXC.TheVvForm.VvFlag_RowsAreAddingOrDeletingOrBoth = false;
   }

   private void grid_Sorted(object sender, EventArgs e)
   {
      VvDataGridView theGrid = sender as VvDataGridView;
      bool shouldDirtyFlagging;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None)
         shouldDirtyFlagging = false;
      else
         shouldDirtyFlagging = true;

      TheVvTabPage.TheVvForm.VvFlag_RowsAreAddingOrDeletingOrBoth = true;

      RenumerateLineNumbers(theGrid, 0);

      if(shouldDirtyFlagging) ZXC.TheVvForm.SetDirtyFlag("grid_sorted" + " ...pa onda RenumerateLineNumbersFrom");
   }

   public static void RenumerateLineNumbers(DataGridView dgv)
   {
      RenumerateLineNumbers(dgv, 0);
   }

   public static void RenumerateLineNumbers(DataGridView dgv, int fromRowIndex)
   {
      if(fromRowIndex == -1) fromRowIndex = 0; // malo tu trkeljimo ali zasada tako. 
      if(fromRowIndex < 0) return;

      for(int rowIndex = fromRowIndex; rowIndex < dgv.Rows.Count; ++rowIndex)
      {
         if(!dgv.Rows[rowIndex].IsNewRow) dgv.Rows[rowIndex].HeaderCell.Value = (rowIndex + 1).ToString();
      }
   }

   public /*protected*/ void UpdateLineCount(VvDataGridView theGrid)
   {
      DataGridView theSumGrid = theGrid.TheLinkedGrid_Sum;

      int idxCorrector;

      if(theGrid.AllowUserToAddRows == false) idxCorrector = 0;
      else idxCorrector = 1;

      theSumGrid.Rows[0].HeaderCell.Value = (theGrid.RowCount - idxCorrector).ToString();
   }

   private void grid_CellEnter_CopyPrevRowValue(object sender, DataGridViewCellEventArgs e)
   {
      VvDataGridView dgv = sender as VvDataGridView;
      if (dgv == null) return;

      if (ZXC.TheVvForm?.TheVvTabPage?.WriteMode == ZXC.WriteMode.None) return;

      if (e.ColumnIndex < 0 || e.ColumnIndex >= dgv.Columns.Count) return;

      VvTextBox vvtb = dgv.Columns[e.ColumnIndex]?.Tag as VvTextBox;

      if (vvtb != null && (vvtb.JAM_ShouldCopyPrevRow || vvtb.JAM_ShouldCopyPrevRowUnCond))
      {
         int currRow = e.RowIndex;
         int currCol = e.ColumnIndex;

         if (currRow < 1) return;

         if (vvtb.JAM_ShouldCopyPrevRowUnCond == false &&
            ((currRow != dgv.RowCount - 2)) &&
            (currRow != dgv.RowCount - 1 && currCol != 2))
            return;

         object prevValueAsObject = dgv.GetCellValueAsObject(currCol, currRow - 1, false);
         object currValueAsObject = dgv.GetCellValueAsObject(currCol, currRow, false);

         string currValueStr = currValueAsObject?.ToString() ?? "";

         // Fixed condition - all checks properly grouped
         if (prevValueAsObject != null &&
            (currValueAsObject == null ||
             currValueStr.IsEmpty() ||
             currValueStr == "0" ||
             currValueStr == "01.01.0001. 0:00:00"))
         {
            dgv.PutCell(currCol, currRow, prevValueAsObject);
            dgv.BeginEdit(true);
            originalText = "";
         }
      }
   }
   private void grid_CellEnter_SetStatusText(object sender, DataGridViewCellEventArgs e)
   {
      DataGridView dgv = sender as DataGridView;

      // Faza 1d / C14: WriteMode preko C12-settable TheVvTabPage (u Fazi 3
      // respektira host-specific tab); status text kroz ZXC.StatusTextPusher.
      if(this.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvTextBox vvtb = dgv.Columns[e.ColumnIndex].Tag as VvTextBox;

      if(vvtb != null && vvtb.JAM_StatusText.NotEmpty())
      {
         if(ZXC.StatusTextPusher != null) { ZXC.StatusTextPusher(vvtb.JAM_StatusText); return; }

         ZXC.TheVvForm.statusTextBackup       = ZXC.TheVvForm.TStripStatusLabel.Text;
         ZXC.TheVvForm.TStripStatusLabel.Text = vvtb.JAM_StatusText;
      }
   }

   private void grid_CellLeave_RestoreStatusText(object sender, DataGridViewCellEventArgs e)
   {
      DataGridView dgv = sender as DataGridView;

      // Faza 1d / C14: isto kao gore — TheVvTabPage kroz C12 setter; pop kroz ZXC.StatusTextPopper.
      if(this.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvTextBox vvtb = dgv.Columns[e.ColumnIndex].Tag as VvTextBox;

      if(vvtb != null && vvtb.JAM_StatusText.NotEmpty())
      {
         if(ZXC.StatusTextPopper != null) { ZXC.StatusTextPopper(); return; }

       //17.05.2019. da ne skace
       //                                           ZXC.TheVvForm.TStripStatusLabel.Text = ZXC.TheVvForm.statusTextBackup;
        if(ZXC.TheVvForm.statusTextBackup.IsEmpty()) ZXC.TheVvForm.TStripStatusLabel.Text = ZXC.TheVvForm.statusTextBackup = "...";
        else                                         ZXC.TheVvForm.TStripStatusLabel.Text = ZXC.TheVvForm.statusTextBackup        ;
      }
   }

   #endregion Event Handlers

   #region DragAndDrop Row Reordering

   private Rectangle dragBoxFromMouseDown;
   private int rowIndexFromMouseDown;
   private int rowIndexOfItemUnderMouseToDrop;

   private void grid_MouseMove(object sender, MouseEventArgs e)
   {
      VvDataGridView theGrid = sender as VvDataGridView;
      
      if((e.Button & MouseButtons.Left) == MouseButtons.Left)
      {
         // 21.10.2011: 
         if(this.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

         // If the mouse moves outside the rectangle, start the drag.
         if(dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y) && rowIndexFromMouseDown < theGrid.Rows.Count)
         {
            // Proceed with the drag and drop, passing in the list item.                    
            DragDropEffects dropEffect = theGrid.DoDragDrop(theGrid.Rows[rowIndexFromMouseDown], DragDropEffects.Move | DragDropEffects.Copy);
         }
      }
   }

   private void grid_MouseDown(object sender, MouseEventArgs e)
   {
      VvDataGridView theGrid = sender as VvDataGridView;

      // 21.10.2011: 
      if(this.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      // Get the index of the item the mouse is below.
      rowIndexFromMouseDown = theGrid.HitTest(e.X, e.Y).RowIndex;

      // 21.10.2011: 
    //if(rowIndexFromMouseDown != -1)
      if(rowIndexFromMouseDown != -1 && theGrid.HitTest(e.X, e.Y).ColumnIndex.IsNegative()) // dakle, samo na onoj prvoj 'rbr' koloni trzamo na DragAndDrop (21.10.2011) 
                                                                                            // OVO JE ISPARAVAK VEEEELIKOG BUG-a kod odabira LookUpItem-a na Grid-u bi gubio retke prije ovoga.  
      {
         // Remember the point where the mouse down occurred. 
         // The DragSize indicates the size that the mouse can move 
         // before a drag event should be started.                
         Size dragSize = SystemInformation.DragSize;

         // Create a rectangle using the DragSize, with the mouse position being
         // at the center of the rectangle.
         dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
      }
      else
         // Reset the rectangle if the mouse is not over an item in the ListBox.
         dragBoxFromMouseDown = Rectangle.Empty;
   }

   private void grid_DragOver(object sender, DragEventArgs e)
   {
      if((e.KeyState & 8) == 8 && (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
      {
         // CTL KeyState for copy.
         e.Effect = DragDropEffects.Copy;
      }
      //if((e.KeyState & 32) == 32 && (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
      //{
      //   // ALT KeyState for link.
      //   e.Effect = DragDropEffects.Copy;
      //}
      else
      {
         e.Effect = DragDropEffects.Move;
      }
   }

   private void grid_DragDrop(object sender, DragEventArgs e)
   {
      VvDataGridView theGrid = sender as VvDataGridView;

      // The mouse locations are relative to the screen, so they must be 
      // converted to client coordinates.
      Point clientPoint = theGrid.PointToClient(new Point(e.X, e.Y));

      // Get the row index of the item the mouse is below. 
      rowIndexOfItemUnderMouseToDrop = theGrid.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

      // If the drag operation was a move then remove and insert the row.
      if(e.Effect == DragDropEffects.Move)
      {
         DataGridViewRow rowToMove = e.Data.GetData(typeof(DataGridViewRow)) as DataGridViewRow;

         // qukatz dodao ovaj if() da mozes insertati new row-ove sos dregendrop 
         if(rowIndexFromMouseDown != theGrid.RowCount - 1)
         {
            theGrid.Rows.RemoveAt(rowIndexFromMouseDown);

            /* Q: */
            if(rowIndexFromMouseDown < rowIndexOfItemUnderMouseToDrop) rowIndexOfItemUnderMouseToDrop--;

            if(rowIndexOfItemUnderMouseToDrop >= 0) // if dodan 27.9.2010, prije toga je skako exception, neznam kako nije i prije!? 
            {
               theGrid.Rows.Insert(rowIndexOfItemUnderMouseToDrop, rowToMove);

               GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(rowIndexOfItemUnderMouseToDrop);
            }

         }
         else
         {
            theGrid.Rows.Insert(rowIndexOfItemUnderMouseToDrop, 1);
         }

      }

      // Q: let's try to copy instead of move
      else if(e.Effect == DragDropEffects.Copy)
      {
         DataGridViewRow rowToCopy = e.Data.GetData(
          typeof(DataGridViewRow)) as DataGridViewRow;

         // ovo nece pa nece 
         //TheG.Rows.Insert(rowIndexOfItemUnderMouseToDrop, rowToCopy);

         // 21.10.2011: 
         if(rowIndexOfItemUnderMouseToDrop.IsNegative()) return;

         theGrid.Rows.Insert(rowIndexOfItemUnderMouseToDrop, 1);

         if(rowIndexFromMouseDown >= rowIndexOfItemUnderMouseToDrop) rowIndexFromMouseDown++;

         foreach(DataGridViewCell cell in theGrid.Rows[rowIndexFromMouseDown].Cells)
         {
            if(cell.Visible)
            {
               theGrid.Rows[rowIndexOfItemUnderMouseToDrop].Cells[cell.ColumnIndex].Value = cell.Value;
            }
         }

         GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(rowIndexOfItemUnderMouseToDrop);

      } // else if(e.Effect == DragDropEffects.Copy)

   }

   #endregion DragAndDrop Row Reordering

 //public void TakeGridRowsFromOtherDocument(uint dokNum)
   public void TakeGridRowsFromOtherDocument(string tt, uint ttNum, uint dokNum)
   {
      VvDocumentRecord vvDocumentRecord = (VvDocumentRecord)VirtualDataRecord.VvDataRecordFactory();

      bool OK;

      if(ZXC.ThisIsVektorProject)
      {
         OK = vvDocumentRecord.VvDao.SetMe_VvDocumentRecord_byTtAndTtNum(TheDbConnection, vvDocumentRecord, tt, ttNum, false, false);
      }
      else
      {
         OK = vvDocumentRecord.VvDao.SetMe_Record_bySomeUniqueColumn(TheDbConnection, vvDocumentRecord, dokNum, ZXC.TheVvForm.GetCommonDrSchema_DocumentRecord_DokNum(), false);
      }

      if(!OK) return;

      // -------- Here We Go! --------- 

      int rowIdx, idxCorrector;

      vvDocumentRecord.VvDao.LoadTranses(TheDbConnection, vvDocumentRecord, false);

      //TheG.RowsRemoved -= new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);
      //TheG.RowsAdded   -= new DataGridViewRowsAddedEventHandler  (grid_RowsAdded);

      idxCorrector = GetDGVsIdxCorrrector(TheG);

      List<VvTransRecord> transes = vvDocumentRecord.VirtualTranses;
      VvDataGridView      theG    = TheG;

      if(vvDocumentRecord is VvPolyDocumRecord)
      {
         VvPolyDocumRecordUC thePolyDUC    = (VvPolyDocumRecordUC)this;
         VvPolyDocumRecord   thePolyDocRec = (VvPolyDocumRecord)vvDocumentRecord;

         if(thePolyDUC.ThePolyGridTabControl.SelectedTab.Title == thePolyDUC.TabPageTitle1)
         {
            transes = thePolyDocRec.VirtualTranses;
            theG    = thePolyDUC.TheG;
         }
         else if(thePolyDUC.ThePolyGridTabControl.SelectedTab.Title == thePolyDUC.TabPageTitle2)
         {
            transes = thePolyDocRec.VirtualTranses2;
            theG    = thePolyDUC.TheG2;
         }
         else if(thePolyDUC.ThePolyGridTabControl.SelectedTab.Title == thePolyDUC.TabPageTitle3)
         {
            transes = thePolyDocRec.VirtualTranses3;
            theG    = thePolyDUC.TheG3;
         }
      }

      if(transes == null) return;

      ArtStat artStat_rec;

      foreach(VvTransRecord trans_rec in transes)
      {
         theG.Rows.Add();

         rowIdx = theG.RowCount - idxCorrector;

         // 12.03.2020: start 
         if((this is IZD_SVD_DUC || this is ZAH_SVD_DUC) && (trans_rec as Rtrans).T_TT == Faktur.TT_URA)
         {
            artStat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, (trans_rec as Rtrans));

            (trans_rec as Rtrans).T_cij = artStat_rec.PrNabCij; 
         }
         // 12.03.2020:  end  

         PutDgvLineFields(trans_rec, rowIdx, true);
         GetDgvLineFields(rowIdx, false, null); // da napuni Document's business.Transes 
      }

      //TheG.RowsAdded   += new DataGridViewRowsAddedEventHandler  (grid_RowsAdded);
      //TheG.RowsRemoved += new DataGridViewRowsRemovedEventHandler(grid_RowsRemoved);

      RenumerateLineNumbers(TheG, 0);
      UpdateLineCount(TheG);

      PutDgvTransSumFields();
   }

   #endregion TheG     
   
   #region DataGridViewSum

   protected VvDataGridView CreateSumGrid(VvDataGridView theGrid, Control _parent, string _name)
   {
      VvDataGridView _theSumGrid = new VvDataGridView();

      theGrid.TheLinkedGrid_Sum = _theSumGrid;

      _theSumGrid.Parent = _parent;
      _theSumGrid.Name   = _name;


      _theSumGrid.Height = ZXC.QUN + ZXC.Qun10; //23;

      _theSumGrid.BorderStyle = BorderStyle.FixedSingle;
      VvHamper.ApplyVVColorAndStyleTabCntrolChange(_theSumGrid);
      // _theSumGrid.BackgroundColor      = SystemColors.ControlLight;
      _theSumGrid.ColumnHeadersVisible = false;
      _theSumGrid.CellBorderStyle      = DataGridViewCellBorderStyle.SingleVertical;// DataGridViewCellBorderStyle.None;
      _theSumGrid.ReadOnly             = true;
      _theSumGrid.Tag                  = theGrid;

      _theSumGrid.AllowUserToAddRows       =
      _theSumGrid.AllowUserToDeleteRows    =
      _theSumGrid.AllowUserToOrderColumns  =
      _theSumGrid.AllowUserToResizeColumns =
      _theSumGrid.AllowUserToResizeRows    = false;

      _theSumGrid.RowHeadersDefaultCellStyle.Alignment = theGrid.RowHeadersDefaultCellStyle.Alignment;
      _theSumGrid.RowTemplate.Height                   = theGrid.RowTemplate.Height;

      //_theSumGrid.ScrollBars = ScrollBars.None;

      _theSumGrid.Resize         += new EventHandler(_theSumGrid_Resize);
      _theSumGrid.VisibleChanged += new EventHandler(_theSumGrid_Resize);

      _theSumGrid.ScrollBars = ScrollBars.None;

      return _theSumGrid;
   }

   protected void InitializeTheSUMGrid_Columns(VvDataGridView theGrid)
   {
      DataGridView theSumGrid = theGrid.TheLinkedGrid_Sum;

      DataGridViewTextBoxColumn gridSumColumn;

      foreach(DataGridViewColumn mainGridColumn in theGrid.Columns)
      {
         gridSumColumn = new DataGridViewTextBoxColumn();

         //gridSumColumn.Name                       = "SUM" + mainGridColumn.Name.TrimStart(new char[] { 't' });
         gridSumColumn.Name                       = mainGridColumn.Name;
         gridSumColumn.DefaultCellStyle.Alignment = mainGridColumn.DefaultCellStyle.Alignment;
         gridSumColumn.AutoSizeMode               = mainGridColumn.AutoSizeMode;
         theSumGrid.AutoGenerateColumns           = false;
         gridSumColumn.Width                      = mainGridColumn.Width;
         gridSumColumn.ValueType                  = mainGridColumn.ValueType;
         gridSumColumn.Visible                    = mainGridColumn.Visible;
         gridSumColumn.Tag                        = mainGridColumn.Tag;
         gridSumColumn.DefaultCellStyle.Format    = mainGridColumn.DefaultCellStyle.Format;
         gridSumColumn.DefaultCellStyle.BackColor = mainGridColumn.DefaultCellStyle.BackColor;

         if(mainGridColumn.AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
         {
            gridSumColumn.MinimumWidth = mainGridColumn.MinimumWidth;
         }
         theSumGrid.Columns.Add(gridSumColumn);

       //  gridSumColumn.Frozen = mainGridColumn.Frozen; zaje... nemerem dokuciti zakaj

      }

      theSumGrid.RowHeadersWidth = theGrid.RowHeadersWidth;
      
      theSumGrid.RowCount = 1;
      
     
      // micanje tamnoplavog polja iz datagrida 
      //
            theSumGrid.TabStop = false;
            theSumGrid.ClearSelection();
      //
      //                                         

   }

   private void _theSumGrid_Resize(object sender, EventArgs e)
   {
      DataGridView theSumGrid = sender as DataGridView;

      SetTheSumGridHeight_HScrollBarVisible(theSumGrid);
   }

   #endregion DataGridViewSum

   #region DataGridCheckBox

   protected DataGridView CreateColChooserGrid(VvDataGridView theGrid, Control _parent, string _name)
   {
      DataGridView _theColChooserGrid = new DataGridView();

      theGrid.TheLinkedGrid_ColC = _theColChooserGrid;

      _theColChooserGrid.Parent = _parent;
      _theColChooserGrid.Name   = _name;

      _theColChooserGrid.Height = ZXC.Q2un + ZXC.Qun4 - ZXC.Qun10; 

      _theColChooserGrid.BorderStyle = BorderStyle.FixedSingle;
      VvHamper.ApplyVVColorAndStyleTabCntrolChange(_theColChooserGrid);
      _theColChooserGrid.ColumnHeadersVisible = false;
      _theColChooserGrid.CellBorderStyle      = DataGridViewCellBorderStyle.Single;
      
      _theColChooserGrid.AllowUserToAddRows       =
      _theColChooserGrid.AllowUserToDeleteRows    =
      _theColChooserGrid.AllowUserToOrderColumns  =
      _theColChooserGrid.AllowUserToResizeColumns =
      _theColChooserGrid.AllowUserToResizeRows    = false;

      _theColChooserGrid.Tag     = "CheckBox";
      _theColChooserGrid.Visible = false;

      _theColChooserGrid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
      _theColChooserGrid.RowHeadersDefaultCellStyle.Font      = ZXC.vvFont.SmallSmallFont;

      _theColChooserGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
      _theColChooserGrid.RowsDefaultCellStyle.SelectionBackColor = ZXC.vvColors.vvTBoxReadOnly_False_BackColor;

      _theColChooserGrid.ScrollBars = ScrollBars.None;
      
      return _theColChooserGrid;

   }

   public /*protected*/ void TheColChooserGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
   {
      if(TheVvTabPage != null /*&& TheVvTabPage.WriteMode != ZXC.WriteMode.None*/)
         GetFields_TheColChooserGrid();
   }

   private void GetFields_TheColChooserGrid()
   {
      string colName;
      bool   visibleInRed, visibleInBlue;


      TheVvTabPage.TheVvForm.VvPref.placaDUC.ColChooserStates.Clear();

      foreach(DataGridViewColumn col in TheColChooserGrid.Columns)
      {
         DataGridViewCell redCell  = TheColChooserGrid[col.Index, 0];
         DataGridViewCell blueCell = TheColChooserGrid[col.Index, 1];

         colName       = col.Name;
         visibleInRed  = (bool)redCell .Value;
         visibleInBlue = (bool)blueCell.Value;

         TheVvTabPage.TheVvForm.VvPref.placaDUC.ColChooserStates.Add(new VvPref.VVColChooserStates(colName, visibleInRed, visibleInBlue));
      }

   }

   protected void InitializeTheChBoxGrid_Columns(VvDataGridView theGrid)
   {
      DataGridView theColCGrid = theGrid.TheLinkedGrid_ColC;

      DataGridViewCheckBoxColumn gridChBoxColumn;

      foreach(DataGridViewColumn mainGridColumn in theGrid.Columns)
      {
         if(mainGridColumn.Tag != null && mainGridColumn.Tag.ToString() == VvDocumentRecordUC.AlwaysInvinsibleStr) continue;

         gridChBoxColumn = new DataGridViewCheckBoxColumn();

         //gridChBoxColumn.Name                       = "ChBox" + mainGridColumn.Name.TrimStart(new char[] { 't' });
         gridChBoxColumn.Name                       = mainGridColumn.Name;
         gridChBoxColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
         gridChBoxColumn.AutoSizeMode               = mainGridColumn.AutoSizeMode;
         gridChBoxColumn.Width                      = mainGridColumn.Width;
         gridChBoxColumn.Visible                    = mainGridColumn.Visible;
         gridChBoxColumn.Tag                        = mainGridColumn.Tag;

         theColCGrid.Columns.Add(gridChBoxColumn);

         gridChBoxColumn.Frozen                     = mainGridColumn.Frozen;
      }

      theColCGrid.RowHeadersWidth = theGrid.RowHeadersWidth;
      theColCGrid.RowCount        = 2;
      theColCGrid.Rows[0].HeaderCell.Value = "Red";
      theColCGrid.Rows[1].HeaderCell.Value = "Blue";

      theColCGrid.Rows[0].DefaultCellStyle.BackColor = ZXC.vvColors.dataGridColumnHeaders_BackColor_Red;
      theColCGrid.Rows[1].DefaultCellStyle.BackColor = ZXC.vvColors.dataGridColumnHeaders_BackColor_Blue;
   }

   #endregion DataGridCheckBox

   #region DataGridCheckBox_VisibleColumns_1Row

   protected DataGridView CreateColChooserGrid_1(VvDataGridView theGrid, Control _parent, string _name)
   {
      DataGridView _theColChooserGrid = new DataGridView();

      theGrid.TheLinkedGrid_ColC = _theColChooserGrid;

      _theColChooserGrid.Parent = _parent;
      _theColChooserGrid.Name   = _name;

      _theColChooserGrid.Height = ZXC.QUN + ZXC.Qun10; 

      _theColChooserGrid.BorderStyle = BorderStyle.FixedSingle;
      VvHamper.ApplyVVColorAndStyleTabCntrolChange(_theColChooserGrid);
      _theColChooserGrid.ColumnHeadersVisible = false;
      _theColChooserGrid.CellBorderStyle      = DataGridViewCellBorderStyle.Single;
      
      _theColChooserGrid.AllowUserToAddRows       =
      _theColChooserGrid.AllowUserToDeleteRows    =
      _theColChooserGrid.AllowUserToOrderColumns  =
      _theColChooserGrid.AllowUserToResizeColumns =
      _theColChooserGrid.AllowUserToResizeRows    = false;

      _theColChooserGrid.Tag     = "CheckBox";
      _theColChooserGrid.Visible = false;

      _theColChooserGrid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
      _theColChooserGrid.RowHeadersDefaultCellStyle.Font      = ZXC.vvFont.SmallSmallFont;

      _theColChooserGrid.AutoSizeRowsMode                        = DataGridViewAutoSizeRowsMode.AllCells;
      _theColChooserGrid.RowsDefaultCellStyle.SelectionBackColor = ZXC.vvColors.vvTBoxReadOnly_False_BackColor;

      _theColChooserGrid.ScrollBars = ScrollBars.None;
      
      return _theColChooserGrid;

   }

   //public /*protected*/ void TheColChooserGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
   //{
   //   if(TheVvTabPage != null /*&& TheVvTabPage.WriteMode != ZXC.WriteMode.None*/)
   //      GetFields_TheColChooserGrid();
   //}

   //private void GetFields_TheColChooserGrid()
   //{
   //   string colName;
   //   bool   visibleInRed, visibleInBlue;

   //   TheVvTabPage.TheVvForm.VvPref.placaDUC.ColChooserStates.Clear();

   //   foreach(DataGridViewColumn col in TheColChooserGrid.Columns)
   //   {
   //      DataGridViewCell redCell  = TheColChooserGrid[col.Index, 0];
   //      DataGridViewCell blueCell = TheColChooserGrid[col.Index, 1];

   //      colName       = col.Name;
   //      visibleInRed  = (bool)redCell .Value;
   //      visibleInBlue = (bool)blueCell.Value;

   //      TheVvTabPage.TheVvForm.VvPref.placaDUC.ColChooserStates.Add(new VvPref.VVColChooserStates(colName, visibleInRed, visibleInBlue));
   //   }

   //}

   protected void InitializeTheChBoxGrid_Columns1(VvDataGridView theGrid)
   {
      DataGridView theColCGrid = theGrid.TheLinkedGrid_ColC;

      DataGridViewCheckBoxColumn gridChBoxColumn;

      foreach(DataGridViewColumn mainGridColumn in theGrid.Columns)
      {
         if(mainGridColumn.Tag != null && mainGridColumn.Tag.ToString() == VvDocumentRecordUC.AlwaysInvinsibleStr) continue;

         gridChBoxColumn = new DataGridViewCheckBoxColumn();

         //gridChBoxColumn.Name                       = "ChBox" + mainGridColumn.Name.TrimStart(new char[] { 't' });
         gridChBoxColumn.Name                       = mainGridColumn.Name;
         gridChBoxColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
         gridChBoxColumn.AutoSizeMode               = mainGridColumn.AutoSizeMode;
         gridChBoxColumn.Width                      = mainGridColumn.Width;
         gridChBoxColumn.Visible                    = mainGridColumn.Visible;
         gridChBoxColumn.Tag                        = mainGridColumn.Tag;

         theColCGrid.Columns.Add(gridChBoxColumn);

      }

      theColCGrid.RowHeadersWidth = theGrid.RowHeadersWidth;
      theColCGrid.RowCount        = 1;
      theColCGrid.Rows[0].HeaderCell.Value = "Kol";
   }

   protected void InitializeChooserGrid_Columns_Faktur(VvDataGridView theGrid)
   {
      DataGridView theColCGrid = theGrid.TheLinkedGrid_ColC;

      DataGridViewCheckBoxColumn gridChBoxColumn;
      DataGridViewTextBoxColumn  gridColumn;

      foreach(DataGridViewColumn mainGridColumn in theGrid.Columns)
      {
         if(mainGridColumn.Tag != null && mainGridColumn.Tag.ToString() == VvDocumentRecordUC.AlwaysInvinsibleStr) continue;

         gridChBoxColumn = new DataGridViewCheckBoxColumn();
         gridColumn      = new DataGridViewTextBoxColumn();

         gridChBoxColumn.Name                       = mainGridColumn.Name;
         gridChBoxColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
         gridChBoxColumn.AutoSizeMode               = mainGridColumn.AutoSizeMode;
         gridChBoxColumn.Width                      = mainGridColumn.Width;
         gridChBoxColumn.Visible                    = mainGridColumn.Visible;
         gridChBoxColumn.Tag                        = mainGridColumn.Tag;

         theColCGrid.Columns.Add(gridChBoxColumn);

         if(mainGridColumn.ValueType == typeof(decimal) && mainGridColumn.Name.StartsWith("t") && mainGridColumn.Name != "t_psvSt")
         {
            gridColumn.Name         = mainGridColumn.Name;
            gridColumn.AutoSizeMode = mainGridColumn.AutoSizeMode;
            gridColumn.Width        = ZXC.QUN;
            gridColumn.Visible      = mainGridColumn.Visible;
            gridColumn.Tag          = mainGridColumn.Tag;
            gridColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridColumn.DefaultCellStyle.BackColor = Color.AliceBlue;

            theColCGrid.Columns.Add(gridColumn);
            gridChBoxColumn.Width = mainGridColumn.Width - gridColumn.Width;
         }
      }

      theColCGrid.RowHeadersWidth = theGrid.RowHeadersWidth;
      theColCGrid.RowCount        = 1;
      theColCGrid.Rows[0].HeaderCell.Value = "";
   }

   #endregion DataGridCheckBox_VisibleColumns_1Row

   #region CalcLocationSizeAnchor_Grids

   protected void CalcLocationSizeAnchor_TheDGV(VvDataGridView theGrid, int nextX, int nextY, int minWidth)
   {
      DataGridView theSumGrid = theGrid.TheLinkedGrid_Sum;

      // ovdje mora doci _theGrid.Parent size i anchor jerbo je od 25.3.2008. _theGrid na vvInnerTabPage-u koji ima samo anchor top-left

      //24.09.2009. Qukatz vozi mali motorin
      //            theGrid1.Parent.Size = new Size(this.Width, this.Height - nextY - 3 * ZXC.QUN); nema smisla -nextY jerbo od this-theGrid1.Parent razliciti su putevi i stupnjevi
      //            nextY ima smisla za lokaciju i visinu grida    
      // zapravo nemam pojma kaj vise ima smisla - visina u svakom slucaju dobro sljaka ali ne kuzim kak

      theGrid.Parent.Size   = new Size(this.Width, this.Height/* - 3 * ZXC.QUN*/);
      theGrid.Parent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
      theGrid.Location      = new Point(ZXC.QunMrgn, nextY + ZXC.QunMrgn);

      // ali opet zeka kad je VvForm maloga formata pa gridovi dobiju svoje scrolove
      // minWidth - ovdje saljem za NalogDuc hamp_zaglavlje.Width - nevalja
      //T ovdje ipak treba _theGrid.width izracunat na temelju [irine njegovih kolona

      int sumGridHeight = theSumGrid.Height;
      if(theSumGrid.Visible == false) sumGridHeight = 0;

      if(theGrid.Parent.Width < minWidth)
         //theGrid.Size = new Size(minWidth - 2 * ZXC.QunMrgn, theGrid.Parent.Height - nextY - 2 * ZXC.QUN - theSumGrid.Height); // 17.05.2012. GFI_TSI nema sume pa je sumgrid.Visible = false
         theGrid.Size = new Size(minWidth - 2 * ZXC.QunMrgn, theGrid.Parent.Height - nextY - 2 * ZXC.QUN - sumGridHeight);
      else
         //theGrid.Size = new Size(theGrid.Parent.Width - 2 * ZXC.QunMrgn, theGrid.Parent.Height - nextY - 2 * ZXC.QUN - theSumGrid.Height);
         theGrid.Size = new Size(theGrid.Parent.Width - 2 * ZXC.QunMrgn, theGrid.Parent.Height - nextY - 2 * ZXC.QUN - sumGridHeight);
      
       theGrid.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

       theGrid.Scroll    += new ScrollEventHandler(theGrid_Scroll); // potrebno kad se ide tabulatorom kroz kolone
       theSumGrid.Scroll += new ScrollEventHandler(theSumGrid_Scroll);

    }

   public void CalcLocationSizeAnchor_TheDGV_ChoosGrid(VvDataGridView theGrid, int nextX, int nextY, VvHamper zaglHamper, bool isVisibleChooser)
   {
      DataGridView theSumGrid   = theGrid.TheLinkedGrid_Sum;
      DataGridView theColCGrid1 = theGrid.TheLinkedGrid_ColC;

      theGrid.Parent.Size   = new Size(this.Width, this.Height);
      theGrid.Parent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

      theColCGrid1.Location = new Point(nextX, nextY + ZXC.QunMrgn);

      if(isVisibleChooser) theGrid.Location = new Point(nextX, theColCGrid1.Bottom);
      else                 theGrid.Location = new Point(nextX, theColCGrid1.Top);

      if(theGrid.Parent.Width < zaglHamper.Width)
         theGrid.Size = new Size(zaglHamper.Width     - 2 * ZXC.QunMrgn, theGrid.Parent.Height - nextY - 2*ZXC.QUN - theSumGrid.Height);
      else                                                                                               
         theGrid.Size = new Size(theGrid.Parent.Width - 2 * ZXC.QunMrgn, theGrid.Parent.Height - nextY - 2*ZXC.QUN - theSumGrid.Height);
      
       theGrid     .Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
       theColCGrid1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

       theColCGrid1.Width = theGrid.Width;
       theSumGrid  .Width = theGrid.Width;

       SetTheSumGridHeight_HScrollBarVisible(theSumGrid);

       if(isVisibleChooser) theGrid.Height = theGrid.Parent.Height - 3*ZXC.QUN - theSumGrid.Height - theColCGrid1.Height - nextY;
       else                 theGrid.Height = theGrid.Parent.Height - 3*ZXC.QUN - theSumGrid.Height - nextY;


       theSumGrid.Location = new Point(theGrid.Location.X, theGrid.Bottom + ZXC.Qun12);
       theSumGrid.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

       theGrid.Scroll    += new ScrollEventHandler(theGrid_Scroll); // potrebno kad se ide tabulatorom kroz kolone
       theSumGrid.Scroll += new ScrollEventHandler(theSumGrid_Scroll);

   }

   protected void CalcLocationSizeAnchor_TheDGVAndTheSumGrid_WithChBoxGrid_NEW(VvDataGridView theGrid, int nextX, int nextY)
   {
      DataGridView theSumGrid  = theGrid.TheLinkedGrid_Sum;
      DataGridView theColCGrid = theGrid.TheLinkedGrid_ColC;

      theColCGrid.Location  = new Point(nextX, nextY);
      theGrid.Location      = new Point(nextX, theColCGrid.Top);

      SetTheSumGridHeight_HScrollBarVisible(theSumGrid);

      theGrid.Size       = new Size(theGrid.Parent.Width - 2*nextX, theGrid.Parent.Height - 2 * ZXC.QUN - theSumGrid.Height);
      
      theGrid.Anchor     = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      theColCGrid.Anchor =                       AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

      theColCGrid.Width = theGrid.Width;

      theSumGrid.Width    = theGrid.Width;
      theSumGrid.Location = new Point(theGrid.Location.X, theGrid.Bottom + ZXC.Qun12);
      theSumGrid.Anchor   = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

      theGrid.Scroll    += new ScrollEventHandler(theGrid_Scroll); // potrebno kad se ide tabulatorom kroz kolone
      theSumGrid.Scroll += new ScrollEventHandler(theSumGrid_Scroll);
   }

   protected void CalcLocationSizeAnchor_TheDGVAndTheSumGrid_NEW(VvDataGridView theGrid, int nextX, int nextY)
   {
      DataGridView theSumGrid  = theGrid.TheLinkedGrid_Sum;

      theGrid.Location      = new Point(nextX, nextY);

      SetTheSumGridHeight_HScrollBarVisible(theSumGrid);

      theGrid.Size       = new Size(theGrid.Parent.Width - 2*nextX, theGrid.Parent.Height - 2 * ZXC.QUN - theSumGrid.Height);
      theGrid.Anchor     = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

      theSumGrid.Width    = theGrid.Width;
      theSumGrid.Location = new Point(theGrid.Location.X, theGrid.Bottom + ZXC.Qun12);
      theSumGrid.Anchor   = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

      theGrid.Scroll    += new ScrollEventHandler(theGrid_Scroll); // potrebno kad se ide tabulatorom kroz kolone
      theSumGrid.Scroll += new ScrollEventHandler(theSumGrid_Scroll);
   }

  // new lookUpDeviza iynad dgva 06.12.2010.  i jos ispod
   protected void CalcLocationSizeAnchor_TheDGVAndTheSumGrid_NEW_WidthTbxBottomOfSumGrid(VvDataGridView theGrid, int _nextX, int _nextY, VvHamper hamperBelowDgv, VvHamper hamperAboveDgv)
   {
      DataGridView theSumGrid  = theGrid.TheLinkedGrid_Sum;
      int below;
      
      if(hamperBelowDgv == null) below = ZXC.Qun2;
      else                       below = hamperBelowDgv.Height;

      int heihgHamp = hamperAboveDgv.Height + below;
     
      hamperAboveDgv.Location = new Point(theGrid.Right - hamperAboveDgv.Width + ZXC.Qun4, _nextY);

      theGrid.Location   = new Point(_nextX, hamperAboveDgv.Bottom);
      theGrid.Size       = new Size(theGrid.Parent.Width - 2*_nextX, theGrid.Parent.Height - ZXC.QUN - theSumGrid.Height - heihgHamp);
      theGrid.Anchor     = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

      theSumGrid.Width    = theGrid.Width;
      SetTheSumGridHeight_HScrollBarVisible(theSumGrid);
      theGrid.Height      = theGrid.Parent.Height - ZXC.QUN - theSumGrid.Height - heihgHamp;

      theSumGrid.Location = new Point(theGrid.Location.X, theGrid.Bottom + ZXC.Qun12);
      theSumGrid.Anchor   = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

      theGrid.Scroll    += new ScrollEventHandler(theGrid_Scroll); // potrebno kad se ide tabulatorom kroz kolone
      theSumGrid.Scroll += new ScrollEventHandler(theSumGrid_Scroll);
     
      hamperAboveDgv.Location = new Point(theGrid.Right - hamperAboveDgv.Width + ZXC.Qun4, _nextY);
      hamperAboveDgv.Anchor   = AnchorStyles.Top | AnchorStyles.Right;

      hamperBelowDgv.Location = new Point(theSumGrid.Right - hamperBelowDgv.Width + ZXC.Qun4, theSumGrid.Bottom);
      hamperBelowDgv.Anchor   = AnchorStyles.Bottom | AnchorStyles.Right;
   }

   //DebitUC only
   protected void CalcLocationSizeAnchor_TheDGV_WidthTextBoxOnBottomOfDgv(VvDataGridView theGrid, int nextX, int nextY, int minWidth, int bottomHeight)
   {
      DataGridView theSumGrid = theGrid.TheLinkedGrid_Sum;

      // ovdje mora doci _theGrid.Parent size i anchor jerbo je od 25.3.2008. _theGrid na vvInnerTabPage-u koji 
      // ima samo anchor top-left
      theGrid.Parent.Size   = new Size(this.Width, this.Height  /*- ZXC.QUN/*- nextY - bottomHeight*/);
      theGrid.Parent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
      theGrid.Location      = new Point(ZXC.QunMrgn, nextY );

      // ali opet zeka kad je VvForm maloga formata pa gridovi dobiju svoje scrolove
      // minWidth - ovdje saljem za NalogDuc hamp_zaglavlje.Width - nevalja
      //T ovdje ipak treba _theGrid.width izracunat na temelju [irine njegovih kolona

      if(theGrid.Parent.Width < minWidth)
      {
       //  theGrid.Size = new Size(minWidth /*- 2 * ZXC.QunMrgn*/         , theGrid.Parent.Height - nextY - 2 * ZXC.QUN - theSumGrid.Height - bottomHeight);
         theGrid.Size = new Size(minWidth , theGrid.Parent.Height - nextY - theSumGrid.Height - bottomHeight);
      }
      else
      {
       //  theGrid.Size = new Size(theGrid.Parent.Width - 2 * ZXC.QunMrgn, theGrid.Parent.Height - nextY - 2 * ZXC.QUN - theSumGrid.Height - bottomHeight);
         theGrid.Size = new Size(theGrid.Parent.Width - 2 * ZXC.QunMrgn, theGrid.Parent.Height - nextY - theSumGrid.Height - bottomHeight);
      }
       theGrid.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
    }

   protected void CalcLocationSizeAnchor_GridSum(VvDataGridView theGrid)
   {
      theGrid.TheLinkedGrid_Sum.Width    = theGrid.Width;
      theGrid.TheLinkedGrid_Sum.Location = new Point(theGrid.Location.X, theGrid.Bottom + ZXC.Qun12);
      theGrid.TheLinkedGrid_Sum.Anchor   = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
   }
   
   public void CalcLocationSizeAnchor_TheDGVAndTheSumGrid_WidthTbxBottomOfSumGrid_Width2ChooserGrid(VvDataGridView theGrid, int _nextX, int _nextY, VvHamper hamperBelowDgv, VvHamper hamperAboveDgv, bool isVisibleCooser)
   {
      DataGridView theSumGrid   = theGrid.TheLinkedGrid_Sum;
      DataGridView theColCGrid1 = theGrid.TheLinkedGrid_ColC;
      DataGridView theColCGrid2 = theGrid.TheLinkedGrid_ColC2;

      int below;
      
      if(hamperBelowDgv == null) below = ZXC.Qun2;
      else                       below = hamperBelowDgv.Height;

      int heihgHamp = hamperAboveDgv.Height + below;
     
      hamperAboveDgv.Location = new Point(theGrid.Right - hamperAboveDgv.Width + ZXC.Qun4, _nextY);

      theColCGrid1.Location = new Point(_nextX, hamperAboveDgv.Bottom);
      theColCGrid2.Location = new Point(_nextX, theColCGrid1.Bottom);

      if(isVisibleCooser)  theGrid.Location = new Point(_nextX, theColCGrid2.Bottom);
      else                 theGrid.Location = new Point(_nextX, theColCGrid1.Top);
    
      theGrid.Size = new Size(theGrid.Parent.Width - 2 * _nextX, theGrid.Parent.Height - ZXC.QUN - theSumGrid.Height - heihgHamp);

      theGrid.Anchor        = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      theColCGrid1.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      theColCGrid2.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      
      theColCGrid1.Width = theGrid.Width;
      theColCGrid2.Width = theGrid.Width;
      theSumGrid.Width   = theGrid.Width;
      
      SetTheSumGridHeight_HScrollBarVisible(theSumGrid);


      if(isVisibleCooser) theGrid.Height = theGrid.Parent.Height - ZXC.QUN - theSumGrid.Height - heihgHamp - theColCGrid1.Height - theColCGrid2.Height;
      else                theGrid.Height = theGrid.Parent.Height - ZXC.QUN - theSumGrid.Height - heihgHamp;

      theSumGrid.Location = new Point(theGrid.Location.X, theGrid.Bottom + ZXC.Qun12);
      theSumGrid.Anchor   = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

      theGrid.Scroll    += new ScrollEventHandler(theGrid_Scroll); // potrebno kad se ide tabulatorom kroz kolone
      theSumGrid.Scroll += new ScrollEventHandler(theSumGrid_Scroll);
     
      hamperAboveDgv.Location = new Point(theGrid.Right - hamperAboveDgv.Width + ZXC.Qun4, _nextY);
      hamperAboveDgv.Anchor   = AnchorStyles.Top | AnchorStyles.Right;

      hamperBelowDgv.Location = new Point(theSumGrid.Right - hamperBelowDgv.Width + ZXC.Qun4, theSumGrid.Bottom);
      hamperBelowDgv.Anchor   = AnchorStyles.Bottom | AnchorStyles.Right;
   }

   public void CalcLocationSizeAnchor_TheDGVAndTheSumGrid_WidthTbxBottomOfSumGrid_Width2ChooserGrid_1row(VvDataGridView theGrid, int _nextX, int _nextY, bool isVisibleCooser)
   {
      DataGridView theSumGrid   = theGrid.TheLinkedGrid_Sum;
      DataGridView theColCGrid1 = theGrid.TheLinkedGrid_ColC;
      DataGridView theColCGrid2 = theGrid.TheLinkedGrid_ColC2;

      theColCGrid1.Location = new Point(_nextX, _nextY);
      theColCGrid2.Location = new Point(_nextX, theColCGrid1.Bottom);

      if(isVisibleCooser)  theGrid.Location = new Point(_nextX, theColCGrid2.Bottom);
      else                 theGrid.Location = new Point(_nextX, theColCGrid1.Top);
    
      theGrid.Size = new Size(theGrid.Parent.Width - 2 * _nextX, theGrid.Parent.Height - ZXC.QUN - theSumGrid.Height);

      theGrid.Anchor        = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      theColCGrid1.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      theColCGrid2.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      
      theColCGrid1.Width = theGrid.Width;
      theColCGrid2.Width = theGrid.Width;
      theSumGrid.Width   = theGrid.Width;
      
      SetTheSumGridHeight_HScrollBarVisible(theSumGrid);


      if(isVisibleCooser) theGrid.Height = theGrid.Parent.Height - ZXC.QUN - theSumGrid.Height- theColCGrid1.Height - theColCGrid2.Height;
      else                theGrid.Height = theGrid.Parent.Height - ZXC.QUN - theSumGrid.Height;

      theSumGrid.Location = new Point(theGrid.Location.X, theGrid.Bottom + ZXC.Qun12);
      theSumGrid.Anchor   = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

      theGrid.Scroll    += new ScrollEventHandler(theGrid_Scroll); // potrebno kad se ide tabulatorom kroz kolone
      theSumGrid.Scroll += new ScrollEventHandler(theSumGrid_Scroll);
     
   }

   public void CalcLocationSizeAnchor_TheDGVAndTheSumGrid_WidthTbxBottomOfSumGrid_WidthChooserGrid(VvDataGridView theGrid, int _nextX, int _nextY, VvHamper hamperBelowDgv, VvHamper hamperAboveDgv, bool isVisibleCooser)
   {
      DataGridView theSumGrid   = theGrid.TheLinkedGrid_Sum;
      DataGridView theColCGrid1 = theGrid.TheLinkedGrid_ColC;
      
      int below;
      
      if(hamperBelowDgv == null) below = ZXC.Qun2;
      else                       below = hamperBelowDgv.Height;

      int heihgHamp = hamperAboveDgv.Height + below;
     
      hamperAboveDgv.Location = new Point(theGrid.Right - hamperAboveDgv.Width + ZXC.Qun4, _nextY);

      theColCGrid1.Location = new Point(_nextX, hamperAboveDgv.Bottom);

      if(isVisibleCooser) theGrid.Location  = new Point(_nextX, theColCGrid1.Bottom);
      else                 theGrid.Location = new Point(_nextX, theColCGrid1.Top);

      //theGrid.Parent.Size   = new Size(this.Width, this.Height  /*- ZXC.QUN/*- nextY - bottomHeight*/);
      //theGrid.Parent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

  //    theGrid.Size = new Size(2000, theGrid.Height);
      theGrid.Size = new Size(theGrid.Parent.Width - 2 * _nextX, theGrid.Parent.Height - ZXC.QUN - theSumGrid.Height - heihgHamp);

      theGrid.Anchor        = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      theColCGrid1.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      
      theColCGrid1.Width = theGrid.Width;
      theSumGrid.Width   = theGrid.Width;
      
      SetTheSumGridHeight_HScrollBarVisible(theSumGrid);


      if(isVisibleCooser) theGrid.Height = theGrid.Parent.Height - ZXC.QUN - theSumGrid.Height - heihgHamp - theColCGrid1.Height;
      else                theGrid.Height = theGrid.Parent.Height - ZXC.QUN - theSumGrid.Height - heihgHamp;

      theSumGrid.Location = new Point(theGrid.Location.X, theGrid.Bottom + ZXC.Qun12);
      theSumGrid.Anchor   = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

      theGrid.Scroll    += new ScrollEventHandler(theGrid_Scroll); // potrebno kad se ide tabulatorom kroz kolone
      theSumGrid.Scroll += new ScrollEventHandler(theSumGrid_Scroll);
     
      hamperAboveDgv.Location = new Point(theGrid.Right - hamperAboveDgv.Width + ZXC.Qun4, _nextY);
      hamperAboveDgv.Anchor   = AnchorStyles.Top | AnchorStyles.Right;

      hamperBelowDgv.Location = new Point(theSumGrid.Right - hamperBelowDgv.Width + ZXC.Qun4, theSumGrid.Bottom);
      hamperBelowDgv.Anchor   = AnchorStyles.Bottom | AnchorStyles.Right;

   }

   public void CalcLocationSizeAnchor_TheDGVAndTheSumGrid_WidthTbxBottomOfSumGrid_RISK(VvDataGridView theGrid, int _nextX, int _nextY, VvHamper hamperBelowDgv, VvHamper hamperAboveDgv, int _preferredSizeWidth, int _colIdxArtiklName)
   {
      DataGridView theSumGrid   = theGrid.TheLinkedGrid_Sum;
      
      int below;
      
      if(hamperBelowDgv == null) below = ZXC.Qun2;
      else                       below = hamperBelowDgv.Height;

      int heihgHamp = hamperAboveDgv.Height + below;
     
      hamperAboveDgv.Location = new Point(theGrid.Right - hamperAboveDgv.Width + ZXC.Qun4, _nextY);

      theGrid.Location = new Point(_nextX, _nextY);

      theGrid.Size = new Size(theGrid.Parent.Width - 2 * _nextX, theGrid.Parent.Height - ZXC.QUN - theSumGrid.Height - heihgHamp);

      theGrid.Anchor     = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      theSumGrid.Width   = theGrid.Width;
      
      SetTheSumGridHeight_HScrollBarVisible(theSumGrid);

      theGrid.Height = theGrid.Parent.Height - ZXC.QUN - theSumGrid.Height - heihgHamp;

      theSumGrid.Location = new Point(theGrid.Location.X, theGrid.Bottom + ZXC.Qun12);
      theSumGrid.Anchor   = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

      theGrid.Scroll    += new ScrollEventHandler(theGrid_Scroll); // potrebno kad se ide tabulatorom kroz kolone
      theSumGrid.Scroll += new ScrollEventHandler(theSumGrid_Scroll);
     
      hamperAboveDgv.Location = new Point(theGrid.Right - hamperAboveDgv.Width + ZXC.Qun4, _nextY);
      hamperAboveDgv.Anchor   = AnchorStyles.Top | AnchorStyles.Right;

      hamperBelowDgv.Location = new Point(theSumGrid.Right - hamperBelowDgv.Width + ZXC.Qun4, theSumGrid.Bottom);
      hamperBelowDgv.Anchor   = AnchorStyles.Bottom | AnchorStyles.Right;

   }

   #endregion CalcLocationSizeAnchor_Grids

   #region Eveniti&Methods HScrollBarVisible
   
   private void SetTheSumGridHeight_HScrollBarVisible(DataGridView theSumGrid)
   {
      foreach(Control ctrl in theSumGrid.Controls)
      {
         if(ctrl is HScrollBar)
         {
            if(ctrl.Visible == true)
            {
               theSumGrid.Height = ZXC.Q2un + ZXC.Qun10;
            }
            else
            {
               theSumGrid.Height = ZXC.QUN + ZXC.Qun10;
            }
         }
      }
   }

   private void theGrid_Scroll(object sender, ScrollEventArgs e)
   {
      VvDataGridView theGrid = sender as VvDataGridView;
      DataGridView   theSumGrid  = theGrid.TheLinkedGrid_Sum;
      DataGridView   theColCGrid = theGrid.TheLinkedGrid_ColC;

      if(theSumGrid  != null) theSumGrid.HorizontalScrollingOffset = theGrid.HorizontalScrollingOffset;
      if(theColCGrid != null) theColCGrid.HorizontalScrollingOffset = theGrid.HorizontalScrollingOffset;

   }

   private void theSumGrid_Scroll(object sender, ScrollEventArgs e)
   {
      DataGridView   theSumGrid  = sender as DataGridView;
      VvDataGridView theGrid     = (VvDataGridView)(theSumGrid.Tag);
      DataGridView   theColCGrid = theGrid.TheLinkedGrid_ColC;

                              theGrid.HorizontalScrollingOffset     = theSumGrid.HorizontalScrollingOffset;
      if(theColCGrid != null) theColCGrid.HorizontalScrollingOffset = theGrid.HorizontalScrollingOffset;
   }
  
   #endregion Eveniti&Methods HScrollBarVisible

}

public abstract class VvPolyDocumRecordUC : VvDocumentRecordUC
{
   #region Constructor

   public VvPolyDocumRecordUC(/*string tp1Title, string tp2Title, string tp3Title*/)
   {
      SuspendLayout();

      CreateThePolyGridTabControl();

      ResumeLayout();
   }



   #endregion Constructor

   #region Common Propertiz

   public abstract string TabPageTitle1 { get; }
   public abstract string TabPageTitle2 { get; }
   public abstract string TabPageTitle3 { get; }

   public virtual  string TabPageTitleResult { get { return ""; } }

   public abstract VvPolyDocumRecord VirtualPolyDocumRecord
   {
      get;
      set;
   }

   public Crownwood.DotNetMagic.Controls.TabControl ThePolyGridTabControl { get; set; }

   //=== PtranE, Trans2, VirtualTranses2, TheG2, ... ============================================= 
   public abstract VvDaoBase TheVvDaoTrans2
   {
      get;
   }

   public DataGridView   TheColChooserGrid2  { get; set; }
   public VvDataGridView TheG2               { get; set; }
   // 03.02.2014: 
 //public   DataGridView   TheSumGrid2         { get; set; }
   public VvDataGridView   TheSumGrid2         { get; set; }

   protected List<VvTransRecord> VirtualTranses2 { get { return this.VirtualPolyDocumRecord.VirtualTranses2; } }

   //=== PtranO, Trans3, VirtualTranses3, TheG3, ... ============================================= 
   public abstract VvDaoBase TheVvDaoTrans3
   {
      get;
   }

   public DataGridView   TheColChooserGrid3  { get; set; }
   public VvDataGridView TheG3               { get; set; }

   // 29.04.2024: 
 //public   DataGridView TheSumGrid3         { get; set; }
   public VvDataGridView TheSumGrid3         { get; set; }

   protected List<VvTransRecord> VirtualTranses3 { get { return this.VirtualPolyDocumRecord.VirtualTranses3; } }

   #endregion Common Propertiz

   #region Transes Utils

   protected void MarkTranses2ToDelete(uint[] dgvRecIDtable)
   {
      MarkTransesToDelete_JOB(dgvRecIDtable, VirtualTranses2);
   }

   protected void MarkTranses3ToDelete(uint[] dgvRecIDtable)
   {
      MarkTransesToDelete_JOB(dgvRecIDtable, VirtualTranses3);
   }

   #endregion Transes Utils

   #region Abstract Methods

   public abstract void PutDgvLineFields1(VvTransRecord trans_rec, int rowIdx, bool skipRecID_andSerial_Columns);
   public abstract void PutDgvLineFields2(VvTransRecord trans_rec, int rowIdx, bool skipRecID_andSerial_Columns);
   public abstract void PutDgvLineFields3(VvTransRecord trans_rec, int rowIdx, bool skipRecID_andSerial_Columns);

   public abstract VvTransRecord GetDgvLineFields1(int rIdx, bool dirtyFlagging, uint[] recIDtable);
   public abstract VvTransRecord GetDgvLineFields2(int rIdx, bool dirtyFlagging, uint[] recIDtable);
   public abstract VvTransRecord GetDgvLineFields3(int rIdx, bool dirtyFlagging, uint[] recIDtable);

   public /*abstract*/ virtual void PutDgvLineResultsFields1(int rIdx, VvTransRecord trans_rec, bool passPtrResultsToZaglavljeTranses) { }
   public /*abstract*/ virtual void PutDgvLineResultsFields2(int rIdx, VvTransRecord trans_rec, bool passPtrResultsToZaglavljeTranses) { }
   public /*abstract*/ virtual void PutDgvLineResultsFields3(int rIdx, VvTransRecord trans_rec, bool passPtrResultsToZaglavljeTranses) { }

   public abstract void PutDgvTransSumFields1();
   public abstract void PutDgvTransSumFields2();
   public abstract void PutDgvTransSumFields3();

   #endregion Abstract Methods

   #region TheCurrentG, GetDgvLineFields, PutDgvLineResultsFields

   public VvDataGridView TheCurrentG
   {
      get
      {
              if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle1) return TheG;
         else if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle2) return TheG2;
         else if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle3) return TheG3;
         else throw new Exception("VvUserControlRecord_Sub.TheCurrentG: ThePolyGridTabCOntrol.SelectedTab.Title [" + ThePolyGridTabControl.SelectedTab.Title + "] Nepoznat!");

         //switch(ThePolyGridTabControl.SelectedTab.Title)
         //{
         //   case TabPageTitle1: return TheG;
         //   case TabPageTitle2: return TheG2;
         //   case TabPageTitle3: return TheG3;

         //   default: throw new Exception("VvUserControlRecord_Sub.TheCurrentG: ThePolyGridTabCOntrol.SelectedTab.Title [" + ThePolyGridTabControl.SelectedTab.Title + "] Nepoznat!");
         //}
      }
   }

   public VvDataGridView TheCurrentSumGrid
   {
      get
      {
              if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle1) return TheSumGrid;
         else if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle2) return TheSumGrid2;
         else if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle3) return TheSumGrid3;
         else throw new Exception("VvUserControlRecord_Sub.TheCurrentSumGrid: ThePolyGridTabCOntrol.SelectedTab.Title [" + ThePolyGridTabControl.SelectedTab.Title + "] Nepoznat!");

         //switch(ThePolyGridTabControl.SelectedTab.Title)
         //{
         //   case TabPageTitle1: return TheG;
         //   case TabPageTitle2: return TheG2;
         //   case TabPageTitle3: return TheG3;

         //   default: throw new Exception("VvUserControlRecord_Sub.TheCurrentG: ThePolyGridTabCOntrol.SelectedTab.Title [" + ThePolyGridTabControl.SelectedTab.Title + "] Nepoznat!");
         //}
      }
   }

   public override void PutDgvLineFields(VvTransRecord trans_rec, int rowIdx, bool skipRecID_andSerial_Columns)
   {
           if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle1) PutDgvLineFields1(trans_rec, rowIdx, skipRecID_andSerial_Columns);
      else if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle2) PutDgvLineFields2(trans_rec, rowIdx, skipRecID_andSerial_Columns);
      else if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle3) PutDgvLineFields3(trans_rec, rowIdx, skipRecID_andSerial_Columns);
      else throw new Exception("VvUserControlRecord_Sub.TheCurrentG: ThePolyGridTabCOntrol.SelectedTab.Title [" + ThePolyGridTabControl.SelectedTab.Title + "] Nepoznat!");
   }

   // TODO: !!!!!!!! 
   public override VvTransRecord GetDgvLineFields(int rIdx, bool dirtyFlagging, uint[] recIDtable)
   {
           if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle1) return GetDgvLineFields1(rIdx, dirtyFlagging, recIDtable);
      else if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle2) return GetDgvLineFields2(rIdx, dirtyFlagging, recIDtable);
      else if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle3) return GetDgvLineFields3(rIdx, dirtyFlagging, recIDtable);
      else throw new Exception("VvUserControlRecord_Sub.TheCurrentG: ThePolyGridTabCOntrol.SelectedTab.Title [" + ThePolyGridTabControl.SelectedTab.Title + "] Nepoznat!");
   }

   public override void PutDgvLineResultsFields(VvTransRecord trans_rec, int rIdx, bool passPtrResultsToZaglavljeTranses)
   {
           if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle1) PutDgvLineResultsFields1(rIdx, trans_rec, passPtrResultsToZaglavljeTranses);
      else if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle2) PutDgvLineResultsFields2(rIdx, trans_rec, passPtrResultsToZaglavljeTranses);
      else if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle3) PutDgvLineResultsFields3(rIdx, trans_rec, passPtrResultsToZaglavljeTranses);
      else throw new Exception("VvUserControlRecord_Sub.PutDgvLineResultsFields: ThePolyGridTabCOntrol.SelectedTab.Title [" + ThePolyGridTabControl.SelectedTab.Title + "] Nepoznat!");
   }

   public override void PutDgvTransSumFields()
   {
           if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle1)      PutDgvTransSumFields1();
      else if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle2)      PutDgvTransSumFields2();
      else if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle3)      PutDgvTransSumFields3();
      else if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitleResult)                        ; // do nothing 
      // 09.06.2022: throw komentiran 
    //else throw new Exception("VvUserControlRecord_Sub.PutDgvTransSumFields: ThePolyGridTabCOntrol.SelectedTab.Title [" + ThePolyGridTabControl.SelectedTab.Title + "] Nepoznat!");
   }

   #endregion TheCurrentG

   #region ThePolyGridTabControl
  
   private void CreateThePolyGridTabControl()
   {
      ThePolyGridTabControl                  = new Crownwood.DotNetMagic.Controls.TabControl();
      ThePolyGridTabControl.Appearance       = VisualAppearance.MultiDocument;
      ThePolyGridTabControl.ShowArrows       = false;
      ThePolyGridTabControl.ShowClose        = false;
      ThePolyGridTabControl.HotTrack         = true;
      ThePolyGridTabControl.Style            = ZXC.vvColors.vvform_VisualStyle;
      ThePolyGridTabControl.OfficeStyle      = ZXC.vvColors.tabControl_OfficeStyle;
      ThePolyGridTabControl.MediaPlayerStyle = ZXC.vvColors.tabControl_MediaPlayerStyle;

      ThePolyGridTabControl.TabPages.Add(new Crownwood.DotNetMagic.Controls.TabPage(TabPageTitle1));
      ThePolyGridTabControl.TabPages.Add(new Crownwood.DotNetMagic.Controls.TabPage(TabPageTitle2));
      ThePolyGridTabControl.TabPages.Add(new Crownwood.DotNetMagic.Controls.TabPage(TabPageTitle3));

      ThePolyGridTabControl.TabPages[TabPageTitle1].BackColor = ZXC.vvColors.tabPage4TheG_BackColor;
      ThePolyGridTabControl.TabPages[TabPageTitle2].BackColor = ZXC.vvColors.tabPage4TheG2_BackColor;
      ThePolyGridTabControl.TabPages[TabPageTitle3].BackColor = ZXC.vvColors.tabPage4TheG3_BackColor;

      ThePolyGridTabControl.TabPages[TabPageTitle1].Tag = ZXC.vvColors.tabPage4TheG_BackColor;
      ThePolyGridTabControl.TabPages[TabPageTitle2].Tag = ZXC.vvColors.tabPage4TheG2_BackColor;
      ThePolyGridTabControl.TabPages[TabPageTitle3].Tag = ZXC.vvColors.tabPage4TheG3_BackColor;

   }

   protected void CalcLocationSizeAnchor_ThePolyGridTabControl(Control parent, int _nextX, int _nextY)
   {
      //SuspendLayout();

      // 02.05.2016: 
      if(ThePolyGridTabControl.Parent == null) return;

      ThePolyGridTabControl.Parent.Size   = new Size(this.Width, this.Height);
      ThePolyGridTabControl.Parent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

   // ThePolyGridTabControl.Location = new Point(ZXC.QunMrgn, _nextY + ZXC.QunMrgn);
      ThePolyGridTabControl.Location = new Point(ZXC.QunMrgn, _nextY + ZXC.Qun2);
      
    //ThePolyGridTabControl.Size     = new Size (parent.Width - 2 * ZXC.QunMrgn, parent.Height - _nextY - ZXC.Q2un - ZXC.Qun4);12.05.2015.
      ThePolyGridTabControl.Size     = new Size (parent.Width - 2 * ZXC.QunMrgn, parent.Height - _nextY - ZXC.QUN);
      
      ThePolyGridTabControl.Anchor   = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

      //ResumeLayout();

   }

   #endregion ThePolyGridTabControl

   public decimal Put_ColSum_OnSumGrid(int colIdx)
   {
      decimal theSum = 0.00M;

      //if(TheVvTabPage.WriteMode == ZXC.WriteMode.None)
      //{
      //        if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle1) return VirtualPolyDocumRecord.VirtualTranses;
      //   else if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle2) return TheSumGrid2;
      //   else if(ThePolyGridTabControl.SelectedTab.Title == TabPageTitle3) return TheSumGrid3;
      //
      //   return faktur_rec.Transes2.Where(rto => rto.T_TT == Faktur.TT_MOC).Count();
      //}

      for(int rowIdx = 0; rowIdx < TheCurrentG.RowCount - 1; ++rowIdx)
      {
         theSum += TheCurrentG.GetDecimalCell(colIdx, rowIdx, false);
      }

      TheCurrentSumGrid.PutCell(colIdx, 0, theSum);

      return theSum;
   }
}

public abstract class VvSifrarRecordUC    : VvRecordUC
{
   #region DGV Transakcije

   private const ushort transMaxComplets = 4;

   public    DataGridView[] aTransesGrid        = new DataGridView[transMaxComplets];
   public    bool        [] aTransesLoaded      = new bool        [transMaxComplets];

   public string SelectedTTin_FIRST_TransGrid
   {
      get
      {
         if(aTransesGrid[0].CurrentRow != null)
            return aTransesGrid[0].CurrentRow.Cells["TT"].Value.ToString();
         else
            return null;
      }
   }

   public int SelectedRecIDIn_FIRST_TransGrid
   {
      get
      {
         if(aTransesGrid[0].CurrentRow != null)
            return int.Parse(aTransesGrid[0].CurrentRow.Cells[0].Value.ToString());
         else
            return -1;
      }
   }

   public int SelectedRecIDIn_SECOND_TransGrid
   {
      get
      {
         if(aTransesGrid[1].CurrentRow != null)
            return int.Parse(aTransesGrid[1].CurrentRow.Cells[0].Value.ToString());
         else
            return -1;
      }
   }

   public int SelectedRecIDIn_THIRD_TransGrid
   {
      get
      {
         if(aTransesGrid[2].CurrentRow != null)
            return int.Parse(aTransesGrid[2].CurrentRow.Cells[0].Value.ToString());
         else
            return -1;
      }
   }

   protected abstract void theFIRST_TransGrid_DoubleClick(object sender, EventArgs e);

   protected void theFIRST_TransGrid_KeyPress(object sender, KeyPressEventArgs e)
   {
      if(e.KeyChar == (char)Keys.Enter)
      {
         theFIRST_TransGrid_DoubleClick(sender, (EventArgs)e);
      }
   }

   #endregion DGV Transakcije
   
   public abstract VvSifrarRecord VirtualSifrarRecord
   {
      get;
      set;
   }

   public abstract void PutNew_Sifra_Field(uint newSifra);
   public virtual  void PutNew_Sifra_Field(string newSifra) {}

   protected void OpenNew_Record_TabPage_OnDoubleClick(ZXC.VvSubModulEnum vvSubModulEnum, int selectedRecID)
   {
      if(selectedRecID < 1) return;

      ZXC.TheVvForm.OpenNew_Record_TabPage(ZXC.TheVvForm.GetSubModulXY(vvSubModulEnum), (uint?)selectedRecID);
   }

   protected (bool, VvSifrarRecord) IsThisSifra_Duplicated_InNY()
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.Add && ZXC.IsSifrar_And_WeAreInPGyear(TheVvTabPage.TheVvDataRecord))
      {
         bool NY_dbExists = ZXC.Does_NY_dbExists(TheDbConnection);

         if(NY_dbExists)
         {
            bool NY_tableExists = ZXC.Does_NY_tableExists(TheDbConnection, TheVvTabPage.TheVvDataRecord.VirtualRecordName);

            if(NY_tableExists)
            {
               (bool thisSifraIs_Duplicated_InNY, VvSifrarRecord inNY_SifrarRecord) = TheVvDao.SifrarRecordExistsInNY(TheDbConnection, TheVvTabPage.TheVvDataRecord as VvSifrarRecord);

               if(thisSifraIs_Duplicated_InNY) return (true, inNY_SifrarRecord);
            }
         }
      }

      return (false, null);
   }
}
