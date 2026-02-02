using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

#region struct XtranoStruct

public struct XtranoStruct
{
   internal uint     _recID;

   /* 01 */   internal uint      _t_parentID;
   /* 02 */   internal uint      _t_dokNum;
   /* 03 */   internal ushort    _t_serial;
   /* 04 */   internal DateTime  _t_dokDate;
   /* 05 */   internal string    _t_tt;
   /* 06 */   internal uint      _t_ttNum;
                                                     /* PutNal - OstaliTroskovi */
   /* 07 */   internal decimal   _t_moneyA        ;  /* iznos troska            */
   /* 08 */   internal string    _t_opis_128      ;  /* opis troska             */
   /* 09 */   internal string    _t_konto         ;  
   /* 10 */   internal string    _t_devName       ;  /* Valuta                  */

   /* 11 */   internal byte[]    _t_XmlZip; // NE ZABORAVI u VvDaoBase.WHERE_or_SET_Clause_Specifics 'if(colName == "theLogo") continue;' za svaki BLOB / MEDIUM BLOB 

   /* 12 */   internal string    _t_theString     ;  
   /* 13 */   internal bool      _t_theBool       ;  
}

#endregion struct XtranoStruct

public class Xtrano : VvTransRecord
{

   #region Fildz

   public const string recordName = "xtrano";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private XtranoStruct currentData;
   private XtranoStruct backupData;

   #endregion Fildz

   #region Constructors

   public Xtrano() : this(0)
   {
   }

   public Xtrano(uint ID) : base()
   {
      this.currentData = new XtranoStruct();

      Memset0(ID);
   }

   public override void Memset0(uint ID)
   {
      this.currentData._recID = ID;
      //this.currentData._addTS = DateTime.MinValue;
      //this.currentData._modTS = DateTime.MinValue;
      //this.currentData._addUID = "";
      //this.currentData._modUID = "";

      // well, svi reference types (string, date, ...)

      /* 01 */ this.currentData._t_parentID         = 0;
      /* 02 */ this.currentData._t_dokNum           = 0;
      /* 03 */ this.currentData._t_serial           = 0;
      /* 04 */ this.currentData._t_dokDate          = DateTime.MinValue;
      /* 05 */ this.currentData._t_tt               = "";
      /* 06 */ this.currentData._t_ttNum            = 0;
      /* 07 */ this.currentData._t_moneyA           = 0.00M;
      /* 08 */ this.currentData._t_opis_128         = "";
      /* 09 */ this.currentData._t_konto            = ""; 
      /* 10 */ this.currentData._t_devName          = ""; 
      /* 11 */ this.currentData._t_XmlZip           = null; 
      /* 12 */ this.currentData._t_theString        = ""; 
      /* 13 */ this.currentData._t_theBool          = false; 

   }

   #endregion Constructors

   #region Sorters

   private VvSQL.RecordSorter[] _sorters = null;

   public override VvSQL.RecordSorter[] Sorters
   {
      get { return _sorters; }
   }


   public override object[] SorterCurrVal(VvSQL.SorterType sortType)
   {
      throw new Exception("Mislim da se ovo nema zasto ikada pozivati?!. not really sure yet.");
   }

   public override VvSQL.RecordSorter DefaultSorter
   {
      //get { return Xtrano.sorter_Person_DokDate_DokNum; }
      get
      {
         return Rtrans.sorterArtiklCD;
         //throw new Exception("Mislim da se ovo nema zasto ikada pozivati?!. not really sure yet."); 
         /*return new VvSQL.RecordSorter();*/
      }
   }

   #endregion Sorters

   #region propertiz

   internal XtranoStruct CurrentData // cijela XtranoStruct struct-ura 
   {
      get { return this.currentData; }
      set { this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.XtranoDao; }
   }

   public override string VirtualRecordName
   {
      get { return Xtrano.recordName; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "ot"; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Xtrano.recordNameArhiva; }
   }

   public override string DocumentRecordName
   {
      get { return Mixer.recordName; }
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()] // Da GetDgvLineFields ne misli krivo: "if(recID > 0) // Postojeci redak" 
   public override uint VirtualRecID
   {
      get { return this.T_recID; }
      set {        this.T_recID = value; }
   }

   public override uint VirtualParentRecID
   {
      get { return this.T_parentID; }
      set {        this.T_parentID = value; }
   }

   public override ushort VirtualT_Serial
   {
      get { return this.T_serial; }
      set {        this.T_serial = value; }
   }

   public override uint VirtualT_dokNum
   {
      get { return this.T_dokNum; }
      set {        this.T_dokNum = value; }
   }

   public override uint VirtualT_ttNum
   {
      get { return this.T_ttNum;         }
      set {        this.T_ttNum = value; }
   }

   public static string MixerForeignKey
   {
      get { return "t_parentID"; }
   }

   //===================================================================
   //===================================================================
   //===================================================================


