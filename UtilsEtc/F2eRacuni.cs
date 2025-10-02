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
public static class Vv_Http_Web_request_QAI
{
   #region Private common methods
   private static HttpWebResponse Vv_SendHttpWebRequest_GetHttpWebResponse(string webAddress, string jsonRequestString)
   {
      HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddress);
      httpWebRequest.ContentType = "application/json; charset=utf-8";
      httpWebRequest.Method = "POST";

      using(StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
      {
         streamWriter.Write(jsonRequestString);
         streamWriter.Flush();
      }

      return (HttpWebResponse)httpWebRequest.GetResponse();
   }

   private static T VvMER_PostJson<T>(string webAddress, string jsonRequestString, Action<T, string> saveToFile = null, string fileName = null)
      where T : class, new()
   {
      T deserializedResponseData = null;

      try
      {
         HttpWebResponse httpResponse = Vv_SendHttpWebRequest_GetHttpWebResponse(webAddress, jsonRequestString);

         using(StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
         {
            string responseJson = streamReader.ReadToEnd();

            if(responseJson.NotEmpty())
            {
               try
               {
                  deserializedResponseData = JsonConvert.DeserializeObject<T>(responseJson);

                  // Optionally save to file if provided
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

      return deserializedResponseData;
   }

   private static JsonSerializerSettings VvMER_JsonSerializerSettings_Default()
   {
      return new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };
   }

   private static string VvMER_JsonRequestString_AllActions(VvMER_Request_Data_AllActions json_AllActions_Request_Data)
   {
      return JsonConvert.SerializeObject(json_AllActions_Request_Data, Newtonsoft.Json.Formatting.Indented, VvMER_JsonSerializerSettings_Default());
   }

   public static string VvMER_UserName   = ZXC.ValOrZero_Int(ZXC.CURR_prjkt_rec.SkyVvDomena).ToString();
   public static string VvMER_Password   = ZXC.CURR_prjkt_rec.SkyPasswordDecrypted                     ;
   public static string VvMER_CompanyId  = ZXC.CURR_prjkt_rec.Oib                                      ;
   public const  string VvMER_SoftwareId = "Vektor-001"                                                ;

   public const string VvMER_webAddress_Send        = @"https://www.moj-eracun.hr/apis/v2/send"       ;
   public const string VvMER_webAddress_QueryOutbox = @"https://www.moj-eracun.hr/apis/v2/queryOutbox";

   #endregion Private common methods

   //######################## https://www.moj-eracun.hr/apis/v2/send #########################################################################################################

   public  static VvMER_Response_Data_SEND VvMER_WebService_SEND(string xmlString, string fullPath_XML_FileName)
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

      VvMER_Request_Data_AllActions json_SEND_Request_Data = new VvMER_Request_Data_AllActions(xmlString);

      string jsonRequestString = JsonConvert.SerializeObject(json_SEND_Request_Data, Newtonsoft.Json.Formatting.Indented/*, VvMER_JsonSerializerSettings_Default()*/);

    //VvMER_Response_Data_SEND deserializedResponseData = VvMER_PostJson_SEND(VvMER_webAddress_Send, jsonRequestString, fullPath_XML_FileName);
      VvMER_Response_Data_SEND deserializedResponseData = 
         
         VvMER_PostJson<VvMER_Response_Data_SEND>
         (
            VvMER_webAddress_Send,
            jsonRequestString,
            (data, fileName) => { if(fileName.NotEmpty()) data.SaveToFile(fileName); },
            fullPath_XML_FileName.Replace(".xml", "_RES.xml")
         );

      return deserializedResponseData;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/queryOutbox - one single status ##############################################################################

   public  static VvMER_Response_Data_Status VvMER_WebService_OneSingleStatus(int electronicID)
   {
      VvMER_Request_Data_AllActions json_Request_Data = new VvMER_Request_Data_AllActions(electronicID); // constructor za STATUS jednog racuna (electronicID-a) 

      List<VvMER_Response_Data_Status> statusList = VvMER_PostJson<List<VvMER_Response_Data_Status>>(VvMER_webAddress_QueryOutbox, VvMER_JsonRequestString_AllActions(json_Request_Data));

      VvMER_Response_Data_Status status = statusList.FirstOrDefault();

      return status;
   }

   //######################## https://www.moj-eracun.hr/apis/v2/queryOutbox - Status List ####################################################################################

   public  static List<VvMER_Response_Data_Status> VvMER_WebService_StatusList(DateTime dateOD, DateTime dateDO)
   {
      VvMER_Request_Data_AllActions json_Request_Data = new VvMER_Request_Data_AllActions(dateOD, dateDO); // constructor za Listu STATUSa racuna (dateOD, dateDO) 

      string jsonRequestString = VvMER_JsonRequestString_AllActions(json_Request_Data);

      List<VvMER_Response_Data_Status> statusList = VvMER_PostJson<List<VvMER_Response_Data_Status>>(VvMER_webAddress_QueryOutbox, jsonRequestString);

      return statusList;
   }

   // ########################################################################################################################################################################
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

   [JsonPropertyName("SoftwareId")]
   public string SoftwareId { get; set; }
}
public class VvMER_Request_Data_AllActions : MER_Credentials_Data
{
   //public int    Username   { get; set; }
   //public string Password   { get; set; }
   //public string CompanyId  { get; set; }
   //public string CompanyBu  { get; set; }
   //public string SoftwareId { get; set; }

   [JsonPropertyName("File")]
   public string File       { get; set; }

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
      this.Username   = Vv_Http_Web_request_QAI.VvMER_UserName  ;
      this.Password   = Vv_Http_Web_request_QAI.VvMER_Password  ;
      this.CompanyId  = Vv_Http_Web_request_QAI.VvMER_CompanyId ;
      this.CompanyBu  = ""                                      ;
      this.SoftwareId = Vv_Http_Web_request_QAI.VvMER_SoftwareId;
      this.File       = xmlString                               ;
   }

   // Query Inbox / Outbox Additions: 

   [JsonPropertyName("ElectronicId")]
   public int ElectronicId { get; set; }

   [JsonPropertyName("StatusId")]
   public int StatusId     { get; set; }

   [JsonPropertyName("From")]
   public DateTime From    { get; set; } // DeteOD 

   [JsonPropertyName("To")]
   public DateTime To      { get; set; } // DateDO 

   public VvMER_Request_Data_AllActions(int electronicId) // za jedan racun 
   {
      this.Username    = Vv_Http_Web_request_QAI.VvMER_UserName  ;
      this.Password    = Vv_Http_Web_request_QAI.VvMER_Password  ;
      this.CompanyId   = Vv_Http_Web_request_QAI.VvMER_CompanyId ;
      this.CompanyBu   = ""                                      ;
      this.SoftwareId  = Vv_Http_Web_request_QAI.VvMER_SoftwareId;
      this.ElectronicId = electronicId                           ;
   }

   public VvMER_Request_Data_AllActions(DateTime dateOD, DateTime dateDO) // za report 
   {
      this.Username    = Vv_Http_Web_request_QAI.VvMER_UserName  ;
      this.Password    = Vv_Http_Web_request_QAI.VvMER_Password  ;
      this.CompanyId   = Vv_Http_Web_request_QAI.VvMER_CompanyId ;
      this.CompanyBu   = ""                                      ;
      this.SoftwareId  = Vv_Http_Web_request_QAI.VvMER_SoftwareId;
      this.From        = dateOD                                  ;
      this.To          = dateDO                                  ;
   }

}
public class VvMER_Response_Data_SEND : Vv_XSD_Bussiness_BASE<VvMER_Response_Data_SEND>
{
   [JsonPropertyName("ElectronicId")]
   public int ElectronicId { get; set; }

   [JsonPropertyName("DocumentNr")]
   public string DocumentNr { get; set; }

   [JsonPropertyName("DocumentTypeId")]
   public int DocumentTypeId { get; set; }

   [JsonPropertyName("DocumentTypeName")]
   public string DocumentTypeName { get; set; }

   [JsonPropertyName("StatusId")]
   public int StatusId { get; set; }

   [JsonPropertyName("StatusName")]
   public string StatusName { get; set; }

   [JsonPropertyName("RecipientBusinessNumber")]
   public string RecipientBusinessNumber { get; set; }

   [JsonPropertyName("RecipientBusinessUnit")]
   public string RecipientBusinessUnit { get; set; }

   [JsonPropertyName("RecipientBusinessName")]
   public string RecipientBusinessName { get; set; }

   [JsonPropertyName("Created")]
   public string Created { get; set; }

   [JsonPropertyName("Sent")]
   public string Sent { get; set; }

   [JsonPropertyName("Modified")]
   public string Modified { get; set; }

   [JsonPropertyName("Updated")]
   public string Updated { get; set; } // za status 

   //public bool?  Delivered { get; set; }

   [JsonPropertyName("Delivered")]
   public string Delivered { get; set; }

   [JsonPropertyName("Error_PropertyName")]
   public string Error_PropertyName { get; set; }

   [JsonPropertyName("Error_PropertyValue")]
   public string Error_PropertyValue { get; set; }

   [JsonPropertyName("Error_Message")]
   public string Error_Message { get; set; }

}
public class VvMER_Response_Data_Status //: Vv_XSD_Bussiness_BASE<VvMER_Response_Data_Status>
{
   [JsonPropertyName("ElectronicId")]
   public int ElectronicId { get; set; }

   [JsonPropertyName("DocumentNr")]
   public string DocumentNr { get; set; }

   [JsonPropertyName("DocumentTypeId")]
   public int DocumentTypeId { get; set; }

   [JsonPropertyName("DocumentTypeName")]
   public string DocumentTypeName { get; set; }

   [JsonPropertyName("StatusId")]
   public int StatusId { get; set; }

   [JsonPropertyName("StatusName")]
   public string StatusName { get; set; }

   [JsonPropertyName("RecipientBusinessNumber")]
   public string RecipientBusinessNumber { get; set; }

   [JsonPropertyName("RecipientBusinessUnit")]
   public string RecipientBusinessUnit { get; set; }

   [JsonPropertyName("RecipientBusinessName")]
   public string RecipientBusinessName { get; set; }

   [JsonPropertyName("Created")]
   public string Created { get; set; }

   [JsonPropertyName("Sent")]
   public string Sent { get; set; }

   [JsonPropertyName("Modified")]
   public string Modified { get; set; }

   [JsonPropertyName("Updated")]
   public string Updated { get; set; } // za status 

   //[JsonPropertyName("Delivered0")]
   //public bool? Delivered0 { get; set; } // po send API-ju 

   [JsonPropertyName("Delivered")]
   public string Delivered { get; set; } // po queryOutbox API-ju ... ovoga ako bas mora biti jer s njim radi i send i querryOutbox ... nota bene: raditi ce i send i querryOutbox sa oba ugasena, tj. nijednim 

   [JsonPropertyName("Error_PropertyName")]
   public string Error_PropertyName { get; set; }

   [JsonPropertyName("Error_PropertyValue")]
   public string Error_PropertyValue { get; set; }

   [JsonPropertyName("Error_Message")]
   public string Error_Message { get; set; }

}

#endregion Bussiness Classes for JSON Request/Response






















// Ovo dole je Trash & Tmp ##########################################################################################################################################################################



#if PUSE_Vv_Http_Web_request_byQ

public static class Vv_Http_Web_request
{
   private static HttpWebResponse Vv_SendHttpWebRequest_GetHttpWebResponse(string webAddress, string jsonRequestString)
   {
      HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddress);
      httpWebRequest.ContentType = "application/json; charset=utf-8";
      httpWebRequest.Method = "POST";

      using(StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
      {
         streamWriter.Write(jsonRequestString);
         streamWriter.Flush();
      }

      HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

      return httpResponse;
   }

   // ############################################################################################################################################################################################################################################################

   public static VvMER_Response_Data_SEND          VvMER_WebService_SEND(string xmlString, string fullPath_XML_FileName)
   {
      //string webAddress = @"http://demo.moj-eracun.hr/hr/";
      //string webAddress = @"https://demo.moj-eracun.hr/apis/v2/send";

    //string webAddress_SEND        = @"https://www.moj-eracun.hr/apis/v2/send"       ;
      string webAddress_QueryOutbox = @"https://www.moj-eracun.hr/apis/v2/queryOutbox";

      // Web adresa Vam je ispravna za demo okruženje: https://demo.moj-eracun.hr/apis/v2/send
      // Produkcijska adresa je : https://www.moj-eracun.hr/apis/v2/send

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

      string webAddress_SEND = @"https://www.moj-eracun.hr/apis/v2/send"; // PRODUKCIJA 
    //int username   = ZXC.ValOrZero_Int(ZXC.CURR_prjkt_rec.SkyVvDomena); // PRODUKCIJA 
    //string password   = ZXC.CURR_prjkt_rec.SkyPasswordDecrypted       ; // PRODUKCIJA 
    //string companyId  = ZXC.CURR_prjkt_rec.Oib                        ; // PRODUKCIJA 
    //string companyBu  = ""                                            ; // PRODUKCIJA 
    //string softwareId = "Vektor-001"                                  ; // PRODUKCIJA 

//#if DEBUG
//         webAddress_SEND = @"https://demo.moj-eracun.hr/apis/v2/send";
//
//         username = 8633        ;
//         password   = "T22zsEY" ;
//         softwareId = "Test-001";
//
//         VvMER_Json_SEND_Request_Data json_SEND_Request_Data = new VvMER_Json_SEND_Request_Data(username, password, companyId, companyBu, softwareId, xmlString); // DEMO testiranje 
//#endif

      VvMER_Request_Data_AllActions json_SEND_Request_Data = new VvMER_Request_Data_AllActions(xmlString); // produkcija 

      JsonSerializerSettings settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };

      string jsonRequestString = JsonConvert.SerializeObject(json_SEND_Request_Data, Newtonsoft.Json.Formatting.Indented);

      VvMER_Response_Data_SEND deserializedResponseData = Vv_Http_Web_request.VvMER_PostJson_SEND(webAddress_SEND, jsonRequestString, fullPath_XML_FileName);

      return deserializedResponseData;
   }
                                                   
   public static VvMER_Response_Data_Status        VvMER_WebService_OneSingleStatus(int electronicID)
   {
      string webAddress_QueryOutbox = @"https://www.moj-eracun.hr/apis/v2/queryOutbox"; // PRODUKCIJA 
    //int username   = ZXC.ValOrZero_Int(ZXC.CURR_prjkt_rec.SkyVvDomena)              ; // PRODUKCIJA 
    //string password   = ZXC.CURR_prjkt_rec.SkyPasswordDecrypted                     ; // PRODUKCIJA 
    //string companyId  = ZXC.CURR_prjkt_rec.Oib                                      ; // PRODUKCIJA 
    //string companyBu  = ""                                                          ; // PRODUKCIJA 
    //string softwareId = "Vektor-001"                                                ; // PRODUKCIJA 

//#if DEBUG
//         webAddress_SEND = @"https://demo.moj-eracun.hr/apis/v2/send";
//
//         username = 8633        ;
//         password   = "T22zsEY" ;
//         softwareId = "Test-001";
//
//         VvMER_Json_SEND_Request_Data json_SEND_Request_Data = new VvMER_Json_SEND_Request_Data(username, password, companyId, companyBu, softwareId, xmlString); // DEMO testiranje 
//#endif

      VvMER_Request_Data_AllActions json_OneSingleSTATUS_Request_Data = new VvMER_Request_Data_AllActions(electronicID); // constructor za STATUS jednog racuna (electronicID-a) 

      JsonSerializerSettings settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };

      string jsonRequestString = JsonConvert.SerializeObject(json_OneSingleSTATUS_Request_Data, Newtonsoft.Json.Formatting.Indented, settings);

      VvMER_Response_Data_Status deserializedResponseData = Vv_Http_Web_request.VvMER_PostJson_OneSingleStatus(webAddress_QueryOutbox, jsonRequestString);

      return deserializedResponseData;
   }
                                                   
   public static List<VvMER_Response_Data_Status>  VvMER_WebService_StatusList(DateTime dateOD, DateTime dateDO)
   {
      string webAddress_QueryOutbox = @"https://www.moj-eracun.hr/apis/v2/queryOutbox"; // PRODUKCIJA 
    //int username   = ZXC.ValOrZero_Int(ZXC.CURR_prjkt_rec.SkyVvDomena)              ; // PRODUKCIJA 
    //string password   = ZXC.CURR_prjkt_rec.SkyPasswordDecrypted                     ; // PRODUKCIJA 
    //string companyId  = ZXC.CURR_prjkt_rec.Oib                                      ; // PRODUKCIJA 
    //string companyBu  = ""                                                          ; // PRODUKCIJA 
    //string softwareId = "Vektor-001"                                                ; // PRODUKCIJA 

//#if DEBUG
//         webAddress_SEND = @"https://demo.moj-eracun.hr/apis/v2/send";
//
//         username = 8633        ;
//         password   = "T22zsEY" ;
//         softwareId = "Test-001";
//
//         VvMER_Json_SEND_Request_Data json_SEND_Request_Data = new VvMER_Json_SEND_Request_Data(username, password, companyId, companyBu, softwareId, xmlString); // DEMO testiranje 
//#endif

      VvMER_Request_Data_AllActions json_STATUS_List_Request_Data = new VvMER_Request_Data_AllActions(dateOD, dateDO); // constructor za Listu STATUSa racuna (dateOD, dateDO) 

      JsonSerializerSettings settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };

      string jsonString = JsonConvert.SerializeObject(json_STATUS_List_Request_Data, Newtonsoft.Json.Formatting.Indented, settings);

      List<VvMER_Response_Data_Status> eRacun_STATUS_List = Vv_Http_Web_request.VvMER_PostJson_StatusList(webAddress_QueryOutbox, jsonString);

      return eRacun_STATUS_List;
   }

