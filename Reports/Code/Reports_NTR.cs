using System;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using Vektor.Reports.FIZ;
using System.Linq;
using System.Collections.Generic;
using Vektor.DataLayer.DS_Reports;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
#endif


//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptNTR_BilancaMP : VvFinNtrReport
{
   private CR_NTR_BilancaMP cr_NTR_BilancaMP  = new CR_NTR_BilancaMP();

   public override ReportDocument VirtualReportDocument { get { return cr_NTR_BilancaMP; } }

   public RptNTR_BilancaMP(string reportName, VvRpt_Fin_Filter rptFilter) : base(reportName, rptFilter)
   {
      base.FillFormulaFields(ZXC.luiListaNTR_BilancaMP, cr_NTR_BilancaMP);
   }
}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptNTR_RDiG : VvFinNtrReport
{
   private CR_NTR_RDiG  cr_NTR_RDiG = new CR_NTR_RDiG();

   public override ReportDocument VirtualReportDocument { get { return cr_NTR_RDiG; } }

   public RptNTR_RDiG(string reportName, VvRpt_Fin_Filter rptFilter) : base(reportName, rptFilter)
   {
      base.FillFormulaFields(ZXC.luiListaNTR_RDiGMP, cr_NTR_RDiG);
   }
}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptNTR_ObrPD : VvFinNtrReport
{
   private CR_NTR_ObrPD cr_NTR_ObrPD = new CR_NTR_ObrPD();

   public override ReportDocument VirtualReportDocument { get { return cr_NTR_ObrPD; } }

   public RptNTR_ObrPD(string reportName, VvRpt_Fin_Filter rptFilter) : base(reportName, rptFilter)
   {
      base.FillFormulaFields(ZXC.luiListaNTR_ObrPD, cr_NTR_ObrPD);
   }
}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptNTR_TSIPOD : VvFinNtrReport
{
   private CR_TSIpod cr_NTR_tsiPod = new CR_TSIpod();

   public override ReportDocument VirtualReportDocument { get { return cr_NTR_tsiPod; } }

   public RptNTR_TSIPOD(string reportName, VvRpt_Fin_Filter rptFilter) : base(reportName, rptFilter)
   {
      base.FillFormulaFields(ZXC.luiListaNTR_TSIPod, cr_NTR_tsiPod);
   }
}


//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptNTR_PPI : RptF_KPI
{
   private CR_PPI cr_NTR_PPI = new CR_PPI();

   public override ReportDocument VirtualReportDocument { get { return cr_NTR_PPI; } }
   //public override DataSet        VirtualUntypedDataSet { get { return null; } }

   protected override bool DisIz_PPI { get { return true;} }

   public RptNTR_PPI(string reportName, VvRpt_Fin_Filter rptFilter): base(reportName, rptFilter)
   {
      ppiRed = new decimal[30];
   }

   public override bool PerformAdditionalDataSetOperation()
   {
      bool ret;

      ret = base.PerformAdditionalDataSetOperation();
      
      this.FillFormulaFields();

      return ret;
   }

   private void FillFormulaFields()
   {
      FormulaFieldDefinitions fofs = VirtualReportDocument.DataDefinition.FormulaFields;

      int    idx = 0;
      string fofName;

      foreach(decimal redValue in ppiRed)
      {
         if(redValue.NotZero())
         {
            fofName = idx.ToString("00") + ".";

            fofs[fofName].Text = redValue.ToString(ZXC.VvNumberFormatInfo2_ForceDot);
         }
         idx++;

      }
   }
}

public class RptNTR_PPI2 : RptF_KPI
{
   private CR_Obrazac_P_PPI cr_NTR_PPI = new CR_Obrazac_P_PPI();

   public override ReportDocument VirtualReportDocument { get { return cr_NTR_PPI; } }
 //public override DataSet        VirtualUntypedDataSet { get { return null; } }

   protected override bool DisIz_PPI2 { get { return true; } }

   public RptNTR_PPI2(string reportName, VvRpt_Fin_Filter rptFilter): base(reportName, rptFilter)
   {
      ppiRed = new decimal[30];
   }

