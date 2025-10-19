using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.IO.Compression;

// Qukatz 6.11.2007: ako ostavis orginalno 'PaddingMode.ISO10126' onda svaki put encoding daje drugi razultat pa te jebe DirtyFlagging
// kad stavis PaddingMode.Zeros onda radi dobro. Nota bene: nemam pojma kako ista od ove 'ebene enkripcije radi! 

public static class VvAES //http://tinyurl.com/2gcy64 
{
   /// <summary>
   /// Use AES to encrypt data string. The output string is the encrypted bytes as a base64 string.
   /// The same password must be used to decrypt the string.
   /// </summary>
   /// <param name="data">Clear string to encrypt.</param>
   /// <param name="password">Password used to encrypt the string.</param>
   /// <returns>Encrypted result as Base64 string.</returns>
   public static string EncryptData(string data, string password)
   {
      if(data == null)
         throw new ArgumentNullException("data");
      if(password == null)
         throw new ArgumentNullException("password"); 
      
      byte[] encBytes = EncryptData(Encoding.UTF8.GetBytes(data), password, PaddingMode./*ISO10126*/Zeros);
      
      return Convert.ToBase64String(encBytes);
   }        
   
   /// <summary>
   /// Decrypt the data string to the original string.  The data must be the base64 string
   /// returned from the EncryptData method.
   /// </summary>
   /// <param name="data">Encrypted data generated from EncryptData method.</param>
   /// <param name="password">Password used to decrypt the string.</param>
   /// <returns>Decrypted string.</returns>
   public static string DecryptData(string data, string password)
   {
      if(data == null)
         throw new ArgumentNullException("data");
      if(password == null)
         throw new ArgumentNullException("password"); 
      
      byte[] encBytes = Convert.FromBase64String(data);
      
      byte[] decBytes = DecryptData(encBytes, password, PaddingMode./*ISO10126*/Zeros);
      
      return Encoding.UTF8.GetString(decBytes);
   }

   public static byte[] EncryptData(byte[] data, string password, PaddingMode paddingMode)
   {
      if(data == null || data.Length == 0)
         throw new ArgumentNullException("data");
      if(password == null)
         throw new ArgumentNullException("password"); 
      
      PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, Encoding.UTF8.GetBytes("Salt"));
      
      RijndaelManaged rm = new RijndaelManaged();
      
      rm.Padding = paddingMode;
      ICryptoTransform encryptor = rm.CreateEncryptor(pdb.GetBytes(16), pdb.GetBytes(16)); 
      using(MemoryStream msEncrypt = new MemoryStream())
      using(CryptoStream encStream = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
      {
         encStream.Write(data, 0, data.Length);
         encStream.FlushFinalBlock();
         return msEncrypt.ToArray();
      }
   }

   public static byte[] DecryptData(byte[] data, string password, PaddingMode paddingMode)
   {
      if(data == null || data.Length == 0)
         // 30.11.2013: 
         //throw new ArgumentNullException("data");
         return new byte[0];
      if(password == null)
         throw new ArgumentNullException("password"); 
      
      PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, Encoding.UTF8.GetBytes("Salt"));
      RijndaelManaged rm = new RijndaelManaged();
      rm.Padding = paddingMode;
      ICryptoTransform decryptor = rm.CreateDecryptor(pdb.GetBytes(16), pdb.GetBytes(16)); 
      using(MemoryStream msDecrypt = new MemoryStream(data))
      using(CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
      {
         // Decrypted bytes will always be less then encrypted bytes, so len of encrypted data will be big enouph for buffer.
         byte[] fromEncrypt = new byte[data.Length];                // Read as many bytes as possible.
         int read = csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
         if(read < fromEncrypt.Length)
         {
            // Return a byte array of proper size.
            byte[] clearBytes = new byte[read];
            Buffer.BlockCopy(fromEncrypt, 0, clearBytes, 0, read);
            return clearBytes;
         }
         return fromEncrypt;
      }
   }
}

internal static class VvStringCompressor
{
   ///  <summary> 
   /// Compresses the string. 
   ///  </summary> 
   ///  <param name="text"> The text.</param> 
   ///  <returns> </returns> 
   public static string CompressString(string text)
   {
      byte[] buffer = Encoding.UTF8.GetBytes(text);
      var memoryStream = new MemoryStream();
      using(var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
      {
         gZipStream.Write(buffer, 0, buffer.Length);
      }

      memoryStream.Position = 0;

      var compressedData = new byte[memoryStream.Length];
      memoryStream.Read(compressedData, 0, compressedData.Length);

      var gZipBuffer = new byte[compressedData.Length + 4];
      Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
      Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
      return Convert.ToBase64String(gZipBuffer);
   }

   ///  <summary> 
   /// Decompresses the string. 
   ///  </summary> 
   ///  <param name="compressedText"> The compressed text.</param> 
   ///  <returns> </returns> 
   public static string DecompressString(string compressedText)
   {
      byte[] gZipBuffer = Convert.FromBase64String(compressedText);
      using(var memoryStream = new MemoryStream())
      {
         int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
         memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

         var buffer = new byte[dataLength];

         memoryStream.Position = 0;
         using(var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
         {
            gZipStream.Read(buffer, 0, buffer.Length);
         }

         return Encoding.UTF8.GetString(buffer);
      }
   }

   /// <summary>
   /// Compresses an XML string to a GZip-compressed byte array suitable for BLOB storage.
   /// </summary>
   public static byte[] CompressXml(string xml)
   {
      if(string.IsNullOrEmpty(xml))
         return new byte[0];

      byte[] buffer = Encoding.UTF8.GetBytes(xml);
      using(var memoryStream = new MemoryStream())
      {
         using(var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
         {
            gZipStream.Write(buffer, 0, buffer.Length);
         }
         return memoryStream.ToArray();
      }
   }
   /// <summary>
   /// Decompresses a GZip-compressed byte array back to the original XML string.
   /// </summary>
   public static string DecompressXml(byte[] compressedData)
   {
      if(compressedData == null || compressedData.Length == 0)
         return string.Empty;

      using(var memoryStream = new MemoryStream(compressedData))
      using(var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
      using(var resultStream = new MemoryStream())
      {
         gZipStream.CopyTo(resultStream);
         return Encoding.UTF8.GetString(resultStream.ToArray());
      }
   }
}