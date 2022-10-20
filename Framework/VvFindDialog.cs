using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Drawing;
using Crownwood.DotNetMagic.Forms;

public class VvFindDialog : DotNetMagicForm
{
   public  Button   button_OK, button_Cancel, button_Apply, button_OpenTPage, button_AddSifrarRec;
   private VvHamper hampButtons, hampOpenTabPage;
   
   private VvRecLstUC vvRecListUC;
   public  VvRecLstUC TheRecListUC
   {
      get { return vvRecListUC; }
      set { vvRecListUC = value; }
   }

   private bool selectionIsNewlyAddedRecord = false;
   public  bool SelectionIsNewlyAddedRecord
   {
      get { return selectionIsNewlyAddedRecord; }
      set {        selectionIsNewlyAddedRecord = value; }
   }

   public VvFindDialog()
   {
      SuspendLayout();

      InitializeApearanceAndLayout();
      CreateHamperOpenTabPage();
      CreateHamperButtons();


      ResumeLayout();


   }

   private void InitializeApearanceAndLayout()
   {
      this.FormBorderStyle = FormBorderStyle.Sizable;
      this.MinimizeBox     = false;
      this.ShowInTaskbar   = false;
      //this.StartPosition   = FormStartPosition.CenterScreen;
      this.StartPosition   = FormStartPosition.Manual;
      this.Font            = ZXC.vvFont.BaseFont;
      this.Style           = ZXC.vvColors.vvform_VisualStyle;
      this.BackColor       = ZXC.vvColors.userControl_BackColor;

      //this.Size    = new Size(400, 725);
      this.Load   += new EventHandler(VvFindDialog_Load);

      //puse: this.Closed += new EventHandler(VvFindDialog_Closed_DisposeDataSet);
   }

   #region hamperi

   private void CreateHamperButtons()
   {
      hampButtons = new VvHamper(3, 1, "", this, false);

      hampButtons.VvColWdt      = new int[] { ZXC.QunBtnW, ZXC.QunBtnW, ZXC.QunBtnW };
      hampButtons.VvSpcBefCol   = new int[] {           0, ZXC.Qun4   , ZXC.Qun4 };
      hampButtons.VvRightMargin = hampButtons.VvLeftMargin;

      hampButtons.VvRowHgt       = new int[] { ZXC.QunBtnH };
      hampButtons.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hampButtons.VvBottomMargin = hampButtons.VvTopMargin;

      button_Apply  = hampButtons.CreateVvButton(2, 0, new EventHandler(button_Apply_Click), "Primjeni");
      button_OK     = hampButtons.CreateVvButton(0, 0, null                                , "Odaberi" );
      button_Cancel = hampButtons.CreateVvButton(1, 0, null                                , "Odustani");

      button_OK.DialogResult     = DialogResult.OK;
      button_Cancel.DialogResult = DialogResult.Cancel;

      this.CancelButton = this.button_Cancel;

      hampButtons.Anchor   = AnchorStyles.Bottom | AnchorStyles.Right;

      int hampButLeft = this.ClientSize.Width - ZXC.QunMrgn - hampButtons.Width;
     
      hampButtons.Location = new Point(hampButLeft, this.Bottom - 4 * ZXC.QunBtnH);

   }

