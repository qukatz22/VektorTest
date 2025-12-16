using System;
using System.Reflection;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;

#region struct FtransStruct

public struct FtransStruct
{
   internal uint     _recID;

   /* 01 */   internal uint      _t_parentID ;
   /* 02 */   internal uint      _t_dokNum   ;
   /* 03 */   internal ushort    _t_serial   ;
   /* 04 */   internal DateTime  _t_dokDate  ;
   /* 05 */   internal string    _t_konto    ;
   /* 06 */   internal uint      _t_kupdob_cd;
   /* 07 */   internal string    _t_ticker   ;
   /* 08 */   internal uint      _t_mtros_cd ;
   /* 09 */   internal string    _t_mtros_tk ;
   /* 00 */   internal string    _t_tipBr    ;
   /* 11 */   internal string    _t_opis     ;
   /* 12 */   internal DateTime  _t_valuta   ;
   /* 13 */   internal string    _t_tt       ;
   /* 14 */   internal string    _t_pdv      ;
   /* 15 */   internal string    _t_037      ;
   /* 16 */   internal decimal   _t_dug      ;
   /* 17 */   internal decimal   _t_pot      ;
   /* 18 */   internal string    _t_projektCD;
   /* 19 */   internal ushort    _t_pdvKnjiga;
   /* 20 */   internal uint      _t_fakRecID ;
   /* 21 */   internal ushort    _t_otsKind  ;
   /* 22 */   internal string    _t_fond     ;
   /* 23 */   internal string    _t_pozicija ;
   /* 24 */   internal string    _t_progAktiv;
   /* 25 */   internal uint      _t_fakYear  ;

}

#endregion struct FtransStruct

public class Ftrans : VvTransRecord
{

   public string Program { get; set; }
   #region Fildz

   public const string recordName = "ftrans";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   /*private*/internal FtransStruct currentData;
   private FtransStruct backupData;

   #endregion Fildz

   #region Constructors

   public Ftrans() : this(0)
   {
   }

   public Ftrans(uint ID) : base()
   {
      this.currentData = new FtransStruct();

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

      /* 01 */      this.currentData._t_parentID = 0;
      /* 02 */      this.currentData._t_dokNum = 0;
      /* 03 */      this.currentData._t_serial = 0;
      /* 04 */      this.currentData._t_dokDate = DateTime.MinValue;
      /* 05 */      this.currentData._t_konto = "";
      /* 06 */      this.currentData._t_kupdob_cd = 0;
      /* 07 */      this.currentData._t_ticker = "";
      /* 08 */      this.currentData._t_mtros_cd = 0;
      /* 09 */      this.currentData._t_mtros_tk = "";
      /* 00 */      this.currentData._t_tipBr = "";
      /* 11 */      this.currentData._t_opis = "";
      /* 12 */      this.currentData._t_valuta = DateTime.MinValue;
      /* 13 */      this.currentData._t_tt = "";
      /* 14 */      this.currentData._t_pdv = "";
      /* 15 */      this.currentData._t_037 = "";
      /* 16 */      this.currentData._t_dug = 0.00M;
      /* 17 */      this.currentData._t_pot = 0.00M;
      /* 18 */      this.currentData._t_projektCD = "";
      /* 19 */      this.currentData._t_pdvKnjiga = 0;
      /* 20 */      this.currentData._t_fakRecID  = 0;
      /* 21 */      this.currentData._t_otsKind   = 0;
      /* 22 */      this.currentData._t_fond      = "";
      /* 23 */      this.currentData._t_pozicija  = "";
      /* 24 */      this.currentData._t_progAktiv = "";
      /* 25 */      this.currentData._t_fakYear   = 0;
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
      //get { return Ftrans.sorter_Person_DokDate_DokNum; }
      get
      {
         //throw new Exception("Mislim da se ovo nema zasto ikada pozivati?!. not really sure yet."); 
         return new VvSQL.RecordSorter();
      }
   }

   #endregion Sorters

   #region propertiz

   internal FtransStruct CurrentData // cijela FtransStruct struct-ura 
   {
      get { return this.currentData; }
      set { this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.FtransDao; }
   }

   public override string VirtualRecordName
   {
      get { return Ftrans.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Ftrans.recordNameArhiva; }
   }

