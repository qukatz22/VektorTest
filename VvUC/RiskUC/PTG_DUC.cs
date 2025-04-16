using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Reflection;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
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

   internal static string PCK_Signature_ToString(decimal RAM, decimal HDD)
   {
      return "RAM: " + RAM.ToString0Vv() + " HDD: " + HDD.ToString0Vv();
   }

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
      ThePolyGridTabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(IfShould_Load_PTG_DOD_Faktur_Grid);
      ThePolyGridTabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(IfShould_Load_PTG_DOD_Rtrans_Grid);
      ThePolyGridTabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(IfShould_Load_PTG_UgAn_i_DOD_Rtrans_Grid);
      ThePolyGridTabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(IfShould_Load_PTG_NajamStanje_Rtrans_Grid);
      ThePolyGridTabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(IfShould_Load_PTG_NajamStanje_Rtrano_Grid);
      ThePolyGridTabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(IfShould_Load_PTG_DOD_Rtrano_Grid);

      //ThePolyGridTabControl.TabPages["Osnovno"].Title = ptgOsn_TabPageName;

      CreateHamper_SumUGAN_PTG();

      hamp_DodAndKopCount.Location = new Point(TheG.Left, ZXC.Qun4);

      //16.12.2024. premjestila na FakturP ya ostale ptg duceve koji imaju rtrano
      //ThePolyGridTabControl.SelectionChanged += ThePolyGridTabControl_SelectionChanged_SupressSelectingDisabledTabs;
   }

   //private void ThePolyGridTabControl_SelectionChanged_SupressSelectingDisabledTabs(Crownwood.DotNetMagic.Controls.TabControl theTabControl, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   //{
   //   if(newPage.Enabled == false)
   //   { 
   //      theTabControl.SelectedIndex = theTabControl.TabPages.IndexOf(oldPage); // vrati ga nazad 
   //   }
   //}


   #endregion Constructor

   #region Fieldz

   public VvHamper hamp_TT_PTG, hamp_v1TT_PTG, hamp_v2TT_PTG, hamp_partner_PTG, hamp_ugovor_PTG, hamp_zaFaktur_PTG, 
                   hamp_ugovorCbx_PTG, hamp_KUGpartner_PTG, hamp_kontakt_PTG, hamp_napomena_PTG,
                   hamp_sklad_PTG, hamp_R_iznosi_PTG, hamp_R_DodKop_PTG, hamp_datumi_PTG, hamp_mjIsporuke_PTG,
                   hamp_potpisan_PTG, hamp_tekstZaRacun_PTG, hamp_posredProviz_PTG, hamp_DodAndKopCount/*, hamp_otPlanRows*/,
                   hamp_semafor;

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


   public  Label     lbl_serNoOk;

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
      InitializeHamper_Semafor           (out hamp_semafor);

      CreateArrOfHampers();

      SetParentOfHamperLeftHampers();

      panel_MigratorsLeftB.SendToBack();
           
      hamp_partner_PTG   .Location = new Point(                     0 ,        0);
      hamp_mjIsporuke_PTG.Location = new Point(hamp_partner_PTG.Left, hamp_partner_PTG.Bottom - hamp_mjIsporuke_PTG.Height);
      hamp_mjIsporuke_PTG.BringToFront();

      hamp_datumi_PTG    .Location = new Point(hamp_partner_PTG .Left , hamp_partner_PTG.Bottom);
      hamp_TT_PTG        .Location = new Point(hamp_partner_PTG .Right,                        0);

      hamp_ugovor_PTG    .Location = new Point(hamp_TT_PTG       .Left, hamp_TT_PTG    .Bottom);
      hamp_zaFaktur_PTG  .Location = new Point(hamp_ugovor_PTG   .Left, hamp_ugovor_PTG  .Bottom);
      hamp_ugovorCbx_PTG .Location = new Point(hamp_datumi_PTG    .Right, hamp_datumi_PTG .Top);
      hamp_napomena_PTG  .Location = new Point(hamp_ugovorCbx_PTG.Right, hamp_partner_PTG.Bottom);

      hamp_R_DodKop_PTG  .Location = new Point(hamp_TT_PTG.Right, hamp_TT_PTG.Top);
      hamp_R_DodKop_PTG.BringToFront();

      hamp_potpisan_PTG.Location = new Point(hamp_R_DodKop_PTG.Right, hamp_R_DodKop_PTG.Top);
      
      hamp_semafor.Location = new Point(hamp_potpisan_PTG.Right - hamp_semafor.Width, hamp_potpisan_PTG.Bottom);

      hamp_tekstZaRacun_PTG.Location = new Point(hamp_napomena_PTG.Right, hamp_napomena_PTG.Bottom - hamp_tekstZaRacun_PTG.Height);
      hamp_posredProviz_PTG.Location = new Point(hamp_tekstZaRacun_PTG.Right - hamp_posredProviz_PTG.Width, hamp_zaFaktur_PTG.Bottom);

      hamp_kontakt_PTG.Location = new Point(hamp_potpisan_PTG.Right - hamp_kontakt_PTG.Width, hamp_zaFaktur_PTG.Top);

      hamp_sklad_PTG.Location = new Point(hamp_potpisan_PTG.Right - hamp_sklad_PTG.Width, hamp_kontakt_PTG.Bottom + ZXC.Qun2);

      nextY = hamp_napomena_PTG.Bottom;

      hamp_twin.Visible = false;

      InitializeHamper_KOPandDODcount(out hamp_DodAndKopCount);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_partner_PTG, hamp_mjIsporuke_PTG, hamp_TT_PTG, hamp_dokNum, //hamp_v1TT_PTG, hamp_v2TT_PTG,
                                    hamp_ugovor_PTG, hamp_datumi_PTG, hamp_zaFaktur_PTG, hamp_ugovorCbx_PTG, 
                                    /*hamp_KUGpartner_PTG,*/ hamp_kontakt_PTG, hamp_napomena_PTG,
                                    hamp_sklad_PTG,/* hamp_R_iznosi_PTG,*/ hamp_R_DodKop_PTG, hamp_potpisan_PTG, hamp_semafor, hamp_posredProviz_PTG, hamp_tekstZaRacun_PTG
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
      tbx_TtNum.JAM_ReadOnly    = true;

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
      tbx_SkladOpis = hamper.CreateVvTextBox      (2, 0, "tbx_SkladOpis", "");
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
      tbx_Sklad2Cd  = hamper.CreateVvTextBoxLookUp (1, 1, "tbx_Sklad2Cd", "Skladište povrata iz najma");
      tbx_Sklad2Opis = hamper.CreateVvTextBox      (2, 1, "tbx_SkladOpis2", "");
      tbx_SkladRbr2 = hamper.CreateVvTextBox       (3, 1, "tbx_SkladRbr2", "");
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

   private void InitializeHamper_Semafor(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", null, false);
      
      hamper.VvColWdt     = new int[] { ZXC.Q4un      };
      hamper.VvSpcBefCol  = new int[] { faBefFirstCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN + ZXC.Qun8};
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      lbl_serNoOk =  hamper.CreateVvLabel(0, 0, "", ContentAlignment.MiddleCenter);
      lbl_serNoOk.BackColor = Color.Green;
      lbl_serNoOk.ForeColor = Color.Yellow;

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
      T_skladCD_CreateColumn       (ZXC.Q3un,               true, "IzlSkl"  , "Izlazno skladište");
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

   #region TheG2_Specific_Columns2
   protected override void InitializeDUC_Specific_Columns2()
   {
      bool isVisible = true;

      T_serno_CreateColumn          (ZXC.Q8un,    isVisible, "Serijski broj", "Serijski broj artikla"             );
      T_artiklCD2_CreateColumn      (ZXC.Q5un,    isVisible, "Šifra"        , "Šifra artikla"                     );
      T_artiklName2_CreateColumnFill(             isVisible, "Naziv"        , "Naziv artikla ili proizvoljan opis");
      R_artiklTS_CreateColumn       (ZXC.Q3un - ZXC.Qun2,    isVisible, "Tip"          , "Tip artikla"            );
      R_ramKlasa2_CreateColumn      (ZXC.Q3un,    isVisible, "RAM klasa"    , "RAM klasa"                         );
      R_hddKlasa2_CreateColumn      (ZXC.Q3un,    isVisible, "HDD klasa"    , "RAM klasa"                         );
      R_skladCD1_CreateColumn       (ZXC.Q3un,    isVisible, "IzlSkl"       , "Izlazno skladište"                 );
      T_skladCD2_CreateColumn       (ZXC.Q3un,    isVisible, "UlzSkl"       , "Ulazno skladište"                  );
      T_dimZ_CreateColumn           (ZXC.Q3un, 0, isVisible, "RAM"          , "RAM"                               );
      T_decC_CreateColumn           (ZXC.Q3un, 0, isVisible, "HDD"          , "HDD old"                           );
      T_grCD_CreateColumn           (ZXC.Q5un,    isVisible, "Opis"         , "Opis"                       , false);
      T_paletaNo_CreateColumn       (ZXC.Q3un,    isVisible, "UgnSt"        , "UGANDO stavka"                     );
   }

   #endregion TheG2_Specific_Columns2

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_UGAN_PTG, clr_UPA, clr_UGAN_PTG);
      ThePolyGridTabControl.TabPages[ptgOpl_TabPageName].Tag = clr_OPL_PTG;
      ThePolyGridTabControl.TabPages[ptgDodRtrans_TabPageName].Tag = clr_DOD_PTG;
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

   //public override void OpenCloseForWriting_AdditionalAction_UCspecific(ZXC.WriteMode writeMode, bool isESC)
   //{
   //   bool idemoUzuto   = writeMode != ZXC.WriteMode.None;
   //   bool idemoUbijelo = !idemoUzuto                    ;

   //   bool isRtranO_zuto = idemoUzuto && ZXC.RISK_Edit_RtranoOnly_InProgress;
   //   bool isRtranS_zuto = idemoUzuto && isRtranO_zuto == false;

   //   int rtranStabIdx = 0;
   //   int rtranOtabIdx = 1;

   //   if(isRtranS_zuto) ThePolyGridTabControl.SelectedIndex = rtranStabIdx;
   //   if(isRtranO_zuto) ThePolyGridTabControl.SelectedIndex = rtranOtabIdx;

   //   for(int i = 0; i < ThePolyGridTabControl.TabPages.Count; ++i)
   //   {
   //      if(idemoUbijelo) ThePolyGridTabControl.TabPages[i].Enabled = true;
   //      else // idemoUzuto 
   //      {
   //              if(i == rtranStabIdx && isRtranS_zuto) ThePolyGridTabControl.TabPages[i].Enabled = true ;
   //         else if(i == rtranOtabIdx && isRtranO_zuto) ThePolyGridTabControl.TabPages[i].Enabled = true ;
   //         else                                        ThePolyGridTabControl.TabPages[i].Enabled = false;
   //      }
   //   }

   //   if(isRtranO_zuto)
   //   {
   //      foreach(VvHamper hamper in hamperLeft)
   //      {
   //         if(hamper.IsDUMMY) continue;
   //         VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvRecordUC);
   //      }

   //      VvHamper.Open_Close_Fields_ForWriting(tbx_opaskaServisa_PTG, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvRecordUC);
   //   }
   //   else
   //   {
   //      VvHamper.Open_Close_Fields_ForWriting(tbx_opaskaServisa_PTG, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvRecordUC);
   //   }

   //   /*rtzrtz*/
   //   if(isRtranS_zuto)
   //   {
   //      this.tbx_KupdobName.Select();
   //   }

   //   if(isRtranO_zuto)
   //   {
   //      this.TheG2.Select();

   //   }

   //} // public override void OpenCloseForWriting_AdditionalAction_UCspecific(ZXC.WriteMode writeMode, bool isESC) 

   public override bool IsPTG_DUC_wRtrano { get { return true; } }

   public bool GetLastFakturForThisSerno(XSqlConnection conn, Faktur faktur_rec, string theSerno/*, bool shouldBeSilent*/)
   {
      Rtrano last_rtrano_rec_forThisSerno = new Rtrano();

      bool isLastRtrano_ForSerno_found = RtranoDao.Get_LastRtrano_ForSerno(conn, last_rtrano_rec_forThisSerno, theSerno, true);

      if(isLastRtrano_ForSerno_found == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Nepoznat serijski broj?!");
         return false;
      }

      (string UgAn_TT, uint UgAn_TtNum) = Get_UgAnFaktur_TtAndTtNum_ForThisRtranoTtAndTtNum(last_rtrano_rec_forThisSerno);

      if(UgAn_TT.IsEmpty() || UgAn_TtNum.IsZero()) return false;

      FakturDao.SetMeFaktur(conn, faktur_rec, UgAn_TT, UgAn_TtNum, false);

      return true;
   }

   internal static (string UgAn_TT, uint UgAn_TtNum) Get_UgAnFaktur_TtAndTtNum_ForThisRtranoTtAndTtNum(Rtrano last_rtrano_rec_forThisSerno)
   {
      string theSerno          = last_rtrano_rec_forThisSerno.T_serno;
      string last_rtrano_TT    = last_rtrano_rec_forThisSerno.T_TT   ;
      uint   last_rtrano_TtNum = last_rtrano_rec_forThisSerno.T_ttNum;

      string UgAnFaktur_TT = "";
      uint   UgAn_TtNum   ;

      TtInfo last_rtrano_ttInfo = ZXC.TtInfo(last_rtrano_TT);

      if(last_rtrano_ttInfo.IsPTGTwinRtrans_UgAnDodTT == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Serijski broj\n\r\n\r{0}\n\r\n\rnije ni pod kojim ugovorom / aneksom.\n\r\n\rZadnja pojava mu je na skladištu {1}\n\r\n\r{2}", 
            theSerno, last_rtrano_rec_forThisSerno.T_skladCD, last_rtrano_rec_forThisSerno);

         return ("", 0);
      }

      // Trazimo TT Faktur dokumenta na osnovi Rtrano ili Rtrans TT-a 

           if(last_rtrano_ttInfo.IsPTGFaktur_UgAnTT    ) { UgAnFaktur_TT = last_rtrano_TT;                     UgAn_TtNum = last_rtrano_TtNum; } // UGN, AUN ... ostavi nepromijenjeno 
      else if(last_rtrano_ttInfo.IsPTGTwinRtrans_UgAnTT) { UgAnFaktur_TT = last_rtrano_ttInfo.LinkedDefaultTT; UgAn_TtNum = last_rtrano_TtNum; } // UG2, AU2 ... ostavi nepromijenjeno 

      else // DODACI (DIZ, PVR, ZIZ) 
      {
         bool isUGN = last_rtrano_TtNum < 100000000;
         bool isAUN = !isUGN;

         if(isUGN) UgAnFaktur_TT = Faktur.TT_UGN;
         if(isAUN) UgAnFaktur_TT = Faktur.TT_AUN;

         UgAn_TtNum = last_rtrano_TtNum / 1000;
      }

      return (UgAnFaktur_TT, UgAn_TtNum);
   }

   internal bool Getfirst_UgAn_rec_forThis_KupdobAndArtikl(XSqlConnection conn, Faktur first_UgAnFaktur_rec_forThis_KupdobAndArtikl, uint theWantedKupdobCD, string theWantedArtiklCD)
   {
      bool uganFakturFound = RtransDao.Getfirst_UgAn_rec_forThis_KupdobAndArtikl(conn, first_UgAnFaktur_rec_forThis_KupdobAndArtikl, theWantedKupdobCD, theWantedArtiklCD);

      return uganFakturFound;
   }
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

public class DIZ_PTG_DUC : FakturPDUC //FakturExtDUC
{
   #region Constructor

   public DIZ_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[]
         {
            Faktur.TT_DIZ
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

      hamp_twin.Visible = false;
      hamp_IznosUvaluti.Visible = false;

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

                    hamper.CreateVvLabel  (0, 0, "Datum:", ContentAlignment.MiddleRight);
      tbx_DokDate = hamper.CreateVvTextBox(1, 0, "tbx_dokDate", "Datum ugovora");
      tbx_DokDate.JAM_IsForDateTimePicker = true;
      dtp_DokDate = hamper.CreateVvDateTimePicker(1, 0, "", tbx_DokDate);
      dtp_DokDate.Name = "dtp_DokDate";
      tbx_DokDate.Font = ZXC.vvFont.BaseBoldFont;
      dtp_DokDate.ValueChanged += new EventHandler(dtp_DokDate_ValueChanged_SetSkladAndPdvDate);
      dtp_DokDate.Leave += new EventHandler(ValidateDOD_dokDate);

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

      hamper.VvColWdt      = new int[] { labelWidth + ZXC.Q2un, ZXC.Q3un - ZXC.Qun2, ZXC.Q5un, ZXC.Q2un - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { faBefFirstCol, faBefCol, faBefCol, faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel        (0, 0, "Izdajemo sa:", ContentAlignment.MiddleRight);
      tbx_SkladCd   = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_SkladCd", "Skladište u najam");
      tbx_SkladOpis = hamper.CreateVvTextBox      (2, 0, "tbx_SkladOpis", "");
      tbx_SkladBR   = hamper.CreateVvTextBox      (3, 0, "tbx_SkladRbr", "");
      tbx_SkladCd.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_SkladCd.JAM_DataRequired = true;
      tbx_SkladCd.JAM_MustTabOutBeforeSubmit = true;

      tbx_SkladCd  .JAM_ReadOnly = true;
      tbx_SkladOpis.JAM_ReadOnly = true;
      tbx_SkladBR  .JAM_ReadOnly = true;

      tbx_SkladCd.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_SkladCd.JAM_lui_NameTaker_JAM_Name    = tbx_SkladOpis.JAM_Name;
      tbx_SkladCd.JAM_lui_IntegerTaker_JAM_Name = tbx_SkladBR.JAM_Name;
      tbx_SkladCd.VVtag2 = true;

                      hamper.CreateVvLabel         (0, 1, "Ulazi na:", ContentAlignment.MiddleRight);
      tbx_Sklad2Cd  = hamper.CreateVvTextBoxLookUp (1, 1, "tbx_Sklad2Cd", "Skladište povrata iz najma");
      tbx_Sklad2Opis = hamper.CreateVvTextBox      (2, 1, "tbx_SkladOpis2", "");
      tbx_SkladRbr2 = hamper.CreateVvTextBox       (3, 1, "tbx_SkladRbr2", "");
      tbx_Sklad2Cd.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_Sklad2Cd.JAM_DataRequired = true;
      tbx_Sklad2Cd.JAM_MustTabOutBeforeSubmit = true;

      tbx_Sklad2Cd  .JAM_ReadOnly = true;
      tbx_Sklad2Opis.JAM_ReadOnly = true;
      tbx_SkladRbr2.JAM_ReadOnly = true;

      tbx_Sklad2Cd.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_Sklad2Cd.JAM_lui_NameTaker_JAM_Name    = tbx_Sklad2Opis.JAM_Name;
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
      T_artiklTS_CreateColumn      (ZXC.Q2un,               true, "Tip"     , "Tip artikla");
      T_doCijMal_CreateColumn      (ZXC.Q3un, 0,            true, "RAM"     , "RAM", false);
      T_noCijMal_CreateColumn      (ZXC.Q3un, 0,            true, "HDD"     , "HDD");
      T_skladCD_CreateColumn       (ZXC.Q3un,               true, "IzlSk"   , "Izlazno skladište");
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

      T_serno_CreateColumn          (ZXC.Q8un,    isVisible, "Serijski broj", "Serijski broj artikla"             );
      T_artiklCD2_CreateColumn      (ZXC.Q5un,    isVisible, "Šifra"        , "Šifra artikla"                     );
      T_artiklName2_CreateColumnFill(             isVisible, "Naziv"        , "Naziv artikla ili proizvoljan opis");
      R_artiklTS_CreateColumn       (ZXC.Q3un - ZXC.Qun2,    isVisible, "Tip"          , "Tip artikla");
      R_ramKlasa2_CreateColumn      (ZXC.Q3un,    isVisible, "RAM klasa"    , "RAM klasa");
      R_hddKlasa2_CreateColumn      (ZXC.Q3un,    isVisible, "HDD klasa"    , "RAM klasa");
      R_skladCD1_CreateColumn       (ZXC.Q3un,    isVisible, "IzlSkl"       , "Izlazno skladište");
      T_skladCD2_CreateColumn       (ZXC.Q3un,    isVisible, "UlzSkl"       , "Ulazno skladište"                 );
      T_dimZ_CreateColumn           (ZXC.Q3un, 0, isVisible, "RAM"          , "RAM"                           );
      T_decC_CreateColumn           (ZXC.Q3un, 0, isVisible, "HDD"          , "HDD old"                           );
      T_grCD_CreateColumn           (ZXC.Q5un,    isVisible, "Opis"         , "Opis"                       , false);
      T_paletaNo_CreateColumn       (ZXC.Q3un,    isVisible, "DodSt"        , "DOD stavka"                     );
   }

   #endregion TheG_Specific_Columns2

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_DOD_PTG, Color.Empty, clr_DOD_PTG);
   }
   public override bool IsPTG_DUC_wRtrano { get { return true; } }

}

public class PVR_PTG_DUC : FakturPDUC //FakturExtDUC
{
   #region Constructor

