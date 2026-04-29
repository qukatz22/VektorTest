using System;
using System.Drawing;
using System.Windows.Forms;
using Crownwood.DotNetMagic.Controls;
using System.Linq;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using                  MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
using System.Collections.Generic;
#endif

public class KupdobUC : VvSifrarRecordUC
{
   #region Fieldz

   public int dialogClientWidth, dialogClientHeight, nextX = 0, nextY = ZXC.Qun10, razmakHamp = ZXC.Qun10;
   public VvHamper  hampNaziv   , hampTip     , hampCentrala, hampAdress,
                    hampKontakt1, hampKontakt2, hampZiro    ,
                    hampTvrtT   , hampSifre   , hampIS    , hampRabati  , hampPlBanka,
                    hampNapomena, hampKomentar, hampRegob, hampPodObrt, hampKomisija, hampAgProviz, hampTime, hampPTG, hamp_Tetragram, hamp_isAMS;

   public VvTextBox  tbxNaziv     , tbxTick     , tbxKupdobCD    , 
                     tbxCentr     , tbxCentrID  , tbxCentrTick, tbxUlica1   , tbxUlica2 , tbxGrad , tbxPostaBr,
                     tbxMatbr     , tbxKontoD   , tbxKontoK   , tbxKontoT   , tbxKontoP , tbxIme  , tbxPrezime, tbxEmail, tbxURL,
                     tbxTel1      , tbxTel2     , tbxFax      , tbxGsm      ,
                     tbxZiro1     , tbxZiro1By  , tbxZiro1PnbM, tbxZiro1PnbV,
                     tbxZiro2     , tbxZiro2By  , tbxZiro2PnbM, tbxZiro2PnbV,
                     tbxZiro3     , tbxZiro3By  , tbxZiro3PnbM, tbxZiro3PnbV,
                     tbxZiro4     , tbxZiro4By  , tbxZiro4PnbM, tbxZiro4PnbV,
                     tbxDugoIme   , tbxGrupa    , tbxSifDcd   , tbxSifDname , tbxPutnikID, tbxPutName,
                     tbxOpcina    , tbxOpcCd    , tbxZupan    , tbxZupCd    , tbxRegob, tbxPnbMPlaca,
                     tbxPnbVPlaca , tbxStProviz , tbxPnbMProv , tbxPnbVProv , tbxNapom1,
                     tbxNapom2    , tbxKomentar , tbxStRbt1   , tbxStRbt2   , tbxStSRbt,
                     tbxStCsSc    , tbxValutaPl , tbxRokOtprm ,
                     tbxIsCentr   , tbxIsObrt   , tbxIsFrgn   , 
                     tbxIsMTros   , tbxIsBanka  , tbxIsKup    , tbxIsDob,
                     tbxOIB       , tbx_vatCntryCode,
                     tbxDrzava, tbxSwift, tbxIban, tbxDevName, tbxfinLimit, tbx_ugovorNo,
                     tbx_komisSkladCD, tbx_komisSkladNaziv, tbx_komisSkladKonto, tbx_komisSkladMalop, tbx_komisSkladBroj,
                     tbx_mitoIzn , tbx_mitoSt  , tbx_investTr, tbx_trecaStr,
                     tbx_timeOd_1, tbx_timeDo_1, 
                     tbx_timeOd_2, tbx_timeDo_2, 
                     tbx_timeOd_3, tbx_timeDo_3, 
                     tbx_timeOd_4, tbx_timeDo_4, 
                     tbx_timeOd_5, tbx_timeDo_5, 
                     tbx_timeOd_6, tbx_timeDo_6, 
                     tbx_timeOd_7, tbx_timeDo_7,
                     tbx_PTG_DanZaFak, tbx_PTG_DanZaFakOpis, tbx_PTG_SlanjeRacuna, tbx_PTG_ZaduzenCd, tbx_PTG_ZaduzenName,
                     tbx_drzavljanstvo, tbx_brojIdI, tbx_dateVazenjaIdI, tbx_izdavIdI, tbx_dateBirth
                     ;

   private VvDateTimePicker dtp_timeOd_1, dtp_timeDo_1,
                            dtp_timeOd_2, dtp_timeDo_2,
                            dtp_timeOd_3, dtp_timeDo_3,
                            dtp_timeOd_4, dtp_timeDo_4,
                            dtp_timeOd_5, dtp_timeDo_5,
                            dtp_timeOd_6, dtp_timeDo_6,
                            dtp_timeOd_7, dtp_timeDo_7,
                            dtp_dateVazenjaIdI, dtp_dateBirth;
   
   public CheckBox cbxIsCentr, cbxIsObrt, cbxIsFrgn, cbxIsPdv,
                   cbxIsMTros, cbxIsBanka, cbxIsKup, cbxIsDob, cbxIsOsoba, cbxPTG_isPojedinacnoFak, cbx_isAMS, cbx_hasPolIzjava;
   public RadioButton rbt_poduzece, rbt_obrtPdv1, rbt_obrtPdv2, rbt_notPdv, rbt_obrtNotPdv, rbt_podPoNapl, rbt_fizickaOsoba,
                      rbt_komisNE, rbt_komisVP, rbt_komisMP,
                      rbt_R1nepoznato, rbt_B2B, rbt_B2C;

   private bool ovoJeKupdob_A_NE_Prjkt;

   public Kupdob  kupdob_rec;

   private KupdobDao.KupdobCI DB_ci
   {
      get { return ZXC.KpdbCI; }
   }
   private Button btn_nextTK;

   protected bool KDC_Tab_Loaded;
   protected bool KDC_Osnovno_Loaded;

   private VvDataGridViewFind kdcGrid;
   protected DataGridViewTextBoxColumn colScrol;

   #endregion Fieldz

   #region Constructor