   public override string DocumentRecordName
   {
      get { return Nalog.recordName; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "ft"; }
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()] // Da GetDgvLineFields ne misli krivo: "if(recID > 0) // Postojeci redak" 
   public override uint VirtualRecID
   {
      get { return this.T_recID; }
      set {        this.T_recID = value; }
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
      get { return /*this.T_ttNum*/0;         }
      set {        /*this.T_ttNum = value*/;  }
   }

   public override uint VirtualParentRecID
   {
      get { return this.T_parentID; }
      set {        this.T_parentID = value; }
   }

   public static string KupdobForeignKey
   {
      get { return "t_kupdob_cd"; }
   }

   public static string MtrosForeignKey
   {
      get { return "t_mtros_cd"; }
   }

   public static string KplanForeignKey
   {
      get { return "t_konto"; }
   }

   public static string NalogForeignKey
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

   //public DateTime AddTS
   //{
   //   get { return this.currentData._addTS; }
   //   set { this.currentData._addTS = value; }
   //}

   //public DateTime ModTS
   //{
   //   get { return this.currentData._modTS; }
   //   set { this.currentData._modTS = value; }
   //}

   //public string AddUID
   //{
   //   get { return this.currentData._addUID; }
   //   set { this.currentData._addUID = value; }
   //}

   //public string ModUID
   //{
   //   get { return this.currentData._modUID; }
   //   set { this.currentData._modUID = value; }
   //}

   //===================================================================

   /* */

   /* 01 */
   public uint T_parentID
   {
      get { return this.currentData._t_parentID; }
      set {        this.currentData._t_parentID = value; }
   }
   /* 02 */
   public uint T_dokNum
   {
      get { return this.currentData._t_dokNum; }
      set {        this.currentData._t_dokNum = value; }
   }
   /* 03 */
   public ushort T_serial
   {
      get { return this.currentData._t_serial; }
      set {        this.currentData._t_serial = value; }
   }
   /* 04 */   public DateTime T_dokDate
   {
      get { return this.currentData._t_dokDate; }
      set {        this.currentData._t_dokDate = value; }
   }
   /* 05 */   public string T_konto
   {
      get { return this.currentData._t_konto; }
      set {        this.currentData._t_konto = value; }
   }
   /* 06 */   public uint T_kupdob_cd
   {
      get { return this.currentData._t_kupdob_cd; }
      set {        this.currentData._t_kupdob_cd = value; }
   }
   /* 07 */   public string T_ticker
   {
      get { return this.currentData._t_ticker; }
      set {        this.currentData._t_ticker = value; }
   }

   /* 08 */   public uint T_mtros_cd
   {
      get { return this.currentData._t_mtros_cd; }
      set {        this.currentData._t_mtros_cd = value; }
   }
   /* 09 */
   public string T_mtros_tk
   {
      get { return this.currentData._t_mtros_tk; }
      set {        this.currentData._t_mtros_tk = value; }
   }
   /* 10 */
   public string T_tipBr
   {
      get { return this.currentData._t_tipBr; }
      set {        this.currentData._t_tipBr = value; }
   }
   /* 11 */   public string T_opis
   {
      get { return this.currentData._t_opis; }
      set {        this.currentData._t_opis = value; }
   }
   /* 12 */   public DateTime T_valuta
   {
      get { return this.currentData._t_valuta; }
      set {        this.currentData._t_valuta = value; }
   }
   /* 13 */
   public string T_TT
   {
      get { return this.currentData._t_tt; }
      set { this.currentData._t_tt = value; }
   }
   /* 14 */
   public string T_pdv
   {
      get { return this.currentData._t_pdv; }
      set { this.currentData._t_pdv = value; }
   }
   /* 15 */
   public string T_037
   {
      get { return this.currentData._t_037; }
      set { this.currentData._t_037 = value; }
   }
   /* 16 */
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal T_dug
   {
      get { return this.currentData._t_dug; }
      set {        this.currentData._t_dug = value; }
   }

   /* 17 */
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal T_pot
   {
      get { return this.currentData._t_pot; }
      set {        this.currentData._t_pot = value; }
   }

   /* 18 */
   public string T_projektCD
   {
      get { return this.currentData._t_projektCD; }
      set { this.currentData._t_projektCD = value; }
   }
   /* */

