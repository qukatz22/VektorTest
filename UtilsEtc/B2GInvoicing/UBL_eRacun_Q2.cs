using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

using B2GSendInvoicePKIClient.B2GSendInvoicePKIWebService;
using B2GSendInvoicePKIClient.WSClient;
using B2GSendInvoicePKIClient.Operations;
using B2GSendInvoicePKIClient.XML;
using System.Globalization;

namespace EN16931.UBL
{
   public partial class InvoiceType
   {
      // 14.10.2025: serializator za Time zaprlja nesto sa milisekundama pa da s ovim ocistimo
      /* byQ timeAbrakakobredabra: */
      private static string NormalizeIssueTimeToHHmmss(string xml)
      {
         // Match the cbc:IssueTime content regardless of what .NET emitted (e.g., 11:34:37.0000000+02:00)
         // and replace it with HH:mm:ss.
         return System.Text.RegularExpressions.Regex.Replace(
            xml,
            @"(<cbc:IssueTime>)([^<]+)(</cbc:IssueTime>)",
            m =>
            {
               var raw = m.Groups[2].Value.Trim();

               // Try DateTimeOffset first (handles offsets), then DateTime as fallback.
               if(DateTimeOffset.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto))
               {
                  return m.Groups[1].Value + dto.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + m.Groups[3].Value;
               }
               if(DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt))
               {
                  return m.Groups[1].Value + dt.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + m.Groups[3].Value;
               }

               // If parsing fails, leave as-is.
               return m.Value;
            },
            System.Text.RegularExpressions.RegexOptions.Singleline);
      }

      #region VvConstructors

      public InvoiceType()
      {
         //this.AccountingSupplierParty = new SupplierPartyType
         //{
         //   Party = new PartyType
         //   {
         //      PartyLegalEntity = new PartyLegalEntityType[1]
         //   }
         //};
         //this.AccountingSupplierParty.Party.PartyLegalEntity[0].RegistrationName = new RegistrationNameType();

         //this.AccountingSupplierParty                                            = new SupplierPartyType();
         //this.AccountingSupplierParty.Party                                      = new PartyType();
         //this.AccountingSupplierParty.Party.PartyLegalEntity                     = new PartyLegalEntityType[] { new PartyLegalEntityType() };
         //this.AccountingSupplierParty.Party.PartyLegalEntity[0].RegistrationName = new RegistrationNameType();

      }

      #endregion VvConstructors

      #region Serialize/Deserialize

      private static System.Xml.Serialization.XmlSerializer serializer;
      private static System.Xml.Serialization.XmlSerializer Serializer
      {
         get
         {
            if((serializer == null))
            {
               serializer = new System.Xml.Serialization.XmlSerializer(typeof(InvoiceType));
            }
            return serializer;
         }
      }

      /// <summary>
      /// Serializes current InvoiceType object into an XML InvoiceType
      /// </summary>
      /// <returns>string XML value</returns>
      public virtual string Serialize(System.Text.Encoding encoding)
      {
         System.IO.StreamReader streamReader = null;
         System.IO.MemoryStream memoryStream = null;
         string xmlString = "";

         try
         {
            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();                                   // byJura 
            xmlSerializerNamespaces.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"   ); // byJura 
            xmlSerializerNamespaces.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"       ); // byJura 
            xmlSerializerNamespaces.Add("sig", "urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2"   ); // byJura lejter 
            xmlSerializerNamespaces.Add("sbc", "urn:oasis:names:specification:ubl:schema:xsd:SignatureBasicComponents-2"    ); // byJura lejter 
            xmlSerializerNamespaces.Add("sac", "urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2"); // byJura lejter 
            xmlSerializerNamespaces.Add("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2"   ); // byJura lejter 

            System.Xml.XmlWriterSettings xmlWriterSettings = new System.Xml.XmlWriterSettings()
            { 
               Encoding    = encoding,
               Indent      = true    , // byQ 
               IndentChars = "   "   , // byQ 
            };

            memoryStream = new System.IO.MemoryStream();

            System.Xml.XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
          //Serializer.Serialize(xmlWriter, this                         );           
            Serializer.Serialize(xmlWriter, this, xmlSerializerNamespaces); // byJura 
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            streamReader = new System.IO.StreamReader(memoryStream);
            xmlString = streamReader.ReadToEnd();

            // 14.10.2025: Normalize cbc:IssueTime to HH:mm:ss (EN16931 requires hh:mm:ss with no fraction/offset)
            /* byQ timeAbrakakobredabra: */ // ILI ćeš sa NormalizeIssueTimeToHHmmss, ILI sa XmlIgnore za IssueTime u InvoiceType.cs
            xmlString = NormalizeIssueTimeToHHmmss(xmlString);

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

      public virtual string Serialize()
      {
         return Serialize(/*Encoding.UTF8*/ ZXC.VvUTF8Encoding_noBOM);
      }

      /// <summary>
      /// Deserializes workflow markup into an InvoiceType object
      /// </summary>
      /// <param name="xml">string workflow markup to deserialize</param>
      /// <param name="obj">Output InvoiceType object</param>
      /// <param name="exception">output Exception value if deserialize failed</param>
      /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
      public static bool Deserialize(string xml, out InvoiceType obj, out System.Exception exception)
      {
         exception = null;
         obj = default(InvoiceType);
         try
         {
            obj = Deserialize(xml);
            return true;
         }
         catch(System.Exception ex)
         {
            exception = ex;
            return false;
         }
      }

      public static bool Deserialize(string xml, out InvoiceType obj)
      {
         System.Exception exception = null;
         return Deserialize(xml, out obj, out exception);
      }

      public static InvoiceType Deserialize(string xml)
      {
         System.IO.StringReader stringReader = null;
         try
         {
            stringReader = new System.IO.StringReader(xml);
            return ((InvoiceType)(Serializer.Deserialize(System.Xml.XmlReader.Create(stringReader))));
         }
         finally
         {
            if((stringReader != null))
            {
               stringReader.Dispose();
            }
         }
      }

#if tumorou

      public virtual string SaveToFile(string fileName, System.Text.Encoding encoding, System.Security.Cryptography.X509Certificates.X509Certificate2 certificate, string certPassword)
      {
         string xmlString = "";

         System.IO.StreamWriter streamWriter = null;
         try
         {
            xmlString = Serialize(encoding);

      #region UBLextension - UBLsigner (By FINA - Jura)

            byte[] xmlSignedByteArray = null;

            if(certificate != null)
            {
               hr.fina.eracun.signature.UBLSigner ublSigner = new hr.fina.eracun.signature.UBLSigner(certificate, certPassword, "Invoice");
               //u memoryStream je xml prije potpisa 
               xmlSignedByteArray = ublSigner.signUBLDocument(encoding.GetBytes(xmlString));
            }

      #endregion UBLextension - UBLsigner

            streamWriter = new System.IO.StreamWriter(fileName, false, /*Encoding.UTF8*/ ZXC.VvUTF8Encoding_noBOM);

            if(xmlSignedByteArray != null)
            {
               streamWriter.WriteLine(xmlSignedByteArray);
            }
            else
            {
               streamWriter.WriteLine(xmlString);
            }

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

#endif

      /// <summary>
      /// Serializes current InvoiceType object into file
      /// </summary>
      /// <param name="fileName">full path of outupt xml file</param>
      /// <param name="exception">output Exception value if failed</param>
      /// <returns>true if can serialize and save into file; otherwise, false</returns>
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

      public bool VvSaveToFile(string xmlString, string fileName, System.Text.Encoding encoding)
      {
         System.IO.StreamWriter streamWriter = null;

         bool isOK = true;
         try
         {
            streamWriter = new System.IO.StreamWriter(fileName, false, encoding /*Encoding.UTF8*/ /*ZXC.VvUTF8Encoding_noBOM*/);
            streamWriter.WriteLine(xmlString);
            streamWriter.Close();
         }
         catch(Exception)
         {
            isOK = false;
         }
         finally
         {
            if((streamWriter != null))
            {
               streamWriter.Dispose();
            }
         }

         return isOK;
      }

      /// <summary>
      /// Deserializes xml markup from file into an InvoiceType object
      /// </summary>
      /// <param name="fileName">string xml file to load and deserialize</param>
      /// <param name="the_eRacun">Output InvoiceType object</param>
      /// <param name="exception">output Exception value if deserialize failed</param>
      /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
      public static bool LoadFromFile(string fileName, System.Text.Encoding encoding, out InvoiceType the_eRacun, out System.Exception exception)
      {
         exception = null;
         the_eRacun = default(InvoiceType);
         try
         {
            the_eRacun = LoadFromFile(fileName, encoding);
            return true;
         }
         catch(System.Exception ex)
         {
            exception = ex;
            return false;
         }
      }

      public static bool LoadFromFile(string fileName, out InvoiceType the_eRacun, out System.Exception exception)
      {
         return LoadFromFile(fileName, /*Encoding.UTF8*/ ZXC.VvUTF8Encoding_noBOM, out the_eRacun, out exception);
      }

      public static bool LoadFromFile(string fileName, out InvoiceType the_eRacun)
      {
         System.Exception exception = null;
         return LoadFromFile(fileName, out the_eRacun, out exception);
      }

      //public static InvoiceType LoadFromFile(string fileName)
      //{
      //   return LoadFromFile(fileName, /*Encoding.UTF8*/ ZXC.VvUTF8Encoding_noBOM);
      //}

      public static InvoiceType LoadFromFile(string fileName, System.Text.Encoding encoding)
      {
         System.IO.FileStream file = null;
         System.IO.StreamReader sr = null;
         try
         {
            file = new System.IO.FileStream(fileName, FileMode.Open, FileAccess.Read);
            sr = new System.IO.StreamReader(file, encoding);
            string xmlString = sr.ReadToEnd();
            sr.Close();
            file.Close();
            return Deserialize(xmlString);
         }
         finally
         {
            if((file != null))
            {
               file.Dispose();
            }
            if((sr != null))
            {
               sr.Dispose();
            }
         }
      }

      #endregion

      #region XSD Validating

      private static XmlSchemaSet GetXmlSchemaSet()
      {
         List<ZXC.VvXmlValidationData> valDataList = new List<ZXC.VvXmlValidationData>
         {
            new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:Invoice-2"                     , @"XSD\eRacun\UBL-Invoice-2.1.xsd"                     ),
            new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2"                  , @"XSD\eRacun\UBL-CreditNote-2.1.xsd"                  ),
            new ZXC.VvXmlValidationData(@"urn:un:unece:uncefact:data:specification:CoreComponentTypeSchemaModule:2"   , @"XSD\eRacun\CCTS_CCT_SchemaModule-2.1.xsd"           ),
            new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"   , @"XSD\eRacun\UBL-CommonAggregateComponents-2.1.xsd"   ),
            new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"       , @"XSD\eRacun\UBL-CommonBasicComponents-2.1.xsd"       ),
            new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2"   , @"XSD\eRacun\UBL-CommonExtensionComponents-2.1.xsd"   ),
            new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2"   , @"XSD\eRacun\UBL-CommonSignatureComponents-2.1.xsd"   ),
            new ZXC.VvXmlValidationData(@"urn:un:unece:uncefact:documentation:2"                                      , @"XSD\eRacun\UBL-CoreComponentParameters-2.1.xsd"     ),
            new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2"   , @"XSD\eRacun\UBL-ExtensionContentDataType-2.1.xsd"    ),
            new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:QualifiedDataTypes-2"          , @"XSD\eRacun\UBL-QualifiedDataTypes-2.1.xsd"          ),
            new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2", @"XSD\eRacun\UBL-SignatureAggregateComponents-2.1.xsd"),
            new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:SignatureBasicComponents-2"    , @"XSD\eRacun\UBL-SignatureBasicComponents-2.1.xsd"    ),
            new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:UnqualifiedDataTypes-2"        , @"XSD\eRacun\UBL-UnqualifiedDataTypes-2.1.xsd"        ),
            new ZXC.VvXmlValidationData(@"http://uri.etsi.org/01903/v1.3.2#"                                          , @"XSD\eRacun\UBL-XAdESv132-2.1.xsd"                   ),
            new ZXC.VvXmlValidationData(@"http://uri.etsi.org/01903/v1.4.1#"                                          , @"XSD\eRacun\UBL-XAdESv141-2.1.xsd"                   ),
            new ZXC.VvXmlValidationData(@"http://www.w3.org/2000/09/xmldsig#"                                         , @"XSD\eRacun\UBL-xmldsig-core-schema-2.1.xsd"         )
         };

         XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();

         foreach(ZXC.VvXmlValidationData valData in valDataList)
         {
            xmlSchemaSet.Add(valData.targetNamespace, valData.schemaUri);
         }

         return xmlSchemaSet;
      }

      public static bool ValidateThis_XML_eRacun(MemoryStream stream)
      {
         XmlSchemaSet xmlSchemaSet = GetXmlSchemaSet();

         ZXC.ErrorsList = new List<string>();

         xsdOK = true; // xsdOK je NOT local. It's global field. 

         #region From MSDN help

         XmlReaderSettings settings = new XmlReaderSettings();

       //settings.ProhibitDtd             = false                ; // eRacun additions! 
         settings.DtdProcessing           = DtdProcessing.Parse  ; // eRacun additions! 
         settings.ValidationType          = ValidationType.Schema;
         settings.ValidationEventHandler += new ValidationEventHandler(settings_ValidationEventHandler);

         settings.Schemas = xmlSchemaSet;

         stream.Position = 0; // !!! 

         using(XmlReader reader = XmlReader.Create(stream, settings))
         {
            while(reader.Read()); // ovo, zapravo, performs validation 
         }

         #endregion From MSDN help

         if(xsdOK == true)
         {
            //ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Datoteka: [{0}]\n\nUSPJESNO VALIDIRANA.", fileName);

            //#region VvSendMail

            //string message = string.Format("Da li zelite exportiranu datoteku: [{0}]\n\nposlati kao privitak mail poruke?", fileName);
            //System.Windows.Forms.DialogResult resultSendMail = System.Windows.Forms.MessageBox.Show(message, "POSLATI MAILOM?!", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
            //if(resultSendMail == System.Windows.Forms.DialogResult.Yes)
            //{
            //   //ZXC.TheVvForm.VvSendFileAsMailAttachment(fileName);
            //   ZXC.TheVvForm.ExporVvtReportToFile_Manager(true, ZXC.RptExportKind.XML/*, false, false*/);
            //}

            //#endregion VvSendMail

         }
         else // Validation Error(s) Occured 
         {
            ZXC.aim_emsg_List("Greške XML Validacije", ZXC.ErrorsList, true);
         }

         return xsdOK;
      }

      public static bool ValidateThis_XML_eRacun(string fileName)
      {
         XmlSchemaSet xmlSchemaSet = GetXmlSchemaSet();

         ZXC.ErrorsList = new List<string>();

         xsdOK = true; // xsdOK je NOT local. It's global field. 

         #region From MSDN help

         XmlReaderSettings settings = new XmlReaderSettings();

       //settings.ProhibitDtd             = false                ; // eRacun additions! 
         settings.DtdProcessing           = DtdProcessing.Parse  ; // eRacun additions! 
         settings.ValidationType          = ValidationType.Schema;
         settings.ValidationEventHandler += new ValidationEventHandler(settings_ValidationEventHandler);

         settings.Schemas = xmlSchemaSet;

         using(XmlReader reader = XmlReader.Create(fileName, settings))
         {
            while(reader.Read()); // ovo, zapravo, performs validation 
         }

         #endregion From MSDN help

         if(xsdOK == true)
         {
            //ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Datoteka: [{0}]\n\nUSPJESNO VALIDIRANA.", fileName);

            //#region VvSendMail

            //string message = string.Format("Da li zelite exportiranu datoteku: [{0}]\n\nposlati kao privitak mail poruke?", fileName);
            //System.Windows.Forms.DialogResult resultSendMail = System.Windows.Forms.MessageBox.Show(message, "POSLATI MAILOM?!", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
            //if(resultSendMail == System.Windows.Forms.DialogResult.Yes)
            //{
            //   //ZXC.TheVvForm.VvSendFileAsMailAttachment(fileName);
            //   ZXC.TheVvForm.ExporVvtReportToFile_Manager(true, ZXC.RptExportKind.XML/*, false, false*/);
            //}

            //#endregion VvSendMail

         }
         else // Validation Error(s) Occured 
         {
            ZXC.aim_emsg_List("Greške Validacije XML-a [" + fileName + "]", ZXC.ErrorsList, true);
         }

         return xsdOK;
      }

      private static void settings_ValidationEventHandler(object sender, ValidationEventArgs e)
      {
         xsdOK = false;
         //ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "e-Račun datoteka: [{1}]\n\nGreška validacije: [{0}]", e.Message, FileNameGlobal);
         ZXC.ErrorsList.Add(e.Message);
      }

      //private static List<ZXC.VvXmlValidationData> Create_VvXmlValidationData_List()
      //{
      //   return new List<ZXC.VvXmlValidationData>
      //   {
      //      new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:Invoice-2"                     , @"XSD\eRacun\UBL-Invoice-2.1.xsd"                     ),
      //      new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2"                  , @"XSD\eRacun\UBL-CreditNote-2.1.xsd"                  ),
      //      new ZXC.VvXmlValidationData(@"urn:un:unece:uncefact:data:specification:CoreComponentTypeSchemaModule:2"   , @"XSD\eRacun\CCTS_CCT_SchemaModule-2.1.xsd"           ),
      //      new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"   , @"XSD\eRacun\UBL-CommonAggregateComponents-2.1.xsd"   ),
      //      new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"       , @"XSD\eRacun\UBL-CommonBasicComponents-2.1.xsd"       ),
      //      new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2"   , @"XSD\eRacun\UBL-CommonExtensionComponents-2.1.xsd"   ),
      //      new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2"   , @"XSD\eRacun\UBL-CommonSignatureComponents-2.1.xsd"   ),
      //      new ZXC.VvXmlValidationData(@"urn:un:unece:uncefact:documentation:2"                                      , @"XSD\eRacun\UBL-CoreComponentParameters-2.1.xsd"     ),
      //      new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2"   , @"XSD\eRacun\UBL-ExtensionContentDataType-2.1.xsd"    ),
      //      new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:QualifiedDataTypes-2"          , @"XSD\eRacun\UBL-QualifiedDataTypes-2.1.xsd"          ),
      //      new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2", @"XSD\eRacun\UBL-SignatureAggregateComponents-2.1.xsd"),
      //      new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:SignatureBasicComponents-2"    , @"XSD\eRacun\UBL-SignatureBasicComponents-2.1.xsd"    ),
      //      new ZXC.VvXmlValidationData(@"urn:oasis:names:specification:ubl:schema:xsd:UnqualifiedDataTypes-2"        , @"XSD\eRacun\UBL-UnqualifiedDataTypes-2.1.xsd"        ),
      //      new ZXC.VvXmlValidationData(@"http://uri.etsi.org/01903/v1.3.2#"                                          , @"XSD\eRacun\UBL-XAdESv132-2.1.xsd"                   ),
      //      new ZXC.VvXmlValidationData(@"http://uri.etsi.org/01903/v1.4.1#"                                          , @"XSD\eRacun\UBL-XAdESv141-2.1.xsd"                   ),
      //      new ZXC.VvXmlValidationData(@"http://www.w3.org/2000/09/xmldsig#"                                         , @"XSD\eRacun\UBL-xmldsig-core-schema-2.1.xsd"         )
      //   };
      //}

      private static bool xsdOK;

      #endregion XSD Validating

      #region Get_eRacun_Application_Certificate

      public X509Certificate2 Get_eRacun_Application_Certificate(string findCertIssuedTo) // "Aplikacijski Certifikat" (PULL) ... ima i Posluziteljski za PUSH 
      {
         X509Certificate2 cert = null;

       //bool success = false;

         #region some debug stuff
         //byQ: start
         //try
         //{
         //   store = new X509Store(/*"MY"*/StoreName.My, StoreLocation.CurrentUser);
         //   store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
         //   X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
         //   X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
         //   X509Certificate2Collection scollection = X509Certificate2UI.SelectFromCollection(fcollection, "Test Certificate Select", "Select a certificate from the following list to get information on that certificate", X509SelectionFlag.SingleSelection);
         //   debug(String.Format("Number of certificates: {0}{1}", scollection.Count, Environment.NewLine));
         //   foreach(X509Certificate2 x509 in scollection)
         //   {
         //      byte[] rawdata = x509.RawData;
         //      
         //      debug(String.Format("Content Type: {0}{1}"         , X509Certificate2.GetCertContentType(rawdata),    Environment.NewLine));
         //      debug(String.Format("Friendly Name: {0}{1}"        , x509.FriendlyName,                               Environment.NewLine));
         //      debug(String.Format("Certificate Verified?: {0}{1}", x509.Verify(),                                   Environment.NewLine));
         //      debug(String.Format("Simple Name: {0}{1}"          , x509.GetNameInfo(X509NameType.SimpleName, true), Environment.NewLine));
         //      debug(String.Format("Signature Algorithm: {0}{1}"  , x509.SignatureAlgorithm.FriendlyName,            Environment.NewLine));
         //      debug(String.Format("Private Key: {0}{1}"          , x509.PrivateKey.ToXmlString(false),              Environment.NewLine));
         //      debug(String.Format("Public Key: {0}{1}"           , x509.PublicKey.Key.ToXmlString(false),           Environment.NewLine));
         //      debug(String.Format("Certificate Archived?: {0}{1}", x509.Archived,                                   Environment.NewLine));
         //      debug(String.Format("Length of Raw Data: {0}{1}"   , x509.RawData.Length,                             Environment.NewLine));
         //      X509Certificate2UI.DisplayCertificate(x509);
         //      x509.Reset();
         //   }
         //   //store.Close();
         //}
         //catch(CryptographicException)
         //{
         //   Console.WriteLine("Information could not be written out for this certificate.");
         //}
         //byQ: end
         #endregion some debug stuff

         //Sign with certificate selection in the windows certificate store

         List<X509Store> X509StoreList = new List<X509Store>();

         X509Store store_Personal_CurrentUser  = new X509Store(StoreName.My  , StoreLocation.CurrentUser ); X509StoreList.Add(store_Personal_CurrentUser );
         X509Store store_Personal_LocalMachine = new X509Store(StoreName.My  , StoreLocation.LocalMachine); X509StoreList.Add(store_Personal_LocalMachine);
         X509Store store_Root_CurrentUser      = new X509Store(StoreName.Root, StoreLocation.CurrentUser ); X509StoreList.Add(store_Root_CurrentUser     );
         X509Store store_Root_LocalMachine     = new X509Store(StoreName.Root, StoreLocation.LocalMachine); X509StoreList.Add(store_Root_LocalMachine    );

         #region Certificate FILTERS

       //X509FindType findType;
       //object findValue;
       //bool validOnly = false;
       //
       //// FILTER 1 
       //findType = X509FindType.FindByTimeValid;
       //findValue = DateTime.Now;
       //X509Certificate2Collection fcollection1 = (X509Certificate2Collection)collection.Find(findType, findValue, validOnly);
       //
       //// FILTER 2 
       //findType = X509FindType.FindByIssuerName;
       //findValue = "ZABA CA";
       //X509Certificate2Collection fcollection2 = (X509Certificate2Collection)fcollection1.Find(findType, findValue, validOnly);
       //
       //// FILTER 3 
       //findType = X509FindType.FindByKeyUsage;
       //findValue = X509KeyUsageFlags.DigitalSignature;
       //X509Certificate2Collection fcollection3 = (X509Certificate2Collection)fcollection2.Find(findType, findValue, validOnly);

       //string title = "Odabir potpisnog certifikata";
       //string message = "Odaberite s liste certifikat kojim želite digitalno potisati PDF datoteku.";
       //X509Certificate2Collection scollection = X509Certificate2UI.SelectFromCollection(fcollection1/*3*/, title, message, X509SelectionFlag.SingleSelection);

         #endregion Certificate FILTERS

         X509Certificate2Collection certCollection;

         foreach(X509Store store in X509StoreList)
         {
            store.Open(OpenFlags.ReadOnly);
            certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, findCertIssuedTo, false);
            if(certCollection.Count.NotZero())
            {
               cert = certCollection[0];
               break;
            }
         }

         if(cert == null)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "'{0}' \n\nCertifikat nije pronađen.", findCertIssuedTo);
            //store_Personal_CurrentUser.Close();
            foreach(X509Store store in X509StoreList)
            {
               store.Close();
            }

            return /*false*/null;
         }

         //try
         //{
         //   success = Sign_eRacun_WithThisCert(cert, fullPath_Raw_XML_FileName); // VOILA 
         //}
         //catch(Exception ex)
         //{
         //   ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, ex.Message);
         //   success = false;
         //}

         //store_Personal_CurrentUser.Close();
         foreach(X509Store store in X509StoreList)
         {
            store.Close();
         }

         return cert;
      }

#if Wrong_Way
      public bool Sign_eRacun_WithThisCert(X509Certificate2 certifikat, XmlDocument xmlDocument)
      {
         //bool success = false;

         RSACryptoServiceProvider provider = (RSACryptoServiceProvider)certifikat.PrivateKey;

         SignedXml xml = null;
         try
         {
            xml                                   = new SignedXml(xmlDocument);
            xml.SigningKey                        = provider;
            xml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            KeyInfo keyInfo             = new KeyInfo();
            KeyInfoX509Data keyInfoData = new KeyInfoX509Data();

            keyInfoData.AddCertificate(certifikat);
            keyInfoData.AddIssuerSerial(certifikat.Issuer, certifikat.GetSerialNumberString());
            keyInfo.AddClause(keyInfoData);

            xml.KeyInfo = keyInfo;


            Reference reference = new Reference("");
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform(false));
            reference.AddTransform(new XmlDsigExcC14NTransform           (false));
          //reference.Uri = "#signXmlId";
            xml.AddReference(reference);
            xml.ComputeSignature();

            XmlElement element = xml.GetXml();
            xmlDocument.DocumentElement.AppendChild(element);
         }
         catch (Exception ex)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška kod potpisivanja XML dokumenta: {0}", ex.Message);

            return false;
         }

         return true;
      }