   public KupdobUC(Control parent, Kupdob _kupdob, VvSubModul vvSubModul)
   {
      if(_kupdob == null) // znaci dodjosmo iz PrjktUC constructora 
         ovoJeKupdob_A_NE_Prjkt = false;
      else
         ovoJeKupdob_A_NE_Prjkt = true;

      kupdob_rec       = _kupdob;

      // samo za KupdobUC ide if provjera
      if(VirtualDataRecord != null)
         ThePrefferedRecordSorter = VirtualDataRecord.DefaultSorter;
      
      this.TheSubModul = vvSubModul;

      SuspendLayout();

      this.TheTabControl.TabPages.Add(CreateVvInnerTabPages("Osnovno", "", ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
      this.TheTabControl.TabPages.Add(CreateVvInnerTabPages("Prošireno", "", ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));

      PoredakHamperaNaTabPage_Osnovno();
      PoredakHamperaNaTabPage_Prosireno();
      //this.ControlForInitialFocus = tbxNaziv;

      InitializeVvUserControl(parent);

      if(ovoJeKupdob_A_NE_Prjkt && ZXC.ThisIsVektorProject)
      {
         VvTabPage vvTabPage = (VvTabPage)(parent.Parent);

         if(vvTabPage.TabPageKind != ZXC.VvTabPageKindEnum.RECORD_TabPage_INTERACTIVE)// ovaj if sluzi kad se kartica dodaje i Find-a da se ne pojave transevi !!!!
         {
            this.TheTabControl.TabPages.Add(CreateVvInnerTabPages(ftrans_TabPageName   , ""             , ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage));
            this.TheTabControl.TabPages.Add(CreateVvInnerTabPages(KDC_TabPageName      , KDC_TabPageName, ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage));

            CreateDataGrid_InitializeTheGrid_ReadOnly_Columns();
         }

      TheTabControl.SelectionChanged += new VvSelectTabHandler(TheTabControl_SelectionChanged_ShouldLoadKDC_Tab);
      TheTabControl.SelectionChanged += new VvSelectTabHandler(TheTabControl_SelectionChanged_ShouldLoadKDC_Osnovno);

      }

      this.Validating += new System.ComponentModel.CancelEventHandler(KupdobDUC_Validating);

      ResumeLayout();

      if(ovoJeKupdob_A_NE_Prjkt)
      {
         kdcGrid = CreateDataGridView_ReadOnly(TheTabControl.TabPages[0], "kdcGrid");
         kdcGrid.Location = new Point(hampIS.Right + ZXC.QUN, hampNaziv.Top + ZXC.Qun2);
         AddColumnsToKDCGrid();
         colScrol = kdcGrid.CreateScrollColumn("scrol", ZXC.QUN);
         colScrol.ReadOnly = true;
       //kdcGrid.Size = new Size(ZXC.Q10un * 4, TheTabControl.TabPages[0].Height - hampPTG.Height - ZXC.Q3un);
         kdcGrid.Size   = new Size(TheTabControl.TabPages[0].Width - hampKomentar.Right - ZXC.Q3un, TheTabControl.TabPages[0].Height - ZXC.Q3un);
         kdcGrid.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

         kdcGrid.TabStop = false;
         kdcGrid.ClearSelection();

         if(ZXC.KSD.Dsc_NePrikazujKDC) kdcGrid.Visible = false;
      }
   }

   private void TheTabControl_SelectionChanged_ShouldLoadKDC_Tab(VvInnerTabControl sender, VvInnerTabPage oldPage, VvInnerTabPage newPage)
   {
      if(TheVvTabPage != null && TheVvTabPage.IsArhivaTabPage) return;
      if(newPage.Name != KDC_TabPageName)                      return;

      // ako smo dosli do ovdje; upravo selektiramo KDC TabPage i zelimo ga, ak vec nije, napuniti kontaktima ovog partnera 

      GetKDClist_AND_PutKDC_Tab_OR_Osnovno_DGV_Fields(kupdob_rec, aTransesGrid[1], KDC_Tab_Loaded);

      ThePanelForFilterUC_PrintTemplateUC.Visible = false;
      aTransesGrid[1].TabStop = false;
      aTransesGrid[1].ClearSelection();

   }

   private void TheTabControl_SelectionChanged_ShouldLoadKDC_Osnovno(VvInnerTabControl sender, VvInnerTabPage oldPage, VvInnerTabPage newPage)
   {
      if(TheVvTabPage != null && TheVvTabPage.IsArhivaTabPage) return;
      if(newPage.Name != "Osnovno")                            return;

      GetKDClist_AND_PutKDC_Tab_OR_Osnovno_DGV_Fields(kupdob_rec, kdcGrid, KDC_Osnovno_Loaded);

      ThePanelForFilterUC_PrintTemplateUC.Visible = false;
      kdcGrid.TabStop = false;
      kdcGrid.ClearSelection();

   }

   private void GetKDClist_AND_PutKDC_Tab_OR_Osnovno_DGV_Fields(Kupdob kupdob_rec, DataGridView theGrid, bool isLoaded)
   {
      if(theGrid == null) return;

      #region Init & GetKDClist

      if(/*KDC_Tab_Loaded*/ isLoaded == true) return;

      List<Xtrans> KDCxtransList = new List<Xtrans>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      // veza Kupdob-a i KDC Xtransa je da je xtrans.t_ttNum = kupdob.kupdobCD
      filterMembers.Add(new VvSqlFilterMember(ZXC.XtransSchemaRows[ZXC.XtrCI.t_tt   ], "tt"   , Mixer.TT_KDC       , " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.XtransSchemaRows[ZXC.XtrCI.t_ttNum], "ttNum", kupdob_rec.KupdobCD, " = "));

      VvDaoBase.LoadGenericVvDataRecordList<Xtrans>(TheDbConnection, KDCxtransList, filterMembers, "t_kpdbNameA_50");

      if(theGrid.Name == kdcGrid        .Name) KDC_Osnovno_Loaded = true;
      if(theGrid.Name == aTransesGrid[1].Name) KDC_Tab_Loaded     = true;

      #endregion Init & GetKDClist

      #region PutKDC_DGV_Fields

      int rowIdx, idxCorrector;


      theGrid.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(theGrid);

      foreach(Xtrans xtrans_rec in KDCxtransList)
      {
         theGrid.Rows.Add();

         rowIdx = theGrid.RowCount - idxCorrector;

         theGrid[0, rowIdx].Value = xtrans_rec.T_parentID;
         theGrid[1, rowIdx].Value = xtrans_rec.T_kpdbNameA_50;
         theGrid[2, rowIdx].Value = xtrans_rec.T_kpdbUlBrA_32;
         theGrid[3, rowIdx].Value = xtrans_rec.T_vezniDokA_64;
         theGrid[4, rowIdx].Value = xtrans_rec.T_kpdbZiroA_32;
         theGrid[5, rowIdx].Value = xtrans_rec.T_isXxx == true ? "X" : "";
         theGrid[6, rowIdx].Value = xtrans_rec.T_opis_128;
         theGrid.Rows[rowIdx].HeaderCell.Value = (KDCxtransList.Count - rowIdx).ToString();
      }

      #endregion PutKDC_DGV_Fields
   }

   #endregion Constructor

   #region PoredakHamperaNaTabPage
   public void PoredakHamperaNaTabPage_Osnovno()
   {
      InitializeNazivHamper(out hampNaziv);
      nextX = hampNaziv.Right;

      InitializeIsMandatory_KupdobHamper(out hamp_isAMS);//
      nextY = hamp_isAMS.Bottom - ZXC.Qun8;//

      InitializeTipHamper(out hampTip);
      
    //nextX = hampTip.Right; //

      InitializeCentralaHamper(out hampCentrala);
      nextY = hampCentrala.Bottom;
      nextX = 0;
      InitializeAdressHamper(out hampAdress);
      nextX = hampNaziv.Right;
      InitializeKontakt1Hamper(out hampKontakt1);
      nextX = hampKontakt1.Right;
      InitializeKontakt2Hamper(out hampKontakt2);

      nextX = hampKontakt1.Right;
      nextY = hampKontakt2.Bottom;
      InitializePodObrtHamper(out hampPodObrt);

      nextY = hampAdress.Bottom;
      nextX = 0;
      InitializeZiroHamper(out hampZiro);
      nextX = hampKontakt1.Right;

      nextY = hampPodObrt.Bottom;
      InitializeRabatiHamper(out hampRabati);
      nextX = hampRabati.Right;
      InitializeISHamper(out hampIS);

      nextX = 0;
      nextY = hampIS.Bottom;

      InitializeNapomenaHamper(out hampNapomena);
      nextY = hampNapomena.Bottom;
      InitializeKomentarHamper(out hampKomentar);

      if(ZXC.IsPCTOGO)
      {
         //nextX = hampCentrala.Right;
         //nextY = hampCentrala.Top;
         nextX = 0;
         nextY = hampKomentar.Bottom;
         InitializePTGHamper(out hampPTG);
      }

      if(ZXC.IsTETRAGRAM_ANY)
      { 
         nextX = 0;
         nextY = hampKomentar.Bottom;
         InitializeTetragramHamper(out hamp_Tetragram);
      }
   }
   public void PoredakHamperaNaTabPage_Prosireno()
   {
      nextX = 0;
      nextY = 0;

      InitializeTvrtkaTipHamper(out hampTvrtT);
      nextY = hampTvrtT.Bottom;
      InitializeSifreHamper(out hampSifre);
      nextX = hampSifre.Right;
      InitializeBanka_PlacaHamper(out hampPlBanka);
      nextY = hampPlBanka.Bottom - ZXC.Qun4;
      InitializeRegobHamper(out hampRegob);

      nextX = 0;
      nextY = hampRegob.Bottom;
      InitializeKomisijaHamper(out hampKomisija);
      nextX = hampSifre.Right;
      InitializeAgProvHamper  (out hampAgProviz);

      nextX = hampTvrtT.Left;
      nextY = hampKomisija.Bottom;


      InitializeTimeHamper(out hampTime);

   }

   #endregion PoredakHamperaNaTabPage

   #region HAMPERI

   #region Naziv

   public void InitializeNazivHamper(out VvHamper hampNaziv)
   {
      hampNaziv = new VvHamper(5, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hampNaziv.VvColWdt      = new int[] { ZXC.Q4un - ZXC.Qun4, ZXC.Q3un + ZXC.Qun10, ZXC.QUN - ZXC.Qun4, ZXC.Q5un + ZXC.Qun4, ZXC.Q4un + ZXC.Qun10 };
      hampNaziv.VvSpcBefCol   = new int[] {            ZXC.Qun4,             ZXC.Qun4,                  0,            ZXC.Qun4,            ZXC.Qun4  };
      hampNaziv.VvRightMargin = hampNaziv.VvLeftMargin;

      hampNaziv.VvRowHgt       = new int[] { ZXC.QUN ,  ZXC.QUN };
      hampNaziv.VvSpcBefRow    = new int[] { ZXC.Qun2, ZXC.Qun4 };
      hampNaziv.VvBottomMargin = hampNaziv.VvTopMargin;

                    hampNaziv.CreateVvLabel  (0, 0, "Naziv:"  , ContentAlignment.MiddleRight);
      tbxNaziv    = hampNaziv.CreateVvTextBox(1, 0, "tbxNaziv", "Naziv poduzeća poslovnog partnera", GetDB_ColumnSize(DB_ci.naziv), 3, 0);
                    hampNaziv.CreateVvLabel  (0, 1, "Ticker:" , ContentAlignment.MiddleRight);
      tbxTick     = hampNaziv.CreateVvTextBox(1, 1, "tbxTick" , "Ticker poduzeća poslovnog partnera", GetDB_ColumnSize(DB_ci.ticker));
                    hampNaziv.CreateVvLabel  (3, 1, "Šifra:"  , ContentAlignment.MiddleRight);
      tbxKupdobCD = hampNaziv.CreateVvTextBox(4, 1, "tbxKupdobCD", "Sifra Partnera", 6 /*GetDB_ColumnSize(DB_ci.kupdobCDrecID)*/);
      tbxKupdobCD.JAM_StatusText    = "Šifra Partnera";
      tbxKupdobCD.JAM_CharEdits     = ZXC.JAM_CharEdits.DigitsOnly;
      tbxKupdobCD.JAM_FillCharacter = '0';

      tbxNaziv   .JAM_DataRequired =
      tbxTick    .JAM_DataRequired =
      tbxKupdobCD.JAM_DataRequired = true;

      // 25.03.2014: 
      tbxTick.JAM_FieldExitMethod = new EventHandler(CheckTICKERexists);

      //04.03.2014: 
      tbxKupdobCD.JAM_MarkAsNumericTextBox(0, false, 1M, 999999M, false);

      tbxNaziv   .Font =
      tbxTick    .Font = 
      tbxKupdobCD.Font = ZXC.vvFont.BaseBoldFont;

      tbxTick.JAM_CharacterCasing = CharacterCasing.Upper;

      btn_nextTK = hampNaziv.CreateVvButton(2, 1, new EventHandler(GetNextTickerWroot_btnClick), "");
      btn_nextTK.Name = "btn_goExLink_nextTK";
      btn_nextTK.FlatStyle = FlatStyle.Flat;
      btn_nextTK.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_nextTK.Image = VvIco.TriangleGreenL24/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_greenLeft.ico")), 24, 24)*/.ToBitmap();
      btn_nextTK.TabStop = false;
      btn_nextTK.Visible = false;


      if(ZXC.IsSkyEnvironment)
      {
         tbxKupdobCD.JAM_ReadOnly = true;
      }

      this.ControlForInitialFocus = tbxNaziv;
   }

   #endregion Naziv

   #region Grupa

   private void InitializeTipHamper(out VvHamper hampTip)
   {
      //hampTip = new VvHamper(3, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      //hampTip.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
      //hampTip.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      //hampTip.VvRightMargin = hampTip.VvLeftMargin;

      //hampTip.VvRowHgt       = new int[] { ZXC.QUN ,  ZXC.QUN };
      //hampTip.VvSpcBefRow    = new int[] { ZXC.Qun2, ZXC.Qun4 };
      //hampTip.VvBottomMargin = hampTip.VvTopMargin;

      //           hampTip.CreateVvLabel        (0, 0, "Grupa:", ContentAlignment.MiddleRight);
      //tbxGrupa = hampTip.CreateVvTextBoxLookUp("tbxGrupa", 1, 0, "Proizvoljna sifra, grupa partnera - mogućnost pripadnosti u više grupa", 10, 1, 0);
      //tbxGrupa.JAM_CharacterCasing = CharacterCasing.Upper;

      //tbxGrupa.JAM_Set_LookUpTable(ZXC.luiListaGrupaPartnera, (int)ZXC.Kolona.prva);
      //tbxGrupa.JAM_lookUp_NOTobligatory  = true;
      //tbxGrupa.JAM_lookUp_MultiSelection = true;

      //             hampTip.CreateVvLabel        (0, 1, "Deviza:", ContentAlignment.MiddleRight);
      //tbxDevName = hampTip.CreateVvTextBoxLookUp(1, 1, "tbxDevName", "Deviza", GetDB_ColumnSize(DB_ci.devName));
      //tbxDevName.JAM_CharacterCasing = CharacterCasing.Upper;
      //tbxDevName.JAM_Set_LookUpTable(ZXC.luiListaDeviza, (int)ZXC.Kolona.prva);

      hampTip = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hampTip.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un + ZXC.Qun2, ZXC.Q3un, ZXC.Q3un };
      hampTip.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun8, ZXC.Qun4, ZXC.Qun8 };
      hampTip.VvRightMargin = hampTip.VvLeftMargin;

      hampTip.VvRowHgt       = new int[] { ZXC.QUN  };
      hampTip.VvSpcBefRow    = new int[] { ZXC.Qun2 };
      hampTip.VvBottomMargin = hampTip.VvTopMargin;

                 hampTip.CreateVvLabel        (0, 0, "Grupa:", ContentAlignment.MiddleRight);
      tbxGrupa = hampTip.CreateVvTextBoxLookUp(1, 0, "tbxGrupa", "Proizvoljna sifra, grupa partnera - mogućnost pripadnosti u više grupa");
      tbxGrupa.JAM_CharacterCasing = CharacterCasing.Upper;

      tbxGrupa.JAM_Set_LookUpTable(ZXC.luiListaGrupaPartnera, (int)ZXC.Kolona.prva);
      tbxGrupa.JAM_lookUp_NOTobligatory  = true;
      tbxGrupa.JAM_lookUp_MultiSelection = true;

                   hampTip.CreateVvLabel        (2, 0, "Deviza:", ContentAlignment.MiddleRight);
      tbxDevName = hampTip.CreateVvTextBoxLookUp(3, 0, "tbxDevName", "Deviza", GetDB_ColumnSize(DB_ci.devName));
      tbxDevName.JAM_CharacterCasing = CharacterCasing.Upper;
      tbxDevName.JAM_Set_LookUpTable(ZXC.luiListaDeviza, (int)ZXC.Kolona.prva);

   }

   #endregion Grupa

   #region Centrala

   public void InitializeCentralaHamper(out VvHamper hampCentrala)
   {
      hampCentrala = new VvHamper(4, 2, "", TheTabControl.TabPages[0], false);
      
      hampCentrala.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un + ZXC.Qun10, ZXC.Q3un + ZXC.Qun5, ZXC.Q3un + ZXC.Qun10 };
      hampCentrala.VvSpcBefCol   = new int[] {           ZXC.Qun8,            ZXC.Qun4,             ZXC.Qun4,             ZXC.Qun4 };
      hampCentrala.VvRightMargin = hampCentrala.VvLeftMargin;

      hampCentrala.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN };
      hampCentrala.VvSpcBefRow    = new int[] { ZXC.Qun2, ZXC.Qun4 };
      hampCentrala.VvBottomMargin = hampCentrala.VvTopMargin;

      Label lbCentrName, lbCentrID, lbCentTick;
      lbCentrID    = hampCentrala.CreateVvLabel  (0, 0, "CID:", ContentAlignment.MiddleRight);
      tbxCentrID   = hampCentrala.CreateVvTextBox(1, 0, "tbxCentrID", "Sifra centrale", GetDB_ColumnSize(DB_ci.centrID));
  
      lbCentTick   = hampCentrala.CreateVvLabel   (2, 0, "CTick:"      , ContentAlignment.MiddleRight);
      tbxCentrTick = hampCentrala.CreateVvTextBox (3, 0, "tbxCentrTick", "Ticker centrale", 10);

      lbCentrName  = hampCentrala.CreateVvLabel   (0, 1, "Centrala:"   , ContentAlignment.MiddleRight);
      tbxCentr     = hampCentrala.CreateVvTextBox (1, 1, "tbxCentrname", "Naziv centrale", GetDB_ColumnSize(DB_ci.centrTick), 2, 0);

      if(ovoJeKupdob_A_NE_Prjkt)
      {
         tbxCentrID.  JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.    SortType, ZXC.AutoCompleteRestrictor.KID_Centrala_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra)  , new EventHandler(AnyKupdobTextBoxLeave));
         tbxCentrTick.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Centrala_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
         tbxCentr.    JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv. SortType, ZXC.AutoCompleteRestrictor.KID_Centrala_Only, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)   , new EventHandler(AnyKupdobTextBoxLeave));
      }
      else
      {
         lbCentrID.Visible  = lbCentTick.Visible   = lbCentrName.Visible =
         tbxCentrID.Visible = tbxCentrTick.Visible = tbxCentr.Visible    = false;     
      }

      tbxCentrTick.JAM_CharacterCasing = CharacterCasing.Upper;
  
      tbxCentrID.JAM_CharEdits    = ZXC.JAM_CharEdits.DigitsOnly;
      //tbxCentrID.JAM_MarkAsNumericTextBox(0, false, decimal.Zero, true);

   }
 
   void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if (tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if (kupdob_rec != null && tb.Text != "")
         {
            Fld_CentrID     = kupdob_rec.KupdobCD/*RecID*/;
            Fld_CentralName = kupdob_rec.Naziv;
            Fld_CentrTick   = kupdob_rec.Ticker;

         }
         else
         {
            Fld_CentraIDAsText = Fld_CentralName = Fld_CentrTick = "";
         }
      }
   }

   #endregion Centrala

   #region Adressa
  
   public void InitializeAdressHamper(out VvHamper hampAdress)
   {
      hampAdress = new VvHamper(7, 6, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hampAdress.VvColWdt      = new int[] { ZXC.Q4un - ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.Q3un - ZXC.Qun4, ZXC.Q2un - ZXC.Qun4, ZXC.Q2un - ZXC.Qun2, ZXC.Q2un + ZXC.Qun8, ZXC.Q3un+ ZXC.Qun10};
      hampAdress.VvSpcBefCol   = new int[] {            ZXC.Qun4, ZXC.Qun4           , ZXC.Qun4,            ZXC.Qun4,           ZXC.Qun4,           ZXC.Qun8, ZXC.Qun4 };
      hampAdress.VvRightMargin = hampAdress.VvLeftMargin;

      hampAdress.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN  };
      hampAdress.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hampAdress.VvBottomMargin = hampAdress.VvTopMargin;

      Label lbUlica1, lbUlica2, lbGrad, lbPostaBr, lbMatbr, lbKontoD, lbKontoK, lbOIB;

      lbUlica1   = hampAdress.CreateVvLabel  (0, 0, "Adresa sjed:" , ContentAlignment.MiddleRight);
      tbxUlica1  = hampAdress.CreateVvTextBox(1, 0, "tbxUlica1"    , "Ulica i broj sjedista", GetDB_ColumnSize(DB_ci.ulica1), 5, 0);
      tbxUlica1.JAM_FieldExitMethod = new EventHandler(CopyUlica1ToUlica2);
      lbUlica2 = hampAdress.CreateVvLabel(0, 1, "Adresa fakt:", ContentAlignment.MiddleRight);
      tbxUlica2  = hampAdress.CreateVvTextBox(1, 1, "tbxUlica2"    , "Ulica i broj za fakturiranje", GetDB_ColumnSize(DB_ci.ulica2), 5, 0);
      lbGrad     = hampAdress.CreateVvLabel  (0, 2, "Grad:"        , ContentAlignment.MiddleRight);
      tbxGrad    = hampAdress.CreateVvTextBox(1, 2, "tbxGrad"      ,"Grad"  , GetDB_ColumnSize(DB_ci.grad)      , 2, 0);
      lbPostaBr  = hampAdress.CreateVvLabel  (4, 2, "PB:"          , ContentAlignment.MiddleRight);
      tbxPostaBr = hampAdress.CreateVvTextBox(5, 2, "tbxPostaBr"   , "Postanski broj", GetDB_ColumnSize(DB_ci.postaBr), 1, 0);

      lbOIB            = hampAdress.CreateVvLabel  (0, 3, "VATc+OIB:"      , ContentAlignment.MiddleRight);
      tbx_vatCntryCode = hampAdress.CreateVvTextBox(1, 3, "tbx_vatCntryCode", "Kod drzave za PDV identifikacijski broj", GetDB_ColumnSize(DB_ci.vatCntryCode));
      tbxOIB           = hampAdress.CreateVvTextBox(2, 3, "tbxOIB"          , "Osobni identifikacijski broj" , GetDB_ColumnSize(DB_ci.oib), 1, 0);

      tbxOIB.JAM_MustTabOutBeforeSubmit = true;
      tbxOIB.JAM_FieldExitMethod = new EventHandler(CheckOIBexists);
      tbxOIB.JAM_FieldExitWithValidationMethod += new System.ComponentModel.CancelEventHandler(CheckOIB_Field_Kupdob);

      lbMatbr    = hampAdress.CreateVvLabel  (4, 3, "MB:"          , ContentAlignment.MiddleRight);
      tbxMatbr   = hampAdress.CreateVvTextBox(5, 3, "tbxMatbr"     , "Maticni (porezni) broj ili JMBG" , GetDB_ColumnSize(DB_ci.matbr), 1, 0);
      lbKontoK   = hampAdress.CreateVvLabel  (0, 4, "KontoK:"      , ContentAlignment.MiddleRight);
      tbxKontoK  = hampAdress.CreateVvTextBox(1, 4, "tbxKontoK"    , "Konto poduzeca kao kupca", GetDB_ColumnSize(DB_ci.kontoPot), 1, 0);
      lbKontoD   = hampAdress.CreateVvLabel  (4, 4, "KontoD:", 1, 0, ContentAlignment.MiddleRight);
      tbxKontoD  = hampAdress.CreateVvTextBox(6, 4, "tbxKontoD"    , "Konto poduzeca kao dobavljaca", GetDB_ColumnSize(DB_ci.kontoDug));

                   hampAdress.CreateVvLabel  (0, 5, "KontoP:"      , ContentAlignment.MiddleRight);
      tbxKontoP  = hampAdress.CreateVvTextBox(1, 5, "tbxKontoP"    , "Uobičajeni konto prihoda za ovog partnera", GetDB_ColumnSize(DB_ci.kontoPrihod), 1, 0);
                   hampAdress.CreateVvLabel  (4, 5, "KontoT:", 1, 0, ContentAlignment.MiddleRight);
      tbxKontoT  = hampAdress.CreateVvTextBox(6, 5, "tbxKontoT"    , "Uobičakeni konto troška za ovog partnera", GetDB_ColumnSize(DB_ci.kontoTrosak));


      tbxKontoK.JAM_CharEdits  = ZXC.JAM_CharEdits.DigitsOnly;
      tbxKontoD.JAM_CharEdits  = ZXC.JAM_CharEdits.DigitsOnly;
      tbxKontoP.JAM_CharEdits  = ZXC.JAM_CharEdits.DigitsOnly;
      tbxKontoT.JAM_CharEdits  = ZXC.JAM_CharEdits.DigitsOnly;      
      
      tbxMatbr.JAM_CharEdits   = ZXC.JAM_CharEdits.DigitsOnly;
      tbxPostaBr.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_vatCntryCode.JAM_CharEdits       = ZXC.JAM_CharEdits.LettersOnly;
      tbx_vatCntryCode.JAM_CharacterCasing = CharacterCasing.Upper; 

      if(ZXC.ThisIsSurgerProject == false && ZXC.ThisIsRemonsterProject == false)
      {
         tbxKontoK.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
         tbxKontoD.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
         tbxKontoP.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
         tbxKontoT.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      }

      if(!ovoJeKupdob_A_NE_Prjkt)
      {
         lbKontoD.Visible  = lbKontoK.Visible  = 
         tbxKontoD.Visible = tbxKontoK.Visible = tbxKontoP.Visible = tbxKontoT.Visible = false;
      }
   }

   public void CopyUlica1ToUlica2(object sender, EventArgs e)
   {
      if(Fld_AdrFakt.IsEmpty()) Fld_AdrFakt = Fld_AdrSjed;
   }

   public void CheckOIBexists(object sender, EventArgs e)
   {
      if(Fld_OIB.NotEmpty())
      {
         KupdobDao.KupdobOIBalreadyExists(TheDbConnection, Fld_OIB, Fld_KupdobCD, ZXC.TheVvForm.TheVvUC is PrjktUC);
      }
   }

   public void CheckTICKERexists(object sender, EventArgs e)
   {
      if(Fld_Ticker.NotEmpty())
      {
         KupdobDao.KupdobTICKER_AlreadyExists(TheDbConnection, Fld_Ticker, Fld_KupdobCD, ZXC.TheVvForm.TheVvUC is PrjktUC);
      }
   }

   #endregion Adressa

   #region Kontakt - i
   
   public void InitializeKontakt1Hamper(out VvHamper hampKontakt1)
   {
      hampKontakt1 = new VvHamper(2, 5, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      int hampCent2col = hampCentrala.VvColWdt[1] + hampCentrala.VvColWdt[2]    + hampCentrala.VvColWdt[3] +
                                                    hampCentrala.VvSpcBefCol[2] + hampCentrala.VvSpcBefCol[3];
      
      hampKontakt1.VvColWdt      = new int[] { hampCentrala.VvColWdt[0], hampCent2col};
      hampKontakt1.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun4 };
      hampKontakt1.VvRightMargin = hampKontakt1.VvLeftMargin;

      hampKontakt1.VvRowHgt       = new int[] {  ZXC.QUN,  ZXC.QUN,  ZXC.QUN,  ZXC.QUN,  ZXC.QUN };
      hampKontakt1.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hampKontakt1.VvBottomMargin = hampKontakt1.VvTopMargin;

      Label lbIme, lbPrezime, lbEmail, lbURL;

      lbIme     = hampKontakt1.CreateVvLabel(0, 0, "Ime:"    , ContentAlignment.MiddleRight);
      lbPrezime = hampKontakt1.CreateVvLabel(0, 1, "Prezime:", ContentAlignment.MiddleRight);
      lbEmail   = hampKontakt1.CreateVvLabel(0, 2, "e-mail:" , ContentAlignment.MiddleRight);
      lbURL     = hampKontakt1.CreateVvLabel(0, 3, "URL:"    , ContentAlignment.MiddleRight);
                  hampKontakt1.CreateVvLabel(0, 4, "Država:" , ContentAlignment.MiddleRight);

      tbxIme     = hampKontakt1.CreateVvTextBox(1, 0, "tbxIme"    , "Ime kontakt - osobe u poduzecu"    , GetDB_ColumnSize(DB_ci.ime));
      tbxPrezime = hampKontakt1.CreateVvTextBox(1, 1, "tbxPrezime", "Prezime kontakt - osobe u poduzecu", GetDB_ColumnSize(DB_ci.prezime));
      tbxEmail   = hampKontakt1.CreateVvTextBox(1, 2, "tbxEmail"  , "Internet e-mail adresa"            , GetDB_ColumnSize(DB_ci.email));
      tbxURL     = hampKontakt1.CreateVvTextBox(1, 3, "tbxURL"    , "Internet adresa web stranice"      , GetDB_ColumnSize(DB_ci.url));
      tbxDrzava  = hampKontakt1.CreateVvTextBox(1, 4, "tbxDrzava" , "Država"                            , GetDB_ColumnSize(DB_ci.drzava));

      // 16.03.2018: kojik?
      //tbxEmail.JAM_CharacterCasing = CharacterCasing.Lower;
      //tbxURL.JAM_CharacterCasing   = CharacterCasing.Lower;

      //TODO: VRATI POSLIJE BEZ TOGA
      //tbxPrezime.JAM_ResultBox = true;
      tbxPrezime.JAM_Highlighted = true;
   }

   public void InitializeKontakt2Hamper(out VvHamper hampKontakt2)
   {
      hampCentrala.Location = new Point(nextX, ZXC.Qun10);

      hampKontakt2 = new VvHamper(2, 4, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hampKontakt2.VvColWdt      = new int[] { hampKontakt1.VvColWdt[0], hampKontakt1.VvColWdt[1] };
      hampKontakt2.VvSpcBefCol   = new int[] {                 ZXC.Qun4,                 ZXC.Qun4 };
      hampKontakt2.VvRightMargin = hampKontakt2.VvLeftMargin;

      hampKontakt2.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN };
      hampKontakt2.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hampKontakt2.VvBottomMargin = hampKontakt2.VvTopMargin;

      Label lbTel1, lbTel2, lbFax, lbGsm;

      lbTel1 = hampKontakt2.CreateVvLabel(0, 0, "Tel1:", ContentAlignment.MiddleRight);
      lbTel2 = hampKontakt2.CreateVvLabel(0, 1, "Tel2:", ContentAlignment.MiddleRight);
      lbFax  = hampKontakt2.CreateVvLabel(0, 2, "Fax:" , ContentAlignment.MiddleRight);
      lbGsm  = hampKontakt2.CreateVvLabel(0, 3, "Gsm:" , ContentAlignment.MiddleRight);

      tbxTel1 = hampKontakt2.CreateVvTextBox(1, 0, "tbxTel1", "Telefon poduzeca", GetDB_ColumnSize(DB_ci.tel1));
      tbxTel2 = hampKontakt2.CreateVvTextBox(1, 1, "tbxTel2", "Telefon poduzeca", GetDB_ColumnSize(DB_ci.tel2));
      tbxFax  = hampKontakt2.CreateVvTextBox(1, 2, "tbxFax" , "Telefax"         , GetDB_ColumnSize(DB_ci.fax));
      tbxGsm  = hampKontakt2.CreateVvTextBox(1, 3, "tbxGsm" , "Gsm"             , GetDB_ColumnSize(DB_ci.gsm));
   }

   #endregion Kontakt - i

   #region Ziro
   
   public void InitializeZiroHamper(out VvHamper hampZiro)
   {
      hampZiro = new VvHamper(5, 6, "", TheTabControl.TabPages[0], false, nextX, nextY + ZXC.Qun4, razmakHamp);

      hampZiro.VvColWdt      = new int[] { ZXC.Q4un - ZXC.Qun4, ZXC.Q8un, ZXC.Q8un + ZXC.Qun4, ZXC.QUN + ZXC.Qun4, ZXC.Q8un + ZXC.Qun2 };
      hampZiro.VvSpcBefCol   = new int[] {            ZXC.Qun4, ZXC.Qun8,            ZXC.Qun8,           ZXC.Qun8,            ZXC.Qun8 };
      hampZiro.VvRightMargin = hampZiro.VvLeftMargin;

      hampZiro.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN };
      hampZiro.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun2 };
      hampZiro.VvBottomMargin = hampZiro.VvTopMargin;

      hampZiro.CreateVvLabel(0, 1, "1:"    , ContentAlignment.MiddleRight);
      hampZiro.CreateVvLabel(0, 2, "2:"    , ContentAlignment.MiddleRight);
      hampZiro.CreateVvLabel(0, 3, "3:"    , ContentAlignment.MiddleRight);
      hampZiro.CreateVvLabel(0, 4, "4:"    , ContentAlignment.MiddleRight);
      hampZiro.CreateVvLabel(0, 5, "Swift:", ContentAlignment.MiddleRight);
      hampZiro.CreateVvLabel(2, 5, "IBAN:" , ContentAlignment.MiddleRight);

      hampZiro.CreateVvLabel(1, 0, "Ziro-račun:"   ,       ContentAlignment.BottomLeft);
      hampZiro.CreateVvLabel(2, 0, "Otvoren kod:"  ,       ContentAlignment.BottomLeft);
      hampZiro.CreateVvLabel(3, 0, "Poziv na broj:", 1, 0, ContentAlignment.BottomLeft);

      tbxZiro1     = hampZiro.CreateVvTextBox(1, 1, "tbxZiro1"    , "Ziro-račun 1"             , GetDB_ColumnSize(DB_ci.ziro1));
      tbxZiro1By   = hampZiro.CreateVvTextBox(2, 1, "tbxZiro1By"  , "Poslovna banka 1"         , GetDB_ColumnSize(DB_ci.ziro1By));
      tbxZiro1PnbM = hampZiro.CreateVvTextBox(3, 1, "tbxZiro1PnbM", "Model placanja (virman) 1", GetDB_ColumnSize(DB_ci.ziro1PnbM));
      tbxZiro1PnbV = hampZiro.CreateVvTextBox(4, 1, "tbxZiro1PnbV", "Poziv na broj (virman) 1" , GetDB_ColumnSize(DB_ci.ziro1PnbV));
      tbxZiro2     = hampZiro.CreateVvTextBox(1, 2, "tbxZiro2"    , "Ziro-račun 2"             , GetDB_ColumnSize(DB_ci.ziro2));
      tbxZiro2By   = hampZiro.CreateVvTextBox(2, 2, "tbxZiro2By"  , "Poslovna banka 2"         , GetDB_ColumnSize(DB_ci.ziro2By));
      tbxZiro2PnbM = hampZiro.CreateVvTextBox(3, 2, "tbxZiro2PnbM", "Model placanja (virman) 2", GetDB_ColumnSize(DB_ci.ziro2PnbM));
      tbxZiro2PnbV = hampZiro.CreateVvTextBox(4, 2, "tbxZiro2PnbV", "Poziv na broj (virman) 2" , GetDB_ColumnSize(DB_ci.ziro2PnbV));
      tbxZiro3     = hampZiro.CreateVvTextBox(1, 3, "tbxZiro3"    , "Ziro-račun 3"             , GetDB_ColumnSize(DB_ci.ziro3));
      tbxZiro3By   = hampZiro.CreateVvTextBox(2, 3, "tbxZiro3By"  , "Poslovna banka 3"         , GetDB_ColumnSize(DB_ci.ziro3By));
      tbxZiro3PnbM = hampZiro.CreateVvTextBox(3, 3, "tbxZiro3PnbM", "Model placanja (virman) 3", GetDB_ColumnSize(DB_ci.ziro3PnbM));
      tbxZiro3PnbV = hampZiro.CreateVvTextBox(4, 3, "tbxZiro3PnbV", "Poziv na broj (virman) 3" , GetDB_ColumnSize(DB_ci.ziro3PnbV));
      tbxZiro4     = hampZiro.CreateVvTextBox(1, 4, "tbxZiro4"    , "Ziro-račun 4"             , GetDB_ColumnSize(DB_ci.ziro4));
      tbxZiro4By   = hampZiro.CreateVvTextBox(2, 4, "tbxZiro4By"  , "Poslovna banka 4"         , GetDB_ColumnSize(DB_ci.ziro4By));
      tbxZiro4PnbM = hampZiro.CreateVvTextBox(3, 4, "tbxZiro4PnbM", "Model placanja (virman) 4", GetDB_ColumnSize(DB_ci.ziro4PnbM));
      tbxZiro4PnbV = hampZiro.CreateVvTextBox(4, 4, "tbxZiro4PnbV", "Poziv na broj (virman) 4" , GetDB_ColumnSize(DB_ci.ziro4PnbV));

      tbxSwift     = hampZiro.CreateVvTextBox(1, 5, "tbxSwift"    , "Swift"                     , GetDB_ColumnSize(DB_ci.swift));
      tbxIban      = hampZiro.CreateVvTextBox(3, 5, "tbxIban"     , "IBAN"                      , GetDB_ColumnSize(DB_ci.iban), 1, 0);

      tbxZiro1PnbM.JAM_CharEdits = 
      tbxZiro2PnbM.JAM_CharEdits = 
      tbxZiro3PnbM.JAM_CharEdits = 
      tbxZiro4PnbM.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

   }

   #endregion Ziro

   # region Rabati

   public void InitializeRabatiHamper(out VvHamper hampRabati)
   {
      hampRabati = new VvHamper(3, 6, "", TheTabControl.TabPages[0], false, nextX , nextY , razmakHamp);

      hampRabati.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Qun2, ZXC.Q2un  };
      hampRabati.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun4, ZXC.Qun4 };
      hampRabati.VvRightMargin = hampRabati.VvLeftMargin;

      hampRabati.VvRowHgt       = new int[] {  ZXC.QUN,  ZXC.QUN,  ZXC.QUN,  ZXC.QUN,  ZXC.QUN,  ZXC.QUN};
      hampRabati.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hampRabati.VvBottomMargin = hampRabati.VvTopMargin;

      hampRabati.CreateVvLabel(0, 0, "Rabat1:"  , ContentAlignment.MiddleRight);
      hampRabati.CreateVvLabel(0, 1, "Rabat2:"  , ContentAlignment.MiddleRight);
      hampRabati.CreateVvLabel(0, 2, "SRabat:"  , ContentAlignment.MiddleRight);
      hampRabati.CreateVvLabel(0, 3, "CasSc:"   , ContentAlignment.MiddleRight);
      hampRabati.CreateVvLabel(0, 4, "ValutaPl:", ContentAlignment.MiddleRight);
      hampRabati.CreateVvLabel(0, 5, "RokOtpr:" , ContentAlignment.MiddleRight);

      tbxStRbt1   = hampRabati.CreateVvTextBox(1, 0, "tbxStRbt1"  , "Redovni rabat kupca"                      , GetDB_ColumnSize(DB_ci.stRbt1), 1, 0);
      tbxStRbt1.JAM_IsForPercent = true;
      
      tbxStRbt2   = hampRabati.CreateVvTextBox(1, 1, "tbxStRbt2"  , "Specijalni rabat kupca"                   , GetDB_ColumnSize(DB_ci.stRbt2), 1, 0);
      tbxStRbt2.JAM_IsForPercent = true;
      tbxStSRbt = hampRabati.CreateVvTextBox(1, 2, "tbxStSRbt", "Super Rabat kupca", GetDB_ColumnSize(DB_ci.stSRbt), 1, 0);
      tbxStSRbt.JAM_IsForPercent = true;
      tbxStCsSc = hampRabati.CreateVvTextBox(1, 3, "tbxStCsSc", "Cassa Sconto kupca", GetDB_ColumnSize(DB_ci.stCsSc), 1, 0);
      tbxStCsSc.JAM_IsForPercent = true;
      tbxValutaPl = hampRabati.CreateVvTextBox(2, 4, "tbxValutaPl", "Fiksna odogoda placanja za ovoga partnera", GetDB_ColumnSize(DB_ci.valutaPl));
      tbxRokOtprm = hampRabati.CreateVvTextBox(2, 5, "tbxRokOtprm", "Rok otpreme robe ovome partneru"          , GetDB_ColumnSize(DB_ci.rokOtprm));

      tbxStRbt1.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbxStRbt1.JAM_DisableNegativeNumberValues = true;
      tbxStRbt2.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbxStRbt2.JAM_DisableNegativeNumberValues = true;
      tbxStSRbt.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbxStSRbt.JAM_DisableNegativeNumberValues = true;
      tbxStCsSc.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
      tbxStCsSc.JAM_DisableNegativeNumberValues = true;

      tbxValutaPl.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbxRokOtprm.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

   }

   #endregion Rabati

   #region Obrt-Pdv-Frgn
  
   public void InitializeISHamper(out VvHamper hampCheckBox)
   {
      hampCheckBox = new VvHamper(1, 7, "", TheTabControl.TabPages[0], false, nextX + ZXC.QUN, nextY, razmakHamp); 

      hampCheckBox.VvColWdt      = new int[] { ZXC.Q6un - ZXC.Qun2 -ZXC.Qun10};
      hampCheckBox.VvSpcBefCol   = new int[] { ZXC.Qun2                      };
      hampCheckBox.VvRightMargin = hampCheckBox.VvLeftMargin;

      hampCheckBox.VvRowHgt       = new int[] { ZXC.QUN,  ZXC.QUN,  ZXC.QUN,  ZXC.QUN, ZXC.QUN,  ZXC.QUN,  ZXC.QUN };
      hampCheckBox.VvSpcBefRow    = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8,ZXC.Qun8, ZXC.Qun8, ZXC.Qun8};
      hampCheckBox.VvBottomMargin = hampCheckBox.VvTopMargin;

      //cbxIsObrt  = hampCheckBox.CreateVvCheckBox_OLD(0, 0, null                            , "Obrt"        , RightToLeft.Yes);
      cbxIsFrgn  = hampCheckBox.CreateVvCheckBox_OLD(0, 0, null                            , "Stranac"     , RightToLeft.Yes);
      cbxIsPdv   = hampCheckBox.CreateVvCheckBox_OLD(0, 0, null                            , "PDV"         , RightToLeft.Yes);
      cbxIsMTros = hampCheckBox.CreateVvCheckBox_OLD(0, 1, null                            , "MjestoTroška", RightToLeft.Yes);
      cbxIsDob   = hampCheckBox.CreateVvCheckBox_OLD(0, 2, null                            , "Dobavljač"   , RightToLeft.Yes);
      cbxIsKup   = hampCheckBox.CreateVvCheckBox_OLD(0, 3, null                            , "Kupac"       , RightToLeft.Yes);
      cbxIsBanka = hampCheckBox.CreateVvCheckBox_OLD(0, 4, null                            , "Banka"       , RightToLeft.Yes);
      cbxIsCentr = hampCheckBox.CreateVvCheckBox_OLD(0, 5, new EventHandler(Centrala_Click), "Centrala"    , RightToLeft.Yes);
      cbxIsOsoba = hampCheckBox.CreateVvCheckBox_OLD(0, 6, null                            , "Osoba"       , RightToLeft.Yes);


      if(!ovoJeKupdob_A_NE_Prjkt)
      {
         cbxIsMTros.Visible =
         cbxIsDob.  Visible =  
         cbxIsKup.  Visible =
         cbxIsBanka.Visible =
         cbxIsCentr.Visible = 
         cbxIsOsoba.Visible = false;
      }

      cbxIsPdv.Visible = false;
   }
 
   public void InitializePodObrtHamper(out VvHamper hamper)
   {
    //hamper = new VvHamper(5, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
    //hamper = new VvHamper(4, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp); 
      hamper = new VvHamper(3, 3, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp); 

    //hamper.VvColWdt      = new int[] { ZXC.Q2un ,ZXC.Q4un, ZXC.Q4un, ZXC.Q5un + ZXC.Qun2, ZXC.Q5un };
    //hamper.VvColWdt      = new int[] { ZXC.Q4un , ZXC.Q4un , ZXC.Q5un , ZXC.Q3un                   };
      hamper.VvColWdt      = new int[] { ZXC.Q4un , ZXC.Q4un , ZXC.Q5un                              };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun10, ZXC.Qun10, ZXC.Qun10                             };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN -ZXC.Qun8, ZXC.QUN -ZXC.Qun8, ZXC.QUN -ZXC.Qun8};
      hamper.VvSpcBefRow    = new int[] {          ZXC.Qun8,          ZXC.Qun8, ZXC.Qun8         };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      rbt_poduzece     = hamper.CreateVvRadioButton(/*1*/0, 0, null, "Poduzeće"    , TextImageRelation.ImageBeforeText);
      rbt_notPdv       = hamper.CreateVvRadioButton(/*2*/1, 0, null, "Nije PDV"    , TextImageRelation.ImageBeforeText);
      rbt_obrtPdv1     = hamper.CreateVvRadioButton(/*1*/0, 1, null, "ObrtPdvR1"   , TextImageRelation.ImageBeforeText);
      rbt_obrtPdv2     = hamper.CreateVvRadioButton(/*2*/1, 1, null, "ObrtPdvR2"   , TextImageRelation.ImageBeforeText);
      rbt_obrtNotPdv   = hamper.CreateVvRadioButton(/*3*/2, 1, null, "ObrtNijePDV" , TextImageRelation.ImageBeforeText);
      rbt_podPoNapl    = hamper.CreateVvRadioButton(/*3*/2, 0, null, "PodPoNaplati", TextImageRelation.ImageBeforeText);
    //rbt_fizickaOsoba = hamper.CreateVvRadioButton(/*4*/3, 1, null, "FizOsoba"    , TextImageRelation.ImageBeforeText);
      rbt_fizickaOsoba = hamper.CreateVvRadioButton(/*4*/2, 2, null, "FizOsoba"    , TextImageRelation.ImageBeforeText);

      rbt_poduzece.Checked = true;
      rbt_poduzece.Tag     = true;
   }
   void Centrala_Click(object sender, EventArgs ea)
   {
      CheckBox cBox = sender as CheckBox;
      if (cBox.Checked)
      {
         tbxCentrID.Enabled = tbxCentrTick.Enabled =  false;
         tbxCentrID.Text    = tbxCentrTick.Text    = tbxCentr.Text = "";
      }
      else
      {
         tbxCentrID.Enabled = tbxCentrTick.Enabled  = true;
      }
   }

   #endregion Obrt-Pdv-Frgn

   // tabPageKupDob1

   #region  TvrtkaTipHamper

   public void InitializeTvrtkaTipHamper(out VvHamper hampTvrtT)
   {
      hampTvrtT = new VvHamper(2, 1, "", TheTabControl.TabPages[1], false, nextX, nextY, razmakHamp);

      hampTvrtT.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q10un * 3 + ZXC.Q5un };
      hampTvrtT.VvSpcBefCol   = new int[] { ZXC.Qun4,                 ZXC.Qun4 };
      hampTvrtT.VvRightMargin = hampTvrtT.VvLeftMargin;

      hampTvrtT.VvRowHgt       = new int[] { ZXC.QUN };
      hampTvrtT.VvSpcBefRow    = new int[] { ZXC.Qun2 };
      hampTvrtT.VvBottomMargin = hampTvrtT.VvTopMargin;

      Label lbDugoIme;
      lbDugoIme = hampTvrtT.CreateVvLabel(0, 0, "Tvrtka:", ContentAlignment.MiddleRight);

      tbxDugoIme = hampTvrtT.CreateVvTextBox(1, 0, "tbxDugoIme", "Cjelokupni naziv / tvrtka drustva", GetDB_ColumnSize(DB_ci.dugoIme));
   }

   #endregion  TvrtkaTipHamper

   #region SifreHamper

   public void InitializeSifreHamper(out VvHamper hampSifre)
   {
      hampSifre = new VvHamper(3, 4, "", TheTabControl.TabPages[1], false, nextX, nextY, razmakHamp);

      hampSifre.VvColWdt      = new int[] { hampTvrtT.VvColWdt[0], ZXC.Q2un + ZXC.Qun10 + ZXC.QUN + ZXC.Qun4, ZXC.Q8un };
      hampSifre.VvSpcBefCol   = new int[] { hampTvrtT.VvSpcBefCol[0], ZXC.Qun4, ZXC.Qun4 };
      hampSifre.VvRightMargin = hampSifre.VvLeftMargin;

      hampSifre.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN , ZXC.QUN, ZXC.QUN };
      hampSifre.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hampSifre.VvBottomMargin = hampSifre.VvTopMargin;

      Label lbSifDname, lbPutName, lbOpcina, lbZupan;
      lbSifDname = hampSifre.CreateVvLabel(0, 0, "Djelatnost:", ContentAlignment.MiddleRight);
      lbOpcina   = hampSifre.CreateVvLabel(0, 1, "Opæina:"    , ContentAlignment.MiddleRight);
      lbZupan    = hampSifre.CreateVvLabel(0, 2, "Županija:"  , ContentAlignment.MiddleRight);
      lbPutName  = hampSifre.CreateVvLabel(0, 3, "Putnik:"    , ContentAlignment.MiddleRight);

      tbxSifDcd   = hampSifre.CreateVvTextBoxLookUp(1, 0, "tbxSifDcd"  , "Šifra djelatnosti", GetDB_ColumnSize(DB_ci.sifDcd));
      tbxSifDname = hampSifre.CreateVvTextBox      (2, 0, "tbxSifDname", "Naziv djelatnosti", GetDB_ColumnSize(DB_ci.sifDname));
      tbxSifDname.JAM_ReadOnly   = true;
      tbxSifDcd.JAM_Set_LookUpTable(ZXC.luiListaDjelat, (int)ZXC.Kolona.prva);
      tbxSifDcd.JAM_lui_NameTaker_JAM_Name = tbxSifDname.JAM_Name;

      tbxOpcCd  = hampSifre.CreateVvTextBoxLookUp(1, 1, "tbxOpcCd" ,"Šifra opæine", GetDB_ColumnSize(DB_ci.opcCd));
      tbxOpcina = hampSifre.CreateVvTextBox      (2, 1, "tbxOpcina", "Naziv opæine", GetDB_ColumnSize(DB_ci.opcina));
      tbxOpcina.JAM_ReadOnly   = true;
      tbxOpcCd.JAM_Set_LookUpTable(ZXC.luiListaOpcina, (int)ZXC.Kolona.prva);
      tbxOpcCd.JAM_lui_NameTaker_JAM_Name = tbxOpcina.JAM_Name;
      
      tbxZupCd = hampSifre.CreateVvTextBoxLookUp(1, 2, "tbxZupCd", "Šifra županije", GetDB_ColumnSize(DB_ci.zupCd));
      tbxZupan = hampSifre.CreateVvTextBox      (2, 2, "tbxZupan", "Naziv županije", GetDB_ColumnSize(DB_ci.zupan));
      tbxZupan.JAM_ReadOnly   = true;
      tbxZupCd.JAM_Set_LookUpTable(ZXC.luiListaZupanija, (int)ZXC.Kolona.prva);
      tbxZupCd.JAM_lui_NameTaker_JAM_Name = tbxZupan.JAM_Name;
      
      tbxPutnikID = hampSifre.CreateVvTextBox(1, 3, "tbxPutnikID", "Sifra putnika / komercijalista", 6/*GetDB_ColumnSize(DB_ci.putnikID) 09.12.2013. vadimo ih iz persona*/);
      tbxPutnikID.JAM_FillCharacter = '0';
      tbxPutName  = hampSifre.CreateVvTextBox(2, 3, "tbxPutName" , "Ime i prez. trg. putnika ili komercijaliste na koga je partner vezan", GetDB_ColumnSize(DB_ci.putName));

      if(this is PrjktUC)
      {
         lbPutName  .Visible =
         tbxPutnikID.Visible =
         tbxPutName .Visible = false;
      }

      //06.11.2013.
      if(ZXC.ThisIsVektorProject)
      {
         tbxPutnikID.JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterSifra), new EventHandler(AnyPersonTextBoxLeave));
         tbxPutnikID.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
         tbxPutName.JAM_CharacterCasing = CharacterCasing.Upper;
      }
   }
   public void AnyPersonTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      TextBox tb = sender as TextBox;
      Person person_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         person_rec = PersonSifrar.Find(FoundInSifrar<Person>);

         if(person_rec != null && tb.Text != "")
         {
            Fld_PutnikID = person_rec.PersonCD/*RecID*/;
            Fld_PutName  = person_rec.PrezimeIme;
         }
         else
         {
            Fld_PutnikCdAsTxt = Fld_PutName = "";
         }
      }
   }

   #endregion SifreHamper

   #region Pnb
 
   public void InitializeBanka_PlacaHamper(out VvHamper hampPl_banka)
   {
      hampPl_banka = new VvHamper(5, 2, "", TheTabControl.TabPages[1], false, nextX, nextY, razmakHamp);

      hampPl_banka.VvColWdt      = new int[] { ZXC.Q6un + ZXC.Qun10, ZXC.QUN + ZXC.Qun4, ZXC.Q8un + ZXC.Qun2 ,ZXC.Q3un, ZXC.Q4un - ZXC.Qun2};
      hampPl_banka.VvSpcBefCol   = new int[] {ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 + ZXC.Qun12, ZXC.Qun4, ZXC.Qun10 };
      hampPl_banka.VvRightMargin = hampPl_banka.VvLeftMargin;

      hampPl_banka.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN };
      hampPl_banka.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hampPl_banka.VvBottomMargin = hampPl_banka.VvTopMargin;

      Label lbPlaca, lbStProviz, lbProvPnb;

      lbPlaca      = hampPl_banka.CreateVvLabel  (0, 0, "Plaæa:"      , ContentAlignment.MiddleRight);
      tbxPnbMPlaca = hampPl_banka.CreateVvTextBox(1, 0, "tbxPnbMPlaca", "Model placanja na zbirnom virmanu za placu", GetDB_ColumnSize(DB_ci.pnbMPlaca));
      tbxPnbVPlaca = hampPl_banka.CreateVvTextBox(2, 0, "tbxPnbVPlaca", "Poziv na broj zbirnog virmana za placu"    , GetDB_ColumnSize(DB_ci.pnbVPlaca));
      lbProvPnb    = hampPl_banka.CreateVvLabel  (0, 1, "PnbProv.:"   , ContentAlignment.MiddleRight);
      tbxPnbMProv  = hampPl_banka.CreateVvTextBox(1, 1, "tbxPnbMProv" , "Model placanja na virmanu za proviziju banci za placu", GetDB_ColumnSize(DB_ci.pnbMProv));
      tbxPnbVProv  = hampPl_banka.CreateVvTextBox(2, 1, "tbxPnbVProv" , "Poziv na broj virmana za proviziju banci za placu", GetDB_ColumnSize(DB_ci.pnbVProv));
      lbStProviz   = hampPl_banka.CreateVvLabel  (3, 1, "StProv.:"    , ContentAlignment.MiddleRight);
      tbxStProviz  = hampPl_banka.CreateVvTextBox(4, 1, "tbxStProviz" , "(eventualna) Stopa provizije banke", GetDB_ColumnSize(DB_ci.stProviz));
    
      
      tbxPnbMPlaca.JAM_CharEdits =
      tbxPnbMProv.JAM_CharEdits  = ZXC.JAM_CharEdits.DigitsOnly;

      tbxStProviz.JAM_MarkAsNumericTextBox(2, false, decimal.MaxValue, 99.99M, true);
   }

   #endregion Pnb

   #region hampREGOB
   
   private void InitializeRegobHamper(out VvHamper hampRegob)
   {
      hampRegob = new VvHamper(3, 3, "", TheTabControl.TabPages[1], false, nextX, nextY, razmakHamp);

      hampRegob.VvColWdt      = new int[] { hampPlBanka.VvColWdt[0], ZXC.Q5un - ZXC.Qun2, ZXC.Q5un };
      hampRegob.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun2 };
      hampRegob.VvRightMargin = hampRegob.VvLeftMargin;

      hampRegob.VvRowHgt       = new int[] { ZXC.QUN  ,ZXC.QUN ,ZXC.QUN  };
      hampRegob.VvSpcBefRow    = new int[] { ZXC.Qun4 ,ZXC.Qun4,ZXC.Qun4 };
      hampRegob.VvBottomMargin = hampRegob.VvTopMargin;

                      hampRegob.CreateVvLabel  (0, 0, "REGOB:"  , ContentAlignment.MiddleRight);
      tbxRegob      = hampRegob.CreateVvTextBox(1, 0, "tbxRegob", "Registarski broj obveznika", GetDB_ColumnSize(DB_ci.regob));

                    hampRegob.CreateVvLabel  (0, 1, "FinLimit:", ContentAlignment.MiddleRight);
      tbxfinLimit = hampRegob.CreateVvTextBox(1, 1, "tbxfinLimit", "Financijski limit", GetDB_ColumnSize(DB_ci.finLimit));
      tbxfinLimit.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      tbxRegob.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

                     hampRegob.CreateVvLabel  (0, 2, "Broj ugovora:", ContentAlignment.MiddleRight);
      tbx_ugovorNo = hampRegob.CreateVvTextBox(1, 2, "tbx_ugovorNo", "Broj ugovora", GetDB_ColumnSize(DB_ci.ugovorNo), 1, 0);

   }

   #endregion hampREGOB

   #region Napomena - Komentar

   public void InitializeNapomenaHamper(out VvHamper hampNapomena)
   {
      hampNapomena = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hampNapomena.VvColWdt      = new int[] { ZXC.Q4un - ZXC.Qun4, ZXC.Q10un + ZXC.Q7un + ZXC.Qun2 + ZXC.Qun4, ZXC.Q4un - ZXC.Qun4, ZXC.Q10un + ZXC.Q7un + ZXC.Qun2};
      hampNapomena.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun4,              ZXC.Qun4, ZXC.Qun4 };
      hampNapomena.VvRightMargin = hampNapomena.VvLeftMargin;

      hampNapomena.VvRowHgt       = new int[] { ZXC.QUN };
      hampNapomena.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hampNapomena.VvBottomMargin = hampNapomena.VvTopMargin;

      Label lb1, lb2;

      lb1       = hampNapomena.CreateVvLabel  (0, 0, "Napomena1:", ContentAlignment.MiddleRight);
      tbxNapom1 = hampNapomena.CreateVvTextBox(1, 0, "tbxNapom1", "Napomena 1", GetDB_ColumnSize(DB_ci.napom1));
      lb2       = hampNapomena.CreateVvLabel  (2, 0, "Napomena2:", ContentAlignment.MiddleRight);
      tbxNapom2 = hampNapomena.CreateVvTextBox(3, 0, "tbxNapom2", "Napomena 2", GetDB_ColumnSize(DB_ci.napom2));


   }

   public void InitializeKomentarHamper(out VvHamper hampKomentar)
   {
      hampKomentar = new VvHamper(2, 3, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);

      hampKomentar.VvColWdt      = new int[] {  ZXC.Q4un - ZXC.Qun4   , hampNapomena.Width - ZXC.Q4un - ZXC.Qun2 };
      hampKomentar.VvSpcBefCol   = new int[] {  ZXC.Qun4, ZXC.Qun4 };
      hampKomentar.VvRightMargin = hampKomentar.VvLeftMargin;

      hampKomentar.VvRowHgt       = new int[] { ZXC.QUN,  ZXC.QUN,  ZXC.QUN };
      hampKomentar.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hampKomentar.VvBottomMargin = hampKomentar.VvTopMargin;

      Label lbK;

      lbK         = hampKomentar.CreateVvLabel  (0, 0, "Komentar:", ContentAlignment.MiddleRight);
      tbxKomentar = hampKomentar.CreateVvTextBox(1, 0, "tbxKomentar", "Komentar", GetDB_ColumnSize(DB_ci.komentar), 0, 2);
      tbxKomentar.Multiline  = true;
      tbxKomentar.ScrollBars = ScrollBars.Vertical;

      tbxKomentar.JAM_BackColor = Color.Bisque;
   }

   #endregion Napomena - Komentar

   #region PCTOGO

   public void InitializePTGHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 4, "", TheTabControl.TabPages[0], false, nextX + ZXC.QUN, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q3un, ZXC.Q9un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN ,  ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun2, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbxPTG_isPojedinacnoFak = hamper.CreateVvCheckBox_OLD(0, 0, null, 2, 0, "Pojedinačno fakturiranje ugovora", RightToLeft.No);

                             hamper.CreateVvLabel        (0, 1, "Dan za fakturiranje:", ContentAlignment.MiddleRight);
      tbx_PTG_DanZaFak     = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_PTG_DanZaFak", "Dan za fakturiranje", GetDB_ColumnSize(DB_ci.fuse1));
      tbx_PTG_DanZaFakOpis = hamper.CreateVvTextBox      (2, 1, "tbx_PTG_DanZaFakOpis", "Dan za fakturiranje", 32);
      tbx_PTG_DanZaFak.JAM_Set_LookUpTable(ZXC.luiListaPTG_DanZaFaktur, (int)ZXC.Kolona.prva);
      tbx_PTG_DanZaFak.JAM_lui_NameTaker_JAM_Name = tbx_PTG_DanZaFakOpis.JAM_Name;
      tbx_PTG_DanZaFakOpis.JAM_ReadOnly = true;


                             hamper.CreateVvLabel        (0, 2, "Slanje računa:", ContentAlignment.MiddleRight);
      tbx_PTG_SlanjeRacuna = hamper.CreateVvTextBoxLookUp("tbx_PTG_SlanjeRacuna", 1, 2, "Slanje računa", GetDB_ColumnSize(DB_ci.fuse2), 1, 0);
      tbx_PTG_SlanjeRacuna.JAM_Set_LookUpTable(ZXC.luiListaPTG_SlanjeRacuna, (int)ZXC.Kolona.prva);

                            hamper.CreateVvLabel  (0, 3, "Zadužen:", ContentAlignment.MiddleRight);
      tbx_PTG_ZaduzenCd   = hamper.CreateVvTextBox(1, 3, "tbx_PTG_ZaduzenCd", "Sifra  komercijalista", 6/*GetDB_ColumnSize(DB_ci.putnikID) 09.12.2013. vadimo ih iz persona*/);
      tbx_PTG_ZaduzenName = hamper.CreateVvTextBox(2, 3, "tbx_PTG_ZaduzenName", "Ime i prez. trg. putnika ili komercijaliste na koga je partner vezan", GetDB_ColumnSize(DB_ci.putName));
      tbx_PTG_ZaduzenCd.JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterSifra), new EventHandler(AnyPersonTextBoxLeave2));
      tbx_PTG_ZaduzenCd.JAM_FillCharacter = '0';
      tbx_PTG_ZaduzenCd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_PTG_ZaduzenName.JAM_CharacterCasing = CharacterCasing.Upper;

      hamper.Visible = ZXC.IsPCTOGO;
   }

   public void AnyPersonTextBoxLeave2(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      TextBox tb = sender as TextBox;
      Person person_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         person_rec = PersonSifrar.Find(FoundInSifrar<Person>);

         if(person_rec != null && tb.Text != "")
         {
            Fld_PTG_ZaduzenCd = person_rec.PersonCD/*RecID*/;
            Fld_PTG_ZaduzenName = person_rec.PrezimeIme;
         }
         else
         {
            Fld_PTG_ZaduzenIdAsTxt = Fld_PTG_ZaduzenName = "";
         }
      }
   }


   #endregion PCTOGO

   #region Tetragram

   public void InitializeTetragramHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 2, "", TheTabControl.TabPages[0], false, nextX + ZXC.QUN, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q10un + ZXC.Q2un, ZXC.Q7un, ZXC.Q4un,ZXC.Q8un, ZXC.Q5un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] {             ZXC.Qun4, ZXC.Qun4, ZXC.QUN ,ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                          hamper.CreateVvLabel  (0, 0, "Državljanstvo:" , ContentAlignment.MiddleRight);
      tbx_drzavljanstvo = hamper.CreateVvTextBox(1, 0, "tbx_drzavljanstvo", "Državljanstvo", GetDB_ColumnSize(DB_ci.idCitizenshp));

      cbx_hasPolIzjava  = hamper.CreateVvCheckBox_OLD(2, 0, null, 1, 0, "Politički izl.os. izjava", RightToLeft.No);

                      hamper.CreateVvLabel  (4, 0, "Datum rođenja:", ContentAlignment.MiddleRight);
      tbx_dateBirth = hamper.CreateVvTextBox(5, 0, "tbx_dateBirth", "Datum rođenja", GetDB_ColumnSize(DB_ci.idBirthDate)); 
      tbx_dateBirth.JAM_IsForDateTimePicker = true;
      dtp_dateBirth = hamper.CreateVvDateTimePicker(5, 0, "", tbx_dateBirth);
      dtp_dateBirth.Name = "dtp_dateBirth";

      
                    hamper.CreateVvLabel  (0, 1, "IDENTIFIKACIJSKA ISPRAVA Broj:", ContentAlignment.MiddleRight);
      tbx_brojIdI = hamper.CreateVvTextBox(1, 1, "tbx_brojIdI", "Broj identifikacijske isprave", GetDB_ColumnSize(DB_ci.idNumber));

                     hamper.CreateVvLabel  (2, 1, "Izdavatelj:", ContentAlignment.MiddleRight);
      tbx_izdavIdI = hamper.CreateVvTextBox(3, 1, "tbx_izdavIdI", "Izdavatelj identifikacijske isprave", GetDB_ColumnSize(DB_ci.idIssuer));

                           hamper.CreateVvLabel  (4, 1, "Datum važenja:", ContentAlignment.MiddleRight);
      tbx_dateVazenjaIdI = hamper.CreateVvTextBox(5, 1, "tbx_dateVazenjaIdI", "Datum važenja identifikacijske isprave", GetDB_ColumnSize(DB_ci.idExpDate)); 
      tbx_dateVazenjaIdI.JAM_IsForDateTimePicker = true;
      dtp_dateVazenjaIdI = hamper.CreateVvDateTimePicker(5, 1, "", tbx_dateVazenjaIdI);
      dtp_dateVazenjaIdI.Name = "dtp_dateVazenjaIdI";

      hamper.Visible = (ovoJeKupdob_A_NE_Prjkt && ZXC.IsTETRAGRAM_ANY);
   }

   #endregion Tetragram

   private void InitializeAgProvHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 4, "", TheTabControl.TabPages[1], false, nextX, nextY, razmakHamp); 

      hamper.VvColWdt      = new int[] { hampPlBanka.VvColWdt[0], ZXC.Q4un - ZXC.Qun2};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Ag.prov. iznos:"      , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 1, "Ag.prov. %:"          , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 2, "Investicijski trošak:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 3, "Trošak naplate:"      , ContentAlignment.MiddleRight);

      tbx_mitoIzn  = hamper.CreateVvTextBox(1, 0, "tbx_mitoIzn" , "", GetDB_ColumnSize(DB_ci.mitoIzn ));
      tbx_mitoSt   = hamper.CreateVvTextBox(1, 1, "tbx_mitoSt"  , "", GetDB_ColumnSize(DB_ci.mitoSt  ));
      tbx_investTr = hamper.CreateVvTextBox(1, 2, "tbx_investTr", "", GetDB_ColumnSize(DB_ci.investTr));
      tbx_trecaStr = hamper.CreateVvTextBox(1, 3, "tbx_trecaStr", "", GetDB_ColumnSize(DB_ci.trecaStr));

      tbx_mitoIzn .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_mitoSt  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_investTr.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_trecaStr.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_mitoSt.JAM_IsForPercent = true;
   }

   public void InitializeKomisijaHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 4, "", TheTabControl.TabPages[1], false, nextX, nextY, razmakHamp); 

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q6un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
           
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Komisijsko skladište:", ContentAlignment.MiddleRight);

      rbt_komisNE = hamper.CreateVvRadioButton(1, 0, null, "NIJE"        , TextImageRelation.ImageBeforeText);
      rbt_komisVP = hamper.CreateVvRadioButton(1, 1, null, "Veleprodajno", TextImageRelation.ImageBeforeText);
      rbt_komisMP = hamper.CreateVvRadioButton(1, 2, null, "Maloprodajno", TextImageRelation.ImageBeforeText);

      rbt_komisNE.Checked = true;
      rbt_komisNE.Tag     = true;

      //hamper.CreateVvLabel(1, 3, "Šifra", ContentAlignment.MiddleLeft);
      //hamper.CreateVvLabel(2, 3, "Naziv", ContentAlignment.MiddleLeft);
      //hamper.CreateVvLabel(3, 3, "Konto", ContentAlignment.MiddleLeft);
      //hamper.CreateVvLabel(4, 3, "M"    , ContentAlignment.MiddleLeft);
      //hamper.CreateVvLabel(5, 3, "Broj" , ContentAlignment.MiddleLeft);

      //tbx_komisSkladCD    = hamper.CreateVvTextBox(1, 4, "tbx_komisSkladCD   ", "Komisijski SkladCD   "/*, GetDB_ColumnSize(DB_ci.napom1)*/);
      //tbx_komisSkladNaziv = hamper.CreateVvTextBox(2, 4, "tbx_komisSkladNaziv", "Komisijski SkladNaziv"/*, GetDB_ColumnSize(DB_ci.napom1)*/);
      //tbx_komisSkladKonto = hamper.CreateVvTextBox(3, 4, "tbx_komisSkladKonto", "Komisijski SkladKonto"/*, GetDB_ColumnSize(DB_ci.napom1)*/);
      //tbx_komisSkladMalop = hamper.CreateVvTextBox(4, 4, "tbx_komisSkladMalop", "Komisijski SkladMalop"/*, GetDB_ColumnSize(DB_ci.napom1)*/);
      //tbx_komisSkladBroj  = hamper.CreateVvTextBox(5, 4, "tbx_komisSkladBroj ", "Komisijski SkladBroj "/*, GetDB_ColumnSize(DB_ci.napom1)*/);

      //tbx_komisSkladCD   .JAM_ReadOnly = 
      //tbx_komisSkladNaziv.JAM_ReadOnly = 
      //tbx_komisSkladMalop.JAM_ReadOnly = true;

      //tbx_komisSkladKonto.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      
      //if(ZXC.ThisIsSurgerProject == false && ZXC.ThisIsRemonsterProject == false)
      //{
      //   tbx_komisSkladKonto.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);
      //   tbx_komisSkladKonto.JAM_SetAutoCompleteData(Kplan.recordName, Kplan.sorterKonto.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kplan_sorterCode), null);

      //   tbx_komisSkladBroj.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      //   tbx_komisSkladBroj.JAM_FieldExitWithValidationMethod += new System.ComponentModel.CancelEventHandler(OnExitCheckRange);
      //}
   }


   private void InitializeTimeHamper(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 8, "", TheTabControl.TabPages[1], false, nextX, nextY, razmakHamp); 

      hamper.VvColWdt      = new int[] { hampTvrtT.VvColWdt[0], ZXC.Q3un,ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] {              ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Dan:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(1, 0, "OD:" , ContentAlignment.MiddleLeft);
      hamper.CreateVvLabel(2, 0, "DO:" , ContentAlignment.MiddleLeft);
      
      hamper.CreateVvLabel(0, 1, "PON:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 2, "UTO:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 3, "SRI:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 4, "ČET:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 5, "PET:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 6, "SUB:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 7, "NED:", ContentAlignment.MiddleRight);

      tbx_timeOd_1 = hamper.CreateVvTextBox(1, 1, "tbx_timeOd_1", "Radno vrijeme Ponedjeljak Od", GetDB_ColumnSize(DB_ci.timeOd_1)); tbx_timeOd_1.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; dtp_timeOd_1 = hamper.CreateVvDateTimePicker(1, 1, "", tbx_timeOd_1); dtp_timeOd_1.Name = "dtp_timeOd_1";
      tbx_timeDo_1 = hamper.CreateVvTextBox(2, 1, "tbx_timeDo_1", "Radno vrijeme Ponedjeljak Do", GetDB_ColumnSize(DB_ci.timeDo_1)); tbx_timeDo_1.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; dtp_timeDo_1 = hamper.CreateVvDateTimePicker(2, 1, "", tbx_timeDo_1); dtp_timeDo_1.Name = "dtp_timeDo_1";
      tbx_timeOd_2 = hamper.CreateVvTextBox(1, 2, "tbx_timeOd_2", "Radno vrijeme Utorak Od"     , GetDB_ColumnSize(DB_ci.timeOd_2)); tbx_timeOd_2.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; dtp_timeOd_2 = hamper.CreateVvDateTimePicker(1, 2, "", tbx_timeOd_2); dtp_timeOd_2.Name = "dtp_timeOd_2"; 
      tbx_timeDo_2 = hamper.CreateVvTextBox(2, 2, "tbx_timeDo_2", "Radno vrijeme Utorak Do"     , GetDB_ColumnSize(DB_ci.timeDo_2)); tbx_timeDo_2.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; dtp_timeDo_2 = hamper.CreateVvDateTimePicker(2, 2, "", tbx_timeDo_2); dtp_timeDo_2.Name = "dtp_timeDo_2";
      tbx_timeOd_3 = hamper.CreateVvTextBox(1, 3, "tbx_timeOd_3", "Radno vrijeme Srijeda Od"    , GetDB_ColumnSize(DB_ci.timeOd_3)); tbx_timeOd_3.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; dtp_timeOd_3 = hamper.CreateVvDateTimePicker(1, 3, "", tbx_timeOd_3); dtp_timeOd_3.Name = "dtp_timeOd_3";
      tbx_timeDo_3 = hamper.CreateVvTextBox(2, 3, "tbx_timeDo_3", "Radno vrijeme Srijeda Do"    , GetDB_ColumnSize(DB_ci.timeDo_3)); tbx_timeDo_3.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; dtp_timeDo_3 = hamper.CreateVvDateTimePicker(2, 3, "", tbx_timeDo_3); dtp_timeDo_3.Name = "dtp_timeDo_3";
      tbx_timeOd_4 = hamper.CreateVvTextBox(1, 4, "tbx_timeOd_4", "Radno vrijeme Četvrtak Od"   , GetDB_ColumnSize(DB_ci.timeOd_4)); tbx_timeOd_4.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; dtp_timeOd_4 = hamper.CreateVvDateTimePicker(1, 4, "", tbx_timeOd_4); dtp_timeOd_4.Name = "dtp_timeOd_4";
      tbx_timeDo_4 = hamper.CreateVvTextBox(2, 4, "tbx_timeDo_4", "Radno vrijeme Četvrtak Do"   , GetDB_ColumnSize(DB_ci.timeDo_4)); tbx_timeDo_4.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; dtp_timeDo_4 = hamper.CreateVvDateTimePicker(2, 4, "", tbx_timeDo_4); dtp_timeDo_4.Name = "dtp_timeDo_4";
      tbx_timeOd_5 = hamper.CreateVvTextBox(1, 5, "tbx_timeOd_5", "Radno vrijeme Petak Od"      , GetDB_ColumnSize(DB_ci.timeOd_5)); tbx_timeOd_5.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; dtp_timeOd_5 = hamper.CreateVvDateTimePicker(1, 5, "", tbx_timeOd_5); dtp_timeOd_5.Name = "dtp_timeOd_5";
      tbx_timeDo_5 = hamper.CreateVvTextBox(2, 5, "tbx_timeDo_5", "Radno vrijeme Petak Do"      , GetDB_ColumnSize(DB_ci.timeDo_5)); tbx_timeDo_5.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; dtp_timeDo_5 = hamper.CreateVvDateTimePicker(2, 5, "", tbx_timeDo_5); dtp_timeDo_5.Name = "dtp_timeDo_5";
      tbx_timeOd_6 = hamper.CreateVvTextBox(1, 6, "tbx_timeOd_6", "Radno vrijeme Subota Od"     , GetDB_ColumnSize(DB_ci.timeOd_6)); tbx_timeOd_6.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; dtp_timeOd_6 = hamper.CreateVvDateTimePicker(1, 6, "", tbx_timeOd_6); dtp_timeOd_6.Name = "dtp_timeOd_6";
      tbx_timeDo_6 = hamper.CreateVvTextBox(2, 6, "tbx_timeDo_6", "Radno vrijeme Subota Do"     , GetDB_ColumnSize(DB_ci.timeDo_6)); tbx_timeDo_6.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; dtp_timeDo_6 = hamper.CreateVvDateTimePicker(2, 6, "", tbx_timeDo_6); dtp_timeDo_6.Name = "dtp_timeDo_6";
      tbx_timeOd_7 = hamper.CreateVvTextBox(1, 7, "tbx_timeOd_7", "Radno vrijeme Nedjelja Od"   , GetDB_ColumnSize(DB_ci.timeOd_7)); tbx_timeOd_7.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; dtp_timeOd_7 = hamper.CreateVvDateTimePicker(1, 7, "", tbx_timeOd_7); dtp_timeOd_7.Name = "dtp_timeOd_7";
      tbx_timeDo_7 = hamper.CreateVvTextBox(2, 7, "tbx_timeDo_7", "Radno vrijeme Nedjelja Do"   , GetDB_ColumnSize(DB_ci.timeDo_7)); tbx_timeDo_7.JAM_IsForDateTimePicker_TimeOnlyDisplay = true; dtp_timeDo_7 = hamper.CreateVvDateTimePicker(2, 7, "", tbx_timeDo_7); dtp_timeDo_7.Name = "dtp_timeDo_7";

   }

   void dtp_OnEnter_resetSelection(object sender, EventArgs e)
   {
      VvDateTimePicker vvDTP = sender as VvDateTimePicker;
      
      DateTimePickerFormat CurrentFormat = vvDTP.Format;
      string CurrentCustomFormat = vvDTP.CustomFormat;
    
      if(vvDTP.Format != DateTimePickerFormat.Custom) 
      {
         vvDTP.Format = DateTimePickerFormat.Custom;
         vvDTP.CustomFormat = "";
      }
      else
      {
         vvDTP.Format = DateTimePickerFormat.Short;
      }
    
      vvDTP.Format = CurrentFormat;
      vvDTP.CustomFormat = CurrentCustomFormat;

   }

   private void OnExitCheckRange(object sender, System.ComponentModel.CancelEventArgs e)
   {
      //if(this.Visible == false) return;
      //if(Fld_Komisija == ZXC.KomisijaKindEnum.NIJE) return;

      //if(Fld_KomisSkladNum < 11 || Fld_KomisSkladNum > 99)
      //{
      //   ZXC.aim_emsg(MessageBoxIcon.Error,
      //      " Brojčana oznaka skladišta {0} mora biti između 11 i 99!", Fld_KomisSkladNum);
      //   e.Cancel = true;
      //}
   }

   #region R1Kind

   public void InitializeIsMandatory_KupdobHamper (out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp+ ZXC.Qun8); 

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q5un, ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun2, ZXC.Qun8, ZXC.Qun8};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun2 };
      hamper.VvBottomMargin = 0;

      rbt_R1nepoznato  = hamper.CreateVvRadioButton(0, 0, null, "Nedef"      , TextImageRelation.ImageBeforeText);
      rbt_B2B          = hamper.CreateVvRadioButton(1, 0, null, "B2B - pravna" , TextImageRelation.ImageBeforeText);
      rbt_B2C          = hamper.CreateVvRadioButton(2, 0, null, "B2C - fizička", TextImageRelation.ImageBeforeText);

      rbt_R1nepoznato.Checked = true;
      rbt_R1nepoznato.Tag     = true;

     hamper.Visible = ovoJeKupdob_A_NE_Prjkt;

    //if(!ZXC.IsF2_2026_rules)
    //{ 
    //   rbt_R1nepoznato.Enabled = false;
    //   rbt_B2B        .Enabled = false;
    //   rbt_B2C        .Enabled = false;
    //}

      hamper.BackColor = Color.Lavender;
   }

   #endregion R1Kind

   #endregion HAMPERI

   #region Filter

   public override void CreateRptFilterAndRptFilterUC()
   {
     // if(ZXC.ThisIsSurgerProject == true && ZXC.ThisIsRemonsterProject == true) return;
      if(ZXC.ThisIsSurgerProject == true || ZXC.ThisIsRemonsterProject == true) return;

      TheFtransFilter    = new VvRpt_Fin_Filter();

      TheFinFilterUC          = new FinFilterUC(this);
      TheFinFilterUC.Parent   = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheFinFilterUC.Width;
   }

   #endregion Filter

   #region DataGridView

   private void CreateDataGrid_InitializeTheGrid_ReadOnly_Columns()
   {
      aTransesGrid[0] = CreateDataGridView_ReadOnly(TheTabControl.TabPages[ftrans_TabPageName], "KupDob_Ftrans");
      int minGridWIdth = InitializeTheGrid_ReadOnly_ftransColumns();
      aTransesGrid[0].Dock = DockStyle.Fill;
      aTransesGrid[0].DoubleClick += new EventHandler(theFIRST_TransGrid_DoubleClick);
      aTransesGrid[0].KeyPress += new KeyPressEventHandler(theFIRST_TransGrid_KeyPress);

      aTransesGrid[1]   = CreateDataGridView_ReadOnly(TheTabControl.TabPages[KDC_TabPageName], KDC_TabPageName);
      int minGrid2WIdth = InitializeTheGrid_ReadOnly_xtransKDCColumns();
      aTransesGrid[1].Dock = DockStyle.Fill;
    //aTransesGrid[1].DoubleClick += new EventHandler(theFIRST_TransGrid_DoubleClick);
    //aTransesGrid[1].KeyPress    += new KeyPressEventHandler(theFIRST_TransGrid_KeyPress);
   }

   private int InitializeTheGrid_ReadOnly_ftransColumns()
   {
      int sumOfColWidth = 0, colWidth;
      int colDateWidth  = ZXC.Q4un + ZXC.Qun4;
      int colSif6Width  = ZXC.Q3un + ZXC.Qun8;

      sumOfColWidth += aTransesGrid[0].RowHeadersWidth;
      colWidth = colSif6Width       ;                            AddDGVColum_RecID_4GridReadOnly   (aTransesGrid[0], "RecID"     , colWidth, false, 0);
      colWidth = colDateWidth       ; sumOfColWidth += colWidth; AddDGVColum_DateTime_4GridReadOnly(aTransesGrid[0], "Datum"     , colWidth);
      colWidth = colSif6Width       ; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (aTransesGrid[0], "Nalog"     , colWidth, true, 6);
      colWidth = ZXC.Q2un + ZXC.Qun4; sumOfColWidth += colWidth; AddDGVColum_Integer_4GridReadOnly (aTransesGrid[0], "Red"       , colWidth, false, 0);
      colWidth = ZXC.Q2un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (aTransesGrid[0], "VK"        , colWidth, false);
      colWidth = ZXC.Q3un + ZXC.Qun4; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (aTransesGrid[0], "Konto"     , colWidth, false);
      colWidth = ZXC.Q6un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (aTransesGrid[0], "Opis"      , colWidth, true);
      colWidth = ZXC.Q4un           ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly  (aTransesGrid[0], "VezniDOk"  , colWidth, false);
      colWidth = colDateWidth       ; sumOfColWidth += colWidth; AddDGVColum_DateTime_4GridReadOnly(aTransesGrid[0], "Valuta"    , colWidth);
      colWidth = ZXC.Q6un           ; sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[0], "Duguje"    , colWidth, 2);
      colWidth = ZXC.Q6un           ; sumOfColWidth += colWidth; AddDGVColum_Decimal_4GridReadOnly (aTransesGrid[0], "Potražuje" , colWidth, 2);

      return sumOfColWidth;
   }

   protected override void theFIRST_TransGrid_DoubleClick(object sender, EventArgs e)
   {
      base.OpenNew_Record_TabPage_OnDoubleClick(ZXC.VvSubModulEnum.NAL_F, SelectedRecIDIn_FIRST_TransGrid);
   }

   private int InitializeTheGrid_ReadOnly_xtransKDCColumns()
   {
      int sumOfColWidth = 0, colWidth;
      int colDateWidth  = ZXC.Q4un + ZXC.Qun4;
      int colSif6Width  = ZXC.Q3un + ZXC.Qun8;

      sumOfColWidth += aTransesGrid[0].RowHeadersWidth;
      colWidth = colSif6Width       ;                  AddDGVColum_RecID_4GridReadOnly (aTransesGrid[1], "RecID"         , colWidth, false, 0);
      colWidth = ZXC.Q9un ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(aTransesGrid[1], "Ime i prezime" , colWidth, false);
      colWidth = ZXC.Q9un ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(aTransesGrid[1], "Telefon"       , colWidth, false);
      colWidth = ZXC.Q10un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(aTransesGrid[1], "e-mail"        , colWidth, true);
      colWidth = ZXC.Q7un ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(aTransesGrid[1], "Funkcija"      , colWidth, false);
      colWidth = ZXC.Q2un ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(aTransesGrid[1], "ZaFakt"        , colWidth, false);
      colWidth = ZXC.Q6un ; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(aTransesGrid[1], "Napomena"      , colWidth, true);

      return sumOfColWidth;
   }

   private void AddColumnsToKDCGrid() 
   {
      //DataGridViewTextBoxColumn col;

      AddDGVColum_RecID_4GridReadOnly (kdcGrid, "RecID"        , ZXC.Q3un, false, 0);
      AddDGVColum_String_4GridReadOnly(kdcGrid, "Ime i prezime", ZXC.Q7un, false   );
      AddDGVColum_String_4GridReadOnly(kdcGrid, "Telefon"      , ZXC.Q5un, false   );
      AddDGVColum_String_4GridReadOnly(kdcGrid, "e-mail"       , ZXC.Q6un, false   );
      AddDGVColum_String_4GridReadOnly(kdcGrid, "Funkcija"     , ZXC.Q7un, false   );
      AddDGVColum_String_4GridReadOnly(kdcGrid, "ZaFakt"       , ZXC.Q2un, false   );
      AddDGVColum_String_4GridReadOnly(kdcGrid, "Napomena"     , ZXC.Q6un, true    );
   }

   #endregion DataGridView

   #region PutFields(), GetFields()

   #region Fld_XY

   //public uint Fld_RecID
   //{
   //   set { tbxKupdobCD.Text = value.ToString("000000"); }
   //}


   /*05*/ public string Fld_Naziv
   {
      get { return tbxNaziv.Text; }
      set {        tbxNaziv.Text = value; }
   }
   /*06*/ public string Fld_Ticker
   {
      get { return tbxTick.Text; }
      set {        tbxTick.Text = value; }
   }
   /*07*/  public uint Fld_KupdobCD
   {
      get { return ZXC.ValOrZero_UInt(tbxKupdobCD.Text); }
      set { tbxKupdobCD.Text = value.ToString("000000"); }
   }

   /*08*/
   public uint Fld_CentrID
   {
      get { return tbxCentrID.GetSomeRecIDField(); }
      set {        tbxCentrID.PutSomeRecIDField(value); }
   }

   public string Fld_CentralName
   {
      get { return tbxCentr.Text; }
      set {        tbxCentr.Text = value; }
   }

   public string Fld_CentraIDAsText
   {
      get { return tbxCentrTick.Text; }
      set {        tbxCentrTick.Text = value; }
   }

   /*09*/
   public string Fld_AdrSjed
   {
      get { return tbxUlica1.Text; }
      set {        tbxUlica1.Text = value; }
   }
   /*10*/
   public string Fld_AdrFakt
   {
      get { return tbxUlica2.Text; }
      set {        tbxUlica2.Text = value; }
   }
   /*11*/
   public string Fld_Grad
   {
      get { return tbxGrad.Text; }
      set {        tbxGrad.Text = value; }
   }
   /*12*/
   public string Fld_PostaBr
   {
      get { return tbxPostaBr.Text; }
      set {        tbxPostaBr.Text = value; }
   }
   /*13*/
   public string Fld_Matbr
   {
      get { return tbxMatbr.Text; }
      set {        tbxMatbr.Text = value; }
   }
   /*14*/
   public string Fld_KontoD
   {
      get { return tbxKontoD.Text; }
      set {        tbxKontoD.Text = value; }
   }
   /*15*/
   public string Fld_KontoK
   {
      get { return tbxKontoK.Text; }
      set {        tbxKontoK.Text = value; }
   }
   /*16*/
   public string Fld_Ime
   {
      get { return tbxIme.Text; }
      set {        tbxIme.Text = value; }
   }
   /*17*/
   public string Fld_Prezime
   {
      get { return tbxPrezime.Text; }
      set {        tbxPrezime.Text = value; }
   }
   /*18*/
   public string Fld_Email
   {
      get { return tbxEmail.Text; }
      set {        tbxEmail.Text = value; }
   }
   /*19*/
   public string Fld_URL
   {
      get { return tbxURL.Text; }
      set {        tbxURL.Text = value; }
   }
   /*20*/
   public string Fld_Tel1
   {
      get { return tbxTel1.Text; }
      set {        tbxTel1.Text = value; }
   }
   /*21*/
   public string Fld_Tel2
   {
      get { return tbxTel2.Text; }
      set {        tbxTel2.Text = value; }
   }
   /*22*/
   public string Fld_Fax
   {
      get { return tbxFax.Text; }
      set {        tbxFax.Text = value; }
   }
   /*23*/
   public string Fld_Gsm
   {
      get { return tbxGsm.Text; }
      set {        tbxGsm.Text = value; }
   }
   /*24*/
   public string Fld_Ziro1
   {
      get { return tbxZiro1.Text; }
      set {        tbxZiro1.Text = value; }
   }
   /*25*/
   public string Fld_Ziro1By
   {
      get { return tbxZiro1By.Text; }
      set {        tbxZiro1By.Text = value; }
   }
   /*26*/
   public string Fld_Ziro1PnbM
   {
      get { return tbxZiro1PnbM.Text; }
      set {        tbxZiro1PnbM.Text = value; }
   }
   /*27*/
   public string Fld_Ziro1PnbV
   {
      get { return tbxZiro1PnbV.Text; }
      set {        tbxZiro1PnbV.Text = value; }
   }
   /*28*/
   public string Fld_Ziro2
   {
      get { return tbxZiro2.Text; }
      set {        tbxZiro2.Text = value; }
   }
   /*29*/
   public string Fld_Ziro2By
   {
      get { return tbxZiro2By.Text; }
      set {        tbxZiro2By.Text = value; }
   }
   /*30*/
   public string Fld_Ziro2PnbM
   {
      get { return tbxZiro2PnbM.Text; }
      set {        tbxZiro2PnbM.Text = value; }
   }
   /*31*/
   public string Fld_Ziro2PnbV
   {
      get { return tbxZiro2PnbV.Text; }
      set {        tbxZiro2PnbV.Text = value; }
   }
   /*32*/
   public string Fld_Ziro3
   {
      get { return tbxZiro3.Text; }
      set {        tbxZiro3.Text = value; }
   }
   /*33*/
   public string Fld_Ziro3By
   {
      get { return tbxZiro3By.Text; }
      set {        tbxZiro3By.Text = value; }
   }
   /*34*/
   public string Fld_Ziro3PnbM
   {
      get { return tbxZiro3PnbM.Text; }
      set {        tbxZiro3PnbM.Text = value; }
   }
   /*35*/
   public string Fld_Ziro3PnbV
   {
      get { return tbxZiro3PnbV.Text; }
      set {        tbxZiro3PnbV.Text = value; }
   }
   /*36*/
   public string Fld_Ziro4
   {
      get { return tbxZiro4.Text; }
      set {        tbxZiro4.Text = value; }
   }
   /*37*/
   public string Fld_Ziro4By
   {
      get { return tbxZiro4By.Text; }
      set {        tbxZiro4By.Text = value; }
   }
   /*38*/
   public string Fld_Ziro4PnbM
   {
      get { return tbxZiro4PnbM.Text; }
      set {        tbxZiro4PnbM.Text = value; }
   }
   /*39*/
   public string Fld_Ziro4PnbV
   {
      get { return tbxZiro4PnbV.Text; }
      set {        tbxZiro4PnbV.Text = value; }
   }
   /*40*/
   public bool Fld_IsCentr
   {
      get { return cbxIsCentr.Checked; }
      set {        cbxIsCentr.Checked = value; }
   }
   /*41*/
   public string Fld_DugoIme
   {
      get { return tbxDugoIme.Text; }
      set {        tbxDugoIme.Text = value; }
   }
   /*42*/
   public string Fld_Tip
   {
      get { return tbxGrupa.Text; }
      set {        tbxGrupa.Text = value; }
   }

   /*43*/
   public string Fld_SifDcd
   {
      get { return tbxSifDcd.Text; }
      set {        tbxSifDcd.Text = value; }
   }
   /*44*/
   public string Fld_SifDname
   {
      get { return tbxSifDname.Text; }
      set {        tbxSifDname.Text = value; }
   }
   /*45*/
   public uint Fld_PutnikID
   {
      get { return tbxPutnikID.GetSomeRecIDField(); }
      set {        tbxPutnikID.PutSomeRecIDField(value); }
   }

   public string Fld_PutnikCdAsTxt { get { return tbxPutnikID.Text; } set { tbxPutnikID.Text = value; } }

   /*46*/
   public string Fld_PutName
   {
      get { return tbxPutName.Text; }
      set {        tbxPutName.Text = value; }
   }
   /*47*/
   public string Fld_Opcina
   {
      get { return tbxOpcina.Text; }
      set {        tbxOpcina.Text = value; }
   }
   /*48*/
   public string Fld_OpcCd
   {
      get { return tbxOpcCd.Text; }
      set {        tbxOpcCd.Text = value; }
   }
   /*49*/
   public string Fld_Zupan
   {
      get { return tbxZupan.Text; }
      set {        tbxZupan.Text = value; }
   }
   /*50*/
   public string Fld_ZupCd
   {
      get { return tbxZupCd.Text; }
      set {        tbxZupCd.Text = value; }
   }
   /*51*/
   public string Fld_Regob
   {
      get { return tbxRegob.Text; }
      set {        tbxRegob.Text = value; }
   }
   /*52*/
   public string Fld_PnbMPlaca
   {
      get { return tbxPnbMPlaca.Text; }
      set {        tbxPnbMPlaca.Text = value; }
   }
   /*53*/
   public string Fld_PnbVPlaca
   {
      get { return tbxPnbVPlaca.Text; }
      set {        tbxPnbVPlaca.Text = value; }
   }
   /*54*/
   public decimal Fld_StProviz
   {
      get { return tbxStProviz.GetDecimalField(); }
      set {        tbxStProviz.PutDecimalField(value); }
   }
   /*55*/
   public string Fld_PnbMProv
   {
      get { return tbxPnbMProv.Text; }
      set {        tbxPnbMProv.Text = value; }
   }
   /*56*/
   public string Fld_PnbVProv
   {
      get { return tbxPnbVProv.Text; }
      set {        tbxPnbVProv.Text = value; }
   }
   /*57*/
   public string Fld_Napom1
   {
      get { return tbxNapom1.Text; }
      set {        tbxNapom1.Text = value; }
   }
   /*58*/
   public string Fld_Napom2
   {
      get { return tbxNapom2.Text; }
      set {        tbxNapom2.Text = value; }
   }
   /*59*/
   public string Fld_Komentar
   {
      get { return tbxKomentar.Text; }
      set {        tbxKomentar.Text = value; }
   }

   /*60*/
   public decimal Fld_StRbt1
   {
      get { return tbxStRbt1.GetDecimalField(); }
      set {        tbxStRbt1.PutDecimalField(value); }
   }

   /*61*/
   public decimal Fld_StRbt2
   {
      get { return tbxStRbt2.GetDecimalField(); }
      set {        tbxStRbt2.PutDecimalField(value); }

   }
   /*62*/
   public decimal Fld_StSRbt
   {
      get { return tbxStSRbt.GetDecimalField(); }
      set {        tbxStSRbt.PutDecimalField(value); }
   }
   /*63*/
   public decimal Fld_StCsSc
   {
      get { return tbxStCsSc.GetDecimalField(); }
      set {        tbxStCsSc.PutDecimalField(value); }
   }
   /*64*/
   public short Fld_ValutaPl
   {
      get { return tbxValutaPl.GetShortField(); }
      set {        tbxValutaPl.PutShortField(value); }
   }
   /*65*/
   public short Fld_RokOtprm
   {
      get { return tbxRokOtprm.GetShortField(); }
      set {        tbxRokOtprm.PutShortField(value); }
   }

   /*66*/
   //public bool Fld_IsObrt
   //{
   //   get { return cbxIsObrt.Checked; }
   //   set {        cbxIsObrt.Checked = value; }
   //}
   public ZXC.PdvRTipEnum Fld_IsObrt
   {
      get
      {
              if(rbt_poduzece     .Checked) return ZXC.PdvRTipEnum.PODUZECE_R1  ;
         else if(rbt_obrtPdv1     .Checked) return ZXC.PdvRTipEnum.OBRT_R1      ;
         else if(rbt_obrtPdv2     .Checked) return ZXC.PdvRTipEnum.OBRT_R2      ;
         else if(rbt_obrtNotPdv   .Checked) return ZXC.PdvRTipEnum.OBRT_NOT_PDV ;
         else if(rbt_notPdv       .Checked) return ZXC.PdvRTipEnum.NOT_IN_PDV   ;
         else if(rbt_podPoNapl    .Checked) return ZXC.PdvRTipEnum.POD_PO_NAPL  ; //poduzece po naplati od 01.01.2015.
         else if(rbt_fizickaOsoba .Checked) return ZXC.PdvRTipEnum.FIZICKA_OSOBA; //poduzece po naplati od 01.01.2015.
              else throw new Exception("Fld_IsObrt: who df is checked?")    ;
      }
      set
      {
         switch(value)
         {
            case ZXC.PdvRTipEnum.PODUZECE_R1   : rbt_poduzece    .Checked = true; break;
            case ZXC.PdvRTipEnum.OBRT_R1       : rbt_obrtPdv1    .Checked = true; break;
            case ZXC.PdvRTipEnum.OBRT_R2       : rbt_obrtPdv2    .Checked = true; break;
            case ZXC.PdvRTipEnum.OBRT_NOT_PDV  : rbt_obrtNotPdv  .Checked = true; break;
            case ZXC.PdvRTipEnum.NOT_IN_PDV    : rbt_notPdv      .Checked = true; break;
            case ZXC.PdvRTipEnum.POD_PO_NAPL   : rbt_podPoNapl   .Checked = true; break;
            case ZXC.PdvRTipEnum.FIZICKA_OSOBA : rbt_fizickaOsoba.Checked = true; break;
         }
      }
   }

   /*67*/
   public bool Fld_IsFrgn
   {
      get { return cbxIsFrgn.Checked; }
      set {        cbxIsFrgn.Checked = value; }
   }
   /*68*/
   public bool Fld_IsPdv
   {
      get { return cbxIsPdv.Checked; }
      set { cbxIsPdv.Checked = value; }
   }

   // razlika u brojevima o odnosu na business
   ///* 46 */ internal DateTime _date     ;
   ///* 49 */ internal string   _fuse1    ;    
   ///* 50 */ internal string   _fuse2    ;    
   ///* 57 */ internal bool     _isXxx    ;    
   ///* 58 */ internal bool     _isYyy    ;    
   ///* 59 */ internal bool     _isZzz    ;    


   /*75*/
   public bool Fld_IsMtr
   {
      get { return cbxIsMTros.Checked; }
      set {        cbxIsMTros.Checked = value; }
   }
   /*76*/
   public bool Fld_IsKupac
   {
      get { return cbxIsKup.Checked; }
      set {        cbxIsKup.Checked = value; }
   }
   /*77*/
   public bool Fld_IsDobav
   {
      get { return cbxIsDob.Checked; }
      set {        cbxIsDob.Checked = value; }
   }
   /*78*/
   public bool Fld_IsBanka
   {
      get { return cbxIsBanka.Checked; }
      set {        cbxIsBanka.Checked = value; }
   }

   /*79*/   public string Fld_CentrTick
   {
      get { return tbxCentrTick.Text; }
      set { tbxCentrTick.Text = value; }
   }

   /*80*/ public string Fld_OIB
   {
      get { return tbxOIB.Text; }
      set {        tbxOIB.Text = value; }
   }
   /* 81 */ public string   Fld_Drzava  
   {
      get { return tbxDrzava.Text; }
      set {        tbxDrzava.Text = value; }
   }  
   /* 82 */ public string   Fld_Swift   
   {
      get { return tbxSwift.Text; }
      set {        tbxSwift.Text = value; }
   }  
   /* 83 */ public string   Fld_Iban    
   {
      get { return tbxIban.Text; }
      set {        tbxIban.Text = value; }
   }  
   /* 84 */ public string   Fld_DevName 
   {
      get { return tbxDevName.Text; }
      set {        tbxDevName.Text = value; }
   }  
   /* 85 */ public decimal  Fld_FinLimit
   {
      get { return tbxfinLimit.GetDecimalField(); }
      set {        tbxfinLimit.PutDecimalField(value); }
   }  


   public bool Fld_IsOsoba
   {
      get { return cbxIsOsoba.Checked; }
      set {        cbxIsOsoba.Checked = value; }
   }

   public string Fld_UgovorNo
   {
      get { return tbx_ugovorNo.Text; }
      set {        tbx_ugovorNo.Text = value; }
   }

   public ZXC.KomisijaKindEnum Fld_Komisija
   {
      get
      {
              if(rbt_komisNE.Checked) return ZXC.KomisijaKindEnum.NIJE;
         else if(rbt_komisMP.Checked) return ZXC.KomisijaKindEnum.MALOPRODAJNA;
         else if(rbt_komisVP.Checked) return ZXC.KomisijaKindEnum.VELEPRODAJNA;
              else throw new Exception("Fld_Komisija: who df is checked?")    ;
      }
      set
      {
         switch(value)
         {
            case ZXC.KomisijaKindEnum.NIJE        : rbt_komisNE.Checked = true; break;
            case ZXC.KomisijaKindEnum.MALOPRODAJNA: rbt_komisMP.Checked = true; break;
            case ZXC.KomisijaKindEnum.VELEPRODAJNA: rbt_komisVP.Checked = true; break;
         }
      }
   }

   //public string Fld_KomisSkladCD    { get { return tbx_komisSkladCD   .Text; } set { tbx_komisSkladCD   .Text = value; } }
   //public string Fld_KomisSkladNaziv { get { return tbx_komisSkladNaziv.Text; } set { tbx_komisSkladNaziv.Text = value; } }
   //public string Fld_KomisSkladKonto { get { return tbx_komisSkladKonto.Text; } set { tbx_komisSkladKonto.Text = value; } }
   //public string Fld_KomisSkladMalop { get { return tbx_komisSkladMalop.Text; } set { tbx_komisSkladMalop.Text = value; } }
   //public uint   Fld_KomisSkladNum   { get { return ZXC.ValOrZero_UInt(tbx_komisSkladBroj.Text); } set { tbx_komisSkladBroj.Text = value.ToString(); } }

   public string Fld_VatCntryCode { get { return tbx_vatCntryCode.Text; } set { tbx_vatCntryCode.Text = value; } }

   public decimal Fld_MitoIzn  { get { return tbx_mitoIzn .GetDecimalField(); } set { tbx_mitoIzn .PutDecimalField(value); } }
   public decimal Fld_MitoSt   { get { return tbx_mitoSt  .GetDecimalField(); } set { tbx_mitoSt  .PutDecimalField(value); } }
   public decimal Fld_InvestTr { get { return tbx_investTr.GetDecimalField(); } set { tbx_investTr.PutDecimalField(value); } }
   public decimal Fld_TrecaStr { get { return tbx_trecaStr.GetDecimalField(); } set { tbx_trecaStr.PutDecimalField(value); } }

   public TimeSpan Fld_TimeOd_1 { get { return ZXC.ValOr_0000_DtpForTimeOnly(dtp_timeOd_1.Value); } set { dtp_timeOd_1.Value = ZXC.ValOr_01011753_DateTime(value); tbx_timeOd_1.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_timeOd_1); } }
   public TimeSpan Fld_TimeDo_1 { get { return ZXC.ValOr_0000_DtpForTimeOnly(dtp_timeDo_1.Value); } set { dtp_timeDo_1.Value = ZXC.ValOr_01011753_DateTime(value); tbx_timeDo_1.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_timeDo_1); } }
   public TimeSpan Fld_TimeOd_2 { get { return ZXC.ValOr_0000_DtpForTimeOnly(dtp_timeOd_2.Value); } set { dtp_timeOd_2.Value = ZXC.ValOr_01011753_DateTime(value); tbx_timeOd_2.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_timeOd_2); } }
   public TimeSpan Fld_TimeDo_2 { get { return ZXC.ValOr_0000_DtpForTimeOnly(dtp_timeDo_2.Value); } set { dtp_timeDo_2.Value = ZXC.ValOr_01011753_DateTime(value); tbx_timeDo_2.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_timeDo_2); } }
   public TimeSpan Fld_TimeOd_3 { get { return ZXC.ValOr_0000_DtpForTimeOnly(dtp_timeOd_3.Value); } set { dtp_timeOd_3.Value = ZXC.ValOr_01011753_DateTime(value); tbx_timeOd_3.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_timeOd_3); } }
   public TimeSpan Fld_TimeDo_3 { get { return ZXC.ValOr_0000_DtpForTimeOnly(dtp_timeDo_3.Value); } set { dtp_timeDo_3.Value = ZXC.ValOr_01011753_DateTime(value); tbx_timeDo_3.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_timeDo_3); } }
   public TimeSpan Fld_TimeOd_4 { get { return ZXC.ValOr_0000_DtpForTimeOnly(dtp_timeOd_4.Value); } set { dtp_timeOd_4.Value = ZXC.ValOr_01011753_DateTime(value); tbx_timeOd_4.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_timeOd_4); } }
   public TimeSpan Fld_TimeDo_4 { get { return ZXC.ValOr_0000_DtpForTimeOnly(dtp_timeDo_4.Value); } set { dtp_timeDo_4.Value = ZXC.ValOr_01011753_DateTime(value); tbx_timeDo_4.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_timeDo_4); } }
   public TimeSpan Fld_TimeOd_5 { get { return ZXC.ValOr_0000_DtpForTimeOnly(dtp_timeOd_5.Value); } set { dtp_timeOd_5.Value = ZXC.ValOr_01011753_DateTime(value); tbx_timeOd_5.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_timeOd_5); } }
   public TimeSpan Fld_TimeDo_5 { get { return ZXC.ValOr_0000_DtpForTimeOnly(dtp_timeDo_5.Value); } set { dtp_timeDo_5.Value = ZXC.ValOr_01011753_DateTime(value); tbx_timeDo_5.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_timeDo_5); } }
   public TimeSpan Fld_TimeOd_6 { get { return ZXC.ValOr_0000_DtpForTimeOnly(dtp_timeOd_6.Value); } set { dtp_timeOd_6.Value = ZXC.ValOr_01011753_DateTime(value); tbx_timeOd_6.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_timeOd_6); } }
   public TimeSpan Fld_TimeDo_6 { get { return ZXC.ValOr_0000_DtpForTimeOnly(dtp_timeDo_6.Value); } set { dtp_timeDo_6.Value = ZXC.ValOr_01011753_DateTime(value); tbx_timeDo_6.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_timeDo_6); } }
   public TimeSpan Fld_TimeOd_7 { get { return ZXC.ValOr_0000_DtpForTimeOnly(dtp_timeOd_7.Value); } set { dtp_timeOd_7.Value = ZXC.ValOr_01011753_DateTime(value); tbx_timeOd_7.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_timeOd_7); } }
   public TimeSpan Fld_TimeDo_7 { get { return ZXC.ValOr_0000_DtpForTimeOnly(dtp_timeDo_7.Value); } set { dtp_timeDo_7.Value = ZXC.ValOr_01011753_DateTime(value); tbx_timeDo_7.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_timeDo_7); } }

   public bool   Fld_PTG_IsPojedinFak        { get { return cbxPTG_isPojedinacnoFak.Checked; } set { cbxPTG_isPojedinacnoFak.Checked = value; } }
   public string Fld_PTG_DanFakturiranja     { get { return tbx_PTG_DanZaFak       .Text;    } set { tbx_PTG_DanZaFak       .Text    = value; } }
   public string Fld_PTG_DanFakturiranjaOpis {                                                 set { tbx_PTG_DanZaFakOpis   .Text = value; } }
   public ZXC.PTG_DanFakturiranjaEnum PTG_DanFakturiranjaEnum
   {
      get
      {
         if(Fld_PTG_DanFakturiranja.IsEmpty()) return ZXC.PTG_DanFakturiranjaEnum.PrviDanMjeseca;
         else                              return (ZXC.PTG_DanFakturiranjaEnum)Enum.Parse(typeof(ZXC.PTG_DanFakturiranjaEnum), Fld_PTG_DanFakturiranja, true);
      }
   }

   public string Fld_PTG_SlanjeRacuna { get { return tbx_PTG_SlanjeRacuna   .Text;    } set { tbx_PTG_SlanjeRacuna   .Text    = value; } }

   public uint   Fld_PTG_ZaduzenCd      { get { return tbx_PTG_ZaduzenCd.GetSomeRecIDField(); } set { tbx_PTG_ZaduzenCd.PutSomeRecIDField(value); } }
   public string Fld_PTG_ZaduzenIdAsTxt { get { return tbx_PTG_ZaduzenCd  .Text             ; } set { tbx_PTG_ZaduzenCd.Text = value;             } }
   public string Fld_PTG_ZaduzenName    { get { return tbx_PTG_ZaduzenName.Text             ; } set { tbx_PTG_ZaduzenName.Text = value;           } }

   public bool     Fld_IdIsPolStmnt      { get { return cbx_hasPolIzjava.Checked; } set { cbx_hasPolIzjava.Checked = value; }}
 //public DateTime Fld_IdBirthDate      { get { return dtp_dateBirth     .Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_dateBirth     .Value = value; } } }
   public DateTime Fld_IdBirthDate
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_dateBirth.Value);
      }
      set
      {
         dtp_dateBirth.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_dateBirth.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_dateBirth);
      }
   }


 //public DateTime Fld_IdExpDate { get { return dtp_dateVazenjaIdI.Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_dateVazenjaIdI.Value = value; } } }
   public DateTime Fld_IdExpDate
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_dateVazenjaIdI.Value);
      }
      set
      {
         dtp_dateVazenjaIdI.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_dateVazenjaIdI.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_dateVazenjaIdI);
      }
   }


   public string   Fld_IdNumber      { get { return tbx_brojIdI       .Text;  } set { tbx_brojIdI      .Text    = value; } }
   public string   Fld_IdIssuer      { get { return tbx_izdavIdI      .Text;  } set { tbx_izdavIdI     .Text    = value; } }
   public string   Fld_IdCitizenshp  { get { return tbx_drzavljanstvo .Text;  } set { tbx_drzavljanstvo.Text    = value; } }
   
   public ZXC.F2_R1enum Fld_R1Kind
   {
      get
      {
              if(rbt_R1nepoznato.Checked) return ZXC.F2_R1enum.Nepoznato  ;
         else if(rbt_B2B        .Checked) return ZXC.F2_R1enum.B2B        ;
         else if(rbt_B2C        .Checked) return ZXC.F2_R1enum.B2C        ;
              else throw new Exception("Fld_AMSstatus: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.F2_R1enum.Nepoznato : rbt_R1nepoznato.Checked = true; break;
            case ZXC.F2_R1enum.B2B       : rbt_B2B        .Checked = true; break;
            case ZXC.F2_R1enum.B2C       : rbt_B2C        .Checked = true; break;
         }
      }
   }

   public string Fld_KontoPrihod { get { return tbxKontoP.Text; }  set { tbxKontoP.Text = value; }   }
   public string Fld_KontoTrosak { get { return tbxKontoT.Text; }  set { tbxKontoT.Text = value; }   }

   #endregion Fld_XY

   #region Classic PutFileds(), GetFields()

   public override void PutFields(VvDataRecord kupdob)
   {
      kupdob_rec = (Kupdob)kupdob;
      Kupdob kupdobSifrar_rec;
      
      if (kupdob_rec != null)
      {
         //Fld_RecID = TheVvTabPage.IsArhivaTabPage ? artikl_rec.OrigRecID : artikl_rec.RecID;

         PutMetaFileds(kupdob_rec.AddUID, kupdob_rec.AddTS, kupdob_rec.ModUID, kupdob_rec.ModTS, kupdob_rec.RecID, kupdob_rec.LanSrvID, kupdob_rec.LanRecID);

         PutIdentityFields(kupdob_rec.KupdobCD/*RecID*/.ToString("000000"), kupdob_rec.Ticker, kupdob_rec.Naziv, "");

         VvHamper.SetChkBoxRadBttnAutoCheck(this, true);

  /*05*/   Fld_Naziv     = kupdob_rec.Naziv;
  /*06*/   Fld_Ticker    = kupdob_rec.Ticker;
  /*07*/   Fld_KupdobCD  = kupdob_rec.KupdobCD;
  /*08*/   Fld_Matbr     = kupdob_rec.Matbr;
  /*09*/   Fld_Tip       = kupdob_rec.Tip;
  /*10*/   Fld_DugoIme   = kupdob_rec.DugoIme;
  /*11*/   Fld_AdrSjed   = kupdob_rec.Ulica1;
  /*12*/   Fld_AdrFakt   = kupdob_rec.Ulica2;
  /*13*/   Fld_Grad      = kupdob_rec.Grad;
  /*14*/   Fld_PostaBr   = kupdob_rec.PostaBr;
  /*15*/   Fld_Opcina    = kupdob_rec.Opcina;
  /*16*/   Fld_OpcCd     = kupdob_rec.OpcCd;
  /*17*/   Fld_Zupan     = kupdob_rec.Zupan;
  /*18*/   Fld_ZupCd     = kupdob_rec.ZupCd;
  /*19*/   Fld_Ime       = kupdob_rec.Ime;
  /*20*/   Fld_Prezime   = kupdob_rec.Prezime;
  /*21*/   Fld_Tel1      = kupdob_rec.Tel1;
  /*22*/   Fld_Tel2      = kupdob_rec.Tel2;
  /*23*/   Fld_Fax       = kupdob_rec.Fax;
  /*24*/   Fld_Gsm       = kupdob_rec.Gsm;
  /*25*/   Fld_Email     = kupdob_rec.Email;
  /*26*/   Fld_URL       = kupdob_rec.Url;
  /*27*/   Fld_Ziro1     = kupdob_rec.Ziro1;
  /*28*/   Fld_Ziro1By   = kupdob_rec.Ziro1By;
  /*29*/   Fld_Ziro1PnbM = kupdob_rec.Ziro1PnbM;
  /*30*/   Fld_Ziro1PnbV = kupdob_rec.Ziro1PnbV;
  /*31*/   Fld_Ziro2     = kupdob_rec.Ziro2;
  /*32*/   Fld_Ziro2By   = kupdob_rec.Ziro2By;
  /*33*/   Fld_Ziro2PnbM = kupdob_rec.Ziro2PnbM;
  /*34*/   Fld_Ziro2PnbV = kupdob_rec.Ziro2PnbV;
  /*35*/   Fld_Ziro3     = kupdob_rec.Ziro3;
  /*36*/   Fld_Ziro3By   = kupdob_rec.Ziro3By;
  /*37*/   Fld_Ziro3PnbM = kupdob_rec.Ziro3PnbM;
  /*38*/   Fld_Ziro3PnbV = kupdob_rec.Ziro3PnbV;
  /*39*/   Fld_Ziro4     = kupdob_rec.Ziro4;
  /*40*/   Fld_Ziro4By   = kupdob_rec.Ziro4By;
  /*41*/   Fld_Ziro4PnbM = kupdob_rec.Ziro4PnbM;
  /*42*/   Fld_Ziro4PnbV = kupdob_rec.Ziro4PnbV;
  /*43*/   Fld_KontoD    = kupdob_rec.KontoDug;
  /*44*/   Fld_Regob     = kupdob_rec.Regob;
  /*45*/   Fld_SifDcd    = kupdob_rec.SifDcd;
  /*46*/   Fld_SifDname  = kupdob_rec.SifDname;
      /* 47 */
  /*48*/   Fld_PutnikID  = kupdob_rec.PutnikID;
  /*49*/   Fld_PutName   = kupdob_rec.PutName;
      /* 50 */  
      /* 51 */  
  /*52*/   Fld_Napom1    = kupdob_rec.Napom1;
  /*53*/   Fld_Napom2    = kupdob_rec.Napom2;
  /*54*/   Fld_Komentar  = kupdob_rec.Komentar;
  /*55*/   Fld_IsObrt    = kupdob_rec./*IsObrt*/PdvRTip;
  /*55*/   Fld_IsFrgn    = kupdob_rec.IsFrgn;
  /*57*/   Fld_IsPdv     = kupdob_rec.IsPdv;
      /* 58 */  
      /* 59 */  
  /*60*/   Fld_IsOsoba   = kupdob_rec.IsZzz;  
  /*61*/   Fld_StRbt1    = kupdob_rec.StRbt1;
  /*62*/   Fld_StRbt2    = kupdob_rec.StRbt2;
  /*63*/   Fld_StSRbt    = kupdob_rec.StSRbt;
  /*64*/   Fld_StCsSc    = kupdob_rec.StCsSc;
  /*65*/   Fld_StProviz  = kupdob_rec.StProviz;
  /*66*/   Fld_PnbMProv  = kupdob_rec.PnbMProv;
  /*67*/   Fld_PnbVProv  = kupdob_rec.PnbVProv;
  /*68*/   Fld_PnbMPlaca = kupdob_rec.PnbMPlaca;
  /*69*/   Fld_PnbVPlaca = kupdob_rec.PnbVPlaca;
  /*70*/   Fld_ValutaPl  = kupdob_rec.ValutaPl;
  /*71*/   Fld_RokOtprm  = kupdob_rec.RokOtprm;
  /*72*/   Fld_IsCentr   = kupdob_rec.IsCentr;
  /*73*/   Fld_CentrID   = kupdob_rec.CentrID;
  /*74*/   Fld_CentrTick = kupdob_rec.CentrTick;
  /*75*/   Fld_KontoK    = kupdob_rec.KontoPot;
  /*76*/   Fld_IsMtr     = kupdob_rec.IsMtr;
  /*77*/   Fld_IsKupac   = kupdob_rec.IsKupac;
  /*78*/   Fld_IsDobav   = kupdob_rec.IsDobav;
  /*79*/   Fld_IsBanka   = kupdob_rec.IsBanka;
  /*80*/   Fld_OIB       = kupdob_rec.Oib;
  /*81*/   Fld_Drzava    = kupdob_rec.Drzava;   
  /*82*/   Fld_Swift     = kupdob_rec.Swift;  
  /*83*/   Fld_Iban      = kupdob_rec.Iban;  
///*84*/   Fld_DevName   = kupdob_rec.DevName;  
  /*84*/   Fld_DevName   = kupdob_rec.DevName_OLD;  
  /*85*/   Fld_FinLimit  = kupdob_rec.FinLimit;  
  /*86*/   Fld_UgovorNo  = kupdob_rec.UgovorNo;  

  /*87*/   Fld_Komisija         = kupdob_rec.Komisija;  
  ///*88*/   Fld_KomisSkladKonto  = kupdob_rec.SklKonto;  
  ///*89*/   Fld_KomisSkladNum    = kupdob_rec.SklNum;

  /*90*/   Fld_VatCntryCode     = kupdob_rec.VatCntryCode;

  //if(kupdob_rec.Komisija != ZXC.KomisijaKindEnum.NIJE)
  //{
  //   Fld_KomisSkladCD    = kupdob_rec.Ticker;
  //   Fld_KomisSkladNaziv = kupdob_rec.Naziv;
  //   Fld_KomisSkladMalop = kupdob_rec.IsKomisMalopSkl ? "X" : "";
  //}
  //else
  //{
  //   Fld_KomisSkladCD    = 
  //   Fld_KomisSkladNaziv = 
  //   Fld_KomisSkladMalop = "";
  //}

  /*91*/  Fld_MitoIzn  = kupdob_rec.MitoIzn  ;
  /*92*/  Fld_MitoSt   = kupdob_rec.MitoSt   ;
  /*93*/  Fld_InvestTr = kupdob_rec.InvestTr ;
  /*94*/  Fld_TrecaStr = kupdob_rec.TrecaStr ;

  /* 95 */  Fld_TimeOd_1 = /*Convert.ToDateTime(*/kupdob_rec.TimeOd_1/*.ToString())*/;
  /* 96 */  Fld_TimeDo_1 = /*Convert.ToDateTime(*/kupdob_rec.TimeDo_1/*.ToString())*/;
  /* 97 */  Fld_TimeOd_2 = /*Convert.ToDateTime(*/kupdob_rec.TimeOd_2/*.ToString())*/;
  /* 98 */  Fld_TimeDo_2 = /*Convert.ToDateTime(*/kupdob_rec.TimeDo_2/*.ToString())*/;
  /* 99 */  Fld_TimeOd_3 = /*Convert.ToDateTime(*/kupdob_rec.TimeOd_3/*.ToString())*/;
  /*100 */  Fld_TimeDo_3 = /*Convert.ToDateTime(*/kupdob_rec.TimeDo_3/*.ToString())*/;
  /*101 */  Fld_TimeOd_4 = /*Convert.ToDateTime(*/kupdob_rec.TimeOd_4/*.ToString())*/;
  /*102 */  Fld_TimeDo_4 = /*Convert.ToDateTime(*/kupdob_rec.TimeDo_4/*.ToString())*/;
  /*103 */  Fld_TimeOd_5 = /*Convert.ToDateTime(*/kupdob_rec.TimeOd_5/*.ToString())*/;
  /*104 */  Fld_TimeDo_5 = /*Convert.ToDateTime(*/kupdob_rec.TimeDo_5/*.ToString())*/;
  /*105 */  Fld_TimeOd_6 = /*Convert.ToDateTime(*/kupdob_rec.TimeOd_6/*.ToString())*/;
  /*106 */  Fld_TimeDo_6 = /*Convert.ToDateTime(*/kupdob_rec.TimeDo_6/*.ToString())*/;
  /*107 */  Fld_TimeOd_7 = /*Convert.ToDateTime(*/kupdob_rec.TimeOd_7/*.ToString())*/;
  /*108 */  Fld_TimeDo_7 = /*Convert.ToDateTime(*/kupdob_rec.TimeDo_7/*.ToString())*/;

         //===================== 

  /*109 */ //Fld_IsAMS     = kupdob_rec.IsAMS    ;
  /*109 */   Fld_R1Kind = kupdob_rec.R1kind;

         if(ZXC.IsTETRAGRAM_ANY)
         { 
            /*110 */ Fld_IdIsPolStmnt  = kupdob_rec.IdIsPolStmnt;
            /*111 */ Fld_IdBirthDate   = kupdob_rec.IdBirthDate ;
            /*112 */ Fld_IdExpDate     = kupdob_rec.IdExpDate   ;
            /*113 */ Fld_IdNumber      = kupdob_rec.IdNumber    ;
            /*114 */ Fld_IdIssuer      = kupdob_rec.IdIssuer    ;
            /*115 */ Fld_IdCitizenshp  = kupdob_rec.IdCitizenshp;
         }

  /*116 */  Fld_KontoPrihod = kupdob_rec.KontoPrihod;
  /*117 */  Fld_KontoTrosak = kupdob_rec.KontoTrosak;


         #region PCTOGO

         if(ZXC.IsPCTOGO)
         {
            Fld_PTG_IsPojedinFak        = kupdob_rec.IsYyy;
            Fld_PTG_DanFakturiranja     = kupdob_rec.Fuse1;
            Fld_PTG_DanFakturiranjaOpis = ZXC.luiListaPTG_DanZaFaktur.GetNameForThisCd(Fld_PTG_DanFakturiranja);
            Fld_PTG_SlanjeRacuna        = kupdob_rec.Fuse2;
            Fld_PTG_ZaduzenCd           = kupdob_rec.PutnikID;
            Fld_PTG_ZaduzenName         = kupdob_rec.PutName;


         }

         #endregion PCTOGO


         if(ovoJeKupdob_A_NE_Prjkt && ZXC.ThisIsVektorProject)
         {
            SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.None);

            //kupdobSifrar_rec = KupdobSifrar.FindByRecID(artikl_rec.CentrID);
            kupdobSifrar_rec = KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD/*VirtualRecID*/ == kupdob_rec.CentrID);
            if(kupdobSifrar_rec != null) Fld_CentralName = kupdobSifrar_rec.Naziv;
            else                         Fld_CentralName = "";

            InitializeFilterUCFields();

            recordReportLoaded = false;
            DecideIfShouldLoad_VvReport(null, null, null);

            aTransesLoaded[0] = false; // ovdje treba nulirati sve postojece 'xyLoaded' varijable
            DecideIfShouldLoad_TransDGV(null, null, null);

            KDC_Tab_Loaded = false;

            if(TheTabControl.SelectedTab.Name == KDC_TabPageName)
            {
               GetKDClist_AND_PutKDC_Tab_OR_Osnovno_DGV_Fields(kupdob_rec, aTransesGrid[1], KDC_Tab_Loaded);
            }

            //DecideIfShouldLoad_KDC_DGV(null, null, null);

            KDC_Osnovno_Loaded = false;

            if(TheTabControl.SelectedTab.Name == "Osnovno")
            {
               GetKDClist_AND_PutKDC_Tab_OR_Osnovno_DGV_Fields(kupdob_rec, kdcGrid, KDC_Osnovno_Loaded);
               kdcGrid.ClearSelection();

            }
         }

         //===================== 

      }

      VvHamper.SetChkBoxRadBttnAutoCheck(this, false);

   }

   public override void GetFields(bool fuse)
   {
      if(kupdob_rec == null) kupdob_rec = new Kupdob();

      /*05*/    kupdob_rec.Naziv     = Fld_Naziv;
      /*06*/    kupdob_rec.Ticker    = Fld_Ticker;
      /*07*/    kupdob_rec.KupdobCD  = Fld_KupdobCD;
      /*08*/    kupdob_rec.Matbr     = Fld_Matbr;
      /*09*/    kupdob_rec.Tip       = Fld_Tip;
      /*10*/    kupdob_rec.DugoIme   = Fld_DugoIme;
      /*11*/    kupdob_rec.Ulica1    = Fld_AdrSjed;
      /*12*/    kupdob_rec.Ulica2    = Fld_AdrFakt;
      /*13*/    kupdob_rec.Grad      = Fld_Grad;
      /*14*/    kupdob_rec.PostaBr   = Fld_PostaBr;
      /*15*/    kupdob_rec.Opcina    = Fld_Opcina;
      /*16*/    kupdob_rec.OpcCd     = Fld_OpcCd;
      /*17*/    kupdob_rec.Zupan     = Fld_Zupan;
      /*18*/    kupdob_rec.ZupCd     = Fld_ZupCd;
      /*19*/    kupdob_rec.Ime       = Fld_Ime;
      /*20*/    kupdob_rec.Prezime   = Fld_Prezime;
      /*21*/    kupdob_rec.Tel1      = Fld_Tel1;
      /*22*/    kupdob_rec.Tel2      = Fld_Tel2;
      /*23*/    kupdob_rec.Fax       = Fld_Fax;
      /*24*/    kupdob_rec.Gsm       = Fld_Gsm;
      /*25*/    kupdob_rec.Email     = Fld_Email;
      /*26*/    kupdob_rec.Url       = Fld_URL;
     
      /*27*/    kupdob_rec.Ziro1     = Fld_Ziro1;
//      /*27*/    kupdob_rec.Ziro1     = ZXC.GetIBANfromOldZiro(Fld_Ziro1);
      
      /*28*/    kupdob_rec.Ziro1By   = Fld_Ziro1By;
      /*29*/    kupdob_rec.Ziro1PnbM = Fld_Ziro1PnbM;
      /*30*/    kupdob_rec.Ziro1PnbV = Fld_Ziro1PnbV;
      /*31*/    kupdob_rec.Ziro2     = Fld_Ziro2;
      /*32*/    kupdob_rec.Ziro2By   = Fld_Ziro2By;
      /*33*/    kupdob_rec.Ziro2PnbM = Fld_Ziro2PnbM;
      /*34*/    kupdob_rec.Ziro2PnbV = Fld_Ziro2PnbV;
      /*35*/    kupdob_rec.Ziro3     = Fld_Ziro3;
      /*36*/    kupdob_rec.Ziro3By   = Fld_Ziro3By;
      /*37*/    kupdob_rec.Ziro3PnbM = Fld_Ziro3PnbM;
      /*38*/    kupdob_rec.Ziro3PnbV = Fld_Ziro3PnbV;
      /*39*/    kupdob_rec.Ziro4     = Fld_Ziro4;
      /*40*/    kupdob_rec.Ziro4By   = Fld_Ziro4By;
      /*41*/    kupdob_rec.Ziro4PnbM = Fld_Ziro4PnbM;
      /*42*/    kupdob_rec.Ziro4PnbV = Fld_Ziro4PnbV;
      /*43*/    kupdob_rec.KontoDug  = Fld_KontoD; 
      /*44*/    kupdob_rec.Regob     = Fld_Regob;
      /*45*/    kupdob_rec.SifDcd    = Fld_SifDcd;
      /*46*/    kupdob_rec.SifDname  = Fld_SifDname;
         /*47*/  
      /*48*/    kupdob_rec.PutnikID  = Fld_PutnikID;
      /*49*/    kupdob_rec.PutName   = Fld_PutName;
         /* 50 */
         /* 51 */
      /*52*/    kupdob_rec.Napom1    = Fld_Napom1;
      /*53*/    kupdob_rec.Napom2    = Fld_Napom2;
      /*54*/    kupdob_rec.Komentar  = Fld_Komentar;
      /*55*/    kupdob_rec./*IsObrt*/PdvRTip    = Fld_IsObrt;
      /*55*/    kupdob_rec.IsFrgn    = Fld_IsFrgn;
      /*57*/    kupdob_rec.IsPdv     = Fld_IsPdv;
         /* 58 */ 
         /* 59 */ 
      /*60*/    kupdob_rec.IsZzz     = Fld_IsOsoba;  
      
      /*61*/    kupdob_rec.StRbt1       = Fld_StRbt1;
      /*62*/    kupdob_rec.StRbt2       = Fld_StRbt2;
      /*63*/    kupdob_rec.StSRbt       = Fld_StSRbt;
      /*64*/    kupdob_rec.StCsSc       = Fld_StCsSc;
      /*65*/    kupdob_rec.StProviz     = Fld_StProviz;
      /*66*/    kupdob_rec.PnbMProv     = Fld_PnbMProv;
      /*67*/    kupdob_rec.PnbVProv     = Fld_PnbVProv;
      /*68*/    kupdob_rec.PnbMPlaca    = Fld_PnbMPlaca;
      /*69*/    kupdob_rec.PnbVPlaca    = Fld_PnbVPlaca;
      /*70*/    kupdob_rec.ValutaPl     = Fld_ValutaPl;
      /*71*/    kupdob_rec.RokOtprm     = Fld_RokOtprm;
      /*72*/    kupdob_rec.IsCentr      = Fld_IsCentr;
      /*73*/    kupdob_rec.CentrID      = Fld_CentrID;
      /*74*/    kupdob_rec.CentrTick    = Fld_CentrTick;
      /*75*/    kupdob_rec.KontoPot     = Fld_KontoK; 
      /*76*/    kupdob_rec.IsMtr        = Fld_IsMtr;
      /*77*/    kupdob_rec.IsKupac      = Fld_IsKupac;
      /*78*/    kupdob_rec.IsDobav      = Fld_IsDobav;
      /*79*/    kupdob_rec.IsBanka      = Fld_IsBanka;
      /*80*/    kupdob_rec.Oib          = Fld_OIB;
      /*81*/    kupdob_rec.Drzava       = Fld_Drzava;   
      /*82*/    kupdob_rec.Swift        = Fld_Swift;  
      /*83*/    kupdob_rec.Iban         = Fld_Iban;  
      /*84*/    kupdob_rec.DevName      = Fld_DevName;  
      /*85*/    kupdob_rec.FinLimit     = Fld_FinLimit;  
      /*86*/    kupdob_rec.UgovorNo     = Fld_UgovorNo;  
      /*87*/    kupdob_rec.Komisija     = Fld_Komisija       ;  
////  /*88*/    kupdob_rec.SklKonto     = Fld_KomisSkladKonto;  
////  /*89*/    kupdob_rec.SklNum       = Fld_KomisSkladNum  ;
      /*90*/    kupdob_rec.VatCntryCode = Fld_VatCntryCode ;
      /*91*/    kupdob_rec.MitoIzn      = Fld_MitoIzn   ;
      /*92*/    kupdob_rec.MitoSt       = Fld_MitoSt    ;
      /*93*/    kupdob_rec.InvestTr     = Fld_InvestTr  ;
      /*94*/    kupdob_rec.TrecaStr     = Fld_TrecaStr  ;
     /* 95*/    kupdob_rec.TimeOd_1     = /*TimeSpan.Parse(*/Fld_TimeOd_1/*.Hour.ToString() + ":" + Fld_TimeOd_1.Minute.ToString())*/; 
     /* 96*/    kupdob_rec.TimeDo_1     = /*TimeSpan.Parse(*/Fld_TimeDo_1/*.Hour.ToString() + ":" + Fld_TimeDo_1.Minute.ToString())*/; 
     /* 97*/    kupdob_rec.TimeOd_2     = /*TimeSpan.Parse(*/Fld_TimeOd_2/*.Hour.ToString() + ":" + Fld_TimeOd_2.Minute.ToString())*/; 
     /* 98*/    kupdob_rec.TimeDo_2     = /*TimeSpan.Parse(*/Fld_TimeDo_2/*.Hour.ToString() + ":" + Fld_TimeDo_2.Minute.ToString())*/; 
     /* 99*/    kupdob_rec.TimeOd_3     = /*TimeSpan.Parse(*/Fld_TimeOd_3/*.Hour.ToString() + ":" + Fld_TimeOd_3.Minute.ToString())*/; 
     /*100*/    kupdob_rec.TimeDo_3     = /*TimeSpan.Parse(*/Fld_TimeDo_3/*.Hour.ToString() + ":" + Fld_TimeDo_3.Minute.ToString())*/; 
     /*101*/    kupdob_rec.TimeOd_4     = /*TimeSpan.Parse(*/Fld_TimeOd_4/*.Hour.ToString() + ":" + Fld_TimeOd_4.Minute.ToString())*/; 
     /*102*/    kupdob_rec.TimeDo_4     = /*TimeSpan.Parse(*/Fld_TimeDo_4/*.Hour.ToString() + ":" + Fld_TimeDo_4.Minute.ToString())*/; 
     /*103*/    kupdob_rec.TimeOd_5     = /*TimeSpan.Parse(*/Fld_TimeOd_5/*.Hour.ToString() + ":" + Fld_TimeOd_5.Minute.ToString())*/; 
     /*104*/    kupdob_rec.TimeDo_5     = /*TimeSpan.Parse(*/Fld_TimeDo_5/*.Hour.ToString() + ":" + Fld_TimeDo_5.Minute.ToString())*/; 
     /*105*/    kupdob_rec.TimeOd_6     = /*TimeSpan.Parse(*/Fld_TimeOd_6/*.Hour.ToString() + ":" + Fld_TimeOd_6.Minute.ToString())*/; 
     /*106*/    kupdob_rec.TimeDo_6     = /*TimeSpan.Parse(*/Fld_TimeDo_6/*.Hour.ToString() + ":" + Fld_TimeDo_6.Minute.ToString())*/; 
     /*107*/    kupdob_rec.TimeOd_7     = /*TimeSpan.Parse(*/Fld_TimeOd_7/*.Hour.ToString() + ":" + Fld_TimeOd_7.Minute.ToString())*/; 
     /*108*/    kupdob_rec.TimeDo_7     = /*TimeSpan.Parse(*/Fld_TimeDo_7/*.Hour.ToString() + ":" + Fld_TimeDo_7.Minute.ToString())*/;

     /*109 */ //kupdob_rec.IsAMS        = Fld_IsAMS       ;
     /*109 */   kupdob_rec.R1kind       = Fld_R1Kind   ;

      if(ZXC.IsTETRAGRAM_ANY)
      { 
         /*110 */   kupdob_rec.IdIsPolStmnt = Fld_IdIsPolStmnt;
         /*111 */   kupdob_rec.IdBirthDate  = Fld_IdBirthDate ;
         /*112 */   kupdob_rec.IdExpDate    = Fld_IdExpDate   ;
         /*113 */   kupdob_rec.IdNumber     = Fld_IdNumber    ;
         /*114 */   kupdob_rec.IdIssuer     = Fld_IdIssuer    ;
         /*115 */   kupdob_rec.IdCitizenshp = Fld_IdCitizenshp;
      }

     /*116 */    kupdob_rec.KontoPrihod = Fld_KontoPrihod;
     /*117 */    kupdob_rec.KontoTrosak = Fld_KontoTrosak;


      #region PCTOGO

      if(ZXC.IsPCTOGO)
      {
         kupdob_rec.IsYyy     = Fld_PTG_IsPojedinFak;
         kupdob_rec.Fuse1     = Fld_PTG_DanFakturiranja;
         kupdob_rec.Fuse2     = Fld_PTG_SlanjeRacuna;
         kupdob_rec.PutnikID  = Fld_PTG_ZaduzenCd;
         kupdob_rec.PutName   = Fld_PTG_ZaduzenName;

      }
      
      #endregion PCTOGO
   }

   #endregion Classic PutFileds(), GetFields()

   #region Put Trans DGV Fileds

   private const string ftrans_TabPageName    = "Fin Trans";
   private const string KDC_TabPageName = "Kontakti";

   private void InitializeFilterUCFields()
   {
      SetFilterRecordDependentDefaults(); // filter.KontoOd i Do = kplan_rec.Konto (punimo bussiness od filtera, ne UC)
      TheFinFilterUC.PutFilterFields(TheFtransFilter);
   }

   public override void DecideIfShouldLoad_TransDGV(VvInnerTabControl sender, VvInnerTabPage oldPage, VvInnerTabPage newPage)
   {
      ZXC.VvInnerTabPageKindEnum innerTabPageKind = ((VvInnerTabPage)TheTabControl.SelectedTab).TheInnerTabPageKindEnum;
     
      if(aTransesLoaded[0] == false &&
         innerTabPageKind == ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage)
      {
         LoadRecordList_AND_PutTransDgvFields();
      }
   }

   public override void LoadRecordList_AND_PutTransDgvFields()
   {
      int rowIdx, idxCorrector;

      TheFinFilterUC.GetFilterFields();
      TheFinFilterUC.AddFilterMemberz(TheFtransFilter, null);

      if(kupdob_rec.Ftranses == null) kupdob_rec.Ftranses = new List<Ftrans>();
      else                            kupdob_rec.Ftranses.Clear();

      VvDaoBase.LoadGenericVvDataRecordList<Ftrans>(TheDbConnection, kupdob_rec.Ftranses, TheFtransFilter.FilterMembers, "t_dokDate DESC, t_dokNum DESC, t_serial DESC");
    
      aTransesLoaded[0] = true;

      aTransesGrid[0].Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(aTransesGrid[0]);

      foreach(Ftrans ftrans_rec in kupdob_rec.Ftranses)
      {
         aTransesGrid[0].Rows.Add();   

         rowIdx = aTransesGrid[0].RowCount - idxCorrector;

         aTransesGrid[0][ 0, rowIdx].Value = ftrans_rec.T_parentID;
         aTransesGrid[0][ 1, rowIdx].Value = ftrans_rec.T_dokDate;
         aTransesGrid[0][ 2, rowIdx].Value = ftrans_rec.T_dokNum;
         aTransesGrid[0][ 3, rowIdx].Value = ftrans_rec.T_serial;
         aTransesGrid[0][ 4, rowIdx].Value = ftrans_rec.T_TT;
         aTransesGrid[0][ 5, rowIdx].Value = ftrans_rec.T_konto;
         aTransesGrid[0][ 6, rowIdx].Value = ftrans_rec.T_opis;
         aTransesGrid[0][ 7, rowIdx].Value = ftrans_rec.T_tipBr;
         aTransesGrid[0][ 8, rowIdx].Value = ftrans_rec.T_valuta;
         aTransesGrid[0][ 9, rowIdx].Value = ftrans_rec.T_dug;
         aTransesGrid[0][10, rowIdx].Value = ftrans_rec.T_pot;

         aTransesGrid[0].Rows[rowIdx].HeaderCell.Value = (kupdob_rec.Ftranses.Count - rowIdx).ToString();
      }

      //VvDocumentRecordUC.RenumerateLineNumbers(gridFtrans, 0);
   }


   #endregion Put Trans DGV Fileds

   #endregion PutFields(), GetFields()

   #region Overriders And Specifics

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.kupdob_rec; }
      set {        this.kupdob_rec = (Kupdob)value; }
   }

   public override VvSifrarRecord VirtualSifrarRecord
   {
      get { return this.VirtualDataRecord as VvSifrarRecord; }
      set {        this.VirtualDataRecord = (Kupdob)value;   }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.KupdobDao; }
   }

   #region PrintSifrarRecord

   public VvRpt_Fin_Filter TheFtransFilter { get; set; }
  
   public FinFilterUC TheFinFilterUC { get; set; }

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
      TheFtransFilter.KD_naziv  = kupdob_rec.Naziv;
      TheFtransFilter.KD_ticker = kupdob_rec.Ticker;
      TheFtransFilter.KD_sifra  = kupdob_rec.KupdobCD/*RecID*/;
      TheFtransFilter.Grupa     = kupdob_rec.Tip;
   }

   //public RptF_FinKartKD TheRptF_FinKartKD { get; set; }

   //public override VvReport VirtualReport
   //{
   //   get
   //   {
   //      return this.TheRptF_FinKartKD;
   //   }
   //}

   public override string VirtualReportName
   {
      get
      {
         return "FINANCIJSKA KARTICA PARTNERA";
      }
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      return new RptF_FinKartKD(reportName, (VvRpt_Fin_Filter)vvRptFilter);
   }

   #endregion PrintSifrarRecord

   #region Update_VvDataRecord (Legacy naming convention)

   public override VvFindDialog CreateVvFindDialog()
   {
      return CreateFind_Kupdob_Dialog();
   }

   public static VvFindDialog CreateFind_Kupdob_Dialog()
   {
      VvSubModul vvSubModul   = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsKID);
      VvDataRecord      vvDataRecord = ZXC.TheVvForm.CreateTheVvDataRecord_SwitchSubModulEnum(vvSubModul);

      VvFindDialog vvFindDialog = new VvFindDialog();
      VvRecLstUC   vvRecListUC  = new KupdobListUC(vvFindDialog, (Kupdob)vvDataRecord, vvSubModul);
      vvFindDialog.TheRecListUC = vvRecListUC;

      return vvFindDialog;
   }

   #region Puse

   //public static string Update_Kupdob_Naziv(object startValue)
   //{
   //   Kupdob artikl_rec = new Kupdob();

   //   KupdobListUC dlg = KupdobUC.CreateFind_Kupdob_Dialog(ZXC.TheVvForm.conn);

   //   dlg.Fld_FromNaziv = startValue.ToString();

   //   if(dlg.ShowDialog() == DialogResult.localOK)
   //   {
   //      if(!ZXC.KupdobDao.SetMe_Record(ZXC.TheVvForm.conn, artikl_rec, (uint)dlg.SelectedRecIDIn_FIRST_TransGrid)) return null;
   //   }
   //   else
   //   {
   //      artikl_rec = null;
   //   }

   //   dlg.Dispose();

   //   if(artikl_rec != null) return artikl_rec.Naziv;
   //   else                   return null;
   //}

   //public static uint Update_Kupdob_Sifra(object startValue)
   //{
   //   Kupdob artikl_rec = new Kupdob();

   //   KupdobListUC dlg = KupdobUC.CreateFind_Kupdob_Dialog(ZXC.TheVvForm.conn);

   //   dlg.Fld_FromOsredCD = ZXC.ValOrZero_UInt(startValue.ToString());

   //   if(dlg.ShowDialog() == DialogResult.localOK)
   //   {
   //      if(!ZXC.KupdobDao.SetMe_Record(ZXC.TheVvForm.conn, artikl_rec, (uint)dlg.SelectedRecIDIn_FIRST_TransGrid)) return 0;
   //   }
   //   else
   //   {
   //      artikl_rec = null;
   //   }

   //   dlg.Dispose();

   //   if(artikl_rec != null) return artikl_rec.RecID;
   //   else                   return 0;
   //}

   //public static string Update_Kupdob_Ticker(object startValue)
   //{
   //   Kupdob artikl_rec = new Kupdob();

   //   KupdobListUC dlg = KupdobUC.CreateFind_Kupdob_Dialog(ZXC.TheVvForm.conn);

   //   dlg.Fld_FromTicker = startValue.ToString();

   //   if(dlg.ShowDialog() == DialogResult.localOK)
   //   {
   //      if(!ZXC.KupdobDao.SetMe_Record(ZXC.TheVvForm.conn, artikl_rec, (uint)dlg.SelectedRecIDIn_FIRST_TransGrid)) return null;
   //   }
   //   else
   //   {
   //      artikl_rec = null;
   //   }

   //   dlg.Dispose();

   //   if(artikl_rec != null) return artikl_rec.Ticker;
   //   else                   return null;
   //}

   #endregion Puse

   /// <summary>
   /// 'FindVvDataRecord' procedura. Inicirana:
   /// 1. Context menu (Mouse right click)
   /// 2. Mouse click (Ctrl ili Alt click)
   /// 3. Keyboard initiated (Ctrl/Alt + F/Space)
   /// </summary>
   /// <param name="startValue"></param>
   /// <returns></returns>
   public static object Update_Kupdob(VvSQL.SorterType whichInformation, object startValue, ZXC.AutoCompleteRestrictor sifrarRestrictor)
   {
      Kupdob          kupdob_rec   = new Kupdob();
      KupdobListUC    kupdobListUC;
      XSqlConnection  dbConnection = ZXC.TheVvForm.TheDbConnection;

      VvFindDialog dlg = CreateFind_Kupdob_Dialog();

      kupdobListUC = (KupdobListUC)(dlg.TheRecListUC);

      switch(whichInformation)
      {
         case VvSQL.SorterType.Name  : kupdobListUC.Fld_FromNaziv  = startValue.ToString()                    ; break;
         case VvSQL.SorterType.Code  : kupdobListUC.Fld_FromSifra  = ZXC.ValOrZero_UInt(startValue.ToString()); break;
         case VvSQL.SorterType.Ticker: kupdobListUC.Fld_FromTicker = startValue.ToString()                    ; break;

         default: ZXC.aim_emsg(" 111: For Kupdob, trazi po [{1}] still nedovrseno!", whichInformation); break;
      }

      #region sifrarRestrictor
      
      // new, from 05.06.2009 

      if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.KID_Centrala_Only) kupdobListUC.Fld_FiltIsCentrala = true;
      if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.KID_Mtros_Only   ) kupdobListUC.Fld_FiltIsMTros    = true;
      if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.KID_Banka_Only   ) kupdobListUC.Fld_FiltIsBanka    = true;
      if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.KID_Dobav_Only   ) kupdobListUC.Fld_FiltIsDob      = true;
      if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.KID_Kupac_Only   ) kupdobListUC.Fld_FiltIsKup      = true;
      if(sifrarRestrictor == ZXC.AutoCompleteRestrictor.KID_Komisija_Only) kupdobListUC.Fld_FiltIsKomisija = true;

      #endregion sifrarRestrictor

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         if(!ZXC.KupdobDao.SetMe_Record_byRecID(dbConnection, kupdob_rec, (uint)dlg.SelectedRecID, false)) return null;
      }
      else
      {
         kupdob_rec = null;
      }

      if(dlg.SelectionIsNewlyAddedRecord == true) ZXC.ShouldForceSifrarRefreshing = true;

      dlg.Dispose();

      if(kupdob_rec != null)
      {
         switch(whichInformation)
         {
               // 10.08.2024: Buon compleano! Nono Lorenzo :-) 
               // Autocomplete duplicates pokusaj rjesenja     
          //case VvSQL.SorterType.Name  : return kupdob_rec.Naziv   ;
            case VvSQL.SorterType.Name  : return kupdob_rec.Naziv + Kupdob.TickerToken + kupdob_rec.NazivUniqueAddition;
            case VvSQL.SorterType.Code  : return kupdob_rec.KupdobCD;
            case VvSQL.SorterType.Ticker: return kupdob_rec.Ticker  ;

            default: ZXC.aim_emsg(" 222: For Kupdob, trazi po [{1}] still nedovrseno!", whichInformation); return null;
         }
      }
      else return null;
   }

   #endregion Update_VvDataRecord (Legacy naming convention)

   public override Size ThisUcSize
   {
      get
      {
         return new Size(hampKontakt2.Right + ZXC.QunMrgn, hampZiro.Bottom + ZXC.QunMrgn);
      }
   }

   #region PutNew_Sifra_Field

   public override void PutNew_Sifra_Field(uint newSifra)
   {
      Fld_KupdobCD = newSifra;
   }

   #endregion PutNew_Sifra_Field

   void KupdobDUC_Validating(object sender, System.ComponentModel.CancelEventArgs e)
   {

      #region Should validate enivej?

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None   ||
         TheVvTabPage.WriteMode == ZXC.WriteMode.Delete ||
         this.Visible           == false) return;

      #endregion Should validate enivej?

      bool tickerExists = false;

      if(Fld_Ticker.NotEmpty())
      {
         tickerExists = KupdobDao.KupdobTICKER_AlreadyExists(TheDbConnection, Fld_Ticker, Fld_KupdobCD, ZXC.TheVvForm.TheVvUC is PrjktUC);
      }

      if(tickerExists) e.Cancel = true;

      //Dodajemo sifrar record u PG, pa treba provjeriti je li slobodna sifra i u NY 
      (bool thisSifraIs_Duplicated_InNY, VvSifrarRecord inNY_SifrarRecord) = IsThisSifra_Duplicated_InNY();

      if(thisSifraIs_Duplicated_InNY)
      {
         e.Cancel = true;
     
        ZXC.aim_emsg(MessageBoxIcon.Stop, "Dodajete šifru ili OIB partnera već zauzeto u novoj godini.\n\r\n\rIspravite ovu šifru na prvu sljedeću slobodnu,\n\r\n\rtj. da je 'slobodna' i u ovoj i u novoj godini.\n\r\n\ru novoj godini je:\n\r\n\r{0}", inNY_SifrarRecord);
      }

   }

   #endregion Overriders And Specifics

   public void GetNextTickerWroot_btnClick(object sender, EventArgs e)
   {
      string rootStr = Fld_Ticker;

    //if(rootStr.IsEmpty()) Fld_Ticker = rootStr = "TH";
      if(rootStr.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Operacija 'Automatski Ticker Increment'\n\nzahtjeva barem jedan znak kao korijen Tickera.");
         return;
      }

      uint rbrNum = ZXC.KupdobDao.GetNextSifraWroot_String(TheDbConnection, Kupdob.recordName, "ticker", rootStr);

      int formatedRbrNumLen = ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KpdbCI.ticker) - rootStr.Length;

      if(formatedRbrNumLen.IsZeroOrNegative())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Operacija 'Automatski Ticker Increment'\n\ndozvoljava max 5 znakova kao korijen Tickera.");
         return;
      }

      Fld_Ticker += rbrNum.ToString("D" + formatedRbrNumLen);

      ZXC.TheVvForm.SetDirtyFlag(sender);
   }


}

