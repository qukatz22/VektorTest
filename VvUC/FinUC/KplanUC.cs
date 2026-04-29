using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using CrystalDecisions.Windows.Forms;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using                  MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
using System.Collections.Generic;
using Vektor.Reports.FIZ;
#endif


public class KplanUC : VvSifrarRecordUC
{
   #region Fieldz

   public VvTextBox     tbx_konto, tbx_tip, tbx_naziv, tbx_naziv2, tbx_naziv3, tbx_opis, tbx_anaGr, tbx_klasa, tbx_klasaNaziv, tbx_subKlasa, tbx_subKlasaNaziv,
                        tbx_fond, tbx_fondOpis;
   public  VvHamper     hamp_konto, hamp_Open, hamp_psRule;
   private int          nextX = 0, nextY = 0, razmakHamp = ZXC.Qun10;
   private Kplan        kplan_rec;
   private Button       btn_PlusMinus;
   private Label        lblCrta, lbl_klasa, lbl_subKlasa;
   private RadioButton  rbt_noOverr, rbt_supress, rbt_force;

   private KplanDao.KplanCI DB_ci
   {
      get { return ZXC.KplCI; }
   }
   
   #endregion Fieldz

   #region Constructor

   public KplanUC(Control parent, Kplan _kplan, VvSubModul vvSubModul)
   {
      kplan_rec         = _kplan;

      ThePrefferedRecordSorter = VirtualDataRecord.DefaultSorter;

      this.TheSubModul = vvSubModul;

      SuspendLayout();

      CreateTabPages(parent);
      CreateHampers();
      InitializeVvUserControl(parent);
      CreateDataGridView_InitializeTheGrid_ReadOnly_Columns();

      this.Validating += new System.ComponentModel.CancelEventHandler(KplanUC_Validating);

      ResumeLayout();
   }

   void KplanUC_Validating(object sender, System.ComponentModel.CancelEventArgs e)
   {
      #region Should validate enivej?

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None ||
         TheVvTabPage.WriteMode == ZXC.WriteMode.Delete ||
         this.Visible == false) return;

      #endregion Should validate enivej?


      //Dodajemo sifrar record u PG, pa treba provjeriti je li slobodna sifra i u NY 
      (bool thisSifraIs_Duplicated_InNY, VvSifrarRecord inNY_SifrarRecord) = IsThisSifra_Duplicated_InNY();

