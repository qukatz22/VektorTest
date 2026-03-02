using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Web;
using CrystalDecisions.Shared;
using System.Drawing;
using EN16931.UBL;
using System.Xml.Linq;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand = MySql.Data.MySqlClient.MySqlCommand;
#endif

public static class Vv_eRacun_HTTP
{
   public const bool DEMO = false;
   public static bool IsF2_2026_rules { get { return ZXC.CURR_prjkt_rec.F2_RolaKind != ZXC.F2_RolaKind.NEMA_F2 && ZXC.projectYearAsInt > 2025; } }

   #region MER Web Service URLs - API endpoints web addresses

   public const string VvMER_baseAddress_production = @"https://www.moj-eracun.hr"     ; 
   public const string VvMER_baseAddress_demo       = @"https://www-demo.moj-eracun.hr"; 

   public const string VvMER_webAddressPOST_Send            = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/apis/v2/send"                                 ; // POST 
   public const string VvMER_webAddressPOST_eIzvj           = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/api/fiscalization/eReporting"                 ; // POST 
   public const string VvMER_webAddressPOST_Receive         = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/apis/v2/receive"                              ; // POST 
   public const string VvMER_webAddressPOST_QueryOutbox     = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/apis/v2/queryOutbox"                          ; // POST 
   public const string VvMER_webAddressPOST_QueryOutbox_DPS = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/apis/v2/queryDocumentProcessStatusOutbox"     ; // POST 
   public const string VvMER_webAddressPOST_QueryInbox      = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/apis/v2/queryInbox"                           ; // POST 
   public const string VvMER_webAddressGET_Ping             = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/apis/v2/Ping"                                 ; // GET! 
   public const string VvMER_webAddressPOST_Check           = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/api/mps/check"                                ; // POST 
   public const string VvMER_webAddressPOST_FiskStatus      = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/api/fiscalization/status"                     ; // POST 
   public const string VvMER_webAddressPOST_FiskStatusOutbox= (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/api/fiscalization/StatusOutbox"               ; // POST 
   public const string VvMER_webAddressPOST_FiskStatusInbox = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/api/fiscalization/statusInbox"                ; // POST 
   public const string VvMER_webAddressPOST_MAP             = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/api/fiscalization/markPaid"                   ; // POST 
   public const string VvMER_webAddressPOST_MAP_WO_eID      = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/api/fiscalization/markPaidWithoutElectronicID"; // POST 

   public const string VvPND_baseAddress_production = @"https://eracun.eposlovanje.hr"; 
   public const string VvPND_baseAddress_demo       = @"https://test.eposlovanje.hr"; 

   public const string VvPND_webAddressPOST_Send            = (DEMO ? VvPND_baseAddress_demo : VvPND_baseAddress_production) + "/api/v2/document/send"            ; // POST 
   public const string VvPND_webAddressPOST_eIzvj           = (DEMO ? VvPND_baseAddress_demo : VvPND_baseAddress_production) + "/api/v2/ereporting/reportdocument"; // POST 
   public const string VvPND_webAddressGET_QueryOutbox      = (DEMO ? VvPND_baseAddress_demo : VvPND_baseAddress_production) + "/api/v2/document/status"          ; // GET! 
   public const string VvPND_webAddressPOST_outgoing        = (DEMO ? VvPND_baseAddress_demo : VvPND_baseAddress_production) + "/api/v2/document/outgoing"        ; // GET! 
   public const string VvPND_webAddressGET_Ping             = (DEMO ? VvPND_baseAddress_demo : VvPND_baseAddress_production) + "/api/v2/ping"                     ; // GET! 
   public const string VvPND_webAddressPOST_Check           = (DEMO ? VvPND_baseAddress_demo : VvPND_baseAddress_production) + "/api/v2/ams/check"                ; // POST 

   // MER authorisation parameters: 
   public static string VvMER_UserName   /*= DEMO ? "8633"    : ZXC.CURR_prjkt_rec.F2_UserName.NotEmpty()          ? ZXC.CURR_prjkt_rec.F2_UserName          : ZXC.CURR_prjkt_rec.SkyVvDomena         */;
   public static string VvMER_Password   /*= DEMO ? "T22zsEY" : ZXC.CURR_prjkt_rec.F2_PasswordDecrypted.NotEmpty() ? ZXC.CURR_prjkt_rec.F2_PasswordDecrypted : ZXC.CURR_prjkt_rec.SkyPasswordDecrypted*/;

   public static string VvMER_CompanyId  /* =                  ZXC.CURR_prjkt_rec.Oib*/                                    ;
   public const  string VvMER_SoftwareId    =                    "Vektor-001"                                              ;

   // PND authorisation parameters: 
   public static string VvPND_API_Key    = "1042a7915a7f66a23a8e0e98d93cb44c6d968263638a8cec54e07cb5abc2ae2f";
   public const  string VvPND_SoftwareId = "Vektor-001"                                                      ;

   public static readonly Dictionary</*string*/int, string> MER_TransportStatuses = new Dictionary</*string*/int, string>
   {
      { /*"20"*/ 20, "U obradi"           }, // TRN 
      { /*"30"*/ 30, "Poslan"             }, // TRN 
      { /*"40"*/ 40, "Preuzet"            }, // TRN 
      { /*"45"*/ 45, "Otkazano"           }, // TRN 
      { /*"50"*/ 50, "Neuspjelo"          }, // TRN 
      { /*"70"*/ 70, "Poslan kao eIZVJ"   }, // TRN 

      // kako MER navodi da su DPS statusi deprecated/obsolete, prestali smo API-jem ic po njih   
      // ako ikada vratimo da idemo po njih, tada vrati ovo ... al vidi kako ces tretirati 'zero' 
      // jer u tom kontekstu oin znaci 'prihvacen' ... IDIJOTI!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 
    //{           0, "Prihvaćen"          }, // DPS 
    //{ /*"20"*/  1, "Odbijen"            }, // DPS 
    //{ /*"30"*/  2, "Plaćeno SVE"        }, // DPS 
    //{ /*"40"*/  3, "Plaćen DIO"         }, // DPS 
    //{ /*"45"*/  4, "Potvrda zaprimanja" }, // DPS 
    //{ /*"50"*/ 99, "Zaprimljen"         }  // DPS 
      {           0, ""                   }, // NOT DPS! nego zero value ... 'nepoznato' 

   };

   #endregion MER Web Service URLs - API endpoints web addresses

   #region PND Web Service URLs - API endpoints web addresses

   //qweqwe

   #endregion PND Web Service URLs - API endpoints web addresses

   #region Private common methods - Voila!

   /// <summary>
   /// Sends a synchronous POST request with JSON content using HttpWebRequest
   /// and returns the HttpWebResponse.
   /// Includes explicit try/catch for safer error handling.
   /// </summary>
   private static HttpWebResponse Vv_POSTmethod_SendHttpWebRequest_GetHttpWebResponse(string webAddress, string jsonRequestString, string token = null)
   {
      HttpWebRequest  httpWebRequest  = null;
      HttpWebResponse httpWebResponse = null;

      try
      {
         // Create the request
         httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddress);
         httpWebRequest.ContentType = "application/json; charset=utf-8";
         httpWebRequest.Method = "POST";

         // Add Authorization header if provided
         if(token.NotEmpty())
         {
            httpWebRequest.Headers["Authorization"] = token;
         }

         // Convert JSON string to UTF-8 bytes (no BOM)
         byte[] requestBytes = Encoding.UTF8.GetBytes(jsonRequestString);

         // Write bytes to request stream
         Stream requestStream = httpWebRequest.GetRequestStream();
         try
         {
            requestStream.Write(requestBytes, 0, requestBytes.Length);
         }
         finally
         {
            requestStream.Close();
         }

         // Send request and get response
          httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse(); // <--- Here we go! !!! 
         return httpWebResponse;
      }
      catch(WebException webEx)
      {
         if(webEx.Response != null)
         {
            var resp = (HttpWebResponse)webEx.Response;
            string body = string.Empty;

            try
            {
               var stream = resp.GetResponseStream();
               if(stream != null)
               {
                  using(var sr = new StreamReader(stream))
                  {
                     body = sr.ReadToEnd(); // Note: consumes the stream
                  }
               }
            }
            catch
            {
               // Swallow logging read errors
            }

            string[] responseBodyLines = ZXC.PrettyPrintResponse(body).Split(new string[] { "\n", "\r", "\n\r", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            string[] webAddressParts   = webAddress.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            string theAPI              = webAddressParts.LastOrDefault();

            if(webAddress == VvMER_webAddressPOST_Check)
            {
               // ne tretiraj statusCode koji nije 200 kao exception,                
               // nego kao odgovor 'false' za MER-ov API 'VvMER_WebService_CheckAMS' 
            }
            else
            {
               ZXC.aim_emsg_List(theAPI + " [" + webAddress + "] Web Exception (status code:" + (int)resp.StatusCode + ") Description: " + resp.StatusDescription, responseBodyLines.ToList());
            }

            return resp; // If callers need the stream, avoid reading it here.
         }

         throw;
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(ex.Message);

         // General exception handling
         Console.WriteLine("Exception while sending HTTP request: " + ex.Message);
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Exception while sending HTTP request: {0}", ex.Message);
         throw;
      }
   }
   private static HttpWebResponse Vv_GETmethod_SendHttpWebRequest_GetHttpWebResponse(string urlWithParams, string token = null)
   {
      HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlWithParams);
      httpWebRequest.ContentType = "application/json; charset=utf-8";
      httpWebRequest.Method = "GET";

      // Add Authorization header if provided
      if(token.NotEmpty())
      {
         httpWebRequest.Headers["Authorization"] = token;
      }

      try
      {
         return (HttpWebResponse)httpWebRequest.GetResponse();
      }
      catch(WebException webEx)
      {
         if(webEx.Response != null)
         {
            HttpWebResponse resp = (HttpWebResponse)webEx.Response;
            string body = string.Empty;

            try
            {
               var stream = resp.GetResponseStream();
               if(stream != null)
               {
                  using(var sr = new StreamReader(stream))
                  {
                     body = sr.ReadToEnd(); // Note: consumes the stream
                  }
               }
            }
            catch
            {
               // Swallow logging read errors
            }

            string[] responseBodyLines = ZXC.PrettyPrintResponse(body).Split(new string[] { "\n", "\r", "\n\r", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            ZXC.aim_emsg_List("Web Exception (status code:" + (int)resp.StatusCode + ") Description: " + resp.StatusDescription, responseBodyLines.ToList());

            return resp; // If callers need the stream, avoid reading it here.
         }

         throw;
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(ex.Message);
         Console.WriteLine("Exception while sending HTTP request: " + ex.Message);
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Exception while sending HTTP request: {0}", ex.Message);
         throw;
      }
   }
   private static WebApiResult<T> Vv_POSTmethod_ExecuteJson<T>(ZXC.F2_WebApi webApiKind, string webAddress, string jsonRequestString, string token = null) where T : class, new()
   {
      WebApiResult<T> webApiResult = new WebApiResult<T>();

      webApiResult.WebApiKind = webApiKind;
      webApiResult.WebApiAddr = webAddress;

      try
      {
         HttpWebResponse httpResponse = Vv_POSTmethod_SendHttpWebRequest_GetHttpWebResponse(webAddress, jsonRequestString, token);

         webApiResult.StatusCode = (int)httpResponse.StatusCode;
         webApiResult.StatusDescription = httpResponse.StatusDescription;

         using(StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
         {
            string responseJson = streamReader.ReadToEnd();

            webApiResult.ResponseString = responseJson;

            if(!string.IsNullOrEmpty(responseJson))
            {
               try
               {
                  webApiResult.ResponseData = JsonConvert.DeserializeObject<T>(responseJson);
               }
               catch(Exception ex2)
               {
                  webApiResult.ResponseData = new T();
                  webApiResult.ErrorBody = responseJson;
                  webApiResult.ExceptionMessage = ex2.Message;
               }
            }
            else
            {
               webApiResult.ErrorBody = "JSON response is empty!";
            }
         }
      }
      catch(WebException ex)
      {
         if(ex.Response is HttpWebResponse resp)
         {
            webApiResult.StatusCode = (int)resp.StatusCode;
            webApiResult.StatusDescription = resp.StatusDescription;

            try
            {
               using(var sr = new StreamReader(resp.GetResponseStream()))
               {
                  webApiResult.ErrorBody = sr.ReadToEnd();
               }
            }
            catch { /* ignore */ }
         }
         webApiResult.ExceptionMessage = ex.Message;
      }
      catch(Exception ex)
      {
         webApiResult.ExceptionMessage = ex.Message;
      }

      return webApiResult;
   }
   private static T Vv_GETmethod_ExecuteJson<T, TRequest>(string webAddress, TRequest request, Action<T, string> saveToFile = null, string fileName = null, string token = null)
       where T : class, new()
       where TRequest : class
   {
      T deserializedResponseData = null;

      try
      {
         // Build URL with query parameters from request object properties
         var uriBuilder = new UriBuilder(webAddress);
         var query = HttpUtility.ParseQueryString(string.Empty);

         // Use reflection to get all properties with JsonPropertyName attribute
         var properties = typeof(TRequest).GetProperties()
             .Where(p => p.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false).Any());

         foreach(var prop in properties)
         {
            var value = prop.GetValue(request);
            if(value != null)
            {
               var jsonAttr = (JsonPropertyNameAttribute)prop.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false).First();
               string paramName = jsonAttr.Name;

               // Handle different types of values using traditional if-else
               string paramValue;
               if(value is DateTime dateTime)
               {
                  paramValue = dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss");
               }
               else if(prop.PropertyType == typeof(DateTime?))
               {
                  var nullableDateTime = (DateTime?)value;
                  paramValue = nullableDateTime?.ToString("yyyy-MM-dd'T'HH:mm:ss");
               }
               else
               {
                  paramValue = value.ToString();
               }

               query[paramName] = paramValue;
            }
         }

         uriBuilder.Query = query.ToString();
         string urlWithParams = uriBuilder.ToString();

         // Use the new overload that accepts a token
         HttpWebResponse httpResponse = Vv_GETmethod_SendHttpWebRequest_GetHttpWebResponse(urlWithParams, token);

         using(StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
         {
            string responseJson = streamReader.ReadToEnd();

            if(responseJson.NotEmpty())
            {
               try
               {
                  deserializedResponseData = JsonConvert.DeserializeObject<T>(responseJson);
                  saveToFile?.Invoke(deserializedResponseData, fileName);
               }
               catch(Exception ex2)
               {
                  // Fallback error handling for deserialization
                  deserializedResponseData = new T();
                  var jsonMsg = new List<string>();
                  int jsonMsgRowIdx = 0;

                  JsonTextReader reader = new JsonTextReader(new StringReader(responseJson));
                  while(reader.Read())
                  {
                     if(reader.Value != null)
                     {
                        jsonMsgRowIdx++;
                        jsonMsg.Add(reader.Value.ToString());
                     }
                  }
                  ZXC.aim_emsg_List("Exception: " + ex2.Message, jsonMsg);
               }
            }
            else
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "JSON response is empty!");
               deserializedResponseData = new T();
            }
         }
      }
      catch(WebException ex)
      {
         ZXC.aim_emsg(ex.Message);
      }

      return deserializedResponseData;
   }

   #endregion Private common methods - Voila!

   #region Utils
   private static JsonSerializerSettings VvMER_JsonSerializerSettings_Default()
   {
      return new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };
   }
   private static string VvMER_Json_SerializeObjectForRequestString_AllActions(VvMER_RequestData json_AllActions_Request_Data)
   {
      return JsonConvert.SerializeObject(json_AllActions_Request_Data, Newtonsoft.Json.Formatting.Indented, VvMER_JsonSerializerSettings_Default());
   }
   public static void InitProjectData()
   {
      VvMER_UserName  = DEMO ? "8633"    : ZXC.CURR_prjkt_rec.F2_UserName.NotEmpty()          ? ZXC.CURR_prjkt_rec.F2_UserName          : ZXC.CURR_prjkt_rec.SkyVvDomena         ;
      VvMER_Password  = DEMO ? "T22zsEY" : ZXC.CURR_prjkt_rec.F2_PasswordDecrypted.NotEmpty() ? ZXC.CURR_prjkt_rec.F2_PasswordDecrypted : ZXC.CURR_prjkt_rec.SkyPasswordDecrypted;
      VvMER_CompanyId = ZXC.CURR_prjkt_rec.Oib                                                                                                                                   ;
   }

   /// <summary>
   /// Gets the date range for querying F2 documents with one week buffer on both ends.
   /// This ensures we capture all relevant documents within the project year plus buffer period.
   /// MER service automatically applies its own limits (60 days back, 10,000 document limit).
   /// </summary>
   /// <returns>Tuple containing (dateFrom, dateTo) with one week buffer on each side of project year</returns>
   private static (DateTime dateOD, DateTime dateDO) GetF2QueryDateRange()
   {
      DateTime dateOD = ZXC.projectYearFirstDay - ZXC.OneWeekSpan;
      DateTime dateDO = ZXC.projectYearLastDay  + ZXC.OneWeekSpan;

      return (dateOD, dateDO);
   }
   public static bool Is_FIR_ON()
   {
      //get
      {
         if(!ZXC.IsF2_2026_rules)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "Ova funkcionalnost je dostupna samo u F2 2026 projektima.");
            return false;
         }