public class VvRenameTickerDlg : VvDialog
{
   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_newTicker;

   public VvRenameTickerDlg()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Preimenuj TICKER";

      CreateHamper();

      dlgWidth        = hamper.Right  + ZXC.QunMrgn;
      dlgHeight       = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_newTicker, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);

   }

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

      int columnSize = ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KupdobDao.CI.ticker);

                      hamper.CreateVvLabel  (0, 0, "Novi TICKER:", ContentAlignment.MiddleRight);
      tbx_newTicker = hamper.CreateVvTextBox(1, 0, "tbx_newTicker", "", columnSize);
      tbx_newTicker.JAM_CharacterCasing = CharacterCasing.Upper;

   }

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   public string Fld_NewTicker
   {
      get { return tbx_newTicker.Text; }
      set {        tbx_newTicker.Text = value; }
   }
}

public class VvRenameKupdobCdDlg : VvDialog
{
   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_newKupdobCd;

   public VvRenameKupdobCdDlg()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Preimenuj ŠIFRU";

      CreateHamper();

      dlgWidth        = hamper.Right  + ZXC.QunMrgn;
      dlgHeight       = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_newKupdobCd, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);

   }

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

      int columnSize = ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KupdobDao.CI.ticker);

                        hamper.CreateVvLabel  (0, 0, "Nova ŠIFRA:", ContentAlignment.MiddleRight);
      tbx_newKupdobCd = hamper.CreateVvTextBox(1, 0, "tbx_newKupdobCd", "", 6/*columnSize*/);
      //tbx_newKupdobCd.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_newKupdobCd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_newKupdobCd.JAM_FillCharacter = '0';

   }

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   public uint Fld_NewKupdobCd
   {
      get { return ZXC.ValOrZero_UInt(tbx_newKupdobCd.Text); }
      set {                           tbx_newKupdobCd.Text = value.ToString("000000"); }
   }
}

