using System;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using Vektor.Reports.RIZ;
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
using XSqlDataReader = MySql.Data.MySqlClient.MySqlDataReader;
using XSqlException  = MySql.Data.MySqlClient.MySqlException;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
#endif

public abstract partial class VvMixerReport : VvReport
{
   #region Legacy stuff

   public override DataSet VirtualUntypedDataSet { get { return null; } }

   protected ReportDocument reportDocument;

   public override ReportDocument VirtualReportDocument { get { return reportDocument; } }

   public override XSqlCommand GetReportCommand(XSqlConnection conn)
   {
      return null; // "We don't need Mixer report command. We use 'Lists<>'!";
   }

   #endregion Legacy stuff

   #region Constructor and some propertiez

   public override VvRptFilter VirtualRptFilter { get { return this.RptFilter; } }

   private   VvRpt_Mix_Filter rptFilter;
   protected VvRpt_Mix_Filter RptFilter
   {
      get { return this.rptFilter; }
      set {        this.rptFilter = value; }
   }

   public ZXC.MIX_FilterStyle FilterStyle;

   public List<Artikl> TheArtiklList { get; set; }
   public List<Kupdob> TheKupdobList { get; set; }
   public List<Prjkt > ThePrjktList  { get; set; }
   public List<Mixer>  TheMixerList  { get; set; }
   public List<Xtrans> TheXtransList { get; set; }
   public List<Xtrano> TheXtranoList { get; set; }

   public List<VvReportSourceUtil> TheDeviznaSumaList { get; set; }

   public List<VirmanStruct>  TheVirmanList   { get; set; }
   public List<GFI_TSI_BilancaRow> TheGFI_TSI_BilancaRows { get; set; }

   public List<VvManyDecimalsReportSourceRow> TheManyDecimalsList { get; set; }

   public VvMixerReport() { }

   public VvMixerReport(ReportDocument _reportDocument, string _reportName, VvRpt_Mix_Filter _rptFilter, 
      bool _rptNeeds_Mixer, 
      bool _rptNeeds_Xtrans, 
      bool _rptNeeds_Xtrano,
      bool _rptNeeds_Kupdob,
      bool _rptNeeds_Artikl, 
      bool _rptNeeds_Prjkt) 
   {
      this.reportDocument = _reportDocument;
      this.RptFilter      = _rptFilter;

      ReportNeeds_Mixer_List      = _rptNeeds_Mixer;
      ReportNeeds_Xtrans_List     = _rptNeeds_Xtrans;
      ReportNeeds_Xtrano_List     = _rptNeeds_Xtrano;
      ReportNeeds_Kupdob_List_Mix = _rptNeeds_Kupdob;
      ReportNeeds_Artikl_List_Mix = _rptNeeds_Artikl;
      ReportNeeds_Prjkt_List_Mix  = _rptNeeds_Prjkt;

      VvUserControl theUC = ZXC.TheVvForm.TheVvUC;

      if(ReportNeeds_Mixer_List        ) TheMixerList         = new List<Mixer        >( );
      if(ReportNeeds_Xtrans_List       ) TheXtransList        = new List<Xtrans       >( );
      if(ReportNeeds_Xtrano_List       ) TheXtranoList        = new List<Xtrano       >( );
      if(ReportNeeds_Artikl_List_Mix   ){TheArtiklList        = new List<Artikl       >( ); if(VvUserControl.ArtiklSifrar == null) theUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name); }
      if(ReportNeeds_Kupdob_List_Mix   ){TheKupdobList        = new List<Kupdob       >( ); if(VvUserControl.KupdobSifrar == null) theUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name); }
      if(ReportNeeds_Prjkt_List_Mix    ){ThePrjktList         = new List<Prjkt        >(1); ThePrjktList.Add(ZXC.CURR_prjkt_rec); }

   }

   #endregion Constructor and some propertiez
}

#region VirmanDUC's PrintRecordReports for Single Mixer record (one or many Virman)

public class RptX_Virman : VvMixerReport
{
   public RptX_Virman(ReportDocument _reportDocument, string _reportName, VirmanDocFilter _rptFilter) : base(_reportDocument, _reportName, _rptFilter, 
      false, // ReportNeeds_Mixer_List  
      true , // ReportNeeds_Xtrans_List 
      false, // ReportNeeds_Xtrano_List 
      false, // ReportNeeds_Kupdob_List_Mix
      false, // ReportNeeds_Artikl_List_Mix
      false) // PRJKT
   {
      IsForExport = true;

      IsSEPA = (RptFilter as VirmanDocFilter).PrintVirZnp == VirmanDocFilter.PrintVirZnpEnum.SEPA;
   }

   private bool IsSEPA { get; set; }

   public override int FillMixerReportLists()
   {
      TheVirmanList = new List<VirmanStruct>(); 

      VirmanDUC theDUC = ((VirmanDocFilter)RptFilter).theDUC;

      Mixer mixer_rec = (Mixer)theDUC.mixer_rec.CreateNewRecordAndCloneItComplete();

      //TheMixerList.Add(mixer_rec);

      TheXtransList = mixer_rec.Transes; if(TheXtransList.Count.IsZero()) TheXtransList.Add(new Xtrans());

      //TheKupdobList.Add(VvUserControl.KupdobSifrar.SingleOrDefault(kpdb => kpdb.KupdobCD == mixer_rec.KupdobCD));

      TheXtransList.ForEach(delegate(Xtrans xtrans)
      {
         TheVirmanList.Add(new VirmanStruct(xtrans));
      });

      return TheVirmanList.Count;
   }

   ZbrojniNalogZaPrijenos TheZNP;
   DateTime               ZnpDate;

