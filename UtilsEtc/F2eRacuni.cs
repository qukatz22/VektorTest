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
public static class Vv_Http_Web_request_QAI
{

   private const bool DEMO = /*true*/false;

   #region MER Web Service URLs - API endpoints web addresses

   public const string VvMER_baseAddress_production = @"https://www.moj-eracun.hr"     ; 
   public const string VvMER_baseAddress_demo       = @"https://www-demo.moj-eracun.hr"; 

   public const string VvMER_webAddressPOST_Send        = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/apis/v2/send"       ; // POST 
   public const string VvMER_webAddressPOST_Receive     = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/apis/v2/receive"    ; // POST 
   public const string VvMER_webAddressPOST_QueryOutbox = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/apis/v2/queryOutbox"; // POST 
   public const string VvMER_webAddressPOST_QueryInbox  = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/apis/v2/queryInbox" ; // POST 
   public const string VvMER_webAddressGET_Ping         = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/apis/v2/Ping"       ; // GET! 
   public const string VvMER_webAddressPOST_Check       = (DEMO ? VvMER_baseAddress_demo : VvMER_baseAddress_production) + "/api/mps/check"      ; // POST 

   public const string VvPND_baseAddress_production = @"https://eracun.eposlovanje.hr"; 
   public const string VvPND_baseAddress_demo       = @"https://test.eposlovanje.hr"; 

   public const string VvPND_webAddressPOST_Send        = (DEMO ? VvPND_baseAddress_demo : VvPND_baseAddress_production) + "/api/v2/document/send"    ; // POST 
   public const string VvPND_webAddressPOST_outgoing    = (DEMO ? VvPND_baseAddress_demo : VvPND_baseAddress_production) + "/api/v2/document/outgoing"; // GET! 
   public const string VvPND_webAddressGET_Ping         = (DEMO ? VvPND_baseAddress_demo : VvPND_baseAddress_production) + "/api/v2/ping"             ; // GET! 

   // MER authorisation parameters: 
   public static string VvMER_UserName   = DEMO ? "8633"    : ZXC.ValOrZero_Int(ZXC.CURR_prjkt_rec.SkyVvDomena).ToString();
   public static string VvMER_Password   = DEMO ? "T22zsEY" : ZXC.CURR_prjkt_rec.SkyPasswordDecrypted                     ;
   public static string VvMER_CompanyId  =                    ZXC.CURR_prjkt_rec.Oib                                      ;
   public const  string VvMER_SoftwareId =                    "Vektor-001"                                                ;

   // PND authorisation parameters: 
   public static string VvPND_API_Key    = "1042a7915a7f66a23a8e0e98d93cb44c6d968263638a8cec54e07cb5abc2ae2f";
   public static string VvPND_CompanyId  = ZXC.CURR_prjkt_rec.Oib;
   public const  string VvPND_SoftwareId = "Vektor-001"          ;