public class VvRenameNameDlg : VvDialog
{
   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_newName;

   public VvRenameNameDlg()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Preimenuj NAZIV";

      CreateHamper();

      dlgWidth        = hamper.Right  + ZXC.QunMrgn;
      dlgHeight       = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_newName, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);

   }

   private void CreateHamper()
   {
      hamper          = new VvHamper(2, 1, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q10un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      int columnSize = ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KupdobDao.CI.naziv);

                    hamper.CreateVvLabel  (0, 0, "Novi NAZIV:", ContentAlignment.MiddleRight);
      tbx_newName = hamper.CreateVvTextBox(1, 0, "tbx_newName", "", columnSize);
   }

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   public string Fld_NewName
   {
      get { return tbx_newName.Text; }
      set {        tbx_newName.Text = value; }
   }
}

public class VvPrenesiKupdobCdDlg : VvDialog
{
   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_newKupdobCd, tbx_newKupdobTK, tbx_newKupdobName;
   private CheckBox  cbx_isDeleteOldKupdob;

   public VvPrenesiKupdobCdDlg(Kupdob kupdob_rec)
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Prenesi sva knjiženja, veze i pojave prikazanoga partnera";

      CreateHamper(kupdob_rec);

      dlgWidth        = hamper.Right  + ZXC.QunMrgn;
      dlgHeight       = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_newKupdobCd  , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_newKupdobTK  , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_newKupdobName, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);

   }

   private void CreateHamper(Kupdob kupdob_rec)
   {
      hamper          = new VvHamper(4, 4, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q10un+ZXC.QUN, ZXC.Q3un, ZXC.Q3un, ZXC.Q10un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4  , ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN, ZXC.QUN, ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.QUN };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      int columnSize = ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KupdobDao.CI.ticker);

      Label lbl =       hamper.CreateVvLabel  (0, 0, String.Format("Prenesi sva knjiženja, veze i pojave partnera\n\n{0}\n\n na ovoga partnera:", kupdob_rec), 0, 2,  ContentAlignment.MiddleRight);
      tbx_newKupdobCd = hamper.CreateVvTextBox(1, 2, "tbx_newKupdobCd", "", 6/*columnSize*/);
      tbx_newKupdobCd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_newKupdobCd.JAM_FillCharacter = '0';

      tbx_newKupdobTK   = hamper.CreateVvTextBox(2, 2, "tbxTick", "Ticker poduzeća poslovnog partnera", 6);
      tbx_newKupdobName = hamper.CreateVvTextBox(3, 2, "tbxNaziv", "Naziv poduzeća poslovnog partnera", 6);

      tbx_newKupdobCd  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(ZXC.TheVvForm.TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_newKupdobTK  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(ZXC.TheVvForm.TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_newKupdobName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(ZXC.TheVvForm.TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyKupdobTextBoxLeave));


      cbx_isDeleteOldKupdob = hamper.CreateVvCheckBox_OLD(0, 3, null, 3, 0, "Nakon uspješne akcije pobriši ovu karticu partnera", RightToLeft.No);

   }
   
   void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(ZXC.TheVvForm.TheVvUC.isPopulatingSifrar) return;

      //if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != ZXC.TheVvForm.TheVvUC.originalText)
      {
         ZXC.TheVvForm.TheVvUC.originalText = tb.Text;
         kupdob_rec = VvUserControl.KupdobSifrar.Find(ZXC.TheVvForm.TheVvUC.FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_NewKupdobName = kupdob_rec.Naziv;
            Fld_NewKupdobCd   = kupdob_rec.KupdobCD/*RecID*/;
            Fld_NewKupdobTK   = kupdob_rec.Ticker;
         }
         else
         {
            Fld_NewKupdobName = Fld_NewKupdobTK = Fld_NewKupdobCdAsTxt = "";
         }
      }
   }

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   public uint Fld_NewKupdobCd
   {
      get { return ZXC.ValOrZero_UInt(tbx_newKupdobCd.Text); }
      set {                           tbx_newKupdobCd.Text = value.ToString("000000"); }
   }

   public bool Fld_isDeleteOldKupdob
   {
      get { return cbx_isDeleteOldKupdob.Checked; }
      set { cbx_isDeleteOldKupdob.Checked = value; }
   }

   public string Fld_NewKupdobCdAsTxt
   {
      get { return tbx_newKupdobCd.Text; }
      set { tbx_newKupdobCd.Text = value; }
   }

   public string Fld_NewKupdobName
   {
      get { return tbx_newKupdobName.Text; }
      set { tbx_newKupdobName.Text = value; }
   }


   public string Fld_NewKupdobTK
   {
      get { return tbx_newKupdobTK.Text; }
      set { tbx_newKupdobTK.Text = value; }
   }

}