   public override string ExportFileName 
   { 
      get 
      {
         if(IsSEPA) return ExportFileName_SEPA;

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

   private string ExportFileName_SEPA
   {
      get
      {
         string yyyymmdd, fNamePreffix, fNameSuffix, sep = ".";

         if(TheVirmanList[0].DatePodnos != DateTime.MinValue) ZnpDate = TheVirmanList[0].DatePodnos;
         else                                                 ZnpDate = DateTime.Today;

         yyyymmdd = ZXC.ValOrEmpty_YyyyMMddDateTime_AsText(ZnpDate);

         fNamePreffix = "UN";
       //fNameSuffix  = RptFilter.DokNumChecked ? RptFilter.DokNum.ToString("0000") : ((int)RptFilter.VirmanGroup + 1).ToString("0000") /*"0001"*/;
         fNameSuffix  =                                                                                                                   "1001"  ;

         return fNamePreffix + sep + ZXC.CURR_prjkt_rec.Oib + sep + yyyymmdd + sep + fNameSuffix + sep + "xml";
      }
   }

   public override bool ExecuteExport(string fullPathFileName)
   {
      if(IsSEPA) return ExecuteExport_SEPA(fullPathFileName);
      
      TheZNP = new ZbrojniNalogZaPrijenos(fullPathFileName, ExportFileName, ZnpDate, TheVirmanList, ZXC.ZNP_Kind.Classic); 
      
      foreach(VirmanStruct virman_rec in TheVirmanList)
      {
         TheZNP.SetAndDumpZnpLine(virman_rec, ZXC.ZNP_Kind.Classic);
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

   private bool ExecuteExport_SEPA(string fullPathFileName)
   {
      // SEPA_PAIN_001_001_03_to_PAIN_001_001_09

      return RptP_SEPA.ExecuteExportSEPA_001_001_03(fullPathFileName, TheVirmanList, ZnpDate, ZXC.VirmanBtchBookgKind.ALL, false);
    //return RptP_SEPA.ExecuteExportSEPA_001_001_09(fullPathFileName, TheVirmanList, ZnpDate, ZXC.VirmanBtchBookgKind.ALL, false);
   }

   public override bool ExecuteExportValidation(string fileName)
   {
      // SEPA_PAIN_001_001_03_to_PAIN_001_001_09

      return RptP_SEPA.ExecuteExportValidationSEPA_001_001_003(fileName, this);
    //return RptP_SEPA.ExecuteExportValidationSEPA_001_001_009(fileName, this);
   }
}

#endregion VirmanDUC's PrintRecordReports for Single Mixer record (one or many Virman)

#region RptX_GFI_TSI

public class RptX_GFI_TSI : VvMixerReport
{
   public RptX_GFI_TSI (ReportDocument _reportDocument, string _reportName, GFI_TSI_DocFilter _rptFilter) : base(_reportDocument, _reportName, _rptFilter, 
      true , // ReportNeeds_Mixer_List  
      true , // ReportNeeds_Xtrans_List 
      false, // ReportNeeds_Xtrano_List 
      true , // ReportNeeds_Kupdob_List_Mix
      false, // ReportNeeds_Artikl_List_Mix
      false) // PRJKT
   {
   }

   public override int FillMixerReportLists()
   {
      GFI_TSI_DUC theDUC = ((GFI_TSI_DocFilter)RptFilter).theDUC;

      Mixer mixer_rec = (Mixer)theDUC.mixer_rec.CreateNewRecordAndCloneItComplete();

      TheMixerList.Add(mixer_rec);

      TheXtransList = mixer_rec.Transes; if(TheXtransList.Count.IsZero()) TheXtransList.Add(new Xtrans());

      TheGFI_TSI_BilancaRows = theDUC.GFI_TSI_BilancaRows;

      return TheXtransList.Count;
   }
}

#endregion RptX_GFI_TSI

#region RptX_STAT_NPF

public class RptX_STAT_NPF : VvMixerReport
{
   public RptX_STAT_NPF(ReportDocument _reportDocument, string _reportName, Statistika_NPF_DocFilter _rptFilter) : base(_reportDocument, _reportName, _rptFilter, 
      true , // ReportNeeds_Mixer_List  
      true , // ReportNeeds_Xtrans_List 
      false, // ReportNeeds_Xtrano_List 
      true , // ReportNeeds_Kupdob_List_Mix
      false, // ReportNeeds_Artikl_List_Mix
      true) // PRJKT
   {
   }

   public override int FillMixerReportLists()
   {
      Statistika_NPF_DUC theDUC = ((Statistika_NPF_DocFilter)RptFilter).theDUC;

      Mixer mixer_rec = (Mixer)theDUC.mixer_rec.CreateNewRecordAndCloneItComplete();

      TheMixerList.Add(mixer_rec);

      TheXtransList = mixer_rec.Transes; if(TheXtransList.Count.IsZero()) TheXtransList.Add(new Xtrans());

      TheGFI_TSI_BilancaRows = theDUC.GFI_TSI_BilancaRows;

      return TheXtransList.Count;
   }
}

#endregion RptX_GFI_TSI

#region Raster

public class RptX_Raster : VvMixerReport
{
   public RptX_Raster(ReportDocument _reportDocument, string _reportName, RasterDocFilter _rptFilter) : base(_reportDocument, _reportName, _rptFilter, 
      true , // ReportNeeds_Mixer_List  
      true , // ReportNeeds_Xtrans_List 
      false, // ReportNeeds_Xtrano_List 
      true , // ReportNeeds_Kupdob_List_Mix
      false, // ReportNeeds_Artikl_List_Mix
      false) // PRJKT
   {
      IsForExport = false;
   }

   public override int FillMixerReportLists()
   {
      RasterDUC theDUC = ((RasterDocFilter)RptFilter).theDUC;

      Mixer mixer_rec = (Mixer)theDUC.mixer_rec.CreateNewRecordAndCloneItComplete();
      TheMixerList.Add(mixer_rec);

      TheXtransList = mixer_rec.Transes; if(TheXtransList.Count.IsZero()) TheXtransList.Add(new Xtrans());
     
      TheKupdobList = VvUserControl.KupdobSifrar.Join(TheMixerList, mtr => mtr.KupdobCD, mixer => mixer.MtrosCD, (k, f) => k).Distinct().ToList();

      return TheXtransList.Count;
   }
}
public class RptX_RasterB : VvMixerReport
{
   public RptX_RasterB (ReportDocument _reportDocument, string _reportName, RasterBDocFilter _rptFilter) : base(_reportDocument, _reportName, _rptFilter, 
      true , // ReportNeeds_Mixer_List  
      true , // ReportNeeds_Xtrans_List 
      false, // ReportNeeds_Xtrano_List 
      true , // ReportNeeds_Kupdob_List_Mix
      false, // ReportNeeds_Artikl_List_Mix
      false) // PRJKT
   {
      IsForExport = false;
   }

   public override int FillMixerReportLists()
   {
      RasterBDUC theDUC = ((RasterBDocFilter)RptFilter).theDUC;

      Mixer mixer_rec = (Mixer)theDUC.mixer_rec.CreateNewRecordAndCloneItComplete();
      TheMixerList.Add(mixer_rec);

      TheXtransList = mixer_rec.Transes; if(TheXtransList.Count.IsZero()) TheXtransList.Add(new Xtrans());
     
      TheKupdobList = VvUserControl.KupdobSifrar.Join(TheMixerList, mtr => mtr.KupdobCD, mixer => mixer.MtrosCD, (k, f) => k).Distinct().ToList();

      return TheXtransList.Count;
   }
}

#endregion Raster

#region PutNal

public class RptX_PutNal : VvMixerReport
{
   public RptX_PutNal(ReportDocument _reportDocument, string _reportName, PutNalFilter _rptFilter) : base(_reportDocument, _reportName, _rptFilter, 
      true , // ReportNeeds_Mixer_List  
      true , // ReportNeeds_Xtrans_List 
      true , // ReportNeeds_Xtrano_List 
      false, // ReportNeeds_Kupdob_List_Mix
      false, // ReportNeeds_Artikl_List_Mix
      true ) // PRJKT 
   {
      IsForExport = false;
   }

   public override int FillMixerReportLists()
   {
      PutNalDUC theDUC = ((PutNalFilter)RptFilter).theDUC;

      Mixer mixer_rec = (Mixer)theDUC.mixer_rec.CreateNewRecordAndCloneItComplete();

      mixer_rec.R_intA_AsText = ZXC.IntAsText(mixer_rec.IntA);

      mixer_rec.ConvertPutNalInKune();

      TheMixerList.Add(mixer_rec);

      TheXtransList = mixer_rec.Transes ; if(TheXtransList.Count.IsZero()) TheXtransList.Add(new Xtrans());
      TheXtranoList = mixer_rec.Transes2; if(TheXtranoList.Count.IsZero()) TheXtranoList.Add(new Xtrano());

      TheDeviznaSumaList = mixer_rec.GetDevizniToPayList();

      return TheXtransList.Count;
   }
}

#endregion PutNal

#region Zahtjev

public class RptX_Zahtjev : VvMixerReport
{
   public RptX_Zahtjev(ReportDocument _reportDocument, string _reportName, ZahtjeviFilter _rptFilter) : base(_reportDocument, _reportName, _rptFilter, 
      true , // ReportNeeds_Mixer_List  
      true , // ReportNeeds_Xtrans_List 
      false, // ReportNeeds_Xtrano_List 
      true , // ReportNeeds_Kupdob_List_Mix
      true , // ReportNeeds_Artikl_List_Mix
      true) // PRJKT  
   {
      IsForExport = false;
   }

   public override int FillMixerReportLists()
   {
      ZahtjeviDUC theDUC = ((ZahtjeviFilter)RptFilter).theDUC;

      Mixer mixer_rec = (Mixer)theDUC.mixer_rec.CreateNewRecordAndCloneItComplete();

      TheMixerList.Add(mixer_rec);

      TheXtransList = mixer_rec.Transes ; if(TheXtransList.Count.IsZero()) TheXtransList.Add(new Xtrans());
           
      TheArtiklList = VvUserControl.ArtiklSifrar.Join(TheXtransList, art  => art.ArtiklCD,  xtran => xtran.T_artiklCD, (a, x) => a).Distinct().ToList();

      return TheXtransList.Count;
   }
}

public class RptX_ZahtjevRNM : VvMixerReport
{
   public RptX_ZahtjevRNM(ReportDocument _reportDocument, string _reportName, ZahtjevRNMFilter _rptFilter) : base(_reportDocument, _reportName, _rptFilter, 
      true , // ReportNeeds_Mixer_List  
      true , // ReportNeeds_Xtrans_List 
      false, // ReportNeeds_Xtrano_List 
      true , // ReportNeeds_Kupdob_List_Mix
      true , // ReportNeeds_Artikl_List_Mix
      true) // PRJKT  
   {
      IsForExport = false;
   }

   public override int FillMixerReportLists()
   {
      ZahtjevRNMDUC theDUC = ((ZahtjevRNMFilter)RptFilter).theDUC;

      Mixer mixer_rec = (Mixer)theDUC.mixer_rec.CreateNewRecordAndCloneItComplete();

      TheMixerList.Add(mixer_rec);

      TheXtransList = mixer_rec.Transes ; if(TheXtransList.Count.IsZero()) TheXtransList.Add(new Xtrans());
           
      TheArtiklList = VvUserControl.ArtiklSifrar.Join(TheXtransList, art  => art.ArtiklCD,  xtran => xtran.T_artiklCD, (a, x) => a).Distinct().ToList();

      return TheXtransList.Count;
   }
}

#endregion Zahtjev

#region SMD

public class RptX_SMD : VvMixerReport
{
   public RptX_SMD(ReportDocument _reportDocument, string _reportName, SmdFilter _rptFilter) : base(_reportDocument, _reportName, _rptFilter,
         true , // ReportNeeds_Mixer_List  
         true , // ReportNeeds_Xtrans_List 
         false, // ReportNeeds_Xtrano_List 
         false, // ReportNeeds_Kupdob_List_Mix
         false, // ReportNeeds_Artikl_List_Mix
         false) // PRJKT 
   {
      IsForExport = false;
   }

   public override int FillMixerReportLists()
   {
      SmdDUC theDUC = ((SmdFilter)RptFilter).theDUC;

      Mixer mixer_rec = (Mixer)theDUC.mixer_rec.CreateNewRecordAndCloneItComplete();

      TheMixerList.Add(mixer_rec);

      TheXtransList = mixer_rec.Transes; if(TheXtransList.Count.IsZero()) TheXtransList.Add(new Xtrans());

      return TheXtransList.Count;
   }
}

#endregion PutNal

#region Evidencija

public class RptX_Evidencija : VvMixerReport
{
   public RptX_Evidencija(ReportDocument _reportDocument, string _reportName, EvidencijaFilter _rptFilter): base(_reportDocument, _reportName, _rptFilter,
         true , // ReportNeeds_Mixer_List  
         false, // ReportNeeds_Xtrans_List 
         false, // ReportNeeds_Xtrano_List 
         false, // ReportNeeds_Kupdob_List_Mix
         false, // ReportNeeds_Artikl_List_Mix
         false) // PRJKT 
   {
      IsForExport = false;
   }

   public override int FillMixerReportLists()
   {
      EvidencijaDUC theDUC = ((EvidencijaFilter)RptFilter).theDUC;

      Mixer mixer_rec = (Mixer)theDUC.mixer_rec.CreateNewRecordAndCloneItComplete();

      TheMixerList.Add(mixer_rec);

     // TheXtransList = mixer_rec.Transes; if(TheXtransList.Count.IsZero()) TheXtransList.Add(new Xtrans());

      return 0;// TheXtransList.Count;
   }
}


#endregion Evidencija

#region URZ

public class RptX_Urudzbeni : VvMixerReport
{
   public RptX_Urudzbeni(ReportDocument _reportDocument, string _reportName, UrudzbeniFilter _rptFilter): base (_reportDocument, _reportName, _rptFilter,
         true, // ReportNeeds_Mixer_List  
         true, // ReportNeeds_Xtrans_List 
         false, // ReportNeeds_Xtrano_List 
         false, // ReportNeeds_Kupdob_List_Mix
         false, // ReportNeeds_Artikl_List_Mix
         false) // PRJKT 
   {
      IsForExport = false;
   }
   public override int FillMixerReportLists()
   {
      UrudzbeniDUC theDUC = ((UrudzbeniFilter)RptFilter).theDUC;

      Mixer mixer_rec = (Mixer)theDUC.mixer_rec.CreateNewRecordAndCloneItComplete();

      TheMixerList.Add(mixer_rec);

      TheXtransList = mixer_rec.Transes; if(TheXtransList.Count.IsZero()) TheXtransList.Add(new Xtrans());

      return /*0;//*/ TheXtransList.Count;
   }
}

#endregion URZ

#region RVR & MVR

public class RptX_RVR : VvMixerReport
{
   public RptX_RVR(ReportDocument _reportDocument, string _reportName, RvrFilter _rptFilter) : base(_reportDocument, _reportName, _rptFilter,
         true , // ReportNeeds_Mixer_List  
         true , // ReportNeeds_Xtrans_List 
         false, // ReportNeeds_Xtrano_List 
         false, // ReportNeeds_Kupdob_List_Mix
         false, // ReportNeeds_Artikl_List_Mix
         false) // PRJKT 
   {
      IsForExport = false;
   }

   public override int FillMixerReportLists()
   {
      RvrDUC theDUC = ((RvrFilter)RptFilter).theDUC;

      Mixer mixer_rec = (Mixer)theDUC.mixer_rec.CreateNewRecordAndCloneItComplete();

      TheMixerList.Add(mixer_rec);

      TheXtransList = mixer_rec.Transes; if(TheXtransList.Count.IsZero()) TheXtransList.Add(new Xtrans());

      return TheXtransList.Count;
   }
}

#endregion RVR & MVR

#region IRV

public class RptX_InterniRvr : VvMixerReport
{
   public RptX_InterniRvr(ReportDocument _reportDocument, string _reportName, InterniRVRFilter _rptFilter) : base(_reportDocument, _reportName, _rptFilter,
         true , // ReportNeeds_Mixer_List  
         true , // ReportNeeds_Xtrans_List 
         false, // ReportNeeds_Xtrano_List 
         false, // ReportNeeds_Kupdob_List_Mix
         false, // ReportNeeds_Artikl_List_Mix
         false) // PRJKT 
   {
      IsForExport = false;
   }

   public override int FillMixerReportLists()
   {
      InterniRvrDUC theDUC = ((InterniRVRFilter)RptFilter).theDUC;

      Mixer mixer_rec = (Mixer)theDUC.mixer_rec.CreateNewRecordAndCloneItComplete();

      TheMixerList.Add(mixer_rec);

      TheXtransList = mixer_rec.Transes; if(TheXtransList.Count.IsZero()) TheXtransList.Add(new Xtrans());

      return TheXtransList.Count;
   }
}

#endregion IRV

#region IRV

public class RptX_AvrReport : VvMixerReport
{
   public RptX_AvrReport(ReportDocument _reportDocument, string _reportName, AvrFilter _rptFilter)
      : base(_reportDocument, _reportName, _rptFilter,
         true , // ReportNeeds_Mixer_List  
         true , // ReportNeeds_Xtrans_List 
         false, // ReportNeeds_Xtrano_List 
         false, // ReportNeeds_Kupdob_List_Mix
         false, // ReportNeeds_Artikl_List_Mix
         false) // PRJKT 
   {
      IsForExport = false;
   }

   public override int FillMixerReportLists()
   {
      AvrDUC theDUC = ((AvrFilter)RptFilter).theDUC;

      Mixer mixer_rec = (Mixer)theDUC.mixer_rec.CreateNewRecordAndCloneItComplete();

      TheMixerList.Add(mixer_rec);

      TheXtransList = mixer_rec.Transes; if(TheXtransList.Count.IsZero()) TheXtransList.Add(new Xtrans());

      return TheXtransList.Count;
   }
}

#endregion IRV

#region Mixer Reports (Standard Lists ala Rekapitulacija...)

public class RptX_StandardMixerReport : VvMixerReport
{
   public RptX_StandardMixerReport(ReportDocument _reportDocument, string _reportName, VvRpt_Mix_Filter _rptFilter, ZXC.MIX_FilterStyle _filterStyle, 
      bool _rptNeeds_Mixer, 
      bool _rptNeeds_Xtrans, 
      bool _rptNeeds_Xtrano, 
      bool _needsXtrSO_Join_Mixer,
      bool _rptNeeds_Kupdob,
      bool _rptNeeds_Artikl, 
      bool _rptNeeds_Prjkt) : base(_reportDocument, _reportName, _rptFilter, _rptNeeds_Mixer, _rptNeeds_Xtrans, _rptNeeds_Xtrano, _rptNeeds_Kupdob, _rptNeeds_Artikl, _rptNeeds_Prjkt)  
   {
      FilterStyle = _filterStyle;

      needsXtrSO_Join_Mixer = _needsXtrSO_Join_Mixer;
   }

   public override int FillMixerReportLists()
   {
      if(ReportNeeds_Mixer_List     ) GetMixerList();
      if(ReportNeeds_Xtrans_List    ) GetXtransList();
      if(ReportNeeds_Xtrano_List    ) GetXtranoList();
      if(ReportNeeds_Kupdob_List_Mix) GetMixerKupdobList();
      if(ReportNeeds_Artikl_List_Mix) GetXtransArtiklList();

      if(needsXtrSO_Join_Mixer) Join_XtrSO_And_Mixer();

      return 0;
   }

   private void Join_XtrSO_And_Mixer()
   {
      for(int i = 0; i < TheMixerList.Count; ++i)
      {
         TheMixerList[i].Transes  = TheXtransList.Where(xtrs => xtrs.T_parentID == TheMixerList[i].RecID).ToList();
         if(TheXtranoList != null) TheMixerList[i].Transes2 = TheXtranoList.Where(xtro => xtro.T_parentID == TheMixerList[i].RecID).ToList();
      }
   }

   #region Sam Lokal Propertiz

   private bool needsXtrSO_Join_Mixer { get; set; }

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

   protected DataRowCollection    MixSch   { get { return ZXC.MixerDao.TheSchemaTable.Rows;   } }
   protected MixerDao.MixerCI     MixCI    { get { return ZXC.MixerDao.CI;                    } }
   protected DataRowCollection    XtrSch   { get { return ZXC.XtransDao.TheSchemaTable.Rows;  } }
   protected XtransDao.XtransCI   XtrCI    { get { return ZXC.XtransDao.CI;                   } }
   protected DataRowCollection    XtoSch   { get { return ZXC.XtranoDao.TheSchemaTable.Rows;  } }
   protected XtranoDao.XtranoCI   XtoCI    { get { return ZXC.XtranoDao.CI;                   } }

   #endregion Sam Lokal Propertiz

   protected XSqlConnection TheDbConnection { get { return ZXC.TheVvForm.TheDbConnection;                } }
   protected MixerReportUC  TheReportUC     { get { return ZXC.TheVvForm.TheVvReportUC as MixerReportUC; } }

   protected string MixerOrderBy
   {
      get
      {
         switch(RptFilter.SorterType_Dokument)
         {
            case VvSQL.SorterType.DokDate: return "dokDate , tt, ttNum ";
            case VvSQL.SorterType.DokNum : return "          tt, ttNum ";
            
            default: return "";
         }
      }
   }

   private void GetMixerList()       { VvDaoBase.LoadGenericVvDataRecordList  (TheDbConnection, TheMixerList , RptFilter.FilterMembers, "", MixerOrderBy, false); }
   private void GetXtransList()      { ZXC.XtransDao.LoadManyDocumentsTtranses(TheDbConnection, TheXtransList, RptFilter, "t_serial"); }
   private void GetXtranoList()      { ZXC.XtranoDao.LoadManyDocumentsTtranses(TheDbConnection, TheXtranoList, RptFilter, "t_serial"); }
   private void GetMixerKupdobList() { TheKupdobList = VvUserControl.KupdobSifrar.Join(TheMixerList , kpdb => kpdb.KupdobCD, mixer => mixer.KupdobCD  , (k, m) => k).Distinct().ToList(); }
   private void GetXtransArtiklList(){ TheArtiklList = VvUserControl.ArtiklSifrar.Join(TheXtransList, art  => art.ArtiklCD,  xtran => xtran.T_artiklCD, (a, x) => a).Distinct().ToList(); }

}

public class RptX_EvidencijaRadnogVremena : RptX_StandardMixerReport
{
   public RptX_EvidencijaRadnogVremena(ReportDocument _reportDocument, string _reportName, VvRpt_Mix_Filter _rptFilter, ZXC.MIX_FilterStyle _filterStyle, 
      bool _rptNeeds_Mixer, 
      bool _rptNeeds_Xtrans, 
      bool _rptNeeds_Xtrano, 
      bool _needsXtrSO_Join_Mixer,
      bool _rptNeeds_Kupdob,
      bool _rptNeeds_Artikl,
      bool _rptNeeds_Prjkt)
      : base(_reportDocument, _reportName, _rptFilter, _filterStyle, _rptNeeds_Mixer, _rptNeeds_Xtrans, _rptNeeds_Xtrano, _needsXtrSO_Join_Mixer, _rptNeeds_Kupdob, _rptNeeds_Artikl, _rptNeeds_Prjkt)  
   {
   }

   public override int FillMixerReportLists()
   {
      base.FillMixerReportLists(); // valjda treba 

      #region PersonList

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Person>(null, VvSQL.SorterType.Name);

      var personList = VvUserControl.PersonSifrar.Where(per => per.IsIzuzet == false && (per.IsPlaca || per.IsPoduz));

      if(RptFilter.PersonCD.NotZero()) personList = personList.Where(per => per.PersonCD == RptFilter.PersonCD); // glupo, al' ce raditi 

      #endregion PersonList

      #region variablez

      Xtrans rptXtrans_rec;

      List<Xtrans> rptXtransList = new List<Xtrans>();

      TimeSpan oneDayTS = new TimeSpan(1, 0, 0, 0);

      int startHrs, startMin, endHrs, endMin;

      startHrs = ZXC.CURR_prjkt_rec.RvrOd.Hour  ;
      startMin = ZXC.CURR_prjkt_rec.RvrOd.Minute;
      endHrs   = ZXC.CURR_prjkt_rec.RvrDo.Hour  ;
      endMin   = ZXC.CURR_prjkt_rec.RvrDo.Minute;

      DateTime tmpDateOd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startHrs, startMin, 0);
      DateTime tmpDateDo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, endHrs  , endMin  , 0);

      TimeSpan ts = tmpDateDo.Subtract(tmpDateOd);
      decimal defaultRSati_perDay /*= System.Convert.ToDecimal(ts.TotalHours)*/;

      List<Xtrans> realXtransForPersonForDayList;

      #endregion variablez

      #region The Loops

      foreach(Person person in personList)
      {
         // 17.11.2015.
         DateTime dateOd = (person.DatePri.IsEmpty() || person.DatePri <= RptFilter.DatumOd.Date) ? RptFilter.DatumOd.Date : person.DatePri;
         DateTime dateDo = (person.DateOdj.IsEmpty() || person.DateOdj >= RptFilter.DatumDo.Date) ? RptFilter.DatumDo.Date : person.DateOdj;

         defaultRSati_perDay = System.Convert.ToDecimal(ts.TotalHours);

       //for(DateTime day = RptFilter.DatumOd.Date; day <= RptFilter.DatumDo.Date; day += oneDayTS) 
         for(DateTime day = dateOd           .Date; day <= dateDo           .Date; day += oneDayTS)
         {
            if(RptFilter.DatumOd < ZXC.Date01042015) rptXtrans_rec = GetDefaultXtrans(person, day, startHrs, startMin, endHrs, endMin, defaultRSati_perDay);
            else
            {
               defaultRSati_perDay = person.SkrRV.NotZero() ? person.SkrRV / 5 : defaultRSati_perDay;
               rptXtrans_rec = GetDefaultXtrans_15(person, day, startHrs, startMin, endHrs, endMin, defaultRSati_perDay);
            }
            realXtransForPersonForDayList = TheXtransList.Where(xtr => xtr.T_personCD == person.PersonCD && xtr.T_dokDate == day).ToList();

            foreach(Xtrans realXtrans in realXtransForPersonForDayList)
            {
               if(RptFilter.DatumOd < ZXC.Date01042015) ModifyXtransForRealData   (rptXtrans_rec, realXtrans);
               else                                     ModifyXtransForRealData_15(rptXtrans_rec, realXtrans);
            }

            rptXtransList.Add(rptXtrans_rec);
         }


      }

      #endregion The Loops

      // !!! 
      TheXtransList = rptXtransList;
    //TheXtransList = rptXtransList.OrderBy(x => x.T_personCD).ThenBy(x => x.T_dokDate).ToList();

      return 0;
   }


   private void ModifyXtransForRealData(Xtrans rptXtrans, Xtrans realXtrans)
   {
      switch(realXtrans.T_strA_2)
      {
         case "4a": rptXtrans.RVR_4a = realXtrans.T_kol; break;
         case "4b": rptXtrans.RVR_4b = realXtrans.T_kol; rptXtrans.RVR_4 += realXtrans.T_kol; break; // prekovremeni 
         case "4c": rptXtrans.RVR_4c = realXtrans.T_kol; break;
         case "4d": rptXtrans.RVR_4d = realXtrans.T_kol; break;

         case "5" : rptXtrans.RVR_5  = realXtrans.T_kol; rptXtrans.RVR_4 -= realXtrans.T_kol; rptXtrans.RVR_6 -= realXtrans.T_kol; break;
         case "6" : rptXtrans.RVR_6  = realXtrans.T_kol; rptXtrans.RVR_4 -= realXtrans.T_kol; rptXtrans.RVR_6 -= realXtrans.T_kol; break;
         case "7" : rptXtrans.RVR_7  = realXtrans.T_kol; rptXtrans.RVR_4 -= realXtrans.T_kol; rptXtrans.RVR_6 -= realXtrans.T_kol; break;
         case "8" : rptXtrans.RVR_8  = realXtrans.T_kol; rptXtrans.RVR_4 -= realXtrans.T_kol; rptXtrans.RVR_6 -= realXtrans.T_kol; break;
         case "9" : rptXtrans.RVR_9  = realXtrans.T_kol; rptXtrans.RVR_4 -= realXtrans.T_kol; rptXtrans.RVR_6 -= realXtrans.T_kol; break;
         case "10": rptXtrans.RVR_10 = realXtrans.T_kol; rptXtrans.RVR_4 -= realXtrans.T_kol; rptXtrans.RVR_6 -= realXtrans.T_kol; break;
         case "11": rptXtrans.RVR_11 = realXtrans.T_kol; rptXtrans.RVR_4 -= realXtrans.T_kol; rptXtrans.RVR_6 -= realXtrans.T_kol; break;
         case "12": rptXtrans.RVR_12 = realXtrans.T_kol; rptXtrans.RVR_4 -= realXtrans.T_kol; rptXtrans.RVR_6 -= realXtrans.T_kol; break;
         case "13": rptXtrans.RVR_13 = realXtrans.T_kol; rptXtrans.RVR_4 -= realXtrans.T_kol; rptXtrans.RVR_6 -= realXtrans.T_kol; break;
      }

      if(rptXtrans.RVR_4.IsNegative()) rptXtrans.RVR_4 = 0M;
      if(rptXtrans.RVR_6.IsNegative()) rptXtrans.RVR_6 = 0M;
   }

   private void ModifyXtransForRealData_15(Xtrans rptXtrans, Xtrans realXtrans)
   {
      switch(realXtrans.T_strA_2)
      {
     /////* UkRadVrijeme */case " 4":                                                                                                                                                     break;
     /////* RedovanRad   */case " 5":                                                                                                                                                     break;
         /* ZastojPrekid */case  "6": rptXtrans.RVR_6  = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol;                                                                           break;
         /* TerenskiRad  */case  "7": rptXtrans.RVR_7  = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol;                                                                           break;
         /* Pripravnost  */case  "8": rptXtrans.RVR_8  = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol;                                                                           break;
         /* DvokratniRad */case  "9": rptXtrans.RVR_9  = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol;                                                                           break;
         /* SmjenskiRad  */case "10": rptXtrans.RVR_10 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol;                                                                           break;
         /* DnevniOdmor  */case "11": rptXtrans.RVR_11 = realXtrans.T_kol;                                                                                                                break;
         /* TjedniOdmor  */case "12": rptXtrans.RVR_12 = realXtrans.T_kol;                                                                                                                break;
         /* GodisnjiOdmor*/case "13": rptXtrans.RVR_13 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol; rptXtrans.RVR_11 /*-= 16*/ = 0.00M;                                       break;
         /* Blagdan      */case "14": rptXtrans.RVR_14 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol; rptXtrans.RVR_11 /*-= 16*/ = 0.00M;                                       break;
         /* KratkoBolov  */case "15": rptXtrans.RVR_15 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol; rptXtrans.RVR_11 /*-= 16*/ = 0.00M; rptXtrans.RVR_14 = 0.00M;             break; // 26.10.2015. bolovanje jace od blagdana
         /* DugoBoloanje */case "16": rptXtrans.RVR_16 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol; rptXtrans.RVR_11 /*-= 16*/ = 0.00M; rptXtrans.RVR_14 = 0.00M;             break; // 26.10.2015. bolovanje jace od blagdana
         /* Rodiljni     */case "17": rptXtrans.RVR_17 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol; rptXtrans.RVR_11 /*-= 16*/ = 0.00M; rptXtrans.RVR_14 = 0.00M;             break; // 26.10.2015. bolovanje jace od blagdana
         /* MirovanjeRO  */case "18": rptXtrans.RVR_18 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol; rptXtrans.RVR_11 /*-= 16*/ = 0.00M;                                       break;
         /* PlaceniDopus */case "19": rptXtrans.RVR_19 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol; rptXtrans.RVR_11 /*-= 16*/ = 0.00M;                                       break;
         /* NeplDopust   */case "20": rptXtrans.RVR_20 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol; rptXtrans.RVR_11 /*-= 16*/ = 0.00M;                                       break;
         /* KrivnjaRadnik*/case "21": rptXtrans.RVR_21 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol; rptXtrans.RVR_11 /*-= 16*/ = 0.00M;                                       break;
         /* ZahtjevRadnik*/case "22": rptXtrans.RVR_22 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol; rptXtrans.RVR_11 /*-= 16*/ = 0.00M;                                       break;
         /* Strajk       */case "23": rptXtrans.RVR_23 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol; rptXtrans.RVR_11 /*-= 16*/ = 0.00M;                                       break;
         /* Lockout      */case "24": rptXtrans.RVR_24 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol; rptXtrans.RVR_11 /*-= 16*/ = 0.00M;                                       break;
         /* RadNocu      */case "25": rptXtrans.RVR_25 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol;                                                                           break;
         /* RadBlgNoc    */case "26": rptXtrans.RVR_26 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol; rptXtrans.RVR_14 -= realXtrans.T_kol;                                     break;
         /* RadNedNoc    */case "27": rptXtrans.RVR_27 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol;                                                                           break;
         /* RadBlagdan   */case "28": rptXtrans.RVR_28 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol; rptXtrans.RVR_14 -= realXtrans.T_kol;                                     break;
         /* RadDvokratNed*/case "29": rptXtrans.RVR_29 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol;                                                                           break;
         /* RadNedjeljom */case "30": rptXtrans.RVR_30 = realXtrans.T_kol; rptXtrans.RVR_5 -= realXtrans.T_kol;                                                                           break;
         /* Prekovremeni */case "31": rptXtrans.RVR_31 = realXtrans.T_kol;                                      rptXtrans.RVR_4 += realXtrans.T_kol; rptXtrans.RVR_11 -= realXtrans.T_kol;break;
         /* PrekoNoc     */case "32": rptXtrans.RVR_32 = realXtrans.T_kol;                                      rptXtrans.RVR_4 += realXtrans.T_kol; rptXtrans.RVR_11 -= realXtrans.T_kol;break;
         /* PrekoBlagNed */case "33": rptXtrans.RVR_33 = realXtrans.T_kol;                                      rptXtrans.RVR_4 += realXtrans.T_kol; rptXtrans.RVR_11 -= realXtrans.T_kol;break;
      }

      if(rptXtrans.RVR_5.IsNegative()) rptXtrans.RVR_5 = 0M;

     #region real time

      bool isRealDate = !(realXtrans.T_dateOd.Hour.IsZero() && realXtrans.T_dateDo.Hour.IsZero());
      TimeSpan oneDayTS = new TimeSpan(1, 0, 0, 0);

      int realStartHrs, realStartMin, realEndHrs, realEndMin;

      realStartHrs = realXtrans.T_dateOd.Hour  ;
      realStartMin = realXtrans.T_dateOd.Minute;
      realEndHrs   = realXtrans.T_dateDo.Hour  ;
      realEndMin   = realXtrans.T_dateDo.Minute;

      DateTime tmpDateOd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, realStartHrs, realStartMin, 0);
      DateTime tmpDateDo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, realEndHrs  , realEndMin  , 0);

      TimeSpan ts = tmpDateDo.Subtract(tmpDateOd);
      decimal defaultRealSati_perDay = System.Convert.ToDecimal(ts.TotalHours);

      if(isRealDate)
      {
         rptXtrans.T_dateOd = new DateTime(realXtrans.T_dateOd.Year, realXtrans.T_dateOd.Month, realXtrans.T_dateOd.Day, realStartHrs, realStartMin, 0);
         rptXtrans.T_dateDo = new DateTime(realXtrans.T_dateDo.Year, realXtrans.T_dateDo.Month, realXtrans.T_dateDo.Day, realEndHrs  , realEndMin  , 0);
      }
      #endregion real time

   }


