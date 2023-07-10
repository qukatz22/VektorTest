using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using Crownwood.DotNetMagic.Controls;
using ikvm.lang;

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

public class KUG_PTG_DUC : FakturExtDUC
{
   #region Constructor

   public KUG_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[]
         {
            Faktur.TT_KUG
         });
   }

   #endregion Constructor

   #region Hampers

   public VvHamper hamp_TT_PTG, hamp_v1TT_PTG, hamp_danFak;

   public VvTextBox tbx_DanFakturiranja, tbx_DanFakturiranjaOpis;

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      InitializeHamper_TT_PTG  (out hamp_TT_PTG);
      InitializeHamper_v1TT_PTG(out hamp_v1TT_PTG);
      InitializeHamper_danFak_PTG(out hamp_danFak);

      CreateArrOfHampers();

      SetParentOfHamperLeftHampers();

      panel_MigratorsLeftB.SendToBack();

      hamp_TT_PTG.Location   = new Point(hamp_kupdobNaziv.Right                 , hamp_kupdobNaziv.Top);
      hamp_dokDate.Location  = new Point(hamp_TT_PTG.Right                      , hamp_TT_PTG.Top);
      hamp_dokNum  .Location = new Point(hamp_dokDate.Right                     , hamp_TT_PTG.Top);
      hamp_v1TT_PTG.Location = new Point(hamp_TT_PTG.Right - hamp_v1TT_PTG.Width, hamp_TT_PTG.Bottom);
      hamp_danFak.Location   = new Point(hamp_TT_PTG.Left                       , hamp_v1TT_PTG.Bottom);

      nextY = hamp_danFak.Bottom;

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_TT_PTG, hamp_kupdobNaziv, hamp_kupdobOther, hamp_dokDate, hamp_dokNum, hamp_v1TT_PTG, hamp_danFak
                                  };

      //hamperMigr = new VvHamper[] { hamp_Mtros};

      //hamperCbx4Migr = new VvHamper[] { hampCbxM_Mtros };

   }

   private void InitializeHamper_TT_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", null, false);

      hamper.VvColWdt      = new int[] { ZXC.Q4un     , ZXC.Q2un + ZXC.Qun4, ZXC.Q5un, ZXC.Q2un- ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { faBefFirstCol,            faBefCol, faBefCol, faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN + ZXC.Qun8 };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

               hamper.CreateVvLabel        (0, 0, "Tip:", ContentAlignment.MiddleRight);
      tbx_TT = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_tt", "Tip transakcije - racuna");
      // Nota bene; buduci je tbx_tt VvLookUp TextBox onde mu je tbx_tt.JAM_FieldExitMethod 'potrosena' lookUp methodom. 
      //tbx_tt.JAM_FieldExitMethod_2 = new EventHandler(OnExit_ValidateTTrestrictor);

      tbx_TT.JAM_FieldExitWithValidationMethod += new System.ComponentModel.CancelEventHandler(OnExit_ValidateTTrestrictor);

      tbx_TtOpis = hamper.CreateVvTextBox(2, 0, "tbx_ttOpis", "");
      tbx_TtNum  = hamper.CreateVvTextBox(3, 0, "tbx_ttNum", "Ovo bolje ostavi kako je...", 7); // !!! 
      tbx_TT.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_TT.JAM_DataRequired = true;
      tbx_TT.JAM_MustTabOutBeforeSubmit = true;
      tbx_TtOpis.JAM_ReadOnly = true;

      tbx_TtNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_TT.JAM_ReadOnly = true;
      tbx_TT.JAM_lui_NameTaker_JAM_Name = tbx_TtOpis.JAM_Name;
      tbx_TT.JAM_ttNumTaker_JAM_Name = tbx_TtNum.JAM_Name;

      tbx_TtNum.JAM_IsSupressTab = true;

      tbx_TT.Font = ZXC.vvFont.BaseBoldFont;
      tbx_TtOpis.Font = ZXC.vvFont.BaseFont;
      tbx_TtNum.Font = ZXC.vvFont.BaseBoldFont;

      tbx_TtNum.JAM_MarkAsNumericTextBox(0, false, 1, 40, false);
   }

   private void InitializeHamper_v1TT_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", null, false);
      
      hamper.VvColWdt     = new int[] { ZXC.Q2un - ZXC.Qun4 };
      hamper.VvSpcBefCol  = new int[] { faBefFirstCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

//                      hamper.CreateVvLabel        (0, 0, "KUG:"       , ContentAlignment.MiddleRight);
      tbx_v1_ttNum  = hamper.CreateVvTextBox      (0, 0, "tbx_v1_ttNum", "Broj Krovnog Ugovora", 2/*GetDB_ColumnSize(DB_ci.iz_ttNum)*/);
      tbx_v1_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_v1_ttNum.JAM_IsSupressTab = true;
      tbx_v1_ttNum.JAM_MarkAsNumericTextBox(0, false, 1, 40, false);
      tbx_v1_ttNum.TextAlign = HorizontalAlignment.Right;
      tbx_v1_ttNum.JAM_ReadOnly = true;
   }

   private void InitializeHamper_danFak_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", null, false);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q2un, ZXC.Q6un + faBefCol };
      hamper.VvSpcBefCol   = new int[] { faBefCol, faBefCol, faBefCol            };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                                hamper.CreateVvLabel        (0, 0, "DatZaFaktur:", ContentAlignment.MiddleRight);
      tbx_DanFakturiranja     = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_DanZaFakturiranje", "tbx_DanZaFakturiranje", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.opciAvalue));
      tbx_DanFakturiranjaOpis = hamper.CreateVvTextBox      (2, 0, "tbx_DanFakturiranjaOpis", "DanZaFakturiranje najma", 32);
      tbx_DanFakturiranja.JAM_Set_LookUpTable(ZXC.luiListaPTG_DanZaFaktur, (int)ZXC.Kolona.prva);
      tbx_DanFakturiranja.JAM_lui_NameTaker_JAM_Name = tbx_DanFakturiranjaOpis.JAM_Name;
      tbx_DanFakturiranjaOpis.JAM_ReadOnly = true;
   }

   #endregion Hampers

   public string PTG_DanFakturiranjaString //opciAvalue
   {
      get { return tbx_DanFakturiranja.Text; }
      set {        tbx_DanFakturiranja.Text = value; }
   }
   public string Fld_PTG_DanFakturiranjaOpis { set { tbx_DanFakturiranjaOpis.Text = value; } }
   public ZXC.PTG_DanFakturiranjaEnum Fld_PTG_DanFakturiranja
   {
      get
      {
         if(tbx_DanFakturiranja.Text.IsEmpty()) return ZXC.PTG_DanFakturiranjaEnum.PrviDanMjeseca;
         else                              return (ZXC.PTG_DanFakturiranjaEnum)Enum.Parse(typeof(ZXC.PTG_DanFakturiranjaEnum), PTG_DanFakturiranjaString, true);
      }
   }
}

// trebali bi micati ovaj duc a vec je otislo van - hocemo li to sto je otislo van uporijebiti ya nesto drugo
public class A1_KUG_PTG_DUC : KUG_PTG_DUC
{
   #region Constructor

   public A1_KUG_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
   }

   #endregion Constructor

}


public class UGNorAUN_PTG_DUC : FakturPDUC // FakturExtDUC 
{
   internal PTG_Ugovor   PtgUgovor_rec     { get; set; }
   internal List<Rtrans> RtransList_allDOD { get; set; }

   #region Constructor

   public UGNorAUN_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      this.PtgUgovor_rec = new PTG_Ugovor(faktur_rec);

      this.RtransList_allDOD = new List<Rtrans>();

   //dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
   //(Faktur.tt_colName, new string[]
   //{
   //   Faktur.TT_UGN
   //});

      ThePolyGridTabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(IfShould_Load_PTG_OPL_Grid);
      ThePolyGridTabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(IfShould_Load_PTG_DOD_Grid);
      ThePolyGridTabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(IfShould_Load_PTG_UNA_ANA_Grid);
      ThePolyGridTabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(IfShould_Load_PTG_UNA_SIN_Grid);

      //ThePolyGridTabControl.TabPages["Osnovno"].Title = ptgOsn_TabPageName;

      CreateHamper_SumUGAN_PTG();

      hamp_DodAndKopCount.Location = new Point(TheG.Left, ZXC.Qun4);

      ThePolyGridTabControl.SelectionChanged += ThePolyGridTabControl_SelectionChanged_SupressSelectingDisabledTabs;
   }

   private void ThePolyGridTabControl_SelectionChanged_SupressSelectingDisabledTabs(Crownwood.DotNetMagic.Controls.TabControl theTabControl, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   {
      if(newPage.Enabled == false)
      { 
         theTabControl.SelectedIndex = theTabControl.TabPages.IndexOf(oldPage); // vrati ga nazad 
      }
   }


   #endregion Constructor

   #region Fieldz

   public VvHamper hamp_TT_PTG, hamp_v1TT_PTG, hamp_v2TT_PTG, hamp_partner_PTG, hamp_ugovor_PTG, hamp_zaFaktur_PTG, 
                   hamp_ugovorCbx_PTG, hamp_KUGpartner_PTG, hamp_kontakt_PTG, hamp_napomena_PTG,
                   hamp_sklad_PTG, hamp_R_iznosi_PTG, hamp_R_DodKop_PTG, hamp_datumi_PTG, hamp_mjIsporuke_PTG,
                   hamp_potpisan_PTG, hamp_tekstZaRacun_PTG, hamp_posredProviz_PTG, hamp_DodAndKopCount/*, hamp_otPlanRows*/;

   public VvTextBox tbx_R_iznosOtkupa, tbx_KDCFunkc_PTG, tbx_KDCEmail_PTG, tbx_KDCTel_PTG, tbx_KDCnaziv_PTG,
                    tbx_NajamNaRok, tbx_NajamNaRokOpis, tbx_DanFakturiranja, tbx_DanFakturiranjaOpis, tbx_vrstaNajma, tbx_vrstaNajmaOpis,
                    tbx_R_iznosUGAN, tbx_R_iznosRate, tbx_R_iznosUGAN_NV,
                    tbx_R_DodCount, tbx_R_KopCount, tbx_Napomena_PTG, tbx_opaskaServisa_PTG, tbx_R_DodCount_2, tbx_R_KopCount_2,
                    tbx_OrigNV, tbx_OrigIznos, tbx_OrigRata, tbx_IznosNV, tbx_UkIznos, tbx_Rata, tbx_korekcijaRata_PTG, tbx_trajanjeUgovora_PTG, tbx_fakRate_PTG, tbx_neFakrate_PTG,
                    tbx_OsigPlacanja, tbx_PTG_OsigPlOpis,
                    tbx_NumOf_OtPlan_Rows_UK, tbx_NumOf_OtPlan_Rows_UgPredRata, tbx_NumOf_OtPlan_Rows_UgBrojRata, tbx_NumOf_OtPlan_Rows_DodPredRata, tbx_NumOf_OtPlan_Rows_KorekRata, 
                    tbx_NumOf_OtPlan_Rows_OtkupRata, tbx_NumOf_OtPlan_Rows_OsnovRate
                    ;

   public RadioButton rbt_mjIsp_Korisnik, rbt_mjIsp_PcToGo, rbt_mjIsp_KorisOvers,
                      rbt_vrsta_Mjesecno, rbt_vrsta_Dnevno,
                      rbt_Faktur_NaDanUgovora     ,
                      rbt_Faktur_PrviDanMjeseca   ,
                      rbt_Faktur_ZadnjiDanMjeseca ;

   private VvCheckBox cbxV_PTG_1_isPovOprBezPen,
                      cbxV_PTG_2_isPodnajam,
                      cbxV_PTG_3_isOsigOpreme,
                      cbxV_PTG_4_isNovaOprema,
                      cbxV_PTG_5_isPotpisan,
                      cbxV_PTG_6_isVracenaOprema,
                      cbxV_PTG_7_isOtkup,
                      cbxV_PTG_8_isIspisnoRj
                      ;

   private VvTextBox tbx_PTG_1_isPovOprBezPen ,
                     tbx_PTG_2_isPodnajam     ,
                     tbx_PTG_3_isOsigOpreme   ,
                     tbx_PTG_4_isNovaOprema   ,
                     tbx_PTG_5_isPotpisan     ,
                     tbx_PTG_6_isVracenaOprema,
                     tbx_PTG_7_isOtkup        ,
                     tbx_PTG_8_isIspisnoRj
                     ;
   #endregion Fieldz

   #region Hampers

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      InitializeHamper_Partner_PTG       (out hamp_partner_PTG);
      InitializeHamper_MjIsporuke_PTG    (out hamp_mjIsporuke_PTG);
      InitializeHamper_TT_PTG            (out hamp_TT_PTG);
    //InitializeHamper_v1TT_PTG          (out hamp_v1TT_PTG);
    //InitializeHamper_KUGpartner_PTG    (out hamp_KUGpartner_PTG);
    //InitializeHamper_v2TT_PTG          (out hamp_v2TT_PTG);
      InitializeHamper_Ugovor_PTG        (out hamp_ugovor_PTG);
      InitializeHamper_Datumi_PTG        (out hamp_datumi_PTG);
      InitializeHamper_Fakturiranje_PTG  (out hamp_zaFaktur_PTG);
      InitializeHamper_UgovorCbx_PTG     (out hamp_ugovorCbx_PTG);
      InitializeHamper_Kontakt_PTG       (out hamp_kontakt_PTG);
      InitializeHamper_Napomena_PTG      (out hamp_napomena_PTG);
      InitializeHamper_Skladista_PTG     (out hamp_sklad_PTG);
    //InitializeHamper_R_Iznosi_PTG      (out hamp_R_iznosi_PTG);
      InitializeHamper_DodAndKopCount_PTG(out hamp_R_DodKop_PTG);
      InitializeHamper_UgovorPotpisan_PTG(out hamp_potpisan_PTG);
      InitializeHamper_PosredProviz_PTG  (out hamp_posredProviz_PTG);
      InitializeHamper_TekstZaRacun_PTG  (out hamp_tekstZaRacun_PTG);

      CreateArrOfHampers();

      SetParentOfHamperLeftHampers();

      panel_MigratorsLeftB.SendToBack();
           
      hamp_partner_PTG   .Location = new Point(                     0 ,        0);
      hamp_mjIsporuke_PTG.Location = new Point(hamp_partner_PTG.Left, hamp_partner_PTG.Bottom - hamp_mjIsporuke_PTG.Height);
      hamp_mjIsporuke_PTG.BringToFront();

      hamp_datumi_PTG    .Location = new Point(hamp_partner_PTG .Left , hamp_partner_PTG.Bottom);
      hamp_TT_PTG        .Location = new Point(hamp_partner_PTG .Right,                        0);

    //int hamp_v2TT_PTGLeft = hamp_TT_PTG.Right - hamp_v2TT_PTG.Width;
    //int hamp_v1TT_PTGLeft = hamp_TT_PTG.Right - hamp_v2TT_PTG.Width - hamp_v1TT_PTG.Width;
      
    //hamp_v2TT_PTG      .Location = new Point(hamp_v2TT_PTGLeft      , hamp_TT_PTG      .Bottom);
    //hamp_v1TT_PTG      .Location = new Point(hamp_v1TT_PTGLeft      , hamp_TT_PTG      .Bottom);
    //hamp_ugovor_PTG    .Location = new Point(hamp_TT_PTG       .Left, hamp_v2TT_PTG    .Bottom);
      hamp_ugovor_PTG    .Location = new Point(hamp_TT_PTG       .Left, hamp_TT_PTG    .Bottom);
      hamp_zaFaktur_PTG  .Location = new Point(hamp_ugovor_PTG   .Left, hamp_ugovor_PTG  .Bottom);
      hamp_ugovorCbx_PTG .Location = new Point(hamp_datumi_PTG    .Right, hamp_datumi_PTG .Top);
      hamp_napomena_PTG  .Location = new Point(hamp_ugovorCbx_PTG.Right, hamp_partner_PTG.Bottom);

      hamp_R_DodKop_PTG  .Location = new Point(hamp_TT_PTG.Right, hamp_TT_PTG.Top);
      hamp_R_DodKop_PTG.BringToFront();

      hamp_potpisan_PTG.Location = new Point(hamp_R_DodKop_PTG.Right, hamp_R_DodKop_PTG.Top);

      hamp_tekstZaRacun_PTG.Location = new Point(hamp_napomena_PTG.Right, hamp_napomena_PTG.Bottom - hamp_tekstZaRacun_PTG.Height);
      hamp_posredProviz_PTG.Location = new Point(hamp_tekstZaRacun_PTG.Right - hamp_posredProviz_PTG.Width, hamp_zaFaktur_PTG.Bottom);

      hamp_kontakt_PTG.Location = new Point(hamp_potpisan_PTG.Right - hamp_kontakt_PTG.Width, hamp_zaFaktur_PTG.Top);

      hamp_sklad_PTG.Location = new Point(hamp_potpisan_PTG.Right - hamp_sklad_PTG.Width, hamp_kontakt_PTG.Bottom + ZXC.Qun2);
    
      //hamp_R_iznosi_PTG.Location = new Point(hamp_kontakt_PTG.Right - hamp_R_iznosi_PTG.Width, hamp_kontakt_PTG.Bottom);

      //hamp_dokNum.Location = new Point(hamp_R_iznosi_PTG.Right, hamp_TT_PTG.Top);

    //hamp_KUGpartner_PTG.Location = new Point(hamp_TT_PTG.Left, hamp_TT_PTG.Bottom);

      nextY = hamp_napomena_PTG.Bottom;

      hamp_twin.Visible = false;

      InitializeHamper_KOPandDODcount(out hamp_DodAndKopCount);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_partner_PTG, hamp_mjIsporuke_PTG, hamp_TT_PTG, hamp_dokNum, //hamp_v1TT_PTG, hamp_v2TT_PTG,
                                    hamp_ugovor_PTG, hamp_datumi_PTG, hamp_zaFaktur_PTG, hamp_ugovorCbx_PTG, 
                                    /*hamp_KUGpartner_PTG,*/ hamp_kontakt_PTG, hamp_napomena_PTG,
                                    hamp_sklad_PTG,/* hamp_R_iznosi_PTG,*/ hamp_R_DodKop_PTG, hamp_potpisan_PTG, hamp_posredProviz_PTG, hamp_tekstZaRacun_PTG
                                  };
   }

   private void InitializeHamper_TT_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 1, "", null, false);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q2un + ZXC.Qun4, ZXC.Q4un - ZXC.Qun4, ZXC.QUN + ZXC.Qun12, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun2 };
      hamper.VvSpcBefCol   = new int[] { faBefCol,            faBefCol,            faBefCol, ZXC.Qun12          , faBefCol           , faBefCol            };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN + ZXC.Qun8 };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
      hamper.VvBottomMargin = ZXC.Qun8;

      bool isUGN = this is UGO_PTG_DUC;
      string labTip = isUGN ? "UgovorBr:" : "AneksBr:";

               hamper.CreateVvLabel        (0, 0, labTip, ContentAlignment.MiddleRight);
      tbx_TT = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_tt", "Tip ugovora");
      // Nota bene; buduci je tbx_tt VvLookUp TextBox onde mu je tbx_tt.JAM_FieldExitMethod 'potrosena' lookUp methodom. 
      //tbx_tt.JAM_FieldExitMethod_2 = new EventHandler(OnExit_ValidateTTrestrictor);
      tbx_TT.JAM_FieldExitWithValidationMethod += new System.ComponentModel.CancelEventHandler(OnExit_ValidateTTrestrictor);

    //tbx_TtOpis = hamper.CreateVvTextBox(2, 0, "tbx_ttOpis", "");
      tbx_TtNum  = hamper.CreateVvTextBox(2, 0, "tbx_ttNum", "Ovo bolje ostavi kako je...", 7); // !!! 
      tbx_TT.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_TT.JAM_DataRequired = true;
      tbx_TT.JAM_MustTabOutBeforeSubmit = true;
    //tbx_TtOpis.JAM_ReadOnly = true;

      tbx_TtNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_TT.JAM_ReadOnly = true;
    //tbx_TT.JAM_lui_NameTaker_JAM_Name = tbx_TtOpis.JAM_Name;
      tbx_TT.JAM_ttNumTaker_JAM_Name = tbx_TtNum.JAM_Name;

      tbx_TtNum.JAM_IsSupressTab = true;

      tbx_TT.Font     = ZXC.vvFont.BaseBoldFont;
    //tbx_TtOpis.Font = ZXC.vvFont.BaseBoldFont;
      tbx_TtNum.Font  = ZXC.vvFont.LargeBoldFont;

      tbx_TtNum.TextAlign = HorizontalAlignment.Right;

      tbx_v1_ttNum = hamper.CreateVvTextBox(3, 0, "tbx_v1_ttNum", "Broj Krovnog Ugovora", 2/*GetDB_ColumnSize(DB_ci.iz_ttNum)*/);
      tbx_v1_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_v1_ttNum.JAM_IsSupressTab = true;
      tbx_v1_ttNum.JAM_MarkAsNumericTextBox(0, false, 1, 40, false);
      tbx_v1_ttNum.TextAlign = HorizontalAlignment.Right;
      tbx_v1_ttNum.JAM_ReadOnly = true;

      tbx_v2_ttNum = hamper.CreateVvTextBox(4, 0, "tbx_v2_ttNum", "Broj Ugovora/Aneksa", 5/*GetDB_ColumnSize(DB_ci.na_ttNum)*/);
      tbx_v2_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_v2_ttNum.JAM_IsSupressTab = true;
      tbx_v2_ttNum.JAM_ReadOnly = true;

      tbx_v2_ttNum.Font = ZXC.vvFont.BaseBoldFont;
      tbx_v2_ttNum.TextAlign = HorizontalAlignment.Right;

      tbx_Konto = hamper.CreateVvTextBox(5, 0, "tbx_PTG_KugPartner", "Partner Krovnog Ugovora");
      tbx_Konto.JAM_IsSupressTab = true;
      tbx_Konto.JAM_ReadOnly = true;
      tbx_Konto.Visible = !isUGN;
   }

   private void InitializeHamper_v1TT_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", null, false);
      
      hamper.VvColWdt     = new int[] { ZXC.QUN + ZXC.Qun12 };
      hamper.VvSpcBefCol  = new int[] { 0   };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