   /* 19 */
   public ZXC.PdvKnjigaEnum T_pdvKnjiga
   {
      get { return (ZXC.PdvKnjigaEnum)this.currentData._t_pdvKnjiga; }
      set {                           this.currentData._t_pdvKnjiga = (ushort)value; }
   }

   /* 19 */
   public ushort T_pdvKnjiga_u
   {
      get { return this.currentData._t_pdvKnjiga; }
      set {        this.currentData._t_pdvKnjiga = value; }
   }

   /* 20 */
   public uint T_fakRecID
   {
      get { return this.currentData._t_fakRecID; }
      set { this.currentData._t_fakRecID = value; }
   }

   /* 21 */
   public ushort T_otsKind_u
   {
      get { return this.currentData._t_otsKind; }
      set { this.currentData._t_otsKind = value; }
   }

   /* 21 */
   public ZXC.OtsKindEnum T_otsKind
   {
      get { return (ZXC.OtsKindEnum)this.currentData._t_otsKind; }
      set {                         this.currentData._t_otsKind = (ushort)value; }
   }

   /* 22 */ public string T_fond
   {
      get { return this.currentData._t_fond; }
      set {        this.currentData._t_fond = value; }
   }

   /* 23 */ public string T_pozicija
   {
      get { return this.currentData._t_pozicija; }
      set {        this.currentData._t_pozicija = value; }
   }

   /* 24 */ public string T_progAktiv
   {
      get { return this.currentData._t_progAktiv; }
      set {        this.currentData._t_progAktiv = value; }
   }

   /* 25 */
   public uint T_fakYear
   {
      get { return this.currentData._t_fakYear; }
      set {        this.currentData._t_fakYear = value; }
   }

   // ====== NOT DataLayer Propertiz 

   // 10.10.2012: 
   public string  R_tipBr_Resorted // Za OTS i slicno...    
   { 
      get 
      {
         string tt;
         uint ttNum;

         ParseTipBr(out tt, out ttNum);

         return tt + "-" + ttNum.ToString("000000");
      } 
   }

   public int     OtsZakas  { get; set; }
   public decimal OtsOtvor  { get; set; }
   public decimal OtsZatvor { get; set; }
   public decimal OtsSaldo  { get { return OtsOtvor - OtsZatvor; } }

   public bool    OtsIsKontra { get; set; } // URe u SET-u IRA ili IRe u SET-u URA

   // 19.01.2016: 
 //public bool    OtsIsURA    { get { return T_konto.StartsWith("22")      ; } }
   public bool    OtsIsURA    { get { return Kplan.GetIsKontoDobav(T_konto); } }
   // 19.01.2016: 
 //public bool    OtsIsIRA    { get { return !OtsIsURA; } }
   public bool    OtsIsIRA    { get { return Kplan.GetIsKontoKupac(T_konto); } }

   public decimal OtsNeDospjelo  { get { return (OtsZakas                   <=   0 ? OtsSaldo : 0.00M); } }
   public decimal OtsDospjelo    { get { return (OtsZakas >   0                    ? OtsSaldo : 0.00M); } }
   public decimal OtsDosp01_30   { get { return (OtsZakas >   0 && OtsZakas <=  30 ? OtsSaldo : 0.00M); } }
   public decimal OtsDosp31_60   { get { return (OtsZakas >  30 && OtsZakas <=  60 ? OtsSaldo : 0.00M); } }
   public decimal OtsDosp61_90   { get { return (OtsZakas >  60 && OtsZakas <=  90 ? OtsSaldo : 0.00M); } }
   public decimal OtsDosp91_120  { get { return (OtsZakas >  90 && OtsZakas <= 120 ? OtsSaldo : 0.00M); } }
   public decimal OtsDosp121_150 { get { return (OtsZakas > 120 && OtsZakas <= 150 ? OtsSaldo : 0.00M); } }
   public decimal OtsDosp151_180 { get { return (OtsZakas > 150 && OtsZakas <= 180 ? OtsSaldo : 0.00M); } }
   public decimal OtsDosp181_    { get { return (OtsZakas > 180                    ? OtsSaldo : 0.00M); } }