         if(ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_FUR_Only)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "Ova funkcionalnost je nedostupna ulozi projekta 'VlastitoKnjigovodstvo_F2_FUR_Only'.");
            return false;
         }

         return true;
      }
   }
   public static bool Is_FIR_SEND_ON()
   {
      //get
      {
         if(Is_FIR_ON() == false) return false;

         if(ZXC.CURR_prjkt_rec.F2_RolaKind != ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_ALL)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "Ova funkcionalnost je dostupna samo ulozi projekta 'VlastitoKnjigovodstvo_F2_ALL'.");
            return false;
         }

         return true;
      }
   }
   public static bool Is_FUR_ON()
   {
      //get
      {
         if(!ZXC.IsF2_2026_rules)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "Ova funkcionalnost je dostupna samo u F2 2026 projektima.");
            return false;
         }

         return true;
      }
   }
   public static string ExtractEmbeddedPdfsFromXmlFileOLD(string xmlFilePath)
   {
      string pdfFilePath = "";

      try
      {
         // Validate input
         if(string.IsNullOrEmpty(xmlFilePath) || !File.Exists(xmlFilePath))
         {
            throw new FileNotFoundException($"XML file not found: {xmlFilePath}");
         }

         // Load and deserialize the XML
         string xmlString = File.ReadAllText(xmlFilePath);
         EN16931.UBL.InvoiceType the_eRacun = EN16931.UBL.InvoiceType.Deserialize(xmlString);

         // Discover PDF documents using the same pattern as your existing code
         var pdfDocs = the_eRacun.AdditionalDocumentReference?
             .Where(docRef =>
                 docRef?.Attachment?.EmbeddedDocumentBinaryObject != null &&
                 docRef.Attachment.EmbeddedDocumentBinaryObject.mimeCode == "application/pdf")
             .ToList();

         if(pdfDocs != null)
         {
            // Get XML file directory and base name for PDF files
            string xmlDirectory = Path.GetDirectoryName(xmlFilePath);
            string xmlFileNameWithoutExtension = Path.GetFileNameWithoutExtension(xmlFilePath);

            foreach(var docRef in pdfDocs)
            {
               var filename = docRef.Attachment.EmbeddedDocumentBinaryObject.filename;
               if(string.IsNullOrEmpty(filename))
                  filename = "output.pdf";
               else
                  filename = System.IO.Path.GetFileName(filename); // Extract filename only from full path

               var pdfBytes = docRef.Attachment.EmbeddedDocumentBinaryObject.Value;

               // Create PDF file path (same directory as XML, using XML base name + PDF filename)
               pdfFilePath = Path.Combine(xmlDirectory, $"{xmlFileNameWithoutExtension}_{filename}");

               // Write PDF file
               File.WriteAllBytes(pdfFilePath, pdfBytes);

               Console.WriteLine($"PDF extracted successfully to: {pdfFilePath}");
            }
         }
         else
         {
            Console.WriteLine("No embedded PDFs found in the XML file.");
         }
      }
      catch(Exception ex)
      {
         //throw new Exception($"Error extracting PDFs from XML: {ex.Message}", ex);
         ZXC.aim_emsg(MessageBoxIcon.Error, "Xml datoteka ne izgleda kao eRačun.");
         pdfFilePath = "";
      }

      return pdfFilePath;
   }
   public static List<string> ExtractEmbeddedPdfsFromXmlFile(string xmlFilePath)
   {
      List<string> extractedPdfPaths = new List<string>();

      try
      {
         // Validate input
         if(string.IsNullOrEmpty(xmlFilePath) || !File.Exists(xmlFilePath))
         {
            throw new FileNotFoundException($"XML file not found: {xmlFilePath}");
         }

         // Load XML using XDocument for better namespace handling
         string xmlString = File.ReadAllText(xmlFilePath);
         XDocument xmlDoc = XDocument.Parse(xmlString);

         // Define namespaces used in UBL documents
         XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
         XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";

         // Find all AdditionalDocumentReference elements with embedded PDFs
         var pdfElements = xmlDoc.Descendants(cac + "AdditionalDocumentReference")
            .Where(docRef =>
            {
               var attachment = docRef.Element(cac + "Attachment");
               var embeddedObj = attachment?.Element(cbc + "EmbeddedDocumentBinaryObject");
               return embeddedObj?.Attribute("mimeCode")?.Value == "application/pdf";
            });

         if(pdfElements.Any())
         {
            // Get XML file directory and base name for PDF files
            string xmlDirectory = Path.GetDirectoryName(xmlFilePath);
            string xmlFileNameWithoutExtension = Path.GetFileNameWithoutExtension(xmlFilePath);

            int pdfCounter = 0;

            foreach(var docRef in pdfElements)
            {
               var attachment = docRef.Element(cac + "Attachment");
               var embeddedObj = attachment?.Element(cbc + "EmbeddedDocumentBinaryObject");

               if(embeddedObj != null)
               {
                  string filename = embeddedObj.Attribute("filename")?.Value ?? "output.pdf";
                  filename = System.IO.Path.GetFileName(filename); // Extract filename only from full path

                  string base64Content = embeddedObj.Value?.Trim();
                  if(!string.IsNullOrEmpty(base64Content))
                  {
                     byte[] pdfBytes = Convert.FromBase64String(base64Content);

                     // Create unique PDF file path (same directory as XML, using XML base name + counter + PDF filename)
                     string pdfFilePath;
                     if(pdfCounter == 0)
                     {
                        pdfFilePath = Path.Combine(xmlDirectory, $"{xmlFileNameWithoutExtension}_{filename}");
                     }
                     else
                     {
                        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(filename);
                        string fileExt = Path.GetExtension(filename);
                        pdfFilePath = Path.Combine(xmlDirectory, $"{xmlFileNameWithoutExtension}_{fileNameWithoutExt}_{pdfCounter}{fileExt}");
                     }

                     // Write PDF file
                     File.WriteAllBytes(pdfFilePath, pdfBytes);

                     Console.WriteLine($"PDF extracted successfully to: {pdfFilePath}");

                     extractedPdfPaths.Add(pdfFilePath);
                     pdfCounter++;
                  }
               }
            }
         }
         else
         {
            Console.WriteLine("No embedded PDFs found in the XML file.");
         }
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Xml datoteka ne izgleda kao eRačun.");
      }

      return extractedPdfPaths;
   }
   internal static string Get_MER_TransportStatus_Safe(int f2_StatusCD)
   {
      try
      {
         if(Vv_eRacun_HTTP.MER_TransportStatuses == null) return "";

         if(Vv_eRacun_HTTP.MER_TransportStatuses.TryGetValue(f2_StatusCD, out string statusText))
         {
            return statusText ?? "";
         }

         return "";
      }
      catch(KeyNotFoundException ex)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning,
            "Nepoznat F2 transportni status (StatusCD={0}).\n\r\n\rDetalj: {1}",
            f2_StatusCD,
            ex.Message);

         return "";
      }
   }

   /// <summary>
   /// Gets the Faktur record associated with the payment Ftrans, handling both current and previous year records.
   /// </summary>
   /// <param name="paymentftrans_rec">The payment Ftrans record</param>
   /// <param name="dbConnection">Database connection for current year</param>
   /// <param name="paymentMethod">Output parameter for the payment method (T, O, or Z)</param>
   /// <returns>The associated Faktur record, or null if not found or invalid</returns>
   internal static Faktur GetFakturFromPaymentFtrans(Ftrans paymentftrans_rec, XSqlConnection dbConnection, out string paymentMethod)
   {
      paymentMethod = "T"; // Default payment method

      Faktur MAP_CandidateFaktur_rec = new Faktur();

      if(paymentftrans_rec.T_fakRecID.NotZero())
      {
         if(paymentftrans_rec.T_fakYear.IsZero())
         {
            //ZXC.aim_emsg(MessageBoxIcon.Error, $"{paymentftrans_rec.ToShortString()}{Environment.NewLine}T_fakYear IsZero!!!");
            return null;
         }

         if(paymentftrans_rec.T_fakYear == ZXC.projectYearAsInt) // uplata se odnosi na Faktur iz ove godine 
         {
            MAP_CandidateFaktur_rec.VvDao.SetMe_Record_byRecID_Complete(dbConnection, paymentftrans_rec.T_fakRecID, MAP_CandidateFaktur_rec);
         }
         else // uplata se odnosi na Faktur iz neke PROŠLE godine 
         {
            MAP_CandidateFaktur_rec.VvDao.SetMe_Record_byRecID_Complete(/*conn*/ZXC.TheSecondDbConn_SameDB_OtherYear((int)paymentftrans_rec.T_fakYear), paymentftrans_rec.T_fakRecID, MAP_CandidateFaktur_rec);
         }

         paymentMethod = paymentftrans_rec.T_TT == Nalog.IZ_TT ? "T" : paymentftrans_rec.T_TT == Nalog.KP_TT ? "O" : "Z"; //16.12.2025.

         // trik, da paymentMethod pospremimo u faktur za kasnije 
         MAP_CandidateFaktur_rec.CjenikTT = paymentMethod;
      }
      else
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, $"{paymentftrans_rec.ToShortString()}{Environment.NewLine}T_fakRecID IsZero!!!");

         MAP_CandidateFaktur_rec = null;
         // TODO: ako ispadne da je T_fakRecID prazan, ovdje treba potražiti fakturu preko T_tipBr-a ... ili kojiK ... npr ako je R1/R2 'po naplati' 
      }

      return MAP_CandidateFaktur_rec;
   }

   #endregion Utils

   #region Concrete API / EndPoint methods implementations - 'ZEBRA'

   //######################## https://www.moj-eracun.hr/apis/v2/send #############################################################################################################
   public static WebApiResult<VvMER_ResponseData> /*VvMER_Response_Data_AllActions*/ VvMER_WebService_SEND(string xmlString)
   {
      // Web adresa Vam je ispravna za demo okruženje: https://demo.moj-eracun.hr/apis/v2/send
      // Produkcijska adresa je : https://www.moj-eracun.hr/apis/v2/send

      // DEMO old             
      // ID: 8633             
      // Lozinka: T22zsEY     
      // SoftwareID: Test-001 


      //int    username   = 6072         ;
      //string password   = "buV733eX"   ;
      //int    username   = 8633         ;
      //string password   = "T22zsEY"    ;
      //string companyId  = "60042587515";
      //string companyId  = "04192765979";
      //string softwareId = "Test-001"   ;

      string        webApiAddr = VvMER_webAddressPOST_Send;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.SEND       ;

      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData(xmlString);

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<VvMER_ResponseData> webApiResult = 
         
         Vv_POSTmethod_ExecuteJson<VvMER_ResponseData>
         (
            webApiKind,
            webApiAddr,
            jsonRequestString//,
            //(data, fileName) => { if(fileName.NotEmpty()) data.SaveToFile(fileName); },
            //fullPath_XML_FileName.Replace(".xml", "_RES.xml")
         );

      return webApiResult;
   }
   public  static WebApiResult<VvMER_ResponseData> VvPND_WebService_SEND(string xmlString)
   {
      string        webApiAddr = VvPND_webAddressPOST_Send;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.SEND       ;

      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData(xmlString);

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<VvMER_ResponseData> webApiResult =

         Vv_POSTmethod_ExecuteJson<VvMER_ResponseData>
         (
            webApiKind,
            webApiAddr,
            jsonRequestString,
            //(data, fileName) => { if(fileName.NotEmpty()) data.SaveToFile(fileName); },
            //fullPath_XML_FileName.Replace(".xml", "_RES.xml"),
            VvPND_API_Key
         );

      return webApiResult;
   }

   //######################## F2_eIzvj API #######################################################################################################################################
   public  static WebApiResult<VvMER_ResponseData> /*VvMER_Response_Data_AllActions*/ VvMER_WebService_eIzvj(string xmlString, DateTime _DeliveryDate, bool _IsCopy, string _InvoiceType)
   {
      string        webApiAddr = VvMER_webAddressPOST_eIzvj;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.eIzvj       ;

      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData()
      {
         xmlInvoice   = xmlString,
         DeliveryDate = _DeliveryDate,
         IsCopy       = _IsCopy,
         InvoiceType  = _InvoiceType
      };

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<VvMER_ResponseData> webApiResult =

         Vv_POSTmethod_ExecuteJson<VvMER_ResponseData>
         (
            webApiKind,
            webApiAddr,
            jsonRequestString
         );

      return webApiResult;
   }
   public  static WebApiResult<VvMER_ResponseData> VvPND_WebService_eIzvj(string xmlString, string _InvoiceType)
   {
      string        webApiAddr = VvPND_webAddressPOST_eIzvj;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.eIzvj;

      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData()
      {
         document   = xmlString,
         type       = _InvoiceType
      };

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<VvMER_ResponseData> webApiResult =

         Vv_POSTmethod_ExecuteJson<VvMER_ResponseData>
         (
            webApiKind,
            webApiAddr,
            jsonRequestString,
            VvPND_API_Key
         );

      return webApiResult;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/queryOutbox - one single TRN status ##############################################################################
   public static WebApiResult<List<VvMER_ResponseData>> VvMER_WebService_QueryOutbox_TRN_Single(uint electronicID)
   {
      string        webApiAddr = VvMER_webAddressPOST_QueryOutbox;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.OutboxTRNstatus   ;

      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData(electronicID); // constructor za STATUS jednog racuna (electronicID-a) 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<List<VvMER_ResponseData>> webApiResult = Vv_POSTmethod_ExecuteJson<List<VvMER_ResponseData>>(webApiKind, webApiAddr, jsonRequestString);

      return webApiResult;
   }
   public static VvMER_ResponseData VvPND_WebService_QueryOutbox_TRNandDPS_Single(uint electronicID)
   {
      string        webApiAddr = VvPND_webAddressGET_QueryOutbox;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.OutboxTRNstatus  ;

      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData(electronicID); // constructor za STATUS jednog racuna (electronicID-a) 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      VvMER_ResponseData responseData_AllActions =
          Vv_GETmethod_ExecuteJson<VvMER_ResponseData, VvMER_RequestData>
          (
              //webApiKind,
              webApiAddr,
              request_Data_AllActions,
              null,
              null,
              VvPND_API_Key
          );

      return responseData_AllActions;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/queryDocumentProcessStatusOutbox - one single DPS status #########################################################
   public static WebApiResult<List<WebApiResult<VvMER_ResponseData>>> VvMER_WebService_QueryOutbox_DPS_Single(uint electronicID)
   {
      string        webApiAddr = VvMER_webAddressPOST_QueryOutbox_DPS;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.OutboxDPSstatus       ;

      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData(electronicID); // constructor za STATUS jednog racuna (electronicID-a) 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<List<WebApiResult<VvMER_ResponseData>>> webApiResult = Vv_POSTmethod_ExecuteJson<List<WebApiResult<VvMER_ResponseData>>>(webApiKind, webApiAddr, jsonRequestString);

      return webApiResult;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/queryOutbox - TRN Status List ####################################################################################
   public static WebApiResult<List<VvMER_ResponseData>> VvMER_WebService_QueryOutbox_TRN_List(DateTime dateOD, DateTime dateDO)
   {
      string        webApiAddr = VvMER_webAddressPOST_QueryOutbox ;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.OutboxTRNstatusList;

      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData(dateOD, dateDO); // constructor za Listu STATUSa racuna (dateOD, dateDO) 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<List<VvMER_ResponseData>> webApiResultWithList = Vv_POSTmethod_ExecuteJson<List<VvMER_ResponseData>>(webApiKind, webApiAddr, jsonRequestString);

      // Filter out response data where Issued year doesn't match project year                                        
      // !!! TODO !!! provjeriti empirijski kakvi su zaista podaci iz MER-a kroz prijelazne periode godina            
      // tj. koji datumi konkretno dolaze u:                                                                          
      // “Created“  : “2016 - 04 - 18T08: 13:03.177”,                                                                 
      // “Updated“  : “2016 - 04 - 18T08: 13:03.177”,                                                                 
      // “Sent“     : “2016 - 04 - 18T08: 13:03.177”,                                                                 
      // “Delivered“: null                                                                                            
      // “Issued“   : “2016 - 04 - 18T00: 00:00”,                                                                     
      // Za sada se oslanjamo na “Issued“ datum jer je to datum samog računa.                                         
      // Ono što želimo postići je da u response'u ostavimo samo one račune kojima je DokDate.Year == ZXC.projectYear 
      if(webApiResultWithList != null && webApiResultWithList.ResponseData != null)
      {
         webApiResultWithList.ResponseData = webApiResultWithList.ResponseData
            .Where(rd => rd.Issued.HasValue && rd.Issued.Value.Year == ZXC.projectYearAsInt)
            .ToList();
      }

      return webApiResultWithList;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/queryDocumentProcessStatusOutbox - DPS Status List ###############################################################
   public static WebApiResult<List<VvMER_ResponseData>> VvMER_WebService_QueryOutbox_DPS_List(DateTime dateOD, DateTime dateDO)
   {
      string        webApiAddr = VvMER_webAddressPOST_QueryOutbox_DPS;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.OutboxDPSstatusList   ;

      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData(dateOD, dateDO); // constructor za Listu STATUSa racuna (dateOD, dateDO) 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<List<VvMER_ResponseData>> webApiResult = Vv_POSTmethod_ExecuteJson<List<VvMER_ResponseData>>(webApiKind, webApiAddr, jsonRequestString);

      return webApiResult;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/queryInbox - Status List #########################################################################################
   public static WebApiResult<List<VvMER_ResponseData>> VvMER_WebService_QueryInbox_List(DateTime dateOD, DateTime dateDO)
   {
      string        webApiAddr = VvMER_webAddressPOST_QueryInbox;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.InboxTRNstatusList;

      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData(dateOD, dateDO); // constructor za Listu STATUSa racuna (dateOD, dateDO) 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<List<VvMER_ResponseData>> webApiResultWithList = Vv_POSTmethod_ExecuteJson<List<VvMER_ResponseData>>(webApiKind, webApiAddr, jsonRequestString);

      // Filter out response data where Issued year doesn't match project year                                        
      // !!! TODO !!! provjeriti empirijski kakvi su zaista podaci iz MER-a kroz prijelazne periode godina            
      // tj. koji datumi konkretno dolaze u:                                                                          
      //“Updated“  : “2016 - 04 - 18T08: 17:22.937”,
      //“Sent“     : “2016 - 04 - 18T08: 17:22.937”,
      //“Delivered“: “2016 - 04 - 19T08: 17:00”,      // Za sada se u Outboxu-u oslanjamo na “Issued“ datum jer je to datum samog računa.                                         
      // ALI buduci da Inbox ga nema, moramo se osloniti na “Sent“ datum.
      // Ono što želimo postići je da u response'u ostavimo samo one račune kojima je DokDate.Year == ZXC.projectYear 
      if(webApiResultWithList != null && webApiResultWithList.ResponseData != null)
      {
         webApiResultWithList.ResponseData = webApiResultWithList.ResponseData
            .Where(rd => rd./*Issued*/Sent.HasValue && rd./*Issued*/Sent.Value.Year == ZXC.projectYearAsInt)
            .ToList();
      }

      return webApiResultWithList;
   }

   //######################## https://www.moj-eracun.hr/api/fiscalization/status - Get 3 kind FISK status ########################################################################
   public static WebApiResult<VvMER_ResponseData> VvMER_WebService_Get_FISK_Status(uint electronicID, string messageType)
   {
      string        webApiAddr = VvMER_webAddressPOST_FiskStatus;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.FISK_singleStatus;

      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData(electronicID) { MessageType = messageType }; 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<VvMER_ResponseData> webApiResult = Vv_POSTmethod_ExecuteJson<VvMER_ResponseData>(webApiKind, webApiAddr, jsonRequestString);

      return webApiResult;
   }

   public static WebApiResult<List<VvMER_Response_Data_FiscalizationStatus>> VvMER_WebService_Get_FISK_Status_Outbox(DateTime dateOD, DateTime dateDO)
   {
      string        webApiAddr = VvMER_webAddressPOST_FiskStatusOutbox;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.FISKstatusOutbox;

      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData(dateOD, dateDO); // constructor za Listu Fisk STATUSa racuna (dateOD, dateDO) 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<List<VvMER_Response_Data_FiscalizationStatus>> webApiResult =
         Vv_POSTmethod_ExecuteJson<List<VvMER_Response_Data_FiscalizationStatus>>(
            webApiKind,
            webApiAddr,
            jsonRequestString
         );
      return webApiResult;
   }

   public static WebApiResult<List<VvMER_Response_Data_FiscalizationStatus>> VvMER_WebService_Get_FISK_Status_Inbox(DateTime dateOD, DateTime dateDO)
   {
      string        webApiAddr = VvMER_webAddressPOST_FiskStatusInbox;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.FISKstatusInbox;

      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData(dateOD, dateDO); // constructor za Listu Fisk STATUSa racuna (dateOD, dateDO) 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<List<VvMER_Response_Data_FiscalizationStatus>> webApiResult =
         Vv_POSTmethod_ExecuteJson<List<VvMER_Response_Data_FiscalizationStatus>>(
            webApiKind,
            webApiAddr,
            jsonRequestString
         );
      return webApiResult;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/receive - one single document ####################################################################################
   public static WebApiResult<VvMER_ResponseData> VvMER_WebService_Receive_XML(uint electronicID)
   {
      string webApiAddr = VvMER_webAddressPOST_Receive;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.RECEIVEdocument;

      WebApiResult<VvMER_ResponseData> webApiResult = new WebApiResult<VvMER_ResponseData>();

      webApiResult.WebApiKind = webApiKind;
      webApiResult.WebApiAddr = webApiAddr;

      try
      {
         VvMER_RequestData request_Data_AllActions = new VvMER_RequestData(electronicID); // constructor za RECEIVE jednog racuna (electronicID-a) 

         string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

         HttpWebResponse httpResponse = Vv_POSTmethod_SendHttpWebRequest_GetHttpWebResponse(webApiAddr, jsonRequestString);

         webApiResult.StatusCode = (int)httpResponse.StatusCode;
         webApiResult.StatusDescription = httpResponse.StatusDescription;

       //using(StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream(), ZXC.VvUTF8Encoding_noBOM, true))
         using(StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8, detectEncodingFromByteOrderMarks: false))
         {
            string responseString = streamReader.ReadToEnd();

            webApiResult.ResponseString = responseString;

            // Check if the response is an error message (typically JSON) or valid XML
            bool isXml  = responseString.TrimStart().StartsWith("<");
            bool isJson = responseString.TrimStart().StartsWith("{") || responseString.TrimStart().StartsWith("[");

            if(responseString.NotEmpty() && isXml)
            {
               webApiResult.ResponseData = new VvMER_ResponseData();
               webApiResult.ResponseData.DocumentXml = responseString;
            }
            else
            {
               webApiResult.ErrorBody = "XML response is empty or isn't XML!";
               webApiResult.ResponseData = new VvMER_ResponseData();
            }
         }
      }
      catch(WebException ex)
      {
         if(ex.Response is HttpWebResponse resp)
         {
            webApiResult.StatusCode = (int)resp.StatusCode;
            webApiResult.StatusDescription = resp.StatusDescription;

            try
            {
               using(var sr = new StreamReader(resp.GetResponseStream()))
               {
                  webApiResult.ErrorBody = sr.ReadToEnd();
               }
            }
            catch { /* ignore */ }
         }
         webApiResult.ExceptionMessage = ex.Message;
         webApiResult.ResponseData = new VvMER_ResponseData();
      }
      catch(Exception ex)
      {
         webApiResult.ExceptionMessage = ex.Message;
         webApiResult.ResponseData = new VvMER_ResponseData();
      }

      return webApiResult;
   }
   //######################## https://www.moj-eracun.hr/apis/v2/Ping - Checks if service is up ###################################################################################
   public static VvMER_ResponseData VvMER_WebService_Ping()
   {
      string        webApiAddr = VvMER_webAddressGET_Ping;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.PING      ;

      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData();
   
      VvMER_ResponseData responseData_AllActions = Vv_GETmethod_ExecuteJson<VvMER_ResponseData, VvMER_RequestData>(/*webApiKind,*/ webApiAddr, request_Data_AllActions);
   
      return responseData_AllActions;
   }
   public static VvMER_ResponseData VvPND_WebService_Ping()
   {
      string        webApiAddr = VvPND_webAddressGET_Ping;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.PING      ; // FUSE 

      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData();

      VvMER_ResponseData responseData_AllActions =
          Vv_GETmethod_ExecuteJson<VvMER_ResponseData, VvMER_RequestData>
          (
            //webApiKind,
              webApiAddr,
              request_Data_AllActions,
              null,
              null,
              VvPND_API_Key 
          );

      return responseData_AllActions;
   }

   //######################## https://www.moj-eracun.hr/api/mps/check - Check Identifier #########################################################################################
   public static WebApiResult<VvMER_ResponseData> VvMER_WebService_CheckAMS(string _Identifiervalue)
   {
      string        webApiAddr = VvMER_webAddressPOST_Check;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.CheckAMS    ;


      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData() 
      { 
         IdentifierValue = _Identifiervalue,
         IdentifierType  = /*0*/"0",
      };

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<VvMER_ResponseData> webApiResult = Vv_POSTmethod_ExecuteJson<VvMER_ResponseData>(webApiKind, webApiAddr, jsonRequestString);

      return webApiResult;
   }
   public static WebApiResult<VvMER_ResponseData> VvPND_WebService_CheckAMS(string _identifier)
   {
      string        webApiAddr = VvPND_webAddressPOST_Check;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.CheckAMS    ;

      VvMER_RequestData request_Data_AllActions = new VvMER_RequestData() 
      { 
         identifier = _identifier,
         schema     = "9934",
      };

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<VvMER_ResponseData> webApiResult = Vv_POSTmethod_ExecuteJson<VvMER_ResponseData>(webApiKind, webApiAddr, jsonRequestString, VvPND_API_Key);

      return webApiResult;
   }

   //######################## https://www.moj-eracun.hr/api/fiscalization/markPaid - Mark Paid action ############################################################################
   public static WebApiResult<VvMER_ResponseData> VvMER_WebService_MAP(VvMER_RequestData request_Data_AllActions)
   {
      string        webApiAddr = VvMER_webAddressPOST_MAP;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.MAPaction;

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<VvMER_ResponseData> webApiResult = Vv_POSTmethod_ExecuteJson<VvMER_ResponseData>(webApiKind, webApiAddr, jsonRequestString);

      return webApiResult;
   }
   public static WebApiResult<VvMER_ResponseData> VvMER_WebService_MAP_WO_eID(VvMER_RequestData request_Data_AllActions)
   {
      string        webApiAddr = VvMER_webAddressPOST_MAP_WO_eID;
      ZXC.F2_WebApi webApiKind = ZXC.F2_WebApi.MAPaction_WO_eID;

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<VvMER_ResponseData> webApiResult = Vv_POSTmethod_ExecuteJson<VvMER_ResponseData>(webApiKind, webApiAddr, jsonRequestString);

      return webApiResult;
   }


   // ########################################################################################################################################################################
   //                                                                                                                                                                         
   //  Kako dodavati nove API-je:                                                                                                                                             
   //                                                                                                                                                                         
   //  1. Da li je GET ili POST                                                                                                                                               
   //  2. Da li vraća 1 responseData_AllActions ili responseData_AllActions_List                                                                                              
   //  3. VvMER_webAddress_XYZ                                                                                                                                                
   //  4. Constructor od VvMER_Request_Data_AllActions() prilagođen zahtjevima dotičnog API-ja                                                                                
   //                                                                                                                                                                         
   // ########################################################################################################################################################################



   #endregion Concrete API / EndPoint methods implementations - 'ZEBRA'

   #region FIR / FUR / NIR Load List and SubmodulActions

   #region FIR
   /* AAA */internal static int Load_IRn_FakturList(F2_Izlaz_UC theUC)
   {
      ZXC.SetStatusText("Load_IRn_FakturList");

      theUC.TheFakturList = new List<Faktur>();

      int newsCount = 0;

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>();

      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt]       , "theTT" , ZXC.RRD.Dsc_F2_TT      , " = " ));
    //filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate]  , "dateOD", ZXC.projectYearFirstDay, " >= "));
    //filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate]  , "dateDO", ZXC.projectYearLastDay , " <= "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.f2_R1kind], "R1Kind", ZXC.F2_R1enum.B2B      , " = " ));

      string asdDscStr;
    //string limitStr = "LIMIT " + (ZXC.RRD.Dsc_F2_NumOfRows.IsPositive() ? ZXC.RRD.Dsc_F2_NumOfRows.ToString() : "100");
      string limitStr = "LIMIT " +                                                                                "5000";

      if(ZXC.RRD.Dsc_F2_IsAsc == false) asdDscStr = " DESC ";
      else                              asdDscStr = " ASC " ;

      bool dbOK = VvDaoBase.LoadGenericVvDataRecordList<Faktur>(theUC.TheDbConnection, theUC.TheFakturList, filterMembers, "", "ttNum " + asdDscStr + limitStr, true);

      //if(dbOK == false)
      //{
      //   ZXC.aim_emsg(MessageBoxIcon.Stop, "Inicijalizirajte tablicu tako da otvorite ekran IFA.");
      //   return -1;
      //}

      theUC.TheFakturList.RemoveAll(fak => fak.IsF2 == false);

      if(theUC.TheFakturList.NotEmpty()) theUC.PutDgvFields();

      ZXC.SetStatusText("");

      return newsCount;
   }
   /* XXX */internal static int WS_Import_Extern_Faktur_IFA/*WS_Ufati_Veleform_Ritam*/(F2_Izlaz_UC theUC)
   {
      #region Init

      Cursor.Current = Cursors.WaitCursor;

      ZXC.SetStatusText("Sinhroniziram klijentove izlazne račune");

      int newsCount = 0;

      (DateTime queryOutbox_DateOD, DateTime queryOutbox_DateDO) = GetF2QueryDateRange();

      bool isNewFaktur, addrecOK, kupdobOK, xmlValidationOK;

      string theXmlString = "", theOIB = "";

      Faktur existingFaktur_byElectronicID, existingFaktur_byTtNumFiskal, newIFA_Faktur_rec = null;

      EN16931.UBL.InvoiceType    deserialized_InvoiceType    = null;
      EN16931.UBL.CreditNoteType deserialized_CreditNoteType = null;

      Kupdob kupdob_rec;

      List<string> updatedStatusInfoList = new List<string>();
           string  updatedStatusInfo                         ;

      List<string> newKupdobInfoList = new List<string>();
           string  newKupdobInfo                         ;

      #endregion Init

      #region Synchronise Servis Faktur DataLayer with Klijent Faktur DataLayer via news from QueryOutbox 

      WebApiResult<List<VvMER_ResponseData>> webApiResultWithList = Vv_eRacun_HTTP.VvMER_WebService_QueryOutbox_TRN_List(queryOutbox_DateOD, queryOutbox_DateDO);

      if(webApiResultWithList == null || webApiResultWithList.ResponseData == null /*|| webApiResultWithList.ResponseData.IsEmpty()*/)
      {
         if(webApiResultWithList == null)
         {
            webApiResultWithList = new WebApiResult<List<VvMER_ResponseData>>()
            {
               WebApiKind        = ZXC.F2_WebApi.OutboxTRNstatusListAsKnjigServis,
               WebApiAddr        = webApiResultWithList.WebApiAddr,
               StatusCode        = -1,
               StatusDescription = "No response data",
               ErrorBody         = "No response data"
            };
         }
         else
         {
            if(webApiResultWithList.ResponseData != null && webApiResultWithList.ResponseData.IsEmpty())
            {
               webApiResultWithList.ErrorBody = "Lista je prazna";
            }
         }

         //Show_WebApiResult_ErrorMessageBox(webApiResultWithList);
         Show_WebApiResult_ErrorMessageBox<VvMER_ResponseData>(webApiResultWithList);

         return 0;
      }

      List<VvMER_ResponseData> loopList = webApiResultWithList.ResponseData.OrderBy(rd => rd.Created).ToList();
      
      foreach(VvMER_ResponseData responseData in loopList)
      {
         Cursor.Current = Cursors.WaitCursor;

         existingFaktur_byElectronicID = theUC.TheFakturList./*Single*/FirstOrDefault(f => f.F2_ElectronicID == responseData.ElectronicId);
         existingFaktur_byTtNumFiskal = theUC.TheFakturList./*Single*/FirstOrDefault(f => f./*TtNumFiskal*/VezniDok == responseData.DocumentNr);

         //isNewFaktur = existingFaktur_byElectronicID == null                                        ; 
         isNewFaktur = existingFaktur_byElectronicID == null && existingFaktur_byTtNumFiskal == null;

         if(isNewFaktur == false) continue;

         // here we go 

         #region 1. Call RECEIVE to get full XML document

         WebApiResult<VvMER_ResponseData> webApiResult = null;

         bool receiveOK = true;

         switch(ZXC.F2_TheProvider)
         {
            case ZXC.F2_Provider_enum.MER:
               {
                  try
                  {
                     webApiResult = Vv_eRacun_HTTP.VvMER_WebService_Receive_XML((uint)responseData.ElectronicId);

                     theXmlString = webApiResult.ResponseData.DocumentXml;

                     if(webApiResult.ResponseData == null || webApiResult.ResponseData.DocumentXml.IsEmpty())
                     {
                        Show_WebApiResult_ErrorMessageBox(webApiResult);
                        receiveOK = false;
                     }
                  }
                  catch(Exception ex)
                  {
                     ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom slanja na WebServis: {0}", ex.Message);
                     receiveOK = false;
                  }

                  break;

               } // case ZXC.F2_Provider_enum.MER: 

            case ZXC.F2_Provider_enum.PND:
               {
                  throw new NotImplementedException("Get_FISK_Status_ForElectronicID: F2 Provider PND not implemented yet.");
                  receiveOK = false;
                  break;
               }
         }

         #endregion 1. Call RECEIVE to get full XML document

         #region 2. Deserialize eRacun XML document into 'InvoiceType' bussiness object & Validate XML against XSD schema

         xmlValidationOK = false;

         bool isCreditNote = receiveOK && IsXmlCreditNoteType(theXmlString);
         bool isInvoice    = receiveOK && IsXmlInvoiceType   (theXmlString);

         if(isCreditNote)
         {
            deserialized_CreditNoteType = receiveOK ? GetCreditNoteTypeByDeserializing_xmlString(theXmlString, false, responseData) : null;
            deserialized_InvoiceType = null;
         }
         else if(isInvoice)
         {
            deserialized_InvoiceType = receiveOK ? GetInvoiceTypeByDeserializing_xmlString(theXmlString, false, responseData) : null;
            deserialized_CreditNoteType = null;
         }
         else
         {
            //ZXC.aim_emsg(MessageBoxIcon.Error, $"Outbox dokument {webApiResult.ResponseData} nije niti račun (InvoiceType) niti odobrenje (CreditNote). Dokument neće biti učitan u IFA-e.");
            ZXC.aim_emsg(MessageBoxIcon.Error,
               $"Outbox dokument nije niti račun (InvoiceType) niti odobrenje (CreditNote).{Environment.NewLine}Dokument neće biti učitan u IFA-e.{Environment.NewLine}{Environment.NewLine}" +
               $"Elektronski ID: {responseData.ElectronicId}{Environment.NewLine}" +
               $"Broj dokumenta: {responseData.DocumentNr ?? "N/A"}{Environment.NewLine}" +
               $"Pošiljatelj: {responseData.SenderBusinessName ?? "N/A"}{Environment.NewLine}" +
               $"Datum slanja: {responseData.Sent?.ToString(ZXC.VvDateFormat) ?? "N/A"}{Environment.NewLine}" +
               $"Status: {responseData.StatusName ?? responseData.StatusId?.ToString() ?? "N/A"}");
         }

         bool hasDeserializedDocument = (deserialized_InvoiceType != null || deserialized_CreditNoteType != null);

         if(hasDeserializedDocument)
         {
            try
            {
               if(!isCreditNote)
                  xmlValidationOK = Vv_XSD_Bussiness_BASE<EN16931.UBL.InvoiceType>.ValidateXmlAgainstXsd(theXmlString);
               else
                  xmlValidationOK = true; // TODO: dodati XSD validaciju za CreditNote ako bude potrebno 
            }
            catch(Exception ex)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, ex.Message);
            }
         }

         #endregion 2. Deserialize eRacun XML document into 'InvoiceType' bussiness object & Validate XML against XSD schema

         #region 3. Create_Faktur_From_eRacun & ADDREC to Vektor DataLayer

         if(receiveOK && hasDeserializedDocument && xmlValidationOK)
         {
            #region Get Kupdob / New Kupdob?

            if(isInvoice) theOIB = deserialized_InvoiceType.VvCustomerOIB;
            else if(isCreditNote) theOIB = deserialized_CreditNoteType.VvCustomerOIB;

            if(theOIB.IsEmpty())
            {
               kupdob_rec = null;
            }
            else
            {
               kupdob_rec = theUC.Get_Kupdob_FromVvUcSifrar_byOIB(theOIB);
            }

            if(kupdob_rec != null) kupdobOK = true;
            else kupdobOK = false;

            if(kupdobOK == false) // try to create NEW Kupdob from eRacun data 
            {
               if(isInvoice)
               {
                  kupdob_rec = EN16931.UBL.InvoiceType.Create_Kupdob_from_InvoiceType(theUC.TheDbConnection, deserialized_InvoiceType, true);
               }
               else if(isCreditNote)
               {
                  kupdob_rec = EN16931.UBL.CreditNoteType.Create_Kupdob_from_CreditNote(theUC.TheDbConnection, deserialized_CreditNoteType, true);
               }

               if(kupdob_rec != null) // NEW Kupdob created ok 
               {
                  addrecOK = kupdob_rec.VvDao.ADDREC(theUC.TheDbConnection, kupdob_rec);

                  if(addrecOK)
                  {
                     newKupdobInfo = string.Format("Novi kupac [{0}],  OIB: [{1}], Ulica: {2}, Mjesto: {3}", kupdob_rec.Naziv, kupdob_rec.Oib, kupdob_rec.Ulica1, kupdob_rec.ZipAndMjesto);

                     newKupdobInfoList.Add(newKupdobInfo);

                     theUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

                     kupdobOK = true;
                  }
                  else
                  {
                     ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom ADDREC novog kupca (kupdob) iz eRačuna s eID={0} za OIB [{1}].", responseData.ElectronicId, theOIB);
                     kupdobOK = false;
                  }
               }
               else
               {
                  ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom kreiranja novog kupca (kupdob) iz eRačuna s eID={0} za OIB [{1}].", responseData.ElectronicId, theOIB);
               }

            } // if(kupdobOK == false) // try to create NEW Kupdob from eRacun data 

            else // kupdobOK == true, provjeri ima li dobar R1_Kind ... i ak nema ...
            {
               if(kupdob_rec.R1kind != ZXC.F2_R1enum.B2B)
               {
                  theUC.TheVvTabPage.TheVvForm.BeginEdit(kupdob_rec);

                  kupdob_rec.R1kind = ZXC.F2_R1enum.B2B;

                  kupdob_rec.VvDao.RWTREC(theUC.TheDbConnection, kupdob_rec, false, true, false);

                  theUC.TheVvTabPage.TheVvForm.EndEdit(kupdob_rec);

                  theUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name); // REFRESH sifrar! 
               }
            }

            #endregion Get Kupdob / New Kupdob?

            // 2. Create new Faktur bussiness object record from 'InvoiceType' OR 'CreditNoteType' in XML document 

            if(isInvoice)
            {
               newIFA_Faktur_rec = deserialized_InvoiceType.Create_Faktur_From_InvoiceType(theUC.TheDbConnection, (uint)responseData.ElectronicId, (DateTime)responseData.Sent, kupdob_rec, true);
            }
            else if(isCreditNote)
            {
               newIFA_Faktur_rec = deserialized_CreditNoteType.Create_Faktur_From_CreditNoteType(theUC.TheDbConnection, (uint)responseData.ElectronicId, (DateTime)responseData.Sent, kupdob_rec, true);
            }

            if(newIFA_Faktur_rec != null)
            {
               theUC.TheFakturList.Add(newIFA_Faktur_rec);

               newsCount++;

               updatedStatusInfo = string.Format("{0} (OrigBrDok: {1}) Nova IFA klijenta je {2} {3} {4}",
                                             newIFA_Faktur_rec.TipBr,
                                             newIFA_Faktur_rec./*F2_ElectronicID*/VezniDok,
                                             "DODANA u lokalnu bazu",
                                             newIFA_Faktur_rec.DokDate.ToString(ZXC.VvDateFormat), newIFA_Faktur_rec.KupdobName);

               updatedStatusInfoList.Add(updatedStatusInfo);

               ZXC.SetStatusText($"{newsCount}. od {loopList.Count}: {updatedStatusInfo}");
            }

         } // if(receiveOK) 

         #endregion 3. Create_Faktur_From_eRacun & ADDREC to Vektor DataLayer

      } // foreach(VvMER_Response_Data_AllActions responseData in webApiResultWithList.ResponseData.OrderBy(rd => rd.Created)) 

      #endregion Synchronise Servis Faktur DataLayer with Klijent Faktur DataLayer via news from QueryOutbox 

      #region TheFakturList ... OrderBy And PutDgvFields

      //if(ZXC.RRD.Dsc_F2_IsAsc == false) theUC.TheFakturList = theUC.TheFakturList.OrderByDescending(f => f.TtNum).ToList();
      //else                              theUC.TheFakturList = theUC.TheFakturList.OrderBy          (f => f.TtNum).ToList();
      //
      //if(theUC.TheFakturList.NotEmpty()) theUC.PutDgvFields();

      #endregion TheFakturList ... OrderBy And PutDgvFields

      #region Finish

      Cursor.Current = Cursors.Default;

      ZXC.SetStatusText("");

      if(updatedStatusInfoList.NotEmpty())
      {
         Load_IRn_FakturList(theUC);

         ZXC.aim_emsg_List(string.Format("DODANO je {0} novih klijentovih računa u Vektorovu bazu podataka.", updatedStatusInfoList.Count), updatedStatusInfoList);
      }

      if(newKupdobInfoList.NotEmpty())
      {
         ZXC.aim_emsg_List(string.Format("DODANO je {0} novih partnera (kupaca) u Vektorovu bazu podataka.", newKupdobInfoList.Count), newKupdobInfoList);
      }

      return newsCount;

      #endregion Finish
   }

   /* BBB */
   internal static int WS_Discover_Candidates_And_Eventually_SEND_eRacune(F2_Izlaz_UC theUC, bool isManual)
   {
      #region Init & Get Dialog Fields

      ZXC.SetStatusText("WS_Discover_Candidates_And_Eventually_SEND_eRacune");

      int newsCount = 0;

      //if(ZXC.RRD.Dsc_F2_IsAutoSend == false) return;

    //List<Faktur> sendCandidatesFakturList = theUC.TheFakturList.Where(fak => fak.IsF2 && fak.F2_ElectronicID.IsZero()).OrderBy(fak => fak.TtNum).ToList();
      List<Faktur> sendCandidatesFakturList = theUC.TheFakturList.Where(fak => fak.IsF2 && fak.F2_IsOKToSend           ).OrderBy(fak => fak.TtNum).ToList();

      if(sendCandidatesFakturList.IsEmpty())
      {
         if(isManual) ZXC.aim_emsg(MessageBoxIcon.Information, "Nema ništa za slanje.");
         return 0; 
      }

      List<VvReportSourceUtil> messageList = new List<VvReportSourceUtil>();

      foreach(Faktur sendCandidateFaktur_rec in sendCandidatesFakturList)
      {
         messageList.Add(new VvReportSourceUtil()
         {
            TheCD      = sendCandidateFaktur_rec.TipBr,
            DevName    = sendCandidateFaktur_rec.DokDate.ToString(ZXC.VvDateFormat),
            KupdobName = sendCandidateFaktur_rec.KupdobName,
            TheMoney   = sendCandidateFaktur_rec.S_ukKCRP,

            UtilUint   = sendCandidateFaktur_rec.RecID
         });
      }

      VvMessageBoxDLG  sendCandidatesFakturList_InfoDLG = new VvMessageBoxDLG (false, ZXC.VvmBoxKind.F2_SEND_candidates);
    //VvMessageBoxForm sendCandidatesFakturList_InfoDLG = new VvMessageBoxForm(false, ZXC.VvmBoxKind.F2_SEND_candidates);
      sendCandidatesFakturList_InfoDLG.Text = "Kandidati za slanje kao eRačun:";

      sendCandidatesFakturList_InfoDLG.TheUC.PutDgvFields_F2_SEND_candidates(messageList);
      sendCandidatesFakturList_InfoDLG.TheUC.Fld_IsAutoSend = ZXC.RRD.Dsc_F2_IsAutoSend;

      DialogResult dlgResult = sendCandidatesFakturList_InfoDLG.ShowDialog();

      if(dlgResult != DialogResult.OK)
      {
         sendCandidatesFakturList_InfoDLG.Dispose();
         return -1;
      }

      if(ZXC.RRD.Dsc_F2_IsAutoSend != sendCandidatesFakturList_InfoDLG.TheUC.Fld_IsAutoSend)
      {
         ZXC.RRD.Dsc_F2_IsAutoSend = sendCandidatesFakturList_InfoDLG.TheUC.Fld_IsAutoSend; 
         ZXC.RRD.SaveDscToLookUpItemList();
      }

      int numOfFirstLinesOnly   =  sendCandidatesFakturList_InfoDLG.TheUC.Fld_NumOfFirstLinesOnly_Send;

      #region Izbaci 'preskočene'

      bool shouldSkip;
      uint fakRecIDtoSkip;
      int foundCount;

      for(int rIdx = 0; rIdx < sendCandidatesFakturList_InfoDLG.TheUC.TheGrid.RowCount /*- 1*/; ++rIdx)
      {
         shouldSkip = sendCandidatesFakturList_InfoDLG.TheUC.TheGrid.GetBoolCell(sendCandidatesFakturList_InfoDLG.TheUC.DgvCI.iT_shouldS, rIdx, false);

         if(shouldSkip)
         {
            fakRecIDtoSkip = sendCandidatesFakturList_InfoDLG.TheUC.TheGrid.GetUint32Cell(sendCandidatesFakturList_InfoDLG.TheUC.DgvCI.iT_ftrRecID, rIdx, false);
            
            foundCount = sendCandidatesFakturList.RemoveAll(scfl => scfl.RecID == fakRecIDtoSkip);

            if(foundCount.IsZero()) ZXC.aim_emsg(MessageBoxIcon.Error, "shouldSkip MAP_action NOT FOUND!");
         }
      }

      #endregion Izbaci 'preskočene'

      sendCandidatesFakturList_InfoDLG.Dispose();

      Cursor.Current = Cursors.WaitCursor;

      int sendCount = 0; bool sendOK;

      Outgoing_eRacun_parameters oeRp;

      ZXC.FakturList_To_PDF_InProgress = true;

      System.Diagnostics.Stopwatch dispatchStopWatch = System.Diagnostics.Stopwatch.StartNew();

      uint soFarCount      = 0;
       int ofTotalCount    = numOfFirstLinesOnly.NotZero() ? numOfFirstLinesOnly : sendCandidatesFakturList.Count;
      long elapsedTicks    = 0, remainTicks;
      decimal soFarKoef       ;
      TimeSpan elapsedTime = new TimeSpan(0);
      TimeSpan remainTime     ;
      string statusText       ;

      #endregion Init & Get Dialog Fields

      #region The Send Loop - foreach Faktur

      foreach(Faktur sendCandidateFaktur_rec in sendCandidatesFakturList)
      {
         Cursor.Current = Cursors.WaitCursor;

         sendCandidateFaktur_rec.VvDao.LoadTranses(theUC.TheDbConnection, sendCandidateFaktur_rec, false);

         ZXC.FakturRec = (Faktur)sendCandidateFaktur_rec.CreateNewRecordAndCloneItComplete();

         oeRp = ZXC.TheVvForm.Set_Outgoing_eRacun_parameters(sendCandidateFaktur_rec, theUC, false, false);
         
         sendOK = ZXC.TheVvForm.RISK_Outgoing_eRacun_JOB(oeRp, false); // VOILA! 
         
         if(sendOK) sendCount++;

         #region set status text

         soFarCount++;

         #region soFar vs remaining calc

         soFarKoef     = ZXC.DivSafe(soFarCount, ofTotalCount);
         elapsedTicks += dispatchStopWatch.Elapsed.Ticks          ;
         elapsedTime  += dispatchStopWatch.Elapsed                ;
         remainTicks   = (long)(ZXC.DivSafe((decimal)elapsedTicks, soFarKoef) - elapsedTicks);
         remainTime    = new TimeSpan(remainTicks);

         #endregion soFar vs remaining calc

         statusText =
            dispatchStopWatch.Elapsed.TotalSeconds.ToString1Vv() + "s " +
            "(" + (elapsedTime.TotalSeconds / (double)soFarCount).ToString1Vv() + "s avg) done " +
             (/*++*/soFarCount).ToString() +
             " of " + ofTotalCount +
             " (" + (soFarKoef * 100M).ToString0Vv() + "%)" +
            //" <"   + remainTime + "> "                              +
             string.Format(" remain <{0:00}:{1:00}:{2:00}> ", remainTime.Hours, remainTime.Minutes, remainTime.Seconds) +
             " " + sendCandidateFaktur_rec.ToString();

         dispatchStopWatch.Restart();

         ZXC.SetStatusText(statusText); Cursor.Current = Cursors.WaitCursor;

         #endregion set status text

         if(numOfFirstLinesOnly.NotZero() && /*sendCount*/soFarCount == numOfFirstLinesOnly) break;

      } // foreach(Faktur sendCandidateFaktur_rec in sendCandidatesFakturList) 

      #endregion The Send Loop - foreach Faktur

      #region Finish

      ZXC.FakturList_To_PDF_InProgress = false;

      ZXC.FakturRec = null;

      ZXC.SetStatusText("");

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Završeno slanje {0} eRačuna.", sendCount);

      return newsCount;

      #endregion Finish
   }
   /* CCC */internal static int WS_Refresh_ALL_FIR_Statuses_AndArhiviraj(F2_Izlaz_UC theUC/*, bool isDPS*/) // VOILA! 
   {
      #region Init

      int newsCount = 0;

      if(theUC.TheFakturList.IsEmpty()) return 0;

      Faktur F2_IRn_faktur_rec;

      List<string> updatedStatusInfoList = new List<string>();
           string  updatedStatusInfo                         ;

      Cursor.Current = Cursors.WaitCursor;

      decimal MAP_Ftr_naplacenoMoney, MAP_Xtr_prijavljenoMoney;
      int     MAP_Ftr_naplacenoCount, MAP_Xtr_prijavljenoCount;
      string  MAP_Ftr_naplacenoStr  , MAP_Xtr_prijavljenoStr  ;

      List<Ftrans> MAP_naplacenoFtransList;
      List<Xtrano> MAP_prijavljenoXtranoList;

      #endregion Init

      #region Refresh TRN status

      ZXC.SetStatusText("Refresh TRN status");

      (DateTime queryOutbox_DateOD, DateTime queryOutbox_DateDO) = GetF2QueryDateRange();

      WebApiResult<List<VvMER_ResponseData>> webApiResultWithList = Vv_eRacun_HTTP.VvMER_WebService_QueryOutbox_TRN_List(queryOutbox_DateOD, queryOutbox_DateDO);

    //if(webApiResultWithList == null || webApiResultWithList.ResponseData == null /*|| webApiResultWithList.ResponseData.IsEmpty()*/)
      if(webApiResultWithList == null || webApiResultWithList.ResponseData == null   || webApiResultWithList.ResponseData.IsEmpty()  )
      {
         if(webApiResultWithList == null)
         {
            webApiResultWithList = new WebApiResult<List<VvMER_ResponseData>>()
            {
               WebApiKind = ZXC.F2_WebApi.InboxTRNstatusList,
               WebApiAddr = webApiResultWithList.WebApiAddr,
               StatusCode = -1,
               StatusDescription = "No response data",
               ErrorBody = "No response data"
            };
         }
         else
         {
            if(webApiResultWithList.ResponseData != null && webApiResultWithList.ResponseData.IsEmpty())
            {
               webApiResultWithList.ErrorBody = "Lista je prazna";
            }
         }

       //Show_WebApiResult_ErrorMessageBox(webApiResultWithList);
         Show_WebApiResult_ErrorMessageBox<VvMER_ResponseData>(webApiResultWithList);

         //return 0;
      }

      // join na ElektronicId da dobijemo samo one responseData koji su relevantni za naše fakture u goodCandidatesFakturList 
      if(webApiResultWithList?.ResponseData != null)
      {
         var theTRN_NewsList = /*vvMER_responseDataList*/webApiResultWithList.ResponseData
             .Join(
                 /*queryOutbox_CandidatesFakturList*/theUC.TheFakturList,
                 respData => respData.ElectronicId ?? 0L,
                 fak => (long)fak.F2_ElectronicID,
                 (respData, fak) => new
                 {
                    rowIdx = theUC.TheFakturList.IndexOf(fak),
                    refreshedStatusId = respData.StatusId,
                    faktur = fak
                 }
             )
             .Where(item => item.refreshedStatusId.HasValue && item.refreshedStatusId.Value != item.faktur.F2_StatusCD);

         foreach(var theTRN_News in theTRN_NewsList)
         {
            newsCount++;

            F2_IRn_faktur_rec = theTRN_News.faktur;

            // update Vv dataLayer 

            theUC.TheVvTabPage.TheVvForm.BeginEdit(F2_IRn_faktur_rec);

            F2_IRn_faktur_rec.F2_StatusCD = theTRN_News.refreshedStatusId.Value;

            // kada Faktur nastaje ne rucno, neg iz npr Rastera onda tad jos nema TtNumFisk_InVezniDok pa ovdje nadoknadjujemo 
            if(F2_IRn_faktur_rec.Is_F2_TtNumFisk_InVezniDok && F2_IRn_faktur_rec.VezniDok.IsEmpty())
            {
               F2_IRn_faktur_rec.VezniDok = F2_IRn_faktur_rec.TtNumFiskal; // cuvat cemo u data layeru rezultat result propertya 'TtNumFiskal' 
            }

            bool rwtOK = F2_IRn_faktur_rec.VvDao.RWTREC(theUC.TheDbConnection, F2_IRn_faktur_rec, false, true, false);

            theUC.TheVvTabPage.TheVvForm.EndEdit(F2_IRn_faktur_rec);

            if(rwtOK)
            {
               theUC.PutDgvLineFields(theTRN_News.rowIdx, F2_IRn_faktur_rec); // osvjezi prikaz 
               updatedStatusInfo = string.Format("{0} ({1}) Novi TRANSPORTNI status:      {2}      {3} {4}",
                                             F2_IRn_faktur_rec.TipBr,
                                             F2_IRn_faktur_rec.F2_ElectronicID/*MER_ElectronicID*/,
                                             Vv_eRacun_HTTP.MER_TransportStatuses[theTRN_News.refreshedStatusId.Value],
                                             F2_IRn_faktur_rec.DokDate.ToString(ZXC.VvDateFormat), F2_IRn_faktur_rec.KupdobName);

               updatedStatusInfoList.Add(updatedStatusInfo);

            } // if(rwtOK)

         } // foreach(var item in theNewsList) 
      }

      #endregion Refresh TRN status

      #region Refresh DPS status

      #endregion Refresh TRN or DPS status

      if(!ZXC.IsF2_2026_rules) goto RECEIVE_AND_FINISH_LABEL;

      #region Refresh AllFISK_Outbox status

      ZXC.SetStatusText("Refresh AllFISK_Outbox status");

      WebApiResult<List<VvMER_Response_Data_FiscalizationStatus>> webApiResultWithList_2 = Vv_eRacun_HTTP.VvMER_WebService_Get_FISK_Status_Outbox(queryOutbox_DateOD, queryOutbox_DateDO);

      if(webApiResultWithList_2 == null || webApiResultWithList_2.ResponseData == null /*|| webApiResultWithList_2.ResponseData.IsEmpty()*/)
      {
       //Show_WebApiResult_ErrorMessageBox(webApiResultWithList_2);
         Show_WebApiResult_ErrorMessageBox<VvMER_Response_Data_FiscalizationStatus>(webApiResultWithList_2);

         //return 0;
      }

      #region 1. FISK Status - Outgoing eRacun

      int wantedMessageType = 0; // 0 – Kao POŠILJATELJ dohvati status fiskalizacije 

      var theFakturList_not_FISK_yet = theUC.TheFakturList.Where(fak => fak.F2_IsFisk != ZXC.F2_StatusInAndOutBoxEnum.DA_JE); // Lista Faktura koje nisu fiskalizirane 

      List<VvMER_FiscalizationMessage> theFISKstatus_MessagesList = new List<VvMER_FiscalizationMessage>();
      VvMER_FiscalizationMessage lastFISKMessage;

      if(webApiResultWithList_2?.ResponseData != null)
      foreach(VvMER_Response_Data_FiscalizationStatus respData in webApiResultWithList_2.ResponseData)
      {
       //lastFISKMessage = respData.Messages.LastOrDefault(msg => msg.MessageType == wantedMessageType && (bool)msg.IsSuccess);
         lastFISKMessage = respData.Messages.LastOrDefault(msg => msg.MessageType == wantedMessageType && msg.StatusOutboxKind == ZXC.F2_StatusInAndOutBoxEnum.DA_JE);

         if(lastFISKMessage == null) continue;

         lastFISKMessage.TheElectronicId = respData.ElectronicId ?? 0L;

         theFISKstatus_MessagesList.Add(lastFISKMessage);
      }

      var theFISKstatus_Outbox_NewsList = theFISKstatus_MessagesList
          .Join(
              theFakturList_not_FISK_yet,
              message => message.TheElectronicId,
              fak => (long)fak.F2_ElectronicID,
              (message, fak) => new
              {
                 rowIdx = theUC.TheFakturList.IndexOf(fak),
                 faktur = fak
              }
          );

      foreach(var theFISKstatus_Outbox_News in theFISKstatus_Outbox_NewsList)
      {
         newsCount++;

         F2_IRn_faktur_rec = theFISKstatus_Outbox_News.faktur;

         // update Vv dataLayer 

         theUC.TheVvTabPage.TheVvForm.BeginEdit(F2_IRn_faktur_rec);

         F2_IRn_faktur_rec.F2_IsFisk = ZXC.F2_StatusInAndOutBoxEnum.DA_JE;

         bool rwtOK = F2_IRn_faktur_rec.VvDao.RWTREC(theUC.TheDbConnection, F2_IRn_faktur_rec, false, true, false);

         theUC.TheVvTabPage.TheVvForm.EndEdit(F2_IRn_faktur_rec);

         if(rwtOK)
         {
            theUC.PutDgvLineFields(theFISKstatus_Outbox_News.rowIdx, F2_IRn_faktur_rec); // osvjezi prikaz 
            updatedStatusInfo = string.Format("Nova FISKALIZACIJA računa: {0} ({1}) {2} {3}",
                                          F2_IRn_faktur_rec.TipBr,
                                          F2_IRn_faktur_rec.F2_ElectronicID,
                                          F2_IRn_faktur_rec.DokDate.ToString(ZXC.VvDateFormat), 
                                          F2_IRn_faktur_rec.KupdobName);

            updatedStatusInfoList.Add(updatedStatusInfo);

         } // if(rwtOK)

      } // foreach(var item in theFISKstatus_Outbox_NewsList) 

      #endregion 1. FISK Status - Outgoing eRacun

      #region 2. REJECT Status - Outgoing eRacun

      wantedMessageType = 2; // 2 – Dohvati status odbijanja 

      var theFakturList_not_REJECT_yet = theUC.TheFakturList.Where(fak => fak.F2_IsRejected != ZXC.F2_StatusInAndOutBoxEnum.DA_JE); // Lista Faktura koje nisu odbijene 

      List<VvMER_FiscalizationMessage> theREJECTstatus_MessagesList = new List<VvMER_FiscalizationMessage>();
      VvMER_FiscalizationMessage lastREJECTMessage;

      if(webApiResultWithList_2?.ResponseData != null)
      foreach(VvMER_Response_Data_FiscalizationStatus respData in webApiResultWithList_2.ResponseData)
      {
         lastREJECTMessage = respData.Messages.LastOrDefault(msg => msg.MessageType == wantedMessageType && msg.StatusOutboxKind == ZXC.F2_StatusInAndOutBoxEnum.DA_JE);
         
         if(lastREJECTMessage == null) continue;

         lastREJECTMessage.TheElectronicId = respData.ElectronicId ?? 0L;
         theREJECTstatus_MessagesList.Add(lastREJECTMessage);
      }

      var theREJECTstatus_Outbox_NewsList = theREJECTstatus_MessagesList
          .Join(
              theFakturList_not_REJECT_yet,
              message => message.TheElectronicId,
              fak => (long)fak.F2_ElectronicID,
              (message, fak) => new
              {
                 rowIdx = theUC.TheFakturList.IndexOf(fak),
                 faktur = fak
              }
          );

      foreach(var theREJECTstatus_Outbox_News in theREJECTstatus_Outbox_NewsList)
      {
         newsCount++;

         F2_IRn_faktur_rec = theREJECTstatus_Outbox_News.faktur;

         // update Vv dataLayer 

         theUC.TheVvTabPage.TheVvForm.BeginEdit(F2_IRn_faktur_rec);

         F2_IRn_faktur_rec.F2_IsRejected = ZXC.F2_StatusInAndOutBoxEnum.DA_JE;

         bool rwtOK = F2_IRn_faktur_rec.VvDao.RWTREC(theUC.TheDbConnection, F2_IRn_faktur_rec, false, true, false);

         theUC.TheVvTabPage.TheVvForm.EndEdit(F2_IRn_faktur_rec);

         if(rwtOK)
         {
            theUC.PutDgvLineFields(theREJECTstatus_Outbox_News.rowIdx, F2_IRn_faktur_rec); // osvjezi prikaz 
            updatedStatusInfo = string.Format("Novo !!! ODBIJANJE / REJECT!!! računa: {0} ({1}) {2} {3}",
                                          F2_IRn_faktur_rec.TipBr,
                                          F2_IRn_faktur_rec.F2_ElectronicID,
                                          F2_IRn_faktur_rec.DokDate.ToString(ZXC.VvDateFormat), 
                                          F2_IRn_faktur_rec.KupdobName);

            updatedStatusInfoList.Add(updatedStatusInfo);

         } // if(rwtOK)

      } // foreach(var item in theFISKstatus_Outbox_NewsList) 

      #endregion 2. REJECT Status - Outgoing eRacun

      #region 3. MAP Status - Outgoing eRacun

      wantedMessageType = 3; // 3 – Dohvati status plaćanja 

      var theFakturList_not_MAP_yet = theUC.TheFakturList.Where(fak => fak.F2_IsMarkAsPaid != ZXC.F2_StatusInAndOutBoxEnum.DA_JE); // Lista Faktura koje nisu označene kao plaćene 

      List<VvMER_FiscalizationMessage> theMAPstatus_MessagesList = new List<VvMER_FiscalizationMessage>();
      VvMER_FiscalizationMessage lastMAPMessage;

      if(webApiResultWithList_2?.ResponseData != null)
      foreach(VvMER_Response_Data_FiscalizationStatus respData in webApiResultWithList_2.ResponseData)
      {
         lastMAPMessage = respData.Messages.LastOrDefault(msg => msg.MessageType == wantedMessageType && msg.StatusOutboxKind == ZXC.F2_StatusInAndOutBoxEnum.DA_JE);

         if(lastMAPMessage == null) continue;

         lastMAPMessage.TheElectronicId = respData.ElectronicId ?? 0L;
         theMAPstatus_MessagesList.Add(lastMAPMessage);
      }

      var theMAPstatus_Outbox_NewsList = theMAPstatus_MessagesList
          .Join(
              theFakturList_not_MAP_yet,
              message => message.TheElectronicId,
              fak => (long)fak.F2_ElectronicID,
              (message, fak) => new
              {
                 rowIdx = theUC.TheFakturList.IndexOf(fak),
                 faktur = fak
              }
          );

      foreach(var theMAPstatus_Outbox_News in theMAPstatus_Outbox_NewsList)
      {
         newsCount++;

         F2_IRn_faktur_rec = theMAPstatus_Outbox_News.faktur;

         // update Vv dataLayer 

         theUC.TheVvTabPage.TheVvForm.BeginEdit(F2_IRn_faktur_rec);

         F2_IRn_faktur_rec.F2_IsMarkAsPaid = ZXC.F2_StatusInAndOutBoxEnum.DA_JE;

         bool rwtOK = F2_IRn_faktur_rec.VvDao.RWTREC(theUC.TheDbConnection, F2_IRn_faktur_rec, false, true, false);

         theUC.TheVvTabPage.TheVvForm.EndEdit(F2_IRn_faktur_rec);

         if(rwtOK)
         {
            theUC.PutDgvLineFields(theMAPstatus_Outbox_News.rowIdx, F2_IRn_faktur_rec); // osvjezi prikaz 
            updatedStatusInfo = string.Format("Novi Označen Kao Plaćen račun: {0} ({1}) {2} {3}",
                                          F2_IRn_faktur_rec.TipBr,
                                          F2_IRn_faktur_rec.F2_ElectronicID,
                                          F2_IRn_faktur_rec.DokDate.ToString(ZXC.VvDateFormat), 
                                          F2_IRn_faktur_rec.KupdobName);

            updatedStatusInfoList.Add(updatedStatusInfo);

         } // if(rwtOK)

      } // foreach(var item in theFISKstatus_Outbox_NewsList) 

      #endregion 3. MAP Status - Outgoing eRacun

      #region 4. eIZVJ Status - Outgoing eRacun

      wantedMessageType = 4; // 4 – Dohvati status eIzvještavanja 

      var theFakturList_not_eIZVJ_yet = theUC.TheFakturList.Where(fak => fak.F2_IsEizvj != ZXC.F2_StatusInAndOutBoxEnum.DA_JE); // Lista Faktura koje nisu prijavljene kao/na eIZVJ 

      List<VvMER_FiscalizationMessage> theeIZVJstatus_MessagesList = new List<VvMER_FiscalizationMessage>();
      VvMER_FiscalizationMessage lasteIZVJMessage;

      if(webApiResultWithList_2?.ResponseData != null)
      foreach(VvMER_Response_Data_FiscalizationStatus respData in webApiResultWithList_2.ResponseData)
      {
         lasteIZVJMessage = respData.Messages.LastOrDefault(msg => msg.MessageType == wantedMessageType && msg.StatusOutboxKind == ZXC.F2_StatusInAndOutBoxEnum.DA_JE);

         if(lasteIZVJMessage == null) continue;

         lasteIZVJMessage.TheElectronicId = respData.ElectronicId ?? 0L;
         theeIZVJstatus_MessagesList.Add(lasteIZVJMessage);
      }

      var theeIZVJstatus_Outbox_NewsList = theeIZVJstatus_MessagesList
          .Join(
              theFakturList_not_eIZVJ_yet,
              message => message.TheElectronicId,
              fak => (long)fak.F2_ElectronicID,
              (message, fak) => new
              {
                 rowIdx = theUC.TheFakturList.IndexOf(fak),
                 faktur = fak
              }
          );

      foreach(var theeIZVJstatus_Outbox_News in theeIZVJstatus_Outbox_NewsList)
      {
         newsCount++;

         F2_IRn_faktur_rec = theeIZVJstatus_Outbox_News.faktur;

         // update Vv dataLayer 

         theUC.TheVvTabPage.TheVvForm.BeginEdit(F2_IRn_faktur_rec);

         F2_IRn_faktur_rec.F2_IsEizvj = ZXC.F2_StatusInAndOutBoxEnum.DA_JE;

         bool rwtOK = F2_IRn_faktur_rec.VvDao.RWTREC(theUC.TheDbConnection, F2_IRn_faktur_rec, false, true, false);

         theUC.TheVvTabPage.TheVvForm.EndEdit(F2_IRn_faktur_rec);

         if(rwtOK)
         {
            theUC.PutDgvLineFields(theeIZVJstatus_Outbox_News.rowIdx, F2_IRn_faktur_rec); // osvjezi prikaz 
            updatedStatusInfo = string.Format("Novi račun prihvaćen ne eIzvještavanje: {0} ({1}) {2} {3}",
                                          F2_IRn_faktur_rec.TipBr,
                                          F2_IRn_faktur_rec.F2_ElectronicID,
                                          F2_IRn_faktur_rec.DokDate.ToString(ZXC.VvDateFormat),
                                          F2_IRn_faktur_rec.KupdobName);

            updatedStatusInfoList.Add(updatedStatusInfo);

         } // if(rwtOK)

      } // foreach(var item in theFISKstatus_Outbox_NewsList) 

      #endregion 4. eIZVJ Status - Outgoing eRacun

      #endregion Refresh AllFISK_Outbox status

      #region Refresh MarkAsPaid_InfoColumns

      ZXC.SetStatusText("Refresh MarkAsPaid InfoColumns");

      for(int rIdx = 0; (ZXC.IsF2_2026_rules || Vv_eRacun_HTTP.DEMO) && rIdx < theUC.TheG.RowCount; ++rIdx)
      {
         F2_IRn_faktur_rec = theUC.TheFakturList[rIdx];

         if(F2_IRn_faktur_rec.F2_HasNoSense_Refresh_MarkAsPaid_InfoColumns) continue; // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

         MAP_naplacenoFtransList   = FtransDao.Get_Naplaceno_OR_TodoMAP_FtransList_For_FakRecID(theUC.TheDbConnection, F2_IRn_faktur_rec.RecID, false);
         MAP_Ftr_naplacenoMoney    = MAP_naplacenoFtransList.Sum(ft => ft.T_pot);
         MAP_Ftr_naplacenoCount    = MAP_naplacenoFtransList.Count;
         MAP_prijavljenoXtranoList = XtranoDao.Get_Prijavljeno_MAP_XtranoList_For_FakRecID     (theUC.TheDbConnection, F2_IRn_faktur_rec.RecID       );
         MAP_Xtr_prijavljenoMoney  = MAP_prijavljenoXtranoList.Sum(ft => ft.T_moneyA);
         MAP_Xtr_prijavljenoCount  = MAP_prijavljenoXtranoList.Count;

         MAP_Ftr_naplacenoStr      = MAP_Ftr_naplacenoCount   > 1 ? string.Format("({0}) {1}", MAP_Ftr_naplacenoCount  , MAP_Ftr_naplacenoMoney  .ToStringVv()) :
                                                                                                                         MAP_Ftr_naplacenoMoney  .ToStringVv()  ; 
         MAP_Xtr_prijavljenoStr    = MAP_Xtr_prijavljenoCount > 1 ? string.Format("({0}) {1}", MAP_Xtr_prijavljenoCount, MAP_Xtr_prijavljenoMoney.ToStringVv()) :
                                                                                                                         MAP_Xtr_prijavljenoMoney.ToStringVv()  ;

         theUC.TheG.PutCell(theUC.DgvCI.iT_uplata    , rIdx, MAP_Ftr_naplacenoStr  );
         theUC.TheG.PutCell(theUC.DgvCI.iT_markPaid  , rIdx, MAP_Xtr_prijavljenoStr);
         theUC.TheG.PutCell(theUC.DgvCI.iT_razlikaUpl, rIdx, MAP_Ftr_naplacenoMoney - MAP_Xtr_prijavljenoMoney);

         if((MAP_Ftr_naplacenoMoney - MAP_Xtr_prijavljenoMoney).NotZero()) (theUC.TheG.Rows[rIdx].Cells[theUC.DgvCI.iT_razlikaUpl]).Style.ForeColor = Color.Red;

      } // for(int rIdx = 0; rIdx < theUC.TheG.RowCount; ++rIdx)

      #endregion Refresh MarkAsPaid_InfoColumns

      RECEIVE_AND_FINISH_LABEL:

      #region RECEIVE eRacun for Arhiva 

      ZXC.SetStatusText("RECEIVE eRacun for Arhiva");

      int firstRowIdx = 0;
      int lastRowIdx  = theUC.TheG.RowCount;
      bool isNaopako  = ZXC.RRD.Dsc_F2_IsAsc == false;

      if(isNaopako)
      {
         firstRowIdx = theUC.TheG.RowCount - 1;
         lastRowIdx  = -1 ;
      }

      for(int rIdx = firstRowIdx; isNaopako ? (rIdx > lastRowIdx) : (rIdx < lastRowIdx); rIdx = (isNaopako ? rIdx - 1 : rIdx + 1))
      {
         F2_IRn_faktur_rec = theUC.TheFakturList[rIdx];

         if(F2_IRn_faktur_rec.F2_HasNoSense_RECEIVE_document2arhiva) continue; // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

         uint arhivaXtrano_recID = WS_Get_RECEIVE_Izlaz_Document2Arhiva_ForElectronicID(theUC, F2_IRn_faktur_rec.F2_ElectronicID, F2_IRn_faktur_rec);

         if(arhivaXtrano_recID.NotZero() && F2_IRn_faktur_rec.F2_ArhRecID.IsZero())
         {
            // update Faktur dataLayer 

            theUC.TheVvTabPage.TheVvForm.BeginEdit(F2_IRn_faktur_rec);
            
            F2_IRn_faktur_rec.F2_ArhRecID = arhivaXtrano_recID;

            bool rwtOK = F2_IRn_faktur_rec.VvDao.RWTREC(theUC.TheDbConnection, F2_IRn_faktur_rec, false, true, false);

            theUC.TheVvTabPage.TheVvForm.EndEdit(F2_IRn_faktur_rec);

            if(rwtOK)
            {
               newsCount++;

               theUC.PutDgvLineFields(rIdx, F2_IRn_faktur_rec); // osvjezi prikaz 

               updatedStatusInfo = string.Format("{0} ({1}) Novi eRačun u arhivi:      {2}      {3} {4}",
                                             F2_IRn_faktur_rec.TipBr,
                                             F2_IRn_faktur_rec.F2_ElectronicID,
                                             "ARHIVIRANO",
                                             F2_IRn_faktur_rec.DokDate.ToString(ZXC.VvDateFormat), F2_IRn_faktur_rec.KupdobName);

               updatedStatusInfoList.Add(updatedStatusInfo);

            } // if(rwtOK)

         }

      } // for(int rIdx = 0; rIdx < theUC.TheG.RowCount; ++rIdx)

      #endregion RECEIVE eRacun for Arhiva 

      #region Finish

      Cursor.Current = Cursors.Default;

      ZXC.SetStatusText("");

      if(updatedStatusInfoList.NotEmpty())
      {
         ZXC.aim_emsg_List(string.Format("Ima {0} novosti.", updatedStatusInfoList.Count), updatedStatusInfoList);
      }

      return newsCount;

      #endregion Finish
   } 
   /* DDD */internal static int WS_Discover_Candidates_And_Eventually_MAPaj_uplate(VvUserControl theUC, bool isManual, bool isFromNalog)
   {
      #region Init & Get Dialog Fields AND Create MAP_requestDataList 

      int newsCount = 0;

      //if(ZXC.RRD.Dsc_F2_IsAutoMAP == false) return;

      List<(VvMER_RequestData request, Ftrans ftrans, Faktur faktur)> MAP_ActionsList = new List<(VvMER_RequestData request, Ftrans ftrans, Faktur faktur)>();

      Faktur MAP_CandidateFaktur_rec;

      List<VvReportSourceUtil> messageList = new List<VvReportSourceUtil>();

      List<Ftrans> paymentftransList;

      string thePaymentMethod = "T"; // T je default a dole se jos postavlja prema TT-u 

      if(isFromNalog)
      {
         NalogDUC nalogDUC = theUC as NalogDUC;

         List<Ftrans> MAPftransList = nalogDUC.nalog_rec.Transes.Where(ftr => ftr.R_IsMAPcandidate_Ftr).ToList();

         paymentftransList = new List<Ftrans>();

         foreach(Ftrans MAPftrans_rec in MAPftransList)
         {
            if(!FtransDao.IsMAPdone(theUC.TheDbConnection, MAPftrans_rec)) paymentftransList.Add(MAPftrans_rec); // dodaj ako nije još MAPano 
         }
      }
      else // IsFromFIR 
      {
         paymentftransList = FtransDao.Get_MAP_FtransList(theUC.TheDbConnection).OrderBy(ftr => ftr.T_dokNum).ToList(); // ftrans 'MAP' kandidati: naplate od KUPACa koje nisu jos MAPane 
      }

      if(paymentftransList.IsEmpty())
      {
         if(isManual) ZXC.aim_emsg(MessageBoxIcon.Information, "Nema ništa za slanje.");
         return 0;
      }

      VvMER_RequestData MAP_requestData;

      int zeroFakYearCount = paymentftransList.Count(ftr => ftr.T_fakYear.IsZero());

      if(zeroFakYearCount.IsPositive()) ZXC.aim_emsg(MessageBoxIcon.Warning, $"Postoji {zeroFakYearCount} zatvaranja sa oznakom T_FakYear nula!?");

      // Remove one koji su prije 2026 
      paymentftransList.RemoveAll(ftr => ftr.T_fakYear.NotZero() && ftr.T_fakYear < 2026);

      foreach(Ftrans paymentftrans_rec in paymentftransList)
      {
         //MAP_CandidateFaktur_rec = new Faktur();
         //
         //if(paymentftrans_rec.T_fakRecID.NotZero()) 
         //{
         //   if(paymentftrans_rec.T_fakYear.IsZero())
         //   {
         //      ZXC.aim_emsg(MessageBoxIcon.Error, $"{paymentftrans_rec.ToShortString()}{Environment.NewLine}T_fakYear IsZero!!!");
         //      continue;
         //   }
         //
         //   if(paymentftrans_rec.T_fakYear == ZXC.projectYearAsInt) // uplata se odnosi na Faktur iz ove godine 
         //   {
         //      MAP_CandidateFaktur_rec.VvDao.SetMe_Record_byRecID_Complete(theUC.TheDbConnection, paymentftrans_rec.T_fakRecID, MAP_CandidateFaktur_rec);
         //   }
         //   else // uplata se odnosi na Faktur iz neke PROŠLE godine 
         //   {
         //      MAP_CandidateFaktur_rec.VvDao.SetMe_Record_byRecID_Complete(/*conn*/ZXC.TheSecondDbConn_SameDB_OtherYear((int)paymentftrans_rec.T_fakYear), paymentftrans_rec.T_fakRecID, MAP_CandidateFaktur_rec);
         //   }
         //
         //   thePaymentMethod = paymentftrans_rec.T_TT == Nalog.IZ_TT ? "T" : paymentftrans_rec.T_TT == Nalog.KP_TT ? "O" : "Z"; //16.12.2025.
         //}
         //else
         //{
         //   ZXC.aim_emsg(MessageBoxIcon.Error, $"{paymentftrans_rec.ToShortString()}{Environment.NewLine}T_fakRecID IsZero!!!");
         //
         //   MAP_CandidateFaktur_rec = null;
         //   // TODO: ako ispadne da je T_fakRecID prazan, ovdje treba potražiti fakturu preko T_tipBr-a ... ili kojiK ... npr ako je R1/R2 'po naplati' 
         //}

         MAP_CandidateFaktur_rec = GetFakturFromPaymentFtrans(paymentftrans_rec, theUC.TheDbConnection, out thePaymentMethod);

         if(MAP_CandidateFaktur_rec == null)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Ne mogu pronaći fakturu za MAP!? Ftrans: {0}", paymentftrans_rec);
            continue; 
         }

         if(MAP_CandidateFaktur_rec.F2_ElectronicID.IsZero())
         {
            // odlucujemo ne javljati poruku neg samo odi dalje. ovo rjesava preskakamji YRN-ova i IRM-ova 
          //ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, 
          //   $"Ne mogu prijaviti uplatu za fakturu koja još nije ni poslana!? Ftrans: {paymentftrans_rec}{Environment.NewLine}{Environment.NewLine}Faktur: {MAP_CandidateFaktur_rec}");
            continue;
         }

         MAP_requestData = new VvMER_RequestData()
         {
            ElectronicId  = MAP_CandidateFaktur_rec.F2_ElectronicID,
            PaymentDate   = paymentftrans_rec.T_dokDate            ,
            PaymentAmount = paymentftrans_rec.T_pot                ,
            PaymentMethod = thePaymentMethod
         };

         MAP_ActionsList.Add((MAP_requestData, paymentftrans_rec, MAP_CandidateFaktur_rec));

         messageList.Add(new VvReportSourceUtil()
         {
          //IsNekakav  = MAP_CandidateFaktur_rec.Is_MAP_with_ElectronicID,

            TheCD      = MAP_CandidateFaktur_rec.TipBr,
            DevName    = MAP_CandidateFaktur_rec.DokDate.ToString(ZXC.VvDateFormat),
            KupdobName = MAP_CandidateFaktur_rec.KupdobName,
            TheMoney   = MAP_CandidateFaktur_rec.S_ukKCRP,

            String1    = paymentftrans_rec.T_dokDate.ToString(ZXC.VvDateFormat),
            TheMoney2  = paymentftrans_rec.T_pot,
            String2    = thePaymentMethod,
            String3    = paymentftrans_rec.T_dokNum.ToString() + "/" + paymentftrans_rec.T_serial.ToString(),
            String4    = paymentftrans_rec.T_opis,
            String5    = paymentftrans_rec.T_konto,

            UtilUint   = paymentftrans_rec.T_recID

         });

      } // foreach(Ftrans paymentftrans_rec in paymentftransList) 

      VvMessageBoxDLG  MAP_CandidatesFtransList_InfoDLG = new VvMessageBoxDLG (false, ZXC.VvmBoxKind.F2_MAP_candidates);
      MAP_CandidatesFtransList_InfoDLG.Text = "Kandidati za slanje prijave plaćanja:";

      MAP_CandidatesFtransList_InfoDLG.TheUC.PutDgvFields_F2_MAP_candidates(messageList);
      MAP_CandidatesFtransList_InfoDLG.TheUC.Fld_IsAutoMAP = ZXC.RRD.Dsc_F2_IsAutoMAP;

      DialogResult dlgResult = MAP_CandidatesFtransList_InfoDLG.ShowDialog();

      if(dlgResult != DialogResult.OK)
      {
         MAP_CandidatesFtransList_InfoDLG.Dispose();
         return -1;
      }

      if(ZXC.RRD.Dsc_F2_IsAutoMAP != MAP_CandidatesFtransList_InfoDLG.TheUC.Fld_IsAutoMAP)
      {
         ZXC.RRD.Dsc_F2_IsAutoMAP = MAP_CandidatesFtransList_InfoDLG.TheUC.Fld_IsAutoMAP; 
         ZXC.RRD.SaveDscToLookUpItemList();
      }

      int numOfFirstLinesOnly   =  MAP_CandidatesFtransList_InfoDLG.TheUC.Fld_NumOfFirstLinesOnly_MAP;

      #region Izbaci 'preskočene'

      bool shouldSkip;
      uint ftrRecIDtoSkip;
      int foundCount;

      for(int rIdx = 0; rIdx < MAP_CandidatesFtransList_InfoDLG.TheUC.TheGrid.RowCount /*- 1*/; ++rIdx)
      {
         shouldSkip = MAP_CandidatesFtransList_InfoDLG.TheUC.TheGrid.GetBoolCell(MAP_CandidatesFtransList_InfoDLG.TheUC.DgvCI.iT_shouldS, rIdx, false);

         if(shouldSkip)
         {
            ftrRecIDtoSkip = MAP_CandidatesFtransList_InfoDLG.TheUC.TheGrid.GetUint32Cell(MAP_CandidatesFtransList_InfoDLG.TheUC.DgvCI.iT_ftrRecID, rIdx, false);
            
            foundCount = MAP_ActionsList.RemoveAll(MAPal => MAPal.ftrans.T_recID == ftrRecIDtoSkip);

            if(foundCount.IsZero()) ZXC.aim_emsg(MessageBoxIcon.Error, "shouldSkip MAP_action NOT FOUND!");
         }
      }

      #endregion Izbaci 'preskočene'

      MAP_CandidatesFtransList_InfoDLG.Dispose();

      Cursor.Current = Cursors.WaitCursor;

      ZXC.SetStatusText("Discover_Candidates_And_Eventually_MAPaj_uplate");

      int mapCount = 0; bool MAP_OK;

      System.Diagnostics.Stopwatch dispatchStopWatch = System.Diagnostics.Stopwatch.StartNew();

      uint soFarCount      = 0;
       int ofTotalCount    = numOfFirstLinesOnly.NotZero() ? numOfFirstLinesOnly : paymentftransList.Count;
      long elapsedTicks    = 0, remainTicks;
      decimal soFarKoef       ;
      TimeSpan elapsedTime = new TimeSpan(0);
      TimeSpan remainTime     ;
      string statusText       ;

      WebApiResult<VvMER_ResponseData> webApiResult;

      Xtrano F2_MAP_Xtrano_rec;

      #endregion Init & Get Dialog Fields AND Create MAP_requestDataList 

      #region The MAP API Loop - foreach MAP_requestData

      foreach((VvMER_RequestData request, Ftrans ftrans, Faktur faktur) MAP_Action in MAP_ActionsList)
      {
         Cursor.Current = Cursors.WaitCursor;

         #region call the MAP API

         webApiResult = WS_Mark_Paid_With_OR_Without_ElectronicID(MAP_Action.request, true /*MAP_Action.faktur.Is_MAP_with_ElectronicID*/);

         MAP_OK       = (webApiResult.ResponseData != null);

         if(MAP_OK)
         {
            mapCount++;

            F2_MAP_Xtrano_rec = VvMER_ResponseData.F2_MAPtrans_SetXtranoFrom_Ftrans(MAP_Action.ftrans, MAP_Action.faktur, webApiResult.ResponseData);

            if(F2_MAP_Xtrano_rec != null)
            {
             //byte[] T_XmlZip = F2_MAP_Xtrano_rec.T_XmlZip;

               bool OK = ZXC.XtranoDao.ADDREC(theUC.TheDbConnection, F2_MAP_Xtrano_rec, /*false*/true, false, false, false);

               if(OK)
               {
                  newsCount++;

                  // 14.02.2026: ugaseno jer vise ne spremamo T_XmlZip za 'MAP' Xtrano 
                //theUC.TheVvTabPage.TheVvForm.BeginEdit(F2_MAP_Xtrano_rec);
                //
                //F2_MAP_Xtrano_rec.T_XmlZip = T_XmlZip;
                //
                //VvDaoBase.Rwtrec_BLOBsingleColumn(theUC.TheDbConnection, F2_MAP_Xtrano_rec, "t_XmlZip", F2_MAP_Xtrano_rec.T_XmlZip);
                //
                //theUC.TheVvTabPage.TheVvForm.EndEdit(F2_MAP_Xtrano_rec);
               }
            }
         }

         #endregion call the MAP API

         #region set status text

         soFarCount++;

         #region soFar vs remaining calc

         soFarKoef     = ZXC.DivSafe(soFarCount, ofTotalCount);
         elapsedTicks += dispatchStopWatch.Elapsed.Ticks          ;
         elapsedTime  += dispatchStopWatch.Elapsed                ;
         remainTicks   = (long)(ZXC.DivSafe((decimal)elapsedTicks, soFarKoef) - elapsedTicks);
         remainTime    = new TimeSpan(remainTicks);

         #endregion soFar vs remaining calc

         statusText =
            dispatchStopWatch.Elapsed.TotalSeconds.ToString1Vv() + "s " +
            "(" + (elapsedTime.TotalSeconds / (double)soFarCount).ToString1Vv() + "s avg) done " +
             (/*++*/soFarCount).ToString() +
             " of " + ofTotalCount +
             " (" + (soFarKoef * 100M).ToString0Vv() + "%)" +
            //" <"   + remainTime + "> "                              +
             string.Format(" remain <{0:00}:{1:00}:{2:00}> ", remainTime.Hours, remainTime.Minutes, remainTime.Seconds) +
             " " + MAP_Action.request.ToString();

         dispatchStopWatch.Restart();

         ZXC.SetStatusText(statusText); Cursor.Current = Cursors.WaitCursor;

         #endregion set status text

         if(numOfFirstLinesOnly.NotZero() && /*sendCount*/soFarCount == numOfFirstLinesOnly) break;

      } // foreach(Faktur sendCandidateFaktur_rec in sendCandidatesFakturList) 

      #endregion The MAP API Loop - foreach MAP_requestData

      #region Finish

    //ZXC.FakturRec = null;

      ZXC.SetStatusText("");

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Završeno slanje {0} prijava plaćanja.", mapCount);

      return newsCount;

      #endregion Finish
   }
   /* QQQ */internal static int HDD_Import_Extern_Faktur_IFA/*NOT WS_Ufati_Veleform_Ritam*/(F2_Izlaz_UC theUC)
   {
      #region Init

      Cursor.Current = Cursors.WaitCursor;

      ZXC.SetStatusText("Učitavam klijentove izlazne račune");

      int newsCount = 0;

      (DateTime queryOutbox_DateOD, DateTime queryOutbox_DateDO) = GetF2QueryDateRange();

      bool isNewFaktur, addrecOK, kupdobOK, xmlValidationOK;

      string theXmlString = "", theOIB = "";

      Faktur existingFaktur_byElectronicID, existingFaktur_byTtNumFiskal, newIFA_Faktur_rec = null;

      EN16931.UBL.InvoiceType    deserialized_InvoiceType    = null;
      EN16931.UBL.CreditNoteType deserialized_CreditNoteType = null;

      Kupdob kupdob_rec;

      List<string> updatedStatusInfoList = new List<string>();
           string  updatedStatusInfo                         ;

      List<string> newKupdobInfoList = new List<string>();
           string  newKupdobInfo                         ;

      string fullDirectoryPath;

      //using(FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
      //{
      //   string initialDir = VvForm.GetLocalDirectoryForVvFile(ZXC.eRacuniDIR);
      //
      //   folderBrowserDialog.Description = "Odaberite direktorij sa XML datotekama:";
      //   folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
      //   folderBrowserDialog.ShowNewFolderButton = false;
      //
      //   if(Directory.Exists(initialDir))
      //   {
      //      folderBrowserDialog.SelectedPath = initialDir;
      //   }
      //
      //   DialogResult result = folderBrowserDialog.ShowDialog();
      //
      //   if(result != DialogResult.OK || string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
      //   {
      //      ZXC.SetStatusText("");
      //      Cursor.Current = Cursors.Default;
      //      return 0;
      //   }
      //
      //   fullDirectoryPath = folderBrowserDialog.SelectedPath;
      //}

      using(OpenFileDialog openFileDialog = new OpenFileDialog())
      {
         string initialDir = (ZXC.TheVvForm.VvPref.theHDD_Import_Extern_Faktur_IFA_Prefs.DirectoryName.IsEmpty() ||
                              ZXC.TheVvForm.VvPref.theHDD_Import_Extern_Faktur_IFA_Prefs.DirectoryName == @".\"   ) ? 
                              Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) :
                              ZXC.TheVvForm.VvPref.theHDD_Import_Extern_Faktur_IFA_Prefs.DirectoryName;

         openFileDialog.Title           = "Odaberite direktorij sa XML datotekama IZLAZNIH računa";
         openFileDialog.ValidateNames   = false;
         openFileDialog.CheckFileExists = false;
         openFileDialog.CheckPathExists = true;
         openFileDialog.FileName        = "Odaberite directory";
         openFileDialog.DefaultExt      = ".xml";
       //openFileDialog.ReadOnlyChecked = true;
       //openFileDialog.ShowReadOnly    = true;

         if(Directory.Exists(initialDir))
         {
            openFileDialog.InitialDirectory = initialDir;
         }

         DialogResult result = openFileDialog.ShowDialog();

         if(result != DialogResult.OK || string.IsNullOrEmpty(openFileDialog.FileName))
         {
            ZXC.SetStatusText("");
            Cursor.Current = Cursors.Default;
            return 0;
         }

         ZXC.TheVvForm.VvPref.theHDD_Import_Extern_Faktur_IFA_Prefs.DirectoryName =

         fullDirectoryPath = Path.GetDirectoryName(openFileDialog.FileName);
      }

      bool thisXMLhaswrongOIB = false;
      bool wrongOIBdetected   = false;

      int rIdx;

      #endregion Init

      #region Synchronise Servis Faktur DataLayer with Klijent Faktur DataLayer via news from HDD directory 

    //WebApiResult<List<VvMER_ResponseData>> webApiResultWithList = Vv_eRacun_HTTP.VvMER_WebService_QueryOutbox_TRN_List(                   queryOutbox_DateOD, queryOutbox_DateDO);
      WebApiResult<List<VvMER_ResponseData>> webApiResultWithList = Vv_eRacun_HTTP.VvMER_LocalHDD_QueryOutbox_List     (fullDirectoryPath, queryOutbox_DateOD, queryOutbox_DateDO);

      if(webApiResultWithList == null || webApiResultWithList.ResponseData == null /*|| webApiResultWithList.ResponseData.IsEmpty()*/)
      {
         if(webApiResultWithList == null)
         {
            webApiResultWithList = new WebApiResult<List<VvMER_ResponseData>>()
            {
               WebApiKind        = ZXC.F2_WebApi.OutboxTRNstatusListAsKnjigServis,
               WebApiAddr        = webApiResultWithList.WebApiAddr,
               StatusCode        = -1,
               StatusDescription = "No response data",
               ErrorBody         = "No response data"
            };
         }
         else
         {
            if(webApiResultWithList.ResponseData != null && webApiResultWithList.ResponseData.IsEmpty())
            {
               webApiResultWithList.ErrorBody = "Lista je prazna";
            }
         }

         //Show_WebApiResult_ErrorMessageBox(webApiResultWithList);
         Show_WebApiResult_ErrorMessageBox<VvMER_ResponseData>(webApiResultWithList);

         return 0;
      }

      List<VvMER_ResponseData> loopList = webApiResultWithList.ResponseData.OrderBy(rd => rd.Created).ToList();
      
      foreach(VvMER_ResponseData responseData in loopList)
      {
         Cursor.Current = Cursors.WaitCursor;

       //existingFaktur_byElectronicID = theUC.TheFakturList./*Single*/FirstOrDefault(f => f.F2_ElectronicID         == responseData.ElectronicId);
         existingFaktur_byTtNumFiskal  = theUC.TheFakturList./*Single*/FirstOrDefault(f => f./*TtNumFiskal*/VezniDok == responseData.DocumentNr  );

         isNewFaktur = /*existingFaktur_byElectronicID == null &&*/ existingFaktur_byTtNumFiskal == null;

         if(isNewFaktur == false) continue;

         // here we go 

         #region 1. Call RECEIVE to get full XML document

         // NIJE RECEIVE API nego xml dode iz fajla z diska 
         theXmlString = responseData.DocumentXml;

         bool receiveOK = true;

         #endregion 1. Call RECEIVE to get full XML document

         #region 2. Deserialize eRacun XML document into 'InvoiceType' bussiness object & Validate XML against XSD schema

         xmlValidationOK = false;

         bool isCreditNote = receiveOK && IsXmlCreditNoteType(theXmlString);
         bool isInvoice    = receiveOK && IsXmlInvoiceType   (theXmlString);

         if(isCreditNote)
         {
            deserialized_CreditNoteType = receiveOK ? GetCreditNoteTypeByDeserializing_xmlString(theXmlString, false, responseData) : null;
            deserialized_InvoiceType = null;

            thisXMLhaswrongOIB = ZXC.CURR_prjkt_rec.Oib != deserialized_CreditNoteType.VvSupplierOIB;
         }
         else if(isInvoice)
         {
            deserialized_InvoiceType = receiveOK ? GetInvoiceTypeByDeserializing_xmlString(theXmlString, false, responseData) : null;
            deserialized_CreditNoteType = null;

            thisXMLhaswrongOIB = ZXC.CURR_prjkt_rec.Oib != deserialized_InvoiceType.VvSupplierOIB;
         }
         else 
         {
          //ZXC.aim_emsg(MessageBoxIcon.Error, $"Outbox dokument {webApiResult.ResponseData} nije niti račun (InvoiceType) niti odobrenje (CreditNote). Dokument neće biti učitan u IFA-e.");
            ZXC.aim_emsg(MessageBoxIcon.Error,
               $"Outbox dokument nije niti račun (InvoiceType) niti odobrenje (CreditNote).{Environment.NewLine}Dokument neće biti učitan u IFA-e.{Environment.NewLine}{Environment.NewLine}" +
               $"Elektronski ID: {responseData.ElectronicId}{Environment.NewLine}" +
               $"Broj dokumenta: {responseData.DocumentNr ?? "N/A"}{Environment.NewLine}" +
               $"Pošiljatelj: {responseData.SenderBusinessName ?? "N/A"}{Environment.NewLine}" +
               $"Datum slanja: {responseData.Sent?.ToString(ZXC.VvDateFormat) ?? "N/A"}{Environment.NewLine}" +
               $"Status: {responseData.StatusName ?? responseData.StatusId?.ToString() ?? "N/A"}");
         }

         if(thisXMLhaswrongOIB)
         {
            wrongOIBdetected = true;
            continue;
         }

         bool hasDeserializedDocument = (deserialized_InvoiceType != null || deserialized_CreditNoteType != null);

         if(hasDeserializedDocument)
         {
            try
            {
               if(!isCreditNote)
                  xmlValidationOK = Vv_XSD_Bussiness_BASE<EN16931.UBL.InvoiceType>.ValidateXmlAgainstXsd(theXmlString);
               else
                  xmlValidationOK = true; // TODO: dodati XSD validaciju za CreditNote ako bude potrebno 
            }
            catch(Exception ex)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, ex.Message);
            }
         }

         #endregion 2. Deserialize eRacun XML document into 'InvoiceType' bussiness object & Validate XML against XSD schema

         #region 3. Create_Faktur_From_eRacun & ADDREC to Vektor DataLayer

         if(receiveOK && hasDeserializedDocument && xmlValidationOK)
         {
            #region Get Kupdob / New Kupdob?

            if(isInvoice)         theOIB = deserialized_InvoiceType   .VvCustomerOIB;
            else if(isCreditNote) theOIB = deserialized_CreditNoteType.VvCustomerOIB;

            if(theOIB.IsEmpty())
            {
               kupdob_rec = null;
            }
            else
            {
               kupdob_rec = theUC.Get_Kupdob_FromVvUcSifrar_byOIB(theOIB);
            }

            if(kupdob_rec != null) kupdobOK = true ;
            else                   kupdobOK = false;

            if(kupdobOK == false) // try to create NEW Kupdob from eRacun data 
            {
               if(isInvoice)
               {
                  kupdob_rec = EN16931.UBL.InvoiceType.Create_Kupdob_from_InvoiceType(theUC.TheDbConnection, deserialized_InvoiceType, true);
               }
               else if(isCreditNote)
               {
                  kupdob_rec = EN16931.UBL.CreditNoteType.Create_Kupdob_from_CreditNote(theUC.TheDbConnection, deserialized_CreditNoteType, true);
               }

               if(kupdob_rec != null) // NEW Kupdob created ok 
               {
                  addrecOK = kupdob_rec.VvDao.ADDREC(theUC.TheDbConnection, kupdob_rec);

                  if(addrecOK)
                  {
                     newKupdobInfo = string.Format("Novi kupac [{0}],  OIB: [{1}], Ulica: {2}, Mjesto: {3}", kupdob_rec.Naziv, kupdob_rec.Oib, kupdob_rec.Ulica1, kupdob_rec.ZipAndMjesto);

                     newKupdobInfoList.Add(newKupdobInfo);

                     theUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

                     kupdobOK = true;
                  }
                  else
                  {
                     ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom ADDREC novog kupca (kupdob) iz eRačuna s eID={0} za OIB [{1}].", responseData.ElectronicId, theOIB);
                     kupdobOK = false;
                  }
               }
               else
               {
                  ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom kreiranja novog kupca (kupdob) iz eRačuna s eID={0} za OIB [{1}].", responseData.ElectronicId, theOIB);
               }

            } // if(kupdobOK == false) // try to create NEW Kupdob from eRacun data 
            
            else // kupdobOK == true, provjeri ima li dobar R1_Kind ... i ak nema ...
            {
               if(kupdob_rec.R1kind != ZXC.F2_R1enum.B2B)
               {
                  theUC.TheVvTabPage.TheVvForm.BeginEdit(kupdob_rec);

                  kupdob_rec.R1kind = ZXC.F2_R1enum.B2B;

                  kupdob_rec.VvDao.RWTREC(theUC.TheDbConnection, kupdob_rec, false, true, false);

                  theUC.TheVvTabPage.TheVvForm.EndEdit(kupdob_rec);

                  theUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name); // REFRESH sifrar! 
               }
            }

            #endregion Get Kupdob / New Kupdob?

            // 2. Create new Faktur bussiness object record from 'InvoiceType' OR 'CreditNoteType' in XML document 

            if(isInvoice)
            {
               newIFA_Faktur_rec = deserialized_InvoiceType.Create_Faktur_From_InvoiceType(theUC.TheDbConnection, (uint)responseData.ElectronicId, (DateTime)responseData.Sent, kupdob_rec, true);
            }
            else if(isCreditNote)
            {
               newIFA_Faktur_rec = deserialized_CreditNoteType.Create_Faktur_From_CreditNoteType(theUC.TheDbConnection, (uint)responseData.ElectronicId, (DateTime)responseData.Sent, kupdob_rec, true);
            }

            if(newIFA_Faktur_rec != null)
            {
               theUC.TheFakturList.Add(newIFA_Faktur_rec);

               newsCount++;
            
               updatedStatusInfo = string.Format("{0} (OrigBrDok: {1}) Nova IFA klijenta je {2} {3} {4}",
                                             newIFA_Faktur_rec.TipBr,
                                             newIFA_Faktur_rec./*F2_ElectronicID*/VezniDok,
                                             "DODANA u lokalnu bazu",
                                             newIFA_Faktur_rec.DokDate.ToString(ZXC.VvDateFormat), newIFA_Faktur_rec.KupdobName);
            
               updatedStatusInfoList.Add(updatedStatusInfo);

               ZXC.SetStatusText($"{newsCount}. od {loopList.Count}: {updatedStatusInfo}");
            }

         } // if(receiveOK) 

         #endregion 3. Create_Faktur_From_eRacun & ADDREC to Vektor DataLayer

         #region 4. Odglumi RECEIVE eRacun for Arhiva

         if(newIFA_Faktur_rec.F2_HasNoSense_RECEIVE_document2arhiva) continue; // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
         
         newIFA_Faktur_rec.VvDao.SetMe_Record_byRecID_Complete(theUC.TheDbConnection, newIFA_Faktur_rec.RecID, newIFA_Faktur_rec);
         
         uint arhivaXtrano_recID = HDD_Get_Izlaz_Document2Arhiva(theUC, theXmlString, newIFA_Faktur_rec);
         
         if(arhivaXtrano_recID.NotZero() && newIFA_Faktur_rec.F2_ArhRecID.IsZero())
         {
            // update Faktur dataLayer 
         
            theUC.TheVvTabPage.TheVvForm.BeginEdit(newIFA_Faktur_rec);
         
            newIFA_Faktur_rec.F2_ArhRecID = arhivaXtrano_recID;
         
            bool rwtOK = newIFA_Faktur_rec.VvDao.RWTREC(theUC.TheDbConnection, newIFA_Faktur_rec, false, true, false);
         
            theUC.TheVvTabPage.TheVvForm.EndEdit(newIFA_Faktur_rec);
         
            if(rwtOK)
            {
               newsCount++;
         
               rIdx = theUC.TheFakturList.IndexOf(newIFA_Faktur_rec);
         
               //theUC.PutDgvLineFields(rIdx, newIFA_Faktur_rec); // osvjezi prikaz 
         
               updatedStatusInfo = string.Format("{0} ({1}) Novi eRačun u arhivi:      {2}      {3} {4}",
                                             newIFA_Faktur_rec.TipBr,
                                             newIFA_Faktur_rec.F2_ElectronicID,
                                             "ARHIVIRANO",
                                             newIFA_Faktur_rec.DokDate.ToString(ZXC.VvDateFormat), newIFA_Faktur_rec.KupdobName);
         
               updatedStatusInfoList.Add(updatedStatusInfo);
         
            } // if(rwtOK)
         
         }

         #endregion 4. Odglumi RECEIVE eRacun for Arhiva

      } // foreach(VvMER_ResponseData responseData in loopList) 

      #endregion Synchronise Servis Faktur DataLayer with Klijent Faktur DataLayer via news from HDD directory 

      #region TheFakturList ... OrderBy And PutDgvFields

      //if(ZXC.RRD.Dsc_F2_IsAsc == false) theUC.TheFakturList = theUC.TheFakturList.OrderByDescending(f => f.TtNum).ToList();
      //else                              theUC.TheFakturList = theUC.TheFakturList.OrderBy          (f => f.TtNum).ToList();
      //
      //if(theUC.TheFakturList.NotEmpty()) theUC.PutDgvFields();

      #endregion TheFakturList ... OrderBy And PutDgvFields

      #region Finish

      Cursor.Current = Cursors.Default;

      ZXC.SetStatusText("");

      if(updatedStatusInfoList.NotEmpty())
      {
         Load_IRn_FakturList(theUC);

         ZXC.aim_emsg_List(string.Format("DODANO je {0} novih klijentovih računa u Vektorovu bazu podataka.", updatedStatusInfoList.Count), updatedStatusInfoList);
      }

      if(newKupdobInfoList.NotEmpty())
      {
         ZXC.aim_emsg_List(string.Format("DODANO je {0} novih partnera (kupaca) u Vektorovu bazu podataka.", newKupdobInfoList.Count), newKupdobInfoList);
      }

      if(newsCount.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Information, "Nema novosti.");
      }

      if(wrongOIBdetected)
      {
         ZXC.aim_emsg(MessageBoxIcon.Exclamation, "Pronađene su datoteke sa neodgovarajućim OIB-om!\n\r\n\rKrivi projekt ili krivo odabrani directory?");
      }

      return newsCount;

      #endregion Finish
   }
   private static WebApiResult<List<VvMER_ResponseData>> VvMER_LocalHDD_QueryOutbox_List(string fullDirectoryPath, DateTime queryOutbox_DateOD, DateTime queryOutbox_DateDO)
   {
      WebApiResult<List<VvMER_ResponseData>> webApiResult = new WebApiResult<List<VvMER_ResponseData>>()
      {
         WebApiKind = ZXC.F2_WebApi.HDD_OutboxListAsKnjigServis,
         WebApiAddr = fullDirectoryPath,
      };

      try
      {
         // Validate directory
         if(string.IsNullOrEmpty(fullDirectoryPath) || !Directory.Exists(fullDirectoryPath))
         {
            webApiResult.StatusCode = -1;
            webApiResult.StatusDescription = "Directory not found";
            webApiResult.ErrorBody = $"Direktorij ne postoji: {fullDirectoryPath}";
            return webApiResult;
         }

         // Get all XML files from directory
         string[] xmlFileNamesList = Directory.GetFiles(fullDirectoryPath, "*.xml", SearchOption.TopDirectoryOnly);

         if(xmlFileNamesList.Length == 0)
         {
            webApiResult.StatusCode = -1;
            webApiResult.StatusDescription = "No XML files found";
            webApiResult.ErrorBody = $"Nema XML datoteka u direktoriju: {fullDirectoryPath}";
            return webApiResult;
         }

         List<VvMER_ResponseData> responseDataList = new List<VvMER_ResponseData>();

         foreach(string xmlFileName in xmlFileNamesList)
         {
            try
            {
               string theXmlString = File.ReadAllText(xmlFileName, Encoding.UTF8);

               if(theXmlString.IsEmpty()) continue;

               bool isCreditNote = IsXmlCreditNoteType(theXmlString);
               bool isInvoice    = IsXmlInvoiceType   (theXmlString);

               if(!isInvoice && !isCreditNote) continue; // skip non-UBL files

               string cleanedXmlString = RemoveSignatureElements(theXmlString);

               string documentNr  = null;
               string senderOIB   = null;
               string senderName  = null;
               DateTime? issueDate = null;
               decimal   money     = 0M;

               if(isInvoice)
               {
                  EN16931.UBL.InvoiceType deserializedInvoiceType = null;
                  try { deserializedInvoiceType = EN16931.UBL.InvoiceType.Deserialize(cleanedXmlString); } catch { /* skip */ }

                  if(deserializedInvoiceType != null)
                  {
                     documentNr = deserializedInvoiceType.ID?.Value;
                     issueDate  = deserializedInvoiceType.IssueDate?.Value;
                     senderOIB  = deserializedInvoiceType.VvSupplierOIB;
                     senderName = deserializedInvoiceType.AccountingSupplierParty?.Party?.PartyLegalEntity?.FirstOrDefault()?.RegistrationName?.Value
                               ?? deserializedInvoiceType.AccountingSupplierParty?.Party?.PartyName?.FirstOrDefault()?.Name?.Value;
                     money      = deserializedInvoiceType.LegalMonetaryTotal?.TaxInclusiveAmount?.Value ?? 0M;
                  }
                  else
                  {
                     // fallback: try to extract basic info from XML directly
                     //money = ExtractTaxInclusiveAmountFromXml(xmlString);
                  }
               }
               else // isCreditNote
               {
                  EN16931.UBL.CreditNoteType deserializedCreditNote = null;
                  try { deserializedCreditNote = EN16931.UBL.CreditNoteType.Deserialize(cleanedXmlString); } catch { /* skip */ }

                  if(deserializedCreditNote != null)
                  {
                     documentNr = deserializedCreditNote.ID?.Value;
                     issueDate  = deserializedCreditNote.IssueDate?.Value;
                     senderOIB  = deserializedCreditNote.VvSupplierOIB;
                     senderName = deserializedCreditNote.AccountingSupplierParty?.Party?.PartyLegalEntity?.FirstOrDefault()?.RegistrationName?.Value
                               ?? deserializedCreditNote.AccountingSupplierParty?.Party?.PartyName?.FirstOrDefault()?.Name?.Value;
                     money      = deserializedCreditNote.LegalMonetaryTotal?.TaxInclusiveAmount?.Value ?? 0M;
                  }
                  else
                  {
                     //money = ExtractTaxInclusiveAmountFromXml(xmlString);
                  }
               }

               // Filter by date range (analogno WebService filtriranju po Issued datumu)
               if(issueDate.HasValue && (issueDate.Value < queryOutbox_DateOD || issueDate.Value > queryOutbox_DateDO))
               {
                  continue;
               }

               // Build VvMER_ResponseData equivalent to what WebService returns
               VvMER_ResponseData responseData = new VvMER_ResponseData()
               {
                  ElectronicId       = 0                              , // nema electronicId kod lokalnih datoteka
                  DocumentNr         = documentNr ?? Path.GetFileNameWithoutExtension(xmlFileName),
                  SenderBusinessNumber = senderOIB                    ,
                  SenderBusinessName = senderName ?? ""               ,
                  StatusId           = 40                             , // 40 = "Preuzet"
                  StatusName         = "Preuzet",
                  Issued             = issueDate                      ,
                  Sent               = issueDate ?? File.GetCreationTime(xmlFileName),
                  Created            = File.GetCreationTime(xmlFileName),
                  DocumentXml        = theXmlString                      , // spremamo originalni XML za kasniji RECEIVE
               };

               responseDataList.Add(responseData);
            }
            catch(Exception exFile)
            {
               System.Diagnostics.Debug.WriteLine($"Error processing XML file [{xmlFileName}]: {exFile.Message}");
               // skip problematic file, continue with next
            }

         } // foreach xmlFilePath

         webApiResult.ResponseData = responseDataList;
         webApiResult.StatusCode = 200;
         webApiResult.StatusDescription = $"Loaded {responseDataList.Count} documents from disk";

         // Filter by project year (analogno VvMER_WebService_QueryOutbox_TRN_List)
         if(webApiResult.ResponseData != null)
         {
            webApiResult.ResponseData = webApiResult.ResponseData
               .Where(rd => rd.Issued.HasValue && rd.Issued.Value.Year == ZXC.projectYearAsInt)
               .ToList();
         }
      }
      catch(Exception ex)
      {
         webApiResult.StatusCode = -1;
         webApiResult.StatusDescription = "Error reading directory";
         webApiResult.ErrorBody = ex.Message;
         webApiResult.ExceptionMessage = ex.Message;
      }

      return webApiResult;
   }

   #endregion FIR

   #region FUR
   /* 111 */ internal static int Load_AUR_XtranoList(F2_Ulaz_UC theUC)
   {
      ZXC.SetStatusText("Load_AUR_XtranoList");

      theUC.TheXtranoList = new List<Xtrano>();

      int newsCount = 0;

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>();

      filterMembers.Add(new VvSqlFilterMember(ZXC.XtranoSchemaRows[ZXC.XtoCI.t_tt], "theTT" , Mixer.TT_AUR, " = " ));

      string asdDscStr;
    //string limitStr = "LIMIT " + (ZXC.RRD.Dsc_F2_NumOfRows.IsPositive() ? ZXC.RRD.Dsc_F2_NumOfRows.ToString() : "100");
      string limitStr = "LIMIT " +                                                                                "5000";

      if(ZXC.RRD.Dsc_F2_IsAsc == false) asdDscStr = " DESC ";
      else                              asdDscStr = " ASC " ;

      VvDaoBase.LoadGenericVvDataRecordList<Xtrano>(theUC.TheDbConnection, theUC.TheXtranoList, filterMembers, "", "t_dokDate " + asdDscStr + limitStr, false);

      if(theUC.TheXtranoList.NotEmpty()) theUC.PutDgvFields();

      ZXC.SetStatusText("");

      return newsCount;
   }
   /* YYY */ internal static int WS_QueryInbox_Receive_StatusInbox(F2_Ulaz_UC theUC)
   {
      #region Init

      Cursor.Current = Cursors.WaitCursor;

      ZXC.SetStatusText("Preuzimam prispjele ulazne račune u Inbox i Arhivu.");

      int newsCount = 0;

      (DateTime queryInbox_DateOD, DateTime queryInbox_DateDO) = GetF2QueryDateRange();

      string theXmlString = "";

      bool isNewXtrano, addrecOK;

      Xtrano existingXtrano, newAUR_Xtrano_rec = null;

      List<string> receiveInboxInfoList = new List<string>();
           string  receiveInboxInfo                         ;

      List<string> updatedStatusInfoList = new List<string>();
           string  updatedStatusInfo                         ;

      #endregion Init

      #region Synchronise Xtrano DataLayer (FUR Inbox i Arhiva) with provider via news from QueryInbox

      WebApiResult<List<VvMER_ResponseData>> webApiResultWithList = Vv_eRacun_HTTP.VvMER_WebService_QueryInbox_List(queryInbox_DateOD, queryInbox_DateDO);

    //if(webApiResultWithList == null || webApiResultWithList.ResponseData == null /*|| webApiResultWithList.ResponseData.IsEmpty()*/)
      if(webApiResultWithList == null || webApiResultWithList.ResponseData == null   || webApiResultWithList.ResponseData.IsEmpty()  )
      {
         if(webApiResultWithList == null)
         {
            webApiResultWithList = new WebApiResult<List<VvMER_ResponseData>>()
            {
               WebApiKind        = ZXC.F2_WebApi.InboxTRNstatusList,
               WebApiAddr        = webApiResultWithList.WebApiAddr,
               StatusCode        = -1,
               StatusDescription = "No response data",
               ErrorBody         = "No response data"
            };
         }
         else
         {
            if(webApiResultWithList.ResponseData != null && webApiResultWithList.ResponseData.IsEmpty())
            {
               webApiResultWithList.ErrorBody = "Lista je prazna";
            }
         }

       //Show_WebApiResult_ErrorMessageBox(webApiResultWithList);
         Show_WebApiResult_ErrorMessageBox<VvMER_ResponseData>(webApiResultWithList);

         //return 0;
      }

      List<VvMER_ResponseData> loopList = webApiResultWithList.ResponseData.OrderBy(rd => rd./*Created*/Sent).ToList();

      // Check for duplicate ElectronicId entries in loopList
      var duplicateElectronicIds = loopList
         .Where(rd => rd.ElectronicId.HasValue && rd.ElectronicId.Value != 0)
         .GroupBy(rd => rd.ElectronicId)
         .Where(g => g.Count() > 1)
         .Select(g => g.Key)
         .ToList();

      if(duplicateElectronicIds.NotEmpty())
      {
         string dupList = string.Join(Environment.NewLine, duplicateElectronicIds.Select(id => $"  ElectronicId: {id}"));
         ZXC.aim_emsg(MessageBoxIcon.Error,
            $"Pronađeni su duplikati ElectronicId-a u Inbox listi!{Environment.NewLine}{Environment.NewLine}" +
            $"Broj duplikata: {duplicateElectronicIds.Count}{Environment.NewLine}{Environment.NewLine}" +
            $"{dupList}{Environment.NewLine}{Environment.NewLine}" +
            $"Operacija je prekinuta.");
         return 0;
      }

      EN16931.UBL.InvoiceType    deserialized_InvoiceType    = null;
      EN16931.UBL.CreditNoteType deserialized_CreditNoteType = null;
     
      foreach(VvMER_ResponseData responseData in loopList)
      {
         Cursor.Current = Cursors.WaitCursor;

         try
         {
            existingXtrano = theUC.TheXtranoList.SingleOrDefault(xtr => xtr.F2_ElectronicID == responseData.ElectronicId);
         }
         catch(InvalidOperationException)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, $"Više od jednog Xtrano zapisa sa istim ElectronicId={responseData.ElectronicId} u lokalnoj bazi!");
            //return 0;

            // TheXtranoList sadrži više zapisa s istim F2_ElectronicID - uzimamo prvi
            existingXtrano = theUC.TheXtranoList.FirstOrDefault(xtr => xtr.F2_ElectronicID == responseData.ElectronicId);
         }

         isNewXtrano = existingXtrano == null;

         if(isNewXtrano == false) continue;

         // here we go 

         #region 1. Call RECEIVE to get theXmlString

         WebApiResult<VvMER_ResponseData> webApiResult = null;

         bool receiveOK = true;

         switch(ZXC.F2_TheProvider)
         {
            case ZXC.F2_Provider_enum.MER:
            {
               try
               {
                  webApiResult = Vv_eRacun_HTTP.VvMER_WebService_Receive_XML((uint)responseData.ElectronicId);

                  theXmlString = webApiResult.ResponseData.DocumentXml;

                  if(webApiResult.ResponseData == null || webApiResult.ResponseData.DocumentXml.IsEmpty())
                  {
                     Show_WebApiResult_ErrorMessageBox(webApiResult, responseData);
                     receiveOK = false;
                  }
               }
               catch(Exception ex)
               {
                  ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom slanja na WebServis: {0}", ex.Message);
                  receiveOK = false;
               }
               break;

            } // case ZXC.F2_Provider_enum.MER: 

            case ZXC.F2_Provider_enum.PND:
            {
               throw new NotImplementedException("Get_FISK_Status_ForElectronicID: F2 Provider PND not implemented yet.");
               receiveOK = false;
               break;
            }
         }

         #endregion 1. Call RECEIVE to get theXmlString

         #region 2. Deserialize eRacun XML document into 'InvoiceType' bussiness object & Validate XML against XSD schema

         bool isCreditNote = receiveOK && IsXmlCreditNoteType(theXmlString);
         bool isInvoice    = receiveOK && IsXmlInvoiceType   (theXmlString);

         if(isCreditNote)
         {
            deserialized_InvoiceType = null;

            deserialized_CreditNoteType = receiveOK ? GetCreditNoteTypeByDeserializing_xmlString(theXmlString, false, responseData) : null;

            if(deserialized_CreditNoteType == null) // deser. nije uspjela. Idemo pokusati barem krnjim deserialized_CreditNoteType-om omoguciti ulazak u Inbox zbogradi vidi PDF mogucnosti 
            {
               deserialized_CreditNoteType = Create_BareBone_CreditNoteType_FromProblematicXml(theXmlString);
            }
         }
         else if(isInvoice)
         {
            deserialized_CreditNoteType = null;

            deserialized_InvoiceType = receiveOK ? GetInvoiceTypeByDeserializing_xmlString(theXmlString, false, responseData) : null;

            if(deserialized_InvoiceType == null) // deser. nije uspjela. Idemo pokusati barem krnjim deserialized_InvoiceType-om omoguciti ulazak u Inbox zbogradi vidi PDF mogucnosti 
            {
               deserialized_InvoiceType = Create_BareBone_InvoiceType_FromProblematicXml(theXmlString);
            }
         }
         else
         {
            ZXC.aim_emsg(MessageBoxIcon.Error,
               $"Inbox dokument nije niti račun (InvoiceType) niti odobrenje (CreditNote).{Environment.NewLine}Dokument neće biti učitan u Arhivu ulaznih računa.{Environment.NewLine}{Environment.NewLine}" +
               $"Elektronski ID: {responseData.ElectronicId}{Environment.NewLine}" +
               $"Broj dokumenta: {responseData.DocumentNr ?? "N/A"}{Environment.NewLine}" +
               $"Pošiljatelj: {responseData.SenderBusinessName ?? "N/A"}{Environment.NewLine}" +
               $"Datum slanja: {responseData.Sent?.ToString(ZXC.VvDateFormat) ?? "N/A"}{Environment.NewLine}" +
               $"Status: {responseData.StatusName ?? responseData.StatusId?.ToString() ?? "N/A"}");

            // 27.02.2026: TODO ... vidi par redova nize 

         }

         // 02.03.2026: 
       //bool hasDeserializedDocument = (deserialized_InvoiceType != null || deserialized_CreditNoteType != null);

         #endregion 2. Deserialize eRacun XML document into 'InvoiceType' bussiness object & Validate XML against XSD schema

         #region 3. Create AUR Xtrano as ARHIVA

       //if(receiveOK && /*deserialized_eRacun != null*/hasDeserializedDocument /*&& xmlValidationOK*/)
         if(receiveOK                                                                                 )
         {
            if(isCreditNote)
            {
               newAUR_Xtrano_rec = VvMER_ResponseData.F2_eRacun_Arhiva_Set_AUR_XtranoFrom_Response_CreditNote(theXmlString, responseData, deserialized_CreditNoteType);
            }
            else if(isInvoice)
            {
               newAUR_Xtrano_rec = VvMER_ResponseData.F2_eRacun_Arhiva_Set_AUR_XtranoFrom_Response_InvoiceType(theXmlString, responseData, deserialized_InvoiceType);
            }
            else
            {
               newAUR_Xtrano_rec = VvMER_ResponseData.F2_eRacun_Arhiva_Set_AUR_XtranoFrom_Response_UNKNOWN_Type(theXmlString, responseData/*, deserialized_InvoiceType*/);
            }

            if(newAUR_Xtrano_rec != null)
            {
               byte[] T_XmlZip = newAUR_Xtrano_rec.T_XmlZip;

               addrecOK = ZXC.XtranoDao.ADDREC(theUC.TheDbConnection, newAUR_Xtrano_rec, /*false*/true, false, false, false);

               if(addrecOK)
               {
                  theUC.TheVvTabPage.TheVvForm.BeginEdit(newAUR_Xtrano_rec);

                  newAUR_Xtrano_rec.T_XmlZip = T_XmlZip;

                  VvDaoBase.Rwtrec_BLOBsingleColumn(theUC.TheDbConnection, newAUR_Xtrano_rec, "t_XmlZip", newAUR_Xtrano_rec.T_XmlZip);

                  theUC.TheVvTabPage.TheVvForm.EndEdit(newAUR_Xtrano_rec);
               }

               else //if(!addrecOK)
               {
                  ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Ne mogu dodati novi Xtrano zapis u bazu podataka za prispjeli ulazni račun (inbox). Elektronski ID: {0}", newAUR_Xtrano_rec.F2_ElectronicID);
                  continue;
               }

               theUC.TheXtranoList.Add(newAUR_Xtrano_rec);

               newsCount++;

               updatedStatusInfo = $"Novi ulazni račun u Inboxu i Arhivi. Poslano: {newAUR_Xtrano_rec.T_dokDate.ToString(ZXC.VvDateFormat)} Pošiljatelj: {newAUR_Xtrano_rec.T_opis_128}";

               //string.Format("{0} (OrigBrDok: {1}) Novi ulazni račun u Inboxu i Arhivi {2} {3} {4}",
               //                           newIFA_Faktur_rec.TipBr,
               //                           newIFA_Faktur_rec./*F2_ElectronicID*/VezniDok,
               //                           "DODANA u lokalnu bazu",
               //                           newIFA_Faktur_rec.DokDate.ToString(ZXC.VvDateFormat), newIFA_Faktur_rec.KupdobName);

               updatedStatusInfoList.Add(updatedStatusInfo);

               ZXC.SetStatusText($"{newsCount}. od {loopList.Count}: {updatedStatusInfo}");
            }
         }

         #endregion 3. Create AUR Xtrano as ARHIVA

      } // foreach(VvMER_Response_Data_AllActions responseData in webApiResultWithList.ResponseData.OrderBy(rd => rd.Created)) 

      if(theUC.TheXtranoList.NotEmpty()) theUC.PutDgvFields();

      #endregion Synchronise Xtrano DataLayer (FUR Inbox i Arhiva) with provider via news from QueryInbox

      #region Get Fiscalization Status Inbox News

      ZXC.SetStatusText("Refresh AllFISK_Inbox status");

      WebApiResult<List<VvMER_Response_Data_FiscalizationStatus>> webApiResultWithList_2 = Vv_eRacun_HTTP.VvMER_WebService_Get_FISK_Status_Inbox(queryInbox_DateOD, queryInbox_DateDO);

      if(webApiResultWithList_2 == null || webApiResultWithList_2.ResponseData == null /*|| webApiResultWithList_2.ResponseData.IsEmpty()*/) // !!! tu lutamo i ne znamo kada proglasiti error a kada ne 
      {
       //Show_WebApiResult_ErrorMessageBox(webApiResultWithList_2);
         Show_WebApiResult_ErrorMessageBox<VvMER_Response_Data_FiscalizationStatus>(webApiResultWithList_2);
         return 0;
      }

      #region 1. FISK Status - Incoming eRacun

      int wantedMessageType = 1; // 1 – Kao PRIMATELJ dohvati status fiskalizacije 

      var theXtranoList_not_FISK_yet = theUC.TheXtranoList.Where(xto => xto.F2_IsFisk != ZXC.F2_StatusInAndOutBoxEnum.DA_JE); // Lista Xtrano koje nisu fiskalizirane 

      List<VvMER_FiscalizationMessage> theFISKstatus_MessagesList = new List<VvMER_FiscalizationMessage>();
      VvMER_FiscalizationMessage lastFISKMessage;

      foreach(VvMER_Response_Data_FiscalizationStatus respData in webApiResultWithList_2.ResponseData)
      {
       //lastFISKMessage = respData.Messages.LastOrDefault(msg => msg.MessageType == wantedMessageType && (bool)msg.IsSuccess);
         lastFISKMessage = respData.Messages.LastOrDefault(msg => msg.MessageType == wantedMessageType && msg.StatusOutboxKind == ZXC.F2_StatusInAndOutBoxEnum.DA_JE);

         if(lastFISKMessage == null) continue;

         lastFISKMessage.TheElectronicId = respData.ElectronicId ?? 0L;

         theFISKstatus_MessagesList.Add(lastFISKMessage);
      }

      var theFISKstatus_Inbox_NewsList = theFISKstatus_MessagesList
          .Join(
              theXtranoList_not_FISK_yet,
              message => message.TheElectronicId,
              xto => (long)xto.F2_ElectronicID,
              (message, xto) => new
              {
                 rowIdx = theUC.TheXtranoList.IndexOf(xto),
                 xtrano = xto
              }
          );

      foreach(var theFISKstatus_Inbox_News in theFISKstatus_Inbox_NewsList)
      {
         newsCount++;

         newAUR_Xtrano_rec = theFISKstatus_Inbox_News.xtrano;

         // update Vv dataLayer 

         theUC.TheVvTabPage.TheVvForm.BeginEdit(newAUR_Xtrano_rec);

         newAUR_Xtrano_rec.F2_IsFisk = ZXC.F2_StatusInAndOutBoxEnum.DA_JE;

         // abrakadabra anuliraj umjetno podvaljeni MaxValue kao parentID ... za WHERE clausulu 
         if(newAUR_Xtrano_rec.T_parentID == UInt32.MaxValue) newAUR_Xtrano_rec.T_bak_parentID = 0;

         bool rwtOK = newAUR_Xtrano_rec.VvDao.RWTREC(theUC.TheDbConnection, newAUR_Xtrano_rec, false, false, false);

         theUC.TheVvTabPage.TheVvForm.EndEdit(newAUR_Xtrano_rec);

         if(rwtOK)
         {
            theUC.PutDgvLineFields(theFISKstatus_Inbox_News.rowIdx, newAUR_Xtrano_rec); // osvjezi prikaz 
            updatedStatusInfo = string.Format("Nova FISKALIZACIJA ulaznog računa: {0} ({1}) {2} {3}",
                                          newAUR_Xtrano_rec.T_theString,
                                          newAUR_Xtrano_rec.F2_ElectronicID,
                                          newAUR_Xtrano_rec.T_dokDate.ToString(ZXC.VvDateFormat), 
                                          newAUR_Xtrano_rec.T_opis_128);

            updatedStatusInfoList.Add(updatedStatusInfo);

         } // if(rwtOK)

      } // foreach(var item in theFISKstatus_Outbox_NewsList) 

      #endregion 1. FISK Status - Outgoing eRacun

      #region 2. REJECT Status - Incoming eRacun

      wantedMessageType = 2; // 2 – Dohvati status odbijanja 

      var theXtranoList_not_REJECT_yet = theUC.TheXtranoList.Where(xto => xto.F2_IsReject == false); // Lista Xtrano koje nisu rejectani 

      List<VvMER_FiscalizationMessage> theREJECTstatus_MessagesList = new List<VvMER_FiscalizationMessage>();
      VvMER_FiscalizationMessage lastREJECTMessage;

      foreach(VvMER_Response_Data_FiscalizationStatus respData in webApiResultWithList_2.ResponseData)
      {
         lastREJECTMessage = respData.Messages.LastOrDefault(msg => msg.MessageType == wantedMessageType && msg.StatusOutboxKind == ZXC.F2_StatusInAndOutBoxEnum.DA_JE);

         if(lastREJECTMessage == null) continue;

         lastREJECTMessage.TheElectronicId = respData.ElectronicId ?? 0L;

         theREJECTstatus_MessagesList.Add(lastREJECTMessage);
      }

      var theREJECTstatus_Inbox_NewsList = theREJECTstatus_MessagesList
          .Join(
              theXtranoList_not_REJECT_yet,
              message => message.TheElectronicId,
              xto => (long)xto.F2_ElectronicID,
              (message, xto) => new
              {
                 rowIdx = theUC.TheXtranoList.IndexOf(xto),
                 xtrano = xto
              }
          );

      foreach(var theREJECTstatus_Inbox_News in theREJECTstatus_Inbox_NewsList)
      {
         newsCount++;

         newAUR_Xtrano_rec = theREJECTstatus_Inbox_News.xtrano;

         // update Vv dataLayer 

         theUC.TheVvTabPage.TheVvForm.BeginEdit(newAUR_Xtrano_rec);

         newAUR_Xtrano_rec.F2_IsReject = true;

         bool rwtOK = newAUR_Xtrano_rec.VvDao.RWTREC(theUC.TheDbConnection, newAUR_Xtrano_rec, false, false, false);

         theUC.TheVvTabPage.TheVvForm.EndEdit(newAUR_Xtrano_rec);

         if(rwtOK)
         {
            theUC.PutDgvLineFields(theREJECTstatus_Inbox_News.rowIdx, newAUR_Xtrano_rec); // osvjezi prikaz 
            updatedStatusInfo = string.Format("Novo ! ODBIJANJE ! ulaznog računa: {0} ({1}) {2} {3}",
                                          newAUR_Xtrano_rec.T_theString,
                                          newAUR_Xtrano_rec.F2_ElectronicID,
                                          newAUR_Xtrano_rec.T_dokDate.ToString(ZXC.VvDateFormat), 
                                          newAUR_Xtrano_rec.T_opis_128);

            updatedStatusInfoList.Add(updatedStatusInfo);

         } // if(rwtOK)

      } // foreach(var item in theFISKstatus_Outbox_NewsList) 

      #endregion 2. REJECT Status - Incoming eRacun

      #endregion Get Fiscalization Status Inbox News

      #region Finish

      Cursor.Current = Cursors.Default;

      ZXC.SetStatusText("");

      if(updatedStatusInfoList.NotEmpty())
      {
         Load_AUR_XtranoList(theUC);

         ZXC.aim_emsg_List(string.Format("Ima {0} novosti.", updatedStatusInfoList.Count), updatedStatusInfoList);
      }

      return newsCount;

      #endregion Finish
   }
   private static InvoiceType Create_BareBone_InvoiceType_FromProblematicXml(string theXmlString)
   {
      InvoiceType deserialized_InvoiceType = new InvoiceType();

      deserialized_InvoiceType.LegalMonetaryTotal = new MonetaryTotalType
      {
         TaxInclusiveAmount = new TaxInclusiveAmountType { Value = VvMER_ResponseData.ExtractTaxInclusiveAmountFromXml(theXmlString) }
      };

      deserialized_InvoiceType.IssueDate = new IssueDateType { Value = VvMER_ResponseData.ExtractIssueDateFromXml(theXmlString) }; //BT-2 Datum izdavanja Datum        1..1 

      return deserialized_InvoiceType;
   }
   private static CreditNoteType Create_BareBone_CreditNoteType_FromProblematicXml(string theXmlString)
   {
      CreditNoteType deserialized_CreditNoteType = new CreditNoteType();

      deserialized_CreditNoteType.LegalMonetaryTotal = new MonetaryTotalType
      {
         TaxInclusiveAmount = new TaxInclusiveAmountType { Value = VvMER_ResponseData.ExtractTaxInclusiveAmountFromXml(theXmlString) }
      };

      deserialized_CreditNoteType.IssueDate = new IssueDateType { Value = VvMER_ResponseData.ExtractIssueDateFromXml(theXmlString) }; //BT-2 Datum izdavanja Datum        1..1 

      return deserialized_CreditNoteType;
   }
   /* ZZZ */ internal static int Import_FUR_Fakturs_JOB(F2_Ulaz_UC theUC)
   {
      #region Init & Get Dialog Fields AND Create newFaktur_List 

      EN16931.UBL.InvoiceType    deserialized_InvoiceType    = null;
      EN16931.UBL.CreditNoteType deserialized_CreditNoteType = null;

      int newsCount = 0;

      bool addrecKupdobOK, foundKupdobOK;

      string theOIB, theXmlString;
      Kupdob kupdob_rec;
      Faktur newUFA_Faktur_rec = null;

      List<string> newKupdobInfoList = new List<string>();
           string  newKupdobInfo                         ;

      List<string> updatedStatusInfoList = new List<string>();
           string  updatedStatusInfo                         ;

      List<Xtrano> XtranosForImport_List = 

         theUC.TheXtranoList.Where(xto => xto.T_parentID.IsZero()).OrderBy(xto => xto.T_dokDate) // TODO ... ovdje ce se ubuduce nalaziti i TIME komponenta pa bu OK 
                                                                //.ThenBy(xto => xto.T_dokNum)
                                                                  .ToList();                     // tamo gdje je T_parentID  0, znaci da jos nije u Faktur DataLayer-u         
                                                                                                 // T_parentID NotZero moze biti normalan ili UInt32.MaxValue!                 
                                                                                                 // notZero - faktur nastao importom ... UInt32.MaxValue - faktur nastao ručno 

      List<VvReportSourceUtil> messageList = new List<VvReportSourceUtil>();

      if(XtranosForImport_List.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Information, "Nema ništa za import.");
         return 0;
      }

      foreach(Xtrano xtranoForImport_rec in XtranosForImport_List)
      {
         messageList.Add(new VvReportSourceUtil()
         {
            KupdobCD   = xtranoForImport_rec.F2_ElectronicID,
            TheDate    = xtranoForImport_rec.T_dokDate,
            KupdobName = xtranoForImport_rec.T_opis_128,
            String1    = xtranoForImport_rec.T_theString,
            String2    = xtranoForImport_rec.T_konto,
            String3    = xtranoForImport_rec.T_devName,
            TheMoney   = xtranoForImport_rec.T_moneyA,
            TheDate2   = xtranoForImport_rec.T_dokDate2,

            UtilUint   = xtranoForImport_rec.T_recID,
         });
      } 

      VvMessageBoxDLG  FUR_ImportCandidatesXtranoList_InfoDLG = new VvMessageBoxDLG (false, ZXC.VvmBoxKind.F2_IMPORT_candidates);
      FUR_ImportCandidatesXtranoList_InfoDLG.Text = "Kandidati za IMPORT:";

      FUR_ImportCandidatesXtranoList_InfoDLG.TheUC.PutDgvFields_F2_IMPORT_candidates(messageList);
      FUR_ImportCandidatesXtranoList_InfoDLG.TheUC.Fld_IsAutoImport = ZXC.RRD.Dsc_F2_IsAutoImport;

      DialogResult dlgResult = FUR_ImportCandidatesXtranoList_InfoDLG.ShowDialog();

      if(dlgResult != DialogResult.OK)
      {
         FUR_ImportCandidatesXtranoList_InfoDLG.Dispose();
         return -1;
      }

      if(ZXC.RRD.Dsc_F2_IsAutoImport != FUR_ImportCandidatesXtranoList_InfoDLG.TheUC.Fld_IsAutoImport)
      {
         ZXC.RRD.Dsc_F2_IsAutoImport = FUR_ImportCandidatesXtranoList_InfoDLG.TheUC.Fld_IsAutoImport; 
         ZXC.RRD.SaveDscToLookUpItemList();
      }

      int numOfFirstLinesOnly = FUR_ImportCandidatesXtranoList_InfoDLG.TheUC.Fld_NumOfFirstLinesOnly_Import;

      #region Izbaci 'preskočene'

      bool shouldSkip;
      uint xtoRecIDtoSkip;
      int  foundCount;

      for(int rIdx = 0; rIdx < FUR_ImportCandidatesXtranoList_InfoDLG.TheUC.TheGrid.RowCount /*- 1*/; ++rIdx)
      {
         shouldSkip = FUR_ImportCandidatesXtranoList_InfoDLG.TheUC.TheGrid.GetBoolCell(FUR_ImportCandidatesXtranoList_InfoDLG.TheUC.DgvCI.iT_shouldS, rIdx, false);

         if(shouldSkip)
         {
            xtoRecIDtoSkip = FUR_ImportCandidatesXtranoList_InfoDLG.TheUC.TheGrid.GetUint32Cell(FUR_ImportCandidatesXtranoList_InfoDLG.TheUC.DgvCI.iT_ftrRecID, rIdx, false);
            
            foundCount = XtranosForImport_List.RemoveAll(xto => xto.T_recID == xtoRecIDtoSkip);

            if(foundCount.IsZero()) ZXC.aim_emsg(MessageBoxIcon.Error, "shouldSkip FUR_Import line NOT FOUND!");
         }
      }

      #endregion Izbaci 'preskočene'

      FUR_ImportCandidatesXtranoList_InfoDLG.Dispose();

      Cursor.Current = Cursors.WaitCursor;

      ZXC.SetStatusText("Importiram ulazne račune u Vektorov Data Layer.");

      #endregion Init & Get Dialog Fields AND Create MAP_requestDataList 

      #region ADDREC Faktur Loop

      foreach(Xtrano AURxtrano_rec in XtranosForImport_List)
      {
         theXmlString = VvStringCompressor.DecompressXml(AURxtrano_rec.T_XmlZip);

         bool isCreditNote = IsXmlCreditNoteType(theXmlString);
         bool isInvoice    = IsXmlInvoiceType   (theXmlString);

         if(isCreditNote)
         {
            deserialized_CreditNoteType = GetCreditNoteTypeByDeserializing_xmlString(theXmlString, false);
            deserialized_InvoiceType = null;
         }
         else if(isInvoice)
         {
            deserialized_InvoiceType = GetInvoiceTypeByDeserializing_xmlString(theXmlString, false);
            deserialized_CreditNoteType = null;
         }
         else
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Nije niti račun (InvoiceType) niti odobrenje (CreditNote)?!");
         }

         bool hasDeserializedDocument = (deserialized_InvoiceType != null || deserialized_CreditNoteType != null);

         if(hasDeserializedDocument == false)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, $"{AURxtrano_rec.To_AUR_String}\n\r\n\rNe mogu deserijalizirati xml iz Inbox-a.\n\r\n\rRačun nije moguće na ovaj način importirati u Vektor.\n\r\n\rDodajte ga ručno.");
            continue;
         }

         #region Get Kupdob / New Kupdob?

         addrecKupdobOK = false;

         theOIB = AURxtrano_rec.T_konto;

         kupdob_rec = theUC.Get_Kupdob_FromVvUcSifrar_byOIB(theOIB);

         if(kupdob_rec != null) foundKupdobOK = true;
         else                   foundKupdobOK = false;

         if(foundKupdobOK == false) // try to create NEW Kupdob from eRacun data 
         {
            if(isInvoice)
            {
               kupdob_rec = EN16931.UBL.InvoiceType.Create_Kupdob_from_InvoiceType(theUC.TheDbConnection, deserialized_InvoiceType, false);
            }
            else if(isCreditNote)
            {
               kupdob_rec = EN16931.UBL.CreditNoteType.Create_Kupdob_from_CreditNote(theUC.TheDbConnection, deserialized_CreditNoteType, false);
            }

            if(kupdob_rec != null) // NEW Kupdob created ok 
            {
               addrecKupdobOK = kupdob_rec.VvDao.ADDREC(theUC.TheDbConnection, kupdob_rec);

               if(addrecKupdobOK)
               {
                  newKupdobInfo = string.Format("Novi kupac [{0}],  OIB: [{1}], Ulica: {2}, Mjesto: {3}", kupdob_rec.Naziv, kupdob_rec.Oib, kupdob_rec.Ulica1, kupdob_rec.ZipAndMjesto);

                  newKupdobInfoList.Add(newKupdobInfo);

                  theUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

                  foundKupdobOK = true;
               }
               else
               {
                  ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom ADDREC novog kupca (kupdob) iz eRačuna s eID={0} za OIB [{1}].", AURxtrano_rec.F2_ElectronicID, theOIB);
                  foundKupdobOK = false;
               }
            }
            else
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom kreiranja novog kupca (kupdob) iz eRačuna s eID={0} za OIB [{1}].", AURxtrano_rec.F2_ElectronicID, theOIB);
            }
         }

         #endregion Get Kupdob / New Kupdob?

         if(foundKupdobOK || addrecKupdobOK)
         {
            // 2. Create new Faktur bussiness object record from 'InvoiceType' in XML document 

            if(isInvoice) // InvoiceType 
            {
               newUFA_Faktur_rec = deserialized_InvoiceType.Create_Faktur_From_InvoiceType(theUC.TheDbConnection, AURxtrano_rec.F2_ElectronicID, AURxtrano_rec.T_dokDate, kupdob_rec, false, AURxtrano_rec.T_recID);
            }
            else if(isCreditNote) // CreditNoteType 
            {
               newUFA_Faktur_rec = deserialized_CreditNoteType.Create_Faktur_From_CreditNoteType(theUC.TheDbConnection, AURxtrano_rec.F2_ElectronicID, AURxtrano_rec.T_dokDate, kupdob_rec, false, AURxtrano_rec.T_recID);
            }

            if(newUFA_Faktur_rec != null)
            {
               newsCount++;

               updatedStatusInfo = string.Format("{0} (OrigBrDok: {1}) Nova UFA je {2} {3} {4}",
                                             newUFA_Faktur_rec.TipBr,
                                             newUFA_Faktur_rec./*F2_ElectronicID*/VezniDok,
                                             "DODANA u lokalnu bazu",
                                             newUFA_Faktur_rec.DokDate.ToString(ZXC.VvDateFormat), newUFA_Faktur_rec.KupdobName);

               updatedStatusInfoList.Add(updatedStatusInfo);

               ZXC.SetStatusText($"{newsCount}. od {XtranosForImport_List.Count}: {updatedStatusInfo}");
            }

            // now RWTREC Xtrano for FEEDBACK 
            // T_parentID      = faktur_rec.RecID          , 
            // T_ttNum         = faktur_rec.TtNum          , 
            theUC.TheVvTabPage.TheVvForm.BeginEdit(AURxtrano_rec);

            AURxtrano_rec.T_parentID = newUFA_Faktur_rec.RecID;
            AURxtrano_rec.T_ttNum    = newUFA_Faktur_rec.TtNum;

            AURxtrano_rec.VvDao.RWTREC(theUC.TheDbConnection, AURxtrano_rec, false, false, false);

            theUC.TheVvTabPage.TheVvForm.EndEdit(AURxtrano_rec);

         } // if(kupdobOK || addrecKupdobOK) 
         
         else // Nema postojeceg kupdoba ili addrec novog kupdoba nije uspio 
         { 
            // ??? !!! 
         }

      } // foreach(Xtrano AURxtrano_rec in XtranosForImport_List)

      #endregion ADDREC Faktur Loop

      #region Finish

      ZXC.SetStatusText("");

      Cursor.Current = Cursors.Default;

      if(updatedStatusInfoList.NotEmpty())
      {
         ZXC.aim_emsg_List(string.Format("DODANO je {0} novih ulaznih računa u Vektorovu bazu podataka.", updatedStatusInfoList.Count), updatedStatusInfoList);
         
         theUC.PutDgvFields();
      }
      
      if(newKupdobInfoList.NotEmpty())
      {
         ZXC.aim_emsg_List(string.Format("DODANO je {0} novih partnera (kupaca) u Vektorovu bazu podataka.", newKupdobInfoList.Count), newKupdobInfoList);
      }

      return newsCount;

      #endregion Finish
   }
   /* KKK */ internal static int HDD_Import_Extern_Faktur_UFA(F2_Ulaz_UC theUC)
   {
      #region Init

      Cursor.Current = Cursors.WaitCursor;

      ZXC.SetStatusText("Preuzimanje ulaznih računa u Inbox i Arhivu SA LOKALNOG DISKA.");

      int newsCount = 0;

      (DateTime queryInbox_DateOD, DateTime queryInbox_DateDO) = GetF2QueryDateRange();

      string theXmlString = "";

      bool isNewXtrano, addrecOK;

      Xtrano existingXtrano, newAUR_Xtrano_rec = null;

      List<string> receiveInboxInfoList = new List<string>();
           string  receiveInboxInfo                         ;

      List<string> updatedStatusInfoList = new List<string>();
           string  updatedStatusInfo                         ;

      string fullDirectoryPath;

      using(OpenFileDialog openFileDialog = new OpenFileDialog())
      {
         string initialDir = (ZXC.TheVvForm.VvPref.theHDD_Import_Extern_Faktur_UFA_Prefs.DirectoryName.IsEmpty() ||
                              ZXC.TheVvForm.VvPref.theHDD_Import_Extern_Faktur_UFA_Prefs.DirectoryName == @".\"   ) ? 
                              Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) :
                              ZXC.TheVvForm.VvPref.theHDD_Import_Extern_Faktur_UFA_Prefs.DirectoryName;

         openFileDialog.Title           = "Odaberite direktorij sa XML datotekama ULAZNIH računa";
         openFileDialog.ValidateNames   = false;
         openFileDialog.CheckFileExists = false;
         openFileDialog.CheckPathExists = true;
         openFileDialog.FileName        = "Odaberite directory";
         openFileDialog.DefaultExt      = ".xml";
       //openFileDialog.ReadOnlyChecked = true;
       //openFileDialog.ShowReadOnly    = true;

         if(Directory.Exists(initialDir))
         {
            openFileDialog.InitialDirectory = initialDir;
         }

         DialogResult result = openFileDialog.ShowDialog();

         if(result != DialogResult.OK || string.IsNullOrEmpty(openFileDialog.FileName))
         {
            ZXC.SetStatusText("");
            Cursor.Current = Cursors.Default;
            return 0;
         }

         ZXC.TheVvForm.VvPref.theHDD_Import_Extern_Faktur_UFA_Prefs.DirectoryName =

         fullDirectoryPath = Path.GetDirectoryName(openFileDialog.FileName);
      }

      bool thisXMLhaswrongOIB = false;
      bool wrongOIBdetected   = false;

      #endregion Init

      #region Synchronise Xtrano DataLayer (FUR Inbox i Arhiva) with provider via news from QueryInbox

    //WebApiResult<List<VvMER_ResponseData>> webApiResultWithList = Vv_eRacun_HTTP.VvMER_WebService_QueryInbox_List(                   queryInbox_DateOD, queryInbox_DateDO);
      WebApiResult<List<VvMER_ResponseData>> webApiResultWithList = Vv_eRacun_HTTP.VvMER_LocalHDD_QueryInbox_List  (fullDirectoryPath, queryInbox_DateOD, queryInbox_DateDO);

    //if(webApiResultWithList == null || webApiResultWithList.ResponseData == null /*|| webApiResultWithList.ResponseData.IsEmpty()*/)
      if(webApiResultWithList == null || webApiResultWithList.ResponseData == null   || webApiResultWithList.ResponseData.IsEmpty()  )
      {
         if(webApiResultWithList == null)
         {
            webApiResultWithList = new WebApiResult<List<VvMER_ResponseData>>()
            {
               WebApiKind        = ZXC.F2_WebApi.InboxTRNstatusList,
               WebApiAddr        = webApiResultWithList.WebApiAddr,
               StatusCode        = -1,
               StatusDescription = "No response data",
               ErrorBody         = "No response data"
            };
         }
         else
         {
            if(webApiResultWithList.ResponseData != null && webApiResultWithList.ResponseData.IsEmpty())
            {
               webApiResultWithList.ErrorBody = "Lista je prazna";
            }
         }

       //Show_WebApiResult_ErrorMessageBox(webApiResultWithList);
         Show_WebApiResult_ErrorMessageBox<VvMER_ResponseData>(webApiResultWithList);

         //return 0;
      }

      List<VvMER_ResponseData> loopList = webApiResultWithList.ResponseData.OrderBy(rd => rd./*Created*/Sent).ToList();

      EN16931.UBL.InvoiceType    deserialized_InvoiceType    = null;
      EN16931.UBL.CreditNoteType deserialized_CreditNoteType = null;

      foreach(VvMER_ResponseData responseData in loopList)
      {
         Cursor.Current = Cursors.WaitCursor;

       //existingXtrano = theUC.TheXtranoList.SingleOrDefault(xtr => xtr.F2_ElectronicID == responseData.ElectronicId);
         existingXtrano = theUC.TheXtranoList.SingleOrDefault(xtr => xtr.T_theString     == responseData.DocumentNr  );

         isNewXtrano = existingXtrano == null;

         if(isNewXtrano == false) continue;

         // here we go 

         #region 1. Call RECEIVE to get full XML document

         // NIJE RECEIVE API nego xml dode iz fajla z diska 
         theXmlString = responseData.DocumentXml;

         bool receiveOK = true;

         #endregion 1. Call RECEIVE to get full XML document

         #region 2. Deserialize eRacun XML document into 'InvoiceType' bussiness object & Validate XML against XSD schema

         bool isCreditNote = receiveOK && IsXmlCreditNoteType(theXmlString);
         bool isInvoice    = receiveOK && IsXmlInvoiceType   (theXmlString);

         if(isCreditNote)
         {
            deserialized_InvoiceType = null;

            deserialized_CreditNoteType = receiveOK ? GetCreditNoteTypeByDeserializing_xmlString(theXmlString, false, responseData) : null;

            if(deserialized_CreditNoteType == null) // deser. nije uspjela. Idemo pokusati barem krnjim deserialized_CreditNoteType-om omoguciti ulazak u Inbox zbogradi vidi PDF mogucnosti 
            {
               deserialized_CreditNoteType = Create_BareBone_CreditNoteType_FromProblematicXml(theXmlString);
            }
            
            thisXMLhaswrongOIB = ZXC.CURR_prjkt_rec.Oib != deserialized_CreditNoteType.VvCustomerOIB;

         }
         else if(isInvoice)
         {
            deserialized_CreditNoteType = null;

            deserialized_InvoiceType = receiveOK ? GetInvoiceTypeByDeserializing_xmlString(theXmlString, false, responseData) : null;

            if(deserialized_InvoiceType == null) // deser. nije uspjela. Idemo pokusati barem krnjim deserialized_InvoiceType-om omoguciti ulazak u Inbox zbogradi vidi PDF mogucnosti 
            {
               deserialized_InvoiceType = Create_BareBone_InvoiceType_FromProblematicXml(theXmlString);
            }

            thisXMLhaswrongOIB = ZXC.CURR_prjkt_rec.Oib != deserialized_InvoiceType.VvCustomerOIB;

         }
         else
         {
            ZXC.aim_emsg(MessageBoxIcon.Error,
               $"Inbox dokument nije niti račun (InvoiceType) niti odobrenje (CreditNote).{Environment.NewLine}Dokument neće biti učitan u Arhivu ulaznih računa.{Environment.NewLine}{Environment.NewLine}" +
               $"Elektronski ID: {responseData.ElectronicId}{Environment.NewLine}" +
               $"Broj dokumenta: {responseData.DocumentNr ?? "N/A"}{Environment.NewLine}" +
               $"Pošiljatelj: {responseData.SenderBusinessName ?? "N/A"}{Environment.NewLine}" +
               $"Datum slanja: {responseData.Sent?.ToString(ZXC.VvDateFormat) ?? "N/A"}{Environment.NewLine}" +
               $"Status: {responseData.StatusName ?? responseData.StatusId?.ToString() ?? "N/A"}");
         }

         if(thisXMLhaswrongOIB)
         {
            wrongOIBdetected = true;
            continue;
         }

         bool hasDeserializedDocument = (deserialized_InvoiceType != null || deserialized_CreditNoteType != null);

         #endregion 2. Deserialize eRacun XML document into 'InvoiceType' bussiness object & Validate XML against XSD schema

         #region 3. Create AUR Xtrano as ARHIVA

         if(receiveOK && /*deserialized_eRacun != null*/hasDeserializedDocument /*&& xmlValidationOK*/)
         {
            if(isCreditNote)
            {
               newAUR_Xtrano_rec = VvMER_ResponseData.F2_eRacun_Arhiva_Set_AUR_XtranoFrom_Response_CreditNote(theXmlString, responseData, deserialized_CreditNoteType);
            }
            else if(isInvoice)
            {
               newAUR_Xtrano_rec = VvMER_ResponseData.F2_eRacun_Arhiva_Set_AUR_XtranoFrom_Response_InvoiceType(theXmlString, responseData, deserialized_InvoiceType);
            }
            else
            {
               newAUR_Xtrano_rec = VvMER_ResponseData.F2_eRacun_Arhiva_Set_AUR_XtranoFrom_Response_UNKNOWN_Type(theXmlString, responseData/*, deserialized_InvoiceType*/);
            }

            if(newAUR_Xtrano_rec != null)
            {
               byte[] T_XmlZip = newAUR_Xtrano_rec.T_XmlZip;

               addrecOK = ZXC.XtranoDao.ADDREC(theUC.TheDbConnection, newAUR_Xtrano_rec, /*false*/true, false, false, false);

               if(addrecOK)
               {
                  theUC.TheVvTabPage.TheVvForm.BeginEdit(newAUR_Xtrano_rec);

                  newAUR_Xtrano_rec.T_XmlZip = T_XmlZip;

                  VvDaoBase.Rwtrec_BLOBsingleColumn(theUC.TheDbConnection, newAUR_Xtrano_rec, "t_XmlZip", newAUR_Xtrano_rec.T_XmlZip);

                  theUC.TheVvTabPage.TheVvForm.EndEdit(newAUR_Xtrano_rec);
               }

               else //if(!addrecOK)
               {
                  ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Ne mogu dodati novi Xtrano zapis u bazu podataka za prispjeli ulazni račun (inbox). Elektronski ID: {0}", newAUR_Xtrano_rec.F2_ElectronicID);
                  continue;
               }

               theUC.TheXtranoList.Add(newAUR_Xtrano_rec);

               newsCount++;

               updatedStatusInfo = $"Novi ulazni račun u Inboxu i Arhivi. Poslano: {newAUR_Xtrano_rec.T_dokDate.ToString(ZXC.VvDateFormat)} Pošiljatelj: {newAUR_Xtrano_rec.T_opis_128}";

               //string.Format("{0} (OrigBrDok: {1}) Novi ulazni račun u Inboxu i Arhivi {2} {3} {4}",
               //                           newIFA_Faktur_rec.TipBr,
               //                           newIFA_Faktur_rec./*F2_ElectronicID*/VezniDok,
               //                           "DODANA u lokalnu bazu",
               //                           newIFA_Faktur_rec.DokDate.ToString(ZXC.VvDateFormat), newIFA_Faktur_rec.KupdobName);

               updatedStatusInfoList.Add(updatedStatusInfo);

               ZXC.SetStatusText($"{newsCount}. od {loopList.Count}: {updatedStatusInfo}");
            }
         }

         #endregion 3. Create AUR Xtrano as ARHIVA

      } // foreach(VvMER_Response_Data_AllActions responseData in webApiResultWithList.ResponseData.OrderBy(rd => rd.Created)) 

      if(theUC.TheXtranoList.NotEmpty()) theUC.PutDgvFields();

      #endregion Synchronise Xtrano DataLayer (FUR Inbox i Arhiva) with provider via news from QueryInbox

      #region Finish

      Cursor.Current = Cursors.Default;

      ZXC.SetStatusText("");

      if(updatedStatusInfoList.NotEmpty())
      {
         Load_AUR_XtranoList(theUC);

         ZXC.aim_emsg_List(string.Format("Ima {0} novosti.", updatedStatusInfoList.Count), updatedStatusInfoList);
      }

      if(newsCount.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Information, "Nema novosti.");
      }

      if(wrongOIBdetected)
      {
         ZXC.aim_emsg(MessageBoxIcon.Exclamation, "Pronađene su datoteke sa neodgovarajućim OIB-om!\n\r\n\rKrivi projekt ili krivo odabrani directory?");
      }

      return newsCount;

      #endregion Finish
   }
   private static WebApiResult<List<VvMER_ResponseData>> VvMER_LocalHDD_QueryInbox_List(string fullDirectoryPath, DateTime queryInbox_DateOD, DateTime queryInbox_DateDO)
   {
      WebApiResult<List<VvMER_ResponseData>> webApiResult = new WebApiResult<List<VvMER_ResponseData>>()
      {
         WebApiKind = ZXC.F2_WebApi.HDD_InboxListAsKnjigServis,
         WebApiAddr = fullDirectoryPath,
      };

      try
      {
         // Validate directory
         if(string.IsNullOrEmpty(fullDirectoryPath) || !Directory.Exists(fullDirectoryPath))
         {
            webApiResult.StatusCode = -1;
            webApiResult.StatusDescription = "Directory not found";
            webApiResult.ErrorBody = $"Direktorij ne postoji: {fullDirectoryPath}";
            return webApiResult;
         }

         // Get all XML files from directory
         string[] xmlFileNamesList = Directory.GetFiles(fullDirectoryPath, "*.xml", SearchOption.TopDirectoryOnly);

         if(xmlFileNamesList.Length == 0)
         {
            webApiResult.StatusCode = -1;
            webApiResult.StatusDescription = "No XML files found";
            webApiResult.ErrorBody = $"Nema XML datoteka u direktoriju: {fullDirectoryPath}";
            return webApiResult;
         }

         List<VvMER_ResponseData> responseDataList = new List<VvMER_ResponseData>();

         foreach(string xmlFileName in xmlFileNamesList)
         {
            try
            {
               string theXmlString = File.ReadAllText(xmlFileName, Encoding.UTF8);

               if(theXmlString.IsEmpty()) continue;

               bool isCreditNote = IsXmlCreditNoteType(theXmlString);
               bool isInvoice    = IsXmlInvoiceType   (theXmlString);

               if(!isInvoice && !isCreditNote) continue; // skip non-UBL files

               string cleanedXmlString = RemoveSignatureElements(theXmlString);

               string documentNr  = null;
               string senderOIB   = null;
               string senderName  = null;
               DateTime? issueDate = null;
               decimal   money     = 0M;

               if(isInvoice)
               {
                  EN16931.UBL.InvoiceType deserialized = null;
                  try { deserialized = EN16931.UBL.InvoiceType.Deserialize(cleanedXmlString); } catch { /* skip */ }

                  if(deserialized != null)
                  {
                     documentNr = deserialized.ID?.Value;
                     issueDate  = deserialized.IssueDate?.Value;
                     senderOIB  = deserialized.VvSupplierOIB;
                     senderName = deserialized.AccountingSupplierParty?.Party?.PartyLegalEntity?.FirstOrDefault()?.RegistrationName?.Value
                               ?? deserialized.AccountingSupplierParty?.Party?.PartyName?.FirstOrDefault()?.Name?.Value;
                     money      = deserialized.LegalMonetaryTotal?.TaxInclusiveAmount?.Value ?? 0M;
                  }
                  else
                  {
                     // fallback: try to extract basic info from XML directly
                     //money = ExtractTaxInclusiveAmountFromXml(xmlString);
                  }
               }
               else // isCreditNote
               {
                  EN16931.UBL.CreditNoteType deserialized = null;
                  try { deserialized = EN16931.UBL.CreditNoteType.Deserialize(cleanedXmlString); } catch { /* skip */ }

                  if(deserialized != null)
                  {
                     documentNr = deserialized.ID?.Value;
                     issueDate  = deserialized.IssueDate?.Value;
                     senderOIB  = deserialized.VvSupplierOIB;
                     senderName = deserialized.AccountingSupplierParty?.Party?.PartyLegalEntity?.FirstOrDefault()?.RegistrationName?.Value
                               ?? deserialized.AccountingSupplierParty?.Party?.PartyName?.FirstOrDefault()?.Name?.Value;
                     money      = deserialized.LegalMonetaryTotal?.TaxInclusiveAmount?.Value ?? 0M;
                  }
                  else
                  {
                     //money = ExtractTaxInclusiveAmountFromXml(xmlString);
                  }
               }

               // Filter by date range (analogno WebService filtriranju po Issued datumu)
               if(issueDate.HasValue && (issueDate.Value < queryInbox_DateOD || issueDate.Value > queryInbox_DateDO))
               {
                  continue;
               }

               // Build VvMER_ResponseData equivalent to what WebService returns
               VvMER_ResponseData responseData = new VvMER_ResponseData()
               {
                  ElectronicId       = 0                              , // nema electronicId kod lokalnih datoteka
                  DocumentNr         = documentNr ?? Path.GetFileNameWithoutExtension(xmlFileName),
                  SenderBusinessNumber = senderOIB                    ,
                  SenderBusinessName = senderName ?? ""               ,
                  StatusId           = 40                             , // 40 = "Preuzet"
                  StatusName         = "Preuzet",
                  Issued             = issueDate                      ,
                  Sent               = issueDate ?? File.GetCreationTime(xmlFileName),
                  Created            = File.GetCreationTime(xmlFileName),
                  DocumentXml        = theXmlString                      , // spremamo originalni XML za kasniji RECEIVE
               };

               responseDataList.Add(responseData);
            }
            catch(Exception exFile)
            {
               System.Diagnostics.Debug.WriteLine($"Error processing XML file [{xmlFileName}]: {exFile.Message}");
               // skip problematic file, continue with next
            }

         } // foreach xmlFilePath

         webApiResult.ResponseData = responseDataList;
         webApiResult.StatusCode = 200;
         webApiResult.StatusDescription = $"Loaded {responseDataList.Count} documents from disk";

         // Filter by project year (analogno VvMER_WebService_QueryOutbox_TRN_List)
         if(webApiResult.ResponseData != null)
         {
            webApiResult.ResponseData = webApiResult.ResponseData
               .Where(rd => rd.Issued.HasValue && rd.Issued.Value.Year == ZXC.projectYearAsInt)
               .ToList();
         }
      }
      catch(Exception ex)
      {
         webApiResult.StatusCode = -1;
         webApiResult.StatusDescription = "Error reading directory";
         webApiResult.ErrorBody = ex.Message;
         webApiResult.ExceptionMessage = ex.Message;
      }

      return webApiResult;
   }

   #endregion FUR

   #region Other
   private static bool ShouldCheckRefreshed_TRN_Or_DPS_Status(Faktur F2_IRn_faktur_rec, bool isDPS)
   {
      return !ShouldSkipRefreshing_TRN_Or_DPS_Status(F2_IRn_faktur_rec, isDPS);
   }
   private static bool ShouldSkipRefreshing_TRN_Or_DPS_Status(Faktur F2_IRn_faktur_rec, bool isDPS)
   {
      if(F2_IRn_faktur_rec.IsF2 == false) return true;

      if( isDPS && F2_IRn_faktur_rec.F2_QueryOutbox_HasNoSense_Refresh_DPS_Status) return true; // DPS 
      if(!isDPS && F2_IRn_faktur_rec.F2_QueryOutbox_HasNoSense_Refresh_TRN_Status) return true; // TRN 

      return false; // Placeholder
   }
   internal static void WS_QueryInbox_DPS(F2_Ulaz_UC theUC)
   {
#if false
      Faktur F2_URn_faktur_rec;

      List<string> updatedStatusInfoList = new List<string>();
           string  updatedStatusInfo                         ;

      Cursor.Current = Cursors.WaitCursor;

      for(int rIdx = 0; rIdx < theUC.TheG.RowCount; ++rIdx)
      {
         F2_URn_faktur_rec = theUC.TheFakturList[rIdx];

         if(F2_URn_faktur_rec.IsF2 == false) continue;

         if( isDPS && F2_URn_faktur_rec.F2_Outbox_IsNoSense_Refresh_DPS_Status) continue; // DPS 
         if(!isDPS && F2_URn_faktur_rec.F2_Outbox_IsNoSense_Refresh_TRN_Status) continue; // TRN 

         int justRefreshedStatusCD = Get_QueryOutboxStatus_ForElectronicID(F2_URn_faktur_rec.F2_ElectronicID/*MER_ElectronicID*/, isDPS);

         if(justRefreshedStatusCD.IsPositive() && justRefreshedStatusCD != F2_URn_faktur_rec.F2_StatusCD)
         {
            // update Vv dataLayer 

            theUC.TheVvTabPage.TheVvForm.BeginEdit(F2_URn_faktur_rec);
            
            F2_URn_faktur_rec.F2_StatusCD = justRefreshedStatusCD;

            bool rwtOK = true; // ZXC.FakturRec.VvDao.RWTREC(TheDbConnection, F2_IRn_faktur_rec, false, true, false); upali ovo 

            theUC.TheVvTabPage.TheVvForm.EndEdit(F2_URn_faktur_rec);

            if(rwtOK)
            { 
               theUC.PutDgvLineFields(rIdx, F2_URn_faktur_rec); // osvjezi prikaz 

               updatedStatusInfo = string.Format("{0} ({1}) Novi status:      {2}      {3} {4}",
                                             F2_URn_faktur_rec.TipBr,
                                             F2_URn_faktur_rec.F2_ElectronicID/*MER_ElectronicID*/,
                                             Vv_eRacun_HTTP.MER_TransportStatuses[justRefreshedStatusCD],
                                             F2_URn_faktur_rec.DokDate.ToString(ZXC.VvDateFormat), F2_URn_faktur_rec.KupdobName);

               updatedStatusInfoList.Add(updatedStatusInfo);

            } // if(rwtOK)

         }

      } // for(int rIdx = 0; rIdx < theUC.TheG.RowCount; ++rIdx)

      Cursor.Current = Cursors.Default;

      if(updatedStatusInfoList.NotEmpty())
      {
         ZXC.aim_emsg_List(string.Format("Dohvatio {0} novih statusa.", updatedStatusInfoList.Count), updatedStatusInfoList);
      }
#endif
   }

   // 5.5 MessageType                                             
   // 0 – As SENDER get the status of the fiscalization message   
   // 1 – As RECIPIENT get the status of the fiscalization message
   // 2 – Get the rejection status                                
   // 3 – Get the payment status                                  
   private static bool WS_Get_FISK_or_REJECTED_or_MARKAsP_Status_ForElectronicID(uint electronicID, string messageType)
   {
      WebApiResult<VvMER_ResponseData> webApiResult = null;
      bool getStatusOK = true;

      switch(ZXC.F2_TheProvider)
      {
         case ZXC.F2_Provider_enum.MER:
         {
            try
            {
               webApiResult = Vv_eRacun_HTTP.VvMER_WebService_Get_FISK_Status(electronicID, messageType);
               // TODO: Vv_eRacun_HTTP.Show_WebApiResult_ErrorMessageBox(webApiResult, ZXC.F2_WebApi.xyz);

               if(webApiResult == null) getStatusOK = false;
            }
            catch(Exception ex)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom slanja na WebServis: {0}", ex.Message);
               getStatusOK = false;
            }
            if(getStatusOK)
            {
               return (bool)webApiResult.ResponseData.IsSuccess;
            }
            else
            {
               return false;
            }

         } // case ZXC.F2_Provider_enum.MER:

         case ZXC.F2_Provider_enum.PND:
         {
            throw new NotImplementedException("Get_FISK_Status_ForElectronicID: F2 Provider PND not implemented yet.");
         }
      }

      return false;
   }
   private static uint WS_Get_RECEIVE_Izlaz_Document2Arhiva_ForElectronicID(F2_Izlaz_UC theUC, uint electronicID, Faktur F2_IRn_faktur_rec)
   {
      WebApiResult<VvMER_ResponseData> webApiResult = null;

      bool receiveOK = true;

      uint arhivaXtrano_recID = 0;

      switch(ZXC.F2_TheProvider)
      {
         case ZXC.F2_Provider_enum.MER:
         {
            try
            {
               webApiResult = Vv_eRacun_HTTP.VvMER_WebService_Receive_XML(electronicID);

               // za provjeru: pokušaj deserijalizirati kao InvoiceType ili CreditNoteType 
               string theXmlString = webApiResult.ResponseData?.DocumentXml;

               bool isCreditNote = theXmlString.NotEmpty() && IsXmlCreditNoteType(theXmlString);
               bool isInvoice    = theXmlString.NotEmpty() && IsXmlInvoiceType   (theXmlString);

               bool deserializationOK = false;

               if(isInvoice)
               {
                  EN16931.UBL.InvoiceType deserialized_InvoiceType = GetInvoiceTypeByDeserializing_xmlString(theXmlString, true);
                  deserializationOK = deserialized_InvoiceType != null;
               }
               else if(isCreditNote)
               {
                  EN16931.UBL.CreditNoteType deserialized_CreditNoteType = GetCreditNoteTypeByDeserializing_xmlString(theXmlString, true);
                  deserializationOK = deserialized_CreditNoteType != null;
               }
               else
               {
                  ZXC.aim_emsg(MessageBoxIcon.Error,
                     $"Outbox dokument nije niti račun (InvoiceType) niti odobrenje (CreditNote).{Environment.NewLine}Dokument neće biti učitan u IFA-e.{Environment.NewLine}{Environment.NewLine}" +
                     $"Elektronski ID: {webApiResult.ResponseData.ElectronicId}{Environment.NewLine}" +
                     $"Broj dokumenta: {webApiResult.ResponseData.DocumentNr ?? "N/A"}{Environment.NewLine}" +
                     $"Pošiljatelj: {webApiResult.ResponseData.SenderBusinessName ?? "N/A"}{Environment.NewLine}" +
                     $"Datum slanja: {webApiResult.ResponseData.Sent?.ToString(ZXC.VvDateFormat) ?? "N/A"}{Environment.NewLine}" +
                     $"Status: {webApiResult.ResponseData.StatusName ?? webApiResult.ResponseData.StatusId?.ToString() ?? "N/A"}");
               }
               if(webApiResult.ResponseData == null || theXmlString.IsEmpty() || !deserializationOK)
               {
                  Show_WebApiResult_ErrorMessageBox(webApiResult);
                  receiveOK = false;
               }
            }
            catch(Exception ex)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom slanja na WebServis: {0}", ex.Message);
               receiveOK = false;
            }

            if(receiveOK)
            {
               Xtrano F2arhivaXtrano_rec = VvMER_ResponseData.F2_eRacun_Arhiva_Set_AIR_XtranoFrom_XmlDocument_And_Faktur(webApiResult.ResponseData.DocumentXml, F2_IRn_faktur_rec);

               if(F2arhivaXtrano_rec != null)
               {
                  byte[] T_XmlZip = F2arhivaXtrano_rec.T_XmlZip;

                  bool OK = ZXC.XtranoDao.ADDREC(theUC.TheDbConnection, F2arhivaXtrano_rec, /*false*/true, false, false, false);

                  if(OK)
                  {
                     theUC.TheVvTabPage.TheVvForm.BeginEdit(F2arhivaXtrano_rec);

                     F2arhivaXtrano_rec.T_XmlZip = T_XmlZip;

                     VvDaoBase.Rwtrec_BLOBsingleColumn(theUC.TheDbConnection, F2arhivaXtrano_rec, "t_XmlZip", F2arhivaXtrano_rec.T_XmlZip);

                     theUC.TheVvTabPage.TheVvForm.EndEdit(F2arhivaXtrano_rec);
                  }

                  if(OK)
                  {
                     arhivaXtrano_recID = F2arhivaXtrano_rec.T_recID;
                  }
               }
            }

            return arhivaXtrano_recID;

         } // case ZXC.F2_Provider_enum.MER: 

         case ZXC.F2_Provider_enum.PND:
         {
            throw new NotImplementedException("Get_FISK_Status_ForElectronicID: F2 Provider PND not implemented yet.");
         }
      }

      return 0;
   }
   private static uint HDD_Get_Izlaz_Document2Arhiva(F2_Izlaz_UC theUC, string theXmlString, Faktur F2_IRn_faktur_rec)
   {
      Xtrano F2arhivaXtrano_rec = VvMER_ResponseData.F2_eRacun_Arhiva_Set_AIR_XtranoFrom_XmlDocument_And_Faktur(theXmlString, F2_IRn_faktur_rec);

      uint arhivaXtrano_recID = 0;

      if(F2arhivaXtrano_rec != null)
      {
         byte[] T_XmlZip = F2arhivaXtrano_rec.T_XmlZip;

         bool OK = ZXC.XtranoDao.ADDREC(theUC.TheDbConnection, F2arhivaXtrano_rec, /*false*/true, false, false, false);

         if(OK)
         {
            theUC.TheVvTabPage.TheVvForm.BeginEdit(F2arhivaXtrano_rec);

            F2arhivaXtrano_rec.T_XmlZip = T_XmlZip;

            VvDaoBase.Rwtrec_BLOBsingleColumn(theUC.TheDbConnection, F2arhivaXtrano_rec, "t_XmlZip", F2arhivaXtrano_rec.T_XmlZip);

            theUC.TheVvTabPage.TheVvForm.EndEdit(F2arhivaXtrano_rec);
         }

         if(OK)
         {
            arhivaXtrano_recID = F2arhivaXtrano_rec.T_recID;
         }
      }

      return arhivaXtrano_recID;
   }
   private static WebApiResult<VvMER_ResponseData> WS_Mark_Paid_With_OR_Without_ElectronicID(VvMER_RequestData MAP_requestData, bool isWithElectronicID)
   {
      WebApiResult<VvMER_ResponseData> webApiResult = null;
      bool MAP_OK = true;

      switch(ZXC.F2_TheProvider)
      {
         case ZXC.F2_Provider_enum.MER:
         {
            try
            {
               if(isWithElectronicID) webApiResult = Vv_eRacun_HTTP.VvMER_WebService_MAP       (MAP_requestData);
               else                   webApiResult = Vv_eRacun_HTTP.VvMER_WebService_MAP_WO_eID(MAP_requestData);

               // TODO: Vv_eRacun_HTTP.Show_WebApiResult_ErrorMessageBox(webApiResult, ZXC.F2_WebApi.xyz);

               if(webApiResult == null) MAP_OK = false;
            }
            catch(Exception ex)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom slanja na WebServis: {0}", ex.Message);
               MAP_OK = false;
            }
            if(MAP_OK)
            {
               return       webApiResult          ;
             //return (bool)responseData.IsSuccess;
            }
            else
            {
             //return false;
               return null ;
            }

         } // case ZXC.F2_Provider_enum.MER:

         case ZXC.F2_Provider_enum.PND:
         {
            throw new NotImplementedException("WS_Mark_Paid_WithElectronicID: F2 Provider PND not implemented yet.");
         }
      }

    //return false;
      return null ;
   }
   internal static void Show_WebApiResult_ErrorMessageBox<T>(WebApiResult<List<T>> webApiResultWithList, string xmlFilePath = null) where T : class
   {
      WebApiResult<T> webApiResult = WebApiResult<T>.GetWebApiResult_From_WebApiResultWithList(webApiResultWithList);
   
      Show_WebApiResult_ErrorMessageBox(webApiResult, null, xmlFilePath);
   }
   
   internal static void Show_WebApiResult_ErrorMessageBox<T>(WebApiResult<T> webApiResult, VvMER_ResponseData responseData = null, string xmlFilePath = null) where T : class
   {
      VvMessageBoxDLG Send_OR_eIzvj_ErrorMessageBox = new VvMessageBoxDLG(false, ZXC.VvmBoxKind.F2_webApiResults);
   
      switch(webApiResult.WebApiKind)
      {
         case ZXC.F2_WebApi.SEND               : Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom 'SEND' slanja eRačuna:"                        ; break;
         case ZXC.F2_WebApi.eIzvj              : Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom 'eIzvještavanje' slanja eRačuna:"              ; break;
         case ZXC.F2_WebApi.OutboxTRNstatus    : Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom dohvata transportnog statusa eRačuna:"         ; break;
         case ZXC.F2_WebApi.OutboxDPSstatus    : Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom dohvata procesnog statusa eRačuna:"            ; break;
         case ZXC.F2_WebApi.OutboxTRNstatusList: Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom dohvata liste transportnih statusa eRačuna:"   ; break;
         case ZXC.F2_WebApi.OutboxDPSstatusList: Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom dohvata liste procesnih statusa eRačuna:"      ; break;
         case ZXC.F2_WebApi.InboxDPSstatusList : Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom dohvata DPS statusa ULAZNIH eRačuna:"          ; break;
         case ZXC.F2_WebApi.InboxTRNstatusList : Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom dohvata TRN statusa ULAZNIH eRačuna:"          ; break;
         case ZXC.F2_WebApi.FISK_singleStatus  : Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom dohvata statusa FISKALIZACIJE eRačuna:"        ; break;
         case ZXC.F2_WebApi.FISKstatusOutbox   : Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom dohvata liste statusa FISKALIZACIJE eRačuna:"  ; break;
         case ZXC.F2_WebApi.FISKstatusInbox    : Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom dohvata liste statusa FISKALIZACIJE eRačuna:"  ; break;
         case ZXC.F2_WebApi.REJECTstatus       : Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom dohvata statusa ODBIJANJA eRačuna:"            ; break;
         case ZXC.F2_WebApi.MAPstatus          : Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom dohvata statusa NAPLATE eRačuna:"              ; break;
         case ZXC.F2_WebApi.MAPaction          : Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom akcije izvještavanja NAPLATE eRačuna:"         ; break;
         case ZXC.F2_WebApi.MAPaction_WO_eID   : Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom akcije izvještavanja NAPLATE eRačuna bez ID-a:"; break;
         case ZXC.F2_WebApi.RECEIVEdocument    : Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom preuzimanja eRačuna:"                          ; break;
         case ZXC.F2_WebApi.PING               : Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom spajanja na servis:"                           ; break;
         case ZXC.F2_WebApi.CheckAMS           : Send_OR_eIzvj_ErrorMessageBox.Text = webApiResult.WebApiKind.ToString() + " [" + webApiResult.WebApiAddr + "] " + " - Greška prilikom dohvata info o AMS statusu partnera:"          ; break;
      }
   
      Send_OR_eIzvj_ErrorMessageBox.TextForSupportMailFromAddition = webApiResult.WebApiKind.ToString();
   
      // Set the XML file attachment path if provided
      if (xmlFilePath.NotEmpty() && System.IO.File.Exists(xmlFilePath))
      {
         Send_OR_eIzvj_ErrorMessageBox.AttachmentFilePath = xmlFilePath;
      }
   
      #region dodao naknadno ako ima i responseData jos podataka
   
      // Get the existing message list
      List<string> allMessages = new List<string>(webApiResult.MessageList);
   
      if(responseData != null)
      {
         allMessages.Add(Environment.NewLine + "---- Response Data ----" + Environment.NewLine);
   
         if(responseData.ElectronicId.HasValue)
         {
            allMessages.Add(string.Format("ElectronicId: {0}{1}", responseData.ElectronicId.Value, Environment.NewLine));
         }
         if(responseData.SenderBusinessName.NotEmpty())
         {
            allMessages.Add(string.Format("SenderBusinessName: {0}{1}", responseData.SenderBusinessName, Environment.NewLine));
         }
         if(!string.IsNullOrEmpty(responseData.DocumentNr))
         {
            allMessages.Add(string.Format("DocumentNr: {0}{1}", responseData.DocumentNr, Environment.NewLine));
         }
         if(responseData.Sent.HasValue)
         {
            allMessages.Add(string.Format("Sent: {0}{1}", responseData.Sent, Environment.NewLine));
         }
         // dodati jos po potrebi 
      }
   
      #endregion dodao naknadno ako ima i responseData jos podataka
   
      for(int i = 0; i < allMessages.Count; ++i)
      {
         Send_OR_eIzvj_ErrorMessageBox.TextForSupportMailBody += allMessages[i] + Environment.NewLine;
      }
   
      Send_OR_eIzvj_ErrorMessageBox.TheUC.PutDgvFields(allMessages);
      DialogResult dlgResult = Send_OR_eIzvj_ErrorMessageBox.ShowDialog();
      Send_OR_eIzvj_ErrorMessageBox.Dispose();
   }   
   internal static EN16931.UBL.InvoiceType GetInvoiceTypeByDeserializing_xmlString_OLD(string xmlString, bool beSilent)
   {
      EN16931.UBL.InvoiceType theInvoiceType = null;

      try
      {
         // Ukloni UBLExtensions elemente prije deserializacije
         string cleanedXmlString = RemoveSignatureElements(xmlString);

         theInvoiceType = EN16931.UBL.InvoiceType.Deserialize(/*xmlString*/cleanedXmlString);
      }
      catch(Exception ex)
      {
         if(!beSilent)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error,
               "Greška prilikom deserializacije InvoiceType iz XML stringa:\n\n{0}\n\nInner: {1}",
               ex.Message,
               ex.InnerException?.Message);
         }
      }

      return theInvoiceType;
   }
   internal static EN16931.UBL.InvoiceType GetInvoiceTypeByDeserializing_xmlString(string xmlString, bool beSilent, VvMER_ResponseData responseData = null)
   {
      EN16931.UBL.InvoiceType theInvoiceType = null;

      try
      {
         // Ukloni UBLExtensions elemente prije deserializacije
         string cleanedXmlString = RemoveSignatureElements(xmlString);

         theInvoiceType = EN16931.UBL.InvoiceType.Deserialize(cleanedXmlString);
      }
      catch(Exception ex)
      {
         if(!beSilent)
         {
            string cleanedXmlString = RemoveSignatureElements(xmlString);

            // Save XML to temp file for attachment
            string debugPath = Path.Combine(@"C:\temp", $"failed_invoice_deserialization_{DateTime.Now:yyyyMMdd_HHmmss}.xml");
            System.IO.Directory.CreateDirectory(@"C:\temp");
            System.IO.File.WriteAllText(debugPath, /*xmlString*/cleanedXmlString, System.Text.Encoding.UTF8);

            // Create WebApiResult for error display
            WebApiResult<VvMER_ResponseData> webApiResult = new WebApiResult<VvMER_ResponseData>
            {
               WebApiKind = ZXC.F2_WebApi.RECEIVEdocument,
               WebApiAddr = "Deserialization",
               StatusCode = -1,
               StatusDescription = "XML Deserialization Failed",
               ErrorBody = $"Ne mogu deserijalizirati InvoiceType iz XML stringa.\n\r\n\r" +
                           $"U PITANJU JE GREŠKA SA STRANE POŠILJATELJA RAČUNA.\n\r\n\r" +
                           (responseData != null? $"{responseData.SenderBusinessName} Rn. Br.:{responseData.DocumentNr} ElectronicID: {responseData.ElectronicId}\n\r\n\r" : "") +
                           $"Zamolite dobavljača da ovaj račun stornira/otkaže\n\rte nanovo pošalje ispravan eRačun.\n\r",
               ExceptionMessage = $"Exception: {ex.Message}\n\nInner Exception: {ex.InnerException?.Message}",
               ResponseData = new VvMER_ResponseData()
            };

            Show_WebApiResult_ErrorMessageBox(webApiResult, null, debugPath);
         }
      }

      return theInvoiceType;
   }
   internal static EN16931.UBL.CreditNoteType GetCreditNoteTypeByDeserializing_xmlString_OLD(string xmlString, bool beSilent)
   {
      EN16931.UBL.CreditNoteType theCreditNoteType = null;

      try
      {
         // Ukloni UBLExtensions elemente prije deserializacije
         string cleanedXmlString = RemoveSignatureElements(xmlString);

         //System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(EN16931.UBL.CreditNoteType));
         //using(StringReader sr = new StringReader(cleanedXmlString))
         //{
         //   theCreditNoteType = (EN16931.UBL.CreditNoteType)serializer.Deserialize(System.Xml.XmlReader.Create(sr));
         //}

         theCreditNoteType = EN16931.UBL.CreditNoteType.Deserialize(/*xmlString*/cleanedXmlString);

      }
      catch(Exception ex)
      {
         if(!beSilent)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error,
               "Greška prilikom deserializacije CreditNote iz XML stringa:\n\n{0}\n\nInner: {1}",
               ex.Message,
               ex.InnerException?.Message);
         }
      }

      return theCreditNoteType;
   }
   internal static EN16931.UBL.CreditNoteType GetCreditNoteTypeByDeserializing_xmlString(string xmlString, bool beSilent, VvMER_ResponseData responseData = null)
   {
      EN16931.UBL.CreditNoteType theCreditNoteType = null;

      try
      {
         // Ukloni UBLExtensions elemente prije deserializacije
         string cleanedXmlString = RemoveSignatureElements(xmlString);

         theCreditNoteType = EN16931.UBL.CreditNoteType.Deserialize(cleanedXmlString);
      }
      catch(Exception ex)
      {
         if(!beSilent)
         {
            string cleanedXmlString = RemoveSignatureElements(xmlString);

            // Save XML to temp file for attachment
            string debugPath = Path.Combine(@"C:\temp", $"failed_creditnote_deserialization_{DateTime.Now:yyyyMMdd_HHmmss}.xml");
            System.IO.Directory.CreateDirectory(@"C:\temp");
            System.IO.File.WriteAllText(debugPath, /*xmlString*/cleanedXmlString, System.Text.Encoding.UTF8);

            // Create WebApiResult for error display
            WebApiResult<VvMER_ResponseData> webApiResult = new WebApiResult<VvMER_ResponseData>
            {
               WebApiKind = ZXC.F2_WebApi.RECEIVEdocument,
               WebApiAddr = "Deserialization",
               StatusCode = -1,
               StatusDescription = "XML Deserialization Failed",
               ErrorBody = $"Ne mogu deserijalizirati CreditNote iz XML stringa\n\r\n\r" +
                           $"U PITANJU JE GREŠKA SA STRANE POŠILJATELJA RAČUNA.\n\r\n\r" +
                           (responseData != null ? $"{responseData.SenderBusinessName} Rn. Br.:{responseData.DocumentNr} ElectronicID: {responseData.ElectronicId}\n\r\n\r" : "") +
                           $"Zamolite dobavljača da ovaj račun stornira/otkaže\n\rte nanovo pošalje ispravan eRačun.\n\r",

               ExceptionMessage = $"Exception: {ex.Message}\n\nInner Exception: {ex.InnerException?.Message}",
               ResponseData = new VvMER_ResponseData()
            };

            Show_WebApiResult_ErrorMessageBox(webApiResult, null, debugPath);
         }
      }

      return theCreditNoteType;
   }
   public /*private*/ static string RemoveSignatureElements(string xmlString)
   {
      try
      {
         XDocument doc = XDocument.Parse(xmlString);

         // Define namespaces
         XNamespace ext = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";
         XNamespace sig = "urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2";
         XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";

         // Ukloni UBLDocumentSignatures elemente
         doc.Descendants(sig + "UBLDocumentSignatures").Remove();

         // Ukloni sve ds:Signature elemente
         doc.Descendants(ds + "Signature").Remove();

         // Ukloni prazne UBLExtension elemente nakon uklanjanja potpisa
         var emptyExtensions = doc.Descendants(ext + "UBLExtension")
            .Where(x => !x.HasElements || x.Elements().All(e => string.IsNullOrWhiteSpace(e.Value)))
            .ToList();
         emptyExtensions.ForEach(x => x.Remove());

         return doc.ToString();
      }
      catch(Exception ex)
      {
         System.Diagnostics.Debug.WriteLine($"Error removing signature elements: {ex.Message}");
         return xmlString;
      }
   }

   private static bool IsXmlInvoiceType(string theXmlString)
   {
      if(string.IsNullOrEmpty(theXmlString)) return false;
      string trimmed = theXmlString.TrimStart();
      return trimmed.Contains("<Invoice ")
          || trimmed.Contains("<Invoice>")
          || trimmed.Contains(":Invoice ")
          || trimmed.Contains(":Invoice>")
          || trimmed.Contains("urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
   }

   private static bool IsXmlCreditNoteType(string theXmlString)
   {
      if(string.IsNullOrEmpty(theXmlString)) return false;
      string trimmed = theXmlString.TrimStart();
      return trimmed.Contains("<CreditNote ")
          || trimmed.Contains("<CreditNote>")
          || trimmed.Contains(":CreditNote ")
          || trimmed.Contains(":CreditNote>")
          || trimmed.Contains("urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2");
   }
   
   #endregion Other

   #region NIR

   /* AAA */
   internal static int Load_MAP_FtransList(F2_NIR_UC theUC)
   {
      ZXC.SetStatusText("Load_MAP_FtransList");

      Faktur MAP_CandidateFaktur_rec;

      List<VvReportSourceUtil> messageList = new List<VvReportSourceUtil>();

      string thePaymentMethod = "T"; // T je default a dole se jos postavlja prema TT-u 

    //theUC.TheFtransList = FtransDao.Get_MAP_FtransList       (theUC.TheDbConnection).OrderBy(ftr => ftr.T_dokNum).ToList(); // ftrans 'MAP' kandidati: naplate od KUPACa koje nisu jos MAPane 
      theUC.TheFtransList = FtransDao.Get_Zatvaranja_FtransList(theUC.TheDbConnection)
         .OrderByDescending(ftr => ftr.T_dokDate)
         .ThenByDescending (ftr => ftr.T_dokNum )
         .ThenByDescending (ftr => ftr.T_serial )
         .ToList(); // ftrans     zatvaranja / naplate od KUPACa                      

      int zeroFakYearCount = theUC.TheFtransList.Count(ftr => ftr.T_fakYear.IsZero());

      if(zeroFakYearCount.IsPositive()) ZXC.aim_emsg(MessageBoxIcon.Warning, $"Postoji {zeroFakYearCount} zatvaranja sa oznakom T_FakYear nula!?");

      // Remove one koji su prije 2026 
      theUC.TheFtransList.RemoveAll(ftr => ftr.T_fakYear.NotZero() && ftr.T_fakYear < 2026);

      if(theUC.TheFtransList.NotEmpty()) theUC.PutDgvFields();

      int newsCount = 0;

      ZXC.SetStatusText("");

      return newsCount;
   }

   /* NNN */internal static int Create_MAP_XML_From_NIR(F2_NIR_UC theUC, bool isByDates = false, DateTime? dateOD = null, DateTime? dateDO = null)
   {
      #region Init & Get Dialog Fields AND Create MAP_XML_List 

      string theXmlString;

      int newsCount = 0;

      List<(Ftrans ftrans, Faktur faktur)> MAP_ActionsList = new List<(Ftrans ftrans, Faktur faktur)>();

      Faktur MAP_CandidateFaktur_rec;

      List<VvReportSourceUtil> messageList = new List<VvReportSourceUtil>();

      List<Ftrans> paymentftransList;

      string thePaymentMethod = "T"; // T je default a dole se jos postavlja prema TT-u 

      if(isByDates)
      {
         paymentftransList = FtransDao.Get_Zatvaranja_FtransList_byDate(theUC.TheDbConnection, dateOD.Value, dateDO.Value).OrderBy(ftr => ftr.T_dokNum).ToList(); // ftrans 'MAP' kandidati: naplate od KUPACa koje nisu jos MAPane 
      }
      else
      {
         paymentftransList = FtransDao.Get_MAP_FtransList(theUC.TheDbConnection).OrderBy(ftr => ftr.T_dokNum).ToList(); // ftrans 'MAP' kandidati: naplate od KUPACa koje nisu jos MAPane 
      }

      if(paymentftransList.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Information, "Nema ništa za slanje.");
         return 0;
      }

      int zeroFakYearCount = paymentftransList.Count(ftr => ftr.T_fakYear.IsZero());

      if(zeroFakYearCount.IsPositive()) ZXC.aim_emsg(MessageBoxIcon.Warning, $"Postoji {zeroFakYearCount} zatvaranja sa oznakom T_FakYear nula!?");

      // Remove one koji su prije 2026 
      paymentftransList.RemoveAll(ftr => /*ftr.T_fakYear.NotZero() &&*/ ftr.T_fakYear < 2026);

      foreach(Ftrans paymentftrans_rec in paymentftransList)
      {
         MAP_CandidateFaktur_rec = GetFakturFromPaymentFtrans(paymentftrans_rec, theUC.TheDbConnection, out thePaymentMethod);

         if(MAP_CandidateFaktur_rec == null)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Ne mogu pronaći fakturu za MAP!? Ftrans: {0}", paymentftrans_rec);
            continue; 
         }

         MAP_ActionsList.Add((paymentftrans_rec, MAP_CandidateFaktur_rec));

         messageList.Add(new VvReportSourceUtil()
         {
          //IsNekakav  = MAP_CandidateFaktur_rec.Is_MAP_with_ElectronicID,

            TheCD      = MAP_CandidateFaktur_rec.TipBr,
            DevName    = MAP_CandidateFaktur_rec.DokDate.ToString(ZXC.VvDateFormat),
            KupdobName = MAP_CandidateFaktur_rec.KupdobName,
            TheMoney   = MAP_CandidateFaktur_rec.S_ukKCRP,

            String1    = paymentftrans_rec.T_dokDate.ToString(ZXC.VvDateFormat),
            TheMoney2  = paymentftrans_rec.T_pot,
            String2    = thePaymentMethod,
            String3    = paymentftrans_rec.T_dokNum.ToString() + "/" + paymentftrans_rec.T_serial.ToString(),
            String4    = paymentftrans_rec.T_opis,
            String5    = paymentftrans_rec.T_konto,

            UtilUint   = paymentftrans_rec.T_recID

         });

      } // foreach(Ftrans paymentftrans_rec in paymentftransList) 

      VvMessageBoxDLG  MAP_CandidatesFtransList_InfoDLG = new VvMessageBoxDLG (false, ZXC.VvmBoxKind.F2_MAP_candidates);
      MAP_CandidatesFtransList_InfoDLG.Text = "Kandidati za slanje prijave plaćanja:";

      MAP_CandidatesFtransList_InfoDLG.TheUC.PutDgvFields_F2_MAP_candidates(messageList);
      MAP_CandidatesFtransList_InfoDLG.TheUC.Fld_IsAutoMAP = ZXC.RRD.Dsc_F2_IsAutoMAP;

      DialogResult dlgResult = MAP_CandidatesFtransList_InfoDLG.ShowDialog();

      if(dlgResult != DialogResult.OK)
      {
         MAP_CandidatesFtransList_InfoDLG.Dispose();
         return -1;
      }

      int numOfFirstLinesOnly   =  MAP_CandidatesFtransList_InfoDLG.TheUC.Fld_NumOfFirstLinesOnly_MAP;

      #region Izbaci 'preskočene'

      bool shouldSkip;
      uint ftrRecIDtoSkip;
      int foundCount;

      for(int rIdx = 0; rIdx < MAP_CandidatesFtransList_InfoDLG.TheUC.TheGrid.RowCount /*- 1*/; ++rIdx)
      {
         shouldSkip = MAP_CandidatesFtransList_InfoDLG.TheUC.TheGrid.GetBoolCell(MAP_CandidatesFtransList_InfoDLG.TheUC.DgvCI.iT_shouldS, rIdx, false);

         if(shouldSkip)
         {
            ftrRecIDtoSkip = MAP_CandidatesFtransList_InfoDLG.TheUC.TheGrid.GetUint32Cell(MAP_CandidatesFtransList_InfoDLG.TheUC.DgvCI.iT_ftrRecID, rIdx, false);
            
            foundCount = MAP_ActionsList.RemoveAll(MAPal => MAPal.ftrans.T_recID == ftrRecIDtoSkip);

            if(foundCount.IsZero()) ZXC.aim_emsg(MessageBoxIcon.Error, "shouldSkip MAP_action NOT FOUND!");
         }
      }

      #endregion Izbaci 'preskočene'

      MAP_CandidatesFtransList_InfoDLG.Dispose();

      Cursor.Current = Cursors.WaitCursor;

      ZXC.SetStatusText("Create_MAP_XML_From_NIR");

      int mapCount = 0; bool MAP_OK;

      System.Diagnostics.Stopwatch dispatchStopWatch = System.Diagnostics.Stopwatch.StartNew();

      uint soFarCount      = 0;
      // int ofTotalCount    = numOfFirstLinesOnly.NotZero() ? numOfFirstLinesOnly : paymentftransList.Count;
      //long elapsedTicks    = 0, remainTicks;
      //decimal soFarKoef       ;
      //TimeSpan elapsedTime = new TimeSpan(0);
      //TimeSpan remainTime     ;
      //string statusText       ;
      //
      //WebApiResult<VvMER_ResponseData> webApiResult;

      Xtrano F2_MAP_Xtrano_rec;

      #endregion Init & Get Dialog Fields AND Create MAP_ActionsList 

      #region The MAP API Loop - foreach MAP_ActionsList

      EN16931.UBL.QWE2.EvidentirajNaplatuZahtjev theNaplataZahtjev = EN16931.UBL.QWE2.EvidentirajNaplatuZahtjev.Create_MAP_XML_ZAGLAVLJE();

      EN16931.UBL.QWE2.Naplata theNaplata;

      List<EN16931.UBL.QWE2.Naplata> naplataList = new List<EN16931.UBL.QWE2.Naplata>();

      foreach((Ftrans ftrans, Faktur faktur) MAP_Action in MAP_ActionsList)
      {
         Cursor.Current = Cursors.WaitCursor;

         #region SET XML Line

         //webApiResult = WS_Mark_Paid_With_OR_Without_ElectronicID(MAP_Action.request, true /*MAP_Action.faktur.Is_MAP_with_ElectronicID*/);
         //
         MAP_OK       = true;

         theNaplata = EN16931.UBL.QWE2.EvidentirajNaplatuZahtjev.Create_MAP_XML_Naplata_From_MAPaction(MAP_Action);

         naplataList.Add(theNaplata);

         if(MAP_OK)
         {
            mapCount++;
         
            F2_MAP_Xtrano_rec = VvMER_ResponseData.F2_MAPtrans_SetXtranoFrom_Ftrans(MAP_Action.ftrans, MAP_Action.faktur/*, webApiResult.ResponseData*/);
         
            if(F2_MAP_Xtrano_rec != null)
            {
             //byte[] T_XmlZip = F2_MAP_Xtrano_rec.T_XmlZip;
         
               bool OK = ZXC.XtranoDao.ADDREC(theUC.TheDbConnection, F2_MAP_Xtrano_rec, /*false*/true, false, false, false);
         
               if(OK)
               {
                  newsCount++;
         
                  // 14.02.2026: ugaseno jer vise ne spremamo T_XmlZip za 'MAP' Xtrano 
                //theUC.TheVvTabPage.TheVvForm.BeginEdit(F2_MAP_Xtrano_rec);
                //
                //F2_MAP_Xtrano_rec.T_XmlZip = T_XmlZip;
                //
                //VvDaoBase.Rwtrec_BLOBsingleColumn(theUC.TheDbConnection, F2_MAP_Xtrano_rec, "t_XmlZip", F2_MAP_Xtrano_rec.T_XmlZip);
                //
                //theUC.TheVvTabPage.TheVvForm.EndEdit(F2_MAP_Xtrano_rec);
               }
            }
         }

         #endregion SET XML Line

         #region set status text

         //soFarCount++;
         //
         //#region soFar vs remaining calc
         //
         //soFarKoef     = ZXC.DivSafe(soFarCount, ofTotalCount);
         //elapsedTicks += dispatchStopWatch.Elapsed.Ticks          ;
         //elapsedTime  += dispatchStopWatch.Elapsed                ;
         //remainTicks   = (long)(ZXC.DivSafe((decimal)elapsedTicks, soFarKoef) - elapsedTicks);
         //remainTime    = new TimeSpan(remainTicks);
         //
         //#endregion soFar vs remaining calc
         //
         //statusText =
         //   dispatchStopWatch.Elapsed.TotalSeconds.ToString1Vv() + "s " +
         //   "(" + (elapsedTime.TotalSeconds / (double)soFarCount).ToString1Vv() + "s avg) done " +
         //    (/*++*/soFarCount).ToString() +
         //    " of " + ofTotalCount +
         //    " (" + (soFarKoef * 100M).ToString0Vv() + "%)" +
         //   //" <"   + remainTime + "> "                              +
         //    string.Format(" remain <{0:00}:{1:00}:{2:00}> ", remainTime.Hours, remainTime.Minutes, remainTime.Seconds) +
         //    " " + MAP_Action.ftrans.ToString();
         //
         //dispatchStopWatch.Restart();
         //
         //ZXC.SetStatusText(statusText); Cursor.Current = Cursors.WaitCursor;

         #endregion set status text

         if(numOfFirstLinesOnly.NotZero() && /*sendCount*/soFarCount == numOfFirstLinesOnly) break;

      } // foreach(Faktur sendCandidateFaktur_rec in sendCandidatesFakturList) 

      theNaplataZahtjev.Naplata = naplataList.ToArray();

      #endregion The MAP API Loop - foreach MAP_ActionsList

      #region Write MAP_XML to file

      string dateID = isByDates ? dateOD.Value.ToString(ZXC.VvDateDdMmYyyyFormat) + "_" + dateDO.Value.ToString(ZXC.VvDateDdMmYyyyFormat) : DateTime.Now.ToString(ZXC.VvDateDdMmYyyyFormat);

      string fileName = "eNaplate_" + ZXC.CURR_prjkt_rec.Ticker + "_" + dateID + ".xml";
      
      string deaultVvPDFdirectoryName = VvForm.GetLocalDirectoryForVvFile(/*"_ eRacun IZLAZNI"*/ZXC.eRacuniDIR);

      string todayDir = "NIR" + "_XML_" + DateTime.Today.ToString(ZXC.VvDateYyyyMmDdMySQLFormat);
      
      string dirName = Path.Combine(deaultVvPDFdirectoryName, todayDir);
      
      string fullName = Path.Combine(dirName, fileName);

      if(!Directory.Exists(dirName))
      {
         Directory.CreateDirectory(dirName);
      }

      theXmlString = theNaplataZahtjev.Serialize(ZXC.VvUTF8Encoding_noBOM);

      theXmlString = EN16931.UBL.QWE2.EvidentirajNaplatuZahtjev.NormalizeDatumVrijemeSlanjaToHHmmss(theXmlString); // jebemvamsvimamater 

      bool saveOK = EN16931.UBL.InvoiceType.VvSaveToFile(theXmlString, fullName, ZXC.VvUTF8Encoding_noBOM);
    //string theXmlString = theNaplataZahtjev.SaveToFile(fullName, ZXC.VvUTF8Encoding_noBOM);

      // upali ako os debagirati
      //System.Diagnostics.Process.Start(fullName);

      #endregion Write MAP_XML to file

      #region Finish

      //ZXC.FakturRec = null;

      ZXC.SetStatusText("");

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Kreirana je datoteka u kojoj je {0} prijava plaćanja.\n\r\n\rOtvaram directory lokacije datoteke\n\rte ju, po želji, možete kopirati i na neko drugo mjesto.", mapCount);

      if(saveOK)
      {
         // Open File Explorer and select the file
         System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fullName}\"");
      }
      else
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error,
            "Neuspješno spremanje XML datoteke.\n\nPutanja: {0}", fullName);
      }

      return newsCount;

      #endregion Finish
   }

   #endregion NIR

   #endregion FIR / FUR / NIR Load List and SubmodulActions

}