//                      hamper.CreateVvLabel        (0, 0, "KUG:"       , ContentAlignment.MiddleRight);
      tbx_v1_ttNum  = hamper.CreateVvTextBox      (0, 0, "tbx_v1_ttNum", "Broj Krovnog Ugovora", 2/*GetDB_ColumnSize(DB_ci.iz_ttNum)*/);
      tbx_v1_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_v1_ttNum.JAM_IsSupressTab = true;
      tbx_v1_ttNum.JAM_MarkAsNumericTextBox(0, false, 1, 40, false);
      tbx_v1_ttNum.TextAlign = HorizontalAlignment.Right;
      tbx_v1_ttNum.JAM_ReadOnly = true;

      //tbx_PTG_KugPartner = hamper.CreateVvTextBox(2, 0, "tbx_PTG_KugPartner", "Partner krovnog ugovora");
      
   }

   private void InitializeHamper_KUGpartner_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", null, false);
      
      hamper.VvColWdt     = new int[] { labelWidth + ZXC.QUN + ZXC.Qun4, ZXC.Q9un + ZXC.QUN + ZXC.Qun4 + faBefCol};
      hamper.VvSpcBefCol  = new int[] { faBefCol, faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

    //tbx_PTG_KugPartner = hamper.CreateVvTextBox(1, 0, "tbx_PTG_KugPartner", "Partner Krovnog Ugovora");
    //tbx_PTG_KugPartner.JAM_IsSupressTab = true;
    //tbx_PTG_KugPartner.JAM_ReadOnly = true;
      tbx_Konto = hamper.CreateVvTextBox(1, 0, "tbx_PTG_KugPartner", "Partner Krovnog Ugovora");
      tbx_Konto.JAM_IsSupressTab = true;
      tbx_Konto.JAM_ReadOnly = true;

      hamper.Visible = (this is ANU_PTG_DUC) ? true : false;    
   }

   private void InitializeHamper_v2TT_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", null, false);

      hamper.VvColWdt      = new int[] { ZXC.Q3un - ZXC.Qun2};
      hamper.VvSpcBefCol   = new int[] { faBefCol           };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

     // string lblText = (this is UGO_PTG_DUC) ? "UGN:" : "AUN:";
     //
     //                hamper.CreateVvLabel  (0, 0, lblText      , ContentAlignment.MiddleRight);
      tbx_v2_ttNum = hamper.CreateVvTextBox(0, 0, "tbx_v2_ttNum", "Broj Ugovora/Aneksa", 5/*GetDB_ColumnSize(DB_ci.na_ttNum)*/);
      tbx_v2_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_v2_ttNum.JAM_IsSupressTab = true;
      tbx_v2_ttNum.JAM_ReadOnly     = true;

      tbx_v2_ttNum.Font      = ZXC.vvFont.BaseBoldFont;
      tbx_v2_ttNum.TextAlign = HorizontalAlignment.Right;
   }

   private void InitializeHamper_Partner_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(8, 5, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth + ZXC.QUN, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun4, ZXC.Q6un, ZXC.Q4un, ZXC.Q4un, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] {        faBefFirstCol,            faBefCol,            faBefCol, faBefCol, faBefCol, faBefCol, faBefCol, faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN + ZXC.Qun8,  ZXC.QUN,  ZXC.QUN,  ZXC.QUN,  ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] {           ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                     hamper.CreateVvLabel    (0, 0, "Partner:", ContentAlignment.MiddleRight);
      tbx_KupdobCd = hamper.CreateVvTextBox  (1, 0, "tbx_kupdobCd"  , "Sifra partnera" , 6);
      tbx_KupdobTk = hamper.CreateVvTextBox  (2, 0, "tbx_kupdobTk"  , "Ticker partnera", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.kupdobTK));
      tbx_KupdobName = hamper.CreateVvTextBox(3, 0, "tbx_kupdobName", "Naziv partnera" , GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.kupdobName), 2, 0);
      tbx_KupdobCd.JAM_MustTabOutBeforeSubmit =
      tbx_KupdobTk.JAM_MustTabOutBeforeSubmit =
      tbx_KupdobName.JAM_MustTabOutBeforeSubmit = true;
      tbx_KupdobCd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_KupdobTk.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_KupdobName.JAM_Highlighted = true;

      this.ControlForInitialFocus = tbx_KupdobName;

      tbx_KupdobCd  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyKupdobTextBoxLeave));
      tbx_KupdobTk  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_KupdobName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyKupdobTextBoxLeave));

      tbx_KupdobCd.DoubleClick += new EventHandler(AnyKupdobTextBox_DoubleClick);
      tbx_KupdobTk.DoubleClick += new EventHandler(AnyKupdobTextBox_DoubleClick);
      tbx_KupdobName.DoubleClick += new EventHandler(AnyKupdobTextBox_DoubleClick);
      tbx_KupdobName.Font = ZXC.vvFont.LargeBoldFont;

      tbx_KupdobUlica = hamper.CreateVvTextBox (1, 0, "tbx_KupdobUlica ", "KupdobUlica ", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.kdUlica));
      tbx_KupdobZip   = hamper.CreateVvTextBox (1, 0, "tbx_KupdobZip   ", "KupdobZip   ", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.kdZip));
      tbx_KupdobMjesto = hamper.CreateVvTextBox(1, 0, "tbx_KupdobMjesto", "KupdobMjesto", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.kdMjesto));
      tbx_KupdobUlica .Visible =
      tbx_KupdobZip   .Visible =
      tbx_KupdobMjesto.Visible = false;


                     hamper.CreateVvLabel  (0, 1, "AdrFaktur:", ContentAlignment.MiddleRight);
      tbx_KdAdresa = hamper.CreateVvTextBox(1, 1, "kdAdresa", "Adresa partnera", 74, 2, 0);

                  hamper.CreateVvLabel  (4, 1, "OIB:", ContentAlignment.MiddleRight);
      tbx_KdOib = hamper.CreateVvTextBox(5, 1, "kdOib", "Oib", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.kdOib));

      tbx_KdOib.JAM_IsSupressTab = tbx_KdAdresa.JAM_IsSupressTab = true;
      tbx_KdOib.JAM_ReadOnly     = tbx_KdAdresa.JAM_ReadOnly     = true;

      // !!!! dopuni PutAllKupdobFields ZA napomenu sa Kupdoba

                         hamper.CreateVvLabel  (0, 2, "PoslJed:", ContentAlignment.MiddleRight);
      tbx_PosJedCd     = hamper.CreateVvTextBox(1, 2, "tbx_posJedCd", "Sifra poslovne jedinice", 6);
      tbx_PosJedTk     = hamper.CreateVvTextBox(2, 2, "tbx_posJedTk", "Ticker poslovne jedinice", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.posJedTK));
      tbx_PosJedName   = hamper.CreateVvTextBox(3, 2, "tbx_posJedName", "Naziv oslovne jedinice", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.posJedName));
                         hamper.CreateVvLabel  (4, 2, "AdresaPJ:", ContentAlignment.MiddleRight);
      tbx_PosJedAdresa = hamper.CreateVvTextBox(5, 2, "tbx_posJedUlica", "Ulica partnera - poslovne jedinice", 74, 2, 0);

      tbx_PosJedCd    .JAM_ReadOnly =
      tbx_PosJedTk    .JAM_ReadOnly =
      tbx_PosJedName  .JAM_ReadOnly =
      tbx_PosJedAdresa.JAM_ReadOnly = true;

                        hamper.CreateVvLabel  (6, 0, "Rabat:", ContentAlignment.MiddleRight);
      tbx_somePercent = hamper.CreateVvTextBox(7, 0, "tbx_somePercent", "", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.somePercent));//PTG_RabatKupcaPosto 
      tbx_somePercent.JAM_IsForPercent = true;
      tbx_somePercent.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);

                    hamper.CreateVvLabel  (6, 1, "OdgodaPl:", ContentAlignment.MiddleRight);
      tbx_RokPlac = hamper.CreateVvTextBox(7, 1, "tbx_RokPlac", "Odgoda plaćanja", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.rokPlac));//PTG_OdgodaPl
      tbx_RokPlac.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_RokPlac.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

                       hamper.CreateVvLabel  (0, 3, "Narudžba:", ContentAlignment.MiddleRight);
      tbx_OpciAlabel = hamper.CreateVvTextBox(1, 3, "tbx_OpciAlabel", "Narudžba", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.opciAlabel), 2, 0);//PTG_Narudzba

                     hamper.CreateVvLabel  (4, 3, "Lokacija:", ContentAlignment.MiddleRight);
      tbx_DostAddr = hamper.CreateVvTextBox(5, 3, "tbx_DostAddr", "Adresa dostave", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.dostAddr), 2, 0);//PTG_Lokacija



                    hamper.CreateVvLabel        (6, 4, "IzvVal:", ContentAlignment.MiddleRight);
      tbx_ValName = hamper.CreateVvTextBoxLookUp(7, 4, "tbx_ValName", "Naziv devizne valute", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.devName));
      tbx_ValName.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_ValName.JAM_Set_LookUpTable(ZXC.luiListaDeviza, (int)ZXC.Kolona.prva);
      // Nota bene; buduci je tbx_DevName VvLookUp TextBox onde mu je tbx_tt.JAM_FieldExitMethod 'potrosena' lookUp methodom. 
      tbx_ValName.JAM_FieldExitMethod_2 = new EventHandler(OnExit_DevName_SetValutaNameInUse);
      tbx_ValName.JAM_IsSupressTab = true;

   }

   private void InitializeHamper_MjIsporuke_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth + ZXC.QUN, ZXC.Q3un+ZXC.Qun2, ZXC.Q6un, ZXC.Q3un + ZXC.Qun2 };
      hamper.VvSpcBefCol   = new int[] { faBefCol, faBefCol, faBefCol, faBefCol};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                             hamper.CreateVvLabel      (0, 0,       "MjestoIsp:"       , ContentAlignment.MiddleRight);
      rbt_mjIsp_Korisnik   = hamper.CreateVvRadioButton(1, 0, null, "Korisnik"         , TextImageRelation.ImageBeforeText);
      rbt_mjIsp_KorisOvers = hamper.CreateVvRadioButton(2, 0, null, "Korisnik-Overseas", TextImageRelation.ImageBeforeText);
      rbt_mjIsp_PcToGo     = hamper.CreateVvRadioButton(3, 0, null, "PCTOGO"           , TextImageRelation.ImageBeforeText);
      rbt_mjIsp_Korisnik.Checked = true;
      rbt_mjIsp_Korisnik.Tag = true;

   }

   private void InitializeHamper_Ugovor_PTG(out VvHamper hamper)
   {
    //hamper = new VvHamper( 6, 3, "", null, false);
      hamper = new VvHamper(10, 3, "", null, false);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q2un, ZXC.Q2un, ZXC.Q4un, ZXC.Q3un + ZXC.Qun4, ZXC.Q2un- ZXC.Qun4, ZXC.Qun2  , ZXC.QUN+ ZXC.Qun4, ZXC.Qun2  , ZXC.QUN + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { faBefCol, faBefCol, faBefCol, faBefCol,            faBefCol,           faBefCol, ZXC.Qun12 , ZXC.Qun12, ZXC.Qun12, ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN   };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                    hamper.CreateVvLabel  (0, 0, "DatUgovor:", ContentAlignment.MiddleRight);
      tbx_DokDate = hamper.CreateVvTextBox(1, 0, "tbx_dokDate", "Datum ugovora",12, 1,0);
      tbx_DokDate.JAM_IsForDateTimePicker = true;
      dtp_DokDate = hamper.CreateVvDateTimePicker(1, 0, "", 1, 0, tbx_DokDate);//PTG_DatUgovora     
      dtp_DokDate.Name = "dtp_DokDate";
      tbx_DokDate.Font = ZXC.vvFont.BaseBoldFont;

      dtp_DokDate.ValueChanged += new EventHandler(dtp_DokDate_ValueChanged_SetSkladAndPdvDate);

      //10.06.2022. zamjenil smo mjesta tbx-ovima i vazan je PTG_BrojRata/rokPonude jer na osnovu njega racunam o opl a ovaj drugi je samo informativan
      hamper.CreateVvLabel  (4, 0, "TrajanjeUg:", ContentAlignment.MiddleRight);
      tbx_RokPonude = hamper.CreateVvTextBox(5, 0, "tbx_RokPonude", "Broj rata", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.rokPonude)); //PTG_BrojRata
      tbx_RokPonude.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_RokPonude.JAM_FieldExitMethod = new EventHandler(PTG_BrojRataExitMethod);


      hamper.CreateVvLabel(4, 1, "UgBrojRata:", ContentAlignment.MiddleRight);
      tbx_trajanjeUgovora_PTG = hamper.CreateVvTextBox(5, 1, "tbx_trajanjeUgovora_PTG", "Trajanje ugovore u danima/mjesecima", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.cjenTTnum));//PTG_TrajanjeUg
      tbx_trajanjeUgovora_PTG.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
   

                              hamper.CreateVvLabel  (4, 2, "KorekRata:", ContentAlignment.MiddleRight);
      tbx_korekcijaRata_PTG = hamper.CreateVvTextBox(5, 2, "tbx_korekcijaRata_PTG", "Korekcija broja rata", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.rokIsporuke)); //PTG_KorekcijaBrojRata
      tbx_korekcijaRata_PTG.JAM_CharEdits = ZXC.JAM_CharEdits.NumericOnly;
      tbx_korekcijaRata_PTG.JAM_ReadOnly = true;


      hamper.CreateVvLabel        (0, 1, "Vrsta najma:", ContentAlignment.MiddleRight);
      tbx_vrstaNajma     = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_vrstaNajma", "Vrsta najma", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.opciBlabel));
      tbx_vrstaNajmaOpis = hamper.CreateVvTextBox      (2, 1, "tbx_vrstaNajmaOpis", "Vrsta najma", 32, 1, 0);
      tbx_vrstaNajma.JAM_Set_LookUpTable(ZXC.luiListaPTG_VrstaNajma, (int)ZXC.Kolona.prva);
      tbx_vrstaNajma.JAM_lui_NameTaker_JAM_Name = tbx_vrstaNajmaOpis.JAM_Name;
      tbx_vrstaNajmaOpis.JAM_ReadOnly = true;

                           hamper.CreateVvLabel        (0, 2, "Na rok:", ContentAlignment.MiddleRight);
      tbx_NajamNaRok     = hamper.CreateVvTextBoxLookUp(1, 2, "tbx_NajamNaRok", "Najam na rok", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.opciBvalue));
      tbx_NajamNaRokOpis = hamper.CreateVvTextBox      (2, 2,"tbx_NajamNaRokOpis", "Najam na rok", 32, 1, 0);
      tbx_NajamNaRok.JAM_Set_LookUpTable(ZXC.luiListaPTG_NajamNaRok, (int)ZXC.Kolona.prva);
      tbx_NajamNaRok.JAM_lui_NameTaker_JAM_Name = tbx_NajamNaRokOpis.JAM_Name;
      tbx_NajamNaRokOpis.JAM_ReadOnly = true;

   }

   public void PTG_BrojRataExitMethod(object sender, EventArgs e)
   {
      if(Fld_RokPonude.NotZero()) Fld_PTG_trajanjeUgovora = (uint)Fld_RokPonude;
   }

   private void InitializeHamper_UgovorPotpisan_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 2, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth + ZXC.Q6un, ZXC.QUN - ZXC.Qun10 };
      hamper.VvSpcBefCol   = new int[] { faBefFirstCol, 0 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN ,  ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Potpisan"     , ContentAlignment.MiddleRight); tbx_PTG_5_isPotpisan      = hamper.CreateVvTextBox(1, 0, "PTG_6pdvNum_isPotpisan     ", "Potpisan"      ); cbxV_PTG_5_isPotpisan       = hamper.CreateVvCheckBox(1, 0, "", tbx_PTG_5_isPotpisan     , "", "X");
      hamper.CreateVvLabel(0, 1, "OpremaVraćena", ContentAlignment.MiddleRight); tbx_PTG_6_isVracenaOprema = hamper.CreateVvTextBox(1, 1, "PTG_7pdvNum_isVracenaOprema", "Oprema vraćena");  cbxV_PTG_6_isVracenaOprema = hamper.CreateVvCheckBox(1, 1, "", tbx_PTG_6_isVracenaOprema, "", "X");
      tbx_PTG_5_isPotpisan     .TextAlign = HorizontalAlignment.Center;
      tbx_PTG_6_isVracenaOprema.TextAlign = HorizontalAlignment.Center;

      // tbx_PTG_9pdvNum_isIspisnoRj    .TextAlign = HorizontalAlignment.Center;
      // hamper.CreateVvLabel(1, 8, "IspisnoRj"     , 1, 0,    ContentAlignment.MiddleRight); tbx_PTG_9pdvNum_isIspisnoRj     = hamper.CreateVvTextBox(3, 8, "PTG_9pdvNum_isIspisnoRj    ", "IspisnoRj"     );  cbxV_PTG_9pdvNum_isIspisnoRj     = hamper.CreateVvCheckBox(3, 8, "", tbx_PTG_9pdvNum_isIspisnoRj    , "", "X"); tbx_PTG_9pdvNum_isIspisnoRj    .JAM_Highlighted = true; 

   }

   private void InitializeHamper_Fakturiranje_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", null, false);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q2un, ZXC.Q6un + faBefCol };
      hamper.VvSpcBefCol   = new int[] { faBefCol, faBefCol, faBefCol            };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                                hamper.CreateVvLabel        (0, 0, "DatZaFaktur:", ContentAlignment.MiddleRight);
      tbx_DanFakturiranja     = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_DanZaFakturiranje", "tbx_DanZaFakturiranje", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.opciAvalue));
      tbx_DanFakturiranjaOpis = hamper.CreateVvTextBox      (2, 0, "tbx_DanFakturiranjaOpis", "Vrsta najma", 32);
      tbx_DanFakturiranja.JAM_Set_LookUpTable(ZXC.luiListaPTG_DanZaFaktur, (int)ZXC.Kolona.prva);
      tbx_DanFakturiranja.JAM_lui_NameTaker_JAM_Name = tbx_DanFakturiranjaOpis.JAM_Name;
      tbx_DanFakturiranjaOpis.JAM_ReadOnly = true;

      if(this is ANU_PTG_DUC) tbx_DanFakturiranja.JAM_ReadOnly = true;
   }

   private void InitializeHamper_Datumi_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 6, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth + ZXC.QUN, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { faBefCol     ,        faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN ,ZXC.QUN ,ZXC.QUN ,ZXC.QUN ,ZXC.QUN ,ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8,ZXC.Qun8,ZXC.Qun8,ZXC.Qun8,ZXC.Qun8,ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      
                      hamper.CreateVvLabel  (0, 0, "DatSkidSkl:", ContentAlignment.MiddleRight);
      tbx_SkladDate = hamper.CreateVvTextBox(1, 0, "tbx_SkladDate", "Datum skidanja sa skladišta");
      tbx_SkladDate.JAM_IsForDateTimePicker = true;
      dtp_SkladDate = hamper.CreateVvDateTimePicker(1, 0, "", tbx_SkladDate);//PTG_DatSkidSaSklad 
      dtp_SkladDate.Name = "dtp_SkladDate";
      tbx_SkladDate.JAM_IsSupressTab = true;


                     hamper.CreateVvLabel  (0, 1, "DatDostave:", ContentAlignment.MiddleRight);
      tbx_DokDate2 = hamper.CreateVvTextBox(1, 1, "tbx_DokDate2", "Datum dostave");
      tbx_DokDate2.JAM_IsForDateTimePicker = true;
      dtp_DokDate2 = hamper.CreateVvDateTimePicker(1, 1, "", tbx_DokDate2);//PTG_DatDostave     
      dtp_DokDate2.Name = "dtp_DokDate2";

                     hamper.CreateVvLabel  (0, 2, "DatPrvogRn:", ContentAlignment.MiddleRight);
      tbx_DospDate = hamper.CreateVvTextBox(1, 2, "tbx_DospDate", "Datum prvog računa");
      tbx_DospDate.JAM_IsForDateTimePicker = true;
      dtp_DospDate = hamper.CreateVvDateTimePicker(1, 2, "", tbx_DospDate);//PTG_DatPrvogRn
      dtp_DospDate.Name = "dtp_DospDate";
      tbx_DospDate.JAM_ReadOnly = true;

                      hamper.CreateVvLabel  (0, 3, "DatZadRn:", ContentAlignment.MiddleRight);
      tbx_PonudDate = hamper.CreateVvTextBox(1, 3, "tbx_PonudDate", "Datum zadnjeg računa");
      tbx_PonudDate.JAM_IsForDateTimePicker = true;
      dtp_PonudDate = hamper.CreateVvDateTimePicker(1, 3, "", tbx_PonudDate);//PTG_DatZadnjegRn   
      dtp_PonudDate.Name = "dtp_PonudDate";
      tbx_PonudDate.JAM_ReadOnly = true;

                       hamper.CreateVvLabel  (0, 4, "DatIsteka:", ContentAlignment.MiddleRight);
      tbx_RokIspDate = hamper.CreateVvTextBox(1, 4, "tbx_RokIspDate", "Datum isteka ugovora/aneksa");
      tbx_RokIspDate.JAM_IsForDateTimePicker = true;
      dtp_RokIspDate = hamper.CreateVvDateTimePicker(1, 4, "", tbx_RokIspDate); //PTG_datIstekaUg    
      dtp_RokIspDate.Name = "dtp_RokIspDate";

                  hamper.CreateVvLabel  (0, 5, "DatPovOpr:", ContentAlignment.MiddleRight);
      tbx_dateX = hamper.CreateVvTextBox(1, 5, "tbx_dateX", "Datum povratka");
      tbx_dateX.JAM_IsForDateTimePicker = true;
      dtp_dateX = hamper.CreateVvDateTimePicker(1, 5, "", tbx_dateX);//PTG_DatPovrataOpr  
      dtp_dateX.Name = "dtp_dateX";

      hamper.BringToFront();
   }

   private void InitializeHamper_UgovorCbx_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 6, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth, ZXC.Q2un, ZXC.Q3un, ZXC.QUN-ZXC.Qun10 };
      hamper.VvSpcBefCol   = new int[] { faBefFirstCol,  faBefCol, faBefCol, 0 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN,  ZXC.QUN,  ZXC.QUN,  ZXC.QUN,  ZXC.QUN,  ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "PovrOprBezPenala", 2, 0, ContentAlignment.MiddleRight); tbx_PTG_1_isPovOprBezPen  = hamper.CreateVvTextBox(3, 0, "PTG_1pdvNum_isPovOprBezPen ", "PovrOprBezPen" ); cbxV_PTG_1_isPovOprBezPen   = hamper.CreateVvCheckBox(3, 0, "", tbx_PTG_1_isPovOprBezPen , "", "x"); //tbx_PTG_1pdvNum_isPovOprBezPen .JAM_Highlighted = true;
    
                              hamper.CreateVvLabel(0, 1, "OsigPlaćanja"    ,       ContentAlignment.MiddleRight);
      tbx_OsigPlacanja      = hamper.CreateVvTextBoxLookUp (1, 1, "tbx_PTG_OsigPlacanja ", "OsigPlaćanja");
      tbx_PTG_OsigPlOpis    = hamper.CreateVvTextBox       (2, 1, "tbx_PTG_OsigPlOpis", "Vrsta osiguranja plaćanja", 32, 1, 0);
      tbx_OsigPlacanja.JAM_Set_LookUpTable(ZXC.luiListaPTG_OsigPlacanja, (int)ZXC.Kolona.prva);
      tbx_OsigPlacanja.JAM_lui_NameTaker_JAM_Name = tbx_PTG_OsigPlOpis.JAM_Name;
      tbx_PTG_OsigPlOpis.JAM_ReadOnly = true;
                  
      hamper.CreateVvLabel(0, 2, "Podnajam"        , 2, 0, ContentAlignment.MiddleRight); tbx_PTG_2_isPodnajam      = hamper.CreateVvTextBox(3, 2, "PTG_3pdvNum_isPodnajam     ", "Podnajam"      ); cbxV_PTG_2_isPodnajam       = hamper.CreateVvCheckBox(3, 2, "", tbx_PTG_2_isPodnajam     , "", "x"); //tbx_PTG_3pdvNum_isPodnajam     .JAM_Highlighted = true;
      hamper.CreateVvLabel(0, 3, "OsigOpreme"      , 2, 0, ContentAlignment.MiddleRight); tbx_PTG_3_isOsigOpreme    = hamper.CreateVvTextBox(3, 3, "PTG_4pdvNum_isOsigOpreme   ", "OsigOpreme"    ); cbxV_PTG_3_isOsigOpreme     = hamper.CreateVvCheckBox(3, 3, "", tbx_PTG_3_isOsigOpreme   , "", "x"); //tbx_PTG_4pdvNum_isOsigOpreme   .JAM_Highlighted = true;
      hamper.CreateVvLabel(1, 4, "Otkup"           ,/*1, 0,*/ ContentAlignment.MiddleRight); tbx_PTG_7_isOtkup      = hamper.CreateVvTextBox(3, 4, "PTG_8pdvNum_isOtkup        ", "Otkup"         );  cbxV_PTG_7_isOtkup         = hamper.CreateVvCheckBox(3, 4, "", tbx_PTG_7_isOtkup        , "", "x"); //tbx_PTG_8pdvNum_isOtkup        .JAM_Highlighted = true; 
      hamper.CreateVvLabel(0, 5, "NovaOprema"      , 2, 0, ContentAlignment.MiddleRight); tbx_PTG_4_isNovaOprema    = hamper.CreateVvTextBox(3, 5, "PTG_5pdvNum_isNovaOprema   ", "NovaOprema"    ); cbxV_PTG_4_isNovaOprema     = hamper.CreateVvCheckBox(3, 5, "", tbx_PTG_4_isNovaOprema   , "", "x"); //tbx_PTG_5pdvNum_isNovaOprema   .JAM_Highlighted = true;

      tbx_PTG_1_isPovOprBezPen .TextAlign = HorizontalAlignment.Center;
      tbx_PTG_2_isPodnajam     .TextAlign = HorizontalAlignment.Center;
      tbx_PTG_3_isOsigOpreme   .TextAlign = HorizontalAlignment.Center;
      tbx_PTG_4_isNovaOprema   .TextAlign = HorizontalAlignment.Center;
      tbx_PTG_7_isOtkup        .TextAlign = HorizontalAlignment.Center;

      tbx_decimal01 = hamper.CreateVvTextBox(2, 4, "tbx_decimal01", "", GetDB_ColumnSize(DB_ci.decimal01));//PTG_OtkupPosto      
      tbx_decimal01.JAM_IsForPercent = true;
      tbx_decimal01.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);

    //tbx_R_iznosOtkupa = hamper.CreateVvTextBox(2, 4, "tbx_R_iznosOtkupa", "", 12);
    //tbx_R_iznosOtkupa.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
    //tbx_R_iznosOtkupa.JAM_ReadOnly = true;
   }

   private void InitializeHamper_Kontakt_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 4, "", null, false);

      hamper.VvColWdt    = new int[] { labelWidth + ZXC.Qun2  , ZXC.Q7un };
      hamper.VvSpcBefCol = new int[] { faBefFirstCol,  faBefCol};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel  (0, 0, "ImePrezime:"     , ContentAlignment.MiddleRight);
      tbx_KDCnaziv_PTG = hamper.CreateVvTextBox(1, 0, "tbx_KDCnaziv_PTG", "Kontakt osoba", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.odgvPersName));//PTG_KontaktImePrez 
      tbx_KDCnaziv_PTG.JAM_SetAutoCompleteData(Xtrans.recordName, Xtrans.sorterKDCnaziv.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Xtrans_sorterKCDnaziv), new EventHandler(KDCnaziv_TextBoxLeave));
      tbx_KDCnaziv_PTG.VVtag2 = tbx_KupdobCd; // za Update_KdcXtrans 

                         hamper.CreateVvLabel  (0, 1, "Funkcija:", ContentAlignment.MiddleRight);
      tbx_KDCFunkc_PTG = hamper.CreateVvTextBox(1, 1, "tbx_kontaktFunkc_PTG", "Putnik");

                       hamper.CreateVvLabel  (0, 2, "Tel/Mob:"   , ContentAlignment.MiddleRight);
      tbx_KDCTel_PTG = hamper.CreateVvTextBox(1, 2, "tbx_kontaktTel_PTG", "Telefon/mobitel osobe za kontakt");

                         hamper.CreateVvLabel  (0, 3, "e-mail:", ContentAlignment.MiddleRight);
      tbx_KDCEmail_PTG = hamper.CreateVvTextBox(1, 3, "tbx_kontaktEmail_PTG", "e-mail kontakt osobe");

      tbx_KDCFunkc_PTG.JAM_ReadOnly =
      tbx_KDCTel_PTG  .JAM_ReadOnly =
      tbx_KDCEmail_PTG.JAM_ReadOnly = true;

   }

   private void KDCnaziv_TextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Xtrans  xtrans_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         if(tb.Text.IsEmpty())
         {
            Fld_PTG_KDCFunkc_PTG = Fld_PTG_KDCTel_PTG = Fld_PTG_KDCEmail_PTG = "";
            return;
         }

       //xtrans_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);
         xtrans_rec = new Xtrans();
       //bool found = ZXC.XtransDao.SetMe_Record_bySomeUniqueColumn(TheDbConnection, xtrans_rec, tb.Text, ZXC.XtransSchemaRows[ZXC.XtrCI.t_kpdbNameA_50], false, false);
         bool found = MixerDao.SetMeLast_KDCxtrans_ByKupdobCD_And_KDCname(TheDbConnection, xtrans_rec, Fld_KupdobCd, tb.Text);

         if(!found)
         {
            Fld_PTG_KDCFunkc_PTG = Fld_PTG_KDCTel_PTG = Fld_PTG_KDCEmail_PTG = "";
            return;
         }

         if(xtrans_rec != null && tb.Text != "")
         {
            Fld_PTG_KDCFunkc_PTG = xtrans_rec.T_kpdbZiroA_32;
            Fld_PTG_KDCTel_PTG   = xtrans_rec.T_kpdbUlBrA_32;
            Fld_PTG_KDCEmail_PTG = xtrans_rec.T_vezniDokA_64;
         }
         else
         {
            Fld_PTG_KDCFunkc_PTG = Fld_PTG_KDCTel_PTG = Fld_PTG_KDCEmail_PTG = "";
         }
      }
   }

   private void InitializeHamper_Napomena_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 6, "", null, false);

      hamper.VvColWdt       = new int[] { ZXC.Q10un, faBefCol, ZXC.Q10un};
      hamper.VvSpcBefCol    = new int[] { faBefCol , faBefCol, faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN- ZXC.Qun8 , ZXC.QUN + ZXC.Qun4, ZXC.QUN, ZXC.QUN- ZXC.Qun8, ZXC.QUN, ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] {          ZXC.Qun12, ZXC.Qun10,ZXC.Qun8,           ZXC.Qun10, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel  (0, 0, "Napomena:", ContentAlignment.MiddleLeft);
      tbx_Napomena_PTG = hamper.CreateVvTextBox(0, 1, "tbx_Napomena_PTG", "Napomena", GetDB_ColumnSize(DB_ci.opis), 2, 1);
      tbx_Napomena_PTG.Font = ZXC.vvFont.SmallFont;
      tbx_Napomena_PTG.Multiline = true;
      tbx_Napomena_PTG.ScrollBars = ScrollBars.Both;

                          hamper.CreateVvLabel  (0, 3, "Opaska Statusa Servisa:", ContentAlignment.MiddleLeft);
      tbx_opaskaServisa_PTG = hamper.CreateVvTextBox(0, 4, "tbx_opaskaServisa", "Opaska statusa servisa", GetDB_ColumnSize(DB_ci.napomena), 0, 1);
      tbx_opaskaServisa_PTG.Font = ZXC.vvFont.SmallFont;
      tbx_opaskaServisa_PTG.Multiline = true;
      tbx_opaskaServisa_PTG.ScrollBars = ScrollBars.Both;
    //tbx_opaskaServisa.JAM_ReadOnly = true;

                                 hamper.CreateVvLabel  (2, 3, "Napomena sa Partnera:"       , ContentAlignment.MiddleLeft); 
      tbx_R_napFromPartner_PTG = hamper.CreateVvTextBox(2, 4, "R_napFromPartne", "Napomena sa partnera", 246, 0, 1);
      tbx_R_napFromPartner_PTG.Font = ZXC.vvFont.SmallFont;
      tbx_R_napFromPartner_PTG.Multiline = true;
      tbx_R_napFromPartner_PTG.ScrollBars = ScrollBars.Both;
      tbx_R_napFromPartner_PTG.JAM_ReadOnly = true;

   }

   private void InitializeHamper_Skladista_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth - ZXC.Qun2, ZXC.Q3un - ZXC.Qun2, ZXC.Q5un, ZXC.Q2un - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { faBefFirstCol, faBefCol, faBefCol, faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel        (0, 0, "IzlSkl:", ContentAlignment.MiddleRight);
      tbx_SkladCd   = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_SkladCd", "Skladište u najam");
      tbx_SkladOpis = hamper.CreateVvTextBox      (2, 0, "tbx_SkladOpiS", "");
      tbx_SkladBR   = hamper.CreateVvTextBox      (3, 0, "tbx_SkladRbr", "");
      tbx_SkladCd.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_SkladCd.JAM_DataRequired = true;
      tbx_SkladCd.JAM_MustTabOutBeforeSubmit = true;

      tbx_SkladCd  .JAM_ReadOnly = true;
      tbx_SkladOpis.JAM_ReadOnly = true;
      tbx_SkladBR  .JAM_ReadOnly = true;

      tbx_SkladCd.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_SkladCd.JAM_lui_NameTaker_JAM_Name = tbx_SkladOpis.JAM_Name;
      tbx_SkladCd.JAM_lui_IntegerTaker_JAM_Name = tbx_SkladBR.JAM_Name;
      tbx_SkladCd.VVtag2 = true;

                      hamper.CreateVvLabel         (0, 1, "UlzSkl:", ContentAlignment.MiddleRight);
      tbx_Sklad2Cd  = hamper.CreateVvTextBoxLookUp (1, 1, "tbx_SkladCd", "Skladište povrata iy najma");
      tbx_Sklad2Opis = hamper.CreateVvTextBox      (2, 1, "tbx_SkladOpiS", "");
      tbx_SkladRbr2 = hamper.CreateVvTextBox       (3, 1, "tbx_SkladRbr", "");
      tbx_Sklad2Cd.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_Sklad2Cd.JAM_DataRequired = true;
      tbx_Sklad2Cd.JAM_MustTabOutBeforeSubmit = true;

      tbx_Sklad2Cd  .JAM_ReadOnly = true;
      tbx_Sklad2Opis.JAM_ReadOnly = true;
      tbx_SkladRbr2.JAM_ReadOnly = true;

      tbx_Sklad2Cd.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_Sklad2Cd.JAM_lui_NameTaker_JAM_Name = tbx_Sklad2Opis.JAM_Name;
      tbx_Sklad2Cd.JAM_lui_IntegerTaker_JAM_Name = tbx_SkladRbr2.JAM_Name;
      tbx_Sklad2Cd.VVtag2 = true;

   }

   private void InitializeHamper_R_Iznosi_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 3, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth + ZXC.Q2un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { faBefCol  , faBefCol};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                           hamper.CreateVvLabel   (0, 0, "IznosUgan NV:", ContentAlignment.MiddleRight);
      tbx_R_iznosUGAN_NV = hamper.CreateVvTextBox (1, 0, "tbx_R_iznosUGAN_NV", "Iznos UGAN-a nabavna vrijednost");
                           hamper.CreateVvLabel   (0, 1, "IznosUgan-a:", ContentAlignment.MiddleRight);
      tbx_R_iznosUGAN    = hamper.CreateVvTextBox (1, 1, "tbx_R_iznosUGAN", "Iznos UGAN-a");
                           hamper.CreateVvLabel   (0, 2, "Rata:", ContentAlignment.MiddleRight);
      tbx_R_iznosRate    = hamper.CreateVvTextBox (1, 2, "tbx_R_iznosRate", "Iznos jedne rate");

      tbx_R_iznosUGAN   .JAM_ReadOnly = true;
      tbx_R_iznosRate   .JAM_ReadOnly = true;
      tbx_R_iznosUGAN_NV.JAM_ReadOnly = true;
   }

   private void InitializeHamper_DodAndKopCount_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", null, false);
      
      hamper.VvColWdt     = new int[] { ZXC.Q2un - ZXC.Qun4, ZXC.QUN + ZXC.Qun12, ZXC.Q2un - ZXC.Qun4, ZXC.QUN + ZXC.Qun12 };
      hamper.VvSpcBefCol  = new int[] {                    0, ZXC.Qun12         ,                    0, ZXC.Qun12          };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4};
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel        (0, 0, "DOD:"          , ContentAlignment.MiddleRight);
      tbx_R_DodCount  = hamper.CreateVvTextBox      (1, 0, "tbx_R_DodCount", "Broj Dodataka ovog Ugovora", 2);
      tbx_R_DodCount.JAM_IsSupressTab = true;
      tbx_R_DodCount.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_R_DodCount.TextAlign = HorizontalAlignment.Right;
      tbx_R_DodCount.JAM_ReadOnly = true;
      tbx_R_DodCount.JAM_ForeColor = Color.DarkViolet; //Color.FromArgb(218, 195, 249); 
      tbx_R_DodCount.Font = ZXC.vvFont.BaseBoldFont;

                        hamper.CreateVvLabel        (2, 0, "KOP:"          , ContentAlignment.MiddleRight);
      tbx_R_KopCount  = hamper.CreateVvTextBox      (3, 0, "tbx_R_KopCount", "Broj Korekcija Otplatnog Plana", 2);
      tbx_R_KopCount.JAM_IsSupressTab = true;
      tbx_R_KopCount.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_R_KopCount.TextAlign = HorizontalAlignment.Right;
      tbx_R_KopCount.JAM_ReadOnly = true;
      tbx_R_KopCount.JAM_ForeColor = Color.Red;
    //tbx_R_KopCount.Font = ZXC.vvFont.BaseBoldFont;
   }

   private void InitializeHamper_TekstZaRacun_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", null, false);
      
      hamper.VvColWdt     = new int[] { ZXC.Q10un };
      hamper.VvSpcBefCol  = new int[] { ZXC.Qun8  };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel  (0, 0, "TekstZaRačun:", ContentAlignment.MiddleLeft);
      tbx_Napomena2 = hamper.CreateVvTextBox(0, 1, "tbx_napomena2", "Tekst koji se zeli prikazati na računu", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.napomena2), 0, 1);//PTG_TekstZaRn
      tbx_Napomena2.Multiline = true;
      tbx_Napomena2.ScrollBars = ScrollBars.Both;
   }

   private void InitializeHamper_PosredProviz_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", null, false);
      
      hamper.VvColWdt     = new int[] {ZXC.Q5un, ZXC.Q3un   };
      hamper.VvSpcBefCol  = new int[] {ZXC.Qun8, ZXC.Qun8  };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel  (0, 0, "PosredProviz%:", ContentAlignment.MiddleRight); 
      tbx_decimal02 = hamper.CreateVvTextBox(1, 0, "tbx_decimal02", "", GetDB_ColumnSize(DB_ci.decimal02));//PTG_PosredProvPosto 
      tbx_decimal02.JAM_IsForPercent = true;
      tbx_decimal02.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
   }

   private void InitializeHamper_KOPandDODcount(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", ThePolyGridTabControl.TabPages[0], false);
      
      hamper.VvColWdt     = new int[] { ZXC.Q8un, ZXC.QUN + ZXC.Qun12, ZXC.Q4un, ZXC.QUN + ZXC.Qun12 };
      hamper.VvSpcBefCol  = new int[] {        0, ZXC.Qun12          ,        0, ZXC.Qun12          };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel    (0, 0, "KOP:"          , ContentAlignment.MiddleRight);
      tbx_R_KopCount_2  = hamper.CreateVvTextBox(1, 0, "tbx_R_KopCount_2", "Broj Korekcija Otplatnog Plana", 2);
      tbx_R_KopCount_2.JAM_IsSupressTab = true;
      tbx_R_KopCount_2.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_R_KopCount_2.TextAlign = HorizontalAlignment.Right;
      tbx_R_KopCount_2.JAM_ReadOnly = true;
      tbx_R_KopCount_2.JAM_ForeColor = Color.Red;

                          hamper.CreateVvLabel  (2, 0, "DOD:"          , ContentAlignment.MiddleRight);
      tbx_R_DodCount_2  = hamper.CreateVvTextBox(3, 0, "tbx_R_DodCount_2", "Broj Dodataka ovog Ugovora", 2);
      tbx_R_DodCount_2.JAM_IsSupressTab = true;
      tbx_R_DodCount_2.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_R_DodCount_2.TextAlign = HorizontalAlignment.Right;
      tbx_R_DodCount_2.JAM_ReadOnly = true;
      tbx_R_DodCount_2.JAM_ForeColor = Color.DarkViolet; //Color.FromArgb(218, 195, 249); 
      tbx_R_DodCount_2.Font = ZXC.vvFont.BaseBoldFont;

   }

   public void InitializeHamper_StrukturaOtPlan_Rows(out VvHamper hamper)
   {
      //RedUK = OsnovRate (UgBrojRata + KorekRata) + UgPredRata + DodPredRata + OtkupRata

      hamper = new VvHamper(14, 1, "", ThePolyGridTabControl.TabPages[ptgOpl_TabPageName], false);
      //                                   0          1         2        3                4              5         6          7                 8             9               10              11       12       13       
      hamper.VvColWdt     = new int[] { ZXC.Q4un, ZXC.Q2un, ZXC.Q4un, ZXC.Q2un, ZXC.Q4un - ZXC.Qun4, ZXC.Q2un, ZXC.Q4un , ZXC.Q2un, ZXC.Q5un - ZXC.Qun8 , ZXC.Q2un, ZXC.Q5un - ZXC.Qun4, ZXC.Q2un, ZXC.Q4un, ZXC.Q2un };
      hamper.VvSpcBefCol  = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8,           ZXC.Qun12, ZXC.Qun8, ZXC.Qun12, ZXC.Qun8,            ZXC.Qun12, ZXC.Qun8,           ZXC.Qun12, ZXC.Qun8, ZXC.Qun12, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel( 0, 0, "RedUK:"       , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel( 2, 0, "=  OsnovRata:"   , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel( 4, 0, "( MjTrajUg:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel( 6, 0, " +  KorekRata:", ContentAlignment.MiddleRight);

      hamper.CreateVvLabel( 8, 0, ")  +  UgPredRata:" , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(10, 0, " +  DodPredRata:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(12, 0, " +  OtkupRata:"  , ContentAlignment.MiddleRight);



      tbx_NumOf_OtPlan_Rows_UK          = hamper.CreateVvTextBox( 1, 0, "tbx_NumOf_OtPlan_Rows_UK"         , "NumOf_OtPlan_Rows_UK         ", 3);
      tbx_NumOf_OtPlan_Rows_OsnovRate   = hamper.CreateVvTextBox( 3, 0, "tbx_NumOf_OtPlan_Rows_OsnovRate"  , "NumOf_OtPlan_Rows_OsnovRate  ", 3);
      tbx_NumOf_OtPlan_Rows_UgBrojRata  = hamper.CreateVvTextBox( 5, 0, "tbx_NumOf_OtPlan_Rows_UgBrojRata" , "NumOf_OtPlan_Rows_UgBrojRata ", 3);
      tbx_NumOf_OtPlan_Rows_KorekRata   = hamper.CreateVvTextBox( 7, 0, "tbx_NumOf_OtPlan_Rows_KorekRata"  , "NumOf_OtPlan_Rows_KorekRata  ", 3);
      tbx_NumOf_OtPlan_Rows_UgPredRata  = hamper.CreateVvTextBox( 9, 0, "tbx_NumOf_OtPlan_Rows_UgPredRata" , "NumOf_OtPlan_Rows_UgPredRata ", 3);
      tbx_NumOf_OtPlan_Rows_DodPredRata = hamper.CreateVvTextBox(11, 0, "tbx_NumOf_OtPlan_Rows_DodPredRata", "NumOf_OtPlan_Rows_DodPredRata", 3);
      tbx_NumOf_OtPlan_Rows_OtkupRata   = hamper.CreateVvTextBox(13, 0, "tbx_NumOf_OtPlan_Rows_OtkupRata"  , "NumOf_OtPlan_Rows_OtkupRata  ", 3);

      tbx_NumOf_OtPlan_Rows_UK         .JAM_IsSupressTab = true;
      tbx_NumOf_OtPlan_Rows_UgPredRata .JAM_IsSupressTab = true;
      tbx_NumOf_OtPlan_Rows_UgBrojRata .JAM_IsSupressTab = true;
      tbx_NumOf_OtPlan_Rows_DodPredRata.JAM_IsSupressTab = true;
      tbx_NumOf_OtPlan_Rows_KorekRata  .JAM_IsSupressTab = true;
      tbx_NumOf_OtPlan_Rows_OtkupRata  .JAM_IsSupressTab = true;
      tbx_NumOf_OtPlan_Rows_OsnovRate  .JAM_IsSupressTab = true;


      tbx_NumOf_OtPlan_Rows_UK         .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_NumOf_OtPlan_Rows_UgPredRata .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_NumOf_OtPlan_Rows_UgBrojRata .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_NumOf_OtPlan_Rows_DodPredRata.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_NumOf_OtPlan_Rows_KorekRata  .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_NumOf_OtPlan_Rows_OtkupRata  .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_NumOf_OtPlan_Rows_OsnovRate  .JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      tbx_NumOf_OtPlan_Rows_UK         .TextAlign = HorizontalAlignment.Right;
      tbx_NumOf_OtPlan_Rows_UgPredRata .TextAlign = HorizontalAlignment.Right;
      tbx_NumOf_OtPlan_Rows_UgBrojRata .TextAlign = HorizontalAlignment.Right;
      tbx_NumOf_OtPlan_Rows_DodPredRata.TextAlign = HorizontalAlignment.Right;
      tbx_NumOf_OtPlan_Rows_KorekRata  .TextAlign = HorizontalAlignment.Right;
      tbx_NumOf_OtPlan_Rows_OtkupRata  .TextAlign = HorizontalAlignment.Right;
      tbx_NumOf_OtPlan_Rows_OsnovRate  .TextAlign = HorizontalAlignment.Right;

      tbx_NumOf_OtPlan_Rows_UK         .JAM_ReadOnly = true;
      tbx_NumOf_OtPlan_Rows_UgPredRata .JAM_ReadOnly = true;
      tbx_NumOf_OtPlan_Rows_UgBrojRata .JAM_ReadOnly = true;
      tbx_NumOf_OtPlan_Rows_DodPredRata.JAM_ReadOnly = true;
      tbx_NumOf_OtPlan_Rows_KorekRata  .JAM_ReadOnly = true;
      tbx_NumOf_OtPlan_Rows_OtkupRata  .JAM_ReadOnly = true;
      tbx_NumOf_OtPlan_Rows_OsnovRate  .JAM_ReadOnly = true;

      //tbx_R_KopCount_2.JAM_ForeColor = Color.Red;
   }


   #endregion Hampers

   #region SumHampers

   public void CreateHamper_SumUGAN_PTG()
   {
      hamp_SumUGAN_PTG = new VvHamper(12, 1, "", ThePolyGridTabControl.TabPages[0], false);

      hamp_SumUGAN_PTG.VvColWdt      = new int[] { labelWidth, ZXC.Q4un, labelWidth, ZXC.Q4un, labelWidth, ZXC.Q4un, labelWidth, ZXC.Q4un, labelWidth, ZXC.Q4un, labelWidth+ ZXC.Qun2, ZXC.Q4un };
      hamp_SumUGAN_PTG.VvSpcBefCol   = new int[] { faBefCol  , faBefCol, faBefCol  , faBefCol, faBefCol  , faBefCol, faBefCol  , faBefCol, faBefCol  , faBefCol, faBefCol  , faBefCol };
      hamp_SumUGAN_PTG.VvRightMargin = hamp_twin_pix.VvLeftMargin;

      hamp_SumUGAN_PTG.VvRowHgt       = new int[] { ZXC.QUN };
      hamp_SumUGAN_PTG.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamp_SumUGAN_PTG.VvBottomMargin = hamp_twin_pix.VvTopMargin;

      hamp_SumUGAN_PTG.CreateVvLabel( 0, 0, "OrigNV:"   , ContentAlignment.MiddleRight);
      hamp_SumUGAN_PTG.CreateVvLabel( 2, 0, "OrigIznos:", ContentAlignment.MiddleRight);
      hamp_SumUGAN_PTG.CreateVvLabel( 4, 0, "OrigRata:" , ContentAlignment.MiddleRight);
      hamp_SumUGAN_PTG.CreateVvLabel( 6, 0, "Iznos NV:" , ContentAlignment.MiddleRight);
      hamp_SumUGAN_PTG.CreateVvLabel( 8, 0, "UkIznos:"  , ContentAlignment.MiddleRight);
      hamp_SumUGAN_PTG.CreateVvLabel(10, 0, "Rata:"     , ContentAlignment.MiddleRight);

      tbx_OrigNV    = hamp_SumUGAN_PTG.CreateVvTextBox( 1, 0, "tbx_OrigNV"   , "", 12);
      tbx_OrigIznos = hamp_SumUGAN_PTG.CreateVvTextBox( 3, 0, "tbx_OrigIznos", "", 12);
      tbx_OrigRata  = hamp_SumUGAN_PTG.CreateVvTextBox( 5, 0, "tbx_OrigRata" , "", 12);
      tbx_IznosNV   = hamp_SumUGAN_PTG.CreateVvTextBox( 7, 0, "tbx_IznosNV" , "", 12);
      tbx_UkIznos   = hamp_SumUGAN_PTG.CreateVvTextBox( 9, 0, "tbx_UkIznos"  , "", 12);
      tbx_Rata      = hamp_SumUGAN_PTG.CreateVvTextBox(11, 0, "tbx_Rata"     , "", 12);

      tbx_OrigNV   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_OrigIznos.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_OrigRata .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_IznosNV  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_UkIznos  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_Rata     .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      tbx_OrigNV   .JAM_ReadOnly = 
      tbx_OrigIznos.JAM_ReadOnly = 
      tbx_OrigRata .JAM_ReadOnly = 
      tbx_IznosNV  .JAM_ReadOnly = 
      tbx_UkIznos  .JAM_ReadOnly = 
      tbx_Rata     .JAM_ReadOnly = true;

      hamp_SumUGAN_PTG.Visible = false;
   }

   #endregion SumHampers

   #region Fld

   public string Fld_PTG_DanFakturiranjaString { get { return tbx_DanFakturiranja.Text; } set { tbx_DanFakturiranja    .Text = value; } }
   public string Fld_PTG_DanFakturiranjaOpis   {                                          set { tbx_DanFakturiranjaOpis.Text = value; } }
   public ZXC.PTG_DanFakturiranjaEnum Fld_PTG_DanFakturiranja//opciAvalue
   {
      get
      {
         if(tbx_DanFakturiranja.Text.IsEmpty()) return ZXC.PTG_DanFakturiranjaEnum.PrviDanMjeseca;
         else                              return (ZXC.PTG_DanFakturiranjaEnum)Enum.Parse(typeof(ZXC.PTG_DanFakturiranjaEnum), Fld_PTG_DanFakturiranjaString, true);
      }
   }

   public string Fld_PTG_VrstaNajma     { get { return tbx_vrstaNajma.Text; }  set { tbx_vrstaNajma    .Text = value; } }
   public string Fld_PTG_VrstaNajmaOpis {                                      set { tbx_vrstaNajmaOpis.Text = value; } }
   //public ZXC.PTG_VrstaNajmaEnum Fld_PTG_VrstaNajmaAsEnum //opciBlabel
   //{
   //   get
   //   {
   //      if(tbx_vrstaNajma.Text.IsEmpty()) return ZXC.PTG_VrstaNajmaEnum.Mjesecni;
   //      else return (ZXC.PTG_VrstaNajmaEnum)Enum.Parse(typeof(ZXC.PTG_VrstaNajmaEnum), tbx_vrstaNajma.Text, true);
   //   }
   //}

   public string Fld_PTG_NajamNaRok     { get { return tbx_NajamNaRok.Text; } set { tbx_NajamNaRok    .Text = value; } }
   public string Fld_PTG_NajamNaRokOpis {                                     set { tbx_NajamNaRokOpis.Text = value; } }
   //public ZXC.PTG_NajamNaRokEnum Fld_PTG_NajamNaRokAsEnum //opciBvalue 
   //{
   //   get
   //   {
   //      if(tbx_NajamNaRok.Text.IsEmpty()) return ZXC.PTG_NajamNaRokEnum.EMPTY;
   //      else                              return (ZXC.PTG_NajamNaRokEnum)Enum.Parse(typeof(ZXC.PTG_NajamNaRokEnum), tbx_NajamNaRok.Text, true);
   //   }
   //}

   public ZXC.PTG_MjestoIsporukeEnum Fld_PTG_MjestoIsporuke //_pdvZPkind
   {
      get
      {
              if(rbt_mjIsp_Korisnik  .Checked) return ZXC.PTG_MjestoIsporukeEnum.Korisnik        ;
         else if(rbt_mjIsp_KorisOvers.Checked) return ZXC.PTG_MjestoIsporukeEnum.KorisnikOverseas;
         else if(rbt_mjIsp_PcToGo    .Checked) return ZXC.PTG_MjestoIsporukeEnum.PcToGo          ;

         else throw new Exception("Fld_PTG_MjestoIsporuke: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.PTG_MjestoIsporukeEnum.Korisnik        : rbt_mjIsp_Korisnik  .Checked = true; break;
            case ZXC.PTG_MjestoIsporukeEnum.KorisnikOverseas: rbt_mjIsp_KorisOvers.Checked = true; break;
            case ZXC.PTG_MjestoIsporukeEnum.PcToGo          : rbt_mjIsp_PcToGo    .Checked = true; break;
         }
      }
   }

 //public DateTime Fld_PTG_DatUgovora     { get { return this.currentData._dokDate; } set { this.currentData._dokDate = value; } }
   public DateTime Fld_PTG_DatDostave     { get { return dtp_DokDate2  .Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_DokDate2  .Value = value; } }      }
   public DateTime Fld_PTG_DatSkidSaSklad { get { return dtp_SkladDate .Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_SkladDate .Value = value; } } }
   public DateTime Fld_PTG_DatPrvogRn     { get { return dtp_DospDate  .Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_DospDate  .Value = value; } } }
   public DateTime Fld_PTG_DatZadnjegRn   { get { return dtp_PonudDate .Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_PonudDate .Value = value; } } }
   public DateTime Fld_PTG_datIstekaUg    { get { return dtp_RokIspDate.Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_RokIspDate.Value = value; } } }
   public DateTime Fld_PTG_DatPovrataOpr  { get { return dtp_dateX     .Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_dateX     .Value = value; } } }

   public uint Fld_PTG_DodCount
   {
      //get { return tbx_PTG_DodNum.GetUintField(); }
      set { tbx_R_DodCount.PutUintField(value); }
   }

   public uint Fld_PTG_KopCount
   {
      //get { return tbx_PTG_DodNum.GetUintField(); }
      set { tbx_R_KopCount.PutUintField(value); }
   }

   public uint Fld_PTG_DodCount_2
   {
      //get { return tbx_PTG_DodNum.GetUintField(); }
      set { tbx_R_DodCount_2.PutUintField(value); }
   }

   public uint Fld_PTG_KopCount_2
   {
      //get { return tbx_PTG_DodNum.GetUintField(); }
      set { tbx_R_KopCount_2.PutUintField(value); }
   }


   public PTG_OtplatniPlan TheOtplatniPlan { get { return this.PtgUgovor_rec.TheOtplatniPlan; } set { this.PtgUgovor_rec.TheOtplatniPlan = value; } }

   public string Fld_PTG_Napomena   { get { return tbx_Napomena_PTG.Text; } set { tbx_Napomena_PTG.Text = value; } }
   public string Fld_PTG_OpaskaServisa   { get { return tbx_opaskaServisa_PTG.Text; } set { tbx_opaskaServisa_PTG.Text = value; } }

   public string Fld_NapFromPartner_PTG { set { tbx_R_napFromPartner_PTG.Text = value; } }
   public string Fld_PTG_KugPartner     { set { tbx_PTG_KugPartner      .Text = value; } }

   public int  Fld_PTG_KorekcijaRata   { get { return tbx_korekcijaRata_PTG.GetIntField();    } set { tbx_korekcijaRata_PTG.PutIntField(value)   ; } }//rokisporuke
   public uint Fld_PTG_trajanjeUgovora { get { return tbx_trajanjeUgovora_PTG.GetUintField(); } set { tbx_trajanjeUgovora_PTG.PutUintField(value); } }//cjenTTnum

   public int Fld_PTG_fakRate   { get { return ZXC.ValOrZero_Int(tbx_fakRate_PTG  .Text); } set { tbx_fakRate_PTG  .Text = value.ToString(); } }
   public int Fld_PTG_NeFakRate { get { return ZXC.ValOrZero_Int(tbx_neFakrate_PTG.Text); } set { tbx_neFakrate_PTG.Text = value.ToString(); } }
   
   public string Fld_PTG_OsigPlacanja     { get { return tbx_OsigPlacanja.Text; }  set { tbx_OsigPlacanja.Text = value; } }
   public string Fld_PTG_OsigPlacanjaOpis {                                        set { tbx_PTG_OsigPlOpis  .Text = value; } }

   public bool Fld_PTG_b1_povOprBezPen  { get { return cbxV_PTG_1_isPovOprBezPen .Checked; } set { cbxV_PTG_1_isPovOprBezPen .Checked = value; } }
   public bool Fld_PTG_b2_podnajam      { get { return cbxV_PTG_2_isPodnajam     .Checked; } set { cbxV_PTG_2_isPodnajam     .Checked = value; } }
   public bool Fld_PTG_b3_osigOpreme    { get { return cbxV_PTG_3_isOsigOpreme   .Checked; } set { cbxV_PTG_3_isOsigOpreme   .Checked = value; } }
   public bool Fld_PTG_b4_novaOprema    { get { return cbxV_PTG_4_isNovaOprema   .Checked; } set { cbxV_PTG_4_isNovaOprema   .Checked = value; } }
   public bool Fld_PTG_b5_potpisan      { get { return cbxV_PTG_5_isPotpisan     .Checked; } set { cbxV_PTG_5_isPotpisan     .Checked = value; } }
   public bool Fld_PTG_b6_vracenaOprema { get { return cbxV_PTG_6_isVracenaOprema.Checked; } set { cbxV_PTG_6_isVracenaOprema.Checked = value; } }
   public bool Fld_PTG_b7_otkup         { get { return cbxV_PTG_7_isOtkup        .Checked; } set { cbxV_PTG_7_isOtkup        .Checked = value; } }
   public bool Fld_PTG_b8_ispisnoRj     { get { return false /*cbxV_PTG_8_isIspisnoRj    .Checked*/       ; } /*set { cbxV_PTG_1_isPovOprBezPen.Checked = value; } */}
   public bool Fld_PTG_b9_isJednokratPl { get { return false /*cbxV_PTG_9pdvNum_isJednokratPl   .Checked*/; } /*set { cbxV_PTG_1_isPovOprBezPen.Checked = value; } */}

   public string Fld_PTG_b1_povOprBezPen_X  { set { tbx_PTG_1_isPovOprBezPen .Text = value; }  }
   public string Fld_PTG_b2_podnajam_X      { set { tbx_PTG_2_isPodnajam     .Text = value; }  }
   public string Fld_PTG_b3_osigOpreme_X    { set { tbx_PTG_3_isOsigOpreme   .Text = value; }  }
   public string Fld_PTG_b4_novaOprema_X    { set { tbx_PTG_4_isNovaOprema   .Text = value; }  }
   public string Fld_PTG_b5_potpisan_X      { set { tbx_PTG_5_isPotpisan     .Text = value; }  }
   public string Fld_PTG_b6_vracenaOprema_X { set { tbx_PTG_6_isVracenaOprema.Text = value; }  }
   public string Fld_PTG_b7_otkup_X         { set { tbx_PTG_7_isOtkup        .Text = value; }  }
   //public string Fld_PTG_b8_ispisnoRj_X     { set { tbx_PTG_8_isIspisnoRj.Text = value; }  }
   //public string Fld_PTG_b9_isJednokratPl_X { set { tbx_isIzuzet.Text = value; }  }

   public string Fld_PTG_KDCnaziv     { get { return tbx_KDCnaziv_PTG.Text; } set { tbx_KDCnaziv_PTG.Text = value; } }
   public string Fld_PTG_KDCFunkc_PTG {                                       set { tbx_KDCFunkc_PTG.Text = value; } }
   public string Fld_PTG_KDCTel_PTG   {                                       set { tbx_KDCTel_PTG  .Text = value; } }
   public string Fld_PTG_KDCEmail_PTG {                                       set { tbx_KDCEmail_PTG.Text = value; } }

   public int Fld_NumOf_OtPlan_Rows_UK          { set { tbx_NumOf_OtPlan_Rows_UK         .Text = value.ToString(); } }  
   public int Fld_NumOf_OtPlan_Rows_UgPredRata  { set { tbx_NumOf_OtPlan_Rows_UgPredRata .Text = value.ToString(); } }
   public int Fld_NumOf_OtPlan_Rows_UgBrojRata  { set { tbx_NumOf_OtPlan_Rows_UgBrojRata .Text = value.ToString(); } }
   public int Fld_NumOf_OtPlan_Rows_DodPredRata { set { tbx_NumOf_OtPlan_Rows_DodPredRata.Text = value.ToString(); } }
   public int Fld_NumOf_OtPlan_Rows_KorekRata   { set { tbx_NumOf_OtPlan_Rows_KorekRata  .Text = value.ToString(); } }
   public int Fld_NumOf_OtPlan_Rows_OtkupRata   { set { tbx_NumOf_OtPlan_Rows_OtkupRata  .Text = value.ToString(); } }
   public int Fld_NumOf_OtPlan_Rows_OsnovRate   { set { tbx_NumOf_OtPlan_Rows_OsnovRate  .Text = value.ToString(); } }

   #endregion Fld

   #region TheG_Specific_Columns

   public override bool HasRtrans_SkladCD_Exposed { get { return true; } }
   public override bool HasRtrano_SkladCD_Exposed { get { return true; } }
   protected override void InitializeDUC_Specific_Columns()
   {
      T_artiklCD_CreateColumn      (ZXC.Q3un           ,    true, "Šifra"   , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                        true, "Naziv"   , "Naziv artikla ili proizvoljan opis");
      T_artiklTS_CreateColumn      (ZXC.Q2un,               true, "Tip"     , "Tip artikla");
      T_doCijMal_CreateColumn      (ZXC.Q3un, 0,            true, "RAM"     , "RAM", false);
      T_noCijMal_CreateColumn      (ZXC.Q3un, 0,            true, "HDD"     , "HDD");
      T_skladCD_CreateColumn       (ZXC.Q3un,               true, "Sklad"   , "Izlazno skladište");
      T_jedMj_CreateColumn         (ZXC.Q2un           ,    true, "JM"      , "Jedinica mjere"                    );
      T_kol_CreateColumn           (ZXC.Q3un           , 2, true, "Kol"     , "Količina"                );
      T_cij_CreateColumn           (ZXC.Q4un           , 2, true, "Cijena"  , "Jedinična cijena"                  );
      T_rbt1St_CreateColumn        (ZXC.Q3un - ZXC.Qun4, 2, true, "Rbt"     , "Stopa rabata");
      R_cij_kcr_CreateColumn       (ZXC.Q4un           , 2, true, "CijSRbt" , "Cijena s rabatom");
      R_rbt1_CreateColumn          (ZXC.Q4un           , 2, true, "IznosRbt", "Iznos rabata");
      R_KCR_CreateColumn           (ZXC.Q4un,            2, true, "Iznos"   , "Ukupan iznos bez PDV-a");
      R_NC_CreateColumn            (ZXC.Q4un           , 2, true, "NabCij"  , "Nabavna cijena");
      R_NV_CreateColumn            (ZXC.Q4un           , 2, true, "NabVri"  , "Nabavna vrijednost");
      R_RUC_CreateColumn           (ZXC.Q4un           , 2, true, "RUC"     , "RUC - razlika u cijeni");
      R_RUV_CreateColumn           (ZXC.Q4un           , 2, true, "RUV"     , "RUV - razlika u vrijednosti");
   }

   #endregion TheG_Specific_Columns

   #region TheG_Specific_Columns2

   protected override void InitializeDUC_Specific_Columns2()
   {
      bool isVisible = true;

      T_artiklCD2_CreateColumn      (ZXC.Q5un,    isVisible, "Šifra"        , "Šifra artikla"                     );
      T_artiklName2_CreateColumnFill(             isVisible, "Naziv"        , "Naziv artikla ili proizvoljan opis");
      T_paletaNo_CreateColumn       (ZXC.Q3un,    isVisible, "Stavka"       , "UGANDO stavka"                     );
      T_skladCD2_CreateColumn       (ZXC.Q3un,    isVisible, "Sklad"        , "Izlazno skladište"                 );
      T_serno_CreateColumn          (ZXC.Q8un,    isVisible, "Serijski broj", "Serijski broj artikla"             );
   }

   #endregion TheG_Specific_Columns2

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_UGAN_PTG, clr_UPA, clr_UGAN_PTG);
      ThePolyGridTabControl.TabPages[ptgOpl_TabPageName].Tag = clr_OPL_PTG;
      ThePolyGridTabControl.TabPages[ptgDod_TabPageName].Tag = clr_DOD_PTG;
   }
   #endregion Colors

   #region TabPageTitle1

   public override string TabPageTitle1
   {
      get { return ptgOsn_TabPageName; }
   }

   #endregion TabPageTitle1

   #region Update_KdcXtrans

   public static string Update_KdcXtrans(object startValue, object sender)
   {
      Xtrans         xtrans_rec = new Xtrans();
      KDC_ListUC     kdcXtransListUC;
      XSqlConnection dbConnection = ZXC.TheVvForm.TheDbConnection;

      VvFindDialog dlg = CreateFind_Xtrans_Dialog();

      kdcXtransListUC = (KDC_ListUC)(dlg.TheRecListUC);

      kdcXtransListUC.Fld_FromNaziv = startValue.ToString();

      VvTextBox vvTextBox_kupdobCD = ((sender as VvTextBox).VVtag2 as VvTextBox);
      kdcXtransListUC.Fld_TT    = Mixer.TT_KDC;
      kdcXtransListUC.Fld_TtNum = vvTextBox_kupdobCD.GetSomeRecIDField();

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         if(!ZXC.XtransDao.SetMe_Record_byRecID(dbConnection, xtrans_rec, (uint)dlg.SelectedRecID, false/*(uint)dlg.SelectedRecIDIn_FIRST_TransGrid*/)) return null;
      }
      else
      {
         xtrans_rec = null;
      }

      if(dlg.SelectionIsNewlyAddedRecord == true) ZXC.ShouldForceSifrarRefreshing = true;

      dlg.Dispose();

      if(xtrans_rec != null) return xtrans_rec.T_kpdbNameA_50;
      else                   return null;
   }

   #endregion Update_KdcXtrans
   
   #region VvFindDialog - CreateFind_Xtrans_Dialog

   //public override VvFindDialog CreateVvFindDialog() // override je za i na Faktur-u 
   //{
   //   return CreateFind_Xtrans_Dialog();
   //}

   public static VvFindDialog CreateFind_Xtrans_Dialog()
   {
      VvForm.VvSubModul vvSubModul   = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsKDC);
      VvDataRecord      vvDataRecord = new Xtrans();

      VvFindDialog vvFindDialog = new VvFindDialog();
      VvRecLstUC   vvRecListUC  = new KDC_ListUC(vvFindDialog, (Xtrans)vvDataRecord, vvSubModul);

      vvFindDialog.TheRecListUC = vvRecListUC;
      
      return vvFindDialog;
   }

   #endregion VvFindDialog - CreateFind_Xtrans_Dialog

   public override void OpenCloseForWriting_AdditionalAction_UCspecific(ZXC.WriteMode writeMode, bool isESC)
   {
      bool idemoUzuto   = writeMode != ZXC.WriteMode.None;
      bool idemoUbijelo = !idemoUzuto                    ;

      bool isRtranO_zuto = idemoUzuto && ZXC.RISK_Edit_RtranoOnly_InProgress;
      bool isRtranS_zuto = idemoUzuto && isRtranO_zuto == false;

      int rtranStabIdx = 0;
      int rtranOtabIdx = 1;

      if(isRtranS_zuto) ThePolyGridTabControl.SelectedIndex = rtranStabIdx;
      if(isRtranO_zuto) ThePolyGridTabControl.SelectedIndex = rtranOtabIdx;

      for(int i = 0; i < ThePolyGridTabControl.TabPages.Count; ++i)
      {
         if(idemoUbijelo) ThePolyGridTabControl.TabPages[i].Enabled = true;
         else // idemoUzuto 
         {
                 if(i == rtranStabIdx && isRtranS_zuto) ThePolyGridTabControl.TabPages[i].Enabled = true ;
            else if(i == rtranOtabIdx && isRtranO_zuto) ThePolyGridTabControl.TabPages[i].Enabled = true ;
            else                                        ThePolyGridTabControl.TabPages[i].Enabled = false;
         }
      }

      if(isRtranO_zuto)
      {
         foreach(VvHamper hamper in hamperLeft)
         {
            if(hamper.IsDUMMY) continue;
            VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvRecordUC);
         }

         VvHamper.Open_Close_Fields_ForWriting(tbx_opaskaServisa_PTG, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvRecordUC);
      }
      else
      {
         VvHamper.Open_Close_Fields_ForWriting(tbx_opaskaServisa_PTG, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvRecordUC);
      }

   } // public override void OpenCloseForWriting_AdditionalAction_UCspecific(ZXC.WriteMode writeMode, bool isESC) 
}