   public PVR_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[]
         {
            Faktur.TT_PVR
         });

      ThePolyGridTabControl.SelectedIndex = 1;

      ThePolyGridTabControl.SelectionChanged += ThePolyGridTabControl_SelectionChanged_SupressSelectingDisabledTabs;

      TheSumGrid.Visible = true;

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

   public VvHamper hamp_partner_PTG, hamp_TT_PTG, hamp_v1TT_PTG, hamp_v2TT_PTG, hamp_KUGpartner_PTG, hamp_PvrNum_PTG,
                   hamp_napomena_PTG, hamp_sklad_PTG;
   public RadioButton rbt_mjIsp_Korisnik, rbt_mjIsp_PcToGo, rbt_mjIsp_KorisOvers;

   #endregion Fieldz
   
   #region Hampers

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      InitializeHamper_Partner_PTG   (out hamp_partner_PTG);
      InitializeHamper_TT_PTG        (out hamp_TT_PTG        );
      InitializeHamper_v1TT_PTG      (out hamp_v1TT_PTG      );
      InitializeHamper_v2TT_PTG      (out hamp_v2TT_PTG      );
      InitializeHamper_PVRnum_PTG    (out hamp_PvrNum_PTG    );
      InitializeHamper_KUGpartner_PTG(out hamp_KUGpartner_PTG);
      InitializeHamper_Datum_PTG     (out hamp_dokDate   );
      InitializeHamper_Napomena_PTG  (out hamp_napomena_PTG  );
      InitializeHamper_Skladista_PTG (out hamp_sklad_PTG     );
      CreateArrOfHampers();

      SetParentOfHamperLeftHampers();

      panel_MigratorsLeftB.SendToBack();

      hamp_partner_PTG.Location = new Point(0, 0);
      hamp_TT_PTG     .Location = new Point(hamp_partner_PTG.Right, 0);

      int hamp_PTG_PvrNumLeft = hamp_TT_PTG.Right - hamp_PvrNum_PTG.Width;
      int hamp_v2TT_PTGLeft   = hamp_TT_PTG.Right - hamp_PvrNum_PTG.Width - hamp_v2TT_PTG.Width;
      int hamp_v1TT_PTGLeft   = hamp_TT_PTG.Right - hamp_PvrNum_PTG.Width - hamp_v2TT_PTG.Width - hamp_v1TT_PTG.Width;

      hamp_v2TT_PTG.Location   = new Point(hamp_v2TT_PTGLeft  , hamp_TT_PTG.Bottom);
      hamp_v1TT_PTG.Location   = new Point(hamp_v1TT_PTGLeft  , hamp_TT_PTG.Bottom);
      hamp_PvrNum_PTG.Location = new Point(hamp_PTG_PvrNumLeft, hamp_TT_PTG.Bottom);

      hamp_dokNum .Location = new Point(hamp_TT_PTG.Right, hamp_TT_PTG.Top );
      hamp_dokDate.Location = new Point(hamp_TT_PTG.Left , hamp_v1TT_PTG.Bottom );

      hamp_KUGpartner_PTG.Location = new Point(hamp_TT_PTG.Left, hamp_TT_PTG.Bottom);

      hamp_napomena_PTG.Location = new Point(hamp_partner_PTG.Left, hamp_partner_PTG.Bottom);

      hamp_sklad_PTG.Location = new Point(hamp_TT_PTG.Left + ZXC.Q2un, hamp_partner_PTG.Bottom);

      nextY = hamp_napomena_PTG.Bottom;

      hamp_twin.Visible = false;
      hamp_IznosUvaluti.Visible = false;

   }

   private void CreateArrOfHampers()
   {

      hamperLeft = new VvHamper[] { hamp_TT_PTG, hamp_partner_PTG, hamp_dokDate, hamp_dokNum,
                                    hamp_v1TT_PTG, hamp_v2TT_PTG, hamp_PvrNum_PTG, hamp_KUGpartner_PTG,
                                    hamp_napomena_PTG, hamp_sklad_PTG
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

   private void InitializeHamper_PVRnum_PTG(out VvHamper hamper)
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

   private void InitializeHamper_Datum_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth + ZXC.QUN, ZXC.Q4un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { faBefCol  ,            faBefCol,         };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
      hamper.VvBottomMargin = ZXC.Qun8;

                    hamper.CreateVvLabel  (0, 0, "Datum:", ContentAlignment.MiddleRight);
      tbx_DokDate = hamper.CreateVvTextBox(1, 0, "tbx_dokDate", "Datum ugovora");
      tbx_DokDate.JAM_IsForDateTimePicker = true;
      dtp_DokDate = hamper.CreateVvDateTimePicker(1, 0, "", tbx_DokDate);//PTG_DatUgovora     
      dtp_DokDate.Name = "dtp_DokDate";
      dtp_DokDate.ValueChanged += new EventHandler(dtp_DokDate_ValueChanged_SetSkladAndPdvDate);
      dtp_DokDate.Leave += new EventHandler(ValidateDOD_dokDate);

      tbx_DokDate.Font = ZXC.vvFont.BaseBoldFont;
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

      hamper.VvColWdt      = new int[] { labelWidth + ZXC.Q2un, ZXC.Q3un - ZXC.Qun2, ZXC.Q5un, ZXC.Q2un - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] {         faBefFirstCol, faBefCol, faBefCol, faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel        (0, 0, "Vraća se sa:", ContentAlignment.MiddleRight);
      tbx_SkladCd   = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_SkladCd", "Skladište u najam");
      tbx_SkladOpis = hamper.CreateVvTextBox      (2, 0, "tbx_SkladOpis", "");
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

                      hamper.CreateVvLabel         (0, 1, "Ulazi na:", ContentAlignment.MiddleRight);
      tbx_Sklad2Cd  = hamper.CreateVvTextBoxLookUp (1, 1, "tbx_Sklad2Cd", "Skladište povrata iz najma");
      tbx_Sklad2Opis = hamper.CreateVvTextBox      (2, 1, "tbx_SkladOpis2", "");
      tbx_SkladRbr2 = hamper.CreateVvTextBox       (3, 1, "tbx_SkladRbr2", "");
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

   #endregion Hampers

   #region Fld

   public DateTime Fld_PTG_DatDostave { get { return dtp_DokDate2.Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_DokDate2.Value = value; } } }


   #endregion Fld

   #region TheG_Specific_Columns
   public override bool HasRtrans_SkladCD_Exposed { get { return true; } }
   public override bool HasRtrano_SkladCD_Exposed { get { return true; } }
   public override bool Is_Rtrans_Readonly        { get { return true; } }

   protected override void InitializeDUC_Specific_Columns()
   {
      T_artiklCD_CreateColumn      (ZXC.Q3un           ,    true, "Šifra"   , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                        true, "Naziv"   , "Naziv artikla ili proizvoljan opis");
      T_artiklTS_CreateColumn      (ZXC.Q2un,               true, "Tip"     , "Tip artikla");
      T_doCijMal_CreateColumn      (ZXC.Q3un, 0,            true, "RAM"     , "RAM", false);
      T_noCijMal_CreateColumn      (ZXC.Q3un, 0,            true, "HDD"     , "HDD");
      T_skladCD_CreateColumn       (ZXC.Q3un,               true, "IzlSkl"  , "Izlazno skladište");
      R_skladCd2_CreateColumn      (ZXC.Q3un,               true, "UlzSkl"  , "Ulaz na skladišta");
      T_jedMj_CreateColumn         (ZXC.Q2un           ,    true, "JM"      , "Jedinica mjere"                    );
      T_kol_CreateColumn           (ZXC.Q3un           , 2, true, "Kol"     , "Količina"                );
      T_cij_CreateColumn           (ZXC.Q4un           , 4, true, "Cijena"  , "Jedinična cijena"                  );
      //T_rbt1St_CreateColumn        (ZXC.Q3un - ZXC.Qun4, 2, true, "Rbt"     , "Stopa rabata");
      //R_cij_kcr_CreateColumn       (ZXC.Q4un           , 2, true, "CijSRbt" , "Cijena s rabatom");
      //R_rbt1_CreateColumn          (ZXC.Q4un           , 2, true, "IznosRbt", "Iznos rabata");
      R_KCR_CreateColumn           (ZXC.Q4un,            2, true, "Iznos"   , "Ukupan iznos bez PDV-a");
   }

   #endregion TheG_Specific_Columns

   #region TheG_Specific_Columns2

   protected override void InitializeDUC_Specific_Columns2()
   {
      bool isVisible = true;
      T_selection_CreateColumn      (ZXC.Q2un,    false    , "", "");
      T_serno_CreateColumn          (ZXC.Q8un,    isVisible, "Serijski broj", "Serijski broj artikla"             );
      T_artiklCD2_CreateColumn      (ZXC.Q5un,    isVisible, "Šifra"        , "Šifra artikla"                     );
      T_artiklName2_CreateColumnFill(             isVisible, "Naziv"        , "Naziv artikla ili proizvoljan opis");
      R_artiklTS_CreateColumn       (ZXC.Q3un - ZXC.Qun2,    isVisible, "Tip"          , "Tip artikla");
      R_ramKlasa2_CreateColumn      (ZXC.Q3un,    isVisible, "RAM klasa"    , "RAM klasa");
      R_hddKlasa2_CreateColumn      (ZXC.Q3un,    isVisible, "HDD klasa"    , "RAM klasa");
      R_skladCD1_CreateColumn       (ZXC.Q3un,    isVisible, "IzlSkl"       , "Izlazno skladište");
      T_skladCD2_CreateColumn       (ZXC.Q3un,    isVisible, "UlzSkl"       , "Ulazno skladište"                 );
      T_dimZ_CreateColumn           (ZXC.Q3un, 0, isVisible, "RAM"          , "RAM"                           );
      T_decC_CreateColumn           (ZXC.Q3un, 0, isVisible, "HDD"          , "HDD old"                           );
      T_grCD_CreateColumn           (ZXC.Q5un,    isVisible, "Opis"         , "Opis"                       , false);
      T_paletaNo_CreateColumn       (ZXC.Q3un,    false    , "PvrSt"        , "PVR stavka"                     );
      T_rtrRecID_CreateColumn       (ZXC.Q3un,    false    , "RtrRecID"     , "RtrRecID"                       );
   }

   #endregion TheG_Specific_Columns2

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_PVR_PTG, Color.Empty, clr_PVR_PTG);
   }
   public override bool IsPTG_DUC_wRtrano { get { return true; } }

   public override void OpenCloseForWriting_AdditionalAction_UCspecific(ZXC.WriteMode writeMode, bool isESC)
   {
      bool idemoUzuto   = writeMode != ZXC.WriteMode.None;
      bool idemoUbijelo = !idemoUzuto                    ;

      int rtranOtabIdx = 1;

      if(idemoUzuto) ThePolyGridTabControl.SelectedIndex = rtranOtabIdx;

      for(int i = 0; i < ThePolyGridTabControl.TabPages.Count; ++i)
      {
         if(idemoUbijelo)
         { 
            ThePolyGridTabControl.TabPages[i].Enabled = true;
            colCboxClassic.Visible = false;
         
         }
         else // idemoUzuto 
         {
            if(i == rtranOtabIdx) ThePolyGridTabControl.TabPages[i].Enabled = true ;
            else                  ThePolyGridTabControl.TabPages[i].Enabled = false;

            colCboxClassic.Visible = true;
         }
      }
   } // public override void OpenCloseForWriting_AdditionalAction_UCspecific(ZXC.WriteMode writeMode, bool isESC) 

   internal void SintRtranoToRtransOnPVR(VvForm theVvForm) // On <SAVE> 
   {
      foreach(VvTransRecord pvrRtrans_rec in faktur_rec.Transes)
      {
         pvrRtrans_rec.VvDao.DELREC(TheDbConnection, pvrRtrans_rec, false);

         VvTransRecord twinTrans_rec2 = VvDaoBase.SetTwinTransRec2(TheDbConnection, faktur_rec, pvrRtrans_rec, ZXC.WriteMode.Delete);

         ZXC.InAddTwinIzlazParentId = (twinTrans_rec2 as VvTransRecord).VirtualParentRecID;

         twinTrans_rec2.VvDao.DELREC(TheDbConnection, twinTrans_rec2, false, false);

         pvrRtrans_rec.VvDao.Delete_Then_Renew_Cache_FromThisRtrans(TheDbConnection, pvrRtrans_rec, VvSQL.DB_RW_ActionType.DEL);
      }

      faktur_rec.InvokeTransClear();

      theVvForm.BeginEdit(faktur_rec);

      #region ADDREC_PVR_Rtrans_From_PVR_Rtrano

      Rtrans PVR_rtrans_rec     = null; // new rtrans                        
      Rtrans UgAnDod_rtrans_rec = null; // rtrans koji nas je poslao u najam 

      bool UgAnDod_rtrans_rec_Found;

      ushort t_serial = 0;

      foreach(Rtrano rtrano_rec in faktur_rec.Transes2)
      {
         #region Init, Get theCij

         Artikl  artikl_rec   ;
         string  t_jedMj      ;
         decimal theCij   = 0M;

         artikl_rec = ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == rtrano_rec.T_artiklCD);

         if(artikl_rec != null) t_jedMj = artikl_rec.JedMj;
         else                   t_jedMj = ""              ;

         UgAnDod_rtrans_rec = new Rtrans();

         UgAnDod_rtrans_rec_Found = UgAnDod_rtrans_rec.VvDao.SetMe_Record_byRecID(TheDbConnection, UgAnDod_rtrans_rec, rtrano_rec.T_rtrRecID, false, false);
       //UgAnDod_rtrans_rec = Get_UgAnDod_rtrans_4PVR(TheDbConnection, rtrano_rec, this.UGAN_ttNum_ofThisDUC);

         theCij = UgAnDod_rtrans_rec.T_cij;

         if(theCij.IsZero()) 
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "Nema UGN cijene za artikl\n\r\n\r[{0}]", artikl_rec);
         }

         #endregion Init

         rtrano_rec.T_kol = 1.00M; // !!! ovoga nema na MOD varijanti 

         PVR_rtrans_rec = new Rtrans(Faktur.TT_PVR, rtrano_rec, t_jedMj, ++t_serial);

       //PVR_rtrans_rec.T_twinID = rtrano_rec.T_recID; // Link it!                       
         PVR_rtrans_rec.T_cij    = theCij            ; // this is what we are living for 

         if(artikl_rec != null) PVR_rtrans_rec.T_artiklName = artikl_rec.ArtiklName;
         
         PVR_rtrans_rec.R_utilString = PVR_rtrans_rec.T_skladCD;
         PVR_rtrans_rec.T_skladCD    = ZXC.PTG_UNJ;

         Add_or_Sint_RtransToFakturTransesCollection(faktur_rec, PVR_rtrans_rec);
      }

      #endregion ADDREC_PVR_Rtrans_From_PVR_Rtrano

      faktur_rec.TakeTransesSumToDokumentSum(true);

    //PutFields(faktur_rec); 
    //theVvForm.SintSameArtiklRows(this, EventArgs.Empty);

      bool OK = theVvForm.TheVvDao.RWTREC(TheDbConnection, faktur_rec);

      if(!OK) { theVvForm.CancelEdit(faktur_rec); return; }

      theVvForm.EndEdit(faktur_rec);

      theVvForm.PutFieldsActions(TheDbConnection, faktur_rec, this);
   }
   private void Add_or_Sint_RtransToFakturTransesCollection(Faktur _faktur_rec, Rtrans _rtrans_rec)
   {
      Rtrans existingRtrans_rec = _faktur_rec.Transes.SingleOrDefault(rtr => rtr.T_artiklCD == _rtrans_rec.T_artiklCD && rtr.T_cij == _rtrans_rec.T_cij);
      
      bool isToSint = existingRtrans_rec != null;
      if(isToSint)
      {
         existingRtrans_rec.T_kol += _rtrans_rec.T_kol;

         existingRtrans_rec.CalcTransResults(null);
      }
      else // first occurence of this T_artiklCD & T_cij 
      {
         _rtrans_rec.CalcTransResults(null);
         _rtrans_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;
         _faktur_rec.Transes.Add(_rtrans_rec);
      }
   }

}

public class ZIZ_PTG_DUC : FakturPDUC 
{
   #region Constructor

