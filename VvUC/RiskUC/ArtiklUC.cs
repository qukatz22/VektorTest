using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Linq;
using System.ServiceModel;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
using System.Collections.Generic;
//tam
using Vektor.Reports.PIZ;
using System.ComponentModel;
#endif

public class ArtiklUC : VvSifrarRecordUC
{
   #region Fieldz

   private VvTextBox tbx_artiklCD, /*tbx_artiklName,*/ tbx_barCode1, tbx_skladCD, tbx_skladOpis, tbx_grupa1CD, tbx_grupa1Opis, tbx_jedMj,
                     tbx_ts, tbx_tsOpis, tbx_konto, tbx_longOpis, tbx_pdvKat, tbx_pdvKatOpis,
                     tbx_artiklCD2, tbx_artiklName2,
                     tbx_barCode2, tbx_serNo, tbx_grupa2CD, tbx_grupa2Opis, tbx_grupa3CD, tbx_grupa3Opis,
                     tbx_placement, tbx_linkArtCD, tbx_linkArtName, tbx_dateProizv, tbx_prefValName, tbx_orgPak,
                     tbx_masaNetto, tbx_masaBruto, tbx_promjer, tbx_povrsina, tbx_zapremina, tbx_duljina, tbx_sirina,
                     tbx_visina, tbx_starost, tbx_boja, tbx_spol,
                     tbx_psKolSt, tbx_psNabCij, tbx_psFinSt,
                     tbx_kolSt, tbx_prNabCij, tbx_prNabCij2, tbx_prNabCijU, tbx_finSt, tbx_SklKol,
                     tbx_cjenikVpc1, tbx_VPC2, tbx_MPC1, tbx_devC, tbx_zaSkladCD, tbx_zaSkladOpis,
                     tbx_ulMinCij, tbx_ulMaxCij, tbx_ulLastCij, tbx_ruc, tbx_rabat1, tbx_rabat2, tbx_minKol,
                     tbx_invKolSt, tbx_invFinSt, tbx_ulazKol, tbx_ulazFin, tbx_izlazKol, tbx_izlazFinNab, tbx_izlazFinNab2, tbx_izlazFinProd, tbx_izlazRUV, tbx_izlRezervKol, tbx_KolStFree,
                     /*tbx_prProdCij,*/ tbx_ruvPost,
                     tbx_velicina, tbx_garRok, tbx_drzPorjekla, tbx_atestBr, tbx_atestDate, tbx_url,
                     tbx_dobCd, tbx_dobNaziv, tbx_dobTick, tbx_proizvCd, tbx_proizvNaziv, tbx_proizvTick, tbx_marza,
                     tbx_masaNettoJM, tbx_masaBrutoJM, tbx_promjerJM, tbx_povrsinaJM, tbx_zapreminaJM, tbx_duljinaJM, tbx_sirinaJM, tbx_visinaJM,
                     tbx_naDan,
                     tbx_uKolFisycal, tbx_iKolFisycal, tbx_izlazCijMin, tbx_izlazCijMax, tbx_izlazCijLast, tbx_dateZadUlaz, tbx_dateZadIzlaz, tbx_dateZadInv, tbx_datePS, tbx_dateZadnji,
                     tbx_carTarifa, tbx_partNo, tbx_napomena, tbx_HelpNabC, tbx_importCij,
                     tbx_frsMinTT, tbx_frsMinTTnum,
                     tbx_snaga, tbx_snagaJM, tbx_emisCo2, tbx_euroNorma, tbx_prNabCijOP; 
   private Label     lbl_frsMin;
   public VvTextBox  tbx_artiklName;
   private VvDateTimePicker dtp_dateProizv, dtp_atestDate, dtp_naDan, dtp_dateZadUlaz, dtp_dateZadIzlaz, dtp_dateZadInv, dtp_datePS, dtp_dateZadnji;

   private CheckBox cbx_isRashod, cbx_isAkcija, cbx_isMaster, cbx_isDozvMinus, cbx_isSerBr, cbx_isPrnOpis;

   private VvHamper hamp_naziv, hamp_result, hamp_zaSklad, hamp_VPC,
                    hamp_artiklCD2,
                    hamp_barCode2, hamp_serNo, hamp_grupa2CD, hamp_grupa3CD,
                    hamp_placement, hamp_linkArtCD, hamp_dateProizv,   hamp_prefValName, hamp_orgPak,
                    hamp_masaNetto,  hamp_masaBruto, hamp_promjer, hamp_povrsina, hamp_zapremina, hamp_duljina, hamp_sirina,
                    hamp_visina, hamp_starost, hamp_boja, hamp_spol, hamp_isAkcija, hamp_isMaster,
                    hamp_VPC2, hamp_MPC1, hamp_devC, hamp_rabat1, hamp_rabat2, hamp_minKol, hamp_marza,
                    hamp_velicina, hamp_garRok, hamp_drzPorjekla, hamp_atestBr, hamp_atestDate, hamp_url,
                    hamp_dobavljac, hamp_proizvodjac, hamp_isDozvMinus, hamp_isSerBr, hamp_printOpis, hamp_carTarifa, hamp_partNo, hamp_napomena, hamp_importCij,
                    hamp_snaga, hamp_co2, hamp_eun, hamp_OP;

   private RadioButton rbt_vpc1, rbt_marza;

   private Panel  panel_result, panel_MigratorsLeftB, panel_MigratorsLeftA, panel_MigratorsRightA,panel_MigratorsRightB;

   private int nextX = 0, nextY = 0, razmakHamp = 0;

   public Artikl  artikl_rec;

   private ArtiklDao.ArtiklCI DB_ci
   {
      get { return ZXC.ArtCI; }
   }

   private int colDateWidth = ZXC.Q4un + ZXC.Qun4;
   private int colSif6Width = ZXC.Q3un + ZXC.Qun8;

   int drugakolona = ZXC.Q2un - ZXC.Qun2;
   int labelWidth  = ZXC.Q3un;
   Color backColorBmigrator, backColorSklad, backColorResult, foreColorMostResult, foreColorRUVResult, foreColorKolSklResult;
   
   private Button btn_numCd, btn_proj, btn_openExLinkURL, btn_openExLinkNap;

   internal PCK_ArtiklList_UC pcKInfoUC;

   protected bool PTG_PCKinfoLoaded;

   #endregion Fieldz

   #region Constructor

   public ArtiklUC(Control parent, Artikl _artikl, VvForm.VvSubModul vvSubModul)
   {
      artikl_rec = _artikl;

      ThePrefferedRecordSorter = VirtualDataRecord.DefaultSorter;

      this.TheSubModul = vvSubModul;

      SuspendLayout();

      SetColorsOfResultTbx();

      CreateTabPages(parent);

      CreateMigratingHampers_Location();

      CreateHamperTabPage_Osnovno();

      #region Initialize and PutMigratorsStates

      ZXC.TheVvForm.PutFieldsInProgress = true;

      PutMigratorsStates(MigratorRightParentA);
      PutMigratorsStates(MigratorRightParentB);
      
      ZXC.TheVvForm.PutFieldsInProgress = false;

      #endregion Initialize and PutMigratorsStates

      InitializeVvUserControl(parent);

      CreateDataGridView_InitializeTheGrid_ReadOnly_Columns();

      ResumeLayout();

      // 09.11.2023: 
      this.Validating += new CancelEventHandler(ArtiklUC_Validating);

      //string oldName = "Nb HP EliteBook 8470p, i5, 4GB RAM, 120 GB SSD HDD, DVD-RW, 14\", Ms Win7pro (upgrade to";
      //string newName = ZXC.ModifyPCK_ArtiklName(oldName, 100M, 200M);
      //ZXC.aim_emsg(oldName + "\n\r" + newName);
   }

   void ArtiklUC_Validating(object sender, CancelEventArgs e)
   {
      #region Should validate enivej?

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None ||
         TheVvTabPage.WriteMode == ZXC.WriteMode.Delete ||
         this.Visible == false) return;

      //DateTime serverNow = VvSQL.GetServer_DateTime_Now(TheDbConnection);

      #endregion Should validate enivej?

      //if(Fld_Grupa1CD.IsEmpty())
      //{
      //   ZXC.aim_emsg(MessageBoxIcon.Error, "Polje 'Grupa A' ne moze biti prazno!");
      //   e.Cancel = true;
      //}

