using System;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using Vektor.Reports.RIZ;
using Vektor.DataLayer.DS_AllColumns;
using Vektor.DataLayer.DS_Reports;
using System.Linq;
using CrystalDecisions.Windows.Forms;
using CrystalDecisions.Shared;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
using XSqlDataReader = MySql.Data.MySqlClient.MySqlDataReader;
using XSqlException  = MySql.Data.MySqlClient.MySqlException;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Forms;
#endif

public /*struct*/ class CrossDataForMalop
{
   public TtInfo  TtInfo             { get; set; }
   public bool    IsMalopFin_U       { get { return TtInfo.TheTT == "ManyTT" ? false : TtInfo.IsMalopFin_UorVMIorTRI; } }
   public bool    IsMalopFin_I       { get { return TtInfo.TheTT == "ManyTT" ? false : TtInfo.IsMalopFin_I     ; } }

   public decimal KPM_zaduzenje      { get; set; }
   public decimal KPM_razduzenje_rob { get; set; }
   public decimal KPM_razduzenje_usl { get; set; }

   public decimal S_ukKCRP_U         { get; set; }
   public decimal S_ukPdv_U          { get; set; }
   public decimal S_ukMSK_25_U       { get; set; }
   public decimal S_ukMSK_05_U       { get; set; }
   public decimal S_ukMSK_23_U       { get; set; }
   public decimal S_ukMSK_10_U       { get; set; }
   public decimal S_ukMSK_00_U       { get; set; }
   public decimal S_ukMskPdv_25_U    { get; set; }
   public decimal S_ukMskPdv_05_U    { get; set; }
   public decimal S_ukMskPdv_23_U    { get; set; }
   public decimal S_ukMskPdv_10_U    { get; set; }
   public decimal S_ukMrz_U          { get; set; }
   public decimal R_ukKCRP_rob_U     { get; set; }
   public decimal S_ukKCRP_usl_U     { get; set; }
   public decimal R_ukMSK_U          { get; set; }
   public decimal R_ukMskPdv_U       { get; set; }
   public decimal S_ukMskPNP_U       { get; set; }
   public decimal R_NivVrj00_U       { get; set; }
   public decimal R_NivVrj10_U       { get; set; }
   public decimal R_NivVrj23_U       { get; set; }
   public decimal R_NivVrj25_U       { get; set; }
   public decimal R_NivVrj05_U       { get; set; }
   public decimal R_NivVrj_U         { get; set; }
   public decimal R_NivMskPdv10_U    { get; set; }
   public decimal R_NivMskPdv23_U    { get; set; }
   public decimal R_NivMskPdv25_U    { get; set; }
   public decimal R_NivMskPdv05_U    { get; set; }
   public decimal R_NivMskPdv_U      { get; set; }
   public decimal R_NivMskPNP_U      { get; set; }
   public decimal R_NivMrz_U         { get; set; }
   public int     R_dokCount_U       { get; set; }

   public decimal R_ukKCRP_cash_I    { get; set; }
   public decimal R_ukKCRP_ziro_I    { get; set; }
   public decimal S_ukPdv_I          { get; set; }
   public decimal S_ukPNP_I          { get; set; }
   public decimal R_ukKCR_rob_I      { get; set; }
   public decimal S_ukKCR_usl_I      { get; set; }
   public decimal S_ukMSK_25_I       { get; set; }
   public decimal S_ukMSK_05_I       { get; set; }
   public decimal S_ukMSK_23_I       { get; set; }
   public decimal S_ukMSK_10_I       { get; set; }
   public decimal S_ukMSK_00_I       { get; set; }
   public decimal S_ukMskPdv_25_I    { get; set; }
   public decimal S_ukMskPdv_05_I    { get; set; }
   public decimal S_ukMskPdv_23_I    { get; set; }
   public decimal S_ukMskPdv_10_I    { get; set; }
   public decimal R_ukMskMrz_I       { get; set; }
   public decimal Ira_ROB_NV_I       { get; set; }
   public decimal R_ukKCRP_rob_I     { get; set; }
   public decimal S_ukKCRP_usl_I     { get; set; }
   public decimal R_IrmRobRbt_I      { get; set; }
   public decimal R_ukMSK_I          { get; set; }
   public decimal R_ukMskPdv_I       { get; set; }
   public decimal S_ukMskPNP_I       { get; set; }
   public decimal R_NivVrj00_I       { get; set; }
   public decimal R_NivVrj10_I       { get; set; }
   public decimal R_NivVrj23_I       { get; set; }
   public decimal R_NivVrj25_I       { get; set; }
   public decimal R_NivVrj05_I       { get; set; }
   public decimal R_NivVrj_I         { get; set; }
   public decimal R_NivMskPdv10_I    { get; set; }
   public decimal R_NivMskPdv23_I    { get; set; }
   public decimal R_NivMskPdv25_I    { get; set; }
   public decimal R_NivMskPdv05_I    { get; set; }
   public decimal R_NivMskPdv_I      { get; set; }
   public decimal R_NivMskPNP_I      { get; set; }
   public decimal R_NivMrz_I         { get; set; }
   public int     R_dokCount_I       { get; set; }

   public string S_ukKCRP_U_kto      { get; set; }
   public string S_ukPdv_U_kto       { get; set; }
   public string S_ukMSK_25_kto      { get; set; }
   public string S_ukMSK_05_kto      { get; set; }
   public string S_ukMSK_23_kto      { get; set; }
   public string S_ukMSK_10_kto      { get; set; }
   public string S_ukMSK_00_kto      { get; set; }
   public string S_ukMskPdv_25_kto   { get; set; }
   public string S_ukMskPdv_05_kto   { get; set; }
   public string S_ukMskPdv_23_kto   { get; set; }
   public string S_ukMskPdv_10_kto   { get; set; }
   public string S_ukMrz_kto         { get; set; }
   public string K_ukKCRP_cash_I_kto { get; set; }
   public string K_ukKCRP_ziro_I_kto { get; set; }
   public string S_ukPdv_I_kto       { get; set; }
   public string R_ukKCR_rob_I_kto   { get; set; }
   public string S_ukKCR_usl_I_kto   { get; set; }
   public string Ira_ROB_NV_I_kto    { get; set; }

   // 08.11.2016. dodano za prikaz knjizenja na rekap
   public decimal S_ukPdv05m         { get; set; }
   public decimal S_ukPdv10m         { get; set; }
   public decimal S_ukPdv25m         { get; set; }
   public decimal X_ukPpmvIzn        { get; set; }

   public decimal R_ukKCRP_storno    { get; set; }

   //public List<VvReportSourceUtil> NacPlacInfo { get; set; }

}

public abstract partial class VvRiskReport : VvReport
{
   #region Legacy stuff

   public override DataSet VirtualUntypedDataSet { get { return null; } }

   protected ReportDocument reportDocument;

   public override ReportDocument VirtualReportDocument { get { return reportDocument; } }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      return null; // "We don't need RISK report command. We use 'Lists<>'!";
   }

   #endregion Legacy stuff

   #region Constructor and some propertiez

   protected XSqlConnection TheDbConnection { get { return ZXC.TheVvForm.TheDbConnection; } }

   public override VvRptFilter VirtualRptFilter { get { return this.RptFilter; } }

   private   VvRpt_RiSk_Filter rptFilter;
   /*protected*/internal VvRpt_RiSk_Filter RptFilter
   {
      get { return this.rptFilter; }
      set {        this.rptFilter = value; }
   }

   public ZXC.RIZ_FilterStyle FilterStyle;

   public List<VvLookUpItem > TheUtilLookupList    { get; set; }

   public List<Artikl       > TheArtiklList        { get; set; }
   public List<ArtiklLight  > TheArtiklLightList   { get; set; }
   public List<ArtStat      > TheArtStatList       { get; set; }
 //public List<PFaktur      > ThePFakturList       { get; set; }
   public List<Faktur       > TheFakturList        { get; set; }
   public List<Faktur       > TheFakturBBBList     { get; set; }
   public List<FakturLight  > TheFakturLightList   { get; set; }
 //public List<PRtrans      > ThePRtransList       { get; set; }
   public List<Rtrans       > TheRtransList        { get; set; }
   public List<Rtrano       > TheRtranoList        { get; set; }
   public List<Kupdob       > TheKupdobList        { get; set; }
   public List<Kupdob       > ThePosJedList        { get; set; }
   public List<Prjkt        > ThePrjktList         { get; set; }
   public List<Ftrans       > TheFtransList        { get; set; }

   public List<VvReportSourceUtil> TheDeviznaSumaList { get; set; } // za CrossData.NacPlacInfo { get; set; } 
   public CrossDataForMalop        CrossData          { get; set; }

   public List<KamateReportLine> TheKamateReportList { get; set; }

   public List<VvManyDecimalsReportSourceRow> TheManyDecimalsList { get; set; }

   public List<SVD_RptLine                  > TheSVD_RptLineList        { get; set; }
   public List<SVD_SubRptLine               > TheSVD_SubRptLineList     { get; set; }
   public List<SVD_SubRptLine_Mon           > TheSVD_SubRptLineList_Mon { get; set; }

   public List<FakturImages> TheFakturImagesList { get; set; }

   public Faktur Faktur_rec_SumaRazdoblja_IRA { get; set; }
   public Faktur Faktur_rec_KumulativOdPg_IRA { get; set; }
   public Faktur Faktur_rec_SumaRazdoblja_URA { get; set; }
   public Faktur Faktur_rec_KumulativOdPg_URA { get; set; }

   public Faktur Faktur_rec_SumaRazdoblja_BLG { get; set; }
   public Faktur Faktur_rec_DonosPretRazd_BLG { get; set; }

   public Rtrans TheRtrans_AllSkladSUM { get; set; }

   //public VvRiskReport() { }

   public VvRiskReport(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl) 
   {
      this.reportDocument = _reportDocument;
      this.RptFilter      = _rptFilter;

      ReportNeeds_Artikl_List        = _rptNeeds_Artikl;
      ReportNeeds_ArtWars_List       = _rptNeeds_ArtWars;
      ReportNeeds_ArtStat_List       = _rptNeeds_ArtStat;
      ReportNeeds_Faktur_List        = _rptNeeds_Faktur;
      ReportNeeds_Rtrans_List        = _rptNeeds_Rtrans;
      ReportNeeds_Kupdob_List_Fak    = _rptNeeds_Kupdob;
      ReportNeeds_Prjkt_List         = _rptNeeds_Prjkt;
      ReportNeeds_Rtrans4ruc_List    = _rptNeeds_Rtrans4ruc;

      VvUserControl theUC = ZXC.TheVvForm.TheVvUC;

      if(ReportNeeds_ArtStat_List      ) TheArtStatList       = new List<ArtStat      >( );
      if(ReportNeeds_Artikl_List       ){TheArtiklList        = new List<Artikl       >( ); if(VvUserControl.ArtiklSifrar == null) theUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name); }
      if(ReportNeeds_ArtWars_List      ){TheArtiklList        = new List<Artikl       >( ); }
      if(ReportNeeds_Faktur_List       ) TheFakturList        = new List<Faktur       >( );
      if(ReportNeeds_Rtrans_List ||
         ReportNeeds_Rtrans4ruc_List   ) TheRtransList        = new List<Rtrans       >( );
      if(ReportNeeds_Rtrano_List       ) TheRtranoList        = new List<Rtrano       >( );
      if(ReportNeeds_Kupdob_List_Fak   ){TheKupdobList        = new List<Kupdob       >( ); 
                                         ThePosJedList        = new List<Kupdob       >( ); if(VvUserControl.KupdobSifrar == null) theUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name); }
      if(ReportNeeds_Prjkt_List        ){ThePrjktList         = new List<Prjkt        >(1); ThePrjktList.Add(ZXC.CURR_prjkt_rec); }

   }

   public override void TheReportLists_Clear()
   {
      if(this.IsForExport) return;

      if(TheUtilLookupList    != null) TheUtilLookupList  .Clear();
      if(TheArtiklList        != null) TheArtiklList      .Clear();
      if(TheArtiklLightList   != null) TheArtiklLightList .Clear();
      if(TheArtStatList       != null) TheArtStatList     .Clear();
      if(TheFakturList        != null) TheFakturList      .Clear();
      if(TheFakturBBBList     != null) TheFakturBBBList   .Clear();
      if(TheFakturLightList   != null) TheFakturLightList .Clear();
      if(TheRtransList        != null) TheRtransList      .Clear();
      if(TheRtranoList        != null) TheRtranoList      .Clear();
      if(TheKupdobList        != null) TheKupdobList      .Clear();
      if(ThePosJedList        != null) ThePosJedList      .Clear();
      if(ThePrjktList         != null) ThePrjktList       .Clear();
      if(TheFtransList        != null) TheFtransList      .Clear();
      if(TheDeviznaSumaList   != null) TheDeviznaSumaList .Clear();
      if(TheKamateReportList  != null) TheKamateReportList.Clear();
      if(TheManyDecimalsList  != null) TheManyDecimalsList.Clear();
   }

   #endregion Constructor and some propertiez

   #region Some Metodz

   protected const uint sklCdAsKupdobCd_dodatak = 1000000;

   protected void AddAllSkladAsArtificialKupdob(List<Kupdob> kupdobSifrar)
   {
      Kupdob kupdob_rec;

      foreach(VvLookUpItem lui in ZXC.luiListaSkladista)
      {
         kupdob_rec = new Kupdob();

         kupdob_rec.KupdobCD = (uint)lui.Integer + sklCdAsKupdobCd_dodatak;
         kupdob_rec.Naziv    = "   Skladište: " + lui.Name;

         kupdobSifrar.Add(kupdob_rec);
      }
   }

   public static List<Rtrans> SintRtransList_By_ArtiklCD_And_R_CIJ_KCRP(List<Rtrans> ANA_rtransList)
   {
      string  outerArtiklCD, innerArtiklCD;
      decimal outerKol     , innerKol     ;
      decimal outerR_CIJ_KCRP, innerR_CIJ_KCRP;

      // tu si BIO stao. moras se sjetiti gdje si vec nedavno imao potrebu i kako si rijesio da 
      // nastane nova lista iz neke postojece a da bude totalni deep copy
      // da, ovo napravi novu (deep) listu ali ti clanovi liste ostanu pointeri na istu stvar. 
    //List<Rtrans> SIN_rtransList = new List<Rtrans>(ANA_rtransList.ToList());
      List<Rtrans> SIN_rtransList = ANA_rtransList.ConvertAll(r => ZXC.DeepCopy(r));

      for(int outerRowIdx = 0; outerRowIdx < SIN_rtransList.Count/* - 1*/; ++outerRowIdx)
      {
         outerArtiklCD   = SIN_rtransList[outerRowIdx].T_artiklCD;
         outerR_CIJ_KCRP = SIN_rtransList[outerRowIdx].R_CIJ_KCRP/*.Ron2()*/;

         for(int innerRowIdx = outerRowIdx + 1; innerRowIdx < SIN_rtransList.Count/* - 1*/; ++innerRowIdx)
         {
            innerArtiklCD   = SIN_rtransList[innerRowIdx].T_artiklCD;
            innerR_CIJ_KCRP = SIN_rtransList[innerRowIdx].R_CIJ_KCRP/*.Ron2()*/;

            // !!! 
            // PAZI !!! Observacija od 21.05.2020: Ovdje ne provjeravas jednakost ostalih parametara rtrans calc-a a 
            // koji moraju biti isti da bi sazmi imalo smisla (mora biti ista i cijena, pdvSt, rbt, ...) 
            // !!! 
          //if(outerArtiklCD == innerArtiklCD)
          //if(outerArtiklCD == innerArtiklCD && outerR_CIJ_KCRP        == innerR_CIJ_KCRP       ) // NOVO! u SubModulima ovo ne provjeravas  //28.12.2021. ja b ovje stavila Ron2()!!!!!!!
            if(outerArtiklCD == innerArtiklCD && outerR_CIJ_KCRP.Ron2() == innerR_CIJ_KCRP.Ron2()) // NOVO! u SubModulima ovo ne provjeravas  //28.12.2021. ja b ovje stavila Ron2()!!!!!!!
            {
               outerKol = SIN_rtransList[outerRowIdx].T_kol;
               innerKol = SIN_rtransList[innerRowIdx].T_kol;

               SIN_rtransList[outerRowIdx].T_kol = outerKol + innerKol;

               SIN_rtransList.RemoveAt(innerRowIdx--);

               SIN_rtransList[outerRowIdx].CalcTransResults(null);

            }
         } // for(int innerRowIdx = outerRowIdx + 1; innerRowIdx < TheRtransList.Count - 1; ++innerRowIdx)
      } // for(int outerRowIdx = 0; outerRowIdx < TheRtransList.Count/* - 1*/; ++outerRowIdx)

      return SIN_rtransList;
   }

   #endregion Some Metodz

   #region Dispatch_FakturList

   #region Overriders 

   public static void FakturListTo_PDF(List<Faktur> theFakturList)
   {
      Dispatch_FakturList(theFakturList, 0, false, false, false);
   }

   public static void FakturListTo_PDF(List<Faktur> theFakturList, ushort subDsc)
   {
      Dispatch_FakturList(theFakturList, subDsc, false, false, false);
   }

   public static void FakturListTo_QuickPrint(List<Faktur> theFakturList)
   {
      Dispatch_FakturList(theFakturList, 0, true, false, false);
   }

   public static void FakturListTo_QuickPrint(List<Faktur> theFakturList, ushort subDsc)
   {
      Dispatch_FakturList(theFakturList, subDsc, true, false, false);
   }

   public static void FakturListTo_eMail(List<Faktur> theFakturList)
   {
      Dispatch_FakturList(theFakturList, 0, false, true, false);
   }

   public static void FakturListTo_eMail(List<Faktur> theFakturList, ushort subDsc)
   {
      Dispatch_FakturList(theFakturList, subDsc, false, true, false);
   }

   public static void FakturListTo_eRacun(List<Faktur> theFakturList)
   {
      Dispatch_FakturList(theFakturList, 0, false, false, true);
   }

   public static void FakturListTo_eRacun(List<Faktur> theFakturList, ushort subDsc)
   {
      Dispatch_FakturList(theFakturList, subDsc, false, false, true);
   }

   #endregion Overriders 

   private static void Dispatch_FakturList(List<Faktur> theFakturList, ushort subDsc, bool isTo_Printer, bool isTo_eMail, bool isTo_eRacun)
   {
      #region Init

      if(theFakturList.IsEmpty())
      {
         ZXC.aim_emsg("Lista za PDF je prazna?!");
         return;
      }

      ZXC.FakturList_To_PDF_InProgress = true  ;
      ZXC.FakturList_To_PDF_subDsc     = subDsc;

      string theTT = theFakturList.First().TT;

    //ReportDocument             reportDocument;
      RptR_IRA                   theRptR_IRA   ;

      ExportOptions              CrExportOptions;
      DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
      PdfRtfWordFormatOptions    CrFormatTypeOptions          = new PdfRtfWordFormatOptions();

      string deaultVvPDFdirectoryName = VvForm.GetLocalDirectoryForVvFile(VvPref.VvMailData.DeaultVvPDFdirectoryName);
      string todayDir                 = theTT + "_PDF_" + DateTime.Today.ToString(ZXC.VvDateYyyyMmDdMySQLFormat);
      string fileNameOnly ;
      string PDFfileFullPathName ;

      VvForm.CreateDirectoryInMyDocuments(Path.Combine(deaultVvPDFdirectoryName, todayDir));

      PrnFakDsc thePFD = new PrnFakDsc(FakturDUC.GetDscLuiListForThisTT(theTT, subDsc));

      string statusText;
      string sendingResult;

      System.Diagnostics.Stopwatch syncStopWatch = System.Diagnostics.Stopwatch.StartNew();

      uint soFarCount      = 0;
       int ofTotalCount    = theFakturList.Count;
      long elapsedTicks    = 0, remainTicks;
      decimal soFarKoef       ;
      TimeSpan elapsedTime = new TimeSpan(0);
      TimeSpan remainTime     ;

      bool isTo_PDF = isTo_Printer == false && isTo_eMail == false && isTo_eRacun == false;

      ZXC.SetStatusText("Faktur Lista u PDF ---> START");

      #endregion Init

      foreach(Faktur faktur_rec in theFakturList)
      {
         Cursor.Current = Cursors.WaitCursor;

         ZXC.FakturRec = faktur_rec;

         #region Create PDF files to hard disk - OR - Print To Printer

         // 1. GetReportDocument
       //reportDocument = GetReportDocument(faktur_rec, thePFD, theTT);
         theRptR_IRA    = GetRptR_IRA      (faktur_rec, thePFD, theTT);

         // 2. get fileName 
         fileNameOnly = faktur_rec.TT_And_TtNum + " [" + faktur_rec.KupdobName + "]" + ".pdf";

         PDFfileFullPathName = Path.Combine(deaultVvPDFdirectoryName, todayDir, fileNameOnly);
         // 3. set reportDocument.ExportOptions
         try
         {
            CrDiskFileDestinationOptions.DiskFileName = PDFfileFullPathName;
            CrExportOptions                           = theRptR_IRA.reportDocument.ExportOptions;

            CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
            CrExportOptions.ExportFormatType      = ExportFormatType.PortableDocFormat;
            CrExportOptions.DestinationOptions    = CrDiskFileDestinationOptions;
            CrExportOptions.FormatOptions         = CrFormatTypeOptions;

            if(isTo_Printer) theRptR_IRA.reportDocument.PrintToPrinter(1, false, 0, 0);
            else /* to PDF */theRptR_IRA.reportDocument.Export        (              );
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(ex.ToString());
         }

         #endregion Create PDF files to hard disk - OR - Print To Printer

         #region CreateAndSend_eMail_WithPDFasAttachment

         if(isTo_Printer)
         {
            #region Rwtrec Feedback 

            sendingResult = "Ispisano";

            ZXC.TheVvForm.BeginEdit(faktur_rec);
   
            faktur_rec.FiskPrgBr = ZXC.LenLimitedStr(sendingResult, ZXC.FaktExDao.GetSchemaColumnSize(ZXC.FexCI.fiskPrgBr));
   
            bool rwtOK = faktur_rec.VvDao.RWTREC(ZXC.TheVvForm.TheDbConnection, faktur_rec, false, true, false, false, true/* !!! */ ); // zbog implicitne synchronizacije ovaj rwtrec ne zelimo u Insert_LAN_LogEntry 

            ZXC.TheVvForm.EndEdit(faktur_rec);
   
            #endregion Rwtrec Feedback 

         } // isToEmail 

         if(isTo_eMail)
         {
            List<string> mailTO_AddressList = new List<string>();

            Kupdob kupdob_rec   = ZXC.TheVvForm.TheVvUC.Get_Kupdob_FromVvUcSifrar(faktur_rec.KupdobCD);

#if KADA_BUDE_realRELEASE
            mailTO_AddressList.Add(kupdob_rec.Email);
          //if(Email2.NotEmpty()) mailTO_AddressList.Add(kupdob_rec.Email2); // TODO 
            mailTO_AddressList.Add(@"vicko.pervan@pctogo.hr");
            mailTO_AddressList.Add(@"dorotea.golub@pctogo.hr");
#endif
          //mailTO_AddressList.Add(@"QWEtamaraQWE@viper.hr");
            mailTO_AddressList.Add(@"robert@viper.hr");
          //mailTO_AddressList.Add(@"viper@zg.htnet.hr");
          //mailTO_AddressList.Add(@"QWEviperQWE@zg.htnet.hr");

            bool sentOK = Dispatch_FakturList_AsPDFmailAttachment(mailTO_AddressList.ToArray(), PDFfileFullPathName, faktur_rec.VvDocumIdentOhneTitle); // todo: fakturNum 

            #region Rwtrec Feedback 

            sendingResult = sentOK ? "eMail poslan" : "eMail NEUSPJEH!";

            ZXC.TheVvForm.BeginEdit(faktur_rec);
   
            faktur_rec.FiskPrgBr = ZXC.LenLimitedStr(sendingResult, ZXC.FaktExDao.GetSchemaColumnSize(ZXC.FexCI.fiskPrgBr));
   
            bool rwtOK = faktur_rec.VvDao.RWTREC(ZXC.TheVvForm.TheDbConnection, faktur_rec, false, true, false, false, true/* !!! */ ); // zbog implicitne synchronizacije ovaj rwtrec ne zelimo u Insert_LAN_LogEntry 

            ZXC.TheVvForm.EndEdit(faktur_rec);
   
            #endregion Rwtrec Feedback 

         } // isToEmail 

         #endregion CreateAndSend_eMail_WithPDFasAttachment

         #region createAndSend_eRacun_WithPDFasAttachment

         if(isTo_eRacun)
         {
            Outgoing_eRacun_parameters oeRp = new Outgoing_eRacun_parameters(false);

            Kupdob kupdob_rec   = ZXC.TheVvForm.TheVvUC.Get_Kupdob_FromVvUcSifrar(faktur_rec.KupdobCD  );
            Kupdob primPlat_rec = ZXC.TheVvForm.TheVvUC.Get_Kupdob_FromVvUcSifrar(faktur_rec.PrimPlatCD);

            /* oeRp_1. */ oeRp.faktur_rec              = faktur_rec                                      ;
            /* oeRp_2. */ oeRp.kupdob_rec              = kupdob_rec                                      ;
            /* oeRp_3. */ oeRp.primPlat_rec            = primPlat_rec                                    ;
            /* oeRp_4. */ oeRp.thePFD                  = thePFD                                          ;
            /* oeRp_5. */ oeRp.PDF_as_base64_byteArray = System.IO.File.ReadAllBytes(PDFfileFullPathName);
            /* oeRp_6. */ oeRp.pdfFileNameOnly         = PDFfileFullPathName                             ;
            /* oeRp_7. */ oeRp.fullPath_XML_FileName   = oeRp.suggestedXmlFileName                       ;

            ZXC.TheVvForm.RISK_Outgoing_eRacun_JOB(oeRp); // ovaj sam već sredi Rwtrec Feedback u faktur_rec.FiskPrgBr 
         }

         #endregion createAndSend_eRacun_WithPDFasAttachment

         Cursor.Current = Cursors.Default;

         #region set status text

         soFarCount++;

         #region soFar vs remaining calc

         soFarKoef     = ZXC.DivSafe(soFarCount, ofTotalCount);
         elapsedTicks += syncStopWatch.Elapsed.Ticks          ;
         elapsedTime  += syncStopWatch.Elapsed                ;
         remainTicks   = (long)(ZXC.DivSafe((decimal)elapsedTicks, soFarKoef) - elapsedTicks);
         remainTime    = new TimeSpan(remainTicks);

         #endregion soFar vs remaining calc

         statusText =
            syncStopWatch.Elapsed.TotalSeconds.ToString1Vv() + "s " +
            "(" + (elapsedTime.TotalSeconds / (double)soFarCount).ToString1Vv() + "s avg) done " +
             (/*++*/soFarCount).ToString() +
             " of " + ofTotalCount +
             " (" + (soFarKoef * 100M).ToString0Vv() + "%)" +
            //" <"   + remainTime + "> "                              +
             string.Format(" remain <{0:00}:{1:00}:{2:00}> ", remainTime.Hours, remainTime.Minutes, remainTime.Seconds) +
             " " + faktur_rec.ToString();


         syncStopWatch.Restart();

         ZXC.SetStatusText(statusText); Cursor.Current = Cursors.WaitCursor;

         #endregion set status text

         theRptR_IRA.VirtualReportDocument.Close  ();
         theRptR_IRA.VirtualReportDocument.Dispose();
         theRptR_IRA                      .Dispose();

      } // foreach(Faktur faktur_rec in theFakturList)

      #region Finish

      syncStopWatch.Stop();

      ZXC.FakturRec = null;

      ZXC.FakturList_To_PDF_InProgress = false;
      ZXC.FakturList_To_PDF_subDsc     =     0;

      if(isTo_PDF == true) // samo kad je to pdf only 
      {
         DialogResult result = MessageBox.Show("Gotovo. Kreirao " + ofTotalCount + " PDF-ova.\n\nDa li želite otvoriti directory? ",
            "Prikaži directory sa PDF datotekama?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

         if(result == DialogResult.Yes)
         {
            System.Diagnostics.Process.Start(Path.Combine(deaultVvPDFdirectoryName, todayDir));
         }
      }

      if(isTo_eMail == true) 
      {
         ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo. Isporučio " + ofTotalCount + " mail-ova.");
      }

      if(isTo_Printer == true)
      {
         ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo. Ispisao " + ofTotalCount + " računa.");
      }

      ZXC.SetStatusText("");

      #endregion Finish

   } // private static void Dispatch_FakturList() 

   private static bool Dispatch_FakturList_AsPDFmailAttachment(string[] mailTO_AddressList, string PDFfileFullPathName, string fakturNum)
   {
      VvMailClient mailClient = new VvMailClient();

      mailClient.EmailFromPasswd      = ZXC.TheVvForm.VvPref.vvMailData.EmailFromPasswdDecrypted; // !!! ovo moze biti i prazno! Tada ida anonymously?! 
      mailClient.MailHost             = ZXC.TheVvForm.VvPref.vvMailData.MailHost                ;
      mailClient.EmailFromAddress     = ZXC.TheVvForm.VvPref.vvMailData.EmailFromAddress        ;
      mailClient.EmailFromUserName    = ZXC.TheVvForm.VvPref.vvMailData.UserName                ;
      mailClient.EmailFromDisplayName = ZXC.TheVvForm.VvPref.vvMailData.EmailFromDisplayName    ;
      mailClient.PortNo               = ZXC.TheVvForm.VvPref.vvMailData.Port                    ;
      mailClient.EnableSSL            = ZXC.TheVvForm.VvPref.vvMailData.UseSSL                  ;
      mailClient.IsCcToMySelf         = ZXC.TheVvForm.VvPref.vvMailData.PoslajiKopijuSebi       ;

      mailClient.MessageSubject       = "Račun " + fakturNum;

      mailClient.MessageBody          = "Poštovani, \n"                                      +
                                        "U prilogu je račun za najam informatičke opreme.\n" +
                                        "Srdačan pozdrav.\n"                                 +
                                        "PCTOGO d.o.o.\n"                                    ;

      mailClient.MailAttachmentFileNameList = new string[] { PDFfileFullPathName };

      bool OK = mailClient.SendMail_Normal(false, mailTO_AddressList);

      return OK;
   }

 //public static ReportDocument GetReportDocument(Faktur faktur_rec, PrnFakDsc thePFD, string theTT)
   public static RptR_IRA       GetRptR_IRA      (Faktur faktur_rec, PrnFakDsc thePFD, string theTT)
   {
      FakturDUC theDUC;

      //Control dummyControl = new Control();

      switch(theTT)
      {
         case Faktur.TT_IFA:
            theDUC = new IFADUC(null, faktur_rec, new VvForm.VvSubModul(ZXC.VvSubModulEnum.R_IFA));
            break;
         case Faktur.TT_IRA:
            theDUC = new IRADUC(null, faktur_rec, new VvForm.VvSubModul(ZXC.VvSubModulEnum.R_IRA));
            break;

         default: theDUC = null; break;
      }

      FakturDocFilter fakturFilter = new FakturDocFilter(theDUC);

      fakturFilter.PFD = thePFD;
      fakturFilter.isFakturList_To_PDF  = true ;
      fakturFilter.fakturList_To_PDF_TT = theTT;

      fakturFilter.theDUC = theDUC;

      RptR_IRA rptR_IRA = new RptR_IRA(new Vektor.Reports.RIZ.CR_PrnManyFak_ArtiklLight(), ""/*reportName*/, fakturFilter);

      rptR_IRA.FillDataSet_And_SetDataSource(null);

      //rptR_IRA.Insert_VvReportHeader_And_SetHeaderFofs();

      theDUC.Dispose();

      return rptR_IRA/*.reportDocument*/;
   }

#endregion Dispatch_FakturList

}

#region ArtiklUC's PrintRecordReports for Single Artikl

public class RptR_ArtiklKartica  : VvRiskReport
{
#region Constructor

   public RptR_ArtiklKartica(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter) : base(_reportDocument, _reportName, _rptFilter,
         false, // ArtWars         
         false, // ArtStat        
         // 12.8.2011: 
/*false*/true , // Faktur         
         // 07.10.2011: 
/*false*/true , // Rtrans         
         true , // Kupdob         
         false, // Prjkt          
         false, // Rtrans4ruc     
         false) // Artikl         

   {
   }

#endregion Constructor

   public override int FillRiskReportLists()
   {
#region GetRtransWithArtstatList and Init Stuff

      ArtiklUC theUC = ((ArtiklCardFilter)RptFilter).theArtiklUC;

      bool isFor_All_SkladCD = RptFilter.SkladCD.IsEmpty();
      bool isFor_One_SkladCD = !isFor_All_SkladCD;

      // 15.03.2016: 
      string orderBy = Rtrans.artiklOrderBy_ASC.Replace("t_", "R.t_");
      if(reportDocument is Vektor.Reports.RIZ.CR_RobnaKarticaKOL_Serlot)
      {
         orderBy = "R.t_serlot, " + orderBy;
      }

      if(isFor_One_SkladCD)
      {
         RtransDao.GetRtransWithArtstatList(TheDbConnection, TheRtransList, "", RptFilter.FilterMembers, orderBy);
      }
      else // this isFor_All_SkladCD - go Recursive 
      {
         var skladCDlist = ArtiklDao.GetDistinctSkladCdListForArtikl(TheDbConnection, RptFilter.ArtiklCdOD)/*.Where(sklCD => sklCD.NotEmpty())*/;

         ArtiklSifrarFilterUC artiklSifrarFilterUC = theUC.TheArtiklFilterUC;

         foreach(string currSkladCD in skladCDlist)
         {
            RptFilter.SkladCD = currSkladCD;

            artiklSifrarFilterUC.AddFilterMemberz(RptFilter, this);

            RtransDao.GetRtransWithArtstatList(TheDbConnection, TheRtransList, "", RptFilter.FilterMembers, orderBy); // Rekurzija 
         }

         RptFilter.SkladCD = "";
         RptFilter.FilterMembers.RemoveAll(fm => fm.name.StartsWith("SkladCD"));

      }

#endregion GetRtransWithArtstatList and Init Stuff

#region ForInternDocuments podmetni SkladName as PartnerName

      if(TheRtransList.Any(rtr => rtr.TtInfo.IsInternUI))
      {
         VvDaoBase.LoadGenericVvDataRecordList(TheDbConnection, TheFakturList, RptFilter.FilterMembers, "", "", false, "*", "LEFT JOIN rtrans R ON faktur.RecID = R.t_parentID", false, " tt, ttNum ");
      }

      TheRtransList.ForEach
         (
            delegate(Rtrans rtrans)
            {
               VvLookUpItem lui;

               // 11.01.2013: ne znam zasto su PSM, ZPC, VMI i VMU 10.01.2012 stavljeni da JESU extendables (TtInfo.arrayExtendables) pa ovdje ne ulaze 
             //if(rtrans.TtInfo.IsExtendableTT == false)
               if(rtrans.TtInfo.IsExtendableTT == false || rtrans.T_TT == Faktur.TT_PSM || rtrans.T_TT == Faktur.TT_ZPC || rtrans.T_TT == Faktur.TT_VMI || rtrans.T_TT == Faktur.TT_VMU) 
               {
                  string skladCD_AsKupdobCD = GetSkladCD_AsKupdobCD_ForInternDocuments(rtrans);

                  lui = ZXC.luiListaSkladista.GetLuiForThisCd(skladCD_AsKupdobCD);

                  if(lui != null)
                  {
                     rtrans.T_kupdobCD = (uint)lui.Integer + sklCdAsKupdobCd_dodatak;
                  }
               }
            }
      );

#endregion ForInternDocuments podmetni SkladName as PartnerName

#region TheKupdobList
      
      TheKupdobList = VvUserControl.KupdobSifrar.Join(TheRtransList, kpdb => kpdb.KupdobCD, rtrans => rtrans.T_kupdobCD, (kpdb, rtrans) => kpdb).Distinct().ToList();

      AddAllSkladAsArtificialKupdob(TheKupdobList);

#endregion TheKupdobList

#region TheRtrans_AllSkladSUM

      // We want RobnaKartica for All Sklads, or some check 

      bool isAllSkl = RptFilter.SkladCD.IsEmpty();

      TheRtrans_AllSkladSUM = new Rtrans();

      TheRtrans_AllSkladSUM.A_UkPstKol          = TheRtransList.Where(rtr => rtr.TtInfo.IsFinKol_PS).Sum(rtr => rtr.A_RtrPstKol );
      TheRtrans_AllSkladSUM.A_UkPstKol2         = TheRtransList.Where(rtr => rtr.TtInfo.IsFinKol_PS).Sum(rtr => rtr.A_RtrPstKol2);
      TheRtrans_AllSkladSUM.A_UkPstFinNBC       = TheRtransList.Where(rtr => rtr.TtInfo.IsFinKol_PS).Sum(rtr => rtr.A_RtrPstVrjNBC);
      TheRtrans_AllSkladSUM.A_UkPstFinMPC       = TheRtransList.Where(rtr => rtr.TtInfo.IsFinKol_PS).Sum(rtr => rtr.A_RtrPstVrjMPC);

      TheRtrans_AllSkladSUM.A_UkUlazKol         = TheRtransList.Where(rtr => (isAllSkl && rtr.TtInfo.IsAllSkladFinKol_U) || (isAllSkl == false && rtr.TtInfo.IsFinKol_U  && rtr.TtInfo.IsFinKol_PS == false)).Sum(rtr => rtr.A_RtrUlazKol        );
      TheRtrans_AllSkladSUM.A_UkUlazKol2        = TheRtransList.Where(rtr => (isAllSkl && rtr.TtInfo.IsAllSkladFinKol_U) || (isAllSkl == false && rtr.TtInfo.IsFinKol_U  && rtr.TtInfo.IsFinKol_PS == false)).Sum(rtr => rtr.A_RtrUlazKol2       );
      TheRtrans_AllSkladSUM.A_UkUlazFinNBC      = TheRtransList.Where(rtr => (isAllSkl && rtr.TtInfo.IsAllSkladFinKol_U) || (isAllSkl == false && rtr.TtInfo.IsFinKol_U  && rtr.TtInfo.IsFinKol_PS == false)).Sum(rtr => rtr.A_RtrUlazVrjNBC     );
      TheRtrans_AllSkladSUM.A_UkUlazFinMPC      = TheRtransList.Where(rtr => (isAllSkl && rtr.TtInfo.IsAllSkladFinKol_U) || (isAllSkl == false && rtr.TtInfo.IsFinKol_U  && rtr.TtInfo.IsFinKol_PS == false)).Sum(rtr => rtr.A_RtrUlazVrjMPC     );
      TheRtrans_AllSkladSUM.A_UkIzlazKol        = TheRtransList.Where(rtr => (isAllSkl && rtr.TtInfo.IsAllSkladFinKol_I) || (isAllSkl == false && rtr.TtInfo.IsFinKol_I                                    )).Sum(rtr => rtr.A_RtrIzlazKol       );
      TheRtrans_AllSkladSUM.A_UkIzlazKol2       = TheRtransList.Where(rtr => (isAllSkl && rtr.TtInfo.IsAllSkladFinKol_I) || (isAllSkl == false && rtr.TtInfo.IsFinKol_I                                    )).Sum(rtr => rtr.A_RtrIzlazKol2      );
      TheRtrans_AllSkladSUM.A_UkIzlazFinNBC     = TheRtransList.Where(rtr => (isAllSkl && rtr.TtInfo.IsAllSkladFinKol_I) || (isAllSkl == false && rtr.TtInfo.IsFinKol_I                                    )).Sum(rtr => rtr.A_RtrIzlazVrjNBC    );
      TheRtrans_AllSkladSUM.A_UkIzlazFinMPC     = TheRtransList.Where(rtr => (isAllSkl && rtr.TtInfo.IsAllSkladFinKol_I) || (isAllSkl == false && rtr.TtInfo.IsFinKol_I                                    )).Sum(rtr => rtr.A_RtrIzlazVrjMPC    );
      TheRtrans_AllSkladSUM.A_UkUlazKolFisycal  = TheRtransList.Where(rtr => (isAllSkl && rtr.TtInfo.IsKolOnly_U       ) || (isAllSkl == false && rtr.TtInfo.IsKolOnly_U && rtr.TtInfo.IsFinKol_PS == false)).Sum(rtr => rtr.A_RtrUlazKolFisycal );
      TheRtrans_AllSkladSUM.A_UkIzlazKolFisycal = TheRtransList.Where(rtr => (isAllSkl && rtr.TtInfo.IsKolOnly_I       ) || (isAllSkl == false && rtr.TtInfo.IsKolOnly_I && rtr.TtInfo.IsFinKol_PS == false)).Sum(rtr => rtr.A_RtrIzlazKolFisycal);

      if(ZXC.CURR_prjkt_rec.UrSkipKolStSkl == false)
      {
         TheRtrans_AllSkladSUM.A_UkUlazKolFisycal += TheRtrans_AllSkladSUM.A_UkUlazKol;
      }
      if(ZXC.CURR_prjkt_rec.IrSkipKolStSkl == false)
      {
         TheRtrans_AllSkladSUM.A_UkIzlazKolFisycal += TheRtrans_AllSkladSUM.A_UkIzlazKol;
      }

      TheRtrans_AllSkladSUM.A_UkRezervKolNaruc  = TheRtransList.Where(rtr => rtr.TtInfo.IsRezervKol).Sum(rtr => rtr.A_RtrKolNaruceno);
      TheRtrans_AllSkladSUM.A_UkRezervKolIsporu = TheRtransList.Where(rtr => rtr.TtInfo.IsRezervKol).Sum(rtr => rtr.A_RtrKolIsporuceno);
      TheRtrans_AllSkladSUM.A_UkStanjeKolRezerv = TheRtransList.Where(rtr => rtr.TtInfo.IsRezervKol).Sum(rtr => rtr.A_RtrKolNaruceno - rtr.A_RtrKolIsporuceno);


#endregion TheRtrans_AllSkladSUM

#region New from 28.11.2011 set KupdobName in rtrans_rec.R_kupdobName

      Kupdob kupdob_rec;
      //foreach(Rtrans rtrans in TheRtransList)
      // remarck next line and unremarck previous for non parallel*/ 
      Parallel.ForEach(TheRtransList, rtrans =>
      {
         // 11.01.2013: 
         if(rtrans.TtInfo.IsNivelacijaZPC)
         {
          //rtrans.R_kupdobName = "Zpc za kol: " + rtrans.A_StanjeKol.ToStringVv() + " i ruc: " + rtrans.A_DiffMalopCij.ToStringVv();
          //rtrans.R_kupdobName = "Zpc za kol: " + rtrans.T_kol.ToStringVv() + " i ruc: " + rtrans.R_ZCP_DiffCij       .ToStringVv();
            
            // 02.01.2019: TH: ukoliko NIJE na snazi nulti ZPC cijena ZPCa se posvadi s cijenom PSM-a pa ju treba ignorirati 
            if(Faktur.Get_IsNultiZPC(rtrans.T_TT, rtrans.T_ttNum) && rtrans.T_skladDate == ZXC.projectYearFirstDay)
            {
               rtrans.R_kupdobName = "NULTI ZPC";
            }
            else // old, classic 
            {
               rtrans.R_kupdobName = "Zpc za kol: " + rtrans.T_kol.ToStringVv() + " i ruc: " + rtrans.R_CIJ_MSK.ToStringVv();
            }
         }
         else
         {
            kupdob_rec = TheKupdobList.SingleOrDefault(kpdb => kpdb.KupdobCD == rtrans.T_kupdobCD);

            if(kupdob_rec != null)
            {
               rtrans.R_kupdobName = kupdob_rec.Naziv;

               // 14.04.2020: 
               if(ZXC.IsSvDUH && rtrans.T_TT == Faktur.TT_NRD)
               {
                  rtrans.R_kupdobName = kupdob_rec.Naziv + " - kol: " + rtrans.T_kol.ToString0Vv() + " cij : " + rtrans.R_CIJ_KCRP.ToStringVv();
               }
            }
         }
      }
      /* remarck this zagrada line for non parallel*/ );

#endregion Set KupdobName in rtrans_rec.R_kupdobName

#region Add Eventual Artificial Shadow Rtranses

      if( ZXC.RRD.Dsc_IsSupressSHADOWing       == false  && 
         (reportDocument is CR_RobnaKarticaKOL == false) && 
          TheRtransList.Any(rtr => rtr.TtInfo.HasShadowTT))
      {
         Rtrans shadowRtrans_rec;

         for(int i = 0; i < TheRtransList.Count; ++i)
         {
            if(TheRtransList[i].TtInfo.HasShadowTT == false) continue;

            shadowRtrans_rec = CreateRtransShadow(TheRtransList[i], i);

            //UndoShadowResultsForRealRtrans(TheRtransList[i]);

            if(shadowRtrans_rec != null) // ako je null znaci da nema potrebe za nivelacijom jer ili je prevKol nula ili je novaCijena = staraCijena 
            {
               TheRtransList.Insert(i /*+ 1*/, shadowRtrans_rec); ++i;
            }
         }
      }

#endregion Add Eventual Artificial Shadow Rtranses

#region Fill eventual missing cache (because this rtransTT is NOT cacheable)

      // 17.03.2016: 
      if(TheRtransList.Count.NotZero() && TheRtransList.Any(rtr => rtr.IsCacheable == true) && TheRtransList.Any(rtr => rtr.IsCacheable == false))
      {
         ArtStat prevNotEmptyArtstat = null;

       //foreach(Rtrans rtrans in TheRtransList.Where(rtr => rtr.IsCacheable == false))
         foreach(Rtrans rtrans in TheRtransList                                       )
         {
            if(rtrans.IsCacheable == true) // ima cache 
            {
               prevNotEmptyArtstat = rtrans.TheAsEx.MakeDeepCopy();
            }
            else if(prevNotEmptyArtstat != null) // nema cache, daj mu prethodnog 
            {
               rtrans.TheAsEx = ArtStat.NullifyRtransValues(prevNotEmptyArtstat);
            }
         }
      }

      #endregion Fill eventual last is missing cache (because this rtransTT is NOT cacheable)

      // 27.07.2022: 
      // 21.12.2022: 
    //if(ZXC.IsSvDUH) TheRtransList.RemoveAll(rtr => rtr.T_TT == Faktur.TT_NRD);
      if(ZXC.IsSvDUH) TheRtransList.RemoveAll(rtr => rtr.T_TT == Faktur.TT_NRD || rtr.T_TT == Faktur.TT_INV || rtr.T_TT == Faktur.TT_ZAH);

      return TheRtransList.Count;
   }

   private Rtrans CreateRtransShadow(Rtrans rtrans, int i)
   {
      // Do we need this, enivej?
      // 24.03.2014: 
    //if( rtrans.A_PrevKolStanje.Ron2().IsZero()                                       ||
      if((rtrans.A_PrevKolStanje.Ron2().IsZero() && rtrans.A_MalopCij.Ron2().IsZero()) ||
          rtrans.A_DiffMalopCij .Ron2().IsZero()                                       ||
      // 08.01.2016: 
          i.IsZero() // prva stavka robne kartice 
         ) return null; // ili je prevKol nula ili je novaCijena = staraCijena 

      Rtrans shadowRtrans_rec = new Rtrans();

      //shadowRtrans_rec.TheAsEx = rtrans.TheAsEx;

      // 04.11.2016: 
    //shadowRtrans_rec.T_TT  =
    //shadowRtrans_rec.AS_TT = Faktur.TT_NIV;
           if(rtrans.TtInfo.IsMalopFin_I) { shadowRtrans_rec.T_TT = shadowRtrans_rec.AS_TT = Faktur.TT_NIV; }
      else if(rtrans.TtInfo.IsMalopFin_U) { shadowRtrans_rec.T_TT = shadowRtrans_rec.AS_TT = Faktur.TT_NUV; }
      else throw new Exception("CreateFakturShadow: Niti U niti I");


    //shadowRtrans_rec.T_ttNum  =
    //shadowRtrans_rec.AS_TtNum = rtrans.T_ttNum;
      shadowRtrans_rec.T_skladDate  =
      shadowRtrans_rec.AS_SkladDate = rtrans.T_skladDate;
      shadowRtrans_rec.T_artiklCD   =
      shadowRtrans_rec.AS_ArtiklCD  = rtrans.T_artiklCD;
      shadowRtrans_rec.T_skladCD    = 
      shadowRtrans_rec.AS_SkladCD   = rtrans.T_skladCD;

      // ... just to remember ...
      // PrevKolStanje   = (StanjeKol     - (RtrPstKol + RtrUlazKol - RtrIzlazKol)); 
      // DiffMalopCij    = (RtrCijenaMPC  - PrevMalopCij                          ); 
      // NivelacUlazVrj  = (PrevKolStanje * DiffMalopCij                          ); 
      // NivelacIzlacVrj = (RtrIzlazKol   * DiffMalopCij                          ); 

      shadowRtrans_rec.A_UkUlazKol = rtrans.A_PrevKolStanje;

    //if(rtrans.T_TT == Faktur.TT_IRM) // Implicitna NIVELACIJA zbogPoradi rucno promijenjene cijene na IRM-u          
      if(rtrans.TtInfo.IsMalopFin_I)   // Implicitna NIVELACIJA zbogPoradi rucno promijenjene cijene na IRM-u, IZM     
      {
         shadowRtrans_rec.A_UkUlazFinMPC   =  rtrans.A_StanjeFinMPC + rtrans.A_RtrIzlazVrjMPC;

         shadowRtrans_rec.A_RtrCijenaMPC   =
         shadowRtrans_rec.A_RtrIzlazCijMPC =  rtrans.A_DiffMalopCij;
         shadowRtrans_rec.A_RtrIzlazVrjMPC = -rtrans.A_NivelacIzlazVrj; // nota bene ovaj '-' na pocetku 

         shadowRtrans_rec.R_kupdobName = "Niv za kol: " + rtrans.A_RtrIzlazKol.ToStringVv() + " i ruc: " + rtrans.A_DiffMalopCij.ToStringVv();
      }
      else                             // Implicitna NIVELACIJA zbogPoradi novopridosle cijene koja je drukcija od zatecene 
      {
         shadowRtrans_rec.A_UkUlazFinMPC   =  rtrans.A_StanjeFinMPC - rtrans.A_RtrUlazVrjMPC;
                                           
         shadowRtrans_rec.A_RtrCijenaMPC   =
         shadowRtrans_rec.A_RtrUlazCijMPC  =  rtrans.A_DiffMalopCij;
         shadowRtrans_rec.A_RtrUlazVrjMPC  =  rtrans.A_NivelacUlazVrj;

         shadowRtrans_rec.R_kupdobName = "Niv za kol: " + rtrans.A_PrevKolStanje.ToStringVv() + " i ruc: " + rtrans.A_DiffMalopCij.ToStringVv();
      }

      return shadowRtrans_rec;
   }

   private string GetSkladCD_AsKupdobCD_ForInternDocuments(Rtrans rtrans_rec)
   {
      string skladCD_AsKupdobCD;

      //Faktur.TT_MSU, 
      //Faktur.TT_MSI, 
      //Faktur.TT_PIZ, 
      //Faktur.TT_IMT, 
      //Faktur.TT_PPR, 
      //Faktur.TT_POV, 
      //Faktur.TT_PUL: 
      //Faktur.TT_VMU: 
      //Faktur.TT_VMI: 
      if(rtrans_rec.TtInfo.IsInternUI)
      {
         Faktur faktur_rec = TheFakturList./*Single*/FirstOrDefault(fak => fak.RecID == rtrans_rec.T_parentID);
         if(faktur_rec == null)
         {
            skladCD_AsKupdobCD = " ?!? ";
         }
         else
         {
            if(rtrans_rec.TtInfo.IsInternIzlaz)
            {
               skladCD_AsKupdobCD = faktur_rec.SkladCD2;
            }
            else // Interni Ulaz 
            {
               skladCD_AsKupdobCD = faktur_rec.SkladCD;
            }
         }
      }
      else //Faktur.TT_PST, ... 
      {
         skladCD_AsKupdobCD = rtrans_rec.T_skladCD;
      }

      return skladCD_AsKupdobCD;
   }

}

public class RptR_ArtiklRtranses : VvRiskReport
{
   public RptR_ArtiklRtranses(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter) : base(_reportDocument, _reportName, _rptFilter,
         false, // ArtWars         
         false, // ArtStat        
         false, // Faktur         
         true , // Rtrans         
         true , // Kupdob         
         false, // Prjkt          
         false, // Rtrans4ruc     
         false) // Artikl         

   {
   }

   public override int FillRiskReportLists()
   {
      ArtiklUC theUC = ((ArtiklCardFilter)RptFilter).theArtiklUC;

      // TheRtransList
      VvDaoBase.LoadGenericVvDataRecordList<Rtrans>(TheDbConnection, TheRtransList, ((ArtiklCardFilter)RptFilter).FilterMembers, Rtrans.artiklOrderBy_ASC);

      TheRtransList.ForEach(rtr => rtr.CalcTransResults(null));

      // TODO: !!! koji  a
      if(TheRtransList.Count.IsZero()) TheRtransList.Add(new Rtrans());

      // TheKupdobList
      TheKupdobList = VvUserControl.KupdobSifrar.Join(TheRtransList, kpdb => kpdb.KupdobCD, rtrans => rtrans.T_kupdobCD, (kpdb, rtrans) => kpdb).Distinct().ToList();

      // TEMP!!! OBRISI OVO KASNIJE:
      //VvDaoBase.LoadGenericVvDataRecordList<Faktur>(conn, TheFakturList = new List<Faktur>(), null, "", "", true);
      // ovo 'TheFakturList = new List<Faktur>()' ti je trebalo jer ovaj izvj po defaultu ne treba TheFakturList pa je inace ovdje null
      // VAZNO, idijote!, da ne zaboravis; ovo ti je primjer kako si uspio napraviti da sa standardnim 'LoadGenericVvDataRecordList' dohvatis i Faktur i FaktEx dataLayer!!!!!!!!!!

      return TheRtransList.Count;
   }
}

public class RptR_ArtiklMaticni  : VvRiskReport
{
   public RptR_ArtiklMaticni(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter) : base(_reportDocument, _reportName, _rptFilter, 
         true , // ArtWars        
         true , // ArtStat        
         false, // Faktur         
         false, // Rtrans         
         false, // Kupdob         
         false, // Prjkt          
         false, // Rtrans4ruc     
         false) // Artikl         

   {
   }

   public override int FillRiskReportLists()
   {
      ArtiklUC theUC = ((ArtiklCardFilter)RptFilter).theArtiklUC;

      // TheArStatList
      TheArtStatList.Add(ArtiklDao.GetArtiklStatus(TheDbConnection, theUC.artikl_rec.ArtiklCD, theUC.TheCurrentSkladCD, theUC.Fld_NaDan));

      // TheArtiklList
      TheArtiklList.Add(theUC.artikl_rec);

      return TheArtiklList.Count;
   }
}

#endregion ArtiklUC's PrintRecordReports for Single Artikl

#region FakturDUC's PrintRecordReports for Single Faktur

public /*partial*/ class RptR_IRA : VvRiskReport
{
   public RptR_IRA(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter) : base(_reportDocument, _reportName, _rptFilter, 
         true , // ArtWars         
         false, // ArtStat        
         true , // Faktur         
         true , // Rtrans         
         true , // Kupdob         
         true , // Prjkt          
         false, // Rtrans4ruc     
         false) // Artikl         

   {
      ReportNeeds_Rtrano_List = true;

      string tt = "";

      // 11.02.2022: 
      if(_rptFilter.isFakturList_To_PDF == true)
      {
         tt = _rptFilter.fakturList_To_PDF_TT;
      }
      else // kao do sad 
      {
         // 18.06.2019: eRacun 
         tt = ((FakturDocFilter)RptFilter).theDUC.faktur_rec.TT;
      }

    // 12.05.2021 samo za probu za sada Tembo treba eRacun IRM
    //if(tt == Faktur.TT_IRA || tt == Faktur.TT_IFA)
      if(tt == Faktur.TT_IRA || tt == Faktur.TT_IFA || tt == Faktur.TT_IRM)
      {
         IsForExport = true;
      }

   }

   public List<Rtrans> RealizacijaRtransList { get; set; }

   public ZXC.SVD_PotrosnjaInfo SVD_PotrosnjaInfo;

   public override int FillRiskReportLists()
   {
      FakturDUC theDUC = ((FakturDocFilter)RptFilter).theDUC;

      Faktur faktur_rec = (Faktur)theDUC.faktur_rec.CreateNewRecordAndCloneItComplete();

      ZXC.ValutaNameEnum presentValutaEnum = ((FakturDocFilter)RptFilter).DevNameAsEnum;

      decimal devTecaj;

      // 17.12.2012: dodan if() 
      if(theDUC.IsShowingConvertedMoney == true)
      {
         devTecaj = ZXC.DevTecDao.GetHnbTecaj(presentValutaEnum, faktur_rec.DokDate);
         faktur_rec.ConvertBussinessValuesToDeviza(devTecaj, ((FakturDocFilter)RptFilter).DevNameAsEnum);
      }

      TheFakturList.Add(faktur_rec);

      //TheRtransList = faktur_rec.Transes                                                                     ; if(TheRtransList.Count.IsZero()) TheRtransList.Add(new Rtrans());
      TheRtransList = faktur_rec.Transes.Where(r => r.T_pdvColTip != ZXC.PdvKolTipEnum.AVANS_STORNO).ToList(); if(TheRtransList.Count.IsZero()) TheRtransList.Add(new Rtrans());
      TheRtranoList = faktur_rec.Transes2; if(TheRtranoList.Count.IsZero()) TheRtranoList.Add(new Rtrano());

      if(faktur_rec.TtInfo.IsExtendableTT)
      {
         TheKupdobList.Add(VvUserControl.KupdobSifrar.SingleOrDefault(kpdb => kpdb.KupdobCD == faktur_rec.KupdobCD));
         ThePosJedList.Add(VvUserControl.KupdobSifrar.SingleOrDefault(kpdb => kpdb.KupdobCD == faktur_rec.PosJedCD));
      }

      TheArtiklList = VvUserControl.ArtiklSifrar.Join(TheRtransList, art => art.ArtiklCD, rtr => rtr.T_artiklCD, (art, rtr) => art).Distinct().ToList();

      if(RptFilter.PFD != null && (RptFilter.PFD.Dsc_R_mjMasaN || RptFilter.PFD.Dsc_OnlyArtiklLongOpis))
      {
         faktur_rec.SetAllRtrans_R_mjData(TheArtiklList);
      }

#region Podmetni u nazivArtikla 'MadeIn'
      // ovo ne ide ovako jer ArtiklName na CR dolazi iz Rtransa a ne iz ArtiklLighta
      bool isMadeIn2NazivArtikla = false;
      if(isMadeIn2NazivArtikla == true)
      {
         TheArtiklList.ForEach(art => art.ArtiklName = art.MadeIn);
      }

#endregion Podmetni u nazivArtikla 'MadeIn'

      TheArtiklLightList = TheArtiklList.Select(artikl => new ArtiklLight(artikl)).ToList();

#region Light Print Data Source Additions

      //ThePFakturList = new List<PFaktur>(1);
      //ThePFakturList.Add(new PFaktur(faktur_rec));

      //ThePRtransList = new List<PRtrans>(TheRtransList.Count);
      //foreach(Rtrans rtrans_rec in TheRtransList)
      //{
      //   ThePRtransList.Add(new PRtrans(rtrans_rec));
      //}

#endregion Light Print Data Source Additions

#region RNP_Analiza Additions

      if(((FakturDocFilter)RptFilter).IsRNP_Analiza == true)
      {
#region Ftrans

         //KtoShemaDsc KSD = new KtoShemaDsc(ZXC.dscLuiLst_KtoShema);
         KtoShemaDsc KSD = ZXC.KSD;

         int[] prevYears = FtransDao.GetPrevYearsList(faktur_rec.TtNum, false);

         // ------------------------------------------------------------------------------------------------- ------------------------------------------------------------------------------------------------- 
         List<Ftrans> ftransList_dirT = new List<Ftrans>();
         List<VvSqlFilterMember> filterMembers_FTR_dirT = FtransDao.GetFM_ftrOTP(DateTime.MinValue, DateTime.Now, KSD.Dsc_otp_rdAnaGR, "4", faktur_rec.TT_And_TtNum, FtransDao.OtpLista.raspDirkt, true);
         // ------------------------------------------------------------------------------------------------- ------------------------------------------------------------------------------------------------- 
         List<Ftrans> ftransList_indT = new List<Ftrans>();
         List<VvSqlFilterMember> filterMembers_FTR_indT = FtransDao.GetFM_ftrOTP(DateTime.MinValue, DateTime.Now, KSD.Dsc_otp_niAnaGR, "4", faktur_rec.TT_And_TtNum, FtransDao.OtpLista.raspIndir, true);
         // ------------------------------------------------------------------------------------------------- ------------------------------------------------------------------------------------------------- 
         List<Ftrans> ftransList_priA = new List<Ftrans>(); // primljeni avansi 
         List<VvSqlFilterMember> filterMembers_FTR_priA = new List<VvSqlFilterMember>();
         filterMembers_FTR_priA.Add(new VvSqlFilterMember("SUBSTRING(t_konto, 1, 3)", "(" + /*wantedKontoSet*/KSD.Dsc_PrimAvansKonta + ")", " IN ")); // Samo konta iz SET-a Dsc_PrimAvansKonta 
         filterMembers_FTR_priA.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_projektCD], "prjktCD", faktur_rec.TT_And_TtNum, "  = "));
         // ------------------------------------------------------------------------------------------------- ------------------------------------------------------------------------------------------------- 
         List<Ftrans> ftransList_kupRn = new List<Ftrans>(); // racuni od kupaca 
         List<VvSqlFilterMember> filterMembers_FTR_kupRn = new List<VvSqlFilterMember>();
         filterMembers_FTR_kupRn.Add(new VvSqlFilterMember("SUBSTRING(t_konto, 1, 3)", "(" + /*wantedKontoSet*/KSD.Dsc_KupacKontaIOS + ")", " IN ")); // Samo konta iz SET-a Dsc_KupacKontaIOS 
         filterMembers_FTR_kupRn.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_projektCD], "prjktCD", faktur_rec.TT_And_TtNum, "  = "));
         // ------------------------------------------------------------------------------------------------- ------------------------------------------------------------------------------------------------- 
         List<Ftrans> ftransList_danA = new List<Ftrans>(); // dani avansi 
         List<VvSqlFilterMember> filterMembers_FTR_danA = new List<VvSqlFilterMember>();
         filterMembers_FTR_danA.Add(new VvSqlFilterMember("SUBSTRING(t_konto, 1, 3)", "(" + /*wantedKontoSet*/KSD.Dsc_DaniAvansKonta + ")", " IN ")); // Samo konta iz SET-a Dsc_DaniAvansKonta 
         filterMembers_FTR_danA.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_projektCD], "prjktCD", faktur_rec.TT_And_TtNum, "  = "));
         // ------------------------------------------------------------------------------------------------- ------------------------------------------------------------------------------------------------- 
         List<Ftrans> ftransList_dobRn = new List<Ftrans>(); // racuni od dobavljaca 
         List<VvSqlFilterMember> filterMembers_FTR_dobRn = new List<VvSqlFilterMember>();
         filterMembers_FTR_dobRn.Add(new VvSqlFilterMember("SUBSTRING(t_konto, 1, 3)", "(" + /*wantedKontoSet*/KSD.Dsc_DobavKontaIOS + ")", " IN ")); // Samo konta iz SET-a Dsc_DobavKontaIOS 
         filterMembers_FTR_dobRn.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_projektCD], "prjktCD", faktur_rec.TT_And_TtNum, "  = "));
         // ------------------------------------------------------------------------------------------------- ------------------------------------------------------------------------------------------------- 

         bool isSomePreviousYear;

#endregion Ftrans

#region Faktur

         TheFakturList = new List<Faktur>();
         List<VvSqlFilterMember> filterMembers_FAK = new List<VvSqlFilterMember>(1);

         filterMembers_FAK.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.projektCD], "prjktCD", faktur_rec.TT_And_TtNum, "  = "));

#endregion Faktur

#region Get: many years FtransList, many years FakturList

         foreach(int year in prevYears)
         {
            isSomePreviousYear = year < ZXC.projectYearFirstDay.Year;

            VvDaoBase.LoadGenericVvDataRecordList(isSomePreviousYear ? ZXC.TheSecondDbConn_SameDB_OtherYear(year) : TheDbConnection, ftransList_dirT, filterMembers_FTR_dirT, "");
            VvDaoBase.LoadGenericVvDataRecordList(isSomePreviousYear ? ZXC.TheSecondDbConn_SameDB_OtherYear(year) : TheDbConnection, ftransList_indT, filterMembers_FTR_indT, "");
            VvDaoBase.LoadGenericVvDataRecordList(isSomePreviousYear ? ZXC.TheSecondDbConn_SameDB_OtherYear(year) : TheDbConnection, ftransList_priA, filterMembers_FTR_priA, "");
            VvDaoBase.LoadGenericVvDataRecordList(isSomePreviousYear ? ZXC.TheSecondDbConn_SameDB_OtherYear(year) : TheDbConnection, ftransList_kupRn, filterMembers_FTR_kupRn, "");
            VvDaoBase.LoadGenericVvDataRecordList(isSomePreviousYear ? ZXC.TheSecondDbConn_SameDB_OtherYear(year) : TheDbConnection, ftransList_danA, filterMembers_FTR_danA, "");
            VvDaoBase.LoadGenericVvDataRecordList(isSomePreviousYear ? ZXC.TheSecondDbConn_SameDB_OtherYear(year) : TheDbConnection, ftransList_dobRn, filterMembers_FTR_dobRn, "");

            VvDaoBase.LoadGenericVvDataRecordList(isSomePreviousYear ? ZXC.TheSecondDbConn_SameDB_OtherYear(year) : TheDbConnection, TheFakturList, filterMembers_FAK, "", "", true);
         }

         TheFakturList.RemoveAll(fak => fak.TT != Faktur.TT_NRD && fak.TT != Faktur.TT_NRS && fak.TT != Faktur.TT_NRU);

#endregion many years FtransList, many years FakturList

#region TheManyDecimalsList - report source

         DateTime datObracuna = DateTime.Now; // !! todo? 
         DateTime datUgovora = faktur_rec.DospDate;
         DateTime datOtvRNP = faktur_rec.DokDate; // !! todo? 

         string brojRNP = faktur_rec.TT_And_TtNum;
         string partner = faktur_rec.KupdobName;

         // 11.11.2016. Frigo problem je kada je dom kup/dob uzima cijenu sa PDV-om a treba uzimati samo osnovicu jer je i ugovorena cijena bez PDV-a
         decimal domDob = 0.00M;
         decimal inoDob = 0.00M;
         decimal domKup = 0.00M;
         decimal inoKup = 0.00M;

         foreach(Ftrans ftr in ftransList_dobRn)
         {
            if(ftr.T_konto.StartsWith("220")) domDob += (ZXC.VvGet_100_from_125(ftr.T_dug, Faktur.CommonPdvStForThisDate(ftr.T_dokDate)));
            else inoDob += ftr.T_dug;
         }
         foreach(Ftrans ftr in ftransList_kupRn)
         {
            if(ftr.T_konto.StartsWith("120")) domKup += (ZXC.VvGet_100_from_125(ftr.T_pot, Faktur.CommonPdvStForThisDate(ftr.T_dokDate)));
            else inoKup += ftr.T_pot;
         }
         // 11.11.2016. dodano - moglo bi se i  tako da se gore rade dvije liste posebno za domace a posebno za ino kup/dob


         decimal ugCijena = faktur_rec.SomeMoney;
         decimal naplAvansom = ftransList_priA.Sum(f => f.R_PotMinusDug);
         //decimal  naplRacunom = ftransList_kupRn.Sum(f => f.T_pot        ); 11.11.2016.
         decimal naplRacunom = domKup + inoKup;
         decimal dirTrsk = ftransList_dirT.Sum(f => f.T_dug);
         decimal indTrsk = ftransList_indT.Sum(f => f.T_dug);
         decimal placAvansom = ftransList_danA.Sum(f => f.R_DugMinusPot);
         //decimal  placRacunom = ftransList_dobRn.Sum(f => f.T_dug        ); 11.11.2016.
         decimal placRacunom = domDob + inoDob;
         decimal rezTrsk = faktur_rec.Decimal01;
         decimal rezAmort = faktur_rec.Decimal02;
         decimal rezervac1 = ((FakturDocFilter)RptFilter).RNP_Rezervacija1;
         decimal rezervac2 = ((FakturDocFilter)RptFilter).RNP_Rezervacija2;
         decimal izdaneNrd = TheFakturList.Sum(fak => fak.S_ukKCRP);

         decimal ukupTrsk = dirTrsk + indTrsk + rezTrsk + rezAmort + rezervac1 + rezervac2;
         decimal budOckTrsk = izdaneNrd > (placAvansom + placRacunom) ? izdaneNrd - placAvansom - placRacunom : 0.00M;
         decimal ukNaplaceno = naplAvansom + naplRacunom;

         TheManyDecimalsList = new List<VvManyDecimalsReportSourceRow>(1);
         TheManyDecimalsList.Add(new VvManyDecimalsReportSourceRow(

            /* DateTime date1,     */ datObracuna,
            /* DateTime date2,     */ datUgovora,
            /* DateTime date3,     */ datOtvRNP,
            /* uint    rowUintCD   */ 0,
            /* string  rowStringCD */ brojRNP,
            /* string  rowName     */ partner,
            /* decimal decimA01    */ ugCijena,
            /* decimal decimA02    */ naplAvansom,
            /* decimal decimA03    */ naplRacunom,
            /* decimal decimA04    */ ukNaplaceno,
            /* decimal decimA05    */ dirTrsk,
            /* decimal decimA06    */ indTrsk,
            /* decimal decimA07    */ rezTrsk,
            /* decimal decimA08    */ rezAmort,
            /* decimal decimA09    */ rezervac1,
            /* decimal decimA10    */ rezervac2,
            /* decimal decimA11    */ 0.00M,
            /* decimal decimA12    */ ukupTrsk,
            /* decimal decimA13    */ izdaneNrd,
            /* decimal decimA14    */ placAvansom,
            /* decimal decimA15    */ placRacunom,
            /* decimal decimA16    */ budOckTrsk,
            /* decimal decimA17    */ ukNaplaceno - ukupTrsk - budOckTrsk, // stanje po naplacenosti
                                                                           /* decimal decimA18    */ ugCijena - ukupTrsk - budOckTrsk, // stanje po ugovorenoj cijeni
                                                                        /* decimal decimA19    */ ugCijena - ukNaplaceno,           // preostalo za naplatu
                                                                        /* decimal decimA20    */ 0.00M

               ));



#endregion TheManyDecimalsList - report source

      }

#endregion RNP_Analiza Additions

#region OTS na kraju fakture

      // 06.10.2015: 
      if((ZXC.RRD.Dsc_IsPrintOTSafterIRA || (RptFilter.PFD != null && RptFilter.PFD.Dsc_OcuOTS_saldo)) &&
         (faktur_rec.TT == Faktur.TT_IFA || faktur_rec.TT == Faktur.TT_IRA))
      {
         VvRpt_RiSk_Filter OTSrptFilter = new VvRpt_RiSk_Filter();

         OTSrptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_dokDate], false, "DateOD", ZXC.projectYearFirstDay, ZXC.projectYearFirstDay.ToString("dd.MM.yyyy."), "Od datuma:", " >= ", ""));
         OTSrptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_dokDate], false, "DateDO", faktur_rec.DokDate, faktur_rec.DokDate.ToString("dd.MM.yyyy."), "Do datuma:", " <= ", ""));
         OTSrptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_kupdob_cd], false, "kupdobCD", faktur_rec.KupdobCD, faktur_rec.KupdobName, "Za partnera:", " = ", ""));

         TheFtransList = new List<Ftrans>();

         //NalogDao.LoadFtransListFor_OtsReport(TheDbConnection, TheFtransList, OTSrptFilter.FilterMembers, /* DateTime.Now */ OTSrptFilter.OtsDate, /*IsOtsKupaca*/true, OTSrptFilter.IsOtsAnalitKontre, OTSrptFilter.IsOtsDospOnly, false);
         NalogDao.LoadFtransListFor_OtsReport(TheDbConnection, TheFtransList, OTSrptFilter.FilterMembers, /* DateTime.Now */ faktur_rec.DokDate, /*IsOtsKupaca*/true, false, true, false);

         // todo: treba ovo? 
         if(OTSrptFilter.IsForceOtsByDokDate)
         {
            TheFtransList = TheFtransList.OrderBy(ftr => ftr.T_ticker).ThenBy(ftr => ftr.T_dokDate).ThenBy(ftr => ftr.R_tipBr_Resorted).ThenBy(ftr => ftr.T_serial).ToList();
         }
         else // default
         {
            TheFtransList = TheFtransList.OrderBy(ftr => ftr.T_ticker).ThenBy(ftr => ftr.R_tipBr_Resorted).ThenBy(ftr => ftr.T_dokDate).ThenBy(ftr => ftr.T_serial).ToList();
         }

         //TheKupdobList = VvUserControl.KupdobSifrar.Join(TheFtransList, kpdb => kpdb.KupdobCD, ftrans => ftrans.T_kupdob_cd, (k, f) => k).Distinct().ToList();

         // todo: treba ovo? 
         if(OTSrptFilter.IsOtsNevezano == true)
         {
            var kupdobsCdWithEmptyTipBrList = TheFtransList.Where(ftr => ftr.T_tipBr.IsEmpty()).Select(ftr => ftr.T_kupdob_cd).Distinct();

            TheFtransList.RemoveAll(ftr => kupdobsCdWithEmptyTipBrList.Contains(ftr.T_kupdob_cd) == false);
         }

         theDUC.TheTheFtransList = this.TheFtransList;
      } // if(ZXC.IsPrintOTS_AfterPrintIRA && 

#endregion OTS na kraju fakture

#region RNM, RNP, RNS, PRJ, RealizacijaRtransList
      // 16.03.2016. ovo je samo na RNMDUC-u pa radi grsku kod Analize RNP-a
      //if(theDUC.IsRadNalog && (theDUC as IVvRealizabeFakturDUC).RealizacijaRtransList != null) this.RealizacijaRtransList = (theDUC as IVvRealizabeFakturDUC).RealizacijaRtransList;
      if(theDUC is RNMDUC && (theDUC as IVvRealizableFakturDUC).RealizRtrList_AllYears != null) this.RealizacijaRtransList = (theDUC as IVvRealizableFakturDUC).RealizRtrList_AllYears;

      //else                  this.RealizacijaRtransList = null;

#endregion RNM, RNP, RNS, PRJ, RealizacijaRtransList

#region SVD_PotrosnjaInfo

      if(ZXC.IsSvDUH)
      {
#region 26.08.2020 koliko je  L /P

         if(faktur_rec.TT == Faktur.TT_URA)
         {
            Artikl artikl_rec;

            foreach(Rtrans rtrans_rec in faktur_rec.Transes)
            {
               artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == rtrans_rec.T_artiklCD);

               if(artikl_rec != null)
               {
                  rtrans_rec.TmpDecimal  = artikl_rec.IsSvdArtGR_Ljek_ ? rtrans_rec.R_KCRP : 0M;
                  rtrans_rec.TmpDecimal2 = artikl_rec.IsSvdArtGR_Potr_ ? rtrans_rec.R_KCRP : 0M;
               }
               else
               {
                  rtrans_rec.TmpDecimal  = 0M;
                  rtrans_rec.TmpDecimal2 = 0M;
               }
            }

            faktur_rec.Decimal01 = faktur_rec.Transes.Sum(rtr => rtr.TmpDecimal ); // ukLJEK 
            faktur_rec.Decimal02 = faktur_rec.Transes.Sum(rtr => rtr.TmpDecimal2); // ukPOTR 
         }

#endregion 26.08.2020 koliko je  L /P

         Kupdob kupdob_rec = null;

         if(TheKupdobList.Count.NotZero())
         {
            kupdob_rec = TheKupdobList[0];
         }

         //12.05.2022                                                 
         // printanju MSI-a i PIZ-a ne treba a smeta SVD_PotrosnjaInfo
         //20.07.2022 a trebali bi printati i PST a i INV
       //if(faktur_rec.TT != Faktur.TT_MSI && faktur_rec.TT != Faktur.TT_PIZ                                                                    )
         if(faktur_rec.TT != Faktur.TT_MSI && faktur_rec.TT != Faktur.TT_PIZ && faktur_rec.TT != Faktur.TT_PST && faktur_rec.TT != Faktur.TT_INV)
         {   
            SVD_PotrosnjaInfo = RtransDao.Get_SVD_PotrosnjaInfo(TheDbConnection, faktur_rec.TT, kupdob_rec, DateTime.MinValue, faktur_rec.DokDate, VvUserControl.KupdobSifrar, false);
         }
      }

#endregion SVD_PotrosnjaInfo

#region TheFakturImagesList

    //if(RptFilter.ReportNeedsImages)
      if(RptFilter.ReportNeedsImages || faktur_rec.IsFiskalDutyFaktur_ONLINE)
      {
         TheFakturImagesList = new List<FakturImages>();

         if(TheKupdobList.IsEmpty()) { TheKupdobList = new List<Kupdob>(); TheKupdobList.Add(new Kupdob()); }

         TheFakturImagesList.Add(new FakturImages(faktur_rec, TheKupdobList[0]));
      }

#endregion TheFakturImagesList

#region NAK - Get Naziv Artikla Za Kupca

      if(ZXC.RRD.Dsc_IsUseNAK)
      {
         TheRtransList.ForEach(rtr => rtr.T_artiklName = XtransDao.GetNazivArtiklaZaKupca(TheDbConnection, rtr.T_kupdobCD, rtr.T_artiklCD, rtr.T_artiklName));
      }

#endregion NAK - Get Naziv Artikla Za Kupca

#region Sinteza Artikla Za Print

      if(ZXC.RRD.Dsc_IsSintArt4Print)
      {
                       //SintRtransList_By_ArtiklCD_And_R_CIJ_KCRP(TheRtransList);
         TheRtransList = SintRtransList_By_ArtiklCD_And_R_CIJ_KCRP(TheRtransList);
      }

#endregion Sinteza Artikla Za Print

#region PCTOGO

      if(ZXC.IsPCTOGO)
      {
         if(faktur_rec.TT == Faktur.TT_UGN || faktur_rec.TT == Faktur.TT_AUN)
         {

         }
      }



#endregion PCTOGO

      return TheRtransList.Count;
   }

   // eRacun ovo ono 
   public override string ExportFileName { get { return "Qwe Exception!"; } }

   public override bool ExecuteExport(string fileName)
   {
      return true;
   }

}

#endregion FakturDUC's PrintRecordReports for Single Faktur

#region Classic RISK Reports

#region RptR_StandardRiskReport (The Father)

public class RptR_StandardRiskReport : VvRiskReport
{
   protected bool HRDweWant = false; // DELLME LATTER!!! 
   public RptR_StandardRiskReport(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl) : base(_reportDocument, _reportName, _rptFilter,
         _rptNeeds_ArtWars   , // ArtiklWithArtstat         
         _rptNeeds_ArtStat   , // ArtStat        
         _rptNeeds_Faktur    , // Faktur         
         _rptNeeds_Rtrans    , // Rtrans         
         _rptNeeds_Kupdob    , // Kupdob         
         _rptNeeds_Prjkt     , // Prjkt          
         _rptNeeds_Rtrans4ruc, // Rtrans4ruc     
         _rptNeeds_Artikl    ) // Artikl         

   {
      FilterStyle = _filterStyle;

      // 21.01.2015: 
      if(ZXC.IsTEXTHOshop && RptFilter.SkladCD.IsEmpty()) 
      {
         RptFilter.SkladCD = ZXC.luiListaSkladista.FirstOrDefault(lui => lui.Cd.Length >= 2 && lui.Cd.Substring(0, 2) == ZXC.vvDB_ServerID.ToString()).Cd;
      }
      // 19.05.2015: 
      if(VvUserControl.ArtiklSifrar == null) ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);
      if(VvUserControl.KupdobSifrar == null) ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

   }

   public override int FillRiskReportLists()
   {

      if(ReportNeeds_ArtWars_List)    GetArtiklWithArtstatList();
    //if(ReportNeeds_FakturPG_List  ) GetFakturList           ();
      if(ReportNeeds_Faktur_List    ) GetFakturList           ();
      if(ReportNeeds_Rtrans4ruc_List) GetRtransWithArtstatList();
      if(ReportNeeds_Kupdob_List_Fak) GetFaktursKupdobList    (); // Ovo 'Fakturs' jer ces jednom mozda trebati 'Artikls' Kupdob-e ili 'Rtrans' Kupdob-e. 
      if(ReportNeeds_Rtrans_List)     GetRtransList           ();
      if(ReportNeeds_Artikl_List)     GetArtiklList           ();

      return 0;
   }

#region GetSpecificList

   private void GetArtiklList() 
   {
      if(reportDocument is Vektor.Reports.RIZ.CR_SkladBilten)
      {
         TheArtiklList = new List<Artikl>(VvUserControl.ArtiklSifrar);

         if(RptFilter.ArtiklCdOD.NotEmpty()) TheArtiklList = TheArtiklList.Where(art => art.ArtiklCD  .CompareTo(RptFilter.ArtiklCdOD) >= 0).ToList();
         if(RptFilter.ArtiklCdDO.NotEmpty()) TheArtiklList = TheArtiklList.Where(art => art.ArtiklCD  .CompareTo(RptFilter.ArtiklCdDO) <= 0).ToList();
         if(RptFilter. ArtNameOD.NotEmpty()) TheArtiklList = TheArtiklList.Where(art => art.ArtiklName.CompareTo(RptFilter .ArtNameOD) >= 0).ToList();
         if(RptFilter. ArtNameDO.NotEmpty()) TheArtiklList = TheArtiklList.Where(art => art.ArtiklName.CompareTo(RptFilter .ArtNameDO) <= 0).ToList();
         
         switch(RptFilter.SorterType_Sifrar)
         {
            case VvSQL.SorterType.Name   : TheArtiklList = TheArtiklList.OrderBy(art => art.ArtiklName)                              .ToList(); break;
            case VvSQL.SorterType.Code   : TheArtiklList = TheArtiklList.OrderBy(art => art.ArtiklCD  )                              .ToList(); break;
            case VvSQL.SorterType.BarCode: TheArtiklList = TheArtiklList.OrderBy(art => art.BarCode1  ).ThenBy(art => art.ArtiklName).ToList(); break;
         }
      }
      else
      {
         TheArtiklList = VvUserControl.ArtiklSifrar.Join(TheRtransList, art => art.ArtiklCD, rtran => rtran.T_artiklCD, (a, x) => a).Distinct().ToList();
      }
   }

   private void GetRtransList()
   {
      GetRtransList(ZXC.projectYearFirstDay.Year);
   }
   protected void GetRtransList(int year) 
   {
      // !!!                                                                                                                                                               
      // !!!                                                                                                                                                               
      // !!! 24.10.2017: nota bene! Ovdje ce riknuti ako se zada bilo koji Faktur filterMember a da nije zajednicki (datum, skladiste, ...)                                
      // !!! Treba, dakle, sve unaprijed poznate, kao npr. 'DevName', eliminirati.                                                                                         
      // !!!                                                                                                                                                               

      bool isSomeOtherYear = year < ZXC.projectYearFirstDay.Year;

      // 03.03.2017 ___ START ___
      VvRpt_RiSk_Filter localRptFilter = new VvRpt_RiSk_Filter(true);
      localRptFilter.FilterMembers = new List<VvSqlFilterMember>(RptFilter.FilterMembers);
      if(this is RptR_Rekap_RNM)
      {
         localRptFilter.FilterMembers.RemoveAll(fm => fm.name == "kupdobCD");
      }
      // 03.03.2017 ___ START ___

      // 24.10.2017: izbaci Tembu DevName za 'RptR_PrnManyFak'
      if(this is RptR_PrnManyFak)
      {
         localRptFilter.FilterMembers.RemoveAll(fm => fm.name.StartsWith("DevName"));
      }

      ZXC.RtransDao.LoadManyDocumentsTtranses(isSomeOtherYear ? ZXC.TheSecondDbConn_SameDB_OtherYear(year) : TheDbConnection, TheRtransList, localRptFilter, "t_ttSort ASC, t_ttNum ASC, t_serial ASC "); 
   }

#region Sam Lokal Propertiz

   protected DataRowCollection    ArtSch   { get { return ZXC.ArtiklDao.TheSchemaTable.Rows;  } }
   protected ArtiklDao.ArtiklCI   ArtCI    { get { return ZXC.ArtiklDao.CI;                   } }
   protected DataRowCollection    ArsSch   { get { return ZXC.ArtStatDao.TheSchemaTable.Rows; } }
   protected ArtStatDao.ArtStatCI ArsCI    { get { return ZXC.ArtStatDao.CI;                  } }
   protected DataRowCollection    FakSch   { get { return ZXC.FakturDao.TheSchemaTable.Rows;  } }
   protected FakturDao.FakturCI   FakCI    { get { return ZXC.FakturDao.CI;                   } }
   protected DataRowCollection    FakExSch { get { return ZXC.FaktExDao.TheSchemaTable.Rows;  } }
   protected FaktExDao.FaktExCI   FakExCI  { get { return ZXC.FaktExDao.CI;                   } }
   protected DataRowCollection    RtrSch   { get { return ZXC.RtransDao.TheSchemaTable.Rows;  } }
   protected RtransDao.RtransCI   RtrCI    { get { return ZXC.RtransDao.CI;                   } }
   protected DataRowCollection    FtrSch   { get { return ZXC.FtransDao.TheSchemaTable.Rows;  } }
   protected FtransDao.FtransCI   FtrCI    { get { return ZXC.FtransDao.CI;                   } }

#endregion Sam Lokal Propertiz

   protected RiskReportUC   TheReportUC     { get { return ZXC.TheVvForm.TheVvReportUC as RiskReportUC; } }

   protected string FakturOrderBy
   {
      get
      {
         switch(RptFilter.SorterType_Dokument)
         {
            case VvSQL.SorterType.DokDate : return "dokDate , ttSort, ttNum ";
            case VvSQL.SorterType.DokNum  : 
            case VvSQL.SorterType.TtNum   : return "          ttSort, ttNum ";
            case VvSQL.SorterType.KpdbName:
               TtInfo ttInfo = ZXC.TtInfo(RptFilter.TT);

               if(ttInfo.IsExtendableTT)
               {
                  if(RptFilter.FuseBool3 == true) // IsPoslJed instead of Kupdob 
                     return "posJedName, dokDate , ttSort, ttNum ";
                  else
                     return "kupdobName, dokDate , ttSort, ttNum ";
               }
               else  return "            dokDate , ttSort, ttNum ";
            
            default: return "";
         }
      }
   }

   protected string ArtiklOrderBy
   {
      get
      {
         switch(RptFilter.SorterType_Sifrar)
         {
            case VvSQL.SorterType.Name   : return "artiklName"           ;
            case VvSQL.SorterType.Code   : return "artiklCD"             ;
            case VvSQL.SorterType.BarCode: return "barCode1, artiklName ";
            case VvSQL.SorterType.s_lio:   return "atestBr, artiklName " ;

            default: return "";
         }
      }
   }

   private void GetFakturList()
   {
      GetFakturList(ZXC.projectYearFirstDay.Year);
   }
   protected void GetFakturList(int year)
   {
      List<VvSqlFilterMember> filterMembers = RptFilter.FilterMembers;

      bool isSomePreviousYear = year < ZXC.projectYearFirstDay.Year;
      // 19.03.2012: u RptR_PrometArtikla if(IsFakGrDataInFakturBussiness(fakturGR) || needsKpdbList) se ide vaditi ova FakturList-a         
      // a ako je istovremeno zbog Rtransa filtrirano i po npr. ArtiklCD-u ili nazivu onda ti Artikl filterMemberi SMETAJU u faktur recenici 
      if(this is RptR_PrometArtikla) filterMembers = RptFilter.FilterMembers.Where(fm => fm.name.Contains("Artikl") == false).ToList();

      // 25.11.2016: 
      if(this is RptR_RekapIRMasBlagIzvj)
      {
         filterMembers.RemoveAll(fm => fm.name         .ToUpper().Contains("TT") || 
                                       fm.forcedColName.ToUpper().Contains("TT"));
         string blagAndIrmTT_INclause = TtInfo.GetSql_IN_Clause(new string[] { Faktur.TT_IRM, Faktur.TT_BIS, Faktur.TT_BUP });

         filterMembers.Add(new VvSqlFilterMember("TT", blagAndIrmTT_INclause, " IN "));
      }

      // 10.02.2017: 
      if(this is RptR_Rekap_IRMvsFtrans && (this as RptR_Rekap_IRMvsFtrans).isIRA)
      {
         filterMembers.RemoveAll(fm => fm.name         .ToUpper().Contains("TT") || 
                                       fm.forcedColName.ToUpper().Contains("TT"));
         string PrihodTT_without_IRM_INclause = TtInfo.Prihod_IN_Clause.Replace("'IRM',", "");

         filterMembers.Add(new VvSqlFilterMember("TT", PrihodTT_without_IRM_INclause, " IN "));
      }

      //// 24.11.2015: BigData logic ... start 
      //int? recCount = FakturDao.CountRecords(isSomePreviousYear ? ZXC.TheSecondDbConn_SameDB_OtherYear(year) : TheDbConnection, filterMembers);
      //bool isBigData = recCount > ZXC.BigDataRecCountLimit;
      //// 24.11.2015: BigData logic ... end   

      VvDaoBase.LoadGenericVvDataRecordList(isSomePreviousYear ? ZXC.TheSecondDbConn_SameDB_OtherYear(year) : TheDbConnection, TheFakturList, filterMembers, "", FakturOrderBy, true);

#region WEB_DEMO_DATA
#if(WWWDEMO)

      DemoiseKupdobSifrarList(/*TheKupdobList*/VvUserControl.KupdobSifrar);

      for(int i=0, w=0; i < TheFakturList.Count; ++i, ++w)
      {
         //if(w == demoKupdobs.Length) w = 0;
         //TheFakturList[i].KupdobName  = demoKupdobs[w].TheName;
         //TheFakturList[i].KupdobTK = demoKupdobs[w].TheCd  ;

         if(TheFakturList[i].KupdobTK.IsEmpty()) continue;

         switch(TheFakturList[i].KupdobTK.ToLower()[0])
         {
            case 'a':
                     TheFakturList[i].KupdobName  = demoKupdobs[0].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[0].TheCd  ; break;
            case 'b':
                     TheFakturList[i].KupdobName  = demoKupdobs[1].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[1].TheCd  ; break;
            case 'c':
                     TheFakturList[i].KupdobName  = demoKupdobs[2].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[2].TheCd  ; break;
            case 'd':
                     TheFakturList[i].KupdobName  = demoKupdobs[3].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[3].TheCd  ; break;
            case 'e':
                     TheFakturList[i].KupdobName  = demoKupdobs[4].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[4].TheCd  ; break;
            case 'f':
                     TheFakturList[i].KupdobName  = demoKupdobs[5].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[5].TheCd  ; break;
            case 'g':
                     TheFakturList[i].KupdobName  = demoKupdobs[6].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[6].TheCd  ; break;
            case 'h':
                     TheFakturList[i].KupdobName  = demoKupdobs[7].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[7].TheCd  ; break;
            case 'i':
                     TheFakturList[i].KupdobName  = demoKupdobs[8].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[8].TheCd  ; break;
            case 'j':
                     TheFakturList[i].KupdobName  = demoKupdobs[9].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[9].TheCd  ; break;
            case 'k':
                     TheFakturList[i].KupdobName  = demoKupdobs[10].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[10].TheCd  ; break;
            case 'l':
                     TheFakturList[i].KupdobName  = demoKupdobs[11].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[11].TheCd  ; break;
            case 'm':
                     TheFakturList[i].KupdobName  = demoKupdobs[12].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[12].TheCd  ; break;
            case 'n':                                      
                     TheFakturList[i].KupdobName  = demoKupdobs[13].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[13].TheCd  ; break;
            case 'o':                                      
                     TheFakturList[i].KupdobName  = demoKupdobs[14].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[14].TheCd  ; break;
            case 'p':                                      
                     TheFakturList[i].KupdobName  = demoKupdobs[15].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[15].TheCd  ; break;
            case 'r':                                      
                     TheFakturList[i].KupdobName  = demoKupdobs[16].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[16].TheCd  ; break;
            case 's':                                      
                     TheFakturList[i].KupdobName  = demoKupdobs[17].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[17].TheCd  ; break;
            case 't':                                      
                     TheFakturList[i].KupdobName  = demoKupdobs[18].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[18].TheCd  ; break;
            case 'u':                                      
                     TheFakturList[i].KupdobName  = demoKupdobs[19].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[19].TheCd  ; break;
            case 'v':                                      
                     TheFakturList[i].KupdobName  = demoKupdobs[20].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[20].TheCd  ; break;
            case 'z':                                      
                     TheFakturList[i].KupdobName  = demoKupdobs[21].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[21].TheCd  ; break;
            default:                                       
                     TheFakturList[i].KupdobName  = demoKupdobs[22].TheName;
                     TheFakturList[i].KupdobTK = demoKupdobs[22].TheCd  ; break;
         }
      }

#endif
#endregion WEB_DEMO_DATA

      // 08.09.2016 pokusaj rjesenja Tembo Peugeot/Citroen ovoono 
    //if(RptFilter.NeedsHorizontalLine                        ) // TRIK!!! da ne dodajemo novi RptFilter property... ako stavis 'daj horiz crte' 
      if(RptFilter.NeedsHorizontalLine && ZXC.IsSvDUH == false) // TRIK!!! da ne dodajemo novi RptFilter property... ako stavis 'daj horiz crte' 
                                                                // tada ce ti na ispisima umjesto Kupdob podataka ispisivati PoslJed podatke     
      TheFakturList.Where(f => f.PosJedCD != f.KupdobCD).ToList().ForEach(f => 
      { 
         f.KupdobCD   = f.PosJedCD  ;
         f.KupdobTK   = f.PosJedTK  ;
         f.KupdobName = f.PosJedName; 
      });

      if(HRDweWant && year <= 2022)
      {
         TheFakturList.Where(f => f.DokDate.Year == year).ToList().ForEach(f => f.Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB(null));
      }
   }

   /*private*/protected void GetRtransWithArtstatList()
   {
      GetRtransWithArtstatList(ZXC.projectYearFirstDay.Year);
   }
   protected void GetRtransWithArtstatList(int year)
   {
      bool isSomeOtherYear = year < ZXC.projectYearFirstDay.Year;
      // 07.03.2012: ako ne ides na drill down tada ti i ne treba ORDER BY
      RtransDao.GetRtransWithArtstatList(isSomeOtherYear ? ZXC.TheSecondDbConn_SameDB_OtherYear(year) : TheDbConnection, TheRtransList, "", RptFilter.FilterMembers, "");
    //RtransDao.GetRtransWithArtstatList(TheDbConnection, TheRtransList, "", RptFilter.FilterMembers, "R.t_" + ArtiklOrderBy + ", " + Rtrans.artiklOrderBy_ASC.Replace("t_", "R.t_"));
   }

   internal bool shouldGetArtstatListForRekapTT;
   private  void GetArtiklWithArtstatList()
   {
      bool thisSkladHasInvProblems = false;
      bool anySkladHasInvProblems  = false;
      bool isInventuraReport       = (reportDocument is Vektor.Reports.RIZ.CR_InventurneRazlike);
      bool thisSkladHasVisManjDocs = false;

      bool isPrmRazdoblja = (reportDocument is Vektor.Reports.RIZ.CR_PrometRazdoblja   ) || 
                            (reportDocument is Vektor.Reports.RIZ.CR_PrometRazdoblja_OP)  ;

//  bool*/ shouldGetArtstatListForRekapTT   = (reportDocument is Vektor.Reports.RIZ.CR_StanjeSklad_B                  ) && RptFilter.IsChk0 == true;
  /*bool*/ shouldGetArtstatListForRekapTT   = (reportDocument is Vektor.Reports.RIZ.CR_StanjeSklad_B || isPrmRazdoblja) && RptFilter.IsChk0 == true;
      
      bool isForceMPSK_by_NBC = RptFilter.FuseBool1; // RptFilter.FuseBool1 je Fld_IsNbcZaMPSK 

      System.Diagnostics.Stopwatch syncStopWatch = System.Diagnostics.Stopwatch.StartNew(); long elapsedTicks = 0; List<ZXC.CdAndName_CommonStruct> infoList = new List<ZXC.CdAndName_CommonStruct>();

      if(shouldGetArtstatListForRekapTT)
      {
         TheArtStatList      = new List<ArtStat>();
         TheManyDecimalsList = new List<VvManyDecimalsReportSourceRow>();
      }

      // ======AAAAAAAAAAAAAAAAAAAAAAAAA========================================================= 
      if(RptFilter.SkladCD.IsEmpty()) // all_sklad we want 
      {
         List<string> skladList = ArtiklDao.GetDistinctSkladCdListForArtikl(TheDbConnection, /*_artiklCD*/ "");

         // 21.01.2015: 
         if(ZXC.IsTEXTHOshop) skladList.RemoveAll(scd => scd.Length >= 2 && scd.Substring(0, 2) != ZXC.vvDB_ServerID.ToString());

         // 27.09.2017: 
         if(ZXC.IsTEMBO && RptFilter.SkladFilter == ZXC.KomisijaKindEnum.TMB2SKL)
         {
            skladList.RemoveAll(scd => scd != "VPSK" && scd != "VPS2");
         }
 
         var skladListW_sklNum = skladList.Join(ZXC.luiListaSkladista, sklList => sklList, lui => lui.Cd, (sklad, lui) => lui).OrderBy(lui => lui.Integer).ToList();

         // 03.09.2015: 
         if(RptFilter.SkladFilter == ZXC.KomisijaKindEnum.VELEPRODAJNA) skladListW_sklNum.RemoveAll(s => s.Flag == true );
         if(RptFilter.SkladFilter == ZXC.KomisijaKindEnum.MALOPRODAJNA) skladListW_sklNum.RemoveAll(s => s.Flag == false);

         // 23.12.2022:
         List<Artikl> thisPassArtiklList;

         //foreach(string skladCD in skladList)
         foreach(VvLookUpItem lui in skladListW_sklNum)
         {
            // 15.08.2012: VelaGospa, bijo danas na Grobniku...
            // 13.07.2022: dodan " && isPrmRazdoblja"
            if(RptFilter.DatumOd != ZXC.projectYearFirstDay && isPrmRazdoblja) // znaci trazimo Promet RAZDOBLJA, a validacija dateOD je obavljena ranije. (StanjeSkladista mora ici od 01.01, PrometRazdoblja neSmije ici od 01.01.) 
            {                                                
               List<Artikl> endList   = new List<Artikl>();
               List<Artikl> startList = new List<Artikl>();

               ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, endList  , lui.Cd,           RptFilter.DatumDo            , RptFilter, /*fuse*/ "", ArtiklOrderBy);
               ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, startList, lui.Cd, /* !!! */ RptFilter.DatumOd.AddDays(-1), RptFilter, /*fuse*/ "", ArtiklOrderBy);

               /*TheArtiklList = */ SetArtiklListForRazdoblje(endList, startList);

#region GetArtstat_SUM_list

               if(shouldGetArtstatListForRekapTT)
               {
                  ArtStatDao.GetArtstat_SUM_list(TheDbConnection, true, TheArtStatList, lui.Cd, RptFilter.DatumOd, RptFilter.DatumDo, RptFilter);
               }

#endregion GetArtstat_SUM_list

            }
            else // classic 
            {
               thisPassArtiklList = ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, TheArtiklList, lui.Cd, RptFilter.DatumDo, RptFilter, /*fuse*/ "", ArtiklOrderBy);

               // 15.12.2022: one time only; TH provjera ima li neki s ArtiklCD2 praznim 
               if(RptFilter.IsChk0 == true && ZXC.IsTEXTHOany && ZXC.projectYearAsInt == 2022)
               {
                  var ErrorsList = new List<string>();

                  foreach(Artikl artikl in /*TheArtiklList*/thisPassArtiklList.Where(art => (art.Grupa3CD == "PRKOM" || (art.Grupa3CD == "NBiPR" && art.Grupa1CD != "Akat" && art.ArtiklCD.Length == 6)) && art.AS_StanjeKol.NotZero()))
                  {
                     if(artikl.ArtiklCD2.IsEmpty())
                     {
                        ErrorsList.Add(String.Format("{0} {1} {2} NEMA newEuroArtikl_rec", lui.Cd, lui.Name, artikl));
                     }
                  }

                  if(ErrorsList.NotEmpty())
                  {
                     ZXC.aim_emsg_List(string.Format("{0} Nema NEW_EURO artikla.", ErrorsList.Count), ErrorsList);
                  }
               }


               // 29.10.2019: otkrili da ovo smeta za webShopExport: 
               //if(ZXC.IsTEMBO                                              ) // TEMBO svedi na OrgPak = 1 sve NE 'litre' PodJM 
               if(ZXC.IsTEMBO && (this is RptR_TemboWebShopExport) == false) // TEMBO svedi na OrgPak = 1 sve NE 'litre' PodJM 
               {
                  foreach(Artikl artikl_rec in TheArtiklList)
                  {
                     if(artikl_rec.AS_OrgPakJM.ToUpper().StartsWith("L") == false)
                     {
                        artikl_rec.AS_OrgPak = 1.00M;
                     }
                  }
               }

#region GetArtstat_SUM_list 

               if(shouldGetArtstatListForRekapTT)
               {
                  ArtStatDao.GetArtstat_SUM_list(TheDbConnection, false, TheArtStatList, lui.Cd, DateTime.MinValue, RptFilter.DatumDo, RptFilter);
               }

#endregion GetArtstat_SUM_list

#region CheckInvProblems

               if(isInventuraReport)
               {
                  thisSkladHasInvProblems = CheckInvProblems(lui, ref thisSkladHasVisManjDocs);
                  if(thisSkladHasInvProblems) anySkladHasInvProblems = true;
               }

#endregion CheckInvProblems

            }

            elapsedTicks += syncStopWatch.Elapsed.Ticks;
            infoList.Add(new ZXC.CdAndName_CommonStruct() { TheCd = lui.Cd, TheName = lui.Name, TheUint = (uint)syncStopWatch.Elapsed.Seconds });
            syncStopWatch.Restart();

         } // foreach(VvLookUpItem lui in skladListW_sklNum) 

         syncStopWatch.Stop();

         // 27.05.2014: Lager sva skladista odjemput (ne sklad po sklad (tembo))
         if(this.reportDocument is Vektor.Reports.RIZ.CR_LagerListaPodJM_SvaSkl)
         {
            switch(RptFilter.SorterType_Sifrar)
            {
               case VvSQL.SorterType.Name   : /*return "artiklName"           ;*/ TheArtiklList = TheArtiklList.OrderBy(a => a.ArtiklName).ThenBy(a => a.AS_SkladCD).ToList(); break;
               case VvSQL.SorterType.Code   : /*return "artiklCD"             ;*/ TheArtiklList = TheArtiklList.OrderBy(a => a.ArtiklCD  ).ThenBy(a => a.AS_SkladCD).ToList(); break;
               case VvSQL.SorterType.BarCode: /*return "barCode1, artiklName ";*/ TheArtiklList = TheArtiklList.OrderBy(a => a.BarCode1  ).ThenBy(a => a.AS_SkladCD).ToList(); break;
   
               //default: return "";
            }
         }

      } // if(RptFilter.SkladCD.IsEmpty()) // all_sklad we want 
      // ======AAAAAAAAAAAAAAAAAAAAAAAAA========================================================= 

      // ======BBBBBBBBBBBBBBBBBBBBBBBBB========================================================= 
      else // single skladCD from RptFilter.SkladCD 
      {
         // 15.08.2012: VelaGospa, bijo danas na Grobniku...
         // 13.07.2022: dodan " && isPrmRazdoblja"
         if(RptFilter.DatumOd != ZXC.projectYearFirstDay && isPrmRazdoblja) // znaci trazimo Promet RAZDOBLJA, a validacija dateOD je obavljena ranije. (StanjeSkladista mora ici od 01.01, PrometRazdoblja neSmije ici od 01.01.) 
         {                                                
            List<Artikl> endList   = new List<Artikl>();
            List<Artikl> startList = new List<Artikl>();

            ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, endList  , RptFilter.SkladCD,           RptFilter.DatumDo            , RptFilter, /*fuse*/ "", ArtiklOrderBy);
            ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, startList, RptFilter.SkladCD, /* !!! */ RptFilter.DatumOd.AddDays(-1), RptFilter, /*fuse*/ "", ArtiklOrderBy);

            /*TheArtiklList = */ SetArtiklListForRazdoblje(endList, startList);

#region GetArtstat_SUM_list

            if(shouldGetArtstatListForRekapTT)
            {
               ArtStatDao.GetArtstat_SUM_list(TheDbConnection, true, TheArtStatList, RptFilter.SkladCD, RptFilter.DatumOd, RptFilter.DatumDo, RptFilter);
            }

#endregion GetArtstat_SUM_list

         }
         else // classic 
         {
            ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, TheArtiklList, RptFilter.SkladCD, RptFilter.DatumDo, RptFilter, /*fuse*/ "", ArtiklOrderBy);

            // 29.10.2019: otkrili da ovo smeta za webShopExport: 
          //if(ZXC.IsTEMBO                                              ) // TEMBO svedi na OrgPak = 1 sve NE 'litre' PodJM 
            if(ZXC.IsTEMBO && (this is RptR_TemboWebShopExport) == false) // TEMBO svedi na OrgPak = 1 sve NE 'litre' PodJM 
            {
               foreach(Artikl artikl_rec in TheArtiklList)
               {
                  if(artikl_rec.AS_OrgPakJM.ToUpper().StartsWith("L") == false)
                  {
                     artikl_rec.AS_OrgPak = 1.00M;
                  }
               }
            }

#region GetArtstat_SUM_list

            if(shouldGetArtstatListForRekapTT)
            {
               ArtStatDao.GetArtstat_SUM_list(TheDbConnection, false, TheArtStatList, RptFilter.SkladCD, DateTime.MinValue, RptFilter.DatumDo, RptFilter);
            }

#endregion GetArtstat_SUM_list

#region CheckInvProblems

            if(isInventuraReport)
            {
               thisSkladHasInvProblems = CheckInvProblems(ZXC.luiListaSkladista.GetLuiForThisCd(RptFilter.SkladCD), ref thisSkladHasVisManjDocs);
               if(thisSkladHasInvProblems) anySkladHasInvProblems = true;
            }

#endregion CheckInvProblems

            // For Debug Purposes Sometimes: 
            // TheArtiklList.RemoveAll(ast => ast.AS_DateZadIzlaz.IsEmpty());
         }
      } // else // single skladCD from RptFilter.SkladCD 
      // ======BBBBBBBBBBBBBBBBBBBBBBBBB========================================================= 

      // 29.01.2014: ne zelimo zasebne tablice po skladistima ... Bilo pa proslo ne bumo tak ... 
      //if(this.reportDocument is CR_LagerLista_OP && RptFilter.SkladCD.IsEmpty())
      //{
      //   TheArtiklList.ForEach(aa => aa.AS_SkladCD = "00");
      //
      //   TheDeviznaSumaList = new List<VvReportSourceUtil>();
      //   TheDeviznaSumaList = TheArtiklList.GroupBy(aa => aa.AS_ArtiklTS)
      //}

      // 09.12.2015: 
      // 09.12.2016: 
    //if(RptFilter.IsChkO == true)
    //if(RptFilter.IsChkO == true && shouldGetArtstatList == false) // Dakle, IsChk0 ide samo ako izvj NIJE 'CR_StanjeSklad_B'                         
    //{                                                             // Za 'CR_StanjeSklad_B' IsChk0 sluzi da kazemo da HOCEMO i rekapTT na dnu tablice 
      // malo kasnije vratio da ipak IsChkO radi stari posao i kada je CR_StanjeSklad_B 
      // pa jos kasnije izbacio Promet Razdoblja                                        
      if(RptFilter.IsChk0 == true && isPrmRazdoblja == false)
      {
         bool nijeSveNaNuli  ;
         bool nemaNBCimaKolSt;
         bool nemaMPCimaKolSt;
         bool isMalopSkl;
         foreach(Artikl artikl in TheArtiklList)
         {
            if(artikl.IsFKZ) continue;

          //isMalopSkl = ZXC.luiListaSkladista.GetFlagForThisCd(RptFilter.SkladCD);
            isMalopSkl = ZXC.luiListaSkladista.GetFlagForThisCd(artikl.AS_SkladCD);
         
          //nijeSveNaNuli = artikl.AS_StanjeKol.IsZero() && ( artikl.AS_StanjeFinNBC.NotZero   () || (isMalopSkl &&  artikl.AS_StanjeFinMPC.NotZero   ()));
            nijeSveNaNuli = artikl.AS_StanjeKol.IsZero() && (!artikl.AS_StanjeFinNBC.AlmostZero() || (isMalopSkl && !artikl.AS_StanjeFinMPC.AlmostZero()));
            if(nijeSveNaNuli)
            {
               if(isMalopSkl) ZXC.aim_emsg(MessageBoxIcon.Error, "On artstat [{4}]\n\nArtikl [{0}] na sklad [{3}]\n\nima količinsko stanje nula a financijsko različito od nule!\n\nFinSt NBC: {1}\n\nFinSt MPC: {2}", artikl.ArtiklCD, artikl.AS_StanjeFinNBC.ToStringVv(), artikl.AS_StanjeFinMPC.ToStringVv(), artikl.AS_SkladCD, artikl.TheAsEx.ToString());
               else           ZXC.aim_emsg(MessageBoxIcon.Error, "On artstat [{4}]\n\nArtikl [{0}] na sklad [{3}]\n\nima količinsko stanje nula a financijsko različito od nule!\n\nFinSt NBC: {1}"                  , artikl.ArtiklCD, artikl.AS_StanjeFinNBC.ToStringVv(), artikl.AS_StanjeFinMPC.ToStringVv(), artikl.AS_SkladCD, artikl.TheAsEx.ToString());
            }

            // 16.12.2016: 
            nemaNBCimaKolSt = artikl.AS_PrNabCij.IsZero() && artikl.AS_StanjeKol.NotZero();
            if(nemaNBCimaKolSt)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "On artstat [{3}]\n\nArtikl [{0}] na sklad [{2}]\n\nima količinsko stanje a NEMA NBC!\n\nKolSt: {1}", 
                  artikl.ArtiklCD, 
                  artikl.AS_StanjeKol.ToStringVv(), artikl.AS_SkladCD, artikl.TheAsEx.ToString());
            }
            if(isMalopSkl)
            {
               // 16.12.2016: 
               nemaMPCimaKolSt = artikl.AS_MalopCij.IsZero() && artikl.AS_StanjeKol.NotZero();
               if(nemaMPCimaKolSt)
               {
                  ZXC.aim_emsg(MessageBoxIcon.Error, "On artstat [{3}]\n\nArtikl [{0}] na sklad [{2}]\n\nima količinsko stanje a NEMA MPC!\n\nKolSt: {1}",
                     artikl.ArtiklCD,
                     artikl.AS_StanjeKol.ToStringVv(), artikl.AS_SkladCD, artikl.TheAsEx.ToString());
               }
            } // if(isMalopSkl) check MPC 
         }
      } // if(RptFilter.IsChkO == true) 

      if(anySkladHasInvProblems) // jerbo smo naknadno dodali implicitne artikle koji fale na INV/INM dokumentu, pa sjebasmo sort 
      {
         if(RptFilter.SorterType_Sifrar == VvSQL.SorterType.Name) TheArtiklList = TheArtiklList.OrderBy(art => art./*SkladCD*/AS_SkladCD).ThenBy(art => art.ArtiklName).ToList();
         if(RptFilter.SorterType_Sifrar == VvSQL.SorterType.Code) TheArtiklList = TheArtiklList.OrderBy(art => art./*SkladCD*/AS_SkladCD).ThenBy(art => art.ArtiklCD  ).ToList();
      }

      // 16.09.2016: 
      if(RptFilter.SviArtikli)
      {
         TheArtiklList = TheArtiklList.Distinct().ToList();

         // a mogao si i ovako ostaviti samo distinktivne ako te fuka Equals i GetHascode
         //var distinctItems = items.GroupBy(x => x.Id).Select(y => y.First());
      }

#region GetArtstat_SUM_list

      if(shouldGetArtstatListForRekapTT)
      {
         VvManyDecimalsReportSourceRow manyDecimalsReportSourceRow;
         string theSkladCD;

         foreach(var artiklList in TheArtiklList.GroupBy(art => art.AS_SkladCD))
         {
            theSkladCD = artiklList.First().AS_SkladCD;

            var artstatList = TheArtStatList.Where(ast => ast.SkladCD == theSkladCD);

            manyDecimalsReportSourceRow = new VvManyDecimalsReportSourceRow();

            manyDecimalsReportSourceRow.RowStringCD = theSkladCD;

            manyDecimalsReportSourceRow.DecimA01 = (artiklList.Sum(art => art.AS_UkPstKol  ) - artstatList.Sum(ast => ast.UkPstKol  )).Ron2();
            manyDecimalsReportSourceRow.DecimA02 = (artiklList.Sum(art => art.AS_UkUlazKol ) - artstatList.Sum(ast => ast.UkUlazKol )).Ron2();
            manyDecimalsReportSourceRow.DecimA03 = (artiklList.Sum(art => art.AS_UkIzlazKol) - artstatList.Sum(ast => ast.UkIzlazKol)).Ron2();

            decimal diffPstFinNBC  ;
            decimal diffPstFinMPC  ;
            decimal diffUlazFinNBC ;
            decimal diffUlazFinMPC ;
            decimal diffIzlazFinNBC;
            decimal diffIzlazFinMPC;

            if(isPrmRazdoblja)
            {
               diffPstFinNBC   = artiklList.Sum(art => art.AS_RtrPstVrjNBC)  .Ron2() - artstatList.Sum(ast => ast.UkPstFinNBC)  .Ron2();
               diffPstFinMPC   = artiklList.Sum(art => art.AS_RtrPstVrjNBC)  .Ron2() - artstatList.Sum(ast => ast.UkPstFinKNJ)  .Ron2();
               diffUlazFinNBC  = artiklList.Sum(art => art.AS_RtrUlazVrjNBC) .Ron2() - artstatList.Sum(ast => ast.UkUlazFinNBC) .Ron2();
               diffUlazFinMPC  = artiklList.Sum(art => art.AS_RtrUlazVrjNBC) .Ron2() - artstatList.Sum(ast => ast.UkUlazFinKNJ) .Ron2();
               diffIzlazFinNBC = artiklList.Sum(art => art.AS_RtrIzlazVrjNBC).Ron2() - artstatList.Sum(ast => ast.UkIzlazFinNBC).Ron2();
               diffIzlazFinMPC = artiklList.Sum(art => art.AS_RtrIzlazVrjNBC).Ron2() - artstatList.Sum(ast => ast.UkIzlazFinKNJ).Ron2();
            }
            else // Stanje Skladista 
            {
               diffPstFinNBC   = artiklList.Sum(art => art.AS_UkPstFinNBC)  .Ron2() - artstatList.Sum(ast => ast.UkPstFinNBC)  .Ron2();
               diffPstFinMPC   = artiklList.Sum(art => art.AS_UkPstFinKNJ)  .Ron2() - artstatList.Sum(ast => ast.UkPstFinKNJ)  .Ron2();
               diffUlazFinNBC  = artiklList.Sum(art => art.AS_UkUlazFinNBC) .Ron2() - artstatList.Sum(ast => ast.UkUlazFinNBC) .Ron2();
               diffUlazFinMPC  = artiklList.Sum(art => art.AS_UkUlazFinKNJ) .Ron2() - artstatList.Sum(ast => ast.UkUlazFinKNJ) .Ron2();
               diffIzlazFinNBC = artiklList.Sum(art => art.AS_UkIzlazFinNBC).Ron2() - artstatList.Sum(ast => ast.UkIzlazFinNBC).Ron2();
               diffIzlazFinMPC = artiklList.Sum(art => art.AS_UkIzlazFinKNJ).Ron2() - artstatList.Sum(ast => ast.UkIzlazFinKNJ).Ron2();

             //decimal artiklSUM, artstatSUM;
             //
             //artiklSUM = artiklList.Sum(art => art.AS_UkPstFinNBC)  .Ron2(); artstatSUM = artstatList.Sum(ast => ast.UkPstFinNBC)  .Ron2(); diffPstFinNBC   = artiklSUM - artstatSUM;
             //artiklSUM = artiklList.Sum(art => art.AS_UkPstFinKNJ)  .Ron2(); artstatSUM = artstatList.Sum(ast => ast.UkPstFinKNJ)  .Ron2(); diffPstFinMPC   = artiklSUM - artstatSUM;
             //artiklSUM = artiklList.Sum(art => art.AS_UkUlazFinNBC) .Ron2(); artstatSUM = artstatList.Sum(ast => ast.UkUlazFinNBC) .Ron2(); diffUlazFinNBC  = artiklSUM - artstatSUM;
             //artiklSUM = artiklList.Sum(art => art.AS_UkUlazFinKNJ) .Ron2(); artstatSUM = artstatList.Sum(ast => ast.UkUlazFinKNJ) .Ron2(); diffUlazFinMPC  = artiklSUM - artstatSUM;
             //artiklSUM = artiklList.Sum(art => art.AS_UkIzlazFinNBC).Ron2(); artstatSUM = artstatList.Sum(ast => ast.UkIzlazFinNBC).Ron2(); diffIzlazFinNBC = artiklSUM - artstatSUM;
             //artiklSUM = artiklList.Sum(art => art.AS_UkIzlazFinKNJ).Ron2(); artstatSUM = artstatList.Sum(ast => ast.UkIzlazFinKNJ).Ron2(); diffIzlazFinMPC = artiklSUM - artstatSUM;
            }

            manyDecimalsReportSourceRow.DecimA04 = isForceMPSK_by_NBC ? diffPstFinNBC   : diffPstFinMPC  ;
            manyDecimalsReportSourceRow.DecimA05 = isForceMPSK_by_NBC ? diffUlazFinNBC  : diffUlazFinMPC ;
            manyDecimalsReportSourceRow.DecimA06 = isForceMPSK_by_NBC ? diffIzlazFinNBC : diffIzlazFinMPC;

            TheManyDecimalsList.Add(manyDecimalsReportSourceRow);
         }
      }

#endregion GetArtstat_SUM_list

      // 10.01.2018: On BarcodeSort required, swap Barcode as ArtiklCD 
      if(RptFilter.SorterType_Sifrar == VvSQL.SorterType.BarCode) TheArtiklList.ForEach(art => art.ArtiklCD = art.BarCode1);

#region SVD HALMED potrosnje ... ex ALMP 

      // 11.03.2019: 
      if(reportDocument is Vektor.Reports.RIZ.CR_SVD_ALMP || reportDocument is Vektor.Reports.RIZ.CR_SVD_HALMED)
      {
         List<Halmed_SVD.HALMEDartikl> halmedArtiklList = VvDaoBase.Get_HALMEDartikl_List(TheDbConnection, "");

         Halmed_SVD.HALMEDartikl halmedArtikl;

         decimal lastORG;
         decimal artORG ;
         decimal theORG ;

         bool isPrijava   = reportDocument is Vektor.Reports.RIZ.CR_SVD_ALMP;
         bool isProvjera = !isPrijava                                       ;

         TheArtiklList.RemoveAll(art => art.IsSvdArtGR_Ljek_ == false || art.AS_UkIzlazKol.IsZero()); // Ostavi samo lijekove (10, 90, A0, N0) a koji imaju IZLAZ 

         // 14.03.2022:
         switch(RptFilter.SorterType_Sifrar)
         {
            case VvSQL.SorterType.Name   : TheArtiklList = TheArtiklList.OrderBy(art => art.ArtiklName).ToList(); break;
            case VvSQL.SorterType.Code   : TheArtiklList = TheArtiklList.OrderBy(art => art.ArtiklCD  ).ToList(); break;
            case VvSQL.SorterType.s_lio  : TheArtiklList = TheArtiklList.OrderBy(art => art.AtestBr   ).ToList(); break; 
         }

         List<string> nonUniqueSlioErrMsgs           = new List<string>();
         List<string> ATKinkonzistErrMsgs            = new List<string>();
         List<string> RemovedNegativeFInIzlazErrMsgs = new List<string>();
         List<string> RemovedNeupareniErrMsgs        = new List<string>();
         List<string> HalmedNemaOve_s_lio_Msgs       = new List<string>();

#region Check is s_lio UNIQUE

       //var artiklGrouppedBy_s_lio = TheArtiklList.Where(art => art.AtestBr.NotEmpty()).GroupBy(art => art.AtestBr + art.AS_SkladCD);

         var artiklSifrarLijekova = VvUserControl.ArtiklSifrar.Where(art => art.IsSvdArtGR_Ljek_ && art.AtestBr.NotEmpty()).OrderBy(art => art.AtestBr);

         var artiklGrouppedBy_s_lio = artiklSifrarLijekova.GroupBy(art => art.AtestBr);

         List<Artikl> nonUnique_s_lio_artiklList = new List<Artikl>();
         Artikl sumirani_nonUnique_s_lio_Artikl_rec;

         IEnumerable<Artikl> TheArtGR;

         int nonUNIQUEcounter = 0;
         string eMsg;
         decimal sumirani_nonUnique_s_lio_UkIzlazKol;
         decimal sumirani_nonUnique_s_lio_UkIzlazFinNBC;

         foreach(var lioGR in artiklGrouppedBy_s_lio)
         {
            if(lioGR.Count() > 1 /*&& !RptFilter.IsChk0*/)
            {
               string kumulativniPodaci = "";
               decimal halmedORG = 0M;

               halmedArtikl = halmedArtiklList.SingleOrDefault(ha => ha.s_lio == lioGR.Key);

               if(halmedArtikl.s_lio.NotEmpty())
               {
                  halmedORG = ZXC.ValOrZero_Decimal(halmedArtikl.br_pak, 2);
               }

               TheArtGR = TheArtiklList.Where(art => art.AtestBr == lioGR.Key);

               // DA LI JE UOPĆE atraktivan za prijavu 
               if(TheArtGR.Count().NotZero() && halmedORG.NotZero() && TheArtGR.Sum(art => art.AS_UkIzlazKol).NotZero())
               {
                  sumirani_nonUnique_s_lio_Artikl_rec         = TheArtGR.Last()        .MakeDeepCopy();
                  sumirani_nonUnique_s_lio_Artikl_rec.TheAsEx = TheArtGR.Last().TheAsEx.MakeDeepCopy();

                  sumirani_nonUnique_s_lio_Artikl_rec.TheAsEx.HalmedORG = halmedORG; // ! 

                  sumirani_nonUnique_s_lio_Artikl_rec.AS_UkIzlazKol     = TheArtGR.Sum(art => art.AS_UkIzlazKol   );
                  sumirani_nonUnique_s_lio_Artikl_rec.AS_UkIzlazFinNBC  = TheArtGR.Sum(art => art.AS_UkIzlazFinNBC);

                  nonUnique_s_lio_artiklList.Add(sumirani_nonUnique_s_lio_Artikl_rec);

                //kumulativniPodaci += string.Format("Halmed ORG PAK: {0}\n", sumirani_nonUnique_s_lio_Artikl_rec.TheAsEx.HalmedORG.ToString0Vv());
                //kumulativniPodaci += string.Format("Vektor Izl KOL: {0}\n", sumirani_nonUnique_s_lio_Artikl_rec.AS_UkIzlazKol    .ToStringVv ());
                //kumulativniPodaci += string.Format("Halmed  Cijena: {0}\n", sumirani_nonUnique_s_lio_Artikl_rec.TheAsEx.HalmedCIJ.ToStringVv ());
                //kumulativniPodaci += string.Format("Halmed     BOP: {0}\n", sumirani_nonUnique_s_lio_Artikl_rec.TheAsEx.HalmedBOP.ToString0Vv());
                //kumulativniPodaci += string.Format("Halmed COP-VPC: {0}\n", sumirani_nonUnique_s_lio_Artikl_rec.TheAsEx.HalmedCOP.ToStringVv ());
                //kumulativniPodaci += string.Format("Vektor Izl FIN: {0}\n", sumirani_nonUnique_s_lio_Artikl_rec.AS_UkIzlazFinNBC .ToStringVv ());
                  kumulativniPodaci += string.Format("Halmed ORG PAK: {0}\n", ZXC.ToStringVv_fieldLength(sumirani_nonUnique_s_lio_Artikl_rec.TheAsEx.HalmedORG, 12));
                  kumulativniPodaci += string.Format("Vektor Izl KOL: {0}\n", ZXC.ToStringVv_fieldLength(sumirani_nonUnique_s_lio_Artikl_rec.AS_UkIzlazKol    , 12));
                  kumulativniPodaci += string.Format("Halmed  Cijena: {0}\n", ZXC.ToStringVv_fieldLength(sumirani_nonUnique_s_lio_Artikl_rec.TheAsEx.HalmedCIJ, 12));
                  kumulativniPodaci += string.Format("Halmed     BOP: {0}\n", ZXC.ToStringVv_fieldLength(sumirani_nonUnique_s_lio_Artikl_rec.TheAsEx.HalmedBOP, 12));
                  kumulativniPodaci += string.Format("Halmed COP-VPC: {0}\n", ZXC.ToStringVv_fieldLength(sumirani_nonUnique_s_lio_Artikl_rec.TheAsEx.HalmedCOP, 12));
                  kumulativniPodaci += string.Format("Vektor Izl FIN: {0}\n", ZXC.ToStringVv_fieldLength(sumirani_nonUnique_s_lio_Artikl_rec.AS_UkIzlazFinNBC , 12));

                  eMsg = String.Format("Za HALMED-ovu šifru (s_lio) [{0}] postoji više od jedan povezani artikl!\n", lioGR.Key);

                  foreach(Artikl artikl in lioGR)
                  {
                     sumirani_nonUnique_s_lio_UkIzlazKol    = TheArtGR.Where(theArt => theArt.ArtiklCD == artikl.ArtiklCD).Sum(art => art.AS_UkIzlazKol   );
                     sumirani_nonUnique_s_lio_UkIzlazFinNBC = TheArtGR.Where(theArt => theArt.ArtiklCD == artikl.ArtiklCD).Sum(art => art.AS_UkIzlazFinNBC);

                     eMsg += String.Format("kol: {3} fin: {4} Art: {0} {1} ATK {2}\n", artikl.ArtiklCD, artikl.ArtiklName, artikl.ArtiklCD2,
                      //sumirani_nonUnique_s_lio_UkIzlazKol   .ToStringVv(), 
                      //sumirani_nonUnique_s_lio_UkIzlazFinNBC.ToStringVv());
                        ZXC.ToStringVv_fieldLength(sumirani_nonUnique_s_lio_UkIzlazKol   , 10),
                        ZXC.ToStringVv_fieldLength(sumirani_nonUnique_s_lio_UkIzlazFinNBC, 12));
                  }
                
                //ZXC.aim_emsg(MessageBoxIcon.Error, eMsg);
                
                //eMsg += "\n" + kumulativniPodaci;
                  eMsg = "[" + ++nonUNIQUEcounter + ".] " + eMsg + "\n" + kumulativniPodaci;
                
                  nonUniqueSlioErrMsgs.Add(eMsg);
                
               } //if(TheArtGR.Count().NotZero() && halmedORG.NotZero() && TheArtGR.Sum(art => art.AS_UkIzlazKol).NotZero())

            } // if(lioGR.Count() > 1 /*&& !RptFilter.IsChk0*/) 
         }

         if(nonUniqueSlioErrMsgs.Count.NotZero())
         {
            ZXC.aim_emsg_List(string.Format("{0} Višekratno pridruženih artikala.", nonUniqueSlioErrMsgs.Count), nonUniqueSlioErrMsgs);
         }

#endregion Check is s_lio UNIQUE

#region Fill vektorArtikl data from halmedArtikl data

         // 26.04.2021: otkriven BUG koji moze isplivati bilo gdje gdje se dosada koristi 
         // ...List.RemoveAt(i--)                                                         
         // kada je i=0 skociti ce Exception pa treba sa bool-om                          

         bool wasRemoved     = false;
         int  neupareniCount =     0;

         bool s_lio_OK;

       //foreach(Artikl vektorArtikl in TheArtiklList)
         for(int i=0; i < TheArtiklList.Count; ++i)
         {
            if(wasRemoved)
            {
               wasRemoved = false;
               i--;
            }

            s_lio_OK = false; // s_lio_OK ce biti false: ili ako je AtestBr prazan ili ako je neprazan ali ne postoji u tekucoj halmedovoj tablici 

            if(TheArtiklList[i].AtestBr.NotEmpty()) // znaci, vektorovom artiklu je pridruzen s_lio 
            {
               halmedArtikl = halmedArtiklList.SingleOrDefault(ha => ha.s_lio == TheArtiklList[i].AtestBr);

               if(halmedArtikl.s_lio.NotEmpty()) s_lio_OK = true;

               if(s_lio_OK) 
               {
                  TheArtiklList[i].AS_HalmedORG = ZXC.ValOrZero_Decimal(halmedArtikl.br_pak, 2);

                  TheArtiklList[i].SerNo       = halmedArtikl.s_atk    ; // S_ATK      
                  TheArtiklList[i].Placement   = halmedArtikl.naziv    ; // NAZIV      
                  TheArtiklList[i].Boja        = halmedArtikl.br_pak   ; // BR_PAK     
                  TheArtiklList[i].MadeIn      = halmedArtikl.doza     ; // DOZA       
                  TheArtiklList[i].Url         = halmedArtikl.opis_doze; // OPIS_DOZE  
                  TheArtiklList[i].CarTarifa   = halmedArtikl.mj_ozn   ; // MJ_OZN     
                  TheArtiklList[i].PartNo      = halmedArtikl.obl_ozn  ; // OBL_OZN    
                  TheArtiklList[i].LinkArtCD   = halmedArtikl.s_mj     ; // s_mj       
                  TheArtiklList[i].PdvKat      = halmedArtikl.s_obl    ; // s_obl      
                  TheArtiklList[i].LongOpis    = halmedArtikl.s_par_pro; // s_par_pro  
                  TheArtiklList[i].PrefValName = halmedArtikl.par_naziv; // par_naziv  

                  // 11.03.2022:
                  TheArtiklList[i].SnagaJM     = halmedArtikl.hzzo_kind; // novo!      
                  TheArtiklList[i].PromjerJM   = halmedArtikl.s_lio    ; // novo!      za saznavanje da li s_lio jos uvijek postoji:          
                                                                         // AtestBr == PromjerJM ---> sve OK                                  
                                                                         // AtestBr.NotEmpty() && PromjerJM.IsEmpty() ---> lose/krovi upareno 
                                                                         // s_lio nje postoji u novoj halmedovoj tablici                      

                  if(TheArtiklList[i].ArtiklCD2.SubstringSafe(0, 7) != halmedArtikl.s_atk)
                  {
                   //if(!RptFilter.IsChk0) ZXC.aim_emsg(MessageBoxIcon.Warning, "ATK nekonzistentnost!\n\nPovezane šifre nemaju isti ATK (prvih 7 znamenki)\n\nVektor Artikl:\n{0} {1}\nATK: {2}\n\nHalmedArtikl:\n{3} {4}\nATK: {5}",
                   //                         TheArtiklList[i].ArtiklCD, TheArtiklList[i].ArtiklName, TheArtiklList[i].ArtiklCD2, 
                   //                         halmedArtikl.naziv, halmedArtikl.opis_doze, halmedArtikl.s_atk);
                     ATKinkonzistErrMsgs.Add(String.Format("Povezanima se razlikuje prvih 7 ATK znamenki: ATK: {2} {0} {1}\n" +
                                                           "                                HalmedArtikl: ATK: {5} {3} {4}\n",
                                              TheArtiklList[i].ArtiklCD, TheArtiklList[i].ArtiklName, TheArtiklList[i].ArtiklCD2,
                                              halmedArtikl.naziv, halmedArtikl.opis_doze, halmedArtikl.s_atk));

                  }

                  if(TheArtiklList[i].AS_UkIzlazFinNBC.IsNegative())
                  {
                   //if(RptFilter.IsChk0) ZXC.aim_emsg(MessageBoxIcon.Error, "IZBACUJEM artikl sa NEGATIVNI fin. izlaz!\n\nArtikl:\n{0} {1}\nATK: {2}\n\nFIN izlaz: {3}",
                   //                         TheArtiklList[i].ArtiklCD, TheArtiklList[i].ArtiklName, TheArtiklList[i].ArtiklCD2, TheArtiklList[i].AS_UkIzlazFinNBC.ToStringVv());
                     RemovedNegativeFInIzlazErrMsgs.Add(String.Format("IZBACUJEM artikl sa NEGATIVNI fin. izlaz!\n\nArtikl:\n{0} {1}\nATK: {2}\n\nFIN izlaz: {3}",
                                              TheArtiklList[i].ArtiklCD, TheArtiklList[i].ArtiklName, TheArtiklList[i].ArtiklCD2, TheArtiklList[i].AS_UkIzlazFinNBC.ToStringVv()));

                   //if(isExport)
                     {
                        // 26.04.2021: otkriven BUG koji moze isplivati bilo gdje gdje se dosada koristi 
                      //TheArtiklList.RemoveAt(i--);
                        TheArtiklList.RemoveAt(i/*--*/);
                        wasRemoved = true;
                     }
                  }

               } // if(halmedArtikl.s_lio.NotEmpty()) 

               else // iz nekog razloga, ovaj se s_lio ne moze naci u 'HALMED_artikl' MySQL tablici (rucno-krivo uparen ili novi artikla a stara HALMED_artikl tablica) 
               {
                  HalmedNemaOve_s_lio_Msgs.Add(String.Format("s_lio [{3}] NE POSTOJI u HALMEDOVOJ tablici: {0} {1} ATK: {2}",
                                           TheArtiklList[i].ArtiklCD, TheArtiklList[i].ArtiklName, TheArtiklList[i].ArtiklCD2,
                                           TheArtiklList[i].AtestBr));

                //TheArtiklList[i].PromjerJM = "PUSE s_lio";
                  TheArtiklList[i].PromjerJM = TheArtiklList[i].AtestBr;
                  TheArtiklList[i].AtestBr   = "";

                  if(isPrijava && TheReportUC.TheRiskFilterUC.Fld_JeliJe_HALMED_Uparen != ZXC.UparenostKind.NE_uparen)
                  {
                     TheArtiklList.RemoveAt(i/*--*/);
                     wasRemoved = true;
                  }
               }

            } // if(TheArtiklList[i].AtestBr.NotEmpty()) (s_lio JE pridružen)

            else // NEUPARENI TheArtiklList[i] (prazan AtestBr - s_lio) 
            {
             //if(isPrijava) ... ALI nemoj izbacivati ako bas zelis samo neuparene da ih prijavis HALMED-u 
               if(isPrijava && TheReportUC.TheRiskFilterUC.Fld_JeliJe_HALMED_Uparen != ZXC.UparenostKind.NE_uparen)
               {
                  // 26.04.2021: otkriven BUG koji moze isplivati bilo gdje gdje se dosada koristi 
                //TheArtiklList.RemoveAt(i--);

                //if(!RptFilter.IsChk0) ZXC.aim_emsg(MessageBoxIcon.Error, "NEUPARENI artikl koji je LIJEK te ima izlazni promet!\n\nVektor Artikl:\n{0} {1}\nATK: {2}",
                //                         TheArtiklList[i].ArtiklCD, TheArtiklList[i].ArtiklName, TheArtiklList[i].ArtiklCD2);
                  RemovedNeupareniErrMsgs.Add(String.Format("[{3}.] NEUPARENI artikl {0} {1} ATK: {2}",
                                           TheArtiklList[i].ArtiklCD, TheArtiklList[i].ArtiklName, TheArtiklList[i].ArtiklCD2, ++neupareniCount));

                  TheArtiklList.RemoveAt(i/*--*/);
                  wasRemoved = true;

               }
            }

         } // foreach(Artikl vektorArtikl in TheArtiklList) 

         if(HalmedNemaOve_s_lio_Msgs.Count.NotZero())
         {
            ZXC.aim_emsg_List(string.Format("{0} Nepostojećih s_lio šifri.", HalmedNemaOve_s_lio_Msgs.Count), HalmedNemaOve_s_lio_Msgs);
         }

         if(ATKinkonzistErrMsgs.Count.NotZero())
         {
            ZXC.aim_emsg_List(string.Format("{0} ATK nekonzistentnosti.", ATKinkonzistErrMsgs.Count), ATKinkonzistErrMsgs);
         }

         if(RemovedNegativeFInIzlazErrMsgs.Count.NotZero())
         {
            ZXC.aim_emsg_List(string.Format("{0} Izbačeni artikli zbog NEGATIVNOG fin. izlaza.", RemovedNegativeFInIzlazErrMsgs.Count), RemovedNegativeFInIzlazErrMsgs);
         }

         if(RemovedNeupareniErrMsgs.Count.NotZero())
         {
            ZXC.aim_emsg_List(string.Format("{0} Izbačeni NEUPARENI artikli.", RemovedNeupareniErrMsgs.Count), RemovedNeupareniErrMsgs);
         }

#endregion Fill vektorArtikl data from halmedArtikl data

#region SUM As Kumulativ Stanja na svim skladistima

         // List<Artikl> analiticArtiklList = TheArtiklList.ToList(); // ...a li ovo ti je primjer kako nabrzake klonirati listu .clone 

         var artiklGroups = TheArtiklList.GroupBy(art => art.ArtiklCD).ToList(); // ToList() da prekine shallow copy 

         TheArtiklList.Clear();

         Artikl sumiraniArtikl_rec;

         foreach(var artGR in artiklGroups)
         {
            sumiraniArtikl_rec         = artGR.Last()        .MakeDeepCopy();
            sumiraniArtikl_rec.TheAsEx = artGR.Last().TheAsEx.MakeDeepCopy();

            sumiraniArtikl_rec.TheAsEx.HalmedORG = artGR.Last().TheAsEx.HalmedORG; // ! 

            sumiraniArtikl_rec.AS_UkIzlazKol    = artGR.Sum(art => art.AS_UkIzlazKol   );
            sumiraniArtikl_rec.AS_UkIzlazFinNBC = artGR.Sum(art => art.AS_UkIzlazFinNBC);

            TheArtiklList.Add(sumiraniArtikl_rec);
         }

         // 14.03.2022:
       //TheArtiklList = TheArtiklList.OrderBy(art => art.ArtiklName).ToList();
         switch(RptFilter.SorterType_Sifrar)
         {
            case VvSQL.SorterType.Name   : TheArtiklList = TheArtiklList.OrderBy(art => art.ArtiklName).ToList(); break;
            case VvSQL.SorterType.Code   : TheArtiklList = TheArtiklList.OrderBy(art => art.ArtiklCD  ).ToList(); break;
            case VvSQL.SorterType.s_lio  : TheArtiklList = TheArtiklList.OrderBy(art => art.AtestBr   ).ToList(); break; 
         }

#endregion SUM As Kumulativ Stanja na svim skladistima

#region Set theORG 

         ZXC.ErrorsList = new List<string>();

         foreach(Artikl artikl in TheArtiklList)
         {
            lastORG = RtransDao.GetLastUsed_URA_ORG(TheDbConnection, artikl.ArtiklCD, artikl.ArtiklCD2, true);

            artORG = ZXC.ValOrZero_Decimal(artikl.OrgPak, 0);

            theORG = lastORG.NotZero() ? lastORG :
                     artORG .NotZero() ? artORG  :
                     1;

            artikl.TheAsEx.OrgPak = theORG;
         }

         // !!! Ovo je u biti PUSE jer od 2020.g. koristimo ORG iz HALMED-ovog sifrarnika 
         // 06.05.2021. ugasili
       //if(ZXC.ErrorsList.NotEmpty())
       //{
       //   string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\Vv_ORG_inkonzist_ErrorList" + " @ " + DateTime.Now.ToString(ZXC.VvTimeStampFormat4FileName) + "." + "txt";
       //
       //   ZXC.aim_emsg(MessageBoxIcon.Error, "OBAVEZNO!!!\n\nProvjeriti datoteku\n\n{0}\n\nNekonzistentnost ORGa!", fName);
       //
       //   using(System.IO.StreamWriter sw = new System.IO.StreamWriter(fName, false, System.Text.Encoding.GetEncoding(1250)))
       //     {
       //        foreach(string errorMessage in ZXC.ErrorsList)
       //        {
       //           sw.WriteLine(errorMessage);
       //        }
       //     }
       //}

         ZXC.ErrorsList = null;

#endregion Set theORG 

#region Izbaci neželjene (SVI ili samoNEUPARENI ili samoUPARENI)

         // Izbaci NEUPARENE: 
         if(TheReportUC.TheRiskFilterUC.Fld_JeliJe_HALMED_Uparen == ZXC.UparenostKind.DA_uparen)
         {
            TheArtiklList.RemoveAll(art => art.AtestBr.IsEmpty()); 
         }
         // Izbaci UPARENE: 
         if(TheReportUC.TheRiskFilterUC.Fld_JeliJe_HALMED_Uparen == ZXC.UparenostKind.NE_uparen)
         {
            TheArtiklList.RemoveAll(art => art.AtestBr.NotEmpty());
         }

         #endregion Izbaci neželjene (SVI ili samoNEUPARENI ili samoUPARENI)

         #region Kreiraj HALMED error txt fajlove da ih saljes Gabrijeli 

         Create_HALMED_errorTxtFile(HalmedNemaOve_s_lio_Msgs      , @"\Vv_HalmedNemaOve_s_lio_Msgs"      );
         Create_HALMED_errorTxtFile(nonUniqueSlioErrMsgs          , @"\Vv_nonUniqueSlioErrMsgs"          );
         Create_HALMED_errorTxtFile(ATKinkonzistErrMsgs           , @"\Vv_ATKinkonzistErrMsgs"           );
         Create_HALMED_errorTxtFile(RemovedNegativeFInIzlazErrMsgs, @"\Vv_RemovedNegativeFInIzlazErrMsgs");
         Create_HALMED_errorTxtFile(RemovedNeupareniErrMsgs       , @"\Vv_RemovedNeupareniErrMsgs"       );

         #endregion Kreiraj HALMED error txt fajlove da ih saljes Gabrijeli 

      } //if(reportDocument is Vektor.Reports.RIZ.CR_SVD_ALMP || reportDocument is Vektor.Reports.RIZ.CR_SVD_HALMED)

      #endregion SVD HALMED potrosnje ... ex ALMP 

      // 21.01.2022: 
      if(ZXC.IsSvDUH)
      {
         if(RptFilter.SVD_LiP == ZXC.PdvZPkindEnum.SVD_LJEK)
         {
            TheArtiklList.RemoveAll(art => art.IsSvdArtGR_Ljek_ == false);
         }
         if(RptFilter.SVD_LiP == ZXC.PdvZPkindEnum.SVD_POTR)
         {
            TheArtiklList.RemoveAll(art => art.IsSvdArtGR_Potr_ == false);
         }
      }

      // 04.07.2022: 
      if(isInventuraReport && RptFilter.ZaExportToExcel) 
      {
         TheArtiklList.RemoveAll(art => art.HasInvVisOrManj == false);
      }

      // 06.07.2022: 
      if(isPrmRazdoblja) 
      {
         TheArtiklList.RemoveAll(art => art.AS_UkUlazKol.IsZero() && art.AS_UkIzlazKol.IsZero()); // makni van one koji NEMAJU promet u zadanom razdoblju 
      }

      // 28.07.2022: for metaflex etikete 
      if(ZXC.vvDB_VvDomena == "vvMN" && reportDocument is Vektor.Reports.RIZ.CR_LagreEtikete)
      {
         Rtrans lastMalopUlaz_rtrans_rec;
         Kupdob lastURM_kupdob_rec;
         string lastDobavNaziv;

         bool lastMalopUlaz_rtrans_Found;

         foreach(Artikl artikl in TheArtiklList)
         {
            lastMalopUlaz_rtrans_rec = new Rtrans();

            lastMalopUlaz_rtrans_Found = FakturDao.SetMeLastRtransForArtiklAnd_anyOfTheseTTs(TheDbConnection, lastMalopUlaz_rtrans_rec,
               new string[] { Faktur.TT_PSM, Faktur.TT_KLK, Faktur.TT_URM }, artikl.ArtiklCD, /*false*/true);

            if(lastMalopUlaz_rtrans_Found == false) continue;

            if(lastMalopUlaz_rtrans_rec.T_TT == Faktur.TT_PSM)
            {
               lastDobavNaziv = "POČETNO STANJE - PSM";
            }
            else
            {
               lastURM_kupdob_rec = TheReportUC.Get_Kupdob_FromVvUcSifrar(lastMalopUlaz_rtrans_rec.T_kupdobCD);

               if(lastURM_kupdob_rec != null) lastDobavNaziv = lastURM_kupdob_rec.Naziv;
               else                           lastDobavNaziv = "nepoznato";
            }

            lastMalopUlaz_rtrans_rec.CalcTransResults(null);

          //artikl.Starost   = lastMalopUlaz_rtrans_rec.T_cij                                                          ; // MPC sa URM-a u kunama 
          //artikl.Starost   = lastMalopUlaz_rtrans_rec.T_wanted                                                       ; // MPC sa URM-a u kunama je T_wanted 
            artikl.Starost   = lastMalopUlaz_rtrans_rec.R_CIJ_MSK                                                      ; // MPC sa URM-a u kunama 
            artikl.ImportCij = ZXC.EURiIzKuna_HRD_(artikl.Starost)                                                     ; // EUR po fix tecaju     
            artikl.MadeIn    = lastDobavNaziv                                                                          ; // DobavName             
            artikl.CarTarifa = Faktur.Set_TT_And_TtNum(lastMalopUlaz_rtrans_rec.T_TT, lastMalopUlaz_rtrans_rec.T_ttNum); // TipBr                 
         }
      }

      if(HRDweWant)
      {
         TheArtiklList.ForEach(art => 
         { 
            art.AS_UkPstFinNBC   = ZXC.EURiIzKuna_ILI_KuneIzEURa_HRD_(art.AS_UkPstFinNBC  ); 
            art.AS_UkUlazFinNBC  = ZXC.EURiIzKuna_ILI_KuneIzEURa_HRD_(art.AS_UkUlazFinNBC ); 
            art.AS_UkIzlazFinNBC = ZXC.EURiIzKuna_ILI_KuneIzEURa_HRD_(art.AS_UkIzlazFinNBC); 
                                       
            art.AS_UkPstFinMPC   = ZXC.EURiIzKuna_ILI_KuneIzEURa_HRD_(art.AS_UkPstFinMPC  ); 
            art.AS_UkUlazFinMPC  = ZXC.EURiIzKuna_ILI_KuneIzEURa_HRD_(art.AS_UkUlazFinMPC ); 
            art.AS_UkIzlazFinMPC = ZXC.EURiIzKuna_ILI_KuneIzEURa_HRD_(art.AS_UkIzlazFinMPC); 
         });
      }

   } // private  void GetArtiklWithArtstatList() 

   private void Create_HALMED_errorTxtFile(List<string> errMessageList, string errFileName)
   {
      if(errMessageList.NotEmpty())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + errFileName + " @ " + DateTime.Now.ToString(ZXC.VvTimeStampFormat4FileName) + "." + "txt";
         using(System.IO.StreamWriter sw = new System.IO.StreamWriter(fName, false, System.Text.Encoding.GetEncoding(1250)))
         {
            foreach(string errorMessage in errMessageList)
            {
               sw.WriteLine(errorMessage);
            }
         }
      }
   }

   private bool CheckInvProblems(VvLookUpItem lui, ref bool thisSkladHasVisManjDocs)
   {
      bool hasInvProblems = false; bool overflowAttached = false;
      string invTT = lui.Flag ? Faktur.TT_INM : Faktur.TT_INV;
      string mnjTT = lui.Flag ? Faktur.TT_IZM : Faktur.TT_IZD;
      string visTT = lui.Flag ? Faktur.TT_KLK : Faktur.TT_PRI;
      DateTime dateInv;
      try
      {
         dateInv = TheArtiklList.Count.NotZero() ? TheArtiklList.Where(art => art.AS_DateZadInv.NotEmpty()).FirstOrDefault().AS_DateZadInv : DateTime.MinValue;
      }
      catch(NullReferenceException /*ex*/)
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Za sklad [{0}] nema dokumenta inventure", lui.Cd);
         //dateInv = RptFilter.DatumDo;
         return false;
      }

      // 05.07.2022: modificirana ova metoda nanacinda Transovi inv_mnj_vis nisu vise samo od 1 dokumenta 
      // nego je za ocekivati i vise njih                                                                 
    //Faktur fakturINV_rec = new Faktur(); // INVENTURA dokument 
    //Faktur fakturMNJ_rec = new Faktur(); // IZD/IZM MANJKA     
    //Faktur fakturVIS_rec = new Faktur(); // PRI/KLK VISKA      
      //bool invDocFound = FakturDao.SetMeFaktur_BySklad_And_TT_And_Date(TheDbConnection, fakturINV_rec, lui.Cd, invTT, dateInv, true);
      //bool mnjDocFound = FakturDao.SetMeFaktur_BySklad_And_TT_And_Date(TheDbConnection, fakturMNJ_rec, lui.Cd, mnjTT, dateInv, true);
      //bool visDocFound = FakturDao.SetMeFaktur_BySklad_And_TT_And_Date(TheDbConnection, fakturVIS_rec, lui.Cd, visTT, dateInv, true);

      List<Rtrans> INV_Transes = RtransDao.Get_RtransList_For_Sklad_And_TT_And_Date(TheDbConnection, lui.Cd, invTT, dateInv);
      List<Rtrans> MNJ_Transes = RtransDao.Get_RtransList_For_Sklad_And_TT_And_Date(TheDbConnection, lui.Cd, mnjTT, dateInv); ;
      List<Rtrans> VIS_Transes = RtransDao.Get_RtransList_For_Sklad_And_TT_And_Date(TheDbConnection, lui.Cd, visTT, dateInv); ;

      bool invDocFound = INV_Transes.NotEmpty();
      bool mnjDocFound = MNJ_Transes.NotEmpty();
      bool visDocFound = VIS_Transes.NotEmpty();

      thisSkladHasVisManjDocs = (visDocFound || mnjDocFound);
      lui.R_Bool = thisSkladHasVisManjDocs;

      bool isStanje_isNotRazlike = !RptFilter.IsTrue;

      if(invDocFound)
      {
       //fakturINV_rec.VvDao.LoadTranses(TheDbConnection, fakturINV_rec, false);
       //fakturMNJ_rec.VvDao.LoadTranses(TheDbConnection, fakturMNJ_rec, false);

       //List<string> rtransYesManjNoInventuraList = ArtiklDao.Get_NoInventura_YesManjak_RtransList(fakturMNJ_rec.Transes, fakturINV_rec.Transes);
         List<string> rtransYesManjNoInventuraList = ArtiklDao.Get_NoInventura_YesManjak_RtransList(      MNJ_Transes    ,       INV_Transes);

         Artikl implicitArtikl_rec;
         Rtrans manjakRtrans_rec;
         int count = 0, maxCount = 16;

         if(rtransYesManjNoInventuraList.Count.NotZero()) // znaci, IMA problema 
         {
            hasInvProblems = true;
            string errMessage = "";

            foreach(string artiklCD in rtransYesManjNoInventuraList)
            {
#region isStanje_isNotRazlike - Add artifitial implict artikl in TheArtiklList

               if(isStanje_isNotRazlike)
               {
                //manjakRtrans_rec = fakturMNJ_rec.Transes.First(rtr => rtr.T_artiklCD == artiklCD).MakeDeepCopy();
                  manjakRtrans_rec =           MNJ_Transes.First(rtr => rtr.T_artiklCD == artiklCD).MakeDeepCopy();
                
                  implicitArtikl_rec = new Artikl()
                     {
                        ArtiklCD   = artiklCD,
                        ArtiklName = manjakRtrans_rec.T_artiklName,
                        SkladCD    = manjakRtrans_rec.T_skladCD,
                     };
                
                  implicitArtikl_rec.TheAsEx = ArtiklDao.GetArtiklStatus(TheDbConnection, manjakRtrans_rec);
                
                  if(TheArtiklList.Count(art => art.ArtiklCD == artiklCD && art.TheAsEx.SkladCD == manjakRtrans_rec.T_skladCD).NotZero())
                  {
                     // ...znaci RptFilter.SamoArtikliSaStanjem NIJE checkiran pa ovoga ima u TheArtiklList-i 
                     // ali je krnji pa mu treba dopuniti ArtStat varijable kao da ima INV/INM 
                     // kako je element liste immutuable, prvo ga treba izbaciti pa onda novoga dodati 
                     TheArtiklList.RemoveAll(art => art.ArtiklCD == artiklCD && art.TheAsEx.SkladCD == manjakRtrans_rec.T_skladCD);
                  }
                
                  implicitArtikl_rec.AS_IsShadowInventura = true;
                  // Ako je ovo true, tada u izvj inventure 
                  // InvKol_Manjk_BEF    treba zamjeniti sa StanjeKol    
                  // InvFinNBC_Manjk_BEF treba zamjeniti sa StanjeFinNBC 
                  // InvFinMPC_Manjk_BEF treba zamjeniti sa StanjeFinMPC 
                
                  TheArtiklList.Add(implicitArtikl_rec);
                  
               } // if(isStanje_isNotRazlike)

#endregion isStanje_isNotRazlike - Add artifitial implict artikl in TheArtiklList

               if(++count <= maxCount)
               {
                //errMessage += "Artikl [" + artiklCD + "] sklad [" + lui.Cd + "]\n\nIma inventurni manjak (nastao VIŠ/MANJ procedurom)\n\na NEMA ga na dokumentu inventure\n\npa NEĆE izaći na ovom izvještaju!\n";
                  errMessage += "Artikl [" + artiklCD + "] sklad [" + lui.Cd + "]\n";
               }
             //else 
               else if(overflowAttached == false)
               {
                  errMessage += "\n... i još [" + (rtransYesManjNoInventuraList.Count - count + 1).ToString() + "] artikla ...";
                  //break; !!!!!!!!! bijo bug ! 
                  overflowAttached = true;
               }

            } // foreach(string artiklCD in rtransYesManjNoInventuraList) 

            errMessage += "\n\nPostoji na izdatnici inventurnog manjka (nastaloj VIŠ/MANJ procedurom) a artikla NEMA na dokumentu inventure pa se pretpostavlja da mu kao inventurno stanje zadajete stanje 0, te je implicitno dodan na ovaj izvještaj sa stanjem 0.";

            MessageBox.Show(errMessage, "ARTIKLI KOJI SU IMPLICITNO DODANI NA IZVJEŠTAJ INVENTURE!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

         } // if(badList.Count.NotZero()) // znaci, IMA problema 

      } // if(invDocFound)

      return hasInvProblems;
   }

   private /*List<Artikl>*/ void SetArtiklListForRazdoblje(List<Artikl> endList, List<Artikl> startList)
   {
      foreach(Artikl endArtikl_rec in endList)
      {
         Artikl startArtikl_rec = startList.SingleOrDefault(start => start.ArtiklCD == endArtikl_rec.ArtiklCD);

         // 16.12.2016: pitamo se: 'A zasto smo do sada izbacivali artikle koji nemaju promjenu u razdoblju?' 
         // te taj uvjet gasimo                                                                               
       //if(startArtikl_rec == null ||
       //   startArtikl_rec.AS_UkUlazKol  != endArtikl_rec.AS_UkUlazKol ||
       //   startArtikl_rec.AS_UkIzlazKol != endArtikl_rec.AS_UkIzlazKol )
         {
            TheArtiklList.Add(GetArtiklStatusForRazdoblje(endArtikl_rec, startArtikl_rec));
         }
      }
   }

   private Artikl GetArtiklStatusForRazdoblje(Artikl endArtikl_rec, Artikl startArtikl_rec)
   {
      Artikl artiklRazdoblje = endArtikl_rec.MakeDeepCopy();
      artiklRazdoblje.TheAsEx = new ArtStat();
      //artiklRazdoblje.TheAsEx = endArtikl_rec.TheAsEx.MakeDeepCopy();
      artiklRazdoblje.AS_ArtiklTS = endArtikl_rec.AS_ArtiklTS; // ova dva su potrebna jer se po njima GRUPIRA, a ako je podatak po kojem se grupira PTAZAN onda row ne izlazi uprkos tome sto postoji u listi! 
      artiklRazdoblje.AS_SkladCD  = endArtikl_rec.AS_SkladCD;
      artiklRazdoblje.AS_OrgPak   = endArtikl_rec.AS_OrgPak ;

      //if(artiklRazdoblje.AS_OrgPakJM.IsEmpty() && artiklRazdoblje.AS_OrgPak == 1) artiklRazdoblje.AS_OrgPak = 0; // !!! 

      bool isForceNBC_for_MPSK = RptFilter.FuseBool1; // RptFilter.FuseBool1 je Fld_IsNbcZaMPSK 

      if(startArtikl_rec == null) // znaci artikl je dobio promet tek u razdoblju, prije ga nije ni bilo 
      {
         startArtikl_rec = new Artikl();
      }

      // KOLICINE: 
      /* Stanje pocetak razdoblja */ artiklRazdoblje.AS_UkUlazKolFisycal  = startArtikl_rec.AS_StanjeKol;
      /* U Razdoblju - ULAZ       */ artiklRazdoblje.AS_UkUlazKol         = endArtikl_rec  .AS_UkUlazKol  - startArtikl_rec.AS_UkUlazKol ;
      /* U Razdoblju - IZLAZ      */ artiklRazdoblje.AS_UkIzlazKol        = endArtikl_rec  .AS_UkIzlazKol - startArtikl_rec.AS_UkIzlazKol;
      /* U Razdoblju - SALDO      */ artiklRazdoblje.AS_UkStanjeKolRezerv = endArtikl_rec  .AS_StanjeKol;
    //Stanje kraj razdoblja       */ artiklRazdoblje.AS_StanjeKol         = artiklRazdoblje.AS_UkUlazKol  - artiklRazdoblje.AS_UkIzlazKol; IMPLICITNO - result 

      // KOLICINE OP: 
      /* Stanje pocetak razdoblja */ artiklRazdoblje.AS_PreDefVpc1 = artiklRazdoblje.AS_UkUlazKolFisycal  * artiklRazdoblje.AS_OrgPak;
      /* U Razdoblju - ULAZ       */ artiklRazdoblje.AS_PreDefVpc2 = artiklRazdoblje.AS_UkUlazKol         * artiklRazdoblje.AS_OrgPak;
      /* U Razdoblju - IZLAZ      */ artiklRazdoblje.AS_PreDefMpc1 = artiklRazdoblje.AS_UkIzlazKol        * artiklRazdoblje.AS_OrgPak;
      /* U Razdoblju - SALDO      */ artiklRazdoblje.AS_PreDefDevc = artiklRazdoblje.AS_UkStanjeKolRezerv * artiklRazdoblje.AS_OrgPak;

      // FINANCIJSKI: 
      // 17.01.2013: rastavljeno na NBC/MPC zbog force NBC on MPSK 
      if(isForceNBC_for_MPSK)
      {
         artiklRazdoblje.AS_RtrPstVrjNBC   = startArtikl_rec.AS_StanjeFinNBC                                     ;
         artiklRazdoblje.AS_RtrUlazVrjNBC  = endArtikl_rec  .AS_UkUlazFinNBC  - startArtikl_rec.AS_UkUlazFinNBC  ;
         artiklRazdoblje.AS_RtrIzlazVrjNBC = endArtikl_rec  .AS_UkIzlazFinNBC - startArtikl_rec.AS_UkIzlazFinNBC ;
         artiklRazdoblje.AS_RtrCijenaNBC   = artiklRazdoblje.AS_RtrUlazVrjNBC - artiklRazdoblje.AS_RtrIzlazVrjNBC;
         artiklRazdoblje.AS_RtrCijenaMPC   = endArtikl_rec  .AS_StanjeFinNBC;
      }
      else
      {
         artiklRazdoblje.AS_RtrPstVrjNBC   = startArtikl_rec.AS_StanjeFinKNJ                                     ;
         artiklRazdoblje.AS_RtrUlazVrjNBC  = endArtikl_rec  .AS_UkUlazFinKNJ  - startArtikl_rec.AS_UkUlazFinKNJ  ;
         artiklRazdoblje.AS_RtrIzlazVrjNBC = endArtikl_rec  .AS_UkIzlazFinKNJ - startArtikl_rec.AS_UkIzlazFinKNJ ;
         artiklRazdoblje.AS_RtrCijenaNBC   = artiklRazdoblje.AS_RtrUlazVrjNBC - artiklRazdoblje.AS_RtrIzlazVrjNBC;
         artiklRazdoblje.AS_RtrCijenaMPC   = endArtikl_rec  .AS_StanjeFinKNJ;
      }

      // Kolone za report: 

      // 1. KOL: Stanje pocetak razdoblja [artiklRazdoblje.AS_UkUlazKolFisycal ] 
      // 2. KOL: U Razdoblju - ULAZ       [artiklRazdoblje.AS_UkUlazKol        ] 
      // 3. KOL: U Razdoblju - IZLAZ      [artiklRazdoblje.AS_UkIzlazKol       ] 
      // 4. KOL: U Razdoblju - SALDO      [artiklRazdoblje.AS_StanjeKol        ] 
      // 5. KOL: Stanje kraj razdoblja    [artiklRazdoblje.AS_UkStanjeKolRezerv] 

      // 1. FIN: Stanje pocetak razdoblja [artiklRazdoblje.AS_RtrPstVrjNBC     ] 
      // 2. FIN: U Razdoblju - ULAZ       [artiklRazdoblje.AS_RtrUlazVrjNBC    ] 
      // 3. FIN: U Razdoblju - IZLAZ      [artiklRazdoblje.AS_RtrIzlazVrjNBC   ] 
      // 4. FIN: U Razdoblju - SALDO      [artiklRazdoblje.AS_RtrCijenaNBC     ] 
      // 5. FIN: Stanje kraj razdoblja    [artiklRazdoblje.AS_RtrCijenaMPC     ] 

      return artiklRazdoblje;
   }

   private void GetFaktursKupdobList()
   {
#region ForInternDocuments podmetni SkladName as PartnerName

      if(ZXC.TtInfo(RptFilter.TT).IsExtendableTT == false )
      {
         VvLookUpItem lui;

         TheFakturList.ForEach(delegate(Faktur fak)
         {
            if(fak.TtInfo.IsExtendableTT == false)
            {
               string skladCD_AsKupdobCD = GetSkladCD_AsKupdobCD_ForInternDocuments(fak);

               lui = ZXC.luiListaSkladista.GetLuiForThisCd(skladCD_AsKupdobCD);

               if(lui != null)
               {
                  fak.KupdobCD = (uint)lui.Integer + sklCdAsKupdobCd_dodatak;
                  fak.KupdobName = "   Skladište: " + lui.Name;
               }
            }
         });

      }

#endregion ForInternDocuments podmetni SkladName as PartnerName

      TheKupdobList = VvUserControl.KupdobSifrar.Join(TheFakturList, kpdb => kpdb.KupdobCD, faktur => faktur.KupdobCD, (k, f) => k).Distinct().ToList();
      ThePosJedList = VvUserControl.KupdobSifrar.Join(TheFakturList, kpdb => kpdb.KupdobCD, faktur => faktur.PosJedCD, (k, f) => k).Distinct().ToList();

      //AddAllSkladAsArtificialKupdob(TheKupdobList);
      List<Kupdob> theMtrosList = VvUserControl.KupdobSifrar.Join(TheFakturList.Where(f => f.MtrosCD.NotZero()), kpdb => kpdb.KupdobCD, faktur => faktur.MtrosCD, (k, f) => k).Distinct().ToList();
      TheKupdobList = TheKupdobList.Union(theMtrosList).ToList();

#region WEB_DEMO_DATA
#if(WWWDEMO)

      VvUserControl.KupdobSifrar = null;
      TheReportUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.None); // Get back normal sifrar 

#endif
#endregion WEB_DEMO_DATA

   }

#region WEB_DEMO_DATA
#if(WWWDEMO)

   protected static void DemoiseKupdobSifrarList(List<Kupdob> theList)
   {
      for(int i = 0, w = 0; i < theList.Count; ++i, ++w)
      {
         //if(w == demoKupdobs.Length) w = 0;
         //theList[i].Naziv  = demoKupdobs[w].TheName;
         //theList[i].Ticker = demoKupdobs[w].TheCd  ;

         switch(theList[i].Ticker.ToLower()[0])
         {
            case 'a':
               theList[i].Naziv = demoKupdobs[0].TheName;
               theList[i].Ticker = demoKupdobs[0].TheCd; break;
            case 'b':
               theList[i].Naziv = demoKupdobs[1].TheName;
               theList[i].Ticker = demoKupdobs[1].TheCd; break;
            case 'c':
               theList[i].Naziv = demoKupdobs[2].TheName;
               theList[i].Ticker = demoKupdobs[2].TheCd; break;
            case 'd':
               theList[i].Naziv = demoKupdobs[3].TheName;
               theList[i].Ticker = demoKupdobs[3].TheCd; break;
            case 'e':
               theList[i].Naziv = demoKupdobs[4].TheName;
               theList[i].Ticker = demoKupdobs[4].TheCd; break;
            case 'f':
               theList[i].Naziv = demoKupdobs[5].TheName;
               theList[i].Ticker = demoKupdobs[5].TheCd; break;
            case 'g':
               theList[i].Naziv = demoKupdobs[6].TheName;
               theList[i].Ticker = demoKupdobs[6].TheCd; break;
            case 'h':
               theList[i].Naziv = demoKupdobs[7].TheName;
               theList[i].Ticker = demoKupdobs[7].TheCd; break;
            case 'i':
               theList[i].Naziv = demoKupdobs[8].TheName;
               theList[i].Ticker = demoKupdobs[8].TheCd; break;
            case 'j':
               theList[i].Naziv = demoKupdobs[9].TheName;
               theList[i].Ticker = demoKupdobs[9].TheCd; break;
            case 'k':
               theList[i].Naziv = demoKupdobs[10].TheName;
               theList[i].Ticker = demoKupdobs[10].TheCd; break;
            case 'l':
               theList[i].Naziv = demoKupdobs[11].TheName;
               theList[i].Ticker = demoKupdobs[11].TheCd; break;
            case 'm':
               theList[i].Naziv = demoKupdobs[12].TheName;
               theList[i].Ticker = demoKupdobs[12].TheCd; break;
            case 'n':
               theList[i].Naziv = demoKupdobs[13].TheName;
               theList[i].Ticker = demoKupdobs[13].TheCd; break;
            case 'o':
               theList[i].Naziv = demoKupdobs[14].TheName;
               theList[i].Ticker = demoKupdobs[14].TheCd; break;
            case 'p':
               theList[i].Naziv = demoKupdobs[15].TheName;
               theList[i].Ticker = demoKupdobs[15].TheCd; break;
            case 'r':
               theList[i].Naziv = demoKupdobs[16].TheName;
               theList[i].Ticker = demoKupdobs[16].TheCd; break;
            case 's':
               theList[i].Naziv = demoKupdobs[17].TheName;
               theList[i].Ticker = demoKupdobs[17].TheCd; break;
            case 't':
               theList[i].Naziv = demoKupdobs[18].TheName;
               theList[i].Ticker = demoKupdobs[18].TheCd; break;
            case 'u':
               theList[i].Naziv = demoKupdobs[19].TheName;
               theList[i].Ticker = demoKupdobs[19].TheCd; break;
            case 'v':
               theList[i].Naziv = demoKupdobs[20].TheName;
               theList[i].Ticker = demoKupdobs[20].TheCd; break;
            case 'z':
               theList[i].Naziv = demoKupdobs[21].TheName;
               theList[i].Ticker = demoKupdobs[21].TheCd; break;
            default:
               theList[i].Naziv = demoKupdobs[22].TheName;
               theList[i].Ticker = demoKupdobs[22].TheCd; break;
         }
      }
   }

#endif
#endregion WEB_DEMO_DATA

   private string GetSkladCD_AsKupdobCD_ForInternDocuments(Faktur faktur_rec)
   {
      string skladCD_AsKupdobCD;

      //Faktur.TT_MSU, 
      //Faktur.TT_MSI, 
      //Faktur.TT_PIZ, 
      //Faktur.TT_IMT, 
      //Faktur.TT_PPR, 
      //Faktur.TT_POV, 
      //Faktur.TT_PUL: 
      if(faktur_rec.TtInfo.IsInternUI)
      {
         if(faktur_rec.TtInfo.IsInternIzlaz)
         {
            skladCD_AsKupdobCD = faktur_rec.SkladCD2.NotEmpty() ? faktur_rec.SkladCD2 : faktur_rec.SkladCD; // npr 'PPR' ili 'POV' nemaju SkladCD2 
         }
         else // Interni Ulaz 
         {
            skladCD_AsKupdobCD = faktur_rec.SkladCD;
         }
      }
      else //Faktur.TT_PST, ... 
      {
         skladCD_AsKupdobCD = faktur_rec.SkladCD;
      }

      return skladCD_AsKupdobCD;
   }

   protected void GroupJoinFakturWithRtrans()
   {
      var rtransesByFakturGroups = TheFakturList.GroupJoin(TheRtransList, F => (F.DokDate.Year * 1000000) + F.RecID, R => (R.T_skladDate.Year * 1000000) + R.T_parentID, (F, R) => new { faktur = F, rtranses = R });

      Parallel.ForEach(rtransesByFakturGroups, fakGroup =>
      {
#region Inner foreach rtrans ...

         //foreach(Rtrans rtrans in fakGroup.rtranses)
         //{
         // //if(rtrans.TheAsEx.ArtiklTS == "USL")
         //   if(rtrans.TheAsEx.IsRuc4Usluga)
         //   {
         //      fakGroup.faktur.Ira_USL_KC    += rtrans.R_KC;
         //      fakGroup.faktur.Ira_USL_PV    += rtrans.R_Ira_PV;
         //      fakGroup.faktur.Ira_USL_Rbt12 += rtrans.R_rbtAll;
         //   }
         //   else
         //   {
         //      fakGroup.faktur.Ira_ROB_KC    += rtrans.R_KC;
         //      fakGroup.faktur.Ira_ROB_PV    += rtrans.R_Ira_PV;
         //      fakGroup.faktur.Ira_ROB_Rbt12 += rtrans.R_rbtAll;
         //      fakGroup.faktur.Ira_ROB_NV    += rtrans.R_Ira_NV;
         //   }
         //}

#endregion Inner foreach rtrans ...

         fakGroup.faktur.Transes = fakGroup.rtranses.ToList();
      }
      /* remarck this zagrada line for non parallel*/ );
   }

   protected void AggregateFaktursIraRucProperties(bool isRucPoStavkama)
   {
      // 02.03.2015:
    //var rtransesByFakturGroups = TheFakturList.GroupJoin(TheRtransList, F =>                              F.RecID, R =>                                  R.T_parentID, (F, R) => new { faktur = F, rtranses = R });
      var rtransesByFakturGroups = TheFakturList.GroupJoin(TheRtransList, F => (F.DokDate.Year * 1000000) + F.RecID, R => (R.T_skladDate.Year * 1000000) + R.T_parentID, (F, R) => new { faktur = F, rtranses = R });

      //foreach(var fakGroup in rtransesByFakturGroups)
      // remarck next line and unremarck previous for non parallel*/ 
      Parallel.ForEach(rtransesByFakturGroups, fakGroup =>
      {
#region Inner foreach rtrans ...

         foreach(Rtrans rtrans in fakGroup.rtranses)
         {
          //if(rtrans.TheAsEx.ArtiklTS == "USL")
            if(rtrans.TheAsEx.IsRuc4Usluga)
            {
               fakGroup.faktur.Ira_USL_KC    += rtrans.R_KC;

               // 25.01.2016: 
               if(isRucPoStavkama) fakGroup.faktur.Ira_USL_PV    += rtrans.R_Ira_PVunr; // u R_Ira_PVunr ne ulazi UNR artikli 
               else                fakGroup.faktur.Ira_USL_PV    += rtrans.R_Ira_PV   ; // classic, bilo koja usluga 

               // 25.01.2016: NE zbrajaj rabat na UNR 
               if(isRucPoStavkama == false)                fakGroup.faktur.Ira_USL_Rbt12 += rtrans.R_rbtAll; // ako nije rucPoStavkama, uvijek zbrajaj rabat na uslugu            
               else if(rtrans.TheAsEx.IsUslNoRuc == false) fakGroup.faktur.Ira_USL_Rbt12 += rtrans.R_rbtAll; // ako je rucPoStavkama zbrajaj samo klasicne usluge, a unr preskaci 
            }
            else
            {
               fakGroup.faktur.Ira_ROB_KC    += rtrans.R_KC;
               fakGroup.faktur.Ira_ROB_PV    += rtrans.R_Ira_PV;
               fakGroup.faktur.Ira_ROB_Rbt12 += rtrans.R_rbtAll;
               fakGroup.faktur.Ira_ROB_NV    += rtrans.R_Ira_NV;
            }
         }

#endregion Inner foreach rtrans ...

         // 31.01.2012: za RekapFaktur: 
         fakGroup.faktur.Transes = fakGroup.rtranses.ToList();
      }
      /* remarck this zagrada line for non parallel*/ );
   }

#endregion GetSpecificList

#region CrossData

   protected CrossDataForMalop GetCrossData(List<Faktur> fakturList)
   {
      this.CrossData = new CrossDataForMalop();

      if(fakturList.Count.IsZero()) { TheDeviznaSumaList = new List<VvReportSourceUtil>(); return CrossData; }

      if(fakturList.Select(fak => fak.TT).Distinct().Count() == 1)
      {
         CrossData.TtInfo = fakturList[0].TtInfo;
      }
      else
      {
         CrossData.TtInfo = new TtInfo("ManyTT", 0, false, false, false, ZXC.VvSubModulEnum.FORBIDDEN);
      }

      CrossData.KPM_zaduzenje      = fakturList./*Where(fak => fak.TtInfo.IsMalopFin_U).*/Sum(fak => fak.KPM_zaduzenje     );
      CrossData.KPM_razduzenje_rob = fakturList./*Where(fak => fak.TtInfo.IsMalopFin_U).*/Sum(fak => fak.KPM_razduzenje_rob);
      CrossData.KPM_razduzenje_usl = fakturList./*Where(fak => fak.TtInfo.IsMalopFin_U).*/Sum(fak => fak.KPM_razduzenje_usl);

#region Malop ULAZ

      CrossData.S_ukKCRP_U      = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.S_ukKCRP     );
      CrossData.S_ukPdv_U       = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.S_ukPdv      );
      CrossData.S_ukMSK_25_U    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.S_ukMSK_25   );
      CrossData.S_ukMSK_05_U    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.S_ukMSK_05   );
      CrossData.S_ukMSK_23_U    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.S_ukMSK_23   );
      CrossData.S_ukMSK_10_U    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.S_ukMSK_10   );
      CrossData.S_ukMSK_00_U    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.S_ukMSK_00   );
      CrossData.S_ukMskPdv_25_U = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.S_ukMskPdv25 );
      CrossData.S_ukMskPdv_05_U = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.S_ukMskPdv05 );
      CrossData.S_ukMskPdv_23_U = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.S_ukMskPdv23 );
      CrossData.S_ukMskPdv_10_U = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.S_ukMskPdv10 );
      CrossData.S_ukMrz_U       = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.S_ukMrz      );
      CrossData.R_ukKCRP_rob_U  = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.R_ukKCRP_rob );
      CrossData.S_ukKCRP_usl_U  = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.S_ukKCRP_usl );
      CrossData.R_ukMSK_U       = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.R_ukMSK      );
      CrossData.R_ukMskPdv_U    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.R_ukMskPdv   );
      CrossData.S_ukMskPNP_U    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.S_ukMskPNP   );
      CrossData.R_NivVrj00_U    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.R_NivVrj00   );
      CrossData.R_NivVrj10_U    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.R_NivVrj10   );
      CrossData.R_NivVrj23_U    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.R_NivVrj23   );
      CrossData.R_NivVrj25_U    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.R_NivVrj25   );
      CrossData.R_NivVrj05_U    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.R_NivVrj05   );
      CrossData.R_NivVrj_U      = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.R_NivVrj     );
      CrossData.R_NivMskPdv10_U = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.R_NivMskPdv10);
      CrossData.R_NivMskPdv23_U = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.R_NivMskPdv23);
      CrossData.R_NivMskPdv25_U = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.R_NivMskPdv25);
      CrossData.R_NivMskPdv05_U = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.R_NivMskPdv05);
      CrossData.R_NivMskPdv_U   = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.R_NivMskPdv  );
      CrossData.R_NivMskPNP_U   = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.R_NivMskPNP  );
      CrossData.R_NivMrz_U      = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Sum(fak => fak.R_NivMrz     );
      CrossData.R_dokCount_U    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_UorVMIorTRI).Count();

#endregion Malop ULAZ
      
#region Malop IZLAZ

      CrossData.R_ukKCRP_cash_I = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_ukKCRP_cash);
      CrossData.R_ukKCRP_ziro_I = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_ukKCRP_ziro);
      CrossData.S_ukPdv_I       = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukPdv      );
      CrossData.S_ukPNP_I       = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukIznPNP   );
      CrossData.R_ukKCR_rob_I   = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_ukKCR_rob  );
      CrossData.S_ukKCR_usl_I   = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukKCR_usl  );
      CrossData.S_ukMSK_25_I    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukMSK_25   );
      CrossData.S_ukMSK_05_I    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukMSK_05   );
      CrossData.S_ukMSK_23_I    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukMSK_23   );
      CrossData.S_ukMSK_10_I    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukMSK_10   );
      CrossData.S_ukMSK_00_I    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukMSK_00   );
      CrossData.S_ukMskPdv_25_I = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukMskPdv25 );
      CrossData.S_ukMskPdv_05_I = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukMskPdv05 );
      CrossData.S_ukMskPdv_23_I = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukMskPdv23 );
      CrossData.S_ukMskPdv_10_I = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukMskPdv10 );
      CrossData.R_ukMskMrz_I    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_ukMskMrz   );
      CrossData.Ira_ROB_NV_I    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_Ira_NV     );
      CrossData.R_ukKCRP_rob_I  = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_ukKCRP_rob );
      CrossData.S_ukKCRP_usl_I  = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukKCRP_usl );
      CrossData.R_IrmRobRbt_I   = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_IrmRobRbt  );
      CrossData.R_ukMSK_I       = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_ukMSK      );
      CrossData.R_ukMskPdv_I    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_ukMskPdv   );
      CrossData.S_ukMskPNP_I    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukMskPNP   );
      CrossData.R_NivVrj00_I    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_NivVrj00   );
      CrossData.R_NivVrj10_I    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_NivVrj10   );
      CrossData.R_NivVrj23_I    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_NivVrj23   );
      CrossData.R_NivVrj25_I    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_NivVrj25   );
      CrossData.R_NivVrj05_I    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_NivVrj05   );
      CrossData.R_NivVrj_I      = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_NivVrj     );
      CrossData.R_NivMskPdv10_I = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_NivMskPdv10);
      CrossData.R_NivMskPdv23_I = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_NivMskPdv23);
      CrossData.R_NivMskPdv25_I = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_NivMskPdv25);
      CrossData.R_NivMskPdv05_I = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_NivMskPdv05);
      CrossData.R_NivMskPdv_I   = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_NivMskPdv  );
      CrossData.R_NivMskPNP_I   = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_NivMskPNP  );
      CrossData.R_NivMrz_I      = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_NivMrz     );
      CrossData.R_dokCount_I    = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Count();
      CrossData.S_ukPdv05m      = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukPdv05m   );
      CrossData.S_ukPdv10m      = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukPdv10m   );
      CrossData.S_ukPdv25m      = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.S_ukPdv25m   );
      CrossData.X_ukPpmvIzn     = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.X_ukPpmvIzn  );

      // 08.06.2022: 
      CrossData.R_ukKCRP_storno = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I).Sum(fak => fak.R_ukKCRP_storno);


      //var nacPlacGRP = fakturList.GroupBy(fak => fak.NacPlac).Select(grp => new
      //{
      //   nacPlac = grp.Key,
      //   isCash  = ZXC.luiListaRiskVrstaPl.GetFlagForThisCd(grp.Key),
      //   ukKCRP  = grp.Sum(g => g.S_ukKCRP)
      //});

      // 30.01.2012: Radi i ovo gore, ali ovo dole mi se cini vise slick! 

      TheDeviznaSumaList
  /*CrossData.NacPlacInfo*/ = fakturList.Where(fak => fak.TtInfo.IsMalopFin_I)
                                        .GroupBy(fak => fak.NacPlac)
                                        .Select(grp => new VvReportSourceUtil(/* DevName   */ grp.Key, 
                                                                              /* TheMoney  */ grp.Sum(g => g.S_ukKCRP), 
                                                                              /* IsNekakav */ ZXC.luiListaRiskVrstaPl.GetFlagForThisCd(grp.Key)))
                                        .OrderByDescending(suma => suma.IsNekakav)
                                        .ThenByDescending(suma => suma.TheMoney)
                                        .ToList();

    // ova dva ne trebaju. Samo su kao kontrola, jer imas to vec u 'fak.R_ukKCRP_cash' i 'fak.R_ukKCRP_ziro' 
    //decimal ukKCRP_cash = CrossData.NacPlacInfo.Where(np => np.IsNekakav/*isCash*/ == true ).Sum(np => np.TheMoney/*ukKCRP*/);
    //decimal ukKCRP_ziro = CrossData.NacPlacInfo.Where(np => np.IsNekakav/*isCash*/ == false).Sum(np => np.TheMoney/*ukKCRP*/);

#endregion Malop IZLAZ

      return CrossData;
   }

#endregion CrossData

#region Group handling For RakapFaktur, Ruc, PrometArtikla

   protected string GetArtGrCD(string artiklGRtype, string artiklCD)
   {
      if(artiklGRtype.IsEmpty()) return "";

      Artikl artikl_rec = TheArtiklList.SingleOrDefault(a => a.ArtiklCD == artiklCD);

      if(artikl_rec == null) return " NEPOSTOJEĆI Artikl";

      switch(artiklGRtype)
      {
         case "TS"      : return artikl_rec.TS      .NotEmpty() ? artikl_rec.TS       : " NEDEFINIRANI Tip Artikla"   ;
         case "Grupa1CD": return artikl_rec.Grupa1CD.NotEmpty() ? artikl_rec.Grupa1CD : " NEDEFINIRANA Grupa1 Artikla";
         case "Grupa2CD": return artikl_rec.Grupa2CD.NotEmpty() ? artikl_rec.Grupa2CD : " NEDEFINIRANA Grupa2 Artikla";
         case "Grupa3CD": return artikl_rec.Grupa3CD.NotEmpty() ? artikl_rec.Grupa3CD : " NEDEFINIRANA Grupa3 Artikla";
         
         default: return "";
      }
   }

   protected string GetArtGrName(string artiklGRtype, string artiklCD)
   {
      string artGrCD = GetArtGrCD(artiklGRtype, artiklCD);

      if(artGrCD.IsEmpty()) return "";
       
      string grName;

      switch(artiklGRtype)
      {
         case "TS"      : grName = ZXC.luiListaArtiklTS     .GetNameForThisCd(artGrCD); return grName/*.NotEmpty() ? grName : "NEDEFINIRANI Tip Artikla"  */;
         case "Grupa1CD": grName = ZXC.luiListaGrupa1Artikla.GetNameForThisCd(artGrCD); return grName/*.NotEmpty() ? grName : "NEDEFINIRANA Grupa Artikla"*/;
         case "Grupa2CD": grName = ZXC.luiListaGrupa2Artikla.GetNameForThisCd(artGrCD); return grName/*.NotEmpty() ? grName : "NEDEFINIRANA Grupa Artikla"*/;
         case "Grupa3CD": grName = ZXC.luiListaGrupa3Artikla.GetNameForThisCd(artGrCD); return grName/*.NotEmpty() ? grName : "NEDEFINIRANA Grupa Artikla"*/;
         
         default: return "";
      }
   }

   public static string GetGrDataForDate(string fakturGRtype, DateTime dokDate, DateTime addTS)
   {
      return GetGrDataForDate(fakturGRtype, dokDate.Year, dokDate.Month, dokDate.Day, dokDate.GetWeekOfYear(), addTS);
   }

   protected static string GetGrDataForDate(string fakturGRtype, int year, int month, int day, int week, DateTime addTS)
   {
      string grData;

      switch(fakturGRtype)
      {
         case "DokYear"   : grData = year.ToString();                                                                         return grData.NotEmpty() ? grData : " NEDEFINIRANA Godina";
         case "DokMonth"  : grData = year.ToString() + "-" + month.ToString("00");                                            return grData.NotEmpty() ? grData : " NEDEFINIRANI Mjesec";
         case "DokWeek"   : grData = week.ToString("00");                                                                     return grData.NotEmpty() ? grData : " NEDEFINIRANI Tjedan";
         case "DokDay"    : grData = GetGrDateYyyyMmDd_ddMMyyyy(year, month, day);                                            return grData.NotEmpty() ? grData : " NEDEFINIRANI Dan"   ;
         case "AddTS"     : grData = GetGrDateYyyyMmDd_ddMMyyyy(year, month, day) + " - " + addTS.Hour.ToString("00") + "h)"; return grData.NotEmpty() ? grData : " NEDEFINIRANI Sat"   ;
      }

      return "";
   }

   private static string GetGrDateYyyyMmDd_ddMMyyyy(int year, int month, int day)
   {
      return year.ToString("0000") + month.ToString("00") + day.ToString("00") + " (" + day.ToString("00")  + "." + month.ToString("00") + "." + year.ToString("0000") + ")";
   }

   protected string GetFakGRdata(string fakturGRtype, Faktur faktur_rec)
   {
      return GetFakGRdata(fakturGRtype, faktur_rec, false);
   }

   protected string GetFakGRdata(string fakturGRtype, Faktur faktur_rec, bool isCompareChrono)
   {
      string grData;

      switch(fakturGRtype)
      {
         case ""          : return ""                                                                                                       ;
         case "AddUID"    : grData = faktur_rec.AddUID                                  ; return grData.NotEmpty() ? grData : " NEDEFINIRANI User"     ;
         case "DevName"   : grData = faktur_rec.DevName                                 ; return grData.NotEmpty() ? grData : " NEDEFINIRANA Deviza"   ;
         case "PosJedName": grData = PoslJedName(faktur_rec.PosJedCD.NotZero() ? faktur_rec.PosJedCD : faktur_rec.KupdobCD); return grData.NotEmpty() ? grData : " NEDEFINIRANA PoslJed";
         case "ProjektCD" : grData = faktur_rec.ProjektCD                               ; return grData.NotEmpty() ? grData : " NEDEFINIRANI Projekt"  ;
         case "Putnik"    : grData = faktur_rec.PersonName                              ; return grData.NotEmpty() ? grData : " NEDEFINIRANI Komerc."   ;
         case "NacPlac"   : grData = faktur_rec.NacPlac                                 ; return grData.NotEmpty() ? grData : " NEDEFINIRANI NacPlac"  ;

         case "KupdobName": if(RptFilter.FuseBool3) // IsPoslJed instead of Kupdob 
                            {
                               grData = PoslJedName(faktur_rec.PosJedCD.NotZero() ? faktur_rec.PosJedCD : faktur_rec.KupdobCD);
                            }
                            else
                            {
                               grData = KupdobOrMtrosName(faktur_rec.KupdobCD);
                            }
                                                                                          return grData.NotEmpty() ? grData : " NEDEFINIRANI Partner"  ;

         case "MtrosName" : grData = KupdobOrMtrosName(faktur_rec.MtrosCD)              ; return grData.NotEmpty() ? grData : " NEDEFINIRANO MjTros"   ;
         case "TT"        : grData = faktur_rec.TT                                      ; return grData.NotEmpty() ? grData : " NEDEFINIRANI TT"       ;

         case "DokYear"   :
         case "DokMonth"  : 
         case "DokWeek"   : 
         case "DokDay"    : 
         case "AddTS"     : if(isCompareChrono) return faktur_rec.VezniDok;
                            else                return GetGrDataForDate(fakturGRtype, faktur_rec.DokDate, faktur_rec.AddTS);

         case "DayOfWeek" : grData = ((int)faktur_rec.DokDate.DayOfWeek).ToString() + "-" + 
                                     faktur_rec.DokDate.ToString("dddd")                       ; return grData.NotEmpty() ? grData : " NEDEFINIRANI DanTjedna";
         case "SkladCD"   : grData = faktur_rec.SkladCD + " " +  
                                     ZXC.luiListaSkladista.GetNameForThisCd(faktur_rec.SkladCD); return grData.NotEmpty() ? grData : " NEDEFINIRANO Skladiste";
         case "TH_CycleM" : grData = faktur_rec.TH_CycleMoment                                 ; return grData.NotEmpty() ? grData : " NEDEFINIRANI TH_CycleM";
         case "HourOfDay" : grData = faktur_rec.AddTS.Hour.ToString("00") + "h"                ; return grData.NotEmpty() ? grData : " NEDEFINIRANI SatDana"  ;

         default: ZXC.aim_emsg(MessageBoxIcon.Error, "FakturGRtype [{0}] nedefiniran u GetFakGRdata()!", fakturGRtype); return "";
      }
   }

   protected string GetFakGRdata(string fakturGRtype, Rtrans rtrans_rec)
   {
      string grData;

      switch(fakturGRtype)
      {
         case ""          : return ""                                                     ;
         case "KupdobName": grData = KupdobOrMtrosName(rtrans_rec.T_kupdobCD)             ; return grData.NotEmpty() ? grData : " NEDEFINIRANI Partner"  ;
         case "SVDklinika": grData = SVDklinikaName   (rtrans_rec.T_kupdobCD)             ; return grData.NotEmpty() ? grData : " NEDEFINIRANI Partner"  ;
         case "MtrosName" : grData = KupdobOrMtrosName(rtrans_rec.T_mtrosCD)              ; return grData.NotEmpty() ? grData : " NEDEFINIRANO MjTros"   ;
         case "TT"        : grData = rtrans_rec.T_TT                                      ; return grData.NotEmpty() ? grData : " NEDEFINIRANI TT"       ;

         case "DokYear"   :
         case "DokMonth"  : 
         case "DokWeek"   : 
         case "DokDay"    : return GetGrDataForDate(fakturGRtype, rtrans_rec.T_skladDate, /* Dummy: */ DateTime.MinValue/*, TimeSpan.MinValue*/);
                            
         case "DayOfWeek" : grData = ((int)rtrans_rec.T_skladDate.DayOfWeek).ToString() + "-" + 
                                     rtrans_rec.T_skladDate.ToString("dddd")                   ; return grData.NotEmpty() ? grData : " NEDEFINIRANI DanTjedna";
         case "SkladCD"   : grData = rtrans_rec.T_skladCD + " " +
                                  ZXC.luiListaSkladista.GetNameForThisCd(rtrans_rec.T_skladCD) ; return grData.NotEmpty() ? grData : " NEDEFINIRANO Skladiste";
         case "TH_CycleM" : grData = rtrans_rec.TH_CycleMoment                                 ; return grData.NotEmpty() ? grData : " NEDEFINIRANI TH_CycleM";

         default: ZXC.aim_emsg(MessageBoxIcon.Error, "FakturGRtype [{0}] nedefiniran u GetFakGRdata()!", fakturGRtype); return "";
      }
   }

   private string KupdobOrMtrosName(uint theCD)
   {
      Kupdob kupdob_rec = TheKupdobList.SingleOrDefault(k => k.KupdobCD == theCD);

      string kcdAddition = " - " + theCD.ToString("000000");

      // 18.04.2019: 
      if(this is RptR_Rekap_TH_DjelatRabat) kcdAddition = "";

      if(kupdob_rec != null) return kupdob_rec.Naziv + kcdAddition;
      else                   return "";
   }

   private string SVDklinikaName(uint theCD)
   {
      Kupdob kupdob_rec = TheKupdobList.SingleOrDefault(k => k.KupdobCD == theCD);

      if(kupdob_rec != null) return kupdob_rec.Ulica2;
      else                   return "";
   }

   private string PoslJedName(uint theCD)
   {
      Kupdob kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(k => k.KupdobCD == theCD);

      if(kupdob_rec != null) return kupdob_rec.Naziv;
      else                   return "";
   }

#endregion Group handling For RakapFaktur, Ruc, PrometArtikla

#region WEB_DEMO_DATA

#if(WWWDEMO)

   protected static ZXC.CdAndName_CommonStruct[] demoKupdobs = 
   { 
      new ZXC.CdAndName_CommonStruct("DUCATI" , "Ducati Motor Holding S.p.A."),
      new ZXC.CdAndName_CommonStruct("PIRELI" , "Pirelli & C. S.p.A. - Pirelli Tyre S.p.A."),
      new ZXC.CdAndName_CommonStruct("DIEHRD" , "Nakatomi Trading Corp."),
      new ZXC.CdAndName_CommonStruct("MALAKL" , "Mala Klinika s.p.o."),
      new ZXC.CdAndName_CommonStruct("CLKENT" , "Daily Planet"),
      new ZXC.CdAndName_CommonStruct("SYRIAN" , "Connex-Killen"),
      new ZXC.CdAndName_CommonStruct("FRASER" , "KACL 780 AM"),
      new ZXC.CdAndName_CommonStruct("MURPHY" , "FYI Television"),
      new ZXC.CdAndName_CommonStruct("SCHWAR" , "SkyNet Corporation"),
      new ZXC.CdAndName_CommonStruct("GEKKO"  , "Greed is good, greed is right"),
      new ZXC.CdAndName_CommonStruct("MICROS" , "Northwind Traders"),
      new ZXC.CdAndName_CommonStruct("MADMAN" , "Sterling Cooper Draper Pryce"),
      new ZXC.CdAndName_CommonStruct("RIPLEY" , "Weyland-Yutani Corp."),
      new ZXC.CdAndName_CommonStruct("SUPERM" , "LexCorp"),
      new ZXC.CdAndName_CommonStruct("ARNOLD" , "Cyberdyne Systems Corp."),
      new ZXC.CdAndName_CommonStruct("MARVEL" , "Stark Industries"),
      new ZXC.CdAndName_CommonStruct("DALLAS" , "Ewing Oil"),
      new ZXC.CdAndName_CommonStruct("LOONEY" , "ACME Corporation"),
      new ZXC.CdAndName_CommonStruct("MONTYP" , "Very Big Corp. of America"),
      new ZXC.CdAndName_CommonStruct("BLADER" , "Tyrell Corporation"),
      new ZXC.CdAndName_CommonStruct("BATMAN" , "Wayne Enterprises"),
      new ZXC.CdAndName_CommonStruct("HALLAB" , "H.A.L. Labs 2001"),
      new ZXC.CdAndName_CommonStruct("FRIEND" , "Central Perk Friends"),
   };

#endif

#endregion WEB_DEMO_DATA

}

#endregion RptR_StandardRiskReport (The Father)

#region Kretanje SKLAD, StanjeSKL po PRJ, Stanje REVERSa, RptR_TemboWebShopExport, RptR_Jeftinije_hr_Export, RptR_Artikl2BCterminal

public class RptR_KretanjeSklad            : RptR_StandardRiskReport
{
#region Constructor

   public RptR_KretanjeSklad(string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle)

      : base(new Vektor.Reports.RIZ.CR_KretanjeSklad() as ReportDocument, _reportName, _rptFilter, _filterStyle,

         false, // ArtiklWithArtstat 
         false, // ArtStat        
         false, // Faktur         
         false, // Rtrans         
         false, // Kupdob         
         false, // Prjkt          
         false, // Rtrans4ruc     
         false) // Artikl         

   {
      ReportNeeds_RSU_List = true;
   }

#endregion Constructor

   public override int FillRiskReportLists()
   {
      // TODO: mejbi uan dej: all_sklad we want - kao GetArtiklWithArtstatList() 

      string ulazClause ;
      string izlazClause;

      bool isMalopSkl = ZXC.luiListaSkladista.GetFlagForThisCd(RptFilter.SkladCD);

      if(isMalopSkl)
      {
         ulazClause  = ArtiklDao.ResCol_UlazFinMPC ;
         izlazClause = ArtiklDao.ResCol_IzlazFinMPC;
      }
      else
      {
         ulazClause  = ArtiklDao.ResCol_UlazFinNBC ;
         izlazClause = ArtiklDao.ResCol_IzlazFinNBC;
      }

      TheDeviznaSumaList = new List<VvReportSourceUtil>();
      List<ZXC.VvUtilDataPackage> utilList = new List<ZXC.VvUtilDataPackage>();

      ArtiklDao.GetKretanjeSkladList(TheDbConnection, utilList, RptFilter, ulazClause, izlazClause);

#region Fill missing days

      TimeSpan oneDayTS = new TimeSpan(1, 0, 0, 0);

      for(DateTime day = RptFilter.DatumOd; day <= RptFilter.DatumDo.Date; day += oneDayTS)
      {
         if(utilList.Select(util => util.TheDate).Contains(day) == false) // day is missing in utilList 
         {
            utilList.Add(new ZXC.VvUtilDataPackage(day, 0.00M, 0.00M));
         }
      }

#endregion Fill missing days

#region RunningTotal example, ROLLUP (isti kuratz) example

      //var DELETEME1 = TheFakturLightList.Rollup(0, (s, x) => (int)s.Decim01 + x);
      //var DELETEME2 = TheFakturLightList.Zip(DELETEME1, (a1, a2) => new
      //{
      //   Data = a1.Decim01,
      //   RunningTotal = a2,
      //});

      //// SOME OTHER WAY ================================================================================== 

      //List<int> list = new List<int>() { 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12, 13, 69, 2007 };
      //int running_total = 0;

      //var result_set = list.Select(item =>
      //   new
      //    {
      //       num = item,
      //       THErunning_total = (running_total = running_total + item)
      //    }
      //);

#endregion RunningTotal example, ROLLUP (isti kuratz) example

#region Create TheDeviznaSumaList (VvReportSourceUtil) from utilList (ZXC.VvUtilDataPackage)

      decimal saldo = 0;

      TheDeviznaSumaList = 
         
      utilList
         .OrderBy(util => util.TheDate)
         .Select (util => new VvReportSourceUtil(util.TheDate, util.TheDecimal, util.TheDecimal2, saldo = saldo + (util.TheDecimal - util.TheDecimal2), 0))
         .ToList();

#endregion Create TheDeviznaSumaList (VvReportSourceUtil) from utilList (ZXC.VvUtilDataPackage)

#region Cumulate ukIzlazniPromet, prosStanjeSkladista and finally TheKoef

      // Formula: koefObrtajaSkladista = sumIzlazniPromet / prosStanjeSkladista 

      decimal prosStanjeSkladista, sumIzlazniPromet = 0.00M, sumStanjeSklad = 0.00M;
      int     dayCount = 0;

      foreach(VvReportSourceUtil dsu in TheDeviznaSumaList)
      {
         sumIzlazniPromet += dsu.TheMoney2;
         sumStanjeSklad   += dsu.TheSaldo;

         prosStanjeSkladista = sumStanjeSklad / ++dayCount;

         dsu.TheKoef = ZXC.DivSafe(sumIzlazniPromet, prosStanjeSkladista);
      }

#endregion Cumulate ukIzlazniPromet, prosStanjeSkladista and finally TheKoef

      return TheDeviznaSumaList.Count;
   }

   public static bool IsFilterWellFormed(RiskReportUC reportUC)
   {
      bool OK = true;
      VvRpt_RiSk_Filter filter = reportUC.TheRptFilter;

      if(filter.SkladCD.IsEmpty())
      {
         ZXC.aim_emsg("Molim, zadajte SKLADIŠTE.");
         return false;
      }

      return (OK);
   }

}

public class RptR_StanjeSkladPoPRJ         : RptR_StandardRiskReport
{
   public RptR_StanjeSkladPoPRJ(string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle)

      : base(new Vektor.Reports.RIZ.CR_StanjeSklad_Prjkt() as ReportDocument, _reportName, _rptFilter, _filterStyle,

         false, // ArtiklWithArtstat 
         false, // ArtStat        
         false, // Faktur         
         false, // Rtrans         
         false, // Kupdob         
         false, // Prjkt          
         false, // Rtrans4ruc     
         false) // Artikl         

   {
      ReportNeeds_RSU_List = true;
   }

   public override int FillRiskReportLists()
   {
      // TODO: mejbi uan dej: all_sklad we want - kao GetArtiklWithArtstatList() 

      string ulazClause, izlazClause, ulazIzlazClause;

      ulazClause      = ArtiklDao.Rtr_Ulaz_Kol     ;
      izlazClause     = ArtiklDao.Rtr_Izlaz_Kol    ;
      ulazIzlazClause = ArtiklDao.Rtr_UlazIzlaz_Kol;

      TheDeviznaSumaList = new List<VvReportSourceUtil>();
      List<ZXC.VvUtilDataPackage> utilList = new List<ZXC.VvUtilDataPackage>();

      ArtiklDao.GetStanjeSklPoRNPList(TheDbConnection, utilList, RptFilter, ulazClause, izlazClause, ulazIzlazClause);

      TheDeviznaSumaList = utilList.Select(item =>
         new VvReportSourceUtil(item.TheStr1, item.TheStr2, item.TheDecimal, item.TheDecimal2)
      ).ToList();

      // TODO: !!! FilterMember koji ce govoriti ocu sve ili ocu samo one koji nisu na nuli 
      // do tada koristimo 'RptFilter.OcuGraf' 
      if(RptFilter.OcuGraf == true) // izbaci one koji su na nuli
      {
         TheDeviznaSumaList.RemoveAll(dSum => dSum.DiffMoney.IsZero());
      }

      if(VvUserControl.ArtiklSifrar == null) ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

      TheArtiklLightList = VvUserControl.ArtiklSifrar
         
         .Select(artikl => new ArtiklLight(artikl))
         .Join(TheDeviznaSumaList, art => art.ArtiklCD, dSum => dSum.TheCD, (a, x) => a)
         .Distinct().ToList();

      // Get RNP detailed data _____ START _________________________________
      Faktur prjFaktur_rec;
      string prjTt;
      uint   prjTtNum;
      bool   OK;
      foreach(VvReportSourceUtil the_rec in TheDeviznaSumaList)
      {
         prjFaktur_rec = new Faktur();
         Ftrans.ParseTipBr(the_rec.DevName, out prjTt, out prjTtNum);
         OK = FakturDao.SetMeFaktur(ZXC.TheMainDbConnection, prjFaktur_rec, prjTt, prjTtNum, true);
         if(OK) the_rec.DevName = prjFaktur_rec.TipBr + "-" + prjFaktur_rec.KupdobName + "-" + prjFaktur_rec.VezniDok + "-" + prjFaktur_rec.PrjArtName;
      }
      // Get RNP detailed data _____ END   _________________________________

      return TheDeviznaSumaList.Count;
   }

   public static bool IsFilterWellFormed(RiskReportUC reportUC)
   {
      bool OK = true;
      VvRpt_RiSk_Filter filter = reportUC.TheRptFilter;

      if(filter.SkladCD.IsEmpty())
      {
         ZXC.aim_emsg("Molim, zadajte SKLADIŠTE.");
         return false;
      }

      return (OK);
   }

}

public class RptR_StanjeReversa            : RptR_StandardRiskReport
{
   public RptR_StanjeReversa(string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle)

      : base(new Vektor.Reports.RIZ.CR_StanjeSklad_Revers() as ReportDocument, _reportName, _rptFilter, _filterStyle,

         false, // ArtiklWithArtstat 
         false, // ArtStat        
         false, // Faktur         
         false, // Rtrans         
         false, // Kupdob         
         false, // Prjkt          
         false, // Rtrans4ruc     
         false) // Artikl         
   {
      ReportNeeds_RSU_List = true;
   }

   public override int FillRiskReportLists()
   {
      // TODO: mejbi uan dej: all_sklad we want - kao GetArtiklWithArtstatList() 

      string ulazClause, izlazClause, ulazIzlazClause;

      ulazClause      = "('RVI')";
      izlazClause     = "('RVU')";
      ulazIzlazClause = "('RVI', 'RVU')";

      TheDeviznaSumaList = new List<VvReportSourceUtil>();
      List<ZXC.VvUtilDataPackage> utilList = new List<ZXC.VvUtilDataPackage>();

      ArtiklDao.GetStanjeReversaList(TheDbConnection, utilList, RptFilter, ulazClause, izlazClause, ulazIzlazClause);

      TheDeviznaSumaList = utilList.Select(item =>
         new VvReportSourceUtil(item.TheUint, item.TheStr2, item.TheDecimal, item.TheDecimal2)
      ).ToList();

      // TODO: !!! FilterMember koji ce govoriti ocu sve ili ocu samo one koji nisu na nuli 
      // do tada koristimo 'RptFilter.OcuGraf' 
      if(RptFilter.OcuGraf == true) // izbaci one koji su na nuli
      {
         TheDeviznaSumaList.RemoveAll(dSum => dSum.DiffMoney.IsZero());
      }

      if(VvUserControl.ArtiklSifrar == null) ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);
      if(VvUserControl.KupdobSifrar == null) ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      TheArtiklLightList = VvUserControl.ArtiklSifrar
         
         .Select(artikl => new ArtiklLight(artikl))
         .Join(TheDeviznaSumaList, art => art.ArtiklCD, dSum => dSum.TheCD, (a, x) => a)
         .Distinct().ToList();

      TheKupdobList = VvUserControl.KupdobSifrar.Join(TheDeviznaSumaList, kpdb => kpdb.KupdobCD, dSum => dSum.KupdobCD, (k, f) => k).Distinct().ToList();

      return TheDeviznaSumaList.Count;
   }

   public static bool IsFilterWellFormed(RiskReportUC reportUC)
   {
      bool OK = true;
      VvRpt_RiSk_Filter filter = reportUC.TheRptFilter;

      if(filter.SkladCD.IsEmpty())
      {
         ZXC.aim_emsg("Molim, zadajte SKLADIŠTE.");
         return false;
      }

      return (OK);
   }

}

public class RptR_TemboWebShopExport : RptR_StandardRiskReport
{
   public RptR_TemboWebShopExport(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle)

      : base(/*new Vektor.Reports.RIZ.CR_TemboWebShopExport() as ReportDocument*/ _reportDocument, _reportName, _rptFilter, _filterStyle,

         true , // ArtiklWithArtstat 
         false, // ArtStat        
         false, // Faktur         
         false, // Rtrans         
         false, // Kupdob         
         false, // Prjkt          
         false, // Rtrans4ruc     
         false) // Artikl         

   {
      IsForExport = true;
   }

   public override int FillRiskReportLists()
   {
      base.FillRiskReportLists();

      bool is_Jeftinije_hr = this.reportDocument is Vektor.Reports.RIZ.CR_Jeftinije_hr_Export; 
      bool is_TemboWebShop = !is_Jeftinije_hr;

#region SUM As Kumulativ Stanja na svim skladistima

      // List<Artikl> analiticArtiklList = TheArtiklList.ToList(); // ...a li ovo ti je primjer kako nabrzake klonirati listu .clone 

      var artiklGroups = is_TemboWebShop ? TheArtiklList.Where(art => art.Grupa2CD.NotEmpty()).GroupBy(art => art.ArtiklCD).ToList() :
                        /* jeftinije.hr */ TheArtiklList.Where(art => art.Grupa3CD.NotEmpty()).GroupBy(art => art.ArtiklCD).ToList() ; // ToList() da prekine shallow copy 

      TheArtiklList.Clear();

      Artikl sumiraniArtikl_rec;

      foreach(var artGR in artiklGroups)
      {
         sumiraniArtikl_rec         = artGR.Last()        .MakeDeepCopy();
         sumiraniArtikl_rec.TheAsEx = artGR.Last().TheAsEx.MakeDeepCopy();

         // Za sumiraniArtikl_rec.AS_StanjeKol: 
         sumiraniArtikl_rec.AS_UkPstKol   = artGR.Sum(art => art.AS_UkPstKol  );
         sumiraniArtikl_rec.AS_UkUlazKol  = artGR.Sum(art => art.AS_UkUlazKol );
         sumiraniArtikl_rec.AS_UkIzlazKol = artGR.Sum(art => art.AS_UkIzlazKol);

         // Za sumiraniArtikl_rec.AS_StanjeFinNBC: 
         sumiraniArtikl_rec.AS_UkPstFinNBC   = artGR.Sum(art => art.AS_UkPstFinNBC  );
         sumiraniArtikl_rec.AS_UkUlazFinNBC  = artGR.Sum(art => art.AS_UkUlazFinNBC );
         sumiraniArtikl_rec.AS_UkIzlazFinNBC = artGR.Sum(art => art.AS_UkIzlazFinNBC);

         TheArtiklList.Add(sumiraniArtikl_rec);
      }

#endregion SUM As Kumulativ Stanja na svim skladistima

      // NC |WebCa |WebCb |WebCc |WebCg - gost 
      //     +M     +1     +1     +3           
      //                                       
      // NC: nab cij po litri bez pdv          
      // M : marza po litri bez pdv            
      //                                       
      // WebCa = NC    + M                     
      // WebCb = WebCa + 1                     
      // WebCc = WebCb + 1                     
      //                                       
      // WebCg = WebCc + 3                     

      decimal NC_OP, M_OP, WebCa_OP, WebCb_OP, WebCc_OP, WebCg_OP;
      decimal NC   ,       WebCa   , WebCb   , WebCc   , WebCg   ;

      bool pakManjeOd1;

      foreach(Artikl artikl_rec in TheArtiklList)
      {
         pakManjeOd1 = artikl_rec.Zapremina < 1.00M; // 29.10.2019: tu malo lutamo treba li provjeravati 'Zapremina' ili 'AS_OrgPak' 

         NC    = artikl_rec.AS_PrNabCij  ;
         NC_OP = artikl_rec.AS_PrNabCijOP;
         M_OP  = ZXC.luiListaGrupa2Artikla.GetNumberForThisCd(artikl_rec.Grupa2CD);

         if(pakManjeOd1 == false) // normal case 
         {
            WebCa_OP = NC_OP    + M_OP;
            WebCb_OP = WebCa_OP + 1M  ;
            WebCc_OP = WebCb_OP + 1M  ;
            WebCg_OP = WebCc_OP + 3M  ;

            WebCa    = WebCa_OP * artikl_rec.AS_OrgPak;
            WebCb    = WebCb_OP * artikl_rec.AS_OrgPak;
            WebCc    = WebCc_OP * artikl_rec.AS_OrgPak;
            WebCg    = WebCg_OP * artikl_rec.AS_OrgPak;
         }
         else // pakiranje JE manje od 1 litre 
         { 
            WebCa    = NC    + M_OP;
            WebCb    = WebCa + 1M  ;
            WebCc    = WebCb + 1M  ;
            WebCg    = WebCc + 3M  ;

            WebCa_OP = ZXC .DivSafe(WebCa, artikl_rec.AS_OrgPak);
            WebCb_OP = ZXC .DivSafe(WebCb, artikl_rec.AS_OrgPak);
            WebCc_OP = ZXC .DivSafe(WebCc, artikl_rec.AS_OrgPak);
            WebCg_OP = ZXC .DivSafe(WebCg, artikl_rec.AS_OrgPak);
         }

         artikl_rec.AS_PreDefVpc1    = WebCa_OP;
         artikl_rec.AS_PreDefVpc2    = WebCb_OP;
         artikl_rec.AS_PreDefMpc1    = WebCc_OP;
         artikl_rec.AS_PreDefDevc    = WebCg_OP;

         artikl_rec.AS_InvKolDiff    = WebCa   ;
         artikl_rec.AS_InvKol2Diff   = WebCb   ;
         artikl_rec.AS_InvFinDiffNBC = WebCc   ;
         artikl_rec.AS_InvFinDiffMPC = WebCg   ;
      }

      return 0;
   }

   public override string ExportFileName
   {
      get
      {
         return "VvTEMBO 4WEB" + " @ " + DateTime.Now.ToString(ZXC.VvTimeStampFormat4FileName) + "." + "CSV";
      }
   }

   public override bool ExecuteExport(string fileName)
   {
      using(StreamWriter sw = new StreamWriter(fileName, false, Encoding.GetEncoding(1250)))
      {
         string dumpLine = "\"Product SKU\";\"Price Shopper Group\";\"Cost Price\";\"Stock\"";

         sw.WriteLine(dumpLine);

         foreach(Artikl artikl_rec in TheArtiklList)
         {
            dumpLine = BuildStringForExportLine(artikl_rec);

            sw.WriteLine(dumpLine);
         }
      }

      ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Gotovo.\n\nKreirao datoteku\n\n" + fileName);

      return true;
   }

   private string BuildStringForExportLine(Artikl artikl_rec)
   {
      return
         "\"" + artikl_rec.ArtiklCD                        + "\";" +
         "\"A|B|C|GOST|MPC"                                + "\";" + "\"" +
         artikl_rec.AS_InvKolDiff   .ToStringVv_NoGroup () + "|"   +
         artikl_rec.AS_InvKol2Diff  .ToStringVv_NoGroup () + "|"   +
         artikl_rec.AS_InvFinDiffNBC.ToStringVv_NoGroup () + "|"   +
         artikl_rec.AS_InvFinDiffMPC.ToStringVv_NoGroup () + "|"   +
         artikl_rec.AS_InvFinDiffMPC.ToStringVv_NoGroup () + "\";" + "\"" +
         artikl_rec.AS_StanjeKol    .ToString0Vv_NoGroup() + "\""  ;
   }
}

public class RptR_Jeftinije_hr_Export : RptR_TemboWebShopExport
{
   public RptR_Jeftinije_hr_Export(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle)

      : base(/*new Vektor.Reports.RIZ.CR_TemboWebShopExport() as ReportDocument*/ _reportDocument, _reportName, _rptFilter, _filterStyle)

   {
      IsForExport = true;
   }

   public override int FillRiskReportLists()
   {
      int retVal = base.FillRiskReportLists();

      foreach(Artikl artikl in TheArtiklList)
      {
#region webCij_wPdv

         decimal webCij_wPdv, thePdvSt;

         if(artikl.PdvKat.NotEmpty())
         {
            VvLookUpItem lui = ZXC.luiListaPdvKat.GetLuiForThisCd(artikl.PdvKat);
            thePdvSt = lui.Number;
         }
         else 
         {
            thePdvSt = Faktur.CommonPdvStForThisDate(DateTime.Today);
         }

         webCij_wPdv = ZXC.VvGet_125_on_100(artikl.AS_InvFinDiffMPC, thePdvSt);

         artikl.AS_InvFinDiffMPC = webCij_wPdv;

#endregion webCij_wPdv

      }

      return retVal;
   }

   public override string ExportFileName
   {
      get
      {
         return ZXC.CURR_prjkt_rec.Ticker + " za jeftinije_hr" + " @ " + DateTime.Now.ToString(ZXC.VvTimeStampFormat4FileName) + "." + "xml";
      }
   }

   public override bool ExecuteExport(string fileName)
   {
#region GORE

      Jeftinije_hr.CNJExport theCNJExport = new Jeftinije_hr.CNJExport();

      Jeftinije_hr.CNJExportItem itemLine;

      theCNJExport.Items = new Jeftinije_hr.CNJExportItem[TheArtiklList.Count];

      Artikl artikl;

      for(int i = 0; i < theCNJExport.Items.Length; ++i)
      {
         artikl = TheArtiklList[i];

         itemLine = theCNJExport.Items[i] = new Jeftinije_hr.CNJExportItem();

#endregion GORE

         itemLine.name          = artikl.ArtiklName;
         itemLine.ID            = artikl.ArtiklCD  ;
         itemLine.description   = artikl.LongOpis  ;
         itemLine.link          = artikl.Url       ;
         itemLine.mainImage     = artikl.Napomena  ;
         itemLine.price         = artikl.AS_InvFinDiffMPC.ToStringVv_NoGroup(); // WebCg + PDV! 
         itemLine.fileUnder     = artikl.Grupa3Name;
         itemLine.EAN           = artikl.BarCode1  ;
         itemLine.cenCategoryId = artikl.Grupa3CD  ;

#region DOLE

      } // for(int i = 0; i < theCNJExport.Items.Length; ++i)

      string xmlString = theCNJExport.SaveToFile(fileName, ZXC.VvUTF8Encoding_noBOM);

      ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Gotovo.\n\nKreirao datoteku\n\n" + fileName);

      return true;

#endregion DOLE
   }

}

public class RptR_Artikl2BCterminal : RptR_StandardRiskReport
{
   public RptR_Artikl2BCterminal(string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle)

      : base(new Vektor.Reports.RIZ.CR_Artikl2BCterminal() as ReportDocument, _reportName, _rptFilter, _filterStyle,

         true , // ArtiklWithArtstat 
         false, // ArtStat        
         false, // Faktur         
         false, // Rtrans         
         false, // Kupdob         
         false, // Prjkt          
         false, // Rtrans4ruc     
         false) // Artikl         

   {
      IsForExport = true;
   }

   public override int FillRiskReportLists()
   {
      base.FillRiskReportLists();

      Artikl artiklWadditionalBC_rec;

      foreach(Artikl artikl_rec in TheArtiklList.ToList()) // ženial! Sa ovim '.ToList()' zapravo napravis deep copy pa ne moras for petlju (nemres mjenjati List-u koju iteriras) 
      {
         if(artikl_rec.BarCode2.NotEmpty())
         {
            artiklWadditionalBC_rec = artikl_rec.MakeDeepCopy();

            artiklWadditionalBC_rec.IsAkcija = true;

            artiklWadditionalBC_rec.BarCode1 = artikl_rec.BarCode2;

            artikl_rec.BarCode2 = artiklWadditionalBC_rec.BarCode2 = "";

            TheArtiklList.Add(artiklWadditionalBC_rec);
         }
      }

      TheArtiklList = TheArtiklList.OrderBy(art => art.ArtiklName).ToList();

      return 0;
   }

   public override string ExportFileName
   {
      get
      {
         return "VvBCS_Artikli" /*+ " @ " + DateTime.Now.ToString(ZXC.VvTimeStampFormat4FileName)*/ + "." + "txt";
      }
   }

   public override bool ExecuteExport(string fileName)
   {
      using(StreamWriter sw = new StreamWriter(fileName, false, Encoding.GetEncoding(1250)))
      {
         string dumpLine /*= "\"Product SKU\";\"Price Shopper Group\";\"Cost Price\";\"Stock\""*/;
         //sw.WriteLine(dumpLine);

         foreach(Artikl artikl_rec in TheArtiklList)
         {
            dumpLine = BuildStringForExportLine(artikl_rec);

            sw.WriteLine(dumpLine);
         }
      }

      ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Gotovo.\n\nKreirao datoteku\n\n" + fileName);

      return true;
   }

   private string BuildStringForExportLine(Artikl artikl_rec)
   {
      string separator = ";";

      return

         artikl_rec.RecID        + separator + // Artikl ID        
         artikl_rec.ArtiklCD     + separator + // Šifra            
         ""                      + separator + // Kataloški broj   
         artikl_rec.BarCode1     + separator + // Bar code         
         artikl_rec.ArtiklName   + separator + // Naziv robe       
         0.00M                   + separator + // VP Cijena        
         0.00M                   + separator + // MP Cijena        
         artikl_rec.AS_StanjeKol + separator + // Stanje           
         artikl_rec.ArtiklName2  + separator + // Drugi naziv robe 
         ""                      + separator ; // Treći naziv robe 
   }
}

public class RptR_SVD_HALMED_Potrosnja : RptR_StandardRiskReport
{
   public RptR_SVD_HALMED_Potrosnja(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle)

      : base(/*new Vektor.Reports.RIZ.CR_TemboWebShopExport() as ReportDocument*/ _reportDocument, _reportName, _rptFilter, _filterStyle,

         true , // ArtiklWithArtstat 
         false, // ArtStat        
         false, // Faktur         
         false, // Rtrans         
         false, // Kupdob         
         false, // Prjkt          
         false, // Rtrans4ruc     
         false) // Artikl         

   {
      //IsForExport = true;
   }

   public override int FillRiskReportLists()
   {
      return base.FillRiskReportLists();

      // Gore se u GetArtiklWithArtstatList()                                       
      // if(reportDocument is Vektor.Reports.RIZ.CR_SVD_HALMED)                     
      // napune:                                                                    
                                                                                    
      // vektorArtikl.AS_HalmedORG = ZXC.ValOrZero_Decimal(halmedArtikl.br_pak, 2); 
      //                                                                            
      // vektorArtikl.SerNo       = halmedArtikl.s_atk    ; // S_ATK                
      // vektorArtikl.Placement   = halmedArtikl.naziv    ; // NAZIV                
      // vektorArtikl.Boja        = halmedArtikl.br_pak   ; // BR_PAK               
      // vektorArtikl.MadeIn      = halmedArtikl.doza     ; // DOZA                 
      // vektorArtikl.Url         = halmedArtikl.opis_doze; // OPIS_DOZE            
      // vektorArtikl.CarTarifa   = halmedArtikl.mj_ozn   ; // MJ_OZN               
      // vektorArtikl.PartNo      = halmedArtikl.obl_ozn  ; // OBL_OZN              
      // vektorArtikl.LinkArtCD   = halmedArtikl.s_mj     ; // s_mj                 
      // vektorArtikl.PdvKat      = halmedArtikl.s_obl    ; // s_obl                
      // vektorArtikl.LongOpis    = halmedArtikl.s_par_pro; // s_par_pro            
      // vektorArtikl.PrefValName = halmedArtikl.par_naziv; // par_naziv            

   }

   public override string ExportFileName
   {
      get
      {
         return ZXC.CURR_prjkt_rec.Ticker + " za HALMED" + " @ " + DateTime.Now.ToString(ZXC.VvTimeStampFormat4FileName) + "." + "xml";
      }
   }

   public override bool ExecuteExport(string fileName)
   {
#region GORE

      Halmed_SVD.podaci thePodaci = new Halmed_SVD.podaci();

      Halmed_SVD.podaciPotrosnja itemLine;

      thePodaci.Items = new Halmed_SVD.podaciPotrosnja[TheArtiklList.Count];

      Artikl vektorArtikl;

      bool isNaHzzo;
      //List<Halmed_SVD.HALMEDartikl> halmedArtiklList = VvDaoBase.Get_HALMEDartikl_List(TheDbConnection);

      //Halmed_SVD.HALMEDartikl halmedArtikl;

      for(int i = 0; i < thePodaci.Items.Length; ++i)
      {
         vektorArtikl = TheArtiklList[i];

         //halmedArtikl = halmedArtiklList.SingleOrDefault(ha => ha.s_lio == vektorArtikl.AtestBr);

         itemLine = thePodaci.Items[i] = new Halmed_SVD.podaciPotrosnja();

         // old: 
         isNaHzzo = vektorArtikl.ArtiklCD2.Length == 11; // vektor ATK11 je popunjen 

         // 11.03.2022: todo !!! kako od sada nadalje zakljucivati isNaHzzo? na osnovu nove kolone u 'HALMED_artikl_22' (D,N,O) 
         isNaHzzo = vektorArtikl.SnagaJM == "D" || vektorArtikl.SnagaJM == "O"; // !!! PROVJERI NEGDJE !!! TODO: !!! 

         #endregion GORE

         itemLine.s_lio     = vektorArtikl.AtestBr    ;                                                                                                                              
         itemLine.naziv     = vektorArtikl.Placement  ;
         itemLine.br_pak    = vektorArtikl.Boja       ; // vektorArtikl.AS_HalmedORG je decimal varijanta ovog podatka 
         itemLine.doza      = vektorArtikl.MadeIn     ;
         itemLine.s_mj      = vektorArtikl.LinkArtCD  ;
         itemLine.mj_ozn    = vektorArtikl.CarTarifa  ;
         itemLine.s_obl     = vektorArtikl.PdvKat     ;
         itemLine.obl_ozn   = vektorArtikl.PartNo     ;
         itemLine.s_par_pro = vektorArtikl.LongOpis   ;
         itemLine.par_naziv = vektorArtikl.PrefValName;
         itemLine.god       = ZXC.projectYear         ;
         itemLine.s_par     = "3000205"               ; 
         itemLine.dat_pot   = DateTime.Today.ToString(ZXC.VvDateDdMmYyyyFormat)       ;
         itemLine.br_pku    = vektorArtikl.AS_HalmedBOP .ToStringVv_NoGroup_ForceDot();
         itemLine.br_pk     = vektorArtikl.AS_UkIzlazKol.ToStringVv_NoGroup_ForceDot();

         if(isNaHzzo)
         {
            itemLine.na_hzzo = itemLine.br_pku;
            itemLine.ne_hzzo = 0M.ToStringVv_NoGroup_ForceDot();
         }
         else
         {
            itemLine.na_hzzo = 0M.ToStringVv_NoGroup_ForceDot();
            itemLine.ne_hzzo = itemLine.br_pku;
         }

         itemLine.vpc       = vektorArtikl.AS_HalmedCOP    .ToStringVv_NoGroup_ForceDot();
         itemLine.iznos     = vektorArtikl.AS_UkIzlazFinNBC.ToStringVv_NoGroup_ForceDot();
         itemLine.prebacen  = "0";

#region DOLE

      } // for(int i = 0; i < theCNJExport.Items.Length; ++i)

      string xmlString = thePodaci.SaveToFile(fileName, ZXC.VvUTF8Encoding_noBOM);

      return true;

#endregion DOLE
   }

}

#endregion SKLAD_Kretanje

#region RptR_RekapFaktur

public class RptR_RekapFaktur       : RptR_StandardRiskReport
{
   protected string FakturGR;

   public RptR_RekapFaktur(string _fakturGR, /*ReportDocument _reportDocument,*/ string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)

      : base(
      (_rptFilter.TT == Faktur.TT_TRI || _rptFilter.TT == Faktur.TT_VMI                  ) ? new Vektor.Reports.RIZ.CR_RekapFaktur_TRI()     as ReportDocument : // 02.09.2015. TRI i VMI
      (_rptFilter.TT == Faktur.TT_MVI                                                    ) ? new Vektor.Reports.RIZ.CR_RekapFaktur_MVI()     as ReportDocument : // 02.09.2015. MVI
      (_rptFilter.TT == Faktur.TT_ZPC                                                    ) ? new Vektor.Reports.RIZ.CR_RekapFaktur_ZPC()     as ReportDocument : // 07.09.2015. ZPC
      (_rptFilter.TT == Faktur.TT_PSM                                                    ) ? new Vektor.Reports.RIZ.CR_RekapFaktur_PSM()     as ReportDocument : // 07.09.2015. ZPC
      (ZXC.TtInfo(_rptFilter.TT).IsMalopFin_UorVMIorTRI                                  ) ? new Vektor.Reports.RIZ.CR_RekapFaktur_Malop_U() as ReportDocument :
      (ZXC.TtInfo(_rptFilter.TT).IsMalopFin_I                                            ) ? new Vektor.Reports.RIZ.CR_RekapFaktur_Malop_I() as ReportDocument :
      (_rptFilter.TT == Faktur.TT_MSI                                                    ) ? new Vektor.Reports.RIZ.CR_RekapFaktur_MSI()     as ReportDocument : // 03.09.2015. MSI
                                                                                             new Vektor.Reports.RIZ.CR_RekapFaktur_Light()   as ReportDocument ,
      _reportName, 
      _rptFilter, 
      _filterStyle,

         _rptNeeds_ArtWars,    // ArtiklWithArtstat 
         _rptNeeds_ArtStat,    // ArtStat        
         _rptNeeds_Faktur ,    // Faktur         
         _rptNeeds_Rtrans ,    // Rtrans         
         _rptNeeds_Kupdob ,    // Kupdob         
         _rptNeeds_Prjkt  ,    // Prjkt          
         (ZXC.TtInfo(_rptFilter.TT).IsMalopTTorVMIorTRI) ? true : _rptNeeds_Rtrans4ruc, // Rtrans4ruc     
         _rptNeeds_Artikl    ) // Artikl         

   {
      FakturGR = _fakturGR;
      RptFilter.NeedsGroupTree = (RptFilter.GrupiranjeDokum == "KupdobName") ? true : false;
      //TheFilterSet = RiskFilterSetEnum.REALIZ;

      if(TheReportUC != null && TheReportUC.TheReportViewer != null)
      {
         TheReportUC.TheReportViewer.DoubleClickPage -= new PageMouseEventHandler(TheReportUC.     TheReportViewer_DoubleClickPage);
         TheReportUC.TheReportViewer.DoubleClickPage += new PageMouseEventHandler(RptR_RekapFaktur_TheReportViewer_DoubleClickPage);
      }

   }

   void RptR_RekapFaktur_TheReportViewer_DoubleClickPage(object sender, PageMouseEventArgs e)
   {
    //if(e.ObjectInfo.Name == "TtipBr1") // "IFA-100123" 
    //{
    //   ZXC.TheVvForm.ShowFakturDUC_For_TipBr(e.ObjectInfo.Text);
    //}
    //if(e.ObjectInfo.Name == "TdokNum1") // "000190" 
    //{
    //   ZXC.TheVvForm.ShowNalogDUC_For_DokNum(e.ObjectInfo.Text);
    //}

      if(e.ObjectInfo.Name == "TtNum1") // "100123" 
      {
         if(RptFilter.TT.IsEmpty() || e.ObjectInfo.Text.IsEmpty()) return;

         string tipBr, tt   ;
         uint          ttNum;

         tt    = RptFilter.TT;
         ttNum = ZXC.ValOrZero_UInt(e.ObjectInfo.Text);

         if(ttNum.IsZero()) return;

         tipBr = Faktur.Set_TT_And_TtNum(tt, ttNum);

         ZXC.TheVvForm.ShowFakturDUC_For_TipBr(tipBr);
      }
   }


   public override int FillRiskReportLists()
   {

      // 07.06.2012:
      if(ReportNeedsPGyearData(RptFilter.DatumOd))
      {
         for(int year = RptFilter.DatumOd.Year; year < ZXC.projectYearFirstDay.Year; ++year)
         {
            GetFakturList(year);
         }
      }

      base.FillRiskReportLists();

      // 15.01.2015: 
      if(ZXC.IsTEXTHOshop) 
         TheFakturList
            .RemoveAll(fak => 
               fak.SkladCD .SubstringSafe(0, 2) != ZXC.vvDB_ServerID.ToString() && fak.SkladCD .SubstringSafe(0, 2) != ZXC.vvDB_ServerID_CENTRALA.ToString() &&
               fak.SkladCD2.SubstringSafe(0, 2) != ZXC.vvDB_ServerID.ToString() && fak.SkladCD2.SubstringSafe(0, 2) != ZXC.vvDB_ServerID_CENTRALA.ToString());

      // 25.03.2014: 
      VvLookUpItem lui = ZXC.luiListaRiskVrstaPl.SingleOrDefault(np => np.Cd.ToUpper() == "VIRMAN");
      string virmanStr = lui == null ? "VIRMAN" : ZXC.luiListaRiskVrstaPl.SingleOrDefault(np => np.Cd.ToUpper() == "VIRMAN").Cd;
      if(virmanStr.IsEmpty()) virmanStr = "VIRMAN";
      TheFakturList.ForEach(f => { if(f.NacPlac.IsEmpty()) f.NacPlac = virmanStr; });

      if(RptFilter.IsForceOtsByDokDate) // trik / magic 
      {
         GroupJoinFakturWithRtrans();
         TheFakturList.RemoveAll(f => f.Has_TrnSum_vs_S_Sum_Discrepancy.IsEmpty());
         TheFakturList.ForEach(f => { f.KupdobName = f.Has_TrnSum_vs_S_Sum_Discrepancy.Replace("S-uk ", "").Replace(" TrnSum ", ""); });
      }

    //11.11.2015.
    //if(ZXC.TtInfo(RptFilter.TT).IsMalopTTorVMIorTRI)
      bool isRptR_RekapIRMasBlagIzvj   = (this is RptR_RekapIRMasBlagIzvj  );
      bool isRptR_Rekap_TH_DjelatRabat = (this is RptR_Rekap_TH_DjelatRabat); // 18.04.2019.
    //if(ZXC.TtInfo(RptFilter.TT).IsMalopTTorVMIorTRI && (isRptR_RekapIRMasBlagIzvj == false))
      if(ZXC.TtInfo(RptFilter.TT).IsMalopTTorVMIorTRI && (isRptR_RekapIRMasBlagIzvj == false) && (isRptR_Rekap_TH_DjelatRabat == false))
      {
         AggregateFaktursIraRucProperties(/* isRucPoStavkama */ false);

         this.CrossData = GetCrossData(TheFakturList);
      }

      // 11.10.2016: za Rekap IRA sa R_firstRtransPodJM potrebom ___ start ___ 
      bool doWeNeed_kolOP_data = (RptFilter.TT == Faktur.TT_IRA && ZXC.RRD.Dsc_IsOrgPakVisible);
      if(doWeNeed_kolOP_data)
      {
         TheRtransList = new List<Rtrans>();
         GetRtransWithArtstatList();
         GroupJoinFakturWithRtrans();
         TheArtiklList = new List<Artikl>(VvUserControl.ArtiklSifrar);
         TheFakturList.ForEach(fak => fak.SetAllRtrans_R_mjData(TheArtiklList));
      }
      // 11.10.2016: za Rekap IRA sa R_firstRtransPodJM potrebom ___  end  ___ 

      // 28.03.2017: PPR po stupnju dovrsenosti ___ START ___ 
      if(RptFilter.TT == Faktur.TT_PPR)
      {
      }
      // 28.03.2017: PPR po stupnju dovrsenosti ___  END  ___ 

      // 28.11.2016: 
    //TheFakturLightList = TheFakturList.Select(faktur =>                             new FakturLight(RptFilter.FuseBool3, faktur, GetFakGRdata(FakturGR, faktur))).ToList();
      TheFakturLightList = TheFakturList.Select(faktur => isRptR_RekapIRMasBlagIzvj ? new FakturLight(RptFilter.FuseBool3, faktur, GetFakGRdata(FakturGR, faktur), true ) : 
                                                                                      new FakturLight(RptFilter.FuseBool3, faktur, GetFakGRdata(FakturGR, faktur), false)).ToList();

    //// 28.04.2016: Crvenilo handling / reporting ___ START ___ 
    //if(TheFakturList.Any(f => f.Has_TrnSum_vs_S_Sum_Discrepancy.NotEmpty()))
    //{
    //   foreach(var fak in TheFakturList.Where(f => f.Has_TrnSum_vs_S_Sum_Discrepancy.NotEmpty()))
    //   {
    //      ZXC.aim_emsg(MessageBoxIcon.Warning ,"TrnSum_vs_S_Sum_Discrepancy\n\n{0}\n\n{1}", fak.TT_And_TtNum, fak.Has_TrnSum_vs_S_Sum_Discrepancy);
    //   }
    //}
    //// 28.04.2016: Crvenilo handling / reporting ___  END  ___ 

      return 0;
   }

   protected int BaseFillRiskReportLists()
   {
      return base.FillRiskReportLists();
   }

}

public class RptR_RekapCompare      : RptR_RekapFaktur
{
   public RptR_RekapCompare(string _fakturGR, ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)

      : base(_fakturGR, /*_reportDocument,*/ _reportName, _rptFilter, _filterStyle, _rptNeeds_ArtWars, _rptNeeds_ArtStat, _rptNeeds_Faktur, _rptNeeds_Rtrans, _rptNeeds_Kupdob, _rptNeeds_Prjkt, _rptNeeds_Rtrans4ruc, _rptNeeds_Artikl)
   {
      this.reportDocument = _reportDocument;
   }

   public override int FillRiskReportLists()
   {
#region Get THIS razdoblje TheFakturList

      base.BaseFillRiskReportLists();

#endregion Get THIS razdoblje TheFakturList

#region Get PRETHODNO razdoblje TheFakturPRList

      VvSqlFilterMember[] fmArray = RptFilter.FilterMembers.ToArray();

      fmArray[0].value = TheReportUC.TheRiskFilterUC.Fld_CompDateOd;
      fmArray[1].value = TheReportUC.TheRiskFilterUC.Fld_CompDateDo;

      TheFakturBBBList = new List<Faktur>();

      int year = RptFilter.Date2.Year;
      bool isSomeOtherYear = year < ZXC.projectYearFirstDay.Year;

      VvDaoBase.LoadGenericVvDataRecordList(isSomeOtherYear ? ZXC.TheSecondDbConn_SameDB_OtherYear(year) : TheDbConnection, TheFakturBBBList, fmArray.ToList(), "", FakturOrderBy, true);

      if(HRDweWant && year <= 2022)
      {
         TheFakturBBBList.Where(f => f.DokDate.Year == year).ToList().ForEach(f => f.Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB(null));
      }

      #endregion Get PRETHODNO razdoblje TheFakturPRList

      if(IsGroupingChronologically(FakturGR)) // Kronolosko grupiranje 
      {
         DateTime DateOd_AAA = TheReportUC.TheRiskFilterUC.Fld_DatumOd   ; // AAA - prvi red, BBB - drugi red 
         DateTime DateDo_AAA = TheReportUC.TheRiskFilterUC.Fld_DatumDo   ; // AAA - prvi red, BBB - drugi red 
         DateTime DateOd_BBB = TheReportUC.TheRiskFilterUC.Fld_CompDateOd; // AAA - prvi red, BBB - drugi red 

         //bool isWholeMonths;

         //if(DateOd_AAA.Day == 1 && DateDo_AAA.Day == DateTime.DaysInMonth(DateDo_AAA.Year, DateDo_AAA.Month)) // prvi i zadnji dan u mjesecu 
         //   isWholeMonths = true;
         //else
         //   isWholeMonths = false;

         int dayOffset   = DateOd_AAA.Day   - DateOd_BBB.Day  ;
         int monthOffset = DateOd_AAA.Month - DateOd_BBB.Month;
         int yearOffset  = DateOd_AAA.Year  - DateOd_BBB.Year ;

         int weekOffset  = DateOd_AAA.GetWeekOfYear() - DateOd_BBB.GetWeekOfYear();
         
         TheFakturList.ForEach(fak => 
                                 {
                                    fak.VezniDok = GetOffsetDateAsStr(FakturGR, fak.DokDate, fak.AddTS,          0,            0,           0,           0, false);
                                    fak.NacPlac  = GetOffsetDateAsStr(FakturGR, fak.DokDate, fak.AddTS, -dayOffset, -monthOffset, -yearOffset, -weekOffset, true ); // ako je BBBlista prazna za tu grupu, da onda odglumimo offsetData 
                                 }
                              );
         
         // PonudDate = backup of DokDate, DospDate = backup of AddTS date
         TheFakturBBBList.ForEach(fakPR =>
                                    {
                                       fakPR.PonudDate = fakPR.DokDate; 
                                       fakPR.DospDate  = fakPR.AddTS; 
                                       fakPR.VezniDok  = GetOffsetDateAsStr(FakturGR, fakPR.DokDate, fakPR.AddTS, dayOffset, monthOffset, yearOffset, weekOffset, true);
                                    }
                                 ); 

      } // if(IsGroupingChronologically(FakturGR)) // Kronolosko grupiranje 

      List<FakturLight> theFakturBBBLightList;

      TheFakturLightList    = TheFakturList   .Select(faktur => new FakturLight(RptFilter.FuseBool3, faktur, GetFakGRdata(FakturGR, faktur, IsGroupingChronologically(FakturGR)), false, FakturGR)).ToList(); // THIS period 
      theFakturBBBLightList = TheFakturBBBList.Select(faktur => new FakturLight(RptFilter.FuseBool3, faktur, GetFakGRdata(FakturGR, faktur, IsGroupingChronologically(FakturGR)), true , FakturGR)).ToList(); // PREV period 

    //TheFakturLightList    = TheFakturBBBList.Select(faktur => new FakturLight(RptFilter.FuseBool3, faktur, GetFakGRdata(FakturGR, faktur, IsGroupingChronologically(FakturGR)), true , FakturGR)).ToList(); // PREV period 
    //theFakturBBBLightList = TheFakturList   .Select(faktur => new FakturLight(RptFilter.FuseBool3, faktur, GetFakGRdata(FakturGR, faktur, IsGroupingChronologically(FakturGR)), false, FakturGR)).ToList(); // THIS period 

      TheFakturLightList.AddRange(theFakturBBBLightList);

      return 0;
   }

   private string GetOffsetDateAsStr(string fakturGRtype, DateTime origDate_BBB, DateTime addTS_BBB, int dayOffset, int monthOffset, int yearOffset, int weekOffset, bool isAAArazdoblje)
   {
      int orgDay_BBB, orgMonth_BBB, orgYear_BBB, orgWeek_BBB;
      int newDay_BBB, newMonth_BBB, newYear_BBB, newWeek_BBB = 0; 

      orgDay_BBB   = origDate_BBB.Day  ;
      orgMonth_BBB = origDate_BBB.Month;
      orgYear_BBB  = origDate_BBB.Year ;
      orgWeek_BBB  = origDate_BBB.GetWeekOfYear();

      newDay_BBB   = orgDay_BBB                ;
      newMonth_BBB = orgMonth_BBB + monthOffset;
      newYear_BBB  = orgYear_BBB  + yearOffset ;
      newWeek_BBB  = orgWeek_BBB  + weekOffset ;

      return GetGrDataForDate(fakturGRtype, newYear_BBB, newMonth_BBB, newDay_BBB, newWeek_BBB, addTS_BBB);
   }

   private bool IsGroupingChronologically(string fakturGRtype)
   {
      switch(fakturGRtype)
      {
         case ""          : 
         case "AddUID"    : 
         case "DevName"   : 
         case "ProjektCD" : 
         case "Putnik"    : 
         case "NacPlac"   :
         case "KupdobName":
         case "PosJedName": 
         case "MtrosName" : 
         case "TT"        : 
         case "DayOfWeek" : 
         case "SkladCD"   : 
         case "HourOfDay" : return false;

         case "AddTS"     : // DokHour u biti 
         case "DokYear"   :
         case "DokMonth"  : 
         case "DokWeek"   : 
         case "DokDay"    : return true;

         default: ZXC.aim_emsg(MessageBoxIcon.Error, "FakturGRtype [{0}] nedefiniran u GetFakGRdata()!", fakturGRtype); return false;
      }
   }

   public static bool IsFilterWellFormed(RiskReportUC reportUC)
   {
      bool OK = true;
      VvRpt_RiSk_Filter filter = reportUC.TheRptFilter;

      if(reportUC.TheRiskFilterUC.Fld_CompDateOd.IsEmpty() ||
         reportUC.TheRiskFilterUC.Fld_CompDateDo.IsEmpty())
      {
         ZXC.aim_emsg("Molim, zadajte usporedno razdoblje.");
         return false;
      }

      return (OK);
   }

}

public class RptR_RekapIRMasBlagIzvj : RptR_RekapFaktur
{
   public RptR_RekapIRMasBlagIzvj(string _fakturGR, ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl) :

      base(_fakturGR, /*_reportDocument,*/ _reportName, _rptFilter, _filterStyle, _rptNeeds_ArtWars, _rptNeeds_ArtStat, _rptNeeds_Faktur, _rptNeeds_Rtrans, _rptNeeds_Kupdob, _rptNeeds_Prjkt, _rptNeeds_Rtrans4ruc, _rptNeeds_Artikl)
   {
      this.reportDocument = _reportDocument;
   }

   public override int FillRiskReportLists()
   {
      base.FillRiskReportLists();

      // Fill Faktur_rec_SumaRazdoblja_BLG, fill Faktur_rec_DonosPretRazd_BLG 

      /*Faktur_rec_DonosPretRazd_BLG = */

      LoadBLAGSumaPrethodRazdob(TheDbConnection, true);

      return 0;
   }
   
   protected void LoadBLAGSumaPrethodRazdob(XSqlConnection conn, bool needsLineCount)
   {
      //Faktur faktur_rec;

      // ako trazimo dnevnik od pocetka godine onda je prethodno razdoblje prazno / null ... (a sta ako blagakna ima pocetno stanje?) 
      //if(RptFilter.DatumOd == ZXC.projectYearFirstDay) return new Faktur();

      DateTime origDateOd = RptFilter.DatumOd; // da moze restore-ati originalni dateOd 
      DateTime origDateDo = RptFilter.DatumDo; // da moze restore-ati originalni dateDo 

      // Dakle, ako zelimo analitiku razdoblja 1.3.2011 - 31.3.2011 
      // onda DonosPretRazd_BLG         ide od 1.1.2011 - 28.2.2011 
      RptFilter.DatumDo = RptFilter.DatumOd.AddDays(-1);
      RptFilter.DatumOd = ZXC.projectYearFirstDay.AddDays(-1); // da se dobije 31.12. prosle godine 

      TheReportUC.AddFilterMemberz();

      RptFilter.FilterMembers.RemoveAll(fm => fm.name.ToUpper().Contains("TT") ||
                                              fm.forcedColName.ToUpper().Contains("TT"));

      string IN_clause = TtInfo.Blagajna_IN_Clause;

      RptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", IN_clause, " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!

      //faktur_rec = FakturDao.GetManyFakturSumAsOneSintFaktur(conn, RptFilter.FilterMembers, /*_isUra*/ true, needsLineCount, "");
    //base.FillRiskReportLists();
      // Fill TheFakturList 

      List<Faktur> donosFakturList = new List<Faktur>();

      VvDaoBase.LoadGenericVvDataRecordList(TheDbConnection, donosFakturList, RptFilter.FilterMembers, "", /*FakturOrderBy*/"dokDate, ttSort, ttNum ", true, " * ", ""); 

      Faktur_rec_DonosPretRazd_BLG = new Faktur();
      Faktur_rec_DonosPretRazd_BLG.SumValuesFromList(donosFakturList);

      RptFilter.DatumOd = origDateOd;
      RptFilter.DatumDo = origDateDo;

      TheReportUC.AddFilterMemberz();
   }
}

public class RptR_Rekap_FISK_Faktur : RptR_StandardRiskReport
{
   public RptR_Rekap_FISK_Faktur(string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)

      : base(new Vektor.Reports.RIZ.CR_RekapFiskalRn() as ReportDocument,
         _reportName         ,                                                    
         _rptFilter          , 
         _filterStyle        ,
         _rptNeeds_ArtWars   , // ArtiklWithArtstat 
         _rptNeeds_ArtStat   , // ArtStat        
         _rptNeeds_Faktur    , // Faktur         
         _rptNeeds_Rtrans    , // Rtrans         
         _rptNeeds_Kupdob    , // Kupdob         
         _rptNeeds_Prjkt     , // Prjkt          
         _rptNeeds_Rtrans4ruc, // Rtrans4ruc     
         _rptNeeds_Artikl    ) // Artikl         

   {

   }

   public override int FillRiskReportLists()
   {
      base.FillRiskReportLists();

      //TheFakturList.RemoveAll(faktur => faktur.IsFiskalDutyFaktur_ONLINE == false);
      //
      //// 16.01.2015: 
      //TheFakturList.RemoveAll(faktur => faktur.FiskJIR.NotEmpty());

      // 02.06.2015: 

      TheFakturList.RemoveAll(faktur => faktur.IsFiskalDutyFaktur_ONLINE == false || faktur.FiskJIR.NotEmpty());

      return 0;
   }

}

public class RptR_Rekap_IRMvsFtrans : RptR_StandardRiskReport
{
   public bool isIRA;

   public RptR_Rekap_IRMvsFtrans(bool _isIRA, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)

      : base(
      (_isIRA ? new Vektor.Reports.RIZ.CR_Rekap_IRAvsFtrans() as ReportDocument : new Vektor.Reports.RIZ.CR_Rekap_IRMvsFtrans() as ReportDocument),
         _reportName         ,                                                    
         _rptFilter          , 
         _filterStyle        ,
         _rptNeeds_ArtWars   , // ArtiklWithArtstat 
         _rptNeeds_ArtStat   , // ArtStat        
         _rptNeeds_Faktur    , // Faktur         
         _rptNeeds_Rtrans    , // Rtrans         
         _rptNeeds_Kupdob    , // Kupdob         
         _rptNeeds_Prjkt     , // Prjkt          
         _rptNeeds_Rtrans4ruc, // Rtrans4ruc     
         _rptNeeds_Artikl    ) // Artikl         

   {
      this.isIRA = _isIRA;
   }

   public override int FillRiskReportLists()
   {
      base.FillRiskReportLists();

#region Init 

      uint mtrosCD;
      bool isDug;
      Kupdob kupdob_rec;
      string konto = "";
      string currSkladCD;
      decimal prNabVrij;
      decimal uracMarza = 0M;
      decimal uracPDV   = 0M, uracPDV05, uracPDV13, uracPDV25;
      decimal razdSklad = 0M, razdSklad00, razdSklad05, razdSklad13, razdSklad25;

      decimal prNabVrij_Risk;
      decimal uracMarza_Risk;
      decimal uracPDV_Risk  ;
      decimal razdSklad_Risk;

      TheManyDecimalsList = new List<VvManyDecimalsReportSourceRow>();

      VvLookUpLista luiLista = ZXC.luiListaSkladista; // just as shortcut 
      VvLookUpItem  lui;

#endregion Init

      AggregateFaktursIraRucProperties(false);

      foreach(var oneMonthSkladFakturGroup in TheFakturList.GroupBy(fak => fak.DokDate_Month_SKL_AsString).OrderBy(gr => gr.Key))
      {
#region Get mtrosCD from skladCD

         currSkladCD = oneMonthSkladFakturGroup.First().SkladCD;

         kupdob_rec = Kupdob.GetKupdobFromSkladCD(currSkladCD);

         if(kupdob_rec != null && kupdob_rec.IsMtr == true) mtrosCD = kupdob_rec.KupdobCD;
         else                                               mtrosCD = 0;

         lui = luiLista.GetLuiForThisCd(currSkladCD);

#endregion Get mtrosCD from skladCD

#region Ftrans values

         DateTime DatumOd = ZXC.ThisMonthFirstDay(oneMonthSkladFakturGroup.First().DokDate);
         DateTime DatumDo = ZXC.ThisMonthLastDay (oneMonthSkladFakturGroup.First().DokDate);


         // PrNabCij __________________________________________________________________________________________________ 
         if(isIRA)
         {
            isDug = false; if(lui != null && !lui.Flag) konto = Faktur2NalogRulesAndData.GetSkladKontoForSkladCD(currSkladCD);
            if(konto == "0") // nema konta u LookupListi skladista 
            {
               konto = ZXC.KSD.Dsc_Kto_Skladiste;
            }
            if(konto.IsEmpty() || konto == "0") { ZXC.aim_emsg(MessageBoxIcon.Error, "Za sklad {0}\n\nu listi skladista\n\na niti u 'pravilima' nije zadan konto.\n\nTrazim na '6600'", currSkladCD); konto = "6600"; }
            prNabVrij   = FtransDao.GetMoneyBy_Konto_Dates_MtrosCD(TheDbConnection, konto, /*RptFilter.*/DatumOd, /*RptFilter.*/DatumDo, mtrosCD, isDug, lui.Flag, isIRA);
         }
         else
         {
         isDug = true ; konto = ZXC.KSD.Dsc_Kto_Realizacija; prNabVrij   = FtransDao.GetMoneyBy_Konto_Dates_MtrosCD(TheDbConnection, konto, /*RptFilter.*/DatumOd, /*RptFilter.*/DatumDo, mtrosCD, isDug, lui.Flag, isIRA);
         // uracMarza __________________________________________________________________________________________________                    
         isDug = true ; konto = ZXC.KSD.Dsc_Mrz            ; uracMarza   = FtransDao.GetMoneyBy_Konto_Dates_MtrosCD(TheDbConnection, konto, /*RptFilter.*/DatumOd, /*RptFilter.*/DatumDo, mtrosCD, isDug, lui.Flag);
         // uracPDV   __________________________________________________________________________________________________                    
         isDug = true ; konto = ZXC.KSD.Dsc_MskPdv_25      ; uracPDV25   = FtransDao.GetMoneyBy_Konto_Dates_MtrosCD(TheDbConnection, konto, /*RptFilter.*/DatumOd, /*RptFilter.*/DatumDo, mtrosCD, isDug, lui.Flag);
         isDug = true ; konto = ZXC.KSD.Dsc_MskPdv_10      ; uracPDV13   = FtransDao.GetMoneyBy_Konto_Dates_MtrosCD(TheDbConnection, konto, /*RptFilter.*/DatumOd, /*RptFilter.*/DatumDo, mtrosCD, isDug, lui.Flag);
         isDug = true ; konto = ZXC.KSD.Dsc_MskPdv_05      ; uracPDV05   = FtransDao.GetMoneyBy_Konto_Dates_MtrosCD(TheDbConnection, konto, /*RptFilter.*/DatumOd, /*RptFilter.*/DatumDo, mtrosCD, isDug, lui.Flag);
         uracPDV = uracPDV25 + uracPDV13 + uracPDV05       ;                                                                                
         // razdSklad __________________________________________________________________________________________________                    
         isDug = false; konto = ZXC.KSD.Dsc_MSK_25         ; razdSklad25 = FtransDao.GetMoneyBy_Konto_Dates_MtrosCD(TheDbConnection, konto, /*RptFilter.*/DatumOd, /*RptFilter.*/DatumDo, mtrosCD, isDug, lui.Flag);
         isDug = false; konto = ZXC.KSD.Dsc_MSK_10         ; razdSklad13 = FtransDao.GetMoneyBy_Konto_Dates_MtrosCD(TheDbConnection, konto, /*RptFilter.*/DatumOd, /*RptFilter.*/DatumDo, mtrosCD, isDug, lui.Flag);
         isDug = false; konto = ZXC.KSD.Dsc_MSK_05         ; razdSklad05 = FtransDao.GetMoneyBy_Konto_Dates_MtrosCD(TheDbConnection, konto, /*RptFilter.*/DatumOd, /*RptFilter.*/DatumDo, mtrosCD, isDug, lui.Flag);
         isDug = false; konto = ZXC.KSD.Dsc_MSK_00         ; razdSklad00 = FtransDao.GetMoneyBy_Konto_Dates_MtrosCD(TheDbConnection, konto, /*RptFilter.*/DatumOd, /*RptFilter.*/DatumDo, mtrosCD, isDug, lui.Flag);
         razdSklad = razdSklad25 + razdSklad13 + razdSklad05 + razdSklad00;
         }

#endregion Ftrans values

#region Faktur Values

         prNabVrij_Risk = oneMonthSkladFakturGroup.Sum(fak => fak.Ira_ROB_NV                  );
         uracMarza_Risk = oneMonthSkladFakturGroup.Sum(fak => fak.R_ukMskMrz - fak.R_NivMrz   ); // not bene '-' 
         uracPDV_Risk   = oneMonthSkladFakturGroup.Sum(fak => fak.R_ukMskPdv - fak.R_NivMskPdv); // not bene '-' 
         razdSklad_Risk = oneMonthSkladFakturGroup.Sum(fak => fak.R_ukMSK    - fak.R_NivVrj   ); // not bene '-' 

#endregion Faktur Values

#region Add To TheManyDecimalsList (report source)

         TheManyDecimalsList.Add(new VvManyDecimalsReportSourceRow() 
         {
            RowStringCD = oneMonthSkladFakturGroup.Key,
            RowName     = Faktur2NalogRulesAndData.GetSkladKontoForSkladCD(currSkladCD), 
            TheStr      = currSkladCD,
            Date_1      = ZXC.ThisMonthLastDay(oneMonthSkladFakturGroup.First().DokDate),

            DecimA01    = prNabVrij_Risk              .Ron2(), 
            DecimA02    = uracMarza_Risk              .Ron2(), 
            DecimA03    = uracPDV_Risk                .Ron2(), 
            DecimA04    = razdSklad_Risk              .Ron2(),

            DecimA05    = prNabVrij                   .Ron2(), 
            DecimA06    = uracMarza                   .Ron2(), 
            DecimA07    = uracPDV                     .Ron2(), 
            DecimA08    = razdSklad                   .Ron2(),

            DecimA09    = (prNabVrij_Risk - prNabVrij).Ron2(), 
            DecimA10    = (uracMarza_Risk - uracMarza).Ron2(), 
            DecimA11    = (uracPDV_Risk   - uracPDV  ).Ron2(), 
            DecimA12    = (razdSklad_Risk - razdSklad).Ron2() 

         });

#endregion Add To TheManyDecimalsList (report source)

      }

      return TheFakturList.Count;
   }

   public static bool IsFilterWellFormed(bool isIRA, RiskReportUC reportUC)
   {
      bool OK = true;
      VvRpt_RiSk_Filter filter = reportUC.TheRptFilter;

      if(!isIRA && filter.TT != Faktur.TT_IRM)
      {
         ZXC.aim_emsg("Molim, zadajte TT dokumenta IRM.");
         return false;
      }

      if(isIRA && filter.TT != Faktur.TT_IRA && filter.IsPrihodTT == false)
      {
         ZXC.aim_emsg("Molim, zadajte TT dokumenta IRA ili 'PrihodTT'.");
         return false;
      }

      return (OK);
   }

}

public class RptR_Rekap_RNM : RptR_StandardRiskReport
{
   public bool isPipArtiklRekap;
   public bool isRekapRNM      ;

   public RptR_Rekap_RNM(bool _isPipArtiklRekap, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)

      : base(
      (_isPipArtiklRekap ? new Vektor.Reports.RIZ.CR_PipArtiklRekapCij() as ReportDocument : new Vektor.Reports.RIZ.CR_RekapRealizRNM() as ReportDocument),
         _reportName         ,                                                    
         _rptFilter          , 
         _filterStyle        ,
         _rptNeeds_ArtWars   , // ArtiklWithArtstat 
         _rptNeeds_ArtStat   , // ArtStat        
         _rptNeeds_Faktur    , // Faktur         
         _rptNeeds_Rtrans    , // Rtrans         
         _rptNeeds_Kupdob    , // Kupdob         
         _rptNeeds_Prjkt     , // Prjkt          
         _rptNeeds_Rtrans4ruc, // Rtrans4ruc     
         _rptNeeds_Artikl    ) // Artikl         

   {
      this.isPipArtiklRekap =  _isPipArtiklRekap;
      this.isRekapRNM       = !_isPipArtiklRekap;
   }

   public override int FillRiskReportLists()
   {
#region Init

      TheManyDecimalsList = new List<VvManyDecimalsReportSourceRow>();

      decimal RI_razd, pipArtiklIznosPPR_SUM, pipArtiklIznosOTP_SUM, pipArtiklIznosPPR_OTP_SUM, pipArtiklKolSUM, pipArtiklKolKgSUM;

#endregion Init

#region Get RNM FakturList & RNM RtransList

      // ovaj ce za DateOD - DateDO napuniti RNM Fakturs i pripadajuce Trans-ove 
      base.FillRiskReportLists();

#endregion Get RNM FakturList & RNM RtransList

#region Get RNM bef period with PIP in period FakturList & RtransList

      // Sad treba pokupiti eventualne RNM-ove prije tog perioda, 
      //a koji imaju PIP-ove u tom periodu + njihove Trans-ove    

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>();

      filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.tt     ], false, "pipTT"    , Faktur.TT_PIP    , "", "", "  = ", "", "L"   ));
      filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.dokDate], false, "pipDateOD", RptFilter.DatumOd, "", "", " >= ", "", "L"   ));
      filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.dokDate], false, "pipDateDO", RptFilter.DatumDo, "", "", " <= ", "", "L"   ));
      filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.dokDate], false, "rnmDateDO", RptFilter.DatumOd, "", "", " <  ", "", "rnmF"));

      string anotherJoinClause = "LEFT JOIN faktur rnmF ON rnmF.TT = 'RNM' AND L.ProjektCD = rnmF.ProjektCD\nLEFT JOIN  faktEx rnmR ON rnmF.RecID = rnmR.fakturRecID";

      bool oldRNMsFound = 
      VvDaoBase.LoadGenericVvDataRecordList(TheDbConnection, TheFakturList, filterMembers, "L", ""/*FakturOrderBy*/, true, "rnmF.*, rnmR.* ", anotherJoinClause, false, "rnmF.ProjektCD");

      string rnmBefPeriodTtNums = TtInfo.GetSql_IN_Clause_Integer(TheFakturList.Where(rnm => rnm.DokDate < RptFilter.DatumOd).Select(rnm => (int)rnm.TtNum).ToArray());
      VvRpt_RiSk_Filter rnmBefPeriodRptFilter = new VvRpt_RiSk_Filter(true);
      rnmBefPeriodRptFilter.FilterMembers = new List<VvSqlFilterMember>();
      rnmBefPeriodRptFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.tt     ], false, "rnmTT"    , Faktur.TT_RNM    , "", "", "  = ", "", "L"   ));
      rnmBefPeriodRptFilter.FilterMembers.Add(new VvSqlFilterMember("ttNum", rnmBefPeriodTtNums, " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!

      if(oldRNMsFound)
      {
         ZXC.RtransDao.LoadManyDocumentsTtranses(TheDbConnection, TheRtransList, rnmBefPeriodRptFilter, "t_ttSort ASC, t_ttNum ASC, t_serial ASC ");
      }

#endregion Get RNM bef period with PIP in period FakturList & RtransList

#region Set rtrans's R_grName as ProjektCD for RNM rtranses

      Faktur parentFaktur;
    //Parallel.ForEach(TheRtransList, rtrans => { rtrans.R_grName = TheFakturList.Single         (fak => fak.RecID == rtrans.T_parentID).ProjektCD      ; });
    //Parallel.ForEach(TheRtransList, rtrans => { rtrans.R_grName = TheFakturList.SingleOrDefault(fak => fak.RecID == rtrans.T_parentID).ProjektCD ?? ""; }); // ?? ti je COALESCE 

      foreach(Rtrans rtrans in TheRtransList)
      {
         parentFaktur = TheFakturList.SingleOrDefault(fak => fak.RecID == rtrans.T_parentID);
      
         if(parentFaktur != null) rtrans.R_grName = parentFaktur.ProjektCD;
      }

#endregion Set rtrans's R_grName as ProjektCD for RNM rtranses

#region 1. Get Aotp Cijenu ('A3') po svakom RNM-u 2. Get PPR_PIP RtransList 3. Get NC per ulaz KG Cijenu po svakom RNM-u 4. Set OTP cijenu svakom PIP-u

      RNMstatus theRNMstatus;
      List<ZXC.NameAndDecimal_CommonStruct> RNMncPerUlKolKgOTPList = new List<ZXC.NameAndDecimal_CommonStruct>(TheFakturList.Count);

      foreach(Faktur rnmFaktur_rec in TheFakturList)
      {
         // 1. Get Aotp Cijenu ('A3') po svakom RNM-u 
         RI_razd = FtransDao.Get_RI_razd_ForProjektCD(TheDbConnection, rnmFaktur_rec.ProjektCD, ZXC.projectYearFirstDay, ZXC.projectYearLastDay, ZXC.KSD);
         rnmFaktur_rec.R2_uplata = RI_razd;

         // 2. Get PPR_PIP RtransList 
         var RtransList_ForProjektCD = RtransDao.GetRtransList_ForProjektCD(TheDbConnection, rnmFaktur_rec.ProjektCD, "t_ttSort ASC, t_ttNum ASC, t_serial ASC ");
       //RtransList_ForProjektCD.ForEach(rtr =>   rtr.R_grName = rnmFaktur_rec.ProjektCD                                                                            ); // !!! da bi dole znali koji PPR/PIP rtrans pripada kojem RNM-u 
         RtransList_ForProjektCD.ForEach(rtr => { rtr.R_grName = rnmFaktur_rec.ProjektCD; rtr.T_dokNum = (rtr.R_utilBool ? rnmFaktur_rec.DokNum+1 : rtr.T_dokNum); }); // !!! da bi dole znali koji PPR/PIP rtrans pripada kojem RNM-u 
         TheRtransList.AddRange(RtransList_ForProjektCD);

         // 3. Get NC per ulaz KG Cijenu po svakom RNM-u 
         theRNMstatus = 
            
            new RNMstatus(rnmFaktur_rec,
                  TheRtransList.Where(rtr => rtr.R_grName == rnmFaktur_rec.ProjektCD &&  rtr.T_TT == Faktur.TT_RNU) .ToList(), // rnmFaktur_rec.RNU_Transes 
                  TheRtransList.Where(rtr => rtr.R_grName == rnmFaktur_rec.ProjektCD &&  rtr.R_utilBool == false &&
                                                                                        (rtr.T_TT == Faktur.TT_PPR ||
                                                                                         rtr.T_TT == Faktur.TT_PIP)).ToList()  // rnmFaktur_rec.realizRtransList 
                  );

         RNMncPerUlKolKgOTPList.Add(new ZXC.NameAndDecimal_CommonStruct(theRNMstatus.ProjektCD, theRNMstatus.ncPerUlKolKgOTP, theRNMstatus.TtNum));

         rnmFaktur_rec.Ira_ROB_NV = theRNMstatus.ncPerUlKolKg   ; // R_cijOP_PPR: VOLIA! prema PPR fin 
         rnmFaktur_rec.K_NivVrj00 = theRNMstatus.ncPerUlKolKgOTP; // R_cijOP_OTP: VOLIA! prema OTP fin 

         rnmFaktur_rec.Fco        = ZXC.luiListaVrstaRNM .GetNameForThisCd(rnmFaktur_rec.VezniDok); // Vrsta RNM name 
         rnmFaktur_rec.PnbM       = ZXC.luiListaSkladista.GetNameForThisCd(rnmFaktur_rec.SkladCD ); // Sklad     name 
         rnmFaktur_rec.PnbV       = ZXC.luiListaSkladista.GetNameForThisCd(rnmFaktur_rec.SkladCD2); // Sklad2    name 

#region Rekap RNM 1. 2. 3.

         if(isRekapRNM)
         {
            TheManyDecimalsList.Add(new VvManyDecimalsReportSourceRow()
            {
               RowStringCD = rnmFaktur_rec.ProjektCD         ,

               DecimA01 = theRNMstatus.koefDovrsPG     * 100M, // Spg                      
               DecimA02 = theRNMstatus.koefDovrsOG     * 100M, // Sog                      
               DecimA03 = theRNMstatus.koefDovrsUK     * 100M, // Suk                      
               DecimA04 = theRNMstatus.koefDovrsUKreal * 100M, // Suk Real                 
               DecimA05 = theRNMstatus.ukKolKgUlazPG         , // Bpg                      
               DecimA06 = theRNMstatus.ukKolKgUlazOG         , // Bog                      
               DecimA07 = theRNMstatus.ukKolIzlazOG          ,
               DecimA08 = theRNMstatus.ukKolUlazOG           , // za ncPerUlKol            
               DecimA09 = theRNMstatus.ukKolKgIzlazOG        ,
               DecimA10 = theRNMstatus.ukKolKgUlazUK         , // Bpg + Bog                
               DecimA11 = theRNMstatus.ukFinIzlazPG          , // ApgUk                    
               DecimA12 = theRNMstatus.ukFinNedovrIzlPG      , // Apg                      
               DecimA13 = theRNMstatus.ukFinIzlazUK +
                          theRNMstatus.ukFinViaOTP           , // Auk + RIT                
               DecimA14 = theRNMstatus.ukFinIzlazOG          , // Aog                      
               DecimA15 = theRNMstatus.ukFinUlazOG           , // Cog                      
               DecimA16 = theRNMstatus.ukFinIzlazUK          , // Auk                      
               DecimA17 = theRNMstatus.finDiff               , // (Cog-Auk)                
               DecimA18 = theRNMstatus.ukFinViaOTP           , // RIT                      
               DecimA19 = theRNMstatus.ncPerUlKol            , // (Auk / ukKolUlazOG)      
               DecimA20 = theRNMstatus.ncPerUlKolKg          , // (Auk / Bog)              
               DecimA21 = theRNMstatus.ncPerUlKolKgOTP       ,

               IsXxx    = rnmFaktur_rec.DokDate.Year != ZXC.projectYearFirstDay.Year // RNM NIJE iz ove godine (ima PG vrijednosti) 

            });

         } // if(isRekapRNM) 

#endregion Rekap RNM 1. 2. 3.

#region PIP Artikl Cijena 4.

         else // if(isPipArtiklRekap == true) 
         {
            // 4. Set OTP cijenu svakom PIP-u 
            TheRtransList.Where(rtr => rtr.R_grName == rnmFaktur_rec.ProjektCD && rtr.T_TT == Faktur.TT_PIP).ToList()
               .ForEach(rtr => 
               { 
                  rtr.TmpDecimal   = rnmFaktur_rec.K_NivVrj00                                                  ; // INDIR       cij po kg ... jednaka za svaki PIP ovog RNM-a                     
                  rtr.TmpDecimal2  = rnmFaktur_rec.K_NivVrj00 + rnmFaktur_rec.Ira_ROB_NV                       ; // INDIR + DIR cij po kg ... jednaka za svaki PIP ovog RNM-a                     
                  rtr.Tkn_cij      = rnmFaktur_rec.K_NivVrj00 * rtr.R_kolOP                                    ; // INDIR trosak za ovaj PIP rtrans (theRNMstatus.ncPerUlKolKgOTP * rtr.R_kolOP)  
                                                                                                               
                  rtr.Rkn_KC       = rnmFaktur_rec.K_NivVrj00 * rtr.R_kolOP + rtr.R_KC                         ; // Ukupni trosak (INDIR + DIR) 
                  rtr.Rkn_rbt1     = ZXC.DivSafe(rtr.R_KC, 
                                                 rnmFaktur_rec.K_NivVrj00 * rtr.R_kolOP + rtr.R_KC) * 100M     ; // Udio   DIR u ukupnom trosku 
                  rtr.Rkn_CIJ_KCR  = ZXC.DivSafe(rnmFaktur_rec.K_NivVrj00 * rtr.R_kolOP,                                                             
                                                 rnmFaktur_rec.K_NivVrj00 * rtr.R_kolOP + rtr.R_KC) * 100M     ; // Udio INDIR u ukupnom trosku 
                  rtr.Rkn_KCR      = ZXC.DivSafe(rnmFaktur_rec.K_NivVrj00 * rtr.R_kolOP, rtr.T_kol)            ; // INDIR  trosak po komadu     
                  rtr.Rkn_CIJ_KCRP = ZXC.DivSafe(rnmFaktur_rec.K_NivVrj00 * rtr.R_kolOP, rtr.T_kol) + rtr.T_cij; // Ukupni trosak po komadu     
               });

            // sada svaki PIP unutar ovog RNM-a ima jednake sljedece podatke: 
            // 1. pipRtrans_rec.R_cijOP     - ncPerUlKolKg po PPR-ovima       
            // 2. pipRtrans_rec.TmpDecimal  - ncPerUlKolKg po OTP-u           
            // 3. pipRtrans_rec.TmpDecimal2 - ncPerUlKolKg PPR + OTP          

         } // if(isPipArtiklRekap == true) 

#endregion PIP Artikl Cijena 4.

      } // foreach(Faktur rnmFaktur_rec in TheFakturList)

      if(isRekapRNM)
      {
         TheRtransList = TheRtransList
            .OrderBy(r => r.R_grName                      )
          //.ThenBy (r => r.R_utilBool                    )
          //.ThenBy (r => r.T_dokNum                      ) // .. npr RNM-1711006 ima prvo napravljene PPR-ove pa tek onda RNM ... dokNum PPR-a je manji od dokNuma RNM-a! 
            .ThenBy (r => r.TtInfo.IsRNM_Realizacija_setTT) // RNM i RNu ce ovaj bool imati 'false' pa ce doci prije PPRa i PIPa
            .ThenBy (r => r.T_skladDate                   )
            .ThenBy (r => r.T_serial                      )
            .ToList();

         // set info bool for PPR-PIP Zaglavlje
         Rtrans currRtrans, prevRtrans;
         for(int i = 0; i < TheRtransList.Count; ++i)
         {
            if(i.IsZero()) continue;

            currRtrans = TheRtransList[i  ]; // for shorter syntax 
            prevRtrans = TheRtransList[i-1]; // for shorter syntax 

            if(currRtrans.TtInfo.IsRNM_Realizacija_setTT  && // PPR / PIP 
               prevRtrans.TtInfo.IsRNM_Plan_setTT          ) // RNM / RNU 
            {
               currRtrans.T_isIrmUsluga = true; // mark currentRtrans as 'new realizacija zaglavlje before this rtrans' 
            }

            if(currRtrans.T_TT == Faktur.TT_RNU) // PLAN 
            {
               currRtrans.TmpDecimal =
                  FakturDUC.GetDiffKol_PlanVsRealizacijaPIPR
                     (currRtrans.T_artiklCD, TheRtransList.Where(r => r.R_grName == currRtrans.R_grName && r.TtInfo.IsRNM_Realizacija_setTT).ToList(), currRtrans.T_kol);
            }
            else if(currRtrans.T_TT == Faktur.TT_PIP) // REALIZACIJA 
            {
               currRtrans.Tkn_cij = RNMncPerUlKolKgOTPList.Single(rnm => rnm.TheName == currRtrans.R_grName).TheDecimal * currRtrans.R_kolOP; // Iznos INDIR troska za ovaj PIP rtrans (theRNMstatus.ncPerUlKolKgOTP * currRtrans.R_kolOP)  
            }

         } // for(int i = 0; i < TheRtransList.Count; ++i)

         return TheFakturList.Count; // odi van, ne trebamo pipArtikl data 

      } // if(isRekapRNM)

#endregion 1. Get PPR_PIP RtransList 2. Get NC per ulaz KG Cijenu po svakom RNM-u 3. Get Aotp Cijenu ('A3') po svakom RNM-u 4. Set OTP cijenu svakom PIP-u

#region Create TheManyDecimalsList (Foreach PIP artikl) - VOILA!

      // TODO: !!! dilema; treba li ovo biti grupirano i po skladistu? 
    //var pipsGrouppedByArtiklCD = TheRtransList.Where(rtr => rtr.T_TT == Faktur.TT_PIP).GroupBy(rtr => rtr.T_artiklCD);
      TheRtransList.RemoveAll(rtr => rtr.T_TT != Faktur.TT_PIP);

      TheRtransList = TheRtransList
         .OrderBy(rtr => rtr.T_artiklCD )
         .ThenBy (rtr => rtr.T_skladDate)
         .ThenBy (rtr => rtr.T_ttNum    )
         .ThenBy (rtr => rtr.T_serial   ).ToList();

      var pipsGrouppedByArtiklCD = TheRtransList.GroupBy(rtr => rtr.T_artiklCD);

      IEnumerable<decimal> cijPoKomList;
      decimal maxDevijacija, avgValue, valueWithMaxOdstupanje;

      foreach(var pipRtransesGR in pipsGrouppedByArtiklCD)
      {
         pipArtiklIznosPPR_SUM     = pipRtransesGR.Sum(r =>  r.R_KC                           ); 
         pipArtiklIznosOTP_SUM     = pipRtransesGR.Sum(r => (r.R_kolOP * r.TmpDecimal ).Ron2()); 

       //pipArtiklIznosPPR_OTP_SUM = pipRtransesGR.Sum(r => (r.R_kolOP * r.TmpDecimal2).Ron2()); 
         pipArtiklIznosPPR_OTP_SUM = pipRtransesGR.Sum(r =>  r.R_KC                           )+
                                     pipRtransesGR.Sum(r => (r.R_kolOP * r.TmpDecimal ).Ron2()); 

         pipArtiklKolKgSUM         = pipRtransesGR.Sum(r =>  r.R_kolOP                        );
         pipArtiklKolSUM           = pipRtransesGR.Sum(r =>  r.R_kol                          ); 

         cijPoKomList              = pipRtransesGR.Select(r => r.T_cij                        );

         maxDevijacija             = ZXC.StopaNajvecegOdstupanjaOdProsjeka(cijPoKomList, /*pipRtransesGR.Key,*/ out avgValue, out valueWithMaxOdstupanje);

         TheManyDecimalsList.Add(new VvManyDecimalsReportSourceRow()
         {
            RowStringCD = pipRtransesGR.Key                    ,
            RowName     = pipRtransesGR.First().T_artiklName   ,
            TheStr      = pipRtransesGR.First().T_skladCD      ,
            Date_1      = pipRtransesGR.Min(r => r.T_skladDate),
            Date_2      = pipRtransesGR.Max(r => r.T_skladDate),

            DecimA01    = pipArtiklIznosPPR_SUM                                                   , // iznos sum by PPR                                   
            DecimA02    = pipArtiklIznosOTP_SUM                                                   , // iznos sum by OTP                                   
            DecimA03    = pipArtiklIznosPPR_OTP_SUM                                               , // iznos sum by PPR + OTP                             
                                                                                                                                                          
            DecimA04    = ZXC.DivSafe(pipArtiklIznosPPR_SUM    , pipArtiklKolKgSUM               ), // PrNabCij po kg by PPR                              
            DecimA05    = ZXC.DivSafe(pipArtiklIznosOTP_SUM    , pipArtiklKolKgSUM               ), // PrNabCij po kg by OTP                              
            DecimA06    = ZXC.DivSafe(pipArtiklIznosPPR_OTP_SUM, pipArtiklKolKgSUM               ), // PrNabCij po kg by OTP + PPR                        
                                                                                                                                                          
            DecimA07    = ZXC.DivSafe(pipArtiklIznosPPR_SUM    , pipArtiklKolSUM                 ), // PrNabCij po kom by PPR                             
            DecimA08    = ZXC.DivSafe(pipArtiklIznosOTP_SUM    , pipArtiklKolSUM                 ), // PrNabCij po kom by OTP                             
            DecimA09    = ZXC.DivSafe(pipArtiklIznosPPR_OTP_SUM, pipArtiklKolSUM                 ), // PrNabCij po kom by OTP + PPR : VOILA!              
                                                                                                                                                          
            DecimA16    = ZXC.DivSafe(pipArtiklIznosPPR_SUM    , pipArtiklIznosPPR_OTP_SUM) * 100M, // Udio direktnih   - PPR                             
            DecimA17    = ZXC.DivSafe(pipArtiklIznosOTP_SUM    , pipArtiklIznosPPR_OTP_SUM) * 100M, // Udio indirektnih - OTP                             

            DecimA18    = valueWithMaxOdstupanje                                                  , // Cijena max odstupanja                              
            DecimA19    = avgValue                                                                , // Srednja cijena - NIJE ponderirana prosjecna        
            DecimA20    = maxDevijacija                                                           , // StopaNajvecegOdstupanjaOdProsjeka CIJENE PO KOMADU 
            DecimA21    = pipRtransesGR.First().T_ppmvOsn                                         ,

            IsXxx       = maxDevijacija > ZXC.RRD.Dsc_KomProvizSt                                 , // Dsc_KomProvizSt je stopa tolerancije za warning 
            IsYyy       = ZXC.DivSafe(pipArtiklIznosPPR_SUM    , pipArtiklKolKgSUM).IsZero()      , // PrNabCij po kg by PPR is ZERO                   

         });
      }

#endregion Create TheManyDecimalsList (Foreach PIP artikl)

      return TheFakturList.Count;
   }

}

public class RptR_Rekap_RNZ_PoslJed : RptR_StandardRiskReport // Upisnik objekata 
{
   public RptR_Rekap_RNZ_PoslJed(string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)

      : base(new Vektor.Reports.RIZ.CR_Rekap_RNZ_Obj() as ReportDocument,
         _reportName         ,                                                    
         _rptFilter          , 
         _filterStyle        ,
         _rptNeeds_ArtWars   , // ArtiklWithArtstat 
         _rptNeeds_ArtStat   , // ArtStat        
         _rptNeeds_Faktur    , // Faktur         
         _rptNeeds_Rtrans    , // Rtrans         
         _rptNeeds_Kupdob    , // Kupdob         
         _rptNeeds_Prjkt     , // Prjkt          
         _rptNeeds_Rtrans4ruc, // Rtrans4ruc     
         _rptNeeds_Artikl    ) // Artikl         

   {

   }

   public override int FillRiskReportLists()
   {
      base.FillRiskReportLists();

      TheFakturLightList = new List<FakturLight>();

      foreach(var objektGR in TheFakturList.GroupBy(faktur => faktur.PosJedCD))
      {
         Faktur firstFaktur_rec = objektGR.First();

         uint distinctPersonCount = (uint)objektGR.Select(f => f.PersonCD).Distinct().Count();

         TheFakturLightList.Add(new FakturLight() {

            FakRecID   = objektGR.Key                ,
            KupdobName = firstFaktur_rec.PosJedName  ,
            SkladName  = firstFaktur_rec.PosJedAdresa,
            NacPlac    = firstFaktur_rec.Fco         ,
            VezniDok   = firstFaktur_rec.VezniDok2   ,
            TtNum      = distinctPersonCount         ,

         });
      }

      return TheFakturLightList.Count;
   }

}

public class RptR_Rekap_TH_DjelatRabat : RptR_RekapFaktur
{
   public RptR_Rekap_TH_DjelatRabat(string _fakturGR, ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl) :

      base(_fakturGR, /*_reportDocument,*/ _reportName, _rptFilter, _filterStyle, _rptNeeds_ArtWars, _rptNeeds_ArtStat, _rptNeeds_Faktur, _rptNeeds_Rtrans, _rptNeeds_Kupdob, _rptNeeds_Prjkt, _rptNeeds_Rtrans4ruc, _rptNeeds_Artikl)
   {
      this.reportDocument = _reportDocument;
      FakturGR = "KupdobName";
      RptFilter.GrupiranjeDokum = "KupdobName";

   }

   public override int FillRiskReportLists()
   {
      base.FillRiskReportLists();

      // Fill Faktur_rec_SumaRazdoblja_BLG, fill Faktur_rec_DonosPretRazd_BLG 

      /*Faktur_rec_DonosPretRazd_BLG = */

      //LoadBLAGSumaPrethodRazdob(TheDbConnection, true);

      return 0;
   }
   
}


public class RptR_SVD_FinIzlaz : RptR_StandardRiskReport
{
   public bool isColByAgr;
   public bool isColBySkl;
   public bool isANA;
   public bool isSIN;
   public bool isTOPlista;
   public bool isINVlista;
   public bool isNEW_4KNJ; // u 2020, za knjigovodstvo novi izvj: SINT po KLINIKAMA ali zasebne tablice po SKL 

   public bool Needs_SubReport_ByMonth;

   public string Skl50MaxPrice_Name;

   public RptR_SVD_FinIzlaz(bool _isColByAgr, bool _isANA, ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)

      : base(
         //(new Vektor.Reports.RIZ.CR_SVD_FinIzlaz() as ReportDocument),
         //(_isColByAgr ? (new Vektor.Reports.RIZ.CR_SVD_FinIzlaz() as ReportDocument) : (new Vektor.Reports.RIZ.CR_SVD_FinIzlazGrSkl() as ReportDocument)),
         _reportDocument     ,
         _reportName         ,                                                    
         _rptFilter          , 
         _filterStyle        ,
         _rptNeeds_ArtWars   , // ArtiklWithArtstat 
         _rptNeeds_ArtStat   , // ArtStat        
         _rptNeeds_Faktur    , // Faktur         
         _rptNeeds_Rtrans    , // Rtrans         
         _rptNeeds_Kupdob    , // Kupdob         
         _rptNeeds_Prjkt     , // Prjkt          
         _rptNeeds_Rtrans4ruc, // Rtrans4ruc     
         _rptNeeds_Artikl    ) // Artikl         

   {
      this.reportDocument = _reportDocument;

      this.isANA =  _isANA;
      this.isSIN = !_isANA;

      this.isColByAgr = _isColByAgr;
      this.isColBySkl = !isColByAgr;

      this.isTOPlista = reportDocument is Vektor.Reports.RIZ.CR_SVD_TopListPart           ;
      this.isINVlista = reportDocument is Vektor.Reports.RIZ.CR_SVD_Artikli_KlinikaInv_Exp;

      Needs_SubReport_ByMonth = RptFilter.DatumOd.Month != RptFilter.DatumDo.Month;

      this.isNEW_4KNJ = reportDocument is Vektor.Reports.RIZ.CR_SVD_FinIzlaz_4Knj;
}

   public override int FillRiskReportLists()
   {
      // TheRtransList
      base.FillRiskReportLists();

      if(isINVlista) return Fill_INVlista_ReportList();

      // TheKupdobList
    //TheKupdobList =              VvUserControl.KupdobSifrar.Join(TheRtransList, kpdb => kpdb.KupdobCD, rtrans => rtrans.T_kupdobCD, (kpdb, rtrans) => kpdb).Distinct().ToList();
      TheKupdobList = isTOPlista ? VvUserControl.KupdobSifrar.Join(TheRtransList, kpdb => kpdb.KupdobCD, rtrans => rtrans.T_kupdobCD, (kpdb, rtrans) => kpdb).Distinct().ToList() :
                                   VvUserControl.KupdobSifrar.Where(kpdb => kpdb.IsMtr).ToList();

      // Add dummy rtranses, da bi svaki odjel izasao u izvj, iako nema prometa, a zbog limita 
      // ... osim ako ne potrazujes samo za jedan odjel ili kliniku                            
      List<Kupdob> filteredKupdobList = TheKupdobList/*.ToList()*/;

      if(RptFilter.KD_sifra.NotZero())
      {
         if(RptFilter.KD_naziv.Length == 1) // KLINIKA / ZAVOD ONLY 
         {
            filteredKupdobList = TheKupdobList.Where(kpdb => kpdb.IsMtr && kpdb.Ulica1 == RptFilter.KD_naziv).ToList();
         }
         else // real partner (kup / dob) ONLY 
         {
            filteredKupdobList = TheKupdobList.Where(kpdb => kpdb.KupdobCD == RptFilter.KD_sifra).ToList();
         }
      }

      // 09.09.2022: dodan if() jer ovi dummy rtransovi smataju kada npr. filtriras po skladistu 
      if(!isNEW_4KNJ)
      filteredKupdobList.Where(kpdb => kpdb.FinLimit.NotZero()).ToList()
         .ForEach(kpdb => TheRtransList.Add( new Rtrans()
         { T_kupdobCD = kpdb.KupdobCD, T_skladCD = "10", A_ArtGrCd1 = "10"/*T_artiklName = "SVD_dummy"*/ } ));

      // 17.10.2022:  skladista: sva / samo DON / samo NOT DON 
      if(isNEW_4KNJ)
      {
         if(RptFilter.SVD_IsDonacSklad == ZXC.JeliJeTakav.NIJE_TAKAV) // todo: smo NOT DON 
         {
            TheRtransList.RemoveAll(rtr => ZXC.IsSvDUH_donSkl(rtr.T_skladCD) == true);
         }
         if(RptFilter.SVD_IsDonacSklad == ZXC.JeliJeTakav.JE_TAKAV) // todo: smo DON 
         {
            TheRtransList.RemoveAll(rtr => ZXC.IsSvDUH_donSkl(rtr.T_skladCD) == false);
         }
      }

      // 04.02.2020: 
      if(isNEW_4KNJ)
      {
         TheRtransList.ForEach(rtr => rtr.R_grName = rtr.T_skladCD + "@" + rtr.T_kupdobCD);
      }

      // TheSVD_RptLineList 
      SetSVD_RptLineList();

      // TheSVD_SubRptLineList 
      SetSVD_SubRptLineList();

      var subRptLine = TheSVD_SubRptLineList.FirstOrDefault(srl => srl.Skl50MaxPrice_Name.NotEmpty());
      Skl50MaxPrice_Name = subRptLine != null ? subRptLine.Skl50MaxPrice_Name : "";

      #region Razrada Rekap Po Skladistima - dodatno po mjesecima 

      TheSVD_SubRptLineList_Mon = new List<SVD_SubRptLine_Mon>(); // !!! 

      int startMonth = RptFilter.DatumOd.Month;
      int endMonth   = RptFilter.DatumDo.Month;

      for(int mon = startMonth; mon <= endMonth; ++mon)
      {
         // TheSVD_SubRptLineList_Mon 
         SetSVD_SubRptLineList_Monthly(mon);
      }

      #endregion Razrada Rekap Po Skladistima - dodatno po mjesecima 

      #region Promet Artikla za tt IZD za skl 50

      TheDeviznaSumaList =

         TheRtransList.Where(rtr => rtr.T_skladCD == "50")
         .GroupBy(R => R.T_artiklCD)
         .Select(grp => new VvReportSourceUtil
            (/* DevName   */ grp.First().T_artiklName,
             /* TheCD     */ grp.Key,
             /* Count     */ grp.Count(),
             /* Kol       */ grp.Sum(R => R.R_kol),
             /* TheMoney  */ grp.Sum(R => R.R_KCR),
             /* TheMoney2 */ grp.Sum(R => R.R_Kol_Puta_PrNabCij),
             /* TheSaldo  */ grp.Sum(R => R.R_Ira_RUV),
             /* IsNekakav */ grp.Any(R => R.T_skladCD == "50") //_SVDsamoLijekovi_23_11_2021_zaUpravnoVijece
            ))
         .OrderByDescending(R => R.TheMoney)
         .ToList();

      #endregion Promet Artikla za tt IZD za skl 50

      return TheRtransList.Count;
   }

   private int Fill_INVlista_ReportList()
   {
      uint wantedKlinikaCD = RptFilter.KD_sifra;
      if(wantedKlinikaCD.IsZero()) return 0;

      Kupdob klinikaKupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(k => k.KupdobCD == wantedKlinikaCD);
      if(klinikaKupdob_rec == null) return 0;

      //TheDeviznaSumaList = new List<VvReportSourceUtil>();

      TheDeviznaSumaList = TheRtransList.GroupBy(rtr => rtr.T_artiklCD).Select(gr => new VvReportSourceUtil()
      {
         ArtiklGrCD   = gr.Key,
         ArtiklGrName = gr.First().T_artiklName,
         DevName      = Artikl.IsSvdArtGR_Ljek(gr.First().A_ArtGrCd1) ? "LIJEKOVI" : "POTROŠNI MEDICINSKI MATERIJALI",
       //Kol          = rtr.T_kol,
         TheMoney     = ZXC.DivSafe(gr.Sum(g => g.R_KCRP), gr.Sum(g => g.R_kol)),
       //TheMoney2    = rtr.R_KCRP,
         TheCD        = klinikaKupdob_rec.Naziv,
         KupdobName   = klinikaKupdob_rec.Ulica1
      }).OrderBy(qwe => qwe.DevName).ThenBy(qwe => qwe.ArtiklGrName).ToList();

      return TheDeviznaSumaList.Count;
   }

   private /*List<SVD_RptLine>*/ void SetSVD_RptLineList()
   {
      // 04.02.2010: 
    //var rtransListGroupedByKCD_or_SCDandKCD =              TheRtransList.GroupBy(rtr => rtr.T_kupdobCD           ) ;
      var rtransListGroupedByKCD_or_SCDandKCD = isNEW_4KNJ ? TheRtransList.GroupBy(rtr => rtr.R_grName             ) : // isNEW_4KNJ   
                                                             TheRtransList.GroupBy(rtr => rtr.T_kupdobCD.ToString()) ; // old - classic

      TheSVD_RptLineList = new List<SVD_RptLine>();

      if(rtransListGroupedByKCD_or_SCDandKCD.Count().IsZero()) return /*TheSVD_RptLineList*/;

      Kupdob kupdob_rec;

      //decimal totalReportSUM_LJ = TheRtransList.Where(r => 
      //                                                      r.A_ArtGrCd1 == "90" ||
      //                                                      r.A_ArtGrCd1 == "A0" ||
      //                                                      r.A_ArtGrCd1 == "N0" ||
      //                                                      r.A_ArtGrCd1 == "10"  ).Sum(r => r.R_KCRP);
      //decimal totalReportSUM_PM = TheRtransList.Where(r =>
      //                                                      r.A_ArtGrCd1 == "00" ||
      //                                                      r.A_ArtGrCd1 == "20" ||
      //                                                      r.A_ArtGrCd1 == "30" ||
      //                                                      r.A_ArtGrCd1 == "40" ||
      //                                                      r.A_ArtGrCd1 == "50" ||
      //                                                      r.A_ArtGrCd1 == "60" ||
      //                                                      r.A_ArtGrCd1 == "70" ||
      //                                                      r.A_ArtGrCd1 == "80").Sum(r => r.R_KCRP);

      decimal totalReportSUM_LJ = TheRtransList.Where(r => Artikl.IsSvdArtGR_Ljek(r.A_ArtGrCd1)).Sum(r => r.R_KCRP);
      decimal totalReportSUM_PM = TheRtransList.Where(r => Artikl.IsSvdArtGR_Potr(r.A_ArtGrCd1)).Sum(r => r.R_KCRP);

      uint theKCD;

      foreach(var rtrGR in rtransListGroupedByKCD_or_SCDandKCD)
      {
         // 04.02.2020: 
       //theKCD = rtrGR.Key               ;
         theKCD = rtrGR.First().T_kupdobCD;

         kupdob_rec = TheKupdobList.SingleOrDefault(k => k.KupdobCD == theKCD);

         if(theKCD.IsZero())
         {
            List<uint> badTtNums = rtrGR.Select(g => g.T_ttNum).Distinct().ToList();
            string errMsg = "";
            string theTT = rtrGR.First().T_TT;

            foreach(uint badTtNum in badTtNums)
            {
               errMsg += theTT + "-" + badTtNum + "\n";
            }

            ZXC.aim_emsg(MessageBoxIcon.Error, "Dokumenti sa nedefiniranim partnerom:\n\n{0}", errMsg);
            kupdob_rec = new Kupdob() { Naziv = "NEDEFINIRANI PARTNER"};
         }

         // 04.05.2020: 
         //TheSVD_RptLineList.Add(new SVD_RptLine(TheDbConnection, rtrGR.ToList(), kupdob_rec, this.isColByAgr, this.isTOPlista, this.isNEW_4KNJ, RptFilter.DatumOd, RptFilter.DatumDo, RptFilter.IsChk0, totalReportSUM_LJ, totalReportSUM_PM                   ));
           TheSVD_RptLineList.Add(new SVD_RptLine(TheDbConnection, rtrGR.ToList(), kupdob_rec, this.isColByAgr, this.isTOPlista, this.isNEW_4KNJ, RptFilter.DatumOd, RptFilter.DatumDo, RptFilter.IsChk0, totalReportSUM_LJ, totalReportSUM_PM, RptFilter.SkladCD));
      }

      // 04.02.2020: 
      //TheSVD_RptLineList = TheSVD_RptLineList.OrderBy(rptl => rptl.LineCD).ToList();
      if(isNEW_4KNJ)
      {
         TheSVD_RptLineList = TheSVD_RptLineList.OrderBy(rptl => rptl.GrupCD).ThenBy(rptl => rptl.LineCD).ToList();
      }
      else // old, classic 
      {
         TheSVD_RptLineList = TheSVD_RptLineList.OrderBy(rptl => rptl.LineCD).ToList();
      }

      return /*TheSVD_RptLineList*/;
   }

   private /*List<SVD_RptLine>*/ void SetSVD_SubRptLineList()
   {
      var rtransListGroupedBy_Skl_Or_Agr = isColByAgr  ? TheRtransList.GroupBy(rtr => rtr.T_skladCD ) :
                                        /* isColBySkl */ TheRtransList.GroupBy(rtr => rtr.A_ArtGrCd1) ;

      TheSVD_SubRptLineList = new List<SVD_SubRptLine>();

      if(rtransListGroupedBy_Skl_Or_Agr.Count().IsZero()) return /*TheSVD_RptLineList*/;

      VvLookUpItem skl_or_agr_lui;

      foreach(var rtrGR in rtransListGroupedBy_Skl_Or_Agr)
      {
         skl_or_agr_lui = isColByAgr  ? ZXC.luiListaSkladista    .GetLuiForThisCd(rtrGR.Key) :
                       /* isColBySkl */ ZXC.luiListaGrupa1Artikla.GetLuiForThisCd(rtrGR.Key) ;

         TheSVD_SubRptLineList.Add(new SVD_SubRptLine(rtrGR.ToList(), skl_or_agr_lui, this.isColByAgr));
      }

      TheSVD_SubRptLineList = TheSVD_SubRptLineList.OrderBy(srptl => srptl.LineCD).ToList();

      return /*TheSVD_SubRptLineList*/;
   }

   private /*List<SVD_RptLine>*/ void SetSVD_SubRptLineList_Monthly(int _month)
   {
      var rtransListGroupedBy_Skl_Or_Agr = isColByAgr  ? TheRtransList.Where(rtr => rtr.T_skladDate.Month == _month).GroupBy(rtr => rtr.T_skladCD ) :
                                        /* isColBySkl */ TheRtransList.Where(rtr => rtr.T_skladDate.Month == _month).GroupBy(rtr => rtr.A_ArtGrCd1) ;

      // TheSVD_SubRptLineList_Mon = new List<SVD_SubRptLine>(); !!! 

      if(rtransListGroupedBy_Skl_Or_Agr.Count().IsZero()) return /*TheSVD_RptLineList*/;

      VvLookUpItem skl_or_agr_lui;

      foreach(var rtrGR in rtransListGroupedBy_Skl_Or_Agr)
      {
         skl_or_agr_lui = isColByAgr  ? ZXC.luiListaSkladista    .GetLuiForThisCd(rtrGR.Key) :
                       /* isColBySkl */ ZXC.luiListaGrupa1Artikla.GetLuiForThisCd(rtrGR.Key) ;

         TheSVD_SubRptLineList_Mon.Add(new SVD_SubRptLine_Mon(rtrGR.ToList(), skl_or_agr_lui, this.isColByAgr) { Month = _month} );
      }

      TheSVD_SubRptLineList_Mon = TheSVD_SubRptLineList_Mon.OrderBy(srptl => srptl.LineCD).ToList();

      return /*TheSVD_SubRptLineList*/;
   }

}

public class RptR_Rekap_SVD_PlanRealizUGO : RptR_StandardRiskReport/*RptR_RekapFaktur*/
{
   public RptR_Rekap_SVD_PlanRealizUGO(/*string _fakturGR,*/ ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)

      : base(_reportDocument ,
         _reportName         ,                                                    
         _rptFilter          , 
         _filterStyle        ,
         _rptNeeds_ArtWars   , // ArtiklWithArtstat 
         _rptNeeds_ArtStat   , // ArtStat        
         _rptNeeds_Faktur    , // Faktur         
         _rptNeeds_Rtrans    , // Rtrans         
         _rptNeeds_Kupdob    , // Kupdob         
         _rptNeeds_Prjkt     , // Prjkt          
         _rptNeeds_Rtrans4ruc, // Rtrans4ruc     
         _rptNeeds_Artikl    ) // Artikl         

   {
      EnableDrillDown = true;
   }

   public override int FillRiskReportLists()
   {
      base.FillRiskReportLists(); // // Ovo puni UGO TheFakturList, UGO TheRtransList

      GroupJoinFakturWithRtrans();

#region local vars

      decimal  this_UGO_rtrans_ostvarenoOGbyART_KCRP;
      decimal  this_UGO_rtrans_ostvarenoOGbyART_kol ;
      DateTime this_UGO_rtrans_datZadByART          ;

      decimal  this_UGO_faktur_ostvarenoOGbyART_KCRP;
      decimal  this_UGO_faktur_ostvarenoOGbyART_kol ;
      DateTime this_UGO_faktur_datZadByART          ;

      List<Rtrans> URArtransList_For_UGO_Artikl;

      TheFakturLightList = new List<FakturLight>();

#endregion local vars

      foreach(Faktur UGOfaktur in TheFakturList)
      {
#region nullify faktur sum

         this_UGO_faktur_ostvarenoOGbyART_KCRP =                0M;
         this_UGO_faktur_ostvarenoOGbyART_kol  =                0M;
         this_UGO_faktur_datZadByART           = DateTime.MinValue;

#endregion nullify local vars

         if(UGOfaktur.Transes.IsEmpty()) // Ugovor nema stavaka 
         {
            SetFakturLight(UGOfaktur, null, null);

            continue; // !!!
         }

         foreach(Rtrans UGOrtrans in UGOfaktur.Transes)
         {
#region nullify local vars

            this_UGO_rtrans_ostvarenoOGbyART_KCRP =                0M;
            this_UGO_rtrans_ostvarenoOGbyART_kol  =                0M;
            this_UGO_rtrans_datZadByART           = DateTime.MinValue;

#endregion nullify local vars

#region Get this_UGO_Artikl KCRPsum, kolSum, zadDate

            URArtransList_For_UGO_Artikl = RtransDao.Get_RtransList_For_TT_Artikl_Kupdob_Dates(TheDbConnection, Faktur.TT_URA,
               UGOrtrans.T_artiklCD   , // artiklCD 
               UGOrtrans.T_kupdobCD   , // kupdobCD 
             //ZXC.projectYearFirstDay, // dateOD   
               UGOfaktur.DokDate      , // dateOD   ... prosla i ova godina! 
               UGOfaktur.DospDate    ); // dateDO   !? 

            if(URArtransList_For_UGO_Artikl.IsEmpty()) // Ugovor nema nijedan URA rtrans 
            {
               SetFakturLight(UGOfaktur, UGOrtrans, null);

               continue; // !!!
            }

            // Rtrans sum: 
            this_UGO_rtrans_ostvarenoOGbyART_kol  = URArtransList_For_UGO_Artikl.Sum(rtr => rtr.R_kol      );
            this_UGO_rtrans_ostvarenoOGbyART_KCRP = URArtransList_For_UGO_Artikl.Sum(rtr => rtr.R_KCRP     );
            this_UGO_rtrans_datZadByART           = URArtransList_For_UGO_Artikl.Max(rtr => rtr.T_skladDate);

            // Faktur sum: 
            this_UGO_faktur_ostvarenoOGbyART_kol  += this_UGO_rtrans_ostvarenoOGbyART_kol ;
            this_UGO_faktur_ostvarenoOGbyART_KCRP += this_UGO_rtrans_ostvarenoOGbyART_KCRP;
            if(this_UGO_rtrans_datZadByART > this_UGO_faktur_datZadByART) this_UGO_faktur_datZadByART = this_UGO_rtrans_datZadByART;

            foreach(Rtrans URArtrans in URArtransList_For_UGO_Artikl)
            {
               SetFakturLight(UGOfaktur, UGOrtrans, URArtrans); // Voila! 

            } // foreach(Rtrans URArtrans in URArtransList_For_UGO_Artikl) 

#endregion Get this_UGO_Artikl KCRPsum, kolSum, zadDate

         } // foreach(Rtrans UGOrtrans in UGOfaktur.Transes) 

         var thisUGO_fakturLightList = TheFakturLightList.Where(fl => fl.TtNum == UGOfaktur.TtNum);

         foreach(FakturLight fakturLight_rec in thisUGO_fakturLightList)
         {
          //fakturLight_rec.Decim15  = thisUGO_fakturLightList.Sum(fl => fl.Decim09)                                   ; // Ostvareno           - po Ugovoru 
          //fakturLight_rec.Decim16  = fakturLight_rec.Decim14 - fakturLight_rec.Decim15                               ; // Ostatak             - po Ugovoru 
          //fakturLight_rec.Decim17  = ZXC.StopaRealizacije (fakturLight_rec.Decim14, fakturLight_rec.Decim15)         ; // Indeks ostvarenosti - po Ugovoru 
          //fakturLight_rec.DokDate4 = this_UGO_faktur_datZadByART                                                     ; // 'Konacni datum izvrsenja' Ugovora 
                                                                                                                       
            fakturLight_rec.Decim15  = this_UGO_faktur_ostvarenoOGbyART_KCRP                                           ; // Ostvareno           - po Ugovoru 
            fakturLight_rec.Decim16  = UGOfaktur.S_ukKCRP - this_UGO_faktur_ostvarenoOGbyART_KCRP                      ; // Ostatak             - po Ugovoru 
            fakturLight_rec.Decim17  = ZXC.StopaRealizacije (UGOfaktur.S_ukKCRP, this_UGO_faktur_ostvarenoOGbyART_KCRP); // Indeks ostvarenosti - po Ugovoru 
            fakturLight_rec.DokDate4 = this_UGO_faktur_datZadByART                                                     ; // 'Konacni datum izvrsenja' Ugovora 
         }

      } // foreach(Faktur UGOfaktur in TheFakturList) 

      //var fakturLightList_GroupedByUGO = TheFakturLightList.GroupBy(fl => fl.TtNum);
      //foreach(List<FakturLight> thisUGO_flList in fakturLightList_GroupedByUGO)
      //{
      //   foreach(FakturLight fakturLight_rec in thisUGO_flList)
      //   {
      //      fakturLight_rec.Decim15 = thisUGO_flList.Sum(fl => fl.Decim09)                                   ; // Ostvareno           - po Ugovoru 
      //      fakturLight_rec.Decim16 = fakturLight_rec.Decim14 - fakturLight_rec.Decim15                      ; // Ostatak             - po Ugovoru 
      //      fakturLight_rec.Decim17 = ZXC.StopaRealizacije (fakturLight_rec.Decim14, fakturLight_rec.Decim15); // Indeks ostvarenosti - po Ugovoru 
      //   }
      //}

      return 0;
   }

   private FakturLight SetFakturLight(Faktur UGOfaktur, Rtrans UGOrtrans, Rtrans URArtrans)
   {
      FakturLight fakturLight_rec = new FakturLight();

      fakturLight_rec.TtNum      = UGOfaktur.TtNum     ;
      fakturLight_rec.KupdobCD   = UGOfaktur.KupdobCD  ;
      fakturLight_rec.KupdobName = UGOfaktur.KupdobName;
      fakturLight_rec.VezniDok   = UGOfaktur.VezniDok  ;
      fakturLight_rec.DokDate    = UGOfaktur.DokDate   ;
      fakturLight_rec.DokDate2   = UGOfaktur.DospDate  ;
      fakturLight_rec.SkladName2 = UGOfaktur.Napomena  ;

      fakturLight_rec.Decim13    = UGOfaktur.S_ukKCRM ;
      fakturLight_rec.Decim14    = UGOfaktur.S_ukKCRP ;

      if(UGOrtrans != null)
      {
         fakturLight_rec.SkladCD   = UGOrtrans.T_artiklCD  ;
         fakturLight_rec.SkladName = UGOrtrans.T_artiklName;
         fakturLight_rec.Decim01   = UGOrtrans.T_kol       ;
         fakturLight_rec.Decim02   = UGOrtrans.T_cij       ;
         fakturLight_rec.Decim03   = UGOrtrans.T_pdvSt     ;
         fakturLight_rec.Decim12   = UGOrtrans.R_KCRM      ; // iznos bez pdv-a
         fakturLight_rec.Decim04   = UGOrtrans.R_KCRP      ; // iznos sa pdv-om
      }

      if(URArtrans != null)
      {
         fakturLight_rec.TtNum_2  = URArtrans.T_ttNum    ;
         fakturLight_rec.DokDate3 = URArtrans.T_skladDate;
         fakturLight_rec.Decim05  = URArtrans.T_kol      ;
         fakturLight_rec.Decim06  = URArtrans.T_cij      ;
         fakturLight_rec.Decim07  = URArtrans.T_rbt1St   ;
         fakturLight_rec.Decim08  = URArtrans.T_pdvSt    ;
         fakturLight_rec.Decim09  = URArtrans.R_KCRP     ;
      }

      TheFakturLightList.Add(fakturLight_rec);

      return fakturLight_rec;
   }

#if OldWay_RptR_Rekap_SVD_PlanRealizUGO

   public override int FillRiskReportLists()
   {
      base.FillRiskReportLists(); // Ovo puni URA TheFakturList 

#region Get UGO FakturList

      VvSqlFilterMember[] fmArray = RptFilter.FilterMembers.ToArray();

      int ttFM_idx = RptFilter.FilterMembers.FindIndex(fm => fm.name == "TT");

      fmArray[0       ].value = DateTime.MinValue;
      fmArray[ttFM_idx].value = Faktur.TT_UGO    ;

      TheFakturBBBList = new List<Faktur>(); // UGO FakturList

      VvDaoBase.LoadGenericVvDataRecordList(TheDbConnection, TheFakturBBBList, fmArray.ToList(), "", FakturOrderBy, true);

#endregion Get UGO FakturList

      FakturLightByTtAndTtNumComparer fakturLightByTtAndTtNumComparer = new FakturLightByTtAndTtNumComparer();

      TheFakturLightList =
         TheFakturList                        // Source Collection          
         .FullOuterJoinJoin
         (TheFakturBBBList                  , // Inner  Collection          
            ura => ura.ProjektCD            , // PK                         
            ugo => ugo.TT_And_TtNum         , // FK                         
            fakturLightByTtAndTtNumComparer , // comparer: Qukatz additions 
            (ura, ugo) =>                     // Result Collection          
         new FakturLight
         {
          //FakturGR = GetFakGRdata(FakturGR, ura),
            TT         = ura == null ? ""                : ura.TT        , // URA 
            TtNum      = ura == null ? 0                 : ura.TtNum     , // URA 
            DokDate    = ura == null ? DateTime.MinValue : ura.DokDate   , // URA 
            VezniDok   = ura == null ? ""                : ura.VezniDok  , // URA 
            KupdobCD   = ura == null ? ugo.KupdobCD      : ura.KupdobCD  , // URA 
            KupdobName = ura == null ? ugo.KupdobName    : ura.KupdobName, // URA 
            SkladCD    = ura == null ? ""                : ura.SkladCD   , // URA 
            Decim01    = ura == null ? 0M                : ura.S_ukKCRP  , // URA 
            SkladCD2   = ura == null ? ""                : ura.PdvNum.NotZero() ? ura.PdvNum.ToString() : ura.GetFirstVezaTtNumForTT(Faktur.TT_NRD).ToString(), // URA brNarudzbe 

            NacPlac    = ugo == null ? " - "             : ugo.VezniDok , // UGO ekst. Br. Ugovora               - po svakoj URA-i 
            FakRecID   = ugo == null ? ura.KupdobCD      : ugo.TtNum    , // UGO ugo.TtNum (URA-e bez ProjektCD) - po svakoj URA-i 
            DokDate2   = ugo == null ? DateTime.MinValue : ugo.DokDate  , // UGO ugo.DokDate                     - po svakoj URA-i 
            DokDate3   = ugo == null ? DateTime.MaxValue : ugo.DospDate , // UGO ugo.DospDate                    - po svakoj URA-i 
            Decim02    = ugo == null ? 0M                : ugo.SomeMoney, // UGO Ostvarenu u proslim godinama    - po svakoj URA-i 
            Decim03    = ugo == null ? 0M                : ugo.S_ukKCRP , // UGO ug uk sa  PDV-om                - po svakoj URA-i 
            Decim04    = ugo == null ? 0M                : ugo.S_ukKCRM , // UGO ug uk bez PDV-a                 - po svakoj URA-i 

            TT_2       = ugo == null ? ""                : ugo.TT       , // UGO if exists, UGO TT               - po svakoj URA-i 
            TtNum_2    = ugo == null ? 0                 : ugo.TtNum    , // UGO if exists, UGO TtNum            - po svakoj URA-i 

         }).OrderBy(fl => fl.KupdobName).ThenBy(fl => fl.TT).ThenBy(fl => fl.DokDate).ThenBy(fl => fl.TtNum).ToList();

      foreach(var fakturLightlist in TheFakturLightList.GroupBy(ura => ura.FakRecID)) // FakRecID je UGO ugo.TtNum 
      {
         foreach(FakturLight fakturLight in fakturLightlist)
         {
            fakturLight.DokDate4 = fakturLightlist.Max(fl => fl.DokDate)                         ; // Konačni Datum izvršenja - po ugovoru 
            fakturLight.Decim05  = fakturLightlist.Sum(fl => fl.Decim01)                         ; // Ostvareno u ovoj godini - po ugovoru 
            fakturLight.Decim06  = fakturLight.Decim02 + fakturLight.Decim05                     ; // Ostvareno sveukupno     - po ugovoru 
            fakturLight.Decim07  = fakturLight.Decim03 - fakturLight.Decim06                     ; // Ostatak                 - po ugovoru 
            fakturLight.Decim08  = ZXC.StopaRealizacije(fakturLight.Decim03, fakturLight.Decim06); // Indeks ostvarenosti     - po ugovoru 
         }
      }

      foreach(var fakturLightlist in TheFakturLightList.GroupBy(fl => fl.KupdobCD))
      {
         var thisKupdobUGOfakturList = TheFakturBBBList.Where(fak => fak.KupdobCD == fakturLightlist.Key);

         foreach(FakturLight fakturLight in fakturLightlist)
         {
            fakturLight.Decim09 = thisKupdobUGOfakturList.Sum(ugo => ugo.SomeMoney              ); // UGO Ostvarenu u proslim godinama - po dobavljacu 
            fakturLight.Decim10 = thisKupdobUGOfakturList.Sum(ugo => ugo.S_ukKCRP               ); // UGO ug uk sa  PDV-om             - po dobavljacu 
            fakturLight.Decim11 = thisKupdobUGOfakturList.Sum(ugo => ugo.S_ukKCRM               ); // UGO ug uk bez PDV-a              - po dobavljacu 
                                                                                        
            fakturLight.Decim12 = fakturLightlist.Sum(fl => fl.Decim01                          ); // Ostvareno u ovoj godini          - po dobavljacu 
            fakturLight.Decim13 = fakturLight.Decim09 + fakturLight.Decim12                      ; // Ostvareno sveukupno              - po dobavljacu 
            fakturLight.Decim14 = fakturLight.Decim10 - fakturLight.Decim13                      ; // Ostatak                          - po dobavljacu 
            fakturLight.Decim15 = ZXC.StopaRealizacije (fakturLight.Decim10, fakturLight.Decim13); // Indeks ostvarenosti              - po dobavljacu 
         }
      }

      if(RptFilter.IsBlgInIzvVal == false /*true*/) // da li je izvj. za svakog URA dobavljaca ili za UGO dobavljaca ... LEFT ili RIGHT JOIN 
      {                                             // default je UGO LEFT JOIN (Za svaki UGO vidi ima li i URA-e)                           
         TheFakturLightList.RemoveAll(fl => fl.TtNum_2.IsZero());
      }

      return 0;
   }
#endif
   public class FakturLightByTtAndTtNumComparer : IEqualityComparer<FakturLight>
   {
      // Ptranss are equal if their names and Ptrans numbers are equal.
      public bool Equals(FakturLight x, FakturLight y)
      {
   
         //Check whether the compared objects reference the same data.
         if(Object.ReferenceEquals(x, y)) return true;
   
         //Check whether any of the compared objects is null.
         if(Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
            return false;
   
         //Check whether the Ptrans' properties are equal.
         return 
               x.TT      == y.TT    && 
               x.TtNum   == y.TtNum &&
               x.TT_2    == y.TT_2  && 
               x.TtNum_2 == y.TtNum_2;
      }
   
      // If Equals() returns true for a pair of objects 
      // then GetHashCode() must return the same value for these objects.
   
      public int GetHashCode(FakturLight fakturLight)
      {
         if(Object.ReferenceEquals(fakturLight, null)) return 0;
   
         int hashChktt        = fakturLight/*.TT   */     == null ? 0 : fakturLight.TT     .GetHashCode();
         int hashChkttNum     = fakturLight/*.TtNum*/     == null ? 0 : fakturLight.TtNum  .GetHashCode();
         int hashChktt2       = fakturLight/*.TT   */     == null ? 0 : fakturLight.TT_2   .GetHashCode();
         int hashChkttNum2    = fakturLight/*.TtNum*/     == null ? 0 : fakturLight.TtNum_2.GetHashCode();
   
   
         //Calculate the hash code for the TtAndTtNum.
         return 
            hashChktt    ^
            hashChkttNum ^
            hashChktt2   ^
            hashChkttNum2;
      }
   
   }

}

public class RptR_SVD_URA4Knjigovodstvo : RptR_RekapFaktur
{
   public RptR_SVD_URA4Knjigovodstvo(string _fakturGR, ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)
                                    
      : base(_fakturGR, /*_reportDocument,*/ _reportName, _rptFilter, _filterStyle, _rptNeeds_ArtWars, _rptNeeds_ArtStat, _rptNeeds_Faktur, _rptNeeds_Rtrans, _rptNeeds_Kupdob, _rptNeeds_Prjkt, _rptNeeds_Rtrans4ruc, _rptNeeds_Artikl)
   {
      this.reportDocument = _reportDocument;
   }

   public override int FillRiskReportLists()
   {
      base.FillRiskReportLists();

      TheFakturLightList = TheFakturLightList.OrderBy(fak => fak.SVD_Knjigovod_TtNum).ToList();

      return 0;
   }

}

public class RptR_Rekap_MER_STATUS : RptR_RekapFaktur
{
   public RptR_Rekap_MER_STATUS(string _fakturGR, ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)
                                    
      : base(_fakturGR, /*_reportDocument,*/ _reportName, _rptFilter, _filterStyle, _rptNeeds_ArtWars, _rptNeeds_ArtStat, _rptNeeds_Faktur, _rptNeeds_Rtrans, _rptNeeds_Kupdob, _rptNeeds_Prjkt, _rptNeeds_Rtrans4ruc, _rptNeeds_Artikl)
   {
      this.reportDocument = _reportDocument;
   }

   public override int FillRiskReportLists()
   {
      base.FillRiskReportLists();

      //TheFakturList.RemoveAll(faktur => faktur.IsFiskalDutyFaktur_ONLINE == false);
      //
      //// 16.01.2015: 
      //TheFakturList.RemoveAll(faktur => faktur.FiskJIR.NotEmpty());

      //TheFakturList.RemoveAll(faktur => faktur.IsFiskalDutyFaktur_ONLINE == false || faktur.FiskJIR.NotEmpty());

      List<VvMER_Json_Status_Data> vvMER_Json_StatusList_Data = null;

      bool getStatusOK = true;
      try
      {
         vvMER_Json_StatusList_Data = Vv_Http_Web_request.Get_eRacun_STATUS_List_MER_WebService(RptFilter.DatumOd, RptFilter.DatumDo);
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom slanja na WebServis: {0}", ex.Message);
         getStatusOK = false;
      }

      if(getStatusOK)
      {
         VvMER_Json_Status_Data status;

         foreach(FakturLight fakturLight in TheFakturLightList)
         {
            status = vvMER_Json_StatusList_Data.SingleOrDefault(stat => stat.ElectronicId == fakturLight.MER_ElectronicID);

            if(status != null)
            {
               fakturLight.MER_ElectronicID = fakturLight.MER_ElectronicID; 
               fakturLight.FiskPrgBr        = fakturLight.FiskPrgBr       ; 
               fakturLight.SkladName2       = status.StatusName           ;
           }
         }
      }

      return 0;
   }

}

#endregion RptR_RekapFaktur

#region RptR_Ira_Ruc

public class RptR_Ira_Ruc            : RptR_StandardRiskReport
{
#region Constructor, Fieldz, Propertiez

   protected string FakturGR;

   bool isRucPoStavkama;
   bool isPoNaplati    ;
 //bool isUDGwanted    ;

   public RptR_Ira_Ruc(string _fakturGR, ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)

      : base(_reportDocument, _reportName, _rptFilter, _filterStyle,

         _rptNeeds_ArtWars,    // ArtiklWithArtstat 
         _rptNeeds_ArtStat,    // ArtStat        
         _rptNeeds_Faktur ,    // Faktur         
         _rptNeeds_Rtrans ,    // Rtrans         
         _rptNeeds_Kupdob ,    // Kupdob         
         _rptNeeds_Prjkt  ,    // Prjkt          
         _rptNeeds_Rtrans4ruc, // Rtrans4ruc     
         _rptNeeds_Artikl    ) // Artikl         

   {
      FakturGR = _fakturGR;
      RptFilter.NeedsGroupTree = (RptFilter.GrupiranjeDokum == "KupdobName") ? true : false;
      //TheFilterSet = RiskFilterSetEnum.REALIZ;

      if(_reportDocument is Vektor.Reports.RIZ.CR_RUC_Rtrans)      { isRucPoStavkama = true ; }
      else                                                         { isRucPoStavkama = false; }

      if(_reportDocument is Vektor.Reports.RIZ.CR_ProvizijaKomerc) { isPoNaplati     = true ; }
      else                                                         { isPoNaplati     = false; }

      // 10.05.2017: Tembo UDG additions ... 15.05. ipak NE 
    //if(isRucPoStavkama == true || isPoNaplati == true) isUDGwanted = false;
    //else                                               isUDGwanted = true ;

   }

#endregion Constructor, Fieldz, Propertiez

   public override int FillRiskReportLists()
   {
#region Classic FillRiskReportLists, isPoStavkama

      // 02.03.2015:
      bool reportNeedsPGyearData = ReportNeedsPGyearData(RptFilter.DatumOd);
      if(reportNeedsPGyearData)
      {
         for(int year = RptFilter.DatumOd.Year; year < ZXC.projectYearFirstDay.Year; ++year)
         {
            GetFakturList           (year);
            GetRtransWithArtstatList(year);
          //GetRtransList           (year);
         }
      }

      base.FillRiskReportLists();

      // 10.05.2017: Tembo UDG additions ... 15.05. ipak NE 
    //if(isUDGwanted == false) // izbaci UDG rtransove 
    //{
    //   TheRtransList.RemoveAll(r => r.A_ArtiklTS == "UDG"   ); 
    //   TheFakturList.RemoveAll(f => f.MtrosTK    == "TIFO79"); // holala, profi programiranje :-( 
    //}

      // 25.11.2016: 
      // 15.05.2017: ipak NE. Sipek, ipak, oce vidjeti i ruc UDP artikala u RUCu po stavkama. 

      // 15.05.2017: ipak NE.if(isRucPoStavkama) // UDP - usluga da izadje ali bez ruca  
      // 15.05.2017: ipak NE.{
      // 15.05.2017: ipak NE.   List<ZXC.VvUtilDataPackage> udpList = new List<ZXC.VvUtilDataPackage>();
      // 15.05.2017: ipak NE.
      // 15.05.2017: ipak NE.   foreach(Rtrans udpRtr in TheRtransList.Where(r => r.A_ArtiklTS == "UDP")) 
      // 15.05.2017: ipak NE.   {
      // 15.05.2017: ipak NE.      udpList.Add(new ZXC.VvUtilDataPackage() { TheUint = udpRtr.T_parentID, TheDecimal = udpRtr.R_KCR }); 
      // 15.05.2017: ipak NE.
      // 15.05.2017: ipak NE.      udpRtr.T_cij = 0M;
      // 15.05.2017: ipak NE.      udpRtr.CalcTransResults(null);
      // 15.05.2017: ipak NE.   }
      // 15.05.2017: ipak NE.
      // 15.05.2017: ipak NE.   TheFakturList.ForEach(f => f.S_ukKCR -= udpList.Where(u => u.TheUint == f.RecID).Sum(u => u.TheDecimal));
      // 15.05.2017: ipak NE.
      // 15.05.2017: ipak NE.} // if(isRucPoStavkama) // UDP - usluga da izadje ali bez ruca 

      AggregateFaktursIraRucProperties(isRucPoStavkama);

      if(isRucPoStavkama)
      {
         TheFakturList.ForEach(fak => fak.SetAllRtrans_R_mjData(TheArtiklList));
      }

#endregion Classic FillRiskReportLists, isPoStavkama

#region isPoNaplati - VOILA!
       
      if(isPoNaplati) // VOILA! This is smart! 
      {
#region Get Fakturs Ftranses

         TheFtransList = new List<Ftrans>();
         List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(4);

         filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_dokDate], "dateDO" , RptFilter.DatumDo, " <= ")); // Do datuma kraja razdoblja koje proucavamo        
       //filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_pot    ], "t_pot"  , 0                , " != ")); // Da t_pot   NIJE nula, znaci samo uplate          
         filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_tipBr  ], "t_tipBr", ""               , " != ")); // Da t_tipBr NIJE prazan, znaci samo vezane uplate 

         // 06.11.2014:
       //string wantedKontoSet = "120, 121, 122";
       //KtoShemaDsc KSD = new KtoShemaDsc(ZXC.dscLuiLst_KtoShema);
         string wantedKontoSet = ZXC.KSD.Dsc_KupacKontaIOS;

         filterMembers.Add(new VvSqlFilterMember("SUBSTRING(t_konto, 1, 3)", "(" + wantedKontoSet + ")", " IN ")); // Samo konta 'kupaca' 

         VvDaoBase.LoadGenericVvDataRecordList(TheDbConnection, TheFtransList, filterMembers, "");

#endregion Get Fakturs Ftranses

         // 21.01.2016: 
       //TheFakturList.RemoveAll(f => f.TT != Faktur.TT_IRA && f.TT != Faktur.TT_IOD                        );
         TheFakturList.RemoveAll(f => f.TT != Faktur.TT_IRA && f.TT != Faktur.TT_IOD && !f.TtInfo.Is_WYRN_TT);

         // 04.04.2018: skuteri out! 
         TheFakturList.RemoveAll(f => f.SkladCD == "MOTO");

         // 01.02.2017: 
         // 01.03.2017: ali ne i za ovogodisnje racune! 
       //TheFakturList.                                                                                             ForEach(f => f.TT = f.DokDate_Year_LastDigit + "p" + f.TT); // racuni iz PGPG razdoblja bi inace dobivali krivu 1. znamenku u NalogDao.GetTBRforPsNalog(string origTBR) 
         // 26.02.2020: ovo ubacivanje '9p' na pocetak smeta kada uplata zatvara YRN-ove 
       //TheFakturList.Where(f => f.DokDate.Year != ZXC.projectYearFirstDay.Year                        ).ToList().ForEach(f => f.TT =  f.DokDate_Year_LastDigit + "p" + f.TT ); // racuni iz PGPG razdoblja bi inace dobivali krivu 1. znamenku u NalogDao.GetTBRforPsNalog(string origTBR) 
         TheFakturList.Where(f => f.DokDate.Year != ZXC.projectYearFirstDay.Year && !f.TtInfo.Is_WYRN_TT).ToList().ForEach(f => f.TT =  f.DokDate_Year_LastDigit + "p" + f.TT ); // racuni iz PGPG razdoblja bi inace dobivali krivu 1. znamenku u NalogDao.GetTBRforPsNalog(string origTBR) 

         // 02.03.2015: svim prethodnogodisnjim fakturama rename-aj TipBr ala NalogPS 
       //var faktursWithFtransesList = TheFakturList.GroupJoin(TheFtransList, Fak => Fak.TipBr    , Ftr => Ftr.T_tipBr, (FAK, FTR) => new { faktur = FAK, ftranses = FTR }).ToList();
         var faktursWithFtransesList = TheFakturList.GroupJoin(TheFtransList, Fak => Fak.TipBr4RUC, Ftr => Ftr.T_tipBr, (FAK, FTR) => new { faktur = FAK, ftranses = FTR }).ToList();

         // 02.03.2015:
         bool isThisFakturPG = false;

         //foreach(var fakturWftranses in faktursWithFtransesList)
         for(int i = 0; i < faktursWithFtransesList.Count; ++i)
         // remarck next line and unremarck previous for non parallel*/ 
         //Parallel.ForEach(faktursWithFtransesList, fakturWftranses =>
         {
            // 01.02.2017: neuspjeh 
          //for(int fi = 0; fi < faktursWithFtransesList[i].ftranses.Count(); ++fi)
          //{
          //   if(faktursWithFtransesList[i].ftranses.ToArray()[fi].T_kupdob_cd != faktursWithFtransesList[i].faktur.KupdobCD)
          //   {
          //      faktursWithFtransesList[i].ftranses.ToList().RemoveAt(fi--);
          //   }
          //}

            if(faktursWithFtransesList[i].faktur.DokDate.Year < ZXC.projectYearFirstDay.Year) isThisFakturPG = true;

            if(faktursWithFtransesList[i].ftranses != null && faktursWithFtransesList[i].ftranses.Count().NotZero())
            {
               // 02.03.2015:
               if(true /*isThisFakturPG*/) // prioslogodisnja faktura koja je mogla biti djelomicno placena u prosloj godini (NalpgPS prenosi samo saldo racuna!)
               {
                  faktursWithFtransesList[i].faktur.R2_uplata = faktursWithFtransesList[i].ftranses.Sum(ftr => ftr.T_dug - ftr.T_pot);     // znaci, u R2_uplata se sada nalazi SALDO po tom racunu. 
               }
               else // classic, ovogodisnja faktura 
               {
                  faktursWithFtransesList[i].faktur.R2_uplata = faktursWithFtransesList[i].ftranses.Sum(ftr => ftr.T_pot);     // znaci, u R2_uplata se sada nalazi suma uplata po tom racunu. 
               }

               faktursWithFtransesList[i].faktur.PonudDate = faktursWithFtransesList[i].ftranses.Max(ftr => ftr.T_dokDate); // znaci, u PonudDate se sada nalazi datum zadnje uplate.       

               // 02.03.2015:
               if(/*isThisFakturPG &&*/ (ZXC.AlmostEqual(faktursWithFtransesList[i].faktur.R2_uplata, 0.00M, 0.01M) == false)) // R2_uplata sadrzi SALDO ovogodisnjeh ftransa - prioslogodisnja faktura koja je mogla biti djelomicno placena u prosloj godini (NalpgPS prenosi samo saldo racuna!)
               {
                  faktursWithFtransesList.RemoveAt(i--);
               }
             //else if(ZXC.AlmostEqual(faktursWithFtransesList[i].faktur.S_ukKCRP, faktursWithFtransesList[i].faktur.R2_uplata, 0.01M) == false) // faktura NIJE zatvorena. Izbaci ju!  
             //{
             //   faktursWithFtransesList.RemoveAt(i--);
             //}
             //else if(faktursWithFtransesList[i].faktur.PonudDate < RptFilter.DatumOd || 
               else if(faktursWithFtransesList[i].faktur.PonudDate < RptFilter.Date3   || 
                       faktursWithFtransesList[i].faktur.PonudDate > RptFilter.DatumDo) // zadnja uplata (dakle datum krajnjeg zatvaranja)
                                                                                        // NIJE iz ovog razdoblja. Izbaci ju! 
               {
                  faktursWithFtransesList.RemoveAt(i--);
               }
            } // if(fakturWftranses.ftranses != null && fakturWftranses.ftranses.Count().NotZero())
            else
            {
               faktursWithFtransesList.RemoveAt(i--);
            }
         }

         /* remarck this zagrada line for non parallel*///);

         TheDeviznaSumaList =

            faktursWithFtransesList
            .Select(fakturWftranses => new VvReportSourceUtil
              (
               /* DevName      */ fakturWftranses.faktur.PersonName,
               /* TheCD        */ fakturWftranses.faktur.TipBr,
               /* Count        */ (int)((ZXC.VvGet_25_of_100(fakturWftranses.faktur.Ira_ROB_Ruv, ZXC.RRD.Dsc_KomProvizSt)) * 100M), // trik da do kristala dojde iznos provizije (ali ga Tamara mora dijeliti sa 100)
               /* Kol          */ ZXC.RRD.Dsc_KomProvizSt,
               /* TheMoney     */ fakturWftranses.faktur.S_ukKCR,
               /* TheMoney2    */ fakturWftranses.faktur.Ira_ROB_Ruv,
               /* FakturGR     */ /*GetFakGRdata(FakturGR, fakturWftranses.faktur)*/ fakturWftranses.faktur.PersonName, // well, ovaj izvj. fiksno grupiramo po Komercijalisti 
               /* KupdobCD     */ fakturWftranses.faktur.KupdobCD,
               /* KupdobName   */ fakturWftranses.faktur.KupdobName,
               /* ArtiklGrCD   */ "",
               /* ArtiklGrName */ "",
               /* TheDate      */ fakturWftranses.faktur.DokDate,
               /* TheDate2        fakturWftranses.ftranses.Where(ftr => (ftr.T_TT == Nalog.IZ_TT || ftr.T_TT == Nalog.KP_TT)).Max(ftr => ftr.T_dokDate), // zadnji datum izvoda ili kompenzacije */
               /* TheDate2     */ fakturWftranses.ftranses.Where(ftr => (ftr.OtsSTATUS == "Z"                              )).Max(ftr => ftr.T_dokDate), // zadnji datum izvoda ili kompenzacije 
               /* TheKoef      */ fakturWftranses.faktur.TrnSum_R_kolOP
               ))
            .OrderBy(RSU =>  RSU.DevName + RSU.KupdobName + RSU.TheCD)
            .ToList();

#region Add nepojavljene kupdobe za invest. trosak

         List<Kupdob> noFakturKupdobList = VvUserControl.KupdobSifrar.Except(TheKupdobList).Where(kpdb => kpdb.InvestTr.NotZero()).ToList();

         if(noFakturKupdobList.Count.NotZero())
         {
            VvReportSourceUtil rsu_rec;

            foreach(Kupdob noFakturKupdob_rec in noFakturKupdobList)
            {
               if(RptFilter.Putnik_PersonCD.NotZero() && RptFilter.Putnik_PersonCD != noFakturKupdob_rec.PutnikID) continue;

               rsu_rec = new VvReportSourceUtil
                  (
                  /* DevName      */ noFakturKupdob_rec.PutName,
                  /* TheCD        */ "Nema rn.",
                  /* Count        */ 0,
                  /* Kol          */ ZXC.RRD.Dsc_KomProvizSt,
                  /* TheMoney     */ 0.00M,
                  /* TheMoney2    */ 0.00M,
                  /* FakturGR     */ noFakturKupdob_rec.PutName,
                  /* KupdobCD     */ noFakturKupdob_rec.KupdobCD,
                  /* KupdobName   */ noFakturKupdob_rec.Naziv,
                  /* ArtiklGrCD   */ "",
                  /* ArtiklGrName */ "",
                  /* TheDate      */ DateTime.MinValue,
                  /* TheDate2        fakturWftranses.ftranses.Where(ftr => (ftr.T_TT == Nalog.IZ_TT || ftr.T_TT == Nalog.KP_TT)).Max(ftr => ftr.T_dokDate), // zadnji datum izvoda ili kompenzacije */
                  /* TheDate2     */ DateTime.MinValue,
                  /* TheKoef      */ 0.00M
                  );

               TheDeviznaSumaList.Add(rsu_rec);

               TheKupdobList.Add(noFakturKupdob_rec);
            }

            TheDeviznaSumaList = TheDeviznaSumaList.OrderBy(RSU => RSU.DevName + RSU.KupdobName + RSU.TheCD).ToList();
         }

#endregion Add nepojavljene kupdobe za invest. trosak

      } // if(isPoNaplati) // VOILA! This is smart!  

#endregion isPoNaplati - VOILA!

      TheFakturLightList = TheFakturList.Select(faktur => new FakturLight(RptFilter.FuseBool3, GetFakGRdata(FakturGR, faktur), faktur)).ToList();

      return 0;
   }

}

public class RptR_Ira_Ruc_Rtrans     : RptR_Ira_Ruc
{
   public RptR_Ira_Ruc_Rtrans(string _fakturGR, ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)

      : base(_fakturGR, _reportDocument, _reportName, _rptFilter, _filterStyle,

         _rptNeeds_ArtWars,    // ArtiklWithArtstat 
         _rptNeeds_ArtStat,    // ArtStat        
         _rptNeeds_Faktur ,    // Faktur         
         _rptNeeds_Rtrans ,    // Rtrans         
         _rptNeeds_Kupdob ,    // Kupdob         
         _rptNeeds_Prjkt  ,    // Prjkt          
         _rptNeeds_Rtrans4ruc, // Rtrans4ruc     
         _rptNeeds_Artikl    ) // Artikl         

   {
   }

   public override int FillRiskReportLists()
   {
      base.FillRiskReportLists();

      // 25.04.2014: 
      // 18.02.2016: 
    //TheRtransList.RemoveAll(r => r.T_isIrmUsluga);
      TheRtransList.RemoveAll(r => r.T_isIrmUsluga && r.A_ArtiklTS != "UDP"); // UDP - uslugu 'daljnja prodaja' zelimo ostaviti u listi 

      // 04.04.2018: 
      TheRtransList.RemoveAll(r => r.A_ArtiklTS == "SKU"); // SKUT - Skuteri; izbaci ih odavdje 

      TheRtransList = TheFakturList
         .Join(TheRtransList, fak => fak.RecID, rtr => rtr.T_parentID, (fak, rtr) => new Rtrans()
         {
            currentData = rtr.currentData,
            _rtrResults = rtr._rtrResults,
            TmpDecimal  = rtr.TmpDecimal,
            TheAsEx     = rtr.TheAsEx,
            R_grName    = GetFakGRdata(FakturGR, fak),
         }).ToList();

      //prodRtransList.ForEach(rtr => rtr.CalcTransResults(null));


      return 0;
   }
}

#endregion RptR_Ira_Ruc

#region The PDV Suite

public struct PdvObrazacData
{

   public decimal ukOsnPdvObrI2       ;
   public decimal ukOsnPdvObrI        ;
   public decimal ukOsnPdvF_obrII4    ;
   public decimal ukPdvPdvF_obrII4    ;
   public decimal ukOsnPdvObrII       ;
   public decimal ukOsnIRA            ;
   public decimal ukPdvIRA            ;
   public decimal ukOsn25mRP          ;
   public decimal ukOsn22i23mRP       ;
   public decimal ukOsn23mRP          ;
   public decimal ukOsn22mRP          ;
   public decimal ukOsn05i10mRP       ;
   public decimal ukOsnURA            ;
   public decimal pdvIspravak_obrIII8 ;
   public decimal ukPdv25mRP          ;
   public decimal ukPdv22i23mRP       ;
   public decimal ukPdv23mRP          ;
   public decimal ukPdv22mRP          ;
   public decimal ukPdv05i10mRP       ;
   public decimal ukPdvURA            ;
   public decimal porObveza           ;
   public decimal porezKredit         ;
   public decimal ukRazlika           ;
   public decimal ukOsnUu22i23        ; 
   public decimal ukPdvUu22i23        ;
   public decimal ukOsn22i23_Ira      ;
   public decimal ukPdv22i23_Ira      ;
   public decimal ukOsnUr23i25        ;
   public decimal ukPdvUr23i25        ;
   public decimal ukOsnUr23i25i05     ;
   public decimal ukPdvUr23i25i05     ;

   public decimal ukOsn05i10_Ira      ;
   public decimal ukPdv05i10_Ira      ;

   public decimal porNeOdb350         ;
   public decimal porNeOdb360         ;
   public decimal porNeOdb370         ;

   //EU__________________________________________
   public decimal ukOsn25mn_TP        ;
   public decimal ukPdv25mn_TP        ;
   public decimal ukOsnR05mn_EU       ;
   public decimal ukPdvR05mn_EU       ;
   public decimal ukOsnR10mn_EU       ;
   public decimal ukPdvR10mn_EU       ;
   public decimal ukOsnR25mn_EU       ;
   public decimal ukPdvR25mn_EU       ;
   public decimal ukOsnU05mn_EU       ;
   public decimal ukPdvU05mn_EU       ;
   public decimal ukOsnU10mn_EU       ;
   public decimal ukPdvU10mn_EU       ;
   public decimal ukOsnU25mn_EU       ;
   public decimal ukPdvU25mn_EU       ;
   public decimal ukOsn10mn_BS        ;
   public decimal ukPdv10mn_BS        ;
   public decimal ukOsn25mn_BS        ;
   public decimal ukPdv25mn_BS        ;
   public decimal ukOsn222325_HR      ;
   public decimal ukPdv222325_HR      ;

   public decimal ukOsn_HR            ;
   public decimal ukPdv_HR            ;
   public decimal ukOsnRm_EU          ;
   public decimal ukPdvRm_EU          ;
   public decimal ukOsnUm_EU          ;
   public decimal ukPdvUm_EU          ;
   public decimal ukOsnm_BS           ;
   public decimal ukPdvm_BS           ;
   public decimal ukOsnm_TP           ;
   public decimal ukPdvm_TP           ;

   public decimal ukOsn_WO            ;
   public decimal ukPdv_WO            ;
   public decimal ukOsnObr_WO         ;
   public decimal ukPdvObr_WO         ;

   public decimal sumI                ; // suma tocke I                 
   public decimal sumIIOsn            ; // suma tocke II osnovica       
   public decimal sumIIPdv            ; // suma tocke II pdv            
   public decimal sumIiII             ; // suma tocke I + suma tocke II 
   public decimal sumIIIOsn           ; // suma tocke III osnovica      
   public decimal sumIIIPdv           ; // suma tocke III pdv           
   public decimal obvezaPDV           ; // II-III ili III-II            
   public decimal razlikaUplPov       ; // za uplatu/povrat             
   public decimal redakVII            ; // %                            

   public decimal ukUlazOsn05_HR      ; // od 01.01.2014.
   public decimal ukUlazPdv05_HR      ;
   public decimal ukUlazOsn13_HR      ;
   public decimal ukUlazPdv13_HR      ;
   public decimal ukUlazOsn25_HR      ;
   public decimal ukUlazPdv25_HR      ;

   public decimal pdv_810_sum; // od 01.01.2015.
   public decimal pdv_811    ;
   public decimal pdv_812    ;
   public decimal pdv_813    ;
   public decimal pdv_814    ;
   public decimal pdv_815    ;
   public decimal pdv_820    ;
   public decimal pdv_830_sum;
   public decimal pdv_831vr  ;
   public decimal pdv_832vr  ;
   public decimal pdv_833vr  ;
   public int     pdv_831br  ;
   public int     pdv_832br  ;
   public int     pdv_833br  ;
   public decimal pdv_840_sum;
   public decimal pdv_850_sum;
   public decimal pdv_860    ;

   public PdvObrazacData(Faktur _faktur_rec_SumaRazdoblja_URA, Faktur _faktur_rec_SumaRazdoblja_IRA, VvRpt_RiSk_Filter _rptFilter)
   {
#region "R"_sume IRA

      this.ukOsnPdvObrI2    = _faktur_rec_SumaRazdoblja_IRA.S_ukOsn08 + 
                              _faktur_rec_SumaRazdoblja_IRA.S_ukOsn09 + 
                              _faktur_rec_SumaRazdoblja_IRA.S_ukOsn10 + 
                              _faktur_rec_SumaRazdoblja_IRA.S_ukOsn11 ;
                            
      this.ukOsnPdvObrI     = ukOsnPdvObrI2                                  + 
                              _faktur_rec_SumaRazdoblja_IRA.S_ukOsn0  + 
                              _faktur_rec_SumaRazdoblja_IRA.S_ukOsn07 ;

      this.ukOsnPdvF_obrII4 = _rptFilter.PdvF_Osn;
      this.ukPdvPdvF_obrII4 = _rptFilter.PdvF_Pdv;

      this.ukOsnPdvObrII    = _faktur_rec_SumaRazdoblja_IRA.S_ukOsn10m +
                              _faktur_rec_SumaRazdoblja_IRA.S_ukOsn22m +
                              _faktur_rec_SumaRazdoblja_IRA.S_ukOsn23m +
                              _faktur_rec_SumaRazdoblja_IRA.S_ukOsn25m +
                              _faktur_rec_SumaRazdoblja_IRA.S_ukOsn05m +  // ovaj redak dodan 30.01.2013. za novi PDV2013 
                              ukOsnPdvF_obrII4; 
      
      this.ukOsnIRA        = ukOsnPdvObrI  + ukOsnPdvObrII; 
                           
      this.ukPdvIRA        = _faktur_rec_SumaRazdoblja_IRA.S_ukPdv10m +
                             _faktur_rec_SumaRazdoblja_IRA.S_ukPdv22m +
                             _faktur_rec_SumaRazdoblja_IRA.S_ukPdv23m +
                             _faktur_rec_SumaRazdoblja_IRA.S_ukPdv25m +
                             _faktur_rec_SumaRazdoblja_IRA.S_ukPdv05m +  // ovaj redak dodan 30.01.2013. za novi PDV2013 
                             ukPdvPdvF_obrII4;

      this.ukOsn22i23_Ira = _faktur_rec_SumaRazdoblja_IRA.S_ukOsn22m + _faktur_rec_SumaRazdoblja_IRA.S_ukOsn23m;
      this.ukPdv22i23_Ira = _faktur_rec_SumaRazdoblja_IRA.S_ukPdv22m + _faktur_rec_SumaRazdoblja_IRA.S_ukPdv23m;

      this.ukOsn05i10_Ira = _faktur_rec_SumaRazdoblja_IRA.S_ukOsn05m + _faktur_rec_SumaRazdoblja_IRA.S_ukOsn10m;
      this.ukPdv05i10_Ira = _faktur_rec_SumaRazdoblja_IRA.S_ukPdv05m + _faktur_rec_SumaRazdoblja_IRA.S_ukPdv10m;

#endregion "R"_sume IRA

#region "R"_sume URA

      this.ukOsn25mRP    = _faktur_rec_SumaRazdoblja_URA.S_ukOsn25m - 
                           _faktur_rec_SumaRazdoblja_URA.S_ukOsnUr25 -
                           _faktur_rec_SumaRazdoblja_URA.S_ukOsnUu25;
                         
      this.ukOsn23mRP    = _faktur_rec_SumaRazdoblja_URA.S_ukOsn23m - 
                           _faktur_rec_SumaRazdoblja_URA.S_ukOsnUr23 -
                           _faktur_rec_SumaRazdoblja_URA.S_ukOsnUu23;
                         
      this.ukOsn22mRP    = _faktur_rec_SumaRazdoblja_URA.S_ukOsn22m -
                           _faktur_rec_SumaRazdoblja_URA.S_ukOsnUu22;

      this.ukOsn22i23mRP = this  .ukOsn22mRP + this  .ukOsn23mRP;

      this.ukOsn05i10mRP = _faktur_rec_SumaRazdoblja_URA.S_ukOsn10m  -
                           _faktur_rec_SumaRazdoblja_URA.S_ukOsnUu10 +
                           _faktur_rec_SumaRazdoblja_URA.S_ukOsn05m  - // ovaj redak dodan 30.01.2013. za novi PDV2013 
                           _faktur_rec_SumaRazdoblja_URA.S_ukOsnUr05 ; // ovaj redak dodan 01.03.2013. jer postoji  S_ukOsnUr05 tj. treba oduzeti ono sto se odnosi na uvoz robe
                       
      this.ukOsnURA      = _faktur_rec_SumaRazdoblja_URA.S_ukOsn10m  +
                           _faktur_rec_SumaRazdoblja_URA.S_ukOsn22m  + 
                           _faktur_rec_SumaRazdoblja_URA.S_ukOsn23m  +
                           _faktur_rec_SumaRazdoblja_URA.S_ukOsn25m  +
                           _faktur_rec_SumaRazdoblja_URA.S_ukOsn05m  ; // ovaj redak dodan 30.01.2013. za novi PDV2013 

      this.pdvIspravak_obrIII8 = _rptFilter.PdvIspravak;

      this.ukPdv25mRP    = _faktur_rec_SumaRazdoblja_URA.S_ukPdv25m - 
                           _faktur_rec_SumaRazdoblja_URA.S_ukPdvUr25 -
                           _faktur_rec_SumaRazdoblja_URA.S_ukPdvUu25;
                         
      this.ukPdv23mRP    = _faktur_rec_SumaRazdoblja_URA.S_ukPdv23m - 
                           _faktur_rec_SumaRazdoblja_URA.S_ukPdvUr23 -
                           _faktur_rec_SumaRazdoblja_URA.S_ukPdvUu23;
                         
      this.ukPdv22mRP    = _faktur_rec_SumaRazdoblja_URA.S_ukPdv22m -
                           _faktur_rec_SumaRazdoblja_URA.S_ukPdvUu22;

      this.ukPdv22i23mRP = this.ukPdv22mRP + this.ukPdv23mRP;

      this.ukPdv05i10mRP = _faktur_rec_SumaRazdoblja_URA.S_ukPdv10m  -
                           _faktur_rec_SumaRazdoblja_URA.S_ukPdvUu10 +
                           _faktur_rec_SumaRazdoblja_URA.S_ukPdv05m  - // ovaj redak dodan 30.01.2013. za novi PDV2013 
                           _faktur_rec_SumaRazdoblja_URA.S_ukPdvUr05; // ovaj redak dodan 01.03.2013. jer postoji  S_ukPdvUr05 tj. treba oduzeti ono sto se odnosi na uvoz robe

      this.ukPdvURA      = _faktur_rec_SumaRazdoblja_URA.S_ukPdv10m  + 
                           _faktur_rec_SumaRazdoblja_URA.S_ukPdv22m  + 
                           _faktur_rec_SumaRazdoblja_URA.S_ukPdv23m  + 
                           _faktur_rec_SumaRazdoblja_URA.S_ukPdv25m  + 
                           _faktur_rec_SumaRazdoblja_URA.S_ukPdv05m  + // ovaj redak dodan 30.01.2013. za novi PDV2013 
                           pdvIspravak_obrIII8;

      this.ukOsnUu22i23 = _faktur_rec_SumaRazdoblja_URA.S_ukOsnUu22 + _faktur_rec_SumaRazdoblja_URA.S_ukOsnUu23;
      this.ukPdvUu22i23 = _faktur_rec_SumaRazdoblja_URA.S_ukPdvUu22 + _faktur_rec_SumaRazdoblja_URA.S_ukPdvUu23;

      this.ukOsnUr23i25 = _faktur_rec_SumaRazdoblja_URA.S_ukOsnUr23 + _faktur_rec_SumaRazdoblja_URA.S_ukOsnUr25;
      this.ukPdvUr23i25 = _faktur_rec_SumaRazdoblja_URA.S_ukPdvUr23 + _faktur_rec_SumaRazdoblja_URA.S_ukPdvUr25;
     

#endregion "R"_sume URA

#region "R"_sume POREZNA OBVEZA

      this  .porObveza   = ukPdvIRA - ukPdvURA;
      this  .porezKredit = _rptFilter.PdvKredit;
      this  .ukRazlika   = porObveza - porezKredit;

#endregion "R"_sume POREZNA OBVEZA

#region news in 2012

      this  .porNeOdb350 = _rptFilter.Pdv_Fof_III5; 
      this  .porNeOdb360 = _rptFilter.Pdv_Fof_III6; 
      this  .porNeOdb370 = _rptFilter.Pdv_Fof_III7;

      this.ukOsnUr23i25i05 = _faktur_rec_SumaRazdoblja_URA.S_ukOsnUr23 + _faktur_rec_SumaRazdoblja_URA.S_ukOsnUr25 + _faktur_rec_SumaRazdoblja_URA.S_ukOsnUr05;
      this.ukPdvUr23i25i05 = _faktur_rec_SumaRazdoblja_URA.S_ukPdvUr23 + _faktur_rec_SumaRazdoblja_URA.S_ukPdvUr25 + _faktur_rec_SumaRazdoblja_URA.S_ukPdvUr05;

#endregion news in 2012

#region EU

      // II **********************************************************************************************************************

      // virtualni pdv iz ure ide na pozicije ira i to moze i nemoze jer je to ukuno obracunati pdv !!!!!!!                       
      /*  */    this.ukOsn25mn_TP   = _faktur_rec_SumaRazdoblja_URA.S_ukOsn25m_TP + _faktur_rec_SumaRazdoblja_URA.S_ukOsn25n_TP;
      /*  */    this.ukPdv25mn_TP   = _faktur_rec_SumaRazdoblja_URA.S_ukPdv25m_TP + _faktur_rec_SumaRazdoblja_URA.S_ukPdv25n_TP;
      /*  */                        
      /*  */    this.ukOsnR05mn_EU  = _faktur_rec_SumaRazdoblja_URA.S_ukOsnR05m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnR05n_EU;
      /*  */    this.ukPdvR05mn_EU  = _faktur_rec_SumaRazdoblja_URA.S_ukPdvR05m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukPdvR05n_EU;
      /*  */    this.ukOsnR10mn_EU  = _faktur_rec_SumaRazdoblja_URA.S_ukOsnR10m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnR10n_EU;
      /*  */    this.ukPdvR10mn_EU  = _faktur_rec_SumaRazdoblja_URA.S_ukPdvR10m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukPdvR10n_EU;
      /*  */    this.ukOsnR25mn_EU  = _faktur_rec_SumaRazdoblja_URA.S_ukOsnR25m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnR25n_EU;
      /*  */    this.ukPdvR25mn_EU  = _faktur_rec_SumaRazdoblja_URA.S_ukPdvR25m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukPdvR25n_EU;
      /*  */    this.ukOsnU05mn_EU  = _faktur_rec_SumaRazdoblja_URA.S_ukOsnU05m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnU05n_EU;
      /*  */    this.ukPdvU05mn_EU  = _faktur_rec_SumaRazdoblja_URA.S_ukPdvU05m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukPdvU05n_EU;
      /*  */    this.ukOsnU10mn_EU  = _faktur_rec_SumaRazdoblja_URA.S_ukOsnU10m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnU10n_EU;
      /*  */    this.ukPdvU10mn_EU  = _faktur_rec_SumaRazdoblja_URA.S_ukPdvU10m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukPdvU10n_EU;
      /*  */    this.ukOsnU25mn_EU  = _faktur_rec_SumaRazdoblja_URA.S_ukOsnU25m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnU25n_EU;
      /*  */    this.ukPdvU25mn_EU  = _faktur_rec_SumaRazdoblja_URA.S_ukPdvU25m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukPdvU25n_EU;
      /*  */                        
      /*  */    this.ukOsn10mn_BS   = _faktur_rec_SumaRazdoblja_URA.S_ukOsn10m_BS + _faktur_rec_SumaRazdoblja_URA.S_ukOsn10n_BS;
      /*  */    this.ukPdv10mn_BS   = _faktur_rec_SumaRazdoblja_URA.S_ukPdv10m_BS + _faktur_rec_SumaRazdoblja_URA.S_ukPdv10n_BS;
      /*  */    this.ukOsn25mn_BS   = _faktur_rec_SumaRazdoblja_URA.S_ukOsn25m_BS + _faktur_rec_SumaRazdoblja_URA.S_ukOsn25n_BS;
      /*  */    this.ukPdv25mn_BS   = _faktur_rec_SumaRazdoblja_URA.S_ukPdv25m_BS + _faktur_rec_SumaRazdoblja_URA.S_ukPdv25n_BS;
      // virtualni pdv iz ure ide na pozicije ira                                                                                  

    //09.06.2020. do corone nije upotrebljavano, postoji potreba ali ne moze se tocno izvesti iz dokumnata pa se upisuje rucno
    //this.ukOsnObr_WO = 0.00M/*this.ukOsnUr23i25i05 + _faktur_rec_SumaRazdoblja_URA.S_ukOsnUu10 + _faktur_rec_SumaRazdoblja_URA.S_ukOsnUu25*/;
    //this.ukPdvObr_WO = 0.00M/*this.ukPdvUr23i25i05 + _faktur_rec_SumaRazdoblja_URA.S_ukPdvUu10 + _faktur_rec_SumaRazdoblja_URA.S_ukPdvUu25*/;
      this.ukOsnObr_WO = _rptFilter.PretpPriUvz_Osn;
      this.ukPdvObr_WO = _rptFilter.PretpPriUvz_Pdv;
      
      this.ukOsn222325_HR = _faktur_rec_SumaRazdoblja_IRA.S_ukOsn25m + _faktur_rec_SumaRazdoblja_IRA.S_ukOsn23m + _faktur_rec_SumaRazdoblja_IRA.S_ukOsn22m;
      this.ukPdv222325_HR = _faktur_rec_SumaRazdoblja_IRA.S_ukPdv25m + _faktur_rec_SumaRazdoblja_IRA.S_ukPdv23m + _faktur_rec_SumaRazdoblja_IRA.S_ukPdv22m;     

      // III *******************************************************************************************************************************

      //this.ukOsn_HR      = this.ukOsn25mRP + this.ukOsn22i23mRP + this.ukOsn05i10mRP ;  // u ovo se zbrajaju virtualni pretporezi pa ih treba izbiti
      //this.ukPdv_HR      = this.ukPdv25mRP + this.ukPdv22i23mRP + this.ukPdv05i10mRP ;  // u ovo se zbrajaju virtualni pretporezi pa ih treba izbiti

      // virtualni pretporez iz ure ili upe samo onaj koji se moze priznati !!!!!!                                                           
      /*  */    this.ukOsnRm_EU     = _faktur_rec_SumaRazdoblja_URA.S_ukOsnR05m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnR10m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnR25m_EU ;
      /*  */    this.ukPdvRm_EU     = _faktur_rec_SumaRazdoblja_URA.S_ukPdvR05m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukPdvR10m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukPdvR25m_EU;
      /*  */    
      /*  */    this.ukOsnUm_EU     = _faktur_rec_SumaRazdoblja_URA.S_ukOsnU05m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnU10m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnU25m_EU;
      /*  */    this.ukPdvUm_EU     = _faktur_rec_SumaRazdoblja_URA.S_ukPdvU05m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukPdvU10m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukPdvU25m_EU;
      /*  */    
      /*  */   // this.ukOsnm_BS      = _faktur_rec_SumaRazdoblja_URA.S_ukOsn25m_BS + _faktur_rec_SumaRazdoblja_URA.S_ukOsn10m_BS ;
      /*  */   // this.ukPdvm_BS      = _faktur_rec_SumaRazdoblja_URA.S_ukPdv25m_BS + _faktur_rec_SumaRazdoblja_URA.S_ukPdv10m_BS ;
      /*  */   // 15.07.2013. Zorica kaze sda ide ovako
      /*  */    this.ukOsnm_BS      = _faktur_rec_SumaRazdoblja_URA.S_ukOsn25m_BS + _faktur_rec_SumaRazdoblja_URA.S_ukOsn10m_BS + _faktur_rec_SumaRazdoblja_URA.S_ukOsnUu10 + _faktur_rec_SumaRazdoblja_URA.S_ukOsnUu25;
      /*  */    this.ukPdvm_BS      = _faktur_rec_SumaRazdoblja_URA.S_ukPdv25m_BS + _faktur_rec_SumaRazdoblja_URA.S_ukPdv10m_BS + _faktur_rec_SumaRazdoblja_URA.S_ukPdvUu10 + _faktur_rec_SumaRazdoblja_URA.S_ukPdvUu25;
      /*  */      
      /*  */    this.ukOsnm_TP      = _faktur_rec_SumaRazdoblja_URA.S_ukOsn25m_TP;
      /*  */    this.ukPdvm_TP      = _faktur_rec_SumaRazdoblja_URA.S_ukPdv25m_TP;
      // virtualni pretporez                                                                                                                  

      // 18.08.2013. zbog "15.07.2013. Zorica kaze sda ide ovako"
      //this.ukOsn_HR = this.ukOsn25mRP + this.ukOsn22i23mRP + this.ukOsn05i10mRP - this.ukOsnRm_EU - this.ukOsnUm_EU - this.ukOsnm_BS - this.ukOsnm_TP;
      //this.ukPdv_HR = this.ukPdv25mRP + this.ukPdv22i23mRP + this.ukPdv05i10mRP - this.ukPdvRm_EU - this.ukPdvUm_EU - this.ukPdvm_BS - this.ukPdvm_TP;
      this.ukOsn_HR = this.ukOsn25mRP + this.ukOsn22i23mRP + this.ukOsn05i10mRP - this.ukOsnRm_EU - this.ukOsnUm_EU - (_faktur_rec_SumaRazdoblja_URA.S_ukOsn25m_BS + _faktur_rec_SumaRazdoblja_URA.S_ukOsn10m_BS) - this.ukOsnm_TP;
      this.ukPdv_HR = this.ukPdv25mRP + this.ukPdv22i23mRP + this.ukPdv05i10mRP - this.ukPdvRm_EU - this.ukPdvUm_EU - (_faktur_rec_SumaRazdoblja_URA.S_ukPdv25m_BS + _faktur_rec_SumaRazdoblja_URA.S_ukPdv10m_BS) - this.ukPdvm_TP;
     
 //     this.ukOsn_WO      = this.ukOsnUr23i25i05 + _faktur_rec_SumaRazdoblja_URA.S_ukOsnUu10 + _faktur_rec_SumaRazdoblja_URA.S_ukOsnUu25;
 //     this.ukPdv_WO      = this.ukPdvUr23i25i05 + _faktur_rec_SumaRazdoblja_URA.S_ukPdvUu10 + _faktur_rec_SumaRazdoblja_URA.S_ukPdvUu25;
 // 15.07.2013. Zorica kaze sda ide ovako
      this.ukOsn_WO      = this.ukOsnUr23i25i05 /*+ _faktur_rec_SumaRazdoblja_URA.S_ukOsnUu10 + _faktur_rec_SumaRazdoblja_URA.S_ukOsnUu25*/;
      this.ukPdv_WO      = this.ukPdvUr23i25i05 /*+ _faktur_rec_SumaRazdoblja_URA.S_ukPdvUu10 + _faktur_rec_SumaRazdoblja_URA.S_ukPdvUu25*/;

      // SUME *******************************************************************************************************************************

      this.sumI          = _faktur_rec_SumaRazdoblja_IRA.S_ukOsn07 + _faktur_rec_SumaRazdoblja_IRA.S_ukOsn08 + _faktur_rec_SumaRazdoblja_IRA.S_ukOsn09 +
                           _faktur_rec_SumaRazdoblja_IRA.S_ukOsn10 + _faktur_rec_SumaRazdoblja_IRA.S_ukOsn11 + _faktur_rec_SumaRazdoblja_IRA.S_ukOsn12 +
                           _faktur_rec_SumaRazdoblja_IRA.S_ukOsn13 + _faktur_rec_SumaRazdoblja_IRA.S_ukOsn14 + _faktur_rec_SumaRazdoblja_IRA.S_ukOsn15 +
                           _faktur_rec_SumaRazdoblja_IRA.S_ukOsn16 ;

      this.sumIIOsn      = _faktur_rec_SumaRazdoblja_IRA.S_ukOsn05m + _faktur_rec_SumaRazdoblja_IRA.S_ukOsn10m + _faktur_rec_SumaRazdoblja_IRA.S_ukOsn25m +
                           _faktur_rec_SumaRazdoblja_IRA.S_ukOsn23m + _faktur_rec_SumaRazdoblja_IRA.S_ukOsn22m +
                           this.ukOsn25mn_TP  + this.ukOsn10mn_BS  + this.ukOsn25mn_BS  + 
                           this.ukOsnR05mn_EU + this.ukOsnR10mn_EU + this.ukOsnR25mn_EU +
                           this.ukOsnU05mn_EU + this.ukOsnU10mn_EU + this.ukOsnU25mn_EU +
                           this.ukOsnObr_WO   + this.ukOsnPdvF_obrII4                   ;

      this.sumIIPdv      =  _faktur_rec_SumaRazdoblja_IRA.S_ukPdv05m + _faktur_rec_SumaRazdoblja_IRA.S_ukPdv10m + _faktur_rec_SumaRazdoblja_IRA.S_ukPdv25m +  
                            _faktur_rec_SumaRazdoblja_IRA.S_ukPdv23m + _faktur_rec_SumaRazdoblja_IRA.S_ukPdv22m +
                            this.ukPdv25mn_TP  + this.ukPdv10mn_BS  + this.ukPdv25mn_BS  + 
                            this.ukPdvR05mn_EU + this.ukPdvR10mn_EU + this.ukPdvR25mn_EU +
                            this.ukPdvU05mn_EU + this.ukPdvU10mn_EU + this.ukPdvU25mn_EU +
                            this.ukPdvObr_WO   + this.ukPdvPdvF_obrII4                   ;
                         
      this.sumIiII       = this.sumI + this.sumIIOsn;

      this.sumIIIOsn     = this.ukOsn_HR + this.ukOsnRm_EU + this.ukOsnUm_EU + this.ukOsnm_BS + this.ukOsn_WO  + _faktur_rec_SumaRazdoblja_URA.S_ukOsn25m_TP;
      this.sumIIIPdv     = this.ukPdv_HR + this.ukPdvRm_EU + this.ukPdvUm_EU + this.ukPdvm_BS + this.ukPdv_WO  + _faktur_rec_SumaRazdoblja_URA.S_ukPdv25m_TP + this.pdvIspravak_obrIII8;

      // IV *******************************************************************************************************************************

      this.obvezaPDV     = this.sumIIPdv - this.sumIIIPdv;        
      this.razlikaUplPov = obvezaPDV - porezKredit;        
      this.redakVII      = 0.00M;          

#endregion EU

#region news 2014
      
      // III *******************************************************************************************************************************

      this.ukUlazOsn05_HR = _faktur_rec_SumaRazdoblja_URA.S_ukOsn05m - (_faktur_rec_SumaRazdoblja_URA.S_ukOsnR05m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnU05m_EU +/*_faktur_rec_SumaRazdoblja_URA.S_ukOsn05m_BS*/ _faktur_rec_SumaRazdoblja_URA.S_ukOsnUr05                                                             );
      this.ukUlazPdv05_HR = _faktur_rec_SumaRazdoblja_URA.S_ukPdv05m - (_faktur_rec_SumaRazdoblja_URA.S_ukPdvR05m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukPdvU05m_EU +/*_faktur_rec_SumaRazdoblja_URA.S_ukPdv05m_BS*/ _faktur_rec_SumaRazdoblja_URA.S_ukPdvUr05                                                             );
      this.ukUlazOsn13_HR = _faktur_rec_SumaRazdoblja_URA.S_ukOsn10m - (_faktur_rec_SumaRazdoblja_URA.S_ukOsnR10m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnU10m_EU +  _faktur_rec_SumaRazdoblja_URA.S_ukOsn10m_BS                                                                                                         );  
      this.ukUlazPdv13_HR = _faktur_rec_SumaRazdoblja_URA.S_ukPdv10m - (_faktur_rec_SumaRazdoblja_URA.S_ukPdvR10m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukPdvU10m_EU +  _faktur_rec_SumaRazdoblja_URA.S_ukPdv10m_BS                                                                                                         );
      this.ukUlazOsn25_HR = _faktur_rec_SumaRazdoblja_URA.S_ukOsn25m + 
                            _faktur_rec_SumaRazdoblja_URA.S_ukOsn23m +
                            _faktur_rec_SumaRazdoblja_URA.S_ukOsn22m - (_faktur_rec_SumaRazdoblja_URA.S_ukOsnR25m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnU25m_EU +  _faktur_rec_SumaRazdoblja_URA.S_ukOsn25m_BS + _faktur_rec_SumaRazdoblja_URA.S_ukOsnUr23 + _faktur_rec_SumaRazdoblja_URA.S_ukOsnUr25 + this.ukOsnm_TP);
      this.ukUlazPdv25_HR = _faktur_rec_SumaRazdoblja_URA.S_ukPdv25m +                                                                                                 
                            _faktur_rec_SumaRazdoblja_URA.S_ukPdv23m +                                                                                                 
                            _faktur_rec_SumaRazdoblja_URA.S_ukPdv22m - (_faktur_rec_SumaRazdoblja_URA.S_ukPdvR25m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukPdvU25m_EU +  _faktur_rec_SumaRazdoblja_URA.S_ukPdv25m_BS + _faktur_rec_SumaRazdoblja_URA.S_ukPdvUr23 + _faktur_rec_SumaRazdoblja_URA.S_ukPdvUr25 + this.ukPdvm_TP);  




#endregion news 2014

#region news 2015

      this.redakVII     = _rptFilter.Pdv_RedVII;          
      this.pdv_811      = _rptFilter.Pdv_811   ;
      this.pdv_812      = _rptFilter.Pdv_812   ;
      this.pdv_813      = _rptFilter.Pdv_813   ;
      this.pdv_814      = _rptFilter.Pdv_814   ;
      this.pdv_815      = _rptFilter.Pdv_815   ;
      this.pdv_810_sum  = this.pdv_811 + this.pdv_812 + this.pdv_813 + this.pdv_814 + this.pdv_815;
      this.pdv_820      = _rptFilter.Pdv_820   ;
      this.pdv_831vr    = _rptFilter.Pdv_831vr ;
      this.pdv_832vr    = _rptFilter.Pdv_832vr ;
      this.pdv_833vr    = _rptFilter.Pdv_833vr ;
      this.pdv_830_sum  = this.pdv_831vr + this.pdv_832vr + this.pdv_833vr;
      this.pdv_831br    = _rptFilter.Pdv_831br ;
      this.pdv_832br    = _rptFilter.Pdv_832br ;
      this.pdv_833br    = _rptFilter.Pdv_833br ;

    //24.11.2020. u ovu sumu bi trebalo ulaziti i trece zemlje, B, ali ne znamo koliko je usluga a koliko dobara                                                                                                         
    //            pa onda korisnik sam unosi sumarni podatak, ako postoji podatak u rptFilteru uzima taj a ako ne onda sumu kao do sada                                                                                  
    //this.pdv_840_sum  =                                                     _faktur_rec_SumaRazdoblja_URA.S_ukOsnU05m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnU10m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnU25m_EU;
      this.pdv_840_sum  = _rptFilter.Pdv_840.NotZero() ? _rptFilter.Pdv_840 : _faktur_rec_SumaRazdoblja_URA.S_ukOsnU05m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnU10m_EU + _faktur_rec_SumaRazdoblja_URA.S_ukOsnU25m_EU;

      this.pdv_850_sum  = _faktur_rec_SumaRazdoblja_IRA.S_ukOsn10      + _faktur_rec_SumaRazdoblja_IRA.S_ukOsn11.Ron2();
      this.pdv_860      = _rptFilter.Pdv_860;

#endregion news 2015


   }

}

public class RptR_PDV                : RptR_StandardRiskReport
{
#region Constructor 

   public bool isURA { get; set; }

   public RptR_PDV(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter) :

      base(_reportDocument, _reportName, _rptFilter, ZXC.RIZ_FilterStyle.Faktur, 
         /* _rptNeeds_ArtWars      */            false, // ArtiklWithArtstat         
         /* _rptNeeds_ArtStat      */            false, // ArtStat        
         /* _rptNeeds_Faktur       */            false, // Faktur         
         /* _rptNeeds_Rtrans       */            false, // Rtrans         
         /* _rptNeeds_Kupdob       */            false, // Kupdob         
         /* _rptNeeds_Prjkt        */            true , // Prjkt          
         /* _rptNeeds_Rtrans4ruc   */            false, // Rtrans4ruc     
         /* _rptNeeds_Artikl       */            false) // Artikl         
   {
      //TheFilterSet = RiskFilterSetEnum.PDV;
   }

#endregion Constructor

#region LoadPdvSume

   protected Faktur LoadPdvSume(XSqlConnection conn, DateTime _dateOd, bool _isUra, bool needsLineCount)
   {
      Faktur faktur_rec;

      DateTime origDateOd = RptFilter.DatumOd; // ako je od pocetka godine da moze restore-ati originalni dateOd 

      RptFilter.DatumOd = _dateOd;
      
      this.isURA = _isUra;

      TheReportUC.AddFilterMemberz();

      faktur_rec = FakturDao.GetManyFakturSumAsOneSintFaktur(conn, RptFilter.FilterMembers, _isUra, needsLineCount, VvSQL.JoinClauseFor_R2_fakturs);

      RptFilter.DatumOd = origDateOd;

      // 17.02.2014: za MakeDeepCoopy da budu IsExtendable = true 
      faktur_rec.TT = (isURA ? "URA" : "IRA");

      return faktur_rec;

   }

#endregion LoadPdvSume

}

public class RptR_PDV_Knjiga         : RptR_PDV
{
   public RptR_PDV_Knjiga(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, bool _isURA) : base(_reportDocument, _reportName, _rptFilter)
   {
      this.isURA = _isURA;

      // 22.01.2019: 
      if(isURA)
      {
         IsForExport = true;
      }
   }

   public override int FillRiskReportLists()
   {
      //base.FillRiskReportLists(); // Za Prjkt only 

      TheFakturList = new List<Faktur>();

      // Fill TheFakturList 
      // 04.10.2018: dodana PDV_RUC knjiga, a njoj treba sve iz faktur dataLayera 
    //if(isURA                                                    )
      if(isURA || RptFilter.PdvKnjiga == ZXC.PdvKnjigaEnum.PDV_RUC)
      {
         VvDaoBase.LoadGenericVvDataRecordList(TheDbConnection, TheFakturList, RptFilter.FilterMembers, "", /*FakturOrderBy*/"L.recID", true, /*" * "*/ " L.*, R.*, " + FakturDao.ftrUplata + " ", VvSQL.JoinClauseFor_R2_fakturs); //!!! Dakle, PDV knjiga URA se soritra po redosljedu unosa (VIDI i VvSql.LoadIraUnionIrmGroupedFakturList_Command()) 
      }
      else // IRA 
      { 
         FakturDao.LoadIraUnionIrmGroupedFakturList(TheDbConnection, TheFakturList, SetFilterMembers4KnjigaIRA()); 
      }

      // Fill sumaRazdoblja, fill kumulativOdPg 
      FillFakturRec_SUM();

      if(isURA)
      {
         foreach(Faktur faktur in TheFakturList.Where(f => f.KdOib.IsEmpty() || f.VezniDok.IsEmpty()))
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Prazan OIB ili OrigBrDok!\n\n{0}", faktur.ToString());
         }
      }

      return 0;
   }

   private void FillFakturRec_SUM()
   {
      Faktur faktur_rec_SumaRazdoblja, faktur_rec_KumulativOdPg_justForPdvNum;
      
      if(isURA)
      {
         Faktur_rec_SumaRazdoblja_URA           = new Faktur();
         Faktur_rec_KumulativOdPg_URA           = LoadPdvSume(TheDbConnection, ZXC.projectYearFirstDay, isURA, true);
         faktur_rec_SumaRazdoblja               = Faktur_rec_SumaRazdoblja_URA;
         faktur_rec_KumulativOdPg_justForPdvNum = Faktur_rec_KumulativOdPg_URA;
      }
      else
      {
         Faktur_rec_SumaRazdoblja_IRA           = new Faktur();
         Faktur_rec_KumulativOdPg_IRA           = LoadPdvSume(TheDbConnection, ZXC.projectYearFirstDay, isURA, true);
         faktur_rec_SumaRazdoblja               = Faktur_rec_SumaRazdoblja_IRA;
         faktur_rec_KumulativOdPg_justForPdvNum = Faktur_rec_KumulativOdPg_IRA;

         // 18.03.2016: za R2 po naplati ovdje moraju doci i YRN-ovi 
         if(ZXC.CURR_prjkt_rec.NoNeedFor_WRN_UFRA == false)
         {
            faktur_rec_KumulativOdPg_justForPdvNum = LoadPdvSume(TheDbConnection, ZXC.Date01012010 /*ZXC.projectYearFirstDay*/, isURA, true);
         }
      }

   // 19.03.2018: data layer 'PdvNum' property oslobađamo od dosadasnje upotrebe da bi ga mogli koristiti za zaista data layer upotrebu 
    //uint startPdvNum = faktur_rec_KumulativOdPg_justForPdvNum.  PdvNum - (uint)TheFakturList.Count + 1;
      uint startPdvNum = faktur_rec_KumulativOdPg_justForPdvNum.X_PdvNum - (uint)TheFakturList.Count + 1;

      TheFakturList.ForEach(delegate(Faktur fak)
      {
      // 19.03.2018: data layer 'PdvNum' property oslobađamo od dosadasnje upotrebe da bi ga mogli koristiti za zaista data layer upotrebu 
       //fak.  PdvNum = startPdvNum++;
         fak.X_PdvNum = startPdvNum++;

         if(fak.TtInfo.IsUlazniPdvTT && fak.PdvR12 == ZXC.PdvR12Enum.R2 && fak.R2_uplata.NotZero()) // Izlazni pdv se vec obavi u 'LoadIraUnionIrmGroupedFakturList' 
         {
            fak.RatioValuesOnR2Uplata();
         }
      });

      faktur_rec_SumaRazdoblja.SumValuesFromList(TheFakturList);

   }

   private List<VvSqlFilterMember> SetFilterMembers4KnjigaIRA()
   {
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(6);

    //filterMembers.Add(new VvSqlFilterMember(FakSch  [FakCI  .tt       ], false, "TT",        Faktur.TT_IRM,       "", "", " = ",  ""));
      filterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.pdvDate  ], ZXC.FM_OR_Enum.OPEN_OR , false, "PdvFakDateOD", RptFilter.DatumOd,   "", "", " >= ", ""));
      filterMembers.Add(new VvSqlFilterMember(FtrSch  [FtrCI.t_dokDate  ], ZXC.FM_OR_Enum.CLOSE_OR, false, "PdvFtrDateOD", RptFilter.DatumOd,   "", "", " >= ", ""));
      filterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.pdvDate  ], ZXC.FM_OR_Enum.OPEN_OR , false, "PdvFakDateDO", RptFilter.DatumDo,   "", "", " <= ", ""));
      filterMembers.Add(new VvSqlFilterMember(FtrSch  [FtrCI.t_dokDate  ], ZXC.FM_OR_Enum.CLOSE_OR, false, "PdvFtrDateDO", RptFilter.DatumDo,   "", "", " <= ", ""));
      if(RptFilter.MT_sifra.NotZero())
      filterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.mtrosCD  ], false, "MtrosCD",   RptFilter.MT_sifra,  "", "", " = ",  ""));
      filterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.pdvKnjiga], false, "PdvKnjiga", RptFilter.PdvKnjiga, "", "", " = ",  ""));

      return filterMembers;
   }

#region ePDV_URA XML Export

   // ovdje nisi nasao nikakve upute kako bi se trebao zvati fajl pa ga imenujes proizvoljno 
   public override string ExportFileName
   {
      get
      {
         string mmyyyy = RptFilter.DatumOd.Month.ToString("00") + RptFilter.DatumOd.Year.ToString("0000");
         string   yyyy =                                          RptFilter.DatumOd.Year.ToString("0000");

         return "PDV_URA_"  + ZXC.CURR_prjkt_rec.Oib + "_" + mmyyyy + ".xml";
      }
   }

#region Xml Schema Validation

   public override bool ExecuteExportValidation(string fileName)
   {
      List<ZXC.VvXmlValidationData> valDataList = new List<ZXC.VvXmlValidationData>();
      
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacURA/v1-0", @"XSD\ObrazacURA-v1-0.xsd"          ));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacURA/v1-0", @"XSD\ObrazacURAtipovi-v1-0.xsd"    ));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\ObrazacURAmetapodaci-v1-0.xsd"));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\MetapodaciTipovi-v2-0.xsd"    ));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/TemeljniTipovi/v2-1"     , @"XSD\TemeljniTipovi-v2-1.xsd"      ));

      return ExecuteExportValidation_Base(valDataList);
   }

#endregion Xml Schema Validation

   public override bool ExecuteExport(string fileName)
   {
      PDV_URA.sObrazacURA theURA = new PDV_URA.sObrazacURA();

#region Metapodaci

      theURA.Metapodaci = new PDV_URA.sURAmetapodaci()
      {
         Naslov        = new PDV_URA.sNaslovTemeljni()        { Value = "Knjiga primljenih (ulaznih) računa"                      },
         Autor         = new PDV_URA.sAutorTemeljni ()        { Value = ZXC.CURR_prjkt_rec.Ime + " " + ZXC.CURR_prjkt_rec.Prezime },
         Datum         = new PDV_URA.sDatumTemeljni()         { Value = DateTime.Now                                              },
         Format        = new PDV_URA.sFormatTemeljni()        { Value = PDV_URA.tFormat.textxml                                   },
         Jezik         = new PDV_URA.sJezikTemeljni()         { Value = PDV_URA.tJezik.hrHR                                       },
         Identifikator = new PDV_URA.sIdentifikatorTemeljni() { Value = Guid.NewGuid().ToString("D")/*.ToUpper()*/                },
         Uskladjenost  = new PDV_URA.sUskladjenost()          { Value = "ObrazacURA-v1-0"                                         },
         Tip           = new PDV_URA.sTipTemeljni()           { Value = PDV_URA.tTip.Elektroničkiobrazac                          },
         Adresant      = new PDV_URA.sAdresantTemeljni()      { Value = "Ministarstvo Financija, Porezna uprava, Zagreb"          }
      };

#endregion Metapodaci
      
#region Zaglavlje

      theURA.Zaglavlje           = new PDV_URA.sZaglavlje();

      theURA.Zaglavlje.Razdoblje = new PDV_URA.sRazdoblje() { DatumOd = RptFilter.DatumOd, DatumDo = RptFilter.DatumDo };

      theURA.Zaglavlje.Obveznik  = new PDV_URA.sPorezniObveznik();
      theURA.Zaglavlje.Obveznik.ItemElementName = PDV_URA.ItemChoiceType.OIB;
      theURA.Zaglavlje.Obveznik.Item            = ZXC.CURR_prjkt_rec.Oib    ;

      if(ZXC.CURR_prjkt_rec.IsFizickaOsoba)
      {
         theURA.Zaglavlje.Obveznik.ItemsElementName = new PDV_URA.ItemsChoiceType[] { PDV_URA.ItemsChoiceType.Ime, PDV_URA.ItemsChoiceType.Prezime };
         theURA.Zaglavlje.Obveznik.Items            = new string[]                  {      ZXC.CURR_prjkt_rec.Ime,      ZXC.CURR_prjkt_rec.Prezime };
      }
      else
      {
         theURA.Zaglavlje.Obveznik.ItemsElementName = new PDV_URA.ItemsChoiceType[] { PDV_URA.ItemsChoiceType.Naziv };
         theURA.Zaglavlje.Obveznik.Items            = new string[]                  { ZXC.CURR_prjkt_rec.Naziv      };
      }
      
      theURA.Zaglavlje.Obveznik.Adresa              = new PDV_URA.sAdresa()
            {
               Mjesto             = ZXC.CURR_prjkt_rec.Grad,
               Ulica              = ZXC.CURR_prjkt_rec.UlicaBezBroja_1,
               Broj               = ZXC.CURR_prjkt_rec.UlicniBroj_1_BezDodatka,
               DodatakKucnomBroju = ZXC.CURR_prjkt_rec.UlicniBroj_1_Dodatak,
            };

      theURA.Zaglavlje.Obveznik.PodrucjeDjelatnosti = GetPodrucjeDjelatnosti(ZXC.CURR_prjkt_rec.SifDcd);
      theURA.Zaglavlje.Obveznik.SifraDjelatnosti    = ZXC.CURR_prjkt_rec.SifDcd ;

      string ime, prezime;
      if(RptFilter.IsUserSastavio == true)
      {
         ime     = ZXC.CURR_user_rec.Ime    ;
         prezime = ZXC.CURR_user_rec.Prezime;
      }
      else
      {
         ime     = ZXC.CURR_prjkt_rec.Ime    ;
         prezime = ZXC.CURR_prjkt_rec.Prezime;
      }
      
      theURA.Zaglavlje.ObracunSastavio = new PDV_URA.sIspunjavatelj()
      {
         Ime     = ime    ,
         Prezime = prezime
      };

#endregion Zaglavlje

#region Tijelo

      theURA.Tijelo = new PDV_URA.sTijelo();

      PDV_URA.sRacun racunLine;

      theURA.Tijelo.Racuni = new PDV_URA.sRacun[TheFakturList.Count];

      for(int i = 0; i < theURA.Tijelo.Racuni.Length; ++i)
      {
         racunLine = theURA.Tijelo.Racuni[i] = new PDV_URA.sRacun();

         racunLine.R1  = TheFakturList[i].X_PdvNum.ToString();
         racunLine.R2  = TheFakturList[i].VezniDok          ;
         racunLine.R3  = TheFakturList[i].DokDate.Date;
         racunLine.R4  = TheFakturList[i].KupdobName     ;
         racunLine.R5  = TheFakturList[i].KdAdresa     ;
         racunLine.R6  = Get_ID_Type(TheFakturList[i].KupdobCD);
         racunLine.R7  = TheFakturList[i].KdOib;
         racunLine.R8  = TheFakturList[i].R_ukOsn05.Ron2();
         racunLine.R9  = TheFakturList[i].R_ukOsn10.Ron2();
         racunLine.R10 = (TheFakturList[i].R_ukOsn22 + TheFakturList[i].R_ukOsn23 + TheFakturList[i].R_ukOsn25).Ron2();
         racunLine.R11 = TheFakturList[i].R_ukKCRPwoPr.Ron2();
         racunLine.R12 = TheFakturList[i].S_ukPdv.Ron2();
         racunLine.R13 = TheFakturList[i].S_ukPdv05m.Ron2();
         racunLine.R14 = TheFakturList[i].S_ukPdv05n.Ron2();
         racunLine.R15 = TheFakturList[i].S_ukPdv10m.Ron2();
         racunLine.R16 = TheFakturList[i].S_ukPdv10n.Ron2();
         racunLine.R17 = (TheFakturList[i].S_ukPdv22m + TheFakturList[i].S_ukPdv23m + TheFakturList[i].S_ukPdv25m).Ron2();
         racunLine.R18 = (TheFakturList[i].S_ukPdv22n + TheFakturList[i].S_ukPdv23n + TheFakturList[i].S_ukPdv25n).Ron2();
      }

      theURA.Tijelo.Ukupno = new PDV_URA.sRacuniUkupno()
      {
         U8  = this.Faktur_rec_SumaRazdoblja_URA.R_ukOsn05.Ron2(),
         U9  = this.Faktur_rec_SumaRazdoblja_URA.R_ukOsn10.Ron2(),
         U10 = (this.Faktur_rec_SumaRazdoblja_URA.R_ukOsn25 + this.Faktur_rec_SumaRazdoblja_URA.R_ukOsn23 + this.Faktur_rec_SumaRazdoblja_URA.R_ukOsn22).Ron2(),
         U11 = this.Faktur_rec_SumaRazdoblja_URA.R_ukKCRPwoPr.Ron2(),
         U12 = this.Faktur_rec_SumaRazdoblja_URA.S_ukPdv     .Ron2(),
         U13 = this.Faktur_rec_SumaRazdoblja_URA.S_ukPdv05m  .Ron2(),
         U14 = this.Faktur_rec_SumaRazdoblja_URA.S_ukPdv05n  .Ron2(),
         U15 = this.Faktur_rec_SumaRazdoblja_URA.S_ukPdv10m  .Ron2(),
         U16 = this.Faktur_rec_SumaRazdoblja_URA.S_ukPdv10n  .Ron2(),
         U17 = (this.Faktur_rec_SumaRazdoblja_URA.S_ukPdv25m + this.Faktur_rec_SumaRazdoblja_URA.S_ukPdv23m + this.Faktur_rec_SumaRazdoblja_URA.S_ukPdv22m).Ron2(),
         U18 = (this.Faktur_rec_SumaRazdoblja_URA.S_ukPdv25n + this.Faktur_rec_SumaRazdoblja_URA.S_ukPdv23n + this.Faktur_rec_SumaRazdoblja_URA.S_ukPdv22n).Ron2(),
      };

#endregion Tijelo

      string xmlString = theURA.SaveToFile(fileName, ZXC.VvUTF8Encoding_noBOM);

      return true;
   }

   private int Get_ID_Type(uint kupdobCD)
   {
      Kupdob kupdob_rec = TheReportUC.Get_Kupdob_FromVvUcSifrar(kupdobCD);
      if(kupdob_rec == null) kupdob_rec = new Kupdob();

      if(kupdob_rec.IsHRVATSKA) return 1;
      else                      return 2;
   }

   private string GetPodrucjeDjelatnosti(string sifDcd)
   {
      string sifDcdRoot = ZXC.SubstringSafe(sifDcd, 0, 2);

      switch(sifDcdRoot)
      {
         case "01":
         case "02":
         case "03": return "A";
         case "05":
         case "06":
         case "07":
         case "08":
         case "09": return "B";
         case "10":
         case "11":
         case "12":
         case "13":
         case "14":
         case "15":
         case "16":
         case "17":
         case "18":
         case "19":
         case "20":
         case "21":
         case "22":
         case "23":
         case "24":
         case "25":
         case "26":
         case "27":
         case "28":
         case "29":
         case "30":
         case "31":
         case "32":
         case "33": return "C";
         case "35": return "D";
         case "36":
         case "37":
         case "38":
         case "39": return "E";
         case "41":
         case "42":
         case "43": return "F";
         case "45":
         case "46":
         case "47": 
         case "58": 
         case "48": return "G";
         case "49":
         case "50":
         case "51":
         case "52":
         case "53": return "H";
         case "55":
         case "56": return "I";
         case "59":
         case "60":
         case "61":
         case "62":
         case "63": return "J";
         case "64":
         case "65":
         case "66": return "K";
         case "68": return "L";
         case "69":
         case "70":
         case "71":
         case "72":
         case "73":
         case "74":
         case "75": return "M";
         case "77":
         case "78":
         case "79":
         case "80":
         case "81":
         case "82": return "N";
         case "84": return "O";
         case "85": return "P";
         case "86":
         case "87":
         case "88": return "Q";
         case "90":
         case "91":
         case "92":
         case "93": return "R";
         case "94":
         case "95":
         case "96": return "S";
         case "97":
         case "98": return "T";
         case "99": return "U";

         default  : return "";
      }

   }

#endregion ePDV_URA XML Export

}

public class RptR_PDV_PDV            : RptR_PDV
{
   public bool IsPdvK;
   public bool PdvSchema_23;
   public bool PdvSchema_25;
   public bool PdvSchema_2013;
   public bool PdvSchema_2013_EU;
   public bool PdvSchema_2014;
   public bool PdvSchema_2015;

   public RptR_PDV_PDV(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, bool isPdvK) : base(_reportDocument, _reportName, _rptFilter)
   {
      IsForExport = true;

      this.IsPdvK = isPdvK;

      this.PdvSchema_23      = _reportDocument is Vektor.Reports.RIZ.CR_PDVobrazac                                                                          ;
      this.PdvSchema_25      =(RptFilter.DatumDo <  ZXC.Date01012013 || ZXC.projectYear == "2012") && !PdvSchema_23                                         ;
    //this.PdvSchema_2013    = RptFilter.DatumOd >= ZXC.Date01012013  &&                                          !PdvSchema_23 && !PdvSchema_25            ;
      this.PdvSchema_2013    = RptFilter.DatumOd >= ZXC.Date01012013  && RptFilter.DatumOd < ZXC.PdvEU_EraDate && !PdvSchema_23 && !PdvSchema_25            ;
      this.PdvSchema_2013_EU = RptFilter.DatumOd >= ZXC.PdvEU_EraDate && RptFilter.DatumOd < ZXC.Date01012014                                               ; 
      this.PdvSchema_2014    = RptFilter.DatumOd >= ZXC.Date01012014  && RptFilter.DatumOd < ZXC.Date01012015/* 01.01.2014 */                                                                      ;
      this.PdvSchema_2015    = RptFilter.DatumOd >= ZXC.Date01012015  /* 01.01.2015 */                                                                      ;

      if(isPdvK && PdvSchema_2013)
      {
         PdvSchema_2013    = false;
         PdvSchema_2013_EU = true;
      }
   }

   public override int FillRiskReportLists()
   {
#region PdvK za 01.01.2013. do 31.12.2013.

      if(IsPdvK == true && PdvSchema_2013_EU == true) // PdvK za 01.01.2013. do 31.12.2013. 
      {
         Faktur faktur_rec_SumaRazdoblja_URA_01do06, faktur_rec_SumaRazdoblja_IRA_01do06, faktur_rec_SumaRazdoblja_URA_07do12, faktur_rec_SumaRazdoblja_IRA_07do12;

         RptFilter.DatumDo = ZXC.PdvEU_EraDate - ZXC.OneDaySpan; // 30.06.2013. 
         faktur_rec_SumaRazdoblja_URA_01do06 = LoadPdvSume(TheDbConnection, RptFilter.DatumOd, true , false);
         faktur_rec_SumaRazdoblja_IRA_01do06 = LoadPdvSume(TheDbConnection, RptFilter.DatumOd, false, false);

         RptFilter.DatumDo = ZXC.Date01012014 - ZXC.OneDaySpan; // 31.12.2013. 
         faktur_rec_SumaRazdoblja_URA_07do12 = LoadPdvSume(TheDbConnection, ZXC.PdvEU_EraDate, true , false);
         faktur_rec_SumaRazdoblja_IRA_07do12 = LoadPdvSume(TheDbConnection, ZXC.PdvEU_EraDate, false, false);

         PdvObrazacData pdv01do06 = new PdvObrazacData(faktur_rec_SumaRazdoblja_URA_01do06, faktur_rec_SumaRazdoblja_IRA_01do06, RptFilter);
         PdvObrazacData pdv07do12 = new PdvObrazacData(faktur_rec_SumaRazdoblja_URA_07do12, faktur_rec_SumaRazdoblja_IRA_07do12, RptFilter);

         Faktur_rec_SumaRazdoblja_URA = Sint2halfOf2013_URA(faktur_rec_SumaRazdoblja_URA_01do06, faktur_rec_SumaRazdoblja_URA_07do12, pdv01do06, pdv07do12);
         Faktur_rec_SumaRazdoblja_IRA = Sint2halfOf2013_IRA(faktur_rec_SumaRazdoblja_IRA_01do06, faktur_rec_SumaRazdoblja_IRA_07do12, pdv01do06, pdv07do12);
      }

#endregion PdvK za 01.01.2013. do 31.12.2013.

      else // svo ostali (classic i PdvK koji nije za 2013) 
      {
         Faktur_rec_SumaRazdoblja_URA = LoadPdvSume(TheDbConnection, RptFilter.DatumOd, true,  false);
         Faktur_rec_SumaRazdoblja_IRA = LoadPdvSume(TheDbConnection, RptFilter.DatumOd, false, false);
      }
      return 0;
   }

   private Faktur Sint2halfOf2013_URA(Faktur faktur_rec_SumaRazdoblja_URA_01do06, Faktur faktur_rec_SumaRazdoblja_URA_07do12, PdvObrazacData pdv01do06, PdvObrazacData pdv07do12)
   {
      Faktur faktur_2013_URA = /*new Faktur()*/ faktur_rec_SumaRazdoblja_URA_07do12.MakeDeepCopy();

      /* III.1. */ faktur_2013_URA.S_ukOsn25m = pdv07do12.ukOsn_HR + pdv01do06.ukOsn05i10mRP + pdv01do06.ukOsn22i23mRP + pdv01do06.ukOsn25mRP;
                   faktur_2013_URA.S_ukPdv25m = pdv07do12.ukPdv_HR + pdv01do06.ukPdv05i10mRP + pdv01do06.ukPdv22i23mRP + pdv01do06.ukPdv25mRP;

      /* III.6. */ faktur_2013_URA.S_ukOsnUr25 = pdv07do12.ukOsn_WO + pdv01do06.ukOsnUr23i25i05;
                   faktur_2013_URA.S_ukPdvUr25 = pdv07do12.ukPdv_WO + pdv01do06.ukPdvUr23i25i05;


      /////* III.5. */ faktur_2013_URA.S_ukOsn25m_BS += (faktur_rec_SumaRazdoblja_URA_01do06.S_ukOsnUu10 + faktur_rec_SumaRazdoblja_URA_01do06.S_ukOsnUu25);
      ////             faktur_2013_URA.S_ukPdv25m_BS += (faktur_rec_SumaRazdoblja_URA_01do06.S_ukPdvUu10 + faktur_rec_SumaRazdoblja_URA_01do06.S_ukPdvUu25);
                   
      // za ispravak pretporeza u prvih 6 mjeseci da ne ide u duplo
      /* II.11. */ faktur_2013_URA.S_ukOsnUu10 = faktur_rec_SumaRazdoblja_URA_01do06.S_ukOsnUu10 + faktur_rec_SumaRazdoblja_URA_07do12.S_ukOsnUu10 /*+ faktur_rec_SumaRazdoblja_URA_07do12.S_ukOsn10m_BS*/;
                   faktur_2013_URA.S_ukPdvUu10 = faktur_rec_SumaRazdoblja_URA_01do06.S_ukPdvUu10 + faktur_rec_SumaRazdoblja_URA_07do12.S_ukPdvUu10 /*+ faktur_rec_SumaRazdoblja_URA_07do12.S_ukPdv10m_BS*/;
      /* II.11. */ faktur_2013_URA.S_ukOsnUu25 = faktur_rec_SumaRazdoblja_URA_01do06.S_ukOsnUu25 + faktur_rec_SumaRazdoblja_URA_07do12.S_ukOsnUu25 /*+ faktur_rec_SumaRazdoblja_URA_07do12.S_ukOsn25m_BS*/;
                   faktur_2013_URA.S_ukPdvUu25 = faktur_rec_SumaRazdoblja_URA_01do06.S_ukPdvUu25 + faktur_rec_SumaRazdoblja_URA_07do12.S_ukPdvUu25 /*+ faktur_rec_SumaRazdoblja_URA_07do12.S_ukPdv25m_BS*/;

      return faktur_2013_URA;
   }

   private Faktur Sint2halfOf2013_IRA(Faktur faktur_rec_SumaRazdoblja_IRA_01do06, Faktur faktur_rec_SumaRazdoblja_IRA_07do12, PdvObrazacData pdv01do06, PdvObrazacData pdv07do12)
   {
      Faktur faktur_2013_IRA = /*new Faktur()*/ faktur_rec_SumaRazdoblja_IRA_07do12.MakeDeepCopy();

      /* I.8. */ faktur_2013_IRA.S_ukOsn14 += (faktur_rec_SumaRazdoblja_IRA_01do06.S_ukOsn09 + faktur_rec_SumaRazdoblja_IRA_01do06.S_ukOsn10);
      /* I.9. */ faktur_2013_IRA.S_ukOsn15 +=  faktur_rec_SumaRazdoblja_IRA_01do06.S_ukOsn08;
      /* I.10.*/ faktur_2013_IRA.S_ukOsn16 += (faktur_rec_SumaRazdoblja_IRA_01do06.S_ukOsn07 + faktur_rec_SumaRazdoblja_IRA_01do06.S_ukOsn11 + faktur_rec_SumaRazdoblja_IRA_01do06.S_ukOsn0);

      /* II.1 */ faktur_2013_IRA.S_ukOsn05m += (faktur_rec_SumaRazdoblja_IRA_01do06.S_ukOsn05m     + faktur_rec_SumaRazdoblja_IRA_01do06.S_ukOsn10m);
      /* II.1 */ faktur_2013_IRA.S_ukPdv05m += (faktur_rec_SumaRazdoblja_IRA_01do06.S_ukPdv05m     + faktur_rec_SumaRazdoblja_IRA_01do06.S_ukPdv10m);
      /* II.3 */ faktur_2013_IRA.S_ukOsn25m  = pdv07do12.ukOsn222325_HR + pdv01do06.ukOsn22i23_Ira + faktur_rec_SumaRazdoblja_IRA_01do06.S_ukOsn25m;
      /* II.3 */ faktur_2013_IRA.S_ukPdv25m  = pdv07do12.ukPdv222325_HR + pdv01do06.ukPdv22i23_Ira + faktur_rec_SumaRazdoblja_IRA_01do06.S_ukPdv25m;
      
      return faktur_2013_IRA;
   }

#region ePDV XML Export

   // ovdje nisi nasao nikakve upute kako bi se trebao zvati fajl pa ga imenujes proizvoljno 
   public override string ExportFileName
   {
      get
      {
         string mmyyyy = RptFilter.DatumOd.Month.ToString("00") + RptFilter.DatumOd.Year.ToString("0000");
         string   yyyy =                                          RptFilter.DatumOd.Year.ToString("0000");

         if(IsPdvK) return "PDVK_" + ZXC.CURR_prjkt_rec.Oib + "_" +   yyyy + ".xml";
         else       return "PDV_"  + ZXC.CURR_prjkt_rec.Oib + "_" + mmyyyy + ".xml";
      }
   }

#region Xml Schema Validation

   public override bool ExecuteExportValidation(string fileName)
   {
      List<ZXC.VvXmlValidationData> valDataList = new List<ZXC.VvXmlValidationData>();
      
      if(IsPdvK) // 06.02.2013: op.a. PDV-K Schema zaostaje za 1 za PDV schemom 
      {
         if(PdvSchema_23)
         {
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVK/v3-0", @"XSD\ObrazacPDVK-v3-0.xsd"          ));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVK/v3-0", @"XSD\ObrazacPDVKtipovi-v3-0.xsd"    ));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"          , @"XSD\ObrazacPDVKmetapodaci-v3-0.xsd"));
         }
         else if(PdvSchema_25) 
         {
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVK/v4-0", @"XSD\ObrazacPDVK-v4-0.xsd"));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVK/v4-0", @"XSD\ObrazacPDVKtipovi-v4-0.xsd"));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"          , @"XSD\ObrazacPDVKmetapodaci-v4-0.xsd"));
         }
         else if(PdvSchema_2013) 
         {
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVK/v5-0", @"XSD\ObrazacPDVK-v5-0.xsd"));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVK/v5-0", @"XSD\ObrazacPDVKtipovi-v5-0.xsd"));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"          , @"XSD\ObrazacPDVKmetapodaci-v5-0.xsd"));
         }
         else if(PdvSchema_2013_EU)
         {
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVK/v6-0", @"XSD\ObrazacPDVK-v6-0.xsd"));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVK/v6-0", @"XSD\ObrazacPDVKtipovi-v6-0.xsd"));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"          , @"XSD\ObrazacPDVKmetapodaci-v6-0.xsd"));
            //throw new Exception("PDVK EU NEZAVRŠEN!");
         }
         else if(PdvSchema_2014)
         {
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVK/v7-0", @"XSD\ObrazacPDVK-v7-0.xsd"));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVK/v7-0", @"XSD\ObrazacPDVKtipovi-v7-0.xsd"));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"          , @"XSD\ObrazacPDVKmetapodaci-v7-0.xsd"));
            //throw new Exception("PDVK EU NEZAVRŠEN!");
         }
      }
      else       
      {
         if(PdvSchema_23)
         {
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v4-0", @"XSD\ObrazacPDV-v4-0.xsd"          ));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v4-0", @"XSD\ObrazacPDVtipovi-v4-0.xsd"    ));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\ObrazacPDVmetapodaci-v4-0.xsd"));
         }
         else if(PdvSchema_25)
         {
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v5-0", @"XSD\ObrazacPDV-v5-0.xsd"          ));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v5-0", @"XSD\ObrazacPDVtipovi-v5-0.xsd"    ));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\ObrazacPDVmetapodaci-v5-0.xsd"));
         }
         else if(PdvSchema_2013)
         {
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v6-0", @"XSD\ObrazacPDV-v6-0.xsd"          ));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v6-0", @"XSD\ObrazacPDVtipovi-v6-0.xsd"    ));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\ObrazacPDVmetapodaci-v6-0.xsd"));
         }
         else if(PdvSchema_2013_EU)
         {
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v7-0", @"XSD\ObrazacPDV-v7-0.xsd"          ));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v7-0", @"XSD\ObrazacPDVtipovi-v7-0.xsd"    ));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\ObrazacPDVmetapodaci-v7-0.xsd"));
         }
         else if(PdvSchema_2014)
         {
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v8-0", @"XSD\ObrazacPDV-v8-0.xsd"          ));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v8-0", @"XSD\ObrazacPDVtipovi-v8-0.xsd"    ));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\ObrazacPDVmetapodaci-v8-0.xsd"));
         }
         else if(PdvSchema_2015)
         {
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v9-0", @"XSD\ObrazacPDV-v9-0.xsd"          ));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v9-0", @"XSD\ObrazacPDVtipovi-v9-0.xsd"    ));
            valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\ObrazacPDVmetapodaci-v9-0.xsd"));
         }
      }
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\MetapodaciTipovi-v2-0.xsd"));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/TemeljniTipovi/v2-1"     , @"XSD\TemeljniTipovi-v2-1.xsd"      ));

      return ExecuteExportValidation_Base(valDataList);
   }

#endregion Xml Schema Validation
   
   public override bool ExecuteExport(string fileName)
   {
#region Initialize XmlWriterSettings

      if(Faktur_rec_SumaRazdoblja_URA == null || Faktur_rec_SumaRazdoblja_IRA == null) throw new Exception("Nema se što exporitrati!");

      string ident = "   ";

      XmlWriterSettings settings = new XmlWriterSettings();
      
      settings.Indent = true;
      settings.IndentChars = ident;

#endregion Initialize XmlWriterSettings

      using(XmlWriter writer = XmlWriter.Create(fileName, settings))
      {
#region Init Xml Document

         writer.WriteStartDocument();

         if(IsPdvK)
         {
            if(PdvSchema_23)
            {
               writer.WriteStartElement("ObrazacPDVK", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVK/v3-0");
               writer.WriteAttributeString("verzijaSheme",                                                             "3.0");
            }
            else if(PdvSchema_25)
            {
               writer.WriteStartElement("ObrazacPDVK", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVK/v4-0");
               writer.WriteAttributeString("verzijaSheme",                                                             "4.0");
            }
            else if(PdvSchema_2013)
            {
               writer.WriteStartElement("ObrazacPDVK", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVK/v5-0");
               writer.WriteAttributeString("verzijaSheme",                                                             "5.0");
            }
            else if(PdvSchema_2013_EU) 
            {
               writer.WriteStartElement("ObrazacPDVK", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVK/v6-0");
               writer.WriteAttributeString("verzijaSheme",                                                             "6.0");
               //throw new Exception("PDVK EU NEZAVRŠEN!");
            }
            else if(PdvSchema_2014) 
            {
               writer.WriteStartElement("ObrazacPDVK", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVK/v7-0");
               writer.WriteAttributeString("verzijaSheme",                                                             "7.0");
            }
         }
         else // classic 
         {
            if(PdvSchema_23)
            {
               writer.WriteStartElement("ObrazacPDV", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v4-0");
               writer.WriteAttributeString("verzijaSheme",                                                           "4.0");
            }
            else if(PdvSchema_25)
            {
               writer.WriteStartElement("ObrazacPDV", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v5-0");
               writer.WriteAttributeString("verzijaSheme",                                                           "5.0");
            }
            else if(PdvSchema_2013)
            {
               writer.WriteStartElement("ObrazacPDV", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v6-0");
               writer.WriteAttributeString("verzijaSheme",                                                           "6.0");
            }
            else if(PdvSchema_2013_EU)
            {
               writer.WriteStartElement("ObrazacPDV", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v7-0");
               writer.WriteAttributeString("verzijaSheme",                                                           "7.0");
            }
            else if(PdvSchema_2014)
            {
               writer.WriteStartElement("ObrazacPDV", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v8-0");
               writer.WriteAttributeString("verzijaSheme",                                                           "8.0");
            }
            else if(PdvSchema_2015)
            {
               writer.WriteStartElement("ObrazacPDV", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDV/v9-0");
               writer.WriteAttributeString("verzijaSheme",                                                           "9.0");
            }
         }

#endregion Init Xml Document

#region Metapodaci

         writer.WriteStartElement("Metapodaci", @"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0");
            //writer.WriteElementString("Naslov", @"http://purl.org/dc/elements/1.1/title", "Prijava poreza na dodanu vrijednost");
            writer.WriteRaw("\n");
            writer.WriteRaw(ident + ident + "<Naslov dc=\"http://purl.org/dc/elements/1.1/title\">Prijava poreza na dodanu vrijednost</Naslov>\n");
            writer.WriteRaw(ident + ident + "<Autor dc=\"http://purl.org/dc/elements/1.1/creator\">" + ZXC.CURR_prjkt_rec.Ime + " " + ZXC.CURR_prjkt_rec.Prezime + "</Autor>\n");
            writer.WriteRaw(ident + ident + "<Datum dc=\"http://purl.org/dc/elements/1.1/date\">" + DateTime.Now.ToString("s") + "</Datum>\n");
            writer.WriteRaw(ident + ident + "<Format dc=\"http://purl.org/dc/elements/1.1/format\">text/xml</Format>\n");
            writer.WriteRaw(ident + ident + "<Jezik dc=\"http://purl.org/dc/elements/1.1/language\">hr-HR</Jezik>\n");
            writer.WriteRaw(ident + ident + "<Identifikator dc=\"http://purl.org/dc/elements/1.1/identifier\">" + Guid.NewGuid().ToString("D")/*.ToUpper()*/ + "</Identifikator>\n");

            if(IsPdvK)
            {
               if(PdvSchema_23)           writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPDVK-v3-0</Uskladjenost>\n");
               else if(PdvSchema_25)      writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPDVK-v4-0</Uskladjenost>\n");
               else if(PdvSchema_2013)    writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPDVK-v5-0</Uskladjenost>\n");
               else if(PdvSchema_2013_EU) writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPDVK-v6-0</Uskladjenost>\n");
               else if(PdvSchema_2014)    writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPDVK-v7-0</Uskladjenost>\n");
            }
            else
            {
               if(PdvSchema_23)           writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPDV-v4-0</Uskladjenost>\n");
               else if(PdvSchema_25)      writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPDV-v5-0</Uskladjenost>\n");
               else if(PdvSchema_2013)    writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPDV-v6-0</Uskladjenost>\n");
               else if(PdvSchema_2013_EU) writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPDV-v7-0</Uskladjenost>\n");
               else if(PdvSchema_2014)    writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPDV-v8-0</Uskladjenost>\n");
               else if(PdvSchema_2015)    writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPDV-v9-0</Uskladjenost>\n");
            }

            writer.WriteRaw(ident + ident + "<Tip dc=\"http://purl.org/dc/elements/1.1/type\">Elektronički obrazac</Tip>\n");
            writer.WriteRaw(ident + ident + "<Adresant>Ministarstvo Financija, Porezna uprava, Zagreb</Adresant>\n");
            writer.WriteRaw(ident);
         writer.WriteEndElement(); // Metapodaci 

#endregion Write Header Data

#region Zaglavlje

         writer.WriteStartElement("Zaglavlje");

            writer.WriteStartElement("Razdoblje");
               writer.WriteElementString("DatumOd", RptFilter.DatumOd.ToString("s").Substring(0, 10));
               writer.WriteElementString("DatumDo", RptFilter.DatumDo.ToString("s").Substring(0, 10));
            writer.WriteEndElement(); // Razdoblje 

            writer.WriteStartElement("Obveznik");
               writer.WriteElementString("Naziv", ZXC.CURR_prjkt_rec.Naziv);
               writer.WriteElementString("OIB", ZXC.CURR_prjkt_rec.Oib);
               if(PdvSchema_2013_EU == false && PdvSchema_2014 == false && PdvSchema_2015 == false) 
               writer.WriteElementString("SifraDjelatnosti", ZXC.CURR_prjkt_rec.SifDcd);
               writer.WriteStartElement("Adresa");
                  writer.WriteElementString("Mjesto", ZXC.CURR_prjkt_rec.Grad);
                  writer.WriteElementString("Ulica", ZXC.CURR_prjkt_rec.UlicaBezBroja_1);
                  writer.WriteElementString("Broj", ZXC.CURR_prjkt_rec.UlicniBroj_1);
               writer.WriteEndElement(); // Adresa 
            writer.WriteEndElement(); // Obveznik 

            writer.WriteStartElement("ObracunSastavio");
               writer.WriteElementString("Ime"    , ZXC.CURR_prjkt_rec.Ime);
               writer.WriteElementString("Prezime", ZXC.CURR_prjkt_rec.Prezime);
               if(PdvSchema_2015 == false)
               writer.WriteElementString("Telefon", /*ZXC.CURR_prjkt_rec.Tel1 zbog validacije:*/ZXC.CURR_prjkt_rec.Tel2);
               if(PdvSchema_2015 == false)
               writer.WriteElementString("Email", ZXC.CURR_prjkt_rec.Email);
            writer.WriteEndElement(); // ObracunSastavio 
            
         writer.WriteElementString("Ispostava", ZXC.CURR_prjkt_rec.OpcCd);

         writer.WriteEndElement(); // Zaglavlje 

#endregion Zaglavlje

#region Tijelo

#region SET Tijelo Values

#region Init variablez
         
         decimal p000, p100, p110, p120, p121, p122, p123, p124, p130;
         decimal p200vr, p200pdv, p210vr, p210pdv, p220vr, p220pdv, p230vr, p230pdv, p240vr, p240pdv;
         decimal p300vr, p300pdv, p310vr, p310pdv, p320vr, p320pdv, p330vr, p330pdv, p340vr, p340pdv, p350vr, p350pdv, p360vr, p360pdv, p370vr, p370pdv, p380;
         decimal p400  , p500   , p600;
         decimal p350nePdv, p360nePdv, p370nePdv;

         decimal k710=0.00M, k711=0.00M, k712=0.00M, k713=0.00M, k714=0.00M, k715=0.00M, k716=0.00M, k720=0.00M, k730=0.00M, k740=0.00M, k750=0.00M, k760=0.00M,
                 PretporezOdb=0.00M; //11.02.2013. novi podatak
         decimal k810=0.00M, k811=0.00M, k812=0.00M, k813=0.00M, k814=0.00M, k815=0.00M, k816=0.00M, k820=0.00M, k830=0.00M, k840=0.00M;
         decimal k831vr=0.00M, k832vr=0.00M, k833vr=0.00M, k850=0.00M, k860=0.00M; 
         int     k831br=0, k832br=0, k833br=0; 
         bool    k870=false;

         decimal
            p101     = 0.00M,
            p102     = 0.00M,
            p103     = 0.00M,
            p104     = 0.00M,
            p105     = 0.00M,
            p106     = 0.00M,
            p107     = 0.00M,
            p108     = 0.00M,
            p109     = 0.00M,
            p201vr   = 0.00M,
            p201pdv  = 0.00M,
            p202vr   = 0.00M,
            p202pdv  = 0.00M,
            p203vr   = 0.00M,
            p203pdv  = 0.00M,
            p204vr   = 0.00M,
            p204pdv  = 0.00M,
            p205vr   = 0.00M,
            p205pdv  = 0.00M,
            p206vr   = 0.00M,
            p206pdv  = 0.00M,
            p207vr   = 0.00M,
            p207pdv  = 0.00M,
            p208vr   = 0.00M,
            p208pdv  = 0.00M,
            p209vr   = 0.00M,
            p209pdv  = 0.00M,
            p211vr   = 0.00M,
            p211pdv  = 0.00M,
            p212vr   = 0.00M,
            p212pdv  = 0.00M,
            p213vr   = 0.00M,
            p213pdv  = 0.00M,
            p214vr   = 0.00M,
            p214pdv  = 0.00M,
            p215vr   = 0.00M,
            p215pdv  = 0.00M,
            p301vr   = 0.00M,
            p301pdv  = 0.00M,
            p302vr   = 0.00M,
            p302pdv  = 0.00M,
            p303vr   = 0.00M,
            p303pdv  = 0.00M,
            p304vr   = 0.00M,
            p304pdv  = 0.00M,
            p305vr   = 0.00M,
            p305pdv  = 0.00M,
            p306vr   = 0.00M,
            p306pdv  = 0.00M,
            p307     = 0.00M,
            p307vr   = 0.00M,
            p307pdv  = 0.00M,
            p308vr   = 0.00M,
            p308pdv  = 0.00M,
            p309vr   = 0.00M,
            p309pdv  = 0.00M,
            p311vr   = 0.00M,
            p311pdv  = 0.00M,
            p312vr   = 0.00M,
            p312pdv  = 0.00M,
            p313vr   = 0.00M,
            p313pdv  = 0.00M,
            p314vr   = 0.00M,
            p314pdv  = 0.00M,
            p315     = 0.00M,
            p700     = 0.00M;

         PdvObrazacData pdv = new PdvObrazacData(Faktur_rec_SumaRazdoblja_URA, Faktur_rec_SumaRazdoblja_IRA, RptFilter);

#endregion Init variablez

         if(PdvSchema_23)
         {
#region PdvSchema_23
            p000    = pdv.ukOsnIRA                            .Ron2();
            p100    = pdv.ukOsnPdvObrI                        .Ron2();
            p110    = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn07  .Ron2();
            p120    = pdv.ukOsnPdvObrI2                       .Ron2();
            p121    = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn08  .Ron2();
            p122    = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn09  .Ron2();
            p123    = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn10  .Ron2();
            p124    = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn11  .Ron2();
            p130    = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn0   .Ron2();
            p200vr  = pdv.ukOsnPdvObrII                       .Ron2();
            p200pdv = pdv.ukPdvIRA                            .Ron2();
            p210vr  = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn10m .Ron2();
            p210pdv = Faktur_rec_SumaRazdoblja_IRA.S_ukPdv10m .Ron2();
            p220vr  = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn22m .Ron2();
            p220pdv = Faktur_rec_SumaRazdoblja_IRA.S_ukPdv22m .Ron2();
            p230vr  = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn23m .Ron2();
            p230pdv = Faktur_rec_SumaRazdoblja_IRA.S_ukPdv23m .Ron2();
            p240vr  = pdv.ukOsnPdvF_obrII4                    .Ron2();
            p240pdv = pdv.ukPdvPdvF_obrII4                    .Ron2();
            p300vr  = pdv.ukOsnURA                            .Ron2();
            p300pdv = pdv.ukPdvURA                            .Ron2();
            p310vr  = pdv.ukOsn05i10mRP                       .Ron2();
            p310pdv = pdv.ukPdv05i10mRP                       .Ron2();
            p320vr  = pdv.ukOsn22mRP                          .Ron2();
            p320pdv = pdv.ukPdv22mRP                          .Ron2();
            p330vr  = pdv.ukOsn23mRP                          .Ron2();
            p330pdv = pdv.ukPdv23mRP                          .Ron2();
            p340vr  = Faktur_rec_SumaRazdoblja_URA.S_ukOsnUr23.Ron2();
            p340pdv = Faktur_rec_SumaRazdoblja_URA.S_ukPdvUr23.Ron2();
            p350vr  = Faktur_rec_SumaRazdoblja_URA.S_ukOsnUu10.Ron2();
            p350pdv = Faktur_rec_SumaRazdoblja_URA.S_ukPdvUu10.Ron2();
         p350nePdv  = 0.00M;
            p360vr  = Faktur_rec_SumaRazdoblja_URA.S_ukOsnUu22.Ron2();
            p360pdv = Faktur_rec_SumaRazdoblja_URA.S_ukPdvUu22.Ron2();
         p360nePdv  = 0.00M;
            p370vr  = Faktur_rec_SumaRazdoblja_URA.S_ukOsnUu23.Ron2();
            p370pdv = Faktur_rec_SumaRazdoblja_URA.S_ukPdvUu23.Ron2();
         p370nePdv  = 0.00M;
            p380    = pdv.pdvIspravak_obrIII8                 .Ron2();
            p400    = System.Math.Abs(pdv.porObveza)          .Ron2();
            p500    = -1.00M * pdv.porezKredit                .Ron2();
            p600    = System.Math.Abs(pdv.ukRazlika)          .Ron2();
#endregion PdvSchema_23
         }

         else if(PdvSchema_25 || PdvSchema_2013)
         {
#region PdvSchema_25 || PdvSchema_2013
            p000    = pdv.ukOsnIRA                            .Ron2();
            p100    = pdv.ukOsnPdvObrI                        .Ron2();
            p110    = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn07  .Ron2();
            p120    = pdv.ukOsnPdvObrI2                       .Ron2();
            p121    = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn08  .Ron2();
            p122    = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn09  .Ron2();
            p123    = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn10  .Ron2();
            p124    = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn11  .Ron2();
            p130    = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn0   .Ron2();
            p200vr  = pdv.ukOsnPdvObrII                       .Ron2();
            p200pdv = pdv.ukPdvIRA                            .Ron2();
          //p210vr  = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn10m .Ron2();
          //p210pdv = Faktur_rec_SumaRazdoblja_IRA.S_ukPdv10m .Ron2();
            p210vr  = pdv.ukOsn05i10_Ira                      .Ron2();
            p210pdv = pdv.ukPdv05i10_Ira                      .Ron2();
            p220vr  = pdv.ukOsn22i23_Ira                      .Ron2();
            p220pdv = pdv.ukPdv22i23_Ira                      .Ron2();
            p230vr  = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn25m .Ron2();
            p230pdv = Faktur_rec_SumaRazdoblja_IRA.S_ukPdv25m .Ron2();
            p240vr  = pdv.ukOsnPdvF_obrII4                    .Ron2();
            p240pdv = pdv.ukPdvPdvF_obrII4                    .Ron2();
            p300vr  = pdv.ukOsnURA                            .Ron2();
            p300pdv = pdv.ukPdvURA                            .Ron2();
            p310vr  = pdv.ukOsn05i10mRP                       .Ron2();
            p310pdv = pdv.ukPdv05i10mRP                       .Ron2();
            p320vr  = pdv.ukOsn22i23mRP                       .Ron2();
            p320pdv = pdv.ukPdv22i23mRP                       .Ron2();
            p330vr  = pdv.ukOsn25mRP                          .Ron2();
            p330pdv = pdv.ukPdv25mRP                          .Ron2();
          //p340vr  = Faktur_rec_SumaRazdoblja_URA.S_ukOsnUr25.Ron2();  16.04.2012.
          //p340pdv = Faktur_rec_SumaRazdoblja_URA.S_ukPdvUr25.Ron2();
            p340vr  = pdv.ukOsnUr23i25i05                     .Ron2();
            p340pdv = pdv.ukPdvUr23i25i05                     .Ron2();
            p350vr  = Faktur_rec_SumaRazdoblja_URA.S_ukOsnUu10.Ron2();
            p350pdv = Faktur_rec_SumaRazdoblja_URA.S_ukPdvUu10.Ron2();
         p350nePdv  = pdv.porNeOdb350;
            p360vr  = pdv.ukOsnUu22i23.Ron2();
            p360pdv = pdv.ukPdvUu22i23.Ron2();
         p360nePdv  = pdv.porNeOdb360;
            p370vr  = Faktur_rec_SumaRazdoblja_URA.S_ukOsnUu25.Ron2();
            p370pdv = Faktur_rec_SumaRazdoblja_URA.S_ukPdvUu25.Ron2();
         p370nePdv  = pdv.porNeOdb370;
            p380    = pdv.pdvIspravak_obrIII8                 .Ron2();
          //p400    = System.Math.Abs(pdv.porObveza)          .Ron2(); 16.04.2012.
            p400    = pdv.porObveza                           .Ron2();
            p500    = -1.00M * pdv.porezKredit                .Ron2();
          //p600    = System.Math.Abs(pdv.ukRazlika)          .Ron2(); 16.04.2012.
            p600    = pdv.ukRazlika                           .Ron2();
#endregion PdvSchema_25 || PdvSchema_2013
         }

         // PDV EU --------- START ---------------------------------------
         else if(PdvSchema_2013_EU)
         {
#region PdvSchema_2013_EU
            p000 =  p100 =  p110 =  p120 =  p121 =  p122 =  p123 =  p124 =  p130 =
            p200vr =  p200pdv =  p210vr =  p210pdv =  p220vr =  p220pdv =  p230vr =  p230pdv =  p240vr =  p240pdv =
            p300vr =  p300pdv =  p310vr =  p310pdv =  p320vr =  p320pdv =  p330vr =  p330pdv =  p340vr =  p340pdv =  p350vr =  p350pdv =  p360vr =  p360pdv =  p370vr =  p370pdv =  p380=
            p400 =  p500 =  p600 =
            p350nePdv =  p360nePdv =  p370nePdv = 0.00M;
            
            if(((RptR_PDV_PDV)this).IsPdvK) // za PDV k 2013
            {
               pdv.ukOsn222325_HR = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn25m;
               pdv.ukPdv222325_HR = Faktur_rec_SumaRazdoblja_IRA.S_ukPdv25m;

               pdv.sumIIOsn = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn05m + Faktur_rec_SumaRazdoblja_IRA.S_ukOsn10m + Faktur_rec_SumaRazdoblja_IRA.S_ukOsn25m +
                                   pdv.ukOsn25mn_TP + pdv.ukOsn10mn_BS + pdv.ukOsn25mn_BS +
                                   pdv.ukOsnR05mn_EU + pdv.ukOsnR10mn_EU + pdv.ukOsnR25mn_EU +
                                   pdv.ukOsnU05mn_EU + pdv.ukOsnU10mn_EU + pdv.ukOsnU25mn_EU +
                                   pdv.ukOsnObr_WO + pdv.ukOsnPdvF_obrII4;

               pdv.sumIIPdv = Faktur_rec_SumaRazdoblja_IRA.S_ukPdv05m + Faktur_rec_SumaRazdoblja_IRA.S_ukPdv10m + Faktur_rec_SumaRazdoblja_IRA.S_ukPdv25m +
                                     pdv.ukPdv25mn_TP + pdv.ukPdv10mn_BS + pdv.ukPdv25mn_BS +
                                     pdv.ukPdvR05mn_EU + pdv.ukPdvR10mn_EU + pdv.ukPdvR25mn_EU +
                                     pdv.ukPdvU05mn_EU + pdv.ukPdvU10mn_EU + pdv.ukPdvU25mn_EU +
                                     pdv.ukPdvObr_WO + pdv.ukPdvPdvF_obrII4;

               pdv.sumIiII = pdv.sumI + pdv.sumIIOsn;

               pdv.ukOsn_HR = this.Faktur_rec_SumaRazdoblja_URA.S_ukOsn25m;
               pdv.ukPdv_HR = this.Faktur_rec_SumaRazdoblja_URA.S_ukPdv25m;

               pdv.sumIIIOsn = pdv.ukOsn_HR + pdv.ukOsnRm_EU + pdv.ukOsnUm_EU + pdv.ukOsnm_BS + pdv.ukOsn_WO + Faktur_rec_SumaRazdoblja_URA.S_ukOsn25m_TP;
               pdv.sumIIIPdv = pdv.ukPdv_HR + pdv.ukPdvRm_EU + pdv.ukPdvUm_EU + pdv.ukPdvm_BS + pdv.ukPdv_WO + Faktur_rec_SumaRazdoblja_URA.S_ukPdv25m_TP + pdv.pdvIspravak_obrIII8;

               pdv.obvezaPDV = pdv.sumIIPdv - pdv.sumIIIPdv;
               pdv.razlikaUplPov = pdv.obvezaPDV - pdv.porezKredit;
            }

            p000     = pdv.sumIiII                            .Ron2();
            p100     = pdv.sumI                               .Ron2();
            p101     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn07 .Ron2(); 
            p102     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn08 .Ron2();
            p103     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn09 .Ron2();
            p104     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn10 .Ron2();
            p105     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn11 .Ron2();
            p106     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn12 .Ron2();
            p107     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn13 .Ron2();
            p108     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn14 .Ron2();
            p109     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn15 .Ron2();
            p110     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn16 .Ron2();
            p200vr   = pdv.sumIIOsn                           .Ron2();
            p200pdv  = pdv.sumIIPdv                           .Ron2();
            p201vr   = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn05m.Ron2();
            p201pdv  = Faktur_rec_SumaRazdoblja_IRA.S_ukPdv05m.Ron2();
            p202vr   = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn10m.Ron2();
            p202pdv  = Faktur_rec_SumaRazdoblja_IRA.S_ukPdv10m.Ron2();
            p203vr   = pdv.ukOsn222325_HR                     .Ron2();
            p203pdv  = pdv.ukPdv222325_HR                     .Ron2();
            p204vr   = pdv.ukOsn25mn_TP                       .Ron2();
            p204pdv  = pdv.ukPdv25mn_TP                       .Ron2();
            p205vr   = pdv.ukOsnR05mn_EU                      .Ron2();
            p205pdv  = pdv.ukPdvR05mn_EU                      .Ron2();
            p206vr   = pdv.ukOsnR10mn_EU                      .Ron2();
            p206pdv  = pdv.ukPdvR10mn_EU                      .Ron2();
            p207vr   = pdv.ukOsnR25mn_EU                      .Ron2();
            p207pdv  = pdv.ukPdvR25mn_EU                      .Ron2();
            p208vr   = pdv.ukOsnU05mn_EU                      .Ron2();
            p208pdv  = pdv.ukPdvU05mn_EU                      .Ron2();
            p209vr   = pdv.ukOsnU10mn_EU                      .Ron2();
            p209pdv  = pdv.ukPdvU10mn_EU                      .Ron2();
            p210vr   = pdv.ukOsnU25mn_EU                      .Ron2();
            p210pdv  = pdv.ukPdvU25mn_EU                      .Ron2();
            p211vr   = pdv.ukOsn10mn_BS                       .Ron2();
            p211pdv  = pdv.ukPdv10mn_BS                       .Ron2();
            p212vr   = pdv.ukOsn25mn_BS                       .Ron2();
            p212pdv  = pdv.ukPdv25mn_BS                       .Ron2();
            p213vr   = pdv.ukOsnPdvF_obrII4                   .Ron2();
            p213pdv  = pdv.ukPdvPdvF_obrII4                   .Ron2();
            p214vr   = pdv.ukOsnObr_WO                        .Ron2();
            p214pdv  = pdv.ukPdvObr_WO                        .Ron2();
            p300vr   = pdv.sumIIIOsn                          .Ron2();
            p300pdv  = pdv.sumIIIPdv                          .Ron2();
            p301vr   = pdv.ukOsn_HR                           .Ron2();
            p301pdv  = pdv.ukPdv_HR                           .Ron2();
            p302vr   = pdv.ukOsnm_TP                          .Ron2();
            p302pdv  = pdv.ukPdvm_TP                          .Ron2();
            p303vr   = pdv.ukOsnRm_EU                         .Ron2();
            p303pdv  = pdv.ukPdvRm_EU                         .Ron2();
            p304vr   = pdv.ukOsnUm_EU                         .Ron2();
            p304pdv  = pdv.ukPdvUm_EU                         .Ron2();
            p305vr   = pdv.ukOsnm_BS                          .Ron2();
            p305pdv  = pdv.ukPdvm_BS                          .Ron2();
            p306vr   = pdv.ukOsn_WO                           .Ron2();
            p306pdv  = pdv.ukPdv_WO                           .Ron2();
            p307     = pdv.pdvIspravak_obrIII8                .Ron2();
            p400     = pdv.obvezaPDV                          .Ron2();
            p500     = -1.00M * pdv.porezKredit               .Ron2();
            p600     = pdv.razlikaUplPov                      .Ron2(); 
            p700     = 0.00M; // TODO: !!! 
#endregion PdvSchema_2013_EU
         }
         // PDV EU --------- END ----------------------------------------

         // PDV 2014 --------- START ---------------------------------------
         else if(PdvSchema_2014)
         {
#region PdvSchema_2014
            p000   =  p100    =  p110   =  p120    =  p121   =  p122    =  p123   =  p124    =  p130 =
            p200vr =  p200pdv =  p210vr =  p210pdv =  p220vr =  p220pdv =  p230vr =  p230pdv =  p240vr =  p240pdv =
            p300vr =  p300pdv =  p310vr =  p310pdv =  p320vr =  p320pdv =  p330vr =  p330pdv =  p340vr =  p340pdv =  p350vr =  p350pdv =  p360vr =  p360pdv =  p370vr =  p370pdv =  p380=
            p400   =  p500 =  p600 =
            p350nePdv =  p360nePdv =  p370nePdv = 0.00M;

            p000     = pdv.sumIiII                                .Ron2();
            p100     = pdv.sumI                                   .Ron2();
            p101     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn07     .Ron2(); 
            p102     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn08     .Ron2();
            p103     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn09     .Ron2();
            p104     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn10     .Ron2();
            p105     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn11     .Ron2();
            p106     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn12     .Ron2();
            p107     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn13     .Ron2();
            p108     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn14     .Ron2();
            p109     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn15     .Ron2();
            p110     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn16     .Ron2();
            p200vr   = pdv.sumIIOsn                               .Ron2();
            p200pdv  = pdv.sumIIPdv                               .Ron2();
            p201vr   = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn05m    .Ron2();
            p201pdv  = Faktur_rec_SumaRazdoblja_IRA.S_ukPdv05m    .Ron2();
            p202vr   = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn10m    .Ron2();
            p202pdv  = Faktur_rec_SumaRazdoblja_IRA.S_ukPdv10m    .Ron2();
            p203vr   = pdv.ukOsn222325_HR                         .Ron2();
            p203pdv  = pdv.ukPdv222325_HR                         .Ron2();
            p204vr   = pdv.ukOsn25mn_TP                           .Ron2();
            p204pdv  = pdv.ukPdv25mn_TP                           .Ron2();
            p205vr   = pdv.ukOsnR05mn_EU                          .Ron2();
            p205pdv  = pdv.ukPdvR05mn_EU                          .Ron2();
            p206vr   = pdv.ukOsnR10mn_EU                          .Ron2();
            p206pdv  = pdv.ukPdvR10mn_EU                          .Ron2();
            p207vr   = pdv.ukOsnR25mn_EU                          .Ron2();
            p207pdv  = pdv.ukPdvR25mn_EU                          .Ron2();
            p208vr   = pdv.ukOsnU05mn_EU                          .Ron2();
            p208pdv  = pdv.ukPdvU05mn_EU                          .Ron2();
            p209vr   = pdv.ukOsnU10mn_EU                          .Ron2();
            p209pdv  = pdv.ukPdvU10mn_EU                          .Ron2();
            p210vr   = pdv.ukOsnU25mn_EU                          .Ron2();
            p210pdv  = pdv.ukPdvU25mn_EU                          .Ron2();
            p211vr   = 0.00M; // TODO: !!!  S_ukOsn05m_BS + S_ukOsn05n_BS;
            p211pdv  = 0.00M; // TODO: !!!  S_ukPdv05m_BS + S_ukPdv05n_BS;
            p212vr   = pdv.ukOsn10mn_BS                           .Ron2();
            p212pdv  = pdv.ukPdv10mn_BS                           .Ron2();
            p213vr   = pdv.ukOsn25mn_BS                           .Ron2();
            p213pdv  = pdv.ukPdv25mn_BS                           .Ron2();
            p214vr   = pdv.ukOsnPdvF_obrII4                       .Ron2();
            p214pdv  = pdv.ukPdvPdvF_obrII4                       .Ron2();
            p215vr   = pdv.ukOsnObr_WO                            .Ron2();
            p215pdv  = pdv.ukPdvObr_WO                            .Ron2();
            p300vr   = pdv.sumIIIOsn                              .Ron2();
            p300pdv  = pdv.sumIIIPdv                              .Ron2();
            p301vr   = pdv.ukUlazOsn05_HR                         .Ron2();
            p301pdv  = pdv.ukUlazPdv05_HR                         .Ron2();
            p302vr   = pdv.ukUlazOsn13_HR                         .Ron2();
            p302pdv  = pdv.ukUlazPdv13_HR                         .Ron2();
            p303vr   = pdv.ukUlazOsn25_HR                         .Ron2();
            p303pdv  = pdv.ukUlazPdv25_HR                         .Ron2();
            p304vr   = pdv.ukOsnm_TP                              .Ron2();
            p304pdv  = pdv.ukPdvm_TP                              .Ron2();
            p305vr   = Faktur_rec_SumaRazdoblja_URA.S_ukOsnR05m_EU.Ron2();
            p305pdv  = Faktur_rec_SumaRazdoblja_URA.S_ukPdvR05m_EU.Ron2();
            p306vr   = Faktur_rec_SumaRazdoblja_URA.S_ukOsnR10m_EU.Ron2();
            p306pdv  = Faktur_rec_SumaRazdoblja_URA.S_ukPdvR10m_EU.Ron2();
            p307vr   = Faktur_rec_SumaRazdoblja_URA.S_ukOsnR25m_EU.Ron2();
            p307pdv  = Faktur_rec_SumaRazdoblja_URA.S_ukPdvR25m_EU.Ron2();
            p308vr   = Faktur_rec_SumaRazdoblja_URA.S_ukOsnU05m_EU.Ron2();
            p308pdv  = Faktur_rec_SumaRazdoblja_URA.S_ukPdvU05m_EU.Ron2();
            p309vr   = Faktur_rec_SumaRazdoblja_URA.S_ukOsnU10m_EU.Ron2();
            p309pdv  = Faktur_rec_SumaRazdoblja_URA.S_ukPdvU10m_EU.Ron2();
            p310vr   = Faktur_rec_SumaRazdoblja_URA.S_ukOsnU25m_EU.Ron2();
            p310pdv  = Faktur_rec_SumaRazdoblja_URA.S_ukPdvU25m_EU.Ron2();
            p311vr   = 0.00M; // TODO: !!!  S_ukOsn05m_BS
            p311pdv  = 0.00M; // TODO: !!!  S_ukPdv05m_BS
            p312vr   = Faktur_rec_SumaRazdoblja_URA.S_ukOsn10m_BS .Ron2();
            p312pdv  = Faktur_rec_SumaRazdoblja_URA.S_ukPdv10m_BS .Ron2();
            p313vr   = Faktur_rec_SumaRazdoblja_URA.S_ukOsn25m_BS .Ron2();
            p313pdv  = Faktur_rec_SumaRazdoblja_URA.S_ukPdv25m_BS .Ron2();
            p314vr   = pdv.ukOsn_WO                               .Ron2();
            p314pdv  = pdv.ukPdv_WO                               .Ron2();
            p315     = pdv.pdvIspravak_obrIII8                    .Ron2();
            p400     = pdv.obvezaPDV                              .Ron2();
            p500     = -1.00M * pdv.porezKredit                   .Ron2();
            p600     = pdv.razlikaUplPov                          .Ron2(); 
            p700     = 0.00M; // TODO: !!! 
#endregion PdvSchema_2014
         }
         // PDV 2014 --------- END ----------------------------------------

         // PDV 2015 --------- START ---------------------------------------
         else /*if(PdvSchema_2015)*/
         {
#region PdvSchema_2015

            p000   =  p100    =  p110   =  p120    =  p121   =  p122    =  p123   =  p124    =  p130 =
            p200vr =  p200pdv =  p210vr =  p210pdv =  p220vr =  p220pdv =  p230vr =  p230pdv =  p240vr =  p240pdv =
            p300vr =  p300pdv =  p310vr =  p310pdv =  p320vr =  p320pdv =  p330vr =  p330pdv =  p340vr =  p340pdv =  p350vr =  p350pdv =  p360vr =  p360pdv =  p370vr =  p370pdv =  p380=
            p400   =  p500 =  p600 =
            p350nePdv =  p360nePdv =  p370nePdv = 0.00M;

            p000     = pdv.sumIiII                                .Ron2();
            p100     = pdv.sumI                                   .Ron2();
            p101     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn07     .Ron2(); 
            p102     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn08     .Ron2();
            p103     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn09     .Ron2();
            p104     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn10     .Ron2();
            p105     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn11     .Ron2();
            p106     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn12     .Ron2();
            p107     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn13     .Ron2();
            p108     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn14     .Ron2();
            p109     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn15     .Ron2();
            p110     = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn16     .Ron2();
            p200vr   = pdv.sumIIOsn                               .Ron2();
            p200pdv  = pdv.sumIIPdv                               .Ron2();
            p201vr   = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn05m    .Ron2();
            p201pdv  = Faktur_rec_SumaRazdoblja_IRA.S_ukPdv05m    .Ron2();
            p202vr   = Faktur_rec_SumaRazdoblja_IRA.S_ukOsn10m    .Ron2();
            p202pdv  = Faktur_rec_SumaRazdoblja_IRA.S_ukPdv10m    .Ron2();
            p203vr   = pdv.ukOsn222325_HR                         .Ron2();
            p203pdv  = pdv.ukPdv222325_HR                         .Ron2();
            p204vr   = pdv.ukOsn25mn_TP                           .Ron2();
            p204pdv  = pdv.ukPdv25mn_TP                           .Ron2();
            p205vr   = pdv.ukOsnR05mn_EU                          .Ron2();
            p205pdv  = pdv.ukPdvR05mn_EU                          .Ron2();
            p206vr   = pdv.ukOsnR10mn_EU                          .Ron2();
            p206pdv  = pdv.ukPdvR10mn_EU                          .Ron2();
            p207vr   = pdv.ukOsnR25mn_EU                          .Ron2();
            p207pdv  = pdv.ukPdvR25mn_EU                          .Ron2();
            p208vr   = pdv.ukOsnU05mn_EU                          .Ron2();
            p208pdv  = pdv.ukPdvU05mn_EU                          .Ron2();
            p209vr   = pdv.ukOsnU10mn_EU                          .Ron2();
            p209pdv  = pdv.ukPdvU10mn_EU                          .Ron2();
            p210vr   = pdv.ukOsnU25mn_EU                          .Ron2();
            p210pdv  = pdv.ukPdvU25mn_EU                          .Ron2();
            p211vr   = 0.00M; // TODO: !!!  S_ukOsn05m_BS + S_ukOsn05n_BS;
            p211pdv  = 0.00M; // TODO: !!!  S_ukPdv05m_BS + S_ukPdv05n_BS;
            p212vr   = pdv.ukOsn10mn_BS                           .Ron2();
            p212pdv  = pdv.ukPdv10mn_BS                           .Ron2();
            p213vr   = pdv.ukOsn25mn_BS                           .Ron2();
            p213pdv  = pdv.ukPdv25mn_BS                           .Ron2();
            p214vr   = pdv.ukOsnPdvF_obrII4                       .Ron2();
            p214pdv  = pdv.ukPdvPdvF_obrII4                       .Ron2();
            p215vr   = pdv.ukOsnObr_WO                            .Ron2();
            p215pdv  = pdv.ukPdvObr_WO                            .Ron2();
            p300vr   = pdv.sumIIIOsn                              .Ron2();
            p300pdv  = pdv.sumIIIPdv                              .Ron2();
            p301vr   = pdv.ukUlazOsn05_HR                         .Ron2();
            p301pdv  = pdv.ukUlazPdv05_HR                         .Ron2();
            p302vr   = pdv.ukUlazOsn13_HR                         .Ron2();
            p302pdv  = pdv.ukUlazPdv13_HR                         .Ron2();
            p303vr   = pdv.ukUlazOsn25_HR                         .Ron2();
            p303pdv  = pdv.ukUlazPdv25_HR                         .Ron2();
            p304vr   = pdv.ukOsnm_TP                              .Ron2();
            p304pdv  = pdv.ukPdvm_TP                              .Ron2();
            p305vr   = Faktur_rec_SumaRazdoblja_URA.S_ukOsnR05m_EU.Ron2();
            p305pdv  = Faktur_rec_SumaRazdoblja_URA.S_ukPdvR05m_EU.Ron2();
            p306vr   = Faktur_rec_SumaRazdoblja_URA.S_ukOsnR10m_EU.Ron2();
            p306pdv  = Faktur_rec_SumaRazdoblja_URA.S_ukPdvR10m_EU.Ron2();
            p307vr   = Faktur_rec_SumaRazdoblja_URA.S_ukOsnR25m_EU.Ron2();
            p307pdv  = Faktur_rec_SumaRazdoblja_URA.S_ukPdvR25m_EU.Ron2();
            p308vr   = Faktur_rec_SumaRazdoblja_URA.S_ukOsnU05m_EU.Ron2();
            p308pdv  = Faktur_rec_SumaRazdoblja_URA.S_ukPdvU05m_EU.Ron2();
            p309vr   = Faktur_rec_SumaRazdoblja_URA.S_ukOsnU10m_EU.Ron2();
            p309pdv  = Faktur_rec_SumaRazdoblja_URA.S_ukPdvU10m_EU.Ron2();
            p310vr   = Faktur_rec_SumaRazdoblja_URA.S_ukOsnU25m_EU.Ron2();
            p310pdv  = Faktur_rec_SumaRazdoblja_URA.S_ukPdvU25m_EU.Ron2();
            p311vr   = 0.00M; // TODO: !!!  S_ukOsn05m_BS
            p311pdv  = 0.00M; // TODO: !!!  S_ukPdv05m_BS
            p312vr   = Faktur_rec_SumaRazdoblja_URA.S_ukOsn10m_BS .Ron2();
            p312pdv  = Faktur_rec_SumaRazdoblja_URA.S_ukPdv10m_BS .Ron2();
            p313vr   = Faktur_rec_SumaRazdoblja_URA.S_ukOsn25m_BS .Ron2();
            p313pdv  = Faktur_rec_SumaRazdoblja_URA.S_ukPdv25m_BS .Ron2();
            p314vr   = pdv.ukOsn_WO                               .Ron2();
            p314pdv  = pdv.ukPdv_WO                               .Ron2();
            p315     = pdv.pdvIspravak_obrIII8                    .Ron2();
            p400     = pdv.obvezaPDV                              .Ron2();
            p500     = -1.00M * pdv.porezKredit                   .Ron2();
            p600     = pdv.razlikaUplPov                          .Ron2(); 
            p700     = pdv.redakVII                               .Ron2(); 

            k810   = pdv.pdv_810_sum .Ron2();  
            k811   = pdv.pdv_811     .Ron2();
            k812   = pdv.pdv_812     .Ron2();
            k813   = pdv.pdv_813     .Ron2();
            k814   = pdv.pdv_814     .Ron2();
            k815   = pdv.pdv_815     .Ron2();
            k820   = pdv.pdv_820     .Ron2();
            k830   = pdv.pdv_830_sum .Ron2();  
            k831vr = pdv.pdv_831vr   .Ron2();
            k831br = pdv.pdv_831br          ;
            k832vr = pdv.pdv_832vr   .Ron2();
            k832br = pdv.pdv_832br          ;
            k833vr = pdv.pdv_833vr   .Ron2();
            k833br = pdv.pdv_833br          ;
            k840   = pdv.pdv_840_sum .Ron2();  
            k850   = pdv.pdv_850_sum .Ron2();  
            k860   = pdv.pdv_860     .Ron2();
            k870   = (ZXC.CURR_prjkt_rec.PdvRTip == ZXC.PdvRTipEnum.OBRT_R2 || ZXC.CURR_prjkt_rec.PdvRTip == ZXC.PdvRTipEnum.POD_PO_NAPL) ? true : false; // da li ide po naplacenom 



#endregion PdvSchema_2015
         }
         // PDV 2015 --------- END ----------------------------------------

#endregion SET Tijelo Values

         // TODO: !!!! for PDV_EU 
#region Write to XML

         writer.WriteStartElement("Tijelo");

       //if(PdvSchema_2013_EU == false)                     // classic OLD before EU 
         if(PdvSchema_23 || PdvSchema_25 || PdvSchema_2013) // classic OLD before EU 
         {
#region PdvSchema_23 || PdvSchema_25 || PdvSchema_2013
            writer.WriteElementString("Podatak000", p000.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak100", p100.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak110", p110.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak120", p120.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak121", p121.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak122", p122.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak123", p123.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak124", p124.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak130", p130.ToStringVv_NoGroup_ForceDot());

            writer.WriteStartElement("Podatak200");
               writer.WriteElementString("Vrijednost", p200vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p200pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak210");
               writer.WriteElementString("Vrijednost", p210vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p210pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak220");
               writer.WriteElementString("Vrijednost", p220vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p220pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak230");
               writer.WriteElementString("Vrijednost", p230vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p230pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak240");
               writer.WriteElementString("Vrijednost", p240vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p240pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak300");
               writer.WriteElementString("Vrijednost", p300vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p300pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak310");
               writer.WriteElementString("Vrijednost", p310vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p310pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak320");
               writer.WriteElementString("Vrijednost", p320vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p320pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak330");
               writer.WriteElementString("Vrijednost", p330vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p330pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak340");
               writer.WriteElementString("Vrijednost", p340vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p340pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak350");
               if(PdvSchema_25 || PdvSchema_2013) 
               writer.WriteElementString("PorezNeOdb", p350nePdv.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Vrijednost", p350vr   .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p350pdv  .ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak360");
               if(PdvSchema_25 || PdvSchema_2013) 
               writer.WriteElementString("PorezNeOdb", p360nePdv.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Vrijednost", p360vr   .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p360pdv  .ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak370");
               if(PdvSchema_25 || PdvSchema_2013) 
               writer.WriteElementString("PorezNeOdb", p370nePdv.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Vrijednost", p370vr   .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p370pdv  .ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();

               writer.WriteElementString("Podatak380", p380.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak400", p400.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak500", p500.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak600", p600.ToStringVv_NoGroup_ForceDot());
#endregion PdvSchema_23 || PdvSchema_25 || PdvSchema_2013
         } //if(PdvSchema_23 || PdvSchema_25 || PdvSchema_2013) // classic OLD before EU 

         else if(PdvSchema_2013_EU) // PdvSchema_2013_EU 
         {
#region PdvSchema_2013_EU
            writer.WriteElementString("Podatak000", p000.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak100", p100.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak101", p101.ToStringVv_NoGroup_ForceDot()); 
               writer.WriteElementString("Podatak102", p102.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak103", p103.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak104", p104.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak105", p105.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak106", p106.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak107", p107.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak108", p108.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak109", p109.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak110", p110.ToStringVv_NoGroup_ForceDot());
            writer.WriteStartElement("Podatak200");
               writer.WriteElementString("Vrijednost", p200vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p200pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak201");
               writer.WriteElementString("Vrijednost", p201vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p201pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak202");
               writer.WriteElementString("Vrijednost", p202vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p202pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak203");
               writer.WriteElementString("Vrijednost", p203vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p203pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak204");
               writer.WriteElementString("Vrijednost", p204vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p204pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak205");
               writer.WriteElementString("Vrijednost", p205vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p205pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak206");
               writer.WriteElementString("Vrijednost", p206vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p206pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak207");
               writer.WriteElementString("Vrijednost", p207vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p207pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak208");
               writer.WriteElementString("Vrijednost", p208vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p208pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak209");
               writer.WriteElementString("Vrijednost", p209vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p209pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak210");
               writer.WriteElementString("Vrijednost", p210vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p210pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak211");
               writer.WriteElementString("Vrijednost", p211vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p211pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak212");
               writer.WriteElementString("Vrijednost", p212vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p212pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak213");
               writer.WriteElementString("Vrijednost", p213vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p213pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak214");
               writer.WriteElementString("Vrijednost", p214vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p214pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak300");
               writer.WriteElementString("Vrijednost", p300vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p300pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak301");
               writer.WriteElementString("Vrijednost", p301vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p301pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak302");
               writer.WriteElementString("Vrijednost", p302vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p302pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak303");
               writer.WriteElementString("Vrijednost", p303vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p303pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak304");
               writer.WriteElementString("Vrijednost", p304vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p304pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak305");
               writer.WriteElementString("Vrijednost", p305vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p305pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak306");
               writer.WriteElementString("Vrijednost", p306vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p306pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
               writer.WriteElementString("Podatak307", p307.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak400", p400.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak500", p500.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak600", p600.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak700", p700.ToStringVv_NoGroup_ForceDot());
#endregion PdvSchema_2013_EU
         } // else // PdvSchema_2013_EU 

         else if(PdvSchema_2014) // PdvSchema_2014 
         {
#region PdvSchema_2014
            writer.WriteElementString("Podatak000", p000.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak100", p100.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak101", p101.ToStringVv_NoGroup_ForceDot()); 
               writer.WriteElementString("Podatak102", p102.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak103", p103.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak104", p104.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak105", p105.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak106", p106.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak107", p107.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak108", p108.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak109", p109.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak110", p110.ToStringVv_NoGroup_ForceDot());
            writer.WriteStartElement("Podatak200");
               writer.WriteElementString("Vrijednost", p200vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p200pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak201");
               writer.WriteElementString("Vrijednost", p201vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p201pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak202");
               writer.WriteElementString("Vrijednost", p202vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p202pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak203");
               writer.WriteElementString("Vrijednost", p203vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p203pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak204");
               writer.WriteElementString("Vrijednost", p204vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p204pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak205");
               writer.WriteElementString("Vrijednost", p205vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p205pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak206");
               writer.WriteElementString("Vrijednost", p206vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p206pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak207");
               writer.WriteElementString("Vrijednost", p207vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p207pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak208");
               writer.WriteElementString("Vrijednost", p208vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p208pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak209");
               writer.WriteElementString("Vrijednost", p209vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p209pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak210");
               writer.WriteElementString("Vrijednost", p210vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p210pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak211");
               writer.WriteElementString("Vrijednost", p211vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p211pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak212");
               writer.WriteElementString("Vrijednost", p212vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p212pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak213");
               writer.WriteElementString("Vrijednost", p213vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p213pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak214");
               writer.WriteElementString("Vrijednost", p214vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p214pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak215");
               writer.WriteElementString("Vrijednost", p215vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p215pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak300");
               writer.WriteElementString("Vrijednost", p300vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p300pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak301");
               writer.WriteElementString("Vrijednost", p301vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p301pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak302");
               writer.WriteElementString("Vrijednost", p302vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p302pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak303");
               writer.WriteElementString("Vrijednost", p303vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p303pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak304");
               writer.WriteElementString("Vrijednost", p304vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p304pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak305");
               writer.WriteElementString("Vrijednost", p305vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p305pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak306");
               writer.WriteElementString("Vrijednost", p306vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p306pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak307");
               writer.WriteElementString("Vrijednost", p307vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p307pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak308");
               writer.WriteElementString("Vrijednost", p308vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p308pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak309");
               writer.WriteElementString("Vrijednost", p309vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p309pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak310");
               writer.WriteElementString("Vrijednost", p310vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p310pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak311");
               writer.WriteElementString("Vrijednost", p311vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p311pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak312");
               writer.WriteElementString("Vrijednost", p312vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p312pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak313");
               writer.WriteElementString("Vrijednost", p313vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p313pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak314");
               writer.WriteElementString("Vrijednost", p314vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p314pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
               writer.WriteElementString("Podatak315", p315.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak400", p400.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak500", p500.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak600", p600.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak700", p700.ToStringVv_NoGroup_ForceDot());
#endregion PdvSchema_2014
         } // else // PdvSchema_2014 

         else if(PdvSchema_2015) // PdvSchema_2015 
         {
#region PdvSchema_2015
            writer.WriteElementString("Podatak000", p000.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak100", p100.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak101", p101.ToStringVv_NoGroup_ForceDot()); 
               writer.WriteElementString("Podatak102", p102.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak103", p103.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak104", p104.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak105", p105.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak106", p106.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak107", p107.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak108", p108.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak109", p109.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak110", p110.ToStringVv_NoGroup_ForceDot());
            writer.WriteStartElement("Podatak200");
               writer.WriteElementString("Vrijednost", p200vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p200pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak201");
               writer.WriteElementString("Vrijednost", p201vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p201pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak202");
               writer.WriteElementString("Vrijednost", p202vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p202pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak203");
               writer.WriteElementString("Vrijednost", p203vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p203pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak204");
               writer.WriteElementString("Vrijednost", p204vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p204pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak205");
               writer.WriteElementString("Vrijednost", p205vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p205pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak206");
               writer.WriteElementString("Vrijednost", p206vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p206pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak207");
               writer.WriteElementString("Vrijednost", p207vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p207pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak208");
               writer.WriteElementString("Vrijednost", p208vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p208pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak209");
               writer.WriteElementString("Vrijednost", p209vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p209pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak210");
               writer.WriteElementString("Vrijednost", p210vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p210pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak211");
               writer.WriteElementString("Vrijednost", p211vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p211pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak212");
               writer.WriteElementString("Vrijednost", p212vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p212pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak213");
               writer.WriteElementString("Vrijednost", p213vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p213pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak214");
               writer.WriteElementString("Vrijednost", p214vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p214pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak215");
               writer.WriteElementString("Vrijednost", p215vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p215pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak300");
               writer.WriteElementString("Vrijednost", p300vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p300pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak301");
               writer.WriteElementString("Vrijednost", p301vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p301pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak302");
               writer.WriteElementString("Vrijednost", p302vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p302pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak303");
               writer.WriteElementString("Vrijednost", p303vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p303pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak304");
               writer.WriteElementString("Vrijednost", p304vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p304pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak305");
               writer.WriteElementString("Vrijednost", p305vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p305pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak306");
               writer.WriteElementString("Vrijednost", p306vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p306pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak307");
               writer.WriteElementString("Vrijednost", p307vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p307pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak308");
               writer.WriteElementString("Vrijednost", p308vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p308pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak309");
               writer.WriteElementString("Vrijednost", p309vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p309pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak310");
               writer.WriteElementString("Vrijednost", p310vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p310pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak311");
               writer.WriteElementString("Vrijednost", p311vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p311pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak312");
               writer.WriteElementString("Vrijednost", p312vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p312pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak313");
               writer.WriteElementString("Vrijednost", p313vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p313pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteStartElement("Podatak314");
               writer.WriteElementString("Vrijednost", p314vr .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"     , p314pdv.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
            writer.WriteElementString("Podatak315", p315.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak400", p400.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak500", p500.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak600", p600.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak700", p700.ToStringVv_NoGroup_ForceDot());
            
            writer.WriteElementString("Podatak810", k810.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak811", k811.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak812", k812.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak813", k813.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak814", k814.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak815", k815.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak820", k820.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak830", k830.ToStringVv_NoGroup_ForceDot());
            writer.WriteStartElement("Podatak831");
               writer.WriteElementString("Vrijednost", k831vr.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Broj"      , k831br.ToString()                   ); writer.WriteEndElement();
            writer.WriteStartElement("Podatak832");
               writer.WriteElementString("Vrijednost", k832vr.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Broj"      , k832br.ToString()                   ); writer.WriteEndElement();
            writer.WriteStartElement("Podatak833");
               writer.WriteElementString("Vrijednost", k833vr.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Broj"      , k833br.ToString()                   ); writer.WriteEndElement();
            writer.WriteElementString("Podatak840", k840.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak850", k850.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak860", k860.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("Podatak870", k870.ToString().ToLower()                   );

#endregion PdvSchema_2015
         } // else // PdvSchema_2015 

         if(IsPdvK)
         {
#region IsPdvK
            if(PdvSchema_2013_EU || PdvSchema_2014)
            {
               writer.WriteElementString("Podatak810", k810.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak811", k811.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak812", k812.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak813", k813.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak814", k814.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak815", k815.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak816", k816.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak820", k820.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak830", k830.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak840", k840.ToStringVv_NoGroup_ForceDot());

            
            }
            else
            {
               writer.WriteElementString("Podatak710", k710.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak711", k711.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak712", k712.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak713", k713.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak714", k714.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak715", k715.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak716", k716.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak720", k720.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak730", k730.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak740", k740.ToStringVv_NoGroup_ForceDot());
               if(PdvSchema_23) 
                  writer.WriteElementString("Podatak750", k750.ToStringVv_NoGroup_ForceDot());
               if(PdvSchema_23) 
                  writer.WriteElementString("Podatak760", k760.ToStringVv_NoGroup_ForceDot());
               if(PdvSchema_25 || PdvSchema_2013 /*|| PdvSchema_2014 ? */)  
                  writer.WriteElementString("PretporezOdb", PretporezOdb.ToStringVv_NoGroup_ForceDot());
            }
#endregion IsPdvK

         }

         writer.WriteEndElement(); // Tijelo 

#endregion Write to XML

#endregion Tijelo

#region Finish Xml Document

         writer.WriteEndElement();
         writer.WriteEndDocument();

#endregion Finish Xml Document

      }
      return true;
   }

#endregion ePDV XML Export

}

//public class RptR_PDV_PPO            : RptR_StandardRiskReport
  public class RptR_PDV_PPO            : RptR_PDV_Knjiga
{
   public bool PdvSchema_2015;

   public RptR_PDV_PPO(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter)

      : base(_reportDocument, _reportName, _rptFilter, false) 

   {
      IsForExport = true;

      this.PdvSchema_2015 = RptFilter.DatumOd >= ZXC.Date01012015  /* 01.01.2015 */                                                                      ;
   }

   //IEnumerable<IGrouping<int,    Faktur>> monthlyGrouppedFakturs;
   //IEnumerable<IGrouping<string, Faktur>> oibGrouppedFakturs    ;

   public override int FillRiskReportLists()
   {
      RptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.s_ukOsn07], "s_ukOsn07", 0M, " != "));
      RptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", "prihod", TtInfo.Prihod_IN_Clause, "Prihod - IFA, IRA, IRM, IOD, IPV", "Za tip:", " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!

      RptFilter.FilterMembers.RemoveAll(fm => fm.name == "TT");

      base.FillRiskReportLists();

    //18.07.2019. ne znam zasto onaj gore filterMembers ne radi .... jer su dolazile i fakture koje nisu trebale doci pa sam ovo dodala
      TheFakturList.RemoveAll(fak => fak.S_ukOsn07 == 0);

      TheDeviznaSumaList = new List<VvReportSourceUtil>();

#region Fill missing months

      string or;
      int firstMonth;
      if(PdvSchema_2015) // new
      {
         or = GetPPO_Tromjesecje(RptFilter.DatumDo);
         firstMonth = (or == "1" ? 1 : or == "2" ? 4 : or == "3" ? 7 : 10);
      }
      else // old 
      {
         or = GetPPO_OznakaRazdoblja(RptFilter.DatumOd);
         firstMonth = (or == "1" ? 7 : or == "2" ? 1 : or == "3" ? 7 : /* todo za buduca razdoblja*/ 1);
      }

      bool fakExistsInMonth, touched = false;

      for(int month = firstMonth; month < firstMonth + (PdvSchema_2015 ? 3 : 6); ++month)
      {
         fakExistsInMonth = TheFakturList.Any(f => f.PdvDate.Month == month);

         if(fakExistsInMonth == false)
         {
            TheFakturList.Add(new Faktur { PdvDate = new DateTime(RptFilter.DatumOd.Year, month, 1), S_ukOsn07 = 0.00M });
            touched = true;
         }
      }

  // 18.07.2019. zasto bi bili sortirani samo kada nema ni jedne fakture u mjesecu???
  //             ako je neka ifa placena prije neke koja je ispred nje odnosno kasnije onda se poseremetilo
  //if(touched) TheFakturList = TheFakturList.OrderBy(f => f.PdvDate).ToList();
                TheFakturList = TheFakturList.OrderBy(f => f.PdvDate).ToList();

#endregion Fill missing months

      var monthlyGrouppedFakturs = TheFakturList.GroupBy(fak => fak.PdvDate.Month);

      foreach(var monthGR in monthlyGrouppedFakturs)
      {
            var oibGrouppedFakturs = monthGR.GroupBy(mgr => mgr.KdOib);

            foreach(var oibGR in oibGrouppedFakturs)
            {
               TheDeviznaSumaList.Add(new VvReportSourceUtil(oibGR.First().PdvDate, (PdvSchema_2015 ? oibGR.First().PdvDate.RbrMjUkvartalu() : oibGR.First().PdvDate.ToString("MMMM")), oibGR.Key, oibGR.Sum(fak => fak.S_ukOsn07.Ron2())));
            }
      }

      return 0;
   }

#region PPO XML Export

   // ovdje nisi nasao nikakve upute kako bi se trebao zvati fajl pa ga imenujes proizvoljno 
   public override string ExportFileName
   {
      get
      {
         string mmOD_mmDO_yyyy = RptFilter.DatumOd.Month.ToString("00") + RptFilter.DatumDo.Month.ToString("00") + RptFilter.DatumOd.Year.ToString("0000");

         return "PPO_" + ZXC.CURR_prjkt_rec.Oib + "_" + mmOD_mmDO_yyyy + ".xml";
      }
   }

#region Xml Schema Validation

   public override bool ExecuteExportValidation(string fileName)
   {
      List<ZXC.VvXmlValidationData> valDataList = new List<ZXC.VvXmlValidationData>();

      if(PdvSchema_2015 == true)
      {
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPPO/v1-0", @"XSD\ObrazacPPO-v1-0.xsd"          ));
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPPO/v1-0", @"XSD\ObrazacPPOtipovi-v1-0.xsd"    ));
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\ObrazacPPOmetapodaci-v1-0.xsd"));
      }
      else // OLD 2013_2014 
      {
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPPO20132014/v1-0", @"XSD\ObrazacPPO20132014-v1-0.xsd"          ));
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPPO20132014/v1-0", @"XSD\ObrazacPPO20132014tipovi-v1-0.xsd"    ));
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"                 , @"XSD\ObrazacPPO20132014metapodaci-v1-0.xsd"));
      }

      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\MetapodaciTipovi-v2-0.xsd"));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/TemeljniTipovi/v2-1"     , @"XSD\TemeljniTipovi-v2-1.xsd"      ));

      return ExecuteExportValidation_Base(valDataList);
   }

#endregion Xml Schema Validation
   
   public override bool ExecuteExport(string fileName)
   {
#region Initialize XmlWriterSettings

      if(TheDeviznaSumaList == null || TheDeviznaSumaList.Count().IsZero()) throw new Exception("Nema se što exporitrati!");

      string ident = "   ";

      XmlWriterSettings settings = new XmlWriterSettings();
      
      settings.Indent = true;
      settings.IndentChars = ident;

#endregion Initialize XmlWriterSettings

      using(XmlWriter writer = XmlWriter.Create(fileName, settings))
      {
#region Init Xml Document

         writer.WriteStartDocument();

         if(PdvSchema_2015 == true) // novi 
            writer.WriteStartElement("ObrazacPPO"        , @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPPO/v1-0"        );
         else // stari 
            writer.WriteStartElement("ObrazacPPO20132014", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPPO20132014/v1-0");

         writer.WriteAttributeString("verzijaSheme",                                                                              "1.0");

#endregion Init Xml Document

#region Metapodaci

         writer.WriteStartElement("Metapodaci", @"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0");
            //writer.WriteElementString("Naslov", @"http://purl.org/dc/elements/1.1/title", "Prijava poreza na dodanu vrijednost");
            writer.WriteRaw("\n");
            writer.WriteRaw(ident + ident + "<Naslov dc=\"http://purl.org/dc/elements/1.1/title\">Prijava prijenosa porezne obveze</Naslov>\n");
            writer.WriteRaw(ident + ident + "<Autor dc=\"http://purl.org/dc/elements/1.1/creator\">" + ZXC.CURR_prjkt_rec.Ime + " " + ZXC.CURR_prjkt_rec.Prezime + "</Autor>\n");
            writer.WriteRaw(ident + ident + "<Datum dc=\"http://purl.org/dc/elements/1.1/date\">" + DateTime.Now.ToString("s") + "</Datum>\n");
            writer.WriteRaw(ident + ident + "<Format dc=\"http://purl.org/dc/elements/1.1/format\">text/xml</Format>\n");
            writer.WriteRaw(ident + ident + "<Jezik dc=\"http://purl.org/dc/elements/1.1/language\">hr-HR</Jezik>\n");
            writer.WriteRaw(ident + ident + "<Identifikator dc=\"http://purl.org/dc/elements/1.1/identifier\">" + Guid.NewGuid().ToString("D")/*.ToUpper()*/ + "</Identifikator>\n");

            if(PdvSchema_2015) writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPPO-v1-0</Uskladjenost>\n");
            else               writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPPO20132014-v1-0</Uskladjenost>\n");

            writer.WriteRaw(ident + ident + "<Tip dc=\"http://purl.org/dc/elements/1.1/type\">Elektronički obrazac</Tip>\n");
            writer.WriteRaw(ident + ident + "<Adresant>Ministarstvo Financija, Porezna uprava, Zagreb</Adresant>\n");
            writer.WriteRaw(ident);
         writer.WriteEndElement(); // Metapodaci 

#endregion Write Header Data

#region Zaglavlje

         writer.WriteStartElement("Zaglavlje");

            writer.WriteStartElement("Razdoblje");
               writer.WriteElementString("DatumOd", RptFilter.DatumOd.ToString("s").Substring(0, 10));
               writer.WriteElementString("DatumDo", RptFilter.DatumDo.ToString("s").Substring(0, 10));
               if(PdvSchema_2015)
               {
                  writer.WriteElementString("Tromjesecje", GetPPO_Tromjesecje(RptFilter.DatumDo)); 
                  writer.WriteElementString("Godina"     , RptFilter.DatumOd.Year.ToString()    ); 
               }
               else
               {
                  writer.WriteElementString("Oznaka", GetPPO_OznakaRazdoblja(RptFilter.DatumOd)); // ??? ... todo ... 
               }
            writer.WriteEndElement(); // Razdoblje 

            writer.WriteStartElement("Obveznik");
               writer.WriteElementString("Naziv", ZXC.CURR_prjkt_rec.Naziv);
               writer.WriteElementString("OIB", ZXC.CURR_prjkt_rec.Oib);
             //if(PdvSchema_2013_EU == false && PdvSchema_2014 == false) 
             //writer.WriteElementString("SifraDjelatnosti", ZXC.CURR_prjkt_rec.SifDcd);
               writer.WriteStartElement("Adresa");
                  writer.WriteElementString("Mjesto", ZXC.CURR_prjkt_rec.Grad);
                  writer.WriteElementString("Ulica", ZXC.CURR_prjkt_rec.UlicaBezBroja_1);
                  writer.WriteElementString("Broj", ZXC.CURR_prjkt_rec.UlicniBroj_1);
               writer.WriteEndElement(); // Adresa 
            writer.WriteEndElement(); // Obveznik 

            writer.WriteStartElement("ObracunSastavio");
               writer.WriteElementString("Ime"     , ZXC.CURR_prjkt_rec.Ime);
               writer.WriteElementString("Prezime" , ZXC.CURR_prjkt_rec.Prezime);
               //writer.WriteElementString("Telefon", /*ZXC.CURR_prjkt_rec.Tel1 zbog validacije:*/ZXC.CURR_prjkt_rec.Tel2);
               if(PdvSchema_2015 == false)
                  writer.WriteElementString("Email"   , ZXC.CURR_prjkt_rec.Email);

            writer.WriteEndElement(); // ObracunSastavio 
            
         writer.WriteElementString("Ispostava", ZXC.CURR_prjkt_rec.OpcCd);
         if(PdvSchema_2015 == false)
            writer.WriteElementString("Napomena", " "); // ??? fuse ??? 

         writer.WriteEndElement(); // Zaglavlje 

#endregion Zaglavlje

#region Tijelo

         var monthlyGrouppedDeviznaSuma = TheDeviznaSumaList.GroupBy(ds => ds.TheDate.Month);

         writer.WriteStartElement("Tijelo");

         writer.WriteStartElement("Isporuke");

         uint rbr;
         foreach(var monthGR in monthlyGrouppedDeviznaSuma)
         {
            rbr = 0;

            writer.WriteStartElement("Isporuka"); // mjesecna grupa 

            writer.WriteStartElement("Podaci");

          var oibGrouppedDeviznaSuma = monthGR.GroupBy(mgr => mgr.TheCD);

          foreach(var oibGR in oibGrouppedDeviznaSuma)
          {
             if(oibGR.Key.IsEmpty()) continue;

               writer.WriteStartElement("Podatak");

               writer.WriteElementString("RedniBroj", (++rbr).ToString());
               writer.WriteElementString("OIB"      , oibGR.Key);
               writer.WriteElementString("Iznos"    , oibGR.Sum(ds => ds.TheMoney.Ron2()).ToStringVv_NoGroup_ForceDot()); 

               writer.WriteEndElement(); // Podatak 
          }

            writer.WriteEndElement(); // Podaci 

            writer.WriteElementString("Iznos",   monthGR.Sum(mgr => mgr.TheMoney.Ron2()).ToStringVv_NoGroup_ForceDot())     ; 
            writer.WriteElementString("DatumOd", monthGR.First().TheDate.ThisMonthFirstDay().ToString("s").Substring(0, 10));
            writer.WriteElementString("DatumDo", monthGR.First().TheDate.ThisMonthLastDay ().ToString("s").Substring(0, 10));

            writer.WriteEndElement(); // Isporuka 
         }

         writer.WriteEndElement(); // Isporuke 

         writer.WriteElementString("Ukupno", TheDeviznaSumaList.Sum(ds => ds.TheMoney.Ron2()).ToStringVv_NoGroup_ForceDot()); 

         writer.WriteEndElement(); // Tijelo 

#endregion Tijelo

#region Finish Xml Document

         writer.WriteEndElement();
         writer.WriteEndDocument();

#endregion Finish Xml Document

      }
      return true;
   }

   private string GetPPO_OznakaRazdoblja(DateTime dateOD)
   {
      //switch(dateOD.Year)
      //{
      //   case 2013: return "1";
      //   case 2014: return "2";
      //
      //   default: return "0";
      //}

      if(dateOD == ZXC.Date01072013) return "1";
      if(dateOD == ZXC.Date01012014) return "2";
      if(dateOD == ZXC.Date01072014) return "3";
      /* todo za buduca razdoblja*/

      return "0";
   }

   private string GetPPO_Tromjesecje(DateTime dateDO)
   {
      switch(dateDO.Month)
      {
         case 1:
         case 2:
         case 3: 
            return "1";
         case 4:
         case 5:
         case 6:
            return "2";
         case 7:
         case 8:
         case 9:
            return "3";
         case 10:
         case 11:
         case 12:
            return "4";
      
         default: return "0";
      }
   }

#endregion ePDV XML Export

}

public class RptR_PDV_PPO_OLDdo2019            : RptR_StandardRiskReport
{
   public bool PdvSchema_2015;

   public RptR_PDV_PPO_OLDdo2019(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)

      : base(_reportDocument     ,
             _reportName         ,                                                    
             _rptFilter          , 
             _filterStyle        ,
             _rptNeeds_ArtWars   , // ArtiklWithArtstat 
             _rptNeeds_ArtStat   , // ArtStat        
             _rptNeeds_Faktur    , // Faktur         
             _rptNeeds_Rtrans    , // Rtrans         
             _rptNeeds_Kupdob    , // Kupdob         
             _rptNeeds_Prjkt     , // Prjkt          
             _rptNeeds_Rtrans4ruc, // Rtrans4ruc     
             _rptNeeds_Artikl    ) // Artikl         

   {
      IsForExport = true;

      this.PdvSchema_2015 = RptFilter.DatumOd >= ZXC.Date01012015  /* 01.01.2015 */                                                                      ;
   }

   //IEnumerable<IGrouping<int,    Faktur>> monthlyGrouppedFakturs;
   //IEnumerable<IGrouping<string, Faktur>> oibGrouppedFakturs    ;

   public override int FillRiskReportLists()
   {
      RptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.s_ukOsn07], "s_ukOsn07", 0M, " != "));
      RptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", "prihod", TtInfo.Prihod_IN_Clause, "Prihod - IFA, IRA, IRM, IOD, IPV", "Za tip:", " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!

      RptFilter.FilterMembers.RemoveAll(fm => fm.name == "TT");

      base.FillRiskReportLists();

      TheDeviznaSumaList = new List<VvReportSourceUtil>();

#region Fill missing months

      string or;
      int firstMonth;
      if(PdvSchema_2015) // new
      {
         or = GetPPO_Tromjesecje(RptFilter.DatumDo);
         firstMonth = (or == "1" ? 1 : or == "2" ? 4 : or == "3" ? 7 : 10);
      }
      else // old 
      {
         or = GetPPO_OznakaRazdoblja(RptFilter.DatumOd);
         firstMonth = (or == "1" ? 7 : or == "2" ? 1 : or == "3" ? 7 : /* todo za buduca razdoblja*/ 1);
      }

      bool fakExistsInMonth, touched = false;

      for(int month = firstMonth; month < firstMonth + (PdvSchema_2015 ? 3 : 6); ++month)
      {
         fakExistsInMonth = TheFakturList.Any(f => f.PdvDate.Month == month);

         if(fakExistsInMonth == false)
         {
            TheFakturList.Add(new Faktur { PdvDate = new DateTime(RptFilter.DatumOd.Year, month, 1), S_ukOsn07 = 0.00M });
            touched = true;
         }
      }

      if(touched) TheFakturList = TheFakturList.OrderBy(f => f.PdvDate).ToList();

#endregion Fill missing months

      var monthlyGrouppedFakturs = TheFakturList.GroupBy(fak => fak.PdvDate.Month);

      foreach(var monthGR in monthlyGrouppedFakturs)
      {
            var oibGrouppedFakturs = monthGR.GroupBy(mgr => mgr.KdOib);

            foreach(var oibGR in oibGrouppedFakturs)
            {
               TheDeviznaSumaList.Add(new VvReportSourceUtil(oibGR.First().PdvDate, (PdvSchema_2015 ? oibGR.First().PdvDate.RbrMjUkvartalu() : oibGR.First().PdvDate.ToString("MMMM")), oibGR.Key, oibGR.Sum(fak => fak.S_ukOsn07.Ron2())));
            }
      }

      return 0;
   }

#region PPO XML Export

   // ovdje nisi nasao nikakve upute kako bi se trebao zvati fajl pa ga imenujes proizvoljno 
   public override string ExportFileName
   {
      get
      {
         string mmOD_mmDO_yyyy = RptFilter.DatumOd.Month.ToString("00") + RptFilter.DatumDo.Month.ToString("00") + RptFilter.DatumOd.Year.ToString("0000");

         return "PPO_" + ZXC.CURR_prjkt_rec.Oib + "_" + mmOD_mmDO_yyyy + ".xml";
      }
   }

#region Xml Schema Validation

   public override bool ExecuteExportValidation(string fileName)
   {
      List<ZXC.VvXmlValidationData> valDataList = new List<ZXC.VvXmlValidationData>();

      if(PdvSchema_2015 == true)
      {
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPPO/v1-0", @"XSD\ObrazacPPO-v1-0.xsd"          ));
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPPO/v1-0", @"XSD\ObrazacPPOtipovi-v1-0.xsd"    ));
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\ObrazacPPOmetapodaci-v1-0.xsd"));
      }
      else // OLD 2013_2014 
      {
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPPO20132014/v1-0", @"XSD\ObrazacPPO20132014-v1-0.xsd"          ));
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPPO20132014/v1-0", @"XSD\ObrazacPPO20132014tipovi-v1-0.xsd"    ));
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"                 , @"XSD\ObrazacPPO20132014metapodaci-v1-0.xsd"));
      }

      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\MetapodaciTipovi-v2-0.xsd"));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/TemeljniTipovi/v2-1"     , @"XSD\TemeljniTipovi-v2-1.xsd"      ));

      return ExecuteExportValidation_Base(valDataList);
   }

#endregion Xml Schema Validation
   
   public override bool ExecuteExport(string fileName)
   {
#region Initialize XmlWriterSettings

      if(TheDeviznaSumaList == null || TheDeviznaSumaList.Count().IsZero()) throw new Exception("Nema se što exporitrati!");

      string ident = "   ";

      XmlWriterSettings settings = new XmlWriterSettings();
      
      settings.Indent = true;
      settings.IndentChars = ident;

#endregion Initialize XmlWriterSettings

      using(XmlWriter writer = XmlWriter.Create(fileName, settings))
      {
#region Init Xml Document

         writer.WriteStartDocument();

         if(PdvSchema_2015 == true) // novi 
            writer.WriteStartElement("ObrazacPPO"        , @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPPO/v1-0"        );
         else // stari 
            writer.WriteStartElement("ObrazacPPO20132014", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPPO20132014/v1-0");

         writer.WriteAttributeString("verzijaSheme",                                                                              "1.0");

#endregion Init Xml Document

#region Metapodaci

         writer.WriteStartElement("Metapodaci", @"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0");
            //writer.WriteElementString("Naslov", @"http://purl.org/dc/elements/1.1/title", "Prijava poreza na dodanu vrijednost");
            writer.WriteRaw("\n");
            writer.WriteRaw(ident + ident + "<Naslov dc=\"http://purl.org/dc/elements/1.1/title\">Prijava prijenosa porezne obveze</Naslov>\n");
            writer.WriteRaw(ident + ident + "<Autor dc=\"http://purl.org/dc/elements/1.1/creator\">" + ZXC.CURR_prjkt_rec.Ime + " " + ZXC.CURR_prjkt_rec.Prezime + "</Autor>\n");
            writer.WriteRaw(ident + ident + "<Datum dc=\"http://purl.org/dc/elements/1.1/date\">" + DateTime.Now.ToString("s") + "</Datum>\n");
            writer.WriteRaw(ident + ident + "<Format dc=\"http://purl.org/dc/elements/1.1/format\">text/xml</Format>\n");
            writer.WriteRaw(ident + ident + "<Jezik dc=\"http://purl.org/dc/elements/1.1/language\">hr-HR</Jezik>\n");
            writer.WriteRaw(ident + ident + "<Identifikator dc=\"http://purl.org/dc/elements/1.1/identifier\">" + Guid.NewGuid().ToString("D")/*.ToUpper()*/ + "</Identifikator>\n");

            if(PdvSchema_2015) writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPPO-v1-0</Uskladjenost>\n");
            else               writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPPO20132014-v1-0</Uskladjenost>\n");

            writer.WriteRaw(ident + ident + "<Tip dc=\"http://purl.org/dc/elements/1.1/type\">Elektronički obrazac</Tip>\n");
            writer.WriteRaw(ident + ident + "<Adresant>Ministarstvo Financija, Porezna uprava, Zagreb</Adresant>\n");
            writer.WriteRaw(ident);
         writer.WriteEndElement(); // Metapodaci 

#endregion Write Header Data

#region Zaglavlje

         writer.WriteStartElement("Zaglavlje");

            writer.WriteStartElement("Razdoblje");
               writer.WriteElementString("DatumOd", RptFilter.DatumOd.ToString("s").Substring(0, 10));
               writer.WriteElementString("DatumDo", RptFilter.DatumDo.ToString("s").Substring(0, 10));
               if(PdvSchema_2015)
               {
                  writer.WriteElementString("Tromjesecje", GetPPO_Tromjesecje(RptFilter.DatumDo)); 
                  writer.WriteElementString("Godina"     , RptFilter.DatumOd.Year.ToString()    ); 
               }
               else
               {
                  writer.WriteElementString("Oznaka", GetPPO_OznakaRazdoblja(RptFilter.DatumOd)); // ??? ... todo ... 
               }
            writer.WriteEndElement(); // Razdoblje 

            writer.WriteStartElement("Obveznik");
               writer.WriteElementString("Naziv", ZXC.CURR_prjkt_rec.Naziv);
               writer.WriteElementString("OIB", ZXC.CURR_prjkt_rec.Oib);
             //if(PdvSchema_2013_EU == false && PdvSchema_2014 == false) 
             //writer.WriteElementString("SifraDjelatnosti", ZXC.CURR_prjkt_rec.SifDcd);
               writer.WriteStartElement("Adresa");
                  writer.WriteElementString("Mjesto", ZXC.CURR_prjkt_rec.Grad);
                  writer.WriteElementString("Ulica", ZXC.CURR_prjkt_rec.UlicaBezBroja_1);
                  writer.WriteElementString("Broj", ZXC.CURR_prjkt_rec.UlicniBroj_1);
               writer.WriteEndElement(); // Adresa 
            writer.WriteEndElement(); // Obveznik 

            writer.WriteStartElement("ObracunSastavio");
               writer.WriteElementString("Ime"     , ZXC.CURR_prjkt_rec.Ime);
               writer.WriteElementString("Prezime" , ZXC.CURR_prjkt_rec.Prezime);
               //writer.WriteElementString("Telefon", /*ZXC.CURR_prjkt_rec.Tel1 zbog validacije:*/ZXC.CURR_prjkt_rec.Tel2);
               if(PdvSchema_2015 == false)
                  writer.WriteElementString("Email"   , ZXC.CURR_prjkt_rec.Email);

            writer.WriteEndElement(); // ObracunSastavio 
            
         writer.WriteElementString("Ispostava", ZXC.CURR_prjkt_rec.OpcCd);
         if(PdvSchema_2015 == false)
            writer.WriteElementString("Napomena", " "); // ??? fuse ??? 

         writer.WriteEndElement(); // Zaglavlje 

#endregion Zaglavlje

#region Tijelo

         var monthlyGrouppedDeviznaSuma = TheDeviznaSumaList.GroupBy(ds => ds.TheDate.Month);

         writer.WriteStartElement("Tijelo");

         writer.WriteStartElement("Isporuke");

         uint rbr;
         foreach(var monthGR in monthlyGrouppedDeviznaSuma)
         {
            rbr = 0;

            writer.WriteStartElement("Isporuka"); // mjesecna grupa 

            writer.WriteStartElement("Podaci");

          var oibGrouppedDeviznaSuma = monthGR.GroupBy(mgr => mgr.TheCD);

          foreach(var oibGR in oibGrouppedDeviznaSuma)
          {
             if(oibGR.Key.IsEmpty()) continue;

               writer.WriteStartElement("Podatak");

               writer.WriteElementString("RedniBroj", (++rbr).ToString());
               writer.WriteElementString("OIB"      , oibGR.Key);
               writer.WriteElementString("Iznos"    , oibGR.Sum(ds => ds.TheMoney.Ron2()).ToStringVv_NoGroup_ForceDot()); 

               writer.WriteEndElement(); // Podatak 
          }

            writer.WriteEndElement(); // Podaci 

            writer.WriteElementString("Iznos",   monthGR.Sum(mgr => mgr.TheMoney.Ron2()).ToStringVv_NoGroup_ForceDot())     ; 
            writer.WriteElementString("DatumOd", monthGR.First().TheDate.ThisMonthFirstDay().ToString("s").Substring(0, 10));
            writer.WriteElementString("DatumDo", monthGR.First().TheDate.ThisMonthLastDay ().ToString("s").Substring(0, 10));

            writer.WriteEndElement(); // Isporuka 
         }

         writer.WriteEndElement(); // Isporuke 

         writer.WriteElementString("Ukupno", TheDeviznaSumaList.Sum(ds => ds.TheMoney.Ron2()).ToStringVv_NoGroup_ForceDot()); 

         writer.WriteEndElement(); // Tijelo 

#endregion Tijelo

#region Finish Xml Document

         writer.WriteEndElement();
         writer.WriteEndDocument();

#endregion Finish Xml Document

      }
      return true;
   }

   private string GetPPO_OznakaRazdoblja(DateTime dateOD)
   {
      //switch(dateOD.Year)
      //{
      //   case 2013: return "1";
      //   case 2014: return "2";
      //
      //   default: return "0";
      //}

      if(dateOD == ZXC.Date01072013) return "1";
      if(dateOD == ZXC.Date01012014) return "2";
      if(dateOD == ZXC.Date01072014) return "3";
      /* todo za buduca razdoblja*/

      return "0";
   }

   private string GetPPO_Tromjesecje(DateTime dateDO)
   {
      switch(dateDO.Month)
      {
         case 1:
         case 2:
         case 3: 
            return "1";
         case 4:
         case 5:
         case 6:
            return "2";
         case 7:
         case 8:
         case 9:
            return "3";
         case 10:
         case 11:
         case 12:
            return "4";
      
         default: return "0";
      }
   }

#endregion ePDV XML Export

}

#endregion The PDV Suite

#region OTS

public class RptR_OTS          : RptR_StandardRiskReport
{
#region Constructor 

   public bool IsOtsKupaca     { get; set; }
   public bool IsOtsDobavljaca { get { return !IsOtsKupaca; } }

   public RptR_OTS(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter) : base(_reportDocument, _reportName, _rptFilter, ZXC.RIZ_FilterStyle.Ftrans,
         /* _rptNeeds_ArtWars      */            false, // ArtiklWithArtstat  
         /* _rptNeeds_ArtStat      */            false, // ArtStat        
         /* _rptNeeds_Faktur       */            false, // Faktur         
         /* _rptNeeds_Rtrans       */            false, // Rtrans         
         /* _rptNeeds_Kupdob       */            true,  // Kupdob         
         /* _rptNeeds_Prjkt        */            true,  // Prjkt          za kompenzaciju 
         /* _rptNeeds_Rtrans4ruc   */            false, // Rtrans4ruc     
         /* _rptNeeds_Artikl       */            false) // Artikl         

   {
      ReportNeeds_Ftrans_List = true;
      //TheFilterSet = RiskFilterSetEnum.IOS;

      TheReportUC.TheReportViewer.DoubleClickPage -= new PageMouseEventHandler(TheReportUC.TheReportViewer_DoubleClickPage);
      TheReportUC.TheReportViewer.DoubleClickPage += new PageMouseEventHandler(   RptR_OTS_TheReportViewer_DoubleClickPage);
   }

#endregion Constructor

   public override int FillRiskReportLists()
   {
     // base.FillRiskReportLists(); 

      TheFtransList = new List<Ftrans>();

      NalogDao.LoadFtransListFor_OtsReport(TheDbConnection, TheFtransList, RptFilter.FilterMembers, /* DateTime.Now */ RptFilter.OtsDate, IsOtsKupaca, RptFilter.IsOtsAnalitKontre, RptFilter.IsOtsDospOnly, false);

      // 06.11.2014:
      if(RptFilter.IsForceOtsByDokDate)
      {
         TheFtransList = TheFtransList.OrderBy(ftr => ftr.T_ticker).ThenBy(ftr => ftr.T_dokDate).ThenBy(ftr => ftr.R_tipBr_Resorted).ThenBy(ftr => ftr.T_serial).ToList();
      }
      else // default
      {
         // 11.02.2020: R_tipBr_Resorted observacija; 'ParseTipBr(string tipBr, out string tt, out uint ttNum)' 
         // ima BUG, ne splitta dobro TipBr-ove iz PG-a! 

         // 25.02.2020: vratili sort sa 'R_tipBr_Resorted' na 'T_tipBr jer ionako ne radi. Probati ikad kasnije prepoznati sve modele TipBr-a 
         // ... a spacka nastane kad su tu i TipBr-ovi iz PG, pa napraviti bolji 'R_tipBr_Resorted' koji ce valjati za sve kopmbinacije       
       //TheFtransList = TheFtransList.OrderBy(ftr => ftr.T_ticker).ThenBy(ftr => ftr.R_tipBr_Resorted).ThenBy(ftr => ftr.T_dokDate).ThenBy(ftr => ftr.T_serial).ToList();
         TheFtransList = TheFtransList.OrderBy(ftr => ftr.T_ticker).ThenBy(ftr => ftr.T_tipBr         ).ThenBy(ftr => ftr.T_dokDate).ThenBy(ftr => ftr.T_serial).ToList();
      }

#region WEB_DEMO_DATA
#if(WWWDEMO)

      DemoiseKupdobSifrarList(/*TheKupdobList*/VvUserControl.KupdobSifrar);

      for(int i = 0, w = 0; i < TheFtransList.Count; ++i, ++w)
      {
         //if(w == demoKupdobs.Length) w = 0;
         //TheFtransList[i].KupdobName  = demoKupdobs[w].TheName;
         //TheFtransList[i].KupdobTK = demoKupdobs[w].TheCd  ;

         if(TheFtransList[i].T_ticker.IsEmpty())
         {
            TheFtransList[i].T_ticker = demoKupdobs[22].TheCd;
            continue;
         }

         switch(TheFtransList[i].T_ticker.ToLower()[0])
         {
            case 'a':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[0].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[0].TheCd; break;
            case 'b':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[1].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[1].TheCd; break;
            case 'c':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[2].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[2].TheCd; break;
            case 'd':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[3].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[3].TheCd; break;
            case 'e':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[4].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[4].TheCd; break;
            case 'f':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[5].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[5].TheCd; break;
            case 'g':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[6].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[6].TheCd; break;
            case 'h':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[7].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[7].TheCd; break;
            case 'i':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[8].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[8].TheCd; break;
            case 'j':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[9].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[9].TheCd; break;
            case 'k':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[10].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[10].TheCd; break;
            case 'l':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[11].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[11].TheCd; break;
            case 'm':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[12].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[12].TheCd; break;
            case 'n':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[13].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[13].TheCd; break;
            case 'o':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[14].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[14].TheCd; break;
            case 'p':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[15].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[15].TheCd; break;
            case 'r':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[16].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[16].TheCd; break;
            case 's':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[17].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[17].TheCd; break;
            case 't':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[18].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[18].TheCd; break;
            case 'u':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[19].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[19].TheCd; break;
            case 'v':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[20].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[20].TheCd; break;
            case 'z':
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[21].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[21].TheCd; break;
            default:
               //TheFtransList[i].T_kupdob_cd = demoKupdobs[22].TheName;
               TheFtransList[i].T_ticker = demoKupdobs[22].TheCd; break;
         }
      }

#endif
#endregion WEB_DEMO_DATA

      TheKupdobList = VvUserControl.KupdobSifrar.Join(TheFtransList, kpdb => kpdb.KupdobCD, ftrans => ftrans.T_kupdob_cd, (k, f) => k).Distinct().ToList();

#region WEB_DEMO_DATA
#if(WWWDEMO)

      VvUserControl.KupdobSifrar = null;
      TheReportUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.None); // Get back normal sifrar 

#endif
#endregion WEB_DEMO_DATA

      if(this is RptR_Kompenzacija)
      {
         (this as RptR_Kompenzacija).SetMoneyAsText_inPrjkt();
      }

      if(RptFilter.IsOtsNevezano == true)
      {
         var kupdobsCdWithEmptyTipBrList = TheFtransList.Where(ftr => ftr.T_tipBr.IsEmpty()).Select(ftr => ftr.T_kupdob_cd).Distinct();

         TheFtransList.RemoveAll(ftr => kupdobsCdWithEmptyTipBrList.Contains(ftr.T_kupdob_cd) == false);
      }

      return 0;
   }

   void RptR_OTS_TheReportViewer_DoubleClickPage(object sender, PageMouseEventArgs e)
   {
      if(e.ObjectInfo.Name == "TtipBr1") // "IFA-100123" 
      {
         ZXC.TheVvForm.ShowFakturDUC_For_TipBr(e.ObjectInfo.Text);
      }
      if(e.ObjectInfo.Name == "TdokNum1") // "000190" 
      {
         ZXC.TheVvForm.ShowNalogDUC_For_DokNum(e.ObjectInfo.Text);
      }
   }

}

public class RptR_OPZ_Stat1    : RptR_OTS  
{
   public RptR_OPZ_Stat1(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, bool _isKupci): base(_reportDocument, _reportName, _rptFilter)
   {
       this.IsOtsKupaca = _isKupci;
       IsForExport = true;
   }

   public DateTime RptDate_NisuNaplaceniDo        { get; set; }
   public DateTime RptDate_RazdobljeStart         { get; set; }
   public DateTime RptDate_RazdobljeEnd           { get; set; }
   public DateTime RptDate_NaDan                  { get; set; }
   public DateTime RptDate_RazdobljeEnd_6yearsOld { get; set; }

   private void SetReportDates() // FTRANS od-do, FAKTUR od-do, XML od-do 
   {
      RptDate_NisuNaplaceniDo = RptFilter.OtsDate.Date;

           if(RptDate_NisuNaplaceniDo == ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format("3101" + "2016")         .Date) // iznimno za prvi put. Dogodine, daj 01.10.  
      {
         RptDate_RazdobljeStart =                 new DateTime(RptDate_NisuNaplaceniDo.Year - 1, /*10*/01, 01);
         RptDate_RazdobljeEnd   = RptDate_NaDan = new DateTime(RptDate_NisuNaplaceniDo.Year - 1, 12, 31);
      }
      // 25.02.2020: 
    //else if(                               RptDate_NisuNaplaceniDo == ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format("3101" + ZXC.projectYear).Date) // 31.01.2017. i nadalje 
      else if(ZXC.projectYearAsInt < 2020 && RptDate_NisuNaplaceniDo == ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format("3101" + ZXC.projectYear).Date) // 31.01.2017. i nadalje 
      {
         RptDate_RazdobljeStart =                 new DateTime(RptDate_NisuNaplaceniDo.Year - 1, 10, 01);
         RptDate_RazdobljeEnd   = RptDate_NaDan = new DateTime(RptDate_NisuNaplaceniDo.Year - 1, 12, 31);
      }
      else
      {
         // =================================================== big news START 
         // 25.02.2020: 
       //RptDate_RazdobljeStart = new DateTime(RptDate_NisuNaplaceniDo.Year, RptDate_NisuNaplaceniDo.Month - 3, 01);
       //RptDate_RazdobljeEnd   = RptDate_NaDan = new DateTime(RptDate_NisuNaplaceniDo.Year, RptDate_NisuNaplaceniDo.Month - 1, DateTime.DaysInMonth(RptDate_NisuNaplaceniDo.Year, RptDate_NisuNaplaceniDo.Month - 1));

         // 15.04.2020: 
       //if(ZXC.projectYearAsInt < 2020)
         if(ZXC.projectYearAsInt < 2019)
         {
            RptDate_RazdobljeStart = new DateTime(RptDate_NisuNaplaceniDo.Year, RptDate_NisuNaplaceniDo.Month - 3, 01);
            RptDate_RazdobljeEnd   = RptDate_NaDan = new DateTime(RptDate_NisuNaplaceniDo.Year, RptDate_NisuNaplaceniDo.Month - 1, DateTime.DaysInMonth(RptDate_NisuNaplaceniDo.Year, RptDate_NisuNaplaceniDo.Month - 1));
         }
         else // OVAKO sad ide od 2020 nadalje (a za racune iz 2019): 
         {
            // 15.04.2020: 
          //RptDate_RazdobljeStart =                 new DateTime(RptDate_NisuNaplaceniDo.Year - 1, 01, 01); // prvi   dan PG 
          //RptDate_RazdobljeEnd   = RptDate_NaDan = new DateTime(RptDate_NisuNaplaceniDo.Year - 1, 12, 31); // zadnji dan PG 
            RptDate_RazdobljeStart =                 new DateTime(RptDate_NisuNaplaceniDo.Year    , 01, 01); // prvi   dan PG 
            RptDate_RazdobljeEnd   = RptDate_NaDan = new DateTime(RptDate_NisuNaplaceniDo.Year    , 12, 31); // zadnji dan PG 
         }
         // =================================================== big news  END  

      }

      // 24.08.2016: start 
      //RptFilter.DatumOd = ZXC.Date01012010;
      DateTime nextRazdobljeFirstDay = (RptDate_RazdobljeEnd + ZXC.OneDaySpan);
      RptDate_RazdobljeEnd_6yearsOld = new DateTime(nextRazdobljeFirstDay.Year - 6, nextRazdobljeFirstDay.Month, nextRazdobljeFirstDay.Day);
      RptFilter.DatumOd = RptDate_RazdobljeEnd_6yearsOld; // reportamo samo za racune stare max 6 godina 
      // 24.08.2016:  end  

    //RptFilter.DatumDo = RptFilter.OtsDate;    // 31.01. / 30.04. / 31.07. / 31.10 
      RptFilter.DatumDo = RptDate_RazdobljeEnd; // 31.12. / 31.03. / 30.06. / 30.09 

      VvHamper.Set_DateTimePicker_ThreadSafe(TheReportUC.TheRiskFilterUC.dtp_DatumDO, RptFilter.DatumDo);
      //TheReportUC.TheRiskFilterUC.Fld_DatumDo = RptFilter.DatumDo;

      // Redesign FilterMembersa za datume 

      RptFilter.FilterMembers.RemoveAll(fm => fm.name.ToLower().StartsWith("date"));

    //RptFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch  [FakCI.dokDate]   , false, "DateOD"   , RptFilter.DatumOd      , RptFilter.DatumOd      .ToString("dd.MM.yyyy."), "Od datuma:", " >= ", ""));
    //RptFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch  [FakCI.dokDate]   , false, "DateDO"   , RptFilter.DatumDo      , RptFilter.DatumDo      .ToString("dd.MM.yyyy."), "Do datuma:", " <= ", ""));

      // 26.08.2016: 
    //RptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.dospDate], false, "DateOD"   , RptFilter.DatumOd      , RptFilter.DatumOd      .ToString("dd.MM.yyyy."), "Od datuma:", " >= ", ""));
      RptFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch  [FakCI  .dokDate ], false, "DateOD"   , RptFilter.DatumOd      , RptFilter.DatumOd      .ToString("dd.MM.yyyy."), "Od datuma:", " >= ", ""));

      RptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.dospDate], false, "DateDO"   , RptFilter.DatumDo      , RptFilter.DatumDo      .ToString("dd.MM.yyyy."), "Do datuma:", " <= ", ""));
    //RptFilter.FilterMembers.Add(new VvSqlFilterMember(FtrSch  [FtrCI.t_dokDate] , false, "DateOD"   , RptFilter.DatumOd      , RptFilter.DatumOd      .ToString("dd.MM.yyyy."), "Od datuma:", " >= ", ""));
      RptFilter.FilterMembers.Add(new VvSqlFilterMember(FtrSch  [FtrCI.t_dokDate] , false, "DateDOftr", RptDate_NisuNaplaceniDo, RptDate_NisuNaplaceniDo.ToString("dd.MM.yyyy."), "Do datuma:", " <= ", ""));

   }

   public override int FillRiskReportLists()
   {
      SetReportDates(); // !!! 

      base.FillRiskReportLists(); // initial TheFtransList as in classic RptR_OTS 

      // 30.08.2016: 
      TheFtransList.RemoveAll(ftr => ftr.T_tipBr == Faktur.TT_IRM); // !!! 

#region Get Ftranses Fakturs

      TheFakturList = new List<Faktur>();
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(7);

      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate] , "dateOD", RptFilter.DatumOd, " >= ")); // 6 godina unazad  
      // 26.08.2016: 
    //filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate ], "dateDO", RptFilter.DatumDo, " <= ")); // Do datuma kraja razdoblja koje proucavamo        
      filterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.dospDate], "dateDO", RptFilter.DatumDo, " <= ")); // Do datuma kraja razdoblja koje proucavamo        

      filterMembers.Add(new VvSqlFilterMember("tt", TtInfo.IzlazniPdv_IN_Clause, " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!

      //WHERE UPPER(nacPlac) = 'VIRMAN'
      filterMembers.Add(new VvSqlFilterMember("UPPER(nacPlac)"                 , "'VIRMAN'"  , " = ", ZXC.FM_OR_Enum.OPEN_OR ));
      filterMembers.Add(new VvSqlFilterMember("UPPER(SUBSTRING(nacPlac, 1, 8))", "'TRANSAKC'", " = ", ZXC.FM_OR_Enum.NONE    ));
      filterMembers.Add(new VvSqlFilterMember("UPPER(SUBSTRING(nacPlac, 1, 5))", "'POUZE'"   , " = ", ZXC.FM_OR_Enum.NONE    ));
      filterMembers.Add(new VvSqlFilterMember("UPPER(nacPlac)", "''"                         , " = ", ZXC.FM_OR_Enum.CLOSE_OR));

      VvDaoBase.LoadGenericVvDataRecordList(TheDbConnection, TheFakturList, filterMembers, "", "dokDate , ttSort, ttNum ", true);

#endregion Get Ftranses Fakturs

#region TheFakturList varijanta - ugasi kasnije nepotrebno

   //// ODLUCI: ILI TheFakturList ILI TheFtransList bude ReportSource 
   //// ovu faktur listu jos treba presortirati na isti nacin kao i ftrans listu
   //// iz faktur liste jos izbaciti racune koji uopce jos niti nisu prebaceni na nalog???!!!
   //
   //int zakas; decimal ukZatvoreno; Ftrans ftrans_rec;
   //foreach(Faktur faktur in TheFakturList)
   //{
   //   ftrans_rec = TheFtransList.FirstOrDefault(ftr => ftr.T_tipBr == faktur.TT_And_TtNum && ftr.T_otsKind == ZXC.OtsKindEnum.OTVARANJE);
   //   if(ftrans_rec != null) zakas = ftrans_rec.OtsZakas;
   //   else                   zakas = 0;
   //
   //   ukZatvoreno = TheFtransList.Where(ftr => ftr.T_tipBr == faktur.TT_And_TtNum).Sum(ftr => ftr.OtsZatvor);
   //
   //   faktur.RokIsporuke = zakas;
   //   faktur.R2_uplata   = ukZatvoreno;
   //}

#endregion TheFakturList varijanta - ugasi kasnije nepotrebno

#region New TheFtransList GrouppedByKcdAndTipBr and with additional Faktur data (WyrnJoin_dokDate, WyrnJoin_sUkPDV, WyrnJoin_ttNumStr)

#region Remove eventual manyOtvaranja PASS

      var ftransesGrouppedByKcdAndTipBr_A = TheFtransList.GroupBy(ftr => ftr.T_kupdob_cd.ToString("000000") + ftr.T_tipBr);

      bool    hasManyOtvaranja          ; 
      bool    isIODnotSTORNO            ; 
      string  TipBr_IOD                 ;
      int     otvaranjaCount            ;
      decimal sumOtvorPDV = 0.0M        ;
      Faktur  fakturPrvoOtvaranje_rec = null;
      Faktur  fakturDodatnoOtvaranje_rec;

      // Remove eventual manyOtvaranja PASS 
      foreach(var ftransGR in ftransesGrouppedByKcdAndTipBr_A)
      {
         var ftransGR_Otvaranja = ftransGR.Where(ftr => ftr.T_otsKind == ZXC.OtsKindEnum.OTVARANJE);
         otvaranjaCount = ftransGR_Otvaranja.Count();
         hasManyOtvaranja = otvaranjaCount > 1;
         if(hasManyOtvaranja) // npr. Partial STORNO (vise od jednog otvaranja - same T_tipBr different T_fakRecID) 
         {
            for(int i=0; i < otvaranjaCount; ++i)
            {
               if(i.IsPositive()) // ako je i > 1 ... sve osim prvog izbaci, a prvome pridodaj vrijednosti izbacenog 
               {
                  // Ftrans 
                  ftransGR_Otvaranja.ElementAt(0).OtsOtvor += ftransGR_Otvaranja.ElementAt(i).OtsOtvor;
                  TheFtransList.Remove(ftransGR_Otvaranja.ElementAt(i)); // the JOB 

                  // Faktur 
                  isIODnotSTORNO = ftransGR_Otvaranja.ElementAt(i).T_opis.StartsWith("IOD") ||
                                   ftransGR_Otvaranja.ElementAt(i).T_opis.StartsWith("IPV")  ;

                  // ovo pase za AUTOMATSKI storno (TipBr vodi na glavni rn a FakRecID na storno) 
                  if(isIODnotSTORNO == false) // ovo je STORNO
                  {
                     fakturDodatnoOtvaranje_rec = TheFakturList.SingleOrDefault(fak => fak.RecID == ftransGR_Otvaranja.ElementAt(i).T_fakRecID);
                  }
                  // ovo pase za RUCNI storno-IOD-... (TipBr I FakRecID vode na glavni rn) 
                  else // ovo JE IOD 
                  {
                     TipBr_IOD = ftransGR_Otvaranja.ElementAt(i).T_opis.Replace(" broj ", "-");
                     fakturDodatnoOtvaranje_rec = TheFakturList.SingleOrDefault(fak => fak.TipBr == TipBr_IOD);
                  }

                  if(fakturDodatnoOtvaranje_rec != null) sumOtvorPDV += fakturDodatnoOtvaranje_rec.S_ukPdv;
               }
            } // for(int i=0; i < otvaranjaCount; ++i) 

            fakturPrvoOtvaranje_rec = TheFakturList.SingleOrDefault(fak => fak.TipBr == ftransGR_Otvaranja.ElementAt(0).T_tipBr);
            if(fakturPrvoOtvaranje_rec != null) fakturPrvoOtvaranje_rec.S_ukPdv += sumOtvorPDV;


         } // if(hasManyOtvaranja) 

      } // foreach(var ftransGR in ftransesGrouppedByKcdAndTipBr) 

#endregion Remove eventual manyOtvaranja PASS

      var ftransesGrouppedByKcdAndTipBr_B = TheFtransList.GroupBy(ftr => ftr.T_kupdob_cd.ToString("000000") + ftr.T_tipBr);

      TheFtransList = new List<Ftrans>(ftransesGrouppedByKcdAndTipBr_B.Count());

      decimal ukOtsZatvor;
      Ftrans ftransOtvaranja_rec;

      // normal, JOB PASS 
      foreach(var ftransGR in ftransesGrouppedByKcdAndTipBr_B)
      {
         ftransOtvaranja_rec = ftransGR.SingleOrDefault(ftr => ftr.T_otsKind == ZXC.OtsKindEnum.OTVARANJE);

         if(ftransOtvaranja_rec != null)
         {
            ukOtsZatvor = ftransGR.Where(ftr => ftr.T_otsKind == ZXC.OtsKindEnum.ZATVARANJE).Sum(ftr => ftr.OtsZatvor);
            ftransOtvaranja_rec.OtsZatvor = ukOtsZatvor;
            TheFtransList.Add(ftransOtvaranja_rec);
         }
         else
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Nedostaje otvaranje računa[{0}]", ftransGR.First());
         }
      }

      // resort again 
      if(RptFilter.IsForceOtsByDokDate)
      {
         TheFtransList = TheFtransList.OrderBy(ftr => ftr.T_ticker).ThenBy(ftr => ftr.T_dokDate).ThenBy(ftr => ftr.R_tipBr_Resorted).ThenBy(ftr => ftr.T_serial).ToList();
      }
      else // default
      {
         TheFtransList = TheFtransList.OrderBy(ftr => ftr.T_ticker).ThenBy(ftr => ftr.R_tipBr_Resorted).ThenBy(ftr => ftr.T_dokDate).ThenBy(ftr => ftr.T_serial).ToList();
      }

#region set additional Faktur data (WyrnJoin_dokDate, WyrnJoin_sUkPDV, WyrnJoin_ttNumStr)

      Faktur faktur_rec;
      foreach(Ftrans ftrans in TheFtransList)
      {
         faktur_rec = TheFakturList.SingleOrDefault(fak => fak.TipBr == ftrans.T_tipBr);
         ftrans.WyrnJoin_dokDate  = faktur_rec == null ? DateTime.MinValue : faktur_rec.DokDate      ;
         // 15.04.2020: !!! 
       //ftrans.WyrnJoin_sUkPDV   = faktur_rec == null ? 0M                : faktur_rec.S_ukPdv      ;
         ftrans.WyrnJoin_sUkPDV   = faktur_rec == null ? 0M                : faktur_rec.S_ukPdv.Ron2();
       //15.02.2017. ako je knjigovodstveni servis onda bi broj racuna trebao dolaziti iz OrigBrDok tj. vezniDok je im tamo bas i neide sve po nekom smislenom redu
       //ftrans.WyrnJoin_ttNumStr = faktur_rec == null ? ""                                                               : faktur_rec.OpzStat1TtNum;
         ftrans.WyrnJoin_ttNumStr = faktur_rec == null ? ""                : RptFilter.IsOpzStatRnVezDok ? faktur_rec.VezniDok : faktur_rec.OpzStat1TtNum;

         // 26.08.2016: 
         if(faktur_rec != null && ftrans.T_valuta != faktur_rec.DospDate)
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "Valuta naloga {0}\ni\nDospijece rn-a {1}\nsu razliciti!\n\n{2}", 
               ftrans.T_valuta.ToString(ZXC.VvDateFormat), faktur_rec.DospDate.ToString(ZXC.VvDateFormat), ftrans);
         }

         // dal' u biti tu treba izbaciti usamljene avanse/zatvaranja? 
       //if(faktur_rec == null)
         if(faktur_rec == null && ftrans.T_tipBr.StartsWith(Faktur.TT_IRM) == false)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Nema računa (u RISK-u)\n\n[{0}]\n\nNe mogu saznati datum računa, pdv iznos i broj računa\n\n{1}", ftrans.T_tipBr, ftrans);
         }
      }

#endregion set additional Faktur data (WyrnJoin_dokDate, WyrnJoin_sUkPDV, WyrnJoin_ttNumStr)

#endregion New TheFtransList GrouppedByKcdAndTipBr and with additional Faktur data (WyrnJoin_dokDate, WyrnJoin_sUkPDV, WyrnJoin_ttNumStr)

      return 0;
   }

#region XML Export

   // ovdje nisi nasao nikakve upute kako bi se trebao zvati fajl pa ga imenujes proizvoljno 
   public override string ExportFileName
   {
      get
      {
         string mmyyyy = RptDate_NisuNaplaceniDo.Month.ToString("00") + RptDate_NisuNaplaceniDo.Year.ToString("0000");
         string   yyyy =                                                RptDate_NisuNaplaceniDo.Year.ToString("0000");

         return "OPZ-Stat1_" + ZXC.CURR_prjkt_rec.Oib + "_" + mmyyyy + ".xml";
      }
   }

#region Xml Schema Validation

   public override bool ExecuteExportValidation(string fileName)
   {
      List<ZXC.VvXmlValidationData> valDataList = new List<ZXC.VvXmlValidationData>();

      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacOPZ/v1-0", @"XSD\ObrazacOPZ-v1-0.xsd"));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacOPZ/v1-0", @"XSD\ObrazacOPZtipovi-v1-0.xsd"));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\ObrazacOPZmetapodaci-v1-0.xsd"));

      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\MetapodaciTipovi-v2-0.xsd"));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/TemeljniTipovi/v2-1"     , @"XSD\TemeljniTipovi-v2-1.xsd"  ));

      return ExecuteExportValidation_Base(valDataList);
   }

#endregion Xml Schema Validation
   
   public override bool ExecuteExport(string fileName)
   {
#region Initialize XmlWriterSettings

      if(TheFtransList.Count().IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Nema se što exporitrati!");
         return false;
      }

      string ident = "   ";

      XmlWriterSettings settings = new XmlWriterSettings();
      
      settings.Indent      = true;
      settings.IndentChars = ident;

#endregion Initialize XmlWriterSettings

      using(XmlWriter writer = XmlWriter.Create(fileName, settings))
      {
#region Init Xml Document

         writer.WriteStartDocument();

         writer.WriteStartElement("ObrazacOPZ", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacOPZ/v1-0");
         writer.WriteAttributeString("verzijaSheme",                                                           "1.0");

#endregion Init Xml Document

#region Metapodaci

         writer.WriteStartElement("Metapodaci", @"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0");
            //writer.WriteElementString("Naslov", @"http://purl.org/dc/elements/1.1/title", "Prijava poreza na dodanu vrijednost");
            writer.WriteRaw("\n");
            writer.WriteRaw(ident + ident + "<Naslov dc=\"http://purl.org/dc/elements/1.1/title\">Obrazac OPZ</Naslov>\n");
            writer.WriteRaw(ident + ident + "<Autor dc=\"http://purl.org/dc/elements/1.1/creator\">" + ZXC.CURR_prjkt_rec.Ime + " " + ZXC.CURR_prjkt_rec.Prezime + "</Autor>\n");
            writer.WriteRaw(ident + ident + "<Datum dc=\"http://purl.org/dc/elements/1.1/date\">" + DateTime.Now.ToString("s") + "</Datum>\n");
            writer.WriteRaw(ident + ident + "<Format dc=\"http://purl.org/dc/elements/1.1/format\">text/xml</Format>\n");
            writer.WriteRaw(ident + ident + "<Jezik dc=\"http://purl.org/dc/elements/1.1/language\">hr-HR</Jezik>\n");
            writer.WriteRaw(ident + ident + "<Identifikator dc=\"http://purl.org/dc/elements/1.1/identifier\">" + Guid.NewGuid().ToString("D")/*.ToUpper()*/ + "</Identifikator>\n");

            writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacOPZ-v1-0</Uskladjenost>\n");

            writer.WriteRaw(ident + ident + "<Tip dc=\"http://purl.org/dc/elements/1.1/type\">Elektronički obrazac</Tip>\n");
            writer.WriteRaw(ident + ident + "<Adresant>Ministarstvo Financija, Porezna uprava, Zagreb</Adresant>\n");
            writer.WriteRaw(ident);
         writer.WriteEndElement(); // Metapodaci 

#endregion Write Header Data

#region Zaglavlje

         writer.WriteStartElement("Zaglavlje");

            writer.WriteStartElement("Razdoblje"); 
               writer.WriteElementString("DatumOd", RptDate_RazdobljeStart.ToString("s").Substring(0, 10));
               writer.WriteElementString("DatumDo", RptDate_RazdobljeEnd  .ToString("s").Substring(0, 10));
            writer.WriteEndElement(); // Razdoblje 

            writer.WriteStartElement("PorezniObveznik");
               writer.WriteElementString("OIB"  , ZXC.CURR_prjkt_rec.Oib  );
               writer.WriteElementString("Naziv", ZXC.CURR_prjkt_rec.Naziv);
               writer.WriteStartElement("Adresa");
                  writer.WriteElementString("Mjesto", ZXC.CURR_prjkt_rec.Grad);
                  writer.WriteElementString("Ulica" , ZXC.CURR_prjkt_rec.UlicaBezBroja_1);
                  writer.WriteElementString("Broj"  , ZXC.CURR_prjkt_rec.UlicniBroj_1);
               writer.WriteEndElement(); // Adresa 
               writer.WriteElementString("Email"    , ZXC.CURR_prjkt_rec.Email);
            writer.WriteEndElement(); // Obveznik 

            writer.WriteStartElement("IzvjesceSastavio");
               writer.WriteElementString("Ime"    , ZXC.CURR_prjkt_rec.Ime    );
               writer.WriteElementString("Prezime", ZXC.CURR_prjkt_rec.Prezime);
               writer.WriteElementString("Telefon", /*ZXC.CURR_prjkt_rec.Tel1 zbog validacije:*/ZXC.CURR_prjkt_rec.Tel2);
               writer.WriteElementString("Fax"    , /*ZXC.CURR_prjkt_rec.Tel1 zbog validacije:*/ZXC.CURR_prjkt_rec.Tel2);
               writer.WriteElementString("Email"  , ZXC.CURR_prjkt_rec.Email);
            writer.WriteEndElement(); // IzvjesceSastavio 

          //writer.WriteElementString("Ispostava", ZXC.CURR_prjkt_rec.OpcCd);
            writer.WriteElementString("NaDan",           RptDate_NaDan          .ToString("s").Substring(0, 10));
            writer.WriteElementString("NisuNaplaceniDo", RptDate_NisuNaplaceniDo.ToString("s").Substring(0, 10)); 

         writer.WriteEndElement(); // Zaglavlje 

#endregion Zaglavlje

#region Tijelo

         Kupdob kupdob_rec;

         var ftransesGrouppedByTicker = TheFtransList.GroupBy(ftr => ftr.T_ticker);

         int rbrKcda = 0;
         int rbrRacuna;
         writer.WriteStartElement("Tijelo");

         writer.WriteStartElement("Kupci");
         foreach(var onePartnerftrGR in ftransesGrouppedByTicker)
         {
            kupdob_rec = TheReportUC.Get_Kupdob_FromVvUcSifrar(onePartnerftrGR.First().T_kupdob_cd);
            if(kupdob_rec == null) kupdob_rec = new Kupdob();

            writer.WriteStartElement("Kupac");

               writer.WriteElementString("K1",(++rbrKcda).ToString());
               writer.WriteElementString("K2", kupdob_rec.OznPorBr_opzStat1);
               writer.WriteElementString("K3", kupdob_rec.OIB_opzStat1     );
               writer.WriteElementString("K4", kupdob_rec.Naziv            );

               writer.WriteElementString("K5", onePartnerftrGR.Sum(f => f.WyrnJoin_sUkKCR).ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("K6", onePartnerftrGR.Sum(f => f.WyrnJoin_sUkPDV).ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("K7", onePartnerftrGR.Sum(f => f.OtsOtvor       ).ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("K8", onePartnerftrGR.Sum(f => f.OtsZatvor      ).ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("K9", onePartnerftrGR.Sum(f => f.OtsSaldo       ).ToStringVv_NoGroup_ForceDot());

               rbrRacuna = 0;
               writer.WriteStartElement("Racuni");
               foreach(Ftrans oneRacunftrans in onePartnerftrGR)
               {
                  writer.WriteStartElement("Racun");

                  writer.WriteElementString("R1" , (++rbrRacuna)                  .ToString());
                  writer.WriteElementString("R2" , oneRacunftrans.WyrnJoin_dokDate.Year.ToString() + "-" + oneRacunftrans.WyrnJoin_ttNumStr);
                  writer.WriteElementString("R3" , oneRacunftrans.WyrnJoin_dokDate.ToString("s").Substring(0, 10));
                  writer.WriteElementString("R4" , oneRacunftrans.T_valuta        .ToString("s").Substring(0, 10));
                  writer.WriteElementString("R5" , oneRacunftrans.OtsZakas        .ToString()); // zakas 
                  writer.WriteElementString("R6" , oneRacunftrans.WyrnJoin_sUkKCR .ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("R7" , oneRacunftrans.WyrnJoin_sUkPDV .ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("R8" , oneRacunftrans.OtsOtvor        .ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("R9" , oneRacunftrans.OtsZatvor       .ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("R10", oneRacunftrans.OtsSaldo        .ToStringVv_NoGroup_ForceDot());

                  writer.WriteEndElement(); // Racun 

               } // foreach(Ftrans oneRacunftrans in onePartnerftrGR) 
               writer.WriteEndElement(); // Racuni 
               
            writer.WriteEndElement(); // Kupac 

         } // foreach(var onePartnerftrGR in ftransesGrouppedByTicker) 
         writer.WriteEndElement(); // Kupci 

         writer.WriteElementString("UkupanIznosRacunaObrasca"       , TheFtransList.Sum(f => f.WyrnJoin_sUkKCR).ToStringVv_NoGroup_ForceDot());
         writer.WriteElementString("UkupanIznosPdvObrasca"          , TheFtransList.Sum(f => f.WyrnJoin_sUkPDV).ToStringVv_NoGroup_ForceDot());
         writer.WriteElementString("UkupanIznosRacunaSPdvObrasca"   , TheFtransList.Sum(f => f.OtsOtvor       ).ToStringVv_NoGroup_ForceDot());
         writer.WriteElementString("UkupniPlaceniIznosRacunaObrasca", TheFtransList.Sum(f => f.OtsZatvor      ).ToStringVv_NoGroup_ForceDot());
         writer.WriteElementString("NeplaceniIznosRacunaObrasca"    , TheFtransList.Sum(f => f.OtsSaldo       ).ToStringVv_NoGroup_ForceDot());
         writer.WriteElementString("OPZUkupanIznosRacunaSPdv"       , 0.00M                                    .ToStringVv_NoGroup_ForceDot()); // ??? 
         writer.WriteElementString("OPZUkupanIznosPdv"              , 0.00M                                    .ToStringVv_NoGroup_ForceDot()); // ??? 

         writer.WriteEndElement(); // Tijelo 

#endregion Tijelo

#region Finish Xml Document

         writer.WriteEndElement(); // ObrazacOPZ
         writer.WriteEndDocument();

#endregion Finish Xml Document

      }

      return true;
   }

#endregion ePDV XML Export

   public static bool IsFilterWellFormed(RiskReportUC reportUC)
   {
      // 25.02.2020: od 2020 god. ne predaju se vise kvartalni 
      // nego jemput za cio PG                                 
      return true;

      bool OK = true;
      VvRpt_RiSk_Filter filter = reportUC.TheRptFilter;

      DateTime NisuNaplaceniDo = reportUC.TheRiskFilterUC.Fld_OtsDate;

      string otsDDMM = NisuNaplaceniDo.Day.ToString("00") + NisuNaplaceniDo.Month.ToString("00");

      string okDDMM1 = "3101";
      string okDDMM2 = "3004";
      string okDDMM3 = "3107";
      string okDDMM4 = "3110";

      if(otsDDMM != okDDMM1 &&
         otsDDMM != okDDMM2 &&
         otsDDMM != okDDMM3 &&
         otsDDMM != okDDMM4)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Molim, u datum IOS na dan zadajte jedan od datuma:\n\n{0}\n\n{1}\n\n{2}\n\n{3}",
            ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(okDDMM1 + ZXC.projectYear).ToString(ZXC.VvDateFormat),
            ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(okDDMM2 + ZXC.projectYear).ToString(ZXC.VvDateFormat),
            ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(okDDMM3 + ZXC.projectYear).ToString(ZXC.VvDateFormat),
            ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(okDDMM4 + ZXC.projectYear).ToString(ZXC.VvDateFormat));

         return false;
      }

      return (OK);
   }

}

public class RptR_Dugovanja    : RptR_OTS  
{
   public RptR_Dugovanja(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, bool _isKupci): base(_reportDocument, _reportName, _rptFilter)
   {
       this.IsOtsKupaca = _isKupci;
   }
}

public class RptR_Potrazivanja : RptR_OTS  
{
   public RptR_Potrazivanja(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, bool _isKupci): base(_reportDocument, _reportName, _rptFilter)
   {
       this.IsOtsKupaca = _isKupci;
   }
}

public class RptR_Kompenzacija : RptR_OTS  
{
   public RptR_Kompenzacija(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, bool _isKupci): base(_reportDocument, _reportName, _rptFilter)
   {
      this.IsOtsKupaca = _isKupci;
      RptFilter.IsOtsAnalitKontre = true;
   }

   internal void SetMoneyAsText_inPrjkt()
   {
      decimal saldo_duznik    = TheFtransList.Where(ftr => ftr.OtsIsKontra == false).Sum(ftr => ftr.OtsOtvor) -
                                TheFtransList.Where(ftr => ftr.OtsIsKontra == false).Sum(ftr => ftr.OtsZatvor);
      decimal saldo_vjerovnik = TheFtransList.Where(ftr => ftr.OtsIsKontra == true ).Sum(ftr => ftr.OtsOtvor) -
                                TheFtransList.Where(ftr => ftr.OtsIsKontra == true ).Sum(ftr => ftr.OtsZatvor);

      decimal saldo_kompenzacije = Math.Abs(saldo_duznik - saldo_vjerovnik);

      ThePrjktList[0].OtsSaldoKompenzacijeAsText = ZXC.KuneIlipe(saldo_kompenzacije);

      RptFilter.OtsSaldoKompenzacijaAsText = ZXC.KuneIlipe(saldo_kompenzacije);
   }
   
   public static bool IsFilterWellFormed(RiskReportUC reportUC)
   {
      bool OK = true;
      VvRpt_RiSk_Filter filter = reportUC.TheRptFilter;

      if(filter.KD_sifra.IsZero())
      {
         ZXC.aim_emsg("Molim, zadajte PARTNERA.");
         return false;
      }

      return (OK);
   }

}

#endregion OTS

#region KAMATE i KARTICA partnera

public class RptR_Kamate_Kartica          : RptR_StandardRiskReport
{
#region Constructor 

   public bool IsOtsKupaca     { get; set; }
   public bool IsOtsDobavljaca { get { return !IsOtsKupaca; } }

   public bool IsKamate        { get; set; }
   public bool IsKartica       { get { return !IsKamate; } }

   public RptR_Kamate_Kartica(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter) : base(_reportDocument, _reportName, _rptFilter, ZXC.RIZ_FilterStyle.Ftrans,
         /* _rptNeeds_ArtWars      */            false, // ArtiklWithArtstat  
         /* _rptNeeds_ArtStat      */            false, // ArtStat        
         /* _rptNeeds_Faktur       */            false, // Faktur         
         /* _rptNeeds_Rtrans       */            false, // Rtrans         
         /* _rptNeeds_Kupdob       */            true , // Kupdob         
         /* _rptNeeds_Prjkt        */            false, // Prjkt          za kompenzaciju 
         /* _rptNeeds_Rtrans4ruc   */            false, // Rtrans4ruc     
         /* _rptNeeds_Artikl       */            false) // Artikl         

   {
      ReportNeeds_Ftrans_List = true;

      TheKamateReportList = new List<KamateReportLine>();
   }

#endregion Constructor

   public override int FillRiskReportLists()
   {
#region Init Lists 

      // base.FillRiskReportLists(); 

      TheFtransList = new List<Ftrans>();

      List<Ftrans> theRacuniList = new List<Ftrans>();
      List<Ftrans> theUplateList = new List<Ftrans>();

      NalogDao.LoadFtransListFor_OtsReport(TheDbConnection, TheFtransList, RptFilter.FilterMembers, /* DateTime.Now */ RptFilter.OtsDate, IsOtsKupaca, RptFilter.IsOtsAnalitKontre, RptFilter.IsOtsDospOnly, true);

      TheKupdobList = VvUserControl.KupdobSifrar.Join(TheFtransList, kpdb => kpdb.KupdobCD, ftrans => ftrans.T_kupdob_cd, (k, f) => k).Distinct().ToList();

#region Check Any EMPTY T_tipBr

      if(IsKamate && TheFtransList.Any(ftr => ftr.T_tipBr.IsEmpty())) // Samo za kamate 
      {
         string errMessage = "";
         
         foreach(Ftrans emptyFtrans_rec in TheFtransList.Where(ftr => ftr.T_tipBr.IsEmpty()))
         {
            errMessage += emptyFtrans_rec.ToShortString() + "\n";
         }

         MessageBox.Show(errMessage, "OBRAČUN KAMATA NEĆE BITI ISPRAVAN jer postoje NEVEZANE STAVKE! ", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    //if(IsKartica && TheFtransList.Any(ftr => ftr.T_tipBr.IsEmpty()) && RptFilter.KD_sifra.IsZero()) // Samo za karticu 
      if(IsKartica && TheFtransList.Any(ftr => ftr.T_tipBr.IsEmpty()) && RptFilter.KD_sifra.IsZero() && RptFilter.AnalitSintet == "A") // Samo za karticu i samo kada je analitika jer za 'S' se ne racuna zakasnina
      {
         string errMessage = "";

         foreach(string ticker in TheFtransList.Where(ftr => ftr.T_tipBr.IsEmpty()).Select(ftr => ftr.T_ticker).Distinct())
         {
            errMessage += ticker + " ima nevezane stavke\n";
         }

         MessageBox.Show(errMessage, "DANI KAŠNJENJA ĆE BITI NEISPRAVNI! ", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

#endregion Check Any EMPTY T_tipBr

      // Makni iz TheFtransListe sve stavke (racune i uplate) koji su 'placeni na vrijeme' tj zakas im je ZeroOrNegative 
      GetOtsZakas_And_MakniDobreRacuneForKamate(TheFtransList, RptFilter.OtsDate); // !!! 

      if(TheFtransList.Count.IsZero()) return 0;

      // KAMATE i KARTICA partnera ADDITIONS: ________________________________________________________________________________________________________________________ 

      if(IsKamate) // za Kamate, listu racuna sortiraj po t_valuta, a za Karticu recune sortiraj po t_dokDate 
      theRacuniList = TheFtransList.Where(ftr => ftr.T_otsKind == ZXC.OtsKindEnum.OTVARANJE ).OrderBy(ftr => ftr.T_ticker).ThenBy(ftr => ftr.T_valuta ).ThenBy(ftr => ftr.R_tipBr_Resorted).ThenBy(ftr => ftr.T_serial).ToList();
      else                                                                                  
      theRacuniList = TheFtransList.Where(ftr => ftr.T_otsKind == ZXC.OtsKindEnum.OTVARANJE ).OrderBy(ftr => ftr.T_ticker).ThenBy(ftr => ftr.T_dokDate).ThenBy(ftr => ftr.R_tipBr_Resorted).ThenBy(ftr => ftr.T_serial).ToList();

      theUplateList = TheFtransList.Where(ftr => ftr.T_otsKind == ZXC.OtsKindEnum.ZATVARANJE).OrderBy(ftr => ftr.T_ticker).ThenBy(ftr => ftr.T_dokDate).ThenBy(ftr => ftr.R_tipBr_Resorted).ThenBy(ftr => ftr.T_serial).ToList();

#endregion Init Lists

#region GetKamateReportLines (as in Remonster)

#region variablez

      KamateReportLine reportLine;

      Ftrans   racun_rec ;
      Ftrans   uplata_rec;
      decimal  osnovica, kamata_ukupno = 0.00M;
      DateTime dateDO;
      bool     isThereAnyUplata = theUplateList.Count.NotZero();
      decimal  kumKamataRn = 0.00M, kumOsnovRn = 0.00M;
      decimal  fondOveUplate;
      decimal  fondPreplate = 0;

#endregion variablez

#region Dodaj umjetnu zadnju 'Teoretska' uplatu ... sta bi bilo kada bi danas (ReportOnThisDate) htjeli cjelovito zatvoriti DUG

      if(IsKamate) // Samo za kamate 
      {
         uplata_rec = new Ftrans();
         
         uplata_rec.T_ticker  = TheFtransList.First().T_ticker;
         uplata_rec.T_dokDate = RptFilter.OtsDate             ;
         uplata_rec.T_otsKind = ZXC.OtsKindEnum.ZATVARANJE    ;
         //uplata_rec.OtsZatvor = jos ga tu ne znamo;

         theUplateList.Add(uplata_rec);
      }

#endregion Dodaj umjetnu zadnju 'Teoretska' uplatu ... sta bi bilo kada bi danas (ReportOnThisDate) htjeli cjelovito zatvoriti DUG

      if(IsKamate) // 11.02.2012. // Samo za kamate 
      {
         for(int uplIdx = 0; uplIdx < theUplateList.Count; ++uplIdx) // petlja po UPLATAma 
         {
            uplata_rec = theUplateList[uplIdx];

            dateDO = uplata_rec.T_dokDate;

            if(IsKamate && uplIdx.IsPositive()) // neka sljedeca uplata, NIJE prva, dodaj preostali dug nakon prethodne uplate i obavi eventualno preostale racune
            {
               racun_rec = new Ftrans();
               racun_rec.T_tipBr = "OSTATAK";
               racun_rec.OtsOtvor = TheKamateReportList.Last().R_KumDugOSN;
               racun_rec.T_valuta = TheKamateReportList.Last().DateUpl;

               theRacuniList.Insert(0, racun_rec);

               //kumKamataRn = 0.00M; 
               kumOsnovRn = 0.00M;
            }

            for(int i = 0; i < theRacuniList.Count; ++i) // petlja po RACUNima + na vrh ubacen DUG IZ PRETHODNOG RAZDOBLJA (ako su Kamate a ne Kartica) 
            {
               if(IsKamate  && theRacuniList[i].T_valuta  > uplata_rec.T_dokDate) break;
               if(IsKartica && theRacuniList[i].T_dokDate > uplata_rec.T_dokDate) break;

               osnovica = theRacuniList[i].OtsOtvor;

               // 07.11.2014: dodan if
               if(theRacuniList[i].T_valuta.IsEmpty())
                  ZXC.aim_emsg(MessageBoxIcon.Error, "Stavka\n\n{0}\n\nima datum valute prazan.\n\nPRESKAČEM!", theRacuniList[i]);
               else
                  kamata_ukupno += CalcAndFprintfKamate(IsKamate, TheKamateReportList, ZXC.OtsKindEnum.OTVARANJE, theRacuniList[i], /*dtranu_rec,*/ theRacuniList[i].T_valuta, uplata_rec.T_dokDate, osnovica, ref kumKamataRn, ref kumOsnovRn);

               theRacuniList.Remove(theRacuniList[i--]);
            }

#region Create UPLATA KamateReportLine AndAddItTo TheKamateReportList

            // 07.11.2014: dodan if
            if(TheKamateReportList.Count.IsZero()) return 0;

            //if(uplIdx == theUplateList.Count - 1 && debit_rec.IsAdActa == false)   // Za onu zadnju 'Teoretsku' uplatu treba zadati ostatak duga 
            // ali samo ako nije AdActa 
            if(IsKamate && uplIdx == theUplateList.Count - 1 && fondPreplate.IsZeroOrNegative()) // 16.01.2013. odnosno ako nije u preplati odnosno kada fondPreplate NIJE > 0
            {
               //if(TheKamateReportList.Count.IsPositive()) // if dodan 05.02.2013: ima situacija kada je TheKamateReportList prazan pa ide Exception
               {
                  uplata_rec.OtsZatvor = TheKamateReportList.Last().R_KumDugALL /*+ fondPreplate*/;
               }
            }
            else
            {
               reportLine = new KamateReportLine();

               reportLine.CustCode = uplata_rec.T_ticker;
               reportLine.TTkind = ZXC.OtsKindEnum.ZATVARANJE;
               reportLine.BrojRn = "UPLATA";
               reportLine.IznosUpl = uplata_rec.OtsZatvor;
               reportLine.DateUpl = uplata_rec.T_dokDate;

               fondOveUplate = reportLine.IznosUpl;

               fondOveUplate = fondOveUplate.PopapajUplatuNa(ref kumKamataRn, false);
               fondOveUplate = fondOveUplate.PopapajUplatuNa(ref kumOsnovRn, true);

               fondPreplate += fondOveUplate;  // 16.01.2013. kada je fondPreplate > 0 onda bi trebalo zaustaviti racunanje kamata,  te prikazati eventualne nove uplate ali bez obrKta

               reportLine.KumKamataRn = kumKamataRn;
               reportLine.KumOsnovRn = kumOsnovRn;

               TheKamateReportList.Add(reportLine);
            }

#endregion Create UPLATA KamateReportLine

         } // for(int uplIdx = 0; uplIdx < theUplateList.Count; ++uplIdx) 
      }

#endregion GetKamateTheKamateReportList (as in Remonster)

      return 0;
   }

   private void GetOtsZakas_And_MakniDobreRacuneForKamate(List<Ftrans> theFtransList, DateTime dateOTS)
   {
      List<Ftrans> ftransesToBeRemoved = new List<Ftrans>();

      // odvrti prvo sve racune (OTVARANJA) da eventualna avansna uplata ima dobar ftransOtvaranja_rec.OtsZakas 
      foreach(Ftrans racun_rec in theFtransList.Where(ftr => ftr.T_otsKind == ZXC.OtsKindEnum.OTVARANJE))
      {
         racun_rec.OtsZakas = GetOtsZakas_OTVARANJA(racun_rec, theFtransList, dateOTS);

         if(racun_rec.OtsZakas.IsZeroOrNegative() && IsKamate) // Samo za kamate 
         {
            ftransesToBeRemoved.Add(racun_rec);
         }
      }

      foreach(Ftrans uplata_rec in theFtransList.Where(ftr => ftr.T_otsKind == ZXC.OtsKindEnum.ZATVARANJE))
      {
         uplata_rec.OtsZakas = GetOtsZakas_ZATVARANJA(uplata_rec, theFtransList);

         if(uplata_rec.OtsZakas.IsZeroOrNegative() && IsKamate) // Samo za kamate 
         {
            ftransesToBeRemoved.Add(uplata_rec);
         }
      }

#region Za eventualne OtsRacBezUplate probaj saznati OtsZakas iz nevezanih uplata

      var nevezaniRacuniList = theFtransList.Where(ftr => ftr.T_otsKind == ZXC.OtsKindEnum.OTVARANJE  && ftr.OtsRacBezUplate == true).ToList();
      var nevezaneUplateList = theFtransList.Where(ftr => ftr.T_otsKind == ZXC.OtsKindEnum.ZATVARANJE && ftr.T_tipBr.IsEmpty()      ).ToList();

      if(RptFilter.KD_sifra.NotZero() && // samo za jednoga bumo kemijali, ako su svi, ... fu'k off 
         nevezaniRacuniList.Count.NotZero() &&
         nevezaneUplateList.Count.NotZero()) // dakle, samo ako ima OtsRacBezUplate (nevezanih racuna) 
      {
         decimal  fondPrethodnihUplata = 0.00M;
         decimal  iznosRN                     ;
         decimal  iznosUPL                    ;
         Ftrans   racun_rec                   ;
         Ftrans   lastBefUplataRacun_rec      ;
         Ftrans   uplata_rec                  ;
         DateTime dateOD                      ;
         DateTime fondUplDate = DateTime.MinValue;
         DateTime uplataDate  = DateTime.MinValue;

         for(int racIdx = 0; racIdx < nevezaniRacuniList.Count; ++racIdx) // petlja po nevezanim RACUNima 
         {
            racun_rec = nevezaniRacuniList[racIdx];

            iznosRN = racun_rec.OtsIsURA ? racun_rec.R_PotMinusDug : racun_rec.R_DugMinusPot;

            dateOD = NalogDao.GetOtsDateOd(racun_rec);

            for(int uplIdx = 0; (uplIdx < nevezaneUplateList.Count || fondPrethodnihUplata.IsPositive()) && iznosRN.IsPositive(); ++uplIdx)
            {

               if(nevezaneUplateList.Count.NotZero())
               {
                  uplata_rec = nevezaneUplateList[uplIdx];

                  iznosUPL = uplata_rec.OtsIsURA ? uplata_rec.R_DugMinusPot : uplata_rec.R_PotMinusDug;

                  if(iznosRN <= fondPrethodnihUplata)
                     uplataDate = fondUplDate;
                  else
                     uplataDate = uplata_rec.T_dokDate;

                  fondUplDate = uplata_rec.T_dokDate;
               }
               else
               {
                  uplata_rec = null;
                  iznosUPL   = 0.00M;
                  uplataDate = fondUplDate;
               }

               fondPrethodnihUplata += iznosUPL;

               if(nevezaneUplateList.Count.NotZero())
               {
                  lastBefUplataRacun_rec = nevezaniRacuniList.LastOrDefault(ftr => ftr.T_dokDate <= uplata_rec.T_dokDate);
                  if(lastBefUplataRacun_rec != null)
                     uplata_rec.OtsZakas = ZXC.GetOtsZakas(lastBefUplataRacun_rec.T_dokDate, uplata_rec.T_dokDate);
                  else
                     uplata_rec.OtsZakas = 0;

                  nevezaneUplateList.Remove(nevezaneUplateList[uplIdx--]);
               }

               fondPrethodnihUplata = fondPrethodnihUplata.PopapajUplatuNa(ref iznosRN, false);

               racun_rec.OtsZakas = ZXC.GetOtsZakas(dateOD, uplataDate);

            }

         } // for(int i = 0; i < nevezaniRacuniList.Count; ++i) // petlja po nevezanim RACUNima 

      } 

#endregion Za eventualne OtsRacBezUplate probaj saznati OtsZakas iz nevezanih uplata

      foreach(Ftrans ftrans_rec in ftransesToBeRemoved)
      {
         theFtransList.Remove(ftrans_rec);
      }

   }

   private int GetOtsZakas_OTVARANJA(Ftrans ftrans_rec, List<Ftrans> theFtransList, DateTime dateOTS)
   {
      //if(theFtransList.Where(ftr => ftr.T_tipBr == ftrans_rec.T_tipBr).Sum(ftr => ftr.R_DugMinusPot).IsZero

      DateTime dateOD = NalogDao.GetOtsDateOd(ftrans_rec);

      // ADDITIONS za wors case tj, MaksZakas 
      ftrans_rec.OtsMaksZakas = ZXC.GetOtsZakas(dateOD, dateOTS);

      if(ftrans_rec.T_tipBr.IsEmpty())  // ako je reacunu T_tipBr prazan, vrati ko da nema nijedne uplate 
      {
         ftrans_rec.OtsRacBezUplate = true;
         return ZXC.GetOtsZakas(dateOD, dateOTS);
      }

      var uplateFtransList = theFtransList.Where(ftr => ftr.T_otsKind == ZXC.OtsKindEnum.ZATVARANJE && ftr.T_tipBr == ftrans_rec.T_tipBr);

      decimal iznosRN  =                             ftrans_rec.OtsIsURA ? ftrans_rec.R_PotMinusDug : ftrans_rec.R_DugMinusPot ;
      decimal iznosUPL = uplateFtransList.Sum(ftr => ftr       .OtsIsURA ? ftr       .R_DugMinusPot : ftr       .R_PotMinusDug);

      DateTime dateZadnjeUplate = uplateFtransList.Count().IsZero() ? dateOTS : uplateFtransList.Last().T_dokDate;

      if(uplateFtransList.Count().IsZero()) ftrans_rec.OtsRacBezUplate = true ;
      else                                  ftrans_rec.OtsRacBezUplate = false;

      if(ZXC.AlmostEqual(iznosRN, iznosUPL, 0.05M)) return ZXC.GetOtsZakas(dateOD, dateZadnjeUplate); // rn je zatvoren u cjelosti i u roku    
      else                                          return ZXC.GetOtsZakas(dateOD, dateOTS         ); // rn je cijeli li djelomicno nezatvoren 
   }

   private static int GetOtsZakas_ZATVARANJA(Ftrans ftrans_rec, List<Ftrans> theFtransList)
   {
      Ftrans ftransOtvaranja_rec = theFtransList.FirstOrDefault(ftr => ftr.T_tipBr == ftrans_rec.T_tipBr && ftr.T_otsKind == ZXC.OtsKindEnum.OTVARANJE);

      if(ftransOtvaranja_rec == null)
      {
         return 0;
      }
      else
      {
         // ADDITIONS za wors case tj, MaksZakas 
         ftrans_rec.OtsMaksZakas = ftransOtvaranja_rec.OtsMaksZakas;

         //return ftransOtvaranja_rec.OtsZakas;

         DateTime dateOD = NalogDao.GetOtsDateOd(ftransOtvaranja_rec);
         return ZXC.GetOtsZakas(dateOD,  ftrans_rec.T_dokDate);
      }
   }


   private static decimal CalcAndFprintfKamate(
          bool                   isKamate     ,
          List<KamateReportLine> reportLines  , 
          ZXC.OtsKindEnum        TTkind       , 
          Ftrans                 racun_rec    , 
          DateTime               dateOD       , 
          DateTime               dateDO       , 
          decimal                osnovica     , 
      ref decimal                _kumKamataRn , 
      ref decimal                _kumOsnovRn  )
   {
      KamateReportLine reportLine;

      DateTime dateCurr, dateBreak;
      int idx=0, dana=0;
      decimal dekurziv_stopa, kamata_ukupno=0.00M, kamata_this=0.00M;
      bool kumuliranjeOsnoviceObavljeno = false;
      bool isKartica = !isKamate;

      dateOD = dateOD.Date;
      dateDO = dateDO.Date;

      dateCurr = dateOD;

      idx = ZXC.set_kamtbl_idx(dateOD);

      if(idx < 0 && dateOD != DateTime.MinValue) { ZXC.aim_emsg("IDX je <{0}>!!!\n", idx); return (0.00M); }

      while(dateCurr <= dateDO) 
      {
       //if(idx < 12                             - 1 && ZXC.kamtbl_rec.kt_date[idx + 1] != DateTime.MinValue) 
         if(idx < ZXC.kamtbl_rec.numOfDateBreaks - 1 && ZXC.kamtbl_rec.kt_date[idx + 1] != DateTime.MinValue) 
         {
            dateBreak = ZXC.kamtbl_rec.kt_date[idx + 1];
         }
         else 
         {
            dateBreak = dateDO;
         }
         
         if(dateDO < dateBreak) 
         {
            dateBreak = dateDO;
         }

         dana = (dateBreak - dateCurr).Days;
         
         if(dana.NotZero())  
         {
            if(dateCurr  == dateOD) dana--;
            if(dateBreak == dateDO) dana++;
         }

         if(isKamate && dana.IsZero()) 
         {
            dateCurr = dateBreak;
            idx++;
            // 26.02.2013: 
            //if(dateCurr == dateDO) break;
            //else                   continue;
         }

         dekurziv_stopa = ZXC.CalcDekurzivStopa(idx, dana, true/*racun_rec.T_isPravna*/);
         if(osnovica.IsZeroOrPositive()) // ako smo u preplati, ne racunaj dalje kamatu 
         {
            kamata_this = osnovica * dekurziv_stopa;
         }

#region Create KamateReportLine AndAddItTo reportLines

         reportLine = new KamateReportLine();

         reportLine.CustCode    = racun_rec.T_ticker   ;
         reportLine.TTkind      = TTkind               ;
         reportLine.BrojRn      = racun_rec.T_tipBr    ;
         reportLine.ValutaRn    = racun_rec.T_valuta   ;
         reportLine.PlacenoRn   = 0.00M                ;
         reportLine.IznosRn     =                      
         reportLine.TheDugRn    = racun_rec.OtsOtvor   ;
         reportLine.DateRn      = racun_rec.T_dokDate  ;

         reportLine.KamDateOD   = dateCurr             ;
         reportLine.KamDateDO   = dateBreak            ;
         reportLine.KamDana     = dana                 ;
         reportLine.KamOsnovica = osnovica             ;
         reportLine.KamStopa    = /*racun_rec.T_isPravna ==*/ true ?  ZXC.kamtbl_rec.kt_stopaPra[idx]: ZXC.kamtbl_rec.kt_stopaFiz[idx];
         reportLine.KamIznos    = kamata_this          ;

         _kumKamataRn += kamata_this;

         if(kumuliranjeOsnoviceObavljeno == false)
         {
            kumuliranjeOsnoviceObavljeno = true;

            _kumOsnovRn += osnovica;
         }

         reportLine.KumKamataRn = _kumKamataRn;
         reportLine.KumOsnovRn  = _kumOsnovRn;

         reportLines.Add(reportLine);

         //fprintf(device,"%c %-23.23s%c%2.2s.%2.2s.%2.2s%c%2.2s.%2.2s.%2.2s%c%4d %c%11.2lf %c%2.0lf%%%c% 10.2lf %c\n", 
         //    fr3[frame], racunBr,
         //    fr3[frame], dateCurr, dateCurr+2, dateCurr+6,
         //    fr3[frame], better,   better  +2, better  +6, 
         //    fr3[frame], dana,
         //    fr3[frame], osnovica, 
         //    fr3[frame], kamtbl_rec.kt_stopaFiz[idx], 
         //   // fr3[frame], konf_stopa, 
         //    fr3[frame], kamata_this, fr3[frame]);

         // (*linesPrinted)++;

#endregion Create KamateReportLine

       //if(                                             ZXC.kamtbl_rec.kt_date[idx + 1] != DateTime.MinValue) idx++;
         if((idx < ZXC.kamtbl_rec.kt_date.Length - 1) && ZXC.kamtbl_rec.kt_date[idx + 1] != DateTime.MinValue) idx++;

         kamata_ukupno += /*ron2*/(kamata_this);

         dateCurr = dateBreak;

         if(dateCurr == dateDO) break;

      } //while(dateCurr <= dateDO) 

      return kamata_ukupno;
   }

   public static bool IsFilterWellFormed(RiskReportUC reportUC)
   {
      bool OK = true;
      VvRpt_RiSk_Filter filter = reportUC.TheRptFilter;

      if(filter.KD_sifra.IsZero())
      {
         ZXC.aim_emsg("Molim, zadajte PARTNERA.");
         return false;
      }

      return (OK);
   }

}

public class RptR_KarticaKupca : RptR_Kamate_Kartica  
{
   public RptR_KarticaKupca(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, bool _isKupci): base(_reportDocument, _reportName, _rptFilter)
   {
      this.IsOtsKupaca = _isKupci;
      this.IsKamate    = false;
   }
}

public class RptR_KarticaDobav : RptR_Kamate_Kartica  
{
   public RptR_KarticaDobav(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, bool _isKupci): base(_reportDocument, _reportName, _rptFilter)
   {
      this.IsOtsKupaca = _isKupci;
      this.IsKamate    = false;
   }
}

public class RptR_KamateKupca : RptR_Kamate_Kartica  
{
   public RptR_KamateKupca(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, bool _isKupci): base(_reportDocument, _reportName, _rptFilter)
   {
      this.IsOtsKupaca = _isKupci;
      this.IsKamate    = true    ;
   }
}

public class RptR_KamateDobav : RptR_Kamate_Kartica  
{
   public RptR_KamateDobav(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, bool _isKupci): base(_reportDocument, _reportName, _rptFilter)
   {
      this.IsOtsKupaca = _isKupci;
      this.IsKamate    = true    ;
   }
}

#endregion KAMATE i KARTICA partnera

#region The BLAGAJNA Suite

public class RptR_BLAG                : RptR_StandardRiskReport
{
#region Constructor 

   public RptR_BLAG(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter) :

      base(_reportDocument, _reportName, _rptFilter, ZXC.RIZ_FilterStyle.Faktur, 
         /* _rptNeeds_ArtWars      */            false, // ArtiklWithArtstat   
         /* _rptNeeds_ArtStat      */            false, // ArtStat        
         /* _rptNeeds_Faktur       */            false, // Faktur         
         /* _rptNeeds_Rtrans       */            false, // Rtrans         
         /* _rptNeeds_Kupdob       */            false, // Kupdob         
         /* _rptNeeds_Prjkt        */            true , // Prjkt          
         /* _rptNeeds_Rtrans4ruc   */            false, // Rtrans4ruc     
         /* _rptNeeds_Artikl       */            false) // Artikl         

   {
      //TheFilterSet = RiskFilterSetEnum.REALIZ;
   }

#endregion Constructor

   public override int FillRiskReportLists()
   {
      //base.FillRiskReportLists(); // Za Prjkt only 

      TheFakturList = new List<Faktur>();

      // Fill TheFakturList 

      // 06.09.2019: hztk oce sort na dan po unosu 
    //VvDaoBase.LoadGenericVvDataRecordList(TheDbConnection, TheFakturList, RptFilter.FilterMembers, "", /*FakturOrderBy*/"dokDate, ttSort, ttNum ", true, " * ", "");

      string blagajnaOrderBy = ZXC.RRD.Dsc_IsBlgOrderByDokNum ? "dokDate, dokNum " : "dokDate, ttSort, ttNum ";
      VvDaoBase.LoadGenericVvDataRecordList(TheDbConnection, TheFakturList, RptFilter.FilterMembers, "", /*FakturOrderBy*/ blagajnaOrderBy           , true, " * ", "");

      // 12.02.2018: 
      if(RptFilter.IsBlgInIzvVal)
      {
         TheFakturList.ForEach(fak => fak.ConvertBussinessValuesToDeviza(ZXC.DevTecDao.GetHnbTecaj(/*presentValutaEnum*/fak.DevNameAsEnum, fak.DokDate), fak.DevNameAsEnum));

         //Faktur_rec_SumaRazdoblja_BLG.ConvertBussinessValuesToDeviza(ZXC.DevTecDao.GetHnbTecaj(/*presentValutaEnum*/Faktur_rec_SumaRazdoblja_BLG.DevNameAsEnum, Faktur_rec_SumaRazdoblja_BLG.DokDate), Faktur_rec_SumaRazdoblja_BLG.DevNameAsEnum);
         //Faktur_rec_DonosPretRazd_BLG.ConvertBussinessValuesToDeviza(ZXC.DevTecDao.GetHnbTecaj(/*presentValutaEnum*/Faktur_rec_DonosPretRazd_BLG.DevNameAsEnum, Faktur_rec_DonosPretRazd_BLG.DokDate), Faktur_rec_DonosPretRazd_BLG.DevNameAsEnum);
      }

      // Fill Faktur_rec_SumaRazdoblja_BLG, fill Faktur_rec_DonosPretRazd_BLG 
      FillFakturRec_SUM();

      int distinctValutaCount = TheFakturList.Select(fak => fak.DevName).Distinct().Count();
      if(distinctValutaCount > 1) ZXC.aim_emsg(MessageBoxIcon.Error, "Ova blagajna ima promete po više od jedne valute!");

      return 0;
   }

   private void FillFakturRec_SUM()
   {
      Faktur_rec_DonosPretRazd_BLG = LoadBLAGSumaPrethodRazdob(TheDbConnection, true);

      Faktur_rec_SumaRazdoblja_BLG = new Faktur();
      Faktur_rec_SumaRazdoblja_BLG.SumValuesFromList(TheFakturList);

   }

   protected Faktur LoadBLAGSumaPrethodRazdob(XSqlConnection conn, bool needsLineCount)
   {
      Faktur fakturPrevListSUM_rec = new Faktur();

      // ako trazimo dnevnik od pocetka godine onda je prethodno razdoblje prazno / null ... (a sta ako blagakna ima pocetno stanje?) 
      //if(RptFilter.DatumOd == ZXC.projectYearFirstDay) return new Faktur();

      DateTime origDateOd = RptFilter.DatumOd; // da moze restore-ati originalni dateOd 
      DateTime origDateDo = RptFilter.DatumDo; // da moze restore-ati originalni dateDo 

      // Dakle, ako zelimo analitiku razdoblja 1.3.2011 - 31.3.2011 
      // onda DonosPretRazd_BLG         ide od 1.1.2011 - 28.2.2011 
      RptFilter.DatumDo = RptFilter.DatumOd.AddDays(-1);
      RptFilter.DatumOd = ZXC.projectYearFirstDay.AddDays(-1); // da se dobije 31.12. prosle godine 
      
      TheReportUC.AddFilterMemberz();

      // 12.02.2018: start
      //fakturPrevListSUM_rec = FakturDao.GetManyFakturSumAsOneSintFaktur(conn, RptFilter.FilterMembers, /*_isUra*/ true, needsLineCount, "");
      List<Faktur>  prevFakturList = new List<Faktur>();

      // Fill TheFakturList 
      VvDaoBase.LoadGenericVvDataRecordList(TheDbConnection, prevFakturList, RptFilter.FilterMembers, "", /*FakturOrderBy*/"dokDate, ttSort, ttNum ", true, " * ", "");

    //28.03.2018. ako se devizna blagajna prkazuje u devizi
      if(RptFilter.IsBlgInIzvVal)
      {
         prevFakturList.ForEach(fak => fak.ConvertBussinessValuesToDeviza(ZXC.DevTecDao.GetHnbTecaj(/*presentValutaEnum*/fak.DevNameAsEnum, fak.DokDate), fak.DevNameAsEnum));

         //Faktur_rec_SumaRazdoblja_BLG.ConvertBussinessValuesToDeviza(ZXC.DevTecDao.GetHnbTecaj(/*presentValutaEnum*/Faktur_rec_SumaRazdoblja_BLG.DevNameAsEnum, Faktur_rec_SumaRazdoblja_BLG.DokDate), Faktur_rec_SumaRazdoblja_BLG.DevNameAsEnum);
         //Faktur_rec_DonosPretRazd_BLG.ConvertBussinessValuesToDeviza(ZXC.DevTecDao.GetHnbTecaj(/*presentValutaEnum*/Faktur_rec_DonosPretRazd_BLG.DevNameAsEnum, Faktur_rec_DonosPretRazd_BLG.DokDate), Faktur_rec_DonosPretRazd_BLG.DevNameAsEnum);
      }

      fakturPrevListSUM_rec.SumValuesFromList(prevFakturList);
      // 12.02.2018: end

      RptFilter.DatumOd = origDateOd;
      RptFilter.DatumDo = origDateDo;

      TheReportUC.AddFilterMemberz();

      return fakturPrevListSUM_rec;

   }

}

#endregion The BLAG Suite

#region RptR_PrnManyFak

public class RptR_PrnManyFak                : RptR_StandardRiskReport
{
#region Constructor 

   /*private*/internal ushort subDsc;

   public RptR_PrnManyFak(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ushort _subDsc) :

      base(_reportDocument, _reportName, _rptFilter, ZXC.RIZ_FilterStyle.Faktur, 
         /* _rptNeeds_ArtWars      */            false, // ArtiklWithArtstat   
         /* _rptNeeds_ArtStat      */            false, // ArtStat        
         /* _rptNeeds_Faktur       */            true , // Faktur         
         /* _rptNeeds_Rtrans       */            true , // Rtrans         
         /* _rptNeeds_Kupdob       */            true , // Kupdob         
         /* _rptNeeds_Prjkt        */            true , // Prjkt          
         /* _rptNeeds_Rtrans4ruc   */            false, // Rtrans4ruc     
         /* _rptNeeds_Artikl       */            true ) // Artikl         

   {
      //TheFilterSet = RiskFilterSetEnum.REALIZ;

      this.subDsc = _subDsc;
   }

#endregion Constructor

   public PrnFakDsc ThePFD { get; set; }

   public override int FillRiskReportLists()
   {
      ThePFD = new PrnFakDsc(FakturDUC.GetDscLuiListForThisTT(RptFilter.TT, /*0*/subDsc));

      //30.11.2020.
      RptFilter.PFD = ThePFD;

      base.FillRiskReportLists();

      TheArtiklLightList = TheArtiklList.Select(artikl => new ArtiklLight(artikl)).ToList();

#region TheFakturImagesList

      TheFakturImagesList = new List<FakturImages>();

      Kupdob kupdob;

      if(RptFilter.ReportNeedsImages)
      {
         foreach(Faktur faktur in TheFakturList)
         {
            kupdob = TheKupdobList.SingleOrDefault(kpd => kpd.KupdobCD == faktur.KupdobCD);
            if(kupdob == null) kupdob = new Kupdob();

            TheFakturImagesList.Add(new FakturImages(faktur, kupdob));
         }
      }

#endregion TheFakturImagesList


      return 0;
   }

   public static bool IsFilterWellFormed(RiskReportUC reportUC)
   {
      bool OK = true;
      VvRpt_RiSk_Filter filter = reportUC.TheRptFilter;

      if(filter.TT.IsEmpty())
      {
         ZXC.aim_emsg("Molim, zadajte TT dokumenta.");
         return false;
      }

      return (OK);
   }

}

#endregion RptR_PrnManyFak

#region KPM - Knjiga Popisa u Maloprodaji

public class RptR_KnjigaPopisa  : RptR_StandardRiskReport
{
#region Constructor 

   public bool ShouldCheckForDailyBalance { get; set; }

   public RptR_KnjigaPopisa(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter) :

      base(_reportDocument, _reportName, _rptFilter, ZXC.RIZ_FilterStyle.Faktur, 
         /* _rptNeeds_ArtWars      */            false, // ArtiklWithArtstat         
         /* _rptNeeds_ArtStat      */            false, // ArtStat        
         /* _rptNeeds_Faktur       */            false, // Faktur         
         /* _rptNeeds_Rtrans       */            false, // Rtrans         
         /* _rptNeeds_Kupdob       */            false, // Kupdob         
         /* _rptNeeds_Prjkt        */            true , // Prjkt          
         /* _rptNeeds_Rtrans4ruc   */            false, // Rtrans4ruc     
         /* _rptNeeds_Artikl       */            false) // Artikl         
   {
      //TheFilterSet = RiskFilterSetEnum.REALIZ;
   }

#endregion Constructor

#region FillRiskReportLists

   public override int FillRiskReportLists()
   {
#region Init stuff

      bool is4usluga = false; // TODO: !!! da dodje iz RptFiltera 

      decimal? VMIzaduzenje;
      Faktur curr_VMI_faktur_rec;

      TheFakturList = new List<Faktur>();

#endregion Init stuff

#region Force Malop SkladCD

      if(RptFilter.SkladCD.IsEmpty())
      {
         VvLookUpItem theMalopLui = ZXC.luiListaSkladista.FirstOrDefault(lui => lui.Flag == true); // probaj naci lui sa mal flagom  

         if(theMalopLui == null || theMalopLui.Cd.IsEmpty())
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Ne mogu naći maloprodajno skladište!");
            return (-1);
         }

         RptFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.skladCD ], ZXC.FM_OR_Enum.OPEN_OR , false, "SkladCD",  theMalopLui.Cd, theMalopLui.Cd, "Za skladište:", " = ", ""));
         RptFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.skladCD2], ZXC.FM_OR_Enum.CLOSE_OR, false, "SkladCD2", theMalopLui.Cd, ""            , ""             , " = ", ""));

      }
      else
      {
         if(ZXC.luiListaSkladista.GetFlagForThisCd(RptFilter.SkladCD) == false)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "{0} Nije maloprodajno skladište!", RptFilter.SkladCD);
            return (-1);
         }
      }

#endregion Force Malop SkladCD

      FakturDao.LoadKPM_MalUlazUnionIrmGroupedFakturList(TheDbConnection, TheFakturList, RptFilter.FilterMembers);

#region IsRaisingWarning (ShouldCheckForDailyBalance)

      // 28.03.2015: 
    //ShouldCheckForDailyBalance =                                                                      TheFakturList.Any(fak => fak.TT == Faktur.TT_VMI); // should check for daily balance 
      ShouldCheckForDailyBalance = ZXC.CURR_prjkt_rec.Ticker != "QQTEXT" && ZXC.IsTEXTHOany == false && TheFakturList.Any(fak => fak.TT == Faktur.TT_VMI); // should check for daily balance 

#endregion IsRaisingWarning (ShouldCheckForDailyBalance)

#region Add Eventual Artificial Shadow Fakturs

      if(ZXC.RRD.Dsc_IsSupressSHADOWing == false && TheFakturList.Any(fak => fak.TtInfo.HasShadowTT))
      {
         Faktur shadowFaktur_rec;

         for(int i = 0; i < TheFakturList.Count; ++i)
         {
            if(TheFakturList[i].TtInfo.HasShadowTT == false) continue;

#region if(ShouldCheckForDailyBalance) then get 'VMIzaduzenje' for check (korisno samo ako ima nivelacija na taj dan)

            // 'ShouldCheckForDailyBalance' - ako koristimo VMI-jeve tada ocekujemo DailyBalance 
            if(ShouldCheckForDailyBalance)
            {
               curr_VMI_faktur_rec = TheFakturList.Take(i).SingleOrDefault(f => f.TT ==Faktur.TT_VMI && f.DokDate == TheFakturList[i].DokDate);

               if(curr_VMI_faktur_rec != null) VMIzaduzenje = curr_VMI_faktur_rec.KPM_zaduzenje;
               else                            VMIzaduzenje = null;
            }
            else
            {
               VMIzaduzenje = null;
            }

#endregion if(ShouldCheckForDailyBalance) then get 'VMIzaduzenje' for check

            shadowFaktur_rec = CreateFakturShadow(TheFakturList[i], VMIzaduzenje);

            if(shadowFaktur_rec != null) // ako je null znaci da nema potrebe za nivelacijom 
            {
               TheFakturList.Insert(i /*+ 1*/, shadowFaktur_rec); ++i;
            }
         }

         TheFakturList.RemoveAll(f => f.KupdobName.StartsWith(" NIVELACIJA")); // !!! trik da sad kad smo ih iskoristili, izbacimo NIV-ove iz 3. UNIJE 
      }

#endregion Add Eventual Artificial Shadow Fakturs

#region Naziv i broj isprave

      //foreach(var faktur in TheFakturList)
      // remarck next line and unremarck previous for non parallel*/ 
      Parallel.ForEach(TheFakturList, faktur =>
      {
         if(faktur.R_IrmRobRbtZad.IsZero())
         {
            faktur.KupdobName = faktur.KupdobName.Replace("Popusti i ", "");
         }
      }
      /* remarck this zagrada line for non parallel*/ );

#endregion Naziv i broj isprave

      var ttInUseList = TheFakturList.Select(f => f.TT).Distinct();
      TheUtilLookupList = ZXC.luiListaFakturType.Join(ttInUseList, lui => lui.Cd, tt => tt, (lui, tt) => lui).OrderBy(lui => lui.Cd).ToList();
      var pero          = ZXC.luiListaFakturType.Join(ttInUseList, lui => lui.Cd, tt => tt, (lui, tt) => tt ).OrderBy(tt => tt     ).ToList();

      return TheFakturList.Count;
   }

   private Faktur CreateFakturShadow(Faktur faktur, decimal? VMIzaduzenje)
   {
      // Do we need this, enivej?
      if(faktur.K_NivVrj.Ron2().IsZero()) return null;

      Faktur shadowFaktur_rec = new Faktur();

      //shadowRtrans_rec.TheAsEx = rtrans.TheAsEx;

      // 04.11.2016: 
    //shadowFaktur_rec.TT         = Faktur.TT_NIV;
           if(faktur.TtInfo.IsMalopFin_I) shadowFaktur_rec.TT = Faktur.TT_NIV;
      else if(faktur.TtInfo.IsMalopFin_U) shadowFaktur_rec.TT = Faktur.TT_NUV;
      else throw new Exception("CreateFakturShadow: Niti U niti I");

      shadowFaktur_rec.DokDate    = faktur.DokDate;
      shadowFaktur_rec.SkladCD    = faktur.SkladCD;

      // ... just to remember ...
      // PrevKolStanje   = (StanjeKol     - (RtrPstKol + RtrUlazKol - RtrIzlazKol)); 
      // DiffMalopCij    = (RtrCijenaMPC  - PrevMalopCij                          ); 
      // NivelacUlazVrj  = (PrevKolStanje * DiffMalopCij                          ); 
      // NivelacIzlacVrj = (RtrIzlazKol   * DiffMalopCij                          ); 

      //public decimal KPM_zaduzenje                  
      //   get                                        
      //           if(isULAZ)  return R_ukMSK;        
      //      else if(isIZLAZ) return R_IrmRobRbtZad; 

      //public decimal R_ukMSK        { get { return this.S_ukMSK_00 + this.S_ukMSK_10 + this.S_ukMSK_23 ; } }
      //public decimal R_IrmRobRbtZad { get { return -1M * (this.R_ukMSK - this.R_ukKCRP_rob); } }

      // ovdje sada treba podmetnuti vrijednosti za properti 'KPM_zaduzenje' 

      shadowFaktur_rec.S_ukMSK_00 = faktur.K_NivVrj00;
      shadowFaktur_rec.S_ukMSK_10 = faktur.K_NivVrj10;
      shadowFaktur_rec.S_ukMSK_23 = faktur.K_NivVrj23;
      shadowFaktur_rec.S_ukMSK_25 = faktur.K_NivVrj25;
      shadowFaktur_rec.S_ukMSK_05 = faktur.K_NivVrj05;

      shadowFaktur_rec.KupdobName = "___Nivelacija po " + (faktur.TtInfo.IsMalopFin_U ? faktur.TT_And_TtNum : faktur.TT/*"IRM"*/);

      // tripper solution 
      if(VMIzaduzenje != null)
      {
         decimal VMIzaduzenje2 = (decimal)(VMIzaduzenje);
         decimal discrepancy = VMIzaduzenje2 + faktur.KPM_zaduzenje - faktur.KPM_razduzenje_rob + faktur.K_NivVrj;
         if(Math.Abs(discrepancy).NotZero() && Math.Abs(discrepancy) < 0.05M)
         {
            shadowFaktur_rec.S_ukMSK_25 -= (discrepancy);
         }
      }
      return shadowFaktur_rec;
   }

#endregion FillRiskReportLists

}

#endregion KPM - Knjiga Popisa u Maloprodaji

#region PrometArtikla

public class RptR_PrometArtikla    : RptR_StandardRiskReport
{
   string ArtiklGR, FakturGR;
   public bool IsOrgPak;

   public RptR_PrometArtikla(bool _isOrgPak, string _artiklGR, string _fakturGR, ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)

      : base(_reportDocument, _reportName, _rptFilter, _filterStyle,
         _rptNeeds_ArtWars   , // ArtiklWithArtstat 
         _rptNeeds_ArtStat   , // ArtStat        
         _rptNeeds_Faktur    , // Faktur         
         _rptNeeds_Rtrans    , // Rtrans         
         _rptNeeds_Kupdob    , // Kupdob         
         _rptNeeds_Prjkt     , // Prjkt          
         _rptNeeds_Rtrans4ruc, // Rtrans4ruc     
         _rptNeeds_Artikl    ) // Artikl         

   {
      ArtiklGR = _artiklGR;
      FakturGR = _fakturGR;
      IsOrgPak = _isOrgPak;

      RptFilter.NeedsGroupTree = (RptFilter.GrupiranjeDokum == "KupdobName") ? true : false;
      //TheFilterSet = RiskFilterSetEnum.PRM_ART;
   }

   public override int FillRiskReportLists()
   {
      base.FillRiskReportLists(); // eventualno napuni TheFakturList (ako je fakturGR.NotEmpty() i IsFakGrDataInFakturBussiness(fakturGR) == true) 

      // 20.01.2015: 
      RptFilter.UtilInt = TheRtransList.Select(rtr => Faktur.Set_TT_And_TtNum(rtr.T_TT, rtr.T_ttNum)).Distinct().Count();

      // 21.01.2014: 
      TheRtransList = TheRtransList.Where(rtr => rtr.T_artiklCD.NotEmpty()).ToList();
      // 21.01.2014: 
      if(IsOrgPak)
      {
         TheRtransList = TheRtransList.Where(rtr => rtr.T_isIrmUsluga == false).ToList();

         if(ZXC.IsTEMBO) // TEMBO izbaci sve NE 'litre' PodJM 
         {
            TheRtransList.RemoveAll(rtr => rtr.A_OrgPakJM.ToUpper().StartsWith("L") == false);
         }
      }

      // 20.01.2015: 
      string firstRtransSkladCD = TheRtransList.Count.NotZero() ? TheRtransList.FirstOrDefault().T_skladCD : "";
      // 13.03.2015: !!! bio BUG u tembu!!! 
    //bool isMalopSkl =                                                 ZXC.luiListaSkladista.GetFlagForThisCd(firstRtransSkladCD);

      // 25.05.2022: u ovome izvj "PrometArtikla" isMalopSkl radi zabunu u koloni IznosPremaDokumentima 
    //bool isMalopSkl = RptFilter.FuseBool1 == false/* MPSK po NBC*/ && ZXC.luiListaSkladista.GetFlagForThisCd(firstRtransSkladCD);
      // 02.06.2022: ipak PRIVREMENO vratili jer se TH buni
    //bool isMalopSkl = false                                                                                                     ;
      bool isMalopSkl = RptFilter.FuseBool1 == false/* MPSK po NBC*/ && ZXC.luiListaSkladista.GetFlagForThisCd(firstRtransSkladCD);

      RptFilter.IsPecatPotpis = isMalopSkl; // trik da RISK rpt zna jel malopSkl

      // DELLMELATTERRRRRR!!!!!!!
#if DEBUG_SVDsamoPotrosni_23_11_2021_zaUpravnoVijece
      TheRtransList.RemoveAll(rtr => Artikl.IsSvdArtGR_Potr(rtr.A_ArtGrCd1) == false);
#endif
#if DEBUGSVDsamoLijekovi_23_11_2021_zaUpravnoVijece
      TheRtransList.RemoveAll(rtr => Artikl.IsSvdArtGR_Ljek(rtr.A_ArtGrCd1) == false);

    //TheRtransList.RemoveAll(rtr =>                        rtr.A_ArtGrCd1 == "90" );
      TheRtransList.RemoveAll(rtr =>                        rtr.A_ArtGrCd1 != "90" );
#endif

      if(ZXC.IsSvDUH)
      {
         if(reportDocument is Vektor.Reports.RIZ.CR_SVD_PrmArt4Nabava)
         {
            TheRtransList.RemoveAll(rtr => rtr.IsSvdArtGR_Ljek_ == false);
         }
         else
         {
            if(RptFilter.SVD_LiP == ZXC.PdvZPkindEnum.SVD_LJEK)
            {
               TheRtransList.RemoveAll(rtr => rtr.IsSvdArtGR_Ljek_ == false);
            }
            if(RptFilter.SVD_LiP == ZXC.PdvZPkindEnum.SVD_POTR)
            {
               TheRtransList.RemoveAll(art => art.IsSvdArtGR_Potr_ == false);
            }
         }
      }

#region FakturGR.NotEmpty()

      if(FakturGR.NotEmpty()) // Grupiramo po necemu sto je Faktur ili Rtrans podatak 
      {
         if(IsFakGrDataInFakturBussiness(FakturGR)) // Dakle, grupiramo po necemu sto se nalazi u pripadajucem FAKTUR podatku 
         {
            TheDeviznaSumaList =

               TheRtransList
               .Join(TheFakturList, Rtr => Rtr.T_parentID, Fak => Fak.RecID, (Rtr, Fak) => new { R = Rtr, FakGRdata = GetFakGRdata(FakturGR, Fak) })
               .GroupBy(theJoin => new { theJoin.FakGRdata, theJoin.R.T_artiklCD })
               .Select(grp => new VvReportSourceUtil
                  (/* DevName      */ grp.First().R.T_artiklName,
                   /* TheCD        */ grp.Key.T_artiklCD,
                   /* Count        */ grp.Count(),
                   /* Kol          */ grp.Sum(theJoin => IsOrgPak ? theJoin.R.R_kolOP : theJoin.R.R_kol),
                   /* TheMoneyKCR  */ grp.Sum(theJoin => (                                theJoin.R.R_KCR)),
                   /* TheMoney     */ grp.Sum(theJoin => (isMalopSkl ? theJoin.R.R_KCRP : theJoin.R.R_KCR)),
                   /* TheMoney2    */ grp.Sum(theJoin => theJoin.R.R_Kol_Puta_PrNabCij),
                   /* FakturGR     */ grp.First().FakGRdata,
                   /* ArtiklGrCD   */ GetArtGrCD  (ArtiklGR, grp.Key.T_artiklCD),
                   /* ArtiklGrName */ GetArtGrName(ArtiklGR, grp.Key.T_artiklCD),
                   /* TheSaldo     */ grp.Sum(gr => gr.R.R_Ira_RUV)
                  ))
               .OrderBy(R => (RptFilter.SorterType_Sifrar == VvSQL.SorterType.Code ? R.TheCD : R.DevName))
               .ToList();
         }
         else // Dakle, grupiramo po necemu sto se nalazi u samom RTRANS podatku 
         {
            TheDeviznaSumaList = 
               
               TheRtransList 
               .Select(Rtr => new { R = Rtr, FakGRdata = GetFakGRdata(FakturGR, Rtr) })
               .GroupBy(RichR => new { RichR.FakGRdata, RichR.R.T_artiklCD })
               .Select(grp => new VvReportSourceUtil
                  (/* DevName      */ grp.First().R.T_artiklName, 
                   /* TheCD        */ grp.Key.T_artiklCD, 
                   /* Count        */ grp.Count(), 
                   /* Kol          */ grp.Sum(gr => IsOrgPak ? gr.R.R_kolOP : gr.R.R_kol),
                   /* TheMoneyKCR  */ grp.Sum(gr => (                           gr.R.R_KCR)),
                   /* TheMoney     */ grp.Sum(gr => (isMalopSkl ? gr.R.R_KCRP : gr.R.R_KCR)),
                   /* TheMoney2    */ grp.Sum(gr => gr.R.R_Kol_Puta_PrNabCij),
                   /* FakturGR     */ grp.First().FakGRdata,
                   /* ArtiklGrCD   */ GetArtGrCD  (ArtiklGR, grp.Key.T_artiklCD),
                   /* ArtiklGrName */ GetArtGrName(ArtiklGR, grp.Key.T_artiklCD),
                   /* TheSaldo     */ grp.Sum(gr => gr.R.R_Ira_RUV)
            
                  ))
               .OrderBy(R => (RptFilter.SorterType_Sifrar == VvSQL.SorterType.Code ? R.TheCD : R.DevName))
               .ToList();


            // FUSE za dnevnik knjiženja 

          //TheDeviznaSumaList = 
          //   
          //   TheRtransList 
          //   .Select(Rtr => new { R = Rtr, FakGRdata = GetFakGRdata(FakturGR, Rtr) })
          // //.GroupBy(RichR => new { RichR.FakGRdata, RichR.R.T_artiklCD })
          //   .Select(rtr => new VvReportSourceUtil
          //      (/* DevName      */ rtr.R.T_artiklName, 
          //       /* TheCD        */ rtr.R.T_artiklCD, 
          //       /* Count        */ 1, 
          //       /* Kol          */ rtr.R.R_kol,
          //       /* TheMoneyKCR  */ rtr.R.R_KCR,
          //       /* TheMoney     */ rtr.R.R_KCR,
          //       /* TheMoney2    */ rtr.R.R_Kol_Puta_PrNabCij,
          //       /* FakturGR     */ rtr.FakGRdata,
          //       /* ArtiklGrCD   */ GetArtGrCD  (ArtiklGR, rtr.R.T_artiklCD),
          //       /* ArtiklGrName */ GetArtGrName(ArtiklGR, rtr.R.T_artiklCD),
          //       /* TheSaldo     */ rtr.R.R_Ira_RUV
          //
          //      ))
          //   .OrderBy(R => (RptFilter.SorterType_Sifrar == VvSQL.SorterType.Code ? R.TheCD : R.DevName))
          //   .ToList();

         }
      }

#endregion FakturGR.NotEmpty()

#region ArtiklGR.NotEmpty()

      else if(ArtiklGR.NotEmpty()) // Grupiramo po necemu sto je Artikl podatak 
      {
         InitLuiList(ArtiklGR);

         TheDeviznaSumaList = 

            TheRtransList 
            .GroupBy(R => R.T_artiklCD)
            .Select(grp => new VvReportSourceUtil
               (/* DevName      */ grp.First().T_artiklName, 
                /* TheCD        */ grp.Key, 
                /* Count        */ grp.Count(), 
                /* Kol          */ grp.Sum(R => IsOrgPak ? R.R_kolOP : R.R_kol),
                /* TheMoneyKCR  */ grp.Sum(R => (                        R.R_KCR)),
                /* TheMoney     */ grp.Sum(R => (isMalopSkl ? R.R_KCRP : R.R_KCR)),
                /* TheMoney2    */ grp.Sum(R => R.R_Kol_Puta_PrNabCij),
                /* ArtiklGrCD   */ GetArtGrCD  (ArtiklGR, grp.Key),
                /* ArtiklGrName */ GetArtGrName(ArtiklGR, grp.Key),
                /* TheSaldo     */ grp.Sum(gr => gr.R_Ira_RUV)
               ))
            .OrderBy(R => (RptFilter.SorterType_Sifrar == VvSQL.SorterType.Code ? R.TheCD : R.DevName))
            .ToList();
      }

#endregion ArtiklGR.NotEmpty()

#region NO Groupping at all

      else // Dakle NEMA grupiranja uopce 
      {

         if(RptFilter.IsFor_SVD_CheckUgovor == false)// 19.08.2022: dodan if, false je classic 
         { 
            TheDeviznaSumaList = 

            TheRtransList 
            .GroupBy(R => R.T_artiklCD)
            .Select(grp => new VvReportSourceUtil
               (/* DevName   */ grp.First().T_artiklName, 
                /* TheCD     */ grp.Key, 
                /* Count     */ grp.Count(), 
                /* Kol       */ grp.Sum(R => IsOrgPak ? R.R_kolOP : R.R_kol),
                /*TheMoneyKCR*/ grp.Sum(R => (                        R.R_KCR)), 
                /* TheMoney  */ grp.Sum(R => (isMalopSkl ? R.R_KCRP : R.R_KCR)), 
                /* TheMoney2 */ grp.Sum(R => R.R_Kol_Puta_PrNabCij),
                /* TheSaldo  */ grp.Sum(R => R.R_Ira_RUV)

#if BiloNekad_SVDsamoLijekovi_23_11_2021_zaUpravnoVijece                
                ,
                /* IsNekakav */ grp.Any(R => R.T_skladCD == "50") //_SVDsamoLijekovi_23_11_2021_zaUpravnoVijece
#endif
               ))
            .OrderBy(R => (RptFilter.SorterType_Sifrar == VvSQL.SorterType.Code ? R.TheCD : R.DevName))
            .ToList();

         } // if NOT IsFor_SVD_CheckUgovor - classic 

         #region isFor_SVD_CheckUgovor

         if(RptFilter.IsFor_SVD_CheckUgovor == true)
         {

            #region Get interni & externi broj Ugovora

            Faktur UGOfaktur_rec = new Faktur();
            Rtrans UGOrtrans_rec = new Rtrans();

            bool UGOrtransFound;

            foreach(Rtrans URArtrans_rec in TheRtransList)
            { 
               UGOrtransFound = FakturDao.SetMeLastUGOrtransForURArtrans(TheDbConnection, UGOrtrans_rec, URArtrans_rec);

               if(UGOrtransFound)
               {
                  URArtrans_rec.R_utilUint = UGOrtrans_rec.T_ttNum;

                  bool UGOfakturFound = FakturDao.SetMeFaktur(TheDbConnection, UGOfaktur_rec, Faktur.TT_UGO, UGOrtrans_rec.T_ttNum, false);
                  if(UGOfakturFound)
                  {
                     string YYaddition = "";
                     string UGO_YYYY   = ZXC.ValOrEmpty_YyyyDateTime_AsText(UGOfaktur_rec.DokDate);

                     if(UGOfaktur_rec.VezniDok.TrimEnd(' ').EndsWith(UGO_YYYY) == false)
                     {
                        YYaddition = " / " + UGO_YYYY;
                     }
                     URArtrans_rec.R_utilString = UGOfaktur_rec.VezniDok + YYaddition;
                  }
               }

            } // foreach 

            #endregion Get interni & externi broj Ugovora

            #region Set TheDeviznaSumaList from TheRtransList

            TheDeviznaSumaList = 

            TheRtransList 
            .GroupBy(R => R.T_artiklCD + R.T_kupdobCD + R.R_utilUint) // !!! grupiranje po Ugovoru 
            .Select(grp => new VvReportSourceUtil
                  (/* DevName   */ grp.First().T_artiklName,
                   /* TheCD     */ /*grp.Key*/ grp.First().T_artiklCD, 
                   /* Count     */ grp.Count(), 
                   /* Kol       */ grp.Sum(R => IsOrgPak ? R.R_kolOP : R.R_kol),
                   /*TheMoneyKCR*/ grp.Sum(R => (                        R.R_KCR )), 
                 ///* TheMoney  */ grp.Sum(R => (isMalopSkl ? R.R_KCRP : R.R_KCR )), !!!!!
                   /* TheMoney  */ grp.Sum(R => (                        R.R_KCRM)), // nadamo se da za potrebe IsFor_SVD_CheckUgovor u R_KCRM imamo bez pdv-a kako hzzo trazi 
                   /* TheMoney2 */ grp.Sum(R => R.R_Kol_Puta_PrNabCij),
                   /* TheSaldo  */ grp.Sum(R => R.R_Ira_RUV)
                  )

                  // additional member initializations, mimo constructora: 

                  { KupdobCD = grp.First().T_kupdobCD, UtilUint = grp.First().R_utilUint, String3 = grp.First().R_utilString }

               ).OrderBy(R => (RptFilter.SorterType_Sifrar == VvSQL.SorterType.Code ? R.TheCD : R.DevName)).ToList();

            #endregion Set TheDeviznaSumaList from TheRtransList

            #region Set ATK, kupdobName

            Artikl artikl_rec;
            Kupdob kupdob_rec;

            foreach(VvReportSourceUtil rsuLine in TheDeviznaSumaList)
            {
               artikl_rec = TheArtiklList.SingleOrDefault(a => a.ArtiklCD == rsuLine.TheCD);

               if(artikl_rec != null) rsuLine.String1 = artikl_rec.ArtiklCD2;

             //kupdob_rec = TheKupdobList.SingleOrDefault(k => k.KupdobCD == rsuLine.KupdobCD);
               kupdob_rec = TheReportUC.Get_Kupdob_FromVvUcSifrar(rsuLine.KupdobCD);

               if(kupdob_rec != null) rsuLine.String2 = kupdob_rec.Naziv;

            }

            #endregion Set ATK, kupdobName

         }

         #endregion isFor_SVD_CheckUgovor

      }

      #endregion NO Groupping at all

#if DEBUG//_SVDsamoLijekovi_23_11_2021_zaUpravnoVijece

      // upali ako ikad opet zatreba. a sta? nem pojma
      //Artikl artikl_rec;
      //foreach(VvReportSourceUtil rsuLine in TheDeviznaSumaList)
      //{
      //   artikl_rec = TheReportUC.Get_Artikl_FromVvUcSifrar(rsuLine.TheCD);
      //   if(artikl_rec != null) rsuLine.KupdobName = artikl_rec.ArtiklName2;
      //   else                   rsuLine.KupdobName = ""                    ;
      //}


#endif

      #region SVD_PrmArt4Nabava

      if(reportDocument is Vektor.Reports.RIZ.CR_SVD_PrmArt4Nabava)
      {
         Artikl artikl_rec;
         Rtrans lastURArtrans_rec;
         Kupdob kupdobSifrar_rec;

         bool lastURArtransFound;

         decimal thePdvSt = 0M;

         List<Halmed_SVD.HALMEDartikl> halmedArtiklList = VvDaoBase.Get_HALMEDartikl_List(TheDbConnection, "");

         Halmed_SVD.HALMEDartikl halmedArtikl;

         TheReportUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.None);

         foreach(VvReportSourceUtil rsuLine in TheDeviznaSumaList)
         {
            artikl_rec = TheReportUC.Get_Artikl_FromVvUcSifrar(rsuLine.TheCD);

            if(artikl_rec != null)
            {
               lastURArtrans_rec = new Rtrans();

               lastURArtransFound = FakturDao.SetMeLastRtransForArtiklAndTT(TheDbConnection, lastURArtrans_rec, Faktur.TT_URA, rsuLine.TheCD, /*false*/true);

               if(lastURArtransFound) thePdvSt = lastURArtrans_rec.T_pdvSt;
               else                   thePdvSt = ZXC.ValOrZero_Decimal(artikl_rec.PdvKat, 0);

               rsuLine.KupdobName = artikl_rec.ArtiklCD2  ; // ATK           
               rsuLine.String1    = artikl_rec.ArtiklName2; // generika      
               rsuLine.String2    = artikl_rec.JedMj      ; // jedinica mjere

               kupdobSifrar_rec = VvUserControl.KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD == artikl_rec.DobavCD);
               if(kupdobSifrar_rec != null && artikl_rec.DobavCD.NotZero())
               {
                  rsuLine.String4 = kupdobSifrar_rec.Naziv;
               }

               halmedArtikl = halmedArtiklList.SingleOrDefault(ha => ha.s_lio == artikl_rec.AtestBr);

               if(halmedArtikl.s_lio.NotEmpty())
               {
                  rsuLine.String3 /*halmedORG*/ = halmedArtikl.obl_ozn + ", " + halmedArtikl.doza + " " + halmedArtikl.mj_ozn;
                  rsuLine.ArtiklGrName          = halmedArtikl.par_naziv;
                  rsuLine.ArtiklGrCD            = halmedArtikl.naziv;
               } // if(halmedArtikl.s_lio.NotEmpty())

            } // if(artikl_rec != null)

            rsuLine.TheMoney2 = ZXC.VvGet_100_from_125(rsuLine.TheMoney, thePdvSt); // iznos bez pdv, dočim je TheMoney iznos sa pdv a Average1i2 su odgovarajuce cijene 
            rsuLine.TheKoef   = thePdvSt;
         }
      }

      #endregion SVD_PrmArt4Nabava

      return TheDeviznaSumaList.Count;
   }

   private void InitLuiList(string artiklGRtype)
   {
      switch(artiklGRtype)
      {
         case "TS"      : ZXC.luiListaArtiklTS     .LazyLoad(); break;
         case "Grupa1CD": ZXC.luiListaGrupa1Artikla.LazyLoad(); break;
         case "Grupa2CD": ZXC.luiListaGrupa2Artikla.LazyLoad(); break;
         case "Grupa3CD": ZXC.luiListaGrupa3Artikla.LazyLoad(); break;
      }
   }

   internal static bool IsFakGrDataInFakturBussiness(string fakturGRtype)
   {
      if(fakturGRtype == "AddUID"     ||
         fakturGRtype == "AddTS"      ||
         fakturGRtype == "HourOfDay"  ||
         fakturGRtype == "DevName"    ||
         fakturGRtype == "PosJedName" ||
         fakturGRtype == "ProjektCD"  ||
         fakturGRtype == "Putnik"     ||
         fakturGRtype == "NacPlac"     ) return true ;
      else                               return false;
   }

   public static bool IsFilterWellFormed(RiskReportUC reportUC)
   {
      bool OK = true;
      VvRpt_RiSk_Filter filter = reportUC.TheRptFilter;

      if(filter.TT.IsEmpty() && filter.IsPrihodTT == false)
      {
         ZXC.aim_emsg("Molim, zadajte Tip Transakcije ili oznacite 'Prihod: IFA, IRA, IRM, IOD, IPV'.");
         return false;
      }

      return (OK);
   }

}

public class RptR_ProdajaPoDobav   : /*RptR_PrometArtikla*/RptR_StandardRiskReport
{
#region Constructor

   string ArtiklGR, FakturGR;

   bool isForRtrans;

   public RptR_ProdajaPoDobav(bool _isForRtrans, string _artiklGR, string _fakturGR, ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)

      : base(/*_artiklGR, _fakturGR,*/ _reportDocument, _reportName, _rptFilter, _filterStyle,
         _rptNeeds_ArtWars   , // ArtiklWithArtstat 
         _rptNeeds_ArtStat   , // ArtStat        
         _rptNeeds_Faktur    , // Faktur         
         _rptNeeds_Rtrans    , // Rtrans         
         _rptNeeds_Kupdob    , // Kupdob         
         _rptNeeds_Prjkt     , // Prjkt          
         _rptNeeds_Rtrans4ruc, // Rtrans4ruc     
         _rptNeeds_Artikl    ) // Artikl         

   {
      this.isForRtrans = _isForRtrans;
   }

#endregion Constructor

   public override int FillRiskReportLists()
   {
      base.FillRiskReportLists();

      if(VvUserControl.KupdobSifrar == null) ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      var prodRtransList = TheRtransList.Where(rtr => rtr.T_artiklCD.NotEmpty()).ToList();

      // Voila!!! 
      prodRtransList = RtransDao.PairDobavByFIFO(TheDbConnection, prodRtransList, RptFilter.SkladCD, false);

      if(isForRtrans) // byRtrans: Vektor.Reports.RIZ.CR_ProdajaPoDobavljacu_B(); 
      {

         prodRtransList = TheFakturList
            .Join(prodRtransList, fak => fak.RecID, rtr => rtr.T_parentID, (fak, rtr) => new Rtrans() 
            {
               currentData = rtr.currentData,
               _rtrResults = rtr._rtrResults,
               TmpDecimal  = rtr.TmpDecimal,
               R_grName    = fak.NacPlac,
            }).ToList();

         //prodRtransList.ForEach(rtr => rtr.CalcTransResults(null));

         TheDeviznaSumaList =

            prodRtransList
          //.Select(Rtr => new { R = Rtr, FakGRdata = Rtr.T_twinID.ToString("000000")/*GetFakGRdata(FakturGR, Rtr)*/ })
          //.GroupBy(R => new { R.T_twinID, R.T_artiklCD }) // u R.T_twinID se krije t_kupdobCD Dobavljača 
            .Select(rtrans => new VvReportSourceUtil
               (/* DevName      */ rtrans.T_artiklName,
                /* TheCD        */ rtrans.T_artiklCD,
                /* Count        */ 0,
                /* Kol          */ rtrans.R_kol,
                /* TheMoney     */ rtrans.R_KCR,
                /* TheMoney2    */ rtrans.TmpDecimal,
                /* FakturGR     */ rtrans./*T_twinID.ToString("000000")*/R_grName,
                /* KupdobCD     */ rtrans.T_twinID                   ,
                /* KupdobName   */ rtrans.T_twinID.IsZero() ? "NEMA DOBAVLJAČA" : VvUserControl.KupdobSifrar.SingleOrDefault(k => k.KupdobCD == rtrans.T_twinID).Naziv,
                /* ArtiklGrCD   */ rtrans.T_TT,
                /* ArtiklGrName */ rtrans.T_ttNum.ToString(),
                /* TheDate      */ rtrans.T_skladDate
               ))
            .OrderBy(R => (RptFilter.SorterType_Sifrar == VvSQL.SorterType.Code ? R.TheCD : R.DevName))
            .ToList();
      }
      else // byArtikl: Vektor.Reports.RIZ.CR_ProdajaPoDobavljacu(); 
      {
         TheDeviznaSumaList =

            prodRtransList
          //.Select(Rtr => new { R = Rtr, FakGRdata = Rtr.T_twinID.ToString("000000")/*GetFakGRdata(FakturGR, Rtr)*/ })
            .GroupBy(R => new { R.T_twinID, R.T_artiklCD }) // u R.T_twinID se krije t_kupdobCD Dobavljača 
            .Select(grp => new VvReportSourceUtil
               (/* DevName      */ grp.First().T_artiklName,
                /* TheCD        */ grp.Key    .T_artiklCD,
                /* Count        */ grp.Count(),
                /* Kol          */ grp.Sum(gr => gr.R_kol),
                /* TheMoney     */ grp.Sum(gr => gr.R_KCR),
                /* TheMoney2    */ grp.Sum(gr => gr./*R_Kol_Puta_PrNabCij*/TmpDecimal),
                /* FakturGR     */ grp.First().T_twinID.ToString("000000"),
                /* KupdobCD     */ grp.First().T_twinID                   ,
                /* KupdobName   */ grp.First().T_twinID.IsZero() ? "NEMA DOBAVLJAČA" : VvUserControl.KupdobSifrar.SingleOrDefault(k => k.KupdobCD == grp.First().T_twinID).Naziv,
                /* ArtiklGrCD   */ GetArtGrCD  (ArtiklGR, grp.Key.T_artiklCD),
                /* ArtiklGrName */ GetArtGrName(ArtiklGR, grp.Key.T_artiklCD),
                /* TheDate      */ DateTime.MinValue
               ))
            .OrderBy(R => (RptFilter.SorterType_Sifrar == VvSQL.SorterType.Code ? R.TheCD : R.DevName))
            .ToList();
      }

      if(RptFilter.KD_sifra.NotZero())
      {
         TheDeviznaSumaList.RemoveAll(line => line.KupdobCD != RptFilter.KD_sifra);
      }

      return TheDeviznaSumaList.Count;
   }
}

public class RptR_StanjePoDobav    : /*RptR_PrometArtikla*/RptR_StandardRiskReport
{
#region Constructor

   string ArtiklGR, FakturGR;

   public RptR_StanjePoDobav(string _artiklGR, string _fakturGR, ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)

      : base(/*_artiklGR, _fakturGR,*/ _reportDocument, _reportName, _rptFilter, _filterStyle,
         _rptNeeds_ArtWars   , // ArtiklWithArtstat 
         _rptNeeds_ArtStat   , // ArtStat        
         _rptNeeds_Faktur    , // Faktur         
         _rptNeeds_Rtrans    , // Rtrans         
         _rptNeeds_Kupdob    , // Kupdob         
         _rptNeeds_Prjkt     , // Prjkt          
         _rptNeeds_Rtrans4ruc, // Rtrans4ruc     
         _rptNeeds_Artikl    ) // Artikl         

   {
   }

#endregion Constructor

   public override int FillRiskReportLists()
   {
      base.FillRiskReportLists(); // Faktur, Kupdob 

      if(VvUserControl.KupdobSifrar == null) ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      List<Rtrans> ulazRtransList  = TheRtransList.Where(rtr => rtr.T_artiklCD.NotEmpty() && ZXC.TtUlazKolArray .Contains(rtr.T_TT)).ToList();
      List<Rtrans> izlazRtransList = TheRtransList.Where(rtr => rtr.T_artiklCD.NotEmpty() && ZXC.TtIzlazKolArray.Contains(rtr.T_TT)).ToList();

      izlazRtransList = RtransDao.PairDobavByFIFO(TheDbConnection, izlazRtransList, RptFilter.SkladCD, true);

      TheRtransList = ulazRtransList.Concat(izlazRtransList).ToList();
      
      TheDeviznaSumaList =

         TheRtransList
         //.Select(Rtr => new { R = Rtr, FakGRdata = Rtr.T_twinID.ToString("000000")/*GetFakGRdata(FakturGR, Rtr)*/ })
         .GroupBy(R => new { R.T_kupdobCD, R.T_artiklCD }) 
         .Select(grp => new VvReportSourceUtil
            (/* DevName      */ grp.First().T_artiklName,
             /* TheCD        */ grp.Key    .T_artiklCD,
             /* Count        */ grp.Count(),
             /* TheSaldo     */ grp.Sum(grRtr => grRtr.TtInfo.IsFinKol_U ?  grRtr.R_kol :           0),                         // UlazKol  
             /* TheKoef      */ grp.Sum(grRtr => grRtr.TtInfo.IsFinKol_I ?  grRtr.R_kol :           0),                         // IzlazKol 
             /* Kol          */ grp.Sum(grRtr => grRtr.TtInfo.IsFinKol_I ? -grRtr.R_kol : grRtr.R_kol),                         // KolSt 
             /* TheMoney     */ grp.Last().A_PrNabCij,                                                                          // PrNabCij 
             /* TheMoney2    */ grp.Sum(grRtr => grRtr.TtInfo.IsFinKol_I ? -grRtr.R_kol : grRtr.R_kol) * grp.Last().A_PrNabCij, // FinSaldo 
             /* FakturGR     */ grp.Key.T_kupdobCD.ToString("000000"),
             /* KupdobCD     */ grp.Key.T_kupdobCD                   ,
             /* KupdobName   */ grp.Key.T_kupdobCD.IsZero() ? "NEMA DOBAVLJAČA" : VvUserControl.KupdobSifrar.SingleOrDefault(k => k.KupdobCD == grp.Key.T_kupdobCD).Naziv,
             /* ArtiklGrCD   */ GetArtGrCD  (ArtiklGR, grp.Key.T_artiklCD),
             /* ArtiklGrName */ GetArtGrName(ArtiklGR, grp.Key.T_artiklCD)
            ))
         .OrderBy(R => (RptFilter.SorterType_Sifrar == VvSQL.SorterType.Code ? R.TheCD : R.DevName))
         .ToList();

      if(RptFilter.FuseBool4)
      {
         TheDeviznaSumaList.RemoveAll(line => line.Kol.IsZero());
      }

      if(RptFilter.FuseBool2)
      {
         TheDeviznaSumaList.RemoveAll(line => line.TheKoef.IsZero());
      }

      if(RptFilter.KD_sifra.NotZero())
      {
         TheDeviznaSumaList.RemoveAll(line => line.KupdobCD != RptFilter.KD_sifra);
      }

      return TheDeviznaSumaList.Count;
   }
}

#endregion PrometArtikla

#region PPMV - Poseban porez na motorna vozila

public class RptR_PPMV_Prilog9 : RptR_StandardRiskReport
{
   public RptR_PPMV_Prilog9(string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl)

      : base(new Vektor.Reports.RIZ.CR_Ppmv_Prilog9() as ReportDocument,
         _reportName         ,                                                    
         _rptFilter          , 
         _filterStyle        ,
         _rptNeeds_ArtWars   , // ArtiklWithArtstat 
         _rptNeeds_ArtStat   , // ArtStat        
         _rptNeeds_Faktur    , // Faktur         
         _rptNeeds_Rtrans    , // Rtrans         
         _rptNeeds_Kupdob    , // Kupdob         
         _rptNeeds_Prjkt     , // Prjkt          
         _rptNeeds_Rtrans4ruc, // Rtrans4ruc     
         _rptNeeds_Artikl    ) // Artikl         

   {

   }

   public override int FillRiskReportLists()
   {

      TheArtiklList = new List<Artikl>(); 
      if(VvUserControl.ArtiklSifrar == null) ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

      RptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.s_ukPpmvOsn], "ppmvOsn" ,      0.00M, " != "      ));
      // 11.09.2015: 
    //RptFilter.FilterMembers.Add(new VvSqlFilterMember(FakSch  [FakCI  .napomena   ], "napomena", @"%STORN%", " NOT LIKE "));

      base.FillRiskReportLists();

      TheArtiklList = VvUserControl.ArtiklSifrar.Join(TheFakturList, art => art.ArtiklCD, fak => fak.PrjArtCD, (a, x) => a).Distinct().ToList();

      return 0;
   }

}

#endregion PPMV - Poseban porez na motorna vozila

#region EU PdvGEOkind

public class RptR_EU_PdvGEOkind : RptR_StandardRiskReport
{
   public RptR_EU_PdvGEOkind(ReportDocument _reportDocument, string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle, bool _rptNeeds_ArtWars, bool _rptNeeds_ArtStat, bool _rptNeeds_Faktur, bool _rptNeeds_Rtrans, bool _rptNeeds_Kupdob, bool _rptNeeds_Prjkt, bool _rptNeeds_Rtrans4ruc, bool _rptNeeds_Artikl, bool isZP)

      : base(_reportDocument     ,
             _reportName         ,                                                    
             _rptFilter          , 
             _filterStyle        ,
             _rptNeeds_ArtWars   , // ArtiklWithArtstat 
             _rptNeeds_ArtStat   , // ArtStat        
             _rptNeeds_Faktur    , // Faktur         
             _rptNeeds_Rtrans    , // Rtrans         
             _rptNeeds_Kupdob    , // Kupdob         
             _rptNeeds_Prjkt     , // Prjkt          
             _rptNeeds_Rtrans4ruc, // Rtrans4ruc     
             _rptNeeds_Artikl    ) // Artikl         

   {
      IsForExport = true;

      this.IsZP = isZP;
   }

   public bool IsZP;

   public override int FillRiskReportLists()
   {
      RptFilter.FilterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.pdvGEOkind], "pdvGEOkind", ZXC.PdvGEOkindEnum.EU, " = "));

      base.FillRiskReportLists();

      return 0;
   }

#region XML Export

   // ovdje nisi nasao nikakve upute kako bi se trebao zvati fajl pa ga imenujes proizvoljno 
   public override string ExportFileName
   {
      get
      {
         string mmyyyy = RptFilter.DatumOd.Month.ToString("00") + RptFilter.DatumOd.Year.ToString("0000");
         string   yyyy =                                          RptFilter.DatumOd.Year.ToString("0000");

         if(IsZP) return "PDV-ZP_" + ZXC.CURR_prjkt_rec.Oib + "_" + mmyyyy + ".xml";
         else     return "PDV-S_"  + ZXC.CURR_prjkt_rec.Oib + "_" + mmyyyy + ".xml";
      }
   }

#region Xml Schema Validation

   public override bool ExecuteExportValidation(string fileName)
   {
      List<ZXC.VvXmlValidationData> valDataList = new List<ZXC.VvXmlValidationData>();
      
      if(IsZP) // 06.02.2013: op.a. PDV-K Schema zaostaje za 1 za PDV schemom 
      {
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacZP/v1-0", @"XSD\ObrazacZP-v1-0.xsd"          ));
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacZP/v1-0", @"XSD\ObrazacZPtipovi-v1-0.xsd"    ));
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"        , @"XSD\ObrazacZPmetapodaci-v1-0.xsd"));
      }
      else // PDV-S 
      {
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVS/v1-0", @"XSD\ObrazacPDVS-v1-0.xsd"          ));
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVS/v1-0", @"XSD\ObrazacPDVStipovi-v1-0.xsd"    ));
         valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"          , @"XSD\ObrazacPDVSmetapodaci-v1-0.xsd"));
      }

      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\MetapodaciTipovi-v2-0.xsd"));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/TemeljniTipovi/v2-1"     , @"XSD\TemeljniTipovi-v2-1.xsd"  ));

      return ExecuteExportValidation_Base(valDataList);
   }

#endregion Xml Schema Validation
   
   public override bool ExecuteExport(string fileName)
   {
#region Initialize XmlWriterSettings

      //if(IsZP)
      //{
      //   if(Faktur_rec_SumaRazdoblja_IRA == null) throw new Exception("Nema se što exporitrati!");
      //}
      //else // PDV-S 
      //{
      //   if(Faktur_rec_SumaRazdoblja_URA == null) throw new Exception("Nema se što exporitrati!");
      //}

      if(TheFakturList.Where(fak => (fak.TtInfo.IsIzlazniPdvTT || fak.TtInfo.IsUlazniPdvTT) && fak.PdvGEOkind == ZXC.PdvGEOkindEnum.EU).Count().IsZero()) throw new Exception("Nema se što exporitrati!");

      string ident = "   ";

      XmlWriterSettings settings = new XmlWriterSettings();
      
      settings.Indent      = true;
      settings.IndentChars = ident;

#endregion Initialize XmlWriterSettings

      using(XmlWriter writer = XmlWriter.Create(fileName, settings))
      {
#region Init Xml Document

         writer.WriteStartDocument();

         if(IsZP)
         {
            writer.WriteStartElement("ObrazacZP", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacZP/v1-0");
            writer.WriteAttributeString("verzijaSheme",                                                         "1.0");
         }
         else // PDV-S 
         {
            writer.WriteStartElement("ObrazacPDVS", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPDVS/v1-0");
            writer.WriteAttributeString("verzijaSheme",                                                             "1.0");
         }

#endregion Init Xml Document

#region Metapodaci

            if(IsZP)
            {
               writer.WriteStartElement("Metapodaci", @"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0");
               //writer.WriteElementString("Naslov", @"http://purl.org/dc/elements/1.1/title", "Prijava poreza na dodanu vrijednost");
               writer.WriteRaw("\n");
               writer.WriteRaw(ident + ident + "<Naslov dc=\"http://purl.org/dc/elements/1.1/title\">Zbirna prijavu za isporuke dobara i usluga u druge države članice Europske unije</Naslov>\n");
               writer.WriteRaw(ident + ident + "<Autor dc=\"http://purl.org/dc/elements/1.1/creator\">" + ZXC.CURR_prjkt_rec.Ime + " " + ZXC.CURR_prjkt_rec.Prezime + "</Autor>\n");
               writer.WriteRaw(ident + ident + "<Datum dc=\"http://purl.org/dc/elements/1.1/date\">" + DateTime.Now.ToString("s") + "</Datum>\n");
               writer.WriteRaw(ident + ident + "<Format dc=\"http://purl.org/dc/elements/1.1/format\">text/xml</Format>\n");
               writer.WriteRaw(ident + ident + "<Jezik dc=\"http://purl.org/dc/elements/1.1/language\">hr-HR</Jezik>\n");
               writer.WriteRaw(ident + ident + "<Identifikator dc=\"http://purl.org/dc/elements/1.1/identifier\">" + Guid.NewGuid().ToString("D")/*.ToUpper()*/ + "</Identifikator>\n");

               writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacZP-v1-0</Uskladjenost>\n");
            }
            else // PDV-S 
            {
               writer.WriteStartElement("Metapodaci", @"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0");
               //writer.WriteElementString("Naslov", @"http://purl.org/dc/elements/1.1/title", "Prijava poreza na dodanu vrijednost");
               writer.WriteRaw("\n");
               writer.WriteRaw(ident + ident + "<Naslov dc=\"http://purl.org/dc/elements/1.1/title\">Prijava za stjecanje dobara i primljene usluge iz drugih država članica Europske unije</Naslov>\n");
               writer.WriteRaw(ident + ident + "<Autor dc=\"http://purl.org/dc/elements/1.1/creator\">" + ZXC.CURR_prjkt_rec.Ime + " " + ZXC.CURR_prjkt_rec.Prezime + "</Autor>\n");
               writer.WriteRaw(ident + ident + "<Datum dc=\"http://purl.org/dc/elements/1.1/date\">" + DateTime.Now.ToString("s") + "</Datum>\n");
               writer.WriteRaw(ident + ident + "<Format dc=\"http://purl.org/dc/elements/1.1/format\">text/xml</Format>\n");
               writer.WriteRaw(ident + ident + "<Jezik dc=\"http://purl.org/dc/elements/1.1/language\">hr-HR</Jezik>\n");
               writer.WriteRaw(ident + ident + "<Identifikator dc=\"http://purl.org/dc/elements/1.1/identifier\">" + Guid.NewGuid().ToString("D")/*.ToUpper()*/ + "</Identifikator>\n");

               writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPDVS-v1-0</Uskladjenost>\n");
            }

            writer.WriteRaw(ident + ident + "<Tip dc=\"http://purl.org/dc/elements/1.1/type\">Elektronički obrazac</Tip>\n");
            writer.WriteRaw(ident + ident + "<Adresant>Ministarstvo Financija, Porezna uprava, Zagreb</Adresant>\n");
            writer.WriteRaw(ident);
         writer.WriteEndElement(); // Metapodaci 

#endregion Write Header Data

#region Zaglavlje

         //if(IsZP)
         //{
         writer.WriteStartElement("Zaglavlje");

            writer.WriteStartElement("Razdoblje");
               writer.WriteElementString("DatumOd", RptFilter.DatumOd.ToString("s").Substring(0, 10));
               writer.WriteElementString("DatumDo", RptFilter.DatumDo.ToString("s").Substring(0, 10));
            writer.WriteEndElement(); // Razdoblje 

            writer.WriteStartElement("Obveznik");
               writer.WriteElementString("Naziv", ZXC.CURR_prjkt_rec.Naziv);
               writer.WriteElementString("OIB", ZXC.CURR_prjkt_rec.Oib);
               writer.WriteStartElement("Adresa");
                  writer.WriteElementString("Mjesto", ZXC.CURR_prjkt_rec.Grad);
                  writer.WriteElementString("Ulica", ZXC.CURR_prjkt_rec.UlicaBezBroja_1);
                  writer.WriteElementString("Broj", ZXC.CURR_prjkt_rec.UlicniBroj_1);
               writer.WriteEndElement(); // Adresa 
            writer.WriteEndElement(); // Obveznik 

            writer.WriteStartElement("ObracunSastavio");
               writer.WriteElementString("Ime"    , ZXC.CURR_prjkt_rec.Ime);
               writer.WriteElementString("Prezime", ZXC.CURR_prjkt_rec.Prezime);
               writer.WriteElementString("Telefon", /*ZXC.CURR_prjkt_rec.Tel1 zbog validacije:*/ZXC.CURR_prjkt_rec.Tel2);
               writer.WriteElementString("Email"  , ZXC.CURR_prjkt_rec.Email);
            writer.WriteEndElement(); // ObracunSastavio 
            
         writer.WriteElementString("Ispostava", ZXC.CURR_prjkt_rec.OpcCd);

         writer.WriteEndElement(); // Zaglavlje 
         //}
         //else // PDV-S 
         //{
         //}

#endregion Zaglavlje

#region Tijelo

         IEnumerable<Faktur> pdvGeoFakturList;

         if(IsZP) pdvGeoFakturList = TheFakturList.Where(fak => fak.TtInfo.IsIzlazniPdvTT && fak.PdvGEOkind == ZXC.PdvGEOkindEnum.EU);
         else     pdvGeoFakturList = TheFakturList.Where(fak => fak.TtInfo.IsUlazniPdvTT  && fak.PdvGEOkind == ZXC.PdvGEOkindEnum.EU);

         var faktursGroupedByVatNo = pdvGeoFakturList.GroupBy(fak => fak.VATnumber).Select(fak => new
         {
          //rptLinePDVID = fak.Key,
            rptLinePDVID = fak.First().KdOib,
            rptLineCCode = fak.First().VatCntryCode,

            rptLineMnyI1_ZP = fak.Sum(f => f.S_ukOsnZP_11).Ron2(),
            rptLineMnyI2_ZP = fak.Sum(f => f.S_ukOsnZP_12).Ron2(),
            rptLineMnyI3_ZP = fak.Sum(f => f.S_ukOsnZP_13).Ron2(),
            rptLineMnyI4_ZP = fak.Sum(f => f.S_ukOsn10   ).Ron2(),

            rptLineMnyI1_S  = fak.Sum(f => f.S_ukOsnR25m_EU + f.S_ukOsnR25n_EU + f.S_ukOsnR10m_EU + f.S_ukOsnR10n_EU + f.S_ukOsnR05m_EU + f.S_ukOsnR05n_EU).Ron2(),
            rptLineMnyI2_S  = fak.Sum(f => f.S_ukOsnU25m_EU + f.S_ukOsnU25n_EU + f.S_ukOsnU10m_EU + f.S_ukOsnU10n_EU + f.S_ukOsnU05m_EU + f.S_ukOsnU05n_EU).Ron2(),

         });

         writer.WriteStartElement("Tijelo");
            writer.WriteStartElement("Isporuke");

         int rbr = 0;
         foreach(var rptLine in faktursGroupedByVatNo)
         {
            writer.WriteStartElement("Isporuka");
               writer.WriteElementString("RedBr",     (++rbr).ToString());
               writer.WriteElementString("KodDrzave", rptLine.rptLineCCode);
               writer.WriteElementString("PDVID"    , rptLine.rptLinePDVID);
               if(IsZP)
               {
                  writer.WriteElementString("I1", rptLine.rptLineMnyI1_ZP.ToStringVv_NoGroup_ForceDot()); 
                  writer.WriteElementString("I2", rptLine.rptLineMnyI2_ZP.ToStringVv_NoGroup_ForceDot()); 
                  writer.WriteElementString("I3", rptLine.rptLineMnyI3_ZP.ToStringVv_NoGroup_ForceDot()); 
                  writer.WriteElementString("I4", rptLine.rptLineMnyI4_ZP.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
               }
               else // PDV-S 
               {
                  writer.WriteElementString("I1", rptLine.rptLineMnyI1_S.ToStringVv_NoGroup_ForceDot()); 
                  writer.WriteElementString("I2", rptLine.rptLineMnyI2_S.ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
               }
         }

         writer.WriteEndElement(); // Isporuke 

         writer.WriteStartElement("IsporukeUkupno");
         if(IsZP)
         {
            writer.WriteElementString("I1", faktursGroupedByVatNo.Sum(gr => gr.rptLineMnyI1_ZP).ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("I2", faktursGroupedByVatNo.Sum(gr => gr.rptLineMnyI2_ZP).ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("I3", faktursGroupedByVatNo.Sum(gr => gr.rptLineMnyI3_ZP).ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("I4", faktursGroupedByVatNo.Sum(gr => gr.rptLineMnyI4_ZP).ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
         }
         else // PDV-S 
         {
            writer.WriteElementString("I1", faktursGroupedByVatNo.Sum(gr => gr.rptLineMnyI1_S) .ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("I2", faktursGroupedByVatNo.Sum(gr => gr.rptLineMnyI2_S) .ToStringVv_NoGroup_ForceDot()); writer.WriteEndElement();
         }

         writer.WriteEndElement(); // Tijelo 

#endregion Tijelo

#region Finish Xml Document

         writer.WriteEndElement();
         writer.WriteEndDocument();

#endregion Finish Xml Document

      }
      return true;
   }

#endregion ePDV XML Export

}

#endregion EU PdvGEOkind

#region PNP

public class RptR_Rekap_PNP_Rtrans : RptR_StandardRiskReport
{
   public RptR_Rekap_PNP_Rtrans(string _reportName, VvRpt_RiSk_Filter _rptFilter, ZXC.RIZ_FilterStyle _filterStyle)

      : base(new Vektor.Reports.RIZ.CR_ObrazacPP_MI_PO() as ReportDocument,
         _reportName         ,                                                    
         _rptFilter          , 
         _filterStyle        ,
         false,  // ArtiklWithArtstat         
         false,  // ArtStat        
         false,  // Faktur         
/*false*/true ,  // Rtrans         
         false,  // Kupdob         
         false,  // Prjkt          
         false,  // Rtrans4ruc     
         true )  // Artikl   

   {
      IsForExport = true;
   }

   public override int FillRiskReportLists()
   {
      base.FillRiskReportLists();

      TheRtransList = TheRtransList
         .Join(VvUserControl.ArtiklSifrar, rtr => rtr.T_artiklCD, art => art.ArtiklCD, (rtr, art) => new Rtrans()
         {
            currentData = rtr.currentData,
            _rtrResults = rtr._rtrResults,
          //TheAsEx     = rtr.TheAsEx    ,
            // 12.02.2015: ubijamo grupiranje 
          //R_grName    =                                       art.Grupa2CD    , 
            R_grName    = ZXC.projectYearFirstDay.Year < 2015 ? art.Grupa2CD: "",
            R_utilBool  = art.IsPNP      , //porez na potrosnju 
         }).
         ToList();

      TheRtransList.RemoveAll(rrtr => rrtr.R_utilBool == false);

      if(ZXC.projectYearFirstDay.Year < 2015) // 12.02.2015: ubijamo grupiranje 
      {
         // Dummies, ako nema takvoga da ipak popuni obrazac sa nulama 
         TheRtransList.Add(new Rtrans { R_grName = "VIN" });
         TheRtransList.Add(new Rtrans { R_grName = "ŽAP" });
         TheRtransList.Add(new Rtrans { R_grName = "PIV" });
         TheRtransList.Add(new Rtrans { R_grName = "BAP" });
      }

      TheDeviznaSumaList = TheRtransList
      .GroupBy(R => R.R_grName)
      .Select(grp => new VvReportSourceUtil
         (/* ArtiklGrCD   */ GetAOPoznaka(grp.Key)   ,
          /* ArtiklGrName */ GetAOPtext  (grp.Key)   ,
          /* TheMoney     */ grp.Sum(R => R.R_PnpOsn),
          /* TheKoef      */ grp.First().T_pnpSt     ,
          /* TheSaldo     */ grp.Sum(R => R.R_Pnp   )
         ))
      .OrderBy(RSU => RSU.ArtiklGrCD)
      .ToList();
      return 0;
   }

   private string GetAOPoznaka(string gr2CD)
   {
      switch(gr2CD.ToUpper())
      {
         case "VIN": return "01";
         case "ŽAP": return "02";
         case "PIV": return "03";
         case "BAP": return "04";
         default   : return ""  ;
      }
   }

   private string GetAOPtext(string gr2CD)
   {
      switch(gr2CD.ToUpper())
      {
         case "VIN": return "Vino";
         case "ŽAP": return "Žestoka alkoholna pića";
         case "PIV": return "Pivo";
         case "BAP": return "Bezalkoholna pića";
         default   : return ""  ;
      }
   }

#region XML Export

   // ovdje nisi nasao nikakve upute kako bi se trebao zvati fajl pa ga imenujes proizvoljno 
   public override string ExportFileName
   {
      get
      {
         string mmyyyy = RptFilter.DatumOd.Month.ToString("00") + RptFilter.DatumOd.Year.ToString("0000");
       //string   yyyy =                                          RptFilter.DatumOd.Year.ToString("0000");

         return "PPMIPO_" + ZXC.CURR_prjkt_rec.Oib + "_" + mmyyyy + ".xml";
      }
   }

#region Xml Schema Validation

   public override bool ExecuteExportValidation(string fileName)
   {
      List<ZXC.VvXmlValidationData> valDataList = new List<ZXC.VvXmlValidationData>();

      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPPMIPO/v1-0", @"XSD\ObrazacPPMIPO-v1-0.xsd"));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPPMIPO/v1-0", @"XSD\ObrazacPPMIPOtipovi-v1-0.xsd"));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"            , @"XSD\ObrazacPPMIPOmetapodaci-v1-0.xsd"));

      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\MetapodaciTipovi-v2-0.xsd"));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/TemeljniTipovi/v2-1"     , @"XSD\TemeljniTipovi-v2-1.xsd"  ));

      return ExecuteExportValidation_Base(valDataList);
   }

#endregion Xml Schema Validation
   
   public override bool ExecuteExport(string fileName)
   {
#region Initialize XmlWriterSettings

      //if(IsZP)
      //{
      //   if(Faktur_rec_SumaRazdoblja_IRA == null) throw new Exception("Nema se što exporitrati!");
      //}
      //else // PDV-S 
      //{
      //   if(Faktur_rec_SumaRazdoblja_URA == null) throw new Exception("Nema se što exporitrati!");
      //}

      if(TheDeviznaSumaList.Count().IsZero()) throw new Exception("Nema se što exporitrati!");

      string ident = "   ";

      XmlWriterSettings settings = new XmlWriterSettings();
      
      settings.Indent      = true;
      settings.IndentChars = ident;

#endregion Initialize XmlWriterSettings

      using(XmlWriter writer = XmlWriter.Create(fileName, settings))
      {
#region Init Xml Document

         writer.WriteStartDocument();

         writer.WriteStartElement("ObrazacPPMIPO", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacPPMIPO/v1-0");
         writer.WriteAttributeString("verzijaSheme",                                                                 "1.0");

#endregion Init Xml Document

#region Metapodaci

            writer.WriteStartElement("Metapodaci", @"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0");
            //writer.WriteElementString("Naslov", @"http://purl.org/dc/elements/1.1/title", "Prijava poreza na dodanu vrijednost");
            writer.WriteRaw("\n");
            writer.WriteRaw(ident + ident + "<Naslov dc=\"http://purl.org/dc/elements/1.1/title\">Izvješće o obračunu poreza na potrošnju</Naslov>\n");
            writer.WriteRaw(ident + ident + "<Autor dc=\"http://purl.org/dc/elements/1.1/creator\">" + ZXC.CURR_prjkt_rec.Ime + " " + ZXC.CURR_prjkt_rec.Prezime + "</Autor>\n");
            writer.WriteRaw(ident + ident + "<Datum dc=\"http://purl.org/dc/elements/1.1/date\">" + DateTime.Now.ToString("s") + "</Datum>\n");
            writer.WriteRaw(ident + ident + "<Format dc=\"http://purl.org/dc/elements/1.1/format\">text/xml</Format>\n");
            writer.WriteRaw(ident + ident + "<Jezik dc=\"http://purl.org/dc/elements/1.1/language\">hr-HR</Jezik>\n");
            writer.WriteRaw(ident + ident + "<Identifikator dc=\"http://purl.org/dc/elements/1.1/identifier\">" + Guid.NewGuid().ToString("D")/*.ToUpper()*/ + "</Identifikator>\n");

            writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacPPMIPO-v1-0</Uskladjenost>\n");

            writer.WriteRaw(ident + ident + "<Tip dc=\"http://purl.org/dc/elements/1.1/type\">Elektronički obrazac</Tip>\n");
            writer.WriteRaw(ident + ident + "<Adresant>Ministarstvo Financija, Porezna uprava, Zagreb</Adresant>\n");
            writer.WriteRaw(ident);
         writer.WriteEndElement(); // Metapodaci 

#endregion Write Header Data

#region Zaglavlje

         writer.WriteStartElement("Zaglavlje");

            writer.WriteStartElement("Razdoblje");
               writer.WriteElementString("DatumOd", RptFilter.DatumOd.ToString("s").Substring(0, 10));
               writer.WriteElementString("DatumDo", RptFilter.DatumDo.ToString("s").Substring(0, 10));
            writer.WriteEndElement(); // Razdoblje 

            writer.WriteStartElement("Obveznik");
               writer.WriteElementString("Naziv", ZXC.CURR_prjkt_rec.Naziv);
               writer.WriteElementString("OIB", ZXC.CURR_prjkt_rec.Oib);
               writer.WriteStartElement("Adresa");
                  writer.WriteElementString("Mjesto", ZXC.CURR_prjkt_rec.Grad);
                  writer.WriteElementString("Ulica", ZXC.CURR_prjkt_rec.UlicaBezBroja_1);
                  writer.WriteElementString("Broj", ZXC.CURR_prjkt_rec.UlicniBroj_1);
               writer.WriteEndElement(); // Adresa 
            writer.WriteEndElement(); // Obveznik 

          //writer.WriteStartElement("ObracunSastavio");
          //   writer.WriteElementString("Ime"    , ZXC.CURR_prjkt_rec.Ime);
          //   writer.WriteElementString("Prezime", ZXC.CURR_prjkt_rec.Prezime);
          //   writer.WriteElementString("Telefon", /*ZXC.CURR_prjkt_rec.Tel1 zbog validacije:*/ZXC.CURR_prjkt_rec.Tel2);
          //   writer.WriteElementString("Email"  , ZXC.CURR_prjkt_rec.Email);
          //writer.WriteEndElement(); // ObracunSastavio 
            
         writer.WriteElementString("Ispostava", ZXC.CURR_prjkt_rec.OpcCd);

         writer.WriteEndElement(); // Zaglavlje 

#endregion Zaglavlje

#region Tijelo

         writer.WriteStartElement("Tijelo");
            writer.WriteStartElement("Obracuni");

         int rbr = 0;
         foreach(var rptLine in TheDeviznaSumaList)
         {
            writer.WriteStartElement("Obracun");
               writer.WriteElementString("RedniBroj",        (++rbr).ToString());
             //14.07.2022. za Macana - kada se PNP racuna u mjestu razlicitom od sjedista firme
             //writer.WriteElementString("SifraOpcineGrada", "0" +                                                                                                        ZXC.CURR_prjkt_rec.OpcCd  );
             //writer.WriteElementString("NazivOpcineGrada",                                                                                                              ZXC.CURR_prjkt_rec.Opcina ); 
               writer.WriteElementString("SifraOpcineGrada", "0" + (ZXC.RRD.Dsc_OpcinaCd_PNP.NotEmpty() ? ZXC.RRD.Dsc_OpcinaCd_PNP                                      : ZXC.CURR_prjkt_rec.OpcCd ));
               writer.WriteElementString("NazivOpcineGrada",       (ZXC.RRD.Dsc_OpcinaCd_PNP.NotEmpty() ? ZXC.luiListaOpcina.GetNameForThisCd(ZXC.RRD.Dsc_OpcinaCd_PNP) : ZXC.CURR_prjkt_rec.Opcina));
               writer.WriteElementString("BrojObjekata"    , "1"); // ubuduce? 

               writer.WriteElementString("Osnovica", rptLine.TheMoney.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Stopa"   , rptLine.TheKoef .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Porez"   , rptLine.TheSaldo.ToStringVv_NoGroup_ForceDot()); 
            writer.WriteEndElement();
         }

         writer.WriteEndElement(); // Obracuni 

         writer.WriteStartElement("ObracuniUkupno");
            writer.WriteElementString("UkBrojObjekata", "1"); // ubuduce? 
            writer.WriteElementString("UkOsnovica"    , TheDeviznaSumaList.Sum(rptLine => rptLine.TheMoney).ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("UkPorez"       , TheDeviznaSumaList.Sum(rptLine => rptLine.TheSaldo).ToStringVv_NoGroup_ForceDot());
         writer.WriteEndElement();

         writer.WriteEndElement(); // Tijelo 

#endregion Tijelo

#region Finish Xml Document

         writer.WriteEndElement();
         writer.WriteEndDocument();

#endregion Finish Xml Document

      }
      return true;
   }

#endregion ePDV XML Export

}

#endregion PNP

#endregion Classic RISK Reports
