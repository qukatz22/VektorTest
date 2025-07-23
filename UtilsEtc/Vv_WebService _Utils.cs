using System;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Text;

public static class Vv_WS
{
   public static WebRequest CreateWebRequest(ISoapMessage soapMessage)
   {
      var webRequest = WebRequest.Create(soapMessage.Uri);
      webRequest.ContentType = "text/xml;charset=utf-8";
      webRequest.ContentLength = soapMessage.ContentXml.Length;

      webRequest.Headers.Add("SOAPAction", soapMessage.SoapAction);
      webRequest.Credentials = soapMessage.Credentials;
      webRequest.Method = "POST";
      webRequest.GetRequestStream().Write(Encoding.UTF8.GetBytes(soapMessage.ContentXml), 0, soapMessage.ContentXml.Length);

      return webRequest;
   }

   public interface ISoapMessage
   {
      string Uri { get; }
      string ContentXml { get; }
      string SoapAction { get; }
      ICredentials Credentials { get; }
   }

   // ============================================================================================================================ 
   // ============================================================================================================================ 
   // ============================================================================================================================ 

   public static string GetEncodedBasicAuthorization(string username, string password)
   {
      string encodedAuthorization = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
      return encodedAuthorization;
   }


   // "Basic HTTP authorization" 
   public static WebRequest CreateWebRequest(string uri, /*ICredentials credentials*/String encodedAuthorization, XmlDocument xmlDocument, string method)
   {
      WebRequest webRequest = WebRequest.Create(uri);

      if(webRequest == null)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "For uri: [{0}] webRequest is NULL!", uri);
         return null;
      }

      webRequest.ContentType = "text/xml;charset=utf-8";

      //... tu si nest zapeo. Ne znas da li je npr "LijekInfoEcho" 'metoda' koju spominju
      // webRequest.Method ili nekakva SOAP action
      webRequest.Method      = "POST";
    //webRequest.Method      = method;

      //webRequest.Credentials = credentials;

      webRequest.Headers.Add("Authorization", "Basic " + encodedAuthorization);

      byte[] byteArray = UTF8Encoding.UTF8.GetBytes(xmlDocument.InnerXml);
      webRequest.ContentLength = byteArray.Length;

      using(Stream requestStream = webRequest.GetRequestStream())
      {
         requestStream.Write(byteArray, 0, byteArray.Length);
      }

      return webRequest;
   }

 //public static HttpWebResponse GetWebResponse(WebRequest webRequest)
   public static XmlDocument GetXmlDocumentFromWebResponse(WebRequest webRequest)
   {
       HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse;

       Stream responseStream = response.GetResponseStream();
       Encoding encode = Encoding.GetEncoding("utf-8");
       StreamReader readStream = new StreamReader(responseStream, encode);
       string txt = readStream.ReadToEnd();

       XmlDocument xmlDocument = new XmlDocument();
       xmlDocument.PreserveWhitespace = true;
       xmlDocument.LoadXml(txt);

      return xmlDocument;
   }

}