   public decimal R_DugMinusPot { get { return T_dug - T_pot; } }
   public decimal R_DugPlusPot  { get { return T_dug + T_pot; } }
   public decimal R_PotMinusDug { get { return T_pot - T_dug; } }

   public decimal R_DugMinusPot_ABS { get { return Math.Abs(R_DugMinusPot); } }

   public bool    OtsRacBezUplate { get; set; } // U procesu 'GetOtsZakas_OTVARANJA()' nije nadjena nijedna uplata po ovom racunu 

   public int     OtsMaksZakas  { get; set; }
   // ovo necemo, mada bi bilo priorodno, prrek enum-a jer kristali oce string
   // Z - zeleno (racun placen na vrijeme)
   // P - plavo  (racun placen sa zakasnjejem)
   // C - crveno (racun ne placen)
   public string OtsSTATUS
   {
      get
      {
         if(OtsZakas.IsZeroOrNegative()) return "Z";
         if(OtsZakas == OtsMaksZakas)    return "C";

         return "P";              
      }
   }

   public uint R_ttNum { get; set; }

   public string OrigPgTipBr { get; set; }

   public decimal R_WyrnKCRP
   {
      get
      {
         if(OtsIsURA) /* this is URA */ return R_PotMinusDug;
         else         /* this is IRA */ return R_DugMinusPot;
      }
   }

   public decimal  WyrnJoin_sUkPDV   { get; set; }
   public decimal  WyrnJoin_sUkKCR   { get { return OtsOtvor - WyrnJoin_sUkPDV; } }
   public DateTime WyrnJoin_dokDate  { get; set; }
   public string   WyrnJoin_ttNumStr { get; set; }

   public string R_forcedTipBr
   {
      get
      {
         if(NalogDao.IsSaldaKontiKTO(this.T_konto) == false) return T_tipBr;

         return T_tipBr.NotEmpty() ? T_tipBr : "XY:" + T_dokDate.Year + "-" + T_recID;
      }
   }

   //internal string OTS_ID
   //{
   //   get
   //   {
   //      return T_kupdob_cd.ToString() + "_" + T_tipBr;
   //   }
   //}

   #endregion propertiz

   #region ToString

   public override string ToString()
   {
      return "\nNal\t: "   + T_dokNum + " (" + T_dokDate.ToShortDateString() + ")" +
             "\nRed\t: "   + T_serial +
             "\nKto\t: "   + T_konto +
             "\nOpis\t: "  + T_opis +
             "\nTkr\t: "   + T_ticker +
             "\nTtNum\t: " + T_tipBr +
             "\nTtNRe\t: " + R_tipBr_Resorted +
             "\nT_prjCD\t: " + T_projektCD +
             "\nDug\t: "   + T_dug +
             "\nPot\t: "   + T_pot + 
             "\nOts\t: "   + T_otsKind + "\n";
   }

   public string ToShortString()
   {
      return "Nal " + T_dokNum + " (" + T_dokDate.ToShortDateString() + ")" + " " + 
             "Red " + T_serial                                              + " " + 
             "Kto " + T_konto                                               + " " + 
             "Dug " + T_dug                                                 + " " + 
             "Pot " + T_pot                                                 + "\n";
   }

   #endregion ToString

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<FtransStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<FtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<FtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<FtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<FtransStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Ftrans newObject = new Ftrans();

      Generic_CloneData<FtransStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      // 1.4.2011: !!! NOTA BENE for future; VvDataRecord.Clone() ti ne klonira eventualne property-e koji nisu u currentData strukturi, a zivi su podatak a ne izvedenice npr: 
      newObject.SaveTransesWriteMode = this.SaveTransesWriteMode;

