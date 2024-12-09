using System;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using Vektor.Reports.PIZ;
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
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Vektor;
// SEPA_PAIN_001_001_03_to_PAIN_001_001_09
//using PAIN_001_001_03;
using PAIN_001_001_09;
#endif

// 17.02.2011: Za Report-e, daj Distinct-ivne Ptrans-e po t_personCd-u
// Custom comparer for the Ptrans class
public class PtransByPersonCdComparer : IEqualityComparer<DS_Placa.IzvjTableRow>
{
   // Ptranss are equal if their names and Ptrans numbers are equal.
   public bool Equals(DS_Placa.IzvjTableRow x, DS_Placa.IzvjTableRow y)
   {

      //Check whether the compared objects reference the same data.
      if(Object.ReferenceEquals(x, y)) return true;

      //Check whether any of the compared objects is null.
      if(Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
         return false;

      //Check whether the Ptrans' properties are equal.
      return x.t_personCD == y.t_personCD;
   }

   // If Equals() returns true for a pair of objects 
   // then GetHashCode() must return the same value for these objects.

   public int GetHashCode(DS_Placa.IzvjTableRow Ptrans)
   {
      //Check whether the object is null
      if(Object.ReferenceEquals(Ptrans, null)) return 0;

      //Get hash code for the Code field.
      int hashPtransCode = Ptrans.t_personCD.GetHashCode();

      //Calculate the hash code for the Ptrans.
      return hashPtransCode;
   }

}


public /*abstract*/ partial class VvPlacaReport : VvReport
{
   public struct JOPPD_stranaA
   {
      public DateTime dateIzvj   ;
      public string   oznakaIzvj ;
      public string   vrstaIzvj  ;
      public string   piNaziv    ;
      public string   piMjesto   ;
      public string   piUlica    ;
      public string   piUlicaBr  ;
      public string   piEmail    ;
      public string   piOIB      ;
      public string   piOznaka   ;
      public string   opNaziv    ;
      public string   opMjesto   ;
      public string   opUlica    ;
      public string   opUlicaBr  ;
      public string   opEmail    ;
      public string   opOIB      ;
      public int      brojOsoba  ;
      public int      brojRedaka ;
      public decimal  porP01     ;
      public decimal  porP11     ;
      public decimal  porP12     ;
      public decimal  porP02     ;
      public decimal  porP03     ;
      public decimal  porP04     ;
      public decimal  porP05     ;
      public decimal  mio1P01    ;
      public decimal  mio1P02    ;
      public decimal  mio1P03    ;
      public decimal  mio1P04    ;
      public decimal  mio1P05    ;
      public decimal  mio1P06    ;
      public decimal  mio2P01    ;
      public decimal  mio2P02    ;
      public decimal  mio2P03    ;
      public decimal  mio2P04    ;
      public decimal  mio2P05    ;
      public decimal  zdrP01     ;
      public decimal  zdrP02     ;
      public decimal  zdrP03     ;
      public decimal  zdrP04     ;
      public decimal  zdrP05     ;
      public decimal  zdrP06     ;
      public decimal  zdrP07     ;
      public decimal  zdrP08     ;
      public decimal  zdrP09     ;
      public decimal  zdrP10     ;
      public decimal  zapP01     ;
      public decimal  zapP02     ;
      public decimal  zapP03     ;
      public decimal  npNetto    ;
      public decimal  ktaMO2     ;
      public string   sastIme    ;
      public string   sastPrz    ;

      // of 28.02.2015.
      public decimal  porP06  ;
      public decimal  mio1P07 ;
      public decimal  mio2P06 ;
      public decimal  zdrP11  ;
      public decimal  zdrP12  ;
      public decimal  zapP04  ; // ovog jos nema u shemi ali ima na papiru
      public decimal  nerezid ; 
      public int      nzInvP1 ;
      public decimal  nzInvP2 ;
   }

   protected JOPPD_stranaA jpdStrA;

   #region OnoStoBiInaceBiloPojedinacno

   #region Universal Typed DataSet

   protected DS_Placa ds_PlacaReport = new DS_Placa();

   public override DataSet VirtualUntypedDataSet { get { return ds_PlacaReport; } }

   #endregion Universal Typed DataSet

   protected ReportDocument reportDocument;

   public override ReportDocument VirtualReportDocument { get { return reportDocument; } }

   int jpdRbr;

   #endregion OnoStoBiInaceBiloPojedinacno

   #region Constructor and some propertiez

   public VvPlacaReport()
   {
   }

   public VvPlacaReport(ReportDocument _reportDocument, string _reportName, VvRpt_Placa_Filter _rptFilter) : this(_reportDocument, ZXC.VvRptExternTblChooser_Placa.Empty, _reportName, _rptFilter)
   {
   }

   public VvPlacaReport(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter) : base(_reportName)
   {
      this.reportDocument = _reportDocument;
      this.RptFilter      = _rptFilter;

      ReportDatasetNeedsExternTable_Placa = true;

      // 09.12.2024: 
      if(reportDocument is Vektor.Reports.PIZ.CR_PersonMaticni)
      {
         //ReportDatasetNeedsExternTable_Placa = false;

         ds_PlacaReport.Relations.Clear();
      }

      if(externTblChooser.Person == true) ReportDatasetNeedsExternTable_Person = true;
      if(externTblChooser.PtranE == true) ReportDatasetNeedsExternTable_PtranE = true;
      if(externTblChooser.PtranO == true) ReportDatasetNeedsExternTable_PtranO = true;

      if(externTblChooser.Kupdob  == true) ReportDatasetNeedsExternTable_Kupdob                     = true;
      if(externTblChooser.Kupdob2 == true) ReportDatasetNeedsExternTable_Kupdob_ForSomeSecondReason = true;

   }

   public override VvRptFilter VirtualRptFilter { get { return this.RptFilter; } }

   private VvRpt_Placa_Filter rptFilter;
   protected VvRpt_Placa_Filter RptFilter
   {
      get { return this.rptFilter; }
      set {        this.rptFilter = value; }
   }

   public ZXC.PIZ_FilterStyle FilterStyle;

   public bool VvLet_TT_RRiPP_Only { get; set; }

   public bool VvLet_TT_AHiUGiNOiK4_Only { get; set; }

   public bool VvSkip_VrstaObracuna_00 { get; set; }

   public bool DajSamoZbirneObustave { get; set; }

   protected struct PlacaVirmanInfo
   {
      public bool aJeObrtnik;
      //public bool aJeBankaAneZAP;
      public bool aJeStranger;
      public bool aJeZORo;
      public bool aJeAutHonorar;
      public bool aJeAutHonUmje;
      public bool aJeAHSamostUm;
      public bool aJeUgovODjelu;
      public bool aJeNadzorOdb;
      public bool aJeZAP_II;
      public bool aJePoduzetPl;
      public bool aJeIDD_Kolona4;
      public bool aJeSezZapPolj;
      public bool aJePorNaDobit;
      public bool aJeStrucOspos;
      public bool aJeSamoDop; // 09.02.2016.

      public string oib_inUse;
      public string RSm_ID_inUse;

      public bool aJeOtherDohodak { get { return aJeAutHonorar || aJeAutHonUmje || aJeAHSamostUm || aJeNadzorOdb || aJeUgovODjelu || aJeSezZapPolj; } }

   };

   protected PlacaVirmanInfo pvi;

   // 28.12.2012: 
 //const string standardGovernmentZiroRacun = "1001005-1863000160";
   public const string standardGovernmentZiroRacun = "HR1210010051863000160";

   public const string Normal_IBAN_root = "32";
   public const string Zastic_IBAN_root = "35";
   public const string ZiroRn_IBAN_root = "31";

   protected List<VirmanStruct> TheVirmanList;

   protected Ptrans PtransSumRec_alfa13;
   protected Ptrans PtransSumRec_beta14;
   protected Ptrans PtransSumRec_gama15;

   protected int Rad1PersonCount_1all { get; set; } protected int Rad1PersonCount_1zene { get; set; }
   protected int Rad1PersonCount_2all { get; set; } protected int Rad1PersonCount_2zene { get; set; }
   protected int Rad1PersonCount_3all { get; set; } protected int Rad1PersonCount_3zene { get; set; }
   protected int Rad1PersonCount_4all { get; set; } protected int Rad1PersonCount_4zene { get; set; }

   protected decimal Rad1Sati_prekovr { get; set; }
   protected decimal Rad1Sati_bolDo42 { get; set; }
   protected decimal Rad1Sati_godOdmr { get; set; }
   protected decimal Rad1Sati_blagInr { get; set; }
   protected decimal Rad1Sati_ostalip { get; set; }

   protected bool IsBtchBookg_NettoAndObust { get; set; }

   #endregion Constructor and some propertiez

   #region GetPtrans_Command, Overriding GetPlaca_Command, GetPerson_Command, GetPtranEO_Command

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      return GetPtrans_Command(conn, "", "");

      //if(true /* todo daj samo kad treba */)
      //{
      //   Ptrans ptransSumRec_alfa13 = PtransDao.GetPtransSum_alfa13(conn
      //
      //}
   }

   public XSqlCommand GetPtrans_Command(XSqlConnection conn, string selectColumns, string orderByColumns)
   {
      XSqlCommand cmd = VvSQL.InitCommand(conn);

      VirtualRptFilter.ClearAllFilters_FromClauseGotTableName();

      if(selectColumns.IsEmpty()) selectColumns = VvSQL.GetAllDataTableColumnNames_4Select(ds_PlacaReport.IzvjTable, Ptrans.recordName, "R_", false);

      if(orderByColumns.IsEmpty()) orderByColumns = " t_dokNum, t_serial";

      string recordName = Ptrans.recordName;

      cmd.CommandText =

         "SELECT " + selectColumns + "\n" +
         " FROM  " + recordName    + "\n" +

         VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(RptFilter)      + "\n" +

         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(RptFilter.FilterMembers, false) + "\n" +

         "ORDER BY " + orderByColumns;

      VvSQL.SetReportCommandParamValues(cmd, VirtualRptFilter.FilterMembers);

      return cmd;
   }

   public override XSqlCommand GetPlacaCommand(XSqlConnection conn)
   {
      XSqlCommand cmd = VvSQL.InitCommand(conn);

      VirtualRptFilter.ClearAllFilters_FromClauseGotTableName();

      string recordName    = Placa.recordName;
      string selectColumns = VvSQL.GetAllDataTableColumnNames_4Select(ds_PlacaReport.placa, recordName, "S_", false);

      cmd.CommandText =

         "SELECT " + selectColumns + "\n" +
         " FROM  " + recordName    + "\n" +

         " JOIN " + Ptrans.recordName + " ON " + Ptrans.PlacaForeignKey + " = " + recordName + ".recID\n" +
         
         VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(VirtualRptFilter) +

         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(VirtualRptFilter.FilterMembers, false) + "\n" +

         "GROUP BY " + recordName + " .recID";

      VvSQL.SetReportCommandParamValues(cmd, VirtualRptFilter.FilterMembers);

      return cmd;
   }

   public override XSqlCommand GetPersonCommand(XSqlConnection conn)
   {
      XSqlCommand cmd = VvSQL.InitCommand(conn);

      VirtualRptFilter.ClearAllFilters_FromClauseGotTableName();

      string recordName    = Person.recordName;
    //string selectColumns = VvSQL.GetAllDataTableColumnNames_4Select(ds_PlacaReport.person, recordName, false, false);
      string selectColumns = VvSQL.GetAllDataTableColumnNames_4Select(ds_PlacaReport.person, recordName, "R_" , false);

      cmd.CommandText =

         "SELECT "    + selectColumns + "\n" +

         "FROM "      + Ptrans.recordName + " \n" +
         
         "LEFT JOIN " + recordName + " ON " + Ptrans.PersonForeignKey + " = person.personCD \n" +
         
         VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(VirtualRptFilter) +

         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(VirtualRptFilter.FilterMembers, false) + "\n" +

         "GROUP BY " + recordName + " .recID";

      VvSQL.SetReportCommandParamValues(cmd, VirtualRptFilter.FilterMembers);

      return cmd;
   }

   public override XSqlCommand GetKupdobCommand(XSqlConnection conn)
   {
      XSqlCommand cmd = VvSQL.InitCommand(conn);

      VirtualRptFilter.ClearAllFilters_FromClauseGotTableName();

      string recordName    = Kupdob.recordName;
      string selectColumns = VvSQL.GetAllDataTableColumnNames_4Select(ds_PlacaReport.kupdob, recordName, false, false);

      cmd.CommandText =

         "SELECT "     + selectColumns     + "\n" +
         "FROM "       + Ptrans.recordName + "\n" +
         "LEFT  JOIN " + Person.recordName + " ON        " + Ptrans.PersonForeignKey + " = person.personCD \n" +
         "RIGHT JOIN " + Kupdob.recordName + " ON person." + Person.BankaForeignKey  + " = kupdobCD        \n" +
                                             " OR person." + Person.MtrosForeignKey  + " = kupdobCD        \n" +
                                             " OR person." + Person.Banka2ForeignKey + " = kupdobCD        \n" +

         VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(VirtualRptFilter) +

         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(VirtualRptFilter.FilterMembers, false) + "\n" +

         "GROUP BY " + recordName + ".recID";

      VvSQL.SetReportCommandParamValues(cmd, VirtualRptFilter.FilterMembers);

      return cmd;
   }

   public override XSqlCommand GetKupdobCommand2(XSqlConnection conn)
   {
      XSqlCommand cmd = VvSQL.InitCommand(conn);

      VirtualRptFilter.ClearAllFilters_FromClauseGotTableName();

      string recordName    = Kupdob.recordName;
      string selectColumns = VvSQL.GetAllDataTableColumnNames_4Select(ds_PlacaReport.kupdob, recordName, false, false);

      cmd.CommandText =

         "SELECT "     + selectColumns     + "\n" +
         "FROM "       + Ptrans.recordName + "\n" +

         "RIGHT JOIN " + Ptrano.recordName + " USING (" + Ptrans.PlacaForeignKey + ")\n" +

         "RIGHT JOIN " + Kupdob.recordName + " ON " + Ptrano.recordName + "." + Ptrano.KupdobForeignKey + " = kupdobCD        \n" +

         VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(VirtualRptFilter) +

         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(VirtualRptFilter.FilterMembers, false) + "\n" +

         "GROUP BY " + recordName + ".recID";

      VvSQL.SetReportCommandParamValues(cmd, VirtualRptFilter.FilterMembers);

      return cmd;
   }

   public override XSqlCommand GetPtraneCommand(XSqlConnection conn)
   {
      return GetPtranEOCommand(conn, Ptrane.recordName, " t_personCD, t_rsOD ");
   }

   public override XSqlCommand GetPtranoCommand(XSqlConnection conn)
   {
      return GetPtranEOCommand(conn, Ptrano.recordName, "" /* TODO?*/);
   }

   private XSqlCommand GetPtranEOCommand(XSqlConnection conn, string recordName, string orderByColumns)
   {
      XSqlCommand cmd = VvSQL.InitCommand(conn);

      VirtualRptFilter.ClearAllFilters_FromClauseGotTableName();

      string selectColumns = VvSQL.GetAllDataTableColumnNames_4Select(ds_PlacaReport.Tables[recordName], recordName, true, false);

      cmd.CommandText =

         "SELECT "    + selectColumns + "\n" +

         "FROM "      + Ptrans.recordName + " \n" +
         
         //"RIGHT JOIN " + recordName + " USING (" + Ptrans.PlacaForeignKey + ")\n" +
         "RIGHT JOIN " + recordName + " USING (" + Ptrans.PlacaForeignKey + ", " + Ptrans.PersonForeignKey + ")\n" +
         
         VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(VirtualRptFilter) +

         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(VirtualRptFilter.FilterMembers, false) + "\n" +

         "GROUP BY " + recordName + ".recID" + "\n" +

         (orderByColumns.NotEmpty() ? "ORDER BY " + orderByColumns : "");

      VvSQL.SetReportCommandParamValues(cmd, VirtualRptFilter.FilterMembers);

      return cmd;
   }

   #endregion GetPtrans_Command, Overriding GetPlaca_Command, GetPerson_Command, GetPtranEO_Command

   #region GetPersonSifrar_OrderByColumns

   protected string GetPersonSifrar_OrderByColumns(VvSQL.SorterType sorterType)
   {
      string orderColumns = "Dokum ORDER_BAJ Very BAD! in GetPersonSifrar_OrderByColumns()";

      switch(sorterType)
      {
         case VvSQL.SorterType.Person: orderColumns = " prezime, ime, personCD "; break;
         case VvSQL.SorterType.Code  : orderColumns = " personCD "; break;
      }
      return orderColumns;
   }

   #endregion GetPersonSifrar_OrderByColumns

   #region PerformAdditionalDataSetOperation

   private const string NetoBtchBookgID = "Grupa NETO placa";

   public override bool PerformAdditionalDataSetOperation()
   {
      #region Main loop

      if(ds_PlacaReport.IzvjTable.Count == 0) return false;

      DS_Placa.IzvjTableDataTable  ptransTable     = ds_PlacaReport.IzvjTable;
      DS_Placa.ptraneDataTable     ptraneTable     = ds_PlacaReport.ptrane;
      DS_Placa.ptranoDataTable     ptranoTable     = ds_PlacaReport.ptrano;
      DS_Placa.placaDataTable      placaTable      = ds_PlacaReport.placa;
      DS_Placa.personDataTable     personTable     = ds_PlacaReport.person;
      DS_Placa.kupdobDataTable     kupdobTable     = ds_PlacaReport.kupdob;

      DS_Placa.placaSumDataTable   placaSumTable   = ds_PlacaReport.placaSum;
      DS_Placa.ptransSumDataTable  ptransSumTable  = ds_PlacaReport.ptransSum;
      DS_Placa.ptraneSumDataTable  ptraneSumTable  = ds_PlacaReport.ptraneSum;
      DS_Placa.rsmBstranaDataTable rsmBstranaTable = ds_PlacaReport.rsmBstrana;
      DS_Placa.jpdBstranaDataTable jpdBstranaTable = ds_PlacaReport.jpdBstrana;
      DS_Placa.opcinaSumDataTable  opcinaSumTable  = ds_PlacaReport.opcinaSum;
      DS_Placa.virmanDataTable     virmanTable     = ds_PlacaReport.virman;


      Placa  placa_rec  = new Placa();
      Ptrans ptrans_rec = new Ptrans();

      DS_Placa.placaRow placaRowOfCurrentTranses;

      string theVOP;
      List<uint> personCDw110list = new List<uint>();

      uint currParentID, rsmRbr = 0;

      currParentID = ptransTable[0].t_parentID;

      OnFirstLineOfDocumentAction_InitializePlacaRec(currParentID, placa_rec, placaTable, out placaRowOfCurrentTranses, ptraneTable, ptranoTable);

      // 30.06.2015: 
      foreach(DS_Placa.kupdobRow kupdobRow in kupdobTable.Rows)
      {
         kupdobRow.ziro1 = ZXC.GetIBANfromOldZiro(kupdobRow.ziro1);
      }

      foreach(DS_Placa.IzvjTableRow ptransRow in ptransTable.Rows)
      {
         // New Document Actions
         if(currParentID != ptransRow.t_parentID)
         {
            OnLastLineOfDocumentAction_SumPlacaResults(currParentID, placaRowOfCurrentTranses, ptransTable);

            currParentID = ptransRow.t_parentID;

            OnFirstLineOfDocumentAction_InitializePlacaRec(currParentID, placa_rec, placaTable, out placaRowOfCurrentTranses, ptraneTable, ptranoTable);
         }

         var ptraneRowsOfThisPtrans = ptraneTable.Where(ptranE => ptranE.t_personCD == ptransRow.t_personCD &&
                                                                  ptranE.t_parentID == ptransRow.t_parentID);

         ptrans_rec.FillFromDataRow_CalcResults_SetRowResults(ptransRow, placa_rec, ptraneRowsOfThisPtrans);

      } // foreach(DS_Placa.IzvjTableRow ptransRow in ptransTable.Rows) 

      // Za zadnjega

      OnLastLineOfDocumentAction_SumPlacaResults(currParentID, placaRowOfCurrentTranses, ptransTable);

      #endregion Main loop

      #region FUSE - PUSE

      // List<DS_Placa.placaRow> placaRowsToBeRemoved = new List<DS_Placa.placaRow>(); // Za Neku Filter 'skip ovakve' logiku koje je nepoznata u trenutku vadjenja iz data layer-a 

      // Vidi u Report_DBT.cs !!! 

      #region Remove Ptrano's non IsZbirObust

      List<DS_Placa.ptranoRow> ptranoRowsToBeRemoved = null;
      
      if(this.DajSamoZbirneObustave == true)
      {
         ptranoRowsToBeRemoved = new List<DS_Placa.ptranoRow>();

         foreach(DS_Placa.ptranoRow ptranoRow in ptranoTable.Rows)
         {
            if(ptranoRow.t_isZbir == 0)
            {
               ptranoRowsToBeRemoved.Add(ptranoRow);
            }
         }
      }

      #endregion Remove Ptrano's non IsZbirObust

      #endregion FUSE - PUSE

      #region X_SUME, P_SUME, OPC_SUME, E_SUME

      #region X_SUME

      #region RowAtIndex[0] ALL (IsplacenoUplacenoObracunato)

      DS_Placa.placaRow[] placaTable_OK_ONLY;
      DS_Placa.placaRow[] placaTable_ISP_ONLY=null;
      DS_Placa.placaRow[] placaTable_OBR_ONLY=null;

      int personCount, prsnCount_Mio, prsnCount_KrPr;
       
      // 17.02.2011:
    //var personsDistinct = ptransTable.Select(ptrans => new { ptrans.t_personCD, ptrans.t_ime, ptrans.t_prezime }).Distinct();
      var persDistPtrList = ptransTable.Distinct(new PtransByPersonCdComparer()).ToList();

      if(reportDocument is CR_IDObrazac_Subreport || reportDocument is CR_IDobrazac2011)
      {
         placaTable_OK_ONLY = placaTable.Where(placa => placa.tt == Placa.TT_REDOVANRAD   ||
                                                        placa.tt == Placa.TT_PLACAUNARAVI ||
                                                        placa.tt == Placa.TT_OSTALIPRIM   || //04.12.2024.
                                                        placa.tt == Placa.TT_PODUZETPLACA).ToArray();

         placaTable_ISP_ONLY = placaTable_OK_ONLY.Where(placa => placa.vrstaObr != Placa.VrObr_NeisplacenaPlaca).ToArray();

         placaTable_OBR_ONLY = placaTable_OK_ONLY.Where(placa => placa.vrstaObr != Placa.VrObr_NadoplacenaPlaca).ToArray();

      }
      else if(reportDocument is CR_IDDobrazac     ||
              reportDocument is CR_ID1obrazac     ||
              reportDocument is CR_ID1Obrazac2013 ||
              reportDocument is CR_PotvrdaDrugiDoh)
      {
         placaTable_OK_ONLY = placaTable.Where(placa => placa.tt == Placa.TT_AUTORHONOR   ||
                                                        placa.tt == Placa.TT_AUTORHONUMJ  ||
                                                        placa.tt == Placa.TT_AHSAMOSTUMJ  ||
                                                        placa.tt == Placa.TT_NADZORODBOR  ||
                                                        placa.tt == Placa.TT_TURSITVIJECE ||
                                                        placa.tt == Placa.TT_IDD_KOLONA_4 ||
                                                        placa.tt == Placa.TT_SEZZAPPOLJOP ||
                                                        placa.tt == Placa.TT_UGOVORODJELU ||
                                                        placa.tt == Placa.TT_DDBEZDOPRINO ||
                                                        placa.tt == Placa.TT_AUVECASTOPA  ||
                                                        placa.tt == Placa.TT_NR1_PX1NEDOP ||
                                                        placa.tt == Placa.TT_NR2_P01NEDOP ||
                                                        placa.tt == Placa.TT_NR3_PX1DADOP 
                                                        ).ToArray();
      }
      else
      {
         placaTable_OK_ONLY = placaTable.ToArray();
      }

      DS_Placa.placaSumRow placaSumRow_OK_ONLY = (DS_Placa.placaSumRow)placaSumTable.Rows.Add();

      /* 01 */   placaSumRow_OK_ONLY.X_tBrutoOsn   = placaTable_OK_ONLY.Sum(placa => placa.S_tBrutoOsn  );
      /* 02 */   placaSumRow_OK_ONLY.X_tTopObrok   = placaTable_OK_ONLY.Sum(placa => placa.S_tTopObrok  );  
      /* 03 */   placaSumRow_OK_ONLY.X_tDodBruto   = placaTable_OK_ONLY.Sum(placa => placa.S_tDodBruto  );  

      // 26.04.2016. rbrJop koristimo t_zivotno pa ne smije doci nigdje drugdje osim u joppcu za redni broj redka
      /* 04 */   placaSumRow_OK_ONLY.X_tZivotno    = placaTable_OK_ONLY.Sum(placa => placa.S_tZivotno   );
      /* 05 */   placaSumRow_OK_ONLY.X_tDopZdr     = placaTable_OK_ONLY.Sum(placa => placa.S_tDopZdr    );
      /* 06 */   placaSumRow_OK_ONLY.X_tDobMIO     = placaTable_OK_ONLY.Sum(placa => placa.S_tDobMIO    );
      if(ptrans_rec.T_dokDate > new DateTime(2010, 07, 01)) placaSumRow_OK_ONLY.X_tZivotno = 0.00M;
      
      /*    */   placaSumRow_OK_ONLY.X_tDopZdr2020 = placaTable_OK_ONLY.Sum(placa => placa.S_tDopZdr2020);

      /* 07 */   placaSumRow_OK_ONLY.X_tNetoAdd    = placaTable_OK_ONLY.Sum(placa => placa.S_tNetoAdd   );
      /* 08 */   placaSumRow_OK_ONLY.X_tPrijevoz   = placaTable_OK_ONLY.Sum(placa => placa.S_tPrijevoz  );
      /* 09 */   placaSumRow_OK_ONLY.X_rBruto100   = placaTable_OK_ONLY.Sum(placa => placa.S_rBruto100  );
      /* 10 */   placaSumRow_OK_ONLY.X_rTheBruto   = placaTable_OK_ONLY.Sum(placa => placa.S_rTheBruto  );
      /* 11 */   placaSumRow_OK_ONLY.X_rMioOsn     = placaTable_OK_ONLY.Sum(placa => placa.S_rMioOsn    );
      /* 12 */   placaSumRow_OK_ONLY.X_rMio1stup   = placaTable_OK_ONLY.Sum(placa => placa.S_rMio1stup  );
      /* 13 */   placaSumRow_OK_ONLY.X_rMio2stup   = placaTable_OK_ONLY.Sum(placa => placa.S_rMio2stup  );
      /* 14 */   placaSumRow_OK_ONLY.X_rMioAll     = placaTable_OK_ONLY.Sum(placa => placa.S_rMioAll    );
      /* 15 */   placaSumRow_OK_ONLY.X_rDoprIz     = placaTable_OK_ONLY.Sum(placa => placa.S_rDoprIz    );
      /* 16 */   placaSumRow_OK_ONLY.X_rOdbitak    = placaTable_OK_ONLY.Sum(placa => placa.S_rOdbitak   );
      /* 17 */   placaSumRow_OK_ONLY.X_rPremije    = placaTable_OK_ONLY.Sum(placa => placa.S_rPremije   );
      /* 18 */   placaSumRow_OK_ONLY.X_rDohodak    = placaTable_OK_ONLY.Sum(placa => placa.S_rDohodak   );
      /* 19 */   placaSumRow_OK_ONLY.X_rPorOsnAll  = placaTable_OK_ONLY.Sum(placa => placa.S_rPorOsnAll );
      /* 20 */   placaSumRow_OK_ONLY.X_rPorOsn1    = placaTable_OK_ONLY.Sum(placa => placa.S_rPorOsn1   );
      /* 21 */   placaSumRow_OK_ONLY.X_rPorOsn2    = placaTable_OK_ONLY.Sum(placa => placa.S_rPorOsn2   );
      /* 22 */   placaSumRow_OK_ONLY.X_rPorOsn3    = placaTable_OK_ONLY.Sum(placa => placa.S_rPorOsn3   );
      /* 23 */   placaSumRow_OK_ONLY.X_rPorOsn4    = placaTable_OK_ONLY.Sum(placa => placa.S_rPorOsn4   );
      /* 24 */   placaSumRow_OK_ONLY.X_rPor1uk     = placaTable_OK_ONLY.Sum(placa => placa.S_rPor1uk    );
      /* 25 */   placaSumRow_OK_ONLY.X_rPor2uk     = placaTable_OK_ONLY.Sum(placa => placa.S_rPor2uk    );
      /* 26 */   placaSumRow_OK_ONLY.X_rPor3uk     = placaTable_OK_ONLY.Sum(placa => placa.S_rPor3uk    );
      /* 27 */   placaSumRow_OK_ONLY.X_rPor4uk     = placaTable_OK_ONLY.Sum(placa => placa.S_rPor4uk    );
      /* 28 */   placaSumRow_OK_ONLY.X_rPorezAll   = placaTable_OK_ONLY.Sum(placa => placa.S_rPorezAll  );
      /* 29 */   placaSumRow_OK_ONLY.X_rPrirez     = placaTable_OK_ONLY.Sum(placa => placa.S_rPrirez    );
      /* 30 */   placaSumRow_OK_ONLY.X_rPorPrirez  = placaTable_OK_ONLY.Sum(placa => placa.S_rPorPrirez );
      /* 31 */   placaSumRow_OK_ONLY.X_rNetto      = placaTable_OK_ONLY.Sum(placa => placa.S_rNetto     );
      /* 32 */   placaSumRow_OK_ONLY.X_rObustave   = placaTable_OK_ONLY.Sum(placa => placa.S_rObustave  );
      /* 33 */   placaSumRow_OK_ONLY.X_r2Pay       = placaTable_OK_ONLY.Sum(placa => placa.S_r2Pay      );
      /* 34 */   placaSumRow_OK_ONLY.X_rNaRuke     = placaTable_OK_ONLY.Sum(placa => placa.S_rNaRuke    );
      /* 35 */   placaSumRow_OK_ONLY.X_rZdrNa      = placaTable_OK_ONLY.Sum(placa => placa.S_rZdrNa     );
      /* 36 */   placaSumRow_OK_ONLY.X_rZorNa      = placaTable_OK_ONLY.Sum(placa => placa.S_rZorNa     );
      /* 37 */   placaSumRow_OK_ONLY.X_rZapNa      = placaTable_OK_ONLY.Sum(placa => placa.S_rZapNa     );
      /* 38 */   placaSumRow_OK_ONLY.X_rZapII      = placaTable_OK_ONLY.Sum(placa => placa.S_rZapII     );
      /* 39 */   placaSumRow_OK_ONLY.X_rZapAll     = placaTable_OK_ONLY.Sum(placa => placa.S_rZapAll    );
      /* 40 */   placaSumRow_OK_ONLY.X_rDoprNa     = placaTable_OK_ONLY.Sum(placa => placa.S_rDoprNa    );
      /* 41 */   placaSumRow_OK_ONLY.X_rDoprAll    = placaTable_OK_ONLY.Sum(placa => placa.S_rDoprAll   );
      /* 42 */   placaSumRow_OK_ONLY.X_rMio1stupNa = placaTable_OK_ONLY.Sum(placa => placa.S_rMio1stupNa);
      /* 43 */   placaSumRow_OK_ONLY.X_rMio2stupNa = placaTable_OK_ONLY.Sum(placa => placa.S_rMio2stupNa);
      /* 44 */   placaSumRow_OK_ONLY.X_rMioAllNa   = placaTable_OK_ONLY.Sum(placa => placa.S_rMioAllNa  );
      /* 45 */   placaSumRow_OK_ONLY.X_rKrizPorOsn = placaTable_OK_ONLY.Sum(placa => placa.S_rKrizPorOsn);
      /* 46 */   placaSumRow_OK_ONLY.X_rKrizPorUk  = placaTable_OK_ONLY.Sum(placa => placa.S_rKrizPorUk );
      /* 47 */   placaSumRow_OK_ONLY.X_rSatiR      = placaTable_OK_ONLY.Sum(placa => placa.S_rSatiR     );
      /* 48 */   placaSumRow_OK_ONLY.X_rSatiB      = placaTable_OK_ONLY.Sum(placa => placa.S_rSatiB     );
      /* 49 */   placaSumRow_OK_ONLY.X_rZpiUk      = placaTable_OK_ONLY.Sum(placa => placa.S_rZpiUk     );
      /* 50 */   placaSumRow_OK_ONLY.X_rDaniZpi    = placaTable_OK_ONLY.Sum(placa => placa.S_rDaniZpi   );
      /* 51 */   placaSumRow_OK_ONLY.X_rNettoWoAdd = placaTable_OK_ONLY.Sum(placa => placa.S_rNettoWoAdd);
      /* 52 */   placaSumRow_OK_ONLY.X_rAHizdatak  = placaTable_OK_ONLY.Sum(placa => placa.S_rAHizdatak );
      /* 53 */   placaSumRow_OK_ONLY.X_rNettoAftKrp= placaTable_OK_ONLY.Sum(placa => placa.S_rNettoAftKrp );
      /* 54 */   placaSumRow_OK_ONLY.X_rBrtDodNaStaz=placaTable_OK_ONLY.Sum(placa => placa.S_rBrtDodNaStaz);
      /* 55 */   placaSumRow_OK_ONLY.X_rTheBruto_WoNZ=placaTable_OK_ONLY.Sum(placa => placa.S_rTheBruto_WoNZ);
      /* 56 */   placaSumRow_OK_ONLY.X_tBrDodPoloz  = placaTable_OK_ONLY.Sum(placa => placa.S_tBrDodPoloz );  
              
                 placaSumRow_OK_ONLY.X_rMio1Olk     = placaTable_OK_ONLY.Sum(placa => placa.S_rMio1Olk );  
                 placaSumRow_OK_ONLY.X_rMio1Osn     = placaTable_OK_ONLY.Sum(placa => placa.S_rMio1Osn );  
                 placaSumRow_OK_ONLY.X_tNP73        = placaTable_OK_ONLY.Sum(placa => placa.S_tNP73    );  

                 placaSumRow_OK_ONLY.X_rIDizdaci   = placaSumRow_OK_ONLY.X_rDoprIz + placaSumRow_OK_ONLY.X_rPremije;

                 placaSumRow_OK_ONLY.X_uMMYYYY     = Placa.Get_uMMYYYY(placa_rec.DokDate);
                 placaSumRow_OK_ONLY.X_uMM         = Placa.Get_uMM(placa_rec.DokDate);
                 placaSumRow_OK_ONLY.X_uYYYY       = Placa.Get_uYYYY(placa_rec.DokDate);
                 
                 placaSumRow_OK_ONLY.X_zaMMYYYY    = placa_rec.MMYYYY;
                 placaSumRow_OK_ONLY.X_zaMM        = Placa.Get_zaMM  (placa_rec.MMYYYY);
                 placaSumRow_OK_ONLY.X_zaYYYY      = Placa.Get_zaYYYY(placa_rec.MMYYYY);

                 placaSumRow_OK_ONLY.X_stMioIz     = placa_rec.Rule_StMio1stup;
                 placaSumRow_OK_ONLY.X_stMioIz2    = placa_rec.Rule_StMio2stup;
                 placaSumRow_OK_ONLY.X_stZdrNa     = placa_rec.Rule_StZdrNa;
                 placaSumRow_OK_ONLY.X_stZorNa     = placa_rec.Rule_StZorNa;
                 placaSumRow_OK_ONLY.X_stZapNa     = placa_rec.Rule_StZapNa;
                 placaSumRow_OK_ONLY.X_stZapII     = placa_rec.Rule_StZapII;

                 placaSumRow_OK_ONLY.X_stZdrDD     = placa_rec.Rule_StZdrDD;

                 personCount    = placaTable_OK_ONLY.Join(ptransTable, 
                    placa => placa.recID, ptrans => ptrans.t_parentID, (placa, ptrans) => new { ptrans.t_personCD }).Distinct().Count();

                 prsnCount_Mio  = placaTable_OK_ONLY.Join(ptransTable.Where(ptr => ptr.R_MioAll  .NotZero()), 
                    placa => placa.recID, ptrans => ptrans.t_parentID, (placa, ptrans) => new { ptrans.t_personCD }).Distinct().Count();

                 prsnCount_KrPr = placaTable_OK_ONLY.Join(ptransTable.Where(ptr => ptr.R_KrizPorUk.NotZero()), 
                    placa => placa.recID, ptrans => ptrans.t_parentID, (placa, ptrans) => new { ptrans.t_personCD }).Distinct().Count();

                 placaSumRow_OK_ONLY.X_personCount    = personCount;
                 placaSumRow_OK_ONLY.X_prsnCount_Mio  = prsnCount_Mio;
                 placaSumRow_OK_ONLY.X_prsnCount_KrPr = prsnCount_KrPr;

                 // ovo je za ID obrazac. Za RSm ga se dolje prejebe 
                 if(placaTable[0].tt == Placa.TT_PODUZETPLACA) placaSumRow_OK_ONLY.X_rSm_ID = "3";
                 else                                          placaSumRow_OK_ONLY.X_rSm_ID = "1";

      #endregion RowAtIndex[0] ALL (IsplacenoUplacenoObracunato)

      #region RowAtIndex[1] ONLY IsplacenoUplaceno 

      DS_Placa.placaSumRow placaSumRow_ISP = (DS_Placa.placaSumRow)placaSumTable.Rows.Add();

      if(placaTable_ISP_ONLY != null)
      {
         /* 01 */   placaSumRow_ISP.X_rTheBruto   = placaTable_ISP_ONLY.Sum(placa => placa.S_rTheBruto  );
         /* 02 */   placaSumRow_ISP.X_rMio1stup   = placaTable_ISP_ONLY.Sum(placa => placa.S_rMio1stup  );
         /* 03 */   placaSumRow_ISP.X_rMio2stup   = placaTable_ISP_ONLY.Sum(placa => placa.S_rMio2stup  );
         /* 04 */   placaSumRow_ISP.X_rDoprIz     = placaTable_ISP_ONLY.Sum(placa => placa.S_rDoprIz    );
         /* 05 */   placaSumRow_ISP.X_rPremije    = placaTable_ISP_ONLY.Sum(placa => placa.S_rPremije   );
         /* 06 */   placaSumRow_ISP.X_tZivotno    = placaTable_ISP_ONLY.Sum(placa => placa.S_tZivotno   );
         /* 07 */   placaSumRow_ISP.X_tDopZdr     = placaTable_ISP_ONLY.Sum(placa => placa.S_tDopZdr    );
         /*    */   placaSumRow_ISP.X_tDopZdr2020 = placaTable_ISP_ONLY.Sum(placa => placa.S_tDopZdr2020);
         /* 08 */   placaSumRow_ISP.X_tDobMIO     = placaTable_ISP_ONLY.Sum(placa => placa.S_tDobMIO    );
         /* 09 */   placaSumRow_ISP.X_rDohodak    = placaTable_ISP_ONLY.Sum(placa => placa.S_rDohodak   );
         /* 10 */   placaSumRow_ISP.X_rOdbitak    = placaTable_ISP_ONLY.Sum(placa => placa.S_rOdbitak   );
         /* 11 */   placaSumRow_ISP.X_rPorOsnAll  = placaTable_ISP_ONLY.Sum(placa => placa.S_rPorOsnAll );
         /* 12 */   placaSumRow_ISP.X_rPorPrirez  = placaTable_ISP_ONLY.Sum(placa => placa.S_rPorPrirez );
         /* 13 */   placaSumRow_ISP.X_rPorezAll   = placaTable_ISP_ONLY.Sum(placa => placa.S_rPorezAll  );
         /* 14 */   placaSumRow_ISP.X_rPrirez     = placaTable_ISP_ONLY.Sum(placa => placa.S_rPrirez    );
         /* 15 */   placaSumRow_ISP.X_rMio1stupNa = placaTable_ISP_ONLY.Sum(placa => placa.S_rMio1stupNa);
         /* 16 */   placaSumRow_ISP.X_rMio2stupNa = placaTable_ISP_ONLY.Sum(placa => placa.S_rMio2stupNa);
         /* 17 */   placaSumRow_ISP.X_rZdrNa      = placaTable_ISP_ONLY.Sum(placa => placa.S_rZdrNa     );
         /* 18 */   placaSumRow_ISP.X_rZorNa      = placaTable_ISP_ONLY.Sum(placa => placa.S_rZorNa     );
         /* 19 */   placaSumRow_ISP.X_rZapNa      = placaTable_ISP_ONLY.Sum(placa => placa.S_rZapNa     );
         /* 20 */   placaSumRow_ISP.X_rZapII      = placaTable_ISP_ONLY.Sum(placa => placa.S_rZapII     );
         /* 21 */   placaSumRow_ISP.X_rZpiUk      = placaTable_ISP_ONLY.Sum(placa => placa.S_rZpiUk     );

                    placaSumRow_ISP.X_rIDizdaci   = placaSumRow_ISP.X_rDoprIz + placaSumRow_ISP.X_rPremije;

         personCount = placaTable_ISP_ONLY.Join(ptransTable,
            placa => placa.recID, ptrans => ptrans.t_parentID, (placa, ptrans) => new { ptrans.t_personCD }).Distinct().Count();

         prsnCount_Mio = placaTable_ISP_ONLY.Join(ptransTable.Where(ptr => ptr.R_MioAll.NotZero()),
            placa => placa.recID, ptrans => ptrans.t_parentID, (placa, ptrans) => new { ptrans.t_personCD }).Distinct().Count();

         placaSumRow_ISP.X_personCount   = personCount;
         placaSumRow_ISP.X_prsnCount_Mio = prsnCount_Mio;
      }

      #endregion RowAtIndex[1] ONLY IsplacenoUplaceno

      #region RowAtIndex[2] ONLY Obracunano

      DS_Placa.placaSumRow placaSumRow_OBR = (DS_Placa.placaSumRow)placaSumTable.Rows.Add();

      if(placaTable_OBR_ONLY != null)
      {
         /* 01 */   placaSumRow_OBR.X_rTheBruto   = placaTable_OBR_ONLY.Sum(placa => placa.S_rTheBruto  );
         /* 02 */   placaSumRow_OBR.X_rMio1stup   = placaTable_OBR_ONLY.Sum(placa => placa.S_rMio1stup  );
         /* 03 */   placaSumRow_OBR.X_rMio2stup   = placaTable_OBR_ONLY.Sum(placa => placa.S_rMio2stup  );
         /* 04 */   placaSumRow_OBR.X_rDoprIz     = placaTable_OBR_ONLY.Sum(placa => placa.S_rDoprIz    );
         /* 05 */   placaSumRow_OBR.X_rPremije    = placaTable_OBR_ONLY.Sum(placa => placa.S_rPremije   );
         /* 06 */   placaSumRow_OBR.X_tZivotno    = placaTable_OBR_ONLY.Sum(placa => placa.S_tZivotno   );
         /* 07 */   placaSumRow_OBR.X_tDopZdr     = placaTable_OBR_ONLY.Sum(placa => placa.S_tDopZdr    );
         /*    */   placaSumRow_OBR.X_tDopZdr2020 = placaTable_OBR_ONLY.Sum(placa => placa.S_tDopZdr2020);
         /* 08 */   placaSumRow_OBR.X_tDobMIO     = placaTable_OBR_ONLY.Sum(placa => placa.S_tDobMIO    );
         /* 09 */   placaSumRow_OBR.X_rDohodak    = placaTable_OBR_ONLY.Sum(placa => placa.S_rDohodak   );
         /* 10 */   placaSumRow_OBR.X_rOdbitak    = placaTable_OBR_ONLY.Sum(placa => placa.S_rOdbitak   );
         /* 11 */   placaSumRow_OBR.X_rPorOsnAll  = placaTable_OBR_ONLY.Sum(placa => placa.S_rPorOsnAll );
         /* 12 */   placaSumRow_OBR.X_rPorPrirez  = placaTable_OBR_ONLY.Sum(placa => placa.S_rPorPrirez );
         /* 13 */   placaSumRow_OBR.X_rPorezAll   = placaTable_OBR_ONLY.Sum(placa => placa.S_rPorezAll  );
         /* 14 */   placaSumRow_OBR.X_rPrirez     = placaTable_OBR_ONLY.Sum(placa => placa.S_rPrirez    );
         /* 15 */   placaSumRow_OBR.X_rMio1stupNa = placaTable_OBR_ONLY.Sum(placa => placa.S_rMio1stupNa);
         /* 16 */   placaSumRow_OBR.X_rMio2stupNa = placaTable_OBR_ONLY.Sum(placa => placa.S_rMio2stupNa);
         /* 17 */   placaSumRow_OBR.X_rZdrNa      = placaTable_OBR_ONLY.Sum(placa => placa.S_rZdrNa     );
         /* 18 */   placaSumRow_OBR.X_rZorNa      = placaTable_OBR_ONLY.Sum(placa => placa.S_rZorNa     );
         /* 19 */   placaSumRow_OBR.X_rZapNa      = placaTable_OBR_ONLY.Sum(placa => placa.S_rZapNa     );
         /* 20 */   placaSumRow_OBR.X_rZapII      = placaTable_OBR_ONLY.Sum(placa => placa.S_rZapII     );
         /* 21 */   placaSumRow_OBR.X_rZpiUk      = placaTable_OBR_ONLY.Sum(placa => placa.S_rZpiUk     );

                    placaSumRow_OBR.X_rIDizdaci   = placaSumRow_OBR.X_rDoprIz + placaSumRow_OBR.X_rPremije;

         personCount = placaTable_OBR_ONLY.Join(ptransTable,
            placa => placa.recID, ptrans => ptrans.t_parentID, (placa, ptrans) => new { ptrans.t_personCD }).Distinct().Count();

         prsnCount_Mio = placaTable_OBR_ONLY.Join(ptransTable.Where(ptr => ptr.R_MioAll.NotZero()),
            placa => placa.recID, ptrans => ptrans.t_parentID, (placa, ptrans) => new { ptrans.t_personCD }).Distinct().Count();

         placaSumRow_OBR.X_personCount   = personCount;
         placaSumRow_OBR.X_prsnCount_Mio = prsnCount_Mio;
      }

      #endregion RowAtIndex[2] ONLY Obracunano

      #region RowAtIndex[3] ONLY TT_NADZORODBOR or TT_UGOVORODJELU or TT_TURSITVIJECE

      var ptransTable_NOT_PENZ_ONLY = ptransTable.Where
         (
            ptrans => 
               ptrans.t_spc != (byte)Ptrans.SpecEnum.PENZ &&
               (ptrans.t_tt == Placa.TT_NADZORODBOR  ||
                ptrans.t_tt == Placa.TT_UGOVORODJELU ||
                ptrans.t_tt == Placa.TT_TURSITVIJECE     )
         ).ToArray();

      DS_Placa.placaSumRow placaSumRow_NoUg_WoPENZ = (DS_Placa.placaSumRow)placaSumTable.Rows.Add();

      /* 01 */   placaSumRow_NoUg_WoPENZ.X_rTheBruto   = ptransTable_NOT_PENZ_ONLY.Sum(ptrans => ptrans.R_TheBruto );
      /* 02 */   placaSumRow_NoUg_WoPENZ.X_rMioOsn     = ptransTable_NOT_PENZ_ONLY.Sum(ptrans => ptrans.R_MioOsn   );
      /* 03 */   placaSumRow_NoUg_WoPENZ.X_rMio1stup   = ptransTable_NOT_PENZ_ONLY.Sum(ptrans => ptrans.R_Mio1stup );
      /* 04 */   placaSumRow_NoUg_WoPENZ.X_rMio2stup   = ptransTable_NOT_PENZ_ONLY.Sum(ptrans => ptrans.R_Mio2stup );
      /* 05 */   placaSumRow_NoUg_WoPENZ.X_rZdrNa      = ptransTable_NOT_PENZ_ONLY.Sum(ptrans => ptrans.R_ZdrNa    );
      /* 06 */   placaSumRow_NoUg_WoPENZ.X_rZorNa      = ptransTable_NOT_PENZ_ONLY.Sum(ptrans => ptrans.R_ZorNa    );
      /* 07 */   placaSumRow_NoUg_WoPENZ.X_rZpiUk      = ptransTable_NOT_PENZ_ONLY.Sum(ptrans => ptrans.R_ZpiUk    );
      /* 08 */   placaSumRow_NoUg_WoPENZ.X_rDoprAll    = ptransTable_NOT_PENZ_ONLY.Sum(ptrans => ptrans.R_DoprAll  );
      /* 09 */   placaSumRow_NoUg_WoPENZ.X_rDohodak    = ptransTable_NOT_PENZ_ONLY.Sum(ptrans => ptrans.R_Dohodak  );
      /* 10 */   placaSumRow_NoUg_WoPENZ.X_rPorOsnAll  = ptransTable_NOT_PENZ_ONLY.Sum(ptrans => ptrans.R_PorOsnAll);
      /* 11 */   placaSumRow_NoUg_WoPENZ.X_rPorezAll   = ptransTable_NOT_PENZ_ONLY.Sum(ptrans => ptrans.R_PorezAll );
      /* 12 */   placaSumRow_NoUg_WoPENZ.X_rPrirez     = ptransTable_NOT_PENZ_ONLY.Sum(ptrans => ptrans.R_Prirez   );
      /* 13 */   placaSumRow_NoUg_WoPENZ.X_rPorPrirez  = ptransTable_NOT_PENZ_ONLY.Sum(ptrans => ptrans.R_PorPrirez);
      /* 14 */   placaSumRow_NoUg_WoPENZ.X_rDoprIz     = ptransTable_NOT_PENZ_ONLY.Sum(ptrans => ptrans.R_DoprIz   );

                 placaSumRow_NoUg_WoPENZ.X_personCount = ptransTable_NOT_PENZ_ONLY.Select(ptrans => new { ptrans.t_personCD }).Distinct().Count();

      #endregion RowAtIndex[3] ONLY TT_NADZORODBOR or TT_UGOVORODJELU

      #region RowAtIndex[4] ONLY ptrans.t_spc == "U" (Penzici)

    //var ptransTable_PENZ_ONLY = ptransTable.Where(ptrans => ptrans.t_spc == (byte)Ptrans.SpecEnum.PENZ                                        ).ToArray();
      var ptransTable_PENZ_ONLY = ptransTable.Where(ptrans => ptrans.t_spc == (byte)Ptrans.SpecEnum.PENZ || ptrans.t_tt == Placa.TT_IDD_KOLONA_4).ToArray();

      DS_Placa.placaSumRow placaSumRow_PENZ = (DS_Placa.placaSumRow)placaSumTable.Rows.Add();

      /* 01 */   placaSumRow_PENZ.X_rTheBruto   = ptransTable_PENZ_ONLY.Sum(ptrans => ptrans.R_TheBruto );
      /* 02 */   placaSumRow_PENZ.X_rMioOsn     = ptransTable_PENZ_ONLY.Sum(ptrans => ptrans.R_MioOsn   );
      /* 03 */   placaSumRow_PENZ.X_rMio1stup   = ptransTable_PENZ_ONLY.Sum(ptrans => ptrans.R_Mio1stup );
      /* 04 */   placaSumRow_PENZ.X_rMio2stup   = ptransTable_PENZ_ONLY.Sum(ptrans => ptrans.R_Mio2stup );
      /* 05 */   placaSumRow_PENZ.X_rZdrNa      = ptransTable_PENZ_ONLY.Sum(ptrans => ptrans.R_ZdrNa    );
      /* 06 */   placaSumRow_PENZ.X_rZorNa      = ptransTable_PENZ_ONLY.Sum(ptrans => ptrans.R_ZorNa    );
      /* 07 */   placaSumRow_PENZ.X_rZpiUk      = ptransTable_PENZ_ONLY.Sum(ptrans => ptrans.R_ZpiUk    );
      /* 08 */   placaSumRow_PENZ.X_rDoprAll    = ptransTable_PENZ_ONLY.Sum(ptrans => ptrans.R_DoprAll  );
      /* 09 */   placaSumRow_PENZ.X_rDohodak    = ptransTable_PENZ_ONLY.Sum(ptrans => ptrans.R_Dohodak  );
      /* 10 */   placaSumRow_PENZ.X_rPorOsnAll  = ptransTable_PENZ_ONLY.Sum(ptrans => ptrans.R_PorOsnAll);
      /* 11 */   placaSumRow_PENZ.X_rPorezAll   = ptransTable_PENZ_ONLY.Sum(ptrans => ptrans.R_PorezAll );
      /* 12 */   placaSumRow_PENZ.X_rPrirez     = ptransTable_PENZ_ONLY.Sum(ptrans => ptrans.R_Prirez   );
      /* 13 */   placaSumRow_PENZ.X_rPorPrirez  = ptransTable_PENZ_ONLY.Sum(ptrans => ptrans.R_PorPrirez);
      /* 14 */   placaSumRow_PENZ.X_rDoprIz     = ptransTable_PENZ_ONLY.Sum(ptrans => ptrans.R_DoprIz   );

                 placaSumRow_PENZ.X_personCount = ptransTable_PENZ_ONLY.Select(ptrans => new { ptrans.t_personCD }).Distinct().Count();

      #endregion RowAtIndex[4] ONLY ptrans.t_spc == "U" (Penzici)

      #region RowAtIndex[5] ONLY TT_AUTORHONOR

      var placaTable_AH_ONLY = placaTable.Where(placa => placa.tt == Placa.TT_AUTORHONOR  || 
                                                         placa.tt == Placa.TT_AUTORHONUMJ || 
                                                         placa.tt == Placa.TT_AHSAMOSTUMJ ||
                                                         placa.tt == Placa.TT_AUVECASTOPA ||
                                                         placa.tt == Placa.TT_DDBEZDOPRINO||
                                                         placa.tt == Placa.TT_NR2_P01NEDOP||
                                                         placa.tt == Placa.TT_NR1_PX1NEDOP
                                                         ).ToArray();

      DS_Placa.placaSumRow placaSumRow_AH = (DS_Placa.placaSumRow)placaSumTable.Rows.Add();

      /* 01 */   placaSumRow_AH.X_rTheBruto   = placaTable_AH_ONLY.Sum(placa => placa.S_rTheBruto  );
      /* 02 */   placaSumRow_AH.X_rAHizdatak  = placaTable_AH_ONLY.Sum(placa => placa.S_rAHizdatak );
      /* 03 */   placaSumRow_AH.X_rDohodak    = placaTable_AH_ONLY.Sum(placa => placa.S_rDohodak  );
      /* 04 */   placaSumRow_AH.X_rPorOsnAll  = placaTable_AH_ONLY.Sum(placa => placa.S_rPorOsnAll);
      /* 05 */   placaSumRow_AH.X_rPorezAll   = placaTable_AH_ONLY.Sum(placa => placa.S_rPorezAll );
      /* 06 */   placaSumRow_AH.X_rPrirez     = placaTable_AH_ONLY.Sum(placa => placa.S_rPrirez   );
      /* 07 */   placaSumRow_AH.X_rPorPrirez  = placaTable_AH_ONLY.Sum(placa => placa.S_rPorPrirez);

      personCount = placaTable_AH_ONLY.Join(ptransTable,
         placa => placa.recID, ptrans => ptrans.t_parentID, (placa, ptrans) => new { ptrans.t_personCD }).Distinct().Count();

      placaSumRow_AH.X_personCount = personCount;

      #endregion RowAtIndex[5] ONLY TT_AUTORHONOR

      #region RowAtIndex[6] ONLY REDOVAN RAD + PODUZETNICKA

      var placaTable_RR_ONLY = placaTable.Where(placa => placa.tt == Placa.TT_REDOVANRAD   ||
                                                         placa.tt == Placa.TT_PLACAUNARAVI || 
                                                         placa.tt == Placa.TT_OSTALIPRIM   ||  //04.12.2024.
                                                         placa.tt == Placa.TT_PODUZETPLACA).ToArray();

      DS_Placa.placaSumRow placaSumRow_RR = (DS_Placa.placaSumRow)placaSumTable.Rows.Add();

      /* 01 */ placaSumRow_RR.X_rKrizPorOsn = placaTable_RR_ONLY.Sum(placa => placa.S_rKrizPorOsn);
      /* 02 */ placaSumRow_RR.X_rKrizPorUk  = placaTable_RR_ONLY.Sum(placa => placa.S_rKrizPorUk );

      prsnCount_KrPr = placaTable_RR_ONLY.Join(ptransTable.Where(ptr => ptr.R_KrizPorUk.NotZero()),
         placa => placa.recID, ptrans => ptrans.t_parentID, (placa, ptrans) => new { ptrans.t_personCD }).Distinct().Count();

      placaSumRow_RR.X_prsnCount_KrPr = prsnCount_KrPr;

      #endregion RowAtIndex[6] ONLY REDOVAN RAD + PODUZETNICKA

      #region RowAtIndex[7] ONLY OTHER DOHODAK (AH+UG+NO+AU+K4+TV)
                                                                                                     
      var placaTable_OtherDohodak_ONLY = placaTable.Where(placa => placa.tt == Placa.TT_AUTORHONOR   ||
                                                                   placa.tt == Placa.TT_AUTORHONUMJ  ||
                                                                   placa.tt == Placa.TT_AHSAMOSTUMJ  ||
                                                                   placa.tt == Placa.TT_NADZORODBOR  ||
                                                                   placa.tt == Placa.TT_TURSITVIJECE ||
                                                                   placa.tt == Placa.TT_IDD_KOLONA_4 ||
                                                                   placa.tt == Placa.TT_SEZZAPPOLJOP ||
                                                                   placa.tt == Placa.TT_UGOVORODJELU ||
                                                                   placa.tt == Placa.TT_DDBEZDOPRINO ||
                                                                   placa.tt == Placa.TT_AUVECASTOPA  ||
                                                                   placa.tt == Placa.TT_NR1_PX1NEDOP ||
                                                                   placa.tt == Placa.TT_NR2_P01NEDOP ||
                                                                   placa.tt == Placa.TT_NR3_PX1DADOP
                                                                   ).ToArray();

      DS_Placa.placaSumRow placaSumRow_OD = (DS_Placa.placaSumRow)placaSumTable.Rows.Add();

      /* 01 */ placaSumRow_OD.X_rKrizPorOsn = placaTable_OtherDohodak_ONLY.Sum(placa => placa.S_rKrizPorOsn);
      /* 02 */ placaSumRow_OD.X_rKrizPorUk  = placaTable_OtherDohodak_ONLY.Sum(placa => placa.S_rKrizPorUk );

      prsnCount_KrPr = placaTable_OtherDohodak_ONLY.Join(ptransTable.Where(ptr => ptr.R_KrizPorUk.NotZero()),
         placa => placa.recID, ptrans => ptrans.t_parentID, (placa, ptrans) => new { ptrans.t_personCD }).Distinct().Count();

      placaSumRow_OD.X_prsnCount_KrPr = prsnCount_KrPr;

      #endregion RowAtIndex[7] ONLY OTHER DOHODAK (AH+UG+NO)

      #region placaSumRow_OK_ONLY.X_prsnCount_IDD (podatak za RowAtIndex[0] ali tek kada izracuna RowAtIndex[2, 3, 4, ...]

                 placaSumRow_OK_ONLY.X_prsnCount_IDD =

                 placaSumRow_NoUg_WoPENZ.X_personCount +
                 placaSumRow_PENZ       .X_personCount +
                 placaSumRow_AH         .X_personCount;

      #endregion placaSumRow_OK_ONLY.X_prsnCount_IDD (podatak za RowAtIndex[0] ali tek kada izracuna RowAtIndex[2, 3, 4, ...]

      #region RowAtIndex[8] ONLY TT_NADZORODBOR

      var placaTable_NO_ONLY = placaTable.Where(placa => placa.tt == Placa.TT_NADZORODBOR).ToArray();

      DS_Placa.placaSumRow placaSumRow_NO = (DS_Placa.placaSumRow)placaSumTable.Rows.Add();

      /* 01 */ placaSumRow_NO.X_rPorPrirez  = placaTable_NO_ONLY.Sum(placa => placa.S_rPorPrirez);

      #endregion RowAtIndex[8] ONLY TT_NADZORODBOR

      #region RowAtIndex[9] ONLY TT_UGOVORODJELU

      var placaTable_UG_ONLY = placaTable.Where(placa => placa.tt == Placa.TT_UGOVORODJELU).ToArray();

      DS_Placa.placaSumRow placaSumRow_UG = (DS_Placa.placaSumRow)placaSumTable.Rows.Add();

      /* 01 */ placaSumRow_UG.X_rPorPrirez  = placaTable_UG_ONLY.Sum(placa => placa.S_rPorPrirez);

      #endregion RowAtIndex[9] ONLY TT_UGOVORODJELU

      #region RowAtIndex[10] ONLY TT_IDD_KOLONA_4

      var placaTable_K4_ONLY = placaTable.Where(placa => placa.tt == Placa.TT_IDD_KOLONA_4).ToArray();

      DS_Placa.placaSumRow placaSumRow_K4 = (DS_Placa.placaSumRow)placaSumTable.Rows.Add();

      /* 01 */
      placaSumRow_K4.X_rPorPrirez = placaTable_K4_ONLY.Sum(placa => placa.S_rPorPrirez);

      #endregion RowAtIndex[10] ONLY TT_IDD_KOLONA_4

      #region JOPPD_SUME

    //  #region RowAtIndex[11] JOPPD data for placaTT 'RR'

    //  DS_Placa.placaRow[] placaTable_JOPPD_ONLY_RR;

    //  placaTable_JOPPD_ONLY_RR = placaTable.Where(placa => placa.tt == Placa.TT_REDOVANRAD).ToArray();

    //  DS_Placa.placaSumRow placaSumRow_JOPPD_ONLY_RR = (DS_Placa.placaSumRow)placaSumTable.Rows.Add();

    //  placaSumRow_JOPPD_ONLY_RR.X_rMio1stup   = placaTable_JOPPD_ONLY_RR.Sum(placa => placa.S_rMio1stup  );
    //  placaSumRow_JOPPD_ONLY_RR.X_rMio2stup   = placaTable_JOPPD_ONLY_RR.Sum(placa => placa.S_rMio2stup  );
    //  placaSumRow_JOPPD_ONLY_RR.X_rPorPrirez  = placaTable_JOPPD_ONLY_RR.Sum(placa => placa.S_rPorPrirez );
    //  placaSumRow_JOPPD_ONLY_RR.X_rZdrNa      = placaTable_JOPPD_ONLY_RR.Sum(placa => placa.S_rZdrNa     );
    //  placaSumRow_JOPPD_ONLY_RR.X_rZorNa      = placaTable_JOPPD_ONLY_RR.Sum(placa => placa.S_rZorNa     );

    //  placaSumRow_JOPPD_ONLY_RR.X_rZapNa      = placaTable_JOPPD_ONLY_RR.Sum(placa => placa.S_rZapNa     );
    //  placaSumRow_JOPPD_ONLY_RR.X_rZapII      = placaTable_JOPPD_ONLY_RR.Sum(placa => placa.S_rZapII     );

    //  #endregion RowAtIndex[11] JOPPD data for placaTT 'RR'

    //  #region RowAtIndex[12] JOPPD data for placaTT 'PP'

    //  DS_Placa.placaRow[] placaTable_JOPPD_ONLY_PP;

    //  placaTable_JOPPD_ONLY_PP = placaTable.Where(placa => placa.tt == Placa.TT_PODUZETPLACA).ToArray();

    //  DS_Placa.placaSumRow placaSumRow_JOPPD_ONLY_PP = (DS_Placa.placaSumRow)placaSumTable.Rows.Add();

    //  placaSumRow_JOPPD_ONLY_PP.X_rMio1stup   = placaTable_JOPPD_ONLY_PP.Sum(placa => placa.S_rMio1stup  );
    //  placaSumRow_JOPPD_ONLY_PP.X_rMio2stup   = placaTable_JOPPD_ONLY_PP.Sum(placa => placa.S_rMio2stup  );
    //  placaSumRow_JOPPD_ONLY_PP.X_rPorPrirez  = placaTable_JOPPD_ONLY_PP.Sum(placa => placa.S_rPorPrirez );
    //  placaSumRow_JOPPD_ONLY_PP.X_rZdrNa      = placaTable_JOPPD_ONLY_PP.Sum(placa => placa.S_rZdrNa     );
    //  placaSumRow_JOPPD_ONLY_PP.X_rZorNa      = placaTable_JOPPD_ONLY_PP.Sum(placa => placa.S_rZorNa     );

    //  #endregion RowAtIndex[12] JOPPD data for placaTT 'PP'

    //  #region RowAtIndex[13] JOPPD data for placaTT 'DrugiDohodci'

    //  DS_Placa.placaRow[] placaTable_JOPPD_ONLY_DD;

    //  placaTable_JOPPD_ONLY_DD = placaTable.Where(placa => placa.tt == Placa.TT_AUTORHONOR   ||
    //                                                       placa.tt == Placa.TT_AUTORHONUMJ  ||
    //                                                       placa.tt == Placa.TT_NADZORODBOR  ||
    //                                                       placa.tt == Placa.TT_IDD_KOLONA_4 ||
    //                                                       placa.tt == Placa.TT_UGOVORODJELU).ToArray();

    //  DS_Placa.placaSumRow placaSumRow_JOPPD_ONLY_DD = (DS_Placa.placaSumRow)placaSumTable.Rows.Add();

    //  placaSumRow_JOPPD_ONLY_DD.X_rMio1stup   = placaTable_JOPPD_ONLY_DD.Sum(placa => placa.S_rMio1stup  );
    //  placaSumRow_JOPPD_ONLY_DD.X_rMio2stup   = placaTable_JOPPD_ONLY_DD.Sum(placa => placa.S_rMio2stup  );
    //  placaSumRow_JOPPD_ONLY_DD.X_rPorPrirez  = placaTable_JOPPD_ONLY_DD.Sum(placa => placa.S_rPorPrirez );
    //  placaSumRow_JOPPD_ONLY_DD.X_rZdrNa      = placaTable_JOPPD_ONLY_DD.Sum(placa => placa.S_rZdrNa     );
    ////placaSumRow_JOPPD_ONLY_DD.X_rZorNa      = placaTable_JOPPD_ONLY_DD.Sum(placa => placa.S_rZorNa     );

    //  #endregion RowAtIndex[13] JOPPD data for placaTT 'DrugiDohodci'

      #endregion JOPPD_SUME

      #region JOPPD Obrazac additions

      if(this is RptP_JOPPD)
      {
         placaSumRow_OK_ONLY.X_vrstaObr = placa_rec.VrstaObr;

         //foreach(DS_Placa.IzvjTableRow ptransRow in ptransTable.Rows)
         //{
         //   SetJPD_B_strana(placa_rec.Prules, jpdBstranaTable, ptransRow, ptraneTable, personTable);
         //}

         // 21.04.2016. premjestila tu
         jpdStrA.vrstaIzvj = placa_rec.VrstaJOPPD.IsEmpty() ? "1" : placa_rec.VrstaJOPPD;

         bool isKonacniObrPor_dodan = false; // hendlamo da se redak konacnog obracuna pojavi samo jednom po covjeku (a ako uopce treba) 
         uint currPersonCD          =     0; // hendlamo da se redak konacnog obracuna pojavi samo jednom po covjeku (a ako uopce treba) 

         if(ptransTable.Rows.Count.NotZero())
         {
            currPersonCD = ((DS_Placa.IzvjTableRow)ptransTable.Rows[0]).t_personCD;
         }

         for(int pRowIdx = 0; pRowIdx < ptransTable.Rows.Count; ++pRowIdx)
         {
            if(currPersonCD != ((DS_Placa.IzvjTableRow)ptransTable.Rows[pRowIdx]).t_personCD) // novi person 
            {
               currPersonCD = ((DS_Placa.IzvjTableRow)ptransTable.Rows[pRowIdx]).t_personCD;
               isKonacniObrPor_dodan = false;
            }

            SetJPD_B_strana(placa_rec.Prules, jpdBstranaTable, /*ptransRow*/(DS_Placa.IzvjTableRow)ptransTable.Rows[pRowIdx], ptraneTable, personTable, jpdStrA.vrstaIzvj, ref isKonacniObrPor_dodan);

         } // for(int pRowIdx = 0; pRowIdx < ptransTable.Rows.Count; ++pRowIdx)

         //2024
         DateTime zaMMYY_asDateTime = Placa.GetDateTimeFromMMYYYY(placaSumRow_OK_ONLY.X_zaMMYYYY, false);
         bool is_posInval_DO_1123 = zaMMYY_asDateTime < ZXC.Date01122023;
          
         jpdStrA.dateIzvj   = placa_rec.DokDate                    ;
         jpdStrA.oznakaIzvj = placa_rec.RSm_ID                     ;
         
         // 10.02.2016: 
         if(RptFilter.ShowVirDatePod) // prisiljeni DokDate & joppdBroj za 
         {
            jpdStrA.dateIzvj      = RptFilter.VirDatePod ;
            jpdStrA.oznakaIzvj    = RptFilter.ForcedRSmID;
         }

         //jpdStrA.vrstaIzvj  = placa_rec.VrstaJOPPD.IsEmpty() ? "1" : placa_rec.VrstaJOPPD;
         jpdStrA.piNaziv    = ZXC.CURR_prjkt_rec.Naziv             ;
         jpdStrA.piMjesto   = ZXC.CURR_prjkt_rec.Grad              ;
         jpdStrA.piUlica    = ZXC.CURR_prjkt_rec.UlicaBezBroja_1   ;
         jpdStrA.piUlicaBr  = ZXC.CURR_prjkt_rec.UlicniBroj_1      ;
         jpdStrA.piEmail    = ZXC.CURR_prjkt_rec.Email             ;
         jpdStrA.piOIB      = ZXC.CURR_prjkt_rec.Oib               ;
       //jpdStrA.piOznaka   =                                           ZXC.CURR_prjkt_rec.IsObrt ? "2" : "1"; 
         jpdStrA.piOznaka   = ZXC.CURR_prjkt_rec.IsFizickaOsoba ? "4" : ZXC.CURR_prjkt_rec.IsObrt ? "2" : "1"; 
         jpdStrA.opNaziv    = ZXC.CURR_prjkt_rec.Naziv             ;
         jpdStrA.opMjesto   = ZXC.CURR_prjkt_rec.Grad              ;
         jpdStrA.opUlica    = ZXC.CURR_prjkt_rec.UlicaBezBroja_1   ;
         jpdStrA.opUlicaBr  = ZXC.CURR_prjkt_rec.UlicniBroj_1      ;
         jpdStrA.opEmail    = ZXC.CURR_prjkt_rec.Email             ;
         jpdStrA.opOIB      = ZXC.CURR_prjkt_rec.Oib               ;
         jpdStrA.brojOsoba  = jpdBstranaTable.Select(row => row.b_ime).Distinct().Count();
         jpdStrA.brojRedaka = jpdBstranaTable.Rows.Count;

         jpdStrA.porP11 = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_REDOVANRAD   ||
                                                           jpdBrow.b_tt == Placa.TT_OSTALIPRIM   ||
                                                           jpdBrow.b_tt == Placa.TT_PLACAUNARAVI ||
                                                           jpdBrow.b_tt == Placa.TT_BIVSIRADNIK  || //23.12.2019.
                                                           jpdBrow.b_tt == Placa.TT_PODUZETPLACA).Sum(jpdBrow => jpdBrow.b_PorPrir);
         jpdStrA.porP12 = 0.00M;
         jpdStrA.porP01 = jpdStrA.porP11 + jpdStrA.porP12;
         jpdStrA.porP02 = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_POREZNADOBIT).Sum(jpdBrow => jpdBrow.b_PorPrir);  
         jpdStrA.porP03 = 0.00M;
         jpdStrA.porP04 = 0.00M;
         jpdStrA.porP05 = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_AUTORHONOR   ||
                                                           jpdBrow.b_tt == Placa.TT_AUTORHONUMJ  ||
                                                           jpdBrow.b_tt == Placa.TT_AHSAMOSTUMJ  || //od 01.01.2017.
                                                           jpdBrow.b_tt == Placa.TT_NADZORODBOR  ||
                                                           jpdBrow.b_tt == Placa.TT_TURSITVIJECE ||
                                                           jpdBrow.b_tt == Placa.TT_IDD_KOLONA_4 ||
                                                           jpdBrow.b_tt == Placa.TT_SEZZAPPOLJOP ||
                                                           jpdBrow.b_tt == Placa.TT_UGOVORODJELU ||
                                                           jpdBrow.b_tt == Placa.TT_DDBEZDOPRINO ||  //12.2018.
                                                           jpdBrow.b_tt == Placa.TT_AUVECASTOPA  ||  //12.2018.
                                                           jpdBrow.b_tt == Placa.TT_NR1_PX1NEDOP ||  //12.2018.
                                                           jpdBrow.b_tt == Placa.TT_NR2_P01NEDOP ||  //12.2018.
                                                           jpdBrow.b_tt == Placa.TT_NR3_PX1DADOP     //12.2018.
                                                           ).Sum(jpdBrow => jpdBrow.b_PorPrir);

         jpdStrA.mio1P01 = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_REDOVANRAD   ||
                                                            jpdBrow.b_tt == Placa.TT_OSTALIPRIM   ||
                                                            jpdBrow.b_tt == Placa.TT_STRUCNOOSPOS ||
                                                            jpdBrow.b_tt == Placa.TT_NEPLACDOPUST ||
                                                            jpdBrow.b_tt == Placa.TT_PLACAUNARAVI).Sum(jpdBrow => jpdBrow.b_Mio1stup);

         jpdStrA.mio1P02 = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_AUTORHONOR   ||
                                                            jpdBrow.b_tt == Placa.TT_AUTORHONUMJ  ||
                                                            jpdBrow.b_tt == Placa.TT_AHSAMOSTUMJ  || // od 01.01.2017.
                                                            jpdBrow.b_tt == Placa.TT_NADZORODBOR  ||
                                                            jpdBrow.b_tt == Placa.TT_TURSITVIJECE ||
                                                            jpdBrow.b_tt == Placa.TT_IDD_KOLONA_4 ||
                                                            jpdBrow.b_tt == Placa.TT_SEZZAPPOLJOP ||
                                                            jpdBrow.b_tt == Placa.TT_UGOVORODJELU ||
                                                            jpdBrow.b_tt == Placa.TT_AUVECASTOPA  ||  //12.2018.
                                                            jpdBrow.b_tt == Placa.TT_BIVSIRADNIK  ||  //23.12.2019.
                                                            jpdBrow.b_tt == Placa.TT_NR3_PX1DADOP     //12.2018.
                                                            ).Sum(jpdBrow => jpdBrow.b_Mio1stup);

         jpdStrA.mio1P03 = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_PODUZETPLACA).Sum(jpdBrow => jpdBrow.b_Mio1stup);
         jpdStrA.mio1P04 = 0.00M;
         jpdStrA.mio1P05 = 0.00M;
         jpdStrA.mio1P06 = jpdBstranaTable./*Where(jpdBrow => jpdBrow.b_tt == Placa.TT_PODUZETPLACA).*/Sum(jpdBrow => jpdBrow.b_Mio1stupNa);
         jpdStrA.mio1P07 = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_SAMODOPRINOS).Sum(jpdBrow => jpdBrow.b_Mio1stup); // 08.02.2016. dodano 
         jpdStrA.mio2P01 = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_REDOVANRAD   ||
                                                            jpdBrow.b_tt == Placa.TT_OSTALIPRIM   ||
                                                            jpdBrow.b_tt == Placa.TT_NEPLACDOPUST ||
                                                            jpdBrow.b_tt == Placa.TT_STRUCNOOSPOS ||
                                                            jpdBrow.b_tt == Placa.TT_PLACAUNARAVI).Sum(jpdBrow => jpdBrow.b_Mio2stup);

         jpdStrA.mio2P02 = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_AUTORHONOR   ||
                                                            jpdBrow.b_tt == Placa.TT_AUTORHONUMJ  ||
                                                            jpdBrow.b_tt == Placa.TT_AHSAMOSTUMJ  || // od 01.01.2017.
                                                            jpdBrow.b_tt == Placa.TT_NADZORODBOR  ||
                                                            jpdBrow.b_tt == Placa.TT_TURSITVIJECE ||
                                                            jpdBrow.b_tt == Placa.TT_IDD_KOLONA_4 ||
                                                            jpdBrow.b_tt == Placa.TT_SEZZAPPOLJOP ||
                                                            jpdBrow.b_tt == Placa.TT_UGOVORODJELU ||
                                                            jpdBrow.b_tt == Placa.TT_BIVSIRADNIK  ||  //23.12.2019.
                                                            jpdBrow.b_tt == Placa.TT_AUVECASTOPA  ||  //12.2018.
                                                            jpdBrow.b_tt == Placa.TT_NR3_PX1DADOP     //12.2018.
                                                            ).Sum(jpdBrow => jpdBrow.b_Mio2stup);

         jpdStrA.mio2P03 = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_PODUZETPLACA).Sum(jpdBrow => jpdBrow.b_Mio2stup);
         jpdStrA.mio2P04 = 0.00M;
         jpdStrA.mio2P05 = jpdBstranaTable./*Where(jpdBrow => jpdBrow.b_tt == Placa.TT_PODUZETPLACA).*/Sum(jpdBrow => jpdBrow.b_Mio2stupNa);
         jpdStrA.mio2P06 = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_SAMODOPRINOS).Sum(jpdBrow => jpdBrow.b_Mio2stup);// 08.02.2016. dodano 

         jpdStrA.zdrP01  = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_REDOVANRAD   ||
                                                            jpdBrow.b_tt == Placa.TT_OSTALIPRIM   ||
                                                            jpdBrow.b_tt == Placa.TT_NEPLACDOPUST ||
                                                            jpdBrow.b_tt == Placa.TT_STRUCNOOSPOS ||
                                                            jpdBrow.b_tt == Placa.TT_PLACAUNARAVI).Sum(jpdBrow => jpdBrow.b_ZdrNa);
         jpdStrA.zdrP02  = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_REDOVANRAD   ||
                                                            jpdBrow.b_tt == Placa.TT_OSTALIPRIM   ||
                                                            jpdBrow.b_tt == Placa.TT_NEPLACDOPUST ||
                                                            jpdBrow.b_tt == Placa.TT_STRUCNOOSPOS ||
                                                            jpdBrow.b_tt == Placa.TT_PLACAUNARAVI).Sum(jpdBrow => jpdBrow.b_ZorNa);
         jpdStrA.zdrP03  = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_PODUZETPLACA).Sum(jpdBrow => jpdBrow.b_ZdrNa);
         jpdStrA.zdrP04  = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_PODUZETPLACA).Sum(jpdBrow => jpdBrow.b_ZorNa);
         jpdStrA.zdrP05  = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_AUTORHONOR   ||
                                                            jpdBrow.b_tt == Placa.TT_AUTORHONUMJ  ||
                                                            jpdBrow.b_tt == Placa.TT_AHSAMOSTUMJ  || // od 01.01.2017.
                                                            jpdBrow.b_tt == Placa.TT_NADZORODBOR  ||
                                                            jpdBrow.b_tt == Placa.TT_TURSITVIJECE ||
                                                            jpdBrow.b_tt == Placa.TT_IDD_KOLONA_4 ||
                                                            jpdBrow.b_tt == Placa.TT_SEZZAPPOLJOP ||
                                                            jpdBrow.b_tt == Placa.TT_UGOVORODJELU ||
                                                            jpdBrow.b_tt == Placa.TT_BIVSIRADNIK  ||  //23.12.2019.
                                                            jpdBrow.b_tt == Placa.TT_AUVECASTOPA  ||  //12.2018.
                                                            jpdBrow.b_tt == Placa.TT_NR3_PX1DADOP     //12.2018.
                                                            ).Sum(jpdBrow => jpdBrow.b_ZdrNa);
         jpdStrA.zdrP06  = jpdBstranaTable./*Where(jpdBrow => jpdBrow.b_tt == Placa.TT_PODUZETPLACA).*/Sum(jpdBrow => jpdBrow.b_ZpiUk);
         jpdStrA.zdrP07  = 0.00M;
         jpdStrA.zdrP08  = 0.00M;
         jpdStrA.zdrP09  = 0.00M;
         jpdStrA.zdrP10  = 0.00M;
         jpdStrA.zdrP11 = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_SAMODOPRINOS).Sum(jpdBrow => jpdBrow.b_ZdrNa); // 08.02.2016. dodano 
         jpdStrA.zdrP12 = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_SAMODOPRINOS).Sum(jpdBrow => jpdBrow.b_ZorNa); // 08.02.2016. dodano 
       // 07.03.2014. dodan novi redak jpdStrA.zapP03 te se razdvaja PP i RR doprinosi za zaposljavanje
       //jpdStrA.zapP01  = jpdBstranaTable./*Where(jpdBrow => jpdBrow.b_tt == Placa.TT_PODUZETPLACA).*/Sum(jpdBrow => jpdBrow.b_ZapNa  );
       //jpdStrA.zapP02  = jpdBstranaTable./*Where(jpdBrow => jpdBrow.b_tt == Placa.TT_PODUZETPLACA).*/Sum(jpdBrow => jpdBrow.b_ZapII  );
         jpdStrA.zapP01  = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt != Placa.TT_PODUZETPLACA && 
                                                            jpdBrow.b_tt != Placa.TT_SAMODOPRINOS).Sum(jpdBrow => jpdBrow.b_ZapNa);
       // od za 122023 promjene b_ZapII je Mio1Olak
       //jpdStrA.zapP02  = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt != Placa.TT_PODUZETPLACA).Sum(jpdBrow => jpdBrow.b_ZapII);
         jpdStrA.zapP02  = is_posInval_DO_1123 ? jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt != Placa.TT_PODUZETPLACA).Sum(jpdBrow => jpdBrow.b_ZapII) : 0.00M;


         jpdStrA.zapP03  = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_PODUZETPLACA).Sum(jpdBrow => jpdBrow.b_ZapNa  ); // 06.03.2014.dodano
         jpdStrA.zapP04  = jpdBstranaTable.Where(jpdBrow => jpdBrow.b_tt == Placa.TT_SAMODOPRINOS).Sum(jpdBrow => jpdBrow.b_ZapNa  ); // 08.02.2016.dodano
         jpdStrA.npNetto = jpdBstranaTable./*Where(jpdBrow => jpdBrow.b_tt == Placa.TT_PODUZETPLACA).*/Sum(jpdBrow => jpdBrow.b_NetoAdd);
         jpdStrA.ktaMO2  = 0.00M; 
         jpdStrA.sastIme = ZXC.CURR_prjkt_rec.Ime;
         jpdStrA.sastPrz = ZXC.CURR_prjkt_rec.Prezime;

         // od 28.02.2015.
         jpdStrA.porP06  =  0.00M; // porez na kamate - banke
         //jpdStrA.mio1P07 =  0.00M; // od 01.01.2016. po osnovi obavljanja samostalne djelatnosti  // 08.02.2016. stavljeno u upotrebu
         //jpdStrA.mio2P06 =  0.00M; // od 01.01.2016. po osnovi obavljanja samostalne djelatnosti  // 08.02.2016. stavljeno u upotrebu
         //jpdStrA.zdrP11  =  0.00M; // od 01.01.2016. po osnovi obavljanja samostalne djelatnosti  // 08.02.2016. stavljeno u upotrebu
         //jpdStrA.zdrP12  =  0.00M; // od 01.01.2016. po osnovi obavljanja samostalne djelatnosti  // 08.02.2016. stavljeno u upotrebu
         //jpdStrA.zapP04  =  0.00M; // od 01.01.2016. po osnovi obavljanja samostalne djelatnosti  // 08.02.2016. stavljeno u upotrebu
         jpdStrA.nerezid =  0.00M; // UkupniNeoporeziviPrimici (nerezidenata u neprofitnim organizacijama)
         jpdStrA.nzInvP1 =  0;     // porez na nezaposljavanje invalida
         jpdStrA.nzInvP2 =  0.00M; // porez na nezaposljavanje invalida
      }

      #endregion jpd Obrazac additions

      #endregion X_SUME

      #region P_SUME

      DS_Placa.ptransSumRow ptransSumRow;

    //foreach(var person in persDistPtrList)
      for(int p=0; p < persDistPtrList.Count; ++p)
      {
         #region Za IP Kartice 

         if(this is RptP_IP_Kartica )
         {
            for(int uMM = 1; uMM <= 12; ++uMM) // u mjesecu (MM komponenta t_dokDate-a 
            {
               ptransSumRow = (DS_Placa.ptransSumRow)ptransSumTable.Rows.Add(persDistPtrList[p].t_personCD, persDistPtrList[p].t_ime, persDistPtrList[p].t_prezime, "", uMM);

               ptransSumRow.P_uYYYY = ptransTable.FirstOrDefault(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).t_dokDate.Year;

               ptransSumRow.P_ipIdentif = (ptransTable.FirstOrDefault(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).t_koef < 0.5M ? "2" : "1");

             //var ptransRowOfThisPersonInThisMonth = ptransTable.FirstOrDefault(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI ||                                       ptrans.t_tt == Placa.TT_PODUZETPLACA));
               var ptransRowOfThisPersonInThisMonth = ptransTable.FirstOrDefault(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA));

               if(ptransRowOfThisPersonInThisMonth != null && ptransRowOfThisPersonInThisMonth.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca)
               {
                  ptransSumRow.P_opcCD = ptransRowOfThisPersonInThisMonth.t_opcCD;
               }

               var personRow = personTable.SingleOrDefault(prsn => prsn.personCD == persDistPtrList[p].t_personCD);
               if(personRow != null) ptransSumRow.P_oib = personRow.oib;

               if(uMM == 12 && rptFilter.HocuKonacniObrPor)
               {
                  // 1. alfa13 = dosadasnja suma 12 mjeseci
                  // 2. beta14 = suma 'konacni obracun' (sta bi bilo da je bilo, kako je trebalo biti) 
                  // 3. gama15 = beta14 - alfa13: razlika po konacnom obracunu 
                  // 4. sumiraj gama15 u ovaj redak 12-og mjeseca 
                  SetKonacniObracun_Alfa13Beta14Gama15_PtransRecs(persDistPtrList[p].t_personCD, placa_rec.Prules, personRow);

                  if(PtransSumRec_alfa13 != null)
                  {
                     if(ZXC.AlmostEqual(PtransSumRec_gama15.R_Netto.Ron2(), 0M, rptFilter.TolerancijaPoKO) == false)
                     {
                        DS_Placa.IzvjTableRow ptransRow_gama15 = (DS_Placa.IzvjTableRow)ptransTable.Rows.Add();

                        PtransSumRec_gama15.T_dokDate = ZXC.projectYearLastDay;
                        PtransDao.FillTypedDataRowResults(ptransRow_gama15, PtransSumRec_gama15, /*ptraneRowsOfThisPerson*/null, /*placa_rec*/ null);
                     }
                  }
               }

               /* 01 */ ptransSumRow.P_TheBruto    = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_TheBruto   );
               /* 02 */ ptransSumRow.P_DoprIz      = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_DoprIz     );
               /* 03 */ ptransSumRow.P_Premije     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_Premije    );
               /* 04 */ ptransSumRow.P_Dohodak     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_Dohodak    );
               /* 05 */ ptransSumRow.P_Odbitak     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_Odbitak    );
               /* 06 */ ptransSumRow.P_PorOsnAll   = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_PorOsnAll  );
               /* 07 */ ptransSumRow.P_PorPrirez   = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_PorPrirez  );
               /* 08 */ ptransSumRow.P_KrizPorUk   = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_KrizPorUk  );
               /* 09 */ ptransSumRow.P_NettoAftKrp = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_NettoAftKrp);

               //if(uMM == 12 && rptFilter.HocuKonacniObrPor)
               //{
               //   // 1. alfa13 = dosadasnja suma 12 mjeseci
               //   // 2. beta14 = suma 'konacni obracun' (sta bi bilo da je bilo, kako je trebalo biti) 
               //   // 3. gama15 = beta14 - alfa13: razlika po konacnom obracunu 
               //   // 4. sumiraj gama15 u ovaj redak 12-og mjeseca 
               //   SetKonacniObracun_Alfa13Beta14Gama15_PtransRecs(persDistPtrList[p].t_personCD, placa_rec.Prules, personRow);

               //   if(PtransSumRec_alfa13 != null)
               //   {
               //      //DS_Placa.IzvjTableRow ptransRow_alfa13 = (DS_Placa.IzvjTableRow)ptransTable.Rows.Add();
               //      //DS_Placa.IzvjTableRow ptransRow_beta14 = (DS_Placa.IzvjTableRow)ptransTable.Rows.Add();
               //      DS_Placa.IzvjTableRow ptransRow_gama15 = (DS_Placa.IzvjTableRow)ptransTable.Rows.Add();

               //      //PtransDao.FillTypedDataRowResults(ptransRow_alfa13, PtransSumRec_alfa13, /*ptraneRowsOfThisPerson*/null, /*placa_rec*/ null);
               //      //PtransDao.FillTypedDataRowResults(ptransRow_beta14, PtransSumRec_beta14, /*ptraneRowsOfThisPerson*/null, /*placa_rec*/ null);
               //      PtransDao.FillTypedDataRowResults(ptransRow_gama15, PtransSumRec_gama15, /*ptraneRowsOfThisPerson*/null, /*placa_rec*/ null);
               //   }
               //}
            }

            // 01.12.2014: 13-ti redak "KONAČNI OBRAČUN POREZA" 

         }

         #endregion Za IP Kartice 

         #region RptP_GodObrPoreza

         else if(this is RptP_GodObrPoreza)
         {
            for(int uMM = 1; uMM <= 12; ++uMM) // u mjesecu (MM komponenta t_dokDate-a 
            {
               ptransSumRow = (DS_Placa.ptransSumRow)ptransSumTable.Rows.Add(persDistPtrList[p].t_personCD, persDistPtrList[p].t_ime, persDistPtrList[p].t_prezime, "", uMM);

               ptransSumRow.P_uYYYY = ptransTable.FirstOrDefault(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).t_dokDate.Year;

               ptransSumRow.P_ipIdentif = (ptransTable.FirstOrDefault(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).t_koef < 0.5M ? "2" : "1");

             //var ptransRowOfThisPersonInThisMonth = ptransTable.FirstOrDefault(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_PODUZETPLACA));
               var ptransRowOfThisPersonInThisMonth = ptransTable.FirstOrDefault(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_PODUZETPLACA || ptrans.t_tt == Placa.TT_OSTALIPRIM));

               if(ptransRowOfThisPersonInThisMonth != null && ptransRowOfThisPersonInThisMonth.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca)
               {
                  ptransSumRow.P_opcCD = ptransRowOfThisPersonInThisMonth.t_opcCD;
               }

               var personRow = personTable.SingleOrDefault(prsn => prsn.personCD == persDistPtrList[p].t_personCD);
               if(personRow != null) ptransSumRow.P_oib = personRow.oib;

               /* 01 */ ptransSumRow.P_TheBruto    = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_TheBruto   );
               /* 02 */ ptransSumRow.P_DoprIz      = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_DoprIz     );
               /* 04 */ ptransSumRow.P_Dohodak     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_Dohodak    );
               /* 05 */ ptransSumRow.P_Odbitak     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_Odbitak    );
               /* 06 */ ptransSumRow.P_PorOsnAll   = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_PorOsnAll  );
               /* 12 */ ptransSumRow.P_PorOsn1     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_PorOsn1    );
               /* 13 */ ptransSumRow.P_PorOsn2     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_PorOsn2    );
               /* 14 */ ptransSumRow.P_PorOsn3     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_PorOsn3    );
               /* 07 */ ptransSumRow.P_PorezAll    = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_PorezAll   );
               /* 08 */ ptransSumRow.P_Prirez      = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_Prirez     );
               /* 09 */ ptransSumRow.P_NettoAftKrp = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_dokDate.Month == uMM && (ptrans.t_tt == Placa.TT_REDOVANRAD || ptrans.t_tt == Placa.TT_PLACAUNARAVI || ptrans.t_tt == Placa.TT_OSTALIPRIM || ptrans.t_tt == Placa.TT_PODUZETPLACA) && ptrans.R_vrstaObr != Placa.VrObr_NeisplacenaPlaca).Sum(ptrans => ptrans.R_NettoAftKrp);

               if(uMM == 12)
               {
                  // 1. alfa13 = dosadasnja suma 12 mjeseci
                  // 2. beta14 = suma 'konacni obracun' (sta bi bilo da je bilo, kako je trebalo biti) 
                  // 3. gama15 = beta14 - alfa13: razlika po konacnom obracunu 
                  // 4. sumiraj gama15 u ovaj redak 12-og mjeseca 

                  SetKonacniObracun_Alfa13Beta14Gama15_PtransRecs(persDistPtrList[p].t_personCD, placa_rec.Prules, personRow);

                  if(PtransSumRec_alfa13 != null)
                  {
                     DS_Placa.IzvjTableRow ptransRow_alfa13 = (DS_Placa.IzvjTableRow)ptransTable.Rows.Add();
                     DS_Placa.IzvjTableRow ptransRow_beta14 = (DS_Placa.IzvjTableRow)ptransTable.Rows.Add();
                     DS_Placa.IzvjTableRow ptransRow_gama15 = (DS_Placa.IzvjTableRow)ptransTable.Rows.Add();
                     
                     PtransDao.FillTypedDataRowResults(ptransRow_alfa13, PtransSumRec_alfa13, /*ptraneRowsOfThisPerson*/null, /*placa_rec*/ null);
                     PtransDao.FillTypedDataRowResults(ptransRow_beta14, PtransSumRec_beta14, /*ptraneRowsOfThisPerson*/null, /*placa_rec*/ null);
                     PtransDao.FillTypedDataRowResults(ptransRow_gama15, PtransSumRec_gama15, /*ptraneRowsOfThisPerson*/null, /*placa_rec*/ null);
                  }
               } // if(uMM == 12)
            } // for(int uMM = 1; uMM <= 12; ++uMM) // u mjesecu (MM komponenta t_dokDate-a 
         } // else if(this is RptP_GodObrPoreza )

         #endregion RptP_GodObrPoreza
            
         #region Default cases

         else 
         {
            ptransSumRow = (DS_Placa.ptransSumRow)ptransSumTable.Rows.Add(persDistPtrList[p].t_personCD, persDistPtrList[p].t_ime, persDistPtrList[p].t_prezime);

            var personRow = personTable.SingleOrDefault(prsn => prsn.personCD == persDistPtrList[p].t_personCD);
            if(personRow != null) ptransSumRow.P_oib = personRow.oib;

            ptransSumRow.P_opcCD = ptransTable.FirstOrDefault(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).t_opcCD;
            
            ptransSumRow.P_koef  = ptransTable.FirstOrDefault(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).t_koef;
            
            ptransSumRow.P_uYYYY = ptransTable.FirstOrDefault(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).t_dokDate.Year;

            if(rptFilter.HocuKonacniObrPor)
            {
               // 1. alfa13 = dosadasnja suma 12 mjeseci
               // 2. beta14 = suma 'konacni obracun' (sta bi bilo da je bilo, kako je trebalo biti) 
               // 3. gama15 = beta14 - alfa13: razlika po konacnom obracunu 
               // 4. sumiraj gama15 u ovaj redak 12-og mjeseca 

               SetKonacniObracun_Alfa13Beta14Gama15_PtransRecs(persDistPtrList[p].t_personCD, placa_rec.Prules, personRow);

               // 04.12.2015: 
               bool nettoDiffJeVelik = true;

               if(PtransSumRec_alfa13 != null)
               {
                  if(ZXC.AlmostEqual(PtransSumRec_gama15.R_Netto.Ron2(), 0M, rptFilter.TolerancijaPoKO) == false) 
                  {
                     nettoDiffJeVelik = true;
                     //DS_Placa.IzvjTableRow ptransRow_alfa13 = (DS_Placa.IzvjTableRow)ptransTable.Rows.Add();
                     //DS_Placa.IzvjTableRow ptransRow_beta14 = (DS_Placa.IzvjTableRow)ptransTable.Rows.Add();
                       DS_Placa.IzvjTableRow ptransRow_gama15 = (DS_Placa.IzvjTableRow)ptransTable.Rows.Add();

                     //PtransDao.FillTypedDataRowResults(ptransRow_alfa13, PtransSumRec_alfa13, /*ptraneRowsOfThisPerson*/null, /*placa_rec*/ null);
                     //PtransDao.FillTypedDataRowResults(ptransRow_beta14, PtransSumRec_beta14, /*ptraneRowsOfThisPerson*/null, /*placa_rec*/ null);
                       PtransDao.FillTypedDataRowResults(ptransRow_gama15, PtransSumRec_gama15, /*ptraneRowsOfThisPerson*/null, /*placa_rec*/ null);
                  }
                  else
                  {
                     nettoDiffJeVelik = false;
                  }
               }

               #region Korigiraj X - sumu za ptransRow_gama15

               if(PtransSumRec_gama15 != null && nettoDiffJeVelik == true)
               {
                  /* 01 */   //placaSumRow_OK_ONLY.X_tBrutoOsn      += PtransSumRec_gama15.S_tBrutoOsn     ;
                  /* 02 */   //placaSumRow_OK_ONLY.X_tTopObrok      += PtransSumRec_gama15.S_tTopObrok     ;
                  /* 03 */   //placaSumRow_OK_ONLY.X_tDodBruto      += PtransSumRec_gama15.S_tDodBruto     ;
                  /* 04 */   //placaSumRow_OK_ONLY.X_tZivotno       += PtransSumRec_gama15.S_tZivotno      ;
                  /* 05 */   //placaSumRow_OK_ONLY.X_tDopZdr        += PtransSumRec_gama15.S_tDopZdr       ;
                  /* 06 */   //placaSumRow_OK_ONLY.X_tDobMIO        += PtransSumRec_gama15.S_tDobMIO       ;
                  /* 07 */   //placaSumRow_OK_ONLY.X_tNetoAdd       += PtransSumRec_gama15.S_tNetoAdd      ;
                  /* 08 */   //placaSumRow_OK_ONLY.X_tPrijevoz      += PtransSumRec_gama15.S_tPrijevoz     ;
                  /* 09 */   //placaSumRow_OK_ONLY.X_rBruto100      += PtransSumRec_gama15.S_rBruto100     ;
                  /* 10 */     placaSumRow_OK_ONLY.X_rTheBruto      += PtransSumRec_gama15.R_TheBruto      ;
                  /* 11 */   //placaSumRow_OK_ONLY.X_rMioOsn        += PtransSumRec_gama15.S_rMioOsn       ;
                  /* 12 */   //placaSumRow_OK_ONLY.X_rMio1stup      += PtransSumRec_gama15.S_rMio1stup     ;
                  /* 13 */   //placaSumRow_OK_ONLY.X_rMio2stup      += PtransSumRec_gama15.S_rMio2stup     ;
                  /* 14 */   //placaSumRow_OK_ONLY.X_rMioAll        += PtransSumRec_gama15.S_rMioAll       ;
                  /* 15 */     placaSumRow_OK_ONLY.X_rDoprIz        += PtransSumRec_gama15.R_DoprIz        ;
                  /* 16 */     placaSumRow_OK_ONLY.X_rOdbitak       += PtransSumRec_gama15.R_Odbitak       ;
                  /* 17 */   //placaSumRow_OK_ONLY.X_rPremije       += PtransSumRec_gama15.S_rPremije      ;
                  /* 18 */     placaSumRow_OK_ONLY.X_rDohodak       += PtransSumRec_gama15.R_Dohodak       ;
                  /* 19 */     placaSumRow_OK_ONLY.X_rPorOsnAll     += PtransSumRec_gama15.R_PorOsnAll     ;
                  /* 20 */     placaSumRow_OK_ONLY.X_rPorOsn1       += PtransSumRec_gama15.R_PorOsn1       ;
                  /* 21 */     placaSumRow_OK_ONLY.X_rPorOsn2       += PtransSumRec_gama15.R_PorOsn2       ;
                  /* 22 */     placaSumRow_OK_ONLY.X_rPorOsn3       += PtransSumRec_gama15.R_PorOsn3       ;
                  /* 23 */   //placaSumRow_OK_ONLY.X_rPorOsn4       += PtransSumRec_gama15.S_rPorOsn4      ;
                  /* 24 */     placaSumRow_OK_ONLY.X_rPor1uk        += PtransSumRec_gama15.R_Por1Uk        ;
                  /* 25 */     placaSumRow_OK_ONLY.X_rPor2uk        += PtransSumRec_gama15.R_Por2Uk        ;
                  /* 26 */     placaSumRow_OK_ONLY.X_rPor3uk        += PtransSumRec_gama15.R_Por3Uk        ;
                  /* 27 */   //placaSumRow_OK_ONLY.X_rPor4uk        += PtransSumRec_gama15.S_rPor4uk       ;
                  /* 28 */     placaSumRow_OK_ONLY.X_rPorezAll      += PtransSumRec_gama15.R_PorezAll      ;
                  /* 29 */     placaSumRow_OK_ONLY.X_rPrirez        += PtransSumRec_gama15.R_Prirez        ;
                  /* 30 */     placaSumRow_OK_ONLY.X_rPorPrirez     += PtransSumRec_gama15.R_PorPrirez     ;
                  /* 31 */     placaSumRow_OK_ONLY.X_rNetto         += PtransSumRec_gama15.R_Netto         ;
                  /* 32 */   //placaSumRow_OK_ONLY.X_rObustave      += PtransSumRec_gama15.S_rObustave     ;
                  /* 33 */   //placaSumRow_OK_ONLY.X_r2Pay          += PtransSumRec_gama15.S_r2Pay         ;
                  /* 34 */     placaSumRow_OK_ONLY.X_rNaRuke        += PtransSumRec_gama15.R_NaRuke        ;
                  /* 35 */   //placaSumRow_OK_ONLY.X_rZdrNa         += PtransSumRec_gama15.S_rZdrNa        ;
                  /* 36 */   //placaSumRow_OK_ONLY.X_rZorNa         += PtransSumRec_gama15.S_rZorNa        ;
                  /* 37 */   //placaSumRow_OK_ONLY.X_rZapNa         += PtransSumRec_gama15.S_rZapNa        ;
                  /* 38 */   //placaSumRow_OK_ONLY.X_rZapII         += PtransSumRec_gama15.S_rZapII        ;
                  /* 39 */   //placaSumRow_OK_ONLY.X_rZapAll        += PtransSumRec_gama15.S_rZapAll       ;
                  /* 40 */   //placaSumRow_OK_ONLY.X_rDoprNa        += PtransSumRec_gama15.S_rDoprNa       ;
                  /* 41 */   //placaSumRow_OK_ONLY.X_rDoprAll       += PtransSumRec_gama15.S_rDoprAll      ;
                  /* 42 */   //placaSumRow_OK_ONLY.X_rMio1stupNa    += PtransSumRec_gama15.S_rMio1stupNa   ;
                  /* 43 */   //placaSumRow_OK_ONLY.X_rMio2stupNa    += PtransSumRec_gama15.S_rMio2stupNa   ;
                  /* 44 */   //placaSumRow_OK_ONLY.X_rMioAllNa      += PtransSumRec_gama15.S_rMioAllNa     ;
                  /* 45 */   //placaSumRow_OK_ONLY.X_rKrizPorOsn    += PtransSumRec_gama15.S_rKrizPorOsn   ;
                  /* 46 */   //placaSumRow_OK_ONLY.X_rKrizPorUk     += PtransSumRec_gama15.S_rKrizPorUk    ;
                  /* 47 */   //placaSumRow_OK_ONLY.X_rSatiR         += PtransSumRec_gama15.S_rSatiR        ;
                  /* 48 */   //placaSumRow_OK_ONLY.X_rSatiB         += PtransSumRec_gama15.S_rSatiB        ;
                  /* 49 */   //placaSumRow_OK_ONLY.X_rZpiUk         += PtransSumRec_gama15.S_rZpiUk        ;
                  /* 50 */   //placaSumRow_OK_ONLY.X_rDaniZpi       += PtransSumRec_gama15.S_rDaniZpi      ;
                  /* 51 */   //placaSumRow_OK_ONLY.X_rNettoWoAdd    += PtransSumRec_gama15.S_rNettoWoAdd   ;
                  /* 52 */   //placaSumRow_OK_ONLY.X_rAHizdatak     += PtransSumRec_gama15.S_rAHizdatak    ;
                  /* 53 */     placaSumRow_OK_ONLY.X_rNettoAftKrp   += PtransSumRec_gama15.R_NettoAftKrp   ;
                  /* 54 */   //placaSumRow_OK_ONLY.X_rBrtDodNaStaz  += PtransSumRec_gama15.S_rBrtDodNaStaz ;
                  /* 55 */   //placaSumRow_OK_ONLY.X_rTheBruto_WoNZ += PtransSumRec_gama15.S_rTheBruto_WoNZ;
                  /* 56 */   //placaSumRow_OK_ONLY.X_tBrDodPoloz    += PtransSumRec_gama15.S_tBrDodPoloz   ;

               } // PtransSumRec_gama15 

               #endregion Korigiraj X - sumu za ptransRow_gama15

            } // if(rptFilter.HocuKonacniObrPor) 

            /* 01 */ ptransSumRow.P_Bruto100     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Bruto100    );
            /* 02 */ ptransSumRow.P_TheBruto     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_TheBruto    );
            /* 03 */ ptransSumRow.P_MioOsn       = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_MioOsn      );
            /* 04 */ ptransSumRow.P_Mio1stup     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Mio1stup    );
            /* 05 */ ptransSumRow.P_Mio2stup     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Mio2stup    );
            /* 06 */ ptransSumRow.P_MioAll       = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_MioAll      );
            /* 07 */ ptransSumRow.P_DoprIz       = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_DoprIz      );
            /* 08 */ ptransSumRow.P_Odbitak      = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Odbitak     );
            /* 09 */ ptransSumRow.P_Premije      = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Premije     );
            /* 10 */ ptransSumRow.P_Dohodak      = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Dohodak     );
            /* 11 */ ptransSumRow.P_PorOsnAll    = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_PorOsnAll   );
            /* 12 */ ptransSumRow.P_PorOsn1      = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_PorOsn1     );
            /* 13 */ ptransSumRow.P_PorOsn2      = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_PorOsn2     );
            /* 14 */ ptransSumRow.P_PorOsn3      = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_PorOsn3     );
            /* 15 */ ptransSumRow.P_PorOsn4      = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_PorOsn4     );
            /* 16 */ ptransSumRow.P_Por1Uk       = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Por1Uk      );
            /* 17 */ ptransSumRow.P_Por2Uk       = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Por2Uk      );
            /* 18 */ ptransSumRow.P_Por3Uk       = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Por3Uk      );
            /* 19 */ ptransSumRow.P_Por4Uk       = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Por4Uk      );
            /* 20 */ ptransSumRow.P_PorezAll     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_PorezAll    );
            /* 21 */ ptransSumRow.P_Prirez       = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Prirez      );
            /* 22 */ ptransSumRow.P_PorPrirez    = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_PorPrirez   );
            /* 23 */ ptransSumRow.P_Netto        = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Netto       );
            /* 24 */ ptransSumRow.P_Obustave     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Obustave    );
            /* 25 */ ptransSumRow.P_2Pay         = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_2Pay        );
            /* 26 */ ptransSumRow.P_NaRuke       = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_NaRuke      );
            /* 27 */ ptransSumRow.P_ZdrNa        = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_ZdrNa       );
            /* 28 */ ptransSumRow.P_ZorNa        = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_ZorNa       );
            /* 29 */ ptransSumRow.P_ZapNa        = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_ZapNa       );
            /* 30 */ ptransSumRow.P_ZapII        = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_ZapII       );
            /* 31 */ ptransSumRow.P_ZapAll       = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_ZapAll      );
            /* 32 */ ptransSumRow.P_DoprNa       = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_DoprNa      );
            /* 33 */ ptransSumRow.P_DoprAll      = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_DoprAll     );
            /* 34 */ ptransSumRow.P_SatiR        = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_SatiR       );
            /* 35 */ ptransSumRow.P_SatiB        = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_SatiB       );
            /* 36 */ ptransSumRow.P_SatiUk       = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_SatiUk      ); 
            /* 37 */ ptransSumRow.P_PtranEsCount = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_PtranEsCount);
            /* 38 */ ptransSumRow.P_PtranOsCount = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_PtranOsCount);
            /* 39 */ ptransSumRow.P_FondSatiDiff = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_FondSatiDiff);
            /* 40 */ ptransSumRow.P_Mio1stupNa   = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Mio1stupNa  );
            /* 41 */ ptransSumRow.P_Mio2stupNa   = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Mio2stupNa  );
            /* 42 */ ptransSumRow.P_MioAllNa     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_MioAllNa    );
            /* 43 */ ptransSumRow.P_KrizPorOsn   = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_KrizPorOsn  );
            /* 44 */ ptransSumRow.P_KrizPorUk    = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_KrizPorUk   );
            /* 45 */ ptransSumRow.P_ZpiUk        = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_ZpiUk       );
            /* 46 */ ptransSumRow.P_DaniZpi      = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_DaniZpi     );
            /* 47 */ ptransSumRow.P_NettoWoAdd   = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_NettoWoAdd  );
             
            /* 48 */   ptransSumRow.P_brutoOsn   = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.t_brutoOsn );
            /* 49 */   ptransSumRow.P_topObrok   = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.t_topObrok );
            /* 50 */   ptransSumRow.P_dodBruto   = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.t_dodBruto );
            
            /*    */   ptransSumRow.P_Mio1Olk    = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Mio1Olk);
            /*    */   ptransSumRow.P_Mio1Osn    = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_Mio1Osn );
          
            //          22.04.2016. ziovtno smo uzeli za rbrJop i ne vrijedi od T_dokDate < new DateTime(2010, 07, 01)   
            /* 51 */   ptransSumRow.P_zivotno    = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.t_zivotno  );
            /* 52 */   ptransSumRow.P_dopZdr     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.t_dopZdr   );
            /* 53 */   ptransSumRow.P_dobMIO     = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.t_dobMIO   );
            if(ptrans_rec.T_dokDate > new DateTime(2010, 07, 01)) ptransSumRow.P_zivotno = ptransSumRow.P_Premije = 0.00M;
            /*    */   ptransSumRow.P_dopZdr2020 = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.t_dopZdr2020);


            /* 54 */   ptransSumRow.P_netoAdd    = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.t_netoAdd  );
            /* 55 */   ptransSumRow.P_prijevoz   = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.t_prijevoz );
            /* 55 */   ptransSumRow.P_NP73       = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.t_NP73     );
            
            /*    */   ptransSumRow.P_AHizdatak  = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_AHizdatak   );
            /*    */   ptransSumRow.P_NettoAftKrp= ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_NettoAftKrp );
            /*    */   ptransSumRow.P_BrtDodNaStaz=ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD).Sum(ptrans => ptrans.R_BrtDodNaStaz);

            // ID-1 additions
            /* 56 */   ptransSumRow.P_PorPriNO   = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD &&(ptrans.t_tt == Placa.TT_NADZORODBOR|| ptrans.t_tt == Placa.TT_TURSITVIJECE                                       )).Sum(ptrans => ptrans.R_PorPrirez );
            /* 57 */   ptransSumRow.P_PorPriAH   = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD &&(ptrans.t_tt == Placa.TT_AUTORHONOR || ptrans.t_tt == Placa.TT_AUTORHONUMJ || ptrans.t_tt == Placa.TT_AHSAMOSTUMJ )).Sum(ptrans => ptrans.R_PorPrirez );
            /* 58 */   ptransSumRow.P_PorPriUG   = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_tt == Placa.TT_UGOVORODJELU                                                                              ).Sum(ptrans => ptrans.R_PorPrirez );
            /*    */   ptransSumRow.P_PorPriK4   = ptransTable.Where(ptrans => ptrans.t_personCD == persDistPtrList[p].t_personCD && ptrans.t_tt == Placa.TT_IDD_KOLONA_4                                                                              ).Sum(ptrans => ptrans.R_PorPrirez );


         } // else (this is NOT IP Kartice 

         #endregion Default cases

      } // foreach(var person in personsDistinct) 

      #endregion P_SUME

      #region OPC_SUME

      var ptransTableGroupedByOpcinaDictionary = ptransTable.GroupBy(p => p.t_opcCD, p => new { p.t_opcCD, p.t_opcName, p.R_PorezAll, p.R_Prirez, p.R_PorPrirez, p.t_stPrirez });

      var opcinaSumList = ptransTableGroupedByOpcinaDictionary.Select(opcinaDict => new
      {
         opcCD     = opcinaDict.Key,
         opcName   = opcinaDict.First().t_opcName,
         stopaPri  = opcinaDict.First().t_stPrirez,
         porez     = opcinaDict.Sum(opc => opc.R_PorezAll),
         prirez    = opcinaDict.Sum(opc => opc.R_Prirez),
         porPrirez = opcinaDict.Sum(opc => opc.R_PorPrirez)
      });

      if(placa_rec.VrstaObr != Placa.VrObr_NeisplacenaPlaca)
      {
         foreach(var opcina in opcinaSumList)
         {
            opcinaSumTable.Rows.Add(opcina.opcCD, opcina.opcName, opcina.porez, opcina.prirez, opcina.porPrirez, opcina.stopaPri);
         }
      }


      #endregion OPC_SUME

      #region E_SUME

      var ptrAndPtranEsGroup = ptransTable.GroupJoin(ptraneTable, ptR => new { ptR.t_personCD, ptR.t_parentID }, ptE => new { ptE.t_personCD, ptE.t_parentID }, (ptR, ptE) => new { ptR, ptE });

      var ptrWOptranes = ptrAndPtranEsGroup.Where(pt => pt.ptE.Count().IsZero());

      var ptrWOptranesAsPtraneShortTable = ptrWOptranes.Select(ptrans => new 
      { 
         t_vrstaR_cd   = "01", 
         t_vrstaR_name = "REDOVAN RAD", 
         t_cijPerc     = 100.00M, 
         t_sati        = placa_rec.FondSati, 
         R_EvrBruto    = ptrans.ptR.R_TheBruto,

         // 12.10.2015: 
         t_rsOO        = "10",
         t_personCD    = ptrans.ptR.t_personCD,
         t_parentID    = ptrans.ptR.t_parentID,
      });

      var ptraneShortTable = ptraneTable .Select(ptrane => new 
      { 
         ptrane.t_vrstaR_cd, 
         ptrane.t_vrstaR_name, 
         ptrane.t_cijPerc, 
         ptrane.t_sati, 
         ptrane.R_EvrBruto,

         // 12.10.2015: 
         ptrane.t_rsOO,
         ptrane.t_personCD,
         ptrane.t_parentID,
      });

      var concatShortTables = ptrWOptranesAsPtraneShortTable.Concat(ptraneShortTable);

    //var ptraneTableGroupedByVrstaRadaCD = ptraneTable.GroupBy      (p => p.t_vrstaR_cd, p => new { p.t_vrstaR_cd, p.t_vrstaR_name, p.t_cijPerc, p.t_sati, p.R_EvrBruto           });
    //var ptraneTableGroupedByVrstaRadaCD = concatShortTables.GroupBy(p => p.t_vrstaR_cd, p => new { p.t_vrstaR_cd, p.t_vrstaR_name, p.t_cijPerc, p.t_sati, p.R_EvrBruto           });
      var ptraneTableGroupedByVrstaRadaCD = concatShortTables.GroupBy(p => p.t_vrstaR_cd, p => new { p.t_vrstaR_cd, p.t_vrstaR_name, p.t_cijPerc, p.t_sati, p.R_EvrBruto, p.t_rsOO });

      var ptraneSumList = ptraneTableGroupedByVrstaRadaCD.Select(vrRadaCdGroup => new
      {
         vrstaR_cd   = vrRadaCdGroup.Key,
         vrstaR_name = vrRadaCdGroup.First().t_vrstaR_name,
         cijPerc     = vrRadaCdGroup.First().t_cijPerc,
         satiSum     = vrRadaCdGroup.Sum(gr => gr.t_sati),
         evrBrutoSum = vrRadaCdGroup.Sum(gr => gr.R_EvrBruto),

         // 12.10.2015: 
         rsOO        = vrRadaCdGroup.First().t_rsOO
      });

      foreach(var ptraneSum in ptraneSumList)
      {
         ptraneSumTable.Rows.Add(ptraneSum.vrstaR_cd, ptraneSum.vrstaR_name, ptraneSum.cijPerc, ptraneSum.satiSum, ptraneSum.evrBrutoSum /* tu ako ti treba dodaj novododani 't_rsOO' */);
      }

      #endregion E_SUME

      #endregion X_SUME, P_SUME, OPC_SUME, E_SUME

      #region RSm Obrazac additions

      if(this is RptP_ObrazRSm)
      {
         //string currPersonCD = ptransTable[0].t_personCD;
         uint currPersonCD = 0;
         List<string> ocurredOOlist = new List<string>();

         // X_SUME RSm Additions 
         placaSumRow_OK_ONLY.X_VrstaObveznika = SetRSmVrstaObveznika(ZXC.CURR_prjkt_rec, placa_rec);
         placaSumRow_OK_ONLY.X_vrstaObr       = placa_rec.VrstaObr;
         placaSumRow_OK_ONLY.X_rSm_ID         = placa_rec.RSm_ID;

         foreach(DS_Placa.IzvjTableRow ptransRow in ptransTable.Rows)
         {
            if(currPersonCD != ptransRow.t_personCD)
            {
               ocurredOOlist = new List<string>();
               currPersonCD  = ptransRow.t_personCD;
            }

            SetRSm_B_strana(rsmBstranaTable, ptransRow, ptraneTable, personTable, ++rsmRbr, ocurredOOlist);
         }

         // X_SUME RSm Corrections
         placaSumRow_OK_ONLY.X_rTheBruto = rsmBstranaTable.Sum(rsmBrow => ZXC.ValOrZero_Decimal(rsmBrow.b_Bruto,    2));
         placaSumRow_OK_ONLY.X_rMioOsn   = rsmBstranaTable.Sum(rsmBrow => ZXC.ValOrZero_Decimal(rsmBrow.b_MioOsn,   2));
         placaSumRow_OK_ONLY.X_rMio1stup = rsmBstranaTable.Sum(rsmBrow => ZXC.ValOrZero_Decimal(rsmBrow.b_Mio1stup, 2));
         placaSumRow_OK_ONLY.X_rMio2stup = rsmBstranaTable.Sum(rsmBrow => ZXC.ValOrZero_Decimal(rsmBrow.b_Mio2stup, 2));
         placaSumRow_OK_ONLY.X_rNetto    = rsmBstranaTable.Sum(rsmBrow => ZXC.ValOrZero_Decimal(rsmBrow.b_Netto,    2));

         if(placaSumRow_OK_ONLY.X_vrstaObr == Placa.VrObr_NeisplacenaPlaca)
         {
            placaSumRow_OK_ONLY.X_uMMYYYY = placaSumRow_OK_ONLY.X_uMM = placaSumRow_OK_ONLY.X_uYYYY = "";
         }
         
         if(placaSumRow_OK_ONLY.X_vrstaObr == "03")
         {
            //tamtam ovo je krivo treba godinu izvući iz X_zaMMYYYY
          //placaSumRow_OK_ONLY.X_zaMMYYYY = "00" + Placa.Get_uYYYY(placa_rec.DokDate);
            placaSumRow_OK_ONLY.X_zaMMYYYY = "00" + Placa.Get_zaYYYY(placa_rec.MMYYYY);
            placaSumRow_OK_ONLY.X_zaMM     = "00";
          //placaSumRow_OK_ONLY.X_zaYYYY = Placa.Get_uYYYY(placa_rec.DokDate);
            placaSumRow_OK_ONLY.X_zaYYYY = Placa.Get_zaYYYY(placa_rec.MMYYYY);
         }



         placaSumRow_OK_ONLY.X_HASHcode = ZXC.LastExportFileHASHcode;
      }

      #endregion RSm Obrazac additions

      #region VIRMANI Additions

      // 07.01.2015: 
    //if(this is RptP_Virmani)
      if(this is RptP_Virmani && (this is RptP_ListaBanka == false))
      {
         // 26.02.2024: 
         bool hasMixed_aJeTo = Get_hasMixed_aJeTo(placaTable);
         if(hasMixed_aJeTo)
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "VAŽNO upozorenje!!!\n\r\n\rZadanim filterom obuhvaća se skup raznovrsnih tipova plaće\n\rte će možda biti krivo sumirani ili krivo formirani PNB-ovi\n\rza pojedine 'virmane'?!");
         }

         #region Local Variables & Init common values

         DS_Placa.virmanRow virmanRow;
         Prjkt              cp = ZXC.CURR_prjkt_rec;
         DateTime           dateValuta, datePodnos;
         object[]           stdParams;

         DS_Placa.placaSumRow placaSumRow = (DS_Placa.placaSumRow)placaSumTable.Rows[0];

         if(RptFilter.ShowVirDateVal == true) dateValuta = RptFilter.VirDateVal;
         else                                 dateValuta = DateTime.MinValue;
         if(RptFilter.ShowVirDatePod == true) datePodnos = RptFilter.VirDatePod;
         else                                 datePodnos = DateTime.MinValue;

         stdParams = new object[] { cp.Naziv, cp.Ulica1, cp.PostaBr + " " + cp.Grad, ZXC.GetIBANfromOldZiro(RptFilter.Ziro1), cp.Grad, dateValuta, datePodnos };

         bool isPlacaBef2014 = placa_rec.DokDate < ZXC.Date01012014;

         #region PlacaVirmanInfo Initialization

         // WARNING:!!! Ovdje dosta racunas s tim da ti je u placa_rec.tt isti, tj nerpomijenjen za sve evantualne ostale placaRow-ove u placaTable. 
         //             A ako nije?!

       //pvi.aJeObrtnik     = ZXC.CURR_prjkt_rec.IsObrt;
         pvi.aJeObrtnik     = ZXC.CURR_prjkt_rec.IsObrt || ZXC.CURR_prjkt_rec.IsFizickaOsoba;
         pvi.aJeStranger    = ZXC.CURR_prjkt_rec.IsFrgn;
         pvi.aJeZORo        = ZXC.CURR_prjkt_rec.IsZorNa;
         pvi.aJeZAP_II      = placaSumRow.X_personCount >= placa_rec.Rule_GranBrRad;
         pvi.aJePoduzetPl   = placa_rec.TT == Placa.TT_PODUZETPLACA;
         pvi.aJeAutHonorar  = placa_rec.TT == Placa.TT_AUTORHONOR  || placa_rec.TT == Placa.TT_NR1_PX1NEDOP || placa_rec.TT == Placa.TT_NR2_P01NEDOP || placa_rec.TT == Placa.TT_DDBEZDOPRINO;
         pvi.aJeAutHonUmje  = placa_rec.TT == Placa.TT_AUTORHONUMJ || placa_rec.TT == Placa.TT_AUVECASTOPA  || placa_rec.TT == Placa.TT_NR3_PX1DADOP;
         pvi.aJeAHSamostUm  = placa_rec.TT == Placa.TT_AHSAMOSTUMJ;
         pvi.aJeUgovODjelu  = placa_rec.TT == Placa.TT_UGOVORODJELU ;
         pvi.aJeNadzorOdb   = placa_rec.TT == Placa.TT_NADZORODBOR || placa_rec.TT == Placa.TT_TURSITVIJECE;
         pvi.aJeIDD_Kolona4 = placa_rec.TT == Placa.TT_IDD_KOLONA_4;
         pvi.aJeSezZapPolj  = placa_rec.TT == Placa.TT_SEZZAPPOLJOP;
         pvi.aJePorNaDobit  = placa_rec.TT == Placa.TT_POREZNADOBIT;
         pvi.aJeStrucOspos  = placa_rec.TT == Placa.TT_STRUCNOOSPOS;
         pvi.aJeSamoDop     = placa_rec.TT == Placa.TT_SAMODOPRINOS;
         // jos imas property: 
         //aJeOtherDohodak { get { return aJeAutHonorar || aJeNadzorOdb || aJeUgovODjelu;  } }

         pvi.RSm_ID_inUse = placa_rec.RSm_ID;

         if(pvi.aJePoduzetPl == true)
         {
            // predmnijevamo da je i prvi i zadnji i jedini u ptransTable-u poduzetnik. 
            var personPoduzetnik = personTable.SingleOrDefault(person => person.personCD == ptransTable[0].t_personCD);
            pvi.oib_inUse = personPoduzetnik.oib;
         }
         else
         {
            pvi.oib_inUse = ZXC.CURR_prjkt_rec.Oib;
         }

         #endregion PlacaVirmanInfo Initialization



         #endregion Local Variables & Init common values

         #region Doprinosi

         virmanRow = Create_Doprinosi_Virman(ZXC.VirmanEnum.MIO1,   virmanTable, stdParams, placaSumRow.X_rMio1stup  , placa_rec.TT, isPlacaBef2014, placa_rec.DokDate);
         virmanRow = Create_Doprinosi_Virman(ZXC.VirmanEnum.MIO2,   virmanTable, stdParams, placaSumRow.X_rMio2stup  , placa_rec.TT, isPlacaBef2014, placa_rec.DokDate);
         virmanRow = Create_Doprinosi_Virman(ZXC.VirmanEnum.ZDR,    virmanTable, stdParams, placaSumRow.X_rZdrNa     , placa_rec.TT, isPlacaBef2014, placa_rec.DokDate);
         virmanRow = Create_Doprinosi_Virman(ZXC.VirmanEnum.ZOR,    virmanTable, stdParams, placaSumRow.X_rZorNa     , placa_rec.TT, isPlacaBef2014, placa_rec.DokDate);
         virmanRow = Create_Doprinosi_Virman(ZXC.VirmanEnum.ZPI,    virmanTable, stdParams, placaSumRow.X_rZpiUk     , placa_rec.TT, isPlacaBef2014, placa_rec.DokDate);
         virmanRow = Create_Doprinosi_Virman(ZXC.VirmanEnum.ZAP,    virmanTable, stdParams, placaSumRow.X_rZapNa     , placa_rec.TT, isPlacaBef2014, placa_rec.DokDate);
         virmanRow = Create_Doprinosi_Virman(ZXC.VirmanEnum.ZPP,    virmanTable, stdParams, placaSumRow.X_rZapII     , placa_rec.TT, isPlacaBef2014, placa_rec.DokDate);
         virmanRow = Create_Doprinosi_Virman(ZXC.VirmanEnum.MIO1NA, virmanTable, stdParams, placaSumRow.X_rMio1stupNa, placa_rec.TT, isPlacaBef2014, placa_rec.DokDate);
         virmanRow = Create_Doprinosi_Virman(ZXC.VirmanEnum.MIO2NA, virmanTable, stdParams, placaSumRow.X_rMio2stupNa, placa_rec.TT, isPlacaBef2014, placa_rec.DokDate);

         #endregion Doprinosi

         #region Porez & Prirez & KrizniPorez

         if(pvi.aJeOtherDohodak)
         {
            foreach(var ptrans in ptransTable)
            {
               var person = personTable.SingleOrDefault(prs => prs.personCD == ptrans.t_personCD);

               if(isPlacaBef2014)
               {
                  virmanRow = Create_PorezPrirez_Virman(ZXC.VirmanEnum.POR, virmanTable, stdParams, ptrans.R_PorezAll,             0.00M, ptrans.t_opcCD, ptrans.t_opcName,            0.00M,  person.oib, person.ime + " " + person.prezime, placa_rec.TT, isPlacaBef2014);
                  virmanRow = Create_PorezPrirez_Virman(ZXC.VirmanEnum.PRI, virmanTable, stdParams, ptrans.R_Prirez  , ptrans.t_stPrirez, ptrans.t_opcCD, ptrans.t_opcName, ptrans.R_PorezAll, person.oib, person.ime + " " + person.prezime, placa_rec.TT, isPlacaBef2014);
               }
               else
               {
                  virmanRow = Create_PorezPrirez_Virman(ZXC.VirmanEnum.POR, virmanTable, stdParams, ptrans.R_PorezAll,             0.00M, ptrans.t_opcCD, ptrans.t_opcName,            0.00M,  ZXC.CURR_prjkt_rec.Oib, person.ime + " " + person.prezime, placa_rec.TT, isPlacaBef2014);
                  virmanRow = Create_PorezPrirez_Virman(ZXC.VirmanEnum.PRI, virmanTable, stdParams, ptrans.R_Prirez  , ptrans.t_stPrirez, ptrans.t_opcCD, ptrans.t_opcName, ptrans.R_PorezAll, ZXC.CURR_prjkt_rec.Oib, person.ime + " " + person.prezime, placa_rec.TT, isPlacaBef2014);
               }
            }
         }
         else
         {
            foreach(var opcina in opcinaSumList)
            {
               if(isPlacaBef2014)
               {
                  virmanRow = Create_PorezPrirez_Virman(ZXC.VirmanEnum.POR, virmanTable, stdParams, opcina.porez,            0.00M, opcina.opcCD, opcina.opcName,        0.00M, pvi.oib_inUse, "", placa_rec.TT, isPlacaBef2014);
                  virmanRow = Create_PorezPrirez_Virman(ZXC.VirmanEnum.PRI, virmanTable, stdParams, opcina.prirez, opcina.stopaPri, opcina.opcCD, opcina.opcName, opcina.porez, pvi.oib_inUse, "", placa_rec.TT, isPlacaBef2014);
               }
               else
               {
                  virmanRow = Create_PorezPrirez_Virman(ZXC.VirmanEnum.POR, virmanTable, stdParams, opcina.porez,            0.00M, opcina.opcCD, opcina.opcName,        0.00M, ZXC.CURR_prjkt_rec.Oib, "", placa_rec.TT, isPlacaBef2014);
                  virmanRow = Create_PorezPrirez_Virman(ZXC.VirmanEnum.PRI, virmanTable, stdParams, opcina.prirez, opcina.stopaPri, opcina.opcCD, opcina.opcName, opcina.porez, ZXC.CURR_prjkt_rec.Oib, "", placa_rec.TT, isPlacaBef2014);
               }
           }
         }

         virmanRow = Create_KrizniPorez_Virman(ZXC.VirmanEnum.KRP, virmanTable, stdParams, placaSumRow.X_rKrizPorUk, placa_rec.TT);

         #endregion Porez & Prirez & KrizniPorez

         #region NETTO (Zbirni bankama, pojedinacni na tekuci)

         // 18.05.2016: ___ SEPA news ___ START ___                                                                                               

         bool isSEPA = (this as RptP_Virmani).ZNPkind == ZXC.ZNP_Kind.SEPA_Placa;

         if(isSEPA)
         {
            foreach(var person in personTable)
            {
               if(((Person.VrstaIsplateEnum)person.vrstaIsplate) != Person.VrstaIsplateEnum.TEKUCI)
               {
                  // qweqwe ... 
                  person.vrstaIsplate = (byte)Person.VrstaIsplateEnum.TEKUCI;
                  person.R_isBanka2Tekuci = true; // flag, da znamo da je prisiljeno transformirana vrstaIsplate 
               }
            }
         }

         // 18.05.2016: ___ SEPA news ___  END  ___                                                                                               

         #region ZBIRNI NETTO po banci 

         var personBankaOnlyList = personTable.Where(per => ((Person.VrstaIsplateEnum)per.vrstaIsplate) == Person.VrstaIsplateEnum.BANKA);

         var personBankaOnlyWmoneyList = personBankaOnlyList.GroupJoin(ptransTable, per => per.personCD, ptr => ptr.t_personCD, (per, ptrList) => new { per, personMoney = ptrList.Sum(ptr => ptr.R_NaRuke) });

         var personsGroupedByBanka = personBankaOnlyWmoneyList.GroupBy(perWmoney => perWmoney.per.banka_cd).Select(perWmoney => new
         {
            bankaCD    = perWmoney.Key,
            bankaMoney = perWmoney.Sum(per => per.personMoney)
         });

         var bankaGroupWkupdobList = personsGroupedByBanka.Join(kupdobTable, bankaGr => bankaGr.bankaCD, kpdb => kpdb.kupdobCD, (bankaGr, kpdb) => new 
         {
            bankaMoney = bankaGr.bankaMoney, 
            bankaCD    = bankaGr.bankaCD,
            banNaziv   = kpdb.naziv,
            banUlica   = kpdb.ulica1,
            banZipGrad = kpdb.postaBr + " " + kpdb.grad,
            banGrad    = kpdb.grad,
            banZiroRn  = ZXC.GetIBANfromOldZiro(kpdb.ziro1),
            banPnbOmod = kpdb.pnbMPlaca,
            banPnbO    = kpdb.pnbVPlaca
         });

         foreach(var bankaGroup in bankaGroupWkupdobList) // zbirni po banci netto virmani 
         {
            virmanRow = Create_Netto_Virman(ZXC.VirmanEnum.NET, virmanTable, stdParams, bankaGroup.bankaMoney, placa_rec.TT, /*do 14.03.2014 placa_rec.MMYY*/ placa_rec.RSm_ID, 0);

            if(virmanRow == null) continue;
         
            virmanRow.prim1   = bankaGroup.banNaziv; 
            virmanRow.prim2   = bankaGroup.banUlica;
            virmanRow.prim3   = bankaGroup.banZipGrad;
            virmanRow.mjesto2 = bankaGroup.banGrad;
            virmanRow.ziro2   = ZXC.GetIBANfromOldZiro(bankaGroup.banZiroRn);
          //08.12.2015. kada je NP onda HR99 + prazno
          //virmanRow.pnboMod =                                                    bankaGroup.banPnbOmod;
          //virmanRow.pnboMod =                                                    ZXC.GetPnbModfromOldPnbMod(virmanRow.pnboMod); //16.01.2013.
            virmanRow.pnboMod = (placa_rec.TT == Placa.TT_NEOPOREZPRIM) ? "HR99" : bankaGroup.banPnbOmod;
            virmanRow.pnboMod = (placa_rec.TT == Placa.TT_NEOPOREZPRIM) ? ""     : ZXC.GetPnbModfromOldPnbMod(virmanRow.pnboMod); //16.01.2013.

            virmanRow.pnbo    = bankaGroup.banPnbO;

             virmanRow.opisPl  = "PLAĆA za " + placa_rec.MMYYYY + " (Zbirni NETO - obustave)";

         } // foreach(var bankaGroup in bankaGroupWkupdobList) 

         #endregion ZBIRNI NETTO po banci

         #region POJEDINACNI NETTO po personu 

         // Rolling for TEKUCI only _______________________________________________________________________________________________________ 

         var personTekuciOnlyList = personTable.Where(per => ((Person.VrstaIsplateEnum)per.vrstaIsplate) == Person.VrstaIsplateEnum.TEKUCI);

         // provjera, ako neki nema zadanu banku
         foreach(var person in personTekuciOnlyList.Where(pto => pto.banka_cd.IsZero()))
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Djelatnik {0} nema zadanu banku!", person.prezime);
         }

         this.IsBtchBookg_NettoAndObust = ZXC.CURR_prjkt_rec.IsBtchBookg; 
       //this.IsBtchBookg_NettoNP       = true; // TODO: !!! 

         var personTekuciOnlyWmoneyList = 
            personTekuciOnlyList
            .GroupJoin(ptransTable, per => per.personCD, ptr => ptr.t_personCD, (per, ptrList) => new 
            { 
               per, 
               personMoney          = ptrList.Sum(ptr => ptr.R_NaRuke),
               firstPtransT_spc     = ptrList.First().t_spc,
               firstPtransT_isMioII = ptrList.First().t_isMioII,
            });

         var personTekuciWkupdobList = 
            personTekuciOnlyWmoneyList
            .Join(kupdobTable, perWmoney => perWmoney.per.banka_cd, kpdb => kpdb.kupdobCD, (perWmoney, kpdb) => new 
         {
            perMoney             = perWmoney.personMoney,
            firstPtransT_spc     = perWmoney.firstPtransT_spc,
            firstPtransT_isMioII = perWmoney.firstPtransT_isMioII,
            banNaziv             = kpdb.naziv,
            banUlica             = kpdb.ulica1,
            banZipGrad           = kpdb.postaBr + " " + kpdb.grad,
            banGrad              = kpdb.grad,
          //banZiroRn            = ZXC.GetIBANfromOldZiro(kpdb.ziro1),
            banZiroRn            = perWmoney.per.R_isBanka2Tekuci ? perWmoney.per.pnbV : ZXC.GetIBANfromOldZiro(kpdb.ziro1),
            banPnbOmod           = perWmoney.per.pnbM,
            banPnbO              = perWmoney.per.pnbV,
            perImePrez           = perWmoney.per.ime + " " + perWmoney.per.prezime,
            perWmoney            = perWmoney // dodano za SEPA-u 
         });

         foreach(var perWkpdb in personTekuciWkupdobList) // POJEDINACNI NETTO VIRMANI .. od kada je SEPA tu stizemo 
                                                          // i sa BtchBookg i sa  'Viper' virmanima (NON BtchBookg)  
         {                                                                                                                              //TODOTAMQ ovdje dolazi samo 1 ptrans
            virmanRow = Create_Netto_Virman(ZXC.VirmanEnum.NET, virmanTable, stdParams, perWkpdb.perMoney, placa_rec.TT, /*do 14.03.2014.placa_rec.MMYY*/ placa_rec.RSm_ID, GetX_pnbz(placa_rec, (Ptrans.SpecEnum)perWkpdb.firstPtransT_spc, ((int)perWkpdb.firstPtransT_isMioII).IsZero()));

            if(virmanRow == null) continue;
         
            virmanRow.prim1   = perWkpdb.banNaziv; 
            virmanRow.prim2   = perWkpdb.banUlica;
            virmanRow.prim3   = perWkpdb.banZipGrad;
            virmanRow.mjesto2 = perWkpdb.banGrad;
            virmanRow.ziro2   = ZXC.GetIBANfromOldZiro(perWkpdb.banZiroRn);
            virmanRow.pnboMod = perWkpdb.banPnbOmod;
          //08.12.2015. kada je NP onda HR99 + prazno
          //virmanRow.pnboMod = ZXC.GetPnbModfromOldPnbMod(virmanRow.pnboMod); //16.01.2013.
          //virmanRow.pnbo    = perWkpdb.banPnbO;
            virmanRow.pnboMod = (placa_rec.TT == Placa.TT_NEOPOREZPRIM) ? "HR99" : ZXC.GetPnbModfromOldPnbMod(virmanRow.pnboMod); //16.01.2013.
            virmanRow.pnbo    = (placa_rec.TT == Placa.TT_NEOPOREZPRIM) ? ""     : perWkpdb.banPnbO;

            if(placa_rec.TT == Placa.TT_NEOPOREZPRIM)
            {
               virmanRow.opisPl = perWkpdb.perImePrez + " Neoporezivi primici za " + placa_rec.MMYYYY + " Rn: " + perWkpdb.banPnbO;
            }
            else if(placa_rec.TT == Placa.TT_STRUCNOOSPOS)
            {
               virmanRow.opisPl = perWkpdb.perImePrez + " Za " + placa_rec.MMYYYY + " Rn: " + perWkpdb.banPnbO;
            }
            else if(placa_rec.TT == Placa.TT_REDOVANRAD ||
                    placa_rec.TT == Placa.TT_PODUZETPLACA)
            {
               virmanRow.opisPl = perWkpdb.perImePrez + " Plaća za " + placa_rec.MMYYYY + " Rn: " + perWkpdb.banPnbO;

               if(IsBtchBookg_NettoAndObust) virmanRow.btchBookgID = NetoBtchBookgID;
            }
            else if(placa_rec.TT == Placa.TT_OSTALIPRIM)
            {
               virmanRow.opisPl = perWkpdb.perImePrez + " Ostali primici Za " + placa_rec.MMYYYY + " Rn: " + perWkpdb.banPnbO;
            }
            else
            {
               virmanRow.opisPl = perWkpdb.perImePrez + " Drugi doh. za " + placa_rec.MMYYYY + " Rn: " + perWkpdb.banPnbO;
            }

            // SEPA NEWS 
            //if(isSEPA) ... nije samo SEPA news vec i papirnati virmani moraju ovo dobiti od 06.06.2016. 
            //{
               virmanRow.prim1   = perWkpdb.perImePrez; 
               virmanRow.prim2   = perWkpdb.perWmoney.per.ulica;
               virmanRow.prim3   = (perWkpdb.perWmoney.per.postaBr + " " + perWkpdb.perWmoney.per.grad).TrimStart(' ').TrimEnd(' ');
               virmanRow.mjesto2 = perWkpdb.perWmoney.per.grad;

               string ziroSub = virmanRow.ziro2.SubstringSafe(11, 2); // 31.05.2016. da na osnovu broja zirca yna da li je zasticeni ili ne 
//31.05.2016. trebali bi znati da li radnik ima na obustavama "Z" zasticeni racun obustavaKind == Ptrans.PtranoKind.ZASTICENIrn jer o tome ovisi VOP na drugom racunu radnika
               virmanRow.pnboMod = "HR69";
               theVOP = GetVOPformPlacaTT(placa_rec.TT, ziroSub);
               virmanRow.pnbo    = "40002-" + ZXC.CURR_prjkt_rec.Oib + /*"-100"*/ theVOP;

               if(theVOP == "-110") personCDw110list.Add(perWkpdb.perWmoney.per.personCD);
               virmanRow.statOb = perWkpdb.perWmoney.per.personCD.ToString(); // !!! 

               virmanRow.opisPl = GetOpisPlacanjaformPlacaTT(placa_rec.TT) + placa_rec.MMYYYY; //23.05.2016.

               //02.06.2016. isSALA
               if(placa_rec.TT == Placa.TT_REDOVANRAD && ziroSub == Normal_IBAN_root){ virmanRow.ptranoKind = (int)Ptrans.PtranoKind.NEZASTICENIrn; virmanRow.sifraPl = "SALA"; }
               if(placa_rec.TT == Placa.TT_REDOVANRAD && ziroSub == Zastic_IBAN_root){ virmanRow.ptranoKind = (int)Ptrans.PtranoKind.ZASTICENIrn  ; virmanRow.sifraPl = "SALA"; }

               // 03.03.2021: za NP news 
               virmanRow.UtilInt = (int)perWkpdb.perWmoney.per.personCD;

            //} 06.02.2016.da i na virmanima dojde dobro

         } // foreach(var perWkpdb in personTekuciWkupdobList) // POJEDINACNI NETTO VIRMANI  

         #endregion POJEDINACNI NETTO po personu

         #region POJEDINACNI NP po ptransu (new 2021)

         if(placa_rec.TT == Placa.TT_NEOPOREZPRIM)
         {
            // trba iz svakog POJEDINACNI NETTO po personu virmanRow-a 
            // umnoziti za svaki ptrans sa njegovom vrstom NP-a 

            DS_Placa.virmanRow oldVirmanRowNP;
            int oldCount = virmanTable.Rows.Count;

            //foreach(DS_Placa.virmanRow oldVirmanRowNP in virmanTable.Rows)
            for(int rowIdx = 0; rowIdx < oldCount; ++rowIdx)
            {
               oldVirmanRowNP = (DS_Placa.virmanRow)virmanTable.Rows[rowIdx];

               /*List<Ptrans>*/ var thisPersonPtransList = ptransTable.Where(ptrRow => ptrRow.t_personCD == oldVirmanRowNP.UtilInt)/*.ToList()*/;
               bool thisIsfirstPtransOfThisPerson = true;
               DS_Placa.virmanRow newVirmanRowNP;

               string orig_pnbo = oldVirmanRowNP.pnbo;

               foreach(var ptransRow in thisPersonPtransList)
               {
                  if(thisIsfirstPtransOfThisPerson) // samo izmjeni podatke postojecem virmanu 
                  {
                     oldVirmanRowNP.pnbo = Get_NP_theVOP(oldVirmanRowNP.pnbo, ptransRow.t_neoPrimCD);

                   //oldVirmanRowNP.money = ptransRow.R_NaRuke; // TODO: !!! 
                     oldVirmanRowNP.money = ptransRow.t_netoAdd; 

                     thisIsfirstPtransOfThisPerson = false;
                  }
                  else // dodaj novi virman 
                  {
                     newVirmanRowNP = (DS_Placa.virmanRow)virmanTable.NewRow();
                     newVirmanRowNP.ItemArray = oldVirmanRowNP.ItemArray.Clone() as object[];

                     newVirmanRowNP.pnbo = Get_NP_theVOP(orig_pnbo, ptransRow.t_neoPrimCD);

                   //newVirmanRowNP.money = ptransRow.R_NaRuke; // TODO: !!! 
                     newVirmanRowNP.money = ptransRow.t_netoAdd; 

                     virmanTable.Rows.Add(newVirmanRowNP);
                  }
               }
            }

         }

         #endregion POJEDINACNI NP po ptransu (new 2021)

         #endregion NETTO (Zbirni bankama, pojedinacni na tekuci)

         #region Obustave

         // Rolling for OBUSTAVE zbirni only _______________________________________________________________________________________________________ 

         var obustaveZbirneOnlyList = ptranoTable.Where(ptr => Convert.ToBoolean(ptr.t_isZbir) == true);

          var obustaveGroupedByKupdob = obustaveZbirneOnlyList.GroupBy(grp => grp.t_kupdob_cd).Select(grp => new
         {
            kupdobCD    = grp.Key,
            kupdobMoney = grp.Sum(ptr => ptr.t_iznosOb),
            personCount = grp.Count(),
            obustOpis   = grp.First().t_opisOb
         });

         var kupdobGroupWkupdobList = obustaveGroupedByKupdob.Join(kupdobTable, kupdobGr => kupdobGr.kupdobCD, kpdb => kpdb.kupdobCD, (kupdobGr, kpdb) => new 
         {
            obustavaMoney = kupdobGr.kupdobMoney,
            obustPerCount = kupdobGr.personCount,
            obustOpis     = kupdobGr.obustOpis,
            kpdbNaziv     = kpdb.naziv,
            kpdbUlica     = kpdb.ulica1,
            kpdbZipGrad   = kpdb.postaBr + " " + kpdb.grad,
            kpdbGrad      = kpdb.grad,
            kpdbZiroRn    = ZXC.GetIBANfromOldZiro(kpdb.ziro1),
            kpdbPnbOmod   = kpdb.pnbMPlaca,
            kpdbPnbO      = kpdb.pnbVPlaca
         });

         foreach(var kupdobGroup in kupdobGroupWkupdobList) // ZBIRNE OBUSTAVE 
         {
          //virmanRow = Create_Obustava_Virman(ZXC.VirmanEnum.NET, virmanTable, stdParams, kupdobGroup.obustavaMoney, placa_rec.TT);
          // 12.02.2013.
            //virmanRow = Create_Obustava_Virman(ZXC.VirmanEnum.NET, virmanTable, stdParams, kupdobGroup.obustavaMoney, placa_rec.TT, placa_rec.MMYY, GetX_pnbz(placa_rec, Ptrans.SpecEnum.XNIJE, false));
            //08.01.2015. joppd umjesto mmyy
            virmanRow = Create_Obustava_Virman(ZXC.VirmanEnum.NET, virmanTable, stdParams, kupdobGroup.obustavaMoney, placa_rec.TT, /*placa_rec.MMYY*/ placa_rec.RSm_ID, GetX_pnbz(placa_rec, Ptrans.SpecEnum.XNIJE, false));

            if(virmanRow == null) continue;
         
            virmanRow.prim1   = kupdobGroup.kpdbNaziv; 
            virmanRow.prim2   = kupdobGroup.kpdbUlica;
            virmanRow.prim3   = kupdobGroup.kpdbZipGrad.TrimStart(' ').TrimEnd(' ');
            virmanRow.mjesto2 = kupdobGroup.kpdbGrad;
            virmanRow.ziro2   = ZXC.GetIBANfromOldZiro(kupdobGroup.kpdbZiroRn);
            virmanRow.pnboMod = kupdobGroup.kpdbPnbOmod;
            virmanRow.pnboMod = ZXC.GetPnbModfromOldPnbMod(virmanRow.pnboMod); //16.01.2013.
            virmanRow.pnbo    = kupdobGroup.kpdbPnbO;

          //virmanRow.opisPl = "Obustava " + kupdobGroup.obustOpis + " (za " + kupdobGroup.obustPerCount + " djelatnika)";
          // 12.02.2013.
            virmanRow.opisPl = "Obustava za " + kupdobGroup.obustPerCount + " djelatnika";

            // SEPA NEWS 
            //if(isSEPA) 02.06.2016. da i virmani dobe pnb kak se spada
            //{
             //if(virmanRow.prim2.IsEmpty()) ZXC.aim_emsg("{0}\n\nAdresa - Ulica NEPOPUNJENA!", virmanRow.prim1);
             //if(virmanRow.prim3.IsEmpty() || virmanRow.prim3 == " ") ZXC.aim_emsg("{0}\n\nAdresa - Grad NEPOPUNJEN!", virmanRow.prim1);

             //if(IsBtchBookg_Obustave) virmanRow.btchBookgID = "Grupa OBUSTAVE"; 24.05.2016.
//30.05.2016. if(IsBtchBookg_NettoAndObust) virmanRow.btchBookgID = NetoBtchBookgID;

            virmanRow.pnboMod = GetPnbM(virmanRow.pnboMod, virmanRow.pnbo);

            //}02.06.2016. da i virmani dobe pnb kak se spada
         } // foreach(var kupdobGroup in kupdobGroupWkupdobList) // ZBIRNE OBUSTAVE 

         // Rolling for OBUSTAVE NOT zbirni only _______________________________________________________________________________________________________ 

         var obustavePojedinacneOnlyList = ptranoTable.Where(ptr => Convert.ToBoolean(ptr.t_isZbir) == false);

         var obustavaWkupdobList = obustavePojedinacneOnlyList.Join(kupdobTable, obustava => obustava.t_kupdob_cd, kpdb => kpdb.kupdobCD, (obustava, kpdb) => new 
         {
            obustavaMoney = obustava.t_iznosOb,
            obustOpis     = obustava.t_opisOb,
            obusImePrez   = obustava.t_ime + " " + obustava.t_prezime,
            kpdbNaziv     = kpdb.naziv,
            kpdbUlica     = kpdb.ulica1,
            kpdbZipGrad   = kpdb.postaBr + " " + kpdb.grad,
            kpdbGrad      = kpdb.grad,
            kpdbZiroRn    = ZXC.GetIBANfromOldZiro(kpdb.ziro1),
            kpdbPnbOmod   = kpdb.pnbMPlaca,
            kpdbPnbO      = kpdb.pnbVPlaca,
            obustavaKind  = obustava.t_isZastRn,  // 23.05.2016.
            personCD      = obustava.t_personCD
         });

         Ptrans.PtranoKind obustKind;

         foreach(var obustava in obustavaWkupdobList) // NE ZBIRNE OBUSTAVE 
         {
            virmanRow = Create_Netto_Virman(ZXC.VirmanEnum.NET, virmanTable, stdParams, obustava.obustavaMoney, placa_rec.TT, /*do 14.03.2014.placa_rec.MMYY*/ placa_rec.RSm_ID, 0);

            if(virmanRow == null) continue;
         
            virmanRow.prim1   = obustava.kpdbNaziv; 
            virmanRow.prim2   = obustava.kpdbUlica;
            virmanRow.prim3   = obustava.kpdbZipGrad.TrimStart(' ').TrimEnd(' ');
            virmanRow.mjesto2 = obustava.kpdbGrad;
            virmanRow.ziro2   = ZXC.GetIBANfromOldZiro(obustava.kpdbZiroRn);
            virmanRow.pnboMod = obustava.kpdbPnbOmod;
            virmanRow.pnboMod = ZXC.GetPnbModfromOldPnbMod(virmanRow.pnboMod); //16.01.2013.
            virmanRow.pnbo    = obustava.kpdbPnbO;

            virmanRow.opisPl  = obustava.obusImePrez + " obustava " + obustava.obustOpis;

          // SEPA NEWS 
          //if(isSEPA) ... da i virmani dobe pnb kak se spada 
          //{
               obustKind = (Ptrans.PtranoKind)obustava.obustavaKind;
               virmanRow.ptranoKind = (int)obustKind;

               virmanRow.statOb = obustava.personCD.ToString(); // !!! 
               
               if(IsBtchBookg_NettoAndObust && (obustKind == Ptrans.PtranoKind.ZASTICENIrn ||
                                                obustKind == Ptrans.PtranoKind.NEZASTICENIrn)) virmanRow.btchBookgID = NetoBtchBookgID;

               // 23.05.2016.
               if(obustKind == Ptrans.PtranoKind.ZASTICENIrn)
               {
                  virmanRow.pnboMod = "HR69";
                  virmanRow.pnbo    = "40002-" + ZXC.CURR_prjkt_rec.Oib + "-110"/* GetVOPformPlacaTT(placa_rec.TT)*/;
                  virmanRow.sifraPl = "SALA";

                  personCDw110list.Add(obustava.personCD);
               }
               else if(obustKind == Ptrans.PtranoKind.NEZASTICENIrn) // 31.05.2016. ovo treba provjeriti da li ide u bb
               {
                  virmanRow.pnboMod = "HR69";
                  virmanRow.pnbo    = "40002-" + ZXC.CURR_prjkt_rec.Oib + "-120"/* GetVOPformPlacaTT(placa_rec.TT)*/;
                  virmanRow.sifraPl = "SALA";
               }
               else
               {
                  virmanRow.pnboMod = GetPnbM(virmanRow.pnboMod, virmanRow.pnbo);
               }
          //} 

         } // foreach(var obustava in obustavaWkupdobList) // NE ZBIRNE OBUSTAVE 

         // Additional virmanTable pass... eventual rename VOP 100 to VOP 120 (ako ima isplatu n a obadvije vrste racuna) 
         bool is100, has110;
         foreach(var vir in virmanTable)
         {
            is100 = vir.pnbo.EndsWith("-100");

            if(is100)
            {
               has110 = personCDw110list.Contains(ZXC.ValOrZero_UInt(vir.statOb)); // u statOb smo spremili personCD 

               if(has110) vir.pnbo = vir.pnbo.Replace("-100", "-120");
            }
         }

         #endregion OBUSTAVE

         #region Fill VirmanList (bussiness for ZNP)

         TheVirmanList = new List<VirmanStruct>(virmanTable.Rows.Count);

         VirmanStruct virman_rec;

         int virRbr = 0;
         bool badVirmanGroupChoice = false;
         if(this is RptP_SEPA)
         {
            if((this as RptP_SEPA).IsBtchBookgExtraFile && RptFilter.VirmanGroup == ZXC.VirmanBtchBookgKind.ALL)
            {
               badVirmanGroupChoice = true;
               ZXC.aim_emsg(MessageBoxIcon.Error, "KRIVI ODABIR GRUPE VIRMANA!\n\nZadana poslovna banka [{0}]\n\nzahtjeva razdvojene datoteke zbirnih i pojedinačnih transakcija.", RptFilter.Ziro1);
            }
         }

         // 02.06.2016: SEPA; skip eventual unwanted VirmanBtchBookgKind
         List<DS_Placa.virmanRow> virmanRowsToBeRemoved = new List<DS_Placa.virmanRow>();

         foreach(var virman in virmanTable)
         {
            // 28.11.2022: izbaci virmane sa negativnim moneyom 
            if(virman.money.IsZeroOrNegative())
            {
               virmanRowsToBeRemoved.Add(virman);
               continue;
            }

            // 02.06.2016: SEPA; skip eventual unwanted VirmanBtchBookgKind ___ START ___ 

            if(isSEPA && RptFilter.VirmanGroup != ZXC.VirmanBtchBookgKind.ALL)
            {
               if(RptFilter.VirmanGroup == ZXC.VirmanBtchBookgKind.BtchBookg_ONLY     && virman.btchBookgID.IsEmpty ()) { virmanRowsToBeRemoved.Add(virman); continue; }
               if(RptFilter.VirmanGroup == ZXC.VirmanBtchBookgKind.NON_BtchBookg_ONLY && virman.btchBookgID.NotEmpty()) { virmanRowsToBeRemoved.Add(virman); continue; }
            }
            else
            {
               if(badVirmanGroupChoice) continue;
            }
            // 02.06.2016: SEPA; skip eventual unwanted VirmanBtchBookgKind ___  END  ___ 

            virman_rec = new VirmanStruct(stdParams);

            virRbr++;

            virman_rec.BtchBookgID = virman.btchBookgID;

            virman_rec.PtranoKind = (Ptrans.PtranoKind)virman.ptranoKind;

            virman_rec.Prim1   = virman.prim1;
            virman_rec.Prim2   = virman.prim2;
            virman_rec.Prim3   = virman.prim3;
            virman_rec.Ziro2   = virman.ziro2;
            virman_rec.Mjesto2 = virman.mjesto2;

            virman_rec.PnbzMod = virman.pnbzMod; 
            virman_rec.Pnbz    = virman.pnbz   ;
            virman_rec.PnboMod = virman.pnboMod;
            virman_rec.Pnbo    = virman.pnbo   ;

            virman_rec.StatOb  = virman.statOb;
            virman_rec.SifraPl = virman.sifraPl;
            virman_rec.OpisPl  = virman.opisPl;
            
            virman_rec.Money   = virman.money;

            virman_rec.VirKind = (ZXC.VirmanEnum)virman.virKind;

            virman.HUB3Data_PDF417 = virman_rec.HUB3Data_PDF417;

            if(this is RptP_SEPA)
            {
               (this as RptP_SEPA).CheckSomeSEPAvalues(virRbr, virman_rec);
            }

            TheVirmanList.Add(virman_rec);

         } // foreach(var virman in virmanTable)

         // 02.06.2016: SEPA; skip eventual unwanted VirmanBtchBookgKind 
         foreach(DS_Placa.virmanRow unwantedVirmanRow in virmanRowsToBeRemoved)
         {
            virmanTable.Rows.Remove(unwantedVirmanRow);
         }

         #endregion Fill VirmanList (bussiness for ZNP)

      } // if(VIRMANI) 

      #endregion VIRMANI Additions

      #region Lista Banka Additions

      if(this is RptP_ListaBanka)
      {
         DS_Placa.virmanRow virmanRow;
         Prjkt              cp = ZXC.CURR_prjkt_rec;
         object[]           stdParams;

         stdParams = new object[] { cp.Naziv, cp.Ulica1, cp.PostaBr + " " + cp.Grad, RptFilter.Ziro1, cp.Grad };

         var personBankaOnlyList       = personTable.Where(per => ((Person.VrstaIsplateEnum)per.vrstaIsplate) == Person.VrstaIsplateEnum.BANKA);
         var personBankaOnlyWmoneyList = 
            personBankaOnlyList
            .GroupJoin(ptransTable, per => per.personCD, ptr => ptr.t_personCD, (per, ptrList) => new 
            { 
               per, 
               personMoney = ptrList.Sum(ptr => ptr.R_NaRuke),
               firstPtransT_spc = ptrList.First().t_spc,
               firstPtransT_isMioII = ptrList.First().t_isMioII,
            });
         var personTekuciWkupdobList   = 
            personBankaOnlyWmoneyList
            .Join(kupdobTable, perWmoney => perWmoney.per.banka_cd, kpdb => kpdb.kupdobCD, (perWmoney, kpdb) => new 
         {
            perMoney             = perWmoney.personMoney, 
            firstPtransT_spc     = perWmoney.firstPtransT_spc,
            firstPtransT_isMioII = perWmoney.firstPtransT_isMioII,
            banNaziv             = kpdb.naziv,
            banUlica             = kpdb.ulica1,
            banZipGrad           = kpdb.postaBr + " " + kpdb.grad,
            banGrad              = kpdb.grad,
            banZiroRn            = ZXC.GetIBANfromOldZiro(kpdb.ziro1),
            banPoslovn           = kpdb.ziro1PnbV,
            perPnbM              = perWmoney.per.pnbM,
            perPnbV              = perWmoney.per.pnbV,
          //perPrezIme           = perWmoney.per.prezime + ", " + perWmoney.per.ime
            perPrezIme           = perWmoney.per.ime + " " + perWmoney.per.prezime
         });

         // 07.01.2015: 
         TheVirmanList = new List<VirmanStruct>();

         foreach(var perWkpdb in personTekuciWkupdobList)
         {                                                                                                                              //TODOTAMQ ovdje dolazi samo 1 ptrans
            virmanRow = Create_Netto_Virman(ZXC.VirmanEnum.NET, virmanTable, stdParams, perWkpdb.perMoney, placa_rec.TT, /* do 14.03.2014. placa_rec.MMYY*/ placa_rec.RSm_ID, GetX_pnbz(placa_rec, (Ptrans.SpecEnum)perWkpdb.firstPtransT_spc, ((int)perWkpdb.firstPtransT_isMioII).IsZero()));

            if(virmanRow == null) continue;
         
            virmanRow.prim1   = perWkpdb.banNaziv; 
            virmanRow.prim2   = perWkpdb.banUlica;
            virmanRow.prim3   = perWkpdb.banZipGrad;
            virmanRow.mjesto2 = perWkpdb.banGrad;
            virmanRow.ziro2   = ZXC.GetIBANfromOldZiro(perWkpdb.banZiroRn);
            virmanRow.statOb  = perWkpdb.banPoslovn;
          //08.12.2015. kada je NP onda HR99 + prazno
          //virmanRow.pnboMod = perWkpdb.perPnbM;
          //virmanRow.pnboMod = ZXC.GetPnbModfromOldPnbMod(virmanRow.pnboMod); //16.01.2013.
            virmanRow.pnboMod = (placa_rec.TT == Placa.TT_NEOPOREZPRIM) ? "HR99" : perWkpdb.perPnbM;
            virmanRow.pnboMod = (placa_rec.TT == Placa.TT_NEOPOREZPRIM) ? ""     : ZXC.GetPnbModfromOldPnbMod(virmanRow.pnboMod); //16.01.2013.
            virmanRow.pnbo    = perWkpdb.perPnbV;

            virmanRow.opisPl = perWkpdb.perPrezIme;

            // 06.07.2015. da se lista za banku moze napraviti unaprijed
            virmanRow.datePodnos = rptFilter.VirDatePod;

            // 07.01.2015: 
            TheVirmanList.Add(new VirmanStruct(virmanRow));

         }

      } // if(this is RptP_ListaBanka) 
      
      #endregion Lista Banka Additions

      #region Remove Unaproppriate ptranoRow.t_isZbir, ...

      if(this.DajSamoZbirneObustave == true)
      {
         foreach(DS_Placa.ptranoRow ptranoRow in ptranoRowsToBeRemoved)
         {
            ptranoTable.Rows.Remove(ptranoRow);
         }

         ptranoRowsToBeRemoved.Clear(); // treba li ovo? 
      }

      #endregion Remove Unaproppriate zakas, SkipImaloNaplacene

      #region RAD1, RAD1_G

      if(this is RptP_RAD1_G)
      {
         ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Person>(null, VvSQL.SorterType.None);

         foreach(var person in personTable)
         {
            person.R_StarostNa3103 = VvUserControl.PersonSifrar.SingleOrDefault(per => per.PersonCD == person.personCD).StarostUgodinama_Na_31_03_ProjectYear;
            person.R_StarosnaGr    = (byte)VvUserControl.PersonSifrar.SingleOrDefault(per => per.PersonCD == person.personCD).RAD1G_StarosnaGrupa            ;
         }

         Ptrans PtransSumRec_alfa13;
         List<Ptrans> thisPersonWholeYearPtransList;
         int mjeseciCount;
         bool thisPersonHasNot12Placa;
         bool thisPersonHasSkracenoRV;
         Person person_rec;
         uint personCD;

         for(int rowIdx = 0; rowIdx < ptransSumTable.Rows.Count; ++rowIdx)
         {
            personCD = ((DS_Placa.ptransSumRow)ptransSumTable.Rows[rowIdx]).P_personCD;

            PtransSumRec_alfa13 = PtransDao.GetPtransSum_alfa13(ZXC.TheVvForm.TheDbConnection /* poboljsaj ovo! */,
                                                               personCD, out thisPersonWholeYearPtransList);

            mjeseciCount = thisPersonWholeYearPtransList.Where(pt => pt.T_TT == Placa.TT_REDOVANRAD).Select(ptr => ptr.T_dokDate.Month).Distinct().Count();

            thisPersonHasNot12Placa = (mjeseciCount < 12);

            if(thisPersonHasNot12Placa)
            {
               ptransSumTable.Rows.RemoveAt(rowIdx--);
            }

            // 25.11.2024: 
            person_rec = VvUserControl.PersonSifrar.SingleOrDefault(per => per.PersonCD == personCD);
            thisPersonHasSkracenoRV = person_rec.VrstaRadVrem == Person.VrstaRadnogVremenaEnum.SKRACENO;
            if(thisPersonHasSkracenoRV)
            {
               ptransSumTable.Rows.RemoveAt(rowIdx--);
            }
         }

      } // if(this is RptP_RAD1_G)

      #region RAD1

      if(this is RptP_RAD1)
      {
         Rad1PersonCount_1all = Rad1PersonCount_1zene =
         Rad1PersonCount_2all = Rad1PersonCount_2zene =
         Rad1PersonCount_3all = Rad1PersonCount_3zene =
         Rad1PersonCount_4all = Rad1PersonCount_4zene = 0;

         DateTime dateP, dateO, dateRPTstartMonth = DateTime.MinValue, dateRPTendMonth = DateTime.MinValue;

       //if(RptFilter.ZaMjesecChecked) dateRPTstartMonth = Placa.GetDateTimeFromMMYYYY(RptFilter.ZaMjesec, false);
       //if(RptFilter.ZaMjesecChecked) dateRPTendMonth   = Placa.GetDateTimeFromMMYYYY(RptFilter.ZaMjesec, true );
         if(RptFilter.UMjesecuChecked)
         {
            dateRPTstartMonth = Placa.GetDateTimeFromMMYYYY(RptFilter.UMjesecu, false);
            dateRPTendMonth   = Placa.GetDateTimeFromMMYYYY(RptFilter.UMjesecu, true);
         }
         else { ZXC.aim_emsg(MessageBoxIcon.Error, "Filter mora biti 'U mjesecu'\n\nPodaci će biti netočni!"); return false; }

         DateTime dateRPTendPrevMonth = dateRPTendMonth.AddMonths(-1);
         bool isZena;

         ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Person>(null, VvSQL.SorterType.None);

       //foreach(DS_Placa.personRow personRow in personTable.Rows)                                            
       //foreach(Person             personRow in VvUserControl.PersonSifrar)                                  
         foreach(Person             personRow in VvUserControl.PersonSifrar.Where(prs => prs.IsPlaca == true))
         {
          //dateP  = personRow.datePri  ;
          //dateO  = personRow.dateOdj  ;
          //isZena = personRow.spol == 2;
            dateP  = personRow.DatePri;
            dateO  = personRow.DateOdj;
            isZena = personRow.Spol == ZXC.Spol.ZENSKO;

            if((dateP.IsEmpty() || dateP <  dateRPTstartMonth   ) && 
               (dateO.IsEmpty() || dateO >= dateRPTendPrevMonth)) { Rad1PersonCount_1all++; if(isZena) Rad1PersonCount_1zene++; }

          //if((dateP.IsEmpty() || (dateP < dateRPT          && Placa.Get_uMMYYYY(dateP) != Placa.Get_uMMYYYY(dateRPT))) &&
          //   (dateO.IsEmpty() || (dateO > dateRPTprevMonth && Placa.Get_uMMYYYY(dateO) != Placa.Get_uMMYYYY(dateRPT)))
          //                                                         ) { Rad1PersonCount_1all++; if(isZena) Rad1PersonCount_1zene++; }

            if(Placa.Get_uMMYYYY(dateP) == Placa.Get_uMMYYYY(dateRPTendMonth)) { Rad1PersonCount_2all++; if(isZena) Rad1PersonCount_2zene++; }
            if(Placa.Get_uMMYYYY(dateO) == Placa.Get_uMMYYYY(dateRPTendMonth)) { Rad1PersonCount_3all++; if(isZena) Rad1PersonCount_3zene++; }

            Rad1PersonCount_4all  = Rad1PersonCount_1all  + Rad1PersonCount_2all  - Rad1PersonCount_3all ;
            Rad1PersonCount_4zene = Rad1PersonCount_1zene + Rad1PersonCount_2zene - Rad1PersonCount_3zene;
         }


         // RAD1: 4. PLACENI SATI RADA [prekovremeni, bolovanje do 42, godisnji odmor, blagdani i neradni, ostali placeni a neizvrseni]::: 
         Rad1Sati_prekovr = 0M;
         Rad1Sati_bolDo42 = 0M;
         Rad1Sati_godOdmr = 0M;
         Rad1Sati_blagInr = 0M;
         Rad1Sati_ostalip = 0M;

         decimal               blagdanSatiUmjesecuZaUmjetniPtrane;
         DS_Placa.placaRow     placaOfUmjetniPtrane              ;
         DS_Placa.IzvjTableRow ptransOfUmjetniPtrane             ;

         foreach(var ptraneSum in ptraneSumList)
         {
            //ptraneSumTable.Rows.Add(ptraneSum.vrstaR_cd, ptraneSum.vrstaR_name, ptraneSum.cijPerc, ptraneSum.satiSum, ptraneSum.evrBrutoSum);
            if(Ptrane.IsBolDo42(ptraneSum.rsOO, ptraneSum.cijPerc, ptraneSum.vrstaR_name))
            {
               Rad1Sati_bolDo42 += ptraneSum.satiSum;
            }
            if(Ptrane.IsGodisnjiOdmor(ptraneSum.rsOO, ptraneSum.vrstaR_name))
            {
               Rad1Sati_godOdmr += ptraneSum.satiSum;
            }
            if(Ptrane.IsPrekovremeno(ptraneSum.rsOO))
            {
               Rad1Sati_prekovr += ptraneSum.satiSum;
            }
            if(Ptrane.IsBlagdan(ptraneSum.cijPerc, ptraneSum.vrstaR_name))
            {
               Rad1Sati_blagInr += ptraneSum.satiSum;
            }
         }

         // Za umjetno dodane ptrane-ove, ako zahvaca blagdan
         foreach(var umjetniPtrane in ptrWOptranesAsPtraneShortTable)
         {
            placaOfUmjetniPtrane = placaTable.Single(pl => pl.recID == umjetniPtrane.t_parentID);
            ptransOfUmjetniPtrane = ptransTable.Single(ptr => ptr.t_parentID == umjetniPtrane.t_parentID && ptr.t_personCD == umjetniPtrane.t_personCD);

            if(ptransOfUmjetniPtrane.t_tt == Placa.TT_PLACAUNARAVI) continue;

            blagdanSatiUmjesecuZaUmjetniPtrane = ZXC.GetSumaBlagdanskihRadnihSatiZaMjesec(ptransOfUmjetniPtrane.t_mmyyyy, ((int)placaOfUmjetniPtrane.isTrgFondSati).IsZero() ? false : true, ((int)ptransOfUmjetniPtrane.t_isPoluSat).IsZero() ? false : true, ptransOfUmjetniPtrane.t_dnFondSati);
            Rad1Sati_blagInr += blagdanSatiUmjesecuZaUmjetniPtrane;
         }

         // SINT all ONPN placaTable.placaRows as one 
         if(placaTable.Any(pl => pl.S_ONPNbruto.NotZero()))
         {
            var placaTableONPNlist = placaTable.Where(pl => pl.S_ONPNbruto.NotZero()).ToList();
            
            DS_Placa.placaRow placaRowSINTonpn = (DS_Placa.placaRow)placaTable.Rows.Add();
            
            placaRowSINTonpn.S_ONPNbruto = placaTableONPNlist.Sum(pl => pl.S_ONPNbruto);
            placaRowSINTonpn.S_ONPNnetto = placaTableONPNlist.Sum(pl => pl.S_ONPNnetto);

            int zaposlenihONPN = ptransTable.Where(ptr => ptr.R_bruto_ONPN.NotZero()).Select(ptr => ptr.t_personCD).Distinct().Count();

            placaRowSINTonpn.S_ONPNptrCount = zaposlenihONPN;

            for(int i = 0; i < placaTableONPNlist.Count; ++i)
            {
               placaTable.RemoveplacaRow(

                  (placaTableONPNlist[i])

                  );
            }
          //foreach(var onpnPlacaRow in placaTableONPN)
          //{
          //   placaTable.RemoveplacaRow(onpnPlacaRow);
          //}
         }

      } // if(this is RptP_RAD1)

      #endregion RAD1

      #endregion RAD1, RAD1_G

      #region joppd oib.Empty

      if(this is RptP_JOPPD)
      {
         foreach(var person in personTable)
         {
            if((person.oib.IsEmpty()))
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "OIB nedostaje! Djelatnik [{0}] [{1}]", person.ime, person.prezime);
            }
         }
      }

      #endregion joppd oib.Empty
     
      #region MaticniPodaciradnika u nizu

      if(this is RptP_PersonMaticniPodaci)
      {
         for(int rowIdx = 0; rowIdx < personTable.Rows.Count; ++rowIdx)
         {
            if(personTable[rowIdx].isPlaca == 0)
            {
               personTable.Rows.RemoveAt(rowIdx--);
            }
         }
      }

      #endregion MaticniPodaciradnika u nizu

      return true;
   }

   private bool Get_hasMixed_aJeTo(DS_Placa.placaDataTable placaTable)
   {
      //pvi.aJePoduzetPl   = placa_rec.TT == Placa.TT_PODUZETPLACA;
      //pvi.aJeAutHonorar  = placa_rec.TT == Placa.TT_AUTORHONOR  || placa_rec.TT == Placa.TT_NR1_PX1NEDOP || placa_rec.TT == Placa.TT_NR2_P01NEDOP || placa_rec.TT == Placa.TT_DDBEZDOPRINO;
      //pvi.aJeAutHonUmje  = placa_rec.TT == Placa.TT_AUTORHONUMJ || placa_rec.TT == Placa.TT_AUVECASTOPA  || placa_rec.TT == Placa.TT_NR3_PX1DADOP;
      //pvi.aJeAHSamostUm  = placa_rec.TT == Placa.TT_AHSAMOSTUMJ;
      //pvi.aJeUgovODjelu  = placa_rec.TT == Placa.TT_UGOVORODJELU ;
      //pvi.aJeNadzorOdb   = placa_rec.TT == Placa.TT_NADZORODBOR || placa_rec.TT == Placa.TT_TURSITVIJECE;
      //pvi.aJeIDD_Kolona4 = placa_rec.TT == Placa.TT_IDD_KOLONA_4;
      //pvi.aJeSezZapPolj  = placa_rec.TT == Placa.TT_SEZZAPPOLJOP;
      //pvi.aJePorNaDobit  = placa_rec.TT == Placa.TT_POREZNADOBIT;
      //pvi.aJeStrucOspos  = placa_rec.TT == Placa.TT_STRUCNOOSPOS;
      //pvi.aJeSamoDop     = placa_rec.TT == Placa.TT_SAMODOPRINOS;

      //private static string[] arrayRRsetTT = new string[] {
      //   Placa.TT_REDOVANRAD   /*"RR"*/, // redovan rad                                                              
      //   Placa.TT_PODUZETPLACA /*"PP"*/, // poduzetnicka placa                                                       
      //   Placa.TT_PLACAUNARAVI /*"PN"*/, // placa u naravi                                                           
      //   Placa.TT_NEPLACDOPUST /*"ND"*/, // 25.11.14. neplaceni dopust                                               
      //   Placa.TT_BIVSIRADNIK  /*"BR"*/, // 23.12.2019. Obračun primitaka prema kojima se doprinosi obračunavaju na način koji ima obilježje drugog dohotka, a porez na dohodak prema primitcima od kojih se utvrđuje dohodak od nesamostalnog rada
      //   Placa.TT_OSTALIPRIM   /*"OP"*/, // 01.01.2024. Ostali primici - iznad neoporezivog iznosa - koristi spent za oporz ali ne i za MIO1olaksicu
      //};

      return placaTable.ToList().Select(pl => pl.tt).Distinct().Count().MoreThenOne();
   }

   private string Get_NP_theVOP(string pnbo, string t_neoPrimCD)
   {
      string newTheVOP; // dolazno "690"

      switch(t_neoPrimCD)
      {
         case "17": newTheVOP = "-210"; break;
         case "18": newTheVOP = "-240"; break;
         case "19": newTheVOP = "-190"; break;
         case "20": newTheVOP = "-330"; break;
         case "21": newTheVOP = "-280"; break;
         case "22": newTheVOP = "-260"; break;
         case "23": newTheVOP = "-210"; break;
         case "24": newTheVOP = "-451"; break;
         case "25": newTheVOP = "-220"; break;
         case "26": newTheVOP = "-320"; break;
         case "28": newTheVOP = "-290"; break;
         case "52": newTheVOP = "-200"; break;
         case "60": newTheVOP = "-690"; break;
         case "61": newTheVOP = "-270"; break;
         case "63": newTheVOP = "-250"; break;
         case "65": newTheVOP = "-191"; break;
         case "66": newTheVOP = "-191"; break;

         default: newTheVOP = defaultOrUnknownVOPforPlacaTT; break;
      }

      return pnbo.Replace(defaultOrUnknownVOPforPlacaTT, newTheVOP);
   }

   //private       string GetPnbM(string pnbM, string pnb)
   public static string GetPnbM(string pnbM, string pnb)
   {
           if(pnbM.IsEmpty() && pnb.IsEmpty())  return "HR99";
      else if(pnbM.IsEmpty() && pnb.NotEmpty()) return "HR00";
           else                                 return  pnbM ;

   }

 //private static string defaultOrUnknownVOPforPlacaTT = "-399";
   private static string defaultOrUnknownVOPforPlacaTT = "-690"; // mozda 699, pitanje je da li je izuzeto ili ne od ovrhe

   private string GetVOPformPlacaTT(string placaTT, string ziroSubStr)
   {
      switch(placaTT)
      {
       //case Placa.TT_REDOVANRAD  : //23.12.2019.
         case Placa.TT_REDOVANRAD  :
         case Placa.TT_BIVSIRADNIK :
         case Placa.TT_PODUZETPLACA: // dodano 10.03.2021.
                 if(     ziroSubStr == Zastic_IBAN_root) return "-110";
                 else if(ziroSubStr == Normal_IBAN_root) return "-100";
                 else                                    return "-120";

         case Placa.TT_UGOVORODJELU:    return "-130";

         case Placa.TT_NADZORODBOR :
         case Placa.TT_TURSITVIJECE:    return "-160";
         //09.06.2020.
         case Placa.TT_POREZNADOBIT:    return "-150";
         
       //default:                       return                        "-399";
         default:                       return defaultOrUnknownVOPforPlacaTT;
      }
   }
 
   private string GetOpisPlacanjaformPlacaTT(string placaTT)
   {
      switch(placaTT)
      {
         case Placa.TT_REDOVANRAD  : 
         case Placa.TT_PODUZETPLACA: return "Plaća za ";
         case Placa.TT_OSTALIPRIM  : return "Ostali primici ";
         case Placa.TT_NEOPOREZPRIM: return "Neoporezivi primitci za ";
         default:                    return "Drugi dohodak za ";
      }
   }

   #region VIRMANI Methods

   private bool IsThisVirmanWanted(ZXC.VirmanEnum virmanEnum)
   {
      switch(virmanEnum)
      {
         case ZXC.VirmanEnum.KRP    : return(RptFilter.PrintKRP   );
         case ZXC.VirmanEnum.MIO1   : return(RptFilter.PrintMIO1  );
         case ZXC.VirmanEnum.MIO1NA : return(RptFilter.PrintMIO1NA);
         case ZXC.VirmanEnum.MIO2   : return(RptFilter.PrintMIO2  );
         case ZXC.VirmanEnum.MIO2NA : return(RptFilter.PrintMIO2NA);
         case ZXC.VirmanEnum.NET    : return(RptFilter.PrintNET   );
         case ZXC.VirmanEnum.POR    : return(RptFilter.PrintPOR   );
         case ZXC.VirmanEnum.PRE    : return(RptFilter.PrintPRE   );
         case ZXC.VirmanEnum.PRI    : return(RptFilter.PrintPRI   );
         case ZXC.VirmanEnum.ZAP    : return(RptFilter.PrintZAP   );
         case ZXC.VirmanEnum.ZDR    : return(RptFilter.PrintZDR   );
         case ZXC.VirmanEnum.ZOR    : return(RptFilter.PrintZOR   );
         case ZXC.VirmanEnum.ZPI    : return(RptFilter.PrintZPI   );
         case ZXC.VirmanEnum.ZPP    : return(RptFilter.PrintZPP   );

         default: ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Virman Enum {0} nepoznat u Reports_PIZ.IsThisVirmanWanted()!", virmanEnum.ToString()); return true;
      }
   }

   private DS_Placa.virmanRow Create_Doprinosi_Virman(ZXC.VirmanEnum virmanEnum, DS_Placa.virmanDataTable virmanTable, object[] stdParams, decimal money, string placaTT, bool isPlacaBef2014, DateTime dokDate)
   {
      if(money.IsZero()) return null; // !!! 

      if(IsThisVirmanWanted(virmanEnum) == false) return null;

      DS_Placa.virmanRow virmanRow = (DS_Placa.virmanRow)virmanTable.Rows.Add(stdParams);

      virmanRow.money = money;

      virmanRow.prim1   = "DRŽAVNI PRORAČUN";
      virmanRow.ziro2   = standardGovernmentZiroRacun;

      virmanRow.pnbz    = Kplan.GetPlacaKonta_New(virmanEnum, ZXC.SaldoOrDugOrPot.POT, placaTT);

    //virmanRow.sifraPl = "08";

      string specificPNBOstart = "";
      string stdPNBOend        = "-" + pvi.oib_inUse + "-" + pvi.RSm_ID_inUse;

      //30.12.2016. dopinosi idu i na AH i AU od 01.01.2017.
      bool isNew2017Placa = dokDate >= ZXC.Date01012017;

      #region Switch

      switch(virmanEnum)
      {
         case ZXC.VirmanEnum.MIO1:

            virmanRow.prim2 = virmanRow.opisPl = "Doprinos za MIO I stup";
            virmanRow.prim3 = "A. Mihanovića 3";

            if(isPlacaBef2014)
            {
                    if(pvi.aJePoduzetPl)  specificPNBOstart = "8303";
               else if(pvi.aJeObrtnik)    specificPNBOstart = "8117";
               else if(pvi.aJeStranger)   specificPNBOstart = "8109"; 
               else if(pvi.aJeUgovODjelu) specificPNBOstart = "8290";
               else if(pvi.aJeNadzorOdb)  specificPNBOstart = "8290";
               else                       specificPNBOstart = "8109";
            }
            else
            {
                    if(pvi.aJePoduzetPl)  specificPNBOstart = "8184";
//             else if(pvi.aJeObrtnik)    specificPNBOstart = "8168"; 22.05.2014. davalo je krivo kada je UD kod obrtnika a tak je isti kao else
//             else if(pvi.aJeStranger)   specificPNBOstart = "8168"; 22.05.2014. isti kao else
               else if(pvi.aJeUgovODjelu) specificPNBOstart = "8176";
               else if(pvi.aJeNadzorOdb)  specificPNBOstart = "8176";
               else if((pvi.aJeAutHonorar ||                          // 30.12.2016.
                        pvi.aJeAutHonUmje) && 
                        isNew2017Placa)   specificPNBOstart = "8176";
               else if(pvi.aJeSamoDop  )  specificPNBOstart = "8230"; //09.02.2016.
               else                       specificPNBOstart = "8168";
            
            }

            break;

         case ZXC.VirmanEnum.MIO2:

            // 28.12.2012: 
          //virmanRow.ziro2 = "1001005-1700036001";
            virmanRow.ziro2 = "HR7610010051700036001";
            virmanRow.prim2 = virmanRow.opisPl = "Doprinos za MIO II stup";
            virmanRow.prim3 = "A. Mihanovića 3";

            if(isPlacaBef2014)
            {
                    if(pvi.aJePoduzetPl)  specificPNBOstart = "2216";
               else if(pvi.aJeObrtnik)    specificPNBOstart = "2011";
               else if(pvi.aJeStranger)   specificPNBOstart = "2003"; 
               else if(pvi.aJeUgovODjelu) specificPNBOstart = "2194";
               else if(pvi.aJeNadzorOdb)  specificPNBOstart = "2194";
               else                       specificPNBOstart = "2003";
            }
            else
            {
                    if(pvi.aJePoduzetPl)  specificPNBOstart = "2305";
//             else if(pvi.aJeObrtnik)    specificPNBOstart = "2283"; 22.05.2014. davalo je krivo kada je UD kod obrtnika a tak je isti kao else
//             else if(pvi.aJeStranger)   specificPNBOstart = "2283"; 22.05.2014. isti kao else
               else if(pvi.aJeUgovODjelu) specificPNBOstart = "2291";
               else if(pvi.aJeNadzorOdb)  specificPNBOstart = "2291";
               else if((pvi.aJeAutHonorar ||                          // 30.12.2016.
                        pvi.aJeAutHonUmje) && 
                        isNew2017Placa)   specificPNBOstart = "2291";
               else if(pvi.aJeSamoDop  )  specificPNBOstart = "2330";
               else                       specificPNBOstart = "2283";
            }

            break;

         case ZXC.VirmanEnum.ZDR:

            //29.12.2014. za virmane od 1.1.2015.
            if(dokDate >= ZXC.Date01012015)
            {
               virmanRow.prim1 = "Hrvatski zavod za zdravstveno osiguranje";
               virmanRow.ziro2 = "HR6510010051550100001";
            }


            virmanRow.prim2 = virmanRow.opisPl = "Dopr. za ZDRAVSTVENO osiguranje";
            virmanRow.prim3 = "Margaretska 3";

            if(isPlacaBef2014)
            {
                    if(pvi.aJePoduzetPl)  specificPNBOstart = "8680";
               else if(pvi.aJeObrtnik)    specificPNBOstart = "8419";
               else if(pvi.aJeStranger)   specificPNBOstart = "8427";
               else if(pvi.aJeUgovODjelu) specificPNBOstart = "8664";
               else if(pvi.aJeNadzorOdb)  specificPNBOstart = "8664";
               else                       specificPNBOstart = "8400";
            }
            else
            {
                    if(pvi.aJePoduzetPl)  specificPNBOstart = "8494";
//             else if(pvi.aJeObrtnik)    specificPNBOstart = "8486"; 22.05.2014. davalo je krivo kada je UD kod obrtnika a tak je isti kao else
//             else if(pvi.aJeStranger)   specificPNBOstart = "8486"; 22.05.2014. isti kao else
               else if(pvi.aJeUgovODjelu) specificPNBOstart = "8540";
               else if(pvi.aJeNadzorOdb)  specificPNBOstart = "8540";
               else if((pvi.aJeAutHonorar ||                          // 30.12.2016.
                        pvi.aJeAutHonUmje) && 
                        isNew2017Placa)   specificPNBOstart = "8540";
               else if(pvi.aJeSamoDop  )  specificPNBOstart = "8605";
               else                       specificPNBOstart = "8486";
            }

            break;

         case ZXC.VirmanEnum.ZOR:
 
         //29.12.2014. za virmane od 1.1.2015.
            if(dokDate >= ZXC.Date01012015)
            {
               virmanRow.prim1 = "Hrvatski zavod za zdravstveno osiguranje";
               virmanRow.ziro2 = "HR6510010051550100001";
            }

            virmanRow.prim2 = virmanRow.opisPl = "Dopr za ZOs od ozljedNaRadu";
            virmanRow.prim3 = "Margaretska 3";

            if(isPlacaBef2014)
            {
                    if(pvi.aJePoduzetPl)  specificPNBOstart = "8699";
               else if(pvi.aJeObrtnik)    specificPNBOstart = "8567";
               else if(pvi.aJeStranger)   specificPNBOstart = "8575";
               else                       specificPNBOstart = "8559";
            }
            else
            {
                    if(pvi.aJePoduzetPl)  specificPNBOstart = "8648";
//             else if(pvi.aJeObrtnik)    specificPNBOstart = "8630"; 22.05.2014. isti kao else
//             else if(pvi.aJeStranger)   specificPNBOstart = "8630"; 22.05.2014. isti kao else
               else if(pvi.aJeSamoDop )   specificPNBOstart = "8133"; // 09.02.2016.
               else                       specificPNBOstart = "8630";
            }

            break;

         case ZXC.VirmanEnum.ZPI:
            
         //03.06.2015. za virmane od 1.1.2015.
            if(dokDate >= ZXC.Date01012015)
            {
               virmanRow.prim1 = "Hrvatski zavod za zdravstveno osiguranje";
               virmanRow.ziro2 = "HR6510010051550100001";
            }

            virmanRow.prim2 = virmanRow.opisPl = "Dopr za ZOs za put u inozemstvo";
            virmanRow.prim3 = "Margaretska 3";

            if(isPlacaBef2014)
            {
               specificPNBOstart = "8443";
            }
            else
            {
               specificPNBOstart = "8508";
            }

            break;

         case ZXC.VirmanEnum.ZAP:

            virmanRow.prim2 = virmanRow.opisPl = "Doprinos za ZAPOŠLJAVANJE";
            virmanRow.prim3 = "DZAP";

            if(isPlacaBef2014)
            {
               specificPNBOstart = "8702";
            }
            else
            {
               // 04.02.2014. i poduzetnik placa za isplate place 012014 pa nadalje
                    if(pvi.aJePoduzetPl) specificPNBOstart = "8788";
               else if(pvi.aJeSamoDop  ) specificPNBOstart = "8796";
               else                      specificPNBOstart = "8753";
            }
            break;

         case ZXC.VirmanEnum.ZPP:

            virmanRow.prim2 = virmanRow.opisPl = "Doprinos za ZAPOŠLJAVANJE 2";
            virmanRow.prim3 = "DZAP2";

            if(isPlacaBef2014)
            {
               specificPNBOstart = "8729";
            }
            else
            {
               // 04.02.2014. i poduzetnik placa  jos ne znamo da li se dijeli ili ne  za isplate place 012014 pa nadalje
               //if(pvi.aJePoduzetPl) specificPNBOstart = "8788";
               specificPNBOstart = "8761";
            }

            break;
       
         case ZXC.VirmanEnum.MIO1NA:

            virmanRow.prim2 = virmanRow.opisPl = "Doprinos za MIO I staž s povećanim trajanjem";
            virmanRow.prim3 = "A. Mihanovića 3";

            if(isPlacaBef2014)
            {
               specificPNBOstart = "8150";
            }
            else
            {
               specificPNBOstart = "8192";
            }

            break;
       
         case ZXC.VirmanEnum.MIO2NA:

            // 28.12.2012: 
            //virmanRow.ziro2 = "1001005-1700036001";
            virmanRow.ziro2 = "HR7610010051700036001";
            virmanRow.prim2 = virmanRow.opisPl = "Doprinos za MIO II staž s povećanim trajanjem";
            virmanRow.prim3 = "A. Mihanovića 3";

            if(isPlacaBef2014)
            {
               specificPNBOstart = "2020";
            }
            else
            {
               specificPNBOstart = "2321";
            }
            break;

      }

      #endregion Switch

      virmanRow.pnboMod = "68";
      virmanRow.pnboMod = ZXC.GetPnbModfromOldPnbMod(virmanRow.pnboMod); //16.01.2013.

      virmanRow.pnbo    = specificPNBOstart + stdPNBOend;

      virmanRow.virKind = (int)virmanEnum;

      virmanRow.pnbzMod = GetPnbM(virmanRow.pnbzMod, virmanRow.pnbz); 

      return virmanRow;
   }

   private DS_Placa.virmanRow Create_PorezPrirez_Virman(ZXC.VirmanEnum virmanEnum, DS_Placa.virmanDataTable virmanTable, object[] stdParams, decimal money, decimal stopaPri, string opcCD, string opcName, decimal prirezOsnova, string oib, string prezimeIme, string placaTT, bool isPlacaBef2014)
   {
      if(money.IsZero()) return null; // !!! 

      if(IsThisVirmanWanted(virmanEnum) == false) return null;

      DS_Placa.virmanRow virmanRow = (DS_Placa.virmanRow)virmanTable.Rows.Add(stdParams);

      virmanRow.money = money;

      virmanRow.prim1   = "DRŽAVNI PRORAČUN";

      virmanRow.pnbz    = Kplan.GetPlacaKonta_New(virmanEnum, ZXC.SaldoOrDugOrPot.POT, placaTT);

    //virmanRow.sifraPl = "08";

      string specificPNBOstart;

      if(isPlacaBef2014)
      {
              if(pvi.aJeAutHonorar)  specificPNBOstart = "1465";
         else if(pvi.aJeAutHonUmje)  specificPNBOstart = "1830";
         else if(pvi.aJeUgovODjelu)  specificPNBOstart = "1813";
         else if(pvi.aJeNadzorOdb)   specificPNBOstart = "1457";
         else                        specificPNBOstart = "1406";
      }
      else
      {
              if(pvi.aJeAutHonorar)  specificPNBOstart = "1945";
         else if(pvi.aJeAutHonUmje)  specificPNBOstart = "1945";
         else if(pvi.aJeAHSamostUm)  specificPNBOstart = "1945";
         else if(pvi.aJeUgovODjelu)  specificPNBOstart = "1945";
         else if(pvi.aJeNadzorOdb )  specificPNBOstart = "1945";
         else if(pvi.aJeSezZapPolj)  specificPNBOstart = "1945";
         else if(pvi.aJePorNaDobit)  specificPNBOstart = "1910";
         else                        specificPNBOstart = "1880";
      }

      string stdPNBOend        = "-" + oib + "-" + pvi.RSm_ID_inUse;

      string str4ISOcheck = ZXC.GetStr4ISOcheck(opcCD, "1200");

      virmanRow.ziro2     = "1001005-" + str4ISOcheck + ZXC.GetISO7064(str4ISOcheck);

      // 28.12.2012: 
      virmanRow.ziro2 = ZXC.GetIBANfromOldZiro(virmanRow.ziro2);

      #region Switch

      switch(virmanEnum)
      {
         case ZXC.VirmanEnum.POR:

            virmanRow.prim2  = "POREZ NA DOHODAK";
            virmanRow.prim3  = "PU " + opcName;
            if(pvi.aJeOtherDohodak) virmanRow.opisPl = prezimeIme      + " " + virmanRow.prim2;
            else                    virmanRow.opisPl = virmanRow.prim3 + " " + virmanRow.prim2;

            break;

         case ZXC.VirmanEnum.PRI:

            virmanRow.prim2 = "PRIREZ porezu na dohodak";
            virmanRow.prim3 = "Prirez " + opcName;

            // 14.11.2022: nekom tam parseru smeta "%" 
          //if(pvi.aJeOtherDohodak) virmanRow.opisPl = prezimeIme      + " " + stopaPri + @"%(" + prirezOsnova + ")";
          //else                    virmanRow.opisPl = virmanRow.prim3 + " " + stopaPri + @"%(" + prirezOsnova + ")";
            if(pvi.aJeOtherDohodak) virmanRow.opisPl = prezimeIme      + " " + stopaPri + @"posto(" + prirezOsnova + ")";
            else                    virmanRow.opisPl = virmanRow.prim3 + " " + stopaPri + @"posto(" + prirezOsnova + ")";

            break;

      }

      #endregion Switch

      virmanRow.pnboMod = "68";
      virmanRow.pnboMod = ZXC.GetPnbModfromOldPnbMod(virmanRow.pnboMod); //16.01.2013.
      
      virmanRow.pnbo = specificPNBOstart + stdPNBOend;

      virmanRow.virKind = (int)virmanEnum;

      virmanRow.pnbzMod = GetPnbM(virmanRow.pnbzMod, virmanRow.pnbz); 

      return virmanRow;
   }

   private DS_Placa.virmanRow Create_KrizniPorez_Virman(ZXC.VirmanEnum virmanEnum, DS_Placa.virmanDataTable virmanTable, object[] stdParams, decimal money, string placaTT)
   {
      if(money.IsZero()) return null; // !!! 

      if(IsThisVirmanWanted(virmanEnum) == false) return null;

      DS_Placa.virmanRow virmanRow = (DS_Placa.virmanRow)virmanTable.Rows.Add(stdParams);

      virmanRow.money = money;

      virmanRow.prim1  = "DRŽAVNI PRORAČUN";
      virmanRow.prim2  =
      virmanRow.opisPl = "POSEBAN POREZ";
      virmanRow.ziro2  = standardGovernmentZiroRacun;

      virmanRow.pnbz    = Kplan.GetPlacaKonta_New(virmanEnum, ZXC.SaldoOrDugOrPot.POT, placaTT);

    //virmanRow.sifraPl = "08";

      string specificPNBOstart = "1902";
      string stdPNBOend        = "-" + pvi.oib_inUse + "-" + pvi.RSm_ID_inUse;

      virmanRow.pnboMod = "68";
      virmanRow.pnboMod = ZXC.GetPnbModfromOldPnbMod(virmanRow.pnboMod); //16.01.2013.
      
      virmanRow.pnbo = specificPNBOstart + stdPNBOend;

      virmanRow.virKind = (int)virmanEnum;

      return virmanRow;
   }

   #region Orig da Offix
   //getISO7064(string)
   //char      *string;
   //{
   //   char currChDigit[2];
   //   int i, ckDigit, cDg;
   //   int a, x, b, y, z; 
      
   //   b = 10; // inicijalni ostatak od '% 10'
      
   //   for(i=0; string[i]; ++i) {

   //      if(!isdigit(string[i])) { aim_emsg("<%s> contains non digit characters!", string); return(0); }
   //      sprintf(currChDigit, "%1.1s", string + i);
   //      z = atoi(currChDigit);
   ////aim_emsg("%d: String<%s> cChDg<%s> cDg<%d>", i+1, string, currChDigit, z);

   //      x = b + z;
   //      a = x % 10;
   //      if(!a) a = 10;
   //      y = 2 * a;
   //      b = y % 11;
   //   }
      
   //   ckDigit = 11 - b;

   //   if(ckDigit == 10)
   //      ckDigit = 0;
      
   //   return(ckDigit);
   //}
   #endregion Orig da Offix

   //private int GetX_pnbz(Placa placa_rec, Ptrans ptrans_rec)
   private int GetX_pnbz(Placa placa_rec, Ptrans.SpecEnum T_spc, bool T_isMioII)
   {
      if(placa_rec.TT == Placa.TT_AUTORHONOR  ) return  8;// od 2017 i honorarci placaju doprinose a mjenjam to tek 12.2018 u 8// od 01.07.2015. promjenjeno 4;
      if(placa_rec.TT == Placa.TT_AUTORHONUMJ ) return  8;// od 2017 i honorarci placaju doprinose a mjenjam to tek 12.2018 u 8// od 01.07.2015. promjenjeno 4;
      if(placa_rec.TT == Placa.TT_AHSAMOSTUMJ ) return  8;// od 2017 i honorarci placaju doprinose a mjenjam to tek 12.2018 u 8// od 01.07.2015. promjenjeno 4;
      if(placa_rec.TT == Placa.TT_AUVECASTOPA ) return  8;// od 2017 i honorarci placaju doprinose a mjenjam to tek 12.2018 u 8//12.2018. od 01.07.2015. promjenjeno 4;
      if(placa_rec.TT == Placa.TT_IDD_KOLONA_4) return 15;// od 01.07.2015. promjenjeno 4;
      if(placa_rec.TT == Placa.TT_SEZZAPPOLJOP) return 15;// od 01.07.2015. promjenjeno 4;
      if(placa_rec.TT == Placa.TT_NADZORODBOR ) return  8; // od 01.07.2015. promjenjeno 3;
      if(placa_rec.TT == Placa.TT_TURSITVIJECE) return  8; // od 01.07.2015. promjenjeno 3;

      //09.06.2020.
    //if(placa_rec.TT == Placa.TT_POREZNADOBIT) return  4; //13.05.2014. kao trebalo bi nista ali 4 je za one koji nemaju doprinosa pa bi trebalo biti ok
      if(placa_rec.TT == Placa.TT_POREZNADOBIT) return 12; 

      if(placa_rec.TT == Placa.TT_DDBEZDOPRINO) return 15; //12.2018 valjda ide tako jer je oso
      if(placa_rec.TT == Placa.TT_NR2_P01NEDOP) return 15; //12.2018. valjda ide tako 
      if(placa_rec.TT == Placa.TT_NR3_PX1DADOP) return  8; //12.2018. valjda ide tako 
      if(placa_rec.TT == Placa.TT_NR1_PX1NEDOP) return 15; //12.2018. valjda ide tako 

      if(placa_rec.TT == Placa.TT_UGOVORODJELU &&
                    T_spc == Ptrans.SpecEnum.PENZ) return 15; //4 od 01.07.2015. isplta drugog dohotka koji ne podlijeze oporezivanju;

    //if(T_isMioII == false) return 3; TODOTAMQ 23.05.2012. ovo krivo zvuci ali je tak jer dolazi obrnuto pa bi trebalo ispraviti u dolasku
      if(T_isMioII == true ) return 3;
      
      // 14.03.2014.
      if(T_spc == Ptrans.SpecEnum.NOVOZAPOSL   ) return 4;
      if(T_spc == Ptrans.SpecEnum.NOVO_MINMIONE) return 4; //18.12.2019.
    //if(isplata prvog  dijela place) return 1; 
    //if(isplata drugog dijela place) return 2; 

      if(placa_rec.TT == Placa.TT_UGOVORODJELU) return 8;

      // 06.07.2015.
      //Na klasicni ugovor o djelu ide 8, a na bezdoprinosni 15 ???

      // kazu da 8 ide s doprinosima a 15 za isplate oslobodene doprinosa
      return 0;
   }

   private DS_Placa.virmanRow Create_Netto_Virman(ZXC.VirmanEnum virmanEnum, DS_Placa.virmanDataTable virmanTable, object[] stdParams, decimal money, string placaTT, string mmyy, int X_pnbz)
   {

      if(money.IsZero()) return null; // !!! 

      if(IsThisVirmanWanted(virmanEnum) == false) return null;

      DS_Placa.virmanRow virmanRow = (DS_Placa.virmanRow)virmanTable.Rows.Add(stdParams);

      virmanRow.money   = money;

    //  virmanRow.pnbz = Kplan.GetPlacaKonta_New(virmanEnum, ZXC.SaldoOrDugOrPot.POT, placaTT);
    //04.05.2012. new nalog za isplatu place TODO!!!!!!!!
      virmanRow.pnbzMod = "67";
      virmanRow.pnbzMod = ZXC.GetPnbModfromOldPnbMod(virmanRow.pnbzMod); //16.01.2013.
      

      // 14.03.2014. od 15.03.2014. vrijedi novo pravilo - umjesto mmyy dolazi JoppdID
      virmanRow.pnbz = ZXC.CURR_prjkt_rec.Oib + "-" + mmyy + "-" + X_pnbz;


      //08.12.2015. da ne ispisuje kada je TT = Neoporezivi primitak odnosno HR99
      if(placaTT == Placa.TT_NEOPOREZPRIM)
      {
         virmanRow.pnbzMod = "HR99";
         virmanRow.pnbz = "";
      }
    
      
      //24.09.2014. da ne ispisuje gornji pnbz da ne skuzi da je placa Ag
      if(this.RptFilter.IsZnpBezPnb)
      {
         virmanRow.pnbzMod  = 
         virmanRow.pnbz     = "";
      }
 
    //virmanRow.sifraPl = "16";

      virmanRow.virKind = (int)virmanEnum;

      return virmanRow;
   }

   private DS_Placa.virmanRow Create_Obustava_Virman(ZXC.VirmanEnum virmanEnum, DS_Placa.virmanDataTable virmanTable, object[] stdParams, decimal money, string placaTT, string mmyy, int X_pnbz)
   {
      if(money.IsZero()) return null; // !!! 

      if(IsThisVirmanWanted(virmanEnum) == false) return null;

      DS_Placa.virmanRow virmanRow = (DS_Placa.virmanRow)virmanTable.Rows.Add(stdParams);

      virmanRow.money   = money;

      // 3.10. bobesic rekla da banke tak zahtjevaju
      //virmanRow.pnbz    = Kplan.GetPlacaKonta_New(virmanEnum, ZXC.SaldoOrDugOrPot.POT, placaTT);
      //virmanRow.pnbz = ZXC.CURR_prjkt_rec.Oib;

      // 12.02.2013. bobesic - i na obustavama kao i na netu
      virmanRow.pnbzMod = "67";
      virmanRow.pnbzMod = ZXC.GetPnbModfromOldPnbMod(virmanRow.pnbzMod); //16.01.2013.

      virmanRow.pnbz = ZXC.CURR_prjkt_rec.Oib + "-" + mmyy + "-" + X_pnbz;

      //24.09.2014. da ne ispisuje gornji pnbz da ne skuzi da je placa Ag
      if(this.RptFilter.IsZnpBezPnb)
      {
         virmanRow.pnbzMod =
         virmanRow.pnbz = "";
      }

      //virmanRow.sifraPl = "16";


      virmanRow.virKind = (int)virmanEnum;

      virmanRow.pnboMod = GetPnbM(virmanRow.pnboMod, virmanRow.pnbo); 

      return virmanRow;
   }

   #endregion VIRMANI Methods

   #region RSm, JOPPD & Other Util Methods

   private string SetRSmVrstaObveznika(Prjkt curr_prjkt_rec, Placa placa_rec)
   {
      string vrstaObveznika = "01";

      if(curr_prjkt_rec.IsObrt)                     vrstaObveznika = "02";
      if(curr_prjkt_rec.IsFrgn)                     vrstaObveznika = "12";
      if(placa_rec.VrstaObr == "03")                vrstaObveznika = "17";
      if(placa_rec.TT == Placa.TT_PODUZETPLACA)     vrstaObveznika = "21";

      return vrstaObveznika;
   }

   private string SetJPDVrstaObveznika(Prjkt curr_prjkt_rec, Placa placa_rec)
   {
      string vrstaObveznika = "1";

      if(curr_prjkt_rec.IsFizickaOsoba)           vrstaObveznika = "4";
      if(curr_prjkt_rec.IsObrt)                   vrstaObveznika = "2";
      if(curr_prjkt_rec.IsFrgn)                   vrstaObveznika = "3";
      // vidjeti za potrebe - ostale fizicke osobe i ostali poslovni subjekti

      return vrstaObveznika;
   }

   private void OnFirstLineOfDocumentAction_InitializePlacaRec(uint currParentID, Placa placa_rec, DS_Placa.placaDataTable placaTable, out DS_Placa.placaRow placaRowOfCurrentTranses, DS_Placa.ptraneDataTable ptraneTable, DS_Placa.ptranoDataTable ptranoTable)
   {
      placaRowOfCurrentTranses = placaTable.SingleOrDefault(placa => placa.recID == currParentID);

      PlacaDao.FillFromTypedPlacaDataRow(placa_rec, placaRowOfCurrentTranses);

      #region Fill placa_Rec.Transes2 from ptraneDataTable

      if(ptraneTable.Count > 0)
      {
         var ptranEsOfThisPlaca = ptraneTable.Where(ptranE => ptranE.t_parentID == currParentID);
         Ptrane ptrane_rec;
         foreach(var ptranErow in ptranEsOfThisPlaca)
         {
            ptrane_rec = new Ptrane();
            PtraneDao.FillFromTypedPtraneDataRow(ptrane_rec, ptranErow);
            placa_rec.Transes2.Add(ptrane_rec);
         }
      }

      #endregion Fill placa_Rec.Transes2 from ptraneDataTable

      #region Fill placa_Rec.Transes3 from ptranoDataTable

      if(ptranoTable.Count > 0)
      {
         var ptranosOfThisPlaca = ptranoTable.Where(ptrano => ptrano.t_parentID == currParentID);
         Ptrano ptrano_rec;
         foreach(var ptranOrow in ptranosOfThisPlaca)
         {
            ptrano_rec = new Ptrano();
            PtranoDao.FillFromTypedPtranoDataRow(ptrano_rec, ptranOrow);
            placa_rec.Transes3.Add(ptrano_rec);
         }
      }

      #endregion Fill placa_Rec.Transes3 from ptranoDataTable

   }

   private void OnLastLineOfDocumentAction_SumPlacaResults(uint currParentID, DS_Placa.placaRow placaRowOfCurrentTranses, DS_Placa.IzvjTableDataTable ptransTable)
   {
      // var = EnumerableRowCollection<Vektor.DataLayer.DS_Reports.DS_Placa.IzvjTableRow>  
      var ptransesOfThisPlaca = ptransTable.Where(ptr => ptr.t_parentID == currParentID);

      // ovo ti treba da bi Crystali imali otkuda ispisivati sume za DocumentRecors 
      PlacaDao.FillTypedDataRowSumResults(placaRowOfCurrentTranses, ptransesOfThisPlaca);
   }

   private void SetRSm_B_strana(DS_Placa.rsmBstranaDataTable rsmBstranaTable, DS_Placa.IzvjTableRow ptransRow, DS_Placa.ptraneDataTable ptraneTable, DS_Placa.personDataTable personTable, uint rsmRbr, List<string> ocurredOOlist)
   {
      VvLookUpItem theLui; bool izDisOOnaTeretHZZOa = false;

      DS_Placa.personRow personRowOfCurrentTrans = personTable.SingleOrDefault(person => person.personCD == ptransRow.t_personCD);

      var ptraneRowsOfThisPtrans = ptraneTable.Where(ptranE => ptranE.t_personCD == ptransRow.t_personCD &&
                                                               ptranE.t_parentID == ptransRow.t_parentID);

      DS_Placa.IzvjTableDataTable ptransTable = ds_PlacaReport.IzvjTable;
      DS_Placa.rsmBstranaRow      rsmBstranaRow;

      // Ako ovaj ptrans_rec nema pojava u PtranE-ou, 
      // Dodaj umjetni PtranE row da bi report imao otkuda ispisati T_rsOO, T_rsOD, T_rsDO. 
      if(ptraneTable.Count(ptrane => ptrane.t_personCD == ptransRow.t_personCD          && 
                                     ptrane.t_rsOO     != Ptrane.RSOO_NE_ZBRAJAJ_U_FOND &&
                                     ptrane.t_rsOO     != Ptrane.RSOO_PRIBRAJAJ_NA_FOND).IsZero())
      //if(ptraneRowsOfThisPtrans.Count().IsZero())
      {
         Add_Artificial_rsmBstranaRow(rsmBstranaTable, ptransRow, personRowOfCurrentTrans, rsmRbr);
      }
      else // rolling ptrane 
      {
         foreach(var ptranErow in ptraneRowsOfThisPtrans)
         {
            if(ptranErow.t_rsOO == Ptrane.RSOO_NE_ZBRAJAJ_U_FOND) continue;
            
            if(ptranErow.t_rsOO == Ptrane.RSOO_PRIBRAJAJ_NA_FOND) continue;

            theLui = ZXC.luiListaVrstaRadaEVR.FirstOrDefault(lui => lui.Integer.ToString("00") == ptranErow.t_rsOO);

            izDisOOnaTeretHZZOa = theLui.Flag;

            rsmBstranaRow = Add_rsmBstranaRow(rsmBstranaTable, rsmRbr, ptransRow, personRowOfCurrentTrans, ptranErow.t_rsOO, ptranErow.t_rsOD, ptranErow.t_rsDO, izDisOOnaTeretHZZOa);

            //rsmBstranaRow.b_rbr = ""; // eventualni iduci put rbr ide prazan 

            if(ocurredOOlist.Contains(ptranErow.t_rsOO)) // ovi OO se vec pojavio 
            {
               rsmBstranaRow.b_Bruto      = decimal.Zero.ToStringVv_RSm();
               rsmBstranaRow.b_MioOsn     = decimal.Zero.ToStringVv_RSm();
               rsmBstranaRow.b_Mio1stup   = decimal.Zero.ToStringVv_RSm();
               rsmBstranaRow.b_Mio2stup   = decimal.Zero.ToStringVv_RSm();
               rsmBstranaRow.b_Netto      = decimal.Zero.ToStringVv_RSm();
            }
            else // ovome OO je ovo prva pojava 
            {
               ocurredOOlist.Add(ptranErow.t_rsOO);
            }

         } // foreach(var ptranErow in ptraneRowsOfThisPtrans)
      }
   }

   private void Add_Artificial_rsmBstranaRow(DS_Placa.rsmBstranaDataTable rsmBstranaTable, DS_Placa.IzvjTableRow ptransRow, DS_Placa.personRow personRow, uint rsmRbr)
   {
      string t_rsOO;
      int    t_rsOD, t_rsDO;

      switch(ptransRow.t_tt)
      {
         case Placa.TT_REDOVANRAD  : t_rsOO = "10"; break;

         case Placa.TT_UGOVORODJELU:
         case Placa.TT_NADZORODBOR :
         case Placa.TT_TURSITVIJECE:
         case Placa.TT_IDD_KOLONA_4:
         case Placa.TT_AUTORHONUMJ :
         case Placa.TT_AHSAMOSTUMJ :
         case Placa.TT_AUTORHONOR  : t_rsOO = "21"; break;

         case Placa.TT_PODUZETPLACA: t_rsOO = "16"; break;

         default: ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Za TT [" + ptransRow.t_tt + "] nedefiniran T_rsOO!"); 
            t_rsOO = "??"; 
            break;
      }

      if(t_rsOO == "21")
      {
         t_rsOD = t_rsDO = 00;
      }
      else
      {
         t_rsOD = 01;
         t_rsDO = Placa.GetDateTimeFromMMYYYY(ptransRow.t_mmyyyy, true).Day;
      }

      Add_rsmBstranaRow(rsmBstranaTable, rsmRbr, ptransRow, personRow, t_rsOO, t_rsOD, t_rsDO, false);
   }

   private DS_Placa.rsmBstranaRow Add_rsmBstranaRow(DS_Placa.rsmBstranaDataTable rsmBstranaTable, uint rsmRbr, DS_Placa.IzvjTableRow ptransRow, DS_Placa.personRow personRow, string t_rsOO, int t_rsOD, int t_rsDO, bool izDisOOnaTeretHZZOa)
   {
      DS_Placa.IzvjTableDataTable ptransTable   = ds_PlacaReport.IzvjTable;
      DS_Placa.placaSumDataTable  placaSumTable = ds_PlacaReport.placaSum;
      DS_Placa.rsmBstranaRow      rsmBstranaRow = (DS_Placa.rsmBstranaRow)rsmBstranaTable.Rows.Add();

      bool isThis_NEISPLACENA_Placa_VrObr00 = placaSumTable[0].X_vrstaObr == Placa.VrObr_NeisplacenaPlaca;
      bool isThis_NADOPLACENA_Placa_VrObr77 = placaSumTable[0].X_vrstaObr == Placa.VrObr_NadoplacenaPlaca;

      rsmBstranaRow.b_rbr        = rsmRbr.ToString();
      rsmBstranaRow.b_personCD   = ptransRow.t_personCD;
      rsmBstranaRow.b_ime        = ptransRow.t_ime;
      rsmBstranaRow.b_prezime    = ptransRow.t_prezime;
      rsmBstranaRow.b_oib        = personRow.oib;
      rsmBstranaRow.b_opcRadCD   = ptransRow.t_opcRadCD.NotEmpty() ? ptransRow.t_opcRadCD : ptransRow.t_opcCD;
      rsmBstranaRow.b_rsB        = ptransRow.t_rsB.ToString();
      rsmBstranaRow.b_rsOO       = t_rsOO;
      rsmBstranaRow.b_rsOD       = t_rsOD.ToString("00");
      rsmBstranaRow.b_rsDO       = t_rsDO.ToString("00");

      if(izDisOOnaTeretHZZOa == true) // ovi OO je OOnaTeretHZZOa 
      {
         rsmBstranaRow.b_Bruto      = decimal.Zero.ToStringVv_RSm();
         rsmBstranaRow.b_MioOsn     = decimal.Zero.ToStringVv_RSm();
         rsmBstranaRow.b_Mio1stup   = decimal.Zero.ToStringVv_RSm();
         rsmBstranaRow.b_Mio2stup   = decimal.Zero.ToStringVv_RSm();
         rsmBstranaRow.b_Netto      = ptransTable.Where(ptrans => ptrans.t_personCD == rsmBstranaRow.b_personCD).Sum(ptrans => ptrans.t_netoAdd).ToStringVv_RSm(); // !? 
      }
      else // obicni OO '10', '17', '18', ... 
      {
         rsmBstranaRow.b_Bruto      = ptransTable.Where(ptrans => ptrans.t_personCD == rsmBstranaRow.b_personCD).Sum(ptrans => ptrans.R_TheBruto  ).ToStringVv_RSm();
         rsmBstranaRow.b_MioOsn     = ptransTable.Where(ptrans => ptrans.t_personCD == rsmBstranaRow.b_personCD).Sum(ptrans => ptrans.R_MioOsn    ).ToStringVv_RSm();
         rsmBstranaRow.b_Mio1stup   = ptransTable.Where(ptrans => ptrans.t_personCD == rsmBstranaRow.b_personCD).Sum(ptrans => ptrans.R_Mio1stup  ).ToStringVv_RSm();
         rsmBstranaRow.b_Mio2stup   = ptransTable.Where(ptrans => ptrans.t_personCD == rsmBstranaRow.b_personCD).Sum(ptrans => ptrans.R_Mio2stup  ).ToStringVv_RSm();
         rsmBstranaRow.b_Netto      = ptransTable.Where(ptrans => ptrans.t_personCD == rsmBstranaRow.b_personCD).Sum(ptrans => ptrans.R_NettoWoAdd).ToStringVv_RSm();
      }

      if(isThis_NEISPLACENA_Placa_VrObr00 == true)
      {
         rsmBstranaRow.b_Netto      = decimal.Zero.ToStringVv_RSm();
      }

      if(isThis_NADOPLACENA_Placa_VrObr77 == true)
      {
         rsmBstranaRow.b_Bruto      = decimal.Zero.ToStringVv_RSm();
         rsmBstranaRow.b_MioOsn     = decimal.Zero.ToStringVv_RSm();
         rsmBstranaRow.b_Mio1stup   = decimal.Zero.ToStringVv_RSm();
         rsmBstranaRow.b_Mio2stup   = decimal.Zero.ToStringVv_RSm();
      }

      return rsmBstranaRow;
   }

 // 21.04.2016. vrstaJoppd za ispravak i dopunu
 //private void SetJPD_B_strana(PrulesStruct pR, DS_Placa.jpdBstranaDataTable jpdBstranaTable, DS_Placa.IzvjTableRow ptransRow, DS_Placa.ptraneDataTable ptraneTable, DS_Placa.personDataTable personTable)
   private void SetJPD_B_strana(PrulesStruct pR, DS_Placa.jpdBstranaDataTable jpdBstranaTable, DS_Placa.IzvjTableRow ptransRow, DS_Placa.ptraneDataTable ptraneTable, DS_Placa.personDataTable personTable, string vrstaJoppd, ref bool isKonacniObrPor_dodan)
   {
      VvLookUpItem theLui; bool izDisOOnaTeretHZZOa = false;

      DS_Placa.personRow personRowOfCurrentTrans = personTable.SingleOrDefault(person => person.personCD == ptransRow.t_personCD);

      var ptraneRowsOfThisPtrans = ptraneTable.Where(ptranE => ptranE.t_personCD == ptransRow.t_personCD &&
                                                               ptranE.t_parentID == ptransRow.t_parentID);

      DS_Placa.IzvjTableDataTable ptransTable = ds_PlacaReport.IzvjTable;
    //DS_Placa.jpdBstranaRow      jpdBstranaRow;

      // Ako ovaj ptrans_rec nema pojava u PtranE-ou, 
      // Dodaj umjetni PtranE row da bi report imao otkuda ispisati T_rsOO, T_rsOD, T_rsDO. 

    //if(ptraneRowsOfThisPtrans.Count().IsZero())
      if(ptraneRowsOfThisPtrans.Count(ptrane => ptrane.t_rsOO     != Ptrane.RSOO_NE_ZBRAJAJ_U_FOND  &&
                                                ptrane.t_rsOO     != Ptrane.RSOO_PRIBRAJAJ_NA_FOND).IsZero() )
                                     
      {
       // 21.04.2016. ovisno o vrstiJoppd dodajemo redke i redni broj ako je 1 onda ne gleda rbJop a ako je 2 il 3 uzima samo one koji imaju RbJop
       //Add_Artificial_jpdBstranaRow(pR, jpdBstranaTable, ptransRow, personRowOfCurrentTrans/*, ++jpdRbr*/);
         if(((vrstaJoppd == "2" || vrstaJoppd == "3") && ptransRow.t_zivotno != 0.00M) ||(vrstaJoppd == "1"))
            Add_Artificial_jpdBstranaRow(pR, jpdBstranaTable, ptransRow, personRowOfCurrentTrans/*, ++jpdRbr*/, vrstaJoppd, Convert.ToInt16(ptransRow.t_zivotno));
      }
      else // rolling ptrane 
      {
         int min_rsOD, max_rsDO;

         foreach(var ptranErow in ptraneRowsOfThisPtrans)
         {
            // yapravo ni ovo ne valja jer bi trebao uzeti min od OD i max od DO
          //min_rsOD = ptraneRowsOfThisPtrans.First(p => ((int)p.t_rsOD).NotZero()).t_rsOD;
          //max_rsDO = ptraneRowsOfThisPtrans.Last (p => ((int)p.t_rsDO).NotZero()).t_rsDO;
            try { min_rsOD = ptraneRowsOfThisPtrans.Where(p => ((int)p.t_rsOD).NotZero()).Min(p => p.t_rsOD); } catch { min_rsOD = 0; }
            try { max_rsDO = ptraneRowsOfThisPtrans.Where(p => ((int)p.t_rsDO).NotZero()).Max(p => p.t_rsDO); } catch { max_rsDO = 0; }

            if(min_rsOD.IsZero()) min_rsOD = 1;
            if(max_rsDO.IsZero()) max_rsDO = DateTime.DaysInMonth(Placa.Get_zaYYYYint(ptransRow.t_mmyyyy), Placa.Get_zaMMint(ptransRow.t_mmyyyy)); 

            if(ptranErow.t_rsOO == Ptrane.RSOO_NE_ZBRAJAJ_U_FOND && ptranErow.t_vrstaR_cd != Ptrane.VrstaR_cd_thisIs_ZPI) continue;  // po novom ZPI mora ici u poseban redak a svi ostali "99" sluze samo za obracun place

            if(ptranErow.t_rsOO == Ptrane.RSOO_PRIBRAJAJ_NA_FOND) continue;

            theLui = ZXC.luiListaVrstaRadaEVR.FirstOrDefault(lui => lui.Integer.ToString("00") == ptranErow.t_rsOO);

            izDisOOnaTeretHZZOa = theLui.Flag;

          //jpdBstranaRow = Add_jpdBstranaRow(jpdBstranaTable, jpdRbr, ptransRow, personRowOfCurrentTrans, ptranErow.t_rsOO, ptranErow.t_rsOD, ptranErow.t_rsDO, izDisOOnaTeretHZZOa);
          // 21.04.2016. vrstajoppd za ispravak i dounu  ako je 1 onda ne gleda rbJop a ako je 2 il 3 uzima samo one koji imaju RbJop                              
          //jpdBstranaRow = Add_jpdBstranaRow(pR, jpdBstranaTable, ptransRow, personRowOfCurrentTrans, ptranErow.t_vrstaR_cd, ptranErow.t_rsOO, ptranErow.t_rsOD, ptranErow.t_rsDO, min_rsOD, max_rsDO, izDisOOnaTeretHZZOa, ptranErow.t_sati, ptranErow.t_stjecatCD, ptranErow.t_primDohCD, ptranErow.t_pocKrajCD);
            if(((vrstaJoppd == "2" || vrstaJoppd == "3") && ptranErow.t_rbrIsprJop != 0.00M) || (vrstaJoppd == "1"))
            { 
               Add_jpdBstranaRow(pR, jpdBstranaTable, ptransRow, personRowOfCurrentTrans, ptranErow.t_vrstaR_cd, ptranErow.t_rsOO, ptranErow.t_rsOD, ptranErow.t_rsDO, min_rsOD, max_rsDO, izDisOOnaTeretHZZOa, ptranErow.t_sati, ptranErow.t_stjecatCD, ptranErow.t_primDohCD, ptranErow.t_pocKrajCD, vrstaJoppd, ptranErow.t_rbrIsprJop);
            }

         } // foreach(var ptranErow in ptraneRowsOfThisPtrans)

         //08.03.2019. ???? - ovdje bi islo na kraju kad se izvrti ptrane po nekom ptransRow-u -+ staviti datume od i do tog mjeseca a negdje i ubaciti 
         // a trebalo bi nekako oznaciti jer je tt=RR pa da znamo kasnije dolje ispuniti redak joppda
         //if(nesto nije empty()) dodajemo redak NP sa specificnostima, mjesec od do, 63 i sl 
         //Add_jpdBstranaRow(pR, jpdBstranaTable, ptransRow, personRowOfCurrentTrans, "", "00", 1, 28, 1, 28, false, 0, "0000", "0000", "0", vrstaJoppd, 0);

      }

      // 12.03.2019: novi ekstra redak za NP63 
      if(ZXC.projectYearAsInt >= 2019 && ptransRow.t_dopZdr.NotZero())
      {
       //10.03.2020. bespotrebno ponavljamo code
       //Add_NP63_jpdBstranaRow(jpdBstranaTable, ptransRow, personRowOfCurrentTrans, vrstaJoppd, Convert.ToInt16(ptransRow.t_zivotno));
       //26.03.2020.
       //Add_NeoporeziviUzRR_jpdBstranaRow(jpdBstranaTable, ptransRow, personRowOfCurrentTrans, vrstaJoppd, Convert.ToInt16(ptransRow.t_zivotno), "63"                    );
         Add_NeoporeziviUzRR_jpdBstranaRow(jpdBstranaTable, ptransRow, personRowOfCurrentTrans, vrstaJoppd, Convert.ToInt16(ptransRow.t_zivotno), "63", ptransRow.t_dopZdr);
      }

      // 06.09.2019: novi ekstra redak za npTobrok "65" 
      if(ZXC.projectYearAsInt >= 2019 && ptransRow.t_dobMIO.NotZero())
      {
       //10.03.2020. bespotrebno ponavljamo code
       //Add_npTobrok_jpdBstranaRow(jpdBstranaTable, ptransRow, personRowOfCurrentTrans, vrstaJoppd, Convert.ToInt16(ptransRow.t_zivotno));
       // 26.03.2020.
       //Add_NeoporeziviUzRR_jpdBstranaRow(jpdBstranaTable, ptransRow, personRowOfCurrentTrans, vrstaJoppd, Convert.ToInt16(ptransRow.t_zivotno), "65"                    );
         Add_NeoporeziviUzRR_jpdBstranaRow(jpdBstranaTable, ptransRow, personRowOfCurrentTrans, vrstaJoppd, Convert.ToInt16(ptransRow.t_zivotno), "65", ptransRow.t_dobMIO);
      }

      // 03.03.2020: novi ekstra redak za dopZdr2020 "71"
      if(ZXC.projectYearAsInt >= 2020 && ptransRow.t_dopZdr2020.NotZero())
      {
       //10.03.2020. bespotrebno ponavljamo code
       //Add_dopZdr2020_jpdBstranaRow(jpdBstranaTable, ptransRow, personRowOfCurrentTrans, vrstaJoppd, Convert.ToInt16(ptransRow.t_zivotno));
       // 26.03.2020.
       //Add_NeoporeziviUzRR_jpdBstranaRow(jpdBstranaTable, ptransRow, personRowOfCurrentTrans, vrstaJoppd, Convert.ToInt16(ptransRow.t_zivotno), "71"                        );
         Add_NeoporeziviUzRR_jpdBstranaRow(jpdBstranaTable, ptransRow, personRowOfCurrentTrans, vrstaJoppd, Convert.ToInt16(ptransRow.t_zivotno), "71", ptransRow.t_dopZdr2020);
      }

      // 29.01.2024: novi ekstra redak za NP73 "73"
      if(ZXC.projectYearAsInt >= 2024 && ptransRow.t_NP73.NotZero())
      {
         Add_NeoporeziviUzRR_jpdBstranaRow(jpdBstranaTable, ptransRow, personRowOfCurrentTrans, vrstaJoppd, Convert.ToInt16(ptransRow.t_NP73), "73", ptransRow.t_NP73);
      }

      // 23.12.2020: dodaj Konacni Obracun SAMO JEDNOM! 
    //if(/*goodMjesec*/ ptransRow.t_dokDate.Month == 12 && rptFilter.HocuKonacniObrPor == true)
      if(/*goodMjesec*/ ptransRow.t_dokDate.Month == 12 && rptFilter.HocuKonacniObrPor == true && isKonacniObrPor_dodan == false)
      {
         // 1. alfa13 = dosadasnja suma 12 mjeseci
         // 2. beta14 = suma 'konacni obracun' (sta bi bilo da je bilo, kako je trebalo biti) 
         // 3. gama15 = beta14 - alfa13: razlika po konacnom obracunu 
         // 4. sumiraj gama15 u ovaj redak 12-og mjeseca 

         SetKonacniObracun_Alfa13Beta14Gama15_PtransRecs(ptransRow.t_personCD, pR, personRowOfCurrentTrans);

         if(PtransSumRec_alfa13 != null)
         {
            DS_Placa.IzvjTableRow ptransRow_gama15 = (DS_Placa.IzvjTableRow)ptransTable.Rows.Add();

            PtransDao.FillTypedDataRowResults(ptransRow_gama15, PtransSumRec_gama15, /*ptraneRowsOfThisPerson*/null, /*placa_rec*/ null);

            //30.11.2020: otkriven BUG da je t_mmyyyy prazan pri konacnom obracun
            ptransRow_gama15.t_mmyyyy = ptransRow.t_mmyyyy;

          //if(PtransSumRec_gama15.R_NettoAftKrp.Ron2().NotZero())
          //if(PtransSumRec_gama15.R_NettoAftKrp.Ron2() > rptFilter.GoObrPiPLimit) 
            if(ZXC.AlmostEqual(PtransSumRec_gama15.R_Netto.Ron2(), 0M, rptFilter.TolerancijaPoKO) == false)
            { 
             // 21.04.2016.
             //Add_jpdBstranaRow(pR, jpdBstranaTable, /*jpdRbr,*/ ptransRow_gama15, personRowOfCurrentTrans, "", "XY", 1, 30, 1, 30, false, 0, "0001", "0406", "3");
               Add_jpdBstranaRow(pR, jpdBstranaTable, /*jpdRbr,*/ ptransRow_gama15, personRowOfCurrentTrans, "", "XY", 1, 30, 1, 30, false, 0, "0001", "0406", "3", vrstaJoppd, 0);

               isKonacniObrPor_dodan = true;
            }

            ptransTable.Rows.Remove(ptransRow_gama15);
         }
      } // if(/*goodMjesec*/ ptransRow.t_dokDate.Month == 12 && rptFilter.HocuKonacniObrPor == true)
   }

  // 26.03.2020bug ... uzimao je samo jedan izno
 //private DS_Placa.jpdBstranaRow Add_NeoporeziviUzRR_jpdBstranaRow(DS_Placa.jpdBstranaDataTable jpdBstranaTable, DS_Placa.IzvjTableRow ptransRow, DS_Placa.personRow personRow, string vrstaJoppd, int rbrJop, string oznakaNP)
   private DS_Placa.jpdBstranaRow Add_NeoporeziviUzRR_jpdBstranaRow(DS_Placa.jpdBstranaDataTable jpdBstranaTable, DS_Placa.IzvjTableRow ptransRow, DS_Placa.personRow personRow, string vrstaJoppd, int rbrJop, string oznakaNP, decimal iznosNP)
   {
      DS_Placa.IzvjTableDataTable ptransTable   = ds_PlacaReport.IzvjTable;
      DS_Placa.placaSumDataTable  placaSumTable = ds_PlacaReport.placaSum;
      DS_Placa.jpdBstranaRow      jpdBstranaRow = (DS_Placa.jpdBstranaRow)jpdBstranaTable.Rows.Add();

      jpdBstranaRow.b_rbr        = vrstaJoppd == "1" ? ++jpdRbr : rbrJop;

      jpdBstranaRow.b_tt         = ptransRow.t_tt;
      jpdBstranaRow.b_personCD   = ptransRow.t_personCD;
      jpdBstranaRow.b_ime        = ptransRow.t_ime + " " + ptransRow.t_prezime;
      jpdBstranaRow.b_prezime    = ptransRow.t_prezime;
      jpdBstranaRow.b_oib        = personRow.oib;
      
      // 01.04.2014. stranci pocinju sa 99 a kod nas nema sifre opcine koja pocinje sa 9 pa se to zasada tako radi
      string prefix              = ptransRow.t_opcCD.StartsWith("9") ? "9" : "0";
      jpdBstranaRow.b_opcCD      = ptransRow.t_opcCD.NotEmpty()    ? prefix + ptransRow.t_opcCD    : "00000";

      //14.04.2023. i za opcRadCD treba poseban prefix jer onaj opcCD može biti 0 a ovaj 9
      string prefix2           = ptransRow.t_opcRadCD.StartsWith("9") ? "9" : "0";
      jpdBstranaRow.b_opcRadCD = ptransRow.t_opcRadCD.NotEmpty() ? prefix2 + ptransRow.t_opcRadCD : "00000";

      //09.05.2024. kod NP ne treba taj beneficirani staz
    //jpdBstranaRow.b_rsB        = ptransRow.t_rsB.ToString();
      jpdBstranaRow.b_rsB        = "0";
      
      jpdBstranaRow.b_rsOO       = "0";
      jpdBstranaRow.b_satiNeRad  = 0M;

    //10.03.2020. umjesto placaSumTable[0].X_zaMMYYYY treba ptransRow.t_mmyyyy kako bi placa koja je za 2 mjesec mogla ici na isti joppd sa neoporzivim primicima za npr za 3 mjesec
    //jpdBstranaRow.b_rsOD      = Placa.GetDateTimeFromMMYYYY(placaSumTable[0].X_zaMMYYYY, false).ToString("s").Substring(0, 10); // na bazi punog mjeseca
    //jpdBstranaRow.b_rsDO      = Placa.GetDateTimeFromMMYYYY(placaSumTable[0].X_zaMMYYYY, true ).ToString("s").Substring(0, 10); // na bazi punog mjeseca
    //jpdBstranaRow.b_rsODcr    = Placa.GetDateTimeFromMMYYYY(placaSumTable[0].X_zaMMYYYY, false).ToString(ZXC.VvDateFormat);
    //jpdBstranaRow.b_rsDOcr    = Placa.GetDateTimeFromMMYYYY(placaSumTable[0].X_zaMMYYYY, true ).ToString(ZXC.VvDateFormat);
      jpdBstranaRow.b_rsOD      = Placa.GetDateTimeFromMMYYYY(ptransRow.t_mmyyyy, false).ToString("s").Substring(0, 10); // na bazi punog mjeseca
      jpdBstranaRow.b_rsDO      = Placa.GetDateTimeFromMMYYYY(ptransRow.t_mmyyyy, true ).ToString("s").Substring(0, 10); // na bazi punog mjeseca
      jpdBstranaRow.b_rsODcr    = Placa.GetDateTimeFromMMYYYY(ptransRow.t_mmyyyy, false).ToString(ZXC.VvDateFormat);
      jpdBstranaRow.b_rsDOcr    = Placa.GetDateTimeFromMMYYYY(ptransRow.t_mmyyyy, true ).ToString(ZXC.VvDateFormat);

    //20.01.2023. Danijel i jos neki - da za neoporezive primitke mjesec ide sa datumom U mjesecu,  kada su na RR dok placa ide ZA mjesec
      KtoShemaPlacaDsc ksPl = new KtoShemaPlacaDsc(ZXC.dscLuiLst_KtoShemaPlaca);
      if(ksPl.Dsc_IsNPnaRR_U_mj)
      { 
         jpdBstranaRow.b_rsOD      = Placa.GetDateTimeFromMMYYYY(placaSumTable[0].X_uMMYYYY, false).ToString("s").Substring(0, 10); // na bazi punog mjeseca
         jpdBstranaRow.b_rsDO      = Placa.GetDateTimeFromMMYYYY(placaSumTable[0].X_uMMYYYY, true ).ToString("s").Substring(0, 10); // na bazi punog mjeseca
         jpdBstranaRow.b_rsODcr    = Placa.GetDateTimeFromMMYYYY(placaSumTable[0].X_uMMYYYY, false).ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_rsDOcr    = Placa.GetDateTimeFromMMYYYY(placaSumTable[0].X_uMMYYYY, true ).ToString(ZXC.VvDateFormat);
      }
      
      jpdBstranaRow.b_pocKrajCD = "0";
      jpdBstranaRow.b_sati      = 0;
      jpdBstranaRow.b_neoPrimCD = oznakaNP;
      jpdBstranaRow.b_posInval  = "0"; 
      jpdBstranaRow.b_radVr     = "0"; 
      jpdBstranaRow.b_nacIsplCD = "1";
      jpdBstranaRow.b_MioOsn    = 0.00M;
      jpdBstranaRow.b_NetoAdd   = iznosNP/*ptransRow.t_dopZdr*/;
      jpdBstranaRow.b_Netto     = iznosNP/*ptransRow.t_dopZdr*/;
      jpdBstranaRow.b_ObrPrimPl = 0.00M;
      jpdBstranaRow.b_satiNeRad = 0;
      jpdBstranaRow.b_stjecatCD = "0000";
      jpdBstranaRow.b_primDohCD = "0000";

      jpdBstranaRow.b_Bruto      = 
      jpdBstranaRow.b_Mio1stup   = 
      jpdBstranaRow.b_Mio2stup   = 
      jpdBstranaRow.b_ZdrNa      = 
      jpdBstranaRow.b_ZorNa      = 
      jpdBstranaRow.b_ZapNa      = 
      jpdBstranaRow.b_Mio1stupNa = 
      jpdBstranaRow.b_Mio2stupNa = 
      jpdBstranaRow.b_ZpiUk      =  
      jpdBstranaRow.b_ZapII      = 
      jpdBstranaRow.b_AHizdatak  = 
      jpdBstranaRow.b_MioAll     = 
      jpdBstranaRow.b_Dohodak    = 
      jpdBstranaRow.b_Odbitak    = 
      jpdBstranaRow.b_PorOsnAll  = 
      jpdBstranaRow.b_PorezAll   = 
      jpdBstranaRow.b_PorPrir    = 
      jpdBstranaRow.b_Prirez     = 0.00M;


      return jpdBstranaRow;
   }

 // 21.04.2016. vrstaJoppd ya ispravak i dopunu
 //private void Add_Artificial_jpdBstranaRow(PrulesStruct pR, DS_Placa.jpdBstranaDataTable jpdBstranaTable, DS_Placa.IzvjTableRow ptransRow, DS_Placa.personRow personRow/*, uint jpdRbr*/)
   private void Add_Artificial_jpdBstranaRow(PrulesStruct pR, DS_Placa.jpdBstranaDataTable jpdBstranaTable, DS_Placa.IzvjTableRow ptransRow, DS_Placa.personRow personRow/*, uint jpdRbr*/, string vrstaJoppd, int rbrJop)
   {
      // umjetno dodani 
      string    t_rsOO;
      int       t_dayOD, t_dayDO;
      decimal   t_sati;
      string    t_stjecatCD;
      string    t_primDohCD;
      string    t_pocKrajCD;
      bool      isPenzic   = (ptransRow.t_spc == (byte)Ptrans.SpecEnum.PENZ      );
    //bool      isBezStaza = (ptransRow.t_spc == (byte)Ptrans.SpecEnum.NOVOZAPOSL                                                          );
      bool      isBezStaza = (ptransRow.t_spc == (byte)Ptrans.SpecEnum.NOVOZAPOSL || ptransRow.t_spc == (byte)Ptrans.SpecEnum.NOVO_MINMIONE); //18.12.2019.
      bool      isClanUprv = (ptransRow.t_spc == (byte)Ptrans.SpecEnum.CLANUPRAVE); //30.01.2017.
      bool      isBef2017  = (ptransRow.t_dokDate < ZXC.Date01012017);
      bool      isIzaslrad = (ptransRow.t_spc == (byte)Ptrans.SpecEnum.IZASLANRADNIK); //14.04.2023.

      switch(ptransRow.t_tt)
      {
         case Placa.TT_REDOVANRAD  : t_rsOO = "10"; // ovo ostavljam jer je jednoznacan i oznacava mi RR
            t_stjecatCD = isIzaslrad ? "0005" : "0001"; 
            t_primDohCD = isClanUprv ? "0005" : "0001"; 
            t_pocKrajCD = "3"; 
            t_sati      = ptransRow.R_SatiUk;  
            break;

         case Placa.TT_PODUZETPLACA: t_rsOO = "16";
            t_stjecatCD = "0032"; t_primDohCD = "0101"; t_pocKrajCD = "3"; t_sati = ptransRow.R_SatiUk;  
            break;

         case Placa.TT_UGOVORODJELU: t_rsOO = "21";
          //if(isPenzic) { t_stjecatCD = "4001"; t_primDohCD = "4032"; t_pocKrajCD = "0"; t_sati = 0; }  od 01.01.2017. i penzici placaju doprinose
                 if(isPenzic && isBef2017) { t_stjecatCD = "4001"; t_primDohCD = "4032"; t_pocKrajCD = "0"; t_sati = 0; } 
            else if(isPenzic             ) { t_stjecatCD = "4002"; t_primDohCD = "4030"; t_pocKrajCD = "0"; t_sati = 0; } 
            else                           { t_stjecatCD = "4002"; t_primDohCD = "4010"; t_pocKrajCD = "0"; t_sati = 0; } 
            break;

         case Placa.TT_NADZORODBOR :
         case Placa.TT_TURSITVIJECE: 
            t_rsOO = "21";
          //if(isPenzic) { t_stjecatCD = "4001"; t_primDohCD = "4016"; t_pocKrajCD = "0"; t_sati = 0;} od 01.01.2017. i penzici placaju doprinose
            if(isPenzic && isBef2017) { t_stjecatCD = "4001"; t_primDohCD = "4016"; t_pocKrajCD = "0"; t_sati = 0; } 
            else                      { t_stjecatCD = "4002"; t_primDohCD = "4014"; t_pocKrajCD = "0"; t_sati = 0;} 
            break;

         case Placa.TT_IDD_KOLONA_4: t_rsOO = "21";
           t_stjecatCD = "4001"; t_primDohCD = "xxxx"; t_pocKrajCD = "0"; t_sati = 0;
           break;

         case Placa.TT_AUTORHONOR  : t_rsOO = "21";
          //t_stjecatCD =             "4001";          t_primDohCD = "4001"; t_pocKrajCD = "0"; t_sati = 0; od 01.01.2017. i honoraci placaju doprinose
            t_stjecatCD = isBef2017 ? "4001" : "4002"; t_primDohCD = "4001"; t_pocKrajCD = "0"; t_sati = 0;
            break;
         
         case Placa.TT_AUTORHONUMJ : t_rsOO = "21";
          //t_stjecatCD =              "4001";          t_primDohCD = "4002"; t_pocKrajCD = "0"; t_sati = 0;  od 01.01.2017. i honoraci placaju doprinose
            t_stjecatCD =  isBef2017 ? "4001" : "4002"; t_primDohCD = "4002"; t_pocKrajCD = "0"; t_sati = 0; 
            break;

         case Placa.TT_AHSAMOSTUMJ : t_rsOO = "21"; // novo od 01.01.2017.
            t_stjecatCD =  "4001"; t_primDohCD = "4002"; t_pocKrajCD = "0"; t_sati = 0; 
            break;

         case Placa.TT_NEOPOREZPRIM : t_rsOO = "00";
            t_stjecatCD = "0000"; t_primDohCD = "0000"; t_pocKrajCD = "0"; t_sati = 0; 
            break;

         case Placa.TT_PLACAUNARAVI: t_rsOO = "17";
            t_stjecatCD = "0001"; t_primDohCD = "0021"; t_pocKrajCD = "3"; t_sati = 0;
            break;
        
         case Placa.TT_SEZZAPPOLJOP: t_rsOO = "21";
            t_stjecatCD = "4001"; t_primDohCD = "4033"; t_pocKrajCD = "0"; t_sati = 0;
            break;

         case Placa.TT_POREZNADOBIT: t_rsOO = "21";
            t_stjecatCD = "1001"; t_primDohCD = "1001"; t_pocKrajCD = "0"; t_sati = 0;
            break;

         case Placa.TT_STRUCNOOSPOS: t_rsOO = "00";
          //t_stjecatCD = "5702"; t_primDohCD = isBezStaza ? "5703":"5701" ; t_pocKrajCD = "3"; t_sati = ptransRow.R_SatiUk; 24.01.2017. SO ide na DANE umjesto sate
            t_stjecatCD = "5702"; t_primDohCD = isBezStaza ? "5703":"5701" ; t_pocKrajCD = "3"; t_sati = isBef2017 ? ptransRow.R_SatiUk : ptransRow.R_daniR;
            break;

         case Placa.TT_NEPLACDOPUST: t_rsOO = "00";
          //t_stjecatCD = "0001"; t_primDohCD = "0041"; t_pocKrajCD = "4"; t_sati = ptransRow.R_SatiUk; 25.08.2020 mjenja se sifra 41 u 45
            t_stjecatCD = "0001"; t_primDohCD = "0045"; t_pocKrajCD = "4"; t_sati = ptransRow.R_SatiUk;
            break;

         case Placa.TT_SAMODOPRINOS: t_rsOO = "10"; // ovo ostavljam jer je jednoznacan i oznacava mi RR
            t_stjecatCD = "0041"; t_primDohCD = "5801"; t_pocKrajCD = "3"; t_sati = ptransRow.R_daniR;
            break;

         case Placa.TT_DDBEZDOPRINO:  t_rsOO = "21";
            t_stjecatCD = "4001"; t_primDohCD = "4002"; t_pocKrajCD = "0"; t_sati = 0;
            break;
         case Placa.TT_AUVECASTOPA:   t_rsOO = "21";
            t_stjecatCD = "4002"; t_primDohCD = "4002"; t_pocKrajCD = "0"; t_sati = 0;
            break;
         case Placa.TT_NR2_P01NEDOP: t_rsOO = "21";
            t_stjecatCD = "4001"; t_primDohCD = "4006"; t_pocKrajCD = "0"; t_sati = 0;
            break;
         case Placa.TT_NR3_PX1DADOP: t_rsOO = "21";
            t_stjecatCD = "4002"; t_primDohCD = "4004"; t_pocKrajCD = "0"; t_sati = 0;
            break;
         case Placa.TT_NR1_PX1NEDOP:  t_rsOO = "21";
            t_stjecatCD = "4001"; t_primDohCD = "4006"; t_pocKrajCD = "0"; t_sati = 0;
            break;
         case Placa.TT_BIVSIRADNIK: //23.12.2019.
            t_rsOO = "10";
            t_stjecatCD = "0201"; t_primDohCD = "0402"; t_pocKrajCD = "0"; t_sati = 0;
            break;
         case Placa.TT_OSTALIPRIM: //01.01.2024.
            t_rsOO = "10";
            t_stjecatCD = "0001"; t_primDohCD = "0021"; t_pocKrajCD = "3"; t_sati = 0;
            break;


         default: ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Za TT [" + ptransRow.t_tt + "] nedefiniran T_rsOO!"); 
            t_rsOO = "??"; 
            t_stjecatCD = "0000"; t_primDohCD = "0000"; t_pocKrajCD = "0";  t_sati = 0;  
            break;
      }
      t_dayOD = 1; t_dayDO = Placa.GetDateTimeFromMMYYYY(ptransRow.t_mmyyyy, true).Day;

    // 21.04.2016. vrtsJoppd za ispravak i dopunu
    //Add_jpdBstranaRow(pR, jpdBstranaTable, /*jpdRbr,*/ ptransRow, personRow, "", t_rsOO, t_dayOD, t_dayDO, t_dayOD, t_dayDO, false, t_sati, t_stjecatCD, t_primDohCD, t_pocKrajCD);
      Add_jpdBstranaRow(pR, jpdBstranaTable, /*jpdRbr,*/ ptransRow, personRow, "", t_rsOO, t_dayOD, t_dayDO, t_dayOD, t_dayDO, false, t_sati, t_stjecatCD, t_primDohCD, t_pocKrajCD, vrstaJoppd, rbrJop);
   }

   // 21.04.2016. vrstaJoppd za ispravak i dopunu
 //private DS_Placa.jpdBstranaRow Add_jpdBstranaRow(PrulesStruct pR, DS_Placa.jpdBstranaDataTable jpdBstranaTable, /*uint jpdRbr,*/ DS_Placa.IzvjTableRow ptransRow, DS_Placa.personRow personRow, string t_vrstaR_cd, string t_rsOO, int t_rsOD, int t_rsDO, int min_rsOD, int max_rsDO, bool izDisOOnaTeretHZZOa, decimal t_sati, string t_stjecatCD, string t_primDohCD, string t_pocKrajCD)
   private DS_Placa.jpdBstranaRow Add_jpdBstranaRow(PrulesStruct pR, DS_Placa.jpdBstranaDataTable jpdBstranaTable, /*uint jpdRbr,*/ DS_Placa.IzvjTableRow ptransRow, DS_Placa.personRow personRow, string t_vrstaR_cd, string t_rsOO, int t_rsOD, int t_rsDO, int min_rsOD, int max_rsDO, bool izDisOOnaTeretHZZOa, decimal t_sati, string t_stjecatCD, string t_primDohCD, string t_pocKrajCD, string vrstaJoppd, int rbrJop)
   {
      DS_Placa.IzvjTableDataTable ptransTable   = ds_PlacaReport.IzvjTable;
      DS_Placa.placaSumDataTable  placaSumTable = ds_PlacaReport.placaSum;
      DS_Placa.jpdBstranaRow      jpdBstranaRow = (DS_Placa.jpdBstranaRow)jpdBstranaTable.Rows.Add();

      bool isThis_NEISPLACENA_Placa_VrObr00 = placaSumTable[0].X_vrstaObr == Placa.VrObr_NeisplacenaPlaca;
      bool isThis_NADOPLACENA_Placa_VrObr77 = placaSumTable[0].X_vrstaObr == Placa.VrObr_NadoplacenaPlaca;
      bool isDrugiDohodak = (ptransRow.t_tt == Placa.TT_IDD_KOLONA_4 || ptransRow.t_tt == Placa.TT_AUTORHONOR   || 
                             ptransRow.t_tt == Placa.TT_SEZZAPPOLJOP || ptransRow.t_tt == Placa.TT_AUTORHONUMJ  || 
                             ptransRow.t_tt == Placa.TT_NADZORODBOR  || ptransRow.t_tt == Placa.TT_UGOVORODJELU ||
                             ptransRow.t_tt == Placa.TT_TURSITVIJECE || ptransRow.t_tt == Placa.TT_AHSAMOSTUMJ  ||
                             ptransRow.t_tt == Placa.TT_DDBEZDOPRINO || ptransRow.t_tt == Placa.TT_AUVECASTOPA  ||
                             ptransRow.t_tt == Placa.TT_NR2_P01NEDOP || ptransRow.t_tt == Placa.TT_NR3_PX1DADOP ||
                             ptransRow.t_tt == Placa.TT_NR1_PX1NEDOP || ptransRow.t_tt == Placa.TT_BIVSIRADNIK
                            );

      bool isBef2017  = ptransRow.t_dokDate < ZXC.Date01012017;
      bool isBef2024  = ptransRow.t_dokDate < ZXC.Date01012024;

    //10.03.2020. umjesto placaSumTable[0].X_zaMMYYYY treba ptransRow.t_mmyyyy kako bi placa koja je za 2 mjesec mogla ici na isti joppd sa neoporzivim primicima za npr za 3 mjesec
    //int mm   = ZXC.ValOrZero_Int(placaSumTable[0].X_zaMM  );
    //int yyyy = ZXC.ValOrZero_Int(placaSumTable[0].X_zaYYYY);
      int mm   = Placa.Get_zaMMint  (ptransRow.t_mmyyyy);
      int yyyy = Placa.Get_zaYYYYint(ptransRow.t_mmyyyy);

      DateTime dateOD;
      DateTime dateDO;

      try
      {
         dateOD = new DateTime(yyyy, mm, t_rsOD);
         dateDO = new DateTime(yyyy, mm, t_rsDO);
      }
      catch (Exception ex)
      {
         ZXC.aim_emsg("djelatnik {0} ime nekorektne OD ili DO vrijednosti na EVR", ptransRow.t_prezime );
         ZXC.aim_emsg_VvException(ex);
         dateOD = dateDO = DateTime.MinValue;
      }
      //21.04.2016. ako je 2 ili 3 vrstaJoppd onda ide broj rbrJop a inace ++jpdRbr
      //jpdBstranaRow.b_rbr        = ++jpdRbr; 
      jpdBstranaRow.b_rbr        = vrstaJoppd == "1" ? ++jpdRbr : rbrJop;


      jpdBstranaRow.b_tt         = ptransRow.t_tt;
      jpdBstranaRow.b_personCD   = ptransRow.t_personCD;
      jpdBstranaRow.b_ime        = ptransRow.t_ime + " " + ptransRow.t_prezime;
      jpdBstranaRow.b_prezime    = ptransRow.t_prezime;
      jpdBstranaRow.b_oib        = personRow.oib;
      
      // 01.04.2014. stranci pocinju sa 99 a kod nas nema sifre opcine koja pocinje sa 9 pa se to zasada tako radi
      string prefix              = ptransRow.t_opcCD.StartsWith("9") ? "9" : "0";
      jpdBstranaRow.b_opcCD      = ptransRow.t_opcCD.NotEmpty()    ? prefix + ptransRow.t_opcCD    : "00000";
      
     //14.04.2023. i za opcRadCD treba poseban prefix jer onaj opcCD može biti 0 a ovaj 9
      string prefix2             = ptransRow.t_opcRadCD.StartsWith("9") ? "9" : "0";
      jpdBstranaRow.b_opcRadCD   = ptransRow.t_opcRadCD.NotEmpty() ? prefix2 + ptransRow.t_opcRadCD : "00000";
      
      jpdBstranaRow.b_rsB        = ptransRow.t_rsB.ToString();
      jpdBstranaRow.b_rsOO       = t_rsOO;
      jpdBstranaRow.b_satiNeRad  = ptransRow.R_SatiNeR;

      // za novosti u placi od za 122023
      DateTime zaMMYY_asDateTime = Placa.GetDateTimeFromMMYYYY(ptransRow.t_mmyyyy, false);
      bool is_posInval_DO_1123 = zaMMYY_asDateTime < ZXC.Date01122023;
      bool is_posInval_OD_1223 = !is_posInval_DO_1123;

      #region if(isDrugiDohodak)

      if(isDrugiDohodak)
      {
         jpdBstranaRow.b_rsOD      = ZXC.projectYearFirstDay.ToString("s").Substring(0, 10); // na bazi godine
         jpdBstranaRow.b_rsDO      = ZXC.projectYearLastDay .ToString("s").Substring(0, 10); // na bazi godine
         jpdBstranaRow.b_rsODcr    = ZXC.projectYearFirstDay.ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_rsDOcr    = ZXC.projectYearLastDay .ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_pocKrajCD = "0";
         jpdBstranaRow.b_sati      =  0;
         jpdBstranaRow.b_neoPrimCD = "0";
         jpdBstranaRow.b_posInval  = "0"; 
         jpdBstranaRow.b_radVr     = "0";

       //                                           jpdBstranaRow.b_nacIsplCD = ptransRow.t_nacIsplCD.IsEmpty() ? "2" : ptransRow.t_nacIsplCD;
         if(ptransRow.t_tt == Placa.TT_BIVSIRADNIK) jpdBstranaRow.b_nacIsplCD = ptransRow.t_nacIsplCD.IsEmpty() ? "1" : ptransRow.t_nacIsplCD; //23.12.2019.
         else                                       jpdBstranaRow.b_nacIsplCD = ptransRow.t_nacIsplCD.IsEmpty() ? "2" : ptransRow.t_nacIsplCD;


       //jpdBstranaRow.b_MioOsn    = ptransRow.R_MioOsn; // samo napomena da je osnovicaDop = R_MioOsn = R_TheBruto za drugi dohodak
       // e pa nije od 01.01.2017. jer i AH i AU sda imaju osnovicu za doprinose
         jpdBstranaRow.b_MioOsn    = ptransRow.R_MioOsn; // samo napomena da je osnovicaDop = R_MioOsn = R_TheBruto za drugi dohodak
         jpdBstranaRow.b_NetoAdd   = 0.00M;
         jpdBstranaRow.b_Netto     = ptransRow.R_Netto;
         jpdBstranaRow.b_ObrPrimPl = 0.00M;
         jpdBstranaRow.b_opcRadCD  = "00000";
         jpdBstranaRow.b_satiNeRad = 0;
      }

      #endregion if(isDrugiDohodak)

      #region else if(ptransRow.t_tt == Placa.TT_NEOPOREZPRIM)
     
      else if(ptransRow.t_tt == Placa.TT_NEOPOREZPRIM)
      {
       //10.03.2020. umjesto placaSumTable[0].X_zaMMYYYY treba ptransRow.t_mmyyyy kako bi placa koja je za 2 mjesec mogla ici na isti joppd sa neoporzivim primicima za npr za 3 mjesec
       //jpdBstranaRow.b_rsOD      = Placa.GetDateTimeFromMMYYYY(placaSumTable[0].X_zaMMYYYY, false).ToString("s").Substring(0, 10); // na bazi punog mjeseca
       //jpdBstranaRow.b_rsDO      = Placa.GetDateTimeFromMMYYYY(placaSumTable[0].X_zaMMYYYY, true ).ToString("s").Substring(0, 10); // na bazi punog mjeseca
       //jpdBstranaRow.b_rsODcr    = Placa.GetDateTimeFromMMYYYY(placaSumTable[0].X_zaMMYYYY, false).ToString(ZXC.VvDateFormat);
       //jpdBstranaRow.b_rsDOcr    = Placa.GetDateTimeFromMMYYYY(placaSumTable[0].X_zaMMYYYY, true ).ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_rsOD      = Placa.GetDateTimeFromMMYYYY(ptransRow.t_mmyyyy, false).ToString("s").Substring(0, 10); // na bazi punog mjeseca
         jpdBstranaRow.b_rsDO      = Placa.GetDateTimeFromMMYYYY(ptransRow.t_mmyyyy, true ).ToString("s").Substring(0, 10); // na bazi punog mjeseca
         jpdBstranaRow.b_rsODcr    = Placa.GetDateTimeFromMMYYYY(ptransRow.t_mmyyyy, false).ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_rsDOcr    = Placa.GetDateTimeFromMMYYYY(ptransRow.t_mmyyyy, true ).ToString(ZXC.VvDateFormat);

         jpdBstranaRow.b_pocKrajCD = "0";
         jpdBstranaRow.b_sati      = 0;
         jpdBstranaRow.b_neoPrimCD = ptransRow.t_neoPrimCD.IsEmpty() ? "0" : ptransRow.t_neoPrimCD;
         jpdBstranaRow.b_posInval  = "0"; 
         jpdBstranaRow.b_radVr     = "0"; 
         jpdBstranaRow.b_nacIsplCD = ptransRow.t_nacIsplCD.IsEmpty() ? "1" : ptransRow.t_nacIsplCD;
         jpdBstranaRow.b_MioOsn    = 0.00M;
         jpdBstranaRow.b_NetoAdd   = ptransRow.t_netoAdd;
         jpdBstranaRow.b_Netto     = ptransRow.t_netoAdd;
         jpdBstranaRow.b_ObrPrimPl = 0.00M;
         jpdBstranaRow.b_satiNeRad = 0;

         //06.04.2023. kada je neoporeyiv primitak i nema isplate (0) tada u koloni 16.2. treba biti 0.00, ta se kolona nikuda ni ne zbaraja
         if(ptransRow.t_nacIsplCD == "0") jpdBstranaRow.b_Netto = 0.00M;
      }

      #endregion else if(ptransRow.t_tt == Placa.TT_NEOPOREZPRIM)

      #region else if(ptransRow.t_tt == Placa.TT_PLACAUNARAVI) 

      else if(ptransRow.t_tt == Placa.TT_PLACAUNARAVI) // jos provjeri kaj sve mora biti !!!
      {
         jpdBstranaRow.b_rsOD      = ZXC.projectYearFirstDay.ToString("s").Substring(0, 10); // na bazi godine
         jpdBstranaRow.b_rsDO      = ZXC.projectYearLastDay .ToString("s").Substring(0, 10); // na bazi godine
         jpdBstranaRow.b_rsODcr    = ZXC.projectYearFirstDay.ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_rsDOcr    = ZXC.projectYearLastDay .ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_pocKrajCD = t_pocKrajCD.IsEmpty() ? "3" : t_pocKrajCD;
         jpdBstranaRow.b_sati      = 0;
         jpdBstranaRow.b_neoPrimCD = "0";
         jpdBstranaRow.b_posInval  = "0";
        
         //14.01.2015.
         //jpdBstranaRow.b_radVr     = ptransRow.t_isPoluSat == 0      ? "1" : "2"; // jos treba doraditi
         jpdBstranaRow.b_radVr = ((ptransRow.t_dnFondSati == 0 || ptransRow.t_dnFondSati == 8) && ptransRow.t_isPoluSat == 0) ? "1" : "2"; 


         jpdBstranaRow.b_nacIsplCD = ptransRow.t_nacIsplCD.IsEmpty() ? "5" : ptransRow.t_nacIsplCD; ;
         jpdBstranaRow.b_MioOsn    = ptransRow.R_TheBruto;  // i ovdje je osnovicaDop = R_MioOsn = R_TheBruto
         jpdBstranaRow.b_NetoAdd   = 0.00M;
         jpdBstranaRow.b_Netto     = ptransRow.R_Netto;
         jpdBstranaRow.b_ObrPrimPl = 0.00M;
         jpdBstranaRow.b_satiNeRad = 0;
      }

      #endregion else if(ptransRow.t_tt == Placa.TT_PLACAUNARAVI) // jos provjeri kaj sve mora biti !!!

      #region else if(ptransRow.t_tt == Placa.TT_POREZNADOBIT) // 07.03.2014

      else if(ptransRow.t_tt == Placa.TT_POREZNADOBIT) // 07.03.2014
      {
         jpdBstranaRow.b_rsOD      = ZXC.projectYearFirstDay.ToString("s").Substring(0, 10); // na bazi godine
         jpdBstranaRow.b_rsDO      = ZXC.projectYearLastDay .ToString("s").Substring(0, 10); // na bazi godine
         jpdBstranaRow.b_rsODcr    = ZXC.projectYearFirstDay.ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_rsDOcr    = ZXC.projectYearLastDay .ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_pocKrajCD = "0";
         jpdBstranaRow.b_sati      =  0;
         jpdBstranaRow.b_neoPrimCD = "0";
         jpdBstranaRow.b_posInval  = "0";
         jpdBstranaRow.b_radVr     = "0"; 
         jpdBstranaRow.b_nacIsplCD = ptransRow.t_nacIsplCD.IsEmpty() ? "1" : ptransRow.t_nacIsplCD;
         jpdBstranaRow.b_MioOsn    = 0.00M;
         jpdBstranaRow.b_NetoAdd   = 0.00M;
         jpdBstranaRow.b_Netto     = ptransRow.R_Netto;
         jpdBstranaRow.b_ObrPrimPl = 0.00M;
         jpdBstranaRow.b_opcRadCD  = "00000";
         jpdBstranaRow.b_satiNeRad = 0;
      }

      #endregion else if(ptransRow.t_tt == Placa.TT_POREZNADOBIT) // 07.03.2014

      #region else // redovna placa i poduzetnicka placa

      else // redovna placa i poduzetnicka placa
      {
         DateTime minDateOD = new DateTime(yyyy, mm, min_rsOD);
         DateTime maxDateDO = new DateTime(yyyy, mm, max_rsDO);

         jpdBstranaRow.b_rsOD      = minDateOD                                                      .ToString("s").Substring(0, 10); // na bazi punog mjeseca TTTODO za one koji su poceli/prestali raditi unutar jednog mjeseca!!!!!
         jpdBstranaRow.b_rsDO      = maxDateDO                                                      .ToString("s").Substring(0, 10); // na bazi punog mjeseca TTTODO za one koji su poceli/prestali raditi unutar jednog mjeseca!!!!!
         jpdBstranaRow.b_rsODcr    = minDateOD                                                      .ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_rsDOcr    = maxDateDO                                                      .ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_pocKrajCD = t_pocKrajCD.IsEmpty() ? "3" : t_pocKrajCD;
       //02.11.2020.
       //jpdBstranaRow.b_sati      = t_sati;
         jpdBstranaRow.b_sati      = ptransRow.R_SatiR;
         jpdBstranaRow.b_neoPrimCD = "0";

         //17.01.2014. kod poduyetnika ni nema takvog poreza
         //if(ptransRow.t_tt == Placa.TT_PODUZETPLACA) jpdBstranaRow.b_posInval = "0"; 
         //else                                        jpdBstranaRow.b_posInval = ZXC.CURR_prjkt_rec.IsOver20 ? "1" : "0"; // doraditi sko  ce biti potrebe
         // 04.02.2014. od place 012014 poduzetnici placaju zaposljavanje




#if kakojeKodIzgledaoDoKraja2023
         //04.12.2014
      // jpdBstranaRow.b_posInval = ZXC.CURR_prjkt_rec.IsOver20                                 ? "1" : "0"; // doraditi sko  ce biti potrebe
         // XY je konacni godisnji obracun
         jpdBstranaRow.b_posInval = ZXC.CURR_prjkt_rec.IsOver20 && jpdBstranaRow.b_rsOO != "XY" ? "1" : "0"; // doraditi sko  ce biti potrebe
#endif
         //DateTime zaMMYY_asDateTime = Placa.GetDateTimeFromMMYYYY(ptransRow.t_mmyyyy, false);
         //bool is_posInval_DO_1123 = zaMMYY_asDateTime < ZXC.Date01122023;
         //bool is_posInval_OD_1223 = !is_posInval_DO_1123;

         if(is_posInval_DO_1123)
         {
            jpdBstranaRow.b_posInval = ZXC.CURR_prjkt_rec.IsOver20 && jpdBstranaRow.b_rsOO != "XY" ? "1" : "0"; // doraditi sko  ce biti potrebe
         }
         if(is_posInval_OD_1223)
         {
            jpdBstranaRow.b_posInval = ptransRow.R_Mio1OlkKind.ToString();
         }


         // 14.01.2015.
         //jpdBstranaRow.b_radVr     = ptransRow.t_isPoluSat == 0      ? "1" : "2"; // jos treba doraditi
         jpdBstranaRow.b_radVr = ((ptransRow.t_dnFondSati == 0 || ptransRow.t_dnFondSati == 8) && ptransRow.t_isPoluSat == 0) ? "1" : "2"; 
         
         jpdBstranaRow.b_nacIsplCD = ptransRow.t_nacIsplCD.IsEmpty() ? "1" : ptransRow.t_nacIsplCD; ;
//       jpdBstranaRow.b_MioOsn    = ptransRow.R_TheBruto;  15.10.2014. kada je osnovica min ili maks onda ne dolazi dobro, ne zanm kak to do sada nitko nije primjetio
//       jpdBstranaRow.b_MioOsn    = ptransRow.R_MioOsn;    13.05.2015. ovdje treba doci osnovica doprionosa NA placu a ne mio koja je kod velikih placa bruto a kod malih mioOsn
         jpdBstranaRow.b_MioOsn    = ptransRow.R_osnovicaDop;
         jpdBstranaRow.b_NetoAdd   = 0.00M;
         jpdBstranaRow.b_Netto     = ptransRow.R_Netto;
         jpdBstranaRow.b_ObrPrimPl = ptransRow.R_TheBruto;
      }

      #endregion else // redovna placa i poduzetnicka placa

      if(ptransRow.t_tt == Placa.TT_REDOVANRAD)
      {
         jpdBstranaRow.b_stjecatCD = t_stjecatCD.IsEmpty() ? "0001" : t_stjecatCD;
         jpdBstranaRow.b_primDohCD = t_primDohCD.IsEmpty() ? "0001" : t_primDohCD;
      }
      else
      {
         jpdBstranaRow.b_stjecatCD = t_stjecatCD.IsEmpty() ? "0000" : t_stjecatCD;
         jpdBstranaRow.b_primDohCD = t_primDohCD.IsEmpty() ? "0000" : t_primDohCD;           
      }
      
      jpdBstranaRow.b_Bruto      = ptransRow.R_TheBruto  ;
      jpdBstranaRow.b_Mio1stup   = ptransRow.R_Mio1stup  ;
      jpdBstranaRow.b_Mio2stup   = ptransRow.R_Mio2stup  ;
      jpdBstranaRow.b_ZdrNa      = ptransRow.R_ZdrNa     ;
      jpdBstranaRow.b_ZorNa      = ptransRow.R_ZorNa     ;
      jpdBstranaRow.b_ZapNa      = ptransRow.R_ZapNa     ;
      jpdBstranaRow.b_Mio1stupNa = ptransRow.R_Mio1stupNa;
      jpdBstranaRow.b_Mio2stupNa = ptransRow.R_Mio2stupNa;
      jpdBstranaRow.b_ZpiUk      = 0.00M                 ;  // poseban redak kad postoji zpi a inace je 0 

   
    //                         jpdBstranaRow.b_ZapII      = ptransRow.R_ZapII     ; od 2024 ovdje idu olaksice
      if(is_posInval_DO_1123) {jpdBstranaRow.b_ZapII = ptransRow.R_ZapII;  }
      if(is_posInval_OD_1223) {jpdBstranaRow.b_ZapII = ptransRow.R_Mio1Olk;}
     
      jpdBstranaRow.b_AHizdatak  = ptransRow.R_AHizdatak ;
      jpdBstranaRow.b_MioAll     = ptransRow.R_MioAll    ;
      jpdBstranaRow.b_Dohodak    = ptransRow.R_Dohodak   ;
      jpdBstranaRow.b_Odbitak    = ptransRow.R_Odbitak   ;
      jpdBstranaRow.b_PorOsnAll  = ptransRow.R_PorOsnAll ;
      jpdBstranaRow.b_PorezAll   = ptransRow.R_PorezAll  ;

      // od 2024
    //jpdBstranaRow.b_PorPrir    =             ptransRow.R_PorPrirez                       ;
    //jpdBstranaRow.b_Prirez     =             ptransRow.R_Prirez                          ;
      jpdBstranaRow.b_PorPrir    = isBef2024 ? ptransRow.R_PorPrirez : ptransRow.R_PorezAll;
      jpdBstranaRow.b_Prirez     = isBef2024 ? ptransRow.R_Prirez    : 0.00M               ;



    //if(ptransRow.t_tt == Placa.TT_AUTORHONOR || ptransRow.t_tt == Placa.TT_AUTORHONUMJ                                          )
    //if(ptransRow.t_tt == Placa.TT_AUTORHONOR || ptransRow.t_tt == Placa.TT_AUTORHONUMJ || ptransRow.t_tt == Placa.TT_AHSAMOSTUMJ)
    //12.2018.
      if(ptransRow.t_tt == Placa.TT_AUTORHONOR   || 
         ptransRow.t_tt == Placa.TT_AUTORHONUMJ  || 
         ptransRow.t_tt == Placa.TT_AHSAMOSTUMJ  ||
         ptransRow.t_tt == Placa.TT_DDBEZDOPRINO ||
         ptransRow.t_tt == Placa.TT_AUVECASTOPA  ||
         ptransRow.t_tt == Placa.TT_NR1_PX1NEDOP ||
         ptransRow.t_tt == Placa.TT_NR2_P01NEDOP ||
         ptransRow.t_tt == Placa.TT_NR3_PX1DADOP
         )
      {
         jpdBstranaRow.b_Dohodak = ptransRow.R_PorOsnAll;
      }

      #region TT_PODUZETPLACA od 2016

      if(ptransRow.t_tt == Placa.TT_PODUZETPLACA && yyyy > 2015) // dodano 08.02.2016.
      {
         jpdBstranaRow.b_sati      = ptransRow.R_daniR + ptransRow.R_daniB;
         jpdBstranaRow.b_satiNeRad = ptransRow.R_daniB;
         jpdBstranaRow.b_ObrPrimPl = 0.00M;

      }
      #endregion TT_PODUZETPLACA  od 2016

      #region TT_SAMODOPRINOS

      if(ptransRow.t_tt == Placa.TT_SAMODOPRINOS) // dodano 08.02.2016.
      {
         DateTime minDateOD = new DateTime(yyyy, mm, min_rsOD);
         DateTime maxDateDO = new DateTime(yyyy, mm, max_rsDO);

         jpdBstranaRow.b_rsOD      = minDateOD.ToString("s").Substring(0, 10); // na bazi punog mjeseca TTTODO za one koji su poceli/prestali raditi unutar jednog mjeseca!!!!!
         jpdBstranaRow.b_rsDO      = maxDateDO.ToString("s").Substring(0, 10); // na bazi punog mjeseca TTTODO za one koji su poceli/prestali raditi unutar jednog mjeseca!!!!!
         jpdBstranaRow.b_rsODcr    = minDateOD.ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_rsDOcr    = maxDateDO.ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_pocKrajCD = t_pocKrajCD.IsEmpty() ? "3" : t_pocKrajCD;
         jpdBstranaRow.b_sati      = ptransRow.R_daniR + ptransRow.R_daniB;
         jpdBstranaRow.b_satiNeRad = ptransRow.R_daniB;
         jpdBstranaRow.b_neoPrimCD = "0";
         jpdBstranaRow.b_posInval  = "0"; 
         
         //14.01.2015.
         //jpdBstranaRow.b_radVr     = ptransRow.t_isPoluSat == 0      ? "1" : "2"; // jos treba doraditi
         jpdBstranaRow.b_radVr = ((ptransRow.t_dnFondSati == 0 || ptransRow.t_dnFondSati == 8) && ptransRow.t_isPoluSat == 0) ? "1" : "3"; 

         
         jpdBstranaRow.b_nacIsplCD = ptransRow.t_nacIsplCD.IsEmpty() ? "0" : ptransRow.t_nacIsplCD; ;

         jpdBstranaRow.b_Bruto      = 0.00M;
         jpdBstranaRow.b_AHizdatak  = 0.00M;
         jpdBstranaRow.b_MioAll     = 0.00M;
         jpdBstranaRow.b_Dohodak    = 0.00M;
         jpdBstranaRow.b_Odbitak    = 0.00M;
         jpdBstranaRow.b_PorOsnAll  = 0.00M;
         jpdBstranaRow.b_PorezAll   = 0.00M;
         jpdBstranaRow.b_PorPrir    = 0.00M;
         jpdBstranaRow.b_Prirez     = 0.00M;
         jpdBstranaRow.b_ObrPrimPl  = 0.00M;
         jpdBstranaRow.b_Netto      = 0.00M;

      }

      #endregion TT_SAMODOPRINOS

      #region OO je OOnaTeretHZZOa
      
      if(izDisOOnaTeretHZZOa == true) // ovi OO je OOnaTeretHZZOa 
      {
         jpdBstranaRow.b_rsOD      = dateOD.ToString("s").Substring(0, 10); // na bazi stvarnog razdoblja
         jpdBstranaRow.b_rsDO      = dateDO.ToString("s").Substring(0, 10); // na bazi stvarnog razdoblja
         jpdBstranaRow.b_rsODcr    = dateOD.ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_rsDOcr    = dateDO.ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_neoPrimCD = ptransRow.t_neoPrimCD.IsEmpty() ? "0" : ptransRow.t_neoPrimCD;
         jpdBstranaRow.b_radVr     = "0"; // 08.01.2014.
         jpdBstranaRow.b_posInval  = "0"; // 10.01.2014.
         jpdBstranaRow.b_opcRadCD  = "00000";

         jpdBstranaRow.b_Bruto      = 0.00M;
         jpdBstranaRow.b_MioOsn     = 0.00M;
         jpdBstranaRow.b_Mio1stup   = 0.00M;
         jpdBstranaRow.b_Mio2stup   = 0.00M;
         jpdBstranaRow.b_ZdrNa      = 0.00M;
         jpdBstranaRow.b_ZorNa      = 0.00M;
         jpdBstranaRow.b_ZapNa      = 0.00M;
         jpdBstranaRow.b_Mio1stupNa = 0.00M;
         jpdBstranaRow.b_Mio2stupNa = 0.00M;
         jpdBstranaRow.b_ZapII      = 0.00M;
         jpdBstranaRow.b_AHizdatak  = 0.00M;
         jpdBstranaRow.b_MioAll     = 0.00M;
         jpdBstranaRow.b_Dohodak    = 0.00M;
         jpdBstranaRow.b_Odbitak    = 0.00M;
         jpdBstranaRow.b_PorOsnAll  = 0.00M;
         jpdBstranaRow.b_PorezAll   = 0.00M;
         jpdBstranaRow.b_PorPrir    = 0.00M;
         jpdBstranaRow.b_Prirez     = 0.00M;
         jpdBstranaRow.b_NetoAdd    = 
         jpdBstranaRow.b_Netto      = ptransRow.t_netoAdd; // ptransTable.Where(ptrans => ptrans.t_personCD == jpdBstranaRow.b_personCD).Sum(ptrans => ptrans.t_netoAdd); // !? 
         jpdBstranaRow.b_ObrPrimPl  = 0.00M;

         //02.11.2020.
         jpdBstranaRow.b_sati = t_sati;

         jpdBstranaRow.b_satiNeRad  = jpdBstranaRow.b_sati;

         if(ptransRow.t_tt == Placa.TT_SAMODOPRINOS || (ptransRow.t_tt == Placa.TT_PODUZETPLACA && yyyy > 2015 )) // dodano 08.02.2016.
         {
            jpdBstranaRow.b_sati      = ptransRow.R_daniB;
            jpdBstranaRow.b_satiNeRad = ptransRow.R_daniB;
            jpdBstranaRow.b_Netto     = 0.00M;
         }

      }

      #endregion OO je OOnaTeretHZZOa

      #region RR + prijevoz

      if(ptransRow.t_tt == Placa.TT_REDOVANRAD && ptransRow.t_prijevoz.NotZero() && (t_rsOO == "10" || t_rsOO == "18"/*i oni na pola rad vremena 09.02.2015.*/)) // kada se prijevoz isplacuje zajedno sa placom
      { 
         jpdBstranaRow.b_NetoAdd   = ptransRow.t_prijevoz;
         jpdBstranaRow.b_Netto     = ptransRow.R_Netto + ptransRow.t_prijevoz ;
       //jpdBstranaRow.b_neoPrimCD = ptransRow.t_neoPrimCD.IsEmpty() ? "19" : ptransRow.t_neoPrimCD;
       //12.02.2016. ako ima prijevoz i naknadu za bolovanje na teret HZZO a koja je za sada 12 !!!! pazi ako se promjeni !!!
         jpdBstranaRow.b_neoPrimCD = (ptransRow.t_neoPrimCD.IsEmpty() || ptransRow.t_neoPrimCD == "12") ? "19" : ptransRow.t_neoPrimCD;
      }

      #endregion RR + prijevoz

      #region TT_STRUCNOOSPOS

      if(ptransRow.t_tt == Placa.TT_STRUCNOOSPOS)
      {
         DateTime minDateOD = new DateTime(yyyy, mm, min_rsOD);
         DateTime maxDateDO = new DateTime(yyyy, mm, max_rsDO);

         jpdBstranaRow.b_rsOD      = minDateOD.ToString("s").Substring(0, 10); // na bazi punog mjeseca TTTODO za one koji su poceli/prestali raditi unutar jednog mjeseca!!!!!
         jpdBstranaRow.b_rsDO      = maxDateDO.ToString("s").Substring(0, 10); // na bazi punog mjeseca TTTODO za one koji su poceli/prestali raditi unutar jednog mjeseca!!!!!
         jpdBstranaRow.b_rsODcr    = minDateOD.ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_rsDOcr    = maxDateDO.ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_pocKrajCD = t_pocKrajCD.IsEmpty() ? "3" : t_pocKrajCD;
       //jpdBstranaRow.b_sati      = t_sati; 24.01.2017. SO vise nema sate vec dane!!!!
         jpdBstranaRow.b_sati      = isBef2017 ?  t_sati : ptransRow.R_daniR + ptransRow.R_daniB;
         jpdBstranaRow.b_satiNeRad = ptransRow.R_daniB;

         jpdBstranaRow.b_neoPrimCD = "0";
         jpdBstranaRow.b_posInval  = "0"; 
         
         //14.01.2015.
         //jpdBstranaRow.b_radVr     = ptransRow.t_isPoluSat == 0      ? "1" : "2"; // jos treba doraditi
         jpdBstranaRow.b_radVr = ((ptransRow.t_dnFondSati == 0 || ptransRow.t_dnFondSati == 8) && ptransRow.t_isPoluSat == 0) ? "1" : "2"; 

         
         jpdBstranaRow.b_nacIsplCD = ptransRow.t_nacIsplCD.IsEmpty() ? "0" : ptransRow.t_nacIsplCD; ;
      
         jpdBstranaRow.b_Bruto      = 0.00M;
         jpdBstranaRow.b_ZapNa      = 0.00M;
         jpdBstranaRow.b_Mio1stupNa = 0.00M;
         jpdBstranaRow.b_Mio2stupNa = 0.00M;
         jpdBstranaRow.b_ZapII      = 0.00M;
         jpdBstranaRow.b_AHizdatak  = 0.00M;
         jpdBstranaRow.b_MioAll     = 0.00M;
         jpdBstranaRow.b_Dohodak    = 0.00M;
         jpdBstranaRow.b_Odbitak    = 0.00M;
         jpdBstranaRow.b_PorOsnAll  = 0.00M;
         jpdBstranaRow.b_PorezAll   = 0.00M;
         jpdBstranaRow.b_PorPrir    = 0.00M;
         jpdBstranaRow.b_Prirez     = 0.00M;
         jpdBstranaRow.b_ObrPrimPl  = 0.00M;
         
         if(ptransRow.t_prijevoz.NotZero()) // kada se prijevoz isplacuje za strucno osposobljavanje
         {
            jpdBstranaRow.b_NetoAdd   = ptransRow.t_prijevoz;
            jpdBstranaRow.b_Netto     = ptransRow.t_prijevoz;
            jpdBstranaRow.b_neoPrimCD = ptransRow.t_neoPrimCD.IsEmpty() ? "19" : ptransRow.t_neoPrimCD;
         }
      }
      #endregion TT_STRUCNOOSPOS

      #region TT_NEPLACDOPUST

      if(ptransRow.t_tt == Placa.TT_NEPLACDOPUST)
      {
         DateTime minDateOD = new DateTime(yyyy, mm, min_rsOD);
         DateTime maxDateDO = new DateTime(yyyy, mm, max_rsDO);

         jpdBstranaRow.b_rsOD      = minDateOD.ToString("s").Substring(0, 10); // na bazi punog mjeseca TTTODO za one koji su poceli/prestali raditi unutar jednog mjeseca!!!!!
         jpdBstranaRow.b_rsDO      = maxDateDO.ToString("s").Substring(0, 10); // na bazi punog mjeseca TTTODO za one koji su poceli/prestali raditi unutar jednog mjeseca!!!!!
         jpdBstranaRow.b_rsODcr    = minDateOD.ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_rsDOcr    = maxDateDO.ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_pocKrajCD = t_pocKrajCD.IsEmpty() ? "4" : t_pocKrajCD;
         jpdBstranaRow.b_sati      = t_sati;
         jpdBstranaRow.b_neoPrimCD = "0";
         jpdBstranaRow.b_posInval  = "0"; 
         
         //14.01.2015.
         //jpdBstranaRow.b_radVr     = ptransRow.t_isPoluSat == 0      ? "1" : "2"; // jos treba doraditi
         jpdBstranaRow.b_radVr = ((ptransRow.t_dnFondSati == 0 || ptransRow.t_dnFondSati == 8) && ptransRow.t_isPoluSat == 0) ? "1" : "2"; 

         jpdBstranaRow.b_nacIsplCD = ptransRow.t_nacIsplCD.IsEmpty() ? "0" : ptransRow.t_nacIsplCD; ;
         jpdBstranaRow.b_stjecatCD = t_stjecatCD.IsEmpty() ? "0001" : t_stjecatCD;
         jpdBstranaRow.b_primDohCD = t_primDohCD.IsEmpty() ? "0045" : t_primDohCD;           

         jpdBstranaRow.b_Bruto      = 0.00M;
//       jpdBstranaRow.b_ZapNa      = 0.00M;
         jpdBstranaRow.b_Mio1stupNa = 0.00M;
         jpdBstranaRow.b_Mio2stupNa = 0.00M;
//       jpdBstranaRow.b_ZapII      = 0.00M;
         jpdBstranaRow.b_AHizdatak  = 0.00M;
         jpdBstranaRow.b_MioAll     = 0.00M;
         jpdBstranaRow.b_Dohodak    = 0.00M;
         jpdBstranaRow.b_Odbitak    = 0.00M;
         jpdBstranaRow.b_PorOsnAll  = 0.00M;
         jpdBstranaRow.b_PorezAll   = 0.00M;
         jpdBstranaRow.b_PorPrir    = 0.00M;
         jpdBstranaRow.b_Prirez     = 0.00M;
         jpdBstranaRow.b_ObrPrimPl  = 0.00M;
         jpdBstranaRow.b_satiNeRad  = 0;
      }

      #endregion TT_NEPLACDOPUST

      #region ZPI

      if(t_vrstaR_cd == Ptrane.VrstaR_cd_thisIs_ZPI)
      {
         decimal mothHasDays_4ZPI = DateTime.DaysInMonth(yyyy, mm);
         decimal ZPI_osnovaPerDay = (pR._minMioOsn / mothHasDays_4ZPI);
         decimal daniZpi          = t_rsDO - t_rsOD + 1;
         decimal stopaZpi         = pR._stZpi/100;

         decimal r_ZpiUk  = (daniZpi * ZPI_osnovaPerDay * stopaZpi).Ron2();
         decimal r_ZpiOsn = (daniZpi * ZPI_osnovaPerDay           ).Ron2();

         jpdBstranaRow.b_rsOD       = dateOD.ToString("s").Substring(0, 10); // na bazi stvarnog razdoblja
         jpdBstranaRow.b_rsDO       = dateDO.ToString("s").Substring(0, 10); // na bazi stvarnog razdoblja
         jpdBstranaRow.b_rsODcr     = dateOD.ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_rsDOcr     = dateDO.ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_radVr      = "0"; // 08.01.2014.

         jpdBstranaRow.b_ZpiUk      = r_ZpiUk;
         jpdBstranaRow.b_MioOsn     = r_ZpiOsn;
         jpdBstranaRow.b_opcRadCD   = "00000";
         jpdBstranaRow.b_nacIsplCD  = "0";

         jpdBstranaRow.b_stjecatCD  = t_stjecatCD.IsEmpty() ? "0001" : t_stjecatCD;
         jpdBstranaRow.b_primDohCD  = t_primDohCD.IsEmpty() ? "5002" : t_primDohCD;           
         jpdBstranaRow.b_pocKrajCD  = t_pocKrajCD.IsEmpty() ? "4"    : t_pocKrajCD;           
         jpdBstranaRow.b_sati       = 0;
         jpdBstranaRow.b_Bruto      = 
         jpdBstranaRow.b_Mio1stup   = 
         jpdBstranaRow.b_Mio2stup   = 
         jpdBstranaRow.b_ZdrNa      = 
         jpdBstranaRow.b_ZorNa      = 
         jpdBstranaRow.b_ZapNa      = 
         jpdBstranaRow.b_Mio1stupNa = 
         jpdBstranaRow.b_Mio2stupNa = 
         jpdBstranaRow.b_ZapII      = 
         jpdBstranaRow.b_AHizdatak  = 
         jpdBstranaRow.b_MioAll     = 
         jpdBstranaRow.b_Dohodak    = 
         jpdBstranaRow.b_Odbitak    = 
         jpdBstranaRow.b_PorOsnAll  = 
         jpdBstranaRow.b_PorezAll   = 
         jpdBstranaRow.b_PorPrir    = 
         jpdBstranaRow.b_Prirez     = 
         jpdBstranaRow.b_NetoAdd    = 
         jpdBstranaRow.b_Netto      = 
         jpdBstranaRow.b_ObrPrimPl  = 0.00M;
         jpdBstranaRow.b_satiNeRad  = 0;
      }

      #endregion ZPI

      #region NEISPLATA PLACE

      if(isThis_NEISPLACENA_Placa_VrObr00 == true)
      {
         jpdBstranaRow.b_stjecatCD = t_stjecatCD.IsEmpty() ? "0001" : t_stjecatCD;
         jpdBstranaRow.b_primDohCD = /*t_primDohCD.IsEmpty() ?*/"0041" /*: t_primDohCD*/;

         jpdBstranaRow.b_AHizdatak  =             0.00M                        ;
         jpdBstranaRow.b_Bruto      = isBef2017 ? 0.00M : ptransRow.R_TheBruto ; 
         jpdBstranaRow.b_MioAll     = isBef2017 ? 0.00M : ptransRow.R_MioAll   ;
         jpdBstranaRow.b_Dohodak    = isBef2017 ? 0.00M : ptransRow.R_Dohodak  ;
         jpdBstranaRow.b_Odbitak    = isBef2017 ? 0.00M : ptransRow.R_Odbitak  ;
         jpdBstranaRow.b_PorOsnAll  = isBef2017 ? 0.00M : ptransRow.R_PorOsnAll;
         jpdBstranaRow.b_PorezAll   = isBef2017 ? 0.00M : ptransRow.R_PorezAll ;

         //2024
       //jpdBstranaRow.b_PorPrir    = isBef2017 ? 0.00M :             ptransRow.R_PorPrirez                       ;
       //jpdBstranaRow.b_Prirez     = isBef2017 ? 0.00M :             ptransRow.R_Prirez                          ;
         jpdBstranaRow.b_PorPrir    = isBef2017 ? 0.00M : isBef2024 ? ptransRow.R_PorPrirez : ptransRow.R_PorezAll;
         jpdBstranaRow.b_Prirez     = isBef2017 ? 0.00M : isBef2024 ? ptransRow.R_Prirez    : 0.00M               ;
         jpdBstranaRow.b_Netto      =             0.00M                        ;
         
         if(ptransRow.t_tt == Placa.TT_PODUZETPLACA)
         {
            jpdBstranaRow.b_ObrPrimPl = 0.00M; // dodano 30.01.2014.
            jpdBstranaRow.b_primDohCD = "0103";
         }
         jpdBstranaRow.b_nacIsplCD = "0";
      }

      #endregion NEISPLATA PLACE

      #region ISPLATA ZAOSTALE PLACE  - vec priijavljeni doprinosi

      if(isThis_NADOPLACENA_Placa_VrObr77 == true)
      {
         jpdBstranaRow.b_stjecatCD = t_stjecatCD.IsEmpty() ? "0001" : t_stjecatCD;
         jpdBstranaRow.b_primDohCD = /*t_primDohCD.IsEmpty() ?*/ "0051" /*: t_primDohCD*/;

         jpdBstranaRow.b_sati      = 0;
         jpdBstranaRow.b_satiNeRad = 0;

         //jpdBstranaRow.b_Bruto      =  11.03.2014. ovo je bilo krivo
         jpdBstranaRow.b_MioOsn     =
         jpdBstranaRow.b_Mio1stup   = 
         jpdBstranaRow.b_Mio2stup   = 
         jpdBstranaRow.b_ZdrNa      = 
         jpdBstranaRow.b_ZorNa      = 
         jpdBstranaRow.b_ZapNa      = 
         jpdBstranaRow.b_Mio1stupNa = 
         jpdBstranaRow.b_Mio2stupNa = 
         jpdBstranaRow.b_ZapII      = 
         jpdBstranaRow.b_ObrPrimPl  = 0.00M;

         if(isBef2017 == false)
         {
          jpdBstranaRow.b_Odbitak    =  0.00M ;
          jpdBstranaRow.b_PorOsnAll  =  0.00M ;
          jpdBstranaRow.b_PorezAll   =  0.00M ;
          jpdBstranaRow.b_PorPrir    =  0.00M ;
          jpdBstranaRow.b_Prirez     =  0.00M ;
          jpdBstranaRow.b_neoPrimCD  =    "59";
          jpdBstranaRow.b_NetoAdd    = ptransRow.R_PorPrirez;

         }

      }

      #endregion ISPLATA ZAOSTALE PLACE  - vec priijavljeni doprinosi

      #region Ostali primici koji se isplaćuju uz plaću-oporezivi NA BAZI GODINE  6.2.=0021 JUBILARNE NAGRADE OTPREMNINE 0025

      // 09.07.2014. za HZTK isplata jubilarne nagrade iznad nepoporezivog
      // i 30.12.2014. za ZarPtica isplata naknade kad je radnik na dugom bolovanju a zele mu isplatiti razliku
      // 12.03.2015. i otpremnine iznad neoporezivog 0025
      // 01.01.2024. je za ovo napravljen novi TT - OP - OSTALI PRIMICI

    //if(              ptransRow.t_tt == Placa.TT_REDOVANRAD && jpdBstranaRow.b_primDohCD == "0021" || ptransRow.t_tt == Placa.TT_REDOVANRAD && jpdBstranaRow.b_primDohCD == "0025")
      if(isBef2024 && (ptransRow.t_tt == Placa.TT_REDOVANRAD && jpdBstranaRow.b_primDohCD == "0021" || ptransRow.t_tt == Placa.TT_REDOVANRAD && jpdBstranaRow.b_primDohCD == "0025"))
      { 
         jpdBstranaRow.b_rsOD      = ZXC.projectYearFirstDay.ToString("s").Substring(0, 10); // na bazi godine
         jpdBstranaRow.b_rsDO      = ZXC.projectYearLastDay .ToString("s").Substring(0, 10); // na bazi godine
         jpdBstranaRow.b_rsODcr    = ZXC.projectYearFirstDay.ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_rsDOcr    = ZXC.projectYearLastDay .ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_sati      = 0;
         jpdBstranaRow.b_ObrPrimPl = 0.00M;
         jpdBstranaRow.b_satiNeRad = 0;
      }

      // u 2024 uvodimo novi TT za te ostale primitke jer se MIO1 racuna drugacie od place
      if(!isBef2024  && ptransRow.t_tt == Placa.TT_OSTALIPRIM)
      { 
         jpdBstranaRow.b_rsOD      = ZXC.projectYearFirstDay.ToString("s").Substring(0, 10); // na bazi godine
         jpdBstranaRow.b_rsDO      = ZXC.projectYearLastDay .ToString("s").Substring(0, 10); // na bazi godine
         jpdBstranaRow.b_rsODcr    = ZXC.projectYearFirstDay.ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_rsDOcr    = ZXC.projectYearLastDay .ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_sati      = 0;
         jpdBstranaRow.b_ObrPrimPl = 0.00M;
         jpdBstranaRow.b_satiNeRad = 0;
      }

      #endregion Ostali primici koji se isplaćuju uz plaću-oporezivi  6.2.=0021

    //if(ptransRow.t_tt == Placa.TT_NEOPOREZPRIM)
    //{ 
    //   jpdBstranaRow.b_Bruto      = 
    //   jpdBstranaRow.b_Mio1stup   = 
    //   jpdBstranaRow.b_Mio2stup   = 
    //   jpdBstranaRow.b_ZdrNa      = 
    //   jpdBstranaRow.b_ZorNa      = 
    //   jpdBstranaRow.b_ZapNa      = 
    //   jpdBstranaRow.b_Mio1stupNa = 
    //   jpdBstranaRow.b_Mio2stupNa = 
    //   jpdBstranaRow.b_ZpiUk      =  
    //   jpdBstranaRow.b_ZapII      = 
    //   jpdBstranaRow.b_AHizdatak  = 
    //   jpdBstranaRow.b_MioAll     = 
    //   jpdBstranaRow.b_Dohodak    = 
    //   jpdBstranaRow.b_Odbitak    = 
    //   jpdBstranaRow.b_PorOsnAll  = 
    //   jpdBstranaRow.b_PorezAll   = 
    //   jpdBstranaRow.b_PorPrir    = 
    //   jpdBstranaRow.b_Prirez     = 0.00M;
    //}



      #region godisnji obracun poreza

      if(jpdBstranaRow.b_rsOO == "XY")// godisnji obracun poreza
      { 
         jpdBstranaRow.b_rsOD      = ZXC.projectYearFirstDay.ToString("s").Substring(0, 10); // na bazi godine
         jpdBstranaRow.b_rsDO      = ZXC.projectYearLastDay .ToString("s").Substring(0, 10); // na bazi godine
         jpdBstranaRow.b_rsODcr    = ZXC.projectYearFirstDay.ToString(ZXC.VvDateFormat);
         jpdBstranaRow.b_rsDOcr    = ZXC.projectYearLastDay .ToString(ZXC.VvDateFormat);
         //jpdBstranaRow.b_posInval  = "0";
         jpdBstranaRow.b_satiNeRad = 0;
      }

      #endregion godisnji obracun poreza

      return jpdBstranaRow;
   }

   #endregion RSm, JOPPD & Other Util Methods

   #region SetKonacniObracun_Alfa13Beta14Gama15_PtransRecs

   private void SetKonacniObracun_Alfa13Beta14Gama15_PtransRecs(uint personCD, PrulesStruct pR, DS_Placa.personRow personRowOfCurrentTrans)
   {
      PtransSumRec_beta14 = null;
      PtransSumRec_gama15 = null;

      List<Ptrans> thisPersonWholeYearPtransList;

      PtransSumRec_alfa13 = PtransDao.GetPtransSum_alfa13(ZXC.TheVvForm.TheDbConnection /* poboljsaj ovo! */, personCD, out thisPersonWholeYearPtransList);

      if(PtransSumRec_alfa13 == null) return; // moze biti null ako je ThisPersonHasNot12Placa_or_PrirezNotKonzistentan 

      PtransSumRec_beta14 = PtransDao.GetPtransSum_beta14(PtransSumRec_alfa13, pR, personRowOfCurrentTrans, thisPersonWholeYearPtransList);
      PtransSumRec_gama15 = PtransDao.GetPtransSum_gama15(PtransSumRec_alfa13, PtransSumRec_beta14);
   }

   #endregion SetKonacniObracun_Alfa13Beta14Gama15_PtransRecs

   #endregion PerformAdditionalDataSetOperation

}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptP_JOPPD : VvPlacaReport
{
   public RptP_JOPPD(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter): base(_reportDocument, externTblChooser, _reportName, _rptFilter)
   {
      IsForExport = true;

      jpdStrA = new JOPPD_stranaA();

      // 10.02.2016: 
      if(RptFilter.ShowVirDatePod) // prisiljeni DokDate & joppdBroj za 
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE!\n\nIzrađuje se JOPPD sa 'prisiljenim' datumom i joppdID-om\n\n[{0}] / [{1}]", 
            RptFilter.VirDatePod.ToString(ZXC.VvDateFormat), RptFilter.ForcedRSmID);
      }
   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      // TODO: !!! cemu vo sluzi 
      return GetPtrans_Command(conn, "", " t_personCD, t_dokNum, t_serial ");
   }

   // ovdje nisi nasao nikakve upute kako bi se trebao zvati fajl pa ga imenujes proizvoljno 
   public override string ExportFileName
   {
      get
      {
         // 10.02.2016: 
       //string rmsID = RptFilter.RSmID      ;
         string rmsID = RptFilter.ForcedRSmID;
       //string mmyyyy = RptFilter.DatumOd.Month.ToString("00") + RptFilter.DatumOd.Year.ToString("0000");
       //string   yyyy =                                          RptFilter.DatumOd.Year.ToString("0000");

         return "JOPPD_" + ZXC.CURR_prjkt_rec.Oib + "_" + rmsID + ".xml";
      }
   }

   #region Xml Schema Validation

   public override bool ExecuteExportValidation(string fileName)
   {
      List<ZXC.VvXmlValidationData> valDataList = new List<ZXC.VvXmlValidationData>();
      
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacJOPPD/v1-1", @"XSD\ObrazacJOPPD-v1-1.xsd"          ));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacJOPPD/v1-1", @"XSD\ObrazacJOPPDtipovi-v1-1.xsd"    ));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"           , @"XSD\ObrazacJOPPDmetapodaci-v1-1.xsd"));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"           , @"XSD\MetapodaciTipovi-v2-0.xsd"      ));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/TemeljniTipovi/v2-1"       , @"XSD\TemeljniTipovi-v2-1.xsd"        ));

      return ExecuteExportValidation_Base(valDataList);
   }

   #endregion Xml Schema Validation

   //struct stranaA
   //{
   //   public string dateIzvj   ;
   //   public string oznakaIzvj ;
   //   public string vrstaIzvj  ;
   //   public string piNaziv    ;
   //   public string piMjesto   ;
   //   public string piUlica    ;
   //   public string piUlicaBr  ;
   //   public string piEmail    ;
   //   public string piOIB      ;
   //   public string piOznaka   ;
   //   public string opNaziv    ;
   //   public string opMjesto   ;
   //   public string opUlica    ;
   //   public string opUlicaBr  ;
   //   public string opEmail    ;
   //   public string opOIB      ;
   //   public string brojOsoba  ;
   //   public string brojRedaka ;
   //   public string porP01     ;
   //   public string porP11     ;
   //   public string porP12     ;
   //   public string porP02     ;
   //   public string porP03     ;
   //   public string porP04     ;
   //   public string porP05     ;
   //   public string mio1P01    ;
   //   public string mio1P02    ;
   //   public string mio1P03    ;
   //   public string mio1P04    ;
   //   public string mio1P05    ;
   //   public string mio1P06    ;
   //   public string mio2P01    ;
   //   public string mio2P02    ;
   //   public string mio2P03    ;
   //   public string mio2P04    ;
   //   public string mio2P05    ;
   //   public string zdrP01     ;
   //   public string zdrP02     ;
   //   public string zdrP03     ;
   //   public string zdrP04     ;
   //   public string zdrP05     ;
   //   public string zdrP06     ;
   //   public string zdrP07     ;
   //   public string zdrP08     ;
   //   public string zdrP09     ;
   //   public string zdrP10     ;
   //   public string zapP01     ;
   //   public string zapP02     ;
   //   public string npNetto    ;
   //   public string ktaMO2     ;
   //   public string sastIme    ;
   //   public string sastPrz    ;
   //}

   struct stranaB
   {
      public string P001;
      public string P002;
      public string P003;
      public string P004;
      public string P005;
      public string P061;
      public string P062;
      public string P071;
      public string P072;
      public string P008;
      public string P009;
      public string P010;
      public string P101;
      public string P102;
      public string P011;
      public string P012;
      public string P121;
      public string P122;
      public string P123;
      public string P124;
      public string P125;
      public string P126;
      public string P127;
      public string P128;
      public string P129;
      public string P131;
      public string P132;
      public string P133;
      public string P134;
      public string P135;
      public string P141;
      public string P142;
      public string P151;
      public string P152;
      public string P161;
      public string P162;
      public string P017;
      public string P100; // od 28.02.2015.
   }

   public override bool ExecuteExport(string fileName)
   {
      if(ZXC.TheVvForm.BadGuys("JOPPD"))
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Stara verzija JOPPD xml sheme.");
         return false;
      }

      DS_Placa.jpdBstranaDataTable jpdBstranaTable = ds_PlacaReport.jpdBstrana;

      #region Initialize XmlWriterSettings

      //if(Faktur_rec_SumaRazdoblja_URA == null || Faktur_rec_SumaRazdoblja_IRA == null) throw new Exception("Nema se što exporitrati!");

      string ident = "   ";

      XmlWriterSettings settings = new XmlWriterSettings();
      
      settings.Indent = true;
      settings.IndentChars = ident;

      #endregion Initialize XmlWriterSettings

      using(XmlWriter writer = XmlWriter.Create(fileName, settings))
      {
         #region Init Xml Document

         writer.WriteStartDocument();

         writer.WriteStartElement   ("ObrazacJOPPD", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacJOPPD/v1-1");
         writer.WriteAttributeString("verzijaSheme",                                                                  "1.1");

         #endregion Init Xml Document

         #region Metapodaci

         writer.WriteStartElement("Metapodaci", @"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0");
            //writer.WriteElementString("Naslov", @"http://purl.org/dc/elements/1.1/title", "Prijava poreza na dodanu vrijednost");
            writer.WriteRaw("\n");
            writer.WriteRaw(ident + ident + "<Naslov dc=\"http://purl.org/dc/elements/1.1/title\">Izvješće o primicima, porezu na dohodak i prirezu te doprinosima za obvezna osiguranja</Naslov>\n");
            writer.WriteRaw(ident + ident + "<Autor dc=\"http://purl.org/dc/elements/1.1/creator\">" + ZXC.CURR_prjkt_rec.Ime + " " + ZXC.CURR_prjkt_rec.Prezime + "</Autor>\n");
            writer.WriteRaw(ident + ident + "<Datum dc=\"http://purl.org/dc/elements/1.1/date\">" + DateTime.Now.ToString("s") + "</Datum>\n");
            writer.WriteRaw(ident + ident + "<Format dc=\"http://purl.org/dc/elements/1.1/format\">text/xml</Format>\n");
            writer.WriteRaw(ident + ident + "<Jezik dc=\"http://purl.org/dc/elements/1.1/language\">hr-HR</Jezik>\n");
            writer.WriteRaw(ident + ident + "<Identifikator dc=\"http://purl.org/dc/elements/1.1/identifier\">" + Guid.NewGuid().ToString("D")/*.ToUpper()*/ + "</Identifikator>\n");
            writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacJOPPD-v1-1</Uskladjenost>\n");

            writer.WriteRaw(ident + ident + "<Tip dc=\"http://purl.org/dc/elements/1.1/type\">Elektronički obrazac</Tip>\n");
            writer.WriteRaw(ident + ident + "<Adresant>Ministarstvo Financija, Porezna uprava, Zagreb</Adresant>\n");
            writer.WriteRaw(ident);
         writer.WriteEndElement(); // Metapodaci 

         #endregion Write Header Data

         #region StranaA

         #region Set DATA

         //stranaA strA = new stranaA();

         //DS_Placa.placaSumDataTable placaSumTable = ds_PlacaReport.placaSum;

         //// ... TODO: ... 

         //strA.dateIzvj   = DateTime.Now.ToString("s").Substring(0, 10);
         //strA.oznakaIzvj = "12345";
         //strA.vrstaIzvj  = "1" ;
         //strA.piNaziv    = ZXC.CURR_prjkt_rec.Naziv          ;
         //strA.piMjesto   = ZXC.CURR_prjkt_rec.Grad           ;
         //strA.piUlica    = ZXC.CURR_prjkt_rec.UlicaBezBroja_1;
         //strA.piUlicaBr  = ZXC.CURR_prjkt_rec.UlicniBroj_1   ;
         //strA.piEmail    = ZXC.CURR_prjkt_rec.Email          ;
         //strA.piOIB      = ZXC.CURR_prjkt_rec.Oib            ;
         //strA.piOznaka   = "1";
         //strA.opNaziv    = ZXC.CURR_prjkt_rec.Naziv          ;
         //strA.opMjesto   = ZXC.CURR_prjkt_rec.Grad           ;
         //strA.opUlica    = ZXC.CURR_prjkt_rec.UlicaBezBroja_1;
         //strA.opUlicaBr  = ZXC.CURR_prjkt_rec.UlicniBroj_1   ;
         //strA.opEmail    = ZXC.CURR_prjkt_rec.Email          ;
         //strA.opOIB      = ZXC.CURR_prjkt_rec.Oib            ;
         //strA.brojOsoba  = "44";
         //strA.brojRedaka = "55";
         //strA.porP01     = placaSumTable[11].X_rMio1stup.ToStringVv_NoGroup_ForceDot();
         //strA.porP11     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.porP12     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.porP02     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.porP03     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.porP04     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.porP05     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.mio1P01    = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.mio1P02    = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.mio1P03    = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.mio1P04    = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.mio1P05    = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.mio1P06    = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.mio2P01    = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.mio2P02    = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.mio2P03    = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.mio2P04    = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.mio2P05    = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.zdrP01     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.zdrP02     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.zdrP03     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.zdrP04     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.zdrP05     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.zdrP06     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.zdrP07     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.zdrP08     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.zdrP09     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.zdrP10     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.zapP01     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.zapP02     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.npNetto    = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.ktaMO2     = 123.45M.ToStringVv_NoGroup_ForceDot();
         //strA.sastIme    = "Roko";
         //strA.sastPrz    = "Prč";

         #endregion Set DATA

         #region Spritz XML strings

         writer.WriteStartElement("StranaA");

            writer.WriteElementString("DatumIzvjesca" , jpdStrA.dateIzvj.ToString("s").Substring(0, 10));
            writer.WriteElementString("OznakaIzvjesca", jpdStrA.oznakaIzvj );
            writer.WriteElementString("VrstaIzvjesca" , jpdStrA.vrstaIzvj  );
            writer.WriteStartElement("PodnositeljIzvjesca");
               writer.WriteElementString("Naziv",       jpdStrA.piNaziv);
                 writer.WriteStartElement("Adresa");
                    writer.WriteElementString("Mjesto", jpdStrA.piMjesto   );
                    writer.WriteElementString("Ulica" , jpdStrA.piUlica    );
                    writer.WriteElementString("Broj"  , jpdStrA.piUlicaBr  );
                 writer.WriteEndElement(); // Adresa 
               writer.WriteElementString("Email" ,      jpdStrA.piEmail    );
               writer.WriteElementString("OIB"   ,      jpdStrA.piOIB      );
               writer.WriteElementString("Oznaka",      jpdStrA.piOznaka   );
            writer.WriteEndElement(); // PodnositeljIzvjesca 
          //writer.WriteStartElement("ObveznikPlacanja");                    18.05.2018. nakon zastite podataka?!Podaci o obvezniku plaćanja popunjavaju se samo za oznake podnositelja 6 -13.
          //   writer.WriteElementString("Naziv",       jpdStrA.opNaziv    );
          //     writer.WriteStartElement("Adresa");           
          //        writer.WriteElementString("Mjesto", jpdStrA.opMjesto   );
          //        writer.WriteElementString("Ulica" , jpdStrA.opUlica    );
          //        writer.WriteElementString("Broj"  , jpdStrA.opUlicaBr  );
          //     writer.WriteEndElement(); // Adresa 
          //   writer.WriteElementString("Email" ,      jpdStrA.opEmail    );
          //   writer.WriteElementString("OIB"   ,      jpdStrA.opOIB      );
          //writer.WriteEndElement(); // ObveznikPlacanja 
            writer.WriteElementString("BrojOsoba"  ,    jpdStrA.brojOsoba .ToString());
            writer.WriteElementString("BrojRedaka" ,    jpdStrA.brojRedaka.ToString());
            writer.WriteStartElement("PredujamPoreza");
               writer.WriteElementString("P1",          jpdStrA.porP01.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("P11",         jpdStrA.porP11.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("P12",         jpdStrA.porP12.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("P2",          jpdStrA.porP02.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("P3",          jpdStrA.porP03.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("P4",          jpdStrA.porP04.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("P5",          jpdStrA.porP05.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("P6",          jpdStrA.porP06.ToStringVv_NoGroup_ForceDot());
            writer.WriteEndElement(); // PredujamPoreza 
            writer.WriteStartElement("Doprinosi");
               writer.WriteStartElement("GeneracijskaSolidarnost");
                  writer.WriteElementString("P1",       jpdStrA.mio1P01.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P2",       jpdStrA.mio1P02.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P3",       jpdStrA.mio1P03.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P4",       jpdStrA.mio1P04.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P5",       jpdStrA.mio1P05.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P6",       jpdStrA.mio1P06.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P7",       jpdStrA.mio1P07.ToStringVv_NoGroup_ForceDot());
               writer.WriteEndElement(); // GeneracijskaSolidarnost 
               writer.WriteStartElement("KapitaliziranaStednja");
                  writer.WriteElementString("P1",       jpdStrA.mio2P01.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P2",       jpdStrA.mio2P02.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P3",       jpdStrA.mio2P03.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P4",       jpdStrA.mio2P04.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P5",       jpdStrA.mio2P05.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P6",       jpdStrA.mio2P06.ToStringVv_NoGroup_ForceDot());
               writer.WriteEndElement(); // KapitaliziranaStednja 
               writer.WriteStartElement("ZdravstvenoOsiguranje");
                  writer.WriteElementString("P1",       jpdStrA.zdrP01.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P2",       jpdStrA.zdrP02.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P3",       jpdStrA.zdrP03.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P4",       jpdStrA.zdrP04.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P5",       jpdStrA.zdrP05.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P6",       jpdStrA.zdrP06.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P7",       jpdStrA.zdrP07.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P8",       jpdStrA.zdrP08.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P9",       jpdStrA.zdrP09.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P10",      jpdStrA.zdrP10.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P11",      jpdStrA.zdrP11.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P12",      jpdStrA.zdrP12.ToStringVv_NoGroup_ForceDot());
               writer.WriteEndElement(); // ZdravstvenoOsiguranje 
               writer.WriteStartElement("Zaposljavanje");
                  writer.WriteElementString("P1",       jpdStrA.zapP01.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P2",       jpdStrA.zapP02.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P3",       jpdStrA.zapP03.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P4",       jpdStrA.zapP04.ToStringVv_NoGroup_ForceDot());
               writer.WriteEndElement(); // Zaposljavanje 
            writer.WriteEndElement(); // Doprinosi 
            writer.WriteElementString("IsplaceniNeoporeziviPrimici", jpdStrA.npNetto.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("KamataMO2"                  , jpdStrA.ktaMO2 .ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("UkupniNeoporeziviPrimici"   , jpdStrA.nerezid.ToStringVv_NoGroup_ForceDot());
            writer.WriteStartElement("NaknadaZaposljavanjeInvalida");
                  writer.WriteElementString("P1",       jpdStrA.nzInvP1.ToString());
                  writer.WriteElementString("P2",       jpdStrA.nzInvP2.ToStringVv_NoGroup_ForceDot());
            writer.WriteEndElement(); // NaknadaZaposljavanjeInvalida 
            writer.WriteStartElement("IzvjesceSastavio");
               writer.WriteElementString("Ime",        jpdStrA.sastIme     );
               writer.WriteElementString("Prezime",    jpdStrA.sastPrz     );
            writer.WriteEndElement(); // IzvjesceSastavio 

            writer.WriteEndElement(); // StranaA 

         #endregion Spritz XML strings

         #endregion StranaA

         #region StranaB

         writer.WriteStartElement("StranaB");
            writer.WriteStartElement("Primatelji");

         foreach(DS_Placa.jpdBstranaRow jpdBstranaRow in jpdBstranaTable.Rows)
         {
            #region Set DATA

            stranaB strB = new stranaB();

            strB.P001 = jpdBstranaRow.b_rbr       .ToString();
            strB.P002 = jpdBstranaRow.b_opcCD     ;
            strB.P003 = jpdBstranaRow.b_opcRadCD  ;
            strB.P004 = jpdBstranaRow.b_oib       ;
            strB.P005 = jpdBstranaRow.b_ime       ;
            strB.P061 = jpdBstranaRow.b_stjecatCD ;
            strB.P062 = jpdBstranaRow.b_primDohCD ;
            strB.P071 = jpdBstranaRow.b_rsB       ;
            strB.P072 = jpdBstranaRow.b_posInval  ;
            strB.P008 = jpdBstranaRow.b_pocKrajCD ;
            strB.P009 = jpdBstranaRow.b_radVr     ;
            strB.P010 = jpdBstranaRow.b_sati      .ToStringVv_NoDecimalNoGroup(); 
            strB.P101 = jpdBstranaRow.b_rsOD      ;
            strB.P102 = jpdBstranaRow.b_rsDO      ;
            strB.P011 = jpdBstranaRow.b_Bruto     .ToStringVv_NoGroup_ForceDot();
          //strB.P012 = jpdBstranaRow.b_Bruto     .ToStringVv_NoGroup_ForceDot(); 08.01.2014. falilo kod ZPI-a
            strB.P012 = jpdBstranaRow.b_MioOsn    .ToStringVv_NoGroup_ForceDot();
            strB.P121 = jpdBstranaRow.b_Mio1stup  .ToStringVv_NoGroup_ForceDot();
            strB.P122 = jpdBstranaRow.b_Mio2stup  .ToStringVv_NoGroup_ForceDot();
            strB.P123 = jpdBstranaRow.b_ZdrNa     .ToStringVv_NoGroup_ForceDot();
            strB.P124 = jpdBstranaRow.b_ZorNa     .ToStringVv_NoGroup_ForceDot();
            strB.P125 = jpdBstranaRow.b_ZapNa     .ToStringVv_NoGroup_ForceDot();
            strB.P126 = jpdBstranaRow.b_Mio1stupNa.ToStringVv_NoGroup_ForceDot();
            strB.P127 = jpdBstranaRow.b_Mio2stupNa.ToStringVv_NoGroup_ForceDot();
            strB.P128 = jpdBstranaRow.b_ZpiUk     .ToStringVv_NoGroup_ForceDot();
            strB.P129 = jpdBstranaRow.b_ZapII     .ToStringVv_NoGroup_ForceDot(); // ovdje je od 122023 Mio1Olaksica
            strB.P131 = jpdBstranaRow.b_AHizdatak .ToStringVv_NoGroup_ForceDot();
            strB.P132 = jpdBstranaRow.b_MioAll    .ToStringVv_NoGroup_ForceDot();
            strB.P133 = jpdBstranaRow.b_Dohodak   .ToStringVv_NoGroup_ForceDot();
            strB.P134 = jpdBstranaRow.b_Odbitak   .ToStringVv_NoGroup_ForceDot();
            strB.P135 = jpdBstranaRow.b_PorOsnAll .ToStringVv_NoGroup_ForceDot();
            strB.P141 = jpdBstranaRow.b_PorezAll  .ToStringVv_NoGroup_ForceDot();
            strB.P142 = jpdBstranaRow.b_Prirez    .ToStringVv_NoGroup_ForceDot();
            strB.P151 = jpdBstranaRow.b_neoPrimCD ;
            strB.P152 = jpdBstranaRow.b_NetoAdd   .ToStringVv_NoGroup_ForceDot();
            strB.P161 = jpdBstranaRow.b_nacIsplCD ;
            strB.P162 = jpdBstranaRow.b_Netto     .ToStringVv_NoGroup_ForceDot();
          //strB.P017 = jpdBstranaRow.b_Bruto     .ToStringVv_NoGroup_ForceDot(); 10.01.2014.
            strB.P017 = jpdBstranaRow.b_ObrPrimPl .ToStringVv_NoGroup_ForceDot();

            // od 28.02.2015.
            strB.P100 = jpdBstranaRow.b_satiNeRad.ToStringVv_NoDecimalNoGroup(); 

            #endregion Set DATA

            #region Spritz XML strings

            writer.WriteStartElement("P");

            writer.WriteElementString("P1"  , strB.P001);
            writer.WriteElementString("P2"  , strB.P002);
            writer.WriteElementString("P3"  , strB.P003);
            writer.WriteElementString("P4"  , strB.P004);
            writer.WriteElementString("P5"  , strB.P005);
            writer.WriteElementString("P61" , strB.P061);
            writer.WriteElementString("P62" , strB.P062);
            writer.WriteElementString("P71" , strB.P071);
            writer.WriteElementString("P72" , strB.P072);
            writer.WriteElementString("P8"  , strB.P008);
            writer.WriteElementString("P9"  , strB.P009);
            writer.WriteElementString("P10" , strB.P010);
            writer.WriteElementString("P100", strB.P100);
            writer.WriteElementString("P101", strB.P101);
            writer.WriteElementString("P102", strB.P102);
            writer.WriteElementString("P11" , strB.P011);
            writer.WriteElementString("P12" , strB.P012);
            writer.WriteElementString("P121", strB.P121);
            writer.WriteElementString("P122", strB.P122);
            writer.WriteElementString("P123", strB.P123);
            writer.WriteElementString("P124", strB.P124);
            writer.WriteElementString("P125", strB.P125);
            writer.WriteElementString("P126", strB.P126);
            writer.WriteElementString("P127", strB.P127);
            writer.WriteElementString("P128", strB.P128);
            writer.WriteElementString("P129", strB.P129);
            writer.WriteElementString("P131", strB.P131);
            writer.WriteElementString("P132", strB.P132);
            writer.WriteElementString("P133", strB.P133);
            writer.WriteElementString("P134", strB.P134);
            writer.WriteElementString("P135", strB.P135);
            writer.WriteElementString("P141", strB.P141);
            writer.WriteElementString("P142", strB.P142);
            writer.WriteElementString("P151", strB.P151);
            writer.WriteElementString("P152", strB.P152);
            writer.WriteElementString("P161", strB.P161);
            writer.WriteElementString("P162", strB.P162);
            writer.WriteElementString("P17" , strB.P017);

            writer.WriteEndElement(); // P 

            #endregion Spritz XML strings

         } // foreach(DS_Placa.jpdBstranaRow jpdBstranaRow in jpdBstranaTable.Rows) 

            writer.WriteEndElement(); //          writer.WriteEndElement(); // Primatelji 
         writer.WriteEndElement(); // StranaB 

         #endregion StranaB

         #region Finish Xml Document

         writer.WriteEndElement();
         writer.WriteEndDocument();

         #endregion Finish Xml Document

      }

      return true;
   }

}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptP_ObrazRSm : VvPlacaReport
{
   public RptP_ObrazRSm(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter): base(_reportDocument, externTblChooser, _reportName, _rptFilter)
   {
      IsForExport = true;

      InitializeExportStuff();
   }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      return GetPtrans_Command(conn, "", " t_personCD, t_dokNum, t_serial ");
   }

   public override string ExportFileName { get { return "REGOS.RM0"; } }

   public override bool ExecuteExport(string fileName)
   {
      DS_Placa.personDataTable     personTable     = ds_PlacaReport.person;
      DS_Placa.rsmBstranaDataTable rsmBstranaTable = ds_PlacaReport.rsmBstrana;
      DS_Placa.placaSumDataTable   placaSumTable   = ds_PlacaReport.placaSum;

      DS_Placa.placaSumRow placaSumRow = ds_PlacaReport.placaSum[0];

      //using(StreamWriter sw = new StreamWriter(ExportFileName, false, Encoding.GetEncoding(1250)))
      using(StreamWriter sw = new StreamWriter(fileName, false, Encoding.GetEncoding(1250)))
      {
         SetSlog_0_Values(/*ZXC.ROOT_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec);
         VvImpExp.DumpFields(sw, Slog_0_Fields);

         SetSlog_3_Values(ZXC.CURR_prjkt_rec, placaSumRow);
         VvImpExp.DumpFields(sw, Slog_3_Fields);

         foreach(DS_Placa.rsmBstranaRow rsmBstranaRow in rsmBstranaTable.Rows)
         {
            SetSlog_5_Values(rsmBstranaRow);
            VvImpExp.DumpFields(sw, Slog_5_Fields);
         }

         SetSlog_7_Values(ZXC.CURR_prjkt_rec, placaSumRow, rsmBstranaTable.Rows.Count);
         VvImpExp.DumpFields(sw, Slog_7_Fields);

         SetSlog_9_Values(/*ZXC.ROOT_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec);
         VvImpExp.DumpFields(sw, Slog_9_Fields);
      }

      ZXC.LastExportFileHASHcode = GetHashCodeForFile(fileName);

      ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Gotovo.\n\nKreirao datoteku\n\n" + fileName + "\n\nBroj djelatnika: " + placaSumRow.X_personCount + "\n\nBroj redaka B strane: " + rsmBstranaTable.Rows.Count + "\n\nKONTROLNI BROJ: " + ZXC.LastExportFileHASHcode);

      return true;
   }

   #region EXPORT DETAILS

   #region Propertiz

   private Dictionary<string, VvImpExp.ImpExpField> Slog_0_Dict { get; set; }
   private Dictionary<string, VvImpExp.ImpExpField> Slog_3_Dict { get; set; }
   private Dictionary<string, VvImpExp.ImpExpField> Slog_5_Dict { get; set; }
   private Dictionary<string, VvImpExp.ImpExpField> Slog_7_Dict { get; set; }
   private Dictionary<string, VvImpExp.ImpExpField> Slog_9_Dict { get; set; }

   private const int lineFixedLength = 250;

   public int? NumOfFileLines { get; private set; }

   public int NumOfTransLinesSoFar { get; private set; }

   private decimal MoneySUM { get; set; }

   //private bool slog_0_OK = true, slog_3_OK = true, slog_5_OK = true, slog_7_OK = true, slog_9_OK = true, linesOK = true, fileOK = true;

   //public  bool BadData
   //{
   //   get
   //   {
   //      return (!slog_0_OK || !slog_3_OK || !slog_5_OK || !slog_7_OK || !slog_9_OK || !linesOK);
   //   }
   //}

   #endregion Propertiz

   #region Element Arrays

   // •	Tip sloga "0"  -   POČETNI SLOG FORMATA  
   VvImpExp.ImpExpField[] Slog_0_Fields = new VvImpExp.ImpExpField[]
   {
      new VvImpExp.ImpExpField("RS0REZ1",   1,   1, 12), 
      new VvImpExp.ImpExpField("RS0IBOO",   2,  13, 13), 
      new VvImpExp.ImpExpField("RS0NAZO",   3,  26, 50),
      new VvImpExp.ImpExpField("RS0ADRO",   4,  76, 50), 
      new VvImpExp.ImpExpField("RS0OSOB",   5, 126, 50), 
      new VvImpExp.ImpExpField("RS0TELE",   6, 176, 10), 
      new VvImpExp.ImpExpField("RS0MAIL",   7, 186, 30),
      new VvImpExp.ImpExpField("RS0DTMN",   8, 216,  8), 
      new VvImpExp.ImpExpField("RS0OFOR",   9, 224,  5), 
      new VvImpExp.ImpExpField("RS0REZE",  10, 229, 21), 
      new VvImpExp.ImpExpField("RS0TIPS",  11, 250,  1), 
   };

   // •	Tip sloga "3"  -   POČETNI SLOG JEDNOG OBRASCA R-Sm U FORMATU 
   VvImpExp.ImpExpField[] Slog_3_Fields = new VvImpExp.ImpExpField[]
   {
      new VvImpExp.ImpExpField("RS3REZ1",   1,   1, 12), 
      new VvImpExp.ImpExpField("RS3IBOO",   2,  13, 13), 
      new VvImpExp.ImpExpField("RS3IDOB",   3,  26,  4),
      new VvImpExp.ImpExpField("RS3NAOB",   4,  30, 50), 
      new VvImpExp.ImpExpField("RS3ADOB",   5,  80, 50), 
      new VvImpExp.ImpExpField("RS3VROB",   6, 130,  2), 
      new VvImpExp.ImpExpField("RS3MMGG",   7, 132,  6),
      new VvImpExp.ImpExpField("RS3VROI",   8, 138,  2), 
      new VvImpExp.ImpExpField("RS3BROS",   9, 140,  5), 
      new VvImpExp.ImpExpField("RS3IBRT",  10, 145, 15), 
      new VvImpExp.ImpExpField("RS3IOSN",  11, 160, 15), 
      new VvImpExp.ImpExpField("RS3IST1",  12, 175, 15), 
      new VvImpExp.ImpExpField("RS3IST2",  13, 190, 15), 
      new VvImpExp.ImpExpField("RS3INET",  14, 205, 15), 
      new VvImpExp.ImpExpField("RS3IGMM",  15, 220,  6), 
      new VvImpExp.ImpExpField("RS3REZE",  16, 226, 24), 
      new VvImpExp.ImpExpField("RS3TIPS",  17, 250,  1), 
   };

   //•	Tip sloga "5"  -   SLOG POJEDINAČNE TRANSAKCIJE OBRASCA R-Sm 
   VvImpExp.ImpExpField[] Slog_5_Fields = new VvImpExp.ImpExpField[]
   {
      new VvImpExp.ImpExpField("RS5RBRO",   1,   1,  5), 
      new VvImpExp.ImpExpField("RS5IBOO",   2,   6, 13), 
      new VvImpExp.ImpExpField("RS5PRIM",   3,  19, 30),
      new VvImpExp.ImpExpField("RS5GROP",   4,  49,  4), 
      new VvImpExp.ImpExpField("RS5OSOB",   5,  53,  3), 
      new VvImpExp.ImpExpField("RS5RAZD",   6,  56,  4), 
      new VvImpExp.ImpExpField("RS5IBRT",   7,  60, 15),
      new VvImpExp.ImpExpField("RS5IOSN",   8,  75, 15), 
      new VvImpExp.ImpExpField("RS5IST1",   9,  90, 15), 
      new VvImpExp.ImpExpField("RS5IST2",  10, 105, 15), 
      new VvImpExp.ImpExpField("RS5INET",  11, 120, 15), 
      new VvImpExp.ImpExpField("RS5REZE",  12, 135,115), 
      new VvImpExp.ImpExpField("RS5TIPS",  13, 250,  1), 
   };

   //•	Tip sloga "7"  -   ZAVRŠNI SLOG JEDNOG OBRASCA R-Sm U FORMATU 
   VvImpExp.ImpExpField[] Slog_7_Fields = new VvImpExp.ImpExpField[]
   {
      new VvImpExp.ImpExpField("RS7REZ1",   1,   1, 12), 
      new VvImpExp.ImpExpField("RS7IBOO",   2,  13, 13), 
      new VvImpExp.ImpExpField("RS7IDOB",   3,  26,  4),
      new VvImpExp.ImpExpField("RS7BRSL",   4,  30,  5), 
      new VvImpExp.ImpExpField("RS7DTRS",   5,  35,  8), 
      new VvImpExp.ImpExpField("RS7REZE",   6,  43,207), 
      new VvImpExp.ImpExpField("RS7TIPS",   7, 250,  1),
   };

   //•	Tip sloga "9"  -   ZAVRŠNI SLOG FORMATA 
   VvImpExp.ImpExpField[] Slog_9_Fields = new VvImpExp.ImpExpField[]
   {
      new VvImpExp.ImpExpField("RS9REZ1",   1,   1, 12), 
      new VvImpExp.ImpExpField("RS9IBOO",   2,  13, 13), 
      new VvImpExp.ImpExpField("RS9DTMN",   3,  26,  8),
      new VvImpExp.ImpExpField("RS9BROJ",   4,  34,  6), 
      new VvImpExp.ImpExpField("RS9REZE",   5,  40,210), 
      new VvImpExp.ImpExpField("RS9TIPS",   6, 250,  1), 
   };

   #endregion Element Arrays

   #region Methods

   private string GetHashCodeForFile(string fileName)
   {
      string hashCode, hashLetters, h1, h2, h3, h4;

      using(System.Security.Cryptography.HashAlgorithm hashAlg = new System.Security.Cryptography.SHA1Managed())
      {
         using(Stream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
         {
            byte[] hash = hashAlg.ComputeHash(file);

            hashLetters = BitConverter.ToString(hash).Replace("-", "");
         }
      }

      h1 = hashLetters.Substring( 0, 10);
      h2 = hashLetters.Substring(10, 10);
      h3 = hashLetters.Substring(20, 10);
      h4 = hashLetters.Substring(30, 10);

      hashCode = h1 + " - " + h2 + " - " + h3 + " - " + h4;

      return hashCode;
   }

   private void InitializeExportStuff()
   {
      this.NumOfTransLinesSoFar = 0;
      this.MoneySUM             = 0.00M;

      Slog_0_Dict = VvImpExp.CreateDictionary(Slog_0_Fields, "Slog_0", lineFixedLength);
      Slog_3_Dict = VvImpExp.CreateDictionary(Slog_3_Fields, "Slog_3", lineFixedLength);
      Slog_5_Dict = VvImpExp.CreateDictionary(Slog_5_Fields, "Slog_5", lineFixedLength);
      Slog_7_Dict = VvImpExp.CreateDictionary(Slog_7_Fields, "Slog_7", lineFixedLength);
      Slog_9_Dict = VvImpExp.CreateDictionary(Slog_9_Fields, "Slog_9", lineFixedLength);
   }

   private void SetSlog_0_Values(Prjkt root_prjkt_rec)
   {

//R.br. polja	Naziv polja	Opis polja	Tip
//polja	Od	Do	Broj
//mjesta
//1.		RS0REZ1	Rezerva (prazno)	C	1	12	12
//2.		RS0IBOO	Identifikacijski broj ovlaštene osobe za predaju obrasca	C	13	25	13
//3.		RS0NAZO	Naziv/prezime i ime ovlaštene osobe	C	26	75	50
//4.		RS0ADRO	Adresa ovlaštene osobe	C	76	125	50
//5.		RS0OSOB	Ime i prezime osobe za kontakt 	C	126	175	50
//6.		RS0TELE	Broj telefona za kontakt	C	176	185	10
//7.		RS0MAIL	Adresa elektronske pošte za kontakt	C	186	215	30
//8.		RS0DTMN	Datum formiranja formata (DDMMGGGG)	N	216	223	8
//9.		RS0OFOR	Oznaka formata ("RM500")	C	224	228	5
//10.	   RS0REZE	Rezerva (prazno) 	C	229	249	21
//11.	   RS0TIPS	Tip sloga ("0") 	N	250	250	1


      /*  1 */ Slog_0_Dict["RS0REZ1"].FldValue = "";
      /*  2 */ Slog_0_Dict["RS0IBOO"].FldValue = "  " + root_prjkt_rec.Oib;
      /*  3 */ Slog_0_Dict["RS0NAZO"].FldValue = root_prjkt_rec.Naziv + " / " + root_prjkt_rec.Prezime + " " + root_prjkt_rec.Ime;
      /*  4 */ Slog_0_Dict["RS0ADRO"].FldValue = root_prjkt_rec.Ulica1 + ", " + root_prjkt_rec.PostaBr + " " + root_prjkt_rec.Grad;
      /*  5 */ Slog_0_Dict["RS0OSOB"].FldValue = root_prjkt_rec.Ime + " " + root_prjkt_rec.Prezime;
      /*  6 */ Slog_0_Dict["RS0TELE"].FldValue = root_prjkt_rec.Tel1;
      /*  7 */ Slog_0_Dict["RS0MAIL"].FldValue = root_prjkt_rec.Email;
      /*  8 */ Slog_0_Dict["RS0DTMN"].SetDDMMYYYYFldValue(RptFilter.DateIzvj);
      /*  9 */ Slog_0_Dict["RS0OFOR"].FldValue = "RM500";
      /* 10 */ Slog_0_Dict["RS0REZE"].FldValue = "";
      /* 11 */ Slog_0_Dict["RS0TIPS"].FldValue = "0";

   }

   private void SetSlog_3_Values(Prjkt curr_prjkt_rec, DS_Placa.placaSumRow placaSumRow)
   {
//R.br. polja	Naziv polja	Opis polja	Tip
//polja	Od	Do	Broj
//mjesta
//1.		RS3REZ1	Rezerva (prazno)	C	1	12	12
//2.		RS3IBOO	Identifikacijski broj obveznika	C	13	25	13
//3.		RS3IDOB	Identifikator obrasca (od 0001 do 9899)	N	26	29	4
//4.		RS3NAOB	Naziv/Prezime i ime obveznika 	C	30	79	50
//5.		RS3ADOB	Adresa obveznika	C	80	129	50
//6.		RS3VROB	Vrsta obveznika (prema šifarniku)	N	130	131	2
//7.		RS3MMGG	Mjesec i godina za koje se podnose podaci (MMGGGG)	N	132	137	6
//8.		RS3VROI	Vrsta obračuna/ispravka (prema šifarniku)	N	138	139	2
//9.		RS3BROS	Broj osiguranika za koje se podnosi Obrazac R-Sm (broj različitih identifikacijskih brojeva osiguranika na stranici B obrasca )	N	140	144	5
//10.		RS3IBRT	Iznos obračunane plaće/naknade/drugog dohotka 	N	145	159	15
//11.		RS3IOSN	Iznos osnovice za obračun doprinosa za MO	N	160	174	15
//12.		RS3IST1	Iznos obračunanog doprinosa za I. stup MO	N	175	189	15
//13.		RS3IST2	Iznos obračunanog doprinosa za II. stup MO	N	190	204	15
//14.		RS3INET	Iznos isplaćene plaće/naknade/drugog dohotka 	N	205	219	15
//15.		RS3IGMM	Godina i mjesec isplate  plaće/naknade/drugog dohotka (GGGGMM)	N	220	225	6
//16.		RS3REZE	Rezerva (prazno)	C	226	249	24
//17.		RS3TIPS	Tip sloga ("3")  	N	250	250	1

      /*  1 */ Slog_3_Dict["RS3REZ1"].FldValue = "";
      /*  2 */ Slog_3_Dict["RS3IBOO"].FldValue = "  " + curr_prjkt_rec.Oib;
      /*  3 */ Slog_3_Dict["RS3IDOB"].FldValue = placaSumRow.X_rSm_ID;
      /*  4 */ Slog_3_Dict["RS3NAOB"].FldValue = curr_prjkt_rec.Naziv;
      /*  5 */ Slog_3_Dict["RS3ADOB"].FldValue = curr_prjkt_rec.Ulica1 + ", " + curr_prjkt_rec.PostaBr + " " + curr_prjkt_rec.Grad;
      /*  6 */ Slog_3_Dict["RS3VROB"].FldValue = placaSumRow.X_VrstaObveznika;
      /*  7 */ Slog_3_Dict["RS3MMGG"].FldValue = placaSumRow.X_zaMMYYYY;
      /*  8 */ Slog_3_Dict["RS3VROI"].FldValue = placaSumRow.X_vrstaObr;
      /*  9 */ Slog_3_Dict["RS3BROS"].SetIntgerFldValue(placaSumRow.X_personCount);
      /* 10 */ Slog_3_Dict["RS3IBRT"].SetDecimalFldValue_RSm(placaSumRow.X_rTheBruto);
      /* 11 */ Slog_3_Dict["RS3IOSN"].SetDecimalFldValue_RSm(placaSumRow.X_rMioOsn);
      /* 12 */ Slog_3_Dict["RS3IST1"].SetDecimalFldValue_RSm(placaSumRow.X_rMio1stup);
      /* 13 */ Slog_3_Dict["RS3IST2"].SetDecimalFldValue_RSm(placaSumRow.X_rMio2stup);
      /* 14 */ Slog_3_Dict["RS3INET"].SetDecimalFldValue_RSm(placaSumRow.X_rNetto);
      /* 15 */ Slog_3_Dict["RS3IGMM"].FldValue = (placaSumRow.X_uYYYY + placaSumRow.X_uMM).NotEmpty() ? placaSumRow.X_uYYYY + placaSumRow.X_uMM : "000000";
      /* 16 */ Slog_3_Dict["RS3REZE"].FldValue = "";
      /* 17 */ Slog_3_Dict["RS3TIPS"].FldValue = "3";
   }

   private void SetSlog_7_Values(Prjkt curr_prjkt_rec, DS_Placa.placaSumRow placaSumRow, int bPageRowCount)
   {
//R.br. polja	Naziv polja	Opis polja	Tip
//polja	Od	Do	Broj
//mjesta
//1.		RS7REZ1	Rezerva (prazno)	C	1	12	12
//2.		RS7IBOO	Identifikacijski broj obveznika	C	13	25	13
//3.		RS7IDOB	Identifikator obrasca (od 0001 do 9899)	N	26	29	4
//4.		RS7BRSL	Broj slogova tipa 5 koji se pojavljuju u  obrascu čiji je ovo zadnji slog 	N	30	34	5
//5.		RS7DTRS	Datum formiranja obrasca (DDMMGGGG)	N	35	42	8
//6.		RS7REZE	Rezerva (prazno)	C	43	249	207
//7.		RS7TIPS	Tip sloga ("7")  	N	250	250	1

      /*  1 */ Slog_7_Dict["RS7REZ1"].FldValue = "";
      /*  2 */ Slog_7_Dict["RS7IBOO"].FldValue = "  " + curr_prjkt_rec.Oib;
      /*  3 */ Slog_7_Dict["RS7IDOB"].FldValue = placaSumRow.X_rSm_ID;
      /*  4 */ Slog_7_Dict["RS7BRSL"].SetIntgerFldValue(bPageRowCount);
      /*  5 */ Slog_7_Dict["RS7DTRS"].SetDDMMYYYYFldValue(RptFilter.DateIzvj);
      /*  6 */ Slog_7_Dict["RS7REZE"].FldValue = "";
      /*  7 */ Slog_7_Dict["RS7TIPS"].FldValue = "7";
   }

   private void SetSlog_9_Values(Prjkt root_prjkt_rec)
   {

//R.br. polja	Naziv polja	Opis polja	Tip
//polja	Od	Do	Broj
//mjesta
//1.		RS9REZ1	Rezerva (prazno)	C	1	12	12
//2.		RS9IBOO	Identifikacijski broj ovlaštene osobe za predaju obrasca	C	13	25	13
//3.		RS9DTMN	Datum formiranja formata (DDMMGGGG)	N	26	33	8
//4.		RS9BROJ	Broj Obrazaca R-Sm u formatu	N	34	39	6
//5.		RS9REZE	Rezerva (prazno)	C	40	249	210
//6.		RS9TIPS	Tip sloga ("9")  	N	250	250	1


      /*  1 */ Slog_9_Dict["RS9REZ1"].FldValue = "";
      /*  2 */ Slog_9_Dict["RS9IBOO"].FldValue = "  " + root_prjkt_rec.Oib;
      /*  3 */ Slog_9_Dict["RS9DTMN"].SetDDMMYYYYFldValue(RptFilter.DateIzvj);
      /*  4 */ Slog_9_Dict["RS9BROJ"].FldValue = "000001";
      /*  5 */ Slog_9_Dict["RS9REZE"].FldValue = "";
      /*  6 */ Slog_9_Dict["RS9TIPS"].FldValue = "9";

   }

   private void SetSlog_5_Values(DS_Placa.rsmBstranaRow rsmBstranaRow)
   {

//R.br. polja	Naziv polja	Opis polja	Tip
//polja	Od	Do	Broj
//mjesta
//1.		RS5RBRO	Redni broj osiguranika na Obrascu R-Sm 	N	1	5	5
//2.		RS5IBOO	Identifikacijski broj osiguranika	C	6	18	13
//3.		RS5PRIM	Prezime i ime osiguranika	C	19	48	30
//4.		RS5GROP	Šifra općine/grada rada (prema šifarniku)	N	49	52	4
//5.		RS5OSOB	Osnova obračuna : OO(2)+B(1)	N	53	55	3
//6.		RS5RAZD	Razdoblje s istom osnovom obračuna (ddDD)	N	56	59	4
//7.		RS5IBRT	Iznos obračunane plaće/naknade/drugog dohotka 	N	60	74	15
//8.		RS5IOSN	Iznos osnovice za obračun doprinosa za MO	N	75	89	15
//9.		RS5IST1	Iznos obračunanog doprinosa za I. stup MO	N	90	104	15
//10.		RS5IST2	Iznos obračunanog doprinosa za II. stup MO	N	105	119	15
//11.		RS5INET	Iznos isplaćene plaće/naknade/drugog dohotka 	N	120	134	15
//12.		RS5REZE	Rezerva (prazno)	C	135	249	115
//13.		RS5TIPS	Tip sloga ("5")	N	250	250	1


      /*  1 */ Slog_5_Dict["RS5RBRO"].SetIntgerFldValue(ZXC.ValOrZero_Int(rsmBstranaRow.b_rbr));
      /*  2 */ Slog_5_Dict["RS5IBOO"].FldValue = "  " + rsmBstranaRow.b_oib;
      /*  3 */ Slog_5_Dict["RS5PRIM"].FldValue = rsmBstranaRow.b_prezime + " " + rsmBstranaRow.b_ime;
      /*  4 */ Slog_5_Dict["RS5GROP"].FldValue = rsmBstranaRow.b_opcRadCD;
      /*  5 */ Slog_5_Dict["RS5OSOB"].FldValue = rsmBstranaRow.b_rsOO + rsmBstranaRow.b_rsB;
      /*  6 */ Slog_5_Dict["RS5RAZD"].FldValue = rsmBstranaRow.b_rsOD + rsmBstranaRow.b_rsDO;
      /*  7 */ Slog_5_Dict["RS5IBRT"].SetDecimalFldValue_RSm(ZXC.ValOrZero_Decimal(rsmBstranaRow.b_Bruto   , 2));
      /*  8 */ Slog_5_Dict["RS5IOSN"].SetDecimalFldValue_RSm(ZXC.ValOrZero_Decimal(rsmBstranaRow.b_MioOsn  , 2));
      /*  9 */ Slog_5_Dict["RS5IST1"].SetDecimalFldValue_RSm(ZXC.ValOrZero_Decimal(rsmBstranaRow.b_Mio1stup, 2));
      /* 10 */ Slog_5_Dict["RS5IST2"].SetDecimalFldValue_RSm(ZXC.ValOrZero_Decimal(rsmBstranaRow.b_Mio2stup, 2));
      /* 11 */ Slog_5_Dict["RS5INET"].SetDecimalFldValue_RSm(ZXC.ValOrZero_Decimal(rsmBstranaRow.b_Netto   , 2));
      /* 12 */ Slog_5_Dict["RS5REZE"].FldValue = "";
      /* 13 */ Slog_5_Dict["RS5TIPS"].FldValue = "5";

   }

   #endregion Methods
   
   #endregion EXPORT DETAILS
   
}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptP_ObrazacID1 : VvPlacaReport
{
   public RptP_ObrazacID1(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter) : base(_reportDocument, externTblChooser, _reportName, _rptFilter)
   {
      IsForExport            = true;
      VvLet_TT_AHiUGiNOiK4_Only = true;
   }

   public override string ExportFileName 
   { 
      get 
      {
         return "ID-1_" + ZXC.CURR_prjkt_rec.Oib + "_01.xml";
      }
   }

   #region Xml Schema Validation

   public override bool ExecuteExportValidation(string fileName)
   {
      return ExecuteExportValidation_Base(/*fileName,*/ @"http://e-porezna.porezna-uprava.hr/obrasci/id1/v4-0", @"XSD\id1-obrazac_4_0.xsd");
   }

   #endregion Xml Schema Validation

   public override bool ExecuteExport(string fileName)
   {
      if(/*ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.MANDAR ||
         ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.TEM95*/false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "NELICENCIRANA UPOTREBA");
         return false;
      }


      #region Initialize XmlWriterSettings

      DS_Placa.ptransSumDataTable ptransSumTable = ds_PlacaReport.ptransSum;

      if(ptransSumTable.Count.IsZero()) throw new Exception("Nema se što exporitrati!");

      DS_Placa.ptransSumRow       ptransSum_FIRST_Row = ptransSumTable[0];

      XmlWriterSettings settings = new XmlWriterSettings();
      
      settings.Indent = true;
      settings.IndentChars = ("   ");

      #endregion Initialize XmlWriterSettings

      using(XmlWriter writer = XmlWriter.Create(fileName, settings))
      {
         #region Init Xml Document

         writer.WriteStartDocument();

         writer.WriteStartElement("Id1", @"http://e-porezna.porezna-uprava.hr/obrasci/id1/v4-0");

         writer.WriteAttributeString("storno",  "false");

         writer.WriteAttributeString("xmlns", "xsi",            null, @"http://www.w3.org/2001/XMLSchema-instance");
         //writer.WriteAttributeString("xsi",   "schemaLocation", null, @"http://e-porezna.porezna-uprava.hr/obrasci/id1/v1-0 id1-obrazac_1_0.xsd");

         #endregion Init Xml Document

         #region Write Header Data

         // 08.01.2013: 
         writer.WriteElementString("Identifikator", "1");

         writer.WriteElementString("IsplataUGodini", ptransSum_FIRST_Row.P_uYYYY.ToString());

         writer.WriteStartElement("Isplatitelj");  writer.WriteAttributeString("oib", ZXC.CURR_prjkt_rec.Oib);

            writer.WriteStartElement("KontaktOsoba");

               writer.WriteElementString("Ime"    , /*ZXC.CURR_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Ime);
               writer.WriteElementString("Prezime", /*ZXC.CURR_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Prezime);
                  writer.WriteStartElement("Telefoni");
                     writer.WriteElementString("Telefon", /*ZXC.CURR_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Tel1);
                     //if(ZXC.CURR_prjkt_rec.Tel2.NotEmpty())
                     {
                        writer.WriteElementString("Telefon", /*ZXC.CURR_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Tel1);
                     }
                  writer.WriteEndElement();
               writer.WriteElementString("Email", /*ZXC.CURR_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Email);

            writer.WriteEndElement(); // KontaktOsoba

         writer.WriteEndElement(); // Isplatitelj

         #endregion Write Header Data

         #region Transes

         writer.WriteStartElement("Obveznici");

         string opcCDin3;

         foreach(DS_Placa.ptransSumRow ptransSumRow in ptransSumTable)
         {
            if(ptransSumRow.P_opcCD.IsEmpty() || ptransSumRow.P_opcCD.Length < 3)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Djelatnik {0} ima krivu opcinu [{1}]", ptransSumRow.P_prezime, ptransSumRow.P_opcCD);
               opcCDin3 = "";
            }
            else
            {
               opcCDin3 = ptransSumRow.P_opcCD.Substring(0, 3);
            }

            writer.WriteStartElement("Obveznik"); writer.WriteAttributeString("oib", ptransSumRow.P_oib); writer.WriteAttributeString("oznGrOp", opcCDin3);

               writer.WriteElementString("Ime"    , ptransSumRow.P_ime);
               writer.WriteElementString("Prezime", ptransSumRow.P_prezime);

               writer.WriteStartElement("Primitak"        ); writer.WriteValue(ptransSumRow.P_TheBruto ); writer.WriteEndElement();
               writer.WriteStartElement("Izdatak"         ); writer.WriteValue(ptransSumRow.P_AHizdatak); writer.WriteEndElement();
               writer.WriteStartElement("IzdatakDoprinosa"); writer.WriteValue(ptransSumRow.P_DoprIz   ); writer.WriteEndElement();

               writer.WriteStartElement("PorezPrirez");

                  writer.WriteStartElement("Racuni");

                  if(ptransSumRow.P_PorPriAH.NotZero())
                  {
                     writer.WriteStartElement("Racun"); writer.WriteAttributeString("brojRacuna", "1465");

                     writer.WriteStartElement("Iznos"); writer.WriteValue(ptransSumRow.P_PorPriAH); writer.WriteEndElement();

                     writer.WriteEndElement(); // Racun 
                  }
                  if(ptransSumRow.P_PorPriNO.NotZero())
                  {
                     writer.WriteStartElement("Racun"); writer.WriteAttributeString("brojRacuna", "1457");

                     writer.WriteStartElement("Iznos"); writer.WriteValue(ptransSumRow.P_PorPriNO); writer.WriteEndElement();

                     writer.WriteEndElement(); // Racun 
                  }
                  if((ptransSumRow.P_PorPriUG + ptransSumRow.P_PorPriK4).NotZero())
                  {
                     writer.WriteStartElement("Racun"); writer.WriteAttributeString("brojRacuna", "1813");

                     writer.WriteStartElement("Iznos"); writer.WriteValue(ptransSumRow.P_PorPriUG + ptransSumRow.P_PorPriK4); writer.WriteEndElement();

                     writer.WriteEndElement(); // Racun 
                  }
                  // 19.1.2011: 
                  if(ptransSumRow.P_KrizPorUk.NotZero())
                  {
                     writer.WriteStartElement("Racun"); writer.WriteAttributeString("brojRacuna", "1902");

                     writer.WriteStartElement("Iznos"); writer.WriteValue(ptransSumRow.P_KrizPorUk); writer.WriteEndElement();

                     writer.WriteEndElement(); // Racun 
                  }

                  writer.WriteEndElement(); // Racuni 

                  writer.WriteStartElement("Ukupno"); writer.WriteValue(ptransSumRow.P_PorPrirez /* 19.1.2011: */ + ptransSumRow.P_KrizPorUk); writer.WriteEndElement();

               writer.WriteEndElement(); // PorezPrirez 

               writer.WriteStartElement("IsplaceniPrimitak"); writer.WriteValue(/* 19.1.2011: ptransSumRow.P_Netto*/ ptransSumRow.P_NettoAftKrp); writer.WriteEndElement();

            writer.WriteEndElement(); // Obveznik 
         }

         writer.WriteEndElement(); // Obveznici 

         #endregion Transes

         #region Sume (element 'Ukupno')

         DS_Placa.placaSumDataTable   placaSumTable   = ds_PlacaReport.placaSum;

         DS_Placa.placaSumRow placaSumRow_AH            = (DS_Placa.placaSumRow)placaSumTable.Rows[ 5];
         DS_Placa.placaSumRow placaSumRow_NO            = (DS_Placa.placaSumRow)placaSumTable.Rows[ 8];
         DS_Placa.placaSumRow placaSumRow_UG            = (DS_Placa.placaSumRow)placaSumTable.Rows[ 9];
         DS_Placa.placaSumRow placaSumRow_K4            = (DS_Placa.placaSumRow)placaSumTable.Rows[10];
         DS_Placa.placaSumRow placaSumRow_AHNOUGK4_ONLY = (DS_Placa.placaSumRow)placaSumTable.Rows[ 0];

         writer.WriteStartElement("Ukupno");

            writer.WriteStartElement("Primitak"        ); writer.WriteValue(placaSumRow_AHNOUGK4_ONLY.X_rTheBruto ); writer.WriteEndElement();
            writer.WriteStartElement("Izdatak"         ); writer.WriteValue(placaSumRow_AHNOUGK4_ONLY.X_rAHizdatak); writer.WriteEndElement();
            writer.WriteStartElement("IzdatakDoprinosa"); writer.WriteValue(placaSumRow_AHNOUGK4_ONLY.X_rDoprIz   ); writer.WriteEndElement();

            writer.WriteStartElement("PorezPrirez");

               writer.WriteStartElement("Racuni");

               if(placaSumRow_AH.X_rPorPrirez.NotZero())
               {
                  writer.WriteStartElement("Racun"); writer.WriteAttributeString("brojRacuna", "1465");

                  writer.WriteStartElement("Iznos"); writer.WriteValue(placaSumRow_AH.X_rPorPrirez); writer.WriteEndElement();

                  writer.WriteEndElement(); // Racun 
               }
               if(placaSumRow_NO.X_rPorPrirez.NotZero())
               {
                  writer.WriteStartElement("Racun"); writer.WriteAttributeString("brojRacuna", "1457");

                  writer.WriteStartElement("Iznos"); writer.WriteValue(placaSumRow_NO.X_rPorPrirez); writer.WriteEndElement();

                  writer.WriteEndElement(); // Racun 
               }
               if((placaSumRow_UG.X_rPorPrirez + placaSumRow_K4.X_rPorPrirez).NotZero())
               {
                  writer.WriteStartElement("Racun"); writer.WriteAttributeString("brojRacuna", "1813");

                  writer.WriteStartElement("Iznos"); writer.WriteValue(placaSumRow_UG.X_rPorPrirez + placaSumRow_K4.X_rPorPrirez); writer.WriteEndElement();

                  writer.WriteEndElement(); // Racun 
               }
               // 19.1.2011: 
               if(placaSumRow_AHNOUGK4_ONLY.X_rKrizPorUk.NotZero())
               {
                  writer.WriteStartElement("Racun"); writer.WriteAttributeString("brojRacuna", "1902");

                  writer.WriteStartElement("Iznos"); writer.WriteValue(placaSumRow_AHNOUGK4_ONLY.X_rKrizPorUk); writer.WriteEndElement();

                  writer.WriteEndElement(); // Racun 
               }

               writer.WriteEndElement(); // Racuni 

               writer.WriteStartElement("Ukupno"); writer.WriteValue(placaSumRow_AHNOUGK4_ONLY.X_rPorPrirez + /* 19.1.2011: */ + placaSumRow_AHNOUGK4_ONLY.X_rKrizPorUk); writer.WriteEndElement();

            writer.WriteEndElement(); // PorezPrirez 

            writer.WriteStartElement("IsplaceniPrimitak"); writer.WriteValue(/* 19.1.2011:placaSumRow_AHNOUG_ONLY.X_rNetto*/ placaSumRow_AHNOUGK4_ONLY.X_rNettoAftKrp); writer.WriteEndElement();

         writer.WriteEndElement(); // Ukupno 

         #endregion Sume (element 'Ukupno')

         #region Finish Xml Document

         writer.WriteEndElement();
         writer.WriteEndDocument();

         #endregion Finish Xml Document

      }
      return true;
   }

}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptP_ObrazacID : VvPlacaReport
{
   public RptP_ObrazacID(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter) : base(_reportDocument, externTblChooser, _reportName, _rptFilter)
   {
      IsForExport            = true;
      //VvLet_TT_AHiUGiNO_Only = true;
   }

   public override string ExportFileName 
   { 
      get 
      {
         return "ID_" + ZXC.CURR_prjkt_rec.Oib + "_01.xml";
      }
   }

   #region Xml Schema Validation

   public override bool ExecuteExportValidation(string fileName)
   {
      List<ZXC.VvXmlValidationData> valDataList = new List<ZXC.VvXmlValidationData>();

      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacID/v3-0" , @"XSD\ObrazacID-v3-0.xsd"           ));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacID/v3-0" , @"XSD\ObrazacIDtipovi-v3-0.xsd"     ));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\ObrazacIDmetapodaci-v3-0.xsd" ));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"         , @"XSD\MetapodaciTipovi-v2-0.xsd"    ));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/TemeljniTipovi/v2-1"     , @"XSD\TemeljniTipovi-v2-1.xsd"      ));

      return ExecuteExportValidation_Base(valDataList);
   }

   #endregion Xml Schema Validation
   
   public override bool ExecuteExport(string fileName)
   {
      #region Initialize XmlWriterSettings

      DS_Placa.placaSumRow        placaSumRow = ds_PlacaReport.placaSum[0];
      DS_Placa.opcinaSumDataTable opcinaSum   = ds_PlacaReport.opcinaSum;

      string ident = "   ";

      XmlWriterSettings settings = new XmlWriterSettings();
      
      settings.Indent = true;
      settings.IndentChars = ident;

      #endregion Initialize XmlWriterSettings

      using(XmlWriter writer = XmlWriter.Create(fileName, settings))
      {
         #region Init Xml Document

         writer.WriteStartDocument();

         writer.WriteStartElement("ObrazacID", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacID/v3-0");

         writer.WriteAttributeString("verzijaSheme", "3.0");

//         writer.WriteAttributeString("xmlns", "", null, @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacID/v3-0");

         #endregion Init Xml Document

         #region Metapodaci

         writer.WriteStartElement("Metapodaci", @"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0");
            //writer.WriteElementString("Naslov", @"http://purl.org/dc/elements/1.1/title", "ID obrazac");
            writer.WriteRaw("\n");
            writer.WriteRaw(ident + ident + "<Naslov dc=\"http://purl.org/dc/elements/1.1/title\">ID obrazac</Naslov>\n");
            writer.WriteRaw(ident + ident + "<Autor dc=\"http://purl.org/dc/elements/1.1/creator\">" + ZXC.CURR_prjkt_rec.Ime + " " + ZXC.CURR_prjkt_rec.Prezime + "</Autor>\n");
            writer.WriteRaw(ident + ident + "<Datum dc=\"http://purl.org/dc/elements/1.1/date\">" + DateTime.Now.ToString("s") + "</Datum>\n");
            writer.WriteRaw(ident + ident + "<Format dc=\"http://purl.org/dc/elements/1.1/format\">text/xml</Format>\n");
            writer.WriteRaw(ident + ident + "<Jezik dc=\"http://purl.org/dc/elements/1.1/language\">hr-HR</Jezik>\n");
            writer.WriteRaw(ident + ident + "<Identifikator dc=\"http://purl.org/dc/elements/1.1/identifier\">" + Guid.NewGuid().ToString("D")/*.ToUpper()*/ + "</Identifikator>\n");
            writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacID-v3-0</Uskladjenost>\n");
            writer.WriteRaw(ident + ident + "<Tip dc=\"http://purl.org/dc/elements/1.1/type\">Elektronički obrazac</Tip>\n");
            writer.WriteRaw(ident + ident + "<Adresant>Ministarstvo Financija, Porezna uprava, Zagreb</Adresant>\n");
            writer.WriteRaw(ident);
         writer.WriteEndElement(); // Metapodaci 

         #endregion Write Header Data

         #region Zaglavlje

         writer.WriteStartElement("Zaglavlje");

            writer.WriteElementString("PodrucniUred", ZXC.CURR_prjkt_rec.Opcina);
            writer.WriteElementString("Ispostava"   , /*ZXC.CURR_prjkt_rec.OpcCd*/ ZXC.CURR_prjkt_rec.PorezIspostCD);

            writer.WriteStartElement("Razdoblje");
               writer.WriteElementString("DatumOd", RptFilter.DatumOd.ToString("s").Substring(0, 10));
               writer.WriteElementString("DatumDo", RptFilter.DatumDo.ToString("s").Substring(0, 10));
            writer.WriteEndElement(); // Razdoblje 

            writer.WriteStartElement("PodnositeljZahtjeva");
               writer.WriteElementString("Naziv", ZXC.CURR_prjkt_rec.Naziv);
               writer.WriteElementString("OIB", ZXC.CURR_prjkt_rec.Oib);
               writer.WriteStartElement("Adresa");
                  writer.WriteElementString("Mjesto", ZXC.CURR_prjkt_rec.Grad);
                  writer.WriteElementString("Ulica", ZXC.CURR_prjkt_rec.UlicaBezBroja_1);
                  writer.WriteElementString("Broj", ZXC.CURR_prjkt_rec.UlicniBroj_1);
               writer.WriteEndElement(); // Adresa 
            writer.WriteEndElement(); // PodnositeljZahtjeva 

            writer.WriteElementString("Identifikator", placaSumRow.X_rSm_ID);

         writer.WriteEndElement(); // Zaglavlje 

         #endregion Zaglavlje

         #region Tijelo

         writer.WriteStartElement("Tijelo");

         #region Write to IsplaceniPrimiciIObracunPoreza

            writer.WriteStartElement("IsplaceniPrimiciIObracunPoreza");
               writer.WriteElementString("Podatak100", placaSumRow.X_rTheBruto    .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak200", placaSumRow.X_rDoprIz      .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak210", placaSumRow.X_rMio1stup    .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak220", placaSumRow.X_rMio2stup    .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak230", 0.00M                      .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak300", placaSumRow.X_rDohodak     .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak400", placaSumRow.X_rOdbitak     .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak500", placaSumRow.X_rPorOsnAll   .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak600", placaSumRow.X_rPorPrirez   .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak610", placaSumRow.X_rPorezAll    .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak620", placaSumRow.X_rPrirez      .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak700", placaSumRow.X_rPorPrirez   .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak800", placaSumRow.X_prsnCount_Mio.ToString                   ());
            writer.WriteEndElement();

         #endregion Write to IsplaceniPrimiciIObracunPoreza

         #region Write to DoprinosiUkupno

            writer.WriteStartElement("DoprinosiUkupno");
               writer.WriteElementString("Podatak110", placaSumRow.X_rMio1stup  .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak120", placaSumRow.X_rMio1stupNa.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak210", placaSumRow.X_rMio2stup  .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak220", placaSumRow.X_rMio2stupNa.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak310", placaSumRow.X_rZdrNa     .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak320", placaSumRow.X_rZorNa     .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak330", placaSumRow.X_rZpiUk     .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak410", placaSumRow.X_rZapNa     .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak420", placaSumRow.X_rZapII     .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Podatak500", placaSumRow.X_personCount.ToString                   ());
            writer.WriteEndElement();

         #endregion Write to DoprinosiUkupno

         #region Write to ObracunatiPorezi

            writer.WriteStartElement("ObracunatiPorezi");

            foreach(var opcina in opcinaSum)
            {
               writer.WriteStartElement("ObracunatiPorez");
                  writer.WriteElementString("Sifra"  , opcina.opcinaCD.Substring(0, 3));
                  writer.WriteElementString("Poreza" , opcina.porez    .ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("Prireza", opcina.prirez   .ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("Ukupno" , opcina.porPrirez.ToStringVv_NoGroup_ForceDot());
               writer.WriteEndElement();
            }

            writer.WriteEndElement();

         #endregion Write to ObracunatiPorezi

         #region Ukupno

            writer.WriteStartElement("Ukupno");
               writer.WriteElementString("Poreza" , opcinaSum.Sum(opc => opc.porez)    .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Prireza", opcinaSum.Sum(opc => opc.prirez)   .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Ukupno" , opcinaSum.Sum(opc => opc.porPrirez).ToStringVv_NoGroup_ForceDot());
            writer.WriteEndElement();

         #endregion Ukupno

         writer.WriteEndElement(); // Tijelo 

         #endregion Tijelo

         #region Finish Xml Document

         writer.WriteEndElement();
         writer.WriteEndDocument();

         #endregion Finish Xml Document

      }
      return true;
   }

}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptP_ObrazacIDD : VvPlacaReport
{
   public RptP_ObrazacIDD(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter) : base(_reportDocument, externTblChooser, _reportName, _rptFilter)
   {
      IsForExport            = true;
      VvLet_TT_AHiUGiNOiK4_Only = true;
   }

   public override string ExportFileName 
   { 
      get 
      {
         return "IDD_" + ZXC.CURR_prjkt_rec.Oib + "_01.xml";
      }
   }

   #region Xml Schema Validation

   public override bool ExecuteExportValidation(string fileName)
   {
      List<ZXC.VvXmlValidationData> valDataList = new List<ZXC.VvXmlValidationData>();

      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacIDD/v1-0" , @"XSD\ObrazacIDD-v1-0.xsd"           ));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacIDD/v1-0" , @"XSD\ObrazacIDDtipovi-v1-0.xsd"     ));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"          , @"XSD\ObrazacIDDmetapodaci-v1-0.xsd" ));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"          , @"XSD\MetapodaciTipovi-v2-0.xsd"     ));
      valDataList.Add(new ZXC.VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/TemeljniTipovi/v2-1"      , @"XSD\TemeljniTipovi-v2-1.xsd"       ));

      return ExecuteExportValidation_Base(valDataList);
   }

   #endregion Xml Schema Validation
   
   public override bool ExecuteExport(string fileName)
   {
      #region Initialize XmlWriterSettings

      DS_Placa.placaSumRow placaSumRow0 = ds_PlacaReport.placaSum[0];
      DS_Placa.placaSumRow placaSumRow3 = ds_PlacaReport.placaSum[3];
      DS_Placa.placaSumRow placaSumRow4 = ds_PlacaReport.placaSum[4];
      DS_Placa.placaSumRow placaSumRow5 = ds_PlacaReport.placaSum[5];

      string ident = "   ";

      XmlWriterSettings settings = new XmlWriterSettings();
      
      settings.Indent = true;
      settings.IndentChars = ident;

      #endregion Initialize XmlWriterSettings

      using(XmlWriter writer = XmlWriter.Create(fileName, settings))
      {
         #region Init Xml Document

         writer.WriteStartDocument();

         writer.WriteStartElement("ObrazacIDD", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacIDD/v1-0");

         writer.WriteAttributeString("verzijaSheme", "1.0");

//         writer.WriteAttributeString("xmlns", "", null, @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacIDD/v1-0");

         #endregion Init Xml Document

         #region Metapodaci

         writer.WriteStartElement("Metapodaci", @"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0");
            //writer.WriteElementString("Naslov", @"http://purl.org/dc/elements/1.1/title", "IDD obrazac");
            writer.WriteRaw("\n");
            writer.WriteRaw(ident + ident + "<Naslov dc=\"http://purl.org/dc/elements/1.1/title\">IDD obrazac</Naslov>\n");
            writer.WriteRaw(ident + ident + "<Autor dc=\"http://purl.org/dc/elements/1.1/creator\">" + ZXC.CURR_prjkt_rec.Ime + " " + ZXC.CURR_prjkt_rec.Prezime + "</Autor>\n");
            writer.WriteRaw(ident + ident + "<Datum dc=\"http://purl.org/dc/elements/1.1/date\">" + DateTime.Now.ToString("s") + "</Datum>\n");
            writer.WriteRaw(ident + ident + "<Format dc=\"http://purl.org/dc/elements/1.1/format\">text/xml</Format>\n");
            writer.WriteRaw(ident + ident + "<Jezik dc=\"http://purl.org/dc/elements/1.1/language\">hr-HR</Jezik>\n");
            writer.WriteRaw(ident + ident + "<Identifikator dc=\"http://purl.org/dc/elements/1.1/identifier\">" + Guid.NewGuid().ToString("D")/*.ToUpper()*/ + "</Identifikator>\n");
            writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacIDD-v1-0</Uskladjenost>\n");
            writer.WriteRaw(ident + ident + "<Tip dc=\"http://purl.org/dc/elements/1.1/type\">Elektronički obrazac</Tip>\n");
            writer.WriteRaw(ident + ident + "<Adresant>Ministarstvo Financija, Porezna uprava, Zagreb</Adresant>\n");
            writer.WriteRaw(ident);
         writer.WriteEndElement(); // Metapodaci 

         #endregion Write Header Data

         #region Zaglavlje

         writer.WriteStartElement("Zaglavlje");

            writer.WriteStartElement("PodnositeljZahtjeva");
               writer.WriteElementString("Naziv", ZXC.CURR_prjkt_rec.Naziv);
               writer.WriteElementString("OIB", ZXC.CURR_prjkt_rec.Oib);
               writer.WriteStartElement("Adresa");
                  writer.WriteElementString("Mjesto", ZXC.CURR_prjkt_rec.Grad);
                  writer.WriteElementString("Ulica", ZXC.CURR_prjkt_rec.UlicaBezBroja_1);
                  writer.WriteElementString("Broj", ZXC.CURR_prjkt_rec.UlicniBroj_1);
               writer.WriteEndElement(); // Adresa 
            writer.WriteEndElement(); // PodnositeljZahtjeva 

            writer.WriteStartElement("Razdoblje");
               writer.WriteElementString("DatumOd", RptFilter.DatumOd.ToString("s").Substring(0, 10));
               writer.WriteElementString("DatumDo", RptFilter.DatumDo.ToString("s").Substring(0, 10));
            writer.WriteEndElement(); // Razdoblje 

         writer.WriteEndElement(); // Zaglavlje 

         #endregion Zaglavlje

         #region Tijelo

         writer.WriteStartElement("Tijelo");

            writer.WriteStartElement("Podatak010");
               writer.WriteElementString("Iznos3", placaSumRow3.X_personCount.ToString());
               writer.WriteElementString("Iznos4", placaSumRow4.X_personCount.ToString());
               writer.WriteElementString("Iznos5", placaSumRow5.X_personCount.ToString());
               writer.WriteElementString("Iznos6", 0 /* fuse */              .ToString());
               writer.WriteElementString("Iznos7", 0 /* fuse */              .ToString());
               writer.WriteElementString("Iznos8", 0 /* fuse */              .ToString());
               writer.WriteElementString("Iznos9", placaSumRow0.X_personCount.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Podatak020");
               writer.WriteElementString("Iznos3", placaSumRow3.X_rTheBruto.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos4", placaSumRow4.X_rTheBruto.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos5", placaSumRow5.X_rTheBruto.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos6", 0M /* fuse */           .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos7", 0M /* fuse */           .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos8", 0M /* fuse */           .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos9", placaSumRow0.X_rTheBruto.ToStringVv_NoGroup_ForceDot());
            writer.WriteEndElement();

            writer.WriteStartElement("Podatak030");
               writer.WriteElementString("Iznos5", placaSumRow5.X_rAHizdatak.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos6", 0M /* fuse */            .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos7", 0M /* fuse */            .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos9", placaSumRow0.X_rAHizdatak.ToStringVv_NoGroup_ForceDot());
            writer.WriteEndElement();

            writer.WriteStartElement("Podatak040");
               writer.WriteElementString("Iznos3", placaSumRow3.X_rMioOsn.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos7", 0M /* fuse */         .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos8", 0M /* fuse */         .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos9", placaSumRow0.X_rMioOsn.ToStringVv_NoGroup_ForceDot());
            writer.WriteEndElement();

            writer.WriteStartElement("Podatak051");
               writer.WriteElementString("Iznos3", placaSumRow3.X_rMio1stup.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos7", 0M /* fuse */           .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos9", placaSumRow0.X_rMio1stup.ToStringVv_NoGroup_ForceDot());
            writer.WriteEndElement();

            writer.WriteStartElement("Podatak052");
               writer.WriteElementString("Iznos8", 0M /* fuse */           .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos9", 0M                      .ToStringVv_NoGroup_ForceDot());
            writer.WriteEndElement();

            writer.WriteStartElement("Podatak053");
               writer.WriteElementString("Iznos3", placaSumRow3.X_rMio2stup.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos7", 0M /* fuse */           .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos9", placaSumRow0.X_rMio2stup.ToStringVv_NoGroup_ForceDot());
            writer.WriteEndElement();

            writer.WriteStartElement("Podatak054");
               writer.WriteElementString("Iznos3", placaSumRow3.X_rZdrNa.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos7", 0M /* fuse */        .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos9", placaSumRow0.X_rZdrNa.ToStringVv_NoGroup_ForceDot());
            writer.WriteEndElement();

            writer.WriteStartElement("Podatak055");
               writer.WriteElementString("Iznos8", 0M /* fuse */           .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos9", 0M                      .ToStringVv_NoGroup_ForceDot());
            writer.WriteEndElement();

            writer.WriteStartElement("Podatak056");
               writer.WriteElementString("Iznos3", placaSumRow3.X_rZpiUk.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos4", placaSumRow4.X_rZpiUk.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos5", placaSumRow5.X_rZpiUk.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos6", 0M /* fuse */        .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos8", 0M /* fuse */        .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos9", placaSumRow0.X_rZpiUk.ToStringVv_NoGroup_ForceDot());
            writer.WriteEndElement();

            writer.WriteStartElement("Podatak060");
               writer.WriteElementString("Iznos3", placaSumRow3.X_rDoprIz.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos7", 0M /* fuse */         .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos9", placaSumRow0.X_rDoprIz.ToStringVv_NoGroup_ForceDot());
            writer.WriteEndElement();

            writer.WriteStartElement("Podatak070");
               writer.WriteElementString("Iznos3", placaSumRow3.X_rPorOsnAll.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos4", placaSumRow4.X_rPorOsnAll.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos5", placaSumRow5.X_rPorOsnAll.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos6", 0M /* fuse */            .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos7", 0M /* fuse */            .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos8", 0M /* fuse */            .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos9", placaSumRow0.X_rPorOsnAll.ToStringVv_NoGroup_ForceDot());
            writer.WriteEndElement();

            writer.WriteStartElement("Podatak080");
               writer.WriteElementString("Iznos3", placaSumRow3.X_rPorezAll.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos4", placaSumRow4.X_rPorezAll.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos5", placaSumRow5.X_rPorezAll.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos6", 0M /* fuse */           .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos7", 0M /* fuse */           .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos8", 0M /* fuse */           .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos9", placaSumRow0.X_rPorezAll.ToStringVv_NoGroup_ForceDot());
            writer.WriteEndElement();

            writer.WriteStartElement("Podatak090");
               writer.WriteElementString("Iznos3", placaSumRow3.X_rPrirez.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos4", placaSumRow4.X_rPrirez.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos5", placaSumRow5.X_rPrirez.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos6", 0M /* fuse */         .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos7", 0M /* fuse */         .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos8", 0M /* fuse */         .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos9", placaSumRow0.X_rPrirez.ToStringVv_NoGroup_ForceDot());
            writer.WriteEndElement();

            writer.WriteStartElement("Podatak100");
               writer.WriteElementString("Iznos3", placaSumRow3.X_rPorPrirez.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos4", placaSumRow4.X_rPorPrirez.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos5", placaSumRow5.X_rPorPrirez.ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos6", 0M /* fuse */            .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos7", 0M /* fuse */            .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos8", 0M /* fuse */            .ToStringVv_NoGroup_ForceDot());
               writer.WriteElementString("Iznos9", placaSumRow0.X_rPorPrirez.ToStringVv_NoGroup_ForceDot());
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

}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 
public class RptP_IP_Kartica : VvPlacaReport
{
   public RptP_IP_Kartica(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter) : base(_reportDocument, externTblChooser, _reportName, _rptFilter)
   {
      IsForExport         = true;
      VvLet_TT_RRiPP_Only = true;
   }

   public override string ExportFileName
   {
      get
      {
         return "IP_" + ZXC.CURR_prjkt_rec.Oib + "_01.xml";
      }
   }

   #region Xml Schema Validation

   public override bool ExecuteExportValidation(string fileName)
   {
      return ExecuteExportValidation_Base(/*fileName,*/ @"http://e-porezna.porezna-uprava.hr/obrasci/ip/v4-0", @"XSD\ip-obrazac_4_0.xsd");
   }

   #endregion Xml Schema Validation

   public override bool ExecuteExport(string fileName)
   {
      if(/*ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.MANDAR ||
         ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.TEM95*/false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "NELICENCIRANA UPOTREBA");
         return false;
      }

      #region Initialize XmlWriterSettings

      DS_Placa.ptransSumDataTable ptransSumTable = ds_PlacaReport.ptransSum;

      if(ptransSumTable.Count.IsZero()) throw new Exception("Nema se što exporitrati!");

      DS_Placa.ptransSumRow ptransSum_FIRST_Row = ptransSumTable[0];

      XmlWriterSettings     settings = new XmlWriterSettings();
      
      settings.Indent = true;
      settings.IndentChars = ("   ");
      //settings.Encoding = Encoding.UTF8;

      #endregion Initialize XmlWriterSettings

      using(XmlWriter writer = XmlWriter.Create(fileName, settings))
      {
         #region Init Xml Document

         //writer.WriteStartDocument();
         writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'"); // 17.01.2013: ipValidator zajebava jer WriteStartDocument da 'utf-8' a on oce 'UTF-8'. Sa WriteProcessingInstruction gadjas onda tocno!  

         writer.WriteStartElement("ip", "Ip", @"http://e-porezna.porezna-uprava.hr/obrasci/ip/v4-0");

         writer.WriteAttributeString("storno",  "false");

         #endregion Init Xml Document

         #region Write Header Data

         writer.WriteElementString("ip", "IsplataUGodini", null, ptransSum_FIRST_Row.P_uYYYY.ToString());

         writer.WriteStartElement("ip", "Poslodavac", null); writer.WriteAttributeString("oib", ZXC.CURR_prjkt_rec.Oib);

            writer.WriteStartElement("ip", "KontaktOsoba", null);

            writer.WriteElementString("ip", "Ime", null, /*ZXC.ROOT_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Ime);
               writer.WriteElementString("ip", "Prezime", null, /*ZXC.ROOT_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Prezime);
                  writer.WriteStartElement("ip", "Telefoni", null);
                     writer.WriteElementString("ip", "Telefon", null, /*ZXC.ROOT_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Tel1);
                     //if(/*ZXC.ROOT_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Tel2.NotEmpty())
                     {
                        writer.WriteElementString("ip", "Telefon", null, /*ZXC.ROOT_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Tel1);
                     }
                  writer.WriteEndElement();
               writer.WriteElementString("ip", "Email", null, /*ZXC.ROOT_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Email);

            writer.WriteEndElement(); // KontaktOsoba

         writer.WriteEndElement(); // Isplatitelj

         #endregion Write Header Data

         #region Transes

         writer.WriteStartElement("ip", "IpObrasci", null);

         var personGroups = ptransSumTable.GroupBy(ptrSumRow => ptrSumRow.P_personCD).Select(personPtransSet => new
         {
            personPtransSet,
            personCD    = personPtransSet.Key,
            
            personOib   = personPtransSet.FirstOrDefault(pps => pps./*P_opcCD*/P_oib.NotEmpty()).P_oib,
            uYYYY       = personPtransSet.FirstOrDefault(pps => pps./*P_opcCD*/P_oib.NotEmpty()).P_uYYYY,
            ipIdentif   = personPtransSet.FirstOrDefault(pps => pps./*P_opcCD*/P_oib.NotEmpty()).P_ipIdentif,

            IsplPlMir   = personPtransSet.Sum(pps => pps.P_TheBruto),
            UplDopr     = personPtransSet.Sum(pps => pps.P_DoprIz),
            UplPremOsig = personPtransSet.Sum(pps => pps.P_Premije),
            Dohodak     = personPtransSet.Sum(pps => pps.P_Dohodak),
            OsobOdb     = personPtransSet.Sum(pps => pps.P_Odbitak),
            PorOsn      = personPtransSet.Sum(pps => pps.P_PorOsnAll),
            UplPorPrir  = personPtransSet.Sum(pps => pps.P_PorPrirez),
            PosPorez    = personPtransSet.Sum(pps => pps.P_KrizPorUk),
            NetoIspl    = personPtransSet.Sum(pps => pps.P_NettoAftKrp)
         });

         foreach(var person in personGroups)
         {
            writer.WriteStartElement("ip", "IpObrazac", null); writer.WriteAttributeString("identifikator", person.ipIdentif.IsEmpty() ? "" : person.ipIdentif); writer.WriteAttributeString("isplataZaGodinu", person.uYYYY.ToString());
               writer.WriteStartElement("ip", "Obveznik", null);
            writer.WriteStartElement("ip", "Radnik", null); writer.WriteAttributeString("oib", (person.personOib.IsEmpty() ? "" : person.personOib));
                     writer.WriteElementString("ip", "VrPrimanja", null, "PL");
                  writer.WriteEndElement(); // Radnik 
               writer.WriteEndElement(); // Obveznik 

               writer.WriteStartElement("ip", "Mjeseci", null);

            foreach(var personsMonthlyPtransSum in person.personPtransSet)
            {
               if(personsMonthlyPtransSum.P_TheBruto.IsZero()) continue;

                  writer.WriteStartElement("ip", "Mjesec", null); writer.WriteAttributeString("mjIspl", personsMonthlyPtransSum.P_uMM.ToString("00")); writer.WriteAttributeString("sifGrOp", personsMonthlyPtransSum.P_opcCD.Length < 3 ? "" : personsMonthlyPtransSum.P_opcCD.Substring(0, 3));

                     writer.WriteStartElement("ip", "IsplPlMir",   null); writer.WriteValue(personsMonthlyPtransSum.P_TheBruto)     ; writer.WriteEndElement();
                     writer.WriteStartElement("ip", "UplDopr",     null); writer.WriteValue(personsMonthlyPtransSum.P_DoprIz)       ; writer.WriteEndElement();
                     writer.WriteStartElement("ip", "UplPremOsig", null); writer.WriteValue(personsMonthlyPtransSum.P_Premije)      ; writer.WriteEndElement();
                     writer.WriteStartElement("ip", "Dohodak",     null); writer.WriteValue(personsMonthlyPtransSum.P_Dohodak)      ; writer.WriteEndElement();
                     writer.WriteStartElement("ip", "OsobOdb",     null); writer.WriteValue(personsMonthlyPtransSum.P_Odbitak)      ; writer.WriteEndElement();
                     writer.WriteStartElement("ip", "PorOsn",      null); writer.WriteValue(personsMonthlyPtransSum.P_PorOsnAll)    ; writer.WriteEndElement();
                     writer.WriteStartElement("ip", "UplPorPrir",  null); writer.WriteValue(personsMonthlyPtransSum.P_PorPrirez)    ; writer.WriteEndElement();
                     writer.WriteStartElement("ip", "PosPorez",    null); writer.WriteValue(personsMonthlyPtransSum.P_KrizPorUk)    ; writer.WriteEndElement();
                     writer.WriteStartElement("ip", "NetoIspl",    null); writer.WriteValue(personsMonthlyPtransSum.P_NettoAftKrp)  ; writer.WriteEndElement();

                  writer.WriteEndElement(); // Mjesec 

            } // foreach(var personsMonthlyPtransSum in person.personPtransSet) 

               writer.WriteEndElement(); // Mjeseci 

               writer.WriteStartElement("ip", "Ukupno", null);

                  writer.WriteStartElement("ip", "IsplPlMir",   null); writer.WriteValue(person.IsplPlMir)  ; writer.WriteEndElement();
                  writer.WriteStartElement("ip", "UplDopr",     null); writer.WriteValue(person.UplDopr)    ; writer.WriteEndElement();
                  writer.WriteStartElement("ip", "UplPremOsig", null); writer.WriteValue(person.UplPremOsig); writer.WriteEndElement();
                  writer.WriteStartElement("ip", "Dohodak",     null); writer.WriteValue(person.Dohodak)    ; writer.WriteEndElement();
                  writer.WriteStartElement("ip", "OsobOdb",     null); writer.WriteValue(person.OsobOdb)    ; writer.WriteEndElement();
                  writer.WriteStartElement("ip", "PorOsn",      null); writer.WriteValue(person.PorOsn)     ; writer.WriteEndElement();
                  writer.WriteStartElement("ip", "UplPorPrir",  null); writer.WriteValue(person.UplPorPrir) ; writer.WriteEndElement();
                  writer.WriteStartElement("ip", "PosPorez",    null); writer.WriteValue(person.PosPorez)   ; writer.WriteEndElement();
                  writer.WriteStartElement("ip", "NetoIspl",    null); writer.WriteValue(person.NetoIspl)   ; writer.WriteEndElement();

               writer.WriteEndElement(); // Ukupno 

            writer.WriteEndElement(); // IpObrazac 

         }

         writer.WriteEndElement(); // IpObrasci 

         #endregion Transes

         #region Finish Xml Document

         writer.WriteEndElement();
         writer.WriteEndDocument();

         #endregion Finish Xml Document

      }

      return true;
   }

}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptP_Virmani : VvPlacaReport
{
   protected ZbrojniNalogZaPrijenos TheZNP;
   protected DateTime               ZnpDate;

   /*protected*/public ZXC.ZNP_Kind ZNPkind = ZXC.ZNP_Kind.Classic;

   public static bool IsNewOrder062012 { get { return true; } }

   public RptP_Virmani(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter) : base(_reportDocument, externTblChooser, _reportName, _rptFilter)
   {
      IsForExport = true;

      TheZNP = null;
   }

   public override string ExportFileName 
   { 
      get 
      {
         string znpDate4fName, fNamePreffix;

         if(TheVirmanList[0].DatePodnos != DateTime.MinValue) ZnpDate = TheVirmanList[0].DatePodnos;
         else                                                 ZnpDate = DateTime.Today;

         if(/*ZnpDate < ZXC.The2013Year*/false) // OLD way 
         {
            znpDate4fName = ZXC.ValOrEmpty_ddMMyyDateTime_AsText(ZnpDate);

            //if(ZXC.CURR_prjkt_rec.Ziro1By.StartsWith("ZAGREB", StringComparison.OrdinalIgnoreCase)) fNamePreffix = "OB";
            //else                                                                                 
            fNamePreffix = "MM";

            return fNamePreffix + znpDate4fName + ".ZAP";
         }
         else // new way
         {
            znpDate4fName = ZXC.ValOrEmpty_YyyyMMddDateTime_AsText(ZnpDate);

            fNamePreffix = "UN";

            return fNamePreffix + znpDate4fName + ".txt";
         }
      } 
   }

   public override bool ExecuteExport(string fullPathFileName)
   {
      TheZNP = new ZbrojniNalogZaPrijenos(fullPathFileName, ExportFileName, ZnpDate, TheVirmanList, ZNPkind); 
      
      foreach(VirmanStruct virman_rec in TheVirmanList)
      {
         TheZNP.SetAndDumpZnpLine(virman_rec, ZNPkind);
      }

      TheZNP.CloseZNP();

      if(ZbrojniNalogZaPrijenos.IsNewZNP2013Format == true) 
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, 
            "Gotovo." + 
            "\n\nKreirao datoteku: "      + fullPathFileName + 
            "\n\nBroj naloga: "           + TheVirmanList.Count + 
            "\n\nUkupan iznos placanja: " + TheVirmanList.Sum(virman_rec => virman_rec.Money).ToStringVv() + " Kn" +

            "\n\n\n\nZNP Check Data:" + 
            "\n\nFile Name: "             + TheZNP.FileName + 
            "\n\nTransactions: "          +  ZXC.ValOrZero_Int    (TheZNP.Dict301["S301BRNALUK" ].FldValue) +
            "\n\nUkupan iznos placanja: " + (ZXC.ValOrZero_Decimal(TheZNP.Dict301["S301IZNNALUK"].FldValue, 2) / 100.00M).ToStringVv());
      }
      else
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, 
            "Gotovo." + 
            "\n\nKreirao datoteku: "      + fullPathFileName + 
            "\n\nBroj naloga: "           + TheVirmanList.Count + 
            "\n\nUkupan iznos placanja: " + TheVirmanList.Sum(virman_rec => virman_rec.Money).ToStringVv() + " Kn" +

            "\n\n\n\nZNP Check Data:" + 
            "\n\nFile Name: "             + TheZNP.FileName + 
            "\n\nTransactions: "          +  ZXC.ValOrZero_Int    (TheZNP.LeadDict["ZN9BRNAL"].FldValue) +
            "\n\nUkupan iznos placanja: " + (ZXC.ValOrZero_Decimal(TheZNP.LeadDict["ZN9SVOTA"].FldValue, 2) / 100.00M).ToStringVv());
      }
      return true;
   }

}

public class RptP_SEPA : RptP_Virmani
{
   public RptP_SEPA(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter) : 
               base(               _reportDocument,                                 externTblChooser,        _reportName,                    _rptFilter)
   {
      // TODO: !!!!!!!!! gasi ovo hardkodirano, a ako ispadne da treba, izvadi sa GetFields sa RptFilterUC-a 
     // _rptFilter.VirmanGroup = ZXC.VirmanBtchBookgKind.ALL;

      ZNPkind = ZXC.ZNP_Kind.SEPA_Placa;

      #region IsBtchBookgExtraFile decision 

      IsBtchBookgExtraFile = _rptFilter == null ? false : (

         ZNPkind == ZXC.ZNP_Kind.SEPA_Placa
         
         &&

         ZXC.CURR_prjkt_rec.IsBtchBookg == true

         &&

       //                       _rptFilter.Ziro1 .SubstringSafe(4).StartsWith("2360000") // HRxy2360000... je ZABA 
         (
            ZXC.GetIBANfromOldZiro(_rptFilter.Ziro1).SubstringSafe(4).StartsWith("2360000") // HRxy2360000... je ZABA 
            ||
            ZXC.GetIBANfromOldZiro(_rptFilter.Ziro1).SubstringSafe(4).StartsWith("2484008") // HRxy2484008... je RBA  
         )
      );

      #endregion IsBtchBookgExtraFile decision

   }

   public bool IsBtchBookgExtraFile { get; set; }

   public static bool IsFilterWellFormed(PlacaReportUC reportUC)
   {
      bool OK = true;
    //VvRpt_Placa_Filter filter = reportUC.TheRptFilter;
    //
    //if(filter.BankaCd.IsZero())
    //{
    //   ZXC.aim_emsg("Molim, zadajte BANKU.");
    //   return false;
    //}
      return (OK);
   }

   public override string ExportFileName 
   { 
      get 
      {
         string yyyymmdd, fNamePreffix, fNameSuffix, sep = ".";

         if(TheVirmanList[0].DatePodnos != DateTime.MinValue) ZnpDate = TheVirmanList[0].DatePodnos;
         else                                                 ZnpDate = DateTime.Today;

         yyyymmdd = ZXC.ValOrEmpty_YyyyMMddDateTime_AsText(ZnpDate);

         fNamePreffix = "UN";
         fNameSuffix  = RptFilter.DokNumChecked ? RptFilter.DokNum.ToString("0000") : ((int)RptFilter.VirmanGroup + 1).ToString("0000") /*"0001"*/;

         return fNamePreffix + sep + ZXC.CURR_prjkt_rec.Oib + sep + yyyymmdd + sep + fNameSuffix + sep + "xml";
      } 
   }

   #region Xml Schema Validation

   public override bool ExecuteExportValidation(string fileName)
   {
      // SEPA_PAIN_001_001_03_to_PAIN_001_001_09

    //return ExecuteExportValidationSEPA_001_001_003(fileName, this);
      return ExecuteExportValidationSEPA_001_001_009(fileName, this);
   }

   public static bool ExecuteExportValidationSEPA_001_001_003(string fileName, VvReport theReport)
   {
      List<ZXC.VvXmlValidationData> valDataList = new List<ZXC.VvXmlValidationData>();

      // 20.05.2019: 
    //valDataList.Add(new ZXC.VvXmlValidationData(@"urn:iso:std:iso:20022:tech:xsd:scthr:pain.001.001.03", @"XSD\sepa.hr.pain.001.001.03_07052015.xsd"));
      valDataList.Add(new ZXC.VvXmlValidationData(@"urn:iso:std:iso:20022:tech:xsd:scthr:pain.001.001.03", @"XSD\sepa.hr.pain.001.001.03.NOVA.xsd"    ));
    //valDataList.Add(new ZXC.VvXmlValidationData(@"urn:iso:std:iso:20022:tech:xsd:scthr:pain.001.001.03", @"XSD\sepa.hr.pain.001.001.09_11-2023-2.xsd"));

    //valDataList.Add(new VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacDI/v1-0", @"XSD\ObrazacDI-v1-0.xsd"));
    //valDataList.Add(new VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacDI/v1-0", @"XSD\ObrazacDItipovi-v1-0.xsd"));
    //valDataList.Add(new VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"        , @"XSD\ObrazacDImetapodaci-v1-0.xsd"));
    //valDataList.Add(new VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"        , @"XSD\MetapodaciTipovi-v2-0.xsd"));
    //valDataList.Add(new VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/TemeljniTipovi/v2-1"    , @"XSD\TemeljniTipovi-v2-1.xsd"  ));

      return theReport.ExecuteExportValidation_Base(valDataList);
   }

   public static bool ExecuteExportValidationSEPA_001_001_009(string fileName, VvReport theReport)
   {
      List<ZXC.VvXmlValidationData> valDataList = new List<ZXC.VvXmlValidationData>();

      // 20.05.2019: 
    //valDataList.Add(new ZXC.VvXmlValidationData(@"urn:iso:std:iso:20022:tech:xsd:scthr:pain.001.001.03", @"XSD\sepa.hr.pain.001.001.03_07052015.xsd"));
    //valDataList.Add(new ZXC.VvXmlValidationData(@"urn:iso:std:iso:20022:tech:xsd:scthr:pain.001.001.03", @"XSD\sepa.hr.pain.001.001.03.NOVA.xsd"    ));
      valDataList.Add(new ZXC.VvXmlValidationData(@"urn:iso:std:iso:20022:tech:xsd:scthr:pain.001.001.09", @"XSD\sepa.hr.pain.001.001.09_11-2023-2.xsd"));

    //valDataList.Add(new VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacDI/v1-0", @"XSD\ObrazacDI-v1-0.xsd"));
    //valDataList.Add(new VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacDI/v1-0", @"XSD\ObrazacDItipovi-v1-0.xsd"));
    //valDataList.Add(new VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"        , @"XSD\ObrazacDImetapodaci-v1-0.xsd"));
    //valDataList.Add(new VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"        , @"XSD\MetapodaciTipovi-v2-0.xsd"));
    //valDataList.Add(new VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/TemeljniTipovi/v2-1"    , @"XSD\TemeljniTipovi-v2-1.xsd"  ));

      return theReport.ExecuteExportValidation_Base(valDataList);
   }

   #endregion Xml Schema Validation

   public override bool ExecuteExport(string fullPathFileName)
   {
      // SEPA_PAIN_001_001_03_to_PAIN_001_001_09

    //return ExecuteExportSEPA_001_001_03(fullPathFileName, TheVirmanList, ZnpDate, RptFilter.VirmanGroup, true);
      return ExecuteExportSEPA_001_001_09(fullPathFileName, TheVirmanList, ZnpDate, RptFilter.VirmanGroup, true);
   }

#if SEPA_001_001_03
   public static bool ExecuteExportSEPA_001_001_03(string fullPathFileName, List<VirmanStruct> _theVirmanList, DateTime _znpDate, ZXC.VirmanBtchBookgKind _virmanGroup, bool isPlaca) // VOILA 
   {
      // 16.10.2023: tu si stao. sada treba ovaj Document zamijeniti sa Document_PAIN_001_001_09
      // pa gore ugasiti nepotreban 'using PAIN_001_001_03;'                                    
      // pa u ExecuteExportValidationSEPA ugasiti                                               
      // sepa.hr.pain.001.001.03.NOVA                                                           
      // a upaliti                                                                              
      // sepa.hr.pain.001.001.09_11-2023-2                                                      
      Document sepa = new Document();
    //Document_PAIN_001_001_09 sepa = new Document_PAIN_001_001_09();

   #region GrpHdr

      //<MsgId>:
      //UNggggmmddnnnnizvor dokumenta; gdje je UN oznaka za kreditni transfer, ggggmmdd tekući          
      //datum podnošenja/slanja poruke, nnnn redni broj poruke u tekućem datumu i izvor dokumenta opisan
      //u nastavku.
      //Izvor dokumenta se popunjava u slučaju nacionalnih platnih transakcija u kunama kada se plaćanje
      //podnosi u jedinicu Fine ili inicira putem servisa e-plaćanja Fine. Dopuštene vrijednosti su:
      //300 - nalozi banaka za svoja plaćanja i za plaćanja na teret računa svojih klijenta
      //701 - nalozi klijenata inicirani putem servisa e-plaćanja Fine i na šalteru jedinica Fine
      //803 - elektronski nalozi koje inicira banka za plaćanja na teret transakcijskih računa svojih
      //klijenta radi izvršenja osnova za plaćanje
      //652 - elektronski medij koji podnosi HNB i Ministarstvo financija za plaćanja putem HSVP
      //530 - nalozi vezani za kolekciju države
      //502 - nalozi javnih prihoda s posebnim kontrolama
      //520 - posebna obrada naloga za plaćanje
      //550 - povrati i preknjiženja javnih prihoda koje dostavlja Porezna uprava odnosno APIS-IT
      //XML tag: <MsgId>
      //Učestalost pojave i ponavljanja: [1..1]
      //Vrsta podatka: Text/Tekst
      //Format podatka: maxLength: 35, MinLength:1/max 35 znakova
      //Primjer: <MsgId>UN201311260001701</MsgId>

      string izvorDokumenta = "";

    //string  theMsgId   = ExportFileName.Replace(".xml", "").Replace(".", "") + izvorDokumenta;
      System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(fullPathFileName);
      string  theMsgId   = dInfo.Name.Replace(".xml", "").Replace(".", "") + izvorDokumenta;
      string  theCreDtTm = _znpDate.ToString("s");
      string  theNbOfTxs = _theVirmanList.Count.ToString();
    //string  theCtrlSum = _theVirmanList.Sum(virman_rec => virman_rec.Money).ToStringVv_NoGroup_ForceDot();
    //decimal theCtrlSum = _theVirmanList.Sum(virman_rec => virman_rec.Money);
      decimal theCtrlSum = _theVirmanList.Sum(virman_rec => virman_rec.Money).Ron2();

      sepa.CstmrCdtTrfInitn.GrpHdr.MsgId       = theMsgId                ;
      sepa.CstmrCdtTrfInitn.GrpHdr.CreDtTm     = /*theCreDtTm*/_znpDate   ;
      sepa.CstmrCdtTrfInitn.GrpHdr.NbOfTxs     = theNbOfTxs              ;
      sepa.CstmrCdtTrfInitn.GrpHdr.CtrlSumSpecified = true               ;
      sepa.CstmrCdtTrfInitn.GrpHdr.CtrlSum     = theCtrlSum              ;

      sepa.CstmrCdtTrfInitn.GrpHdr.InitgPty.Nm = ZXC.CURR_prjkt_rec.Naziv;
    //sepa.CstmrCdtTrfInitn.GrpHdr.InitgPty.PstlAdr.AdrLine.Add(ZXC.CURR_prjkt_rec.Ulica1);
    //sepa.CstmrCdtTrfInitn.GrpHdr.InitgPty.PstlAdr.AdrLine.Add(ZXC.CURR_prjkt_rec.Grad);

      sepa.CstmrCdtTrfInitn.GrpHdr.InitgPty.IdSpecified = false;
    //sepa.CstmrCdtTrfInitn.GrpHdr.InitgPty.IdSpecified = true;
    //sepa.CstmrCdtTrfInitn.GrpHdr.InitgPty.Id.Item = new OrganisationIdentification4();
    //(sepa.CstmrCdtTrfInitn.GrpHdr.InitgPty.Id.Item as OrganisationIdentification4).Item = new GenericOrganisationIdentification1();
    //((GenericOrganisationIdentification1)(sepa.CstmrCdtTrfInitn.GrpHdr.InitgPty.Id.Item as OrganisationIdentification4).Item).Id = ZXC.CURR_prjkt_rec.Oib;
    //((GenericOrganisationIdentification1)(sepa.CstmrCdtTrfInitn.GrpHdr.InitgPty.Id.Item as OrganisationIdentification4).Item).SchmeNm.Item = "Cd";

   #endregion GrpHdr

   #region PmtInf

   #region Common variablez

      List<string> PmtInf_btchBookgIDlist = _theVirmanList.Where(v => v.BtchBookgID.NotEmpty()).Select(v => v.BtchBookgID).Distinct().ToList();

      CreditTransferTransactionInformation10 theTx;
      PaymentInstructionInformation3         pmtInf;

      DateTime executionDate = _theVirmanList[0].DateValuta.NotEmpty() ? _theVirmanList[0].DateValuta : _znpDate; // TODO: check zis! 
      string   dbtrIBAN      = _theVirmanList[0].Ziro1;

   #endregion Common variablez

   #region GRUPA1 (NON batching) PmtInf - header (everything bef transactions)

      if(/*RptFilter.VirmanGroup*/_virmanGroup != ZXC.VirmanBtchBookgKind.BtchBookg_ONLY) // nemoj raditi GRUPA1 (NON batching) - ako zelimo samo BtchBookg_ONLY 
      {
         pmtInf = new PaymentInstructionInformation3();
       
       //pmtInf.PmtTpInfSpecified  =
         pmtInf.UltmtDbtrSpecified = false;
       
         pmtInf.CtrlSumSpecified  = true ; // ! 
       
         theNbOfTxs = _theVirmanList.Where(v => v.BtchBookgID.IsEmpty()).Count().ToString();
       //theCtrlSum = _theVirmanList.Where(v => v.BtchBookgID.IsEmpty()).Sum(virman_rec => virman_rec.Money);
         theCtrlSum = _theVirmanList.Where(v => v.BtchBookgID.IsEmpty()).Sum(virman_rec => virman_rec.Money).Ron2();
       
         pmtInf.PmtInfId         = (PmtInf_btchBookgIDlist.Count.IsZero()) ? "GRUPA" : "POREZI I DOPRINOSI";
         pmtInf.PmtMtd           = PaymentMethod3Code.TRF  ;
         pmtInf.NbOfTxs          = theNbOfTxs              ;
         pmtInf.CtrlSum          = theCtrlSum              ;

         // 05.11.2019: RBA se buni, pa je ovo 1. pokusaj zadovoljenja ___ START ___ 
         pmtInf.PmtTpInf                    = new PaymentTypeInformation19_1();
         pmtInf.PmtTpInf.InstrPrty          = Priority2Code.NORM;
         pmtInf.PmtTpInf.InstrPrtySpecified = true;
         // 05.11.2019: RBA se buni, pa je ovo 1. pokusaj zadovoljenja ___  END  ___ 

         // 08.05.2023: neki njemacki klijent rozel-a treba sepu a njem. banka se buni da ovo fali pa dodajemo: 
         pmtInf.PmtTpInf.SvcLvl.Cd = "SEPA";
       //pmtInf.PmtTpInf.SvcLvlSpecified = true;

         pmtInf.ReqdExctnDt      = executionDate           ;
       
         pmtInf.Dbtr.Nm          = ZXC.CURR_prjkt_rec.Naziv;
         pmtInf.Dbtr.PstlAdr.AdrLine.Add(ZXC.CURR_prjkt_rec.Ulica1);
         pmtInf.Dbtr.PstlAdr.AdrLine.Add(ZXC.CURR_prjkt_rec.Grad);
       
         pmtInf.Dbtr.IdSpecified = true;
         pmtInf.Dbtr.Id.Item = new OrganisationIdentification4();
         (pmtInf.Dbtr.Id.Item as OrganisationIdentification4).Item = new GenericOrganisationIdentification1();
         ((GenericOrganisationIdentification1)(pmtInf.Dbtr.Id.Item as OrganisationIdentification4).Item).Id = ZXC.CURR_prjkt_rec.Oib;
         ((GenericOrganisationIdentification1)(pmtInf.Dbtr.Id.Item as OrganisationIdentification4).Item).SchmeNmSpecified = false;
       //((GenericOrganisationIdentification1)(pmtInf.Dbtr.Id.Item as OrganisationIdentification4).Item).SchmeNm.Item = "Cd";
       
         pmtInf.DbtrAcct.Id.IBAN = dbtrIBAN.TrimStart(' ').TrimEnd(' ');
       //pmtInf.DbtrAgt.FinInstnId.Item = "NOTPROVIDED";
         pmtInf.DbtrAgt.FinInstnId.Item = Get_DbtrAgt_BIC(pmtInf.DbtrAcct.Id.IBAN);

   #endregion GRUPA1 (NON batching) PmtInf - header (everything bef transactions)

   #region GRUPA1 (NON batching) Transactions LOOP ===========================================================================================

         pmtInf.CdtTrfTxInf = new List<CreditTransferTransactionInformation10>(_theVirmanList.Count);

         //int virRbr = 0;

         foreach(VirmanStruct virman_rec in _theVirmanList) // CdtTrfTxInf
         {
            //virRbr++;

            //CheckSomeSEPAvalues(virRbr, virman_rec);

            if(virman_rec.BtchBookgID.NotEmpty()) continue; // this virman goes down, in some Batch Group 

            theTx = Get_SEPATx_placa_001_001_03(virman_rec);

            pmtInf.CdtTrfTxInf.Add(theTx);

         } // foreach(VirmanStruct virman_rec in _theVirmanList) // CdtTrfTxInf

         sepa.CstmrCdtTrfInitn.PmtInf.Add(pmtInf); // !!! 

      } // if(RptFilter.VirmanGroup != ZXC.VirmanBtchBookgKind.BtchBookg_ONLY) 

   #endregion GRUPA1 (NON batching) Transactions LOOP ========================================================================================

      foreach(string currBtchBookgID in PmtInf_btchBookgIDlist) // GRUPE LOOPing - za sada se UVIJEK vrti samo jemput (sam0 jedna BatchBooking grupa smije postojati) 
      {
   #region CURR-btchBookgID PmtInf - header (everything bef transactions)

         pmtInf = new PaymentInstructionInformation3();

         pmtInf.BtchBookgSpecified = true; // !!! 
         pmtInf.BtchBookg          = true; // !!! 

       //pmtInf.PmtTpInfSpecified  = false;
       //pmtInf.UltmtDbtrSpecified = false;

         pmtInf.CtrlSumSpecified  = true ; // ! 

         theNbOfTxs = _theVirmanList.Where(v => v.BtchBookgID == currBtchBookgID).Count().ToString();
       //theCtrlSum = _theVirmanList.Where(v => v.BtchBookgID == currBtchBookgID).Sum(virman_rec => virman_rec.Money);
         theCtrlSum = _theVirmanList.Where(v => v.BtchBookgID == currBtchBookgID).Sum(virman_rec => virman_rec.Money).Ron2();

         pmtInf.PmtTpInf.CtgyPurp.Cd = "SALA";

         pmtInf.PmtInfId         = currBtchBookgID         ;
         pmtInf.PmtMtd           = PaymentMethod3Code.TRF  ;
         pmtInf.NbOfTxs          = theNbOfTxs              ;
         pmtInf.CtrlSum          = theCtrlSum              ;
         pmtInf.ReqdExctnDt      = executionDate           ;

         pmtInf.Dbtr.Nm          = ZXC.CURR_prjkt_rec.Naziv;
         pmtInf.Dbtr.PstlAdr.AdrLine.Add(ZXC.CURR_prjkt_rec.Ulica1);
         pmtInf.Dbtr.PstlAdr.AdrLine.Add(ZXC.CURR_prjkt_rec.Grad);

         pmtInf.Dbtr.IdSpecified = true;
         pmtInf.Dbtr.Id.Item = new OrganisationIdentification4();
         (pmtInf.Dbtr.Id.Item as OrganisationIdentification4).Item = new GenericOrganisationIdentification1();
         ((GenericOrganisationIdentification1)(pmtInf.Dbtr.Id.Item as OrganisationIdentification4).Item).Id = ZXC.CURR_prjkt_rec.Oib;
         ((GenericOrganisationIdentification1)(pmtInf.Dbtr.Id.Item as OrganisationIdentification4).Item).SchmeNmSpecified = false;
       //((GenericOrganisationIdentification1)(pmtInf.Dbtr.Id.Item as OrganisationIdentification4).Item).SchmeNm.Item = "Cd";

         pmtInf.DbtrAcct.Id.IBAN = dbtrIBAN.TrimStart(' ').TrimEnd(' ');
         pmtInf.DbtrAcct.Ccy     = /*"HRK"*/ ZXC.EURorHRKstr;
       //pmtInf.DbtrAgt.FinInstnId.Item = "NOTPROVIDED";
         pmtInf.DbtrAgt.FinInstnId.Item = Get_DbtrAgt_BIC(pmtInf.DbtrAcct.Id.IBAN);

         pmtInf.UltmtDbtrSpecified    = 
         pmtInf.UltmtDbtr.IdSpecified = true;

         pmtInf.UltmtDbtr.Id.Item = new OrganisationIdentification4();
         (pmtInf.UltmtDbtr.Id.Item as OrganisationIdentification4).Item = new GenericOrganisationIdentification1();
         ((GenericOrganisationIdentification1)(pmtInf.UltmtDbtr.Id.Item as OrganisationIdentification4).Item).Id = ZXC.CURR_prjkt_rec.Oib;
         ((GenericOrganisationIdentification1)(pmtInf.UltmtDbtr.Id.Item as OrganisationIdentification4).Item).SchmeNmSpecified = false;
       //((GenericOrganisationIdentification1)(pmtInf.UltmtDbtr.Id.Item as OrganisationIdentification4).Item).SchmeNm.Item = "Cd";

   #endregion CURR-btchBookgID  PmtInf - header (everything bef transactions)

   #region CURR-btchBookgID  Transactions LOOP ===========================================================================================

         pmtInf.CdtTrfTxInf = new List<CreditTransferTransactionInformation10>(_theVirmanList.Count);

         foreach(VirmanStruct virman_rec in _theVirmanList.Where(v => v.BtchBookgID == currBtchBookgID)) // CdtTrfTxInf
         {
            theTx = Get_SEPATx_placa_001_001_03(virman_rec);

            pmtInf.CdtTrfTxInf.Add(theTx);

         } // foreach(VirmanStruct virman_rec in _theVirmanList) // CdtTrfTxInf

         sepa.CstmrCdtTrfInitn.PmtInf.Add(pmtInf); // !!! 

   #endregion CURR-btchBookgID  Transactions LOOP ========================================================================================

      } // foreach(string btchBookgID in PmtInf_btchBookgIDlist) 

   #endregion PmtInf

   #region Serialize to XML Doucment & Save to XML File

      sepa.SaveToFile(fullPathFileName, /*Encoding.UTF8*/ ZXC.VvUTF8Encoding_noBOM);

   #endregion Serialize to XML Doucment & Save to XML File

      return true;
   }

   private static CreditTransferTransactionInformation10 Get_SEPATx_placa_001_001_03(VirmanStruct virman_rec)
   {
      CreditTransferTransactionInformation10 theTx = new CreditTransferTransactionInformation10();

      theTx.PmtTpInfSpecified  = 
      theTx.UltmtDbtrSpecified = 
      theTx.UltmtCdtrSpecified = 
      theTx.PurpSpecified      = false;
      theTx.CdtrAgtSpecified   = false;

      theTx.PmtId.EndToEndId = virman_rec.PnbzMod + virman_rec.Pnbz; // poziv na broj platitelja

      theTx.Amt.InstdAmt.Ccy = /*"HRK"*/ ZXC.EURorHRKstr;
    //theTx.Amt.InstdAmt.Value = virman_rec.Money;
      theTx.Amt.InstdAmt.Value = virman_rec.Money.Ron2();

      theTx.ChrgBr = ChargeBearerType1Code.SLEV; //a ako nije popunjeno SLEV, podrazumijeva se Troškovna opcija SLEV

      theTx.Cdtr.Nm          = virman_rec.Prim1;
      theTx.Cdtr.IdSpecified = false;
      theTx.Cdtr.PstlAdr.AdrLine.Add(virman_rec.Prim2);
      if(virman_rec.Prim3.NotEmpty()) theTx.Cdtr.PstlAdr.AdrLine.Add(virman_rec.Prim3);

      theTx.CdtrAcct.Id.Item = virman_rec.Ziro2.TrimStart(' ').TrimEnd(' ');

      theTx.RmtInf.Item = new StructuredRemittanceInformation7();
      (theTx.RmtInf.Item as StructuredRemittanceInformation7).CdtrRefInf.Tp.CdOrPrtry.Cd = DocumentType3Code.SCOR;
      (theTx.RmtInf.Item as StructuredRemittanceInformation7).CdtrRefInf.Ref             = virman_rec.PnboMod + virman_rec.Pnbo; // poziv na broj primatelja
      (theTx.RmtInf.Item as StructuredRemittanceInformation7).AddtlRmtInf                = virman_rec.OpisPl;

      if(virman_rec.BtchBookgID.NotEmpty() || virman_rec.IsSALA) // ??? !!! ??? 
      {
         theTx.PurpSpecified = true;
         theTx.Purp.Cd       = "SALA";
      }

      return theTx;
   }

#endif

//#if SEPA_001_001_09

   public static bool ExecuteExportSEPA_001_001_09(string fullPathFileName, List<VirmanStruct> _theVirmanList, DateTime _znpDate, ZXC.VirmanBtchBookgKind _virmanGroup, bool isPlaca) // VOILA 
   {
      // 16.10.2023: tu si stao. sada treba ovaj Document zamijeniti sa Document_PAIN_001_001_09
      // pa gore ugasiti nepotreban 'using PAIN_001_001_03;'                                    
      // pa u ExecuteExportValidationSEPA ugasiti                                               
      // sepa.hr.pain.001.001.03.NOVA                                                           
      // a upaliti                                                                              
      // sepa.hr.pain.001.001.09_11-2023-2                                                      
      //Document sepa = new Document();
      Document sepa = new Document();

      #region GrpHdr

      //<MsgId>:
      //UNggggmmddnnnnizvor dokumenta; gdje je UN oznaka za kreditni transfer, ggggmmdd tekući          
      //datum podnošenja/slanja poruke, nnnn redni broj poruke u tekućem datumu i izvor dokumenta opisan
      //u nastavku.
      //Izvor dokumenta se popunjava u slučaju nacionalnih platnih transakcija u kunama kada se plaćanje
      //podnosi u jedinicu Fine ili inicira putem servisa e-plaćanja Fine. Dopuštene vrijednosti su:
      //300 - nalozi banaka za svoja plaćanja i za plaćanja na teret računa svojih klijenta
      //701 - nalozi klijenata inicirani putem servisa e-plaćanja Fine i na šalteru jedinica Fine
      //803 - elektronski nalozi koje inicira banka za plaćanja na teret transakcijskih računa svojih
      //klijenta radi izvršenja osnova za plaćanje
      //652 - elektronski medij koji podnosi HNB i Ministarstvo financija za plaćanja putem HSVP
      //530 - nalozi vezani za kolekciju države
      //502 - nalozi javnih prihoda s posebnim kontrolama
      //520 - posebna obrada naloga za plaćanje
      //550 - povrati i preknjiženja javnih prihoda koje dostavlja Porezna uprava odnosno APIS-IT
      //XML tag: <MsgId>
      //Učestalost pojave i ponavljanja: [1..1]
      //Vrsta podatka: Text/Tekst
      //Format podatka: maxLength: 35, MinLength:1/max 35 znakova
      //Primjer: <MsgId>UN201311260001701</MsgId>

      string izvorDokumenta = "";

      System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(fullPathFileName);
      string  theMsgId   = dInfo.Name.Replace(".xml", "").Replace(".", "") + izvorDokumenta;
      string  theCreDtTm = _znpDate.ToString("s");
      string  theNbOfTxs = _theVirmanList.Count.ToString();
      decimal theCtrlSum = _theVirmanList.Sum(virman_rec => virman_rec.Money).Ron2();

      sepa.CstmrCdtTrfInitn.GrpHdr             = new GroupHeader85()     ;
      sepa.CstmrCdtTrfInitn.GrpHdr.MsgId       = theMsgId                ;
      sepa.CstmrCdtTrfInitn.GrpHdr.CreDtTm     = /*theCreDtTm*/_znpDate  ;
      sepa.CstmrCdtTrfInitn.GrpHdr.NbOfTxs     = theNbOfTxs              ;
  //sepa.CstmrCdtTrfInitn.GrpHdr.CtrlSumSpecified = true               ;  //!!!Specified
      sepa.CstmrCdtTrfInitn.GrpHdr.CtrlSum     = theCtrlSum              ;

      sepa.CstmrCdtTrfInitn.GrpHdr.InitgPty    = new PartyIdentification135_2();
      sepa.CstmrCdtTrfInitn.GrpHdr.InitgPty.Nm = ZXC.CURR_prjkt_rec.Naziv;
  //sepa.CstmrCdtTrfInitn.GrpHdr.InitgPty.IdSpecified = false;  //!!!Specified

      #endregion GrpHdr

      #region PmtInf

      #region Common variablez

      List<string> PmtInf_btchBookgIDlist = _theVirmanList.Where(v => v.BtchBookgID.NotEmpty()).Select(v => v.BtchBookgID).Distinct().ToList();

      CreditTransferTransaction34 theTx;
      PaymentInstruction30        pmtInf;

      DateTime executionDate = _theVirmanList[0].DateValuta.NotEmpty() ? _theVirmanList[0].DateValuta : _znpDate; // TODO: check zis! 
      string   dbtrIBAN      = _theVirmanList[0].Ziro1;

      #endregion Common variablez

      #region GRUPA1 (NON batching) PmtInf - header (everything bef transactions)

      if(/*RptFilter.VirmanGroup*/_virmanGroup != ZXC.VirmanBtchBookgKind.BtchBookg_ONLY) // nemoj raditi GRUPA1 (NON batching) - ako zelimo samo BtchBookg_ONLY 
      {
         pmtInf = new PaymentInstruction30();
       
//pmtInf.UltmtDbtrSpecified = false;     //!!!Specified

//pmtInf.CtrlSumSpecified  = true ; // ! //!!!Specified
       
         theNbOfTxs = _theVirmanList.Where(v => v.BtchBookgID.IsEmpty()).Count().ToString();
         theCtrlSum = _theVirmanList.Where(v => v.BtchBookgID.IsEmpty()).Sum(virman_rec => virman_rec.Money).Ron2();
       
         pmtInf.PmtInfId         = (PmtInf_btchBookgIDlist.Count.IsZero()) ? "GRUPA" : "POREZI I DOPRINOSI";
         pmtInf.PmtMtd           = PaymentMethod3Code.TRF  ;
         pmtInf.NbOfTxs          = theNbOfTxs              ;
         pmtInf.CtrlSum          = theCtrlSum              ;

         // 05.11.2019: RBA se buni, pa je ovo 1. pokusaj zadovoljenja ___ START ___ 
         pmtInf.PmtTpInf                    = new PaymentTypeInformation26PmtInf();
         pmtInf.PmtTpInf.InstrPrty          = Priority2Code.NORM;
         pmtInf.PmtTpInf.InstrPrtySpecified = true;
         // 05.11.2019: RBA se buni, pa je ovo 1. pokusaj zadovoljenja ___  END  ___ 

         // 08.05.2023: neki njemacki klijent rozel-a treba sepu a njem. banka se buni da ovo fali pa dodajemo: 
         pmtInf.PmtTpInf.SvcLvl    = new ServiceLevel8Choice();
         pmtInf.PmtTpInf.SvcLvl.Cd = "SEPA";
       //pmtInf.PmtTpInf.SvcLvlSpecified = true;

         pmtInf.ReqdExctnDt    = new DateAndDateTime2Choice();
         pmtInf.ReqdExctnDt.Dt = executionDate           ; //!!! nemam pojma kaj s time
       
         pmtInf.Dbtr                 = new PartyIdentification135_1();
         pmtInf.Dbtr.Nm              = ZXC.CURR_prjkt_rec.Naziv;
         pmtInf.Dbtr.PstlAdr         = new PostalAddress24();
         pmtInf.Dbtr.PstlAdr.StrtNm  = ZXC.CURR_prjkt_rec.UlicaBezBroja_1;
         pmtInf.Dbtr.PstlAdr.BldgNb  = ZXC.CURR_prjkt_rec.UlicniBroj_1;
         pmtInf.Dbtr.PstlAdr.PstCd   = ZXC.CURR_prjkt_rec.PostaBr;
         pmtInf.Dbtr.PstlAdr.TwnNm   = ZXC.CURR_prjkt_rec.Grad;
         pmtInf.Dbtr.PstlAdr.Ctry    = ZXC.CURR_prjkt_rec.VatCntryCode_NonEmpty; //novo - ovo jos provjeri
       //pmtInf.Dbtr.PstlAdr.AdrLine = new string[] { ZXC.CURR_prjkt_rec.Ulica1, ZXC.CURR_prjkt_rec.Grad };

         pmtInf.DbtrAcct         = new CashAccount38Dbtr();
         pmtInf.DbtrAcct.Id      = new AccountIdentification4Choice();
         pmtInf.DbtrAcct.Id.IBAN = dbtrIBAN.TrimStart(' ').TrimEnd(' ');
         
         pmtInf.DbtrAgt                 = new BranchAndFinancialInstitutionIdentification6DbtrAgt();
         pmtInf.DbtrAgt.FinInstnId      = new FinancialInstitutionIdentification18DbtrAgt();
         pmtInf.DbtrAgt.FinInstnId.Item = Get_DbtrAgt_BIC(pmtInf.DbtrAcct.Id.IBAN);

      #endregion GRUPA1 (NON batching) PmtInf - header (everything bef transactions)

      #region GRUPA1 (NON batching) Transactions LOOP ===========================================================================================

       //pmtInf.CdtTrfTxInf = new List<CreditTransferTransaction34>(_theVirmanList.Count);
         pmtInf.CdtTrfTxInf = new CreditTransferTransaction34[_theVirmanList.Count];

         int i = 0;
         foreach(VirmanStruct virman_rec in _theVirmanList) // CdtTrfTxInf
         {
            if(virman_rec.BtchBookgID.NotEmpty()) continue; // this virman goes down, in some Batch Group 

            theTx = Get_SEPATx_placa_001_001_09(virman_rec);

          //pmtInf.CdtTrfTxInf.Add(theTx);
            pmtInf.CdtTrfTxInf[i++] =theTx;

         } // foreach(VirmanStruct virman_rec in _theVirmanList) // CdtTrfTxInf

       //sepa.CstmrCdtTrfInitn.PmtInf.Add(pmtInf); // !!! 
         sepa.CstmrCdtTrfInitn.PmtInf = new PaymentInstruction30[] { pmtInf }; // !!! 

      } // if(RptFilter.VirmanGroup != ZXC.VirmanBtchBookgKind.BtchBookg_ONLY) 

      #endregion GRUPA1 (NON batching) Transactions LOOP ========================================================================================

      foreach(string currBtchBookgID in PmtInf_btchBookgIDlist) // GRUPE LOOPing - za sada se UVIJEK vrti samo jemput (sam0 jedna BatchBooking grupa smije postojati) NETO!!!
      {
         #region CURR-btchBookgID PmtInf - header (everything bef transactions)

         pmtInf = new PaymentInstruction30();

         pmtInf.BtchBookgSpecified = true; // !!! 
         pmtInf.BtchBookg          = true; // !!! 

//         pmtInf.CtrlSumSpecified  = true ; // ! 

         theNbOfTxs = _theVirmanList.Where(v => v.BtchBookgID == currBtchBookgID).Count().ToString();
         theCtrlSum = _theVirmanList.Where(v => v.BtchBookgID == currBtchBookgID).Sum(virman_rec => virman_rec.Money).Ron2();

//       pmtInf.PmtTpInf.CtgyPurp.Cd   = "SALA";

         pmtInf.PmtInfId         = currBtchBookgID         ;
         pmtInf.PmtMtd           = PaymentMethod3Code.TRF  ;
         pmtInf.NbOfTxs          = theNbOfTxs              ;
         pmtInf.CtrlSum          = theCtrlSum              ;

         //05.04.2024. dodajemo ovo i ovdje jer ne prolazi BBneto
         //05.11.2019: RBA se buni, pa je ovo 1. pokusaj zadovoljenja ___ START ___ 
         pmtInf.PmtTpInf                    = new PaymentTypeInformation26PmtInf();
         pmtInf.PmtTpInf.InstrPrty          = Priority2Code.NORM;
         pmtInf.PmtTpInf.InstrPrtySpecified = true;
         // 05.11.2019: RBA se buni, pa je ovo 1. pokusaj zadovoljenja ___  END  ___ 

         // 08.05.2023: neki njemacki klijent rozel-a treba sepu a njem. banka se buni da ovo fali pa dodajemo: 
         pmtInf.PmtTpInf.SvcLvl    = new ServiceLevel8Choice();
         pmtInf.PmtTpInf.SvcLvl.Cd = "SEPA";
         //05.04.2024. dodajemo ovo i ovdje jer ne prolazi BBneto
 
         //09.04.2024.!!!!
         pmtInf.PmtTpInf.CtgyPurp = new CategoryPurpose1Choice();
         pmtInf.PmtTpInf.CtgyPurp.Item = "SALA";

         pmtInf.ReqdExctnDt    = new DateAndDateTime2Choice();
         pmtInf.ReqdExctnDt.Dt = executionDate           ;

         pmtInf.Dbtr                 = new PartyIdentification135_1();
         pmtInf.Dbtr.Nm              = ZXC.CURR_prjkt_rec.Naziv;
         pmtInf.Dbtr.PstlAdr         = new PostalAddress24();
         pmtInf.Dbtr.PstlAdr.StrtNm  = ZXC.CURR_prjkt_rec.UlicaBezBroja_1;
         pmtInf.Dbtr.PstlAdr.BldgNb  = ZXC.CURR_prjkt_rec.UlicniBroj_1;
         pmtInf.Dbtr.PstlAdr.PstCd   = ZXC.CURR_prjkt_rec.PostaBr;
         pmtInf.Dbtr.PstlAdr.TwnNm   = ZXC.CURR_prjkt_rec.Grad;
         pmtInf.Dbtr.PstlAdr.Ctry    = ZXC.CURR_prjkt_rec.VatCntryCode_NonEmpty; //novo - ovo jos provjeri

         // 19.03.2024: 
         if(ZXC.CURR_prjkt_rec.Ulica1.IsEmpty()) ZXC.aim_emsg(MessageBoxIcon.Warning, "Pri izradi SEPA datoteke prazna 'Ulica1' platitelja možda bude problem?!");
         if(ZXC.CURR_prjkt_rec.Grad  .IsEmpty()) ZXC.aim_emsg(MessageBoxIcon.Warning, "Pri izradi SEPA datoteke prazan 'Grad'   platitelja možda bude problem?!");

         pmtInf.DbtrAcct         = new CashAccount38Dbtr();
         pmtInf.DbtrAcct.Id      = new AccountIdentification4Choice();
         pmtInf.DbtrAcct.Id.IBAN = dbtrIBAN.TrimStart(' ').TrimEnd(' ');
         
         pmtInf.DbtrAgt                 = new BranchAndFinancialInstitutionIdentification6DbtrAgt();
         pmtInf.DbtrAgt.FinInstnId      = new FinancialInstitutionIdentification18DbtrAgt();
         pmtInf.DbtrAgt.FinInstnId.Item = Get_DbtrAgt_BIC(pmtInf.DbtrAcct.Id.IBAN);


//         pmtInf.UltmtDbtrSpecified    = 
//         pmtInf.UltmtDbtr.IdSpecified = true;

//pmtInf.UltmtDbtr.Id.Item = new OrganisationIdentification29();
//(pmtInf.UltmtDbtr.Id.Item as OrganisationIdentification29).Item = new GenericOrganisationIdentification1();
//((GenericOrganisationIdentification1)(pmtInf.UltmtDbtr.Id.Item as OrganisationIdentification29).Item).Id = ZXC.CURR_prjkt_rec.Oib;
//         ((GenericOrganisationIdentification1)(pmtInf.UltmtDbtr.Id.Item as OrganisationIdentification29).Item).SchmeNmSpecified = false;

         #endregion CURR-btchBookgID  PmtInf - header (everything bef transactions)

         #region CURR-btchBookgID  Transactions LOOP ===========================================================================================

         //pmtInf.CdtTrfTxInf = new List<CreditTransferTransaction34>(_theVirmanList.Count);
         pmtInf.CdtTrfTxInf = new CreditTransferTransaction34[_theVirmanList.Count];

         int i = 0;
         foreach(VirmanStruct virman_rec in _theVirmanList.Where(v => v.BtchBookgID == currBtchBookgID)) // CdtTrfTxInf
         {
            theTx = Get_SEPATx_placa_001_001_09(virman_rec);

          //pmtInf.CdtTrfTxInf.Add(theTx);
            pmtInf.CdtTrfTxInf[i++] = theTx;

         } // foreach(VirmanStruct virman_rec in _theVirmanList) // CdtTrfTxInf

       //sepa.CstmrCdtTrfInitn.PmtInf.Add(pmtInf); // !!! 
         sepa.CstmrCdtTrfInitn.PmtInf = new PaymentInstruction30[] { pmtInf }; // !!! 

         #endregion CURR-btchBookgID  Transactions LOOP ========================================================================================

      } // foreach(string btchBookgID in PmtInf_btchBookgIDlist) 

      #endregion PmtInf

      #region Serialize to XML Doucment & Save to XML File

      sepa.SaveToFile(fullPathFileName, /*Encoding.UTF8*/ ZXC.VvUTF8Encoding_noBOM);

      #endregion Serialize to XML Doucment & Save to XML File

      return true;
   }

   private static CreditTransferTransaction34 Get_SEPATx_placa_001_001_09(VirmanStruct virman_rec)
   {
      CreditTransferTransaction34 theTx = new CreditTransferTransaction34();

//    theTx.PmtTpInfSpecified  = 
//    theTx.UltmtDbtrSpecified = 
//    theTx.UltmtCdtrSpecified = 
//    theTx.PurpSpecified      = false;
//    theTx.CdtrAgtSpecified   = false;

      theTx.PmtId            = new PaymentIdentification6();
      theTx.PmtId.EndToEndId = virman_rec.PnbzMod + virman_rec.Pnbz; // poziv na broj platitelja

      theTx.Amt              = new AmountType4Choice();
      theTx.Amt.InstdAmt     = new ActiveOrHistoricCurrencyAndAmount();
      theTx.Amt.InstdAmt.Ccy = /*"HRK"*/ ZXC.EURorHRKstr;
      theTx.Amt.InstdAmt.Value = virman_rec.Money.Ron2();

      theTx.ChrgBr = ChargeBearerType1Code.SLEV; //a ako nije popunjeno SLEV, podrazumijeva se Troškovna opcija SLEV

      theTx.Cdtr    = new PartyIdentification135_1();
      theTx.Cdtr.Nm = virman_rec.Prim1;
//      theTx.Cdtr.IdSpecified = false;

    //theTx.Cdtr.PstlAdr.AdrLine.Add(virman_rec.Prim2);
    //if(virman_rec.Prim3.NotEmpty()) theTx.Cdtr.PstlAdr.AdrLine.Add(virman_rec.Prim3);
      theTx.Cdtr.PstlAdr         = new PostalAddress24();

      // 19.03.2024: 
      theTx.Cdtr.PstlAdr.Ctry    = "HR";

      // 19.03.2024: 
    //theTx.Cdtr.PstlAdr.AdrLine = new string[] { virman_rec.Prim2, virman_rec.Prim2.NotEmpty() ? virman_rec.Prim3 : " " };
      theTx.Cdtr.PstlAdr.AdrLine = new string[] { virman_rec.Prim2,                               virman_rec.Prim3       };

      // 19.03.2024: 
      if(virman_rec.Prim2.IsEmpty()) ZXC.aim_emsg(MessageBoxIcon.Warning, "Pri izradi SEPA datoteke prazna 'AdrLine1' primatelja možda bude problem?!");
      if(virman_rec.Prim3.IsEmpty()) ZXC.aim_emsg(MessageBoxIcon.Warning, "Pri izradi SEPA datoteke prazan 'AdrLine2' primatelja možda bude problem?!");

      theTx.CdtrAcct         = new CashAccount38Cdtr();
      theTx.CdtrAcct.Id      = new AccountIdentification4Choice_2();
      theTx.CdtrAcct.Id.Item = virman_rec.Ziro2.TrimStart(' ').TrimEnd(' ');

      theTx.RmtInf      = new RemittanceInformation16();
      theTx.RmtInf.Item = new StructuredRemittanceInformation16();
      (theTx.RmtInf.Item   as StructuredRemittanceInformation16).CdtrRefInf                 = new CreditorReferenceInformation2();
      (theTx.RmtInf.Item   as StructuredRemittanceInformation16).CdtrRefInf.Tp              = new CreditorReferenceType2();
      (theTx.RmtInf.Item   as StructuredRemittanceInformation16).CdtrRefInf.Tp.CdOrPrtry    = new CreditorReferenceType1Choice();
      (theTx.RmtInf.Item   as StructuredRemittanceInformation16).CdtrRefInf.Tp.CdOrPrtry.Cd = DocumentType3Code.SCOR;
      (theTx.RmtInf.Item   as StructuredRemittanceInformation16).CdtrRefInf.Ref             = virman_rec.PnboMod + virman_rec.Pnbo; // poziv na broj primatelja
      (theTx.RmtInf.Item   as StructuredRemittanceInformation16).AddtlRmtInf                = virman_rec.OpisPl;

      if(virman_rec.BtchBookgID.NotEmpty() || virman_rec.IsSALA) // ??? !!! ??? 
      {
//         theTx.PurpSpecified = true;
         theTx.Purp          = new Purpose2Choice();
         theTx.Purp.Cd       = "SALA";
      }

      return theTx;
   }

//#endif

   private static object Get_DbtrAgt_BIC(string IBAN)
   {
      string IBANroot6 = IBAN.SubstringSafe(4, 7);

      if(IBANroot6.Length < 7) return "NOTPROVIDED";

      switch(IBANroot6)
      {
/* BANCO POPOLARE CROATIA d.d. Zagreb           */ case "4115008": return "BPCRHR22";
/* BANKA BROD d.d. Slavonski Brod               */ case "4124003": return "BBRDHR22";
/* BANKA KOVANICA d.d. Varaždin                 */ case "4133006": return "SKOVHR22";
/* BANKA SPLITSKO-DALMATINSKA d.d. Split        */ case "4109006": return "DALMHR22";
/* BKS BANK d.d. Rijeka                         */ case "2488001": return "BFKKHR22";
/* CENTAR BANKA d.d. Zagreb                     */ case "2382001": return "CBZGHR2X";
/* CROATIA BANKA d.d. Zagreb                    */ case "2485003": return "CROAHR2X";
/* ERSTE & STEIERMÄRKISCHE BANK d.d. Rijeka     */ case "2402006": return "ESBCHR22";
/* HRVATSKA BANKA ZA OBNOVU I RAZVITAK Zagreb   */ case "2493003": return "HKBOHR2X";
/* HRVATSKA NARODNA BANKA                       */ case "1001005": return "NBHRHR2D";
/* HRVATSKA POŠTANSKA BANKA d.d. Zagreb         */ case "2390001": return "HPBZHR2X";
/* HYPO ALPE-ADRIA-BANK d.d. Zagreb             */ case "2500009": return "HAABHR22";
/* IMEX BANKA d.d. Split                        */ case "2492008": return "IMXXHR22";
/* ISTARSKA KREDITNA BANKA UMAG d.d. Umag       */ case "2380006": return "ISKBHR2X";
/* JADRANSKA BANKA d.d. Šibenik                 */ case "2411006": return "JADRHR2X";
/* KARLOVAČKA BANKA d.d. Karlovac               */ case "2400008": return "KALCHR2X";
/* KREDITNA BANKA ZAGREB d.d. Zagreb            */ case "2481000": return "KREZHR2X";
/* MEĐIMURSKA BANKA d.d. Čakovec                */ case "2392007": return "MBCKHR2X";
/* NAVA BANKA d.d. Zagreb                       */ case "2495009": return "NAVBHR22";
/* OTP BANKA HRVATSKA d.d. Zadar                */ case "2407000": return "OTPVHR2X";
/* PARTNER BANKA d.d. Zagreb                    */ case "2408002": return "PAZGHR2X";
/* PODRAVSKA BANKA d.d. Koprivnica              */ case "2386002": return "PDKCHR2X";
/* PRIMORSKA BANKA d.d. Rijeka                  */ case "4132003": return "SPRMHR22";
/* PRIVREDNA BANKA ZAGREB d.d. Zagreb           */ case "2340009": return "PBZGHR2X";
/* RAIFFEISENBANK AUSTRIA d.d. Zagreb           */ case "2484008": return "RZBHHR2X";
/* SAMOBORSKA BANKA d.d. Samobor                */ case "2403009": return "SMBRHR22";
/* SLATINSKA BANKA d.d. Slatina                 */ case "2412009": return "SBSLHR2X";
/* SOCIETE GENERALE - SPLITSKA BANKA d.d. Split */ case "2330003": return "SOGEHR22";
/* ŠTEDBANKA d.d. Zagreb                        */ case "2483005": return "STEDHR22";
/* TESLA ŠTEDNA BANKA d.d. Zagreb               */ case "6717002": return "ASBZHR22";
/* VABA d.d. BANKA Varaždin                     */ case "2489004": return "VBVZHR22";
/* VENETO BANKA d.d.. Zagreb                    */ case "2381009": return "CCBZHR2X";
/* VOLKSBANK d.d. Zagreb                        */ case "2503007": return "VBCRHR22";
/* ZAGREBAČKA BANKA d.d. Zagreb                 */ case "2360000": return "ZABAHR2X";
                                                   default       : return "NOTPROVIDED";
      }
   }

   internal void CheckSomeSEPAvalues(int virRbr, VirmanStruct virman_rec)
   {
      if(virman_rec.Ziro2.IsEmpty() || virman_rec.Ziro2.TrimStart(' ').TrimEnd(' ').Length != 21) ReportBadSEPAdata(virRbr, virman_rec.Prim1, "IBAN", virman_rec.Ziro2);

      if(virman_rec.VirKind == ZXC.VirmanEnum.NET /*virman_rec.Ziro2 != standardGovernmentZiroRacun*/) // TODO: !!! 
      {
         if(virman_rec.Prim2.IsEmpty()                           ) ReportBadSEPAdata(virRbr, virman_rec.Prim1, "Ulica", virman_rec.Prim2);
         if(virman_rec.Prim3.IsEmpty() || virman_rec.Prim3 == " ") ReportBadSEPAdata(virRbr, virman_rec.Prim1, "Grad" , virman_rec.Prim3);
      }
   }

   private string ReportBadSEPAdata(int virRbr, string virmanIdentifikator, string dataName, string dataValue)
   {
      string message = string.Format("TrnRbr {3}. {0}\n\n{1} - neispravan podatak: [{2}]", virmanIdentifikator, dataName, dataValue, virRbr);

      ZXC.aim_emsg(MessageBoxIcon.Warning, message);

      return message;
   }

}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptP_ListaBanka : RptP_Virmani
{
   public RptP_ListaBanka(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter) : 
                     base(               _reportDocument,                                 externTblChooser,        _reportName,                    _rptFilter)
   {
      ZNPkind = ZXC.ZNP_Kind.Placa_Specifikacija; // TODO: iz filtera ako ipak neko hoce ono datoteka s placom za internet bankarstvo (ZXC.ZNP_Kind.Placa_ZNP) 
   }

   public static bool IsFilterWellFormed(PlacaReportUC reportUC)
   {
      bool OK = true;
      VvRpt_Placa_Filter filter = reportUC.TheRptFilter;

      if(filter.BankaCd.IsZero())
      {
         ZXC.aim_emsg("Molim, zadajte BANKU.");
         return false;
      }
      return (OK);
   }
}

public class RptP_ListaBankaOLD : VvPlacaReport
{
   #region Fieldz

   enum BankaEnum { ZABA, PBZ, SPLIT, UNKNOWN } 

   BankaEnum bankaEnum = BankaEnum.UNKNOWN;

   private Dictionary<string, VvImpExp.ImpExpField> Lead1_Dict { get; set; }
   private Dictionary<string, VvImpExp.ImpExpField> Lead2_Dict { get; set; } // ovaj treba Splitskoj banci 
   private Dictionary<string, VvImpExp.ImpExpField> Trans_Dict { get; set; }

   #endregion Fieldz

   ZbrojniNalogZaPrijenos TheZNP;
   DateTime               ZnpDate;


   public RptP_ListaBankaOLD(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter) : base(_reportDocument, externTblChooser, _reportName, _rptFilter)
   {
      IsForExport = true;

      TheZNP = null;
   }

   public static bool IsFilterWellFormed(PlacaReportUC reportUC)
   {
      bool OK = true;
      VvRpt_Placa_Filter filter = reportUC.TheRptFilter;

      if(filter.BankaCd.IsZero())
      {
         ZXC.aim_emsg("Molim, zadajte BANKU.");
         return false;
      }


      return (OK);
   }

   public override string ExportFileName 
   { 
#if Bef2015
      get 
      {
         if(RptFilter.BankaNaziv.ToUpper().Contains("ZAGREB"))
         {
            this.bankaEnum = BankaEnum.ZABA;
            return "PL" + ZXC.ValOrEmpty_ddMMyyDateTime_AsText(RptFilter.VirDatePod) + ".TXT";
         }
         else if(RptFilter.BankaNaziv.ToUpper().Contains("PRIVREDNA"))
         {
            this.bankaEnum = BankaEnum.PBZ;

            return ZXC.CURR_prjkt_rec.Ticker + ".TEK";
         }
         else if(RptFilter.BankaNaziv.ToUpper().Contains("SPLITSKA"))
         {
            this.bankaEnum = BankaEnum.SPLIT;

            return "DOHODAK.DAT";
         }
         else
         {
            this.bankaEnum = BankaEnum.UNKNOWN;

            return "Banka [" + RptFilter.BankaNaziv + "] jos neobrađena!!!";
         }
      }
#endif 

      get 
      {
         string znpDate4fName, fNamePreffix;

         if(TheVirmanList[0].DatePodnos != DateTime.MinValue) ZnpDate = TheVirmanList[0].DatePodnos;
         else                                                 ZnpDate = DateTime.Today;

         znpDate4fName = ZXC.ValOrEmpty_YyyyMMddDateTime_AsText(ZnpDate);

         fNamePreffix = "UN";

         return fNamePreffix + znpDate4fName + ".txt";
      } 
   }

   public override bool ExecuteExport(string fileName)
   {
      DS_Placa.virmanDataTable virmanTable = ds_PlacaReport.virman;

    //using(StreamWriter sw = new StreamWriter(ExportFileName, false, Encoding.GetEncoding(1250)))
      using(StreamWriter sw = new StreamWriter(fileName, false, Encoding.GetEncoding(1250)))
      {
         switch(bankaEnum)
         {
            case BankaEnum.ZABA : SetAndDump_ZABA (sw, virmanTable); break;
            case BankaEnum.PBZ  : SetAndDump_PBZ  (sw, virmanTable); break;
            case BankaEnum.SPLIT: SetAndDump_SPLIT(sw, virmanTable); break;

            default: ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "ExecuteExport: neobradjena banka."); break;
         }


      }

      ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Gotovo.\n\nKreirao datoteku\n\n" + fileName + "\n\nBroj djelatnika: " + virmanTable.Rows.Count +

         "\n\nIznos neto-a: " + virmanTable.Sum(vir => vir.money).ToStringVv() + " Kn");

      return true;
   }

   private void SetAndDump_ZABA (StreamWriter sw, DS_Placa.virmanDataTable virmanTable)
   {
      #region Slog Fields Initialization

      VvImpExp.ImpExpField[] Lead1_Fields = new VvImpExp.ImpExpField[]
      {  
         new VvImpExp.ImpExpField("VrSloga" ,   1,   1,  3), 
         new VvImpExp.ImpExpField("Prazno01",   2,   3,  7), 
         new VvImpExp.ImpExpField("StatBr"  ,   3,  10,  7),
         new VvImpExp.ImpExpField("Datum"   ,   4,  17,  6), 
         new VvImpExp.ImpExpField("Prazno02",   5,  23, 13), 
         new VvImpExp.ImpExpField("MoneySum",   6,  36, 13), 
         new VvImpExp.ImpExpField("TransNum",   7,  49,  4),
         new VvImpExp.ImpExpField("MatBr"   ,   8,  53,  8), 
         new VvImpExp.ImpExpField("PodMatBr",   9,  61,  3), 
         new VvImpExp.ImpExpField("Prazno03",  10,  64,  6), 
         new VvImpExp.ImpExpField("Poslovn" ,  11,  70,  6), 
         new VvImpExp.ImpExpField("Asterisk",  12,  76,  1), 
         new VvImpExp.ImpExpField("Prazno 4",  13,  77,  3), 
      };

      VvImpExp.ImpExpField[] Trans_Fields = new VvImpExp.ImpExpField[]
      {  
         new VvImpExp.ImpExpField("VrSloga"  ,   1,   1,  3), 
         new VvImpExp.ImpExpField("Prazno01" ,   2,   3,  7), 
         new VvImpExp.ImpExpField("RefBr"    ,   3,  10,  6),
         new VvImpExp.ImpExpField("Datum"    ,   4,  16,  6), 
         new VvImpExp.ImpExpField("VrPosla"  ,   5,  22,  2), 
         new VvImpExp.ImpExpField("VrPrometa",   6,  24,  2), 
         new VvImpExp.ImpExpField("Money"    ,   7,  26, 11),
         new VvImpExp.ImpExpField("BrTekRn"  ,   8,  37, 10), 
         new VvImpExp.ImpExpField("Prazno02" ,   9,  47,  6), 
         new VvImpExp.ImpExpField("MatBr"    ,  10,  53,  8), 
         new VvImpExp.ImpExpField("PodMatBr" ,  11,  61,  3), 
         new VvImpExp.ImpExpField("Prazno03" ,  12,  64, 12), 
         new VvImpExp.ImpExpField("Asterisk" ,  13,  76,  1), 
         new VvImpExp.ImpExpField("Prazno 4" ,  14,  77,  3), 
      };

      Lead1_Dict = VvImpExp.CreateDictionary(Lead1_Fields, "Lead1", 80 /*82*/);
      Trans_Dict = VvImpExp.CreateDictionary(Trans_Fields, "Trans", 80 /*82*/);

      #endregion Slog Fields

      #region Set & Dump Lead1_Dict Data

      /* 01 */ Lead1_Dict["VrSloga" ].FldValue = "030";
      /* 02 */ Lead1_Dict["Prazno01"].FldValue = "0000000";
      /* 03 */ Lead1_Dict["StatBr"  ].FldValue = "3234495";
      /* 04 */ Lead1_Dict["Datum"   ].FldValue = ZXC.ValOrEmpty_ddMMyyDateTime_AsText(RptFilter.VirDatePod);
      /* 05 */ Lead1_Dict["Prazno02"].FldValue = "0000000000000";
      /* 06 */ Lead1_Dict["MoneySum"].SetDecimalFldValue(virmanTable.Sum(vir => vir.money));
      /* 07 */ Lead1_Dict["TransNum"].SetIntgerFldValue(virmanTable.Rows.Count);
      /* 08 */ Lead1_Dict["MatBr"   ].FldValue = ZXC.CURR_prjkt_rec.Matbr;
      /* 09 */ Lead1_Dict["PodMatBr"].FldValue = "000";
      /* 10 */ Lead1_Dict["Prazno03"].FldValue = "000000";
      /* 11 */ Lead1_Dict["Poslovn" ].FldValue = virmanTable[0].statOb;
      /* 12 */ Lead1_Dict["Asterisk"].FldValue = "*";
      /* 13 */ Lead1_Dict["Prazno 4"].FldValue = "   ";

      VvImpExp.DumpFields(sw, Lead1_Fields);

      #endregion Set & Dump Lead1_Dict Data

      foreach(DS_Placa.virmanRow virmanRow in virmanTable.Rows)
      {
         #region Set & Dump Trans_Dict Data

         /* 01 */ Trans_Dict["VrSloga"  ].FldValue = "031";
         /* 02 */ Trans_Dict["Prazno01" ].FldValue = "0000000";
         /* 03 */ Trans_Dict["RefBr"    ].FldValue = "000000";
         /* 04 */ Trans_Dict["Datum"    ].FldValue = ZXC.ValOrEmpty_ddMMyyDateTime_AsText(RptFilter.VirDatePod);
         /* 05 */ Trans_Dict["VrPosla"  ].FldValue = "01";
         /* 06 */ Trans_Dict["VrPrometa"].FldValue = "50";
         /* 07 */ Trans_Dict["Money"    ].SetDecimalFldValue(virmanRow.money);
         /* 08 */ Trans_Dict["BrTekRn"  ].FldValue = virmanRow.pnbo;
         /* 09 */ Trans_Dict["Prazno02" ].FldValue = "000000";
         /* 10 */ Trans_Dict["MatBr"    ].FldValue = ZXC.CURR_prjkt_rec.Matbr;
         /* 11 */ Trans_Dict["PodMatBr" ].FldValue = "000";
         /* 12 */ Trans_Dict["Prazno03" ].FldValue = "000000000000";
         /* 13 */ Trans_Dict["Asterisk" ].FldValue = "*";
         /* 14 */ Trans_Dict["Prazno 4" ].FldValue = "   ";

         VvImpExp.DumpFields(sw, Trans_Fields);

         #endregion Set & Dump Trans_Dict Data
      }
   }

   private void SetAndDump_PBZ  (StreamWriter sw, DS_Placa.virmanDataTable virmanTable)
   {
      #region Slog Fields Initialization

      VvImpExp.ImpExpField[] Lead1_Fields = new VvImpExp.ImpExpField[]
      {  
         new VvImpExp.ImpExpField("VrSloga" ,   1,   1,  1), 
         new VvImpExp.ImpExpField("Ziro"    ,   2,   1, 17), 
         new VvImpExp.ImpExpField("Prazno1" ,   3,  18,  2),
         new VvImpExp.ImpExpField("Prazno2" ,   4,  20, 22), 
         new VvImpExp.ImpExpField("Prazno3" ,   5,  42,  3), 
      };

      VvImpExp.ImpExpField[] Lead2_Fields = new VvImpExp.ImpExpField[]
      {  
         new VvImpExp.ImpExpField("VrSloga" ,   1,   0,  1), 
         new VvImpExp.ImpExpField("Jedan"   ,   2,   0,  1), 
         new VvImpExp.ImpExpField("TrnCount",   3,   0,  5),
         new VvImpExp.ImpExpField("UkMoney" ,   4,   0, 15), 
         new VvImpExp.ImpExpField("Datum"   ,   5,   0,  8), 
         new VvImpExp.ImpExpField("PJ"      ,   6,   0,  2), 
         new VvImpExp.ImpExpField("POS"     ,   7,   0,  3), 
         new VvImpExp.ImpExpField("Prazno1" ,   8,   0, 10), 
      };

      VvImpExp.ImpExpField[] Trans_Fields = new VvImpExp.ImpExpField[]
      {  
         new VvImpExp.ImpExpField("VrSloga" ,   1,   0,  1), 
         new VvImpExp.ImpExpField("RedBr"   ,   2,   0,  5), 
         new VvImpExp.ImpExpField("BrTekRn",    3,   0, 11),
         new VvImpExp.ImpExpField("Money"   ,   4,   0, 15), 
         new VvImpExp.ImpExpField("Prazno01",   5,   0, 13), 
      };

      Lead1_Dict = VvImpExp.CreateDictionary(Lead1_Fields, "Lead1", 45 );
      Lead2_Dict = VvImpExp.CreateDictionary(Lead2_Fields, "Lead2", 45 );
      Trans_Dict = VvImpExp.CreateDictionary(Trans_Fields, "Trans", 45 );

      #endregion Slog Fields

      #region Set & Dump Lead1&2_Dict Data

      /* 01 */ Lead1_Dict["VrSloga" ].FldValue = "0";
      /* 02 */ Lead1_Dict["Ziro"    ].FldValue = ZXC.CURR_prjkt_rec.Ziro1.Replace("-", "");
      /* 03 */ Lead1_Dict["Prazno1" ].FldValue = "  ";
      /* 04 */ Lead1_Dict["Prazno2" ].FldValue = "                      ";
      /* 05 */ Lead1_Dict["Prazno3" ].FldValue = "   ";

      VvImpExp.DumpFields(sw, Lead1_Fields);

      /* 01 */ Lead2_Dict["VrSloga" ].FldValue = "9";
      /* 02 */ Lead2_Dict["Jedan"]   .FldValue = "1";
      /* 03 */ Lead2_Dict["TrnCount"].SetIntgerFldValue(virmanTable.Rows.Count);
      /* 04 */ Lead2_Dict["UkMoney"] .SetDecimalFldValue(virmanTable.Sum(vir => vir.money));
      /* 05 */ Lead2_Dict["Datum"]   .FldValue = ZXC.ValOrEmpty_ddMMyyDateTime_AsText(RptFilter.VirDatePod);
      /* 06 */ Lead2_Dict["PJ"]      .FldValue = "07";
      /* 07 */ Lead2_Dict["POS"]     .FldValue = "181";
      /* 08 */ Lead2_Dict["Prazno1" ].FldValue = "          ";

      VvImpExp.DumpFields(sw, Lead2_Fields);

      #endregion Set & Dump Lead1_Dict Data

      int i = 0;

      foreach(DS_Placa.virmanRow virmanRow in virmanTable.Rows)
      {
         #region Set & Dump Trans_Dict Data

         /* 01 */ Trans_Dict["VrSloga"  ].FldValue = "1";
         /* 02 */ Trans_Dict["RedBr"    ].SetIntgerFldValue(++i);
         /* 03 */ Trans_Dict["BrTekRn"  ].FldValue = virmanRow.pnbo;
         /* 04 */ Trans_Dict["Money"    ].SetDecimalFldValue(virmanRow.money);
         /* 05 */ Trans_Dict["Prazno01" ].FldValue = "             ";

         VvImpExp.DumpFields(sw, Trans_Fields);

         #endregion Set & Dump Trans_Dict Data
      }
   }

   private void SetAndDump_SPLIT(StreamWriter sw, DS_Placa.virmanDataTable virmanTable)
   {
      #region Slog Fields Initialization

      VvImpExp.ImpExpField[] Lead1_Fields = new VvImpExp.ImpExpField[]
      {  
         new VvImpExp.ImpExpField("VrUplat",   1,   0,  2), 
         new VvImpExp.ImpExpField("Datum"  ,   2,   0,  6), 
         new VvImpExp.ImpExpField("UkMoney",   3,   0, 15),
      };

      VvImpExp.ImpExpField[] Lead2_Fields = new VvImpExp.ImpExpField[]
      {  
         new VvImpExp.ImpExpField("VrUplat",   1,   0,  2), 
         new VvImpExp.ImpExpField("NazivPl",   2,   0, 25), 
      };

      VvImpExp.ImpExpField[] Trans_Fields = new VvImpExp.ImpExpField[]
      {  
         new VvImpExp.ImpExpField("BrTekRn" ,   1,   0, 11), 
         new VvImpExp.ImpExpField("Money"   ,   2,   0, 15), 
         new VvImpExp.ImpExpField("Predznak",   3,   0,  1),
      };

      Lead1_Dict = VvImpExp.CreateDictionary(Lead1_Fields, "Lead1", 23);
      Lead2_Dict = VvImpExp.CreateDictionary(Lead2_Fields, "Lead2", 27);
      Trans_Dict = VvImpExp.CreateDictionary(Trans_Fields, "Trans", 27);

      #endregion Slog Fields

      #region Set & Dump Lead1&2_Dict Data

      /* 01 */ Lead1_Dict["VrUplat"].FldValue = "ST";
      /* 02 */ Lead1_Dict["Datum"  ].FldValue = ZXC.ValOrEmpty_ddMMyyDateTime_AsText(RptFilter.VirDatePod);
      /* 03 */ Lead1_Dict["UkMoney"].SetDecimalFldValue(virmanTable.Sum(vir => vir.money));

      VvImpExp.DumpFields(sw, Lead1_Fields);

      /* 01 */ Lead2_Dict["VrUplat"].FldValue = "ST";
      /* 02 */ Lead2_Dict["NazivPl"].FldValue = ZXC.CURR_prjkt_rec.Naziv;

      VvImpExp.DumpFields(sw, Lead2_Fields);

      #endregion Set & Dump Lead1_Dict Data

      foreach(DS_Placa.virmanRow virmanRow in virmanTable.Rows)
      {
         #region Set & Dump Trans_Dict Data

         /* 01 */ Trans_Dict["BrTekRn"  ].FldValue = virmanRow.pnbo;
         /* 02 */ Trans_Dict["Money"    ].SetDecimalFldValue(virmanRow.money);
         /* 03 */ Trans_Dict["Predznak" ].FldValue = " ";

         VvImpExp.DumpFields(sw, Trans_Fields);

         #endregion Set & Dump Trans_Dict Data
      }
   }

}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptP_GodObrPoreza : VvPlacaReport
{
   public RptP_GodObrPoreza(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter) : base(_reportDocument, externTblChooser, _reportName, _rptFilter)
   {
      IsForExport         = false;
      VvLet_TT_RRiPP_Only = true;
   }
}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptP_RAD1 : VvPlacaReport
{
   public RptP_RAD1(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter) : base(_reportDocument, externTblChooser, _reportName, _rptFilter)
   {
      IsForExport         = false;
      // TODO15: 
      VvLet_TT_RRiPP_Only = true;
   }
}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptP_RAD1_G : VvPlacaReport
{
   public RptP_RAD1_G(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter) : base(_reportDocument, externTblChooser, _reportName, _rptFilter)
   {
      IsForExport         = false;
      VvLet_TT_RRiPP_Only = true;
   }
}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptP_NeoporezPrimici: RptP_JOPPD
{
   public RptP_NeoporezPrimici(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter) : base(_reportDocument, externTblChooser, _reportName, _rptFilter)
   {
      IsForExport = false;

      ZXC.luiListaNeoporPrim.LazyLoad();

   }
}

//[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+]-[+] 

public class RptP_PersonMaticniPodaci: VvPlacaReport
{
   public RptP_PersonMaticniPodaci(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter) : base(_reportDocument, externTblChooser, _reportName, _rptFilter)
   {
      IsForExport = false;
   }
}