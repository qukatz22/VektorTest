using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
#else
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using System.Xml;
#endif

public class VvFiskalizacija
{
   // TODO: treba li Racun_ZastitniKod, FISK_messageID usnimavati u DataLayer pa da budu fiksni za kasniju upotrebu?!!! 
   // TODO: od kuda ces saznavati certFileName, certPassword, xmlFileName!!!??? Put it in some PREFS? 
   // TODO: ??? koji dokTime ces podvaljivati?! 

   #region fieldz

   private Faktur         faktur_rec          ;
   private XSqlConnection theDbConnection     ;
   private string         certFullPathFileName;
   private string         certFileName        ;
   private string         certPassword        ;
   private string         certDirectoryName   ;
   private string         xmlFullPathFileName ;
   private string         xmlFileName         ;
   private string         xmlDirectoryName    ;

   private string   fisk_privateKey; //( pk - Base64 kodirani privatni ključ iz certifikata )
   private string   fisk_oib       ;
   private string   fisk_rnDatVrij ; // (datum i vrijeme izdavanja računa zapisan kao tekst u formatu 'dd.MM.gggg hh:MM:ss' )
   private string   fisk_rnDatVrijT; // (datum i vrijeme izdavanja računa zapisan kao tekst u formatu 'dd.mm.ggggThh:mm:ss' )
   private string   fisk_bor       ; // ( bor – brojčana oznaka računa )
   private string   fisk_opp       ; // ( opp – oznaka poslovnog prostora )
   private string   fisk_onu       ; // ( onu – oznaka naplatnog uređaja )
    
   private string   fisk_MessageGUID    ;
   private string   fisk_MessageDateTime;
   private string   fisk_oznakaSlijeda  ; // P - na nivou poslovnog prostora, N - na nivou naplatnog uređaja
    
   private bool fisk_isPDV;

   private decimal fisk_osnOslobodPDV ;
   private decimal fisk_osnNePodlijPDV;
   private decimal fisk_osn10;
   private decimal fisk_pdv10;
   private decimal fisk_osn22;
   private decimal fisk_pdv22;
   private decimal fisk_osn23;
   private decimal fisk_pdv23;
   private decimal fisk_osn25;
   private decimal fisk_pdv25;
    
   private decimal fisk_oslobodjeno;
   private decimal fisk_nePodlijeze;
    
   private string fisk_nacPlac;
   private string fisk_operaterOIB;

   private bool fisk_isNaknadno; // NAKNADNA dostava racuna (jer je inicijalnu pratio neki problem) 

   #endregion fieldz

   #region Propertiz

   //public bool IsOK { get; set; }

   #endregion Propertiz

   #region Constructor

   public VvFiskalizacija(XSqlConnection _conn, Faktur _faktur_rec, string _certFullPathFileName, string _certPassword, string _xmlFullPathFileName, bool _isNaknadno)
   {
      //this.IsOK = true;

      this.theDbConnection = _conn;
      this.faktur_rec      = _faktur_rec;

      DirectoryInfo cert_dInfo, xml_dInfo;

      certFullPathFileName = _certFullPathFileName;
      cert_dInfo           = new DirectoryInfo(certFullPathFileName);
      certFileName         = cert_dInfo.Name;
      certDirectoryName    = certFullPathFileName.Substring(0, certFullPathFileName.Length - (certFileName.Length+1));
      certPassword         = _certPassword;

      xmlFullPathFileName = _xmlFullPathFileName;
      xml_dInfo           = new DirectoryInfo(xmlFullPathFileName);
      xmlFileName         = xml_dInfo.Name;
      xmlDirectoryName    = xmlFullPathFileName.Substring(0, xmlFullPathFileName.Length - (xmlFileName.Length+1));

      InitializeRacunDataFor_FISK_Format(_isNaknadno);
   }

   #endregion Constructor

   #region The Racun

