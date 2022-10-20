using System;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using Vektor.Reports.KIZ;
using Vektor.DataLayer.DS_AllColumns;
using Vektor.DataLayer.DS_Reports;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
#endif

public abstract partial class VvKupdobReport : VvReport
{
   #region Constructor and some propertiez

   public VvKupdobReport(string _reportName, VvRpt_Kupdob_Filter rptFilter) : base(_reportName)
   {
      this.RptFilter = rptFilter;
   }

   public override VvRptFilter VirtualRptFilter { get { return this.RptFilter; } }

   private   VvRpt_Kupdob_Filter rptFilter;
   protected VvRpt_Kupdob_Filter RptFilter
   {
      get { return this.rptFilter; }
      set {        this.rptFilter = value;  }
   }

   #endregion Constructor and some propertiez

   #region GetKupdob_Command

   protected XSqlCommand GetKupdob_Command(XSqlConnection conn, DataTable dtForColumnNames, bool isForPrjkt)
   {
      string tableName = (isForPrjkt == true ? Prjkt.recordName : Kupdob.recordName);

      XSqlCommand cmd = VvSQL.InitCommand(conn);

      cmd.CommandText =

      //_________________________________________________________________________________
         "SELECT " + "\n" +

         VvSQL.GetAllDataTableColumnNames_4Select(dtForColumnNames, tableName, false, false) + "\n" +

         "FROM " + tableName + "\n" +

         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(RptFilter.FilterMembers, false) +

         "ORDER BY " + GetPartnerSifrar_OrderByColumns(RptFilter.SorterType_Sifrar);
      //_________________________________________________________________________________

      VvSQL.SetReportCommandParamValues(cmd, RptFilter.FilterMembers);

      return cmd;
   }

   #endregion GetKupdob_Command

   #region GetPartnerSifrar_OrderByColumns

   protected string GetPartnerSifrar_OrderByColumns(VvSQL.SorterType sorterType)
   {
      string orderColumns = "Dokum ORDER_BAJ Very BAD! in GetPartnerSifrar_OrderByColumns()";

      switch(sorterType)
      {
         case VvSQL.SorterType.Name  : orderColumns = " naziv"  ; break;
         case VvSQL.SorterType.Code/*RecID*/ : orderColumns = /*" prjktKupdobCD"*/"kupdobCD"; break;
         case VvSQL.SorterType.Ticker: orderColumns = " ticker" ; break;
         case VvSQL.SorterType.City  : orderColumns = " grad"   ; break;
         case VvSQL.SorterType.OIB   : orderColumns = " oib"    ; break;
         case VvSQL.SorterType.Person: orderColumns = " prezime"; break;
      }
      return orderColumns;
   }

   #endregion GetPartnerSifrar_OrderByColumns

}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptK_GeneralList : VvKupdobReport
{
   private CR_GeneralList cr_list  = new CR_GeneralList();
   private DS_Kupdob_AllC dsReport = new DS_Kupdob_AllC();

   public override ReportDocument VirtualReportDocument { get { return cr_list;  } }
   public override DataSet        VirtualUntypedDataSet { get { return dsReport; } }

   public RptK_GeneralList(string reportName, VvRpt_Kupdob_Filter rptFilter) : base(reportName, rptFilter)
   {
      
   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      return GetKupdob_Command(conn, dsReport.IzvjTable, false);
   }
}

public class RptK_PartnerKontakti : RptK_GeneralList
{
   private CR_PartnerKontakti cr_list = new CR_PartnerKontakti();
 //private CR_AdreseProba cr_list = new CR_AdreseProba();

   public override ReportDocument VirtualReportDocument { get { return cr_list; } }

   public RptK_PartnerKontakti(string reportName, VvRpt_Kupdob_Filter rptFilter): base(reportName, rptFilter)
   {

   }
}

public class RptK_PartnerFaktur   : RptK_GeneralList
{
   private CR_PartnerFaktur cr_list = new CR_PartnerFaktur();

   public override ReportDocument VirtualReportDocument { get { return cr_list; } }