   public override bool PerformAdditionalDataSetOperation()
   {
      bool ret;

      ret = base.PerformAdditionalDataSetOperation();

      this.FillFormulaFields();

      return ret;
   }

   private void FillFormulaFields()
   {
      FormulaFieldDefinitions fofs = VirtualReportDocument.DataDefinition.FormulaFields;

      int idx = 0;
      string fofName;

      foreach(decimal redValue in ppiRed)
      {
         if(redValue.NotZero())
         {
            fofName = idx.ToString("00") + ".";

            fofs[fofName].Text = redValue.ToString(ZXC.VvNumberFormatInfo2_ForceDot);
         }
         idx++;

      }
   }
}

public class RptF_KPI : VvFinReport
{
   private CR_KPI cr_KPI = new CR_KPI();
   private DS_KPI ds_KPI = new DS_KPI();

   public override ReportDocument VirtualReportDocument { get { return cr_KPI; } }
   public override DataSet        VirtualUntypedDataSet { get { return ds_KPI; } }

   private IEnumerable<string> kplanDistinctDUGlist;
   private IEnumerable<string> kplanDistinctPOTlist;

   // 22.02.2024: 
 //private VvLookUpLista       kpiLookUpLista =                                                                                         ZXC.luiListaNTR_KPI; 
   private VvLookUpLista       kpiLookUpLista = (ZXC.KSD.Dsc_IsKPI24 && ZXC .projectYearAsInt >= 2024) ? ZXC.luiListaNTR_KPI24 : ZXC.luiListaNTR_KPI;

   private VvLookUpLista       ppiLookUpLista = ZXC.luiListaNTR_PPI;
   private VvLookUpLista       ppi2LookUpLista= ZXC.luiListaNTR_PPI2;
   protected decimal[]         ppiRed;

   protected virtual bool DisIz_PPI       { get { return false; } }
   protected virtual bool DisIz_PPI2      { get { return false; } }
   protected virtual bool DisIz_KnjigaURA { get { return false; } }

   public RptF_KPI(string reportName, VvRpt_Fin_Filter rptFilter) : base(reportName, rptFilter)
   {
      List<Kplan>         kplanCompleteDatabaseList;

      FilterStyle = ZXC.FIZ_FilterStyle.Ftrans;

      switch(rptFilter.SorterType_Dokument)
      {
         case VvSQL.SorterType.DokDate: rptFilter.OrderBy = VvSQL.RptOrderBy.FIZ_Dnevnik_DokDate; break;
         case VvSQL.SorterType.DokNum:  rptFilter.OrderBy = VvSQL.RptOrderBy.FIZ_Dnevnik_DokNum;  break;
      }

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kplan>(null, VvSQL.SorterType.None);

      kpiLookUpLista.LazyLoad();

      kplanCompleteDatabaseList = VvUserControl.KplanSifrar;

      if(DisIz_PPI)
      {
         ppiLookUpLista.LazyLoad();

         kplanDistinctDUGlist = GetKplanDistinctInUseList(kplanCompleteDatabaseList, kpiLookUpLista, ppiLookUpLista, ZXC.SaldoOrDugOrPot.DUG);
         kplanDistinctPOTlist = GetKplanDistinctInUseList(kplanCompleteDatabaseList, kpiLookUpLista, ppiLookUpLista, ZXC.SaldoOrDugOrPot.POT);
      }
    // 22.02.2016.
    //if(DisIz_PPI2)
      else
      {
         ppi2LookUpLista.LazyLoad();

         kplanDistinctDUGlist = GetKplanDistinctInUseList(kplanCompleteDatabaseList, kpiLookUpLista, ppi2LookUpLista, ZXC.SaldoOrDugOrPot.DUG);
         kplanDistinctPOTlist = GetKplanDistinctInUseList(kplanCompleteDatabaseList, kpiLookUpLista, ppi2LookUpLista, ZXC.SaldoOrDugOrPot.POT);
      }

   }

