using System;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using Vektor.Reports.OSR;
using Vektor.DataLayer.DS_AllColumns;
using Vektor.DataLayer.DS_Reports;
using System.Linq;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
using Vektor.Reports.OSR;
using System.Collections.Generic;
using System.Xml;
#endif

public abstract partial class VvOsredReport : VvReport
{
   public List<Osred> TheOsredList { get; set; }

   protected XSqlConnection TheDbConnection { get { return ZXC.TheVvForm.TheDbConnection; } }

   #region Constructor and some propertiez

   public VvOsredReport(string _reportName, VvRpt_Osred_Filter rptFilter) : base(_reportName)
   {
      this.RptFilter = rptFilter;
   }

   public override VvRptFilter VirtualRptFilter { get { return this.RptFilter; } }

   private   VvRpt_Osred_Filter rptFilter;
   protected VvRpt_Osred_Filter RptFilter
   {
      get { return this.rptFilter; }
      set {        this.rptFilter = value; }
   }

   public ZXC.AIZ_FilterStyle FilterStyle;

   #endregion Constructor and some propertiez

   #region GetAtransWithStatusData

   public XSqlCommand GetAtransWithStatusData_Command(XSqlConnection conn, string selectColumns, string orderByColumns, bool needsJoinWithOsred)
   {
      bool weCareAboutIsRashodovan = RptFilter.IsRashodSignif;

      XSqlCommand cmd = VvSQL.InitCommand(conn);

      cmd.CommandText =

      //_________________________________________________________________________________
         "SELECT " + selectColumns + ", " + "\n" +

         Rule_IsRashodovan +

         Rule_KolSt    +
         Rule_UkNabDug +
         Rule_UkNabPot +
         Rule_UkRasDug +
         Rule_UkRasPot +
         Rule_OldAmDug +
         Rule_OldAmPot +
         Rule_NewAmDug +
         Rule_NewAmPot +

         "FROM " +
         (needsJoinWithOsred ? Osred.recordName + " LEFT JOIN " : "") + "\n" +
         Atrans.recordName + "\n" +
         (needsJoinWithOsred ? "ON osredCD = t_osredCD" : "") + "\n" +

         VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(RptFilter) +
         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(RptFilter.FilterMembers, false) +

         (weCareAboutIsRashodovan ? Rule_HavingAdequateRashodState(RptFilter.IsRashodovan) : "") + "\n" +

         "ORDER BY " + orderByColumns;
      //_________________________________________________________________________________

      VvSQL.SetReportCommandParamValues(cmd, this.RptFilter.FilterMembers);


      //--- dateStartAm additions --- Start -------------------------

      DateTime dateStartAm = OsredStatus.GetDateAmRazdobljeStart(RptFilter.AmortRazdoblje, RptFilter.DatumDo);
      VvSQL.CreateCommandNamedParameter(cmd, "filter_", "dateStartAm", dateStartAm, ZXC.AtransSchemaRows[ZXC.AtrCI.t_dokDate]);

      //--- dateStartAm additions --- End ---------------------------


      return cmd;
   }

   #endregion GetAtransWithStatusData

   #region GetAtransWithOUTStatusData

   public XSqlCommand GetAtransWithOUTStatusData_Command(XSqlConnection conn, string selectColumns, string orderByColumns)
   {
      bool weCareAboutIsRashodovan = RptFilter.IsRashodSignif;

      XSqlCommand cmd = VvSQL.InitCommand(conn);

      cmd.CommandText =

      //_________________________________________________________________________________
         "SELECT " + "\n" +

         Rule_IsRashodovan +

         selectColumns + "\n" +
         "FROM " + Atrans.recordName + "\n" +
         VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(RptFilter) +
         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(RptFilter.FilterMembers, false) +

         (weCareAboutIsRashodovan ? Rule_HavingAdequateRashodState(RptFilter.IsRashodovan) : "") + "\n" +

         "ORDER BY " + orderByColumns;
      //_________________________________________________________________________________

      VvSQL.SetReportCommandParamValues(cmd, this.RptFilter.FilterMembers);

      return cmd;
   }

   #endregion GetAtransWithStatusData

   #region GetOsredWithStatusData_Command

