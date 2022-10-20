//Copyright (c) 2012. Raverus d.o.o.

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Text;
using System.Security;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

// 27.11.2018: komentirano jer smeta za UBL.signer 
// zasad, neizvjesne posljedice ?! :-(             
//[assembly: AllowPartiallyTrustedCallers]
namespace Raverus.FiskalizacijaDEV
{
    /// <summary>
    /// Koristi se za komunikaciju sa CIS-om.</summary>
    /// <remarks>
    /// Ovo je osnovna klasa čija je namjena pozivanje web metoda CIS-a: Echo, RacunZahtjev i PoslovniProstorZahtjev.
    /// </remarks>
    public class CentralniInformacijskiSustav
    {
       // byQukatz: 19.12.2016: paramaterless constructor for setting dyrectories 
          public CentralniInformacijskiSustav()
          {
             #region Objasnjenje

             /// <summary>
             /// Naziv mape (foldera) u koji će se spremati XML dokumenti za zahtjeve. Ukoliko vrijednost nije postavljena, dokumenti se neće snimati.
             /// </summary>
             //public string NazivMapeZahtjev { get; set; }
             
             /// <summary>
             /// Naziv mape (foldera) u koji će se spremati XML dokumenti za odgovore. Ukoliko vrijednost nije postavljena, dokumenti se neće snimati.
             /// </summary>
             //public string NazivMapeOdgovor { get; set; }
             
             /// <summary>
             /// Određuje da li se naziv generira automatski koristeći UUID ili datoteka uvijek ima isti naziv
             /// </summary>
             /// <remarks>
             /// Ako je vrijednost true, naziv datoteke će biti određen koristeći naziv tipa dokumenta i UUID-a, ako je false naziv datoteke će biti uvijek isti i biti će određen tipom dokumenta.
             /// Ne koristi se ukoliko NazivMapeZahtjev odnosno NazivMapeOdgovor nisu postavljeni na odgovarajuću vrijednost.
             /// Nema smisla postavljati na TRUE za ECHO.
             /// </remarks>
             //public bool NazivAutoGeneriranje { get; set; }
             
             #endregion Objasnjenje
          
             NazivMapeOdgovor     = VvForm.GetLocalDirectoryForVvFile(@"FiskXML Odgovor");
             NazivMapeZahtjev     = VvForm.GetLocalDirectoryForVvFile(@"FiskXML Zahtjev");
             NazivAutoGeneriranje = true                                                 ;

          }


        #region Events
        /// <summary>
        /// "Okida" se neposredno prije poziva web servisa. Cancel postavljen na true će prekinuti poziv.</summary>
        public event EventHandler<CentralniInformacijskiSustavEventArgs> SoapMessageSending;
        /// <summary>
        /// "Okida" se neposredno nakon poziva web servisa.</summary>
        public event EventHandler<EventArgs> SoapMessageSent;
        #endregion

        #region Fields
        /// <summary>
        /// Sadrži URL to web servisa, u ovom trenutku se radi o službenom testnom okruženju.</summary>
        /// <remarks>
        /// U finalnoj verziji ova će se adresa svakako mijenjati.
        /// </remarks>
#if(DEBUG)
        // TESTNI URL  web servisa: 
        /*private*/internal const string cisUrl = "https://cistest.apis-it.hr:8449/FiskalizacijaServiceTest";

#else
        // PRODUKCIJSKI URL  web servisa: 
        /*private*/internal const string cisUrl = "https://cis.porezna-uprava.hr:8449/FiskalizacijaService";
#endif

        #endregion



        #region Poslovni prostor

       // byQukatz: sa certifikatFileName-om i certPassword-om iz prjkt_rec-a 
       /// <summary>
       /// byQukatz: sa certifikatFileName-om i certPassword-om iz prjkt_rec-a 
       /// </summary>
       /// <param name="poslovniProstor"></param>
       /// <param name="certifikatDatoteka"></param>
       /// <param name="zaporka"></param>
       /// <returns></returns>
        public XmlDocument PosaljiPoslovniProstor(string vvid, Schema.PoslovniProstorType poslovniProstor/*, string certifikatDatoteka, string zaporka*/)
        {
           XmlDocument racunOdgovor = null;

           Schema.PoslovniProstorZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajPoslovniProstorZahtjev(poslovniProstor);
           XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajPoslovniProstorZahtjev(zahtjev);

           PosaljiZahtjev(/*certifikatDatoteka, zaporka,*/ ref racunOdgovor, zahtjevXml, vvid);

           return racunOdgovor;
        }

