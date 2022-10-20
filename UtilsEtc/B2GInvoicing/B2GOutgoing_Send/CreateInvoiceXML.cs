using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using EN16931.UBL;
using hr.fina.eracun.signature;

namespace B2GSendInvoicePKIClient.XML
{
    class CreateInvoiceXML
    {
        public byte[] getSignedInvoice()
        {

          //string clientCertificatePath     =  "putanja/do/privatnikljuc.p12";
          //string clientCertificatePassword =  "***password od p12***"       ;
            string clientCertificatePath     = @"D:\000_XSD_Qtmp\ByJuraByQ\B2GSendInvoicePKIClient\certs\demo\VEKTOR.p12";
            string clientCertificatePassword =  "1q1q1Q";

            InvoiceType invoice = new InvoiceType();

            IDType brojRacuna = new IDType();
            brojRacuna.Value = "broj-racuna-0001-NET";
            invoice.ID = brojRacuna;
            //TODO napuniti xml

            
            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            xmlSerializerNamespaces.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"    );
            xmlSerializerNamespaces.Add("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(InvoiceType));
            MemoryStream memoryStream   = new MemoryStream();
            xmlSerializer.Serialize(memoryStream, invoice, xmlSerializerNamespaces);

            UBLSigner ublSigner = new UBLSigner(clientCertificatePath, clientCertificatePassword, "Invoice");


         string vvSignedXmlString = @"<?xml version=""1.0"" encoding=""UTF - 8"" standalone=""no""?><Invoice xmlns=""urn: oasis: names: specification: ubl: schema: xsd: Invoice - 2"" xmlns:cac=""urn: oasis: names: specification: ubl: schema: xsd: CommonAggregateComponents - 2"" xmlns:cbc=""urn: oasis: names: specification: ubl: schema: xsd: CommonBasicComponents - 2"" xmlns:ext=""urn: oasis: names: specification: ubl: schema: xsd: CommonExtensionComponents - 2"" xmlns:sac=""urn: oasis: names: specification: ubl: schema: xsd: SignatureAggregateComponents - 2"" xmlns:sbc=""urn: oasis: names: specification: ubl: schema: xsd: SignatureBasicComponents - 2"" xmlns:sig=""urn: oasis: names: specification: ubl: schema: xsd: CommonSignatureComponents - 2""><ext:UBLExtensions><ext:UBLExtension><ext:ExtensionContent><sig:UBLDocumentSignatures><sac:SignatureInformation><Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"" Id=""data_signature""><SignedInfo><CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#WithComments""/><SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""/><Reference Type=""http://www.w3.org/2000/09/xmldsig#SignatureProperties"" URI=""#ide2664030-fc8c-457f-ba12-d55b4e95aa24""><Transforms><Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#WithComments""/></Transforms><DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/><DigestValue>0q4HxVm6sSZyezJoUTCP2Z562rk=</DigestValue></Reference><Reference URI=""""><Transforms><Transform Algorithm=""http://www.w3.org/2002/06/xmldsig-filter2""><XPath xmlns=""http://www.w3.org/2002/06/xmldsig-filter2"" xmlns:inv=""urn:oasis:names:specification:ubl:schema:xsd:Invoice-2"" Filter=""intersect"">here()/ancestor::inv:Invoice[1]</XPath><XPath xmlns=""http://www.w3.org/2002/06/xmldsig-filter2"" xmlns:inv=""urn:oasis:names:specification:ubl:schema:xsd:Invoice-2"" Filter=""subtract"">here()/ancestor::ext:UBLExtensions/ext:UBLExtension[last()]</XPath></Transform><Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#WithComments""/></Transforms><DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/><DigestValue>KWqsL+WeVym/HW9b4zPzxHQLmaM=</DigestValue></Reference></SignedInfo><SignatureValue>k9CJnswYD6HLOFO/Zpt1y+76yRN/9KB75pg1T9IYW+noH7spVFp/Vx4Zbz3LVaBQgYnQMeQ3kqfK
         EhEOkc3R2QwUMe + rlIve1GwNjELCdyFzZi + r9 / IUXOpH2aLTH2 / uRFJZvAG1HIS0pc9XBd9ojWqE
bS9AmuRI1fEdW2rQHYSWLt8jsbhYPjYLX +/ MAEgsuf7S / vS + 2hoWRz1PU + kpoI2Q12hc9GZ5DZFr
gx0V6Dfg5gAaKQihB88lFfk1BkSNQAnRIOKNkHWR / MAtWu3W0Zy8YVBbLnT4U9acjgZuEHEmSiT7
aaNYuakZVBoXhQ2OEPxWtZV / DAOsHcIibo / H + g ==</ SignatureValue >< KeyInfo >< X509Data >< X509Certificate > MIIHHTCCBQWgAwIBAgIQXIJFMCpmwgYAAAAAUywU1DANBgkqhkiG9w0BAQsFADBIMQswCQYDVQQG
EwJIUjEdMBsGA1UEChMURmluYW5jaWpza2EgYWdlbmNpamExGjAYBgNVBAMTEUZpbmEgRGVtbyBD
QSAyMDE0MB4XDTE4MTEwOTA5NTIxNloXDTIzMTEwOTA5NTIxNlowZTELMAkGA1UEBhMCSFIxHDAa
BgNVBAoTE1ZJUEVSLVpBR1JFQiBELk8uTy4xFjAUBgNVBGETDUhSNjAwNDI1ODc1MTUxDzANBgNV
BAcTBlpBR1JFQjEPMA0GA1UEAxMGVkVLVE9SMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKC
AQEAmu6syCkHRy4xWNIZhb9bURUqYcHquV2hWiipYp / aThrfmxexjpUt1vYHkY / S0z1ri2TX6E1O
+ lKIvizj7G6bC + 7RaufLA / T0GGXe / rj / W5hqZ1HKUT6F / RyUALIMKSdx7IKj9kfpNtvMq99f27V5
cK1Sm85Fz7DliSH7R5rwe + nRxAMBhDT4hyXNGKoQD1C / D2zEjo8ZgAnX1gWX8CAIWyS8JpCRInRr
bciNc61sIhsTS50YoV3BH6G784MNUCfMWOBGWkBrLS5HDkMCntMY9NsLJuRSSAqtJHaIIT / H2bCt
eeGCGN2z9 / WIYWh6DXi + PSyEE5hUUADcLF2eMc5hPwIDAQABo4IC5DCCAuAwDgYDVR0PAQH / BAQD
AgWgMB0GA1UdJQQWMBQGCCsGAQUFBwMEBggrBgEFBQcDAjCBrAYDVR0gBIGkMIGhMIGUBgkrfIhQ
BSAPAwEwgYYwQQYIKwYBBQUHAgEWNWh0dHA6Ly9kZW1vLXBraS5maW5hLmhyL2Nwcy9jcHNucWNk
ZW1vMjAxNHYyLTAtaHIucGRmMEEGCCsGAQUFBwIBFjVodHRwOi8vZGVtby1wa2kuZmluYS5oci9j
cHMvY3BzbnFjZGVtbzIwMTR2Mi0wLWVuLnBkZjAIBgYEAI96AQEwfQYIKwYBBQUHAQEEcTBvMCgG
CCsGAQUFBzABhhxodHRwOi8vZGVtbzIwMTQtb2NzcC5maW5hLmhyMEMGCCsGAQUFBzAChjdodHRw
Oi8vZGVtby1wa2kuZmluYS5oci9jZXJ0aWZpa2F0aS9kZW1vMjAxNF9zdWJfY2EuY2VyMBoGA1Ud
EQQTMBGBD3JvYmVydEB2aXBlci5ocjCCARgGA1UdHwSCAQ8wggELMIGmoIGjoIGghihodHRwOi8v
ZGVtby1wa2kuZmluYS5oci9jcmwvZGVtbzIwMTQuY3JshnRsZGFwOi8vZGVtby1sZGFwLmZpbmEu
aHIvY249RmluYSUyMERlbW8lMjBDQSUyMDIwMTQsbz1GaW5hbmNpanNrYSUyMGFnZW5jaWphLGM9
SFI / Y2VydGlmaWNhdGVSZXZvY2F0aW9uTGlzdCUzQmJpbmFyeTBgoF6gXKRaMFgxCzAJBgNVBAYT
AkhSMR0wGwYDVQQKExRGaW5hbmNpanNrYSBhZ2VuY2lqYTEaMBgGA1UEAxMRRmluYSBEZW1vIENB
IDIwMTQxDjAMBgNVBAMTBUNSTDExMB8GA1UdIwQYMBaAFDuEWhT1xTzhSDtd0Sc1e9VlvA4qMB0G
A1UdDgQWBBSkvSQHr + 3PSL9 + lisqSAr62VJFnjAJBgNVHRMEAjAAMA0GCSqGSIb3DQEBCwUAA4IC
AQBHcEBr8 + 0p4BdPLRMUrjy2iWoEcJZ6ArZ7OYrkYzUsLkOG2kg + SubI9ut / HaU3rkQZyB + BmJVW
AYAbsFFdKh2k + uO3QgFh60DABgD2NqkvG7jqvxn9eWF1cygPc84YrbnenLfEdTepdeUbGlZKY3gb
s3EZFPkCH / ZXRr0mpAlEA3BbsHZeJoGhpx1uLuq7AtkxmAcU8ULOhKpUOjux4JfiEFK5yzaG7WrZ
+ LI2 / F0YTXAL0dv9q / 7RZGA4ld1f0Xj2oztWUiAngnFeRZbOiK9yTQbX6xlh2lgzPaTnx4AF4ezC
     no3IzWyCby68bkc01YTVCrY2ZS1ftuOoTr0zkxQMm5eiac9WISLz67miSR85tNMeQKQvUjTzxOQh
HtV37MixQvoVHZ + tKqHE8EnUdFnw / CmjCal3F8gTJOVYPNYYn1I0Cksr4g5OPu + dHCZxjI9nyQL +
6kG7nDfRk5R9J9wKrvASXdLWPDFP + mGJDoHBv / SI8xgK5vZeVJQIl6IkMiJMlpFa5edb +/ gF3U8H
3VKQcfbaL58EkCYmUJq8VdRmAgX//H0yKE6fhat+Jbf9fpkA6ApsMrUnsvzD60xtsOd4g+KrigWy
QEKvTspRsvViOfqbBNucCaqGG59z / n1bFutIdEpDPR2xeu8RR1Hy5UI3buoTwnHmNt7Pan5OuxgH
tQ ==</ X509Certificate ></ X509Data ></ KeyInfo >< Object >< SignatureProperties Id = ""ide2664030-fc8c-457f-ba12-d55b4e95aa24"" >< SignatureProperty Target = ""data_signature"" >< Name xmlns = ""http://ns.adobe.com/pdf/2006"" > VEKTOR </ Name ></ SignatureProperty >< SignatureProperty Target = ""data_signature"" >< M xmlns = ""http://ns.adobe.com/pdf/2006"" > D:20181130130555 + 01'00' </ M ></ SignatureProperty ></ SignatureProperties ></ Object ></ Signature ></ sac:SignatureInformation ></ sig:UBLDocumentSignatures ></ ext:ExtensionContent ></ ext:UBLExtension ></ ext:UBLExtensions >< cbc:CustomizationID > urn:cen.eu:en16931: 2017 </ cbc:CustomizationID >< cbc:ProfileID > P1 </ cbc:ProfileID >< cbc:ID > 1 - 1 - 1 </ cbc:ID >< cbc:IssueDate > 2018 - 11 - 01 </ cbc:IssueDate >< cbc:IssueTime > 13:25:28.0000000 + 01:00 </ cbc:IssueTime >< cbc:DueDate > 2018 - 01 - 01 </ cbc:DueDate >< cbc:InvoiceTypeCode > 380 </ cbc:InvoiceTypeCode >< cbc:Note > 033#Oznaka operatera#</cbc:Note><cbc:Note>13:25#Vrijeme izdavanja#</cbc:Note><cbc:Note>Robert Manestar#Odgovorna osoba#</cbc:Note><cbc:DocumentCurrencyCode>HRK</cbc:DocumentCurrencyCode><cbc:TaxCurrencyCode>HRK</cbc:TaxCurrencyCode><cac:AccountingSupplierParty><cac:Party><cbc:EndpointID schemeID=""EMAIL"">viper@zg.htnet.hr</cbc:EndpointID><cac:PartyIdentification><cbc:ID>9934:60042587515</cbc:ID></cac:PartyIdentification><cac:PostalAddress><cbc:StreetName>Tuškanova BB</cbc:StreetName><cbc:CityName>YAGREB</cbc:CityName><cbc:PostalZone>10000</cbc:PostalZone><cac:Country><cbc:IdentificationCode>HR</cbc:IdentificationCode></cac:Country></cac:PostalAddress><cac:PartyTaxScheme><cbc:CompanyID>HR60042587515</cbc:CompanyID><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:PartyTaxScheme><cac:PartyLegalEntity><cbc:RegistrationName>VIPER1</cbc:RegistrationName></cac:PartyLegalEntity><cac:Contact><cbc:ElectronicMail>viper@zg.htnet.hr</cbc:ElectronicMail></cac:Contact></cac:Party></cac:AccountingSupplierParty><cac:AccountingCustomerParty><cac:Party><cbc:EndpointID schemeID=""EMAIL"">abnor.malan@svemir.hr</cbc:EndpointID><cac:PartyIdentification><cbc:ID>9934:65119154523</cbc:ID></cac:PartyIdentification><cac:PostalAddress><cac:Country><cbc:IdentificationCode>HR</cbc:IdentificationCode></cac:Country></cac:PostalAddress><cac:PartyTaxScheme><cbc:CompanyID>HR65119154523</cbc:CompanyID><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:PartyTaxScheme><cac:PartyLegalEntity><cbc:RegistrationName>0 Neki jako normalan i uobičajeni partner d.o.o.</cbc:RegistrationName></cac:PartyLegalEntity></cac:Party></cac:AccountingCustomerParty><cac:Delivery><cac:DeliveryLocation><cac:Address><cbc:StreetName>Dostavna Adresa</cbc:StreetName><cac:Country><cbc:IdentificationCode>HR</cbc:IdentificationCode></cac:Country></cac:Address></cac:DeliveryLocation></cac:Delivery><cac:PaymentMeans><cbc:PaymentMeansCode>30</cbc:PaymentMeansCode><cbc:PaymentID>1-1-1</cbc:PaymentID><cac:PayeeFinancialAccount><cbc:ID>HR6223600001101318630</cbc:ID></cac:PayeeFinancialAccount></cac:PaymentMeans><cac:AllowanceCharge><cbc:ChargeIndicator>false</cbc:ChargeIndicator><cbc:Amount currencyID=""HRK"">330000.00</cbc:Amount><cac:TaxCategory><cbc:ID>S</cbc:ID><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:TaxCategory></cac:AllowanceCharge><cac:AllowanceCharge><cbc:ChargeIndicator>false</cbc:ChargeIndicator><cbc:Amount currencyID=""HRK"">330000.00</cbc:Amount><cac:TaxCategory><cbc:ID>S</cbc:ID><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:TaxCategory></cac:AllowanceCharge><cac:AllowanceCharge><cbc:ChargeIndicator>false</cbc:ChargeIndicator><cbc:Amount currencyID=""HRK"">110000.00</cbc:Amount><cac:TaxCategory><cbc:ID>S</cbc:ID><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:TaxCategory></cac:AllowanceCharge><cac:TaxTotal><cbc:TaxAmount currencyID=""HRK"">1489100.00</cbc:TaxAmount><cac:TaxSubtotal><cbc:TaxableAmount currencyID=""HRK"">3670000.00</cbc:TaxableAmount><cbc:TaxAmount currencyID=""HRK"">917500.00</cbc:TaxAmount><cac:TaxCategory><cbc:ID>S</cbc:ID><cbc:Percent>25</cbc:Percent><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:TaxCategory></cac:TaxSubtotal><cac:TaxSubtotal><cbc:TaxableAmount currencyID=""HRK"">3670000.00</cbc:TaxableAmount><cbc:TaxAmount currencyID=""HRK"">477100.00</cbc:TaxAmount><cac:TaxCategory><cbc:ID>S</cbc:ID><cbc:Percent>13</cbc:Percent><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:TaxCategory></cac:TaxSubtotal><cac:TaxSubtotal><cbc:TaxableAmount currencyID=""HRK"">1890000.00</cbc:TaxableAmount><cbc:TaxAmount currencyID=""HRK"">94500.00</cbc:TaxAmount><cac:TaxCategory><cbc:ID>S</cbc:ID><cbc:Percent>5</cbc:Percent><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:TaxCategory></cac:TaxSubtotal></cac:TaxTotal><cac:LegalMonetaryTotal><cbc:LineExtensionAmount currencyID=""HRK"">10000000.00</cbc:LineExtensionAmount><cbc:TaxExclusiveAmount currencyID=""HRK"">9230000.00</cbc:TaxExclusiveAmount><cbc:TaxInclusiveAmount currencyID=""HRK"">10719100.00</cbc:TaxInclusiveAmount><cbc:PayableAmount currencyID=""HRK"">10719100.00</cbc:PayableAmount></cac:LegalMonetaryTotal><cac:InvoiceLine><cbc:ID>1</cbc:ID><cbc:InvoicedQuantity unitCode=""H87"">1000.00</cbc:InvoicedQuantity><cbc:LineExtensionAmount currencyID=""HRK"">1000000.00</cbc:LineExtensionAmount><cac:Item><cbc:Name>zAaa</cbc:Name><cac:ClassifiedTaxCategory><cbc:ID>S</cbc:ID><cbc:Percent>25</cbc:Percent><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:ClassifiedTaxCategory></cac:Item><cac:Price><cbc:PriceAmount currencyID=""HRK"">1000000.00</cbc:PriceAmount></cac:Price></cac:InvoiceLine><cac:InvoiceLine><cbc:ID>2</cbc:ID><cbc:InvoicedQuantity unitCode=""H87"">1000.00</cbc:InvoicedQuantity><cbc:LineExtensionAmount currencyID=""HRK"">1000000.00</cbc:LineExtensionAmount><cac:Item><cbc:Name>bb</cbc:Name><cac:ClassifiedTaxCategory><cbc:ID>S</cbc:ID><cbc:Percent>25</cbc:Percent><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:ClassifiedTaxCategory></cac:Item><cac:Price><cbc:PriceAmount currencyID=""HRK"">1000000.00</cbc:PriceAmount></cac:Price></cac:InvoiceLine><cac:InvoiceLine><cbc:ID>3</cbc:ID><cbc:InvoicedQuantity unitCode=""H87"">1000.00</cbc:InvoicedQuantity><cbc:LineExtensionAmount currencyID=""HRK"">1000000.00</cbc:LineExtensionAmount><cac:Item><cbc:Name>q1</cbc:Name><cac:ClassifiedTaxCategory><cbc:ID>S</cbc:ID><cbc:Percent>13</cbc:Percent><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:ClassifiedTaxCategory></cac:Item><cac:Price><cbc:PriceAmount currencyID=""HRK"">1000000.00</cbc:PriceAmount></cac:Price></cac:InvoiceLine><cac:InvoiceLine><cbc:ID>4</cbc:ID><cbc:InvoicedQuantity unitCode=""H87"">1000.00</cbc:InvoicedQuantity><cbc:LineExtensionAmount currencyID=""HRK"">1000000.00</cbc:LineExtensionAmount><cac:Item><cbc:Name>q2</cbc:Name><cac:ClassifiedTaxCategory><cbc:ID>S</cbc:ID><cbc:Percent>13</cbc:Percent><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:ClassifiedTaxCategory></cac:Item><cac:Price><cbc:PriceAmount currencyID=""HRK"">1000000.00</cbc:PriceAmount></cac:Price></cac:InvoiceLine><cac:InvoiceLine><cbc:ID>5</cbc:ID><cbc:InvoicedQuantity unitCode=""LTR"">1000.00</cbc:InvoicedQuantity><cbc:LineExtensionAmount currencyID=""HRK"">1000000.00</cbc:LineExtensionAmount><cac:Item><cbc:Name>q3</cbc:Name><cac:ClassifiedTaxCategory><cbc:ID>S</cbc:ID><cbc:Percent>5</cbc:Percent><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:ClassifiedTaxCategory></cac:Item><cac:Price><cbc:PriceAmount currencyID=""HRK"">1000000.00</cbc:PriceAmount></cac:Price></cac:InvoiceLine><cac:InvoiceLine><cbc:ID>6</cbc:ID><cbc:InvoicedQuantity unitCode=""H87"">1000.00</cbc:InvoicedQuantity><cbc:LineExtensionAmount currencyID=""HRK"">1000000.00</cbc:LineExtensionAmount><cac:Item><cbc:Name>zAaa</cbc:Name><cac:ClassifiedTaxCategory><cbc:ID>S</cbc:ID><cbc:Percent>25</cbc:Percent><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:ClassifiedTaxCategory></cac:Item><cac:Price><cbc:PriceAmount currencyID=""HRK"">890000.00</cbc:PriceAmount></cac:Price></cac:InvoiceLine><cac:InvoiceLine><cbc:ID>7</cbc:ID><cbc:InvoicedQuantity unitCode=""H87"">1000.00</cbc:InvoicedQuantity><cbc:LineExtensionAmount currencyID=""HRK"">1000000.00</cbc:LineExtensionAmount><cac:Item><cbc:Name>bb</cbc:Name><cac:ClassifiedTaxCategory><cbc:ID>S</cbc:ID><cbc:Percent>25</cbc:Percent><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:ClassifiedTaxCategory></cac:Item><cac:Price><cbc:PriceAmount currencyID=""HRK"">780000.00</cbc:PriceAmount></cac:Price></cac:InvoiceLine><cac:InvoiceLine><cbc:ID>8</cbc:ID><cbc:InvoicedQuantity unitCode=""H87"">1000.00</cbc:InvoicedQuantity><cbc:LineExtensionAmount currencyID=""HRK"">1000000.00</cbc:LineExtensionAmount><cac:Item><cbc:Name>q1</cbc:Name><cac:ClassifiedTaxCategory><cbc:ID>S</cbc:ID><cbc:Percent>13</cbc:Percent><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:ClassifiedTaxCategory></cac:Item><cac:Price><cbc:PriceAmount currencyID=""HRK"">890000.00</cbc:PriceAmount></cac:Price></cac:InvoiceLine><cac:InvoiceLine><cbc:ID>9</cbc:ID><cbc:InvoicedQuantity unitCode=""H87"">1000.00</cbc:InvoicedQuantity><cbc:LineExtensionAmount currencyID=""HRK"">1000000.00</cbc:LineExtensionAmount><cac:Item><cbc:Name>q2</cbc:Name><cac:ClassifiedTaxCategory><cbc:ID>S</cbc:ID><cbc:Percent>13</cbc:Percent><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:ClassifiedTaxCategory></cac:Item><cac:Price><cbc:PriceAmount currencyID=""HRK"">780000.00</cbc:PriceAmount></cac:Price></cac:InvoiceLine><cac:InvoiceLine><cbc:ID>10</cbc:ID><cbc:InvoicedQuantity unitCode=""LTR"">1000.00</cbc:InvoicedQuantity><cbc:LineExtensionAmount currencyID=""HRK"">1000000.00</cbc:LineExtensionAmount><cac:Item><cbc:Name>q3</cbc:Name><cac:ClassifiedTaxCategory><cbc:ID>S</cbc:ID><cbc:Percent>5</cbc:Percent><cac:TaxScheme><cbc:ID>VAT</cbc:ID></cac:TaxScheme></cac:ClassifiedTaxCategory></cac:Item><cac:Price><cbc:PriceAmount currencyID=""HRK"">890000.00</cbc:PriceAmount></cac:Price></cac:InvoiceLine></Invoice>";

         System.Text.Encoding encoding_noBOM = new System.Text.UTF8Encoding(false);
         byte[] vvSignedXmlByteArray = encoding_noBOM.GetBytes(vvSignedXmlString);
         return vvSignedXmlByteArray;




         byte[] signedXML = ublSigner.signUBLDocument(memoryStream.ToArray());

            return signedXML;
        }
    }
}
