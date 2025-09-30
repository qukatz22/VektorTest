using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MojEracun.Api
{
   #region Common

   public class MerCredentials_Data
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

   public class MerError_Data
   {
      [JsonPropertyName("Code")]
      public int Code { get; set; }

      [JsonPropertyName("Message")]
      public string Message { get; set; }
   }

   #endregion

   #region Ping

   public class PingResponse_Data
   {
      [JsonPropertyName("Status")]
      public string Status { get; set; }

      [JsonPropertyName("Message")]
      public string Message { get; set; }
   }

   #endregion

   #region Send

   public class SendRequest_Data : MerCredentials_Data
   {
      [JsonPropertyName("File")]
      public string File { get; set; }
   }

   public class SendResponse_Data
   {
      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }

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
      public DateTimeOffset Created { get; set; }

      [JsonPropertyName("Sent")]
      public DateTimeOffset? Sent { get; set; }

      [JsonPropertyName("Delivered")]
      public DateTimeOffset? Delivered { get; set; }

      [JsonPropertyName("Modified")]
      public DateTimeOffset? Modified { get; set; }
   }

   #endregion

   #region QueryInbox / QueryOutbox

   public class QueryDocumentsRequest_Data : MerCredentials_Data
   {
      [JsonPropertyName("Filter")]
      public string Filter { get; set; }

      [JsonPropertyName("ElectronicId")]
      public long? ElectronicId { get; set; }

      [JsonPropertyName("StatusId")]
      public int? StatusId { get; set; }

      [JsonPropertyName("From")]
      public DateTimeOffset? From { get; set; }

      [JsonPropertyName("To")]
      public DateTimeOffset? To { get; set; }
   }

   public class DocumentInfo_Data
   {
      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }

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

      [JsonPropertyName("Created")]
      public DateTimeOffset Created { get; set; }

      [JsonPropertyName("Updated")]
      public DateTimeOffset? Updated { get; set; }

      [JsonPropertyName("Sent")]
      public DateTimeOffset? Sent { get; set; }

      [JsonPropertyName("Delivered")]
      public DateTimeOffset? Delivered { get; set; }

      [JsonPropertyName("Issued")]
      public DateTimeOffset? Issued { get; set; }

      [JsonPropertyName("Imported")]
      public bool? Imported { get; set; }
   }

   public class QueryDocumentsResponse_Data
   {
      [JsonPropertyName("Documents")]
      public List<DocumentInfo_Data> Documents { get; set; }
   }

   #endregion

   #region Receive

   public class ReceiveRequest_Data : MerCredentials_Data
   {
      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }
   }

   public class ReceiveResponse_Data
   {
      [JsonPropertyName("DocumentXml")]
      public string DocumentXml { get; set; }
   }

   #endregion

   #region Update Document Process Status

   public class UpdateDocumentProcessStatusRequest_Data : MerCredentials_Data
   {
      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }

      [JsonPropertyName("StatusId")]
      public int StatusId { get; set; }

      [JsonPropertyName("RejectReason")]
      public string RejectReason { get; set; }
   }

   public class UpdateDocumentProcessStatusResponse_Data
   {
      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }

      [JsonPropertyName("DokumentProcessStatus")]
      public int DokumentProcessStatus { get; set; }

      [JsonPropertyName("UpdateDate")]
      public DateTimeOffset UpdateDate { get; set; }
   }

   #endregion

   #region NotifyImport

   public class NotifyImportRequest_Data : MerCredentials_Data { }

   public class NotifyImportResponse_Data
   {
      [JsonPropertyName("Status")]
      public string Status { get; set; }
   }

   #endregion

   #region DocumentAction

   public class DocumentActionRequest_Data : MerCredentials_Data
   {
      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }

      [JsonPropertyName("ActionType")]
      public string ActionType { get; set; }
   }

   public class DocumentActionResponse_Data
   {
      [JsonPropertyName("Success")]
      public bool Success { get; set; }

      [JsonPropertyName("Message")]
      public string Message { get; set; }
   }

   #endregion

   #region MarkPaid

   public class MarkPaidRequest_Data : MerCredentials_Data
   {
      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }

      [JsonPropertyName("PaymentDate")]
      public DateTimeOffset PaymentDate { get; set; }

      [JsonPropertyName("PaymentAmount")]
      public decimal PaymentAmount { get; set; }

      [JsonPropertyName("PaymentMethod")]
      public string PaymentMethod { get; set; }
   }

   public class MarkPaidResponse_Data
   {
      [JsonPropertyName("IsSuccess")]
      public bool IsSuccess { get; set; }

      [JsonPropertyName("FiscalizationTimestamp")]
      public DateTimeOffset FiscalizationTimestamp { get; set; }

      [JsonPropertyName("EncodedXml")]
      public string EncodedXml { get; set; }
   }

   #endregion

   #region Reject

   public class RejectRequest_Data : MerCredentials_Data
   {
      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }

      [JsonPropertyName("RejectReason")]
      public string RejectReason { get; set; }
   }

   public class RejectResponse_Data
   {
      [JsonPropertyName("IsSuccess")]
      public bool IsSuccess { get; set; }

      [JsonPropertyName("RejectTimestamp")]
      public DateTimeOffset RejectTimestamp { get; set; }
   }

   #endregion

   #region eReporting / FiscalizationStatus / Registration / KPD

   public class FiscalizationStatusRequest_Data : MerCredentials_Data
   {
      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }
   }

   public class FiscalizationStatusResponse_Data
   {
      [JsonPropertyName("Status")]
      public string Status { get; set; }

      [JsonPropertyName("FiscalizationDate")]
      public DateTimeOffset FiscalizationDate { get; set; }
   }

   public class RegistrationRequest_Data : MerCredentials_Data
   {
      [JsonPropertyName("CompanyNumber")]
      public string CompanyNumber { get; set; }
   }

   public class RegistrationResponse_Data
   {
      [JsonPropertyName("IsRegistered")]
      public bool IsRegistered { get; set; }
   }

   public class KpdClassificationRequest_Data : MerCredentials_Data
   {
      [JsonPropertyName("KPDCode")]
      public string KPDCode { get; set; }
   }

   public class KpdClassificationResponse_Data
   {
      [JsonPropertyName("Description")]
      public string Description { get; set; }
   }

   #endregion

   #region IdentifierCheck

   public class IdentifierCheckRequest_Data : MerCredentials_Data
   {
      [JsonPropertyName("IdentifierType")]
      public int IdentifierType { get; set; } // 0=OIB,1=VAT,99=Other

      [JsonPropertyName("IdentifierValue")]
      public string IdentifierValue { get; set; }
   }

   public class IdentifierCheckResponse_Data
   {
      [JsonPropertyName("IsRegistered")]
      public bool IsRegistered { get; set; }

      [JsonPropertyName("CompanyName")]
      public string CompanyName { get; set; }

      [JsonPropertyName("RegistrationDate")]
      public DateTimeOffset? RegistrationDate { get; set; }

      [JsonPropertyName("Message")]
      public string Message { get; set; }
   }

   #endregion
}