   public ZIZ_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[]
         {
            Faktur.TT_ZIZ
         });


   }

   #endregion Constructor

   #region Fieldz

   public VvHamper hamp_partner_PTG, hamp_TT_PTG, hamp_v1TT_PTG, hamp_v2TT_PTG, hamp_KUGpartner_PTG, hamp_PvrNum_PTG,
                   hamp_napomena_PTG, hamp_sklad_PTG;
   public RadioButton rbt_mjIsp_Korisnik, rbt_mjIsp_PcToGo, rbt_mjIsp_KorisOvers;
   private CheckBox cbx_zizStatus;

   #endregion Fieldz

   #region Hampers

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      InitializeHamper_Partner_PTG   (out hamp_partner_PTG);
      InitializeHamper_TT_PTG        (out hamp_TT_PTG        );
      InitializeHamper_v1TT_PTG      (out hamp_v1TT_PTG      );
      InitializeHamper_v2TT_PTG      (out hamp_v2TT_PTG      );
      InitializeHamper_PVRnum_PTG    (out hamp_PvrNum_PTG    );
      InitializeHamper_KUGpartner_PTG(out hamp_KUGpartner_PTG);
      InitializeHamper_Datum_PTG     (out hamp_dokDate   );
      InitializeHamper_Napomena_PTG  (out hamp_napomena_PTG  );
      InitializeHamper_Skladista_PTG (out hamp_sklad_PTG     );
      CreateArrOfHampers();

      SetParentOfHamperLeftHampers();

      panel_MigratorsLeftB.SendToBack();

      hamp_partner_PTG.Location = new Point(0, 0);
      hamp_TT_PTG     .Location = new Point(hamp_partner_PTG.Right, 0);

      int hamp_PTG_PvrNumLeft = hamp_TT_PTG.Right - hamp_PvrNum_PTG.Width;
      int hamp_v2TT_PTGLeft   = hamp_TT_PTG.Right - hamp_PvrNum_PTG.Width - hamp_v2TT_PTG.Width;
      int hamp_v1TT_PTGLeft   = hamp_TT_PTG.Right - hamp_PvrNum_PTG.Width - hamp_v2TT_PTG.Width - hamp_v1TT_PTG.Width;

      hamp_v2TT_PTG.Location   = new Point(hamp_v2TT_PTGLeft  , hamp_TT_PTG.Bottom);
      hamp_v1TT_PTG.Location   = new Point(hamp_v1TT_PTGLeft  , hamp_TT_PTG.Bottom);
      hamp_PvrNum_PTG.Location = new Point(hamp_PTG_PvrNumLeft, hamp_TT_PTG.Bottom);

      hamp_dokNum .Location = new Point(hamp_TT_PTG.Right, hamp_TT_PTG.Top );
      hamp_dokDate.Location = new Point(hamp_TT_PTG.Left , hamp_v1TT_PTG.Bottom );

      hamp_KUGpartner_PTG.Location = new Point(hamp_TT_PTG.Left, hamp_TT_PTG.Bottom);

      hamp_napomena_PTG.Location = new Point(hamp_partner_PTG.Left, hamp_partner_PTG.Bottom);

      hamp_sklad_PTG.Location = new Point(hamp_TT_PTG.Left + ZXC.Q2un, hamp_partner_PTG.Bottom);

      nextY = hamp_napomena_PTG.Bottom;

      hamp_twin.Visible = false;
      hamp_IznosUvaluti.Visible = false;

   }

   private void CreateArrOfHampers()
   {

      hamperLeft = new VvHamper[] { hamp_TT_PTG, hamp_partner_PTG, hamp_dokDate, hamp_dokNum,
                                    hamp_v1TT_PTG, hamp_v2TT_PTG, hamp_PvrNum_PTG, hamp_KUGpartner_PTG,
                                    hamp_napomena_PTG, hamp_sklad_PTG
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

   private void InitializeHamper_PVRnum_PTG(out VvHamper hamper)
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

                       hamper.CreateVvLabel  (0, 0, "Partner:", ContentAlignment.MiddleRight);
      tbx_KupdobCd   = hamper.CreateVvTextBox(1, 0, "tbx_kupdobCd"  , "Sifra partnera" , 6);
      tbx_KupdobTk   = hamper.CreateVvTextBox(2, 0, "tbx_kupdobTk"  , "Ticker partnera", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.kupdobTK));
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
      tbx_PosJedAdresa.JAM_ReadOnly = 
      tbx_KupdobTk    .JAM_ReadOnly = 
      tbx_KupdobName  .JAM_ReadOnly = 
      tbx_KupdobCd    .JAM_ReadOnly = false;
      tbx_somePercent .JAM_ReadOnly = 
      tbx_RokPlac     .JAM_ReadOnly = true;

   }

   private void InitializeHamper_Datum_PTG(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth + ZXC.QUN, ZXC.Q4un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { faBefCol  ,            faBefCol,         };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;
      hamper.VvBottomMargin = ZXC.Qun8;

                    hamper.CreateVvLabel  (0, 0, "Datum:", ContentAlignment.MiddleRight);
      tbx_DokDate = hamper.CreateVvTextBox(1, 0, "tbx_dokDate", "Datum ugovora");
      tbx_DokDate.JAM_IsForDateTimePicker = true;
      dtp_DokDate = hamper.CreateVvDateTimePicker(1, 0, "", tbx_DokDate);//PTG_DatUgovora     
      dtp_DokDate.Name = "dtp_DokDate";
      dtp_DokDate.ValueChanged += new EventHandler(dtp_DokDate_ValueChanged_SetSkladAndPdvDate);
      dtp_DokDate.Leave += new EventHandler(ValidateDOD_dokDate);

      tbx_DokDate.Font = ZXC.vvFont.BaseBoldFont;
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
      hamper = new VvHamper(4, 3, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth + ZXC.Q2un, ZXC.Q3un - ZXC.Qun2, ZXC.Q5un, ZXC.Q2un - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { faBefFirstCol, faBefCol, faBefCol, faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel        (0, 0, "Izdajemo sa:", ContentAlignment.MiddleRight);
      tbx_SkladCd   = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_SkladCd", "Izadjemo sa skladista");
      tbx_SkladOpis = hamper.CreateVvTextBox      (2, 0, "tbx_SkladOpis", "");
      tbx_SkladBR   = hamper.CreateVvTextBox      (3, 0, "tbx_SkladRbr", "");
      tbx_SkladCd.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_SkladCd.JAM_DataRequired = true;
      tbx_SkladCd.JAM_MustTabOutBeforeSubmit = true;

    //tbx_SkladCd  .JAM_ReadOnly = true;
      tbx_SkladOpis.JAM_ReadOnly = true;
      tbx_SkladBR  .JAM_ReadOnly = true;

      tbx_SkladCd.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_SkladCd.JAM_lui_NameTaker_JAM_Name    = tbx_SkladOpis.JAM_Name;
      tbx_SkladCd.JAM_lui_IntegerTaker_JAM_Name = tbx_SkladBR.JAM_Name;
      tbx_SkladCd.VVtag2 = true;

                      hamper.CreateVvLabel         (0, 1, "Vraća se na:", ContentAlignment.MiddleRight);
      tbx_Sklad2Cd  = hamper.CreateVvTextBoxLookUp (1, 1, "tbx_Sklad2Cd", "Vraća se na skladište");
      tbx_Sklad2Opis = hamper.CreateVvTextBox      (2, 1, "tbx_SkladOpis2", "");
      tbx_SkladRbr2 = hamper.CreateVvTextBox       (3, 1, "tbx_SkladRbr2", "");
      tbx_Sklad2Cd.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_Sklad2Cd.JAM_DataRequired = true;
      tbx_Sklad2Cd.JAM_MustTabOutBeforeSubmit = true;

    //tbx_Sklad2Cd  .JAM_ReadOnly = true;
      tbx_Sklad2Opis.JAM_ReadOnly = true;
      tbx_SkladRbr2.JAM_ReadOnly = true;

      tbx_Sklad2Cd.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_Sklad2Cd.JAM_lui_NameTaker_JAM_Name    = tbx_Sklad2Opis.JAM_Name;
      tbx_Sklad2Cd.JAM_lui_IntegerTaker_JAM_Name = tbx_SkladRbr2.JAM_Name;
      tbx_Sklad2Cd.VVtag2 = true;

      tbx_SkladCd .JAM_FieldExitWithValidationMethod += new System.ComponentModel.CancelEventHandler(OnExitSkladCD_SetTtNum_And_ValidateSkladCD);
      tbx_Sklad2Cd.JAM_FieldExitWithValidationMethod += new System.ComponentModel.CancelEventHandler(OnExitSkladCD_SetTtNum_And_ValidateSkladCD);

      cbx_zizStatus = hamper.CreateVvCheckBox_OLD(1, 2, null, 1, 0, "ZIZ status", RightToLeft.No);
      cbx_zizStatus.Enabled = false;
   }

   #endregion Hampers

   #region Fld

   public DateTime Fld_PTG_DatDostave { get { return dtp_DokDate2.Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_DokDate2.Value = value; } } }

   public bool Fld_PTG_ZizStatus { get { return cbx_zizStatus.Checked; } set { cbx_zizStatus.Checked = value; } }

   #endregion Fld

   #region TheG_Specific_Columns
   public override bool HasRtrans_SkladCD_Exposed { get { return false; } }
   public override bool HasRtrano_SkladCD_Exposed { get { return true; } }
   protected override void InitializeDUC_Specific_Columns()
   {
      T_TT_CreateColumnG1          (ZXC.Q2un,               true, ""      , "Tip Izlaznog dokumenta", true);
      R_Opis_CreateColumn          (ZXC.Q4un,               true, ""      , "Opis Izlaznog dokumenta");
      T_skladCD_CreateColumn       (ZXC.Q3un,               true, "SaSkl" , "Izlaz sa skladišta");
      R_TT2_CreateColumn           (ZXC.Q2un,               true, ""      , "Tip Ulaznog dokumenta");
      R_Opis2_CreateColumn         (ZXC.Q3un,               true, ""      , "Opis Ulaznog dokumenta");
      R_skladCd2_CreateColumn      (ZXC.Q3un,               true, "NaSkl" , "Ulaz na skladišta");
      T_artiklCD_CreateColumn      (ZXC.Q3un           ,    true, "Šifra" , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                        true, "Naziv" , "Naziv artikla ili proizvoljan opis");
      T_artiklTS_CreateColumn      (ZXC.Q2un,               true, "Tip"   , "Tip artikla");
      T_doCijMal_CreateColumn      (ZXC.Q3un, 0,            true, "RAM"   , "RAM", false);
      T_noCijMal_CreateColumn      (ZXC.Q3un, 0,            true, "HDD"   , "HDD");
      T_jedMj_CreateColumn         (ZXC.Q2un           ,    true, "JM"    , "Jedinica mjere"                    );
      T_kol_CreateColumn           (ZXC.Q3un           , 2, true, "Kol"   , "Količina"                );
      T_cij_CreateColumn           (ZXC.Q4un           , 4, true, "Cijena", "Jedinična cijena"                  );
      R_KCR_CreateColumn           (ZXC.Q4un,            2, true, "Iznos" , "Ukupan iznos bez PDV-a");

      vvtbT_cij.JAM_ReadOnly = true;

   }

   public static string ZIZ_DUC_izlazText = "Izlazi sa";
   public static string ZIZ_DUC_ulazText  = "Ulazi na";

   #endregion TheG_Specific_Columns

   #region TheG_Specific_Columns2

   protected override void InitializeDUC_Specific_Columns2()
   {
      bool isVisible = true;

      T_TT_CreateColumn             (ZXC.Q2un,         true, "TT"           , "Tip dokumenta"        );
      T_serno_CreateColumn          (ZXC.Q8un,    isVisible, "Serijski broj", "Serijski broj artikla"             );
      T_artiklCD2_CreateColumn      (ZXC.Q5un,    isVisible, "Šifra"        , "Šifra artikla"                     );
      T_artiklName2_CreateColumnFill(             isVisible, "Naziv"        , "Naziv artikla ili proizvoljan opis");
      R_artiklTS_CreateColumn       (ZXC.Q3un - ZXC.Qun2,    isVisible, "Tip"          , "Tip artikla");
      R_ramKlasa2_CreateColumn      (ZXC.Q3un,    isVisible, "RAM klasa"    , "RAM klasa");
      R_hddKlasa2_CreateColumn      (ZXC.Q3un,    isVisible, "HDD klasa"    , "RAM klasa");
      R_skladCD1_CreateColumn       (ZXC.Q3un,    isVisible, "IzlSkl"       , "Izlazno skladište");
      T_skladCD2_CreateColumn       (ZXC.Q3un,    isVisible, "UlzSkl"       , "Ulazno skladište"                 );
      T_dimZ_CreateColumn           (ZXC.Q3un, 0, isVisible, "RAM"          , "RAM"                           );
      T_decC_CreateColumn           (ZXC.Q3un, 0, isVisible, "HDD"          , "HDD old"                           );
      T_grCD_CreateColumn           (ZXC.Q5un,    isVisible, "Opis"         , "Opis"                       , false);
      T_paletaNo_CreateColumn       (ZXC.Q3un,    isVisible, "PvrSt"        , "PVR stavka"                     );
   }

   #endregion TheG_Specific_Columns2

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_DOD_PTG, Color.Empty, clr_DOD_PTG);
   }
   public override bool IsPTG_DUC_wRtrano { get { return true; } }

   public bool IsZIZ_completed 
   { 
      get 
      {
         Rtrans                 rtrans_rec;
         Rtrano prevRtrano_rec, rtrano_rec;

         return true;
         // tu si stao 
         // NEMOJ ovo kao property i vidi kad s ga pozivati (nemoj na saki get fields nego sdamo kod sejvanja)
         // ZIZ je kompletiran kada je uspost simetrija:
         // 1. ukZelenaKOL = ukBijelaKol
         // 2. ukZelenaKOL + ukBijelaKol = ukPopunjenihSernoaCount

         decimal ZIZkol = /*faktur_rec.Transes.Where(rtr => rtr.T_TT == Faktur.TT_ZIZ).Sum(rtr => rtr.T_kol)*/ 0M;
         decimal ZULkol = /*faktur_rec.Transes.Where(rtr => rtr.T_TT == Faktur.TT_ZUL).Sum(rtr => rtr.T_kol)*/ 0M;

         for(int rowIdx1 = 0; rowIdx1 < TheG./*RowCount - 1*/VvEffectiveRowCount; ++rowIdx1)
         {
            rtrans_rec = (Rtrans)GetDgvLineFields1(rowIdx1, false, null);

            if(rtrans_rec.T_TT == Faktur.TT_ZIZ) ZIZkol += rtrans_rec.T_kol;
            if(rtrans_rec.T_TT == Faktur.TT_ZUL) ZULkol += rtrans_rec.T_kol;
         }

         bool isKolOK = ZIZkol == ZULkol;

         bool isTtNumOK = true;

         uint prevRtrano_TtNum;

         for(int rowIdx2 = 0; rowIdx2 < TheG2./*RowCount - 1*/VvEffectiveRowCount; ++rowIdx2)
         {
            rtrano_rec = (Rtrano)GetDgvLineFields2(rowIdx2, false, null);

            if(rtrano_rec.T_TT != Faktur.TT_ZU2) continue; // provjeravaj samo zeleni / ulazni ZU2 serno ... da li je bio na tom UgAnTtNum-u

            prevRtrano_rec = FakturDao.SetMePreviousRtranoForThisSerno(TheDbConnection, rtrano_rec.T_serno, rtrano_rec);

            if(prevRtrano_rec == null)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "NEMA PreviousRtranoForThisSerno [{0}]", rtrano_rec.T_serno);

               isTtNumOK = false;

               continue;
            }

            prevRtrano_TtNum = prevRtrano_rec.T_ttNum;

            (string prevUgAn_TT, uint prevUgAn_TtNum) = UGNorAUN_PTG_DUC.Get_UgAnFaktur_TtAndTtNum_ForThisRtranoTtAndTtNum(prevRtrano_rec);

            if(Fld_V2_ttNum != prevUgAn_TtNum)
            {
               isTtNumOK = false;
            }
         }

         // tu si stao 
         // primjer: na ZIZ 15002 imas lon2 zeleni radak a on je yapravo prema UGN 16 otiso u najam 
         // a ti si na ZIZ 15 .. pa treba reagirati 

         return (isKolOK && isTtNumOK);
      } 
   }
}

public class PRN_DIZ_PTG_DUC : DIZ_PTG_DUC
{
   #region Constructor

   public PRN_DIZ_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
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

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q5un, ZXC.Q2un - ZXC.Qun2, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q3un, ZXC.Q4un };
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

      tbx_RacunOd               = hamper.CreateVvTextBox(1, 0, "tbx_RacunOd", "Račun od", 8);
      tbx_RacunOd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_RacunOd.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_RacunDo = hamper.CreateVvTextBox(3, 0, "tbx_RacunDo", "Račun do", 8);
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
      vvtb_racun      = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_racun"     , null, -12, "RačunBr"           ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_racun     , null, "R_brRacuna"  , "Broj računa"      , ZXC.Q4un + ZXC.Qun2);
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

public class URA_PTG_DUC : FakturPDUC
{
   #region Constructor

   public URA_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_URA
         });
      // 31.10.2013: nakon Tembo problem (ubuduce bi trebalo nekako preko rulsa raci koji DUC-evi mogu a koji ne kroz vise skladista)
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;
   }

   #endregion Constructor
  
   #region HamperLocation
   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(true, true, true, false);

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_pdvGeokind, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_osobaX,hamp_carinaKind,/* hamp_rokIsporuke, hamp_rokIspDate, hamp_tipOtpreme,*/
                                    hamp_tipOtpreme, 
                                    hamp_externLink1, hamp_externLink2, hamp_prjIdent, hamp_eRproc,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX, hampCbxM_carinaKind,/* hampCbxM_rokIsporuke, hampCbxM_rokIspDate	, hampCbxM_tipOtpreme,*/
                                        hampCbxM_tipOtpreme, 
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent, hampCbxM_eRproc,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns
   public override bool HasRtrans_SkladCD_Exposed { get { return false; } }
   public override bool HasRtrano_SkladCD_Exposed { get { return true; } }

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn  (ZXC.Q4un   ,             isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                     isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_artiklTS_CreateColumn  (ZXC.Q2un,               true, "Tip"     , "Tip artikla");
      T_doCijMal_CreateColumn  (ZXC.Q3un, 0,             isVisible, "RAM"        , "RAM", false);
      T_noCijMal_CreateColumn  (ZXC.Q3un, 0,             isVisible, "HDD"        , "HDD");
      T_isIrmUsluga_CreateColumn(ZXC.QUN + ZXC.Qun4,     isVisible, "Usl"        , "Usluga");
      T_konto_CreateColumn     (ZXC.Q2un ,               isVisible, "Konto"      , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_kol_CreateColumn       (ZXC.Q3un, 2,             isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   ,             isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4,             isVisible, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn    (ZXC.Q3un - ZXC.Qun4, 2,  isVisible, "Rb1"        , "Stopa rabata 1");
      R_KCR_CreateColumn       (ZXC.Q4un, 2,             isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
      T_pdvSt_CreateColumn     (ZXC.Q2un, 0,             isVisible, "PdvSt"      , "Stopa PDV-a"           );
      T_pdvKolTip_CreateColumn (ZXC.QUN    ,             isVisible                                       );
      R_KCRP_CreateColumn      (ZXC.Q4un + ZXC.Qun2 , 2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om");
   }

   #endregion TheG_Specific_Columns
   
   #region TheG_Specific_Columns2

   protected override void InitializeDUC_Specific_Columns2()
   {
      bool isVisible = true;

      T_serno_CreateColumn          (ZXC.Q8un,               isVisible, "Serijski broj", "Serijski broj artikla"             );
      T_artiklCD2_CreateColumn      (ZXC.Q5un,               isVisible, "Šifra"        , "Šifra artikla"                     );
      T_artiklName2_CreateColumnFill(                        isVisible, "Naziv"        , "Naziv artikla ili proizvoljan opis");
      R_artiklTS_CreateColumn       (ZXC.Q3un - ZXC.Qun2,    isVisible, "Tip"          , "Tip artikla");
      R_ramKlasa2_CreateColumn      (ZXC.Q3un,               isVisible, "RAM klasa"    , "RAM klasa");
      R_hddKlasa2_CreateColumn      (ZXC.Q3un,               isVisible, "HDD klasa"    , "RAM klasa");
      T_skladCD2_CreateColumn       (ZXC.Q3un,               isVisible, "UlzSkl"       , "Ulazno skladište"                 );
      T_dimZ_CreateColumn           (ZXC.Q3un, 0,            isVisible, "RAM"          , "RAM"                               );
      T_decC_CreateColumn           (ZXC.Q3un, 0,            isVisible, "HDD"          , "HDD old"                           );
      T_grCD_CreateColumn           (ZXC.Q5un,    isVisible, "Opis"         , "Opis"                       , false);
      T_paletaNo_CreateColumn       (ZXC.Q3un,               isVisible, "PriSt"       , "UGANDO stavka"                     );

      vvtbT_skladCD2.JAM_ReadOnly = true;
   }

   #endregion TheG_Specific_Columns2

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturURbDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Ulaz, clr_Sklad, clr_Ulaz);
   }
   public override bool IsPTG_DUC_wRtrano { get { return true; } }

}

public class PRI_PTG_DUC : FakturPDUC
{

   #region Constructor
   public PRI_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
      (Faktur.tt_colName, new string[]
      {
         Faktur.TT_PRI
      });
   }

   #endregion Constructor
       
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, true, false);

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_osobaX,hamp_carinaKind,/* hamp_rokIsporuke, hamp_rokIspDate, hamp_tipOtpreme,*/
                                    hamp_externLink1, hamp_externLink2, hamp_prjIdent,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX, hampCbxM_carinaKind,/* hampCbxM_rokIsporuke, hampCbxM_rokIspDate	, hampCbxM_tipOtpreme,*/
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns
   public override bool HasRtrans_SkladCD_Exposed { get { return false; } }
   public override bool HasRtrano_SkladCD_Exposed { get { return true; } }

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un   , isVisible, "Šifra" , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(             isVisible, "Naziv" , "Naziv artikla ili proizvoljan opis");
      T_artiklTS_CreateColumn      (ZXC.Q2un,    isVisible, "Tip"   , "Tip artikla");
      T_doCijMal_CreateColumn      (ZXC.Q3un, 0, isVisible, "RAM"   , "RAM", false);
      T_noCijMal_CreateColumn      (ZXC.Q3un, 0, isVisible, "HDD"   , "HDD");
      T_kol_CreateColumn           (ZXC.Q3un, 2, isVisible, "Kol"   , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   , isVisible, "JM"    , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q4un, 2, isVisible, "Cijena", "Jedinična cijena");
      T_rbt1St_CreateColumn        (ZXC.Q3un, 2, isVisible, "Rb1"   , "Stopa rabata 1");
      R_KCRM_CreateColumn          (ZXC.Q4un, 2, isVisible, "Iznos" , "Iznos");
   
   }

   #endregion TheG_Specific_Columns

   #region TheG_Specific_Columns2

   protected override void InitializeDUC_Specific_Columns2()
   {
      bool isVisible = true;

      T_serno_CreateColumn          (ZXC.Q8un,               isVisible, "Serijski broj", "Serijski broj artikla"             );
      T_artiklCD2_CreateColumn      (ZXC.Q5un,               isVisible, "Šifra"        , "Šifra artikla"                     );
      T_artiklName2_CreateColumnFill(                        isVisible, "Naziv"        , "Naziv artikla ili proizvoljan opis");
      R_artiklTS_CreateColumn       (ZXC.Q3un - ZXC.Qun2,    isVisible, "Tip"          , "Tip artikla");
      R_ramKlasa2_CreateColumn      (ZXC.Q3un,               isVisible, "RAM klasa"    , "RAM klasa");
      R_hddKlasa2_CreateColumn      (ZXC.Q3un,               isVisible, "HDD klasa"    , "RAM klasa");
      T_skladCD2_CreateColumn       (ZXC.Q3un,               isVisible, "UlzSkl"       , "Ulazno skladište"                 );
      T_dimZ_CreateColumn           (ZXC.Q3un, 0,            isVisible, "RAM"          , "RAM"                               );
      T_decC_CreateColumn           (ZXC.Q3un, 0,            isVisible, "HDD"          , "HDD old"                           );
      T_grCD_CreateColumn           (ZXC.Q5un,    isVisible, "Opis"         , "Opis"                       , false);
      T_paletaNo_CreateColumn       (ZXC.Q3un,               isVisible, "PriSt"       , "UGANDO stavka"                     );

      vvtbT_skladCD2.JAM_ReadOnly = true;
   }

   #endregion TheG_Specific_Columns2

   #region overrideMigratorList

 //internal /*protected*/ override List<VvMigrator> MigratorList
 //{
 //   get { return ZXC.TheVvForm.VvPref.fakturURPDUC.MigratorStates; }
 //}

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Ulaz, clr_Sklad, clr_Ulaz);
   }

   public override bool IsPTG_DUC_wRtrano { get { return true; } }

}

public class IRA_PTG_DUC : FakturPDUC
{
   #region Constructor