// staru nomenklaturu "UGO" (nova je UGN) smo morali ostaviti kod naziva DUC-a jer je vec otislo u vvusercontrol 
public class UGO_PTG_DUC : UGNorAUN_PTG_DUC
{
   #region Constructor (empty, only for calling base.Constructor)

   public UGO_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
      (Faktur.tt_colName, new string[]
      {
         Faktur.TT_UGN
      });
   }

   #endregion Constructor
}

// staru nomenklaturu "ANU" (nova je AUN) smo morali ostaviti kod naziva DUC-a jer je vec otislo u vvusercontrol 
public class ANU_PTG_DUC : UGNorAUN_PTG_DUC
{
   #region Constructor

   public ANU_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
      (Faktur.tt_colName, new string[]
      {
         Faktur.TT_AUN
      });
   }

   #endregion Constructor
}

public class A1_ANU_PTG_DUC : UGNorAUN_PTG_DUC
{
   #region Constructor

   public A1_ANU_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
      (Faktur.tt_colName, new string[]
      {
         Faktur.TT_AUN
      });
   }

   #endregion Constructor
}

public class DOD_PTG_DUC : FakturPDUC //FakturExtDUC
{
   #region Constructor

   public DOD_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[]
         {
            Faktur.TT_DOD
         });
   }

   #endregion Constructor

   #region Fieldz

   public VvHamper hamp_partner_PTG, hamp_TT_PTG, hamp_v1TT_PTG, hamp_v2TT_PTG, hamp_KUGpartner_PTG, hamp_DodNum_PTG,
                   hamp_Dodatak_PTG, hamp_napomena_PTG, hamp_sklad_PTG;
   public RadioButton rbt_mjIsp_Korisnik, rbt_mjIsp_PcToGo, rbt_mjIsp_KorisOvers;

   #endregion Fieldz
   
   #region Hampers

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      InitializeHamper_Partner_PTG   (out hamp_partner_PTG);
      InitializeHamper_TT_PTG        (out hamp_TT_PTG        );
      InitializeHamper_v1TT_PTG      (out hamp_v1TT_PTG      );
      InitializeHamper_v2TT_PTG      (out hamp_v2TT_PTG      );
      InitializeHamper_DODnum_PTG    (out hamp_DodNum_PTG    );
      InitializeHamper_KUGpartner_PTG(out hamp_KUGpartner_PTG);
      InitializeHamper_Dodatak_PTG   (out hamp_Dodatak_PTG   );
      InitializeHamper_Napomena_PTG  (out hamp_napomena_PTG  );
      InitializeHamper_Skladista_PTG (out hamp_sklad_PTG     );
      CreateArrOfHampers();

      SetParentOfHamperLeftHampers();

      panel_MigratorsLeftB.SendToBack();

      hamp_partner_PTG.Location = new Point(0, 0);
      hamp_TT_PTG     .Location = new Point(hamp_partner_PTG.Right, 0);

      int hamp_PTG_DodNumLeft = hamp_TT_PTG.Right - hamp_DodNum_PTG.Width;
      int hamp_v2TT_PTGLeft   = hamp_TT_PTG.Right - hamp_DodNum_PTG.Width - hamp_v2TT_PTG.Width;
      int hamp_v1TT_PTGLeft   = hamp_TT_PTG.Right - hamp_DodNum_PTG.Width - hamp_v2TT_PTG.Width - hamp_v1TT_PTG.Width;

      hamp_v2TT_PTG.Location   = new Point(hamp_v2TT_PTGLeft  , hamp_TT_PTG.Bottom);
      hamp_v1TT_PTG.Location   = new Point(hamp_v1TT_PTGLeft  , hamp_TT_PTG.Bottom);
      hamp_DodNum_PTG.Location = new Point(hamp_PTG_DodNumLeft, hamp_TT_PTG.Bottom);

      hamp_Dodatak_PTG.Location = new Point(hamp_TT_PTG.Left, hamp_v1TT_PTG.Bottom);

      hamp_dokNum .Location = new Point(hamp_TT_PTG.Right, hamp_TT_PTG.Top );

      hamp_KUGpartner_PTG.Location = new Point(hamp_TT_PTG.Left, hamp_TT_PTG.Bottom);

      hamp_napomena_PTG.Location = new Point(hamp_partner_PTG.Left, hamp_partner_PTG.Bottom);

    //hamp_KUGpartner_PTG.Visible  = (this.Fld_V1_ttNum.NotZero()) ? true : false;
      hamp_sklad_PTG.Location = new Point(hamp_Dodatak_PTG.Left + ZXC.Q10un + ZXC.Qun2, hamp_Dodatak_PTG.Bottom);

      nextY = hamp_napomena_PTG.Bottom;
   }

   private void CreateArrOfHampers()
   {

      hamperLeft = new VvHamper[] { hamp_TT_PTG, hamp_partner_PTG, hamp_dokDate, hamp_dokNum,
                                    hamp_v1TT_PTG, hamp_v2TT_PTG, hamp_DodNum_PTG, hamp_KUGpartner_PTG,
                                    hamp_Dodatak_PTG, hamp_napomena_PTG, hamp_sklad_PTG
                                  };
   }

   private void InitializeHamper_TT_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth + ZXC.QUN, ZXC.Q3un - ZXC.Qun2, ZXC.Q10un, ZXC.Q6un - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { faBefCol            ,            faBefCol, faBefCol,            faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN + ZXC.Qun8 };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
      hamper.VvBottomMargin = ZXC.Qun8;

               hamper.CreateVvLabel        (0, 0, "Tip:", ContentAlignment.MiddleRight);
      tbx_TT = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_tt", "Tip transakcije - racuna");
      // Nota bene; buduci je tbx_tt VvLookUp TextBox onde mu je tbx_tt.JAM_FieldExitMethod 'potrosena' lookUp methodom. 
      //tbx_tt.JAM_FieldExitMethod_2 = new EventHandler(OnExit_ValidateTTrestrictor);

      tbx_TT.JAM_FieldExitWithValidationMethod += new System.ComponentModel.CancelEventHandler(OnExit_ValidateTTrestrictor);

      tbx_TtOpis = hamper.CreateVvTextBox(2, 0, "tbx_ttOpis", "");
      tbx_TtNum  = hamper.CreateVvTextBox(3, 0, "tbx_ttNum", "Ovo bolje ostavi kako je...", 7); // !!! 
      tbx_TT.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_TT.JAM_DataRequired = true;
      tbx_TT.JAM_MustTabOutBeforeSubmit = true;

      tbx_TT    .JAM_ReadOnly = true;
      tbx_TtNum .JAM_ReadOnly = true;
      tbx_TtOpis.JAM_ReadOnly = true;

      tbx_TtNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_TT.JAM_Set_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_TT.JAM_ReadOnly = true;
      tbx_TT.JAM_lui_NameTaker_JAM_Name = tbx_TtOpis.JAM_Name;
      tbx_TT.JAM_ttNumTaker_JAM_Name = tbx_TtNum.JAM_Name;

      tbx_TtNum.JAM_IsSupressTab = true;

      tbx_TT.Font     = ZXC.vvFont.BaseBoldFont;
      tbx_TtOpis.Font = ZXC.vvFont.BaseBoldFont;
      tbx_TtNum.Font  = ZXC.vvFont.LargeBoldFont;

      tbx_TtNum.TextAlign = HorizontalAlignment.Right;

   }

   private void InitializeHamper_v1TT_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", null, false);
      
      hamper.VvColWdt     = new int[] { ZXC.QUN + ZXC.Qun12 };
      hamper.VvSpcBefCol  = new int[] { ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

//                      hamper.CreateVvLabel        (0, 0, "KUG:"       , ContentAlignment.MiddleRight);
      tbx_v1_ttNum  = hamper.CreateVvTextBox      (0, 0, "tbx_v1_ttNum", "Broj Krovnog Ugovora", 2/*GetDB_ColumnSize(DB_ci.iz_ttNum)*/);
      tbx_v1_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_v1_ttNum.JAM_IsSupressTab = true;
      tbx_v1_ttNum.JAM_MarkAsNumericTextBox(0, false, 1, 40, false);
      tbx_v1_ttNum.TextAlign = HorizontalAlignment.Right;
      tbx_v1_ttNum.JAM_ReadOnly = true;

      //tbx_PTG_KugPartner = hamper.CreateVvTextBox(2, 0, "tbx_PTG_KugPartner", "Partner krovnog ugovora");
      
   }

   private void InitializeHamper_v2TT_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 2, "", null, false);

      hamper.VvColWdt      = new int[] { ZXC.Q3un - ZXC.Qun2};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN -ZXC.Qun12 };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun12         };
      hamper.VvBottomMargin = hamper.VvTopMargin;

     // string lblText = (this is UGO_PTG_DUC) ? "UGN:" : "AUN:";
     //
     //                hamper.CreateVvLabel  (0, 0, lblText      , ContentAlignment.MiddleRight);
      tbx_v2_ttNum = hamper.CreateVvTextBox(0, 0, "tbx_v2_ttNum", "Broj Ugovora/Aneksa", 5/*GetDB_ColumnSize(DB_ci.na_ttNum)*/);
      tbx_v2_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_v2_ttNum.JAM_IsSupressTab = true;
      tbx_v2_ttNum.JAM_ReadOnly     = true;

      tbx_v2_ttNum.Font      = ZXC.vvFont.BaseBoldFont;
      tbx_v2_ttNum.TextAlign = HorizontalAlignment.Right;

      btn_v2TT = hamper.CreateVvButton(0, 1, new EventHandler(GoTo_UGAN_Dokument_Click), "");
      btn_v2TT.Name = "v2_TT";
      btn_v2TT.FlatStyle = FlatStyle.Flat;
      btn_v2TT.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_v2TT.Image = VvIco.TriangleBlue16.ToBitmap();
      btn_v2TT.Tag = 1;
      btn_v2TT.TabStop = false;
   }

   private void InitializeHamper_DODnum_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", null, false);

      hamper.VvColWdt      = new int[] { ZXC.Q2un - ZXC.Qun8};
      hamper.VvSpcBefCol   = new int[] { faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

   //                   hamper.CreateVvLabel(0, 0, "DOD:", ContentAlignment.MiddleRight);
      tbx_PTG_DodNum = hamper.CreateVvTextBox(0, 0, "tbx_PTG_DodNum", "Dodatak broj", 3);

      tbx_PTG_DodNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_PTG_DodNum.JAM_IsSupressTab = true;
      tbx_PTG_DodNum.TextAlign = HorizontalAlignment.Right;
      tbx_PTG_DodNum.JAM_ReadOnly = true;
   }

   private void InitializeHamper_KUGpartner_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", null, false);
      
      hamper.VvColWdt     = new int[] { labelWidth + ZXC.QUN, ZXC.Q9un + ZXC.Q3un + ZXC.Qun2 + faBefCol};
      hamper.VvSpcBefCol  = new int[] { faBefCol, faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      tbx_PTG_KugPartner = hamper.CreateVvTextBox(1, 0, "tbx_PTG_KugPartner", "Partner Krovnog Ugovora");
      tbx_PTG_KugPartner.JAM_IsSupressTab = true;
      tbx_PTG_KugPartner.JAM_ReadOnly = true;

   }

   private void InitializeHamper_Partner_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 4, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth + ZXC.QUN, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un - ZXC.Qun4, ZXC.Q10un, ZXC.Q4un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { faBefFirstCol, faBefCol, faBefCol, faBefCol, faBefCol, faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN + ZXC.Qun8, ZXC.QUN , ZXC.QUN , ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] {           ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel    (0, 0, "Partner:", ContentAlignment.MiddleRight);
      tbx_KupdobCd   = hamper.CreateVvTextBox  (1, 0, "tbx_kupdobCd"  , "Sifra partnera" , 6);
      tbx_KupdobTk   = hamper.CreateVvTextBox  (2, 0, "tbx_kupdobTk"  , "Ticker partnera", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.kupdobTK));
      tbx_KupdobName = hamper.CreateVvTextBox(3, 0, "tbx_kupdobName", "Naziv partnera" , GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.kupdobName), 2, 0);
      tbx_KupdobCd.JAM_MustTabOutBeforeSubmit =
      tbx_KupdobTk.JAM_MustTabOutBeforeSubmit =
      tbx_KupdobName.JAM_MustTabOutBeforeSubmit = true;
      tbx_KupdobCd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_KupdobTk.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_KupdobName.JAM_Highlighted = true;

      tbx_KupdobCd  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyKupdobTextBoxLeave));
      tbx_KupdobTk  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_KupdobName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyKupdobTextBoxLeave));

      tbx_KupdobCd.DoubleClick += new EventHandler(AnyKupdobTextBox_DoubleClick);
      tbx_KupdobTk.DoubleClick += new EventHandler(AnyKupdobTextBox_DoubleClick);
      tbx_KupdobName.DoubleClick += new EventHandler(AnyKupdobTextBox_DoubleClick);
      tbx_KupdobName.Font = ZXC.vvFont.LargeBoldFont;

      tbx_KupdobUlica = hamper.CreateVvTextBox (1, 0, "tbx_KupdobUlica ", "KupdobUlica ", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.kdUlica));
      tbx_KupdobZip   = hamper.CreateVvTextBox (1, 0, "tbx_KupdobZip   ", "KupdobZip   ", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.kdZip));
      tbx_KupdobMjesto = hamper.CreateVvTextBox(1, 0, "tbx_KupdobMjesto", "KupdobMjesto", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.kdMjesto));
      tbx_KupdobUlica .Visible =
      tbx_KupdobZip   .Visible =
      tbx_KupdobMjesto.Visible = false;


                     hamper.CreateVvLabel  (0, 1, "AdrFaktur:", ContentAlignment.MiddleRight);
      tbx_KdAdresa = hamper.CreateVvTextBox(1, 1, "kdAdresa", "Adresa partnera", 74, 2, 0);

                  hamper.CreateVvLabel  (4, 1, "OIB:", ContentAlignment.MiddleRight);
      tbx_KdOib = hamper.CreateVvTextBox(5, 1, "kdOib", "Oib", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.kdOib));

      tbx_KdOib.JAM_IsSupressTab = tbx_KdAdresa.JAM_IsSupressTab = true;
      tbx_KdOib.JAM_ReadOnly     = tbx_KdAdresa.JAM_ReadOnly     = true;

      // !!!! dopuni PutAllKupdobFields ZA napomenu sa Kupdoba

                         hamper.CreateVvLabel  (0, 2, "PoslJed:", ContentAlignment.MiddleRight);
      tbx_PosJedCd     = hamper.CreateVvTextBox(1, 2, "tbx_posJedCd", "Sifra poslovne jedinice", 6);
      tbx_PosJedTk     = hamper.CreateVvTextBox(2, 2, "tbx_posJedTk", "Ticker poslovne jedinice", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.posJedTK));
      tbx_PosJedName   = hamper.CreateVvTextBox(3, 2, "tbx_posJedName", "Naziv oslovne jedinice", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.posJedName));
                         hamper.CreateVvLabel  (0, 3, "AdresaPJ:", ContentAlignment.MiddleRight);
      tbx_PosJedAdresa = hamper.CreateVvTextBox(1, 3, "tbx_posJedUlica", "Ulica partnera - poslovne jedinice", 74, 2, 0);

      tbx_PosJedCd    .JAM_ReadOnly =
      tbx_PosJedTk    .JAM_ReadOnly =
      tbx_PosJedName  .JAM_ReadOnly =
      tbx_PosJedAdresa.JAM_ReadOnly = true;

      //                  hamper.CreateVvLabel  (4, 3, "Rabat:", ContentAlignment.MiddleRight);
      //tbx_somePercent = hamper.CreateVvTextBox(5, 3, "tbx_somePercent", "", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.somePercent));//PTG_RabatKupcaPosto 
      //tbx_somePercent.JAM_IsForPercent = true;
      //tbx_somePercent.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);

      //              hamper.CreateVvLabel  (4, 4, "OdgodaPlać:", ContentAlignment.MiddleRight);
      //tbx_RokPlac = hamper.CreateVvTextBox(5, 4, "tbx_RokPlac", "Odgoda plaćanja", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.rokPlac));//PTG_OdgodaPl
      //tbx_RokPlac.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_KupdobCd   .JAM_ReadOnly = 
      tbx_KupdobTk   .JAM_ReadOnly = 
      tbx_KupdobName .JAM_ReadOnly = 
      tbx_somePercent.JAM_ReadOnly = 
      tbx_RokPlac    .JAM_ReadOnly = true;

   }

   private void InitializeHamper_Dodatak_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 3, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth + ZXC.QUN, ZXC.Q4un + ZXC.Qun4, ZXC.Q6un, ZXC.Q4un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { faBefCol  ,            faBefCol,           faBefCol, faBefCol, faBefCol};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
      hamper.VvBottomMargin = ZXC.Qun8;

                    hamper.CreateVvLabel  (0, 0, "DatumDOD:", ContentAlignment.MiddleRight);
      tbx_DokDate = hamper.CreateVvTextBox(1, 0, "tbx_dokDate", "Datum ugovora");
      tbx_DokDate.JAM_IsForDateTimePicker = true;
      dtp_DokDate = hamper.CreateVvDateTimePicker(1, 0, "", tbx_DokDate);//PTG_DatUgovora     
      dtp_DokDate.Name = "dtp_DokDate";
      tbx_DokDate.Font = ZXC.vvFont.BaseBoldFont;

                     hamper.CreateVvLabel  (0, 1, "DatDostave:", ContentAlignment.MiddleRight);
      tbx_DokDate2 = hamper.CreateVvTextBox(1, 1, "tbx_DokDate2", "Datum dostave");
      tbx_DokDate2.JAM_IsForDateTimePicker = true;
      dtp_DokDate2 = hamper.CreateVvDateTimePicker(1, 1, "", tbx_DokDate2);//PTG_DatDostave     
      dtp_DokDate2.Name = "dtp_DokDate2";

                           hamper.CreateVvLabel        (0, 2,       "MjestoIsp:"       , ContentAlignment.MiddleRight);
      rbt_mjIsp_Korisnik = hamper.CreateVvRadioButton  (1, 2, null, "Korisnik"         , TextImageRelation.ImageBeforeText);
      rbt_mjIsp_KorisOvers = hamper.CreateVvRadioButton(2, 2, null, "Korisnik-Overseas", TextImageRelation.ImageBeforeText);
      rbt_mjIsp_PcToGo = hamper.CreateVvRadioButton    (3, 2, null, "PCTOGO"           , TextImageRelation.ImageBeforeText);
      rbt_mjIsp_Korisnik.Checked = true;
      rbt_mjIsp_Korisnik.Tag = true;


                       hamper.CreateVvLabel  (2, 1, "Narudžba:", ContentAlignment.MiddleRight);
      tbx_OpciAlabel = hamper.CreateVvTextBox(3, 1, "tbx_OpciAlabel", "Narudžba", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.opciAlabel), 1, 0);//PTG_Narudzba

      //               hamper.CreateVvLabel  (3, 2, "Lokacija:", ContentAlignment.MiddleRight);
      //tbx_DostAddr = hamper.CreateVvTextBox(4, 2, "tbx_DostAddr", "Adresa dostave", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.dostAddr), 2, 0);//PTG_Lokacija

      this.ControlForInitialFocus = tbx_OpciAlabel;
   }

   private void InitializeHamper_Napomena_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 3, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth + ZXC.QUN, ZXC.Q10un * 2 + ZXC.Q4un - ZXC.Qun4 - ZXC.Qun12 };
      hamper.VvSpcBefCol   = new int[] { faBefFirstCol, faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt      = new int[] { ZXC.QUN , ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                     hamper.CreateVvLabel  (0, 0, "Napomena:", ContentAlignment.MiddleRight);
      tbx_Napomena = hamper.CreateVvTextBox(1, 0, "tbx_napomena", "Napomena", GetDB_ColumnSize(DB_ci.napomena), 0, 2);
      tbx_Napomena.Font = ZXC.vvFont.SmallFont;
      tbx_Napomena.Font = ZXC.vvFont.SmallFont;
      tbx_Napomena.Multiline = true;
      tbx_Napomena.ScrollBars = ScrollBars.Both;

   }

   private void InitializeHamper_Skladista_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth - ZXC.Qun2, ZXC.Q3un - ZXC.Qun2, ZXC.Q5un, ZXC.Q2un - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { faBefFirstCol, faBefCol, faBefCol, faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel        (0, 0, "IzlSkl:", ContentAlignment.MiddleRight);
      tbx_SkladCd   = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_SkladCd", "Skladište u najam");
      tbx_SkladOpis = hamper.CreateVvTextBox      (2, 0, "tbx_SkladOpiS", "");
      tbx_SkladBR   = hamper.CreateVvTextBox      (3, 0, "tbx_SkladRbr", "");
      tbx_SkladCd.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_SkladCd.JAM_DataRequired = true;
      tbx_SkladCd.JAM_MustTabOutBeforeSubmit = true;

      tbx_SkladCd  .JAM_ReadOnly = true;
      tbx_SkladOpis.JAM_ReadOnly = true;
      tbx_SkladBR  .JAM_ReadOnly = true;

      tbx_SkladCd.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_SkladCd.JAM_lui_NameTaker_JAM_Name = tbx_SkladOpis.JAM_Name;
      tbx_SkladCd.JAM_lui_IntegerTaker_JAM_Name = tbx_SkladBR.JAM_Name;
      tbx_SkladCd.VVtag2 = true;

                      hamper.CreateVvLabel         (0, 1, "UlzSkl:", ContentAlignment.MiddleRight);
      tbx_Sklad2Cd  = hamper.CreateVvTextBoxLookUp (1, 1, "tbx_SkladCd", "Skladište povrata iy najma");
      tbx_Sklad2Opis = hamper.CreateVvTextBox      (2, 1, "tbx_SkladOpiS", "");
      tbx_SkladRbr2 = hamper.CreateVvTextBox       (3, 1, "tbx_SkladRbr", "");
      tbx_Sklad2Cd.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_Sklad2Cd.JAM_DataRequired = true;
      tbx_Sklad2Cd.JAM_MustTabOutBeforeSubmit = true;

      tbx_Sklad2Cd  .JAM_ReadOnly = true;
      tbx_Sklad2Opis.JAM_ReadOnly = true;
      tbx_SkladRbr2.JAM_ReadOnly = true;

      tbx_Sklad2Cd.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_Sklad2Cd.JAM_lui_NameTaker_JAM_Name = tbx_Sklad2Opis.JAM_Name;
      tbx_Sklad2Cd.JAM_lui_IntegerTaker_JAM_Name = tbx_SkladRbr2.JAM_Name;
      tbx_Sklad2Cd.VVtag2 = true;

   }


   //public void GoTo_UGAN_Dokument_Click(object sender, EventArgs e)
   //{
   //   Point vvSubModulXY;
   //   string tt;

   //   if(Fld_V1_ttNum.IsZero())
   //   {
   //      vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_UGO_PTG);
   //      tt = Faktur.TT_UGN;
   //   }
   //   else
   //   {
   //      vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_ANU_PTG);
   //      tt = Faktur.TT_AUN;
   //   }

   //   VvTabPage existingTabPage = ZXC.TheVvForm.TheVvTabPage.TheVvForm.TheTabControl.TabPages.Cast<VvTabPage>().FirstOrDefault(tab => tab.WriteMode == ZXC.WriteMode.None && tab.SubModul_xy == vvSubModulXY);

   //   Faktur linkedFaktur_rec;

   //   if(existingTabPage == null) linkedFaktur_rec = new Faktur();
   //   else                        linkedFaktur_rec = (Faktur)existingTabPage.TheVvDataRecord;

   //   bool dbOK = FakturDao.SetMeFaktur(ZXC.TheVvForm.TheDbConnection, linkedFaktur_rec, tt, (Fld_V1_ttNum * 100000) + Fld_V2_ttNum, false);

   //   if(dbOK == false) return;

   //   if(existingTabPage != null)
   //   {
   //      existingTabPage.Selected = true;

   //      string currTT  = ((FakturDUC)(existingTabPage.TheVvDocumentRecordUC)).Fld_TT;
   //      uint currTTnum = ((FakturDUC)(existingTabPage.TheVvDocumentRecordUC)).Fld_TtNum;

   //      //if(currTT == _tt && currTTnum == _ttNum) return;

   //      if(dbOK)
   //      {
   //         existingTabPage.TheVvForm.PutFieldsActions(ZXC.TheVvForm.TheDbConnection, linkedFaktur_rec, existingTabPage.TheVvRecordUC);
   //      }
   //   }
   //   else
   //   {
   //      ZXC.TheVvForm.OpenNew_Record_TabPage(vvSubModulXY, linkedFaktur_rec.RecID);
   //   }

   //}

   #endregion Hampers

   #region Fld

   public DateTime Fld_PTG_DatDostave { get { return dtp_DokDate2.Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_DokDate2.Value = value; } } }

   public ZXC.PTG_MjestoIsporukeEnum Fld_PTG_MjestoIsporuke //_pdvZPkind
   {
      get
      {
              if(rbt_mjIsp_Korisnik  .Checked) return ZXC.PTG_MjestoIsporukeEnum.Korisnik        ;
         else if(rbt_mjIsp_KorisOvers.Checked) return ZXC.PTG_MjestoIsporukeEnum.KorisnikOverseas;
         else if(rbt_mjIsp_PcToGo    .Checked) return ZXC.PTG_MjestoIsporukeEnum.PcToGo          ;

         else throw new Exception("Fld_PTG_MjestoIsporuke: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.PTG_MjestoIsporukeEnum.Korisnik        : rbt_mjIsp_Korisnik  .Checked = true; break;
            case ZXC.PTG_MjestoIsporukeEnum.KorisnikOverseas: rbt_mjIsp_KorisOvers.Checked = true; break;
            case ZXC.PTG_MjestoIsporukeEnum.PcToGo          : rbt_mjIsp_PcToGo    .Checked = true; break;
         }
      }
   }

   #endregion Fld

   #region TheG_Specific_Columns
   public override bool HasRtrans_SkladCD_Exposed { get { return true; } }
   public override bool HasRtrano_SkladCD_Exposed { get { return true; } }
   protected override void InitializeDUC_Specific_Columns()
   {
      T_artiklCD_CreateColumn      (ZXC.Q3un           ,    true, "Šifra"   , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                        true, "Naziv"   , "Naziv artikla ili proizvoljan opis");
      T_doCijMal_CreateColumn      (ZXC.Q3un, 0,            true, "RAM"     , "RAM", false);
      T_noCijMal_CreateColumn      (ZXC.Q3un, 0,            true, "HDD"     , "HDD");
      T_skladCD_CreateColumn       (ZXC.Q3un,               true, "Sklad"   , "Izlazno skladište");
      T_jedMj_CreateColumn         (ZXC.Q2un           ,    true, "JM"      , "Jedinica mjere"                    );
      T_kol_CreateColumn           (ZXC.Q3un           , 2, true, "Kol"     , "Količina"                );
      T_cij_CreateColumn           (ZXC.Q4un           , 4, true, "Cijena"  , "Jedinična cijena"                  );
      T_rbt1St_CreateColumn        (ZXC.Q3un - ZXC.Qun4, 2, true, "Rbt"     , "Stopa rabata");
      R_cij_kcr_CreateColumn       (ZXC.Q4un           , 2, true, "CijSRbt" , "Cijena s rabatom");
      R_rbt1_CreateColumn          (ZXC.Q4un           , 2, true, "IznosRbt", "Iznos rabata");
      R_KCR_CreateColumn           (ZXC.Q4un,            2, true, "Iznos"   , "Ukupan iznos bez PDV-a");
    //R_NC_CreateColumn            (ZXC.Q4un           , 2, true, "NabCij"  , "Nabavna cijena");
    //R_NV_CreateColumn            (ZXC.Q4un           , 2, true, "NabVri"  , "Nabavna vrijednost");
    //R_RUC_CreateColumn           (ZXC.Q4un           , 2, true, "RUC"     , "RUC - razlika u cijeni");
    //R_RUV_CreateColumn           (ZXC.Q4un           , 2, true, "RUV"     , "RUV - razlika u vrijednosti");

   }

   #endregion TheG_Specific_Columns

   #region TheG_Specific_Columns2

   protected override void InitializeDUC_Specific_Columns2()
   {
      bool isVisible = true;

      T_artiklCD2_CreateColumn      (ZXC.Q5un, isVisible, "Šifra"        , "Šifra artikla"                     );
      T_artiklName2_CreateColumnFill(          isVisible, "Naziv"        , "Naziv artikla ili proizvoljan opis");
      T_skladCD2_CreateColumn       (ZXC.Q3un, isVisible, "Sklad"        , "Izlazno skladište"                 );
      T_serno_CreateColumn          (ZXC.Q8un, isVisible, "Serijski broj", "Serijski broj artikla"             );
   }

   #endregion TheG_Specific_Columns2


   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_DOD_PTG, Color.Empty, clr_DOD_PTG);
   }
}