   // ############################################################################################################################################################################################################################################################

   private static VvMER_Response_Data_SEND         VvMER_PostJson_SEND(string webAddress, string jsonRequestString, string fullPath_eRacun_XML_FileName)
   {
      VvMER_Response_Data_SEND deserializedResponseData = null;

      string fullPath_send_response_XML_FileName = fullPath_eRacun_XML_FileName.Replace(".xml", "_RES.xml");

      try
      {
         HttpWebResponse httpResponse = Vv_SendHttpWebRequest_GetHttpWebResponse(webAddress, jsonRequestString);

         using(StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
         {
            string responseJson = streamReader.ReadToEnd();

            if(responseJson.NotEmpty())
            {
               try
               {
                  deserializedResponseData = VvMER_Response_Deserializator_SEND(responseJson);

                  if(deserializedResponseData.ElectronicId.IsZero()) throw new Exception("Zero ElectronicID in response JSON!");
                  if(deserializedResponseData == null              ) throw new Exception("Deserialized Response Data is null!");

                  if(fullPath_send_response_XML_FileName.NotEmpty())
                  {
                     deserializedResponseData.SaveToFile(fullPath_send_response_XML_FileName); // !!! Voila! 
                  }

               }
               catch(Exception ex2) // nije uspio Deserializirati, odi preko JsonTextReader 
               {
                  List<string> responseMsgList = new List<string>();

                  deserializedResponseData = new VvMER_Response_Data_SEND { StatusName = "Err " };

                  int errMsgRowIdx = 0;

                  JsonTextReader reader = new JsonTextReader(new StringReader(responseJson));
                  while(reader.Read())
                  {
                     if(reader.Value != null)
                     {
                        errMsgRowIdx++;

                        if(errMsgRowIdx == 1) deserializedResponseData.Error_PropertyName  = reader.Value.ToString();
                        if(errMsgRowIdx == 3) deserializedResponseData.Error_PropertyValue = reader.Value.ToString();
                        if(errMsgRowIdx == 5) deserializedResponseData.Error_Message       = reader.Value.ToString();

                        if(errMsgRowIdx == 1 || errMsgRowIdx == 3 || errMsgRowIdx == 5)
                        {
                         //responseMsgList.Add(String.Format("{0}, Value: {1}", reader.TokenType, reader.Value          ));
                         //responseMsgList.Add(String.Format("{0}: {1}"       , reader.TokenType, reader.Value          ));
                           responseMsgList.Add(                                                   reader.Value.ToString());
                        }
                     }
                     else
                     {
                      //responseMsgList.Add(String.Format("{0}", reader.TokenType));
                     }
                  }

                  ZXC.aim_emsg_List("Response Messages from JsonTextReader", responseMsgList);

                  deserializedResponseData.StatusName += deserializedResponseData.Error_PropertyName;

                  if(fullPath_send_response_XML_FileName.NotEmpty())
                  {
                     deserializedResponseData.SaveToFile(fullPath_send_response_XML_FileName); // !!! Voila! 
                  }
               }

               //Now you have your response.
               //or false depending on information in the response     

            } // if(responseJson.NotEmpty())
            else
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "JSON response is empty!");
               deserializedResponseData = new VvMER_Response_Data_SEND { StatusName = "JSON empty" };

               if(fullPath_send_response_XML_FileName.NotEmpty())
               {
                  deserializedResponseData.SaveToFile(fullPath_send_response_XML_FileName); // !!! Voila! 
               }
            }

         } // using(var streamReader = new StreamReader(httpResponse.GetResponseStream())) 

      } // try 

