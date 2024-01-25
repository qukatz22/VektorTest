using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;


#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using                  MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
using XSqlDataReader = MySql.Data.MySqlClient.MySqlDataReader;
using XSqlErrorCode  = MySql.Data.MySqlClient.MySqlErrorCode;
using XSqlException  = MySql.Data.MySqlClient.MySqlException;
using System.Windows.Forms;
using System.Xml.Linq;
#endif


public static class VvImpExp 
{
   #region IMPORT

   public static string[] GetAllFileNamesInDirectory(string _DirectoryPath)
   {
      string[] fNames;
      DirectoryInfo dInfo = new DirectoryInfo(_DirectoryPath);

      if(!dInfo.Exists)
      {
         ZXC.aim_emsg("The directory \n[{0}]\ndoes not exist.", _DirectoryPath);
         return null;
      }

      // Call the GetFileSystemInfos method.
      FileInfo[] fInfos = dInfo.GetFiles();

      fNames = new string[fInfos.Length];

      int i = 0;
      foreach(FileInfo fInfo in fInfos.OrderBy(f => f.FullName))
      {
         fNames[i++] = fInfo.FullName;
      }

      return fNames;
   }

   public static string[] GetFileLinesAsStringArray(string fName)
   {
      if(!File.Exists(fName))
      {
         //VvSQL.ReportGenericError("OPEN FILE", string.Format("Ne mogu otvoriti datoteku!\n\n\"{0}\"", Environment.CurrentDirectory + "\\" + fName), System.Windows.Forms.MessageBoxButtons.localOK);
         VvSQL.ReportGenericError("OPEN FILE", string.Format("Ne mogu otvoriti datoteku!\n\n\"{0}\"", fName), System.Windows.Forms.MessageBoxButtons.OK); 
         return null;
      }

      string[] lines = File.ReadAllLines(fName, Encoding.GetEncoding(1250));

      #region Problem da 27.4.2011: 
      
      // od 27.4.2011 odjemput moj Ripley7 vise ne snima txt kao 1250 nego UTF-8!? Pa moram preduhitriti obje opcije 

      bool isMaybeUTF_8 = lines.Any(line => line.Length != ZapIzvod.OldZapLineLength && line.Length != ZapIzvod.NewZapLineLength);
         //ZapIzvod.OldZapLineLength =  250; 
         //ZapIzvod.NewZapLineLength = 1000; 

      if(isMaybeUTF_8)
      {
         lines = File.ReadAllLines(fName, Encoding.UTF8);
      }

      #endregion Problem da 27.4.2011:

      return lines;
   }

   public static string[] SeparateStringsFromImportLine(string importLine, int[] partLenghts)
   {
      int cursor = 0, i = 0, checkSum;
      char[] chars;
      string[] strings = new string[partLenghts.Length];

      checkSum = partLenghts.Sum(); // Linq 

      if(checkSum != importLine.Length)
      {
         VvSQL.ReportGenericError("SeparateStringsFromImportLine", string.Format("Suma partLenghtsa {0} nije jednaka duljini importLinije {1}",
            checkSum, importLine.Length), System.Windows.Forms.MessageBoxButtons.OK);
         return null;
      }

      foreach(int partLen in partLenghts)
      {
         chars = new char[partLen];

         importLine.CopyTo(/*sourceIndex*/ cursor, /*destination*/ chars, /*destinationIndex*/ 0, /*count*/ chars.Length);

         cursor += chars.Length;

         strings[i++] = new string(chars).TrimEnd(' ');
      }

      return strings;
   }

   #endregion IMPORT

   #region EXPORT

   #region struct ImpExpField

   /// <summary>
   /// 'Natural' zanaci da nije zero based index, nego 1 based index
   /// </summary>
   public /*struct*/ class ImpExpField
   {
      string fldName;
      int    fldRbr_Natural;
      int    fldStartIdx_Natural;
      int    fldLength;
      string fldValue;

      public ImpExpField(string name, int rbr, int startIdxNatural, int length)
      {
         this.fldName         = name;
         this.fldRbr_Natural  = rbr;

         this.fldStartIdx_Natural = startIdxNatural;
         this.fldLength           = length;

         this.fldValue = "";
      }

      // new constructor for 26.04.2012: 
      public ImpExpField(string name, int rbr, int length) : this(name, rbr, 0, length)
      {
      }

      // 21.03.2010: neznam koji kooratz uopce sluzi 'startIdxNatural' parametar

      public string FldName               { get { return fldName; } }
      public int    FldRbr_Natural        { get { return fldRbr_Natural; } }
      //public int    FldStartIdx_Natural   { get { return fldStartIdx_Natural; } }
      public int    FldLength             { get { return fldLength; } }
      //public int    FldStartIdx_ZeroBased { get { return fldStartIdx_Natural - 1; } }

      public string FldValue
      {
         get 
         { 
            return fldValue; 
         }
         set 
         {
            // Odrezi prvo eventualni visak. .NET formatiranje nema "%10.10s" !
            if(value.Length > FldLength) value = value.Substring(0, FldLength);

            string formatter = string.Format("{{0,-{0}}}", this.FldLength);

            fldValue = string.Format(formatter, value);
         }
      } // public string ElValue 

      public string SetDecimalFldValue(decimal num)
      {
         string formatter = string.Format("{{0:D{0}}}", this.FldLength); 

         this.fldValue = string.Format(formatter, (long)(num * 100.00M));

         return FldValue;
      }

      public static string SetDecimalFldValue_Static(decimal num, int fldLength)
      {
         string formatter = string.Format("{{0:D{0}}}", fldLength);

         return string.Format(formatter, (long)(num * 100.00M));
      }

      public string SetDecimalFldValue_RSm(decimal num)
      {
         string formatter = string.Format("+{{0:D{0}}}", this.FldLength-1);

         this.fldValue = string.Format(formatter, (long)(num * 100.00M));

         return FldValue;
      }

      public string SetIntgerFldValue(int num)
      {
         string formatter = string.Format("{{0:D{0}}}", this.FldLength); 

         this.fldValue = string.Format(formatter, num);

         return FldValue;
      }

      public string SetDDMMYYYYFldValue(DateTime dt)
      {
         if(this.FldLength != 8) throw new Exception(FldName + ": duljina " + FldLength + " ne odgovara formatu DDMMYYYY!");

         this.fldValue = dt.ToString("ddMMyyyy");

         return FldValue;
      }

      public string SetYYYYMMDDFldValue(DateTime dt)
      {
         if(this.FldLength != 8) throw new Exception(FldName + ": duljina " + FldLength + " ne odgovara formatu YYYYMMDD!");

         this.fldValue = dt.ToString("yyyyMMdd");

         return FldValue;
      }

      public string SetDDMMYYFldValue(DateTime dt)
      {
         if(this.FldLength != 6) throw new Exception(FldName + ": duljina " + FldLength + " ne odgovara formatu DDMMYY!");

         this.fldValue = dt.ToString("ddMMyy");

         return FldValue;
      }

      public string SetZirornPartFldValue(string completeZirorn, ZXC.Redak zeljeniKomad)
      {
         string[] splitters;

         splitters = completeZirorn.Split("-".ToCharArray());
         if(splitters.Length > 2) { ZXC.aim_emsg("UPOZORENJE!: ZIRO RACUN [{0}] ima vise od 1 znaka '-'.!!!", completeZirorn); return "xxx"; }
         if(splitters.Length < 2) { ZXC.aim_emsg("UPOZORENJE!: ZIRO RACUN [{0}] nema znak '-'.!!!"          , completeZirorn); return "yyy"; }

         switch(zeljeniKomad)
         {
            case ZXC.Redak.prvi : this.FldValue = splitters[0]; break;
            case ZXC.Redak.drugi: this.FldValue = splitters[1]; break;

            default: throw new Exception("Trazis krivi komad ziroracuna!");
         }

         return FldValue;
      }

   }

   #endregion struct ImpExpField

   //public static string BuildStringForExportLine(string[] partStrings, int[] partLenghts)
   //{
   //   int col = 0;
   //   string formatter;
   //   StringBuilder buildedExportLine = new StringBuilder(partLenghts.Length);

   //   foreach(int partLen in partLenghts)
   //   {
   //      formatter = string.Format("{{0,-{0}}}", partLenghts[col]);

   //      buildedExportLine.AppendFormat(formatter, partStrings[col]);

   //      col++;
   //   }
   //   buildedExportLine.AppendLine(); // dodaje new line 

   //   return buildedExportLine.ToString();
   //}

   public static string BuildStringForExportLine(VvImpExp.ImpExpField[] fields)
   {
      string formatter;
      StringBuilder buildedExportLine = new StringBuilder(fields.Length);

      foreach(VvImpExp.ImpExpField field in fields)
      {
         //if(element.ElValue.Length != element.ElLength) throw new Exception(element.ElName + ": ElValue.Length: " + element.ElValue.Length + " vs element.ElLength: " + element.ElLength + "!"); 
         
         formatter = string.Format("{{0,-{0}}}", field.FldLength);

         buildedExportLine.AppendFormat(formatter, field.FldValue);
      }

      //buildedExportLine.AppendLine(); // dodaje new line. Ne treba, StreamWriter will do. 

      return buildedExportLine.ToString();
   }

   public static Dictionary<string, VvImpExp.ImpExpField> CreateDictionary(VvImpExp.ImpExpField[] fields, string identifyer, uint? _lineFixedLength)
   {
      int lenSum;

      lenSum = fields.Sum(fld => fld.FldLength);

      Dictionary<string, VvImpExp.ImpExpField> dictionary;

      dictionary = fields.ToDictionary(fld => fld.FldName);

      if(_lineFixedLength != null && _lineFixedLength != fields.Sum(el => el.FldLength)) throw new Exception(identifyer + ": Fields lengths " + fields.Sum(el => el.FldLength) + " nije " + _lineFixedLength + "!");

      return dictionary;
   }

   public static void ClearFldValues(VvImpExp.ImpExpField[] impExpFields)
   {
      foreach(VvImpExp.ImpExpField field in impExpFields)
      {
         field.FldValue = "";
      }
   }

   public static void DumpFields(StreamWriter sw, VvImpExp.ImpExpField[] fields)
   {
      string dumpLine = BuildStringForExportLine(fields);

      sw.WriteLine(dumpLine);
   }

   #endregion EXPORT

}

public abstract class VvDataRecordImporter
{
   #region Fieldz & Propertiz

   private string[]fields;

   public XSqlConnection TheDbConn{ get; set; }

   public string FullPathFileName { get; set; }
   public string Delimiter        { get; set; }

   protected abstract void ProcessLine     (XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess);
   protected virtual  void PostProcessLines(XSqlConnection conn) {}

   #endregion Fieldz & Propertiz

   #region Constructor & Methodz

   protected VvDataRecordImporter(XSqlConnection conn, string fullPathFileName, string delimiter)
   {
      this.TheDbConn        = conn;
      this.FullPathFileName = fullPathFileName;
      this.Delimiter        = delimiter;
   }

   /// <summary>
   /// If true, means; ADDREC-anje ne ide pri StreamReader.ReadLine() nego semo ADD-a u List(VvSomeDataRecord) - u, 
   /// A sam ADDREC ide u PostProcessLines(), nakon sto si nesto sortirao, grupirao, izbacio, ... itd.
   /// </summary>
   /// <param name="addrecGoesOnPostprocess"></param>
   /// <returns></returns>
   public int LoadData(bool addrecGoesOnPostprocess)
   {
      return LoadData(addrecGoesOnPostprocess, "");
   }

   public int LoadData(bool addrecGoesOnPostprocess, string skladCD)
   {
      int i = 0;
      ushort line = 0;

      if(!File.Exists(FullPathFileName))
      {
         VvSQL.ReportGenericError("OPEN FILE", string.Format("Ne mogu otvoriti datoteku!\n\n\"{0}\"", Environment.CurrentDirectory + "\\" + FullPathFileName), System.Windows.Forms.MessageBoxButtons.OK);
         return 0;
      }

      // 13.12.2010
    //using(StreamReader sr = File.OpenText(FullPathFileName))
      using(StreamReader sr = new StreamReader(FullPathFileName, Encoding.GetEncoding(1250)))
      {
         string lineFromFile;

         while((lineFromFile = sr.ReadLine()) != null)
         {
            fields = lineFromFile.Split(Delimiter.ToCharArray());

            if(this is VvRtransImporter && skladCD.NotEmpty())
            {
               (this as VvRtransImporter).forcedSkladCD = skladCD;
            }
            ProcessLine(TheDbConn, ref line, addrecGoesOnPostprocess);

            ++i;
         }
         sr.Close();

      } // using(StreamReader sr = File.OpenText(FullPathFileName)) 

      if(this is VvRtransImporter) // '... i jos jemput za zadnjega' pattern 
      {
         if(((VvRtransImporter)this).ImportKind == VvRtransImporter.ImportKindEnum.Offix || 
            ((VvRtransImporter)this).ImportKind == VvRtransImporter.ImportKindEnum.SVDUH || 
            ((VvRtransImporter)this).ImportKind == VvRtransImporter.ImportKindEnum.OffixPPUK)
         {
            ((VvRtransImporter)this).OnLastRtrans_Action();
         }
      }

      PostProcessLines(TheDbConn);

      return i;
   }

   private bool FldOK(int fldRbr)
   {
      return (fldRbr.NotZero() && fldRbr <= fields.Length);
   }

   protected string GetString(int fldRbr)
   {
      if(FldOK(fldRbr))
      {
         string text = fields[fldRbr - 1];

         text = text.Replace("\"\"", "\"");

         char[] trimThis;

         if(text.StartsWith("\"") && text.EndsWith("\""))
         {
            trimThis = new char[] { '\"' };
            text = text.TrimStart(trimThis).TrimEnd(trimThis);
         }

         trimThis = new char[] { ' ' };
         return text.TrimStart(trimThis).TrimEnd(trimThis);
      }
      else return "";
   }
   protected uint GetUint32(int fldRbr)
   {
      if(FldOK(fldRbr)) return ZXC.ValOrZero_UInt(fields[fldRbr - 1]);
      else              return 0;
   }
   protected ushort GetUint16(int fldRbr)
   {
      if(FldOK(fldRbr)) return ZXC.ValOrZero_Ushort(fields[fldRbr - 1]);
      else              return 0;
   }
   protected decimal GetDecimalOffix(int fldRbr) // 13.12.2010: Ne kuzim kaj sam htio dole ssa ovim / 100.00M a niti komenter nije jasan, Napraviti cu novu metodu za nove imporrte da ne sje*em stare
   {
      if(FldOK(fldRbr)) return ZXC.ValOrZero_Decimal(fields[fldRbr-1], 2) / 100.00M; // ovo  '* 100.00' je da te ne sjebe jel' on client decimal separator ',' ili '.' (OFFIX ONLY!!!) 
      else              return Decimal.Zero;
   }
   protected decimal GetDecimal(int fldRbr) 
   {
      //string theStr = fields[fldRbr - 1].Replace('.', ','); // ako je decimalna tocka umjesto zareza 

    //if(FldOK(fldRbr)) return ZXC.ValOrZero_Decimal(theStr            , 2);
      if(FldOK(fldRbr)) return ZXC.ValOrZero_Decimal(fields[fldRbr - 1], 2); 
      else              return Decimal.Zero;
   }
   protected decimal GetDecimalDot2Comma(int fldRbr) 
   {
      string theStr = fields[fldRbr - 1].Replace('.', ','); // ako je decimalna tocka umjesto zareza 
      //string theStr = fields[fldRbr - 1];

      if(FldOK(fldRbr)) return ZXC.ValOrZero_Decimal(theStr, 2) ; 
      else              return Decimal.Zero;
   }
   protected decimal GetDecimalComma2Dot(int fldRbr) 
   {
      string theStr = fields[fldRbr - 1].Replace(',', '.'); // ako je decimalni zarez a hoces tocku 
      //string theStr = fields[fldRbr - 1];

      if(FldOK(fldRbr)) return ZXC.ValOrZero_Decimal(theStr, 2) ; 
      else              return Decimal.Zero;
   }
   protected DateTime GetddMMyyyy(int fldRbr)
   {
      if(FldOK(fldRbr)) return ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(fields[fldRbr-1].Replace(".", "")); // ako ima tocke npr '03.01.2011. --- > 03012011'
      else              return DateTime.MinValue;
   }

   #endregion Constructor & Methodz
}



public class VvAtransImporter : VvDataRecordImporter
{
   public AtransDao.AtransCI FldRbr { get; set; }

   private List<Atrans> AtransList { get; set; }

   private bool IsNonOffix { get; set; }

   public VvAtransImporter(XSqlConnection conn, string fullPathFileName, string delimiter, bool isNonOffix) : base(conn, fullPathFileName, delimiter)
   {
      this.AtransList = new List<Atrans>();
      this.IsNonOffix = isNonOffix;
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      Atrans atrans_rec = new Atrans();

      atrans_rec.T_osredCD = GetString       (FldRbr.t_osredCD);
      atrans_rec.T_dokDate = GetddMMyyyy     (FldRbr.t_dokDate);
      atrans_rec.T_TT      = GetString       (FldRbr.t_tt);
      atrans_rec.T_opis    = GetString       (FldRbr.t_opis);
      atrans_rec.T_koefAm  = GetString       (FldRbr.t_koef_am);

      if(IsNonOffix == true) // custom for HZTK, za neke nove importe tu prekrajaj po potrebi 
      {
         atrans_rec.T_kol     = GetDecimal/*Offix*/ (FldRbr.t_kol);
         atrans_rec.T_amortSt = GetDecimal/*Offix*/ (FldRbr.t_amort_st);
         atrans_rec.T_normalAm= GetDecimal/*Offix*/ (FldRbr.t_normalAm);
         atrans_rec.T_dug     = GetDecimal/*Offix*/ (FldRbr.t_dug);
         atrans_rec.T_pot     = GetDecimal/*Offix*/ (FldRbr.t_pot);

         // ovo je trebalo SAMO za HZTK 
         //if(atrans_rec.T_TT == Amort.NABAVA_TT)
         //{
         //   atrans_rec.T_pot = GetDecimal/*Offix*/  (/*FldRbr.t_pot*/10);
         //}
         //if(atrans_rec.T_TT == Amort.AMORT_TT)
         //{
         //   atrans_rec.T_dug = 0.00M;
         //}
      }
      else // classic OFFIX 
      {
         atrans_rec.T_kol     = GetDecimalOffix (FldRbr.t_kol);
         atrans_rec.T_amortSt = GetDecimalOffix (FldRbr.t_amort_st);
         atrans_rec.T_normalAm= GetDecimalOffix (FldRbr.t_normalAm);
         atrans_rec.T_dug     = GetDecimalOffix (FldRbr.t_dug);
         atrans_rec.T_pot     = GetDecimalOffix (FldRbr.t_pot);
      }

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.AtransList.Add(atrans_rec);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {
         AmortDao.AutoSetAmort(conn, ref line,

            /* DateTime n_dokDate,  */ atrans_rec.T_dokDate,
            /* string   n_tt,       */ atrans_rec.T_TT,
            /* string   n_napomena, */ atrans_rec.T_opis,
            /* string   t_osredCD,  */ atrans_rec.T_osredCD,
            /* string   t_opis,     */ atrans_rec.T_opis,
            /* decimal  t_kol,      */ atrans_rec.T_kol,
            /* string   t_koef_am,  */ atrans_rec.T_koefAm,
            /* decimal  t_amort_st, */ atrans_rec.T_amortSt,
            /* decimal  t_normalAm, */ atrans_rec.T_normalAm,
            /* decimal  t_dug,      */ atrans_rec.T_dug,
            /* decimal  t_pot)      */ atrans_rec.T_pot);

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      var atransListSorted = AtransList.OrderBy(atrans => atrans.T_dokDate).ThenBy(atrans => atrans.T_TT);

      ushort line = 0;

      string currGroup = null;

      foreach(Atrans atrans_rec in atransListSorted)
      {
         if(currGroup != atrans_rec.T_dokDate.ToShortDateString() + atrans_rec.T_TT)
         {
            line = 0;
            currGroup = atrans_rec.T_dokDate.ToShortDateString() + atrans_rec.T_TT;
         }

         AmortDao.AutoSetAmort(conn, ref line,

            /* DateTime n_dokDate,  */ atrans_rec.T_dokDate,
            /* string   n_tt,       */ atrans_rec.T_TT,
            /* string   n_napomena, */ atrans_rec.T_opis,
            /* string   t_osredCD,  */ atrans_rec.T_osredCD,
            /* string   t_opis,     */ atrans_rec.T_opis,
            /* decimal  t_kol,      */ atrans_rec.T_kol,
            /* string   t_koef_am,  */ atrans_rec.T_koefAm,
            /* decimal  t_amort_st, */ atrans_rec.T_amortSt,
            /* decimal  t_normalAm, */ atrans_rec.T_normalAm,
            /* decimal  t_dug,      */ atrans_rec.T_dug,
            /* decimal  t_pot)      */ atrans_rec.T_pot);
      }
   }
}

public class VvFtransImporter : VvDataRecordImporter
{
   public FtransDao.FtransCI FldRbr      { get; set; }

   public int                FldRbrNalog { get; set; }

   public VvFtransImporter(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      Ftrans ftrans_rec = new Ftrans();
      string n_napomena = "";

      n_napomena            = GetString  (FldRbrNalog       );

      ftrans_rec.T_dokDate  = GetddMMyyyy(FldRbr.t_dokDate  );
      ftrans_rec.T_dokDate  = GetddMMyyyy(FldRbr.t_dokDate  );
      ftrans_rec.T_konto    = GetString  (FldRbr.t_konto    );
      ftrans_rec.T_kupdob_cd= GetUint32  (FldRbr.t_kupdob_cd);
      ftrans_rec.T_ticker   = (ftrans_rec.T_kupdob_cd.IsZero() ? "" : ZXC.KupdobDao.GetTickerForKupdobCD(conn, ftrans_rec.T_kupdob_cd, false));
      ftrans_rec.T_mtros_cd = GetUint32  (FldRbr.t_mtros_cd );
      ftrans_rec.T_mtros_tk = (ftrans_rec.T_mtros_cd .IsZero() ? "" : ZXC.KupdobDao.GetTickerForKupdobCD(conn, ftrans_rec.T_mtros_cd,  false));
      ftrans_rec.T_tipBr    = GetString  (FldRbr.t_tipBr    );
      ftrans_rec.T_opis     = GetString  (FldRbr.t_opis     ).VvTranslate437ToLatin2();
      ftrans_rec.T_valuta   = GetddMMyyyy(FldRbr.t_valuta   );
      ftrans_rec.T_TT       = GetString  (FldRbr.t_tt       );
      ftrans_rec.T_pdv      = GetString  (FldRbr.t_pdv      );
      ftrans_rec.T_037      = GetString  (FldRbr.t_037      );
      ftrans_rec.T_dug      = GetDecimalOffix (FldRbr.t_dug      );
      ftrans_rec.T_pot      = GetDecimalOffix (FldRbr.t_pot      );

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         //this.FtransList.Add(ftrans_rec);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {
         //------------------------------------------------------------------------- 
         /* */
         /* */  NalogDao.AutoSetNalog(conn, ref line,
         /* */
         /* */ /*DateTime n_dokDate    */ ftrans_rec.T_dokDate,
         /* */ /*string   n_tt         */ ftrans_rec.T_TT,
         /* */ /*string   n_napomena   */ n_napomena,
         /* */
         /* */ /*string   t_konto      */ ftrans_rec.T_konto,
         /* */ /*uint     t_kupdob_cd  */ ftrans_rec.T_kupdob_cd,
         /* */ /*string   t_ticker     */ ftrans_rec.T_ticker,
         /* */ /*uint     t_mtros_cd   */ ftrans_rec.T_mtros_cd,
         /* */ /*uint     t_mtros_tk   */ ftrans_rec.T_mtros_tk,
         /* */ /*string   t_tipBr      */ ftrans_rec.T_tipBr,
         /* */ /*string   t_opis       */ ftrans_rec.T_opis,
         /* */ /*DateTime t_valuta     */ ftrans_rec.T_valuta,
         /* */ /*string   t_pdv        */ ftrans_rec.T_pdv,
         /* */ /*string   t_037        */ ftrans_rec.T_037,
         /* */ /* string   t_projektCD */ "",
         /* */ /* ushort   t_pdvKnjiga */ ZXC.PdvKnjigaEnum.NIJEDNA,
         /* */ /* uint     t_fakRecID  */ 0,
         /* */ /* OtsKindEnum t_otsKind*/ ZXC.OtsKindEnum.NIJEDNO,
         /* */ /* string   t_fond      */ "",
         /* */ /* string   t_pozicija  */ "",
         /* */ /*decimal  t_dug        */ ftrans_rec.T_dug,
         /* */ /*decimal  t_pot        */ ftrans_rec.T_pot);
         /* */
         //------------------------------------------------------------------------- 
      }

   }

}

public class VvRtransImporter : VvDataRecordImporter
{
   #region Local Propertiz

   public enum ImportKindEnum { Offix, SensoPS, ZagriaMicroline, DucatiPS, OffixPPUK, Veridi, Likum, Tembo, Josavac, UNIVERZALE, TEXTHO, SVDUH }
   public       ImportKindEnum ImportKind  { get; set; }
   public       bool           IsZagria    { get { return ZXC.CURR_prjkt_rec.Ticker.StartsWith("ZAGRIA"); } }

   /*private*/internal List<RawTransData> RawDataList { get; set; }

   private uint   offFakturCount { get; set; }
   private uint   startDokNum    { get; set; }
   private ushort serial         { get; set; }
   private string F_flagC        { get; set; }

   private bool   IRM2IRA        { get; set; }
   private bool   WasIRM = false;

   private bool   AisUFA        { get; set; }
   private bool   IisIFA        { get; set; }

   private bool   AisURA        { get { return !AisUFA; } }
   private bool   IisIRA        { get { return !IisIFA; } }

   private Faktur faktur_rec;

   private Artikl artiklSifrar_rec;
   private Kupdob kupdobSifrar_rec;

   private ArtStat artstat_rec;
   private bool weNeedArtstat = false;

   public string forcedSkladCD;

   #endregion Local Propertiz

   internal struct RawTransData // Ovu sad strukturu krojis kako ti pase tj. kako ti importni fajl diktira... tu navodis polja iz importFajla, a dolje ih spajas sa trans-om ili faktur-om 
   {
      // trans memberz 
      internal string            t_tt       ;
      internal uint              t_ttNum    ;
      internal DateTime          t_dokDate  ;
      internal string            t_skladCD  ;
      internal string            t_artiklCD ;
      internal decimal           t_kol      ;
      internal decimal           t_cij      ;
      internal decimal           t_wanted   ;
      internal ZXC.MalopCalcKind t_mCalcKind;
      internal ZXC.PdvKolTipEnum t_pdvKolTip;
      internal uint              t_kupdobCD ;
      internal decimal           t_pdvSt    ;
      internal decimal           t_rbt1St   ;
      internal decimal           t_rbt2St   ;
      internal decimal           t_ztrUk    ;
    //internal decimal           t_doCijMal ;
      internal decimal           t_noCijMal ;
                                 
      internal string            t_externArtCD;
      internal string            t_dokumCD    ;
      internal string            t_jedMj      ;

      internal RtransResultStruct results;

   }