      // 09.11.2023: 
      if(ZXC.IsPCTOGO && Fld_Ts == ZXC.PCK_TS)
      {
         if(Fld_Zapremina.IsZero ()) { ZXC.aim_emsg(MessageBoxIcon./*Stop*/Warning, "Za PCK artikl morate zadati RAM kapacitet."  ); /*e.Cancel = true;*/}
         if(Fld_Duljina  .IsZero ()) { ZXC.aim_emsg(MessageBoxIcon./*Stop*/Warning, "Za PCK artikl morate zadati HDD kapacitet."  ); /*e.Cancel = true;*/}
         if(Fld_CarTarifa.IsEmpty()) { ZXC.aim_emsg(MessageBoxIcon.Stop           , "Za PCK artikl morate zadati PCK baznu šifru."); e.Cancel = true; }
      }

   }

   private void SetToolTipsForPredugackys()
   {
      tbx_artiklName2.TextAsToolTip(toolTip);
   }

   private void SetColorsOfResultTbx()
   {
      backColorBmigrator    = Color.LavenderBlush;
      backColorSklad        = Color.Lavender;
      backColorResult       = Color.Thistle;
      foreColorMostResult   = Color.Indigo;
      foreColorRUVResult    = Color.DarkBlue;
      foreColorKolSklResult = Color.LightSkyBlue;
   }

   #endregion Constructor

   #region TabPages

   private void CreateTabPages(Control _parent)
   {
      TheTabControl.TabPages.Add(CreateVvInnerTabPages("Osnovno"  , "", ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
      TheTabControl.TabPages.Add(CreateVvInnerTabPages("Prošireno", "", ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));

      VvTabPage vvTabPage = (VvTabPage)(_parent.Parent);

      // ovaj if sluzi kad se kartica dodaje i Find-a da se ne pojave transevi !!!!
      if(vvTabPage.TabPageKind != ZXC.VvTabPageKindEnum.RECORD_TabPage_INTERACTIVE)
      {
         TheTabControl.TabPages.Add(CreateVvInnerTabPages(rtrans_TabPageName, "", ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage));
      }

      if(ZXC.IsPCTOGO)
      { 
         TheTabControl.TabPages.Add(CreateVvInnerTabPages(pckInfo_TabPageName, pckInfo_TabPageName, ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage));
               
         pcKInfoUC = new PCK_ArtiklList_UC(TheTabControl.TabPages[pckInfo_TabPageName], artikl_rec.ArtiklCD, /*Fld_ZaSkladCD*/"");

         TheTabControl.SelectionChanged += DecideIfShouldLoad_PCKinfo;
      }
   }

   #endregion TabPages

   #region  HAMPER

   public void CreateHamperTabPage_Osnovno()
   {
      nextX = 0;
      nextY = 0;

      if(ZXC.IsPCTOGO)
      {
         InitializeNazivHamper_PTG(out hamp_naziv);
      }
      else
      { 
         InitializeNazivHamper(out hamp_naziv);
      }
   
      nextY = hamp_naziv.Bottom + razmakHamp;

      CreateResultPanel();

      CreatePanels4MigratorHampers_LocationOfPanels();
      
   }

   private void InitializeNazivHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(11, 5, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      //                                       0                 1                     2                3                  4                                 5                                        6              7             8                       9  
      hamper.VvColWdt      = new int[] { labelWidth, ZXC.Q3un - ZXC.Qun2, ZXC.Q8un + ZXC.Qun4,ZXC.QUN - ZXC.Qun4, labelWidth-(ZXC.QUN )+ ZXC.Qun2, ZXC.Q3un - ZXC.Qun2, labelWidth,  ZXC.QUN + ZXC.Qun2+ ZXC.Qun4, ZXC.Q3un, labelWidth, ZXC.Q10un + ZXC.QUN };
      hamper.VvSpcBefCol   = new int[] {   ZXC.Qun4,            ZXC.Qun4,            ZXC.Qun4,                 0,   0,            ZXC.Qun4,   ZXC.Qun4,                      ZXC.Qun4, ZXC.Qun4,   ZXC.Qun4,             ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      } 
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl_artiklCD, lbl_artiklName, lbl_barCode1, lbl_skladCD, lbl_grupa1CD, lbl_jedMj,
            lbl_ts, lbl_konto, lbl_longOpis, lbl_pdvKat, lbl_polCij;
      
      
      lbl_artiklName = hamper.CreateVvLabel  (0, 0, "Naziv:", ContentAlignment.MiddleRight);
      tbx_artiklName = hamper.CreateVvTextBox(1, 0, "tbx_artiklName", "Naziv artikla", GetDB_ColumnSize(DB_ci.artiklName), 7, 0);
      tbx_artiklName.JAM_Highlighted = true;
      tbx_artiklName.JAM_DataRequired = true;
      
      lbl_artiklCD = hamper.CreateVvLabel  (0, 1, "Šifra:", ContentAlignment.MiddleRight);
      tbx_artiklCD = hamper.CreateVvTextBox(1, 1, "tbx_artiklCD", "Šifra artikla", GetDB_ColumnSize(DB_ci.artiklCD), 1, 0);
      tbx_artiklCD.JAM_Highlighted = true;
      tbx_artiklCD.JAM_DataRequired = true;

      btn_numCd = hamper.CreateVvButton(3, 1, new EventHandler(GetNextSifraWroot_String_btnClick), "");
      btn_numCd.Name = "btn_goExLinkNewNum";
      btn_numCd.FlatStyle = FlatStyle.Flat;
      btn_numCd.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_numCd.Image = VvIco.TriangleGreenL24/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_greenLeft.ico")), 24, 24)*/.ToBitmap();
      btn_numCd.TabStop = false;
      btn_numCd.Visible = false;

      lbl_barCode1 = hamper.CreateVvLabel  (4, 1, "BarKod:"   , ContentAlignment.MiddleRight);
      tbx_barCode1 = hamper.CreateVvTextBox(5, 1, "tbx_barCode1", "Barkod artikla", GetDB_ColumnSize(DB_ci.barCode1), 1, 0);
      

      lbl_ts = hamper.CreateVvLabel        (0, 2, "Tip:", ContentAlignment.MiddleRight);
      tbx_ts = hamper.CreateVvTextBoxLookUp(1, 2, "tbx_ts", "Tip (kategorija) artikla roba, mater, vlproizv, usluga, ambalaza, uzorak, prolazna stavka, takse", GetDB_ColumnSize(DB_ci.ts));
      tbx_tsOpis = hamper.CreateVvTextBox  (2, 2, "tbx_tsOpis", "", 32);
      tbx_tsOpis.JAM_ReadOnly = true;
      tbx_ts.JAM_Set_LookUpTable(ZXC.luiListaArtiklTS, (int)ZXC.Kolona.prva);
      tbx_ts.JAM_lui_NameTaker_JAM_Name = tbx_tsOpis.JAM_Name;

      lbl_grupa1CD   = hamper.CreateVvLabel        (0, 3, "Grupa A:", ContentAlignment.MiddleRight);
      tbx_grupa1CD   = hamper.CreateVvTextBoxLookUp(1, 3, "tbx_grupa1CD", "", GetDB_ColumnSize(DB_ci.grupa1CD));
      tbx_grupa1Opis = hamper.CreateVvTextBox      (2, 3, "tbx_grupa1Opis", "", 32);
      tbx_grupa1Opis.JAM_ReadOnly = true;
      tbx_grupa1CD.JAM_Set_LookUpTable(ZXC.luiListaGrupa1Artikla, (int)ZXC.Kolona.prva);
      tbx_grupa1CD.JAM_lui_NameTaker_JAM_Name = tbx_grupa1Opis.JAM_Name;

      // 02.07.2018: 
      if(ZXC.IsSvDUH) tbx_grupa1CD.JAM_DataRequired = true;

      lbl_skladCD = hamper.CreateVvLabel        (0, 4, "Uob Sklad:", ContentAlignment.MiddleRight);
      tbx_skladCD = hamper.CreateVvTextBoxLookUp(1, 4, "tbx_skladCD", "", GetDB_ColumnSize(DB_ci.skladCD));
      tbx_skladOpis = hamper.CreateVvTextBox    (2, 4, "tbx_skladOpis", "", 32);
      tbx_skladOpis.JAM_ReadOnly = true;
      tbx_skladCD.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_skladCD.JAM_lui_NameTaker_JAM_Name = tbx_skladOpis.JAM_Name;


      lbl_jedMj = hamper.CreateVvLabel  (3, 2, "JM:", 1, 0, ContentAlignment.MiddleRight);
      tbx_jedMj = hamper.CreateVvTextBox(5, 2, "tbx_jedMj", "Jedinica mjere", GetDB_ColumnSize(DB_ci.jedMj));
    
      lbl_konto = hamper.CreateVvLabel  (6, 2, "Konto:"   , 1, 0, ContentAlignment.MiddleRight);
      tbx_konto = hamper.CreateVvTextBox(8, 2, "tbx_konto", "Konto", GetDB_ColumnSize(DB_ci.konto));
      tbx_konto.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_konto.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);


      lbl_polCij = hamper.CreateVvLabel      (3, 3, "ProdCij:", 1, 0, ContentAlignment.MiddleRight);
      rbt_vpc1   = hamper.CreateVvRadioButton(5, 3, null, "VPC1",  TextImageRelation.TextBeforeImage);
      rbt_marza  = hamper.CreateVvRadioButton(6, 3, null, "Marža",  TextImageRelation.TextBeforeImage);

      cbx_isRashod = hamper.CreateVvCheckBox_OLD(7, 3, null, 1, 0, "Neaktivan", System.Windows.Forms.RightToLeft.Yes);

      lbl_pdvKat     = hamper.CreateVvLabel        (3, 4, "PDV razred:", 1, 0, ContentAlignment.MiddleRight);
      tbx_pdvKat     = hamper.CreateVvTextBoxLookUp(5, 4, "tbx_pdvKat", "", GetDB_ColumnSize(DB_ci.pdvKat));
      tbx_pdvKatOpis = hamper.CreateVvTextBox      (6, 4, "tbx_pdvKatOpis", "PDV razred (npr 23%, 10%, 0%, oslob, ...)", 32, 2, 0);
      tbx_pdvKatOpis.JAM_ReadOnly = true;
      tbx_pdvKat.JAM_Set_LookUpTable(ZXC.luiListaPdvKat, (int)ZXC.Kolona.prva);
      tbx_pdvKat.JAM_lui_NameTaker_JAM_Name = tbx_pdvKatOpis.JAM_Name;

      rbt_vpc1.Checked = true;
      rbt_vpc1.Tag     = true;
      //rbt_zadano.CheckAlign = ContentAlignment.MiddleRight;
      //rbt_marza.CheckAlign  = ContentAlignment.MiddleRight;
      //rbt_zadano.TextAlign  = ContentAlignment.MiddleRight;
      //rbt_marza.TextAlign   = ContentAlignment.MiddleRight;

      lbl_longOpis = hamper.CreateVvLabel  (8, 1, "Opis:", ContentAlignment.MiddleRight);
      tbx_longOpis = hamper.CreateVvTextBox(9, 0, "tbx_longOpis", "Opširan opis artikla", GetDB_ColumnSize(DB_ci.longOpis), 1, 4);
      tbx_longOpis.Font = ZXC.vvFont.SmallFont;

      tbx_longOpis.Multiline = true;
      tbx_longOpis.ScrollBars = ScrollBars.Vertical;


      this.ControlForInitialFocus = tbx_artiklName;
   }

   private void InitializeNazivHamper_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(11, 6, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      //                                       0                 1                     2                3             4           5        6         7         8                   9             10            
      hamper.VvColWdt      = new int[] { labelWidth, ZXC.Q3un - ZXC.Qun2, ZXC.Q8un + ZXC.Qun4,ZXC.QUN - ZXC.Qun4, labelWidth + ZXC.Qun2, ZXC.Q3un - ZXC.Qun2, ZXC.Q2un, ZXC.Q2un, ZXC.Q2un, ZXC.Q2un-ZXC.Qun2, ZXC.Q10un + ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] {   ZXC.Qun4,            ZXC.Qun4,            ZXC.Qun4,                 0,                     0,            ZXC.Qun8, ZXC.Qun8,        0,       0,         ZXC.Qun12,  ZXC.Qun4             };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      } 
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel  (0, 0, "Naziv:", ContentAlignment.MiddleRight);
      tbx_artiklName = hamper.CreateVvTextBox(1, 0, "tbx_artiklName", "Naziv artikla", GetDB_ColumnSize(DB_ci.artiklName), 8, 0);
      tbx_artiklName.JAM_Highlighted = true;
      tbx_artiklName.JAM_DataRequired = true;
      
                     hamper.CreateVvLabel  (0, 1, "Šifra:", ContentAlignment.MiddleRight);
      tbx_artiklCD = hamper.CreateVvTextBox(1, 1, "tbx_artiklCD", "Šifra artikla", GetDB_ColumnSize(DB_ci.artiklCD), 1, 0);
      tbx_artiklCD.JAM_Highlighted = true;
      tbx_artiklCD.JAM_DataRequired = true;

      btn_numCd = hamper.CreateVvButton(3, 1, new EventHandler(GetNextSifraWroot_String_btnClick), "");
      btn_numCd.Name = "btn_goExLinkNewNum";
      btn_numCd.FlatStyle = FlatStyle.Flat;
      btn_numCd.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_numCd.Image = VvIco.TriangleGreenL24/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_greenLeft.ico")), 24, 24)*/.ToBitmap();
      btn_numCd.TabStop = false;
      btn_numCd.Visible = false;

               hamper.CreateVvLabel        (0, 2, "Tip:", ContentAlignment.MiddleRight);
      tbx_ts = hamper.CreateVvTextBoxLookUp(1, 2, "tbx_ts", "Tip (kategorija) artikla roba, mater, vlproizv, usluga, ambalaza, uzorak, prolazna stavka, takse", GetDB_ColumnSize(DB_ci.ts));
      tbx_tsOpis = hamper.CreateVvTextBox  (2, 2, "tbx_tsOpis", "", 32);
      tbx_tsOpis.JAM_ReadOnly = true;
      tbx_ts.JAM_Set_LookUpTable(ZXC.luiListaArtiklTS, (int)ZXC.Kolona.prva);
      tbx_ts.JAM_lui_NameTaker_JAM_Name = tbx_tsOpis.JAM_Name;

                       hamper.CreateVvLabel        (0, 3, "Grupa A:", ContentAlignment.MiddleRight);
      tbx_grupa1CD   = hamper.CreateVvTextBoxLookUp(1, 3, "tbx_grupa1CD", "", GetDB_ColumnSize(DB_ci.grupa1CD));
      tbx_grupa1Opis = hamper.CreateVvTextBox      (2, 3, "tbx_grupa1Opis", "", 32);
      tbx_grupa1Opis.JAM_ReadOnly = true;
      tbx_grupa1CD.JAM_Set_LookUpTable(ZXC.luiListaGrupa1Artikla, (int)ZXC.Kolona.prva);
      tbx_grupa1CD.JAM_lui_NameTaker_JAM_Name = tbx_grupa1Opis.JAM_Name;

                       hamper.CreateVvLabel        (0, 4, "Gr RAM:", ContentAlignment.MiddleRight);
      tbx_grupa2CD   = hamper.CreateVvTextBoxLookUp(1, 4, "tbx_grupa2CD", "", GetDB_ColumnSize(DB_ci.grupa2CD));
      tbx_grupa2Opis = hamper.CreateVvTextBox      (2, 4, "tbx_grupa2Opis", "", 32);
      tbx_grupa2Opis.JAM_ReadOnly = true;
      tbx_grupa2CD.JAM_Set_LookUpTable(ZXC.luiListaGrupa2Artikla, (int)ZXC.Kolona.prva);
      tbx_grupa2CD.JAM_lui_NameTaker_JAM_Name = tbx_grupa2Opis.JAM_Name;

                       hamper.CreateVvLabel        (0, 5, "Gr HDD:", ContentAlignment.MiddleRight);
      tbx_grupa3CD   = hamper.CreateVvTextBoxLookUp(1, 5, "tbx_grupa3CD", "", GetDB_ColumnSize(DB_ci.grupa3CD));
      tbx_grupa3Opis = hamper.CreateVvTextBox      (2, 5, "tbx_grupa3Opis", "", 32);
      tbx_grupa3Opis.JAM_ReadOnly = true;
      tbx_grupa3CD.JAM_Set_LookUpTable(ZXC.luiListaGrupa3Artikla, (int)ZXC.Kolona.prva);
      tbx_grupa3CD.JAM_lui_NameTaker_JAM_Name = tbx_grupa3Opis.JAM_Name;


                      hamper.CreateVvLabel  (4, 1, "PCK baza:", ContentAlignment.MiddleRight);
      tbx_carTarifa = hamper.CreateVvTextBox(5, 1, "tbx_CarTarifa", "PCK baza", GetDB_ColumnSize(DB_ci.carTarifa), 1, 0);

                        hamper.CreateVvLabel   (7, 1, "RAM:", ContentAlignment.MiddleRight);
      tbx_zapremina   = hamper.CreateVvTextBox (8, 1, "tbx_zapremina"  , "RAM"   , GetDB_ColumnSize(DB_ci.zapremina));
      tbx_zapreminaJM = hamper.CreateVvTextBox (9, 1, "tbx_zapreminaJM", "RAM JM", GetDB_ColumnSize(DB_ci.zapreminaJM));
      tbx_zapremina.JAM_ForeColor = tbx_zapreminaJM.JAM_ForeColor = ZXC.vvColors.clr_RAM_PTG; 
      tbx_zapremina.JAM_Highlighted = true;
      tbx_zapremina.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

                      hamper.CreateVvLabel   (7, 2, "HDD:", ContentAlignment.MiddleRight);
      tbx_duljina   = hamper.CreateVvTextBox (8, 2, "tbx_duljina"  , "HDD"   , GetDB_ColumnSize(DB_ci.duljina));
      tbx_duljinaJM = hamper.CreateVvTextBox (9, 2, "tbx_duljinaJM", "HDD JM", GetDB_ColumnSize(DB_ci.duljinaJM));
      tbx_duljina.JAM_ForeColor = tbx_duljinaJM.JAM_ForeColor = ZXC.vvColors.clr_HDD_PTG; 
      tbx_duljina.JAM_Highlighted = true;
      tbx_duljina.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

                     hamper.CreateVvLabel  (4, 3, "BarKod:"   , ContentAlignment.MiddleRight);
      tbx_barCode1 = hamper.CreateVvTextBox(5, 3, "tbx_barCode1", "Barkod artikla", GetDB_ColumnSize(DB_ci.barCode1), 1, 0);

                  hamper.CreateVvLabel  ( 7, 3, "JM:", ContentAlignment.MiddleRight);
      tbx_jedMj = hamper.CreateVvTextBox( 8, 3, "tbx_jedMj", "Jedinica mjere", GetDB_ColumnSize(DB_ci.jedMj),1,0);
    
                       hamper.CreateVvLabel        (4, 4, "PDV razred:", ContentAlignment.MiddleRight);
      tbx_pdvKat     = hamper.CreateVvTextBoxLookUp(5, 4, "tbx_pdvKat", "", GetDB_ColumnSize(DB_ci.pdvKat));
      tbx_pdvKatOpis = hamper.CreateVvTextBox      (6, 4, "tbx_pdvKatOpis", "PDV razred (npr 23%, 10%, 0%, oslob, ...)", 32, 3, 0);
      tbx_pdvKatOpis.JAM_ReadOnly = true;
      tbx_pdvKat.JAM_Set_LookUpTable(ZXC.luiListaPdvKat, (int)ZXC.Kolona.prva);
      tbx_pdvKat.JAM_lui_NameTaker_JAM_Name = tbx_pdvKatOpis.JAM_Name;

                    hamper.CreateVvLabel        (4, 5, "Uob Skl:", ContentAlignment.MiddleRight);
      tbx_skladCD = hamper.CreateVvTextBoxLookUp(5, 5, "tbx_skladCD", "", GetDB_ColumnSize(DB_ci.skladCD));
      tbx_skladOpis = hamper.CreateVvTextBox    (6, 5, "tbx_skladOpis", "", 32, 3, 0);
      tbx_skladOpis.JAM_ReadOnly = true;
      tbx_skladCD.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_skladCD.JAM_lui_NameTaker_JAM_Name = tbx_skladOpis.JAM_Name;
  
                    // hamper.CreateVvLabel  (8, 1, "Opis:", ContentAlignment.MiddleRight);
      tbx_longOpis = hamper.CreateVvTextBox(10, 0, "tbx_longOpis", "Opširan opis artikla", GetDB_ColumnSize(DB_ci.longOpis), 0, 5);
      tbx_longOpis.Font = ZXC.vvFont.SmallFont;
      tbx_longOpis.Multiline = true;
      tbx_longOpis.ScrollBars = ScrollBars.Vertical;



                //hamper.CreateVvLabel  (6, 2, "Konto:"   , 1, 0, ContentAlignment.MiddleRight);
      tbx_konto = hamper.CreateVvTextBox(8, 2, "tbx_konto", "Konto", GetDB_ColumnSize(DB_ci.konto));
      tbx_konto.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_konto.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
                   //hamper.CreateVvLabel      (3, 3, "ProdCij:", 1, 0, ContentAlignment.MiddleRight);
      rbt_vpc1     = hamper.CreateVvRadioButton(5, 3, null, "VPC1",  TextImageRelation.TextBeforeImage);
      rbt_marza    = hamper.CreateVvRadioButton(6, 3, null, "Marža",  TextImageRelation.TextBeforeImage);
      cbx_isRashod = hamper.CreateVvCheckBox_OLD(7, 3, null, 1, 0, "Neaktivan", System.Windows.Forms.RightToLeft.Yes);
      tbx_konto   .Visible =
      rbt_vpc1    .Visible = 
      rbt_marza   .Visible = 
      cbx_isRashod.Visible = false;
      rbt_vpc1.Checked = true;
      rbt_vpc1.Tag     = true;

      this.ControlForInitialFocus = tbx_artiklName;
   }

   #region CreateReadOnlyHampers
 
   private void CreateResultPanel()
   {
      nextX = 0;
      nextY = 0;
      InitializeZaSkladHamper(out hamp_zaSklad);
      nextY = hamp_zaSklad.Bottom;
      InitializeResultHamper(out hamp_result);
      
      nextX = hamp_result.Right + ZXC.Qun8;

      InitializeVPCHamper(out hamp_VPC);
      InitializeOPHamper(out hamp_OP);

      panel_result.Size      = new Size(hamp_naziv.Right -ZXC.Qun2+ZXC.Qun12, hamp_result.Bottom);
      panel_result.BackColor = backColorResult;     // ZXC.vvColors.vvPanel4TBoxResultBox_BackColor;

   }

   private void InitializeZaSkladHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(9, 1, "", panel_result, false, nextX, nextY, razmakHamp);
      //                                          0                  1                      2               3           4                      5          
      //hamper.VvColWdt      = new int[] { ZXC.Q10un + ZXC.Qun4, ZXC.Q3un-ZXC.Qun2, ZXC.Q8un + ZXC.Qun4 , labelWidth, ZXC.Q4un-ZXC.Qun4, ZXC.Q10un};
      hamper.VvColWdt      = new int[] { ZXC.Q10un + ZXC.Qun4, ZXC.Q3un - ZXC.Qun2, ZXC.Q8un + ZXC.Qun4, labelWidth, ZXC.Q4un - ZXC.Qun8, ZXC.Q4un + ZXC.Qun4, ZXC.Q3un - ZXC.Qun8, ZXC.Q3un - ZXC.Qun8, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] {                    0,            ZXC.Qun4,             ZXC.Qun4,   ZXC.Qun4, ZXC.Qun4, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8,ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
                        hamper.CreateVvLabel        (0, 0, "SITUACIJA ZA SKLADIŠTE", ContentAlignment.MiddleRight);
      tbx_zaSkladCD   = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_zaSkladCD", "", GetDB_ColumnSize(DB_ci.skladCD));
      tbx_zaSkladOpis = hamper.CreateVvTextBox      (2, 0, "tbx_zaSkladOpis", "", 32);
      tbx_zaSkladOpis.JAM_ReadOnly = true;
      tbx_zaSkladCD.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_zaSkladCD.JAM_lui_NameTaker_JAM_Name = tbx_zaSkladOpis.JAM_Name;
     
      tbx_zaSkladCD.JAM_WriteOnly = true;

      hamper.BackColor = backColorSklad;

      // BE ADVICED! ovako se 'konstantno' drzi event kao cjepivo za OpenOrCloseForWriting tj, da ga ne mijenja 
      tbx_zaSkladCD.TextChanged += new EventHandler(tbx_zaSkladCD.JAM_FieldExitMethod);

      tbx_zaSkladCD.Leave += new EventHandler(tbx_zaSkladCD_Leave_GetArtiklStatus_PutResultFields);
    //tbx_zaSkladCD.Leave       += new EventHandler(tbx_zaSkladCD_Leave_RememberTheLastUsedSkladCD);
    //tbx_zaSkladCD.TextChanged += new EventHandler(tbx_zaSkladCD_TextChanged_RememberTheLastUsedSkladCD);

                  hamper.CreateVvLabel(3, 0, "Na dan:", ContentAlignment.MiddleRight);
      tbx_naDan = hamper.CreateVvTextBox(4, 0, "tbx_naDan", "Stanje na dan");
      tbx_naDan.JAM_IsForDateTimePicker = true;
      tbx_naDan.JAM_WriteOnly           = true;
      dtp_naDan = hamper.CreateVvDateTimePicker(4, 0, "", tbx_naDan);
      dtp_naDan.Name = "dTP_naDan";

         #region Fld_NaDan & dTP_naDan

         dtp_naDan.ValueChanged += new EventHandler(dTP_naDan_ValueChanged);

         #endregion Fld_NaDan & dTP_naDan

      
      lbl_frsMin      = hamper.CreateVvLabel  (5, 0, "Prvi MINUS:"    , ContentAlignment.MiddleRight);
      tbx_frsMinTT    = hamper.CreateVvTextBox(6, 0, "tbx_frsMinTT"   , "");
      tbx_frsMinTTnum = hamper.CreateVvTextBox(7, 0, "tbx_frsMinTTnum", "");
      tbx_frsMinTT   .JAM_ReadOnly = true;
      tbx_frsMinTTnum.JAM_ReadOnly = true;
      tbx_frsMinTT   .JAM_ForeColor = Color.Red;
      tbx_frsMinTTnum.JAM_ForeColor = Color.Red;
      tbx_frsMinTT   .JAM_Highlighted = true;
      tbx_frsMinTTnum.JAM_Highlighted = true;
      lbl_frsMin.ForeColor = Color.Red;
      lbl_frsMin.Font = ZXC.vvFont.SmallBoldFont;
      //tbx_frsMinTT   .Text = "ura";
      //tbx_frsMinTTnum.Text = "159";

      tbx_frsMinTT   .Visible = 
      tbx_frsMinTTnum.Visible = 
      lbl_frsMin     .Visible = false;
   }
 
   private void InitializeResultHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(8, 11, "", panel_result, false, nextX, nextY, razmakHamp);
      //                                        0             1                  2          3              4                   5           6               7
      hamper.VvColWdt      = new int[] { labelWidth, ZXC.Q4un-ZXC.Qun4, labelWidth, ZXC.Q4un-ZXC.Qun4, labelWidth, ZXC.Q4un-ZXC.Qun4, labelWidth, ZXC.Q4un-ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] {          0,          ZXC.Qun4,   ZXC.Qun4,          ZXC.Qun4,   ZXC.Qun4,          ZXC.Qun4,   ZXC.Qun4,           ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvRowHgt[9]     = ZXC.Qun4;
      hamper.VvSpcBefRow[10] = ZXC.Qun10;
      hamper.VvBottomMargin  = hamper.VvTopMargin;

      //ps                                                                                                                                   
                         hamper.CreateVvLabel  (0, 0, "POČETNO STANJE", 1, 0, ContentAlignment.MiddleRight);
                         hamper.CreateVvLabel  (0, 1, "Zadnje:", ContentAlignment.MiddleRight);
      tbx_datePS       = hamper.CreateVvTextBox(1, 1, "tbx_dateZadUlaz", "Datum PS");
      tbx_datePS.JAM_IsForDateTimePicker = true;
      dtp_datePS = hamper.CreateVvDateTimePicker(1, 1, "", tbx_datePS);
      dtp_datePS.Name = "dtp_datePS";
                         hamper.CreateVvLabel  (0, 2, "Količina:"     , ContentAlignment.MiddleRight);
      tbx_psKolSt      = hamper.CreateVvTextBox(1, 2, "tbx_psKolSt"   , "Početno stanje - kolicina", 12);
                         hamper.CreateVvLabel  (0, 4, "Cijena:"       , ContentAlignment.MiddleRight);
      tbx_psNabCij     = hamper.CreateVvTextBox(1, 4, "tbx_psNabCij"  , "Početno stanje - cijena", 12);
                         hamper.CreateVvLabel  (0, 5, "Vrijednost:"   , ContentAlignment.MiddleRight);
      tbx_psFinSt      = hamper.CreateVvTextBox(1, 5, "tbx_psFinSt"   , "Početno stanje - količnina", 12);

                         hamper.CreateVvLabel   (0, 6, "INVENTUR:", ContentAlignment.MiddleRight);
      tbx_dateZadInv   = hamper.CreateVvTextBox (1, 6, "tbx_dateZadInv", "Datum zadnje inventure");
      tbx_dateZadInv.JAM_IsForDateTimePicker = true;
      dtp_dateZadInv = hamper.CreateVvDateTimePicker(1, 6, "", tbx_dateZadInv);
      dtp_dateZadInv.Name = "dtp_dateZadInv";

                         hamper.CreateVvLabel  (0, 7, "Količina:"    , ContentAlignment.MiddleRight);
      tbx_invKolSt     = hamper.CreateVvTextBox(1, 7, "tbx_invKolSt"  , "Inventurno stanje - količnina", 12);
                         hamper.CreateVvLabel  (0, 8, "Vrijednost:"    , ContentAlignment.MiddleRight);
      tbx_invFinSt     = hamper.CreateVvTextBox(1, 8, "tbx_invFinSt"  , "Inventurno stanje - vrijednost", 12);

      //ulaz                                                                                                                                   
                         hamper.CreateVvLabel   (2, 0, "ULAZ", 1, 0, ContentAlignment.MiddleRight);
                         hamper.CreateVvLabel   (2, 1, "Zadnji:", ContentAlignment.MiddleRight);
      tbx_dateZadUlaz  = hamper.CreateVvTextBox (3, 1, "tbx_dateZadUlaz", "Datum zadnjeg ulaza");
      tbx_dateZadUlaz.JAM_IsForDateTimePicker = true; 
      dtp_dateZadUlaz = hamper.CreateVvDateTimePicker(3, 1, "", tbx_dateZadUlaz);
      dtp_dateZadUlaz.Name = "dtp_dateZadUlaz";
 
                         hamper.CreateVvLabel   (2, 2, "KnjigKol:", ContentAlignment.MiddleRight);
      tbx_ulazKol      = hamper.CreateVvTextBox (3, 2, "tbx_ulazKol", "", 12);
                         hamper.CreateVvLabel   (2, 3, "FizičKol:", ContentAlignment.MiddleRight);
      tbx_uKolFisycal  = hamper.CreateVvTextBox (3, 3, "tbx_uKolFisycal", "Ulazno Količinsko stanje skladista ", 12);
                         hamper.CreateVvLabel   (2, 4, "PrUlazCij:"              , ContentAlignment.MiddleRight);
      tbx_prNabCijU    = hamper.CreateVvTextBox (3, 4, "tbx_prNabCijU", "Stanje zaliha - prosječna cijena", 12);
                         hamper.CreateVvLabel   (2, 5, "Vrijednost:", ContentAlignment.MiddleRight);
      tbx_ulazFin      = hamper.CreateVvTextBox (3, 5, "tbx_ulazFin", "", 12);
                         hamper.CreateVvLabel   (2, 6, "MinNabCij:"          , ContentAlignment.MiddleRight);
      tbx_ulMinCij     = hamper.CreateVvTextBox (3, 6, "tbx_cijMin"    , "Minimalna nabavna cijena", 12);
                         hamper.CreateVvLabel   (2, 7, "MaxNabCij:"          , ContentAlignment.MiddleRight);
      tbx_ulMaxCij     = hamper.CreateVvTextBox (3, 7, "tbx_cijMax"    , "Maksimalna nabvna cijena", 12);
                         hamper.CreateVvLabel   (2, 8, "ZadnjaCij:"       , ContentAlignment.MiddleRight);
      tbx_ulLastCij    = hamper.CreateVvTextBox (3, 8, "tbx_ulLastCij"    , "Zadnja nabavna cijena", 12);

      //izlaz                                                                                                                                   
                         hamper.CreateVvLabel   (4, 0, "IZLAZ", 1, 0, ContentAlignment.MiddleRight);
                         hamper.CreateVvLabel   (4, 1, "Zadnji:", ContentAlignment.MiddleRight);
      tbx_dateZadIzlaz = hamper.CreateVvTextBox (5, 1, "tbx_dateZadIzlaz", "Datum zadnjeg izlaza");
      tbx_dateZadIzlaz.JAM_IsForDateTimePicker = true;
      dtp_dateZadIzlaz = hamper.CreateVvDateTimePicker(5, 1, "", tbx_dateZadIzlaz);
      dtp_dateZadIzlaz.Name = "dtp_dateZadIzlaz";
                         hamper.CreateVvLabel   (4, 2, "KnjigKol:", ContentAlignment.MiddleRight);
      tbx_izlazKol     = hamper.CreateVvTextBox (5, 2, "tbx_izlazKol", "", 12);
                         hamper.CreateVvLabel   (4, 3, "FizičKol:"         , ContentAlignment.MiddleRight);
      tbx_iKolFisycal  = hamper.CreateVvTextBox (5, 3, "tbx_iKolFisycal"   , "Izlaz Količinsko stanje skladista ", 12);
                         hamper.CreateVvLabel   (4, 4, "PrIzNabCij:", ContentAlignment.MiddleRight);
      tbx_prNabCij2    = hamper.CreateVvTextBox (5, 4, "tbx_prNabCij2", "Po nabavnoj cijeni", 12);
                         hamper.CreateVvLabel   (4, 5, "VrPoNabC:", ContentAlignment.MiddleRight);
      tbx_izlazFinNab  = hamper.CreateVvTextBox (5, 5, "tbx_izlazFinNab", "Vrijednost prodaje po nabavnoj cijeni", 12);
                         hamper.CreateVvLabel   (4, 6, "MinProdCij:"           , ContentAlignment.MiddleRight);
      tbx_izlazCijMin  = hamper.CreateVvTextBox (5, 6, "tbx_izlazCijMin"   , "Minimalna izlazna cijena", 12);
                         hamper.CreateVvLabel   (4, 7, "MaxProdCij:"           , ContentAlignment.MiddleRight);
      tbx_izlazCijMax  = hamper.CreateVvTextBox (5, 7, "tbx_izlazCijMax"   , "Maksimalna izlazna cijena", 12);
                         hamper.CreateVvLabel   (4, 8, "ZadnjaCij:"        ,  ContentAlignment.MiddleRight);
      tbx_izlazCijLast = hamper.CreateVvTextBox (5, 8, "tbtbx_izlazCijLast", "Zadnja izlazna cijena", 12);

      //stanje                                                                                                                                   
                         hamper.CreateVvLabel   (6, 0, "TRENUTNO STANJE", 1, 0, ContentAlignment.MiddleRight);
                         hamper.CreateVvLabel   (6, 1, "ZdKnjPrm:", ContentAlignment.MiddleRight);
      tbx_dateZadnji   = hamper.CreateVvTextBox (7, 1, "tbx_dateZadnji", "Datum zadnje promjene");
      tbx_dateZadnji.JAM_IsForDateTimePicker = true;
      dtp_dateZadnji = hamper.CreateVvDateTimePicker(7, 1, "", tbx_dateZadnji);
      dtp_dateZadnji.Name = "dtp_dateZadnji";
                         hamper.CreateVvLabel   (6, 2, "KnjigKol:"            , ContentAlignment.MiddleRight);
      tbx_kolSt        = hamper.CreateVvTextBox (7, 2, "tbx_kolSt"          , "Stanje zaliha - financijski saldo", 12);
      tbx_kolSt.JAM_Highlighted = true;
                         hamper.CreateVvLabel   (6, 3, ZXC.IsSvDUH ? "NbcBezPdv:" : "FizičKol:"      , ContentAlignment.MiddleRight);
      tbx_SklKol       = hamper.CreateVvTextBox (7, 3, "tbx_SklKol", "Količinsko stanje skladista ", 12);
                         hamper.CreateVvLabel   (6, 4, "PrNabCij:"           , ContentAlignment.MiddleRight);
      tbx_prNabCij     = hamper.CreateVvTextBox (7, 4, "tbx_prNabCij", "Stanje zaliha - prosječna cijena", 12);
                         hamper.CreateVvLabel   (6, 5, "VrPoNabC:", ContentAlignment.MiddleRight);
      tbx_finSt        = hamper.CreateVvTextBox (7, 5, "tbx_finSt"        , "Stanje zaliha - robno", 12);
                         hamper.CreateVvLabel   (6, 6, "Rezervacije:"          , ContentAlignment.MiddleRight);
      tbx_izlRezervKol = hamper.CreateVvTextBox (7, 6, "tbx_izlRezervKol"        , "Količina obvezujuce ponude", 12);
                         hamper.CreateVvLabel   (6, 7, "RasplžvKol:"          , ContentAlignment.MiddleRight);
      tbx_KolStFree    = hamper.CreateVvTextBox (7, 7, "tbx_KolStFree", "Raspoloživa količina ", 12);
                         hamper.CreateVvLabel   (6, 8, "ZadPrNbC:"          , ContentAlignment.MiddleRight);
      tbx_HelpNabC     = hamper.CreateVvTextBox (7, 8, "tbx_HelpNabC", "HelpNabC", 12);


      Label labCrta = hamper.CreateVvLabel(0, 9, "", 7, 0, ContentAlignment.MiddleLeft);
      labCrta.BackColor =  backColorSklad;

                         hamper.CreateVvLabel   (0, 10, "IzlPoProdC:", ContentAlignment.MiddleRight);
      tbx_izlazFinProd = hamper.CreateVvTextBox (1, 10, "tbx_proVrP", "Vrijednost prodaje po prodajnoj cijeni", 12);
                         hamper.CreateVvLabel   (2, 10, "IzlPoNabC:", ContentAlignment.MiddleRight);
      tbx_izlazFinNab2 = hamper.CreateVvTextBox (3, 10, "tbx_izlazFinNab", "Vrijednost prodaje po nabavnoj cijeni", 12);
                         hamper.CreateVvLabel   (4, 10, "RUV izn:"    , ContentAlignment.MiddleRight);
      tbx_izlazRUV     = hamper.CreateVvTextBox (5, 10, "tbx_izlazRUV" , "Razlika u vrijednosti", 12);
                         hamper.CreateVvLabel   (6, 10, "RUV %:"   , ContentAlignment.MiddleRight);
      tbx_ruvPost      = hamper.CreateVvTextBox (7, 10, "tbx_ruv" , "Razlika u vrijednosti", 12);

      tbx_ruvPost.JAM_IsForPercent = true;

      foreach(Control ctr in hamper.Controls)
      {
         if(ctr is VvTextBox)
         {
            ((VvTextBox)ctr).JAM_ResultBox = true;
            if(!((VvTextBox)ctr).JAM_IsForDateTimePicker) ((VvTextBox)ctr).JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
         }
      }

      tbx_kolSt   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, false);
      tbx_SklKol  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, false);
      tbx_prNabCij.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, false);
      tbx_finSt   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, false);
      tbx_HelpNabC.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, false);


      tbx_prNabCij2   .JAM_ForeColor =
      tbx_psKolSt     .JAM_ForeColor =
      tbx_izlazKol    .JAM_ForeColor =
      tbx_ulazKol     .JAM_ForeColor =
      tbx_kolSt       .JAM_ForeColor =
      tbx_SklKol      .JAM_ForeColor =
      tbx_psNabCij    .JAM_ForeColor =
      tbx_prNabCijU   .JAM_ForeColor =
      tbx_ulLastCij   .JAM_ForeColor =
      tbx_ulMinCij    .JAM_ForeColor =
      tbx_ulMaxCij    .JAM_ForeColor =
      tbx_prNabCij    .JAM_ForeColor = 
      tbx_psFinSt     .JAM_ForeColor =
      tbx_ulazFin     .JAM_ForeColor =
      tbx_izlazFinNab .JAM_ForeColor =
      tbx_finSt       .JAM_ForeColor =
      tbx_izlRezervKol.JAM_ForeColor =
      tbx_KolStFree   .JAM_ForeColor = 
      tbx_izlazCijMin .JAM_ForeColor =
      tbx_izlazCijMax .JAM_ForeColor =
      tbx_izlazCijLast.JAM_ForeColor =
      tbx_dateZadUlaz .JAM_ForeColor =
      tbx_dateZadIzlaz.JAM_ForeColor =
      tbx_datePS      .JAM_ForeColor =
      tbx_dateZadnji  .JAM_ForeColor = 
      tbx_HelpNabC    .JAM_ForeColor =   foreColorMostResult;

      tbx_izlazFinProd.JAM_ForeColor =
      tbx_izlazFinNab2.JAM_ForeColor =
      tbx_izlazRUV    .JAM_ForeColor =
      tbx_ruvPost     .JAM_ForeColor = foreColorRUVResult;

      tbx_uKolFisycal.JAM_ForeColor =
      tbx_iKolFisycal.JAM_ForeColor =
      tbx_SklKol     .JAM_ForeColor = foreColorKolSklResult;
   }

   private void InitializeVPCHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", panel_result, false, nextX, nextY, razmakHamp);
      //                                            0             1   }
      hamper.VvColWdt      = new int[] {labelWidth, ZXC.Q4un-ZXC.Qun4,labelWidth, ZXC.Q4un-ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] {  ZXC.Qun4,          ZXC.Qun4,  ZXC.Qun4,          ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl, lbl_VPC1, lbl_ruc;

      lbl            = hamper.CreateVvLabel  (0, 0, "ZADANO", 1, 0, ContentAlignment.MiddleRight);
      lbl_VPC1       = hamper.CreateVvLabel  (0, 1, "VPC1:"   , ContentAlignment.MiddleRight);
      tbx_cjenikVpc1 = hamper.CreateVvTextBox(1, 1, "tbx_VPC1", "Veleprodajna cijena 1", 12);
      lbl_ruc        = hamper.CreateVvLabel  (2, 1, "RUC %:"  , ContentAlignment.MiddleRight);
      tbx_ruc        = hamper.CreateVvTextBox(3, 1, "tbx_ruc" , "Razlika u cijeni", 12);

      tbx_cjenikVpc1.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_ruc       .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      tbx_cjenikVpc1.JAM_ResultBox =
      tbx_ruc       .JAM_ResultBox = true;
      
      tbx_cjenikVpc1.JAM_ForeColor = 
      tbx_ruc       .JAM_ForeColor = foreColorMostResult;

      tbx_ruc.JAM_IsForPercent = true;

      hamper.BackColor = backColorBmigrator;
   }

   private void InitializeOPHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", panel_result, false, nextX, hamp_VPC.Bottom + ZXC.Q8un, razmakHamp);
  
      hamper.VvColWdt      = new int[] {labelWidth, ZXC.Q4un-ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] {  ZXC.Qun4,          ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel  (0, 0, "NabCijOP", ContentAlignment.MiddleRight);
      tbx_prNabCijOP = hamper.CreateVvTextBox(1, 0, "tbx_prNabCijOP", "", 12);

      tbx_prNabCijOP.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_prNabCijOP.JAM_ResultBox = true;
      tbx_prNabCijOP.JAM_ForeColor = foreColorMostResult;

      hamper.BackColor = backColorResult;

      hamper.Visible = ZXC.RRD.Dsc_IsOrgPakVisible;

   }

   #endregion CreateReadOnlyHampers

   #endregion  HAMPER

   #region CreateMigratingHampers

   private void CreatePanels4MigratorHampers_LocationOfPanels()
   {
      panel_result.Location         = new Point(ZXC.Qun4, hamp_naziv.Bottom + ZXC.QUN);

      panel_MigratorsLeftA.Location = new Point(0, hamp_naziv.Bottom - ZXC.Qun8);
      panel_MigratorsLeftA.Size     = new Size (hamp_naziv.Right, 0);

      panel_MigratorsLeftB.Location  = new Point(hamp_VPC.Left, hamp_VPC.Bottom - ZXC.Qun8);
      panel_MigratorsLeftB.Size      = new Size (hamp_VPC.Width+ ZXC.Qun2 , hamp_result.Height - hamp_VPC.Height + ZXC.Qun8 - hamp_OP.Height);
      panel_MigratorsLeftB.BackColor = backColorBmigrator;
      panel_MigratorsLeftB.BringToFront();


      panel_MigratorsRightA.Location  = new Point(0, 0);
      panel_MigratorsRightA.Size      = new Size(hamp_artiklCD2.Right, hamp_garRok.Bottom + ZXC.QunMrgn);

      panel_MigratorsRightB.Size      = new Size(hamp_VPC2.Width + ZXC.Qun2, hamp_marza.Bottom);
      panel_MigratorsRightB.Location  = new Point(panel_MigratorsRightA.Right + ZXC.Qun4, ZXC.Qun8);
      panel_MigratorsRightB.BackColor = backColorBmigrator;
      panel_MigratorsRightB.BringToFront();
   }
   
   private void CreateMigratingHampers_Location()
   {
      panel_result          = new Panel();
      panel_result.Parent   = TheTabControl.TabPages[0];

      panel_MigratorsLeftA        = new Panel();
      panel_MigratorsLeftA.Parent = TheTabControl.TabPages[0];
    
      panel_MigratorsLeftB        = new Panel();
      panel_MigratorsLeftB.Parent = panel_result;

      panel_MigratorsRightA        = new Panel();
      panel_MigratorsRightA.Parent = TheTabControl.TabPages[1];

      panel_MigratorsRightB        = new Panel();
      panel_MigratorsRightB.Parent = TheTabControl.TabPages[1];

      int hamp1Col = ZXC.QUN - ZXC.Qun8; // izmedju hampera i cbx

      Initialize_ArtiklCD2Hamper(out hamp_artiklCD2, "ANaziv 2:", hamp1Col, 0);
      CreateCbxhamper4Migrators(hamp_artiklCD2, 0, 0, 1, true);

      drugakolona += hamp_artiklCD2.Right;

      Initialize_PartNoHamper(out hamp_partNo, "APartNo:", hamp1Col, hamp_artiklCD2.Bottom);
      CreateCbxhamper4Migrators(hamp_partNo, 0, hamp_artiklCD2.Bottom, 1, true);

      Initialize_DobavljacHamper(out hamp_dobavljac, "ADobavljač:", hamp1Col, hamp_partNo.Bottom);
      CreateCbxhamper4Migrators(hamp_dobavljac, 0, hamp_partNo.Bottom, 1, true);

      Initialize_ProizvodjacHamper(out hamp_proizvodjac, "AProizvođač:", hamp1Col, hamp_dobavljac.Bottom);
      CreateCbxhamper4Migrators(hamp_proizvodjac, 0, hamp_dobavljac.Bottom, 1, true);

      Initialize_DrzPorjeklaHamper(out hamp_drzPorjekla, "AMade In:", hamp1Col, hamp_proizvodjac.Bottom);
      CreateCbxhamper4Migrators(hamp_drzPorjekla, 0, hamp_proizvodjac.Bottom, 1, true);

      Initialize_BarCode2Hamper(out hamp_barCode2, "ABar Kod 2:", hamp1Col, hamp_drzPorjekla.Bottom);
      CreateCbxhamper4Migrators(hamp_barCode2, 0, hamp_drzPorjekla.Bottom, 1, true);

      Initialize_SerNoHamper(out hamp_serNo, "ASerijskiBr:", hamp1Col, hamp_barCode2.Bottom);
      CreateCbxhamper4Migrators(hamp_serNo, 0, hamp_barCode2.Bottom, 1, true);

      Initialize_Grupa2CDHamper(out hamp_grupa2CD, "AGrupa B:", hamp1Col, hamp_serNo.Bottom);
      CreateCbxhamper4Migrators(hamp_grupa2CD, 0, hamp_serNo.Bottom, 1, true);

      Initialize_Grupa3CDHamper(out hamp_grupa3CD, "AGrupa C:", hamp1Col, hamp_grupa2CD.Bottom);
      CreateCbxhamper4Migrators(hamp_grupa3CD, 0, hamp_grupa2CD.Bottom, 1, true);

      Initialize_PlacementHamper(out hamp_placement, "ASmještaj:", hamp1Col, hamp_grupa3CD.Bottom);
      CreateCbxhamper4Migrators(hamp_placement, 0, hamp_grupa3CD.Bottom, 1, true);

      Initialize_UrlHamper(out hamp_url, "AURL:", hamp1Col, hamp_placement.Bottom);
      CreateCbxhamper4Migrators(hamp_url, 0, hamp_placement.Bottom, 1, true);

      Initialize_CarTarifaHamper(out hamp_carTarifa, "ACarTarifa:", hamp1Col, hamp_url.Bottom);
      CreateCbxhamper4Migrators(hamp_carTarifa, 0, hamp_url.Bottom, 1, true);

      Initialize_LinkArtCDHamper(out hamp_linkArtCD, "AVezaniArtikl:", hamp1Col, hamp_carTarifa.Bottom);
      CreateCbxhamper4Migrators(hamp_linkArtCD, 0, hamp_carTarifa.Bottom, 1, true);

      Initialize_IsMasterHamper(out hamp_isMaster, "AGlavni artikl:", hamp1Col, hamp_linkArtCD.Bottom);
      CreateCbxhamper4Migrators(hamp_isMaster, 0, hamp_linkArtCD.Bottom, 1, true);
      
      Initialize_AtestBrHamp   (out hamp_atestBr, "AAtest Broj:", hamp1Col, hamp_isMaster.Bottom);
      CreateCbxhamper4Migrators(hamp_atestBr, 0, hamp_isMaster.Bottom, 1, true);

      Initialize_AtestDateHamp(out hamp_atestDate, "AAtestDat:", hamp1Col, hamp_atestBr.Bottom);
      CreateCbxhamper4Migrators(hamp_atestDate, 0, hamp_atestBr.Bottom, 1, true);

      Initialize_DateProizHamper(out hamp_dateProizv, "AGodProizv:", hamp1Col, hamp_atestDate.Bottom);
      CreateCbxhamper4Migrators(hamp_dateProizv, 0, hamp_atestDate.Bottom, 1, true);

      Initialize_PrefValNameHamp(out hamp_prefValName, "AValuta:", hamp1Col, hamp_dateProizv.Bottom);
      CreateCbxhamper4Migrators(hamp_prefValName, 0, hamp_dateProizv.Bottom, 1, true);

      Initialize_OrgPakHamper(out hamp_orgPak, "AOrgPakir:", hamp1Col, hamp_prefValName.Bottom);
      CreateCbxhamper4Migrators(hamp_orgPak, 0, hamp_prefValName.Bottom, 1, true);

      Initialize_GarRokHamp(out hamp_garRok, "AMjJamstva:", hamp1Col, hamp_orgPak.Bottom);
      CreateCbxhamper4Migrators(hamp_garRok, 0, hamp_orgPak.Bottom, 1, true);


      int cbx2Col  = hamp_url.Right + ZXC.Qun4;
      int hamp2Col = cbx2Col + hamp1Col;

      Initialize_NapomenaHamper(out hamp_napomena, "ANapomena:", hamp2Col, hamp_artiklCD2.Bottom);
      CreateCbxhamper4Migrators(hamp_napomena, cbx2Col, hamp_artiklCD2.Bottom, 1, true);

      Initialize_IsAkcijaHamper(out hamp_isAkcija, "ANa akciji:", hamp2Col, hamp_napomena.Bottom);
      CreateCbxhamper4Migrators(hamp_isAkcija, cbx2Col, hamp_napomena.Bottom, 1, true);

      Initialize_IsDozvMinusHamper(out hamp_isDozvMinus, "ADozvoli Minus:", hamp2Col, hamp_isAkcija.Bottom);
      CreateCbxhamper4Migrators(hamp_isDozvMinus, cbx2Col, hamp_isAkcija.Bottom, 1, true);

      Initialize_PrintOpisHamper(out hamp_printOpis, "APrintOpis:", hamp2Col, hamp_isDozvMinus.Bottom);
      CreateCbxhamper4Migrators(hamp_printOpis, cbx2Col, hamp_isDozvMinus.Bottom, 1, true);

      Initialize_IsSerBrHamper(out hamp_isSerBr, "ASerBr:", hamp2Col, hamp_printOpis.Bottom);
      CreateCbxhamper4Migrators(hamp_isSerBr, cbx2Col, hamp_printOpis.Bottom, 1, true);

      CreateMarkHampersJM(out hamp_masaNetto, "AMasaNetto:", hamp2Col, hamp_isSerBr.Bottom, out tbx_masaNetto, GetDB_ColumnSize(DB_ci.masaNetto), true, out tbx_masaNettoJM, GetDB_ColumnSize(DB_ci.masaNettoJM));
      CreateCbxhamper4Migrators(hamp_masaNetto, cbx2Col, hamp_isSerBr.Bottom, 1, true);

      CreateMarkHampersJM(out hamp_masaBruto, "AMasaBruto:", hamp2Col, hamp_masaNetto.Bottom, out tbx_masaBruto, GetDB_ColumnSize(DB_ci.masaBruto), true, out tbx_masaBrutoJM, GetDB_ColumnSize(DB_ci.masaBrutoJM));
      CreateCbxhamper4Migrators(hamp_masaBruto, cbx2Col, hamp_masaNetto.Bottom, 1, true);

      CreateMarkHampersJM(out hamp_promjer, "APromjer:", hamp2Col, hamp_masaBruto.Bottom, out tbx_promjer, GetDB_ColumnSize(DB_ci.promjer), true, out tbx_promjerJM, GetDB_ColumnSize(DB_ci.promjerJM));
      CreateCbxhamper4Migrators(hamp_promjer, cbx2Col, hamp_masaBruto.Bottom, 1, true);

      CreateMarkHampersJM(out hamp_povrsina, "APovršina:", hamp2Col, hamp_promjer.Bottom, out tbx_povrsina, GetDB_ColumnSize(DB_ci.povrsina), true, out tbx_povrsinaJM, GetDB_ColumnSize(DB_ci.povrsinaJM));
      CreateCbxhamper4Migrators(hamp_povrsina, cbx2Col, hamp_promjer.Bottom, 1, true);

      CreateMarkHampersJM(out hamp_zapremina, "AZapremina:", hamp2Col, hamp_povrsina.Bottom, out tbx_zapremina, GetDB_ColumnSize(DB_ci.zapremina), true, out tbx_zapreminaJM, GetDB_ColumnSize(DB_ci.zapreminaJM));
      CreateCbxhamper4Migrators(hamp_zapremina, cbx2Col, hamp_povrsina.Bottom, 1, true);

      CreateMarkHampersJM(out hamp_duljina, "ADuljina:", hamp2Col, hamp_zapremina.Bottom, out tbx_duljina, GetDB_ColumnSize(DB_ci.duljina), true, out tbx_duljinaJM, GetDB_ColumnSize(DB_ci.duljinaJM));
      CreateCbxhamper4Migrators(hamp_duljina, cbx2Col, hamp_zapremina.Bottom, 1, true);

      CreateMarkHampersJM(out hamp_sirina, "AŠirina:", hamp2Col, hamp_duljina.Bottom, out tbx_sirina, GetDB_ColumnSize(DB_ci.sirina), true, out tbx_sirinaJM, GetDB_ColumnSize(DB_ci.sirinaJM));
      CreateCbxhamper4Migrators(hamp_sirina, cbx2Col, hamp_duljina.Bottom, 1, true);

      CreateMarkHampersJM(out hamp_visina, "AVisina:", hamp2Col, hamp_sirina.Bottom, out tbx_visina, GetDB_ColumnSize(DB_ci.visina), true, out tbx_visinaJM, GetDB_ColumnSize(DB_ci.visinaJM));
      CreateCbxhamper4Migrators(hamp_visina, cbx2Col, hamp_sirina.Bottom, 1, true);

      CreateMarkHampers(out hamp_starost, "AUzrast:", hamp2Col, hamp_visina.Bottom, out tbx_starost, GetDB_ColumnSize(DB_ci.starost), true);
      CreateCbxhamper4Migrators(hamp_starost, cbx2Col, hamp_visina.Bottom, 1, true);

      CreateMarkHampers(out hamp_boja, "ABoja:", hamp2Col, hamp_starost.Bottom, out tbx_boja, GetDB_ColumnSize(DB_ci.boja), false);
      CreateCbxhamper4Migrators(hamp_boja, cbx2Col, hamp_starost.Bottom, 1, true);

      CreateMarkHampers(out hamp_velicina, "AVeličina:", hamp2Col, hamp_boja.Bottom, out tbx_velicina, GetDB_ColumnSize(DB_ci.velicina), false);
      CreateCbxhamper4Migrators(hamp_velicina, cbx2Col, hamp_boja.Bottom, 1, true);

      CreateMarkHampers(out hamp_spol, "ASpol:", hamp2Col, hamp_velicina.Bottom, out tbx_spol, GetDB_ColumnSize(DB_ci.spol), false);
      CreateCbxhamper4Migrators(hamp_spol, cbx2Col, hamp_velicina.Bottom, 1, true);

      CreateMarkHampers(out hamp_importCij, "AImportCij:", hamp2Col, hamp_spol.Bottom, out tbx_importCij, GetDB_ColumnSize(DB_ci.importCij), true);
      CreateCbxhamper4Migrators(hamp_importCij, cbx2Col, hamp_spol.Bottom, 1, true);

      CreateMarkHampersJM(out hamp_snaga, "ASnaga:", hamp2Col, hamp_importCij.Bottom, out tbx_snaga, GetDB_ColumnSize(DB_ci.snaga), true, out tbx_snagaJM, GetDB_ColumnSize(DB_ci.snagaJM));
      CreateCbxhamper4Migrators(hamp_snaga, cbx2Col, hamp_importCij.Bottom, 1, true);

      int cbx3Col  = hamp_importCij.Right + ZXC.Qun4;
      int hamp3Col = cbx3Col + hamp1Col;

      CreateMarkHampers(out hamp_co2, "AEmisijaCO2:", hamp3Col, hamp_napomena.Bottom, out tbx_emisCo2, GetDB_ColumnSize(DB_ci.emisCO2), false);
      CreateCbxhamper4Migrators(hamp_co2, cbx3Col, hamp_napomena.Bottom, 1, true);

      CreateMarkHampers(out hamp_eun, "AEuroNorma:", hamp3Col, hamp_co2.Bottom, out tbx_euroNorma, GetDB_ColumnSize(DB_ci.euroNorma), false);
      CreateCbxhamper4Migrators(hamp_eun, cbx3Col, hamp_co2.Bottom, 1, true);
      tbx_euroNorma.JAM_AllowedInputCharacters = "123456";
      

      // BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
      CreateMarkHampers(out hamp_VPC2, "BVPC2:", hamp1Col, 0, out tbx_VPC2, 12, true);
      CreateCbxhamper4Migrators(hamp_VPC2, 0, 0, 1, true);

      CreateMarkHampers(out hamp_MPC1, "BMPC1:", hamp1Col, hamp_VPC2.Bottom, out tbx_MPC1, 12, true);
      CreateCbxhamper4Migrators(hamp_MPC1, 0, hamp_VPC2.Bottom, 1, true);

      CreateMarkHampers(out hamp_devC, "BDevC:", hamp1Col, hamp_MPC1.Bottom, out tbx_devC, 12, true);
      CreateCbxhamper4Migrators(hamp_devC, 0, hamp_MPC1.Bottom, 1, true);

      CreateMarkHampers(out hamp_rabat1, "BRabat1:", hamp1Col, hamp_devC.Bottom, out tbx_rabat1, 12, true);
      CreateCbxhamper4Migrators(hamp_rabat1, 0, hamp_devC.Bottom, 1, true);

      CreateMarkHampers(out hamp_rabat2, "BRabat2:", hamp1Col, hamp_rabat1.Bottom, out tbx_rabat2, 12, true);
      CreateCbxhamper4Migrators(hamp_rabat2, 0, hamp_rabat1.Bottom, 1, true);

      CreateMarkHampers(out hamp_minKol, "BMinKol:", hamp1Col, hamp_rabat2.Bottom, out tbx_minKol, 12, true);
      CreateCbxhamper4Migrators(hamp_minKol, 0, hamp_rabat2.Bottom, 1, true);

      CreateMarkHampers(out hamp_marza, "BMarža:", hamp1Col, hamp_minKol.Bottom, out tbx_marza, 12, true);
      CreateCbxhamper4Migrators(hamp_marza, 0, hamp_minKol.Bottom, 1, true);

      // BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB

      tbx_rabat1.JAM_IsForPercent = true;
      tbx_rabat2.JAM_IsForPercent = true;

   }

   private void CreateMarkHampers(out VvHamper hamper, string name, int _nextX, int _nextY, out VvTextBox tbx, int dbci, bool isDecimal)
   {
      if(name.StartsWith("A"))
      {
         hamper = new VvHamper(2, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp); ;
      }
      else
      {
         hamper = new VvHamper(2, 1, "", MigratorRightParentB, false, _nextX, _nextY, razmakHamp); ;
         hamper.BackColor = backColorBmigrator;
      }
 
      hamper.VvColWdt    = new int[] { labelWidth, ZXC.Q4un - ZXC.Qun4 };
      hamper.VvSpcBefCol = new int[] {   ZXC.Qun4,            ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;
   
             hamper.CreateVvLabel  (0, 0, name, ContentAlignment.MiddleRight);
      tbx  = hamper.CreateVvTextBox(1, 0, "tbx_" + name, name, dbci);

      if(name.StartsWith("AImportCij"))
      {
        if(isDecimal) tbx.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      }
      else if(name.StartsWith("A"))
      {
         if(isDecimal) tbx.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, 9999M, true);
      }
      else
      {
         if(isDecimal) tbx.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
         
         tbx.JAM_ResultBox = true;
         tbx.JAM_ForeColor = foreColorMostResult;
      }
   }
  
   private void Initialize_NapomenaHamper(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(3, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

    //hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q10un + ZXC.QUN};
      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q9un + ZXC.QUN + ZXC.Qun4, ZXC.QUN - ZXC.Qun4 };

      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, 0};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

                     hamper.CreateVvLabel  (0, 0, hamper.Name, ContentAlignment.MiddleRight);
      tbx_napomena = hamper.CreateVvTextBox(1, 0, "tbx_napomena", "Napomena", GetDB_ColumnSize(DB_ci.napomena));

      btn_openExLinkNap = hamper.CreateVvButton(2, 0, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLinkNap.Name = "btn_openExLinkNap";
      btn_openExLinkNap.FlatStyle = FlatStyle.Flat;
      btn_openExLinkNap.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLinkNap.Image = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLinkNap.Tag = 2;
      btn_openExLinkNap.TabStop = false;


   }

   private void CreateMarkHampersJM(out VvHamper hamper, string name, int _nextX, int _nextY, out VvTextBox tbx, int dbci, bool isDecimal, out VvTextBox tbxJM, int dbciJM)
   {
      hamper = new VvHamper(3, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp); ;
 
      hamper.VvColWdt    = new int[] { labelWidth, ZXC.Q2un + ZXC.Qun4 -ZXC.Qun12, ZXC.Q2un-ZXC.Qun2 };
      hamper.VvSpcBefCol = new int[] {   ZXC.Qun4,                       ZXC.Qun4,         ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;


              hamper.CreateVvLabel   (0, 0, name, ContentAlignment.MiddleRight);
      tbx   = hamper.CreateVvTextBox (1, 0, "tbx_"   + name, name, dbci);
      tbxJM = hamper.CreateVvTextBox (2, 0, "tbxJM_" + name, name, dbciJM);

      //28.09.2016.????
      //if(name.StartsWith("A"))  if(isDecimal) tbx.JAM_MarkAsNumericTextBox(3, false, decimal.MaxValue, 9999.999M, true);
      //else if(isDecimal)                      tbx.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, decimal.MinValue, true);
      tbx.JAM_MarkAsNumericTextBox(3, false, decimal.MaxValue, decimal.MinValue, true);

      
      if(name == "AZapremina:" && ZXC.IsPCTOGO == true) { hamper.Visible = false; }
      if(name == "ADuljina:"   && ZXC.IsPCTOGO == true) { hamper.Visible = false; }
   }

   private void Initialize_ArtiklCD2Hamper(out VvHamper hamper, string name, int _nextX, int _nextY) 
   {
      hamper = new VvHamper(4, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q10un + ZXC.QUN, ZXC.Q3un, ZXC.Q10un + ZXC.QUN};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

                      hamper.CreateVvLabel  (0, 0, ZXC.IsSvDUH ? "AATK:" : "AŠifra2:", ContentAlignment.MiddleRight);
      tbx_artiklCD2 = hamper.CreateVvTextBox(1, 0, "tbx_artiklCD2", "Šifra artikla2", GetDB_ColumnSize(DB_ci.artiklCD2));
      
                        hamper.CreateVvLabel  (2, 0, ZXC.IsSvDUH ? "AGenerika:" : name, ContentAlignment.MiddleRight);
      tbx_artiklName2 = hamper.CreateVvTextBox(3, 0, "tbx_artiklName2", "Naziv artikla2", GetDB_ColumnSize(DB_ci.artiklName2));

   }

   private void Initialize_PartNoHamper(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(2, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q10un + ZXC.QUN};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

                   hamper.CreateVvLabel  (0, 0, hamper.Name, ContentAlignment.MiddleRight);
      tbx_partNo = hamper.CreateVvTextBox(1, 0, "tbx_partNo", "PartNo", GetDB_ColumnSize(DB_ci.partNo));

   }

   private void Initialize_CarTarifaHamper(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(2, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q10un + ZXC.QUN};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

                      hamper.CreateVvLabel  (0, 0, hamper.Name, ContentAlignment.MiddleRight);
      tbx_carTarifa = hamper.CreateVvTextBox(1, 0, "tbx_CarTarifa", "Carinska Tarifa", GetDB_ColumnSize(DB_ci.carTarifa));

      hamper.Visible = !ZXC.IsPCTOGO;
   }

   private void Initialize_DobavljacHamper(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(4, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun2, ZXC.Q6un - ZXC.Qun2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

                     hamper.CreateVvLabel  (0, 0, name, ContentAlignment.MiddleRight);
      tbx_dobCd    = hamper.CreateVvTextBox(1, 0, "tbx_dobCd   ", "Šifra dobavljača" , GetDB_ColumnSize(DB_ci.dobavCD));
      tbx_dobTick  = hamper.CreateVvTextBox(2, 0, "tbx_dobTick ", "Ticker dobavljača",  6);
      tbx_dobNaziv = hamper.CreateVvTextBox(3, 0, "tbx_dobNaziv", "Naziv dobavljača" ,  32);
      
      tbx_dobCd.JAM_CharEdits   = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_dobTick.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_dobCd   .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyDobTextBoxLeave));
      tbx_dobTick .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyDobTextBoxLeave));
      tbx_dobNaziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyDobTextBoxLeave));

   }
   
   private void AnyDobTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob  kupdob_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;
         
         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_DobavCD  = kupdob_rec.KupdobCD/*RecID*/;
            Fld_DobTick  = kupdob_rec.Ticker;
            Fld_DobNaziv = kupdob_rec.Naziv;
         }
         else
         {
            Fld_DobCdAsTxt = Fld_DobTick = Fld_DobNaziv = "";
         }
      }
   }

   private void Initialize_ProizvodjacHamper(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(4, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun2, ZXC.Q6un - ZXC.Qun2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

                        hamper.CreateVvLabel  (0, 0, name, ContentAlignment.MiddleRight);
      tbx_proizvCd    = hamper.CreateVvTextBox(1, 0, "tbx_proizvCd   ", "Šifra proizvodjaca" , GetDB_ColumnSize(DB_ci.proizCD));
      tbx_proizvTick  = hamper.CreateVvTextBox(2, 0, "tbx_proizvTick ", "Ticker proizvodjaca",/* GetDB_ColumnSize(DB_ci.artiklName2)*/ 6);
      tbx_proizvNaziv = hamper.CreateVvTextBox(3, 0, "tbx_proizvNaziv", "Naziv proizvodjaca",/* GetDB_ColumnSize(DB_ci.artiklName2)*/ 32);
      
      tbx_proizvCd.JAM_CharEdits   = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_proizvTick.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_proizvCd   .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyProizvTextBoxLeave));
      tbx_proizvTick .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyProizvTextBoxLeave));
      tbx_proizvNaziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyProizvTextBoxLeave));
   }

   private void AnyProizvTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob  kupdob_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;
         
         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_ProizCD     = kupdob_rec.KupdobCD/*RecID*/;
            Fld_ProizvTick  = kupdob_rec.Ticker;
            Fld_ProizvNaziv = kupdob_rec.Naziv;
         }
         else
         {
            Fld_ProizvCdAsTxt = Fld_ProizvTick = Fld_ProizvNaziv = "";
         }
      }
   }

   private void Initialize_DrzPorjeklaHamper(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(3, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q6un - ZXC.Qun4, ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;      
      
                        hamper.CreateVvLabel  (0, 0, name, ContentAlignment.MiddleRight);
      tbx_drzPorjekla = hamper.CreateVvTextBox(1, 0, "tbx_drzPorjekla", "DrzPorjekla", GetDB_ColumnSize(DB_ci.madeIn));
      
   }

   private void Initialize_BarCode2Hamper (out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(2, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q10un + ZXC.QUN};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;      
      
                     hamper.CreateVvLabel  (0, 0, name, ContentAlignment.MiddleRight);
      tbx_barCode2 = hamper.CreateVvTextBox(1, 0, "tbx_barCode2", "BarCode2", GetDB_ColumnSize(DB_ci.barCode2));
      
   }
   
   private void Initialize_SerNoHamper    (out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(2, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q10un + ZXC.QUN};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

                  hamper.CreateVvLabel  (0, 0, hamper.Name, ContentAlignment.MiddleRight);
      tbx_serNo = hamper.CreateVvTextBox(1, 0, "tbx_serNo", "Serijski broj", GetDB_ColumnSize(DB_ci.serNo));

   }
   
   private void Initialize_Grupa2CDHamper (out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(3, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un - ZXC.Qun2, ZXC.Q8un + ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun4,            ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

                       hamper.CreateVvLabel        (0, 0, name, ContentAlignment.MiddleRight);
      tbx_grupa2CD   = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_grupa2CD", "", GetDB_ColumnSize(DB_ci.grupa2CD));
      tbx_grupa2Opis = hamper.CreateVvTextBox      (2, 0, "tbx_grupa2Opis", "", 32);
      
      tbx_grupa2Opis.JAM_ReadOnly = true;
      tbx_grupa2CD.JAM_Set_LookUpTable(ZXC.luiListaGrupa2Artikla, (int)ZXC.Kolona.prva);
      tbx_grupa2CD.JAM_lui_NameTaker_JAM_Name = tbx_grupa2Opis.JAM_Name;

      hamper.Visible = !ZXC.IsPCTOGO;
   }

   private void Initialize_Grupa3CDHamper (out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(3, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un - ZXC.Qun2, ZXC.Q8un + ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun4,            ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

                       hamper.CreateVvLabel        (0, 0, name, ContentAlignment.MiddleRight);
      tbx_grupa3CD   = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_grupa3CD", "", GetDB_ColumnSize(DB_ci.grupa3CD));
      tbx_grupa3Opis = hamper.CreateVvTextBox      (2, 0, "tbx_grupa3Opis", "", 32);
      tbx_grupa3Opis.JAM_ReadOnly = true;
      tbx_grupa3CD.JAM_Set_LookUpTable(ZXC.luiListaGrupa3Artikla, (int)ZXC.Kolona.prva);
      tbx_grupa3CD.JAM_lui_NameTaker_JAM_Name = tbx_grupa3Opis.JAM_Name;

      hamper.Visible = !ZXC.IsPCTOGO;

   }

   private void Initialize_PlacementHamper(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(4, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q6un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.Q4un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun4,           ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

      string tekst = ZXC.IsSvDUH ? "AUgovor:" : "ASmještaj:";

                      hamper.CreateVvLabel  (0, 0, tekst, ContentAlignment.MiddleRight);
      tbx_placement = hamper.CreateVvTextBox(1, 0, "tbx_placement", "Smještaj", GetDB_ColumnSize(DB_ci.placement));

      if(ZXC.IsSvDUH)
      {
         tbx_placement.JAM_ReadOnly = true;

         btn_proj = hamper.CreateVvButton(2, 0, new EventHandler(GoToProjektCD_RISK_Dokument_Click), "");
         btn_proj.Name = "projekt";
         btn_proj.FlatStyle = FlatStyle.Flat;
         btn_proj.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
         btn_proj.Image = VvIco.TriangleBlue16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue2.ico")), 16, 16)*/.ToBitmap();
         btn_proj.Tag = 10;
         btn_proj.TabStop = false;

      }

   }
   public void GoToProjektCD_RISK_Dokument_Click(object sender, EventArgs e)
   {
      string tt;
      uint ttNum;

      Ftrans.ParseTipBr(Fld_Placement, out tt, out ttNum);

      if(tt.IsEmpty()) return;

      FakturDUC.GoTo_RISK_Dokument(tt, ttNum);

   }

   private void Initialize_LinkArtCDHamper(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(3, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q2un + ZXC.QUN, ZXC.Q8un - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,           ZXC.Qun4,            ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

                        hamper.CreateVvLabel  (0, 0, name, ContentAlignment.MiddleRight);
      tbx_linkArtCD   = hamper.CreateVvTextBox(1, 0, "tbx_linkArtCD"  , "Vezni artikl", GetDB_ColumnSize(DB_ci.linkArtCD));
      tbx_linkArtName = hamper.CreateVvTextBox(2, 0, "tbx_linkArtName", "Vezni artikl", GetDB_ColumnSize(DB_ci.artiklName));
      tbx_linkArtName.JAM_ReadOnly = true;

      tbx_linkArtCD  .JAM_SetAutoCompleteData(Artikl.recordName, Artikl.sorterCD.SortType  , new EventHandler(OnVvTBEnter_SetAutocmplt_Artikl_sorterSifra), new EventHandler(AnyArtiklTextBox_Leave));
   }
    
   private void AnyArtiklTextBox_Leave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Artikl  artikl_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         artikl_rec = ArtiklSifrar.Find(FoundInSifrar<Artikl>);

         if(artikl_rec != null && tb.Text != "")
         {
            Fld_LinkArtCD   = artikl_rec.ArtiklCD;
            Fld_LinkArtName = artikl_rec.ArtiklName;
         }
         else
         {
            Fld_LinkArtCD = Fld_LinkArtName = "";
         }
      }
   }

   private void Initialize_DateProizHamper(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(2, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q4un-ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

                       hamper.CreateVvLabel        (0, 0, name, ContentAlignment.MiddleRight);
      tbx_dateProizv = hamper.CreateVvTextBox      (1, 0, "tbx_dateProizv", "Datum proizvodnje", GetDB_ColumnSize(DB_ci.dateProizv));
      tbx_dateProizv.JAM_IsForDateTimePicker_YearOnly = true;
      tbx_dateProizv.TextAlign = HorizontalAlignment.Left;

      dtp_dateProizv = hamper.CreateVvDateTimePicker(1, 0, "", tbx_dateProizv);
      dtp_dateProizv.Name = "dtp_dateProizv";
      dtp_dateProizv.ShowUpDown = true;
      dtp_dateProizv.MaxDate = DateTime.Now.AddDays(1); //ZXC.projectYearLastDay;

   }
   
   private void Initialize_PrefValNameHamp(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(2, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q4un - ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
     
      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

                        hamper.CreateVvLabel  (0, 0, name, ContentAlignment.MiddleRight);
      tbx_prefValName = hamper.CreateVvTextBox(1, 0, "tbx_prefValName", "", GetDB_ColumnSize(DB_ci.prefValName));
   }
   
   private void Initialize_OrgPakHamper   (out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(2, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q4un -ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
   
      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

                   hamper.CreateVvLabel  (0, 0, name, ContentAlignment.MiddleRight);
      tbx_orgPak = hamper.CreateVvTextBox(1, 0, "tbx_orgPak", "", GetDB_ColumnSize(DB_ci.orgPak));

   }

   private void Initialize_GarRokHamp(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(2, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q4un -ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
     
      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

                   hamper.CreateVvLabel  (0, 0, name, ContentAlignment.MiddleRight);
      tbx_garRok = hamper.CreateVvTextBox(1, 0, "tbx_garRok", "", GetDB_ColumnSize(DB_ci.garancija));
   }

   private void Initialize_AtestBrHamp(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(2, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q10un + ZXC.QUN};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
     
      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

      string text = ZXC.IsSvDUH ? "AHalmed:" : name;
    //hamper.CreateVvLabel(0, 0, name, ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 0, text, ContentAlignment.MiddleRight);

      if(ZXC.IsSvDUH)
      {
         tbx_atestBr  = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_atestBr", "", GetDB_ColumnSize(DB_ci.atestBr));
         tbx_atestBr.JAM_Set_LookUpTable(ZXC.luiListaRiskZiroRn, (int)ZXC.Kolona.prva);
         tbx_atestBr.JAM_lookUp_NOTobligatory = true;
      }
      else
      { 
         tbx_atestBr = hamper.CreateVvTextBox(1, 0, "tbx_atestBr", "" , GetDB_ColumnSize(DB_ci.atestBr));
      }

   }

   private void Initialize_AtestDateHamp(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(3, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q4un, ZXC.Q7un - ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

                      hamper.CreateVvLabel        (0, 0, name, ContentAlignment.MiddleRight);
      tbx_atestDate = hamper.CreateVvTextBox(1, 0, "tbx_atestDate", "Datum atesta", GetDB_ColumnSize(DB_ci.atestDate));
      tbx_atestDate.JAM_IsForDateTimePicker = true;
      dtp_atestDate = hamper.CreateVvDateTimePicker(1, 0, "", tbx_atestDate);
      dtp_atestDate.Name = "dtp_atestDate";
   }

   private void Initialize_UrlHamper(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(3, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

    //hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q10un + ZXC.QUN                               };
      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q9un + ZXC.QUN + ZXC.Qun4, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, 0};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Name = name;
      hamper.VvInitialHamperLocation  = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;

                hamper.CreateVvLabel  (0, 0, hamper.Name, ContentAlignment.MiddleRight);
      tbx_url = hamper.CreateVvTextBox(1, 0, "tbx_url", "url", GetDB_ColumnSize(DB_ci.url));

      btn_openExLinkURL = hamper.CreateVvButton(2, 0, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLinkURL.Name = "btn_openExLinkURL";
      btn_openExLinkURL.FlatStyle = FlatStyle.Flat;
      btn_openExLinkURL.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLinkURL.Image = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLinkURL.Tag = 1;
      btn_openExLinkURL.TabStop = false;

   }

   private void Initialize_IsAkcijaHamper(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(1, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q7un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
   
      hamper.Name = name;
      hamper.VvInitialHamperLocation = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable = true;
     
      string text  = name.TrimEnd(':').Substring(1);
      cbx_isAkcija = hamper.CreateVvCheckBox_OLD(0, 0, null, text, System.Windows.Forms.RightToLeft.Yes );
    }

   private void Initialize_IsMasterHamper(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(2, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q7un, ZXC.Q7un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
  
      hamper.Name = name;
      hamper.VvInitialHamperLocation = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable         = true;

      string text  = name.TrimEnd(':').Substring(1);
      cbx_isMaster = hamper.CreateVvCheckBox_OLD(0, 0, null, text, System.Windows.Forms.RightToLeft.Yes);

   }

   private void Initialize_IsSerBrHamper(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(1, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q7un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
  
      hamper.Name = name;
      hamper.VvInitialHamperLocation = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable         = true;

      string text     = name.TrimEnd(':').Substring(1);
      cbx_isSerBr = hamper.CreateVvCheckBox_OLD(0, 0, null, text, System.Windows.Forms.RightToLeft.Yes);
   }

   private void Initialize_IsDozvMinusHamper(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(1, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q7un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
  
      hamper.Name = name;
      hamper.VvInitialHamperLocation = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable         = true;

      string text = name.TrimEnd(':').Substring(1);
      cbx_isDozvMinus = hamper.CreateVvCheckBox_OLD(0, 0, null, text, System.Windows.Forms.RightToLeft.Yes);
   }

   private void Initialize_PrintOpisHamper(out VvHamper hamper, string name, int _nextX, int _nextY)
   {
      hamper = new VvHamper(1, 1, "", MigratorRightParentA, false, _nextX, _nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q7un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
  
      hamper.Name = name;
      hamper.VvInitialHamperLocation = new Point(_nextX, _nextY);
      hamper.VvIsMigrateable         = true;

      string text   = name.TrimEnd(':').Substring(1);
      cbx_isPrnOpis = hamper.CreateVvCheckBox_OLD(0, 0, null, text, System.Windows.Forms.RightToLeft.Yes);
   }

   #endregion CreateMigratingHampers

   #region Rtrans DataGridView

   private void CreateDataGridView_InitializeTheGrid_ReadOnly_Columns()
   {
      aTransesGrid[0]      = CreateDataGridView_ReadOnly(TheTabControl.TabPages[rtrans_TabPageName], "Rtrans");
      aTransesGrid[0].Dock = DockStyle.Fill;
      InitializeTheGrid_ReadOnly_Columns_Rtrans();

      aTransesGrid[0].DoubleClick += new EventHandler(theFIRST_TransGrid_DoubleClick);
      aTransesGrid[0].KeyPress    += new KeyPressEventHandler(theFIRST_TransGrid_KeyPress);
   }

   protected override void theFIRST_TransGrid_DoubleClick(object sender, EventArgs e)
   {
    //OpenNew_Record_TabPage_OnDoubleClick(ZXC.VvSubModulEnum.R_URA, SelectedRecIDIn_FIRST_TransGrid);
      OpenNew_Record_TabPage_OnDoubleClick(TheVvTabPage.TheVvForm.GetVvSubModulEnumFrom_SubModulShortName(SelectedTTin_FIRST_TransGrid), SelectedRecIDIn_FIRST_TransGrid);
   }

   private void InitializeTheGrid_ReadOnly_Columns_Rtrans()
   {
      AddDGVColum_RecID_4GridReadOnly   (aTransesGrid[0], "RecID"  , colSif6Width, false, 0);
      AddDGVColum_String_4GridReadOnly  (aTransesGrid[0], "TT"     , ZXC.Q2un, false);
      AddDGVColum_Integer_4GridReadOnly (aTransesGrid[0], "TTnum"  , colSif6Width - ZXC.Qun2, true, 6);
      AddDGVColum_Integer_4GridReadOnly (aTransesGrid[0], "Red"    , ZXC.Q2un + ZXC.Qun8, false, 0);
      AddDGVColum_DateTime_4GridReadOnly(aTransesGrid[0], "Datum"  , colDateWidth - ZXC.Qun2 - ZXC.Qun4);
      AddDGVColum_String_4GridReadOnly  (aTransesGrid[0], "Sklad"  , ZXC.Q2un, false);
      AddDGVColum_String_4GridReadOnly  (aTransesGrid[0], "Partner", ZXC.Q10un, true);
   // AddDGVColum_Integer_4GridReadOnly (aTransesGrid[0], "Partner", ZXC.Q10un, true, 6);
      AddDGVColum_Integer_4GridReadOnly (aTransesGrid[0], "BrDok"  , colSif6Width - ZXC.Qun2, true, 6);
      AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[0], "Kol"    , ZXC.Q3un - ZXC.Qun8, 2);
      AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[0], "Cijena" , ZXC.Q3un, 2);
      AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[0], "Iznos"  , ZXC.Q4un, 2);

   }

   #endregion DataGridView

   #region Filter

   public override void CreateRptFilterAndRptFilterUC()
   {
      TheArtiklFilter   = new ArtiklCardFilter(this);
      TheArtiklFilterUC = new ArtiklSifrarFilterUC(this);

      TheArtiklFilterUC.Parent                  = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheArtiklFilterUC.Width;

   }

   #endregion Filter

   #region Fld_

   /* 05 */ public string Fld_ArtiklCd
      {
         get { return tbx_artiklCD.Text; }
         set {        tbx_artiklCD.Text = value; }
      }
   /* 06 */ public string Fld_ArtiklName
      {
         get { return tbx_artiklName.Text; }
         set {        tbx_artiklName.Text = value; }
      }
   /* 07 */ public string Fld_BarCode1      
      {
         get 
         { 

            return tbx_barCode1.Text; 
         }
         set {        tbx_barCode1.Text = value; }
      }
   /* 08 */ public string Fld_UobSkladCD      
      {
         get { return tbx_skladCD.Text; }
         set {        tbx_skladCD.Text = value; }
      }
   /*    */ public string Fld_SkladOpis
      {
         set { tbx_skladOpis.Text = value; }
      }
   /* 09 */ public string Fld_Grupa1CD      
      {
         get { return tbx_grupa1CD.Text; }
         set {        tbx_grupa1CD.Text = value; }
      }
   /*    */ public string Fld_Grupa1Opis
      {
         set { tbx_grupa1Opis.Text = value; }
      }
   /* 10 */ public string Fld_JedMj      
      {
         get { return tbx_jedMj.Text; }
         set {        tbx_jedMj.Text = value; }
      }        
   /* 11 */ public string Fld_Konto      
      {
         get { return tbx_konto.Text; }
         set {        tbx_konto.Text = value; }
      }
   /* 12 */ public string Fld_ArtiklCd2
      {
         get { return tbx_artiklCD2.Text; }
         set {        tbx_artiklCD2.Text = value; }
      }
   /* 13 */ public string Fld_ArtiklName2
      {
         get { return tbx_artiklName2.Text; }
         set {        tbx_artiklName2.Text = value; }
      }
   /* 14 */ public string Fld_Ts
      {
         get { return tbx_ts.Text; }
         set {        tbx_ts.Text = value; }
      }
   /*    */ public string Fld_TsOpis
   {
      set { tbx_tsOpis.Text = value; }
   }
   /* 15 */ public string Fld_BarCode2
      {
         get { return tbx_barCode2.Text; }
         set {        tbx_barCode2.Text = value; }
      }
   /* 16 */ public string Fld_SerNo       
      {
         get { return tbx_serNo.Text; }
         set {        tbx_serNo.Text = value; }
      }
   /* 17 */ public string Fld_Grupa2CD     
      {
         get { return tbx_grupa2CD.Text; }
         set {        tbx_grupa2CD.Text = value; }
      }
   /*    */public string Fld_Grupa2Opis
      {
         set { tbx_grupa2Opis.Text = value; }
      }
   /* 18 */public string Fld_Grupa3CD     
      {
         get { return tbx_grupa3CD.Text; }
         set {        tbx_grupa3CD.Text = value; }
      }
   /*    */public string Fld_Grupa3Opis
      {
         set { tbx_grupa3Opis.Text = value; }
      }
   /* 19 */public string Fld_Placement    
      {
         get { return tbx_placement.Text; }
         set {        tbx_placement.Text = value; }
      }
   /* 20 */
   public string Fld_LinkArtCD
   {
      get { return tbx_linkArtCD.Text; }
      set { tbx_linkArtCD.Text = value; }
   }
   public string Fld_LinkArtName    
      {
         set { tbx_linkArtName.Text = value; }
      }
   /* 21 */public DateTime Fld_DateProizv
      {
         get
         {
            DateTime tmpDate = ZXC.ValOr_01010001_DtpDateTime(dtp_dateProizv.Value);
            return new DateTime(tmpDate.Year, 1, 1);
         }
         set
         {
            dtp_dateProizv.Value = ZXC.ValOr_01011753_DateTime(value);
            tbx_dateProizv.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_dateProizv);
         }
      }
   /* 22 */public string Fld_PdvKat       
      {
         get { return tbx_pdvKat.Text; }
         set {        tbx_pdvKat.Text = value; }
      }
   /*    */public string Fld_PdvKatOpis
      {
         set { tbx_pdvKatOpis.Text = value; }
      }
   /* 23 */public string Fld_LongOpis     
      {
         get { return tbx_longOpis.Text; }
         set {        tbx_longOpis.Text = value; }
      }
   /* 24 */public string Fld_PrefValName      
      {
         get { return tbx_prefValName.Text; }
         set {        tbx_prefValName.Text = value; }
      }
   /* 25 */public string Fld_OrgPak       
      {
         get { return tbx_orgPak.Text; }
         set {        tbx_orgPak.Text = value; }
      }
   /* 26 */public bool Fld_IsRashod     
      {
         get { return cbx_isRashod.Checked; }
         set {        cbx_isRashod.Checked = value; }
      }
   /* 27 */public bool Fld_IsAkcija     
      {
         get { return cbx_isAkcija.Checked; }
         set {        cbx_isAkcija.Checked = value; }
      }
   /* 28 */public bool Fld_IsMaster     
      {
         get { return cbx_isMaster.Checked; }
         set {        cbx_isMaster.Checked = value; }
      }
   /* 29 */ public decimal Fld_MasaNetto
      {
         get { return tbx_masaNetto.GetDecimalField(); }
         set {        tbx_masaNetto.PutDecimalField(value); }
      }
   /* 30 */public decimal Fld_MasaBruto
      {
         get { return tbx_masaBruto.GetDecimalField(); }
         set {        tbx_masaBruto.PutDecimalField(value); }
      }
   /* 31 */public decimal Fld_Promjer
      {
         get { return tbx_promjer.GetDecimalField(); }
         set {        tbx_promjer.PutDecimalField(value); }
      }
   /* 32 */public decimal Fld_Povrsina
      {
         get { return tbx_povrsina.GetDecimalField(); }
         set {        tbx_povrsina.PutDecimalField(value); }
      }
   /* 33 */public decimal Fld_Zapremina
      {
         get { return tbx_zapremina.GetDecimalField(); }
         set {        tbx_zapremina.PutDecimalField(value); }
      }
   /* 34 */public decimal Fld_Duljina
      {
         get { return tbx_duljina.GetDecimalField(); }
         set {        tbx_duljina.PutDecimalField(value); }
      }
   /* 35 */public decimal Fld_Sirina
      {
         get { return tbx_sirina.GetDecimalField(); }
         set {        tbx_sirina.PutDecimalField(value); }
      }
   /* 36 */public decimal Fld_Visina
      {
         get { return tbx_visina.GetDecimalField(); }
         set {        tbx_visina.PutDecimalField(value); }
      }
   /* 37 */public decimal Fld_Starost
      {
         get { return tbx_starost.GetDecimalField(); }
         set {        tbx_starost.PutDecimalField(value); }
      }
   /* 38 */public string Fld_Boja
      {
         get { return tbx_boja.Text; }
         set {        tbx_boja.Text = value; }
      }
   /* 39 */public ushort Fld_Spol
      {
         get { return tbx_spol.GetUshortField(); }
         set {        tbx_spol.PutUshortField(value); }
      }
   /* 40 */public ushort Fld_Garancija
   {
      get { return tbx_garRok.GetUshortField(); }
      set {        tbx_garRok.PutUshortField(value); }
   }
   /* 41 */public uint   Fld_DobavCD
      {
         get { return tbx_dobCd.GetSomeRecIDField();      }
         set {        tbx_dobCd.PutSomeRecIDField(value); }
      }
   /*    */public string Fld_DobCdAsTxt
      {
         get { return tbx_dobCd.Text;         }
         set {        tbx_dobCd.Text = value; }
      }
   /*    */public string Fld_DobNaziv
      {
         get { return tbx_dobNaziv.Text;         }
         set {        tbx_dobNaziv.Text = value; }
      }
   /*    */public string Fld_DobTick
      {
         get { return tbx_dobTick.Text;         }
         set {        tbx_dobTick.Text = value; }
      }
   /* 42 */public uint   Fld_ProizCD
      {
         get { return tbx_proizvCd.GetSomeRecIDField();      }
         set {        tbx_proizvCd.PutSomeRecIDField(value); }
      }
   /*    */public string Fld_ProizvCdAsTxt
      {
         get { return tbx_proizvCd.Text;         }
         set {        tbx_proizvCd.Text = value; }
      }
   /*    */public string Fld_ProizvNaziv
      {
         get { return tbx_proizvNaziv.Text;         }
         set {        tbx_proizvNaziv.Text = value; }
      }
   /*    */public string Fld_ProizvTick
      {
         get { return tbx_proizvTick.Text;         }
         set {        tbx_proizvTick.Text = value; }
      }
   ///* 43 */public decimal Fld_Marza
   //   {
   //      get { return tbx_marza.GetDecimalField(); }
   //      set {        tbx_marza.PutDecimalField(value); }
   //   }
   /* 44 */public bool    Fld_IsAllowMinus  
     {
         get { return cbx_isDozvMinus.Checked; }
         set {        cbx_isDozvMinus.Checked = value; }
      }
   /* 45 */public bool     Fld_IsSerNo
   {
      get { return cbx_isSerBr.Checked; }
      set {        cbx_isSerBr.Checked = value; }
   }
   /* 46 */public string Fld_MasaNettoJM
   {
      get { return tbx_masaNettoJM.Text; }
      set {        tbx_masaNettoJM.Text = value; }
   }
   /* 47 */public string Fld_MasaBrutoJM
   {
      get { return tbx_masaBrutoJM.Text; }
      set {        tbx_masaBrutoJM.Text = value; }
   }
   /* 48 */public string Fld_PromjerJM
   {
      get { return tbx_promjerJM.Text; }
      set {        tbx_promjerJM.Text = value; }
   }
   /* 49 */public string Fld_PovrsinaJM
   {
      get { return tbx_povrsinaJM.Text; }
      set {        tbx_povrsinaJM.Text = value; }
   }
   /* 50 */public string Fld_ZapreminaJM
   {
      get { return tbx_zapreminaJM.Text; }
      set {        tbx_zapreminaJM.Text = value; }
   }
   /* 51 */public string Fld_DuljinaJM
   {
      get { return tbx_duljinaJM.Text; }
      set {        tbx_duljinaJM.Text = value; }
   }
   /* 52 */public string Fld_SirinaJM
   {
      get { return tbx_sirinaJM.Text; }
      set {        tbx_sirinaJM.Text = value; }
   }
   /* 53 */public string Fld_VisinaJM
   {
      get { return tbx_visinaJM.Text; }
      set {        tbx_visinaJM.Text = value; }
   } 
   /* 54 */ public string   Fld_Velicina
   { 
      get { return tbx_velicina.Text; }
      set {         tbx_velicina.Text = value; }
   } 
   /* 55 */ public string   Fld_MadeIn
   {
      get { return tbx_drzPorjekla.Text; }
      set {        tbx_drzPorjekla.Text = value; }
   } 
   /* 56 */ public string   Fld_Url
   {
      get { return tbx_url.Text; }
      set {        tbx_url.Text = value; }
   } 
   /* 57 */ public string   Fld_AtestBr
   {
      get { return tbx_atestBr.Text; }
      set {        tbx_atestBr.Text = value; }
   } 
   /* 58 */ public DateTime Fld_AtestDate
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_atestDate.Value);
      }
      set
      {
         dtp_atestDate.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_atestDate.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_atestDate);
      }
   }


   /*  */public string Fld_ZaSkladCD
   {
      get 
      {
         ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD = tbx_zaSkladCD.Text;
         TheArtiklFilterUC.Fld_Sklad      = tbx_zaSkladCD.Text;

         return                             tbx_zaSkladCD.Text; 
      }
      set 
      {
         //ZXC.TheVvForm.theLastUsedSkladCD = value;
         tbx_zaSkladCD.Text = value;
         TheArtiklFilterUC.Fld_Sklad = value;
      }
   }
   /*  */public string Fld_ZaSkladOpis
   {
      set { tbx_zaSkladOpis.Text = value; }
   }

   /* 59 */public ZXC.ArtiklVpc1Policy Fld_Vpc1Policy
   {
      get
      {
              if(rbt_vpc1 .Checked) return ZXC.ArtiklVpc1Policy.ZADANO;
         else if(rbt_marza.Checked) return ZXC.ArtiklVpc1Policy.MARZA;

         else throw new Exception("Fld_VrstaRadOdns: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.ArtiklVpc1Policy.ZADANO: rbt_vpc1 .Checked = true; break;
            case ZXC.ArtiklVpc1Policy.MARZA : rbt_marza.Checked = true; break;
         }
      }
   }
   /// <summary>
   /// Na racunu printaj opis umjesto naziva artikla
   /// </summary>
   public bool Fld_IsPrnOpis
   {
      get { return cbx_isPrnOpis.Checked; }
      set {        cbx_isPrnOpis.Checked = value; }
   }

   public DateTime Fld_NaDan
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_naDan.Value);
      }
      set
      {
         dtp_naDan.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_naDan.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_naDan);
      }
   }

   /* 60 */ public string Fld_CarTarifa
   {
      get { return tbx_carTarifa.Text; }
      set {        tbx_carTarifa.Text = value; }
   } 

   /* 61 */ public string Fld_PartNo
   {
      get { return tbx_partNo.Text; }
      set {        tbx_partNo.Text = value; }
   }

   /* 62 */
   public string Fld_Napomena
   {
      get { return tbx_napomena.Text; }
      set {        tbx_napomena.Text = value; }
   }
   /* 63 */
   public decimal Fld_ImportCij
   {
      get { return tbx_importCij.GetDecimalField(); }
      set {        tbx_importCij.PutDecimalField(value); }
   }

 
   public decimal Fld_Snaga
   {
         get { return tbx_snaga.GetDecimalField(); }
         set {        tbx_snaga.PutDecimalField(value); }
   }
   public string Fld_SnagaJM
   {
      get { return tbx_snagaJM.Text; }
      set {        tbx_snagaJM.Text = value; }
   }

   public ushort Fld_EmisCO2
   {
      get { return tbx_emisCo2.GetUshortField(); }
      set {        tbx_emisCo2.PutUshortField(value); }
   }
   
   public ZXC.EuroNormaEnum Fld_EuroNorma
   {
      get
      {
         switch(tbx_euroNorma.Text)
         {
            case "1": return ZXC.EuroNormaEnum.EuroI  ;
            case "2": return ZXC.EuroNormaEnum.EuroII ;
            case "3": return ZXC.EuroNormaEnum.EuroIII;
            case "4": return ZXC.EuroNormaEnum.EuroIV ;
            case "5": return ZXC.EuroNormaEnum.EuroV  ;
            case "6": return ZXC.EuroNormaEnum.EuroVI ;

            default: return ZXC.EuroNormaEnum.NIJEDNA;
         }
      }
      set
      {
         switch(value)
         {
            case ZXC.EuroNormaEnum.EuroI  : tbx_euroNorma.Text = "1"; break;
            case ZXC.EuroNormaEnum.EuroII : tbx_euroNorma.Text = "2"; break;
            case ZXC.EuroNormaEnum.EuroIII: tbx_euroNorma.Text = "3"; break;
            case ZXC.EuroNormaEnum.EuroIV : tbx_euroNorma.Text = "4"; break;
            case ZXC.EuroNormaEnum.EuroV  : tbx_euroNorma.Text = "5"; break;
            case ZXC.EuroNormaEnum.EuroVI : tbx_euroNorma.Text = "6"; break;
            default: tbx_euroNorma.Text = ""; break;
         }
      }
   }

   #endregion Fld_

   #region ResultFld_r

   public decimal Fld_rPsKolSt
   {
      set { tbx_psKolSt.PutDecimalField(value); }
   }
   public decimal Fld_rPsNabCij
   {
      set { tbx_psNabCij.PutDecimalField(value); }
   }
   public decimal Fld_rPsFinSt
   {
      set { tbx_psFinSt.PutDecimalField(value); }
   }
   public decimal Fld_rKolSt
   {
      set { tbx_kolSt.PutDecimalField(value); }
   }
   public decimal Fld_rFinSt
   {
      set { tbx_finSt.PutDecimalField(value); }
   }
   public decimal Fld_rVPC1
   {
      set { tbx_cjenikVpc1.PutDecimalField(value); }
   }
   public decimal Fld_rVPC2
   {
      set { tbx_VPC2.PutDecimalField(value); }
   }
   public decimal Fld_rMPC
   {
      set { tbx_MPC1.PutDecimalField(value); }
   }
   public decimal Fld_rDevC
   {
      set { tbx_devC.PutDecimalField(value); }
   } 
   public decimal Fld_rUlMinCij
   {
      set { tbx_ulMinCij.PutDecimalField(value); }
   }
   public decimal Fld_rUlMaxCij
   {
      set { tbx_ulMaxCij.PutDecimalField(value); }
   }
   public decimal Fld_rUlLastCij
   {
      set { tbx_ulLastCij.PutDecimalField(value); }
   }
   public decimal Fld_rRuc
   {
      set { tbx_ruc.PutDecimalField(value); }
   }
   public decimal Fld_rRabat1
   {
      set { tbx_rabat1.PutDecimalField(value); }
   }
   public decimal Fld_rRabat2
   {
      set { tbx_rabat2.PutDecimalField(value); }
   }
   public decimal Fld_rMinKol
   {
      set { tbx_minKol.PutDecimalField(value); }
   }
   public decimal Fld_rInvKolSt
   {
      set { tbx_invKolSt.PutDecimalField(value); }
   }
   public decimal Fld_rInvFinSt
   {
      set { tbx_invFinSt.PutDecimalField(value); }
   }
   public decimal Fld_rUlazKol
   {
      set { tbx_ulazKol.PutDecimalField(value); }
   }
   public decimal Fld_rUlazFin
   {
      set { tbx_ulazFin.PutDecimalField(value); }
   }
   public decimal Fld_rIzlazKol
   {
      set { tbx_izlazKol.PutDecimalField(value); }
   }
   public decimal Fld_rIzlazFinNab
   {
      set { tbx_izlazFinNab.PutDecimalField(value); }
   }
   public decimal Fld_rIzlazFinNab2
   {
      set { tbx_izlazFinNab2.PutDecimalField(value); }
   }

   public decimal Fld_rIzlazFinProd
   {
      set { tbx_izlazFinProd.PutDecimalField(value); }
   }
   public decimal Fld_rIzlazRUV
   {
      set { tbx_izlazRUV.PutDecimalField(value); }
   }
   public decimal Fld_rIzlRezervKol
   {
      set { tbx_izlRezervKol.PutDecimalField(value); }
   }
   public decimal Fld_rKolStFree
   {
      set { tbx_KolStFree.PutDecimalField(value); }
   }
   //public decimal Fld_rPrProdCij
   //{
   //   set { tbx_prProdCij.PutDecimalField(value); }
   //}
   public decimal Fld_rRuvStopa
   {
      set { tbx_ruvPost.PutDecimalField(value); }
   }
   public decimal Fld_rStanjeKnjigCij
   {
      set { tbx_prNabCij.PutDecimalField(value); }
   }
   public decimal Fld_rStanjeKnjigCijOP
   {
      set { tbx_prNabCijOP.PutDecimalField(value); }
   }
   public decimal Fld_rIzlazPrNabCij
   {
      set { tbx_prNabCij2.PutDecimalField(value); }
   }

   public decimal Fld_rKolStSkl
   {
      set { tbx_SklKol.PutDecimalField(value); }
   }
   
   public decimal Fld_rPrNabCiju
   {
      set { tbx_prNabCijU.PutDecimalField(value); }
   }
   public decimal Fld_rUlazKolFisycal
   {
      set { tbx_uKolFisycal.PutDecimalField(value); }
   }
   public decimal Fld_rIzlazKolFisycal
   {
      set { tbx_iKolFisycal.PutDecimalField(value); }
   }
   public decimal Fld_rIzlazCijMin 
   {
      set { tbx_izlazCijMin.PutDecimalField(value); }
   }
   public decimal Fld_rIzlazCijMax
   {
      set { tbx_izlazCijMax.PutDecimalField(value); }
   }
   public decimal Fld_rIzlazCijLast
   {
      set { tbx_izlazCijLast.PutDecimalField(value); }
   }
   public decimal Fld_Marza
   {
      set { tbx_marza.PutDecimalField(value); }
   }

   public DateTime Fld_rDateZadUlaz
   {
      set
      {
         dtp_dateZadUlaz.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_dateZadUlaz.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_dateZadUlaz);
      }
   }
   public DateTime Fld_rDateZadIzlaz
   {
         set
         {
            dtp_dateZadIzlaz.Value = ZXC.ValOr_01011753_DateTime(value);
            tbx_dateZadIzlaz.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_dateZadIzlaz);
         }
   }
   public DateTime Fld_rDateZadInv
   {
         set
         {
            dtp_dateZadInv.Value = ZXC.ValOr_01011753_DateTime(value);
            tbx_dateZadInv.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_dateZadInv);
         }
   }
   public DateTime Fld_rDateZadnji
   {
      set
      {
         dtp_dateZadnji.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_dateZadnji.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_dateZadnji);
      }
   }
   public DateTime Fld_rDatePS
   {
      set
      {
         dtp_datePS.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_datePS.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_datePS);
      }
   }
   public decimal Fld_HelpNabC
   {
      set { tbx_HelpNabC.PutDecimalField(value); }
   }

   public string Fld_rFrsMinTT
   {
      get { return tbx_frsMinTT.Text; }
      set {        tbx_frsMinTT.Text = value; }
   }
   
   public uint Fld_rFrsMinTTnum
   {
      get { return ZXC.ValOrZero_UInt(tbx_frsMinTTnum.Text); }
      set {                           tbx_frsMinTTnum.Text = value.ToString(/*"00000"*/);  }
   }

   #endregion ResultFld_r

   #region PutFields(), GetFields()

   public override void PutFields(VvDataRecord artikl)
   {
      artikl_rec = (Artikl)artikl;

      if(artikl_rec != null)
      {
         PutMetaFileds(artikl_rec.AddUID, artikl_rec.AddTS, artikl_rec.ModUID, artikl_rec.ModTS, artikl_rec.RecID, artikl_rec.LanSrvID, artikl_rec.LanRecID);

         PutIdentityFields(artikl_rec.ArtiklCD, artikl_rec.ArtiklName, "", "");

         VvHamper.SetChkBoxRadBttnAutoCheck(this, true);

         PutDataLayerFields();

         #region Fld_ZaSkladCD & Fld_NaDan

         Fld_ZaSkladCD        = TheCurrentSkladCD;

         dtp_naDan.ValueChanged -= new EventHandler(dTP_naDan_ValueChanged);
         Fld_NaDan = ZXC.ValOr_01011753_DateTime(DateTime.Now);
         dtp_naDan.ValueChanged += new EventHandler(dTP_naDan_ValueChanged);

         #endregion Fld_ZaSkladCD & Fld_NaDan

       //PutResultFields(                     ArtiklDao.GetArtiklStatus(TheDbConnection, artikl_rec.ArtiklCD, TheCurrentSkladCD/*, true*/));
         PutResultFields(artikl_rec.TheAsEx = ArtiklDao.GetArtiklStatus(TheDbConnection, artikl_rec.ArtiklCD, TheCurrentSkladCD/*, true*/));

         VvHamper.SetChkBoxRadBttnAutoCheck(this, false);

         InitializeFilterUCFields();

         recordReportLoaded = false;
         DecideIfShouldLoad_VvReport(null, null, null);

         aTransesLoaded[0] = false; // ovdje treba nulirati sve postojece 'xyLoaded' varijable

         DecideIfShouldLoad_TransDGV(null, null, null);

         SetToolTipsForPredugackys();

       //PTG_PCKinfoLoaded = false;
       if(ZXC.IsPCTOGO)
       {
          //PTG_PCKinfoLoaded = false;
          DecideIfShouldLoad_PCKinfo(null, null, null);//03.07.2023

            if(artikl_rec.TS == ZXC.PCK_TS)
            {
               if(artikl_rec.PCK_RAM.IsZero()) tbx_zapremina.Text = "0";
               if(artikl_rec.PCK_HDD.IsZero()) tbx_duljina  .Text = "0";

               pcKInfoUC.currArtiklCD = artikl_rec.ArtiklCD;
               pcKInfoUC.currSkladCD  = Fld_ZaSkladCD      ;
            }
       }

         //Rtrans UGOrtrans_rec = new Rtrans();
         //bool found = FakturDao.SetMeLastRtransForArtiklAndTT(TheDbConnection, UGOrtrans_rec, Faktur.TT_IZD/*UGO*/, artikl_rec.ArtiklCD, true);
         //if(found) ZXC.aim_emsg("{0}", UGOrtrans_rec);
      }
   }

   private void PutDataLayerFields()
   {
      Kupdob kupdobSifrar_rec;

         /* 05 */ Fld_ArtiklCd    = artikl_rec.ArtiklCD;
         /* 06 */ Fld_ArtiklName  = artikl_rec.ArtiklName;
         /* 07 */ Fld_BarCode1    = artikl_rec.BarCode1;
         /* 08 */ Fld_UobSkladCD   = artikl_rec.SkladCD;
         /* 09 */ Fld_Grupa1CD    = artikl_rec.Grupa1CD;
         /* 10 */ Fld_JedMj       = artikl_rec.JedMj;
         /* 11 */ Fld_Konto       = artikl_rec.Konto;
         /* 12 */ Fld_ArtiklCd2   = artikl_rec.ArtiklCD2;
         /* 13 */ Fld_ArtiklName2 = artikl_rec.ArtiklName2;
         /* 14 */ Fld_Ts          = artikl_rec.TS       ; 
         /* 15 */ Fld_BarCode2    = artikl_rec.BarCode2  ;
         /* 16 */ Fld_SerNo       = artikl_rec.SerNo     ;
         /* 17 */ Fld_Grupa2CD    = artikl_rec.Grupa2CD  ;
         /* 18 */ Fld_Grupa3CD    = artikl_rec.Grupa3CD  ;
         /* 19 */ Fld_Placement   = artikl_rec.Placement ;
         /* 20 */ Fld_LinkArtCD   = artikl_rec.LinkArtCD ;
         /* 21 */ Fld_DateProizv  = artikl_rec.DateProizv;
         /* 22 */ Fld_PdvKat      = artikl_rec.PdvKat    ;
         /* 23 */ Fld_LongOpis    = artikl_rec.LongOpis  ;
         /* 24 */ Fld_PrefValName = artikl_rec.PrefValName;
         /* 25 */ Fld_OrgPak      = artikl_rec.OrgPak    ;
         /* 26 */ Fld_IsRashod    = artikl_rec.IsRashod  ;
         /* 27 */ Fld_IsAkcija    = artikl_rec.IsAkcija  ;
         /* 28 */ Fld_IsMaster    = artikl_rec.IsMaster  ;
         /* 29 */ Fld_MasaNetto = artikl_rec.MasaNetto;
         /* 30 */ Fld_MasaBruto = artikl_rec.MasaBruto;
         /* 31 */ Fld_Promjer   = artikl_rec.Promjer  ;
         /* 32 */ Fld_Povrsina  = artikl_rec.Povrsina ;
         /* 33 */ Fld_Zapremina = artikl_rec.Zapremina;
         /* 34 */ Fld_Duljina   = artikl_rec.Duljina  ;
         /* 35 */ Fld_Sirina    = artikl_rec.Sirina   ;
         /* 36 */ Fld_Visina    = artikl_rec.Visina   ;
         /* 37 */ Fld_Starost   = artikl_rec.Starost  ;
         /* 38 */ Fld_Boja      = artikl_rec.Boja     ;
         /* 39 */ Fld_Spol      = artikl_rec.Spol     ; 
         /* 40 */ Fld_Garancija     = artikl_rec.Garancija;   
         /* 41 */ Fld_DobavCD       = artikl_rec.DobavCD    ; 
         /* 42 */ Fld_ProizCD       = artikl_rec.ProizCD  ;   
         /* 43 */ //nekad bila marza
         /* 44 */ Fld_IsAllowMinus  = artikl_rec.IsAllowMinus;
         /* 45 */ Fld_IsSerNo       = artikl_rec.IsSerNo     ;
         /* 46 */ Fld_MasaNettoJM   = artikl_rec.MasaNettoJM ;
         /* 47 */ Fld_MasaBrutoJM   = artikl_rec.MasaBrutoJM ;
         /* 48 */ Fld_PromjerJM     = artikl_rec.PromjerJM   ;
         /* 49 */ Fld_PovrsinaJM    = artikl_rec.PovrsinaJM  ;
         /* 50 */ Fld_ZapreminaJM   = artikl_rec.ZapreminaJM ;
         /* 51 */ Fld_DuljinaJM     = artikl_rec.DuljinaJM   ;
         /* 52 */ Fld_SirinaJM      = artikl_rec.SirinaJM    ;
         /* 53 */ Fld_VisinaJM      = artikl_rec.VisinaJM    ;
         /* 54 */ Fld_Velicina      = artikl_rec.Velicina    ;
         /* 55 */ Fld_MadeIn        = artikl_rec.MadeIn      ;
         /* 56 */ Fld_Url           = artikl_rec.Url         ;
         /* 57 */ Fld_AtestBr       = artikl_rec.AtestBr     ;
         /* 58 */ Fld_AtestDate     = artikl_rec.AtestDate   ;
         /* 59 */ Fld_Vpc1Policy    = artikl_rec.Vpc1Policy  ;
         /* 60 */ Fld_IsPrnOpis     = artikl_rec.IsPrnOpis;
         /* 60 */ Fld_CarTarifa     = artikl_rec.CarTarifa; 
         /* 61 */ Fld_PartNo        = artikl_rec.PartNo; 
         /* 62 */ Fld_Napomena      = artikl_rec.Napomena; 
         /* 63 */ Fld_ImportCij     = artikl_rec.ImportCij; 
         /* 64 */ Fld_Snaga         = artikl_rec.Snaga; 
         /* 65 */ Fld_SnagaJM       = artikl_rec.SnagaJM; 
         /* 66 */ Fld_EmisCO2       = artikl_rec.EmisCO2  ; 
         /* 67 */ Fld_EuroNorma     = artikl_rec.EuroNorma; 


         //Fld_SkladOpis  = ZXC.luiListaSkladista    .GetNameForThisCd(artikl_rec.SkladCD);
         //Fld_Grupa1Opis = ZXC.luiListaGrupa1Artikla.GetNameForThisCd(artikl_rec.Grupa1CD);
         //Fld_PdvKatOpis = ZXC.luiListaPdvKat       .GetNameForThisCd(artikl_rec.PdvKat);
         //Fld_Grupa2Opis = ZXC.luiListaGrupa2Artikla.GetNameForThisCd(artikl_rec.Grupa2CD);
         //Fld_Grupa3Opis = ZXC.luiListaGrupa3Artikla.GetNameForThisCd(artikl_rec.Grupa3CD);
         //Fld_TsOpis     = ZXC.luiListaArtiklTS     .GetNameForThisCd(artikl_rec.TS);

         Fld_SkladOpis  = artikl_rec.SkladName ;
         Fld_Grupa1Opis = artikl_rec.Grupa1Name;
         Fld_PdvKatOpis = artikl_rec.PdvKatName;
         Fld_Grupa2Opis = artikl_rec.Grupa2Name;
         Fld_Grupa3Opis = artikl_rec.Grupa3Name;
         Fld_TsOpis     = artikl_rec.TsName    ;
         //===================== 

         SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.None);

         kupdobSifrar_rec = KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD/*VirtualRecID*/ == artikl_rec.DobavCD);
         if(kupdobSifrar_rec != null && artikl_rec.DobavCD.NotZero())
         {
            Fld_DobNaziv = kupdobSifrar_rec.Naziv;
            Fld_DobTick  = kupdobSifrar_rec.Ticker;
         }
         else Fld_DobCdAsTxt = Fld_DobTick = Fld_DobNaziv = "";
         

         kupdobSifrar_rec = KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD/*VirtualRecID*/ == artikl_rec.ProizCD);
         if(kupdobSifrar_rec != null && artikl_rec.ProizCD.NotZero())
         {
            Fld_ProizvNaziv = kupdobSifrar_rec.Naziv;
            Fld_ProizvTick  = kupdobSifrar_rec.Ticker;
         }
         else Fld_ProizvTick = Fld_ProizvNaziv = "";

         //===================== 
         tbx_napomena.TextAsToolTip(toolTip);

         SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.None);
         Artikl linkedArtikl_rec = ArtiklSifrar.SingleOrDefault(vvDR => vvDR.ArtiklCD == artikl_rec.LinkArtCD);
         if(linkedArtikl_rec != null && artikl_rec.LinkArtCD.NotEmpty())
         {
            Fld_LinkArtCD   = linkedArtikl_rec.ArtiklCD;
            Fld_LinkArtName = linkedArtikl_rec.ArtiklName;
         }
         else Fld_LinkArtName = Fld_LinkArtCD = "";

      if(ZXC.IsSvDUH) // Get_HALMEDartikl_List 
      {
         #region Fill LookUpList

         ZXC.luiListaRiskZiroRn.Clear();

         string luiName;

         string ATK_root7 = artikl_rec.ArtiklCD2.SubstringSafe(0, 7);

         if(ATK_root7.NotEmpty())
         {
            ZXC.hArtiklList = VvDaoBase.Get_HALMEDartikl_List(TheDbConnection, ATK_root7); // globalna varijabla 

            foreach(Halmed_SVD.HALMEDartikl halmedArtikl in ZXC.hArtiklList)
            {
               luiName = halmedArtikl.naziv + " " + halmedArtikl.obl_ozn + " " + halmedArtikl.br_pak + " X " + halmedArtikl.doza + " " + halmedArtikl.mj_ozn;
               ZXC.luiListaRiskZiroRn.Add(new VvLookUpItem(halmedArtikl.s_lio, luiName));
            }
         }

         #endregion Fill LookUpList

      } // if(ZXC.IsSvDUH) Get_HALMEDartikl_List 

   }

   public  string TheCurrentSkladCD
   {
      get 
      {
         if(TheVvTabPage.TheVvForm.VvPref.findArtikl.LastUsedSkladCD == null)
         {
                 if(Fld_ZaSkladCD     .NotEmpty()) TheVvTabPage.TheVvForm.VvPref.findArtikl.LastUsedSkladCD = Fld_ZaSkladCD;
            else if(artikl_rec.SkladCD.NotEmpty()) TheVvTabPage.TheVvForm.VvPref.findArtikl.LastUsedSkladCD = artikl_rec.SkladCD;
            else
            {
               VvLookUpItem theLui = ZXC.luiListaSkladista.SingleOrDefault(lui => lui.Integer == 1); // probaj naci lui sa integerom '1' (integer nam je kao intera sifra skladista) 

               if(theLui != null) TheVvTabPage.TheVvForm.VvPref.findArtikl.LastUsedSkladCD = theLui.Cd;
               else               TheVvTabPage.TheVvForm.VvPref.findArtikl.LastUsedSkladCD = "";
            }
         }

         return TheVvTabPage.TheVvForm.VvPref.findArtikl.LastUsedSkladCD;
      }

      set { TheVvTabPage.TheVvForm.VvPref.findArtikl.LastUsedSkladCD = value; }
   }

   void dTP_naDan_ValueChanged(object sender, EventArgs e)
   {
      PutResultFields(artikl_rec.TheAsEx = ArtiklDao.GetArtiklStatus(TheDbConnection, artikl_rec.ArtiklCD, TheCurrentSkladCD, Fld_NaDan));
   }

   void tbx_zaSkladCD_Leave_GetArtiklStatus_PutResultFields(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode != ZXC.WriteMode.None) return;

      VvTextBox vvtb = sender as VvTextBox;

      if(vvtb.Text == TheCurrentSkladCD) return;

      TheCurrentSkladCD = vvtb.Text;
      ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD = vvtb.Text;
      TheArtiklFilterUC.Fld_Sklad = tbx_zaSkladCD.Text;

      PutResultFields(artikl_rec.TheAsEx = ArtiklDao.GetArtiklStatus(TheDbConnection, artikl_rec.ArtiklCD, TheCurrentSkladCD, Fld_NaDan));
   }

   private void PutResultFields(ArtStat artiklStatus_rec)
   {
      VvHamper.ClearFieldContents(hamp_result);
      VvHamper.ClearFieldContents(hamp_VPC);
      VvHamper.ClearFieldContents(MigratorLeftParentB);
      VvHamper.ClearFieldContents(MigratorRightParentB);
      VvHamper.ClearFieldContents(hamp_OP);

      // Ikada u minusu 
      tbx_frsMinTT.Visible = tbx_frsMinTTnum.Visible = lbl_frsMin.Visible =

         (artiklStatus_rec != null) && 
         (artiklStatus_rec.FrsMinTt.NotEmpty() && artiklStatus_rec.FrsMinTtNum.NotZero());

      if(artiklStatus_rec == null) return;
      
      Fld_rPsKolSt         = artiklStatus_rec.UkPstKol;
      Fld_rPsNabCij        = artiklStatus_rec.PstCijProsNBC;
      Fld_rPsFinSt         = artiklStatus_rec.UkPstFinNBC;
      Fld_rInvKolSt        = artiklStatus_rec.InvKol;
      Fld_rInvFinSt        = artiklStatus_rec.InvFinNBC;
      Fld_rUlazKol         = artiklStatus_rec.UkUlazKol;
      Fld_rUlLastCij       = artiklStatus_rec.UlazCijLast;
      Fld_rUlazFin         = artiklStatus_rec.UkUlazFinNBC;
      Fld_rUlMinCij        = artiklStatus_rec.UlazCijMin;
      Fld_rUlMaxCij        = artiklStatus_rec.UlazCijMax;
      Fld_rIzlazKol        = artiklStatus_rec.UkIzlazKol;
      Fld_rIzlazFinProd    = artiklStatus_rec.UkIzlFinProdKCR;
      Fld_rIzlazFinNab     = artiklStatus_rec.UkIzlazFinNBC;
      Fld_rIzlazRUV        = artiklStatus_rec.IzlazRUVIznos;
      Fld_rRuvStopa        = artiklStatus_rec.IzlazRUVStopa;
      Fld_rKolSt           = artiklStatus_rec.StanjeKol;
      Fld_rPrNabCiju       = artiklStatus_rec.UlazCijProsNBC;
      // 13.12.2012: 
    //Fld_rFinSt           = artiklStatus_rec.StanjeFinKNJ;
      Fld_rFinSt           = artiklStatus_rec.StanjeFinNBC;
      Fld_rIzlRezervKol    = artiklStatus_rec.UkStanjeKolRezerv;
    //Fld_rIzlRezervKol    = artiklStatus_rec.PrNabCijOP;
      Fld_rKolStFree       = artiklStatus_rec.StanjeKolFree;
      Fld_rVPC1            = artiklStatus_rec.PreDefVpc1;
      Fld_rRuc             = artiklStatus_rec.RucVpc1Stopa;
      Fld_rVPC2            = artiklStatus_rec.PreDefVpc2;
    //Fld_rMPC             = artiklStatus_rec.PreDefMpc1;
    //Fld_rMPC             = artiklStatus_rec.LastUlazMPC;
      Fld_rMPC             = artiklStatus_rec.LastUlazMPC.NotZero() ? artiklStatus_rec.LastUlazMPC : artiklStatus_rec.PreDefMpc1;
      Fld_rDevC            = artiklStatus_rec.PreDefDevc;
      Fld_rMinKol          = artiklStatus_rec.PreDefMinKol;
      Fld_rRabat1          = artiklStatus_rec.PreDefRbt1;
      Fld_rRabat2          = artiklStatus_rec.PreDefRbt2;
      Fld_Marza            = artiklStatus_rec.PreDefMarza;
      Fld_rKolStSkl        = artiklStatus_rec.StanjKolFisycal;
      Fld_rUlazKolFisycal  = artiklStatus_rec.UkUlazKolFisycal;
      Fld_rIzlazKolFisycal = artiklStatus_rec.UkIzlazKolFisycal;
      Fld_rIzlazCijMin     = artiklStatus_rec.IzlazCijMin;
      Fld_rIzlazCijMax     = artiklStatus_rec.IzlazCijMax;
      Fld_rIzlazCijLast    = artiklStatus_rec.IzlazCijLast;
      Fld_rDateZadUlaz     = artiklStatus_rec.DateZadUlaz;
      Fld_rDateZadIzlaz    = artiklStatus_rec.DateZadIzlaz;
      Fld_rDateZadInv      = artiklStatus_rec.DateZadInv;
      Fld_rDateZadnji      = artiklStatus_rec.DateZadPromj;
      Fld_rDatePS          = artiklStatus_rec.DateZadPst;
      Fld_rIzlazFinNab2    = artiklStatus_rec.UkIzlazFinNBC;
      // 13.12.2012: 
    //Fld_rStanjeKnjigCij  = artiklStatus_rec.KnjigCij;
      Fld_rStanjeKnjigCij  = artiklStatus_rec.PrNabCij;
      // 22.04.2015: 
      if(ZXC.RRD.Dsc_IsOrgPakVisible == true)
      {
         Fld_rStanjeKnjigCijOP = artiklStatus_rec.PrNabCijOP;
      }

      // Nota Bene: za Fld_rIzlazPrNabCij artiklStatus_rec.PrNabCij sadrzava besmislicu u slucaju 'Za Sva Skladista' jer se radi o kumulativu po svim skladistima 
      //if(artiklStatus_rec.UkIzlazKol.NotZero())
      //{
      //   if(artiklStatus_rec.SkladCD.NotEmpty()) Fld_rIzlazPrNabCij = artiklStatus_rec.PrNabCij;
      //   else                                    Fld_rIzlazPrNabCij = ZXC.DivideSafely(artiklStatus_rec.UkIzlazFinNBC, artiklStatus_rec.UkIzlazKol);
      //}
      //else
      //{
      //   Fld_rIzlazPrNabCij = 0.00M;
      //}
      Fld_rIzlazPrNabCij = artiklStatus_rec.IzlCijProsNBC;
      Fld_HelpNabC       = artiklStatus_rec.LastPrNabCij; // od 1.2.2011 su zapravo 'LastPrNabCij' i 'PrNabCij' uvijek iste! 

      Fld_rFrsMinTT    = artiklStatus_rec.FrsMinTt;
      Fld_rFrsMinTTnum = artiklStatus_rec.FrsMinTtNum;

      if(ZXC.IsSvDUH)
      {
         Rtrans UGOrtrans_rec = new Rtrans();

         bool UGOrtransFound = FakturDao.SetMeLastRtransForArtiklAndTT(TheDbConnection, UGOrtrans_rec, Faktur.TT_UGO, artikl_rec.ArtiklCD, /*false*/true);

         if(UGOrtransFound)
         {
            Fld_Placement = "UGO-" + UGOrtrans_rec.T_ttNum;
         }

         decimal pdvSt = artikl_rec.PdvKat.IsEmpty() ? Faktur.CommonPdvStForThisDate(artiklStatus_rec.DateZadUlaz) : ZXC.ValOrZero_Decimal(artikl_rec.PdvKat, 0);
         Fld_rKolStSkl = ZXC.VvGet_100_from_125(artiklStatus_rec.PrNabCij, pdvSt);
      }

   }

   public override void GetFields(bool fuse)
   {
      if(artikl_rec == null) artikl_rec = new Artikl();

      artikl_rec.ArtiklCD    = Fld_ArtiklCd   ;
      artikl_rec.ArtiklName  = Fld_ArtiklName ;
      artikl_rec.BarCode1    = Fld_BarCode1    ;
      artikl_rec.SkladCD     = Fld_UobSkladCD    ;
      artikl_rec.Grupa1CD    = Fld_Grupa1CD   ;
      artikl_rec.JedMj       = Fld_JedMj      ;
      artikl_rec.Konto       = Fld_Konto      ;
      artikl_rec.ArtiklCD2   = Fld_ArtiklCd2  ;
      artikl_rec.ArtiklName2 = Fld_ArtiklName2;

      artikl_rec.TS          = Fld_Ts         ; 
      artikl_rec.BarCode2    = Fld_BarCode2   ;
      artikl_rec.SerNo       = Fld_SerNo      ;
      artikl_rec.Grupa2CD    = Fld_Grupa2CD   ;
      artikl_rec.Grupa3CD    = Fld_Grupa3CD   ;
      artikl_rec.Placement   = Fld_Placement  ;
      artikl_rec.LinkArtCD   = Fld_LinkArtCD  ;
      artikl_rec.DateProizv  = Fld_DateProizv ;
      artikl_rec.PdvKat      = Fld_PdvKat     ;
      artikl_rec.LongOpis    = Fld_LongOpis   ;
      artikl_rec.PrefValName = Fld_PrefValName;
      artikl_rec.OrgPak      = Fld_OrgPak     ;
      artikl_rec.IsRashod    = Fld_IsRashod   ;
      artikl_rec.IsAkcija    = Fld_IsAkcija   ;
      artikl_rec.IsMaster    = Fld_IsMaster   ;
      artikl_rec.MasaNetto   = Fld_MasaNetto;
      artikl_rec.MasaBruto   =  Fld_MasaBruto ;
      artikl_rec.Promjer     =  Fld_Promjer   ;
      artikl_rec.Povrsina    =  Fld_Povrsina  ;
      artikl_rec.Zapremina   =  Fld_Zapremina ;
      artikl_rec.Duljina     =  Fld_Duljina   ;
      artikl_rec.Sirina      =  Fld_Sirina    ;
      artikl_rec.Visina      =  Fld_Visina    ;
      artikl_rec.Starost     =  Fld_Starost   ;
      artikl_rec.Boja        =  Fld_Boja      ;
      artikl_rec.Spol        =  Fld_Spol      ; 
      artikl_rec.Garancija    = Fld_Garancija   ;   
      artikl_rec.DobavCD      = Fld_DobavCD     ; 
      artikl_rec.ProizCD      = Fld_ProizCD     ;   
      artikl_rec.IsAllowMinus = Fld_IsAllowMinus;
      artikl_rec.IsSerNo      = Fld_IsSerNo     ;
      artikl_rec.MasaNettoJM  = Fld_MasaNettoJM ;
      artikl_rec.MasaBrutoJM  = Fld_MasaBrutoJM ;
      artikl_rec.PromjerJM    = Fld_PromjerJM   ;
      artikl_rec.PovrsinaJM   = Fld_PovrsinaJM  ;
      artikl_rec.ZapreminaJM  = Fld_ZapreminaJM ;
      artikl_rec.DuljinaJM    = Fld_DuljinaJM   ;
      artikl_rec.SirinaJM     = Fld_SirinaJM    ;
      artikl_rec.VisinaJM     = Fld_VisinaJM    ;
      artikl_rec.Velicina     = Fld_Velicina    ;
      artikl_rec.MadeIn       = Fld_MadeIn      ;
      artikl_rec.Url          = Fld_Url         ;
      artikl_rec.AtestBr      = Fld_AtestBr     ;
      artikl_rec.AtestDate    = Fld_AtestDate   ;
      artikl_rec.Vpc1Policy   = Fld_Vpc1Policy  ;
      artikl_rec.IsPrnOpis    = Fld_IsPrnOpis   ;
      artikl_rec.CarTarifa    = Fld_CarTarifa   ; 
      artikl_rec.PartNo       = Fld_PartNo      ; 
      artikl_rec.Napomena     = Fld_Napomena    ;
      artikl_rec.ImportCij    = Fld_ImportCij   ;

      artikl_rec.Snaga       = Fld_Snaga;
      artikl_rec.SnagaJM     = Fld_SnagaJM;

      artikl_rec.EmisCO2     = Fld_EmisCO2; 
      artikl_rec.EuroNorma   = Fld_EuroNorma; 

      //GetMigratorsStates(MigratorParent);
   }

   #endregion PutFields(), GetFields()

   #region Put Trans DGV Fileds

   private const string rtrans_TabPageName  = "RTrans";
   private const string pckInfo_TabPageName = "PCKinfo";

   // Tu dolazimo na 1 nacin: 1. Classic PutFields 
   private void InitializeFilterUCFields()
   {
      SetFilterRecordDependentDefaults(); // filter.KontoOd i Do = kplan_rec.Konto (punimo bussiness od filtera, ne UC)
      TheArtiklFilterUC.PutFilterFields(TheArtiklFilter);
   }

   // Tu dolazimo na 2 nacina:          
   // 1. Classic PutFields              
   // 2. TheTabControl.SelectionChanged 
   public override void DecideIfShouldLoad_TransDGV(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   {
      ZXC.VvInnerTabPageKindEnum innerTabPageKind = ((VvInnerTabPage)TheTabControl.SelectedTab).TheInnerTabPageKindEnum;

      if(innerTabPageKind == ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage)
      {
         TheArtiklFilter.IsPopulatingTransDGV = true;
         //TheArtiklFilter.ArtiklCards = (ArtiklCardFilter.ArtiklCardsEnum)Enum.Parse(typeof(ArtiklCardFilter.ArtiklCardsEnum), ((VvInnerTabPage)TheTabControl.SelectedTab).Name);
         //TheArtiklFilterUC.Fld_ArtiklCard = TheArtiklFilter.ArtiklCards;

         if(aTransesLoaded[0] == false)
         {
            LoadRecordList_AND_PutTransDgvFields();
         }
      }

   }

   // Tu dolazimo na 2 nacina:          
   // 1. ButtonIzlistaj_Click (sa FilterUC-a) 
   // 2. DecideIfShouldLoad_TransDGV    
   public override void LoadRecordList_AND_PutTransDgvFields()
   {
      int rowIdx, idxCorrector;

      TheArtiklFilterUC.GetFilterFields();
      TheArtiklFilterUC.AddFilterMemberz(TheArtiklFilter, null);

      if  (artikl_rec.Rtranses == null) artikl_rec.Rtranses = new List<Rtrans>();
      else                              artikl_rec.Rtranses.Clear();

      VvDaoBase.LoadGenericVvDataRecordList<Rtrans>(TheDbConnection, artikl_rec.Rtranses, TheArtiklFilter.FilterMembers, Rtrans.recordName, Rtrans.artiklOrderBy_DESC/*, true*/);

      aTransesLoaded[0] = true;

      //--------------------------------------------------------------------------------------------------------------------------------------------- 
      aTransesGrid[0].Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(aTransesGrid[0]);

      foreach(Rtrans rtrans_rec in artikl_rec.Rtranses)
      {
         aTransesGrid[0].Rows.Add();

         rowIdx = aTransesGrid[0].RowCount - idxCorrector;

         rtrans_rec.CalcTransResults(null/*faktur_rec*/);

         aTransesGrid[0][ 0, rowIdx].Value = rtrans_rec.T_parentID;
         aTransesGrid[0][ 1, rowIdx].Value = rtrans_rec.T_TT;
         aTransesGrid[0][ 2, rowIdx].Value = rtrans_rec.T_ttNum;
         aTransesGrid[0][ 3, rowIdx].Value = rtrans_rec.T_serial;
         aTransesGrid[0][ 4, rowIdx].Value = rtrans_rec.T_skladDate;
         aTransesGrid[0][ 5, rowIdx].Value = rtrans_rec.T_skladCD;
         //aTransesGrid[0][ 6, rowIdx].Value = rtrans_rec.T_kupdobCD;

         Kupdob kupdobSifrar_rec = KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD/*VirtualRecID*/ == rtrans_rec.T_kupdobCD);
         
         if(kupdobSifrar_rec != null) aTransesGrid[0][ 6, rowIdx].Value = kupdobSifrar_rec.Naziv;
         else                         aTransesGrid[0][ 6, rowIdx].Value = "";
         
         aTransesGrid[0][ 7, rowIdx].Value = rtrans_rec.T_dokNum;
         aTransesGrid[0][ 8, rowIdx].Value = rtrans_rec.T_kol;
         aTransesGrid[0][ 9, rowIdx].Value = rtrans_rec.R_CIJ_KCR;
         aTransesGrid[0][10, rowIdx].Value = rtrans_rec.R_KCR;

         aTransesGrid[0].Rows[rowIdx].HeaderCell.Value = (artikl_rec.Rtranses.Count - rowIdx).ToString();
      }
   }

   #endregion Put Trans DGV Fileds

   #region Overriders And Specifics

   #region VvDataRecord/VvDaoBase

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.artikl_rec; }
      set {        this.artikl_rec = (Artikl)value; }
   }

   public override VvSifrarRecord VirtualSifrarRecord
   {
      get { return this.VirtualDataRecord as VvSifrarRecord; }
      set {        this.VirtualDataRecord = (Artikl)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.ArtiklDao; }
   }

   #endregion VvDataRecord/VvDaoBase

   #region VvMigrationHampers

   protected override Control MigratorRightParentA
   {
      get
      {
         return panel_MigratorsRightA;
      }
   }
  
   protected override Control MigratorRightParentB
   {
      get
      {
         return panel_MigratorsRightB;
      }
   }

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get
      {
         return ZXC.TheVvForm.VvPref.artiklUC.MigratorStates;
      }
   }
 
   protected override Panel MigratorLeftParentA
   {
      get
      {
         return panel_MigratorsLeftA;
      }
   }
 
   protected override Panel MigratorLeftParentB
   {
      get
      {
         return panel_MigratorsLeftB;
      }
   }

   protected override Control ControlUnderMigLeftParentA
   {
      get
      {
         return panel_result;
      }
   }

  
   #endregion Migration


   #region VvFindDialog

   public override VvFindDialog CreateVvFindDialog()
   {
      return CreateFind_Artikl_Dialog();
   }

   public static VvFindDialog CreateFind_Artikl_Dialog()
   {
      VvForm.VvSubModul vvSubModul = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsART);
      VvDataRecord vvDataRecord    = ZXC.TheVvForm.CreateTheVvDataRecord_SwitchSubModulEnum(vvSubModul);

      VvFindDialog vvFindDialog = new VvFindDialog();
      VvRecLstUC vvRecListUC    = new ArtiklListUC(vvFindDialog, (Artikl)vvDataRecord, vvSubModul);
      vvFindDialog.TheRecListUC = vvRecListUC;

      if(ZXC.IsSkyEnvironment)
      {
         vvFindDialog.button_AddSifrarRec.Enabled = false;
      }


      return vvFindDialog;
   }

   #endregion VvFindDialog

   #region PrintSifrarRecord

   public ArtiklSifrarFilterUC TheArtiklFilterUC { get; set; }
   public ArtiklCardFilter     TheArtiklFilter   { get; set; }

  // public RptP_ArtiklCard    TheRptP_Artikl { get; set; }

   //protected VvReport specificArtiklReport;
   protected string specificArtiklReportName;

   //public override VvReport VirtualReport
   //{
   //   get
   //   {
   //      return this.specificArtiklReport;
   //   }
   //}

   public override string VirtualReportName
   {
      get
      {
         return this.specificArtiklReportName;
      }
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      ArtiklCardFilter artiklCardFilter = (ArtiklCardFilter)vvRptFilter;

      // 3.2.2011: 
      SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

      switch(artiklCardFilter.ArtiklCards)
      {
         case ArtiklCardFilter.ArtiklCardsEnum.Artikl        : return new RptR_ArtiklMaticni      (new Vektor.Reports.RIZ.CR_ArtiklCard()            , reportName, artiklCardFilter);
         case ArtiklCardFilter.ArtiklCardsEnum.ArtiklL       : return new RptR_ArtiklMaticni      (new Vektor.Reports.RIZ.CR_ArtiklCard()            , reportName, artiklCardFilter);
         case ArtiklCardFilter.ArtiklCardsEnum.RobKartA      : return new RptR_ArtiklKartica      (new Vektor.Reports.RIZ.CR_RobnaKartica_A()        , reportName, artiklCardFilter);
         case ArtiklCardFilter.ArtiklCardsEnum.RobKartAP     : return new RptR_ArtiklKartica      (new Vektor.Reports.RIZ.CR_RobnaKarica_AP()        , reportName, artiklCardFilter);
         case ArtiklCardFilter.ArtiklCardsEnum.RobKartB      : return new RptR_ArtiklKartica      (new Vektor.Reports.RIZ.CR_RobnaKartica_B()        , reportName, artiklCardFilter);
         case ArtiklCardFilter.ArtiklCardsEnum.RobKartKol    : return new RptR_ArtiklKartica      (new Vektor.Reports.RIZ.CR_RobnaKarticaKOL()       , reportName, artiklCardFilter);
         case ArtiklCardFilter.ArtiklCardsEnum.RekapTrans    : return new RptR_ArtiklRtranses     (new Vektor.Reports.RIZ.CR_RekapTrans()            , reportName, artiklCardFilter);
         case ArtiklCardFilter.ArtiklCardsEnum.RbKrtKolSerlot: return new RptR_ArtiklKartica      (new Vektor.Reports.RIZ.CR_RobnaKarticaKOL_Serlot(), reportName, artiklCardFilter);
         case ArtiklCardFilter.ArtiklCardsEnum.RobKartAMB    : return new RptR_ArtiklKartica      (new Vektor.Reports.RIZ.CR_RobnaKartica_AMB()      , reportName, artiklCardFilter);
         case ArtiklCardFilter.ArtiklCardsEnum.PCKinfo       : return new RptR_PTG_Artikl_PCK_info(new Vektor.Reports.RIZ.CR_PTG_PCKinfo()          , reportName, artiklCardFilter);

         default: ZXC.aim_emsg("{0}\nPrintSomeArtiklDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), artiklCardFilter.ArtiklCards); return null;
      }
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheArtiklFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheArtiklFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
      this.TheArtiklFilter.ArtiklCdOD = artikl_rec.ArtiklCD;
      this.TheArtiklFilter.ArtiklCdDO = artikl_rec.ArtiklCD;

      this.TheArtiklFilter.SkladCD = ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD;

   }


   #endregion PrintSifrarRecord

   public override Size ThisUcSize
   {
      get
      {
         return new Size(hamp_naziv.Right + ZXC.QunMrgn, panel_result.Bottom + ZXC.QunMrgn);
      }
   }

   #region PutNew_Sifra_Field

   public override void PutNew_Sifra_Field(uint newSifra)
   {
      // 02.07.2018: 
    //                 Fld_ArtiklCd = newSifra.ToString("000000");
      if(!ZXC.IsSvDUH) Fld_ArtiklCd = newSifra.ToString("000000");
   }

   public override void PutNew_Sifra_Field(string newSifra)
   {
      Fld_ArtiklCd = newSifra;
   }

   public override void CleanUniqueFieldsOnCopyFromOtherRecord()
   {
      Fld_ArtiklCd2 = Fld_BarCode1 = Fld_BarCode2 = Fld_SerNo = Fld_PartNo = "";

      if(ZXC.IsSvDUH) Fld_Placement = "";

   }

   #endregion PutNew_Sifra_Field

   public void GetNextSifraWroot_String_btnClick(object sender, EventArgs e)
   {
      string rootPart = Fld_ArtiklCd.ToUpper();
      string sifraColName = VirtualSifrarRecord.SifraColName;

      uint nextNum = TheVvDao.GetNextSifraWroot_String(TheDbConnection, Artikl.recordName, sifraColName, rootPart);

      Fld_ArtiklCd += nextNum;
   }

   #endregion Overriders And Specifics

   #region Update_VvDataRecord (Legacy naming convention)

   /// <summary>
   /// 'FindVvDataRecord' procedura. Inicirana:
   /// 1. Context menu (Mouse right click)
   /// 2. Mouse click (Ctrl ili Alt click)
   /// 3. Keyboard initiated (Ctrl/Alt + F/Space)
   /// </summary>
   /// <param name="startValue"></param>
   /// <returns></returns>
   public static string Update_Artikl(VvSQL.SorterType whichInformation, object startValue, ZXC.AutoCompleteRestrictor sifrarRestrictor)
   {
      Artikl         artikl_rec = new Artikl();
      ArtiklListUC   artiklListUC;
      XSqlConnection dbConnection = ZXC.TheVvForm.TheDbConnection;

      VvFindDialog dlg = CreateFind_Artikl_Dialog();

      artiklListUC = (ArtiklListUC)(dlg.TheRecListUC);

      switch(whichInformation)
      {
         case VvSQL.SorterType.Name   : artiklListUC.Fld_FromName     = startValue.ToString(); break;
         case VvSQL.SorterType.Code   : artiklListUC.Fld_FromArtiklCD = startValue.ToString(); break;
         case VvSQL.SorterType.BarCode: artiklListUC.Fld_FromBarCode1 = startValue.ToString(); break;

         default: ZXC.aim_emsg(" 111: For Artikl, trazi po [{1}] still nedovrseno!", whichInformation); break;
      }

      // 15.1.2011:
      if(ZXC.TheVvForm.TheVvUC is FakturDUC) artiklListUC.Fld_SituacijaZaSkladCD = ((FakturDUC)ZXC.TheVvForm.TheVvUC).Fld_SkladCD; 

      #region sifrarRestrictor
      
      // new, from 05.06.2009 

      if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.ART_MOT_VOZILO_Only)   artiklListUC.Fld_Grupa1CD = ZXC.MotVoziloGrCD;

      if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.ART_NabOrProdKAT_Only && ZXC.TheVvForm.TheVvUC is FakturDUC)
      {
         FakturDUC theDUC = ZXC.TheVvForm.TheVvUC as FakturDUC;

         int currRow = theDUC.TheG.CurrentRow.Index;

         TtInfo ttInfo = theDUC.GetTtInfoOfThisRow(currRow);

         if(ttInfo.TheTT == Faktur.TT_PRI ||
            ttInfo.TheTT == Faktur.TT_MSI ||
            ttInfo.TheTT == Faktur.TT_IZD ||
            ttInfo.TheTT == Faktur.TT_PST ||
            ttInfo.TheTT == Faktur.TT_TRI ||
            ttInfo.TheTT == Faktur.TT_PUL ||
            ttInfo.TheTT == Faktur.TT_INV  ) ZXC.FindART_NabOrProdKAT = Artikl.NabRobaGrCD;
            
    else if(ttInfo.TheTT == Faktur.TT_IRM ||
          //ttInfo.TheTT == Faktur.TT_MVI ||
            ttInfo.TheTT == Faktur.TT_TRM ||
            ttInfo.TheTT == Faktur.TT_PSM ||
            ttInfo.TheTT == Faktur.TT_ZPC ||
            ttInfo.TheTT == Faktur.TT_PIZ ||
            ttInfo.TheTT == Faktur.TT_INM  ) ZXC.FindART_NabOrProdKAT = Artikl.ProdRobaGrCD;
    
    else if(ttInfo.TheTT == Faktur.TT_MVI) //14.09.2015. razlika MVI i MVI-2
         {
            if(theDUC is MedjuSkladMVI2DUC) ZXC.FindART_NabOrProdKAT = Artikl.ProAndNabGrCD;
            else                            ZXC.FindART_NabOrProdKAT = Artikl.ProdRobaGrCD;
         }

    else if(ttInfo.TheTT == Faktur.TT_VMI  ) ZXC.FindART_NabOrProdKAT = Artikl.ProAndNabGrCD;
    else if(ttInfo.TheTT == Faktur.TT_URA  ) ZXC.FindART_NabOrProdKAT = Artikl.ProAndNabGrCD;
            
         else 
         {
            ZXC.FindART_NabOrProdKAT = "";
         }

         artiklListUC.Fld_Grupa3CD = ZXC.FindART_NabOrProdKAT;
      }

      //if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.KID_Mtros_Only   ) kupdobListUC.Fld_FiltIsMTros    = true;
      //if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.KID_Banka_Only   ) kupdobListUC.Fld_FiltIsBanka    = true;
      //if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.KID_Dobav_Only   ) kupdobListUC.Fld_FiltIsDob      = true;
      //if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.KID_Kupac_Only   ) kupdobListUC.Fld_FiltIsKup      = true;
      //if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.KID_Komisija_Only) kupdobListUC.Fld_FiltIsKomisija = true;

      #endregion sifrarRestrictor

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         if(!ZXC.ArtiklDao.SetMe_Record_byRecID(dbConnection, artikl_rec, (uint)dlg.SelectedRecID, false)) return null;
      }
      else
      {
         artikl_rec = null;
      }

      if(dlg.SelectionIsNewlyAddedRecord == true) ZXC.ShouldForceSifrarRefreshing = true;

      dlg.Dispose();

      if(artikl_rec != null)
      {
         switch(whichInformation)
         {
            case VvSQL.SorterType.Name   : return artikl_rec.ArtiklName;
            case VvSQL.SorterType.Code   : return artikl_rec.ArtiklCD.ToString();
            case VvSQL.SorterType.BarCode: return artikl_rec.BarCode1;

            default: ZXC.aim_emsg(" 222: For Artikl, trazi po [{1}] still nedovrseno!", whichInformation); return null;
         }
      }
      else return null;
   }

   #endregion Update_VvDataRecord (Legacy naming convention)

   public void Show_ExternDokument_Click(object sender, EventArgs e)
   {
      #region fieldz

      Button btn = sender as Button;
      int linkId = ZXC.ValOrZero_Int(btn.Tag.ToString()); if(linkId != 1 && linkId != 2) throw new Exception("Link_ExternDokument_Click: linkId unknown! (" + linkId.ToString() + ")");

      string fullPathFileName = "";

      #endregion fieldz

      switch(linkId)
      {
         case 1: fullPathFileName = Fld_Url     ; break;
         case 2: fullPathFileName = Fld_Napomena; break;
      }

      if(fullPathFileName.IsEmpty()) return;

      // here we go 
      try
      {
         System.Diagnostics.Process.Start(fullPathFileName);
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, ex.Message);
      }
   }

   public void DecideIfShouldLoad_PCKinfo(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   {
      bool PCKinfo_TabPageIsVisible = TheTabControl.SelectedTab.Name == pckInfo_TabPageName;
     
      if(/*PTG_PCKinfoLoaded == false && */this.artikl_rec.TS == ZXC.PCK_TS && PCKinfo_TabPageIsVisible)
      {
         //PTG_PCKinfoLoaded = true;

         TheArtiklFilterUC.rbt_PCKinfo.Checked = true;

         pcKInfoUC.ThePCKInfoGrid.Rows.Clear();

         string skladCD = this.TheCurrentSkladCD;

         List<PCK_Artikl> PCK_ArtiklInfo_List = RtranoDao.Get_PCK_ArtiklList_ByPCK_Baza_AndSklad(TheDbConnection, this.artikl_rec, skladCD, pcKInfoUC.Fld_Pck_Info_kind,
                                                                                                                                            pcKInfoUC.Fld_IsIstaRamKlasa,
                                                                                                                                            pcKInfoUC.Fld_IsIstaHddKlasa);

         pcKInfoUC.PutDgvFields(PCK_ArtiklInfo_List);

         pcKInfoUC.Size = new Size(pcKInfoUC.Parent.Width - ZXC.QunMrgn, pcKInfoUC.Parent.Height - ZXC.QUN);
         pcKInfoUC.ThePCKInfoGrid.Height = pcKInfoUC.Size.Height - pcKInfoUC.ThePCKInfoSumGrid.Height - ZXC.Q2un;
         pcKInfoUC.TheSernoGrid  .Height = pcKInfoUC.ThePCKInfoGrid.Height + pcKInfoUC.ThePCKInfoSumGrid.Height/* - ZXC.Q2un*/;
         
         pcKInfoUC.ThePCKInfoSumGrid.Width = pcKInfoUC.ThePCKInfoGrid.Width;
         pcKInfoUC.ThePCKInfoSumGrid.Location = new Point(pcKInfoUC.ThePCKInfoGrid.Location.X, pcKInfoUC.ThePCKInfoGrid.Bottom + ZXC.Qun12);
         pcKInfoUC.ThePCKInfoSumGrid.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
         
         pcKInfoUC.ThePCKInfoGrid.ColumnHeadersDefaultCellStyle.BackColor = pcKInfoUC.TheSernoGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.PowderBlue;
         pcKInfoUC.ThePCKInfoGrid.ColumnHeadersDefaultCellStyle.ForeColor = pcKInfoUC.TheSernoGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.DarkSlateGray;
         pcKInfoUC.ThePCKInfoGrid.RowHeadersDefaultCellStyle.BackColor    = pcKInfoUC.TheSernoGrid.RowHeadersDefaultCellStyle.BackColor    = Color.PowderBlue; //Color.FloralWhite;
         pcKInfoUC.ThePCKInfoGrid.RowHeadersDefaultCellStyle.ForeColor    = pcKInfoUC.TheSernoGrid.RowHeadersDefaultCellStyle.ForeColor    = Color.DarkSlateGray;

         //if(PCK_ArtiklInfo_List.NotEmpty()) // da kod prethodni sljedeci ispuni prvoga a ako je empty da ga prazni
         //{
         //   PCK_Artikl PCK_Line = PCK_ArtiklInfo_List[0];
         //
         //   pcKInfoUC.PutDgv2Fields(PCK_Line.PCK_Unikat_List);
         //}
         //else 
         //{
            pcKInfoUC.TheSernoGrid.Rows.Clear();
         //}
       
         VvHamper.Open_Close_Fields_ForWriting(pcKInfoUC.hamp_rbtBaza , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);// jer se ponasa ko reportFilter
         VvHamper.Open_Close_Fields_ForWriting(pcKInfoUC.hamp_cbxKlase, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);// jer se ponasa ko reportFilter

         pcKInfoUC.Visible = true;
      }
      else
      {
         //TheArtiklFilterUC.rbt_PCKinfo.Checked = false;
         TheArtiklFilterUC.rbt_robKartA.Checked = true;

         pcKInfoUC.Visible = false;
      }
   }
}