        // byQukatz: sa certifikatFileName-om i certPassword-om 
       /// <summary>
        /// byQukatz: sa certifikatFileName-om i certPassword-om 
       /// </summary>
       /// <param name="poslovniProstor"></param>
       /// <param name="certifikatDatoteka"></param>
       /// <param name="zaporka"></param>
       /// <returns></returns>
        public XmlDocument PosaljiPoslovniProstor(Schema.PoslovniProstorType poslovniProstor, string certifikatDatoteka, string zaporka)
        {
           XmlDocument racunOdgovor = null;

           Schema.PoslovniProstorZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajPoslovniProstorZahtjev(poslovniProstor);
           XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajPoslovniProstorZahtjev(zahtjev);

           PosaljiZahtjev(certifikatDatoteka, zaporka, ref racunOdgovor, zahtjevXml);

           return racunOdgovor;
        }

        /// <summary>
        /// Koristi se za slanje informacija o poslovnom prostoru (PoslovniProstorZahtjev).</summary>
        /// <param name="poslovniProstor">Objekt tipa Schema.PoslovniProstorType koji sadrži informacije o poslovnom prostoru.</param>
        /// <param name="certificateSubject">Naziv certifikata koji se koristi, na primjer "FISKAL 1".</param>
        /// <example>
        ///  Raverus.FiskalizacijaDEV.Schema.PoslovniProstorType poslovniProstor = new Schema.PoslovniProstorType();
        ///  XmlDocument doc = cis.PosaljiPoslovniProstor(poslovniProstor, "FISKAL 1");
        /// </example>
        /// <returns>
        /// Vraća XmlDocument koji sadrži XML poruku vraćeno od CIS-a. U slučaju greške, vraća null.</returns>
        public XmlDocument PosaljiPoslovniProstor(Schema.PoslovniProstorType poslovniProstor, string certificateSubject)
        {
            XmlDocument racunOdgovor = null;

            Schema.PoslovniProstorZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajPoslovniProstorZahtjev(poslovniProstor);
            XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajPoslovniProstorZahtjev(zahtjev);

            PosaljiZahtjev(certificateSubject, ref racunOdgovor, zahtjevXml);

            return racunOdgovor;
        }

        public XmlDocument PosaljiPoslovniProstor(Schema.PoslovniProstorType poslovniProstor, X509Certificate2 certifikat)
        {
            XmlDocument racunOdgovor = null;

            Schema.PoslovniProstorZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajPoslovniProstorZahtjev(poslovniProstor);
            XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajPoslovniProstorZahtjev(zahtjev);

            PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(zahtjevXml, certifikat);
            PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref zahtjevXml);

            racunOdgovor = SendSoapMessage(zahtjevXml);


            return racunOdgovor;
        }

        public XmlDocument PosaljiPoslovniProstor(Schema.PoslovniProstorType poslovniProstor, string certificateSubject, StoreLocation storeLocation, StoreName storeName)
        {
            // prema sugestiji dkustec: http://fiskalizacija.codeplex.com/workitem/693
            XmlDocument racunOdgovor = null;

            Schema.PoslovniProstorZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajPoslovniProstorZahtjev(poslovniProstor);
            XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajPoslovniProstorZahtjev(zahtjev);

            PosaljiZahtjev(certificateSubject, storeLocation, storeName, ref racunOdgovor, zahtjevXml);

            return racunOdgovor;
        }

        public XmlDocument PosaljiPoslovniProstor(Schema.PoslovniProstorType poslovniProstor, string certificateSubject, DateTime datumVrijeme)
        {
            XmlDocument racunOdgovor = null;

            Schema.PoslovniProstorZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajPoslovniProstorZahtjev(poslovniProstor, datumVrijeme);
            XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajPoslovniProstorZahtjev(zahtjev);

            PosaljiZahtjev(certificateSubject, ref racunOdgovor, zahtjevXml);

            return racunOdgovor;
        }

        public XmlDocument PosaljiPoslovniProstor(Schema.PoslovniProstorType poslovniProstor, X509Certificate2 certifikat, DateTime datumVrijeme)
        {
            XmlDocument racunOdgovor = null;

            Schema.PoslovniProstorZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajPoslovniProstorZahtjev(poslovniProstor, datumVrijeme);
            XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajPoslovniProstorZahtjev(zahtjev);

            PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(zahtjevXml, certifikat);
            PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref zahtjevXml);

            racunOdgovor = SendSoapMessage(zahtjevXml);