   public VvRtransImporter(XSqlConnection conn, string fullPathFileName, string delimiter, bool aIsUFA, bool iIsIFA, bool isForceOffix) : base(conn, fullPathFileName, delimiter)
   {
      this.AisUFA = aIsUFA;
      this.IisIFA = iIsIFA;

      this.IRM2IRA = false;
           
           if(FullPathFileName.Contains("off"))               ImportKind = ImportKindEnum.Offix     ;              
      else if(FullPathFileName.Contains("vvUni"))             ImportKind = ImportKindEnum.UNIVERZALE;              
      else if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("DUCATI")) ImportKind = ImportKindEnum.DucatiPS  ;
      else if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("SENSO"))  ImportKind = ImportKindEnum.SensoPS   ;
      else if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("PPUK"))   ImportKind = ImportKindEnum.OffixPPUK ;
      else if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("VERIDI")) ImportKind = ImportKindEnum.Veridi    ;
      else if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("TEMBO"))  ImportKind = ImportKindEnum.Tembo     ;
      else if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("JOSAV"))  ImportKind = ImportKindEnum.Josavac   ;
      else if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("LIKUM"))  ImportKind = ImportKindEnum.Likum     ;
      else if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("TEXTHO")) ImportKind = ImportKindEnum.TEXTHO    ;
      else if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("SVDUH" )) ImportKind = ImportKindEnum.SVDUH     ;
      else if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("ZAGRIA") &&
              isForceOffix == false)                          ImportKind = ImportKindEnum.ZagriaMicroline; // za Zagriu, je default ImportMicroline URA, a ako je forceOffix onda je klasika iz offix-a 
      else                                                  { ImportKind = ImportKindEnum.Offix; IRM2IRA = true; }

      if(ImportKind == ImportKindEnum.Offix || ImportKind == ImportKindEnum.SVDUH)
      {
         offFakturCount = 0;

         startDokNum = ZXC.FakturDao.GetNextDokNum(TheDbConn, Faktur.recordName);
      }
      else
      {
         this.RawDataList = new List<RawTransData>();
      }

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);
      
      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawTransData rawDatStruct = new RawTransData();

      //     if(FullPathFileName.Contains("off"))               { FillFakturRec_Offix (); return; }
      //else if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("SENSO"))    FillRawDataStruct_Senso (ref rawDatStruct);
      //else if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("ZAGRIA"))   FillRawDataStruct_Microline4Zagria(ref rawDatStruct);
      //else                                                    { FillFakturRec_Offix (); return; }

      switch(ImportKind)
      {
         case ImportKindEnum.UNIVERZALE     : FillRawDataStruct_UNIVERZALE      (ref rawDatStruct);                       break;
         case ImportKindEnum.DucatiPS       : FillRawDataStruct_DucatiPS        (ref rawDatStruct);                       break;
         case ImportKindEnum.Veridi         : FillRawDataStruct_VeridiZPC       (ref rawDatStruct); weNeedArtstat = true; break;
         case ImportKindEnum.TEXTHO         : FillRawDataStruct_TextilZPC       (ref rawDatStruct); /*weNeedArtstat = true;*/ break;
       //case ImportKindEnum.Likum          : FillRawDataStruct_LikumPSM        (ref rawDatStruct);                       break;
         case ImportKindEnum.Likum          : FillRawDataStruct_LikumKKM        (ref rawDatStruct);                       break;
         case ImportKindEnum.Tembo          : FillRawDataStruct_TemboVPS        (ref rawDatStruct);                       break;
         case ImportKindEnum.Josavac        : FillRawDataStruct_JosavacVPS      (ref rawDatStruct);                       break;
         case ImportKindEnum.SensoPS        : FillRawDataStruct_Senso           (ref rawDatStruct);                       break;
         case ImportKindEnum.ZagriaMicroline: FillRawDataStruct_Microline4Zagria(ref rawDatStruct);                       break;

         case ImportKindEnum.Offix:
         case ImportKindEnum.SVDUH:
         default                  : 
            {
               FillFakturRec_Offix(conn, rawDatStruct); return; 
            }

      }

      CalcRawResults(ref rawDatStruct);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {
      }

   }

   private void FillFakturRec_Offix(XSqlConnection conn, RawTransData rawDatStruct)
   {
      Rtrans rtrans_rec;

      ushort f_Rtip;
      string lineID, f_tip, /*f_flagC="",*/ f_skl_cd, f_tip2, f_Utip, f_Ktip, f_firma, tmpArtiklCD, tmpArtiklName/*, offRtransORG*/;
      uint f_br, f_firma_cd;
      DateTime f_date, f_dateUnDok;
      decimal f_BK_OSNOV, f_BK_PDV, f_BK_UKUPNO;
      lineID = GetString(1);

      bool dbRwtOK;
      int rwtCount = 0;

      Artikl artikl_rec;

      #region Get Faktur Line

      if(lineID == "FAK:")
      {
         // Dakle, nastupio je novi dokument pa prethodnog treba ADDREC-ati
         if(offFakturCount++ > 0)
         {
            OnLastRtrans_Action();
         }

         faktur_rec = new Faktur();

         serial = 1;

         f_tip = GetString(2); faktur_rec.TT    = GetVektorTT_ForImportTT(f_tip); faktur_rec.TtSort = faktur_rec.TtInfo.TtSort; if(f_tip == "M" && IRM2IRA) WasIRM = true; else WasIRM = false;
         f_br  = GetUint32(3); faktur_rec.TtNum = f_br                          ; faktur_rec.DokNum = startDokNum++;

         f_skl_cd = GetString(4); faktur_rec.SkladCD = (f_skl_cd == "04" && !IRM2IRA ? "MPSK" : "VPSK");

         if(ImportKind == ImportKindEnum.OffixPPUK && faktur_rec.TT == Faktur.TT_PIX) faktur_rec.SkladCD2 = faktur_rec.SkladCD;

         f_firma_cd  = GetUint32(5)  ;
         f_firma     = GetString(6)  ; faktur_rec.KupdobName = f_firma; faktur_rec.KupdobCD = f_firma_cd; SetAllKupdobFields(f_firma_cd); // ako ne nadje lokalno onda bar ostane kupdonName iz Offix-a 
         f_date      = GetddMMyyyy(7); faktur_rec.DokDate = faktur_rec.SkladDate = faktur_rec.PdvDate = f_date;
         f_dateUnDok = GetddMMyyyy(8); if(faktur_rec.TT == Faktur.TT_UFA || faktur_rec.TT == Faktur.TT_URA) faktur_rec.PdvDate = f_dateUnDok;
         f_tip2      = GetString(9)  ; if(f_tip2 == "R") faktur_rec.PdvKnjiga = ZXC.PdvKnjigaEnum.REDOVNA; else if(f_tip2 == "P") faktur_rec.PdvKnjiga = ZXC.PdvKnjigaEnum.PREDUJAM;
         f_Utip      = GetString(10) ; if(f_Utip == "R") faktur_rec.PdvKnjiga = ZXC.PdvKnjigaEnum.UVOZ_ROB; else if(f_Utip == "U") faktur_rec.PdvKnjiga = ZXC.PdvKnjigaEnum.UVOZ_USL;
         f_Ktip      = GetString(11) ;
         f_Rtip      = GetUint16(12) ; faktur_rec.PdvR12_u = f_Rtip;
         F_flagC     = GetString(13) ;

         switch(f_Ktip) // !!! faktur_rec.PdvKolTip je mrtva - puse dataLayerKolona, ali ju ovdije koristimo za punjenje rtrans_rec.T_pdvKolTip-a
         {
            // 22.11.2018: bio puse/fuse pa poceli koristiti za VvUBL_PolsProc 
          //case "M": faktur_rec.PdvKolTip = ZXC.PdvKolTipEnum.MOZE  ; break;
          //case "N": faktur_rec.PdvKolTip = ZXC.PdvKolTipEnum.NEMOZE; break;
          //case "7": faktur_rec.PdvKolTip = ZXC.PdvKolTipEnum.KOL07 ; break;
          //case "8": faktur_rec.PdvKolTip = ZXC.PdvKolTipEnum.KOL08 ; break;
          //case "9": faktur_rec.PdvKolTip = ZXC.PdvKolTipEnum.KOL09 ; break;
          //case "D": faktur_rec.PdvKolTip = ZXC.PdvKolTipEnum.KOL10 ; break;
          //case "0": faktur_rec.PdvKolTip = ZXC.PdvKolTipEnum.NIJE  ; break;
          //case "2": faktur_rec.PdvKolTip = ZXC.PdvKolTipEnum.NIJE  ; break;
            default: ZXC.aim_emsg("Nepoznata pdv kolona [{0}] na dokumentu [{1}{2}]", f_Ktip, f_tip, f_br); break;
         }

         f_tip = GetString(14); faktur_rec.V1_tt    = GetVektorTT_ForImportTT(f_tip);
         f_br  = GetUint32(15); faktur_rec.V1_ttNum = f_br;
         f_tip = GetString(16); faktur_rec.V2_tt    = GetVektorTT_ForImportTT(f_tip);
         f_br  = GetUint32(17); faktur_rec.V2_ttNum = f_br;

         faktur_rec.VezniDok = GetString(18);
       //if(ImportKind == ImportKindEnum.SVDUH && f_tip         == "I"  ) faktur_rec.VezniDok = "";
         if(ImportKind == ImportKindEnum.SVDUH && faktur_rec.TT == "IZD") faktur_rec.VezniDok = "";
         faktur_rec.Napomena = GetString(19);
         faktur_rec.Konto    = GetString(20);
         faktur_rec.DospDate = GetddMMyyyy(21);
         faktur_rec.NacPlac  = GetString(22);
         faktur_rec.DevName  = GetString(23);
         faktur_rec.S_ukKCRP = GetDecimal(24) / 100.00M;
         f_BK_OSNOV          = GetDecimal(25) / 100.00M;
         f_BK_PDV            = GetDecimal(26) / 100.00M;
         f_BK_UKUPNO         = GetDecimal(27) / 100.00M;

         //qweasd: 
         faktur_rec.PdvNum = GetUint32(29);
         faktur_rec.SkladCD = /*GetSvDuhSkladCD*/(f_skl_cd);

         if(ImportKind == ImportKindEnum.SVDUH && faktur_rec.SkladCD == "51") faktur_rec.SkladCD = "90";

         if(f_Ktip == "M" || f_Ktip == "0")
         {
            faktur_rec.S_ukOsn23m = f_BK_OSNOV;
            faktur_rec.S_ukPdv23m = f_BK_PDV;
            faktur_rec.S_ukKCRP   = f_BK_UKUPNO;

            faktur_rec.S_ukOsn23m -= faktur_rec.TrnSum_Osn23n;
            faktur_rec.S_ukPdv23m -= faktur_rec.TrnSum_Pdv23n;

            faktur_rec.S_ukOsn23n = faktur_rec.TrnSum_Osn23n;
            faktur_rec.S_ukPdv23n = faktur_rec.TrnSum_Pdv23n;
         }
         else
         {
            faktur_rec.S_ukOsn23n = f_BK_OSNOV;
            faktur_rec.S_ukPdv23n = f_BK_PDV;
            faktur_rec.S_ukKCRP   = f_BK_UKUPNO;
         }
      }

      #endregion Get Faktur Line

      #region Get Trans Line

      else
      {
         rtrans_rec = new Rtrans();

         rtrans_rec.T_serial = serial++;

         if(ImportKind == ImportKindEnum.OffixPPUK && faktur_rec.TT == Faktur.TT_PIX)
         {
            string offTransTT = GetString(1).Substring(0, 1);
            if(offTransTT == "F")
            {
               rtrans_rec.T_TT = Faktur.TT_PUX;
               rtrans_rec.T_ttSort = 25;
            }
            else if(offTransTT == "J")
            {
               rtrans_rec.T_TT = Faktur.TT_PIX;
               rtrans_rec.T_ttSort = 43;
            }
         }
         else
         {
            rtrans_rec.T_TT     = faktur_rec.TT;
            rtrans_rec.T_ttSort = faktur_rec.TtSort;
         }

         rtrans_rec.T_ttNum     = faktur_rec.TtNum;
         rtrans_rec.T_dokNum    = faktur_rec.DokNum;
         rtrans_rec.T_skladDate = faktur_rec.SkladDate;
         rtrans_rec.T_skladCD   = faktur_rec.SkladCD;
         rtrans_rec.T_kupdobCD  = faktur_rec.KupdobCD;

         #region Zagria Offix ArtiklCD manager

         if(IsZagria) // import Offix - Zagria skraceni artiklCD - '~' 
         {
            tmpArtiklCD = GetString(2);
            tmpArtiklName = GetString(3);
            artiklSifrar_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(vvDR => vvDR.ArtiklCD.ToUpper() == tmpArtiklCD.ToUpper());
            if(artiklSifrar_rec != null) // mejbi vi ar laki 
            {
               rtrans_rec.T_artiklCD = artiklSifrar_rec.ArtiklCD;
               rtrans_rec.T_artiklName = artiklSifrar_rec.ArtiklName;
            }
            else // lec traj po nazivu 
            {
               artiklSifrar_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(vvDR => vvDR.ArtiklName.ToUpper() == tmpArtiklName.ToUpper());
               if(artiklSifrar_rec != null)
               {
                  rtrans_rec.T_artiklCD = artiklSifrar_rec.ArtiklCD;
                  rtrans_rec.T_artiklName = artiklSifrar_rec.ArtiklName;
               }
               else // lec traj po 12znamnkastom korjenu sifre
               {
                  string artiklCD_12;

                  if(tmpArtiklCD.Length >= 13 && tmpArtiklCD[12] == '~')
                  {
                     artiklCD_12 = tmpArtiklCD.Substring(0, 12);
                     artiklCD_12 = artiklCD_12.Trim();
                  }
                  else
                  {
                     artiklCD_12 = tmpArtiklCD.Substring(0, tmpArtiklCD.Length);
                  }

                  artiklSifrar_rec = VvUserControl.ArtiklSifrar.FirstOrDefault(vvDR => vvDR.ArtiklCD.ToUpper().StartsWith(artiklCD_12.ToUpper()));

                  if(artiklSifrar_rec != null)
                  {
                     rtrans_rec.T_artiklCD = artiklSifrar_rec.ArtiklCD;
                     rtrans_rec.T_artiklName = artiklSifrar_rec.ArtiklName;
                  }
                  else // kara, ostavi kak' je 
                  {
                     #region ADDREC Artikl from IMPORT bikoz is nepostojeci u VEKTOR-u

                     artiklSifrar_rec = new Artikl();

                     artiklSifrar_rec.ArtiklCD = tmpArtiklCD;
                     artiklSifrar_rec.ArtiklName = tmpArtiklName;
                     artiklSifrar_rec.TS = "XY";
                     artiklSifrar_rec.SkladCD = "VPSK";
                     artiklSifrar_rec.PdvKat = "23";

                     bool OK = artiklSifrar_rec.VvDao.ADDREC(conn, artiklSifrar_rec, false, false, false, true);

                     if(OK) ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

                     #endregion ADDREC Artikl from IMPORT bikoz is nepostojeci u VEKTOR-u

                     rtrans_rec.T_artiklCD = tmpArtiklCD;
                     rtrans_rec.T_artiklName = tmpArtiklName;
                  }
               }
            }
         }

         #endregion Zagria Offix ArtiklCD manager

         else // normal, classic Offix import
         {
            rtrans_rec.T_artiklCD   = GetString(2);
            rtrans_rec.T_artiklName = GetString(3);
         }
         rtrans_rec.T_jedMj  = GetString(4);
         rtrans_rec.T_konto  = GetString(5);
         // 01.02.2013: ppuk 
         //rtrans_rec.T_kol  = GetDecimal( 6) / 100.00M;
         rtrans_rec.T_kol    = GetDecimal( 6) / 1000.00M;
         // 22.03.2018: za svduh 
       //rtrans_rec.T_cij    = GetDecimal( 7) /   10000.00M;
         rtrans_rec.T_cij    = GetDecimal( 7) / 1000000.00M;
         rtrans_rec.T_pdvSt  = GetDecimal( 8) / 100.00M;
         rtrans_rec.T_rbt1St = GetDecimal( 9) / 100.00M;
         rtrans_rec.T_rbt2St = GetDecimal(10) / 100.00M;
         rtrans_rec.T_wanted = GetDecimal(11) / 10000.00M;
         rtrans_rec.T_ztr    = GetDecimal(12) / 100.00M;
         rtrans_rec.T_kol2   = GetDecimal(13) / 100.00M; // ppuk t_zDim - debljina 

         if(rtrans_rec.T_artiklCD.ToLower().StartsWith("qweno")) rtrans_rec.T_pdvColTip = ZXC.PdvKolTipEnum.NEMOZE;
         else if(rtrans_rec.T_pdvSt.IsZero() && F_flagC == "X")  rtrans_rec.T_pdvColTip = ZXC.PdvKolTipEnum.PROLAZ;
         else                                                    rtrans_rec.T_pdvColTip = (ZXC.PdvKolTipEnum)faktur_rec.PdvKolTip; // pazi; od 22.11.2018 PdvKolTip bio puse/fuse pa poceli koristiti za VvUBL_PolsProc ... dakle ovo je vise netocno 

         if(rtrans_rec.T_artiklCD.ToLower().StartsWith("qwe")   == true &&
            rtrans_rec.T_artiklCD.ToLower().StartsWith("qweno") == false)
         {
            rtrans_rec.T_artiklCD = "";
         }

         if(IRM2IRA && WasIRM) // znaci ucitavamo Offix-ov 'M' da zavrsi kao Vektor-ova 'IRA'. Prvo treba MALOP Calc a onda CLASSIC Calc 
         {
            rtrans_rec.T_TT     = Faktur.TT_IRM; // temporary, da digne MALOP Calc 
            rtrans_rec.CalcTransResults(null);
            rtrans_rec.T_rbt1St = rtrans_rec.T_rbt2St = 0.00M;
            rtrans_rec.T_cij    = rtrans_rec.R_KCRdivKOL;
            rtrans_rec.T_TT     = Faktur.TT_IRA;
         }

         if(ImportKind    == ImportKindEnum.SVDUH &&
            (faktur_rec.TT == Faktur.TT_URA || faktur_rec.TT == Faktur.TT_NRD))
         {
            rtrans_rec.T_doCijMal = GetDecimal(14); // rtrans_rec.T_doCijMal is ORG 
            // sad tu nadi artikl u sifrarniku i vidi jel ORG jednak a ako nije digni buku 
            artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == rtrans_rec.T_artiklCD);
            //decimal artiklOrgPak = ZXC.ValOrZero_Decimal(artikl_rec.OrgPak, 0);

            if(faktur_rec.TT == Faktur.TT_URA && artikl_rec != null && /*artiklOrgPak*/ZXC.ValOrZero_Decimal(artikl_rec.OrgPak, 0) != rtrans_rec.T_doCijMal)
            {

               ZXC.aim_log("URA-{0}:{1} art[{2}][{3}] vvORG[{4}] offORG[{5}]",
                  rtrans_rec.T_ttNum,
                  rtrans_rec.T_serial,
                  rtrans_rec.T_artiklCD,
                  rtrans_rec.T_artiklName,
                  artikl_rec.OrgPak,
                  rtrans_rec.T_doCijMal
                  );

               artikl_rec.BeginEdit();
               artikl_rec.OrgPak = rtrans_rec.T_doCijMal.ToString0Vv();
               if(artikl_rec.EditedHasChanges())
               {
                  dbRwtOK = artikl_rec.VvDao.RWTREC(conn, artikl_rec, false, false);
                  if(dbRwtOK) rwtCount++;
               }
               else
               {
                  dbRwtOK = true;
               }
               artikl_rec.EndEdit();

            } //if(artikl_rec != null && artikl_rec.OrgPak != tmpORG) 
         }

         rtrans_rec.CalcTransResults(null);

         faktur_rec.Transes.Add(rtrans_rec);
      }

      #endregion Get Trans Line

   }

   public void OnLastRtrans_Action()
   {
      if(ImportKind == ImportKindEnum.OffixPPUK && faktur_rec.TT == Faktur.TT_PIX)
      {
         SynthesizeRtrans(faktur_rec);
      }

      faktur_rec.TakeTransesSumToDokumentSum(true);

      faktur_rec.VvDao.ADDREC(TheDbConn, faktur_rec);
   }

   private void SynthesizeRtrans(Faktur faktur_rec)
   {
      List<Rtrans> origRtranses = faktur_rec.CloneTranses().ConvertAll(trans => trans as Rtrans);

      faktur_rec.Transes = origRtranses
         .GroupBy(rtr => rtr.T_artiklCD)
         .Select(grp => new Rtrans() 
            { 
               T_artiklCD   = grp.Key,
               T_artiklName = grp.First().T_artiklName,
               T_skladDate  = grp.First().T_skladDate, 
               T_skladCD    = grp.First().T_skladCD, 
               T_dokNum     = grp.First().T_dokNum, 
               T_kol        = grp.Sum(r => r.T_kol),
               T_cij        = grp.First().T_cij,
               T_jedMj      = grp.First().T_jedMj,
               T_kupdobCD   = grp.First().T_kupdobCD,
               T_pdvSt      = grp.First().T_pdvSt,
               T_serial     = grp.First().T_serial,
               T_TT         = grp.First().T_TT,
               T_ttSort     = grp.First().T_ttSort,
               T_ttNum      = grp.First().T_ttNum,
            })
         .OrderBy(grp => grp.T_artiklName, new VvCompareStringsOrdinal())
         .ToList();

      ushort serial = 0;
      foreach(Rtrans rtrans_rec in origRtranses)
      {
         faktur_rec.Transes2.Add(new Rtrano() 
         { 
            T_artiklCD   = rtrans_rec.T_artiklCD, 
            T_artiklName = rtrans_rec.T_artiklName,
            T_dokNum     = rtrans_rec.T_dokNum,
            T_kol        = rtrans_rec.T_kol,
            T_kupdobCD   = rtrans_rec.T_kupdobCD,
            T_serial     = ++serial,
            T_skladCD    = rtrans_rec.T_skladCD,
            T_skladDate  = rtrans_rec.T_skladDate,
            T_TT         = rtrans_rec.T_TT,
            T_ttNum      = rtrans_rec.T_ttNum,
            T_ttSort     = rtrans_rec.T_ttSort,
            T_dimZ       = rtrans_rec.T_kol2
         });
      }
   }

   private void SetAllKupdobFields(uint f_firma_cd)
   {
      kupdobSifrar_rec = VvUserControl.KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD == f_firma_cd);

      if(kupdobSifrar_rec == null) return;

      faktur_rec.KupdobCD   = kupdobSifrar_rec.KupdobCD;
      faktur_rec.KupdobName = kupdobSifrar_rec.Naziv;
      faktur_rec.KupdobTK   = kupdobSifrar_rec.Ticker;
      faktur_rec.KdOib      = kupdobSifrar_rec.Oib;
      faktur_rec.KdUlica    = kupdobSifrar_rec.Ulica2;
      faktur_rec.KdZip      = kupdobSifrar_rec.PostaBr;
      faktur_rec.KdMjesto   = kupdobSifrar_rec.Grad;
   }

   private string GetVektorTT_ForImportTT(string f_tip)
   {
      string theTT;

      switch(f_tip) // ovo treba tweak-ati za svaki import 
      {
         case "A"  : theTT = AisUFA ? Faktur.TT_UFA : 
                                      Faktur.TT_URA ; break;
         case "I"  : theTT = IisIFA ? Faktur.TT_IFA : 
                                      ZXC.IsSvDUH ? Faktur.TT_IZD : Faktur.TT_IRA ; break;

         case "D"  : theTT = Faktur.TT_PST   ; break;
         case "S"  : theTT = Faktur.TT_NRD   ; break;
         case "P"  : theTT = Faktur.TT_PSM   ; break;
         case "C"  : theTT = Faktur.TT_PRI   ; break;
         case "R"  : theTT = Faktur.TT_IZD   ; break; 
         case "M"  : if(IRM2IRA)
                     theTT = Faktur.TT_IRA;
                     else 
                     theTT = Faktur.TT_IRM   ; break;
         case "CV1": theTT = Faktur.TT_CJ_VP1; break;
         case "CV2": theTT = Faktur.TT_CJ_VP2; break;
         case "X"  : theTT = Faktur.TT_PIX   ; break;
         default   : theTT = f_tip           ; break;
      }

      return theTT;
   }

   private void FillRawDataStruct_Microline4Zagria(ref RawTransData rawDatStruct)
   {
      rawDatStruct.t_tt       = GetString  ( 1);
    //rawDatStruct.ttNum    = GetUint32  ( 2);
      rawDatStruct.t_dokumCD  = GetString  ( 2);
      rawDatStruct.t_dokDate  = GetddMMyyyy( 3);
      rawDatStruct.t_skladCD  = GetString  ( 4);
      rawDatStruct.t_artiklCD = GetString  ( 5);
      rawDatStruct.t_kol      = GetDecimal ( 6);
      rawDatStruct.t_cij      = GetDecimal ( 7);

      rawDatStruct.t_kupdobCD = GetUint32  ( 8);
      rawDatStruct.t_pdvSt    = GetDecimal ( 9);
      rawDatStruct.t_rbt1St   = GetDecimal (10);
    //rawDatStruct.rbt2St   = GetDecimal (11) / 100.00M;
    //rawDatStruct.ztrUk    = GetDecimal (12) / 100.00M;

   }

   private void FillRawDataStruct_Senso(ref RawTransData rawDatStruct)
   {
      rawDatStruct.t_externArtCD = GetString  ( 1);
      rawDatStruct.t_kol         = GetDecimal ( 3);
      rawDatStruct.t_cij         = GetDecimal ( 4);

      rawDatStruct.t_tt      = "D";
      rawDatStruct.t_dokDate = ZXC.projectYearFirstDay;

      string externArtCD = rawDatStruct.t_externArtCD;
      artiklSifrar_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(vvDR => vvDR.ArtiklCD2 == externArtCD); // po staroj Pantelijinoj sifri trazimo 
      if(artiklSifrar_rec != null && externArtCD.NotEmpty()) rawDatStruct.t_artiklCD = artiklSifrar_rec.ArtiklCD;
      else                                                   rawDatStruct.t_artiklCD = "XY-" + externArtCD;
   }

   private void FillRawDataStruct_TemboVPS(ref RawTransData rawDatStruct)
   {
      rawDatStruct.t_externArtCD = GetString  ( 1);
      rawDatStruct.t_kol         = GetDecimal ( 4);
      rawDatStruct.t_cij         = GetDecimal ( 5);

      rawDatStruct.t_tt      = "D";
      rawDatStruct.t_dokDate = ZXC.projectYearFirstDay;

      string externArtCD = rawDatStruct.t_externArtCD;
      artiklSifrar_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == externArtCD); // po staroj IPOS-OVOJ 
      if(artiklSifrar_rec != null && externArtCD.NotEmpty()) rawDatStruct.t_artiklCD = artiklSifrar_rec.ArtiklCD;
      else                                                   rawDatStruct.t_artiklCD = "XY-" + externArtCD;
   }

   private void FillRawDataStruct_JosavacVPS(ref RawTransData rawDatStruct)
   {
      rawDatStruct.t_externArtCD = GetString ( 1);
      rawDatStruct.t_kol         = GetDecimal( 3);
      rawDatStruct.t_cij         = GetDecimal( 4);
      rawDatStruct.t_skladCD     = GetString ( 5);

      rawDatStruct.t_tt      = "D";
      rawDatStruct.t_dokDate = ZXC.projectYearFirstDay;

      string externArtCD = rawDatStruct.t_externArtCD;
      artiklSifrar_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == externArtCD); 
      if(artiklSifrar_rec != null && externArtCD.NotEmpty()) rawDatStruct.t_artiklCD = artiklSifrar_rec.ArtiklCD;
      else                                                   rawDatStruct.t_artiklCD = "XY-" + externArtCD;
   }

   private void FillRawDataStruct_UNIVERZALE(ref RawTransData rawDatStruct)
   {
      rawDatStruct.t_externArtCD = GetString ( 1);
      rawDatStruct.t_kol         = GetDecimal( 3);
      rawDatStruct.t_cij         = GetDecimal( 4);
      rawDatStruct.t_skladCD     = GetString ( 5);
      rawDatStruct.t_tt          = GetString ( 6); // npr 'D' ili 'PST' ili 'CV1', itd 

      rawDatStruct.t_dokDate = ZXC.projectYearFirstDay;

      string externArtCD = rawDatStruct.t_externArtCD;
      artiklSifrar_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == externArtCD); 
      if(artiklSifrar_rec != null && externArtCD.NotEmpty()) rawDatStruct.t_artiklCD = artiklSifrar_rec.ArtiklCD;
      else                                                   rawDatStruct.t_artiklCD = "XY-" + externArtCD;
   }

   private void FillRawDataStruct_DucatiPS(ref RawTransData rawDatStruct)
   {
      // A -  1 
      // B -  2 
      // C -  3 
      // D -  4 
      // E -  5 
      // F -  6 
      // G -  7 
      // H -  8 
      // I -  9 
      // J - 10 
      // K - 11 
      // L - 12 
      // M - 13 
      // N - 14 
      // O - 15 
      // P - 16 
      // Q - 17 

      rawDatStruct.t_externArtCD = GetString  ( 1); // A 
      rawDatStruct.t_kol         = GetDecimal (11); // K 
      rawDatStruct.t_cij         = GetDecimal (13); // M 
      rawDatStruct.t_wanted      = GetDecimal (15); // O 

      rawDatStruct.t_tt      = "P";
      rawDatStruct.t_dokDate = ZXC.projectYearFirstDay;

      string externArtCD = rawDatStruct.t_externArtCD;
      artiklSifrar_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(vvDR => vvDR.ArtiklCD2 == externArtCD); // po staroj Pantelijinoj sifri trazimo 
      if(artiklSifrar_rec != null && externArtCD.NotEmpty()) rawDatStruct.t_artiklCD = artiklSifrar_rec.ArtiklCD;
      else                                                   rawDatStruct.t_artiklCD = "XY-" + externArtCD;

      rawDatStruct.t_pdvSt     = 25.00M;
      rawDatStruct.t_skladCD   = "MPSK";
      rawDatStruct.t_mCalcKind = ZXC.MalopCalcKind.By_VPC; 
   }

   private void FillRawDataStruct_VeridiZPC(ref RawTransData rawDatStruct)
   {
      // A -  1 
      // B -  2 
      // C -  3 
      // D -  4 
      // E -  5 
      // F -  6 
      // G -  7 
      // H -  8 
      // I -  9 
      // J - 10 
      // K - 11 
      // L - 12 
      // M - 13 
      // N - 14 
      // O - 15 
      // P - 16 
      // Q - 17 

      rawDatStruct.t_artiklCD = GetString  ( 2); // B 
    //rawDatStruct.t_cij      = GetDecimal ( 5); // E 
      rawDatStruct.t_noCijMal = 
      rawDatStruct.t_wanted   = GetDecimal ( 5); // E 

    //rawDatStruct.t_tt      = "CV1";
      rawDatStruct.t_tt      = "ZPC";
      rawDatStruct.t_dokDate = ZXC.projectYearFirstDay;

      //string externArtCD = rawDatStruct.t_externArtCD;
      //artiklSifrar_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(vvDR => vvDR.ArtiklCD2 == externArtCD); // po staroj Pantelijinoj sifri trazimo 
      //if(artiklSifrar_rec != null && externArtCD.NotEmpty()) rawDatStruct.t_artiklCD = artiklSifrar_rec.ArtiklCD;
      //else                                                   rawDatStruct.t_artiklCD = "XY-" + externArtCD;

      rawDatStruct.t_pdvSt   = 25.00M;
    //rawDatStruct.t_skladCD = "SVPS";
      rawDatStruct.t_skladCD = "MPSK";
      rawDatStruct.t_mCalcKind = ZXC.MalopCalcKind.By_MPC; 
   }

   private void FillRawDataStruct_TextilZPC(ref RawTransData rawDatStruct)
   {
      // A -  1 
      // B -  2 
      // C -  3 
      // D -  4 
      // E -  5 
      // F -  6 
      // G -  7 
      // H -  8 
      // I -  9 
      // J - 10 
      // K - 11 
      // L - 12 
      // M - 13 
      // N - 14 
      // O - 15 
      // P - 16 
      // Q - 17 

      rawDatStruct.t_artiklCD = GetString  ( 1); // A 
      rawDatStruct.t_noCijMal = 
    //rawDatStruct.t_wanted   = GetDecimal ( 2); // B 
      rawDatStruct.t_wanted   = Artikl.GetTH_MPCfromArtiklCD (rawDatStruct.t_artiklCD); // B 

    //rawDatStruct.t_tt      = "CV1";
      rawDatStruct.t_tt      = "ZPC";
      rawDatStruct.t_dokDate = ZXC.projectYearFirstDay;

      //string externArtCD = rawDatStruct.t_externArtCD;
      //artiklSifrar_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(vvDR => vvDR.ArtiklCD2 == externArtCD); // po staroj Pantelijinoj sifri trazimo 
      //if(artiklSifrar_rec != null && externArtCD.NotEmpty()) rawDatStruct.t_artiklCD = artiklSifrar_rec.ArtiklCD;
      //else                                                   rawDatStruct.t_artiklCD = "XY-" + externArtCD;

      rawDatStruct.t_pdvSt     = 25.00M;
      rawDatStruct.t_mCalcKind = ZXC.MalopCalcKind.By_MPC; 
    //rawDatStruct.t_skladCD   = "SVPS";
    //rawDatStruct.t_skladCD   = "98M5";
      rawDatStruct.t_skladCD   = forcedSkladCD;
      rawDatStruct.t_ttNum     = ZXC.ValOrZero_UInt(forcedSkladCD.SubstringSafe(0, 2)) * 10000;
   }

   private void FillRawDataStruct_LikumPSM(ref RawTransData rawDatStruct)
   {
      // A -  1 
      // B -  2 
      // C -  3 
      // D -  4 
      // E -  5 
      // F -  6 
      // G -  7 
      // H -  8 
      // I -  9 
      // J - 10 
      // K - 11 
      // L - 12 
      // M - 13 
      // N - 14 
      // O - 15 
      // P - 16 
      // Q - 17 

      rawDatStruct.t_artiklCD = GetString  ( 1); // A 
      rawDatStruct.t_cij      = GetDecimal ( 2); // B 
      rawDatStruct.t_wanted   = GetDecimal ( 3); // C 

      rawDatStruct.t_tt      = "PSM";
      rawDatStruct.t_dokDate = /*ZXC.projectYearFirstDay*/new DateTime(2013, 09, 20);

      //string externArtCD = rawDatStruct.t_externArtCD;
      //artiklSifrar_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(vvDR => vvDR.ArtiklCD2 == externArtCD); // po staroj Pantelijinoj sifri trazimo 
      //if(artiklSifrar_rec != null && externArtCD.NotEmpty()) rawDatStruct.t_artiklCD = artiklSifrar_rec.ArtiklCD;
      //else                                                   rawDatStruct.t_artiklCD = "XY-" + externArtCD;

      //rawDatStruct.t_pdvSt     = 25.00M;
      rawDatStruct.t_skladCD   = "KARAS";
      rawDatStruct.t_mCalcKind = ZXC.MalopCalcKind.By_MPC; 
   }

   private void FillRawDataStruct_LikumKKM(ref RawTransData rawDatStruct)
   {
      // A -  1 
      // B -  2 
      // C -  3 
      // D -  4 
      // E -  5 
      // F -  6 
      // G -  7 
      // H -  8 
      // I -  9 
      // J - 10 
      // K - 11 
      // L - 12 
      // M - 13 
      // N - 14 
      // O - 15 
      // P - 16 
      // Q - 17 

      rawDatStruct.t_artiklCD    = GetString  ( 1); // A 
      rawDatStruct.t_externArtCD = GetString  ( 5); // E - AUTOR NAZIV 
      rawDatStruct.t_kol         = GetDecimal ( 7); // G 
      rawDatStruct.t_cij         = GetDecimal ( 9); // I 
      rawDatStruct.t_wanted      = GetDecimal (10); // J 
      rawDatStruct.t_dokumCD     = GetString  (11); // K - LK or NF 

      rawDatStruct.t_artiklCD    = rawDatStruct.t_dokumCD + rawDatStruct.t_artiklCD.Replace("0000", "");

      string tmpKupdobName = ZXC.VvTranslate437ToLatin2(rawDatStruct.t_externArtCD);
      kupdobSifrar_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Naziv == tmpKupdobName);
      rawDatStruct.t_kupdobCD = kupdobSifrar_rec.KupdobCD;

      rawDatStruct.t_tt        = "KKM";
      rawDatStruct.t_pdvKolTip = ZXC.PdvKolTipEnum.UMJETN;
      rawDatStruct.t_dokDate   = /*ZXC.projectYearFirstDay*/new DateTime(2013, 11, 04);

      //string externArtCD = rawDatStruct.t_externArtCD;
      //artiklSifrar_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(vvDR => vvDR.ArtiklCD2 == externArtCD); // po staroj Pantelijinoj sifri trazimo 
      //if(artiklSifrar_rec != null && externArtCD.NotEmpty()) rawDatStruct.t_artiklCD = artiklSifrar_rec.ArtiklCD;
      //else                                                   rawDatStruct.t_artiklCD = "XY-" + externArtCD;

      rawDatStruct.t_pdvSt     = 25.00M;
      rawDatStruct.t_skladCD   = "LIKUM";
      rawDatStruct.t_jedMj     = "KOM";
      rawDatStruct.t_mCalcKind = ZXC.MalopCalcKind.By_MPC; 
   }

   public static string Limited_Rtrans(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.RtransDao.GetSchemaColumnSize(cIdx));
   }
   public static string Limited_Faktur(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.FakturDao.GetSchemaColumnSize(cIdx));
   }
   public static string Limited_FaktEx(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.FaktExDao.GetSchemaColumnSize(cIdx));
   }
   public static string Limited_Ftrans(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.FtransDao.GetSchemaColumnSize(cIdx));
   }

   private string GetDocumentGroupID(RawTransData rawData)
   {
      if(ImportKind == ImportKindEnum.Likum/*KKM*/)
      {
         return rawData.t_externArtCD + rawData.t_skladCD + rawData.t_dokDate.ToShortDateString() + rawData.t_tt + rawData.t_ttNum;
      }
      else
      {
         return                         rawData.t_skladCD + rawData.t_dokDate.ToShortDateString() + rawData.t_tt + rawData.t_ttNum;
      }
   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      if(ImportKind == ImportKindEnum.Offix || ImportKind == ImportKindEnum.SVDUH) return;

      var rawDataListSorted = RawDataList.OrderBy(raw => raw.t_skladCD).ThenBy(raw => raw.t_dokDate).ThenBy(raw => raw.t_tt).ThenBy(raw => raw.t_ttNum);

      //RawDataList.ForEach(rawData => CalcRawResults(ref rawData));

      ushort line = 0;
      string currGroupID = null;

      string  artiklName="", theTT="";
      string      f_kupdobName = "";
      string      f_kupdobTK   = "";
      string      f_kdOib      = "";
      string      f_kdUlica    = "";
      string      f_kdMjesto   = "";
      string      f_kdZip      = "";
      string      f_ZiroRn     = "";
      string      f_Konto      = "";
ZXC.PdvKnjigaEnum f_PdvKnjiga  = ZXC.PdvKnjigaEnum.NIJEDNA;
decimal s_ukK = 0M, s_ukKC = 0M, s_ukKCR = 0M, s_ukKCRM = 0M, s_ukKCRP = 0M, s_ukZavisni = 0M, s_ukRbt1 = 0M, s_ukOsn25m = 0M, s_ukPdv = 0M, s_ukPdv25m = 0M, s_ukMSK = 0M, s_ukMSK_25 = 0M, s_ukMSKpdv = 0M, s_ukMSKpdv_25 = 0M, s_ukMrz = 0M;
decimal stanjeKol, prevMalopCij = 0M;
      int     s_trnCount = 0;
      
      foreach(RawTransData rawData in rawDataListSorted)
      {
         if(currGroupID != GetDocumentGroupID(rawData)) // Novi dokument (zaglavlje) 
         {
            line = 0;
            currGroupID  = GetDocumentGroupID(rawData);

            s_ukK       = RawDataList.Where(raw1 => GetDocumentGroupID(raw1) == currGroupID).Sum(raw2 =>  raw2.t_kol           );
            s_ukKC      = RawDataList.Where(raw1 => GetDocumentGroupID(raw1) == currGroupID).Sum(raw2 => (raw2.results._r_KC  ));
            s_ukKCR     = RawDataList.Where(raw1 => GetDocumentGroupID(raw1) == currGroupID).Sum(raw2 => (raw2.results._r_KCR ));
            s_ukKCRM    = RawDataList.Where(raw1 => GetDocumentGroupID(raw1) == currGroupID).Sum(raw2 => (raw2.results._r_KCRM));
            s_ukKCRP    = RawDataList.Where(raw1 => GetDocumentGroupID(raw1) == currGroupID).Sum(raw2 => (raw2.results._r_KCRP));
          //s_ukZavisni = RawDataList.Where(raw1 => GetDocumentGroupID(raw1) == currGroupID).Sum(raw2 => (raw2.results._r_ztr ));
            s_ukZavisni = RawDataList.Where(raw1 => GetDocumentGroupID(raw1) == currGroupID).Sum(raw2 => (raw2.t_ztrUk        ));

            s_ukRbt1    = RawDataList.Where(raw1 => GetDocumentGroupID(raw1) == currGroupID).Sum(raw2 => (raw2.results._r_rbt1));
            s_ukOsn25m  = s_ukKCR;                                                                                            
            s_ukPdv     = RawDataList.Where(raw1 => GetDocumentGroupID(raw1) == currGroupID).Sum(raw2 => (raw2.results._r_pdv ));
            s_ukPdv25m  = s_ukPdv;

            s_ukMrz       = RawDataList.Where(raw1 => GetDocumentGroupID(raw1) == currGroupID).Sum(raw2 => (raw2.results._r_mrz));
            s_ukMSKpdv    = RawDataList.Where(raw1 => GetDocumentGroupID(raw1) == currGroupID).Sum(raw2 => (raw2.results._r_mskPdv));
            s_ukMSKpdv_25 = s_ukMSKpdv;
            s_ukMSK       = RawDataList.Where(raw1 => GetDocumentGroupID(raw1) == currGroupID).Sum(raw2 => (raw2.results._r_MSK));
            s_ukMSK_25    = s_ukMSK;

            s_trnCount  = RawDataList.Count(raw => GetDocumentGroupID(raw) == currGroupID);

         }

         artiklSifrar_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(vvDR => vvDR.ArtiklCD == rawData.t_artiklCD);
         kupdobSifrar_rec = VvUserControl.KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD == rawData.t_kupdobCD);

         if(artiklSifrar_rec != null && rawData.t_artiklCD.NotEmpty()){ artiklName = artiklSifrar_rec.ArtiklName;                                }
         else                                                         { artiklName = "---!!!--- " + rawData.t_artiklCD; /*rawData.artiklCD = "";*/ }

         // 18.01.2014: Za veridian ZPC import
/* !!! */stanjeKol = rawData.t_kol;
         if(weNeedArtstat && artiklSifrar_rec != null && rawData.t_artiklCD.NotEmpty()) 
         { 
            artstat_rec = ArtiklDao.GetArtiklStatus(conn, rawData.t_artiklCD, rawData.t_skladCD);
            if(artstat_rec != null)
            {
               stanjeKol    = artstat_rec.StanjeKol       ;
               prevMalopCij = artstat_rec./*Prev*/MalopCij;
            }
         }

         if(kupdobSifrar_rec != null && rawData.t_kupdobCD.NotZero()) 
         { 
            f_kupdobName = kupdobSifrar_rec.Naziv   ; 
            f_kupdobTK   = kupdobSifrar_rec.Ticker  ; 
            f_kdOib      = kupdobSifrar_rec.Oib     ;
            f_kdUlica    = kupdobSifrar_rec.Ulica2  ;
            f_kdMjesto   = kupdobSifrar_rec.Grad    ;
            f_kdZip      = kupdobSifrar_rec.PostaBr ;
            f_ZiroRn     = kupdobSifrar_rec.Ziro1   ;
            f_Konto      = kupdobSifrar_rec.KontoDug;
            f_PdvKnjiga  = ZXC.PdvKnjigaEnum.REDOVNA;
         }
         else                                                         
         { 
            f_kupdobName = rawData.t_kupdobCD.ToString(); 
            f_kupdobTK   = "";
            f_kdOib      = "";
            f_kdUlica    = "";
            f_kdMjesto   = "";
            f_kdZip      = "";
            f_ZiroRn     = "";
            f_Konto      = "";
            f_PdvKnjiga  = ZXC.PdvKnjigaEnum.REDOVNA;
         }

         theTT = GetVektorTT_ForImportTT(rawData.t_tt);

         FakturDao.AutoSetFaktur(conn, ref line,

            /* string   f_tt          ,*/ Limited_Rtrans(     theTT        , ZXC.RtrCI.t_tt      ),
            /* string   f_ttNum       ,*/               (rawData.t_ttNum                         ),
            /* DateTime f_dokDate     ,*/               (rawData.t_dokDate                       ),
            /* string   f_skladCD     ,*/ Limited_Rtrans(rawData.t_skladCD , ZXC.RtrCI.t_skladCD ),
            /* string   f_vezniDok    ,*/ Limited_Faktur(rawData.t_dokumCD , ZXC.FakCI.vezniDok  ),
            /* string   f_projektCD   ,*/ Limited_Faktur(     ""           , ZXC.FakCI.projektCD ),
            /* string   f_napomena    ,*/ Limited_Faktur(     ""           , ZXC.FakCI.napomena  ),
            /* decimal  s_ukZavisni   ,*/               (s_ukZavisni                             ),
            /* decimal  s_ukKCRP      ,*/               (s_ukKCRP                                ),
            /* decimal  s_ukKCRM      ,*/               (     s_ukKCRM                           ),
            /* decimal  s_ukKCR       ,*/               (     s_ukKCR                            ),
            /* decimal  s_ukKC        ,*/               (     s_ukKC                             ),
            /* decimal  s_ukK         ,*/               (     s_ukK                              ),
            /* decimal  s_ukRbt1      ,*/               (s_ukRbt1                                ),
            /* decimal  s_ukOsn23m    ,*/               (s_ukOsn25m                              ),
            /* decimal  s_ukPdv       ,*/               (s_ukPdv                                 ),
            /* decimal  s_ukPdv25m    ,*/               (s_ukPdv25m                              ),
            /* decimal  s_ukMrz       ,*/               (s_ukMrz                                 ),
            /* decimal  s_ukMSKpdv    ,*/             //(s_ukMSKpdv                              ),
            /* decimal  s_ukMSKpdv_25 ,*/               (s_ukMSKpdv_25                           ),
            /* decimal  s_ukMSK       ,*/             //(s_ukMSK                                 ),
            /* decimal  s_ukMSK_25    ,*/               (s_ukMSK_25                              ),
            /* uint     s_trnCount    ,*/               (     (uint)s_trnCount                   ),
            /* string   f_kupdobCD    ,*/               (rawData.t_kupdobCD                      ),
            /* string   f_kupdobName  ,*/ Limited_FaktEx(f_kupdobName, ZXC.FexCI.kupdobName      ),
            /* string   f_kupdobTicker,*/ Limited_FaktEx(f_kupdobTK  , ZXC.FexCI.kupdobTK        ),
    /*         string   f_kdOib       ,*/ Limited_FaktEx(f_kdOib     , ZXC.FexCI.kdOib           ),
    /*         string   f_kdUlica     ,*/ Limited_FaktEx(f_kdUlica   , ZXC.FexCI.kdUlica         ),
    /*         string   f_kdMjesto    ,*/ Limited_FaktEx(f_kdMjesto  , ZXC.FexCI.kdMjesto        ),
    /*         string   f_kdZip       ,*/ Limited_FaktEx(f_kdZip     , ZXC.FexCI.kdZip           ),
    /*         string   f_ZiroRn      ,*/ Limited_FaktEx(f_ZiroRn    , ZXC.FexCI.ziroRn          ),
    /*         string   f_Konto       ,*/ Limited_FaktEx(f_Konto     , ZXC.FakCI.konto           ),
    /*ZXC.PdvKnjigaEnum f_PdvKnjiga   ,*/                  f_PdvKnjiga                            ,
            /* string   f_rokPlac     ,*/                                                       30,
            /* string   f_dospDate    ,*/            rawData.t_dokDate + new TimeSpan(30, 0, 0, 0),
            /* string   t_artiklCD    ,*/ Limited_Rtrans(rawData.t_artiklCD, ZXC.RtrCI.t_artiklCD),
            /* string   t_artiklName  ,*/ Limited_Rtrans(artiklName      , ZXC.RtrCI.t_artiklName),
            /* decimal  t_kol         ,*/               (     stanjeKol                          ),
            /* decimal  t_cij         ,*/               (     rawData.t_cij                      ),
            /* string   t_konto       ,*/               (     ""                                 ),
            /* decimal  t_pdvSt       ,*/               (     rawData.t_pdvSt                    ),
            /* decimal  t_rbt1St      ,*/               (     rawData.t_rbt1St                   ),
            /* decimal  t_rbt2St      ,*/               (     rawData.t_rbt2St                   ),
            /* decimal  t_wanted      ,*/               (     rawData.t_wanted                   ),
  /* ZXC.MalopCalcKind  t_mCalcKind   ,*/               (     rawData.t_mCalcKind                ),
  /* ZXC.PdvKolTipEnum  t_pdvKolTip   ,*/               (     rawData.t_pdvKolTip                ),
  /*            string  t_jedMJ       ,*/               (     rawData.t_jedMj                    ),
            /* decimal  t_ztr         ,*/               (     rawData.t_ztrUk                    ),
            /* decimal  t_doCijMal    ,*/               (     prevMalopCij                       ),
            /* decimal  t_noCijMal    ,*/               (rawData.t_noCijMal.NotZero() ? rawData.t_noCijMal : prevMalopCij),
            /* decimal  t_pnpSt       ,*/               (     0M                                 )
            );

      } // foreach(RawData rawData in rawDataListSorted) 
   }

   private void CalcRawResults(ref RawTransData rawData)
   {
      Rtrans rtrans_rec = new Rtrans();

      rtrans_rec.T_TT = GetVektorTT_ForImportTT(rawData.t_tt);

      rtrans_rec.T_kol    = rawData.t_kol;
      rtrans_rec.T_cij    = rawData.t_cij;
      rtrans_rec.T_pdvSt  = rawData.t_pdvSt;
      rtrans_rec.T_rbt1St = rawData.t_rbt1St;
      rtrans_rec.T_rbt2St = rawData.t_rbt2St;
      // 13.12.2012: 
    //rtrans_rec.T_wanted = /*rawData.*/0.00M;
      rtrans_rec.T_wanted    = rawData.t_wanted;
      rtrans_rec.T_mCalcKind = rawData.t_mCalcKind;
      rtrans_rec.T_pdvColTip = rawData.t_pdvKolTip;

      rtrans_rec.CalcTransResults(null);

      rawData.results = rtrans_rec.RtrResults;

   }

}


