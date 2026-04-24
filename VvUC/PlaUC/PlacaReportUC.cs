using System;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CrystalDecisions.Windows.Forms;

public class PlacaReportUC : VvReportUC
{
   #region Fieldz

   public PlaFilterUC ThePlacaFilterUC { get; set; }
 
   private int razmakIzmjedjuHampera;
   private int nextX, nextY, razmakHamp = ZXC.Qun10;
   private int maxHampWidth;

   private VvHamper    hamp_rbtZaU, hamp_sortPerson, hamp_Open, hamp_bankaMjtr, hamp_person,
                       hamp_Spol, hamp_vrstaRadVremena, hamp_vrstaRadOdns, hamp_VrtsaIsplate, hamp_showFileAfterExport,
                       hamp_kontBrRsm, hamp_virmanDate, hamp_sortDok, hamp_printVir, hamp_jezik, hamp_goObrPor, hamp_VirmanBtchBookgKind;
   private RadioButton rbt_SortSifra, rbt_SortPrezime;
   private RadioButton rbt_standardFilter, rbt_dokNum, rbt_rsmID, rbt_uMj, rbt_zaMj, rbt_uGod, rbt_zaGod, rbt_naDan,
                       rbt_rvNebitno, rbt_rvPuno, rbt_rvNepun, rbt_rvSkr,
                       rbt_roNebitno, rbt_roNeodr, rbt_roOdr, rbt_roPripVjez,
                       rbt_ilNebitno, rbt_tekuci, rbt_banka, rbt_gotovina,
                       rbt_musko, rbt_zensko, rbt_nepoznato,
                       rBtcurrChecked, rbt_HRV, rbt_ENG;
   public RadioButton rbt_SortBroj, rbt_SortDatum,
                      rbt_ALLvirman, rbt_BtchBookg_ONLY, rbt_NON_BtchBookg_ONLY;

   public VvTextBox   tbx_dokNum, tbx_rsmID, tbx_uMj, tbx_zaMj, tbx_uGod, tbx_zaGod,tbx_naDan,
                      tbx_banka_cd, tbx_banka_tk, tbx_banka_Naziv, tbx_mtros_cd, tbx_mtros_tk, tbx_mtros_Naziv, tbx_strSpr, tbx_strSprCd, tbx_ts,
                      tbx_showFileAfterExport, tbx_kontBrRsm,
                      tbx_virDateVal, tbx_virDatePod, tbx_showVirDateVal, tbx_showVirDatePod,
                      tbx_POR, tbx_PRI, tbx_MIO1, tbx_MIO2, tbx_ZDR, tbx_ZOR, tbx_ZPI, tbx_ZAP, tbx_ZPP, tbx_NET,
                      tbx_KRP, tbx_MIO1NA, tbx_MIO2NA, tbx_PRE
                      , tbx_pomak, tbx_goObrLimit;                

   private Button btn_PlusMinus;
   private Label  lblCrta;
   private VvDateTimePicker dTP_uGod, dTP_zaGod, dtp_virDatVal, dtp_virDatPod, dtp_naDan;
   private VvCheckBox cbx_showFileAfterExport, cbx_kontBrRsm, cbx_showVirDatVal, cbx_showVirDatPod,
                      cbx_POR, cbx_PRI, cbx_MIO1, cbx_MIO2, cbx_ZDR, cbx_ZOR, cbx_ZPI, cbx_ZAP, cbx_ZPP, cbx_NET,
                      cbx_KRP, cbx_MIO1NA, cbx_MIO2NA, cbx_PRE;
   private ComboBox combbx_Ziro1;
   private CheckBox cbx_pomakListe, cbx_znpBezPnb, cbx_hocuGoPiP;

   #endregion Fieldz

   #region Constructor

   public PlacaReportUC(Control parent, VvRpt_Placa_Filter _rptFilter, VvSubModul vvSubModul)
   {
      this.TheSubModul = vvSubModul;
      this.TheRptFilter = _rptFilter;

      SuspendLayout();

      AddThePlacaFiltetUC();

      CreateHamperBeforeThePlacaFilterUC();

      SetLocation4ThePlacaFilterUcHampers();
      CreateHampeArfterThePlacaFilterUC();

      ThePlacaFilterUC.hamperHorLine.Location = new Point(nextX, hamp_Open.Bottom+razmakIzmjedjuHampera);
      ThePlacaFilterUC.hamperHorLine.Parent = TheFilterPanel;
      ThePlacaFilterUC.hamperHorLine.BringToFront();

      nextY = ThePlacaFilterUC.hamperHorLine.Bottom + razmakIzmjedjuHampera;

      InitializeVvUserControl(parent);

      TheFilterPanel_Width = ThePlacaFilterUC.Width;
      CalcTheFilterPanelWidth();

      //  ******************************************************************************************************
      // VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      // nemoj se opet zajebati ovo nemere ici tu jerbo cisti filterUC dolazi na recordTabPage i on mora imati svoj
      // pa ti dodjeli tamo koamo treba tj. u hampere koji imaju vvTexBoxove ... 
      //  ******************************************************************************************************

      this.AutoScroll = false;
      ResumeLayout();


   }

   #endregion Constructor

   #region  ThePlacaFilterUC
   
   private void AddThePlacaFiltetUC()
   {
      ThePlacaFilterUC        = new PlaFilterUC(this);
      ThePlacaFilterUC.Parent = TheFilterPanel;
      maxHampWidth            = ThePlacaFilterUC.MaxHamperWidth;
      razmakIzmjedjuHampera   = ThePlacaFilterUC.razmakIzmjedjuHampera;

      ThePlacaFilterUC.Height = ThePlacaFilterUC.Height - ThePlacaFilterUC.hamperHorLine.Height - (4 * razmakIzmjedjuHampera);

      nextX = ThePlacaFilterUC.nextX;
      nextY = ThePlacaFilterUC.hamper4buttons.Bottom + razmakIzmjedjuHampera;
   }

   private void SetLocation4ThePlacaFilterUcHampers()
   {
      ThePlacaFilterUC.hamp_dateIzvj.Location  = new Point(nextX, nextY);
      ThePlacaFilterUC.hamp_OdDo.Location      = new Point(nextX, ThePlacaFilterUC.hamp_dateIzvj.Bottom  + razmakIzmjedjuHampera);
      ThePlacaFilterUC.hamp_tt.Location        = new Point(nextX, ThePlacaFilterUC.hamp_OdDo.Bottom      + razmakIzmjedjuHampera);
      ThePlacaFilterUC.hamp_rootPrjkt.Location = new Point(nextX, ThePlacaFilterUC.hamp_tt.Bottom        + razmakIzmjedjuHampera);

      nextX = razmakIzmjedjuHampera;
      ThePlacaFilterUC.Height += hamp_rbtZaU.Height;
      ThePlacaFilterUC.Height += razmakIzmjedjuHampera;
   }
   
   #endregion  ThePlacaFilterUC

   #region Hampers
   
   private void CreateHamperBeforeThePlacaFilterUC()
   {
      InitializeHamper_RbtZaU(out hamp_rbtZaU);
      nextY = hamp_rbtZaU.Bottom + razmakIzmjedjuHampera;
   }

   private void InitializeHamper_RbtZaU(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 8, "", TheFilterPanel, false, nextX, nextY, razmakIzmjedjuHampera);

      hamper.VvColWdt      = new int[] { ZXC.Q6un + ZXC.Qun4, ZXC.Q4un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Q2un, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
      rbt_naDan = hamper.CreateVvRadioButton  (0, 0, new EventHandler(rbt_Click), "Na dan", TextImageRelation.ImageBeforeText);
      tbx_naDan = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_zaDan", "Obračun plaće na zadani dan");
      tbx_naDan.JAM_IsForDateTimePicker = true;
      dtp_naDan = hamper.CreateVvDateTimePicker(1, 0, "", tbx_naDan);
      rbt_naDan.Tag = tbx_naDan;
      dtp_naDan.Name = "dtp_datumOd";
      dtp_naDan.Tag = rbt_naDan;

      rbt_dokNum = hamper.CreateVvRadioButton(0, 1, new EventHandler(rbt_Click), "Broj dokumenta", TextImageRelation.ImageBeforeText);
      tbx_dokNum = hamper.CreateVvTextBox    (1, 1, "tbx_dokNum", "Prikaz za dokument broj", 6);
      tbx_dokNum.Tag = rbt_dokNum;
      rbt_dokNum.Tag = tbx_dokNum;
      tbx_dokNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_dokNum.JAM_FillCharacter = '0';
      tbx_dokNum.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_dokNum.DoubleClick += new EventHandler(tbx_DoubleClick);

      string rsmJopd = int.Parse(ZXC.projectYear) < 2014 ? "Rsm ID" : "JOPPD";
      rbt_rsmID  = hamper.CreateVvRadioButton(0, 2, new EventHandler(rbt_Click), rsmJopd , TextImageRelation.ImageBeforeText);
      tbx_rsmID  = hamper.CreateVvTextBox    (1, 2, "tbx_rsmID", "RSm identifikator");
      tbx_rsmID.Tag = rbt_rsmID;
      rbt_rsmID.Tag = tbx_rsmID;
      rbt_rsmID.Checked = true;
      this.ControlForInitialFocus = tbx_rsmID;
      tbx_rsmID.DoubleClick += new EventHandler(tbx_DoubleClick);

      rbt_uMj = hamper.CreateVvRadioButton  (0, 3, new EventHandler(rbt_Click), "U mjesecu"     , TextImageRelation.ImageBeforeText);
      tbx_uMj = hamper.CreateVvTextBoxLookUp(1, 3, "tbx_uMj", "Obračun plaće U zadanom mjesecu u godini", 6);
      tbx_uMj.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_uMj.DoubleClick += new EventHandler(tbx_DoubleClick);
      tbx_uMj.Tag = rbt_uMj;
      rbt_uMj.Tag = tbx_uMj;

      rbt_zaMj   = hamper.CreateVvRadioButton  (0, 4, new EventHandler(rbt_Click), "Za mjesec"     , TextImageRelation.ImageBeforeText);
      tbx_zaMj   = hamper.CreateVvTextBoxLookUp(1, 4, "tbx_zaMj", "Obračun plaće ZA zadani mjesec u godini", 6);
      tbx_zaMj.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_zaMj.DoubleClick += new EventHandler(tbx_DoubleClick);
      tbx_zaMj.Tag = rbt_zaMj;
      rbt_zaMj.Tag = tbx_zaMj;

      // 14.10.2010:
      //VvLookUpLista fondSatiLista = ZXC.CURR_prjkt_rec.IsTrgRs ? ZXC.luiListaFondSati_TRG : ZXC.luiListaFondSati_NOR;
      VvLookUpLista fondSatiLista = ZXC.luiListaFondSati_NOR;
      tbx_zaMj.JAM_Set_LookUpTable(fondSatiLista, (int)ZXC.Kolona.prva);
      tbx_uMj .JAM_Set_LookUpTable(fondSatiLista, (int)ZXC.Kolona.prva);

      rbt_uGod = hamper.CreateVvRadioButton   (0, 5, new EventHandler(rbt_Click), "U godini", TextImageRelation.ImageBeforeText);
      tbx_uGod = hamper.CreateVvTextBox       (1, 5, "tbx_uGod", "");
      rbt_uGod.Tag = tbx_uGod;
      tbx_uGod.JAM_IsForDateTimePicker = true;

      dTP_uGod      = hamper.CreateVvDateTimePicker(1, 5, "", tbx_uGod);
      dTP_uGod.Name = "dTP_uGod";
      dTP_uGod.Tag  = rbt_uGod;

      tbx_uGod.JAM_IsForDateTimePicker_YearOnly = true;
      dTP_uGod.ShowUpDown = true;
      tbx_uGod.DoubleClick += new EventHandler(tbx_DoubleClick);

      rbt_zaGod = hamper.CreateVvRadioButton   (0, 6, new EventHandler(rbt_Click), "Za godinu"     , TextImageRelation.ImageBeforeText);
      tbx_zaGod = hamper.CreateVvTextBox       (1, 6, "tbx_zaGod", "");
      rbt_zaGod.Tag = tbx_zaGod;
      tbx_zaGod.JAM_IsForDateTimePicker = true;
     
      dTP_zaGod      = hamper.CreateVvDateTimePicker(1, 6, "", tbx_zaGod);
      dTP_zaGod.Name = "dTP_zaGod";
      dTP_zaGod.Tag  = rbt_zaGod;
 
      tbx_zaGod.JAM_IsForDateTimePicker_YearOnly = true;
      dTP_zaGod.ShowUpDown = true;

      tbx_zaGod.DoubleClick += new EventHandler(tbx_DoubleClick);

      rbt_standardFilter = hamper.CreateVvRadioButton(0, 7, new EventHandler(rbt_Click), "Standardni dokument filter", 1, 0, TextImageRelation.ImageBeforeText);
      // rbt_standardFilter.Checked = true;

      VvHamper.Open_Close_Fields_ForWriting(hamper   , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.HamperStyling(hamper);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, ThePlacaFilterUC.MaxHamperWidth, razmakIzmjedjuHampera);

      //rbt_standardFilter.PerformClick();
      rbt_rsmID.PerformClick();
     
      hamper.BringToFront();
   }

   private void CreateHampeArfterThePlacaFilterUC()
   {
      nextX = ThePlacaFilterUC.nextX;
      nextY = ThePlacaFilterUC.Bottom + razmakIzmjedjuHampera/2;

      InitializeHamper_SortDok(out hamp_sortDok);

      nextX = hamp_sortDok.Right + razmakIzmjedjuHampera;
      InitializeHamper_SortPerson(out hamp_sortPerson);

      nextY = hamp_sortDok.Bottom + razmakIzmjedjuHampera;
      nextX = ThePlacaFilterUC.nextX;

      InitializeHamper_ShowFileAfterExport(out hamp_showFileAfterExport);
      nextY = hamp_showFileAfterExport.Bottom + razmakIzmjedjuHampera;

    //InitializeHamper_KontrolniBrRsm(out hamp_kontBrRsm);  // premjesteno u ostalo
    //nextY = hamp_kontBrRsm.Bottom + razmakIzmjedjuHampera;

      InitializeHamper_VirmanBtchBookgKind(out hamp_VirmanBtchBookgKind);
      nextY = hamp_VirmanBtchBookgKind.Bottom + razmakIzmjedjuHampera;


      InitializeHamper_VirmanDate(out hamp_virmanDate);
      nextY = hamp_virmanDate.Bottom + razmakIzmjedjuHampera;
    
      InitializeHamper_PrintVirman(out hamp_printVir);
      nextY = hamp_printVir.Bottom + razmakIzmjedjuHampera;
 
      InitializeHamper_GoObrPor(out hamp_goObrPor);
      nextY = hamp_goObrPor.Bottom + razmakIzmjedjuHampera;

      InitializeHamper_Jezici(out hamp_jezik);
      nextY = hamp_jezik.Bottom + razmakIzmjedjuHampera;

      CreateHamperOpen();
      //nextY = hamp_Open.Bottom + razmakIzmjedjuHampera - hamp_rbtZaU.Height + ZXC.Qun4;
      nextY = hamp_Open.Bottom - ZXC.Q3un ;

      InitializeHamper_BankaMjTr(out hamp_bankaMjtr);
      nextY = hamp_bankaMjtr.Bottom + razmakIzmjedjuHampera;

      InitializeHamper_Person(out hamp_person);
      nextY = hamp_person.Bottom + razmakIzmjedjuHampera;

      InitializeHamper_KontrolniBrRsm(out hamp_kontBrRsm);
      nextY = hamp_kontBrRsm.Bottom + razmakIzmjedjuHampera;

      InitializeHampeSpol(out hamp_Spol);

      nextX = hamp_Spol.Right + razmakIzmjedjuHampera;
      InitializeHamperRadVr(out hamp_vrstaRadVremena);

      nextX = ThePlacaFilterUC.nextX;
      nextY = hamp_vrstaRadVremena.Bottom + razmakIzmjedjuHampera;
      InitializeHamperVrstIsplate(out hamp_VrtsaIsplate);

      nextX = hamp_VrtsaIsplate.Right + razmakIzmjedjuHampera;
      InitializeHamperRadOdnos(out hamp_vrstaRadOdns);

      nextX = ThePlacaFilterUC.nextX;
      nextY = hamp_vrstaRadVremena.Bottom;
   }