   private Xtrans GetDefaultXtrans(Person person, DateTime day, int startHrs, int startMin, int endHrs, int endMin, decimal defaultRSati_perDay)
   {
      Xtrans xtrans = new Xtrans();

      xtrans.T_personCD     = person.PersonCD;
      xtrans.T_dokDate      = day;
      xtrans.T_kpdbNameA_50 = person.Prezime;
      xtrans.T_kpdbNameB_50 = person.Ime;

      if(ZXC.IsThisDanPraznik(day, day.Year) == true && ZXC.IsThisDanVikend(day) == false)
      {
         xtrans.RVR_6 = defaultRSati_perDay; // blagdani 

         xtrans.T_dateOd = new DateTime(day.Year, day.Month, day.Day, startHrs, startMin, 0);
         xtrans.T_dateDo = new DateTime(day.Year, day.Month, day.Day, endHrs  , endMin  , 0);
      }
      else if(ZXC.IsThisDanNeradniDan(day, day.Year) == false)
      {
         xtrans.RVR_4 = defaultRSati_perDay; // ukupno dnevno radno vrijeme 

         xtrans.T_dateOd = new DateTime(day.Year, day.Month, day.Day, startHrs, startMin, 0);
         xtrans.T_dateDo = new DateTime(day.Year, day.Month, day.Day, endHrs  , endMin  , 0);
      }

      return xtrans;
   }