   protected XSqlCommand GetOsredWithStatusData_Command(XSqlConnection conn, bool needsDateNab)
   {
      bool weCareAboutIsRashodovan = RptFilter.IsRashodSignif;

      XSqlCommand cmd = VvSQL.InitCommand(conn);

      cmd.CommandText =

      //_________________________________________________________________________________
         "SELECT " + "\n" +

         // 10.11.2015: 
       //"osred.osredCD AS t_osredCD, osred.naziv AS t_opis_oNaziv, osred.konto AS t_koef_am_oKonto, osred.amort_st AS t_amort_st, \n" +
         "osred.osredCD AS t_osredCD, osred.naziv AS t_opis_oNaziv, osred.konto AS t_koef_am_oKonto, osred.amort_st AS t_amort_st, osred.invbr_od AS invbr_od, \n" +

         (needsDateNab ? "CAST(MIN(IF(t_tt = '" + Amort.NABAVA_TT + "', t_dokDate, '2999-12-31')) AS DATE) AS DateNab, \n" : "") +

         Rule_IsRashodovan +

         //"SUM" + Rule_KolSt +
         //"SUM" + Rule_UkNabDug +
         //"SUM" + Rule_UkNabPot +
         //"SUM" + Rule_UkRasDug +
         //"SUM" + Rule_UkRasPot +
         //"SUM" + Rule_OldAmDug +
         //"SUM" + Rule_OldAmPot +
         //"SUM" + Rule_NewAmDug +
         //"SUM" + Rule_NewAmPot;
         SumStatusRulesClause +

         "FROM " + Osred.recordName + " LEFT JOIN " + Atrans.recordName + "\n" +

         "ON osredCD = t_osredCD \n" +

         VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(RptFilter) +
         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(RptFilter.FilterMembers, false) +

         "GROUP BY osred.osredCD " +

         (weCareAboutIsRashodovan ? Rule_HavingAdequateRashodState(RptFilter.IsRashodovan) + " OR KolSt != 0 ": "") + "\n" +

         "ORDER BY " + GetOsredSifrar_OrderByColumns(RptFilter.SorterType_Sifrar);
      //_________________________________________________________________________________

      VvSQL.SetReportCommandParamValues(cmd, RptFilter.FilterMembers);


      //--- dateStartAm additions --- Start -------------------------

      DateTime dateStartAm = OsredStatus.GetDateAmRazdobljeStart(RptFilter.AmortRazdoblje, RptFilter.DatumDo);
      VvSQL.CreateCommandNamedParameter(cmd, "filter_", "dateStartAm", dateStartAm, ZXC.AtransSchemaRows[ZXC.AtrCI.t_dokDate]);

      //--- dateStartAm additions --- End ---------------------------

      return cmd;
   }

   #endregion GetOsredWithStatusData_Command

   #region GetCommon_ExternTable_Command_ForAtrans

   public XSqlCommand GetCommon_ExternTable_Command_ForAtrans(XSqlConnection conn, string tableName)
   {
      XSqlCommand cmd = VvSQL.InitCommand(conn);

      VirtualRptFilter.ClearAllFilters_FromClauseGotTableName();

      switch(tableName)
      {
         case Osred .recordName       : cmd.CommandText = CommonExternOsredsCommandText();              break;
         case Amort .recordName       : cmd.CommandText = CommonExternAmortsCommandText();              break;
         //case Amort .tableName+"AllC": cmd.CommandText = CommonExternAmortsAllCCommandText();          break;

         default: ZXC.aim_emsg("GetCommon_ParentTable_Command tableName '{0}' not recognized!"); break;
      }

      VvSQL.SetReportCommandParamValues(cmd, VirtualRptFilter.FilterMembers);

      return cmd;
   }

   protected string CommonExternOsredsCommandText()
   {
      VirtualRptFilter.FromClauseGot_Osred_TableName = true;

      //DataTable osredDT = ((Vektor.DataLayer.DS_Reports.DS_DnevnikAM)VirtualUntypedDataSet).Tables["osred"];

      return
      //"SELECT " + VvSQL.GetAllDataTableColumnNames_4Select(osredDT, Osred.tableName, false) + ", \n" +
      "SELECT " +
      "osred.naziv,     osred.osredCD,   osred.konto,    osred.konto_iv, osred.grupa,    osred.ser_br, osred.mtros_cd, osred.mtros_tk, " +
      "osred.kupdob_cd, osred.kupdob_tk, osred.dokum_cd, osred.koef_am,  osred.amort_st, osred.vijek,  osred.isRashod, osred.invbr_od, \n" +
      "k.naziv AS kpdb_naziv, m.naziv AS mtrs_naziv \n" +
      "FROM " + Atrans.recordName + " t \n" +
      "LEFT JOIN " + Osred .recordName +"  osred ON osred.osredCD   = " + Atrans.OsredForeignKey + " \n" +
      "LEFT JOIN " + Kupdob.recordName + " k     ON osred.kupdob_cd = k.kupdobCD \n" +
      "LEFT JOIN " + Kupdob.recordName + " m     ON osred.mtros_cd  = m.kupdobCD \n" +
      VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(VirtualRptFilter) +
      VvSQL.ParameterizedWhereClauseFromVvSqlFilter(VirtualRptFilter.FilterMembers, false) + "\n" +
      "GROUP BY osred.osredCD";
   }

   protected string CommonExternAmortsCommandText()
   {
      return
      "SELECT amo.recID, amo.napomena, amo.ttNum \n" +
      "FROM " + Amort.recordName + " amo JOIN " + Atrans.recordName + " ON " + Atrans.AmortForeignKey + " = amo.recID\n" +
      VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(VirtualRptFilter) +
      VvSQL.ParameterizedWhereClauseFromVvSqlFilter(VirtualRptFilter.FilterMembers, false) + "\n" +
      "GROUP BY amo.recID";
   }

