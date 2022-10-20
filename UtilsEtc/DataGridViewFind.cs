using System;
using System.Windows.Forms;

// news: 22.10.2010 DataGridViewFind je do sada bio 'samo' DataGridView a od sada je Vv...
public class VvDataGridViewFind : VvDataGridView
{
   public override bool IsDgvFind { get { return (true); } }

   protected override bool ProcessDataGridViewKey(KeyEventArgs e)
   {
      if (e.KeyCode == Keys.Enter)
      {
         return true;  // javili smo tamo nekome gore da je enter nasa briga i da o s njim nema kaj vise traziti
      }
      return base.ProcessDataGridViewKey(e);
   }
}