   [System.Xml.Serialization.XmlIgnoreAttribute()] // Da GetDgvLineFields ne misli krivo: "if(recID > 0) // Postojeci redak" 
   public uint T_recID
   {
      get { return this.currentData._recID; }
      set { this.currentData._recID = value; }
   }

   //===================================================================

   /* */

   /* 01 */ public uint T_parentID
   {
      get { return this.currentData._t_parentID; }
      set {        this.currentData._t_parentID = value; }
   }
   /* 02 */ public uint T_dokNum
   {
      get { return this.currentData._t_dokNum; }
      set {        this.currentData._t_dokNum = value; }
   }
   public uint F2_ElectronicID
   {
      get { return this.T_dokNum;         }
      set {        this.T_dokNum = value; }
   }
   /* 03 */ public ushort T_serial
   {
      get { return this.currentData._t_serial; }
      set {        this.currentData._t_serial = value; }
   }

   public ZXC.F2_StatusInAndOutBoxEnum F2_IsFisk       
   { 
      get 
      { 
         return (ZXC.F2_StatusInAndOutBoxEnum)this.T_serial; } set { this.T_serial = (ushort)value; 
      } 
   }   
   /* 04 */ public DateTime T_dokDate
   {
      get { return this.currentData._t_dokDate; }
      set {        this.currentData._t_dokDate = value; }
   }
   /* 05 */ public string T_TT
   {
      get { return this.currentData._t_tt; }
      set { this.currentData._t_tt = value; }
   }
   /* 06 */ public uint T_ttNum
   {
      get { return this.currentData._t_ttNum; }
      set {        this.currentData._t_ttNum = value; }
   }
   /* 07 */ public decimal T_moneyA
   {
      get { return this.currentData._t_moneyA; }
      set {        this.currentData._t_moneyA = value; }
   }
   /* 08 */ public string T_opis_128
   {
      get { return this.currentData._t_opis_128; }
      set {        this.currentData._t_opis_128 = value; }
   }
   /* 09 */ public string T_konto
   {
      get { return this.currentData._t_konto; }
      set {        this.currentData._t_konto = value; }
   }
   /* 10 */ public string T_devName
   {
      get { return this.currentData._t_devName; }
      set {        this.currentData._t_devName = value; }
   }
   // NE ZABORAVI u VvDaoBase.WHERE_or_SET_Clause_Specifics 'if(colName == "theLogo") continue;' za svaki BLOB / MEDIUM BLOB 
   public byte[] T_XmlZip
   {
      get { return this.currentData._t_XmlZip; }
      set {        this.currentData._t_XmlZip = value; }
   }
   /* 12 */ public string T_theString
   {
      get { return this.currentData._t_theString; }
      set {        this.currentData._t_theString = value; }
   }
   /* 13 */ public bool T_theBool
   {
      get { return this.currentData._t_theBool; }
      set {        this.currentData._t_theBool = value; }
   }
   public bool F2_IsReject
   {
      get { return this.T_theBool; } set { this.T_theBool = value; }
   }

   #endregion propertiz

   #region ToString

   public override string ToString()
   {
      return " TT: "     + T_TT + "-" + T_ttNum + " (" + T_dokDate.ToShortDateString() + ")" +
             " Ser: "    + T_serial + String.Format(" kn:{0:N}", T_moneyA);
   }

   #endregion ToString

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<XtranoStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<XtranoStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<XtranoStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<XtranoStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<XtranoStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Xtrano newObject = new Xtrano();