#region Bussiness Classes for JSON Request/Response
public class MER_CredentialsData
{
   [JsonPropertyName("Username")]
   public string Username { get; set; }

   [JsonPropertyName("Password")]
   public string Password { get; set; }

   [JsonPropertyName("CompanyId")]
   public string CompanyId { get; set; }

   [JsonPropertyName("CompanyBu")]
   public string CompanyBu { get; set; }

   // MER ima veliko slovo S u SoftwareId 
   // PND ima malo   slovo s u softwareId 
   // jos ne znamo koje su reperkusije toga 
   [JsonPropertyName("SoftwareId")]
   public string SoftwareId { get; set; }
   //[JsonPropertyName("softwareId")]
   //public string softwareId { get; set; }
}
public class VvMER_RequestData : MER_CredentialsData
{
   #region Constructors and Init
   private void InitMER_Credentials()
   {
      Vv_eRacun_HTTP.InitProjectData();

      this.Username   = Vv_eRacun_HTTP.VvMER_UserName  ;
      this.Password   = Vv_eRacun_HTTP.VvMER_Password  ;
      this.CompanyId  = Vv_eRacun_HTTP.VvMER_CompanyId ;
      this.CompanyBu  = ""                             ;
      this.SoftwareId = Vv_eRacun_HTTP.VvMER_SoftwareId;
   }