#if(DEBUG)

public class VvArtikl_KEREMP_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */ internal string _artiklCD;
      /* B 02 */ internal string _artiklName;
      /* C 03 */ internal string _jedMmj;
      /* D 04 */ internal string _artGrCdA;
      /* E 05 */ internal string _artGrCdB;
   }

   public VvArtikl_KEREMP_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      //Artikl artikl_rec = new Artikl();
      RawData rawDatStruct = new RawData();

      rawDatStruct._artiklCD   = GetString(01);
      rawDatStruct._artiklName = GetString(02);
      rawDatStruct._jedMmj     = GetString(03);
      rawDatStruct._artGrCdA   = GetString(04);
      rawDatStruct._artGrCdB   = GetString(05);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Artikl artikl_rec = new Artikl();

      //Kupdob kupdob_rec;

      bool dbOK;
      int errCount = 0, okCount = 0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         artikl_rec.Memset0(0);

         artikl_rec.ArtiklCD   = LimitedStr(rawDatStruct._artiklCD  , ZXC.ArtCI.artiklCD  );
         artikl_rec.ArtiklName = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.artiklName);
         artikl_rec.JedMj      = LimitedStr(rawDatStruct._jedMmj    , ZXC.ArtCI.jedMj     );
         artikl_rec.Grupa1CD   = LimitedStr(rawDatStruct._artGrCdA  , ZXC.ArtCI.grupa1CD  );
         artikl_rec.Grupa2CD   = LimitedStr(rawDatStruct._artGrCdB  , ZXC.ArtCI.grupa2CD  );
         artikl_rec.TS = "SIT";
         //artikl_rec.SkladCD     = "SVPS";
         //artikl_rec.PdvKat = "25";


         //kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Naziv == rawDatStruct._ducatiCD);

         //if(kupdob_rec != null) 
         //{
         //   artikl_rec.ProizCD = kupdob_rec.KupdobCD;
         //}

         #region ADDREC

         dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, /*false 06.07.2015:*/true, false, false, /*false*/ true); // don't report errors, save them in the list

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artName")) // Meaning, ADDREC failed bikoz Microline's artiklName is duplicated (artiklCD is ok, I guess?) 
         {
            artikl_rec.Napomena = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD + ")";
            artikl_rec.ArtiklName = LimitedStrWithAddition(artikl_rec.ArtiklName, uniqueAddition, ZXC.ArtCI.artiklName);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artCD"))
         {
            artikl_rec.Napomena = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD2 + ")";
            artikl_rec.ArtiklCD = LimitedStrWithAddition(artikl_rec.ArtiklCD, uniqueAddition, ZXC.ArtCI.artiklCD);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         #endregion ADDREC

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvArtikl_KEREMP_Importer.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

   private string LimitedStrWithAddition(string data, string uniqueAddition, int cIdx)
   {
      return ZXC.LenLimitedStrWithAddition(data, uniqueAddition, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvArtikl_TEXTHO_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */
      internal string _artiklCD;
      /* B 02 */
      internal string _artiklName;
      /* C 03 */
      internal string _jedMmj;
      /* D 04 */
      internal string _artGrCdA;
      /* E 05 */
      internal string _artGrCdB;
      /* F 06 */
      internal string _artGrCdC;
   }

   public VvArtikl_TEXTHO_Importer(XSqlConnection conn, string fullPathFileName, string delimiter)
      : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      //Artikl artikl_rec = new Artikl();
      RawData rawDatStruct = new RawData();

      rawDatStruct._artiklCD = GetString(01);
      rawDatStruct._artiklName = GetString(02);
      rawDatStruct._jedMmj = GetString(03);
      rawDatStruct._artGrCdA = GetString(04);
      rawDatStruct._artGrCdB = GetString(05);
      rawDatStruct._artGrCdC = GetString(06);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Artikl artikl_rec = new Artikl();

      //Kupdob kupdob_rec;

      bool dbOK;
      int errCount = 0, okCount = 0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         artikl_rec.Memset0(0);

         artikl_rec.ArtiklCD = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.artiklCD);
         artikl_rec.ArtiklName = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.artiklName);
         artikl_rec.JedMj = LimitedStr(rawDatStruct._jedMmj, ZXC.ArtCI.jedMj);
         artikl_rec.Grupa1CD = LimitedStr(rawDatStruct._artGrCdA, ZXC.ArtCI.grupa1CD);
         artikl_rec.Grupa2CD = LimitedStr(rawDatStruct._artGrCdB, ZXC.ArtCI.grupa2CD);
         artikl_rec.Grupa3CD = LimitedStr(rawDatStruct._artGrCdC, ZXC.ArtCI.grupa3CD);
         artikl_rec.TS = "ROB";
         //artikl_rec.SkladCD     = "SVPS";
         artikl_rec.PdvKat = "25";


         //kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Naziv == rawDatStruct._ducatiCD);

         //if(kupdob_rec != null) 
         //{
         //   artikl_rec.ProizCD = kupdob_rec.KupdobCD;
         //}

         #region ADDREC

         dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, /*false 06.07.2015:*/true, false, false, /*false*/ true); // don't report errors, save them in the list

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artName")) // Meaning, ADDREC failed bikoz Microline's artiklName is duplicated (artiklCD is ok, I guess?) 
         {
            artikl_rec.Napomena = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD + ")";
            artikl_rec.ArtiklName = LimitedStrWithAddition(artikl_rec.ArtiklName, uniqueAddition, ZXC.ArtCI.artiklName);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artCD"))
         {
            artikl_rec.Napomena = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD2 + ")";
            artikl_rec.ArtiklCD = LimitedStrWithAddition(artikl_rec.ArtiklCD, uniqueAddition, ZXC.ArtCI.artiklCD);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         #endregion ADDREC

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvArtikl_TEXTHO_Importer.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

   private string LimitedStrWithAddition(string data, string uniqueAddition, int cIdx)
   {
      return ZXC.LenLimitedStrWithAddition(data, uniqueAddition, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvArtikl_SENSO_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */internal string  _oldArtCD;
      /* B 02 */internal string  _partNo;
      /* C 03 */internal string  _artiklName;
      /* D 04 */internal string  _oldArtName;
      /* E 05 */internal string  _grupa1;
      /* F 06 */internal string  _grupa2;
      /* G 07 */internal string  _tipSif;
      /* H 08 */internal string  _dobav;
      /* I 09 */internal string  _pdvKat;
            //internal string  _prefValName;
      /* J 10 */internal string  _jedMj;
      /* K 11 */internal ushort  _garancija;
      /* L 12 */internal string  _carTarifa;
      /* M 13 */internal string  _konto;
      /* N 14 */internal string  _longOpis;

   }

   public VvArtikl_SENSO_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      //Artikl artikl_rec = new Artikl();
      RawData rawDatStruct = new RawData();

      rawDatStruct._oldArtName  = GetString ( 1);
      rawDatStruct._oldArtCD    = GetString ( 2);
      rawDatStruct._partNo      = GetString ( 3);
      rawDatStruct._artiklName  = GetString ( 4);
      rawDatStruct._grupa1      = GetString ( 5);
      rawDatStruct._grupa2      = GetString ( 6);
      rawDatStruct._tipSif      = GetString ( 7);
      rawDatStruct._dobav       = GetString ( 8);
      rawDatStruct._pdvKat      = GetString ( 9);
      rawDatStruct._jedMj       = GetString (10);
      rawDatStruct._garancija   = GetUint16 (11);
      rawDatStruct._carTarifa   = GetString (12);
      rawDatStruct._konto       = GetString (13);
      rawDatStruct._longOpis    = GetString (14);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Artikl artikl_rec = new Artikl();

      Kupdob kupdob_rec;

      #region Grupa1&2

      bool grupeSiVecUcitao = true; // s ovime reguliras oces ili neces (dakle, samo za first import) 

      if(grupeSiVecUcitao == false)
      {
         string origDbName = ZXC.TheMainDbConnection.Database;

         int gr1CD = 0;
         var grupa1DistinctList   = RawDataList.Select(rd => rd._grupa1).Distinct().OrderBy(x => x);
         var grupa1DistinctWithCD = grupa1DistinctList.Select(disGrName => new { grupa1name = disGrName, grupa1CD = (++gr1CD).ToString("00") });

         int gr2CD = 0;
         var grupa2DistinctList   = RawDataList.Select(rd => rd._grupa2).Distinct().OrderBy(x => x);
         var grupa2DistinctWithCD = grupa2DistinctList.Select(disGrName => new { grupa2name = disGrName, grupa2CD = (++gr2CD).ToString("00") });

         foreach(var gr1 in grupa1DistinctWithCD)
         {
            ZXC.luiListaGrupa1Artikla.Add(new VvLookUpItem(gr1.grupa1CD, gr1.grupa1name));
         }
         VvDaoBase.SaveLookUpListToSqlTable(ZXC.luiListaGrupa1Artikla);

         foreach(var gr2 in grupa2DistinctWithCD)
         {
            ZXC.luiListaGrupa2Artikla.Add(new VvLookUpItem(gr2.grupa2CD, gr2.grupa2name));
         }
         VvDaoBase.SaveLookUpListToSqlTable(ZXC.luiListaGrupa2Artikla);

         ZXC.SetMainDbConnDatabaseName(origDbName); // jer SaveLookUpListToSqlTable promijeni na 'vvektor' ... 
      }

      #endregion Grupa1&2

      int newCDcount = 0;
      string maybeArtCD;
      bool dbOK;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType./*None*/Ticker);

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         artikl_rec.Memset0(0);

         artikl_rec.PartNo = LimitedStr(rawDatStruct._partNo, ZXC.ArtCI.artiklCD);
         
         artikl_rec.ArtiklName  = LimitedStr(rawDatStruct._artiklName , ZXC.ArtCI.artiklName );
         artikl_rec.ArtiklCD2   = LimitedStr(rawDatStruct._oldArtCD   , ZXC.ArtCI.artiklCD2  );
         artikl_rec.ArtiklName2 = LimitedStr(rawDatStruct._oldArtName , ZXC.ArtCI.artiklName2);
         artikl_rec.JedMj       = LimitedStr(rawDatStruct._jedMj      , ZXC.ArtCI.jedMj      );
         artikl_rec.Garancija   =            rawDatStruct._garancija;
         artikl_rec.CarTarifa   = LimitedStr(rawDatStruct._carTarifa  , ZXC.ArtCI.carTarifa  );
         artikl_rec.Konto       = LimitedStr(rawDatStruct._konto      , ZXC.ArtCI.konto      );
         artikl_rec.LongOpis    = LimitedStr(rawDatStruct._longOpis   , ZXC.ArtCI.longOpis   );

         if(rawDatStruct._grupa1.NotEmpty())
         artikl_rec.Grupa1CD    = ZXC.luiListaGrupa1Artikla.Single(lui => lui.Name == rawDatStruct._grupa1).Cd;
         if(rawDatStruct._grupa2.NotEmpty())
         artikl_rec.Grupa2CD    = ZXC.luiListaGrupa2Artikla.Single(lui => lui.Name == rawDatStruct._grupa2).Cd;

         kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Naziv == rawDatStruct._dobav);
         if(kupdob_rec != null) 
         {
            artikl_rec.DobavCD = kupdob_rec.KupdobCD;
         }

         switch(rawDatStruct._tipSif)
         {
            case "200": artikl_rec.TS = "ROB"; break; // roba
            case "300": artikl_rec.TS = "VLP"; break; // vlas. proizvod 
            case "700": artikl_rec.TS = "USL"; break; // usluga 
         }

         if(rawDatStruct._pdvKat.StartsWith("23")) artikl_rec.PdvKat = "23";

         artikl_rec.SkladCD = "VPSK";

         maybeArtCD = artikl_rec.PartNo/*ArtiklCD2*/;
         if(maybeArtCD.NotEmpty())
            artikl_rec.ArtiklCD = maybeArtCD;
         else
            artikl_rec.ArtiklCD = "XY-" + (++newCDcount).ToString("0000");

         dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvArtikl_SENSO_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvNalogPS_SENSO_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   private struct RawData
   {
      /* A 01 */internal string   _vezniDok;
      /* B 02 */internal string   _tvrtka  ;
      /* C 03 */internal string   _konto   ;
      /* D 04 */internal DateTime _dateVal ;
      /* E 05 */internal string   _dokument;
      /* F 06 */internal DateTime _dateDok ;
      /* G 07 */internal string   _napomena;
      /* H 08 */internal decimal  _dug     ;
      /* I 09 */internal decimal  _pot     ;

                internal uint     _kupdobCd;
                internal string   _kupdobTk;

                public override string ToString()
                {
                   return "[" + _kupdobTk + "]" + _tvrtka + "-" + _konto + "-" + _dokument;
                }
   }

   private string CurrKupdobName { get; set; }
   private uint   CurrKupdobCd   { get; set; }
   private string CurrKupdobTk   { get; set; }

   public VvNalogPS_SENSO_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Ticker);

      CurrKupdobName = "";
      CurrKupdobCd   = 0;
      CurrKupdobTk   = "";
   }

   private string GetDocumentGroupID(RawData rawData)
   {
      return rawData._tvrtka;
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      Kupdob kupdobSifrar_rec;

      RawData rawDatStruct = new RawData();

      rawDatStruct._vezniDok  = GetString  ( 1);
      rawDatStruct._tvrtka    = GetString  ( 2);
      rawDatStruct._konto     = GetString  ( 3);
      rawDatStruct._dateVal   = GetddMMyyyy( 4);
      rawDatStruct._dokument  = GetString  ( 5); // TipBr 
      rawDatStruct._dateDok   = GetddMMyyyy( 6);
      rawDatStruct._napomena  = GetString  ( 7);
      rawDatStruct._dug       = GetDecimal ( 8);
      rawDatStruct._pot       = GetDecimal ( 9);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         if(rawDatStruct._tvrtka.IsEmpty() && rawDatStruct._dokument.IsEmpty() && rawDatStruct._konto.IsEmpty()) return;

         if(rawDatStruct._tvrtka.NotEmpty() && rawDatStruct._dokument.IsEmpty()) // KupDob Header 
         {
            CurrKupdobName = rawDatStruct._tvrtka;

            kupdobSifrar_rec = VvUserControl.KupdobSifrar./*Single*/FirstOrDefault(vvDR => vvDR.Naziv.ToLower() == rawDatStruct._tvrtka.ToLower());

            if(kupdobSifrar_rec != null)
            {
               CurrKupdobCd = kupdobSifrar_rec.KupdobCD;
               CurrKupdobTk = kupdobSifrar_rec.Ticker;
            }
            else
            {
               CurrKupdobCd = 0;
               CurrKupdobTk = "";
            }
         }
         else // classic 
         {
            rawDatStruct._tvrtka   = CurrKupdobName;
            rawDatStruct._kupdobCd = CurrKupdobCd;
            rawDatStruct._kupdobTk = CurrKupdobTk;

            this.RawDataList.Add(rawDatStruct);
         }
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Ftrans ftrans_rec = new Ftrans();
      ushort line = 0;
      string n_napomena = "";
      string currGroupID = null;

      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         if(currGroupID != GetDocumentGroupID(rawDatStruct)) // Novi dokument (zaglavlje) 
         {
            line = 0;
            currGroupID = GetDocumentGroupID(rawDatStruct);
         }

         ftrans_rec.Memset0(0);

         ftrans_rec.T_ticker    = LimitedStr(rawDatStruct._kupdobTk, ZXC.FtrCI.t_ticker);
         ftrans_rec.T_kupdob_cd =            rawDatStruct._kupdobCd                     ;
         ftrans_rec.T_tipBr     = LimitedStr(rawDatStruct._dokument, ZXC.FtrCI.t_tipBr );
         ftrans_rec.T_konto     = LimitedStr(rawDatStruct._konto   , ZXC.FtrCI.t_konto );
         ftrans_rec.T_valuta    =            rawDatStruct._dateVal                      ;
         ftrans_rec.T_opis      = LimitedStr(rawDatStruct._dateDok.ToString(ZXC.VvDateDdMmYyFormat) + " / " +
                                             rawDatStruct._vezniDok                                 + " / " +
                                             rawDatStruct._napomena, ZXC.FtrCI.t_opis);
         ftrans_rec.T_dug       =            rawDatStruct._dug;
         ftrans_rec.T_pot       =            rawDatStruct._pot;
                                
         ftrans_rec.T_TT        = "PS";
         ftrans_rec.T_dokDate   = ZXC.projectYearFirstDay;
         
         n_napomena = (ftrans_rec.T_kupdob_cd.IsZero() ? "XY?!-" : "") + LimitedStrNalog(rawDatStruct._tvrtka, ZXC.NalCI.napomena);

         //#if(Njett)
         //------------------------------------------------------------------------- 
         /* */
         /* */  NalogDao.AutoSetNalog(conn, ref line,
         /* */
         /* */ /*DateTime n_dokDate    */ ftrans_rec.T_dokDate,
         /* */ /*string   n_tt         */ ftrans_rec.T_TT,
         /* */ /*string   n_napomena   */ n_napomena,
         /* */
         /* */ /*string   t_konto      */ ftrans_rec.T_konto,
         /* */ /*uint     t_kupdob_cd  */ ftrans_rec.T_kupdob_cd,
         /* */ /*string   t_ticker     */ ftrans_rec.T_ticker,
         /* */ /*uint     t_mtros_cd   */ ftrans_rec.T_mtros_cd,
         /* */ /*uint     t_mtros_tk   */ ftrans_rec.T_mtros_tk,
         /* */ /*string   t_tipBr      */ ftrans_rec.T_tipBr,
         /* */ /*string   t_opis       */ ftrans_rec.T_opis,
         /* */ /*DateTime t_valuta     */ ftrans_rec.T_valuta,
         /* */ /*string   t_pdv        */ ftrans_rec.T_pdv,
         /* */ /*string   t_037        */ ftrans_rec.T_037,
         /* */ /* string   t_projektCD */ "",
         /* */ /* ushort   t_pdvKnjiga */ ZXC.PdvKnjigaEnum.NIJEDNA,
         /* */ /* uint     t_fakRecID  */ 0,
         /* */ /* OtsKindEnum t_otsKind*/ ZXC.OtsKindEnum.NIJEDNO,
         /* */ /* string   t_fond      */ "",
         /* */ /* string   t_pozicija  */ "",
         /* */ /*decimal  t_dug        */ ftrans_rec.T_dug,
         /* */ /*decimal  t_pot        */ ftrans_rec.T_pot);
         /* */
         //------------------------------------------------------------------------- 
//#endif

         // some debuggin' stuff
         //List<RawData> pero = this.RawDataList.Where(x => x._kupdobCd == 0).ToList();
         //List<string> distinct = pero.Select(p => p._tvrtka).Distinct().ToList();

         #region Report Errors manager

         if(ZXC.sqlErrNo.NotZero())
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvNalogPS_SENSO_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.FtransDao.GetSchemaColumnSize(cIdx));
   }
   private string LimitedStrNalog(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.NalogDao.GetSchemaColumnSize(cIdx));
   }


}

public class VvNalogPS_TEMBO_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   private struct RawData
   {
      /* A 01 */internal string   _kcdAndName_or_TipBr;
      /* B 02 */internal string   _valuta_or_opisA    ;
      /* C 03 */internal string   _opisB              ;
      /* D 04 */internal decimal  _dug                ;
      /* E 05 */internal decimal  _pot                ;

                internal uint     _kupdobCd;
                internal string   _kupdobTk;
                internal string   _tipBr   ;
                internal DateTime _dateVal ;
                internal string   _theOpis ;

                public override string ToString()
                {
                   return "[" + _kcdAndName_or_TipBr + "]" + _valuta_or_opisA + "-" + _opisB;
                }
   }

   private string CurrKupdobName { get; set; }
   private uint   CurrKupdobCd   { get; set; }
   private string CurrKupdobTk   { get; set; }

   public VvNalogPS_TEMBO_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Ticker);

      CurrKupdobName = "";
      CurrKupdobCd   = 0;
      CurrKupdobTk   = "";
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._kcdAndName_or_TipBr  = GetString  ( 1);
      rawDatStruct._valuta_or_opisA      = GetString  ( 2);
      rawDatStruct._opisB                = GetString  ( 3);
      rawDatStruct._dug                  = GetDecimal ( 4);
      rawDatStruct._pot                  = GetDecimal ( 5);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {}
   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Ftrans ftrans_rec = new Ftrans();
      ushort line = 0;
      string n_napomena = "";
      string currTipBr = "";

      string maybeCD;
      uint iposKCD;
      bool newSuccker;

      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);
      Kupdob kupdobSifrar_rec;

      List<RawData> betterRawDataList = new List<RawData>();
      RawData betterRawData = new RawData();


      uint theKCD; int oibIdx; 
      string theName, theOIB="", theUlica, theGrad, thePostaBr;
      string[] subStrings;
      DateTime tmpDate;

      foreach(RawData raw in this.RawDataList)
      {
         if(raw.ToString().Length < 5)              continue;
         if(raw.ToString().StartsWith("Copyright")) continue;

         //first4Chars = raw._kupdobCDAndName.Length >= 4 ? raw._kupdobCDAndName.Substring(0, 4).TrimEnd(' ') : "NJET";
         //iposKCD = ZXC.ValOrZero_UInt(first4Chars);
         //if(first4Chars == iposKCD.ToString()) { newSuccker = true ; }
         //else                                  { newSuccker = false; }


         maybeCD = raw._kcdAndName_or_TipBr.Substring(0, (raw._kcdAndName_or_TipBr + " ").IndexOf(' '));
         iposKCD = ZXC.ValOrZero_UInt(maybeCD);
         if(maybeCD == iposKCD.ToString()) { newSuccker = true;  }
         else                              { newSuccker = false; }

         if(newSuccker)
         {
            kupdobSifrar_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.KupdobCD == iposKCD);
            if(kupdobSifrar_rec != null)
            {
               CurrKupdobCd = kupdobSifrar_rec.KupdobCD;
               CurrKupdobTk = kupdobSifrar_rec.Ticker;
            }
            else
            {
               CurrKupdobCd = iposKCD;
               CurrKupdobTk = "_?!?_";
            }

            ftrans_rec = new Ftrans();
         }
         else
         {
            betterRawData = new RawData();

            betterRawData._kupdobCd = CurrKupdobCd;
            betterRawData._kupdobTk = CurrKupdobTk;
            betterRawData._dug      = raw._dug    ;
            betterRawData._pot      = raw._pot    ;

            betterRawData._kcdAndName_or_TipBr = raw._kcdAndName_or_TipBr.TrimEnd(' ') + " ";
            betterRawData._valuta_or_opisA     = raw._valuta_or_opisA    .TrimEnd(' ');
            betterRawData._opisB               = raw._opisB              .TrimEnd(' ');

            if(raw._kcdAndName_or_TipBr.StartsWith("IRN") || raw._kcdAndName_or_TipBr.StartsWith("URN"))
            {
               betterRawData._tipBr = currTipBr = raw._kcdAndName_or_TipBr;
            }
            else
            {
             //betterRawData._tipBr = raw._kcdAndName_or_TipBr;
               betterRawData._tipBr = currTipBr;
            }

            tmpDate = ZXC.ValOr_01010001_DateTime(raw._valuta_or_opisA);
            if(tmpDate != DateTime.MinValue)
            {
               betterRawData._dateVal = tmpDate;
            }
            else
            {
               betterRawData._theOpis = raw._valuta_or_opisA;
            }

            betterRawData._theOpis = betterRawData._theOpis.IsEmpty() ? raw._opisB : betterRawData._theOpis + "/" + raw._opisB;

            betterRawDataList.Add(betterRawData);
         }

      }
      //.. Za zadnjega 
      // treba li tu kaj?! 


      foreach(RawData rawDatStruct in betterRawDataList)
      {
//         ftrans_rec.Memset0(0);

         ftrans_rec.T_ticker    = LimitedStr(rawDatStruct._kupdobTk, ZXC.FtrCI.t_ticker);
         ftrans_rec.T_kupdob_cd =            rawDatStruct._kupdobCd                     ;
         ftrans_rec.T_tipBr     = LimitedStr(rawDatStruct._tipBr   , ZXC.FtrCI.t_tipBr );
       //ftrans_rec.T_konto     = LimitedStr(rawDatStruct._konto   , ZXC.FtrCI.t_konto );
         ftrans_rec.T_konto     = this.FullPathFileName.Contains("d.txt") ? "2200" : "1200";
         ftrans_rec.T_valuta    =            rawDatStruct._dateVal                      ;
         ftrans_rec.T_opis      = LimitedStr(rawDatStruct._theOpis , ZXC.FtrCI.t_opis);
         ftrans_rec.T_dug       =            rawDatStruct._dug;
         ftrans_rec.T_pot       =            rawDatStruct._pot;
                                
         ftrans_rec.T_TT        = "PS";
         ftrans_rec.T_dokDate   = ZXC.projectYearFirstDay;
         
       //n_napomena = (ftrans_rec.T_kupdob_cd.IsZero() ? "XY?!-" : "") + LimitedStrNalog(rawDatStruct._valuta_or_opisA, ZXC.NalCI.napomena);

         //#if(Njett)
         //------------------------------------------------------------------------- 
         /* */
         /* */  NalogDao.AutoSetNalog(conn, ref line,
         /* */
         /* */ /*DateTime n_dokDate    */ ftrans_rec.T_dokDate,
         /* */ /*string   n_tt         */ ftrans_rec.T_TT,
         /* */ /*string   n_napomena   */ n_napomena,
         /* */
         /* */ /*string   t_konto      */ ftrans_rec.T_konto,
         /* */ /*uint     t_kupdob_cd  */ ftrans_rec.T_kupdob_cd,
         /* */ /*string   t_ticker     */ ftrans_rec.T_ticker,
         /* */ /*uint     t_mtros_cd   */ ftrans_rec.T_mtros_cd,
         /* */ /*uint     t_mtros_tk   */ ftrans_rec.T_mtros_tk,
         /* */ /*string   t_tipBr      */ ftrans_rec.T_tipBr,
         /* */ /*string   t_opis       */ ftrans_rec.T_opis,
         /* */ /*DateTime t_valuta     */ ftrans_rec.T_valuta,
         /* */ /*string   t_pdv        */ ftrans_rec.T_pdv,
         /* */ /*string   t_037        */ ftrans_rec.T_037,
         /* */ /* string   t_projektCD */ "",
         /* */ /* ushort   t_pdvKnjiga */ ZXC.PdvKnjigaEnum.NIJEDNA,
         /* */ /* uint     t_fakRecID  */ 0,
         /* */ /* OtsKindEnum t_otsKind*/ ZXC.OtsKindEnum.NIJEDNO,
         /* */ /* string   t_fond      */ "",
         /* */ /* string   t_pozicija  */ "",
         /* */ /*decimal  t_dug        */ ftrans_rec.T_dug,
         /* */ /*decimal  t_pot        */ ftrans_rec.T_pot);
         /* */
         //------------------------------------------------------------------------- 
//#endif

         // some debuggin' stuff
         //List<RawData> pero = this.RawDataList.Where(x => x._kupdobCd == 0).ToList();
         //List<string> distinct = pero.Select(p => p._tvrtka).Distinct().ToList();

         #region Report Errors manager

         if(ZXC.sqlErrNo.NotZero())
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvNalogPS_TEMBO_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.FtransDao.GetSchemaColumnSize(cIdx));
   }
   private string LimitedStrNalog(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.NalogDao.GetSchemaColumnSize(cIdx));
   }


}

public class VvNalogPS_TEXTHO_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   private struct RawData
   {
      /* A 01 */internal string   _t_konto    ;
      /* B 02 */internal uint     _t_kupdob_cd;
      /* C 03 */internal uint     _t_mtros_cd ;
      /* D 04 */internal string   _t_tipBr    ;
      /* E 05 */internal string   _t_opis     ;
      /* F 06 */internal string   _t_valuta   ;
      /* G 07 */internal string   _t_tt       ;
      /* H 08 */internal string   _t_dokDate  ;
      /* I 09 */internal string   _t_pdv      ;
      /* J 10 */internal string   _t_037      ;
      /* K 11 */internal decimal  _t_dug      ;
      /* L 12 */internal decimal  _t_pot      ;

                public override string ToString()
                {
                   return "[" + _t_konto + "]" + _t_kupdob_cd + "-" + _t_tipBr;
                }
   }

   private string CurrKupdobName { get; set; }
   private uint   CurrKupdobCd   { get; set; }
   private string CurrKupdobTk   { get; set; }

   public VvNalogPS_TEXTHO_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Ticker);

      CurrKupdobName = "";
      CurrKupdobCd   = 0;
      CurrKupdobTk   = "";
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._t_konto      = GetString  ( 1);
      rawDatStruct._t_kupdob_cd  = GetUint32  ( 2);
      rawDatStruct._t_mtros_cd   = GetUint16  ( 3);
      rawDatStruct._t_tipBr      = GetString  ( 4);
      rawDatStruct._t_opis       = GetString  ( 5);
      rawDatStruct._t_valuta     = GetString  ( 6);
      rawDatStruct._t_tt         = GetString  ( 7);
      rawDatStruct._t_dokDate    = GetString  ( 8);
      rawDatStruct._t_pdv        = GetString  ( 9);
      rawDatStruct._t_037        = GetString  (10);
      rawDatStruct._t_dug        = GetDecimal (11);
      rawDatStruct._t_pot        = GetDecimal (12);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {}
   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Ftrans ftrans_rec = new Ftrans();
      ushort line = 0;
      string n_napomena = "";
      string currTipBr = "";

      //string maybeCD;
      //uint iposKCD;
      //bool newSuccker;

      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);
      Kupdob kupdobSifrar_rec;

    //List<RawData> betterRawDataList = new List<RawData>();
      RawData betterRawData = new RawData();


      uint theKCD; int oibIdx; 
      string theName, theOIB="", theUlica, theGrad, thePostaBr;
      string[] subStrings;
      DateTime tmpDate;

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         kupdobSifrar_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.KupdobCD == rawDatStruct._t_kupdob_cd);
         if(kupdobSifrar_rec != null)
         {
            CurrKupdobCd = kupdobSifrar_rec.KupdobCD;
            CurrKupdobTk = kupdobSifrar_rec.Ticker;
         }
         else if(rawDatStruct._t_kupdob_cd.NotZero())
         {
            CurrKupdobCd = rawDatStruct._t_kupdob_cd;
            CurrKupdobTk = "_?!?_";
         }
         else
         {
            CurrKupdobCd = 0;
            CurrKupdobTk = "";
         }

         ftrans_rec = new Ftrans();

         ftrans_rec.T_ticker    = LimitedStr(CurrKupdobTk,          ZXC.FtrCI.t_ticker);

         ftrans_rec.T_konto     = LimitedStr(rawDatStruct._t_konto, ZXC.FtrCI.t_konto );
         ftrans_rec.T_kupdob_cd =            CurrKupdobCd                              ;
         ftrans_rec.T_mtros_cd  =            0                                         ;
         ftrans_rec.T_tipBr     = LimitedStr(rawDatStruct._t_tipBr, ZXC.FtrCI.t_tipBr );
         ftrans_rec.T_opis      = LimitedStr(rawDatStruct._t_opis , ZXC.FtrCI.t_opis  );
         ftrans_rec.T_valuta    = DateTime.MinValue;
         ftrans_rec.T_TT        = LimitedStr(rawDatStruct._t_tt   , ZXC.FtrCI.t_tt    );
         ftrans_rec.T_dokDate   = ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(rawDatStruct._t_dokDate.Replace(".", ""));
         ftrans_rec.T_pdv       = LimitedStr(rawDatStruct._t_pdv  , ZXC.FtrCI.t_pdv   );
         ftrans_rec.T_037       = LimitedStr(rawDatStruct._t_037  , ZXC.FtrCI.t_037   );
         ftrans_rec.T_dug       =            rawDatStruct._t_dug;
         ftrans_rec.T_pot       =            rawDatStruct._t_pot;
         
       //n_napomena = (ftrans_rec.T_kupdob_cd.IsZero() ? "XY?!-" : "") + LimitedStrNalog(rawDatStruct._valuta_or_opisA, ZXC.NalCI.napomena);

         //#if(Njett)
         //------------------------------------------------------------------------- 
         /* */
         /* */  NalogDao.AutoSetNalog(conn, ref line,
         /* */
         /* */ /*DateTime n_dokDate    */ ftrans_rec.T_dokDate,
         /* */ /*string   n_tt         */ ftrans_rec.T_TT,
         /* */ /*string   n_napomena   */ n_napomena,
         /* */
         /* */ /*string   t_konto      */ ftrans_rec.T_konto,
         /* */ /*uint     t_kupdob_cd  */ ftrans_rec.T_kupdob_cd,
         /* */ /*string   t_ticker     */ ftrans_rec.T_ticker,
         /* */ /*uint     t_mtros_cd   */ ftrans_rec.T_mtros_cd,
         /* */ /*uint     t_mtros_tk   */ ftrans_rec.T_mtros_tk,
         /* */ /*string   t_tipBr      */ ftrans_rec.T_tipBr,
         /* */ /*string   t_opis       */ ftrans_rec.T_opis,
         /* */ /*DateTime t_valuta     */ ftrans_rec.T_valuta,
         /* */ /*string   t_pdv        */ ftrans_rec.T_pdv,
         /* */ /*string   t_037        */ ftrans_rec.T_037,
         /* */ /* string   t_projektCD */ "",
         /* */ /* ushort   t_pdvKnjiga */ ZXC.PdvKnjigaEnum.NIJEDNA,
         /* */ /* uint     t_fakRecID  */ 0,
         /* */ /* OtsKindEnum t_otsKind*/ ZXC.OtsKindEnum.NIJEDNO,
         /* */ /* string   t_fond      */ "",
         /* */ /* string   t_pozicija  */ "",
         /* */ /*decimal  t_dug        */ ftrans_rec.T_dug,
         /* */ /*decimal  t_pot        */ ftrans_rec.T_pot);
         /* */
         //------------------------------------------------------------------------- 