   //protected string CommonExternAmortsAllCCommandText()
   //{
   //   return
   //   "SELECT " + VvSQL.GetAllDataTableColumnNames_4Select(VirtualUntypedDataSet.Tables["amort"], "amo", false) + "\n" +
   //   "FROM " + Amort.tableName + " amo JOIN " + Atrans.tableName + " ON " + Atrans.AmortForeignKey + " = amo.prjktKupdobCD\n" +
   //   VvSQL.EventualTableRelationNameFor_Xtrans(VirtualRptFilter) +
   //   VvSQL.ParameterizedWhereClauseFromVvSqlFilter(VirtualRptFilter.TheFilterMembers, false) + "\n" +
   //   "GROUP BY amo.prjktKupdobCD";
   //}

   #endregion GetCommon_ExternTable_Command_ForAtrans

   #region Status Rules

 //internal const string Rule_IsRashodovan = "IF(COUNT(t_tt = '" + Amort.RASHOD_TT + "') > 0 AND SUM(t_dug)-SUM(t_pot) = 0, 1, 0) AS IsRashodovan, \n";
   // SubQuery: 
   internal const  string Rule_IsRashodovan = "IF((SELECT COUNT(*) FROM atrans sub WHERE sub.t_osredCD = atrans.t_osredCD AND t_tt = '" + Amort.RASHOD_TT + "') > 0, 1, 0 ) AS IsRashodovan, \n";
   internal static string Rule_HavingAdequateRashodState(ZXC.JeliJeTakav rashodovan)
   {
      return "HAVING IsRashodovan = " + (rashodovan == ZXC.JeliJeTakav.JE_TAKAV ? "1" : "0");
   }

   internal const string Rule_InvSt    = "(IF(t_tt = '" + Amort.INVENT_TT + "' && t_dokDate =  ?filter_dateDO     , t_kol, 0)) AS InvSt, \n";

   internal const string Rule_KolSt    = "(CASE t_tt WHEN '" + Amort.NABAVA_TT + "' THEN      t_kol \n" +
                                          "          WHEN '" + Amort.RASHOD_TT + "' THEN -1 * t_kol ELSE 0 END)                AS KolSt   , \n";

   internal const string Rule_UkNabDug = "(IF(t_tt = '" + Amort.NABAVA_TT + "'                                    , t_dug, 0)) AS UkNabDug, \n";
   internal const string Rule_UkNabPot = "(IF(t_tt = '" + Amort.NABAVA_TT + "'                                    , t_pot, 0)) AS UkNabPot, \n";
   internal const string Rule_UkRasDug = "(IF(t_tt = '" + Amort.RASHOD_TT + "'                                    , t_dug, 0)) AS UkRasDug, \n";
   internal const string Rule_UkRasPot = "(IF(t_tt = '" + Amort.RASHOD_TT + "'                                    , t_pot, 0)) AS UkRasPot, \n";
   internal const string Rule_OldAmDug = "(IF(t_tt = '" + Amort.AMORT_TT  + "' && t_dokDate <  ?filter_dateStartAm, t_dug, 0)) AS OldAmDug, \n";
   internal const string Rule_OldAmPot = "(IF(t_tt = '" + Amort.AMORT_TT  + "' && t_dokDate <  ?filter_dateStartAm, t_pot, 0)) AS OldAmPot, \n";
   internal const string Rule_NewAmDug = "(IF(t_tt = '" + Amort.AMORT_TT  + "' && t_dokDate >= ?filter_dateStartAm, t_dug, 0)) AS NewAmDug, \n";
   internal const string Rule_NewAmPot = "(IF(t_tt = '" + Amort.AMORT_TT  + "' && t_dokDate >= ?filter_dateStartAm, t_pot, 0)) AS NewAmPot  \n";

   internal const string CommonAtransColumnsList = "t_dokDate, t_dokNum, t_serial AS t_serial_aTtNum, t_tt, t_osredCD, t_opis AS t_opis_oNaziv, t_kol, t_koef_am AS t_koef_am_oKonto, t_amort_st, t_dug, t_pot ";

   internal const string SumStatusRulesClause =
      "SUM" + Rule_InvSt +
      "SUM" + Rule_KolSt +
      "SUM" + Rule_UkNabDug +
      "SUM" + Rule_UkNabPot +
      "SUM" + Rule_UkRasDug +
      "SUM" + Rule_UkRasPot +
      "SUM" + Rule_OldAmDug +
      "SUM" + Rule_OldAmPot +
      "SUM" + Rule_NewAmDug +
      "SUM" + Rule_NewAmPot;

   #endregion Status Rules

   #region GetOsredSifrar_OrderByColumns

   protected string GetOsredSifrar_OrderByColumns(VvSQL.SorterType sorterType)
   {
      string orderColumns = "Dokum ORDER_BAJ Very BAD! in GetOsredSifrar_OrderByColumns()";

      switch(sorterType)
      {
         case VvSQL.SorterType.Name: orderColumns = " naziv";   break;
         case VvSQL.SorterType.Code: orderColumns = " osredCD"; break;
      }
      return orderColumns;
   }
   
