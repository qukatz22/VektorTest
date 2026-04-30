using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using CrystalDecisions.Windows.Forms;
using Vektor.Reports.FIZ;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using                   MySql.Data.MySqlClient;
using XSqlConnection  = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand     = MySql.Data.MySqlClient.MySqlCommand;
using XSqlDataReader  = MySql.Data.MySqlClient.MySqlDataReader;
using XSqlDbType      = MySql.Data.MySqlClient.MySqlDbType;
using XSqlParameter   = MySql.Data.MySqlClient.MySqlParameter;
using XSqlException   = MySql.Data.MySqlClient.MySqlException;
using XSqlDataAdapter = MySql.Data.MySqlClient.MySqlDataAdapter;
using Vektor.Reports.FIZ;
#endif

public class NalogClose47DLG : VvDialog
{
   private Button okButton, cancelButton;
   private VvHamper hamper;
   private int dlgWidth, dlgHeight;
   private CheckBox cbx_ocePrenosnaRashode;

   public NalogClose47DLG()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
    //this.Text = "Potvrdite zatvaranje klase 4 i 7";
      this.Text = "Potvrdite zatvaranje klasa Prihoda i Rashoda";

      CreateHamper();

      dlgWidth = hamper.Right; // ovdje je manji jer je naslov veliki
      dlgHeight = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
   }

   private void CreateHamper()
   {
      hamper = new VvHamper(1, 1, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q10un + ZXC.Q4un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 + ZXC.QUN };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
      string text = "";
           if (ZXC.CURR_prjkt_rec.IsNeprofit)                          text = "Prijenos na obračun prihoda i rashoda (52110)";
      else if (ZXC.CURR_prjkt_rec.PlanKind == ZXC.PlanKindEnum.PlnBy_FOND) text = "";
      else                                                             text = "Prijenos na rashode (7000)";

    //cbx_ocePrenosnaRashode = hamper.CreateVvCheckBox_OLD(0, 0, null, "Prijenos na rashode (7000)", RightToLeft.No);
    // 07.03.2016. 
      cbx_ocePrenosnaRashode = hamper.CreateVvCheckBox_OLD(0, 0, null, text, RightToLeft.No);
      if(ZXC.CURR_prjkt_rec.PlanKind == ZXC.PlanKindEnum.PlnBy_FOND) cbx_ocePrenosnaRashode.Visible = false;
      
      cbx_ocePrenosnaRashode.Font = ZXC.vvFont.BaseFont;
   }

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   public bool Fld_OcePrenosNaRashode
   {
      get { return cbx_ocePrenosnaRashode.Checked; }
      set { cbx_ocePrenosnaRashode.Checked = value; }
   }
}

public class PrenosPsDLG : VvDialog
{
   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_dbName;
   private CheckBox  cbx_preskoci_WYRN;
   private bool      isFromNalog;
   private VvTextBox tbx_skladCd, tbx_skladOpis;

   public PrenosPsDLG(string prevYear, string newYear, bool _isFromNalog)
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Potvrdite prijenos početnog stanja " + prevYear + " ---> " + newYear;
      
      this.isFromNalog   = _isFromNalog;
      
      CreateHamper();
      
      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvOtherUC);

      dlgWidth = hamper.Right + ZXC.QunMrgn;
      dlgHeight = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
   }

   private void CreateHamper()
   {
      hamper = new VvHamper(2, 4, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q10un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN, ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.QUN, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                   hamper.CreateVvLabel(0, 0, "Prošlogod. podaci:", ContentAlignment.MiddleRight);
      tbx_dbName = hamper.CreateVvTextBox(1, 0, "tbx_dbName", "");
      
      cbx_preskoci_WYRN = hamper.CreateVvCheckBox_OLD(0, 1, null, 1, 2, "\t\tPRESKOČI kreiranje WYRN dokumenata\n\t\t(potrebnih za OPZ-STAT-1)", System.Windows.Forms.RightToLeft.No);
      cbx_preskoci_WYRN.Visible = isFromNalog;

      Label lblSkl  = hamper.CreateVvLabel        (0, 2, "Samo za Sklad:", ContentAlignment.MiddleRight);
      tbx_skladCd   = hamper.CreateVvTextBoxLookUp(0, 3, "tbx_skladCd", "Šifra skladišta");
      tbx_skladOpis = hamper.CreateVvTextBox      (1, 3, "tbx_skladOpis_InVisible", "Naziv skladišta");
      tbx_skladCd.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_skladCd.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_skladCd.JAM_lui_NameTaker_JAM_Name = tbx_skladOpis.JAM_Name;
      tbx_skladOpis.JAM_ReadOnly = true;

      lblSkl.Visible = tbx_skladCd.Visible = tbx_skladOpis.Visible = !isFromNalog;

   }

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   public string Fld_DBName        {  get { return tbx_dbName.Text;           }  set { tbx_dbName.Text          = value; } }
   public bool   Fld_Preskoci_WYRN {  get { return cbx_preskoci_WYRN.Checked; } set { cbx_preskoci_WYRN.Checked = value; } }
   public string Fld_skladCD       { get { return tbx_skladCd.Text;           } set { tbx_skladCd.Text          = value; } }

}

public class PreviewZNPdlg : Form
{
   #region Constructor, Porpertiez, Methodz, ...

   private Vektor.DataLayer.DS_Reports.DS_ZapIzvodNEW TheDS_ZapIzvod { get; set; }
   private CR_PreviewZNP TheCR_PreviewZNP { get; set; }

   private ZbrojniNalogZaPrijenos TheZNP { get; set; }

   private OpenFileDialog TheOpenFileDialog { get; set; }

   private VvStandAloneReportViewerUC thePreviewZnpUC;

   //private bool alreadyDisposed = false;

   public PreviewZNPdlg()
   {
      SuspendLayout();

      this.Font      = ZXC.vvFont.BaseFont;
      this.BackColor = ZXC.vvColors.userControl_BackColor;

      this.StartPosition = FormStartPosition.CenterScreen;
      //this.Location      = Point.Empty;
      this.Size = new Size(SystemInformation.WorkingArea.Width / 2, SystemInformation.WorkingArea.Height);

      this.FormClosing += new FormClosingEventHandler(PreviewZNPdlg_FormClosing);


      #region Set VvPreviewReportUC

      thePreviewZnpUC = new VvStandAloneReportViewerUC();
      thePreviewZnpUC.Parent = this;
      thePreviewZnpUC.Dock = DockStyle.Fill;

      #endregion Set VvPreviewReportUC

      ZXC.TheVvForm.ToolStripReportVisible_tsBtnOnTsReportVisible(false, ZXC.ReportMode.Working, true);
      ZXC.TheVvForm.Report_SubModul_EnableOrDisable_TSButtonsAndTSMnItems(ZXC.ReportMode.Working, true);

      ZXC.TheVvForm.ts_Report.Items["Open"].Click += new EventHandler(OpenExternReportSource_Click);

      ResumeLayout();

      #region FileDialog

      TheOpenFileDialog = new OpenFileDialog();
      TheOpenFileDialog.InitialDirectory = @"ZNP";
      //TheOpenFileDialog.Filter = "ZNP datoteke (*.txt, *.ZAP, *.ZABA)|*.txt;*.ZAP;*.ZABA|All files (*.*)|*.*";
      TheOpenFileDialog.Filter = "ZNP datoteke (*.ZAP)|*.ZAP;|All files (*.*)|*.*";
      TheOpenFileDialog.FilterIndex = 1;
      TheOpenFileDialog.RestoreDirectory = true;

      #endregion FileDialog

      TheCR_PreviewZNP = new CR_PreviewZNP();

   }

   private void PreviewZNPdlg_FormClosing(object sender, FormClosingEventArgs e)
   {
      // ... jer je samo jedan ts_Report 
      ZXC.TheVvForm.ts_Report.Parent = ZXC.TheVvForm.tsPanel_SubModul;

      //  ...obvisno od kuda se poziva ovaj dialog/forma jer ako nije sa reporta onda tsReport mora biti nevisible
      ZXC.TheVvForm.ts_Report.Visible = true;

      ZXC.TheVvForm.ToolStripReportVisible_tsBtnOnTsReportVisible(true, ZXC.ReportMode.Done, false);

      ZXC.TheVvForm.ts_Report.Items["Open"].Click -= new EventHandler(OpenExternReportSource_Click);
   }

   #endregion Constructor, Porpertiez, Methodz, ...

   #region FillDataset

   public void FillZnpDataSet(ZbrojniNalogZaPrijenos _theZNP)
   {
      TheDS_ZapIzvod = new Vektor.DataLayer.DS_Reports.DS_ZapIzvodNEW();

      TheDS_ZapIzvod.headRecord.Rows.Add(

         _theZNP.HeadRecord.iz_br,
         _theZNP.HeadRecord.iz_date,
         _theZNP.HeadRecord.iz_prev_sign,
         _theZNP.HeadRecord.iz_new_sign,
         _theZNP.HeadRecord.iz_firma,
         _theZNP.HeadRecord.bankaName,
         _theZNP.HeadRecord.bankaVbdi,
         _theZNP.HeadRecord.iz_ziro_rnpri,
         _theZNP.HeadRecord.iz_ziro_racun,
         _theZNP.HeadRecord.iz_uk_dug,
         _theZNP.HeadRecord.iz_uk_pot,
         _theZNP.HeadRecord.iz_prev_saldo,
         _theZNP.HeadRecord.iz_saldo,
         _theZNP.HeadRecord.iz_mjesto,
         _theZNP.HeadRecord.iz_dateIn6,
         _theZNP.HeadRecord.iz_numOfTranses,
         _theZNP.FileName
      );

      int i = 0;
      foreach(ZapIzvod.TransRecordStruct trans_rec in _theZNP.Transes)
      {
         TheDS_ZapIzvod.transRecord.Rows.Add(

            _theZNP.Transes[i].t_br,
            _theZNP.Transes[i].t_serial,
            _theZNP.Transes[i].t_valuta,
            _theZNP.Transes[i].t_rbr,
            _theZNP.Transes[i].t_kd_ziro,
            _theZNP.Transes[i].t_kupdob,
            _theZNP.Transes[i].t_kd_mjesto,
            _theZNP.Transes[i].t_pnb0_zad,
            _theZNP.Transes[i].t_pnb1_zad,
            _theZNP.Transes[i].t_pnb0_odob,
            _theZNP.Transes[i].t_pnb1_odob,
            _theZNP.Transes[i].t_vezna_oznaka,
            _theZNP.Transes[i].t_svrha_doz,
            _theZNP.Transes[i].t_new_old_rn,
            _theZNP.Transes[i].t_reklamacija,
            _theZNP.Transes[i].t_iznos,
            _theZNP.Transes[i].t_dug,
            _theZNP.Transes[i].t_pot,
            _theZNP.Transes[i].t_vrsta_transakcije,
            _theZNP.Transes[i].t_rnpvd,
            _theZNP.Transes[i].t_kd_vbdi
         );

         i++;
      }
   }

   #endregion FillDataset

   private void OpenExternReportSource_Click(object sender, EventArgs e)
   {
      if(TheOpenFileDialog.ShowDialog() == DialogResult.OK)
      {
         TheZNP = new ZbrojniNalogZaPrijenos(TheOpenFileDialog.FileName);

         #region FillDataset, SetDataSource, AssignReportSource

         if(TheZNP != null)
         {
            FillZnpDataSet(TheZNP);

            thePreviewZnpUC.SetDataSource_And_AssignReportSource(TheCR_PreviewZNP, TheDS_ZapIzvod);
         }

         #endregion FillDataset, CreateReportDocument, SetDataSource, AssignReportSource

         ZXC.TheVvForm.Report_SubModul_EnableOrDisable_TSButtonsAndTSMnItems(ZXC.ReportMode.Done, true);
      }

      TheOpenFileDialog.Dispose();
   }

}

public class LoadIzvodDLG : Form// VvDialog{
{
   #region Fieldz

   private ToolStripPanel  tsPanel;
   private ToolStrip       ts_izvod;
   private ToolStripButton tsb_okStav, tsb_skip, tsb_abort, tsb_offOpen, tsb_acceptIzv,
                           tsb_previewIzv;
   private ToolStripMenuItem mi_Open, mi_okStav, mi_prew;
   
   #endregion Fieldz

   #region Propertiez

   private NalogDUC TheDUC
   {
      get { return (NalogDUC)ZXC.TheVvForm.TheVvDocumentRecordUC; }
   }

   public LoadIzvodUC TheUC { get; private set; } 
   
   private Form ThePreviewIzvodForm { get; set; }
  
   private OpenFileDialog  TheOpenFileDialog { get; set; }
   
   private CR_PreviewIzvod TheCR_PreviewIzvod { get; set; }

   public bool IsSEPA { get; set; }

   #endregion Propertiez

   #region Constructor

   public LoadIzvodDLG(bool isSEPA)
   {
      this.IsSEPA = isSEPA;

      ZXC.CurrentForm = this;

      TheUC = new LoadIzvodUC();

      SuspendLayout();

      this.Font        = ZXC.vvFont.BaseFont;
      this.BackColor   = ZXC.vvColors.userControl_BackColor;
      this.MaximizeBox = false;
      
      this.StartPosition = FormStartPosition.Manual;
      this.Text          = "Učitaj Izvod";

      CreateToolStripIzvod();
      CreateTheUC();

      this.ClientSize = new Size(TheUC.Width , TheUC.Height + ts_izvod.Height);
      this.Location   = new Point(SystemInformation.WorkingArea.Width - this.Width, 0);
      
      EnableDisabled_AB_CD(false);

      this.Load += new EventHandler(LoadIzvodDLG_Load);
      this.FormClosing += new FormClosingEventHandler(LoadIzvodDLG_FormClosing_ClosePreviewForm);

      if(this.DialogResult == DialogResult.Cancel) ThePreviewIzvodForm.Close();

      this.FormClosing += new FormClosingEventHandler(TheUC.FormClosing_GetAlfaFields);

      this.FormClosing +=new FormClosingEventHandler(RestoreZxcCurrentForm_FormClosing);

      ResumeLayout();

      #region FileDialog

      TheOpenFileDialog                  = new OpenFileDialog();
      TheOpenFileDialog.InitialDirectory = TheUC.Fld_A_DirectoryName;
      //TheOpenFileDialog.Filter = "ZNP datoteke (*.txt, *.ZAP, *.ZABA)|*.txt;*.ZAP;*.ZABA|All files (*.*)|*.*";
      if(IsSEPA)
      {
         TheOpenFileDialog.Filter = "SEPA IZVOD files (*.xml)|*.xml|All files (*.*)|*.*";
      }
      else
      {
         TheOpenFileDialog.Filter = "IZVOD files (*.txt, *.wri, *.dat)|*.txt;*.wri;*.dat|All files (*.*)|*.*";
      }
      TheOpenFileDialog.FilterIndex      = 1;
      TheOpenFileDialog.RestoreDirectory = true;


      #endregion FileDialog

      VvHamper.AttachEscPressForEachControl(this);

      TheUC.OtsInfoSelectionList = new List<OtsTipBrGroupInfo>();

      ZXC.LoadIzvodDLG_isON = true;
   }

   #endregion Constructor

   #region ToolStrip+Button

   private void CreateToolStripIzvod()
   {
      tsPanel           = new ToolStripPanel();
      tsPanel.Parent    = this;
      tsPanel.Dock      = DockStyle.Top;
      tsPanel.Name      = "tsPanel_Izvod";
      tsPanel.BackColor = ZXC.vvColors.tsPanel_BackColor;

      ts_izvod                  = new ToolStrip();
      ts_izvod.Parent           = tsPanel;
      ts_izvod.Name             = "ts_izvod";
      ts_izvod.ShowItemToolTips = true;
      ts_izvod.GripStyle        = ToolStripGripStyle.Hidden;
      ts_izvod.BackColor        = ZXC.vvColors.tsPanel_BackColor;

      MenuStrip menu = new MenuStrip();
      menu.Parent    = this;
      menu.Visible   = false;

      tsb_offOpen             = new ToolStripButton("Otvori", VvIco.OffOpen32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.OffOpen.ico")), 32, 32)*/.ToBitmap(), new EventHandler(OpenButton_Click), "tsb_offOpen");
      tsb_offOpen.ToolTipText = "Otvori datoteku";
      ts_izvod.Items.Add(tsb_offOpen);
      
      mi_Open = new ToolStripMenuItem("Otvori", null, new EventHandler(OpenButton_Click), Keys.Control | Keys.O);
      menu.Items.Add(mi_Open);

      tsb_acceptIzv             = new ToolStripButton("Start", VvIco.Next32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.next.ico")), 32, 32)*/.ToBitmap(), new EventHandler(StartButton_Click), "tsb_acceptIzv");
      tsb_acceptIzv.ToolTipText = "Započni kreiranje naloga odobranog izvoda";
      ts_izvod.Items.Add(tsb_acceptIzv);

      tsb_okStav             = new ToolStripButton("Prihvati", VvIco.IzvOk32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.IzvOk.ico")), 32, 32)*/.ToBitmap(), new EventHandler(PrihvatiStavkuButton_Click), "tsb_okStav");
      tsb_okStav.ToolTipText = "Prihvat stavke izvoda";
      ts_izvod.Items.Add(tsb_okStav);
      mi_okStav = new ToolStripMenuItem("Prihvati", null, new EventHandler(PrihvatiStavkuButton_Click), Keys.Control | Keys.S);
      menu.Items.Add(mi_okStav);

      tsb_skip             = new ToolStripButton("Preskok", VvIco.Skip32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.skip.ico")), 32, 32)*/.ToBitmap(), new EventHandler(SkipStavkaButton_Click), "tsb_skip");
      tsb_skip.ToolTipText = "Preskoci ovu stavku";
      ts_izvod.Items.Add(tsb_skip);

      tsb_previewIzv             = new ToolStripButton("Pogledaj", VvIco.PrintPrw32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.printPrw.ico")), 32, 32)*/.ToBitmap(), new EventHandler(PreviewIzvodButton_Click), "tsb_previewIzv");
      tsb_previewIzv.ToolTipText = "Pogledaj izvod";
      ts_izvod.Items.Add(tsb_previewIzv);

      mi_prew = new ToolStripMenuItem("Pogledaj", null, new EventHandler(PreviewIzvodButton_Click), Keys.Alt | Keys.A);
      menu.Items.Add(mi_prew);

      tsb_abort             = new ToolStripButton("Prekid", VvIco.Esc32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.esc.ico")), 32, 32)*/.ToBitmap(), new EventHandler(AbortIzvodButton_Click), "tsb_abort");
      tsb_abort.ToolTipText = "Odustani od učitavanja izvoda";
      ts_izvod.Items.Add(tsb_abort);

      foreach(ToolStripButton tsb in ts_izvod.Items)
      {
         tsb.ImageScaling      = ToolStripItemImageScaling.None;
         tsb.DisplayStyle      = ToolStripItemDisplayStyle.ImageAndText;
         tsb.TextImageRelation = TextImageRelation.ImageAboveText;
         tsb.Font              = ZXC.vvFont.ToolStripBtnFont;
      }
   }

   #endregion ToolStrip+Button

   #region TheUC

   private void CreateTheUC()
   {
      TheUC.Parent   = this;
      TheUC.Location = new Point(0, ts_izvod.Bottom);
   }

   #endregion TheUC

   #region FormClosing + Load

   void LoadIzvodDLG_Load(object sender, EventArgs e)
   {
      ZXC.CurrentForm = this;

      // 01.04.2026:|
      ZXC.LoadIzvodDLG_isON = true;
   }

   private void LoadIzvodDLG_FormClosing_ClosePreviewForm(object sender, FormClosingEventArgs e)
   {
      if(ThePreviewIzvodForm != null) ThePreviewIzvodForm.Close();
   }

   private void ThePreviewIzvodForm_FormClosing_EnabledPreviewButton(object sender, FormClosingEventArgs e)
   {
      tsb_previewIzv.Enabled = true;

      // ovo se mora napraviti jer je samo jedan ts_Report 
      ZXC.TheVvForm.ts_Report.Parent = ZXC.TheVvForm.tsPanel_SubModul;
      ZXC.TheVvForm.ts_Report.Visible = false;
   }

   void RestoreZxcCurrentForm_FormClosing(object sender, FormClosingEventArgs e)
   {
      ZXC.CurrentForm = ZXC.TheVvForm;

      // 01.04.2026:|
      ZXC.LoadIzvodDLG_isON = false;
   }

   #endregion FormClosing + Load

   #region Buttons_clik

   private void StartButton_Click(object sender, EventArgs e)
   {
      bool OK;

      OK = TheUC.Open_Load_PutFields(TheUC.Fld_A_DirectoryName + @"\" + TheUC.Fld_A_FileName);

      if(!OK) return;

      EnableDisabled_AB_CD(true);

      VvHamper.Open_Close_Fields_ForWriting(TheUC.TheGrid, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvOtherUC);

      //================================================== 
      //=== Here we go! ================================== 
      //================================================== 

      ZXC.TheVvForm.NewRecord_OnClick(this, EventArgs.Empty);

      TheDUC.Fld_DokDate  = TheUC.TheIzvod.HeadRecord.iz_date;
      TheDUC.Fld_Napomena = "IZV-" + TheUC.TheIzvod.HeadRecord.iz_br.ToString("000") + ", " + TheUC.TheIzvod.NumOfTransLines + " stavaka.";
      TheDUC.Fld_TipTran  = Nalog.IZ_TT;
      TheDUC.Fld_TtOpis   = ZXC.luiListaNalogTT.GetNameForThisCd(Nalog.IZ_TT);
      TheDUC.Fld_TtNum    = ZXC.NalogDao.GetNextTtNum(ZXC.TheVvForm.TheDbConnection, Nalog.IZ_TT, null);

      TheUC.CurrentTransRowIdx = 0;

      TheUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);
      TheUC.SetSifrarAndAutocomplete<Kplan> (null, VvSQL.SorterType.Konto/*Code*/);

      if(TheUC.Fld_A_IsPrmPrvoKnj == true) TheUC.DumpPrometIzvodaOnDucDgv();

      // 20.04.2012:
      try
      {
         TheUC.PresumeAndPutDGVfields(TheUC.TheIzvod.Transes[TheUC.CurrentTransRowIdx]);
      }
      catch(Exception ex)
      {
         System.Windows.Forms.MessageBox.Show("PresumeAndPutDGVfields PROBLEM:\n\n" + ex.Message);
      }

      // 22.11.2016: 
      if(TheUC.Fld_A_IsQuickLoad)
      {
         for(int i = 0; i < TheUC.TheIzvod.NumOfTransLines; ++i )
         {
            PrihvatiStavkuButton_Click("QuickLoad", EventArgs.Empty);
         }
      }
   }

   private void OpenButton_Click(object sender, EventArgs e)
   {
      if(TheOpenFileDialog.ShowDialog() == DialogResult.OK)
      {
         TheUC.Open_Load_PutFields(TheOpenFileDialog.FileName);
      }

      ZXC.LastExportFileName = TheOpenFileDialog.FileName;

      TheOpenFileDialog.Dispose();
   }

   private void PreviewIzvodButton_Click(object sender, EventArgs e)
   {
      SuspendLayout();

      #region Set Form

      ThePreviewIzvodForm = new Form();

      ThePreviewIzvodForm.Show();

      tsb_previewIzv.Enabled = false;

      ThePreviewIzvodForm.Font      = ZXC.vvFont.BaseFont;
      ThePreviewIzvodForm.BackColor = ZXC.vvColors.userControl_BackColor;


      ThePreviewIzvodForm.FormClosing += new FormClosingEventHandler(ThePreviewIzvodForm_FormClosing_EnabledPreviewButton);

      //this.Location                = new Point(0, 0);
      //ThePreviewIzvodForm.Location = new Point(this.Right, 0);
      ThePreviewIzvodForm.Location = Point.Empty;
      ThePreviewIzvodForm.Size     = new Size(SystemInformation.WorkingArea.Width - this.Width, SystemInformation.WorkingArea.Height);
      this.Location                = new Point(ThePreviewIzvodForm.Right, 0);

      #endregion Set Form

      #region Set VvPreviewReportUC

      VvStandAloneReportViewerUC thePreviewIzvodUC = new VvStandAloneReportViewerUC();

      thePreviewIzvodUC.Parent = ThePreviewIzvodForm;
      thePreviewIzvodUC.Dock   = DockStyle.Fill;

      #endregion Set VvPreviewReportUC

      ResumeLayout();

      #region FillDataset, CreateReportDocument, SetDataSource, AssignReportSource

      TheUC.FillIzvodDataSet(TheUC.TheIzvod);

      TheCR_PreviewIzvod = new CR_PreviewIzvod();

      thePreviewIzvodUC.SetDataSource_And_AssignReportSource(TheCR_PreviewIzvod, TheUC.TheDS_ZapIzvod);

      #endregion FillDataset, CreateReportDocument, SetDataSource, AssignReportSource

      ZXC.TheVvForm.ToolStripReportVisible_tsBtnOnTsReportVisible(false, ZXC.ReportMode.Done, false);

   }

   private void SkipStavkaButton_Click(object sender, EventArgs e)
   {
      PrihvatiStavkuButton_Click(null, EventArgs.Empty);
   }

   private void PrihvatiStavkuButton_Click(object sender, EventArgs e)
   {
      // 06.04.2017: preselio sa dna, na vrh ove metode
      TheUC.TheGrid.EndEdit(); // !!! 

      if(sender != null) // if IS null, means 'SkipStavkaButton...'
      {
         TheUC.DumpDGV2DGV();

         TheDUC.PutDgvTransSumFields();
      }

      TheUC.CurrentTransRowIdx++;

      if(TheUC.CurrentTransRowIdx < TheUC.TheIzvod.NumOfTransLines)
      {
         TheUC.PutGamaFields(TheUC.TheIzvod.Transes[TheUC.CurrentTransRowIdx]);

         TheUC.PresumeAndPutDGVfields(TheUC.TheIzvod.Transes[TheUC.CurrentTransRowIdx]);

         TheUC.OtsInfoSelectionList.Clear();
      }
      else // Kraj izvoda 
      {
         if(TheUC.Fld_A_IsPrmPrvoKnj == false) TheUC.DumpPrometIzvodaOnDucDgv();

         TheDUC.PutDgvTransSumFields();

         this.Close();

         if(TheUC.Fld_A_IsAutoSave == true) // TODO! via prefs 
         {
            ZXC.TheVvForm.SaveRecord_OnClick(this, EventArgs.Empty);
         }
      }

      // 06.04.2017: preselio sa dna, na vrh ove metode
      //TheUC.TheGrid.EndEdit();
   }

   private void AbortIzvodButton_Click(object sender, EventArgs e)
   {
      this.Close();

      ZXC.TheVvForm.EscapeAction_OnClick(this, EventArgs.Empty);
   }

   #endregion Buttons_clik

   #region Methods

   private void EnableDisabled_AB_CD(bool isIzvAccept)
   {
      VvHamper.Enable_Disable_Fields_ForWriting(TheUC.panel_StavkeIzv, isIzvAccept);
      VvHamper.Enable_Disable_Fields_ForWriting(TheUC.panel_DataGrid , isIzvAccept);


      tsb_okStav.Enabled    = mi_okStav.Enabled = isIzvAccept;
      tsb_skip.Enabled      = isIzvAccept;
      tsb_abort.Enabled     = isIzvAccept;
      tsb_acceptIzv.Enabled = !isIzvAccept;
      tsb_offOpen.Enabled   = mi_Open.Enabled = !isIzvAccept;

      if(isIzvAccept)
      {
         VvHamper.Open_Close_Fields_ForWriting(TheUC.panel_Datoteka, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      }
      else
      {
         VvHamper.Open_Close_Fields_ForWriting(TheUC.panel_Datoteka, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
      }
   }

   #endregion Methods
}

public class LoadIzvodUC : VvOtherUC
{
   #region Fieldz

   public  Panel panel_Datoteka, panel_IzvodInfo, panel_StavkeIzv, panel_DataGrid;
   private VvHamper hamp_Datoteka, hamp_kontoZiro, hamp_knjiz, hamp_redak, hamp_cbxAutoSave,
                     hamp_izvodInfoBroj, hamp_izvodInfoKorisnik, hamp_izvodInfoPromet, hamp_DIFF,
                     hamp_StavkaIzvoda,
                     hamp_nazivi;
   private int dlgWidth, dlgHeight, nextX, nextY, razmak;

   private VvTextBox tbx_fileName, tbx_directoryName, tbx_kontoZiro, tbx_nepozDug, tbx_nepozPot,
                     tbx_iz_br, tbx_iz_date, tbx_iz_firma, tbx_iz_zap_name, tbx_iz_ziro_bdi,
                     tbx_iz_ziro_rnpri, tbx_saldoKtaDug, tbx_saldoKtaPot, tbx_iz_prev_saldoDug,
                     tbx_iz_prev_saldoPot, tbx_iz_saldoDug, tbx_iz_saldoPot, tbx_iz_uk_dug,
                     tbx_iz_uk_pot, tbx_rbr, tbx_kd_ziro, tbx_kd_bankName, tbx_pnb0_zad, tbx_pnb0_odob, tbx_pnb1_zad, tbx_pnb1_odob,
                     tbx_svrha_doz, tbx_t_dug, tbx_t_pot, tbx_numOfTrans, tbx_kontoIsti,
                     tbx_razlDug, tbx_razlPot,
                     tbx_nazivKonta, tbx_nazivKupDob, tbx_devName, tbx_devTec;

   private VvTextBox vvtbT_konto, vvtbT_opis, vvtbT_kupdob_cd, vvtbT_ticker, vvtbT_mtros_cd, vvtbT_mtros_tk,
                     vvtbT_tipBr, vvtbT_dug, vvtbT_pot, vvtbT_projektCD, vvtbT_pozicija, vvtbT_fond;

   private VvTextBoxColumn colVvText;
   private VvDateTimePickerColumn colDate;
   private DataGridViewTextBoxColumn col;
   private RadioButton rbt_frstKnj, rbt_lastKnj, rbt_jedanRed, rbt_dvaRed;
   private Label lbl_diff;

   private CheckBox cbx_AutoSave, cbx_quickLoad;
   protected KtoShemaDsc KSD;

   #endregion Fieldz

   #region Constructor

   public LoadIzvodUC()
   {
    //KSD = new KtoShemaDsc(ZXC.dscLuiLst_KtoShema);
      KSD = ZXC.KSD;

      #region UC Initialization

      SuspendLayout();
      
      CreatePanelsForHampers();

      nextX = nextY = ZXC.Qun4;
      razmak = ZXC.QUN;
      InitializeHamperDatoteka(out hamp_Datoteka);

      nextX = hamp_Datoteka.Right + razmak / 2;
      InitializeHamperKontoZiro(out hamp_kontoZiro);

      nextX = hamp_kontoZiro.Right + razmak / 2;
      InitializeHamperKontoKnjizenje(out hamp_knjiz);

      nextX = hamp_knjiz.Right + razmak / 2;
      InitializeHamperKontoRedak(out hamp_redak);

      nextX = hamp_kontoZiro.Left;
      nextY = hamp_knjiz.Bottom;
      InitializeHamperAutomatskiUsnimi(out hamp_cbxAutoSave);

      nextX = ZXC.QunMrgn;
      nextY = ZXC.Qun4;
      InitializeHamperizvodInfo_broj(out hamp_izvodInfoBroj);

      nextX = hamp_izvodInfoBroj.Right + razmak;
      InitializeHamperizvodInfo_korisnik(out hamp_izvodInfoKorisnik);

      nextX = hamp_izvodInfoKorisnik.Right + razmak;
      InitializeHamperizvodInfo_promet(out hamp_izvodInfoPromet);

      nextX = ZXC.QunMrgn;
      nextY = hamp_izvodInfoPromet.Bottom;
      InitializeHamperDIFF(out hamp_DIFF);

      nextX = ZXC.QunMrgn;
      nextY = ZXC.Qun4;
      InitializeHamperStavkaIzvoda(out hamp_StavkaIzvoda);

      nextX = ZXC.QunMrgn;
      CreateTheGrid();

      nextX += TheGrid.RowHeadersWidth;
      nextY = TheGrid.Bottom;
      InitializeHamperNazivi(out hamp_nazivi);

      nextY = ZXC.Qun4;
      LocationAndSize_PanelsForhampers_AndClientSize();

      ResumeLayout();

      #endregion UC Initialization

      PutAlfaFields(ZXC.TheVvForm.VvPref.zapIzvod);


      uint izvodNumWeWant = ZXC.NalogDao.GetNextTtNum(ZXC.TheVvForm.TheDbConnection, "IZ", null);
      string suggestedIzvodFileName = ZapIzvod.GetIzvodFileName_ContainingIzvodNumWeWant(Fld_A_DirectoryName, izvodNumWeWant);

      if(suggestedIzvodFileName.NotEmpty())
      {
         Open_Load_PutFields(suggestedIzvodFileName);
      }

      //TheDUC = ZXC.TheVvForm.TheVvDocumentRecordUC;
   }

   #endregion Constructor

   #region Propertiez

   public  Vektor.DataLayer.DS_Reports.DS_ZapIzvodNEW TheDS_ZapIzvod     { get; set; }

   //private Crownwood.DotNetMagic.Forms.DotNetMagicForm ThePreviewIzvodForm { get; set; }

   public VvDataGridView TheGrid { get; set; }

   public ZapIzvod TheIzvod { get; set; }

   private decimal DIFF_PrevSaldoPrgVsIzv { get; set; }

   private decimal previousSaldoInProgram;
   private decimal PreviousSaldoInProgram
   {
      get 
      {
         return previousSaldoInProgram; 
      }
      set
      {
         previousSaldoInProgram = value;
         if(TheIzvod != null)
         {
            DIFF_PrevSaldoPrgVsIzv = value - GetPrevIzvodSaldoNonAbsolute(TheIzvod);
         }
      }
   }

   private VvDaoBase TheVvDaoTrans
   {
      get { return ZXC.FtransDao; }
   }

   private NalogDUC TheDUC
   {
      get { return (NalogDUC)ZXC.TheVvForm.TheVvDocumentRecordUC; }
   }

   private FtransDao.FtransCI DB_Tci
   {
      get { return ZXC.FtrCI; }
   }

   //private VvDocumentRecordUC TheDUC { get; set; }

   public int CurrentTransRowIdx { get; set; }

   public List<OtsTipBrGroupInfo> OtsInfoSelectionList { get; set; }

   //private bool isQuickLoad;
   //public bool IsQuickLoad
   //{
   //   get { return true/*isQuickLoad*/; }
   //   set { isQuickLoad = value; }
   //}

   #endregion Propertiez

   #region Panels

   #region PanelsForHampers

   private void CreatePanelsForHampers()
   {
      panel_Datoteka  = new Panel();
      panel_IzvodInfo = new Panel();
      panel_StavkeIzv = new Panel();
      panel_DataGrid  = new Panel();

      panel_Datoteka.Parent  =
      panel_IzvodInfo.Parent =
      panel_StavkeIzv.Parent =
      panel_DataGrid.Parent  = this;

      panel_Datoteka.BorderStyle  =
      panel_DataGrid.BorderStyle  = //BorderStyle.None;
      panel_IzvodInfo.BorderStyle =
      panel_StavkeIzv.BorderStyle = BorderStyle.FixedSingle;

   }

   private void LocationAndSize_PanelsForhampers_AndClientSize()
   {
      int razmak = ZXC.Qun8;

      dlgWidth = TheGrid.Width + 2 * ZXC.QUN + ZXC.QunMrgn;

      panel_Datoteka.Size  = new Size(dlgWidth, hamp_cbxAutoSave.Bottom   + nextY);
      panel_IzvodInfo.Size = new Size(dlgWidth, hamp_DIFF.Bottom          + nextY);
      panel_StavkeIzv.Size = new Size(dlgWidth, hamp_StavkaIzvoda.Bottom  + nextY);
      panel_DataGrid.Size  = new Size(dlgWidth, hamp_nazivi.Bottom        + nextY + ZXC.QUN);

      panel_Datoteka.Location  = new Point(0, 0                             );
      panel_IzvodInfo.Location = new Point(0, panel_Datoteka.Bottom + razmak*3);
      panel_StavkeIzv.Location = new Point(0, panel_IzvodInfo.Bottom );
      panel_DataGrid.Location  = new Point(0, panel_StavkeIzv.Bottom + razmak*3);

      dlgHeight       = panel_DataGrid.Bottom;
      this.Size       = new Size(dlgWidth, dlgHeight);

      VvHamper.Open_Close_Fields_ForWriting(panel_IzvodInfo, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(panel_StavkeIzv, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);

      panel_IzvodInfo.BackColor =
      panel_StavkeIzv.BackColor = Color.FromArgb(255, 224, 192);
   }

   #endregion PanelsForHampers

   #region panel_Datoteka

   private void InitializeHamperDatoteka(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 4, "", panel_Datoteka, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q10un + ZXC.Q4un, ZXC.Q2un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.QUN ,            ZXC.Qun4,            ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbDat = hamper.CreateVvLabel(0, 0, "Datoteka:", ContentAlignment.MiddleRight);
      Label lbDir = hamper.CreateVvLabel(0, 1, "Directory:", ContentAlignment.MiddleRight);

      tbx_fileName      = hamper.CreateVvTextBox(1, 0, "tbx_fileName"     , "");
      tbx_directoryName = hamper.CreateVvTextBox(1, 1, "tbx_directoryName", "");

      tbx_directoryName.JAM_ReadOnly = true;

      cbx_quickLoad = hamper.CreateVvCheckBox_OLD(0, 3, null, 1, 0, "Automatski učitaj cijeli nalog bez pojedinačnih potvrda", RightToLeft.Yes);

   }

   private void InitializeHamperKontoZiro(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 3, "", panel_Datoteka, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q2un, ZXC.Q3un, ZXC.Q2un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label kto = hamper.CreateVvLabel(0, 0, "Konto žiro-računa:", 2, 0, ContentAlignment.MiddleLeft);
      Label kta = hamper.CreateVvLabel(0, 1, "Konta za nepoznatu vrstu prometa:", 3, 0, ContentAlignment.MiddleLeft);
      Label dug = hamper.CreateVvLabel(0, 2, "DUG:", ContentAlignment.MiddleRight);
      Label pot = hamper.CreateVvLabel(2, 2, "POT:", ContentAlignment.MiddleRight);

      tbx_kontoZiro = hamper.CreateVvTextBox(3, 0, "tbx_kontoZiro", "", 6);
      tbx_kontoZiro.TextChanged += new EventHandler(tbx_kontoZiro_TextChanged_UpdateSaldo);
      tbx_nepozDug = hamper.CreateVvTextBox(1, 2, "tbx_nepozDug", "", 6);
      tbx_nepozPot = hamper.CreateVvTextBox(3, 2, "tbx_nepozPot", "", 6);

      tbx_kontoZiro.JAM_CharEdits =
      tbx_nepozDug.JAM_CharEdits  =
      tbx_nepozPot.JAM_CharEdits  = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_kontoZiro.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_nepozDug.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_nepozPot.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);

   }

   private void InitializeHamperKontoKnjizenje(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", panel_Datoteka, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q5un + ZXC.Qun2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lb    = hamper.CreateVvLabel      (0, 0, "Promet izvoda je:", ContentAlignment.MiddleLeft);
      rbt_frstKnj = hamper.CreateVvRadioButton(0, 1, null, "Prvo knjiženje", TextImageRelation.ImageBeforeText);
      rbt_lastKnj = hamper.CreateVvRadioButton(0, 2, null, "Zadnje knjiženje", TextImageRelation.ImageBeforeText);
      rbt_frstKnj.Checked = true;
   }

   private void InitializeHamperKontoRedak(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", panel_Datoteka, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lb     = hamper.CreateVvLabel(0, 0, "Promet izvoda u:", ContentAlignment.MiddleLeft);
      rbt_jedanRed = hamper.CreateVvRadioButton(0, 1, null, "Jednom redu", TextImageRelation.ImageBeforeText);
      rbt_dvaRed   = hamper.CreateVvRadioButton(0, 2, null, "Dva reda", TextImageRelation.ImageBeforeText);
      rbt_jedanRed.Checked = true;
   }

   private void InitializeHamperAutomatskiUsnimi(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", panel_Datoteka, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { hamp_knjiz.Width + hamp_kontoZiro.Width + ZXC.Q2un -ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbx_AutoSave = hamper.CreateVvCheckBox_OLD(0, 0, null, "Automatski usnimi nalog nakon zadnje stavke izvoda", RightToLeft.Yes);
   }

   #endregion panel_Datoteka

   #region panel_IzvodInfo

   private void InitializeHamperizvodInfo_broj(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 6, "", panel_IzvodInfo, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q3un + ZXC.Qun2, ZXC.Q3un, ZXC.Q2un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN - ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.QUN , ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] {                  0,           ZXC.Qun4,           ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbBroj = hamper.CreateVvLabel(0, 1, "Broj:"        , ContentAlignment.MiddleRight);
      Label lbDatu = hamper.CreateVvLabel(0, 2, "Datum:"       , ContentAlignment.MiddleRight);
      Label lbBrTr = hamper.CreateVvLabel(0, 3, "Broj stavaka:", ContentAlignment.MiddleRight);
      lbBroj.Font = lbDatu.Font = ZXC.vvFont.BaseFont;

                     hamper.CreateVvLabel(0, 4, "Valuta:", ContentAlignment.MiddleRight);
                     hamper.CreateVvLabel(0, 5, "Tečaj:", ContentAlignment.MiddleRight);


      tbx_iz_br      = hamper.CreateVvTextBox(1, 1, "tbx_iz_br"     , "");
      tbx_iz_date    = hamper.CreateVvTextBox(1, 2, "tbx_iz_date"   , "", 11, 1, 0);
      tbx_numOfTrans = hamper.CreateVvTextBox(1, 3, "tbx_numOfTrans", "");
      
      tbx_devName = hamper.CreateVvTextBox(1, 4, "tbx_devName", "");
      tbx_devTec  = hamper.CreateVvTextBox(1, 5, "tbx_devTec", "", 12, 1, 0);
      tbx_devTec.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      tbx_iz_br.Font   =
      tbx_iz_date.Font = ZXC.vvFont.LargeBoldFont;

      tbx_iz_br.JAM_ReadOnly      =
      tbx_iz_date.JAM_ReadOnly    =
      tbx_numOfTrans.JAM_ReadOnly = true;
   }

   private void InitializeHamperizvodInfo_korisnik(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 5, "", panel_IzvodInfo, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q8un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lb1 = hamper.CreateVvLabel(0, 1, "Naziv korisnika:"  , ContentAlignment.MiddleRight);
      Label lb2 = hamper.CreateVvLabel(0, 2, "Vodeci broj banke:", ContentAlignment.MiddleRight);
      Label lb3 = hamper.CreateVvLabel(0, 3, "Racun korisnika:"  , ContentAlignment.MiddleRight);
      Label lb4 = hamper.CreateVvLabel(0, 4, "Naziv banke:"      , ContentAlignment.MiddleRight);

      tbx_iz_firma      = hamper.CreateVvTextBox(1, 1, "tbx_iz_firma", "");
      tbx_iz_ziro_bdi   = hamper.CreateVvTextBox(1, 2, "tbx_iz_ziro_bdi", "");
      tbx_iz_ziro_rnpri = hamper.CreateVvTextBox(1, 3, "tbx_iz_rnpri", "");
      tbx_iz_zap_name   = hamper.CreateVvTextBox(1, 4, "tbx_zap_name", "");

      tbx_iz_firma.JAM_ReadOnly      =
      tbx_iz_ziro_bdi.JAM_ReadOnly   =
      tbx_iz_ziro_rnpri.JAM_ReadOnly =
      tbx_iz_zap_name.JAM_ReadOnly   = true;

   }

   private void InitializeHamperizvodInfo_promet(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 5, "", panel_IzvodInfo, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q4un + ZXC.Qun2, ZXC.Q3un, ZXC.Q5un, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] {            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label dug = hamper.CreateVvLabel(2, 0, "Duguje"   , ContentAlignment.MiddleRight);
      Label pot = hamper.CreateVvLabel(3, 0, "Potražuje", ContentAlignment.MiddleRight);

      Label psk = hamper.CreateVvLabel(0, 1, "Preth. saldo kta:"             , ContentAlignment.MiddleRight);
      Label pss = hamper.CreateVvLabel(0, 2, "Prethodni saldo izvoda:" , 1, 0, ContentAlignment.MiddleRight);
      Label prm = hamper.CreateVvLabel(0, 3, "Promet izvoda:"          , 1, 0, ContentAlignment.MiddleRight);
      Label nsl = hamper.CreateVvLabel(0, 4, "Novi saldo izvoda:"      , 1, 0, ContentAlignment.MiddleRight);

      tbx_kontoIsti        = hamper.CreateVvTextBox(1, 1, "tbx_kontoIsti"       , "");
      tbx_saldoKtaDug      = hamper.CreateVvTextBox(2, 1, "tbx_saldoKtaDug"     , "", 12);
      tbx_saldoKtaPot      = hamper.CreateVvTextBox(3, 1, "tbx_saldoKtaPot"     , "", 12);
      tbx_iz_prev_saldoDug = hamper.CreateVvTextBox(2, 2, "tbx_iz_prev_saldoDug", "", 12);
      tbx_iz_prev_saldoPot = hamper.CreateVvTextBox(3, 2, "tbx_iz_prev_saldoPot", "", 12);
      tbx_iz_uk_dug        = hamper.CreateVvTextBox(2, 3, "tbx_iz_uk_dug"       , "", 12);
      tbx_iz_uk_pot        = hamper.CreateVvTextBox(3, 3, "tbx_iz_uk_pot"       , "", 12);
      tbx_iz_saldoDug      = hamper.CreateVvTextBox(2, 4, "tbx_iz_saldoDug"     , "", 12);
      tbx_iz_saldoPot      = hamper.CreateVvTextBox(3, 4, "tbx_iz_saldoPot"     , "", 12);

      tbx_kontoIsti.JAM_ReadOnly        =
      tbx_saldoKtaDug.JAM_ReadOnly      =
      tbx_saldoKtaPot.JAM_ReadOnly      =
      tbx_iz_prev_saldoDug.JAM_ReadOnly =
      tbx_iz_prev_saldoPot.JAM_ReadOnly =
      tbx_iz_uk_dug.JAM_ReadOnly        =
      tbx_iz_uk_pot.JAM_ReadOnly        =
      tbx_iz_saldoDug.JAM_ReadOnly      =
      tbx_iz_saldoPot.JAM_ReadOnly      = true;

      tbx_saldoKtaDug.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_saldoKtaPot.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      
      tbx_iz_prev_saldoDug.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_iz_prev_saldoPot.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      
      tbx_iz_uk_dug.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_iz_uk_pot.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      
      tbx_iz_saldoDug.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_iz_saldoPot.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

   }

   private void InitializeHamperDIFF(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", panel_IzvodInfo, false);

      hamper.VvColWdt      = new int[] { ZXC.Q10un * 2, ZXC.Q5un, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { 0 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Location = new Point(hamp_izvodInfoPromet.Right - hamper.Width, nextY);

      lbl_diff           = hamper.CreateVvLabel(0, 0, "POSTOJI RAZLIKA IZMEDJU PRETHODNOG SALDA IZVODA I SALDA ZIRO-a", ContentAlignment.MiddleRight);
      lbl_diff.ForeColor = Color.Red;

      tbx_razlDug = hamper.CreateVvTextBox(1, 0, "tbx_razlDug", "", 12);
      tbx_razlPot = hamper.CreateVvTextBox(2, 0, "tbx_razlPot", "", 12);

      tbx_razlDug.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_razlPot.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      tbx_razlDug.JAM_ReadOnly =
      tbx_razlPot.JAM_ReadOnly = true;

      tbx_razlDug.Visible = tbx_razlPot.Visible = lbl_diff.Visible = false;
      tbx_razlDug.Tag     = tbx_razlPot.Tag     = "ForeColorColor.Red";
   }

   #endregion panel_IzvodInfo

   #region panel_StavkeIzv

   private void InitializeHamperStavkaIzvoda(out VvHamper hamper)
   {
      hamper = new VvHamper(8, 4, "", panel_StavkeIzv, false, nextX, nextY, 0);

      hamper.VvColWdt = new int[] { ZXC.Q2un + ZXC.Qun2, ZXC.Q10un, ZXC.Q2un, ZXC.Q8un, ZXC.Q2un, ZXC.Q8un, ZXC.Q5un, ZXC.Q5un };
      hamper.VvSpcBefCol = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun10, ZXC.Qun4, ZXC.Qun10, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt = new int[] { ZXC.QUN - ZXC.Qun8, ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow = new int[] { 0, 0, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lb1 = hamper.CreateVvLabel(0, 1, "Red.broj", ContentAlignment.MiddleRight);
      Label lb2 = hamper.CreateVvLabel(1, 0, "Račun i naziv", ContentAlignment.MiddleLeft);
      Label lb3 = hamper.CreateVvLabel(1, 1, "uplatitelja/primatelja", ContentAlignment.MiddleLeft);
      Label lb4 = hamper.CreateVvLabel(4, 0, "Poziv na broj zaduženja", 1, 0, ContentAlignment.MiddleLeft);
      Label lb5 = hamper.CreateVvLabel(2, 0, "Poziv na broj odobrenja", 1, 0, ContentAlignment.MiddleLeft);
      Label lb6 = hamper.CreateVvLabel(2, 1, "Opis prometa / svrha doznake", 2, 0, ContentAlignment.MiddleLeft);
      Label lb7 = hamper.CreateVvLabel(6, 0, "Dugovni", ContentAlignment.MiddleRight);
      Label lb8 = hamper.CreateVvLabel(6, 1, "promet", ContentAlignment.MiddleRight);
      Label lb9 = hamper.CreateVvLabel(7, 0, "Potražni", ContentAlignment.MiddleRight);
      Label lb10 = hamper.CreateVvLabel(7, 1, "promet", ContentAlignment.MiddleRight);

      tbx_rbr = hamper.CreateVvTextBox(0, 2, "tbx_rbr", "");
      tbx_kd_ziro = hamper.CreateVvTextBox(1, 2, "tbx_kd_ziro", "");
      tbx_kd_bankName = hamper.CreateVvTextBox(1, 3, "tbx_kd_bankName", "");
      tbx_pnb0_odob = hamper.CreateVvTextBox(2, 2, "tbx_pnb0_odob", "");
      tbx_pnb1_odob = hamper.CreateVvTextBox(3, 2, "tbx_pnb1_odob", "");
      tbx_pnb0_zad = hamper.CreateVvTextBox(4, 2, "tbx_pnb0_zad", "");
      tbx_pnb1_zad = hamper.CreateVvTextBox(5, 2, "tbx_pnb1_zad", "");
      tbx_svrha_doz = hamper.CreateVvTextBox(2, 3, "tbx_svrha_doz", "", 40, 3, 0);
      tbx_t_dug = hamper.CreateVvTextBox(6, 2, "tbx_t_dug", "", 12);
      tbx_t_pot = hamper.CreateVvTextBox(7, 2, "tbx_t_pot", "", 12);

      tbx_rbr.JAM_ReadOnly =
      tbx_kd_ziro.JAM_ReadOnly =
      tbx_kd_bankName.JAM_ReadOnly =
      tbx_pnb0_odob.JAM_ReadOnly =
      tbx_pnb0_zad.JAM_ReadOnly =
      tbx_pnb1_odob.JAM_ReadOnly =
      tbx_pnb1_zad.JAM_ReadOnly =
      tbx_svrha_doz.JAM_ReadOnly =
      tbx_t_dug.JAM_ReadOnly =
      tbx_t_pot.JAM_ReadOnly = true;

      tbx_t_dug.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_t_pot.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
   }

   #endregion panel_StavkeIzv

   #endregion Panels

   #region TheG

   private void CreateTheGrid()
   {
      TheGrid          = new VvDataGridView();

      TheGrid.ThisIsOneRowFixedGrid = true;

      TheGrid.Parent              = panel_DataGrid;
      TheGrid.Location            = new Point(nextX, nextY+ ZXC.QUN);
      TheGrid.AutoGenerateColumns = false;

      TheGrid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

      TheGrid.RowHeadersBorderStyle = TheGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
      TheGrid.ColumnHeadersHeight   = ZXC.QUN;
      TheGrid.RowTemplate.Height    = ZXC.QUN;
      TheGrid.RowHeadersWidth       = ZXC.Q3un + ZXC.Qun4;
      TheGrid.Height                = TheGrid.ColumnHeadersHeight +TheGrid.RowTemplate.Height;

      TheGrid.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(VvDocumentRecordUC.Grid_EditingControlShowing);
      TheGrid.CellBeginEdit         += new DataGridViewCellCancelEventHandler           (VvDocumentRecordUC.grid_CellBeginEdit_AtachJAM_EventHandlers);
      TheGrid.CellEndEdit           += new DataGridViewCellEventHandler                 (VvDocumentRecordUC.grid_CellEndEdit_DetachJAM_EventHandlers);

      TheGrid.CellFormatting        += new DataGridViewCellFormattingEventHandler       (VvDocumentRecordUC.grid_CellFormatting_FormatVvDateTime);
      TheGrid.Validating            += new System.ComponentModel.CancelEventHandler     (VvDocumentRecordUC.grid_Validating);

      CreateDataGridViewColumnIzv();

      VvHamper.ApplyVVColorAndStyleTabCntrolChange(TheGrid);
      VvHamper.Open_Close_Fields_ForWriting(TheGrid, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);

      // ako hoces samo jedan vidljiv redak onda mora biti ovakav redosljed 1. ogranicenja  a 2.RowCount
      TheGrid.AllowUserToAddRows       =
      TheGrid.AllowUserToDeleteRows    =
      TheGrid.AllowUserToOrderColumns  =
      TheGrid.AllowUserToResizeColumns =
      TheGrid.AllowUserToResizeRows    = false;

      TheGrid.RowCount = 1;
   }

   private void CreateDataGridViewColumnIzv()
   {
      vvtbT_konto                  = TheGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_konto", TheVvDaoTrans, DB_Tci.t_konto, "Konto iz Kontnog Plana");
      vvtbT_konto.JAM_CharEdits    = ZXC.JAM_CharEdits.DigitsOnly;
      vvtbT_konto.JAM_DataRequired = true;
      vvtbT_konto.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, ZXC.AutoCompleteRestrictor.KPL_Analitika_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), new EventHandler(KontoTextBoxLeave));
    //vvtbT_konto.JAM_SetAutoCompleteData(Kplan.recordName, VvSQL.SorterType.KontoNaziv, ZXC.AutoCompleteRestrictor.KPL_Analitika_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_KplanNaziv_sorterCode), null);
    //vvtbT_konto.JAM_FieldExitMethod/*_2*/ = new EventHandler(OnExitKonto_ClearPreffix); ovo mi nekak neide

      
      colVvText = TheGrid.CreateVvTextBoxColumn(vvtbT_konto, TheVvDaoTrans, DB_Tci.t_konto, "Konto", ZXC.Q4un);
      //AddDGVRedakIzvColum("Konto", ZXC.Q4un);

      vvtbT_opis = TheGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_opis", TheVvDaoTrans, DB_Tci.t_opis, "Tekstualni opis knjiženja.");
      colVvText  = TheGrid.CreateVvTextBoxColumn(vvtbT_opis, TheVvDaoTrans, DB_Tci.t_opis, "Opis", 0);
      colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      colVvText.MinimumWidth = ZXC.Q10un;
      //AddDGVRedakIzvColum("Opis", ZXC.Q10un);

      vvtbT_ticker               = TheGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_ticker", TheVvDaoTrans, DB_Tci.t_ticker, "Ticker iz adresara");
      vvtbT_ticker.JAM_CharacterCasing = CharacterCasing.Upper;
      vvtbT_ticker.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      colVvText = TheGrid.CreateVvTextBoxColumn(vvtbT_ticker, TheVvDaoTrans, DB_Tci.t_ticker, "Ticker", ZXC.Q4un);
      //AddDGVRedakIzvColum("Ticker", ZXC.Q4un);

      vvtbT_kupdob_cd = TheGrid.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColT_kupdob_cd", TheVvDaoTrans, DB_Tci.t_kupdob_cd, "Sifra iz adresara");
      vvtbT_kupdob_cd.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupdobTextBoxLeave));
      colVvText = TheGrid.CreateVvTextBoxColumn(vvtbT_kupdob_cd, TheVvDaoTrans, DB_Tci.t_kupdob_cd, "Sif K/D", ZXC.Q4un);
      //AddDGVRedakIzvColum("Sif K/D", ZXC.Q4un);

      vvtbT_tipBr = TheGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_tipBr", TheVvDaoTrans, DB_Tci.t_tipBr, "Vezni dokument iz robnog knjigovodstva");
      //vvtbT_tipBr.JAM_CharacterCasing = CharacterCasing.Upper;
      vvtbT_tipBr.JAM_SetOtsInfoData(/*new EventHandler(TipBrTextBoxLeave)*/);
      vvtbT_tipBr.JAM_FieldEntryMethod = new EventHandler(OnEnterSaveKontoAndKupdobCd);
      colVvText = TheGrid.CreateVvTextBoxColumn(vvtbT_tipBr, TheVvDaoTrans, DB_Tci.t_tipBr, "VeznDok", ZXC.Q4un);

      vvtbT_fond = TheGrid.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_fond", TheVvDaoTrans, DB_Tci.t_fond, "Fond");
      vvtbT_fond.JAM_CharacterCasing = CharacterCasing.Upper;
      vvtbT_fond.JAM_Set_LookUpTable(ZXC.luiListaFinFond, (int)ZXC.Kolona.prva);
      colVvText = TheGrid.CreateVvTextBoxColumn(vvtbT_fond, TheVvDaoTrans, DB_Tci.t_fond, "Fond", ZXC.Q2un);
      colVvText.Visible = (KSD.Dsc_IsVisibleColFond);

      if(KSD.Dsc_IsVisibleColPozicija) VvLookUpLista.LoadResultLuiList_PozicijePlana_PLN_or_RLZ(/* isPLN */ false);

      vvtbT_pozicija = TheGrid.CreateVvTextBoxFor_LookUp_ColumnTemplate("vvtb4ColT_pozicija", TheVvDaoTrans, DB_Tci.t_pozicija, "Pozicija");
      vvtbT_pozicija.JAM_CharacterCasing = CharacterCasing.Upper;
      vvtbT_pozicija.JAM_Set_LookUpTable(ZXC.luiListaPozicijePlanaRLZ, (int)ZXC.Kolona.prva);
      colVvText = TheGrid.CreateVvTextBoxColumn(vvtbT_pozicija, TheVvDaoTrans, DB_Tci.t_pozicija, "Pozicija", ZXC.Q4un);
      colVvText.Visible = (KSD.Dsc_IsVisibleColPozicija);


      vvtbT_projektCD = TheGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_projektCD", TheVvDaoTrans, DB_Tci.t_projektCD, "Projekt");
      vvtbT_projektCD.JAM_CharacterCasing = CharacterCasing.Upper;
      //vvtbT_projektCD.JAM_SetOtsInfoData(/*new EventHandler(TipBrTextBoxLeave)*/);
      //vvtbT_projektCD.JAM_FieldEntryMethod = new EventHandler(OnEnterSaveKontoAndKupdobCd);
      colVvText = TheGrid.CreateVvTextBoxColumn(vvtbT_projektCD, TheVvDaoTrans, DB_Tci.t_projektCD, "Projekt", ZXC.Q4un);
      colVvText.Visible = (TheDUC is NalogProjektDUC || KSD.Dsc_IsVisibleColProjekt);

      //AddDGVRedakIzvColum("VezniDok", ZXC.Q4un);

      //13.03.2014..............................................
      vvtbT_mtros_tk = TheGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColT_mtros_tk", TheVvDaoTrans, DB_Tci.t_mtros_tk, "Ticker iz adresara");
      vvtbT_mtros_tk.JAM_CharacterCasing = CharacterCasing.Upper;
      vvtbT_mtros_tk.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      colVvText = TheGrid.CreateVvTextBoxColumn(vvtbT_mtros_tk, TheVvDaoTrans, DB_Tci.t_mtros_tk, "MjTroška", ZXC.Q4un);
      colVvText.Visible = (TheDUC is NalogMtrDUC || KSD.Dsc_IsVisibleColMtrosTK);

      vvtbT_mtros_cd = TheGrid.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColT_mtros_cd", TheVvDaoTrans, DB_Tci.t_mtros_cd, "Šifra iz adresara");
      vvtbT_mtros_cd.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyMtrosTextBoxLeave));
      colVvText = TheGrid.CreateVvTextBoxColumn(vvtbT_mtros_cd, TheVvDaoTrans, DB_Tci.t_mtros_cd, "Šif MjTr", ZXC.Q4un);
      colVvText.Visible = (TheDUC is NalogMtrDUC || KSD.Dsc_IsVisibleColMtrosCD);

      colDate = TheGrid.CreateCalendarColumn(TheVvDaoTrans, DB_Tci.t_valuta, "Valuta", ZXC.Q5un + ZXC.Qun4 + ZXC.Qun8);
      //AddDGVRedakIzvColum("Valuta", ZXC.Q5un + ZXC.Qun4 + ZXC.Qun8);

      vvtbT_dug = TheGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColT_dug", TheVvDaoTrans, DB_Tci.t_dug, "Iznos DUGUJE");
      vvtbT_dug.JAM_ShouldSumGrid = true;
      colVvText = TheGrid.CreateVvTextBoxColumn(vvtbT_dug, TheVvDaoTrans, DB_Tci.t_dug, "Duguje", ZXC.Q5un);
      colVvText.MinimumWidth = ZXC.Q5un;             // __mora biti == sum.MinWidth
      //AddDGVRedakIzvColum("Duguje", ZXC.Q5un);

      vvtbT_pot = TheGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColT_pot", TheVvDaoTrans, DB_Tci.t_pot, "Iznos POTRAZUJE");
      vvtbT_pot.JAM_ShouldSumGrid = true;
      colVvText = TheGrid.CreateVvTextBoxColumn(vvtbT_pot, TheVvDaoTrans, DB_Tci.t_pot, "Potrazuje", ZXC.Q5un);
      colVvText.MinimumWidth = ZXC.Q5un;             // __mora biti == sum.MinWidth
      //AddDGVRedakIzvColum("Potražuje", ZXC.Q5un);

      int sumoOfColumns = 0;

      foreach(DataGridViewColumn dc in TheGrid.Columns)
      {

         sumoOfColumns += (dc.Visible ? dc.Width : 0);
      }
      TheGrid.Width = sumoOfColumns + TheGrid.RowHeadersWidth + ZXC.Qun5;

      TheGrid.Columns[4].DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;

      col         = new DataGridViewTextBoxColumn();
      col.Name    = "t_fakRecID";
      col.Visible = false;
      col.Tag     = VvDocumentRecordUC.AlwaysInvinsibleStr;
      TheGrid.Columns.Add(col);

      col         = new DataGridViewTextBoxColumn();
      col.Name    = "t_fakYear";
      col.Visible = false;
      col.Tag     = VvDocumentRecordUC.AlwaysInvinsibleStr;
      TheGrid.Columns.Add(col);

   }
   public void OnExitKonto_ClearPreffix(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvTextBoxEditingControl vvTbKonto = sender as VvTextBoxEditingControl;

      string dirtyString = vvTbKonto.Text, cleanString;

      int spaceIdx = dirtyString.IndexOf(' ');

      if(dirtyString.Length.IsZero() || spaceIdx.IsNegative()) return;

      cleanString = dirtyString.Substring(0, spaceIdx);

      //vvTbKonto.Text = cleanString;
      //TheG.EditingControl.Text = cleanString;
      TheGrid.PutCell(DB_Tci.t_konto, vvTbKonto.EditingControlDataGridView.CurrentRow.Index, cleanString);
   }

   public void OnEnterSaveKontoAndKupdobCd(object sender, EventArgs e)
   {
      VvTextBoxEditingControl vtbec = sender as VvTextBoxEditingControl;
      VvDataGridView          dgv   = (VvDataGridView)vtbec.EditingControlDataGridView;

      int rowIdx = dgv.CurrentRow.Index;
      int colIdx = dgv.CurrentCellAddress.X;

      VvTextBox vvtb = (VvTextBox)(dgv.Columns[colIdx].Tag);

      vtbec.OtsKonto    = vvtb.OtsKonto    = Fld_DGV_Konto;
      vtbec.OtsKupdobCd = vvtb.OtsKupdobCd = Fld_DGV_KupdobCd;
      vtbec.OtsDateDo   = vvtb.OtsDateDo   = TheIzvod.HeadRecord.iz_date;

      decimal t_dug = Fld_DGV_Dug;
      decimal t_pot = Fld_DGV_Pot;

      vtbec.OtsMoney = vvtb.OtsMoney = t_dug.NotZero() ? t_dug : t_pot;
   }

   private void KontoTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

//      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      Kplan kplan_rec;

      VvTextBoxEditingControl vvtb_editingControl = sender as VvTextBoxEditingControl;

      if(vvtb_editingControl == null) return;

      if(vvtb_editingControl.Text != this.originalText)
      {
         this.originalText = vvtb_editingControl.Text;
         kplan_rec = KplanSifrar.Find(FoundInSifrar<Kplan>);

         int currRow = vvtb_editingControl.EditingControlRowIndex;

         if(kplan_rec != null && vvtb_editingControl.Text != "")
         {
            Fld_D_NazivKonta = kplan_rec.Naziv;
         }
         else
         {
            Fld_D_NazivKonta = "";
         }
      }
   }

   private void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

//      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      Kupdob kupdob_rec;

      VvTextBoxEditingControl vvtb_editingControl = sender as VvTextBoxEditingControl;

      if(vvtb_editingControl == null) return;

      if(vvtb_editingControl.Text != this.originalText)
      {
         this.originalText = vvtb_editingControl.Text;
         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         int currRow = vvtb_editingControl.EditingControlRowIndex;

         if(kupdob_rec != null && vvtb_editingControl.Text != "")
         {
            //TheG.PutCell(ci.iT_ticker, currRow, artikl_rec.Ticker);
            Fld_DGV_Ticker    = kupdob_rec.Ticker;
            //TheG.PutCell(ci.iT_kupdob_cd, currRow, artikl_rec.RecID);
            Fld_DGV_KupdobCd  = kupdob_rec.KupdobCD/*RecID*/;
            Fld_D_NazivKupDob = kupdob_rec.Naziv;
         }
         else
         {
            //TheG.PutCell(ci.iT_ticker, currRow, "");
            Fld_DGV_Ticker = "";
            //TheG.PutCell(ci.iT_kupdob_cd, currRow, "");
            Fld_DGV_KupdobCd = 0;

            Fld_D_NazivKupDob = "";

         }
      }
   }

   private void AnyMtrosTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      //if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      Kupdob kupdob_rec;

      VvTextBoxEditingControl vvtb_editingControl = sender as VvTextBoxEditingControl;

      if(vvtb_editingControl == null) return;

      if(vvtb_editingControl.Text != this.originalText)
      {
         this.originalText = vvtb_editingControl.Text;
         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         int currRow = vvtb_editingControl.EditingControlRowIndex;

         if(kupdob_rec != null && vvtb_editingControl.Text != "")
         {
            Fld_DGV_MtrosTK = kupdob_rec.Ticker;
            Fld_DGV_MtrosCd = kupdob_rec.KupdobCD/*RecID*/;
         }
         else
         {
            Fld_DGV_MtrosTK = "";
            Fld_DGV_MtrosCd = 0;
         }
      }
   }



   //private void TipBrTextBoxLeave(object sender, EventArgs e)
   //{
   //   if(isPopulatingSifrar) return;

   //   if(OtsInfoSifrar == null) return;

   //   ZXC.OtsTipBrGroupInfo otsInfo;

   //   VvTextBoxEditingControl vvtb_editingControl = sender as VvTextBoxEditingControl;

   //   if(vvtb_editingControl == null) return;

   //   if(vvtb_editingControl.Text != this.originalText)
   //   {
   //      this.originalText = vvtb_editingControl.Text;
   //      otsInfo = OtsInfoSifrar.SingleOrDefault(ots => ots.TipBr == originalText);

   //      int currRow = vvtb_editingControl.EditingControlRowIndex;

   //      if(otsInfo.TipBr.NotEmpty() && vvtb_editingControl.Text != "")
   //      {
   //         Fld_DGV_TipBr  = otsInfo.TipBr;
   //         Fld_DGV_Valuta = otsInfo.OpenDokumentValuta;
   //      }
   //      else
   //      {
   //         Fld_DGV_TipBr  = "";
   //         Fld_DGV_Valuta = DateTime.MinValue;

   //      }
   //   }
   //}

   private void InitializeHamperNazivi(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", panel_DataGrid, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q10un + ZXC.Q4un, ZXC.Q10un + ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { 0                   , ZXC.Qun5 - ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      tbx_nazivKonta = hamper.CreateVvTextBox(0, 0, "tbx_nazivKonta", "");
      tbx_nazivKupDob = hamper.CreateVvTextBox(1, 0, "tbx_nazivKupDob", "");

      tbx_nazivKonta.JAM_ReadOnly = tbx_nazivKupDob.JAM_ReadOnly = true;

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
   }

   #endregion TheG

   #region Fld_

   //==== A set ======================================================= 

   public string Fld_A_FileName
   {
      get { return tbx_fileName.Text; }
      set { tbx_fileName.Text = value; }
   }

   public string Fld_A_DirectoryName
   {
      get { return tbx_directoryName.Text; }
      set { tbx_directoryName.Text = value; }
   }

   public string Fld_A_KontoZiro
   {
      get { return tbx_kontoZiro.Text; }
      set
      {
         tbx_kontoZiro.Text = value;
         tbx_kontoIsti.Text = value;
      }
   }

   public string Fld_A_NepozDug
   {
      get { return tbx_nepozDug.Text; }
      set { tbx_nepozDug.Text = value; }
   }

   public string Fld_A_NepozPot
   {
      get { return tbx_nepozPot.Text; }
      set { tbx_nepozPot.Text = value; }
   }

   public bool Fld_A_IsPrmPrvoKnj
   {
      get { return rbt_frstKnj.Checked; }
      set
      {
         rbt_frstKnj.Checked = value;
         rbt_lastKnj.Checked = !value;
      }
   }

   public bool Fld_A_IsPrmUJedRed
   {
      get { return rbt_jedanRed.Checked; }
      set
      {
         rbt_jedanRed.Checked = value;
         rbt_dvaRed.Checked = !value;
      }
   }

   public bool Fld_A_IsAutoSave
   {
      get { return cbx_AutoSave.Checked; }
      set { cbx_AutoSave.Checked = value; }
   }

   public bool Fld_A_IsQuickLoad { get { return cbx_quickLoad.Checked; } set { cbx_quickLoad.Checked = value; } }

   //== B set ========================================================= 

   public uint Fld_B_IzBr
   {
      set { tbx_iz_br.Text = value.ToString("000"); }
   }

   public DateTime Fld_B_IzDate
   {
      set { tbx_iz_date.Text = value.ToString(ZXC.VvDateFormat); }
   }

   public int Fld_B_NumOfTrans
   {
      set { tbx_numOfTrans.Text = value.ToString(); }
   }

   public string Fld_B_IzFirma
   {
      set { tbx_iz_firma.Text = value; }
   }

   public string Fld_B_IzZapName
   {
      set { tbx_iz_zap_name.Text = value; }
   }

   public string Fld_B_IzZiroBdi
   {
      set { tbx_iz_ziro_bdi.Text = value; }
   }

   public string Fld_B_IzZiroRnpri
   {
      set { tbx_iz_ziro_rnpri.Text = value; }
   }

   public string Fld_B_KontoIsti
   {
      set { tbx_kontoIsti.Text = value; }
   }

   public decimal Fld_B_SaldoKtaDug
   {
      set { tbx_saldoKtaDug.PutDecimalField(value); }
   }

   public decimal Fld_B_SaldoKtaPot
   {
      set { tbx_saldoKtaPot.PutDecimalField(value); }
   }

   public decimal Fld_B_IzPrevSaldoDug
   {
      set { tbx_iz_prev_saldoDug.PutDecimalField(value); }
   }

   public decimal Fld_B_IzPrevSaldoPot
   {
      set { tbx_iz_prev_saldoPot.PutDecimalField(value); }
   }

   public decimal Fld_B_IzUkDug
   {
      set { tbx_iz_uk_dug.PutDecimalField(value); }
   }

   public decimal Fld_B_IzUkPot
   {
      set { tbx_iz_uk_pot.PutDecimalField(value); }
   }

   public decimal Fld_B_IzSaldoDug
   {
      set { tbx_iz_saldoDug.PutDecimalField(value); }
   }

   public decimal Fld_B_IzSaldoPot
   {
      set { tbx_iz_saldoPot.PutDecimalField(value); }
   }

   public decimal Fld_B_RazlikaDug
   {
      set { tbx_razlDug.PutDecimalField(value); }
   }

   public decimal Fld_B_RazlikaPot
   {
      set { tbx_razlPot.PutDecimalField(value); }
   }

   public string Fld_B_DevName
   {
      set { tbx_devName.Text = value; }
   }

   public decimal Fld_B_DevTec
   {
      set { tbx_devTec.PutDecimalField(value); }
   }

   //== C set ========================================================= 

   public string Fld_C_RbrStavke
   {
      set { tbx_rbr.Text = value; }
   }

   public string Fld_C_KdZiro
   {
      set { tbx_kd_ziro.Text = value; }
   }

   public string Fld_C_KdName
   {
      set { tbx_kd_bankName.Text = value; }
   }

   public string Fld_C_Pnb0Odob
   {
      set { tbx_pnb0_odob.Text = value; }
   }

   public string Fld_C_Pnb1Odob
   {
      set { tbx_pnb1_odob.Text = value; }
   }

   public string Fld_C_Pnb0Zad
   {
      set { tbx_pnb0_zad.Text = value; }
   }

   public string Fld_C_Pnb1Zad
   {
      set { tbx_pnb1_zad.Text = value; }
   }

   public string Fld_C_SvrhaDoz
   {
      set { tbx_svrha_doz.Text = value; }
   }

   public decimal Fld_C_DugStav
   {
      set { tbx_t_dug.PutDecimalField(value); }
   }
   public decimal Fld_C_PotStav
   {
      set { tbx_t_pot.PutDecimalField(value); }
   }

   //==D set ========================================================= 

   public string Fld_D_NazivKonta
   {
      set { tbx_nazivKonta.Text = value; }
   }
   public string Fld_D_NazivKupDob
   {
      set { tbx_nazivKupDob.Text = value; }
   }

   #endregion Fld_

   #region PutFields(), GetFields()

   private void PutAlfaFields(VvPref.ZapIzvodPrefs zapIzvodPrefs)
   {
      Fld_A_DirectoryName = zapIzvodPrefs.DirectoryName;
      Fld_A_KontoZiro     = zapIzvodPrefs.KontoZiro;
      Fld_A_NepozDug      = zapIzvodPrefs.NepozDug;
      Fld_A_NepozPot      = zapIzvodPrefs.NepozPot;
      Fld_A_IsPrmPrvoKnj  = zapIzvodPrefs.IsPrmPrvoKnj;
      Fld_A_IsPrmUJedRed  = zapIzvodPrefs.IsPrmUJedRed;
      Fld_A_IsAutoSave    = zapIzvodPrefs.IsAutoSave;
      Fld_A_IsQuickLoad   = zapIzvodPrefs.IsQuickLoad;
   }

   public void FormClosing_GetAlfaFields(object sender, FormClosingEventArgs e)
   {
      ZXC.TheVvForm.VvPref.zapIzvod.DirectoryName = Fld_A_DirectoryName;
      ZXC.TheVvForm.VvPref.zapIzvod.KontoZiro     = Fld_A_KontoZiro;
      ZXC.TheVvForm.VvPref.zapIzvod.NepozDug      = Fld_A_NepozDug;
      ZXC.TheVvForm.VvPref.zapIzvod.NepozPot      = Fld_A_NepozPot;
      ZXC.TheVvForm.VvPref.zapIzvod.IsPrmPrvoKnj  = Fld_A_IsPrmPrvoKnj;
      ZXC.TheVvForm.VvPref.zapIzvod.IsPrmUJedRed  = Fld_A_IsPrmUJedRed;
      ZXC.TheVvForm.VvPref.zapIzvod.IsAutoSave    = Fld_A_IsAutoSave;
      ZXC.TheVvForm.VvPref.zapIzvod.IsQuickLoad   = Fld_A_IsQuickLoad;
   }

   private void PutSelectedIzvodFileNameFields(ZapIzvod zapIzvod_rec)
   {
      Fld_A_FileName = zapIzvod_rec.FileName;
      Fld_A_DirectoryName = TheIzvod.DirectoryName;

      this.Text = "Učitaj Izvod [" + TheIzvod.FileName + "]";
   }

   private void PutBetaFields(ZapIzvod zapIzvod_rec)
   {
      Fld_B_IzBr        = zapIzvod_rec.HeadRecord.iz_br;
      Fld_B_IzDate      = zapIzvod_rec.HeadRecord.iz_date;
      Fld_B_NumOfTrans  = zapIzvod_rec.NumOfTransLines;
      Fld_B_IzFirma     = zapIzvod_rec.HeadRecord.iz_firma;
      Fld_B_IzZapName   = zapIzvod_rec.HeadRecord.bankaName;
      Fld_B_IzZiroBdi   = zapIzvod_rec.HeadRecord.bankaVbdi;
      Fld_B_IzZiroRnpri = zapIzvod_rec.HeadRecord.iz_ziro_rnpri;
      Fld_B_IzZapName   = zapIzvod_rec.HeadRecord.bankaName;


      Fld_B_IzPrevSaldoDug = GetPrevIzvodSaldoNonAbsolute(zapIzvod_rec) > decimal.Zero ? GetPrevIzvodSaldoNonAbsolute(zapIzvod_rec) : decimal.Zero;
      Fld_B_IzPrevSaldoPot = GetPrevIzvodSaldoNonAbsolute(zapIzvod_rec) < decimal.Zero ? GetPrevIzvodSaldoNonAbsolute(zapIzvod_rec) : decimal.Zero;


      Fld_B_IzUkDug = zapIzvod_rec.HeadRecord.iz_uk_dug;
      Fld_B_IzUkPot = zapIzvod_rec.HeadRecord.iz_uk_pot;

      Fld_B_IzSaldoDug = GetSaldoIzvNonAbsolute(zapIzvod_rec) > decimal.Zero ? GetSaldoIzvNonAbsolute(zapIzvod_rec) : decimal.Zero;
      Fld_B_IzSaldoPot = GetSaldoIzvNonAbsolute(zapIzvod_rec) < decimal.Zero ? GetSaldoIzvNonAbsolute(zapIzvod_rec) : decimal.Zero;


      #region DIFF calc

      DIFF_AndFldAndTbx(zapIzvod_rec);

      #endregion DIFF calc

      PutGamaFields(zapIzvod_rec.Transes[0]);
   }

   public void PutGamaFields(ZapIzvod.TransRecordStruct trans_rec)
   {
      Fld_C_RbrStavke = trans_rec.t_rbr;
      Fld_C_KdZiro    = trans_rec.t_kd_ziro;
      Fld_C_KdName    = trans_rec.t_kupdob;
      Fld_C_Pnb0Odob  = trans_rec.t_pnb0_odob;
      Fld_C_Pnb1Odob  = trans_rec.t_pnb1_odob;
      Fld_C_Pnb0Zad   = trans_rec.t_pnb0_zad;
      Fld_C_Pnb1Zad   = trans_rec.t_pnb1_zad;
      Fld_C_SvrhaDoz  = trans_rec.t_svrha_doz;
      Fld_C_DugStav   = trans_rec.t_dug;
      Fld_C_PotStav   = trans_rec.t_pot;
   }

   #region PutDgvFields, GetDgvFields

   #region DGV Columns as Fld_

   private string GetColName(int colIdx)
   {
      return(TheVvDaoTrans.GetSchemaColumnName(colIdx));
   }

   public string Fld_DGV_Konto
   {
      get { return TheGrid.GetStringCell(GetColName(DB_Tci.t_konto), 0, false); }
      set {        TheGrid.PutCell      (GetColName(DB_Tci.t_konto), 0, value); }
   }

   public string Fld_DGV_Opis
   {
      get { return TheGrid.GetStringCell(GetColName(DB_Tci.t_opis), 0, false); }
      set {        TheGrid.PutCell      (GetColName(DB_Tci.t_opis), 0, value); }
   }

   public string Fld_DGV_Ticker
   {
      get { return TheGrid.GetStringCell(GetColName(DB_Tci.t_ticker), 0, false); }
      set {        TheGrid.PutCell      (GetColName(DB_Tci.t_ticker), 0, value); }
   }

   public uint   Fld_DGV_KupdobCd
   {
      get { return TheGrid.GetUint32Cell(GetColName(DB_Tci.t_kupdob_cd), 0, false); }
      set {        TheGrid.PutCell      (GetColName(DB_Tci.t_kupdob_cd), 0, value); }
   }

   public string Fld_DGV_MtrosTK
   {
      get { return TheGrid.GetStringCell(GetColName(DB_Tci.t_mtros_tk), 0, false); }
      set {        TheGrid.PutCell      (GetColName(DB_Tci.t_mtros_tk), 0, value); }
   }

   public uint   Fld_DGV_MtrosCd
   {
      get { return TheGrid.GetUint32Cell(GetColName(DB_Tci.t_mtros_cd), 0, false); }
      set {        TheGrid.PutCell      (GetColName(DB_Tci.t_mtros_cd), 0, value); }
   }

   public uint   Fld_DGV_FakRecID
   {
      get { return TheGrid.GetUint32Cell(GetColName(DB_Tci.t_fakRecID), 0, false); }
      set {        TheGrid.PutCell      (GetColName(DB_Tci.t_fakRecID), 0, value); }
   }
   public uint   Fld_DGV_FakYear
   {
      get { return TheGrid.GetUint32Cell(GetColName(DB_Tci.t_fakYear), 0, false); }
      set {        TheGrid.PutCell      (GetColName(DB_Tci.t_fakYear), 0, value); }
   }

   public string Fld_DGV_TipBr
   {
      get { return TheGrid.GetStringCell(GetColName(DB_Tci.t_tipBr), 0, false); }
      set {        TheGrid.PutCell      (GetColName(DB_Tci.t_tipBr), 0, value); }
   }

   public string Fld_DGV_ProjektCD
   {
      get { return TheGrid.GetStringCell(GetColName(DB_Tci.t_projektCD), 0, false); }
      set {        TheGrid.PutCell      (GetColName(DB_Tci.t_projektCD), 0, value); }
   }

   public DateTime Fld_DGV_Valuta
   {
      get { return TheGrid.GetDateCell(GetColName(DB_Tci.t_valuta), 0, false); }
      set {        TheGrid.PutCell    (GetColName(DB_Tci.t_valuta), 0, value); }
   }

   public decimal Fld_DGV_Dug
   {
      get { return TheGrid.GetDecimalCell(GetColName(DB_Tci.t_dug), 0, false); }
      set {        TheGrid.PutCell       (GetColName(DB_Tci.t_dug), 0, value); }
   }

   public decimal Fld_DGV_Pot
   {
      get { return TheGrid.GetDecimalCell(GetColName(DB_Tci.t_pot), 0, false); }
      set {        TheGrid.PutCell       (GetColName(DB_Tci.t_pot), 0, value); }
   }

   public string Fld_DGV_Pozicija
   {
      get { return TheGrid.GetStringCell(GetColName(DB_Tci.t_pozicija), 0, false); }
      set {        TheGrid.PutCell      (GetColName(DB_Tci.t_pozicija), 0, value); }
   }
   public string Fld_DGV_Fond
   {
      get { return TheGrid.GetStringCell(GetColName(DB_Tci.t_fond), 0, false); }
      set {        TheGrid.PutCell      (GetColName(DB_Tci.t_fond), 0, value); }
   }

   #endregion DGV Columns as Fld_

   #endregion PutDgvFields, GetDgvFields

   #endregion PutFields(), GetFields()

   #region FillDataset

   public void FillIzvodDataSet(ZapIzvod _theIzvod)
   {
      if(_theIzvod == null) return;

      TheDS_ZapIzvod = new Vektor.DataLayer.DS_Reports.DS_ZapIzvodNEW();

      TheDS_ZapIzvod.headRecord.Rows.Add(

         _theIzvod.HeadRecord.iz_br,
         _theIzvod.HeadRecord.iz_date,
         _theIzvod.HeadRecord.iz_prev_sign,
         _theIzvod.HeadRecord.iz_new_sign,
         _theIzvod.HeadRecord.iz_firma,
         _theIzvod.HeadRecord.bankaName,
         _theIzvod.HeadRecord.bankaVbdi,
         _theIzvod.HeadRecord.iz_ziro_rnpri,
         _theIzvod.HeadRecord.iz_ziro_racun,
         _theIzvod.HeadRecord.iz_uk_dug,
         _theIzvod.HeadRecord.iz_uk_pot,
         _theIzvod.HeadRecord.iz_prev_saldo,
         _theIzvod.HeadRecord.iz_saldo,
         _theIzvod.HeadRecord.iz_mjesto,
         _theIzvod.HeadRecord.iz_dateIn6,
         _theIzvod.HeadRecord.iz_numOfTranses
      );

      //headTable[0].iz_br         = TheIzvod.HeadRecord.iz_br;
      //headTable[0].iz_date       = TheIzvod.HeadRecord.iz_date;
      //headTable[0].iz_prev_sign  = TheIzvod.HeadRecord.iz_prev_sign;
      //headTable[0].iz_new_sign   = TheIzvod.HeadRecord.iz_new_sign;
      //headTable[0].iz_firma      = TheIzvod.HeadRecord.iz_firma;
      //headTable[0].iz_zap_name   = TheIzvod.HeadRecord.iz_zap_name;
      //headTable[0].iz_ziro_bdi   = TheIzvod.HeadRecord.iz_ziro_bdi;
      //headTable[0].iz_ziro_rnpri = TheIzvod.HeadRecord.iz_ziro_rnpri;
      //headTable[0].iz_ziro_racun = TheIzvod.HeadRecord.iz_ziro_racun;
      //headTable[0].iz_uk_dug     = TheIzvod.HeadRecord.iz_uk_dug;
      //headTable[0].iz_uk_pot     = TheIzvod.HeadRecord.iz_uk_pot;
      //headTable[0].iz_prev_saldo = TheIzvod.HeadRecord.iz_prev_saldo;
      //headTable[0].iz_saldo      = TheIzvod.HeadRecord.iz_saldo;

      int i = 0;
      foreach(ZapIzvod.TransRecordStruct trans_rec in _theIzvod.Transes)
      {
         TheDS_ZapIzvod.transRecord.Rows.Add(

            _theIzvod.Transes[i].t_br,
            _theIzvod.Transes[i].t_serial,
            _theIzvod.Transes[i].t_valuta,
            _theIzvod.Transes[i].t_rbr,
            _theIzvod.Transes[i].t_kd_ziro,
            _theIzvod.Transes[i].t_kupdob,
            _theIzvod.Transes[i].t_kd_mjesto,
            _theIzvod.Transes[i].t_pnb0_zad,
            _theIzvod.Transes[i].t_pnb1_zad,
            _theIzvod.Transes[i].t_pnb0_odob,
            _theIzvod.Transes[i].t_pnb1_odob,
            _theIzvod.Transes[i].t_vezna_oznaka,
            _theIzvod.Transes[i].t_svrha_doz,
            _theIzvod.Transes[i].t_new_old_rn,
            _theIzvod.Transes[i].t_reklamacija,
            _theIzvod.Transes[i].t_iznos,
            _theIzvod.Transes[i].t_dug,
            _theIzvod.Transes[i].t_pot,
            _theIzvod.Transes[i].t_vrsta_transakcije,
            _theIzvod.Transes[i].t_rnpvd,
            _theIzvod.Transes[i].t_kd_vbdi
         );

         //transTable[i].t_br               = TheIzvod.Transes[i].t_br;
         //transTable[i].t_serial           = TheIzvod.Transes[i].t_serial;
         //transTable[i].t_valuta           = TheIzvod.Transes[i].t_valuta;
         //transTable[i].t_rbr              = TheIzvod.Transes[i].t_rbr;
         //transTable[i].t_kd_ziro          = TheIzvod.Transes[i].t_kd_ziro;
         //transTable[i].t_kupdob           = TheIzvod.Transes[i].t_kupdob;
         //transTable[i].t_kd_mjesto        = TheIzvod.Transes[i].t_kd_mjesto;
         //transTable[i].t_pnb0_zad         = TheIzvod.Transes[i].t_pnb0_zad;
         //transTable[i].t_pnb1_zad         = TheIzvod.Transes[i].t_pnb1_zad;
         //transTable[i].t_pnb0_odob        = TheIzvod.Transes[i].t_pnb0_odob;
         //transTable[i].t_pnb1_odob        = TheIzvod.Transes[i].t_pnb1_odob;
         //transTable[i].t_vezna_oznaka     = TheIzvod.Transes[i].t_vezna_oznaka;
         //transTable[i].t_svrha_doz        = TheIzvod.Transes[i].t_svrha_doz;
         //transTable[i].t_new_old_rn       = TheIzvod.Transes[i].t_new_old_rn;
         //transTable[i].t_reklamacija      = TheIzvod.Transes[i].t_reklamacija;
         //transTable[i].t_iznos            = TheIzvod.Transes[i].t_iznos;
         //transTable[i].t_dug              = TheIzvod.Transes[i].t_dug;
         //transTable[i].t_pot              = TheIzvod.Transes[i].t_pot;
         //transTable[i].t_vrsta_transakcije= TheIzvod.Transes[i].t_vrsta_transakcije;
         //transTable[i].t_rnpvd            = TheIzvod.Transes[i].t_rnpvd;
         //transTable[i].t_kd_vbdi          = TheIzvod.Transes[i].t_kd_vbdi;
         //transTable[i].t_br               = TheIzvod.Transes[i].t_br;

         i++;
      }
   }

   #endregion FillDataset

   #region Methods BEFORE Start Operation

   void tbx_kontoZiro_TextChanged_UpdateSaldo(object sender, EventArgs e)
   {
      string konto = ((TextBox)sender).Text;

      if(konto.NotEmpty())
      {
         //PreviousSaldoInProgram = KplanDao.GetKplan_SALDO(ZXC.TheVvForm.conn, konto, "", DateTime.MinValue, DateTime.MaxValue);
         PreviousSaldoInProgram = KplanDao.GetKplan_SaldoOrDugOrPot_SUM(ZXC.TheVvForm.TheDbConnection, ZXC.SaldoOrDugOrPot.SALDO, konto, "", DateTime.MinValue, DateTime.MaxValue);

         if(TheIzvod != null) DIFF_AndFldAndTbx(TheIzvod);

      }
      else
      {
         PreviousSaldoInProgram = decimal.Zero;
         DIFF_PrevSaldoPrgVsIzv = decimal.Zero;
      }

      Fld_B_KontoIsti = konto;

      VvUserControl.PutSaldoKontaFields(PreviousSaldoInProgram, tbx_saldoKtaDug, tbx_saldoKtaPot);
   }

   public bool Open_Load_PutFields(string fullPathFileName)
   {
      TheIzvod = new ZapIzvod(fullPathFileName);

      // 25.04.2012: 
      if(TheIzvod.HeadRecord.firmaOIB.NotEmpty() &&
         TheIzvod.HeadRecord.firmaOIB != ZXC.CURR_prjkt_rec.Oib)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "OIB u izvodu <{0}> NE odgovara OIB-u u adresaru Projekt-a <{1}>", TheIzvod.HeadRecord.firmaOIB, ZXC.CURR_prjkt_rec.Oib);
      }

      if(TheIzvod.BadData)
    //if(TheIzvod.BadData || ZXC.vvDB_VvDomena == "vvIN")
    //if(TheIzvod.BadData || ZXC.CURR_prjkt_rec.Ticker == "TURZML")
      {
         VvSQL.ReportGenericError("OPEN FILE", string.Format("Datoteka\n\n{0}\n\nNe izgleda kao izvod 'txt' datoteka!", fullPathFileName), System.Windows.Forms.MessageBoxButtons.OK);
         return false;
      }
      else
      {
         PutSelectedIzvodFileNameFields(TheIzvod);
         PutBetaFields(TheIzvod);
         return true;
      }
   }

   private decimal GetPrevIzvodSaldoNonAbsolute(ZapIzvod zapIzvod_rec)
   {
      return zapIzvod_rec.HeadRecord.iz_prev_saldo * (zapIzvod_rec.HeadRecord.iz_prev_sign == '-' ? -1.00M : 1.00M);
   }

   private decimal GetSaldoIzvNonAbsolute(ZapIzvod zapIzvod_rec)
   {
      return zapIzvod_rec.HeadRecord.iz_saldo * (zapIzvod_rec.HeadRecord.iz_new_sign == '-' ? -1.00M : 1.00M);
   }

   private void DIFF_AndFldAndTbx(ZapIzvod zapIzvod_rec)
   {
      // 04.05.2010. - zbog negativnih izvoda - tj, mogucnosti minusa
      //DIFF_PrevSaldoPrgVsIzv = PreviousSaldoInProgram - GetPrevIzvodSaldoNonAbsolute(zapIzvod_rec);
      DIFF_PrevSaldoPrgVsIzv = PreviousSaldoInProgram * (PreviousSaldoInProgram < 0.00M ? -1.00M : 1.00M) - GetPrevIzvodSaldoNonAbsolute(zapIzvod_rec);

      if(DIFF_PrevSaldoPrgVsIzv != decimal.Zero)
         tbx_razlDug.Visible = tbx_razlPot.Visible = lbl_diff.Visible = true;
      else
         tbx_razlDug.Visible = tbx_razlPot.Visible = lbl_diff.Visible = false;

      Fld_B_RazlikaDug = (DIFF_PrevSaldoPrgVsIzv > decimal.Zero ? DIFF_PrevSaldoPrgVsIzv : decimal.Zero);
      Fld_B_RazlikaPot = (DIFF_PrevSaldoPrgVsIzv < decimal.Zero ? DIFF_PrevSaldoPrgVsIzv * -1.00M : decimal.Zero);
   }

   #endregion Methods BEFORE Start Operation

   #region Methods AFTER Start Operation

   private struct RacunData // ovo kasnije zamijeni sa pravim Faktur bussinessom 
   {
      public string   tipBr;
      public string   projektCD;
      public DateTime valuta;
      //public decimal  money;
      
      public uint fakRecID;
      public uint fakYear ;

      public uint   mtrosCD;
      public string mtrosTK;
   }

   private RacunData Racun { get; set; }

   public void PresumeAndPutDGVfields(ZapIzvod.TransRecordStruct trans_rec)
   {
      // 06.04.2017: START 
   /* string   */ Fld_DGV_Konto     = "";
   /* string   */ Fld_DGV_Opis      = "";
   /* string   */ Fld_DGV_Ticker    = "";
   /* uint     */ Fld_DGV_KupdobCd  = 0 ;
   /* string   */ Fld_DGV_MtrosTK   = "";
   /* uint     */ Fld_DGV_MtrosCd   = 0 ;
   /* uint     */ Fld_DGV_FakRecID  = 0 ; // !!! zbog ovoga smo ovo i radili jer u nekim sl. nespojena stavka izvoda preuzme fakRecID od prethodne 
   /* uint     */ Fld_DGV_FakYear   = 0 ; 
   /* string   */ Fld_DGV_TipBr     = "";
   /* string   */ Fld_DGV_ProjektCD = "";
   /* DateTime */ Fld_DGV_Valuta    = DateTime.MinValue;
   /* decimal  */ Fld_DGV_Dug       = 0M;
   /* decimal  */ Fld_DGV_Pot       = 0M;
   /* string   */ Fld_DGV_Pozicija  = "";
   /* string   */ Fld_DGV_Fond      = "";
      // 06.04.2017:  END  

      Kupdob kupdob_rec = GetKupdobInCurrentIzvodRecTransRow(trans_rec);

      Fld_DGV_Konto     = GetKontoForDGV(kupdob_rec, trans_rec);
      Fld_D_NazivKonta  = KplanSifrar.Where(k => k.Konto == Fld_DGV_Konto).Select(k => k.Naziv).SingleOrDefault();

      // 22.11.2016: 
    //Fld_DGV_Opis      = trans_rec.t_pnb1_odob;

      Fld_DGV_Opis = ZXC.LenLimitedStr(trans_rec.t_pnb1_odob + (trans_rec.t_pnb1_odob.NotEmpty() ? ";" : "") + trans_rec.t_svrha_doz, ZXC.FtransDao.GetSchemaColumnSize(ZXC.FtrCI.t_opis));

      // 17.05.2016: 
      bool isSaldaKontiKTO = NalogDao.IsSaldaKontiKTO_ForNalogPS(Fld_DGV_Konto, KplanSifrar);

      if(kupdob_rec != null)
      {
         Fld_DGV_Ticker    = kupdob_rec.Ticker;
         Fld_D_NazivKupDob = kupdob_rec.Naziv;
         Fld_DGV_KupdobCd  = kupdob_rec.KupdobCD/*RecID*/;

       //// 16.07.2015: ovo ode u timeout 
       //if((ZXC.IsTEXTHOany || ZXC.CURR_prjkt_rec.Ticker == "QQTEXT") && Fld_DGV_Konto == "1207")
       //{
       //   Racun = new RacunData();
       //}
       //else // classic 
       //{
       //   Racun = GetFirstNonClosedTipBr(Fld_DGV_Konto, Fld_DGV_KupdobCd, TheIzvod.HeadRecord.iz_date, trans_rec.TransMoney);
       //}

         // 17.05.2016: 
         if(isSaldaKontiKTO)
         {
            Racun = GetFirstNonClosedTipBr(Fld_DGV_Konto, Fld_DGV_KupdobCd, TheIzvod.HeadRecord.iz_date, trans_rec.TransMoney);
         }
         else
         {
            Racun = new RacunData();
         }

         Fld_DGV_FakRecID = Racun.fakRecID;
         Fld_DGV_FakYear  = Racun.fakYear ;
         Fld_DGV_TipBr    = Racun.tipBr;
         Fld_DGV_ProjektCD= Racun.projektCD;
         Fld_DGV_MtrosCd  = Racun.mtrosCD;
         Fld_DGV_MtrosTK  = Racun.mtrosTK;
         Fld_DGV_Valuta   = Racun.valuta;
      }
      else
      {
         Fld_DGV_Ticker    = 
         Fld_D_NazivKupDob = 
         Fld_DGV_TipBr     = "";
         Fld_DGV_ProjektCD = "";
         Fld_DGV_KupdobCd  = 0;
         Fld_DGV_MtrosTK   = "";
         Fld_DGV_MtrosCd   = 0;
         Fld_DGV_Valuta    = DateTime.MinValue;

         // 06.04.2017: !!! 
         Fld_DGV_FakRecID  = 0;
         Fld_DGV_FakYear   = 0;
      }

      Fld_DGV_Dug       = trans_rec.t_dug;
      Fld_DGV_Pot       = trans_rec.t_pot;
   }

   private string GetKontoForDGV(Kupdob kupdob_rec, ZapIzvod.TransRecordStruct trans_rec)
   {
      if(     trans_rec.IsDugovniPromet)  return kupdob_rec != null && kupdob_rec.KontoDug.NotEmpty() ? kupdob_rec.KontoDug : Fld_A_NepozDug;
      else if(trans_rec.IsPotrazniPromet) return kupdob_rec != null && kupdob_rec.KontoPot.NotEmpty() ? kupdob_rec.KontoPot : Fld_A_NepozPot;
      else 
         ZXC.aim_emsg("Vrsta transakcije [" + trans_rec.t_vrsta_transakcije + "] nije niti [10] niti [20]!"); return "";
      
      //switch(trans_rec.t_vrsta_transakcije)
      //{
      //   case "10": return artikl_rec != null &&  artikl_rec.KontoDug.NotEmpty() ? artikl_rec.KontoDug : Fld_A_NepozDug; 
      //   case "20": return artikl_rec != null &&  artikl_rec.KontoPot.NotEmpty() ? artikl_rec.KontoPot : Fld_A_NepozPot; 

      //   default: ZXC.aim_emsg("Vrsta transakcije [" + trans_rec.t_vrsta_transakcije + "] nije niti [10] niti [20]!"); return "";
      //}
   }

   private Kupdob GetKupdobInCurrentIzvodRecTransRow(ZapIzvod.TransRecordStruct trans_rec)
   {
      string wz      = trans_rec.t_kd_ziro; // wanted ziro racun 
      string wzPnb   = trans_rec.t_pnb1_odob;
      string wzName  = trans_rec.t_kupdob;
      string wzSvrha = trans_rec.t_svrha_doz;

      bool isHNB = wz.StartsWith("1001005");

      if(isHNB && wzPnb.Length > 4) wzPnb = wzPnb.Substring(0, 4);

      // 11.10.2012: 
      //var kupdobs = KupdobSifrar.Where
      //   (
      //      k =>
      //         ((k.Ziro1 == wz && (isHNB ? k.Ziro1PnbV == wzPnb : true)) ||
      //          (k.Ziro2 == wz && (isHNB ? k.Ziro2PnbV == wzPnb : true)) ||
      //          (k.Ziro3 == wz && (isHNB ? k.Ziro3PnbV == wzPnb : true)) ||
      //          (k.Ziro4 == wz && (isHNB ? k.Ziro4PnbV == wzPnb : true)))
      //   );
      var kupdobs = KupdobSifrar.Where
         (
            k =>
               ((k.Ziro1 == wz && (isHNB ? k.Ziro1PnbV.StartsWith(wzPnb) : true)) ||
                (k.Ziro2 == wz && (isHNB ? k.Ziro2PnbV.StartsWith(wzPnb) : true)) ||
                (k.Ziro3 == wz && (isHNB ? k.Ziro3PnbV.StartsWith(wzPnb) : true)) ||
                (k.Ziro4 == wz && (isHNB ? k.Ziro4PnbV.StartsWith(wzPnb) : true)))
         );

      if(kupdobs.Count() == 2 && isHNB && wzPnb.StartsWith("1406")) // Porez - Prirez dilema 
      {
         // 23.11.2010: 
         //return kupdobs.Where(a => a.Naziv.ToUpper().StartsWith(wzName.ToUpper().Substring(0, 3))).SingleOrDefault();
         
         Kupdob kupdob_rec;

         if(wzSvrha.ToUpper().Contains("PRI"))
         {
            kupdob_rec = kupdobs.Where(k => k.Naziv.ToUpper().Contains("PRI")).SingleOrDefault();
         }
         else
         {
            kupdob_rec = kupdobs.Where(k => k.Naziv.ToUpper().Contains("PRI") == false).SingleOrDefault();
         }
         

         return kupdob_rec;
      }

      if(kupdobs.Count() < 1) return AddNewKupdob_OR_PairOldKupdob_WithThisZiro(trans_rec);

      if(kupdobs.Count() > 1) return ChooseOneOfManyKupdobsWithSameZiro(trans_rec);

      return kupdobs.ElementAt(0);
   }

   private Kupdob ChooseOneOfManyKupdobsWithSameZiro(ZapIzvod.TransRecordStruct trans_rec)
   {
      // 22.11.2016: 
      if(Fld_A_IsQuickLoad) return null;

      Kupdob kupdob_rec = null;

      System.Text.StringBuilder massaze = new System.Text.StringBuilder("U datoteci postoje dva partnera sa traženim žiro računom:\n\n");

      massaze.AppendFormat("žr [{0}] od [{1}]\n\n", trans_rec.t_kd_ziro, trans_rec.t_kupdob);
      massaze.Append("Odaberite jednoga sa liste.\n\n");
      massaze.Append("Ukoliko stavku izvoda nije potrebno uparivati sa Partnerom kiliknite 'Cancel'.");

      if(VvSQL.ReportGenericError("UPARIVANJE PUTEM ŽIRO RAČUNA", massaze.ToString(), System.Windows.Forms.MessageBoxButtons.OKCancel) == false) return kupdob_rec;

      // 21.09.2012: 
    //return AddNew_OR_PairOld_OR_ChooseOneOfMany(trans_rec, "!", trans_rec.t_kd_ziro); // '!' ti je po ASCII sortu prvi. 
      return AddNew_OR_PairOld_OR_ChooseOneOfMany(trans_rec, " ", trans_rec.t_kd_ziro); // ' ' ti je po ASCII sortu prvi. 
   }

   private Kupdob AddNewKupdob_OR_PairOldKupdob_WithThisZiro(ZapIzvod.TransRecordStruct trans_rec)
   {
      // 22.11.2016: 
      if(Fld_A_IsQuickLoad) return null;

      Kupdob       kupdob_rec = null;

      System.Text.StringBuilder massaze = new System.Text.StringBuilder("Ne mogu upariti podatke iz izvoda:\n\n");

      massaze.AppendFormat("žr [{0}] od [{1}]\n\n", trans_rec.t_kd_ziro, trans_rec.t_kupdob);
      massaze.Append("sa podacima iz Adresara Partnera. Ukoliko partner već postoji u datoteci,\nodaberite ga sa liste a ako ne, kliknite 'Dodaj Novi'.\n\n");
      massaze.Append("Ukoliko stavku izvoda nije potrebno uparivati sa Partnerom kiliknite 'Cancel'.");

      if(VvSQL.ReportGenericError("UPARIVANJE PUTEM ŽIRO RAČUNA", massaze.ToString(), System.Windows.Forms.MessageBoxButtons.OKCancel) == false) return kupdob_rec;

      return AddNew_OR_PairOld_OR_ChooseOneOfMany(trans_rec, trans_rec.t_kupdob.Substring(0, 3), "");
   }

   private Kupdob AddNew_OR_PairOld_OR_ChooseOneOfMany(ZapIzvod.TransRecordStruct trans_rec, string startingNaziv, string filterZiroRn)
   {
      Kupdob       kupdob_rec = new Kupdob();
      KupdobListUC kupdobListUC;

      VvFindDialog dlg = KupdobUC.CreateFind_Kupdob_Dialog();

      kupdobListUC = (KupdobListUC)(dlg.TheRecListUC);

      kupdobListUC.Fld_FromNaziv  = startingNaziv;
      kupdobListUC.Fld_FilterZiro = filterZiroRn;

      //--- Additions for 'AddAndGetNewVvSifrarRecordInteractive' START 

      bool isHNB = trans_rec.t_kd_ziro.StartsWith("1001005");

      kupdob_rec.Naziv  = trans_rec.t_kupdob;

      if(trans_rec.t_kupdob.Length > 6) kupdob_rec.Ticker = trans_rec.t_kupdob.Substring(0, 6);
      else                              kupdob_rec.Ticker = trans_rec.t_kupdob;     

      kupdob_rec.Grad = trans_rec.t_kd_mjesto;

      kupdob_rec.Ziro1 = trans_rec.t_kd_ziro;
      if(isHNB)
      {
         kupdob_rec.Ziro1PnbV = trans_rec.t_pnb1_odob;
      }

      ZXC.TheGlobalVvDataRecord = kupdob_rec;

      //--- Additions for 'AddAndGetNewVvSifrarRecordInteractive' END   

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         if(!ZXC.KupdobDao.SetMe_Record_byRecID(ZXC.TheVvForm.TheDbConnection, kupdob_rec, (uint)dlg.SelectedRecID, false)) return null;

         // 21.09.2012: 
       //if(dlg.SelectionIsNewlyAddedRecord == false && startingNaziv != "!") // samo ako nije NewRecord i ako nije ChooseOneOfManyKupdobsWithSameZiro 
         if(dlg.SelectionIsNewlyAddedRecord == false && startingNaziv != " ") // samo ako nije NewRecord i ako nije ChooseOneOfManyKupdobsWithSameZiro 
         {
            RwtrecKupdob(kupdob_rec, trans_rec);
         }
      }
      else
      {
         kupdob_rec = null;
      }

      dlg.Dispose();

      return kupdob_rec;
   }

   private void RwtrecKupdob(Kupdob kupdob_rec, ZapIzvod.TransRecordStruct trans_rec)
   {
      DialogResult result = MessageBox.Show("Da li zelite usnimiti žiro račun [" + trans_rec.t_kd_ziro + "] na partnera [" + kupdob_rec.Naziv + "] ?",
         "Usnimiti žiro račun?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

      if(result != DialogResult.Yes) return;


      //=== Here we GO! ======================= 

      kupdob_rec.BeginEdit();


      string wz    = trans_rec.t_kd_ziro; // wanted ziro racun 
      string wzPnb = trans_rec.t_pnb1_odob;

      bool isHNB = trans_rec.t_kd_ziro.StartsWith("1001005");

      if(kupdob_rec.Ziro1.IsEmpty())      { kupdob_rec.Ziro1 = wz; if(isHNB) kupdob_rec.Ziro1PnbV = wzPnb; }
      else if(kupdob_rec.Ziro2.IsEmpty()) { kupdob_rec.Ziro2 = wz; if(isHNB) kupdob_rec.Ziro2PnbV = wzPnb; }
      else if(kupdob_rec.Ziro3.IsEmpty()) { kupdob_rec.Ziro3 = wz; if(isHNB) kupdob_rec.Ziro4PnbV = wzPnb; }
      else if(kupdob_rec.Ziro4.IsEmpty()) { kupdob_rec.Ziro4 = wz; if(isHNB) kupdob_rec.Ziro1PnbV = wzPnb; }
      else
      { 
         ZXC.aim_emsg("Svi su žiro računi popunjeni!!!");
         kupdob_rec.EndEdit();
         return;
      }

      kupdob_rec.VvDao.RWTREC(ZXC.TheVvForm.TheDbConnection, kupdob_rec, false, false);

      kupdob_rec.EndEdit();

      SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name); 

      return;
   }

   private void DumpIzvodDataOnNewDucDgvRow(string _konto, string _opis, string _ticker, uint _kupdob_cd, uint _fakRecID, uint _fakYear, string _tipBr, string _projektCD, DateTime _valuta, decimal _dug, decimal _pot, string _mtrosTk, uint _mtrosCd, string _pozicija, string _fond)
   {
      int rowIdx, idxCorrector;

      VvDataGridView grid = TheDUC.TheG;

      idxCorrector = GetDGVsIdxCorrrector(grid);

      grid.Rows.Add();

      //rowIdx = TheG.NewRowIndex - 1;
      rowIdx = grid.RowCount - idxCorrector;

      grid.PutCell(TheDUC.DgvCI.iT_konto    , rowIdx, _konto    );
      grid.PutCell(TheDUC.DgvCI.iT_opis     , rowIdx, _opis     );
      grid.PutCell(TheDUC.DgvCI.iT_ticker   , rowIdx, _ticker   );
      grid.PutCell(TheDUC.DgvCI.iT_kupdob_cd, rowIdx, _kupdob_cd);
      grid.PutCell(TheDUC.DgvCI.iT_tipBr    , rowIdx, _tipBr    );
      grid.PutCell(TheDUC.DgvCI.iT_projektCD, rowIdx, _projektCD);
      grid.PutCell(TheDUC.DgvCI.iT_fakRecID , rowIdx, _fakRecID );
      grid.PutCell(TheDUC.DgvCI.iT_fakYear  , rowIdx, _fakYear  );
      grid.PutCell(TheDUC.DgvCI.iT_valuta   , rowIdx, _valuta   );
      grid.PutCell(TheDUC.DgvCI.iT_dug      , rowIdx, _dug      );
      grid.PutCell(TheDUC.DgvCI.iT_pot      , rowIdx, _pot      );

      grid.PutCell(TheDUC.DgvCI.iT_mtros_tk, rowIdx, _mtrosTk   );
      grid.PutCell(TheDUC.DgvCI.iT_mtros_cd, rowIdx, _mtrosCd   );
      grid.PutCell(TheDUC.DgvCI.iT_pozicija, rowIdx, _pozicija  );
      grid.PutCell(TheDUC.DgvCI.iT_fond    , rowIdx, _fond      );

      //TheG.Rows.Add();
   }

   public void DumpDGV2DGV()
   {
      if(Fld_DGV_TipBr.StartsWith("MULTI"))
      {
         decimal dug, pot;

         foreach(OtsTipBrGroupInfo otsInfo in OtsInfoSelectionList)
         {
            if(otsInfo.IsKupac)
            {
               pot = otsInfo.UkSaldo;
               dug = 0.00M;
            }
            else
            {
               dug = otsInfo.UkSaldo;
               pot = 0.00M;
            }

            DumpIzvodDataOnNewDucDgvRow(Fld_DGV_Konto, Fld_DGV_Opis, Fld_DGV_Ticker, Fld_DGV_KupdobCd, otsInfo.FakRecID, otsInfo.FakYear, otsInfo.TipBr, otsInfo.ProjektCD, otsInfo.OpenDokumentValuta, dug, pot, Fld_DGV_MtrosTK, Fld_DGV_MtrosCd, Fld_DGV_Pozicija, Fld_DGV_Fond);
         }
      }
      else
      {
         DumpIzvodDataOnNewDucDgvRow(Fld_DGV_Konto, Fld_DGV_Opis, Fld_DGV_Ticker, Fld_DGV_KupdobCd, Fld_DGV_FakRecID, Fld_DGV_FakYear, Fld_DGV_TipBr, Fld_DGV_ProjektCD, Fld_DGV_Valuta, Fld_DGV_Dug, Fld_DGV_Pot, Fld_DGV_MtrosTK, Fld_DGV_MtrosCd, Fld_DGV_Pozicija, Fld_DGV_Fond);
      }
   }

   public void DumpPrometIzvodaOnDucDgv()
   {
      if(Fld_A_IsPrmUJedRed == true)
      {
         DumpIzvodDataOnNewDucDgvRow(Fld_A_KontoZiro, "I-" + TheIzvod.HeadRecord.iz_br.ToString("000") + "/PROMET", "", 0, 0, 0, "", "", DateTime.MinValue, TheIzvod.HeadRecord.iz_uk_pot, TheIzvod.HeadRecord.iz_uk_dug, "", 0, "", "");
      }
      else
      {
         if(TheIzvod.HeadRecord.iz_uk_dug.NotZero())
         {
            DumpIzvodDataOnNewDucDgvRow(Fld_A_KontoZiro, "I-" + TheIzvod.HeadRecord.iz_br.ToString("000") + "/PROMET", "", 0, 0, 0, "", "", DateTime.MinValue, 0.00M, TheIzvod.HeadRecord.iz_uk_dug, "", 0, "", "");
         }

         if(TheIzvod.HeadRecord.iz_uk_pot.NotZero())
         {
            DumpIzvodDataOnNewDucDgvRow(Fld_A_KontoZiro, "I-" + TheIzvod.HeadRecord.iz_br.ToString("000") + "/PROMET", "", 0, 0, 0, "", "", DateTime.MinValue, TheIzvod.HeadRecord.iz_uk_pot, 0.00M, "", 0, "", "");
         }
      }
   }

   #endregion Methods AFTER Start Operation

   #region GetOTS

   private RacunData GetFirstNonClosedTipBr(string konto, uint kupdob_cd, DateTime dateIzvod, decimal izvodMoney)
   {
      RacunData rd = new RacunData();

      List<OtsTipBrGroupInfo> otsDistinctByTipBrSortedList = GetOtsDistinctList(konto, kupdob_cd, dateIzvod);

      var moneyMatchList = otsDistinctByTipBrSortedList.Where(ots => ots.UkSaldo == izvodMoney);

      OtsTipBrGroupInfo firstOts = moneyMatchList.ElementAtOrDefault(0);

      rd.fakRecID = firstOts.FakRecID ; 
      rd.fakYear  = firstOts.FakYear  ; 
      rd.tipBr    = firstOts.TipBr    ; 
      rd.projektCD= firstOts.ProjektCD; 
      rd.mtrosCD  = firstOts.MtrosCD  ; 
      rd.mtrosTK  = firstOts.MtrosTK  ; 
      rd.valuta   = firstOts.OpenDokumentValuta;

      return rd;
   }

   public static List<OtsTipBrGroupInfo> GetOtsDistinctList(string konto, uint kupdob_cd, DateTime dateIzvod)
   {
      List<Ftrans> allOtsFtransList = NalogDao.GetOTS_FtransByTipBrSortedList(ZXC.TheVvForm.TheDbConnection, konto, kupdob_cd, /* 05.12.2011: dateIzvod*/ DateTime.Now); // da pokupi i avansne 

      List<OtsTipBrGroupInfo> otsInfoList = SetOtsInfoData(allOtsFtransList, dateIzvod);

      return otsInfoList.Distinct().ToList();
   }

   private static List<OtsTipBrGroupInfo> SetOtsInfoData(List<Ftrans> allOtsFtransList, DateTime dateIzvod)
   {
      int      tmpZakas;
      DateTime odDate, doDate;
      TimeSpan timeSpan;

      List<OtsTipBrGroupInfo> otsInfoList = new List<OtsTipBrGroupInfo>();
      OtsTipBrGroupInfo       otsInfo;

      foreach(Ftrans outer_ftrans_rec in allOtsFtransList)
      {
         otsInfo = new OtsTipBrGroupInfo(/*outer_ftrans_rec*/);

         var oneTipBrFtranses = allOtsFtransList.Where(ftr => ftr.T_tipBr == outer_ftrans_rec.T_tipBr);

         decimal ukDug = oneTipBrFtranses.Sum(ftr => ftr.T_dug);
         decimal ukPot = oneTipBrFtranses.Sum(ftr => ftr.T_pot);

         otsInfo.Konto    = outer_ftrans_rec.T_konto;
         otsInfo.KupdobCd = outer_ftrans_rec.T_kupdob_cd;
         otsInfo.FakRecID = outer_ftrans_rec.T_fakRecID;
         otsInfo.FakYear  = outer_ftrans_rec.T_fakYear;
         otsInfo.TipBr    = outer_ftrans_rec.T_tipBr;
         otsInfo.ProjektCD= outer_ftrans_rec.T_projektCD;
         otsInfo.MtrosCD  = outer_ftrans_rec.T_mtros_cd;
         otsInfo.MtrosTK  = outer_ftrans_rec.T_mtros_tk;
         otsInfo.UkOpen   = otsInfo.IsKupac ? ukDug : ukPot;
         otsInfo.UkClosed = otsInfo.IsKupac ? ukPot : ukDug;
         otsInfo.UkSaldo  = oneTipBrFtranses.Sum(ftr => (otsInfo.IsKupac ? ftr.T_dug - ftr.T_pot : ftr.T_pot - ftr.T_dug));

         foreach(Ftrans inner_ftrans_rec in oneTipBrFtranses)
         {

            if(FtransIs_OTVARANJE(inner_ftrans_rec))
            {
               otsInfo.OpenDokumentDokNum  = inner_ftrans_rec.T_dokNum;
               otsInfo.OpenDokumentDokDate = inner_ftrans_rec.T_dokDate;
               otsInfo.OpenDokumentOpis    = inner_ftrans_rec.T_opis;

               if(inner_ftrans_rec.T_valuta.Equals(DateTime.MinValue)) // t_valuta je empty tj "01.01.0001" 
               {
                  otsInfo.OpenDokumentValuta = inner_ftrans_rec.T_dokDate;
               }
               else
               {
                  otsInfo.OpenDokumentValuta = inner_ftrans_rec.T_valuta;
               }

               odDate = otsInfo.OpenDokumentValuta;
               doDate = /*DateTime.Now*/ dateIzvod;

               timeSpan = doDate.Date - odDate.Date;
               tmpZakas = (int)(timeSpan.TotalDays);

               if(tmpZakas > 0) otsInfo.IsDospjelo = true;

               otsInfo.FoundOpening = true;

               break;
            }
         } // foreach(Ftrans inner_ftrans_rec in oneTipBrFtranses) 

         if(FtransIs_OTVARANJE(outer_ftrans_rec))
         {
            // Ako priliom otvaranja valutu zatekne praznu onda je valuta sam datum racuna,
            // tj. dan od kada se racuna zakasnjenje je sam datum racuna 
            // odervajs, ako valuta postoji onda zakas krece od tada... 

            if(outer_ftrans_rec.T_valuta.Equals(DateTime.MinValue)) // t_valuta je empty tj "01.01.0001" 
            {
               odDate = outer_ftrans_rec.T_dokDate;
            }
            else
            {
               odDate = outer_ftrans_rec.T_valuta;
            }

            // :::::::::::::::::::::::::: ovo gore: odDate logika, ovo dole:doDate logika ::::::::::::::::::

            doDate = /*DateTime.Now*/ dateIzvod; 

            // vidi komentar na 'public struct OtsTipBrGroupInfo' 
            //otsInfo.IsOtvaranje = true;

         }
         else if(FtransIs_ZATVARANJE(outer_ftrans_rec))
         {
            if(otsInfo.FoundOpening == true)
            { // znaci postoji bar jedno otvaranje za taj TipBr 

               odDate = otsInfo.OpenDokumentValuta;

            }
            else
            { // ne postoji niti jedno otvaranje zanaci ovu su goli AVANSi 

               odDate = outer_ftrans_rec.T_dokDate;
            }

            // added 09.06.2008: (za Report, ovdje NE!)
            //outer_ftrans_rec.T_valuta = odDate;

            // :::::::::::::::::::::::::: ovo gore: odDate logika, ovo dole:doDate logika ::::::::::::::::::

            // ako je stavka zatvaranje onda se zakas racuna do datuma te uplate, a ne do datuma stampanja
            // izvjestaja kako ima smisla kod stavke koja otvara dogadjaj. 

            doDate = /*outer_ftrans_rec.T_dokDate*/ dateIzvod;

            // KRIVO!!! Ovo gore je KRIVO! Iako je ovdje neko djelomicno zatvaranje, tu nas ne interesira 
            // kasnjenje od valute pa do te djelomicne uplate vec do dana danasnjega buduci da tipBr kao takav u cjelosti nije placen


            //strcpy(doDate, NR.date); 

         } // ==============================================================================================

         else
         {
            ZXC.aim_emsg("stavka <{0}>nor OTVARANJE nor ZATVARANJE!", outer_ftrans_rec.ToString());
            odDate = doDate = DateTime.MinValue;
         }

         timeSpan = doDate.Date - odDate.Date;
         //otsInfo.zakas = timeSpan.Days;
         otsInfo.Zakas = (int)timeSpan.TotalDays;
         
         otsInfoList.Add(otsInfo);
      }

      return otsInfoList;
   }

   private static bool FtransIs_OTVARANJE(Ftrans ftrans_rec)
   {
      if(ftrans_rec.T_TT.StartsWith("PS") ||
         ftrans_rec.T_TT.StartsWith("IR") ||
         ftrans_rec.T_TT.StartsWith("UR") ||
         ftrans_rec.T_TT.StartsWith("33") ||
         ftrans_rec.T_TT.StartsWith("33") ||
         ftrans_rec.T_TT.StartsWith("44") ||
         ftrans_rec.T_TT.StartsWith("45")) return true;

      // 19.01.2016: 
    //if( ftrans_rec.T_konto.StartsWith("12")                                               && ftrans_rec.T_dug != decimal.Zero) return true;
    //if( ftrans_rec.T_konto.StartsWith("22")                                               && ftrans_rec.T_pot != decimal.Zero) return true;
    //if( ftrans_rec.T_konto.StartsWith("21")                                               && ftrans_rec.T_pot != decimal.Zero) return true;
      if((ftrans_rec.T_konto.StartsWith("12") || Kplan.GetIsKontoKupac(ftrans_rec.T_konto)) && ftrans_rec.T_dug != decimal.Zero) return true;
      if((ftrans_rec.T_konto.StartsWith("22") || Kplan.GetIsKontoDobav(ftrans_rec.T_konto)) && ftrans_rec.T_pot != decimal.Zero) return true;
      if((ftrans_rec.T_konto.StartsWith("21")                                             ) && ftrans_rec.T_pot != decimal.Zero) return true;

      return false;
   }

   private static bool FtransIs_ZATVARANJE(Ftrans ftrans_rec)
   {
      if(ftrans_rec.T_TT.StartsWith("IZ") ||
         ftrans_rec.T_TT.StartsWith("22")) return true;

      // 19.01.2016: 
    //if( ftrans_rec.T_konto.StartsWith("12")                                               && ftrans_rec.T_pot != decimal.Zero) return true;
    //if( ftrans_rec.T_konto.StartsWith("22")                                               && ftrans_rec.T_dug != decimal.Zero) return true;
    //if( ftrans_rec.T_konto.StartsWith("21")                                               && ftrans_rec.T_dug != decimal.Zero) return true;
      if((ftrans_rec.T_konto.StartsWith("12") || Kplan.GetIsKontoKupac(ftrans_rec.T_konto)) && ftrans_rec.T_pot != decimal.Zero) return true;
      if((ftrans_rec.T_konto.StartsWith("22") || Kplan.GetIsKontoDobav(ftrans_rec.T_konto)) && ftrans_rec.T_dug != decimal.Zero) return true;
      if((ftrans_rec.T_konto.StartsWith("21")                                             ) && ftrans_rec.T_dug != decimal.Zero) return true;

      return false;
   }

   #endregion GetOTS

}

public class SelectOTSdlg : VvDialog // Crownwood.DotNetMagic.Forms.DotNetMagicForm
{
   #region Fieldz

   private Button okButton, cancelButton;
   private int    dlgWidth, dlgHeight;

   #endregion Fieldz

   #region Propertiez

   public SelectOTSUC TheUC { get; set; }

   #endregion Propertiez

   #region Constructor

   public SelectOTSdlg(VvTextBox vvTextBox)
   {
      TheUC = new SelectOTSUC(vvTextBox);

      TheUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      Kupdob theKupdobRec = VvUserControl.KupdobSifrar.SingleOrDefault(k => k.KupdobCD/*RecID*/ == vvTextBox.OtsKupdobCd);

      SuspendLayout();

      this.Font        = ZXC.vvFont.BaseFont;
      this.Style       = ZXC.vvColors.vvform_VisualStyle;
      this.BackColor   = ZXC.vvColors.userControl_BackColor;
      //this.MaximizeBox = false;

      this.StartPosition = FormStartPosition.Manual;
      this.Location      = Point.Empty;

      this.Text          = string.Format("Otvorene stavke za Konto: {0}, Partner: {1} / {2} / {3}",
                              vvTextBox.OtsKonto,
                              (theKupdobRec == null ? "" : theKupdobRec.KupdobCD/*RecID*/.ToString("000000")),
                              (theKupdobRec == null ? "" : theKupdobRec.Ticker),
                              (theKupdobRec == null ? "" : theKupdobRec.Naziv));

      CreateTheUC();

      dlgWidth  = TheUC.Width ;
      dlgHeight = TheUC.Height + ZXC.QunBtnH + ZXC.QunMrgn;

      this.ClientSize = new Size(dlgWidth, SystemInformation.WorkingArea.Height - ZXC.Q3un);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      ResumeLayout();

      VvHamper.AttachEscPressForEachControl(this);
   }

   #endregion Constructor

   #region TheUC

   private void CreateTheUC()
   {
      TheUC.Parent   = this;
      TheUC.Location = Point.Empty;
   }

   #endregion TheUC
}

public class SelectOTSUC : VvOtherUC
{
   #region Fieldz

   private VvHamper     hamp_Fond;
   private VvTextBox    tbx_FndUkupno,  tbx_FndIskoristeno, tbx_FndRaspolozivo;
   private DataGridView gridSum;
   public  TextBox tbx_DummyForDefaultFocus;

   #endregion Fieldz

   #region Propertiez
   
   public VvDataGridView TheGrid { get; set; }

   public VvTextBox TheTipBrTextBox { get; set; }

   public BindingSource TheBindingSource { get; set; }

   //public List<ZXC.OtsTipBrGroupInfo> InitialList   { get; set; }
   //public List<ZXC.OtsTipBrGroupInfo> SelectionList { get; set; }

   #endregion Propertiez

   #region Constructor

   public SelectOTSUC(VvTextBox vvTextBox)
   {
      TheTipBrTextBox = vvTextBox;
    
      SuspendLayout();


      Create_DummyForDefaultFocusOts();

      InitializeHamperFond(out hamp_Fond);
      
      CreateTheGrid();
      CreateGridSum();

      CalcLocationAndSize();
      /*InitialList =*/ SetDataSource(TheTipBrTextBox.OtsKonto, TheTipBrTextBox.OtsKupdobCd, TheTipBrTextBox.OtsDateDo);

      Fld_FndUkupno = Fld_FndRaspolozivo = TheTipBrTextBox.OtsMoney;

      ResumeLayout();
   }

   private void CalcLocationAndSize()
   {
      this.Size        = new Size(TheGrid.Width + 2 * ZXC.QunMrgn, SystemInformation.WorkingArea.Height - ZXC.Q5un);
      TheGrid.Height   = this.Size.Height - hamp_Fond.Bottom - ZXC.Q2un;
      gridSum.Location = new Point(TheGrid.Left, TheGrid.Bottom);
   }


   #endregion Constructor

   #region hampers

   private void InitializeHamperFond(out VvHamper hamper)
   {
      hamper          = new VvHamper(6, 1, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q5un, ZXC.Q6un, ZXC.Q5un, ZXC.Q6un, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl1 = hamper.CreateVvLabel(0, 0, "Ukupan Fond    : ", ContentAlignment.MiddleRight);
      Label lbl2 = hamper.CreateVvLabel(2, 0, "Raspoloživ Fond: ", ContentAlignment.MiddleRight);
      Label lbl3 = hamper.CreateVvLabel(4, 0, "Iskorišten Fond: ", ContentAlignment.MiddleRight);

      tbx_FndUkupno      = hamper.CreateVvTextBox(1, 0, "tbx_FndUkupno"     , "", 12);
      tbx_FndRaspolozivo = hamper.CreateVvTextBox(3, 0, "tbx_FndRaspolozivo", "", 12);
      tbx_FndIskoristeno = hamper.CreateVvTextBox(5, 0, "tbx_FndIskoristeno", "", 12);


      tbx_FndUkupno     .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_FndRaspolozivo.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_FndIskoristeno.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_FndUkupno.JAM_FieldExitMethod = new EventHandler(FndUkupnoExitMethod);
      this.ControlForInitialFocus = tbx_FndUkupno;
  
      VvHamper.Open_Close_Fields_ForWriting(tbx_FndUkupno     , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvOtherUC );
      VvHamper.Open_Close_Fields_ForWriting(tbx_FndRaspolozivo, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC) ;
      VvHamper.Open_Close_Fields_ForWriting(tbx_FndIskoristeno, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC) ;
   }
   public void FndUkupnoExitMethod(object sender, EventArgs e)
   {
      Fld_FndRaspolozivo = Fld_FndUkupno;
   }

   #endregion hampers

   #region TheG  TheSumGrid

   public Dictionary<int, bool> TheCheckState { get; set; }

   private void CreateTheGrid()
   {
      TheGrid          = new VvDataGridView();
      TheGrid.Parent   = this;
      TheGrid.Location = new Point(ZXC.QunMrgn, hamp_Fond.Bottom + ZXC.QunMrgn);

      TheGrid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
      TheGrid.AutoGenerateColumns = false;

      TheGrid.RowHeadersBorderStyle = TheGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
      TheGrid.ColumnHeadersHeight   = ZXC.QUN;
      TheGrid.RowTemplate.Height    = ZXC.QUN + ZXC.Qun4;
      TheGrid.RowHeadersWidth       = ZXC.Q3un + ZXC.Qun4;
      TheGrid.Height                = TheGrid.ColumnHeadersHeight + TheGrid.RowTemplate.Height;

      TheGrid.CellFormatting        += new DataGridViewCellFormattingEventHandler       (VvDocumentRecordUC.grid_CellFormatting_FormatVvDateTime);
      //TheG.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(VvDocumentRecordUC.Grid_EditingControlShowing);

      VvHamper.ApplyVVColorAndStyleTabCntrolChange(TheGrid);

      TheGrid.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;

      // ============== Mixing Bound / Unbound mode (bez ovoga nemozes programski utjecati na stanje checkBox kolone) 
      TheGrid.VirtualMode = true;

      TheGrid.CellValueChanged += new DataGridViewCellEventHandler(TheGrid_CellValueChanged);
      TheGrid.CellValueNeeded  += new DataGridViewCellValueEventHandler(TheGrid_CellValueNeeded);
      TheGrid.CellValuePushed  += new DataGridViewCellValueEventHandler(TheGrid_CellValuePushed);

      TheCheckState = new Dictionary<int, bool>();
      // ============================================================================================================ 

      TheGrid.CellContentClick += new DataGridViewCellEventHandler(TheGrid_CellContentClick);
      //TheG.CellValidating += new DataGridViewCellValidatingEventHandler(TheGrid_CellValidating);

      CreateGridColumns();

      //TheG.ClearSelection(); // unselecting all selected cells tu ne sljaka

   }

   #region Failure

   void TheGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
   {
      if(e.ColumnIndex == 0 && e.RowIndex != -1)
      {
         TheCheckState[e.RowIndex] = (bool)TheGrid.Rows[e.RowIndex].Cells[0].Value;
      }
   }

   void TheGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
   {
      if(e.ColumnIndex == 0 && e.RowIndex != -1)
      {
         if(TheCheckState.ContainsKey(e.RowIndex))
            e.Value = TheCheckState[e.RowIndex];
         else
            e.Value = false;
      }
   }

   void TheGrid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
   {
      if(e.ColumnIndex == 0 && e.RowIndex != -1)
      {
         if(!TheCheckState.ContainsKey(e.RowIndex))
            TheCheckState.Add(e.RowIndex, (bool)e.Value);
         else
            TheCheckState[e.RowIndex] = (bool)e.Value;
      }
   }

   //void TheGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
   //{
   //   if(e.RowIndex == -1) return;

   //   VvDataGridView theGrid1 = sender as VvDataGridView;

   //   if(theGrid1[e.ColumnIndex, e.RowIndex] is DataGridViewCheckBoxCell)
   //   {
   //      DataGridViewCheckBoxCell cbCell = theGrid1[e.ColumnIndex, e.RowIndex] as DataGridViewCheckBoxCell;

   //      bool isChecked = (bool)cbCell.EditedFormattedValue;

   //      bool approved = UpdateFondSaldo(isChecked, theGrid1.GetDecimalCell("UkSaldo", e.RowIndex, false));

   //      if(!approved)
   //      {
   //         e.Cancel = true;
   //      }
   //   }
   //}

   #endregion Failure

   #region TheGrid_CellContentClick + UpdateFondSaldo

   void TheGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
   {
      if(e.RowIndex == -1) return;

      VvDataGridView dgv = sender as VvDataGridView;

      if(dgv[e.ColumnIndex, e.RowIndex] is DataGridViewCheckBoxCell)
      {
         DataGridViewCheckBoxCell cbCell = dgv[e.ColumnIndex, e.RowIndex] as DataGridViewCheckBoxCell;

         bool isChecked = (bool)cbCell.EditedFormattedValue;

         bool approved = UpdateFondSaldo(isChecked, dgv.GetDecimalCell("UkSaldo", e.RowIndex, false));

         if(!approved)
         {
            //checkState[e.RowIndex] = false;
            cbCell.Value = false;
            //cbCell.FalseValue = true;
            //cbCell.TrueValue  = false;
         }
      }
   }

   private bool UpdateFondSaldo(bool isChecked, decimal otsSaldo)
   {
      bool approved = true;

      // 10.10.2012:
      //if(otsSaldo < 0) otsSaldo = otsSaldo * (-1);

      if(isChecked)
      {
         Fld_FndIskoristeno += otsSaldo;

         if(Fld_FndUkupno.NotZero() && Fld_FndRaspolozivo <= 0.00M) 
         {
            ZXC.aim_emsg("Upozorenje: Prekoracili ste fond");
            approved = false;
         }
      }
      else
      {
         Fld_FndIskoristeno -= otsSaldo;
      }

      Fld_FndRaspolozivo = Fld_FndUkupno - Fld_FndIskoristeno;

      return approved;
   }

   #endregion TheGrid_CellContentClick + UpdateFondSaldo

   #region CreateGridColumns

   private void CreateGridColumns()
   {
      DataGridViewCheckBoxColumn colChbx = new DataGridViewCheckBoxColumn();
      colChbx.Name             = "OTS";
      colChbx.HeaderText       = "OTS";
      colChbx.Width            = ZXC.Q2un;
      colChbx.ReadOnly         = false;
      colChbx.DataPropertyName = "ots";
      
      TheGrid.Columns.Add(colChbx);

      AddDGVColum_String_4GridReadOnly  (TheGrid, "Vezni Dok", ZXC.Q5un, false, "TipBr");
      AddDGVColum_DateTime_4GridReadOnly(TheGrid, "Datum"    , ZXC.Q5un        ,   "OpenDokumentDokDate");
      AddDGVColum_String_4GridReadOnly  (TheGrid, "Opis"     , ZXC.Q10un, false,   "OpenDokumentOpis");
      AddDGVColum_DateTime_4GridReadOnly(TheGrid, "Valuta"   , ZXC.Q5un        ,   "OpenDokumentValuta");
      AddDGVColum_Integer_4GridReadOnly (TheGrid, "Zakas"    , ZXC.Q3un, false, 5, "Zakas");
      AddDGVColum_Decimal_4GridReadOnly (TheGrid, "Otvoreno" , ZXC.Q5un,      2,   "UkOpen");
      AddDGVColum_Decimal_4GridReadOnly (TheGrid, "Zatvoreno", ZXC.Q5un,      2,   "UkClosed");
      AddDGVColum_Decimal_4GridReadOnly (TheGrid, "Saldo"    , ZXC.Q5un,      2,   "UkSaldo");
      AddDGVColum_RecID_4GridReadOnly   (TheGrid, "FID"      , ZXC.Qun12, false, 6, "FakRecID");
      AddDGVColum_RecID_4GridReadOnly   (TheGrid, "FY"       , ZXC.Qun12, false, 6, "FakYear");

      int sumoOfColumns = 0;
      foreach(DataGridViewColumn dc in TheGrid.Columns)
      {
         sumoOfColumns += dc.Width;
      }

      TheGrid.Width  = sumoOfColumns + TheGrid.RowHeadersWidth + ZXC.QUN + ZXC.Qun4;
      TheGrid.Height = this.Size.Height - hamp_Fond.Bottom - ZXC.QUN;
   }

   #endregion CreateGridColumns

   #region TheSumGrid

   private void CreateGridSum()
   {
      gridSum        = new DataGridView();
      gridSum.Parent = this;
      gridSum.Name   = "gridSum";
      
      gridSum.Height               = ZXC.QUN + ZXC.Qun10; //23;
      gridSum.BorderStyle          = BorderStyle.FixedSingle;
      gridSum.ColumnHeadersVisible = false;
      gridSum.CellBorderStyle      = DataGridViewCellBorderStyle.None;
      gridSum.ReadOnly             = true;
      gridSum.Tag                  = "ROnly";

      gridSum.AllowUserToAddRows       =
      gridSum.AllowUserToDeleteRows    =
      gridSum.AllowUserToOrderColumns  =
      gridSum.AllowUserToResizeColumns =
      gridSum.AllowUserToResizeRows    = false;

      gridSum.RowHeadersDefaultCellStyle.Alignment = TheGrid.RowHeadersDefaultCellStyle.Alignment;
      gridSum.RowTemplate.Height                   = TheGrid.RowTemplate.Height;

      gridSum.Location = new Point(TheGrid.Left, TheGrid.Bottom);
      gridSum.Width    = TheGrid.Width;

      VvHamper.ApplyVVColorAndStyleTabCntrolChange(gridSum);

      InitializeTheSUMGrid_Columns();

      gridSum.TabStop = false;
      gridSum.ClearSelection(); // unselecting all selected cells
   }

   protected void InitializeTheSUMGrid_Columns()
   {
      DataGridViewTextBoxColumn gridSumColumn;

      foreach(DataGridViewColumn mainGridColumn in TheGrid.Columns)
      {
         gridSumColumn = new DataGridViewTextBoxColumn();

         gridSumColumn.Name                       = "SUM" + mainGridColumn.Name.TrimStart(new char[] { 't' });
         gridSumColumn.DefaultCellStyle.Alignment = mainGridColumn.DefaultCellStyle.Alignment;
         gridSumColumn.AutoSizeMode               = mainGridColumn.AutoSizeMode;
         gridSum.AutoGenerateColumns              = false;
         gridSumColumn.Width                      = mainGridColumn.Width;
         gridSumColumn.ValueType                  = mainGridColumn.ValueType;
         gridSumColumn.Visible                    = mainGridColumn.Visible;
         gridSumColumn.Tag                        = mainGridColumn.Tag;
         gridSumColumn.DefaultCellStyle.Format    = mainGridColumn.DefaultCellStyle.Format;

         gridSum.Columns.Add(gridSumColumn);
      }

      gridSum.RowHeadersWidth = TheGrid.RowHeadersWidth;
      gridSum.RowCount        = 1;
   }

   private List<OtsTipBrGroupInfo> SetDataSource(string konto, uint kupdob_cd, DateTime dateDo)
   {
      List<OtsTipBrGroupInfo> initialList = LoadIzvodUC.GetOtsDistinctList(konto, kupdob_cd, dateDo);

      TheBindingSource = new BindingSource();
      //bindingSource.DataMember = /*"kplan"*/ TheDataTable.TableName;
      TheBindingSource.DataSource = /*VirtualUntypedDataSet*/ initialList;
      TheGrid.DataSource          = TheBindingSource;

      decimal openedSum, closedSum, saldoSum;

      openedSum = initialList.Sum(ots => ots.UkOpen);
      closedSum = initialList.Sum(ots => ots.UkClosed);
      saldoSum = initialList.Sum(ots => ots.UkSaldo);

      gridSum[6, 0].Value = openedSum;
      gridSum[7, 0].Value = closedSum;
      gridSum[8, 0].Value = saldoSum;

      return initialList;
   }

   #endregion TheSumGrid
   
   #endregion TheG  TheSumGrid

   #region Fld_

   public decimal Fld_FndUkupno
   {
      get { return tbx_FndUkupno.GetDecimalField(); }
      set {        tbx_FndUkupno.PutDecimalField(value); }
   }

   public decimal Fld_FndRaspolozivo
   {
      get { return tbx_FndRaspolozivo.GetDecimalField(); }
      set {        tbx_FndRaspolozivo.PutDecimalField(value); }
   }

   public decimal Fld_FndIskoristeno
   {
      get { return tbx_FndIskoristeno.GetDecimalField(); }
      set {        tbx_FndIskoristeno.PutDecimalField(value); }
   }

   #endregion Fld_

   #region Create_DummyForDefaultFocusOts

   public void Create_DummyForDefaultFocusOts()
   {
      tbx_DummyForDefaultFocus          = new TextBox();
      tbx_DummyForDefaultFocus.Parent   = this;
      tbx_DummyForDefaultFocus.Name     = "tbx_DummyForDefaultFocusOts";
      tbx_DummyForDefaultFocus.Location = Point.Empty;
      tbx_DummyForDefaultFocus.Size     = Size.Empty;
   }

   #endregion Create_DummyForDefaultFocusOts

}

public class VvAddDataGridRowFromOtherDocDlg : VvDialog
{
   #region Fieldz
 
   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_TT, tbx_TtNum, tbx_DokNum;

   #endregion Fieldz

   #region Constructor

   public VvAddDataGridRowFromOtherDocDlg()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Spoji stavke sa drugog dokumenta";

      CreateHamper();

      dlgWidth        = hamper.Right  + ZXC.QunMrgn;
      dlgHeight       = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_TT    , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_TtNum , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_DokNum, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);

      this.cancelButton.Click += new EventHandler(cancelButton_Click); // Da supresa validaciju
      this.FormClosing += new FormClosingEventHandler(VvAddDataGridRowFromOtherDocDlg_FormClosing);
   }

   void cancelButton_Click(object sender, EventArgs e)
   {
      if(ZXC.ThisIsVektorProject == true) Fld_TtNum = 1;
      else                                Fld_DocNum = 1;
   }

   void VvAddDataGridRowFromOtherDocDlg_FormClosing(object sender, FormClosingEventArgs e)
   {
      if(Fld_TtNum.IsZero() && ZXC.ThisIsVektorProject == true)
      {
         ZXC.RaiseErrorProvider(tbx_TtNum, "Molim, zadajte broj dokumenta sa kojega želite pridodati stavke.");
         e.Cancel = true;
      }

      if(Fld_DocNum.IsZero() && ZXC.ThisIsVektorProject == false)
      {
         ZXC.RaiseErrorProvider(tbx_DokNum, "Molim, zadajte broj dokumenta sa kojega želite pridodati stavke.");
         e.Cancel = true;
      }

   }

   #endregion Constructor

   #region hamper

   private void CreateHamper()
   {
      hamper          = new VvHamper(3, 1, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q10un + ZXC.QUN, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] {            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      VvLookUpLista tTtLookUpList = ((VvDocumentRecordUC)ZXC.TheVvForm.TheVvUC).TheTtLookUpList;

      string text = ZXC.ThisIsVektorProject == true ? "TT i TtNum dokumenta 'davaoca':" : "Broj dokumenta 'davaoca':";


               hamper.CreateVvLabel        (0, 0, text,  ContentAlignment.MiddleRight);
      tbx_TT = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_tt", "Tip transakcije - racuna");
      tbx_TT.JAM_Set_LookUpTable(tTtLookUpList, (int)ZXC.Kolona.prva);
      tbx_TT.JAM_CharacterCasing = CharacterCasing.Upper;
      //tbx_TT.JAM_DataRequired = true;
      tbx_TT   .JAM_MustTabOutBeforeSubmit = true;
      tbx_TtNum = hamper.CreateVvTextBox(2, 0, "tbx_ttNum", "Ovo bolje ostavi kako je...", 6);
      tbx_TtNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_DokNum = hamper.CreateVvTextBox(1, 0, "tbx_DokNum", "Ovo bolje ostavi kako je...", 6);
      tbx_DokNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      if(ZXC.ThisIsVektorProject)
      { 
         tbx_TT    .Visible = 
         tbx_TtNum .Visible = true;
         tbx_DokNum.Visible = false;
      }
      else
      {
         tbx_TT    .Visible = 
         tbx_TtNum .Visible = false;
         tbx_DokNum.Visible = true;
      }
   }

   #endregion hamper

   #region Fld_

   public string Fld_TT
   {
      get { return tbx_TT.Text; }
      set {        tbx_TT.Text = value; }
   }
   public uint Fld_TtNum
   {
      get { return ZXC.ValOrZero_UInt(tbx_TtNum.Text); }
      set 
      { 
         tbx_TtNum.Text = value.ToString();
      }
   }
   public uint Fld_DocNum
   {
      get { return ZXC.ValOrZero_UInt(tbx_DokNum.Text); }
      set
      {
         tbx_DokNum.Text = value.ToString();
      }
   }

   #endregion Fld_

}

public class VvCreateNalogDUCDlg : VvDialog
{
   #region Fieldz
 
   private Button      okButton, cancelButton;
   private VvHamper    hamper;
   private int         dlgWidth, dlgHeight;
   private CheckBox    cbx_isOpenNalog;

   #endregion Fieldz

   #region Constructor

   public VvCreateNalogDUCDlg()
   {
      SuspendLayout();

      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Potvrdite PRENOS U FINANCIJSKO KNJIGOVODSTVO";

      CreateHamper();

      dlgWidth        = (hamper.Right  + ZXC.QunMrgn) * 2 + ZXC.Q5un;
      dlgHeight       = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      ResumeLayout();

   }

   #endregion Constructor

   #region hamper

   private void CreateHamper()
   {
      hamper          = new VvHamper(1, 1, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q8un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbx_isOpenNalog = hamper.CreateVvCheckBox_OLD(0, 0, null, "Prikaži novododani nalog", RightToLeft.No);
      cbx_isOpenNalog.Checked = true;


   }

   #endregion hamper

   #region Fld_

   public bool Fld_OpenNalogDUC
   {
      get { return cbx_isOpenNalog.Checked; }
      set {        cbx_isOpenNalog.Checked = value; }
   }
   #endregion Fld_

}

public class LoadFaktur2NalogDlg : VvDialog
{
   #region Propertiez

   private /*public*/ Faktur2NalogRulesAndData theRules;
   public Faktur2NalogRulesAndData TheRules
   {
      get { return theRules; }
      set {        theRules = value; }
   }

   private Form ThePreviewFaktur2NalogForm { get; set; }
   private Vektor.Reports.RIZ.CR_PreviewFaktur2Nalog   TheCR_PreviewFaktur2Nalog  { get; set; }
 
   public List<Faktur> fakturList ;

   #endregion Propertiez

   #region Fieldz

   private Button okButton, cancelButton;
   private int    dlgWidth, dlgHeight;

   public  LoadFaktur2NalogUC TheUC { get; private set; }
   public  Button             btn_Preview;
   private VvHamper           hamp_btnPreview;
  
   #endregion Fieldz

   #region Constructor

   public LoadFaktur2NalogDlg()
   {

      TheUC = new LoadFaktur2NalogUC();

      theRules = new Faktur2NalogRulesAndData();

    //theRules.KtoShemaDsc              = new KtoShemaDsc(ZXC.dscLuiLst_KtoShema);
      theRules.KtoShemaDsc              = ZXC.KSD;
      SuspendLayout();

      this.Font        = ZXC.vvFont.BaseFont;
      this.Style       = ZXC.vvColors.vvform_VisualStyle;
      this.BackColor   = ZXC.vvColors.userControl_BackColor;

      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Prijenos Računa u Glavnu Knjigu / Salda Konti";

      CreateTheUC();

      dlgWidth        = TheUC.Width;
      dlgHeight       = TheUC.Height + ZXC.QunMrgn * 2 + ZXC.QunBtnH;


      this.ClientSize = new Size(dlgWidth, dlgHeight);
      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
     
      InitializeHamper_Preview(out hamp_btnPreview);

      ResumeLayout();
   }

   public void GetTheRulesFields()
   {
      theRules.DateOd = TheUC.Fld_DatumOd.Date;
      theRules.DateDo = TheUC.Fld_DatumDo.EndOfDay();

      theRules.Fak2nalSet = TheUC.Fld_Faktur2NalogSet;
      theRules.IsAutomatic = false;

      if(theRules.Fak2nalSet == ZXC.Faktur2NalogSetEnum.OneExactTT) theRules.ThisTT_Only = TheUC.Fld_ThisTT_Only;
      else                                                          theRules.ThisTT_Only = null;
      //theRules.Fak2nalTimeRule = TheUC.Fld_Fak2nalTimeRule;
      //theRules.GroupIrmByMonth = TheUC.fld
     // theRules.GroupIrmByNacPlac = TheUC.Fld_IsNacPlacanja;
      theRules.PeriodDefinedVia_DokDate = true;
   }
   
   private void InitializeHamper_Preview(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", this, false);

      hamper.Location = new Point(ZXC.QunMrgn, okButton.Top);

      hamper.VvColWdt      = new int[] { ZXC.QunBtnW };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8    };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QunBtnH;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      btn_Preview = hamper.CreateVvButton(0, 0, new EventHandler(PreviewFaktur2NalogReport), "Pogledaj");
   }

   #endregion Constructor

   #region UC

   private void CreateTheUC()
   {
      TheUC.Parent   = this;
      TheUC.Location = new Point(0, 0);
   }

   #endregion UC

   #region Eveniti

   void cancelButton_Click(object sender, EventArgs e)
   {
      // tamara a zasto se ovo nikada ne poziva, cemu onda sluzi ova metyoda?
      /*if(TheCR_PreviewFaktur2Nalog != null)*/ TheCR_PreviewFaktur2Nalog.Close();

      this.Close();
   }

   private void PreviewFaktur2NalogReport(object sender, EventArgs e)
   {
      SuspendLayout();

      #region Set Form

      ThePreviewFaktur2NalogForm = new Form();

      ThePreviewFaktur2NalogForm.Show();
      
      btn_Preview.Enabled = false;

      ThePreviewFaktur2NalogForm.Font      = ZXC.vvFont.BaseFont;
      ThePreviewFaktur2NalogForm.BackColor = ZXC.vvColors.userControl_BackColor;

      ThePreviewFaktur2NalogForm.FormClosing += new FormClosingEventHandler(ThePreviewFaktur2NalogForm_FormClosing_EnabledPreviewButton);

      ThePreviewFaktur2NalogForm.Location = Point.Empty;
      ThePreviewFaktur2NalogForm.Size     = new Size(SystemInformation.WorkingArea.Width - this.Width, SystemInformation.WorkingArea.Height);
      this.Location                       = new Point(ThePreviewFaktur2NalogForm.Right, 0);

      #endregion Set Form

      #region Set VvPreviewReportUC

      VvStandAloneReportViewerUC thePreviewFaktur2NalogUC = new VvStandAloneReportViewerUC();

      thePreviewFaktur2NalogUC.Parent = ThePreviewFaktur2NalogForm;
      thePreviewFaktur2NalogUC.Dock   = DockStyle.Fill;

      #endregion Set VvPreviewReportUC

      ResumeLayout();

      #region FillDataset, CreateReportDocument, SetDataSource, AssignReportSource

      GetTheRulesFields();

      Cursor.Current = Cursors.WaitCursor;

      ZXC.VvUtilDataPackage[] nacPlacArray;

      fakturList = FakturDao.GetNeprebaceniFakturAndRtrans2NalogLists(ZXC.TheVvForm.TheDbConnection, TheRules, false, out nacPlacArray);

      TheCR_PreviewFaktur2Nalog = new Vektor.Reports.RIZ.CR_PreviewFaktur2Nalog();

      TheCR_PreviewFaktur2Nalog.Database.Tables[typeof(Faktur).Name].SetDataSource(fakturList);

      #region Fof_s
      
      CrystalDecisions.CrystalReports.Engine.FormulaFieldDefinitions dscFof = TheCR_PreviewFaktur2Nalog.DataDefinition.FormulaFields;
      
      dscFof["OD"]   .Text = "'" + this.TheUC.Fld_DatumOd.Date.ToString("dd.MM.yyyy.") + "'";
      dscFof["DO"]   .Text = "'" + this.TheUC.Fld_DatumDo.Date.ToString("dd.MM.yyyy.") + "'";
      dscFof["Prjkt"].Text = "'" + ZXC.CURR_prjkt_rec.Naziv + "'";


      switch(TheUC.Fld_Faktur2NalogSet)
      { 
         case ZXC.Faktur2NalogSetEnum.ULAZNI_VP:
             dscFof["ZaTT"].Text = "'" + this.TheUC.Fld_Faktur2NalogSet.ToString() + ": UFA, URA, UPV, UOD" + "'";
             break;
         case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP:
            dscFof["ZaTT"].Text = "'" + this.TheUC.Fld_Faktur2NalogSet.ToString()  + ": IFA, IRA, IPV, IOD" + "'";
            break;
         case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP:
            dscFof["ZaTT"].Text = "'" + this.TheUC.Fld_Faktur2NalogSet.ToString()  + ": IRM"                + "'";
            break;
         case ZXC.Faktur2NalogSetEnum.BLAGAJNA:
            dscFof["ZaTT"].Text = "'" + this.TheUC.Fld_Faktur2NalogSet.ToString()  + ": UPL, ISP"           + "'";
            break;
         case ZXC.Faktur2NalogSetEnum.OneExactTT:  
            dscFof["ZaTT"].Text = "'" + this.TheUC.Fld_ThisTT_Only                                          + "'";
            break;
      
      }

      #endregion Fof_s

      if(fakturList.Count <= 0)  MessageBox.Show("Nema neprebacenih zadanih dokumenata za zadano razdoblje");
      else                       thePreviewFaktur2NalogUC.TheReportViewer.ReportSource = TheCR_PreviewFaktur2Nalog;

      #endregion FillDataset, CreateReportDocument, SetDataSource, AssignReportSource

      ZXC.TheVvForm.ToolStripReportVisible_tsBtnOnTsReportVisible(false, ZXC.ReportMode.Done, false);

      Cursor.Current = Cursors.Default;
   }

   private void ThePreviewFaktur2NalogForm_FormClosing_EnabledPreviewButton(object sender, FormClosingEventArgs e)
   {
      btn_Preview.Enabled = true;

      // ovo se mora napraviti jer je samo jedan ts_Report  UVIJEK kada radis VvStandAloneReportViewerUC !!!
      ZXC.TheVvForm.ts_Report.Parent = ZXC.TheVvForm.tsPanel_SubModul;
      ZXC.TheVvForm.ts_Report.Visible = false;

      TheCR_PreviewFaktur2Nalog.Close();
   }

   #endregion Eveniti

}

public class LoadFaktur2NalogUC : VvOtherUC
{
   #region Fieldz

   private VvHamper         hamp_racuni, hamp_date;
   private RadioButton      rbt_ulazni, rbt_ulazni_MP, rbt_izlazni_VP, rbt_izlazni_MP, rbt_blg, rbt_intU, rbt_intI, rbt_samoTT, rbt_VMI, rbt_IRA_Rlz;
   private VvDateTimePicker dtp_DatumOD, dtp_DatumDO;
   private VvTextBox        tbx_DatumOD, tbx_DatumDO, tbx_TT, tbx_TTOpis;

   #endregion Fieldz

   #region Constructor

   public LoadFaktur2NalogUC()
   {

      SuspendLayout();

      CreateHampers();

      this.Size = new Size(hamp_date.Right + ZXC.Q2un, hamp_date.Bottom);

      Fld_DatumOd = ZXC.projectYearFirstDay;
      Fld_DatumDo = DateTime.Now;
      VvHamper.Open_Close_Fields_ForWriting(tbx_TT    , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_TTOpis, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);

      ResumeLayout();
   }

   #endregion Constructor

   #region Hampers

   private void CreateHampers()
   {
      InitializeHamper_Racuni (out hamp_racuni);
      InitializeHamper_Datum  (out hamp_date);
   }


   private void InitializeHamper_Racuni(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 10, "", this, false);

      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);

      hamper.VvColWdt      = new int[] { ZXC.Q3un + ZXC.Qun2, ZXC.Q3un, ZXC.Q10un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      rbt_ulazni     = hamper.CreateVvRadioButton(0, 0, new EventHandler(rbt_checked), "Ulazni: UFA, URA, UOD, UPV"                 , 2, 0, TextImageRelation.ImageBeforeText);
      rbt_izlazni_VP = hamper.CreateVvRadioButton(0, 1, new EventHandler(rbt_checked), "Izlazni: IFA, IRA, IOD, IPV"                , 2, 0, TextImageRelation.ImageBeforeText);
      rbt_ulazni_MP  = hamper.CreateVvRadioButton(0, 2, new EventHandler(rbt_checked), "Ulazni Maloprodaja: URM, UFM, VMI, ZPC, NIV", 2, 0, TextImageRelation.ImageBeforeText);
      rbt_VMI        = hamper.CreateVvRadioButton(0, 3, new EventHandler(rbt_checked), "Međuskaldišnica Velep/Malop : VMI"          , 2, 0, TextImageRelation.ImageBeforeText);
      rbt_izlazni_MP = hamper.CreateVvRadioButton(0, 4, new EventHandler(rbt_checked), "Izlazni Maloprodaja: IRM"                   , 2, 0, TextImageRelation.ImageBeforeText);
      rbt_blg        = hamper.CreateVvRadioButton(0, 5, new EventHandler(rbt_checked), "Blagajna: UPL, ISP"                         , 2, 0, TextImageRelation.ImageBeforeText);
      rbt_intU       = hamper.CreateVvRadioButton(0, 6, new EventHandler(rbt_checked), "Interni Ulaz: "                             , 2, 0, TextImageRelation.ImageBeforeText);
      rbt_intI       = hamper.CreateVvRadioButton(0, 7, new EventHandler(rbt_checked), "Interni Izlaz: PPR, POV"                    , 2, 0, TextImageRelation.ImageBeforeText);
      rbt_samoTT     = hamper.CreateVvRadioButton(0, 8, new EventHandler(rbt_checked), "Samo TT"                                    ,       TextImageRelation.ImageBeforeText);
      rbt_IRA_Rlz    = hamper.CreateVvRadioButton(0, 9, new EventHandler(rbt_checked), "IRA: knjiženje razlike realizacije"         , 2, 0, TextImageRelation.ImageBeforeText);
      rbt_ulazni.Checked = true;

      tbx_TT     = hamper.CreateVvTextBoxLookUp(1, 8, "tbx_TT", "Tip transakcije");
      tbx_TTOpis = hamper.CreateVvTextBox      (2, 8, "tbx_TTOpis_InVisible", "Opis tipa transkacije");
      tbx_TT.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_TT.JAM_lui_NameTaker_JAM_Name = tbx_TTOpis.JAM_Name;
      tbx_TTOpis.JAM_ReadOnly = true;
      tbx_TTOpis.Tag = ZXC.vvColors.userControl_BackColor;

   }

   private void InitializeHamper_Datum(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", this, false);

      hamper.Location = new Point(ZXC.QunMrgn, hamp_racuni.Bottom + ZXC.QunMrgn);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q4un, ZXC.QUN + ZXC.Qun2, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun2, ZXC.Qun8,           ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                    hamper.CreateVvLabel(0, 0, "Datum dokumenta OD :", ContentAlignment.MiddleRight);
      tbx_DatumOD = hamper.CreateVvTextBox(1, 0, "tbx_datumOd", "Od datuma");
      tbx_DatumOD.JAM_IsForDateTimePicker = true;
      dtp_DatumOD = hamper.CreateVvDateTimePicker(1, 0, "", tbx_DatumOD);
      dtp_DatumOD.Name = "dtp_DatumOD";
      dtp_DatumOD.Tag  = tbx_DatumOD;
      tbx_DatumOD.Tag  = dtp_DatumOD;

                    hamper.CreateVvLabel(2, 0, "DO:", ContentAlignment.MiddleRight);
      tbx_DatumDO = hamper.CreateVvTextBox(3, 0, "tbx_datumDo", "");
      tbx_DatumDO.JAM_IsForDateTimePicker = true;
      dtp_DatumDO = hamper.CreateVvDateTimePicker(3, 0, "", tbx_DatumDO);
      dtp_DatumDO.Name = "dtp_DatumDO";
      dtp_DatumDO.Tag = tbx_DatumDO;
      tbx_DatumDO.Tag = dtp_DatumDO;

      tbx_DatumOD.ContextMenu = dtp_DatumOD.ContextMenu = 
      tbx_DatumDO.ContextMenu = dtp_DatumDO.ContextMenu =  CreateNewContexMenu_Date();

   }


   #region Datumi_ContexMenu

   private VvStandardTextBoxContextMenu CreateNewContexMenu_Date()
   {
      VvStandardTextBoxContextMenu date_ContexMenu = new VvStandardTextBoxContextMenu(new MenuItem[] 
            { 
               new MenuItem("Danas"           , IspuniDatume),
               new MenuItem("Tekuća godina"   , IspuniDatume),
               new MenuItem("Tekući mjesec"   , IspuniDatume),
               new MenuItem("Prvi kvartal"    , IspuniDatume),
               new MenuItem("Drugi kvartal"   , IspuniDatume),
               new MenuItem("Treći kvartal"   , IspuniDatume),
               new MenuItem("Četvrti kvartal" , IspuniDatume),
               new MenuItem("1 -11 mjesec"    , IspuniDatume),
               new MenuItem("1 -10 mjesec"    , IspuniDatume),
               new MenuItem("1 - 9 mjesec"    , IspuniDatume),
               new MenuItem("1 - 8 mjesec"    , IspuniDatume),
               new MenuItem("1 - 7 mjesec"    , IspuniDatume),
               new MenuItem("1 - 6 mjesec"    , IspuniDatume),
               new MenuItem("1 - 5 mjesec"    , IspuniDatume),
               new MenuItem("1 - 4 mjesec"    , IspuniDatume),
               new MenuItem("1 - 3 mjesec"    , IspuniDatume),
               new MenuItem("1 - 2 mjesec"    , IspuniDatume),
               new MenuItem("Siječanj"        , IspuniDatume),
               new MenuItem("Veljača"         , IspuniDatume),
               new MenuItem("Ožujak"          , IspuniDatume),
               new MenuItem("Travanj"         , IspuniDatume),
               new MenuItem("Svibanj"         , IspuniDatume),
               new MenuItem("Lipanj"          , IspuniDatume),
               new MenuItem("Srpanj"          , IspuniDatume),
               new MenuItem("Kolovoz"         , IspuniDatume),
               new MenuItem("Rujan"           , IspuniDatume),
               new MenuItem("Listopad"        , IspuniDatume),
               new MenuItem("Studeni"         , IspuniDatume),
               new MenuItem("Prosinac"        , IspuniDatume),
               new MenuItem("-")
            });

      return date_ContexMenu;
   }

   private void IspuniDatume(object sender, EventArgs e)
   {
      MenuItem tsmi = sender as MenuItem;

      string text = tsmi.Text;
      string textOd = "";
      string textDo = "";
      string mj02 = "28"; ;

      // ovo je dobro. ostavi ovako (TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.ProjectYear;) 
      string godina = ZXC.projectYear;
      int god = int.Parse(godina);

      if(DateTime.IsLeapYear(god)) mj02 = "29";
      else mj02 = "28";

      switch(text)
      { //                              mmOD           dd.mmDO
         case "Tekuća godina"  : textOd = "01"; textDo = "31.12"; break;
         case "Tekući mjesec"  : textOd = DateTime.Today.Month.ToString(); textDo = DateTime.DaysInMonth(god, DateTime.Today.Month).ToString() + "." + DateTime.Today.Month.ToString(); break;
         case "Prvi kvartal"   : textOd = "01"; textDo = "31.03"; break;
         case "Drugi kvartal"  : textOd = "04"; textDo = "30.06"; break;
         case "Treći kvartal"  : textOd = "07"; textDo = "30.09"; break;
         case "Četvrti kvartal": textOd = "10"; textDo = "31.12"; break;
         case "1 -11 mjesec"   : textOd = "01"; textDo = "30.11"; break;
         case "1 -10 mjesec"   : textOd = "01"; textDo = "31.10"; break;
         case "1 - 9 mjesec"   : textOd = "01"; textDo = "30.09"; break;
         case "1 - 8 mjesec"   : textOd = "01"; textDo = "31.08"; break;
         case "1 - 7 mjesec"   : textOd = "01"; textDo = "31.07"; break;
         case "1 - 6 mjesec"   : textOd = "01"; textDo = "30.06"; break;
         case "1 - 5 mjesec"   : textOd = "01"; textDo = "31.05"; break;
         case "1 - 4 mjesec"   : textOd = "01"; textDo = "30.04"; break;
         case "1 - 3 mjesec"   : textOd = "01"; textDo = "31.03"; break;
         case "1 - 2 mjesec"   : textOd = "01"; textDo = mj02 + ".02"; break;
         case "Siječanj"       : textOd = "01"; textDo = "31.01"; break;
         case "Veljača"        : textOd = "02"; textDo = mj02 + ".02"; break;
         case "Ožujak"         : textOd = "03"; textDo = "31.03"; break;
         case "Travanj"        : textOd = "04"; textDo = "30.04"; break;
         case "Svibanj"        : textOd = "05"; textDo = "31.05"; break;
         case "Lipanj"         : textOd = "06"; textDo = "30.06"; break;
         case "Srpanj"         : textOd = "07"; textDo = "31.07"; break;
         case "Kolovoz"        : textOd = "08"; textDo = "31.08"; break;
         case "Rujan"          : textOd = "09"; textDo = "30.09"; break;
         case "Listopad"       : textOd = "10"; textDo = "31.10"; break;
         case "Studeni"        : textOd = "11"; textDo = "30.11"; break;
         case "Prosinac"       : textOd = "12"; textDo = "31.12"; break;
         case "Danas"          : textOd = ""  ; textDo = ""     ; break;
      }

      if(text == "Danas")
      {
         tbx_DatumOD.Text = DateTime.Today.ToString("dd.MM.yyyy");
         tbx_DatumDO.Text = DateTime.Today.ToString("dd.MM.yyyy");
      }
      else
      {
         tbx_DatumOD.Text = "01." + textOd + "." + godina;
         tbx_DatumDO.Text = textDo + "." + godina;
      }
      
      dtp_DatumOD.Value = DateTime.Parse(tbx_DatumOD.Text);
      dtp_DatumDO.Value = DateTime.Parse(tbx_DatumDO.Text);

   }

   #endregion Datumi_ContexMenu

   #endregion Hampers

   #region Fld_

   public string Fld_ThisTT_Only
   {
      get { return tbx_TT.Text; }
      set { tbx_TT.Text = value; }
   }

   public ZXC.Faktur2NalogSetEnum Fld_Faktur2NalogSet
   {
      get
      {
              if(rbt_ulazni    .Checked) return ZXC.Faktur2NalogSetEnum.ULAZNI_VP;
         else if(rbt_izlazni_VP.Checked) return ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP;
         else if(rbt_ulazni_MP .Checked) return ZXC.Faktur2NalogSetEnum.ULAZNI_MP;
         else if(rbt_VMI       .Checked) return ZXC.Faktur2NalogSetEnum.VMI;
         else if(rbt_izlazni_MP.Checked) return ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP;
         else if(rbt_blg       .Checked) return ZXC.Faktur2NalogSetEnum.BLAGAJNA;
         else if(rbt_intU      .Checked) return ZXC.Faktur2NalogSetEnum.ULAZ_INT;
         else if(rbt_intI      .Checked) return ZXC.Faktur2NalogSetEnum.PROIZ_IZLAZ;
         else if(rbt_samoTT    .Checked) return ZXC.Faktur2NalogSetEnum.OneExactTT;
         else if(rbt_IRA_Rlz   .Checked) return ZXC.Faktur2NalogSetEnum.IRA_RealizOnly;

         else throw new Exception("Fld_Faktur2NalogTime_Ulaz: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.Faktur2NalogSetEnum.ULAZNI_VP      : rbt_ulazni    .Checked = true; break;
            case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_VP    : rbt_izlazni_VP.Checked = true; break;
            case ZXC.Faktur2NalogSetEnum.ULAZNI_MP      : rbt_ulazni_MP .Checked = true; break;
            case ZXC.Faktur2NalogSetEnum.VMI            : rbt_VMI       .Checked = true; break;
            case ZXC.Faktur2NalogSetEnum.IZLAZ_RN_MP    : rbt_izlazni_MP.Checked = true; break;
            case ZXC.Faktur2NalogSetEnum.BLAGAJNA       : rbt_blg       .Checked = true; break;
            case ZXC.Faktur2NalogSetEnum.ULAZ_INT       : rbt_intU      .Checked = true; break;
            case ZXC.Faktur2NalogSetEnum.PROIZ_IZLAZ    : rbt_intI      .Checked = true; break;
            case ZXC.Faktur2NalogSetEnum.OneExactTT     : rbt_samoTT    .Checked = true; break;
            case ZXC.Faktur2NalogSetEnum.IRA_RealizOnly : rbt_IRA_Rlz   .Checked = true; break;
         }
      }
   }

   public DateTime Fld_DatumOd
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumOD.Value);
      }
      set
      {
         dtp_DatumOD.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DatumOD.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumOD);
      }
   }

   public DateTime Fld_DatumDo
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumDO.Value);
      }
      set
      {
         dtp_DatumDO.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DatumDO.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumDO);
      }
   }


   #endregion Fld_

   #region PutFields(), GetFields()


   #endregion PutFields(), GetFields()

   #region Eveniti

   private void rbt_checked(object sender, EventArgs e)
   {
      RadioButton rbt = sender as RadioButton;

      if(rbt.Checked && rbt == rbt_samoTT)
      {
         VvHamper.Open_Close_Fields_ForWriting(tbx_TT, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvOtherUC);
      }
      else
      {
         VvHamper.Open_Close_Fields_ForWriting(tbx_TT, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
      }

      if(rbt.Checked && rbt == rbt_IRA_Rlz)
      {
         ((LoadFaktur2NalogDlg)this.Parent).btn_Preview.Enabled = false;
      }
      else
      {
         ((LoadFaktur2NalogDlg)this.Parent).btn_Preview.Enabled = true;
      }

   }

   #endregion Eveniti

}


public partial class ObrProDLG : Form// VvDialog{
{
   #region Fieldz

   private ToolStripPanel    tsPanel;
   private ToolStrip         ts_izvod;
   private ToolStripButton   tsb_ucitaj, tsb_nalog, tsb_abort, tsb_qPrint;
   private XSqlConnection    theDbConnection;
      
   #endregion Fieldz

   #region Propertiez

   public ObrProUC TheUC { get; private set; }

 //private List<OTPstruct> otpList;

   private List<Ftrans> nrspIndirFtrList;
   private List<Ftrans> raspDirktFtrList;
   private List<Ftrans> proUtijekFtrList;
   private List<Faktur> rnpFakturList;

   #endregion Propertiez

   #region Constructor

   public ObrProDLG(MySqlConnection TheDbConnection)
   {
      ZXC.CurrentForm = this;

      this.theDbConnection = TheDbConnection;

      TheUC = new ObrProUC();

      SuspendLayout();

      this.Font        = ZXC.vvFont.BaseFont;
      this.BackColor   = ZXC.vvColors.userControl_BackColor;

      this.StartPosition = FormStartPosition.Manual;
      this.Text = "Obračun proizvodnje";

      CreateToolStripIzvod();
      CreateTheUC();

      this.WindowState = FormWindowState.Maximized;
      this.MinimumSize = new Size(TheUC.hamp_kta.Right + ZXC.Q3un, TheUC.Bottom + ZXC.Q6un);

      CalcSizeTheUC();

      EnableDisable_tsBtn_Nalog(false);

      this.Load += new EventHandler(TrosakProizvodnjeDLG_Load);

      this.FormClosing +=new FormClosingEventHandler(RestoreZxcCurrentForm_FormClosing);
      this.Resize += new EventHandler(ObrProDLG_Resize);
      ResumeLayout();

      VvHamper.AttachEscPressForEachControl(this);

   }

   void ObrProDLG_Resize(object sender, EventArgs e)
   {
      CalcSizeTheUC();
   }

   private void CalcSizeTheUC()
   {
      TheUC.Size   = new Size(this.Width, this.Height - ts_izvod.Height);
      TheUC.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
      
      TheUC.CalcLocationSizeAnchor_ThePolyGridTabControl();
   }

   #endregion Constructor

   #region ToolStrip+Button

   private void CreateToolStripIzvod()
   {
      tsPanel           = new ToolStripPanel();
      tsPanel.Parent    = this;
      tsPanel.Dock      = DockStyle.Top;
      tsPanel.Name      = "tsPanel_Izvod";
      tsPanel.BackColor = ZXC.vvColors.tsPanel_BackColor;

      ts_izvod                  = new ToolStrip();
      ts_izvod.Parent           = tsPanel;
      ts_izvod.Name             = "ts_izvod";
      ts_izvod.ShowItemToolTips = true;
      ts_izvod.GripStyle        = ToolStripGripStyle.Hidden;
      ts_izvod.BackColor        = ZXC.vvColors.tsPanel_BackColor;

      MenuStrip menu = new MenuStrip();
      menu.Parent    = this;
      menu.Visible   = false;

      tsb_ucitaj = new ToolStripButton("Učitaj", VvIco.UcitajRn32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.ucitajRacune.ico")), 32, 32)*/.ToBitmap(), new EventHandler(UcitajButton_Click), "tsb_ucitaj");
      tsb_ucitaj.ToolTipText = "Učitaj podatke";
      ts_izvod.Items.Add(tsb_ucitaj);

      tsb_nalog = new ToolStripButton("Nalog", VvIco.Next32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.next.ico")), 32, 32)*/.ToBitmap(), new EventHandler(NaNalogButton_Click), "tsb_nalog");
      tsb_nalog.ToolTipText = "Proknjiži obračun na nalog";
      ts_izvod.Items.Add(tsb_nalog);

      tsb_qPrint = new ToolStripButton("Print", VvIco.PrintPrw32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.printPrw.ico")), 32, 32)*/.ToBitmap(), new EventHandler(QPrint_Click), "tsb_qPrint");
      tsb_qPrint.ToolTipText = "Print tablice na ekranu";
      ts_izvod.Items.Add(tsb_qPrint);

      tsb_abort             = new ToolStripButton("Prekid", VvIco.Esc32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.esc.ico")), 32, 32)*/.ToBitmap(), new EventHandler(AbortButton_Click), "tsb_abort");
      tsb_abort.ToolTipText = "Odustani od obračuna";
      ts_izvod.Items.Add(tsb_abort);

      foreach(ToolStripButton tsb in ts_izvod.Items)
      {
         tsb.ImageScaling      = ToolStripItemImageScaling.None;
         tsb.DisplayStyle      = ToolStripItemDisplayStyle.ImageAndText;
         tsb.TextImageRelation = TextImageRelation.ImageAboveText;
         tsb.Font              = ZXC.vvFont.ToolStripBtnFont;
      }
   }

   #endregion ToolStrip+Button

   #region TheUC

   private void CreateTheUC()
   {
      TheUC.Parent   = this;
      TheUC.Location = new Point(0, ts_izvod.Bottom);
   }

   #endregion TheUC

   #region FormClosing + Load

   void TrosakProizvodnjeDLG_Load(object sender, EventArgs e)
   {
      ZXC.CurrentForm = this;
   }

   private void ThePreviewIzvodForm_FormClosing_EnabledPreviewButton(object sender, FormClosingEventArgs e)
   {
     // tsb_previewIzv.Enabled = true;

      // ovo se mora napraviti jer je samo jedan ts_Report 
      ZXC.TheVvForm.ts_Report.Parent = ZXC.TheVvForm.tsPanel_SubModul;
      ZXC.TheVvForm.ts_Report.Visible = false;
   }

   void RestoreZxcCurrentForm_FormClosing(object sender, FormClosingEventArgs e)
   {
      ZXC.CurrentForm = ZXC.TheVvForm;
   }

   #endregion FormClosing + Load

   #region Methods

   public void EnableDisable_tsBtn_Nalog(bool isListeLoaded)
   {
      //VvHamper.Enable_Disable_Fields_ForWriting(TheUC.panel_DataGrid , isIzvAccept);

      //tsb_ucitaj.Enabled = !isListeLoaded;
      tsb_nalog .Enabled = isListeLoaded;
      //tsb_abort .Enabled = isListeLoaded;

   }

   #endregion Methods


}

public partial class ObrProUC : VvOtherUC
{
   #region Fieldz

   public  Panel    panel_hampers;
   public VvHamper hamp_razdTT, hamp_anaGr, hamp_endKto, hamp_raspored, hamp_kta, hamp_readOnly, hamp_lbl;
   private int      dlgWidth, dlgHeight, nextX, nextY, razmak;

   private VvTextBox tbx_DatumOD       , tbx_DatumDO        ,
                     tbx_tipRN 	      , tbx_anaGrDirTr	   , 
                     tbx_ktoPreraspTrsk, tbx_ktoProizv		, tbx_kto7trosProizv,	
                     tbx_anaGrIndTr 	, tbx_iznosIndirTr	,
                     tbx_ktoGotProiz	, tbx_ktoIndirEnd,
                     tbx_dirTrsk       , tbx_koefDir        , tbx_status;

   private VvDateTimePicker dtp_DatumOD, dtp_DatumDO;
   private RadioButton      rbt_trDirMat, rbt_trCijProiz, rbt_ktoIndirSame, rbt_ktoIndirEnd;
   private CheckBox         cbx_isSkladGotProizv;

   public KtoShemaDsc KSD { get; set; }
   public VvInnerTabControl ThePolyGridTabControl { get; set; }

   public VvDataGridView TheGrid_1 { get; set; }
   public VvDataGridView TheGrid_2 { get; set; }
   public VvDataGridView TheGrid_3 { get; set; }
   public VvDataGridView TheGrid_4 { get; set; }
   public VvDataGridView TheGrid_5 { get; set; }
   public VvDataGridView TheGrid_6 { get; set; }

   public VvDataGridView TheSumGrid_1 { get; set; }
   public VvDataGridView TheSumGrid_2 { get; set; }
   public VvDataGridView TheSumGrid_3 { get; set; }
   public VvDataGridView TheSumGrid_4 { get; set; }
   public VvDataGridView TheSumGrid_5 { get; set; }
   public VvDataGridView TheSumGrid_6 { get; set; }


   private VvTextBox vvtbR_projektCD_otp              , vvtbR_projektCD_fa   ,  vvtbR_dokNum_ni       , vvtbR_dokNum_rd     , vvtbR_dokNum_pt      , vvtbR_konto_ni_S,
                     vvtbR_PUT_dug_year  /*  1     */ , vvtbR_narInv_fa      ,  vvtbR_ttNum_ni        , vvtbR_ttNum_rd      , vvtbR_ttNum_pt       , vvtbR_naziv_ni_S,
                     vvtbR_PUT_pot_year  /*  2     */ , vvtbR_objektCd_fa    ,  vvtbR_konto_ni        , vvtbR_konto_rd      , vvtbR_konto_pt       , vvtbR_dug_ni_S  ,
                     vvtbR_RD_razd       /*  3     */ , vvtbR_objektName_fa  ,   vvtbR_opis_ni         , vvtbR_opis_rd       , vvtbR_opis_pt       , vvtbR_pot_ni_S  ,
                     vvtbR_RD_year       /*  4=1+3 */ , vvtbR_refDok_fa      ,  vvtbR_kupdob_cd_ni    , vvtbR_kupdob_cd_rd  , vvtbR_kupdob_cd_pt   ,
                     vvtbR_NIraspored    /*  5 rni */ ,                         vvtbR_kupdob_name_ni  , vvtbR_kupdob_name_rd, vvtbR_kupdob_name_pt ,
                     vvtbR_NewTr_razd    /*  6=3+5 */ ,                         vvtbR_ticker_ni       , vvtbR_ticker_rd     , vvtbR_ticker_pt      ,
                     vvtbR_NewTr_year    /*  7=1+6 */ , vvtbR_ugCijena_fa    ,  vvtbR_tipBr_ni        , vvtbR_tipBr_rd      , vvtbR_tipBr_pt       ,
                     vvtbR_dovrsenost    /*  8     */ , vvtbR_dovrsenost_fa  ,  vvtbR_dug_ni          , vvtbR_dug_rd        , vvtbR_dug_pt         ,
                     vvtbR_Otp_year      /*  9=8*7 */ , vvtbR_status_fa      ,  vvtbR_pot_ni          , vvtbR_pot_rd        , vvtbR_pot_pt         ,
                     vvtbR_Otp_razd      /* 10=9-2 */ ,                         vvtbR_projektCD_ni    , vvtbR_projektCD_rd  , vvtbR_projektCD_pt   ,
                     vvtbR_PlanCijena                 ,
                     vvtbR_PostoRaspored              ,
                     vvtbR_investit_otp
                     ;

   private VvTextBoxColumn colVvText;
   private VvDateTimePickerColumn colDate;
   private DataGridViewTextBoxColumn colScrol;

   public bool IsPutKtoShemaDscFields_InProgress { get; set; }

   #endregion Fieldz

   #region Constructor

   public ObrProUC()
   {

      SuspendLayout();

      CreatePanelsForHampers();
      CreateHampers();
      CreateThePolyGridTabControl();

      LocationAndSize_PanelsForhampers_AndClientSize();
      CalcLocationSizeAnchor_ThePolyGridTabControl();

      if(ZXC.CURR_userName == ZXC.vvDB_systemSuperUserName || ZXC.CURR_userName == ZXC.vvDB_programSuperUserName || ZXC.CURR_user_rec.IsSuper)
      {
         VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvOtherUC);
      }
      else
      {
         VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
      }

      VvHamper.Open_Close_Fields_ForWriting(TheGrid_2, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(TheGrid_3, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(TheGrid_4, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(TheGrid_5, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(TheGrid_6, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);

    //KSD = new KtoShemaDsc(ZXC.dscLuiLst_KtoShema);
      KSD = ZXC.KSD;

      PutKtoShemaDscFields(KSD);

      IsPutKtoShemaDscFields_InProgress = true;
      Fld_DatumOd = DateTime.Now.PrevMonthFirstDay();
      Fld_DatumDo = DateTime.Now.PrevMonthLastDay();
      IsPutKtoShemaDscFields_InProgress = false;

      ResumeLayout();

      SetNrspIndirFtrListColumnIndexes();
      SetRaspDirktFtrListColumnIndexes();
      SetProUtijekFtrListColumnIndexes();
      SetRnpFakturListColumnIndexes();
      SetOTPColumnIndexes();
      SetNrspIndirSFtrListColumnIndexes();

   }

   #endregion Constructor

   #region PanelsForHampers

   private void CreatePanelsForHampers()
   {
      panel_hampers             = new Panel();
      panel_hampers.Parent      = this;
     // panel_hampers.BorderStyle = BorderStyle.FixedSingle;//BorderStyle.None;
   }
   
   private void LocationAndSize_PanelsForhampers_AndClientSize()
   {
      int razmak = ZXC.Qun8;

      dlgWidth = 7 * ZXC.Q10un + ZXC.QunMrgn;

      panel_hampers.Size     = new Size(dlgWidth, hamp_lbl.Bottom + nextY);
      panel_hampers.Location = new Point(0, 0);
      ThePolyGridTabControl.Location = new Point(0, panel_hampers.Bottom);
      
      dlgHeight              = ThePolyGridTabControl.Bottom;
      this.Size              = new Size(dlgWidth, dlgHeight);
   }

   #endregion PanelsForHampers

   #region Hampers

   private void CreateHampers()
   {
      nextX = nextY = ZXC.Qun8;
      razmak = ZXC.QUN;

      InitializeHamperRazdTT(out hamp_razdTT);

      nextX = hamp_razdTT.Right + ZXC.Qun8; 
      InitializeHamperReadOnly(out hamp_readOnly);

      nextY = hamp_razdTT.Bottom + ZXC.Qun8;
      nextX = ZXC.Qun8;
      InitializeHamperAnalitGrupa(out hamp_anaGr);
      nextY = hamp_anaGr.Bottom;

      nextX = ZXC.Qun4;
      nextY = hamp_anaGr.Bottom + ZXC.Qun8;
      InitializeHamperRasporedTr(out hamp_raspored);

      nextX = hamp_raspored.Right;
      InitializeHamperEndKto(out hamp_endKto);

      nextY = ZXC.Qun8; 
      nextX = hamp_readOnly.Right;
      InitializeHamperKonta(out hamp_kta);

      nextY = hamp_kta.Bottom;
      nextX = ZXC.Qun4;
      InitializeHamperLbl(out hamp_lbl);
      nextY = ZXC.Qun8; 

   }

   private void InitializeHamperLbl(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", panel_hampers, false, nextX, nextY, 0);
      //                                     0           1  
      hamper.VvColWdt = new int[] { ZXC.Q10un + ZXC.Q5un, ZXC.Q10un + ZXC.Q5un, ZXC.Q10un + ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "PR - početak razdoblja (stanje na početku razdoblja obračuna)", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(1, 0, "UR - u razdoblju (promet troškova unutar razdoblja)", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 0, "KR - kraj razdoblja (stanje na kraju razdoblja obračuna)", ContentAlignment.MiddleRight); 

   }

   private void InitializeHamperReadOnly(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 3, "", panel_hampers, false, nextX, nextY, 0);
      //                                     0           1  
      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                    hamper.CreateVvLabel   (0, 0, "Direktni troškovi:", ContentAlignment.MiddleRight); 
      tbx_dirTrsk = hamper.CreateVvTextBox (1, 0, "tbx_direktniTrsk"  , "", 12);
      tbx_dirTrsk.JAM_ReadOnly = true;
      tbx_dirTrsk.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_dirTrsk.JAM_Highlighted = true;

      
                 hamper.CreateVvLabel  (0, 1, "Koeficijent:", ContentAlignment.MiddleRight);
      tbx_koefDir = hamper.CreateVvTextBox(1, 1, "tbx_koef", "", 12);
      tbx_koefDir.JAM_ReadOnly = true;
      tbx_koefDir.JAM_MarkAsNumericTextBox(6, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_koefDir.JAM_Highlighted = true;

                         hamper.CreateVvLabel   (0, 2, "Neraspoređeni troškovi:", ContentAlignment.MiddleRight); 
      tbx_iznosIndirTr = hamper.CreateVvTextBox (1, 2, "tbx_iznosIndirTr"    , "", 12);
      tbx_iznosIndirTr.JAM_ReadOnly = true;
      tbx_iznosIndirTr.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_iznosIndirTr.JAM_Highlighted = true;

   }

   private void InitializeHamperRazdTT(out VvHamper hamper)
   {
      hamper = new VvHamper(7, 1, "", panel_hampers, false, nextX, nextY, 0);
      //                                     0           1        2        3        4              5                   6     
      hamper.VvColWdt      = new int[] { ZXC.Q7un, ZXC.Q4un, ZXC.Q4un, ZXC.Q5un, ZXC.Q2un, ZXC.Q2un+ZXC.Qun4, ZXC.Q2un -ZXC.Qun2};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun2, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel         (0, 0, "Razdoblje obračuna:", ContentAlignment.MiddleRight);
      tbx_DatumOD      = hamper.CreateVvTextBox       (1, 0, "tbx_datumOd", "Od datuma");
      tbx_DatumOD.JAM_IsForDateTimePicker = true;
      dtp_DatumOD      = hamper.CreateVvDateTimePicker(1, 0, "", tbx_DatumOD);
      dtp_DatumOD.Name = "dtp_DatumOD";
      dtp_DatumOD.Tag  = tbx_DatumOD;
      tbx_DatumOD.Tag  = dtp_DatumOD;

      tbx_DatumDO      = hamper.CreateVvTextBox       (2, 0, "tbx_datumDo", "");
      tbx_DatumDO.JAM_IsForDateTimePicker = true;
      dtp_DatumDO      = hamper.CreateVvDateTimePicker(2, 0, "", tbx_DatumDO);
      dtp_DatumDO.Name = "dtp_DatumDO";
      dtp_DatumDO.Tag  = tbx_DatumDO;
      tbx_DatumDO.Tag  = dtp_DatumDO;
      
      tbx_DatumOD.ContextMenu = dtp_DatumOD.ContextMenu =
                          tbx_DatumDO.ContextMenu =
                          dtp_DatumDO.ContextMenu = CreateNewContexMenu_Date();

                  hamper.CreateVvLabel  (3, 0 , "TT Rad.Nal/Proj:", ContentAlignment.MiddleRight)      ; 
      tbx_tipRN = hamper.CreateVvTextBox(4, 0, "tbx_tipRN"           , "");

                   hamper.CreateVvLabel        (5, 0, "Preskoči:", ContentAlignment.MiddleRight);
      tbx_status = hamper.CreateVvTextBoxLookUp(6, 0, "tbx_Status", "Preskoči naloge sa odobranim statusom", 1);

      tbx_status.JAM_Set_LookUpTable(ZXC.luiListaRiskStatus, (int)ZXC.Kolona.prva);
      tbx_status.JAM_lookUp_NOTobligatory = false;
      tbx_status.JAM_CharacterCasing = CharacterCasing.Upper;

      dtp_DatumOD.ValueChanged += new EventHandler(DisableNalogAction);
      dtp_DatumDO.ValueChanged += new EventHandler(DisableNalogAction);
      tbx_tipRN  .TextChanged  += new EventHandler(DisableNalogAction);
      tbx_status .TextChanged  += new EventHandler(DisableNalogAction);
   }

   private void InitializeHamperAnalitGrupa(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 1, "", panel_hampers, false, nextX, nextY, 0);
      //                                    0             1      2              3             4          5
      hamper.VvColWdt      = new int[] { ZXC.Q7un, ZXC.Q7un, ZXC.QUN , ZXC.Q5un +ZXC.Qun4,  ZXC.QUN};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,           ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel  (0, 0, "Analitička grupa konta:", ContentAlignment.MiddleRight); 
                       hamper.CreateVvLabel  (1, 0, "Direktnih troškova:", ContentAlignment.MiddleRight);
      tbx_anaGrDirTr = hamper.CreateVvTextBox(2, 0, "tbx_anaGrDirTr", "");
                       hamper.CreateVvLabel  (3, 0, "Indirektnih troškova:", ContentAlignment.MiddleRight);
      tbx_anaGrIndTr = hamper.CreateVvTextBox(4, 0, "tbx_anaGrIndTr", "");

      tbx_anaGrDirTr.TextChanged += new EventHandler(DisableNalogAction);
      tbx_anaGrIndTr.TextChanged += new EventHandler(DisableNalogAction);

    }

   private void InitializeHamperRasporedTr  (out VvHamper hamper)
   {
      hamper = new VvHamper(2, 2, "", panel_hampers, false, nextX, nextY, 0);
      
      hamper.VvColWdt      = new int[] { ZXC.Q7un, ZXC.Q6un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel       (0, 0, "Raspored troškova koef:", ContentAlignment.MiddleRight);
      rbt_trDirMat   = hamper.CreateVvRadioButton (1, 0, new EventHandler(DisableNalogAction), "Direktni troškovi", TextImageRelation.ImageBeforeText);
      rbt_trCijProiz = hamper.CreateVvRadioButton (1, 1, new EventHandler(DisableNalogAction), "Planirana cijena" , TextImageRelation.ImageBeforeText);
      rbt_trDirMat.Checked = true;
      rbt_trDirMat  .Tag = 0; //vvtbR_RD_year;
      rbt_trCijProiz.Tag = 1; //vvtbR_PlanCijena;
   }

   private void InitializeHamperEndKto(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 2, "", panel_hampers, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q7un,ZXC.Q5un + ZXC.Qun2 + ZXC.Qun8, ZXC.QUN  };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,ZXC.Qun4, ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel      (0, 0,       "Knjiži na:"  , ContentAlignment.MiddleRight);
      rbt_ktoIndirSame = hamper.CreateVvRadioButton(1, 0, null, "Isti konto"  , TextImageRelation.ImageBeforeText);
      rbt_ktoIndirEnd  = hamper.CreateVvRadioButton(1, 1, null, "Zadnja znamenka:", TextImageRelation.ImageBeforeText);
      rbt_ktoIndirEnd.Checked = true;
      
      tbx_ktoIndirEnd  = hamper.CreateVvTextBox    (2, 1, "tbx_ktoIndirEnd", "", 1);
      tbx_ktoIndirEnd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;


   }

   private void InitializeHamperKonta(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 4, "", panel_hampers, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.QUN - ZXC.Qun4, ZXC.Q2un, ZXC.Q5un, ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, 0, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbDir        = hamper.CreateVvLabel  (0,  0, "KONTA:", 1, 0, ContentAlignment.MiddleRight);
                           hamper.CreateVvLabel  (0,  0, "Raspored troškova:"             , 2, 0, ContentAlignment.MiddleRight); 
      tbx_ktoPreraspTrsk = hamper.CreateVvTextBox(3,  0, "tbx_ktoPreraspTrsk",      "Raspored troškova za obračun proizvoda i usluga (prema HSFI 10 i MRS-u 2 i MRS-u 11) - uskladištivi troškovi (na račune 60, 62 i 63)", 8);
                           hamper.CreateVvLabel  (0,  1, "Proizvodnja u  tijeku:"         , 2, 0, ContentAlignment.MiddleRight); 
      tbx_ktoProizv      = hamper.CreateVvTextBox(3,  1, "tbx_ktoProizv",           "Proizvodnja u tijeku", 8);
                           hamper.CreateVvLabel  (0,  2, "Trošak zaliha prodanih proizvoda:"   , 2, 0, ContentAlignment.MiddleRight); 
      tbx_kto7trosProizv = hamper.CreateVvTextBox(3,  2, "tbx_kto7trosProizv"  ,    "Trošak zaliha prodanih proizvoda (60, 62, 63 i 64)", 8);
                           hamper.CreateVvLabel  (1,  3, "Gotovi proizvodi na skladištu:"   , 1, 0, ContentAlignment.MiddleRight); 
      tbx_ktoGotProiz    = hamper.CreateVvTextBox(3,  3, "tbx_ktoGotProiz"     ,    "Gotovi proizvodi na skladištu", 8);

      cbx_isSkladGotProizv = hamper.CreateVvCheckBox_OLD(0, 3, null, "", RightToLeft.Yes);

      tbx_ktoPreraspTrsk.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_ktoProizv     .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_kto7trosProizv.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_ktoGotProiz   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_ktoPreraspTrsk.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_ktoProizv     .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_kto7trosProizv.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_ktoGotProiz   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);

      tbx_ktoProizv.TextChanged += new EventHandler(DisableNalogAction);

   }

   #endregion Hampers

   #region Datumi_ContexMenu

   private VvStandardTextBoxContextMenu CreateNewContexMenu_Date()
   {
      VvStandardTextBoxContextMenu date_ContexMenu = new VvStandardTextBoxContextMenu(new MenuItem[] 
            { 
               new MenuItem("Danas"           , IspuniDatume),
               new MenuItem("Tekuća godina"   , IspuniDatume),
               new MenuItem("Tekući mjesec"   , IspuniDatume),
               new MenuItem("Prvi kvartal"    , IspuniDatume),
               new MenuItem("Drugi kvartal"   , IspuniDatume),
               new MenuItem("Treći kvartal"   , IspuniDatume),
               new MenuItem("Četvrti kvartal" , IspuniDatume),
               new MenuItem("1 -11 mjesec"    , IspuniDatume),
               new MenuItem("1 -10 mjesec"    , IspuniDatume),
               new MenuItem("1 - 9 mjesec"    , IspuniDatume),
               new MenuItem("1 - 8 mjesec"    , IspuniDatume),
               new MenuItem("1 - 7 mjesec"    , IspuniDatume),
               new MenuItem("1 - 6 mjesec"    , IspuniDatume),
               new MenuItem("1 - 5 mjesec"    , IspuniDatume),
               new MenuItem("1 - 4 mjesec"    , IspuniDatume),
               new MenuItem("1 - 3 mjesec"    , IspuniDatume),
               new MenuItem("1 - 2 mjesec"    , IspuniDatume),
               new MenuItem("Siječanj"        , IspuniDatume),
               new MenuItem("Veljača"         , IspuniDatume),
               new MenuItem("Ožujak"          , IspuniDatume),
               new MenuItem("Travanj"         , IspuniDatume),
               new MenuItem("Svibanj"         , IspuniDatume),
               new MenuItem("Lipanj"          , IspuniDatume),
               new MenuItem("Srpanj"          , IspuniDatume),
               new MenuItem("Kolovoz"         , IspuniDatume),
               new MenuItem("Rujan"           , IspuniDatume),
               new MenuItem("Listopad"        , IspuniDatume),
               new MenuItem("Studeni"         , IspuniDatume),
               new MenuItem("Prosinac"        , IspuniDatume),
               new MenuItem("-")
            });

      return date_ContexMenu;
   }

   private void IspuniDatume(object sender, EventArgs e)
   {
      MenuItem tsmi = sender as MenuItem;

      string text = tsmi.Text;
      string textOd = "";
      string textDo = "";
      string mj02 = "28"; ;

      // ovo je dobro. ostavi ovako (TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.ProjectYear;) 
      string godina = ZXC.projectYear;
      int god = int.Parse(godina);

      if(DateTime.IsLeapYear(god)) mj02 = "29";
      else mj02 = "28";

      switch(text)
      { //                              mmOD           dd.mmDO
         case "Tekuća godina"  : textOd = "01"; textDo = "31.12"; break;
         case "Tekući mjesec"  : textOd = DateTime.Today.Month.ToString(); textDo = DateTime.DaysInMonth(god, DateTime.Today.Month).ToString() + "." + DateTime.Today.Month.ToString(); break;
         case "Prvi kvartal"   : textOd = "01"; textDo = "31.03"; break;
         case "Drugi kvartal"  : textOd = "04"; textDo = "30.06"; break;
         case "Treći kvartal"  : textOd = "07"; textDo = "30.09"; break;
         case "Četvrti kvartal": textOd = "10"; textDo = "31.12"; break;
         case "1 -11 mjesec"   : textOd = "01"; textDo = "30.11"; break;
         case "1 -10 mjesec"   : textOd = "01"; textDo = "31.10"; break;
         case "1 - 9 mjesec"   : textOd = "01"; textDo = "30.09"; break;
         case "1 - 8 mjesec"   : textOd = "01"; textDo = "31.08"; break;
         case "1 - 7 mjesec"   : textOd = "01"; textDo = "31.07"; break;
         case "1 - 6 mjesec"   : textOd = "01"; textDo = "30.06"; break;
         case "1 - 5 mjesec"   : textOd = "01"; textDo = "31.05"; break;
         case "1 - 4 mjesec"   : textOd = "01"; textDo = "30.04"; break;
         case "1 - 3 mjesec"   : textOd = "01"; textDo = "31.03"; break;
         case "1 - 2 mjesec"   : textOd = "01"; textDo = mj02 + ".02"; break;
         case "Siječanj"       : textOd = "01"; textDo = "31.01"; break;
         case "Veljača"        : textOd = "02"; textDo = mj02 + ".02"; break;
         case "Ožujak"         : textOd = "03"; textDo = "31.03"; break;
         case "Travanj"        : textOd = "04"; textDo = "30.04"; break;
         case "Svibanj"        : textOd = "05"; textDo = "31.05"; break;
         case "Lipanj"         : textOd = "06"; textDo = "30.06"; break;
         case "Srpanj"         : textOd = "07"; textDo = "31.07"; break;
         case "Kolovoz"        : textOd = "08"; textDo = "31.08"; break;
         case "Rujan"          : textOd = "09"; textDo = "30.09"; break;
         case "Listopad"       : textOd = "10"; textDo = "31.10"; break;
         case "Studeni"        : textOd = "11"; textDo = "30.11"; break;
         case "Prosinac"       : textOd = "12"; textDo = "31.12"; break;
         case "Danas"          : textOd = ""  ; textDo = ""     ; break;
      }

      if(text == "Danas")
      {
         tbx_DatumOD.Text = DateTime.Today.ToString("dd.MM.yyyy");
         tbx_DatumDO.Text = DateTime.Today.ToString("dd.MM.yyyy");
      }
      else
      {
         tbx_DatumOD.Text = "01." + textOd + "." + godina;
         tbx_DatumDO.Text = textDo + "." + godina;
      }
      
      dtp_DatumOD.Value = DateTime.Parse(tbx_DatumOD.Text);
      dtp_DatumDO.Value = DateTime.Parse(tbx_DatumDO.Text);

   }

   #endregion Datumi_ContexMenu

   #region Fld_

   public DateTime Fld_DatumOd
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumOD.Value);
      }
      set
      {
         dtp_DatumOD.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DatumOD.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumOD);
      }
   }

   public DateTime Fld_DatumDo
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumDO.Value);
      }
      set
      {
         dtp_DatumDO.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DatumDO.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumDO);
      }
   }

   public string  Fld_ObrProTT     { get { return tbx_tipRN           .Text;              } set { tbx_tipRN           .Text =          value ; } }  
   public string  Fld_NiAnaGR      { get { return tbx_anaGrIndTr      .Text;              } set { tbx_anaGrIndTr      .Text =          value ; } }  
  // public string  Fld_NiKtoRoot    { get { return tbx_rootKtoIndTr    .Text;              } set { tbx_rootKtoIndTr    .Text =          value ; } }  
   public string  Fld_KtoObrade    { get { return tbx_ktoProizv       .Text;              } set { tbx_ktoProizv       .Text =          value ; } }  
   public string  Fld_RdAnaGR      { get { return tbx_anaGrDirTr      .Text;              } set { tbx_anaGrDirTr      .Text =          value ; } }  
  // public string  Fld_RdKtoRoot    { get { return tbx_rootKtoDirTr    .Text;              } set { tbx_rootKtoDirTr    .Text =          value ; } }  
  
   public string  Fld_KtoPreraspTrsk   { get { return tbx_ktoPreraspTrsk  .Text;              } set { tbx_ktoPreraspTrsk  .Text =          value ; } }  
   public string  Fld_Kto7trosProizv   { get { return tbx_kto7trosProizv  .Text;              } set { tbx_kto7trosProizv  .Text =          value ; } }  
   public string  Fld_KtoGotProiz      { get { return tbx_ktoGotProiz     .Text;              } set { tbx_ktoGotProiz     .Text =          value ; } }  
   public string  Fld_KtoIndirEnd      { get { return tbx_ktoIndirEnd     .Text;              } set { tbx_ktoIndirEnd     .Text =          value ; } }
   public decimal Fld_A_IznosIndirTr   { get { return tbx_iznosIndirTr    .GetDecimalField(); } set { tbx_iznosIndirTr    .PutDecimalField(value); } }
   public bool    Fld_IsSkladGotProizv { get { return cbx_isSkladGotProizv.Checked;           } set { cbx_isSkladGotProizv.Checked =       value ; } }
   public bool    Fld_IsDirekt         
   {
      get
      {
         if     (rbt_trDirMat  .Checked) return  true;
         else if(rbt_trCijProiz.Checked) return  false;

         else throw new Exception("Fld_IsDirekt: who df is checked?");
      }
      set
      {
         if(value == true) rbt_trDirMat  .Checked = true;
         else              rbt_trCijProiz.Checked = true;
      }
   }
   public bool    Fld_IsKtoEndSame     
   {
      get
      {
         if     (rbt_ktoIndirSame.Checked) return  true;
         else if(rbt_ktoIndirEnd .Checked) return  false;

         else throw new Exception("Fld_IsKtoEndSame: who df is checked?");
      }
      set
      {
         if(value == true) rbt_ktoIndirSame.Checked = true;
         else              rbt_ktoIndirEnd .Checked = true;
      }
   }
   //public bool    Fld_IsAutoSave       { get { return cbx_AutoSave.Checked; } set { cbx_AutoSave.Checked = value; } }

   public decimal Fld_DirektniTr { get { return tbx_dirTrsk.GetDecimalField(); } set { tbx_dirTrsk.PutDecimalField(value); } }
   public decimal Fld_KoefDir    { get { return tbx_koefDir.GetDecimalField(); } set { tbx_koefDir.PutDecimalField(value); } }
   public string  Fld_SkipSatus  { get { return tbx_status .Text             ; } set { tbx_status .Text =          value ; } }  

   #endregion Fld_

   #region PutShemaKontoFields(), GetShemaKontoFields()

   private void PutKtoShemaDscFields(KtoShemaDsc KSD)
   {
      IsPutKtoShemaDscFields_InProgress = true;

      Fld_ObrProTT         = KSD.Dsc_otp_obrProTT        ;
      Fld_NiAnaGR          = KSD.Dsc_otp_niAnaGR         ;
    //Fld_SkipSatus        = KSD.Dsc_otp_niKtoRoot       ; 
      Fld_SkipSatus        = KSD.Dsc_otp_skipSatus       ; 
      Fld_KtoObrade        = KSD.Dsc_otp_ktoObrade       ;
      Fld_RdAnaGR          = KSD.Dsc_otp_rdAnaGR         ;
    //Fld_RdKtoRoot        = KSD.Dsc_otp_rdKtoRoot       ;
      Fld_KtoPreraspTrsk   = KSD.Dsc_otp_ktoPrerspTrsk   ;
      Fld_Kto7trosProizv   = KSD.Dsc_otp_kto7trosProizv  ;
      Fld_KtoGotProiz      = KSD.Dsc_otp_ktoGotProiz     ;
      Fld_KtoIndirEnd      = KSD.Dsc_otp_ktoIndirEnd     ;
      Fld_IsSkladGotProizv = KSD.Dsc_otp_IsSkladGotProizv;
      Fld_IsDirekt         = KSD.Dsc_otp_IsDirekt        ;
      Fld_IsKtoEndSame     = KSD.Dsc_otp_IsKtoEndSame    ;
    //Fld_IsAutoSave       = KSD.Dsc_otp_IsAutoSave      ;

      IsPutKtoShemaDscFields_InProgress = false;
   }

   public bool GetKtoShemaDscFields() // bool je za validaciju 
   {
      KSD.Dsc_otp_obrProTT         = Fld_ObrProTT         ;
      KSD.Dsc_otp_niAnaGR          = Fld_NiAnaGR          ;
    //KSD.Dsc_otp_niKtoRoot        = Fld_SkipSatus        ;
      KSD.Dsc_otp_skipSatus        = Fld_SkipSatus;
      KSD.Dsc_otp_ktoObrade        = Fld_KtoObrade        ;
      KSD.Dsc_otp_rdAnaGR          = Fld_RdAnaGR          ;
    //KSD.Dsc_otp_rdKtoRoot        = Fld_RdKtoRoot        ;
      KSD.Dsc_otp_ktoPrerspTrsk    = Fld_KtoPreraspTrsk   ;
      KSD.Dsc_otp_kto7trosProizv   = Fld_Kto7trosProizv   ;
      KSD.Dsc_otp_ktoGotProiz      = Fld_KtoGotProiz      ;
      KSD.Dsc_otp_ktoIndirEnd      = Fld_KtoIndirEnd      ;
      KSD.Dsc_otp_IsSkladGotProizv = Fld_IsSkladGotProizv ;
      KSD.Dsc_otp_IsDirekt         = Fld_IsDirekt         ;
      KSD.Dsc_otp_IsKtoEndSame     = Fld_IsKtoEndSame     ;
    //KSD.Dsc_otp_IsAutoSave       = Fld_IsAutoSave       ;

      KSD.SaveDscToLookUpItemList();

      if(
         KSD.Dsc_otp_obrProTT      .IsEmpty() ||
         KSD.Dsc_otp_niAnaGR       .IsEmpty() ||
       //KSD.Dsc_otp_niKtoRoot     .IsEmpty() ||
         KSD.Dsc_otp_ktoObrade     .IsEmpty() ||
         KSD.Dsc_otp_rdAnaGR       .IsEmpty() ||
       //KSD.Dsc_otp_rdKtoRoot     .IsEmpty() ||
         KSD.Dsc_otp_ktoPrerspTrsk .IsEmpty() ||
         KSD.Dsc_otp_kto7trosProizv.IsEmpty() ||
         KSD.Dsc_otp_ktoGotProiz   .IsEmpty() ||
         KSD.Dsc_otp_skipSatus     .IsEmpty()
        ) 
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nepotpuni podaci.");
         return true;
      }

      return false;
   }

   #endregion PutShemaKontoFields(), GetShemaKontoFields()

   #region TabPageTitle

   public string TabPageTitle1 { get { return "°  Obračun Troškova Proizvodnje  °"; } }
   public string TabPageTitle2 { get { return "°  Projekti / Radni Nalozi  °"; } }
   public string TabPageTitle3 { get { return "°  Neraspoređeni Indirektni Tr. A  °"; } }
   public string TabPageTitle4 { get { return "°  Direktni Troškovi Proizvodnje  °"; } }
   public string TabPageTitle5 { get { return "°  Proizvodnja U Tijeku  °"; } }
   public string TabPageTitle6 { get { return "°  Neraspoređeni Indirektni Tr. S  °"; } }

   #endregion TabPageTitle

   #region CreateThePolyGridTabControl

   private void CreateThePolyGridTabControl()
   {
      ThePolyGridTabControl          = new VvInnerTabControl();
      ThePolyGridTabControl.Parent   = this;
      ThePolyGridTabControl.Location = new Point(ZXC.Qun8, panel_hampers.Bottom + ZXC.Qun8);

      ThePolyGridTabControl.TabPages.Add(new VvInnerTabPage(TabPageTitle1, TabPageTitle1, ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
      ThePolyGridTabControl.TabPages.Add(new VvInnerTabPage(TabPageTitle2, TabPageTitle2, ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
      ThePolyGridTabControl.TabPages.Add(new VvInnerTabPage(TabPageTitle6, TabPageTitle6, ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
      ThePolyGridTabControl.TabPages.Add(new VvInnerTabPage(TabPageTitle3, TabPageTitle3, ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
      ThePolyGridTabControl.TabPages.Add(new VvInnerTabPage(TabPageTitle4, TabPageTitle4, ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
      ThePolyGridTabControl.TabPages.Add(new VvInnerTabPage(TabPageTitle5, TabPageTitle5, ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));

      ThePolyGridTabControl.TabPages[TabPageTitle1].BackColor = ZXC.vvColors.tabPage4TheG_BackColor;
      ThePolyGridTabControl.TabPages[TabPageTitle2].BackColor = Color.NavajoWhite;

      ThePolyGridTabControl.TabPages[TabPageTitle1].Tag = ZXC.vvColors.tabPage4TheG_BackColor;
      ThePolyGridTabControl.TabPages[TabPageTitle2].Tag = Color.NavajoWhite;

      TheGrid_1 = CreateTheGrid(ThePolyGridTabControl.TabPages[TabPageTitle1], TabPageTitle1);
      TheGrid_2 = CreateTheGrid(ThePolyGridTabControl.TabPages[TabPageTitle2], TabPageTitle2);
      TheGrid_3 = CreateTheGrid(ThePolyGridTabControl.TabPages[TabPageTitle3], TabPageTitle3);
      TheGrid_4 = CreateTheGrid(ThePolyGridTabControl.TabPages[TabPageTitle4], TabPageTitle4);
      TheGrid_5 = CreateTheGrid(ThePolyGridTabControl.TabPages[TabPageTitle5], TabPageTitle5);
      TheGrid_6 = CreateTheGrid(ThePolyGridTabControl.TabPages[TabPageTitle6], TabPageTitle6);
      TheGrid_1.ColumnHeadersHeight = ZXC.Q2un;

      CreateColumn_1(TheGrid_1);
      CreateColumn_2(TheGrid_2);
      CreateColumn_3(TheGrid_3);
      CreateColumn_4(TheGrid_4);
      CreateColumn_5(TheGrid_5);
      CreateColumn_6(TheGrid_6);

      TheSumGrid_1 = CreateSumGrid(TheGrid_1, TheGrid_1.Parent, "SUM" + TabPageTitle1);
      TheSumGrid_2 = CreateSumGrid(TheGrid_2, TheGrid_2.Parent, "SUM" + TabPageTitle2);
      TheSumGrid_3 = CreateSumGrid(TheGrid_3, TheGrid_3.Parent, "SUM" + TabPageTitle3);
      TheSumGrid_4 = CreateSumGrid(TheGrid_4, TheGrid_4.Parent, "SUM" + TabPageTitle4);
      TheSumGrid_5 = CreateSumGrid(TheGrid_5, TheGrid_5.Parent, "SUM" + TabPageTitle5);
      TheSumGrid_6 = CreateSumGrid(TheGrid_6, TheGrid_6.Parent, "SUM" + TabPageTitle6);

      InitializeTheSUMGrid_Columns(TheGrid_1);
      InitializeTheSUMGrid_Columns(TheGrid_2);
      InitializeTheSUMGrid_Columns(TheGrid_3);
      InitializeTheSUMGrid_Columns(TheGrid_4);
      InitializeTheSUMGrid_Columns(TheGrid_5);
      InitializeTheSUMGrid_Columns(TheGrid_6);

      //TheGrid_1.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged);
      //TheGrid_1.Scroll += new ScrollEventHandler(TheGrid_Scroll);
      //TheGrid_2.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged);
      //TheGrid_3.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged);
      //TheGrid_4.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged);
      //TheGrid_5.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged);
      //TheGrid_6.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged);

   }

   private void TheGrid_Scroll(object sender, ScrollEventArgs e)
   {
      VvDataGridView theGrid = sender as VvDataGridView;
      DataGridView theSumGrid = theGrid.TheLinkedGrid_Sum;

      if(theSumGrid != null) theSumGrid.HorizontalScrollingOffset = theGrid.HorizontalScrollingOffset;
   }
   public void CalcLocationSizeAnchor_ThePolyGridTabControl()
   {
      

      panel_hampers.Size = new Size(dlgWidth, hamp_lbl.Bottom);
      panel_hampers.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

      ThePolyGridTabControl.Size = new Size(this.Width - ZXC.Q2un, this.Height - panel_hampers.Height - ZXC.QUN);
      ThePolyGridTabControl.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

      CalcLocationSizeAnchor_TheDGVAndTheSumGrid(TheGrid_1);
      CalcLocationSizeAnchor_TheDGVAndTheSumGrid(TheGrid_2);
      CalcLocationSizeAnchor_TheDGVAndTheSumGrid(TheGrid_3);
      CalcLocationSizeAnchor_TheDGVAndTheSumGrid(TheGrid_4);
      CalcLocationSizeAnchor_TheDGVAndTheSumGrid(TheGrid_5);
      CalcLocationSizeAnchor_TheDGVAndTheSumGrid(TheGrid_6);
   }

   #endregion CreateThePolyGridTabControl

   #region TheG

   private VvDataGridView CreateTheGrid(Control parent, string name)
   {
      VvDataGridView theGrid      = new VvDataGridView();

      theGrid.Name                = name;
      theGrid.Parent              = parent;
      theGrid.Location            = new Point(ZXC.Qun2, ZXC.Qun2);
      theGrid.AutoGenerateColumns = false;

      theGrid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

      theGrid.RowHeadersBorderStyle = theGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
      theGrid.ColumnHeadersHeight   = ZXC.QUN;
      theGrid.RowTemplate.Height    = ZXC.QUN;
      theGrid.RowHeadersWidth       = ZXC.Q2un + ZXC.Qun4;

      theGrid.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(VvDocumentRecordUC.Grid_EditingControlShowing);
      theGrid.CellBeginEdit         += new DataGridViewCellCancelEventHandler           (VvDocumentRecordUC.grid_CellBeginEdit_AtachJAM_EventHandlers);
      theGrid.CellEndEdit           += new DataGridViewCellEventHandler                 (VvDocumentRecordUC.grid_CellEndEdit_DetachJAM_EventHandlers);

      theGrid.CellFormatting        += new DataGridViewCellFormattingEventHandler       (VvDocumentRecordUC.grid_CellFormatting_FormatVvDateTime);
      theGrid.Validating            += new System.ComponentModel.CancelEventHandler     (VvDocumentRecordUC.grid_Validating);

      VvHamper.ApplyVVColorAndStyleTabCntrolChange(theGrid);
      VvHamper.Open_Close_Fields_ForWriting(theGrid, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);

      theGrid.AllowUserToAddRows       =
      theGrid.AllowUserToDeleteRows    =
      theGrid.AllowUserToOrderColumns  =
      theGrid.AllowUserToResizeColumns =
      theGrid.AllowUserToResizeRows    = false;

      theGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.PowderBlue;
      theGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.DarkSlateGray;
      theGrid.RowHeadersDefaultCellStyle.BackColor    = Color.PowderBlue; //Color.FloralWhite;
      theGrid.RowHeadersDefaultCellStyle.ForeColor    = Color.DarkSlateGray;
      
      return theGrid;
   }

   private VvDataGridView CreateSumGrid(VvDataGridView theGrid, Control _parent, string _name)
   {
      VvDataGridView theSumGrid = new VvDataGridView();

      theGrid.TheLinkedGrid_Sum = theSumGrid;

      theSumGrid.Parent = _parent;
      theSumGrid.Name   = _name;

      theSumGrid.Height = ZXC.QUN + ZXC.Qun10; //23;

      theSumGrid.BorderStyle = BorderStyle.FixedSingle;
      VvHamper.ApplyVVColorAndStyleTabCntrolChange(theSumGrid);
      theSumGrid.ColumnHeadersVisible = false;
      theSumGrid.CellBorderStyle      = DataGridViewCellBorderStyle.SingleVertical;// DataGridViewCellBorderStyle.None;
      theSumGrid.ReadOnly             = true;
      theSumGrid.Tag                  = theGrid;

      theSumGrid.AllowUserToAddRows       =
      theSumGrid.AllowUserToDeleteRows    =
      theSumGrid.AllowUserToOrderColumns  =
      theSumGrid.AllowUserToResizeColumns =
      theSumGrid.AllowUserToResizeRows    = false;

      theSumGrid.RowHeadersDefaultCellStyle.Alignment = theGrid.RowHeadersDefaultCellStyle.Alignment;
      theSumGrid.RowTemplate.Height                   = theGrid.RowTemplate.Height;

      //theSumGrid.ScrollBars = ScrollBars.None;

      theSumGrid.ScrollBars = ScrollBars.None;

      theSumGrid.Location = new Point(theGrid.Left, theGrid.Bottom);

      return theSumGrid;
   }

   private void InitializeTheSUMGrid_Columns(VvDataGridView theGrid)
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

   public void CalcLocationSizeAnchor_TheDGVAndTheSumGrid(VvDataGridView theGrid)
   {
      DataGridView theSumGrid   = theGrid.TheLinkedGrid_Sum;
   
      theGrid.Location = new Point(ZXC.Qun2, ZXC.Qun2);
      theGrid.Size     = new Size(theGrid.Parent.Width - ZXC.QUN, theGrid.Parent.Height - ZXC.Q2un - theSumGrid.Height);

      theGrid.Anchor        = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      theSumGrid.Width   = theGrid.Width;
      
      theGrid.Height = theGrid.Parent.Height - ZXC.Q2un - theSumGrid.Height;

      theSumGrid.Location = new Point(theGrid.Location.X, theGrid.Bottom + ZXC.Qun12);
      theSumGrid.Anchor   = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

      theSumGrid.Scroll += new ScrollEventHandler(theSumGrid_Scroll);
 //     theGrid.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged);
      theGrid.Scroll += new ScrollEventHandler(TheGrid_Scroll);


   }
   private void theSumGrid_Scroll(object sender, ScrollEventArgs e)
   {
      DataGridView theSumGrid = sender as DataGridView;
      VvDataGridView theGrid = (VvDataGridView)(theSumGrid.Tag);

      theGrid.HorizontalScrollingOffset = theSumGrid.HorizontalScrollingOffset;
      int sumNalog = theGrid.RowHeadersWidth;
      int sumSum = theSumGrid.RowHeadersWidth /*+ 1*/;

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
   private void grid_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
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

   #endregion TheG

   #region GridColumns

   private void CreateColumn_1(VvDataGridView theGrid)
   {
      vvtbR_projektCD_otp = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (   "vvtb4ColR_projektCD_otp", null, -12, "Projekt"                                                              );                   colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_projektCD_otp, null, "R_projektCD"    , "Projekt\n1"                       , ZXC.Q5un); vvtbR_projektCD_otp.JAM_ReadOnly = true;
      vvtbR_investit_otp  = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (   "vvtb4ColR_projektCD_otp", null, -12, "Nar/Investi"                                                          );                   colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_investit_otp , null, "R_investit_otp" , "Naručitelj / Investitor\n2"       ,        0); vvtbR_investit_otp.JAM_ReadOnly = true;
      colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      colVvText.MinimumWidth = ZXC.Q6un;

      vvtbR_dovrsenost    = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_dovrsenost"   , null, -12, "Unos % dovršenosti"); 
      vvtbR_dovrsenost.JAM_IsForPercent = true;                            
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_dovrsenost   , null, "R_dovrsenost"   , "Dovrš %\n3"       , ZXC.Q3un - ZXC.Qun4);                                               colVvText.MinimumWidth = ZXC.Q3un- ZXC.Qun4;
      vvtbR_dovrsenost.JAM_FieldExitMethod = new EventHandler(CalcOnExitPostoDovrseno);  
     
      vvtbR_PlanCijena    = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_PlanCijena"   , null, -12, "Planirana cijena proizvodnje"                                         );                   colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_PlanCijena   , null, "R_PlanCijena"   , "PlaniranaCijena\n"+  "4"            /*                                          */, ZXC.Q5un - ZXC.Qun4); vvtbR_PlanCijena   .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un;           vvtbR_PlanCijena   .JAM_ReadOnly = true;
      vvtbR_PUT_dug_year  = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_PUT_dug_year" , null, -12, "Proizvodnja u tijeku od 1.1. tekuće godine do zadanog razdoblja"      );                   colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_PUT_dug_year , null, "R_PUT_dug_year" , "ProizvUtijekuPR\n"+  "5"            /*                                          */, ZXC.Q5un - ZXC.Qun4); vvtbR_PUT_dug_year .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un;           vvtbR_PUT_dug_year .JAM_ReadOnly = true;
      vvtbR_PUT_pot_year  = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_PUT_pot_year" , null, -12, "Završena proizvodnja od 1.1. tekuće godine do zadanog razdoblja"      );                   colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_PUT_pot_year , null, "R_PUT_pot_year" , "ProizvedenoPR\n"  +  "6"            /*                                          */, ZXC.Q5un - ZXC.Qun4); vvtbR_PUT_pot_year .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un;           vvtbR_PUT_pot_year .JAM_ReadOnly = true;
      vvtbR_RD_razd       = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_RD_razd"      , null, -12, "Direktni troškovi zadanog razdoblja"                                  );                   colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_RD_razd      , null, "R_RD_razd"      , "DirektniTr\n"     +  "7"            /*                                          */, ZXC.Q5un - ZXC.Qun4); vvtbR_RD_razd      .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un;           vvtbR_RD_razd      .JAM_ReadOnly = true;
      vvtbR_RD_year       = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_RD_year"      , null, -12, "Raspoređeni troškovi od 1.1. tekuće godine"                           );                   colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_RD_year      , null, "R_RD_year"      , "RaspoređenTr\n"   +  "8 (5 + 7)"    /*PUT_dug_year + RD_razd                    */, ZXC.Q5un - ZXC.Qun4); vvtbR_RD_year      .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un;           vvtbR_RD_year      .JAM_ReadOnly = true;
      vvtbR_NIraspored    = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_NIraspored"   , null, -12, "Raspored neraspoređenih troškova zadanog razdoblja"                   );                   colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_NIraspored   , null, "R_NIraspored"   , "NovoRaspTr\n"     +  "9 (8 * koef)" /*RD_year      * Fld_KoefDir                */, ZXC.Q5un - ZXC.Qun4); vvtbR_NIraspored   .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un;           vvtbR_NIraspored   .JAM_ReadOnly = true;      colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;
      vvtbR_PostoRaspored = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_PostoRaspored", null, -12, "Postotni udio u nerasporedjenim troskovima"); vvtbR_PostoRaspored.JAM_IsForPercent = true; colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_PostoRaspored, null, "R_PostoRaspored", "Udio %\n"         + "10"            /*                                          */, ZXC.Q3un - ZXC.Qun4); vvtbR_PostoRaspored.JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q3un- ZXC.Qun4; vvtbR_PostoRaspored.JAM_ReadOnly = true;
      vvtbR_NewTr_razd    = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_NewTr_razd"   , null, -12, "Ukupni novi troškovi zadanog razdoblja"                               );                   colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_NewTr_razd   , null, "R_NewTr_razd"   , "UkTroškoviUR\n"   + "11 (7 + 9)"    /*RD_razd      + NIraspored                 */, ZXC.Q5un - ZXC.Qun4); vvtbR_NewTr_razd   .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un;           vvtbR_NewTr_razd   .JAM_ReadOnly = true;
      vvtbR_Otp_razd      = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_Otp_razd"     , null, -12, "Završena proizvodnja zadanog razdoblja"                               );                   colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_Otp_razd     , null, "R_Otp_razd"     , "ProizvednoUR\n"   + "12 (13*3 - 6)" /*R_NewTr_year * dovrsenost - R_PUT_pot_year*/, ZXC.Q5un - ZXC.Qun4); vvtbR_Otp_razd     .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un;           vvtbR_Otp_razd     .JAM_ReadOnly = true;
      vvtbR_NewTr_year    = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_NewTr_year"   , null, -12, "Ukupni troškovi proizvodnje od 1.1. tekuće godine"                    );                   colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_NewTr_year   , null, "R_NewTr_year"   , "UkTroškoviKR\n"   + "13 (5 + 11)"   /*PUT_dug_year + NewTr_razd                 */, ZXC.Q5un - ZXC.Qun4); vvtbR_NewTr_year   .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un;           vvtbR_NewTr_year   .JAM_ReadOnly = true;
      vvtbR_Otp_year      = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_Otp_year"     , null, -12, "Završena proizvodnja od 1.1. tekuće godine do kraja zadanog razdoblja");                   colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_Otp_year     , null, "R_Otp_year"     , "ProizvedenoKR\n"  + "14 (6 + 12)"   /*PUT_pot_year + Otp_razd                   */, ZXC.Q5un - ZXC.Qun4); vvtbR_Otp_year     .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un;           vvtbR_Otp_year     .JAM_ReadOnly = true;


      if(rbt_trDirMat.Checked) vvtbR_RD_year   .JAM_Highlighted = true; // ovo ne sljaka
      else                     vvtbR_PlanCijena.JAM_Highlighted = true; // ovo ne sljaka

      vvtbR_projektCD_otp.JAM_ForeColor = Color.Black;
      vvtbR_investit_otp .JAM_ForeColor = Color.Black;
      vvtbR_dovrsenost   .JAM_ForeColor = Color.Black;
      vvtbR_PlanCijena   .JAM_ForeColor = Color.Black;
      vvtbR_PUT_dug_year .JAM_ForeColor = Color.Black; 
      vvtbR_PUT_pot_year .JAM_ForeColor = Color.Black; 
      vvtbR_RD_razd      .JAM_ForeColor = Color.DarkBlue; 
      vvtbR_RD_year      .JAM_ForeColor = Color.DarkBlue; 
      vvtbR_NIraspored   .JAM_ForeColor = Color.DarkBlue; 
      vvtbR_PostoRaspored.JAM_ForeColor = Color.DarkBlue; 
      vvtbR_NewTr_razd   .JAM_ForeColor = Color.DarkBlue; 
      vvtbR_Otp_razd     .JAM_ForeColor = Color.DarkBlue; 
      vvtbR_NewTr_year   .JAM_ForeColor = Color.DarkGreen; 
      vvtbR_Otp_year     .JAM_ForeColor = Color.DarkGreen; 


      vvtbR_projektCD_otp.JAM_BackColor = Color.AliceBlue;
      vvtbR_investit_otp .JAM_BackColor = Color.AliceBlue;
      vvtbR_PlanCijena   .JAM_BackColor = Color.AliceBlue;
      vvtbR_PUT_dug_year .JAM_BackColor = Color.AliceBlue;
      vvtbR_PUT_pot_year .JAM_BackColor = Color.AliceBlue;
      vvtbR_RD_razd      .JAM_BackColor = Color.PeachPuff;
      vvtbR_RD_year      .JAM_BackColor = Color.PeachPuff;
      vvtbR_NIraspored   .JAM_BackColor = Color.PeachPuff;
      vvtbR_PostoRaspored.JAM_BackColor = Color.PeachPuff;
      vvtbR_NewTr_razd   .JAM_BackColor = Color.PeachPuff;
      vvtbR_Otp_razd     .JAM_BackColor = Color.PeachPuff;
      vvtbR_NewTr_year   .JAM_BackColor = Color.MintCream;
      vvtbR_Otp_year     .JAM_BackColor = Color.MintCream;
      
//      vvtbR_PUT_dug_year  /*  1     */ 
//      vvtbR_PUT_pot_year  /*  2     */ 
//      vvtbR_RD_razd       /*  3     */ 
//      vvtbR_RD_year       /*  4=1+3 */  = vvtbR_PUT_dug_year + vvtbR_RD_razd
//      vvtbR_NIraspored    /*  5 rni */  = vvtbR_RD_year * Fld_KoefDir
//      vvtbR_NewTr_razd    /*  6=3+5 */  = vvtbR_RD_razd + vvtbR_NIraspored
//      vvtbR_Otp_razd      /* 10=9-2  tj 8*6 */  = vvtbR_Otp_year - vvtbR_PUT_pot_year  odnosno vvtbR_dovrsenost * vvtbR_NewTr_razd
//      vvtbR_NewTr_year    /*  7=1+6 */  = vvtbR_PUT_dug_year + vvtbR_NewTr_razd
//      vvtbR_dovrsenost    /*  8     */  
//      vvtbR_Otp_year      /*  9=8*7 */  = vvtbR_dovrsenost * vvtbR_NewTr_year
      
      
      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);

   }
   private void CreateColumn_2(VvDataGridView theGrid)
   {
      vvtbR_projektCD_fa  = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_projektCD_fa" , null, -12, "Projekt"               ); 
      colVvText           = theGrid.CreateVvTextBoxColumn(vvtbR_projektCD_fa , null, "R_projektCD"    , "Projekt"       , ZXC.Q5un); 
      vvtbR_projektCD_fa .JAM_ReadOnly = true;
     
      vvtbR_status_fa = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_status_fa", null, -12, "Status");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_status_fa, null, "R_status_fa", "Status", ZXC.Q2un);
      vvtbR_status_fa.JAM_ReadOnly = true;

      vvtbR_dovrsenost_fa = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_dovrsenost_fa", null, -12, "% Dovršenosti");
      vvtbR_dovrsenost_fa.JAM_IsForPercent = true;
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_dovrsenost_fa, null, "R_dovrsenost_fa", "Dovrš %", ZXC.Q3un - ZXC.Qun4);
      colVvText.MinimumWidth = ZXC.Q3un - ZXC.Qun4; 
      vvtbR_dovrsenost_fa.JAM_ReadOnly = true;

      vvtbR_ugCijena_fa = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_ugCijena_fa", null, -12, "PlanCijena");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_ugCijena_fa, null, "R_ugCijena_fa", "Planirana Cijena", ZXC.Q5un);
      vvtbR_ugCijena_fa.JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un; vvtbR_ugCijena_fa.JAM_ReadOnly = true;
      

      vvtbR_narInv_fa     = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_narInv_fa"    , null, -12, "Naručitelj/Investitor" ); 
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_narInv_fa    , null, "R_narInv_fa"    , "Naručitelj / Investitor", ZXC.Q8un); 
      vvtbR_narInv_fa    .JAM_ReadOnly = true;

      vvtbR_objektCd_fa   = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_objektCd_fa"  , null, -12, "Objekt šifra"          ); 
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_objektCd_fa  , null, "R_objektCd_fa"  , "ŠifObjekta"      , ZXC.Q4un); 
      vvtbR_objektCd_fa  .JAM_ReadOnly = true;

      vvtbR_objektName_fa = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_objektName_fa", null, -12, "Objekt"                ); 
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_objektName_fa, null, "R_objektName_fa", "Naziv Objekta"        , 0); 
      vvtbR_objektName_fa.JAM_ReadOnly = true;
      colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      colVvText.MinimumWidth = ZXC.Q7un;

      vvtbR_refDok_fa     = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_refDok_fa"    , null, -12, "RefDok"                ); 
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_refDok_fa    , null, "R_refDok_fa"    , "RefDok"        , ZXC.Q10un); 
      vvtbR_refDok_fa    .JAM_ReadOnly = true;

      colDate = theGrid.CreateCalendarColumn_R(null, "R_dateStart_fa", "PočIzvođ", ZXC.Q3un + ZXC.Qun2); colDate.ReadOnly = true;
      
      colDate = theGrid.CreateCalendarColumn_R(null, "R_dateRokIsp_fa", "RokIzvođ", ZXC.Q3un + ZXC.Qun2); colDate.ReadOnly = true;

     
      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);

   }
   private void CreateColumn_3(VvDataGridView theGrid)
   {
      vvtbR_projektCD_ni = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_projektCD_ni", null, -12, "Radni nalog / Projekt");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_projektCD_ni  , null, "R_projektCD"  , "Projekt"   , ZXC.Q5un);
      vvtbR_projektCD_ni.JAM_ReadOnly = true;

      colDate = theGrid.CreateCalendarColumn_R(null, "R_dokDate", "Datum", ZXC.Q3un + ZXC.Qun2);
      colDate.ReadOnly = true;

      vvtbR_dokNum_ni      = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColR_dokNum_ni"     , null, -6, "Broj naloga");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_dokNum_ni     , null, "R_dokNum"     , "DokBroj"   , ZXC.Q3un);
      vvtbR_dokNum_ni.JAM_ReadOnly = true;

      vvtbR_ttNum_ni       = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (      "vvtb4ColR_ttNum_ni"      , null, -12, "Vrsta knjiženja");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_ttNum_ni, null, "R_ttNum", "VK", ZXC.Q2un);
      vvtbR_ttNum_ni.JAM_ReadOnly = true;

      vvtbR_konto_ni = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_konto_ni", null, -12, "Konto");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_konto_ni, null, "R_konto", "Konto", ZXC.Q3un);
      vvtbR_konto_ni.JAM_ReadOnly = true;

      vvtbR_opis_ni = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_opis_ni", null, -12, "Opis");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_opis_ni, null, "R_opis", "Opis", 0);
      vvtbR_opis_ni.JAM_ReadOnly = true;
      colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      colVvText.MinimumWidth = ZXC.Q6un;

      vvtbR_kupdob_name_ni = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_kupdob_name_ni", null, -12, "Naziv partnera");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_kupdob_name_ni, null, "R_kupdob_name", "Partner", ZXC.Q8un);
      vvtbR_kupdob_name_ni.JAM_ReadOnly = true;

      vvtbR_ticker_ni = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_ticker_ni", null, -12, "Tiker partnera");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_ticker_ni, null, "R_ticker", "Ticker", ZXC.Q3un);
      vvtbR_ticker_ni.JAM_ReadOnly = true;
      
      vvtbR_kupdob_cd_ni = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColR_kupdob_cd_ni", null, -6, "Šifra partnera");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_kupdob_cd_ni, null, "R_kupdob_cd", "SifK/D", ZXC.Q3un);
      vvtbR_kupdob_cd_ni.JAM_ReadOnly = true;
     
      vvtbR_tipBr_ni = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_tipBr_ni", null, -12, "TT Br");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_tipBr_ni, null, "R_tipBr", "TT Br", ZXC.Q4un);
      vvtbR_tipBr_ni.JAM_ReadOnly = true;

      vvtbR_dug_ni = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_dug_ni", null, -12, "Iznos duguje");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_dug_ni, null, "R_dug", "Duguje", ZXC.Q5un);
      vvtbR_dug_ni.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = ZXC.Q4un;
      vvtbR_dug_ni.JAM_ReadOnly = true;

      vvtbR_pot_ni = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_pot_ni", null, -12, "Iznos potražuje");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_pot_ni, null, "R_pot", "Potražuje", ZXC.Q5un);
      vvtbR_pot_ni.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = ZXC.Q4un;
      vvtbR_pot_ni.JAM_ReadOnly = true;

      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);
   }
   private void CreateColumn_4(VvDataGridView theGrid)
   {
      vvtbR_projektCD_rd = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_projektCD_rd", null, -12, "Radni nalog / Projekt");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_projektCD_rd, null, "R_projektCD", "Projekt", ZXC.Q5un);
      vvtbR_projektCD_rd.JAM_ReadOnly = true;

      colDate = theGrid.CreateCalendarColumn_R(null, "R_dokDate", "Datum", ZXC.Q3un + ZXC.Qun2);

      vvtbR_dokNum_rd      = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColR_dokNum_rd"     , null, -6, "Broj naloga");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_dokNum_rd     , null, "R_dokNum"     , "DokBroj"   , ZXC.Q3un);
      vvtbR_dokNum_rd.JAM_ReadOnly = true;

      vvtbR_ttNum_rd       = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (      "vvtb4ColR_ttNum_rd"      , null, -12, "Vrsta knjiženja");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_ttNum_rd, null, "R_ttNum", "VK", ZXC.Q2un);
      vvtbR_ttNum_rd.JAM_ReadOnly = true;

      vvtbR_konto_rd = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_konto_rd", null, -12, "Konto");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_konto_rd, null, "R_konto", "Konto", ZXC.Q3un);
      vvtbR_konto_rd.JAM_ReadOnly = true;

      vvtbR_opis_rd = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_opis_rd", null, -12, "Opis");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_opis_rd, null, "R_opis", "Opis", 0);
      vvtbR_opis_rd.JAM_ReadOnly = true;
      colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      colVvText.MinimumWidth = ZXC.Q6un;

      vvtbR_kupdob_name_rd = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_kupdob_name_rd", null, -12, "Naziv partnera");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_kupdob_name_rd, null, "R_kupdob_name", "Partner", ZXC.Q8un);
      vvtbR_kupdob_name_rd.JAM_ReadOnly = true;

      vvtbR_ticker_rd = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_ticker_rd", null, -12, "Tiker partnera");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_ticker_rd, null, "R_ticker", "Ticker", ZXC.Q3un);
      vvtbR_ticker_rd.JAM_ReadOnly = true;

      vvtbR_kupdob_cd_rd = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColR_kupdob_cd_rd", null, -6, "Šifra partnera");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_kupdob_cd_rd, null, "R_kupdob_cd", "SifK/D", ZXC.Q3un);
      vvtbR_kupdob_cd_rd.JAM_ReadOnly = true;

      vvtbR_tipBr_rd = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_tipBr_rd", null, -12, "TT Br");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_tipBr_rd, null, "R_tipBr", "TT Br", ZXC.Q4un);
      vvtbR_tipBr_rd.JAM_ReadOnly = true;

      vvtbR_dug_rd = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_dug_rd", null, -12, "Iznos duguje");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_dug_rd, null, "R_dug", "Duguje", ZXC.Q5un);
      vvtbR_dug_rd.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = ZXC.Q4un;
      vvtbR_dug_rd.JAM_ReadOnly = true;

      vvtbR_pot_rd = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_pot_rd", null, -12, "Iznos potražuje");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_pot_rd, null, "R_pot", "Potražuje", ZXC.Q5un);
      vvtbR_pot_rd.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = ZXC.Q4un;
      vvtbR_pot_rd.JAM_ReadOnly = true;

      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);

   }
   private void CreateColumn_5(VvDataGridView theGrid)
   {
      vvtbR_projektCD_pt = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_projektCD_pt", null, -12, "Radni nalog / Projekt");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_projektCD_pt, null, "R_projektCD", "Projekt", ZXC.Q5un);
      vvtbR_projektCD_pt.JAM_ReadOnly = true;

      colDate = theGrid.CreateCalendarColumn_R(null, "R_dokDate", "Datum", ZXC.Q3un + ZXC.Qun2);

      vvtbR_dokNum_pt = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColR_dokNum_pt", null, -6, "Broj naloga");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_dokNum_pt     , null, "R_dokNum"     , "DokBroj"   , ZXC.Q3un);
      vvtbR_dokNum_pt.JAM_ReadOnly = true;

      vvtbR_ttNum_pt = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_ttNum_pt", null, -12, "Vrsta knjiženja");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_ttNum_pt, null, "R_ttNum", "VK", ZXC.Q2un);
      vvtbR_ttNum_pt.JAM_ReadOnly = true;

      vvtbR_konto_pt = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_konto_pt", null, -12, "Konto");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_konto_pt, null, "R_konto", "Konto", ZXC.Q3un);
      vvtbR_konto_pt.JAM_ReadOnly = true;

      vvtbR_opis_pt = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_opis_pt", null, -12, "Opis");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_opis_pt, null, "R_opis", "Opis", 0);
      vvtbR_opis_pt.JAM_ReadOnly = true;
      colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      colVvText.MinimumWidth = ZXC.Q6un;

      vvtbR_kupdob_name_pt = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_kupdob_name_pt", null, -12, "Naziv partnera");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_kupdob_name_pt, null, "R_kupdob_name", "Partner", ZXC.Q8un);
      vvtbR_kupdob_name_pt.JAM_ReadOnly = true;

      vvtbR_ticker_pt = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_ticker_pt", null, -12, "Tiker partnera");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_ticker_pt, null, "R_ticker", "Ticker", ZXC.Q3un);
      vvtbR_ticker_pt.JAM_ReadOnly = true;

      vvtbR_kupdob_cd_pt = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate(true, "vvtb4ColR_kupdob_cd_pt", null, -6, "Šifra partnera");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_kupdob_cd_pt, null, "R_kupdob_cd", "SifK/D", ZXC.Q3un);
      vvtbR_kupdob_cd_pt.JAM_ReadOnly = true;


      vvtbR_tipBr_pt = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_tipBr_pt", null, -12, "TT Br");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_tipBr_pt, null, "R_tipBr", "TT Br", ZXC.Q4un);
      vvtbR_tipBr_pt.JAM_ReadOnly = true;

      vvtbR_dug_pt = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_dug_pt", null, -12, "Iznos duguje");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_dug_pt, null, "R_dug", "Duguje", ZXC.Q5un);
      vvtbR_dug_pt.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = ZXC.Q4un;
      vvtbR_dug_pt.JAM_ReadOnly = true;

      vvtbR_pot_pt = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_pot_pt", null, -12, "Iznos potražuje");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_pot_pt, null, "R_pot", "Potražuje", ZXC.Q5un);
      vvtbR_pot_pt.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = ZXC.Q4un;
      vvtbR_pot_pt.JAM_ReadOnly = true;

      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);

   }
   private void CreateColumn_6(VvDataGridView theGrid)
   {
      vvtbR_konto_ni_S = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_konto_ni_S", null, -12, "Konto");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_konto_ni_S, null, "R_konto", "Konto", ZXC.Q3un);
      vvtbR_konto_ni.JAM_ReadOnly = true;

      vvtbR_naziv_ni_S = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_naziv_ni_S", null, -12, "Naziv konta");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_naziv_ni_S, null, "R_opis", "Naziv konta", 0);
      vvtbR_naziv_ni_S.JAM_ReadOnly = true;
      colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      colVvText.MinimumWidth = ZXC.Q6un;

      vvtbR_dug_ni_S = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_dug_ni_S", null, -12, "Iznos duguje");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_dug_ni_S, null, "R_dug", "Duguje", ZXC.Q5un);
      vvtbR_dug_ni_S.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = ZXC.Q4un;
      vvtbR_dug_ni_S.JAM_ReadOnly = true;

      vvtbR_pot_ni_S = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_pot_ni_S", null, -12, "Iznos potražuje");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_pot_ni_S, null, "R_pot", "Potražuje", ZXC.Q5un);
      vvtbR_pot_ni_S.JAM_ShouldSumGrid = true;
      colVvText.MinimumWidth = ZXC.Q4un;
      vvtbR_pot_ni_S.JAM_ReadOnly = true;

      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);
   }

   #endregion GridColumns

   #region SetColumnIndexes()

   private       OTP_colIdx ci1;
   public        OTP_colIdx DgvCI1 { get { return ci1; } }
   public struct OTP_colIdx
   {
      internal int iT_recID        ;
      internal int iT_projektCD    ;
      internal int iT_PUT_dug_year ;
      internal int iT_PUT_pot_year ;
      internal int iT_RD_razd      ;
      internal int iT_RD_year      ;
      internal int iT_NIraspored   ;
      internal int iT_NewTr_razd   ;
      internal int iT_NewTr_year   ;
      internal int iT_dovrsenost   ;
      internal int iT_Otp_year     ;
      internal int iT_Otp_razd     ;
      internal int iT_PlanCijena   ;
      internal int iT_PostoRaspored;
      internal int it_Investit_otp ;
   
      }
   private void SetOTPColumnIndexes()
   {
      ci1 = new OTP_colIdx();

      ci1.iT_recID         = TheGrid_1.IdxForColumn("T_recID"        );
      ci1.iT_projektCD     = TheGrid_1.IdxForColumn("R_projektCD"    );
      ci1.iT_PUT_dug_year  = TheGrid_1.IdxForColumn("R_PUT_dug_year" );
      ci1.iT_PUT_pot_year  = TheGrid_1.IdxForColumn("R_PUT_pot_year" );
      ci1.iT_RD_razd       = TheGrid_1.IdxForColumn("R_RD_razd"      );
      ci1.iT_RD_year       = TheGrid_1.IdxForColumn("R_RD_year"      );
      ci1.iT_NIraspored    = TheGrid_1.IdxForColumn("R_NIraspored"   );
      ci1.iT_NewTr_razd    = TheGrid_1.IdxForColumn("R_NewTr_razd"   );
      ci1.iT_NewTr_year    = TheGrid_1.IdxForColumn("R_NewTr_year"   );
      ci1.iT_dovrsenost    = TheGrid_1.IdxForColumn("R_dovrsenost"   );
      ci1.iT_Otp_year      = TheGrid_1.IdxForColumn("R_Otp_year"     );
      ci1.iT_Otp_razd      = TheGrid_1.IdxForColumn("R_Otp_razd"     );
      ci1.iT_PlanCijena    = TheGrid_1.IdxForColumn("R_PlanCijena"   );
      ci1.iT_PostoRaspored = TheGrid_1.IdxForColumn("R_PostoRaspored");
      ci1.it_Investit_otp  = TheGrid_1.IdxForColumn("R_investit_otp");

            

   }

   private       RnpFaktur_colIdx ci2;
   public        RnpFaktur_colIdx DgvCI2 { get { return ci2; } }
   public struct RnpFaktur_colIdx
   {
      internal int iT_recID        ;
      internal int iT_projektCD    ;
      internal int iT_narInv_fa    ;
      internal int iT_objektCd_fa  ;
      internal int iT_objektName_fa;
      internal int iT_refDok_fa    ;
      internal int iT_dateStart_fa ;
      internal int iT_dateRokIsp_fa;
      internal int iT_ugCijena_fa  ;
      internal int iT_dovrsenost_fa;
      internal int iT_status_fa    ;
   }
   private void SetRnpFakturListColumnIndexes()
   {
      ci2 = new RnpFaktur_colIdx();

      ci2.iT_recID         = TheGrid_2.IdxForColumn("T_recID");
      ci2.iT_projektCD     = TheGrid_2.IdxForColumn("R_projektCD");
      ci2.iT_narInv_fa     = TheGrid_2.IdxForColumn("R_narInv_fa");
      ci2.iT_objektCd_fa   = TheGrid_2.IdxForColumn("R_objektCd_fa");
      ci2.iT_objektName_fa = TheGrid_2.IdxForColumn("R_objektName_fa");
      ci2.iT_refDok_fa     = TheGrid_2.IdxForColumn("R_refDok_fa");
      ci2.iT_dateStart_fa  = TheGrid_2.IdxForColumn("R_dateStart_fa");
      ci2.iT_dateRokIsp_fa = TheGrid_2.IdxForColumn("R_dateRokIsp_fa");
      ci2.iT_ugCijena_fa   = TheGrid_2.IdxForColumn("R_ugCijena_fa");
      ci2.iT_dovrsenost_fa = TheGrid_2.IdxForColumn("R_dovrsenost_fa");
      ci2.iT_status_fa     = TheGrid_2.IdxForColumn("R_status_fa");

   }

   private       NrspIndir_colIdx ci3;
   public        NrspIndir_colIdx DgvCI3 { get { return ci3; } }
   public struct NrspIndir_colIdx
   {
      internal int iT_recID;
      internal int iT_dokDate;
      internal int iT_dokNum  ;
      internal int iT_ttNum   ;
      internal int iT_konto;
      internal int iT_opis;
      internal int iT_kupdob_cd;
      internal int iT_kupdob_name;
      internal int iT_ticker;
      internal int iT_tipBr;
      internal int iT_dug;
      internal int iT_pot;
      internal int iT_projektCD;
   }
   private void SetNrspIndirFtrListColumnIndexes()
   {
      ci3 = new NrspIndir_colIdx();

      ci3.iT_recID       = TheGrid_3.IdxForColumn("T_recID");
      ci3.iT_dokDate     = TheGrid_3.IdxForColumn("R_dokDate");
      ci3.iT_dokNum      = TheGrid_3.IdxForColumn("R_dokNum");
      ci3.iT_ttNum       = TheGrid_3.IdxForColumn("R_ttNum");
      ci3.iT_konto       = TheGrid_3.IdxForColumn("R_konto");
      ci3.iT_opis        = TheGrid_3.IdxForColumn("R_opis");
      ci3.iT_kupdob_cd   = TheGrid_3.IdxForColumn("R_kupdob_cd");
      ci3.iT_kupdob_name = TheGrid_3.IdxForColumn("R_kupdob_name");
      ci3.iT_ticker      = TheGrid_3.IdxForColumn("R_ticker");
      ci3.iT_tipBr       = TheGrid_3.IdxForColumn("R_tipBr");
      ci3.iT_dug         = TheGrid_3.IdxForColumn("R_dug");
      ci3.iT_pot         = TheGrid_3.IdxForColumn("R_pot");
      ci3.iT_projektCD   = TheGrid_3.IdxForColumn("R_projektCD");
   }

   private       RaspDirkt_colIdx ci4;
   public        RaspDirkt_colIdx DgvCI4 { get { return ci4; } }
   public struct RaspDirkt_colIdx
   {
      internal int iT_recID;
      internal int iT_dokDate;
      internal int iT_dokNum  ;
      internal int iT_ttNum   ;
      internal int iT_konto;
      internal int iT_opis;
      internal int iT_kupdob_cd;
      internal int iT_kupdob_name;
      internal int iT_ticker;
      internal int iT_tipBr;
      internal int iT_dug;
      internal int iT_pot;
      internal int iT_projektCD;
   }
   private void SetRaspDirktFtrListColumnIndexes()
   {
      ci4 = new RaspDirkt_colIdx();

      ci4.iT_recID       = TheGrid_4.IdxForColumn("T_recID");
      ci4.iT_dokDate     = TheGrid_4.IdxForColumn("R_dokDate");
      ci4.iT_dokNum      = TheGrid_4.IdxForColumn("R_dokNum");
      ci4.iT_ttNum       = TheGrid_4.IdxForColumn("R_ttNum");
      ci4.iT_konto       = TheGrid_4.IdxForColumn("R_konto");
      ci4.iT_opis        = TheGrid_4.IdxForColumn("R_opis");
      ci4.iT_kupdob_cd   = TheGrid_4.IdxForColumn("R_kupdob_cd");
      ci4.iT_kupdob_name = TheGrid_4.IdxForColumn("R_kupdob_name");
      ci4.iT_ticker      = TheGrid_4.IdxForColumn("R_ticker");
      ci4.iT_tipBr       = TheGrid_4.IdxForColumn("R_tipBr");
      ci4.iT_dug         = TheGrid_4.IdxForColumn("R_dug");
      ci4.iT_pot         = TheGrid_4.IdxForColumn("R_pot");
      ci4.iT_projektCD   = TheGrid_4.IdxForColumn("R_projektCD");
   }

   private       ProUtijek_colIdx ci5;
   public        ProUtijek_colIdx DgvCI5 { get { return ci5; } }
   public struct ProUtijek_colIdx
   {
      internal int iT_recID;
      internal int iT_dokDate;
      internal int iT_dokNum  ;
      internal int iT_ttNum   ;
      internal int iT_konto;
      internal int iT_opis;
      internal int iT_kupdob_cd;
      internal int iT_kupdob_name;
      internal int iT_ticker;
      internal int iT_tipBr;
      internal int iT_dug;
      internal int iT_pot;
      internal int iT_projektCD;
   }
   private void SetProUtijekFtrListColumnIndexes()
   {
      ci5 = new ProUtijek_colIdx();

      ci5.iT_recID       = TheGrid_5.IdxForColumn("T_recID");
      ci5.iT_dokDate     = TheGrid_5.IdxForColumn("R_dokDate");
      ci5.iT_dokNum      = TheGrid_5.IdxForColumn("R_dokNum");
      ci5.iT_ttNum       = TheGrid_5.IdxForColumn("R_ttNum");
      ci5.iT_konto       = TheGrid_5.IdxForColumn("R_konto");
      ci5.iT_opis        = TheGrid_5.IdxForColumn("R_opis");
      ci5.iT_kupdob_cd   = TheGrid_5.IdxForColumn("R_kupdob_cd");
      ci5.iT_kupdob_name = TheGrid_5.IdxForColumn("R_kupdob_name");
      ci5.iT_ticker      = TheGrid_5.IdxForColumn("R_ticker");
      ci5.iT_tipBr       = TheGrid_5.IdxForColumn("R_tipBr");
      ci5.iT_dug         = TheGrid_5.IdxForColumn("R_dug");
      ci5.iT_pot         = TheGrid_5.IdxForColumn("R_pot");
      ci5.iT_projektCD   = TheGrid_5.IdxForColumn("R_projektCD");
   }

   private       NrspIndirS_colIdx ci6;
   public        NrspIndirS_colIdx DgvCI6 { get { return ci6; } }
   public struct NrspIndirS_colIdx
   {
      internal int iT_recID;
      internal int iT_konto;
      internal int iT_opis;
      internal int iT_dug;
      internal int iT_pot;
   }
   private void SetNrspIndirSFtrListColumnIndexes()
   {
      ci6 = new NrspIndirS_colIdx();

      ci6.iT_recID       = TheGrid_6.IdxForColumn("T_recID");
      ci6.iT_konto       = TheGrid_6.IdxForColumn("R_konto");
      ci6.iT_opis        = TheGrid_6.IdxForColumn("R_opis");
      ci6.iT_dug         = TheGrid_6.IdxForColumn("R_dug");
      ci6.iT_pot         = TheGrid_6.IdxForColumn("R_pot");
   }


   #endregion SetNalogColumnIndexes()

}


public partial class AnalizaProizDLG : Form// VvDialog{
{
   #region Fieldz

   private ToolStripPanel    tsPanel;
   private ToolStrip         ts_izvod;
   private ToolStripButton   tsb_ucitaj, tsb_abort, tsb_printPrw;
   private XSqlConnection    theDbConnection;
      
   #endregion Fieldz

   #region Propertiez

   public AnalizaProizUC TheUC { get; private set; }

 //private List<OTPstruct> otpList;

   private List<Ftrans> raspIndirFtrList;
   private List<Ftrans> raspDirktFtrList;
   private List<Ftrans> nrspRezskFtrList;
   private List<Ftrans> proUtijekFtrList;
   private List<Ftrans> nrspAmortFtrList;
   private List<Faktur> rnpFakturList;

   //private bool IsRNP_Analiza { get; set; }

   #endregion Propertiez

   #region Constructor

   public AnalizaProizDLG(MySqlConnection TheDbConnection, object sender)
   {
      ZXC.CurrentForm = this;

      this.theDbConnection = TheDbConnection;

      // 14.05.2015: 
      //IsRNP_Analiza = sender.ToString() == FakturDUC.RNP_Analiza;

      TheUC = new AnalizaProizUC(/*IsRNP_Analiza*/);

      SuspendLayout();

      this.Font        = ZXC.vvFont.BaseFont;
      this.BackColor   = ZXC.vvColors.userControl_BackColor;

      this.StartPosition = FormStartPosition.Manual;
      this.Text = "Integralna Analiza Troškova Proizvodnje";

      CreateToolStripIzvod();
      CreateTheUC();

      this.WindowState = FormWindowState.Maximized;
      this.MinimumSize = new Size(TheUC.hamp_kta.Right + ZXC.Q3un, TheUC.Bottom + ZXC.Q6un);

      CalcSizeTheUC();

      EnableDisable_tsBtn_Nalog(false);

      this.Load += new EventHandler(TrosakProizvodnjeDLG_Load);

      this.FormClosing +=new FormClosingEventHandler(RestoreZxcCurrentForm_FormClosing);
      this.Resize += new EventHandler(AnalizaProizDLG_Resize);
      ResumeLayout();

      VvHamper.AttachEscPressForEachControl(this);

   }

   void AnalizaProizDLG_Resize(object sender, EventArgs e)
   {
      CalcSizeTheUC();
   }

   private void CalcSizeTheUC()
   {
      TheUC.Size   = new Size(this.Width, this.Height - ts_izvod.Height);
      TheUC.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
      
      TheUC.CalcLocationSizeAnchor_ThePolyGridTabControl();
   }

   #endregion Constructor

   #region ToolStrip+Button

   private void CreateToolStripIzvod()
   {
      tsPanel           = new ToolStripPanel();
      tsPanel.Parent    = this;
      tsPanel.Dock      = DockStyle.Top;
      tsPanel.Name      = "tsPanel_Izvod";
      tsPanel.BackColor = ZXC.vvColors.tsPanel_BackColor;

      ts_izvod                  = new ToolStrip();
      ts_izvod.Parent           = tsPanel;
      ts_izvod.Name             = "ts_izvod";
      ts_izvod.ShowItemToolTips = true;
      ts_izvod.GripStyle        = ToolStripGripStyle.Hidden;
      ts_izvod.BackColor        = ZXC.vvColors.tsPanel_BackColor;

      MenuStrip menu = new MenuStrip();
      menu.Parent    = this;
      menu.Visible   = false;

      tsb_ucitaj = new ToolStripButton("Učitaj", VvIco.UcitajRn32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.ucitajRacune.ico")), 32, 32)*/.ToBitmap(), new EventHandler(UcitajButton_Click), "tsb_ucitaj");
      tsb_ucitaj.ToolTipText = "Učitaj podatke";
      ts_izvod.Items.Add(tsb_ucitaj);

      tsb_printPrw = new ToolStripButton("QPrint", VvIco.PrintPrw32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.printPrw.ico")), 32, 32)*/.ToBitmap(), new EventHandler(QPrint_Click), "tsb_qPrint");
      tsb_printPrw.ToolTipText = "Print tablice na ekranu";
      ts_izvod.Items.Add(tsb_printPrw);

      tsb_abort             = new ToolStripButton("Prekid", VvIco.Esc32/*new Icon(new Icon(ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.esc.ico")), 32, 32)*/.ToBitmap(), new EventHandler(AbortButton_Click), "tsb_abort");
      tsb_abort.ToolTipText = "Odustani od obračuna";
      ts_izvod.Items.Add(tsb_abort);

      foreach(ToolStripButton tsb in ts_izvod.Items)
      {
         tsb.ImageScaling      = ToolStripItemImageScaling.None;
         tsb.DisplayStyle      = ToolStripItemDisplayStyle.ImageAndText;
         tsb.TextImageRelation = TextImageRelation.ImageAboveText;
         tsb.Font              = ZXC.vvFont.ToolStripBtnFont;
      }
   }

   #endregion ToolStrip+Button
   
   #region TheUC

   private void CreateTheUC()
   {
      TheUC.Parent   = this;
      TheUC.Location = new Point(0, ts_izvod.Bottom);
   }

   #endregion TheUC

   #region FormClosing + Load

   void TrosakProizvodnjeDLG_Load(object sender, EventArgs e)
   {
      ZXC.CurrentForm = this;
   }

   private void ThePreviewIzvodForm_FormClosing_EnabledPreviewButton(object sender, FormClosingEventArgs e)
   {
     // tsb_previewIzv.Enabled = true;

      // ovo se mora napraviti jer je samo jedan ts_Report 
      ZXC.TheVvForm.ts_Report.Parent = ZXC.TheVvForm.tsPanel_SubModul;
      ZXC.TheVvForm.ts_Report.Visible = false;
   }

   void RestoreZxcCurrentForm_FormClosing(object sender, FormClosingEventArgs e)
   {
      ZXC.CurrentForm = ZXC.TheVvForm;
   }

   #endregion FormClosing + Load

   #region Methods

   public void EnableDisable_tsBtn_Nalog(bool isListeLoaded)
   {
      //VvHamper.Enable_Disable_Fields_ForWriting(TheUC.panel_DataGrid , isIzvAccept);

      //tsb_ucitaj.Enabled = !isListeLoaded;
      tsb_printPrw.Enabled = isListeLoaded;
      //tsb_abort .Enabled = isListeLoaded;

   }

   #endregion Methods

   private void QPrint_Click(object sender, EventArgs e)
   {
      SuspendLayout();

      #region Set Form

      Form ThePreviewForm = new Form();
      ThePreviewForm.Show();
      

      ThePreviewForm.Font      = ZXC.vvFont.BaseFont;
      ThePreviewForm.BackColor = ZXC.vvColors.userControl_BackColor;

      ThePreviewForm.FormClosing += new FormClosingEventHandler(ThePreviewIzvodForm_FormClosing_EnabledPreviewButton);

      ThePreviewForm.Location = Point.Empty;
      ThePreviewForm.Size     = new Size(SystemInformation.WorkingArea.Width - ZXC.Q10un * 3, SystemInformation.WorkingArea.Height);
      this.Location           = new Point(ThePreviewForm.Right, 0);

      #endregion Set Form

      ResumeLayout();

      #region FillDataset, CreateReportDocument, SetDataSource, AssignReportSource

      SetReportAndReportName_ForThisInnerTabPage(TheUC.ThePolyGridTabControl.SelectedTab.Title, ThePreviewForm);

      #endregion FillDataset, CreateReportDocument, SetDataSource, AssignReportSource

      ZXC.TheVvForm.ToolStripReportVisible_tsBtnOnTsReportVisible(false, ZXC.ReportMode.Done, false);

   }
   
   private void SetReportAndReportName_ForThisInnerTabPage(string selectedTabName, Control parent)
   {
      VvStandAloneReportViewerUC thePreviewUC = new VvStandAloneReportViewerUC();
      thePreviewUC.Parent = parent;
      thePreviewUC.Dock   = DockStyle.Fill;

      CR_OTP_dgv36   otp_dgv36Kto = new CR_OTP_dgv36  ();
      CR_OTP_dgv45   otp_dgv45PCd = new CR_OTP_dgv45  ();
      CR_OTP_RnList  otp_RnList   = new CR_OTP_RnList ();
      CR_IATP        otp_Obracun  = new CR_IATP();

      List<Kplan>  kplanList  = VvUserControl.KplanSifrar.Join(nrspRezskFtrList, kpln => kpln.Konto, ftr => ftr.T_konto, (kpln, ftr) => kpln).Distinct().ToList();

      if     (selectedTabName == TheUC.TabPageTitle1) { thePreviewUC.SetNonDatasetDataSource_And_AssignReportSource(otp_Obracun , TheOtpList           , typeof(OTPdata).Name);                                SetReportNameAndDr(otp_Obracun , TheUC.TabPageTitle1); }
      else if(selectedTabName == TheUC.TabPageTitle2) { thePreviewUC.SetNonDatasetDataSource_And_AssignReportSource(otp_RnList  , this.rnpFakturList   , typeof(Faktur ).Name); SetReportNameAndDr(otp_RnList  , TheUC.TabPageTitle2); }
   }

   private void SetReportNameAndDr(CrystalDecisions.CrystalReports.Engine.ReportDocument vvReportDocument,  string rptName)
   {
      CrystalDecisions.CrystalReports.Engine.ReportObjects reportObj = vvReportDocument.ReportDefinition.ReportObjects;

      vvReportDocument.DataDefinition.FormulaFields["Fof_ReportTitle"].Text = "'" + rptName                                   + "'";
      vvReportDocument.DataDefinition.FormulaFields["Fof_NazivFirme" ].Text = "'" + ZXC.CURR_prjkt_rec.Naziv                  + "'";
      vvReportDocument.DataDefinition.FormulaFields["Fof_DatOd"      ].Text = "'" + TheUC.Fld_DatumOd.ToString("dd.MM.yyyy.") + "'";
      vvReportDocument.DataDefinition.FormulaFields["Fof_DatDo"      ].Text = "'" + TheUC.Fld_DatumDo.ToString("dd.MM.yyyy.") + "'";


      int report_Width, razmak;
      razmak = 112;

      report_Width = vvReportDocument.PrintOptions.PageContentWidth;

      reportObj["Tobj_Razdoblje"].Width = report_Width;
      reportObj["Tobj_Razdoblje"].Left = razmak;
      (reportObj["Tobj_Razdoblje"] as CrystalDecisions.CrystalReports.Engine.TextObject).ObjectFormat.HorizontalAlignment = CrystalDecisions.Shared.Alignment.HorizontalCenterAlign;

      //if(rptName == TheUC.TabPageTitle1) vvReportDocument.DataDefinition.FormulaFields["Fof_Koef"  ].Text = "'" + TheUC.Fld_KoefDir.ToString("N", ZXC.GetNumberFormatInfo(6)) + "'";

      vvReportDocument.DataDefinition.FormulaFields["Fof_ReportTitle"].Text = "'" + rptName + "'";

      // 03.03.2017: 
      theReportDocument = vvReportDocument;
   }



}

public partial class AnalizaProizUC : VvOtherUC
{
   #region Fieldz

   public  Panel    panel_hampers;
   public VvHamper  hamp_razdTT, hamp_anaGr, hamp_endKto, hamp_raspored, hamp_kta, hamp_readOnly, hamp_lbl, hamp_analizaRNP, hamp_rezervac;
   private int      dlgWidth, dlgHeight, nextX, nextY, razmak;

   private VvTextBox tbx_DatumOD       , tbx_DatumDO        ,
                     tbx_tipRN 	      , tbx_anaGrDirTr	   , 
                     tbx_ktoPreraspTrsk, tbx_ktoProizv		, tbx_kto7trosProizv,	
                     tbx_anaGrIndTr 	, tbx_iznosIndirTr	,
                     tbx_ktoGotProiz	, tbx_ktoIndirEnd    , 
                     tbx_dirTrsk       , /*tbx_koefDir      ,*/ tbx_status,
                     tbx_ktoAmKorjen   , tbx_anaGrRrTr      , tbx_postoRr, tbx_rezijaPosto, tbx_amPosto, 
                     tbx_daniAvansKonto, tbx_primAvansKonto , tbx_kupciKonto, tbx_dobavKonto,
                     tbx_rezervacija1  , tbx_rezervacija2;

   private VvDateTimePicker dtp_DatumOD, dtp_DatumDO;
   private RadioButton      rbt_trDirMat, rbt_trCijProiz, rbt_ktoIndirSame, rbt_ktoIndirEnd;
   private CheckBox         cbx_isSkladGotProizv, cbx_hocuAmort;

   public KtoShemaDsc KSD { get; set; }
   public VvInnerTabControl ThePolyGridTabControl { get; set; }

   public VvDataGridView TheGrid_1 { get; set; }
   public VvDataGridView TheGrid_2 { get; set; }

   public VvDataGridView TheSumGrid_1 { get; set; }
   public VvDataGridView TheSumGrid_2 { get; set; }


   private VvTextBox vvtbR_projektCD_otp,  vvtbR_projektCD_fa   , 
                     vvtbR_investit_otp ,  vvtbR_narInv_fa      , 
                     vvtbR_PlanCijena   ,  vvtbR_objektCd_fa    , 
                     vvtbR_PUT_dug_year ,  vvtbR_objektName_fa  , 
                     vvtbR_RD_razd      ,  vvtbR_refDok_fa      , 
                     vvtbR_NIraspored   ,                         
                     vvtbR_RRraspored   ,                         
                     vvtbR_AMraspored   ,  vvtbR_ugCijena_fa    , 
                                           vvtbR_dovrsenost_fa  , 
                     vvtbR_NewTr_year   ,  vvtbR_status_fa      , 
                     vvtbR_UkTrRN       ,                         
                     vvtbR_RazlikaPlan 
                     ;

   private VvTextBoxColumn colVvText;
   private DataGridViewTextBoxColumn colScrol;
   private VvDateTimePickerColumn colDate;

   public bool IsPutKtoShemaDscFields_InProgress { get; set; }

   //private bool IsRNP_Analiza { get; set; }

   #endregion Fieldz

   #region Constructor

   public AnalizaProizUC(/*bool _isRNP_Analiza*/)
   {
      //this.IsRNP_Analiza = _isRNP_Analiza;

      SuspendLayout();

      CreatePanelsForHampers();
      CreateHampers();
      CreateThePolyGridTabControl();

      LocationAndSize_PanelsForhampers_AndClientSize();
      CalcLocationSizeAnchor_ThePolyGridTabControl();

      if(ZXC.CURR_userName == ZXC.vvDB_systemSuperUserName || ZXC.CURR_userName == ZXC.vvDB_programSuperUserName || ZXC.CURR_user_rec.IsSuper)
      {
         VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvOtherUC);
      }
      else
      {
         VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
      }


    //KSD = new KtoShemaDsc(ZXC.dscLuiLst_KtoShema);
      KSD = ZXC.KSD;

      PutKtoShemaDscFields(KSD);

      IsPutKtoShemaDscFields_InProgress = true;
      Fld_DatumOd = DateTime.Now.PrevMonthFirstDay();
      Fld_DatumDo = DateTime.Now.PrevMonthLastDay();
      IsPutKtoShemaDscFields_InProgress = false;

      ResumeLayout();

      SetOTPColumnIndexes();
      SetRnpFakturListColumnIndexes();


   }

   #endregion Constructor

   #region PanelsForHampers

   private void CreatePanelsForHampers()
   {
      panel_hampers             = new Panel();
      panel_hampers.Parent      = this;
     // panel_hampers.BorderStyle = BorderStyle.FixedSingle;//BorderStyle.None;
   }
   
   private void LocationAndSize_PanelsForhampers_AndClientSize()
   {
      int razmak = ZXC.Qun8;

      dlgWidth = 7 * ZXC.Q10un + ZXC.QunMrgn;

      panel_hampers.Size     = new Size(dlgWidth, hamp_lbl.Bottom + nextY);
      panel_hampers.Location = new Point(0, 0);
      ThePolyGridTabControl.Location = new Point(0, panel_hampers.Bottom);
      
      dlgHeight              = ThePolyGridTabControl.Bottom;
      this.Size              = new Size(dlgWidth, dlgHeight);
   }

   #endregion PanelsForHampers

   #region Hampers

   private void CreateHampers()
   {
      nextX = nextY = ZXC.Qun8;
      razmak = ZXC.QUN;

      InitializeHamperRazdTT(out hamp_razdTT);

      nextX = hamp_razdTT.Right + ZXC.Qun8; 
      InitializeHamperReadOnly(out hamp_readOnly);
      //hamp_readOnly.Visible = false;

      nextY = hamp_razdTT.Bottom + ZXC.Qun8;
      nextX = ZXC.Qun8;
      InitializeHamperAnalitGrupa(out hamp_anaGr);
      nextY = hamp_anaGr.Bottom;

      nextX = ZXC.Qun4;
      nextY = hamp_anaGr.Bottom + ZXC.Qun8;
      InitializeHamperRasporedTr(out hamp_raspored);
      hamp_raspored.Visible = false;

      nextX = hamp_raspored.Right;
      InitializeHamperEndKto(out hamp_endKto);
      hamp_endKto.Visible = false;

      nextY = ZXC.Qun8; 
      nextX = hamp_readOnly.Right;
      InitializeHamperKonta(out hamp_kta);

      nextY = hamp_kta.Bottom;
      nextX = ZXC.Qun4;
      
      InitializeHamperAnaliza(out hamp_analizaRNP);
      nextY = hamp_analizaRNP.Bottom;
      
      InitializeHamperRezervacije(out hamp_rezervac);
      nextY = hamp_rezervac.Bottom;

      InitializeHamperLbl(out hamp_lbl);
      nextY = ZXC.Qun8; 

   }


   private void InitializeHamperLbl(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", panel_hampers, false, nextX, nextY, 0);
      //                                     0           1  
      hamper.VvColWdt = new int[] { ZXC.Q10un + ZXC.Q5un, ZXC.Q10un + ZXC.Q5un, ZXC.Q10un + ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "PR - početak razdoblja (stanje na početku razdoblja obračuna)", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(1, 0, "UR - u razdoblju (promet troškova unutar razdoblja)", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 0, "KR - kraj razdoblja (stanje na kraju razdoblja obračuna)", ContentAlignment.MiddleRight); 

   }

   private void InitializeHamperReadOnly(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 4, "", panel_hampers, false, nextX, nextY, 0);
      //                                     0           1  
      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel   (0, 0, "Režijski troškovi 100%:", ContentAlignment.MiddleRight); 
      tbx_iznosIndirTr = hamper.CreateVvTextBox (1, 0, "tbx_iznosIndirTr"    , "", 12);
      tbx_iznosIndirTr.JAM_ReadOnly = true;
      tbx_iznosIndirTr.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_iznosIndirTr.JAM_Highlighted = true;

                        hamper.CreateVvLabel   (0, 1, "Režijski troškovi u %:", ContentAlignment.MiddleRight); 
      tbx_rezijaPosto = hamper.CreateVvTextBox (1, 1, "tbx_rezijaPosto"    , "", 12);
      tbx_rezijaPosto.JAM_ReadOnly = true;
      tbx_rezijaPosto.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_rezijaPosto.JAM_Highlighted = true;

                    hamper.CreateVvLabel   (0, 2, "Amortizacija:", ContentAlignment.MiddleRight); 
      tbx_dirTrsk = hamper.CreateVvTextBox (1, 2, "tbx_direktniTrsk"  , "", 12);
      tbx_dirTrsk.JAM_ReadOnly = true;
      tbx_dirTrsk.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_dirTrsk.JAM_Highlighted = true;


                   // hamper.CreateVvLabel   (0, 3, "Amortizacija:", ContentAlignment.MiddleRight); 
      tbx_amPosto = hamper.CreateVvTextBox (1, 3, "tbx_amPosto"  , "", 12);
      tbx_amPosto.JAM_ReadOnly = true;
      tbx_amPosto.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_amPosto.JAM_Highlighted = true;
      tbx_amPosto.Visible = false;
   }

   private void InitializeHamperRazdTT(out VvHamper hamper)
   {
      hamper = new VvHamper(7, 1, "", panel_hampers, false, nextX, nextY, 0);
      //                                     0           1        2        3        4              5                   6     
      hamper.VvColWdt      = new int[] { ZXC.Q7un, ZXC.Q4un, ZXC.Q4un, ZXC.Q5un, ZXC.Q2un, ZXC.Q2un+ZXC.Qun4, ZXC.Q2un -ZXC.Qun2};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun2, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel         (0, 0, "Razdoblje obračuna:", ContentAlignment.MiddleRight);
      tbx_DatumOD      = hamper.CreateVvTextBox       (1, 0, "tbx_datumOd", "Od datuma");
      tbx_DatumOD.JAM_IsForDateTimePicker = true;
      dtp_DatumOD      = hamper.CreateVvDateTimePicker(1, 0, "", tbx_DatumOD);
      dtp_DatumOD.Name = "dtp_DatumOD";
      dtp_DatumOD.Tag  = tbx_DatumOD;
      tbx_DatumOD.Tag  = dtp_DatumOD;

      tbx_DatumDO      = hamper.CreateVvTextBox       (2, 0, "tbx_datumDo", "");
      tbx_DatumDO.JAM_IsForDateTimePicker = true;
      dtp_DatumDO      = hamper.CreateVvDateTimePicker(2, 0, "", tbx_DatumDO);
      dtp_DatumDO.Name = "dtp_DatumDO";
      dtp_DatumDO.Tag  = tbx_DatumDO;
      tbx_DatumDO.Tag  = dtp_DatumDO;
      
      tbx_DatumOD.ContextMenu = dtp_DatumOD.ContextMenu =
                          tbx_DatumDO.ContextMenu =
                          dtp_DatumDO.ContextMenu = CreateNewContexMenu_Date();

                  hamper.CreateVvLabel  (3, 0 , "TT Rad.Nal/Proj:", ContentAlignment.MiddleRight)      ; 
      tbx_tipRN = hamper.CreateVvTextBox(4, 0, "tbx_tipRN"           , "");

                   hamper.CreateVvLabel        (5, 0, "Preskoči:", ContentAlignment.MiddleRight);
      tbx_status = hamper.CreateVvTextBoxLookUp(6, 0, "tbx_Status", "Preskoči naloge sa odobranim statusom", 1);

      tbx_status.JAM_Set_LookUpTable(ZXC.luiListaRiskStatus, (int)ZXC.Kolona.prva);
      tbx_status.JAM_lookUp_NOTobligatory = false;
      tbx_status.JAM_CharacterCasing = CharacterCasing.Upper;

      dtp_DatumOD.ValueChanged += new EventHandler(DisablePrintPrwAction);
      dtp_DatumDO.ValueChanged += new EventHandler(DisablePrintPrwAction);
      tbx_tipRN  .TextChanged  += new EventHandler(DisablePrintPrwAction);
      tbx_status .TextChanged  += new EventHandler(DisablePrintPrwAction);
   }

   private void InitializeHamperAnalitGrupa(out VvHamper hamper)
   {
      hamper = new VvHamper(8, 2, "", panel_hampers, false, nextX, nextY, 0);
      //                                    0             1      2         3             4               5                6            7     
      hamper.VvColWdt      = new int[] { ZXC.Q7un, ZXC.Q3un, ZXC.Q2un, ZXC.QUN , ZXC.Q5un +ZXC.Qun4,  ZXC.QUN, ZXC.Q5un +ZXC.Qun2,  ZXC.QUN};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,           ZXC.Qun4, ZXC.Qun4,           ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel  (0, 0, "Analitička grupa konta:", 1, 0, ContentAlignment.MiddleRight); 
                       hamper.CreateVvLabel  (2, 0, "Direktnih troškova:", ContentAlignment.MiddleRight);
      tbx_anaGrDirTr = hamper.CreateVvTextBox(3, 0, "tbx_anaGrDirTr", "");
                       hamper.CreateVvLabel  (4, 0, "Indirektnih troškova:", ContentAlignment.MiddleRight);
      tbx_anaGrIndTr = hamper.CreateVvTextBox(5, 0, "tbx_anaGrIndTr", "");
                       hamper.CreateVvLabel  (6, 0, "Režijskih troškova:", ContentAlignment.MiddleRight);
      tbx_anaGrRrTr  = hamper.CreateVvTextBox(7, 0, "tbx_anaGrDirTr", "");
                       hamper.CreateVvLabel  (0, 1, "% Priznatih Režijskih troškova:", 1, 0, ContentAlignment.MiddleRight);
      tbx_postoRr    = hamper.CreateVvTextBox(2, 1, "tbx_postoRr", "", 6, 1, 0);

      tbx_anaGrDirTr.TextChanged += new EventHandler(DisablePrintPrwAction);
      tbx_anaGrIndTr.TextChanged += new EventHandler(DisablePrintPrwAction);
      tbx_anaGrRrTr .TextChanged += new EventHandler(DisablePrintPrwAction);
      tbx_postoRr.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

    }

   private void InitializeHamperRasporedTr  (out VvHamper hamper)
   {
      hamper = new VvHamper(2, 2, "", panel_hampers, false, nextX, nextY, 0);
      
      hamper.VvColWdt      = new int[] { ZXC.Q7un, ZXC.Q6un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel       (0, 0, "Raspored troškova koef:", ContentAlignment.MiddleRight);
      rbt_trDirMat   = hamper.CreateVvRadioButton (1, 0, new EventHandler(DisablePrintPrwAction), "Direktni troškovi", TextImageRelation.ImageBeforeText);
      rbt_trCijProiz = hamper.CreateVvRadioButton (1, 1, new EventHandler(DisablePrintPrwAction), "Planirana cijena" , TextImageRelation.ImageBeforeText);
      rbt_trDirMat.Checked = true;
      rbt_trDirMat  .Tag = 0; //vvtbR_RD_year;
      rbt_trCijProiz.Tag = 1; //vvtbR_PlanCijena;
   }

   private void InitializeHamperEndKto(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 2, "", panel_hampers, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q7un,ZXC.Q5un + ZXC.Qun2 + ZXC.Qun8, ZXC.QUN  };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,ZXC.Qun4, ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel      (0, 0,       "Knjiži na:"  , ContentAlignment.MiddleRight);
      rbt_ktoIndirSame = hamper.CreateVvRadioButton(1, 0, null, "Isti konto"  , TextImageRelation.ImageBeforeText);
      rbt_ktoIndirEnd  = hamper.CreateVvRadioButton(1, 1, null, "Zadnja znamenka:", TextImageRelation.ImageBeforeText);
      rbt_ktoIndirEnd.Checked = true;
      
      tbx_ktoIndirEnd  = hamper.CreateVvTextBox    (2, 1, "tbx_ktoIndirEnd", "", 1);
      tbx_ktoIndirEnd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;


   }

   private void InitializeHamperKonta(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 4, "", panel_hampers, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q2un, ZXC.Q5un, ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, 0, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbDir        = hamper.CreateVvLabel  (0,  0, "KONTA:", ContentAlignment.MiddleRight);
      //                     hamper.CreateVvLabel  (0,  0, "Raspored troškova:"             , 2, 0, ContentAlignment.MiddleRight); 
      tbx_ktoPreraspTrsk = hamper.CreateVvTextBox(3,  0, "tbx_ktoPreraspTrsk",      "Raspored troškova za obračun proizvoda i usluga (prema HSFI 10 i MRS-u 2 i MRS-u 11) - uskladištivi troškovi (na račune 60, 62 i 63)", 8);
                           hamper.CreateVvLabel  (0,  0, "Amortizacija korijen konta:", 2, 0, ContentAlignment.MiddleRight); 
      tbx_ktoAmKorjen    = hamper.CreateVvTextBox(3,  0, "tbx_ktoAmKorjen",  "Korijen konta troškova amortizacije", 8);

      
      
                           hamper.CreateVvLabel  (0,  1, "Proizvodnja u  tijeku:"         , 2, 0, ContentAlignment.MiddleRight); 
      tbx_ktoProizv      = hamper.CreateVvTextBox(3,  1, "tbx_ktoProizv",           "Proizvodnja u tijeku", 8);
      //                     hamper.CreateVvLabel  (0,  2, "Trošak zaliha prodanih proizvoda:"   , 2, 0, ContentAlignment.MiddleRight); 
      tbx_kto7trosProizv = hamper.CreateVvTextBox(3,  2, "tbx_kto7trosProizv"  ,    "Trošak zaliha prodanih proizvoda (60, 62, 63 i 64)", 8);
      //                     hamper.CreateVvLabel  (1,  3, "Gotovi proizvodi na skladištu:"   , 1, 0, ContentAlignment.MiddleRight); 
      tbx_ktoGotProiz    = hamper.CreateVvTextBox(3,  3, "tbx_ktoGotProiz"     ,    "Gotovi proizvodi na skladištu", 8);

      cbx_isSkladGotProizv = hamper.CreateVvCheckBox_OLD(0, 3, null, "", RightToLeft.Yes);
      cbx_hocuAmort        = hamper.CreateVvCheckBox_OLD(0, 3, null, 2, 0, "Prikazi i amortizaciju",  RightToLeft.Yes);

      tbx_ktoPreraspTrsk.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_ktoProizv     .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_kto7trosProizv.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_ktoGotProiz   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_ktoAmKorjen   .JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_ktoPreraspTrsk.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_ktoProizv     .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_kto7trosProizv.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_ktoGotProiz   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      tbx_ktoAmKorjen   .JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);

      tbx_ktoProizv.TextChanged += new EventHandler(DisablePrintPrwAction);

      tbx_ktoPreraspTrsk.Visible =
      tbx_kto7trosProizv.Visible =
      tbx_ktoGotProiz.Visible =
      cbx_isSkladGotProizv.Visible = false;
   }

   public void DisablePrintPrwAction(object sender, EventArgs e)
   {
   }


   private void InitializeHamperAnaliza(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", panel_hampers, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q10un, ZXC.Q10un, ZXC.Q10un, ZXC.Q10un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                            hamper.CreateVvLabel  (0, 0, "Korijeni konta KUPCI:"               , ContentAlignment.MiddleLeft);
                            hamper.CreateVvLabel  (1, 0, "Korijeni konta PRIMLJENI PREDUJMOVI:", ContentAlignment.MiddleLeft);
                            hamper.CreateVvLabel  (2, 0, "Korijeni konta DOBAVLJAČI:"          , ContentAlignment.MiddleLeft);
                            hamper.CreateVvLabel  (3, 0, "Korijeni konta DANI PREDUJMOVI:"     , ContentAlignment.MiddleLeft);

      tbx_kupciKonto      = hamper.CreateVvTextBox(0, 1, "tbx_kupciKonto"    , "");
      tbx_primAvansKonto  = hamper.CreateVvTextBox(1, 1, "tbx_primAvansKonto", "");
      tbx_dobavKonto      = hamper.CreateVvTextBox(2, 1, "tbx_dobavKonto"    , "");
      tbx_daniAvansKonto  = hamper.CreateVvTextBox(3, 1, "tbx_daniAvansKonto", "");
   }

   private void InitializeHamperRezervacije(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", panel_hampers, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q5un - ZXC.Qun4, ZXC.Q5un, ZXC.Q5un - ZXC.Qun4, ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] {            ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel  (0, 0, "Rezervacija 1:", ContentAlignment.MiddleRight);
                         hamper.CreateVvLabel  (2, 0, "Rezervacija 2:", ContentAlignment.MiddleRight);

      tbx_rezervacija1 = hamper.CreateVvTextBox(1, 0, "tbx_rezervacija1", "", 12);
      tbx_rezervacija2 = hamper.CreateVvTextBox(3, 0, "tbx_rezervacija2", "", 12);

      tbx_rezervacija1.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_rezervacija2.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

   }


   #endregion Hampers

   #region Datumi_ContexMenu

   private VvStandardTextBoxContextMenu CreateNewContexMenu_Date()
   {
      VvStandardTextBoxContextMenu date_ContexMenu = new VvStandardTextBoxContextMenu(new MenuItem[] 
            { 
               new MenuItem("Danas"           , IspuniDatume),
               new MenuItem("Tekuća godina"   , IspuniDatume),
               new MenuItem("Tekući mjesec"   , IspuniDatume),
               new MenuItem("Prvi kvartal"    , IspuniDatume),
               new MenuItem("Drugi kvartal"   , IspuniDatume),
               new MenuItem("Treći kvartal"   , IspuniDatume),
               new MenuItem("Četvrti kvartal" , IspuniDatume),
               new MenuItem("1 -11 mjesec"    , IspuniDatume),
               new MenuItem("1 -10 mjesec"    , IspuniDatume),
               new MenuItem("1 - 9 mjesec"    , IspuniDatume),
               new MenuItem("1 - 8 mjesec"    , IspuniDatume),
               new MenuItem("1 - 7 mjesec"    , IspuniDatume),
               new MenuItem("1 - 6 mjesec"    , IspuniDatume),
               new MenuItem("1 - 5 mjesec"    , IspuniDatume),
               new MenuItem("1 - 4 mjesec"    , IspuniDatume),
               new MenuItem("1 - 3 mjesec"    , IspuniDatume),
               new MenuItem("1 - 2 mjesec"    , IspuniDatume),
               new MenuItem("Siječanj"        , IspuniDatume),
               new MenuItem("Veljača"         , IspuniDatume),
               new MenuItem("Ožujak"          , IspuniDatume),
               new MenuItem("Travanj"         , IspuniDatume),
               new MenuItem("Svibanj"         , IspuniDatume),
               new MenuItem("Lipanj"          , IspuniDatume),
               new MenuItem("Srpanj"          , IspuniDatume),
               new MenuItem("Kolovoz"         , IspuniDatume),
               new MenuItem("Rujan"           , IspuniDatume),
               new MenuItem("Listopad"        , IspuniDatume),
               new MenuItem("Studeni"         , IspuniDatume),
               new MenuItem("Prosinac"        , IspuniDatume),
               new MenuItem("-")
            });

      return date_ContexMenu;
   }

   private void IspuniDatume(object sender, EventArgs e)
   {
      MenuItem tsmi = sender as MenuItem;

      string text = tsmi.Text;
      string textOd = "";
      string textDo = "";
      string mj02 = "28"; ;

      // ovo je dobro. ostavi ovako (TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.ProjectYear;) 
      string godina = ZXC.projectYear;
      int god = int.Parse(godina);

      if(DateTime.IsLeapYear(god)) mj02 = "29";
      else mj02 = "28";

      switch(text)
      { //                              mmOD           dd.mmDO
         case "Tekuća godina"  : textOd = "01"; textDo = "31.12"; break;
         case "Tekući mjesec"  : textOd = DateTime.Today.Month.ToString(); textDo = DateTime.DaysInMonth(god, DateTime.Today.Month).ToString() + "." + DateTime.Today.Month.ToString(); break;
         case "Prvi kvartal"   : textOd = "01"; textDo = "31.03"; break;
         case "Drugi kvartal"  : textOd = "04"; textDo = "30.06"; break;
         case "Treći kvartal"  : textOd = "07"; textDo = "30.09"; break;
         case "Četvrti kvartal": textOd = "10"; textDo = "31.12"; break;
         case "1 -11 mjesec"   : textOd = "01"; textDo = "30.11"; break;
         case "1 -10 mjesec"   : textOd = "01"; textDo = "31.10"; break;
         case "1 - 9 mjesec"   : textOd = "01"; textDo = "30.09"; break;
         case "1 - 8 mjesec"   : textOd = "01"; textDo = "31.08"; break;
         case "1 - 7 mjesec"   : textOd = "01"; textDo = "31.07"; break;
         case "1 - 6 mjesec"   : textOd = "01"; textDo = "30.06"; break;
         case "1 - 5 mjesec"   : textOd = "01"; textDo = "31.05"; break;
         case "1 - 4 mjesec"   : textOd = "01"; textDo = "30.04"; break;
         case "1 - 3 mjesec"   : textOd = "01"; textDo = "31.03"; break;
         case "1 - 2 mjesec"   : textOd = "01"; textDo = mj02 + ".02"; break;
         case "Siječanj"       : textOd = "01"; textDo = "31.01"; break;
         case "Veljača"        : textOd = "02"; textDo = mj02 + ".02"; break;
         case "Ožujak"         : textOd = "03"; textDo = "31.03"; break;
         case "Travanj"        : textOd = "04"; textDo = "30.04"; break;
         case "Svibanj"        : textOd = "05"; textDo = "31.05"; break;
         case "Lipanj"         : textOd = "06"; textDo = "30.06"; break;
         case "Srpanj"         : textOd = "07"; textDo = "31.07"; break;
         case "Kolovoz"        : textOd = "08"; textDo = "31.08"; break;
         case "Rujan"          : textOd = "09"; textDo = "30.09"; break;
         case "Listopad"       : textOd = "10"; textDo = "31.10"; break;
         case "Studeni"        : textOd = "11"; textDo = "30.11"; break;
         case "Prosinac"       : textOd = "12"; textDo = "31.12"; break;
         case "Danas"          : textOd = ""  ; textDo = ""     ; break;
      }

      if(text == "Danas")
      {
         tbx_DatumOD.Text = DateTime.Today.ToString("dd.MM.yyyy");
         tbx_DatumDO.Text = DateTime.Today.ToString("dd.MM.yyyy");
      }
      else
      {
         tbx_DatumOD.Text = "01." + textOd + "." + godina;
         tbx_DatumDO.Text = textDo + "." + godina;
      }
      
      dtp_DatumOD.Value = DateTime.Parse(tbx_DatumOD.Text);
      dtp_DatumDO.Value = DateTime.Parse(tbx_DatumDO.Text);

   }

   #endregion Datumi_ContexMenu

   #region Fld_

   public DateTime Fld_DatumOd
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumOD.Value);
      }
      set
      {
         dtp_DatumOD.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DatumOD.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumOD);
      }
   }

   public DateTime Fld_DatumDo
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumDO.Value);
      }
      set
      {
         dtp_DatumDO.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DatumDO.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumDO);
      }
   }

   public string  Fld_AnalizaProizTT   { get { return tbx_tipRN           .Text;              } set { tbx_tipRN           .Text =          value ; } }  
   public string  Fld_NiAnaGR          { get { return tbx_anaGrIndTr      .Text;              } set { tbx_anaGrIndTr      .Text =          value ; } }  
   public string  Fld_KtoObrade        { get { return tbx_ktoProizv       .Text;              } set { tbx_ktoProizv       .Text =          value ; } }  
   public string  Fld_RdAnaGR          { get { return tbx_anaGrDirTr      .Text;              } set { tbx_anaGrDirTr      .Text =          value ; } }  
  
   public string  Fld_KtoPreraspTrsk         { get { return tbx_ktoPreraspTrsk  .Text;              } set { tbx_ktoPreraspTrsk  .Text =          value ; } }  
   public string  Fld_Kto7trosProizv         { get { return tbx_kto7trosProizv  .Text;              } set { tbx_kto7trosProizv  .Text =          value ; } }  
  
   public decimal Fld_A_IznosREZIJSKIH_Tr    { get { return tbx_iznosIndirTr.GetDecimalField(); } set { tbx_iznosIndirTr .PutDecimalField(value); } }
   public decimal Fld_A_IznosAMORT_Tr        { get { return tbx_dirTrsk     .GetDecimalField(); } set { tbx_dirTrsk      .PutDecimalField(value); } } 
   public decimal Fld_A_IznPostoREZIJSKIH_Tr { get { return tbx_rezijaPosto.GetDecimalField();  } set { tbx_rezijaPosto  .PutDecimalField(value); } }
   public decimal Fld_A_IznPostoAMORT_Tr     { get { return tbx_amPosto    .GetDecimalField();  } set { tbx_amPosto      .PutDecimalField(value); } } 

   public bool    Fld_IsDirekt         
   {
      get
      {
         if     (rbt_trDirMat  .Checked) return  true;
         else if(rbt_trCijProiz.Checked) return  false;

         else throw new Exception("Fld_IsDirekt: who df is checked?");
      }
      set
      {
         if(value == true) rbt_trDirMat  .Checked = true;
         else              rbt_trCijProiz.Checked = true;
      }
   }
   public bool    Fld_IsKtoEndSame     
   {
      get
      {
         if     (rbt_ktoIndirSame.Checked) return  true;
         else if(rbt_ktoIndirEnd .Checked) return  false;

         else throw new Exception("Fld_IsKtoEndSame: who df is checked?");
      }
      set
      {
         if(value == true) rbt_ktoIndirSame.Checked = true;
         else              rbt_ktoIndirEnd .Checked = true;
      }
   }

   public decimal Fld_DirektniTr { get { return tbx_dirTrsk.GetDecimalField(); } set { tbx_dirTrsk.PutDecimalField(value); } }
   //public decimal Fld_KoefDir    { get { return tbx_koefDir.GetDecimalField(); } set { tbx_koefDir.PutDecimalField(value); } }
   public string  Fld_SkipSatus  { get { return tbx_status .Text             ; } set { tbx_status .Text =          value ; } }
   
   public decimal Fld_PostoRr     { get { return tbx_postoRr.GetDecimalField(); } set { tbx_postoRr.PutDecimalField(value); } }
   public string  Fld_KtoAmKorjen { get { return tbx_ktoAmKorjen.Text         ; } set { tbx_ktoAmKorjen.Text =      value ; } }
   public string  Fld_RrAnaGR     { get { return tbx_anaGrRrTr.Text;            } set { tbx_anaGrRrTr  .Text =      value ; } }
   public bool    Fld_HocuAmort   { get { return cbx_hocuAmort.Checked        ; } set { cbx_hocuAmort.Checked =     value ; } }

   public string  Fld_KupciKonto     { get { return tbx_kupciKonto    .Text; } set { tbx_kupciKonto    .Text = value; } }
   public string  Fld_PrimAvansKonto { get { return tbx_primAvansKonto.Text; } set { tbx_primAvansKonto.Text = value; } }
   public string  Fld_DobavKonto     { get { return tbx_dobavKonto    .Text; } set { tbx_dobavKonto    .Text = value; } }
   public string  Fld_DaniAvansKonto { get { return tbx_daniAvansKonto.Text; } set { tbx_daniAvansKonto.Text = value; } }
   public decimal Fld_Rezervacija1   { get { return tbx_rezervacija1.GetDecimalField(); } set { tbx_rezervacija1.PutDecimalField(value); } }
   public decimal Fld_Rezervacija2   { get { return tbx_rezervacija2.GetDecimalField(); } set { tbx_rezervacija2.PutDecimalField(value); } }
   
   #endregion Fld_

   #region PutShemaKontoFields(), GetShemaKontoFields()

   private void PutKtoShemaDscFields(KtoShemaDsc KSD)
   {
      IsPutKtoShemaDscFields_InProgress = true;

      Fld_AnalizaProizTT   = KSD.Dsc_otp_obrProTT      ;
      Fld_NiAnaGR          = KSD.Dsc_otp_niAnaGR       ;
      Fld_SkipSatus        = KSD.Dsc_otp_skipSatus     ; 
      Fld_KtoObrade        = KSD.Dsc_otp_ktoObrade     ;
      Fld_RdAnaGR          = KSD.Dsc_otp_rdAnaGR       ;
      Fld_KtoPreraspTrsk   = KSD.Dsc_otp_ktoPrerspTrsk ;
      Fld_Kto7trosProizv   = KSD.Dsc_otp_kto7trosProizv;
      Fld_IsDirekt         = KSD.Dsc_otp_IsDirekt      ;
      Fld_IsKtoEndSame     = KSD.Dsc_otp_IsKtoEndSame  ;

      Fld_PostoRr          = KSD.Dsc_atp_postoRr       ;
      Fld_KtoAmKorjen      = KSD.Dsc_atp_ktoAmKorjen   ;
      Fld_RrAnaGR          = KSD.Dsc_atp_rrAnaGR       ;
      Fld_HocuAmort        = KSD.Dsc_atp_hocuAmort     ;

      Fld_KupciKonto       = KSD.Dsc_KupacKontaIOS     ;   
      Fld_PrimAvansKonto   = KSD.Dsc_PrimAvansKonta    ;
      Fld_DobavKonto       = KSD.Dsc_DobavKontaIOS     ;   
      Fld_DaniAvansKonto   = KSD.Dsc_DaniAvansKonta    ;

      IsPutKtoShemaDscFields_InProgress = false;
   }

   public bool GetKtoShemaDscFields() // bool je za validaciju 
   {
      KSD.Dsc_otp_obrProTT       = Fld_AnalizaProizTT         ;
      KSD.Dsc_otp_niAnaGR        = Fld_NiAnaGR          ;
      KSD.Dsc_otp_skipSatus      = Fld_SkipSatus;
      KSD.Dsc_otp_ktoObrade      = Fld_KtoObrade        ;
      KSD.Dsc_otp_rdAnaGR        = Fld_RdAnaGR          ;
      KSD.Dsc_otp_ktoPrerspTrsk  = Fld_KtoPreraspTrsk   ;
      KSD.Dsc_otp_kto7trosProizv = Fld_Kto7trosProizv   ;
      KSD.Dsc_otp_IsDirekt       = Fld_IsDirekt         ;
      KSD.Dsc_otp_IsKtoEndSame   = Fld_IsKtoEndSame     ;

      KSD.Dsc_atp_postoRr        = Fld_PostoRr;
      KSD.Dsc_atp_ktoAmKorjen    = Fld_KtoAmKorjen;
      KSD.Dsc_atp_rrAnaGR        = Fld_RrAnaGR;
      KSD.Dsc_atp_hocuAmort      = Fld_HocuAmort;

      KSD.Dsc_KupacKontaIOS      = Fld_KupciKonto     ;   
      KSD.Dsc_PrimAvansKonta     = Fld_PrimAvansKonto ;
      KSD.Dsc_DobavKontaIOS      = Fld_DobavKonto     ;   
      KSD.Dsc_DaniAvansKonta     = Fld_DaniAvansKonto ;
      
      KSD.SaveDscToLookUpItemList();

      if(
         KSD.Dsc_otp_obrProTT      .IsEmpty() ||
         KSD.Dsc_otp_niAnaGR       .IsEmpty() ||
         KSD.Dsc_otp_ktoObrade     .IsEmpty() ||
         KSD.Dsc_otp_rdAnaGR       .IsEmpty() ||
         KSD.Dsc_otp_ktoPrerspTrsk .IsEmpty() ||
         KSD.Dsc_otp_kto7trosProizv.IsEmpty() ||
         KSD.Dsc_otp_ktoGotProiz   .IsEmpty() ||
         KSD.Dsc_otp_skipSatus     .IsEmpty() ||
         KSD.Dsc_atp_ktoAmKorjen   .IsEmpty() ||
         KSD.Dsc_atp_rrAnaGR       .IsEmpty() 
         ) 
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nepotpuni podaci.");
         return true;
      }

      return false;
   }

   #endregion PutShemaKontoFields(), GetShemaKontoFields()

   #region TabPageTitle

   public string TabPageTitle1 { get { return "°  Integralna Analiza Troškova Proizvodnje  °"; } }
   public string TabPageTitle2 { get { return "°  Projekti / Radni Nalozi  °"; } }

   #endregion TabPageTitle

   #region CreateThePolyGridTabControl

   private void CreateThePolyGridTabControl()
   {
      ThePolyGridTabControl          = new VvInnerTabControl();
      ThePolyGridTabControl.Parent   = this;
      ThePolyGridTabControl.Location = new Point(ZXC.Qun8, panel_hampers.Bottom + ZXC.Qun8);

      ThePolyGridTabControl.TabPages.Add(new VvInnerTabPage(TabPageTitle1, TabPageTitle1, ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
      ThePolyGridTabControl.TabPages.Add(new VvInnerTabPage(TabPageTitle2, TabPageTitle2, ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));

      ThePolyGridTabControl.TabPages[TabPageTitle1].BackColor = ZXC.vvColors.tabPage4TheG_BackColor;
      ThePolyGridTabControl.TabPages[TabPageTitle2].BackColor = Color.NavajoWhite;

      ThePolyGridTabControl.TabPages[TabPageTitle1].Tag = ZXC.vvColors.tabPage4TheG_BackColor;
      ThePolyGridTabControl.TabPages[TabPageTitle2].Tag = Color.NavajoWhite;

      TheGrid_1 = CreateTheGrid(ThePolyGridTabControl.TabPages[TabPageTitle1], TabPageTitle1);
      TheGrid_2 = CreateTheGrid(ThePolyGridTabControl.TabPages[TabPageTitle2], TabPageTitle2);
      TheGrid_1.ColumnHeadersHeight = ZXC.Q2un;

      CreateColumn_1(TheGrid_1);
      CreateColumn_2(TheGrid_2);

      TheSumGrid_1 = CreateSumGrid(TheGrid_1, TheGrid_1.Parent, "SUM" + TabPageTitle1);
      TheSumGrid_2 = CreateSumGrid(TheGrid_2, TheGrid_2.Parent, "SUM" + TabPageTitle2);

      InitializeTheSUMGrid_Columns(TheGrid_1);
      InitializeTheSUMGrid_Columns(TheGrid_2);

   }

   private void TheGrid_Scroll(object sender, ScrollEventArgs e)
   {
      VvDataGridView theGrid = sender as VvDataGridView;
      DataGridView theSumGrid = theGrid.TheLinkedGrid_Sum;

      if(theSumGrid != null) theSumGrid.HorizontalScrollingOffset = theGrid.HorizontalScrollingOffset;
   }
   public void CalcLocationSizeAnchor_ThePolyGridTabControl()
   {
      panel_hampers.Size = new Size(dlgWidth, hamp_lbl.Bottom);
      panel_hampers.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

      ThePolyGridTabControl.Size = new Size(this.Width - ZXC.Q2un, this.Height - panel_hampers.Height - ZXC.QUN);
      ThePolyGridTabControl.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

      CalcLocationSizeAnchor_TheDGVAndTheSumGrid(TheGrid_1);
      CalcLocationSizeAnchor_TheDGVAndTheSumGrid(TheGrid_2);
   }

   #endregion CreateThePolyGridTabControl

   #region TheG

   private VvDataGridView CreateTheGrid(Control parent, string name)
   {
      VvDataGridView theGrid      = new VvDataGridView();

      theGrid.Name                = name;
      theGrid.Parent              = parent;
      theGrid.Location            = new Point(ZXC.Qun2, ZXC.Qun2);
      theGrid.AutoGenerateColumns = false;

      theGrid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

      theGrid.RowHeadersBorderStyle = theGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
      theGrid.ColumnHeadersHeight   = ZXC.QUN;
      theGrid.RowTemplate.Height    = ZXC.QUN;
      theGrid.RowHeadersWidth       = ZXC.Q2un + ZXC.Qun4;

      theGrid.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(VvDocumentRecordUC.Grid_EditingControlShowing);
      theGrid.CellBeginEdit         += new DataGridViewCellCancelEventHandler           (VvDocumentRecordUC.grid_CellBeginEdit_AtachJAM_EventHandlers);
      theGrid.CellEndEdit           += new DataGridViewCellEventHandler                 (VvDocumentRecordUC.grid_CellEndEdit_DetachJAM_EventHandlers);

      theGrid.CellFormatting        += new DataGridViewCellFormattingEventHandler       (VvDocumentRecordUC.grid_CellFormatting_FormatVvDateTime);
      theGrid.Validating            += new System.ComponentModel.CancelEventHandler     (VvDocumentRecordUC.grid_Validating);

      VvHamper.ApplyVVColorAndStyleTabCntrolChange(theGrid);
      VvHamper.Open_Close_Fields_ForWriting(theGrid, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);

      theGrid.AllowUserToAddRows       =
      theGrid.AllowUserToDeleteRows    =
      theGrid.AllowUserToOrderColumns  =
      theGrid.AllowUserToResizeColumns =
      theGrid.AllowUserToResizeRows    = false;

      theGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.PowderBlue;
      theGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.DarkSlateGray;
      theGrid.RowHeadersDefaultCellStyle.BackColor    = Color.PowderBlue; //Color.FloralWhite;
      theGrid.RowHeadersDefaultCellStyle.ForeColor    = Color.DarkSlateGray;
      
      return theGrid;
   }

   private VvDataGridView CreateSumGrid(VvDataGridView theGrid, Control _parent, string _name)
   {
      VvDataGridView theSumGrid = new VvDataGridView();

      theGrid.TheLinkedGrid_Sum = theSumGrid;

      theSumGrid.Parent = _parent;
      theSumGrid.Name   = _name;

      theSumGrid.Height = ZXC.QUN + ZXC.Qun10; //23;

      theSumGrid.BorderStyle = BorderStyle.FixedSingle;
      VvHamper.ApplyVVColorAndStyleTabCntrolChange(theSumGrid);
      theSumGrid.ColumnHeadersVisible = false;
      theSumGrid.CellBorderStyle      = DataGridViewCellBorderStyle.SingleVertical;// DataGridViewCellBorderStyle.None;
      theSumGrid.ReadOnly             = true;
      theSumGrid.Tag                  = theGrid;

      theSumGrid.AllowUserToAddRows       =
      theSumGrid.AllowUserToDeleteRows    =
      theSumGrid.AllowUserToOrderColumns  =
      theSumGrid.AllowUserToResizeColumns =
      theSumGrid.AllowUserToResizeRows    = false;

      theSumGrid.RowHeadersDefaultCellStyle.Alignment = theGrid.RowHeadersDefaultCellStyle.Alignment;
      theSumGrid.RowTemplate.Height                   = theGrid.RowTemplate.Height;

      //theSumGrid.ScrollBars = ScrollBars.None;

      theSumGrid.ScrollBars = ScrollBars.None;

      theSumGrid.Location = new Point(theGrid.Left, theGrid.Bottom);

      return theSumGrid;
   }

   private void InitializeTheSUMGrid_Columns(VvDataGridView theGrid)
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

   public void CalcLocationSizeAnchor_TheDGVAndTheSumGrid(VvDataGridView theGrid)
   {
      DataGridView theSumGrid   = theGrid.TheLinkedGrid_Sum;
   
      theGrid.Location = new Point(ZXC.Qun2, ZXC.Qun2);
      theGrid.Size     = new Size(theGrid.Parent.Width - ZXC.QUN, theGrid.Parent.Height - ZXC.Q2un - theSumGrid.Height);

      theGrid.Anchor        = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      theSumGrid.Width   = theGrid.Width;
      
      theGrid.Height = theGrid.Parent.Height - ZXC.Q2un - theSumGrid.Height;

      theSumGrid.Location = new Point(theGrid.Location.X, theGrid.Bottom + ZXC.Qun12);
      theSumGrid.Anchor   = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

      theSumGrid.Scroll += new ScrollEventHandler(theSumGrid_Scroll);
 //     theGrid.ColumnWidthChanged += new DataGridViewColumnEventHandler(grid_ColumnWidthChanged);
      theGrid.Scroll += new ScrollEventHandler(TheGrid_Scroll);


   }
   private void theSumGrid_Scroll(object sender, ScrollEventArgs e)
   {
      DataGridView theSumGrid = sender as DataGridView;
      VvDataGridView theGrid = (VvDataGridView)(theSumGrid.Tag);

      theGrid.HorizontalScrollingOffset = theSumGrid.HorizontalScrollingOffset;
      int sumNalog = theGrid.RowHeadersWidth;
      int sumSum = theSumGrid.RowHeadersWidth ;

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
   private void grid_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
   {
      VvDataGridView theGrid    = sender as VvDataGridView;
      DataGridView   theSumGrid = theGrid.TheLinkedGrid_Sum;

      int sumNalog = theGrid.RowHeadersWidth;
      int sumSum   = theSumGrid.RowHeadersWidth;

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

   #endregion TheG

   #region GridColumns

   private void CreateColumn_1(VvDataGridView theGrid)
   {
      vvtbR_projektCD_otp = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (   "vvtb4ColR_projektCD_otp", null, -12, "Projekt"                                                              );                   colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_projektCD_otp, null, "R_projektCD"    , "Projekt\n1"          , ZXC.Q5un); vvtbR_projektCD_otp.JAM_ReadOnly = true;
      vvtbR_investit_otp  = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (   "vvtb4ColR_projektCD_otp", null, -12, "Nar/Investi"                                                          );                   colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_investit_otp , null, "R_investit_otp" , "Naručitelj / Investitor\n2"       ,        0); vvtbR_investit_otp.JAM_ReadOnly = true;
      colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      colVvText.MinimumWidth = ZXC.Q6un;

       vvtbR_PlanCijena   = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_PlanCijena"   , null, -12, "Planirana cijena proizvodnje"                                   ); colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_PlanCijena  , null, "R_PlanCijena"  , "PlanProdCijena\n"  +  "3"         , ZXC.Q5un); vvtbR_PlanCijena  .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un; vvtbR_PlanCijena  .JAM_ReadOnly = true;
       vvtbR_PUT_dug_year = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_PUT_dug_year" , null, -12, "Proizvodnja u tijeku od 1.1. tekuće godine do zadanog razdoblja"); colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_PUT_dug_year, null, "R_PUT_dug_year", "Troškovi PR\n"     +  "4"         , ZXC.Q5un); vvtbR_PUT_dug_year.JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un; vvtbR_PUT_dug_year.JAM_ReadOnly = true;
       vvtbR_RD_razd      = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_RD_razd"      , null, -12, "Direktni troškovi zadanog razdoblja"                            ); colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_RD_razd     , null, "R_RD_razd"     , "DirektniTr\n"      +  "5"         , ZXC.Q5un); vvtbR_RD_razd     .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un; vvtbR_RD_razd     .JAM_ReadOnly = true;
       vvtbR_NIraspored   = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_NIraspored"   , null, -12, "Raspored neraspoređenih troškova zadanog razdoblja"             ); colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_NIraspored  , null, "R_NIraspored"  , "IndirektniTr\n"    +  "6"         , ZXC.Q5un); vvtbR_NIraspored  .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un; vvtbR_NIraspored  .JAM_ReadOnly = true; 
       vvtbR_RRraspored   = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_RRraspored"   , null, -12, "Režijski troškovi"                                              ); colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_RRraspored  , null, "R_RRraspored"  , "RežijskiTr\n"      +  "7"         , ZXC.Q5un); vvtbR_RRraspored  .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un; vvtbR_RRraspored  .JAM_ReadOnly = true; colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;
       vvtbR_AMraspored   = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_AMraspored"   , null, -12, "Troškovi amortizacije"                                          ); colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_AMraspored  , null, "R_AMraspored"  , "Amortizacija\n"    +  "8"         , ZXC.Q5un); vvtbR_AMraspored  .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un; vvtbR_AMraspored  .JAM_ReadOnly = true; colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;
       vvtbR_NewTr_year   = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_NewTr_year"   , null, -12, "Ukupni troškovi od 1.1. tekuće godine"                          ); colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_NewTr_year  , null, "R_NewTr_year"  , "UkTroškoviRazd\n"  + "9 (5+6+7+8)", ZXC.Q5un); vvtbR_NewTr_year  .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un; vvtbR_NewTr_year  .JAM_ReadOnly = true;
       vvtbR_UkTrRN       = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_UkTrRN"       , null, -12, "Ukupni troškovi po Radnom nalogu"                               ); colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_NewTr_year  , null, "R_UkTrRN"      , "SveukupniTr\n"     + "10 (9 + 4)" , ZXC.Q5un); vvtbR_UkTrRN      .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un; vvtbR_UkTrRN      .JAM_ReadOnly = true;
       vvtbR_RazlikaPlan  = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_RazlikaPlan"  , null, -12, "Razlika u odnosu na Planiranu Prodajnu Cijenu Proizvodnje"      ); colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_RazlikaPlan , null, "R_RazlikaPlan" , "RUC\n"             + "11 (3-10)"  , ZXC.Q5un); vvtbR_RazlikaPlan .JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un; vvtbR_RazlikaPlan .JAM_ReadOnly = true; colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;
       
      vvtbR_projektCD_otp.JAM_ForeColor = Color.Black;
      vvtbR_investit_otp .JAM_ForeColor = Color.Black;
      vvtbR_PlanCijena   .JAM_ForeColor = Color.Black;
      vvtbR_PUT_dug_year .JAM_ForeColor = Color.Black; 
      vvtbR_RD_razd      .JAM_ForeColor = Color.Black; 
      vvtbR_NIraspored   .JAM_ForeColor = Color.Black; 
      vvtbR_RRraspored   .JAM_ForeColor = Color.DarkBlue; 
      vvtbR_AMraspored   .JAM_ForeColor = Color.DarkBlue; 
      vvtbR_NewTr_year   .JAM_ForeColor = Color.DarkMagenta; 
      vvtbR_UkTrRN       .JAM_ForeColor = Color.DarkMagenta; 
      vvtbR_RazlikaPlan  .JAM_ForeColor = Color.DarkMagenta; 



      vvtbR_projektCD_otp.JAM_BackColor = Color.AliceBlue;
      vvtbR_investit_otp .JAM_BackColor = Color.AliceBlue;
      vvtbR_PlanCijena   .JAM_BackColor = Color.AliceBlue;
      vvtbR_PUT_dug_year .JAM_BackColor = Color.AliceBlue;
      vvtbR_RD_razd      .JAM_BackColor = Color.AliceBlue;
      vvtbR_NIraspored   .JAM_BackColor = Color.AliceBlue ;
      vvtbR_RRraspored   .JAM_BackColor = Color.PeachPuff;
      vvtbR_AMraspored   .JAM_BackColor = Color.PeachPuff; 
      vvtbR_NewTr_year   .JAM_BackColor = Color.MintCream; 
      vvtbR_UkTrRN       .JAM_BackColor = Color.MintCream; 
      vvtbR_RazlikaPlan  .JAM_BackColor = Color.LightGray; 

      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);
   }
   private void CreateColumn_2(VvDataGridView theGrid)
   {
      vvtbR_projektCD_fa  = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_projektCD_fa" , null, -12, "Projekt"               ); 
      colVvText           = theGrid.CreateVvTextBoxColumn(vvtbR_projektCD_fa , null, "R_projektCD"    , "Projekt"       , ZXC.Q5un); 
      vvtbR_projektCD_fa .JAM_ReadOnly = true;
     
      vvtbR_status_fa = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_status_fa", null, -12, "Status");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_status_fa, null, "R_status_fa", "Status", ZXC.Q2un);
      vvtbR_status_fa.JAM_ReadOnly = true;

      vvtbR_dovrsenost_fa = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_dovrsenost_fa", null, -12, "% Dovršenosti");
      vvtbR_dovrsenost_fa.JAM_IsForPercent = true;
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_dovrsenost_fa, null, "R_dovrsenost_fa", "Dovrš %", ZXC.Q3un - ZXC.Qun4);
      colVvText.MinimumWidth = ZXC.Q3un - ZXC.Qun4; 
      vvtbR_dovrsenost_fa.JAM_ReadOnly = true;

      vvtbR_ugCijena_fa = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb4ColR_ugCijena_fa", null, -12, "PlanCijena");
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_ugCijena_fa, null, "R_ugCijena_fa", "Planirana Cijena", ZXC.Q5un);
      vvtbR_ugCijena_fa.JAM_ShouldSumGrid = true; colVvText.MinimumWidth = ZXC.Q4un; vvtbR_ugCijena_fa.JAM_ReadOnly = true;
      

      vvtbR_narInv_fa     = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_narInv_fa"    , null, -12, "Naručitelj/Investitor" ); 
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_narInv_fa    , null, "R_narInv_fa"    , "Naručitelj / Investitor", ZXC.Q8un); 
      vvtbR_narInv_fa    .JAM_ReadOnly = true;

      vvtbR_objektCd_fa   = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_objektCd_fa"  , null, -12, "Objekt šifra"          ); 
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_objektCd_fa  , null, "R_objektCd_fa"  , "ŠifObjekta"      , ZXC.Q4un); 
      vvtbR_objektCd_fa  .JAM_ReadOnly = true;

      vvtbR_objektName_fa = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_objektName_fa", null, -12, "Objekt"                ); 
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_objektName_fa, null, "R_objektName_fa", "Naziv Objekta"        , 0); 
      vvtbR_objektName_fa.JAM_ReadOnly = true;
      colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      colVvText.MinimumWidth = ZXC.Q7un;

      vvtbR_refDok_fa     = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb4ColR_refDok_fa"    , null, -12, "RefDok"                ); 
      colVvText = theGrid.CreateVvTextBoxColumn(vvtbR_refDok_fa    , null, "R_refDok_fa"    , "RefDok"        , ZXC.Q10un); 
      vvtbR_refDok_fa    .JAM_ReadOnly = true;

      colDate = theGrid.CreateCalendarColumn_R(null, "R_dateStart_fa", "PočIzvođ", ZXC.Q3un + ZXC.Qun2); colDate.ReadOnly = true;
      
      colDate = theGrid.CreateCalendarColumn_R(null, "R_dateRokIsp_fa", "RokIzvođ", ZXC.Q3un + ZXC.Qun2); colDate.ReadOnly = true;

     
      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);

   }

   #endregion GridColumns

   #region SetColumnIndexes()

   private       OTP_colIdx ci1;
   public        OTP_colIdx DgvCI1 { get { return ci1; } }
   public struct OTP_colIdx
   {
      internal int iT_recID        ;
      internal int iT_projektCD    ;
      internal int iT_PUT_dug_year ;
      internal int iT_RD_razd      ;
      internal int iT_NIraspored   ;
      internal int iT_NewTr_year   ;
      internal int iT_PlanCijena   ;
      internal int it_Investit_otp ;
      internal int it_RRraspored ;
      internal int it_AMraspored ;
      internal int it_IRVsati    ;
      internal int it_UkTrRN     ;
      internal int it_RazlikaPlan;

      }
   private void SetOTPColumnIndexes()
   {
      ci1 = new OTP_colIdx();

      ci1.iT_recID         = TheGrid_1.IdxForColumn("T_recID"        );
      ci1.iT_projektCD     = TheGrid_1.IdxForColumn("R_projektCD"    );
      ci1.iT_PUT_dug_year  = TheGrid_1.IdxForColumn("R_PUT_dug_year" );
      ci1.iT_RD_razd       = TheGrid_1.IdxForColumn("R_RD_razd"      );
      ci1.iT_NIraspored    = TheGrid_1.IdxForColumn("R_NIraspored"   );
      ci1.iT_NewTr_year    = TheGrid_1.IdxForColumn("R_NewTr_year"   );
      ci1.iT_PlanCijena    = TheGrid_1.IdxForColumn("R_PlanCijena"   );
      ci1.it_Investit_otp  = TheGrid_1.IdxForColumn("R_investit_otp");
      ci1.it_RRraspored    = TheGrid_1.IdxForColumn("R_RRraspored" );
      ci1.it_AMraspored    = TheGrid_1.IdxForColumn("R_AMraspored" );
      ci1.it_IRVsati       = TheGrid_1.IdxForColumn("R_IRVsati"    );
      ci1.it_UkTrRN        = TheGrid_1.IdxForColumn("R_UkTrRN"     );
      ci1.it_RazlikaPlan   = TheGrid_1.IdxForColumn("R_RazlikaPlan");
   }

   private       RnpFaktur_colIdx ci2;
   public        RnpFaktur_colIdx DgvCI2 { get { return ci2; } }
   public struct RnpFaktur_colIdx
   {
      internal int iT_recID        ;
      internal int iT_projektCD    ;
      internal int iT_narInv_fa    ;
      internal int iT_objektCd_fa  ;
      internal int iT_objektName_fa;
      internal int iT_refDok_fa    ;
      internal int iT_dateStart_fa ;
      internal int iT_dateRokIsp_fa;
      internal int iT_ugCijena_fa  ;
      internal int iT_dovrsenost_fa;
      internal int iT_status_fa    ;
   }
   private void SetRnpFakturListColumnIndexes()
   {
      ci2 = new RnpFaktur_colIdx();

      ci2.iT_recID         = TheGrid_2.IdxForColumn("T_recID");
      ci2.iT_projektCD     = TheGrid_2.IdxForColumn("R_projektCD");
      ci2.iT_narInv_fa     = TheGrid_2.IdxForColumn("R_narInv_fa");
      ci2.iT_objektCd_fa   = TheGrid_2.IdxForColumn("R_objektCd_fa");
      ci2.iT_objektName_fa = TheGrid_2.IdxForColumn("R_objektName_fa");
      ci2.iT_refDok_fa     = TheGrid_2.IdxForColumn("R_refDok_fa");
      ci2.iT_dateStart_fa  = TheGrid_2.IdxForColumn("R_dateStart_fa");
      ci2.iT_dateRokIsp_fa = TheGrid_2.IdxForColumn("R_dateRokIsp_fa");
      ci2.iT_ugCijena_fa   = TheGrid_2.IdxForColumn("R_ugCijena_fa");
      ci2.iT_dovrsenost_fa = TheGrid_2.IdxForColumn("R_dovrsenost_fa");
      ci2.iT_status_fa     = TheGrid_2.IdxForColumn("R_status_fa");

   }

   #endregion SetNalogColumnIndexes()

}


public class NiceKnDlg : VvDialog
{
   #region Fieldz
 
   private Button    okButton, cancelButton, applyButton;
   private VvHamper  hamper, applyHamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_redak, tbx_artiklCD, tbx_artiklName, tbx_msrpEUR, tbx_msrpVPC, tbx_zadanoDev, tbx_zadanoVPC, tbx_zadanoMPC, tbx_msrpMPC, tbx_posto;
   private  Label    lbl_rowIdx, lbl_artiklCD, lbl_artiklName, lbl_ValutaName;
   private string    redak, artiklCD, artiklName, devName;
   private decimal   devTecaj, theMSRP;
   DateTime          dokDate;

   #endregion Fieldz

   #region Constructor

   public NiceKnDlg(int _rowIdx, string _artiklCD, string _artiklName, string _devName, decimal _devTecaj, DateTime _dokDate, decimal _theMSRP)
   {
      this.StartPosition = FormStartPosition.WindowsDefaultLocation;
      this.Text          = "";

      redak      = _rowIdx.ToString();
      artiklCD   = _artiklCD         ;
      artiklName = _artiklName       ;
      devName    = _devName          ;
    //devTecaj   = _devTecaj         ;
      devTecaj   = _devTecaj.NotZero() ? _devTecaj : 1.00M;
      dokDate    = _dokDate          ;
      theMSRP    = _theMSRP          ;

      CreateHamper();
      CreateApplyHamper();

      dlgWidth        = hamper.Right  + ZXC.QunMrgn;
      dlgHeight       = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_msrpEUR   , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_zadanoVPC , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_msrpVPC   , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_zadanoDev , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_posto     , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_redak     , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_artiklCD  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_artiklName, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_msrpMPC   , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_zadanoMPC , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvDialog);

      this.AcceptButton = applyButton;

      this.cancelButton.Click += new EventHandler(cancelButton_Click); // Da supresa validaciju
      //this.FormClosing        += new FormClosingEventHandler(VvAddDataGridRowFromOtherDocDlg_FormClosing);


      // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 
      Fld_MsrpEUR = theMSRP;

      if(theMSRP.NotZero()) { SendKeys.Send("{TAB}"); tbx_zadanoMPC.Focus();  }
   }

   private void CreateApplyHamper()
   {
      applyHamper          = new VvHamper(1, 1, "", this, false);
      applyHamper.Location = new Point(ZXC.QunMrgn + ZXC.Q4un + ZXC.Qun2, hamper.Bottom + ZXC.Qun2 + ZXC.Qun4);

      applyHamper.VvColWdt    = new int[] { ZXC.QunBtnW };
      applyHamper.VvSpcBefCol = new int[] {           0 };
      applyHamper.VvRightMargin = 0;

      for(int i = 0; i < applyHamper.VvNumOfRows; i++)
      {
         applyHamper.VvRowHgt[i]    = ZXC.QunBtnH;
         applyHamper.VvSpcBefRow[i] = 0;
      }
      applyHamper.VvBottomMargin = applyHamper.VvTopMargin;

      applyButton = applyHamper.CreateVvButton(0, 0, null /*new EventHandler(Apply_Click)*/, "Apply");
      applyButton.DialogResult = DialogResult.Retry; // trik da u SubModulActions znas da si Apply 

   }

   #endregion Constructor

   #region EventHandler
   
   void cancelButton_Click(object sender, EventArgs e)
   {
      // Fld_DocNum = 1;
   }
   //private void Apply_Click(object sender, EventArgs e)
   //{
   //}

   #endregion EventHandler

   #region hamper

   private void CreateHamper()
   {
      hamper          = new VvHamper(5, 7, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q4un, ZXC.Q4un, ZXC.Q4un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
      lbl_rowIdx     = hamper.CreateVvLabel(0, 0, "Redak:", ContentAlignment.MiddleRight);
      lbl_artiklCD   = hamper.CreateVvLabel(0, 1, "Šifra:", ContentAlignment.MiddleRight);
      lbl_artiklName = hamper.CreateVvLabel(0, 2, "Naziv:", ContentAlignment.MiddleRight);

      tbx_redak      = hamper.CreateVvTextBox(1, 0, "tbx_redak"     , redak     , 120, 2, 0);
      tbx_artiklCD   = hamper.CreateVvTextBox(1, 1, "tbx_artiklCD"  , artiklCD  , 120, 2, 0);
      tbx_artiklName = hamper.CreateVvTextBox(1, 2, "tbx_artiklName", artiklName, 120, 2, 0);

      tbx_redak     .JAM_ReadOnly = 
      tbx_artiklCD  .JAM_ReadOnly = 
      tbx_artiklName.JAM_ReadOnly = true;

      tbx_redak.     Text  = redak     ;
      tbx_artiklCD.  Text  = artiklCD  ;
      tbx_artiklName.Text  = artiklName;

      string lblVPC = ZXC.IsEURoERA_projectYear ? "EUR VPC" : "Kn VPC";
      string lblMPC = ZXC.IsEURoERA_projectYear ? "EUR MPC" : "Kn MPC";

      lbl_ValutaName = hamper.CreateVvLabel(1, 4, devName   , ContentAlignment.MiddleRight);
                       hamper.CreateVvLabel(2, 4, lblVPC      , ContentAlignment.MiddleRight);
                       hamper.CreateVvLabel(3, 4, lblMPC     , ContentAlignment.MiddleRight);
                       hamper.CreateVvLabel(4, 4, "Promjena", ContentAlignment.MiddleRight);
                       
                       hamper.CreateVvLabel(0, 5, "MSRP" , ContentAlignment.MiddleRight);
                       hamper.CreateVvLabel(0, 6, "Zadano", ContentAlignment.MiddleRight);
     

      tbx_msrpEUR   = hamper.CreateVvTextBox(1, 5, "tbx_msrpVal"  , "", 12);
      tbx_msrpVPC    = hamper.CreateVvTextBox(2, 5, "tbx_msrpKn"   , "", 12);
      tbx_zadanoDev = hamper.CreateVvTextBox(1, 6, "tbx_zadanoVal", "", 12);
      tbx_zadanoVPC  = hamper.CreateVvTextBox(2, 6, "tbx_zadanoKn" , "", 12);
      tbx_msrpMPC   = hamper.CreateVvTextBox(3, 5, "tbx_msrpMPC"  , "", 12);
      tbx_zadanoMPC = hamper.CreateVvTextBox(3, 6, "tbx_zadanoMPC", "", 12);
      tbx_posto     = hamper.CreateVvTextBox(4, 6, "tbx_posto"    , "", 12);

      tbx_msrpMPC  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, false);
      tbx_zadanoMPC.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, false);
      tbx_msrpEUR  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, false);
      tbx_msrpVPC  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, false);
      tbx_zadanoDev.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, false);
      tbx_zadanoVPC .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, false);
      tbx_posto    .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, false);
      tbx_posto    .JAM_IsForPercent = true;

      tbx_msrpEUR .JAM_FieldExitMethod = new EventHandler(OnExitRecalcOvoOno);
      tbx_zadanoMPC.JAM_FieldExitMethod = new EventHandler(OnExitRecalcOvoOno);

      tbx_msrpEUR  .Tag = false; // bool isZadanoMPC = (bool)tbx.Tag;
      tbx_zadanoMPC.Tag = true ; // bool isZadanoMPC = (bool)tbx.Tag;

      tbx_zadanoVPC .JAM_ReadOnly = 
      tbx_msrpVPC   .JAM_ReadOnly = 
      tbx_zadanoDev.JAM_ReadOnly = 
      tbx_posto    .JAM_ReadOnly = true;

   }

   #endregion hamper

   #region Fld_

   public decimal Fld_MsrpEUR   { get { return tbx_msrpEUR  .GetDecimalField(); } set { tbx_msrpEUR  .PutDecimalField(value); } }
   public decimal Fld_MsrpVPC   { get { return tbx_msrpVPC   .GetDecimalField(); } set { tbx_msrpVPC   .PutDecimalField(value); } }
   public decimal Fld_ZadanoEUR { get { return tbx_zadanoDev.GetDecimalField(); } set { tbx_zadanoDev.PutDecimalField(value); } }
   public decimal Fld_ZadanoVPC { get { return tbx_zadanoVPC .GetDecimalField(); } set { tbx_zadanoVPC .PutDecimalField(value); } }
   public decimal Fld_Posto     { get { return tbx_posto    .GetDecimalField(); } set { tbx_posto    .PutDecimalField(value); } }
   public decimal Fld_MsrpMPC   { get { return tbx_msrpMPC  .GetDecimalField(); } set { tbx_msrpMPC  .PutDecimalField(value); } }
   public decimal Fld_ZadanoMPC { get { return tbx_zadanoMPC.GetDecimalField(); } set { tbx_zadanoMPC.PutDecimalField(value); } }

   #endregion Fld_

   public void OnExitRecalcOvoOno(object sender, EventArgs e)
   {
      VvTextBox tbx = sender as VvTextBox;
      bool isZadanoMPC = (bool)tbx.Tag;

      bool isUseless_EURoDevName = false;
      bool isUseless_KuneDevName = devName.ToUpper() == "HRK";

      // 12.01.2023: HB Tamsi :-) 
      if(ZXC.IsEURoERA_projectYear) // >= 2023 
      {
         if(devName.ToUpper() == "EUR") isUseless_EURoDevName = true ;
      }

      bool isUselessDevName = isUseless_EURoDevName || isUseless_KuneDevName;

      if(isZadanoMPC)
      {
         Fld_ZadanoVPC = ZXC.VvGet_100_from_125(Fld_ZadanoMPC, Faktur.CommonPdvStForThisDate(dokDate));
         Fld_ZadanoEUR = ZXC.DivSafe(Fld_ZadanoVPC, (isUselessDevName ? 1.00M : devTecaj));
      }
      else
      {
         Fld_MsrpVPC = Fld_MsrpEUR * (isUselessDevName ? 1.00M : devTecaj);
         Fld_MsrpMPC = ZXC.VvGet_125_on_100(Fld_MsrpVPC, Faktur.CommonPdvStForThisDate(dokDate));
      }

      Fld_Posto = ZXC.StopaPromjene(Fld_MsrpMPC, Fld_ZadanoMPC);
   }

}


public class AddDateToWYRNdlg : VvDialog
{
   #region Propertiez

   #endregion Propertiez

   #region Fieldz

   private Button okButton, abortButton;
   private int    dlgWidth, dlgHeight;

   public  AddDateToWYRNUC TheUC { get; private set; }
  
   #endregion Fieldz

   #region Constructor

 //public AddDateToWYRNdlg(VvUserControl theNalogDUC, Ftrans ftrans_rec)
   public AddDateToWYRNdlg(VvUserControl theNalogDUC, List<Ftrans> ftrGRlist)
   {
      Ftrans ftrans_rec = ftrGRlist.First(ftr => ftr.T_otsKind == ZXC.OtsKindEnum.OTVARANJE);

      TheUC = new AddDateToWYRNUC();

      Kupdob kupdob_rec = theNalogDUC.Get_Kupdob_FromVvUcSifrar(ftrans_rec.T_kupdob_cd);

      if(kupdob_rec != null)
      {
         TheUC.Fld_KupdobCd   = kupdob_rec.KupdobCD;
         TheUC.Fld_KupdobTk   = kupdob_rec.Ticker  ;
         TheUC.Fld_KupdobName = kupdob_rec.Naziv   ;
      }

      TheUC.Fld_Konto      = ftrans_rec.T_konto        ;
      TheUC.Fld_Opis       = ftrans_rec.T_opis         ;
      TheUC.Fld_TipBr      = ftrans_rec.R_forcedTipBr  ;
      TheUC.Fld_DokDate    = ftrans_rec.DokDateFromOpis;
      TheUC.Fld_DospDate   = ftrans_rec.T_valuta       ;

      if(ftrans_rec.DokDateFromOpis.IsEmpty() && ftrans_rec.T_valuta       .NotEmpty()) TheUC.Fld_DokDate  = ftrans_rec.T_valuta       ;
      if(ftrans_rec.T_valuta       .IsEmpty() && ftrans_rec.DokDateFromOpis.NotEmpty()) TheUC.Fld_DospDate = ftrans_rec.DokDateFromOpis;

    //decimal kcrp  = ftrans_rec.R_DugMinusPot_ABS;
    //decimal kcrp  = ftrans_rec.R_WyrnKCRP       ;
      decimal kcrp  = ftrGRlist.Where(ftr => ftr.T_otsKind == ZXC.OtsKindEnum.OTVARANJE).Sum(f => f.R_WyrnKCRP);
      decimal pdvSt = Faktur.CommonPdvStForThisDate(TheUC.Fld_DokDate);


      TheUC.Fld_PdvSt    = pdvSt;
      TheUC.Fld_KCRP     = kcrp ;
      TheUC.Fld_Prolaz   = 0M   ;
      TheUC.Fld_Osnovica = ZXC.VvGet_100_from_125(kcrp, pdvSt);
      TheUC.Fld_Pdv      = (kcrp - TheUC.Fld_Osnovica).Ron2() ;

      SuspendLayout();

      this.Font        = ZXC.vvFont.BaseFont;
      this.Style       = ZXC.vvColors.vvform_VisualStyle;
      this.BackColor   = ZXC.vvColors.userControl_BackColor;

      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Potvrdite / dopunite WYRN podatke";

      CreateTheUC();

      dlgWidth        = TheUC.Width;
      dlgHeight       = TheUC.Height + ZXC.QunMrgn * 2 + ZXC.QunBtnH;


      this.ClientSize = new Size(dlgWidth, dlgHeight);
      AddOkAbortButtons(out okButton, out abortButton, dlgWidth, dlgHeight);
      okButton.Anchor = abortButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
    //AddOkButton(out okButton, dlgWidth, dlgHeight);
    //okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      this.ControlBox = false;

      ResumeLayout();

   }

   #endregion Constructor

   #region UC

   private void CreateTheUC()
   {
      TheUC.Parent   = this;
      TheUC.Location = new Point(0, 0);
   }

   #endregion UC

   #region Eveniti

   #endregion Eveniti

}

public class AddDateToWYRNUC : VvOtherUC
{
   #region Fieldz

   private VvHamper         hamp_podaci, hamp_iznosi, hamp_date;
   private VvDateTimePicker dtp_dokDate, dtp_dospDate;
   private VvTextBox        tbx_dokDate, tbx_dospDate, tbx_kupDobName, tbx_kupDobTk, tbx_kupDobCd, tbx_tipBr, tbx_opis,
                            tbx_kcrp, tbx_kcr, tbx_pdv, tbx_pdvSt, tbx_ukOsnPr, tbx_konto, tbx_pdvKolTip;

   #endregion Fieldz

   #region Constructor

   public AddDateToWYRNUC()
   {

      SuspendLayout();

      CreateHampers();

      this.Size = new Size(hamp_podaci.Right + ZXC.Q2un, hamp_iznosi.Bottom + ZXC.Q2un);

      VvHamper.Open_Close_Fields_ForWriting(tbx_kupDobName, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_kupDobTk  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_kupDobCd  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_opis      , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_konto     , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_tipBr     , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_kcrp      , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_kcr       , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_pdv       , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_pdvSt     , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_ukOsnPr   , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_pdvKolTip , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_dokDate   , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_dospDate  , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvOtherUC);

      ResumeLayout();

      this.Validating += new CancelEventHandler(AddDateToWYRNUC_Validating);

   }

   void AddDateToWYRNUC_Validating(object sender, CancelEventArgs e)
   {
    //decimal thisCalcKCRP = ZXC.VvGet_125_on_100(Fld_Osnovica, Fld_PdvSt) + Fld_Prolaz;
      decimal thisCalcKCRP =                      Fld_Osnovica + Fld_Pdv   + Fld_Prolaz;
      if(ZXC.AlmostEqual(Fld_KCRP, thisCalcKCRP, 0.02M) == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nIskalkulirane sume se ne poklapaju sa iznosom računa.\n\nRačun\t{0}\nOvajKlk\t{1}", Fld_KCRP, thisCalcKCRP);
         e.Cancel = true;
      }
    //if(CtrlOK(theFakturExtDUC.tbx_KupdobCd) && (faktur_rec.KupdobCD.IsZero() || faktur_rec.KupdobName.IsEmpty() || faktur_rec.KupdobTK.IsEmpty()))
    //{
    //   ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nMolim, zadajte partnera prije usnimavanja.");
    //   e.Cancel = true;
    //}
      if(Fld_TipBr.IsEmpty() || Fld_DospDate.IsEmpty() || Fld_DokDate.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nBroj računa, datum računa i dospjeće (valuta) plaćanja\n\nMORAJU BITI ZADANI!", Fld_KCRP, thisCalcKCRP);
         e.Cancel = true;
      }
      if(Fld_DospDate.NotEmpty() && Fld_DokDate.NotEmpty() && Fld_DospDate < Fld_DokDate)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nDatum dospjeća (valute) ne smije biti manji od datuma računa!\n\nDatum rn\t{1}\n\nValuta rn\t{0}", Fld_DospDate.ToString(ZXC.VvDateFormat), Fld_DokDate.ToString(ZXC.VvDateFormat));
         e.Cancel = true;
      }
      if(Fld_DokDate.Year == ZXC.projectYearFirstDay.Year)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nDatum računa ne može biti iz tekuće godine!");
         e.Cancel = true;
      }
   }

   #endregion Constructor

   #region Hampers

   private void CreateHampers()
   {
      InitializeHamper_Podaci(out hamp_podaci);
      InitializeHamper_Datum (out hamp_date);
      InitializeHamper_Iznosi(out hamp_iznosi);
   }


   private void InitializeHamper_Podaci(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 4, "", this, false);

      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);
                                       //    0         1         2         3                4  
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q4un, ZXC.Q4un, ZXC.Q10un +ZXC.Q3un, ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8           , ZXC.Qun8};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Konto:"  , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 1, "Partner:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 2, "Opis:"   , ContentAlignment.MiddleRight);

                                                                         
      tbx_konto      = hamper.CreateVvTextBox(1, 0, "tbx_konto"     , ""          );
      tbx_kupDobCd   = hamper.CreateVvTextBox(1, 1, "tbx_kupDobCd"  , ""          );
      tbx_kupDobTk   = hamper.CreateVvTextBox(2, 1, "tbx_kupDobTk"  , ""          );
      tbx_kupDobName = hamper.CreateVvTextBox(3, 1, "tbx_kupDobName", ""          );
      tbx_opis       = hamper.CreateVvTextBox(1, 2, "tbx_opis"      , "", 80, 2, 0);

   }


   private void InitializeHamper_Datum(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 4, "", this, false);

      hamper.Location = new Point(ZXC.QunMrgn, hamp_podaci.Bottom + ZXC.QunMrgn);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q4un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun2;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                  hamper.CreateVvLabel  (0, 0, "TT Br:", ContentAlignment.MiddleRight);
      tbx_tipBr = hamper.CreateVvTextBox(1, 0, "tbx_tipBr", "", 40, 1, 0);


                    hamper.CreateVvLabel  (0, 2, "Datum računa :", 1, 0, ContentAlignment.MiddleRight);
      tbx_dokDate = hamper.CreateVvTextBox(2, 2, "tbx_datumOd", "Datum računa");
      tbx_dokDate.JAM_IsForDateTimePicker = true;
      dtp_dokDate = hamper.CreateVvDateTimePicker(2, 2, "", tbx_dokDate);
      dtp_dokDate.Name = "dtp_dokDate";
      dtp_dokDate.Tag  = tbx_dokDate;
      tbx_dokDate.Tag  = dtp_dokDate;

                     hamper.CreateVvLabel  (0, 3, "Valuta plaćanja:", 1, 0, ContentAlignment.MiddleRight);
      tbx_dospDate = hamper.CreateVvTextBox(2, 3, "tbx_datumDo", "");
      tbx_dospDate.JAM_IsForDateTimePicker = true;
      dtp_dospDate = hamper.CreateVvDateTimePicker(2, 3, "", tbx_dospDate);
      dtp_dospDate.Name = "dtp_dospDate";
      dtp_dospDate.Tag = tbx_dospDate;
      tbx_dospDate.Tag = dtp_dospDate;

   }
 
   private void InitializeHamper_Iznosi(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 6, "", this, false);

      hamper.Location = new Point(hamp_date.Right + ZXC.QunMrgn, hamp_date.Top);
                                       //    0         1    
      hamper.VvColWdt      = new int[] { ZXC.Q8un, ZXC.QUN , ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8,        0, ZXC.Qun8};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Osnovica:"           , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 1, "PDV stopa:"          , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 2, "Iznos PDV-a:"        , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 3, "ProlaznaSt:"         , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 4, "PK kolona:"          , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 5, "Ukupan iznos računa:", ContentAlignment.MiddleRight);
                                                                         
      tbx_kcr        = hamper.CreateVvTextBox(1, 0, "tbx_kcr"      , "", 12, 1, 0);
      tbx_pdvSt      = hamper.CreateVvTextBox(1, 1, "tbx_pdvSt"    , "",  6, 1, 0);
      tbx_pdv        = hamper.CreateVvTextBox(1, 2, "tbx_pdv"      , "", 12, 1, 0);
      tbx_ukOsnPr    = hamper.CreateVvTextBox(1, 3, "tbx_ukOsnPr"  , "", 12, 1, 0);
      tbx_pdvKolTip  = hamper.CreateVvTextBox(1, 4, "tbx_pdvKolTip", "",  1      );
      tbx_kcrp       = hamper.CreateVvTextBox(1, 5, "tbx_kcrp"     , "", 12, 1, 0);

      tbx_kcrp   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_kcr    .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_pdvSt  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_pdv    .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_ukOsnPr.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      tbx_pdvSt.JAM_IsForPercent = true;


      tbx_pdvKolTip.JAM_AllowedInputCharacters = "7890123456MNPUA";
      tbx_pdvKolTip.JAM_CharacterCasing = CharacterCasing.Upper;

      //URA: M-može se odbiti, N-ne može se odbiti, P-prolazna stavka   
      //IRA: 7-kolona 7, 8-kolona 8, 9-kolona 9, 0-kolona 10, 1-kolona 11, 2-kolona 12, 3-kolona 13, 4-kolona 14, 5-kolona 15, 6-kolona 16
      //A-avans, U-umjetnina
   }

   #endregion Hampers

   #region Fld_

   //public DateTime Fld_DokDate
   //{
   //   get
   //   {
   //      return ZXC.ValOr_01010001_DtpDateTime(dtp_dokDate.Value);
   //   }
   //   set
   //   {
   //      dtp_dokDate.Value = ZXC.ValOr_01011753_DateTime(value);
   //      tbx_dokDate.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_dokDate);
   //   }
   //}

   public DateTime Fld_DokDate
   {
      get { return dtp_dokDate.Value; }
      set
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dtp_dokDate.Value = value;
         }
      }
   }

   public DateTime Fld_DospDate
   {
      get { return dtp_dospDate.Value; }
      set
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dtp_dospDate.Value = value;
         }
      }
   }


   public decimal  Fld_KCRP       { get { return tbx_kcrp      .GetDecimalField()  ; } set { tbx_kcrp       .PutDecimalField(value) ; } }
   public decimal  Fld_Osnovica   { get { return tbx_kcr       .GetDecimalField()  ; } set { tbx_kcr        .PutDecimalField(value) ; } }
   public decimal  Fld_PdvSt      { get { return tbx_pdvSt     .GetDecimalField()  ; } set { tbx_pdvSt      .PutDecimalField(value) ; } }
   public decimal  Fld_Pdv        { get { return tbx_pdv       .GetDecimalField()  ; } set { tbx_pdv        .PutDecimalField(value) ; } }
   public decimal  Fld_Prolaz     { get { return tbx_ukOsnPr   .GetDecimalField()  ; } set { tbx_ukOsnPr    .PutDecimalField(value) ; } }
                                                               
   public uint     Fld_KupdobCd   { get { return tbx_kupDobCd  .GetSomeRecIDField(); } set { tbx_kupDobCd  .PutSomeRecIDField(value); } }
   public string   Fld_KupdobTk   { get { return tbx_kupDobTk  .Text               ; } set { tbx_kupDobTk  .Text = value            ; } }
   public string   Fld_KupdobName { get { return tbx_kupDobName.Text               ; } set { tbx_kupDobName.Text = value            ; } }
   public string   Fld_Opis       { get { return tbx_opis      .Text               ; } set { tbx_opis      .Text = value            ; } }
   public string   Fld_TipBr      { get { return tbx_tipBr     .Text               ; } set { tbx_tipBr     .Text = value            ; } }
   public string   Fld_Konto      { get { return tbx_konto     .Text               ; } set { tbx_konto     .Text = value            ; } }
 //public string   Fld_PdvKolTip  { get { return tbx_pdvKolTip .Text               ; } set { tbx_pdvKolTip .Text = value            ; } }

   public ZXC.PdvKolTipEnum Fld_PdvKolTip
   {
      get 
      {
         return FakturDUC.GetPdvKolTipEnumFromFirstLetter(tbx_pdvKolTip.Text); // (ZXC.PdvKolTipEnum)Enum.Parse(typeof(ZXC.PdvKolTipEnum), tbx_pdvKolTip.Text, true);
      }
    //set {                           this.currentData._t_pdvKolTip =(ushort) value; }
   }

   #endregion Fld_

   #region PutFields(), GetFields()


   #endregion PutFields(), GetFields()

   #region Eveniti

   #endregion Eveniti

}

public class VvBackup_FromVvXmlDRDlg : VvDialog
{
   #region Filedz

   private Button           okButton, cancelButton;
   private VvHamper         hamper;
   private int              dlgWidth, dlgHeight;
   private VvTextBox        tbx_dateTime;
   private VvDateTimePicker dtp_dateTime;

   #endregion Filedz

   #region Constructor

   public VvBackup_FromVvXmlDRDlg()
   {
      this.StartPosition = FormStartPosition.WindowsDefaultLocation;
      this.Text = "Učitavanje xml-a";

      CreateHamper();

      dlgWidth        = hamper.Right + ZXC.QunMrgn;
      dlgHeight       = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_dateTime, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvOtherUC);
      VvHamper.Open_Close_Fields_ForWriting(dtp_dateTime, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvOtherUC);

      //Fld_DateTime = DateTime.Now;
   }

   #endregion Constructor

   #region hamper

   private void CreateHamper()
   {
      hamper          = new VvHamper(2, 1, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q9un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                     hamper.CreateVvLabel  (0, 0, "Od vremena:", ContentAlignment.MiddleRight);
      tbx_dateTime = hamper.CreateVvTextBox(1, 0, "tbx_dateTime", "");
      tbx_dateTime.JAM_IsForDateTimePicker = true;
      dtp_dateTime = hamper.CreateVvDateTimePicker(1, 0, "", tbx_dateTime);
      dtp_dateTime.Name = "dtp_dateTime";
      tbx_dateTime.JAM_IsForDateTimePicker_WithTimeDisplay = true;

   }

   #endregion hamper

   #region Button_Click

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Button_Click

   #region Fld_

   public DateTime Fld_DateTime
   {
      get
      {
         return dtp_dateTime.Value;
      }
      set
      {
         if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime)
         {
            dtp_dateTime.Value = value;
         }
      }
   }

   #endregion Fld_

}

