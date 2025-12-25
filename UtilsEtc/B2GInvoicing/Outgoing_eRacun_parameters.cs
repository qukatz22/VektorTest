using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Outgoing_eRacun_parameters
{
   /* oeRp_1. */ public Faktur     faktur_rec              { get; set; }
   /* oeRp_2. */ public Kupdob     kupdob_rec              { get; set; }
   /* oeRp_3. */ public Kupdob     primPlat_rec            { get; set; }
   /* oeRp_4. */ public PrnFakDsc  thePFD                  { get; set; }
   /* oeRp_5. */ public byte[]     PDF_as_base64_byteArray { get; set; }
   /* oeRp_6. */ public string     pdfFileNameOnly         { get; set; }
   /* oeRp_7. */ public string     fullPath_XML_FileName   { get; set; }

   // 'qwe' je za F2 2026 set varijabli 
   /*         */ public string     qweTheDirectoryName     { get; set; }
   /*         */ public string     qweFileNameBaseOnly     { get; set; }
   /*         */ public string     qwePDFfileNameOnly      { get { return qweFileNameBaseOnly + ".pdf"; } }
   /*         */ public string     qweXMLfileNameOnly      { get { return qweFileNameBaseOnly + ".xml"; } }
   /*         */ public string     qwePDFfullPathAndName   { get { return System.IO.Path.Combine(qweTheDirectoryName, qwePDFfileNameOnly); } }
   /*         */ public string     qweXMLfullPathAndName   { get { return System.IO.Path.Combine(qweTheDirectoryName, qweXMLfileNameOnly); } }

   public string suggestedXmlFileName
   {
      get
      {
         return faktur_rec.TT_And_TtNum + "-" + faktur_rec.DokDate_DDMMYY + "-" + faktur_rec.KupdobTK/*KupdobName*/;
      }
   }

   public bool IsOneOnlyFromFakturDUC { get; set; }

   public Outgoing_eRacun_parameters(bool _isOneOnlyFromFakturDUC)
   {
      IsOneOnlyFromFakturDUC = _isOneOnlyFromFakturDUC;
   }
}