   private void CreateHamperOpenTabPage()
   {
      hampOpenTabPage = new VvHamper(2, 1, "", this, false);

      hampOpenTabPage.VvColWdt      = new int[] { ZXC.QunBtnW, ZXC.QunBtnW };
      hampOpenTabPage.VvSpcBefCol   = new int[] {           0, ZXC.Qun4 };
      hampOpenTabPage.VvRightMargin = hampOpenTabPage.VvLeftMargin;

      hampOpenTabPage.VvRowHgt       = new int[] { ZXC.QunBtnH };
      hampOpenTabPage.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hampOpenTabPage.VvBottomMargin = hampOpenTabPage.VvTopMargin;

      button_OpenTPage    = hampOpenTabPage.CreateVvButton(0, 0, new EventHandler(OpenRecListVvTabPage_Click), "Otvori listu");
      button_AddSifrarRec = hampOpenTabPage.CreateVvButton(1, 0, new EventHandler(AddSifrarRecord_Click)     , "Dodaj novi");

      hampOpenTabPage.Location = new Point(ZXC.QunMrgn, this.Bottom - 4 * ZXC.QunBtnH);
      hampOpenTabPage.Anchor   = AnchorStyles.Bottom | AnchorStyles.Left;

      //if(ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(TheRecListUC.MasterSubModulEnum).subModulKindEnum == ZXC.VvSubModulKindEnum.SIFRAR)
      //{
      //   button_AddSifrarRec.Visible = true;
      //}
      //else
      //{
      //   button_AddSifrarRec.Visible = false;
      //}
    }

   #endregion hamperi

   #region Common Propertiz

   public int SelectedRecID
   {
      get
      {
         //if (TheRecListUC.TheG.CurrentRow != null)
         //   return int.Parse(TheRecListUC.TheG.CurrentRow.Cells["prjktKupdobCD"].Value.ToString());
         //else
         //   return -1;

         return TheRecListUC.SelectedRecID;
      }
   }

   public event EventHandler ApplyEventHandler;

   #endregion Common Propertiz

   #region  Eventz
   
   private void button_Apply_Click(object sender, System.EventArgs e)
   {
      if(ApplyEventHandler != null) ApplyEventHandler(this, new EventArgs());
   }

   void VvFindDialog_Load(object sender, EventArgs e)
   {
      if (SelectedRecID < 1)
      {
         button_OK.Enabled = button_Apply.Enabled = false;
      }

      //if (ApplyEventHandler == null) ((VvFindDialog)this.Parent).button_Apply.Visible = false;

      LocationHamperButtons(button_Apply.Visible);

      if((ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(TheRecListUC.MasterSubModulEnum)).subModulKindEnum == ZXC.VvSubModulKindEnum.SIFRAR)
      {
         button_AddSifrarRec.Visible = true;
      }
      else
      {
         button_AddSifrarRec.Visible = false;

      }

      TheRecListUC.InitializeStartValue();

      if(ZXC.CurrentForm == ZXC.TheVvForm) button_OpenTPage.Enabled = true;
      else                                 button_OpenTPage.Enabled = false;

      if(TheRecListUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.FORBIDDEN) button_OpenTPage.Enabled = false;
   
   }

   private void LocationHamperButtons(bool visibleButt_Apply)
   {
      if(!visibleButt_Apply)
      {
         button_OK.Location     = button_Cancel.Location;
         button_Cancel.Location = button_Apply.Location;
      }
   }

   private void OpenRecListVvTabPage_Click(object sender, System.EventArgs e)
   {
      if(TheRecListUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.FORBIDDEN)
      {
         ZXC.IssueAccessDeniedMessage(ZXC.PrivilegedAction.ENTER_SUBMODUL, TheRecListUC.TheSubModul, "");
         return;
      }

      ZXC.TheVvForm.OpenNew_RecLst_TabPage(TheRecListUC.TheSubModul.xy, TheRecListUC);
      this.Dispose();
   }

   private void AddSifrarRecord_Click(object sender, System.EventArgs e)
   {
      VvSifrarRecord vvSifrarRecord = ZXC.TheVvForm.AddAndGetNewVvSifrarRecordInteractive(TheRecListUC);

      if(vvSifrarRecord != null)
      {
         if(TheRecListUC.TheGrid.CurrentRow == null)
         {
            TheRecListUC.TheGrid.DataSource = null;
            TheRecListUC.TheGrid.Rows.Add();
         }

         TheRecListUC.TheGrid.CurrentRow.Cells["recID"].Value = vvSifrarRecord.VirtualRecID;

         TheRecListUC.TheGrid_DoubleClick(null, EventArgs.Empty);

         SelectionIsNewlyAddedRecord = true;
      }
   }

   #endregion  Eventz

}