   private void InitializeHamper_SortPerson(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", TheFilterPanel, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q6un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "Slijed ispisa radnika:", ContentAlignment.MiddleLeft);

      rbt_SortPrezime = hamper.CreateVvRadioButton(0, 1, null, "Po prezimenu", TextImageRelation.ImageBeforeText);
      rbt_SortSifra   = hamper.CreateVvRadioButton(0, 2, null, "Po šifri", TextImageRelation.ImageBeforeText);
      rbt_SortSifra.Checked = true;

      VvHamper.HamperStyling(hamper);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper,ThePlacaFilterUC.MaxHamperWidth, razmakIzmjedjuHampera);
   }
  
   private void InitializeHamper_SortDok(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", TheFilterPanel, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q6un + ZXC.Qun2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "Slijed ispisa dokumenta:", ContentAlignment.MiddleLeft);

      rbt_SortDatum = hamper.CreateVvRadioButton(0, 1, null, "Po datumu", TextImageRelation.ImageBeforeText);
      rbt_SortDatum.Checked = true;
      rbt_SortBroj = hamper.CreateVvRadioButton(0, 2, null, "Po broju", TextImageRelation.ImageBeforeText);

      VvHamper.HamperStyling(hamper);

   }

   private void InitializeHamper_ShowFileAfterExport(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", TheFilterPanel, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q10un + ZXC.Q2un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8,           ZXC.Qun8};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
   
      Label lbl1              = hamper.CreateVvLabel  (0, 0, "Prikaži datoteku za export:", ContentAlignment.MiddleRight);
      tbx_showFileAfterExport = hamper.CreateVvTextBox(1, 0, "tbx_showFileAfterExport", "");
      tbx_showFileAfterExport.JAM_Highlighted = true;
      cbx_showFileAfterExport = hamper.CreateVvCheckBox(1, 0, "", tbx_showFileAfterExport, "", "X");
   
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, ThePlacaFilterUC.MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_VirmanDate(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 5, "", TheFilterPanel, false, nextX, nextY, 0);

      //hamper.VvColWdt      = new int[] { ZXC.Q2un + ZXC.Qun2, ZXC.Q4un + ZXC.Qun4, ZXC.Q2un, ZXC.Q3un - ZXC.Qun8, ZXC.QUN - ZXC.Qun4 };
      hamper.VvColWdt      = new int[] { ZXC.QUN, ZXC.QUN + ZXC.Qun2, ZXC.Q4un + ZXC.Qun4, ZXC.Q2un, ZXC.Q3un - ZXC.Qun8, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,       0,                   0, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvSpcBefRow[3] = ZXC.Qun4;
      hamper.VvBottomMargin = ZXC.Qun4 + ZXC.Qun10;

      Label lbdate, lbdate1, lbdate2, lbziro;

      lbdate  = hamper.CreateVvLabel(0, 0, "VIRMANI"          , 1, 0, ContentAlignment.MiddleLeft);
      lbdate1 = hamper.CreateVvLabel(2, 0, "Datum valute:"          , ContentAlignment.MiddleRight);
      lbdate2 = hamper.CreateVvLabel(1, 1, "Datum podnošenja:", 1, 0, ContentAlignment.MiddleRight);
      lbziro  = hamper.CreateVvLabel(0, 2, "Žiro plat:"       , 1, 0, ContentAlignment.MiddleRight);


      tbx_pomak = hamper.CreateVvTextBox(0, 1, "tbx_pomak", "Pomak printanja virmana 2 - 2mm, 3 - 3mm, 4 - 4mm, 5 - 5mm", 1);
      tbx_pomak.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_pomak.JAM_AllowedInputCharacters = "2345";
      tbx_pomak.TextAlign = HorizontalAlignment.Center;

      tbx_virDateVal = hamper.CreateVvTextBox(3, 0, "tbx_virDatVal", "", 12, 1, 0);
      tbx_virDateVal.JAM_IsForDateTimePicker = true;
      dtp_virDatVal = hamper.CreateVvDateTimePicker(3, 0, "", 1, 0, tbx_virDateVal);
      dtp_virDatVal.Name = "dtp_virDatVal";
      dtp_virDatVal.Tag = tbx_virDateVal;
      tbx_virDateVal.Tag = dtp_virDatVal;
      tbx_virDateVal.JAM_WriteOnly = true;

      tbx_showVirDateVal = hamper.CreateVvTextBox(5, 0, "tbx_showVirDatVal", "");
      tbx_showVirDateVal.JAM_Highlighted = true;
      cbx_showVirDatVal = hamper.CreateVvCheckBox(5, 0, "", tbx_showVirDateVal, "", "X");
    //  cbx_showVirDatVal.Checked = true;

      tbx_virDatePod = hamper.CreateVvTextBox(3, 1, "tbx_virDatPod", "", 12, 1, 0);
      tbx_virDatePod.JAM_IsForDateTimePicker = true;
      dtp_virDatPod = hamper.CreateVvDateTimePicker(3, 1, "", 1, 0, tbx_virDatePod);
      dtp_virDatPod.Name = "dtp_virDatPod";
      dtp_virDatPod.Tag = tbx_virDatePod;
      tbx_virDatePod.Tag = dtp_virDatPod;
      tbx_virDatePod.JAM_WriteOnly = true;

      tbx_showVirDatePod = hamper.CreateVvTextBox(5, 1, "tbx_showVirDatPod", "");
      tbx_showVirDatePod.JAM_Highlighted = true;
      cbx_showVirDatPod = hamper.CreateVvCheckBox(5, 1, "", tbx_showVirDatePod, "", "X");
     // cbx_showVirDatPod.Checked = true;

      combbx_Ziro1 = hamper.CreateVvComboBox(2, 2, "", 3, 0, "combbx_Ziro1", ComboBoxStyle.DropDownList);
      combbx_Ziro1.DataSource = ZXC.CURR_prjkt_rec.ZiroList;

      Label lbKupDob   = hamper.CreateVvLabel  (0, 3, "Banka:" , 1, 0, ContentAlignment.MiddleRight);
 
      tbx_banka_cd     = hamper.CreateVvTextBox(2, 3, "tbx_banka_cd"   , "Sifra banke" , 6);
      tbx_banka_tk     = hamper.CreateVvTextBox(4, 3, "tbx_banka_tk"   , "Ticker banke", 6, 1, 0);
      tbx_banka_Naziv  = hamper.CreateVvTextBox(2, 4, "tbx_banka_Naziv", "Naziv banke" , 30, 3, 0);

      tbx_banka_cd   .JAM_MustTabOutBeforeSubmit  = true;
      tbx_banka_tk   .JAM_MustTabOutBeforeSubmit = true;
      tbx_banka_Naziv.JAM_MustTabOutBeforeSubmit  = true;
      tbx_banka_cd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_banka_tk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_banka_cd.   JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , ZXC.AutoCompleteRestrictor.KID_Banka_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyBankaTextBoxLeave));
      tbx_banka_tk.   JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Banka_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyBankaTextBoxLeave));
      tbx_banka_Naziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , ZXC.AutoCompleteRestrictor.KID_Banka_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyBankaTextBoxLeave));

    

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, ThePlacaFilterUC.MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);   
   }
  
   private void InitializeHamper_PrintVirman(out VvHamper hamper)
   {
      hamper = new VvHamper(14, 2, "", TheFilterPanel, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.QUN + ZXC.Qun8          , ZXC.QUN - ZXC.Qun4 - ZXC.Qun8+ZXC.Qun12,
                                         ZXC.QUN + ZXC.Qun4          , ZXC.QUN - ZXC.Qun4 - ZXC.Qun8+ZXC.Qun12, 
                                         ZXC.QUN + ZXC.Qun8          , ZXC.QUN - ZXC.Qun4 - ZXC.Qun8+ZXC.Qun12, 
                                         ZXC.QUN + ZXC.Qun8          , ZXC.QUN - ZXC.Qun4 - ZXC.Qun8+ZXC.Qun12, 
                                         ZXC.QUN + ZXC.Qun4          , ZXC.QUN - ZXC.Qun4 - ZXC.Qun8+ZXC.Qun12, 
                                         ZXC.QUN + ZXC.Qun8          , ZXC.QUN - ZXC.Qun4 - ZXC.Qun8+ZXC.Qun12, 
                                         ZXC.QUN + ZXC.Qun4+ZXC.Qun10, ZXC.QUN - ZXC.Qun4 - ZXC.Qun8+ZXC.Qun12};
      for(int i = 0; i < hamper.VvNumOfCols; i++)
      {
         hamper.VvSpcBefCol[i] = 0;
      }
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = ZXC.Qun4;
    
                hamper.CreateVvLabel   (0, 0, "Por", ContentAlignment.MiddleRight);
      tbx_POR = hamper.CreateVvTextBox (1, 0, "tbx_POR"  , "Printanje virmana za porez na dohodak");
      cbx_POR = hamper.CreateVvCheckBox(1, 0, "", tbx_POR, "", "x");
      tbx_POR.JAM_Highlighted = true;
      cbx_POR.Checked         = true;

                hamper.CreateVvLabel   (0, 1, "Pri", ContentAlignment.MiddleRight);
      tbx_PRI = hamper.CreateVvTextBox (1, 1, "tbx_PRI"  , "Printanje virmana za prirez na porez na dohodak");
      cbx_PRI = hamper.CreateVvCheckBox(1, 1, "", tbx_PRI, "", "x");
      tbx_PRI.JAM_Highlighted = true;
      cbx_PRI.Checked         = true;

                 hamper.CreateVvLabel   (2, 0, "Mi1", ContentAlignment.MiddleRight);
      tbx_MIO1 = hamper.CreateVvTextBox (3, 0, "tbx_MIO1", "Printanje virmana za doprinos za MIO I iz plaće");
      cbx_MIO1 = hamper.CreateVvCheckBox(3, 0, "", tbx_MIO1, "", "x");
      tbx_MIO1.JAM_Highlighted = true;
      cbx_MIO1.Checked = true;

                 hamper.CreateVvLabel   (2, 1, "Mi2", ContentAlignment.MiddleRight);
      tbx_MIO2 = hamper.CreateVvTextBox (3, 1, "tbx_MIO2", "Printanje virmana za doprinos za MIO II iz plaće");
      cbx_MIO2 = hamper.CreateVvCheckBox(3, 1, "", tbx_MIO2, "", "x");
      tbx_MIO2.JAM_Highlighted = true;
      cbx_MIO2.Checked = true;

                hamper.CreateVvLabel   (4, 0, "Zdr", ContentAlignment.MiddleRight);
      tbx_ZDR = hamper.CreateVvTextBox (5, 0, "tbx_ZDR", "Printanje virmana za doprinos za zdravstveno osiguranje na plaće");
      cbx_ZDR = hamper.CreateVvCheckBox(5, 0, "", tbx_ZDR, "", "x");
      tbx_ZDR.JAM_Highlighted = true;
      cbx_ZDR.Checked = true;

                hamper.CreateVvLabel   (4, 1, "Zor", ContentAlignment.MiddleRight);
      tbx_ZOR = hamper.CreateVvTextBox (5, 1, "tbx_ZOR", "Printanje virmana za doprinos za zdravstveno osiguranje za ozljede na radu na plaće");
      cbx_ZOR = hamper.CreateVvCheckBox(5, 1, "", tbx_ZOR, "", "x");
      tbx_ZOR.JAM_Highlighted = true;
      cbx_ZOR.Checked = true;

                hamper.CreateVvLabel   (6, 0, "Net", ContentAlignment.MiddleRight);
      tbx_NET = hamper.CreateVvTextBox (7, 0, "tbx_NET", "Printanje virmana za neto");
      cbx_NET = hamper.CreateVvCheckBox(7, 0, "", tbx_NET, "", "x");
      tbx_NET.JAM_Highlighted = true;
      cbx_NET.Checked = true;

                hamper.CreateVvLabel   (6, 1, "Krp", ContentAlignment.MiddleRight);
      tbx_KRP = hamper.CreateVvTextBox (7, 1, "tbx_KRP", "Printanje virmana za poseban porez");
      cbx_KRP = hamper.CreateVvCheckBox(7, 1, "", tbx_KRP, "", "x");
      tbx_KRP.JAM_Highlighted = true;
      cbx_KRP.Checked = true;

                hamper.CreateVvLabel   (8, 0, "Zap", ContentAlignment.MiddleRight);
      tbx_ZAP = hamper.CreateVvTextBox (9, 0, "tbx_ZAP", "Printanje virmana za doprinos za zapošljavanje na plaće");
      cbx_ZAP = hamper.CreateVvCheckBox(9, 0, "", tbx_ZAP, "", "x");
      tbx_ZAP.JAM_Highlighted = true;
      cbx_ZAP.Checked = true;

                hamper.CreateVvLabel   (8, 1, "Zpp", ContentAlignment.MiddleRight);
      tbx_ZPP = hamper.CreateVvTextBox (9, 1, "tbx_ZPP", "Printanje virmana za doprinos za zapošljavanje II na plaće");
      cbx_ZPP = hamper.CreateVvCheckBox(9, 1, "", tbx_ZPP, "", "x");
      tbx_ZPP.JAM_Highlighted = true;
      cbx_ZPP.Checked = true;


                hamper.CreateVvLabel   (10, 0, "Zpi", ContentAlignment.MiddleRight);
      tbx_ZPI = hamper.CreateVvTextBox (11, 0, "tbx_ZPI", "Printanje virmana za ZPI");
      cbx_ZPI = hamper.CreateVvCheckBox(11, 0, "", tbx_ZPI, "", "x");
      tbx_ZPI.JAM_Highlighted = true;
      cbx_ZPI.Checked = true;

                hamper.CreateVvLabel   (10, 1, "Prv", ContentAlignment.MiddleRight);
      tbx_PRE = hamper.CreateVvTextBox (11, 1, "tbx_PRE", "Printanje virmana za PRE");
      cbx_PRE = hamper.CreateVvCheckBox(11, 1, "", tbx_PRE, "", "x");
      tbx_PRE.JAM_Highlighted = true;
      cbx_PRE.Checked = true;

                   hamper.CreateVvLabel   (12, 0, "Mn1", ContentAlignment.MiddleRight);
      tbx_MIO1NA = hamper.CreateVvTextBox (13, 0, "tbx_MIO1NA", "Printanje virmana za MIO1NA");
      cbx_MIO1NA = hamper.CreateVvCheckBox(13, 0, "", tbx_MIO1NA, "", "x");
      tbx_MIO1NA.JAM_Highlighted = true;
      cbx_MIO1NA.Checked = true;
  
                   hamper.CreateVvLabel   (12, 1, "Mn2", ContentAlignment.MiddleRight);
      tbx_MIO2NA = hamper.CreateVvTextBox (13, 1, "tbx_MIO2NA", "Printanje virmana za MIO2NA");
      cbx_MIO2NA = hamper.CreateVvCheckBox(13, 1, "", tbx_MIO2NA, "", "x");
      tbx_MIO2NA.JAM_Highlighted = true;
      cbx_MIO2NA.Checked = true;

      tbx_POR.Font = tbx_PRI.Font = tbx_MIO1.Font = tbx_MIO2.Font = tbx_ZDR.Font = tbx_ZOR.Font = tbx_NET.Font =
      tbx_ZAP.Font = tbx_ZPP.Font = tbx_KRP.Font = tbx_ZPI.Font = tbx_MIO1NA.Font = tbx_MIO2NA.Font = tbx_PRE.Font = ZXC.vvFont.SmallBoldFont;
  

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, ThePlacaFilterUC.MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);   
   }

   private void InitializeHamper_GoObrPor(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 2, "", TheFilterPanel, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q8un, ZXC.Q3un };
      for(int i = 0; i < hamper.VvNumOfCols; i++)
      {
         hamper.VvSpcBefCol[i] = ZXC.Qun4;
      }
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = ZXC.Qun4;


      cbx_hocuGoPiP = hamper.CreateVvCheckBox_OLD(0, 0, null, 1, 0, "Konačni obračun poreza na zadnjoj plaći", System.Windows.Forms.RightToLeft.No);
      
      hamper.CreateVvLabel(0, 1, "Iznos tolerancije neta:", ContentAlignment.MiddleRight);

      tbx_goObrLimit = hamper.CreateVvTextBox(1, 1, "tbx_goObrLimit", "Neto limit GO PorPri:", 12);
      tbx_goObrLimit.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_goObrLimit.JAM_WriteOnly = true;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, ThePlacaFilterUC.MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);   

   }

   private void InitializeHamper_VirmanBtchBookgKind(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", TheFilterPanel, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un - ZXC.Qun2, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "SEPA:", ContentAlignment.MiddleRight);

      rbt_ALLvirman          = hamper.CreateVvRadioButton(1, 0, null, "Sve"   , TextImageRelation.ImageBeforeText);
      rbt_BtchBookg_ONLY     = hamper.CreateVvRadioButton(2, 0, null, "Zbirno", TextImageRelation.ImageBeforeText);
      rbt_NON_BtchBookg_ONLY = hamper.CreateVvRadioButton(3, 0, null, "Ostalo", TextImageRelation.ImageBeforeText);
      rbt_ALLvirman.Checked = true;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, ThePlacaFilterUC.MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }
 
   private void CreateHamperOpen()
   {
      hamp_Open          = new VvHamper(2, 1, "", TheFilterPanel, false);
      hamp_Open.Location = new Point(nextX, nextY);

      hamp_Open.VvColWdt      = new int[] { ZXC.QUN, ZXC.Q3un - ZXC.Qun2 };
      hamp_Open.VvSpcBefCol   = new int[] { 0, ZXC.Qun4 };
      hamp_Open.VvRightMargin = hamp_Open.VvLeftMargin;

      hamp_Open.VvRowHgt       = new int[] { ZXC.QUN };
      hamp_Open.VvSpcBefRow    = new int[] { 0 };
      hamp_Open.VvBottomMargin = hamp_Open.VvTopMargin;

      btn_PlusMinus     = hamp_Open.CreateVvButton(0, 0, new EventHandler(btn_PlusMinus_Click), "+");
      btn_PlusMinus.Tag = "PlusMinus";
      Label lblNaslov = hamp_Open.CreateVvLabel(1, 0, "Dodatno", ContentAlignment.MiddleLeft);

      lblCrta           = new Label();
      lblCrta.Parent    = this;
      lblCrta.Location  = new Point(hamp_Open.Right, hamp_Open.Top + ZXC.Qun4 + ZXC.Qun5);
      lblCrta.Size      = new Size(ZXC.Q4un, 1);
      lblCrta.BackColor = Color.LightGray;

      hamp_Open.BringToFront();
   }

   private void btn_PlusMinus_Click(object sender, System.EventArgs e)
   {
      Button btn = sender as Button;

      if (btn.Text == "+")
      {
         btn.Text = "-";
         hamp_bankaMjtr      .Visible = 
         hamp_person         .Visible =
         hamp_Spol           .Visible =
         hamp_vrstaRadVremena.Visible = 
         hamp_vrstaRadOdns   .Visible =
         hamp_VrtsaIsplate   .Visible =
         hamp_kontBrRsm      .Visible = true;

         hamp_bankaMjtr      .Location = new Point(nextX                                          , hamp_Open           .Top    + ZXC.QUN + ZXC.Qun2);
         hamp_person         .Location = new Point(nextX                                          , hamp_bankaMjtr      .Bottom + razmakIzmjedjuHampera);
         hamp_kontBrRsm      .Location = new Point(nextX                                          , hamp_person         .Bottom + razmakIzmjedjuHampera);
         hamp_Spol           .Location = new Point(nextX                                          , hamp_kontBrRsm      .Bottom + razmakIzmjedjuHampera);
         hamp_vrstaRadVremena.Location = new Point(hamp_Spol.Right + razmakIzmjedjuHampera        , hamp_kontBrRsm      .Bottom + razmakIzmjedjuHampera);
         hamp_VrtsaIsplate   .Location = new Point(ThePlacaFilterUC.nextX                         , hamp_vrstaRadVremena.Bottom + razmakIzmjedjuHampera);
         hamp_vrstaRadOdns   .Location = new Point(hamp_VrtsaIsplate.Right + razmakIzmjedjuHampera, hamp_vrstaRadVremena.Bottom + razmakIzmjedjuHampera);


       //  hamp_bankaMjtr.BringToFront();
                               
         lblCrta.Visible = false;

         this.CalcTheFilterPanelWidth();
         ThePlacaFilterUC.hamperHorLine.Location = new Point(nextX, hamp_vrstaRadOdns.Bottom + razmakIzmjedjuHampera);
      }

      else
      {
         btn.Text = "+";
         hamp_bankaMjtr      .Visible = 
         hamp_person         .Visible =
         hamp_Spol           .Visible =
         hamp_vrstaRadVremena.Visible = 
         hamp_vrstaRadOdns   .Visible =
         hamp_kontBrRsm      .Visible = 
         hamp_VrtsaIsplate   .Visible = false;
         lblCrta.             Visible = true;

         this.CalcTheFilterPanelWidth();
         ThePlacaFilterUC.hamperHorLine.Location = new Point(nextX, hamp_Open.Bottom + razmakIzmjedjuHampera);

      }
   }

   private void InitializeHamper_BankaMjTr(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", TheFilterPanel, false, nextX, hamp_jezik.Bottom, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q2un + ZXC.Qun2, ZXC.Q4un - ZXC.Qun4, ZXC.Q2un - ZXC.Qun8, ZXC.Q4un - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8,              ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
      }
      hamper.VvSpcBefRow = new int[] { ZXC.Qun4, ZXC.Qun8};

      hamper.VvBottomMargin = hamper.VvTopMargin;
   
      Label lbMjTr       = hamper.CreateVvLabel(0, 0, "MjTros:", ContentAlignment.MiddleRight);

      tbx_mtros_cd    = hamper.CreateVvTextBox(1, 0, "tbx_mtros_cd"   , "Šifra mjesta troška", 6);
      tbx_mtros_tk    = hamper.CreateVvTextBox(3, 0, "tbx_mtros_tk"   , "Tiker mjesta troška", 6);
      tbx_mtros_Naziv = hamper.CreateVvTextBox(1, 1, "tbx_mtros_naziv", "Naziv mjesta troška", 30, 2, 0);

      tbx_mtros_cd.JAM_CharEdits   = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_mtros_tk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_mtros_cd.   JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.    SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra)  , new EventHandler(AnyMtrosTextBoxLeave));
      tbx_mtros_tk.   JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_mtros_Naziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv. SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)   , new EventHandler(AnyMtrosTextBoxLeave));

      hamper.Visible = false;

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, ThePlacaFilterUC.MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void AnyBankaTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

 // ovo tu nesmije biti!!! - nepotrebno stavljati na reportUC - samo [teti a nekoristi
 //     if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob  kupdob_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_BankaCd    = kupdob_rec.KupdobCD/*RecID*/;
            Fld_BankaTk    = kupdob_rec.Ticker;
            Fld_BankaNaziv = kupdob_rec.Naziv;
         }
         else
         {
            Fld_BankaCdAsTxt = Fld_BankaTk = Fld_BankaNaziv = "";
         }
      }
   }
 
   private void AnyMtrosTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