      return newObject;
   }

   public Ftrans MakeDeepCopy()
   {
      return (Ftrans)Clone();
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Ftrans();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Ftrans)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Ftrans)vvDataRecord).currentData;
   }
   public override bool Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB(XSqlConnection conn)
   {
      //if(this.Tip != "MT") return false;

      foreach(PropertyInfo pInfo in this.GetType().GetProperties())
      {
         if(pInfo.PropertyType != typeof(decimal)) continue;

         foreach(Attribute attr in pInfo.GetCustomAttributes(typeof(VvIsDevizaConvertibileAttribute), false))
         {
            VvIsDevizaConvertibileAttribute isConvertibileAttr = attr as VvIsDevizaConvertibileAttribute;

            if(isConvertibileAttr != null && isConvertibileAttr.JeLiJeTakav == ZXC.JeliJeTakav.JE_TAKAV)
            {
               pInfo.SetValue(this, ZXC.EURiIzKuna_HRD_((decimal)pInfo.GetValue(this)));
            }
         }
      }

      return this.EditedHasChanges();
   }

   public override bool Convert_Euro_To_Kuna_ForAllMoneyPropertiez_JOB(XSqlConnection conn)
   {
      //if(this.Tip != "MT") return false;

      foreach(PropertyInfo pInfo in this.GetType().GetProperties())
      {
         if(pInfo.PropertyType != typeof(decimal)) continue;

         foreach(Attribute attr in pInfo.GetCustomAttributes(typeof(VvIsDevizaConvertibileAttribute), false))
         {
            VvIsDevizaConvertibileAttribute isConvertibileAttr = attr as VvIsDevizaConvertibileAttribute;

            if(isConvertibileAttr != null && isConvertibileAttr.JeLiJeTakav == ZXC.JeliJeTakav.JE_TAKAV)
            {
               pInfo.SetValue(this, ZXC.KuneIzEURa_HRD_((decimal)pInfo.GetValue(this)));
            }
         }
      }

      return this.EditedHasChanges();
   }

   #endregion VvDataRecordFactory

   #region Some Util Metodz

   public bool ParseTipBr(out string tt, out uint ttNum)
   {
      return Ftrans.ParseTipBr(this.T_tipBr, out tt, out ttNum);
   }

   public static bool ParseTipBr(string tipBr, out string tt, out uint ttNum)
   {
      tt = "";
      ttNum = 0;

      // 05.02.2019:
      if(tipBr.IsEmpty()) return false;

      // 07.12.2021: 
      tipBr = CleanTipBr(tipBr);

      string[] splitters;
      splitters = tipBr.Split("-".ToCharArray());

      if(splitters.Length != 2 ||
         splitters[0].Length != 3) return false;

    //tt = splitters[0];
      tt = splitters[0].ToUpper();

      ttNum = ZXC.ValOrZero_UInt(splitters[1]);

      if(tt.IsEmpty() || ttNum.IsZero()) return false;
      else                               return true;
   }

   // UGN-2300001/12
   private static string CleanTipBr(string tipBr)
   {
      if(!tipBr.Contains("/")) return tipBr;

      int slashIdx = tipBr.IndexOf('/');
      return tipBr.SubstringSafe(0, slashIdx);
   }

   public static string Get_WYRNtt(string origTT/*, int pgYear*/)
   {
      //string pgYlastDigit = ZXC.SubstringSafe(pgYear.ToString(), 3, 1);

      switch(origTT)
      {
       //case Faktur.TT_UFA: return "W" + pgYlastDigit + "F"; // WUR ili WFA 
       //case Faktur.TT_URA: return "W" + pgYlastDigit + "R"; // WUR ili WRA 
       //case Faktur.TT_URM: return "W" + pgYlastDigit + "M"; // WUR ili WRM 
       //case Faktur.TT_IFA: return "Y" + pgYlastDigit + "F"; // YIR ili YFA 
       //case Faktur.TT_IRA: return "Y" + pgYlastDigit + "R"; // YIR ili YRA 
       //case Faktur.TT_IRM: return "Y" + pgYlastDigit + "M"; // YIR ili YRM 

       //case Faktur.TT_UFA: return Faktur.TT_WFA;
       //case Faktur.TT_URA: return Faktur.TT_WRA;
       //case Faktur.TT_URM: return Faktur.TT_WRM;
       //case Faktur.TT_IFA: return Faktur.TT_YFA;
       //case Faktur.TT_IRA: return Faktur.TT_YRA;
       //case Faktur.TT_IRM: return Faktur.TT_YRM;

         case Faktur.TT_UFA: return Faktur.TT_WRN;
         case Faktur.TT_URA: return Faktur.TT_WRN;
         case Faktur.TT_URM: return Faktur.TT_WRN;
         case Faktur.TT_UPV: return Faktur.TT_WRN;
         case Faktur.TT_UOD: return Faktur.TT_WRN;
         // 09.01.2017:
         case Faktur.TT_WRN: return Faktur.TT_WRN;

         case Faktur.TT_IFA: return Faktur.TT_YRN;
         case Faktur.TT_IRA: return Faktur.TT_YRN;
         case Faktur.TT_IRM: return Faktur.TT_YRN;
         case Faktur.TT_IPV: return Faktur.TT_YRN;
         case Faktur.TT_IOD: return Faktur.TT_YRN;
         // 09.01.2017:
         case Faktur.TT_YRN: return Faktur.TT_YRN;

       //default: ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Za tt[{0}] i pgYear[{1}]\n\nNe mogu Get_WYRNtt()", origTT, pgYear); return "XYZ";
         default: ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Za tt[{0}]              \n\nNe mogu Get_WYRNtt()", origTT        ); return "XYZ";
      }
   }

   public static string[] WantedKupciKontaStringArray { get { return ZXC.GetStringArrayFromCommaSeparatedTokens(ZXC.KSD.Dsc_KupacKontaIOS); } }
   public static string[] WantedDobavKontaStringArray { get { return ZXC.GetStringArrayFromCommaSeparatedTokens(ZXC.KSD.Dsc_DobavKontaIOS); } }
   public static string[] WantedMAP_TTsStringArray    { get { return ZXC.GetStringArrayFromCommaSeparatedTokens(ZXC.KSD.Dsc_MAP_TTs      ); } }

   //public DateTime DokDateFromOpis 
   //{ 
   //   get 
   //   {
   //      if(this.T_opis.Length < 8) return DateTime.MinValue;
   //
   //      // rn od 01.05.2011.
   //      // rn od 1.5.2011.
   //      // rn od 01.05.2011
   //      // qweqwe 01.05.2011.
   //
   //      int frsDigitIdx = T_opis.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }); if(frsDigitIdx.IsNegative()) return DateTime.MinValue;
   //
   //      string cleanDateStr = T_opis.Substring(frsDigitIdx);
   //
   //      int idx201 = cleanDateStr.IndexOf("201"); if(idx201.IsNegative()) return DateTime.MinValue;
   //
   //      cleanDateStr = cleanDateStr.SubstringSafe(0, idx201 + 4);
   //
   //      return ZXC.ValOrDefault_DateTime(cleanDateStr, DateTime.MinValue);
   //   } 
   //}

   public DateTime DokDateFromOpis 
   { 
      get 
      {
         if(this.T_opis.Length < 8) return DateTime.MinValue;

       //int lastIdx201 = T_opis.LastIndexOf( "201"); if(lastIdx201.IsNegative()) return DateTime.MinValue;
         int lastIdx201 = T_opis.LastIndexOf(".201"); if(lastIdx201.IsNegative()) return DateTime.MinValue;

       //int start = lastIdx201 - 6;
         int start = lastIdx201 - 5;

         if(start.IsNegative()) return DateTime.MinValue;

         string cleanDateStr = T_opis.SubstringSafe(start, 10);

         return ZXC.ValOrDefault_DateTime(cleanDateStr, DateTime.MinValue);
      } 
   }

   public void SetOtsKind()
   {
      this.T_otsKind = ZXC.OtsKindEnum.NIJEDNO;

      // remarkirao 21.3.2011: jer neka PS imaju prazne kupdob_cd - ove 
    //if(T_kupdob_cd.IsZero()) return;

    //if(T_konto.StartsWith("12") && T_dug.NotZero() ||
    //   T_konto.StartsWith("22") && T_pot.NotZero())
    //{
    //   this.T_otsKind = ZXC.OtsKindEnum.OTVARANJE;
    //}
    //else if(T_konto.StartsWith("12") && T_pot.NotZero() ||
    //        T_konto.StartsWith("22") && T_dug.NotZero())
    //{
    //   this.T_otsKind = ZXC.OtsKindEnum.ZATVARANJE;
    //}

      // 14.04.2020: TEMBO nesto ...
      if(T_konto is null) T_konto = "";

      foreach(string ktoRoot in Ftrans.WantedKupciKontaStringArray) if(T_konto.StartsWith(ktoRoot) && T_dug.NotZero()) this.T_otsKind = ZXC.OtsKindEnum.OTVARANJE;
      foreach(string ktoRoot in Ftrans.WantedDobavKontaStringArray) if(T_konto.StartsWith(ktoRoot) && T_pot.NotZero()) this.T_otsKind = ZXC.OtsKindEnum.OTVARANJE;
      foreach(string ktoRoot in Ftrans.WantedKupciKontaStringArray) if(T_konto.StartsWith(ktoRoot) && T_pot.NotZero()) this.T_otsKind = ZXC.OtsKindEnum.ZATVARANJE;
      foreach(string ktoRoot in Ftrans.WantedDobavKontaStringArray) if(T_konto.StartsWith(ktoRoot) && T_dug.NotZero()) this.T_otsKind = ZXC.OtsKindEnum.ZATVARANJE;

    //ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Ftrans.SetOtsKind(): konto [" + T_konto + "] NOR otvaranje NOR zatvaranje");

#if LikeOffix
      if(T_TT == "UR" ||
         T_TT == "IR" ||
         T_TT == "IM")
      {
         this.T_otsKind = ZXC.OtsKindEnum.OTVARANJE;
      }
      else if(T_TT == "IZ" ||
              T_TT == "DI" ||
              T_TT == "BL")
      {
         this.T_otsKind = ZXC.OtsKindEnum.ZATVARANJE;
      }
      else if(T_konto.StartsWith( "1") && T_dug.NotZero() ||
              T_konto.StartsWith( "2") && T_pot.NotZero() ||
              T_konto.StartsWith("37") && T_pot.NotZero())
      {
         this.T_otsKind = ZXC.OtsKindEnum.OTVARANJE;
      }
      else if(T_konto.StartsWith( "1") && T_pot.NotZero() ||
              T_konto.StartsWith( "2") && T_dug.NotZero() ||
              T_konto.StartsWith("37") && T_dug.NotZero())
      {
         this.T_otsKind = ZXC.OtsKindEnum.ZATVARANJE;
      }
#endif
   }

   //                                            
   // XNMAaaKKKK                                 
   //                                            
   // X    1. Prihod ili  rashod                 
   // N    2. NadProgram (Glavni ili neki drugi) 
   // M    3. Program                            
   // A    4. Aktivnost ili pProjekt             
   // aa   5. Rbr Aktiv ili pProjekta            
   // KKKK 6. Konto - prve 4 znamenke            

   public bool IsDug { get { return T_dug.NotZero(); } }
   public bool IsPot { get { return T_pot.NotZero(); } }

   public decimal MaxAbsDugPot { get { return Math.Max(Math.Abs(T_dug), Math.Abs(T_pot)); } }

   public void CorrectDugOrPot(decimal value, bool isSaldoPositive)
   {
      if(Math.Abs(T_dug) == MaxAbsDugPot) // popravljamo dug 
      {
         if(isSaldoPositive) T_dug -= value;
         else                T_dug += value;
      }
      else                                // popravljamo pot 
      {
         if(isSaldoPositive) T_pot += value;
         else                T_pot -= value;
      }
   }

   #endregion Some Util Metodz

}

