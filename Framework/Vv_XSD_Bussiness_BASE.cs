using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

public abstract class Vv_XSD_Bussiness_BASE<T> where T : class
{
   private static System.Xml.Serialization.XmlSerializer serializer;
   protected static System.Xml.Serialization.XmlSerializer Serializer
   {
      get
      {
         if(serializer == null)
         {
            serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
         }
         return serializer;
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
            Indent = true,
            IndentChars = "   ",
         };

         memoryStream = new System.IO.MemoryStream();
         System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(memoryStream, xmlWriterSettings);
         Serializer.Serialize(xmlWriter, this);
         memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
         streamReader = new System.IO.StreamReader(memoryStream);
         xmlString = streamReader.ReadToEnd();

         return xmlString;
      }
      finally
      {
         streamReader?.Dispose();
         memoryStream?.Dispose();
      }
   }

   public virtual string Serialize()
   {
      return Serialize(ZXC.VvUTF8Encoding_noBOM);
   }

   public static bool Deserialize(string xml, out T obj, out System.Exception exception)
   {
      exception = null;
      obj = default(T);
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

   public static bool Deserialize(string xml, out T obj)
   {
      System.Exception exception = null;
      return Deserialize(xml, out obj, out exception);
   }

   public static T Deserialize(string xml)
   {
      System.IO.StringReader stringReader = null;
      try
      {
         stringReader = new System.IO.StringReader(xml);
         return (T)(Serializer.Deserialize(System.Xml.XmlReader.Create(stringReader)));
      }
      finally
      {
         stringReader?.Dispose();
      }
   }

   public virtual string SaveToFile(string fileName, System.Text.Encoding encoding, out System.Exception exception)
   {
      exception = null;
      try
      {
         return SaveToFile(fileName, encoding);
      }
      catch(System.Exception e)
      {
         exception = e;
         return "";
      }
   }

   public virtual string SaveToFile(string fileName, out System.Exception exception)
   {
      return SaveToFile(fileName, ZXC.VvUTF8Encoding_noBOM, out exception);
   }

   public virtual string SaveToFile(string fileName)
   {
      return SaveToFile(fileName, ZXC.VvUTF8Encoding_noBOM);
   }

   public virtual string SaveToFile(string fileName, System.Text.Encoding encoding)
   {
      string xmlString = "";

      System.IO.StreamWriter streamWriter = null;
      try
      {
         xmlString = Serialize(encoding);
         streamWriter = new System.IO.StreamWriter(fileName, false, ZXC.VvUTF8Encoding_noBOM);
         streamWriter.WriteLine(xmlString);
         streamWriter.Close();
      }
      finally
      {
         streamWriter?.Dispose();
      }

      return xmlString;
   }

   public bool VvSaveToFile(string xmlString, string fileName, System.Text.Encoding encoding)
   {
      System.IO.StreamWriter streamWriter = null;
      bool isOK = true;
      try
      {
         streamWriter = new System.IO.StreamWriter(fileName, false, encoding);
         streamWriter.WriteLine(xmlString);
         streamWriter.Close();
      }
      catch(Exception)
      {
         isOK = false;
      }
      finally
      {
         streamWriter?.Dispose();
      }
      return isOK;
   }

   public static bool LoadFromFile(string fileName, System.Text.Encoding encoding, out T obj, out System.Exception exception)
   {
      exception = null;
      obj = default(T);
      try
      {
         obj = LoadFromFile(fileName, encoding);
         return true;
      }
      catch(System.Exception ex)
      {
         exception = ex;
         return false;
      }
   }

   public static bool LoadFromFile(string fileName, out T obj, out System.Exception exception)
   {
      return LoadFromFile(fileName, ZXC.VvUTF8Encoding_noBOM, out obj, out exception);
   }

   public static bool LoadFromFile(string fileName, out T obj)
   {
      System.Exception exception = null;
      return LoadFromFile(fileName, out obj, out exception);
   }

   public static T LoadFromFile(string fileName, System.Text.Encoding encoding)
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
         file?.Dispose();
         sr?.Dispose();
      }
   }

   #region 2025: XML Validation etc by copilot + byQ
   private static string RemoveXmlSignature(string xmlString)
   {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(xmlString);

      XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
      nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");

      XmlNode signatureNode = doc.SelectSingleNode("//ds:Signature", nsmgr);
      if(signatureNode != null)
      {
         signatureNode.ParentNode.RemoveChild(signatureNode);
      }

      return doc.OuterXml;
   }

   // 2025: made by Copilot
   internal static bool ValidateXmlAgainstXsd_MaybeOLD(string _xmlString, Stream xsdStream, out List<string> validationErrors)
   {
      string xmlString = RemoveXmlSignature(_xmlString);

      var errors = new List<string>();
      bool isValid = true;

      try
      {
         XmlSchemaSet schemas = new XmlSchemaSet();
         schemas.XmlResolver = new XmlUrlResolver();

         // Get the directory containing the main XSD file
         string xsdDirectory = null;
         if(xsdStream is FileStream fileStream)
         {
            xsdDirectory = Path.GetDirectoryName(fileStream.Name);
         }

         // Load the main XSD schema
         using(XmlReader xsdReader = XmlReader.Create(xsdStream))
         {
            schemas.Add(null, xsdReader);
         }

         // Load all dependent XSD files from the same directory
         if(!string.IsNullOrEmpty(xsdDirectory) && Directory.Exists(xsdDirectory))
         {
            string mainXsdFileName = Path.GetFileName(((FileStream)xsdStream).Name);
            string[] xsdFiles = Directory.GetFiles(xsdDirectory, "*.xsd");
            foreach(string xsdFile in xsdFiles)
            {
               // Skip the main XSD file to avoid duplicate declaration
               if(Path.GetFileName(xsdFile).Equals(mainXsdFileName, StringComparison.OrdinalIgnoreCase))
                  continue;

               try
               {
                  using(FileStream fs = new FileStream(xsdFile, FileMode.Open, FileAccess.Read))
                  using(XmlReader xsdReader = XmlReader.Create(fs))
                  {
                     schemas.Add(null, xsdReader);
                  }
               }
               catch(Exception ex)
               {
                  // Log but continue - some XSD files might be duplicates or invalid
                  errors.Add($"Warning: Could not load schema file {Path.GetFileName(xsdFile)}: {ex.Message}");
               }
            }
         }

         // Compile the schema set to resolve all references
         schemas.Compile();

         XmlReaderSettings settings = new XmlReaderSettings();
         settings.ValidationType = ValidationType.Schema;
         settings.Schemas = schemas;
         settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
         settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
         settings.XmlResolver = new XmlUrlResolver();

         settings.ValidationEventHandler += (sender, e) =>
         {
            // Skip validation errors in the signature namespace
            if(e.Message.Contains("http://www.w3.org/2000/09/xmldsig#") ||
               e.Message.Contains("X509SerialNumber"))
            {
               errors.Add($"Warning (Signature - ignored): {e.Message}");
               return; // Don't mark as invalid
            }

            errors.Add($"{e.Severity}: {e.Message}");
            if(e.Severity == XmlSeverityType.Error)
            {
               isValid = false;
            }
         };

         using(StringReader stringReader = new StringReader(xmlString))
         using(XmlReader xmlReader = XmlReader.Create(stringReader, settings))
         {
            while(xmlReader.Read()) { }
         }
      }
      catch(Exception ex)
      {
         errors.Add("EXCEPTION: " + ex.Message);
         isValid = false;
      }

      validationErrors = errors;
      return isValid;
   }
   // 2025: byQ:
   internal static bool ValidateXmlAgainstXsd(string xmlString)
   {
      string theXmlString = RemoveXmlSignature(xmlString);

      XmlDocument xmlDocument = ZXC.RemoveEmptyNodes(theXmlString);
      theXmlString = xmlDocument.OuterXml;
      MemoryStream memoryStream = new MemoryStream();
      xmlDocument.LoadXml(theXmlString);
      xmlDocument.Save(memoryStream);
      bool validateOK = false;

      try { validateOK = EN16931.UBL.InvoiceType.ValidateThis_XML_eRacun(memoryStream, true); } catch(Exception ex) { ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, ex.Message); }

      return validateOK;
   }

   #endregion 2025: XML Validation etc by copilot + byQ

}