//      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_MtrosCd    = kupdob_rec.KupdobCD/*RecID*/;
            Fld_MtrosTk    = kupdob_rec.Ticker;
            Fld_MtrosNaziv = kupdob_rec.Naziv;
         }
         else
         {
            Fld_MtrosCdAsTxt = Fld_MtrosTk = Fld_MtrosNaziv = "";
         }
      }
   }

   private void InitializeHamper_Person(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 2, "", TheFilterPanel, false, nextX, nextY, razmakIzmjedjuHampera);

      hamper.VvColWdt      = new int[] { ZXC.Q3un+ZXC.Qun2, ZXC.Q3un-ZXC.Qun2, ZXC.Q6un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun5, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun10;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.Visible = false;


      Label lbl_tip = hamper.CreateVvLabel        (0, 0, "Tip:"  , ContentAlignment.MiddleRight);
      tbx_ts        = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_ts", "Šifra tipa radnika", 6);
      tbx_ts.JAM_Set_LookUpTable(ZXC.luiListaPersonTS, (int)ZXC.Kolona.prva);
      tbx_ts.JAM_lookUp_NOTobligatory  = true;
      tbx_ts.JAM_lookUp_MultiSelection = true;
      tbx_ts.JAM_CharacterCasing       = CharacterCasing.Upper;

      Label lbl_strSpr = hamper.CreateVvLabel(0, 1, "StrSprema:", ContentAlignment.MiddleRight);

      tbx_strSprCd = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_strSprCd", "Šifra stručne spreme");
      tbx_strSpr   = hamper.CreateVvTextBox      (2, 1, "tbx_strSpr"  , "Stručna sprema"      );

      tbx_strSprCd.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_strSpr.JAM_ReadOnly    = true;
      tbx_strSprCd.JAM_Set_LookUpTable(ZXC.luiListaStrSprema, (int)ZXC.Kolona.prva);
      tbx_strSprCd.JAM_lui_NameTaker_JAM_Name = tbx_strSpr.JAM_Name;

      tbx_strSpr.Tag = ZXC.vvColors.userControl_BackColor;

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, ThePlacaFilterUC.MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_KontrolniBrRsm(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", TheFilterPanel, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q10un + ZXC.Q2un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8,           ZXC.Qun8};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
   
      Label lbl1              = hamper.CreateVvLabel  (0, 0, "Kontrolni broj za R-Sm obrazac:", ContentAlignment.MiddleRight);
      tbx_kontBrRsm = hamper.CreateVvTextBox(1, 0, "tbx_kontBrRsm", "");
      tbx_kontBrRsm.JAM_Highlighted = true;
      cbx_kontBrRsm = hamper.CreateVvCheckBox(1, 0, "", tbx_kontBrRsm, "", "X");

      hamper.Visible = false;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, ThePlacaFilterUC.MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHampeSpol(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 5, "", TheFilterPanel, false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun5;
      }
      //hamper.VvSpcBefRow[0] = ZXC.Qun2;

      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "Spol:", ContentAlignment.MiddleLeft);

      rbt_nepoznato = hamper.CreateVvRadioButton(0, 1, null, "Nebitno", TextImageRelation.ImageBeforeText);
      rbt_musko     = hamper.CreateVvRadioButton(0, 2, null, "Muški"  , TextImageRelation.ImageBeforeText);
      rbt_zensko    = hamper.CreateVvRadioButton(0, 3, null, "Ženski" , TextImageRelation.ImageBeforeText);
      
      rbt_nepoznato.Checked = true;
      rbt_nepoznato.Tag     = true;
     
      hamper.Visible = false;

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.HamperStyling(hamper);
   }
 
   private void InitializeHamperRadVr(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 5, "", TheFilterPanel, false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q7un + ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun5;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl     = hamper.CreateVvLabel      (0, 0,       "Vrsta radnog vremena:" , ContentAlignment.MiddleLeft);
      rbt_rvNebitno = hamper.CreateVvRadioButton(0, 1, null, "Nebitno"               , TextImageRelation.ImageBeforeText);
      rbt_rvPuno    = hamper.CreateVvRadioButton(0, 2, null, "Puno radno vrijeme"    , TextImageRelation.ImageBeforeText);
      rbt_rvNepun   = hamper.CreateVvRadioButton(0, 3, null, "Nepuno radno vrijeme"  , TextImageRelation.ImageBeforeText);
      rbt_rvSkr     = hamper.CreateVvRadioButton(0, 4, null, "Skraćeno radno vrijeme", TextImageRelation.ImageBeforeText);
      rbt_rvNebitno.Checked = true;
      rbt_rvNebitno.Tag     = true;

      hamper.Visible = false;

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, ThePlacaFilterUC.MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamperVrstIsplate(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 5, "", TheFilterPanel, false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun5;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl       = hamper.CreateVvLabel      (0, 0,       "Vrsta isplate:", ContentAlignment.MiddleLeft);
      rbt_ilNebitno   = hamper.CreateVvRadioButton(0, 1, null, "Nebitno"       , TextImageRelation.ImageBeforeText);
      rbt_tekuci      = hamper.CreateVvRadioButton(0, 2, null, "Tekući"        , TextImageRelation.ImageBeforeText);
      rbt_banka       = hamper.CreateVvRadioButton(0, 3, null, "Banka"         , TextImageRelation.ImageBeforeText);
      rbt_gotovina    = hamper.CreateVvRadioButton(0, 4, null, "Gotovina"      , TextImageRelation.ImageBeforeText);

      rbt_ilNebitno.Checked = true;
      rbt_ilNebitno.Tag     = true;

      hamper.Visible = false;

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamperRadOdnos(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 5, "", TheFilterPanel, false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q7un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun5;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl      = hamper.CreateVvLabel      (0, 0,       "Vrsta radnog odnosa:"  , ContentAlignment.MiddleLeft);
      rbt_roNebitno  = hamper.CreateVvRadioButton(0, 1, null, "Nebitno"               , TextImageRelation.ImageBeforeText);
      rbt_roNeodr    = hamper.CreateVvRadioButton(0, 2, null, "Neodređeno vrijeme"    , TextImageRelation.ImageBeforeText);
      rbt_roOdr      = hamper.CreateVvRadioButton(0, 3, null, "Određeno vrijeme"      , TextImageRelation.ImageBeforeText);
      rbt_roPripVjez = hamper.CreateVvRadioButton(0, 4, null, "Pripravnici/vježbenici", TextImageRelation.ImageBeforeText);
      rbt_roNebitno.Checked = true;
      rbt_roNebitno.Tag     = true;

      hamper.Visible = false;

      VvHamper.Open_Close_Fields_ForWriting(hamper, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, ThePlacaFilterUC.MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_Jezici(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 3, "", TheFilterPanel, false, nextX, nextY, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q5un,ZXC.Q2un,ZXC.Q2un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,ZXC.Qun4,ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Isplatna Lista jezik:", ContentAlignment.MiddleLeft);

      rbt_HRV = hamper.CreateVvRadioButton(1, 0, null, "HRV", TextImageRelation.ImageBeforeText);
      rbt_ENG = hamper.CreateVvRadioButton(2, 0, null, "ENG", TextImageRelation.ImageBeforeText);
      rbt_HRV.Checked = true;
      rbt_HRV.Tag     = true;

      cbx_pomakListe = hamper.CreateVvCheckBox_OLD(0, 1, null, 2, 0, "Pomak Liste za kovertu", System.Windows.Forms.RightToLeft.No);
      cbx_znpBezPnb  = hamper.CreateVvCheckBox_OLD(0, 2, null, 2, 0, "ZNP bez Pnb / Prikaz detalja na NP1", System.Windows.Forms.RightToLeft.No);

      VvHamper.HamperStyling(hamper);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper,ThePlacaFilterUC.MaxHamperWidth, razmakIzmjedjuHampera);
   }


   #region Eveniti rbt_tbx
   
   private void rbt_Click(object sender, System.EventArgs e)
   {
      RadioButton rbt = sender as RadioButton;
      rBtcurrChecked = OpenClose_VvTextBoxOnReportUC(rbt, rBtcurrChecked);
   }
   
   private RadioButton OpenClose_VvTextBoxOnReportUC(RadioButton rbt, RadioButton rBtcurrChecked)
   {
      VvTextBox vvTb = (VvTextBox)rbt.Tag;

      if(vvTb != null)
      {
         if(rbt != rBtcurrChecked)
         {
            VvHamper.Open_Close_Fields_ForWriting(vvTb, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvFindDialog);

            if(vvTb.Tag is VvDateTimePicker)
            {
               VvDateTimePicker dtp = (VvDateTimePicker)vvTb.Tag;
               dtp.Visible = true;
               dtp.Focus();
               vvTb.Visible = false;
            }
            else
            {
               vvTb.Focus();
               vvTb.SelectAll();
               vvTb.Enabled = true;
            }
         }
      }

      foreach(Control ctrl in hamp_rbtZaU.VvControlToBeParent.Controls)
      {
         if(ctrl is VvTextBox && ctrl != vvTb)
         {
            VvTextBox vvTbox = ctrl as VvTextBox;

            VvHamper.Open_Close_Fields_ForWriting(vvTbox, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvReportUC);
            vvTbox.Enabled   = false;
            vvTbox.BackColor = ZXC.vvColors.userControl_BackColor;
         }
      }
      return rbt;
   }

   private void tbx_DoubleClick(object sender, System.EventArgs e)
   {
      VvTextBox vvTb  = sender as VvTextBox;
      RadioButton rbt ;

      if(vvTb.Tag is DateTimePicker)
         rbt = (RadioButton)((DateTimePicker)vvTb.Tag).Tag;
      else
         rbt = (RadioButton)vvTb.Tag;

      if(!rbt.Checked) rbt.PerformClick();
   }

   #endregion Eveniti rbt_tbx

   #endregion Hampers

   #region Fld_

   public uint Fld_DokNum
   {
      get { return ZXC.ValOrZero_UInt(tbx_dokNum.Text); }
      set {                           tbx_dokNum.Text = value.ToString("000000"); }
   }
  
   public string Fld_RSmID
   {
      get { return tbx_rsmID.Text; }
      set {        tbx_rsmID.Text = value; }
   }
  
   public string Fld_Umjesecu
   {
      get { return tbx_uMj.Text; }
      set {        tbx_uMj.Text = value; }
   }

   public string Fld_ZaMjesec
   {
      get { return tbx_zaMj.Text; }
      set {        tbx_zaMj.Text = value; }
   }

   public DateTime Fld_UGodini
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dTP_uGod.Value);
      }
      set
      {
         dTP_uGod.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_uGod.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dTP_uGod);
      }
   }

   public DateTime Fld_ZaGodinu
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dTP_zaGod.Value);
      }
      set
      {
         dTP_zaGod.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_zaGod.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dTP_zaGod);
      }
   }

   public uint Fld_BankaCd
   {
      get { return tbx_banka_cd.GetSomeRecIDField(); }
      set {        tbx_banka_cd.PutSomeRecIDField(value); }
   }
  
   public string Fld_BankaCdAsTxt
   {
      get { return tbx_banka_cd.Text;         }
      set {        tbx_banka_cd.Text = value; }
   }

   public string Fld_BankaTk
   {
      get { return tbx_banka_tk.Text; }
      set {        tbx_banka_tk.Text = value; }
   }

   public string Fld_BankaNaziv
   {
      get { return tbx_banka_Naziv.Text; }
      set {        tbx_banka_Naziv.Text = value; }
   }

   public uint Fld_MtrosCd
   {
      get { return tbx_mtros_cd.GetSomeRecIDField(); }
      set {        tbx_mtros_cd.PutSomeRecIDField(value); }
   }

   public string Fld_MtrosCdAsTxt
   {
      get { return tbx_mtros_cd.Text;         }
      set {        tbx_mtros_cd.Text = value; }
   }

   public string Fld_MtrosTk
   {
      get { return tbx_mtros_tk.Text; }
      set {        tbx_mtros_tk.Text = value; }
   }

   public string Fld_MtrosNaziv
   {
      get { return tbx_mtros_Naziv.Text; }
      set {        tbx_mtros_Naziv.Text = value; }
   }
  
   public string Fld_StrSpr
   {
      get { return tbx_strSpr.Text; }
      set {        tbx_strSpr.Text = value; }
   }

   public string Fld_StrSprCd
   {
      get { return tbx_strSprCd.Text; }
      set {        tbx_strSprCd.Text = value; }
   }
    
   public string Fld_TS
   {
      get { return tbx_ts.Text; }
      set {        tbx_ts.Text = value; }
   }

   public ZXC.Spol? Fld_Spol
   {
      get
      {
              if(rbt_nepoznato.Checked) return ZXC.Spol.NEPOZNATO;
         else if(rbt_musko    .Checked) return ZXC.Spol.MUSKO    ;
         else if(rbt_zensko   .Checked) return ZXC.Spol.ZENSKO   ;

              else throw new Exception("Fld_Spol: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.Spol.NEPOZNATO: rbt_nepoznato.Checked = true; break;
            case ZXC.Spol.MUSKO    : rbt_musko    .Checked = true; break;
            case ZXC.Spol.ZENSKO   : rbt_zensko   .Checked = true; break;
         }
      }
   }

   public Person.VrstaRadnogVremenaEnum? Fld_VrstaRadVrem
   {
      get
      {
              if(rbt_rvPuno   .Checked)return Person.VrstaRadnogVremenaEnum.PUNO    ;
         else if(rbt_rvNepun  .Checked)return Person.VrstaRadnogVremenaEnum.NEPUNO  ;
         else if(rbt_rvSkr    .Checked)return Person.VrstaRadnogVremenaEnum.SKRACENO;
         else if(rbt_rvNebitno.Checked)return null;

              else throw new Exception("Fld_VrstaRadVrem: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case Person.VrstaRadnogVremenaEnum.PUNO    : rbt_rvPuno   .Checked = true; break;
            case Person.VrstaRadnogVremenaEnum.NEPUNO  : rbt_rvNepun  .Checked = true; break;
            case Person.VrstaRadnogVremenaEnum.SKRACENO: rbt_rvSkr    .Checked = true; break;
            default                                    : rbt_rvNebitno.Checked = true; break;
         }
      }
   }

   public Person.VrstaRadnogOdnosaEnum? Fld_VrstaRadOdns
   {
      get
      {
              if(rbt_roNeodr   .Checked)return Person.VrstaRadnogOdnosaEnum.NEODREDJENO;
         else if(rbt_roOdr     .Checked)return Person.VrstaRadnogOdnosaEnum.ODREDJENO  ;
         else if(rbt_roPripVjez.Checked)return Person.VrstaRadnogOdnosaEnum.PRIPR_VJEZB;
         else if(rbt_roNebitno .Checked)return null;

              else throw new Exception("Fld_VrstaRadOdns: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case Person.VrstaRadnogOdnosaEnum.NEODREDJENO: rbt_roNeodr   .Checked = true; break;
            case Person.VrstaRadnogOdnosaEnum.ODREDJENO  : rbt_roOdr     .Checked = true; break;
            case Person.VrstaRadnogOdnosaEnum.PRIPR_VJEZB: rbt_roPripVjez.Checked = true; break;
            default                                      : rbt_roNebitno .Checked = true; break;
         }
      }
   }

   public Person.VrstaIsplateEnum? Fld_VrstaIsplate
   {
      get
      {
              if(rbt_tekuci   .Checked)return Person.VrstaIsplateEnum.TEKUCI  ;
         else if(rbt_banka    .Checked)return Person.VrstaIsplateEnum.BANKA   ;
         else if(rbt_gotovina .Checked)return Person.VrstaIsplateEnum.GOTOVINA;
         else if(rbt_ilNebitno.Checked)return                             null;

              else throw new Exception("Fld_VrstaIsplate: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case Person.VrstaIsplateEnum.TEKUCI  : rbt_tekuci   .Checked = true; break;
            case Person.VrstaIsplateEnum.BANKA   : rbt_banka    .Checked = true; break;
            case Person.VrstaIsplateEnum.GOTOVINA: rbt_gotovina .Checked = true; break;
            default                              : rbt_ilNebitno.Checked = true; break;

         }
      }
   }

   public VvSQL.SorterType Fld_SifrarSort
   {
      get
      {
              if(rbt_SortSifra  .Checked) return VvSQL.SorterType.Code;
         else if(rbt_SortPrezime.Checked) return VvSQL.SorterType.Person;
         else                             return VvSQL.SorterType.None;
      }
      set
      {
         switch(value)
         {
            case VvSQL.SorterType.Code  : rbt_SortSifra  .Checked = true; break;
            case VvSQL.SorterType.Person: rbt_SortPrezime.Checked = true; break;
         }
      }
   }

   public bool Fld_UMjesecuChecked
   {
      get { return rbt_uMj.Checked; }
   }

   public bool Fld_ZaMjesecChecked
   {
      get { return rbt_zaMj.Checked; }
   }

   public bool Fld_UGodiniChecked
   {
      get { return rbt_uGod.Checked; }
   }
   
   public bool Fld_ZaGodinuChecked
   {
      get { return rbt_zaGod.Checked; }
   }

   public bool Fld_RSmIDChecked
   {
      get { return rbt_rsmID.Checked; }
   }

   public bool Fld_DokNumChecked
   {
      get { return rbt_dokNum.Checked; }
   }

   public bool Fld_StandardFilterChecked
   {
      get { return rbt_standardFilter.Checked; }
   }

   public bool Fld_ShowFileAfterExport
   {
      get { return cbx_showFileAfterExport.Checked; }
      set {        cbx_showFileAfterExport.Checked = value; }
   }

   public bool Fld_ShowHashCodeRsm
   {
      get { return cbx_kontBrRsm.Checked; }
      set {        cbx_kontBrRsm.Checked = value; }
   }

   public DateTime Fld_VirDatVal
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_virDatVal.Value);
      }
      set
      {
         dtp_virDatVal.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_virDateVal.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_virDatVal);
      }
   }

   public DateTime Fld_VirDatPod
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_virDatPod.Value);
      }
      set
      {
         dtp_virDatPod.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_virDatePod.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_virDatPod);
      }
   }

   public bool Fld_ShowVirDatVal
   {
      get { return cbx_showVirDatVal.Checked; }
      set {        cbx_showVirDatVal.Checked = value; }
   }

   public bool Fld_ShowVirDatPod
   {
      get { return cbx_showVirDatPod.Checked; }
      set {        cbx_showVirDatPod.Checked = value; }
   }

   public string Fld_Ziro1
   {
      //get { return combbx_Ziro1.Text; }
      //get { return combbx_Ziro1.SelectedItem.ToString(); }
      //get { return VvHamper.Get_ControlText_ThreadSafe(combbx_Ziro1); }
      get 
      {
         if(VvHamper.Get_ComboBoxSelectedItem_CallBack_ThreadSafe(combbx_Ziro1) == null) return "";

         return ((ZXC.CdAndName_CommonStruct)VvHamper.Get_ComboBoxSelectedItem_CallBack_ThreadSafe(combbx_Ziro1)).TheCd;
      }
      
      
      //set { combbx_Ziro1.SelectedItem = value; }
   }
  
   public VvSQL.SorterType Fld_DokumentSort
   {
      get
      {
         if     (rbt_SortDatum.Checked) return VvSQL.SorterType.DokDate;
         else if(rbt_SortBroj .Checked) return VvSQL.SorterType.DokNum;
         else                           return VvSQL.SorterType.None;
      }
      set
      {
         switch(value)
         {
            case VvSQL.SorterType.DokDate: rbt_SortDatum.Checked = true; break;
            case VvSQL.SorterType.DokNum : rbt_SortBroj .Checked = true; break;
         }
      }
   }
   public bool Fld_PrintPOR
   {
      get { return cbx_POR.Checked; }
      set {        cbx_POR.Checked = value; }
   }

   public bool Fld_PrintPRI
   {
      get { return cbx_PRI.Checked; }
      set {        cbx_PRI.Checked = value; }
   }

   public bool Fld_PrintMIO1
   {
      get { return cbx_MIO1.Checked; }
      set {        cbx_MIO1.Checked = value; }
   }

   public bool Fld_PrintMIO2
   {
      get { return cbx_MIO2.Checked; }
      set {        cbx_MIO2.Checked = value; }
   }

   public bool Fld_PrintZDR
   {
      get { return cbx_ZDR.Checked; }
      set {        cbx_ZDR.Checked = value; }
   }

   public bool Fld_PrintZOR
   {
      get { return cbx_ZOR.Checked; }
      set {        cbx_ZOR.Checked = value; }
   }

   public bool Fld_PrintZPI
   {
      get { return cbx_ZPI.Checked; }
      set {        cbx_ZPI.Checked = value; }
   }

   public bool Fld_PrintZAP
   {
      get { return cbx_ZAP.Checked; }
      set {        cbx_ZAP.Checked = value; }
   }

   public bool Fld_PrintZPP
   {
      get { return cbx_ZPP.Checked; }
      set {        cbx_ZPP.Checked = value; }
   }

   public bool Fld_PrintNET
   {
      get { return cbx_NET.Checked; }
      set {        cbx_NET.Checked = value; }
   }

   public bool Fld_PrintKRP
   {
      get { return cbx_KRP.Checked; }
      set {        cbx_KRP.Checked = value; }
   }

   public bool Fld_PrintMIO1NA
   {
      get { return cbx_MIO1NA.Checked; }
      set {        cbx_MIO1NA.Checked = value; }
   }

   public bool Fld_PrintMIO2NA
   {
      get { return cbx_MIO2NA.Checked; }
      set {        cbx_MIO2NA.Checked = value; }
   }

   public bool Fld_PrintPRE
   {
      get { return cbx_PRE.Checked; }
      set {        cbx_PRE.Checked = value; }
   }

   public int Fld_Pomak
   {
      get { return ZXC.ValOrZero_Int(tbx_pomak.Text); }
      set {                          tbx_pomak.Text = value.ToString(); }
   }

   public ZXC.JezikEnum? Fld_Jezik
   {
      get
      {
              if(rbt_ENG.Checked) return ZXC.JezikEnum.ENG;
              else                return ZXC.JezikEnum.HRV;
      }
      set
      {
         switch(value)
         {
            case ZXC.JezikEnum.HRV: rbt_HRV.Checked = true; break;
            case ZXC.JezikEnum.ENG: rbt_ENG.Checked = true; break;
         }
      }
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
         tbx_naDan.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_naDan);
      }
   }

   public bool Fld_IsPomakLista
   {
      get { return cbx_pomakListe.Checked; }
      set {        cbx_pomakListe.Checked = value; }
   }
   public bool Fld_IsZnpBezPnb
   {
      get { return cbx_znpBezPnb.Checked; }
      set {        cbx_znpBezPnb.Checked = value; }
   }

   public decimal Fld_GoObrPiPLimit
   {
      get { return tbx_goObrLimit.GetDecimalField(); }
      set { tbx_goObrLimit.PutDecimalField(value); }
   }

   public bool Fld_HocuGoPiP
   {
      get { return cbx_hocuGoPiP.Checked; }
      set { cbx_hocuGoPiP.Checked = value; }
   }

   public ZXC.VirmanBtchBookgKind Fld_VirmanGroup
   {
      get
      {
              if(rbt_BtchBookg_ONLY    .Checked) return ZXC.VirmanBtchBookgKind.BtchBookg_ONLY     ;
         else if(rbt_NON_BtchBookg_ONLY.Checked) return ZXC.VirmanBtchBookgKind.NON_BtchBookg_ONLY ;
         else                                    return ZXC.VirmanBtchBookgKind.ALL                ;
      }
      set
      {
         switch(value)
         {
            case ZXC.VirmanBtchBookgKind.BtchBookg_ONLY    : rbt_BtchBookg_ONLY    .Checked = true; break;
            case ZXC.VirmanBtchBookgKind.NON_BtchBookg_ONLY: rbt_NON_BtchBookg_ONLY.Checked = true; break;
            case ZXC.VirmanBtchBookgKind.ALL               : rbt_ALLvirman         .Checked = true; break;

         }
      }
   }


   #endregion Fld_

   #region PutFields(), GetFields()

   public override void GetFields(bool fuse)
   {
      GetFilterFields();
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheRptFilter = (VvRpt_Placa_Filter)_filter_data;

      if(TheRptFilter != null)
      {
         ThePlacaFilterUC.PutFilterFields(TheRptFilter);
        
         Fld_DokumentSort = TheRptFilter.SorterType_Dokument;
         Fld_SifrarSort = TheRptFilter.SorterType_Sifrar;

         Fld_DokNum     = TheRptFilter.DokNum;
         Fld_RSmID      = TheRptFilter.RSmID;
         Fld_Umjesecu   = TheRptFilter.UMjesecu;
         Fld_ZaMjesec   = TheRptFilter.ZaMjesec;
         Fld_UGodini    = TheRptFilter.UGodini;
         Fld_ZaGodinu   = TheRptFilter.ZaGodinu;
         Fld_MtrosCd    = TheRptFilter.MtrosCd;
         Fld_MtrosTk    = TheRptFilter.MTrosTk;
         Fld_MtrosNaziv = TheRptFilter.MtrosNaziv;
         Fld_BankaCd    = TheRptFilter.BankaCd;
         Fld_BankaTk    = TheRptFilter.BankaTk;
         Fld_BankaNaziv = TheRptFilter.BankaNaziv;

         Fld_ShowFileAfterExport = TheRptFilter.ShowFileAfterExport;
         Fld_ShowHashCodeRsm     = TheRptFilter.ShowHashCodeRsm;

         Fld_VirDatVal     = TheRptFilter.VirDateVal;
         Fld_VirDatPod     = TheRptFilter.VirDatePod;
         Fld_ShowVirDatVal = TheRptFilter.ShowVirDateVal;
         Fld_ShowVirDatPod = TheRptFilter.ShowVirDatePod;


         Fld_PrintPOR    = TheRptFilter.PrintPOR   ;   
         Fld_PrintPRI    = TheRptFilter.PrintPRI   ;  
         Fld_PrintMIO1   = TheRptFilter.PrintMIO1  ; 
         Fld_PrintMIO2   = TheRptFilter.PrintMIO2  ;
         Fld_PrintZDR    = TheRptFilter.PrintZDR   ;
         Fld_PrintZOR    = TheRptFilter.PrintZOR   ;
         Fld_PrintZPI    = TheRptFilter.PrintZPI   ;
         Fld_PrintZAP    = TheRptFilter.PrintZAP   ;
         Fld_PrintZPP    = TheRptFilter.PrintZPP   ;
         Fld_PrintNET    = TheRptFilter.PrintNET   ;
         Fld_PrintKRP    = TheRptFilter.PrintKRP   ;
         Fld_PrintMIO1NA = TheRptFilter.PrintMIO1NA;
         Fld_PrintMIO2NA = TheRptFilter.PrintMIO2NA;
         Fld_PrintPRE    = TheRptFilter.PrintPRE   ;

//         Fld_Ziro1        = TheRptFilter.Ziro1;
         Fld_VrstaIsplate = TheRptFilter.VrstaIsplate;
         Fld_VrstaRadOdns = TheRptFilter.VrstaRadOdnosa;
         Fld_VrstaRadVrem = TheRptFilter.VrstaRadVrem;
         Fld_Spol         = TheRptFilter.Spol;
         Fld_StrSprCd     = TheRptFilter.StrSpr ;
         Fld_TS           = TheRptFilter.TipPerson ;
         Fld_Jezik        = TheRptFilter.Jezik;

         Fld_NaDan        = TheRptFilter.NaDan;
         Fld_IsPomakLista = TheRptFilter.IsPomakLista;
         Fld_IsZnpBezPnb  = TheRptFilter.IsZnpBezPnb;
         Fld_GoObrPiPLimit = TheRptFilter.TolerancijaPoKO;
         Fld_HocuGoPiP     = TheRptFilter.HocuKonacniObrPor    ;

         Fld_VirmanGroup   = TheRptFilter.VirmanGroup;
        // Fld_Pomak        = TheRptFilter.VirPomak;
      }
      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
      ThePlacaFilterUC.GetFilterFields();
      
      TheRptFilter.SorterType_Dokument = Fld_DokumentSort;
      TheRptFilter.SorterType_Sifrar = Fld_SifrarSort;

      TheRptFilter.DokNum     = Fld_DokNum;
      TheRptFilter.RSmID      = Fld_RSmID;
      TheRptFilter.UMjesecu   = Fld_Umjesecu;
      TheRptFilter.ZaMjesec   = Fld_ZaMjesec;
      TheRptFilter.UGodini    = Fld_UGodini;
      TheRptFilter.ZaGodinu   = Fld_ZaGodinu;
      TheRptFilter.MtrosCd    = Fld_MtrosCd;
      TheRptFilter.MTrosTk    = Fld_MtrosTk;
      TheRptFilter.MtrosNaziv = Fld_MtrosNaziv;
      TheRptFilter.BankaCd    = Fld_BankaCd;
      TheRptFilter.BankaTk    = Fld_BankaTk;
      TheRptFilter.BankaNaziv = Fld_BankaNaziv;

      TheRptFilter.DokNumChecked   = Fld_DokNumChecked;
      TheRptFilter.RSmIDChecked    = Fld_RSmIDChecked;
      TheRptFilter.UMjesecuChecked = Fld_UMjesecuChecked;
      TheRptFilter.ZaMjesecChecked = Fld_ZaMjesecChecked;
      TheRptFilter.UGodiniChecked  = Fld_UGodiniChecked;
      TheRptFilter.ZaGodinuChecked = Fld_ZaGodinuChecked;

      TheRptFilter.StandardFilterChecked = Fld_StandardFilterChecked;

      TheRptFilter.ShowFileAfterExport = Fld_ShowFileAfterExport;
      TheRptFilter.ShowHashCodeRsm     = Fld_ShowHashCodeRsm ;

      TheRptFilter.VirDateVal     = Fld_VirDatVal;
      TheRptFilter.VirDatePod     = Fld_VirDatPod;
      TheRptFilter.ShowVirDateVal = Fld_ShowVirDatVal;
      TheRptFilter.ShowVirDatePod = Fld_ShowVirDatPod;

      TheRptFilter.Ziro1 = Fld_Ziro1;
      

      TheRptFilter.PrintPOR    = Fld_PrintPOR   ;   
      TheRptFilter.PrintPRI    = Fld_PrintPRI   ;  
      TheRptFilter.PrintMIO1   = Fld_PrintMIO1  ; 
      TheRptFilter.PrintMIO2   = Fld_PrintMIO2  ;
      TheRptFilter.PrintZDR    = Fld_PrintZDR   ;
      TheRptFilter.PrintZOR    = Fld_PrintZOR   ;
      TheRptFilter.PrintZPI    = Fld_PrintZPI   ;
      TheRptFilter.PrintZAP    = Fld_PrintZAP   ;
      TheRptFilter.PrintZPP    = Fld_PrintZPP   ;
      TheRptFilter.PrintNET    = Fld_PrintNET   ;
      TheRptFilter.PrintKRP    = Fld_PrintKRP   ;
      TheRptFilter.PrintMIO1NA = Fld_PrintMIO1NA;
      TheRptFilter.PrintMIO2NA = Fld_PrintMIO2NA;
      TheRptFilter.PrintPRE    = Fld_PrintPRE   ;

      TheRptFilter.VrstaIsplate   = Fld_VrstaIsplate;
      TheRptFilter.VrstaRadOdnosa = Fld_VrstaRadOdns;
      TheRptFilter.VrstaRadVrem   = Fld_VrstaRadVrem;
      TheRptFilter.Spol           = Fld_Spol;
      TheRptFilter.StrSpr         = Fld_StrSprCd;
      TheRptFilter.TipPerson      = Fld_TS;
      TheRptFilter.Jezik          = Fld_Jezik ;

      TheRptFilter.VirPomak          = Fld_Pomak;
      TheRptFilter.NaDan             = Fld_NaDan ;
      TheRptFilter.IsPomakLista      = Fld_IsPomakLista;
      TheRptFilter.IsZnpBezPnb       = Fld_IsZnpBezPnb;
      TheRptFilter.TolerancijaPoKO   = Fld_GoObrPiPLimit;
      TheRptFilter.HocuKonacniObrPor = Fld_HocuGoPiP;
      TheRptFilter.VirmanGroup       = Fld_VirmanGroup ;

   }

   #endregion PutFields(), GetFields()

   #region Report/Filter

   public VvPlacaReport TheVvPlaReport
   {
      get { return TheVvReport as VvPlacaReport; }
   }

   public VvRpt_Placa_Filter TheRptFilter
   {
      get;
      set;
   }

   #endregion Report/Filter

   #region Override

   public override VvRptFilter VirtualRptFilter
   {
      get { return this.TheRptFilter; }
   }

   public override void AddFilterMemberz()
   {
      ThePlacaFilterUC.AddFilterMemberz(TheRptFilter, TheVvPlaReport);
   }

   public override void ResetRptFilterRbCbControls()
   {
      ThePlacaFilterUC.cb_Line.Checked =
      cbx_showFileAfterExport.Checked  =    false;
      
      //cbx_showVirDatPod.Checked = cbx_showVirDatVal.Checked =
      rbt_SortDatum.Checked     = rbt_SortSifra.Checked     = 
      rbt_ilNebitno.Checked     = rbt_nepoznato.Checked     = 
      rbt_roNebitno.Checked     = rbt_rsmID.Checked         = 
      rbt_rvNebitno.Checked     = true;

      cbx_POR.Checked = cbx_PRI.Checked    = cbx_MIO1.Checked   = cbx_MIO2.Checked = cbx_ZDR.Checked =
      cbx_ZOR.Checked = cbx_ZPI.Checked    = cbx_ZAP.Checked    = cbx_ZPP.Checked  = cbx_NET.Checked =
      cbx_KRP.Checked = cbx_MIO1NA.Checked = cbx_MIO2NA.Checked = cbx_PRE.Checked  = true;


      dtp_virDatPod.Value = dtp_virDatVal.Value =  DateTime.Now;
      SetLastPlacaDokumData_tbx();
      rbt_rsmID.PerformClick();

      if(combbx_Ziro1.Items.Count.NotZero()) combbx_Ziro1.SelectedItem = combbx_Ziro1.Items[0];

   }

   private void SetLastPlacaDokumData_tbx()
   {
      Placa placa_rec = new Placa();

      bool OK = placa_rec.VvDao.FrsPrvNxtLst_REC(ZXC.TheVvForm.TheDbConnection, placa_rec, VvSQL.DBNavigActionType.LST, Placa.sorterDokDate, false, ZXC.DbNavigationRestrictor.Empty, ZXC.DbNavigationRestrictor.Empty, ZXC.DbNavigationRestrictor.Empty);

      if(!OK) return;

      tbx_dokNum.Text = placa_rec.DokNum.ToString("000000");
      tbx_rsmID.Text  = placa_rec.RSm_ID.ToString();
      tbx_uMj.Text    = placa_rec.Umjesecu_DokDateAsMMYYYY.ToString();
      tbx_zaMj.Text   = placa_rec.MMYYYY.ToString();
      tbx_uGod.Text   =
      tbx_zaGod.Text  = ZXC.projectYearFirstDay.Year.ToString();
      dTP_uGod.Value  =
      dTP_zaGod.Value = ZXC.projectYearFirstDay;

      tbx_naDan.Text  = placa_rec.DokDate.ToString();
      dtp_naDan.Value = placa_rec.DokDate;
   }

   #endregion Override
}