public class VvRenameArtiklDlg : VvDialog
{
   #region Fieldz

   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_newArtiklCD;

   #endregion Fieldz

   #region Constructor

   public VvRenameArtiklDlg(bool isRenameNaziv)
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Preimenuj " + (isRenameNaziv ? "naziv" : "šifru") + " artikla";

      CreateHamper(isRenameNaziv);

      dlgWidth        = hamper.Right + ZXC.QunMrgn;
      dlgHeight       = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_newArtiklCD, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
   }

   #endregion Constructor

   #region hamper

   private void CreateHamper(bool isRenameNaziv)
   {
      hamper          = new VvHamper(2, 1, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q6un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      // 28.03.2016: 
    //int columnSize =                                                                                  ZXC.ArtiklDao.GetSchemaColumnSize(ZXC.ArtiklDao.CI.artiklCD); 
      int columnSize = isRenameNaziv ? ZXC.ArtiklDao.GetSchemaColumnSize(ZXC.ArtiklDao.CI.artiklName) : ZXC.ArtiklDao.GetSchemaColumnSize(ZXC.ArtiklDao.CI.artiklCD);

      Label lbl       = hamper.CreateVvLabel  (0, 0, isRenameNaziv ? "Novi naziv" : "Nova šifra:", ContentAlignment.MiddleRight);
      tbx_newArtiklCD = hamper.CreateVvTextBox(1, 0, "tbx_newArtiklCD", "", columnSize);
   }

   #endregion hamper

   #region Event cancelButton

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Event cancelButton

   #region Fld_

   public string Fld_NewArtikl_CD_or_Name
   {
      get { return tbx_newArtiklCD.Text; }
      set {        tbx_newArtiklCD.Text = value; }
   }

   #endregion Fld_

}

