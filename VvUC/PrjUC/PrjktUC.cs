using System;
using System.Drawing;
using System.Windows.Forms;

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

public class PrjktUC : KupdobUC
{
   #region Fieldz

   private VvHamper  hampTvrtka  , hampToPay   , hampPid     , hampIs,
                     hampMemoHeader, hampMemoFooter, hampMemoFooter2, hampbelowGrid, hampMinus, hampRvr, hampFiskal, hamp_CertPasswd, hamp_Sky,
                     hamper_staz, hampPeriodLock, hamp_eSgn, hamp_m2p,
                     hamp_Fiskal_F1, hamp_CertPasswd_F1, hamp_F2_Fiskal, hamp_F2_Provider, hamp_F2_RolaKind;

   private VvTextBox tbx_SudCity , tbx_MbsRbu  , tbx_DateOsn , tbx_TemKapit,
                     tbx_ToPayA  , tbx_ToPayB  , tbx_ToPayC  , 
                     tbx_PidSume , tbx_PidSRent, tbx_PidTurst, tbx_PidDobit,
                     tbx_PidKmDopr, tbx_PidKmClan, tbx_PidMO1  , tbx_PidMO2,
                     tbx_PidZdr  , tbx_PidZor  , tbx_IspostavaCd, tbx_IspostavaName,
                     tbx_DateRadStart, tbx_DateRadEnd, 
                     tbx_CertPasswd, tbx_CertPasswd2, 
                     tbx_fiskTtOnly, 
                     tbx_fiskCertIssuer, tbx_fiskCertSubject, tbx_fiskCertExpire,
                     tbx_eSgnCertPasswd, tbx_eSgnCertPasswd2, tbx_eSgnCertIssuer, tbx_eSgnCertSubject, tbx_eSgnCertExpire,
                     tbx_skySrvrHost, tbx_skyPassword, tbx_skyVvDomena,
                     tbx_F2_Password, tbx_F2_UserName,
                     tbx_vrKoefBruto1, tbx_rnoRkp, tbx_PeriodLockDay,
                     tbx_m2pShaSec, tbx_m2pApikey, tbx_m2pSerno, tbx_m2pModel;
   private CheckBox  cbx_IsSkip  , cbx_IsOver20,
                     cbx_IsZorNa , cbx_IsJednK  , cbx_IsDobit , cbx_IsAuthn,
                     cbx_IsIR, cbx_IsUr, cbx_IsDevMat, cbx_IsPriNarBind, 
                     cbx_IsFiskalOnline, 
                     cbx_IsNoTtNumChk, cbx_isFiskCashOnly, 
                     cbx_IsNoAutoFisk, 
                     cbx_isNeprofit,
                     cbx_isObustOver3, 
                     cbx_isCheckStaz, 
                     cbx_isObrStazaLast, 
                     cbx_isSkipStzOnBol,
                     cbx_isFullStzOnPol, 
                     cbx_ShouldPeriodLock,
                     cbx_ShouldPrevYearLock,
                     cbx_isBtchBookg
                     ;

   //internal byte[] _theLogo;
   public RadioButton rbt_allowAll, rbt_denyAll, rbt_denyVelOnly, rbt_alowAllNoMsg,
                      rbt_F2_MER, rbt_F2_none, rbt_F2_PND, 
                      rbt_NEMA_F2, rbt_VlastitoKnjigovodstvo_F2_ALL, rbt_VlastitoKnjigovodstvo_F2_FUR_Only, rbt_KlijentServisa_TipA, rbt_KlijentServisa_TipB, rbt_KlijentServisa_TipC, rbt_IMA_B2B_Virman_NEMA_F2;


   private PictureBox             picBox_theLogo, picBox_theLogo2;
   private AdvRichTextBox         rtbx_memoHeader, rtbx_memoFooter, rtbx_memoFooter2, rtbx_belowGrid;
   private VvRichTextBoxToolStrip ts_memoHeader  , ts_memoFooter  , ts_memoFooter2, ts_belowGrid;

   private VvDateTimePicker dtp_DateOsn, dtp_DateRadStart, dtp_DateRadEnd;
   private Prjkt            prjkt_rec;

   private PrjktDao.PrjktCI DB_ci
   {
      get { return ZXC.PrjCI; }
   }
   Label lbl_passwd2, lbl_passwd2_F1, lbl_eSgnpasswd2, lbl_eSgnNaslov;


   private const string projekt_TabPageName = "Projekt";
   private const string fiskal_TabPageName  = "Fiskal" ;
   private const string memo_TabPageName    = "Memo"   ;
   private const string placa_TabPageName   = "Plaća"  ;

   #endregion Fieldz

   #region Constructor