      catch(WebException ex)
      {
         ZXC.aim_emsg(ex.Message);
      }

      return deserializedResponseData;
   }

   private static VvMER_Response_Data_Status       VvMER_PostJson_OneSingleStatus(string webAddress, string jsonRequestString)
   {
      VvMER_Response_Data_Status deserializedResponseData = null;

      try
      {       
         HttpWebResponse httpResponse = Vv_SendHttpWebRequest_GetHttpWebResponse(webAddress, jsonRequestString);
      
         using(StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
         {
            string responseJson = streamReader.ReadToEnd();
      
            if(responseJson.NotEmpty())
            {
               try
               {
                  deserializedResponseData = VvMER_Response_Deserializator_OneSingleStatus(responseJson);
      
                  if(deserializedResponseData.ElectronicId.IsZero()) throw new Exception("Zero ElectronicID in response JSON!");
                  if(deserializedResponseData == null              ) throw new Exception("Deserialized Response Data is null!");
               }
               catch(Exception ex2) // nije uspio Deserializirati, odi preko JsonTextReader 
               {
                  List<string> responseMsgList = new List<string>();
      
                  deserializedResponseData = new VvMER_Response_Data_Status { StatusName = "Err " };
      
                  int errMsgRowIdx = 0;
      
                  JsonTextReader reader = new JsonTextReader(new StringReader(responseJson));
                  while(reader.Read())
                  {
                     if(reader.Value != null)
                     {
                        errMsgRowIdx++;
      
                        if(errMsgRowIdx == 1) deserializedResponseData.Error_PropertyName  = reader.Value.ToString();
                        if(errMsgRowIdx == 3) deserializedResponseData.Error_PropertyValue = reader.Value.ToString();
                        if(errMsgRowIdx == 5) deserializedResponseData.Error_Message       = reader.Value.ToString();
      
                        if(errMsgRowIdx == 1 || errMsgRowIdx == 3 || errMsgRowIdx == 5)
                        {
                         //responseMsgList.Add(String.Format("{0}, Value: {1}", reader.TokenType, reader.Value          ));
                         //responseMsgList.Add(String.Format("{0}: {1}"       , reader.TokenType, reader.Value          ));
                           responseMsgList.Add(                                                   reader.Value.ToString());
                        }
                     }
                     else
                     {
                      //responseMsgList.Add(String.Format("{0}", reader.TokenType));
                     }
                  }
      
                  ZXC.aim_emsg_List("Response Messages from JsonTextReader", responseMsgList);
      
                  deserializedResponseData.StatusName += deserializedResponseData.Error_PropertyName;
               }
      
               //Now you have your response.
               //or false depending on information in the response     
      
            } // if(responseJson.NotEmpty())
            else
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "JSON response is empty!");
               deserializedResponseData = new VvMER_Response_Data_Status { StatusName = "JSON empty" };
            }
      
         } // using(var streamReader = new StreamReader(httpResponse.GetResponseStream())) 
      
      } // try 
      
      catch(WebException ex)
      {
         ZXC.aim_emsg(ex.Message);
      }

      return deserializedResponseData;
   }

   private static List<VvMER_Response_Data_Status> VvMER_PostJson_StatusList(string webAddress, string jsonRequestString)
   {
      List<VvMER_Response_Data_Status> deserializedResponseDataList = null;

      try
      {       
         HttpWebResponse httpResponse = Vv_SendHttpWebRequest_GetHttpWebResponse(webAddress, jsonRequestString);
      
         using(StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
         {
            string responseJson = streamReader.ReadToEnd();
      
            if(responseJson.NotEmpty())
            {
               try
               {
                  deserializedResponseDataList = VvMER_Response_Deserializator_StatusList(responseJson);
      
                //if(deserializedResponseDataList.ElectronicId.IsZero()) throw new Exception("Zero ElectronicID in response JSON!");
                  if(deserializedResponseDataList == null              ) throw new Exception("Deserialized Response Data is null!");
               }
               catch(Exception ex2) // nije uspio Deserializirati, odi preko JsonTextReader 
               {
                  List<string> responseMsgList = new List<string>();
      
                  deserializedResponseDataList = new List<VvMER_Response_Data_Status> { new VvMER_Response_Data_Status { StatusName = "Err " } };
      
                  int errMsgRowIdx = 0;
                  int listIdx = 0;

                  JsonTextReader reader = new JsonTextReader(new StringReader(responseJson));
                  while(reader.Read())
                  {
                     if(reader.Value != null)
                     {
                        errMsgRowIdx++;
      
                        if(errMsgRowIdx == 1) deserializedResponseDataList[0].Error_PropertyName  = reader.Value.ToString();
                        if(errMsgRowIdx == 3) deserializedResponseDataList[0].Error_PropertyValue = reader.Value.ToString();
                        if(errMsgRowIdx == 5) deserializedResponseDataList[0].Error_Message       = reader.Value.ToString();
      
                        if(errMsgRowIdx == 1 || errMsgRowIdx == 3 || errMsgRowIdx == 5)
                        {
                         //responseMsgList.Add(String.Format("{0}, Value: {1}", reader.TokenType, reader.Value          ));
                         //responseMsgList.Add(String.Format("{0}: {1}"       , reader.TokenType, reader.Value          ));
                           responseMsgList.Add(                                                   reader.Value.ToString());
                        }
                     }
                     else
                     {
                      //responseMsgList.Add(String.Format("{0}", reader.TokenType));
                     }
                  }
      
                  ZXC.aim_emsg_List("Response Messages from JsonTextReader", responseMsgList);
      
                  deserializedResponseDataList[listIdx].StatusName += deserializedResponseDataList[listIdx].Error_PropertyName;
                  listIdx++;
               }
      
               //Now you have your response.
               //or false depending on information in the response     
      
            } // if(responseJson.NotEmpty())
            else
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "JSON response is empty!");
               deserializedResponseDataList[0] = new VvMER_Response_Data_Status { StatusName = "JSON empty" };
            }
      
         } // using(var streamReader = new StreamReader(httpResponse.GetResponseStream())) 
      
      } // try 
      
      catch(WebException ex)
      {
         ZXC.aim_emsg(ex.Message);
      }

      return deserializedResponseDataList;
   }

   // ############################################################################################################################################################################################################################################################

   private static VvMER_Response_Data_SEND         VvMER_Response_Deserializator_SEND(string jsonResponseString)
   {
      VvMER_Response_Data_SEND deserializedResponseData = JsonConvert.DeserializeObject<VvMER_Response_Data_SEND>(jsonResponseString);

      return deserializedResponseData;
   }

   private static VvMER_Response_Data_Status       VvMER_Response_Deserializator_OneSingleStatus(string jsonResponseString)
   {
      VvMER_Response_Data_Status deserializedResponseData = null;

      List<VvMER_Response_Data_Status> Statuses  = JsonConvert.DeserializeObject<List<VvMER_Response_Data_Status>>(jsonResponseString);

      if(Statuses.NotEmpty())
      {
         deserializedResponseData = Statuses[0]/*.ThisSTATUS_as_VvMER_Json_SEND_Response_Data*/;
      }

      return deserializedResponseData;
   }

   private static List<VvMER_Response_Data_Status> VvMER_Response_Deserializator_StatusList(string jsonResponseString)
   {
      List<VvMER_Response_Data_Status> deserializedResponseData = JsonConvert.DeserializeObject<List<VvMER_Response_Data_Status>>(jsonResponseString);

      return deserializedResponseData;
   }

   // ############################################################################################################################################################################################################################################################

}