//#endif

         // some debuggin' stuff
         //List<RawData> pero = this.RawDataList.Where(x => x._kupdobCd == 0).ToList();
         //List<string> distinct = pero.Select(p => p._tvrtka).Distinct().ToList();

         #region Report Errors manager

         if(ZXC.sqlErrNo.NotZero())
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvNalogPS_TEXTHO_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.FtransDao.GetSchemaColumnSize(cIdx));
   }
   private string LimitedStrNalog(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.NalogDao.GetSchemaColumnSize(cIdx));
   }


}

public class VvArtikl_FRIGOTERM_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 *///internal int    _oldArtID_Dummy;
      /* B 02 */  internal string _artiklCD;
      /* C 03 */  internal string _artiklName;
      /* D 04 */  internal string _artiklOpis;
      /* E 05 */  internal string _proizvName;
      /* F 06 *///internal string _minKol_Dummy;
      /* G 07 */  internal string _jedMj;
      /* H 08 *///internal string _jedMj2_Dummy;
      /* I 09 *///internal string _koefJM_Dummy;
      /* J 10 */  internal string _vrsta;
   }

   public VvArtikl_FRIGOTERM_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      //Artikl artikl_rec = new Artikl();
      RawData rawDatStruct = new RawData();

      rawDatStruct._artiklCD   = GetString ( 2);
      rawDatStruct._artiklName = GetString ( 3).Replace('^', ';');
      rawDatStruct._artiklOpis = GetString ( 4).Replace("^", "\r\n");
      rawDatStruct._proizvName = GetString ( 5);
      rawDatStruct._jedMj      = GetString ( 7);
      rawDatStruct._vrsta      = GetString (10);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Artikl artikl_rec = new Artikl();

      Kupdob kupdob_rec;

      bool dbOK;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         artikl_rec.Memset0(0);

         artikl_rec.ArtiklCD    = LimitedStr(rawDatStruct._artiklCD   , ZXC.ArtCI.artiklCD   );
         artikl_rec.ArtiklName  = LimitedStr(rawDatStruct._artiklName , ZXC.ArtCI.artiklName );
         artikl_rec.LongOpis    = LimitedStr(rawDatStruct._artiklOpis , ZXC.ArtCI.longOpis   );
         artikl_rec.ArtiklName2 = LimitedStr(rawDatStruct._proizvName , ZXC.ArtCI.artiklName2);
         artikl_rec.JedMj       = LimitedStr(rawDatStruct._jedMj      , ZXC.ArtCI.jedMj      );

         artikl_rec.TS          = GetVektorTS_Via_vrsta(rawDatStruct._vrsta);


         kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Naziv == rawDatStruct._proizvName);

         if(kupdob_rec != null) 
         {
            artikl_rec.ProizCD = kupdob_rec.KupdobCD;
         }

         artikl_rec.PdvKat = "23";

         artikl_rec.SkladCD = "GLSK";
         
         #region ADDREC

         dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artName")) // Meaning, ADDREC failed bikoz Microline's artiklName is duplicated (artiklCD is ok, I guess?) 
         {
            artikl_rec.Napomena   = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD + ")";
            artikl_rec.ArtiklName = LimitedStrWithAddition(artikl_rec.ArtiklName, uniqueAddition, ZXC.ArtCI.artiklName);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         #endregion ADDREC

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvArtikl_FRIGOTERM_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string GetVektorTS_Via_vrsta(string frigoArtiklVrsta)
   {
      switch(frigoArtiklVrsta)
      {
         case "M": return "MAT";
         case "A": return "ALA";
         case "B": return "AMB";
         case "G": return "REZ";
         case "H": return "HTZ";
         case "K": return "KMT";
         case "O": return "OSR";
         case "P": return "POT";
         case "R": return "ROB";
         case "S": return "SIT";
         case "T": return "TPL";

         default: return "NDF";
      }
   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

   private string LimitedStrWithAddition(string data, string uniqueAddition, int cIdx)
   {
      return ZXC.LenLimitedStrWithAddition(data, uniqueAddition, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvKupdob_FRIGOTERM_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal uint   _kupdobCD  ;
      /* B 02 */  internal string _kupdobName;
      /* C 03 */  internal string _telefon   ;
      /* D 04 */  internal string _telefax   ;
      /* E 05 */  internal string _adresa    ;
      /* F 06 */  internal string _ptt       ;
      /* G 07 */  internal string _grad      ;
      /* H 08 */  internal string _drzava    ;
      /* I 09 */  internal string _dummy2    ;
      /* J 10 */  internal string _prezime   ;
      /* K 11 */  internal string _ime       ;
      /* L 12 */  internal string _oTelefon  ;
      /* M 13 */  internal string _oAdresa   ;
      /* N 14 */  internal string _oTelefax  ;
   }

   public VvKupdob_FRIGOTERM_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._kupdobCD   = GetUint32 ( 1);
      rawDatStruct._kupdobName = GetString ( 2);
      rawDatStruct._telefon    = GetString ( 3);
      rawDatStruct._telefax    = GetString ( 4);
      rawDatStruct._adresa     = GetString ( 5);
      rawDatStruct._ptt        = GetString ( 6);
      rawDatStruct._grad       = GetString ( 7);
      rawDatStruct._drzava     = GetString ( 8);
      rawDatStruct._dummy2     = GetString ( 9);
      rawDatStruct._prezime    = GetString (10);
      rawDatStruct._ime        = GetString (11);
      rawDatStruct._oTelefon   = GetString (12);
      rawDatStruct._oAdresa    = GetString (13);
      rawDatStruct._oTelefax   = GetString (14);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Kupdob kupdob_rec = new Kupdob();

      bool dbOK=true, isFirma;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      uint   currCentCD = 0, kupdobCD = 0;;
      string currCentTK = "";

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         kupdob_rec.Memset0(0);

         isFirma = (rawDatStruct._kupdobCD.NotZero() && rawDatStruct._kupdobName.NotEmpty());

         if(isFirma)
         {
          //artikl_rec.KupdobCD = (rawDatStruct._kupdobCD);
            kupdob_rec.KupdobCD = ++kupdobCD;
            kupdob_rec.Napom2   = (rawDatStruct._kupdobCD.ToString());
            kupdob_rec.Naziv    = LimitedStr(rawDatStruct._kupdobName, ZXC.KpdbCI.naziv);
            kupdob_rec.Ticker   = kupdob_rec.GenerateTicker(2);

            kupdob_rec.Tel1     = LimitedStr(rawDatStruct._telefon, ZXC.KpdbCI.tel1   );
            kupdob_rec.Fax      = LimitedStr(rawDatStruct._telefax, ZXC.KpdbCI.fax    );
            kupdob_rec.Ulica1   = 
            kupdob_rec.Ulica2   = LimitedStr(rawDatStruct._adresa , ZXC.KpdbCI.ulica1 );
            kupdob_rec.PostaBr  = LimitedStr(rawDatStruct._ptt    , ZXC.KpdbCI.postaBr);
            kupdob_rec.Grad     = LimitedStr(rawDatStruct._grad   , ZXC.KpdbCI.grad   );
            kupdob_rec.Drzava   = LimitedStr(rawDatStruct._drzava , ZXC.KpdbCI.drzava );

            kupdob_rec.IsCentr  = true;

            kupdob_rec.Komentar = LimitedStr(rawDatStruct._telefon + ";" + rawDatStruct._telefax + ";" + rawDatStruct._adresa, ZXC.KpdbCI.komentar);

            currCentCD = kupdob_rec.KupdobCD;
            currCentTK = kupdob_rec.Ticker;
         }
         else // Excel red se odnosi na osobu u nekoj firmi
         {
            if((rawDatStruct._prezime + rawDatStruct._ime).NotEmpty() == false) continue;
            
            kupdob_rec.Prezime = LimitedStr(rawDatStruct._prezime , ZXC.KpdbCI.prezime);
            kupdob_rec.Ime     = LimitedStr(rawDatStruct._ime     , ZXC.KpdbCI.ime    );
            kupdob_rec.Tel1    = LimitedStr(rawDatStruct._oTelefon, ZXC.KpdbCI.tel1   );
            kupdob_rec.Fax     = LimitedStr(rawDatStruct._oTelefax, ZXC.KpdbCI.fax    );
            kupdob_rec.Ulica1  = 
            kupdob_rec.Ulica2  = LimitedStr(rawDatStruct._oAdresa , ZXC.KpdbCI.ulica1 );

            kupdob_rec.CentrID   = currCentCD;
            kupdob_rec.CentrTick = currCentTK;

            kupdob_rec.IsZzz = true;
            kupdob_rec.Tip   = "O";

            kupdob_rec.Naziv = LimitedStr(Person.GetPrezimeIme(kupdob_rec.Prezime, kupdob_rec.Ime) + " / " + currCentTK, ZXC.KpdbCI.naziv);

            kupdob_rec.KupdobCD = ++kupdobCD;
            kupdob_rec.Ticker   = kupdob_rec.KupdobCD.ToString();

            kupdob_rec.Komentar = LimitedStr(rawDatStruct._oTelefon + ";" + rawDatStruct._oTelefax + ";" + rawDatStruct._oAdresa, ZXC.KpdbCI.komentar);

         }

         dbOK = kupdob_rec.VvDao.ADDREC(conn, kupdob_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvKupdob_FRIGOTERM_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.KupdobDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvArtikl_DUCATI_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string _artiklCD;
      /* B 02 */  internal string _artiklName;
      /* C 03 */  internal string _jedMj;
      /* D 04 */  internal string _grupa;
      /* E 05 */  internal string _ducatiCD;
   }

   public VvArtikl_DUCATI_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      //Artikl artikl_rec = new Artikl();
      RawData rawDatStruct = new RawData();

      rawDatStruct._artiklCD   = GetString(1);
      rawDatStruct._artiklName = GetString(2);
      rawDatStruct._jedMj      = GetString(3);
      rawDatStruct._grupa      = GetString(4);
      rawDatStruct._ducatiCD   = GetString(5);
    //rawDatStruct._artiklOpis = GetString ( 43.Replace("^", "\r\n");
    //rawDatStruct._artiklName = GetString(2).Replace('^', ';');

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Artikl artikl_rec = new Artikl();

      //Kupdob kupdob_rec;

      bool dbOK;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         artikl_rec.Memset0(0);

         if(rawDatStruct._ducatiCD.NotEmpty())
         {
            artikl_rec.ArtiklCD  = LimitedStr(rawDatStruct._ducatiCD, ZXC.ArtCI.artiklCD );
            artikl_rec.ArtiklCD2 = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.artiklCD2);
         }
         else
         {
            artikl_rec.ArtiklCD  = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.artiklCD );
            artikl_rec.ArtiklCD2 = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.artiklCD2);
         }

         artikl_rec.ArtiklName  = LimitedStr(rawDatStruct._artiklName , ZXC.ArtCI.artiklName );
         artikl_rec.JedMj       = LimitedStr(rawDatStruct._jedMj      , ZXC.ArtCI.jedMj      );

         artikl_rec.Grupa1CD    = GetVektorGrupa1CD_Via_RawGrupa(rawDatStruct._grupa);
         artikl_rec.TS          = GetVektorTS_Via_JedMj         (rawDatStruct._jedMj);


         //kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Naziv == rawDatStruct._ducatiCD);

         //if(kupdob_rec != null) 
         //{
         //   artikl_rec.ProizCD = kupdob_rec.KupdobCD;
         //}

         artikl_rec.PdvKat = "25";

         artikl_rec.SkladCD = "MPSK";
         
         #region ADDREC

         dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artName")) // Meaning, ADDREC failed bikoz Microline's artiklName is duplicated (artiklCD is ok, I guess?) 
         {
            artikl_rec.Napomena   = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD + ")";
            artikl_rec.ArtiklName = LimitedStrWithAddition(artikl_rec.ArtiklName, uniqueAddition, ZXC.ArtCI.artiklName);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artCD")) 
         {
            artikl_rec.Napomena = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD2 + ")";
            artikl_rec.ArtiklCD   = LimitedStrWithAddition(artikl_rec.ArtiklCD, uniqueAddition, ZXC.ArtCI.artiklCD);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         #endregion ADDREC

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvArtikl_DUCATI_Importer.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string GetVektorGrupa1CD_Via_RawGrupa(string rawGrupa)
   {
      // MOT - Motocikl     
      // REZ - Rezervni Dio 
      // ODJ - Odjeća       
      // ULJ - Ulje         

      // Excel ima: 

      // "/motocikl"
      // "/rez. di"
      // "D/ rez.dio"
      // "D/odjeća"
      // "D/odjeća; rez.dio"
      // "D/rez. dio"
      // "O/ odjeća"
      // "O/odjeća"
      // "O/rez. dio"
      // "U/"
      // "U/odjeća"
      // "U/rez. dio"
      // "U/rez. dio; ulje"
      // "U/ulje"

      if(rawGrupa          .StartsWith("D")) return "REZ";
      if(rawGrupa          .StartsWith("U")) return "ULJ";

      if(rawGrupa.ToLower().Contains("motocikl")) return "MOT";
      if(rawGrupa.ToLower().Contains("ulje"))     return "ULJ";
      if(rawGrupa.ToLower().Contains("odje"))     return "ODJ";
      if(rawGrupa.ToLower().Contains("U//"))      return "ULJ";
      if(rawGrupa.ToLower().Contains("rez"))      return "REZ";

      if(rawGrupa.NotEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "GetVektorGrupa1CD_Via_RawGrupa\n\nUNDEFINED rawGrupa: <" + rawGrupa + ">!");
      }

      return "";
   }

   private string GetVektorTS_Via_JedMj(string ducatiJedMj)
   {
      switch(ducatiJedMj)
      {
         case "dan": return "USL";
         case "USL": return "USL";
         case "KOM": return "ROB";

       //default: return "NDF";
         default: return "ROB";
      }
   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

   private string LimitedStrWithAddition(string data, string uniqueAddition, int cIdx)
   {
      return ZXC.LenLimitedStrWithAddition(data, uniqueAddition, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvKupdob_TEMBO_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string _kupdobCDAndName   ;
      /* B 02 */  internal string _kupdobAddrMatbrOIB;
      /* C 03 */  internal string _kupdobTelFaxGsm   ;
      /* D 04 */  internal string _kupdobZabiljeska  ;

      public override string ToString()
      {
         return
            _kupdobCDAndName    + "/" +
            _kupdobAddrMatbrOIB + "/" +
            _kupdobTelFaxGsm    + "/" +
            _kupdobZabiljeska   ;
      }

      internal uint   kupdobCD { get; set; }
      internal uint   komercCD { get; set; }
      //internal string kName { get; set; }
   }

   public VvKupdob_TEMBO_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._kupdobCDAndName    = GetString ( 1);
      rawDatStruct._kupdobAddrMatbrOIB = GetString ( 2);
      rawDatStruct._kupdobTelFaxGsm    = GetString ( 3);
      rawDatStruct._kupdobZabiljeska   = GetString ( 4);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Kupdob kupdob_rec = new Kupdob();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;
      
      //uint   currCentCD = 0, kupdobCD = 0;
      //string currCentTK = "";

      // ocisti prazne redove 
      //RawDataList.RemoveAll(raw => (raw._kupdobCDAndName + raw._kupdobAddrMatbrOIB + raw._kupdobTelFaxGsm).IsEmpty());
      List<RawData> betterRawDataList = new List<RawData>();
      RawData betterRawData = new RawData();

      string first4Chars;
      string maybeCD;
      uint iposKCD;
      bool newSuccker;
      int komercIdx;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Person>(null, VvSQL.SorterType.Code);
      Person person_rec;

      foreach(RawData raw in this.RawDataList)
      {
         if(raw.ToString().Length < 5)              continue;
         if(raw.ToString().StartsWith("Copyright")) continue;

         //first4Chars = raw._kupdobCDAndName.Length >= 4 ? raw._kupdobCDAndName.Substring(0, 4).TrimEnd(' ') : "NJET";
         //iposKCD = ZXC.ValOrZero_UInt(first4Chars);
         //if(first4Chars == iposKCD.ToString()) { newSuccker = true ; }
         //else                                  { newSuccker = false; }


         maybeCD = raw._kupdobCDAndName.Substring(0, (raw._kupdobCDAndName + " ").IndexOf(' '));
         iposKCD = ZXC.ValOrZero_UInt(maybeCD);
         if(maybeCD == iposKCD.ToString()) { newSuccker = true; }
         else                              { newSuccker = false; }

         if(newSuccker)
         {
            if(betterRawData.ToString().Length > 4) 
            {
               betterRawData._kupdobCDAndName = betterRawData._kupdobCDAndName.TrimEnd(' ');
               betterRawDataList.Add(betterRawData);
            }
            betterRawData = new RawData();

            betterRawData.kupdobCD = iposKCD;

            betterRawData._kupdobCDAndName    = raw._kupdobCDAndName   .TrimEnd(' ') + " ";
            betterRawData._kupdobAddrMatbrOIB = raw._kupdobAddrMatbrOIB.TrimEnd(' ');
            betterRawData._kupdobTelFaxGsm    = raw._kupdobTelFaxGsm   .TrimEnd(' ');
            betterRawData._kupdobZabiljeska   = raw._kupdobZabiljeska  .TrimEnd(' ');
         }
         else
         {
            betterRawData._kupdobCDAndName    += raw._kupdobCDAndName   .TrimEnd(' ') + " ";
            betterRawData._kupdobAddrMatbrOIB += raw._kupdobAddrMatbrOIB.TrimEnd(' ');
            betterRawData._kupdobTelFaxGsm    += raw._kupdobTelFaxGsm   .TrimEnd(' ');
            betterRawData._kupdobZabiljeska   += raw._kupdobZabiljeska  .TrimEnd(' ');

            if(raw._kupdobAddrMatbrOIB.Contains("Komerc:"))
            {
               komercIdx = raw._kupdobAddrMatbrOIB.IndexOf("Komerc:");

               if(komercIdx.IsZeroOrPositive() /*&& raw._kupdobAddrMatbrOIB.Substring(komercIdx).Length >= 15*/)
               {
                  betterRawData.komercCD = ZXC.ValOrZero_UInt(raw._kupdobAddrMatbrOIB.Replace("Komerc: ", "Komerc:")/*.Substring(komercIdx, 15)*/.Replace("Komerc:", ""));
               }
            }
         }

      }
      //.. Za zadnjega 
      if(betterRawData.ToString().Length > 4)
      {
         betterRawData._kupdobCDAndName = betterRawData._kupdobCDAndName.TrimEnd(' ');
         betterRawDataList.Add(betterRawData);
      }


      betterRawDataList.RemoveAll(raw => (raw._kupdobCDAndName + raw._kupdobAddrMatbrOIB + raw._kupdobTelFaxGsm).TrimEnd(' ').IsEmpty());


      uint theKCD; int oibIdx; 
      string theName, theOIB="", theUlica, theGrad, thePostaBr;
      string[] subStrings;
      foreach(RawData rawDatStruct in /*RawDataList*/betterRawDataList)
      {
         kupdob_rec.Memset0(0);

       //theCD   = ZXC.ValOrZero_UInt(rawDatStruct._kupdobCDAndName.Substring(0, 4));
         theKCD   = rawDatStruct.kupdobCD;
         theName = rawDatStruct._kupdobCDAndName.Substring(theKCD.ToString().Length).TrimStart(' ');

         kupdob_rec.KupdobCD = theKCD;
         kupdob_rec.Naziv    = LimitedStr(theName, ZXC.KpdbCI.naziv);
         kupdob_rec.Ticker   = kupdob_rec.GenerateTicker(2);

         kupdob_rec.PutnikID = rawDatStruct.komercCD;
         person_rec = VvUserControl.PersonSifrar.SingleOrDefault(person => person.PersonCD == kupdob_rec.PutnikID);
         if(person_rec != null)
         {
            kupdob_rec.PutName = person_rec.PrezimeIme;
         }

         //kupdob_rec.Napom1 = LimitedStr(theName, ZXC.KpdbCI.napom1);
         //kupdob_rec.Napom2 = LimitedStr(theName, ZXC.KpdbCI.napom2);
         kupdob_rec.Komentar = LimitedStr(rawDatStruct._kupdobAddrMatbrOIB + "\r\n" + 
                                          rawDatStruct._kupdobTelFaxGsm    + "\r\n" +
                                          rawDatStruct._kupdobZabiljeska, ZXC.KpdbCI.komentar);

         kupdob_rec.Napom2   = LimitedStr(rawDatStruct._kupdobTelFaxGsm, ZXC.KpdbCI.napom2);

         #region Ulica, Grad, PostaBr

         subStrings = rawDatStruct._kupdobAddrMatbrOIB.Split(',');
         if(subStrings.Length >= 3 && rawDatStruct._kupdobAddrMatbrOIB.Length > 8)
         {
            theUlica   = subStrings[0].TrimStart(' ');

            if(subStrings[1].TrimStart(' ').Length >= 5)
            {
               thePostaBr = subStrings[1].TrimStart(' ').Substring(0, 5);
               theGrad    = subStrings[1].TrimStart(' ').Substring(5).TrimStart(' ');
            }
            else
            {
               thePostaBr = "";
               theGrad    = "";
            }

            kupdob_rec.Grad    = LimitedStr(theGrad      , ZXC.KpdbCI.grad   );
            kupdob_rec.PostaBr = LimitedStr(thePostaBr   , ZXC.KpdbCI.postaBr);
            kupdob_rec.Ulica1  =                         
            kupdob_rec.Ulica2  = LimitedStr(theUlica     , ZXC.KpdbCI.ulica1 );
         }

         #endregion Ulica, Grad, PostaBr

         if(rawDatStruct._kupdobAddrMatbrOIB.ToLower().Contains("zagreb"))
         {
            kupdob_rec.Grad = LimitedStr("ZAGREB", ZXC.KpdbCI.grad);
         }

         // OIB da _kupdobAddrMatbrOIB 
         if(rawDatStruct._kupdobAddrMatbrOIB.ToLower().Contains("oib"))
         {
            oibIdx = rawDatStruct._kupdobAddrMatbrOIB.ToLower().IndexOf("oib");
            if(oibIdx.IsPositive() && rawDatStruct._kupdobAddrMatbrOIB.Substring(oibIdx).Length >= 15)
            {
               theOIB = rawDatStruct._kupdobAddrMatbrOIB.Replace("OIB: ", "OIB:").Substring(oibIdx, 15).Replace("OIB:", "");
               kupdob_rec.Oib = LimitedStr(theOIB, ZXC.KpdbCI.oib);
            }
         }

         //kupdob_rec.Tel1     = LimitedStr(rawDatStruct._telefon, ZXC.KpdbCI.tel1   );
         //kupdob_rec.Fax      = LimitedStr(rawDatStruct._telefax, ZXC.KpdbCI.fax    );
         //kupdob_rec.Ulica1   = 
         //kupdob_rec.Ulica2   = LimitedStr(rawDatStruct._adresa , ZXC.KpdbCI.ulica1 );
         //kupdob_rec.PostaBr  = LimitedStr(rawDatStruct._ptt    , ZXC.KpdbCI.postaBr);
         //kupdob_rec.Grad     = LimitedStr(rawDatStruct._grad   , ZXC.KpdbCI.grad   );
         //kupdob_rec.Drzava   = LimitedStr(rawDatStruct._drzava , ZXC.KpdbCI.drzava );

         //kupdob_rec.Komentar = LimitedStr(rawDatStruct._telefon + ";" + rawDatStruct._telefax + ";" + rawDatStruct._adresa, ZXC.KpdbCI.komentar);


         dbOK = kupdob_rec.VvDao.ADDREC(conn, kupdob_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvKupdob_DUCATI_Importer.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.KupdobDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvArtikl_LIKUM_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string _artiklCD   ;
      /* B 02 */  internal string _artiklName ;
      /* C 03 */  internal string _artiklName2;
      /* D 04 */  internal string _jedMj      ;
      /* E 05 */  internal string _barCode1   ;
   }

   public VvArtikl_LIKUM_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      //Artikl artikl_rec = new Artikl();
      RawData rawDatStruct = new RawData();

      rawDatStruct._artiklCD    = GetString(1);
      rawDatStruct._artiklName  = GetString(2);
      rawDatStruct._artiklName2 = GetString(3);
      rawDatStruct._jedMj       = GetString(4);
      rawDatStruct._barCode1    = GetString(5);
    //rawDatStruct._artiklOpis = GetString ( 43.Replace("^", "\r\n");
    //rawDatStruct._artiklName = GetString(2).Replace('^', ';');

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Artikl artikl_rec = new Artikl();

      //Kupdob kupdob_rec;

      bool dbOK;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         artikl_rec.Memset0(0);

         artikl_rec.ArtiklCD    = LimitedStr(rawDatStruct._artiklCD   , ZXC.ArtCI.artiklCD   );
         artikl_rec.ArtiklName  = LimitedStr(rawDatStruct._artiklName , ZXC.ArtCI.artiklName );
         artikl_rec.ArtiklName2 = LimitedStr(rawDatStruct._artiklName2, ZXC.ArtCI.artiklName2);
         artikl_rec.JedMj       = LimitedStr(rawDatStruct._jedMj      , ZXC.ArtCI.jedMj      );
         artikl_rec.BarCode1    = LimitedStr(rawDatStruct._barCode1   , ZXC.ArtCI.barCode1   );
         artikl_rec.Grupa1CD    = "BiP";
         artikl_rec.TS          = "ROB";


         //kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Naziv == rawDatStruct._ducatiCD);

         //if(kupdob_rec != null) 
         //{
         //   artikl_rec.ProizCD = kupdob_rec.KupdobCD;
         //}

         artikl_rec.PdvKat = "25";

         artikl_rec.SkladCD = "MPSK";
         
         #region ADDREC

         dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artName")) // Meaning, ADDREC failed bikoz Microline's artiklName is duplicated (artiklCD is ok, I guess?) 
         {
            artikl_rec.Napomena   = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD + ")";
            artikl_rec.ArtiklName = LimitedStrWithAddition(artikl_rec.ArtiklName, uniqueAddition, ZXC.ArtCI.artiklName);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artCD")) 
         {
            artikl_rec.Napomena = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD2 + ")";
            artikl_rec.ArtiklCD   = LimitedStrWithAddition(artikl_rec.ArtiklCD, uniqueAddition, ZXC.ArtCI.artiklCD);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         #endregion ADDREC

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvArtikl_LIKUM_Importer.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

   private string LimitedStrWithAddition(string data, string uniqueAddition, int cIdx)
   {
      return ZXC.LenLimitedStrWithAddition(data, uniqueAddition, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvArtikl_LIKUM2_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string  _artiklCD     ;
      /* B 02 */  internal string  _artiklName   ;
      /* C 03 */  internal string  _artiklTehnika;
      /* D 04 */  internal string  _artiklFormat ;
      /* E 05 */  internal string  _dobavNaziv   ;
      /* F 06 */  internal decimal _someStopa    ;
      /* G 07 */  internal decimal _kol          ;
      /* H 08 */  internal decimal _fakCij       ;
      /* I 09 */  internal decimal _nabCij       ;
      /* J 10 */  internal decimal _malCij       ;
      /* K 11 */  internal string  _lk_nf        ;
   }

   public VvArtikl_LIKUM2_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      //Artikl artikl_rec = new Artikl();
      RawData rawDatStruct = new RawData();

      rawDatStruct._artiklCD      = GetString ( 1);
      rawDatStruct._artiklName    = GetString ( 2);
      rawDatStruct._artiklTehnika = GetString ( 3);
      rawDatStruct._artiklFormat  = GetString ( 4);
      rawDatStruct._dobavNaziv    = GetString ( 5);
      rawDatStruct._someStopa     = GetDecimal( 6);
      rawDatStruct._kol           = GetDecimal( 7);
      rawDatStruct._fakCij        = GetDecimal( 8);
      rawDatStruct._nabCij        = GetDecimal( 9);
      rawDatStruct._malCij        = GetDecimal(10);
      rawDatStruct._lk_nf         = GetString (11);
    //rawDatStruct._artiklOpis = GetString ( 43.Replace("^", "\r\n");
    //rawDatStruct._artiklName = GetString(2).Replace('^', ';');

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Artikl artikl_rec = new Artikl();
      Kupdob kupdob_rec;

      bool dbOK;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         artikl_rec.Memset0(0);

         artikl_rec.ArtiklCD   = LimitedStr(rawDatStruct._lk_nf      + rawDatStruct._artiklCD.Replace("0000", "")                               , ZXC.ArtCI.artiklCD   );
         artikl_rec.ArtiklName = LimitedStr(Artikl.CreateUmjetninaNaziv(rawDatStruct._artiklName, artikl_rec.ArtiklCD, rawDatStruct._dobavNaziv), ZXC.ArtCI.artiklName);

         artikl_rec.PdvKat   = "25";
         artikl_rec.SkladCD  = "LIKUM";
         artikl_rec.TS       = "ROB";
         artikl_rec.Grupa1CD = "UMJ";
         artikl_rec.JedMj    = "KOM";
         artikl_rec.Grupa2CD = LimitedStr(GetGrupa2CD(rawDatStruct._artiklTehnika), ZXC.ArtCI.grupa2CD);
         artikl_rec.Velicina = LimitedStr(rawDatStruct._artiklFormat, ZXC.ArtCI.velicina);
       //artikl_rec.BarCode1 = LimitedStr(                                              );

         kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Naziv == ZXC.VvTranslate437ToLatin2(rawDatStruct._dobavNaziv));

         if(kupdob_rec != null)
         {
            artikl_rec.ProizCD = kupdob_rec.KupdobCD;
         }

         #region ADDREC

         dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artName")) // Meaning, ADDREC failed bikoz Microline's artiklName is duplicated (artiklCD is ok, I guess?) 
         {
            artikl_rec.Napomena   = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD + ")";
            artikl_rec.ArtiklName = LimitedStrWithAddition(artikl_rec.ArtiklName, uniqueAddition, ZXC.ArtCI.artiklName);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artCD")) 
         {
            artikl_rec.Napomena = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD2 + ")";
            artikl_rec.ArtiklCD   = LimitedStrWithAddition(artikl_rec.ArtiklCD, uniqueAddition, ZXC.ArtCI.artiklCD);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         #endregion ADDREC

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvArtikl_LIKUM_Importer.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

   private string LimitedStrWithAddition(string data, string uniqueAddition, int cIdx)
   {
      return ZXC.LenLimitedStrWithAddition(data, uniqueAddition, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

   private string GetGrupa2CD(string excelTehnika) 
   {
      switch(excelTehnika)
      {
         case "ŠAMOT"                      : return "SAM"; //ŠAMOT                      
         case "AKRIL"                      : return "AKR"; //AKRIL                      
         case "AKRIL NA PLATNU"            : return "AKP"; //AKRIL NA PLATNU            
         case "AKRIL/PLATNO"               : return "AKP"; //AKRIL NA PLATNU            
         case "AKVAREL"                    : return "AKV"; //AKVAREL                    
         case "AUTORSKA ČESTITKA"          : return "ACE"; //AUTORSKA ČESTITKA          
         case "AUTORSKI PRINT"             : return "APR"; //AUTORSKI PRINT             
         case "BATIK"                      : return "BAT"; //BATIK                      
         case "BRONCA"                     : return "BRO"; //BRONCA                     
         case "BRONCA-DRVO-KAMEN"          : return "BDK"; //BRONCA-DRVO-KAMEN          
         case "BRONCA-KAMEN"               : return "BRK"; //BRONCA-KAMEN               
         case "CRTEŽ"                      : return "CRT"; //CRTEŽ                      
         case "DORAĐENA GRAFIKA"           : return "GRD"; //DORAĐENA GRAFIKA           
         case "DRVO"                       : return "DRV"; //DRVO                       
         case "DRVO,BRONCA"                : return "DBR"; //DRVO-BRONCA                
         case "DRVO-BRONCA"                : return "DBR"; //DRVO-BRONCA                
         case "FOTOGRAFIJA"                : return "FOT"; //FOTOGRAFIJA                
         case "GRAFIKA"                    : return "GRF"; //GRAFIKA                    
         case "GRAFIKE"                    : return "GRF"; //GRAFIKA                    
         case "GVAŠ"                       : return "GVA"; //GVAŠ                       
         case "KAMEN I BRONCA"             : return "BRK"; //BRONCA-KAMEN               
         case "KERAIKA"                    : return "KER"; //KERAMIKA                   
         case "KERAM."                     : return "KER"; //KERAMIKA                   
         case "KERAMIKA"                   : return "KER"; //KERAMIKA                   
         case "KERAMIKA DJELO.GLAZIRANA"   : return "KDG"; //KERAMIKA DJELO.GLAZIRANA   
         case "KERAMIKA DJELOM,GLAZIRANA"  : return "KDG"; //KERAMIKA DJELO.GLAZIRANA   
         case "KERAMIKA GL. ZL."           : return "KGZ"; //KERAMIKA GL. ZL.           
         case "KERAMIKA GLAZ. POZL."       : return "KGZ"; //KERAMIKA GL. ZL.           
         case "KERAMIKA GLAZIRANA"         : return "KGL"; //KERAMIKA GLAZIRANA         
         case "KERAMIKA, UNIKAT"           : return "KUN"; //KERAMIKA, UNIKAT           
         case "KERAMIKA. UNIKAT"           : return "KUN"; //KERAMIKA, UNIKAT           
         case "KERAMKIKA"                  : return "KER"; //KERAMIKA                   
         case "KERAMOSKULPTURA"            : return "KSK"; //KERAMOSKULPTURA            
         case "KOMB."                      : return "KOT"; //KOMBINIRANA TEHNIKA        
         case "KOMBI,TEHNIKA"              : return "KOT"; //KOMBINIRANA TEHNIKA        
         case "KOMBINIRANA"                : return "KOT"; //KOMBINIRANA TEHNIKA        
         case "KOMBINIRANA TEHNIKA"        : return "KOT"; //KOMBINIRANA TEHNIKA        
         case "KQRAMIKA"                   : return "KER"; //KERAMIKA                   
         case "MEKANI PORCULAN"            : return "MPO"; //MEKANI PORCULAN            
         case "MODELIRANO STAKLO"          : return "MST"; //MODELIRANO STAKLO          
         case "MODELIRANO STAKLO + ZL/PAT" : return "MSZ"; //MODELIRANO STAKLO + ZL/PAT 
         case "ODLJEV"                     : return "ODL"; //ODLJEV                     
         case "PASTEL"                     : return "PAS"; //PASTEL                     
         case "RAKU KERAMIKA"              : return "KRA"; //RAKU KERAMIKA              
         case "REPRODUKCIJA"               : return "REP"; //REPRODUKCIJA               
         case "SERIGRAFIJA"                : return "SER"; //SERIGRAFIJA                
         case "SKULPTURA. DRVO"            : return "SKD"; //SKULPTURA DRVO             
         case "STAKLO"                     : return "STA"; //STAKLO                     
         case "TEKSTIL"                    : return "TEK"; //TEKSTIL                    
         case "TEMPERA"                    : return "TEM"; //TEMPERA                    
         case "TISAK"                      : return "TIS"; //TISAK                      
         case "TMS_TEHNIK"                 : return "TMS"; //TMS_TEHNIK                 
         case "ULJE"                       : return "ULJ"; //ULJE                       
         case "ULJE NA KARTONU"            : return "UKA"; //ULJE NA KARTONU            
         case "ULJE NA PLATNU"             : return "UPL"; //ULJE NA PLATNU             
         case "ULJE/PLOČA"                 : return "UPO"; //ULJE/PLOČA                 

         default: return "";

      }
   }

}

public class VvKupdob_LIKUM_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string _tip        ;
      /* B 01 */  internal uint   _kupdobCD   ;
      /* C 02 */  internal string _kupdobName1;
      /* D 02 */  internal string _kupdobName2;
      /* E 03 */  internal string _grad       ;
      /* F 04 */  internal string _postaBr    ;
      /* G 05 */  internal string _ulica1     ;
      /* H 06 */  internal string _tel1       ;
      /* I 07 */  internal string _tel2       ;
      /* J 08 */  internal string _fax        ;
      /* K 09 */  internal string _ziro1      ;
      /* L 10 */  internal string _ziro2      ;
      /* M 11 */  internal string _matbr      ;
      /* N 12 */  internal string _oib        ;
      /* O 13 */  internal string _prezime    ;
   }

   public VvKupdob_LIKUM_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._tip         = GetString ( 1);
      rawDatStruct._kupdobCD    = GetUint32 ( 2);
      rawDatStruct._kupdobName1 = GetString ( 3);
      rawDatStruct._kupdobName2 = GetString ( 4);
      rawDatStruct._grad        = GetString ( 5);
      rawDatStruct._postaBr     = GetString ( 6);
      rawDatStruct._ulica1      = GetString ( 7);
      rawDatStruct._tel1        = GetString ( 8);
      rawDatStruct._tel2        = GetString ( 9);
      rawDatStruct._fax         = GetString (10);
      rawDatStruct._ziro1       = GetString (11);
      rawDatStruct._ziro2       = GetString (12);
      rawDatStruct._matbr       = GetString (13);
      rawDatStruct._oib         = GetString (14);
      rawDatStruct._prezime     = GetString (15);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Kupdob kupdob_rec = new Kupdob();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      uint   currCentCD = 0, kupdobCD = 0;;
      string currCentTK = "";

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         kupdob_rec.Memset0(0);

         kupdob_rec.KupdobCD = rawDatStruct._kupdobCD + (rawDatStruct._tip == "K" ? 10000u : 20000u);
         kupdob_rec.Naziv    = LimitedStr(rawDatStruct._kupdobName1 + " " + rawDatStruct._kupdobName2, ZXC.KpdbCI.naziv);
         kupdob_rec.Ticker   = kupdob_rec.GenerateTicker(2);

         kupdob_rec.Tel1     = LimitedStr(rawDatStruct._tel1   , ZXC.KpdbCI.tel1   );
         kupdob_rec.Tel2     = LimitedStr(rawDatStruct._tel2   , ZXC.KpdbCI.tel2   );
         kupdob_rec.Fax      = LimitedStr(rawDatStruct._fax    , ZXC.KpdbCI.fax    );
         kupdob_rec.Ulica1   = 
         kupdob_rec.Ulica2   = LimitedStr(rawDatStruct._ulica1 , ZXC.KpdbCI.ulica1 );
         kupdob_rec.PostaBr  = LimitedStr(rawDatStruct._postaBr, ZXC.KpdbCI.postaBr);
         kupdob_rec.Grad     = LimitedStr(rawDatStruct._grad   , ZXC.KpdbCI.grad   );
         kupdob_rec.Tip      = LimitedStr(rawDatStruct._tip    , ZXC.KpdbCI.tip    );
         kupdob_rec.Ziro1    = LimitedStr(rawDatStruct._ziro1  , ZXC.KpdbCI.ziro1  );
         kupdob_rec.Ziro2    = LimitedStr(rawDatStruct._ziro2  , ZXC.KpdbCI.ziro2  );
         kupdob_rec.Matbr    = LimitedStr(rawDatStruct._matbr  , ZXC.KpdbCI.matbr  );
         kupdob_rec.Oib      = LimitedStr(rawDatStruct._oib    , ZXC.KpdbCI.oib    );
         kupdob_rec.Prezime  = LimitedStr(rawDatStruct._prezime, ZXC.KpdbCI.prezime);

         currCentCD = kupdob_rec.KupdobCD;
         currCentTK = kupdob_rec.Ticker;

         dbOK = kupdob_rec.VvDao.ADDREC(conn, kupdob_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvKupdob_LIKUM_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.KupdobDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvKupdob_LIKUM2_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal uint   _kupdobCD   ;
      /* B 02 */  internal string _kupdobCDold;
      /* C 02 */  internal string _kupdobName ;
      /* D 05 */  internal string _ulica1     ;
      /* E 04 */  internal string _postaBr    ;
      /* F 03 */  internal string _grad       ;
      /* G 11 */  internal string _matbr      ;
      /* H 12 */  internal string _oib        ;
      /* I 10 */  internal string _osoba      ;
      /* J 13 */  internal string _imePrezime ;
      /* K 06 */  internal string _tel1       ;
      /* L 08 */  internal string _fax        ;
      /* M 07 */  internal string _gsm        ;
      /* N 09 */  internal string _ziro       ;
      /* O 09 */  internal string _lk_nf      ;
   }

   public VvKupdob_LIKUM2_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._kupdobCD    = GetUint32 ( 1);
      rawDatStruct._kupdobCDold = GetString ( 2);
      rawDatStruct._kupdobName  = GetString ( 3);
      rawDatStruct._ulica1      = GetString ( 4);
      rawDatStruct._postaBr     = GetString ( 5);
      rawDatStruct._grad        = GetString ( 6);
      rawDatStruct._matbr       = GetString ( 7);
      rawDatStruct._oib         = GetString ( 8);
      rawDatStruct._osoba       = GetString ( 9);
      rawDatStruct._imePrezime  = GetString (10);
      rawDatStruct._tel1        = GetString (11);
      rawDatStruct._fax         = GetString (12);
      rawDatStruct._gsm         = GetString (13);
      rawDatStruct._ziro        = GetString (14);
      rawDatStruct._lk_nf       = GetString (15);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Kupdob kupdob_rec = new Kupdob();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      uint   currCentCD = 0, kupdobCD = 0;;
      string currCentTK = "";

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         kupdob_rec.Memset0(0);

         kupdob_rec.KupdobCD = rawDatStruct._kupdobCD;
         kupdob_rec.Naziv    = (LimitedStr(rawDatStruct._kupdobName, ZXC.KpdbCI.naziv));
         kupdob_rec.Ticker   = (kupdob_rec.GenerateTicker(2));

         kupdob_rec.Tel1     =  LimitedStr(rawDatStruct._tel1      , ZXC.KpdbCI.tel1   ) ;
         kupdob_rec.Fax      =  LimitedStr(rawDatStruct._fax       , ZXC.KpdbCI.fax    ) ;
         kupdob_rec.Gsm      =  LimitedStr(rawDatStruct._gsm       , ZXC.KpdbCI.gsm    ) ;
         kupdob_rec.Ulica1   =          
         kupdob_rec.Ulica2   = (LimitedStr(rawDatStruct._ulica1    , ZXC.KpdbCI.ulica1 ));
         kupdob_rec.PostaBr  =  LimitedStr(rawDatStruct._postaBr   , ZXC.KpdbCI.postaBr) ;
         kupdob_rec.Grad     = (LimitedStr(rawDatStruct._grad      , ZXC.KpdbCI.grad   ));
         if(FullPathFileName.ToUpper().Contains("AUTOR"))
         {
         kupdob_rec.Tip      = LimitedStr("A"                     , ZXC.KpdbCI.tip    ) ;
         }
         kupdob_rec.Ziro1    = LimitedStr(rawDatStruct._ziro      , ZXC.KpdbCI.ziro1  ) ;
         kupdob_rec.Matbr    = LimitedStr(rawDatStruct._matbr     , ZXC.KpdbCI.matbr  ) ;
         kupdob_rec.Oib      = LimitedStr(rawDatStruct._oib       , ZXC.KpdbCI.oib    ) ;
         kupdob_rec.Ime      = LimitedStr(rawDatStruct._imePrezime, ZXC.KpdbCI.prezime) ;
         kupdob_rec.Napom1   = LimitedStr(rawDatStruct._lk_nf + kupdob_rec.Tip + "-" + rawDatStruct._kupdobCDold, ZXC.KpdbCI.napom1);
         kupdob_rec.IsZzz    =           (rawDatStruct._osoba.ToUpper().StartsWith("D"));

         currCentCD = kupdob_rec.KupdobCD;
         currCentTK = kupdob_rec.Ticker;

         dbOK = kupdob_rec.VvDao.ADDREC(conn, kupdob_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvKupdob_LIKUM_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.KupdobDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvArtikl_VERIDI_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string  _artGrCD    ;
      /* B 01 */  internal string  _artiklCD   ;
      /* C 01 */  internal string  _artiklCD2  ;
      /* D 02 */  internal string  _artiklName ;
      /* E 03 */  internal decimal _theVPC     ;
      /* D 02 */  internal string  _artiklOpis ;
   }

   public VvArtikl_VERIDI_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected /*override*/ void ProcessLineOLD(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      //Artikl artikl_rec = new Artikl();
      RawData rawDatStruct = new RawData();

      rawDatStruct._artGrCD     = GetString (1);
      rawDatStruct._artiklCD    = GetString (2);
      rawDatStruct._artiklCD2   = GetString (3);
      rawDatStruct._artiklName  = GetString (4);
      rawDatStruct._theVPC      = GetDecimal(5);
    //rawDatStruct._artiklOpis = GetString ( 43.Replace("^", "\r\n");
    //rawDatStruct._artiklName = GetString(2).Replace('^', ';');

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      //Artikl artikl_rec = new Artikl();
      RawData rawDatStruct = new RawData();

      rawDatStruct._artGrCD     = GetString (1);
      rawDatStruct._artiklCD    = GetString (2);
      rawDatStruct._artiklName  = GetString (3);
      rawDatStruct._artiklOpis  = GetString (4);
      rawDatStruct._theVPC      = GetDecimal(5);
    //rawDatStruct._artiklName = GetString(2).Replace('^', ';');

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Artikl artikl_rec = new Artikl();

      //Kupdob kupdob_rec;

      bool dbOK;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         artikl_rec.Memset0(0);

         artikl_rec.Grupa1CD    = LimitedStr(rawDatStruct._artGrCD    , ZXC.ArtCI.grupa1CD  );
         artikl_rec.ArtiklCD    = LimitedStr(rawDatStruct._artiklCD   , ZXC.ArtCI.artiklCD  );
         artikl_rec.LongOpis    = LimitedStr(rawDatStruct._artiklOpis , ZXC.ArtCI.longOpis  );
         artikl_rec.ArtiklName  = LimitedStr(rawDatStruct._artiklName , ZXC.ArtCI.artiklName);
         artikl_rec.ImportCij   =           (rawDatStruct._theVPC                           );
         artikl_rec.JedMj       = "kom";
         artikl_rec.TS          = "ROB";
         artikl_rec.SkladCD     = "SVPS";
         artikl_rec.PdvKat      = "25";


         //kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Naziv == rawDatStruct._ducatiCD);

         //if(kupdob_rec != null) 
         //{
         //   artikl_rec.ProizCD = kupdob_rec.KupdobCD;
         //}

         #region ADDREC

         dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artName")) // Meaning, ADDREC failed bikoz Microline's artiklName is duplicated (artiklCD is ok, I guess?) 
         {
            artikl_rec.Napomena   = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD + ")";
            artikl_rec.ArtiklName = LimitedStrWithAddition(artikl_rec.ArtiklName, uniqueAddition, ZXC.ArtCI.artiklName);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artCD")) 
         {
            artikl_rec.Napomena = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD2 + ")";
            artikl_rec.ArtiklCD   = LimitedStrWithAddition(artikl_rec.ArtiklCD, uniqueAddition, ZXC.ArtCI.artiklCD);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         #endregion ADDREC

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvArtikl_LIKUM_Importer.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

   private string LimitedStrWithAddition(string data, string uniqueAddition, int cIdx)
   {
      return ZXC.LenLimitedStrWithAddition(data, uniqueAddition, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvArtikl_TEMBO_Importer  : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string  _artiklCD       ;
      /* B 02 */  internal string  _grupa          ;
      /* C 03 */  internal string  _artiklNameTembo;
      /* D 04 */  internal decimal _origPak        ;
      /* E 05 */  internal string  _isKg           ;
   }

   public VvArtikl_TEMBO_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      //Artikl artikl_rec = new Artikl();
      RawData rawDatStruct = new RawData();

      rawDatStruct._artiklCD        = GetString (1);
      rawDatStruct._grupa           = GetString (2);
      rawDatStruct._artiklNameTembo = GetString (3);
      rawDatStruct._origPak         = GetDecimal(4);
      rawDatStruct._isKg            = GetString (5);
    //rawDatStruct._artiklOpis = GetString ( 43.Replace("^", "\r\n");
    //rawDatStruct._artiklName = GetString(2).Replace('^', ';');

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Artikl artikl_rec = new Artikl();

      //Kupdob kupdob_rec;

      bool dbOK;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         artikl_rec.Memset0(0);

         artikl_rec.ArtiklCD    = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.artiklCD );

         artikl_rec.TS          = "ROB" ;
         artikl_rec.PdvKat      = "25"  ;
         artikl_rec.SkladCD     = "VPSK";
         artikl_rec.JedMj       = "kom" ;
         artikl_rec.Zapremina   = rawDatStruct._origPak;

         artikl_rec.ZapreminaJM = "L";
         if(rawDatStruct._isKg.NotEmpty())
         {
            artikl_rec.ZapreminaJM = "KG";
         }

         artikl_rec.Grupa1CD    = LimitedStr(rawDatStruct._grupa          , ZXC.ArtCI.grupa1CD   );
         artikl_rec.ArtiklName  = 
         artikl_rec.ArtiklName2 = LimitedStr(rawDatStruct._artiklNameTembo, ZXC.ArtCI.artiklName2);

         if(rawDatStruct._artiklNameTembo.ToUpper().Contains("1 L") ||
            rawDatStruct._artiklNameTembo.ToUpper().Contains("1L")  ||
            rawDatStruct._artiklNameTembo.ToUpper().Contains("2L")  ||
            rawDatStruct._artiklNameTembo.ToUpper().Contains("2 L") ||
            rawDatStruct._artiklNameTembo.ToUpper().Contains("5 L") ||
            rawDatStruct._artiklNameTembo.ToUpper().Contains("5L")  ||
            rawDatStruct._artiklNameTembo.ToUpper().Contains("4L")  ||
            rawDatStruct._artiklNameTembo.ToUpper().Contains("20L")  ||
            rawDatStruct._artiklNameTembo.ToUpper().Contains("60L")  ||
            rawDatStruct._artiklNameTembo.ToUpper().Contains("25 L")  ||
            rawDatStruct._artiklNameTembo.ToUpper().Contains("4.5 L")  ||
            rawDatStruct._artiklNameTembo.ToUpper().Contains("208L")  ||
            rawDatStruct._artiklNameTembo.ToUpper().Contains("KG"))
         {
            artikl_rec.ArtiklName = LimitedStr(rawDatStruct._artiklNameTembo, ZXC.ArtCI.artiklName);
         }
         else
         {
            artikl_rec.ArtiklName  = LimitedStr(rawDatStruct._artiklNameTembo + 
                                                " "                           + 
                                                artikl_rec.Zapremina          +
                                                "" +
                                                artikl_rec.ZapreminaJM, ZXC.ArtCI.artiklName);
         }
         //kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Naziv == rawDatStruct._ducatiCD);

         //if(kupdob_rec != null) 
         //{
         //   artikl_rec.ProizCD = kupdob_rec.KupdobCD;
         //}

         #region ADDREC

         dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artName")) // Meaning, ADDREC failed bikoz Microline's artiklName is duplicated (artiklCD is ok, I guess?) 
         {
            artikl_rec.Napomena   = LimitedStr(rawDatStruct._artiklNameTembo, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD + ")";
            artikl_rec.ArtiklName = LimitedStrWithAddition(artikl_rec.ArtiklName, uniqueAddition, ZXC.ArtCI.artiklName);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artCD")) 
         {
            artikl_rec.Napomena = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD2 + ")";
            artikl_rec.ArtiklCD   = LimitedStrWithAddition(artikl_rec.ArtiklCD, uniqueAddition, ZXC.ArtCI.artiklCD);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         #endregion ADDREC

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvArtikl_DUCATI_Importer.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

   private string LimitedStrWithAddition(string data, string uniqueAddition, int cIdx)
   {
      return ZXC.LenLimitedStrWithAddition(data, uniqueAddition, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvKupdob_JOSAVAC_Importer: VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal uint   _kupdobCD   ;
      /* B 02 */  internal string _kupdobName ;
      /* C 03 */  internal string _napomena1  ;
      /* D 04 */  internal string _ziro       ;
      /* E 05 */  internal string _postaBr    ;
      /* F 06 */  internal string _grad       ;
      /* G 07 */  internal string _ulica1     ;
      /* H 08 */  internal string _tel1       ;
      /* I 09 */  internal string _tel2       ;
      /* J 10 */  internal string _konto      ;
      /* K 11 */  internal string _matbrOib   ;
  }

   public VvKupdob_JOSAVAC_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._kupdobCD    = GetUint32 ( 1);
      rawDatStruct._kupdobName  = GetString ( 2);
      rawDatStruct._napomena1   = GetString ( 3);
      rawDatStruct._ziro        = GetString ( 4);
      rawDatStruct._postaBr     = GetString ( 5);
      rawDatStruct._grad        = GetString ( 6);
      rawDatStruct._ulica1      = GetString ( 7);
      rawDatStruct._tel1        = GetString ( 8);
      rawDatStruct._tel2        = GetString ( 9);
      rawDatStruct._konto       = GetString (10);
      rawDatStruct._matbrOib    = GetString (11);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Kupdob kupdob_rec = new Kupdob();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      uint   currCentCD = 0, kupdobCD = 0;;
      string currCentTK = "";

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         kupdob_rec.Memset0(0);

         kupdob_rec.KupdobCD = rawDatStruct._kupdobCD;
         kupdob_rec.Naziv    = (LimitedStr(rawDatStruct._kupdobName, ZXC.KpdbCI.naziv));
         kupdob_rec.Ticker   = (kupdob_rec.GenerateTicker(2));

         kupdob_rec.Tel1     =  LimitedStr(rawDatStruct._tel1      , ZXC.KpdbCI.tel1   ) ;
         kupdob_rec.Tel2     =  LimitedStr(rawDatStruct._tel2      , ZXC.KpdbCI.tel2   ) ;
         kupdob_rec.Napom1   =  LimitedStr(rawDatStruct._napomena1 , ZXC.KpdbCI.napom1 ) ;
         kupdob_rec.Ulica1   =          
         kupdob_rec.Ulica2   = (LimitedStr(rawDatStruct._ulica1    , ZXC.KpdbCI.ulica1 ));
         kupdob_rec.PostaBr  =  LimitedStr(rawDatStruct._postaBr   , ZXC.KpdbCI.postaBr) ;
         kupdob_rec.Grad     = (LimitedStr(rawDatStruct._grad      , ZXC.KpdbCI.grad   ));
         kupdob_rec.Ziro1    = LimitedStr(rawDatStruct._ziro       , ZXC.KpdbCI.ziro1  ) ;

         if(rawDatStruct._konto.Length.NotZero() && rawDatStruct._konto.StartsWith("22"))
         {
            kupdob_rec.KontoDug = LimitedStr(rawDatStruct._konto.Substring(0, 4), ZXC.KpdbCI.oib);
         }
         else if(rawDatStruct._konto.Length.NotZero() && rawDatStruct._konto.StartsWith("12"))
         {
            kupdob_rec.KontoPot = LimitedStr(rawDatStruct._konto.Substring(0, 4), ZXC.KpdbCI.oib);
         }

         if(rawDatStruct._matbrOib.Length == 11)
         {
            kupdob_rec.Oib = LimitedStr(rawDatStruct._matbrOib, ZXC.KpdbCI.oib);
         }
         else // MatBr 
         {
            kupdob_rec.Matbr = LimitedStr(rawDatStruct._matbrOib, ZXC.KpdbCI.matbr);
         }


         currCentCD = kupdob_rec.KupdobCD;
         currCentTK = kupdob_rec.Ticker;

         dbOK = kupdob_rec.VvDao.ADDREC(conn, kupdob_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvKupdob_LIKUM_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.KupdobDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvArtikl_JOSAVAC_Importer: VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string  _artiklCD  ;
      /* B 03 */  internal string  _artiklName;
      /* C 05 */  internal string  _jedMj     ;
      /* D 02 */  internal string  _grupa1cd  ;
   }

   public VvArtikl_JOSAVAC_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      //Artikl artikl_rec = new Artikl();
      RawData rawDatStruct = new RawData();

      rawDatStruct._artiklCD   = GetString (1);
      rawDatStruct._artiklName = GetString (2);
      rawDatStruct._jedMj      = GetString (3);
    //rawDatStruct._grupa1cd   = GetString (5);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Artikl artikl_rec = new Artikl();

      //Kupdob kupdob_rec;

      bool dbOK;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         artikl_rec.Memset0(0);

         artikl_rec.ArtiklCD   = LimitedStr(rawDatStruct._artiklCD  , ZXC.ArtCI.artiklCD  );
         artikl_rec.ArtiklName = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.artiklName);
         artikl_rec.JedMj      = LimitedStr(rawDatStruct._jedMj     , ZXC.ArtCI.jedMj     );
         artikl_rec.Grupa1CD   = LimitedStr(rawDatStruct._grupa1cd  , ZXC.ArtCI.grupa1CD  );

         artikl_rec.TS          = "ROB" ;
         artikl_rec.PdvKat      = "25"  ;
         artikl_rec.SkladCD     = ""/*"VPSK"*/;

         //kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Naziv == rawDatStruct._ducatiCD);

         //if(kupdob_rec != null) 
         //{
         //   artikl_rec.ProizCD = kupdob_rec.KupdobCD;
         //}

         #region ADDREC

         dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artName")) // Meaning, ADDREC failed bikoz Microline's artiklName is duplicated (artiklCD is ok, I guess?) 
         {
            artikl_rec.Napomena   = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD + ")";
            artikl_rec.ArtiklName = LimitedStrWithAddition(artikl_rec.ArtiklName, uniqueAddition, ZXC.ArtCI.artiklName);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artCD")) 
         {
            artikl_rec.Napomena = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD2 + ")";
            artikl_rec.ArtiklCD   = LimitedStrWithAddition(artikl_rec.ArtiklCD, uniqueAddition, ZXC.ArtCI.artiklCD);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         #endregion ADDREC

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvArtikl_JOSAVC_Importer.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

   private string LimitedStrWithAddition(string data, string uniqueAddition, int cIdx)
   {
      return ZXC.LenLimitedStrWithAddition(data, uniqueAddition, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvKupdob_HZTK_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string _kupdobCD   ;
      /* B 02 */  internal string _kupdobName ;
      /* C 03 */  internal string _ulica1     ;
      /* D 04 */  internal string _zipAndGrad ;
      /* E 05 */  internal string _kontakt    ;
      /* F 06 */  internal string _tel1       ;
      /* G 07 */  internal string _fax        ;
      /* H 08 */  internal string _ziro1      ;
      /* P 16 */  internal string _matbr      ;
      /* Q 17 */  internal string _oib        ;

   }

   public VvKupdob_HZTK_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._kupdobCD    = GetString ( 1);
      rawDatStruct._kupdobName  = GetString ( 2);
      rawDatStruct._ulica1      = GetString ( 3);
      rawDatStruct._zipAndGrad  = GetString ( 4);
      rawDatStruct._kontakt     = GetString ( 5);
      rawDatStruct._tel1        = GetString ( 6);
      rawDatStruct._fax         = GetString ( 7);
      rawDatStruct._ziro1       = GetString ( 8);
      rawDatStruct._matbr       = GetString (16);
      rawDatStruct._oib         = GetString (17);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Kupdob kupdob_rec = new Kupdob();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      uint    theKCD;
      uint   origKCDuint;
      string origKCDstr ;
      bool zipAndGradStartsWithZIP;

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         if(rawDatStruct._kupdobName.StartsWith("*")) continue;

         kupdob_rec.Memset0(0);

         // theKCD start 
         origKCDstr = rawDatStruct._kupdobCD;
         origKCDuint = ZXC.ValOrZero_UInt(origKCDstr);
         if(origKCDstr.Length < 3)  theKCD = (origKCDuint + 2900);
         else if(origKCDuint < 100) theKCD = (origKCDuint + 2000);
         else                       theKCD = origKCDuint;
       //kupdob_rec.KupdobCD = theKCD; ovo ide u prvom prolazu 
         kupdob_rec.KupdobCD = origKCDuint;
         // theKCD end 

         zipAndGradStartsWithZIP = IsThis_zipAndGradStartsWithZIP(rawDatStruct._zipAndGrad);
         if(zipAndGradStartsWithZIP)
         {
            kupdob_rec.PostaBr  = LimitedStr(rawDatStruct._zipAndGrad.Substring(0, 5), ZXC.KpdbCI.postaBr);
            if(rawDatStruct._zipAndGrad.Length > 6)
               kupdob_rec.Grad  = LimitedStr(rawDatStruct._zipAndGrad.Substring(6)   , ZXC.KpdbCI.grad   );
         }
         else
         {
            kupdob_rec.Grad = LimitedStr(rawDatStruct._zipAndGrad, ZXC.KpdbCI.grad);
            if(kupdob_rec.Grad.ToUpper().Contains("ZAGREB"))
               kupdob_rec.PostaBr = "10000";
         }

         // direct, no bullshit 
         kupdob_rec.Naziv    = LimitedStr(rawDatStruct._kupdobName, ZXC.KpdbCI.naziv);
         kupdob_rec.Ticker   = kupdob_rec.GenerateTicker(4);
         kupdob_rec.Ulica1   = 
         kupdob_rec.Ulica2   = LimitedStr(rawDatStruct._ulica1 , ZXC.KpdbCI.ulica1 );
         kupdob_rec.Prezime  = LimitedStr(rawDatStruct._kontakt, ZXC.KpdbCI.prezime);
         kupdob_rec.Tel1     = LimitedStr(rawDatStruct._tel1   , ZXC.KpdbCI.tel1   );
         kupdob_rec.Fax      = LimitedStr(rawDatStruct._fax    , ZXC.KpdbCI.fax    );
         kupdob_rec.Ziro1    = LimitedStr(rawDatStruct._ziro1  , ZXC.KpdbCI.ziro1  );
         kupdob_rec.Matbr    = LimitedStr(rawDatStruct._matbr  , ZXC.KpdbCI.matbr  );
         kupdob_rec.Oib      = LimitedStr(rawDatStruct._oib    , ZXC.KpdbCI.oib    );

         // ovo ide SAMO kod drugog 'mtr' prolaska 
         
         kupdob_rec.IsMtr = true;

         dbOK = kupdob_rec.VvDao.ADDREC(conn, kupdob_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvKupdob_LIKUM_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private bool IsThis_zipAndGradStartsWithZIP(string zipAndGrad)
   {
      if(zipAndGrad.Length < 5) return false;

      string maybeZIPstr = zipAndGrad.Substring(0, 5);
      int   maybeZIPint  = ZXC.ValOrZero_Int(maybeZIPstr);
      string reconverted = maybeZIPint.ToString();

      return maybeZIPstr == reconverted;
   } 

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.KupdobDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvOsred_HZTK_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string _osredCD  ;
      /* B 02 */  internal string _osredName;
      /* C 03 */  internal string _konto    ;
      /* D 04 */  internal string _mtros    ;
      /* F 06 */  internal string _dateNab  ;
      /* M 13 */  internal decimal _stopaAm ;

   }

   public VvOsred_HZTK_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._osredCD     = GetString ( 1);
      rawDatStruct._osredName   = GetString ( 2);
      rawDatStruct._konto       = GetString ( 3);
    //rawDatStruct._mtros       = GetString ( 4);
      rawDatStruct._mtros       = "OS" + GetString (4).Replace(@"/", "");
      rawDatStruct._dateNab     = GetString ( 6);
      rawDatStruct._stopaAm     = GetDecimal(13);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Osred osred_rec = new Osred();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.None);
      Kupdob kupdob_rec;

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         if(rawDatStruct._osredName.StartsWith("*")) continue;

         osred_rec.Memset0(0);

         // direct, no bullshit 
         osred_rec.OsredCD = LimitedStr(rawDatStruct._osredCD  , ZXC.OsrCI.osredCD );
         osred_rec.Naziv   = LimitedStr(rawDatStruct._osredName, ZXC.OsrCI.naziv   );
         osred_rec.Konto   = LimitedStr(rawDatStruct._konto    , ZXC.OsrCI.konto   );
         osred_rec.DokumCd = LimitedStr(rawDatStruct._dateNab  , ZXC.OsrCI.dokum_cd);
         osred_rec.AmortSt = rawDatStruct._stopaAm;

         osred_rec.Vijek   = ZXC.DivSafe(100, osred_rec.AmortSt);

         if(rawDatStruct._mtros.NotEmpty())
         {
            int slashIdx = rawDatStruct._mtros.IndexOf('/');

            if(slashIdx.IsNegative()) kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(k => k.Ticker == rawDatStruct._mtros                       );
            else                      kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(k => k.Ticker == rawDatStruct._mtros.Substring(0, slashIdx));

            if(kupdob_rec != null)
            {
               osred_rec.MtrosCd = kupdob_rec.KupdobCD;
               osred_rec.MtrosTk = kupdob_rec.Ticker;
            }
            else
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Nema mtrosa [{0}]\n\nOsred [{1}]", rawDatStruct._mtros, osred_rec.OsredCD);
            }
         }

         dbOK = osred_rec.VvDao.ADDREC(conn, osred_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvOsred_LIKUM_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.OsredDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvKupdob_AGSJAJ_Importer: VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal uint   _kupdobCD   ;
      /* B 02 */  internal string _kupdobName ;
      /* C 03 */  internal string _ziro1by    ;
      /* D 04 */  internal string _ziro1      ;
      /* E 05 */  internal string _pnbMplaca  ;
      /* F 06 */  internal string _pnbVplaca  ;
      /* G 07 */  internal string _postaBr    ;
      /* H 08 */  internal string _grad       ;
      /* I 09 */  internal string _ulica1     ;
      /* J 10 */  internal string _drzava     ;
      /* K 11 */  internal string _tel1       ;
      /* L 12 */  internal string _fax        ;
      /* M 13 */  internal string _matbrOib   ;
  }

   public VvKupdob_AGSJAJ_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._kupdobCD    = GetUint32 ( 1);
      rawDatStruct._kupdobName  = GetString ( 2);
      rawDatStruct._ziro1by     = GetString ( 3);
      rawDatStruct._ziro1       = GetString ( 4);
      rawDatStruct._pnbMplaca   = GetString ( 5);
      rawDatStruct._pnbVplaca   = GetString ( 6);
      rawDatStruct._postaBr     = GetString ( 7);
      rawDatStruct._grad        = GetString ( 8);
      rawDatStruct._ulica1      = GetString ( 9);
      rawDatStruct._drzava      = GetString (10);
      rawDatStruct._tel1        = GetString (11);
      rawDatStruct._fax         = GetString (12);
      rawDatStruct._matbrOib    = GetString (13);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Kupdob kupdob_rec = new Kupdob();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      uint   currCentCD = 0, kupdobCD = 0;;
      string currCentTK = "";

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         kupdob_rec.Memset0(0);

         kupdob_rec.KupdobCD = rawDatStruct._kupdobCD                                    ;
         kupdob_rec.Naziv    = LimitedStr(rawDatStruct._kupdobName, ZXC.KpdbCI.naziv    );
         kupdob_rec.Ticker   = kupdob_rec.GenerateTicker(2)                              ;
                                                                                        
         kupdob_rec.Ziro1By  = LimitedStr(rawDatStruct._ziro1by   , ZXC.KpdbCI.ziro1By  );
         kupdob_rec.Ziro1    = LimitedStr(rawDatStruct._ziro1     , ZXC.KpdbCI.ziro1    );
         kupdob_rec.PnbMPlaca= LimitedStr(rawDatStruct._pnbMplaca , ZXC.KpdbCI.pnbMPlaca);
         kupdob_rec.PnbVPlaca= LimitedStr(rawDatStruct._pnbVplaca , ZXC.KpdbCI.pnbVPlaca);
         kupdob_rec.PostaBr  = LimitedStr(rawDatStruct._postaBr   , ZXC.KpdbCI.postaBr  );
         kupdob_rec.Grad     = LimitedStr(rawDatStruct._grad      , ZXC.KpdbCI.grad     );
         kupdob_rec.Ulica1   =                                                          
         kupdob_rec.Ulica2   = LimitedStr(rawDatStruct._ulica1    , ZXC.KpdbCI.ulica1   );
         kupdob_rec.Drzava   = LimitedStr(rawDatStruct._drzava    , ZXC.KpdbCI.drzava   );
         kupdob_rec.Tel1     = LimitedStr(rawDatStruct._tel1      , ZXC.KpdbCI.tel1     );
         kupdob_rec.Fax      = LimitedStr(rawDatStruct._fax       , ZXC.KpdbCI.fax      );
         if(rawDatStruct._matbrOib.Length == 11)
         {
            kupdob_rec.Oib = LimitedStr(rawDatStruct._matbrOib, ZXC.KpdbCI.oib          );
         }                                                                              
         else // MatBr                                                                  
         {                                                                              
            kupdob_rec.Matbr = LimitedStr(rawDatStruct._matbrOib, ZXC.KpdbCI.matbr      );
         }

       //kupdob_rec.Napom1   = LimitedStr(rawDatStruct._napomena1 , ZXC.KpdbCI.napom1   );
       //
       //if(rawDatStruct._konto.Length.NotZero() && rawDatStruct._konto.StartsWith("22"))
       //{
       //   kupdob_rec.KontoDug = LimitedStr(rawDatStruct._konto.Substring(0, 4), ZXC.KpdbCI.oib);
       //}
       //else if(rawDatStruct._konto.Length.NotZero() && rawDatStruct._konto.StartsWith("12"))
       //{
       //   kupdob_rec.KontoPot = LimitedStr(rawDatStruct._konto.Substring(0, 4), ZXC.KpdbCI.oib);
       //}


         currCentCD = kupdob_rec.KupdobCD;
         currCentTK = kupdob_rec.Ticker  ;

         dbOK = kupdob_rec.VvDao.ADDREC(conn, kupdob_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvKupdob_AGSJAJ_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.KupdobDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvPerson_AGSJAJ_Importer: VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal uint     _personCD;
      /* B 02 */  internal string   _ime     ;
      /* C 03 */  internal string   _prezime ;
      /* D 04 */  internal string   _oib     ;
      /* E 05 */  internal ushort   _spol    ;
      /* F 06 */  internal string   _bDay    ;
      /* G 07 */  internal string   _napomena;
      /* H 08 */  internal string   _grad    ;
      /* I 09 */  internal string   _ulica   ;
      /* J 10 */  internal string   _radMj   ;
      /* K 11 */  internal string   _datePrij;
      /* L 12 */  internal uint     _bankaCD ;
  }

   public VvPerson_AGSJAJ_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._personCD = GetUint32 ( 1);
      rawDatStruct._ime      = GetString ( 2);
      rawDatStruct._prezime  = GetString ( 3);
      rawDatStruct._oib      = GetString ( 4);
      rawDatStruct._spol     = GetUint16 ( 5);
      rawDatStruct._bDay     = GetString ( 6);
      rawDatStruct._napomena = GetString ( 7);
      rawDatStruct._grad     = GetString ( 8);
      rawDatStruct._ulica    = GetString ( 9);
      rawDatStruct._radMj    = GetString (10);
      rawDatStruct._datePrij = GetString (11);
      rawDatStruct._bankaCD  = GetUint32 (12);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Person person_rec = new Person();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      uint   currCentCD = 0, personCD = 0;;
      string currCentTK = "";

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         person_rec.Memset0(0);

         person_rec.PersonCD =            rawDatStruct._personCD                     ;
         person_rec.BankaCd  =            rawDatStruct._bankaCD                      ;
         person_rec.Ime      = LimitedStr(rawDatStruct._ime     , ZXC.PerCI.ime     );
         person_rec.Prezime  = LimitedStr(rawDatStruct._prezime , ZXC.PerCI.prezime );
         person_rec.Oib      = LimitedStr(rawDatStruct._oib     , ZXC.PerCI.oib     );
         person_rec.Napomena = LimitedStr(rawDatStruct._napomena, ZXC.PerCI.napomena);
         person_rec.Grad     = LimitedStr(rawDatStruct._grad    , ZXC.PerCI.grad    );
         person_rec.Ulica    = LimitedStr(rawDatStruct._ulica   , ZXC.PerCI.ulica   );
         person_rec.RadMj    = LimitedStr(rawDatStruct._radMj   , ZXC.PerCI.radMj   );

         person_rec.Spol     = (rawDatStruct._spol == 1 ? ZXC.Spol.MUSKO : rawDatStruct._spol == 2 ? ZXC.Spol.ZENSKO : ZXC.Spol.NEPOZNATO);

         person_rec.BirthDate= ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(rawDatStruct._bDay    .Replace(".", ""));               
         person_rec.DatePri  = ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(rawDatStruct._datePrij.Replace(".", ""));               

         dbOK = person_rec.VvDao.ADDREC(conn, person_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvPerson_AGSJAJ_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.PersonDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvArtikl_AGSJAJ_Importer: VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string  _artiklCD  ;
      /* B 03 */  internal string  _artiklName;
      /* C 05 */  internal string  _ts        ;
   }

   public VvArtikl_AGSJAJ_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      //Artikl artikl_rec = new Artikl();
      RawData rawDatStruct = new RawData();

      rawDatStruct._artiklCD   = GetString (1);
      rawDatStruct._artiklName = GetString (2);
      rawDatStruct._ts         = GetString (3);
    //rawDatStruct._grupa1cd   = GetString (5);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Artikl artikl_rec = new Artikl();

      //Kupdob kupdob_rec;

      bool dbOK;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         artikl_rec.Memset0(0);

         artikl_rec.ArtiklCD   = LimitedStr(rawDatStruct._artiklCD  , ZXC.ArtCI.artiklCD  );
         artikl_rec.ArtiklName = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.artiklName);
         artikl_rec.TS         = LimitedStr(rawDatStruct._ts        , ZXC.ArtCI.ts        );

       //artikl_rec.TS          = "ROB" ;
       //artikl_rec.PdvKat      = "25"  ;
       //artikl_rec.SkladCD     = ""/*"VPSK"*/;

         //kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Naziv == rawDatStruct._ducatiCD);

         //if(kupdob_rec != null) 
         //{
         //   artikl_rec.ProizCD = kupdob_rec.KupdobCD;
         //}

         #region ADDREC

         dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artName")) // Meaning, ADDREC failed bikoz Microline's artiklName is duplicated (artiklCD is ok, I guess?) 
         {
            artikl_rec.Napomena   = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD + ")";
            artikl_rec.ArtiklName = LimitedStrWithAddition(artikl_rec.ArtiklName, uniqueAddition, ZXC.ArtCI.artiklName);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artCD")) 
         {
            artikl_rec.Napomena = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD2 + ")";
            artikl_rec.ArtiklCD   = LimitedStrWithAddition(artikl_rec.ArtiklCD, uniqueAddition, ZXC.ArtCI.artiklCD);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         #endregion ADDREC

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvArtikl_AGSJAJ_Importer.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

   private string LimitedStrWithAddition(string data, string uniqueAddition, int cIdx)
   {
      return ZXC.LenLimitedStrWithAddition(data, uniqueAddition, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvOsred_AGSJAJ_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string  _osredCD  ;
      /* B 02 */  internal string  _osredName;
      /* M 13 */  internal decimal _stopaAm  ;

   }

   public VvOsred_AGSJAJ_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._osredCD     = GetString ( 1);
      rawDatStruct._osredName   = GetString ( 2);
      rawDatStruct._stopaAm     = GetDecimal( 3);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Osred osred_rec = new Osred();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.None);
      Kupdob kupdob_rec;

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         if(rawDatStruct._osredName.StartsWith("*")) continue;

         osred_rec.Memset0(0);

         // direct, no bullshit 
         osred_rec.OsredCD = LimitedStr(rawDatStruct._osredCD  , ZXC.OsrCI.osredCD );
         osred_rec.Naziv   = LimitedStr(rawDatStruct._osredName, ZXC.OsrCI.naziv   );
         osred_rec.AmortSt = rawDatStruct._stopaAm;

         osred_rec.Vijek   = ZXC.DivSafe(100, osred_rec.AmortSt);

         dbOK = osred_rec.VvDao.ADDREC(conn, osred_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvOsred_AGSJAJ_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.OsredDao.GetSchemaColumnSize(cIdx));
   }

}


public class VvKupdob_TURZML_Importer: VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal uint   _kupdobCD   ;
      /* B 02 */  internal string _kupdobName ;
      /* C 03 */  internal string _ziro1      ;
      /* D 04 */  internal string _ulica1     ;
      /* E 05 */  internal string _grad       ;
      /* F 06 */  internal string _drzava     ;
  }

   public VvKupdob_TURZML_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._kupdobCD    = GetUint32 ( 1);
      rawDatStruct._kupdobName  = GetString ( 2);
      rawDatStruct._ziro1       = GetString ( 3);
      rawDatStruct._ulica1      = GetString ( 4);
      rawDatStruct._grad        = GetString ( 5);
      rawDatStruct._drzava      = GetString ( 6);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Kupdob kupdob_rec = new Kupdob();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      uint   currCentCD = 0, kupdobCD = 0;;
      string currCentTK = "";

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         kupdob_rec.Memset0(0);

         kupdob_rec.KupdobCD = rawDatStruct._kupdobCD                                    ;
         kupdob_rec.Naziv    = LimitedStr(rawDatStruct._kupdobName, ZXC.KpdbCI.naziv    );
         kupdob_rec.Ticker   = kupdob_rec.GenerateTicker(2)                              ;
                                                                                        
         kupdob_rec.Ziro1    = LimitedStr(rawDatStruct._ziro1     , ZXC.KpdbCI.ziro1    );
         kupdob_rec.Grad     = LimitedStr(rawDatStruct._grad      , ZXC.KpdbCI.grad     );
         kupdob_rec.Ulica1   =                                                          
         kupdob_rec.Ulica2   = LimitedStr(rawDatStruct._ulica1    , ZXC.KpdbCI.ulica1   );
         kupdob_rec.Drzava   = LimitedStr(rawDatStruct._drzava    , ZXC.KpdbCI.drzava   );
         //if(rawDatStruct._matbrOib.Length == 11)
         //{
         //   kupdob_rec.Oib = LimitedStr(rawDatStruct._matbrOib, ZXC.KpdbCI.oib          );
         //}                                                                              
         //else // MatBr                                                                  
         //{                                                                              
         //   kupdob_rec.Matbr = LimitedStr(rawDatStruct._matbrOib, ZXC.KpdbCI.matbr      );
         //}

       //kupdob_rec.Napom1   = LimitedStr(rawDatStruct._napomena1 , ZXC.KpdbCI.napom1   );
       //
       //if(rawDatStruct._konto.Length.NotZero() && rawDatStruct._konto.StartsWith("22"))
       //{
       //   kupdob_rec.KontoDug = LimitedStr(rawDatStruct._konto.Substring(0, 4), ZXC.KpdbCI.oib);
       //}
       //else if(rawDatStruct._konto.Length.NotZero() && rawDatStruct._konto.StartsWith("12"))
       //{
       //   kupdob_rec.KontoPot = LimitedStr(rawDatStruct._konto.Substring(0, 4), ZXC.KpdbCI.oib);
       //}


         currCentCD = kupdob_rec.KupdobCD;
         currCentTK = kupdob_rec.Ticker  ;

         dbOK = kupdob_rec.VvDao.ADDREC(conn, kupdob_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvKupdob_TURZML_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.KupdobDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvOsred_TURZML_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string  _osredCD  ;
      /* B 02 */  internal string  _osredName;
      /* C 03 */  internal string  _inventBr ;
      /* D 04 */  internal string  _lokacija ;
      /* E 05 */  internal decimal _stopaAm  ;
      /* F 06 */  internal string  _konto    ;
      /* G 07 */  internal string  _kontoIv  ;

   }

   public VvOsred_TURZML_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._osredCD     = GetString ( 1);
      rawDatStruct._osredName   = GetString ( 2);
      rawDatStruct._inventBr    = GetString ( 3);
      rawDatStruct._lokacija    = GetString ( 4);
      rawDatStruct._stopaAm     = GetDecimal( 5);
      rawDatStruct._konto       = GetString ( 6);
      rawDatStruct._kontoIv     = GetString ( 7);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Osred osred_rec = new Osred();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.None);
      Kupdob kupdob_rec;

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         if(rawDatStruct._osredName.StartsWith("*")) continue;

         osred_rec.Memset0(0);

         // direct, no bullshit 
         osred_rec.OsredCD = LimitedStr(rawDatStruct._osredCD  , ZXC.OsrCI.osredCD );
         osred_rec.Naziv   = LimitedStr(rawDatStruct._osredName, ZXC.OsrCI.naziv   );
         osred_rec.InvbrOd = LimitedStr(rawDatStruct._inventBr , ZXC.OsrCI.invbr_od);
         osred_rec.DokumCd = LimitedStr(rawDatStruct._lokacija , ZXC.OsrCI.dokum_cd);
         osred_rec.Konto   = LimitedStr(rawDatStruct._konto    , ZXC.OsrCI.konto   );
         osred_rec.KontoIv = LimitedStr(rawDatStruct._kontoIv  , ZXC.OsrCI.konto_iv);

         osred_rec.AmortSt = rawDatStruct._stopaAm;
         osred_rec.Vijek   = ZXC.DivSafe(100, osred_rec.AmortSt);

         dbOK = osred_rec.VvDao.ADDREC(conn, osred_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvOsred_TURZML_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.OsredDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvOsred_KEREMP_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string  _osredCD  ;
      /* B 02 */  internal string  _osredName;
      /* C 03 */  internal decimal _stopaAm  ;
      /* D 04 */  internal string  _inventBr ;
      /* E 05 */  internal uint    _mtros    ;
      /* F 06 */  internal string  _konto    ;
      /* G 07 */  internal string  _kontoIv  ;
      /* H 08 */  internal string  _napomena ;

   }

   public VvOsred_KEREMP_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._osredCD   = GetString ( 1);
      rawDatStruct._osredName = GetString ( 2);
      rawDatStruct._stopaAm   = GetDecimal( 3);
      rawDatStruct._inventBr  = GetString ( 4);
      rawDatStruct._mtros     = GetUint32 ( 5);
      rawDatStruct._konto     = GetString ( 6);
      rawDatStruct._kontoIv   = GetString ( 7);
      rawDatStruct._napomena  = GetString ( 8);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Osred osred_rec = new Osred();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.None);
      Kupdob kupdob_rec;

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         if(rawDatStruct._osredName.StartsWith("*")) continue;

         osred_rec.Memset0(0);

         // direct, no bullshit 
         osred_rec.OsredCD = LimitedStr(rawDatStruct._osredCD  , ZXC.OsrCI.osredCD );
         osred_rec.Naziv   = LimitedStr(rawDatStruct._osredName, ZXC.OsrCI.naziv   );
         osred_rec.AmortSt =           (rawDatStruct._stopaAm                      );
         osred_rec.InvbrOd = LimitedStr(rawDatStruct._inventBr , ZXC.OsrCI.invbr_od);
         osred_rec.MtrosCd =           (rawDatStruct._mtros                        );
         osred_rec.Konto   = LimitedStr(rawDatStruct._konto    , ZXC.OsrCI.konto   );
         osred_rec.KontoIv = LimitedStr(rawDatStruct._kontoIv  , ZXC.OsrCI.konto_iv);
         osred_rec.SerBr   = LimitedStr(rawDatStruct._napomena , ZXC.OsrCI.ser_br  );

         //osred_rec.AmortSt = rawDatStruct._stopaAm;
         osred_rec.Vijek   = ZXC.DivSafe(100, osred_rec.AmortSt);

         dbOK = osred_rec.VvDao.ADDREC(conn, osred_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvOsred_KEREMP_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.OsredDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvKupdob_KEREMP_Importer: VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal uint   _kupdobCD     ;
      /* B 02 */  internal string _kupdobPrezime;
      /* C 03 */  internal string _kupdobIme    ;
      /* D 04 */  internal string _ziro1        ;
      /* E 05 */  internal string _ulica1       ;
      /* F 06 */  internal string _grad         ;
  }

   public VvKupdob_KEREMP_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._kupdobCD      = GetUint32 ( 1);
      rawDatStruct._kupdobPrezime = GetString ( 2);
      rawDatStruct._kupdobIme     = GetString ( 3);
      rawDatStruct._ziro1         = GetString ( 4);
      rawDatStruct._ulica1        = GetString ( 5);
      rawDatStruct._grad          = GetString ( 6);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Kupdob kupdob_rec = new Kupdob();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      uint   currCentCD = 0, kupdobCD = 0;;
      string currCentTK = "";

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         kupdob_rec.Memset0(0);

         kupdob_rec.KupdobCD = rawDatStruct._kupdobCD                                                   ;
         kupdob_rec.Prezime  = LimitedStr(rawDatStruct._kupdobPrezime              , ZXC.KpdbCI.prezime);
         kupdob_rec.Ime      = LimitedStr(rawDatStruct._kupdobIme                  , ZXC.KpdbCI.ime    );
         kupdob_rec.Naziv    = LimitedStr(kupdob_rec.Prezime + " " + kupdob_rec.Ime, ZXC.KpdbCI.naziv  );

         kupdob_rec.Ticker   = kupdob_rec.GenerateTicker(2)                              ;
                                                                                        
         kupdob_rec.Ziro1    = LimitedStr(rawDatStruct._ziro1     , ZXC.KpdbCI.ziro1    );
         kupdob_rec.Grad     = LimitedStr(rawDatStruct._grad      , ZXC.KpdbCI.grad     );
         kupdob_rec.Ulica1   =                                                          
         kupdob_rec.Ulica2   = LimitedStr(rawDatStruct._ulica1    , ZXC.KpdbCI.ulica1   );
       //kupdob_rec.Drzava   = LimitedStr(rawDatStruct._drzava    , ZXC.KpdbCI.drzava   );
         //if(rawDatStruct._matbrOib.Length == 11)
         //{
         //   kupdob_rec.Oib = LimitedStr(rawDatStruct._matbrOib, ZXC.KpdbCI.oib          );
         //}                                                                              
         //else // MatBr                                                                  
         //{                                                                              
         //   kupdob_rec.Matbr = LimitedStr(rawDatStruct._matbrOib, ZXC.KpdbCI.matbr      );
         //}

       //kupdob_rec.Napom1   = LimitedStr(rawDatStruct._napomena1 , ZXC.KpdbCI.napom1   );
       //
       //if(rawDatStruct._konto.Length.NotZero() && rawDatStruct._konto.StartsWith("22"))
       //{
       //   kupdob_rec.KontoDug = LimitedStr(rawDatStruct._konto.Substring(0, 4), ZXC.KpdbCI.oib);
       //}
       //else if(rawDatStruct._konto.Length.NotZero() && rawDatStruct._konto.StartsWith("12"))
       //{
       //   kupdob_rec.KontoPot = LimitedStr(rawDatStruct._konto.Substring(0, 4), ZXC.KpdbCI.oib);
       //}


         currCentCD = kupdob_rec.KupdobCD;
         currCentTK = kupdob_rec.Ticker  ;

         dbOK = kupdob_rec.VvDao.ADDREC(conn, kupdob_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvKupdob_KEREMP_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.KupdobDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvPerson_KEREMP_Importer: VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal uint     _personCD;
      /* B 02 */  internal string   _prezime ;
      /* C 03 */  internal string   _ime     ;
      /* D 04 */  internal string   _spol    ;
      /* E 15 */  internal string   _radMj   ;
      /* F 06 */  internal string   _ulica   ;
      /* G 07 */  internal string   _grad    ;
      /* H 08 */  internal string   _jmbg    ;
      /* I 09 */  internal string   _oib     ;
      /* J 00 */  internal string   _bDay    ;
      /* K 11 */  internal string   _datePrij;
      /* L 12 */  internal uint     _prStazYY;
      /* M 13 */  internal uint     _prStazMM;
      /* N 14 */  internal uint     _prStazDD;
      /* O 15 */  internal uint     _bankaCD ;
      /* P 06 */  internal string   _napomena;
  }

   public VvPerson_KEREMP_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._personCD = GetUint32 ( 1);
      rawDatStruct._prezime  = GetString ( 2);
      rawDatStruct._ime      = GetString ( 3);
      rawDatStruct._spol     = GetString ( 4);
      rawDatStruct._radMj    = GetString ( 5);
      rawDatStruct._ulica    = GetString ( 6);
      rawDatStruct._grad     = GetString ( 7);
      rawDatStruct._jmbg     = GetString ( 8);
      rawDatStruct._oib      = GetString ( 9);
      rawDatStruct._bDay     = GetString (10);
      rawDatStruct._datePrij = GetString (11);
      rawDatStruct._prStazYY = GetUint32 (12);
      rawDatStruct._prStazMM = GetUint32 (13);
      rawDatStruct._prStazDD = GetUint32 (14);
      rawDatStruct._bankaCD  = GetUint32 (15);
      rawDatStruct._napomena = GetString (16);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Person person_rec = new Person();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      uint   currCentCD = 0, personCD = 0;;
      string currCentTK = "";

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         person_rec.Memset0(0);

         person_rec.PersonCD =            rawDatStruct._personCD                     ;
         person_rec.Prezime  = LimitedStr(rawDatStruct._prezime , ZXC.PerCI.prezime );
         person_rec.Ime      = LimitedStr(rawDatStruct._ime     , ZXC.PerCI.ime     );
         person_rec.Spol     = (rawDatStruct._spol == "M" ? ZXC.Spol.MUSKO : rawDatStruct._spol == "Ž" ? ZXC.Spol.ZENSKO : ZXC.Spol.NEPOZNATO);
         person_rec.RadMj    = LimitedStr(rawDatStruct._radMj   , ZXC.PerCI.radMj   );
         person_rec.Ulica    = LimitedStr(rawDatStruct._ulica   , ZXC.PerCI.ulica   );
         person_rec.Grad     = LimitedStr(rawDatStruct._grad    , ZXC.PerCI.grad    );
         person_rec.Jmbg     = LimitedStr(rawDatStruct._jmbg    , ZXC.PerCI.jmbg    );
         person_rec.Oib      = LimitedStr(rawDatStruct._oib     , ZXC.PerCI.oib     );
         person_rec.BirthDate= ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(rawDatStruct._bDay    .Replace(".", ""));               
         person_rec.DatePri  = ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(rawDatStruct._datePrij.Replace(".", ""));

         person_rec.PrevStazYY = rawDatStruct._prStazYY;
         person_rec.PrevStazMM = rawDatStruct._prStazMM;
         person_rec.PrevStazDD = rawDatStruct._prStazDD;

         person_rec.BankaCd  =            rawDatStruct._bankaCD                      ;
         person_rec.Napomena = LimitedStr(rawDatStruct._napomena, ZXC.PerCI.napomena);

         dbOK = person_rec.VvDao.ADDREC(conn, person_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvPerson_KEREMP_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.PersonDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvArtikl_VELEFO_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string  _artiklCD   ;
      /* B 02 */  internal string  _artiklName ;
      /* F 06 */  internal string  _ts         ;
      /* C 03 */  internal string  _jedMmj     ;
      /* D 04 */  internal string  _artGrCdA   ;
      /* E 05 */  internal string  _opis       ;
   }

   public VvArtikl_VELEFO_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      //Artikl artikl_rec = new Artikl();
      RawData rawDatStruct = new RawData();

      rawDatStruct._artiklCD   = GetString (01);
      rawDatStruct._artiklName = GetString (02);
      rawDatStruct._ts         = GetString (03);
      rawDatStruct._jedMmj     = GetString (04);
      rawDatStruct._artGrCdA   = GetString (05);
      rawDatStruct._opis       = GetString (06);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Artikl artikl_rec = new Artikl();

      //Kupdob kupdob_rec;

      bool dbOK;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         artikl_rec.Memset0(0);

         artikl_rec.ArtiklCD    = LimitedStr(rawDatStruct._artiklCD   , ZXC.ArtCI.artiklCD  );
         artikl_rec.ArtiklName  = LimitedStr(rawDatStruct._artiklName , ZXC.ArtCI.artiklName);
         artikl_rec.TS          = LimitedStr(rawDatStruct._ts         , ZXC.ArtCI.ts        );
         artikl_rec.JedMj       = LimitedStr(rawDatStruct._jedMmj     , ZXC.ArtCI.jedMj     );
         artikl_rec.Grupa1CD    = LimitedStr(rawDatStruct._artGrCdA   , ZXC.ArtCI.grupa1CD  );
         artikl_rec.Napomena    = LimitedStr(rawDatStruct._opis       , ZXC.ArtCI.napomena  );
       //artikl_rec.SkladCD     = "SVPS";
         artikl_rec.PdvKat      = "25";


         //kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Naziv == rawDatStruct._ducatiCD);

         //if(kupdob_rec != null) 
         //{
         //   artikl_rec.ProizCD = kupdob_rec.KupdobCD;
         //}

         #region ADDREC

         dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artName")) // Meaning, ADDREC failed bikoz Microline's artiklName is duplicated (artiklCD is ok, I guess?) 
         {
            artikl_rec.Napomena   = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD + ")";
            artikl_rec.ArtiklName = LimitedStrWithAddition(artikl_rec.ArtiklName, uniqueAddition, ZXC.ArtCI.artiklName);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artCD")) 
         {
            artikl_rec.Napomena = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD2 + ")";
            artikl_rec.ArtiklCD   = LimitedStrWithAddition(artikl_rec.ArtiklCD, uniqueAddition, ZXC.ArtCI.artiklCD);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         #endregion ADDREC

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvArtikl_VELEFO_Importer.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

   private string LimitedStrWithAddition(string data, string uniqueAddition, int cIdx)
   {
      return ZXC.LenLimitedStrWithAddition(data, uniqueAddition, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvKupdob_TEXTHO_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct oldRawData
   {
      /* A 01 */  internal uint   _kupdobCD   ;
      /* B 02 */  internal string _ticker     ;
      /* C 03 */  internal string _tip        ;
      /* D 04 */  internal string _kupdobName1;
      /* E 05 */  internal string _ulica1     ;
      /* F 06 */  internal string _grad       ;
      /* G 07 */  internal string _postaBr    ;
      /* H 08 */  internal string _tel1       ;
      /* I 09 */  internal string _ime        ;
      /* J 10 */  internal string _prezime    ;
      /* K 11 */  internal string _napomena1  ;
      /* L 12 */  internal string _napomena2  ;
   }

   internal struct RawData
   {
      /* A 01 */  internal uint   _kupdobCD    ;
      /* B 02 */  internal string _kupdobName1 ;
      /* C 03 */  internal string _ulica1      ;
      /* D 04 */  internal string _grad        ;
      /* E 05 */  internal string _postaBr     ;
      /* F 06 */  internal string _vatCntryCode;
      /* G 07 */  internal string _drzava      ;
      /* H 08 */  internal string _oib         ;
      /* I 09 */  internal string _ziro1       ;
      /* J 10 */  internal string _napomena1   ;
   }
   public VvKupdob_TEXTHO_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected /*override*/ void oldProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      oldRawData rawDatStruct = new oldRawData();

      rawDatStruct._kupdobCD    = GetUint32 (01);
      rawDatStruct._ticker      = GetString (02);
      rawDatStruct._tip         = GetString (03);
      rawDatStruct._kupdobName1 = GetString (04);
      rawDatStruct._ulica1      = GetString (05);
      rawDatStruct._grad        = GetString (06);
      rawDatStruct._postaBr     = GetString (07);
      rawDatStruct._tel1        = GetString (08);
      rawDatStruct._ime         = GetString (09);
      rawDatStruct._prezime     = GetString (10);
      rawDatStruct._napomena1   = GetString (11);
      rawDatStruct._napomena2   = GetString (12);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
//         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._kupdobCD    = GetUint32 (01);
      rawDatStruct._kupdobName1 = GetString (02);
      rawDatStruct._ulica1      = GetString (03);
      rawDatStruct._grad        = GetString (04);
      rawDatStruct._postaBr     = GetString (05);
      rawDatStruct._vatCntryCode= GetString (06);
      rawDatStruct._drzava      = GetString (07);
      rawDatStruct._oib         = GetString (08);
      rawDatStruct._ziro1       = GetString (09);
      rawDatStruct._napomena1   = GetString (10);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }
   protected override void PostProcessLines(XSqlConnection conn)
   {
      Kupdob kupdob_rec = new Kupdob();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         kupdob_rec.Memset0(0);

         kupdob_rec.KupdobCD     = rawDatStruct._kupdobCD;
         kupdob_rec.Naziv        = LimitedStr(rawDatStruct._kupdobName1 , ZXC.KpdbCI.naziv       );
         kupdob_rec.Ticker       = (kupdob_rec.GenerateTicker(2));                               
         kupdob_rec.Ulica1       =                                                               
         kupdob_rec.Ulica2       = LimitedStr(rawDatStruct._ulica1      , ZXC.KpdbCI.ulica1      );
         kupdob_rec.Grad         = LimitedStr(rawDatStruct._grad        , ZXC.KpdbCI.grad        );
         kupdob_rec.PostaBr      = LimitedStr(rawDatStruct._postaBr     , ZXC.KpdbCI.postaBr     );
         kupdob_rec.VatCntryCode = LimitedStr(rawDatStruct._vatCntryCode, ZXC.KpdbCI.vatCntryCode);
         kupdob_rec.Drzava       = LimitedStr(rawDatStruct._drzava      , ZXC.KpdbCI.drzava      );
         kupdob_rec.Oib          = LimitedStr(rawDatStruct._oib         , ZXC.KpdbCI.oib         );
         kupdob_rec.Ziro1        = LimitedStr(rawDatStruct._ziro1       , ZXC.KpdbCI.ziro1       );
         kupdob_rec.Napom1       = LimitedStr(rawDatStruct._napomena1   , ZXC.KpdbCI.napom1      );

         // ova promjena za shouldRewfreshData sa false na true je da bi sa refreshom dobio origRecID & origSrvID 
       //dbOK = kupdob_rec.VvDao.ADDREC(conn, kupdob_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         dbOK = kupdob_rec.VvDao.ADDREC(conn, kupdob_rec, true , false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvKupdob_TEXTHO_Importer2_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.KupdobDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvKupdob_METFLX_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal uint   _kupdobCD    ;
      /* B 02 */  internal string _kupdobName1 ;
      /* C 03 */  internal string _ulica1      ;
      /* D 04 */  internal string _postaBr     ;
      /* E 05 */  internal string _grad        ;
      /* F 06 */  internal string _drzava      ;
      /* G 07 */  internal string _tel1        ;
      /* H 08 */  internal string _banka       ;
      /* I 09 */  internal string _ziro        ;
      /* J 10 */  internal string _matbr       ;
      /* K 11 */  internal string _dummy       ;
   }
   public VvKupdob_METFLX_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._kupdobCD     = GetUint32 (01);
      rawDatStruct._kupdobName1  = GetString (02);
      rawDatStruct._ulica1       = GetString (03);
      rawDatStruct._postaBr      = GetString (04).Replace(" ", "");
      rawDatStruct._grad         = GetString (05);
      rawDatStruct._drzava       = GetString (06);
      rawDatStruct._tel1         = GetString (07);
      rawDatStruct._banka        = GetString (08);
      rawDatStruct._ziro         = GetString (09);
      rawDatStruct._matbr        = GetString (10);
      rawDatStruct._dummy        = GetString (11);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }
   protected override void PostProcessLines(XSqlConnection conn)
   {
      Kupdob kupdob_rec = new Kupdob();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         kupdob_rec.Memset0(0);
         kupdob_rec.KupdobCD     = rawDatStruct._kupdobCD;
         kupdob_rec.Naziv        = LimitedStr(rawDatStruct._kupdobName1 , ZXC.KpdbCI.naziv       );
         kupdob_rec.Ticker       = (kupdob_rec.GenerateTicker(2));                               
         kupdob_rec.Ulica1       =                                                               
         kupdob_rec.Ulica2       = LimitedStr(rawDatStruct._ulica1      , ZXC.KpdbCI.ulica1      );
         kupdob_rec.Grad         = LimitedStr(rawDatStruct._grad        , ZXC.KpdbCI.grad        );
         kupdob_rec.PostaBr      = LimitedStr(rawDatStruct._postaBr     , ZXC.KpdbCI.postaBr     );
         //kupdob_rec.VatCntryCode = LimitedStr(rawDatStruct._vatCntryCode, ZXC.KpdbCI.vatCntryCode);
         kupdob_rec.Drzava       = LimitedStr(rawDatStruct._drzava      , ZXC.KpdbCI.drzava      );
         kupdob_rec.Matbr        = LimitedStr(rawDatStruct._matbr       , ZXC.KpdbCI.matbr       );
         kupdob_rec.Tel1         = LimitedStr(rawDatStruct._tel1        , ZXC.KpdbCI.tel1        );
         kupdob_rec.Ziro1        = LimitedStr(rawDatStruct._ziro        , ZXC.KpdbCI.ziro1       );
         kupdob_rec.Ziro1By      = LimitedStr(rawDatStruct._banka       , ZXC.KpdbCI.ziro1By     );

         if(kupdob_rec.Grad.NotEmpty() &&  kupdob_rec.Naziv.EndsWith(kupdob_rec.Grad))
         {
            kupdob_rec.Napom2 = kupdob_rec.Naziv;
          //kupdob_rec.Naziv  = kupdob_rec.Naziv.Replace(kupdob_rec.Grad, "").TrimEnd(' ');
            kupdob_rec.Naziv  = kupdob_rec.Naziv.Replace(" " + kupdob_rec.Grad, "");
         }

       //if(kupdob_rec.KupdobCD < 2000) kupdob_rec.Tip = "K";
       //else                           kupdob_rec.Tip = "D";

         // ova promjena za shouldRewfreshData sa false na true je da bi sa refreshom dobio origRecID & origSrvID 
       //dbOK = kupdob_rec.VvDao.ADDREC(conn, kupdob_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         dbOK = kupdob_rec.VvDao.ADDREC(conn, kupdob_rec, true , false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvKupdob_METFLX_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.KupdobDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvArtikl_METFLX_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string  _artiklCD   ;
      /* B 02 */  internal string  _artiklName ;
      /* C 03 */  internal string  _jedMmj     ;
    ///* D 04 */  internal string  _ts         ;
      /* D 04 */  internal string  _masaKG     ;
   }

   public VvArtikl_METFLX_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      //Artikl artikl_rec = new Artikl();
      RawData rawDatStruct = new RawData();

      rawDatStruct._artiklCD   = GetString (01);
      rawDatStruct._artiklName = GetString (02);
      rawDatStruct._jedMmj     = GetString (03);
      rawDatStruct._masaKG     = GetString (04);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Artikl artikl_rec = new Artikl();

      //Kupdob kupdob_rec;

      bool dbOK;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         if(rawDatStruct._artiklCD == "qweqwe") continue;

         artikl_rec.Memset0(0);

         artikl_rec.ArtiklCD    = LimitedStr(rawDatStruct._artiklCD   , ZXC.ArtCI.artiklCD  );
         artikl_rec.ArtiklName  = LimitedStr(rawDatStruct._artiklName , ZXC.ArtCI.artiklName);
       //artikl_rec.TS          = LimitedStr(rawDatStruct._ts         , ZXC.ArtCI.ts        );
         artikl_rec.MasaNetto   = ZXC.ValOrZero_Decimal(rawDatStruct._masaKG, 2);
         if(artikl_rec.MasaNetto.NotZero())
            artikl_rec.MasaNettoJM = "KG";
         artikl_rec.JedMj       = LimitedStr(rawDatStruct._jedMmj     , ZXC.ArtCI.jedMj     );
       //artikl_rec.Grupa1CD    = LimitedStr(rawDatStruct._artGrCdA   , ZXC.ArtCI.grupa1CD  );
       //artikl_rec.Napomena    = LimitedStr(rawDatStruct._opis       , ZXC.ArtCI.napomena  );
       //artikl_rec.SkladCD     = "SVPS";
       //artikl_rec.PdvKat      = "25";

         if(ZXC.SubstringSafe(artikl_rec.ArtiklCD, 0, 1) == "1" || ZXC.SubstringSafe(artikl_rec.ArtiklCD, 0, 1) == "2") artikl_rec.TS = "MAT";
         if(ZXC.SubstringSafe(artikl_rec.ArtiklCD, 0, 1) == "4" || ZXC.SubstringSafe(artikl_rec.ArtiklCD, 0, 1) == "5") artikl_rec.TS = "SIT";
         if(ZXC.SubstringSafe(artikl_rec.ArtiklCD, 0, 1) == "6"                                                       ) artikl_rec.TS = "PRO";


         if(artikl_rec.ArtiklName.IsEmpty()) artikl_rec.ArtiklName = artikl_rec.ArtiklCD;

         //kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Naziv == rawDatStruct._ducatiCD);

         //if(kupdob_rec != null) 
         //{
         //   artikl_rec.ProizCD = kupdob_rec.KupdobCD;
         //}

         #region ADDREC

         dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artName")) // Meaning, ADDREC failed bikoz Microline's artiklName is duplicated (artiklCD is ok, I guess?) 
         {
            artikl_rec.Napomena   = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD + ")";
            artikl_rec.ArtiklName = LimitedStrWithAddition(artikl_rec.ArtiklName, uniqueAddition, ZXC.ArtCI.artiklName);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artCD")) 
         {
            artikl_rec.Napomena = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.napomena);

          //string uniqueAddition = " (" + artikl_rec.ArtiklCD2 + ")";
            string uniqueAddition = " (" + "2" + ")";
            artikl_rec.ArtiklCD   = LimitedStrWithAddition(artikl_rec.ArtiklCD, uniqueAddition, ZXC.ArtCI.artiklCD);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         #endregion ADDREC

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvArtikl_METFLX_Importer.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

   private string LimitedStrWithAddition(string data, string uniqueAddition, int cIdx)
   {
      return ZXC.LenLimitedStrWithAddition(data, uniqueAddition, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvPerson_TEXTHO_Importer: VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal uint     _personCD     ;
      /* B 02 */  internal string   _prezimeIme   ;
      /* C 03 */  internal string   _radnoMjesto  ;
      /* D 04 */  internal string   _oib          ;
      /* E 05 */  internal string   _mjestoRada   ;
      /* F 06 */  internal string   _opcRadCD     ;
      /* G 07 */  internal string   _ulica        ;
      /* H 08 */  internal string   _grad         ;
      /* I 09 */  internal decimal  _koef         ;
      /* J 10 */  internal uint     _newBankaCD   ;
      /* K 11 */  internal string   _oldBankaName ;
      /* L 12 */  internal string   _oldBankaIban ;
      /* M 13 */  internal string   _oldRadnikIban;
      /* N 14 */  internal string   _oldRadnikPNB ;
      /* O 15 */  internal uint     _nacinIsplate ;
      /* P 16 */  internal string   _zasticeniRN  ;
      /* Q 17 */  internal string   _zasticPNBm   ;
      /* R 18 */  internal string   _zasticPNBv   ;
      /* S 19 */  internal decimal  _bruto        ;
      /* T 20 */  internal uint     _oldMtrCD     ;
      /* U 21 */  internal uint     _newMtrCD     ; // dummy jer se snalazimo sami na osnovu '_oldMtrCD' 
      /* V 22 */  internal uint     _radMjCD      ;
      /* W 23 */  internal uint     _oldMtrNaziv  ;
      /* X 24 */  internal decimal  _prijevoz     ;

      internal string GetNvr()
      {
         switch(_radMjCD)
         {
            case 472: return "472 – Admin and Warehouse";
            case 478: return "478 – Shop Manager";
            case 479: return "479 – Shop Assistant";

            default: return "";
         }
      }

      internal uint GetMtrosCD()
      {
         Kupdob kupdob_rec;

         string maybeTickerRoot = "P" + (_oldMtrCD-100).ToString("00");

              if(_oldMtrCD == 190) kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.KupdobCD == 120012);
         else if(_oldMtrCD == 195) kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.KupdobCD == 120027);
         else                      kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Ticker.StartsWith(maybeTickerRoot));

         if(kupdob_rec != null) return kupdob_rec.KupdobCD;

         return 0;
      }

      internal string GetMtrosTK()
      {
         Kupdob kupdob_rec;

         string maybeTickerRoot = "P" + (_oldMtrCD-100).ToString("00");

              if(_oldMtrCD == 190) kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.KupdobCD == 120012);
         else if(_oldMtrCD == 195) kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.KupdobCD == 120027);
         else                      kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Ticker.StartsWith(maybeTickerRoot));

         if(kupdob_rec != null) return kupdob_rec.Ticker;

         return "";
      }

      internal string GetIme()
      {
         string[] splitters = this._prezimeIme.Split(' ');

         int splCount = splitters.Count();

         if(splCount.IsZero()) return "";

         return splitters[splitters.Count() - 1];
      }

      internal string GetPrezime()
      {
         return _prezimeIme.Replace(GetIme(), "").TrimEnd(' ');
      }
   }

   public VvPerson_TEXTHO_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._prezimeIme = GetString ( 2);

      /* A 01 */  rawDatStruct._personCD      = GetUint32 (01);
      /* B 02 */  rawDatStruct._prezimeIme    = GetString (02);
      /* C 03 */  rawDatStruct._radnoMjesto   = GetString (03);
      /* D 04 */  rawDatStruct._oib           = GetString (04);
      /* E 05 */  rawDatStruct._mjestoRada    = GetString (05);
      /* F 06 */  rawDatStruct._opcRadCD      = GetString (06);
      /* G 07 */  rawDatStruct._ulica         = GetString (07);
      /* H 08 */  rawDatStruct._grad          = GetString (08);
      /* I 09 */  rawDatStruct._koef          = GetDecimal(09);
      /* J 10 */  rawDatStruct._newBankaCD    = GetUint32 (10); // x 
      /* K 11 */  rawDatStruct._oldBankaName  = GetString (11); // x 
      /* L 12 */  rawDatStruct._oldBankaIban  = GetString (12); // x 
      /* M 13 */  rawDatStruct._oldRadnikIban = GetString (13); // x 
      /* N 14 */  rawDatStruct._oldRadnikPNB  = GetString (14); // x 
      /* O 15 */  rawDatStruct._nacinIsplate  = GetUint32 (15);
      /* P 16 */  rawDatStruct._zasticeniRN   = GetString (16); // x 
      /* Q 17 */  rawDatStruct._zasticPNBm    = GetString (17); // x 
      /* R 18 */  rawDatStruct._zasticPNBv    = GetString (18); // x 
      /* S 19 */  rawDatStruct._bruto         = GetDecimal(19);
      /* T 20 */  rawDatStruct._oldMtrCD      = GetUint32 (20);
      /* U 21 */  // dummy 
      /* V 22 */  rawDatStruct._radMjCD       = GetUint32 (22);
      /* W 23 */  rawDatStruct._oldMtrNaziv   = GetUint32 (23);
      /* X 24 */  rawDatStruct._prijevoz      = GetDecimal(24);



      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Person person_rec = new Person();

      Kupdob bankaKupdob_rec;

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      string napomena = "";

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         person_rec.Memset0(0);

         person_rec.PersonCD   =            rawDatStruct._personCD                          ;
         person_rec.Ime        = LimitedStr(rawDatStruct.GetIme()    , ZXC.PerCI.ime       );
         person_rec.Prezime    = LimitedStr(rawDatStruct.GetPrezime(), ZXC.PerCI.prezime   );
         person_rec.RadMj      = LimitedStr(rawDatStruct._radnoMjesto, ZXC.PerCI.radMj     );
         person_rec.Oib        = LimitedStr(rawDatStruct._oib        , ZXC.PerCI.oib       );
         person_rec.MjestoRada = LimitedStr(rawDatStruct._mjestoRada , ZXC.PerCI.mjestoRada);
         person_rec.X_opcRadCD = 
         person_rec.X_opcCD    = LimitedStr(rawDatStruct._opcRadCD   , ZXC.PerCI.x_opcRadCD);
         person_rec.Ulica      = LimitedStr(rawDatStruct._ulica      , ZXC.PerCI.ulica     );
         person_rec.Grad       = LimitedStr(rawDatStruct._grad       , ZXC.PerCI.grad      );
         person_rec.X_koef     =            rawDatStruct._koef                              ;
         person_rec.X_brutoOsn =            rawDatStruct._bruto                             ;
         person_rec.MtrosCd    =            rawDatStruct.GetMtrosCD()                       ;
         person_rec.MtrosTk    =            rawDatStruct.GetMtrosTK()                       ;
         person_rec.NaravVrRad = LimitedStr(rawDatStruct.GetNvr()    , ZXC.PerCI.naravVrRad);
         person_rec.X_prijevoz =            rawDatStruct._prijevoz                          ;


         person_rec.VrstaIsplate = (rawDatStruct._nacinIsplate == 2 ? Person.VrstaIsplateEnum.TEKUCI : Person.VrstaIsplateEnum.BANKA);

         if(person_rec.VrstaIsplate == Person.VrstaIsplateEnum.BANKA)
         {
            person_rec.PnbV = rawDatStruct._oldRadnikIban;

            bankaKupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.KupdobCD == rawDatStruct._newBankaCD);
            if(bankaKupdob_rec != null)
            {
               person_rec.BankaCd = bankaKupdob_rec.KupdobCD;
               person_rec.BankaTk = bankaKupdob_rec.Ticker  ;
            }
         }

         person_rec.IsPlaca   = true;
         person_rec.X_isMioII = true;
         person_rec.X_isTrgov = true; // TODO 

         if(person_rec.X_isTrgov) person_rec.TS = "T";
         else                     person_rec.TS = "R";

         napomena = "IBAN:\t"    + rawDatStruct._oldRadnikIban + "\r\n" +
                    "PNB:\t"     + rawDatStruct._oldRadnikPNB  + "\r\n" +
                    "ZastRn:\t"  + rawDatStruct._zasticeniRN   + "\r\n" +
                    "ZRnPnbM:\t" + rawDatStruct._zasticPNBm    + "\r\n" +
                    "ZRnPnbV:\t" + rawDatStruct._zasticPNBv    + "\r\n" ;

         person_rec.Napomena = LimitedStr(napomena, ZXC.PerCI.napomena);


         dbOK = person_rec.VvDao.ADDREC(conn, person_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvPerson_TEXTHO_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.PersonDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvArtikl_BRADA_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string  _artiklCD  ;
      /* B 02 */  internal string  _artiklName;
      /* C 03 */  internal decimal _importCij ;
      /* D 04 */  internal string  _artiklOpis;
      /* E 05 */  internal string  _grupa1    ;
      /* F 06 */  internal string  _barCode   ;
      /* G 07 */  internal string  _tarifa    ;
      /* H 08 */  internal string  _drzava    ;
      /* I 09 */  internal uint    _dobavCD   ;
   }

   public VvArtikl_BRADA_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      //Artikl artikl_rec = new Artikl();
      RawData rawDatStruct = new RawData();

      rawDatStruct._artiklCD   = GetString (01);
      rawDatStruct._artiklName = GetString (02);
      rawDatStruct._importCij  = GetDecimal(03);
      rawDatStruct._artiklOpis = GetString (04);
      rawDatStruct._grupa1     = GetString (05);
      rawDatStruct._barCode    = GetString (06);
      rawDatStruct._tarifa     = GetString (07);
      rawDatStruct._drzava     = GetString (08);
      rawDatStruct._dobavCD    = GetUint32 (09);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Artikl artikl_rec = new Artikl();

      #region Grupa1&2

      bool grupeSiVecUcitao = true/*false*/; // s ovime reguliras oces ili neces (dakle, samo za first import) 

      if(grupeSiVecUcitao == false)
      {
         string origDbName = ZXC.TheMainDbConnection.Database;

         int gr1CD = 0;
         var grupa1DistinctList   = RawDataList.Select(rd => rd._grupa1).Distinct().OrderBy(x => x);
         var grupa1DistinctWithCD = grupa1DistinctList.Select(disGrName => new { grupa1name = disGrName, grupa1CD = "G" + (++gr1CD).ToString("00") });

       //int gr2CD = 0;
       //var grupa2DistinctList   = RawDataList.Select(rd => rd._grupa2).Distinct().OrderBy(x => x);
       //var grupa2DistinctWithCD = grupa2DistinctList.Select(disGrName => new { grupa2name = disGrName, grupa2CD = (++gr2CD).ToString("00") });

         foreach(var gr1 in grupa1DistinctWithCD)
         {
            ZXC.luiListaGrupa1Artikla.Add(new VvLookUpItem(gr1.grupa1CD, gr1.grupa1name));
         }
         VvDaoBase.SaveLookUpListToSqlTable(ZXC.luiListaGrupa1Artikla);

       //foreach(var gr2 in grupa2DistinctWithCD)
       //{
       //   ZXC.luiListaGrupa2Artikla.Add(new VvLookUpItem(gr2.grupa2CD, gr2.grupa2name));
       //}
       //VvDaoBase.SaveLookUpListToSqlTable(ZXC.luiListaGrupa2Artikla);

         ZXC.SetMainDbConnDatabaseName(origDbName); // jer SaveLookUpListToSqlTable promijeni na 'vvektor' ... 
      }

      #endregion Grupa1&2

      //Kupdob kupdob_rec;

      bool dbOK;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      int artCDcounter = 0;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         if(rawDatStruct._artiklCD == "qweqwe") continue;

         artikl_rec.Memset0(0);

         // ArtiklCD Manager 
         if(rawDatStruct._artiklCD.NotEmpty()) // as in import file 
         {
            artikl_rec.ArtiklCD = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.artiklCD);
         }
         else // izmisljamo sifru 
         {
            if(rawDatStruct._artiklName.Length <= 16) artikl_rec.ArtiklCD = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.artiklCD);
            else                                      artikl_rec.ArtiklCD = LimitedStr(rawDatStruct._artiklName.SubstringSafe(0, 12) + "-" + (++artCDcounter).ToString("000"), ZXC.ArtCI.artiklCD);
         }

         artikl_rec.ArtiklName  = LimitedStr(rawDatStruct._artiklName , ZXC.ArtCI.artiklName);
         artikl_rec.LongOpis    = LimitedStr(rawDatStruct._artiklOpis , ZXC.ArtCI.longOpis  );
         artikl_rec.TS          = "ROB"                                                      ;
         artikl_rec.JedMj       = "Kom";
         if(rawDatStruct._grupa1.NotEmpty())
         artikl_rec.Grupa1CD    = ZXC.luiListaGrupa1Artikla.Single(lui => lui.Name == rawDatStruct._grupa1).Cd;
         artikl_rec.ImportCij   =           (rawDatStruct._importCij                        );
         artikl_rec.BarCode1    = LimitedStr(rawDatStruct._barCode    , ZXC.ArtCI.barCode1  );
         artikl_rec.CarTarifa   = LimitedStr(rawDatStruct._tarifa     , ZXC.ArtCI.carTarifa );
         artikl_rec.MadeIn      = LimitedStr(rawDatStruct._drzava     , ZXC.ArtCI.madeIn    );
         artikl_rec.DobavCD     =           (rawDatStruct._dobavCD                          );
         //artikl_rec.Napomena    = LimitedStr(rawDatStruct._opis       , ZXC.ArtCI.napomena);
         //artikl_rec.SkladCD     = "SVPS";
         //artikl_rec.PdvKat      = "25";

       //if(ZXC.SubstringSafe(artikl_rec.ArtiklCD, 0, 1) == "1" || ZXC.SubstringSafe(artikl_rec.ArtiklCD, 0, 1) == "2") artikl_rec.TS = "MAT";
       //if(ZXC.SubstringSafe(artikl_rec.ArtiklCD, 0, 1) == "4" || ZXC.SubstringSafe(artikl_rec.ArtiklCD, 0, 1) == "5") artikl_rec.TS = "SIT";
       //if(ZXC.SubstringSafe(artikl_rec.ArtiklCD, 0, 1) == "6"                                                       ) artikl_rec.TS = "PRO";


         if(artikl_rec.ArtiklName.IsEmpty()) artikl_rec.ArtiklName = artikl_rec.ArtiklCD;

         //kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Naziv == rawDatStruct._ducatiCD);

         //if(kupdob_rec != null) 
         //{
         //   artikl_rec.ProizCD = kupdob_rec.KupdobCD;
         //}

         #region ADDREC

         dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artName")) // Meaning, ADDREC failed bikoz Microline's artiklName is duplicated (artiklCD is ok, I guess?) 
         {
            artikl_rec.Napomena   = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD + ")";
            artikl_rec.ArtiklName = LimitedStrWithAddition(artikl_rec.ArtiklName, uniqueAddition, ZXC.ArtCI.artiklName);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artCD")) 
         {
            artikl_rec.Napomena = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.napomena);

          //string uniqueAddition = " (" + artikl_rec.ArtiklCD2 + ")";
            string uniqueAddition = " (" + "2" + ")";
            artikl_rec.ArtiklCD   = LimitedStrWithAddition(artikl_rec.ArtiklCD, uniqueAddition, ZXC.ArtCI.artiklCD);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         #endregion ADDREC

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvArtikl_BRADA_Importer.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

   private string LimitedStrWithAddition(string data, string uniqueAddition, int cIdx)
   {
      return ZXC.LenLimitedStrWithAddition(data, uniqueAddition, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvKupdob_FRAG_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal uint   _kupdobCD    ;
      /* B 02 */  internal string _kupdobName1 ;
      /* C 03 */  internal string _ulica1      ;
      /* D 04 */  internal string _postaBr     ;
      /* E 05 */  internal string _grad        ;
      /* F 06 */  internal string _vat         ;
      /* G 07 */  internal string _oib         ;
      /* H 08 */  internal string _matbr       ;
      /* I 09 */  internal string _drzava      ;
      /* J 10 */  internal string _ime         ;
      /* K 11 */  internal string _prezime     ;
      /* L 12 */  internal string _ziro        ;
      /* M 13 */  internal string _tel1        ;
      /* N 14 */  internal string _tel2        ;
      /* O 15 */  internal string _telfax      ;
      /* P 16 */  internal string _tel4        ;
      /* Q 17 */  internal string _eMail       ;
      /* R 18 */  internal string _napomena    ;
   }
   public VvKupdob_FRAG_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._kupdobCD     = GetUint32 (01);
      rawDatStruct._kupdobName1  = GetString (02).TrimEnd(' ');
      rawDatStruct._ulica1       = GetString (03).TrimEnd(' ');
      rawDatStruct._postaBr      = GetString (04).TrimEnd(' ');
      rawDatStruct._grad         = GetString (05).TrimEnd(' ');
      rawDatStruct._vat          = GetString (06).TrimEnd(' ');
      rawDatStruct._oib          = GetString (07).TrimEnd(' ');
      rawDatStruct._matbr        = GetString (08).TrimEnd(' ');
      rawDatStruct._drzava       = GetString (09).TrimEnd(' ');
      rawDatStruct._ime          = GetString (10).TrimEnd(' ');
      rawDatStruct._prezime      = GetString (11).TrimEnd(' ');
      rawDatStruct._ziro         = GetString (12).TrimEnd(' ');
      rawDatStruct._tel1         = GetString (13).TrimEnd(' ');
      rawDatStruct._tel2         = GetString (14).TrimEnd(' ');
      rawDatStruct._telfax       = GetString (15).TrimEnd(' ');
      rawDatStruct._tel4         = GetString (16).TrimEnd(' ');
      rawDatStruct._eMail        = GetString (17).TrimEnd(' ');
      rawDatStruct._napomena     = GetString (18).TrimEnd(' ');

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }
   protected override void PostProcessLines(XSqlConnection conn)
   {
      Kupdob kupdob_rec = new Kupdob();

      bool dbOK=true;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         kupdob_rec.Memset0(0);
         kupdob_rec.KupdobCD     = rawDatStruct._kupdobCD;
         kupdob_rec.Naziv        = LimitedStr(rawDatStruct._kupdobName1 , ZXC.KpdbCI.naziv       );
         kupdob_rec.Ticker       = (kupdob_rec.GenerateTicker(2));                               
         kupdob_rec.Ulica1       =                                                               
         kupdob_rec.Ulica2       = LimitedStr(rawDatStruct._ulica1      , ZXC.KpdbCI.ulica1      );
         kupdob_rec.Grad         = LimitedStr(rawDatStruct._grad        , ZXC.KpdbCI.grad        );
         kupdob_rec.PostaBr      = LimitedStr(rawDatStruct._postaBr     , ZXC.KpdbCI.postaBr     );
         kupdob_rec.VatCntryCode = LimitedStr(rawDatStruct._vat         , ZXC.KpdbCI.vatCntryCode);
         kupdob_rec.Drzava       = LimitedStr(rawDatStruct._drzava      , ZXC.KpdbCI.drzava      );
         kupdob_rec.Oib          = LimitedStr(rawDatStruct._oib         , ZXC.KpdbCI.oib         );
         kupdob_rec.Matbr        = LimitedStr(rawDatStruct._matbr       , ZXC.KpdbCI.matbr       );
         kupdob_rec.Ime          = LimitedStr(rawDatStruct._ime         , ZXC.KpdbCI.ime         );
         kupdob_rec.Prezime      = LimitedStr(rawDatStruct._prezime     , ZXC.KpdbCI.prezime     );
         kupdob_rec.Ziro1        = LimitedStr(rawDatStruct._ziro        , ZXC.KpdbCI.ziro1       );
         kupdob_rec.Tel1         = LimitedStr(rawDatStruct._tel1        , ZXC.KpdbCI.tel1        );
         kupdob_rec.Tel2         = LimitedStr(rawDatStruct._tel2        , ZXC.KpdbCI.tel2        );
         kupdob_rec.Fax          = LimitedStr(rawDatStruct._telfax      , ZXC.KpdbCI.fax         );
         kupdob_rec.Url          = LimitedStr(rawDatStruct._tel4        , ZXC.KpdbCI.url         );
         kupdob_rec.Email        = LimitedStr(rawDatStruct._eMail       , ZXC.KpdbCI.email       );
         kupdob_rec.Napom1       = LimitedStr(rawDatStruct._napomena    , ZXC.KpdbCI.napom1      );

         // ova promjena za shouldRewfreshData sa false na true je da bi sa refreshom dobio origRecID & origSrvID 
       //dbOK = kupdob_rec.VvDao.ADDREC(conn, kupdob_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         dbOK = kupdob_rec.VvDao.ADDREC(conn, kupdob_rec, true , false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvKupdob_FRAG_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.KupdobDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvArtikl_FRAG_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string  _artiklCD   ;
      /* B 02 */  internal string  _artiklName ;
    ///* C 03 */  internal string  _jedMmj     ;
    ///* D 04 */  internal string  _ts         ;
    ///* D 04 */  internal string  _masaKG     ;
   }

   public VvArtikl_FRAG_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      //Artikl artikl_rec = new Artikl();
      RawData rawDatStruct = new RawData();

      rawDatStruct._artiklCD   = GetString (01);
      rawDatStruct._artiklName = GetString (02);
    //rawDatStruct._jedMmj     = GetString (03);
    //rawDatStruct._masaKG     = GetString (04);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Artikl artikl_rec = new Artikl();

      //Kupdob kupdob_rec;

      bool dbOK;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         if(rawDatStruct._artiklCD == "qweqwe") continue;

         artikl_rec.Memset0(0);

         artikl_rec.ArtiklCD    = LimitedStr(rawDatStruct._artiklCD   , ZXC.ArtCI.artiklCD  );
         artikl_rec.ArtiklName  = LimitedStr(rawDatStruct._artiklName , ZXC.ArtCI.artiklName);
       //artikl_rec.TS          = LimitedStr(rawDatStruct._ts         , ZXC.ArtCI.ts        );
       //artikl_rec.MasaNetto   = ZXC.ValOrZero_Decimal(rawDatStruct._masaKG, 2);
       //if(artikl_rec.MasaNetto.NotZero())
       //   artikl_rec.MasaNettoJM = "KG";
       //artikl_rec.JedMj       = LimitedStr(rawDatStruct._jedMmj     , ZXC.ArtCI.jedMj     );
       //artikl_rec.Grupa1CD    = LimitedStr(rawDatStruct._artGrCdA   , ZXC.ArtCI.grupa1CD  );
       //artikl_rec.Napomena    = LimitedStr(rawDatStruct._opis       , ZXC.ArtCI.napomena  );
       //artikl_rec.SkladCD     = "SVPS";
       //artikl_rec.PdvKat      = "25";

       //if(ZXC.SubstringSafe(artikl_rec.ArtiklCD, 0, 1) == "1" || ZXC.SubstringSafe(artikl_rec.ArtiklCD, 0, 1) == "2") artikl_rec.TS = "MAT";
       //if(ZXC.SubstringSafe(artikl_rec.ArtiklCD, 0, 1) == "4" || ZXC.SubstringSafe(artikl_rec.ArtiklCD, 0, 1) == "5") artikl_rec.TS = "SIT";
       //if(ZXC.SubstringSafe(artikl_rec.ArtiklCD, 0, 1) == "6"                                                       ) artikl_rec.TS = "PRO";


         if(artikl_rec.ArtiklName.IsEmpty()) artikl_rec.ArtiklName = artikl_rec.ArtiklCD;

         //kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Naziv == rawDatStruct._ducatiCD);

         //if(kupdob_rec != null) 
         //{
         //   artikl_rec.ProizCD = kupdob_rec.KupdobCD;
         //}

         #region ADDREC

         dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artName")) // Meaning, ADDREC failed bikoz Microline's artiklName is duplicated (artiklCD is ok, I guess?) 
         {
            artikl_rec.Napomena   = LimitedStr(rawDatStruct._artiklName, ZXC.ArtCI.napomena);

            string uniqueAddition = " (" + artikl_rec.ArtiklCD + ")";
            artikl_rec.ArtiklName = LimitedStrWithAddition(artikl_rec.ArtiklName, uniqueAddition, ZXC.ArtCI.artiklName);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artCD")) 
         {
            artikl_rec.Napomena = LimitedStr(rawDatStruct._artiklCD, ZXC.ArtCI.napomena);

          //string uniqueAddition = " (" + artikl_rec.ArtiklCD2 + ")";
            string uniqueAddition = " (" + "2" + ")";
            artikl_rec.ArtiklCD   = LimitedStrWithAddition(artikl_rec.ArtiklCD, uniqueAddition, ZXC.ArtCI.artiklCD);

            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
         }

         #endregion ADDREC

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvArtikl_METFLX_Importer.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

   private string LimitedStrWithAddition(string data, string uniqueAddition, int cIdx)
   {
      return ZXC.LenLimitedStrWithAddition(data, uniqueAddition, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvKupdob_TGPLEM_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      /* A 01 */  internal string _kupdobCD  ;
      /* B 02 */  internal string _kupdobName;
      /* C 03 */  internal string _oib       ;
      /* D 04 */  internal string _adresa    ;
      /* E 05 */  internal string _ptt       ;
      /* F 06 */  internal string _grad      ;
      /* G 07 */  internal string _telefon   ;
      /* H 08 */  internal string _telefax   ;
      /* I 09 */  internal string _mail      ;
      /* J 10 */  internal string _iban      ;
      /* K 11 */  internal string _kontakt   ;
      /* L 12 */  internal string _napomena  ;
   }

   public VvKupdob_TGPLEM_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._kupdobCD   = GetString ( 1);
      rawDatStruct._kupdobName = GetString ( 2);
      rawDatStruct._oib        = GetString ( 3);
      rawDatStruct._adresa     = GetString ( 4);
      rawDatStruct._ptt        = GetString ( 5);
      rawDatStruct._grad       = GetString ( 6);
      rawDatStruct._telefon    = GetString ( 7);
      rawDatStruct._telefax    = GetString ( 8);
      rawDatStruct._mail       = GetString ( 9);
      rawDatStruct._iban       = GetString (10);
      rawDatStruct._kontakt    = GetString (11);
      rawDatStruct._napomena   = GetString (12);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Kupdob kupdob_rec = new Kupdob();

      bool dbOK=true, isFirma;
      int errCount = 0, okCount=0;
      List<string> errMessageList = null;

      uint   currCentCD = 0, kupdobCD = 1000;
      string currCentTK = "";

      foreach(RawData rawDatStruct in this.RawDataList)
      {
         kupdob_rec.Memset0(0);

       //artikl_rec.KupdobCD = (rawDatStruct._kupdobCD);
         kupdob_rec.KupdobCD = ++kupdobCD;
         kupdob_rec.Url      = LimitedStr(rawDatStruct._kupdobCD,   ZXC.KpdbCI.url    );
         kupdob_rec.Oib      = LimitedStr(rawDatStruct._oib,        ZXC.KpdbCI.oib    );
         kupdob_rec.Url      = LimitedStr(rawDatStruct._kupdobCD  , ZXC.KpdbCI.url    );
         kupdob_rec.Naziv    = LimitedStr(rawDatStruct._kupdobName, ZXC.KpdbCI.naziv  );
         kupdob_rec.Ticker   = kupdob_rec.GenerateTicker(2);

         kupdob_rec.Ziro1    = LimitedStr(rawDatStruct._iban    ,   ZXC.KpdbCI.ziro1  );
         kupdob_rec.Tel1     = LimitedStr(rawDatStruct._telefon ,   ZXC.KpdbCI.tel1   );
         kupdob_rec.Fax      = LimitedStr(rawDatStruct._telefax ,   ZXC.KpdbCI.fax    );
         kupdob_rec.Ulica1   =                                      
         kupdob_rec.Ulica2   = LimitedStr(rawDatStruct._adresa  ,   ZXC.KpdbCI.ulica1 );
         kupdob_rec.PostaBr  = LimitedStr(rawDatStruct._ptt     ,   ZXC.KpdbCI.postaBr);
         kupdob_rec.Grad     = LimitedStr(rawDatStruct._grad    ,   ZXC.KpdbCI.grad   );
         kupdob_rec.Email    = LimitedStr(rawDatStruct._mail    ,   ZXC.KpdbCI.email  );
         kupdob_rec.Napom1   = LimitedStr(rawDatStruct._napomena,   ZXC.KpdbCI.napom1 );
         kupdob_rec.Ime      = LimitedStr(rawDatStruct._kontakt ,   ZXC.KpdbCI.ime    );

         dbOK = kupdob_rec.VvDao.ADDREC(conn, kupdob_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvKupdob_FRIGOTERM_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      #endregion Report Errors manager

   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.KupdobDao.GetSchemaColumnSize(cIdx));
   }

}


#endif


public class VvArtikl_ZAGRIA_Importer : VvDataRecordImporter
{
   private List<RawData> RawDataList { get; set; }

   internal struct RawData
   {
      internal string  _artiklCD;
      internal string  _artiklName;
      internal decimal _vpc;
      internal ushort  _garancYears;
    //internal string  _bCode;

   }

   public VvArtikl_ZAGRIA_Importer(XSqlConnection conn, string fullPathFileName, string delimiter) : base(conn, fullPathFileName, delimiter)
   {
      this.RawDataList = new List<RawData>();
   
      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);
   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawData rawDatStruct = new RawData();

      rawDatStruct._artiklCD    = GetString ( 1);
      rawDatStruct._artiklName  = GetString ( 2);
      rawDatStruct._vpc         = GetDecimal( 3);
      rawDatStruct._garancYears = GetUint16 ( 5);
    //rawDatStruct._bCode       = GetString ( 6);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {

      }

   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Artikl artRecToBeRwtreced, artikl_rec = new Artikl();

      bool dbOK, firstLine = true;
      int errCount = 0, okCount=0, addCount=0, rwtCount=0;
      List<string> errMessageList = null;
      string currGr1Name, currGr1CD="";
      string currGr2Name, currGr2CD="";
      VvLookUpItem luiGr1, luiGr2;

      foreach(RawData rawDataStruct in this.RawDataList)
      {
         if(firstLine) { firstLine = false; continue; } // prva linija u Excel-u je naslov kolona pa ju skipamo 

         if(rawDataStruct._artiklCD.IsEmpty() && rawDataStruct._artiklName.IsEmpty()) continue;

         #region Gr1 & Gr2 Manager

         if(rawDataStruct._artiklCD.IsEmpty()) 
         {
            if(NextCdIsEmpty(rawDataStruct)) // Gr2 Manager - Proizvodjaci 
            {
               currGr2Name = rawDataStruct._artiklName;

               luiGr2 = ZXC.luiListaGrupa2Artikla.SingleOrDefault(lui => lui.Name == currGr2Name);

               if(luiGr2 == null) // new Gr2 member 
               {
                  if(ZXC.luiListaGrupa2Artikla.Count.IsZero()) currGr2CD = "001";
                  else                                         currGr2CD = (ZXC.luiListaGrupa2Artikla.Select(lui => ZXC.ValOrZero_Int(lui.Cd)).Max() + 1).ToString("000");

                  ZXC.luiListaGrupa2Artikla.Add(new VvLookUpItem(currGr2CD, currGr2Name));
               }
               else
               {
                  currGr2CD = luiGr2.Cd;
               }
            }
            else // Gr1 Manager - Grupe artikala
            {
               currGr1Name = rawDataStruct._artiklName;

               luiGr1 = ZXC.luiListaGrupa1Artikla.SingleOrDefault(lui => lui.Name == currGr1Name);

               if(luiGr1 == null) // new Gr1 member 
               {
                  if(ZXC.luiListaGrupa1Artikla.Count.IsZero()) currGr1CD = "001";
                  else                                         currGr1CD = (ZXC.luiListaGrupa1Artikla.Select(lui => ZXC.ValOrZero_Int(lui.Cd)).Max() + 1).ToString("000");

                  ZXC.luiListaGrupa1Artikla.Add(new VvLookUpItem(currGr1CD, currGr1Name));
               }
               else
               {
                  currGr1CD = luiGr1.Cd;
               }
            }

            continue;
         }

         #endregion Gr1 & Gr2 Manager

         artikl_rec.Memset0(0);

         artikl_rec.ArtiklCD    = LimitedStr(rawDataStruct._artiklCD    , ZXC.ArtCI.artiklCD  );
         artikl_rec.ArtiklName  = LimitedStr(rawDataStruct._artiklName  , ZXC.ArtCI.artiklName);
         artikl_rec.Garancija   =   (ushort)(rawDataStruct._garancYears * 12                  );
         artikl_rec.ImportCij   =           (rawDataStruct._vpc                               );

         artikl_rec.SkladCD = "VPSK";

         artikl_rec.Grupa1CD = currGr1CD;
         artikl_rec.Grupa2CD = currGr2CD;

         artRecToBeRwtreced = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artikl_rec.ArtiklCD);

         #region RWTREC

         if(artRecToBeRwtreced != null) // dis uan iz old, Let's RWTREC 
         {
            artRecToBeRwtreced.BeginEdit();

            // NE! jerbo ako je bio dupli onda ode u a*ratz 
            //artRecToBeRwtreced.ArtiklName = artikl_rec.ArtiklName;
            artRecToBeRwtreced.Garancija  = artikl_rec.Garancija;
            artRecToBeRwtreced.ImportCij  = artikl_rec.ImportCij;
            artRecToBeRwtreced.SkladCD    = artikl_rec.SkladCD;
            artRecToBeRwtreced.Grupa1CD   = artikl_rec.Grupa1CD;
            artRecToBeRwtreced.Grupa2CD   = artikl_rec.Grupa2CD;

            if(artRecToBeRwtreced.EditedHasChanges())
            {
               dbOK = artRecToBeRwtreced.VvDao.RWTREC(conn, artRecToBeRwtreced, false, false);
               
               if(dbOK) rwtCount++;
            }
            else
            {
               dbOK = true;
            }
            artRecToBeRwtreced.EndEdit();
         }

         #endregion RWTREC

         #region ADDREC

         else // dis uan iz new, Let's ADDREC 
         {
            dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
            
            if(dbOK) addCount++;

            if(dbOK == false && ZXC.sqlErrNo == (int)XSqlErrorCode.DuplicateKeyEntry && ZXC.sqlErrMessage.Contains("BY_artName")) // Meaning, ADDREC failed bikoz Microline's artiklName is duplicated (artiklCD is ok, I guess?) 
            {
               artikl_rec.ArtiklName2 = artikl_rec.ArtiklName;
               artikl_rec.ArtiklName  = LimitedStr(artikl_rec.ArtiklName + " (" + artikl_rec.ArtiklCD + ")", ZXC.ArtCI.artiklName);
               dbOK = artikl_rec.VvDao.ADDREC(conn, artikl_rec, false, false, false, /*false*/ true); // don't report errors, save them in the list
               if(dbOK) addCount++;
            }
         }
         
         #endregion ADDREC

         #region Report Errors manager

         if(dbOK == false)
         {
            errCount++;

            if(errMessageList == null) errMessageList = new List<string>();

            errMessageList.Add("Err " + errCount.ToString("0000") + ": " + ZXC.sqlErrMessage);
         }
         else okCount++;

         #endregion Report Errors manager

      } // foreach(RawData rawDatStruct in this.RawDataList) 

      #region Gr1 & Gr2 Save Manager

      string origDbName = ZXC.TheMainDbConnection.Database;

      VvDaoBase.SaveLookUpListToSqlTable(ZXC.luiListaGrupa1Artikla);

      VvDaoBase.SaveLookUpListToSqlTable(ZXC.luiListaGrupa2Artikla);

      ZXC.SetMainDbConnDatabaseName(origDbName); // jer SaveLookUpListToSqlTable promijeni na 'vvektor' ... 

      #endregion Gr1 & Gr2 Save Manager

      #region Report Errors manager

      if(errCount.NotZero())
      {
         string fName = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\VvArtikl_ZAGRIA_Importer_ErrorList.txt";

         using(StreamWriter sw = new StreamWriter(fName, false, Encoding.GetEncoding(1250)))
         {
            foreach(string error in errMessageList)
            {
               sw.WriteLine(error);
            }
         }

         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Bilo je {0} error-a.\n\nVidi datoteku: {1}.\n\nOK records: {2}. ", errCount, fName, okCount);
      }

      ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Novih kartica: {0}.\n\nIzmijenjenih kartica: {1}.", addCount, rwtCount);

      #endregion Report Errors manager

   }

   private bool NextCdIsEmpty(RawData rawData)
   {
      int nextIdx = RawDataList.IndexOf(rawData) + 1;

      if(nextIdx.IsZero()) return false; // znaci thisIdx=-1 + 1 = 0 ... meaning nije nasao rawData u RawDataList 

      return RawDataList[nextIdx]._artiklCD.IsEmpty();
   }

   private string LimitedStr(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.ArtiklDao.GetSchemaColumnSize(cIdx));
   }

}

public class VvXtransImporter : VvDataRecordImporter
{
   #region Local Propertiz

   public  enum ImportKindEnum { DucatiCijDaItaly }
   public       ImportKindEnum ImportKind  { get; set; }
   public       bool           IsZagria    { get { return ZXC.CURR_prjkt_rec.Ticker.StartsWith("ZAGRIA"); } }

   private List<RawTransData> RawDataList { get; set; }

   private uint   offFakturCount { get; set; }
   private uint   startDokNum    { get; set; }
   private ushort serial         { get; set; }
   private string F_flagC        { get; set; }

   private Mixer mixer_rec;

   private Artikl artiklSifrar_rec;
   private Kupdob kupdobSifrar_rec;

   #endregion Local Propertiz

   internal struct RawTransData // Ovu sad strukturu krojis kako ti pase tj. kako ti importni fajl diktira... tu navodis polja iz importFajla, a dolje ih spajas sa trans-om ili faktur-om 
   {
      // trans memberz 
      internal string            t_tt       ;
      internal uint              t_ttNum    ;
      internal DateTime          t_dokDate  ;
      internal string            t_skladCD  ;
      internal string            t_artiklCD ;
      internal decimal           t_kol      ;
      internal decimal           t_cij      ;
      internal decimal           t_wanted   ;
      internal ZXC.MalopCalcKind t_mCalcKind;

      internal uint              t_kupdobCD ;
      internal decimal           t_pdvSt    ;
      internal decimal           t_rbt1St   ;
      internal decimal           t_rbt2St   ;
      internal decimal           t_ztrUk    ;
                                 
      internal string            t_externArtCD  ;
      internal string            t_externArtName;
      internal string            t_groupCD      ;
      internal string            t_someSetCD    ;
      internal string            t_hrihyCD      ;
      internal string            t_subst        ;

      internal XtransResultStruct results;

   }

   public VvXtransImporter(XSqlConnection conn, string fullPathFileName, string delimiter /*, bool isForceSomething*/) : base(conn, fullPathFileName, delimiter)
   {
           
      if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("DUCATI")) ImportKind = ImportKindEnum.DucatiCijDaItaly;
      //else if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("SENSO"))  ImportKind = ImportKindEnum.SensoPS;

      this.RawDataList = new List<RawTransData>();

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);
      
      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

   }

   protected override void ProcessLine(XSqlConnection conn, ref ushort line, bool addrecGoesOnPostprocess)
   {
      RawTransData rawDatStruct = new RawTransData();

      //     if(FullPathFileName.Contains("off"))               { FillFakturRec_Offix (); return; }
      //else if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("SENSO"))    FillRawDataStruct_Senso (ref rawDatStruct);
      //else if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("ZAGRIA"))   FillRawDataStruct_Microline4Zagria(ref rawDatStruct);
      //else                                                    { FillFakturRec_Offix (); return; }

      switch(ImportKind)
      {
         case ImportKindEnum.DucatiCijDaItaly: FillRawDataStruct_DucatiCijDaItaly(ref rawDatStruct); break;
         //case ImportKindEnum.SensoPS        : FillRawDataStruct_Senso            (ref rawDatStruct); break;
      }

      //CalcRawResults(ref rawDatStruct);

      if(addrecGoesOnPostprocess == true) // ne bus jos nis ADDREC-al, samo spremi u listu za PostProcessLines(). 
      {
         this.RawDataList.Add(rawDatStruct);
      }
      else // Odmah ADDREC-aj, PostProcessLines() nije potrebno. 
      {
      }

   }

   private void SetAllKupdobFields(uint f_firma_cd)
   {
      kupdobSifrar_rec = VvUserControl.KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD == f_firma_cd);

      if(kupdobSifrar_rec == null) return;

      mixer_rec.KupdobCD   = kupdobSifrar_rec.KupdobCD;
      mixer_rec.KupdobName = kupdobSifrar_rec.Naziv;
      mixer_rec.KupdobTK   = kupdobSifrar_rec.Ticker;
   }

   private void OLD_FillRawDataStruct_DucatiCijDaItaly(ref RawTransData rawDatStruct)
   {
      // A -  1 
      // B -  2 
      // C -  3 
      // D -  4 
      // E -  5 
      // F -  6 
      // G -  7 
      // H -  8 
      // I -  9 
      // J - 10 
      // K - 11 
      // L - 12 
      // M - 13 
      // N - 14 
      // O - 15 
      // P - 16 
      // Q - 17 

      rawDatStruct.t_externArtCD   = GetString  ( 1); // A "Material" 
      rawDatStruct.t_externArtName = GetString  ( 3); // C "Inspection text (Material description)" 
      rawDatStruct.t_groupCD       = GetString  ( 6); // F "Status per distr.cat." 
      rawDatStruct.t_someSetCD     = GetString  ( 8); // H "Material set (Discount class)" 
      rawDatStruct.t_hrihyCD       = GetString  ( 9); // I "Product hierarchy" 
      rawDatStruct.t_subst         = GetString  (10); // J "Material (Substitute)" 
      rawDatStruct.t_cij           = GetDecimal (11); // K "Price according to the conditions of sale(Sales price, taxes not)" 

      //string externArtCD = rawDatStruct.t_externArtCD;
      //artiklSifrar_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(vvDR => vvDR.ArtiklCD2 == externArtCD); // po staroj Pantelijinoj sifri trazimo 
      //if(artiklSifrar_rec != null && externArtCD.NotEmpty()) rawDatStruct.t_artiklCD = artiklSifrar_rec.ArtiklCD;
      //else                                                   rawDatStruct.t_artiklCD = "XY-" + externArtCD;

   }

   private void FillRawDataStruct_DucatiCijDaItaly(ref RawTransData rawDatStruct)
   {
      // A -  1 
      // B -  2 
      // C -  3 
      // D -  4 
      // E -  5 
      // F -  6 
      // G -  7 
      // H -  8 
      // I -  9 
      // J - 10 
      // K - 11 
      // L - 12 
      // M - 13 
      // N - 14 
      // O - 15 
      // P - 16 
      // Q - 17 

#if OLDDD
      rawDatStruct.t_externArtCD   = GetString  ( 1); // A "Material" 
      rawDatStruct.t_externArtName = GetString  ( 3); // C "Inspection text (Material description)" 
      rawDatStruct.t_groupCD       = GetString  ( 6); // F "Status per distr.cat." 
      rawDatStruct.t_someSetCD     = GetString  ( 8); // H "Material set (Discount class)" 
      rawDatStruct.t_hrihyCD       = GetString  ( 9); // I "Product hierarchy" 
      rawDatStruct.t_subst         = GetString  (10); // J "Material (Substitute)" 
      rawDatStruct.t_cij           = GetDecimal (11); // K "Price according to the conditions of sale(Sales price, taxes not)" 
#endif

      rawDatStruct.t_externArtCD   = GetString  ( 1); // A "Material" 
      rawDatStruct.t_externArtName = GetString  ( 2); // B "Material Description" 
      rawDatStruct.t_groupCD       = GetString  ( 3); // C "Status per distr.cat." 
      rawDatStruct.t_someSetCD     = GetString  ( 5); // E "Material set (Discount class)" 
      rawDatStruct.t_hrihyCD       = GetString  ( 6); // F "Product hierarchy" 
      rawDatStruct.t_subst         = GetString  ( 7); // G "Substitute" 
      rawDatStruct.t_cij           = GetDecimal ( 8); // H "Retail Price" 
      //string externArtCD = rawDatStruct.t_externArtCD;
      //artiklSifrar_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(vvDR => vvDR.ArtiklCD2 == externArtCD); // po staroj Pantelijinoj sifri trazimo 
      //if(artiklSifrar_rec != null && externArtCD.NotEmpty()) rawDatStruct.t_artiklCD = artiklSifrar_rec.ArtiklCD;
      //else                                                   rawDatStruct.t_artiklCD = "XY-" + externArtCD;

   }

   private string Limited_Xtrans(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.XtransDao.GetSchemaColumnSize(cIdx));
   }
   private string Limited_Mixer(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.MixerDao.GetSchemaColumnSize(cIdx));
   }
   private string GetDocumentGroupID(RawTransData rawData)
   {
      return rawData.t_skladCD + rawData.t_dokDate.ToShortDateString() + rawData.t_tt + rawData.t_ttNum;
   }

   protected override void PostProcessLines(XSqlConnection conn)
   {
      Xtrans xtrans_rec = null;

      var rawDataListSorted = RawDataList.OrderBy(raw => raw.t_skladCD).ThenBy(raw => raw.t_dokDate).ThenBy(raw => raw.t_tt).ThenBy(raw => raw.t_ttNum);

      //RawDataList.ForEach(rawData => CalcRawResults(ref rawData));

      ushort line = 0;
      string currGroupID = null;

      string  artiklName="", theTT="";
      string      f_kupdobName = "";
      string      f_kupdobTK   = "";
      //decimal s_ukK = 0M, s_ukKC = 0M, s_ukKCR = 0M, s_ukKCRM = 0M, s_ukKCRP = 0M, s_ukZavisni = 0M, s_ukRbt1 = 0M, s_ukOsn25m = 0M, s_ukPdv = 0M, s_ukPdv25m = 0M;
      int     s_trnCount = 0;

      
      switch(ImportKind) // DAJ new ZAGLAVLJE 
      {
         case ImportKindEnum.DucatiCijDaItaly: mixer_rec = CreateMixerRec_DucatiCijDaItaly(conn); break;
      }

      foreach(RawTransData rawData in rawDataListSorted)
      {
         if(currGroupID != GetDocumentGroupID(rawData)) // Novi dokument (zaglavlje) 
         {
            line = 0;
            currGroupID  = GetDocumentGroupID(rawData);

            s_trnCount  = RawDataList.Count(raw => GetDocumentGroupID(raw) == currGroupID);
         }

         //artiklSifrar_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(vvDR => vvDR.ArtiklCD == rawData.t_artiklCD);
         //kupdobSifrar_rec = VvUserControl.KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD == rawData.t_kupdobCD);

         //if(artiklSifrar_rec != null && rawData.t_artiklCD.NotEmpty()){ artiklName = artiklSifrar_rec.ArtiklName;                                  }
         //else                                                         { artiklName = "---!!!--- " + rawData.t_artiklCD; /*rawData.artiklCD = "";*/ }

         //if(kupdobSifrar_rec != null && rawData.t_kupdobCD.NotZero()) 
         //{ 
         //   f_kupdobName = kupdobSifrar_rec.Naziv   ; 
         //   f_kupdobTK   = kupdobSifrar_rec.Ticker  ; 
         //}
         //else                                                         
         //{ 
         //   f_kupdobName = rawData.t_kupdobCD.ToString(); 
         //   f_kupdobTK   = "";
         //}

         
         switch(ImportKind) // DAJ novu stavku 
         {
            case ImportKindEnum.DucatiCijDaItaly: xtrans_rec = CreateXtransRec_DucatiCijDaItaly(rawData); break;
         }

         MixerDao.AutoSetMixer(conn, ref line, mixer_rec, xtrans_rec);

      } // foreach(RawData rawData in rawDataListSorted) 
   }

   private Mixer CreateMixerRec_DucatiCijDaItaly(XSqlConnection conn)
   {
      Mixer mixer_rec = new Mixer();

      mixer_rec.TT          = Mixer.TT_EXT_CIJ;
      mixer_rec.DokDate     = VvSQL.GetServer_DateTime_Now(conn);
      mixer_rec.DevName     = ZXC.ValutaNameEnum.EUR.ToString();
      mixer_rec.Napomena    = Limited_Mixer(FullPathFileName, ZXC.MixCI.napomena);

      string reallyFullPathFileName = Environment.CurrentDirectory + "\\" + FullPathFileName;
      mixer_rec.ExternLink1 = Limited_Mixer(reallyFullPathFileName, ZXC.MixCI.externLink1);

      return mixer_rec;
   }

   private Xtrans CreateXtransRec_DucatiCijDaItaly(RawTransData rawData)
   {
      Xtrans xtrans_rec = new Xtrans();

      xtrans_rec.T_artiklCD     = Limited_Xtrans(rawData.t_externArtCD  , ZXC.XtrCI.t_artiklCD    ); // ITALY ArtiklCD   
      xtrans_rec.T_artiklName   = Limited_Xtrans(rawData.t_externArtName, ZXC.XtrCI.t_artiklName  ); // ITALY ArtiklName 
      xtrans_rec.T_strA_2/*4*/  = Limited_Xtrans(rawData.t_groupCD      , ZXC.XtrCI.t_strA_2      ); // F "Status per distr.cat." (nekakva grupa) 
      xtrans_rec.T_strC_2       = Limited_Xtrans(rawData.t_someSetCD    , ZXC.XtrCI.t_strC_2      ); // H "Material set (Discount class)" (rabatna grupa?) 
      xtrans_rec.T_strB_2/*4*/  = Limited_Xtrans(rawData.t_hrihyCD      , ZXC.XtrCI.t_strB_2      ); // I "Product hierarchy" npr ="R11" 
      xtrans_rec.T_vezniDokA_64 = Limited_Xtrans(rawData.t_subst        , ZXC.XtrCI.t_vezniDokA_64); // J "Material (Substitute)" (sifra zamjenskog, alternativnog artikla?) 
      xtrans_rec.T_moneyA       =                rawData.t_cij                                     ; // K "Price according to the conditions of sale(Sales price, taxes not)" 

      xtrans_rec.T_konto = Limited_Xtrans(ZXC.ValutaNameEnum.EUR.ToString(), ZXC.XtrCI.t_konto); // podmetni DevName u t_konto 

      return xtrans_rec;
   }

}