   private void InitPND_Credentials()
   {
      //this.softwareId = Vv_eRacun_HTTP.VvPND_SoftwareId;
        this.SoftwareId = Vv_eRacun_HTTP.VvPND_SoftwareId;
   }

   // za testiranje, pa sa test parametrima 
   public VvMER_RequestData(/*int username,*/ string password, string companyId, string companyBu, string softwareId, string xmlString)
   {
    //this.Username   = username  ;
      this.Username   = Vv_eRacun_HTTP.VvMER_UserName;
      this.Password   = password  ;
      this.CompanyId  = companyId ;
      this.CompanyBu  = companyBu ;
      this.SoftwareId = softwareId;
      this.File       =  xmlString;
   }

   public VvMER_RequestData(string xmlString) // za slanje jednog eRacuna 
   {
      switch(ZXC.F2_TheProvider)
      {
         case ZXC.F2_Provider_enum.MER: InitMER_Credentials(); this.File     = xmlString; break;
         case ZXC.F2_Provider_enum.PND: InitPND_Credentials(); this.document = xmlString; break;
      }
   }

   public VvMER_RequestData(uint electronicId) // za jedan racun 
   {
      switch(ZXC.F2_TheProvider)
      {
         case ZXC.F2_Provider_enum.MER: InitMER_Credentials(); break;
         case ZXC.F2_Provider_enum.PND: InitPND_Credentials(); break;
      }

      this.ElectronicId = electronicId;
   }