   #endregion GetOsredSifrar_OrderByColumns

}


//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptO_KartOsred : VvOsredReport
{
   private CR_KOsred    cr_KOsred    = new CR_KOsred();
   private DS_DnevnikAM ds_DnevnikAM = new DS_DnevnikAM();

   public override ReportDocument VirtualReportDocument { get { return cr_KOsred;    } }
   public override DataSet        VirtualUntypedDataSet { get { return ds_DnevnikAM; } }

   public RptO_KartOsred(string reportName, VvRpt_Osred_Filter rptFilter) : base(reportName, rptFilter)
   {
      FilterStyle = ZXC.AIZ_FilterStyle.Atrans;
      
      EnableDrillDown = true;

      ReportDatasetNeedsExternTable_Osred = true;

      ReportDatasetNeedsExternTable_Amort = true;
   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      //return GetAtransWithStatusData_Command(conn, CommonAtransColumnsList + "t_parentID, ", "t_personCD, " + GetGenericDnevnik_OrderByColumns(RptFilter.SorterType_Dokument), false);
      return GetAtransWithOUTStatusData_Command(conn, CommonAtransColumnsList + ", t_parentID ", "t_osredCD, " + GetGenericDnevnik_OrderByColumns(RptFilter.SorterType_Dokument));
   }

   public override XSqlCommand GetOsredCommand(XSqlConnection conn)
   {
      return GetCommon_ExternTable_Command_ForAtrans(conn, Osred.recordName);
   }

   public override XSqlCommand GetAmortCommand(XSqlConnection conn)
   {
      return GetCommon_ExternTable_Command_ForAtrans(conn, Amort.recordName);
   }

   public static bool IsFilterWellFormed(AmoReportUC reportUC)
   {
      bool OK = true;
      VvRpt_Osred_Filter filter = reportUC.TheRptFilter;

      if(String.IsNullOrEmpty(filter.OsredCDod))
      {
         ZXC.aim_emsg("Molim, zadajte ARTIKL.");
         return false;
      }

      if(filter.OsredCDdo != filter.OsredCDod)
      {
         //reportUC.Set_KontoDoText_ThreadSafe(reportUC.TheFinFilterUC.Fld_KontoOd);
         VvHamper.Set_ControlText_ThreadSafe(reportUC.TheOsredFilterUC.tbx_sifraDo, filter.OsredCDod);
         reportUC.GetFields(false);
      }

      return (OK);
   }

}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptO_DnevnikAM : VvOsredReport
{
   private CR_DnevnikAM cr_dnevnik    = new CR_DnevnikAM();
   private DS_DnevnikAM ds_DnevnikAM  = new DS_DnevnikAM();

   public override ReportDocument VirtualReportDocument { get { return cr_dnevnik; } }
   public override DataSet        VirtualUntypedDataSet { get { return ds_DnevnikAM; } }

   public RptO_DnevnikAM(string reportName, VvRpt_Osred_Filter rptFilter) : base(reportName, rptFilter)
   {
      FilterStyle = ZXC.AIZ_FilterStyle.Atrans;
   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      //return GetAtransWithStatusData_Command(conn, CommonAtransColumnsList, GetGenericDnevnik_OrderByColumns(RptFilter.SorterType_Dokument), false);
      return GetAtransWithOUTStatusData_Command(conn, CommonAtransColumnsList, GetGenericDnevnik_OrderByColumns(RptFilter.SorterType_Dokument));
   }
}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptO_PopisDI : VvOsredReport
{
   private CR_PopisDI   cr_PopisDI   = new CR_PopisDI();
   private DS_DnevnikAM ds_DnevnikAM = new DS_DnevnikAM();

   public override ReportDocument VirtualReportDocument { get { return cr_PopisDI;   } }
   public override DataSet        VirtualUntypedDataSet { get { return ds_DnevnikAM; } }

   public RptO_PopisDI(string reportName, VvRpt_Osred_Filter rptFilter) : base(reportName, rptFilter)
   {
      FilterStyle = ZXC.AIZ_FilterStyle.Atrans;
   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      RptFilter.FromClauseGot_Osred_TableName = true;

      return GetOsredWithStatusData_Command(conn, false);
   }

}

public class RptO_PopisDI_DD : VvOsredReport
{
   private CR_PopisDI_DD  cr_PopisDI   = new CR_PopisDI_DD();
   private DS_DnevnikAM   ds_DnevnikAM = new DS_DnevnikAM();

   public override ReportDocument VirtualReportDocument { get { return cr_PopisDI;   } }
   public override DataSet        VirtualUntypedDataSet { get { return ds_DnevnikAM; } }

   public RptO_PopisDI_DD(string reportName, VvRpt_Osred_Filter rptFilter) : base(reportName, rptFilter)
   {
      FilterStyle = ZXC.AIZ_FilterStyle.Atrans;

      EnableDrillDown = true;

      ReportDatasetNeedsExternTable_Osred = true;
   }