   private Xtrans GetDefaultXtrans_15(Person person, DateTime day, int startHrs, int startMin, int endHrs, int endMin, decimal defaultRSati_perDay)
   {
      Xtrans xtrans = new Xtrans();

      xtrans.T_personCD = person.PersonCD;
      xtrans.T_dokDate = day;
      xtrans.T_kpdbNameA_50 = person.Prezime;
      xtrans.T_kpdbNameB_50 = person.Ime;

      
      if(ZXC.IsThisDanPraznik(day, day.Year) == true && ZXC.IsThisDanVikend(day) == false)
      {
         xtrans.RVR_14 = defaultRSati_perDay; // blagdani 

         xtrans.T_dateOd = new DateTime(day.Year, day.Month, day.Day, startHrs, startMin, 0);
         xtrans.T_dateDo = new DateTime(day.Year, day.Month, day.Day, endHrs  , endMin  , 0);
      
         xtrans.RVR_4 = defaultRSati_perDay; // ukupno dnevno radno vrijeme 
      }
      else if(ZXC.IsThisDanNeradniDan(day, day.Year) == false)
      {
         xtrans.RVR_5 = defaultRSati_perDay; // ukupno dnevno radno vrijeme ali koje ce nastati samo vrijeme provedeno na radu 

         xtrans.T_dateOd = new DateTime(day.Year, day.Month, day.Day, startHrs, startMin, 0);
         xtrans.T_dateDo = new DateTime(day.Year, day.Month, day.Day, endHrs  , endMin  , 0);

         xtrans.RVR_4 = defaultRSati_perDay; // ukupno dnevno radno vrijeme 

      }


      if(ZXC.IsThisDanNeradniDan(day, day.Year) == false) xtrans.RVR_11 = 24.00M - defaultRSati_perDay;

      if(ZXC.IsThisDanVikend(day) == true)                xtrans.RVR_12 = 24.00M;

      return xtrans;
   }

}