      Generic_CloneData<XtranoStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      return newObject;
   }

   public Xtrano MakeDeepCopy()
   {
      return (Xtrano)Clone();
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Xtrano();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Xtrano)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Xtrano)vvDataRecord).currentData;
   }

   #endregion VvDataRecordFactory

   #region PDF Viewer

   // the_eRacun moze imati vise pdf dokumenata u sebi, pa vrati listu byte[]-ova
   public List<byte[]> F2_Get_PDF_Bytes_List()
   {
      if(this.T_TT != Mixer.TT_AUR && this.T_TT != Mixer.TT_AIR) return null;
      
      if(this.T_XmlZip is null || this.T_XmlZip.Length.IsZero()) return null;

      string xmlString = VvStringCompressor.DecompressXml(this.T_XmlZip);

      EN16931.UBL.InvoiceType the_eRacun = EN16931.UBL.InvoiceType.Deserialize(xmlString);

      //// Get the PDF bytes from the_eRacun
      //byte[] pdfBytes = null;
      //
      //if(the_eRacun.AdditionalDocumentReference != null && the_eRacun.AdditionalDocumentReference.Length > 0)
      //{
      //   var docRef = the_eRacun.AdditionalDocumentReference[0];
      //   if(docRef.Attachment != null && docRef.Attachment.EmbeddedDocumentBinaryObject != null)
      //   {
      //      pdfBytes = docRef.Attachment.EmbeddedDocumentBinaryObject.Value;
      //   }
      //}

      // Example: extract all PDFs from the_eRacun
      var pdfDocs = the_eRacun.AdditionalDocumentReference?
          .Where(docRef =>
              docRef?.Attachment?.EmbeddedDocumentBinaryObject != null &&
              docRef.Attachment.EmbeddedDocumentBinaryObject.mimeCode == "application/pdf")
          .ToList();

      List<byte[]> pdfBytesList = new List<byte[]>();

      foreach(var docRef in pdfDocs)
      {
         var pdfBytes = docRef.Attachment.EmbeddedDocumentBinaryObject.Value;

         pdfBytesList.Add(pdfBytes);
         //var filename = docRef.Attachment.EmbeddedDocumentBinaryObject.filename ?? "output.pdf";
         //File.WriteAllBytes($@"C:\Path\To\{filename}", pdfBytes);
      }

      return pdfBytesList;
   }

   public List<string> F2_GetPdfFilenamesFrom_eRacun()
   {
      if(this.T_TT != Mixer.TT_AUR && this.T_TT != Mixer.TT_AIR) return null;

      if(this.T_XmlZip is null || this.T_XmlZip.Length.IsZero()) return null;

      string xmlString = VvStringCompressor.DecompressXml(this.T_XmlZip);

      EN16931.UBL.InvoiceType the_eRacun = EN16931.UBL.InvoiceType.Deserialize(xmlString);

      var pdfDocs = the_eRacun.AdditionalDocumentReference?
          .Where(docRef =>
              docRef?.Attachment?.EmbeddedDocumentBinaryObject != null &&
              docRef.Attachment.EmbeddedDocumentBinaryObject.mimeCode == "application/pdf")
          .ToList();

      List<string> filenames = new List<string>();

      if(pdfDocs != null)
      {
         foreach(var docRef in pdfDocs)
         {
            var filename = docRef.Attachment.EmbeddedDocumentBinaryObject.filename;
            if(!string.IsNullOrEmpty(filename))
               filenames.Add(filename);
            else
               filenames.Add("output.pdf"); // fallback if filename is missing
         }
      }

      return filenames;
   }

   public List<(string Filename, byte[] PdfBytes)> F2_GetPdfFilesWithNamesOLD()
   {
      List<(string Filename, byte[] PdfBytes)> result = new List<(string Filename, byte[] PdfBytes)>();

      if(this.T_TT != Mixer.TT_AUR && this.T_TT != Mixer.TT_AIR) return result;

      if(this.T_XmlZip is null || this.T_XmlZip.Length.IsZero()) return result;

      string xmlString = VvStringCompressor.DecompressXml(this.T_XmlZip);

      EN16931.UBL.InvoiceType the_eRacun = EN16931.UBL.InvoiceType.Deserialize(xmlString);

      var pdfDocs = the_eRacun.AdditionalDocumentReference?
          .Where(docRef =>
              docRef?.Attachment?.EmbeddedDocumentBinaryObject != null &&
              docRef.Attachment.EmbeddedDocumentBinaryObject.mimeCode == "application/pdf")
          .ToList();

      if(pdfDocs != null)
      {
         foreach(var docRef in pdfDocs)
         {
            var filename = docRef.Attachment.EmbeddedDocumentBinaryObject.filename;
            if(string.IsNullOrEmpty(filename))
               filename = "output.pdf";
            else
               filename = System.IO.Path.GetFileName(filename); // Extract filename only from full path

            var pdfBytes = docRef.Attachment.EmbeddedDocumentBinaryObject.Value;
            result.Add((filename, pdfBytes));
         }
      }

      return result;
   }

   public List<(string Filename, byte[] PdfBytes)> F2_GetPdfFilesWithNames()
   {
      List<(string Filename, byte[] PdfBytes)> result = new List<(string Filename, byte[] PdfBytes)>();

      if(this.T_TT != Mixer.TT_AUR && this.T_TT != Mixer.TT_AIR) return result;

      if(this.T_XmlZip is null || this.T_XmlZip.Length.IsZero()) return result;

      string xmlString = VvStringCompressor.DecompressXml(this.T_XmlZip);

      try
      {
         XDocument xmlDoc = XDocument.Parse(xmlString);

         // Define namespaces
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

         foreach(var docRef in pdfElements)
         {
            var attachment = docRef.Element(cac + "Attachment");
            var embeddedObj = attachment?.Element(cbc + "EmbeddedDocumentBinaryObject");

            if(embeddedObj != null)
            {
               string filename = embeddedObj.Attribute("filename")?.Value ?? "output.pdf";
               filename = System.IO.Path.GetFileName(filename);

               string base64Content = embeddedObj.Value?.Trim();
               if(!string.IsNullOrEmpty(base64Content))
               {
                  byte[] pdfBytes = Convert.FromBase64String(base64Content);
                  result.Add((filename, pdfBytes));
               }
            }
         }
      }
      catch(Exception ex)
      {
         Console.WriteLine($"XML parsing error: {ex.Message}");
      }

      return result;
   }
   #endregion PDF Viewer
}

