using System;
using System.IO;

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
}