#region PPUK HrSume

#region ByQ 

public struct HrSumeZaglavlje
{
   public string   IDzaglav         { get; set; } // >13075750</IDzaglav>
   public int      upravaID         { get; set; } // >8</upravaID>
   public string   NazivUprave      { get; set; } // >Uprava šuma Podružnica Sisak</NazivUprave>
   public int      sumarijaID       { get; set; } // >115</sumarijaID>
   public string   NazivSumarije    { get; set; } // >Šumarija Sisak</NazivSumarije>
   public int      vrstaDokumentaID { get; set; } // >4</vrstaDokumentaID>
   public string   BrojDok          { get; set; } // >OH556</BrojDok>
   public int      stranica         { get; set; } // >268</stranica>
   public DateTime datumpd          { get; set; } // >2013-02-15T00:00:00+01:00</datumpd>
   public int      utovarID         { get; set; } // >48055</utovarID>
   public string   NazivSkladista   { get; set; } // >BREZOVICA I - NOVA CESTA</NazivSkladista>
   public int      skladisteID      { get; set; } // >2554807</skladisteID>
   public string   skladisteNaziv   { get; set; } // >PILANA  PUKANIĆ  d.o.o.</skladisteNaziv>
   public string   vozac            { get; set; } // >KAMIONI VANJSKI</vozac>
   public string   vozilo           { get; set; } // >ZG4000PP,ZG5000PP</vozilo>
   public DateTime vrijeme          { get; set; } // >08:27</vrijeme>
   public int      otpremioID       { get; set; } // >1390</otpremioID>
   public string   otpremio         { get; set; } // >Renata Majetić</otpremio>
   public int      maticniBroj      { get; set; } // >83432361102  </maticniBroj>
   public string   kupac            { get; set; } // >PUKANIĆ d.o.o.                                    </kupac>