   public bool CreateXMLfile()
   {
      #region Initialize XmlWriterSettings

      if(Fisk_ZastKodIzdavat.IsEmpty()) throw new Exception("Nema se što exporitrati!");

      string ident = "   ";

      XmlWriterSettings settings = new XmlWriterSettings();
      
      settings.Indent = true;
      settings.IndentChars = ident;

      #endregion Initialize XmlWriterSettings

      using(XmlWriter writer = XmlWriter.Create(xmlFullPathFileName, settings))
      {
         #region Init Xml Document

         writer.WriteStartDocument();

         //writer.WriteRaw(
         //   "\n<tns:RacunZahtjev\n" + 
         //   ident + @"xmlns:tns=""http://www.apis-it.hr/fin/2012/types/f73"""   + "\n" +
         //   ident + @"xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">" + "\n");

         writer.WriteStartElement("RacunZahtjev");
         writer.WriteAttributeString("tns", "RacunZahtjev", @"http://www.apis-it.hr/fin/2012/types/f73", null);
         writer.WriteAttributeString("xsi", "RacunZahtjev", @"http://www.w3.org/2001/XMLSchema-instance", null);

         #endregion Init Xml Document

         #region Write Header Data

         writer.WriteStartElement("tns", "Zaglavlje", null);

            writer.WriteElementString("tns", "IdPoruke"    , null, fisk_MessageGUID    );
            writer.WriteElementString("tns", "DatumVrijeme", null, fisk_MessageDateTime);
         //   writer.WriteStartElement("tns", "KontaktOsoba", null);

         //   writer.WriteElementString("tns", "Ime", null, /*ZXC.ROOT_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Ime);
         //      writer.WriteElementString("tns", "Prezime", null, /*ZXC.ROOT_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Prezime);
         //         writer.WriteStartElement("tns", "Telefoni", null);
         //            writer.WriteElementString("tns", "Telefon", null, /*ZXC.ROOT_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Tel1);
         //            //if(/*ZXC.ROOT_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Tel2.NotEmpty())
         //            {
         //               writer.WriteElementString("tns", "Telefon", null, /*ZXC.ROOT_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Tel1);
         //            }
         //         writer.WriteEndElement();
         //      writer.WriteElementString("tns", "Email", null, /*ZXC.ROOT_prjkt_rec*/ RptFilter.PlacaRootPrjkt_rec.Email);

         //   writer.WriteEndElement(); // KontaktOsoba

         writer.WriteEndElement(); // Zaglavlje

         #endregion Write Header Data

         #region Racun Data

         #endregion Racun Data

         #region Finish Xml Document

         writer.WriteEndElement();
         //writer.WriteRaw("\n</tns:RacunZahtjev>");
         writer.WriteEndDocument();

         #endregion Finish Xml Document

      }
      return true;
   }

   public bool SignXMLfile()
   {
      return true;
   }

   #region Racun Data

   private void InitializeRacunDataFor_FISK_Format(bool _isNaknadno)
   {
      fisk_MessageGUID = Guid.NewGuid().ToString("D");

      fisk_MessageDateTime = VvSQL.GetServer_DateTime_Now(theDbConnection).ToString("dd.MM.yyyyTHH:mm:ss");

      // TODO: !!!!! ??? koji dokTime ces podvaljivati?! 
      int dokHour   = 11;
      int dokMinute = 22;
      int dokSecond = 33;
      DateTime dokDateTime = new DateTime(faktur_rec.DokDate.Year, faktur_rec.DokDate.Month, faktur_rec.DokDate.Day, dokHour, dokMinute, dokSecond);

      fisk_isPDV         = ZXC.CURR_prjkt_rec.IS_IN_PDV;
      fisk_oznakaSlijeda = "P"; // P - na nivou poslovnog prostora, N - na nivou naplatnog uređaja
      fisk_rnDatVrij     = dokDateTime.ToString("dd.MM.yyyy HH:mm:ss"); // (datum i vrijeme izdavanja računa zapisan kao tekst u formatu 'dd.MM.gggg hh:MM:ss' )
      fisk_rnDatVrijT    = dokDateTime.ToString("dd.MM.yyyyTHH:mm:ss"); // (datum i vrijeme izdavanja računa zapisan kao tekst u formatu 'dd.mm.ggggThh:mm:ss' )
      fisk_oib           = ZXC.CURR_prjkt_rec.Oib;
      fisk_opp           = "POSLOVNICA" + (faktur_rec.SkladCD.IsEmpty() ? "0" : faktur_rec.SkladCD); // ( opp – oznaka poslovnog prostora ) 
      fisk_onu           = "1"                              ; // ( onu – oznaka naplatnog uređaja )

    //fisk_bor           = faktur_rec.TT_And_TtNum;           // ( bor – brojčana oznaka računa ) 
    //fisk_bor           = faktur_rec.TtNum.ToString() ;      // ( bor – brojčana oznaka računa ) 
      fisk_bor           = faktur_rec.TtSort_And_TtNum ;      // ( bor – brojčana oznaka računa ) tu ce biti smrdljivih jaja. Kako rjesiti da ne smijes dati slova 
                                                              // nego samo znamenke, a sta ako korisnik koristi IFA + IRA pa se preklapaju TtNum-ovi? Za sada, dakle, podvaljujes
                                                              // TtSort kao prefix, ali ce se FINA mozda buniti?!?!
      fisk_nacPlac       = GetFiskNacPlac();

      #region PDV iznosi

      /* 25% */ if(faktur_rec.S_ukPdv25m.NotZero()) { fisk_osn25 = faktur_rec.S_ukOsn25m; fisk_pdv25 = faktur_rec.S_ukPdv25m; }
      /* 23% */ if(faktur_rec.S_ukPdv23m.NotZero()) { fisk_osn23 = faktur_rec.S_ukOsn23m; fisk_pdv23 = faktur_rec.S_ukPdv23m; }
      /* 22% */ if(faktur_rec.S_ukPdv22m.NotZero()) { fisk_osn22 = faktur_rec.S_ukOsn22m; fisk_pdv22 = faktur_rec.S_ukPdv22m; }
      /* 10% */ if(faktur_rec.S_ukPdv10m.NotZero()) { fisk_osn10 = faktur_rec.S_ukOsn10m; fisk_pdv10 = faktur_rec.S_ukPdv10m; }

      decimal somePdv;
      /* Oslobodjeno od PDVa */ 
      somePdv = faktur_rec.S_ukOsn08 +
                faktur_rec.S_ukOsn09 +
                faktur_rec.S_ukOsn10 +
                faktur_rec.S_ukOsn11; 

      if(somePdv.NotZero()) fisk_oslobodjeno = somePdv; 

      /* NE Podlijeze PDVu */
      somePdv = faktur_rec.S_ukOsn07;

      if(somePdv.NotZero()) fisk_nePodlijeze = somePdv;

      #endregion PDV iznosi

      fisk_isNaknadno    = _isNaknadno;
   }