   public override XSqlCommand GetOsredCommand(XSqlConnection conn)
   {
      return GetCommon_ExternTable_Command_ForAtrans(conn, Osred.recordName);
   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      RptFilter.FromClauseGot_Osred_TableName = true;

      // ovdje ti osim 'ReportDatasetNeedsExternTable_Osred' treba i JOIN da IzvjTable dobije osred.naziv, po kojemu mozes sortirati, 
      // brez JOIN-a ne mozes SORTIRATI po osred.naziv jerbo ga nema u IzvjTable 
      return GetAtransWithStatusData_Command(conn, 
         
         /* selectColumns      */ "osred.naziv, " + CommonAtransColumnsList, 
         /* orderByColumns     */ GetOsredSifrar_OrderByColumns(RptFilter.SorterType_Sifrar) + ", " + GetGenericDnevnik_OrderByColumns(RptFilter.SorterType_Dokument), 
         /* needsJoinWithOsred */ true);
   }
}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptO_ListaPoKontu : VvOsredReport
{
   private CR_ListaPoKontu cr_ListaPoKontu = new CR_ListaPoKontu();
   private DS_DnevnikAM    ds_DnevnikAM    = new DS_DnevnikAM();

   public override ReportDocument VirtualReportDocument { get { return cr_ListaPoKontu; } }
   public override DataSet        VirtualUntypedDataSet { get { return ds_DnevnikAM; } }

   public RptO_ListaPoKontu(string reportName, VvRpt_Osred_Filter rptFilter) : base(reportName, rptFilter)
   {
      FilterStyle = ZXC.AIZ_FilterStyle.Atrans;
   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      RptFilter.FromClauseGot_Osred_TableName = true;

      return GetOsredWithStatusData_Command(conn, true);
   }

}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptO_RekapAmo : VvOsredReport
{
   private CR_RekapAmo  cr_RekapAmo  = new CR_RekapAmo();
   private DS_DnevnikAM ds_DnevnikAM = new DS_DnevnikAM();

   public override ReportDocument VirtualReportDocument { get { return cr_RekapAmo; } }
   public override DataSet        VirtualUntypedDataSet { get { return ds_DnevnikAM; } }

   public RptO_RekapAmo(string reportName, VvRpt_Osred_Filter rptFilter) : base(reportName, rptFilter)
   {
      FilterStyle = ZXC.AIZ_FilterStyle.Amort;
   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      XSqlCommand cmd = VvSQL.InitCommand(conn);

      cmd.CommandText =

         "SELECT " +
         "amort.dokDate AS t_dokDate, amort.dokNum AS t_dokNum, amort.tt AS t_tt, amort.napomena AS t_opis_oNaziv, amort.ttNum AS t_serial_aTtNum, \n" +
         "SUM(t_dug) AS t_dug, \n" +
         "SUM(t_pot) AS t_pot  \n" +
         "FROM " + Amort.recordName + " LEFT JOIN " + Atrans.recordName + "\n" +
         " ON "  + Amort.recordName + ".recID = "   + Atrans.recordName + "." + Atrans.AmortForeignKey +

         VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(RptFilter) +

         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(RptFilter.FilterMembers, false) +

         "GROUP BY " + Amort.recordName + ".dokNum " +

         "ORDER BY"  + GetGenericRekapDokumZaglavlje_OrderByColumns(RptFilter.SorterType_Dokument);

      VvSQL.SetReportCommandParamValues(cmd, this.RptFilter.FilterMembers);

      return cmd;
   }
}

public class RptO_RekapAmo_DD : VvOsredReport
{
   private CR_RekapAmo_DD cr_RekapAmo_DD = new CR_RekapAmo_DD();
   private DS_DnevnikAM   ds_DnevnikAM   = new DS_DnevnikAM();

   public override ReportDocument VirtualReportDocument { get { return cr_RekapAmo_DD; } }
   public override DataSet        VirtualUntypedDataSet { get { return ds_DnevnikAM;   } }

   public RptO_RekapAmo_DD(string reportName, VvRpt_Osred_Filter rptFilter) : base(reportName, rptFilter)
   {
      FilterStyle = ZXC.AIZ_FilterStyle.Amort;

      rptFilter.NeedsDrillDown = true;
      EnableDrillDown = true;

      ReportDatasetNeedsExternTable_Osred = true;

      ReportDatasetNeedsExternTable_Amort = true;
   }

   public override XSqlCommand GetOsredCommand(XSqlConnection conn)
   {
      return GetCommon_ExternTable_Command_ForAtrans(conn, Osred.recordName);
   }