#endif

      #endregion Get_eRacun_Application_Certificate

      #region VvSendB2GOutgoingInvoiceViaPKIWebService

      internal string VvSendB2GOutgoingInvoiceViaPKIWebService(byte[] xmlSignedByteArray, X509Certificate2 clientCertificate, string certificatePassword)
      {
         string endpointAddress           =  "https://prez.fina.hr/SendB2GOutgoingInvoicePKIWebService/services/SendB2GOutgoingInvoicePKIWebService";
         string dnsIdentity               =  "e-invoice";
       //string serviceCertificatePath    =  "putanja/do/e-invoice_DEMO.cer";
       //string clientCertificatePath     =  "putanja/do/privatnikljuc.p12" ;
       //string clientCertificatePassword =  "***password od p12***"        ;
         string serviceCertificatePath    = @"D:\000_XSD_Qtmp\ByJuraByQ\B2GSendInvoicePKIClient\certs\demo\e-invoice_DEMO.cer"; // TODO: !!! 
       //string clientCertificatePath     = @"D:\000_XSD_Qtmp\ByJuraByQ\B2GSendInvoicePKIClient\certs\demo\VEKTOR.p12"        ;
       //string clientCertificatePassword =  "1q1q1Q";
       
         string supplierID        = /*"9934:60042587515"    */ this.AccountingSupplierParty.Party.PartyIdentification[0].ID.Value;
         string buyerID           = /*"9934:65119154523"    */ this.AccountingCustomerParty.Party.PartyIdentification[0].ID.Value;
         string supplierInvoiceID = /*"broj-racuna-0001-NET"*/ this.ID.Value;

         bool invoiceSentSuccess;
         string sendingResult = "";

         try
         { 
          //CreateInvoiceXML xml              = new CreateInvoiceXML();
          //byte[]           signedInvoiceXML = xml.getSignedInvoice();
            
            B2GGenericClient genericClient = new B2GGenericClient();
            eRacunB2GPortTypeClient client = 
          //genericClient.GetB2GClient(endpointAddress, dnsIdentity, serviceCertificatePath, clientCertificatePath, clientCertificatePassword);
            genericClient.GetB2GClient(endpointAddress, dnsIdentity, serviceCertificatePath, clientCertificate                               ); // byQ 
        
            SendInvoiceOperation      sendInvoiceOperation = new SendInvoiceOperation();
            SendB2GOutgoingInvoiceMsg message              = sendInvoiceOperation.GetSendInvoiceMsg(supplierID, buyerID, supplierInvoiceID, xmlSignedByteArray);
        
            SendB2GOutgoingInvoiceAckMsg ackMessage = client.sendB2GOutgoingInvoice(message); // Voila! 

            //Console.Out.WriteLine("MeesageID: \t"     + ackMessage.MessageAck.MessageID    );
            //Console.Out.WriteLine("MeesageAckID: \t"  + ackMessage.MessageAck.MessageAckID );
            //Console.Out.WriteLine("AckStatus: \t"     + ackMessage.MessageAck.AckStatus    );
            //Console.Out.WriteLine("AckStatusCode: \t" + ackMessage.MessageAck.AckStatusCode);
            //Console.Out.WriteLine("AckStatusText: \t" + ackMessage.MessageAck.AckStatusText);

            if(ackMessage.MessageAck.AckStatus != AckStatusType.ACCEPTED) // Message is NOT accepted 
            {
               invoiceSentSuccess = false;

               sendingResult = "Err1: " + ackMessage.MessageAck.AckStatusText;

               ZXC.aim_emsg_List("AckMessages", new List<string>
               {
                  "MessageID: \t"     + ackMessage.MessageAck.MessageID    ,
                  "MeesageAckID: \t"  + ackMessage.MessageAck.MessageAckID ,
                  "AckStatus: \t"     + ackMessage.MessageAck.AckStatus    ,
                  "AckStatusCode: \t" + ackMessage.MessageAck.AckStatusCode,
                  "AckStatusText: \t" + ackMessage.MessageAck.AckStatusText,
               });
            }

            ZXC.aim_log("eRačun AckMsg: " + ackMessage.MessageAck.AckStatus + " - " + ackMessage.MessageAck.AckStatusText + " [" + ackMessage.MessageAck.MessageID + "] [" + ackMessage.MessageAck.MessageAckID + "]");

          //if(ackMessage.MessageAck.AckStatus.Equals(B2GSendInvoicePKIClient.B2GSendInvoicePKIWebService.AckStatusType.ACCEPTED))
            if(ackMessage.MessageAck.AckStatus == AckStatusType.ACCEPTED) // Message is OK 
            {
               SendB2GOutgoingInvoiceAckMsgB2GOutgoingInvoiceEnvelopeB2GOutgoingInvoiceProcessing processing = ackMessage.B2GOutgoingInvoiceEnvelope.B2GOutgoingInvoiceProcessing;
               
               //ako je ispravan račun
               if(processing.Item is SendB2GOutgoingInvoiceAckMsgB2GOutgoingInvoiceEnvelopeB2GOutgoingInvoiceProcessingCorrectB2GOutgoingInvoice)
               {
                  invoiceSentSuccess = true;

                  SendB2GOutgoingInvoiceAckMsgB2GOutgoingInvoiceEnvelopeB2GOutgoingInvoiceProcessingCorrectB2GOutgoingInvoice
                  correctOutgoingInvoice = (SendB2GOutgoingInvoiceAckMsgB2GOutgoingInvoiceEnvelopeB2GOutgoingInvoiceProcessingCorrectB2GOutgoingInvoice)processing.Item;

                  sendingResult = "OK: " + correctOutgoingInvoice.InvoiceTimestamp.ToString(ZXC.VvDateAndTimeFormat) + " [" + correctOutgoingInvoice.InvoiceID + "]";
                  ZXC.aim_log("eRačun Sent OK: [" + correctOutgoingInvoice.InvoiceID + "]");

                  //Console.Out.WriteLine("\nISPRAVAN RAČUN!!!");
                  //Console.Out.WriteLine("SupplierInvoiceID: \t" + correctOutgoingInvoice.SupplierInvoiceID);
                  //Console.Out.WriteLine("InvoiceID: \t\t"       + correctOutgoingInvoice.InvoiceID        );
                  //Console.Out.WriteLine("InvoiceTimestamp: \t"  + correctOutgoingInvoice.InvoiceTimestamp );
               }

               //ako je neispravan račun
               if(processing.Item is SendB2GOutgoingInvoiceAckMsgB2GOutgoingInvoiceEnvelopeB2GOutgoingInvoiceProcessingIncorrectB2GOutgoingInvoice)
               {
                  invoiceSentSuccess = false;

                  SendB2GOutgoingInvoiceAckMsgB2GOutgoingInvoiceEnvelopeB2GOutgoingInvoiceProcessingIncorrectB2GOutgoingInvoice
                  incorrectOutgoingInvoice = (SendB2GOutgoingInvoiceAckMsgB2GOutgoingInvoiceEnvelopeB2GOutgoingInvoiceProcessingIncorrectB2GOutgoingInvoice)processing.Item;

                  sendingResult = "Err2: " + incorrectOutgoingInvoice.ErrorMessage;
                  ZXC.aim_log("eRačun Send Err: [" + incorrectOutgoingInvoice.ErrorCode + "] [" + incorrectOutgoingInvoice.ErrorMessage + "]");

                  //Console.Out.WriteLine("\nNEISPRAVAN RAČUN!!!"                                             );
                  //Console.Out.WriteLine("SupplierInvoiceID: \t" + incorrectOutgoingInvoice.SupplierInvoiceID);
                  //Console.Out.WriteLine("ErrorCode: \t\t"       + incorrectOutgoingInvoice.ErrorCode        );
                  //Console.Out.WriteLine("ErrorMessage: \t"      + incorrectOutgoingInvoice.ErrorMessage     );
               }

            } // if(ackMessage.MessageAck.AckStatus == AckStatusType.ACCEPTED) // Message is OK 

            //Console.ReadLine();
         }
         catch (Exception e)
         {
            ZXC.aim_emsg_VvException(e);
            //Console.Out.WriteLine("Greska: " + e.Message);
            //Console.Out.WriteLine("Inner: " + e.InnerException);
            //Console.ReadLine();
         }

         return sendingResult;

      } // internal void VvSendB2GOutgoingInvoiceViaPKIWebService(byte[] xmlSignedByteArray)

      #endregion VvSendB2GOutgoingInvoiceViaPKIWebService

      #region VvUBL Enums

      /// <summary>
      /// http://docs.oasis-open.org/ubl/cos1-UBL-2.1/cva/UBL-DefaultDTQ-2.1.html#d30e1
      /// </summary>
      public enum QuantityCode
      {
         [Description("")]
         Null,
         [Description("H87")]
         Piece,
         [Description("KGM")]
         Kilogram,
         [Description("KMT")]
         Kilometre,
         [Description("GRM")]
         Gram,
         [Description("MTR")]
         Metre,
         [Description("LTR")]
         Litre,
         [Description("TNE")]
         Tonne,
         [Description("MTK")]
         SquareMetre,
         [Description("MTQ")]
         CubicMetre,
         [Description("MIN")]
         Minute,
         [Description("HUR")]
         Hour,
         [Description("DAY")]
         Day,
         [Description("MON")]
         Month,
         [Description("ANN")]
         Year
      }

      #endregion VvUBL Enums

   }
}