   public IRA_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_IRA
          //, Faktur.TT_YRA
         });

      TheG.CellMouseDoubleClick += TheG_CellMouseDoubleClick_ShowFakturDUC_For_TipBr;

   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(true, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    /*hamp_kupdobOther,*/ hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_pdvZPkind, hamp_pdvGeokind, hamp_kupdobOther, hamp_Cjenik, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_NacPlac, hamp_fiskJIR
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, /*hamp_NacPlac,*/hamp_DatumX,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,  hamp_osobaX,hamp_carinaKind,
                                    hamp_dostava, hamp_PonudDate,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,hamp_fiskMsgID    , hamp_fiskOibOp,     hamp_fiskPrgBr,
                                    hamp_eRproc, hamp_fiskPrgBr, hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, /*hampCbxM_NacPlac,*/hampCbxM_DatumX, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme, hampCbxM_osobaX, hampCbxM_carinaKind,
                                        hampCbxM_dostava, hampCbxM_PonudDate,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,hampCbxM_fiskMsgID, hampCbxM_fiskOibOp, hampCbxM_fiskPrgBr,
                                        hampCbxM_eRproc, hampCbxM_fiskPrgBr, hampCbxM_opis
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   public override bool HasRtrans_SkladCD_Exposed { get { return false; } }
   public override bool HasRtrano_SkladCD_Exposed { get { return true; } }

   protected override void InitializeDUC_Specific_Columns()
   {
      T_artiklCD_CreateColumn      (ZXC.Q4un          , true, "Šifra"    , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                    true, "Naziv"    , "Naziv artikla");
      T_serlot_CreateColumn        (ZXC.Q6un          , true, "UGAN Rata", "Broj ugovora i rate");
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,                                                      true, "Usl"        , "Usluga");
      T_kol_CreateColumn           (ZXC.Q3un, 2,                                                             true, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   ,                                                             true, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q4un, 4,                                                    true, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn        (ZXC.Q3un-ZXC.Qun4, 2,                                           true, "Rb1"        , "Stopa rabata 1");
      R_KCR_CreateColumn           (ZXC.Q4un, 2,                                                    true, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
      R_cij_kcr_CreateColumn       (ZXC.Q4un, 2, false, "VPC"   , "Veleprodajna cijena");
      R_NC_CreateColumn            (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
      R_NV_CreateColumn            (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
      R_RUC_CreateColumn           (ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
      R_RUV_CreateColumn           (ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");

      T_pdvSt_CreateColumn         (ZXC.Q2un, 0         ,    true, "PdvSt"      , "Stopa PDV-a");
      T_pdvKolTip_CreateColumn     (ZXC.QUN             ,    true);
      R_KCRP_CreateColumn          (ZXC.Q4un + ZXC.Qun2 , 2, true, "Uk s PDV-om", "Ukupno s PDV-om");
   }

   #endregion TheG_Specific_Columns

   #region TheG2_Specific_Columns2

   protected override void InitializeDUC_Specific_Columns2()
   {
      bool isVisible = true;

      T_serno_CreateColumn          (ZXC.Q8un,            isVisible, "Serijski broj", "Serijski broj artikla"             );
      T_artiklCD2_CreateColumn      (ZXC.Q5un,            isVisible, "Šifra"        , "Šifra artikla"                     );
      T_artiklName2_CreateColumnFill(                     isVisible, "Naziv"        , "Naziv artikla ili proizvoljan opis");
      R_artiklTS_CreateColumn       (ZXC.Q3un - ZXC.Qun2, isVisible, "Tip"          , "Tip artikla");
      R_ramKlasa2_CreateColumn      (ZXC.Q3un,            isVisible, "RAM klasa"    , "RAM klasa");
      R_hddKlasa2_CreateColumn      (ZXC.Q3un,            isVisible, "HDD klasa"    , "RAM klasa");
      T_skladCD2_CreateColumn       (ZXC.Q3un,            isVisible, "IzlSkl"       , "Izlazno skladište"                 );
      T_dimZ_CreateColumn           (ZXC.Q3un, 0,         isVisible, "RAM"          , "RAM"                               );
      T_decC_CreateColumn           (ZXC.Q3un, 0,         isVisible, "HDD"          , "HDD old"                           );
      T_grCD_CreateColumn           (ZXC.Q5un,            isVisible, "Opis"         , "Opis"                       , false);
      T_paletaNo_CreateColumn       (ZXC.Q3un,            isVisible, "IzdSt"        , "UGANDO stavka"                     );

      vvtbT_skladCD2.JAM_ReadOnly = true;

   }

   #endregion TheG2_Specific_Columns2

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturIRbDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   private void TheG_CellMouseDoubleClick_ShowFakturDUC_For_TipBr(object sender, DataGridViewCellMouseEventArgs e)
   {
      VvDataGridView theG = sender as VvDataGridView;

      int rowIdx = e.RowIndex;

      if(rowIdx.IsNegative()) return;

      string tipBr = theG.GetStringCell(ci.iT_serlot, rowIdx, false);

      ZXC.TheVvForm.ShowFakturDUC_For_TipBr(tipBr);

   }

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_Sklad, clr_Izlaz);
   }

   public override bool IsPTG_DUC_wRtrano { get { return true; } }

   //public override bool HasDscSubVariants ne znam cemu to sluzi?!
   //{
   //   get
   //   {
   //      return true;
   //   }
   //}
}

public class IZD_PTG_DUC : FakturPDUC
{
   #region Constructor
   public IZD_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
      (Faktur.tt_colName, new string[]
      {
         Faktur.TT_IZD
      });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    /*hamp_kupdobOther,*/ hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PonudDate, hamp_kupdobOther,hamp_Cjenik, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,  hamp_osobaX,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme,  hampCbxM_osobaX,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns
   public override bool HasRtrans_SkladCD_Exposed { get { return false; } }
   public override bool HasRtrano_SkladCD_Exposed { get { return true; } }

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un,             isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                      isVisible, "Naziv"      , "Naziv artikla");
      T_artiklTS_CreateColumn      (ZXC.Q2un             ,isVisible, "Tip"        , "Tip artikla");
      T_doCijMal_CreateColumn      (ZXC.Q3un, 0,          isVisible, "RAM"        , "RAM", false);
      T_noCijMal_CreateColumn      (ZXC.Q3un, 0,          isVisible, "HDD"        , "HDD");
      T_kol_CreateColumn           (ZXC.Q3un, 2,          isVisible, "Kol"        , "Količina");
      T_jedMj_CreateColumn         (ZXC.Q2un,             isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q4un, 2,          isVisible, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn        (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"        , "Stopa rabata 1");
      R_KCRM_CreateColumn          (ZXC.Q4un, 2,          isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");

      R_NC_CreateColumn (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
      R_NV_CreateColumn (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
      R_RUC_CreateColumn(ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
      R_RUV_CreateColumn(ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");
   
   }

   #endregion TheG_Specific_Columns

   #region TheG2_Specific_Columns2

   protected override void InitializeDUC_Specific_Columns2()
   {
      bool isVisible = true;

      T_serno_CreateColumn          (ZXC.Q8un,               isVisible, "Serijski broj", "Serijski broj artikla"             );
      T_artiklCD2_CreateColumn      (ZXC.Q5un,               isVisible, "Šifra"        , "Šifra artikla"                     );
      T_artiklName2_CreateColumnFill(                        isVisible, "Naziv"        , "Naziv artikla ili proizvoljan opis");
      R_artiklTS_CreateColumn       (ZXC.Q3un - ZXC.Qun2,    isVisible, "Tip"          , "Tip artikla");
      R_ramKlasa2_CreateColumn      (ZXC.Q3un,               isVisible, "RAM klasa"    , "RAM klasa");
      R_hddKlasa2_CreateColumn      (ZXC.Q3un,               isVisible, "HDD klasa"    , "RAM klasa");
      T_skladCD2_CreateColumn       (ZXC.Q3un,               isVisible, "IzlSkl"       , "Izlazno skladište"                 );
      T_dimZ_CreateColumn           (ZXC.Q3un, 0,            isVisible, "RAM"          , "RAM"                               );
      T_decC_CreateColumn           (ZXC.Q3un, 0,            isVisible, "HDD"          , "HDD old"                           );
      T_grCD_CreateColumn           (ZXC.Q5un,    isVisible, "Opis"         , "Opis"                       , false);
      T_paletaNo_CreateColumn       (ZXC.Q3un,               isVisible, "IzdSt"        , "UGANDO stavka"                     );

      vvtbT_skladCD2.JAM_ReadOnly = true;

   }

   #endregion TheG2_Specific_Columns2

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturIzdatnicaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_Sklad, Color.Empty);
   }

   public override bool IsPTG_DUC_wRtrano { get { return true; } }

}

public class MSI_PTG_DUC : FakturPDUC
{

   #region Constructor
   public MSI_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
      (Faktur.tt_colName, new string[]
      {
         Faktur.TT_MPI
      });
   }

   #endregion Constructor

   #region HamperLocation
   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();
      
      hamp_tt.BringToFront();
      nextY = hamp_napomena.Bottom;

      hamp_twin.Visible = false;
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , hamp_sklad2Cd,
                                    hamp_dokDate, hamp_dokDate2, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT 
                                   };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns
      
   protected override void InitializeDUC_Specific_Columns()
   {
      CreateAllwaysInvisibleDataGridViewColumn(TheG, "t_twinID");
      bool isVisible = true;
      T_artiklCD_CreateColumn      (ZXC.Q3un           ,isVisible, "Šifra"  , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                    isVisible, "Naziv"  , "Naziv artikla ili proizvoljan opis");
      T_artiklTS_CreateColumn      (ZXC.Q2un,               true, "Tip"     , "Tip artikla");
      T_doCijMal_CreateColumn      (ZXC.Q3un, 0,        isVisible, "RAM", "RAM", false);
      T_noCijMal_CreateColumn      (ZXC.Q3un, 0,        isVisible, "HDD", "HDD");
      T_kol_CreateColumn           (ZXC.Q4un, 2,        isVisible, "Kol"    , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un + ZXC.Qun2,isVisible, "JM"     , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q5un, 4       , isVisible, "Cijena" , "Jedinična cijena");
      R_KC_CreateColumn            (ZXC.Q4un, 2       , isVisible, "Iznos"  , "Iznos");

      vvtbT_cij.JAM_ReadOnly = true;
   }

   #endregion TheG_Specific_Columns

   #region TheG2_Specific_Columns2

   protected override void InitializeDUC_Specific_Columns2()
   {
      bool isVisible = true;

      T_serno_CreateColumn          (ZXC.Q8un,               isVisible, "Serijski broj", "Serijski broj artikla"             );
      T_artiklCD2_CreateColumn      (ZXC.Q5un,               isVisible, "Šifra"        , "Šifra artikla"                     );
      T_artiklName2_CreateColumnFill(                        isVisible, "Naziv"        , "Naziv artikla ili proizvoljan opis");
      R_artiklTS_CreateColumn       (ZXC.Q3un - ZXC.Qun2,    isVisible, "Tip"          , "Tip artikla");
      R_ramKlasa2_CreateColumn      (ZXC.Q3un,               isVisible, "RAM klasa"    , "RAM klasa");
      R_hddKlasa2_CreateColumn      (ZXC.Q3un,               isVisible, "HDD klasa"    , "RAM klasa");
      T_skladCD2_CreateColumn       (ZXC.Q3un,               isVisible, "Sklad"        , "Izlazno skladište"                 );
      T_dimZ_CreateColumn           (ZXC.Q3un, 0,            isVisible, "RAM"          , "RAM"                               );
      T_decC_CreateColumn           (ZXC.Q3un, 0,            isVisible, "HDD"          , "HDD old"                           );
      T_grCD_CreateColumn           (ZXC.Q5un,    isVisible, "Opis"         , "Opis"                       , false);
      T_paletaNo_CreateColumn       (ZXC.Q3un,               isVisible, "Stavka"       , "UGANDO stavka"                     );
   }

   #endregion TheG2_Specific_Columns2

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Thistle);
   }

   public override bool IsPTG_DUC_wRtrano { get { return true; } }

}

public class PST_PTG_DUC : FakturPDUC
{
   #region Constructor

   public PST_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PST
         });

      SuspendLayout();

    //ThePolyGridTabControl.TabPages.RemoveAt(1);

      ResumeLayout();
   }

   #endregion Constructor
   
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();
  
      hamp_dokNum.Location = new Point(hamp_napomena.Right - hamp_dokNum.Width - ZXC.Qun4, 0);

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , 
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT, hamp_opis
                                   };
      
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns
   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q3un,            isVisible, "Šifra" , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                     isVisible, "Naziv" , "Naziv artikla ili proizvoljan opis");
      T_artiklTS_CreateColumn      (ZXC.Q2un,               true, "Tip"     , "Tip artikla");
      T_doCijMal_CreateColumn      (ZXC.Q3un, 0,         isVisible, "RAM"   , "RAM", false);
      T_noCijMal_CreateColumn      (ZXC.Q3un, 0,         isVisible, "HDD"   , "HDD");
      T_kol_CreateColumn           (ZXC.Q4un, 2,         isVisible, "Kol"   , "Količina");
      T_jedMj_CreateColumn         (ZXC.Q2un + ZXC.Qun2, isVisible, "JM"    , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q5un, 4  ,       isVisible, "Cijena", "Jedinična cijena");
      R_KC_CreateColumn            (ZXC.Q4un        , 2, isVisible, "Iznos" , "Iznos");

   }

   #endregion TheG_Specific_Columns

   #region TheG2_Specific_Columns2

   protected override void InitializeDUC_Specific_Columns2()
   {
      bool isVisible = true;

      T_serno_CreateColumn          (ZXC.Q8un,               isVisible, "Serijski broj", "Serijski broj artikla"             );
      T_artiklCD2_CreateColumn      (ZXC.Q5un,               isVisible, "Šifra"        , "Šifra artikla"                     );
      T_artiklName2_CreateColumnFill(                        isVisible, "Naziv"        , "Naziv artikla ili proizvoljan opis");
      R_artiklTS_CreateColumn       (ZXC.Q3un - ZXC.Qun2,    isVisible, "Tip"          , "Tip artikla");
      R_ramKlasa2_CreateColumn      (ZXC.Q3un,               isVisible, "RAM klasa"    , "RAM klasa");
      R_hddKlasa2_CreateColumn      (ZXC.Q3un,               isVisible, "HDD klasa"    , "RAM klasa");
      T_skladCD2_CreateColumn       (ZXC.Q3un,               isVisible, "Sklad"        , "Izlazno skladište"                 );
      T_dimZ_CreateColumn           (ZXC.Q3un, 0,            isVisible, "RAM"          , "RAM"                               );
      T_decC_CreateColumn           (ZXC.Q3un, 0,            isVisible, "HDD"          , "HDD old"                           );
      T_grCD_CreateColumn           (ZXC.Q5un,    isVisible, "Opis"         , "Opis"                       , false);
      T_paletaNo_CreateColumn       (ZXC.Q3un,               isVisible, "Stavka"       , "UGANDO stavka"                     );
   }

   #endregion TheG2_Specific_Columns2

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Empty);
   }
   public override bool IsPTG_DUC_wRtrano { get { return true; } }

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

public partial class PCK_ArtiklList_Dlg :  VvDialog
{
   public PCK_ArtiklList_UC TheUC { get; set; }
   private Button okButton;
   private int dlgWidth, dlgHeight;
   public PCK_ArtiklList_Dlg(string currArtiklCD, string currSkladCD/*, PCK_ArtiklList_Caller theCaller*/)
   {
      ZXC.CurrentForm = this;

      TheUC = new PCK_ArtiklList_UC(this, currArtiklCD, currSkladCD/*, theCaller*/);

      SuspendLayout();

      this.Font        = ZXC.vvFont.BaseFont;
      this.Style       = ZXC.vvColors.vvform_VisualStyle;
      this.BackColor   = ZXC.vvColors.userControl_BackColor;

      this.StartPosition = FormStartPosition.Manual;

    //this.Text = TheUC.Fld_CurrArtikl;

      TheUC.Parent   = this;
      TheUC.Location = new Point(ZXC.Qun8, ZXC.Qun8);
      
      dlgWidth  = TheUC.Width;
      dlgHeight = TheUC.Height + ZXC.QunMrgn * 2 + ZXC.QunBtnH;

      this.MaximizeBox = true;

      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddZatvoriButton  (out okButton, dlgWidth, dlgHeight);
      okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      TheUC.Anchor                =  AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      TheUC.ThePCKInfoGrid.Anchor =  AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      TheUC.TheSernoGrid  .Anchor =  AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      
      this.Location = new Point(SystemInformation.WorkingArea.Width - this.Width, (SystemInformation.WorkingArea.Height - this.Height)/2);

      TheUC.ThePCKInfoSumGrid.ClearSelection();

      this.KeyDown += VvDialog_KeyDown;
      this.KeyPreview = true;

      ResumeLayout();

   }
   private void VvDialog_KeyDown(object sender, KeyEventArgs e)
   {
      if(e.KeyCode == Keys.Escape)
      {
         this.Close();
      }
   }
}

public enum PCK_ArtiklList_Caller
{
   ArtiklUC, ArtiklListUC, SubModulAction
}
public class PCK_ArtiklList_UC : VvUserControl
{
   #region Fieldz

   public VvDataGridView ThePCKBazeGrid   { get; set; }
   public VvDataGridView ThePCKInfoGrid   { get; set; }
   public VvDataGridView ThePCKInfoSumGrid   { get; set; }
   public VvDataGridView ThePCKBazeSumGrid   { get; set; }
   
   private VvTextBox
         vvtb_PCK_ArtCD  ,  
         vvtb_PCK_ArtName,
         vvtb_PCK_RAMkind,
         vvtb_PCK_HDDkind,  
         vvtb_PCK_SklCD  ,  
         vvtb_PCK_RAM    ,  
         vvtb_PCK_HDD    ,  
         vvtb_StanjeKol  ,
         vvtb_PCK_BazaName ,
         vvtb_PCK_BazaSklCD,
         vvtb_PCK_BazeStKol;

   private VvTextBoxColumn colVvText;
   private DataGridViewTextBoxColumn colScrol;

   List<PCK_Artikl> PCK_ArtiklList;
   List<PCK_Artikl> PCK_BazeSintList;

   public VvDataGridView TheSernoGrid { get; set; }
   private VvTextBox vvtb_PCK_theSerno, vvtb_PCK_theSernoOp;

   public  VvHamper hamp_rbtBaza, hamp_cbxTbx;
   private RadioButton rbt_ovaPCKbaza, rbt_svePCKbaze, rbt_svePCKbazeAndKomp, rbt_komponenteOnly;
   private VvTextBox tbx_SkladCd, tbx_SkladOpis, tbx_RamKlasa, tbx_HddKlasa;

   public string LocalSkladCD;

   internal Artikl Artikl_rec;

   //public PCK_ArtiklList_Caller TheCaller;

   private System.Windows.Forms.Timer AutoRefreshTimer;

   #endregion Fieldz

   #region Constructor

   public PCK_ArtiklList_UC(Control _parent, string currArtiklCD, string currSkladCD/*, PCK_ArtiklList_Caller theCaller*/)
   {
      this.SuspendLayout();

      this.Parent = _parent;

      this.LocalSkladCD  = currSkladCD ;
    //this.TheCaller     = theCaller   ;

      if(currArtiklCD.NotEmpty()) Artikl_rec = Get_Artikl_FromVvUcSifrar(currArtiklCD);

      CreateHamperRbt();
      CreateHamperCbx();

      ThePCKBazeGrid = CreateTheGrid("ThePCKBazeGrid",                        ZXC.QunMrgn, /*ZXC.QunMrgn*/ hamp_rbtBaza.Bottom);
      ThePCKInfoGrid = CreateTheGrid("ThePCKInfoGrid", ThePCKBazeGrid.Right + ZXC.QunMrgn, /*ZXC.QunMrgn*/ hamp_rbtBaza.Bottom);
      TheSernoGrid   = CreateTheGrid("TheSernoGrid"  , ThePCKInfoGrid.Right + ZXC.QunMrgn, /*ZXC.QunMrgn*/ hamp_rbtBaza.Bottom);

      ThePCKInfoSumGrid = CreateSumGrid(ThePCKInfoGrid, ThePCKInfoGrid.Parent, "SUM" + ThePCKInfoGrid.Name);
      ThePCKBazeSumGrid = CreateSumGrid(ThePCKBazeGrid, ThePCKBazeGrid.Parent, "SUM" + ThePCKBazeGrid.Name);

      Initialize_SumGrid_Columns(ThePCKInfoGrid);
      Initialize_SumGrid_Columns(ThePCKBazeGrid);
    //GridLocationAndSize_Grids(ThePCKInfoGrid);

      CalcLocationAndSize();

      this.ResumeLayout();

      SetPKCColumnIndexes();

      SetPCKBazeColumnIndexes();
      SetSernoColumnIndexes();

    //ThePCKInfoGrid.CellMouseDoubleClick += ThePCKGrid_CellMouseDoubleClick_OpenSernoList;
      ThePCKInfoGrid.CellMouseClick       += ThePCKGrid_CellMouseClick_OpenSernoList      ;
      
      ThePCKInfoGrid.CellMouseDoubleClick += ThePCKInfoGrid_CellMouseDoubleClick_OpenArtiklUC;

      TheSernoGrid.CellMouseDoubleClick   += TheSernoGrid_CellMouseDoubleClick_OpenSernoInfoList;

      ThePCKBazeGrid.CellMouseClick       += ThePCKBazeGrid_CellMouseClick_OpenThisBazaOnlyList;

      //List<PCK_Artikl> PCKbazeList = RtranoDao.Get_PCK_ArtiklList_ByPCK_Baza_AndSklad(TheDbConnection, null, "", ZXC.PCK_Info_Kind.SveBazeOnly, false, false);
      //
      //Put_DGV_All_PCK_Baza_SintList(PCKbazeList);

      VvHamper.Open_Close_Fields_ForWriting(tbx_SkladCd , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_RamKlasa, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_HddKlasa, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);

      #region Auto Refresh Via Timer Tick

      //NeedsAutoRefresh = true; // todo odremarkiraj ovo kada slozis uvjet 

      if(/*NeedsAutoRefresh*/true) 
      {
         int seconds = 15; // todo 

         AutoRefreshTimer = new System.Windows.Forms.Timer();
         AutoRefreshTimer.Interval = seconds * 1000; // milliseconds 
         AutoRefreshTimer.Tick += new System.EventHandler(autoRefreshTimer_Tick);
         AutoRefreshTimer.Start();
      }

      #endregion Auto Refresh Via Timer Tick

   }

   private void autoRefreshTimer_Tick(object sender, EventArgs e)
   {
      //button_Go_Prev_Next_Action(this, EventArgs.Empty);
      //button_GO_Click(this, EventArgs.Empty);

      ShowPckinfo(null, EventArgs.Empty);
   }

   #endregion Constructor

   #region CalcLocationAndSize
   private void CalcLocationAndSize()
   {
      if(this.Parent is VvDialog) this.Size = new Size(ThePCKBazeGrid.Width + ThePCKInfoGrid.Width + 3 * ZXC.QunMrgn + TheSernoGrid.Width, SystemInformation.WorkingArea.Height - 2*ZXC.Q10un);
      else                        this.Size = new Size(ThePCKBazeGrid.Width + ThePCKInfoGrid.Width + 3 * ZXC.QunMrgn + TheSernoGrid.Width, SystemInformation.WorkingArea.Height - ZXC.Q10un - ZXC.Q5un);

      ThePCKBazeGrid.Height = this.Size.Height - ThePCKBazeSumGrid.Height - ZXC.Qun2 - hamp_rbtBaza.Bottom;
      ThePCKInfoGrid.Height = this.Size.Height - ThePCKInfoSumGrid.Height - ZXC.Qun2 - hamp_rbtBaza.Bottom;
      TheSernoGrid  .Height = this.Size.Height                            - ZXC.Qun2 - hamp_rbtBaza.Bottom;
      
      ThePCKInfoSumGrid.Width    = ThePCKInfoGrid.Width;
      ThePCKInfoSumGrid.Location = new Point(ThePCKInfoGrid.Location.X, ThePCKInfoGrid.Bottom + ZXC.Qun12);
      ThePCKInfoSumGrid.Anchor   = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

      ThePCKBazeSumGrid.Width    = ThePCKBazeGrid.Width;
      ThePCKBazeSumGrid.Location = new Point(ThePCKBazeGrid.Location.X, ThePCKBazeGrid.Bottom + ZXC.Qun12);
      ThePCKBazeSumGrid.Anchor   = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
   }

   #endregion CalcLocationAndSize

   #region hampers

   private void CreateHamperRbt()
   {
      hamp_rbtBaza = new VvHamper(4, 1, "", this, false);
      hamp_rbtBaza.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);