/// <summary>
/// Ako ces se poslije cudit ovolikim remarckovima:
/// Ovo je Grupirana, distinct informacija za kumulativno knjizenje
/// po jednom tipBr-u, pa zakas, IsOtv/IsZatvaranje ovdje nemaju smisla
/// </summary>
public struct OtsTipBrGroupInfo
{
   public int      Zakas               { get; set; }
   public bool     IsDospjelo          { get; set; }
   public bool     IsKupac             { get { return Konto[0] == '1'; } }
   public bool     IsDobav             { get { return !IsKupac; } }
   public bool     FoundOpening        { get; set; }
   public uint     OpenDokumentDokNum  { get; set; }
   public DateTime OpenDokumentDokDate { get; set; }
   public DateTime OpenDokumentValuta  { get; set; }

   public decimal  UkSaldo             { get; set; }
   public decimal  UkOpen              { get; set; }
   public decimal  UkClosed            { get; set; }
   public  string  OpenDokumentOpis    { get; set; }
   public  string  Konto               { get; set; }
   public  uint    KupdobCd            { get; set; }
   public  string  TipBr               { get; set; }
   public  uint    FakRecID            { get; set; }
   public  uint    FakYear             { get; set; }
   public  string  ProjektCD           { get; set; }
   public  uint    MtrosCD             { get; set; }
   public  string  MtrosTK             { get; set; }
   //private bool     isOtvaranje;
   //public  Ftrans   ftrans_rec;