   public void FillFromXElement(XElement zaglavljeElement)
   {
      if(zaglavljeElement.Element("IDzaglav")        != null) IDzaglav         =                                                      zaglavljeElement.Element("IDzaglav")        .Value;
      if(zaglavljeElement.Element("upravaID")        != null) upravaID         = ZXC.ValOrZero_Int(                                   zaglavljeElement.Element("upravaID")        .Value);
      if(zaglavljeElement.Element("NazivUprave")     != null) NazivUprave      =                                                      zaglavljeElement.Element("NazivUprave")     .Value;
      if(zaglavljeElement.Element("sumarijaID")      != null) sumarijaID       = ZXC.ValOrZero_Int(                                   zaglavljeElement.Element("sumarijaID")      .Value);
      if(zaglavljeElement.Element("NazivSumarije")   != null) NazivSumarije    =                                                      zaglavljeElement.Element("NazivSumarije")   .Value;
      if(zaglavljeElement.Element("vrstaDokumentaID")!= null) vrstaDokumentaID = ZXC.ValOrZero_Int(                                   zaglavljeElement.Element("vrstaDokumentaID").Value);
      if(zaglavljeElement.Element("BrojDok")         != null) BrojDok          =                                                      zaglavljeElement.Element("BrojDok")         .Value;
      if(zaglavljeElement.Element("stranica")        != null) stranica         = ZXC.ValOrZero_Int(                                   zaglavljeElement.Element("stranica")        .Value);
      if(zaglavljeElement.Element("datumpd")         != null) datumpd          = ZXC.ValOr_01010001_DateTime_Import_yyyy_MM_dd_Format(zaglavljeElement.Element("datumpd")         .Value.Substring(0, 10));
      if(zaglavljeElement.Element("utovarID")        != null) utovarID         = ZXC.ValOrZero_Int(                                   zaglavljeElement.Element("utovarID")        .Value);
      if(zaglavljeElement.Element("NazivSkladista")  != null) NazivSkladista   =                                                      zaglavljeElement.Element("NazivSkladista")  .Value;
      if(zaglavljeElement.Element("skladisteID")     != null) skladisteID      = ZXC.ValOrZero_Int(                                   zaglavljeElement.Element("skladisteID")     .Value);
      if(zaglavljeElement.Element("skladisteNaziv")  != null) skladisteNaziv   =                                                      zaglavljeElement.Element("skladisteNaziv")  .Value;
      if(zaglavljeElement.Element("vozac")           != null) vozac            =                                                      zaglavljeElement.Element("vozac")           .Value;
      if(zaglavljeElement.Element("vozilo")          != null) vozilo           =                                                      zaglavljeElement.Element("vozilo")          .Value;
      if(zaglavljeElement.Element("vrijeme")         != null) vrijeme          = ZXC.ValOr_01010001_DateTime_Import_HHmm_Format(      zaglavljeElement.Element("vrijeme")         .Value);
      if(zaglavljeElement.Element("otpremioID")      != null) otpremioID       = ZXC.ValOrZero_Int(                                   zaglavljeElement.Element("otpremioID")      .Value);
      if(zaglavljeElement.Element("otpremio")        != null) otpremio         =                                                      zaglavljeElement.Element("otpremio")        .Value;
      if(zaglavljeElement.Element("maticniBroj")     != null) maticniBroj      = ZXC.ValOrZero_Int(                                   zaglavljeElement.Element("maticniBroj")     .Value);
      if(zaglavljeElement.Element("kupac")           != null) kupac            =                                                      zaglavljeElement.Element("kupac")           .Value;
   }
}