   private IEnumerable<string> GetKplanDistinctInUseList(List<Kplan> kplanCompleteDatabaseList, VvLookUpLista kpiLookUpLista, VvLookUpLista ppiLookUpLista, ZXC.SaldoOrDugOrPot _DOP)
   {
      List<string> nonDistinctList = new List<string>();
      string       luiKonto;
      ZXC.SaldoOrDugOrPot luiDOP;

      foreach(VvLookUpItem lui in kpiLookUpLista)
      {
         luiKonto = lui.Name;
         luiDOP   = lui.Integer == 1 ? ZXC.SaldoOrDugOrPot.DUG : lui.Integer == 2 ? ZXC.SaldoOrDugOrPot.POT : ZXC.SaldoOrDugOrPot.NULL;

         if(luiDOP == _DOP) // spada li lui u ciljani DUG/POT set? 
         {
            if(lui.Flag) // korijen 
            {
               var konta = kplanCompleteDatabaseList.Where(kplan => kplan.Konto.StartsWith(luiKonto) && kplan.Tip == "A");

               nonDistinctList = nonDistinctList.Concat((konta.Select(kplan => kplan.Konto).ToList())).ToList();
            }
            else // explicitni konto 
            {
               nonDistinctList.Add(luiKonto);
            }
         }
      }

      #region Run agejn for PPI

      if(DisIz_PPI)
      {
         foreach(VvLookUpItem lui in ppiLookUpLista)
         {
            luiKonto = lui.Name;
            luiDOP   = lui.Integer == 1 ? ZXC.SaldoOrDugOrPot.DUG : lui.Integer == 2 ? ZXC.SaldoOrDugOrPot.POT : ZXC.SaldoOrDugOrPot.NULL;

            if(luiDOP == _DOP) // spada li lui u ciljani DUG/POT set? 
            {
               if(lui.Flag) // korijen 
               {
                  var konta = kplanCompleteDatabaseList.Where(kplan => kplan.Konto.StartsWith(luiKonto) && kplan.Tip == "A");

                  nonDistinctList = nonDistinctList.Concat((konta.Select(kplan => kplan.Konto).ToList())).ToList();
               }
               else // explicitni konto 
               {
                  nonDistinctList.Add(luiKonto);
               }
            }
         }
      }

      if(DisIz_PPI2)
      {
         foreach(VvLookUpItem lui in ppi2LookUpLista)
         {
            luiKonto = lui.Name;
            luiDOP   = lui.Integer == 1 ? ZXC.SaldoOrDugOrPot.DUG : lui.Integer == 2 ? ZXC.SaldoOrDugOrPot.POT : ZXC.SaldoOrDugOrPot.NULL;

            if(luiDOP == _DOP) // spada li lui u ciljani DUG/POT set? 
            {
               if(lui.Flag) // korijen 
               {
                  var konta = kplanCompleteDatabaseList.Where(kplan => kplan.Konto.StartsWith(luiKonto) && kplan.Tip == "A");

                  nonDistinctList = nonDistinctList.Concat((konta.Select(kplan => kplan.Konto).ToList())).ToList();
               }
               else // explicitni konto 
               {
                  nonDistinctList.Add(luiKonto);
               }
            }
         }
      }

      #endregion Run agejn for PPI

      return nonDistinctList.Distinct();
   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      //return GetAtransWithStatusData_Command(conn, "t_dokNum, t_dokDate, t_konto, t_tipBr, t_opis, t_tt, t_pdv, t_037, t_dug, t_pot "); 

      XSqlCommand cmd = VvSQL.InitCommand(conn);

      cmd.CommandText =

         // 30.10.2023:
       //"SELECT        t_dokNum, t_dokDate, t_konto, t_tipBr, t_opis, t_tt, t_pdv, t_037, t_dug, t_pot \n" +
         "SELECT ttNum, t_dokNum, t_dokDate, t_konto, t_tipBr, t_opis, t_tt, t_pdv, t_037, t_dug, t_pot \n" +
         "FROM " + Ftrans.recordName + "\n" +

         // 30.10.2023:
         (this is RptF_KPI || this is RptF_KPI_orig ? "RIGHT JOIN nalog n ON n.recID = t_parentID" : "") + "\n" +

         VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(RptFilter) +
         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(RptFilter.FilterMembers, false) +

         // rozDragica: skip PS 
         "AND t_tt != '" + Nalog.PS_TT + "'\n" +

         "AND (\n" +
         "(t_konto IN " + VvSQL.GetInSetClause(kplanDistinctDUGlist) + " && t_dug != 0.00) || \n" +
         "(t_konto IN " + VvSQL.GetInSetClause(kplanDistinctPOTlist) + " && t_pot != 0.00))\n" + 

         OrderClauseFromFilter();

      VvSQL.SetReportCommandParamValues(cmd, this.RptFilter.FilterMembers);

      return cmd;
   }

