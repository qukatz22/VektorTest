using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Diagnostics.Eventing.Reader;
using System.Web.UI.WebControls;
using Raverus.FiskalizacijaDEV.Schema;



#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand = MySql.Data.MySqlClient.MySqlCommand;
#endif

namespace EN16931.UBL
{
   public partial class InvoiceType
   {

      #region Create eRacun object (InvoiceType) From Faktur

      private static string currentLocalDirectory;
      public static string CurrentLocalDirectory
      {

         get
         {
            if(currentLocalDirectory.IsEmpty())
            {
               string deaultVvPDFdirectoryName = VvForm.GetLocalDirectoryForVvFile(VvPref.VvMailData.DeaultVvPDFdirectoryName);
               string todayDir = "XYZ" + "_PDF_" + DateTime.Today.ToString(ZXC.VvDateYyyyMmDdMySQLFormat);

               currentLocalDirectory = Path.Combine(deaultVvPDFdirectoryName, todayDir);
            }

            return currentLocalDirectory;
         }
         set { currentLocalDirectory = value; }
      }

      [System.Xml.Serialization.XmlIgnore]
      public Outgoing_eRacun_parameters The_OERP { get; set; }
      //public static EN16931.UBL.InvoiceType Create_eRacun_fromFaktur(
      //   Faktur ORIG_faktur_rec,
      //   PrnFakDsc thePFD,
      //   Kupdob kupdob_rec,
      //   Kupdob primPlat_rec,
      //   List<Artikl> artiklSifrar,
      //   byte[] PDF_as_base64_bytes,
      //   string pdf_fileName)
      public static EN16931.UBL.InvoiceType Create_eRacun_fromFaktur(XSqlConnection conn, Outgoing_eRacun_parameters oeRp, List<Artikl> artiklSifrar, bool isIRMcalcB)
      {
         #region Init

         Faktur ORIG_faktur_rec = oeRp.faktur_rec;
         PrnFakDsc thePFD = oeRp.thePFD;
         Kupdob kupdob_rec = oeRp.kupdob_rec;
         Kupdob primPlat_rec = oeRp.primPlat_rec;
         byte[] PDF_as_base64_bytes = oeRp.PDF_as_base64_byteArray;
         byte[] ADR_as_base64_bytes = oeRp.ADR_as_base64_byteArray;
         string pdf_fileName = oeRp./*pdfFileNameOnly*/qwePDFfileNameOnly;
         string adr_fileName = null;

         EN16931.UBL.InvoiceType the_eRacun = new EN16931.UBL.InvoiceType();

         the_eRacun.The_OERP = oeRp;

         bool isMojeRacunNotFina = true; // todo kao parametar 
                                         // 2026: gasimo 
                                         //bool isAVANS            = (ORIG_faktur_rec.IsAVANS_STORNO || ORIG_faktur_rec.Is_AfterAvans_PrihodTTa);



         //bool isClassicSTORNO = ORIG_faktur_rec.S_ukKCRP.IsNegative() && ORIG_faktur_rec.Napomena.ToUpper().Contains("STORNO");
         //
         //bool isAvans        = ORIG_faktur_rec.PdvKolTip == ZXC.VvUBL_PolsProcEnum.P04 &&
         //                      ORIG_faktur_rec.StatusCD  == "386"                      &&
         //                      ORIG_faktur_rec.S_ukKCRP.IsPositive()                    ;
         //
         //bool isAvansSTORNO  = ORIG_faktur_rec.PdvKolTip == ZXC.VvUBL_PolsProcEnum.P04 &&
         //                      ORIG_faktur_rec.StatusCD  == "384"                      &&
         //                      ORIG_faktur_rec.S_ukKCRP.IsNegative();
         //
         //bool isFinalRn      = ORIG_faktur_rec.PdvKolTip == ZXC.VvUBL_PolsProcEnum.P11 &&
         //                      ORIG_faktur_rec.StatusCD  == "380"                       ;

         bool isAvans = ORIG_faktur_rec.Is_F2_Avans;
         bool isAvansSTORNO = ORIG_faktur_rec.Is_F2_AvansSTORNO;
         bool isFinalRn = ORIG_faktur_rec.Is_F2_FinalRn;
         bool isClassicSTORNO = ORIG_faktur_rec.Is_F2_ClassicSTORNO;

         // 2026: 
         bool needsReferencaNaPrethodni = isClassicSTORNO || isAvansSTORNO || isFinalRn;
         //bool needsReferencaNaPrethodniPrethodni =                                     isFinalRn;

         if(needsReferencaNaPrethodni)
         {
            bool isYRN = ORIG_faktur_rec.V1_tt == Faktur.TT_YRN && ORIG_faktur_rec.V1_ttNum.NotZero();
            if(isYRN)
            {
               // do nothing, ne treba ti F2_PrvFakYYiRecID 
            }
            else if(ORIG_faktur_rec.F2_PrvFakYYiRecID.IsZero())// classic provjera 
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, $"Ovaj račun treba referencu na prethodni dokument, a NEMA JE!?{Environment.NewLine}{Environment.NewLine}Ne mogu izraditi eRačun.");
               return null;
            }
         }

         #endregion Init

         #region podvali IRM kao IRA za Calc

         bool isIRM = ORIG_faktur_rec.TT == Faktur.TT_IRM;

         Faktur faktur_rec;

         if(isIRM)
         {
            if(isIRMcalcB) faktur_rec = ORIG_faktur_rec.Convert_IRM_Faktur_To_IRA_Faktur_B();
            else faktur_rec = ORIG_faktur_rec.Convert_IRM_Faktur_To_IRA_Faktur_A();
         }
         else // classic 
         {
            faktur_rec = (Faktur)ORIG_faktur_rec.CreateNewRecordAndCloneItComplete();
         }

         #endregion podvali IRM kao IRA za Calc

         #region Set eRacun Values From Faktur

         #region ZAGLAVLJE računa

         the_eRacun.ID = new IDType { Value = Fak2eR__String("BT001", faktur_rec, null) }; //BT-1 Broj računa 	 Identifikator 1..1 
         the_eRacun.InvoiceTypeCode = new InvoiceTypeCodeType { Value = Fak2eR__String("BT003", faktur_rec, null) }; //BT-3 Šifra vrste 	 Šifra         1..1 
         the_eRacun.IssueDate = new IssueDateType { Value = Fak2eR____Date("BT002", faktur_rec, null) }; //BT-2 Datum izdavanja Datum        1..1 
         the_eRacun.TaxPointDate = new TaxPointDateType { Value = Fak2eR____Date("BTqwe", faktur_rec, null) }; //BT-2 Datum izdavanja Datum        1..1 

         if(oeRp.wantsKOPIJA)
         {
            the_eRacun.CopyIndicator = new CopyIndicatorType { Value = oeRp.wantsKOPIJA };
         }

         // 24.06.2019. moj-eracun kaze da ovo ne postoji u EU normi    
         // 14.10.2025 - ipak postoji u EU normi ali cemo ga tek u 2026 
         //if(isMojeRacunNotFina == false)
         if(ZXC.IsF2_2026_rules)
            the_eRacun.IssueTime = new IssueTimeType { Value = Fak2eR____Date("BT002 Time", faktur_rec, null) }; //BT-2 Vrijeme izdavanja Time   1..1 
         /* byQ timeAbrakakobredabra: */
         //the_eRacun.IssueTime            = new IssueTimeType            { ValueFormatted = faktur_rec.DokDate.ToString(ZXC.VvTimeOnlyFormat2) }; //BT-2 Vrijeme izdavanja Time   1..1 

         the_eRacun.DueDate = new DueDateType { Value = Fak2eR____Date("BT002 Dosp", faktur_rec, null) }; //BT-2 Dospjece placanja Datum  1..1 
         the_eRacun.DocumentCurrencyCode = new DocumentCurrencyCodeType { Value = Fak2eR__String("BT005", faktur_rec, null) }; //BT-5 Šifra valute 	 Šifra     1..1 
         the_eRacun.TaxCurrencyCode = new TaxCurrencyCodeType { Value = Fak2eR__String("BT006", faktur_rec, null) }; //????BT-6 Šifra valute obracunatog PDV-a valjda

         if(faktur_rec.PdvKolTip_u.NotZero()) // u 'PdvKolTip_u' napucavamo P1, ... P12 - Poslovni Proces eRacuna 
         {
            the_eRacun.ProfileID = new ProfileIDType { Value = Fak2eR__String("BT023", faktur_rec, null) }; //BT-23 Vrsta poslovnog procesa ProfileID 	 Šifra      1..1 
         }

         //BT-13 Referenca na narudžbenicu
         if(faktur_rec.OpciAvalue.NotEmpty())
         {
            the_eRacun.OrderReference = new OrderReferenceType
            {
               ID = new IDType { Value = Fak2eR__String("BT013", faktur_rec, null) }
            };

            //BT-16 Referenca otpremnice 
            //19.01.2026. za Frag - ako ima narudžbenicu da onda stavimo i otpremnicu broja istog kao račun, da li fiskal ili ttnum 
            // tu cemo doc u probleme ako netko drugi ne koristi opciA za narudzbenicu                                              
            // ili ako ovim automatizmom izmisljeni brij otpremnice smeta                                                           
            the_eRacun.DespatchDocumentReference = new DocumentReferenceType[]
            {
               new DocumentReferenceType
               {
                  ID = new IDType { Value =  faktur_rec.TtNumFiskal      }
                //ID = new IDType { Value =  faktur_rec.TtNum.ToString() }
               }
            };
         }

         #region napomena

         //BT-22 Napomena na računu	Tekst 1..1
         List<NoteType> noteList = new List<NoteType>();

         //14.10.2025. ovo od 1.1.26. ide zasebno te se moze maknuti iz napomene!!!!
         //noteList.Add(new NoteType { Value = Fak2eR__String("BT022 operater", faktur_rec, null) });
         //noteList.Add(new NoteType { Value = Fak2eR__String("BT022 vrijemeR", faktur_rec, null) });
         if(!ZXC.IsF2_2026_rules)
         {
            noteList.Add(new NoteType { Value = Fak2eR__String("BT022 operater", faktur_rec, null) });
            noteList.Add(new NoteType { Value = Fak2eR__String("BT022 vrijemeR", faktur_rec, null) });
         }

         noteList.Add(new NoteType { Value = Prj2eR__String("BT022 odgOsoba") });

         if(faktur_rec.Napomena.NotEmpty()) noteList.Add(new NoteType { Value = Fak2eR__String("BT022 napomena", faktur_rec, null) });
         if(faktur_rec.Opis.NotEmpty()) noteList.Add(new NoteType { Value = Fak2eR__String("BT022 opis", faktur_rec, null) });

         the_eRacun.Note = noteList.ToArray();

         #endregion napomena

         #region VvUBL_CustomizationID

         //BT-24 Identifikator specifikacije	Identifikator 1..1
         the_eRacun.CustomizationID = new CustomizationIDType { Value = Fak2eR__String("BT024", faktur_rec, null) };

         #endregion VvUBL_CustomizationID

         #region Referenca STORNO i stornoAVANSa racuna