public class PRN_DOD_PTG_DUC : DOD_PTG_DUC
{
   #region Constructor

   public PRN_DOD_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
   }

   #endregion Constructor

}

public class MSI_PTG_DUC : FakturExtDUC 
{
   public virtual string IZLAZ_SKL { get { return ""; } }
   public virtual string ULAZ_SKL  { get { return ""; } }

   #region Constructor

   public MSI_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
      (Faktur.tt_colName, new string[]
      {
         Faktur.TT_UGN // TT_IUN 
      });

   }

   #endregion Constructor

}


public class FUG_PTG_UC : VvUserControl
{
   public List<PTG_Rata> The_Rata_List_ZaFakturiranje { get; set; }

   #region Fieldz

   private VvHamper hamp_Izbor, hamp_datumi, hamp_ispisRacuna, hamp_suma;
   private VvTextBox tbx_DateOd, tbx_DateDo, tbx_DatumRacuna, tbx_RacunOd, tbx_RacunDo, tbx_FUGid, tbx_FUGidfilter, tbx_sumaSvihRata;
   private VvDateTimePicker dtp_DateOd, dtp_DateDo, dtp_DatumRacuna;
   private RadioButton rbt_grupnoFak, rbt_pojedinFak, rbt_otkupFak, rbt_uslugeA1Fak,
                       rbt_ispisNaEmail, rbt_ispisNaPisacSvi, rbt_ispisNaPisacPosta, rbt_ispisNaPdf, rbt_ispisEracun;