public class RptX_EvidencijaRadnogVremena_Mjesecna : RptX_StandardMixerReport
{
   public RptX_EvidencijaRadnogVremena_Mjesecna(ReportDocument _reportDocument, string _reportName, VvRpt_Mix_Filter _rptFilter, ZXC.MIX_FilterStyle _filterStyle, 
      bool _rptNeeds_Mixer, 
      bool _rptNeeds_Xtrans, 
      bool _rptNeeds_Xtrano, 
      bool _needsXtrSO_Join_Mixer,
      bool _rptNeeds_Kupdob,
      bool _rptNeeds_Artikl,
      bool _rptNeeds_Prjkt)
      : base(_reportDocument, _reportName, _rptFilter, _filterStyle, _rptNeeds_Mixer, _rptNeeds_Xtrans, _rptNeeds_Xtrano, _needsXtrSO_Join_Mixer, _rptNeeds_Kupdob, _rptNeeds_Artikl, _rptNeeds_Prjkt)  
   {
      IsForExport = false;
   }

   public override int FillMixerReportLists()
   {
      base.FillMixerReportLists();

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Person>(null, VvSQL.SorterType.Name);

      TheManyDecimalsList = new List<VvManyDecimalsReportSourceRow>();

      // prvo postavi svakom MVR xtransu T_moneyD sto je PFS. 
      // NOTA BENE da ce samo jedan personov MVRxtrans ga dobiti. Dakle, ako person ima vise MVRxtransa samo ce jedan imati T_moneyD informaciju (PFS) 
      TheXtransList.ForEach(xtr => xtr.T_moneyD = xtr.GetMonthlyPFS(TheXtransList));

      // Napuni TheManyDecimalsList (koja je report datasource) na osnovi svake neprazne 'sati' kolone MVR Xtrans-a 
      // TheXtransList jr MVR lista 
      TheXtransList.ForEach(xtr => TheManyDecimalsList.AddRange(CreateMVR_ReportSourceRowFromMVR_Xtrans(xtr, ZXC.luiListaMixRadVrijemeMVR, Placa.GetDateTimeFromMMYYYY(RptFilter.ZaMMYYYY, true).Day)));

      TheManyDecimalsList = TheManyDecimalsList.OrderBy(mdl => mdl.RowUintCD /* person */).ThenBy(mdl => mdl.RowStringCD /* rbr dana u mjesecu */).ToList();

      return TheManyDecimalsList.Count;
   }