   private VvLookUpItem GetLuiForThisFtrans(string currKonto, List<string> reportedDuplicates, VvLookUpLista lookUpLista)
   {
      VvLookUpItem foundLui  = null;

      try
      {
         foundLui = lookUpLista.SingleOrDefault(lui => (lui.Name == currKonto || (lui.Flag && currKonto.StartsWith(lui.Name))));

         //if(foundLui != null && foundLui.Cd.EndsWith(".")) foundLui.Cd = foundLui.Cd.Remove(foundLui.Cd.Length-1, 1);
      }
      catch(System.InvalidOperationException)
      {
         if(reportedDuplicates.Contains(currKonto) == false)
         {
            VvLookUpItem[] invalidLuis = lookUpLista.Where(lui => (lui.Name == currKonto || (lui.Flag && currKonto.StartsWith(lui.Name)))).ToArray();

            System.Text.StringBuilder duplicates = new System.Text.StringBuilder("\n\n");

            duplicates.Append("Poz Konto Korijen\n\n");
            foreach(VvLookUpItem dupLui in invalidLuis)
            {
               duplicates.Append(string.Format("{0,2}   {1,-6} {2} \n", dupLui.Cd, dupLui.Name, dupLui.Flag.ToString()));
            }

            VvSQL.ReportGenericError("Pozicioniranje konta " + currKonto, 
               string.Format("Pravilo za konto {0} se pojavljuje više od jednom: {1}", currKonto, duplicates.ToString()), 
               System.Windows.Forms.MessageBoxButtons.OK);

            reportedDuplicates.Add(currKonto);
         }
      }

      return foundLui;
   }

   public override bool PerformAdditionalDataSetOperation()
   {
      VvLookUpItem kpiLui, ppiLui;
      List<string> reportedDuplicates_KPI = new List<string>();
      List<string> reportedDuplicates_PPI = new List<string>();

      DS_KPI.IzvjTableDataTable reportTable = ds_KPI.IzvjTable;

      foreach(DS_KPI.IzvjTableRow dataRow in reportTable.Rows)
      {
         kpiLui = GetLuiForThisFtrans(dataRow.t_konto, reportedDuplicates_KPI, kpiLookUpLista);
         
         if(DisIz_PPI) 
            ppiLui = GetLuiForThisFtrans(dataRow.t_konto, reportedDuplicates_PPI, ppiLookUpLista);
         else if(DisIz_PPI2) 
            ppiLui = GetLuiForThisFtrans(dataRow.t_konto, reportedDuplicates_PPI, ppi2LookUpLista);
         else
            ppiLui = null;

         if(kpiLui == null && ppiLui == null) continue; // curr t_konto + (t_dug/t_pot) isn't interesting for this report 

         #region Checks 

         if(kpiLui != null)
         {
            if(ZXC.ValOrZero_Int_wDot(kpiLui.Cd) < 5 || ZXC.ValOrZero_Int_wDot(kpiLui.Cd) > 16)
            {
               VvSQL.ReportGenericError("KPI Pozicioniranje konta " + kpiLui.Name,
                  string.Format("Pozicija {0} je besmislena.", kpiLui.Cd),
                  System.Windows.Forms.MessageBoxButtons.OK);

               continue;
            }

            if(kpiLui.Integer != 1 && kpiLui.Integer != 2)
            {
               VvSQL.ReportGenericError("Pozicioniranje konta " + kpiLui.Name,
                  string.Format("SDP oznaka {0} nije niti 'DUG' niti 'POT'.", kpiLui.Integer),
                  System.Windows.Forms.MessageBoxButtons.OK);

               continue;
            }
         }

         if(ppiLui != null)
         {
            if(ZXC.ValOrZero_Int_wDot(ppiLui.Cd) < 1 || ZXC.ValOrZero_Int_wDot(ppiLui.Cd) > 21)
            {
               VvSQL.ReportGenericError("PPI Pozicioniranje konta " + ppiLui.Name,
                  string.Format("Pozicija {0} je besmislena.", ppiLui.Cd),
                  System.Windows.Forms.MessageBoxButtons.OK);

               continue;
            }

            if(ppiLui.Integer != 1 && ppiLui.Integer != 2)
            {
               VvSQL.ReportGenericError("Pozicioniranje konta " + ppiLui.Name,
                  string.Format("SDP oznaka {0} nije niti 'DUG' niti 'POT'.", ppiLui.Integer),
                  System.Windows.Forms.MessageBoxButtons.OK);

               continue;
            }
         }

         #endregion Checks

         Calc_KPI(dataRow, kpiLui, ppiLui);
      }

      return true;
   }