   //public bool IsZatvaranje { get { return !isOtvaranje; } }
   //public bool IsOtvaranje
   //{
   //   get { return isOtvaranje; }
   //   set { isOtvaranje = value; }
   //}

   //public string TheTipBr { get { return ftrans_rec.T_tipBr; } }

   //public OtsInfo(Ftrans _ftrans_rec)
   //{
   //   zakas          = 0;
   //   isDospjelo    = false;
   //   foundOpening   = false;
   //   openDokumentDokDate = DateTime.MinValue;
   //   openDokumentValuta  = DateTime.MinValue;
   //   openDokumentOpis    = "";

   //   ftrans_rec  = _ftrans_rec;
   //   ukSaldo    = 0.00M;
   //   ukDug       = 0.00M;
   //   ukPot       = 0.00M;
   //   isOtvaranje = false;
   //}
}

// 22.02.2013: class KamateReportLine postoji u Vektor i Remonster project-u!!! jer su kristali zahebavali kod nekog novog reporta a kada smo KamateReportLine preselili u Vektor 
public class KamateReportLine
{
   public string   CustCode    { get; set; }
   public ZXC.OtsKindEnum TTkind { get; set; }
   public string          TT     { get; set; } // Remonster only
   public string   BrojRn      { get; set; }
   public DateTime DateRn      { get; set; }
   public DateTime ValutaRn    { get; set; }
   public decimal  IznosRn     { get; set; }
   public decimal  PlacenoRn   { get; set; }
   public decimal  TheDugRn    { get; set; } // ovo NIJE result. Za Vektor ovo je f_ukupno 
                                  