#endif

namespace MER_ApiClient
{
   public class MER_Check_AMS_OIB_Request
   {
      public MER_Check_AMS_OIB_Request(string theOib)
      {
         IdentifierValue = theOib;
      }

      [JsonProperty("username")]
      public /*int*/string    Username { get { return Vv_Http_Web_request_QAI.VvMER_UserName; } }
    //public /*int*/string    Username { get { return @"viper@zg.htnet.hr"; } }
    //public string Username { get; set; }

      [JsonProperty("password")]
      public string Password { get { return Vv_Http_Web_request_QAI.VvMER_Password; } }
    //public string Password { get; set; }

      [JsonProperty("companyId")]
      public string CompanyId { get { return Vv_Http_Web_request_QAI.VvMER_CompanyId; } }
    //public string CompanyId { get; set; }

      [JsonProperty("softwareId")]
      public string SoftwareId { get { return Vv_Http_Web_request_QAI.VvMER_SoftwareId; } }
    //public string SoftwareId { get; set; }

      [JsonProperty("identifierType")]
      public string IdentifierType { get { return "0"; } }
    //public string IdentifierType { get; set; }

      [JsonProperty("identifierValue")]
      public string IdentifierValue { get; set; }
   }

   public class UnauthorizedResponse
   {
      [JsonProperty("Username")]
      public UsernameError Username { get; set; }
   }