   public VvMER_RequestData(DateTime dateOD, DateTime dateDO) // za report 
   {
      switch(ZXC.F2_TheProvider)
      {
         case ZXC.F2_Provider_enum.MER: InitMER_Credentials(); break;
         case ZXC.F2_Provider_enum.PND: InitPND_Credentials(); break;
      }

      if(dateOD != DateTime.MinValue) this.From = dateOD;
      if(dateDO != DateTime.MaxValue) this.To   = dateDO;
   }

   public VvMER_RequestData()  
   {
      switch(ZXC.F2_TheProvider)
      {
         case ZXC.F2_Provider_enum.MER: InitMER_Credentials(); break;
         case ZXC.F2_Provider_enum.PND: InitPND_Credentials(); break;
      }
     
   }

   #endregion Constructors and Init

   // Document core properties
   [JsonPropertyName("ElectronicId")]
   public long? ElectronicId { get; set; }

   [JsonPropertyName("StatusId")]
   public int? StatusId { get; set; }

   [JsonPropertyName("File")]
   public string File { get; set; }

   // eIzvj request specific properties - start 

   [JsonPropertyName("xmlInvoice")]
   public string xmlInvoice { get; set; }

   [JsonPropertyName("DeliveryDate")]
   public DateTime? DeliveryDate { get; set; }