   public PrjktUC(Control parent, Prjkt _prjkt, VvSubModul vvSubModul) : base(parent, null, vvSubModul)
   { 
      prjkt_rec  = _prjkt;
      kupdob_rec = prjkt_rec;

      ThePrefferedRecordSorter = VirtualDataRecord.DefaultSorter;

      this.TheSubModul = vvSubModul;

      this.TheTabControl.TabPages.Add(CreateVvInnerTabPages(projekt_TabPageName, projekt_TabPageName, ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
      this.TheTabControl.TabPages.Add(CreateVvInnerTabPages(fiskal_TabPageName , fiskal_TabPageName , ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
      this.TheTabControl.TabPages.Add(CreateVvInnerTabPages(memo_TabPageName   , memo_TabPageName   , ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));
      this.TheTabControl.TabPages.Add(CreateVvInnerTabPages(prvlg_TabPageName  , prvlg_TabPageName  , ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage));
      this.TheTabControl.TabPages.Add(CreateVvInnerTabPages(placa_TabPageName  , placa_TabPageName  , ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));

      PoredakHamperaNaTabPage_Projekt();
      VvHamper.Open_Close_Fields_ForWriting(TheTabControl.TabPages[projekt_TabPageName], ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvRecordUC);

      PoredakHamperaNaTabPage_Fiskal();
      VvHamper.Open_Close_Fields_ForWriting(TheTabControl.TabPages[fiskal_TabPageName], ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvRecordUC);

      PoredakHamperaNaTabPage_Memorandum();
      VvHamper.Open_Close_Fields_ForWriting(TheTabControl.TabPages[memo_TabPageName], ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvRecordUC);

      PoredakHamperaNaTabPage_Place();
      VvHamper.Open_Close_Fields_ForWriting(TheTabControl.TabPages[placa_TabPageName], ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvRecordUC);
      

      CreateDataGridView_InitializeTheGrid_ReadOnly_Columns();
   }


   #endregion Constructor

   #region Tab_Projekt

   private void PoredakHamperaNaTabPage_Projekt()
   {
      nextX = 0;
      nextY = ZXC.Qun10;
      CreateHampTvrtka(out hampTvrtka);

      nextY = hampTvrtka.Bottom + ZXC.QUN;
      CreateHamperToPay(out hampToPay);

      nextY = hampToPay.Bottom + ZXC.QUN;
      CreateHampDateRVR(out hampRvr);

      nextY = ZXC.Qun10;
      nextX = hampTvrtka.Right + ZXC.QUN;
      CreateHamperPid(out hampPid);

      nextX = hampTvrtka.Right + ZXC.QUN;
      nextY = hampPid.Bottom + ZXC.QUN;
      CreateHamperFiskal(out hampFiskal);

      nextY = hampFiskal.Bottom;
      nextX = hampTvrtka.Right - ZXC.QUN;
     
      nextX = hampPid.Right + ZXC.QUN;
      nextY = ZXC.Qun10;
      CreateHamperIs(out hampIs);
      
      nextY = hampIs.Bottom;
      CreateMinusPoli(out  hampMinus);

      CreatePeriodLock(out  hampPeriodLock);

      CreateHamperSky(out  hamp_Sky);

      CreateHamperM2P(out hamp_m2p);

      //CreateHamperF2_Provider(out hamp_F2_Provider);

      //CreateHamperF2_RolaKind(out hamp_F2_RolaKind);

      nextX = ZXC.Qun10;
      nextY = hampRvr.Bottom;

   }

   #region hamperTvrtka

   private void CreateHampTvrtka(out VvHamper hampTvrtka)
   {
      hampTvrtka = new VvHamper(3, 6, "", TheTabControl.TabPages[projekt_TabPageName], false, nextX, nextY, razmakHamp);

      hampTvrtka.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q4un, ZXC.Q4un };
      hampTvrtka.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hampTvrtka.VvRightMargin = hampTvrtka.VvLeftMargin;

      hampTvrtka.VvRowHgt       = new int[] { ZXC.QUN,  ZXC.QUN,  ZXC.QUN ,  ZXC.QUN,  ZXC.QUN,  ZXC.QUN };
      hampTvrtka.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hampTvrtka.VvBottomMargin = hampTvrtka.VvTopMargin;

      hampTvrtka.CreateVvLabel(0, 0, "Datum osnivanja:"  , ContentAlignment.MiddleRight);
      hampTvrtka.CreateVvLabel(0, 1, "Trgovacki sud u:"  , ContentAlignment.MiddleRight);
      hampTvrtka.CreateVvLabel(0, 2, "B.reg.ul/MBS:"     , ContentAlignment.MiddleRight);
      hampTvrtka.CreateVvLabel(0, 3, "Temeljni kapital:" , ContentAlignment.MiddleRight);
      hampTvrtka.CreateVvLabel(0, 4, "Porezna Ispostava:", ContentAlignment.MiddleRight);
      hampTvrtka.CreateVvLabel(0, 5, "RNO / RKP:"        , ContentAlignment.MiddleRight);

      tbx_DateOsn     = hampTvrtka.CreateVvTextBox(1, 0, "tbx_DateOsn"    , "Datum osnivanja"                                  , GetDB_ColumnSize(DB_ci.dateOsn)       );
      tbx_SudCity     = hampTvrtka.CreateVvTextBox(1, 1, "tbx_SudCity"    , "Registrirano kod Trgovačkog suda u..."            , GetDB_ColumnSize(DB_ci.sudCity) , 1, 0);
      tbx_MbsRbu      = hampTvrtka.CreateVvTextBox(1, 2, "tbx_MbsRbu"     , "Matični broj subjekta ili Registarski broj uloška", GetDB_ColumnSize(DB_ci.mbsRbu)  , 1, 0);
      tbx_TemKapit    = hampTvrtka.CreateVvTextBox(1, 3, "tbx_TemKapit"   , "Temeljni kapital"                                 , GetDB_ColumnSize(DB_ci.temKapit), 1, 0);
      
      tbx_IspostavaCd   = hampTvrtka.CreateVvTextBoxLookUp(1, 4, "tbx_IspostavaCd"  , "Porezna ispostava sifra", 4);
      tbx_IspostavaName = hampTvrtka.CreateVvTextBox      (2, 4, "tbx_Ispostavaname", "Porezna ispostava"      );
      tbx_IspostavaCd.JAM_CharEdits = ZXC.JAM_CharEdits.NumericOnly;
      tbx_IspostavaName.JAM_ReadOnly = true;
      tbx_IspostavaCd.JAM_Set_LookUpTable(ZXC.luiListaIspostava, (int)ZXC.Kolona.prva);
      tbx_IspostavaCd.JAM_lui_NameTaker_JAM_Name = tbx_IspostavaName.JAM_Name;



      tbx_DateOsn.JAM_IsForDateTimePicker = true;

      dtp_DateOsn           = hampTvrtka.CreateVvDateTimePicker(1, 0, "", tbx_DateOsn);
      dtp_DateOsn.Name      = "dtp_DateOsn";
      tbx_TemKapit.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      tbx_rnoRkp = hampTvrtka.CreateVvTextBox(1, 5, "tbx_rnoRkp", "RNO broj za neprofitne / RKP broj za proračunske korisnike", GetDB_ColumnSize(DB_ci.rnoRkp), 1, 0);
     
   }

   #endregion hamperTvrtka

   #region hampToPay

   private void CreateHamperToPay(out VvHamper hampToPay)
   {
      hampToPay = new VvHamper(2, 4, "", TheTabControl.TabPages[projekt_TabPageName], false, nextX, nextY, razmakHamp);

      hampToPay.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q4un };
      hampToPay.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hampToPay.VvRightMargin = hampToPay.VvLeftMargin;

      hampToPay.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN  };
      hampToPay.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun2 };
      hampToPay.VvBottomMargin = hampToPay.VvTopMargin;

      Label lbl_ToPayA, lbl_ToPayB, lbl_ToPayC;

      lbl_ToPayA   = hampToPay.CreateVvLabel(0, 0, "Za fakturiranje:", ContentAlignment.MiddleRight);
      lbl_ToPayB   = hampToPay.CreateVvLabel(0, 1, "Poseban porez:"  , ContentAlignment.MiddleRight);
      lbl_ToPayC   = hampToPay.CreateVvLabel(0, 2, "ToPayC:"         , ContentAlignment.MiddleRight);
      

      tbx_ToPayA    = hampToPay.CreateVvTextBox(1, 0, "tbx_ToPayA"   , "", GetDB_ColumnSize(DB_ci.toPayA)  );
      tbx_ToPayB    = hampToPay.CreateVvTextBox(1, 1, "tbx_ToPayB"   , "", GetDB_ColumnSize(DB_ci.toPayB)  );
      tbx_ToPayC    = hampToPay.CreateVvTextBox(1, 2, "tbx_ToPayC"   , "", GetDB_ColumnSize(DB_ci.toPayC)  );

      tbx_ToPayA   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, 999999.99M      , true);
      tbx_ToPayB   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, 999999.99M      , true);
      tbx_ToPayC   .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, 999999.99M      , true);

   }

   #endregion hampToPay

   #region hamoerPid

   private void CreateHamperPid(out VvHamper hampPid)
   {
      hampPid = new VvHamper(2, 10, "", TheTabControl.TabPages[projekt_TabPageName], false, nextX, nextY, razmakHamp);

      hampPid.VvColWdt      = new int[] { ZXC.Q8un, ZXC.Q4un };
      hampPid.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hampPid.VvRightMargin = hampPid.VvLeftMargin;

      hampPid.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hampPid.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hampPid.VvBottomMargin = hampPid.VvTopMargin;

      Label lbl_PidSume, lbl_PidSRent, lbl_PidTurst, lbl_PidDobit, lbl_PidKomor,
            lbl_PidDohod, lbl_PidMO1, lbl_PidMO2, lbl_PidZdr, lbl_PidZor;

      lbl_PidSume  = hampPid.CreateVvLabel(0, 0, "Doprinos za Šume:"      , ContentAlignment.MiddleRight);
      lbl_PidSRent = hampPid.CreateVvLabel(0, 1, "Spomenička Renta:"      , ContentAlignment.MiddleRight);
      lbl_PidTurst = hampPid.CreateVvLabel(0, 2, "Turistička Članarina:"  , ContentAlignment.MiddleRight);
      lbl_PidDobit = hampPid.CreateVvLabel(0, 3, "Porez na Dobit/Dohodak:", ContentAlignment.MiddleRight);
    //lbl_PidKomor = hampPid.CreateVvLabel(0, 4, "Komorska Doprinos:"    , ContentAlignment.MiddleRight); 12.03.2014.
      lbl_PidKomor = hampPid.CreateVvLabel(0, 4, "Dop za Zapošljavanje:"  , ContentAlignment.MiddleRight);
      lbl_PidDohod = hampPid.CreateVvLabel(0, 5, "Komorski Članarina:"    , ContentAlignment.MiddleRight);
      lbl_PidMO1   = hampPid.CreateVvLabel(0, 6, "MIO I stup:"            , ContentAlignment.MiddleRight);
      lbl_PidMO2   = hampPid.CreateVvLabel(0, 7, "MIO II stup:"           , ContentAlignment.MiddleRight);
      lbl_PidZdr   = hampPid.CreateVvLabel(0, 8, "Zdravstveno Osiguranje:", ContentAlignment.MiddleRight);
      lbl_PidZor   = hampPid.CreateVvLabel(0, 9, "ZOR:"                   , ContentAlignment.MiddleRight);

      tbx_PidSume  = hampPid.CreateVvTextBox(1, 0, "tbx_PidSume" , "Doprinos za Hrvatske šume"          , GetDB_ColumnSize(DB_ci.pidSume) );
      tbx_PidSRent = hampPid.CreateVvTextBox(1, 1, "tbx_PidSRent", "Doprinos za Spomeničku rentu"       , GetDB_ColumnSize(DB_ci.pidSRent));
      tbx_PidTurst = hampPid.CreateVvTextBox(1, 2, "tbx_PidTurst", "Doprinos za Turističku zajednicu"   , GetDB_ColumnSize(DB_ci.pidTurst));
      tbx_PidDobit = hampPid.CreateVvTextBox(1, 3, "tbx_PidDobit", "Porez na Dobit / Dohodak"           , GetDB_ColumnSize(DB_ci.pidDobit));
      tbx_PidKmDopr= hampPid.CreateVvTextBox(1, 4, "tbx_PidKomCla","Komorska doprinos HGK / ObrtKomora" , GetDB_ColumnSize(DB_ci.pidKmDopr));
      tbx_PidKmClan= hampPid.CreateVvTextBox(1, 5, "tbx_PidKomDop","Komorski članarina HGK / ObtrKomora", GetDB_ColumnSize(DB_ci.pidKmClan));
      tbx_PidMO1   = hampPid.CreateVvTextBox(1, 6, "tbx_PidMO1"  , "Doprinos MO I stup"                 , GetDB_ColumnSize(DB_ci.pidMO1)  );
      tbx_PidMO2   = hampPid.CreateVvTextBox(1, 7, "tbx_PidMO2"  , "Doprinos MO II stup"                , GetDB_ColumnSize(DB_ci.pidMO2)  );
      tbx_PidZdr   = hampPid.CreateVvTextBox(1, 8, "tbx_PidZdr"  , "Doprinos za zdravstveno osiguranje" , GetDB_ColumnSize(DB_ci.pidZdr)  );
      tbx_PidZor   = hampPid.CreateVvTextBox(1, 9, "tbx_PidZor"  , "Doprinos za ZOR"                    , GetDB_ColumnSize(DB_ci.pidZor)  );

      tbx_PidSume. JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, 999999.99M, true);
      tbx_PidSRent.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, 999999.99M, true);
      tbx_PidTurst.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, 999999.99M, true);
      tbx_PidDobit.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, 999999.99M, true);
      tbx_PidKmDopr.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, 999999.99M, true);
      tbx_PidKmClan.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, 999999.99M, true);
      tbx_PidMO1.  JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, 999999.99M, true);
      tbx_PidMO2.  JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, 999999.99M, true);
      tbx_PidZdr.  JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, 999999.99M, true);
      tbx_PidZor.  JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, 999999.99M, true);   

   }

   #endregion hamoerPid
   
   #region hamperIS

   private void CreateHamperIs(out VvHamper hampIs)
   {
      hampIs = new VvHamper(1, 11, "", TheTabControl.TabPages[projekt_TabPageName], false, nextX, nextY, razmakHamp);

      hampIs.VvColWdt      = new int[] { ZXC.Q10un +ZXC.Q2un};
      hampIs.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hampIs.VvRightMargin = hampIs.VvLeftMargin;

      hampIs.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN };
      hampIs.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hampIs.VvBottomMargin = hampIs.VvTopMargin;

      cbx_IsSkip       = hampIs.CreateVvCheckBox_OLD(0,  0, null, "Izuzet"                              , RightToLeft.Yes);
      cbx_IsOver20     = hampIs.CreateVvCheckBox_OLD(0,  1, null, "Više od 20 radnika"                  , RightToLeft.Yes);
      cbx_IsZorNa      = hampIs.CreateVvCheckBox_OLD(0,  2, null, "Zor na"                              , RightToLeft.Yes);
      cbx_IsJednK      = hampIs.CreateVvCheckBox_OLD(0,  3, null, "Jednostrano Knjiženje"               , RightToLeft.Yes);
      cbx_IsDobit      = hampIs.CreateVvCheckBox_OLD(0,  4, null, "Dobit"                               , RightToLeft.Yes);
      cbx_IsAuthn      = hampIs.CreateVvCheckBox_OLD(0,  5, null, "Koristi Privilegije"                 , RightToLeft.Yes);
      cbx_IsDevMat     = hampIs.CreateVvCheckBox_OLD(0,  6, null, "Devizno"                             , RightToLeft.Yes);
      cbx_IsUr         = hampIs.CreateVvCheckBox_OLD(0,  7, null, "UR-a NE mjenja na skladišnu količinu", RightToLeft.Yes);
      cbx_IsIR         = hampIs.CreateVvCheckBox_OLD(0,  8, null, "IR-a NE mjenja na skladišnu količinu", RightToLeft.Yes);
      cbx_IsPriNarBind = hampIs.CreateVvCheckBox_OLD(0,  9, null, "Primka Vezana na Narudžbu"           , RightToLeft.Yes);
      cbx_isNeprofit   = hampIs.CreateVvCheckBox_OLD(0, 10, null, "Neprofitna organizacija"             , RightToLeft.Yes);
   }

     
   #endregion hamperIS

   private void CreateMinusPoli(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 5, "", TheTabControl.TabPages[projekt_TabPageName], false, nextX, nextY, razmakHamp); 

      hamper.VvColWdt      = new int[] { ZXC.Q9un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Q4un};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun5;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel      (0, 0,       "Politika Minusa:"          , ContentAlignment.MiddleLeft);
      rbt_allowAll     = hamper.CreateVvRadioButton(0, 1, null, "Dozvoli minus"             , TextImageRelation.ImageBeforeText);
      rbt_alowAllNoMsg = hamper.CreateVvRadioButton(0, 2, null, "Dozvoli minus i ne javljaj", TextImageRelation.ImageBeforeText);
      rbt_denyAll      = hamper.CreateVvRadioButton(0, 3, null, "Zabrani minus"             , TextImageRelation.ImageBeforeText);
      rbt_denyVelOnly  = hamper.CreateVvRadioButton(0, 4, null, "Zabrani samo VP minus"     , TextImageRelation.ImageBeforeText);

      rbt_allowAll.Checked = true;
      rbt_allowAll.Tag     = true;

   }
  
   private void CreateHampDateRVR  (out VvHamper hamper)
   {
      hamper = new VvHamper(2, 2, "", TheTabControl.TabPages[projekt_TabPageName], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN,  ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Početak RadVrem:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 1, "Kraj RadVrem:", ContentAlignment.MiddleRight);

      tbx_DateRadStart = hamper.CreateVvTextBox(1, 0, "tbx_RadStart", "Sat početka rada", GetDB_ColumnSize(DB_ci.rvrOd));
    //tbx_DateRadStart.JAM_IsForDateTimePicker = true;
      tbx_DateRadStart.JAM_IsForDateTimePicker_TimeOnlyDisplay = true;
      dtp_DateRadStart = hamper.CreateVvDateTimePicker(1, 0, "", tbx_DateRadStart);
      dtp_DateRadStart.Name = "dtp_DateRadStart";

      tbx_DateRadEnd = hamper.CreateVvTextBox(1, 1, "tbx_RadEnd", "Sat kraj rada", GetDB_ColumnSize(DB_ci.rvrDo));
    //tbx_DateRadEnd.JAM_IsForDateTimePicker = true;
      tbx_DateRadEnd.JAM_IsForDateTimePicker_TimeOnlyDisplay = true;
      dtp_DateRadEnd = hamper.CreateVvDateTimePicker(1, 1, "", tbx_DateRadEnd);
      dtp_DateRadEnd.Name = "dtp_DateRadEnd";
   }

   private void CreatePeriodLock(out VvHamper hampPeriodLock)
   {
      hampPeriodLock = new VvHamper(2, 3, "", TheTabControl.TabPages[projekt_TabPageName], false, hampMinus.Left + ZXC.Q2un, hampMinus.Bottom, razmakHamp);

      hampPeriodLock.VvColWdt      = new int[] { ZXC.Q5un + ZXC.Qun4, ZXC.Q3un };
      hampPeriodLock.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hampPeriodLock.VvRightMargin = hampPeriodLock.VvLeftMargin;

      hampPeriodLock.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN };
      hampPeriodLock.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hampPeriodLock.VvBottomMargin = hampIs.VvTopMargin;

      cbx_ShouldPeriodLock = hampPeriodLock.CreateVvCheckBox_OLD(0, 0, null, 1, 0, "Zaključaj prethodni mjesec", RightToLeft.Yes);

                             hampPeriodLock.CreateVvLabel  (0, 1, "Dana:", ContentAlignment.MiddleRight);
      tbx_PeriodLockDay    = hampPeriodLock.CreateVvTextBox(1, 1, "tbx_RadStart", "Na dan:", GetDB_ColumnSize(DB_ci.periodLockDay));
      tbx_PeriodLockDay.JAM_CharEdits = ZXC.JAM_CharEdits.NumericOnly;

      cbx_ShouldPrevYearLock = hampPeriodLock.CreateVvCheckBox_OLD(0, 2, null, 1, 0, "Zaključaj prethodne godine", RightToLeft.Yes);

   }

   #region OldFiskal

   private void CreateHamperFiskal(out VvHamper hampFiskal)
   {
      hampFiskal = new VvHamper(2, 5, "", TheTabControl.TabPages[projekt_TabPageName], false, nextX, nextY, razmakHamp);

      hampFiskal.VvColWdt      = new int[] { ZXC.Q10un, ZXC.Q2un};
      hampFiskal.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hampFiskal.VvRightMargin = hampFiskal.VvLeftMargin;

      hampFiskal.VvRowHgt       = new int[] { ZXC.QUN ,ZXC.QUN, ZXC.QUN , ZXC.QUN, ZXC.QUN };
      hampFiskal.VvSpcBefRow    = new int[] { ZXC.Qun8,ZXC.Qun2, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8};
      hampFiskal.VvBottomMargin = hampFiskal.VvTopMargin;

      cbx_IsNoTtNumChk   = hampFiskal.CreateVvCheckBox_OLD(0, 0, null, 1, 0,"NE provjeravaj slijednost broja računa", RightToLeft.Yes);

    //cbx_IsFiskalOnline = hampFiskal.CreateVvCheckBox_OLD(0, 1, null, 1, 0, "Online FISKALIZACIJA"          , RightToLeft.Yes);
      cbx_isFiskCashOnly = hampFiskal.CreateVvCheckBox_OLD(0, 2, null, 1, 0, "Fiskaliziraj samo NE VIRMANSKE", RightToLeft.Yes);
    //cbx_IsNoAutoFisk   = hampFiskal.CreateVvCheckBox_OLD(0, 3, null, 1, 0, "NE fiskaliziraj automatski"    , RightToLeft.Yes);

    //                 hampFiskal.CreateVvLabel  (0, 4, "Fiskaliziraj samo TT:", ContentAlignment.MiddleRight);
    //tbx_fiskTtOnly = hampFiskal.CreateVvTextBox(1, 4, "tbx_fiskTtOnly", "TT", GetDB_ColumnSize(DB_ci.fiskTtOnly));
    //tbx_fiskTtOnly.JAM_CharacterCasing = CharacterCasing.Upper;
    //tbx_fiskTtOnly.JAM_CharEdits       = ZXC.JAM_CharEdits.LettersOnly;

   }

   #endregion OldFiskal

   #region Sky

   private void CreateHamperSky(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 3, "", TheTabControl.TabPages[projekt_TabPageName], false, hampRvr.Left, hampRvr.Bottom + ZXC.Q2un, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q10un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel  (0, 0, "SkySrvrHost:"   , ContentAlignment.MiddleRight);
                        hamper.CreateVvLabel  (0, 1, "SkyPassword:"   , ContentAlignment.MiddleRight);
                        hamper.CreateVvLabel  (0, 2, "SkyVvDomena:"   , ContentAlignment.MiddleRight);
      tbx_skySrvrHost = hamper.CreateVvTextBox(1, 0, "tbx_skySrvrHost", "SkySrvrHost", GetDB_ColumnSize(DB_ci.skySrvrHost));
      tbx_skyPassword = hamper.CreateVvTextBox(1, 1, "tbx_skyPassword", "SkyPassword", GetDB_ColumnSize(DB_ci.skyPassword));
      tbx_skyVvDomena = hamper.CreateVvTextBox(1, 2, "tbx_skyVvDomena", "SkyVvDomena", GetDB_ColumnSize(DB_ci.skyVvDomena));

#if(DEBUG)
      // Kada hoces vidjeti userov password, samo komentiraj dolnju liniju. 
      //tbx_skySrvrHost.JAM_IsPassword = true;
      //tbx_skyPassword.JAM_IsPassword = true;
      //tbx_skyVvDomena.JAM_IsPassword = true;
#else

      tbx_skySrvrHost.JAM_IsPassword = true;
      tbx_skyPassword.JAM_IsPassword = true;
      tbx_skyVvDomena.JAM_IsPassword = true;

#endif

      if(ZXC.CURR_userName == ZXC.vvDB_skyUserName         || 
         ZXC.CURR_userName == ZXC.vvDB_systemSuperUserName ||
         ZXC.CURR_userName == ZXC.vvDB_programSuperUserName ) hamper.Visible = true ;
      else                                                    hamper.Visible = false;
   }

   #endregion Sky

   #region M2P

   private void CreateHamperM2P(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 4, "", TheTabControl.TabPages[projekt_TabPageName], false, hampRvr.Left, hamp_Sky.Bottom + ZXC.Q2un, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q10un*2 + ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN , ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel  (0, 0, "M2P ShaSec:", ContentAlignment.MiddleRight);
                        hamper.CreateVvLabel  (0, 1, "M2P Apikey:", ContentAlignment.MiddleRight);
                        hamper.CreateVvLabel  (0, 2, "M2P Serno:" , ContentAlignment.MiddleRight);
                        hamper.CreateVvLabel  (0, 3, "M2P Model:" , ContentAlignment.MiddleRight);
      tbx_m2pShaSec = hamper.CreateVvTextBox(1, 0, "tbx_m2pShaSec", "M2P ShaSec", GetDB_ColumnSize(DB_ci.m2pShaSec));
      tbx_m2pApikey = hamper.CreateVvTextBox(1, 1, "tbx_m2pApikey", "M2P Apikey", GetDB_ColumnSize(DB_ci.m2pApikey));
      tbx_m2pSerno  = hamper.CreateVvTextBox(1, 2, "tbx_m2pSerno" , "M2P Serno" , GetDB_ColumnSize(DB_ci.m2pSerno ));
      tbx_m2pModel  = hamper.CreateVvTextBox(1, 3, "tbx_m2pModel" , "M2P Model" , GetDB_ColumnSize(DB_ci.m2pModel ));

#if(DEBUG)
      // Kada hoces vidjeti userov password, samo komentiraj dolnju liniju. 
      //tbx_m2pShaSec.JAM_IsPassword = true;
      //tbx_m2pApikey.JAM_IsPassword = true;
#else

      tbx_m2pShaSec.JAM_IsPassword = true;
      tbx_m2pApikey.JAM_IsPassword = true;

#endif

      if(ZXC.CURR_userName == ZXC.vvDB_skyUserName         || 
         ZXC.CURR_userName == ZXC.vvDB_systemSuperUserName ||
         ZXC.CURR_userName == ZXC.vvDB_programSuperUserName ) hamper.Visible = true ;
      else                                                    hamper.Visible = false;
   }


   #endregion M2P

   #endregion Tab_Projekt

   #region Tab_Fiskal

   private void PoredakHamperaNaTabPage_Fiskal()
   {
      nextX = ZXC.QunMrgn;
      nextY = ZXC.QunMrgn;
      CreateHamperFiskal_F1(out hamp_Fiskal_F1);

      nextY = hamp_Fiskal_F1.Bottom;
      CreateHamperCertPasswd_F1(out hamp_CertPasswd_F1);

      nextX = hamp_CertPasswd_F1.Right + ZXC.Q2un;
      nextY = ZXC.QunMrgn;
      CreateHamperF2_Provider(out hamp_F2_Provider);

      nextY = hamp_F2_Provider.Bottom;
      CreateHamperF2_fiskal(out hamp_F2_Fiskal);

      nextX = hamp_F2_Fiskal.Right + ZXC.Q2un;
      nextY = ZXC.QunMrgn;
      CreateHamperF2_RolaKind(out hamp_F2_RolaKind);

    //nextX = ZXC.Qun10;

   }

   #region F1

   private void CreateHamperFiskal_F1(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 4, "", TheTabControl.TabPages[fiskal_TabPageName], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q9un + ZXC.Q5un, ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lblF1 = hamper.CreateVvLabel(0, 0, "Fiskalizacija B2C F1", ContentAlignment.MiddleLeft);
      lblF1.Font  = ZXC.vvFont.BaseBoldFont;

      cbx_IsFiskalOnline = hamper.CreateVvCheckBox_OLD(0, 1, null, 1, 0, "Online FISKALIZACIJA", RightToLeft.Yes);
      cbx_IsNoAutoFisk   = hamper.CreateVvCheckBox_OLD(0, 2, null, 1, 0, "NE fiskaliziraj automatski" , RightToLeft.Yes);

                       hamper.CreateVvLabel  (0, 3, "Fiskaliziraj samo TT:", ContentAlignment.MiddleRight);
      tbx_fiskTtOnly = hamper.CreateVvTextBox(1, 3, "tbx_fiskTtOnly", "TT", GetDB_ColumnSize(DB_ci.fiskTtOnly));
      tbx_fiskTtOnly.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_fiskTtOnly.JAM_CharEdits       = ZXC.JAM_CharEdits.LettersOnly;

      hamper.BackColor = Color.Lavender;

   }

   private void CreateHamperCertPasswd_F1(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 5, "", TheTabControl.TabPages[fiskal_TabPageName], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q10un + ZXC.QUN};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;



                       hamper.CreateVvLabel  (0, 0, "Cert Password:", ContentAlignment.MiddleRight);
      tbx_CertPasswd = hamper.CreateVvTextBox(1, 0, "tbx_CertPasswd", "Cert Password", GetDB_ColumnSize(DB_ci.certPasswd));
      //tbx_CertPasswd.MaxLength = 16; // !!! 