      if(thisSifraIs_Duplicated_InNY)
      {
         e.Cancel = true;

         ZXC.aim_emsg(MessageBoxIcon.Stop, "Dodajete konto već zauzet u novoj godini.\n\r\n\rIspravite ovu šifru na prvu sljedeću slobodnu,\n\r\n\rtj. da je 'slobodna' i u ovoj i u novoj godini.\n\r\n\ru novoj godini je:\n\r\n\r{0}", inNY_SifrarRecord);
      }
   }

   #endregion Constructor

   #region TabPages

   private void CreateTabPages(Control _parent)
   {
      TheTabControl.TabPages.Add(CreateVvInnerTabPages("Matični", "", ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));

      VvTabPage vvTabPage = (VvTabPage)(_parent.Parent);

      if(vvTabPage.TabPageKind != ZXC.VvTabPageKindEnum.RECORD_TabPage_INTERACTIVE)
         TheTabControl.TabPages.Add(CreateVvInnerTabPages(ftrans_TabPageName, "", ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage));

      // Add-a se u click buttonu pogled
      //TheTabControl.TabPages.Add(CreateVvInnerTabPages("Print"           , ZXC.VvInnerTabPageKindEnum.ReportViewer_TabPage));
   }

   #endregion TabPages

   #region  HAMPER 
   
   private void CreateHampers()
   {
      InitializeKontoHamperProperties(out hamp_konto);
      InitializeHamperPsRule(out hamp_psRule);
   }

   private void InitializeKontoHamperProperties(out VvHamper kontoHamper)
   {
      kontoHamper = new VvHamper( 5, 9, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      kontoHamper.VvColWdt       = new int[] { ZXC.Q3un, ZXC.Q4un, ZXC.Q3un, ZXC.Q10un, ZXC.QUN };
      kontoHamper.VvSpcBefCol    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      kontoHamper.VvRightMargin  = kontoHamper.VvLeftMargin;

      for(int i = 0; i < kontoHamper.VvNumOfRows; i++)
      {
         kontoHamper.VvRowHgt[i]    = ZXC.QUN;
         kontoHamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      kontoHamper.VvBottomMargin = kontoHamper.VvTopMargin;

      lbl_klasa    = kontoHamper.CreateVvLabel(0, 0, "Klasa:"   , ContentAlignment.MiddleRight);
      lbl_subKlasa = kontoHamper.CreateVvLabel(0, 1, "SubKlasa:", ContentAlignment.MiddleRight);

      tbx_klasa         = kontoHamper.CreateVvTextBox(1, 0, "tbx_klasa    ", "Klasa"    );
      tbx_klasaNaziv    = kontoHamper.CreateVvTextBox(2, 0, "tbx_klasaOpis", "KlasaOpis", 128, 2, 0);
      tbx_subKlasa      = kontoHamper.CreateVvTextBox(1, 1, "tbx_klasa    ", "Klasa"    );
      tbx_subKlasaNaziv = kontoHamper.CreateVvTextBox(2, 1, "tbx_klasaOpis", "KlasaOpis", 128, 2, 0);

      tbx_klasa        .JAM_ReadOnly = 
      tbx_klasaNaziv   .JAM_ReadOnly = 
      tbx_subKlasa     .JAM_ReadOnly = 
      tbx_subKlasaNaziv.JAM_ReadOnly = true;


                  kontoHamper.CreateVvLabel  (0, 2, "Konto:"   , ContentAlignment.MiddleRight);
      tbx_konto = kontoHamper.CreateVvTextBox(1, 2, "tbx_konto", "Broj konta u kontnom planu", GetDB_ColumnSize(DB_ci.konto));
      tbx_konto.JAM_DataRequired = true;
      tbx_konto.JAM_CharEdits    = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_konto.JAM_Highlighted = true;
      
                kontoHamper.CreateVvLabel(3, 2, "Tip:", ContentAlignment.MiddleRight);
      tbx_tip = kontoHamper.CreateVvTextBox(4, 2, "tbx_tip", "A - analiticki, S - sinteticki, P - privatni, Z - sinteticki(privatni)", GetDB_ColumnSize(DB_ci.tip));
      tbx_tip.JAM_CharacterCasing = CharacterCasing.Upper;

                  kontoHamper.CreateVvLabel  (0, 3, "Naziv J1:"  , ContentAlignment.MiddleRight);
      tbx_naziv = kontoHamper.CreateVvTextBox(1, 3, "tbx_naziv", "Naziv konta u kontnom planu - Jezik1", GetDB_ColumnSize(DB_ci.naziv), 3, 0);
      tbx_naziv.JAM_Highlighted = true;

                 kontoHamper.CreateVvLabel  (0, 4, "Opis:"   , ContentAlignment.MiddleRight);
      tbx_opis = kontoHamper.CreateVvTextBox(1, 4, "tbx_opis", "Opis konta u kontnom planu", GetDB_ColumnSize(DB_ci.opis), 3, 0);

                  kontoHamper.CreateVvLabel  (0, 5, "AnalitGr:"    , ContentAlignment.MiddleRight);
      tbx_anaGr = kontoHamper.CreateVvTextBox(1, 5, "tbx_anaGr", "Analitičke grupe", GetDB_ColumnSize(DB_ci.anaGr), 1, 0);
      tbx_anaGr.JAM_CharacterCasing = CharacterCasing.Upper;

                   kontoHamper.CreateVvLabel  (0, 6, "Naziv J2:"  , ContentAlignment.MiddleRight);
      tbx_naziv2 = kontoHamper.CreateVvTextBox(1, 6, "tbx_naziv2", "Naziv konta u kontnom planu - Jezik2", GetDB_ColumnSize(DB_ci.naziv), 3, 0);

                   kontoHamper.CreateVvLabel  (0, 7, "Naziv J3:"  , ContentAlignment.MiddleRight);
      tbx_naziv3 = kontoHamper.CreateVvTextBox(1, 7, "tbx_naziv3", "Naziv konta u kontnom planu - Jezik3", GetDB_ColumnSize(DB_ci.naziv), 3, 0);

                     kontoHamper.CreateVvLabel        (0, 8, "Fond:", ContentAlignment.MiddleRight);
      tbx_fond     = kontoHamper.CreateVvTextBoxLookUp(1, 8, "tbx_fond"    , "Financijski Fond"/*, GetDB_ColumnSize(DB_ci.fond)*/);
      tbx_fondOpis = kontoHamper.CreateVvTextBox      (2, 8, "tbx_opisFond", "Naziv Fonda",248, 2, 0);
      
      tbx_fond.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_fond.JAM_Set_LookUpTable(ZXC.luiListaFinFond, (int)ZXC.Kolona.prva);
      tbx_fond.JAM_lui_NameTaker_JAM_Name = tbx_fondOpis.JAM_Name;
      tbx_fondOpis.JAM_ReadOnly = true;

      this.ControlForInitialFocus = tbx_konto;
      tbx_konto.JAM_StatusText    = "Broj konta u kontnom planu"; // mora jer tek liniju prije taj tbx dobije focus
      
   }

   private void InitializeHamperPsRule(out VvHamper hamp_psRule)
   {
      CreateHamperOpen();

      hamp_psRule = new VvHamper(1, 4, "", TheTabControl.TabPages[0], true);

      hamp_psRule.VvColWdt      = new int[] { ZXC.Q9un };
      hamp_psRule.VvSpcBefCol   = new int[] { ZXC.QunMrgn };
      hamp_psRule.VvRightMargin = hamp_psRule.VvLeftMargin;

      hamp_psRule.VvRowHgt       = new int[] {     ZXC.QUN, ZXC.QUN , ZXC.QUN , ZXC.QUN  };
      hamp_psRule.VvSpcBefRow    = new int[] { ZXC.QunMrgn, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamp_psRule.VvBottomMargin = hamp_psRule.VvTopMargin;

      Label lbl = hamp_psRule.CreateVvLabel(0, 0, "(Računanje salda po računima)", ContentAlignment.MiddleLeft);

      rbt_noOverr = hamp_psRule.CreateVvRadioButton(0, 1, null, "Standardno", TextImageRelation.ImageBeforeText);
      rbt_noOverr.Tag = true; // kod RadioButtona na Hamperu, Tag odgovara na pitanje IsDisDifoltSelektedRejdioButon 
      rbt_supress = hamp_psRule.CreateVvRadioButton(0, 2, null, "Zabrani"   , TextImageRelation.ImageBeforeText);
      rbt_force   = hamp_psRule.CreateVvRadioButton(0, 3, null, "Prislili"  , TextImageRelation.ImageBeforeText);

      rbt_noOverr.Checked = true;
      hamp_psRule.Visible = false;
      hamp_psRule.Location = new Point(hamp_Open.Left, hamp_Open.Top); 
   }

   private void CreateHamperOpen()
   {
      hamp_Open          = new VvHamper(2, 1, "", TheTabControl.TabPages[0], false);
      hamp_Open.Location = new Point(hamp_konto.Right + ZXC.QunMrgn * 2, nextY);

      hamp_Open.VvColWdt      = new int[] { ZXC.QUN, ZXC.Q4un};
      hamp_Open.VvSpcBefCol   = new int[] { 0, ZXC.Qun4 };
      hamp_Open.VvRightMargin = hamp_Open.VvLeftMargin;

      hamp_Open.VvRowHgt       = new int[] { ZXC.QUN };
      hamp_Open.VvSpcBefRow    = new int[] { 0 };
      hamp_Open.VvBottomMargin = hamp_Open.VvTopMargin;

      btn_PlusMinus     = hamp_Open.CreateVvButton(0, 0, new EventHandler(btn_PlusMinus_Click), "+");
      btn_PlusMinus.Tag = "PlusMinus";
      Label lblNaslov = hamp_Open.CreateVvLabel(1, 0, "PS Pravilo", ContentAlignment.MiddleLeft);

      lblCrta           = new Label();
      lblCrta.Parent    = TheTabControl.TabPages[0];
      lblCrta.Location  = new Point(hamp_Open.Right, hamp_Open.Top + ZXC.Qun4 + ZXC.Qun5);
      lblCrta.Size      = new Size(ZXC.Q4un, 1);
      lblCrta.BackColor = Color.LightGray;
   }

   private void btn_PlusMinus_Click(object sender, System.EventArgs e)
   {
      Button btn = sender as Button;

      if (btn.Text == "+")
      {
         btn.Text = "-";
         hamp_psRule.Visible = true;
         lblCrta.Visible     = false;
      }
      else
      {
         btn.Text = "+";
         hamp_psRule.Visible = false;
         lblCrta.Visible     = true;
      }
   }

   #endregion  HAMPER 
 
   #region DataGridView
 
   private void CreateDataGridView_InitializeTheGrid_ReadOnly_Columns()
   {
      aTransesGrid[0]      = CreateDataGridView_ReadOnly(TheTabControl.TabPages[ftrans_TabPageName], "Ftrans");
      aTransesGrid[0].Dock = DockStyle.Fill;
      int minGridWIdth     = InitializeTheGrid_ReadOnly_Columns(); // ovaj int bas i ne treba vise ali.....

      //GridOnSIfrar_SizeAnchorAndScroll(aTransesGrid[0], minGridWIdth); maknuto ybog dockFill

      aTransesGrid[0].DoubleClick += new EventHandler(theFIRST_TransGrid_DoubleClick);
      aTransesGrid[0].KeyPress    += new KeyPressEventHandler(theFIRST_TransGrid_KeyPress);

   }

   protected override void theFIRST_TransGrid_DoubleClick(object sender, EventArgs e)
   {
      base.OpenNew_Record_TabPage_OnDoubleClick(ZXC.VvSubModulEnum.NAL_F, SelectedRecIDIn_FIRST_TransGrid);
   }

   private int InitializeTheGrid_ReadOnly_Columns()
   {
      int sumOfColWidth = 0, colWidth;
      int colDateWidth  = ZXC.Q4un + ZXC.Qun4;
      int colSif6Width  = ZXC.Q3un + ZXC.Qun8;

      sumOfColWidth += aTransesGrid[0].RowHeadersWidth;
      colWidth = colSif6Width         ;                            AddDGVColum_RecID_4GridReadOnly   (aTransesGrid[0], "RecID"     , colWidth, false, 0);
      colWidth = colDateWidth         ; sumOfColWidth += colWidth; AddDGVColum_DateTime_4GridReadOnly(aTransesGrid[0], "Datum"     , colWidth);
      colWidth = colSif6Width         ; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (aTransesGrid[0], "Nalog"     , colWidth, true, 6);
      colWidth = ZXC.Q2un + ZXC.Qun4  ; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (aTransesGrid[0], "Red"       , colWidth, false, 0);
      colWidth = ZXC.Q2un             ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (aTransesGrid[0], "TT"        , colWidth, false);
      colWidth = ZXC.Q6un             ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (aTransesGrid[0], "Opis"      , colWidth, true);
      colWidth = colSif6Width         ; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (aTransesGrid[0], "Šifra"     , colWidth, true, 6);
      colWidth = colSif6Width+ZXC.Qun8; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(aTransesGrid[0], "Ticker", colWidth, false);
      colWidth = ZXC.Q4un             ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (aTransesGrid[0], "VezniDOk"  , colWidth, false);
      colWidth = colDateWidth         ; sumOfColWidth += colWidth; AddDGVColum_DateTime_4GridReadOnly(aTransesGrid[0], "Valuta"    , colWidth);
      colWidth = ZXC.Q6un             ; sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[0], "Duguje"    , colWidth, 2);
      colWidth = ZXC.Q6un             ; sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[0], "Potražuje" , colWidth, 2);

      return sumOfColWidth;
   }

   #endregion DataGridView

   #region Filter

   public override void CreateRptFilterAndRptFilterUC()
   {
      TheFtransFilter = new VvRpt_Fin_Filter();

      TheFinFilterUC          = new FinFilterUC(this);
      TheFinFilterUC.Parent   = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheFinFilterUC.Width;
   }

   #endregion Filter
   
   #region PutFields(), GetFields()

   #region Fld_

   public string Fld_Konto         { get { return tbx_konto        .Text; } set { tbx_konto        .Text = value; } }
   public string Fld_Tip           { get { return tbx_tip          .Text; } set { tbx_tip          .Text = value; } }
   public string Fld_Naziv         { get { return tbx_naziv        .Text; } set { tbx_naziv        .Text = value; } }
   public string Fld_Opis          { get { return tbx_opis         .Text; } set { tbx_opis         .Text = value; } }
   public string Fld_AnaGr         { get { return tbx_anaGr        .Text; } set { tbx_anaGr        .Text = value; } }
   public string Fld_Naziv2        { get { return tbx_naziv2       .Text; } set { tbx_naziv2       .Text = value; } }
   public string Fld_Naziv3        { get { return tbx_naziv3       .Text; } set { tbx_naziv3       .Text = value; } }
   public string Fld_Klasa         { get { return tbx_klasa        .Text; } set { tbx_klasa        .Text = value; } }
   public string Fld_SubKlasa      { get { return tbx_subKlasa     .Text; } set { tbx_subKlasa     .Text = value; } }
   public string Fld_KlasaNaziv    { get { return tbx_klasaNaziv   .Text; } set { tbx_klasaNaziv   .Text = value; } }
   public string Fld_SubKlasaNaziv { get { return tbx_subKlasaNaziv.Text; } set { tbx_subKlasaNaziv.Text = value; } }
   public string Fld_Fond          { get { return tbx_fond         .Text; } set { tbx_fond         .Text = value; } }
   public string Fld_FondOpis      {                                        set { tbx_fondOpis     .Text = value; } }

   public Kplan.PsRuleEnum Fld_PsRule
   {
      get
      {
              if(rbt_noOverr.Checked) return Kplan.PsRuleEnum.NO_OVERRIDE;
         else if(rbt_supress.Checked) return Kplan.PsRuleEnum.SUPRESS_SaldaKontiKTO;
         else if(rbt_force  .Checked) return Kplan.PsRuleEnum.FORCE_SaldaKontiKTO;

         else throw new Exception("Fld_PsRule: who df is checked?");
      }
      set
      {
         rbt_noOverr.Checked =
         rbt_supress.Checked =
         rbt_force  .Checked = false;

         switch(value)
         {
            case Kplan.PsRuleEnum.NO_OVERRIDE:     rbt_noOverr.Checked = true; break;
            case Kplan.PsRuleEnum.SUPRESS_SaldaKontiKTO: rbt_supress.Checked = true; break;
            case Kplan.PsRuleEnum.FORCE_SaldaKontiKTO:   rbt_force  .Checked = true; break;
         }
      }
   }

   #endregion Fld_XY

   #region Classic PutFileds(), GetFields()

   public override void PutFields(VvDataRecord kplan)
   {
      kplan_rec = (Kplan)kplan;

      if(kplan_rec != null)
      {
         PutMetaFileds(kplan_rec.AddUID, kplan_rec.AddTS, kplan_rec.ModUID, kplan_rec.ModTS, kplan_rec.RecID, kplan_rec.LanSrvID, kplan_rec.LanRecID);
         PutIdentityFields(kplan_rec.Konto, kplan_rec.Tip, kplan_rec.Naziv, "");

         Fld_Konto  = kplan_rec.Konto ;
         Fld_Tip    = kplan_rec.Tip   ;
         Fld_Naziv  = kplan_rec.Naziv ;
         Fld_PsRule = kplan_rec.PsRule;
         Fld_Opis   = kplan_rec.Opis  ;
         Fld_AnaGr  = kplan_rec.AnaGr ;
         Fld_Naziv2 = kplan_rec.Naziv2;
         Fld_Naziv3 = kplan_rec.Naziv3;
         Fld_Fond   = kplan_rec.Fond  ;

         Fld_FondOpis = ZXC.luiListaFinFond.GetNameForThisCd(kplan_rec.Fond);

         InitializeFilterUCFields();

         recordReportLoaded = false;
         DecideIfShouldLoad_VvReport(null, null, null);

         aTransesLoaded[0] = false; // ovdje treba nulirati sve postojece 'xyLoaded' varijable
         DecideIfShouldLoad_TransDGV(null, null, null);

         ZXC.luiListaKplanKlase.LazyLoad();
         if(ZXC.luiListaKplanKlase.Count.NotZero() && kplan_rec.Konto.NotEmpty())
         {
            Fld_Klasa         = kplan_rec.Konto.Substring(0, 1);
            Fld_SubKlasa      = kplan_rec.Konto.SubstringSafe(0, 2);
            Fld_KlasaNaziv    = ZXC.luiListaKplanKlase.GetNameForThisCd(kplan_rec.Konto.SubstringSafe(0, 1));
            Fld_SubKlasaNaziv = ZXC.luiListaKplanKlase.GetNameForThisCd(kplan_rec.Konto.SubstringSafe(0, 2));
         }
      }
   }

   public override void GetFields(bool fuse)
   {
      if(kplan_rec == null) kplan_rec = new Kplan();

      kplan_rec.Konto  = Fld_Konto;
      kplan_rec.Tip    = Fld_Tip;
      kplan_rec.Naziv  = Fld_Naziv;
      kplan_rec.PsRule = Fld_PsRule;
      kplan_rec.AnaGr  = Fld_AnaGr;
      kplan_rec.Opis   = Fld_Opis;
      kplan_rec.Naziv2 = Fld_Naziv2;
      kplan_rec.Naziv3 = Fld_Naziv3;
      kplan_rec.Fond   = Fld_Fond  ;

   }

   #endregion Classic PutFileds(), GetFields()

   #region Put Trans DGV Fileds

   private const string ftrans_TabPageName = "Fin Trans";

   // Tu dolazimo na 1 nacin: 1. Classic PutFields 
   private void InitializeFilterUCFields()
   {
      SetFilterRecordDependentDefaults(); // filter.KontoOd i Do = kplan_rec.Konto (punimo bussiness od filtera, ne UC)
      TheFinFilterUC.PutFilterFields(TheFtransFilter);
   }

   // Tu dolazimo na 2 nacina:          
   // 1. Classic PutFields              
   // 2. TheTabControl.SelectionChanged 
   public override void DecideIfShouldLoad_TransDGV(VvInnerTabControl sender, VvInnerTabPage oldPage, VvInnerTabPage newPage)
   {
      ZXC.VvInnerTabPageKindEnum innerTabPageKind = ((VvInnerTabPage)TheTabControl.SelectedTab).TheInnerTabPageKindEnum;

      if(aTransesLoaded[0] == false &&
         innerTabPageKind  == ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage)
      {
         LoadRecordList_AND_PutTransDgvFields();
      }

   }

   // Tu dolazimo na 2 nacina:          
   // 1. ButtonIzlistaj_Click (sa FilterUC-a) 
   // 2. DecideIfShouldLoad_TransDGV    
   public override void LoadRecordList_AND_PutTransDgvFields()
   {
      int rowIdx, idxCorrector;

      TheFinFilterUC.GetFilterFields();
      TheFinFilterUC.AddFilterMemberz(TheFtransFilter, null);

      if(kplan_rec.Ftranses == null) kplan_rec.Ftranses = new List<Ftrans>();
      else                           kplan_rec.Ftranses.Clear();

      string orderByColumns;
      if(TheFinFilterUC.Fld_Sort == VvSQL.SorterType.DokNum)
         orderByColumns = "                t_dokNum DESC, t_serial DESC";
      else
         orderByColumns = "t_dokDate DESC, t_dokNum DESC, t_serial DESC";

      VvDaoBase.LoadGenericVvDataRecordList<Ftrans>(TheDbConnection, kplan_rec.Ftranses, TheFtransFilter.FilterMembers, orderByColumns);

      aTransesLoaded[0] = true;

      aTransesGrid[0].Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(aTransesGrid[0]);

      foreach(Ftrans ftrans_rec in kplan_rec.Ftranses)
      {
         aTransesGrid[0].Rows.Add();

         rowIdx = aTransesGrid[0].RowCount - idxCorrector;

         aTransesGrid[0][0,  rowIdx].Value = ftrans_rec.T_parentID;
         aTransesGrid[0][1,  rowIdx].Value = ftrans_rec.T_dokDate;
         aTransesGrid[0][2,  rowIdx].Value = ftrans_rec.T_dokNum;
         aTransesGrid[0][3,  rowIdx].Value = ftrans_rec.T_serial;
         aTransesGrid[0][4,  rowIdx].Value = ftrans_rec.T_TT;
         aTransesGrid[0][5,  rowIdx].Value = ftrans_rec.T_opis;
         aTransesGrid[0][6,  rowIdx].Value = ftrans_rec.T_kupdob_cd;
         aTransesGrid[0][7,  rowIdx].Value = ftrans_rec.T_ticker;
         aTransesGrid[0][8,  rowIdx].Value = ftrans_rec.T_tipBr;
         aTransesGrid[0][9,  rowIdx].Value = ftrans_rec.T_valuta;
         aTransesGrid[0][10, rowIdx].Value = ftrans_rec.T_dug;
         aTransesGrid[0][11, rowIdx].Value = ftrans_rec.T_pot;

         aTransesGrid[0].Rows[rowIdx].HeaderCell.Value = (kplan_rec.Ftranses.Count - rowIdx).ToString();
      }

      //VvDocumentRecordUC.RenumerateLineNumbers(gridFtrans, 0);

   }

   #endregion Put Trans DGV Fileds

   #endregion PutFields(), GetFields()

   #region Overriders And Specifics

   #region VvDataRecord/VvDaoBase

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.kplan_rec; }
      set {        this.kplan_rec = (Kplan)value; }
   }

   public override VvSifrarRecord VirtualSifrarRecord
   {
      get { return this.VirtualDataRecord as VvSifrarRecord; }
      set {        this.VirtualDataRecord = (Kplan)value;    }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.KplanDao; }
   }

   #endregion VvDataRecord/VvDaoBase

   #region VvFindDialog

   public override VvFindDialog CreateVvFindDialog()
   {
      return CreateFind_Kplan_Dialog();
   }

   public static VvFindDialog CreateFind_Kplan_Dialog()
   {
      VvSubModul vvSubModul   = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsKPL);
      VvDataRecord      vvDataRecord = ZXC.TheVvForm.CreateTheVvDataRecord_SwitchSubModulEnum(vvSubModul);

      VvFindDialog vvFindDialog = new VvFindDialog();
      VvRecLstUC   vvRecListUC  = new KplanListUC(vvFindDialog, (Kplan)vvDataRecord, vvSubModul);

      vvFindDialog.TheRecListUC = vvRecListUC;
      
      return vvFindDialog;
   }

   #endregion VvFindDialog

   #region PrintSifrarRecord

   public VvRpt_Fin_Filter TheFtransFilter { get; set; }

   public FinFilterUC TheFinFilterUC { get; set; }

   //public RptF_KKonta TheRptF_KKonta { get; set; }

   //public override VvReport VirtualReport
   //{
   //   get
   //   {
   //      return this.TheRptF_KKonta;
   //   }
   //}

   public override string VirtualReportName
   {
      get
      {
         return "KARTICA KONTA";
      }
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      return new RptF_KKonta(reportName, (VvRpt_Fin_Filter)vvRptFilter);
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheFtransFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheFinFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
      this.TheFtransFilter.KontoOd = kplan_rec.Konto;
      this.TheFtransFilter.KontoDo = kplan_rec.Konto;
   }

   #endregion PrintSifrarRecord

   #region Update_VvDataRecord (Legacy naming convention)

   /// <summary>
   /// 'FindVvDataRecord' procedura. Inicirana:
   /// 1. Context menu (Mouse right click)
   /// 2. Mouse click (Ctrl ili Alt click)
   /// 3. Keyboard initiated (Ctrl/Alt + F/Space)
   /// </summary>
   /// <param name="startValue"></param>
   /// <returns></returns>
   public static string Update_Kplan(object startValue)
   {
      Kplan           kplan_rec = new Kplan();
      KplanListUC     kplanListUC;
      XSqlConnection dbConnection = ZXC.TheVvForm.TheDbConnection;

      VvFindDialog dlg = CreateFind_Kplan_Dialog();

      kplanListUC = (KplanListUC)(dlg.TheRecListUC);

      kplanListUC.Fld_FromKonto = startValue.ToString();

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         if(!ZXC.KplanDao.SetMe_Record_byRecID(dbConnection, kplan_rec, (uint)dlg.SelectedRecID, false/*(uint)dlg.SelectedRecIDIn_FIRST_TransGrid*/)) return null;
      }
      else
      {
         kplan_rec = null;
      }

      if(dlg.SelectionIsNewlyAddedRecord == true) ZXC.ShouldForceSifrarRefreshing = true;

      dlg.Dispose();

      if(kplan_rec != null) return kplan_rec.Konto;
      else                  return null;
   }

   #endregion Update_VvDataRecord (Legacy naming convention)

   public override Size ThisUcSize 
   {
      get
      {
         return new Size(hamp_psRule.Right + ZXC.QunMrgn, hamp_psRule.Bottom + ZXC.QunMrgn);
      }
   }

   #region PutNew_Sifra_Field

   public override void PutNew_Sifra_Field(uint newSifra)
   {
      // dummy for KplanUC 
   }

   #endregion PutNew_Sifra_Field

   #endregion Overriders And Specifics

}

public class VvRenameKontoDlg : VvDialog
{
   #region Filedz
    
   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_newKonto;

   #endregion Filedz

   #region Constructor

   public VvRenameKontoDlg()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Preimenuj konto";

      CreateHamper();

      dlgWidth        = hamper.Right  + ZXC.QunMrgn;
      dlgHeight       = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_newKonto, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
   }

   #endregion Constructor

   #region hamper

   private void CreateHamper()
   {
      hamper          = new VvHamper(2, 1, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      int columnSize = (int)ZXC.KplanDao.GetSchemaColumnSize(ZXC.KplanDao.CI.konto);

      Label lbl    = hamper.CreateVvLabel  (0, 0, "Novi konto:", ContentAlignment.MiddleRight);
      tbx_newKonto = hamper.CreateVvTextBox(1, 0, "tbx_newKonto", "",  columnSize);
      tbx_newKonto.JAM_CharEdits    = ZXC.JAM_CharEdits.DigitsOnly;
   }

   #endregion hamper

   #region Button_Click

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Button_Click

   #region Fld_ 
   
   public string Fld_NewKonto
   {
      get { return tbx_newKonto.Text;         }
      set {        tbx_newKonto.Text = value; }
   }

   #endregion Fld_

}