public struct HrSumeStavka
{
      public string   IDzaglav          { get; set; } // >13075750</IDzaglav>
      public string   IDstavka          { get; set; } // >259397590</IDstavka>
      public string   BrojUnosa         { get; set; } // >1</BrojUnosa>
      public string   vrstaID           { get; set; } // >1</vrstaID>
      public string   NazivVrste        { get; set; } // >HRAST LUŽNJAK</NazivVrste>
      public string   sortimentID       { get; set; } // >3</sortimentID>
      public string   NazivSortimenta   { get; set; } // >Furnirski trupci F1</NazivSortimenta>
      public string   CjRazred          { get; set; } // >50-59</CjRazred>
      public string   CjDuljina         { get; set; } // >&gt;2</CjDuljina>
      public string   JMID              { get; set; } // >1</JMID>
      public string   NazivJM           { get; set; } // >TRU</NazivJM>
      public string   sumploc           { get; set; } // >SI</sumploc>
      public string   BrojPlocice       { get; set; } // >033333</BrojPlocice>
      public decimal  Duljina           { get; set; } // >440</Duljina>
      public decimal  Promjer           { get; set; } // >60</Promjer>
      public decimal  PromjerBK         { get; set; } // >56</PromjerBK>
      public decimal  masam3            { get; set; } // >1.08</masam3>