      hamp_rbtBaza.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q6un, ZXC.Q6un, ZXC.Q6un};
      hamp_rbtBaza.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamp_rbtBaza.VvRightMargin = 0;

      hamp_rbtBaza.VvRowHgt       = new int[] { ZXC.QUN };
      hamp_rbtBaza.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamp_rbtBaza.VvBottomMargin = hamp_rbtBaza.VvTopMargin;

      rbt_ovaPCKbaza        = hamp_rbtBaza.CreateVvRadioButton(0, 0,        new EventHandler(ShowPckinfo)  , "Ista PCK baza"   , TextImageRelation.ImageBeforeText);
      rbt_svePCKbaze        = hamp_rbtBaza.CreateVvRadioButton(1, 0,        new EventHandler(ShowPckinfo)  , "Sve PCK baze"    , TextImageRelation.ImageBeforeText);
      rbt_svePCKbazeAndKomp = hamp_rbtBaza.CreateVvRadioButton(2, 0,        new EventHandler(ShowPckinfo)  , "PCK i komponente", TextImageRelation.ImageBeforeText);
      rbt_komponenteOnly    = hamp_rbtBaza.CreateVvRadioButton(3, 0,        new EventHandler(ShowPckinfo)  , "Komponente"      , TextImageRelation.ImageBeforeText);
      
    //rbt_ovaPCKbaza.Checked = true;

   }
   /*private*/ public void ShowPckinfo(object sender, EventArgs e)
   {
      //switch(TheCaller)
      //{
      //   case PCK_ArtiklList_Caller.ArtiklListUC  :                                                                        break; // LocalSkladCD already ok 
      //   case PCK_ArtiklList_Caller.ArtiklUC      : this.LocalSkladCD = (ZXC.TheVvForm.TheVvUC as ArtiklUC).Fld_ZaSkladCD; break; 
      //   case PCK_ArtiklList_Caller.SubModulAction:                                                                        break; // LocalSkladCD already ok 
      //}

    //List<PCK_Artikl> PCK_ArtikList      = RtranoDao.Get_PCK_ArtiklList_ByPCK_Baza_AndSklad(ZXC.TheVvForm.TheDbConnection, this.Artikl_rec, this.LocalSkladCD, Fld_Pck_Info_kind            , Fld_IsIstaRamKlasa, Fld_IsIstaHddKlasa);
    //List<PCK_Artikl> PCK_SviArtikliList = RtranoDao.Get_PCK_ArtiklList_ByPCK_Baza_AndSklad(ZXC.TheVvForm.TheDbConnection, null           , this.LocalSkladCD, ZXC.PCK_Info_Kind.SveBazeOnly, false             , false             );
      List<PCK_Artikl> PCK_ArtikList      = RtranoDao.Get_PCK_ArtiklList_ByPCK_Baza_AndSklad(ZXC.TheVvForm.TheDbConnection, this.Artikl_rec, this.LocalSkladCD, Fld_Pck_Info_kind            , Fld_RamKlasa      , Fld_HddKlasa      );
      List<PCK_Artikl> PCK_SviArtikliList = RtranoDao.Get_PCK_ArtiklList_ByPCK_Baza_AndSklad(ZXC.TheVvForm.TheDbConnection, null           , this.LocalSkladCD, ZXC.PCK_Info_Kind.SveBazeOnly, ""                , ""  );



      PutDgvFields(PCK_ArtikList, PCK_SviArtikliList, Artikl_rec.ArtiklCD, this.LocalSkladCD);
   }

   private void CreateHamperCbx()
   {
      hamp_cbxTbx = new VvHamper(7, 1, "", this, false);
      hamp_cbxTbx.Location = new Point(hamp_rbtBaza.Right, ZXC.QunMrgn);

      hamp_cbxTbx.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q4un, ZXC.Q3un, ZXC.Q4un,ZXC.Q3un, ZXC.Q3un, ZXC.Q6un};
      hamp_cbxTbx.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamp_cbxTbx.VvRightMargin = 0;

      hamp_cbxTbx.VvRowHgt       = new int[] { ZXC.QUN };
      hamp_cbxTbx.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamp_cbxTbx.VvBottomMargin = hamp_cbxTbx.VvTopMargin;

      string RAMkindFilterLabel = "Samo " + Artikl_rec.Grupa2CD + " memorije";
      string HDDkindFilterLabel = "Samo " + Artikl_rec.Grupa3CD + " diskovi" ;

                     hamp_cbxTbx.CreateVvLabel        (0, 0, "RAM:", ContentAlignment.MiddleRight);
      tbx_RamKlasa = hamp_cbxTbx.CreateVvTextBoxLookUp(1, 0, "tbx_RamKlasa", "RamKlasa");
                     hamp_cbxTbx.CreateVvLabel        (2, 0, "HDD:", ContentAlignment.MiddleRight);
      tbx_HddKlasa = hamp_cbxTbx.CreateVvTextBoxLookUp(3, 0, "tbx_HddKlasa", "HddKlasa");

                      hamp_cbxTbx.CreateVvLabel        (4, 0, "Za Skl:", ContentAlignment.MiddleRight);
      tbx_SkladCd   = hamp_cbxTbx.CreateVvTextBoxLookUp(5, 0, "tbx_SkladCd", "Skladište");
      tbx_SkladOpis = hamp_cbxTbx.CreateVvTextBox      (6, 0, "tbx_SkladOpiS", "");
      tbx_SkladCd.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_SkladOpis.JAM_ReadOnly = true;

      tbx_RamKlasa.JAM_Set_LookUpTable(ZXC.luiListaGrupa2Artikla, (int)ZXC.Kolona.prva);
      tbx_HddKlasa.JAM_Set_LookUpTable(ZXC.luiListaGrupa3Artikla, (int)ZXC.Kolona.prva);
      
      tbx_SkladCd.JAM_Set_LookUpTable(ZXC.luiListaSkladista, (int)ZXC.Kolona.prva);
      tbx_SkladCd.JAM_lui_NameTaker_JAM_Name = tbx_SkladOpis.JAM_Name;

      tbx_SkladCd .JAM_FieldExitMethod_2 = new EventHandler(OnExit_SkladCD_GetPCKlistsAndPutDGVfields);
      tbx_RamKlasa.JAM_FieldExitMethod_2 = new EventHandler(OnExit_RamOrHddKlasa_GetPCKlistsAndPutDGVfields);
      tbx_HddKlasa.JAM_FieldExitMethod_2 = new EventHandler(OnExit_RamOrHddKlasa_GetPCKlistsAndPutDGVfields);
   }

   void OnExit_SkladCD_GetPCKlistsAndPutDGVfields(object sender, EventArgs e)
   {
      VvTextBox vvtb = sender as VvTextBox;

      string newSkladCD = vvtb.Text;

    //this.TheCaller    = PCK_ArtiklList_Caller.SubModulAction;
      this.LocalSkladCD = newSkladCD;

      ShowPckinfo(null, EventArgs.Empty);
   }

   void OnExit_RamOrHddKlasa_GetPCKlistsAndPutDGVfields(object sender, EventArgs e)
   {
      ShowPckinfo(null, EventArgs.Empty);
   }

   #endregion hampers

   #region TheGrid

   private VvDataGridView CreateTheGrid(string name, int x, int y)
   {
      VvDataGridView theGrid = new VvDataGridView();
      theGrid.Name     = name;
      theGrid.Parent   = this;
      theGrid.Location = new Point(x, y);

      theGrid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
      theGrid.AutoGenerateColumns                  = false;

      theGrid.RowHeadersBorderStyle = theGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
      theGrid.ColumnHeadersHeight   = ZXC.QUN - ZXC.Qun8;
      theGrid.RowTemplate.Height    = ZXC.QUN - ZXC.Qun8;
      theGrid.RowHeadersWidth       = ZXC.Q3un;
      theGrid.Height                = theGrid.ColumnHeadersHeight + theGrid.RowTemplate.Height;

      theGrid.CellFormatting += new DataGridViewCellFormattingEventHandler(VvDocumentRecordUC.grid_CellFormatting_FormatVvDateTime);
      theGrid.Validating += new System.ComponentModel.CancelEventHandler(VvDocumentRecordUC.grid_Validating);

      theGrid.ReadOnly = true;

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

    //TheGrid.ColumnHeadersDefaultCellStyle.Font = ZXC.vvFont.BaseFont;
    //TheGrid.RowsDefaultCellStyle         .Font = ZXC.vvFont.BaseFont;
    //TheGrid.RowHeadersDefaultCellStyle   .Font = ZXC.vvFont.BaseFont;

      CreateColumn(theGrid);

      int sumoOfColumns = 0;
      foreach(DataGridViewColumn dc in theGrid.Columns)
      {
         sumoOfColumns += dc.Width;
      }

      theGrid.Width  = sumoOfColumns + theGrid.RowHeadersWidth + ZXC.QUN + ZXC.Qun4;
      theGrid.Height = this.Size.Height - ZXC.QUN;

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

      VvHamper.ApplyVVColorAndStyleTabCntrolChange(theSumGrid);
      VvHamper.Open_Close_Fields_ForWriting(theSumGrid, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);


      theSumGrid.AllowUserToAddRows       =
      theSumGrid.AllowUserToDeleteRows    =
      theSumGrid.AllowUserToOrderColumns  =
      theSumGrid.AllowUserToResizeColumns =
      theSumGrid.AllowUserToResizeRows    = false;

      theSumGrid.RowHeadersDefaultCellStyle.Alignment = theGrid.RowHeadersDefaultCellStyle.Alignment;
      theSumGrid.RowTemplate.Height = theGrid.RowTemplate.Height;

      theSumGrid.ScrollBars = ScrollBars.None;
      theSumGrid.ClearSelection();

      //theSumGrid.Location = new Point(theGrid.Left, theGrid.Bottom);

      return theSumGrid;
   }

   private void GridLocationAndSize_Grids(VvDataGridView theGrid)
   {
      DataGridView theSumGrid = theGrid.TheLinkedGrid_Sum;

      theGrid.Height      = this.Height - hamp_rbtBaza.Bottom - theSumGrid.Height - ZXC.Q10un;
      theSumGrid.Width    = theGrid.Width;
      theSumGrid.Location = new Point(theGrid.Location.X, theGrid.Bottom + ZXC.Qun12);
      theSumGrid.Anchor   = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

      theGrid.Scroll += new ScrollEventHandler(Grid_VScroll);

      TheSernoGrid.Height = this.Height - hamp_rbtBaza.Bottom - ZXC.Q2un;
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

         gridSumColumn.DefaultCellStyle.Font = ZXC.vvFont.LargeBoldFont;

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
      if(theGrid.Name == "ThePCKInfoGrid")
      { 
         vvtb_PCK_ArtCD   = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (   "vvtb_PCK_ArtCD"  , null, -12, "Šifra"    ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_ArtCD   , null, "R_PCK_ArtCD"  , "Šifra"    , ZXC.Q6un           ); vvtb_PCK_ArtCD   .JAM_ReadOnly = true; 
         vvtb_PCK_ArtName = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (   "vvtb_PCK_ArtName", null, -12, "Naziv"    ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_ArtName , null, "R_PCK_ArtName", "Naziv"    , ZXC.Q3un           ); vvtb_PCK_ArtName .JAM_ReadOnly = true; colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; colVvText.MinimumWidth = ZXC.Q10un + ZXC.Qun5;
         vvtb_PCK_RAMkind = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (   "vvtb_PCK_RAMkind", null, -12, "RAM Klasa"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_RAMkind , null, "R_PCK_RAMkind", "Mem"      , ZXC.Q3un           ); vvtb_PCK_RAMkind .JAM_ReadOnly = true;
         vvtb_PCK_HDDkind = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (   "vvtb_PCK_HDDkind", null, -12, "HDD Klasa"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_HDDkind , null, "R_PCK_HDDkind", "Disk"     , ZXC.Q3un           ); vvtb_PCK_HDDkind .JAM_ReadOnly = true;
         vvtb_PCK_SklCD   = theGrid.CreateVvTextBoxFor_String_ColumnTemplate (   "vvtb_PCK_SklCD"  , null, -12, "Skladište"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_SklCD   , null, "R_PCK_SklCD"  , "Sklad"    , ZXC.Q3un - ZXC.Qun2); vvtb_PCK_SklCD   .JAM_ReadOnly = true;
         vvtb_PCK_RAM     = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(0, "vvtb_PCK_RAM"    , null, -12, "RAM"      ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_RAM     , null, "R_PCK_RAM"    , "RAM"      , ZXC.Q4un           ); vvtb_PCK_RAM     .JAM_ReadOnly = true; colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont; colVvText.DefaultCellStyle.ForeColor = ZXC.vvColors.clr_RAM_PTG;
         vvtb_PCK_HDD     = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(0, "vvtb_PCK_HDD"    , null, -12, "HDD"      ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_HDD     , null, "R_PCK_HDD"    , "HDD"      , ZXC.Q4un           ); vvtb_PCK_HDD     .JAM_ReadOnly = true; colVvText.DefaultCellStyle.Font = ZXC.vvFont.BaseBoldFont; colVvText.DefaultCellStyle.ForeColor = ZXC.vvColors.clr_HDD_PTG;
         vvtb_StanjeKol   = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(0, "vvtb_StanjeKol"  , null, -12, "StanjeKol"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_StanjeKol   , null, "R_StanjeKol"  , "Stanje"   , ZXC.Q4un - ZXC.Qun2); vvtb_StanjeKol   .JAM_ReadOnly = true;

         colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);

         vvtb_PCK_RAM.JAM_ForeColor = ZXC.vvColors.clr_RAM_PTG;
         vvtb_PCK_HDD.JAM_ForeColor = ZXC.vvColors.clr_HDD_PTG;
      }
      else if(theGrid.Name == "ThePCKBazeGrid")
      {
         vvtb_PCK_BazaName  = theGrid.CreateVvTextBoxFor_String_ColumnTemplate(    "vvtb_PCK_BazaName" , null, -12, "PCK Baza" ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_BazaName , null, "R_PCK_BazaName" , "PCK Baza", ZXC.Q3un           ); vvtb_PCK_BazaName .JAM_ReadOnly = true; colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; colVvText.MinimumWidth = ZXC.Q5un;
         vvtb_PCK_BazaSklCD = theGrid.CreateVvTextBoxFor_String_ColumnTemplate(    "vvtb_PCK_BazaSklCD", null, -12, "Skladište"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_BazaSklCD, null, "R_PCK_BazaSklCD", "Sklad"   , ZXC.Q3un - ZXC.Qun2); vvtb_PCK_BazaSklCD.JAM_ReadOnly = true;
         vvtb_PCK_BazeStKol = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate(0, "vvtb_PCK_BazeStKol", null, -12, "StanjeKol"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_BazeStKol, null, "R_BazeStanjeKol", "Stanje"  , ZXC.Q4un - ZXC.Qun2); vvtb_PCK_BazeStKol.JAM_ReadOnly = true;

         colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);

      }
      else
      {
         vvtb_PCK_theSerno   = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_PCK_theSerno"  , null, -12, "Serijski broj"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_theSerno  , null, "R_PCK_Serno"  , "Serijski broj", ZXC.Q6un); vvtb_PCK_theSerno  .JAM_ReadOnly = true;
         vvtb_PCK_theSernoOp = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_PCK_theSernoOp", null, -12, "Opis"         ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_PCK_theSernoOp, null, "R_PCK_SernoOp", "Opis"         , ZXC.Q6un); vvtb_PCK_theSernoOp.JAM_ReadOnly = true;

         colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);
      }
   }

   #endregion TheGridColumn

   #region SetColumnIndexes()

   private PCKInfo_colIdx ci;
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

      ci.iT_PCK_ArtCD   = ThePCKInfoGrid.IdxForColumn("R_PCK_ArtCD"  );
      ci.iT_PCK_ArtName = ThePCKInfoGrid.IdxForColumn("R_PCK_ArtName");
      ci.iT_PCK_RAMkind = ThePCKInfoGrid.IdxForColumn("R_PCK_RAMkind");
      ci.iT_PCK_HDDkind = ThePCKInfoGrid.IdxForColumn("R_PCK_HDDkind");
      ci.iT_PCK_SklCD   = ThePCKInfoGrid.IdxForColumn("R_PCK_SklCD"  );
      ci.iT_PCK_RAM     = ThePCKInfoGrid.IdxForColumn("R_PCK_RAM"    );
      ci.iT_PCK_HDD     = ThePCKInfoGrid.IdxForColumn("R_PCK_HDD"    );
      ci.iT_UkPstKol    = ThePCKInfoGrid.IdxForColumn("R_UkPstKol"   );
      ci.iT_UkUlazKol   = ThePCKInfoGrid.IdxForColumn("R_UkUlazKol"  );
      ci.iT_UkIzlazKol  = ThePCKInfoGrid.IdxForColumn("R_UkIzlazKol" );
      ci.iT_StanjeKol   = ThePCKInfoGrid.IdxForColumn("R_StanjeKol"  );
   }

   private Serno_colIdx ci2;
   public struct Serno_colIdx
   {
      internal int iT_PCK_theSerno;
      internal int iT_PCK_theSernoOp;
      internal int iT_PCK_artiklCD;
   }
   public void SetSernoColumnIndexes()
   {
      ci2 = new Serno_colIdx();
      ci2.iT_PCK_theSerno   = TheSernoGrid.IdxForColumn("R_PCK_Serno"  );
      ci2.iT_PCK_theSernoOp = TheSernoGrid.IdxForColumn("R_PCK_SernoOp");
      ci2.iT_PCK_artiklCD   = TheSernoGrid.IdxForColumn("R_PCK_ArtCD");
   }

   private PCKBaze_colIdx ci3;
   public struct PCKBaze_colIdx
   {
      internal int iT_PCK_BazeName ;
      internal int iT_PCK_BazeSklCD;
      internal int iT_StanjeBazeKol;
   }
   public void SetPCKBazeColumnIndexes()
   {
      ci3 = new PCKBaze_colIdx();

      ci3.iT_PCK_BazeName  = ThePCKBazeGrid.IdxForColumn("R_PCK_BazaName" );
      ci3.iT_PCK_BazeSklCD = ThePCKBazeGrid.IdxForColumn("R_PCK_BazaSklCD");
      ci3.iT_StanjeBazeKol = ThePCKBazeGrid.IdxForColumn("R_BazeStanjeKol");
   }


   #endregion SetColumnIndexes()

   #region Fld

   public ZXC.PCK_Info_Kind Fld_Pck_Info_kind
   {
      get
      {
              if(rbt_ovaPCKbaza       .Checked) return ZXC.PCK_Info_Kind.OvaBazaOnly       ;
         else if(rbt_svePCKbaze       .Checked) return ZXC.PCK_Info_Kind.SveBazeOnly       ;
         else if(rbt_svePCKbazeAndKomp.Checked) return ZXC.PCK_Info_Kind.SveBazeIkomponente;
         else if(rbt_komponenteOnly   .Checked) return ZXC.PCK_Info_Kind.KomponenteOnly    ;
         else                                   return ZXC.PCK_Info_Kind.OvaBazaOnly       ;
      }
      set
      {
         switch(value)
         {
            case ZXC.PCK_Info_Kind.OvaBazaOnly       : rbt_ovaPCKbaza       .Checked = true; break;
            case ZXC.PCK_Info_Kind.SveBazeOnly       : rbt_svePCKbaze       .Checked = true; break;
            case ZXC.PCK_Info_Kind.SveBazeIkomponente: rbt_svePCKbazeAndKomp.Checked  = true; break;
            case ZXC.PCK_Info_Kind.KomponenteOnly    : rbt_komponenteOnly   .Checked  = true; break;
         }
      }
   }

   public bool Fld_IsIstaRamKlasa { get { return false /*cbx_RamKlasa.Checked*/; } /*set { cbx_RamKlasa.Checked = value; } */}
   public bool Fld_IsIstaHddKlasa { get { return false /*cbx_HddKlasa.Checked*/; } /*set { cbx_HddKlasa.Checked = value; } */}

 //public string Fld_CurrArtikl { get { return PCK_ArtCD + " [" + PCK_ArtName + "]" + " [" + PCK_RAMkind + "]" + " RAM: " + PCK_RAM.ToString0Vv() + "Gb [" + PCK_HDDkind + "] HDD: " + PCK_HDD.ToString0Vv() + " Gb"; ; } }
   public string Fld_CurrArtikl { get { return Artikl_rec.ArtiklCD + " " + Artikl_rec.ArtiklName; } }
   public string Fld_SkladCD
   {
      get
      {
         return tbx_SkladCd.Text;
      }
      set
      {
         tbx_SkladCd.Text = value;
      }
   }
   public string Fld_SkladOpis
   {
      get
      {
         return tbx_SkladOpis.Text;
      }
      set
      {
         tbx_SkladOpis.Text = value;
      }
   }
   public string Fld_RamKlasa { get { return tbx_RamKlasa.Text; } set { tbx_RamKlasa.Text = value; } }
   public string Fld_HddKlasa { get { return tbx_HddKlasa.Text; } set { tbx_HddKlasa.Text = value; } }
   #endregion Fld

   #region PutDgvFields
   public void PutDgvFields(List<PCK_Artikl> _PCK_ArtiklList, List<PCK_Artikl> _PCK_SviArtikliList, string initial_PCK_artikl_cd, string initial_skladCD)
   {
      Fld_SkladCD   = initial_skladCD;
      Fld_SkladOpis = ZXC.luiListaSkladista.GetNameForThisCd(initial_skladCD);

      #region srednji, drugi grid (ThePCKInfoGrid - _PCK_Lines)

      this.PCK_ArtiklList = _PCK_ArtiklList;

      int rowIdx;

      ThePCKInfoGrid.Rows.Clear();
      TheSernoGrid  .Rows.Clear();

      if(_PCK_ArtiklList != null)
      {
         for(rowIdx = 0; rowIdx < _PCK_ArtiklList.Count; ++rowIdx)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            ThePCKInfoGrid.Rows.Add();

            PutDgvLineFields(rowIdx, _PCK_ArtiklList[rowIdx]);

            ThePCKInfoGrid.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();

            if(initial_PCK_artikl_cd.NotEmpty() && _PCK_ArtiklList[rowIdx].PCK_ArtCD == initial_PCK_artikl_cd)
            {
               ThePCKInfoGrid.ClearSelection();

               ThePCKInfoGrid.Rows[rowIdx].Selected = true;

               //ThePCKInfoGrid.CurrentCell. performclick

               ThePCKGrid_CellMouseClick_OpenSernoList(ThePCKInfoGrid, new DataGridViewCellMouseEventArgs(0, rowIdx, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)));
            }
            if(initial_PCK_artikl_cd.IsEmpty() && rowIdx == 0) 
            {
               ThePCKGrid_CellMouseClick_OpenSernoList(ThePCKInfoGrid, new DataGridViewCellMouseEventArgs(0, rowIdx, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)));
            }
         }
      }

      PutDgvSumFields(_PCK_ArtiklList);

      ThePCKInfoSumGrid.ClearSelection();

      #endregion srednji, drugi grid (ThePCKInfoGrid - _PCK_Lines)

      #region lijevi, prvi grid (ThePCKBazeGrid - _PCK_Lines)

      // grupiraj po PCK_BazaCD + PCK_SklCD 
      this.PCK_BazeSintList = _PCK_SviArtikliList.GroupBy(pck => pck.PCK_BazaCD + pck.PCK_SklCD).Select(npck => new PCK_Artikl(npck.First().PCK_ArtCD, npck.First().PCK_SklCD, npck.Sum(qwe => qwe.StanjeKol))).ToList();

      ThePCKBazeGrid.Rows.Clear();

      if(this.PCK_BazeSintList != null)
      {
         for(rowIdx = 0; rowIdx < this.PCK_BazeSintList.Count; ++rowIdx)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            ThePCKBazeGrid.Rows.Add();

            PutDgvPCKbazeLineFields(rowIdx, this.PCK_BazeSintList[rowIdx]);

            ThePCKBazeGrid.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();

            if(initial_PCK_artikl_cd.NotEmpty() && PCK_BazeSintList[rowIdx].PCK_BazaCD == Artikl.Get_PCK_BazaCD(initial_PCK_artikl_cd))
            {
               ThePCKBazeGrid.ClearSelection();

               ThePCKBazeGrid.Rows[rowIdx].Selected = true;
            }

            if(initial_PCK_artikl_cd.IsEmpty() && PCK_BazeSintList[rowIdx].PCK_BazaCD == this.Artikl_rec.PCK_BazaCD && PCK_BazeSintList[rowIdx].PCK_SklCD == this.Artikl_rec.SkladCD)
            {
               ThePCKBazeGrid.ClearSelection();

               ThePCKBazeGrid.Rows[rowIdx].Selected = true;
            }
         }
      }

      PutDgvPCKbazeSumFields(this.PCK_BazeSintList);

      ThePCKBazeSumGrid.ClearSelection();

      #endregion lijevi, prvi grid (ThePCKBazeGrid - _PCK_Lines)

   }

   private void PutDgvLineFields(int rowIdx, PCK_Artikl _PCK_Line)
   {
      ThePCKInfoGrid.PutCell(ci.iT_PCK_ArtCD   , rowIdx, _PCK_Line.PCK_ArtCD  );
      ThePCKInfoGrid.PutCell(ci.iT_PCK_ArtName , rowIdx, _PCK_Line.PCK_ArtName);
      ThePCKInfoGrid.PutCell(ci.iT_PCK_RAMkind , rowIdx, _PCK_Line.PCK_RAMkind);
      ThePCKInfoGrid.PutCell(ci.iT_PCK_HDDkind , rowIdx, _PCK_Line.PCK_HDDkind);
      ThePCKInfoGrid.PutCell(ci.iT_PCK_SklCD   , rowIdx, _PCK_Line.PCK_SklCD  );
      ThePCKInfoGrid.PutCell(ci.iT_PCK_RAM     , rowIdx, _PCK_Line.PCK_RAM    );
      ThePCKInfoGrid.PutCell(ci.iT_PCK_HDD     , rowIdx, _PCK_Line.PCK_HDD    );
    //TheGrid.PutCell(ci.iT_UkPstKol           , rowIdx, PCK_Line.UkPstKol    );
    //TheGrid.PutCell(ci.iT_UkUlazKol          , rowIdx, PCK_Line.UkUlazKol   );
    //TheGrid.PutCell(ci.iT_UkIzlazKol         , rowIdx, PCK_Line.UkIzlazKol  );
      ThePCKInfoGrid.PutCell(ci.iT_StanjeKol   , rowIdx, _PCK_Line.StanjeKol  );
   }

   private void PutDgvPCKbazeLineFields(int rowIdx, PCK_Artikl _PCK_Line)
   {
      ThePCKBazeGrid.PutCell(ci3.iT_PCK_BazeName , rowIdx, _PCK_Line.PCK_BazaCD);
      ThePCKBazeGrid.PutCell(ci3.iT_PCK_BazeSklCD, rowIdx, _PCK_Line.PCK_SklCD  );
      ThePCKBazeGrid.PutCell(ci3.iT_StanjeBazeKol, rowIdx, _PCK_Line.StanjeKol  );
   }

   private void PutDgvSumFields(List<PCK_Artikl> _PCK_Lines)
   {
      ThePCKInfoSumGrid.PutCell(ci.iT_PCK_RAM  , 0, _PCK_Lines.Sum(pck => pck.PCK_RAM  ));
      ThePCKInfoSumGrid.PutCell(ci.iT_PCK_HDD  , 0, _PCK_Lines.Sum(pck => pck.PCK_HDD  ));
      ThePCKInfoSumGrid.PutCell(ci.iT_StanjeKol, 0, _PCK_Lines.Sum(pck => pck.StanjeKol));
   }

   private void PutDgvPCKbazeSumFields(List<PCK_Artikl> _PCK_Lines)
   {
      ThePCKBazeSumGrid.PutCell(ci3.iT_StanjeBazeKol, 0, _PCK_Lines.Sum(pck => pck.StanjeKol));
   }

   public void PutDgv2Fields(/*List<PCK_Unikat>*/ List</*string*/(string serno, string opis)> theSernoAndOpisList)
   {
      int rowIdx;

      TheSernoGrid.Rows.Clear();

      if(theSernoAndOpisList != null)
      {
         for(rowIdx = 0; rowIdx < theSernoAndOpisList.Count; ++rowIdx)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheSernoGrid.Rows.Add();

            TheSernoGrid.PutCell(ci2.iT_PCK_theSerno  , rowIdx, theSernoAndOpisList[rowIdx].serno);
            TheSernoGrid.PutCell(ci2.iT_PCK_theSernoOp, rowIdx, theSernoAndOpisList[rowIdx].opis );

            TheSernoGrid.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();
         }
      }

      TheSernoGrid.ClearSelection();
   }

   #endregion PutDgvFields

   #region MouseDouble/Click
 //private void ThePCKGrid_CellMouseDoubleClick_OpenSernoList(object sender, DataGridViewCellMouseEventArgs e)
   private void ThePCKGrid_CellMouseClick_OpenSernoList(object sender, DataGridViewCellMouseEventArgs e)
   {
      VvDataGridView theG     = sender as VvDataGridView        ;
      PCK_ArtiklList_UC theUC = theG.Parent as PCK_ArtiklList_UC;

      theUC.TheSernoGrid.Rows.Clear();
      
      int rowIdx = e.RowIndex;
     
      if(rowIdx.IsNegative()) return;
     
      PCK_Artikl thePCK_Artikl = PCK_ArtiklList[rowIdx];

      if(Get_Artikl_FromVvUcSifrar(thePCK_Artikl.PCK_ArtCD).TS != ZXC.PCK_TS) return;

      List<string> theSernoList = RtranoDao.GetDistinctRtranoSernoForArtiklAndSklad(ZXC.TheVvForm.TheDbConnection, thePCK_Artikl.PCK_ArtCD, thePCK_Artikl.PCK_SklCD);

      List<(string serno, string opis)> theSernoAndOpisList = new List<(string serno, string opis)>();

      Rtrano last_rtrano_rec_forThisSerno;
      bool thisSernoHas_BadArtiklCD_or_SkladCD;

      // sada treba izbaciti one kojima zadnje stanje nije kao ovaj artikl 
      for(int i = 0; i < theSernoList.Count; ++i)
      {
         last_rtrano_rec_forThisSerno = new Rtrano();

         RtranoDao.Get_LastRtrano_ForSerno(ZXC.TheVvForm.TheDbConnection, last_rtrano_rec_forThisSerno, theSernoList[i]);

         thisSernoHas_BadArtiklCD_or_SkladCD = last_rtrano_rec_forThisSerno.T_artiklCD != thePCK_Artikl.PCK_ArtCD || last_rtrano_rec_forThisSerno.T_skladCD != thePCK_Artikl.PCK_SklCD;

         if(thisSernoHas_BadArtiklCD_or_SkladCD)
         {
            theSernoList.RemoveAt(i--);
         }
         else // dojebi t_opis sa Rtrano-a 
         {
            theSernoAndOpisList.Add((last_rtrano_rec_forThisSerno.T_serno, last_rtrano_rec_forThisSerno.T_grCD));
         }
      }

      theUC.PutDgv2Fields(/*thePCK_Artikl.PCK_Unikat_List*/ /*theSernoList*/ theSernoAndOpisList);

   }

   private void TheSernoGrid_CellMouseDoubleClick_OpenSernoInfoList(object sender, DataGridViewCellMouseEventArgs e)
   {
      VvDataGridView theG = sender as VvDataGridView;

      int rowIdx = e.RowIndex;

      if(rowIdx.IsNegative()) return;

      string theSerno = theG.GetStringCell(ci2.iT_PCK_theSerno, rowIdx, false);

      // =============================== 
      ZXC.TheVvForm.OpenNew_RecLst_TabPage(ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.LsRTO), null);

      RtranoListUC rtranoListUC = ZXC.TheVvForm.TheVvRecLstUC as RtranoListUC;
      rtranoListUC.Fld_SerNo    = theSerno;

      rtranoListUC.button_GO.PerformClick();
      // =============================== 
   }

   private void ThePCKInfoGrid_CellMouseDoubleClick_OpenArtiklUC(object sender, DataGridViewCellMouseEventArgs e)
   {
      VvDataGridView theG = sender as VvDataGridView;

      int rowIdx = e.RowIndex;

      if(rowIdx.IsNegative()) return;

      //if(e.ColumnIndex != ci.iT_PCK_ArtCD) return;

      string artiklCD   = theG.GetStringCell(ci.iT_PCK_ArtCD, rowIdx, false);
      
      if(artiklCD.IsEmpty()) return; // znaci da smo u zutome probali doubleclickom inicirati editiranje cell-a 

      //Artikl artikl_rec = new Artikl();
      //artikl_rec.VvDao.SetMe_Record_bySomeUniqueColumn(conn, artikl_rec, artiklCD, ZXC.ArtiklSchemaRows[ZXC.ArtCI.artiklCD], false);

      SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

    // 18.08.2011: bijo BUG! Kako je reference type, saljuci element ArtiklSifrar-a kao TheVvDataRecord, svaka promkena TheVvDataRecord-a je mijenjala i element ArtiklSifrar-a! 
    //Artikl artikl_rec = ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artiklCD);

      Artikl artikl_rec;
      try
      {
         artikl_rec = ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artiklCD).MakeDeepCopy();
      }
      catch(Exception ex)
      {
         artikl_rec = null;
      }

      if(artikl_rec != null)
      {
         ZXC.TheVvForm.OpenNew_Record_TabPage_wInitialRecord(ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.ART), artikl_rec);
      }
   }

   private void ThePCKBazeGrid_CellMouseClick_OpenThisBazaOnlyList(object sender, DataGridViewCellMouseEventArgs e)
   {
      VvDataGridView theG     = sender as VvDataGridView        ;
      PCK_ArtiklList_UC theUC = theG.Parent as PCK_ArtiklList_UC;

      int rowIdx = e.RowIndex;

      if(rowIdx.IsNegative()) return;

      string selected_PCKbazaCD = PCK_BazeSintList[rowIdx].PCK_BazaCD;
      string selected_PCKsklCD  = PCK_BazeSintList[rowIdx].PCK_SklCD ;

      this.Artikl_rec = new Artikl() { PCK_BazaCD = selected_PCKbazaCD, SkladCD = selected_PCKsklCD };

      Fld_Pck_Info_kind = ZXC.PCK_Info_Kind.OvaBazaOnly;

      Fld_SkladCD       = this.LocalSkladCD = selected_PCKsklCD;

      ShowPckinfo(null, EventArgs.Empty);
   }

   public override void GetFields(bool dirtyFlagging)
   {
      throw new NotImplementedException();
   }

   #endregion MouseDouble/Click

   public static string GetFirstActivePCKartiklCD(XSqlConnection conn, string skladCD, string PCK_baza)
   {
      bool success = true;

      string artiklCD;

      using(XSqlCommand cmd = VvSQL.GetFirstActivePCKartiklCD_Command(conn, skladCD, PCK_baza))
      {
         using(MySql.Data.MySqlClient.MySqlDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleResult | System.Data.CommandBehavior.SingleRow))
         {
            success = reader.HasRows;

            if(reader.Read())
            {
               artiklCD = reader.GetString(0);
               reader.Close();
            }
            else
            {
               artiklCD = "" /*"Unknown?"*/;
            }
         } // using reader 
      } // using cmd 

      return artiklCD;
   }

}