   public class UsernameError
   {
      [JsonProperty("Value")]
      public string Value { get; set; }

      [JsonProperty("Messages")]
      public string[] Messages { get; set; }
   }

   public class MER_Check_AMS_OIB_ApiService
   {
      private readonly HttpClient _httpClient;

      public MER_Check_AMS_OIB_ApiService(HttpClient httpClient)
      {
         _httpClient = httpClient;
      }

      public HttpResponseMessage MER_CheckI_AMS_OIB(MER_Check_AMS_OIB_Request request)
      {
         #region POST varijanta

         var url = "/api/mps/check";
         
         // Convert request to JSON
         var json = JsonConvert.SerializeObject(request);
         var content = new StringContent(json, Encoding.UTF8, "application/json");
         
         // Send request synchronously
         HttpResponseMessage response = _httpClient.PostAsync(url, content).Result;

         #endregion POST varijanta

         #region GET varijanta

         // Build query string
         
         //var url = $"/api/mps/check?" +
         //          $"username={Uri.EscapeDataString(ZXC.ValOrZero_Int(ZXC.CURR_prjkt_rec.SkyVvDomena).ToString())}" +
         //          $"&password={Uri.EscapeDataString(ZXC.CURR_prjkt_rec.SkyPasswordDecrypted)}" +
         //          $"&companyId={Uri.EscapeDataString(ZXC.CURR_prjkt_rec.Oib)}" +
         //          $"&softwareId={Uri.EscapeDataString("Vektor-001")}" +
         //          $"&identifierType={"OIB"}" +
         //          $"&identifierValue={Uri.EscapeDataString("60042587515")}";
         //
         //// Send GET request synchronously
         //HttpResponseMessage response = _httpClient.GetAsync(url).Result;

         #endregion GET varijanta

         if(response.IsSuccessStatusCode)
         {
            Console.WriteLine("Identifier is registered (200 OK).");
         }
         else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
         {
            Console.WriteLine("Identifier not found (404 Not Found).");
         }
         else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
         {
            var errorContent = response.Content.ReadAsStringAsync().Result;
            var error = JsonConvert.DeserializeObject<UnauthorizedResponse>(errorContent);

            Console.WriteLine("Unauthorized (401):");
            if(error?.Username != null)
            {
               Console.WriteLine($"  Username: {error.Username.Value}");
               foreach(var msg in error.Username.Messages)
               {
                  Console.WriteLine($"  Message: {msg}");
               }
            }
            else
            {
               Console.WriteLine(errorContent);
            }
         }
         else
         {
            Console.WriteLine($"Unexpected status: {(int)response.StatusCode} {response.ReasonPhrase}");
         }

         return response;
      }
   }