#if(DEBUG)
      // Kada hoces vidjeti userov password, samo komentiraj dolnju liniju. 
      //tbx_CertPasswd.JAM_IsPassword   = true;
#else
      tbx_CertPasswd.JAM_IsPassword = true;
#endif
      //tbx_CertPasswd.JAM_DataRequired = true;
      tbx_CertPasswd.JAM_PasswdField_TextChanged_Method = new EventHandler(tbx_CertPasswd_TextChanged);

      lbl_passwd2     = hamper.CreateVvLabel  (0, 1, "Ponovljeni password:", ContentAlignment.MiddleRight);
      tbx_CertPasswd2 = hamper.CreateVvTextBox(1, 1, "tbx_passwd2", "Ponovljeni password", GetDB_ColumnSize(DB_ci.certPasswd));
      lbl_passwd2.Tag = tbx_CertPasswd2.Tag = "Visible_ only_ZXC.ZaUpis.Otvoreno";
      lbl_passwd2.Visible = tbx_CertPasswd2.Visible = false;
      tbx_CertPasswd2.JAM_IsPassword = true;

      hamper.CreateVvLabel  (0, 2, "Issuer:" , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel  (0, 3, "Subject:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel  (0, 4, "Expire:" , ContentAlignment.MiddleRight);
      tbx_fiskCertIssuer  = hamper.CreateVvTextBox(1, 2, "tbx_fiskCertIssuer" , "Issuer:" );
      tbx_fiskCertSubject = hamper.CreateVvTextBox(1, 3, "tbx_fiskCertSubject", "Subject:");
      tbx_fiskCertExpire  = hamper.CreateVvTextBox(1, 4, "tbx_fiskCertExpire" , "Expire:" );

      tbx_fiskCertIssuer .JAM_ReadOnly = true;
      tbx_fiskCertSubject.JAM_ReadOnly = true;
      tbx_fiskCertExpire .JAM_ReadOnly = true;

      this.Validating += new System.ComponentModel.CancelEventHandler(UserUC_Validating);

      hamper.BackColor = Color.Lavender;

   }

   void UserUC_Validating(object sender, System.ComponentModel.CancelEventArgs e)
   {
      if(tbx_CertPasswd2.Visible == false) return;

      if(Fld_CertPasswd != Fld_CertPasswd2)
      {
         ZXC.RaiseErrorProvider(tbx_CertPasswd2, "Passwordi se ne podudaraju!");
         e.Cancel = true;
      }
      else Fld_CertPasswd2 = "";
   }

   void tbx_CertPasswd_TextChanged(object sender, EventArgs e)
   {
      if(lbl_passwd2.Visible == true && tbx_CertPasswd2.Visible == true) return;

      lbl_passwd2.Visible = tbx_CertPasswd2.Visible = true;
      Fld_CertPasswd2 = "";
   }

   #endregion F1

   #region F2

   private void CreateHamperF2_Provider(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 5, "", TheTabControl.TabPages[fiskal_TabPageName], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q4un};
      hamper.VvSpcBefCol   = new int[] { ZXC.QUN , ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lblF2 = hamper.CreateVvLabel(0, 0, "Fiskalizacija B2B F2", ContentAlignment.MiddleRight);
      lblF2.Font = ZXC.vvFont.BaseBoldFont;


                    hamper.CreateVvLabel      (0, 1, "F2 Posrednik:" , ContentAlignment.MiddleLeft);
      rbt_F2_none = hamper.CreateVvRadioButton(0, 2, null, "Nije MER", TextImageRelation.ImageBeforeText);
      rbt_F2_MER  = hamper.CreateVvRadioButton(0, 3, null, "MER"     , TextImageRelation.ImageBeforeText);
      rbt_F2_PND  = hamper.CreateVvRadioButton(0, 4, null, "PND"     , TextImageRelation.ImageBeforeText);
      rbt_F2_PND.Visible = false; // Za sad sakrij PND provider

      rbt_F2_none.Checked = true;
      rbt_F2_none.Tag = true;

      hamper.BackColor = Color.PeachPuff;

   }

   private void CreateHamperF2_fiskal(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 4, "", TheTabControl.TabPages[fiskal_TabPageName], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q6un };
      hamper.VvSpcBefCol   = new int[] { ZXC.QUN, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;


                        hamper.CreateVvLabel  (0, 0, "Podaci za prijavu na F2 posrednika:", 1, 0,ContentAlignment.MiddleLeft);
                        hamper.CreateVvLabel  (0, 1, "ID broj:"      , ContentAlignment.MiddleRight);
                        hamper.CreateVvLabel  (0, 2, "Password:"     , ContentAlignment.MiddleRight);
      tbx_F2_UserName = hamper.CreateVvTextBox(1, 1, "tbx_F2_UserName", "F2 username", GetDB_ColumnSize(DB_ci.f2_UserName));
      tbx_F2_Password = hamper.CreateVvTextBox(1, 2, "tbx_F2_password", "F2 password"     , GetDB_ColumnSize(DB_ci.f2_Password));

      hamper.BackColor = Color.PeachPuff;

#if(DEBUG)
      // Kada hoces vidjeti userov password, samo komentiraj dolnju liniju. 
      //tbx_F2_UserName.JAM_IsPassword = true;
      //tbx_F2_Password.JAM_IsPassword = true;
#else
      tbx_F2_UserName.JAM_IsPassword = true;
      tbx_F2_Password.JAM_IsPassword = true;
#endif

      if(ZXC.CURR_userName == ZXC.vvDB_skyUserName          || 
         ZXC.CURR_userName == ZXC.vvDB_systemSuperUserName  ||
         ZXC.CURR_userName == ZXC.vvDB_programSuperUserName ||
         ZXC.CURR_user_rec.IsSuper                           ) hamper.Visible = true ;
      else                                                    hamper.Visible = false;
   }

   #endregion F2

   #region RolaKind

   private void CreateHamperF2_RolaKind(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 9, "", TheTabControl.TabPages[fiskal_TabPageName], false, nextX, nextY, razmakHamp); 

      hamper.VvColWdt      = new int[] { ZXC.Q10un + ZXC.Q3un};
      hamper.VvSpcBefCol   = new int[] { ZXC.QUN             };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lblrk = hamper.CreateVvLabel(0, 0, "Fisk Knjigovodstvo", ContentAlignment.MiddleLeft);
      lblrk.Font = ZXC.vvFont.BaseBoldFont;


                                              hamper.CreateVvLabel      (0, 1,       "Uloga projekta:"                                           , ContentAlignment.MiddleLeft);
      rbt_NEMA_F2                           = hamper.CreateVvRadioButton(0, 2, null, ZXC.F2_RolaKind.NEMA_F2                          .ToString(), TextImageRelation.ImageBeforeText);
      rbt_VlastitoKnjigovodstvo_F2_ALL      = hamper.CreateVvRadioButton(0, 3, null, ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_ALL     .ToString(), TextImageRelation.ImageBeforeText);
      rbt_VlastitoKnjigovodstvo_F2_FUR_Only = hamper.CreateVvRadioButton(0, 4, null, ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_FUR_Only.ToString(), TextImageRelation.ImageBeforeText);
      rbt_KlijentServisa_TipA               = hamper.CreateVvRadioButton(0, 5, null, ZXC.F2_RolaKind.KlijentServisa_TipA              .ToString(), TextImageRelation.ImageBeforeText);
      rbt_KlijentServisa_TipB               = hamper.CreateVvRadioButton(0, 6, null, ZXC.F2_RolaKind.KlijentServisa_TipB              .ToString(), TextImageRelation.ImageBeforeText);
      rbt_KlijentServisa_TipC               = hamper.CreateVvRadioButton(0, 7, null, ZXC.F2_RolaKind.KlijentServisa_TipC              .ToString(), TextImageRelation.ImageBeforeText);
      rbt_IMA_B2B_Virman_NEMA_F2            = hamper.CreateVvRadioButton(0, 8, null, ZXC.F2_RolaKind.IMA_B2B_Virman_NEMA_F2           .ToString(), TextImageRelation.ImageBeforeText);

      rbt_NEMA_F2.Checked = true;
      rbt_NEMA_F2.Tag     = true;

    //hamper.CreateVvLabel(1, 2, " - nema obavezu slanja i primanja eRačuna", ContentAlignment.MiddleLeft);
    //hamper.CreateVvLabel(1, 3, " - obveza primanja i slanja eRačuna"      , ContentAlignment.MiddleLeft);
    //hamper.CreateVvLabel(1, 4, " - obveza samo primanja eRačuna"          , ContentAlignment.MiddleLeft);
    //hamper.CreateVvLabel(1, 5, " - obveza primanja i slanja eRačuna"      , ContentAlignment.MiddleLeft);
    //hamper.CreateVvLabel(1, 6, "Uloga projekta:", ContentAlignment.MiddleLeft);
    //hamper.CreateVvLabel(1, 7, "Uloga projekta:", ContentAlignment.MiddleLeft);



      hamper.BackColor = Color.LightBlue;

   }

   #endregion rolaKind

   #endregion Tab_Fiskal

   #region Tab_Place

   private void PoredakHamperaNaTabPage_Place()
   {
      nextX = 0;
      nextY = ZXC.Qun10;
      CreateHampStaz(out hamper_staz);

      nextX = ZXC.Qun10;
      nextY = hampRvr.Bottom;
   }

   private void CreateHampStaz(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 15, "", TheTabControl.TabPages[placa_TabPageName], false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q10un + ZXC.Q4un, ZXC.Q4un, ZXC.Q10un *2 + ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] {             ZXC.Qun4, ZXC.Qun4, ZXC.Q4un  };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel  (0, 0, "Vrijednost Koeficijenta za Bruto1:", ContentAlignment.MiddleRight);
      tbx_vrKoefBruto1 = hamper.CreateVvTextBox(1, 0, "tbx_vrKoefBr1", "", GetDB_ColumnSize(DB_ci.vrKoefBr1));

      tbx_vrKoefBruto1.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      cbx_isObustOver3   = hamper.CreateVvCheckBox_OLD(2, 0, null, "Dopusti obustave veće od 1/3 neto plaće"                                                   , RightToLeft.No);
      cbx_isCheckStaz    = hamper.CreateVvCheckBox_OLD(2, 1, null, "Provjeravaj godine staža za obračun dodatka na staž"                                       , RightToLeft.No); 
      cbx_isObrStazaLast = hamper.CreateVvCheckBox_OLD(2, 2, null, "Obračunaj dodatak na staž i na rezultate evidncije rada (na sve dodatke na plaću)"         , RightToLeft.No);
      cbx_isSkipStzOnBol = hamper.CreateVvCheckBox_OLD(2, 3, null, "Ne obračunavaj dodatak na staž za radnika na bolovanju na teret poslodavca"                , RightToLeft.No);
      cbx_isFullStzOnPol = hamper.CreateVvCheckBox_OLD(2, 4, null, "Obračunaj dodatak na staž za radnika na pola radnog vremena na puni iznos godina staža"    , RightToLeft.No);
      cbx_isBtchBookg    = hamper.CreateVvCheckBox_OLD(2, 6, null, "SEPA - korištenje usluge terećenja računa u ukupnom iznosu za isplatu neto plaća"          , RightToLeft.No);
   }

   #endregion Tab_Place

   #region Tab_Memorandum

   private void PoredakHamperaNaTabPage_Memorandum()
   {
      nextX = nextY = ZXC.QunMrgn;
      //int restX = (int)ZXC.GetPixeliVodoravniZaMilimetre(190);
      int restX = (int)ZXC.GetPixeliVodoravniZaMilimetre(195);
      int restX4Header;

      CreatePicturBox();
      CreatePicturBox2();
      //picBox_theLogo2.Visible = false;

      //if(prjkt_rec.TheLogo != null)
      //{
         nextX = picBox_theLogo.Right;
         picBox_theLogo.Visible = true;
        // restX4Header = (int)ZXC.GetPixeliVodoravniZaMilimetre(190) - picBox_theLogo.Width;
         restX4Header = (int)ZXC.GetPixeliVodoravniZaMilimetre(195) - picBox_theLogo.Width;
      //}
      //else
      //{
      //   nextX = ZXC.QunMrgn;
      //   picBox_theLogo.Visible = false;
      //   restX4Header = restX;
      //}

      CreateHamperHeader(out hampMemoHeader, restX4Header);

      nextX = ZXC.QunMrgn; 
      nextY = hampMemoHeader.Bottom;
      CreateHamperBelowGrid(out hampbelowGrid, restX);

      CreateHamper_eSignCertPasswd(out hamp_eSgn);
      nextY = hampbelowGrid.Bottom + ZXC.QUN;
      
      CreateHamperFooter(out hampMemoFooter, restX);
      nextY = hampMemoFooter.Bottom + ZXC.QUN;

      CreateHamperFooter2(out hampMemoFooter2, restX);

   }
  
   private void CreateHamperHeader(out VvHamper hampMemoHeader, int _width)
   {
      hampMemoHeader = new VvHamper(2, 3, "", TheTabControl.TabPages[memo_TabPageName], false, nextX, nextY - ZXC.Qun8, 0);

      hampMemoHeader.VvColWdt      = new int[] { _width  , ZXC.Q5un };
      hampMemoHeader.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hampMemoHeader.VvRightMargin = hampMemoHeader.VvLeftMargin;
      
      hampMemoHeader.VvRowHgt       = new int[] { ZXC.QUN - ZXC.Qun4 , ZXC.QUN , (int)ZXC.GetPixeliOkomitiZaMilimetre(22) };
      hampMemoHeader.VvSpcBefRow    = new int[] { ZXC.Qun12, 0, 0};
      hampMemoHeader.VvBottomMargin = hampMemoHeader.VvTopMargin;

                        hampMemoHeader.CreateVvLabel               (0, 0, "MemoHeader:"    , ContentAlignment.MiddleLeft);
      rtbx_memoHeader = hampMemoHeader.CreateVvRichTextBox         (0, 2, "rtbx_memoHeader", "", GetDB_ColumnSize(DB_ci.memoHeader));
      ts_memoHeader   = hampMemoHeader.CreateVvRichTextBoxToolStrip(0, 1, "ts_memoHeader"  , rtbx_memoHeader, 1, 0);
   }

   private void CreateHamperBelowGrid(out VvHamper hampbelowGrid, int _width)
   {
      hampbelowGrid = new VvHamper(1, 6, "", TheTabControl.TabPages[memo_TabPageName], false, nextX, nextY, 0);

      hampbelowGrid.VvColWdt      = new int[] { _width};
      hampbelowGrid.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hampbelowGrid.VvRightMargin = hampbelowGrid.VvLeftMargin;
      
      hampbelowGrid.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN, ZXC.QUN , ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hampbelowGrid.VvSpcBefRow    = new int[] { ZXC.Qun4, 0      , 0       , 0      ,       0,        0};
      hampbelowGrid.VvBottomMargin = hampbelowGrid.VvTopMargin;

                       hampbelowGrid.CreateVvLabel               (0, 0, "BelowGrid:"    , ContentAlignment.MiddleLeft);
      rtbx_belowGrid = hampbelowGrid.CreateVvRichTextBox         (0, 2, "rtbx_belowGrid", "", GetDB_ColumnSize(DB_ci.belowGrid), 0, 3);
      ts_belowGrid   = hampbelowGrid.CreateVvRichTextBoxToolStrip(0, 1, "ts_belowGrid"  , rtbx_belowGrid);

      hampbelowGrid.Visible = false;
   }

   private void CreateHamperFooter(out VvHamper hampMemoFooter, int _width)
   {
      hampMemoFooter = new VvHamper(1, 5, "", TheTabControl.TabPages[memo_TabPageName], false, nextX, nextY, 0);

      hampMemoFooter.VvColWdt      = new int[] { _width };
      hampMemoFooter.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hampMemoFooter.VvRightMargin = hampMemoFooter.VvLeftMargin;

      hampMemoFooter.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hampMemoFooter.VvSpcBefRow    = new int[] { ZXC.Qun4, 0, 0, 0, 0};
      hampMemoFooter.VvBottomMargin = hampMemoFooter.VvTopMargin;

                        hampMemoFooter.CreateVvLabel               (0, 0, "MemoFooter:", ContentAlignment.MiddleLeft);
      rtbx_memoFooter = hampMemoFooter.CreateVvRichTextBox         (0, 2, "rtbx_memoFooter", "", GetDB_ColumnSize(DB_ci.memoFooter), 0, 2);
      ts_memoFooter   = hampMemoFooter.CreateVvRichTextBoxToolStrip(0, 1, "ts_memoFooter", rtbx_memoFooter);
   }

   private void CreateHamperFooter2(out VvHamper hampMemoFooter2, int _width)
   {
      hampMemoFooter2 = new VvHamper(1, 5, "", TheTabControl.TabPages[memo_TabPageName], false, nextX, nextY, 0);

      hampMemoFooter2.VvColWdt      = new int[] { _width };
      hampMemoFooter2.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hampMemoFooter2.VvRightMargin = hampMemoFooter2.VvLeftMargin;

      hampMemoFooter2.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN , ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hampMemoFooter2.VvSpcBefRow    = new int[] { ZXC.Qun4, 0, 0, 0, 0};
      hampMemoFooter2.VvBottomMargin = hampMemoFooter2.VvTopMargin;

                         hampMemoFooter2.CreateVvLabel               (0, 0, "MemoFooter2:", ContentAlignment.MiddleLeft);
      rtbx_memoFooter2 = hampMemoFooter2.CreateVvRichTextBox         (0, 2, "rtbx_memoFooter2", "", GetDB_ColumnSize(DB_ci.memoFooter2), 0, 2);
      ts_memoFooter2   = hampMemoFooter2.CreateVvRichTextBoxToolStrip(0, 1, "ts_memoFooter2", rtbx_memoFooter2);
   }

   private void CreatePicturBox()
   {
      picBox_theLogo          = new PictureBox();
      picBox_theLogo.Parent   = TheTabControl.TabPages[memo_TabPageName];
      picBox_theLogo.Location = new Point(nextX, nextY);
      picBox_theLogo.Size     = new Size(ZXC.Q10un, ZXC.Q6un);
      picBox_theLogo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

      // !!! 
      picBox_theLogo.SizeMode = PictureBoxSizeMode.Zoom;
   }

   private void CreatePicturBox2()
   {
      picBox_theLogo2          = new PictureBox();
      picBox_theLogo2.Parent   = TheTabControl.TabPages[memo_TabPageName];
      picBox_theLogo2.Location = new Point(picBox_theLogo.Left, picBox_theLogo.Bottom + ZXC.QUN);
      picBox_theLogo2.Size     = new Size(ZXC.Q10un, ZXC.Q6un);
      picBox_theLogo2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

      // !!! 
      picBox_theLogo2.SizeMode = PictureBoxSizeMode.Zoom;
   }

   private void CreateHamper_eSignCertPasswd(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 6, "", TheTabControl.TabPages[memo_TabPageName], false, hampMemoHeader.Left + ZXC.Q2un, nextY + ZXC.Q2un, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q8un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;



      lbl_eSgnNaslov      = hamper.CreateVvLabel  (0, 0, "Potpisni certifikat", 1, 0, ContentAlignment.MiddleRight);
      lbl_eSgnNaslov.Font = ZXC.vvFont.SmallBoldFont;                           
      
                           hamper.CreateVvLabel  (0, 1, "eSignCert Password:",       ContentAlignment.MiddleRight);
      tbx_eSgnCertPasswd = hamper.CreateVvTextBox(1, 1, "tbx_eSgnCertPasswd" , "eSignCert Password", GetDB_ColumnSize(DB_ci.certPasswd));
      
#if(DEBUG)
      // Kada hoces vidjeti userov password, samo komentiraj dolnju liniju. 
      //tbx_eSgnCertPasswd.JAM_IsPassword = true;
#else
      tbx_eSgnCertPasswd.JAM_IsPassword = true;
#endif

    //tbx_eSgnCertPasswd.JAM_PasswdField_TextChanged_Method = new EventHandler(tbx_eSgnCertPasswd_TextChanged);

      lbl_eSgnpasswd2         = hamper.CreateVvLabel  (0, 2, "Ponovljeni password:", ContentAlignment.MiddleRight);
      tbx_eSgnCertPasswd2     = hamper.CreateVvTextBox(1, 2, "tbx_passwd2", "Ponovljeni password", GetDB_ColumnSize(DB_ci.certPasswd));
      lbl_eSgnpasswd2.Tag     = tbx_eSgnCertPasswd2.Tag = "Visible_ only_ZXC.ZaUpis.Otvoreno";
      lbl_eSgnpasswd2.Visible = tbx_CertPasswd2.Visible = false;
      tbx_eSgnCertPasswd2.JAM_IsPassword = true;

      hamper.CreateVvLabel(0, 3, "Issuer:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 4, "Subject:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 5, "Expire:", ContentAlignment.MiddleRight);
      tbx_eSgnCertIssuer  = hamper.CreateVvTextBox(1, 3, "tbx_eSgnCertIssuer" , "Issuer:");
      tbx_eSgnCertSubject = hamper.CreateVvTextBox(1, 4, "tbx_eSgnCertSubject", "Subject:");
      tbx_eSgnCertExpire  = hamper.CreateVvTextBox(1, 5, "tbx_eSgnCertExpire" , "Expire:");

      tbx_eSgnCertIssuer .JAM_ReadOnly = true;
      tbx_eSgnCertSubject.JAM_ReadOnly = true;
      tbx_eSgnCertExpire .JAM_ReadOnly = true;

    //  this.Validating += new System.ComponentModel.CancelEventHandler(UserUC_Validating);

      hamper.Visible = false;
   }

   #endregion Tab_Memorandum

   #region DataGridView

   private void CreateDataGridView_InitializeTheGrid_ReadOnly_Columns()
   {
      aTransesGrid[0] = CreateDataGridView_ReadOnly(TheTabControl.TabPages[prvlg_TabPageName], "Privilegije_Prjkt");
      aTransesGrid[0].Dock = DockStyle.Fill;
      int minGridWIdth = InitializeTheGrid_ReadOnly_Columns();

      aTransesGrid[0].DoubleClick += new EventHandler(theFIRST_TransGrid_DoubleClick);
      aTransesGrid[0].KeyPress += new KeyPressEventHandler(theFIRST_TransGrid_KeyPress);
   }

   private int InitializeTheGrid_ReadOnly_Columns()
   {
      int sumOfColWidth = 0, colWidth;

      sumOfColWidth += aTransesGrid[0].RowHeadersWidth;
      colWidth = ZXC.Q4un;                            AddDGVColum_RecID_4GridReadOnly (aTransesGrid[0], "RecID"            , colWidth, true, 6);
      colWidth = ZXC.Q6un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(aTransesGrid[0], "User Name"        , colWidth, false);
      colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(aTransesGrid[0], ""                 , colWidth, false);
      colWidth = ZXC.Q7un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(aTransesGrid[0], "Privilegija"      , colWidth, false);
      colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(aTransesGrid[0], ""                 , colWidth, false);
      colWidth = ZXC.Q7un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(aTransesGrid[0], "Doseg privilegije", colWidth, true);
      colWidth = ZXC.Q3un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(aTransesGrid[0], ""                 , colWidth, false);
      colWidth = ZXC.Q7un; sumOfColWidth += colWidth; AddDGVColum_String_4GridReadOnly(aTransesGrid[0], "Dokument"         , colWidth, false);
      
      return sumOfColWidth;
   }

   protected override void theFIRST_TransGrid_DoubleClick(object sender, EventArgs e)
   {
      base.OpenNew_Record_TabPage_OnDoubleClick(ZXC.VvSubModulEnum.PRV, SelectedRecIDIn_FIRST_TransGrid);
   }


   #endregion DataGridView

   #region Filter

   public override void CreateRptFilterAndRptFilterUC()
   {
      ThePrvlgFilter = new VvRpt_Prvlg_Filter();

      ThePrvlgFilterUC = new PrvlgFilterUC(this);
      ThePrvlgFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = ThePrvlgFilterUC.Width;
   }

   #endregion Filter

   #region PutFields(), GetFields()

   #region Fld_XY

   public string Fld_SudCity
   {
      get { return tbx_SudCity.Text;         }
      set {        tbx_SudCity.Text = value; }
   }
    
   public string Fld_MbsRbu
   {
      get { return tbx_MbsRbu.Text;         }
      set {        tbx_MbsRbu.Text = value; }
   }

   public DateTime Fld_DateOsn 
   {
      get 
      { 
         return  ZXC.ValOr_01010001_DtpDateTime(dtp_DateOsn.Value);         
      }
      set
      {
         dtp_DateOsn.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DateOsn.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DateOsn);
      }
   }
     
   public decimal Fld_ToPayA // zaFakturiranje 
   {
      get { return tbx_ToPayA.GetDecimalField();      }
      set {        tbx_ToPayA.PutDecimalField(value); }
   }

   public decimal Fld_ToPayB // harac
   {
      get { return tbx_ToPayB.GetDecimalField();      }
      set {        tbx_ToPayB.PutDecimalField(value); }
   }

   public decimal Fld_ToPayC
   {
      get { return tbx_ToPayC.GetDecimalField();      }
      set {        tbx_ToPayC.PutDecimalField(value); }
   }

   public decimal Fld_TemKapit
   {
      get { return tbx_TemKapit.GetDecimalField();      }
      set {        tbx_TemKapit.PutDecimalField(value); }
   }

   public decimal Fld_PidSume
   {
      get { return tbx_PidSume.GetDecimalField();      }
      set {        tbx_PidSume.PutDecimalField(value); }
   }
   
   public decimal Fld_PidSRent
   {
      get { return tbx_PidSRent.GetDecimalField();      }
      set {        tbx_PidSRent.PutDecimalField(value); }
   }

   public decimal Fld_PidTurstPidSRent
   {
      get { return tbx_PidTurst.GetDecimalField();      }
      set {        tbx_PidTurst.PutDecimalField(value); }
   }
   
   public decimal Fld_PidDobit
   {
      get { return tbx_PidDobit.GetDecimalField();      }
      set {        tbx_PidDobit.PutDecimalField(value); }
   }
   
   public decimal Fld_PidKmDopr
   {
      get { return tbx_PidKmDopr.GetDecimalField();      }
      set {        tbx_PidKmDopr.PutDecimalField(value); }
   }
   
   public decimal Fld_PidKmClan
   {
      get { return tbx_PidKmClan.GetDecimalField();      }
      set {        tbx_PidKmClan.PutDecimalField(value); }
   }

   public decimal Fld_PidMO1
   {
      get { return tbx_PidMO1.GetDecimalField();      }
      set {        tbx_PidMO1.PutDecimalField(value); }
   }
   
   public decimal Fld_PidMO2
   {
      get { return tbx_PidMO2.GetDecimalField();      }
      set {        tbx_PidMO2.PutDecimalField(value); }
   }
   
   public decimal Fld_PidZdr
   {
      get { return tbx_PidZdr.GetDecimalField();      }
      set {        tbx_PidZdr.PutDecimalField(value); }
   }
   
   public decimal Fld_PidZor
   {
      get { return tbx_PidZor.GetDecimalField();      }
      set {        tbx_PidZor.PutDecimalField(value); }
   }
   
   public bool Fld_IsSkip
   {
      get { return cbx_IsSkip.Checked;         }
      set {        cbx_IsSkip.Checked = value; }
   }

   public bool Fld_IsOver20
   {
      get { return cbx_IsOver20.Checked;         }
      set {        cbx_IsOver20.Checked = value; }
   }

   public bool Fld_IsZorNa
   {
      get { return cbx_IsZorNa.Checked;         }
      set {        cbx_IsZorNa.Checked = value; }
   }

   public bool Fld_IsJednK
   {
      get { return cbx_IsJednK.Checked;         }
      set {        cbx_IsJednK.Checked = value; }
   }

   public bool Fld_IsDobit
   {
      get { return cbx_IsDobit.Checked;         }
      set {        cbx_IsDobit.Checked = value; }
   }

   public bool Fld_IsAuthn
   {
      get { return cbx_IsAuthn.Checked;         }
      set {        cbx_IsAuthn.Checked = value; }
   }

   public bool Fld_IrSkipKolFree
   {
      get { return cbx_IsIR.Checked;         }
      set {        cbx_IsIR.Checked = value; }
   }

   public bool Fld_UrSkipKolFree
   {
      get { return cbx_IsUr.Checked; }
      set {        cbx_IsUr.Checked = value; }
   }
  
   public bool Fld_IsDeviznoMat
   {
      get { return cbx_IsDevMat.Checked;         }
      set {        cbx_IsDevMat.Checked = value; }
   }

   public string Fld_MemoHeader
   {
      get { return rtbx_memoHeader.Rtf; }
      set
      {
         // ... ovo je zbog nekog vec dokumentiranog bug-a na MSDN-u u vezi 'gubljenja' zeljenog ZoomFactor-a 
         rtbx_memoHeader.Clear();
         rtbx_memoHeader.Rtf = value;
         rtbx_memoHeader.ZoomFactor = 1.00f;
         rtbx_memoHeader.ZoomFactor = ((VvRichTextBoxToolStrip)(rtbx_memoHeader.Tag)).PrefferedZoomFactor;
      }
   }
   public string Fld_MemoFooter
   {
      get { return rtbx_memoFooter.Rtf; }
      set
      {
         // ... ovo je zbog nekog vec dokumentiranog bug-a na MSDN-u u vezi 'gubljenja' zeljenog ZoomFactor-a 
         rtbx_memoFooter.Clear();
         rtbx_memoFooter.Rtf = value;
         rtbx_memoFooter.ZoomFactor = 1.00f;
         rtbx_memoFooter.ZoomFactor = ((VvRichTextBoxToolStrip)(rtbx_memoFooter.Tag)).PrefferedZoomFactor;
      }
   }
   public string Fld_MemoFooter2
   {
      get { return rtbx_memoFooter2.Rtf; }
      set
      {
         // ... ovo je zbog nekog vec dokumentiranog bug-a na MSDN-u u vezi 'gubljenja' zeljenog ZoomFactor-a 
         rtbx_memoFooter2.Clear();
         rtbx_memoFooter2.Rtf = value;
         rtbx_memoFooter2.ZoomFactor = 1.00f;
         rtbx_memoFooter2.ZoomFactor = ((VvRichTextBoxToolStrip)(rtbx_memoFooter2.Tag)).PrefferedZoomFactor;
      }
   }

   public string Fld_BelowGrid
   {
      get { return rtbx_belowGrid.Rtf; }
      set
      {
         // ... ovo je zbog nekog vec dokumentiranog bug-a na MSDN-u u vezi 'gubljenja' zeljenog ZoomFactor-a 
         rtbx_belowGrid.Clear();
         rtbx_belowGrid.Rtf = value;
         rtbx_belowGrid.ZoomFactor = 1.00f;
         rtbx_belowGrid.ZoomFactor = ((VvRichTextBoxToolStrip)(rtbx_belowGrid.Tag)).PrefferedZoomFactor;
      }
   }

   public Image Fld_TheLogo
   {
      set 
      {
         picBox_theLogo.Image = value;
      }
   }
   public Image Fld_TheLogo2
   {
      set
      {
         picBox_theLogo2.Image = value;
      }
   }

   public bool Fld_IsPriBindNar
   {
      get { return cbx_IsPriNarBind.Checked; }
      set {        cbx_IsPriNarBind.Checked = value; }
   }

   //public bool Fld_IsNoMinus
   //{
   //   get { return cbx_IsNoMinus.Checked; }
   //   set {        cbx_IsNoMinus.Checked = value; }
   //}

   public ZXC.MinusPolicy Fld_IsNoMinus
   {
      get
      {
              if(rbt_allowAll    .Checked) return ZXC.MinusPolicy.ALLOW_ALL         ;
         else if(rbt_alowAllNoMsg.Checked) return ZXC.MinusPolicy.ALOW_ALL_NO_MSG   ;
         else if(rbt_denyAll     .Checked) return ZXC.MinusPolicy.DENY_ALL          ;
         else if(rbt_denyVelOnly .Checked) return ZXC.MinusPolicy.DENY_VEL_ALLOW_MAL;
              else throw new Exception("Fld_IsNoMinus: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.MinusPolicy.ALLOW_ALL         : rbt_allowAll    .Checked = true; break;
            case ZXC.MinusPolicy.ALOW_ALL_NO_MSG   : rbt_alowAllNoMsg.Checked = true; break;
            case ZXC.MinusPolicy.DENY_ALL          : rbt_denyAll     .Checked = true; break;
            case ZXC.MinusPolicy.DENY_VEL_ALLOW_MAL: rbt_denyVelOnly .Checked = true; break;
         }
      }
   }

   public string Fld_PorezIspostCd
   {
      get { return tbx_IspostavaCd.Text;         }
      set {        tbx_IspostavaCd.Text = value; }
   }
   
   public string Fld_PorezIspostName
   {
      get { return tbx_IspostavaName.Text;         }
      set {        tbx_IspostavaName.Text = value; }
   }

   public DateTime Fld_RvrOd
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DateRadStart.Value);
      }
      set
      {
         dtp_DateRadStart.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DateRadStart.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DateRadStart);
      }
   }
   public DateTime Fld_RvrDo
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DateRadEnd.Value);
      }
      set
      {
         dtp_DateRadEnd.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DateRadEnd.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DateRadEnd);
      }
   }

   public bool Fld_IsFiskalOnline
   {
      get { return cbx_IsFiskalOnline.Checked; }
      set {        cbx_IsFiskalOnline.Checked = value; }
   }
   public bool Fld_IsNoAutoFiskal
   {
      get { return cbx_IsNoAutoFisk.Checked; }
      set {        cbx_IsNoAutoFisk.Checked = value; }
   }

   public bool Fld_IsNoTtNumChk
   {
      get { return cbx_IsNoTtNumChk.Checked; }
      set {        cbx_IsNoTtNumChk.Checked = value; }
   }
 
   public string Fld_CertPasswd   { get { return tbx_CertPasswd .Text; }  set { tbx_CertPasswd .Text = value; } }
   public string Fld_CertPasswd2  { get { return tbx_CertPasswd2.Text; }  set { tbx_CertPasswd2.Text = value; } }
   public string Fld_FiskTtOnly   { get { return tbx_fiskTtOnly .Text; }  set { tbx_fiskTtOnly .Text = value; } }

   public bool Fld_IsFiskCashOnly { get { return cbx_isFiskCashOnly.Checked; } set { cbx_isFiskCashOnly.Checked = value; } }
   public bool Fld_IsNeprofit     { get { return cbx_isNeprofit    .Checked; } set { cbx_isNeprofit    .Checked = value; } }

   public string Fld_SkySrvrHost  { get { return tbx_skySrvrHost.Text; }  set { tbx_skySrvrHost.Text = value; } }
   public string Fld_SkyPassword  { get { return tbx_skyPassword.Text; }  set { tbx_skyPassword.Text = value; } }
   public string Fld_SkyVvDomena  { get { return tbx_skyVvDomena.Text; }  set { tbx_skyVvDomena.Text = value; } }

   public decimal Fld_VrKoefBr1   { get { return tbx_vrKoefBruto1.GetDecimalField(); } set { tbx_vrKoefBruto1.PutDecimalField(value); } }

   public bool Fld_IsObustOver3   { get { return cbx_isObustOver3  .Checked; } set { cbx_isObustOver3  .Checked = value; } }
   public bool Fld_IsCheckStaz    { get { return cbx_isCheckStaz   .Checked; } set { cbx_isCheckStaz   .Checked = value; } }
   public bool Fld_IsObrStazaLast { get { return cbx_isObrStazaLast.Checked; } set { cbx_isObrStazaLast.Checked = value; } }
   public bool Fld_IsSkipStzOnBol { get { return cbx_isSkipStzOnBol.Checked; } set { cbx_isSkipStzOnBol.Checked = value; } }
   public bool Fld_IsFullStzOnPol { get { return cbx_isFullStzOnPol.Checked; } set { cbx_isFullStzOnPol.Checked = value; } }
   public bool Fld_IsBtchBookg    { get { return cbx_isBtchBookg   .Checked; } set { cbx_isBtchBookg   .Checked = value; } }

   public string Fld_RnoRkp       { get { return tbx_rnoRkp.Text; } set { tbx_rnoRkp.Text = value; }  }

   public bool Fld_ShouldPeriodLock { get { return cbx_ShouldPeriodLock.Checked; } set { cbx_ShouldPeriodLock.Checked = value; } }
   public uint Fld_PeriodLockDay    { get { return tbx_PeriodLockDay.GetUintField(); } set { tbx_PeriodLockDay.PutUintField(value); } }

   public bool Fld_ShouldPrevYearLock { get { return cbx_ShouldPrevYearLock.Checked; } set { cbx_ShouldPrevYearLock.Checked = value; } }

   public string Fld_FiskCertIssuer  {  set { tbx_fiskCertIssuer .Text = value; } }
   public string Fld_FiskCertSubject {  set { tbx_fiskCertSubject.Text = value; } }
   public string Fld_FiskCertExpire  {  set { tbx_fiskCertExpire .Text = value; } }
   public string Fld_ESgnCertPasswd   { get { return tbx_eSgnCertPasswd .Text; }  set { tbx_eSgnCertPasswd .Text = value; } }
   public string Fld_eSgnCertPasswd2  { get { return tbx_eSgnCertPasswd2.Text; }  set { tbx_eSgnCertPasswd2.Text = value; } }
   public string Fld_ESgnCertIssuer  {  set { tbx_eSgnCertIssuer .Text = value; } }
   public string Fld_ESgnCertSubject {  set { tbx_eSgnCertSubject.Text = value; } }
   public string Fld_ESgnCertExpire  {  set { tbx_eSgnCertExpire .Text = value; } }
   public string Fld_M2PshaSec { get { return tbx_m2pShaSec.Text; } set { tbx_m2pShaSec.Text = value; } }
   public string Fld_M2Papikey { get { return tbx_m2pApikey.Text; } set { tbx_m2pApikey.Text = value; } }
   public string Fld_M2Pserno  { get { return tbx_m2pSerno .Text; } set { tbx_m2pSerno .Text = value; } }
   public string Fld_M2Pmodel  { get { return tbx_m2pModel .Text; } set { tbx_m2pModel .Text = value; } }
   public ZXC.F2_Provider_enum Fld_F2_Provider
   {
      get
      {
              if(rbt_F2_none.Checked) return ZXC.F2_Provider_enum.UNKNOWN;
         else if(rbt_F2_MER .Checked) return ZXC.F2_Provider_enum.MER;
         else if(rbt_F2_PND .Checked) return ZXC.F2_Provider_enum.PND;
              else throw new Exception("Fld_F2_Provider: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.F2_Provider_enum.UNKNOWN: rbt_F2_none.Checked = true; break;
            case ZXC.F2_Provider_enum.MER : rbt_F2_MER .Checked = true; break;
            case ZXC.F2_Provider_enum.PND : rbt_F2_PND .Checked = true; break;
         }
      }
   }

   public ZXC.F2_RolaKind Fld_F2_RolaKind
   {
      get
      {
              if(rbt_NEMA_F2                          .Checked) return ZXC.F2_RolaKind.NEMA_F2                          ;
         else if(rbt_VlastitoKnjigovodstvo_F2_ALL     .Checked) return ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_ALL     ;
         else if(rbt_VlastitoKnjigovodstvo_F2_FUR_Only.Checked) return ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_FUR_Only;
         else if(rbt_KlijentServisa_TipA              .Checked) return ZXC.F2_RolaKind.KlijentServisa_TipA              ;
         else if(rbt_KlijentServisa_TipB              .Checked) return ZXC.F2_RolaKind.KlijentServisa_TipB              ;
         else if(rbt_KlijentServisa_TipC              .Checked) return ZXC.F2_RolaKind.KlijentServisa_TipC              ;
         else if(rbt_IMA_B2B_Virman_NEMA_F2           .Checked) return ZXC.F2_RolaKind.IMA_B2B_Virman_NEMA_F2           ;
         else throw new Exception("Fld_F2_RolaKind: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.F2_RolaKind.NEMA_F2                          : rbt_NEMA_F2                          .Checked = true; break;
            case ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_ALL     : rbt_VlastitoKnjigovodstvo_F2_ALL     .Checked = true; break;
            case ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_FUR_Only: rbt_VlastitoKnjigovodstvo_F2_FUR_Only.Checked = true; break;
            case ZXC.F2_RolaKind.KlijentServisa_TipA              : rbt_KlijentServisa_TipA              .Checked = true; break;
            case ZXC.F2_RolaKind.KlijentServisa_TipB              : rbt_KlijentServisa_TipB              .Checked = true; break;
            case ZXC.F2_RolaKind.KlijentServisa_TipC              : rbt_KlijentServisa_TipC              .Checked = true; break;
            case ZXC.F2_RolaKind.IMA_B2B_Virman_NEMA_F2           : rbt_IMA_B2B_Virman_NEMA_F2           .Checked = true; break;
         }
      }
   }

   public string Fld_F2_UserName { get { return tbx_F2_UserName.Text; }  set { tbx_F2_UserName.Text = value; } }
   public string Fld_F2_Password { get { return tbx_F2_Password.Text; }  set { tbx_F2_Password.Text = value; } }

   #endregion Fld_XY

   #region Classic PutFileds(), GetFields()

   public override void PutFields(VvDataRecord prjkt)
   {
      prjkt_rec = (Prjkt)prjkt;

      if(prjkt_rec != null)
      {
         base.PutFields(prjkt_rec);
    
         VvHamper.SetChkBoxRadBttnAutoCheck(this, true);

         Fld_SudCity          = prjkt_rec.SudCity;
         Fld_MbsRbu           = prjkt_rec.MbsRbu;
         Fld_DateOsn          = prjkt_rec.DateOsn;
         Fld_ToPayA           = prjkt_rec.ToPayA;
         Fld_ToPayB           = prjkt_rec.ToPayB;
         Fld_ToPayC           = prjkt_rec.ToPayC;
         Fld_TemKapit         = prjkt_rec.TemKapit;
         Fld_PidSume          = prjkt_rec.PidSume;
         Fld_PidSRent         = prjkt_rec.PidSRent;
         Fld_PidTurstPidSRent = prjkt_rec.PidTurst;
         Fld_PidDobit         = prjkt_rec.PidDobit;
         Fld_PidKmDopr        = prjkt_rec.PidKmDopr;
         Fld_PidKmClan        = prjkt_rec.PidKmClan;
         Fld_PidMO1           = prjkt_rec.PidMO1;
         Fld_PidMO2           = prjkt_rec.PidMO2;
         Fld_PidZdr           = prjkt_rec.PidZdr;
         Fld_PidZor           = prjkt_rec.PidZor;
         Fld_IsSkip           = prjkt_rec.IsSkip  ;
         Fld_IsOver20         = prjkt_rec.IsOver20;
         Fld_IsZorNa          = prjkt_rec.IsZorNa ;
         Fld_IsJednK          = prjkt_rec.IsJednK ;
         Fld_IsDobit          = prjkt_rec.IsDobit ;
         Fld_IsAuthn          = prjkt_rec.IsAuthn ;
         Fld_IrSkipKolFree    = prjkt_rec.IrSkipKolStSkl;
         Fld_UrSkipKolFree    = prjkt_rec.UrSkipKolStSkl;
         Fld_IsDeviznoMat     = prjkt_rec.IsDevizno ;
         
         Fld_MemoHeader       = prjkt_rec.MemoHeader ;
         Fld_MemoFooter       = prjkt_rec.MemoFooter ;
         Fld_MemoFooter2      = prjkt_rec.MemoFooter2;
         Fld_TheLogo          = prjkt_rec.TheLogoImage;

         Fld_TheLogo2         = prjkt_rec.TheLogoImage2;

#if DEBUG
         //string bcStr = "0000046100115";
         //BarcodeLib.Barcode barcode = new BarcodeLib.Barcode(/*"30583306", BarcodeLib.TYPE.EAN8*/);
         //barcode.IncludeLabel = true;
         //barcode.AlternateLabel = bcStr;
         //Image barcodeImage = barcode.Encode(BarcodeLib.TYPE.EAN13, bcStr, Color.DarkBlue, Color.LightBlue, 100, 100);
         //Fld_TheLogo2 = barcodeImage;
         //Fld_TheLogo2 = ZXC.Get_Faktur_EAN8_Image(new Faktur() { TtSort = 46, TtNum = 100115} );
#endif

         Fld_BelowGrid = prjkt_rec.BelowGrid;

         Fld_IsNoMinus        = prjkt_rec.MinusPolicy/*IsNoMinus*/;
         Fld_PorezIspostCd    = prjkt_rec.PorezIspostCD;
         Fld_IsPriBindNar     = prjkt_rec.IsChkPrKol;

         Fld_RvrOd            = prjkt_rec.RvrOd;
         Fld_RvrDo            = prjkt_rec.RvrDo;
         Fld_IsFiskalOnline   = prjkt_rec.IsFiskalOnline;
         Fld_IsNoTtNumChk     = prjkt_rec.IsNoTtNumChk;

         Fld_PorezIspostName  = ZXC.luiListaIspostava.GetNameForThisCd(prjkt_rec.PorezIspostCD);
                              
         Fld_CertPasswd       = prjkt_rec.CertPasswdDecrypted;
         Fld_IsFiskCashOnly   = prjkt_rec.IsFiskCashOnly2026;
         Fld_FiskTtOnly       = prjkt_rec.FiskTtOnly    ;
         Fld_IsNeprofit       = prjkt_rec.IsNeprofit    ;
                              
       //Fld_SkySrvrHost      = prjkt_rec.SkySrvrHost;
         Fld_SkySrvrHost      = prjkt_rec.SkySrvrHostDecrypted;
       //Fld_SkyPassword      = prjkt_rec.SkyPassword;
         Fld_SkyPassword      = prjkt_rec.SkyPasswordDecrypted;
         Fld_SkyVvDomena      = prjkt_rec.SkyVvDomena;

         Fld_VrKoefBr1        = prjkt_rec.VrKoefBr1;

         Fld_IsObustOver3     = prjkt_rec.IsObustOver3  ;
         Fld_IsCheckStaz      = prjkt_rec.IsCheckStaz   ;
         Fld_IsObrStazaLast   = prjkt_rec.IsObrStazaLast;
         Fld_IsSkipStzOnBol   = prjkt_rec.IsSkipStzOnBol;
         Fld_IsFullStzOnPol   = prjkt_rec.IsFullStzOnPol;
         Fld_IsBtchBookg      = prjkt_rec.IsBtchBookg   ;
         
         Fld_RnoRkp           = prjkt_rec.RnoRkp;

         Fld_ShouldPeriodLock = prjkt_rec.ShouldPeriodLock;
         Fld_PeriodLockDay    = prjkt_rec.PeriodLockDay   ;

         Fld_FiskCertIssuer   = prjkt_rec.FiskalCertifikat_Issuer;
         Fld_FiskCertSubject  = prjkt_rec.FiskalCertifikat_Subject;
         Fld_FiskCertExpire   = prjkt_rec.FiskalCertifikat_Expire;

         //DateTime certExpDate = prjkt_rec.FiskalCertifikat_ExpireD;

         //Fld_ESgnCertPasswd   = prjkt_rec.ESgnCertPasswdDecrypted;
         //Fld_ESgnCertIssuer   = prjkt_rec.ESgnCertifikat_Issuer;
         //Fld_ESgnCertSubject  = prjkt_rec.ESgnCertifikat_Subject;
         //Fld_ESgnCertExpire   = prjkt_rec.ESgnCertifikat_Expire;

         Fld_IsNoAutoFiskal = prjkt_rec.IsNoAutoFiskal;

         Fld_M2PshaSec = prjkt_rec.M2PshaSecDecrypted;
         Fld_M2Papikey = prjkt_rec.M2PapikeyDecrypted;
         Fld_M2Pserno  = prjkt_rec.M2Pserno ;
         Fld_M2Pmodel  = prjkt_rec.M2Pmodel ;

         Fld_F2_Provider = prjkt_rec.F2_Provider;
         Fld_F2_RolaKind = prjkt_rec.F2_RolaKind;
         Fld_F2_UserName = prjkt_rec.F2_UserName;
         Fld_F2_Password = prjkt_rec.F2_PasswordDecrypted;

         InitializeFilterUCFields();

         recordReportLoaded = false;
         DecideIfShouldLoad_VvReport(null, null, null);

         aTransesLoaded[0] = false; // ovdje treba nulirati sve postojece 'xyLoaded' varijable
         DecideIfShouldLoad_TransDGV(null, null, null);
      }

      VvHamper.SetChkBoxRadBttnAutoCheck(this, false);

   }

   public override void GetFields(bool fuse)
   {

      base.GetFields(fuse);

      if(prjkt_rec == null) prjkt_rec = new Prjkt();
      prjkt_rec.SudCity          = Fld_SudCity;
      prjkt_rec.MbsRbu           = Fld_MbsRbu;
      prjkt_rec.DateOsn          = Fld_DateOsn;
      prjkt_rec.ToPayA           = Fld_ToPayA;
      prjkt_rec.ToPayB           = Fld_ToPayB;
      prjkt_rec.ToPayC           = Fld_ToPayC;
      prjkt_rec.TemKapit         = Fld_TemKapit;
      prjkt_rec.PidSume          = Fld_PidSume;
      prjkt_rec.PidSRent         = Fld_PidSRent;
      prjkt_rec.PidTurst         = Fld_PidTurstPidSRent;
      prjkt_rec.PidDobit         = Fld_PidDobit;
      prjkt_rec.PidKmDopr        = Fld_PidKmDopr;
      prjkt_rec.PidKmClan        = Fld_PidKmClan;
      prjkt_rec.PidMO1           = Fld_PidMO1;
      prjkt_rec.PidMO2           = Fld_PidMO2;
      prjkt_rec.PidZdr           = Fld_PidZdr;
      prjkt_rec.PidZor           = Fld_PidZor;
      prjkt_rec.IsSkip           = Fld_IsSkip;
      prjkt_rec.IsOver20         = Fld_IsOver20;
      prjkt_rec.IsZorNa          = Fld_IsZorNa;
      prjkt_rec.IsJednK          = Fld_IsJednK;
      prjkt_rec.IsDobit          = Fld_IsDobit;
      prjkt_rec.IsAuthn          = Fld_IsAuthn;
      prjkt_rec.IsDevizno        = Fld_IsDeviznoMat;
      prjkt_rec.IrSkipKolStSkl   = Fld_IrSkipKolFree ;
      prjkt_rec.UrSkipKolStSkl   = Fld_UrSkipKolFree ;

      prjkt_rec.MemoHeader       = Fld_MemoHeader;
      prjkt_rec.MemoFooter       = Fld_MemoFooter;
      prjkt_rec.MemoFooter2      = Fld_MemoFooter2;
      prjkt_rec.BelowGrid        = Fld_BelowGrid;
     
      prjkt_rec.MinusPolicy/*IsNoMinus*/ = Fld_IsNoMinus ;
      prjkt_rec.PorezIspostCD    = Fld_PorezIspostCd ;
      prjkt_rec.IsChkPrKol       = Fld_IsPriBindNar ;

      prjkt_rec.RvrOd            = Fld_RvrOd;
      prjkt_rec.RvrDo            = Fld_RvrDo;
      prjkt_rec.IsFiskalOnline   = Fld_IsFiskalOnline;
      prjkt_rec.IsNoTtNumChk     = Fld_IsNoTtNumChk;

      prjkt_rec.IsFiskCashOnly   = /*Fld_IsFiskCashOnly*/false;
      prjkt_rec.FiskTtOnly       = Fld_FiskTtOnly    ;
      prjkt_rec.IsNeprofit       = Fld_IsNeprofit    ;
                                 
      prjkt_rec.SkyVvDomena      = Fld_SkyVvDomena;
      prjkt_rec.VrKoefBr1        = Fld_VrKoefBr1;

      prjkt_rec.IsObustOver3     = Fld_IsObustOver3  ;
      prjkt_rec.IsCheckStaz      = Fld_IsCheckStaz   ;
      prjkt_rec.IsObrStazaLast   = Fld_IsObrStazaLast;
      prjkt_rec.IsSkipStzOnBol   = Fld_IsSkipStzOnBol;
      prjkt_rec.IsFullStzOnPol   = Fld_IsFullStzOnPol;
      prjkt_rec.IsBtchBookg      = Fld_IsBtchBookg   ;

      prjkt_rec.RnoRkp           = Fld_RnoRkp;
      prjkt_rec.ShouldPeriodLock = Fld_ShouldPeriodLock;
      prjkt_rec.PeriodLockDay    = Fld_PeriodLockDay;

      prjkt_rec.IsNoAutoFiskal   = Fld_IsNoAutoFiskal ;

      prjkt_rec.CertPasswdEncodedAsInFile = ZXC.EncryptThis_LoginForm_Data(Fld_CertPasswd);
      
      if(prjkt_rec.CertPasswdEncodedAsInFile.Length > GetDB_ColumnSize(DB_ci.certPasswd))
      {
         ZXC.RaiseErrorProvider(tbx_CertPasswd, "Password PREDUGACAK!");
         prjkt_rec.CertPasswdEncodedAsInFile = Fld_CertPasswd = "";
      }

      prjkt_rec.SkyPasswordEncodedAsInFile = ZXC.EncryptThis_LoginForm_Data(Fld_SkyPassword);
      
      if(prjkt_rec.SkyPasswordEncodedAsInFile.Length > GetDB_ColumnSize(DB_ci.skyPassword))
      {
         ZXC.RaiseErrorProvider(tbx_skyPassword, "Password PREDUGACAK!");
         prjkt_rec.SkyPasswordEncodedAsInFile = Fld_SkyPassword = "";
      }

      prjkt_rec.SkySrvrHostEncodedAsInFile = ZXC.EncryptThis_LoginForm_Data(Fld_SkySrvrHost);

      if(prjkt_rec.SkySrvrHostEncodedAsInFile.Length > GetDB_ColumnSize(DB_ci.skySrvrHost))
      {
         ZXC.RaiseErrorProvider(tbx_skySrvrHost, "SrvrHost PREDUGACAK!");
         prjkt_rec.SkySrvrHostEncodedAsInFile = Fld_SkySrvrHost = "";
      }

      //prjkt_rec.SkySrvrHost = Fld_SkySrvrHost;
      //prjkt_rec.SkyPassword = Fld_SkyPassword;
   
       prjkt_rec.M2PshaSecEncodedAsInFile = ZXC.EncryptThis_LoginForm_Data(Fld_M2PshaSec);
       prjkt_rec.M2PapikeyEncodedAsInFile = ZXC.EncryptThis_LoginForm_Data(Fld_M2Papikey);
       prjkt_rec.M2Pserno  = Fld_M2Pserno ;
       prjkt_rec.M2Pmodel  = Fld_M2Pmodel ;

       prjkt_rec.F2_Provider  = Fld_F2_Provider;
       prjkt_rec.F2_RolaKind  = Fld_F2_RolaKind;
       prjkt_rec.F2_UserName  = Fld_F2_UserName;
       prjkt_rec.F2_PasswordEncodedAsInFile  = ZXC.EncryptThis_LoginForm_Data(Fld_F2_Password);

   }

   #endregion Classic PutFileds(), GetFields()

   #region Put Trans DGV Fileds

   private const string prvlg_TabPageName = "Privilegije";
  
   private void InitializeFilterUCFields()
   {
      SetFilterRecordDependentDefaults(); 
      ThePrvlgFilterUC.PutFilterFields(ThePrvlgFilter);
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
      VvLookUpLista documNamesList;

      ThePrvlgFilterUC.GetFilterFields();
      ThePrvlgFilterUC.AddFilterMemberz(ThePrvlgFilter, null);

      if(prjkt_rec.Privileges == null) prjkt_rec.Privileges = new List<Prvlg>();
      else                             prjkt_rec.Privileges.Clear();

      VvDaoBase.LoadGenericVvDataRecordList<Prvlg>(TheDbConnection, prjkt_rec.Privileges, ThePrvlgFilter.FilterMembers, "prjktTick, recID");

      aTransesLoaded[0] = true;

      aTransesGrid[0].Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(aTransesGrid[0]);
      
      foreach(Prvlg prvlg_rec in prjkt_rec.Privileges)
      {
         aTransesGrid[0].Rows.Add();

         rowIdx = aTransesGrid[0].RowCount - idxCorrector;

         aTransesGrid[0][0, rowIdx].Value = prvlg_rec.RecID;
         aTransesGrid[0][1, rowIdx].Value = prvlg_rec.UserName;
         aTransesGrid[0][2, rowIdx].Value = prvlg_rec.PrvlgType;
         aTransesGrid[0][3, rowIdx].Value = ZXC.luiListaPrvlgType.GetNameForThisCd(prvlg_rec.PrvlgType);
         aTransesGrid[0][4, rowIdx].Value = prvlg_rec.PrvlgScope;
         aTransesGrid[0][5, rowIdx].Value = ZXC.luiListaPrvlgScope.GetNameForThisCd(prvlg_rec.PrvlgScope);
         aTransesGrid[0][6, rowIdx].Value = prvlg_rec.DocumType;

         if(PrvlgUC.IsThis_PrvlgScopeCd_OneDocumScope(prvlg_rec.PrvlgScope))
         {
            documNamesList = ZXC.TheVvForm.DocumentTypeLookUpListaForThisModulEnum(PrvlgUC.ModulFromPrvlgScopeCd_asList(prvlg_rec.PrvlgScope));
            aTransesGrid[0][7, rowIdx].Value = documNamesList.GetNameForThisCd(prvlg_rec.DocumType);
         }
         else
         {
            aTransesGrid[0][7, rowIdx].Value = "";
         }

         aTransesGrid[0].Rows[rowIdx].HeaderCell.Value = (prjkt_rec.Privileges.Count - rowIdx).ToString();
      }
   }

   #endregion Put Trans DGV Fileds

   #endregion PutFields(), GetFields()

   #region override

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.prjkt_rec; }
      set { this.prjkt_rec = (Prjkt)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.PrjktDao; }
   }

   #region VvFindDialog

   public override VvFindDialog CreateVvFindDialog()
   {
      return CreateFind_Prjkt_Dialog();
   }

   public static VvFindDialog CreateFind_Prjkt_Dialog()
   {
      VvSubModul vvSubModul   = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsPRJ);
      VvDataRecord      vvDataRecord = ZXC.TheVvForm.CreateTheVvDataRecord_SwitchSubModulEnum(vvSubModul);

      VvFindDialog vvFindDialog = new VvFindDialog();
      VvRecLstUC vvRecListUC    = new PrjktListUC(vvFindDialog, (Prjkt)vvDataRecord, vvSubModul);
      vvFindDialog.TheRecListUC = vvRecListUC;

      return vvFindDialog;
   }

   #endregion VvFindDialog

   #region PrintSifrarRecord

   public VvRpt_Prvlg_Filter ThePrvlgFilter { get; set; }
   
   public PrvlgFilterUC ThePrvlgFilterUC { get; set; }
   
   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.ThePrvlgFilter;
      }
   }
  
   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.ThePrvlgFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
      ThePrvlgFilter.PrjktNaziv =  prjkt_rec.Naziv;
      ThePrvlgFilter.PrjktTick  =  prjkt_rec.Ticker;
      ThePrvlgFilter.PrjktID    =  prjkt_rec.KupdobCD/*RecID*/;
   }
   /* report
   public RptF_KKonta TheRptF_KKonta { get; set; }

   public override VvReport VirtualReport
   {
      get
      {
         return this.TheRptF_KKonta;
      }
   }

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
   */
   #endregion PrintSifrarRecord

   #endregion override

   #region SubModulSet Menu&ToolStrip Actions ... eg. Close47()

   public void CreateDB_OnClick(object sender, EventArgs e)
   {
      CreateDB(sender, e);
   }

   public bool CreateDB(object sender, EventArgs e)
   {
      bool OK;
      Prjkt prjkt_rec = VirtualDataRecord as Prjkt;

      string _projectYear = sender.ToString();

      DialogResult result =
         MessageBox.Show("Potvrdite inicijalizaciju DATABASE-a za projekt \'" + prjkt_rec.ToString() + "\' u godini " + _projectYear + "!?",
            "Novi komplet datoteka u godini " + _projectYear + "?!", 
            MessageBoxButtons.YesNo, 
            MessageBoxIcon.Question);

      if(result != DialogResult.Yes) return false;

      OK = VvSQL.CREATE_DATABASE(TheVvTabPage.TheDbConnection, ZXC.VvDB_NameConstructor(_projectYear, prjkt_rec.Ticker, prjkt_rec.KupdobCD/*RecID*/));

      if(!OK) return false;

      // 13.12.2022: dodan if
      if(_projectYear == ZXC.projectYear) // refreshaj samo ako je ista godina (npr. kod Init_NY NIJE ista godina)
      {
         ZXC.TheVvForm.RefreshDataBasesInfo();
      }

      ZXC.TheVvForm.SubModulSet_EnableOrDisable_TSButtonsAndTSMnItems();

      ZXC.aim_emsg("Inicijalizacija DATABASE-a je uspješno izvršena.");

      return true;
   }

   public void ActivateDB_OnClick(object sender, EventArgs e)
   {
      Prjkt prjkt_rec = VirtualDataRecord as Prjkt;

      ZXC.TheVvForm.InitializeComboBox4Project(prjkt_rec.Ticker, prjkt_rec.KupdobCD/*RecID*/, false);
   }

   #endregion SubModulSet Specific Menu&ToolStrip Actions ... eg. Close47()

   #region Update_VvDataRecord (Legacy naming convention)

   /// <summary>
   /// 'FindVvDataRecord' procedura. Inicirana:
   /// 1. Context menu (Mouse right click)
   /// 2. Mouse click (Ctrl ili Alt click)
   /// 3. Keyboard initiated (Ctrl/Alt + F/Space)
   /// </summary>
   /// <param name="startValue"></param>
   /// <returns></returns>
   public static object Update_Prjkt(VvSQL.SorterType whichInformation, object startValue)
   {
      Prjkt prjkt_rec = new Prjkt();

      PrjktListUC prjktListUC;
      XSqlConnection dbConnection = ZXC.TheVvForm.TheDbConnection;

      VvFindDialog dlg = CreateFind_Prjkt_Dialog();

      prjktListUC = (PrjktListUC)(dlg.TheRecListUC);

      switch(whichInformation)
      {
         case VvSQL.SorterType.Name:   prjktListUC.Fld_FromNaziv  =                    startValue.ToString() ; break;
         //case VvSQL.SorterType.RecID:  prjktListUC.Fld_FromSifra   = ZXC.ValOrZero_UInt(startValue.ToString()); break;
         case VvSQL.SorterType.Code:   prjktListUC.Fld_FromSifra  = ZXC.ValOrZero_UInt(startValue.ToString()); break;
         case VvSQL.SorterType.Ticker: prjktListUC.Fld_FromTicker =                    startValue.ToString() ; break;

         default: ZXC.aim_emsg(" 111: For Prjkt, trazi po [{1}] still nedovrseno!", whichInformation); break;
      }

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         if(!ZXC.PrjktDao.SetMe_Record_byRecID(dbConnection, prjkt_rec, (uint)dlg.SelectedRecID, false)) return null;
      }
      else
      {
         prjkt_rec = null;
      }

      if(dlg.SelectionIsNewlyAddedRecord == true) ZXC.ShouldForceSifrarRefreshing = true;

      dlg.Dispose();

      if(prjkt_rec != null)
      {
         switch(whichInformation)
         {
            case VvSQL.SorterType.Name  : return prjkt_rec.Naziv;
            //case VvSQL.SorterType.RecID:  return prjkt_rec.RecID;
            case VvSQL.SorterType.Code  : return prjkt_rec.KupdobCD/*RecID*/;
            case VvSQL.SorterType.Ticker: return prjkt_rec.Ticker;

            default: ZXC.aim_emsg(" 222: For Prjkt, trazi po [{1}] still nedovrseno!", whichInformation); return null;
         }
      }
      else return null;
   }

   #endregion Update_VvDataRecord (Legacy naming convention)

}