   private List<VvManyDecimalsReportSourceRow> CreateMVR_ReportSourceRowFromMVR_Xtrans(Xtrans xtrans_rec, VvLookUpLista MVRlokupList, int dayDO)
   {
      Person person_rec = VvUserControl.PersonSifrar.SingleOrDefault(per => per.PersonCD == xtrans_rec.T_personCD);
      string rowName = person_rec != null ? person_rec.Ime + " " + person_rec.Prezime : "nepoznat";

      List<VvManyDecimalsReportSourceRow> manyDecimalsList = new List<VvManyDecimalsReportSourceRow>();

      /*if(xtrans_rec.T_dec01.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str01, xtrans_rec.T_dec01, MVRlokupList, "01", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec02.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str02, xtrans_rec.T_dec02, MVRlokupList, "02", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec03.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str03, xtrans_rec.T_dec03, MVRlokupList, "03", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec04.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str04, xtrans_rec.T_dec04, MVRlokupList, "04", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec05.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str05, xtrans_rec.T_dec05, MVRlokupList, "05", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec06.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str06, xtrans_rec.T_dec06, MVRlokupList, "06", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec07.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str07, xtrans_rec.T_dec07, MVRlokupList, "07", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec08.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str08, xtrans_rec.T_dec08, MVRlokupList, "08", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec09.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str09, xtrans_rec.T_dec09, MVRlokupList, "09", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec10.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str10, xtrans_rec.T_dec10, MVRlokupList, "10", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec11.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str11, xtrans_rec.T_dec11, MVRlokupList, "11", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec12.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str12, xtrans_rec.T_dec12, MVRlokupList, "12", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec13.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str13, xtrans_rec.T_dec13, MVRlokupList, "13", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec14.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str14, xtrans_rec.T_dec14, MVRlokupList, "14", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec15.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str15, xtrans_rec.T_dec15, MVRlokupList, "15", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec16.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str16, xtrans_rec.T_dec16, MVRlokupList, "16", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec17.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str17, xtrans_rec.T_dec17, MVRlokupList, "17", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec18.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str18, xtrans_rec.T_dec18, MVRlokupList, "18", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec19.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str19, xtrans_rec.T_dec19, MVRlokupList, "19", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec20.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str20, xtrans_rec.T_dec20, MVRlokupList, "20", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec21.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str21, xtrans_rec.T_dec21, MVRlokupList, "21", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec22.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str22, xtrans_rec.T_dec22, MVRlokupList, "22", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec23.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str23, xtrans_rec.T_dec23, MVRlokupList, "23", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec24.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str24, xtrans_rec.T_dec24, MVRlokupList, "24", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec25.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str25, xtrans_rec.T_dec25, MVRlokupList, "25", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec26.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str26, xtrans_rec.T_dec26, MVRlokupList, "26", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec27.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str27, xtrans_rec.T_dec27, MVRlokupList, "27", xtrans_rec.T_personCD, rowName));
      /*if(xtrans_rec.T_dec28.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str28, xtrans_rec.T_dec28, MVRlokupList, "28", xtrans_rec.T_personCD, rowName));
      if(dayDO > 28)                                                                                                                                                               
      /*if(xtrans_rec.T_dec29.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str29, xtrans_rec.T_dec29, MVRlokupList, "29", xtrans_rec.T_personCD, rowName));
      if(dayDO > 29)                                                                                                                                                               
      /*if(xtrans_rec.T_dec30.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str30, xtrans_rec.T_dec30, MVRlokupList, "30", xtrans_rec.T_personCD, rowName));
      if(dayDO > 30)                                                                                                                                                               
      /*if(xtrans_rec.T_dec31.NotZero())*/ manyDecimalsList.Add(new VvManyDecimalsReportSourceRow(xtrans_rec.T_str31, xtrans_rec.T_dec31, MVRlokupList, "31", xtrans_rec.T_personCD, rowName));

      // Dodavanje dummy MVRa za PFS informaciju tom personu 
      VvManyDecimalsReportSourceRow thisPersonPFSrow = new VvManyDecimalsReportSourceRow();
      decimal thisPersonPFsati = xtrans_rec.T_moneyD;
      if(thisPersonPFsati.NotZero())
      {
         thisPersonPFSrow.RowUintCD = xtrans_rec.T_personCD;
         thisPersonPFSrow.RowName   = rowName              ;

         decimal priznatiPrekovrSati = Placa.GetPriznatiPrekovrSati(thisPersonPFsati);
         decimal visakPrekovrSati    = Placa.GetVisakPrekovrSati   (thisPersonPFsati);

         thisPersonPFSrow.DecimA03 = priznatiPrekovrSati;
         thisPersonPFSrow.DecimA20 = visakPrekovrSati   ;

         // umanji ovom personu redovan rad za ove PFS 
         thisPersonPFSrow.DecimA01 = -thisPersonPFsati;

         manyDecimalsList.Add(thisPersonPFSrow);
      }


      // Dodavanje dummy MVRa za MFS informaciju tom personu 
      VvManyDecimalsReportSourceRow thisPersonMFSrow = new VvManyDecimalsReportSourceRow();
      decimal thisPersonMFsati   = xtrans_rec.R_MVR_MFS ;
      thisPersonMFSrow.RowUintCD = xtrans_rec.T_personCD;
      thisPersonMFSrow.RowName   = rowName              ;

      thisPersonMFSrow.DecimA19 = thisPersonMFsati;

      manyDecimalsList.Add(thisPersonMFSrow);





      return manyDecimalsList;
   }