public class VvGetMandatory_Kupdob_R1enum_Dlg : VvDialog
{
   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private RadioButton rbt_nepoznato, rbt_B2C, rbt_B2B;
   private Label       lbl_kpd;

   public VvGetMandatory_Kupdob_R1enum_Dlg(Kupdob _kupdob_rec)
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Navedite da li je kupac pravna (B2B) ili fizička (B2C) osoba.";

      CreateHamper();

      dlgWidth        = hamper.Right  + ZXC.QunMrgn;
      dlgHeight       = hamper.Bottom + ZXC.QunMrgn * 5 ;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      lbl_kpd.Text = _kupdob_rec.KupdobCD.ToString("") + " " + _kupdob_rec.Ticker + " " + _kupdob_rec.Naziv;
   }

   private void CreateHamper()
   {
      hamper          = new VvHamper(1, 5, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q10un * 2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.QUN };
      hamper.VvRightMargin = 0;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
      hamper.VvSpcBefRow[1] = ZXC.QUN;


      lbl_kpd =        hamper.CreateVvLabel      (0, 0, "", ContentAlignment.BottomLeft);
      lbl_kpd.Font = ZXC.vvFont.BaseBoldFont;

                       hamper.CreateVvLabel      (0, 1, "Kupac je pravna (B2B) ili fizička (B2C) osoba:", ContentAlignment.BottomLeft);
      rbt_nepoznato  = hamper.CreateVvRadioButton(0, 2, null, "Nepoznato"    , TextImageRelation.ImageBeforeText);
      rbt_B2B        = hamper.CreateVvRadioButton(0, 3, null, "B2B - pravna" , TextImageRelation.ImageBeforeText);
      rbt_B2C        = hamper.CreateVvRadioButton(0, 4, null, "B2C - fizička", TextImageRelation.ImageBeforeText);

      rbt_nepoznato.Checked = true;
      rbt_nepoznato.Tag     = true;
   }

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   public ZXC.F2_R1enum Fld_R1Kind
   {
      get
      {
              if(rbt_nepoznato.Checked) return ZXC.F2_R1enum.Nepoznato  ;
         else if(rbt_B2B      .Checked) return ZXC.F2_R1enum.B2B        ;
         else if(rbt_B2C      .Checked) return ZXC.F2_R1enum.B2C        ;
              else throw new Exception("Fld_AMSstatus: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.F2_R1enum.Nepoznato : rbt_nepoznato.Checked = true; break;
            case ZXC.F2_R1enum.B2B       : rbt_B2B      .Checked = true; break;
            case ZXC.F2_R1enum.B2C       : rbt_B2C      .Checked = true; break;
         }
      }
   }


}


