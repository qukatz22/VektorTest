using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MojEracun.Api
{
   #region Request

   public class PingRequest_Data
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

   public class SendRequest_Data
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

      [JsonPropertyName("File")]
      public string File { get; set; }
   }

   public class QueryDocumentsRequest_Data
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

      [JsonPropertyName("Filter")]
      public string Filter { get; set; }

      [JsonPropertyName("ElectronicId")]
      public long? ElectronicId { get; set; }

      [JsonPropertyName("StatusId")]
      public int? StatusId { get; set; }

      [JsonPropertyName("From")]
      public DateTime? From { get; set; }

      [JsonPropertyName("To")]
      public DateTime? To { get; set; }
   }

   public class ReceiveRequest_Data
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

      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }
   }

   public class UpdateDocumentProcessStatusRequest_Data
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

      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }

      [JsonPropertyName("StatusId")]
      public int StatusId { get; set; }

      [JsonPropertyName("RejectReason")]
      public string RejectReason { get; set; }
   }

   public class NotifyImportRequest_Data
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

   public class DocumentActionRequest_Data
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

      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }

      [JsonPropertyName("ActionType")]
      public string ActionType { get; set; }
   }

   public class MarkPaidRequest_Data
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

      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }

      [JsonPropertyName("PaymentDate")]
      public DateTime PaymentDate { get; set; }

      [JsonPropertyName("PaymentAmount")]
      public decimal PaymentAmount { get; set; }

      [JsonPropertyName("PaymentMethod")]
      public string PaymentMethod { get; set; }
   }

   public class RejectRequest_Data
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

      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }

      [JsonPropertyName("RejectReason")]
      public string RejectReason { get; set; }
   }

   public class FiscalizationStatusRequest_Data
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

      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }
   }

   public class RegistrationRequest_Data
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

      [JsonPropertyName("CompanyNumber")]
      public string CompanyNumber { get; set; }
   }

   public class KpdClassificationRequest_Data
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

      [JsonPropertyName("KPDCode")]
      public string KPDCode { get; set; }
   }

   public class IdentifierCheckRequest_Data
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

      [JsonPropertyName("IdentifierType")]
      public int IdentifierType { get; set; }

      [JsonPropertyName("IdentifierValue")]
      public string IdentifierValue { get; set; }
   }

   #endregion

   #region Response

   public class PingResponse_Data
   {
      [JsonPropertyName("Status")]
      public string Status { get; set; }

      [JsonPropertyName("Message")]
      public string Message { get; set; }
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
      public DateTime Created { get; set; }

      [JsonPropertyName("Sent")]
      public DateTime? Sent { get; set; }

      [JsonPropertyName("Delivered")]
      public DateTime? Delivered { get; set; }

      [JsonPropertyName("Modified")]
      public DateTime? Modified { get; set; }
   }

   public class QueryDocumentsResponse_Data
   {
      [JsonPropertyName("Documents")]
      public List<DocumentInfo_Data> Documents { get; set; }
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
      public DateTime Created { get; set; }

      [JsonPropertyName("Updated")]
      public DateTime? Updated { get; set; }

      [JsonPropertyName("Sent")]
      public DateTime? Sent { get; set; }

      [JsonPropertyName("Delivered")]
      public DateTime? Delivered { get; set; }

      [JsonPropertyName("Issued")]
      public DateTime? Issued { get; set; }

      [JsonPropertyName("Imported")]
      public bool? Imported { get; set; }
   }

   public class ReceiveResponse_Data
   {
      [JsonPropertyName("DocumentXml")]
      public string DocumentXml { get; set; }
   }

   public class UpdateDocumentProcessStatusResponse_Data
   {
      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }

      [JsonPropertyName("DokumentProcessStatus")]
      public int DokumentProcessStatus { get; set; }

      [JsonPropertyName("UpdateDate")]
      public DateTime UpdateDate { get; set; }
   }

   public class NotifyImportResponse_Data
   {
      [JsonPropertyName("Status")]
      public string Status { get; set; }
   }

   public class DocumentActionResponse_Data
   {
      [JsonPropertyName("Success")]
      public bool Success { get; set; }

      [JsonPropertyName("Message")]
      public string Message { get; set; }
   }

   public class MarkPaidResponse_Data
   {
      [JsonPropertyName("IsSuccess")]
      public bool IsSuccess { get; set; }

      [JsonPropertyName("FiscalizationTimestamp")]
      public DateTime FiscalizationTimestamp { get; set; }

      [JsonPropertyName("EncodedXml")]
      public string EncodedXml { get; set; }
   }

   public class RejectResponse_Data
   {
      [JsonPropertyName("IsSuccess")]
      public bool IsSuccess { get; set; }

      [JsonPropertyName("RejectTimestamp")]
      public DateTime RejectTimestamp { get; set; }
   }

   public class FiscalizationStatusResponse_Data
   {
      [JsonPropertyName("Status")]
      public string Status { get; set; }

      [JsonPropertyName("FiscalizationDate")]
      public DateTime FiscalizationDate { get; set; }
   }

   public class RegistrationResponse_Data
   {
      [JsonPropertyName("IsRegistered")]
      public bool IsRegistered { get; set; }
   }

   public class KpdClassificationResponse_Data
   {
      [JsonPropertyName("Description")]
      public string Description { get; set; }
   }

   public class IdentifierCheckResponse_Data
   {
      [JsonPropertyName("IsRegistered")]
      public bool IsRegistered { get; set; }

      [JsonPropertyName("CompanyName")]
      public string CompanyName { get; set; }

      [JsonPropertyName("RegistrationDate")]
      public DateTime? RegistrationDate { get; set; }

      [JsonPropertyName("Message")]
      public string Message { get; set; }
   }

   public class MerError_Data
   {
      [JsonPropertyName("Code")]
      public int Code { get; set; }

      [JsonPropertyName("Message")]
      public string Message { get; set; }
   }

   #endregion

   public class UniversalRequest_Data
   {
      // Credentials - common across all requests
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
   }

   public class UniversalResponse_Data
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

      // Collection properties
      [JsonPropertyName("Documents")]
      public List<DocumentInfo_Data> Documents { get; set; }

      // State indicators
      [JsonPropertyName("Imported")]
      public bool? Imported { get; set; }

      // Messages
      [JsonPropertyName("Message")]
      public string Message { get; set; }
   }

}