   class Program
   {
      static void Main(string[] args)
      {
         using(var httpClient = new HttpClient { BaseAddress = new Uri(/*"https://your-api-domain.com"*/"https://www.moj-eracun.hr") })
         {
            var service = new MER_Check_AMS_OIB_ApiService(httpClient);

            var request = new MER_Check_AMS_OIB_Request("04192765979");

            service.MER_CheckI_AMS_OIB(request);
         }
      }
   }
}

#if TRLABBABBA
public abstract class F2ER_Base
{
   public abstract string ProviderName { get; }

   //public abstract string EndPointPreffix_Production { get; }
   //public abstract string EndPointPreffix_DemoTest   { get; }

   public class RESTaction
   { 
      public RESTaction(string actionName/*, bool isProduction /*string endPoint, string endPoint_DemoTest*/)
      {
         this.ActionName         = actionName       ;
         //this.EndpointProduction = EndPointPreffix_Production;
         //this.EndPoint_DemoTest  = endPoint_DemoTest;
      }
      public string ActionName { get; }
      public string Endpoint   { get; set;  }

      public string EndPoint_DemoTest { get; }
   }

   public static RESTaction Create_RESTaction(string actionName, string endPointPreffix)
   {
      RESTaction theRESTaction = new RESTaction(actionName);

      theRESTaction.Endpoint = endPointPreffix + @"/" + actionName;

      return theRESTaction;
   }
}

public class F2ER_MER: F2ER_Base
{
   public override string ProviderName => "MojEračun";

   public static string EndPointPreffix_Production => @"https://www.moj-eracun.hr/apis/v2/";
   public string EndPointPreffix_DemoTest   => @"https://demo.moj-eracun.hr/apis/v2/";


   #region _RESTsend_

   //private F2ER_Base.RESTaction action_Production_send = Create_RESTaction("send", EndPointPreffix_Production, ProviderName);

   #endregion _RESTsend_

   #region _RESTqueryOutbox_

   //F2ER_Base.RESTaction action_queryOutbox = new F2ER_Base.RESTaction("queryOutbox", @"https://www.moj-eracun.hr/apis/v2/queryOutbox", @"https://demo.moj-eracun.hr/apis/v2/queryOutbox");

   #endregion _RESTqueryOutbox_


}

public class F2ER_ePoslovanje : F2ER_Base
{
   public override string ProviderName => "ePoslovanje";


}
#endif