public class VvModificiraj_PTG_Dlg : VvDialog
{
   #region Filedz

   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_komada;

   #endregion Filedz

   #region Constructor

   public VvModificiraj_PTG_Dlg(string currArtiklCD)
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Modificiraj " + currArtiklCD + " PCK artikle";

      CreateHamper();

      dlgWidth        = hamper.Right + ZXC.QunMrgn;
      dlgHeight       = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_komada, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
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

                     hamper.CreateVvLabel  (0, 0, "Komada:", ContentAlignment.MiddleRight);
      tbx_komada = hamper.CreateVvTextBox(1, 0, "tbx_kolicina", "", 4);
      tbx_komada.JAM_CharEdits = ZXC.JAM_CharEdits.NumericOnly;

   }

   #endregion hamper

   #region Button_Click

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Button_Click

   #region Fld_

   public decimal Fld_PTG_MODkomada { get { return tbx_komada.GetDecimalField(); } set { tbx_komada.PutDecimalField(value); } }

   #endregion Fld_

}

public class MOD_PTG_DUC : FakturPDUC
{
   #region Fieldz

   public  VvHamper  hamp_klase, hamp_pckBaza, hamp_ugando, hamp_semafor;
   private VvTextBox tbx_pckBaza, tbx_ramKlasa, tbx_hddKlasa;
   public  Label     lbl_MOC, lbl_SER, lbl_RAM, lbl_HDD;

   #endregion Fieldz

   #region Constructor

   public MOD_PTG_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
      (Faktur.tt_colName, new string[]
      {
         Faktur.TT_MOD 
      });

      ThePolyGridTabControl.SelectedIndex = 1;

      ThePolyGridTabControl.SelectionChanged += ThePolyGridTabControl_SelectionChanged_SupressSelectingDisabledTabs;

      TheSumGrid.Visible = false;

      TheG2.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(OnEC_Showing_DisableInput);