public class PlaFilterUC : VvFilterUC
{
   #region Fieldz

   public VvHamper    hamp_dateIzvj, hamp_OdDo, hamp_tt, hamp_card, hamp_rootPrjkt;
   private VvTextBox  tbx_personCdOd, tbx_personCdDo, tbx_prezimeOd, tbx_prezimeDo, tbx_dokNumOD, tbx_dokNumDO,
                      tbx_TTopis, tbxLookUp_TT, tbx_vrObrOpis, tbxLookUp_vrObr, tbx_rootPrjkt;

   #endregion Fieldz

   #region Constructor

   public PlaFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();

      TheVvUC = vvUC;
      
      InitializeHamper_DateIzvj(out hamp_dateIzvj);
      nextY = hamp_dateIzvj.Bottom + razmakIzmjedjuHampera;

      InitializeHamper_OdDo(out hamp_OdDo);

      MaxHamperWidth = hamp_OdDo.Width;
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamp_dateIzvj, MaxHamperWidth, razmakIzmjedjuHampera);

      CreateHamper_4ButtonsResetGo_Width(MaxHamperWidth);

      nextY = hamp_OdDo.Bottom + razmakIzmjedjuHampera;

      InitializeHamper_tt(out hamp_tt);

      nextY = hamp_tt.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_rootPrjkt(out hamp_rootPrjkt);

      nextY = hamp_rootPrjkt.Bottom + razmakIzmjedjuHampera;

      nextY = LocationOfHamper_HorLine(nextX, nextY, MaxHamperWidth) + razmakIzmjedjuHampera;

      this.Size = new Size(MaxHamperWidth + 2 * razmakIzmjedjuHampera, nextY + ZXC.QunMrgn);

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();

      // TODO: !!! votafak, SHVATI ZASTO implicitno loudanje ovdje ne radi??
      // ako ovo ne pozoves onda fali CalcTrans-u. Zasto se LuiListe init samo pri VvTextBox...
      ZXC.luiListaVrstaRadaEVR.LazyLoad();
   }



   #endregion Constructor

   #region Hamper_s
   
   private void InitializeHamper_DateIzvj(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q7un + ZXC.Qun2, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] {            ZXC.Qun8, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbdate;

      lbdate = hamper.CreateVvLabel(0, 0, "Datum formiranja izvještaja:", ContentAlignment.MiddleRight);

      tbx_dateIzvj      = hamper.CreateVvTextBox(1, 0, "tbx_dateIzvj", "");
      tbx_dateIzvj.JAM_IsForDateTimePicker = true;
      dtp_dateIzvj      = hamper.CreateVvDateTimePicker(1, 0, "", tbx_dateIzvj);
      dtp_dateIzvj.Name = "dtp_dateIzvj";
      dtp_dateIzvj.Tag  = tbx_dateIzvj;
      tbx_dateIzvj.Tag  = dtp_dateIzvj;

      tbx_dateIzvj.JAM_WriteOnly = true;

      VvHamper.HamperStyling(hamper);

   }

   private void InitializeHamper_OdDo(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 5, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q2un + ZXC.Qun2 +ZXC.Qun4, ZXC.Q5un, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] {                      ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbOd, lbDo, lbDatum, lbSifra, lbNaziv, lbTrans;

      lbOd = hamper.CreateVvLabel(1, 0, "OD:", ContentAlignment.MiddleCenter);
      lbDo = hamper.CreateVvLabel(2, 0, "DO:", ContentAlignment.MiddleCenter);

      lbDatum          = hamper.CreateVvLabel(0, 1, "Datum:", ContentAlignment.MiddleRight);
      tbx_DatumOD      = hamper.CreateVvTextBox(1, 1, "tbx_datumOd", "");
      tbx_DatumOD.JAM_IsForDateTimePicker = true;
      dtp_DatumOD      = hamper.CreateVvDateTimePicker(1, 1, "", tbx_DatumOD);
      dtp_DatumOD.Name = "dtp_datumOd";
      dtp_DatumOD.Tag  = tbx_DatumOD;
      tbx_DatumOD.Tag  = dtp_DatumOD;

      tbx_DatumDO      = hamper.CreateVvTextBox(2, 1, "tbx_datumDo", "");
      tbx_DatumDO.JAM_IsForDateTimePicker = true;
      dtp_DatumDO      = hamper.CreateVvDateTimePicker(2, 1, "", tbx_DatumDO);
      dtp_DatumDO.Name = "dtp_datumDo";
      dtp_DatumDO.Tag  = tbx_DatumDO;
      tbx_DatumDO.Tag  = dtp_DatumDO;

      tbx_DatumOD.JAM_WriteOnly = tbx_DatumDO.JAM_WriteOnly = true;

      lbSifra = hamper.CreateVvLabel(0, 2, "Sifra:", ContentAlignment.MiddleRight);
      lbNaziv = hamper.CreateVvLabel(0, 3, "Prezime:", ContentAlignment.MiddleRight);

      tbx_personCdOd = hamper.CreateVvTextBox(1, 2, "tbx_personCdOd", "", 6);
      tbx_personCdDo = hamper.CreateVvTextBox(2, 2, "tbx_personCdDo", "", 6);

      tbx_prezimeOd = hamper.CreateVvTextBox(1, 3, "tbx_prezimeOd", "", 30);
      tbx_prezimeDo = hamper.CreateVvTextBox(2, 3, "tbx_prezimeDo", "", 30);

      tbx_personCdOd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_personCdOd.JAM_FillCharacter = '0';
      tbx_personCdOd.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_personCdDo.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_personCdDo.JAM_FillCharacter = '0';
      tbx_personCdDo.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      tbx_personCdOd.JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Person_sorterSifra), null);
      tbx_personCdDo.JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Person_sorterSifra), null);

      tbx_prezimeOd.JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), null);
      tbx_prezimeDo.JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), null);

      // 13.02.2020: HZTK... 
      // ovdje ipak treba jer tu nema 'EventHandler onVvTextBoxLeave_SetDependendTakers'
      tbx_prezimeOd.JAM_FieldExitMethod = new EventHandler(OnExitPrezime_ClearPreffix);
      tbx_prezimeDo.JAM_FieldExitMethod = new EventHandler(OnExitPrezime_ClearPreffix);

      tbx_personCdOd.JAM_WriteOnly = tbx_personCdDo.JAM_WriteOnly = true;
      tbx_prezimeOd.JAM_WriteOnly = tbx_prezimeDo.JAM_WriteOnly = true;

      tbx_personCdOd.JAM_MustTabOutBeforeSubmit = tbx_personCdDo.JAM_MustTabOutBeforeSubmit = true;
      tbx_prezimeOd.JAM_MustTabOutBeforeSubmit = tbx_prezimeDo.JAM_MustTabOutBeforeSubmit = true;

      tbx_personCdOd.TextAlign = tbx_personCdDo.TextAlign = HorizontalAlignment.Left;

      tbx_DatumOD.ContextMenu = dtp_DatumOD.ContextMenu =
                                tbx_DatumDO.ContextMenu =
                                dtp_DatumDO.ContextMenu = CreateNewContexMenu_Date();

      lbTrans = hamper.CreateVvLabel(0, 4, "DokBr:", ContentAlignment.MiddleRight);

      tbx_dokNumOD = hamper.CreateVvTextBox(1, 4, "tbx_BrTransOD", "", 6);
      tbx_dokNumOD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_dokNumOD.JAM_FillCharacter = '0';
      tbx_dokNumOD.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      tbx_dokNumDO = hamper.CreateVvTextBox(2, 4, "tbx_BrTransDO", "", 6);
      tbx_dokNumDO.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_dokNumDO.JAM_FillCharacter = '0';
      tbx_dokNumDO.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      VvHamper.HamperStyling(hamper);
   }

   public void OnExitPrezime_ClearPreffix(object sender, EventArgs e)
   {
      string dirtyString = "", thePrezime;

      VvTextBox vvTbPrezime = null;

      vvTbPrezime = sender as VvTextBox;

      if(vvTbPrezime == null) return;

      dirtyString = vvTbPrezime.Text;

      int commaIdx = dirtyString.IndexOf(',');

      if(dirtyString.Length.NotZero() && commaIdx.IsZeroOrPositive()) //return;
      { 
         thePrezime = dirtyString.Substring(0, commaIdx);
      }
      else
      {
         thePrezime = dirtyString;
      }

      vvTbPrezime.Text =  thePrezime;

   }

   private void InitializeHamper_tt(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 2, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q2un + ZXC.Qun2, ZXC.Q2un + ZXC.Qun2, ZXC.Q7un + ZXC.Qun2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbVrKnjz;

      lbVrKnjz     = hamper.CreateVvLabel        (0, 0, "TT:", ContentAlignment.MiddleRight);
      tbx_TTopis   = hamper.CreateVvTextBox      (2, 0, "tbx_TtOpis", "");
      tbxLookUp_TT = hamper.CreateVvTextBoxLookUp(1, 0, "tbxLookUp_TT", "");

      tbxLookUp_TT.JAM_CharacterCasing = CharacterCasing.Upper;
      tbxLookUp_TT.JAM_WriteOnly = true;
      tbxLookUp_TT.JAM_Set_LookUpTable(ZXC.luiListaPlacaTT, (int)ZXC.Kolona.prva);
      tbxLookUp_TT.JAM_lui_NameTaker_JAM_Name = tbx_TTopis.JAM_Name;

      tbx_TTopis.JAM_ReadOnly = true;
      tbx_TTopis.Tag = ZXC.vvColors.userControl_BackColor;

      Label  lbVrObr = hamper.CreateVvLabel         (0, 1, "VrObr:", ContentAlignment.MiddleRight);
      tbx_vrObrOpis   = hamper.CreateVvTextBox      (2, 1, "tbx_vrObrOpis", "");
      tbxLookUp_vrObr = hamper.CreateVvTextBoxLookUp(1, 1, "tbxLookUp_vrObr", "");

      tbxLookUp_vrObr.JAM_WriteOnly = true;
      tbxLookUp_vrObr.JAM_Set_LookUpTable(ZXC.luiListaPlacaVrstaObr, (int)ZXC.Kolona.prva);
      tbxLookUp_vrObr.JAM_lui_NameTaker_JAM_Name = tbx_vrObrOpis.JAM_Name;

      tbx_vrObrOpis.JAM_ReadOnly = true;
      tbx_vrObrOpis.Tag = ZXC.vvColors.userControl_BackColor;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
      hamper.BringToFront();
   }

   private void InitializeHamper_rootPrjkt(out VvHamper hamper)
   {
         hamper = new VvHamper(2, 1, "", this, false, nextX, nextY, razmakHampGroup);

         hamper.VvColWdt      = new int[] { ZXC.Q10un - ZXC.Qun2, ZXC.Q3un };
         hamper.VvSpcBefCol   = new int[] {             ZXC.Qun4, ZXC.Qun8 };
         hamper.VvRightMargin = hamper.VvLeftMargin;

         for(int i = 0; i < hamper.VvNumOfRows; i++)
         {
            hamper.VvRowHgt[i] = ZXC.QUN;
            hamper.VvSpcBefRow[i] = ZXC.Qun8;
         }
         hamper.VvBottomMargin = hamper.VvTopMargin;

         Label lbl_rootPrjkt;

         lbl_rootPrjkt = hamper.CreateVvLabel(0, 0, "Glavni projekt (eRS-m):", ContentAlignment.MiddleRight);
         tbx_rootPrjkt = hamper.CreateVvTextBox(1, 0, "tbx_rootPrjkt", "", 6);
         tbx_rootPrjkt.JAM_FillCharacter = '0';
         tbx_rootPrjkt.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
         tbx_rootPrjkt.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
         tbx_rootPrjkt.TextAlign     = HorizontalAlignment.Left;
         VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
         VvHamper.HamperStyling(hamper);

   }

   #endregion Hamper_s

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
         tbx_DatumOD.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumOD);
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
  
   public DateTime Fld_DateIzvj
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_dateIzvj.Value);
      }
      set
      {
         dtp_dateIzvj.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_dateIzvj.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_dateIzvj);
      }
   }

   public uint Fld_PersonCdOd
   {
      get { return ZXC.ValOrZero_UInt(tbx_personCdOd.Text); }
      set { tbx_personCdOd.Text = value.ToString("0000"); }
   }

   public uint Fld_PersonCdDo
   {
      get { return ZXC.ValOrZero_UInt(tbx_personCdDo.Text); }
      set { tbx_personCdDo.Text = value.ToString("0000"); }
   }

   public string Fld_PrezimeOd
   {
      get { return tbx_prezimeOd.Text; }
      set { tbx_prezimeOd.Text = value; }
   }

   public string Fld_PrezimeDo
   {
      get { return tbx_prezimeDo.Text; }
      set { tbx_prezimeDo.Text = value; }
   }

   public uint Fld_DokNumOd
   {
      get { return ZXC.ValOrZero_UInt(tbx_dokNumOD.Text); }
      set { tbx_dokNumOD.Text = value.ToString("000000"); }
   }

   public uint Fld_DokNumDo
   {
      get { return ZXC.ValOrZero_UInt(tbx_dokNumDO.Text); }
      set { tbx_dokNumDO.Text = value.ToString("000000"); }
   }

   public string Fld_TT
   {
      get { return tbxLookUp_TT.Text; }
      set { tbxLookUp_TT.Text = value; }
   }

   public bool Fld_NeedsHorizontalLine
   {
      get { return cb_Line.Checked; }
      set { cb_Line.Checked = value; }
   }

   public uint Fld_RootPrjkt
   {
      get { return ZXC.ValOrZero_UInt(tbx_rootPrjkt.Text); }
      set {                           tbx_rootPrjkt.Text = value.ToString("000000"); }
   }

   public string Fld_VrObr
   {
      get { return tbxLookUp_vrObr.Text; }
      set {        tbxLookUp_vrObr.Text = value; }
   }


   #endregion Fld_

   #region PutFilterFields(), GetFilterFields()

   private VvRpt_Placa_Filter ThePlacaFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as VvRpt_Placa_Filter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      ThePlacaFilter = (VvRpt_Placa_Filter)_filter_data;

      if(ThePlacaFilter != null)
      {
         Fld_PersonCdOd = ThePlacaFilter.PersonCDod;
         Fld_PersonCdDo = ThePlacaFilter.PersonCDdo;
         Fld_PrezimeOd  = ThePlacaFilter.PrezimeOd;
         Fld_PrezimeDo  = ThePlacaFilter.PrezimeDo;
         Fld_DokNumOd   = ThePlacaFilter.DokNumOd;
         Fld_DokNumDo   = ThePlacaFilter.DokNumDo;
         Fld_TT         = ThePlacaFilter.TT;
         Fld_DatumOd    = ThePlacaFilter.DatumOd;
         Fld_DatumDo    = ThePlacaFilter.DatumDo;
         Fld_DateIzvj   = ThePlacaFilter.DateIzvj;
         Fld_VrObr      = ThePlacaFilter.VrObr;



         Fld_NeedsHorizontalLine = ThePlacaFilter.NeedsHorizontalLine;

         Fld_RootPrjkt = ZXC.TheVvForm.VvPref.login.rootPrjktKCD.NotZero() ? ZXC.TheVvForm.VvPref.login.rootPrjktKCD : 1;

      }
      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
      ThePlacaFilter.PersonCDod = Fld_PersonCdOd;
      ThePlacaFilter.PersonCDdo = Fld_PersonCdDo;
      ThePlacaFilter.PrezimeOd  = Fld_PrezimeOd;
      ThePlacaFilter.PrezimeDo  = Fld_PrezimeDo;
      ThePlacaFilter.DokNumOd   = Fld_DokNumOd;
      ThePlacaFilter.DokNumDo   = Fld_DokNumDo;
      ThePlacaFilter.TT         = Fld_TT;
      ThePlacaFilter.DatumOd    = Fld_DatumOd;
      ThePlacaFilter.DatumDo    = Fld_DatumDo;
      ThePlacaFilter.DateIzvj   = Fld_DateIzvj;
      ThePlacaFilter.VrObr      = Fld_VrObr ;
      
      ThePlacaFilter.NeedsHorizontalLine = Fld_NeedsHorizontalLine;

      if(Fld_RootPrjkt.NotZero()) ZXC.TheVvForm.VvPref.login.rootPrjktKCD = Fld_RootPrjkt;

      //string origDbName = ZXC.TheMainDbConnection.Database;

      //ZXC.PrjktDao.SetMe_Record_bySomeUniqueColumn(ZXC.PrjConnection, ThePlacaFilter.PlacaRootPrjkt_rec, Fld_RootPrjkt, ZXC.KupdobSchemaRows[ZXC.KpdbCI.kupdobCD], false, false);

      //ZXC.SetMainDbConnDatabaseName(origDbName); // jer PrjktDao.SetMe_Record_bySomeUniqueColumn promijeni na 'vvektor' ... 

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Prjkt>(null, VvSQL.SorterType.Name);
    //ThePlacaFilter.PlacaRootPrjkt_rec = VvUserControl.PrjktSifrar.Single(prj => prj.KupdobCD == ZXC.TheVvForm.VvPref.login.rootPrjktKCD);
      ThePlacaFilter.PlacaRootPrjkt_rec = VvUserControl.PrjktSifrar.SingleOrDefault(prj => prj.KupdobCD == ZXC.TheVvForm.VvPref.login.rootPrjktKCD);
      if(ThePlacaFilter.PlacaRootPrjkt_rec == null) ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE.\n\nNEMA Projekta [{0}]", ZXC.TheVvForm.VvPref.login.rootPrjktKCD);
   }

   #endregion PutFilterFields(), GetFilterFields()

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
      VvRpt_Placa_Filter theRptFilter   = (VvRpt_Placa_Filter)_vvRptFilter;
      VvPlacaReport      theVvPlaReport = (VvPlacaReport)     _vvReport;
      
      DateTime dateOD, dateDO;
      string   comparer;
      string text, textOD, textDO, text2;
      bool     isDummy, isCheck;
      uint     num, numOD, numDO;
      DataRow  drSchema;
      Person.VrstaIsplateEnum?       vrIspl;
      Person.VrstaRadnogOdnosaEnum?  vrRadOd;
      Person.VrstaRadnogVremenaEnum? vrRadVre;

      theRptFilter.FilterMembers.Clear();
      theRptFilter.ClearAllFilters_FromClauseGotTableName();

      isDummy = false;

      //       
      // Nota bene!!!                                                                                                                                                   
      // U FIlterMember-u: 'relatedTable' treba zadati kada podatka po kojemu filtriramo NEMA u ptrans-u, vec dolazi iz neke druge tablice (npr. Person, Placa, ...)    
      // U FIlterMember-u: 'forcedPreffix' treba zadati kada se podatak po kojemu filtriramo nalazi i u Ptransu i u (Ptrane-ou ili Ptrano-u)                            
      //       


      // !!! PO PRVI PUT ISPROBAVAMO OVAKO. Ovo je FilterMember dodan ne zbogradi FUlterUC.GetFields vec doticni VvReport fiksno zahtjeva neki FilterMember 
      drSchema = ZXC.PlacaSchemaRows[ZXC.PlaCI.vrstaObr];
      text     = theRptFilter.VrObr;

      // classic first 
      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "VrstaObracuna", text, text, "Vrsta Obracuna:", " = ", Placa.recordName));
      }

      // Todo: !!! Tamara za koji nam je ovo ku'ac trebalo?
      // Special, new stuff. Force Skip unwanted VrstaObracuna '00'
      if(theVvPlaReport != null && theVvPlaReport.VvSkip_VrstaObracuna_00 == true)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "VrstaObrNot00", Placa.VrObr_NeisplacenaPlaca, "", "Vrsta Obracuna:", " != ", Placa.recordName));
      }

      // Fld_DatumOdDo                                                                                                                                  

      if(theRptFilter.StandardFilterChecked == true)
      {
         drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_dokDate];

         dateOD = theRptFilter.DatumOd;
         dateDO = theRptFilter.DatumDo;

         if(dateOD == dateDO) comparer = " = ";
         else                 comparer = " >= ";

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DateOD", dateOD, dateOD.ToString("dd.MM.yyyy."), "Od datuma:", comparer, "", Ptrans.recordName));

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DateDO", dateDO, dateDO.ToString("dd.MM.yyyy."), "Do datuma:", " <= "  , "", Ptrans.recordName));
      }

      // Fld_DokNumOdDo                                                                                                                                   

      if(theRptFilter.StandardFilterChecked == true)
      {
         drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_dokNum];

         numOD = theRptFilter.DokNumOd;
         numDO = theRptFilter.DokNumDo;

         if(numOD.NotZero())
         {
            if(numOD == numDO) comparer = " = ";
            else               comparer = " >= ";

            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "BrDokOD", numOD, numOD.ToString("000000"), "Od dokumenta broj:", comparer, "", Ptrans.recordName));
         }
         else if(numDO.NotZero())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true,  "BrDokOD", numOD, "-",                      "Od dokumenta broj:", " >= ",   "", Ptrans.recordName));
         }

         if(numDO.NotZero())
         {
            if(numDO == numOD) isDummy = true;
            else               isDummy = false;

            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "BrDokDO", numDO, numDO.ToString("000000"), "Do dokumenta broj:", " <= ", "", Ptrans.recordName));
         }
         else if(numOD.NotZero())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true,    "BrDokDO", numDO, "-",                      "Do dokumenta broj:", " <= ", "", Ptrans.recordName));
         }

         isDummy = false;

      }
      
      // Fld_DokNum (jedan, jedini)                                                                                                                                   

      if(theRptFilter.DokNumChecked == true)
      {
         drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_dokNum];

         num = theRptFilter.DokNum;

         if(num.NotZero())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "BrDokOD", num, num.ToString("000000"), "Od dokumenta broj:", " = ", "", Ptrans.recordName));
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true , "BrDokDO", num, num.ToString("000000"), "Do dokumenta broj:", " = ", "", Ptrans.recordName));
         }
      }
      
      //Fld_ZaMMYYYY (ZA mjesec)                                                                                                                                      

      if(theRptFilter.ZaMjesecChecked == true)
      {
         drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_mmyyyy];
         text     = theRptFilter.ZaMjesec;

         if(text.NotEmpty())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "ZaMj", text, text, "Za mjesec:", " = ", ""));
         }
      }

      //Fld_ZaYYYY (ZA godinu)                                                                                                                            

      if(theRptFilter.ZaGodinuChecked == true)
      {
         drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_mmyyyy];
         text     = theRptFilter.ZaGodinu.ToString("yyyy");

         if(text.NotEmpty())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "ZaMj", "__" + text, text, "Za godinu:", " LIKE ", ""));
         }
      }

      //Fld_U MMYYYY (U mjesecu)                                                                                                                                      

      if(theRptFilter.UMjesecuChecked == true)
      {
         drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_dokDate];
         text     = theRptFilter.UMjesecu;

         if(text.NotEmpty())
         {
            dateOD = Placa.GetDateTimeFromMMYYYY(text, false);
            dateDO = Placa.GetDateTimeFromMMYYYY(text, true);

            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DateOD", dateOD, dateOD.ToString("dd.MM.yyyy."), "Od:", " >= ", "", Ptrans.recordName));

            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DateDO", dateDO, dateDO.ToString("dd.MM.yyyy."), "Do:", " <= ", "", Ptrans.recordName));
         }
      }

      //Fld_U YYYY (U godini)                                                                                                                                      

      if(theRptFilter.UGodiniChecked == true)
      {
         drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_dokDate];
         text     = theRptFilter.UGodini.ToString("yyyy");

         if(text.NotEmpty())
         {
            dateOD = Placa.GetDateTimeFromYYYY(text, false);
            dateDO = Placa.GetDateTimeFromYYYY(text, true);

            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DateOD", dateOD, dateOD.ToString("dd.MM.yyyy."), "Od:", " >= ", "", Ptrans.recordName));

            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DateDO", dateDO, dateDO.ToString("dd.MM.yyyy."), "Do:", " <= ", "", Ptrans.recordName));
         }
      }

      // Fld_RSmID                                                                                                                                     

      if(theRptFilter.RSmIDChecked == true)
      {
         drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_rSm_ID];
         text     = theRptFilter.RSmID;

         if(text.NotEmpty())
         {
          //theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "RSmID", text, text, "Za RSmID:", " = ", ""));
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "RSmID", text, text, "Za JOPPD:", " = ", ""));
         }
      }

      // Fld_PersonCDOdDo                                                                                                                                     

      drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_personCD];
      numOD    = theRptFilter.PersonCDod;
      numDO    = theRptFilter.PersonCDdo;

      if(numOD != 0)
      {
         if(numOD == numDO) comparer = " = ";
         else               comparer = " >= ";

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "PersonCDOD", numOD, numOD.ToString("0000"), "Od ŠifDjelat:", comparer, "", Ptrans.recordName));
      }
      else if(numDO != 0)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true,    "PersonCDOD", numOD, "-",                    "Od ŠifDjelat:", " >= ",   "", Ptrans.recordName));
      }

      if(numDO != 0)
      {
         if(numDO == numOD) isDummy = true;
         else               isDummy = false;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "PersonCDDO", numDO, numDO.ToString("0000"), "Do ŠifDjelat:", " <= ", "", Ptrans.recordName));
      }
      else if(numOD != 0)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true,    "PersonCDDO", numDO, "-",                    "Do ŠifDjelat:", " <= ", "", Ptrans.recordName));
      }

      isDummy = false;

      // Fld_PrezimeOdDo                                                                                                                                     

      drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_prezime];

      textOD = theRptFilter.PrezimeOd;
      textDO = theRptFilter.PrezimeDo;

      if(textOD.NotEmpty())
      {
         if(textOD == textDO) comparer = " = ";
         else                 comparer = " >= ";

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "PrezimeOD", textOD, textOD, "Od prezimena:", comparer, "", Ptrans.recordName));
      }
      else if(textDO.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true,  "PrezimeOD", textOD, "-",    "Od prezimena:", " >= ",   "", Ptrans.recordName));
      }

      if(textDO.NotEmpty())
      {
         if(textDO == textOD) isDummy = true;
         else isDummy = false;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "PrezimeDO", textDO, textDO, "Do prezimena:", " <= ", "", Ptrans.recordName));
      }
      else if(textOD.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true,    "PrezimeDO", textDO, "-",    "Do prezimena:", " <= ", "", Ptrans.recordName));
      }

      isDummy = false;

      // Fld_TT classic first                                                                                                                                    

      drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_tt];
      text     = theRptFilter.TT;

      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TipTran", text, text, "Za tip plaće:", " = ", "", Ptrans.recordName));
      }

      // Special, new stuff. Force Skip unwanted TT not 'RR & PP' 
      if(theVvPlaReport != null && theVvPlaReport.VvLet_TT_RRiPP_Only == true)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotAH", Placa.TT_AUTORHONOR  , "", "Za tip plaće:", " != ", "", Ptrans.recordName));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotAU", Placa.TT_AUTORHONUMJ , "", "Za tip plaće:", " != ", "", Ptrans.recordName));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotSU", Placa.TT_AHSAMOSTUMJ , "", "Za tip plaće:", " != ", "", Ptrans.recordName));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotK4", Placa.TT_IDD_KOLONA_4, "", "Za tip plaće:", " != ", "", Ptrans.recordName));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotNO", Placa.TT_NADZORODBOR , "", "Za tip plaće:", " != ", "", Ptrans.recordName));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotTV", Placa.TT_TURSITVIJECE, "", "Za tip plaće:", " != ", "", Ptrans.recordName));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotNP", Placa.TT_NEOPOREZPRIM, "", "Za tip plaće:", " != ", "", Ptrans.recordName));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotPD", Placa.TT_POREZNADOBIT, "", "Za tip plaće:", " != ", "", Ptrans.recordName));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotSO", Placa.TT_STRUCNOOSPOS, "", "Za tip plaće:", " != ", "", Ptrans.recordName));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotSZ", Placa.TT_SEZZAPPOLJOP, "", "Za tip plaće:", " != ", "", Ptrans.recordName));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotUG", Placa.TT_UGOVORODJELU, "", "Za tip plaće:", " != ", "", Ptrans.recordName));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotDD", Placa.TT_DDBEZDOPRINO, "", "Za tip plaće:", " != ", "", Ptrans.recordName)); //12.2018.
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotA2", Placa.TT_AUVECASTOPA , "", "Za tip plaće:", " != ", "", Ptrans.recordName)); //12.2018.
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotN1", Placa.TT_NR1_PX1NEDOP, "", "Za tip plaće:", " != ", "", Ptrans.recordName)); //12.2018.
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotN2", Placa.TT_NR2_P01NEDOP, "", "Za tip plaće:", " != ", "", Ptrans.recordName)); //12.2018.
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotN3", Placa.TT_NR3_PX1DADOP, "", "Za tip plaće:", " != ", "", Ptrans.recordName)); //12.2018.
      }
      // Special, new stuff. Force Skip unwanted TT not 'AH & UG & NO' 
      if(theVvPlaReport != null && theVvPlaReport.VvLet_TT_AHiUGiNOiK4_Only == true)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotRR", Placa.TT_REDOVANRAD  , "", "Za tip plaće:", " != ", "", Ptrans.recordName));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotOP", Placa.TT_OSTALIPRIM  , "", "Za tip plaće:", " != ", "", Ptrans.recordName));//2024
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotPP", Placa.TT_PODUZETPLACA, "", "Za tip plaće:", " != ", "", Ptrans.recordName));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotND", Placa.TT_NEPLACDOPUST, "", "Za tip plaće:", " != ", "", Ptrans.recordName));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotPN", Placa.TT_PLACAUNARAVI, "", "Za tip plaće:", " != ", "", Ptrans.recordName));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNotBR", Placa.TT_BIVSIRADNIK , "", "Za tip plaće:", " != ", "", Ptrans.recordName));
      }

      // Fld_KupdobCD                                                                                                                                     

      drSchema = ZXC.PersonSchemaRows[ZXC.PerCI.banka_cd];
      num      = theRptFilter.BankaCd;

      if(num.NotZero())
      {
         PlacaReportUC placaReportUC = (PlacaReportUC)TheVvUC;

         string kcd_string = placaReportUC.tbx_banka_cd.Text + " " + placaReportUC.tbx_banka_tk.Text + " " + placaReportUC.tbx_banka_Naziv.Text;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "BankaOdPersona", num, kcd_string, "Za Banku:", " = ", Person.recordName));
      }

      // Fld_MtrosCD                                                                                                                                     

      drSchema = ZXC.PersonSchemaRows[ZXC.PerCI.mtros_cd];
      num      = theRptFilter.MtrosCd;

      if(num.NotZero())
      {
         PlacaReportUC placaReportUC = (PlacaReportUC)TheVvUC;

         //string kcd_string = placaReportUC.tbx_mtros_cd.Text + " " + placaReportUC.tbx_mtros_tk.Text + " " + placaReportUC.tbx_mtros_Naziv.Text;
         // 11.09.2014.
         string kcd_string = /*placaReportUC.tbx_mtros_cd.Text + " " + */placaReportUC.tbx_mtros_tk.Text + " " + placaReportUC.tbx_mtros_Naziv.Text;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "MtrosOdPersona", num, kcd_string, "Za MjestoTroška:", " = ", Person.recordName));
      }

      // Fld_ShowHashCodeRsm                                                                                                                                          

      isCheck = theRptFilter.ShowHashCodeRsm;

      if(isCheck)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember("HashCode", true));
      }
      else
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember("HashCode", false));
      }

      // Fld_DateIzvj                                                                                                                                  

        theRptFilter.FilterMembers.Add(new VvSqlFilterMember("DateIzvj", theRptFilter.DateIzvj.ToString("dd.MM.yyyy.")));


     // VrstaIsplate                                                                                                                         

      drSchema = ZXC.PersonSchemaRows[ZXC.PerCI.vrstaIsplate];
      vrIspl   = theRptFilter.VrstaIsplate;

      if(vrIspl == Person.VrstaIsplateEnum.TEKUCI)
      {
         text = "Tekući";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "VrstaIsplate", vrIspl, text, "Vrsta isplate:", " = ", Person.recordName));
      }
      else if(vrIspl == Person.VrstaIsplateEnum.BANKA)
      {
         text = "Banka";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "VrstaIsplate", vrIspl, text, "Vrsta isplate:", " = ", Person.recordName));
      }
      else if(vrIspl == Person.VrstaIsplateEnum.GOTOVINA)
      {
         text = "Gotovina";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "VrstaIsplate", vrIspl, text, "Vrsta isplate:", " = ", Person.recordName));
      }

      // VrstaRadOdnosa                                                                                                                         

      drSchema = ZXC.PersonSchemaRows[ZXC.PerCI.vrstaRadOdns];
      vrRadOd   = theRptFilter.VrstaRadOdnosa;

      if(vrRadOd == Person.VrstaRadnogOdnosaEnum.NEODREDJENO)
      {
         text = "Neodređeno";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "VrstaRadOdnosa", vrRadOd, text, "Vrsta radnog odnosa:", " = ", Person.recordName));
      }
      else if(vrRadOd == Person.VrstaRadnogOdnosaEnum.ODREDJENO)
      {
         text = "Određeno";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "VrstaRadOdnosa", vrRadOd, text, "Vrsta radnog odnosa:", " = ", Person.recordName));
      }
      else if(vrRadOd == Person.VrstaRadnogOdnosaEnum.PRIPR_VJEZB)
      {
         text = "Pripravnik/Vježbenik";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "VrstaRadOdnosa", vrRadOd, text, "Vrsta radnog odnosa:", " = ", Person.recordName));
      }


      // VrstaRadVremena                                                                                                                         

      drSchema = ZXC.PersonSchemaRows[ZXC.PerCI.vrstaRadVrem];
      vrRadVre = theRptFilter.VrstaRadVrem;

      if(vrRadVre == Person.VrstaRadnogVremenaEnum.PUNO)
      {
         text = "Puno";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "VrstaRadVrem", vrRadVre, text, "Vrsta radnog vremena:", " = ", Person.recordName));
      }
      else if(vrRadVre == Person.VrstaRadnogVremenaEnum.NEPUNO)
      {
         text = "Nepuno";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "VrstaRadVrem", vrRadVre, text, "Vrsta radnog vremena:", " = ", Person.recordName));
      }
      else if(vrRadVre == Person.VrstaRadnogVremenaEnum.SKRACENO)
      {
         text = "Skraćeno";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "VrstaRadVrem", vrRadVre, text, "Vrsta radnog vremena:", " = ", Person.recordName));
      }
      
      // Fld_Spol                                                                                                                                         

      drSchema       = ZXC.PersonSchemaRows[ZXC.PerCI.spol];
      ZXC.Spol? spol = theRptFilter.Spol;

      if(spol == ZXC.Spol.MUSKO)
      {
         text = "Muški";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Spol", spol, text, "Spol:", " = ", Person.recordName));
      }
      else if(spol == ZXC.Spol.ZENSKO)
      {
         text = "Ženski";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Spol", spol, text, "Spol:", " = ", Person.recordName));
      }
      
      // Fld_Grupa                                                                                                                                      

      drSchema = ZXC.PersonSchemaRows[ZXC.PerCI.ts];
      text     = theRptFilter.TipPerson ;

      if(text.NotEmpty())
      {
         text = ZXC.GetOrovaniCharPattern(theRptFilter.TipPerson);
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Ts", text, text, "Za tip djelatnika:", " REGEXP ", Person.recordName));
      }

      // Fld_StrSpr                                                                                                                                     

      drSchema = ZXC.PersonSchemaRows[ZXC.PerCI.strSprCd];
      text     = theRptFilter.StrSpr;
      text2    = text + " " + ((PlacaReportUC)this.Parent.Parent).Fld_StrSpr;

      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "StrSpr", text, text2, "Za stručnu spremu:", " = ", Person.recordName));
      }

      // NO!Fld_IsZbirObust                                                                                                                                         
      // !!! za ovaj FilterMember nema Fld_-a na ReportUC-u, vec se korisgti za eksplicitno zadavanje iz code-a 
      //drSchema                    = ZXC.PtranoSchemaRows[ZXC.PtoCI.t_isZbir];
      //ZXC.JeliJeTakav jeLiJeTakav = theRptFilter.IsZbirObust;

      //if(jeLiJeTakav == ZXC.JeliJeTakav.JE_TAKAV)
      //{
      //   text = "SamoZbirneObustave";
      //   theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "IsZbir", true, text, "", " = ", Ptrano.recordName, Ptrano.recordName));
      //}
      //else if(jeLiJeTakav == ZXC.JeliJeTakav.NIJE_TAKAV)
      //{
      //   text = "SamoPojedinacneObustave";
      //   theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "IsZbir", false, text, "", " = ", Ptrano.recordName, Ptrano.recordName));
      //}

       //Virmanski pomak                                                                                                                                     
      int numv = theRptFilter.VirPomak;

      if(numv.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember("VirPomak", numv));
      }


   }

   #endregion AddFilterMemberz()

}