   private string GetFiskNacPlac()
   {
      // G – gotovina
      // K – kartice
      // C – ček
      // T – transakcijski račun
      // O – ostalo
      // U slučaju više načina plaćanja po jednom
      // računu, isto je potrebno prijaviti pod
      // 'Ostalo'.
      // Za sve načine plaćanja koji nisu prije
      // navedeni koristiti će se oznaka ‘Ostalo’.

      if(faktur_rec.IsNpCash == true) return "G"; // G – gotovina 

      if(
         faktur_rec.NacPlac.ToUpper().Contains("MASTER"  ) ||
         faktur_rec.NacPlac.ToUpper().Contains("DINERS"  ) ||
         faktur_rec.NacPlac.ToUpper().Contains("VISA"    ) ||
         faktur_rec.NacPlac.ToUpper().Contains("AMERICAN") ||
         faktur_rec.NacPlac.ToUpper().Contains("CARD"    ) ||
         faktur_rec.NacPlac.ToUpper().Contains("MAESTRO" )

      ) return "K"; // K – kartice 

      //switch(faktur_rec.NacPlac)
      //{
      //}

      return "T"; // default say T – transakcijski račun (virman) 
   }

   private string Fisk_ZastKodIzdavat
   {
      get
      {
         string uir = ""     ; // ( uir - ukupni iznos računa )
         string rezultatIspis; // izračunajMD5( medjurezultat )

         uir = faktur_rec.S_ukKCRP.ToStringVv();

         fisk_privateKey = GetPKfromCertifikate(certFullPathFileName, certPassword);

         rezultatIspis = (fisk_privateKey + fisk_oib + fisk_rnDatVrij + fisk_bor + fisk_opp + fisk_onu + uir).VvCalculateMD5();

         return rezultatIspis;
      }
   }

   private string GetPKfromCertifikate(string fileName, string password)
   {
      #region APIS.IT (JAVA code)
      //try
      //{
      //   using(Stream fis = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      //   {
      //      KeyStore keyStore = KeyStore.getInstance("JKS");
      //      keyStore.load(fis, "lozinka".toCharArray());
      //      Key key = keyStore.getKey("privatniKljuc", "lozinka".toCharArray());
      //      pk = Base64.encode(key.getEncoded());
      //   }
      //}
      //catch(Exception e)
      //{
      //   // nije uspjelo čitanje privatnog ključa
      //   e.printStackTrace();
      //}
      #endregion APIS IT

      // Load your certificate from file 
      X509Certificate2 certificate = new X509Certificate2(fileName, password, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);

      // Now you have your private key in binary form as you wanted 
      // You can use rsa.ExportParameters() or rsa.ExportCspBlob() to get you bytes 
      // depending on format you need them in 
      RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)certificate.PrivateKey;

      RSAParameters rsaParam = rsa.ExportParameters(true); // i sta sad? 
      // qukatz:
      return rsa.ToXmlString(true);
   }

   #endregion Racun Data

   #endregion The Racun

}