   internal Crownwood.DotNetMagic.Controls.TabControl ThePolyGridTabControl { get; set; }
   internal Crownwood.DotNetMagic.Controls.TabPage    tabPage_ListaRata, tabPage_ListaFaktura;

   public VvDataGridView TheG_1 { get; set; }
   public VvDataGridView TheG_2 { get; set; }
   public VvDataGridView TheG1_SumGrid, TheG2_SumGrid;

   private VvTextBox vvtb_partner     , vvtb_partnerCd,
                     vvtb_rataDate    , vvtb_rataRbr, vvtb_serial, vvtb_rataSastav,

                     vvtb_uganNum, 
                     vvtb_isDodatak   , 
                     vvtb_rataIznos   ,
                     vvtb_dateOd      , 
                     vvtb_dateDo      , 
                     vvtb_isEmail     ,
                     vvtb_zaduzen     , 
                     vvtb_napomNaRn  ,
                     vvtb_racun, vvtb_datumRn, vvtb_iznosRn, vvtb_fugId, vvtb_feedback,
                     vvtb_email,
                     vvtb_kupac,
                     vvtb_naEmail,
                     vvtb_eRacun,
                     vvtb_napomenaRn ;

   private VvTextBoxColumn colVvText;
   private DataGridViewTextBoxColumn colScrol;
   private VvCheckBoxColumn colfakStop;
   private VvCheckBox cbx_fakStop;