   public override XSqlCommand GetAmortCommand(XSqlConnection conn)
   {
      return GetCommon_ExternTable_Command_ForAtrans(conn, Amort.recordName);
   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      //return GetAtransWithStatusData_Command(conn, CommonAtransColumnsList + "t_parentID, ", GetGenericDnevnik_OrderByColumns(RptFilter.SorterType_Dokument), false);
      return GetAtransWithOUTStatusData_Command(conn, CommonAtransColumnsList + ", t_parentID ", GetGenericDnevnik_OrderByColumns(RptFilter.SorterType_Dokument));
   }
}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptO_AmortDocument : VvOsredReport
{
   private CR_AmortDokument cr_AmortDocument = new CR_AmortDokument();
   private DS_DnevnikAM     ds_DnevnikAM     = new DS_DnevnikAM();

   public override ReportDocument VirtualReportDocument { get { return cr_AmortDocument; } }
   public override DataSet        VirtualUntypedDataSet { get { return ds_DnevnikAM;   } }

   public RptO_AmortDocument(string reportName, VvRpt_Osred_Filter rptFilter) : base(reportName, rptFilter)
   {
      ReportDatasetNeedsExternTable_Osred = true;

      ReportDatasetNeedsExternTable_Amort = true;
   }

   public override XSqlCommand GetOsredCommand(XSqlConnection conn)
   {
      return GetCommon_ExternTable_Command_ForAtrans(conn, Osred.recordName);
   }

   public override XSqlCommand GetAmortCommand(XSqlConnection conn)
   {
      return GetCommon_ExternTable_Command_ForAtrans(conn, Amort.recordName);
   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      return GetAtransWithOUTStatusData_Command(conn, CommonAtransColumnsList + ", t_parentID ", GetGenericDnevnik_OrderByColumns(VvSQL.SorterType.DokNum));
   }
}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptO_InvLista : RptO_InvDiff
{
   private CR_InvLista  cr_InvLista  = new CR_InvLista();

   public override ReportDocument VirtualReportDocument { get { return cr_InvLista; } }

   public RptO_InvLista(string reportName, VvRpt_Osred_Filter rptFilter) : base(reportName, rptFilter)
   {
   }

}

public class RptO_InvDiff : VvOsredReport
{
   private CR_InvDiff   cr_InvDiff   = new CR_InvDiff();
   private DS_DnevnikAM ds_DnevnikAM = new DS_DnevnikAM();

   public override ReportDocument VirtualReportDocument { get { return cr_InvDiff;   } }
   public override DataSet        VirtualUntypedDataSet { get { return ds_DnevnikAM; } }

   public RptO_InvDiff(string reportName, VvRpt_Osred_Filter rptFilter) : base(reportName, rptFilter)
   {
      FilterStyle = ZXC.AIZ_FilterStyle.Atrans;

      // overrjebbing pozicije radio button-a za 'ocemo samo aktivne, rashodovane, ...'
      // dakle, kao hocemo samo aktivne 
      RptFilter.IsRashodSignif   = true;
      RptFilter.WeWantOnlyRashod = false;
      //ReportDatasetNeedsExternTable_Osred = true;

   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      RptFilter.FromClauseGot_Osred_TableName = true;

      return GetOsredWithStatusData_Command(conn, false);
   }