      bool isClearZero = Fld_PrjArtCD.IsEmpty();
      tbx_decimal01.JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, isClearZero);
      tbx_decimal02.JAM_MarkAsNumericTextBox(0, true, decimal.MaxValue, decimal.MinValue, isClearZero);
   }

   private void OnEC_Showing_DisableInput(object sender, DataGridViewEditingControlShowingEventArgs e)
   {
      Control      control  = e.Control;
      DataGridView theG2    = sender as VvDataGridView;

      if(control is VvTextBox)
      {
       //VvTextBox vvtbTalon     = theG2.Columns[theG2.CurrentCell.ColumnIndex].Tag as VvTextBox;
         VvTextBox currVvTextBox = theG2.EditingControl                             as VvTextBox;

         int rowIdx = theG2.CurrentCell.RowIndex   ;
         int colIdx = theG2.CurrentCell.ColumnIndex;

         Rtrano rtrano_rec = (Rtrano)GetDgvLineFields2(rowIdx, false, null);

         bool isMOCrowIdx = ThisIs_MOC_rowIndex(rowIdx);

         #region some util bools

         bool isMOC_MOS_row = rtrano_rec.T_TT == Faktur.TT_MOC || rtrano_rec.T_TT == Faktur.TT_MOS;
         bool isMOI_MOU_row = rtrano_rec.T_TT == Faktur.TT_MOI || rtrano_rec.T_TT == Faktur.TT_MOU;

         bool isKol_col                = colIdx == DgvCI2.iT_kol  ;
         bool isSerno_col              = colIdx == DgvCI2.iT_serno;
         bool isRAM_HDD_plus_minus_col = colIdx == DgvCI2.iT_RAM_plus || colIdx == DgvCI2.iT_RAM_minus || colIdx == DgvCI2.iT_HDD_plus || colIdx == DgvCI2.iT_HDD_minus;

         #endregion some util bools

         if(isMOCrowIdx && rtrano_rec.T_serno.IsEmpty() && colIdx == DgvCI2.iR_artiklCD_Old) currVvTextBox.ReadOnly = true; // Disable Input! 
         if(isMOI_MOU_row && (isSerno_col || isRAM_HDD_plus_minus_col)                     ) currVvTextBox.ReadOnly = true; // Disable Input! 
         if(isMOC_MOS_row &&  isKol_col                                                    ) currVvTextBox.ReadOnly = true; // Disable Input! 

      }
   }

   private void ThePolyGridTabControl_SelectionChanged_SupressSelectingDisabledTabs(Crownwood.DotNetMagic.Controls.TabControl theTabControl, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   {
      if(newPage.Enabled == false)
      {
         theTabControl.SelectedIndex = theTabControl.TabPages.IndexOf(oldPage); // vrati ga nazad 
      }
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      CrateHamperPCKbaza();
      CrateHamperKlasa();
      CreateHamperUGANDO();
      CreateHamperSemafor();

      panel_MigratorsLeftB.SendToBack();

      hamp_dokDate    .Location = new Point(                             0,                              0);
      hamp_tt         .Location = new Point(hamp_dokDate.Right + ZXC.Qun12,                              0);
      hamp_skladCd    .Location = new Point(hamp_tt          .Right,                              0);
      hamp_dokNum     .Location = new Point(hamp_skladCd     .Right,                              0);
      hamp_kupdobNaziv.Location = new Point(                      0, hamp_dokDate.Bottom           );
      hamp_ugando     .Location = new Point(hamp_kupdobNaziv.Right , hamp_dokDate.Bottom + ZXC.Qun8);
      hamp_napomena   .Location = new Point(                 0, hamp_ugando .Bottom + ZXC.Qun8);


      hamp_pckBaza   .Location = new Point(           labelWidth, hamp_napomena.Bottom + ZXC.Qun2);
      hamp_prjArtName.Location = new Point(hamp_pckBaza   .Right, hamp_napomena.Bottom + ZXC.Qun2);
      hamp_klase     .Location = new Point(hamp_prjArtName.Right, hamp_napomena.Bottom + ZXC.Qun2);
      hamp_decimal   .Location = new Point(hamp_klase     .Right, hamp_napomena.Bottom + ZXC.Qun2);
      hamp_someMoney .Location = new Point(hamp_decimal   .Right, hamp_napomena.Bottom + ZXC.Qun2);

      hamp_semafor   .Location = new Point(hamp_someMoney.Right + ZXC.Q2un, hamp_napomena.Top + ZXC.Qun4);

      nextY = hamp_prjArtName.Bottom + ZXC.QUN;
      
      hamp_IznosUvaluti.Visible = false;

      this.ControlForInitialFocus = tbx_Napomena; // tamara ... todo? 

      SetSumeHampers(false, false, false, false);

      hamp_twin.Visible = false;
   }
   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt, hamp_kupdobNaziv, //hamp_SkladDate,
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,// hamp_v1TT,
                                    hamp_prjArtName, hamp_decimal, hamp_someMoney
                                   };
   }

   private void CrateHamperPCKbaza()
   {
      hamp_pckBaza        = new VvHamper(1, 2, "", null, false);
      hamp_pckBaza.Parent = TheTabControl.TabPages[0];

      hamp_pckBaza.VvColWdt      = new int[] { ZXC.Q5un };
      hamp_pckBaza.VvSpcBefCol   = new int[] { ZXC.Qun8 };
      hamp_pckBaza.VvRightMargin = hamp_pckBaza.VvLeftMargin;

      hamp_pckBaza.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN + ZXC.Qun8 };
      hamp_pckBaza.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun10          };
      hamp_pckBaza.VvBottomMargin = hamp_pckBaza.VvTopMargin;

                    hamp_pckBaza.CreateVvLabel  (0, 0, "PCK baza", ContentAlignment.MiddleLeft);
      tbx_pckBaza = hamp_pckBaza.CreateVvTextBox(0, 1, "tbx_pckBaza", "", 16);
      tbx_pckBaza.JAM_ReadOnly = true;
      tbx_pckBaza.Font = ZXC.vvFont.BaseBoldFont;

   }

   private void CrateHamperKlasa()
   {
      hamp_klase = new VvHamper(2, 2, "", null, false);
      hamp_klase.Parent = TheTabControl.TabPages[0];

      hamp_klase.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un};
      hamp_klase.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8};
      hamp_klase.VvRightMargin = hamp_klase.VvLeftMargin;

      hamp_klase.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN + ZXC.Qun8 };
      hamp_klase.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun10          };
      hamp_klase.VvBottomMargin = hamp_klase.VvTopMargin;


      hamp_klase.CreateVvLabel(0, 0, "RAM klasa", ContentAlignment.MiddleRight);
      hamp_klase.CreateVvLabel(1, 0, "HDD klasa", ContentAlignment.MiddleRight);

      tbx_ramKlasa = hamp_klase.CreateVvTextBox(0, 1, "tbx_ramKlasa", "", 24);
      tbx_hddKlasa = hamp_klase.CreateVvTextBox(1, 1, "tbx_hddKlasa", "", 24);

      tbx_ramKlasa.JAM_ReadOnly = true;
      tbx_hddKlasa.JAM_ReadOnly = true;
   }

   private void CreateHamperUGANDO()
   {
      hamp_ugando        = new VvHamper(5, 1, "", null, false);
      hamp_ugando.Parent = TheTabControl.TabPages[0];

      hamp_ugando.VvColWdt      = new int[] { labelWidth, ZXC.Q3un - ZXC.Qun2, ZXC.Q5un, ZXC.Q4un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4 };
      hamp_ugando.VvSpcBefCol   = new int[] { faBefFirstCol, faBefCol, faBefCol, faBefCol, 0 };
      hamp_ugando.VvRightMargin = hamp_ugando.VvLeftMargin;

      hamp_ugando.VvRowHgt       = new int[] { ZXC.QUN  };
      hamp_ugando.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamp_ugando.VvBottomMargin = hamp_ugando.VvTopMargin;

      hamp_ugando.CreateVvLabel(0, 0, "UgAnDo:", ContentAlignment.MiddleRight);


      tbx_v1_tt     = hamp_ugando.CreateVvTextBoxLookUp(1, 0, "tbx_v1_tt"    , "", GetDB_ColumnSize(DB_ci.v1_tt));
      tbx_v1_ttOpis = hamp_ugando.CreateVvTextBox      (2, 0, "tbx_v1_ttOpis", "", 32);
      tbx_v1_ttNum  = hamp_ugando.CreateVvTextBox      (3, 0, "tbx_v1_ttNum" , "", GetDB_ColumnSize(DB_ci.v1_ttNum));


      btn_v1TT = hamp_ugando.CreateVvButton(4, 0, new EventHandler(GoTo_RISK_Dokument_Click/*GoTo_UGAN_Dokument_Click*/), "");

      btn_v1TT.Name = "v1_TT";
      btn_v1TT.FlatStyle = FlatStyle.Flat;
      btn_v1TT.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_v1TT.Image = VvIco.TriangleBlue16.ToBitmap();
      btn_v1TT.Tag = 1;
      btn_v1TT.TabStop = false;

      tbx_v1_tt.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_v1_tt.JAM_Set_NOTobligatory_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_v1_tt.JAM_lui_NameTaker_JAM_Name = tbx_v1_ttOpis.JAM_Name;
      tbx_v1_ttOpis.JAM_ReadOnly = true;

      tbx_v1_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_v1_tt.JAM_IsSupressTab = true;
      tbx_v1_ttNum.JAM_IsSupressTab = true;

   }

   private void CreateHamperSemafor()
   {
      hamp_semafor = new VvHamper(1, 4, "", null, false);
      hamp_semafor.Parent = TheTabControl.TabPages[0];

      hamp_semafor.VvColWdt      = new int[] { ZXC.Q4un};
      hamp_semafor.VvSpcBefCol   = new int[] { 0};
      hamp_semafor.VvRightMargin = hamp_semafor.VvLeftMargin;

      hamp_semafor.VvRowHgt       = new int[] { ZXC.QUN+ZXC.Qun12, ZXC.QUN + ZXC.Qun12, ZXC.QUN + ZXC.Qun12, ZXC.QUN + ZXC.Qun12 };
      hamp_semafor.VvSpcBefRow    = new int[] { 0, 0, 0, 0 };
      hamp_semafor.VvBottomMargin = hamp_semafor.VvTopMargin;

      lbl_MOC = hamp_semafor.CreateVvLabel(0, 1, "", ContentAlignment.MiddleCenter);
    //lbl_SER = hamp_semafor.CreateVvLabel(0, 1, "SerNo?!", ContentAlignment.MiddleCenter);
      lbl_RAM = hamp_semafor.CreateVvLabel(0, 2, "", ContentAlignment.MiddleCenter);
      lbl_HDD = hamp_semafor.CreateVvLabel(0, 3, "", ContentAlignment.MiddleCenter);

      lbl_MOC.BackColor = Color.Green;
    //lbl_SER.BackColor = Color.Green;
      lbl_RAM.BackColor = Color.Green;
      lbl_HDD.BackColor = Color.Green;

      lbl_MOC.ForeColor = Color.Yellow;
    //lbl_SER.ForeColor = Color.Yellow;
      lbl_RAM.ForeColor = Color.Yellow;
      lbl_HDD.ForeColor = Color.Yellow;
   }

   #endregion HamperLocation

   #region Fld

   public string Fld_PTG_MOC_PCK_ArtCD  { get { return                             (Fld_PrjArtCD); } }
   public string Fld_PTG_MOC_PCK_baseCD { get { return Artikl.Get_PCK_BazaCD(Fld_PrjArtCD); } }
   public int    Fld_PTG_MOC_RowCount   { get { return (int)Fld_someMoney; } }

   public string Fld_PTG_RamKlasa { get { return tbx_ramKlasa.Text; } set { tbx_ramKlasa.Text = value; } }
   public string Fld_PTG_HddKlasa { get { return tbx_hddKlasa.Text; } set { tbx_hddKlasa.Text = value; } }
   public string Fld_PTG_PCKbaza  { get { return tbx_pckBaza.Text ; } set { tbx_pckBaza.Text  = value; } }


   #endregion Fld

   #region TheG_Specific_Columns
   public override bool HasRtrans_SkladCD_Exposed     { get { return true; } }
   public override bool HasRtrano_SkladCD_Exposed     { get { return true; } }
   public override bool HasRtrano_TT_Exposed          { get { return true; } }
   public override bool IsRtransTT_MOD_kindDependable { get { return true; } }
   public override bool Is_Rtrans_Readonly            { get { return true; } }

   public DateTime Get_PTG_MOD_MAX_DokDate()
   { 
      string myUgAn_TT    = this.faktur_rec.V1_tt   ;
      uint   myUgAn_TtNum = this.faktur_rec.V1_ttNum;

      if(myUgAn_TT.IsEmpty() || myUgAn_TtNum.IsZero()) return DateTimePicker.MaximumDateTime;

      Faktur myUgAnFaktur_rec = new Faktur();

      bool found = FakturDao.SetMeFaktur(TheDbConnection, myUgAnFaktur_rec, myUgAn_TT, myUgAn_TtNum, false); 

      if(!found) return DateTimePicker.MaximumDateTime;

      return myUgAnFaktur_rec.DokDate; 
   }
   protected override void InitializeDUC_Specific_Columns()
   {
      TheG.ColumnHeadersHeight = ZXC.Q2un;

      T_TT_CreateColumnG1          (ZXC.Q2un,    true, "TT"       , "Tip dokumenta", false);
      T_artiklCD_CreateColumn      (ZXC.Q3un,    true, "Šifra"    , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(             true, "Naziv"    , "Naziv artikla ili proizvoljan opis");
      T_artiklTS_CreateColumn      (ZXC.Q2un,    true, "Tip"      , "Tip artikla");
      T_skladCD_CreateColumn       (ZXC.Q3un,    true, "Sklad"    , "Ulazno ili izlazno skladište");
      T_kol_CreateColumn           (ZXC.Q3un, 0, true, "Kol"      , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   , true, "JM"       , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q4un, 2, true, "Cijena"   , "Jedinična cijena");
      R_KCRM_CreateColumn          (ZXC.Q4un, 2, true, "Iznos"    , "Iznos");
   }

   #endregion TheG_Specific_Columns

   #region TheG2_Specific_Columns2

   protected override void InitializeDUC_Specific_Columns2()
   {
      TheG2.ColumnHeadersHeight = ZXC.Q2un;

      T_TT_CreateColumn             (ZXC.Q2un,         true, "TT"           , "Tip dokumenta"        );
      T_serno_CreateColumn          (ZXC.Q8un,         true, "Serijski broj", "Serijski broj artikla");

      R_PCK_baza_CreateColumn       (ZXC.Q4un         ,true, "PCK baza"     , "PCK baza"             );
      R_artiklTS_CreateColumn       (ZXC.Q2un,         true, "Tip"          , "Tip artikla"          );
      R_ramKlasa2_CreateColumn      (ZXC.Q3un-ZXC.Qun2,true, "RAM klasa"    , "RAM klasa"            );
      R_hddKlasa2_CreateColumn      (ZXC.Q3un-ZXC.Qun2,true, "HDD klasa"    , "RAM klasa"            );

      R_artiklCD2_OLD_CreateColumn  (ZXC.Q5un,         true, "Artikl OLD"   , "Šifra artikla"        );
      T_kolg2_CreateColumn          (ZXC.Q3un     , 0, true, "Kol"          , "Kolicina"             );
      R_ramOld2_CreateColumn        (ZXC.Q3un     , 0, true, "RAM OLD"      , "RAM OLD"              );
      R_hddOld2_CreateColumn        (ZXC.Q3un     , 0, true, "HDD OLD"      , "HDD OLD"              );
      R_grCD_OLD_CreateColumn       (ZXC.Q5un        , true, "Opis OLD"     , "Opis OLD"              );

      T_dimX_CreateColumn           (ZXC.Q3un     , 0, true, "RAM +"        , "RAM +"                );
      T_dimY_CreateColumn           (ZXC.Q3un     , 0, true, "RAM -"        , "RAM -"                );
      T_decA_CreateColumn           (ZXC.Q3un     , 0, true, "HDD +"        , "HDD +"                );
      T_decB_CreateColumn           (ZXC.Q3un     , 0, true, "HDD -"        , "HDD -"                );

      T_artiklCD2_CreateColumn      (ZXC.Q5un,         true, "Artikl NEW"   , "Šifra artikla"        );
      T_dimZ_CreateColumn           (ZXC.Q3un     , 0, true, "RAM NEW"      , "RAM NEW"              );
      T_decC_CreateColumn           (ZXC.Q3un     , 0, true, "HDD NEW"      , "HDD NEW"              );
      T_grCD_CreateColumn           (ZXC.Q5un,         true, "Opis  NEW"    , "Opis  NEW"     , false);
      T_skladCD2_CreateColumn       (ZXC.Q3un-ZXC.Qun2,true, "Sklad"        , "Izlazno skladište"    );

      T_artiklName2_CreateColumn   (ZXC.Q3un        , false, "Naziv"        , "Naziv artikla ili proizvoljan opis");

    //T_paletaNo_CreateColumn      (ZXC.Q3un        ,  true, "ModSt"        , "Osnovno stavka"                    );
   }

   #endregion TheG2_Specific_Columns2

   /// <summary>
   /// Primjer za tuple return value example
   /// </summary>
   /// <param name="MOD_RAM_saldo"></param>
   /// <param name="MOD_HDD_saldo"></param>
   /// <returns></returns>
   public override void OpenCloseForWriting_AdditionalAction_UCspecific(ZXC.WriteMode writeMode, bool isESC)
   {
      bool idemoUzuto   = writeMode != ZXC.WriteMode.None;
      bool idemoUbijelo = !idemoUzuto                    ;

      int rtranStabIdx = 0;
      int rtranOtabIdx = 1;

      if(idemoUzuto) ThePolyGridTabControl.SelectedIndex = rtranOtabIdx;

      for(int i = 0; i < ThePolyGridTabControl.TabPages.Count; ++i)
      {
         if(idemoUbijelo) ThePolyGridTabControl.TabPages[i].Enabled = true;
         else // idemoUzuto 
         {
            if(i == rtranOtabIdx) ThePolyGridTabControl.TabPages[i].Enabled = true ;
            else                  ThePolyGridTabControl.TabPages[i].Enabled = false;
         }
      }
   } // public override void OpenCloseForWriting_AdditionalAction_UCspecific(ZXC.WriteMode writeMode, bool isESC) 

   //ThePolyGridTabControl.TabPages[TabPageTitle2].Tag = ZXC.IsPCTOGO? Color.Beige
   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Beige, Color.Empty, Color.Beige);
   }

   public override bool IsPTG_DUC_wRtrano { get { return true; } }

   internal void SetRow_TT_and_Color_and_Calc_newRam_newHdd(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      int rowIdx = TheG2.CurrentRow.Index;
      int colIdx = TheG2.CurrentCell.ColumnIndex;

      bool isPCKartikl        = TheG2.GetStringCell(ci2.iT_artiklTS, rowIdx, false) == ZXC.PCK_TS;
      bool isKomponentaArtikl = !isPCKartikl;

      bool isPlusOrMinusCol =
           (colIdx == ci2.iT_RAM_plus  ||
            colIdx == ci2.iT_RAM_minus ||
            colIdx == ci2.iT_HDD_plus  ||
            colIdx == ci2.iT_HDD_minus) ;

      string oldArtiklCD = "";

      //if(isPCKartikl)
      {
         oldArtiklCD = TheG2.GetStringCell(ci2.iR_artiklCD_Old, rowIdx, false);
      }

      if(isPlusOrMinusCol && isPCKartikl)
      {
         decimal oldRAM   = TheG2.GetDecimalCell(ci2.iR_ramOld   , rowIdx, false);
         decimal plusRAM  = TheG2.GetDecimalCell(ci2.iT_RAM_plus , rowIdx, false);
         decimal minusRAM = TheG2.GetDecimalCell(ci2.iT_RAM_minus, rowIdx, false);
         decimal newRAM   = oldRAM + plusRAM - minusRAM;
         if(newRAM.IsNegative())
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "New RAM je NEGATIVAN!? Vraćam na nulu!");
            plusRAM  = 0M; TheG2.PutCell(ci2.iT_RAM_plus , rowIdx, 0M);
            minusRAM = 0M; TheG2.PutCell(ci2.iT_RAM_minus, rowIdx, 0M);
            newRAM   = oldRAM;
         }
         TheG2.PutCell(ci2.iT_RAM_new, rowIdx, newRAM);

         decimal oldHDD   = TheG2.GetDecimalCell(ci2.iR_hddOld   , rowIdx, false);
         decimal plusHDD  = TheG2.GetDecimalCell(ci2.iT_HDD_plus , rowIdx, false);
         decimal minusHDD = TheG2.GetDecimalCell(ci2.iT_HDD_minus, rowIdx, false);
         decimal newHDD   = oldHDD + plusHDD - minusHDD;
         if(newHDD.IsNegative())
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "New HDD je NEGATIVAN!? Vraćam na nulu!");
            plusHDD  = 0M; TheG2.PutCell(ci2.iT_HDD_plus , rowIdx, 0M);
            minusHDD = 0M; TheG2.PutCell(ci2.iT_HDD_minus, rowIdx, 0M);
            newHDD   = oldHDD;
         }
         TheG2.PutCell(ci2.iT_HDD_new, rowIdx, newHDD);


         //TheG2.PutCell(ci2.iT_artiklCD, rowIdx, Artikl.Get_PTG_CalculatedArtiklCD_From_SenderArtiklCD_NewRAM_NewHDD(oldArtiklCD, newRAM, newHDD));
         string newArtiklCD = Artikl.Get_PTG_CalculatedArtiklCD_From_SenderArtiklCD_NewRAM_NewHDD(oldArtiklCD, newRAM, newHDD);
         TheG2.PutCell(ci2.iT_artiklCD, rowIdx, newArtiklCD);
      }

      Rtrano rtrano_rec = (Rtrano)GetDgvLineFields2(rowIdx, false, null);

      string  MOC_wanted_ArtiklCD = Fld_PrjArtCD ;
      decimal MOC_wanted_NEW_RAM  = Fld_Decimal01;
      decimal MOC_wanted_NEW_HDD  = Fld_Decimal02;

      bool isMOC_PCK_base = Artikl.Has_equal_PCK_base(MOC_wanted_ArtiklCD, oldArtiklCD);

      string theTT = Get_MOD_RtranoTT(rowIdx, isPCKartikl, rtrano_rec, isMOC_PCK_base, MOC_wanted_NEW_RAM, MOC_wanted_NEW_HDD, false);

      TheG2.PutCell(ci2.iT_TT, rowIdx, theTT);

      if(this is MOD_PTG_DUC) (this as MOD_PTG_DUC).SetColors_MOD_PTG_DUC(theTT, rowIdx);

      if(isKomponentaArtikl) // MOU / MOI stavke (komponente PCK-a) 
      {
         if(oldArtiklCD.NotEmpty())
         {
            TheG2.PutCell(ci2.iT_artiklCD, rowIdx, oldArtiklCD);
         }
         else 
         {
            string newArtiklCD = TheG2.GetStringCell(ci2.iT_artiklCD, rowIdx, false);
            
            if(newArtiklCD.NotEmpty())
            {
               TheG2.PutCell(ci2.iR_artiklCD_Old, rowIdx, newArtiklCD);
            }
         }

         TheVvTabPage.TheVvForm.SetDirtyFlag(this); // !!! 

         // provjeravaj ovo samo za kolone RAM+/- i HDD+/- 
         if(isPlusOrMinusCol == false) return;

         //VvDataGridView dgv  = sender             as VvDataGridView;
         //VvTextBox vvTextBox = dgv.EditingControl as VvTextBox     ;
         VvTextBox vvTextBox = sender as VvTextBox;

         Artikl artikl_rec = Get_Artikl_FromVvUcSifrar(rtrano_rec.T_artiklCD);

         if(vvTextBox is null || artikl_rec is null) return;

         bool isRAM = colIdx == ci2.iT_RAM_plus || colIdx == ci2.iT_RAM_minus;
         bool isHDD = !isRAM;

         decimal enteredKapacitet = isRAM ? rtrano_rec.T_dimX + rtrano_rec.T_dimY : rtrano_rec.T_decA + rtrano_rec.T_decB;

       //decimal kolPutaKapacitet = rtrano_rec.T_kol * artikl_rec.Zapremina;
         decimal kolPutaKapacitet = isRAM ? rtrano_rec.T_kol * artikl_rec.PCK_RAM : rtrano_rec.T_kol * artikl_rec.PCK_HDD;

         if(enteredKapacitet != kolPutaKapacitet)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Uneseni +/- kapacitet ne odgovara specifikaciji komponente.");
         }
      }

      //08.07.2024.
      Put_MOD_Semafor_Labels();
   }

   internal void Put_MOD_Semafor_Labels(/*MOD_PTG_DUC modDUC*/)
   {
      Color okColor  = Color.Green;
      Color badColor = Color.Red  ;

    //int numOfMOCrows = faktur_rec.TrnNonDel2.Count(rto => rto.T_TT == Faktur.TT_MOC);
      int numOfMOCrows = /*modDUC.*/Get_MOC_RowCount();
      int MOCsaldo     = (int)Fld_someMoney - numOfMOCrows;

      decimal RAMsaldo, RAMplus, RAMminus;
      decimal HDDsaldo, HDDplus, HDDminus;

      //if(TheVvTabPage.WriteMode == ZXC.WriteMode.None)
      //{
      //   RAMplus  = faktur_rec./*TrnSum2_dimX*/TrnSum2_ALL_dimX;
      //   RAMminus = faktur_rec./*TrnSum2_dimY*/TrnSum2_ALL_dimY;
      //   HDDplus  = faktur_rec./*TrnSum2_decA*/TrnSum2_ALL_decA;
      //   HDDminus = faktur_rec./*TrnSum2_decB*/TrnSum2_ALL_decB;
      //}
      //else
      //{
         RAMplus  = TheSumGrid2.GetDecimalCell(ci2.iT_RAM_plus , 0, false);
         RAMminus = TheSumGrid2.GetDecimalCell(ci2.iT_RAM_minus, 0, false);
         HDDplus  = TheSumGrid2.GetDecimalCell(ci2.iT_HDD_plus , 0, false);
         HDDminus = TheSumGrid2.GetDecimalCell(ci2.iT_HDD_minus, 0, false);
      //}
      RAMsaldo = RAMplus - RAMminus;
      HDDsaldo = HDDplus - HDDminus;

      if(MOCsaldo.IsZero()) SetMODsemaforLabelColorAndText(/*modDUC.*/lbl_MOC, okColor , "MOC OK");
      else                  SetMODsemaforLabelColorAndText(/*modDUC.*/lbl_MOC, badColor, "MOC " + MOCsaldo);
      if(RAMsaldo.IsZero()) SetMODsemaforLabelColorAndText(/*modDUC.*/lbl_RAM, okColor , "RAM OK");
      else                  SetMODsemaforLabelColorAndText(/*modDUC.*/lbl_RAM, badColor, "RAM " + RAMsaldo.ToString0Vv());
      if(HDDsaldo.IsZero()) SetMODsemaforLabelColorAndText(/*modDUC.*/lbl_HDD, okColor , "HDD OK");
      else                  SetMODsemaforLabelColorAndText(/*modDUC.*/lbl_HDD, badColor, "HDD " + HDDsaldo.ToString0Vv());
   }

   internal void Set_MOU_MOI_RAMorHDD_minus(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      int rowIdx = TheG2.CurrentRow.Index;

      bool isPCK = TheG2.GetStringCell(ci2.iT_artiklTS, rowIdx, false) == ZXC.PCK_TS;

      Rtrano rtrano_rec = (Rtrano)GetDgvLineFields2(rowIdx, false, null);

      if(isPCK == true)
      {
         if(rtrano_rec.T_kol != 1.00M)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "Besmislena je količina različita od 1.");
            TheG2.PutCell(ci2.iT_kol, rowIdx, 1M);
         }
         return; // NIJE moi mou 
      }

      VvTextBox vvTextBox = sender as VvTextBox;

      Artikl artikl_rec = Get_Artikl_FromVvUcSifrar(rtrano_rec.T_artiklCD);

      if(vvTextBox is null || artikl_rec is null) return;

      bool isRAM = artikl_rec.Grupa1CD == ZXC.RAM_GR1;
      bool isHDD = artikl_rec.Grupa1CD == ZXC.HDD_GR1;

      if(isRAM && artikl_rec.PCK_RAM.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Artikl [{0}] nema definiran kapacitet");
         return;
      }

      if(isHDD && artikl_rec.PCK_HDD.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Artikl [{0}] nema definiran kapacitet");
         return;
      }

      decimal ram_kolPutaKapacitet = rtrano_rec.T_kol * artikl_rec.PCK_RAM;
      decimal hdd_kolPutaKapacitet = rtrano_rec.T_kol * artikl_rec.PCK_HDD;

      int ramDelta_colIdx = ci2.iT_RAM_minus;
      int hddDelta_colIdx = ci2.iT_HDD_minus;

      if(rtrano_rec.T_TT == Faktur.TT_MOU)
      {
         ramDelta_colIdx = ci2.iT_RAM_plus;
         hddDelta_colIdx = ci2.iT_HDD_plus;
      }

      TheG2.PutCell(ramDelta_colIdx, rowIdx, ram_kolPutaKapacitet);
      TheG2.PutCell(hddDelta_colIdx, rowIdx, hdd_kolPutaKapacitet);

      SetRow_TT_and_Color_and_Calc_newRam_newHdd(sender, e);
   }

   internal string Get_MOD_RtranoTT(int rowIdx, bool isPCK, Rtrano rtrano_rec, bool isMOC_PCK_base, decimal MOC_wanted_NEW_RAM, decimal MOC_wanted_NEW_HDD, bool shouldWarn)
   {
      if(rtrano_rec.T_artiklCD.IsEmpty()) return ""; // kak bus znal jel MOC MOS MOI MOU ak je artikl prazan 

      bool isMOC = isPCK &&
                   MOC_wanted_NEW_RAM  == rtrano_rec./*T_dimZ*/T_PCK_RAM &&
                   MOC_wanted_NEW_HDD  == rtrano_rec./*T_decC*/T_PCK_HDD &&
                   isMOC_PCK_base                                        &&
                   ThisIs_MOC_rowIndex(rowIdx)                           &&
                   rtrano_rec.T_serno.NotEmpty()                          ; // ovaj redak dodan 17.02.2025. ... jer kod sejvanja MOD-a kad ostavi prazni serno u 'možda MOC' retku radi sranja 

      if(isPCK)
      {
         if(isMOC) return Faktur.TT_MOC;
         else      return Faktur.TT_MOS;
      }

      decimal RAMplus  = TheG2.GetDecimalCell(ci2.iT_RAM_plus , rowIdx, false);
      decimal RAMminus = TheG2.GetDecimalCell(ci2.iT_RAM_minus, rowIdx, false);
      decimal HDDplus  = TheG2.GetDecimalCell(ci2.iT_HDD_plus , rowIdx, false);
      decimal HDDminus = TheG2.GetDecimalCell(ci2.iT_HDD_minus, rowIdx, false);

      bool isMOI = RAMminus.NotZero() || HDDminus.NotZero();
      bool isMOU = RAMplus .NotZero() || HDDplus .NotZero();

      if(isMOI && (RAMplus  + HDDplus ).NotZero()) ZXC.aim_emsg(MessageBoxIcon.Warning, "Komponenta u {0}. retku ima zadanu i plus i minus količinu!?", rowIdx + 1);
      if(isMOU && (RAMminus + HDDminus).NotZero()) ZXC.aim_emsg(MessageBoxIcon.Warning, "Komponenta u {0}. retku ima zadanu i plus i minus količinu!?", rowIdx + 1);

           if(isMOI) return Faktur.TT_MOI;
      else if(isMOU) return Faktur.TT_MOU;
      else
      {
         if(shouldWarn)
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "Stavka redka {0}\n\r\n\r{1}\n\r\n\rnije niti MOU niti MOI jer ima nedefinirane RAM/HDD plus/minus količine?!", rowIdx + 1, rtrano_rec);
         }

         //return Faktur.TT_MOU;
         return "";
      }

   }

   internal void Check_MOD_plusMinus_errors(int rowIdx, Rtrano rtrano_rec)
   {
      // samo kao podsjetnik: 
      // decimal RAMplus  = TheG2.GetDecimalCell(ci2.iT_dimX, rowIdx, false); 
      // decimal RAMminus = TheG2.GetDecimalCell(ci2.iT_dimY, rowIdx, false); 
      // decimal HDDplus  = TheG2.GetDecimalCell(ci2.iT_decA, rowIdx, false); 
      // decimal HDDminus = TheG2.GetDecimalCell(ci2.iT_decB, rowIdx, false); 

      uint numOfPlusMinusUnosa = 0;

      if(rtrano_rec.T_dimX.NotZero()) numOfPlusMinusUnosa++;
      if(rtrano_rec.T_dimY.NotZero()) numOfPlusMinusUnosa++;
      if(rtrano_rec.T_decA.NotZero()) numOfPlusMinusUnosa++;
      if(rtrano_rec.T_decB.NotZero()) numOfPlusMinusUnosa++;

      if(numOfPlusMinusUnosa < 1) ZXC.aim_emsg(MessageBoxIcon.Warning, "Stavka redka {0}\n\r\n\r{1}\n\r\n\rnema definirane RAM/HDD plus/minus količine?!", rowIdx + 1, rtrano_rec);
      if(numOfPlusMinusUnosa > 1) ZXC.aim_emsg(MessageBoxIcon.Warning, "Stavka redka {0}\n\r\n\r{1}\n\r\n\rima višestruko definirane RAM/HDD plus/minus količine?!", rowIdx + 1, rtrano_rec);
   }

   internal void Put_MOD_RAM_HDD_PlusMinus_ColSum_OnSumGrid()
   {
      Put_ColSum_OnSumGrid(ci2.iT_RAM_plus  );
      Put_ColSum_OnSumGrid(ci2.iT_RAM_minus );
      Put_ColSum_OnSumGrid(ci2.iT_HDD_plus  );
      Put_ColSum_OnSumGrid(ci2.iT_HDD_minus );
   }

   internal int Get_MOC_RowCount()
   {
      int theCount = 0;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return faktur_rec.Transes2.Where(rto => rto.T_TT == Faktur.TT_MOC).Count();

      for(int rowIdx = 0; rowIdx < TheCurrentG.RowCount - 1; ++rowIdx)
      {
         if(TheCurrentG.GetStringCell(ci2.iT_TT, rowIdx, false) == Faktur.TT_MOC) theCount++;
      }

      return theCount;
   }

   internal (bool OK, string newArtiklName) ADDREC_NewMOC_MOS_PCK_ArtiklFromOld(XSqlConnection conn, Artikl templateArtikl_rec, string newArtiklCD)
   {
      Artikl newArtikl_rec = templateArtikl_rec.MakeDeepCopy();

      (decimal newPCK_RAM, decimal newPCK_HDD) = Artikl.Get_PTG_RAM_HDD_From_ArtiklCD(newArtiklCD);

      newArtikl_rec.PCK_RAM = newPCK_RAM;
      newArtikl_rec.PCK_HDD = newPCK_HDD;

      newArtikl_rec.ArtiklCD   = newArtikl_rec.New_ArtiklCD_From_PCK_base_RAM_HDD;
      newArtikl_rec.ArtiklName = ZXC.ModifyPCK_ArtiklName(templateArtikl_rec.ArtiklName, newPCK_RAM, newPCK_HDD, templateArtikl_rec.ZapreminaJM, templateArtikl_rec.DuljinaJM);

      bool OK = newArtikl_rec.VvDao.ADDREC(conn, newArtikl_rec, false, false, false, false);

      return (OK, newArtikl_rec.ArtiklName);
   }

   #region SintRtranoToRtransOnMOD

   internal void SintRtranoToRtransOnMOD(VvForm theVvForm)
   {
      foreach(VvTransRecord modRtrans_rec in faktur_rec.Transes)
      {
         modRtrans_rec.VvDao.DELREC                                (TheDbConnection, modRtrans_rec, false);
         modRtrans_rec.VvDao.Delete_Then_Renew_Cache_FromThisRtrans(TheDbConnection, modRtrans_rec, VvSQL.DB_RW_ActionType.DEL);
      }

      faktur_rec.InvokeTransClear();

      theVvForm.BeginEdit(faktur_rec);

      Calc_AndOptional_ADDREC_MOD_Rtrans_From_MOD_Rtrano(TheDbConnection, faktur_rec, 0); // VOILA 

      //// reverse Transes order
      //for(int serial = 0, reverseSerial = faktur_rec.Transes.Count; serial < faktur_rec.Transes.Count; ++serial, --reverseSerial)
      //{
      //   faktur_rec.Transes[serial].T_serial = (ushort)reverseSerial;
      //   faktur_rec.Transes[serial].SaveTransesWriteMode = ZXC.WriteMode.Edit;
      //}

      faktur_rec.TakeTransesSumToDokumentSum(true);

      bool OK = theVvForm.TheVvDao.RWTREC(TheDbConnection, faktur_rec);

      if(!OK) { theVvForm.CancelEdit(faktur_rec); return; }

      theVvForm.EndEdit(faktur_rec);

      theVvForm.PutFieldsActions(TheDbConnection, faktur_rec, this);
   }

   internal static decimal Calc_AndOptional_ADDREC_MOD_Rtrans_From_MOD_Rtrano(XSqlConnection conn, Faktur _faktur_rec, ushort serial_MOUrtransa_kojiTrebaNovuCijenu)
   {
      bool isCheckPrNabCij = serial_MOUrtransa_kojiTrebaNovuCijenu.NotZero();
      bool isSaveMODfaktur = serial_MOUrtransa_kojiTrebaNovuCijenu.IsZero ();

      decimal theMOUcij  = 0M  ;
      Rtrans  rtrans_rec = null;

      decimal ukRAMfinIzlazSUM = 0M;
      decimal ukHDDfinIzlazSUM = 0M;

      ushort t_serial = 0;

      // ------------------------------------------------------------------------------------------------------------------------------------------------------------ // --- 
      List<Rtrano> moi_list = _faktur_rec.Transes2.Where(rto => rto.T_TT == Faktur.TT_MOI).ToList();                                                                  // --- 
                                                                                                                                                                      // --- 
      foreach(Rtrano KMP_rtrano_rec in moi_list)                                                                                                                      // --- 
      {                                                                                                                                                               // --- 
         rtrans_rec = Get_MOD_Rtrans_From_MOD_Rtrano_MOI(conn, _faktur_rec, KMP_rtrano_rec, ref t_serial, ref ukRAMfinIzlazSUM, ref ukHDDfinIzlazSUM);  // MOI KMP Artikl bijeli IZLAZ 
      }                                                                                                                                                               // --- 
      // ------------------------------------------------------------------------------------------------------------------------------------------------------------ // --- 
      List<Rtrano> moc_mos_list = _faktur_rec.Transes2.Where(rto => rto.T_TT == Faktur.TT_MOC || rto.T_TT == Faktur.TT_MOS).ToList();                                 // --- 
                                                                                                                                                                      // --- 
      string origT_artiklCD;                                                                                                                                          // --- 
      foreach(Rtrano PCK_rtrano_rec in moc_mos_list) // OLD PCK Artikl                                                                                                // --- 
      {                                                                                                                                                               // --- 
         origT_artiklCD            = PCK_rtrano_rec.T_artiklCD   ;                                                                                                    // --- 
         PCK_rtrano_rec.T_artiklCD = PCK_rtrano_rec.R_OldArtiklCD;                                                                                                    // --- 
                                                                                                                                                                      // --- 
         rtrans_rec = Get_MOD_Rtrans_From_MOD_Rtrano_MOI(conn, _faktur_rec, PCK_rtrano_rec, ref t_serial, ref ukRAMfinIzlazSUM, ref ukHDDfinIzlazSUM);  // MOI OLD PCK Artikl bijeli IZLAZ 
                                                                                                                                                                      // --- 
         PCK_rtrano_rec.T_artiklCD = origT_artiklCD ;                                                                                                                 // --- 
         PCK_rtrano_rec.T_komada   = rtrans_rec.R_KC; // cuvamo OLD Artikl Fin                                                                                        // --- 
      }                                                                                                                                                               // --- 
                                                                                                                                                                      // --- 
      // ------------------------------------------------------------------------------------------------------------------------------------------------------------ // --- 
      List<Rtrano> mou_list = _faktur_rec.Transes2.Where(rto => rto.T_TT == Faktur.TT_MOU).ToList();                                                                  // --- 
                                                                                                                                                                      // --- 
      decimal RAMplusGB_SUM = _faktur_rec.Transes2.Sum(rtrano => rtrano.T_dimX);                                                                                      // --- 
      decimal HDDplusGB_SUM = _faktur_rec.Transes2.Sum(rtrano => rtrano.T_decA);                                                                                      // --- 
                                                                                                                                                                      // --- 
      foreach(Rtrano KMP_rtrano_rec in mou_list)                                                                                                                      // --- 
      {                                                                                                                                                               // --- 
         rtrans_rec = Get_MOD_Rtrans_From_MOD_Rtrano_MOU(conn, _faktur_rec, KMP_rtrano_rec, ref t_serial, false, ukRAMfinIzlazSUM, RAMplusGB_SUM, ukHDDfinIzlazSUM, HDDplusGB_SUM);  // MOU KMP Artikl zeleni ULAZ 
                                                                                                                                                                      // --- 
         if(rtrans_rec.T_serial == serial_MOUrtransa_kojiTrebaNovuCijenu) theMOUcij = rtrans_rec.T_cij;                                                               // --- 
      }                                                                                                                                                               // --- 
      // ------------------------------------------------------------------------------------------------------------------------------------------------------------ // --- 
                                                                                                                                                                      // --- 
      foreach(Rtrano PCK_rtrano_rec in moc_mos_list) // NEW PCK Artikl                                                                                                // --- 
      {                                                                                                                                                               // --- 
         rtrans_rec = Get_MOD_Rtrans_From_MOD_Rtrano_MOU(conn, _faktur_rec, PCK_rtrano_rec, ref t_serial, true, ukRAMfinIzlazSUM, RAMplusGB_SUM, ukHDDfinIzlazSUM, HDDplusGB_SUM);   // MOU NEW PCK Artikl zeleni ULAZ 
                                                                                                                                                                      // --- 
         if(rtrans_rec.T_serial == serial_MOUrtransa_kojiTrebaNovuCijenu) theMOUcij = rtrans_rec.T_cij;                                                               // --- 
      }                                                                                                                                                               // --- 
      // ------------------------------------------------------------------------------------------------------------------------------------------------------------ // --- 

      return theMOUcij;
   }

   private static Rtrans Get_MOD_Rtrans_From_MOD_Rtrano_MOI(XSqlConnection conn, Faktur _faktur_rec, Rtrano rtrano_rec, ref ushort t_serial, ref decimal ukRAMfinIzlazSUM, ref decimal ukHDDfinIzlazSUM)
   {
      #region Init

      Rtrans rtrans_rec    ;
      Artikl  artikl_rec   ;
      string  t_jedMj      ;
      decimal theCij   = 0M;

    //artikl_rec = Get_Artikl_FromVvUcSifrar(rtrano_rec.T_artiklCD);
      artikl_rec = ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == rtrano_rec.T_artiklCD);

      if(artikl_rec != null) t_jedMj = artikl_rec.JedMj;
      else                   t_jedMj = ""              ;

      Rtrans limiterRtrans_rec = new Rtrans()
      {
         T_artiklCD  = rtrano_rec.T_artiklCD,
         T_skladCD   = rtrano_rec.T_skladCD,
         T_skladDate = rtrano_rec.T_skladDate,
         T_ttSort    = rtrano_rec.T_ttSort,
         T_ttNum     = rtrano_rec.T_ttNum,
         T_serial    = /*rtrano_rec.T_serial*/ (ushort)(t_serial + 1),
      };

    //ArtStat artStat = ArtiklDao.GetArtiklStatus(conn, rtrano_rec.T_artiklCD, rtrano_rec.T_skladCD, rtrano_rec.T_skladDate);
      ArtStat artStat = ArtiklDao.GetArtiklStatus(conn, limiterRtrans_rec);

      theCij = artStat != null ? artStat.PrNabCij : 0.00M;

      if(theCij.IsZero()) // za OLD Artikle i Komponente ocekujemo imati neku odprije cijenu 
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Nema cijene za artikl\n\r\n\r[{0}]", artikl_rec);
      }

      decimal RAMminusGB = rtrano_rec.T_dimY;
      decimal HDDminusGB = rtrano_rec.T_decB;

      #endregion Init

      rtrans_rec = new Rtrans(Faktur.TT_MOI, rtrano_rec, /*theCij,*/ t_jedMj, ++t_serial);

      rtrans_rec.T_twinID = rtrano_rec.T_recID; // Link it!                       
      rtrans_rec.T_cij    = theCij            ; // this is what we are living for 

      rtrans_rec.T_artiklName = artikl_rec.ArtiklName;

      AddRtransToFakturTransesCollection(_faktur_rec, rtrans_rec);

      #region Calc ukRAMfinIzlazSUM, ukHDDfinIzlazSUM

      bool isKomponenta = artikl_rec.TS == ZXC.KMP_TS;
      bool isOLDartikl  = !isKomponenta;
      
      bool isKomponentaRAMizlaz  = isKomponenta && RAMminusGB.NotZero(); 
      bool isOLDartiklLoosingRAM = isOLDartikl  && RAMminusGB.NotZero(); 
      bool isKomponentaHDDizlaz  = isKomponenta && HDDminusGB.NotZero(); 
      bool isOLDartiklLoosingHDD = isOLDartikl  && HDDminusGB.NotZero(); 

      if(isKomponentaRAMizlaz ) ukRAMfinIzlazSUM += rtrans_rec.R_KC                                                                                                    ;
      if(isOLDartiklLoosingRAM) ukRAMfinIzlazSUM += Get_MOD_PCK_OLDArtikl_RAMminusEstimatedValue(/*rtrans_rec.T_cij, artikl_rec.PCK_RAM,*/ RAMminusGB, _faktur_rec.DokDate);

      if(isKomponentaHDDizlaz ) ukHDDfinIzlazSUM += rtrans_rec.R_KC                                                                                                    ;
      if(isOLDartiklLoosingHDD) ukHDDfinIzlazSUM += Get_MOD_PCK_OLDArtikl_HDDminusEstimatedValue(/*rtrans_rec.T_cij, artikl_rec.PCK_HDD,*/ HDDminusGB, _faktur_rec.DokDate);

      #endregion Calc ukRAMfinIzlazSUM, ukHDDfinIzlazSUM

      return rtrans_rec;
   }

   private static Rtrans Get_MOD_Rtrans_From_MOD_Rtrano_MOU(XSqlConnection conn, Faktur _faktur_rec, Rtrano rtrano_rec, ref ushort t_serial, bool isPCKartikl, decimal ukRAMfinIzlazSUM, decimal RAMplusGB_SUM, decimal ukHDDfinIzlazSUM, decimal HDDplusGB_SUM)
   {
      #region Init

      Rtrans  rtrans_rec   ;
    //Artikl  OLDartikl_rec;
      Artikl  NEWartikl_rec;
      string  t_jedMj      ;
    //decimal theCij   = 0M;

    //OLDartikl_rec = Get_Artikl_FromVvUcSifrar(rtrano_rec.R_OldArtiklCD); // OLD!!! 
    //NEWartikl_rec = Get_Artikl_FromVvUcSifrar(rtrano_rec.T_artiklCD   ); // NEW!!! 
    //OLDartikl_rec = ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == rtrano_rec.R_OldArtiklCD); 
      NEWartikl_rec = ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == rtrano_rec.T_artiklCD   );

      if(NEWartikl_rec != null) t_jedMj = NEWartikl_rec.JedMj;
      else                      t_jedMj = ""                 ;

      #endregion Init

      rtrans_rec = new Rtrans(Faktur.TT_MOU, rtrano_rec, /*theCij,*/ t_jedMj, ++t_serial);

      rtrans_rec.T_artiklName = NEWartikl_rec.ArtiklName;

      AddRtransToFakturTransesCollection(_faktur_rec, rtrans_rec);

      decimal RAM1GB_UlazCij = ZXC.DivSafe(ukRAMfinIzlazSUM, RAMplusGB_SUM);
      decimal HDD1GB_UlazCij = ZXC.DivSafe(ukHDDfinIzlazSUM, HDDplusGB_SUM);

      if(isPCKartikl)
      {
         decimal RAMplusGB  = rtrano_rec.T_dimX;
         decimal RAMminusGB = rtrano_rec.T_dimY;
         decimal HDDplusGB  = rtrano_rec.T_decA;
         decimal HDDminusGB = rtrano_rec.T_decB;
      
         decimal RAMplusValue   = RAM1GB_UlazCij * RAMplusGB;
         decimal HDDplusValue   = HDD1GB_UlazCij * HDDplusGB;

         decimal OLDartiklValue = rtrano_rec.T_komada;

         decimal RAMminusValue  = Get_MOD_PCK_OLDArtikl_RAMminusEstimatedValue(/*OLDartiklValue, OLDartikl_rec.PCK_RAM,*/ RAMminusGB, _faktur_rec.DokDate);
         decimal HDDminusValue  = Get_MOD_PCK_OLDArtikl_HDDminusEstimatedValue(/*OLDartiklValue, OLDartikl_rec.PCK_HDD,*/ HDDminusGB, _faktur_rec.DokDate);

         decimal NEWartiklValue = OLDartiklValue + RAMplusValue - RAMminusValue + HDDplusValue - HDDminusValue;

         rtrans_rec.T_twinID = rtrano_rec.T_recID; // Link it!                       
         rtrans_rec.T_cij    = NEWartiklValue    ; // this is what we are living for 
      }
      else // Komponenta 
      {
         decimal kapacitet_RAM_komponente = NEWartikl_rec.PCK_RAM;
         decimal kapacitet_HDD_komponente = NEWartikl_rec.PCK_HDD;

         decimal cijena_komponente = (RAM1GB_UlazCij * kapacitet_RAM_komponente) + // pretpostavljamo da je ili RAM ili HDD kapacitet nula pa ne moramo razlucivati jel RAM jel HDD nego zbrajamo 
                                     (HDD1GB_UlazCij * kapacitet_HDD_komponente) ; // pretpostavljamo da je ili RAM ili HDD kapacitet nula pa ne moramo razlucivati jel RAM jel HDD nego zbrajamo 

         rtrans_rec.T_twinID = rtrano_rec.T_recID; // Link it!                       
         rtrans_rec.T_cij    = cijena_komponente ; // this is what we are living for 
      }

      return rtrans_rec;
   }

   private static void AddRtransToFakturTransesCollection(Faktur _faktur_rec, Rtrans _rtrans_rec)
   {
      _rtrans_rec.CalcTransResults(null);
      _rtrans_rec.SaveTransesWriteMode = ZXC.WriteMode.Add;
      _faktur_rec.Transes.Add(_rtrans_rec);
   }

   private static decimal Get_MOD_PCK_OLDArtikl_RAMminusEstimatedValue(/*decimal OLDartiklValue, decimal OLDartikl_RAM_GBcapacity,*/ decimal RAMminusInGB, DateTime dokDate)
   {
    //decimal estimated_PCK_Artikl_RAM_Value = ZXC.VvGet_25_of_100(OLDartiklValue, 25M); // todo X posto od PCK artikla je procijenjena RAM vrijednost u PCK-u ... from ZXC.RRD 
    //decimal RAMpricePerGB = ZXC.DivSafe(estimated_PCK_Artikl_RAM_Value, OLDartikl_RAM_GBcapacity);

      decimal RAMpricePerGB = GetPCK_RAMorHDD_pricePerGB("RAM", dokDate);

      return RAMminusInGB * RAMpricePerGB;
   }

   private static decimal Get_MOD_PCK_OLDArtikl_HDDminusEstimatedValue(/*decimal OLDartiklValue, decimal OLDartikl_HDD_GBcapacity,*/ decimal HDDminusInGB, DateTime dokDate)
   {
    //decimal estimated_PCK_Artikl_HDD_Value = ZXC.VvGet_25_of_100(OLDartiklValue, 25M); // todo X posto od PCK artikla je procijenjena HDD vrijednost u PCK-u ... from ZXC.RRD 
    //decimal HDDpricePerGB = ZXC.DivSafe(estimated_PCK_Artikl_HDD_Value, OLDartikl_HDD_GBcapacity);

      decimal HDDpricePerGB = GetPCK_RAMorHDD_pricePerGB("HDD", dokDate);

      return HDDminusInGB * HDDpricePerGB;
   }

   private static decimal GetPCK_RAMorHDD_pricePerGB(string priceID, DateTime dokDate)
   {
      VvLookUpLista luiList = ZXC.luiListaPCKpricesPerGB;

      luiList.LazyLoad();

      VvLookUpItem theLui = luiList.Where(lui => dokDate >= lui.DateT && lui.Cd == priceID).OrderBy(l => l.DateT).LastOrDefault();

      if(theLui != null) return theLui.Number;

      ZXC.aim_emsg(MessageBoxIcon.Error, "Nema " + priceID + "pricePerGB od datuma ", dokDate.ToString(ZXC.VvDateFormat)); 
      
      return 0M; 
   }

   #endregion SintRtranoToRtransOnMOD

}