public class PlacaSifrartFilterUC : VvFilterUC
{
   #region Fieldz

   private VvHamper    hamp_dateIzvj, hamp_OdDo, hamp_tt, hamp_sortDok, hamp_card;
   private VvTextBox  tbx_personCdOd, tbx_personCdDo, tbx_prezimeOd, tbx_prezimeDo, tbx_dokNumOD, tbx_dokNumDO,
                      tbx_TTopis, tbxLookUp_TT, tbx_vrObrOpis, tbxLookUp_vrObr;
   private RadioButton rbt_SortBroj, rbt_SortDatum, rbt_radnik, rbt_pTrans, rbt_eTrans, rbt_oTrans;

   #endregion Fieldz

   #region Constructor

   public PlacaSifrartFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();

      TheVvUC = vvUC;
      
      InitializeHamper_DateIzvj(out hamp_dateIzvj);
      nextY = hamp_dateIzvj.Bottom + razmakIzmjedjuHampera;

      InitializeHamper_OdDo(out hamp_OdDo);

      MaxHamperWidth = hamp_OdDo.Width;
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamp_dateIzvj, MaxHamperWidth, razmakIzmjedjuHampera);

      CreateHamper_4ButtonsResetGo_Width(MaxHamperWidth);

      nextY = hamp_OdDo.Bottom + razmakIzmjedjuHampera;

      InitializeHamper_tt(out hamp_tt);

      nextY = hamp_tt.Bottom + razmakIzmjedjuHampera;

      InitializeHamper_SortDok(out hamp_sortDok);

      nextY = hamp_sortDok.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_Card(out hamp_card);
      nextY = hamp_card.Bottom + razmakIzmjedjuHampera;

      nextY = LocationOfHamper_HorLine(nextX, nextY, MaxHamperWidth) + razmakIzmjedjuHampera;

      this.Size = new Size(MaxHamperWidth + 2 * razmakIzmjedjuHampera, nextY + ZXC.QunMrgn);

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();

      // TODO: !!! votafak, SHVATI ZASTO implicitno loudanje ovdje ne radi??
      // ako ovo ne pozoves onda fali CalcTrans-u. Zasto se LuiListe init samo pri VvTextBox...
      ZXC.luiListaVrstaRadaEVR.LazyLoad();
   }



   #endregion Constructor

   #region Hamper_s
   
   private void InitializeHamper_DateIzvj(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q7un + ZXC.Qun2, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] {            ZXC.Qun8, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbdate;

      lbdate = hamper.CreateVvLabel(0, 0, "Datum formiranja izvještaja:", ContentAlignment.MiddleRight);

      tbx_dateIzvj      = hamper.CreateVvTextBox(1, 0, "tbx_dateIzvj", "");
      tbx_dateIzvj.JAM_IsForDateTimePicker = true;
      dtp_dateIzvj      = hamper.CreateVvDateTimePicker(1, 0, "", tbx_dateIzvj);
      dtp_dateIzvj.Name = "dtp_dateIzvj";
      dtp_dateIzvj.Tag  = tbx_dateIzvj;
      tbx_dateIzvj.Tag  = dtp_dateIzvj;

      tbx_dateIzvj.JAM_WriteOnly = true;

      VvHamper.HamperStyling(hamper);

   }

   private void InitializeHamper_OdDo(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 5, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q2un + ZXC.Qun2, ZXC.Q5un, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] {            ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbOd, lbDo, lbDatum, lbSifra, lbNaziv, lbTrans;

      lbOd = hamper.CreateVvLabel(1, 0, "OD:", ContentAlignment.MiddleCenter);
      lbDo = hamper.CreateVvLabel(2, 0, "DO:", ContentAlignment.MiddleCenter);

      lbDatum          = hamper.CreateVvLabel(0, 1, "Datum:", ContentAlignment.MiddleRight);
      tbx_DatumOD      = hamper.CreateVvTextBox(1, 1, "tbx_datumOd", "");
      tbx_DatumOD.JAM_IsForDateTimePicker = true;
      dtp_DatumOD      = hamper.CreateVvDateTimePicker(1, 1, "", tbx_DatumOD);
      dtp_DatumOD.Name = "dtp_datumOd";
      dtp_DatumOD.Tag  = tbx_DatumOD;
      tbx_DatumOD.Tag  = dtp_DatumOD;

      tbx_DatumDO      = hamper.CreateVvTextBox(2, 1, "tbx_datumDo", "");
      tbx_DatumDO.JAM_IsForDateTimePicker = true;
      dtp_DatumDO      = hamper.CreateVvDateTimePicker(2, 1, "", tbx_DatumDO);
      dtp_DatumDO.Name = "dtp_datumDo";
      dtp_DatumDO.Tag  = tbx_DatumDO;
      tbx_DatumDO.Tag  = dtp_DatumDO;

      tbx_DatumOD.JAM_WriteOnly = tbx_DatumDO.JAM_WriteOnly = true;

      lbSifra = hamper.CreateVvLabel(0, 2, "Sifra:", ContentAlignment.MiddleRight);
      lbNaziv = hamper.CreateVvLabel(0, 3, "Prezime:", ContentAlignment.MiddleRight);

      tbx_personCdOd = hamper.CreateVvTextBox(1, 2, "tbx_personCdOd", "", 6);
      tbx_personCdDo = hamper.CreateVvTextBox(2, 2, "tbx_personCdDo", "", 6);

      tbx_prezimeOd = hamper.CreateVvTextBox(1, 3, "tbx_prezimeOd", "", 30);
      tbx_prezimeDo = hamper.CreateVvTextBox(2, 3, "tbx_prezimeDo", "", 30);

      tbx_personCdOd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_personCdOd.JAM_FillCharacter = '0';
      tbx_personCdOd.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);
      tbx_personCdDo.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_personCdDo.JAM_FillCharacter = '0';
      tbx_personCdDo.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      tbx_personCdOd.JAM_ReadOnly = tbx_personCdDo.JAM_ReadOnly = true;
      tbx_prezimeOd.JAM_ReadOnly = tbx_prezimeDo.JAM_ReadOnly = true;

      tbx_personCdOd.TextAlign = tbx_personCdDo.TextAlign = HorizontalAlignment.Left;

      tbx_DatumOD.ContextMenu = dtp_DatumOD.ContextMenu =
                                tbx_DatumDO.ContextMenu =
                                dtp_DatumDO.ContextMenu = CreateNewContexMenu_Date();

      lbTrans = hamper.CreateVvLabel(0, 4, "DokBr:", ContentAlignment.MiddleRight);

      tbx_dokNumOD = hamper.CreateVvTextBox(1, 4, "tbx_BrTransOD", "", 6);
      tbx_dokNumOD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_dokNumOD.JAM_FillCharacter = '0';
      tbx_dokNumOD.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      tbx_dokNumDO = hamper.CreateVvTextBox(2, 4, "tbx_BrTransDO", "", 6);
      tbx_dokNumDO.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_dokNumDO.JAM_FillCharacter = '0';
      tbx_dokNumDO.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_tt(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 2, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q2un + ZXC.Qun2, ZXC.Q2un + ZXC.Qun2, ZXC.Q7un + ZXC.Qun2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbVrKnjz;

      lbVrKnjz     = hamper.CreateVvLabel        (0, 0, "TT:", ContentAlignment.MiddleRight);
      tbx_TTopis   = hamper.CreateVvTextBox      (2, 0, "tbx_TtOpis", "");
      tbxLookUp_TT = hamper.CreateVvTextBoxLookUp(1, 0, "tbxLookUp_TT", "");

      tbxLookUp_TT.JAM_CharacterCasing = CharacterCasing.Upper;
      tbxLookUp_TT.JAM_WriteOnly = true;
      tbxLookUp_TT.JAM_Set_LookUpTable(ZXC.luiListaPlacaTT, (int)ZXC.Kolona.prva);
      tbxLookUp_TT.JAM_lui_NameTaker_JAM_Name = tbx_TTopis.JAM_Name;

      tbx_TTopis.JAM_ReadOnly = true;
      tbx_TTopis.Tag = ZXC.vvColors.userControl_BackColor;

      Label  lbVrObr = hamper.CreateVvLabel         (0, 1, "VrObr:", ContentAlignment.MiddleRight);
      tbx_vrObrOpis   = hamper.CreateVvTextBox      (2, 1, "tbx_vrObrOpis", "");
      tbxLookUp_vrObr = hamper.CreateVvTextBoxLookUp(1, 1, "tbxLookUp_vrObr", "");

      tbxLookUp_vrObr.JAM_WriteOnly = true;
      tbxLookUp_vrObr.JAM_Set_LookUpTable(ZXC.luiListaPlacaVrstaObr, (int)ZXC.Kolona.prva);
      tbxLookUp_vrObr.JAM_lui_NameTaker_JAM_Name = tbx_vrObrOpis.JAM_Name;

      tbx_vrObrOpis.JAM_ReadOnly = true;
      tbx_vrObrOpis.Tag = ZXC.vvColors.userControl_BackColor;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_SortDok(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 3, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q6un +ZXC.Qun2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "Slijed ispisa dokumenta:", ContentAlignment.MiddleLeft);

      rbt_SortDatum = hamper.CreateVvRadioButton(0, 1, null, "Po datumu", TextImageRelation.ImageBeforeText);
      rbt_SortDatum.Checked = true;
      rbt_SortDatum.Tag     = true;
      rbt_SortBroj = hamper.CreateVvRadioButton(0, 2, null, "Po broju", TextImageRelation.ImageBeforeText);

      VvHamper.HamperStyling(hamper);

   }

   private void InitializeHamper_Card(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 5, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q6un + ZXC.Qun4 + ZXC.Qun5 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 + ZXC.Qun5 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel(0, 0, "Ispis Dokumenta:", ContentAlignment.MiddleLeft);

      rbt_radnik = hamper.CreateVvRadioButton(0, 1, null, "Radnik", TextImageRelation.ImageBeforeText);
      rbt_pTrans = hamper.CreateVvRadioButton(0, 2, null, "Ptrans", TextImageRelation.ImageBeforeText);
      rbt_eTrans = hamper.CreateVvRadioButton(0, 3, null, "Etrans", TextImageRelation.ImageBeforeText);
      rbt_oTrans = hamper.CreateVvRadioButton(0, 4, null, "Otrans", TextImageRelation.ImageBeforeText);
      rbt_radnik.Checked = true;
      rbt_radnik.Tag     = true;

      VvHamper.HamperStyling(hamper);
   }

   #endregion Hamper_s

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
         tbx_DatumOD.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumOD);
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
  
   public DateTime Fld_DateIzvj
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_dateIzvj.Value);
      }
      set
      {
         dtp_dateIzvj.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_dateIzvj.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_dateIzvj);
      }
   }

   public uint Fld_PersonCdOd
   {
      get { return ZXC.ValOrZero_UInt(tbx_personCdOd.Text); }
      set { tbx_personCdOd.Text = value.ToString("0000"); }
   }

   public uint Fld_PersonCdDo
   {
      get { return ZXC.ValOrZero_UInt(tbx_personCdDo.Text); }
      set { tbx_personCdDo.Text = value.ToString("0000"); }
   }

   public string Fld_PrezimeOd
   {
      get { return tbx_prezimeOd.Text; }
      set { tbx_prezimeOd.Text = value; }
   }

   public string Fld_PrezimeDo
   {
      get { return tbx_prezimeDo.Text; }
      set { tbx_prezimeDo.Text = value; }
   }

   public uint Fld_DokNumOd
   {
      get { return ZXC.ValOrZero_UInt(tbx_dokNumOD.Text); }
      set { tbx_dokNumOD.Text = value.ToString("000000"); }
   }

   public uint Fld_DokNumDo
   {
      get { return ZXC.ValOrZero_UInt(tbx_dokNumDO.Text); }
      set { tbx_dokNumDO.Text = value.ToString("000000"); }
   }

   public string Fld_TT
   {
      get { return tbxLookUp_TT.Text; }
      set { tbxLookUp_TT.Text = value; }
   }

   public bool Fld_NeedsHorizontalLine
   {
      get { return cb_Line.Checked; }
      set { cb_Line.Checked = value; }
   }

   public VvSQL.SorterType Fld_DokumentSort
   {
      get
      {
         if     (rbt_SortDatum.Checked) return VvSQL.SorterType.DokDate;
         else if(rbt_SortBroj .Checked) return VvSQL.SorterType.DokNum;
         else                           return VvSQL.SorterType.None;
      }
      set
      {
         switch(value)
         {
            case VvSQL.SorterType.DokDate: rbt_SortDatum.Checked = true; break;
            case VvSQL.SorterType.DokNum : rbt_SortBroj .Checked = true; break;
         }
      }
   }


   public string Fld_VrObr
   {
      get { return tbxLookUp_vrObr.Text; }
      set {        tbxLookUp_vrObr.Text = value; }
   }

   public PersonCardFilter.PersonCardsEnum Fld_PersonCard
   {
      get
      {
         if     (rbt_radnik.Checked) return PersonCardFilter.PersonCardsEnum.Matični;
         else if(rbt_pTrans.Checked) return PersonCardFilter.PersonCardsEnum.PTrans;
         else if(rbt_eTrans.Checked) return PersonCardFilter.PersonCardsEnum.ETrans;
         else if(rbt_oTrans.Checked) return PersonCardFilter.PersonCardsEnum.OTrans;

         else throw new Exception("Fld_PrintSomeDebitDoc: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case PersonCardFilter.PersonCardsEnum.Matični: rbt_radnik.Checked = true; break;
            case PersonCardFilter.PersonCardsEnum.PTrans: rbt_pTrans.Checked = true; break;
            case PersonCardFilter.PersonCardsEnum.ETrans: rbt_eTrans.Checked = true; break;
            case PersonCardFilter.PersonCardsEnum.OTrans: rbt_oTrans.Checked = true; break;
         }
      }
   }


   #endregion Fld_

   #region PutFilterFields(), GetFilterFields()

   private PersonCardFilter ThePlacaFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as PersonCardFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      ThePlacaFilter = (PersonCardFilter)_filter_data;

      if(ThePlacaFilter != null)
      {
         Fld_PersonCdOd = ThePlacaFilter.PersonCDod;
         Fld_PersonCdDo = ThePlacaFilter.PersonCDdo;
         Fld_PrezimeOd  = ThePlacaFilter.PrezimeOd;
         Fld_PrezimeDo  = ThePlacaFilter.PrezimeDo;
         Fld_DokNumOd   = ThePlacaFilter.DokNumOd;
         Fld_DokNumDo   = ThePlacaFilter.DokNumDo;
         Fld_TT         = ThePlacaFilter.TT;
         Fld_DatumOd    = ThePlacaFilter.DatumOd;
         Fld_DatumDo    = ThePlacaFilter.DatumDo;
         Fld_DateIzvj   = ThePlacaFilter.DateIzvj;
         Fld_VrObr      = ThePlacaFilter.VrObr;

         Fld_DokumentSort = ThePlacaFilter.SorterType_Dokument;

         Fld_NeedsHorizontalLine = ThePlacaFilter.NeedsHorizontalLine;
         Fld_PersonCard          = ThePlacaFilter.PersonCards;
      }
      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
      ThePlacaFilter.PersonCDod = Fld_PersonCdOd;
      ThePlacaFilter.PersonCDdo = Fld_PersonCdDo;
      ThePlacaFilter.PrezimeOd  = Fld_PrezimeOd;
      ThePlacaFilter.PrezimeDo  = Fld_PrezimeDo;
      ThePlacaFilter.DokNumOd   = Fld_DokNumOd;
      ThePlacaFilter.DokNumDo   = Fld_DokNumDo;
      ThePlacaFilter.TT         = Fld_TT;
      ThePlacaFilter.DatumOd    = Fld_DatumOd;
      ThePlacaFilter.DatumDo    = Fld_DatumDo;
      ThePlacaFilter.DateIzvj   = Fld_DateIzvj;
      ThePlacaFilter.VrObr      = Fld_VrObr ;
      
      ThePlacaFilter.SorterType_Dokument = Fld_DokumentSort;

      ThePlacaFilter.NeedsHorizontalLine = Fld_NeedsHorizontalLine;
      ThePlacaFilter.PersonCards         = Fld_PersonCard ;

   }

   #endregion PutFilterFields(), GetFilterFields()

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
      PersonCardFilter   theRptFilter   = (PersonCardFilter)  _vvRptFilter;
    //VvRpt_Placa_Filter theRptFilter   = (VvRpt_Placa_Filter)_vvRptFilter;
      VvPlacaReport      theVvPlaReport = (VvPlacaReport)     _vvReport;
      
      DateTime dateOD, dateDO;
      string   comparer;
      string   text, textOD, textDO;
      bool     isDummy;
      uint     numOD, numDO;
      DataRow  drSchema;

      bool   isPtranEO = (theRptFilter.PersonCards == PersonCardFilter.PersonCardsEnum.ETrans || theRptFilter.PersonCards == PersonCardFilter.PersonCardsEnum.OTrans);
      string forcedPreffix;
      
      if(theRptFilter.IsPopulatingTransDGV == true) forcedPreffix = "";
      else                                          forcedPreffix = (isPtranEO ? Ptrans.recordName : "");


      theRptFilter.FilterMembers.Clear();
      theRptFilter.ClearAllFilters_FromClauseGotTableName();

      isDummy = false;
      // Fld_PersonCDOdDo                                                                                                                                     

      drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_personCD];
      numOD = theRptFilter.PersonCDod;
      numDO = theRptFilter.PersonCDdo;

      if(numOD != 0)
      {
         if(numOD == numDO) comparer = " = ";
         else comparer = " >= ";

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "PersonCDOD", numOD, numOD.ToString("0000"), "Od ŠifDjelat:", comparer, "", forcedPreffix));
      }
      else if(numDO != 0)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "PersonCDOD", numOD, "-", "Od ŠifDjelat:", " >= ", "", forcedPreffix));
      }

      if(numDO != 0)
      {
         if(numDO == numOD) isDummy = true;
         else isDummy = false;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "PersonCDDO", numDO, numDO.ToString("0000"), "Do ŠifDjelat:", " <= ", "", forcedPreffix));
      }
      else if(numOD != 0)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "PersonCDDO", numDO, "-", "Do ŠifDjelat:", " <= ", "", forcedPreffix));
      }

      isDummy = false;

      // Fld_PrezimeOdDo                                                                                                                                     

      drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_prezime];

      textOD = theRptFilter.PrezimeOd;
      textDO = theRptFilter.PrezimeDo;

      if(textOD.NotEmpty())
      {
         if(textOD == textDO) comparer = " = ";
         else comparer = " >= ";

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "PrezimeOD", textOD, textOD, "Od prezimena:", comparer, "", forcedPreffix));
      }
      else if(textDO.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "PrezimeOD",  textOD, "-",    "Od prezimena:", " >= ",   "", forcedPreffix));
      }

      if(textDO.NotEmpty())
      {
         if(textDO == textOD) isDummy = true;
         else isDummy = false;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "PrezimeDO", textDO, textDO, "Do prezimena:", " <= ", "", forcedPreffix));
      }
      else if(textOD.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true,    "PrezimeDO", textDO, "-",     "Do prezimena:", " <= ", "", forcedPreffix));
      }

      isDummy = false;
      // !!! PO PRVI PUT ISPROBAVAMO OVAKO. Ovo je FilterMember dodan ne zbogradi FUlterUC.GetFields vec doticni VvReport fiksno zahtjeva neki FilterMember 
      drSchema = ZXC.PlacaSchemaRows[ZXC.PlaCI.vrstaObr];
      text     = theRptFilter.VrObr;

      // classic first 
      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "VrstaObracuna", text, text, "Vrsta Obracuna:", " = ", Placa.recordName));
      }

      // Special, new stuff. Force Skip unwanted VrstaObracuna '00'
      if(theVvPlaReport != null && theVvPlaReport.VvSkip_VrstaObracuna_00 == true)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "VrstaObrNot00", Placa.VrObr_NeisplacenaPlaca, "", "Vrsta Obracuna:", " != ", Placa.recordName));
      }

      // Fld_DatumOdDo                                                                                                                                  

         drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_dokDate];

         dateOD = theRptFilter.DatumOd;
         dateDO = theRptFilter.DatumDo;

         if(dateOD == dateDO) comparer = " = ";
         else                 comparer = " >= ";

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DateOD", dateOD, dateOD.ToString("dd.MM.yyyy."), "Od datuma:", comparer, "", forcedPreffix));

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DateDO", dateDO, dateDO.ToString("dd.MM.yyyy."), "Do datuma:", " <= ",   "", forcedPreffix));

      // Fld_DokNumOdDo                                                                                                                                   

         drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_dokNum];

         numOD = theRptFilter.DokNumOd;
         numDO = theRptFilter.DokNumDo;

         if(numOD.NotZero())
         {
            if(numOD == numDO) comparer = " = ";
            else               comparer = " >= ";

            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "BrDokOD", numOD, numOD.ToString("000000"), "Od dokumenta broj:", comparer, "", forcedPreffix));
         }
         else if(numDO.NotZero())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true,  "BrDokOD", numOD, "-",                      "Od dokumenta broj:", " >= ",   "", forcedPreffix));
         }

         if(numDO.NotZero())
         {
            if(numDO == numOD) isDummy = true;
            else               isDummy = false;

            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "BrDokDO", numDO, numDO.ToString("000000"), "Do dokumenta broj:", " <= ", "", forcedPreffix));
         }
         else if(numOD.NotZero())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true,    "BrDokDO", numDO, "-",                      "Do dokumenta broj:", " <= ", "", forcedPreffix));
         }

         isDummy = false;

      // Fld_TT classic first                                                                                                                                    

      drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_tt];
      text     = theRptFilter.TT;

      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TipTran", text, text, "Za tip plaće:", " = ", "", forcedPreffix));
      }

      // Fld_DateIzvj                                                                                                                                  

        theRptFilter.FilterMembers.Add(new VvSqlFilterMember("DateIzvj", theRptFilter.DateIzvj.ToString("dd.MM.yyyy.")));
   }

   #endregion AddFilterMemberz()


}