      public string VvArtiklName { get { return NazivVrste + " " + NazivSortimenta + " " + CjRazred + " " + CjDuljina; } }

      public void FillFromXElement(XElement stavkaElement)
      {
         if(stavkaElement.Element("IDzaglav")         != null) IDzaglav         = stavkaElement.Element("IDzaglav")         .Value;
         if(stavkaElement.Element("IDstavka")         != null) IDstavka         = stavkaElement.Element("IDstavka")         .Value;
         if(stavkaElement.Element("BrojUnosa")        != null) BrojUnosa        = stavkaElement.Element("BrojUnosa")        .Value;
         if(stavkaElement.Element("vrstaID")          != null) vrstaID          = stavkaElement.Element("vrstaID")          .Value;
         if(stavkaElement.Element("NazivVrste")       != null) NazivVrste       = stavkaElement.Element("NazivVrste")       .Value;
         if(stavkaElement.Element("sortimentID")      != null) sortimentID      = stavkaElement.Element("sortimentID")      .Value;
         if(stavkaElement.Element("NazivSortimenta")  != null) NazivSortimenta  = stavkaElement.Element("NazivSortimenta")  .Value;
         if(stavkaElement.Element("CjRazred")         != null) CjRazred         = stavkaElement.Element("CjRazred")         .Value;
         if(stavkaElement.Element("CjDuljina")        != null) CjDuljina        = stavkaElement.Element("CjDuljina")        .Value;
         if(stavkaElement.Element("JMID")             != null) JMID             = stavkaElement.Element("JMID")             .Value;
         if(stavkaElement.Element("NazivJM")          != null) NazivJM          = stavkaElement.Element("NazivJM")          .Value;
         if(stavkaElement.Element("sumploc")          != null) sumploc          = stavkaElement.Element("sumploc")          .Value;
         if(stavkaElement.Element("BrojPlocice")      != null) BrojPlocice      = stavkaElement.Element("BrojPlocice")      .Value;
         if(stavkaElement.Element("Duljina")  != null) Duljina          = ZXC.ValOrZero_Decimal(stavkaElement.Element("Duljina")  .Value                  , 0);
         if(stavkaElement.Element("Promjer")  != null) Promjer          = ZXC.ValOrZero_Decimal(stavkaElement.Element("Promjer")  .Value                  , 0);
         if(stavkaElement.Element("PromjerBK")!= null) PromjerBK        = ZXC.ValOrZero_Decimal(stavkaElement.Element("PromjerBK").Value                  , 0);
         if(stavkaElement.Element("masam3")   != null) masam3           = ZXC.ValOrZero_Decimal(stavkaElement.Element("masam3")   .Value.Replace('.', ','), 2);
      }
}

public struct HrSumeRekapitulacija
{
      public string   IDZaglav        { get; set; } // >13075750</IDZaglav>
      public string   NazivVrste      { get; set; } // >HRAST LUŽNJAK</NazivVrste>
      public string   NazivSortimenta { get; set; } // >Furnirski trupci F1</NazivSortimenta>
      public string   Razred          { get; set; } // >&gt;70  </Razred>
      public string   Duljina         { get; set; } // >&gt;2</Duljina>
      public decimal  Komada          { get; set; } // >2</Komada>
      public decimal  masa            { get; set; } // >4.62</masa>

      public void FillFromXElement(XElement stavkaElement)
      {
         if(stavkaElement.Element("IDZaglav")       != null) IDZaglav        =                       stavkaElement.Element("IDZaglav")       .Value;
         if(stavkaElement.Element("NazivVrste")     != null) NazivVrste      =                       stavkaElement.Element("NazivVrste")     .Value;
         if(stavkaElement.Element("NazivSortimenta")!= null) NazivSortimenta =                       stavkaElement.Element("NazivSortimenta").Value;
         if(stavkaElement.Element("Razred")         != null) Razred          =                       stavkaElement.Element("Razred")         .Value;
         if(stavkaElement.Element("Duljina")        != null) Duljina         =                       stavkaElement.Element("Duljina")        .Value;
         if(stavkaElement.Element("Komada")         != null) Komada          = ZXC.ValOrZero_Decimal(stavkaElement.Element("Komada")         .Value                  , 0);
         if(stavkaElement.Element("masa")           != null) masa            = ZXC.ValOrZero_Decimal(stavkaElement.Element("masa")           .Value.Replace('.', ','), 2);
      }

}

public class HrSumePopratnica
{
   public List<HrSumeZaglavlje     > ZaglavljaList      { get; set; }
   public List<HrSumeStavka        > StavkeList         { get; set; }
   public List<HrSumeRekapitulacija> RekapitulacijeList { get; set; }

   public HrSumePopratnica(string fullXmlFilePath)
   {
      ZaglavljaList      = new List<HrSumeZaglavlje     >();
      StavkeList         = new List<HrSumeStavka        >();
      RekapitulacijeList = new List<HrSumeRekapitulacija>();

      XDocument xDoc1 = XDocument.Load(fullXmlFilePath);

      foreach(XElement zaglavljeElement in xDoc1.Descendants("Zaglavlja"))
      {
         AddZaglavlje(zaglavljeElement);

         foreach(XElement stavkaElement in zaglavljeElement.Descendants("Stavke"))
         {
            AddStavka(stavkaElement);
         }
         foreach(XElement rekapitulacijaElement in zaglavljeElement.Descendants("Rekapitulacija"))
         {
            AddRekapitulacija(rekapitulacijaElement);
         }
      }
   }

   private void AddZaglavlje(XElement zaglavljeElement)
   {
      HrSumeZaglavlje theZaglavlje = new HrSumeZaglavlje();
      theZaglavlje.FillFromXElement(zaglavljeElement);

      ZaglavljaList.Add(theZaglavlje);
   }

   private void AddStavka(XElement StavkaElement)
   {
      HrSumeStavka theStavka = new HrSumeStavka();
      theStavka.FillFromXElement(StavkaElement);

      StavkeList.Add(theStavka);
   }

   private void AddRekapitulacija(XElement RekapitulacijaElement)
   {
      HrSumeRekapitulacija theRekapitulacija = new HrSumeRekapitulacija();
      theRekapitulacija.FillFromXElement(RekapitulacijaElement);

      RekapitulacijeList.Add(theRekapitulacija);
   }

}

#endregion ByQ

#region PPUK Popratnice Class from XSD tool

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.5466
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class Popratnice
{

   private PopratniceZaglavlja[] itemsField;

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute("Zaglavlja", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public PopratniceZaglavlja[] Items
   {
      get
      {
         return this.itemsField;
      }
      set
      {
         this.itemsField = value;
      }
   }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class PopratniceZaglavlja
{

   private int iDzaglavField;

   private byte upravaIDField;

   private string nazivUpraveField;

   private short sumarijaIDField;

   private string nazivSumarijeField;

   private byte vrstaDokumentaIDField;

   private string brojDokField;

   private int stranicaField;

   private System.DateTime datumpdField;

   private int utovarIDField;

   private bool utovarIDFieldSpecified;

   private string nazivSkladistaField;

   private int skladisteIDField;

   private bool skladisteIDFieldSpecified;

   private string skladisteNazivField;

   private string vozacField;

   private string voziloField;

   private string vagonField;

   private string vrijemeField;

   private int otpremioIDField;

   private bool otpremioIDFieldSpecified;

   private string otpremioField;

   private string maticniBrojField;

   private string kupacField;

   private PopratniceZaglavljaStavke[] stavkeField;

   private PopratniceZaglavljaRekapitulacija[] rekapitulacijaField;

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public int IDzaglav
   {
      get
      {
         return this.iDzaglavField;
      }
      set
      {
         this.iDzaglavField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public byte upravaID
   {
      get
      {
         return this.upravaIDField;
      }
      set
      {
         this.upravaIDField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string NazivUprave
   {
      get
      {
         return this.nazivUpraveField;
      }
      set
      {
         this.nazivUpraveField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public short sumarijaID
   {
      get
      {
         return this.sumarijaIDField;
      }
      set
      {
         this.sumarijaIDField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string NazivSumarije
   {
      get
      {
         return this.nazivSumarijeField;
      }
      set
      {
         this.nazivSumarijeField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public byte vrstaDokumentaID
   {
      get
      {
         return this.vrstaDokumentaIDField;
      }
      set
      {
         this.vrstaDokumentaIDField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string BrojDok
   {
      get
      {
         return this.brojDokField;
      }
      set
      {
         this.brojDokField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public int stranica
   {
      get
      {
         return this.stranicaField;
      }
      set
      {
         this.stranicaField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public System.DateTime datumpd
   {
      get
      {
         return this.datumpdField;
      }
      set
      {
         this.datumpdField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public int utovarID
   {
      get
      {
         return this.utovarIDField;
      }
      set
      {
         this.utovarIDField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool utovarIDSpecified
   {
      get
      {
         return this.utovarIDFieldSpecified;
      }
      set
      {
         this.utovarIDFieldSpecified = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string NazivSkladista
   {
      get
      {
         return this.nazivSkladistaField;
      }
      set
      {
         this.nazivSkladistaField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public int skladisteID
   {
      get
      {
         return this.skladisteIDField;
      }
      set
      {
         this.skladisteIDField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool skladisteIDSpecified
   {
      get
      {
         return this.skladisteIDFieldSpecified;
      }
      set
      {
         this.skladisteIDFieldSpecified = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string skladisteNaziv
   {
      get
      {
         return this.skladisteNazivField;
      }
      set
      {
         this.skladisteNazivField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string vozac
   {
      get
      {
         return this.vozacField;
      }
      set
      {
         this.vozacField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string vozilo
   {
      get
      {
         return this.voziloField;
      }
      set
      {
         this.voziloField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string vagon
   {
      get
      {
         return this.vagonField;
      }
      set
      {
         this.vagonField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string vrijeme
   {
      get
      {
         return this.vrijemeField;
      }
      set
      {
         this.vrijemeField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public int otpremioID
   {
      get
      {
         return this.otpremioIDField;
      }
      set
      {
         this.otpremioIDField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool otpremioIDSpecified
   {
      get
      {
         return this.otpremioIDFieldSpecified;
      }
      set
      {
         this.otpremioIDFieldSpecified = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string otpremio
   {
      get
      {
         return this.otpremioField;
      }
      set
      {
         this.otpremioField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string maticniBroj
   {
      get
      {
         return this.maticniBrojField;
      }
      set
      {
         this.maticniBrojField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string kupac
   {
      get
      {
         return this.kupacField;
      }
      set
      {
         this.kupacField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute("Stavke", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public PopratniceZaglavljaStavke[] Stavke
   {
      get
      {
         return this.stavkeField;
      }
      set
      {
         this.stavkeField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute("Rekapitulacija", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public PopratniceZaglavljaRekapitulacija[] Rekapitulacija
   {
      get
      {
         return this.rekapitulacijaField;
      }
      set
      {
         this.rekapitulacijaField = value;
      }
   }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class PopratniceZaglavljaStavke
{

   private string CjRazred_clean { get { return CjRazred.TrimEnd(' '); } }
  
 // 06.10.2016. HS promjenile popratnice - stavili su puno praznog prostora
   private string NazivVrste_clean      { get { return NazivVrste     .Trim(); } }
   private string NazivSortimenta_clean { get { return NazivSortimenta.Trim(); } }

 //public string VvArtiklName { get { return NazivVrste       + " " + NazivSortimenta       + " " + CjRazred_clean + " " + CjDuljina; } } 
   public string VvArtiklName { get { return NazivVrste_clean + " " + NazivSortimenta_clean + " " + CjRazred_clean + " " + CjDuljina; } }

   public string VvArtiklCD2 { get { return vrstaID + ":" + sortimentID + ":" + CjRazred_clean; } }

   public string VvArtiklForExport { get { return VvArtiklName + "\t" + VvArtiklCD2 + "\t" + OffixArtiklCD + "\t" + offVrstaCD + "\t" + offSortimentCD + "\t" + offRazredCD; } }

   public string OffixArtiklCD { get { return offVrstaCD + offSortimentCD + offRazredCD; } }

   private string offVrstaCD
   {
      get
      {
         switch(vrstaID)
         {
            case  1: return "HL";   //HRAST LUŽNJAK
            case  2: return "HK";   //HRAST KITNJAK
            case 10: return "BO";   //BUKVA OBIČNA
            case 12: return "JP";   //JASEN POLJSKI
            case 16: return "GO";   //GRAB OBIČNI
            case 31: return "TD";   //TREŠNJA DIVLJA
            case 39: return "TB";   //OSTALA TVRDA BJELOGORICA
            case 43: return "JC";   //JOHA CRNA
            case 50: return "DT";   //DOMAĆE TOPOLE
            case 59: return "MB";   //OSTALA MEKA BJELOGORICA
            case 61: return "OS";   //OBIČNA SMREKA
            case 62: return "RB";   //BIJELI-OBIČNI BOR
            case 63: return "RC";   //CRNI BOR
            case 69: return "RO";   //BOROVI OSTALI

            default: return "XY";
         }
      }
   }

   private string offSortimentCD
   {
      get
      {
         switch(sortimentID)
         {
            case  3: return "F1";   //Furnirski trupci F1
            case  4: return "F2";   //Furnirski trupci F2
            case  6: return "F0";   //Furnirski trupci
            case  7: return "L0";   //Trupci za ljuštenje
            case  8: return "P1";   //Pilanski trupci I
            case  9: return "P2";   //Pilanski trupci II
            case 10: return "P3";   //Pilanski trupci III
            case 12: return "TO";   //Tanka oblovina

            default: return "XY";
         }
      }
   }

   private string offRazredCD
   {
      get
      {
         switch(CjRazred_clean)
         {
            case "15-19": return "1519";
            case "20-24": return "2024";
            case "20-39": return "2039";
            case "25-39": return "2539";
            case "30-39": return "3039";
            case "35-39": return "3539";
            case "40-49": return "4049";
            case "50-59": return "5059";
            case "60-69": return "6069";
            case ">70"  : return ">70" ;
            case ">60"  : return ">60" ;
            case ">50"  : return ">50" ;  
               
            default: return "XY";
         }
      }
   }

   //================================================================================================== 

   private int iDzaglavField;

   private bool iDzaglavFieldSpecified;

   private long iDstavkaField;

   private bool iDstavkaFieldSpecified;

   private int brojUnosaField;

   private bool brojUnosaFieldSpecified;

   private short vrstaIDField;

   private bool vrstaIDFieldSpecified;

   private string nazivVrsteField;

   private short sortimentIDField;

   private bool sortimentIDFieldSpecified;

   private string nazivSortimentaField;

   private string cjRazredField;

   private string cjDuljinaField;

   private byte jMIDField;

   private bool jMIDFieldSpecified;

   private string nazivJMField;

   private string sumplocField;

   private string brojPlociceField;

   private short duljinaField;

   private bool duljinaFieldSpecified;

   private short promjerField;

   private bool promjerFieldSpecified;

   private short promjerBKField;

   private bool promjerBKFieldSpecified;

   private decimal masam3Field;

   private bool masam3FieldSpecified;

   private decimal kolicinaField;

   private bool kolicinaFieldSpecified;

   private string ktField;

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public int IDzaglav
   {
      get
      {
         return this.iDzaglavField;
      }
      set
      {
         this.iDzaglavField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool IDzaglavSpecified
   {
      get
      {
         return this.iDzaglavFieldSpecified;
      }
      set
      {
         this.iDzaglavFieldSpecified = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public long IDstavka
   {
      get
      {
         return this.iDstavkaField;
      }
      set
      {
         this.iDstavkaField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool IDstavkaSpecified
   {
      get
      {
         return this.iDstavkaFieldSpecified;
      }
      set
      {
         this.iDstavkaFieldSpecified = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public int BrojUnosa
   {
      get
      {
         return this.brojUnosaField;
      }
      set
      {
         this.brojUnosaField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool BrojUnosaSpecified
   {
      get
      {
         return this.brojUnosaFieldSpecified;
      }
      set
      {
         this.brojUnosaFieldSpecified = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public short vrstaID
   {
      get
      {
         return this.vrstaIDField;
      }
      set
      {
         this.vrstaIDField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool vrstaIDSpecified
   {
      get
      {
         return this.vrstaIDFieldSpecified;
      }
      set
      {
         this.vrstaIDFieldSpecified = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string NazivVrste
   {
      get
      {
         return this.nazivVrsteField;
      }
      set
      {
         this.nazivVrsteField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public short sortimentID
   {
      get
      {
         return this.sortimentIDField;
      }
      set
      {
         this.sortimentIDField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool sortimentIDSpecified
   {
      get
      {
         return this.sortimentIDFieldSpecified;
      }
      set
      {
         this.sortimentIDFieldSpecified = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string NazivSortimenta
   {
      get
      {
         return this.nazivSortimentaField;
      }
      set
      {
         this.nazivSortimentaField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string CjRazred
   {
      get
      {
         return this.cjRazredField;
      }
      set
      {
         this.cjRazredField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string CjDuljina
   {
      get
      {
         return this.cjDuljinaField;
      }
      set
      {
         this.cjDuljinaField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public byte JMID
   {
      get
      {
         return this.jMIDField;
      }
      set
      {
         this.jMIDField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool JMIDSpecified
   {
      get
      {
         return this.jMIDFieldSpecified;
      }
      set
      {
         this.jMIDFieldSpecified = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string NazivJM
   {
      get
      {
         return this.nazivJMField;
      }
      set
      {
         this.nazivJMField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string sumploc
   {
      get
      {
         return this.sumplocField;
      }
      set
      {
         this.sumplocField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string BrojPlocice
   {
      get
      {
         return this.brojPlociceField;
      }
      set
      {
         this.brojPlociceField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public short Duljina
   {
      get
      {
         return this.duljinaField;
      }
      set
      {
         this.duljinaField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool DuljinaSpecified
   {
      get
      {
         return this.duljinaFieldSpecified;
      }
      set
      {
         this.duljinaFieldSpecified = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public short Promjer
   {
      get
      {
         return this.promjerField;
      }
      set
      {
         this.promjerField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool PromjerSpecified
   {
      get
      {
         return this.promjerFieldSpecified;
      }
      set
      {
         this.promjerFieldSpecified = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public short PromjerBK
   {
      get
      {
         return this.promjerBKField;
      }
      set
      {
         this.promjerBKField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool PromjerBKSpecified
   {
      get
      {
         return this.promjerBKFieldSpecified;
      }
      set
      {
         this.promjerBKFieldSpecified = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public decimal masam3
   {
      get
      {
         return this.masam3Field;
      }
      set
      {
         this.masam3Field = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool masam3Specified
   {
      get
      {
         return this.masam3FieldSpecified;
      }
      set
      {
         this.masam3FieldSpecified = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public decimal kolicina
   {
      get
      {
         return this.kolicinaField;
      }
      set
      {
         this.kolicinaField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool kolicinaSpecified
   {
      get
      {
         return this.kolicinaFieldSpecified;
      }
      set
      {
         this.kolicinaFieldSpecified = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string KT
   {
      get
      {
         return this.ktField;
      }
      set
      {
         this.ktField = value;
      }
   }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class PopratniceZaglavljaRekapitulacija
{

   private string CjRazred_clean { get { return Razred.TrimEnd(' '); } }

 // 06.10.2016. HS promjenile popratnice - stavili su puno praznog prostora
   private string NazivVrste_clean { get { return NazivVrste.Trim(); } }
   private string NazivSortimenta_clean { get { return NazivSortimenta.Trim(); } }

 //public string VvArtiklName { get { return NazivVrste       + " " + NazivSortimenta       + " " + CjRazred_clean + " " + Duljina; } }
   public string VvArtiklName { get { return NazivVrste_clean + " " + NazivSortimenta_clean + " " + CjRazred_clean + " " + Duljina; } }

   private int iDZaglavField;

   private bool iDZaglavFieldSpecified;

   private string nazivVrsteField;

   private string nazivSortimentaField;

   private string razredField;

   private string duljinaField;

   private int komadaField;

   private bool komadaFieldSpecified;

   private decimal masaField;

   private bool masaFieldSpecified;

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public int IDZaglav
   {
      get
      {
         return this.iDZaglavField;
      }
      set
      {
         this.iDZaglavField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool IDZaglavSpecified
   {
      get
      {
         return this.iDZaglavFieldSpecified;
      }
      set
      {
         this.iDZaglavFieldSpecified = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string NazivVrste
   {
      get
      {
         return this.nazivVrsteField;
      }
      set
      {
         this.nazivVrsteField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string NazivSortimenta
   {
      get
      {
         return this.nazivSortimentaField;
      }
      set
      {
         this.nazivSortimentaField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string Razred
   {
      get
      {
         return this.razredField;
      }
      set
      {
         this.razredField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public string Duljina
   {
      get
      {
         return this.duljinaField;
      }
      set
      {
         this.duljinaField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public int Komada
   {
      get
      {
         return this.komadaField;
      }
      set
      {
         this.komadaField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool KomadaSpecified
   {
      get
      {
         return this.komadaFieldSpecified;
      }
      set
      {
         this.komadaFieldSpecified = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
   public decimal masa
   {
      get
      {
         return this.masaField;
      }
      set
      {
         this.masaField = value;
      }
   }

   /// <remarks/>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public bool masaSpecified
   {
      get
      {
         return this.masaFieldSpecified;
      }
      set
      {
         this.masaFieldSpecified = value;
      }
   }
}

#endregion PPUK Popratnice Class from XSD tool

#endregion PPUK HrSume