   [JsonPropertyName("IsCopy")]
   public bool IsCopy { get; set; }

   [JsonPropertyName("InvoiceType")]
   public string InvoiceType { get; set; }

   // eIzvj request specific properties -  end   

   [JsonPropertyName("Filter")]
   public string Filter { get; set; }

   [JsonPropertyName("ActionType")]
   public string ActionType { get; set; }

   // ovog dole MessageType smo mi rucno dodali za FISK status 
   // a ovog gore ActionType je iz MER API-ja izmislio Copilot 
   [JsonPropertyName("MessageType")]
   public string MessageType { get; set; }

   [JsonPropertyName("RejectReason")]
   public string RejectReason { get; set; }

   // Payment related
   [JsonPropertyName("PaymentDate")]
   public DateTime? PaymentDate { get; set; }

   [JsonPropertyName("PaymentAmount")]
   public decimal? PaymentAmount { get; set; }

   [JsonPropertyName("PaymentMethod")]
   public string PaymentMethod { get; set; }

   [JsonPropertyName("IssueDate")]
   public DateTime? IssueDate { get; set; }

   [JsonPropertyName("InternalMark")]
   public string InternalMark { get; set; }

   [JsonPropertyName("SenderIdentifierValue")]
   public string SenderIdentifierValue { get; set; }