   public static bool IsFilterWellFormed(MixerReportUC reportUC)
   {
      bool OK = true;
      VvRpt_Mix_Filter filter = reportUC.TheRptFilter;

      if(reportUC.TheMixerFilterUC.Fld_ZaMMYYYY.IsEmpty())
      {
         ZXC.aim_emsg("Molim, zadajte ZaMjesec.");
         return false;
      }

      return (OK);
   }

}

public class RptX_PriBor : RptX_StandardMixerReport
{
   public RptX_PriBor(ReportDocument _reportDocument, string _reportName, VvRpt_Mix_Filter _rptFilter, ZXC.MIX_FilterStyle _filterStyle, 
      bool _rptNeeds_Mixer, 
      bool _rptNeeds_Xtrans, 
      bool _rptNeeds_Xtrano, 
      bool _needsXtrSO_Join_Mixer,
      bool _rptNeeds_Kupdob,
      bool _rptNeeds_Artikl,
      bool _rptNeeds_Prjkt)
      : base(_reportDocument, _reportName, _rptFilter, _filterStyle, _rptNeeds_Mixer, _rptNeeds_Xtrans, _rptNeeds_Xtrano, _needsXtrSO_Join_Mixer, _rptNeeds_Kupdob, _rptNeeds_Artikl, _rptNeeds_Prjkt)  
   {
      IsForExport = true;
   }

   private Dictionary<string, VvImpExp.ImpExpField> Trans_Dict { get; set; }

   public static bool IsFilterWellFormed(MixerReportUC reportUC) // FUSE 
   {
      bool OK = true;
      VvRpt_Mix_Filter filter = reportUC.TheRptFilter;

      //if(filter.BankaCd.IsZero())
      //{
      //   ZXC.aim_emsg("Molim, zadajte BANKU.");
      //   return false;
      //}

      return (OK);
   }

   public override string ExportFileName { get { return "PBSMUP.TXT"; } }

   public override bool ExecuteExport(string fileName)
   {
      using(StreamWriter sw = new StreamWriter(fileName, false, Encoding.GetEncoding(1250)))
      {
         SetAndDump_PBSMUP(sw);
      }

      ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Gotovo.\n\nKreirao datoteku\n\n" + fileName + "\n\nBroj prijava: " + TheMixerList.Count);

      return true;
   }