   public static readonly Dictionary<string, string> MER_TransportStatuses = new Dictionary<string, string>
   {
      { "20", "U obradi"             },
      { "30", "Poslan"               },
      { "40", "Preuzet"              },
      { "45", "Povućeno preuzimanje" },
      { "50", "Neuspjelo"            }
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
         httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
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

            //ZXC.aim_emsg(MessageBoxIcon.Error, "{0}\n\r\n\r{1}", resp.StatusDescription, body.Replace("{", "").Replace("}", ""));

            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error,
                "HTTP {0} {1}\r\nBody: {2}",
                (int)resp.StatusCode,
                resp.StatusDescription,
                body);

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

            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error,
                "HTTP {0} {1}\r\nBody: {2}",
                (int)resp.StatusCode,
                resp.StatusDescription,
                body);

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

   private static T Vv_POSTmethod_ExecuteJson<T>(string webAddress, string jsonRequestString, /*Action<T, string> saveToFile = null, string fileName = null,*/ string token = null)
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

                  // Optionally save to file if provided
                  //saveToFile?.Invoke(deserializedResponseData, fileName);
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
                //ZXC.aim_emsg_List("Response Messages from JsonTextReader", errorMsg);
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
      catch(Exception ex)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška pri Vv_POSTmethod_ExecuteJson: {0}", ex.Message);
      }

      return deserializedResponseData;
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

   //######################## https://www.moj-eracun.hr/apis/v2/send #########################################################################################################

   public static VvMER_Response_Data_AllActions VvMER_WebService_SEND(string xmlString, string fullPath_XML_FileName)
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

      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(xmlString);

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      VvMER_Response_Data_AllActions responseData_AllActions = 
         
         Vv_POSTmethod_ExecuteJson<VvMER_Response_Data_AllActions>
         (
            VvMER_webAddressPOST_Send,
            jsonRequestString//,
            //(data, fileName) => { if(fileName.NotEmpty()) data.SaveToFile(fileName); },
            //fullPath_XML_FileName.Replace(".xml", "_RES.xml")
         );

      return responseData_AllActions;
   }

   public  static VvMER_Response_Data_AllActions VvPND_WebService_SEND(string xmlString, string fullPath_XML_FileName)
   {
      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(xmlString);

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      VvMER_Response_Data_AllActions responseData_AllActions = 
         
         Vv_POSTmethod_ExecuteJson<VvMER_Response_Data_AllActions>
         (
            VvPND_webAddressPOST_Send,
            jsonRequestString,
            //(data, fileName) => { if(fileName.NotEmpty()) data.SaveToFile(fileName); },
            //fullPath_XML_FileName.Replace(".xml", "_RES.xml"),
            VvPND_API_Key
         );

      return responseData_AllActions;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/queryOutbox - one single status ##############################################################################

   public  static VvMER_Response_Data_AllActions VvMER_WebService_QueryOutbox_Single(int electronicID)
   {
      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(electronicID); // constructor za STATUS jednog racuna (electronicID-a) 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      List<VvMER_Response_Data_AllActions> responseData_AllActions_List = Vv_POSTmethod_ExecuteJson<List<VvMER_Response_Data_AllActions>>(VvMER_webAddressPOST_QueryOutbox, jsonRequestString);

      VvMER_Response_Data_AllActions responseData_AllActions = responseData_AllActions_List.FirstOrDefault();

      return responseData_AllActions;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/queryOutbox - Status List ####################################################################################

   public  static List<VvMER_Response_Data_AllActions> VvMER_WebService_QueryOutbox_List(DateTime dateOD, DateTime dateDO)
   {
      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(dateOD, dateDO); // constructor za Listu STATUSa racuna (dateOD, dateDO) 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      List<VvMER_Response_Data_AllActions> responseData_AllActions_List = Vv_POSTmethod_ExecuteJson<List<VvMER_Response_Data_AllActions>>(VvMER_webAddressPOST_QueryOutbox, jsonRequestString);

      return responseData_AllActions_List;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/queryInbox - Status List ####################################################################################

   public  static List<VvMER_Response_Data_AllActions> VvMER_WebService_QueryInbox_List(DateTime dateOD, DateTime dateDO)
   {
      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(dateOD, dateDO); // constructor za Listu STATUSa racuna (dateOD, dateDO) 

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      List<VvMER_Response_Data_AllActions> responseData_AllActions_List = Vv_POSTmethod_ExecuteJson<List<VvMER_Response_Data_AllActions>>(VvMER_webAddressPOST_QueryInbox, jsonRequestString);

      return responseData_AllActions_List;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/receive - one single document ##############################################################################

   public static VvMER_Response_Data_AllActions VvMER_WebService_Receive_XML(int electronicID)
   {
      VvMER_Response_Data_AllActions responseData = new VvMER_Response_Data_AllActions();

      try
      {
         VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions(electronicID); // constructor za RECEIVE jednog racuna (electronicID-a) 

         string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

         HttpWebResponse httpResponse = Vv_POSTmethod_SendHttpWebRequest_GetHttpWebResponse(VvMER_webAddressPOST_Receive, jsonRequestString);

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

   //######################## https://www.moj-eracun.hr/apis/v2/Ping - Checks if service is up ##############################################################################

   public static VvMER_Response_Data_AllActions VvMER_WebService_Ping()
   {
      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions();
   
      VvMER_Response_Data_AllActions responseData_AllActions = Vv_GETmethod_ExecuteJson<VvMER_Response_Data_AllActions, VvMER_Request_Data_AllActions>(VvMER_webAddressGET_Ping, request_Data_AllActions);
   
      return responseData_AllActions;
   }

   public static VvMER_Response_Data_AllActions VvPND_WebService_Ping()
   {
      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions();

      string authToken = VvPND_API_Key;

      VvMER_Response_Data_AllActions responseData_AllActions =
          Vv_GETmethod_ExecuteJson<VvMER_Response_Data_AllActions, VvMER_Request_Data_AllActions>
          (
              VvPND_webAddressGET_Ping,
              request_Data_AllActions,
              null,
              null,
              authToken  // Using just the API key
          );

      return responseData_AllActions;
   }

   //######################## https://www.moj-eracun.hr/api/mps/check - Check Identifier ##############################################################################

   public static VvMER_Response_Data_AllActions VvMER_WebService_CheckAMS(string _Identifiervalue)
   {
      VvMER_Request_Data_AllActions request_Data_AllActions = new VvMER_Request_Data_AllActions() 
      { 
         IdentifierValue = _Identifiervalue,
         IdentifierType  = 0,
      };

      string jsonRequestString = VvMER_Json_SerializeObjectForRequestString_AllActions(request_Data_AllActions);

      VvMER_Response_Data_AllActions responseData_AllActions = Vv_POSTmethod_ExecuteJson<VvMER_Response_Data_AllActions>(VvMER_webAddressPOST_Check, jsonRequestString);
   
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





   #region F2IR / F2UR Load List and SubmodulActions
   internal static void F2_Load_IRn_FakturList_And_PutDgvFields(F2_Izlaz_UC _theVvUC)
   {
      F2_Izlaz_UC theUC = _theVvUC as F2_Izlaz_UC;

      theUC.TheFakturList = new List<Faktur>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>();

      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt], "theTT", ZXC.RRD.Dsc_F2_TT, " = "));

      string asdDscStr;
      string limitStr = "LIMIT " + (ZXC.RRD.Dsc_F2_NumOfRows.IsPositive() ? ZXC.RRD.Dsc_F2_NumOfRows.ToString() : "100");

      if(ZXC.RRD.Dsc_F2_IsAsc == false) asdDscStr = " DESC ";
      else                              asdDscStr = " ASC " ;

      VvDaoBase.LoadGenericVvDataRecordList<Faktur>(_theVvUC.TheDbConnection, theUC.TheFakturList, filterMembers, "", "ttNum " + asdDscStr + limitStr, true);

      if(theUC.TheFakturList.NotEmpty()) theUC.PutDgvFields();

      //kuracMethod();
   }

   internal static void F2_Load_URn_FakturList_And_PutDgvFields(F2_Ulaz_UC _theVvUC)
   {
      F2_Ulaz_UC theUC = _theVvUC as F2_Ulaz_UC;

      theUC.TheFakturList = new List<Faktur>();

      List<VvMER_Response_Data_AllActions> vvMER_Json_StatusList_Data = null;

      bool getStatusOK = true;
      try
      {
         vvMER_Json_StatusList_Data = Vv_Http_Web_request_QAI.VvMER_WebService_QueryInbox_List(DateTime.MinValue, DateTime.MaxValue);
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
               TT          = "F2UR"                         ,
               KupdobName  = responseData.SenderBusinessName,
               VezniDok    = responseData.DocumentNr        ,
               FiskPrgBr   = responseData.ElectronicId.ToString(),
             //F2_DocumID  = responseData.ElectronicId      ,
             //F2_SentTS   = responseData.Sent              ,
             //F2_StatusCD = responseData.StatusId          ,
               TtNum       = (uint)(responseData.StatusId)
            };

            theUC.TheFakturList.Add(F2UR_faktur_rec);

         } // foreach(VvMER_Response_Data_AllActions responseData in vvMER_Json_StatusList_Data)

      }

      if(theUC.TheFakturList.NotEmpty()) theUC.PutDgvFields();
   }

   #endregion F2IR / F2UR Load List and SubmodulActions

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
      this.Username   = Vv_Http_Web_request_QAI.VvMER_UserName  ;
      this.Password   = Vv_Http_Web_request_QAI.VvMER_Password  ;
      this.CompanyId  = Vv_Http_Web_request_QAI.VvMER_CompanyId ;
      this.CompanyBu  = ""                                      ;
      this.SoftwareId = Vv_Http_Web_request_QAI.VvMER_SoftwareId;
   }

   private void InitPND_Credentials()
   {
      //this.softwareId = Vv_Http_Web_request_QAI.VvPND_SoftwareId;
        this.SoftwareId = Vv_Http_Web_request_QAI.VvPND_SoftwareId;
   }

   // za testiranje, pa sa test parametrima 
   public VvMER_Request_Data_AllActions(/*int username,*/ string password, string companyId, string companyBu, string softwareId, string xmlString)
   {
    //this.Username   = username  ;
      this.Username   = Vv_Http_Web_request_QAI.VvMER_UserName;
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

   public VvMER_Request_Data_AllActions(int electronicId) // za jedan racun 
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

   [JsonPropertyName("Filter")]
   public string Filter { get; set; }

   [JsonPropertyName("ActionType")]
   public string ActionType { get; set; }

   [JsonPropertyName("RejectReason")]
   public string RejectReason { get; set; }

   // Payment related
   [JsonPropertyName("PaymentDate")]
   public DateTime? PaymentDate { get; set; }

   [JsonPropertyName("PaymentAmount")]
   public decimal? PaymentAmount { get; set; }

   [JsonPropertyName("PaymentMethod")]
   public string PaymentMethod { get; set; }

   // Query date range
   [JsonPropertyName("From")]
   public DateTime? From { get; set; }

   [JsonPropertyName("To")]
   public DateTime? To { get; set; }

   // Registration and identification
   [JsonPropertyName("CompanyNumber")]
   public string CompanyNumber { get; set; }

   [JsonPropertyName("IdentifierType")]
   public int? IdentifierType { get; set; }

   [JsonPropertyName("IdentifierValue")]
   public string IdentifierValue { get; set; }

   // Classification
   [JsonPropertyName("KPDCode")]
   public string KPDCode { get; set; }

   // PND specific
   [JsonPropertyName("document")]
   public string document { get; set; }


}
public class VvMER_Response_Data_AllActions : Vv_XSD_Bussiness_BASE<VvMER_Response_Data_AllActions>
{
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

   // PND fields
   [JsonPropertyName("id")]
   public long Id { get; set; } // If the API returns a GUID/string, change to string.

   [JsonPropertyName("insertedOn")]
   public DateTime? InsertedOn { get; set; }

   public static Xtrano F2_SetXtranoFrom_XmlDocument(string xmlString, string F2_TT, Faktur faktur_rec = null)
   {
      byte[] zipped_xmlString = VvStringCompressor.CompressXml(xmlString);

      //string JSON4xtrano = ZXC.LenLimitedStr(zipped_xmlString, ZXC.XtranoDao.GetSchemaColumnSize(ZXC.XtoCI.t_opis_128));

      Xtrano xmlXtrano_rec = new Xtrano()
      {
         //T_opis_128 = JSON4xtrano        ,
         T_XmpZip = zipped_xmlString,

         T_TT = F2_TT,
         //T_parentID = faktur_rec.RecID   , // NE! Nemas jos faktur_rec.RecID u ovom trenutku 
         //T_dokDate  = faktur_rec.DokDate ,
         //T_ttNum    = faktur_rec.TtNum   ,
         //T_dokNum   = faktur_rec.DokNum  ,
         //T_serial   = 1                  ,
         //T_moneyA   = faktur_rec.S_ukKCRP,
         //T_konto    = ""                 , // fuse 
         //T_devName  = faktur_rec.DevName  
      };

      return xmlXtrano_rec;
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

#endregion Bussiness Classes for JSON Request/Response

public /*sealed*/ partial class VvForm : Crownwood.DotNetMagic.Forms.DotNetMagicForm
{
   private void F2_ReceiveSingle(object sender, EventArgs e)
   {
      VvMER_Response_Data_AllActions responseData = null;

      bool receiveOK = true;
      try
      {
         responseData = Vv_Http_Web_request_QAI.VvMER_WebService_Receive_XML(119499736);
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom slanja na WebServis: {0}", ex.Message);
         receiveOK = false;
      }

      if(receiveOK)
      {
         Xtrano F2arhivaXtrano_rec = VvMER_Response_Data_AllActions.F2_SetXtranoFrom_XmlDocument(responseData.DocumentXml, Mixer.TT_AUR);

         if(F2arhivaXtrano_rec != null)
         {
            bool OK = ZXC.XtranoDao.ADDREC(TheDbConnection, F2arhivaXtrano_rec, false, false, false, false);





            Xtrano check_rec = new Xtrano();
            F2arhivaXtrano_rec.VvDao.SetMe_Record_byRecID(TheDbConnection, check_rec, 1, false);
            string decompXml = VvStringCompressor.DecompressXml(check_rec.T_XmpZip);
         }
      }
   }

   private void RISK_F2_Rules(object sender, EventArgs e)
   {
      F2_Rules_Dlg dlg = new F2_Rules_Dlg();

      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      dlg.TheUC.GetDscFields();

      dlg.Dispose();
   }

   private void F2_RefreshOutbox(object sender, EventArgs e)
   {
      F2_Izlaz_UC theUC = TheVvUC as F2_Izlaz_UC;
      Faktur F2_IRn_faktur_rec;

      for(int rIdx = 0; rIdx < theUC.TheG.RowCount; ++rIdx)
      {
         F2_IRn_faktur_rec = theUC.TheFakturList[rIdx];

         if(F2_IRn_faktur_rec.IsF2 == false) continue;

         if(F2_IRn_faktur_rec.F2_IsNoSense_RefreshTransportStatus) continue;

         // tu smo stali 
         // za svaki F2IR racun, uzmi status sa web servisa i azuriraj u bazi

      } // for(int rIdx = 0; rIdx < theUC.TheG.RowCount; ++rIdx)
   }

}