   //public override XSqlCommand GetOsredCommand(XSqlConnection conn)
   //{
   //   return GetCommon_ExternTable_Command_ForAtrans(conn, Osred.recordName);
   //}

}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptO_ObrazacDI : VvOsredReport
{
   private CR_ObrazacDI cr_obrazacDI = new CR_ObrazacDI();
   private DS_DnevnikAM ds_DnevnikAM = new DS_DnevnikAM();

   public override ReportDocument VirtualReportDocument { get { return cr_obrazacDI; } }
 //public override DataSet        VirtualUntypedDataSet { get { return ds_DnevnikAM; } }
   public override DataSet        VirtualUntypedDataSet { get { return null        ; } }

   public RptO_ObrazacDI(string reportName, VvRpt_Osred_Filter rptFilter) : base(reportName, rptFilter)
   {
      FilterStyle = ZXC.AIZ_FilterStyle.Atrans;

      IsForExport = true;

      ReportNeeds_Osred_List = true;

      this.TheOsredList = new List<Osred>();
   }

   public override int FillOsredReportLists()
   {
      OsredDao.GetOsredWithOsrstatList(TheDbConnection, TheOsredList, RptFilter.DatumDo, RptFilter, /*RptFilter.DatumOd*/ZXC.projectYearFirstDay, GetOsredSifrar_OrderByColumns(RptFilter.SorterType_Sifrar));

      // porezna 'oce 'vako: 
      TheOsredList = TheOsredList.OrderBy(osr => osr.O_DateNabava).ThenBy(osr => osr.Naziv).ToList();

      foreach(Osred osred in TheOsredList)
      {
         if(osred.O_OldKnjVr.IsZero())
         {
            // 22.02.2017:
          //osred.Vijek =  osred.AmortSt = 0M;
          /*osred.Vijek =*/osred.AmortSt = 0M;
         }
      }

      // 10.01.2018: remove osred rashodovani prije tekuce godine 
      TheOsredList.RemoveAll(osr => osr.O_IsRashodovan && osr.O_DateRashod < ZXC.projectYearFirstDay);

      return TheOsredList.Count;
   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
    //RptFilter.FromClauseGot_Osred_TableName = true;
    //return GetOsredWithStatusData_Command(conn, true);
      return null;
   }

   #region XML Export

   // ovdje nisi nasao nikakve upute kako bi se trebao zvati fajl pa ga imenujes proizvoljno 
   public override string ExportFileName
   {
      get
      {
         string mmyyyy = RptFilter.DatumOd.Month.ToString("00") + RptFilter.DatumOd.Year.ToString("0000");
         string   yyyy =                                          RptFilter.DatumOd.Year.ToString("0000");

         return "ObrazacDI_" + ZXC.CURR_prjkt_rec.Oib + "_" + ZXC.projectYear + ".xml";
      }
   }

   #region Xml Schema Validation

   public override bool ExecuteExportValidation(string fileName)
   {
      List<ZXC.VvXmlValidationData> valDataList = new List<ZXC.VvXmlValidationData>();

      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacDI/v2-0", @"XSD\ObrazacDI-v2-0.xsd"));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacDI/v2-0", @"XSD\ObrazacDItipovi-v2-0.xsd"));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"        , @"XSD\ObrazacDImetapodaci-v2-0.xsd"));

      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"        , @"XSD\MetapodaciTipovi-v2-0.xsd"));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/TemeljniTipovi/v2-1"    , @"XSD\TemeljniTipovi-v2-1.xsd"  ));

      return ExecuteExportValidation_Base(valDataList);
   }

   #endregion Xml Schema Validation
   
   public override bool ExecuteExport(string fileName)
   {
      #region Initialize XmlWriterSettings

      DS_DnevnikAM.IzvjTableDataTable izvjTable = ds_DnevnikAM.IzvjTable;

      if(TheOsredList.Count.IsZero()) throw new Exception("Nema se što exporitrati!");

      string ident = "   ";

      XmlWriterSettings settings = new XmlWriterSettings();
      
      settings.Indent      = true;
      settings.IndentChars = ident;

      #endregion Initialize XmlWriterSettings

      using(XmlWriter writer = XmlWriter.Create(fileName, settings))
      {
         #region Init Xml Document

         writer.WriteStartDocument();

         writer.WriteStartElement   ("ObrazacDI", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacDI/v2-0");
         writer.WriteAttributeString("verzijaSheme",                                                            "2.0");

         #endregion Init Xml Document

         #region Metapodaci

         writer.WriteStartElement("Metapodaci", @"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0");
            writer.WriteRaw("\n");
            writer.WriteRaw(ident + ident + "<Naslov dc=\"http://purl.org/dc/elements/1.1/title\">Popis dugotrajne imovine</Naslov>\n");
            writer.WriteRaw(ident + ident + "<Autor dc=\"http://purl.org/dc/elements/1.1/creator\">" + ZXC.CURR_prjkt_rec.Ime + " " + ZXC.CURR_prjkt_rec.Prezime + "</Autor>\n");
            writer.WriteRaw(ident + ident + "<Datum dc=\"http://purl.org/dc/elements/1.1/date\">" + DateTime.Now.ToString("s") + "</Datum>\n");
            writer.WriteRaw(ident + ident + "<Format dc=\"http://purl.org/dc/elements/1.1/format\">text/xml</Format>\n");
            writer.WriteRaw(ident + ident + "<Jezik dc=\"http://purl.org/dc/elements/1.1/language\">hr-HR</Jezik>\n");
            writer.WriteRaw(ident + ident + "<Identifikator dc=\"http://purl.org/dc/elements/1.1/identifier\">" + Guid.NewGuid().ToString("D")/*.ToUpper()*/ + "</Identifikator>\n");

            writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacDI-v2-0</Uskladjenost>\n");

            writer.WriteRaw(ident + ident + "<Tip dc=\"http://purl.org/dc/elements/1.1/type\">Elektronički obrazac</Tip>\n");
            writer.WriteRaw(ident + ident + "<Adresant>Ministarstvo Financija, Porezna uprava, Zagreb</Adresant>\n");
            writer.WriteRaw(ident);
         writer.WriteEndElement(); // Metapodaci 

         #endregion Write Header Data

         #region Zaglavlje

         writer.WriteStartElement("Zaglavlje");

            writer.WriteStartElement("Razdoblje"); 
             //writer.WriteElementString("DatumOd", RptFilter.DatumOd.ToString("s").Substring(0, 10));
             //writer.WriteElementString("DatumDo", RptFilter.DatumDo.ToString("s").Substring(0, 10));
               writer.WriteElementString("DatumOd", ZXC.projectYearFirstDay.Date.ToString("s").Substring(0, 10));
               writer.WriteElementString("DatumDo", ZXC.projectYearLastDay .Date.ToString("s").Substring(0, 10));
            writer.WriteEndElement(); // Razdoblje 

            writer.WriteStartElement("Obveznik");
               writer.WriteElementString("Naziv", ZXC.CURR_prjkt_rec.Naziv);
               writer.WriteElementString("OIB"  , ZXC.CURR_prjkt_rec.Oib  );
               writer.WriteStartElement("Adresa");
                  writer.WriteElementString("Mjesto", ZXC.CURR_prjkt_rec.Grad);
                  writer.WriteElementString("Ulica" , ZXC.CURR_prjkt_rec.UlicaBezBroja_1);
                  writer.WriteElementString("Broj"  , ZXC.CURR_prjkt_rec.UlicniBroj_1);
               writer.WriteEndElement(); // Adresa 
               writer.WriteElementString("Telefon", /*ZXC.CURR_prjkt_rec.Tel1 zbog validacije:*/ZXC.CURR_prjkt_rec.Tel2);
               writer.WriteElementString("Email"    , ZXC.CURR_prjkt_rec.Email);
               writer.WriteElementString("SifraDjelatnosti", ZXC.CURR_prjkt_rec.SifDcd);
            writer.WriteEndElement(); // Obveznik 

            writer.WriteStartElement("ObracunSastavio");
               writer.WriteElementString("Ime"    , ZXC.CURR_prjkt_rec.Ime    );
               writer.WriteElementString("Prezime", ZXC.CURR_prjkt_rec.Prezime);
               writer.WriteElementString("Telefon", /*ZXC.CURR_prjkt_rec.Tel1 zbog validacije:*/ZXC.CURR_prjkt_rec.Tel2);
               writer.WriteElementString("Fax"    , /*ZXC.CURR_prjkt_rec.Tel1 zbog validacije:*/ZXC.CURR_prjkt_rec.Tel2);
               writer.WriteElementString("Email"  , ZXC.CURR_prjkt_rec.Email);
            writer.WriteEndElement(); // ObracunSastavio 

            writer.WriteElementString("Ispostava", ZXC.CURR_prjkt_rec.OpcCd);
          //writer.WriteElementString("NaDan",           RptDate_NaDan          .ToString("s").Substring(0, 10));
          //writer.WriteElementString("NisuNaplaceniDo", RptDate_NisuNaplaceniDo.ToString("s").Substring(0, 10)); 

         writer.WriteEndElement(); // Zaglavlje 

         #endregion Zaglavlje

         #region Tijelo

         writer.WriteStartElement("Tijelo");

         writer.WriteStartElement("Stavke");

         uint rbr = 0;
       //foreach(DS_DnevnikAM.IzvjTableRow osredRow in izvjTable.Rows)
         foreach(Osred osred_rec in TheOsredList)
         {
            writer.WriteStartElement("Stavka");

               writer.WriteElementString("RedniBroj"               , (++rbr)               .ToString()                    );
               writer.WriteElementString("NazivStvariIliPrava"     , osred_rec.Naziv                                      );
               writer.WriteElementString("IspravaBroj"             , osred_rec.DokumCd                                    );
               writer.WriteElementString("IspravaNadnevak"         , osred_rec.O_DateNabava.ToString("s").Substring(0, 10));
               writer.WriteElementString("NabavnaVrijednost"       , osred_rec.O_UkNabDugS .ToStringVv_NoGroup_ForceDot() );
               writer.WriteElementString("KnjigVrijednost"         , osred_rec.O_OldKnjVr  .ToStringVv_NoGroup_ForceDot() );
               writer.WriteElementString("VijekTrajanja"           , ((int)(osred_rec.Vijek  ))     .ToString()           );
               writer.WriteElementString("StopaOtpisa"             , ((int)(osred_rec.AmortSt))     .ToString()           );
               writer.WriteElementString("SvotaOtpisa"             , osred_rec.O_NewAmPotS .ToStringVv_NoGroup_ForceDot() );
               writer.WriteElementString("KnjigVrijedNaKrajuGodine", osred_rec.O_NewKnjVr  .ToStringVv_NoGroup_ForceDot() );
               writer.WriteElementString("SifraAopOznakeDugIm"     , osred_rec.Grupa                                      );
               writer.WriteElementString("DatumOtudenjaDugIm"      , osred_rec.O_DateRashod.IsEmpty() ? "" :
                                                                     osred_rec.O_DateRashod.ToString("s").Substring(0, 10)); 
              
            writer.WriteEndElement(); // Stavka 
         }

         writer.WriteEndElement(); // Stavke 

         writer.WriteElementString("UkNabavnaVrijednost"       , TheOsredList.Sum(osr => osr.O_UkNabDugS).ToStringVv_NoGroup_ForceDot());
         writer.WriteElementString("UkKnjigVrijednost"         , TheOsredList.Sum(osr => osr.O_OldKnjVr ).ToStringVv_NoGroup_ForceDot());
         writer.WriteElementString("UkSvotaOtpisa"             , TheOsredList.Sum(osr => osr.O_NewAmPotS).ToStringVv_NoGroup_ForceDot());
         writer.WriteElementString("UkKnjigVrijedNaKrajuGodine", TheOsredList.Sum(osr => osr.O_NewKnjVr ).ToStringVv_NoGroup_ForceDot());

         writer.WriteEndElement(); // Tijelo 

         #endregion Tijelo

         #region Finish Xml Document

         writer.WriteEndElement(); // ObrazacDI
         writer.WriteEndDocument();

         #endregion Finish Xml Document

      }

      return true;
   }

   #endregion ePDV XML Export

}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