   public DateTime DateUpl     { get; set; }
   public decimal  IznosUpl    { get; set; }

   public DateTime KamDateOD   { get; set; }
   public DateTime KamDateDO   { get; set; }
   public DateTime PrnDateOD   { get { return KamDateOD + ZXC.OneDaySpan; } }
   public DateTime PrnDateDO   { get { return KamDateDO         ; } }
   public int      KamDana     { get; set; }
   public decimal  KamStopa    { get; set; }
   public decimal  KamOsnovica { get; set; }
   public decimal  KamIznos    { get; set; }

   public decimal  KumKamataRn { get; set; }
   public decimal  KumOsnovRn  { get; set; }

   public decimal  R_KumDugRn  { get { return KumKamataRn + KumOsnovRn; } }

   public decimal  KumKamataTr { get; set; }
   public decimal  KumOsnovTr  { get; set; }

   public decimal  R_KumDugTr  { get { return KumKamataTr + KumOsnovTr; } }

   public decimal  R_KumDugOSN { get { return KumOsnovTr + KumOsnovRn ; } }

   public decimal  R_KumDugALL { get { return R_KumDugTr + R_KumDugRn ; } }

   public DateTime DateStanja  { get { return (TTkind == ZXC.OtsKindEnum.OTVARANJE || TT == "O" ? KamDateDO : DateUpl ); } }
   public DateTime TransDate   { get { return (TTkind == ZXC.OtsKindEnum.OTVARANJE || TT == "O" ? DateRn    : DateUpl ); } }
   public decimal  TransIznos  { get { return (TTkind == ZXC.OtsKindEnum.OTVARANJE || TT == "O" ? TheDugRn  : IznosUpl); } }//tamara 05.02.2013.

   public decimal IznosRn_Real { get { return (BrojRn == "OSTATAK" ? 0.00M : IznosRn); } }//tamara 07.05.2014. iznos pravih racuna a ne OSTATKA sto mi treba za sumu racuna u crystalu

   public override string ToString()
   {
      return 
         TTkind.ToString()                      + "/" + 
         BrojRn                                 + "/" + 
         KamDateOD  .ToString(ZXC.VvDateFormat) + "/" + 
         KamDateDO  .ToString(ZXC.VvDateFormat) + "/" + 
         KamDana    .ToString()                 + "/" + 
         KamOsnovica.ToStringVv();
   }

}