public class KDCDUC       : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_kupDobCd, tbx_kupDobTk, tbx_kupDobName;
   private VvHamper  hamp_partner;

   #endregion Fieldz

   #region Constructor

   public KDCDUC(Control parent, Mixer _mixer, VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)

   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_KDC
         });

   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
      InitializeHamper_Partner(out hamp_partner);

      hamp_tt      .Location = new Point(hamp_partner.Right, hamp_partner.Top           );
      hamp_dokDate .Location = new Point(hamp_tt     .Right, hamp_partner.Top + ZXC.Qun8);
      hamp_dokNum  .Location = new Point(hamp_dokDate.Right, hamp_partner.Top + ZXC.Qun8);
      hamp_napomena.Location = new Point(hamp_partner.Left , hamp_partner.Bottom        );

      nextY = hamp_napomena.Bottom;
   }

   private void InitializeHamper_Partner(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false, ZXC.Qun8, ZXC.Qun8, razmakHamp);
      //                                     0                 1                
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un, ZXC.Q3un, ZXC.Q10un + ZXC.Q3un + ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4            };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvRowHgt[0] = ZXC.QUN + ZXC.Qun8;

      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel  (0, 0, "Partner:", ContentAlignment.MiddleRight);
      tbx_kupDobCd   = hamper.CreateVvTextBox(1, 0, "tbx_kupDobCd"  , "Sifra Partnera", GetDB_ColumnSize(DB_ci.kupdobCD)  );
      tbx_kupDobTk   = hamper.CreateVvTextBox(2, 0, "tbx_kupDobTk"  , "Tiker Partnera", GetDB_ColumnSize(DB_ci.kupdobTK)  );
      tbx_kupDobName = hamper.CreateVvTextBox(3, 0, "tbx_kupDobName", "Naziv Partnera", GetDB_ColumnSize(DB_ci.kupdobName));

      tbx_kupDobCd.JAM_CharEdits       = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_kupDobTk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_kupDobCd  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra ), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobTk  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName  ), new EventHandler(AnyKupdobTextBoxLeave));

      tbx_kupDobCd  .JAM_ReadOnly = 
      tbx_kupDobTk  .JAM_ReadOnly = 
      tbx_kupDobName.JAM_ReadOnly = true;

      tbx_kupDobName.JAM_Highlighted = true;
      tbx_kupDobName.Font = ZXC.vvFont.LargeBoldFont;

    //this.ControlForInitialFocus = tbx_kupDobName;

   }

   void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      //if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.originalText)
      {
         originalText = tb.Text;
         kupdob_rec = VvUserControl.KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_KupDobName = kupdob_rec.Naziv;
            Fld_KupDobCd = kupdob_rec.KupdobCD/*RecID*/;
            Fld_KupDobTk = kupdob_rec.Ticker;
         }
         else
         {
            Fld_KupDobName = Fld_KupDobTk = Fld_KupDobCdAsTxt = "";
         }
      }
   }

   #endregion CreateSpecificHampers()


   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {

      T_kpdbNameA_50_CreateColumn  (ZXC.Q9un , "Ime i prezime" , "Ime i prezime", false    );
      T_kpdbUlBrA_32_CreateColumn  (ZXC.Q9un , "Telefon"       , "Telefon"      , ZXC.Q6un );
      T_vezniDokA_64_CreateColumn  (ZXC.Q10un, "e-mail"        , "e-mail"       , null     );
      T_kpdbZiroA_32_CreateColumn  (ZXC.Q7un , "Funkcija"      , "Funkcija"     , null     );
      T_isXxx_CreateColumn         (ZXC.Q2un , "ZaFakt"                                    );
      T_opis_128_CreateColumn      (        0, "Napomena"      , "Napomena"     , 0  , null);
   }

   #endregion InitializeTheGrid_Columns()

   #region Fld_

   public uint    Fld_KupDobCd      { get { return tbx_kupDobCd.GetSomeRecIDField(); } set { tbx_kupDobCd.PutSomeRecIDField(value); } }
   public string  Fld_KupDobCdAsTxt { get { return tbx_kupDobCd.Text;                } set { tbx_kupDobCd  .Text = value;           } }
   public string  Fld_KupDobName    { get { return tbx_kupDobName.Text;              } set { tbx_kupDobName.Text = value;           } }
   public string  Fld_KupDobTk      { get { return tbx_kupDobTk.Text;                } set { tbx_kupDobTk  .Text = value;           } }

   #endregion Fld_


   //#region PrintDocumentRecord

   //public KdKontaktFilterUC  TheKdKontaktZnpFilterUC { get; set; }
   //public KdKontaktDocFilter TheKdKontaktZnpDocFilter { get; set; }

   //protected override void CreateMixerDokumentPrintUC()
   //{
   //   this.TheKdKontaktZnpDocFilter = new KdKontaktDocFilter(this);

   //   TheKdKontaktZnpFilterUC = new KdKontaktFilterUC(this);
   //   TheKdKontaktZnpFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
   //   ThePanelForFilterUC_PrintTemplateUC.Width = TheKdKontaktZnpFilterUC.Width;
   //}

   //public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   //{
   //   KdKontaktDocFilter mixerFilter = (KdKontaktDocFilter)vvRptFilter;

   //  // switch(mixerFilter.PrintVirZnp)
   //  // {
   //  //    //case KdKontaktDocFilter.PrintVirZnpEnum.VirmOkvir  : specificMixerReport = new RptX_KdKontakt(new Vektor.Reports.XIZ.CR_XKdKontakti()      , reportName, mixerFilter); break;
   //  //    case KdKontaktDocFilter.PrintVirZnpEnum.SEPA         : specificMixerReport = new RptX_KdKontakt(new Vektor.Reports.XIZ.CR_XKdKontaktiSEPA()  , reportName, mixerFilter); break;
   //  //    //case KdKontaktDocFilter.PrintVirZnpEnum.VirmNeOkvir: specificMixerReport = new RptX_KdKontakt(new Vektor.Reports.XIZ.CR_XKdKontaktNoOkvir(), reportName, mixerFilter); break;
   //  //    case KdKontaktDocFilter.PrintVirZnpEnum.VirmNeOkvir  : specificMixerReport = new RptX_KdKontakt(new Vektor.Reports.XIZ.CR_XVirmHUB3A()    , reportName, mixerFilter); break;
   //  //    case KdKontaktDocFilter.PrintVirZnpEnum.Znp          : specificMixerReport = new RptX_KdKontakt(new Vektor.Reports.XIZ.CR_XKdKontaktiZNP()   , reportName, mixerFilter); break;
   //  //
   //  //    default: ZXC.aim_emsg("{0}\nPrintSomeKdKontaktDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintVirZnp); return null;
   //  // }

   //   return specificMixerReport;
   //}

   //public override VvRptFilter VirtualRptFilter
   //{
   //   get
   //   {
   //      return this.TheKdKontaktZnpDocFilter;
   //   }
   //}

   //public override VvFilterUC VirtualFilterUC
   //{
   //   get
   //   {
   //      return this.TheKdKontaktZnpFilterUC;
   //   }
   //}

   //public override void SetFilterRecordDependentDefaults()
   //{
   //}

   //#endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Virm, Color.Empty, clr_Virm);
   }

   #endregion Colors

   #region override PutSpecificsFld() GetSpecificsFld()

   public override void PutSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd  )) Fld_KupDobCd = mixer_rec.KupdobCD;
      if(CtrlOK(tbx_kupDobName)) Fld_KupDobName = mixer_rec.KupdobName;
      if(CtrlOK(tbx_kupDobTk  )) Fld_KupDobTk = mixer_rec.KupdobTK;

      Fld_TtOpis = "KONTAKTI";
   }
   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd  )) mixer_rec.KupdobCD = Fld_KupDobCd;
      if(CtrlOK(tbx_kupDobName)) mixer_rec.KupdobName = Fld_KupDobName;
      if(CtrlOK(tbx_kupDobTk  )) mixer_rec.KupdobTK = Fld_KupDobTk;
   }

   #endregion override PutSpecificsFld() GetSpecificsFld()
}