         // 2026: 
         Faktur prvFaktur_rec = null;
         if(needsReferencaNaPrethodni)
         {
            prvFaktur_rec = new Faktur();

            int prevFakYear;
            uint prevFakRecID;

            string refFiskalBr;
            DateTime refDokDate;

            bool isYRN = faktur_rec.V1_tt == Faktur.TT_YRN && faktur_rec.V1_ttNum.NotZero();

            if(!isYRN) // classic 
            {
               (prevFakYear, prevFakRecID) = ZXC.GetYearAndRecIDFrom_YYandRecID(faktur_rec.F2_PrvFakYYiRecID);
            }
            else
            {
               prevFakYear = 0;
               prevFakRecID = 0;
            }

            if(isYRN)
            {
               ZXC.aim_emsg(MessageBoxIcon.Warning, "Ovo je YRN!");

               bool YRN_OK = FakturDao.SetMeFaktur(conn, prvFaktur_rec, faktur_rec.V1_tt, faktur_rec.V1_ttNum, false);

               if(YRN_OK)
               {
                  string refFiskalBr_fromYRN_VezniDok2 = Get_refFiskalBr_fromYRN_VezniDok2(prvFaktur_rec/*.VezniDok2*/);

                  refFiskalBr = refFiskalBr_fromYRN_VezniDok2;
                  refDokDate = prvFaktur_rec.DokDate;
               }
               else
               {
                  ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu naći YRN fakturu za referencu!");
                  return null;
               }
            }
            else if(prevFakYear == ZXC.projectYearAsInt) // prev fak je u ovoj godini 
            {
               prvFaktur_rec.VvDao.SetMe_Record_byRecID_Complete(conn, /*faktur_rec.F2_PrvFakYYiRecID*/ prevFakRecID, prvFaktur_rec);

               refFiskalBr = prvFaktur_rec.VezniDok;
               refDokDate = prvFaktur_rec.DokDate;
            }
            else // prev fak je u nekoj prethodnoj godini 
            {
               prvFaktur_rec.VvDao.SetMe_Record_byRecID_Complete(ZXC.TheSecondDbConn_SameDB_OtherYear(prevFakYear), /*faktur_rec.F2_PrvFakYYiRecID*/ prevFakRecID, prvFaktur_rec);

               refFiskalBr = prvFaktur_rec./*VezniDok*/TtNumFiskal;
               refDokDate = prvFaktur_rec.DokDate;
            }

            if(ZXC.theSecondDbConnection != null) ZXC.theSecondDbConnection.Close(); // nemoj tu pozivaty propertyy nego koristi varijablu (malo slovo)

            if(refFiskalBr.IsEmpty() || refDokDate.IsEmpty())
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, $"refFiskalBr.IsEmpty() [{refFiskalBr}] || refDokDate.IsEmpty() [{refDokDate}]");
               return null;
            }

            the_eRacun.BillingReference = new BillingReferenceType[]
            {
               new BillingReferenceType
               {
                  InvoiceDocumentReference = new DocumentReferenceType
                  {
                   //ID        = new IDType        { Value = prvFaktur_rec.VezniDok },
                   //IssueDate = new IssueDateType { Value = prvFaktur_rec.DokDate  }
                     ID        = new IDType        { Value = refFiskalBr },
                     IssueDate = new IssueDateType { Value = refDokDate  }
                  }
               }
            };
         }

         #endregion Referenca STORNO i stornoAVANSa racuna

         #region PDF
         
         List<DocumentReferenceType> documentReferenceList = new List<DocumentReferenceType>();
         
         // Dodaj glavni PDF računa
         documentReferenceList.Add(new DocumentReferenceType
         {
            ID = new IDType { Value = "1" },
            DocumentDescription = new DocumentDescriptionType[] { new DocumentDescriptionType { Value = "Faktura" } },
            Attachment = new AttachmentType
            {
               EmbeddedDocumentBinaryObject = new EmbeddedDocumentBinaryObjectType
               {
                  filename = pdf_fileName,
                  mimeCode = "application/pdf",
                  Value    = PDF_as_base64_bytes,
               }
            }
         });
         
         // Dodaj dodatni PDF ako postoji
         if(faktur_rec.ExternLink1.NotEmpty() && ADR_as_base64_bytes != null)
         {
            adr_fileName = Path.GetFileName(faktur_rec.ExternLink1);
            documentReferenceList.Add(new DocumentReferenceType
            {
               ID = new IDType { Value = "2" },
               DocumentDescription = new DocumentDescriptionType[] { new DocumentDescriptionType { Value = "Dodatni dokument" } },
               Attachment = new AttachmentType
               {
                  EmbeddedDocumentBinaryObject = new EmbeddedDocumentBinaryObjectType
                  {
                     filename = adr_fileName,
                     mimeCode = "application/pdf",
                     Value    = ADR_as_base64_bytes,
                  }
               }
            });
         }
         
         the_eRacun.AdditionalDocumentReference = documentReferenceList.ToArray();
         
         #endregion PDF

         #region BG-4 BG-5 Prodavatelj (Prjkt)

         //BT-29 ID Prodavatelja 	Tekst 1..1
         the_eRacun.AccountingSupplierParty = new SupplierPartyType();
         the_eRacun.AccountingSupplierParty.Party = new PartyType();
         the_eRacun.AccountingSupplierParty.Party.PartyIdentification = new PartyIdentificationType[]
         {
            new PartyIdentificationType
            {
               ID = new IDType { Value = Prj2eR__String("BT029") }, // !!! SupplierID !!! ... ili ti ga 9934:OIB 
                                                                    // <cbc:ID>9934:60042587515</cbc:ID>         
            }
         };

         // 24.06.2019. - AccountingSupplierParty / Party / PartyName / Name - ovo polje dolazi odmah iza PartyIdentification i sadrži naziv tvrtke
         if(isMojeRacunNotFina == true)
         {
            the_eRacun.AccountingSupplierParty.Party.PartyName = new PartyNameType[]
            {
               new PartyNameType { Name = new NameType1 {Value = Prj2eR__String("BT027") } }
            };
         }

         //BT-27 Naziv Prodavatelja 	Tekst 1..1

         if(isMojeRacunNotFina == false)
         {
            the_eRacun.AccountingSupplierParty.Party.PartyLegalEntity = new PartyLegalEntityType[]
            {
               new PartyLegalEntityType
               {
                  RegistrationName = new RegistrationNameType { Value = Prj2eR__String("BT027") }
               }
            };
         }
         else// 24.06.2019. AccountingSupplierParty / Party / PartyLegalEntity / CompanyID - upisati OIB pošiljatelja: 60042587515
         {
            the_eRacun.AccountingSupplierParty.Party.PartyLegalEntity = new PartyLegalEntityType[]
            {
               new PartyLegalEntityType
               {
                  RegistrationName = new RegistrationNameType { Value = Prj2eR__String("BT027"    ) },
                  CompanyID        = new CompanyIDType        { Value = Prj2eR__String("OIB prjkt") }
               }
            };
         }

         //BT - 31 Porezniidentifikator Prodavatelja porezni broj s prefiksom zemlje
         the_eRacun.AccountingSupplierParty.Party.PartyTaxScheme = new PartyTaxSchemeType[]
         {
            new PartyTaxSchemeType
            {
               CompanyID = new CompanyIDType {                   Value = Prj2eR__String("BT031"     ) },
               TaxScheme = new TaxSchemeType { ID = new IDType { Value = Prj2eR__String("BT031 kind") } }
            }
         };


         //BT-34 Elektronička adresa Prodavatelja	Identifikator sheme  Identifikator1..1	Korisititi EMAIL
         if(isMojeRacunNotFina == false)
         {
            the_eRacun.AccountingSupplierParty.Party.EndpointID = new EndpointIDType { Value = Prj2eR__String("BT034") };
            the_eRacun.AccountingSupplierParty.Party.EndpointID.schemeID = "EMAIL";
         }
         else // 24.06.2019. moj-eracun zeli ovakoAccountingSupplierParty / Party / EndpointID - molim Vas da ga kreirate na sljedeći način:<cbc:EndpointID schemeID="9934">60042587515</cbc:EndpointID>
         {
            the_eRacun.AccountingSupplierParty.Party.EndpointID = new EndpointIDType { Value = Prj2eR__String("OIB prjkt") };
            the_eRacun.AccountingSupplierParty.Party.EndpointID.schemeID = "9934";
         }
         //BG-5 POŠTANSKA ADRESA PRODAVATELJA                               1..1
         //BT-35 Redak adrese Prodavatelja 1 Obično naziv ulice i brojTekst 0..1
         //BT-37 Grad Prodavatelja                                    Tekst 0..1
         //BT-38 Poštanski broj	                                     Tekst 0..1
         //BT-40 Šifra države Prodavatelja	Kod 1..1
         the_eRacun.AccountingSupplierParty.Party.PostalAddress = new AddressType
         {
            StreetName = new StreetNameType { Value = Prj2eR__String("BT035") },
            CityName = new CityNameType { Value = Prj2eR__String("BT037") },
            PostalZone = new PostalZoneType { Value = Prj2eR__String("BT038") },
            Country = new CountryType { IdentificationCode = new IdentificationCodeType { Value = Prj2eR__String("BT040") }, },

            AddressLine = new AddressLineType[]
            {
               new AddressLineType
               {
                  Line = new LineType { Value = Prj2eR__String("BT035x") },
               },
            }
         };

         the_eRacun.AccountingSupplierParty.Party.Contact = new ContactType
         {
            ElectronicMail = new ElectronicMailType { Value = Prj2eR__String("BT043") },
         };

         //14.10.2025. 
         if(ZXC.IsF2_2026_rules)
         {
            the_eRacun.AccountingSupplierParty.SellerContact = new ContactType();
            /*{
               ID = new IDType { Value = ZXC.TheVvForm.GetFisk_Oib_Oper(faktur_rec.AddUID) },
               Name = new NameType1 { Value = ZXC.TheVvForm.GetFisk_RecID_Oper(faktur_rec.AddUID) }
            } ;*/

            the_eRacun.AccountingSupplierParty.SellerContact.ID = new IDType { Value = ZXC.TheVvForm.GetFisk_Oib_Oper(faktur_rec.AddUID) };
            the_eRacun.AccountingSupplierParty.SellerContact.Name = new NameType1 { Value = ZXC.TheVvForm.GetFisk_RecID_Oper(faktur_rec.AddUID) };

         }

         #endregion BG-4 BG-5 Prodavatelj (Prjkt)

         #region BG -8 Kupac

         the_eRacun.AccountingCustomerParty = new CustomerPartyType();
         the_eRacun.AccountingCustomerParty.Party = new PartyType();

         // BT-46 identfikator kupca
         the_eRacun.AccountingCustomerParty.Party.PartyIdentification = new PartyIdentificationType[]
         {
            new PartyIdentificationType
            {
               ID = new IDType { Value = KiD2eR__String("BT046", kupdob_rec) },
            }
         };

         //BT-44 Ime Kupca 	Tekst 1..1

         the_eRacun.AccountingCustomerParty.Party.PartyName = new PartyNameType[]
         {
            new PartyNameType { Name = new NameType1 {Value = Fak2eR__String("BT044", faktur_rec, null) } }
         };

         if(isMojeRacunNotFina == false)
         {
            the_eRacun.AccountingCustomerParty.Party.PartyLegalEntity = new PartyLegalEntityType[]
            {
              new PartyLegalEntityType
              {
                RegistrationName = new RegistrationNameType { Value = Fak2eR__String("BT044", faktur_rec, null) },
              }
            };
         }
         else//24.06.2019. AccountingCustomerParty / Party / PartyLegalEntity / CompanyID - upisati OIB primatelja: 04192765979
         {
            the_eRacun.AccountingCustomerParty.Party.PartyLegalEntity = new PartyLegalEntityType[]
            {
              new PartyLegalEntityType
              {
                RegistrationName = new RegistrationNameType { Value = Fak2eR__String("BT044"    , faktur_rec, null) },
                CompanyID        = new CompanyIDType        { Value = KiD2eR__String("OIB kupca", kupdob_rec)}
              }
            };
         }
         //BT-48 Porezni identifikator Kupca porezni broj s prefiksom zemlje
         the_eRacun.AccountingCustomerParty.Party.PartyTaxScheme = new PartyTaxSchemeType[]
         {
            new PartyTaxSchemeType
            {
               CompanyID = new CompanyIDType {                   Value = KiD2eR__String("BT048"     , kupdob_rec) },
               TaxScheme = new TaxSchemeType { ID = new IDType { Value = KiD2eR__String("BT048 kind", kupdob_rec) } }
            }
         };


         //BT-49 Elektronička adresa Kupca Identifikator sheme Identifikator 1..1
         if(isMojeRacunNotFina == true)
         {
            the_eRacun.AccountingCustomerParty.Party.EndpointID = new EndpointIDType { Value = KiD2eR__String("OIB kupca", kupdob_rec) };
            the_eRacun.AccountingCustomerParty.Party.EndpointID.schemeID = "9934";
         }
         else if(kupdob_rec.Email.NotEmpty())
         {
            the_eRacun.AccountingCustomerParty.Party.EndpointID = new EndpointIDType { Value = KiD2eR__String("BT049", kupdob_rec) };
            the_eRacun.AccountingCustomerParty.Party.EndpointID.schemeID = "EMAIL";
         }
         ;

         //BG-8 POŠTANSKA ADRESA KUPCA	                                                      1..1
         //BT-50 Redak adrese Kupca 1 Glavno adresno polje. Obično naziv ulice i broj 	Tekst 0..1
         //BT-52 Grad Kupca Uobičajeno ime mjesta	                                    Tekst 0..1
         //BT-53 Poštanski broj Kupca	                                                Tekst 0..1
         //BT-55 Šifra države Kupca                                                      Kod 1..1
         the_eRacun.AccountingCustomerParty.Party.PostalAddress = new AddressType
         {
            StreetName = new StreetNameType { Value = Fak2eR__String("BT050", faktur_rec, null) },
            CityName = new CityNameType { Value = Fak2eR__String("BT052", faktur_rec, null) },
            PostalZone = new PostalZoneType { Value = Fak2eR__String("BT053", faktur_rec, null) },
            Country = new CountryType { IdentificationCode = new IdentificationCodeType { Value = KiD2eR__String("BT055", kupdob_rec) } },

            AddressLine = new AddressLineType[]
            {
               new AddressLineType
               {
                  Line = new LineType { Value = Fak2eR__String("BT050x", faktur_rec, null) },
               },
            }
         };

         if(/*isMojeRacunNotFina == true*/kupdob_rec.Email.NotEmpty())
         {
            the_eRacun.AccountingCustomerParty.AccountingContact = new ContactType();
            the_eRacun.AccountingCustomerParty.AccountingContact.ElectronicMail = new ElectronicMailType { Value = KiD2eR__String("BT049", kupdob_rec) };

         }

         #endregion BG -8 Kupac

         #region Primatelj placanja

         //BT-59 Ime primatelja plaćanja.	Tekst 1..1

         if(faktur_rec.PrimPlatCD.NotZero())
         {
            the_eRacun.PayeeParty = new PartyType();
            the_eRacun.PayeeParty.PartyName = new PartyNameType[]
            {
               new PartyNameType { Name = new NameType1 { Value = Fak2eR__String("BT059 PpName", faktur_rec, null) } }
            };
            the_eRacun.PayeeParty.PartyLegalEntity = new PartyLegalEntityType[]
            {
               new PartyLegalEntityType { CompanyID = new CompanyIDType { Value = KiD2eR__String("BT059 PpVatN", primPlat_rec)} }
            };

         }
         #endregion Primatelj placanja

         #region Delivery dostava

         //BT-80 Šifra države dostave	Kod 1..1
         bool isPoslJed = (faktur_rec.PosJedCD != faktur_rec.KupdobCD) && faktur_rec.DostAddr.IsEmpty();

         List<DeliveryType> deliveryList = new List<DeliveryType>();

         if(isMojeRacunNotFina == true) //Delivery / ActualDeliveryDate - potrebno kreirati polje i upisati datum dostave robe ili obavljanja usluge<cbc:ActualDeliveryDate>2019-04-24</cbc:ActualDeliveryDate>
         {
            if(faktur_rec.DostAddr.NotEmpty())
            {
               deliveryList.Add(new DeliveryType
               {
                  ActualDeliveryDate = new ActualDeliveryDateType { Value = Fak2eR____Date("BT072", faktur_rec, null) },
                  DeliveryLocation = new LocationType1
                  {
                     Address = new AddressType
                     {
                        StreetName = new StreetNameType { Value = Fak2eR__String("BT080 DostAdr", faktur_rec, null) },
                        Country = new CountryType
                        {
                           IdentificationCode = new IdentificationCodeType { Value = KiD2eR__String("BT080 ccod", kupdob_rec) }
                        }
                     }
                  }
               });
            }
            else if(faktur_rec.PosJedCD != faktur_rec.KupdobCD)
            {
               deliveryList.Add(new DeliveryType
               {
                  ActualDeliveryDate = new ActualDeliveryDateType { Value = Fak2eR____Date("BT072", faktur_rec, null) },
                  DeliveryLocation = new LocationType1
                  {
                     Address = new AddressType
                     {
                        StreetName = new StreetNameType { Value = Fak2eR__String("BT080 PJ_Ulic", faktur_rec, null) },
                        CityName = new CityNameType { Value = Fak2eR__String("BT080 PJ_Grad", faktur_rec, null) },
                        PostalZone = new PostalZoneType { Value = Fak2eR__String("BT080 PJ_PstB", faktur_rec, null) },
                        Country = new CountryType
                        {
                           IdentificationCode = new IdentificationCodeType { Value = KiD2eR__String("BT080 ccod", kupdob_rec) }
                        }
                     }
                  }
               });
            }
            else
            {
               deliveryList.Add(new DeliveryType
               {
                  ActualDeliveryDate = new ActualDeliveryDateType { Value = Fak2eR____Date("BT072", faktur_rec, null) },
                  DeliveryLocation = new LocationType1
                  {
                     Address = new AddressType
                     {
                        Country = new CountryType
                        {
                           IdentificationCode = new IdentificationCodeType { Value = KiD2eR__String("BT080 ccod", kupdob_rec) }
                        }
                     }
                  }
               });
            }

         }// mojeracun end
         else// fina
         {
            if(faktur_rec.DostAddr.NotEmpty())
            {
               deliveryList.Add(new DeliveryType
               {
                  DeliveryLocation = new LocationType1
                  {
                     Address = new AddressType
                     {
                        StreetName = new StreetNameType { Value = Fak2eR__String("BT080 DostAdr", faktur_rec, null) },
                        Country = new CountryType
                        {
                           IdentificationCode = new IdentificationCodeType { Value = KiD2eR__String("BT080 ccod", kupdob_rec) }
                        }
                     }
                  }
               });
            }
            else if(faktur_rec.PosJedCD != faktur_rec.KupdobCD)
            {
               deliveryList.Add(new DeliveryType
               {
                  DeliveryLocation = new LocationType1
                  {
                     Address = new AddressType
                     {
                        StreetName = new StreetNameType { Value = Fak2eR__String("BT080 PJ_Ulic", faktur_rec, null) },
                        CityName = new CityNameType { Value = Fak2eR__String("BT080 PJ_Grad", faktur_rec, null) },
                        PostalZone = new PostalZoneType { Value = Fak2eR__String("BT080 PJ_PstB", faktur_rec, null) },
                        Country = new CountryType
                        {
                           IdentificationCode = new IdentificationCodeType { Value = KiD2eR__String("BT080 ccod", kupdob_rec) }
                        }
                     }
                  }
               });
            }
            else
            {
               deliveryList.Add(new DeliveryType
               {
                  DeliveryLocation = new LocationType1
                  {
                     Address = new AddressType
                     {
                        Country = new CountryType
                        {
                           IdentificationCode = new IdentificationCodeType { Value = KiD2eR__String("BT080 ccod", kupdob_rec) }
                        }
                     }
                  }
               });
            }
         }// fina end

         the_eRacun.Delivery = deliveryList.ToArray();


         #endregion Delivery dostava

         #region PaymentMeansType placanje i PrepaidPayment-Avans

         //BT-81 Šifra načina plaćanja	    Kod           1..1
         //BT-82 Tekst za  načina plaćanja 
         //BT-83 Informacije o doznaci - broj racuna, poziv na br i sl    
         //BT-84 Identifikator računa plaćanja IBAN	Identifikator 1..1
         the_eRacun.PaymentMeans = new PaymentMeansType[]
         {
            new PaymentMeansType
            {
               PaymentMeansCode      = new PaymentMeansCodeType  {                           Value = Fak2eR__String("BT081", faktur_rec, null)   },
             //InstructionNote       = new InstructionNoteType[] { new InstructionNoteType { Value = Fak2eR__String("BT082", faktur_rec, null) } },
               PaymentID             = new PaymentIDType[]       { new PaymentIDType       { Value = Fak2eR__String("BT083", faktur_rec, null) } },
               PayeeFinancialAccount = new FinancialAccountType  {     ID = new IDType     { Value = Fak2eR__String("BT084", faktur_rec, null) } },
            }
         };

         // 27.06.2019. javlja kao upozorenje da to nije bitno a kad se makne onda je dokument ispravan bez upozorenja
         //if(isAVANS)
         //{
         //   the_eRacun.PrepaidPayment = new PaymentType[]
         //      {
         //         new PaymentType
         //         {
         //            PaidAmount = new PaidAmountType { Value = Fak2eR_Decimal("BT113", faktur_rec, null), currencyID = faktur_rec.CurrencyID}
         //         }
         //      };
         //}

         #endregion placanje

         #region RBT & ZTR by PdvSt

         string ppmvReasonTag = "#HR:PPMV#Posebni porez na motorna vozila";
         decimal ppmvItemsBase = Fak2eR_Decimal("PPMVosn", faktur_rec, null);

         //List<AllowanceChargeType> rbtAndZtr_AllowanceChargeList        = new List<AllowanceChargeType>();
         List<AllowanceChargeType> rbtAndZtrAndPpmv_AllowanceChargeList = new List<AllowanceChargeType>(); //2026 + ppmv

         //BT- 92 Iznos popusta na razini dokumenta	                    Iznos 1..1
         //BT- 95 Šifra kategorije PDV popusta na razini dokumenta 	     Kod   1..1
         //BT- 99 Iznos troška na razini dokumenta Iznos troška bez PDVa. Iznos 1..1
         //BT-102 Šifra kategorije PDV-a troška na razini dokumenta 	     Kod   1..1

         if(faktur_rec.TrnSum_Rbt25.NotZero())
         {
            rbtAndZtrAndPpmv_AllowanceChargeList.Add(new AllowanceChargeType
            {
               ChargeIndicator = new ChargeIndicatorType { Value = false },
               Amount = new AmountType2 { Value = Fak2eR_Decimal("Rbt25 izn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxCategory = new TaxCategoryType[]
               {
                  new TaxCategoryType {
                     ID        = new IDType        { Value = Fak2eR__String("Rbt25 kat", faktur_rec, null) },
                     Percent   = new PercentType1  { Value = Fak2eR_Decimal("Pdv25 stp", faktur_rec, null) },
                     TaxScheme = new TaxSchemeType { ID = new IDType { Value = "VAT" } } }
               },
               AllowanceChargeReason = new AllowanceChargeReasonType[]
               {
                  new AllowanceChargeReasonType { Value = Fak2eR__String("RbtReason", faktur_rec, null) }
               }
            });
         }

         if(faktur_rec.TrnSum_Rbt10.NotZero())
         {
            rbtAndZtrAndPpmv_AllowanceChargeList.Add(new AllowanceChargeType
            {
               ChargeIndicator = new ChargeIndicatorType { Value = false },
               Amount = new AmountType2 { Value = Fak2eR_Decimal("Rbt10 izn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxCategory = new TaxCategoryType[]
               {
                  new TaxCategoryType {
                     ID        = new IDType        { Value = Fak2eR__String("Rbt10 kat", faktur_rec, null) },
                     Percent   = new PercentType1  { Value = Fak2eR_Decimal("Pdv13 stp", faktur_rec, null) },
                     TaxScheme = new TaxSchemeType { ID = new IDType { Value = "VAT" } } }
               },
               AllowanceChargeReason = new AllowanceChargeReasonType[]
               {
                  new AllowanceChargeReasonType { Value = Fak2eR__String("RbtReason", faktur_rec, null) }
               }
            });
         }

         if(faktur_rec.TrnSum_Rbt05.NotZero())
         {
            rbtAndZtrAndPpmv_AllowanceChargeList.Add(new AllowanceChargeType
            {
               ChargeIndicator = new ChargeIndicatorType { Value = false },
               Amount = new AmountType2 { Value = Fak2eR_Decimal("Rbt05 izn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxCategory = new TaxCategoryType[]
               {
                  new TaxCategoryType {
                     ID        = new IDType        { Value = Fak2eR__String("Rbt05 kat", faktur_rec, null) },
                     Percent   = new PercentType1  { Value = Fak2eR_Decimal("Pdv05 stp", faktur_rec, null) },
                     TaxScheme = new TaxSchemeType { ID = new IDType { Value = "VAT" } } }
               },
               AllowanceChargeReason = new AllowanceChargeReasonType[]
               {
                  new AllowanceChargeReasonType { Value = Fak2eR__String("RbtReason", faktur_rec, null) }
               }
            });
         }

         if(faktur_rec.TrnSum_Rbt00.NotZero())
         {
            rbtAndZtrAndPpmv_AllowanceChargeList.Add(new AllowanceChargeType
            {
               ChargeIndicator = new ChargeIndicatorType { Value = false },
               Amount = new AmountType2 { Value = Fak2eR_Decimal("Rbt00 izn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxCategory = new TaxCategoryType[]
               {
                  new TaxCategoryType { ID = new IDType { Value = Fak2eR__String("Rbt00 kat", faktur_rec, null) }, TaxScheme = new TaxSchemeType { ID = new IDType { Value = "FRE" } } } // 'FRE' ! 
               },
               AllowanceChargeReason = new AllowanceChargeReasonType[]
               {
                  new AllowanceChargeReasonType { Value = Fak2eR__String("RbtReason", faktur_rec, null) }
               }
            });
         }

#if ZTR_eRacun
         if(faktur_rec.TrnSum_Ztr25.NotZero())
         {
            rbtAndZtr_AllowanceChargeList.Add(new AllowanceChargeType
            {
               ChargeIndicator = new ChargeIndicatorType { Value = true                                                                         },
               Amount          = new AmountType2         { Value = Fak2eR_Decimal("Ztr25 izn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxCategory     = new TaxCategoryType[]   
               {
                  new TaxCategoryType { ID = new IDType { Value = Fak2eR__String("Ztr25 kat", faktur_rec, null) }, TaxScheme = new TaxSchemeType { ID = new IDType { Value = "VAT" } } }
               }
            });
         }

         if(faktur_rec.TrnSum_Ztr10.NotZero())
         {
            rbtAndZtr_AllowanceChargeList.Add(new AllowanceChargeType
            {
               ChargeIndicator = new ChargeIndicatorType { Value = true                                                                         },
               Amount          = new AmountType2         { Value = Fak2eR_Decimal("Ztr10 izn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxCategory     = new TaxCategoryType[]   
               {
                  new TaxCategoryType { ID = new IDType { Value = Fak2eR__String("Ztr10 kat", faktur_rec, null) }, TaxScheme = new TaxSchemeType { ID = new IDType { Value = "VAT" } } }
               }
            });
         }

         if(faktur_rec.TrnSum_Ztr05.NotZero())
         {
            rbtAndZtr_AllowanceChargeList.Add(new AllowanceChargeType
            {
               ChargeIndicator = new ChargeIndicatorType { Value = true                                                                         },
               Amount          = new AmountType2         { Value = Fak2eR_Decimal("Ztr05 izn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxCategory     = new TaxCategoryType[]   
               {
                  new TaxCategoryType { ID = new IDType { Value = Fak2eR__String("Ztr05 kat", faktur_rec, null) }, TaxScheme = new TaxSchemeType { ID = new IDType { Value = "VAT" } } }
               }
            });
         }

         if(faktur_rec.TrnSum_Ztr00.NotZero())
         {
            rbtAndZtr_AllowanceChargeList.Add(new AllowanceChargeType
            {
               ChargeIndicator = new ChargeIndicatorType { Value = true                                                                         },
               Amount          = new AmountType2         { Value = Fak2eR_Decimal("Ztr00 izn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxCategory     = new TaxCategoryType[]   
               {
                  new TaxCategoryType { ID = new IDType { Value = Fak2eR__String("Ztr00 kat", faktur_rec, null) }, TaxScheme = new TaxSchemeType { ID = new IDType { Value = "FRE" } } } // 'FRE' ! 
               }
            });
         }

#endif
         if(faktur_rec.R_ukPpmvIzn.NotZero())
         {

            rbtAndZtrAndPpmv_AllowanceChargeList.Add(new AllowanceChargeType
            {

               ChargeIndicator = new ChargeIndicatorType { Value = true },
               AllowanceChargeReason = new AllowanceChargeReasonType[]
               {
                                       new AllowanceChargeReasonType { Value = ppmvReasonTag }
               },
               Amount = new AmountType2 { Value = Fak2eR_Decimal("PPMVizn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxCategory = new TaxCategoryType[]
               {
                  new TaxCategoryType
                  {
                     ID                 = new IDType        { Value = "E"                                          },
                     Name               = new NameType1     { Value = "HR:PPMV"                                    },
                     Percent            = new PercentType1  { Value = 0.00M/*Fak2eR_Decimal("PPMV stp", faktur_rec, null)*/ },
                     TaxExemptionReason = new TaxExemptionReasonType[]
                                        { new TaxExemptionReasonType { Value = ppmvReasonTag/*"Posebni porez na motorna vozila"*/ } },
                     TaxScheme          = new TaxSchemeType { ID = new IDType { Value = "VAT" } } }
               }
            });
         }


         the_eRacun.AllowanceCharge = rbtAndZtrAndPpmv_AllowanceChargeList.ToArray();

         #endregion RBT & ZTR by PdvSt

         #region Total Sums

         //BT-106 Zbroj svih neto iznosa stavki računa	                        Iznos 1..1
         //BT-109 Ukupni iznos računa bez PDV-a 	                              Iznos 1..1
         //BT-112 Ukupni iznos računa s PDV-om		                              Iznos 1..1
         //BT-115 Iznos koji dospijeva na plaćanje Preostali iznos za plaćanje	Iznos 1..1

         decimal avansMoney = ((isFinalRn && prvFaktur_rec != null) ? prvFaktur_rec.S_ukKCRP * -1.00M : 0M).Ron2();
         decimal finalMoney = ((isFinalRn && prvFaktur_rec != null) ? faktur_rec.S_ukKCRP + prvFaktur_rec.S_ukKCRP : 0M).Ron2();

         the_eRacun.LegalMonetaryTotal = new MonetaryTotalType
         {
            LineExtensionAmount = new LineExtensionAmountType { Value = Fak2eR_Decimal("BT106", faktur_rec, null), currencyID = faktur_rec.CurrencyID },// zbroj svih neto iznosa stavki racuna    = sumi neto iznosa stavki BT-131
            AllowanceTotalAmount = new AllowanceTotalAmountType { Value = Fak2eR_Decimal("BT107", faktur_rec, null), currencyID = faktur_rec.CurrencyID },// zbroj svih rabata na razini dokumenta   = sumi rabata BT-92             
            TaxExclusiveAmount = new TaxExclusiveAmountType { Value = Fak2eR_Decimal("BT109", faktur_rec, null), currencyID = faktur_rec.CurrencyID },// zbroj svih iznosa bez PDV-a             = sumaBT-131 - BT-107 + BT-108  
            TaxInclusiveAmount = new TaxInclusiveAmountType { Value = Fak2eR_Decimal("BT112", faktur_rec, null), currencyID = faktur_rec.CurrencyID },// ukupni iznos racuna s PDV-om            =BT-109 + BT-110                


            PayableAmount = new PayableAmountType
            {
               Value = isAvans || isAvansSTORNO ? 0.00M :
                                                                          isFinalRn ? finalMoney :
                                                                                                    Fak2eR_Decimal("BT115", faktur_rec, null),
               currencyID = faktur_rec.CurrencyID
            } // iznos koji dospijeva na plaćanje = BT-112 - BT-113 - BT-114(iznos zaokruživanja)
         };

         if(isAvans || isAvansSTORNO || isFinalRn)
         {
            the_eRacun.LegalMonetaryTotal.PrepaidAmount = new PrepaidAmountType
            {
               Value = isAvans || isAvansSTORNO ? faktur_rec.S_ukKCRP.Ron2() :
                       isFinalRn ? avansMoney :
                       0.00M,
               currencyID = faktur_rec.CurrencyID
            };// iznos plaćen unaprijed - avans
         }

         if(faktur_rec.R_ukPpmvIzn.NotZero())
         {
            the_eRacun.LegalMonetaryTotal.ChargeTotalAmount = new ChargeTotalAmountType { Value = Fak2eR_Decimal("PPMVizn", faktur_rec, null), currencyID = faktur_rec.CurrencyID };
         }

         #endregion Total Sums

         #region PDV, PPMV, PNP

         #region Vv Get Tax In Use List

         List<TaxSubtotalType> pdvSubtotalList = new List<TaxSubtotalType>();

         //if(faktur_rec.S_ukPdv25m.NotZero()) 27.06.2019. ovdje ne ulazimo kada je pdv 0 ybog cijelo zatvorenog avansa
         if(faktur_rec.S_ukPdv25m.NotZero() || faktur_rec.R_ukPdv_25m_SUM_AVANS.NotZero())
         {
            pdvSubtotalList.Add(new TaxSubtotalType
            {
               TaxableAmount = new TaxableAmountType { Value = Fak2eR_Decimal("Pdv25 osn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxAmount = new TaxAmountType { Value = Fak2eR_Decimal("Pdv25 izn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxCategory = new TaxCategoryType
               {
                  ID = new IDType { Value = Fak2eR__String("Pdv25 kat", faktur_rec, null) },
                  Percent = new PercentType1 { Value = Fak2eR_Decimal("Pdv25 stp", faktur_rec, null) },
                  TaxScheme = new TaxSchemeType { ID = new IDType { Value = "VAT" } },
               }
            });
         }

         if(faktur_rec.S_ukPdv10m.NotZero() || faktur_rec.R_ukPdv_10m_SUM_AVANS.NotZero())
         {
            pdvSubtotalList.Add(new TaxSubtotalType
            {
               TaxableAmount = new TaxableAmountType { Value = Fak2eR_Decimal("Pdv13 osn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxAmount = new TaxAmountType { Value = Fak2eR_Decimal("Pdv13 izn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxCategory = new TaxCategoryType
               {
                  ID = new IDType { Value = Fak2eR__String("Pdv13 kat", faktur_rec, null) },
                  Percent = new PercentType1 { Value = Fak2eR_Decimal("Pdv13 stp", faktur_rec, null) },
                  TaxScheme = new TaxSchemeType { ID = new IDType { Value = "VAT" } },
               }
            });
         }

         if(faktur_rec.S_ukPdv05m.NotZero() || faktur_rec.R_ukPdv_05m_SUM_AVANS.NotZero())
         {
            pdvSubtotalList.Add(new TaxSubtotalType
            {
               TaxableAmount = new TaxableAmountType { Value = Fak2eR_Decimal("Pdv05 osn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxAmount = new TaxAmountType { Value = Fak2eR_Decimal("Pdv05 izn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxCategory = new TaxCategoryType
               {
                  ID = new IDType { Value = Fak2eR__String("Pdv05 kat", faktur_rec, null) },
                  Percent = new PercentType1 { Value = Fak2eR_Decimal("Pdv05 stp", faktur_rec, null) },
                  TaxScheme = new TaxSchemeType { ID = new IDType { Value = "VAT" } },
               }
            });
         }

         if(faktur_rec.S_ukOsn0.NotZero()) // nulta stopa PDV-a koje u RH trenutno nema
         {
            pdvSubtotalList.Add(new TaxSubtotalType
            {
               TaxableAmount = new TaxableAmountType { Value = Fak2eR_Decimal("Pdv00 osn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxAmount = new TaxAmountType { Value = Fak2eR_Decimal("Pdv00 izn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxCategory = new TaxCategoryType
               {
                  ID = new IDType { Value = Fak2eR__String("Pdv00 kat", faktur_rec, null) },
                  Percent = new PercentType1 { Value = Fak2eR_Decimal("Pdv00 stp", faktur_rec, null) },
                  TaxScheme = new TaxSchemeType { ID = new IDType { Value = "FRE" } },

                  TaxExemptionReason = new TaxExemptionReasonType[]
                  {
                     new TaxExemptionReasonType { Value = ZXC.IsTETRAGRAM_ANY ? GetTekstNoPdvFromThePFD(faktur_rec) : GetTekstNoPdvFromThePFD(thePFD) } // razlog 
                  }
               }
            });
         }

         /* Oslobodjeno od PDVa */
         decimal oslobPdv =
                            faktur_rec.S_ukOsn07 +
                            faktur_rec.S_ukOsn08 +
                            faktur_rec.S_ukOsn09 +
                            faktur_rec.S_ukOsn10 +
                            faktur_rec.S_ukOsn11 +
                            faktur_rec.S_ukOsn12 +
                            faktur_rec.S_ukOsn13 +
                            faktur_rec.S_ukOsn14 +
                            faktur_rec.S_ukOsn15 +
                            faktur_rec.S_ukOsn16;

         if(oslobPdv.NotZero()) // OP oslobodeno PDV-a 
         {
            pdvSubtotalList.Add(new TaxSubtotalType
            {
               TaxableAmount = new TaxableAmountType { Value = Fak2eR_Decimal("PdvOP osn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxAmount = new TaxAmountType { Value = Fak2eR_Decimal("PdvOP izn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },

               // 15.11.2019. Zar ptica - ovo ne prolazi
               //TaxCategory   = new TaxCategoryType
               //{
               //   ID        = new IDType        { Value = Fak2eR__String("PdvOP kat", faktur_rec, null) },
               //   Percent   = new PercentType1  { Value = Fak2eR_Decimal("PdvOP stp", faktur_rec, null) },
               //   TaxScheme = new TaxSchemeType { ID = new IDType { Value = "FRE" } },
               //
               //   TaxExemptionReason = new TaxExemptionReasonType[]
               //   {
               //      new TaxExemptionReasonType { Value = GetTekstNoPdvFromThePFD(thePFD) } // razlog 
               //   }
               //}

               TaxCategory = new TaxCategoryType
               {
                  ID = new IDType
                  {
                     schemeID = "UN/ECE 5305",
                     schemeAgencyID = "6",
                     schemeURI = "http://www.unece.org/trade/untdid/d07a/tred/tred5305.htm",
                     Value = Fak2eR__String("PdvOP kat", faktur_rec, null),
                  },

                  //Name    = new NameType1    { Value = "OSLOBOĐENO_POREZA" }, // 17.12.2019. kažu da nijepotrebno
                  Percent = new PercentType1 { Value = Fak2eR_Decimal("PdvOP stp", faktur_rec, null) },

                  TaxExemptionReason = new TaxExemptionReasonType[]
                  {
                     new TaxExemptionReasonType { Value = ZXC.IsTETRAGRAM_ANY ? GetTekstNoPdvFromThePFD(faktur_rec) : GetTekstNoPdvFromThePFD(thePFD) } // razlog 
                  },

                  TaxScheme = new TaxSchemeType
                  {
                     //17.12.2019.
                     //Name        = new NameType1       { Value = "FRE"       },
                     //TaxTypeCode = new TaxTypeCodeType { Value = "ZeroRated" } 
                     ID = new IDType { Value = "VAT" },
                  }
               }

            });
         }

         // ostaje nam ??????                                                 
         // Prijenos porezne obveze                                           
         // Oporeziva prodaja Kanarski otoci ...                              
         // Neoporeziva prodaja koja ne podlijeze PDV "Out of scope of VAT"   


         #endregion Vv Get Tax In Use List

         #region taxSubtotal za ppmv 2026

         if(faktur_rec.R_ukPpmvIzn.NotZero())
         {
            pdvSubtotalList.Add(new TaxSubtotalType
            {
               TaxableAmount = new TaxableAmountType { Value = Fak2eR_Decimal("PPMVosn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxAmount = new TaxAmountType { Value = 0.00M/*Fak2eR_Decimal("PPMVizn", faktur_rec, null)*/, currencyID = faktur_rec.CurrencyID },
               TaxCategory = new TaxCategoryType
               {
                  ID = new IDType { Value = "E" },
                  Name = new NameType1 { Value = "HR:PPMV" },
                  Percent = new PercentType1 { Value = 0.00M/*Fak2eR_Decimal("PPMV stp", faktur_rec, null)*/ },
                  TaxExemptionReason = new TaxExemptionReasonType[]
                                     { new TaxExemptionReasonType { Value = "#HR:PPMV#Posebni porez na motorna vozila" } },
                  TaxScheme = new TaxSchemeType { ID = new IDType { Value = "VAT" } }
               }
            });
         }
         #endregion taxSubtotal za ppmv 2026


         //BG-023  Da li ukupan pdv po svim osnovama ??????
         //BT-116 Iznos osnovice kategorije PDV-a 	                                       Iznos 1..1
         //BT-117 Iznos kategorije PDV-a Ukupni iznos PDV-a za određenu kategoriju PDV-a. Iznos 1..1
         //BT-118 Šifra kategorije PDV-a UNTDID5305 [6]: 
         //BT-119 Stopa PDV-a PDV-a                      
         //BT-120 tekstualni raylog osobodenja poreza    
         the_eRacun.TaxTotal = new TaxTotalType[]
         {
            new TaxTotalType // the_eRacun.TaxTotal[0] is for PDV 
            {
               TaxAmount   = new TaxAmountType {  Value = Fak2eR_Decimal("BG023", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
               TaxSubtotal = pdvSubtotalList.ToArray()
            }
         };

         #endregion PDV, PPMV, PNP

         #endregion ZAGLAVLJE računa

         #region STAVKE računa

         InvoiceLineType invoiceLine;
         Rtrans rtrans_rec;
         Artikl artikl_rec;

         //the_eRacun.InvoiceLine = new InvoiceLineType[faktur_rec.Transes                  .Count ];        
         //the_eRacun.InvoiceLine = new InvoiceLineType[faktur_rec.TrnNonDel_WO_AVANS_STORNO.Length]; // !!! 
         // Od 18.09.2025: 

         if(ZXC.IsF2_2026_rules) the_eRacun.InvoiceLine = new InvoiceLineType[faktur_rec.TrnNonDel_WO_AVANS_STORNO.Where(rtr => rtr.T_artiklCD.NotEmpty()).Count()]; // NEW way 
         else the_eRacun.InvoiceLine = new InvoiceLineType[faktur_rec.TrnNonDel_WO_AVANS_STORNO.Length];                                          // OLD way 

         bool isStavkaForAdditionalOpis;

         List<string> additionalOpises = new List<string>();

         int invLineIdx = 0;
         // 18.09.2025: 
         //for(int i = 0; i < the_eRacun.InvoiceLine.Length               ; ++i)
         for(int i = 0/*, invLineIdx = 0*/; i < faktur_rec.TrnNonDel_WO_AVANS_STORNO.Count(); ++i)
         {
            //rtrans_rec  = faktur_rec.Transes                  [i];        
            rtrans_rec = faktur_rec.TrnNonDel_WO_AVANS_STORNO[i]; // !!! 

            artikl_rec = artiklSifrar.SingleOrDefault(a => a.ArtiklCD.ToUpper() == rtrans_rec.T_artiklCD.ToUpper());

            isStavkaForAdditionalOpis = ZXC.IsF2_2026_rules && rtrans_rec.T_artiklCD.IsEmpty(); // novo za 2026 

            if(isStavkaForAdditionalOpis)
            {
               additionalOpises.Add(rtrans_rec.T_artiklName);

               continue;
            }
            else
            {
               if(additionalOpises.NotEmpty())
               {
                  string prevValue = "";

                  if(the_eRacun.InvoiceLine[invLineIdx - 1].Item.Description != null && the_eRacun.InvoiceLine[invLineIdx - 1].Item.Description.Length.NotZero())
                  {
                     prevValue = the_eRacun.InvoiceLine[invLineIdx - 1].Item.Description[0].Value;
                  }

                  the_eRacun.InvoiceLine[invLineIdx - 1].Item.Description = new DescriptionType[] { new DescriptionType { Value = prevValue + string.Join("\n", additionalOpises) } };

                  additionalOpises.Clear();
               }
            }

            invoiceLine = the_eRacun.InvoiceLine[invLineIdx++] = new InvoiceLineType();

            invoiceLine.ID = new IDType { Value = (invLineIdx /*+ 1*/).ToString() };//BT-126 Identifikator stavke računa 	  Identifikator 1..1
            invoiceLine.InvoicedQuantity = new InvoicedQuantityType
            {
               Value = Fak2eR_Decimal("BT129", faktur_rec, rtrans_rec),  //BT-129 Obračunata količina Količina artikala Količina 1..1
               unitCode = Fak2eR__String("BT130", faktur_rec, rtrans_rec)
            };//BT-130 Šifra jedinica mjere	   @unitCode     Kod     1..1

            //BT-131 Neto iznos stavke računa "neto" bez PDV-a	Iznos 1..1
            invoiceLine.LineExtensionAmount = new LineExtensionAmountType { Value = Fak2eR_Decimal("BT131", faktur_rec, rtrans_rec), currencyID = faktur_rec.CurrencyID };

            #region RBT & ZTR stavke

            //BT-136 Iznos popusta stavke računa Iznos popusta bez PDV-a. 	Iznos 1..1 
            if(rtrans_rec.T_rbt1St.NotZero())
            {
               invoiceLine.AllowanceCharge = new AllowanceChargeType[]
               { new AllowanceChargeType
                  {
                     ChargeIndicator = new ChargeIndicatorType{ Value = false},
                     Amount          = new AmountType2        { Value = Fak2eR_Decimal("BT136", faktur_rec, rtrans_rec), currencyID  = faktur_rec.CurrencyID },
                     AllowanceChargeReason = new AllowanceChargeReasonType[]
                     {
                        new AllowanceChargeReasonType { Value = Fak2eR__String("RbtReason", faktur_rec, null) }
                     }
                  }
               };
            }
            //BT-141 Iznos troška stavke računa  bez PDVa.                 Iznos 1..1 
            //ZTR Value = true
            //new AllowanceChargeType
            //{
            //   ChargeIndicator = new ChargeIndicatorType{ Value = true},
            //   Amount          = new AmountType2        { Value = Fak2eR_Decimal("BT141", faktur_rec, rtrans_rec), currencyID  = faktur_rec.CurrencyID }
            //}

            #endregion RBT & ZTR stavke

            //BT-146 Neto cijena artikla Cijena artikla bez PDVa	Jedinična cijena1..1
            //BT-150 Šifra jedinice mjere jedinične količine artikla
            invoiceLine.Price = new PriceType();
            invoiceLine.Price.PriceAmount = new PriceAmountType { Value = Fak2eR_Decimal("BT146", faktur_rec, rtrans_rec), currencyID = faktur_rec.CurrencyID };
            invoiceLine.Price.BaseQuantity = new BaseQuantityType { Value = 1M, unitCode = Fak2eR__String("BT130", faktur_rec, rtrans_rec) };

            //BT-151 sifra kategorije pdv-a obracunske stavke UNTDID 5305 [6]:
            //BT-152 pdv stopa stavke
            invoiceLine.Item = new ItemType();

            //invoiceLine.Item.ClassifiedTaxCategory = new TaxCategoryType[]
            //{
            //   new TaxCategoryType
            //   {
            //      ID        = new IDType       {                   Value = Fak2eR__String("BT151 kod", faktur_rec, rtrans_rec) },
            //      Percent   = new PercentType1 {                   Value = Fak2eR_Decimal("BT152 stp", faktur_rec, rtrans_rec) },
            //      TaxScheme = new TaxSchemeType{ ID = new IDType { Value = Fak2eR__String("BT152 txs", faktur_rec, rtrans_rec) }                                          }
            //   }
            //};



            //21.11.2019. oslobodeno poreza
            bool isOslobodPDV = (rtrans_rec.T_pdvColTip == ZXC.PdvKolTipEnum.KOL07 ||
                                 rtrans_rec.T_pdvColTip == ZXC.PdvKolTipEnum.KOL08 ||
                                 rtrans_rec.T_pdvColTip == ZXC.PdvKolTipEnum.KOL08 ||
                                 rtrans_rec.T_pdvColTip == ZXC.PdvKolTipEnum.KOL10 ||
                                 rtrans_rec.T_pdvColTip == ZXC.PdvKolTipEnum.KOL11 ||
                                 rtrans_rec.T_pdvColTip == ZXC.PdvKolTipEnum.KOL12 ||
                                 rtrans_rec.T_pdvColTip == ZXC.PdvKolTipEnum.KOL13 ||
                                 rtrans_rec.T_pdvColTip == ZXC.PdvKolTipEnum.KOL14 ||
                                 rtrans_rec.T_pdvColTip == ZXC.PdvKolTipEnum.KOL15 ||
                                 rtrans_rec.T_pdvColTip == ZXC.PdvKolTipEnum.KOL16);

            if(isOslobodPDV)
            {
               /*invoiceLine.TaxTotal = new TaxTotalType[]
               {
                   new TaxTotalType
                   {
                       TaxAmount   = new TaxAmountType {Value = 0.00M, currencyID = faktur_rec.CurrencyID },
                       TaxSubtotal = new TaxSubtotalType []
                       {
                          new TaxSubtotalType
                          {
                             TaxableAmount = new TaxableAmountType{Value = Fak2eR_Decimal("BT131", faktur_rec, rtrans_rec), currencyID = faktur_rec.CurrencyID },
                             TaxAmount     = new TaxAmountType    {Value = 0.00M                                          , currencyID = faktur_rec.CurrencyID },
                           //CalculationSequenceNumeric = new CalculationSequenceNumericType { Value = 1}
                             TaxCategory = new TaxCategoryType
                             {

                                 ID = new IDType
                                 {
                                    schemeID = "UN/ECE 5305",
                                    schemeAgencyID = "6",
                                    schemeURI = "http://www.unece.org/trade/untdid/d07a/tred/tred5305.htm",
                                    Value = Fak2eR__String("PdvOP kat", faktur_rec, null),
                                 },

                                 Name    = new NameType1    { Value = "OSLOBOĐENO_POREZA" },
                                 Percent = new PercentType1 { Value = Fak2eR_Decimal("BT152 stp", faktur_rec, rtrans_rec) },

                                 TaxExemptionReason = new TaxExemptionReasonType[]
                                 {
                                    new TaxExemptionReasonType { Value = GetTekstNoPdvFromThePFD(thePFD) } // razlog 
                                 },

                                 TaxScheme = new TaxSchemeType
                                 {
                                    Name        = new NameType1       { Value = "FRE"       },
                                    TaxTypeCode = new TaxTypeCodeType { Value = "ZeroRated" }
                                 }
                             }
                          }
                       }
                   } 
               };*/
               invoiceLine.Item.ClassifiedTaxCategory = new TaxCategoryType[]
               {
                  new TaxCategoryType
                  {
                     ID        = new IDType       {                   Value = "E"   },
                     Name      = new NameType1    {                   Value = "HR:E"}, // oznaka za oslobodjeno pdv a
                     Percent   = new PercentType1 {                   Value = 0     },
                     TaxExemptionReason = new TaxExemptionReasonType[] { new TaxExemptionReasonType { Value = ZXC.IsTETRAGRAM_ANY ? GetTekstNoPdvFromThePFD(faktur_rec) : GetTekstNoPdvFromThePFD(thePFD) } },
                     TaxScheme = new TaxSchemeType{ ID = new IDType { Value = "VAT" }
                     }
                  }
               };

            }
            else // po starom za sve ostale koji imaju porez
            {
               invoiceLine.Item.ClassifiedTaxCategory = new TaxCategoryType[]
               {
                  new TaxCategoryType
                  {
                     ID        = new IDType       {                   Value = Fak2eR__String("BT151 kod", faktur_rec, rtrans_rec) },
                     Percent   = new PercentType1 {                   Value = Fak2eR_Decimal("BT152 stp", faktur_rec, rtrans_rec) },
                     TaxScheme = new TaxSchemeType{ ID = new IDType { Value = Fak2eR__String("BT152 txs", faktur_rec, rtrans_rec) }                                          }
                  }
               };
            }

            #region Artikl

            //BT-153 Naziv artikla. Tekst 1..1
            //BT-154 Opis artikla
            //BT-155 Identifikator artikla  ID = sifra Artikla
            invoiceLine.Item.Name = new NameType1 { Value = Fak2eR__String("BT153", faktur_rec, rtrans_rec) };

            //if(ZXC.IsF2_2026_rules) ya avanse da to ne radi jer dode prazan element 23.01.2026.
            if(ZXC.IsF2_2026_rules && artikl_rec.TS != ZXC.AVA_TS)
            {
               invoiceLine.Item.CommodityClassification = new CommodityClassificationType[]
               {
                  new CommodityClassificationType
                  {
                     ItemClassificationCode = new ItemClassificationCodeType
                     {
                        listID = "CG",
                        Value  = artikl_rec.ER_KPD
                     }
                  }
               };
            }

            if(rtrans_rec.T_artiklCD.NotEmpty())
            {
               invoiceLine.Item.SellersItemIdentification = new ItemIdentificationType
               {
                  ID = new IDType { Value = Fak2eR__String("BT155 artCd", faktur_rec, rtrans_rec) },
               };
            }

            if(artikl_rec != null)
            {
               if(artikl_rec.LongOpis.NotEmpty())
               {
                  invoiceLine.Item.Description = new DescriptionType[] { new DescriptionType { Value = Art2eR__String("BT152", artikl_rec) } };
               }
            }

            #endregion Artikl

            #region Opcionalni Atributi Artikla

            ////BT-157 Standardni identifikator artikla Identifikator sheme Identifikator	 1..1
            //invoiceLine.Item.StandardItemIdentification   = new ItemIdentificationType();
            //invoiceLine.Item.StandardItemIdentification.ID = new IDType { schemeID = Fak2eR__String("BT157 shemaID", faktur_rec, rtrans_rec) };    //@schemeID

            ////BT-158 Identifikator klasifikacije artikla sheme Identifikator  sheme 1..1
            //invoiceLine.Item.CommodityClassification = new CommodityClassificationType[]
            //   {
            //      new CommodityClassificationType
            //      {
            //         ItemClassificationCode = new ItemClassificationCodeType{listID = Fak2eR__String("BT158 listID" , faktur_rec, rtrans_rec) } //@listID
            //      }
            //   };

            //BT-160 Naziv atributa artikla Kao što je "Boja".        Tekst 1..1
            //BT-161 Vrijednost atributa artikla Kao što je "Crvena". Tekst 1..1
            //invoiceLine.Item.AdditionalItemProperty = new ItemPropertyType[]
            //{
            //   new ItemPropertyType
            // {
            //    Name = new NameType1 { Value = Fak2eR__String("BT160", faktur_rec, rtrans_rec) },
            //    Value = new ValueType{ Value = Fak2eR__String("BT161", faktur_rec, rtrans_rec) }
            // }
            //};

            List<ItemPropertyType> artiklAtributi_ItemPropertyList = new List<ItemPropertyType>();

            // Za sada: (ostale camo kasnije eventualno po potrebi) 
            // artikl_rec.Zapremina 
            // artikl_rec.MasaNetto 
            // artikl_rec.MasaBruto 
            // artikl_rec.Duljina   
            // artikl_rec.Sirina    
            // artikl_rec.Visina    
            // artikl_rec.Promjer   
            // artikl_rec.Povrsina  

            #region artiklAtributi_ItemPropertyList

            if(artikl_rec != null)
            {
               if(artikl_rec.Zapremina.NotZero())
               {
                  artiklAtributi_ItemPropertyList.Add(new ItemPropertyType
                  {
                     Name = new NameType1 { Value = "Zapremina [" + artikl_rec.ZapreminaJM + "]" },
                     Value = new ValueType { Value = artikl_rec.Zapremina.ToString0Vv() }
                  });
               }

               if(artikl_rec.MasaNetto.NotZero())
               {
                  artiklAtributi_ItemPropertyList.Add(new ItemPropertyType
                  {
                     Name = new NameType1 { Value = "MasaNetto [" + artikl_rec.MasaNettoJM + "]" },
                     Value = new ValueType { Value = artikl_rec.MasaNetto.ToString0Vv() }
                  });
               }
               if(artikl_rec.MasaBruto.NotZero())
               {
                  artiklAtributi_ItemPropertyList.Add(new ItemPropertyType
                  {
                     Name = new NameType1 { Value = "MasaBruto [" + artikl_rec.MasaBrutoJM + "]" },
                     Value = new ValueType { Value = artikl_rec.MasaBruto.ToString0Vv() }
                  });
               }
               if(artikl_rec.Duljina.NotZero())
               {
                  artiklAtributi_ItemPropertyList.Add(new ItemPropertyType
                  {
                     Name = new NameType1 { Value = "Duljina [" + artikl_rec.DuljinaJM + "]" },
                     Value = new ValueType { Value = artikl_rec.Duljina.ToString0Vv() }
                  });
               }
               if(artikl_rec.Sirina.NotZero())
               {
                  artiklAtributi_ItemPropertyList.Add(new ItemPropertyType
                  {
                     Name = new NameType1 { Value = "Širina [" + artikl_rec.SirinaJM + "]" },
                     Value = new ValueType { Value = artikl_rec.Sirina.ToString0Vv() }
                  });
               }
               if(artikl_rec.Visina.NotZero())
               {
                  artiklAtributi_ItemPropertyList.Add(new ItemPropertyType
                  {
                     Name = new NameType1 { Value = "Visina [" + artikl_rec.VisinaJM + "]" },
                     Value = new ValueType { Value = artikl_rec.Visina.ToString0Vv() }
                  });
               }
               if(artikl_rec.Promjer.NotZero())
               {
                  artiklAtributi_ItemPropertyList.Add(new ItemPropertyType
                  {
                     Name = new NameType1 { Value = "Promjer [" + artikl_rec.PromjerJM + "]" },
                     Value = new ValueType { Value = artikl_rec.Promjer.ToString0Vv() }
                  });
               }
               if(artikl_rec.Povrsina.NotZero())
               {
                  artiklAtributi_ItemPropertyList.Add(new ItemPropertyType
                  {
                     Name = new NameType1 { Value = "Površina [" + artikl_rec.PovrsinaJM + "]" },
                     Value = new ValueType { Value = artikl_rec.Povrsina.ToString0Vv() }
                  });
               }

            } // if(artikl_rec != null)

            invoiceLine.Item.AdditionalItemProperty = artiklAtributi_ItemPropertyList.ToArray();

            #endregion artiklAtributi_ItemPropertyList

            #endregion Opcionalni Atributi Artikla


         } // foreach(Rtrans rtrans_rec in faktur_rec.Transes)

         // 'Za zadnjega' 
         if(additionalOpises.NotEmpty())
         {
            string prevValue = "";

            if(the_eRacun.InvoiceLine[invLineIdx - 1].Item.Description != null && the_eRacun.InvoiceLine[invLineIdx - 1].Item.Description.Length.NotZero())
            {
               prevValue = the_eRacun.InvoiceLine[invLineIdx - 1].Item.Description[0].Value;
            }

            the_eRacun.InvoiceLine[invLineIdx - 1].Item.Description = new DescriptionType[] { new DescriptionType { Value = prevValue + string.Join("\n", additionalOpises) } };

            //additionalOpises.Clear();
         }


         #endregion STAVKE računa

         #region HRExtensions 2026

         if(ZXC.IsF2_2026_rules)
         {
            // Build HRTaxSubtotal entries explicitly to match the sample XML (exempt case shown)
            var hrTaxSubtotals = new List<HRTaxSubtotalType>();

            // Example: add PdvOP / exempt subtotal when there is any exempt base
            decimal oslobPdvxxx =
               faktur_rec.S_ukOsn07 +
               faktur_rec.S_ukOsn08 +
               faktur_rec.S_ukOsn09 +
               faktur_rec.S_ukOsn10 +
               faktur_rec.S_ukOsn11 +
               faktur_rec.S_ukOsn12 +
               faktur_rec.S_ukOsn13 +
               faktur_rec.S_ukOsn14 +
               faktur_rec.S_ukOsn15 +
               faktur_rec.S_ukOsn16;

            if(oslobPdvxxx.NotZero())
            {
               hrTaxSubtotals.Add(new HRTaxSubtotalType
               {
                  TaxableAmount = new TaxableAmountType
                  {
                     Value = Fak2eR_Decimal("PdvOP osn", faktur_rec, null),
                     currencyID = faktur_rec.CurrencyID
                  },
                  TaxAmount = new TaxAmountType
                  {
                     Value = Fak2eR_Decimal("PdvOP izn", faktur_rec, null),
                     currencyID = faktur_rec.CurrencyID
                  },
                  HRTaxCategory = new HRTaxCategoryType
                  {
                     ID = new IDType { Value = "E" },
                     Name = new NameType1 { Value = "HR:E" },
                     Percent = new PercentType1 { Value = Fak2eR_Decimal("PdvOP stp", faktur_rec, null) },
                     TaxExemptionReason = new TaxExemptionReasonType[] { new TaxExemptionReasonType { Value = ZXC.IsTETRAGRAM_ANY ? GetTekstNoPdvFromThePFD(faktur_rec) : GetTekstNoPdvFromThePFD(thePFD) } },
                     HRTaxScheme = new HRTaxSchemeType
                     {
                        ID = new IDType { Value = "VAT" }
                     }
                  }
               });
            }

            // (Optionally add other bands similarly: Pdv25, Pdv13, Pdv05, Pdv00)
            if(faktur_rec.S_ukPdv25m.NotZero() || faktur_rec.R_ukPdv_25m_SUM_AVANS.NotZero())
            {
               hrTaxSubtotals.Add(new HRTaxSubtotalType
               {
                  TaxableAmount = new TaxableAmountType { Value = Fak2eR_Decimal("Pdv25 osn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
                  TaxAmount = new TaxAmountType { Value = Fak2eR_Decimal("Pdv25 izn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
                  HRTaxCategory = new HRTaxCategoryType
                  {
                     ID = new IDType { Value = Fak2eR__String("Pdv25 kat", faktur_rec, null) },
                     Name = new NameType1 { Value = "HR:PDV25" },
                     Percent = new PercentType1 { Value = Fak2eR_Decimal("Pdv25 stp", faktur_rec, null) },
                     HRTaxScheme = new HRTaxSchemeType { ID = new IDType { Value = "VAT" } }
                  }
               });
            }

            if(faktur_rec.S_ukPdv10m.NotZero() || faktur_rec.R_ukPdv_10m_SUM_AVANS.NotZero())
            {
               hrTaxSubtotals.Add(new HRTaxSubtotalType
               {
                  TaxableAmount = new TaxableAmountType { Value = Fak2eR_Decimal("Pdv13 osn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
                  TaxAmount = new TaxAmountType { Value = Fak2eR_Decimal("Pdv13 izn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
                  HRTaxCategory = new HRTaxCategoryType
                  {
                     ID = new IDType { Value = Fak2eR__String("Pdv13 kat", faktur_rec, null) },
                     Name = new NameType1 { Value = "HR:PDV13" },
                     Percent = new PercentType1 { Value = Fak2eR_Decimal("Pdv13 stp", faktur_rec, null) },
                     HRTaxScheme = new HRTaxSchemeType { ID = new IDType { Value = "VAT" } }
                  }
               });
            }

            if(faktur_rec.S_ukPdv05m.NotZero() || faktur_rec.R_ukPdv_05m_SUM_AVANS.NotZero())
            {
               hrTaxSubtotals.Add(new HRTaxSubtotalType
               {
                  TaxableAmount = new TaxableAmountType { Value = Fak2eR_Decimal("Pdv05 osn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
                  TaxAmount = new TaxAmountType { Value = Fak2eR_Decimal("Pdv05 izn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
                  HRTaxCategory = new HRTaxCategoryType
                  {
                     ID = new IDType { Value = Fak2eR__String("Pdv05 kat", faktur_rec, null) },
                     Name = new NameType1 { Value = "HR:PDV5" },
                     Percent = new PercentType1 { Value = Fak2eR_Decimal("Pdv05 stp", faktur_rec, null) },
                     HRTaxScheme = new HRTaxSchemeType { ID = new IDType { Value = "VAT" } }
                  }
               });
            }

            if(faktur_rec.R_ukPpmvIzn.NotZero())
            {
               hrTaxSubtotals.Add(new HRTaxSubtotalType
               {
                  TaxableAmount = new TaxableAmountType { Value = Fak2eR_Decimal("PPMVosn", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
                  TaxAmount = new TaxAmountType { Value = 0.00M /*Fak2eR_Decimal("PPMVizn", faktur_rec, null)*/, currencyID = faktur_rec.CurrencyID },
                  HRTaxCategory = new HRTaxCategoryType
                  {
                     ID = new IDType { Value = "O" },
                     Name = new NameType1 { Value = "HR:PPMV" },
                     Percent = new PercentType1 { Value = 0M /*Fak2eR_Decimal("PPMV stp", faktur_rec, null)*/ },
                     TaxExemptionReason = new TaxExemptionReasonType[]
                                         {
                                            new TaxExemptionReasonType { Value = ppmvReasonTag }
                                         },
                     HRTaxScheme = new HRTaxSchemeType { ID = new IDType { Value = "CAR" } }
                  }
               });
            }

            var doc = new XmlDocument();
            XmlElement emptySignatureInfo = doc.CreateElement("sac", "SignatureInformation", "urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2");

            the_eRacun.UBLExtensions = new UBLExtensionType[]
            {
               new UBLExtensionType
               {
                   ExtensionContent = new ExtensionContentType()
                   {
                       Item = new HRFISK20DataType
                       {
                          HRTaxTotal = new HRTaxTotalType
                          {
                             TaxAmount     = new TaxAmountType { Value = Fak2eR_Decimal("BG023", faktur_rec, null), currencyID = faktur_rec.CurrencyID },
                             HRTaxSubtotal = hrTaxSubtotals.ToArray()
                          },
                          HRLegalMonetaryTotal = new HRMonetaryTotalType
                          {
                             TaxExclusiveAmount    = new TaxExclusiveAmountType    { Value = Fak2eR_Decimal("BT109old", faktur_rec, null), currencyID = faktur_rec.CurrencyID },// ovdje ide cisti kcr bez ikakvog poreza !!!!2 mjesta razliciti iznosi!!!!!!!!!!
                             OutOfScopeOfVATAmount = new OutOfScopeOfVATAmountType { /*Value = 0.00M*/Value = Fak2eR_Decimal("PPMVizn", faktur_rec, null), currencyID = faktur_rec.CurrencyID } //ovo je ppmv i sl...
                          }
                       }
                   }
                },
                new UBLExtensionType
                {
                   ExtensionContent = new ExtensionContentType()
                   {
                      Item = new UBLDocumentSignaturesType()
                      {
                       //SignatureInformation = new XmlElement[] { /* empty placeholder - serialized as <sig:SignatureInformation>... */ },
                         SignatureInformation = new XmlElement[] { emptySignatureInfo                                                    },
                      }
                   }
                }
            };
         }

         #endregion HRExtensions 2026

         #endregion Set eRacun Values From Faktur

         return the_eRacun;
      }

      private static string Get_refFiskalBr_fromYRN_VezniDok2(Faktur YRNfaktur_rec)
      {
         // npr za 5pIFA-103390 
         // daj 3390-1-1        

         string vezniDok = YRNfaktur_rec.VezniDok2;

         string tt = "";
         uint ttnum = 0;

         if(vezniDok.Length != 12)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "vezniDok.Length != 12\n\r\n\rne mogu Get_refFiskalBr_fromYRN_VezniDok2()");
            return "";
         }

         string[] splitters = vezniDok.SubstringSafe(2).Split("-".ToCharArray());

         if(splitters.Length > 0) tt = splitters[0];
         if(splitters.Length > 1) ttnum = ZXC.ValOrZero_UInt(splitters[1]);

         if(tt.IsEmpty() || ttnum.IsZero())
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "Neuspjeh parsiranja tt i ttnum!");
            return "";
         }

         Faktur pgFaktur_rec = new Faktur()
         {
            SkladCD = YRNfaktur_rec.SkladCD,
            DokDate = YRNfaktur_rec.DokDate,
            TT = tt,
            TtNum = ttnum
         };

         return pgFaktur_rec.TtNumFiskal;
      }

      #region Fak2eR & VvUBL Utils 

      private static string Fak2eR__String(string BT_ID, Faktur faktur_rec, Rtrans rtrans_rec)
      {
         string theString;

         switch(BT_ID)
         {
            //ZAGLAVLJE:
            case "BT001": theString = faktur_rec.TtNumFiskal; break; //BT- 1 Broj računa                                           
                                                                     //case "BT003"         : theString = VvUBL_InvoiceTypeCode                                                     ; break; //BT- 3 Šifra vrste                                           
            case "BT003": theString = faktur_rec.StatusCD; break; //BT- 3 Šifra vrste  tek 23.01.2026.                                         
            case "BT005": theString = faktur_rec.CurrencyID; break; //BT- 5 Šifra valute !!!Valuta u kojoj su navedeni svi iznosi računa izuzev ukupnog iznosa PDV-a u računovodstvenoj valuti.
            case "BT006": theString = faktur_rec.CurrencyID; break; //BT- 6 TaxCurrencyCode Šifra valute PDV!!!
            case "BT013": theString = faktur_rec.OpciAvalue; break; //BT-13 OrderReference - narudzbenica 
            case "BT022 napomena": theString = faktur_rec.Napomena; break; //BT-22 napomena                                   
            case "BT022 operater": theString = ZXC.TheVvForm.GetFisk_RecID_Oper(faktur_rec.AddUID) + "#Oznaka operatera"; break; //BT-22 operater                                   

            // 16.12.2022: MojEracun nam kaze da se TISAK buni kad ucitava dragicin eRacun jer je time hh:mm a oni ocekuju hh:mm:ss, pa ih evo uslisujemo ?! 
            //case "BT022 vrijemeR": theString = faktur_rec.DokDate.ToString(ZXC.VvTimeOnlyFormat ) + "#Vrijeme izdavanja"  ; break; //BT-22 vrijemeR                                   
            case "BT022 vrijemeR": theString = faktur_rec.DokDate.ToString(ZXC.VvTimeOnlyFormat2) + "#Vrijeme izdavanja"; break; //BT-22 vrijemeR                                   

            case "BT022 opis": theString = faktur_rec.Opis; break;
            case "BT023": theString = "P" + (faktur_rec.PdvKolTip_u.ToString()); break; //BT-23 Vrsta poslovnog procesa ProfileID
            case "BT024": theString = ZXC.IsF2_2026_rules ? VvUBL_CustomizationID_2026 : VvUBL_CustomizationID; break; //BT-24 Identifikator specifikacije                          
            case "BT044": theString = faktur_rec.KupdobName; break; //BT-44 Ime Kupca                                            
            case "BT050": theString = faktur_rec.KdUlica; break; //BT-50 Adresa Kupca Obično naziv ulice i broj               
            case "BT050x": theString = faktur_rec.KdUlica + ", " + faktur_rec.KdZip + " " + faktur_rec.KdMjesto; break; //BT-50 Adresa Kupca Obično naziv ulice i broj               
            case "BT052": theString = faktur_rec.KdMjesto; break; //BT-52 Grad Kupca Uobičajeno ime mjesta	                    
            case "BT053": theString = faktur_rec.KdZip; break; //BT-53 Poštanski broj Kupca	                                
            case "BT059 PpName": theString = faktur_rec.PrimPlatName; break; //BT-59 Ime primatelja plaćanja                              
            case "BT080 DostAdr": theString = faktur_rec.DostAddr; break; //BT-80 Dostavna adresa DostAdres                             
            case "BT080 PJ_Ulic": theString = faktur_rec.PosJedUlica; break; //BT-80 Dostavna adresa PoslJed                             
            case "BT080 PJ_Grad": theString = faktur_rec.PosJedMjesto; break; //BT-80 Dostavna adresa PoslJed                             
            case "BT080 PJ_PstB": theString = faktur_rec.PosJedZip; break; //BT-80 Dostavna adresa PoslJed                             
            case "BT081": theString = "30" /* virman ?! todo */                                                 ; break; //BT-81 Šifra načina plaćanja	                              
                                                                                                                         //case "BT083"         : theString = GetPnbFromFaktur(faktur_rec)                                              ; break; //BT-83 Informacije o doznaci - broj racuna, poziv na br i sl    
            case "BT083": theString = faktur_rec.VvPnb; break; //BT-83 Informacije o doznaci - broj racuna, poziv na br i sl    
                                                               //case "BT084"         : theString = faktur_rec.ZiroRn                                                         ; break; //BT-84 Identifikator računa plaćanja IBAN	      
            case "BT084": theString = faktur_rec.ZiroRn.IsEmpty() ? ZXC.CURR_prjkt_rec.Ziro1 : faktur_rec.ZiroRn; break; //BT-84 Identifikator računa plaćanja IBAN 05.04.2024. kada na faktur ne do]e IBAM onda uzmi iy projekta	      

            case "BT025": theString = faktur_rec.VezniDok; break; //BT-25 Referenca na prethodni račun                         
                                                                  //case "BT082"         : theString = "za sada ovo necemo"                   ; break; //BT-82 Tekst za  načina plaćanja 

            //STAVKE:              
            case "BT130": theString = VvUBL_UnitCode(rtrans_rec.T_jedMj); break; //BT-130 Šifra jedinica mjere	   @unitCode 
            case "BT153": theString = rtrans_rec.T_artiklName; break; //BT-153 Naziv artikla
            case "BT155 artCd": theString = rtrans_rec.T_artiklCD; break; //BT-155 Sifra artikla

            // ovo je privremeno maknuto pa ako ce trebati stavit cemo
            //case "BT157 shemaID" : theString = rtrans_rec.T_artiklName           ; break; //BT-157 Standardni identifikator artikla @schemeID
            //case "BT158 listID"  : theString = rtrans_rec.T_artiklName           ; break; //BT-158 Identifikator klasifikacije artikla sheme @listID
            //case "BT160"         : theString = rtrans_rec.T_artiklName           ; break; //BT-160 Naziv atributa artikla Kao što je "Boja".      
            //case "BT161"         : theString = rtrans_rec.T_artiklName           ; break; //BT-161 Vrijednost atributa artikla Kao što je "Crvena"

            #region PDV

            case "Pdv25 kat":
            case "Pdv13 kat":
            case "Pdv05 kat": theString = "S"; break;
            case "Pdv00 kat": theString = "Zero rated goods"; break;
            case "PdvOP kat": theString = "E"; break;

            //case "Pdv00 rzg":
            //case "Pdv0P rzg": theString = faktur_rec.Napomena2; break; //????? obicno ovo ide na samom printu ali morat ce se to begdje drugdje smjestiti

            case "BT151 kod": theString = VvUBL_PdvKod(rtrans_rec.T_pdvSt, rtrans_rec.T_pdvColTip); break; //BT-151 Šifra kategorije PDV-a UNTDID5305 [6]
            case "BT152 txs": theString = VvUBL_PdvTaxScheme(rtrans_rec.T_pdvSt); break; //BT-151 ID VAT or FRE

            #endregion PDV

            case "Rbt25 kat":
            case "Rbt10 kat":
            case "Rbt05 kat": theString = "S"; break;
            case "Rbt00 kat": theString = "E"; break;
            case "RbtReason": theString = "RABAT"; break;

            default: theString = "VvTODO " + BT_ID; break;
         }

         return theString;
      }

      private static decimal Fak2eR_Decimal(string BT_ID, Faktur faktur_rec, Rtrans rtrans_rec)
      {
         decimal theDecimal;

         // 2026: 
         //bool needsAvansValues =                               faktur_rec.Is_AfterAvans_PrihodTTa; 
         bool needsAvansValues = ZXC.IsF2_2026_rules ? false : faktur_rec.Is_AfterAvans_PrihodTTa;

         switch(BT_ID)
         {
            //ZAGLAVLJE:

            case "BT106": theDecimal = needsAvansValues ? faktur_rec.R_ukKC_SUM_AVANS : faktur_rec.S_ukKC; break; //BT-106 Zbroj svih neto iznosa stavki računa	                     
            case "BT109old": theDecimal = needsAvansValues ? faktur_rec.R_ukKCR_SUM_AVANS : faktur_rec.S_ukKCR; break; //BT-109 Ukupni iznos računa bez PDV-a i bez PPMV !!!!             

            //case "BT109"   : theDecimal = needsAvansValues ? faktur_rec.R_ukKCR_SUM_AVANS  : faktur_rec.S_ukKCR                          ; break; //BT-109 Ukupni iznos računa bez PDV-a 	                           
            case "BT109": theDecimal = needsAvansValues ? faktur_rec.R_ukKCR_SUM_AVANS : faktur_rec.S_ukKCR + faktur_rec.R_ukPpmvIzn; break; //BT-109 Ukupni iznos računa bez PDV-a 	ali sa PPMV-om !!!!!       

            //case "BT112": theDecimal = needsAvansValues ? faktur_rec.R_ukKCRP_SUM_AVANS : faktur_rec.S_ukKCRP                                                   ; break; //BT-112 Ukupni iznos računa s PDV-om		    
            //case "BT112": theDecimal = needsAvansValues ? faktur_rec.R_ukKCRP_SUM_AVANS : faktur_rec.Skn_ukKCRP                                                 ; break; //BT-112 Ukupni iznos računa s PDV-om   2026 
            case "BT112": theDecimal = needsAvansValues ? faktur_rec.R_ukKCRP_SUM_AVANS : faktur_rec.IsPPMV ? faktur_rec.Skn_ukKCRP : faktur_rec.S_ukKCRP; break; //BT-112 Ukupni iznos računa!!!!!                   

            case "BG023": theDecimal = needsAvansValues ? faktur_rec.R_ukPdv_SUM_AVANS : faktur_rec.S_ukPdv; break; //BG-023 Ukupni iznos PDV-a - nisam bas sigurna ali ima u primjernom xml-u

            case "BT092": theDecimal = faktur_rec.S_ukRbt1; break; //BT- 92 Iznos popusta na razini dokumenta
            case "BT107": theDecimal = faktur_rec.S_ukRbt1; break; //BT-107 Iznos popusta na razini dokumenta


            //case "BT115": theDecimal = faktur_rec.S_ukKCRP  ; break; //!!! recimo !!! BT-115 Iznos koji dospijeva na plaćanje Preostali iznos za plaćanje
            //case "BT115": theDecimal = faktur_rec.Skn_ukKCRP; break; // BT-115 Iznos koji dospijeva na plaćanje Preostali iznos za plaćanje 2026
            case "BT115": theDecimal = faktur_rec.IsPPMV ? faktur_rec.Skn_ukKCRP : faktur_rec.S_ukKCRP; break; // BT-115 Iznos koji dospijeva na plaćanje Preostali iznos za plaćanje 2026

            //STAVKE:
            case "BT129": theDecimal = rtrans_rec.T_kol; break; //BT-129 Obračunata količina Količina artikala
            case "BT131": theDecimal = rtrans_rec.R_KC; break; //BT-131 Neto iznos stavke računa "neto" bez PDV-a	
            case "BT136": theDecimal = rtrans_rec.R_rbt1; break; //BT-136 Iznos popusta stavke računa Iznos popusta bez PDV-a.
            case "BT146": theDecimal = rtrans_rec.R_CIJ_KCR; break; //BT-146 Neto cijena artikla Cijena artikla bez PDVa	Jedinična cijena

            //case "BT141": theDecimal = rtrans_rec.R_KCR   ; break; //BT-141 Iznos troška stavke računa  bez PDVa.               

            #region PDV

            case "Pdv25 osn": theDecimal = needsAvansValues ? faktur_rec.R_ukKCR_25m_SUM_AVANS : faktur_rec.S_ukOsn25m; break;
            case "Pdv25 izn": theDecimal = needsAvansValues ? faktur_rec.R_ukPdv_25m_SUM_AVANS : faktur_rec.S_ukPdv25m; break;
            case "Pdv25 stp": theDecimal = 25M; break;
            case "Pdv13 osn": theDecimal = needsAvansValues ? faktur_rec.R_ukKCR_10m_SUM_AVANS : faktur_rec.S_ukOsn10m; break;
            case "Pdv13 izn": theDecimal = needsAvansValues ? faktur_rec.R_ukPdv_10m_SUM_AVANS : faktur_rec.S_ukPdv10m; break;
            case "Pdv13 stp": theDecimal = 13M; break;
            case "Pdv05 osn": theDecimal = needsAvansValues ? faktur_rec.R_ukKCR_05m_SUM_AVANS : faktur_rec.S_ukOsn05m; break;
            case "Pdv05 izn": theDecimal = needsAvansValues ? faktur_rec.R_ukPdv_05m_SUM_AVANS : faktur_rec.S_ukPdv05m; break;
            case "Pdv05 stp": theDecimal = 5M; break;
            case "Pdv00 osn": theDecimal = faktur_rec.S_ukOsn0; break;
            case "Pdv00 izn": theDecimal = 0M; break;
            case "Pdv00 stp": theDecimal = 0M; break;
            case "PdvOP osn":
               theDecimal = faktur_rec.S_ukOsn07 +
                                           faktur_rec.S_ukOsn08 +
                                           faktur_rec.S_ukOsn09 +
                                           faktur_rec.S_ukOsn10 +
                                           faktur_rec.S_ukOsn11 +
                                           faktur_rec.S_ukOsn12 +
                                           faktur_rec.S_ukOsn13 +
                                           faktur_rec.S_ukOsn14 +
                                           faktur_rec.S_ukOsn15 +
                                           faktur_rec.S_ukOsn16; break;
            case "PdvOP izn": theDecimal = 0M; break;
            case "PdvOP stp": theDecimal = 0M; break;

            case "BT152 stp": theDecimal = rtrans_rec.T_pdvSt.Ron(0); break; //BT-152 stopa pdv-a stavke

            #endregion PDV

            case "Rbt25 izn":
               if((faktur_rec.TrnSum_Rbt10 + faktur_rec.TrnSum_Rbt05 + faktur_rec.TrnSum_Rbt00).IsZero()) theDecimal = faktur_rec.S_ukRbt1;
               else theDecimal = faktur_rec.TrnSum_Rbt25;
               break; //BT-Iznos Rbt25 

            case "Rbt10 izn": theDecimal = faktur_rec.TrnSum_Rbt10; break; //BT-Iznos Rbt10 
            case "Rbt05 izn": theDecimal = faktur_rec.TrnSum_Rbt05; break; //BT-Iznos Rbt05 
            case "Rbt00 izn": theDecimal = faktur_rec.TrnSum_Rbt00; break; //BT-Iznos Rbt00 

            case "BT113": theDecimal = /*-1.00M **/ faktur_rec.R_ukKCRP_AVANS_STORNO; break; //BT-84 Identifikator računa plaćanja IBAN	      

            case "PPMVosn": theDecimal = faktur_rec.S_ukPpmvOsn; break; //PPMV osnovica 2026
            case "PPMVizn": theDecimal = faktur_rec.R_ukPpmvIzn; break; //PPMV iznos    2026


            default: theDecimal = 0.00M; break;
         }

         return theDecimal.Ron2();
      }

      private static DateTime Fak2eR____Date(string BT_ID, Faktur faktur_rec, Rtrans rtrans_rec)
      {
         DateTime theDate;

         switch(BT_ID)
         {
            case "BT002": theDate = faktur_rec.DokDate; break; //BT-2 Datum izdavanja
                                                               //case "BT002 Time": theDate = faktur_rec.DokDate ; break; //BT-2 Vrijeme izdavanja
            case "BT002 Time":
               DateTime dateTime = faktur_rec.DokDate;
               //theDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, DateTimeKind.Unspecified); break;
               //theDate = new DateTime(1, 1, 1, dateTime.Hour, dateTime.Minute, dateTime.Second, DateTimeKind.Unspecified); break;
               // Use TimeOfDay to strip milliseconds and create a clean time
               TimeSpan cleanTime = new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second);
               theDate = DateTime.Today.Add(cleanTime); break;

            case "BT002 Dosp": theDate = faktur_rec.DospDate; break; //BT-2 Dospjece placanja
            case "BT072": theDate = faktur_rec.DokDate; break; // 24.06.2019.ActualDeliveryDate

            // 12.01.2024:
            //case "BTqwe"     : theDate = faktur_rec.PdvDate ; break; // 14.12.2020. prema uputama u mailu koji namposla moj-eRacun 14.12. 
            case "BTqwe":
               theDate = faktur_rec.PdvDate;

               if(theDate.IsEmpty())
               {
                  theDate = faktur_rec.DokDate;
               }

               break; // 14.12.2020. prema uputama u mailu koji namposla moj-eRacun 14.12. 

            default: theDate = DateTime.MinValue; break;
         }

         return theDate;
         //return theDate.ToLocalTime();
         //return new DateTime(theDate.Year, theDate.Month, theDate.Day, theDate.Hour, theDate.Minute, theDate.Second);
      }

      private static string KiD2eR__String(string BT_ID, Kupdob kupdob_rec)
      {
         string theString;

         switch(BT_ID)
         {
            case "BTxyz": theString = kupdob_rec.Napom1; break; //BT- 1 qweqwe 
            case "BT048": theString = kupdob_rec.VATnumber; break; //BT-48 Porezni identifikator Kupca 
            case "BT048 kind": theString = "VAT"; break; //BT-48 Porezni identifikator Kupca 
            case "BT049": theString = kupdob_rec.Email; break; //BT-49 Elektronička adresa Kupca   
            case "BT046": theString = GetKupdobIdentification(kupdob_rec); break; //BT-46 Identifikator Kupca         
            case "BT055": theString = kupdob_rec.VatCntryCode_NonEmpty; break; //BT-55 Šifra države Kupca               
            case "BT080 ccod": theString = kupdob_rec.VatCntryCode_NonEmpty; break; //BT-80 Šifra države dostave             
            case "BT059 PpVatN": theString = kupdob_rec.VATnumber; break; //BT-59 Porezni identifikator PrimPlat 
            case "OIB kupca": theString = kupdob_rec.Oib; break; //OIB kupca 

            default: theString = "VvTODO_KiD " + BT_ID; break;
         }

         return theString;
      }

      private static string Prj2eR__String(string BT_ID)
      {
         string theString;

         switch(BT_ID)
         {
            case "BTxyz": theString = ZXC.CURR_prjkt_rec.Napom1; break; // BT-  1 qweqwe               
            case "BT022 odgOsoba": theString = ZXC.CURR_prjkt_rec.Ime + " " + ZXC.CURR_prjkt_rec.Prezime + "#Odgovorna osoba"; break; //BT-22 odgOsoba                                   
            case "BT027": theString = ZXC.CURR_prjkt_rec.Naziv; break; //BT-27 Naziv Prodavatelja                                   
            case "BT029": theString = GetProjektIdentification(ZXC.CURR_prjkt_rec); break; //BT-29 Identifikator Prodavatelja                           
            case "BT031": theString = ZXC.CURR_prjkt_rec.VATnumber; break; //BT-31 Porezni broj Prodavatelja sa oznakom drzave          
                                                                           // 29.01.2020.       
                                                                           //case "BT031 kind"    : theString = ZXC.CURR_prjkt_rec.NOT_IN_PDV ? "FRE" : "VAT";                                  break; //BT-31 "VAT" ili "FRE" je / nije u PDV-u                    
            case "BT031 kind": theString = "VAT"; break; //BT-31 "VAT" ili "FRE" je / nije u PDV-u                    
            case "BT034": theString = ZXC.CURR_prjkt_rec.Email; break; //BT-34 Elektronička adresa Prodavatelja                     
            case "OIB prjkt": theString = ZXC.CURR_prjkt_rec.Oib; break; //24.06.2019. moj-eracun zeli ovdje oib prodavatelja         
            case "BT035": theString = ZXC.CURR_prjkt_rec.Ulica1; break; //BT-35 Redak adrese Prodavatelja 1 Obično naziv ulice i broj
            case "BT035x":
               theString = ZXC.CURR_prjkt_rec.Ulica1 + ", " +
                                               ZXC.CURR_prjkt_rec.PostaBr + " " +
                                               ZXC.CURR_prjkt_rec.Grad; break; //BT-35 Redak adrese Prodavatelja 1 Obično naziv ulice i broj
            case "BT037": theString = ZXC.CURR_prjkt_rec.Grad; break; //BT-37 Grad Prodavatelja                                    
            case "BT038": theString = ZXC.CURR_prjkt_rec.PostaBr; break; //BT-38 Poštanski broj	                                    
            case "BT040": theString = ZXC.CURR_prjkt_rec.VatCntryCode_NonEmpty; break; //BT-40 Šifra države Prodavatelja                            
            case "BT043": theString = ZXC.CURR_prjkt_rec.Email; break; //BT-43 E-Pošta prodavatelja 

            default: theString = "VvTODO_Prj " + BT_ID; break;
         }

         return theString;
      }

      private static string Art2eR__String(string BT_ID, Artikl artikl_rec)
      {
         string theString;

         switch(BT_ID)
         {
            case "BTxyz": theString = artikl_rec.Napomena; break; //BT- 1 qweqwe 
            case "BT152": theString = artikl_rec.LongOpis; break; //BT-152 opis artikla 

            default: theString = "VvTODO_Art " + BT_ID; break;
         }

         return theString;
      }

      private static decimal Art2eR_Decimal(string BT_ID, Artikl artikl_rec)
      {
         decimal theDecimal;

         switch(BT_ID)
         {
            //case "BTxyz": theDecimal = artikl_rec.Napomena; break; //BT- 1 qweqwe 
            //case "BT152": theDecimal = artikl_rec.LongOpis; break; //BT-152 opis artikla 

            default: theDecimal = 0.00M; break;
         }

         return theDecimal;
      }

      private static string VvUBL_PdvTaxScheme(decimal t_pdvSt)
      {
         if(t_pdvSt.NotZero()) return "VAT";
         else return "FRE";
      }

      private static string VvUBL_PdvKod(decimal t_pdvSt, ZXC.PdvKolTipEnum t_pdvColTip)
      {
         if(t_pdvSt.NotZero()) return "S";
         else if(t_pdvColTip == ZXC.PdvKolTipEnum.NIJE) return "Zero rated goods";
         else return "E";
      }

      // http://www.unece.org/trade/untdid/d98a/uncl/uncl1001.htm   
      // 380 Commercial invoice                                     
      // Document/message claiming payment for goods or services    
      // supplied under conditions agreed between seller and buyer. 
      private static string VvUBL_InvoiceTypeCode { get { return "380"; } }


      private static string VvUBL_CustomizationID { get { return "urn:cen.eu:en16931:2017"; } }

      //14.10.2025.
      private static string VvUBL_CustomizationID_2026 { get { return "urn:cen.eu:en16931:2017#compliant#urn:mfin.gov.hr:cius-2025:1.0#conformant#urn:mfin.gov.hr:ext-2025:1.0"; } }

      private static string VvUBL_UnitCode(string t_jm)
      {
         t_jm = t_jm.ToUpper();

         QuantityCode quantityCode;

         switch(t_jm)
         {
            case "LIT":
            case "L": quantityCode = QuantityCode.Litre; break;
            case "KG": quantityCode = QuantityCode.Kilogram; break;
            case "KM": quantityCode = QuantityCode.Kilometre; break;
            case "G": quantityCode = QuantityCode.Gram; break;
            case "M": quantityCode = QuantityCode.Metre; break;
            case "T": quantityCode = QuantityCode.Tonne; break;
            case "M2": quantityCode = QuantityCode.SquareMetre; break;
            case "M3": quantityCode = QuantityCode.CubicMetre; break;
            case "MIN": quantityCode = QuantityCode.Minute; break;
            case "H":
            case "SAT": quantityCode = QuantityCode.Hour; break;
            case "DAN":
            case "DAY": quantityCode = QuantityCode.Day; break;
            case "MJ":
            case "MON": quantityCode = QuantityCode.Month; break;
            case "GOD":
            case "Y": quantityCode = QuantityCode.Month; break;
            case "":
            case "KOM":

            default: quantityCode = QuantityCode.Piece; break;
         }

         return ZXC.GetEnumDescription(quantityCode);
      }

      private static string VvUBL_AllowanceChargeReason { get { return "RABAT"; } }

      private static string GetKupdobIdentification(Kupdob kupdob_rec)
      {
         // "peppol identifiers" 
         //return "9934:" + kupdob_rec.Oib;
         //25.10.2022. ya sifru poslovne jedinice koja bi se trebala upisati negdje na kupdoba
         //< cbc:ID > 9934:18683136487::HR99:11002 </ cbc:ID >
         if(kupdob_rec.Regob.NotEmpty()) return "9934:" + kupdob_rec.Oib + "::HR99:" + kupdob_rec.Regob;
         else return "9934:" + kupdob_rec.Oib;
      }

      private static string GetProjektIdentification(Prjkt prjkt_rec)
      {
         // "peppol identifiers" 
         return "9934:" + prjkt_rec.Oib;
      }

      //private static string GetPnbFromFaktur(Faktur faktur_rec)
      //{
      //   string pnb;
      //
      //   if(faktur_rec.PnbM.NotEmpty() && faktur_rec.PnbV.NotEmpty()) pnb = "HR"    + faktur_rec.PnbM + " " + faktur_rec.PnbV;
      //   else                                                         pnb = "HR99 " + faktur_rec.TtNumFiskal;
      //
      //   return pnb;
      //}

      private static string GetTekstNoPdvFromThePFD(Faktur _faktur_rec)
      {
         string textOslob;

         if(_faktur_rec.Transes.Any(rtr => rtr.T_artiklCD == "OBRKAMZA") ||
            _faktur_rec.Transes.Any(rtr => rtr.T_artiklCD == "ZLCER") ||
            _faktur_rec.Transes.Any(rtr => rtr.T_artiklCD == "SRCERMAT")) textOslob = "Isporuka oslobođena PDV-a sukladno članku 40. Zakona o PDV-u";

         else /*if(faktExDuc.Fld_PdvGEOkind == ZXC.PdvGEOkindEnum.HR)*/     textOslob = "Isporuka investicijskog zlata oslobođena PDV-a sukladno članku 114. Zakona o PDV-u";

         return textOslob;
      }

      private static string GetTekstNoPdvFromThePFD(PrnFakDsc thePFD)
      {
         return thePFD.Dsc_TekstOslobodenPDV;
      }

      #endregion Fak2eR & VvUBL Utils 

      #endregion Create eRacun object (InvoiceType) From Faktur

      #region Create Faktur object From eRacun (InvoiceType)

      public Faktur Create_Faktur_From_InvoiceType(XSqlConnection conn, /*VvMER_ResponseData responseData*/ uint electronicID, DateTime sentDate, Kupdob kupdob_rec, bool isIFA, ZXC.F2_CreateFakturKind createKind,  uint xtranoRecID = 0)
      {
         #region init

         bool isUFA = !isIFA;

         Faktur faktur_rec = Create_COMMON_Faktur_For_eRacun(isIFA);

         Rtrans rtrans_rec;

         ushort line = 0;

         List<Rtrans> newANA_rtransList        = new List<Rtrans>(this.InvoiceLine.Length);
         List<Rtrans> newSIN_rtransList = null;

         bool OK = true;
         //Faktur fak;
         //Rtrans rtr;

         // koristimo stare obsolete Dsc-ove NIR/NUR, pazi da te ne zbuni sa NIR_UC-om! 
         // isIFA je u vezi FIR XXX-a ('Veleform-a'), a isUFA za FUR                    
         bool wantsOneSintStavka = isIFA ? ZXC.RRD.Dsc_F2_IsNIR : ZXC.RRD.Dsc_F2_IsNUR;

         #endregion init

         #region ZAGLAVLJE računa

         if(createKind == ZXC.F2_CreateFakturKind.From_HDD_XXX)
         {
            faktur_rec.F2_StatusCD = 11;
         }

         // From Kupdob 

         faktur_rec.KupdobName   = faktur_rec.PosJedName   = kupdob_rec.Naziv;
         faktur_rec.KupdobCD     = faktur_rec.PosJedCD     = kupdob_rec.KupdobCD;
         faktur_rec.KupdobTK     = faktur_rec.PosJedTK     = kupdob_rec.Ticker;
         faktur_rec.KdUlica      = faktur_rec.PosJedUlica  = kupdob_rec.Ulica1;
         faktur_rec.KdZip        = faktur_rec.PosJedZip    = kupdob_rec.PostaBr;
         faktur_rec.KdMjesto     = faktur_rec.PosJedMjesto = kupdob_rec.Grad;
         faktur_rec.KdOib        = kupdob_rec.Oib;
         faktur_rec.VatCntryCode = kupdob_rec.VatCntryCode;
       //faktur_rec.KdAdresa     = Faktur.GetAdresa(Fld_KupdobUlica, Fld_KupdobZip, Fld_KupdobMjesto); ;

         faktur_rec.F2_R1kind = kupdob_rec.R1kind;

         if(isUFA)
            faktur_rec.ZiroRn = kupdob_rec.Ziro1;

         // From InvoiceType 

         faktur_rec.DokDate = (this.IssueTime != null && this.IssueTime.Value != DateTime.MinValue)
            ? this.IssueDate.Value.Date.Add(this.IssueTime.Value.TimeOfDay)
            : this.IssueDate.Value;

       //faktur_rec.DospDate  = this.DueDate.Value;
         if(this.DueDate != null && this.DueDate.Value != DateTime.MinValue)
         { 
            faktur_rec.DospDate = this.DueDate.Value;

            faktur_rec.RokPlac  = (int)(faktur_rec.DospDate.Date - faktur_rec.DokDate.Date).TotalDays;
         }

         // 25.02.2026: dodan if 
         if(ZXC.CURR_prjkt_rec.PdvRTip != ZXC.PdvRTipEnum.OBRT_R2 &&
            ZXC.CURR_prjkt_rec.PdvRTip != ZXC.PdvRTipEnum.POD_PO_NAPL)
         {
            faktur_rec.PdvDate = this.TaxPointDate?.Value == null ? this.IssueDate.Value : this.TaxPointDate.Value;
         }

         faktur_rec.VezniDok  = this.ID.Value;
        //faktur_rec.Napomena = this.Note[0].Value  ; // hocemo li samo prvu napomenu?
         faktur_rec.Napomena  = ZXC.F2_Unprocessed;
         for(int i = 0; this.Note != null && i < this.Note.Length; i++)
         {
            faktur_rec.Opis += this.Note[i].Value + Environment.NewLine;

            faktur_rec.Opis = ZXC.LenLimitedStr(faktur_rec.Opis, ZXC.FakturDao.GetSchemaColumnSize(ZXC.FakCI.opis));
         }

         faktur_rec.DevName = this.DocumentCurrencyCode.Value;

         //faktur_rec.PdvKolTip =  this.ProfileID.Value;
         //negdje bi mozda trebalo staviti i tip poslovnog procesa (ProfileID) i /ili kod tipa računa (InvoiceTypeCode) da se zna dali je račun za avans ili ne

         faktur_rec.PnbM = GetPNB_FromInvoiceType(false);
         faktur_rec.PnbV = GetPNB_FromInvoiceType(true);

         faktur_rec.PnbM = ZXC.LenLimitedStr(faktur_rec.PnbM, ZXC.FaktExDao.GetSchemaColumnSize(ZXC.FexCI.pnbM));
         faktur_rec.PnbV = ZXC.LenLimitedStr(faktur_rec.PnbV, ZXC.FaktExDao.GetSchemaColumnSize(ZXC.FexCI.pnbV));

         // From some Response 
         if(ZXC.IsF2_2026_rules == false) faktur_rec.FiskPrgBr = "[" + electronicID.ToString() + "]";
         faktur_rec.F2_ElectronicID = electronicID;
         faktur_rec.F2_SentTS = sentDate;
         if(isUFA)
            faktur_rec.F2_ArhRecID = xtranoRecID;

         #endregion ZAGLAVLJE računa

         #region STAVKE računa

       //int numOfRbt_ByDocument = AllowanceCharge?.Length ?? 0;
         int numOfRbt_ByDocument = AllowanceCharge?.Count(ac => ac?.Amount?.Value.NotZero() == true) ?? 0;

       //bool isRbt_ByDocument   = this.LegalMonetaryTotal?.AllowanceTotalAmount?.Value.NotZero() ?? false;
         bool isRbt_ByDocument = numOfRbt_ByDocument.IsPositive();

         bool isRbt_ByLine;
         bool isThereAnyRbt;
       //bool isRbtCOMPLICATED   ;
         bool isRbtNOTCOMPLICATED;
         int numOfRbt_ByLine = 0;

         decimal allowanceAmount = 0.00M;

         foreach(InvoiceLineType invoiceLine in this.InvoiceLine)
         {
            rtrans_rec = new Rtrans();

            //rtrans_rec.T_artiklCD    = ;
            rtrans_rec.T_artiklName = ZXC.LenLimitedStr(invoiceLine.Item.Name.Value, ZXC.RtransDao.GetSchemaColumnSize(ZXC.RtrCI.t_artiklName));

            //rtrans_rec.T_jedMj       = ; 
            rtrans_rec.T_kol = invoiceLine.InvoicedQuantity.Value;

            #region The RABAT

            //numOfRbt_ByLine = invoiceLine.AllowanceCharge?.Length ?? 0;
            numOfRbt_ByLine = invoiceLine.AllowanceCharge?.Count(ac => ac?.Amount?.Value.NotZero() == true) ?? 0;

            isRbt_ByLine = numOfRbt_ByLine.IsPositive();

            isThereAnyRbt = isRbt_ByLine || isRbt_ByDocument;

            isRbtNOTCOMPLICATED = !isRbt_ByDocument && numOfRbt_ByLine == 1;

            //isRbtCOMPLICATED = (numOfRbt_ByDocument > 1 || numOfRbt_ByLine > 1) || (isRbt_ByLine && isRbt_ByDocument);

            if(isThereAnyRbt)
            {
               if(isRbtNOTCOMPLICATED)
               {
                  if(invoiceLine.AllowanceCharge != null &&
                     invoiceLine.AllowanceCharge.Length > 0 &&
                     invoiceLine.AllowanceCharge[0]?.Amount?.Value != null &&
                     invoiceLine.AllowanceCharge[0].Amount.Value.NotZero())
                  {
                     // VAŽNO!!! 
                     // budući da empirijski vidimo da neki debili od programera ovdje stavljaju      
                     // NEGATIVNU vrijednost, koristiti cemo ABS. Sranje ce nastati kada netko zaista 
                     // bude zadao NEGATIVAN rabat                                                    

                     allowanceAmount = invoiceLine.AllowanceCharge[0].Amount.Value;
                     allowanceAmount = Math.Abs(allowanceAmount);
                  }

                  decimal rbtSt = invoiceLine.AllowanceCharge?[0]?.MultiplierFactorNumeric?.Value ?? 0.00M;
                  decimal baseAmount = invoiceLine.AllowanceCharge?[0]?.BaseAmount?.Value ?? 0.00M;
                  decimal cij_KCR = invoiceLine.Price?.PriceAmount?.Value ?? 0.00M;

                  if(allowanceAmount.NotZero())
                  {

                     if(rbtSt.NotZero())
                     {
                        // VAŽNO!!! 
                        // budući da empirijski vidimo da neki debili od programera ovdje stavljaju 
                        // KOEFICIJENT umjesto stope, pravimo se pametni i mnozimo ga sa 100.       
                        // Sranje ce nastati kada netko zaista bude zadao rabat manji od 1.00%      

                        if(rbtSt < 1.00M) rbtSt *= 100M;

                        rtrans_rec.T_rbt1St = rbtSt.Ron2();
                     }
                     else if(baseAmount.NotZero())
                     {
                        rtrans_rec.T_rbt1St = (ZXC.DivSafe(allowanceAmount, baseAmount) * 100M).Ron2();
                     }
                     else // znamo samo cij_KCR (kako smo i mi na pocetku punili xml) 
                     {
                        baseAmount = (cij_KCR * rtrans_rec.T_kol) + allowanceAmount;

                        rtrans_rec.T_rbt1St = (ZXC.DivSafe(allowanceAmount, baseAmount) * 100M).Ron2();
                     }

                  } // if(allowanceAmount.NotZero())

                  // 07.02.2026: 
                  // empirijski vidimo da neki debili od programera krivo pune 'allowanceAmount' InvoiceLine-a 
                  // pa bi u tom slucaju pomogla varijanta B. No, buduci da smo do sada imali dosta uspjeha sa varijantom A,
                  // ostavljamo varijantu B kao rezervnu opciju za slucaj da naletimo na takav primjer u buducnosti.

                  // Varijanta A: 
                  rtrans_rec.T_cij = ZXC.DivSafe(allowanceAmount * 100M, rtrans_rec.T_kol * rtrans_rec.T_rbt1St).Ron2();

                  // Varijanta B: 
                  //rtrans_rec.T_cij = ZXC.DivSafe(cij_KCR * 100M, 100M - rtrans_rec.T_rbt1St).Ron(4);

               } // if(isRbtNOTCOMPLICATED)

               else // Rbt IS COMPLICATED ... uopce necemo racunati rtrans_rec.T_rbt1St. Zelimo samo doci do rtrans_rec.T_cij, kao da nema rabata 
               {
                  if((isRbt_ByLine))
                  {
                     rtrans_rec.T_cij = invoiceLine.Price.PriceAmount.Value;
                  }
                  else // if((isRbt_ByDocument))
                  {
                     // Ovo dole je skroz neprovjereno. Umoran san i neda mi se proucavati.                         
                     // Ili je ovo dobro, ili nikada necemo ni imati ovakav primjer, ili cemo to tek tada rjesavati 

                     decimal LineExtensionAmount = invoiceLine.LineExtensionAmount.Value;
                     decimal totalRbtAmount = 0.00M;

                     for(int i = 0; i < numOfRbt_ByDocument; i++)
                     {
                        if(AllowanceCharge[i]?.Amount?.Value != null &&
                           AllowanceCharge[i].Amount.Value.NotZero())
                        {
                           totalRbtAmount += AllowanceCharge[i].Amount.Value;
                        }
                     }

                     decimal baseAmount = LineExtensionAmount + totalRbtAmount;
                     rtrans_rec.T_cij = ZXC.DivSafe(baseAmount, rtrans_rec.T_kol).Ron2();

                  } // else if((isRbt_ByDocument))

               } // Rbt IS COMPLICATED 

            } // if(isThereAnyRbt) 

            else // no Rbt at all ... classic 
            {
               // TODO: ugraditi kontrolu da ako je cijena 'idiotska'       
               // npr 0, ili 10 puta kriva kao npr kod Francuzove UFA       
               // primljene od MER-a negdje na pocetku godine ....          
               // ... UTOLIKO cijenu treba iskalkulirati cij = iznos / kol! 

               rtrans_rec.T_cij = invoiceLine.Price.PriceAmount.Value;
            }

            #endregion The RABAT

            rtrans_rec.T_pdvSt = invoiceLine.Item?.ClassifiedTaxCategory?[0]?.Percent?.Value ?? 0M;

            //rtrans_rec.T_wanted      = ;
            //rtrans_rec.T_pdvColTip   = ;
            //rtrans_rec.T_isIrmUsluga = ;
            //rtrans_rec.T_ppmvOsn     = ;
            //rtrans_rec.T_ppmvSt1i2   = ;
            //rtrans_rec.T_pnpSt       = ;

            rtrans_rec.CalcTransResults(null);

            newANA_rtransList.Add(rtrans_rec);

         } // foreach(InvoiceLineType invoiceLine in this.InvoiceLine) 

         #endregion STAVKE računa

         #region wantsOneSintStavka

         if(wantsOneSintStavka)
         {
            newSIN_rtransList = new List<Rtrans>();

            string  taxCategory;
            string  taxExemptionReason;
            decimal taxAmount;
            string artiklName;

            // probaj prvo po TaxTotalima 
            foreach(var taxTotal in this.TaxTotal)
            { 
               foreach(var taxSubtotal in taxTotal.TaxSubtotal)
               {
                  taxCategory        = taxSubtotal.TaxCategory.Percent.Value.ToString();
                  taxExemptionReason = taxSubtotal.TaxCategory?.TaxExemptionReason?.FirstOrDefault()?.Value ?? $"Stav. računa po PDV stopi od {taxCategory}%";
                  taxAmount          = taxSubtotal.TaxAmount.Value;

                  artiklName = $"Stav. računa po PDV stopi od {taxCategory}% (u iznosu od {taxAmount})";

                  Rtrans sintRtrans_rec = new Rtrans()
                  {
                     T_artiklName = taxSubtotal.TaxCategory.Percent.Value.NotZero() ? artiklName : ZXC.LenLimitedStr(taxExemptionReason, ZXC.RtransDao.GetSchemaColumnSize(ZXC.RtrCI.t_artiklName)),
                     T_kol        = 1,
                     T_pdvSt      = taxSubtotal.TaxCategory.Percent.Value,

                     T_cij        = taxSubtotal.TaxableAmount.Value
                  };

                  sintRtrans_rec.CalcTransResults(null);

                  newSIN_rtransList.Add(sintRtrans_rec);
               }

            } // foreach(var taxTotal in this.TaxTotal)

            // a kako nema TaxTotala, uzmi LegalMonetaryTotal stuff 
            if(newSIN_rtransList.Count.IsZero())
            {
               Rtrans sintRtrans_rec = new Rtrans()
               {
                  T_artiklName = $"Sumirane stavke računa",
                  T_kol        = 1,

                  T_cij        = this.LegalMonetaryTotal?.TaxExclusiveAmount?.Value ?? 0M // ili mozda PayableAmount? 
               };

               sintRtrans_rec.CalcTransResults(null);

               newSIN_rtransList.Add(sintRtrans_rec);

            } // if(newSIN_rtransList.Count.IsZero())

         } // if(wantsOneSintStavka)

         #endregion wantsOneSintStavka

         #region TakeTransesSumToDokumentSum

         faktur_rec.Transes = wantsOneSintStavka ? newSIN_rtransList : newANA_rtransList;

         faktur_rec.TakeTransesSumToDokumentSum(true);
         faktur_rec.Transes = null;

         #endregion TakeTransesSumToDokumentSum

         foreach(Rtrans rtrans in wantsOneSintStavka ? newSIN_rtransList : newANA_rtransList)
         {
            OK = FakturDao.AutoSetFaktur(conn, ref line, faktur_rec, rtrans);
         }

         #region Return

         if(OK) return faktur_rec;
         else   return null;

         #endregion Return

      }

      private string GetPNB_FromInvoiceType(bool isVeliki)
      {
         // HR00 12345 
         string dirty = this.PaymentMeans?.FirstOrDefault()?.PaymentID?.FirstOrDefault()?.Value ?? string.Empty;

         // 00 12345 
         // 0012345 
         string clean = dirty.Replace("HR", "").Replace(" ", "");

         if(clean.IsEmpty()) return "";

         if(clean.Length < 2) return "";

         string mali = ZXC.SubstringSafe(clean, 0, 2);
         string veliki = ZXC.SubstringSafe(clean, 2);

         return isVeliki ? veliki : mali;
      }

      #region eR2Fak & Utils 

      internal static Faktur Create_COMMON_Faktur_For_eRacun(bool isIFA)
      {
         Faktur faktur_rec = new Faktur();

         faktur_rec.TT = isIFA ? Faktur.TT_IFA :
                                         Faktur.TT_UFA; // todo za FUR 

         faktur_rec.PdvKnjiga = ZXC.PdvKnjigaEnum.REDOVNA;
         faktur_rec.PdvR12 = (ZXC.CURR_prjkt_rec.PdvRTip == ZXC.PdvRTipEnum.OBRT_R2 || ZXC.CURR_prjkt_rec.PdvRTip == ZXC.PdvRTipEnum.POD_PO_NAPL) ? ZXC.PdvR12Enum.R2 : ZXC.PdvR12Enum.R1;
         faktur_rec.PdvGEOkind = ZXC.PdvGEOkindEnum.HR;

         ZXC.luiListaSkladista.LazyLoad();

         //VvLookUpItem theLui = ZXC.luiListaSkladista.OrderBy(lui => lui.Integer).FirstOrDefault(); // probaj naci lui sa najmanjim (integer nam je kao intera sifra skladista) 
         VvLookUpItem theLui = ZXC.luiListaSkladista.Where(lui => lui.Integer.NotZero()).
                                                     OrderBy(lui => lui.Integer).FirstOrDefault(); // probaj naci lui sa najmanjim (integer nam je kao intera sifra skladista) 

         if(isIFA)
         {
            if(theLui != null)
            {
               faktur_rec.SkladCD = theLui.Cd;
            }
            else
            {
               faktur_rec.SkladCD = "VPSK";
               ZXC.aim_emsg(MessageBoxIcon.Error, "LookupLista SKLADIŠTA je NEDEFINIRANA! Račun će dobiti 'VPSK' kao oznaku skladišta.");
            }
         }

         return faktur_rec;
      }

      [System.Xml.Serialization.XmlIgnore]
      internal string VvSupplierOIB
      {
         get
         {
            try
            {
               // 25.02.2026: Obrćemo logiku i prvo pokušavamo dohvatiti OIB iz EndpointID-a 

               // 2. Fallback: pokušaj dohvatiti OIB iz EndpointID (npr. <cbc:EndpointID schemeID="9934">60042587515</cbc:EndpointID>)
               if(this.AccountingSupplierParty?.Party?.EndpointID?.Value != null)
               {
                  return Get_CleanOIB_From_DirtyOIB(this.AccountingSupplierParty.Party.EndpointID.Value);
               }

               // 1. Pokušaj dohvatiti OIB iz PartyIdentification (npr. <cbc:ID>9934:60042587515</cbc:ID>)
               if(this.AccountingSupplierParty?.Party?.PartyIdentification != null &&
                  this.AccountingSupplierParty.Party.PartyIdentification.Length > 0 &&
                  this.AccountingSupplierParty.Party.PartyIdentification[0]?.ID?.Value != null)
               {
                  return Get_CleanOIB_From_DirtyOIB(this.AccountingSupplierParty.Party.PartyIdentification[0].ID.Value); // !!! SupplierID !!! ... ili ti ga 9934:OIB
               }

               ZXC.aim_emsg(MessageBoxIcon.Error, "Error getting VvSupplierOIB: Supplier OIB not found in eRacun XML.");
               return string.Empty;
            }
            catch(Exception ex)
            {
               ZXC.aim_emsg($"Error getting VvSupplierOIB: {ex.Message}");
               return string.Empty;
            }
         }
      }

      [System.Xml.Serialization.XmlIgnore]
      internal string VvCustomerOIB
      {
         get
         {
            try
            {
               // 25.02.2026: Obrćemo logiku i prvo pokušavamo dohvatiti OIB iz EndpointID-a 

               // 2. Fallback: pokušaj dohvatiti OIB iz EndpointID (npr. <cbc:EndpointID schemeID="9934">85821130368</cbc:EndpointID>)
               if(this.AccountingCustomerParty?.Party?.EndpointID?.Value != null)
               {
                  return Get_CleanOIB_From_DirtyOIB(this.AccountingCustomerParty.Party.EndpointID.Value);
               }

               // 1. Pokušaj dohvatiti OIB iz PartyIdentification (npr. <cbc:ID>9934:85821130368</cbc:ID>)
               if(this.AccountingCustomerParty?.Party?.PartyIdentification != null &&
                   this.AccountingCustomerParty.Party.PartyIdentification.Length > 0 &&
                   this.AccountingCustomerParty.Party.PartyIdentification[0]?.ID?.Value != null)
               {
                  return Get_CleanOIB_From_DirtyOIB(this.AccountingCustomerParty.Party.PartyIdentification[0].ID.Value); // !!! CustomerID !!! ... ili ti ga 9934:OIB
               }

               ZXC.aim_emsg(MessageBoxIcon.Error, "Error getting VvCustomerOIB: Customer OIB not found in eRacun XML.");
               return string.Empty;
            }
            catch(Exception ex)
            {
               ZXC.aim_emsg($"Error getting VvCustomerOIB: {ex.Message}");
               return string.Empty;
            }
         }
      }
      internal static string Get_CleanOIB_From_DirtyOIB(string dirtyOIB) // !!! SupplierID !!! ... ili ti ga 9934:OIB 
      {                                                                                               // <cbc:ID>9934:60042587515</cbc:ID>         
         string cleanOIB = "";

         if(dirtyOIB.Contains(":") == false) return dirtyOIB;

         string[] strArray = dirtyOIB.Replace(" ", "").Split(":".ToArray(), StringSplitOptions.RemoveEmptyEntries);

         if(strArray.Length >= 1) return strArray[1];

         return cleanOIB;
      }
      internal static Kupdob Create_Kupdob_from_InvoiceType(MySqlConnection conn, InvoiceType invoiceType, bool isKupac)
      {
         bool isDobav = !isKupac;

         Kupdob kupdob_rec = new Kupdob();

         PartyType theParty = isKupac ? invoiceType.AccountingCustomerParty?.Party :
                                        invoiceType.AccountingSupplierParty?.Party;

         kupdob_rec.R1kind = ZXC.F2_R1enum.B2B;

         uint newSifra = kupdob_rec.VvDao.GetNextSifra_Uint(conn, kupdob_rec.VirtualRecordName, "kupdobCD", kupdob_rec.UintSifraRootNum, kupdob_rec.UintSifraBaseFactor);

         kupdob_rec.KupdobCD = newSifra;
         kupdob_rec.Ticker = newSifra.ToString();

         kupdob_rec.Oib = isKupac ? invoiceType.VvCustomerOIB : invoiceType.VvSupplierOIB;

         kupdob_rec.Naziv = theParty.PartyLegalEntity?[0]?.RegistrationName?.Value ?? string.Empty;
         kupdob_rec.Ulica1 = theParty.PostalAddress?.StreetName?.Value ?? string.Empty;
         kupdob_rec.Ulica2 = theParty.PostalAddress?.StreetName?.Value ?? string.Empty;
         kupdob_rec.Grad = theParty.PostalAddress?.CityName?.Value ?? string.Empty;
         kupdob_rec.PostaBr = theParty.PostalAddress?.PostalZone?.Value ?? string.Empty;
         kupdob_rec.VatCntryCode = theParty.PostalAddress?.Country?.IdentificationCode?.Value ?? string.Empty;

         kupdob_rec.Naziv = ZXC.LenLimitedStr(kupdob_rec.Naziv, ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KpdbCI.naziv));
         kupdob_rec.Ulica1 = ZXC.LenLimitedStr(kupdob_rec.Ulica1, ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KpdbCI.ulica1));
         kupdob_rec.Ulica2 = ZXC.LenLimitedStr(kupdob_rec.Ulica2, ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KpdbCI.ulica2));
         kupdob_rec.Grad = ZXC.LenLimitedStr(kupdob_rec.Grad, ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KpdbCI.grad));

         if(isDobav)
         {
            //kupdob_rec.Ziro1 = invoiceType.PaymentMeans[0].PayeeFinancialAccount.ID.Value;

            string kupdobZiro = invoiceType.PaymentMeans?[0]?.PayeeFinancialAccount?.ID?.Value;
            if(!string.IsNullOrWhiteSpace(kupdobZiro))
            {
               kupdob_rec.Ziro1 = ZXC.LenLimitedStr(kupdobZiro, ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KpdbCI.ziro1));
            }
         }

         return kupdob_rec;
      }

      #endregion eR2Fak & Utils 

      #endregion Create Faktur object From eRacun (InvoiceType)

   }

   public partial class CreditNoteType
   {
      private static System.Xml.Serialization.XmlSerializer serializer;
      private static System.Xml.Serialization.XmlSerializer Serializer
      {
         get
         {
            if((serializer == null))
            {
               serializer = new System.Xml.Serialization.XmlSerializer(typeof(CreditNoteType));
            }
            return serializer;
         }
      }
      public static CreditNoteType Deserialize(string xml)
      {
         System.IO.StringReader stringReader = null;
         try
         {
            stringReader = new System.IO.StringReader(xml);
            return ((CreditNoteType)(Serializer.Deserialize(System.Xml.XmlReader.Create(stringReader))));
         }
         finally
         {
            if((stringReader != null))
            {
               stringReader.Dispose();
            }
         }
      }

      [System.Xml.Serialization.XmlIgnore]
      internal string VvSupplierOIB
      {
         get
         {
            try
            {
               // 2. Fallback: pokušaj dohvatiti OIB iz EndpointID (npr. <cbc:EndpointID schemeID="9934">60042587515</cbc:EndpointID>)
               if(this.AccountingSupplierParty?.Party?.EndpointID?.Value != null)
               {
                  return InvoiceType.Get_CleanOIB_From_DirtyOIB(this.AccountingSupplierParty.Party.EndpointID.Value);
               }

               // 1. Pokušaj dohvatiti OIB iz PartyIdentification (npr. <cbc:ID>9934:60042587515</cbc:ID>)
               if(this.AccountingSupplierParty?.Party?.PartyIdentification != null &&
                  this.AccountingSupplierParty.Party.PartyIdentification.Length > 0 &&
                  this.AccountingSupplierParty.Party.PartyIdentification[0]?.ID?.Value != null)
               {
                  return InvoiceType.Get_CleanOIB_From_DirtyOIB(this.AccountingSupplierParty.Party.PartyIdentification[0].ID.Value); // !!! SupplierID !!! ... ili ti ga 9934:OIB
               }

               ZXC.aim_emsg(MessageBoxIcon.Error, "Error getting VvSupplierOIB: Supplier OIB not found in eRacun XML.");
               return string.Empty;
            }
            catch(Exception ex)
            {
               ZXC.aim_emsg($"Error getting VvSupplierOIB: {ex.Message}");
               return string.Empty;
            }
         }
      }

      [System.Xml.Serialization.XmlIgnore]
      internal string VvCustomerOIB
      {
         get
         {
            try
            {
               // 2. Fallback: pokušaj dohvatiti OIB iz EndpointID (npr. <cbc:EndpointID schemeID="9934">85821130368</cbc:EndpointID>)
               if(this.AccountingCustomerParty?.Party?.EndpointID?.Value != null)
               {
                  return InvoiceType.Get_CleanOIB_From_DirtyOIB(this.AccountingCustomerParty.Party.EndpointID.Value);
               }

               // 1. Pokušaj dohvatiti OIB iz PartyIdentification (npr. <cbc:ID>9934:85821130368</cbc:ID>)
               if(this.AccountingCustomerParty?.Party?.PartyIdentification != null &&
                   this.AccountingCustomerParty.Party.PartyIdentification.Length > 0 &&
                   this.AccountingCustomerParty.Party.PartyIdentification[0]?.ID?.Value != null)
               {
                  return InvoiceType.Get_CleanOIB_From_DirtyOIB(this.AccountingCustomerParty.Party.PartyIdentification[0].ID.Value); // !!! CustomerID !!! ... ili ti ga 9934:OIB
               }

               ZXC.aim_emsg(MessageBoxIcon.Error, "Error getting VvCustomerOIB: Customer OIB not found in eRacun XML.");
               return string.Empty;
            }
            catch(Exception ex)
            {
               ZXC.aim_emsg($"Error getting VvCustomerOIB: {ex.Message}");
               return string.Empty;
            }
         }
      }
      private string GetPNB_FromCreditNoteType(bool isVeliki)
      {
         // HR00 12345 
         string dirty = this.PaymentMeans?.FirstOrDefault()?.PaymentID?.FirstOrDefault()?.Value ?? string.Empty;

         // 00 12345 
         // 0012345 
         string clean = dirty.Replace("HR", "").Replace(" ", "");

         if(clean.IsEmpty()) return "";

         if(clean.Length < 2) return "";

         string mali = ZXC.SubstringSafe(clean, 0, 2);
         string veliki = ZXC.SubstringSafe(clean, 2);

         return isVeliki ? veliki : mali;
      }

      internal static Kupdob Create_Kupdob_from_CreditNote(MySqlConnection conn, CreditNoteType creditNoteType, bool isKupac)
      {
         bool isDobav = !isKupac;

         Kupdob kupdob_rec = new Kupdob();

         PartyType theParty = isKupac ? creditNoteType.AccountingCustomerParty?.Party :
                                        creditNoteType.AccountingSupplierParty?.Party;

         kupdob_rec.R1kind = ZXC.F2_R1enum.B2B;

         uint newSifra = kupdob_rec.VvDao.GetNextSifra_Uint(conn, kupdob_rec.VirtualRecordName, "kupdobCD", kupdob_rec.UintSifraRootNum, kupdob_rec.UintSifraBaseFactor);

         kupdob_rec.KupdobCD = newSifra;
         kupdob_rec.Ticker = newSifra.ToString();

         kupdob_rec.Oib = isKupac ? creditNoteType.VvCustomerOIB : creditNoteType.VvSupplierOIB;

         kupdob_rec.Naziv = theParty.PartyLegalEntity?[0]?.RegistrationName?.Value ?? string.Empty;
         kupdob_rec.Ulica1 = theParty.PostalAddress?.StreetName?.Value ?? string.Empty;
         kupdob_rec.Ulica2 = theParty.PostalAddress?.StreetName?.Value ?? string.Empty;
         kupdob_rec.Grad = theParty.PostalAddress?.CityName?.Value ?? string.Empty;
         kupdob_rec.PostaBr = theParty.PostalAddress?.PostalZone?.Value ?? string.Empty;
         kupdob_rec.VatCntryCode = theParty.PostalAddress?.Country?.IdentificationCode?.Value ?? string.Empty;

         kupdob_rec.Naziv = ZXC.LenLimitedStr(kupdob_rec.Naziv, ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KpdbCI.naziv));
         kupdob_rec.Ulica1 = ZXC.LenLimitedStr(kupdob_rec.Ulica1, ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KpdbCI.ulica1));
         kupdob_rec.Ulica2 = ZXC.LenLimitedStr(kupdob_rec.Ulica2, ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KpdbCI.ulica2));
         kupdob_rec.Grad = ZXC.LenLimitedStr(kupdob_rec.Grad, ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KpdbCI.grad));

         if(isDobav)
         {
            //kupdob_rec.Ziro1 = invoiceType.PaymentMeans[0].PayeeFinancialAccount.ID.Value;

            string kupdobZiro = creditNoteType.PaymentMeans?[0]?.PayeeFinancialAccount?.ID?.Value;
            if(!string.IsNullOrWhiteSpace(kupdobZiro))
            {
               kupdob_rec.Ziro1 = kupdobZiro;
            }
         }

         return kupdob_rec;
      }
      public Faktur Create_Faktur_From_CreditNoteType(XSqlConnection conn, /*VvMER_ResponseData responseData*/ uint electronicID, DateTime sentDate, Kupdob kupdob_rec, bool isIFA, uint xtranoRecID = 0)
      {
         #region init

         bool isUFA = !isIFA;

         Faktur faktur_rec = InvoiceType.Create_COMMON_Faktur_For_eRacun(isIFA);

         Rtrans rtrans_rec;

         ushort line = 0;

         List<Rtrans> rtransList = new List<Rtrans>(this.CreditNoteLine.Length);

         bool OK = true;
         //Faktur fak;
         //Rtrans rtr;

         #endregion init

         #region ZAGLAVLJE računa

         // From Kupdob 

         faktur_rec.KupdobName = faktur_rec.PosJedName = kupdob_rec.Naziv;
         faktur_rec.KupdobCD = faktur_rec.PosJedCD = kupdob_rec.KupdobCD;
         faktur_rec.KupdobTK = faktur_rec.PosJedTK = kupdob_rec.Ticker;
         faktur_rec.KdUlica = faktur_rec.PosJedUlica = kupdob_rec.Ulica1;
         faktur_rec.KdZip = faktur_rec.PosJedZip = kupdob_rec.PostaBr;
         faktur_rec.KdMjesto = faktur_rec.PosJedMjesto = kupdob_rec.Grad;
         faktur_rec.KdOib = kupdob_rec.Oib;
         faktur_rec.VatCntryCode = kupdob_rec.VatCntryCode;
         //faktur_rec.KdAdresa     = Faktur.GetAdresa(Fld_KupdobUlica, Fld_KupdobZip, Fld_KupdobMjesto); ;

         faktur_rec.F2_R1kind = kupdob_rec.R1kind;

         if(isUFA)
            faktur_rec.ZiroRn = kupdob_rec.Ziro1;

         // From CreditNoteType 

         faktur_rec.DokDate = (this.IssueTime != null && this.IssueTime.Value != DateTime.MinValue)
            ? this.IssueDate.Value.Date.Add(this.IssueTime.Value.TimeOfDay)
            : this.IssueDate.Value;

         //faktur_rec.DospDate  = this.DueDate.Value;
         //if(this.DueDate != null && this.DueDate.Value != DateTime.MinValue) faktur_rec.DospDate  = this.DueDate.Value  ;

         // 25.02.2026: dodan if 
         if(ZXC.CURR_prjkt_rec.PdvRTip != ZXC.PdvRTipEnum.OBRT_R2 &&
            ZXC.CURR_prjkt_rec.PdvRTip != ZXC.PdvRTipEnum.POD_PO_NAPL)
         {
            faktur_rec.PdvDate = this.TaxPointDate?.Value == null ? this.IssueDate.Value : this.TaxPointDate.Value;
         }

         faktur_rec.VezniDok = this.ID.Value;
         //faktur_rec.Napomena  = this.Note[0].Value  ; // hocemo li samo prvu napomenu?
         faktur_rec.Napomena = ZXC.F2_Unprocessed;
         for(int i = 0; this.Note != null && i < this.Note.Length; i++)
         {
            faktur_rec.Opis += this.Note[i].Value + Environment.NewLine;
         }

         faktur_rec.DevName = this.DocumentCurrencyCode.Value;

         //faktur_rec.PdvKolTip =  this.ProfileID.Value;
         //negdje bi mozda trebalo staviti i tip poslovnog procesa (ProfileID) i /ili kod tipa računa (CreditNoteTypeCode) da se zna dali je račun za avans ili ne

         //faktur_rec.PnbM = GetPNB_FromCreditNoteType(false);
         //faktur_rec.PnbV = GetPNB_FromCreditNoteType(true );

         // From some Response 
         if(ZXC.IsF2_2026_rules == false) faktur_rec.FiskPrgBr = "[" + electronicID.ToString() + "]";
         faktur_rec.F2_ElectronicID = electronicID;
         faktur_rec.F2_SentTS = sentDate;
         if(isUFA)
            faktur_rec.F2_ArhRecID = xtranoRecID;

         #endregion ZAGLAVLJE računa

         #region STAVKE računa

         foreach(CreditNoteLineType creditNoteLine in this.CreditNoteLine)
         {
            rtrans_rec = new Rtrans();

            rtrans_rec.T_artiklName = ZXC.LenLimitedStr(creditNoteLine.Item.Name.Value, ZXC.RtransDao.GetSchemaColumnSize(ZXC.RtrCI.t_artiklName));

            rtrans_rec.T_kol = /*creditNoteLine.InvoicedQuantity.Value;*/ 1M;

            rtrans_rec.T_pdvSt = creditNoteLine.Item?.ClassifiedTaxCategory?[0]?.Percent?.Value ?? 0M;

            //rtrans_rec.T_cij         =           creditNoteLine.Price.PriceAmount.Value ;
            rtrans_rec.T_cij = -Math.Abs(creditNoteLine.Price.PriceAmount.Value);

            rtrans_rec.CalcTransResults(null);

            rtransList.Add(rtrans_rec);

         } // foreach(InvoiceLineType invoiceLine in this.InvoiceLine) 

         #endregion STAVKE računa

         #region TakeTransesSumToDokumentSum

         faktur_rec.Transes = rtransList;
         faktur_rec.TakeTransesSumToDokumentSum(true);
         faktur_rec.Transes = null;

         #endregion TakeTransesSumToDokumentSum

         foreach(Rtrans rtrans in rtransList)
         {
            OK = FakturDao.AutoSetFaktur(conn, ref line, faktur_rec, rtrans);
         }

         #region Return

         if(OK) return faktur_rec;
         else return null;

         #endregion Return

      }
   }
}

namespace EN16931.UBL.QWE2
{
   public partial class EvidentirajNaplatuZahtjev
   {
      private static System.Xml.Serialization.XmlSerializer serializer;
      private static System.Xml.Serialization.XmlSerializer Serializer
         {
            get
            {
               if((serializer == null))
               {
                  serializer = new System.Xml.Serialization.XmlSerializer(typeof(EvidentirajNaplatuZahtjev));
               }
               return serializer;
            }
         }

      public static EvidentirajNaplatuZahtjev Deserialize(string xml)
         {
            System.IO.StringReader stringReader = null;
            try
            {
               stringReader = new System.IO.StringReader(xml);
               return ((EvidentirajNaplatuZahtjev)(Serializer.Deserialize(System.Xml.XmlReader.Create(stringReader))));
            }
            finally
            {
               if((stringReader != null))
               {
                  stringReader.Dispose();
               }
            }
         }

      public virtual string Serialize(System.Text.Encoding encoding)
         {
            System.IO.StreamReader streamReader = null;
            System.IO.MemoryStream memoryStream = null;
            string xmlString = "";

            try
            {


               System.Xml.XmlWriterSettings xmlWriterSettings = new System.Xml.XmlWriterSettings()
               {
                  Encoding = encoding,
                  Indent = true, // byQ 
                  IndentChars = "   ", // byQ 
               };

               memoryStream = new System.IO.MemoryStream();

               System.Xml.XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
               //Serializer.Serialize(xmlWriter, this                         );           
               Serializer.Serialize(xmlWriter, this); // byJura 
               memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
               streamReader = new System.IO.StreamReader(memoryStream);
               xmlString = streamReader.ReadToEnd();

               xmlString = InvoiceType.NormalizeIssueTimeToHHmmss(xmlString);

               return xmlString;
            }
            finally
            {
               if((streamReader != null))
               {
                  streamReader.Dispose();
               }
               if((memoryStream != null))
               {
                  memoryStream.Dispose();
               }
            }
         }

      public virtual string SaveToFile(string fileName, System.Text.Encoding encoding, out System.Exception exception)
         {
            exception = null;
            try
            {
               return SaveToFile(fileName, encoding);
               //return true;
            }
            catch(System.Exception e)
            {
               exception = e;
               return "";
            }
         }

      public virtual string SaveToFile(string fileName, out System.Exception exception)
         {
            return SaveToFile(fileName, /*Encoding.UTF8*/ ZXC.VvUTF8Encoding_noBOM, out exception);
         }

      public virtual string SaveToFile(string fileName)
         {
            return SaveToFile(fileName, /*Encoding.UTF8*/ ZXC.VvUTF8Encoding_noBOM);
         }

      public virtual string SaveToFile(string fileName, System.Text.Encoding encoding)
         {
            string xmlString = "";

            System.IO.StreamWriter streamWriter = null;
            try
            {
               xmlString = Serialize(encoding);
               xmlString = InvoiceType.NormalizeIssueTimeToHHmmss(xmlString); // jebemvamsvimamater 

            streamWriter = new System.IO.StreamWriter(fileName, false, /*Encoding.UTF8*/ ZXC.VvUTF8Encoding_noBOM);
               streamWriter.WriteLine(xmlString);
               streamWriter.Close();
            }
            finally
            {
               if((streamWriter != null))
               {
                  streamWriter.Dispose();
               }
            }

            return xmlString;
         }
      /// <summary>
      /// Normalizes datumVrijemeSlanja to HH:mm:ss format in XML
      /// </summary>
      public static string NormalizeDatumVrijemeSlanjaToHHmmss(string xmlString)
      {
         if(string.IsNullOrEmpty(xmlString))
            return xmlString;

         try
         {
            System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Parse(xmlString);

            // Find all datumVrijemeSlanja elements
            var datumVrijemeSlanjaElements = doc.Descendants()
               .Where(e => e.Name.LocalName == "datumVrijemeSlanja");

            foreach(var element in datumVrijemeSlanjaElements)
            {
               string value = element.Value;
               if(!string.IsNullOrEmpty(value))
               {
                  // Try to parse as DateTime
                  if(DateTime.TryParse(value, out DateTime dt))
                  {
                     // Format to ensure HH:mm:ss time portion
                     element.Value = dt.ToString("yyyy-MM-ddTHH:mm:ss");
                  }
               }
            }

            return doc.ToString();
         }
         catch(Exception ex)
         {
            System.Diagnostics.Debug.WriteLine($"Error normalizing datumVrijemeSlanja: {ex.Message}");
            return xmlString; // Return original if error
         }
      }
      public static EvidentirajNaplatuZahtjev Create_MAP_XML_ZAGLAVLJE()
      {
         EvidentirajNaplatuZahtjev the_MAP_XML = new EvidentirajNaplatuZahtjev();

         the_MAP_XML.Zaglavlje = new Zaglavlje();

         the_MAP_XML.Zaglavlje.datumVrijemeSlanja = DateTime.Now;

         return the_MAP_XML;
      }

      public static Naplata Create_MAP_XML_Naplata_From_MAPaction((Ftrans ftrans, Faktur faktur) MAP_Action)
      {
         nacinPlacanja theNacinPlacanja = (nacinPlacanja)Enum.Parse(typeof(nacinPlacanja), MAP_Action.faktur.CjenikTT);

         Naplata theNaplata = new Naplata()
         {
            brojDokumenta             = MAP_Action.faktur./*TtNumFiskal*/VezniDok,
            datumIzdavanja            = MAP_Action.faktur.DokDate    ,
            oibPorezniBrojIzdavatelja = ZXC.CURR_prjkt_rec.Oib       ,
            oibPorezniBrojPrimatelja  = MAP_Action.faktur.KdOib      ,
            datumNaplate              = MAP_Action.ftrans.T_dokDate  ,
            naplaceniIznos            = MAP_Action.ftrans.T_pot      ,
            nacinPlacanja             = theNacinPlacanja              
         };

         return theNaplata;
      }

   }
}