            return racunOdgovor;
        }

        public XmlDocument PosaljiPoslovniProstor(Schema.PoslovniProstorType poslovniProstor, string certificateSubject, StoreLocation storeLocation, StoreName storeName, DateTime datumVrijeme)
        {
            // prema sugestiji dkustec: http://fiskalizacija.codeplex.com/workitem/693
            XmlDocument racunOdgovor = null;

            Schema.PoslovniProstorZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajPoslovniProstorZahtjev(poslovniProstor, datumVrijeme);
            XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajPoslovniProstorZahtjev(zahtjev);

            PosaljiZahtjev(certificateSubject, storeLocation, storeName, ref racunOdgovor, zahtjevXml);

            return racunOdgovor;
        }
        #endregion


        #region Račun

       // byQukatz: sa certifikatFileName-om i certPassword-om iz prjkt_rec-a 
       /// <summary>
       /// byQukatz: sa certifikatFileName-om i certPassword-om iz prjkt_rec-a 
       /// </summary>
       /// <param name="racun"></param>
       /// <param name="certifikatDatoteka"></param>
       /// <param name="zaporka"></param>
       /// <returns></returns>
        public XmlDocument PosaljiRacun(string vvid, Schema.RacunType racun/*, string certifikatDatoteka, string zaporka*/)
        {
           XmlDocument racunOdgovor = null;

           Schema.RacunZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajRacunZahtjev(racun);
           XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajRacunZahtjev(zahtjev);

           PosaljiZahtjev(/*certifikatDatoteka, zaporka,*/ ref racunOdgovor, zahtjevXml, vvid);

           return racunOdgovor;
        }

        // byQukatz: sa certifikatFileName-om i certPassword-om 

       /// <summary>
       /// byQukatz: sa certifikatFileName-om i certPassword-om 
       /// </summary>
       /// <param name="racun"></param>
       /// <param name="certifikatDatoteka"></param>
       /// <param name="zaporka"></param>
       /// <returns></returns>
        public XmlDocument PosaljiRacun(Schema.RacunType racun, string certifikatDatoteka, string zaporka)
        {
           XmlDocument racunOdgovor = null;

           Schema.RacunZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajRacunZahtjev(racun);
           XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajRacunZahtjev(zahtjev);

           PosaljiZahtjev(certifikatDatoteka, zaporka, ref racunOdgovor, zahtjevXml);

           return racunOdgovor;
        }

        /// <summary>
        /// Koristi se za slanje informacija o računu (RacunZahtjev).</summary>
        /// <param name="racun">Objekt tipa Schema.RacunType koji sadrži informacije o računu.</param>
        /// <param name="certificateSubject">Naziv certifikata koji se koristi, na primjer "FISKAL 1".</param>
        /// <example>
        ///  Raverus.FiskalizacijaDEV.Schema.RacunType racun = new Schema.RacunType();
        ///  XmlDocument doc = cis.PosaljiRacun(racun, "FISKAL 1");
        /// </example>
        /// <returns>
        /// Vraća XmlDocument koji sadrži XML poruku vraćeno od CIS-a. U slučaju greške, vraća null.</returns>
        public XmlDocument PosaljiRacun(Schema.RacunType racun, string certificateSubject)
        {
            XmlDocument racunOdgovor = null;

            Schema.RacunZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajRacunZahtjev(racun);
            XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajRacunZahtjev(zahtjev);

            PosaljiZahtjev(certificateSubject, ref racunOdgovor, zahtjevXml);

            return racunOdgovor;
        }

        public XmlDocument PosaljiRacun(Schema.RacunType racun, X509Certificate2 certifikat)
        {
            XmlDocument racunOdgovor = null;

            Schema.RacunZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajRacunZahtjev(racun);
            XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajRacunZahtjev(zahtjev);

            PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(zahtjevXml, certifikat);
            PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref zahtjevXml);

            racunOdgovor = SendSoapMessage(zahtjevXml);


            return racunOdgovor;
        }

        public XmlDocument PosaljiRacun(Schema.RacunType racun, string certificateSubject, StoreLocation storeLocation, StoreName storeName)
        {
            XmlDocument racunOdgovor = null;

            Schema.RacunZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajRacunZahtjev(racun);
            XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajRacunZahtjev(zahtjev);

            PosaljiZahtjev(certificateSubject, storeLocation, storeName, ref racunOdgovor, zahtjevXml);

            return racunOdgovor;
        }

        public XmlDocument PosaljiRacun(Schema.RacunType racun, string certificateSubject, DateTime datumVrijeme)
        {
            XmlDocument racunOdgovor = null;

            Schema.RacunZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajRacunZahtjev(racun, datumVrijeme);
            XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajRacunZahtjev(zahtjev);

            PosaljiZahtjev(certificateSubject, ref racunOdgovor, zahtjevXml);

            return racunOdgovor;
        }

        public XmlDocument PosaljiRacun(Schema.RacunType racun, X509Certificate2 certifikat, DateTime datumVrijeme)
        {
            XmlDocument racunOdgovor = null;

            Schema.RacunZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajRacunZahtjev(racun, datumVrijeme);
            XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajRacunZahtjev(zahtjev);

            PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(zahtjevXml, certifikat);
            PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref zahtjevXml);

            racunOdgovor = SendSoapMessage(zahtjevXml);


            return racunOdgovor;
        }

        public XmlDocument PosaljiRacun(Schema.RacunType racun, string certificateSubject, StoreLocation storeLocation, StoreName storeName, DateTime datumVrijeme)
        {
            XmlDocument racunOdgovor = null;

            Schema.RacunZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajRacunZahtjev(racun, datumVrijeme);
            XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajRacunZahtjev(zahtjev);

            PosaljiZahtjev(certificateSubject, storeLocation, storeName, ref racunOdgovor, zahtjevXml);

            return racunOdgovor;
        }
        #endregion


        #region ProvjeraZahtjev
        /// <summary>
        /// Koristi se za slanje informacija o provjeri računu (ProvjeraZahtjev).</summary>
        /// <param name="racun">Objekt tipa Schema.RacunType koji sadrži informacije o računu.</param>
        /// <param name="certificateSubject">Naziv certifikata koji se koristi, na primjer "FISKAL 1".</param>
        /// <returns>
        /// Vraća XmlDocument koji sadrži XML poruku vraćeno od CIS-a. U slučaju greške, vraća null.</returns>
        public XmlDocument PosaljiProvjeruRacuna(Schema.RacunType racun, string certificateSubject)
        {
            XmlDocument provjeraOdgovor = null;

            Schema.ProvjeraZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajProvjeraZahtjev(racun);
            XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajProvjeraZahtjev(zahtjev);

            PosaljiZahtjev(certificateSubject, ref provjeraOdgovor, zahtjevXml);

            return provjeraOdgovor;
        }

        public XmlDocument PosaljiProvjeruRacuna(Schema.RacunType racun, X509Certificate2 certifikat)
        {
            XmlDocument racunOdgovor = null;

            Schema.ProvjeraZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajProvjeraZahtjev(racun);
            XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajProvjeraZahtjev(zahtjev);

            PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(zahtjevXml, certifikat);
            PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref zahtjevXml);

            racunOdgovor = SendSoapMessage(zahtjevXml);


            return racunOdgovor;
        }

        public XmlDocument PosaljiProvjeruRacuna(Schema.RacunType racun, string certificateSubject, StoreLocation storeLocation, StoreName storeName)
        {
            XmlDocument racunOdgovor = null;

            Schema.ProvjeraZahtjev zahtjev = PopratneFunkcije.XmlDokumenti.KreirajProvjeraZahtjev(racun);
            XmlDocument zahtjevXml = PopratneFunkcije.XmlDokumenti.SerijalizirajProvjeraZahtjev(zahtjev);

            PosaljiZahtjev(certificateSubject, storeLocation, storeName, ref racunOdgovor, zahtjevXml);

            return racunOdgovor;
        }

        public XmlDocument PosaljiProvjeruRacuna(XmlDocument zahtjevXml)
        {
            XmlDocument provjeraOdgovor = SendSoapMessage(zahtjevXml);

            return provjeraOdgovor;
        }


        #endregion



        #region Echo
        /// <summary>
        /// Koristi se za slanje ECHO poruke u CIS.</summary>
        /// <param name="poruka">Tekst poruke koja se šalje, na primjer 'test' ili 'test poruka' ili sl. Ukoliko se radi o praznom stringu (""), tada će tekst poruke biti 'echo test'.</param>
        /// <example>
        ///  Raverus.FiskalizacijaDEV.CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();
        ///  XmlDocument doc = cis.PosaljiEcho("");
        /// </example>
        /// <returns>
        /// Vraća XmlDocument koji sadrži XML poruku vraćeno od CIS-a. U slučaju greške, vraća null.</returns>
        public XmlDocument PosaljiEcho(string poruka)
        {
            XmlDocument echoOdgovor = null;

            XmlDocument echoZahtjev = PopratneFunkcije.XmlDokumenti.DohvatiPorukuEchoZahtjev(poruka);
            if (echoZahtjev != null)
            {
                echoOdgovor = new XmlDocument();
                echoOdgovor = SendSoapMessage(echoZahtjev);
            }


            return echoOdgovor;
        }

        /// <summary>
        /// Koristi se za slanje ECHO poruke u CIS.</summary>
        /// <remarks>
        /// Namjena je ove metode da jednostavnim pozivom utvrdite da li servis radi ili ne.
        /// </remarks>
        /// <example>
        ///  
        ///  
        /// </example>
        /// <returns>
        /// Vraća True ukoliko je sve u redu i ukoliko je CIS vratio isti tekst poruke koji je i poslan. U suprotnom vraća False.</returns>
        public bool Echo()
        {
            return Echo("");
        }

        /// <summary>
        /// Koristi se za slanje ECHO poruke u CIS.</summary>
        /// <remarks>
        /// Namjena je ove metode da jednostavnim pozivom utvrdite da li servis radi ili ne.
        /// </remarks>
        /// <param name="poruka">Tekst poruke koja se šalje, na primjer 'test' ili 'test poruka' ili sl. Ukoliko se radi o praznom stringu (""), tada će tekst poruke biti 'echo test'.</param>
        /// <example>
        ///  
        ///  
        /// </example>
        /// <returns>
        /// Vraća True ukoliko je sve u redu i ukoliko je CIS vratio isti tekst poruke koji je i poslan. U suprotnom vraća False.</returns>
        public bool Echo(string poruka)
        {
            bool echo = false;

            XmlDocument echoOdgovor = PosaljiEcho(poruka);
            if (echoOdgovor != null && echoOdgovor.DocumentElement != null)
            {
                string odgovor = echoOdgovor.DocumentElement.InnerText.Trim();

                Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.FormatirajEchoPoruku(ref poruka);

                if (poruka == odgovor)
                    echo = true;
            }

            return echo;
        }
        #endregion


        #region Razno
        /// <summary>
        /// Koristi se za slanje SOAP poruke u CIS.</summary>
        /// <remarks>
        /// XML dokument koji se šalje mora biti u skladu sa Tehničkom dokumentacijom.
        /// Namjena je ove metode da sami pripremite XML poruku, stavite SOAP zaglavlje, potpišete je i zatim je pošaljete koristeći ovu metodu.
        /// </remarks>
        /// <param name="soapPoruka">XML dokument koji šaljete u CIS.</param>
        /// <example>
        ///  
        ///  
        /// </example>
        /// <returns>
        /// Vraća XmlDocument koji sadrži XML poruku vraćeno od CIS-a. U slučaju greške, vraća null.</returns>
        public XmlDocument PosaljiSoapPoruku(XmlDocument soapPoruka)
        {
            return SendSoapMessage(soapPoruka);
        }

        public XmlDocument PosaljiSoapPoruku(string soapPoruka)
        {
            return SendSoapMessage(PopratneFunkcije.XmlDokumenti.UcitajXml(soapPoruka));
        }
        #endregion


        #region Private
        private XmlDocument SendSoapMessage(XmlDocument soapMessage)
        {
            return SendSoapMessage(soapMessage, "");
        }

        private XmlDocument SendSoapMessage(XmlDocument soapMessage, string vvid)
        {
            XmlDocument responseSoapMessage = null;
            OdgovorGreska = null;


            if (SoapMessageSending != null)
            {
                CentralniInformacijskiSustavEventArgs ea = new CentralniInformacijskiSustavEventArgs() { SoapMessage = soapMessage };
               // 17.05.2017: !!! BE ADVICED !!! 
               // ovaj SoapMessageSending sadrzi
               // Application.DoEvents();
               // koji pak izvede da UI odgovara na npr. mouse click iako se vrti SubModul akcija FiskSveNeFisk... !!!
               // !!! tako da dok se ceka refiskalizacija user ipak moze kliktati npr. 'Novi' 
                SoapMessageSending(this, ea);

                if (ea.Cancel)
                    return responseSoapMessage;
            }

            SnimanjeDatoteka(NazivMapeZahtjev, soapMessage, vvid);


            try
            {
                Uri uri;

                if (!string.IsNullOrEmpty(CisUrl))
                    uri = new Uri(CisUrl);
                else
                    uri = new Uri(cisUrl);

#if (!WindowsXP)
                // - DK -
                // TLS1.2 and TLS1.1 integration

                // byQukatz: ovaj 'ServicePointManager.SecurityProtocol' od , valjda, 09.01.2017. treba odkomentirati                               
                // ...sto znaci da od tada na fiskalizaciju idemo samo sa (Tls12 | Tls11) protokolim                                                
                // u tekucoj 2016 verziji Vektora za SecurityProtocol debbugger prikazuje (Ssl3 | Tls) sto je valjda defaultno ponasanje .NET-a 4.0 

#if DotNet45
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
#endif
                ///////////////////
#endif
                if (!TokenExists())
                {
                    HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
                    if (request != null)
                    {
                        //ServicePointManager.Expect100Continue = true; //http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k(SYSTEM.NET.SERVICEPOINTMANAGER.EXPECT100CONTINUE)&rd=true

                        if (TimeOut > 0)
                            request.Timeout = TimeOut;

                        request.ContentType = "text/xml";
                        request.Method = "POST";

                        //request.Headers = new WebHeaderCollection();
                        //request.Headers.Add("SOAPAction", webMethod);


                        byte[] by = UTF8Encoding.UTF8.GetBytes(soapMessage.InnerXml);
                        request.ProtocolVersion = HttpVersion.Version11;
                        request.ContentLength = by.Length;


                        using (Stream requestStream = request.GetRequestStream())
                        {
                            requestStream.Write(by, 0, by.Length);
                        }

                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                        if (response != null)
                        {
                            Stream responseStream = response.GetResponseStream();
                            Encoding encode = Encoding.GetEncoding("utf-8");
                            StreamReader readStream = new StreamReader(responseStream, encode);
                            string txt = readStream.ReadToEnd();

                            responseSoapMessage = new XmlDocument();
                            responseSoapMessage.PreserveWhitespace = true;
                            responseSoapMessage.LoadXml(txt);

                            SnimanjeDatoteka(NazivMapeOdgovor, responseSoapMessage, vvid);

                            if (SoapMessageSent != null)
                            {
                                EventArgs ea = new EventArgs();
                                SoapMessageSent(this, ea);
                            }
                        }
                    }
                }
                else
                    responseSoapMessage = FiskalizirajWebService(soapMessage);
            }
            catch (WebException ex)
            {
                // prema sugestiji mladenbabic (http://fiskalizacija.codeplex.com/workitem/627)
                // prema sugestiji dkustec (http://fiskalizacija.codeplex.com/workitem/637)

                OdgovorGreskaStatus = ex.Status;

                WebResponse ipakPristigliErrorXmlResponse = ((WebException)ex).Response;
                if (ipakPristigliErrorXmlResponse != null)
                {
                    using (Stream noviResponseStream = ipakPristigliErrorXmlResponse.GetResponseStream())
                    {
                        StreamReader responseReader = new StreamReader(noviResponseStream);
                        OdgovorGreska = new XmlDocument();
                        OdgovorGreska.Load(responseReader);

                        // byQukatz:
                        string theGreskaTekst = ZXC.VvXmlElementValue(OdgovorGreska.InnerXml, "PorukaGreske", "tns");

                        ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, theGreskaTekst);
                        Trace.TraceError("Greška kod slanja SOAP poruke. Primljen odgovor od CIS-a, detalji greške u CentralniInformacijskiSustav.OdgovorGreska.");
                        throw ex;
                    }
                }
                else
                {
                    Trace.TraceError(String.Format("Greška kod slanja SOAP poruke. Status greške (prema http://msdn.microsoft.com/en-us/library/system.net.webexceptionstatus.aspx): {0}. Poruka greške: {1}", ex.Status, ex.Message));
                    throw ex;
                }
            }

            catch (System.Web.Services.Protocols.SoapException ex)
            {
                OdgovorGreskaStatus = null;

                if (ex.Detail != null)
                {
                    OdgovorGreska = new XmlDocument();
                    OdgovorGreska.LoadXml(string.Format(@"<?xml version=""1.0"" encoding=""UTF - 8""?><soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">{0}</soap:Envelope>", ex.Detail.InnerXml));

                    // byQukatz:
                    string theGreskaTekst = ZXC.VvXmlElementValue(OdgovorGreska.InnerXml, "PorukaGreske", "tns");

                    ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, theGreskaTekst);
                    Trace.TraceError("Greška kod slanja SOAP poruke. Primljen odgovor od CIS-a, detalji greške u CentralniInformacijskiSustav.OdgovorGreska.");
                    throw ex;
                }
                else
                {
                    Trace.TraceError(String.Format("Greška kod slanja SOAP poruke. Status greške (prema http://msdn.microsoft.com/en-us/library/system.net.webexceptionstatus.aspx): {0}. Poruka greške: {1}", ex.Message, ex.Message));
                    throw ex;
                }
            }

            catch (Exception ex)
            {
                Trace.TraceError(String.Format("Greška kod slanja SOAP poruke: {0}", ex.Message));
                throw ex;
            }


            return responseSoapMessage;
        }

        private void SnimanjeDatoteka(string mapa, XmlDocument dokument, string vvid)
        {
            if (!string.IsNullOrEmpty(mapa) && dokument != null)
            {
                PopratneFunkcije.TipDokumentaEnum tipDokumenta = PopratneFunkcije.XmlDokumenti.OdrediTipDokumenta(dokument);

                if (tipDokumenta != PopratneFunkcije.TipDokumentaEnum.Nepoznato)
                {
                    DirectoryInfo di = PopratneFunkcije.Razno.GenerirajProvjeriMapu(mapa);
                    if (di != null)
                    {
                        string fileName = "";

                        if (!NazivAutoGeneriranje)
                            fileName = Path.Combine(mapa, String.Format("{0}.xml", tipDokumenta));
                        else
                        {
                            string uuid = PopratneFunkcije.XmlDokumenti.DohvatiUuid(dokument, tipDokumenta);
                           // byQukatz:
                           string vvTS = /*" @_" +*/ DateTime.Now.ToString(ZXC.VvTimeStampFormat4FileName);
                           string vvTD = ZXC.GetShortFiskTipDokumenta(tipDokumenta);
                         //if(!string.IsNullOrEmpty(uuid))
                         //  fileName = Path.Combine(mapa, String.Format("{0}_{2}_{1}.xml"    , tipDokumenta, uuid, vvTS));
                             fileName = Path.Combine(mapa, String.Format("{0}_{1}_{2}_{3}.xml", vvTD        , vvid, vvTS, uuid));
                        }

                        if (!string.IsNullOrEmpty(fileName))
                            PopratneFunkcije.XmlDokumenti.SnimiXmlDokumentDatoteka(dokument, fileName);
                    }
                }
            }
        }

        // byQukatz: sa certifikatFileName-om i certPassword-om iz prjkt_rec-a 
        /// <summary>
        /// byQukatz: sa certifikatFileName-om i certPassword-om iz prjkt_rec-a 
       /// </summary>
       /// <param name="certifikatDatoteka"></param>
       /// <param name="zaporka"></param>
       /// <param name="racunOdgovor"></param>
       /// <param name="zahtjevXml"></param>
        private void PosaljiZahtjev(/*string certifikatDatoteka, string zaporka,*/ ref XmlDocument racunOdgovor, XmlDocument zahtjevXml, string vvid)
        {
           // byQukatz:
           ZXC.RemoveEmptyNodes(zahtjevXml);

           if(zahtjevXml != null && !string.IsNullOrEmpty(zahtjevXml.InnerXml))
           {
              X509Certificate2 certificate = PopratneFunkcije.Potpisivanje.DohvatiCertifikat_FromPrjkt(); // VOILA by Q 
              if(certificate != null)
              {
                 PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(zahtjevXml, certificate);
                 PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref zahtjevXml);

                 racunOdgovor = SendSoapMessage(zahtjevXml, vvid);
              }
           }
        }

        // byQukatz: sa certifikatFileName-om i certPassword-om 
       /// <summary>
        /// byQukatz: sa certifikatFileName-om i certPassword-om 
       /// </summary>
       /// <param name="certifikatDatoteka"></param>
       /// <param name="zaporka"></param>
       /// <param name="racunOdgovor"></param>
       /// <param name="zahtjevXml"></param>
        private void PosaljiZahtjev(string certifikatDatoteka, string zaporka, ref XmlDocument racunOdgovor, XmlDocument zahtjevXml)
        {
           // byQukatz:
           ZXC.RemoveEmptyNodes(zahtjevXml);

           if(zahtjevXml != null && !string.IsNullOrEmpty(zahtjevXml.InnerXml))
           {
              X509Certificate2 certificate = PopratneFunkcije.Potpisivanje.DohvatiCertifikat(certifikatDatoteka, zaporka);
              if(certificate != null)
              {
                 PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(zahtjevXml, certificate);
                 PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref zahtjevXml);

                 racunOdgovor = SendSoapMessage(zahtjevXml);
              }
           }
        }

        private void PosaljiZahtjev(string certificateSubject, ref XmlDocument racunOdgovor, XmlDocument zahtjevXml)
        {
           // byQukatz:
           ZXC.RemoveEmptyNodes(zahtjevXml);

            if (zahtjevXml != null && !string.IsNullOrEmpty(zahtjevXml.InnerXml))
            {
                X509Certificate2 certificate = PopratneFunkcije.Potpisivanje.DohvatiCertifikat(certificateSubject);
                if (certificate != null)
                {
                    PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(zahtjevXml, certificate);
                    PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref zahtjevXml);

                    racunOdgovor = SendSoapMessage(zahtjevXml);
                }
            }
        }

        private void PosaljiZahtjev(string certificateSubject, StoreLocation storeLocation, StoreName storeName, ref XmlDocument racunOdgovor, XmlDocument zahtjevXml)
        {
           // byQukatz:
           ZXC.RemoveEmptyNodes(zahtjevXml);

            // prema sugestiji dkustec: http://fiskalizacija.codeplex.com/workitem/693
            if (zahtjevXml != null && !string.IsNullOrEmpty(zahtjevXml.InnerXml))
            {
                X509Certificate2 certificate = PopratneFunkcije.Potpisivanje.DohvatiCertifikat(certificateSubject, storeLocation, storeName);
                if (certificate != null)
                {
                    PopratneFunkcije.Potpisivanje.PotpisiXmlDokument(zahtjevXml, certificate);
                    PopratneFunkcije.XmlDokumenti.DodajSoapEnvelope(ref zahtjevXml);

                    racunOdgovor = SendSoapMessage(zahtjevXml);
                }
            }
        }
        #endregion

        #region WindowsXP
        private string GetTokenFromConfigFile()
        {
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings["token"] != null && !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["token"]))
                    return System.Configuration.ConfigurationManager.AppSettings["token"];
                else
                    return Token;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool TokenExists()
        {
            return !string.IsNullOrEmpty(GetTokenFromConfigFile());
        }

        private bool CisProdukcija()
        {
            if (!string.IsNullOrEmpty(CisUrl) && CisUrl.ToLower() == "https://cis.porezna-uprava.hr:8449/FiskalizacijaService".ToLower())
                return true;
            else
                return false;
        }

        private bool ArchiveXmlMessages()
        {
            return ReadBoolValueFromConfig("ArhivirajXmlPoruke");
        }

        private bool ParseXmlMessages()
        {
            return ReadBoolValueFromConfig("ParsirajXmlPoruke");
        }

        private bool ReadBoolValueFromConfig(string valueName)
        {
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings[valueName] != null && !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[valueName]))
                {
                    bool result;

                    bool test = bool.TryParse(System.Configuration.ConfigurationManager.AppSettings[valueName], out result);
                    if (test)
                        return result;
                    else
                        return false;
                }
                else
                {
                    if (valueName == "ArhivirajXmlPoruke")
                        return ArhivirajXmlPoruke;
                    else if (valueName == "ParsirajXmlPoruke")
                        return ParsirajXmlPoruke;
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private XmlDocument FiskalizirajWebService(XmlDocument soapPoruka)
        {
            XmlDocument odgovor = null;

            string token = GetTokenFromConfigFile();

#if NeznamKakSpojitiStaticWebReference

            using (FDev.WebService fdev = new FDev.WebService())
            {
                if (TimeOut > 0)
                    fdev.Timeout = TimeOut;

                string tmp = fdev.SendData(CisProdukcija(), token, soapPoruka.InnerXml, ArchiveXmlMessages(), ParseXmlMessages(), TimeOut);
                if (!string.IsNullOrEmpty(tmp))
                {
                    odgovor = new XmlDocument();
                    odgovor.LoadXml(tmp);


                    SnimanjeDatoteka(NazivMapeOdgovor, odgovor);

                    if (SoapMessageSent != null)
                    {
                        EventArgs ea = new EventArgs();
                        SoapMessageSent(this, ea);
                    }
                }
            }
#endif
            return odgovor;
        }


        #endregion


        #region Properties
        /// <summary>
        /// Naziv mape (foldera) u koji će se spremati XML dokumenti za zahtjeve. Ukoliko vrijednost nije postavljena, dokumenti se neće snimati.
        /// </summary>
        public string NazivMapeZahtjev { get; set; }

        /// <summary>
        /// Naziv mape (foldera) u koji će se spremati XML dokumenti za odgovore. Ukoliko vrijednost nije postavljena, dokumenti se neće snimati.
        /// </summary>
        public string NazivMapeOdgovor { get; set; }

        /// <summary>
        /// Određuje da li se naziv generira automatski koristeći UUID ili datoteka uvijek ima isti naziv
        /// </summary>
        /// <remarks>
        /// Ako je vrijednost true, naziv datoteke će biti određen koristeći naziv tipa dokumenta i UUID-a, ako je false naziv datoteke će biti uvijek isti i biti će određen tipom dokumenta.
        /// Ne koristi se ukoliko NazivMapeZahtjev odnosno NazivMapeOdgovor nisu postavljeni na odgovarajuću vrijednost.
        /// Nema smisla postavljati na TRUE za ECHO.
        /// </remarks>
        public bool NazivAutoGeneriranje { get; set; }

        /// <summary>
        /// Ukoliko je CIS vratio odgovor i ukoliko taj odgovor sadrži tehničkom specifikacijom propisanu grešku, tada OdgovorGreska sadži vraćenu XML poruku. U suprotnom, vrijednost je NULL.
        /// </summary>
        public XmlDocument OdgovorGreska { get; set; }

        /// <summary>
        /// Vraća WebExceptionStatus greške (http://msdn.microsoft.com/en-us/library/system.net.webexceptionstatus.aspx). U suprotnom, vrijednost je NULL.
        /// </summary>
        public WebExceptionStatus? OdgovorGreskaStatus { get; set; }

        /// <summary>
        /// Vrijednost, u milisekundama, za HttpWebRequest.TimeOut, odnosno, za TimeOut kod komunikacije sa CIS web servisom.
        /// Ako je vrijednost 0 (nula), property se ignorira (ne postavlja se vrijednost za HttpWebRequest.TimeOut).
        /// http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k(SYSTEM.NET.HTTPWEBREQUEST.TIMEOUT)&rd=true
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// Adresa CIS web servisa; ako vrijednost nije postavljena, koristi se trenutna adresa TEST CIS web servisa koja je u službenoj upotrebi.
        /// </summary>
        public string CisUrl { get; set; }

        /// <summary>
        /// Koristi se samo za podršku za Windows XP. Token možete dobiti na https://www.fdev.hr/Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Ne koristi se.
        /// </summary>
        public bool ArhivirajXmlPoruke { get; set; }

        /// <summary>
        /// Ne koristi se.
        /// </summary>
        public bool ParsirajXmlPoruke { get; set; }
        #endregion
    }
}