   #endregion Fieldz

   #region Constructor

   public FUG_PTG_UC(Control _parent, VvForm.VvSubModul vvSubModul)
   {
      this.SuspendLayout();
      
      this.Parent = _parent;
      this.Dock = DockStyle.Fill;

      CreateThePolyGridTabControl();
      SetEnableDisableTsButtons(true);

      ThePolyGridTabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(ThePolyGridTabControl_SelectionChanged);

      CreateHampers();

      CreateGrids();


      this.ResumeLayout();

    //InitializeHamper_SumaRata(out hamp_suma);
    //hamp_suma.Location = new Point(TheG_1.Left, TheG_1.Bottom - ZXC.Q2un);
    //hamp_suma.Anchor = AnchorStyles.Top | AnchorStyles.Left /*| AnchorStyles.Right | AnchorStyles.Bottom*/;
    //hamp_suma.BringToFront();


      SetTheG_1_ColumnIndexes();
      SetTheG_2_ColumnIndexes();

      SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.None);
      SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.None);

   }

   public void SetEnableDisableTsButtonsOnFugDUC(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   {
      bool isFUGgettingVisibility = TheVvTabPage.TheVvForm.TheTabControl.SelectedTab.Title == "FUG [PCTOGO]";

      bool isNewPage = newPage.StartFocus == null;

      if(isFUGgettingVisibility /*&& isNewPage != true/*TheVvTabPage.TheVvUC != null*/)
      {
         if(((FUG_PTG_UC)TheVvTabPage.TheVvUC).ThePolyGridTabControl.SelectedIndex == 0)
         {
            SetEnableDisableTsButtons(true);
         }
         else
         {
            SetEnableDisableTsButtons(false);
         }
      }
   }

   #endregion Constructor

   #region CreateThePolyGridTabControl

   private string TabPageTitle1 { get { return "Lista rata za fakturiranje"  ; } }
   private string TabPageTitle2 { get { return "Lista faktura za slanje"; } } //°

   private void CreateThePolyGridTabControl()
   {
      ThePolyGridTabControl                  = new Crownwood.DotNetMagic.Controls.TabControl();
      ThePolyGridTabControl.Parent           = this;
      ThePolyGridTabControl.Dock             = DockStyle.Fill;
      ThePolyGridTabControl.Appearance       = Crownwood.DotNetMagic.Controls.VisualAppearance.MultiDocument;
      ThePolyGridTabControl.ShowArrows       = false;
      ThePolyGridTabControl.ShowClose        = false;
      ThePolyGridTabControl.HotTrack         = true;
      ThePolyGridTabControl.Style            = ZXC.vvColors.vvform_VisualStyle;
      ThePolyGridTabControl.OfficeStyle      = ZXC.vvColors.tabControl_OfficeStyle;
      ThePolyGridTabControl.MediaPlayerStyle = ZXC.vvColors.tabControl_MediaPlayerStyle;

      tabPage_ListaRata = new Crownwood.DotNetMagic.Controls.TabPage(TabPageTitle1);
      ThePolyGridTabControl.TabPages.Add(tabPage_ListaRata);
      tabPage_ListaRata.BackColor = Color.NavajoWhite;
      tabPage_ListaRata.Name = TabPageTitle1;


      tabPage_ListaFaktura = new Crownwood.DotNetMagic.Controls.TabPage(TabPageTitle2);
      ThePolyGridTabControl.TabPages.Add(tabPage_ListaFaktura);
      tabPage_ListaFaktura.BackColor = Color.FromArgb(255, 255, 165);

   }

   #endregion CreateThePolyGridTabControl

   #region ThePolyGridTabControl_SelectionChanged

   internal void ThePolyGridTabControl_SelectionChanged(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   {
      bool isNewPageTabPageListaRata = newPage == ThePolyGridTabControl.TabPages[0];

      SetEnableDisableTsButtons(isNewPageTabPageListaRata);
   }

   private void SetEnableDisableTsButtons(bool enable)
   {
      Point xy     = ZXC.TheVvForm.TheVvTabPage.SubModul_xy;
      ToolStrip ts = ZXC.TheVvForm.ats_SubModulSet[xy.X][xy.Y];

      ts.Items["IzlisRate"  ].Enabled =
      ts.Items["KOP"        ].Enabled =
      ts.Items["KreirFak"   ].Enabled = enable;
      ts.Items["IzlisFak"   ].Enabled =
      ts.Items["IsporuciFak"].Enabled = !enable;
   }

   #endregion ThePolyGridTabControl_SelectionChanged

   #region Hampers

   private void CreateHampers()
   {
      InitializeHamper_Izbor(out hamp_Izbor);
      InitializeHamper_Datumi(out hamp_datumi);
      hamp_Izbor .Location = new Point(ZXC.QunMrgn, ZXC.Qun8);
      hamp_datumi.Location = new Point(hamp_Izbor.Right, ZXC.Qun8);

      InitializeHamper_Ispis(out hamp_ispisRacuna);
      hamp_ispisRacuna.Location = new Point(ZXC.QunMrgn, ZXC.Qun8);
   }

   private void InitializeHamper_Izbor(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 1, "", tabPage_ListaRata, false);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q4un, ZXC.Q4un + ZXC.Qun2, ZXC.Q4un - ZXC.Qun2, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel      (0, 0,                                "Izbor:"     , ContentAlignment.MiddleRight);
      rbt_grupnoFak    = hamper.CreateVvRadioButton(1, 0, new EventHandler(rbt_checked), "Grupno"     , TextImageRelation.ImageBeforeText);
      rbt_pojedinFak   = hamper.CreateVvRadioButton(2, 0, new EventHandler(rbt_checked), "Pojedinačno", TextImageRelation.ImageBeforeText);
      rbt_otkupFak     = hamper.CreateVvRadioButton(3, 0, new EventHandler(rbt_checked), "Otkup"      , TextImageRelation.ImageBeforeText);
      rbt_uslugeA1Fak  = hamper.CreateVvRadioButton(4, 0, new EventHandler(rbt_checked), "Usluge A1"  , TextImageRelation.ImageBeforeText);
      rbt_grupnoFak.Checked = true;
      rbt_grupnoFak.Tag = true;
   }

   private void rbt_checked(object sender, EventArgs e)
   {
      TheVvTabPage.TheVvForm.RISK_PTG_Izlistaj_TheG1_RateZaFak(null, EventArgs.Empty);
   }

   private void InitializeHamper_Datumi(out VvHamper hamper)
   {
      hamper = new VvHamper(8, 1, "", tabPage_ListaRata, false);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q5un, ZXC.Q2un - ZXC.Qun2, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q3un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8,            ZXC.Qun8, ZXC.Qun8, ZXC.QUN , ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                   hamper.CreateVvLabel  (0, 0, "Datum od:", ContentAlignment.MiddleRight);
      tbx_DateOd = hamper.CreateVvTextBox(1, 0, "tbx_DateOd", "Datum od");
      tbx_DateOd.JAM_IsForDateTimePicker = true;
      dtp_DateOd = hamper.CreateVvDateTimePicker(1, 0, "", tbx_DateOd);
      dtp_DateOd.Name = "dtp_DateOd";

                   hamper.CreateVvLabel  (2, 0, "do:", ContentAlignment.MiddleRight);
      tbx_DateDo = hamper.CreateVvTextBox(3, 0, "tbx_DateDo", "Datum do");
      tbx_DateDo.JAM_IsForDateTimePicker = true;
      dtp_DateDo = hamper.CreateVvDateTimePicker(3, 0, "", tbx_DateDo);
      dtp_DateDo.Name = "dtp_DateDo";

                        hamper.CreateVvLabel  (4, 0, "Datum računa:", ContentAlignment.MiddleRight);
      tbx_DatumRacuna = hamper.CreateVvTextBox(5, 0, "tbx_DatumRacuna", "Datum računa");
      tbx_DatumRacuna.JAM_IsForDateTimePicker = true;
      dtp_DatumRacuna = hamper.CreateVvDateTimePicker(5, 0, "", tbx_DatumRacuna);
      dtp_DatumRacuna.Name = "dtp_DatumRacuna";

                   hamper.CreateVvLabel  (6, 0, "FUG ID:", ContentAlignment.MiddleRight);
      tbx_FUGid  = hamper.CreateVvTextBox(7, 0, "tbx_FUGid", "FUG ID");
      tbx_FUGid.JAM_ReadOnly = true;
      tbx_FUGid.JAM_ForeColor = Color.BlueViolet;

      VvHamper.Open_Close_Fields_ForWriting(tbx_DateOd     , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvRecordUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_DateDo     , ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvRecordUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_DatumRacuna, ZXC.ZaUpis.Otvoreno , ZXC.ParentControlKind.VvRecordUC);
      VvHamper.Open_Close_Fields_ForWriting(tbx_FUGid      , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvRecordUC);
      VvHamper.ApplyVVColorAndStyleTabCntrolChange(tbx_FUGid);

   }

   private void InitializeHamper_Ispis(out VvHamper hamper)
   {
      hamper = new VvHamper(12, 1, "", tabPage_ListaFaktura, false);
      //                                      0         1              2             3          4         5         6               7                   8                    9                   10                   11     
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q4un, ZXC.Q2un - ZXC.Qun4, ZXC.Q4un, ZXC.Q3un, ZXC.Q4un, ZXC.Q4un, ZXC.Q3un + ZXC.Qun4, ZXC.Q3un + ZXC.Qun2, ZXC.Q3un + ZXC.Qun2, ZXC.Q3un + ZXC.Qun2, ZXC.Q3un + ZXC.Qun2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,            ZXC.Qun8,            ZXC.Qun8,            ZXC.Qun8,            ZXC.Qun8,            ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Račun od:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(2, 0, "do:", ContentAlignment.MiddleRight);

      tbx_RacunOd               = hamper.CreateVvTextBox(1, 0, "tbx_RacunOd", "Račun od", 6);
      tbx_RacunOd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_RacunOd.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_RacunDo = hamper.CreateVvTextBox(3, 0, "tbx_RacunDo", "Račun do", 6);
      tbx_RacunDo.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_RacunDo.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      hamper.CreateVvLabel  (4, 0, "FUG ID:", ContentAlignment.MiddleRight);
      tbx_FUGidfilter = hamper.CreateVvTextBox(5, 0, "tbx_FUGidfilter", "FUG ID");

                              hamper.CreateVvLabel      ( 6, 0,       "Isporuka na:", ContentAlignment.MiddleRight);
      rbt_ispisNaEmail      = hamper.CreateVvRadioButton( 7, 0, null, "e-mail"      , TextImageRelation.ImageBeforeText);
      rbt_ispisNaPisacPosta = hamper.CreateVvRadioButton( 8, 0, null, "poštom"      , TextImageRelation.ImageBeforeText);
      rbt_ispisEracun       = hamper.CreateVvRadioButton( 9, 0, null, "eRačun"      , TextImageRelation.ImageBeforeText);
      rbt_ispisNaPisacSvi   = hamper.CreateVvRadioButton(10, 0, null, "Pisač-svi"   , TextImageRelation.ImageBeforeText);
      rbt_ispisNaPdf        = hamper.CreateVvRadioButton(11, 0, null, "PDF"         , TextImageRelation.ImageBeforeText);
      rbt_ispisNaEmail.Checked = true;
      rbt_ispisNaEmail.Tag = true;

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvRecordUC);

   }


   private void InitializeHamper_SumaRata(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", tabPage_ListaRata, false);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel  (0, 0, "Zbroj rata:", ContentAlignment.MiddleRight);
      tbx_sumaSvihRata = hamper.CreateVvTextBox(1, 0, "tbx_sumaSvihRata", "Datum Zbroj svih rata", 12);
      tbx_sumaSvihRata.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_sumaSvihRata.JAM_ReadOnly = true;

      VvHamper.Open_Close_Fields_ForWriting(tbx_sumaSvihRata, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvRecordUC);
      VvHamper.ApplyVVColorAndStyleTabCntrolChange(tbx_sumaSvihRata);
   }


   #endregion Hampers

   #region Fld

   public DateTime Fld_DatumOd     { get { return dtp_DateOd     .Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_DateOd     .Value = value; } } }
   public DateTime Fld_DatumDo     { get { return dtp_DateDo     .Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_DateDo     .Value = value; } } }
   public DateTime Fld_DatumRacuna { get { return dtp_DatumRacuna.Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_DatumRacuna.Value = value; } } }

   public PTG_Ugovor.PTG_FakturiranjeKind Fld_PTG_FakturiranjeKind
   {
      get
      {
              if(rbt_grupnoFak  .Checked) return PTG_Ugovor.PTG_FakturiranjeKind.Grupno        ;
         else if(rbt_pojedinFak .Checked) return PTG_Ugovor.PTG_FakturiranjeKind.Pojedinacno   ;
         else if(rbt_otkupFak   .Checked) return PTG_Ugovor.PTG_FakturiranjeKind.Otkup         ;
         else if(rbt_uslugeA1Fak.Checked) return PTG_Ugovor.PTG_FakturiranjeKind.UslugeA1grupno;

         else throw new Exception("Fld_PTG_FakturiranjeKind: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case PTG_Ugovor.PTG_FakturiranjeKind.Grupno        : rbt_grupnoFak  .Checked = true; break;
            case PTG_Ugovor.PTG_FakturiranjeKind.Pojedinacno   : rbt_pojedinFak .Checked = true; break;
            case PTG_Ugovor.PTG_FakturiranjeKind.Otkup         : rbt_otkupFak   .Checked = true; break;
            case PTG_Ugovor.PTG_FakturiranjeKind.UslugeA1grupno: rbt_uslugeA1Fak.Checked = true; break;
         }
      }
   }

   public string Fld_PTG_FUG_ID        { get { return tbx_FUGid.Text      ; } set { tbx_FUGid      .Text = value; }  }

   public string Fld_PTG_FUG_ID_Filter  { get { return tbx_FUGidfilter.Text; } set { tbx_FUGidfilter.Text = value; } }

   public uint Fld_PTG_RacunOd_Filter { get { return ZXC.ValOrZero_UInt(tbx_RacunOd.Text); } set { tbx_RacunOd.PutUintField(value); } }
   public uint Fld_PTG_RacunDo_Filter { get { return ZXC.ValOrZero_UInt(tbx_RacunDo.Text); } set { tbx_RacunDo.PutUintField(value); } }

   public PTG_Ugovor.PTG_SlanjeFakKind Fld_PTG_SlanjeFakKind
   {
      get
      {
              if(rbt_ispisNaEmail     .Checked) return PTG_Ugovor.PTG_SlanjeFakKind.eMail        ;
         else if(rbt_ispisNaPisacSvi  .Checked) return PTG_Ugovor.PTG_SlanjeFakKind.Knjigovodstvo;
         else if(rbt_ispisNaPisacPosta.Checked) return PTG_Ugovor.PTG_SlanjeFakKind.Print        ;
         else if(rbt_ispisNaPdf       .Checked) return PTG_Ugovor.PTG_SlanjeFakKind.Pdf          ;
         else if(rbt_ispisEracun      .Checked) return PTG_Ugovor.PTG_SlanjeFakKind.eRacun          ;

         else throw new Exception("Fld_PTG_SlanjeFakKind: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case PTG_Ugovor.PTG_SlanjeFakKind.eMail        : rbt_ispisNaEmail     .Checked = true; break;
            case PTG_Ugovor.PTG_SlanjeFakKind.Knjigovodstvo: rbt_ispisNaPisacSvi  .Checked = true; break;
            case PTG_Ugovor.PTG_SlanjeFakKind.Print        : rbt_ispisNaPisacPosta.Checked = true; break;
            case PTG_Ugovor.PTG_SlanjeFakKind.Pdf          : rbt_ispisNaPdf       .Checked = true; break;
            case PTG_Ugovor.PTG_SlanjeFakKind.eRacun       : rbt_ispisEracun      .Checked = true; break;
         }
      }
   }

 //public decimal Fld_S_SumaSvihRata { get { return tbx_sumaSvihRata.GetDecimalField(); } set { tbx_sumaSvihRata.PutDecimalField(value); } }


   #endregion Fld

   #region TheGrid and columns

   private void CreateGrids()
   {
      TheG_1 = CreateGrid(tabPage_ListaRata, "ListaRata", hamp_datumi.Bottom);
      CreateColumn_gridListaRata(TheG_1);
      TheG_1.CellMouseDoubleClick += TheG_1_CellMouseDoubleClick;

      TheG1_SumGrid = CreateSumGrid(TheG_1, TheG_1.Parent, "SUM" + TheG_1.Name);
      InitializePTG_SumGrid_Columns(TheG_1);
      GridLocationAndSize_TehG1AndSumGrid(TheG_1);


      TheG_2 = CreateGrid(tabPage_ListaFaktura, "ListaFaktura", hamp_ispisRacuna.Bottom);
      CreateColumn_gridListafaktura(TheG_2);
      TheG_2.CellMouseDoubleClick += TheG_2_CellMouseDoubleClick;

      TheG2_SumGrid = CreateSumGrid(TheG_2, TheG_2.Parent, "SUM" + TheG_2.Name);
      InitializePTG_SumGrid_Columns(TheG_2);
      GridLocationAndSize_TehG1AndSumGrid(TheG_2);

   }

   private void TheG_1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
   {
      VvDataGridView theG = sender as VvDataGridView;

      int rowIdx = e.RowIndex;

      if(rowIdx.IsNegative()) return;

      #region KupdobUC 

      // open KupdobUC 
      if(e.ColumnIndex == ci.iT_partner || e.ColumnIndex == ci.iT_isEmail || e.ColumnIndex == ci.iT_zaduzen)
      {
         uint kupdobCD = theG.GetUint32Cell(ci.iT_partnerCd, rowIdx, false);  
         Kupdob kupdobFromSifrar_rec = KupdobSifrar.SingleOrDefault(k => k.KupdobCD == kupdobCD);
         Kupdob kupdob_rec;
         if(kupdobFromSifrar_rec == null) return;
         kupdob_rec = kupdobFromSifrar_rec.MakeDeepCopy();
         Point xy = TheVvTabPage.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.KID);
         if(xy.IsEmpty) return;
         TheVvTabPage.TheVvForm.OpenNew_Record_TabPage_wInitialRecord(xy, kupdob_rec);
         return;
      }

      #endregion KupdobUC 

      #region UGAN_DUC or KOP_DUC from UGAN_DUC

      string tipBr = theG.GetStringCell(ci.iT_uganNum, rowIdx, false);

      // just open UGAN_DUC 
      ZXC.TheVvForm.ShowFakturDUC_For_TipBr(tipBr);

      // open KOP_DUC from UGAN_DUC 
      if(e.ColumnIndex == ci.iT_FakStop)
      {
         ZXC.KOPfromFUG_InProgress = true;

         ZXC.TheVvForm.PTG_CreateNew_OrEditExisting_KOP(new PTG_Ugovor(((Faktur)ZXC.TheVvForm.TheVvDataRecord)));
      }

      #endregion UGAN_DUC or KOP_DUC from UGAN_DUC

   }

   private void TheG_2_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
   {
      VvDataGridView theG = sender as VvDataGridView;

      int rowIdx = e.RowIndex;

      if(rowIdx.IsNegative()) return;

      string tipBr = theG.GetStringCell(ci2.iT_brRacuna, rowIdx, false);

      ZXC.TheVvForm.ShowFakturDUC_For_TipBr(tipBr);

   }

   private VvDataGridView CreateGrid(Control parent, string name, int nextY)
   {
      VvDataGridView theGrid = new VvDataGridView();

      theGrid.Name     = name;
      theGrid.Parent   = parent;
      theGrid.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn + nextY);

      theGrid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
      theGrid.AutoGenerateColumns                  = false;

      theGrid.RowHeadersBorderStyle = theGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
      theGrid.ColumnHeadersHeight   = ZXC.QUN + ZXC.Qun8;
      theGrid.RowTemplate.Height    = ZXC.QUN - ZXC.Qun12;
      theGrid.RowHeadersWidth       = ZXC.Q3un;
      theGrid.Height                = theGrid.ColumnHeadersHeight + theGrid.RowTemplate.Height;

      theGrid.CellFormatting += new DataGridViewCellFormattingEventHandler(VvDocumentRecordUC.grid_CellFormatting_FormatVvDateTime);
      theGrid.Validating     += new System.ComponentModel.CancelEventHandler(VvDocumentRecordUC.grid_Validating);

      VvHamper.ApplyVVColorAndStyleTabCntrolChange(theGrid);
      VvHamper.Open_Close_Fields_ForWriting(theGrid, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvRecordUC);

      theGrid.AllowUserToAddRows       =
      theGrid.AllowUserToDeleteRows    =
      theGrid.AllowUserToOrderColumns  =
      theGrid.AllowUserToResizeColumns =
      theGrid.AllowUserToResizeRows    = false;

      theGrid.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;
      theGrid.DefaultCellStyle.ForeColor = ZXC.vvColors.dataGridCellReadOnly_True_ForeColor;

      // maknuto 05.05.2022 nakon sto je PCTOGO postavljen na manji font
    //theGrid.ColumnHeadersDefaultCellStyle.Font = ZXC.vvFont.SmallSmallFont;
    //theGrid.RowsDefaultCellStyle         .Font = ZXC.vvFont.SmallFont;
    //theGrid.RowHeadersDefaultCellStyle   .Font = ZXC.vvFont.SmallFont;

      theGrid.Dock          = DockStyle.Fill;
      theGrid.Location      = new Point(ZXC.QunMrgn, ZXC.Qun8 + nextY);
      theGrid.Parent.Size   = new Size(this.Width, this.Height);
      theGrid.Parent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
      theGrid.Size          = new Size(this.Width - 2 * ZXC.QunMrgn, this.Height - ZXC.Q2un - nextY);
      theGrid.Anchor        = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

      return theGrid;
   }
   
   private void CreateColumn_gridListaRata(VvDataGridView theGrid)
   {
      cbx_fakStop = new VvCheckBox();
      colfakStop  = new VvCheckBoxColumn();
      colfakStop.Tag = cbx_fakStop;
      colfakStop.Name = "FakStop";
      colfakStop.HeaderText = "NeFak";
      colfakStop.ValueType = typeof(bool);
      colfakStop.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
      colfakStop.Width = ZXC.Q2un + ZXC.Qun2;
      theGrid.Columns.Add(colfakStop);
    //colfakStop = theGrid.CreateVvCheckBoxColumn(cbx_fakStop, null, DgvCI.iT_FakStop, "NeFak", ZXC.Q2un + ZXC.Qun2);

      vvtb_partner    = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_partner"  , null, -12, "Partner"          ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_partner  , null, "R_partner"  , "Partner"          , ZXC.Q10un          );
      vvtb_partnerCd  = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_partnerCd", null, -12, "Šifra"            ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_partnerCd, null, "R_partnerCd", "Šifra"            , ZXC.Q3un - ZXC.Qun8);
      vvtb_serial     = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_serial"   , null, -12, "RedOPL"           ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_serial   , null, "R_serial"   , "RedOPL"           , ZXC.Q3un - ZXC.Qun8); colVvText.DefaultCellStyle.Alignment = colVvText.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
      vvtb_rataRbr    = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_rataRbr"  , null, -12, "RbrRate"          ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_rataRbr  , null, "R_rataRbr"  , "RbrRate"          , ZXC.Q3un - ZXC.Qun8); colVvText.DefaultCellStyle.Alignment = colVvText.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
      vvtb_rataDate   = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_rataDate" , null, -12, "Datum rate"       ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_rataDate , null, "R_rataDate" , "Datum rate"       , ZXC.Q4un           );
      vvtb_uganNum    = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_uganNum"  , null, -12, "Ugovor br"        ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_uganNum  , null, "R_uganNum"  , "Ugovor br"        , ZXC.Q6un - ZXC.Qun2);
      vvtb_isDodatak  = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_isDodatak", null, -12, "Dodatak"          ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_isDodatak, null, "R_isDodatak", "Dodatak"          , ZXC.Q3un - ZXC.Qun8); colVvText.DefaultCellStyle.Alignment = colVvText.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
      vvtb_rataSastav = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_rataSastav", null, -12, "Sastav"          ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_rataSastav, null, "R_rataSastav", "Sastav"         , ZXC.Q3un - ZXC.Qun8); colVvText.DefaultCellStyle.Alignment = colVvText.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
      vvtb_rataIznos  = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_rataIznos", null, -12, "Iznos rate"       ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_rataIznos, null, "R_rataIznos", "Iznos rate"       , ZXC.Q4un           ); colVvText.DefaultCellStyle.Alignment = colVvText.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
      vvtb_dateOd     = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_dateOd"   , null, -12, "Datum od"         ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_dateOd   , null, "R_dateOd"   , "Datum od"         , ZXC.Q4un           ); 
      vvtb_dateDo     = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_dateDo"   , null, -12, "Datum do"         ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_dateDo   , null, "R_dateDo"   , "Datum do"         , ZXC.Q4un           ); 
      vvtb_isEmail    = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_isEmail"  , null, -12, "e-mail"           ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_isEmail  , null, "R_isEmail"  , "e-mail"           , ZXC.Q2un + ZXC.Qun2); colVvText.DefaultCellStyle.Alignment = colVvText.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
      vvtb_zaduzen    = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_zaduzen"  , null, -12, "Zadužen"          ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_zaduzen  , null, "R_zaduzen"  , "Zadužen"          , ZXC.Q7un           ); 
      vvtb_napomNaRn  = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_napomNaRn", null, -12, "Napomena za račun"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_napomNaRn, null, "R_napomNaRn", "Napomena za račun", ZXC.Q10un          ); colVvText.MinimumWidth = ZXC.Q10un; colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);
      colScrol.DefaultCellStyle.BackColor = TheG_1.ColumnHeadersDefaultCellStyle.BackColor;

      //foreach(DataGridViewTextBoxColumn dgvCol in theGrid.Columns)
      //{
      //   dgvCol.SortMode = DataGridViewColumnSortMode.NotSortable;
      //}

   }

   private void CreateColumn_gridListafaktura(VvDataGridView theGrid)
   {
      vvtb_racun      = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_racun"     , null, -12, "RačunBr"           ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_racun     , null, "R_brRacuna"  , "Broj računa"      , ZXC.Q4un - ZXC.Qun8);
      vvtb_datumRn    = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_datumRn"   , null, -12, "Datum"             ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_datumRn   , null, "R_datumRn"   , "Datum računa"     , ZXC.Q4un           );
      vvtb_iznosRn    = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_iznosRn"   , null, -12, "Iznos"             ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_iznosRn   , null, "R_iznosRn"   , "Iznos računa"     , ZXC.Q4un           ); colVvText.DefaultCellStyle.Alignment = colVvText.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
      vvtb_fugId      = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_fugId"     , null, -12, "FUG ID"            ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_fugId     , null, "R_fugId"     , "FUG ID"           , ZXC.Q4un           );
      vvtb_feedback   = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_feedback"  , null, -12, "Feedback"          ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_feedback  , null, "R_feedback"  , "Feedback"         , ZXC.Q5un           );
      vvtb_email      = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_email"     , null, -12, "e-mail"            ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_email     , null, "R_email"     , "e-mail"           , ZXC.Q8un           );
      vvtb_kupac      = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_kupac"     , null, -12, "Kupac"             ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_kupac     , null, "R_kupac"     , "Partner"          , ZXC.Q10un          );
      vvtb_naEmail    = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_naEmail"   , null, -12, "Na e-mail"         ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_naEmail   , null, "R_isNaEmail" , "mail"             , ZXC.Q2un + ZXC.Qun2); colVvText.DefaultCellStyle.Alignment = colVvText.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
      vvtb_eRacun     = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_eRacun"    , null, -12, "eRacun"            ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_eRacun    , null, "R_iseRacun"  , "eRacun"           , ZXC.Q2un + ZXC.Qun2); colVvText.DefaultCellStyle.Alignment = colVvText.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
      vvtb_napomenaRn = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_napomenaRn", null, -12, "Napomena za račun" ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_napomenaRn, null, "R_napomenaRn", "Napomena za račun", ZXC.Q10un          ); colVvText.MinimumWidth = ZXC.Q10un; colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);
      colScrol.DefaultCellStyle.BackColor = TheG_1.ColumnHeadersDefaultCellStyle.BackColor;

      //foreach(DataGridViewTextBoxColumn dgvCol in theGrid.Columns)
      //{
      //   dgvCol.SortMode = DataGridViewColumnSortMode.NotSortable;
      //}
   }

   #region SumGrid

   private VvDataGridView CreateSumGrid(VvDataGridView theGrid, Control _parent, string _name)
   {
      VvDataGridView theSumGrid = new VvDataGridView();

      theGrid.TheLinkedGrid_Sum = theSumGrid;

      theSumGrid.Parent = _parent;
      theSumGrid.Name = _name;

      theSumGrid.Height = ZXC.QUN + ZXC.Qun10; //23;

      theSumGrid.BorderStyle = BorderStyle.FixedSingle;
      theSumGrid.ColumnHeadersVisible = false;
      theSumGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleVertical;// DataGridViewCellBorderStyle.None;
      theSumGrid.ReadOnly = true;
      theSumGrid.Tag = theGrid;
      VvHamper.ApplyVVColorAndStyleTabCntrolChange(theSumGrid);

      theSumGrid.AllowUserToAddRows =
      theSumGrid.AllowUserToDeleteRows =
      theSumGrid.AllowUserToOrderColumns =
      theSumGrid.AllowUserToResizeColumns =
      theSumGrid.AllowUserToResizeRows = false;

      theSumGrid.RowHeadersDefaultCellStyle.Alignment = theGrid.RowHeadersDefaultCellStyle.Alignment;
      theSumGrid.RowTemplate.Height = theGrid.RowTemplate.Height;

      theSumGrid.ScrollBars = ScrollBars.None;

      //theSumGrid.Location = new Point(theGrid.Left, theGrid.Bottom);

      return theSumGrid;
   }

   private void InitializePTG_SumGrid_Columns(VvDataGridView theGrid)
   {
      DataGridView theSumGrid = theGrid.TheLinkedGrid_Sum;

      DataGridViewTextBoxColumn gridSumColumn;

      foreach(DataGridViewColumn mainGridColumn in theGrid.Columns)
      {
         gridSumColumn = new DataGridViewTextBoxColumn();

         //gridSumColumn.Name                       = "SUM" + mainGridColumn.Name.TrimStart(new char[] { 't' });
         gridSumColumn.Name = mainGridColumn.Name;
         gridSumColumn.DefaultCellStyle.Alignment = mainGridColumn.DefaultCellStyle.Alignment;
         gridSumColumn.AutoSizeMode = mainGridColumn.AutoSizeMode;
         theSumGrid.AutoGenerateColumns = false;
         gridSumColumn.Width = mainGridColumn.Width;
         gridSumColumn.ValueType = mainGridColumn.ValueType;
         gridSumColumn.Visible = mainGridColumn.Visible;
         gridSumColumn.Tag = mainGridColumn.Tag;
         gridSumColumn.DefaultCellStyle.Format = mainGridColumn.DefaultCellStyle.Format;
         gridSumColumn.DefaultCellStyle.BackColor = mainGridColumn.DefaultCellStyle.BackColor;

         if(mainGridColumn.AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
         {
            gridSumColumn.MinimumWidth = mainGridColumn.MinimumWidth;
            gridSumColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

         }
         theSumGrid.Columns.Add(gridSumColumn);
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

   private void GridLocationAndSize_TehG1AndSumGrid(VvDataGridView theGrid)
   {
      DataGridView theSumGrid   = theGrid.TheLinkedGrid_Sum;

      theGrid.Height = this.Height - ZXC.Q2un - theSumGrid.Height;
      theSumGrid.Width    = this.Width - 2 * ZXC.QunMrgn;
      theSumGrid.Location = new Point(theGrid.Location.X, theGrid.Bottom + ZXC.Qun12);
      theSumGrid.Anchor   = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

      theGrid.Scroll += new ScrollEventHandler(PTG_TheG1_VScroll);

      //ResumeLayout();
   }

   private void PTG_TheG1_VScroll(object sender, ScrollEventArgs e)
   {
      VvDataGridView theGrid = sender as VvDataGridView;
      DataGridView theSumGrid = theGrid.TheLinkedGrid_Sum;

      theSumGrid.HorizontalScrollingOffset = theGrid.HorizontalScrollingOffset;

   }

   #endregion SumGrid

   #region SetColumnIndexes()

   private FugRate_Ptg_colIdx ci;
   public FugRate_Ptg_colIdx DgvCI { get { return ci; } }
   public struct FugRate_Ptg_colIdx
   {
      internal int iT_FakStop  ; 
      internal int iT_partner  ;
      internal int iT_partnerCd; 
      internal int iT_serial   ;
      internal int iT_rataRbr  ;
      internal int iT_rataDate ; 
      internal int iT_uganNum  ; 
      internal int iT_isDodatak; 
      internal int iT_rataSastav;
      internal int iT_rataIznos; 
      internal int iT_dateOd   ; 
      internal int iT_dateDo   ; 
      internal int iT_isEmail  ; 
      internal int iT_zaduzen  ; 
      internal int iT_napomNaRn; 
   }

   private void SetTheG_1_ColumnIndexes()
   {
      ci = new FugRate_Ptg_colIdx();

      ci.iT_FakStop   = TheG_1.IdxForColumn("FakStop"    );
      ci.iT_partner   = TheG_1.IdxForColumn("R_partner"  );
      ci.iT_partnerCd = TheG_1.IdxForColumn("R_partnerCd");
      ci.iT_serial    = TheG_1.IdxForColumn("R_serial"   );
      ci.iT_rataRbr   = TheG_1.IdxForColumn("R_rataRbr"  );
      ci.iT_rataDate  = TheG_1.IdxForColumn("R_rataDate" );
      ci.iT_uganNum   = TheG_1.IdxForColumn("R_uganNum"  );
      ci.iT_isDodatak = TheG_1.IdxForColumn("R_isDodatak");
      ci.iT_rataSastav= TheG_1.IdxForColumn("R_rataSastav");
      ci.iT_rataIznos = TheG_1.IdxForColumn("R_rataIznos");
      ci.iT_dateOd    = TheG_1.IdxForColumn("R_dateOd"   );
      ci.iT_dateDo    = TheG_1.IdxForColumn("R_dateDo"   );
      ci.iT_isEmail   = TheG_1.IdxForColumn("R_isEmail"  );
      ci.iT_zaduzen   = TheG_1.IdxForColumn("R_zaduzen"  );
      ci.iT_napomNaRn = TheG_1.IdxForColumn("R_napomNaRn");
   }

   private FugFak_Ptg_colIdx ci2;
   public  FugFak_Ptg_colIdx DgvCI2 { get { return ci2; } }
   public struct FugFak_Ptg_colIdx
   {
      internal int iT_brRacuna;
      internal int iT_datumRn;
      internal int iT_iznosRn;
      internal int iT_fugId; 
      internal int iT_email     ;
      internal int iT_partner     ; 
      internal int iT_isNaEmail ;
      internal int iT_iseRacun  ;
      internal int iT_napomenaRn;
      internal int iT_feedback;
   }

   private void SetTheG_2_ColumnIndexes()
   {
      ci2 = new FugFak_Ptg_colIdx();

      ci2.iT_brRacuna   = TheG_2.IdxForColumn("R_brRacuna"  );
      ci2.iT_datumRn    = TheG_2.IdxForColumn("R_datumRn"   );
      ci2.iT_iznosRn    = TheG_2.IdxForColumn("R_iznosRn"   );
      ci2.iT_fugId      = TheG_2.IdxForColumn("R_fugId"     );
      ci2.iT_email      = TheG_2.IdxForColumn("R_email"     );
      ci2.iT_partner    = TheG_2.IdxForColumn("R_kupac"     );
      ci2.iT_isNaEmail  = TheG_2.IdxForColumn("R_isNaEmail" );
      ci2.iT_iseRacun   = TheG_2.IdxForColumn("R_iseRacun"  );
      ci2.iT_napomenaRn = TheG_2.IdxForColumn("R_napomenaRn");
      ci2.iT_feedback   = TheG_2.IdxForColumn("R_feedback"  );
   }


   #endregion SetColumnIndexes()

   public void DGV_PTG_HasFakStop_CellValueChangedColor()
   {
      VvDataGridView dgv = TheG_1;
      int rowIdx;
      for(int i = 0; i < dgv.Rows.Count; i++)
      {
         rowIdx = i;

         for(int j = 0; j < dgv.Columns.Count-1; j++)
         {
            if(dgv.Rows[rowIdx].Cells[ci.iT_FakStop].Value != null && dgv.Rows[rowIdx].Cells[ci.iT_FakStop].Value.ToString() == "X")
            {
               dgv.Rows[rowIdx].Cells[j].Style.BackColor = Color.FromArgb(255, 164, 164);
            }

         }
      }
   }

   private void DGV_PTG_HasKOP(int rowIdx)
   {
      VvDataGridView dgv = TheG_1;

      for(int j = 0; j < dgv.Columns.Count - 1; j++)
      {
            dgv.Rows[rowIdx].Cells[j].Style.BackColor = Color.FromArgb(250, 227, 227); ;
      }
   }

   #endregion TheGridColumn

   #region PutDgvFields

   public void PutDgvFields1(/*List<PTG_Rata> the_Rata_List_ZaFakturiranje*/)
   {
      int rowIdx;
      decimal sumRata = 0M;

      TheG_1.Rows.Clear();
    //Fld_S_SumaSvihRata = 0.00M;

      if(The_Rata_List_ZaFakturiranje != null)
      {
         for(rowIdx = 0; rowIdx < The_Rata_List_ZaFakturiranje.Count; ++rowIdx)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheG_1.Rows.Add();

            PutDgvLineFields1(rowIdx, The_Rata_List_ZaFakturiranje[rowIdx]);

            TheG_1.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();

            DGV_PTG_HasFakStop_CellValueChangedColor();

            sumRata += The_Rata_List_ZaFakturiranje[rowIdx].RataMoney;
         }
      }

      TheG1_SumGrid.PutCell(ci.iT_rataIznos, 0, sumRata.ToStringVv());
   }

   private void PutDgvLineFields1(int rowIdx, PTG_Rata theRataZaFak)
   {
      TheG_1.PutCell(ci.iT_FakStop   , rowIdx, /*theRataZaFak.KOPfakStop);//*/VvCheckBox.GetString4Bool(theRataZaFak.KOPfakStop));
      TheG_1.PutCell(ci.iT_partner   , rowIdx, theRataZaFak.TheKupdob.Naziv);
      TheG_1.PutCell(ci.iT_partnerCd , rowIdx, theRataZaFak.TheKupdobCD.ToString("000000"));
      TheG_1.PutCell(ci.iT_serial    , rowIdx, theRataZaFak.Serial);
      TheG_1.PutCell(ci.iT_rataRbr   , rowIdx, theRataZaFak.RataRbr);
      TheG_1.PutCell(ci.iT_rataDate  , rowIdx, theRataZaFak.RataDate.ToString(ZXC.VvDateFormat));
      TheG_1.PutCell(ci.iT_uganNum   , rowIdx, theRataZaFak.TipBr);
      TheG_1.PutCell(ci.iT_isDodatak , rowIdx, theRataZaFak.RataRbr.IsNegative() ? "X" : "");
      TheG_1.PutCell(ci.iT_rataSastav, rowIdx, theRataZaFak.RataSastav);
      TheG_1.PutCell(ci.iT_rataIznos , rowIdx, theRataZaFak.RataMoney.ToStringVv());
      TheG_1.PutCell(ci.iT_dateOd    , rowIdx, theRataZaFak.DatumOd.ToString(ZXC.VvDateFormat));
      TheG_1.PutCell(ci.iT_dateDo    , rowIdx, theRataZaFak.DatumDo.ToString(ZXC.VvDateFormat));
      TheG_1.PutCell(ci.iT_isEmail   , rowIdx, theRataZaFak.TheKupdob.IsPTG_NacSlanja_eMail ? "X" : "");
      TheG_1.PutCell(ci.iT_zaduzen   , rowIdx, theRataZaFak.Putnik);
      TheG_1.PutCell(ci.iT_napomNaRn , rowIdx, theRataZaFak.UGANrnNapomena);

      if(theRataZaFak.HasKOP && theRataZaFak.KOPfakStop == false) DGV_PTG_HasKOP(rowIdx);
   }

   public override void GetFields(bool fuse)
   {
      // notin to do;
   }

   internal void PutDgvFields_FaktureZaSlanje(Faktur faktur_rec, PTG_Rata rata, int rowIdx)
   {
      TheG_2.PutCell(ci2.iT_brRacuna  , rowIdx, faktur_rec.TtNum.IsZero() ? "" : faktur_rec.TT_And_TtNum);
      TheG_2.PutCell(ci2.iT_datumRn   , rowIdx, faktur_rec.DokDate.ToString(ZXC.VvDateFormat));
      TheG_2.PutCell(ci2.iT_iznosRn   , rowIdx, faktur_rec.S_ukKCRP.ToStringVv());
      TheG_2.PutCell(ci2.iT_fugId     , rowIdx, faktur_rec.VezniDok);
      TheG_2.PutCell(ci2.iT_feedback  , rowIdx, faktur_rec.FiskPrgBr);
      TheG_2.PutCell(ci2.iT_email     , rowIdx, rata.TheKupdob.Email);
      TheG_2.PutCell(ci2.iT_partner   , rowIdx, faktur_rec.KupdobName);
      TheG_2.PutCell(ci2.iT_isNaEmail , rowIdx, rata.TheKupdob.IsPTG_NacSlanja_eMail  ? "X" : "");
      TheG_2.PutCell(ci2.iT_iseRacun  , rowIdx, rata.TheKupdob.IsPTG_NacSlanja_eRacun ? "X" : "");
      TheG_2.PutCell(ci2.iT_napomenaRn, rowIdx, rata.UGANrnNapomena);

      DGV2_PTG_ColorVrastSlanja(rowIdx, rata.TheKupdob.IsPTG_NacSlanja_eMail, rata.TheKupdob.IsPTG_NacSlanja_eRacun);
   }

   internal void PutDgvFields_FaktureZaSlanje(Faktur faktur_rec, Kupdob kupdob_rec, int rowIdx)
   {
      TheG_2.PutCell(ci2.iT_brRacuna  , rowIdx, faktur_rec.TT_And_TtNum);
      TheG_2.PutCell(ci2.iT_datumRn   , rowIdx, faktur_rec.DokDate.ToString(ZXC.VvDateFormat));
      TheG_2.PutCell(ci2.iT_iznosRn   , rowIdx, faktur_rec.S_ukKCRP.ToStringVv());
      TheG_2.PutCell(ci2.iT_fugId     , rowIdx, faktur_rec.VezniDok);
      TheG_2.PutCell(ci2.iT_feedback  , rowIdx, faktur_rec.FiskPrgBr);
      TheG_2.PutCell(ci2.iT_email     , rowIdx, kupdob_rec.Email);
      TheG_2.PutCell(ci2.iT_partner   , rowIdx, faktur_rec.KupdobName);
      TheG_2.PutCell(ci2.iT_isNaEmail , rowIdx, kupdob_rec.IsPTG_NacSlanja_eMail  ? "X" : "");
      TheG_2.PutCell(ci2.iT_iseRacun  , rowIdx, kupdob_rec.IsPTG_NacSlanja_eRacun ? "X" : "");
      TheG_2.PutCell(ci2.iT_napomenaRn, rowIdx, faktur_rec.Napomena);

      DGV2_PTG_ColorVrastSlanja(rowIdx, kupdob_rec.IsPTG_NacSlanja_eMail, kupdob_rec.IsPTG_NacSlanja_eRacun);
   }

   private void DGV2_PTG_ColorVrastSlanja(int rowIdx, bool is_eMail, bool is_eRacun)
   {
      VvDataGridView dgv = TheG_2;

      for(int j = 0; j < dgv.Columns.Count - 1; j++)
      {
         if(is_eMail)        dgv.Rows[rowIdx].Cells[j].Style.BackColor = Color.FromArgb(227, 250, 250); 
         else if(is_eRacun)  dgv.Rows[rowIdx].Cells[j].Style.BackColor = Color.FromArgb(243, 252, 252);
         else                dgv.Rows[rowIdx].Cells[j].Style.BackColor = Color.FromArgb(223, 238, 238);
      }
   }

   #endregion PutDgvFields

   public void PutDefaultFilterFields(uint ttNumOD, uint ttNumDO, string fugID)
   {
      Fld_PTG_RacunOd_Filter = ttNumOD;
      Fld_PTG_RacunDo_Filter = ttNumDO;

      Fld_PTG_FUG_ID_Filter  = fugID;
   }

}

public class VvBrojRataPlusMinus_PTG_Dlg : VvDialog
{
   #region Filedz

   private Button okButton, cancelButton;
   private VvHamper hamper;
   private int dlgWidth, dlgHeight;
   private VvTextBox tbx_brRataPlusMinus;

   #endregion Filedz

   #region Constructor

   public VvBrojRataPlusMinus_PTG_Dlg()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Broj rata povećaj/umanji za broj:";

      CreateHamper();

      dlgWidth = hamper.Right + ZXC.QunMrgn;
      dlgHeight = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_brRataPlusMinus, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
   }

   #endregion Constructor

   #region hamper

   private void CreateHamper()
   {
      hamper          = new VvHamper(2, 1, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q3un -ZXC.Qun2};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,          ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                            hamper.CreateVvLabel  (0, 0, "Broj rata +/-:", ContentAlignment.MiddleRight);
      tbx_brRataPlusMinus = hamper.CreateVvTextBox(1, 0, "tbx_brRataPlusMinus", "", 4);
      tbx_brRataPlusMinus.JAM_CharEdits = ZXC.JAM_CharEdits.NumericOnly;

   }

   #endregion hamper

   #region Button_Click

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Button_Click

   #region Fld_

   public int Fld_PTG_brRataPlusMinus { get { return tbx_brRataPlusMinus.GetIntField(); } set { tbx_brRataPlusMinus.PutIntField(value); } }

   #endregion Fld_

}