   public RptK_PartnerFaktur(string reportName, VvRpt_Kupdob_Filter rptFilter) : base(reportName, rptFilter)
   {

   }
}

public class RptK_PartnerGrupTip : RptK_GeneralList
{
   private CR_PartnerGrupTip cr_list = new CR_PartnerGrupTip();

   public override ReportDocument VirtualReportDocument { get { return cr_list; } }

   public RptK_PartnerGrupTip(string reportName, VvRpt_Kupdob_Filter rptFilter) : base(reportName, rptFilter)
   {

   }
}

public class RptK_PartnerRabat    : RptK_GeneralList
{
   private CR_PartnerRabat cr_list = new CR_PartnerRabat();
 //private CR_AdreseProba cr_list = new CR_AdreseProba();

   public override ReportDocument VirtualReportDocument { get { return cr_list; } }

   public RptK_PartnerRabat(string reportName, VvRpt_Kupdob_Filter rptFilter): base(reportName, rptFilter)
   {

   }
}

public class RptK_PartnerZiro     : RptK_GeneralList
{
   private CR_PartnerZiro cr_list = new CR_PartnerZiro();

   public override ReportDocument VirtualReportDocument { get { return cr_list; } }

   public RptK_PartnerZiro(string reportName, VvRpt_Kupdob_Filter rptFilter): base(reportName, rptFilter)
   {

   }
}

public class RptK_PartnerSifre    : RptK_GeneralList
{
   private CR_PartnerSifre cr_list = new CR_PartnerSifre();

   public override ReportDocument VirtualReportDocument { get { return cr_list; } }

   public RptK_PartnerSifre(string reportName, VvRpt_Kupdob_Filter rptFilter) : base(reportName, rptFilter)
   {

   }
}

public class RptK_PartnerZaExport    : RptK_GeneralList
{
   private CR_PartnerZaExport cr_list = new CR_PartnerZaExport();

   public override ReportDocument VirtualReportDocument { get { return cr_list; } }

   public RptK_PartnerZaExport(string reportName, VvRpt_Kupdob_Filter rptFilter) : base(reportName, rptFilter)
   {

   }
}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptK_VizitKarta : VvKupdobReport
{
   private CR_VizitKartaKD cr_vizitka = new CR_VizitKartaKD();
   private DS_Kupdob_AllC dsReport    = new DS_Kupdob_AllC();

   public override ReportDocument VirtualReportDocument { get { return cr_vizitka;  } }
   public override DataSet        VirtualUntypedDataSet { get { return dsReport; } }

   public RptK_VizitKarta(string reportName, VvRpt_Kupdob_Filter rptFilter) : base(reportName, rptFilter)
   {
      
   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      return GetKupdob_Command(conn, dsReport.IzvjTable, false);
   }
}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptK_VizitKaProsireno : VvKupdobReport
{
   private CR_VizitkaProsirena cr_vizitka = new CR_VizitkaProsirena();
   private DS_Kupdob_AllC        dsReport = new DS_Kupdob_AllC();

   public override ReportDocument VirtualReportDocument { get { return cr_vizitka; } }
   public override DataSet        VirtualUntypedDataSet { get { return dsReport; } }

   public RptK_VizitKaProsireno(string reportName, VvRpt_Kupdob_Filter rptFilter)
      : base(reportName, rptFilter)
   {

   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      return GetKupdob_Command(conn, dsReport.IzvjTable, false);
   }
}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 




//   
//                                                                                                                                    
//   PRJKT Reports:                                                                                                                   
//                                                                                                                                    
//   



public class RptPrj_GeneralList : VvKupdobReport
{
   private CR_ListaPrjkta cr_list  = new CR_ListaPrjkta();
   private DS_Prjkt_AllC  dsReport = new DS_Prjkt_AllC();

   public override ReportDocument VirtualReportDocument { get { return cr_list; } }
   public override DataSet        VirtualUntypedDataSet { get { return dsReport; } }

   public RptPrj_GeneralList(string reportName, VvRpt_Kupdob_Filter rptFilter) : base(reportName, rptFilter)
   {

   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      return GetKupdob_Command(conn, dsReport.IzvjTable, true);
   }
}