   [JsonPropertyName("RecipientIdentifierValue")]
   public string RecipientIdentifierValue { get; set; }


   // Query date range
   [JsonPropertyName("From")]
   public DateTime? From { get; set; }

   [JsonPropertyName("To")]
   public DateTime? To { get; set; }

   // Registration and identification
   [JsonPropertyName("CompanyNumber")]
   public string CompanyNumber { get; set; }

   [JsonPropertyName("IdentifierType")]
   public /*int?*/string IdentifierType { get; set; }

   [JsonPropertyName("IdentifierValue")]
   public string IdentifierValue { get; set; }

   // Classification
   [JsonPropertyName("KPDCode")]
   public string KPDCode { get; set; }

   #region PND specifics

   [JsonPropertyName("document")]
   public string document { get; set; }

   [JsonPropertyName("identifier")]
   public string identifier { get; set; }

   [JsonPropertyName("schema")]
   public string schema { get; set; }

   [JsonPropertyName("type")]
   public string type { get; set; }

   #endregion PND specifics

}
public class VvMER_ResponseData : Vv_XSD_Bussiness_BASE<VvMER_ResponseData>
{
   #region Propertiz 

   // Document identification
   [JsonPropertyName("ElectronicId")]
   public long? ElectronicId { get; set; }

   [JsonPropertyName("DocumentNr")]
   public string DocumentNr { get; set; }

   [JsonPropertyName("DocumentTypeId")]
   public int? DocumentTypeId { get; set; }

   [JsonPropertyName("DocumentTypeName")]
   public string DocumentTypeName { get; set; }

   // Status information
   [JsonPropertyName("Status")]
   public string Status { get; set; }

   [JsonPropertyName("StatusId")]
   public int? StatusId { get; set; }

   [JsonPropertyName("StatusName")]
   public string StatusName { get; set; }

   [JsonPropertyName("DokumentProcessStatus")]
   public int? DokumentProcessStatus { get; set; }

   // Success indicators
   [JsonPropertyName("Success")]
   public bool? Success { get; set; }

   [JsonPropertyName("IsSuccess")]
   public bool? IsSuccess { get; set; }

   [JsonPropertyName("IsRegistered")]
   public bool? IsRegistered { get; set; }

   // Business entities information
   [JsonPropertyName("SenderBusinessNumber")]
   public string SenderBusinessNumber { get; set; }

   [JsonPropertyName("SenderBusinessUnit")]
   public string SenderBusinessUnit { get; set; }

   [JsonPropertyName("SenderBusinessName")]
   public string SenderBusinessName { get; set; }

   [JsonPropertyName("RecipientBusinessNumber")]
   public string RecipientBusinessNumber { get; set; }

   [JsonPropertyName("RecipientBusinessUnit")]
   public string RecipientBusinessUnit { get; set; }

   [JsonPropertyName("RecipientBusinessName")]
   public string RecipientBusinessName { get; set; }

   [JsonPropertyName("CompanyName")]
   public string CompanyName { get; set; }

   // Timestamps
   [JsonPropertyName("Created")]
   public DateTime? Created { get; set; }

   [JsonPropertyName("Modified")]
   public DateTime? Modified { get; set; }

   [JsonPropertyName("Updated")]
   public DateTime? Updated { get; set; }

   [JsonPropertyName("Sent")]
   public DateTime? Sent { get; set; }

   [JsonPropertyName("Delivered")]
   public DateTime? Delivered { get; set; }

   [JsonPropertyName("Issued")]
   public DateTime? Issued { get; set; }

   [JsonPropertyName("UpdateDate")]
   public DateTime? UpdateDate { get; set; }

   [JsonPropertyName("FiscalizationDate")]
   public DateTime? FiscalizationDate { get; set; }

   [JsonPropertyName("FiscalizationTimestamp")]
   public DateTime? FiscalizationTimestamp { get; set; }

   [JsonPropertyName("RejectTimestamp")]
   public DateTime? RejectTimestamp { get; set; }

   [JsonPropertyName("RegistrationDate")]
   public DateTime? RegistrationDate { get; set; }

   // Document content
   [JsonPropertyName("DocumentXml")]
   public string DocumentXml { get; set; }

   [JsonPropertyName("EncodedXml")]
   public string EncodedXml { get; set; }

   [JsonPropertyName("Description")]
   public string Description { get; set; }

   //// Collection properties
   //[JsonPropertyName("Documents")]
   //public List<DocumentInfo_Data> Documents { get; set; }

   //[JsonPropertyName("messages")]
   //public List<VvMER_Response_Data_AllActions> messages { get; set; }

   // State indicators
   [JsonPropertyName("Imported")]
   public bool? Imported { get; set; }

   // Messages
   [JsonPropertyName("Message")]
   public string Message { get; set; }

   #region PND specifics

   [JsonPropertyName("id")]
   public long Id { get; set; } // If the API returns a GUID/string, change to string.

   [JsonPropertyName("insertedOn")]
   public DateTime? InsertedOn { get; set; }

   [JsonPropertyName("identifier")]
   public string identifier { get; set; }

   [JsonPropertyName("published​On​Ams")]
   public bool? published​On​Ams { get; set; }

   [JsonPropertyName("schema")]
   public string schema { get; set; }

   [JsonPropertyName("mps​Endpoint")]
   public string mps​Endpoint { get; set; }

   #endregion PND specifics

   [Newtonsoft.Json.JsonIgnore] // da se ne serializira/deserializira automatski
   public string ResponseJson { get; set; }

   #endregion Propertiz 

   // FILIPIDIPI 
   public static Xtrano F2_eRacun_Arhiva_Set_AIR_XtranoFrom_XmlDocument_And_Faktur(string xmlString, Faktur faktur_rec)
   {
      if(faktur_rec == null) throw new Exception("F2_SetXtranoFrom_XmlDocument: faktur record is null!");

      byte[] zipped_xmlString = VvStringCompressor.CompressXml(xmlString);

      Xtrano xmlXtrano_rec = null;

      xmlXtrano_rec = new Xtrano()
      {
         T_XmlZip        = zipped_xmlString          ,
                                                     
         T_TT            = Mixer.TT_AIR              ,
                                                     
         T_konto         = faktur_rec.TT             ,
         T_parentID      = faktur_rec.RecID          , 
         T_dokDate       = faktur_rec.DokDate        ,
         T_ttNum         = faktur_rec.TtNum          ,
         F2_ElectronicID = faktur_rec.F2_ElectronicID,
         T_serial        = 1                         ,
         T_moneyA        = faktur_rec.S_ukKCRP       ,
         T_opis_128      = faktur_rec.KupdobName     , // fuse 
         T_devName       = ""                        , // fuse 
      };

      return xmlXtrano_rec;
   }
   public static Xtrano F2_eRacun_Arhiva_Set_AUR_XtranoFrom_Response_InvoiceType(string xmlString, VvMER_ResponseData responseData, EN16931.UBL.InvoiceType deserialized_InvoiceType)
   {
      if(responseData == null) throw new Exception("F2_SetXtranoFrom_XmlDocument: response is null!");
   
      byte[] zipped_xmlString = VvStringCompressor.CompressXml(xmlString);
   
      bool deserialized_eRacun_Is_Unreadable = deserialized_InvoiceType?.LegalMonetaryTotal?.TaxInclusiveAmount?.Value == null;
   
      decimal  invoiceTypeMoney = 0M;
      DateTime invoiceTypeDokDate = DateTime.MinValue;
      
      if(deserialized_eRacun_Is_Unreadable)
      {
#if !DEBUG
         // Try to extract TaxInclusiveAmount directly from XML as fallback
         invoiceTypeMoney = ExtractTaxInclusiveAmountFromXml(xmlString);
#endif
         if(invoiceTypeMoney.IsZero())
         {
            string debugPath = @"C:\temp\debug_invoice_" + responseData.ElectronicId.ToString() + "_" + responseData.SenderBusinessName + ".xml";
            System.IO.Directory.CreateDirectory(@"C:\temp");
            System.IO.File.WriteAllText(debugPath, xmlString, System.Text.Encoding.UTF8);
            System.Diagnostics.Debug.WriteLine($"XML saved to: {debugPath}");
   
            ZXC.aim_emsg(MessageBoxIcon.Exclamation, $"Deserialized eRacun Is Unreadable and TaxInclusiveAmount extraction failed. Money will be zero!{Environment.NewLine}{Environment.NewLine}[{debugPath}]  file created.");
         }
         else
         {
            // Successfully extracted from XML
            ZXC.aim_emsg(MessageBoxIcon.Information, $"Deserialized eRacun failed but TaxInclusiveAmount extracted from XML: {invoiceTypeMoney:F2}");
         }
      }
      else
      {
         invoiceTypeMoney   = deserialized_InvoiceType.LegalMonetaryTotal.TaxInclusiveAmount.Value;
         invoiceTypeDokDate = deserialized_InvoiceType.IssueDate?.Value ?? DateTime.MinValue;
      }
   
      Xtrano xmlXtrano_rec = new Xtrano()
      {               
         T_XmlZip        = zipped_xmlString                 , // from XML 
         F2_ElectronicID = (uint)responseData.ElectronicId  , // from response 
         T_dokDate       = (DateTime)responseData.Sent      , // from response 
         T_dokDate2      = invoiceTypeDokDate               , // from XML 
         T_opis_128      = responseData.SenderBusinessName	, // from response 
         T_theString     = responseData.DocumentNr          , // from response 
         T_konto	       = responseData.SenderBusinessNumber, // from response 
         T_devName       = responseData.StatusId.ToString() , // from response
         T_TT            = Mixer.TT_AUR                     , 
         T_moneyA        = invoiceTypeMoney                 , // from XML 
      };
   
      return xmlXtrano_rec;
   }
   public static Xtrano F2_eRacun_Arhiva_Set_AUR_XtranoFrom_Response_CreditNote(string xmlString, VvMER_ResponseData responseData, EN16931.UBL.CreditNoteType deserialized_CreditNoteType)
   {
      if(responseData == null) throw new Exception("F2_eRacun_Arhiva_Set_AUR_XtranoFrom_Response_CreditNote: response is null!");
   
      byte[] zipped_xmlString = VvStringCompressor.CompressXml(xmlString);
   
      bool deserialized_CreditNote_Is_Unreadable = deserialized_CreditNoteType?.LegalMonetaryTotal?.TaxInclusiveAmount?.Value == null;
   
      decimal  creditNoteTypeMoney = 0M;
      DateTime creditNoteTypeDokDate = DateTime.MinValue;
      
      if(deserialized_CreditNote_Is_Unreadable)
      {
   #if !DEBUG
         // Try to extract TaxInclusiveAmount directly from XML as fallback
         creditNoteTypeMoney = ExtractTaxInclusiveAmountFromXml(xmlString);
   #endif
         if(creditNoteTypeMoney.IsZero())
         {
            string debugPath = @"C:\temp\debug_creditnote_" + responseData.ElectronicId.ToString() + "_" + responseData.SenderBusinessName + ".xml";
            System.IO.Directory.CreateDirectory(@"C:\temp");
            System.IO.File.WriteAllText(debugPath, xmlString, System.Text.Encoding.UTF8);
            System.Diagnostics.Debug.WriteLine($"XML saved to: {debugPath}");
   
            ZXC.aim_emsg(MessageBoxIcon.Exclamation, $"Deserialized CreditNote Is Unreadable and TaxInclusiveAmount extraction failed. Money will be zero!{Environment.NewLine}{Environment.NewLine}[{debugPath}]  file created.");
         }
         else
         {
            // Successfully extracted from XML
            ZXC.aim_emsg(MessageBoxIcon.Information, $"Deserialized CreditNote failed but TaxInclusiveAmount extracted from XML: {creditNoteTypeMoney:F2}");
         }
      }
      else
      {
         creditNoteTypeMoney   = deserialized_CreditNoteType.LegalMonetaryTotal.TaxInclusiveAmount.Value;
         creditNoteTypeDokDate = deserialized_CreditNoteType.IssueDate?.Value ?? DateTime.MinValue;
      }
   
      Xtrano xmlXtrano_rec = new Xtrano()
      {               
         T_XmlZip        = zipped_xmlString                 , // from XML 
         F2_ElectronicID = (uint)responseData.ElectronicId  , // from response 
         T_dokDate       = (DateTime)responseData.Sent      , // from response 
         T_dokDate2      = creditNoteTypeDokDate            , // from XML 
         T_opis_128      = responseData.SenderBusinessName  , // from response 
         T_theString     = responseData.DocumentNr          , // from response 
         T_konto         = responseData.SenderBusinessNumber, // from response 
         T_devName       = responseData.StatusId.ToString() , // from response
         T_TT            = Mixer.TT_AUR                     , // from XML 
         T_moneyA        = creditNoteTypeMoney,
      };
   
      return xmlXtrano_rec;
   }
   public static Xtrano F2_eRacun_Arhiva_Set_AUR_XtranoFrom_Response_UNKNOWN_Type(string xmlString, VvMER_ResponseData responseData/*, EN16931.UBL.InvoiceType deserialized_InvoiceType*/)
   {
      if(responseData == null) throw new Exception("F2_SetXtranoFrom_XmlDocument: response is null!");
   
      byte[] zipped_xmlString = VvStringCompressor.CompressXml(xmlString);
   
      decimal  invoiceTypeMoney = 0M;
      DateTime invoiceTypeDokDate = DateTime.MinValue;
      
      Xtrano xmlXtrano_rec = new Xtrano()
      {               
         T_XmlZip        = zipped_xmlString                 , // from XML 
         F2_ElectronicID = (uint)responseData.ElectronicId  , // from response 
         T_dokDate       = (DateTime)responseData.Sent      , // from response 
         T_dokDate2      = invoiceTypeDokDate               , // from XML 
         T_opis_128      = responseData.SenderBusinessName	, // from response 
         T_theString     = responseData.DocumentNr          , // from response 
         T_konto	       = responseData.SenderBusinessNumber, // from response 
         T_devName       = responseData.StatusId.ToString() , // from response
         T_TT            = Mixer.TT_AUR                     , 
         T_moneyA        = invoiceTypeMoney                 , // from XML 
      };
   
      return xmlXtrano_rec;
   }

   /// <summary>
   /// MAP Xtrano - evidencija prijave MAP na poreznu upravu
   /// </summary>
   /// <param name="ftrans_rec"></param>
   /// <param name="faktur_rec"></param>
   /// <returns></returns>
   public static Xtrano F2_MAPtrans_SetXtranoFrom_Ftrans(Ftrans MAPftrans_rec, Faktur MAPfaktur_rec, VvMER_ResponseData responseData = null)
   {
      Xtrano MAPxtrano_rec = null;

      // 13.02.206: gasimo jer ne vidimo smisao ('xml' string je nečitljiv ne kuzimo cemu sluzi)
    //byte[] zipped_xmlString = VvStringCompressor.CompressXml(responseData?.EncodedXml);

      MAPxtrano_rec = new Xtrano()
      {
       //T_XmlZip   = zipped_xmlString                             , // !!! Observacija od 05.12.2025: treba li nam ovo?! tako i onako ga imas u AIR Xtrano-u a potencijalno napuhava database !!! 
         
         T_dokDate  = (responseData != null ? (DateTime)responseData?.FiscalizationTimestamp : DateTime.Now),
                                                
         T_TT       = Mixer.TT_MAP                                 ,
                                                                   
         T_konto    = MAPfaktur_rec.TT                             ,
         T_parentID = MAPftrans_rec.T_recID                        , // Ftrans LINK: t_parentID je ftrans recID --- > JOIN!     
         T_ttNum    = MAPfaktur_rec.RecID                          , // Faktur LINK: t_ttNum    je faktur recID ---> nije join! 
         T_dokNum   = MAPfaktur_rec.F2_ElectronicID                ,                                            
         T_serial   = 1                                            ,                                            
         T_moneyA   = MAPftrans_rec.T_pot                          , // Ftrans: t_moneyA je iznos UPLATE        
         T_opis_128 = MAPftrans_rec.T_tipBr                        , //                                         
         T_devName  = ""                                           , // fuse                                    

         T_theBool  = responseData == null                         // znaci, ovo je true ako je MAPano sa NIR-a 
      };

      return MAPxtrano_rec;
   }

   #region Util Metodz
   /// <summary>
   /// Extracts LegalMonetaryTotal.TaxInclusiveAmount.Value directly from XML when UBL deserialization fails
   /// </summary>
   /// <param name="xmlString">The XML content containing the invoice</param>
   /// <returns>The TaxInclusiveAmount value or 0.00M if not found</returns>
   internal static decimal ExtractTaxInclusiveAmountFromXml(string xmlString)
   {
      try
      {
         if(string.IsNullOrEmpty(xmlString))
            return 0.00M;

         XDocument xmlDoc = XDocument.Parse(xmlString);

         // Define namespaces used in UBL documents
         XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
         XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";

         // Navigate to LegalMonetaryTotal/TaxInclusiveAmount/Value
         XElement legalMonetaryTotal = xmlDoc.Descendants(cac + "LegalMonetaryTotal").FirstOrDefault();
         if(legalMonetaryTotal != null)
         {
            XElement taxInclusiveAmount = legalMonetaryTotal.Element(cbc + "TaxInclusiveAmount");
            if(taxInclusiveAmount != null)
            {
               string amountText = taxInclusiveAmount.Value?.Trim();
               if(!string.IsNullOrEmpty(amountText))
               {
                  decimal amount;
                  if(decimal.TryParse(amountText, System.Globalization.NumberStyles.Any,
                      System.Globalization.CultureInfo.InvariantCulture, out amount))
                  {
                     return amount;
                  }
               }
            }
         }
      }
      catch(Exception ex)
      {
         // Log the exception but don't throw - return 0 as fallback
         System.Diagnostics.Debug.WriteLine($"Error extracting TaxInclusiveAmount from XML: {ex.Message}");
      }

      return 0.00M;
   }
   /// <summary>
   /// Extracts IssueDate directly from XML when UBL deserialization fails
   /// </summary>
   /// <param name="xmlString">The XML content containing the invoice</param>
   /// <returns>The IssueDate value or DateTime.MinValue if not found</returns>
   internal static DateTime ExtractIssueDateFromXml(string xmlString)
   {
      try
      {
         if(string.IsNullOrEmpty(xmlString))
            return DateTime.MinValue;

         XDocument xmlDoc = XDocument.Parse(xmlString);

         // Define namespaces used in UBL documents
         XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";

         // Navigate to IssueDate element
         XElement issueDate = xmlDoc.Descendants(cbc + "IssueDate").FirstOrDefault();
         if(issueDate != null)
         {
            string dateText = issueDate.Value?.Trim();
            if(!string.IsNullOrEmpty(dateText))
            {
               DateTime date;
               if(DateTime.TryParse(dateText, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
               {
                  return date;
               }
            }
         }
      }
      catch(Exception ex)
      {
         // Log the exception but don't throw - return DateTime.MinValue as fallback
         System.Diagnostics.Debug.WriteLine($"Error extracting IssueDate from XML: {ex.Message}");
      }

      return DateTime.MinValue;
   }

   #endregion Util Metodz
}

#region Bussiness Classes for JSON Response - FiscalizationStatus
public class VvMER_FiscalizationMessage
{
   [JsonPropertyName("fiscalizationRequestId")]
   public string FiscalizationRequestId { get; set; }

   [JsonPropertyName("dateOfFiscalization")]
   public DateTime? DateOfFiscalization { get; set; }

   //[JsonPropertyName("isSuccess")]
   //public bool? IsSuccess { get; set; }
   [JsonPropertyName("status")]
   public int Status { get; set; }

   // 5.6. Status
   // Value – Description
   // 0 – Uspjeh
   // 1 – Neuspjeh
   // 2 – Na čekanju
   public ZXC.F2_StatusInAndOutBoxEnum StatusOutboxKind 
   {
      get
      {
         switch(this.Status)
         {
            case 0:
               return ZXC.F2_StatusInAndOutBoxEnum.DA_JE;     // TOČKA - Zelena 
            case 1:
               return ZXC.F2_StatusInAndOutBoxEnum.NE_NIJE;   // TOČKA - Crvena 
            case 2:
               return ZXC.F2_StatusInAndOutBoxEnum.Na_cekanju;// TOČKA - Žuta   
            default:
               return ZXC.F2_StatusInAndOutBoxEnum.Nepoznato; // TOČKA - PRAZNA 
         }

         //return (ZXC.F2_StatusOutboxEnum)Status;
      }
   }

   [JsonPropertyName("message")]
   public string Message { get; set; }

   [JsonPropertyName("encodedXml")]
   public string EncodedXml { get; set; }

   [JsonPropertyName("errorCode")]
   public string ErrorCode { get; set; }

   [JsonPropertyName("errorCodeDescription")]
   public string ErrorCodeDescription { get; set; }

   [JsonPropertyName("messageType")]
   public int? MessageType { get; set; }

   [JsonPropertyName("messageTypeDescription")]
   public string MessageTypeDescription { get; set; }

   // za JOIN sa Faktur iz TheFakturList 
   public long? TheElectronicId { get; set; }

}
public class VvMER_Response_Data_FiscalizationStatus
{
   [JsonPropertyName("electronicId")]
   public long? ElectronicId { get; set; }

   [JsonPropertyName("RecipientIdentifierValue")]
   public string RecipientIdentifierValue { get; set; }

   [JsonPropertyName("RecipientName")]
   public string RecipientName { get; set; }

   [JsonPropertyName("channelType")]
   public int? ChannelType { get; set; }

   [JsonPropertyName("channelTypeDescription")]
   public string ChannelTypeDescription { get; set; }

   [JsonPropertyName("messages")]
   public List<VvMER_FiscalizationMessage> Messages { get; set; }

   public VvMER_Response_Data_FiscalizationStatus()
   {
      Messages = new List<VvMER_FiscalizationMessage>();
   }
}

#endregion Bussiness Classes for JSON Response - FiscalizationStatus
public class WebApiResult<T>
{
   public ZXC.F2_WebApi WebApiKind   { get; set; }
   public string        WebApiAddr   { get; set; }
   public T             ResponseData { get; set; }
   public string ResponseString      { get; set; }
   public int?   StatusCode          { get; set; }
   public string StatusDescription   { get; set; }
   public string ErrorBody           { get; set; }
   public string ExceptionMessage    { get; set; }
 //public string ResponseXml         { get; set; }

   public List<string> MessageList 
   { 
      get
      {
         List<string> messageList = new List<string>();
         string messageLine;

         if(ResponseString.NotEmpty())
         {
            messageList.Add("Response String:");
            foreach(string line in ResponseString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
               messageLine = $"    {line}";
               messageList.Add(messageLine);
            }
         }
         if(StatusCode.HasValue)
         {
            messageLine = $"StatusCode: {StatusCode}";
            messageList.Add(messageLine);
         }
         if(!string.IsNullOrEmpty(StatusDescription))
         {
            messageLine = $"StatusDescription: {StatusDescription}";
            messageList.Add(messageLine);
         }
         if(ErrorBody.NotEmpty())
         {
            messageList.Add("ErrorBody:");
            foreach(string line in ErrorBody.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
               messageLine = $"    {line}";
               messageList.Add(messageLine);
            }
         }
         if(ExceptionMessage.NotEmpty())
         {
            messageList.Add("ExceptionMessage:");
            foreach(string line in ExceptionMessage.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
               messageLine = $"    {line}";
               messageList.Add(messageLine);
            }
         }

         return messageList;
      }
   }

   public static WebApiResult<T> GetWebApiResult_From_WebApiResultWithList<T>(WebApiResult<List<T>> webApiResultWithList) where T : class
   {
      WebApiResult<T> webApiResult = new WebApiResult<T>();
      
      if(webApiResultWithList != null)
      {
         webApiResult.WebApiKind        = webApiResultWithList.WebApiKind;
         webApiResult.WebApiAddr        = webApiResultWithList.WebApiAddr;
         webApiResult.ResponseData      = webApiResultWithList.ResponseData?.FirstOrDefault();
         webApiResult.ResponseString    = webApiResultWithList.ResponseString;
         webApiResult.StatusCode        = webApiResultWithList.StatusCode;
         webApiResult.StatusDescription = webApiResultWithList.StatusDescription;
         webApiResult.ErrorBody         = webApiResultWithList.ErrorBody;
         webApiResult.ExceptionMessage  = webApiResultWithList.ExceptionMessage;
      }
   
      return webApiResult;
   }
}

#endregion Bussiness Classes for JSON Request/Response

// VvForm Submodul Actions about F2 fiscalization 
public /*sealed*/ partial class VvForm : Crownwood.DotNetMagic.Forms.DotNetMagicForm
{
   private void F2_UserManual_OnClick(object sender, EventArgs e)
   {
   }
   private void F2_RISK_Rules(object sender, EventArgs e)
   {
      bool isFIR = (sender as ToolStripButton).Name.StartsWith("FIR");

      F2_Rules_Dlg dlg = new F2_Rules_Dlg(isFIR);

      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      dlg.TheUC.GetDscFields();

      dlg.Dispose();
   }
   private void F2_RefreshFIR_FakturListAndStatuses(object sender, EventArgs e) 
   {
      ((F2_Izlaz_UC)TheVvUC).INIT_FIR(); // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX 
   }
   private void F2_Send_eRacune(object sender, EventArgs e) 
   { 
      Vv_eRacun_HTTP.InitProjectData();

      if(Vv_eRacun_HTTP.Is_FIR_ON() == false) return;

      int newsCount = /*BBB*/Vv_eRacun_HTTP.WS_Discover_Candidates_And_Eventually_SEND_eRacune((F2_Izlaz_UC)TheVvUC, true);

      if(newsCount.IsZeroOrPositive()) F2_RefreshFIR_FakturListAndStatuses(sender, e); // -1 means 'cancel' button clicked 
   }
   private void F2_MAPaj_From_FIR(object sender, EventArgs e) 
   { 
      Vv_eRacun_HTTP.InitProjectData();

      if(Vv_eRacun_HTTP.Is_FIR_ON() == false) return;

      int newsCount = /*DDD*/Vv_eRacun_HTTP.WS_Discover_Candidates_And_Eventually_MAPaj_uplate(/*(F2_Izlaz_UC)*/TheVvUC, true, false);

      if(newsCount.IsZeroOrPositive()) F2_RefreshFIR_FakturListAndStatuses(sender, e); // -1 means 'cancel' button clicked 
   }
   private void F2_MAPaj_From_NalogDUC(object sender, EventArgs e) 
   {
      Vv_eRacun_HTTP.InitProjectData();

      if(Vv_eRacun_HTTP.Is_FIR_ON() == false) return;

      int newsCount = /*DDD*/Vv_eRacun_HTTP.WS_Discover_Candidates_And_Eventually_MAPaj_uplate(/*(F2_Izlaz_UC)*/TheVvUC, true, true );
   }
   private bool ShowPDF_FromXtranoArhivaOLD(Xtrano xtrano_rec)
   {
      List<(string Filename, byte[] PdfBytes)> pdfFiles = xtrano_rec.F2_GetPdfFilesWithNames();

      if(pdfFiles.Count.IsZero()) { ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Stop   , "Nema spremljenih PDF-ova za ovaj eRačun."); return false; }

      for(int pdfIdx = 0; pdfIdx < pdfFiles.Count; ++pdfIdx)
      {
         if(pdfFiles.Count > 1) { ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, $"Ima više od jednog PDF-a. Prikazujem {pdfIdx+1}. od {pdfFiles.Count}."); /*return;*/ }

         (string filename, byte[] pdfBytes) thePDF = pdfFiles[pdfIdx];

         string dirame   = VvPref.eRacun_Izlaz_Prefs.DirectoryName;
         string fullName = Path.Combine(dirame, thePDF.filename);

         File.WriteAllBytes(fullName, thePDF.pdfBytes);

         System.Diagnostics.Process.Start(fullName);
      }

      return true;
   }
   private bool ShowPDF_FromXtranoArhiva(Xtrano xtrano_rec)
   {
      List<(string Filename, byte[] PdfBytes)> pdfFiles = xtrano_rec.F2_GetPdfFilesWithNames();

      if(pdfFiles.Count.IsZero()) { ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Stop, "Nema spremljenih PDF-ova za ovaj eRačun."); return false; }

      for(int pdfIdx = 0; pdfIdx < pdfFiles.Count; ++pdfIdx)
      {
         if(pdfFiles.Count > 1) { ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, $"Ima više od jednog PDF-a. Prikazujem {pdfIdx + 1}. od {pdfFiles.Count}."); /*return;*/ }

         (string filename, byte[] pdfBytes) thePDF = pdfFiles[pdfIdx];

         if(pdfFiles.Count > 1)
         {
            string addition = "_" + (pdfIdx + 1).ToString();
            thePDF.filename = thePDF.filename.Replace(".pdf", addition + ".pdf");
         }