public partial class PCK_InfoDLG :  VvDialog
{
   public PCK_Info_UC TheUC { get; set; }
   private Button okButton, cancelButton;
   private int dlgWidth, dlgHeight;

   public PCK_InfoDLG()
   {
      ZXC.CurrentForm = this;

      TheUC = new PCK_Info_UC(this);

      SuspendLayout();

      this.Font        = ZXC.vvFont.BaseFont;
      this.Style       = ZXC.vvColors.vvform_VisualStyle;
      this.BackColor   = ZXC.vvColors.userControl_BackColor;

      this.StartPosition = FormStartPosition.Manual;
      this.Text = "PCK INFO";

      TheUC.Parent   = this;
      TheUC.Location = new Point(ZXC.Qun8, ZXC.Qun8);
      
      dlgWidth  = TheUC.Width;
      dlgHeight = TheUC.Height + ZXC.QunMrgn * 2 + ZXC.QunBtnH;

      this.MaximizeBox = true;

      this.ClientSize = new Size(dlgWidth, dlgHeight);
      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      this.AcceptButton = okButton;

      okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      TheUC.Anchor         =  AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      TheUC.ThePCKGrid.Anchor =  AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      
      this.cancelButton.Click += new EventHandler(cancelButton_Click); // Da supresa validaciju

      ResumeLayout();

   }
   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }
}