   private void SetAndDump_PBSMUP(StreamWriter sw)
   {
      #region Slog Fields Initialization

      // Rbr. Opis             Dužina Tip Napomena                                    
      // 1.   Objekt                9 N   obavezno polje (dodjeljena šifra)           
      // 2.   Prezime              20 C   obavezno polje                              
      // 3.   Ime                  15 C   obavezno polje                              
      // 4.   datum rođenja         8 C   obavezno polje (u obliku GGGGMMDD)          
      // 5.   mjesto rođenja       25 C   ako je nepoznato prazno (blank)             
      // 6.   Državljanstvo         3 C   obavezno polje (šifra)                      
      // 7.   država rođenja        3 C   obavezno polje (šifra)                      
      // 8.   vrsta putne isprave   3 C   obavezno polje (šifra)                      
      // 9.   broj putne isprave   15 C   obavezno polje                              
      // 10.  mjesto ulaska u RH    3 N   obavezno polje (šifra)                      
      // 11.  datum ulaska u RH     8 N   obavezno polje, GGGGMMDD za nepoznato blank 
      // 12.  datum prijave         8 N   obavezno polje, GGGGMMDD                    
      // 13.  adresa               25 C   kontrolira se ako je upisano                
      // 14.  redni broj prijave    7 N   obavezno polje (npr.0004567)                
      // 15.  vrsta gosta           1 C   prema priloženim šiframa                    
      // 16.  status gosta          1 C   prema priloženim šiframa                    
      // 17.  tip objekta           2 N   prema priloženim šiframa                    
      // 18.  datum odjave          8 N   obavezno polje (u obliku GGGGMMDD)          
      // 19.  Slobodno polje       14                                                 
      // 20.  oznaka kraja sloga    2     hexadecimalno 0D0A tj. CRLF                 
      //            Ukupna dužina 180                                                 
      VvImpExp.ImpExpField[] Trans_Fields = new VvImpExp.ImpExpField[]
      {  
         new VvImpExp.ImpExpField("OBJEKT"        ,   1,  9), 
         new VvImpExp.ImpExpField("PREZIME"       ,   2, 20), 
         new VvImpExp.ImpExpField("IME"           ,   3, 15),
         new VvImpExp.ImpExpField("DAT RODJENJA"  ,   4,  8), 
         new VvImpExp.ImpExpField("MJ RODJENJA"   ,   5, 25), 
         new VvImpExp.ImpExpField("DRZAVLJANSTVO" ,   6,  3), 
         new VvImpExp.ImpExpField("DRZV RODJENJA" ,   7,  3),
         new VvImpExp.ImpExpField("VRSTA PUT ISP" ,   8,  3), 
         new VvImpExp.ImpExpField("BROJ PUT ISP"  ,   9, 15), 
         new VvImpExp.ImpExpField("MJ UL U RH"    ,  10,  3), 
         new VvImpExp.ImpExpField("DAT UL U RH"   ,  11,  8), 
         new VvImpExp.ImpExpField("DAT PRIJAVE"   ,  12,  8), 
         new VvImpExp.ImpExpField("ADRESA"        ,  13, 25), 
         new VvImpExp.ImpExpField("RBR PRIJAVE"   ,  14,  7), 
         new VvImpExp.ImpExpField("VRSTA GOSTA"   ,  15,  1), 
         new VvImpExp.ImpExpField("STATUS GOSTA"  ,  16,  1), 
         new VvImpExp.ImpExpField("TIP OBJEKTA"   ,  17,  2), 
         new VvImpExp.ImpExpField("DAT ODJAVE"    ,  18,  8), 
         new VvImpExp.ImpExpField("SLOBODNO"      ,  19, 14), // 178
       //new VvImpExp.ImpExpField("OZN KRAJA SLG" ,  20,  2), 
      };

      Trans_Dict = VvImpExp.CreateDictionary(Trans_Fields, "Trans", /*180*/178);

      #endregion Slog Fields

      foreach(Mixer mixer_rec in TheMixerList)
      {
         #region Set & Dump Trans_Dict Data
                                                        /*Length*/
         /* 01 */ Trans_Dict["OBJEKT"       ].SetIntgerFldValue(/*  9 */ mixer_rec.IntA);
         /* 02 */ Trans_Dict["PREZIME"      ].FldValue = /* 20 */ mixer_rec.StrF_64.ToUpper();
         /* 03 */ Trans_Dict["IME"          ].FldValue = /* 15 */ mixer_rec.StrA_40.ToUpper();
         /* 04 */ Trans_Dict["DAT RODJENJA" ].SetYYYYMMDDFldValue(mixer_rec.DateB)  ;
         /* 05 */ Trans_Dict["MJ RODJENJA"  ].FldValue = /* 25 */ mixer_rec.StrH_32.ToUpper();
         /* 06 */ Trans_Dict["DRZAVLJANSTVO"].FldValue = /*  3 */ mixer_rec.Konto   ; // iz LookupListe 
         /* 07 */ Trans_Dict["DRZV RODJENJA"].FldValue = /*  3 */ mixer_rec.Konto2  ; // iz LookupListe 
         /* 08 */ Trans_Dict["VRSTA PUT ISP"].FldValue = /*  3 */ mixer_rec.V1_tt   ; // iz LookupListe 
         /* 09 */ Trans_Dict["BROJ PUT ISP" ].FldValue = /* 15 */ mixer_rec.StrB_128;
         /* 10 */ Trans_Dict["MJ UL U RH"   ].FldValue = /*  3 */ mixer_rec.V2_tt   ; // iz LookupListe 
         /* 11 */ Trans_Dict["DAT UL U RH"  ].SetYYYYMMDDFldValue(mixer_rec.DateA)  ;
         /* 12 */ Trans_Dict["DAT PRIJAVE"  ].SetYYYYMMDDFldValue(mixer_rec.DokDate);
         /* 13 */ Trans_Dict["ADRESA"       ].FldValue = /* 25 */ mixer_rec.StrE_256;
         /* 14 */ Trans_Dict["RBR PRIJAVE"  ].FldValue = /*  7 */ mixer_rec.DokNum.ToString("0000000"); // formatiraj ttNum ToString("0000000")
         /* 15 */ Trans_Dict["VRSTA GOSTA"  ].FldValue = /*  1 */ mixer_rec.StrC_32 ; // iz LookupListe 
         /* 16 */ Trans_Dict["STATUS GOSTA" ].FldValue = /*  1 */ mixer_rec.StrD_32 ; // iz LookupListe 
         /* 17 */ Trans_Dict["TIP OBJEKTA"  ].FldValue = /*  2 */ mixer_rec.KupdobTK; // iz LookupListe 
         /* 18 */ Trans_Dict["DAT ODJAVE"   ].SetYYYYMMDDFldValue(mixer_rec.DateB)  ; 
         /* 19 */ Trans_Dict["SLOBODNO"     ].FldValue = /* 14 */ ""                ;
       ///* 20 */ Trans_Dict["OZN KRAJA SLG"].FldValue = /*  2 */ "\r\n"            ;

         VvImpExp.DumpFields(sw, Trans_Fields);

         #endregion Set & Dump Trans_Dict Data
      }
   }

}

public class RptX_NocenjaInfo : RptX_StandardMixerReport
{
   public RptX_NocenjaInfo(ReportDocument _reportDocument, string _reportName, VvRpt_Mix_Filter _rptFilter, ZXC.MIX_FilterStyle _filterStyle, 
      bool _rptNeeds_Mixer, 
      bool _rptNeeds_Xtrans, 
      bool _rptNeeds_Xtrano, 
      bool _needsXtrSO_Join_Mixer,
      bool _rptNeeds_Kupdob,
      bool _rptNeeds_Artikl,
      bool _rptNeeds_Prjkt)
      : base(_reportDocument, _reportName, _rptFilter, _filterStyle, _rptNeeds_Mixer, _rptNeeds_Xtrans, _rptNeeds_Xtrano, _needsXtrSO_Join_Mixer, _rptNeeds_Kupdob, _rptNeeds_Artikl, _rptNeeds_Prjkt)  
   {
      //IsForExport = true;
   }

   public override int FillMixerReportLists()
   {
      base.FillMixerReportLists();

      TheManyDecimalsList = new List<VvManyDecimalsReportSourceRow>();

      DateTime dateOD = RptFilter.DatumOd;
      DateTime dateDO = RptFilter.DatumDo;

      var nocenjaPoDrzaviGroup =
         TheMixerList
       //.GroupBy(mix => mix.Konto        ) // drzavljanstvo 
       //.GroupBy(mix => mix.Konto2       ) // prebivaliste  
         .GroupBy(mix => mix.R_GSTdrzavaCD) // result        
         .Select(drzavaGR => new 
            {
               drzavaCD           = drzavaGR.Key                                                       ,
               drzavaName         = ZXC.luiListaDrzave.GetNameForThisCd(drzavaGR.Key)                  ,
               domacaNocenjaMoney = drzavaGR.Sum  (mix => mix.GetGSTdomacaNocenjaMoney(dateOD, dateDO)),
               stranaNocenjaMoney = drzavaGR.Sum  (mix => mix.GetGSTstranaNocenjaMoney(dateOD, dateDO)),
               domacaNocenjaCount = drzavaGR.Sum  (mix => mix.GetGSTdomacaNocenjaCount(dateOD, dateDO)),
               stranaNocenjaCount = drzavaGR.Sum  (mix => mix.GetGSTstranaNocenjaCount(dateOD, dateDO)),
               domaciDolasciCount = drzavaGR.Count(mix => mix.IsDomaciDolazak         (dateOD, dateDO)),
               straniDolasciCount = drzavaGR.Count(mix => mix.IsStraniDolazak         (dateOD, dateDO)),
               isDomaci           = Mixer.IsDomaci(drzavaGR.First().R_GSTdrzavaCD)                     ,
            }
         );

      foreach(var drzavaGR in nocenjaPoDrzaviGroup)
      {
         TheManyDecimalsList.Add(new VvManyDecimalsReportSourceRow() 
            { 
               IsXxx       =           drzavaGR.isDomaci,
               RowStringCD =           drzavaGR.drzavaCD,
               RowName     =           drzavaGR.drzavaName,
               DecimA01    =           drzavaGR.domacaNocenjaMoney,
               DecimA02    =           drzavaGR.stranaNocenjaMoney,
               DecimA03    =           drzavaGR.domacaNocenjaCount,
               DecimA04    =           drzavaGR.stranaNocenjaCount,
               DecimA05    = (decimal) drzavaGR.domaciDolasciCount,
               DecimA06    = (decimal) drzavaGR.straniDolasciCount,
            }
         );
      }

      return TheManyDecimalsList.Count;
   }

}

#endregion Mixer Reports (Standard Lists ala Rekapitulacija...)