         if(thePDF.pdfBytes == null || thePDF.pdfBytes.Length == 0)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Stop, "PDF sadržaj je prazan. Ne mogu spremiti PDF.");
            return false;
         }

         string deaultVvPDFdirectoryName = VvForm.GetLocalDirectoryForVvFile(/*"_ eRacun IZLAZNI"*/ZXC.eRacuniDIR);
         string todayDir = /*theTT*/xtrano_rec.T_TT + "_PDF_" + DateTime.Today.ToString(ZXC.VvDateYyyyMmDdMySQLFormat);

         // 04.02.2026: 
         //string dirName  = VvPref.eRacun_Izlaz_Prefs.DirectoryName;
         string dirName  = Path.Combine(deaultVvPDFdirectoryName, todayDir);
         string fullName = Path.Combine(dirName, thePDF.filename);

         try
         {
            if(dirName.NotEmpty()) Directory.CreateDirectory(dirName);

            File.WriteAllBytes(fullName, thePDF.pdfBytes);

            FileInfo fi = new FileInfo(fullName);
            if(!fi.Exists || fi.Length != thePDF.pdfBytes.Length)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Stop, "Neuspješno spremanje PDF-a (provjera datoteke nije prošla).");
               return false;
            }

            System.Diagnostics.Process.Start(fullName);
         }
         catch(IOException ex)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Stop,
               "Ne mogu spremiti PDF. Datoteka je možda već otvorena u drugom programu ili je disk nedostupan.\n\r\n\rPutanja:\n\r{0}\n\r\n\rDetalj:\n\r{1}",
               fullName,
               ex.Message);
            return false;
         }
         catch(Exception ex)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Stop, $"Ne mogu spremiti ili otvoriti PDF:\n\r\n\r{fullName}\n\r\n\r{ex.Message}");
            return false;
         }
      }

      return true;
   }
   private void F2_Outgoing_eRacun_QuickSend  (object sender, EventArgs e) { F2_Outgoing_eRacun_QuickSend_JOB(false); }
   private void F2_Outgoing_eRacun_QuickSend_B(object sender, EventArgs e) { F2_Outgoing_eRacun_QuickSend_JOB(true ); }
   private void F2_Outgoing_eRacun_QuickSend_JOB(bool isIRMcalcB)
   {
      Faktur faktur_rec = (TheVvDocumentRecordUC as FakturDUC).faktur_rec;

      if(faktur_rec.IsF2 == false) { ZXC.aim_emsg(MessageBoxIcon.Stop, "Račun nije F2! Ne može se slati kao F2 eRačun."); return; }

      if(Vv_eRacun_HTTP.Is_FIR_SEND_ON() == false) return;

      DialogResult result;
      bool sendOK;
      Outgoing_eRacun_parameters oeRp;

      // zelimo poslati KOPIJU racuna? 
      if(faktur_rec.F2_IsSentTry && (faktur_rec.F2_StatusCD == 30 || faktur_rec.F2_StatusCD == 40)) // 30 - poslano, 40 - preuzeto 
      {
         result = MessageBox.Show("KOPIJA!!! Potvrđujete slanje KOPIJE ovog eRačuna?", "Potvrdite KOPIJU eRačuna", MessageBoxButtons.YesNo, MessageBoxIcon.Question); 
         if(result != DialogResult.Yes) return;

         Cursor.Current = Cursors.WaitCursor;

         oeRp = Set_Outgoing_eRacun_parameters(faktur_rec, TheVvUC, true, true, true);

         sendOK = RISK_Outgoing_eRacun_JOB(oeRp, true, isIRMcalcB);

         Cursor.Current = Cursors.Default;

         return;
      }

      // odlucujemo dozvoliti RE SEND samo ako je status 'Neuspjelo' (50) 
      if(faktur_rec.F2_IsSentTry && faktur_rec.F2_StatusCD != 50) 
      { 
         ZXC.aim_emsg(MessageBoxIcon.Stop, $"Ovaj eRačun je već poslan ili je u pokušaju slanja.{Environment.NewLine}{Environment.NewLine}Ponovni pokušaj slanja dozvoljen je samo pri transportnom statusu 'Neuspjelo'"); 
         return; 
      }

      result = MessageBox.Show("Potvrđujete slanje ovog eRačuna?", "Potvrdite eRačun", MessageBoxButtons.YesNo, MessageBoxIcon.Question); 
      if(result != DialogResult.Yes) return;

      Cursor.Current = Cursors.WaitCursor;

      oeRp = Set_Outgoing_eRacun_parameters(faktur_rec, TheVvUC, true, true);

      sendOK = RISK_Outgoing_eRacun_JOB(oeRp, true, isIRMcalcB);

      Cursor.Current = Cursors.Default;
   }
   internal Outgoing_eRacun_parameters Set_Outgoing_eRacun_parameters(Faktur faktur_rec, VvUserControl theVvUC, bool isQuickSend, bool _isOneOnlyFromFakturDUC, bool _wantsKOPIJA = false)
   {
      Outgoing_eRacun_parameters oeRp = new Outgoing_eRacun_parameters(_isOneOnlyFromFakturDUC);

      Kupdob kupdob_rec   = theVvUC.Get_Kupdob_FromVvUcSifrar(faktur_rec.KupdobCD  );
      Kupdob primPlat_rec = theVvUC.Get_Kupdob_FromVvUcSifrar(faktur_rec.PrimPlatCD);

      string theTT = faktur_rec.TT;

    //ZXC.FakturList_To_PDF_subDsc     = subDsc;
      PrnFakDsc thePFD = new PrnFakDsc(FakturDUC.GetDscLuiListForThisTT(theTT, /*subDsc*/ 0 /*?!*/)); // TODO: !!! ili ne? 

      #region Create PDF files to hard disk - OR - Print To Printer

      RptR_IRA                   theRptR_IRA   ;

      ExportOptions              CrExportOptions;
      DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
      PdfRtfWordFormatOptions    CrFormatTypeOptions          = new PdfRtfWordFormatOptions();

      // 04.02.2026: 
    //string deaultVvPDFdirectoryName = VvForm.GetLocalDirectoryForVvFile(VvPref.VvMailData.DeaultVvPDFdirectoryName);
      /* oeRp_1. */ oeRp.faktur_rec   = faktur_rec;
      string deaultVvPDFdirectoryName = VvForm.GetLocalDirectoryForVvFile(/*"_ eRacun IZLAZNI"*/ZXC.eRacuniDIR);

      string todayDir                 = theTT + "_PDF_" + DateTime.Today.ToString(ZXC.VvDateYyyyMmDdMySQLFormat);
      string pdfFileNameOnly     ;
      string PDFfileFullPathName ;

      string currentLocalDirectory = Path.Combine(deaultVvPDFdirectoryName, todayDir);

      VvForm.CreateDirectoryInMyDocuments(currentLocalDirectory);

      InvoiceType.CurrentLocalDirectory = currentLocalDirectory;

      // 1. GetReportDocument
      theRptR_IRA = VvRiskReport.GetRptR_IRA(faktur_rec, thePFD, theTT);

      // 2. get fileName 

      #region 2026 totalno sam popizdio s starim varijablama pa radim nove

      string PDF_And_XML_fileBaseName  = faktur_rec.TT_And_TtNum + " [" + faktur_rec.KupdobName + "]";

      string PDF_And_XML_directoryName = currentLocalDirectory;

      oeRp.qweFileNameBaseOnly = PDF_And_XML_fileBaseName ;
      oeRp.qweTheDirectoryName = PDF_And_XML_directoryName;

      #endregion 2026 totalno sam popizdio s starim varijablama pa radim nove

      pdfFileNameOnly = PDF_And_XML_fileBaseName + ".pdf";

      PDFfileFullPathName = Path.Combine(currentLocalDirectory, pdfFileNameOnly);

      // 3. set reportDocument.ExportOptions
      try
      {
         CrDiskFileDestinationOptions.DiskFileName = PDFfileFullPathName;
         CrExportOptions                           = theRptR_IRA.reportDocument.ExportOptions;
         CrExportOptions.ExportDestinationType     = ExportDestinationType.DiskFile;
         CrExportOptions.ExportFormatType          = ExportFormatType.PortableDocFormat;
         CrExportOptions.DestinationOptions        = CrDiskFileDestinationOptions;
         CrExportOptions.FormatOptions             = CrFormatTypeOptions;

         theRptR_IRA.reportDocument.Export();
      }
      catch(System.Exception ex)
      {
         MessageBox.Show(ex.ToString());
      }
      finally
      {
         if(theRptR_IRA != null)
         {
            theRptR_IRA.reportDocument.Close();
            theRptR_IRA.reportDocument.Dispose();
            theRptR_IRA.Dispose();
         }
      }

      #endregion Create PDF files to hard disk - OR - Print To Printer

    ///* oeRp_1. */ oeRp.faktur_rec              = faktur_rec                                      ; // preselio desetak redove ranije 
      /* oeRp_2. */ oeRp.kupdob_rec              = kupdob_rec                                      ;
      /* oeRp_3. */ oeRp.primPlat_rec            = primPlat_rec                                    ;
      /* oeRp_4. */ oeRp.thePFD                  = thePFD                                          ;
      /* oeRp_5. */ oeRp.PDF_as_base64_byteArray = System.IO.File.ReadAllBytes(PDFfileFullPathName);
      /* oeRp_6. */ oeRp.pdfFileNameOnly         = PDFfileFullPathName                             ;
      /* oeRp_7. */ oeRp.fullPath_XML_FileName   = oeRp.suggestedXmlFileName + ".xml"              ;
      /*         */ oeRp.wantsKOPIJA             = _wantsKOPIJA                                    ;
      // NOTA BENE! imas malo povise #region 2026 totalno sam popizdio s starim varijablama pa radim nove 

      // 22.02.2026: 
      if(oeRp.faktur_rec.ExternLink1.NotEmpty())
      {
         string msg = $"Ovaj rn. ({oeRp.faktur_rec.TT_And_TtNum}) ima pridruženu datoteku {oeRp.faktur_rec.ExternLink1}\n\r\n\ru ExternLink1-u.\n\r\n\rDa li ju želite poslati kao dodatni prateći dokument sa ovim eRačunom?";

         DialogResult result = MessageBox.Show(msg, "Potvrdite dodatni attachment", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

         if(result == DialogResult.Yes)
         {
            if(System.IO.File.Exists(oeRp.faktur_rec.ExternLink1))
            {
               oeRp.ADR_as_base64_byteArray = System.IO.File.ReadAllBytes(oeRp.faktur_rec.ExternLink1);
            }
            else
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, $"Ne mogu pronaći datoteku navedenu u ExternLink1 polju.\n\r\n\rPutanja:\n\r{oeRp.faktur_rec.ExternLink1}\n\r\n\rADR će biti prazan.");
            }

         } // if(result == DialogResult.Yes)
      }

      return oeRp;
   }
   private void F2_RefreshFUR_QueryInboxAndFakturListAndStatuses(object sender, EventArgs e) 
   {
      ((F2_Ulaz_UC)TheVvUC).INIT_FUR(); // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX 
   }
   private void F2_Import_FUR_Fakturs(object sender, EventArgs e)
   {
      Vv_eRacun_HTTP.InitProjectData();

      if(Vv_eRacun_HTTP.Is_FUR_ON() == false) return;

      int newsCount = /*ZZZ*/Vv_eRacun_HTTP.Import_FUR_Fakturs_JOB((F2_Ulaz_UC)TheVvUC);

      if(newsCount.IsZeroOrPositive()) ((F2_Ulaz_UC)TheVvUC).INIT_FUR(); // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX 

   }
   private void F2_ReRecieve_FUR_XtranoArhiva(object sender, EventArgs e)
   {
      F2_Ulaz_UC theUC = TheVvUC as F2_Ulaz_UC;

      int rowIdx = -1;
      Xtrano xtrano_rec = null;

      if(theUC.TheG.SelectedCells.Count == 0)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Molimo odaberite red iz tablice.");
         return;
      }

      rowIdx = theUC.TheG.SelectedCells[0].RowIndex;

      if(rowIdx < 0 || rowIdx >= theUC.TheXtranoList.Count)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Neispravan odabir reda.");
         return;
      }

      xtrano_rec = theUC.TheXtranoList[rowIdx];

      #region 1. Call RECEIVE to get full XML document
      
      string theXmlString = "";

      WebApiResult<VvMER_ResponseData> webApiResult = null;
      
      bool receiveOK = true;

      switch(ZXC.F2_TheProvider)
      {
         case ZXC.F2_Provider_enum.MER:
            {
               try
               {
                  webApiResult = Vv_eRacun_HTTP.VvMER_WebService_Receive_XML(/*(uint)responseData.ElectronicId*/xtrano_rec.F2_ElectronicID);

                  theXmlString = webApiResult.ResponseData.DocumentXml;

                  if(webApiResult.ResponseData == null || webApiResult.ResponseData.DocumentXml.IsEmpty())
                  {
                     Vv_eRacun_HTTP.Show_WebApiResult_ErrorMessageBox(webApiResult);
                     receiveOK = false;
                  }
               }
               catch(Exception ex)
               {
                  ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom slanja na WebServis: {0}", ex.Message);
                  receiveOK = false;
               }
               break;

            } // case ZXC.F2_Provider_enum.MER: 

         case ZXC.F2_Provider_enum.PND:
            {
               throw new NotImplementedException("Get_FISK_Status_ForElectronicID: F2 Provider PND not implemented yet.");
               receiveOK = false;
               break;
            }
      }

      #endregion 1. Call RECEIVE to get full XML document

      if(receiveOK == false)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Stop, "Neuspješno primanje XML dokumenta. Ne mogu RE_RECEIVE");

         return;
      }

      #region DELREC AUR Xtrano

      bool delOK = xtrano_rec.VvDao.DELREC(TheDbConnection, xtrano_rec, true, false);

      if(delOK == false)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Stop, "Neuspješno brisanje AUR Xtrano zapisa prije ponovnog unosa.");
         return;
      }
      else
      {
         ((F2_Ulaz_UC)TheVvUC).INIT_FUR(); // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX 
      }

      #endregion DELREC AUR Xtrano
   }
   private void F2_ShowArhivaPDF_OLD (object sender, EventArgs e)
   {
      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      Faktur faktur_rec = theDUC.faktur_rec;

      if(faktur_rec.F2_ArhRecID.IsZero()) { ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Stop, "Nema spremljenih PDF-ova za ovaj eRačun."); return; }

      Xtrano xtrano_rec = new Xtrano();
      xtrano_rec.VvDao.SetMe_Record_byRecID(TheDbConnection, xtrano_rec, faktur_rec.F2_ArhRecID, false);

      //List<byte[]> pdfBytesList = xtrano_rec.F2_Get_PDF_Bytes_List();
      //List<string> pdfFileNameList = xtrano_rec.F2_GetPdfFilenamesFrom_eRacun();
      //
      //byte[] pdfBytes = pdfBytesList[0]; // your PDF byte array
      //
      //string filename = pdfFileNameList[0];
      //string dirame   = VvPref.eRacun_Izlaz_Prefs.DirectoryName;
      //string fullName = Path.Combine(dirame, filename);
      //File.WriteAllBytes(fullName, pdfBytes);
      //System.Diagnostics.Process.Start(fullName);

      ShowPDF_FromXtranoArhiva(xtrano_rec);
      

      // OVO je za PDFiumViewer testiranje ... ali si odustao od toga JER CEMO SA SYNCFUSION-om! 
      //using(var stream = new MemoryStream(pdfBytes))
      //{
      //   var pdfDocument = PdfiumViewer.PdfDocument.Load(stream);
      //   var pdfViewer = new PdfiumViewer.PdfViewer();
      //   pdfViewer.Document = pdfDocument;
      //   pdfViewer.Dock = DockStyle.Fill;
      //   this.Controls.Add(pdfViewer); // 'this' is your Form or UserControl
      //}

   }
   private void F2_ShowArhivaPDF(object sender, EventArgs e)
   {
      // Determine which UC type we're working with and get the selected row index
      int rowIdx = -1;
      Xtrano xtrano_rec = null;

      if(TheVvUC is FakturExtDUC)
      {
         // Called from FakturDUC - get faktur directly
         FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;
         Faktur faktur_rec = theDUC.faktur_rec;

         if(faktur_rec.F2_ArhRecID.IsZero())
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Stop, "Nema spremljenih PDF-ova za ovaj eRačun.");
            return;
         }

         xtrano_rec = new Xtrano();
         xtrano_rec.VvDao.SetMe_Record_byRecID(TheDbConnection, xtrano_rec, faktur_rec.F2_ArhRecID, false);
      }
      else if(TheVvUC is F2_Izlaz_UC)
      {
         // Called from F2_Izlaz_UC grid - get selected row
         F2_Izlaz_UC theUC = TheVvUC as F2_Izlaz_UC;

         if(theUC.TheG.SelectedCells.Count == 0)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Molimo odaberite red iz tablice.");
            return;
         }

         rowIdx = theUC.TheG.SelectedCells[0].RowIndex;

         if(rowIdx < 0 || rowIdx >= theUC.TheFakturList.Count)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Neispravan odabir reda.");
            return;
         }

         Faktur faktur_rec = theUC.TheFakturList[rowIdx];

         if(faktur_rec.F2_ArhRecID.IsZero())
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Stop, "Nema spremljenih PDF-ova za ovaj eRačun.");
            return;
         }

         xtrano_rec = new Xtrano();
         xtrano_rec.VvDao.SetMe_Record_byRecID(TheDbConnection, xtrano_rec, faktur_rec.F2_ArhRecID, false);
      }
      else if(TheVvUC is F2_Ulaz_UC)
      {
         // Called from F2_Ulaz_UC grid - get selected Xtrano directly
         F2_Ulaz_UC theUC = TheVvUC as F2_Ulaz_UC;

         if(theUC.TheG.SelectedCells.Count == 0)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Molimo odaberite red iz tablice.");
            return;
         }

         rowIdx = theUC.TheG.SelectedCells[0].RowIndex;

         if(rowIdx < 0 || rowIdx >= theUC.TheXtranoList.Count)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Neispravan odabir reda.");
            return;
         }

         xtrano_rec = theUC.TheXtranoList[rowIdx];

         xtrano_rec.VvDao.SetMe_Record_byRecID(TheDbConnection, xtrano_rec, xtrano_rec.T_recID, false);
      }

      if(xtrano_rec == null)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Ne mogu pronaći arhivu za prikaz PDF-a.");
         return;
      }

      ShowPDF_FromXtranoArhiva(xtrano_rec);
   }
   private void F2_ShowArhivaXML(object sender, EventArgs e)
   {
      // Determine which UC type we're working with and get the selected row index
      int rowIdx = -1;
      Xtrano xtrano_rec = null;

      if(TheVvUC is FakturExtDUC)
      {
         // Called from FakturDUC - get faktur directly
         FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;
         Faktur faktur_rec = theDUC.faktur_rec;

         if(faktur_rec.F2_ArhRecID.IsZero())
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Stop, "Nema spremljenih XML-ova za ovaj eRačun.");
            return;
         }

         xtrano_rec = new Xtrano();
         xtrano_rec.VvDao.SetMe_Record_byRecID(TheDbConnection, xtrano_rec, faktur_rec.F2_ArhRecID, false);
      }
      else if(TheVvUC is F2_Izlaz_UC)
      {
         // Called from F2_Izlaz_UC grid - get selected row
         F2_Izlaz_UC theUC = TheVvUC as F2_Izlaz_UC;

         if(theUC.TheG.SelectedCells.Count == 0)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Molimo odaberite red iz tablice.");
            return;
         }

         rowIdx = theUC.TheG.SelectedCells[0].RowIndex;

         if(rowIdx < 0 || rowIdx >= theUC.TheFakturList.Count)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Neispravan odabir reda.");
            return;
         }

         Faktur faktur_rec = theUC.TheFakturList[rowIdx];

         if(faktur_rec.F2_ArhRecID.IsZero())
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Stop, "Nema spremljenih XML-ova za ovaj eRačun.");
            return;
         }

         xtrano_rec = new Xtrano();
         xtrano_rec.VvDao.SetMe_Record_byRecID(TheDbConnection, xtrano_rec, faktur_rec.F2_ArhRecID, false);
      }
      else if(TheVvUC is F2_Ulaz_UC)
      {
         // Called from F2_Ulaz_UC grid - get selected Xtrano directly
         F2_Ulaz_UC theUC = TheVvUC as F2_Ulaz_UC;

         if(theUC.TheG.SelectedCells.Count == 0)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Molimo odaberite red iz tablice.");
            return;
         }

         rowIdx = theUC.TheG.SelectedCells[0].RowIndex;

         if(rowIdx < 0 || rowIdx >= theUC.TheXtranoList.Count)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Neispravan odabir reda.");
            return;
         }

         xtrano_rec = theUC.TheXtranoList[rowIdx];

         xtrano_rec.VvDao.SetMe_Record_byRecID(TheDbConnection, xtrano_rec, xtrano_rec.T_recID, false);
      }

      if(xtrano_rec == null)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Ne mogu pronaći arhivu za prikaz XML-a.");
         return;
      }

      ShowXML_FromXtranoArhiva(xtrano_rec);
   }
   private bool ShowXML_FromXtranoArhiva(Xtrano xtrano_rec)
   {
      string theXmlString = VvStringCompressor.DecompressXml(xtrano_rec.T_XmlZip);

      string fileName = "eRacun_" + xtrano_rec.F2_ElectronicID.ToString() + "_" + xtrano_rec.T_opis_128 + ".xml";

      //string dirName   = VvPref.eRacun_Izlaz_Prefs.DirectoryName;

      string deaultVvPDFdirectoryName = VvForm.GetLocalDirectoryForVvFile(/*"_ eRacun IZLAZNI"*/ZXC.eRacuniDIR);
      string todayDir = /*theTT*/xtrano_rec.T_TT + "_PDF_" + DateTime.Today.ToString(ZXC.VvDateYyyyMmDdMySQLFormat);

      string dirName = Path.Combine(deaultVvPDFdirectoryName, todayDir);

      string fullName = Path.Combine(dirName, fileName);

      if(!Directory.Exists(dirName))
      {
         Directory.CreateDirectory(dirName);
      }

      bool saveOK = EN16931.UBL.InvoiceType.VvSaveToFile(theXmlString, fullName, ZXC.VvUTF8Encoding_noBOM);

      System.Diagnostics.Process.Start(fullName);

      return true;
   }
   private void F2_ShowDiskPDFinXML(object sender, EventArgs e)
   {
      OpenFileDialog openFileDialog = new OpenFileDialog();

    //openFileDialog.InitialDirectory = VvForm.GetLocalDirectoryForVvFile(VvPref.VvMailData.DeaultVvPDFdirectoryName);
      openFileDialog.InitialDirectory = VvForm.GetLocalDirectoryForVvFile(/*"_ eRacun IZLAZNI"*/ZXC.eRacuniDIR);

      openFileDialog.Filter = "Xml datoteke (*.xml)|*.xml|Sve Datoteke (*.*)|*.*";
      openFileDialog.FilterIndex = 1;
      openFileDialog.RestoreDirectory = true;

      if(openFileDialog.ShowDialog() == DialogResult.OK)
      {
         string full_XML_PathName = openFileDialog.FileName;

         List<string> pdfPaths = Vv_eRacun_HTTP.ExtractEmbeddedPdfsFromXmlFile(full_XML_PathName);
         
         int counter = 0;

         foreach(string pdfPath in pdfPaths)
         {
            if(File.Exists(pdfPath))
            {
               counter++;

               if(counter > 1) ZXC.aim_emsg(MessageBoxIcon.Information, $"Prikazujem {counter}. od {pdfPaths.Count}");
               System.Diagnostics.Process.Start(pdfPath);
            }
            else
            {
               ZXC.aim_emsg(MessageBoxIcon.Exclamation, $"Ne mogu pronaći PDF datoteku na disku:\n\r{pdfPath}");
            }
         }
      }

      openFileDialog.Dispose(); // !!! 
   }
   private void F2_ShowDiskXML(object sender, EventArgs e)
   {
      OpenFileDialog openFileDialog = new OpenFileDialog();

    //openFileDialog.InitialDirectory = VvForm.GetLocalDirectoryForVvFile(VvPref.VvMailData.DeaultVvPDFdirectoryName);
      openFileDialog.InitialDirectory = VvForm.GetLocalDirectoryForVvFile(/*"_ eRacun IZLAZNI"*/ZXC.eRacuniDIR);

      openFileDialog.Filter = "Xml datoteke (*.xml)|*.xml|Sve Datoteke (*.*)|*.*";
      openFileDialog.FilterIndex = 1;
      openFileDialog.RestoreDirectory = true;

      if(openFileDialog.ShowDialog() == DialogResult.OK)
      {
         string full_XML_PathName = openFileDialog.FileName;

         if(full_XML_PathName.NotEmpty())
         {
            System.Diagnostics.Process.Start(full_XML_PathName);
         }
      }

      openFileDialog.Dispose(); // !!! 
   }
   private void F2_AddNapomena_UFA(object sender, EventArgs e)
   {
      F2_Ulaz_UC theUC = TheVvUC as F2_Ulaz_UC;

      if(theUC.TheG.SelectedCells.Count == 0)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Molimo odaberite jedan red iz tablice.");
         return;
      }
    
      int rowIdx = theUC.TheG.SelectedCells[0].RowIndex;

      string tt    = theUC.TheG.GetStringCell(theUC.DgvCI.iT_tt   , rowIdx, false);
      uint   ttNum = theUC.TheG.GetUint32Cell(theUC.DgvCI.iT_ttNum, rowIdx, false);
      
      Faktur faktur_rec = new Faktur();

      FakturDao.SetMeFaktur(TheDbConnection, faktur_rec, tt, ttNum, false);

      F2_FUR_addNpomenaUFA_Dlg dlg = new F2_FUR_addNpomenaUFA_Dlg(faktur_rec);

      DialogResult dlgResult = dlg.ShowDialog();

      if(dlgResult != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      string newNapomena = dlg.Fld_Napomena;

      dlg.Dispose();

      BeginEdit(faktur_rec);

      faktur_rec.Napomena = newNapomena;

      bool rwtOK = faktur_rec.VvDao.RWTREC(theUC.TheDbConnection, faktur_rec, false, true, false);

      EndEdit(faktur_rec);

      Xtrano xtrano_rec = theUC.TheXtranoList[rowIdx];

      theUC.PutDgvLineFields(rowIdx, xtrano_rec);
   }
   private void F2_RefreshNIR_FtransLis(object sender, EventArgs e)
   {
      ((F2_NIR_UC)TheVvUC).INIT_NIR(); // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX 
   }
   private void F2_MAPaj_From_NIR(object sender, EventArgs e)
   {
      Vv_eRacun_HTTP.InitProjectData();

      int newsCount = /*NNN*/Vv_eRacun_HTTP.Create_MAP_XML_From_NIR(TheVvUC as F2_NIR_UC);

      if(newsCount.IsZeroOrPositive()) F2_RefreshNIR_FtransLis(sender, e); // -1 means 'cancel' button clicked 
   }
   private void F2_MAPaj_From_NIR_zaRzadoblje(object sender, EventArgs e)
   {
      F2_ZaRazdoblje_Dlg dlg = new F2_ZaRazdoblje_Dlg("Razdoblje uplata za prijavu naplata:");

      DialogResult dlgResult = dlg.ShowDialog();

      if(dlgResult != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }
      DateTime datumOd = dlg.Fld_DatumOd;
      DateTime datumDo = dlg.Fld_DatumDo;

      dlg.Dispose();

      Vv_eRacun_HTTP.InitProjectData();

      int newsCount = /*NNN*/Vv_eRacun_HTTP.Create_MAP_XML_From_NIR(TheVvUC as F2_NIR_UC, true, datumOd, datumDo);

      if(newsCount.IsZeroOrPositive()) F2_RefreshNIR_FtransLis(sender, e); // -1 means 'cancel' button clicked 
   }
   private void F2_HDD_Outbox(object sender, EventArgs e)
   {
      Vv_eRacun_HTTP.InitProjectData();

      if(Vv_eRacun_HTTP.Is_FIR_ON() == false) return;

      Vv_eRacun_HTTP.Load_IRn_FakturList((F2_Izlaz_UC)TheVvUC);

      int newsCount = /*QQQ*/Vv_eRacun_HTTP.HDD_Import_Extern_Faktur_IFA((F2_Izlaz_UC)TheVvUC);

      //if(newsCount.IsZeroOrPositive()) F2_RefreshFIR_FakturListAndStatuses(sender, e); // -1 means 'cancel' button clicked 
   }
   private void F2_HDD_Inbox(object sender, EventArgs e)
   {
      Vv_eRacun_HTTP.InitProjectData();

      if(Vv_eRacun_HTTP.Is_FUR_ON() == false) return;

      int newsCount = /*KKK*/Vv_eRacun_HTTP.HDD_Import_Extern_Faktur_UFA((F2_Ulaz_UC)TheVvUC);

      //if(newsCount.IsZeroOrPositive()) ((F2_Ulaz_UC)TheVvUC).INIT_FUR(); // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX 
   }
   private void F2_ExportFIRXml(object sender, EventArgs e)
   {
      F2_ExportXml_Job(Mixer.TT_AIR);
   }
   private void F2_ExportFURXml(object sender, EventArgs e)
   {
      F2_ExportXml_Job(Mixer.TT_AUR);
   }
   private void F2_ExportXml_Job(string xtrTT) 
   { 
      F2_ZaRazdoblje_Dlg dlg = new F2_ZaRazdoblje_Dlg("Razdoblje za export xml-a:");

      DialogResult dlgResult = dlg.ShowDialog();

      if(dlgResult != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }
      DateTime datumOd = dlg.Fld_DatumOd;
      DateTime datumDo = dlg.Fld_DatumDo;

      dlg.Dispose();

      if(datumOd.IsEmpty()) datumOd = ZXC.projectYearFirstDay;
      if(datumDo.IsEmpty()) datumDo = ZXC.projectYearLastDay ;

      string dateID = datumOd.ToString(ZXC.VvDateDdMmYyyyFormat) + "_" + datumDo.ToString(ZXC.VvDateDdMmYyyyFormat);
      string fileName, fullName;
      string deaultVvPDFdirectoryName = VvForm.GetLocalDirectoryForVvFile(/*"_ eRacun IZLAZNI"*/ZXC.eRacuniDIR);
      string todayDir = xtrTT + "_XML_EXPORT_" + DateTime.Today.ToString(ZXC.VvDateYyyyMmDdMySQLFormat);
      string dirName = Path.Combine(deaultVvPDFdirectoryName, todayDir);
      uint expCount = 0;
      bool saveOK;

      if(!Directory.Exists(dirName))
      {
         Directory.CreateDirectory(dirName);
      }

      string theXmlString;

      List<Xtrano> xtranoList = new List<Xtrano>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      filterMembers.Add(new VvSqlFilterMember(ZXC.XtranoSchemaRows[ZXC.XtoCI.t_tt], "theTT", xtrTT, " = "));

      if(xtrTT == Mixer.TT_AIR)
      {
         filterMembers.Add(new VvSqlFilterMember(ZXC.XtranoSchemaRows[ZXC.XtoCI.t_dokDate], "theDokDateOd", datumOd, " >= "));
         filterMembers.Add(new VvSqlFilterMember(ZXC.XtranoSchemaRows[ZXC.XtoCI.t_dokDate], "theDokDateDo", datumDo, " <= "));
      }
      else if(xtrTT == Mixer.TT_AUR)
      {
         filterMembers.Add(new VvSqlFilterMember(ZXC.XtranoSchemaRows[ZXC.XtoCI.t_dokDate2], "theDokDate2Od", datumOd, " >= "));
         filterMembers.Add(new VvSqlFilterMember(ZXC.XtranoSchemaRows[ZXC.XtoCI.t_dokDate2], "theDokDate2Do", datumDo, " <= "));
      }

      VvDaoBase.LoadGenericVvDataRecordList<Xtrano>(TheDbConnection, xtranoList, filterMembers, "", "t_dokDate ", false);

      foreach(Xtrano xtrano_rec in xtranoList)
      {
         theXmlString = VvStringCompressor.DecompressXml(xtrano_rec.T_XmlZip);

         fileName = xtrTT + "_" + ZXC.CURR_prjkt_rec.Ticker + "_" + xtrano_rec.T_dokNum + ".xml";

         fullName = Path.Combine(dirName, fileName);

         saveOK = EN16931.UBL.InvoiceType.VvSaveToFile(theXmlString, fullName, ZXC.VvUTF8Encoding_noBOM);

         if(saveOK) expCount++;
         else ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Neuspješno spremanje XML datoteke.\n\nPutanja: {0}", fullName);

      }

      ZXC.aim_emsg(MessageBoxIcon.Information, $"Gotovo, exportirao {expCount} računa u direcctory\n\r\n\r{dirName}");
   }
   private void F2_FindOn_FIR_grid(object sender, EventArgs e) 
   {
      if(TheVvUC is F2_Izlaz_UC == false) return;

      F2_FindOnGrid((TheVvUC as F2_Izlaz_UC).TheG);
      
      //(TheVvUC as F2_Izlaz_UC).INIT_FIR();
   }
   private void F2_FindOn_FUR_grid(object sender, EventArgs e) 
   {
      if(TheVvUC is F2_Ulaz_UC == false) return;

      F2_FindOnGrid((TheVvUC as F2_Ulaz_UC ).TheG); 

      //(TheVvUC as F2_Ulaz_UC).INIT_FUR();
   }
   private void F2_FindOnGrid(VvDataGridView TheG)
   { 
      F2_Find_Dlg dlg = new F2_Find_Dlg(TheG);

      DialogResult dlgResult = dlg.ShowDialog();

      if(dlgResult != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      dlg.Dispose();
   }
   private void F2_SortFUR_kupdob(object sender, EventArgs e)
   {
      F2_Ulaz_UC theUC = TheVvUC as F2_Ulaz_UC;

      theUC.TheXtranoList = theUC.TheXtranoList.OrderBy(xtr => xtr.T_opis_128).ThenBy(xtr => xtr.T_theString).ToList();

      theUC.IsOrigSort    = false;
      theUC.IsPartnerSort = true;

      theUC.SetEnableDisableTsButtons(/*false*/);
      theUC.PutDgvFields();
   }
   private void F2_SortFIR_kupdob(object sender, EventArgs e)
   {
      F2_Izlaz_UC theUC = TheVvUC as F2_Izlaz_UC;

      if(theUC.TheFakturList.IsEmpty()) return;

      theUC.TheFakturList = theUC.TheFakturList.OrderBy(fak => fak.KupdobName).ThenBy(fak => fak.TtNum).ToList();

      theUC.IsOrigSort    = false;
      theUC.IsPartnerSort = true;

      theUC.SetEnableDisableTsButtons(/*false*/);
      theUC.PutDgvFields();
   }
   private void F2_FUR_InvertSort(object sender, EventArgs e)
   {
      F2_Ulaz_UC theUC = TheVvUC as F2_Ulaz_UC;

      theUC.TheXtranoList.Reverse();

      if(theUC.IsPartnerSort == false) theUC.IsOrigSort = !theUC.IsOrigSort;
      else                             theUC.IsOrigSort = false            ;

      theUC.SetEnableDisableTsButtons();
      theUC.PutDgvFields();
   }
   private void F2_FIR_InvertSort(object sender, EventArgs e)
   {
      F2_Izlaz_UC theUC = TheVvUC as F2_Izlaz_UC;

      if(theUC.TheFakturList.IsEmpty()) return;

      theUC.TheFakturList.Reverse();

      if(theUC.IsPartnerSort == false) theUC.IsOrigSort = !theUC.IsOrigSort;
      else                             theUC.IsOrigSort = false            ;

      theUC.SetEnableDisableTsButtons(/*false*/);
      theUC.PutDgvFields();
   }
   private void F2_FUR_OriginalSort(object sender, EventArgs e)
   {
      F2_Ulaz_UC theUC = TheVvUC as F2_Ulaz_UC;

      theUC.TheXtranoList = theUC.TheXtranoList.OrderByDescending(xtr => xtr.T_dokDate).ToList();

      theUC.IsOrigSort    = true;
      theUC.IsPartnerSort = false;

      theUC.SetEnableDisableTsButtons(/*true*/);
      theUC.PutDgvFields();
   }
   private void F2_FIR_OriginalSort(object sender, EventArgs e)
   {
      F2_Izlaz_UC theUC = TheVvUC as F2_Izlaz_UC;

      if(theUC.TheFakturList.IsEmpty()) return;

      theUC.TheFakturList = theUC.TheFakturList.OrderByDescending(fak => fak.TtNum).ToList();

      theUC.IsOrigSort    = true;
      theUC.IsPartnerSort = false;

      theUC.SetEnableDisableTsButtons(/*true*/);
      theUC.PutDgvFields();
   }

}