public class PCK_Info_UC : UserControl
{
   #region Fieldz

   public VvDataGridView ThePCKGrid   { get; set; }
   public VvDataGridView ThePCKSumGrid   { get; set; }
   private VvTextBox
         vvtb_PCK_ArtCD  ,  
         vvtb_PCK_ArtName,
         vvtb_PCK_RAMkind,
         vvtb_PCK_HDDkind,  
         vvtb_PCK_SklCD  ,  
         vvtb_PCK_RAM    ,  
         vvtb_PCK_HDD    ,  
         vvtb_UkPstKol   ,  
         vvtb_UkUlazKol  ,  
         vvtb_UkIzlazKol ,
         vvtb_StanjeKol  ;

   private VvTextBoxColumn colVvText;
   private DataGridViewTextBoxColumn colScrol;

   #endregion Fieldz

   #region Constructor

   public PCK_Info_UC(Control _parent)
   {
      this.SuspendLayout();

      this.Parent = _parent;

      CreateTheGrid();

      ThePCKSumGrid = CreateSumGrid(ThePCKGrid, ThePCKGrid.Parent, "SUM" + ThePCKGrid.Name);
      Initialize_SumGrid_Columns(ThePCKGrid);
      GridLocationAndSize_Grids(ThePCKGrid);

      CalcLocationAndSize();

      this.ResumeLayout();

      SetPKCColumnIndexes();

   }

   #endregion Constructor

   #region CalcLocationAndSize

   private void CalcLocationAndSize()
   {
      if(this.Parent is VvDialog)
      {
         this.Size = new Size(ThePCKGrid.Width + 2 * ZXC.QunMrgn, SystemInformation.WorkingArea.Height - 2*ZXC.Q10un);
         ThePCKGrid.Height = this.Size.Height - ThePCKSumGrid.Height - ZXC.Q2un;
      }
   }

   #endregion CalcLocationAndSize

   #region TheGrid

   private void CreateTheGrid()
   {
      ThePCKGrid          = new VvDataGridView();
      ThePCKGrid.Parent   = this;
      ThePCKGrid.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);

      ThePCKGrid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
      ThePCKGrid.AutoGenerateColumns                  = false;

      ThePCKGrid.RowHeadersBorderStyle = ThePCKGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
      ThePCKGrid.ColumnHeadersHeight   = ZXC.QUN - ZXC.Qun8;
      ThePCKGrid.RowTemplate.Height    = ZXC.QUN - ZXC.Qun8;
      ThePCKGrid.RowHeadersWidth       = ZXC.Q2un;
      ThePCKGrid.Height                = ThePCKGrid.ColumnHeadersHeight + ThePCKGrid.RowTemplate.Height;

      ThePCKGrid.CellFormatting += new DataGridViewCellFormattingEventHandler(VvDocumentRecordUC.grid_CellFormatting_FormatVvDateTime);
      ThePCKGrid.Validating += new System.ComponentModel.CancelEventHandler(VvDocumentRecordUC.grid_Validating);

      ThePCKGrid.ReadOnly = true;

      VvHamper.ApplyVVColorAndStyleTabCntrolChange(ThePCKGrid);
      VvHamper.Open_Close_Fields_ForWriting(ThePCKGrid, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);

      ThePCKGrid.AllowUserToAddRows       =
      ThePCKGrid.AllowUserToDeleteRows    =
      ThePCKGrid.AllowUserToOrderColumns  =
      ThePCKGrid.AllowUserToResizeColumns =
      ThePCKGrid.AllowUserToResizeRows    = false;

      ThePCKGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.PowderBlue;
      ThePCKGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.DarkSlateGray;
      ThePCKGrid.RowHeadersDefaultCellStyle.BackColor    = Color.PowderBlue; //Color.FloralWhite;
      ThePCKGrid.RowHeadersDefaultCellStyle.ForeColor    = Color.DarkSlateGray;

    //TheGrid.ColumnHeadersDefaultCellStyle.Font = ZXC.vvFont.BaseFont;
    //TheGrid.RowsDefaultCellStyle         .Font = ZXC.vvFont.BaseFont;
    //TheGrid.RowHeadersDefaultCellStyle   .Font = ZXC.vvFont.BaseFont;

      CreateColumn(ThePCKGrid);

      int sumoOfColumns = 0;
      foreach(DataGridViewColumn dc in ThePCKGrid.Columns)
      {
         sumoOfColumns += dc.Width;
      }

      ThePCKGrid.Width = sumoOfColumns + ThePCKGrid.RowHeadersWidth + ZXC.QUN + ZXC.Qun4;
      ThePCKGrid.Height = this.Size.Height - ZXC.QUN;
   }

   private VvDataGridView CreateSumGrid(VvDataGridView theGrid, Control _parent, string _name)
   {
      VvDataGridView theSumGrid = new VvDataGridView();

      theGrid.TheLinkedGrid_Sum = theSumGrid;

      theSumGrid.Parent = _parent;
      theSumGrid.Name = _name;

      theSumGrid.Height = ZXC.QUN + ZXC.Qun10; //23;

      theSumGrid.BorderStyle = BorderStyle.FixedSingle;
      VvHamper.ApplyVVColorAndStyleTabCntrolChange(theSumGrid);
      theSumGrid.ColumnHeadersVisible = false;
      theSumGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleVertical;// DataGridViewCellBorderStyle.None;
      theSumGrid.ReadOnly = true;
      theSumGrid.Tag = theGrid;

      VvHamper.ApplyVVColorAndStyleTabCntrolChange(theSumGrid);
      VvHamper.Open_Close_Fields_ForWriting(theSumGrid, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);


      theSumGrid.AllowUserToAddRows =
      theSumGrid.AllowUserToDeleteRows =
      theSumGrid.AllowUserToOrderColumns =
      theSumGrid.AllowUserToResizeColumns =
      theSumGrid.AllowUserToResizeRows = false;

      theSumGrid.RowHeadersDefaultCellStyle.Alignment = theGrid.RowHeadersDefaultCellStyle.Alignment;
      theSumGrid.RowTemplate.Height = theGrid.RowTemplate.Height;

      theSumGrid.ScrollBars = ScrollBars.None;

      //theSumGrid.Location = new Point(theGrid.Left, theGrid.Bottom);

      return theSumGrid;
   }

   private void GridLocationAndSize_Grids(VvDataGridView theGrid)
   {
      DataGridView theSumGrid = theGrid.TheLinkedGrid_Sum;

      theGrid.Height = this.Height - ZXC.Q2un - theSumGrid.Height;

      theSumGrid.Width = this.Width - 2 * ZXC.QunMrgn;
      theSumGrid.Location = new Point(theGrid.Location.X, theGrid.Bottom + ZXC.Qun12);
      theSumGrid.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

      theGrid.Scroll += new ScrollEventHandler(Grid_VScroll);
   }

   private void Grid_VScroll(object sender, ScrollEventArgs e)
   {
      VvDataGridView theGrid = sender as VvDataGridView;
      DataGridView theSumGrid = theGrid.TheLinkedGrid_Sum;

      theSumGrid.HorizontalScrollingOffset = theGrid.HorizontalScrollingOffset;

   }


   private void Initialize_SumGrid_Columns(VvDataGridView theGrid)
   {
      DataGridView theSumGrid = theGrid.TheLinkedGrid_Sum;

      DataGridViewTextBoxColumn gridSumColumn;

      foreach(DataGridViewColumn mainGridColumn in theGrid.Columns)
      {
         gridSumColumn = new DataGridViewTextBoxColumn();

         //gridSumColumn.Name                       = "SUM" + mainGridColumn.Name.TrimStart(new char[] { 't' });
         gridSumColumn.Name = mainGridColumn.Name;
         gridSumColumn.DefaultCellStyle.Alignment = mainGridColumn.DefaultCellStyle.Alignment;
         gridSumColumn.AutoSizeMode = mainGridColumn.AutoSizeMode;
         theSumGrid.AutoGenerateColumns = false;
         gridSumColumn.Width = mainGridColumn.Width;
         gridSumColumn.ValueType = mainGridColumn.ValueType;
         gridSumColumn.Visible = mainGridColumn.Visible;
         gridSumColumn.Tag = mainGridColumn.Tag;
         gridSumColumn.DefaultCellStyle.Format = mainGridColumn.DefaultCellStyle.Format;
         gridSumColumn.DefaultCellStyle.BackColor = mainGridColumn.DefaultCellStyle.BackColor;

         if(mainGridColumn.AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
         {
            gridSumColumn.MinimumWidth = mainGridColumn.MinimumWidth;
            gridSumColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

         }
         theSumGrid.Columns.Add(gridSumColumn);
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

   #endregion TheGrid

   #region TheGridColumn

   private void CreateColumn(VvDataGridView theGrid)
   {
      vvtb_PCK_ArtCD   = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (   "vvtb_PCK_ArtCD"  , null, -12, "Šifra"    ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_ArtCD   , null, "R_PCK_ArtCD"  , "Šifra"    , ZXC.Q6un); vvtb_PCK_ArtCD   .JAM_ReadOnly = true; 
      vvtb_PCK_ArtName = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (   "vvtb_PCK_ArtName", null, -12, "Naziv"    ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_ArtName , null, "R_PCK_ArtName", "Naziv"    , ZXC.Q3un); vvtb_PCK_ArtName .JAM_ReadOnly = true; colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; colVvText.MinimumWidth = ZXC.Q10un + ZXC.Qun5;
      vvtb_PCK_RAMkind = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (   "vvtb_PCK_RAMkind", null, -12, "RAM Klasa"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_RAMkind , null, "R_PCK_RAMkind", "Mem"      , ZXC.Q3un); vvtb_PCK_RAMkind .JAM_ReadOnly = true;
      vvtb_PCK_HDDkind = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (   "vvtb_PCK_HDDkind", null, -12, "HDD Klasa"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_HDDkind , null, "R_PCK_HDDkind", "Disk"     , ZXC.Q3un); vvtb_PCK_HDDkind .JAM_ReadOnly = true;
      vvtb_PCK_SklCD   = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (   "vvtb_PCK_SklCD"  , null, -12, "Skladište"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_SklCD   , null, "R_PCK_SklCD"  , "Sklad"    , ZXC.Q3un); vvtb_PCK_SklCD   .JAM_ReadOnly = true;
      vvtb_PCK_RAM     = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(0, "vvtb_PCK_RAM"    , null, -12, "RAM"      ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_RAM     , null, "R_PCK_RAM"    , "RAM"      , ZXC.Q3un); vvtb_PCK_RAM     .JAM_ReadOnly = true; colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;
      vvtb_PCK_HDD     = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(0, "vvtb_PCK_HDD"    , null, -12, "HDD"      ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_HDD     , null, "R_PCK_HDD"    , "HDD"      , ZXC.Q3un); vvtb_PCK_HDD     .JAM_ReadOnly = true; colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont;
    //vvtb_UkPstKol    = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb_UkPstKol"   , null, -12, "PstKol"   ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_UkPstKol    , null, "R_UkPstKol"   , "Pst"      , ZXC.Q4un); vvtb_UkPstKol    .JAM_ReadOnly = true; 
    //vvtb_UkUlazKol   = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb_UkUlazKol"  , null, -12, "UlazKol"  ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_UkUlazKol   , null, "R_UkUlazKol"  , "Ulaz"     , ZXC.Q4un); vvtb_UkUlazKol   .JAM_ReadOnly = true;
    //vvtb_UkIzlazKol  = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb_UkIzlazKol" , null, -12, "IzlazKol" ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_UkIzlazKol  , null, "R_UkIzlazKol" , "Izlaz"    , ZXC.Q4un); vvtb_UkIzlazKol  .JAM_ReadOnly = true;
      vvtb_StanjeKol   = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(2, "vvtb_StanjeKol"  , null, -12, "StanjeKol"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_StanjeKol   , null, "R_StanjeKol"  , "Stanje"   , ZXC.Q4un); vvtb_StanjeKol   .JAM_ReadOnly = true;

      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);
   }

   #endregion TheGridColumn

   #region SetColumnIndexes()

   private PCKInfo_colIdx ci;
   public PCKInfo_colIdx DgvCI { get { return ci; } }
   public struct PCKInfo_colIdx
   {
      internal int iT_PCK_ArtCD  ;
      internal int iT_PCK_ArtName;
      internal int iT_PCK_RAMkind;
      internal int iT_PCK_HDDkind;
      internal int iT_PCK_SklCD  ;
      internal int iT_PCK_RAM    ;
      internal int iT_PCK_HDD    ;
      internal int iT_UkPstKol   ;
      internal int iT_UkUlazKol  ;
      internal int iT_UkIzlazKol ;
      internal int iT_StanjeKol  ;
   }

   public void SetPKCColumnIndexes()
   {
      ci = new PCKInfo_colIdx();

      ci.iT_PCK_ArtCD   = ThePCKGrid.IdxForColumn("R_PCK_ArtCD"  );
      ci.iT_PCK_ArtName = ThePCKGrid.IdxForColumn("R_PCK_ArtName");
      ci.iT_PCK_RAMkind = ThePCKGrid.IdxForColumn("R_PCK_RAMkind");
      ci.iT_PCK_HDDkind = ThePCKGrid.IdxForColumn("R_PCK_HDDkind");
      ci.iT_PCK_SklCD   = ThePCKGrid.IdxForColumn("R_PCK_SklCD"  );
      ci.iT_PCK_RAM     = ThePCKGrid.IdxForColumn("R_PCK_RAM"    );
      ci.iT_PCK_HDD     = ThePCKGrid.IdxForColumn("R_PCK_HDD"    );
      ci.iT_UkPstKol    = ThePCKGrid.IdxForColumn("R_UkPstKol"   );
      ci.iT_UkUlazKol   = ThePCKGrid.IdxForColumn("R_UkUlazKol"  );
      ci.iT_UkIzlazKol  = ThePCKGrid.IdxForColumn("R_UkIzlazKol" );
      ci.iT_StanjeKol   = ThePCKGrid.IdxForColumn("R_StanjeKol"  );
   }

   #endregion SetColumnIndexes()

   public void PutDgvFields(List<PCK_InfoLine> PCK_Lines)
   {
      int rowIdx;

      ThePCKGrid.Rows.Clear();

      if(PCK_Lines != null)
      {
         for(rowIdx = 0; rowIdx < PCK_Lines.Count; ++rowIdx)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            ThePCKGrid.Rows.Add();

            PutDgvLineFields(rowIdx, PCK_Lines[rowIdx]);

            ThePCKGrid.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();
         }
         //PutDgvSumFields(PCK_Lines);
      }

      PutDgvSumFields(PCK_Lines);

   }

   private void PutDgvLineFields(int rowIdx, PCK_InfoLine PCK_Line)
   {
      ThePCKGrid.PutCell(ci.iT_PCK_ArtCD   , rowIdx, PCK_Line.PCK_ArtCD  );
      ThePCKGrid.PutCell(ci.iT_PCK_ArtName , rowIdx, PCK_Line.PCK_ArtName);
      ThePCKGrid.PutCell(ci.iT_PCK_RAMkind , rowIdx, PCK_Line.PCK_RAMkind);
      ThePCKGrid.PutCell(ci.iT_PCK_HDDkind , rowIdx, PCK_Line.PCK_HDDkind);
      ThePCKGrid.PutCell(ci.iT_PCK_SklCD   , rowIdx, PCK_Line.PCK_SklCD  );
      ThePCKGrid.PutCell(ci.iT_PCK_RAM     , rowIdx, PCK_Line.PCK_RAM    );
      ThePCKGrid.PutCell(ci.iT_PCK_HDD     , rowIdx, PCK_Line.PCK_HDD    );
    //TheGrid.PutCell(ci.iT_UkPstKol    , rowIdx, PCK_Line.UkPstKol   );
    //TheGrid.PutCell(ci.iT_UkUlazKol   , rowIdx, PCK_Line.UkUlazKol  );
    //TheGrid.PutCell(ci.iT_UkIzlazKol  , rowIdx, PCK_Line.UkIzlazKol );
      ThePCKGrid.PutCell(ci.iT_StanjeKol   , rowIdx, PCK_Line.StanjeKol  );
   }

   private void PutDgvSumFields(List<PCK_InfoLine> PCK_Lines)
   {
      ThePCKSumGrid.PutCell(ci.iT_PCK_RAM  , 0, PCK_Lines.Sum(pck => pck.PCK_RAM  ));
      ThePCKSumGrid.PutCell(ci.iT_PCK_HDD  , 0, PCK_Lines.Sum(pck => pck.PCK_HDD  ));
      ThePCKSumGrid.PutCell(ci.iT_StanjeKol, 0, PCK_Lines.Sum(pck => pck.StanjeKol));

   }
}