namespace EPoslovanje.Api
{
   // Common

   public class ApiError_Data
   {
      [JsonPropertyName("Code")]
      public int? Code { get; set; }

      [JsonPropertyName("Message")]
      public string Message { get; set; }

      // Unknown/extra fields from server
      [JsonExtensionData]
      public Dictionary<string, object> ExtensionData { get; set; }
   }

   public class Paging_Data
   {
      [JsonPropertyName("Page")]
      public int? Page { get; set; }

      [JsonPropertyName("PageSize")]
      public int? PageSize { get; set; }

      [JsonPropertyName("TotalCount")]
      public int? TotalCount { get; set; }

      [JsonPropertyName("TotalPages")]
      public int? TotalPages { get; set; }

      [JsonExtensionData]
      public Dictionary<string, object> ExtensionData { get; set; }
   }

   // Document list (GET /dokumenti)
   public class DocumentsListResponse_Data
   {
      [JsonPropertyName("Documents")]
      public List<DocumentInfo_Data> Documents { get; set; }

      // Some APIs return meta/paging information in a sibling object
      [JsonPropertyName("Paging")]
      public Paging_Data Paging { get; set; }

      // Optional message or status text
      [JsonPropertyName("Message")]
      public string Message { get; set; }

      // Error if present
      [JsonPropertyName("Error")]
      public ApiError_Data Error { get; set; }

      [JsonExtensionData]
      public Dictionary<string, object> ExtensionData { get; set; }
   }

   // Document summary info (items in list)
   public class DocumentInfo_Data
   {
      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }

      [JsonPropertyName("DocumentNr")]
      public string DocumentNr { get; set; }

      [JsonPropertyName("DocumentTypeId")]
      public int? DocumentTypeId { get; set; }

      [JsonPropertyName("DocumentTypeName")]
      public string DocumentTypeName { get; set; }

      [JsonPropertyName("StatusId")]
      public int? StatusId { get; set; }

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
      public DateTime? Created { get; set; }

      [JsonPropertyName("Updated")]
      public DateTime? Updated { get; set; }

      [JsonPropertyName("Sent")]
      public DateTime? Sent { get; set; }

      [JsonPropertyName("Delivered")]
      public DateTime? Delivered { get; set; }

      [JsonPropertyName("Issued")]
      public DateTime? Issued { get; set; }