//public class KdKontaktFilterUC  : VvFilterUC
//{
//   #region Fieldz

//   private VvHamper    hamp_virDate, hamp_rbt;
//   private VvTextBox   tbx_pomak, tbx_showVirDate;
//   private VvCheckBox  cbx_showVirDatVal;
//   private RadioButton rbt_virO, rbt_virNo, rbt_znp;

//   #endregion Fieldz

//   #region  Constructor

//   public KdKontaktFilterUC(VvUserControl vvUC)
//   {
//      this.SuspendLayout();
//      TheVvUC = vvUC;

//      CreateHampers();

//      CreateHamper_4ButtonsResetGo_Width(this.MaxHamperWidth);

//      hamp_virDate.Location = new Point(nextX, hamper4buttons.Bottom + ZXC.Qun4);
//      hamp_rbt.Location     = new Point(nextX, hamp_virDate  .Bottom + ZXC.Qun4);
//      hamperHorLine.Visible = false;

//      nextY = hamp_rbt.Bottom + razmakIzmjedjuHampera;

//      this.Width  = hamp_virDate.Width + ZXC.QUN;
//      this.Height = hamp_rbt.Bottom + ZXC.QUN;

//      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

//      this.ResumeLayout();
//   }

//   #endregion  Constructor

//   #region Hampers