public class MinusForm : VvDialog
{
   private Button     okButton;
   private VvHamper   hamperMinus, hamperLabel;
   public RadioButton rbt_allMinusError, rbt_NoAllMinusError, rbt_NoMinusError, rbt_NoMinusErrorNextOpen;
   public Label       lblText;

   private int dlgWidth, dlgHeight;

   public MinusForm()
   {
      CreateLabelHamper(out hamperLabel);
      
      CreateMinusHamper(out hamperMinus);
      
      CalcSize();

      AddOkButton(out okButton, dlgWidth, dlgHeight);
      okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      this.Text = "Minusi";
      this.StartPosition = FormStartPosition.CenterScreen;
   }

   #region Hamper

   private void CalcSize()
   {
      dlgWidth        = hamperMinus.Width  + ZXC.QunMrgn * 2;
      dlgHeight       = hamperMinus.Bottom + ZXC.QunBtnH + 3 * ZXC.QunMrgn;
      this.ClientSize = new Size(dlgWidth, dlgHeight);
   }
 
   private void CreateLabelHamper(out VvHamper hamper)
   {
      hamper          = new VvHamper(1, 3, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);

      hamper.VvColWdt      = new int[] { ZXC.Q10un*2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun10 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }

      hamper.VvBottomMargin = hamper.VvTopMargin;

      lblText = hamper.CreateVvLabel(0, 0, "lblText", 0, 2, ContentAlignment.MiddleLeft);

   }

   private void CreateMinusHamper(out VvHamper hamper)
   {
      hamper          = new VvHamper(1, 4, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, hamperLabel.Bottom);

      hamper.VvColWdt      = new int[] { ZXC.Q10un*2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun10 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }

      hamper.VvBottomMargin = hamper.VvTopMargin;

      rbt_allMinusError        = hamper.CreateVvRadioButton(0, 0, null, "Nastavi javljati minuse za sve artikle"                   , TextImageRelation.ImageAboveText);
      rbt_NoMinusError         = hamper.CreateVvRadioButton(0, 1, null, "Prestani javljati minus za ovaj artikl"                   , TextImageRelation.ImageAboveText);
      rbt_NoAllMinusError      = hamper.CreateVvRadioButton(0, 2, null, "Prestani javljati minuse za sve artikle u ovom izvještaju", TextImageRelation.ImageAboveText);
      rbt_NoMinusErrorNextOpen = hamper.CreateVvRadioButton(0, 3, null, "Prestani javljati minuse do sljedećeg ulaska u program"   , TextImageRelation.ImageAboveText);

   }

   #endregion Hamper
 
}
