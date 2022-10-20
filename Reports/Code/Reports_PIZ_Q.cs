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
#endif

public class RptP_JOPPD : VvPlacaReport
{
   public RptP_JOPPD(ReportDocument _reportDocument, ZXC.VvRptExternTblChooser_Placa externTblChooser, string _reportName, VvRpt_Placa_Filter _rptFilter): base(_reportDocument, externTblChooser, _reportName, _rptFilter)
   {
      IsForExport = true;

      jpdStrA = new JOPPD_stranaA();
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
         string rmsID  = RptFilter.RSmID;
         string mmyyyy = RptFilter.DatumOd.Month.ToString("00") + RptFilter.DatumOd.Year.ToString("0000");
         string   yyyy =                                          RptFilter.DatumOd.Year.ToString("0000");

         return "JOPPD_" + ZXC.CURR_prjkt_rec.Oib + "_" + rmsID + ".xml";
      }
   }

   #region Xml Schema Validation

   public override bool ExecuteExportValidation(string fileName)
   {
      List<VvXmlValidationData> valDataList = new List<VvXmlValidationData>();
      
      valDataList.Add(new VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacJOPPD/v1-0", @"ObrazacJOPPD-v1-0.xsd"          ));
      valDataList.Add(new VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacJOPPD/v1-0", @"ObrazacJOPPDtipovi-v1-0.xsd"));
      valDataList.Add(new VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"           , @"ObrazacJOPPDmetapodaci-v1-0.xsd"));
      valDataList.Add(new VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/Metapodaci/v2-0"           , @"MetapodaciTipovi-v2-0.xsd"));
      valDataList.Add(new VvXmlValidationData(@"http://e-porezna.porezna-uprava.hr/sheme/TemeljniTipovi/v2-1"       , @"TemeljniTipovi-v2-1.xsd"      ));

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
   }

   public override bool ExecuteExport(string fileName)
   {
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

         writer.WriteStartElement   ("ObrazacJOPPD", @"http://e-porezna.porezna-uprava.hr/sheme/zahtjevi/ObrazacJOPPD/v1-0");
         writer.WriteAttributeString("verzijaSheme",                                                                  "1.0");

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
            writer.WriteRaw(ident + ident + "<Uskladjenost dc=\"http://purl.org/dc/terms/conformsTo\">ObrazacJOPPD-v1-0</Uskladjenost>\n");

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
            writer.WriteStartElement("ObveznikPlacanja");
               writer.WriteElementString("Naziv",       jpdStrA.opNaziv    );
                 writer.WriteStartElement("Adresa");           
                    writer.WriteElementString("Mjesto", jpdStrA.opMjesto   );
                    writer.WriteElementString("Ulica" , jpdStrA.opUlica    );
                    writer.WriteElementString("Broj"  , jpdStrA.opUlicaBr  );
                 writer.WriteEndElement(); // Adresa 
               writer.WriteElementString("Email" ,      jpdStrA.opEmail    );
               writer.WriteElementString("OIB"   ,      jpdStrA.opOIB      );
            writer.WriteEndElement(); // ObveznikPlacanja 
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
            writer.WriteEndElement(); // PredujamPoreza 
            writer.WriteStartElement("Doprinosi");
               writer.WriteStartElement("GeneracijskaSolidarnost");
                  writer.WriteElementString("P1",       jpdStrA.mio1P01.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P2",       jpdStrA.mio1P02.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P3",       jpdStrA.mio1P03.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P4",       jpdStrA.mio1P04.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P5",       jpdStrA.mio1P05.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P6",       jpdStrA.mio1P06.ToStringVv_NoGroup_ForceDot());
               writer.WriteEndElement(); // GeneracijskaSolidarnost 
               writer.WriteStartElement("KapitaliziranaStednja");
                  writer.WriteElementString("P1",       jpdStrA.mio2P01.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P2",       jpdStrA.mio2P02.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P3",       jpdStrA.mio2P03.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P4",       jpdStrA.mio2P04.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P5",       jpdStrA.mio2P05.ToStringVv_NoGroup_ForceDot());
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
               writer.WriteEndElement(); // ZdravstvenoOsiguranje 
               writer.WriteStartElement("Zaposljavanje");
                  writer.WriteElementString("P1",       jpdStrA.zapP01.ToStringVv_NoGroup_ForceDot());
                  writer.WriteElementString("P2",       jpdStrA.zapP02.ToStringVv_NoGroup_ForceDot());
               writer.WriteEndElement(); // Zaposljavanje 
            writer.WriteEndElement(); // Doprinosi 
            writer.WriteElementString("IsplaceniNeoporeziviPrimici", jpdStrA.npNetto.ToStringVv_NoGroup_ForceDot());
            writer.WriteElementString("KamataMO2"                  , jpdStrA.ktaMO2 .ToStringVv_NoGroup_ForceDot());
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
            strB.P012 = jpdBstranaRow.b_Bruto     .ToStringVv_NoGroup_ForceDot();
            strB.P121 = jpdBstranaRow.b_Mio1stup  .ToStringVv_NoGroup_ForceDot();
            strB.P122 = jpdBstranaRow.b_Mio2stup  .ToStringVv_NoGroup_ForceDot();
            strB.P123 = jpdBstranaRow.b_ZdrNa     .ToStringVv_NoGroup_ForceDot();
            strB.P124 = jpdBstranaRow.b_ZorNa     .ToStringVv_NoGroup_ForceDot();
            strB.P125 = jpdBstranaRow.b_ZapNa     .ToStringVv_NoGroup_ForceDot();
            strB.P126 = jpdBstranaRow.b_Mio1stupNa.ToStringVv_NoGroup_ForceDot();
            strB.P127 = jpdBstranaRow.b_Mio2stupNa.ToStringVv_NoGroup_ForceDot();
            strB.P128 = jpdBstranaRow.b_ZpiUk     .ToStringVv_NoGroup_ForceDot();
            strB.P129 = jpdBstranaRow.b_ZapII     .ToStringVv_NoGroup_ForceDot();
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
            strB.P017 = jpdBstranaRow.b_Bruto     .ToStringVv_NoGroup_ForceDot();

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