//   private void CreateHampers()
//   {
//      InitializeHamper_Rbt(out hamp_rbt);
//      this.MaxHamperWidth = hamp_rbt.Width;

//      InitializeHamper_KdKontaktDate(out hamp_virDate);
//   }

//   private void InitializeHamper_KdKontaktDate(out VvHamper hamper)
//   {
//      hamper = new VvHamper(4, 2, "", this, false);

//      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.QUN - ZXC.Qun4, ZXC.Qun4, ZXC.Q2un };
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,           ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i]    = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = hamper.VvTopMargin;


//                        hamper.CreateVvLabel  (0, 0, "Printaj datume", ContentAlignment.MiddleRight);
//      tbx_showVirDate = hamper.CreateVvTextBox(1, 0, "tbx_showVirDat", "");
//      tbx_showVirDate.JAM_Highlighted = true;
//      cbx_showVirDatVal = hamper.CreateVvCheckBox(1, 0, "", tbx_showVirDate, "", "X");
//      cbx_showVirDatVal.Checked = true;

//                  hamper.CreateVvLabel  (0, 1, "Pomak printa KdKontakta", ContentAlignment.MiddleRight);
//      tbx_pomak = hamper.CreateVvTextBox(1, 1, "tbx_pomak", "Pomak printanja KdKontakta 2 - 2mm, 3 - 3mm, 4 - 4mm, 5 - 5mm", 1, 1, 0);
//      tbx_pomak.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
//      tbx_pomak.JAM_AllowedInputCharacters = "2345";
//      tbx_pomak.TextAlign = HorizontalAlignment.Center;

//      SetUpAsWriteOnlyTbx(hamper);

//      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, this.MaxHamperWidth, razmakIzmjedjuHampera);
//      VvHamper.HamperStyling(hamper);  
//   }

//   private void InitializeHamper_Rbt(out VvHamper hamper)
//   {
//      hamper = new VvHamper(1, 4, "", this, false);

//      hamper.VvColWdt      = new int[] { ZXC.Q10un};
//      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4};
//      hamper.VvRightMargin = hamper.VvLeftMargin;

//      for(int i = 0; i < hamper.VvNumOfRows; i++)
//      {
//         hamper.VvRowHgt[i]    = ZXC.QUN;
//         hamper.VvSpcBefRow[i] = ZXC.Qun8;
//      }
//      hamper.VvBottomMargin = ZXC.Qun4 + ZXC.Qun10;

//                 hamper.CreateVvLabel       (0, 0,       "Vrsta ispisa"             , ContentAlignment.MiddleLeft);
//      rbt_virO  = hamper.CreateVvRadioButton(0, 1, null, "SEPA pain.001"            , TextImageRelation.ImageBeforeText);
//      rbt_virNo = hamper.CreateVvRadioButton(0, 2, null, "KdKontakti bez pozadine"     , TextImageRelation.ImageBeforeText);
//      rbt_znp   = hamper.CreateVvRadioButton(0, 3, null, "Zbrojni nalog za prijenos", TextImageRelation.ImageBeforeText);
//      rbt_virO.Checked = true;
//      rbt_virO.Tag     = true;

//      VvHamper.HamperStyling(hamper);
//   }

//   #endregion Hampers

//   #region Fld_

//   public KdKontaktDocFilter.PrintVirZnpEnum Fld_PrintVirZnp
//   {
//      get
//      {
//         if     (rbt_virO .Checked) return KdKontaktDocFilter.PrintVirZnpEnum.SEPA;
//         if     (rbt_virNo.Checked) return KdKontaktDocFilter.PrintVirZnpEnum.VirmNeOkvir;
//         else if(rbt_znp  .Checked) return KdKontaktDocFilter.PrintVirZnpEnum.Znp    ;

//         else throw new Exception("Fld_PrintVirZnp: who df is checked?");
//      }
//      set
//      {
//         switch(value)
//         {
//            case KdKontaktDocFilter.PrintVirZnpEnum.SEPA  : rbt_virO .Checked = true; break;
//            case KdKontaktDocFilter.PrintVirZnpEnum.VirmNeOkvir: rbt_virNo.Checked = true; break;
//            case KdKontaktDocFilter.PrintVirZnpEnum.Znp        : rbt_znp  .Checked = true; break;
//         }
//      }
//   }

//   public int Fld_Pomak
//   {
//      get { return ZXC.ValOrZero_Int(tbx_pomak.Text); }
//      set {                          tbx_pomak.Text = value.ToString(); }
//   }

//   public bool Fld_ShowVirDat
//   {
//      get { return cbx_showVirDatVal.Checked; }
//      set {        cbx_showVirDatVal.Checked = value; }
//   }


//   #endregion Fld_

//   #region Put & GetFilterFields

//   private KdKontaktDocFilter TheKdKontaktZnpDocFilter
//   {
//      get { return this.TheVvUC.VirtualRptFilter as KdKontaktDocFilter; }
//      set {        this.TheVvUC.VirtualRptFilter = value; }
//   }

//   public override void PutFilterFields(VvRptFilter _filter_data)
//   {
//      TheKdKontaktZnpDocFilter = (KdKontaktDocFilter)_filter_data;

//      if(TheKdKontaktZnpDocFilter != null)
//      {
//         Fld_PrintVirZnp = TheKdKontaktZnpDocFilter.PrintVirZnp;
//         Fld_Pomak       = TheKdKontaktZnpDocFilter.VirPomak;
//         Fld_ShowVirDat  = TheKdKontaktZnpDocFilter.ShowVirDate;
//      }

//      // Za JAM_... : 
//      this.ValidateChildren();
//   }

//   public override void GetFilterFields()
//   {
//      TheKdKontaktZnpDocFilter.PrintVirZnp = Fld_PrintVirZnp;
//      TheKdKontaktZnpDocFilter.VirPomak    = Fld_Pomak;
//      TheKdKontaktZnpDocFilter.ShowVirDate = Fld_ShowVirDat;
//   }

//   #endregion Put & GetFilterFields

//   #region AddFilterMemberz()

//   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
//   {
//      KdKontaktDocFilter theRptFilter = (KdKontaktDocFilter)_vvRptFilter;

//      int numv = theRptFilter.VirPomak;
//      if(numv.NotZero())
//      {
//         theRptFilter.FilterMembers.Add(new VvSqlFilterMember("VirPomak", numv));
//      }

//      theRptFilter.FilterMembers.Add(new VvSqlFilterMember("ShowDate", theRptFilter.ShowVirDate));
//   }

//   #endregion AddFilterMemberz()

//}

//public class KdKontaktDocFilter : VvRpt_Mix_Filter
//{

//   public enum PrintVirZnpEnum
//   {
//    //VirmOkvir, VirmNeOkvir, Znp
//      SEPA, VirmNeOkvir, Znp
//   }

//   public PrintVirZnpEnum PrintVirZnp { get; set; }

//   public KdKontaktDUC theDUC;

//   public int    VirPomak    { get; set; }
//   public bool   ShowVirDate { get; set; }

//   public KdKontaktDocFilter(KdKontaktDUC _theDUC)
//   {
//      this.theDUC = _theDUC;
//      SetDefaultFilterValues();
//   }

//   #region SetDefaultFilterValues()

//   public override void SetDefaultFilterValues()
//   {
//      ZXC.VvDataBaseInfo vvDBinfo = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects;
//      int projectYear = int.Parse(vvDBinfo.ProjectYear);
//      int thisYear = DateTime.Now.Year;

//      PrintVirZnp = PrintVirZnpEnum.SEPA;
//   }

//   #endregion SetDefaultFilterValues()

//}