      [JsonPropertyName("Imported")]
      public bool? Imported { get; set; }

      [JsonExtensionData]
      public Dictionary<string, object> ExtensionData { get; set; }
   }

   // Document details (GET /dokumenti/{id})
   public class DocumentDetailsResponse_Data : DocumentInfo_Data
   {
      // Some endpoints return embedded content/extra fields
      [JsonPropertyName("DocumentXml")]
      public string DocumentXml { get; set; }

      [JsonPropertyName("EncodedXml")]
      public string EncodedXml { get; set; }

      [JsonPropertyName("Message")]
      public string Message { get; set; }

      [JsonPropertyName("Error")]
      public ApiError_Data Error { get; set; }
   }

   // Create/send document (POST /dokumenti)
   public class SendDocumentResponse_Data
   {
      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }

      [JsonPropertyName("DocumentNr")]
      public string DocumentNr { get; set; }

      [JsonPropertyName("DocumentTypeId")]
      public int? DocumentTypeId { get; set; }

      [JsonPropertyName("DocumentTypeName")]
      public string DocumentTypeName { get; set; }

      [JsonPropertyName("StatusId")]
      public int? StatusId { get; set; }

      [JsonPropertyName("StatusName")]
      public string StatusName { get; set; }

      [JsonPropertyName("RecipientBusinessNumber")]
      public string RecipientBusinessNumber { get; set; }

      [JsonPropertyName("RecipientBusinessUnit")]
      public string RecipientBusinessUnit { get; set; }

      [JsonPropertyName("RecipientBusinessName")]
      public string RecipientBusinessName { get; set; }

      [JsonPropertyName("Created")]
      public DateTime? Created { get; set; }

      [JsonPropertyName("Sent")]
      public DateTime? Sent { get; set; }

      [JsonPropertyName("Delivered")]
      public DateTime? Delivered { get; set; }

      [JsonPropertyName("Modified")]
      public DateTime? Modified { get; set; }

      [JsonPropertyName("Message")]
      public string Message { get; set; }

      [JsonPropertyName("Error")]
      public ApiError_Data Error { get; set; }

      [JsonExtensionData]
      public Dictionary<string, object> ExtensionData { get; set; }
   }

   // Download content (e.g., GET /dokumenti/{id}/sadrzaj)
   public class DocumentContentResponse_Data
   {
      [JsonPropertyName("DocumentXml")]
      public string DocumentXml { get; set; }

      [JsonPropertyName("EncodedXml")]
      public string EncodedXml { get; set; }

      // If binary content is returned as Base64, this can be used
      [JsonPropertyName("Content")]
      public string Content { get; set; }

      [JsonPropertyName("ContentType")]
      public string ContentType { get; set; }

      [JsonPropertyName("Message")]
      public string Message { get; set; }

      [JsonPropertyName("Error")]
      public ApiError_Data Error { get; set; }

      [JsonExtensionData]
      public Dictionary<string, object> ExtensionData { get; set; }
   }

   // Update status/process (PATCH/POST /dokumenti/{id}/status or similar)
   public class UpdateDocumentStatusResponse_Data
   {
      [JsonPropertyName("ElectronicId")]
      public long ElectronicId { get; set; }

      [JsonPropertyName("StatusId")]
      public int? StatusId { get; set; }

      [JsonPropertyName("StatusName")]
      public string StatusName { get; set; }

      [JsonPropertyName("DokumentProcessStatus")]
      public int? DokumentProcessStatus { get; set; }

      [JsonPropertyName("UpdateDate")]
      public DateTime? UpdateDate { get; set; }

      [JsonPropertyName("Message")]
      public string Message { get; set; }

      [JsonPropertyName("Error")]
      public ApiError_Data Error { get; set; }

      [JsonExtensionData]
      public Dictionary<string, object> ExtensionData { get; set; }
   }

   // Generic success/failure (for actions like reject, mark paid, etc.)
   public class ActionResultResponse_Data
   {
      [JsonPropertyName("Success")]
      public bool? Success { get; set; }

      [JsonPropertyName("IsSuccess")]
      public bool? IsSuccess { get; set; }

      [JsonPropertyName("Message")]
      public string Message { get; set; }

      [JsonPropertyName("Error")]
      public ApiError_Data Error { get; set; }

      [JsonExtensionData]
      public Dictionary<string, object> ExtensionData { get; set; }
   }
}