public class PlacaDokumentFilterUC : VvFilterUC
{

   #region  Constructor

   public PlacaDokumentFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;

      CreateHamper_4ButtonsResetGo_Width(0); // 0 zato sto je ovdje najsiri hamper bas taj s buttonsima
      MaxHamperWidth = hamper4buttons.Width;

      nextY = hamper4buttons.Bottom + razmakIzmjedjuHampera;
      nextY = LocationOfHamper_HorLine(nextX, nextY, MaxHamperWidth) + razmakIzmjedjuHampera;

      this.Size = new Size(MaxHamperWidth + 2 * razmakIzmjedjuHampera, nextY + ZXC.QunMrgn);

      this.ResumeLayout();

   }

   #endregion  Constructor

   #region Fld_

   public bool Fld_NeedsHorizontalLine
   {
      get { return cb_Line.Checked; }
      set { cb_Line.Checked = value; }
   }

   #endregion Fld_

   #region Put & GetFilterFields

   private PlacaDokumentFilter ThePlacaDokumentFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as  PlacaDokumentFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      ThePlacaDokumentFilter = (PlacaDokumentFilter)_filter_data;

      if(ThePlacaDokumentFilter != null)
      {
         Fld_NeedsHorizontalLine = ThePlacaDokumentFilter.NeedsHorizontalLine;
      }

      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
      ThePlacaDokumentFilter.NeedsHorizontalLine = Fld_NeedsHorizontalLine;
   }

   #endregion Put & GetFilterFields

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
      VvRpt_Placa_Filter theRptFilter   = (VvRpt_Placa_Filter)_vvRptFilter;
      VvPlacaReport      theVvOsrReport = (VvPlacaReport)     _vvReport;

      uint dokNum;
      DataRow drSchema;

      theRptFilter.FilterMembers.Clear();
      theRptFilter.ClearAllFilters_FromClauseGotTableName();

      // Fld_BrNalogaOdDo                                                                                                                                   

      drSchema = ZXC.PtransSchemaRows[ZXC.PtrCI.t_dokNum];

      dokNum = theRptFilter.DokNumOd;

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TheDokNum", dokNum, dokNum.ToString("000000"), "Plaća:", " = ", "", Ptrans.recordName));

   }

   #endregion AddFilterMemberz()

}