   private void Calc_KPI(DS_KPI.IzvjTableRow dataRow, VvLookUpItem kpiLui, VvLookUpItem ppiLui)
   {
      #region KPI rules

      decimal money = 0.00M, pr_stopa = 0.00M, pdv = 0.00M, devetka = 0.00M, petnaj = 0.00M, cetrnaj = 0.00M;
      int     idx   = -1;
      ZXC.SaldoOrDugOrPot luiDOP;

      bool isKPIbef2014 = dataRow.t_dokDate < ZXC.Date01012014;
      bool isKPIbef2017 = dataRow.t_dokDate < ZXC.Date01012017;

      if(kpiLui != null)
      {
         // 30.10.2023:
         bool trebamoLiTtNum = true /*dataRow.t_tt == "IZ"*/; // todo ... sad je uvijek byQ 
         if(trebamoLiTtNum)
         {
            dataRow.t_tt += ("-" + dataRow.ttNum.ToString());
         }

         idx = ZXC.ValOrZero_Int_wDot(kpiLui.Cd);
         luiDOP = kpiLui.Integer == 1 ? ZXC.SaldoOrDugOrPot.DUG : kpiLui.Integer == 2 ? ZXC.SaldoOrDugOrPot.POT : ZXC.SaldoOrDugOrPot.NULL;

         if(luiDOP == ZXC.SaldoOrDugOrPot.DUG) money = dataRow.t_dug;
         if(luiDOP == ZXC.SaldoOrDugOrPot.POT) money = dataRow.t_pot;

         dataRow["t_" + idx.ToString("00")] = money;

         if(dataRow.t_pdv == "N" || ZXC.CURR_prjkt_rec.NOT_IN_PDV == true )
         {
            pr_stopa = 0.00M;
         }
         else if(dataRow.t_pdv == "T")  // PdvSt 10% ili 13%
         {
            if(isKPIbef2014) pr_stopa = 0.0909090909090909090909090909090909090909090909090909090909090M; // 10% do 31.12.2013.
            else             pr_stopa = 0.115044247787611M;                                               // 13% od 01.01.2014. 
         }
         else if(dataRow.t_pdv == "D")  // PdvSt 10% 19.02.2014. treba za zaostatke od pg
         {
            pr_stopa = 0.0909090909090909090909090909090909090909090909090909090909090M; // 10% do 31.12.2013.
         }
         else if(dataRow.t_pdv == "2")
         {
            pr_stopa = 0.180327868M;     // PdvSt 22%
            
         }
         else if(dataRow.t_pdv == "3")
         {
            pr_stopa = 0.18699186991M; // PdvSt 23%
         }
         else if(dataRow.t_pdv == "5")
         {
            pr_stopa = 0.04761904761M; // PdvSt 5% od 01.01.2013.
         }

         else
         {
            if(dataRow.t_dokDate < Faktur.NewPdvStopaDate) pr_stopa = 0.18699186991M; // PdvSt 23% do 29.02.2012.
            else                                           pr_stopa = 0.2M;           // PdvSt 25% od 01.03.2012.
         } 

         pdv = money * pr_stopa;

         if(idx >= 5 && idx <= 7)
         {
            devetka = money - pdv;

            dataRow.t_08 = pdv;
            dataRow.t_09 = devetka;
         }
       // 15.02.2017. ide na 50/50 od 01.01.2017.
       //else if(idx >= 10 && idx <= 12)
         else if(idx >= 10 && idx <= 12 && isKPIbef2017)
         {
            cetrnaj = 0.00M;
            petnaj  = money - pdv;

            if(dataRow.t_037 == "0")
            {
               cetrnaj = money;
               petnaj  = 0.00M;
               pdv     = 0.00M;
            }
            else if(dataRow.t_037 == "7")
            {
               cetrnaj = 0.30M * money;
               petnaj  = 0.70M * petnaj;
               pdv     = 0.70M * pdv;
            }
            else if(dataRow.t_037 == "3")
            {
               cetrnaj = 0.70M * money;
               petnaj  = 0.30M * petnaj;
               pdv     = 0.30M * pdv;
            }
            dataRow.t_13 = pdv;
            dataRow.t_14 = cetrnaj;
            dataRow.t_15 = petnaj;
         } //else if(idx >= 10 && idx <= 12)
         else if(idx >= 10 && idx <= 12 && isKPIbef2017 == false)
         {
            cetrnaj = 0.00M;
            petnaj = money - pdv;

            if(dataRow.t_037 == "0")
            {
               cetrnaj = money;
               petnaj  = 0.00M;
               pdv     = 0.00M;
            }
            else if(dataRow.t_037 == "7")
            {
               cetrnaj = 0.30M * money;
               petnaj  = 0.70M * petnaj;
               pdv     = 0.70M * pdv;
            }
            else if(dataRow.t_037 == "3")
            {
               cetrnaj = 0.70M * money;
               petnaj  = 0.30M * petnaj;
               pdv     = 0.30M * pdv;
            }
            else if(dataRow.t_037 == "5")
            {
               cetrnaj = 0.50M * money;
               petnaj  = 0.50M * petnaj;
               pdv     = 0.50M * pdv;
            }
            dataRow.t_13 = pdv;
            dataRow.t_14 = cetrnaj;
            dataRow.t_15 = petnaj;
         } //else if(idx >= 10 && idx <= 12)

      }
      #endregion KPI rules

      #region PPI rules

      #region PPI Primjer
      //╒═════════════════════════════════════════════════════════════════════════════╕
      //│ R. br.               PRIMICI i IZDACI                                IZNOS  │
      //╞═════════════════════════════════════════════════════════════════════════════╡
      //│   1. Primici u gotovini i cekovima                                    0.00  │
      //│   2. Primici na ziro racun                                       261787.57  │
      //│   3. Ostali primici (u naravi, protuuslugama i reader.)                   0.00  │
      //│   4. Primici ostvareni izuzimanjem imovine                            0.00  │
      //│   5. PDV u primicima                                              46851.90  │
      //╞═════════════════════════════════════════════════════════════════════════════╡
      //│   6. UKUPNI PRIMICI                                              214935.67  │
      //╞═════════════════════════════════════════════════════════════════════════════╡
      //│   7. Izdaci u gotovini                                            40744.66  │
      //│   8. Izdaci putem ziro racuna                                    114137.81  │
      //│   9. Izdaci u naravi                                                  0.00  │
      //│  10. Izdaci iz cl. 18. st. 5. i cl. 19. Zakona o por. na doh.     11372.87  │
      //│  11. Izdaci otpisa / amortizacije                                     0.00  │
      //│  12. Izdaci reprezentacije                                            0.00  │
      //│  13. Izdaci premije osiguranja                                        0.00  │
      //│  14. Izdaci u svezi s otudenjem                                       0.00  │
      //│  15. PDV u izdacima                                                8319.50  │
      //╞═════════════════════════════════════════════════════════════════════════════╡
      //│  16. UKUPNI IZDACI                                               135190.09  │
      //╞═════════════════════════════════════════════════════════════════════════════╡
      //│  17.  D O H O D A K  ILI  G U B I T A K                           79745.58  │
      //╞═════════════════════════════════════════════════════════════════════════════╡
      //                                                                               
      //╞═════════════════════════════════════════════════════════════════════════════╡
      //│   1. Iznos nenaplacenih racuna od kupaca za 2007 g.                   0.00  │
      //│   2. Iznos neplacenih racuna dobavljacima za 2007 g.                  0.00  │
      //│   3. Ulaganje vlastitih novcanih sredstava na ziro racun              0.00  │
      //│   4. Vrijednost nabavljene dugotrajne imovine u 2007 g.               0.00  │
      //╞═════════════════════════════════════════════════════════════════════════════╡
      #endregion PPI Primjer

      if(DisIz_PPI)
      {
         ppiRed[ 5] += dataRow.t_08;
         ppiRed[10] += dataRow.t_14;
         ppiRed[15] += dataRow.t_13;

         if(ppiLui != null) // PPI overriding KPI rules
         {
            int red;
            ZXC.SaldoOrDugOrPot ppiLuiDOP;

            red = ZXC.ValOrZero_Int_wDot(ppiLui.Cd);
            ppiLuiDOP = ppiLui.Integer == 1 ? ZXC.SaldoOrDugOrPot.DUG : ppiLui.Integer == 2 ? ZXC.SaldoOrDugOrPot.POT : ZXC.SaldoOrDugOrPot.NULL;

            if(ppiLuiDOP == ZXC.SaldoOrDugOrPot.DUG) money = dataRow.t_dug;
            if(ppiLuiDOP == ZXC.SaldoOrDugOrPot.POT) money = dataRow.t_pot;

            ppiRed[red] += money;
         }
         else
         {
            switch(idx)
            {

               case  5: ppiRed[1] += money; break;
               case  6: ppiRed[2] += money; break;
               case 10: ppiRed[7] += money; break;
               case 11: ppiRed[8] += money; break;
               case 12: ppiRed[9] += money; break;
            }
         }
      }

      else if(DisIz_PPI2)
      {
         ppiRed[ 6] += dataRow.t_08;
         ppiRed[14] += dataRow.t_14;
         ppiRed[13] += dataRow.t_13;

         if(ppiLui != null) // PPI overriding KPI rules
         {
            int red;
            ZXC.SaldoOrDugOrPot ppiLuiDOP;

            red = ZXC.ValOrZero_Int_wDot(ppiLui.Cd);
            ppiLuiDOP = ppiLui.Integer == 1 ? ZXC.SaldoOrDugOrPot.DUG : ppiLui.Integer == 2 ? ZXC.SaldoOrDugOrPot.POT : ZXC.SaldoOrDugOrPot.NULL;

            if(ppiLuiDOP == ZXC.SaldoOrDugOrPot.DUG) money = dataRow.t_dug;
            if(ppiLuiDOP == ZXC.SaldoOrDugOrPot.POT) money = dataRow.t_pot;

            ppiRed[red] += money;
         }
         else
         {
            switch(idx)
            {
             // 06.02.2017. kazu kod Bobesicke da ovo nije isto i da treba razdvojiti red 5 i red 16,                
             // ali posto ne znamo zasto smo tako postavili nekome mozda pase pa cemo ostaviti defaultno kako je bilo
             // a na filteru odabir bez sumiranja redka 1 u redak 16                                                 
             //case  5: ppiRed[1] += money;                                       ppiRed[16] += money; break;  
               case  5: ppiRed[1] += money; if(this.RptFilter.IsPIPonly16 == false) ppiRed[16] += money; break;
               case  6: ppiRed[2] += money;                                                              break;
               case  7: ppiRed[3] += money;                                                              break;
               case 10: ppiRed[7] += money;                                                              break;
               case 11: ppiRed[8] += money;                                                              break;
               case 12: ppiRed[9] += money;                                                              break;
            }
         }
      }

      #endregion PPI rules

      #region KnjigaURA

      decimal osn0    = 0.00M, prolSt = 0.00M, pdvUk  = 0.00M;
      decimal ukOsn10 = 0.00M, pdv10m = 0.00M, pdv10n = 0.00M;
      decimal ukOsn22 = 0.00M, pdv22m = 0.00M, pdv22n = 0.00M;
      decimal ukOsn23 = 0.00M, pdv23m = 0.00M, pdv23n = 0.00M;

      if(DisIz_KnjigaURA)
      {
            dataRow.t_06 = 0.00M;
            dataRow.t_07 = 0.00M;
            dataRow.t_08 = 0.00M;
            dataRow.t_09 = 0.00M;
            dataRow.t_10 = 0.00M;
            dataRow.t_11 = 0.00M;
            dataRow.t_12 = 0.00M;
            dataRow.t_13 = 0.00M;
            dataRow.t_14 = 0.00M;
            dataRow.t_15 = 0.00M;
            dataRow.t_16 = 0.00M;
            dataRow.t_01 = 0.00M;

            if(idx >= 10 && idx <= 12)
            {
               pdvUk = pdv;
               if(dataRow.t_pdv == "P")
               {
                  pr_stopa = 0.00M;
                  prolSt = money;
                  pdv = 0.00M;
               }
               else if(dataRow.t_pdv == "T")
               {
                  ukOsn10 = money - pdv;
               }
               else if(dataRow.t_pdv == "2")
               {
                  ukOsn22 = money - pdv;
               }
               else if(dataRow.t_pdv == "")
               {
                  ukOsn23 = money - pdv;
               }


               if(dataRow.t_037 == "0")
               {
                  if(dataRow.t_pdv == "T")
                  {
                     pdv10n = pdv;
                  }
                  else if(dataRow.t_pdv == "2")
                  {
                     pdv22n = pdv;
                  }
                  else if(dataRow.t_pdv == "")
                  {
                     pdv23n = pdv;
                  }
               }
               else if(dataRow.t_037 == "7")
               {
                  if(dataRow.t_pdv == "T")
                  {
                     pdv10m = 0.70M * pdv;
                     pdv10n = pdv - pdv10m;
                  }
                  else if(dataRow.t_pdv == "2")
                  {
                     pdv22m = 0.70M * pdv;
                     pdv22n = pdv - pdv22m;
                  }
                  else if(dataRow.t_pdv == "")
                  {
                     pdv23m = 0.70M * pdv;
                     pdv23n = pdv - pdv23m;
                  }
               }
               else if(dataRow.t_037 == "3")
               {
                  if(dataRow.t_pdv == "T")
                  {
                     pdv10m = 0.30M * pdv;
                     pdv10n = pdv - pdv10m;
                  }
                  else if(dataRow.t_pdv == "2")
                  {
                     pdv22m = 0.30M * pdv;
                     pdv22n = pdv - pdv22m;
                  }
                  else if(dataRow.t_pdv == "")
                  {
                     pdv23m = 0.30M * pdv;
                     pdv23n = pdv - pdv23m;
                  }
               }
               else if(dataRow.t_037 == "")
               {
                  if(dataRow.t_pdv == "T")
                  {
                     pdv10m = pdv;

                  }
                  else if(dataRow.t_pdv == "2")
                  {
                     pdv22m = pdv;
                  }
                  else if(dataRow.t_pdv == "")
                  {
                     pdv23m = pdv;
                  }
               }

               dataRow.t_06 = osn0;
               dataRow.t_07 = ukOsn10;
               dataRow.t_08 = ukOsn22;
               dataRow.t_09 = ukOsn23;
               dataRow.t_10 = money - prolSt;
               dataRow.t_11 = pdvUk;
               dataRow.t_12 = pdv10m;
               dataRow.t_13 = pdv10n;
               dataRow.t_14 = pdv22m;
               dataRow.t_15 = pdv22n;
               dataRow.t_16 = pdv23m;
               dataRow.t_01 = pdv23n;

            }
            else
            {
            }


      }

      #endregion KnjigaURA

   }
}

public class RptF_KPI_orig : RptF_KPI
{
   private CR_KPI_orig cr_KPI_orig = new CR_KPI_orig();

   public override ReportDocument VirtualReportDocument { get { return cr_KPI_orig; } }

   public RptF_KPI_orig(string reportName, VvRpt_Fin_Filter rptFilter) : base(reportName, rptFilter)
   {
     
   }

}

public class RptF_KnjigaURA : RptF_KPI
{
   private CR_FinKnjigaURAobrt cr_URA = new CR_FinKnjigaURAobrt();

   public override ReportDocument VirtualReportDocument { get { return cr_URA; } }

   protected override bool DisIz_KnjigaURA { get { return true; } }

   public RptF_KnjigaURA(string reportName, VvRpt_Fin_Filter rptFilter): base(reportName, rptFilter)
   {
     
   }

}

