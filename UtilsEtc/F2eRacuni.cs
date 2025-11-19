using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Net.Http.Headers;
using CrystalDecisions.Shared;
using System.Net.Security;



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
   private const bool DEMO = false;

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
   public const string VvMER_webAddressPOST_MAP             = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/api/fiscalization/markPaid"                   ; // POST 
   public const string VvMER_webAddressPOST_MAP_WO_eID      = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/api/fiscalization/markPaidWithoutElectronicID"; // POST 

   public const string VvPND_baseAddress_production = @"https://eracun.eposlovanje.hr"; 
   public const string VvPND_baseAddress_demo       = @"https://test.eposlovanje.hr"; 

   public const string VvPND_webAddressPOST_Send            = (DEMO ? VvPND_baseAddress_demo : VvPND_baseAddress_production) + "/api/v2/document/send"            ; // POST 
   public const string VvPND_webAddressPOST_eIzvj           = (DEMO ? VvPND_baseAddress_demo : VvPND_baseAddress_production) + "/api/v2/ereporting/reportdocument"; // POST 
   public const string VvPND_webAddressPOST_outgoing        = (DEMO ? VvPND_baseAddress_demo : VvPND_baseAddress_production) + "/api/v2/document/outgoing"        ; // GET! 
   public const string VvPND_webAddressGET_Ping             = (DEMO ? VvPND_baseAddress_demo : VvPND_baseAddress_production) + "/api/v2/ping"                     ; // GET! 
   public const string VvPND_webAddressPOST_Check           = (DEMO ? VvPND_baseAddress_demo : VvPND_baseAddress_production) + "/api/v2/ams/check"                ; // POST 

   // MER authorisation parameters: 
   public static string VvMER_UserName   = DEMO ? "8633"    : ZXC.ValOrZero_Int(ZXC.CURR_prjkt_rec.SkyVvDomena).ToString();
   public static string VvMER_Password   = DEMO ? "T22zsEY" : ZXC.CURR_prjkt_rec.SkyPasswordDecrypted                     ;
   public static string VvMER_CompanyId  =                    ZXC.CURR_prjkt_rec.Oib                                      ;
   public const  string VvMER_SoftwareId =                    "Vektor-001"                                                ;

   // PND authorisation parameters: 
   public static string VvPND_API_Key    = "1042a7915a7f66a23a8e0e98d93cb44c6d968263638a8cec54e07cb5abc2ae2f";
   public static string VvPND_CompanyId  = ZXC.CURR_prjkt_rec.Oib;
   public const  string VvPND_SoftwareId = "Vektor-001"          ;

   public static readonly Dictionary</*string*/int, string> MER_TransportStatuses = new Dictionary</*string*/int, string>
   {
      { /*"20"*/ 20, "U obradi"           }, // TRN 
      { /*"30"*/ 30, "Poslan"             }, // TRN 
      { /*"40"*/ 40, "Preuzet"            }, // TRN 
      { /*"45"*/ 45, "Otkazano"           }, // TRN 
      { /*"50"*/ 50, "Neuspjelo"          }, // TRN 

      {           0, "Prihvaćen"          }, // DPS 
      { /*"20"*/  1, "Odbijen"            }, // DPS 
      { /*"30"*/  2, "Plaćeno SVE"        }, // DPS 
      { /*"40"*/  3, "Plaćen DIO"         }, // DPS 
      { /*"45"*/  4, "Potvrda zaprimanja" }, // DPS 
      { /*"50"*/ 99, "Zaprimljen"         }  // DPS 
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

            if(webAddress == VvMER_webAddressPOST_Check)
            {
               // ne tretiraj statusCode koji nije 200 kao exception,                
               // nego kao odgovor 'false' za MER-ov API 'VvMER_WebService_CheckAMS' 
            }
            else
            {
               ZXC.aim_emsg_List("Web Exception (status code:" + (int)resp.StatusCode + ") Description: " + resp.StatusDescription, responseBodyLines.ToList());
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

   //staro, dellmelatter
   //private static HttpWebResponse Vv_GETmethod_SendHttpWebRequest_GetHttpWebResponse(string urlWithParams, string token = null)
   //{
   //   HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlWithParams);
   //   httpWebRequest.ContentType    = "application/json; charset=utf-8";
   //   httpWebRequest.Method         = "GET";
   //
   //   // Add Authorization header if provided
   //   if(token.NotEmpty())
   //   {
   //      httpWebRequest.Headers["Authorization"] = token;
   //   }
   //
   //   return (HttpWebResponse)httpWebRequest.GetResponse();
   //}

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

#if DELLMELATTERR
   private static T OLD_QWE_Vv_POSTmethod_ExecuteJson<T>(string webAddress, string jsonRequestString, /*Action<T, string> saveToFile = null, string fileName = null,*/ string token = null)
      where T : class, new()
   {
      T deserializedResponseData = null;

      try
      {
         HttpWebResponse httpResponse = Vv_POSTmethod_SendHttpWebRequest_GetHttpWebResponse(webAddress, jsonRequestString, token);

         using(StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
         {
            string responseJson = streamReader.ReadToEnd();

            if(responseJson.NotEmpty())
            {
               try
               {
                  deserializedResponseData = JsonConvert.DeserializeObject<T>(responseJson);

                  // Cuvamo originalni JSON odgovor unutar objekta odgovora 
                  if(deserializedResponseData is VvMER_Response_Data_AllActions resp)
                  {
                     resp.ResponseJson = responseJson;
                  }

                  // Optionally save to file if provided
                  //saveToFile?.Invoke(deserializedResponseData, fileName);
               }
               catch(Exception ex2)
               {
                  // Fallback error handling for deserialization
                  deserializedResponseData = new T();
                  var jsonMsgList = new List<string>();

                  JsonTextReader reader = new JsonTextReader(new StringReader(responseJson));
                  while(reader.Read())
                  {
                     if(reader.Value != null)
                     {
                        jsonMsgList.Add(reader.Value.ToString());
                     }
                  }
                //ZXC.aim_emsg_List("Response Messages from JsonTextReader", errorMsg);
                  ZXC.aim_emsg_List("Exception: " + ex2.Message, jsonMsgList);
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
      catch(Exception ex)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška pri Vv_POSTmethod_ExecuteJson: {0}", ex.Message);
      }

      return deserializedResponseData;
   }
#endif
   private static WebApiResult<T> Vv_POSTmethod_ExecuteJson<T>(string webAddress, string jsonRequestString, string token = null) where T : class, new()
   {
      WebApiResult<T> webApiResult = new WebApiResult<T>();

      try
      {
         HttpWebResponse httpResponse = Vv_POSTmethod_SendHttpWebRequest_GetHttpWebResponse(webAddress, jsonRequestString, token);

         webApiResult.StatusCode = (int)httpResponse.StatusCode;
         webApiResult.StatusDescription = httpResponse.StatusDescription;

         using(StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
         {
            string responseJson = streamReader.ReadToEnd();

            webApiResult.ResponseJson = responseJson;

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

   private static string VvMER_Json_SerializeObjectForRequestString_AllActions(VvMER_Request_Data_AllActions json_AllActions_Request_Data)
   {
      return JsonConvert.SerializeObject(json_AllActions_Request_Data, Newtonsoft.Json.Formatting.Indented, VvMER_JsonSerializerSettings_Default());
   }

   #endregion Utils

   #region Concrete API / EndPoint methods implementations - 'ZEBRA'

   //######################## https://www.moj-eracun.hr/apis/v2/send #############################################################################################################
   public static WebApiResult<VvMER_Response_Data_AllActions> /*VvMER_Response_Data_AllActions*/ VvMER_WebService_SEND(string xmlString, string fullPath_XML_FileName)
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

      string webServiceEndPointAddress = VvMER_webAddressPOST_Send;

      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(xmlString);

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<VvMER_Response_Data_AllActions> webApiResult = 
         
         Vv_POSTmethod_ExecuteJson<VvMER_Response_Data_AllActions>
         (
            webServiceEndPointAddress,
            jsonRequestString//,
            //(data, fileName) => { if(fileName.NotEmpty()) data.SaveToFile(fileName); },
            //fullPath_XML_FileName.Replace(".xml", "_RES.xml")
         );

      return webApiResult/*.ResponseData*/;
   }
   public  static VvMER_Response_Data_AllActions VvPND_WebService_SEND(string xmlString, string fullPath_XML_FileName)
   {
      string webServiceEndPointAddress = VvPND_webAddressPOST_Send;

      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(xmlString);

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<VvMER_Response_Data_AllActions> webApiResult =

         Vv_POSTmethod_ExecuteJson<VvMER_Response_Data_AllActions>
         (
            webServiceEndPointAddress,
            jsonRequestString,
            //(data, fileName) => { if(fileName.NotEmpty()) data.SaveToFile(fileName); },
            //fullPath_XML_FileName.Replace(".xml", "_RES.xml"),
            VvPND_API_Key
         );

      return webApiResult.ResponseData;
   }

   //######################## F2_eIzvj API #######################################################################################################################################
   public  static WebApiResult<VvMER_Response_Data_AllActions> /*VvMER_Response_Data_AllActions*/ VvMER_WebService_eIzvj(string xmlString, DateTime _DeliveryDate, bool _IsCopy, string _InvoiceType)
   {
      string webServiceEndPointAddress = VvMER_webAddressPOST_eIzvj;

      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions()
      {
         xmlInvoice   = xmlString,
         DeliveryDate = _DeliveryDate,
         IsCopy       = _IsCopy,
         InvoiceType  = _InvoiceType
      };

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<VvMER_Response_Data_AllActions> webApiResult =

         Vv_POSTmethod_ExecuteJson<VvMER_Response_Data_AllActions>
         (
            webServiceEndPointAddress,
            jsonRequestString
         );

      return webApiResult/*.ResponseData*/;
   }
   public  static VvMER_Response_Data_AllActions VvPND_WebService_eIzvj(string xmlString, string _InvoiceType)
   {
      string webServiceEndPointAddress = VvPND_webAddressPOST_eIzvj;

      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions()
      {
         document   = xmlString,
         type       = _InvoiceType
      };

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<VvMER_Response_Data_AllActions> webApiResult =

         Vv_POSTmethod_ExecuteJson<VvMER_Response_Data_AllActions>
         (
            webServiceEndPointAddress,
            jsonRequestString,
            VvPND_API_Key
         );

      return webApiResult.ResponseData;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/queryOutbox - one single TRN status ##############################################################################
   public static VvMER_Response_Data_AllActions VvMER_WebService_QueryOutbox_TRN_Single(uint electronicID)
   {
      string webServiceEndPointAddress = VvMER_webAddressPOST_QueryOutbox;

      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(electronicID); // constructor za STATUS jednog racuna (electronicID-a) 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

         WebApiResult<List<WebApiResult<VvMER_Response_Data_AllActions>>> webApiResult = Vv_POSTmethod_ExecuteJson<List<WebApiResult<VvMER_Response_Data_AllActions>>>(webServiceEndPointAddress, jsonRequestString);

      VvMER_Response_Data_AllActions responseData_AllActions = webApiResult.ResponseData.FirstOrDefault()?.ResponseData;

      return responseData_AllActions;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/queryDocumentProcessStatusOutbox - one single DPS status #########################################################
   public static VvMER_Response_Data_AllActions VvMER_WebService_QueryOutbox_DPS_Single(uint electronicID)
   {
      string webServiceEndPointAddress = VvMER_webAddressPOST_QueryOutbox_DPS;

      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(electronicID); // constructor za STATUS jednog racuna (electronicID-a) 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<List<VvMER_Response_Data_AllActions>> webApiResult = Vv_POSTmethod_ExecuteJson<List<VvMER_Response_Data_AllActions>>(webServiceEndPointAddress, jsonRequestString);

      VvMER_Response_Data_AllActions responseData_AllActions = webApiResult.ResponseData.FirstOrDefault();

      return responseData_AllActions;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/queryOutbox - TRN Status List ####################################################################################
   public  static List<VvMER_Response_Data_AllActions> VvMER_WebService_QueryOutbox_TRN_List(DateTime dateOD, DateTime dateDO)
   {
      string webServiceEndPointAddress = VvMER_webAddressPOST_QueryOutbox;

      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(dateOD, dateDO); // constructor za Listu STATUSa racuna (dateOD, dateDO) 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<List<VvMER_Response_Data_AllActions>> webApiResult = Vv_POSTmethod_ExecuteJson<List<VvMER_Response_Data_AllActions>>(webServiceEndPointAddress, jsonRequestString);

      return webApiResult.ResponseData;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/queryDocumentProcessStatusOutbox - DPS Status List ###############################################################
   public static List<VvMER_Response_Data_AllActions> VvMER_WebService_QueryOutbox_DPS_List(DateTime dateOD, DateTime dateDO)
   {
      string webServiceEndPointAddress = VvMER_webAddressPOST_QueryOutbox_DPS;

      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(dateOD, dateDO); // constructor za Listu STATUSa racuna (dateOD, dateDO) 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<List<VvMER_Response_Data_AllActions>> webApiResult = Vv_POSTmethod_ExecuteJson<List<VvMER_Response_Data_AllActions>>(webServiceEndPointAddress, jsonRequestString);

      return webApiResult.ResponseData;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/queryInbox - Status List #########################################################################################
   public  static List<VvMER_Response_Data_AllActions> VvMER_WebService_QueryInbox_List(DateTime dateOD, DateTime dateDO)
   {
      string webServiceEndPointAddress = VvMER_webAddressPOST_QueryInbox;

      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(dateOD, dateDO); // constructor za Listu STATUSa racuna (dateOD, dateDO) 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<List<VvMER_Response_Data_AllActions>> webApiResult = Vv_POSTmethod_ExecuteJson<List<VvMER_Response_Data_AllActions>>(webServiceEndPointAddress, jsonRequestString);

      return webApiResult.ResponseData;
   }

   //######################## https://www.moj-eracun.hr/api/fiscalization/status - Get 3 kind FISK status ########################################################################
   public  static VvMER_Response_Data_AllActions VvMER_WebService_Get_FISK_Status(uint electronicID, string messageType)
   {
      string webServiceEndPointAddress = VvMER_webAddressPOST_FiskStatus;

      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(electronicID) { MessageType = messageType }; 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<List<VvMER_Response_Data_AllActions>> webApiResult = Vv_POSTmethod_ExecuteJson<List<VvMER_Response_Data_AllActions>>(webServiceEndPointAddress, jsonRequestString);

      VvMER_Response_Data_AllActions responseData_AllActions = webApiResult.ResponseData.NotEmpty() ? webApiResult.ResponseData.FirstOrDefault() : null;

      return responseData_AllActions;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/receive - one single document ####################################################################################
   public static VvMER_Response_Data_AllActions VvMER_WebService_Receive_XML(uint electronicID)
   {
      string webServiceEndPointAddress = VvMER_webAddressPOST_Receive;

      VvMER_Response_Data_AllActions responseData = new VvMER_Response_Data_AllActions();

      try
      {
         VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(electronicID); // constructor za RECEIVE jednog racuna (electronicID-a) 

         string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

         HttpWebResponse httpResponse = Vv_POSTmethod_SendHttpWebRequest_GetHttpWebResponse(webServiceEndPointAddress, jsonRequestString);

         using(StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
         {
            string responseXml = streamReader.ReadToEnd();
            if(responseXml.NotEmpty())
            {
               responseData.DocumentXml = responseXml;
            }
            else
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "XML response is empty!");
            }
         }
      }
      catch(WebException ex)
      {
         ZXC.aim_emsg(ex.Message);
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška pri VvMER_WebService_Receive_XML: {0}", ex.Message);
      }

      return responseData;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/Ping - Checks if service is up ###################################################################################
   public static VvMER_Response_Data_AllActions VvMER_WebService_Ping()
   {
      string webServiceEndPointAddress = VvMER_webAddressGET_Ping;

      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions();
   
      VvMER_Response_Data_AllActions responseData_AllActions = Vv_GETmethod_ExecuteJson<VvMER_Response_Data_AllActions, VvMER_Request_Data_AllActions>(webServiceEndPointAddress, request_Data_AllActions);
   
      return responseData_AllActions;
   }
   public static VvMER_Response_Data_AllActions VvPND_WebService_Ping()
   {
      string webServiceEndPointAddress = VvPND_webAddressGET_Ping;

      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions();

      VvMER_Response_Data_AllActions responseData_AllActions =
          Vv_GETmethod_ExecuteJson<VvMER_Response_Data_AllActions, VvMER_Request_Data_AllActions>
          (
              webServiceEndPointAddress,
              request_Data_AllActions,
              null,
              null,
              VvPND_API_Key 
          );

      return responseData_AllActions;
   }

   //######################## https://www.moj-eracun.hr/api/mps/check - Check Identifier #########################################################################################
   public static WebApiResult<VvMER_Response_Data_AllActions> VvMER_WebService_CheckAMS(string _Identifiervalue)
   {
      string webServiceEndPointAddress = VvMER_webAddressPOST_Check;

      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions() 
      { 
         IdentifierValue = _Identifiervalue,
         IdentifierType  = /*0*/"0",
      };

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<VvMER_Response_Data_AllActions> webApiResult = Vv_POSTmethod_ExecuteJson<VvMER_Response_Data_AllActions>(webServiceEndPointAddress, jsonRequestString);

      return webApiResult/*.ResponseData*/;
   }

   public static VvMER_Response_Data_AllActions VvPND_WebService_CheckAMS(string _identifier)
   {
      string webServiceEndPointAddress = VvPND_webAddressPOST_Check;

      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions() 
      { 
         identifier = _identifier,
         schema     = "9934",
      };

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<VvMER_Response_Data_AllActions> webApiResult = Vv_POSTmethod_ExecuteJson<VvMER_Response_Data_AllActions>(webServiceEndPointAddress, jsonRequestString, VvPND_API_Key);

      return webApiResult.ResponseData;
   }

   //######################## https://www.moj-eracun.hr/api/fiscalization/markPaid - Mark Paid action ############################################################################
   public  static VvMER_Response_Data_AllActions VvMER_WebService_MAP(VvMER_Request_Data_AllActions request_Data_AllActions)
   {
      string webServiceEndPointAddress = VvMER_webAddressPOST_MAP;

    //VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(electronicID) { MessageType = messageType }; 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<List<VvMER_Response_Data_AllActions>> webApiResult = Vv_POSTmethod_ExecuteJson<List<VvMER_Response_Data_AllActions>>(webServiceEndPointAddress, jsonRequestString);

      VvMER_Response_Data_AllActions responseData_AllActions = webApiResult.ResponseData.NotEmpty() ? webApiResult.ResponseData.FirstOrDefault() : null;

      return responseData_AllActions;
   }
   public  static VvMER_Response_Data_AllActions VvMER_WebService_MAP_WO_eID(VvMER_Request_Data_AllActions request_Data_AllActions)
   {
      string webServiceEndPointAddress = VvMER_webAddressPOST_MAP_WO_eID;

    //VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(electronicID) { MessageType = messageType }; 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      WebApiResult<List<VvMER_Response_Data_AllActions>> webApiResult = Vv_POSTmethod_ExecuteJson<List<VvMER_Response_Data_AllActions>>(webServiceEndPointAddress, jsonRequestString);

      VvMER_Response_Data_AllActions responseData_AllActions = webApiResult.ResponseData.NotEmpty() ? webApiResult.ResponseData.FirstOrDefault() : null;

      return responseData_AllActions;
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

   #region FIR / FUR Load List and SubmodulActions



   /* AAA */internal static void Load_IRn_FakturList(F2_Izlaz_UC theUC)
   {
      theUC.TheFakturList = new List<Faktur>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>();

      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt], "theTT", ZXC.RRD.Dsc_F2_TT, " = "));

      string asdDscStr;
      string limitStr = "LIMIT " + (ZXC.RRD.Dsc_F2_NumOfRows.IsPositive() ? ZXC.RRD.Dsc_F2_NumOfRows.ToString() : "100");

      if(ZXC.RRD.Dsc_F2_IsAsc == false) asdDscStr = " DESC ";
      else                              asdDscStr = " ASC " ;

      VvDaoBase.LoadGenericVvDataRecordList<Faktur>(theUC.TheDbConnection, theUC.TheFakturList, filterMembers, "", "ttNum " + asdDscStr + limitStr, true);

      if(theUC.TheFakturList.NotEmpty()) theUC.PutDgvFields();

   }
   /* BBB */internal static void WS_Discover_Candidates_And_Eventually_SEND_eRacune(F2_Izlaz_UC theUC, XSqlConnection conn)
   {
      #region Init & Get Dialog Fields

      //if(ZXC.RRD.Dsc_F2_IsAutoSend == false) return;

      List<Faktur> sendCandidatesFakturList = theUC.TheFakturList.Where(fak => fak.IsF2 && fak.F2_ElectronicID.IsZero()).ToList();

      if(sendCandidatesFakturList.IsEmpty()) return;

      List<VvReportSourceUtil> messageList = new List<VvReportSourceUtil>();

      foreach(Faktur sendCandidateFaktur_rec in sendCandidatesFakturList)
      {
         messageList.Add(new VvReportSourceUtil()
         {
            TheCD      = sendCandidateFaktur_rec.TipBr,
            DevName    = sendCandidateFaktur_rec.DokDate.ToString(ZXC.VvDateFormat),
            KupdobName = sendCandidateFaktur_rec.KupdobName,
            TheMoney   = sendCandidateFaktur_rec.S_ukKCRP
         });
      }

      VvMessageBoxDLG  sendCandidatesFakturList_InfoDLG = new VvMessageBoxDLG (false, ZXC.VvmBoxKind.F2_SEND_candidates);
    //VvMessageBoxForm sendCandidatesFakturList_InfoDLG = new VvMessageBoxForm(false, ZXC.VvmBoxKind.F2_SEND_candidates);
      sendCandidatesFakturList_InfoDLG.Text = "Kandidati za slanje kao eRačun:";

      sendCandidatesFakturList_InfoDLG.TheUC.PutDgvFields_F2_SEND_candidates(messageList);

      DialogResult dlgResult = sendCandidatesFakturList_InfoDLG.ShowDialog();

      if(dlgResult != DialogResult.OK)
      {
         sendCandidatesFakturList_InfoDLG.Dispose();
         return;
      }

      ZXC.RRD.Dsc_F2_IsAutoSend = !sendCandidatesFakturList_InfoDLG.TheUC.Fld_StopAutoSend; ZXC.RRD.SaveDscToLookUpItemList();
      int numOfFirstLinesOnly   =  sendCandidatesFakturList_InfoDLG.TheUC.Fld_NumOfFirstLinesOnly_Send;

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

         sendCandidateFaktur_rec.VvDao.LoadTranses(conn, sendCandidateFaktur_rec, false);

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

      #endregion Finish
   }
   /* CCC */internal static void WS_Refresh_ALL_FIR_Statuses_AndArhiviraj(F2_Izlaz_UC theUC/*, bool isDPS*/) // VOILA! 
   {
      #region Init

      if(theUC.TheFakturList.IsEmpty()) return;

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
      #region Refresh MarkAsPaid_InfoColumns

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

      } // for(int rIdx = 0; rIdx < theUC.TheG.RowCount; ++rIdx)

      #endregion Refresh MarkAsPaid_InfoColumns


      #region Refresh TRN status

      List<Faktur> QueryOutbox_CandidatesFakturList = theUC.TheFakturList.Where(fak => ShouldCheckRefreshed_TRN_Or_DPS_Status(fak, false)).ToList();

      // ovdje bi ako se ide na smislenu kronolosku granicu trebalo filtrirati po fak.F2_SentTS a ne po fak.DokDate !!! 
      // za sada, idemo cijela projektna godina                                                                         
    //DateTime minDokDate = goodCandidatesFakturList.Min(fak => fak.DokDate.Date      );
    //DateTime maxDokDate = goodCandidatesFakturList.Max(fak => fak.DokDate.EndOfDay());
      DateTime minDokDate = ZXC.projectYearFirstDay;
      DateTime maxDokDate = ZXC.projectYearLastDay ;

      List<VvMER_Response_Data_AllActions> vvMER_responseDataList = Vv_eRacun_HTTP.VvMER_WebService_QueryOutbox_TRN_List(minDokDate, maxDokDate);

      if(vvMER_responseDataList == null) return;

      // join na ElektronicId da dobijemo samo one responseData koji su relevantni za naše fakture u goodCandidatesFakturList 

      //var theList = from respData in vvMER_responseDataList
      //              join fak in goodCandidatesFakturList
      //              on respData.ElectronicId equals fak.F2_ElectronicID/*MER_ElectronicID*/
      //              select new { rowIdx = theUC.TheFakturList.IndexOf(fak), lastStatusCD = respData.StatusId, faktur = fak };
      var theTRN_NewsList = vvMER_responseDataList
          .Join(
              QueryOutbox_CandidatesFakturList,
              respData => respData.ElectronicId ?? 0L,
              fak => (long)fak.F2_ElectronicID/*MER_ElectronicID*/,
              (respData, fak) => new
              {
                 rowIdx = theUC.TheFakturList.IndexOf(fak),
                 lastStatusCD = respData.StatusId,
                 faktur = fak
              }
          )
          .Where(item => item.lastStatusCD.HasValue && item.lastStatusCD.Value != item.faktur.F2_StatusCD);

      foreach(var item in theTRN_NewsList)
      {
         F2_IRn_faktur_rec = item.faktur;

         // update Vv dataLayer 

         theUC.TheVvTabPage.TheVvForm.BeginEdit(F2_IRn_faktur_rec);

         F2_IRn_faktur_rec.F2_StatusCD = item.lastStatusCD.Value;

         bool rwtOK = true; F2_IRn_faktur_rec.VvDao.RWTREC(theUC.TheDbConnection, F2_IRn_faktur_rec, false, true, false);

         theUC.TheVvTabPage.TheVvForm.EndEdit(F2_IRn_faktur_rec);

         if(rwtOK)
         {
            theUC.PutDgvLineFields(item.rowIdx, F2_IRn_faktur_rec); // osvjezi prikaz 
            updatedStatusInfo = string.Format("{0} ({1}) Novi TRANSPORTNI status:      {2}      {3} {4}",
                                          F2_IRn_faktur_rec.TipBr,
                                          F2_IRn_faktur_rec.F2_ElectronicID/*MER_ElectronicID*/,
                                          Vv_eRacun_HTTP.MER_TransportStatuses[item.lastStatusCD.Value],
                                          F2_IRn_faktur_rec.DokDate.ToString(ZXC.VvDateFormat), F2_IRn_faktur_rec.KupdobName);

            updatedStatusInfoList.Add(updatedStatusInfo);

         } // if(rwtOK)

      } // foreach(var item in theNewsList) 

      #endregion Refresh TRN status

      #region Refresh DPS status

      QueryOutbox_CandidatesFakturList = theUC.TheFakturList.Where(fak => ShouldCheckRefreshed_TRN_Or_DPS_Status(fak, true)).ToList();

      // ovdje bi ako se ide na smislenu kronolosku granicu trebalo filtrirati po fak.F2_SentTS a ne po fak.DokDate !!! 
      // za sada, idemo cijela projektna godina                                                                         
    //minDokDate = goodCandidatesFakturList.Min(fak => fak.DokDate.Date      );
    //maxDokDate = goodCandidatesFakturList.Max(fak => fak.DokDate.EndOfDay());
      minDokDate = ZXC.projectYearFirstDay;
      maxDokDate = ZXC.projectYearLastDay ;

      vvMER_responseDataList = Vv_eRacun_HTTP.VvMER_WebService_QueryOutbox_DPS_List(minDokDate, maxDokDate);

      if(vvMER_responseDataList == null) return;

      // join na ElektronicId da dobijemo samo one responseData koji su relevantni za naše fakture u goodCandidatesFakturList 

      //var theList = from respData in vvMER_responseDataList
      //              join fak in goodCandidatesFakturList
      //              on respData.ElectronicId equals fak.F2_ElectronicID/*MER_ElectronicID*/
      //              select new { rowIdx = theUC.TheFakturList.IndexOf(fak), lastStatusCD = respData.StatusId, faktur = fak };
      var theDPS_NewsList = vvMER_responseDataList
          .Join(
              QueryOutbox_CandidatesFakturList,
              respData => respData.ElectronicId ?? 0L,
              fak => (long)fak.F2_ElectronicID/*MER_ElectronicID*/,
              (respData, fak) => new
              {
                 rowIdx = theUC.TheFakturList.IndexOf(fak),
                 lastStatusCD = respData.StatusId,
                 faktur = fak
              }
          )
          .Where(item => item.lastStatusCD.HasValue && item.lastStatusCD.Value != item.faktur.F2_StatusCD);

      foreach(var item in theDPS_NewsList)
      {
         F2_IRn_faktur_rec = item.faktur;

         // update Vv dataLayer 

         theUC.TheVvTabPage.TheVvForm.BeginEdit(F2_IRn_faktur_rec);

         F2_IRn_faktur_rec.F2_StatusCD = item.lastStatusCD.Value;

         bool rwtOK = true; F2_IRn_faktur_rec.VvDao.RWTREC(theUC.TheDbConnection, F2_IRn_faktur_rec, false, true, false);

         theUC.TheVvTabPage.TheVvForm.EndEdit(F2_IRn_faktur_rec);

         if(rwtOK)
         {
            theUC.PutDgvLineFields(item.rowIdx, F2_IRn_faktur_rec); // osvjezi prikaz 
            updatedStatusInfo = string.Format("{0} ({1}) Novi POSLOVNI status:      {2}      {3} {4}",
                                          F2_IRn_faktur_rec.TipBr,
                                          F2_IRn_faktur_rec.F2_ElectronicID/*MER_ElectronicID*/,
                                          Vv_eRacun_HTTP.MER_TransportStatuses[item.lastStatusCD.Value],
                                          F2_IRn_faktur_rec.DokDate.ToString(ZXC.VvDateFormat), F2_IRn_faktur_rec.KupdobName);

            updatedStatusInfoList.Add(updatedStatusInfo);

         } // if(rwtOK)

      } // foreach(var item in theNewsList) 

      #endregion Refresh TRN or DPS status

      #region Refresh FISK status

      for(int rIdx = 0; (ZXC.IsF2_2026_rules || Vv_eRacun_HTTP.DEMO) && rIdx < theUC.TheG.RowCount; ++rIdx)
      {
         F2_IRn_faktur_rec = theUC.TheFakturList[rIdx];

         if(F2_IRn_faktur_rec.F2_HasNoSense_Refresh_FISK_Status) continue; // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

         bool isTrue = WS_Get_FISK_or_REJECTED_or_MARKAsP_Status_ForElectronicID(F2_IRn_faktur_rec.F2_ElectronicID/*MER_ElectronicID*/, "0"); // "0" = Zahtjev za statusom fiskalizacije AS SENDER 

         if(isTrue == true && F2_IRn_faktur_rec.F2_IsFisk == false)
         {
            // update Vv dataLayer 

            theUC.TheVvTabPage.TheVvForm.BeginEdit(F2_IRn_faktur_rec);
            
            F2_IRn_faktur_rec.F2_IsFisk = isTrue;

            bool rwtOK = true; F2_IRn_faktur_rec.VvDao.RWTREC(theUC.TheDbConnection, F2_IRn_faktur_rec, false, true, false);

            theUC.TheVvTabPage.TheVvForm.EndEdit(F2_IRn_faktur_rec);

            if(rwtOK)
            { 
               theUC.PutDgvLineFields(rIdx, F2_IRn_faktur_rec); // osvjezi prikaz 

               updatedStatusInfo = string.Format("{0} ({1}) Novi isFISK status:      {2}      {3} {4}",
                                             F2_IRn_faktur_rec.TipBr,
                                             F2_IRn_faktur_rec.F2_ElectronicID/*MER_ElectronicID*/,
                                             "FISKALIZIRAN",
                                             F2_IRn_faktur_rec.DokDate.ToString(ZXC.VvDateFormat), F2_IRn_faktur_rec.KupdobName);

               updatedStatusInfoList.Add(updatedStatusInfo);

            } // if(rwtOK)

         }

      } // for(int rIdx = 0; rIdx < theUC.TheG.RowCount; ++rIdx)

      #endregion Refresh FISK status

      #region Refresh REJECTion status

      for(int rIdx = 0; (ZXC.IsF2_2026_rules || Vv_eRacun_HTTP.DEMO) && rIdx < theUC.TheG.RowCount; ++rIdx)
      {
         F2_IRn_faktur_rec = theUC.TheFakturList[rIdx];

         if(F2_IRn_faktur_rec.F2_HasNoSense_Refresh_REJECTion_Status) continue; // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

         bool isTrue = WS_Get_FISK_or_REJECTED_or_MARKAsP_Status_ForElectronicID(F2_IRn_faktur_rec.F2_ElectronicID/*MER_ElectronicID*/, "2"); // "2" = Zahtjev za statusom REJECTed AS SENDER 

         if(isTrue == true && F2_IRn_faktur_rec.F2_IsRejected == false)
         {
            // update Vv dataLayer 

            theUC.TheVvTabPage.TheVvForm.BeginEdit(F2_IRn_faktur_rec);
            
            F2_IRn_faktur_rec.F2_IsRejected = isTrue;

            bool rwtOK = true; F2_IRn_faktur_rec.VvDao.RWTREC(theUC.TheDbConnection, F2_IRn_faktur_rec, false, true, false);

            theUC.TheVvTabPage.TheVvForm.EndEdit(F2_IRn_faktur_rec);

            if(rwtOK)
            { 
               theUC.PutDgvLineFields(rIdx, F2_IRn_faktur_rec); // osvjezi prikaz 

               updatedStatusInfo = string.Format("{0} ({1}) Novi isREJECTED status:      {2}      {3} {4}",
                                             F2_IRn_faktur_rec.TipBr,
                                             F2_IRn_faktur_rec.F2_ElectronicID/*MER_ElectronicID*/,
                                             "ODBIJENO",
                                             F2_IRn_faktur_rec.DokDate.ToString(ZXC.VvDateFormat), F2_IRn_faktur_rec.KupdobName);

               updatedStatusInfoList.Add(updatedStatusInfo);

            } // if(rwtOK)

         }

      } // for(int rIdx = 0; rIdx < theUC.TheG.RowCount; ++rIdx)

      #endregion Refresh REJECTion status

      #region Refresh MarkAsPaid status

      for(int rIdx = 0; (ZXC.IsF2_2026_rules || Vv_eRacun_HTTP.DEMO) && rIdx < theUC.TheG.RowCount; ++rIdx)
      {
         F2_IRn_faktur_rec = theUC.TheFakturList[rIdx];

         if(F2_IRn_faktur_rec.F2_HasNoSense_Refresh_MarkAsPaid_Status) continue; // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

         bool isTrue = WS_Get_FISK_or_REJECTED_or_MARKAsP_Status_ForElectronicID(F2_IRn_faktur_rec.F2_ElectronicID/*MER_ElectronicID*/, "3"); // "3" = Zahtjev za statusom MARKAsPAID AS SENDER 

         if(isTrue == true && F2_IRn_faktur_rec.F2_IsMarkAsPaid == false)
         {
            // update Vv dataLayer 

            theUC.TheVvTabPage.TheVvForm.BeginEdit(F2_IRn_faktur_rec);
            
            F2_IRn_faktur_rec.F2_IsMarkAsPaid = isTrue;

            bool rwtOK = true; F2_IRn_faktur_rec.VvDao.RWTREC(theUC.TheDbConnection, F2_IRn_faktur_rec, false, true, false);

            theUC.TheVvTabPage.TheVvForm.EndEdit(F2_IRn_faktur_rec);

            if(rwtOK)
            { 
               theUC.PutDgvLineFields(rIdx, F2_IRn_faktur_rec); // osvjezi prikaz 

               updatedStatusInfo = string.Format("{0} ({1}) Novi isMARKasPAID status:      {2}      {3} {4}",
                                             F2_IRn_faktur_rec.TipBr,
                                             F2_IRn_faktur_rec.F2_ElectronicID/*MER_ElectronicID*/,
                                             "PLAĆENO",
                                             F2_IRn_faktur_rec.DokDate.ToString(ZXC.VvDateFormat), F2_IRn_faktur_rec.KupdobName);

               updatedStatusInfoList.Add(updatedStatusInfo);

            } // if(rwtOK)

         }

      } // for(int rIdx = 0; rIdx < theUC.TheG.RowCount; ++rIdx)

      #endregion Refresh MarkAsPaid status

      #region Refresh MarkAsPaid_InfoColumns

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

      } // for(int rIdx = 0; rIdx < theUC.TheG.RowCount; ++rIdx)

      #endregion Refresh MarkAsPaid_InfoColumns

      #region RECEIVE eRacun for Arhiva 

      for(int rIdx = 0; rIdx < theUC.TheG.RowCount; ++rIdx)
      {
         F2_IRn_faktur_rec = theUC.TheFakturList[rIdx];

         if(F2_IRn_faktur_rec.F2_HasNoSense_RECEIVE_document2arhiva) continue; // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

         uint arhivaXtrano_recID = WS_Get_RECEIVE_Izlaz_Document2Arhiva_ForElectronicID(theUC, F2_IRn_faktur_rec.F2_ElectronicID/*MER_ElectronicID*/, F2_IRn_faktur_rec);

         if(arhivaXtrano_recID.NotZero() && F2_IRn_faktur_rec.F2_ArhRecID.IsZero())
         {
            // update Faktur dataLayer 

            theUC.TheVvTabPage.TheVvForm.BeginEdit(F2_IRn_faktur_rec);
            
            F2_IRn_faktur_rec.F2_ArhRecID = arhivaXtrano_recID;

            bool rwtOK = true; F2_IRn_faktur_rec.VvDao.RWTREC(theUC.TheDbConnection, F2_IRn_faktur_rec, false, true, false);

            theUC.TheVvTabPage.TheVvForm.EndEdit(F2_IRn_faktur_rec);

            if(rwtOK)
            { 
               theUC.PutDgvLineFields(rIdx, F2_IRn_faktur_rec); // osvjezi prikaz 

               updatedStatusInfo = string.Format("{0} ({1}) Novi eRačun u arhivi:      {2}      {3} {4}",
                                             F2_IRn_faktur_rec.TipBr,
                                             F2_IRn_faktur_rec.F2_ElectronicID/*MER_ElectronicID*/,
                                             "ARHIVIRANO",
                                             F2_IRn_faktur_rec.DokDate.ToString(ZXC.VvDateFormat), F2_IRn_faktur_rec.KupdobName);

               updatedStatusInfoList.Add(updatedStatusInfo);

            } // if(rwtOK)

         }

      } // for(int rIdx = 0; rIdx < theUC.TheG.RowCount; ++rIdx)

      #endregion RECEIVE eRacun for Arhiva 

      #region Finish

      Cursor.Current = Cursors.Default;

      if(updatedStatusInfoList.NotEmpty())
      {
         ZXC.aim_emsg_List(string.Format("Dohvatio {0} novih statusa.", updatedStatusInfoList.Count), updatedStatusInfoList);
      }

      #endregion Finish
   }
   /* DDD */internal static void Discover_Candidates_And_Eventually_MAPaj_uplate(F2_Izlaz_UC theUC, XSqlConnection conn)
   {
      #region Init & Get Dialog Fields AND Create MAP_requestDataList 

      //if(ZXC.RRD.Dsc_F2_IsAutoMAP == false) return;

      List<(VvMER_Request_Data_AllActions request, Ftrans ftrans, Faktur faktur)> MAP_ActionsList = new List<(VvMER_Request_Data_AllActions request, Ftrans ftrans, Faktur faktur)>();

      Faktur MAP_CandidateFaktur_rec;

      List<VvReportSourceUtil> messageList = new List<VvReportSourceUtil>();

      List<Ftrans> paymentftransList;

      string thePaymentMethod = "T"; // TODO ??? !!!              
                                     // 5.1 PaymentMethod         
                                     // ”T” – Transakcijski racun 
                                     // ”O” – Obracunsko placanje 
                                     // ”Z” – Ostalo              

    //paymentftransList = FtransDao.Get_MAP_FtransList_For_FakRecID(conn, MAP_CandidateFaktur_rec.RecID);
      paymentftransList = FtransDao.Get_MAP_FtransList             (conn                               ); // ftrans 'MAP' kandidati: naplate od KUPACa koje nisu jos MAPane 
      
      VvMER_Request_Data_AllActions MAP_requestData;

      foreach(Ftrans paymentftrans_rec in paymentftransList)
      {
         MAP_CandidateFaktur_rec = new Faktur();

         if(paymentftrans_rec.T_fakRecID.NotZero()) // TODO !!! tu treba implementirati ZXC public static (int year, uint recID) GetYearAndRecIDFrom_YYandRecID(uint YYandRecID) 
                                                    // TODO je i prilagoditi Nalog_PS() da salje i T_fakRecID u tom formatu                                                      
         {
            MAP_CandidateFaktur_rec.VvDao.SetMe_Record_byRecID_Complete(conn, paymentftrans_rec.T_fakRecID, MAP_CandidateFaktur_rec);
         }
         else
         {
            MAP_CandidateFaktur_rec = null;
            // TODO: ako ispadne da je T_fakRecID prazan, ovdje treba potražiti fakturu preko T_tipBr-a 
         }

         if(MAP_CandidateFaktur_rec == null)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Ne mogu pronaći fakturu za MAP!? Ftrans: {0}", paymentftrans_rec);
            continue; 
         }

         if(MAP_CandidateFaktur_rec.Is_MAP_with_ElectronicID) // imamo Electronic id ... bilo je F2send 
         { 
            MAP_requestData = new VvMER_Request_Data_AllActions()
            {
               ElectronicId  = MAP_CandidateFaktur_rec.F2_ElectronicID,
               PaymentDate   = paymentftrans_rec.T_dokDate            ,
               PaymentAmount = paymentftrans_rec.T_pot                ,
               PaymentMethod = thePaymentMethod
            };
         }
         else if(MAP_CandidateFaktur_rec.Is_MAP_without_ElectronicID) // NEMAMO Electronic id ...bilo je F2eIzvj 
         { 
            MAP_requestData = new VvMER_Request_Data_AllActions()
            {
               InternalMark             = MAP_CandidateFaktur_rec.TtNumFiskal,
               IssueDate                = MAP_CandidateFaktur_rec.DokDate    ,
               SenderIdentifierValue    = ZXC.CURR_prjkt_rec.Oib             ,
               RecipientIdentifierValue = MAP_CandidateFaktur_rec.KdOib      ,

               PaymentDate              = paymentftrans_rec.T_dokDate        ,
               PaymentAmount            = paymentftrans_rec.T_pot            ,
               PaymentMethod            = thePaymentMethod
            };
         }
         else
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Faktura MAP kind je nedefiniran!?\n\r\n\rFtrans:\n\r\n\r{0}\n\r\n\rFaktur:\n\r\n\r{1}", 
               paymentftrans_rec, MAP_CandidateFaktur_rec);
            continue;
         }

         MAP_ActionsList.Add((MAP_requestData, paymentftrans_rec, MAP_CandidateFaktur_rec));

         messageList.Add(new VvReportSourceUtil()
         {
            IsNekakav  = MAP_CandidateFaktur_rec.Is_MAP_with_ElectronicID,

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
         });

      } // foreach(Ftrans paymentftrans_rec in paymentftransList) 

      VvMessageBoxDLG  MAP_CandidatesFtransList_InfoDLG = new VvMessageBoxDLG (false, ZXC.VvmBoxKind.F2_MAP_candidates);
      MAP_CandidatesFtransList_InfoDLG.Text = "Kandidati za slanje prijave plaćanja:";

      MAP_CandidatesFtransList_InfoDLG.TheUC.PutDgvFields_F2_MAP_candidates(messageList);

      DialogResult dlgResult = MAP_CandidatesFtransList_InfoDLG.ShowDialog();

      if(dlgResult != DialogResult.OK)
      {
         MAP_CandidatesFtransList_InfoDLG.Dispose();
         return;
      }

      ZXC.RRD.Dsc_F2_IsAutoMAP  = !MAP_CandidatesFtransList_InfoDLG.TheUC.Fld_StopAutoMAP; ZXC.RRD.SaveDscToLookUpItemList();
      int numOfFirstLinesOnly   =  MAP_CandidatesFtransList_InfoDLG.TheUC.Fld_NumOfFirstLinesOnly_MAP;

      MAP_CandidatesFtransList_InfoDLG.Dispose();

      Cursor.Current = Cursors.WaitCursor;

      int mapCount = 0; bool MAP_OK;

      System.Diagnostics.Stopwatch dispatchStopWatch = System.Diagnostics.Stopwatch.StartNew();

      uint soFarCount      = 0;
       int ofTotalCount    = numOfFirstLinesOnly.NotZero() ? numOfFirstLinesOnly : paymentftransList.Count;
      long elapsedTicks    = 0, remainTicks;
      decimal soFarKoef       ;
      TimeSpan elapsedTime = new TimeSpan(0);
      TimeSpan remainTime     ;
      string statusText       ;

      VvMER_Response_Data_AllActions responseData;

      Xtrano F2_MAP_Xtrano_rec;

      #endregion Init & Get Dialog Fields AND Create MAP_requestDataList 

      #region The MAP API Loop - foreach MAP_requestData

      foreach((VvMER_Request_Data_AllActions request, Ftrans ftrans, Faktur faktur) MAP_Action in MAP_ActionsList)
      {
         Cursor.Current = Cursors.WaitCursor;

         #region call the MAP API

         responseData = WS_Mark_Paid_With_OR_Without_ElectronicID(MAP_Action.request, MAP_Action.faktur.Is_MAP_with_ElectronicID);

         MAP_OK       = (responseData != null);

         if(MAP_OK)
         {
            mapCount++;

            F2_MAP_Xtrano_rec = VvMER_Response_Data_AllActions.F2_MAPtrans_SetXtranoFrom_Ftrans(MAP_Action.ftrans, MAP_Action.faktur, responseData);

            if(F2_MAP_Xtrano_rec != null)
            {
               byte[] T_XmlZip = F2_MAP_Xtrano_rec.T_XmlZip;

               bool OK = ZXC.XtranoDao.ADDREC(theUC.TheDbConnection, F2_MAP_Xtrano_rec, /*false*/true, false, false, false);

               if(OK)
               {
                  theUC.TheVvTabPage.TheVvForm.BeginEdit(F2_MAP_Xtrano_rec);

                  F2_MAP_Xtrano_rec.T_XmlZip = T_XmlZip;

                  VvDaoBase.Rwtrec_BLOBsingleColumn(theUC.TheDbConnection, F2_MAP_Xtrano_rec, "t_XmlZip", F2_MAP_Xtrano_rec.T_XmlZip);

                  theUC.TheVvTabPage.TheVvForm.EndEdit(F2_MAP_Xtrano_rec);
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

      #endregion Finish
   }
   
   
   
   internal static void WS_Load_URn_FakturList(F2_Ulaz_UC _theVvUC)
   {
      F2_Ulaz_UC theUC = _theVvUC as F2_Ulaz_UC;

      theUC.TheFakturList = new List<Faktur>();

      List<VvMER_Response_Data_AllActions> vvMER_Json_StatusList_Data = null;

      bool getStatusOK = true;
      try
      {
         vvMER_Json_StatusList_Data = Vv_eRacun_HTTP.VvMER_WebService_QueryInbox_List(DateTime.MinValue, DateTime.MaxValue);

         if(vvMER_Json_StatusList_Data.IsEmpty()) return;
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom slanja na WebServis: {0}", ex.Message);
         getStatusOK = false;
      }

      if(getStatusOK)
      {
         int counter = 0;
         Faktur F2UR_faktur_rec;

         foreach(VvMER_Response_Data_AllActions responseData in vvMER_Json_StatusList_Data)
         {
            if(counter++ > ZXC.RRD.Dsc_F2_NumOfRows) break;

            F2UR_faktur_rec = new Faktur()
            {
               TT                = "F2UR"                         ,
               KupdobName        = responseData.SenderBusinessName,
               VezniDok          = responseData.DocumentNr        ,
               FiskPrgBr         = responseData.ElectronicId.ToString(),
               //F2_ElectronicId = responseData.ElectronicId      ,
               //F2_SentTS       = responseData.Sent              ,
               //F2_StatusCD     = responseData.StatusId          ,
               TtNum = (uint)(responseData.StatusId)
            };

            theUC.TheFakturList.Add(F2UR_faktur_rec);

         } // foreach(VvMER_Response_Data_AllActions responseData in vvMER_Json_StatusList_Data)

      }

      if(theUC.TheFakturList.NotEmpty()) theUC.PutDgvFields();
   }

   private static bool ShouldCheckRefreshed_TRN_Or_DPS_Status(Faktur F2_IRn_faktur_rec, bool isDPS)
   {
      return !ShouldSkipRefreshing_TRN_Or_DPS_Status(F2_IRn_faktur_rec, isDPS);
   }
   private static bool ShouldSkipRefreshing_TRN_Or_DPS_Status(Faktur F2_IRn_faktur_rec, bool isDPS)
   {
      if(F2_IRn_faktur_rec.IsF2 == false) return true;

      if( isDPS && F2_IRn_faktur_rec.F2_Outbox_HasNoSense_Refresh_DPS_Status) return true; // DPS 
      if(!isDPS && F2_IRn_faktur_rec.F2_Outbox_HasNoSense_Refresh_TRN_Status) return true; // TRN 

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
      VvMER_Response_Data_AllActions responseData = null;
      bool getStatusOK = true;

      switch(ZXC.F2_TheProvider)
      {
         case ZXC.F2_Provider_enum.MER:
         {
            try
            {
               responseData = Vv_eRacun_HTTP.VvMER_WebService_Get_FISK_Status(electronicID, messageType);

               if(responseData == null) getStatusOK = false;
            }
            catch(Exception ex)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom slanja na WebServis: {0}", ex.Message);
               getStatusOK = false;
            }
            if(getStatusOK)
            {
               return (bool)responseData.IsSuccess;
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
      VvMER_Response_Data_AllActions responseData = null;
      
      bool receiveOK = true;

      uint arhivaXtrano_recID = 0;

      switch(ZXC.F2_TheProvider)
      {
         case ZXC.F2_Provider_enum.MER:
         {
            try
            {
               responseData = Vv_eRacun_HTTP.VvMER_WebService_Receive_XML(electronicID);
            }
            catch(Exception ex)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom slanja na WebServis: {0}", ex.Message);
               receiveOK = false;
            }

            if(receiveOK)
            {
               Xtrano F2arhivaXtrano_rec = VvMER_Response_Data_AllActions.F2_eRacun_Arhiva_SetXtranoFrom_XmlDocument(responseData.DocumentXml, Mixer.TT_AIR, F2_IRn_faktur_rec);

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
   private static VvMER_Response_Data_AllActions WS_Mark_Paid_With_OR_Without_ElectronicID(VvMER_Request_Data_AllActions MAP_requestData, bool isWithElectronicID)
   {
      VvMER_Response_Data_AllActions responseData = null;
      bool MAP_OK = true;

      switch(ZXC.F2_TheProvider)
      {
         case ZXC.F2_Provider_enum.MER:
         {
            try
            {
               if(isWithElectronicID) responseData = Vv_eRacun_HTTP.VvMER_WebService_MAP       (MAP_requestData);
               else                   responseData = Vv_eRacun_HTTP.VvMER_WebService_MAP_WO_eID(MAP_requestData);

               if(responseData == null) MAP_OK = false;
            }
            catch(Exception ex)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom slanja na WebServis: {0}", ex.Message);
               MAP_OK = false;
            }
            if(MAP_OK)
            {
               return       responseData          ;
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

   #endregion FIR / FUR Load List and SubmodulActions

}

#region Bussiness Classes for JSON Request/Response
public class MER_Credentials_Data
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
public class VvMER_Request_Data_AllActions : MER_Credentials_Data
{
   #region Constructors and Init
   private void InitMER_Credentials()
   {
      this.Username   = Vv_eRacun_HTTP.VvMER_UserName  ;
      this.Password   = Vv_eRacun_HTTP.VvMER_Password  ;
      this.CompanyId  = Vv_eRacun_HTTP.VvMER_CompanyId ;
      this.CompanyBu  = ""                                      ;
      this.SoftwareId = Vv_eRacun_HTTP.VvMER_SoftwareId;
   }

   private void InitPND_Credentials()
   {
      //this.softwareId = Vv_eRacun_HTTP.VvPND_SoftwareId;
        this.SoftwareId = Vv_eRacun_HTTP.VvPND_SoftwareId;
   }

   // za testiranje, pa sa test parametrima 
   public VvMER_Request_Data_AllActions(/*int username,*/ string password, string companyId, string companyBu, string softwareId, string xmlString)
   {
    //this.Username   = username  ;
      this.Username   = Vv_eRacun_HTTP.VvMER_UserName;
      this.Password   = password  ;
      this.CompanyId  = companyId ;
      this.CompanyBu  = companyBu ;
      this.SoftwareId = softwareId;
      this.File       =  xmlString;
   }

   public VvMER_Request_Data_AllActions(string xmlString) // za slanje jednog eRacuna 
   {
      switch(ZXC.F2_TheProvider)
      {
         case ZXC.F2_Provider_enum.MER: InitMER_Credentials(); this.File     = xmlString; break;
         case ZXC.F2_Provider_enum.PND: InitPND_Credentials(); this.document = xmlString; break;
      }
   }

   public VvMER_Request_Data_AllActions(uint electronicId) // za jedan racun 
   {
      switch(ZXC.F2_TheProvider)
      {
         case ZXC.F2_Provider_enum.MER: InitMER_Credentials(); break;
         case ZXC.F2_Provider_enum.PND: InitPND_Credentials(); break;
      }

      this.ElectronicId = electronicId;
   }

   public VvMER_Request_Data_AllActions(DateTime dateOD, DateTime dateDO) // za report 
   {
      switch(ZXC.F2_TheProvider)
      {
         case ZXC.F2_Provider_enum.MER: InitMER_Credentials(); break;
         case ZXC.F2_Provider_enum.PND: InitPND_Credentials(); break;
      }

      if(dateOD != DateTime.MinValue) this.From = dateOD;
      if(dateDO != DateTime.MaxValue) this.To   = dateDO;
   }

   public VvMER_Request_Data_AllActions()  
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
public class VvMER_Response_Data_AllActions : Vv_XSD_Bussiness_BASE<VvMER_Response_Data_AllActions>
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

   public static Xtrano F2_eRacun_Arhiva_SetXtranoFrom_XmlDocument(string xmlString, string F2_TT, Faktur faktur_rec = null)
   {
      if(F2_TT == Mixer.TT_AIR && faktur_rec == null) throw new Exception("F2_SetXtranoFrom_XmlDocument: faktur record is null!");

      byte[] zipped_xmlString = VvStringCompressor.CompressXml(xmlString);

      Xtrano xmlXtrano_rec = null;

      if(F2_TT == Mixer.TT_AUR) // ULAZNI racun 
      {
         xmlXtrano_rec = new Xtrano()
         {
            T_XmlZip   = zipped_xmlString          ,
                                                   
            T_TT       = F2_TT                     ,
                                                   
          //T_konto    = faktur_rec.TT             ,
          //T_parentID = faktur_rec.RecID          ,
          //T_dokDate  = faktur_rec.DokDate        ,
          //T_ttNum    = faktur_rec.TtNum          ,
            T_dokNum   = faktur_rec.F2_ElectronicID,
            T_serial   = 1                         ,
          //T_moneyA   = faktur_rec.S_ukKCRP       ,
            T_opis_128 = ""                        , // fuse 
          //T_devName  = ""                        , // fuse 
         };
      }

      if(F2_TT == Mixer.TT_AIR) // IZLAZNI racun 
      {
         xmlXtrano_rec = new Xtrano()
         {
            T_XmlZip   = zipped_xmlString          ,
                                                   
            T_TT       = F2_TT                     ,
                                                   
            T_konto    = faktur_rec.TT             ,
            T_parentID = faktur_rec.RecID          , 
            T_dokDate  = faktur_rec.DokDate        ,
            T_ttNum    = faktur_rec.TtNum          ,
            T_dokNum   = faktur_rec.F2_ElectronicID,
            T_serial   = 1                         ,
            T_moneyA   = faktur_rec.S_ukKCRP       ,
            T_opis_128 = ""                        , // fuse 
            T_devName  = ""                        , // fuse 
         };
      }

      return xmlXtrano_rec;
   }

   /// <summary>
   /// MAP Xtrano - evidencija prijave MAP na poreznu upravu
   /// </summary>
   /// <param name="ftrans_rec"></param>
   /// <param name="faktur_rec"></param>
   /// <returns></returns>
   public static Xtrano F2_MAPtrans_SetXtranoFrom_Ftrans(Ftrans MAPftrans_rec, Faktur MAPfaktur_rec, VvMER_Response_Data_AllActions responseData)
   {
      Xtrano MAPxtrano_rec = null;

      byte[] zipped_xmlString = VvStringCompressor.CompressXml(responseData.EncodedXml);

      MAPxtrano_rec = new Xtrano()
      {
         T_XmlZip   = zipped_xmlString                             ,
         T_dokDate  = (DateTime)responseData.FiscalizationTimestamp,
                                                
         T_TT       = Mixer.TT_MAP                                 ,
                                                                   
         T_konto    = MAPfaktur_rec.TT                             ,
         T_parentID = MAPftrans_rec.T_recID                        , // Ftrans LINK: t_parentID je ftrans recID --- > JOIN!     
         T_ttNum    = MAPfaktur_rec.RecID                          , // Faktur LINK: t_ttNum    je faktur recID ---> nije join! 
         T_dokNum   = MAPfaktur_rec.F2_ElectronicID                ,                                            
         T_serial   = 1                                            ,                                            
         T_moneyA   = MAPftrans_rec.T_pot                          , // Ftrans: t_moneyA je iznos UPLATE        
         T_opis_128 = MAPftrans_rec.T_tipBr                        , //                                         
         T_devName  = ""                                           , // fuse                                    
      };

      return MAPxtrano_rec;
   }

}

//public class DocumentInfo_Data
//{
//   [JsonPropertyName("ElectronicId")]
//   public long ElectronicId { get; set; }
//
//   [JsonPropertyName("DocumentNr")]
//   public string DocumentNr { get; set; }
//
//   [JsonPropertyName("DocumentTypeId")]
//   public int DocumentTypeId { get; set; }
//
//   [JsonPropertyName("DocumentTypeName")]
//   public string DocumentTypeName { get; set; }
//
//   [JsonPropertyName("StatusId")]
//   public int StatusId { get; set; }
//
//   [JsonPropertyName("StatusName")]
//   public string StatusName { get; set; }
//
//   [JsonPropertyName("SenderBusinessNumber")]
//   public string SenderBusinessNumber { get; set; }
//
//   [JsonPropertyName("SenderBusinessUnit")]
//   public string SenderBusinessUnit { get; set; }
//
//   [JsonPropertyName("SenderBusinessName")]
//   public string SenderBusinessName { get; set; }
//
//   [JsonPropertyName("RecipientBusinessNumber")]
//   public string RecipientBusinessNumber { get; set; }
//
//   [JsonPropertyName("RecipientBusinessUnit")]
//   public string RecipientBusinessUnit { get; set; }
//
//   [JsonPropertyName("RecipientBusinessName")]
//   public string RecipientBusinessName { get; set; }
//
//   [JsonPropertyName("Created")]
//   public DateTime Created { get; set; }
//
//   [JsonPropertyName("Updated")]
//   public DateTime? Updated { get; set; }
//
//   [JsonPropertyName("Sent")]
//   public DateTime? Sent { get; set; }
//
//   [JsonPropertyName("Delivered")]
//   public DateTime? Delivered { get; set; }
//
//   [JsonPropertyName("Issued")]
//   public DateTime? Issued { get; set; }
//
//   [JsonPropertyName("Imported")]
//   public bool? Imported { get; set; }
//}

public class WebApiResult<T>
{
   public T ResponseData            { get; set; }
   public string ResponseJson       { get; set; }
   public int? StatusCode           { get; set; }
   public string StatusDescription  { get; set; }
   public string ErrorBody          { get; set; }
   public string ExceptionMessage   { get; set; }
}

#endregion Bussiness Classes for JSON Request/Response

// VvForm Submodul Actions about F2 fiscalization 
public /*sealed*/ partial class VvForm : Crownwood.DotNetMagic.Forms.DotNetMagicForm
{
   private void F2_ReceiveSingle(object sender, EventArgs e)
   {
      VvMER_Response_Data_AllActions responseData = null;

      bool receiveOK = true;
      try
      {
         responseData = Vv_eRacun_HTTP.VvMER_WebService_Receive_XML(119499736);
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom slanja na WebServis: {0}", ex.Message);
         receiveOK = false;
      }

      if(receiveOK)
      {
         Xtrano F2arhivaXtrano_rec = VvMER_Response_Data_AllActions.F2_eRacun_Arhiva_SetXtranoFrom_XmlDocument(responseData.DocumentXml, Mixer.TT_AUR);

         if(F2arhivaXtrano_rec != null)
         {
            bool OK = ZXC.XtranoDao.ADDREC(TheDbConnection, F2arhivaXtrano_rec, false, false, false, false);





            Xtrano check_rec = new Xtrano();
            F2arhivaXtrano_rec.VvDao.SetMe_Record_byRecID(TheDbConnection, check_rec, 1, false);
            string decompXml = VvStringCompressor.DecompressXml(check_rec.T_XmlZip);
         }
      }
   }
   private void F2_RISK_Rules(object sender, EventArgs e)
   {
      F2_Rules_Dlg dlg = new F2_Rules_Dlg();

      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      dlg.TheUC.GetDscFields();

      dlg.Dispose();
   }
   private void F2_RefreshFIR_FakturListAndStatuses(object sender, EventArgs e) 
   { 
      /*AAA*/Vv_eRacun_HTTP.Load_IRn_FakturList                     ((F2_Izlaz_UC)TheVvUC); 
      /*CCC*/Vv_eRacun_HTTP.WS_Refresh_ALL_FIR_Statuses_AndArhiviraj((F2_Izlaz_UC)TheVvUC);
   }
   private void F2_Send_eRacune (object sender, EventArgs e) { /*BBB*/Vv_eRacun_HTTP.WS_Discover_Candidates_And_Eventually_SEND_eRacune((F2_Izlaz_UC)TheVvUC, TheDbConnection); }
   private void F2_MAPaj        (object sender, EventArgs e) { /*DDD*/Vv_eRacun_HTTP.Discover_Candidates_And_Eventually_MAPaj_uplate   ((F2_Izlaz_UC)TheVvUC, TheDbConnection); }
   
   private void F2_Send_eRacun_1(object sender, EventArgs e) {/*BBB Vv_eRacun_HTTP.WS_Discover_Candidates_And_Eventually_SEND_eRacune((F2_Izlaz_UC)TheVvUC, TheDbConnection);*/ }
   private void F2_MAPaj_1      (object sender, EventArgs e) {/*DDD Vv_eRacun_HTTP.Discover_Candidates_And_Eventually_MAPaj_uplate   ((F2_Izlaz_UC)TheVvUC, TheDbConnection);*/ }
      
   private void F2_ArhivaPdf (object sender, EventArgs e) 
   {
      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;
     
      Faktur faktur_rec   = theDUC.faktur_rec;

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

      List<(string Filename, byte[] PdfBytes)> pdfFiles = xtrano_rec.F2_GetPdfFilesWithNames();

      if(pdfFiles.Count.IsZero()) { ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Stop       , "Nema spremljenih PDF-ova za ovaj eRačun."                         );   return;   }
      if(pdfFiles.Count > 1)      { ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning    , "Ima više od jednog PDF-a. Prikazujem prvi od {0}.", pdfFiles.Count); /*return;*/ }

      (string filename, byte[] pdfBytes) thePDF = pdfFiles[0];

      string dirame   = VvPref.eRacun_Izlaz_Prefs.DirectoryName;
      string fullName = Path.Combine(dirame, thePDF.filename);

      File.WriteAllBytes(fullName, thePDF.pdfBytes);

      System.Diagnostics.Process.Start(fullName);

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
   private void F2_Outgoing_eRacun_QuickSend(object sender, EventArgs e)
   {
      Cursor.Current = Cursors.WaitCursor;

      Outgoing_eRacun_parameters oeRp = Set_Outgoing_eRacun_parameters((TheVvDocumentRecordUC as FakturDUC).faktur_rec, TheVvUC, true, true);

      bool sendOK = RISK_Outgoing_eRacun_JOB(oeRp, true);

      Cursor.Current = Cursors.Default;
   }
   internal Outgoing_eRacun_parameters Set_Outgoing_eRacun_parameters(Faktur faktur_rec, VvUserControl theVvUC, bool isQuickSend, bool _isOneOnlyFromFakturDUC)
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

      string deaultVvPDFdirectoryName = VvForm.GetLocalDirectoryForVvFile(VvPref.VvMailData.DeaultVvPDFdirectoryName);
      string todayDir                 = theTT + "_PDF_" + DateTime.Today.ToString(ZXC.VvDateYyyyMmDdMySQLFormat);
      string fileNameOnly ;
      string PDFfileFullPathName ;

      VvForm.CreateDirectoryInMyDocuments(Path.Combine(deaultVvPDFdirectoryName, todayDir));


      // 1. GetReportDocument
      theRptR_IRA = VvRiskReport.GetRptR_IRA(faktur_rec, thePFD, theTT);

      // 2. get fileName 
      fileNameOnly = faktur_rec.TT_And_TtNum + " [" + faktur_rec.KupdobName + "]" + ".pdf";

      PDFfileFullPathName = Path.Combine(deaultVvPDFdirectoryName, todayDir, fileNameOnly);

      // 3. set reportDocument.ExportOptions
      try
      {
         CrDiskFileDestinationOptions.DiskFileName = PDFfileFullPathName;
         CrExportOptions                           = theRptR_IRA.reportDocument.ExportOptions;

         CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
         CrExportOptions.ExportFormatType      = ExportFormatType.PortableDocFormat;
         CrExportOptions.DestinationOptions    = CrDiskFileDestinationOptions;
         CrExportOptions.FormatOptions         = CrFormatTypeOptions;

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

      /* oeRp_1. */ oeRp.faktur_rec              = faktur_rec                                      ;
      /* oeRp_2. */ oeRp.kupdob_rec              = kupdob_rec                                      ;
      /* oeRp_3. */ oeRp.primPlat_rec            = primPlat_rec                                    ;
      /* oeRp_4. */ oeRp.thePFD                  = thePFD                                          ;
      /* oeRp_5. */ oeRp.PDF_as_base64_byteArray = System.IO.File.ReadAllBytes(PDFfileFullPathName);
      /* oeRp_6. */ oeRp.pdfFileNameOnly         = PDFfileFullPathName                             ;
      /* oeRp_7. */ oeRp.fullPath_XML_FileName   = oeRp.suggestedXmlFileName + ".xml"              ;

      return oeRp;
   }

}