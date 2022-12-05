using System;

#region struct AtransStruct

public struct AtransStruct
{
   internal uint     _recID;

   /* 01 */   internal uint      _t_parentID;
   /* 02 */   internal uint      _t_dokNum;
   /* 03 */   internal ushort    _t_serial;
   /* 04 */   internal DateTime  _t_skladDate;
   /* 05 */   internal string    _t_tt;
   /* 06 */   internal string    _t_osredCD;
   /* 07 */   internal string    _t_opis;
   /* 08 */   internal decimal   _t_kol;
   /* 19 */   internal string    _t_koef_am;
   /* 10 */   internal decimal   _t_amort_st;
   /* 11 */   internal decimal   _t_normalAm;
   /* 12 */   internal decimal   _t_dug;
   /* 13 */   internal decimal   _t_pot;
}

#endregion struct AtransStruct

public class Atrans : VvTransRecord
{

   #region Fildz

   public const string recordName = "atrans";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private AtransStruct currentData;
   private AtransStruct backupData;

   #endregion Fildz

   #region Constructors

   public Atrans() : this(0)
   {
   }

   public Atrans(uint ID) : base()
   {
      this.currentData = new AtransStruct();

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

      /* 01 */      this.currentData._t_parentID= 0;
      /* 02 */      this.currentData._t_dokNum  = 0;
      /* 03 */      this.currentData._t_serial  = 0;
      /* 04 */      this.currentData._t_skladDate = DateTime.MinValue;
      /* 05 */      this.currentData._t_tt      = "";
      /* 06 */      this.currentData._t_osredCD = "";
      /* 07 */      this.currentData._t_opis    = "";
      /* 08 */      this.currentData._t_kol     = 0.00M;
      /* 09 */      this.currentData._t_koef_am = "";
      /* 10 */      this.currentData._t_amort_st= 0.00M;
      /* 11 */      this.currentData._t_normalAm= 0.00M;
      /* 12 */      this.currentData._t_dug     = 0.00M;
      /* 13 */      this.currentData._t_pot     = 0.00M;
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
      //get { return Atrans.sorter_Person_DokDate_DokNum; }
      get 
      { 
         throw new Exception("Mislim da se ovo nema zasto ikada pozivati?!. not really sure yet."); 
         /*return new VvSQL.RecordSorter();*/ 
      }
   }

   #endregion Sorters

   #region propertiz

   internal AtransStruct CurrentData // cijela AtransStruct struct-ura 
   {
      get { return this.currentData; }
      set { this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.AtransDao; }
   }

   public override string VirtualRecordName
   {
      get { return Atrans.recordName; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "ot"; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Atrans.recordNameArhiva; }
   }

   public override string DocumentRecordName
   {
      get { return Amort.recordName; }
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
      get { return /*this.T_ttNum*/0;         }
      set {        /*this.T_ttNum = value*/;  }
   }

   public static string OsredForeignKey
   {
      get { return "t_osredCD"; }
   }

   public static string AmortForeignKey
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
   /* 03 */ public ushort T_serial
   {
      get { return this.currentData._t_serial; }
      set {        this.currentData._t_serial = value; }
   }
   /* 04 */ public DateTime T_dokDate
   {
      get { return this.currentData._t_skladDate; }
      set {        this.currentData._t_skladDate = value; }
   }
   /* 05 */ public string T_TT
   {
      get { return this.currentData._t_tt; }
      set { this.currentData._t_tt = value; }
   }
   /* 06 */ public string T_osredCD
   {
      get { return this.currentData._t_osredCD; }
      set {        this.currentData._t_osredCD = value; }
   }
   /* 07 */ public string T_opis
   {
      get { return this.currentData._t_opis; }
      set {        this.currentData._t_opis = value; }
   }
   /* 08 */ public decimal T_kol
   {
      get { return this.currentData._t_kol; }
      set {        this.currentData._t_kol = value; }
   }
   /* 09 */ public string T_koefAm
   {
      get { return this.currentData._t_koef_am; }
      set {        this.currentData._t_koef_am = value; }
   }
   /* 10 */ public decimal T_amortSt
   {
      get { return this.currentData._t_amort_st; }
      set {        this.currentData._t_amort_st = value; }
   }

   /* 11 */ public decimal T_normalAm
   {
      get { return this.currentData._t_normalAm; }
      set {        this.currentData._t_normalAm = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 12 */ public decimal T_dug
   {
      get { return this.currentData._t_dug; }
      set {        this.currentData._t_dug = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 13 */ public decimal T_pot
   {
      get { return this.currentData._t_pot; }
      set {        this.currentData._t_pot = value; }
   }

   /* */

   public decimal Amort1 { get; set; }
   public decimal Amort2 { get; set; }
   public decimal Amort3 { get; set; }

   #endregion propertiz

   #region ToString

   public override string ToString()
   {
      return " Amort: " + T_dokNum + " (" + T_dokDate.ToShortDateString() + ")" + 
         "\n   Ser: " + T_serial + 
         "\n OsrCd: " + T_osredCD + 
         "\n  Opis: " + T_opis + 
         "\n   Dug: " + T_dug + 
         "\n   Pot: " + T_pot + "\n";
   }

   #endregion ToString

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<AtransStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<AtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<AtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<AtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<AtransStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Atrans newObject = new Atrans();

      Generic_CloneData<AtransStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      // 1.4.2011: !!! NOTA BENE for future; VvDataRecord.Clone() ti ne klonira eventualne property-e koji nisu u currentData strukturi, a zivi su podatak a ne izvedenice npr: 
      newObject.SaveTransesWriteMode = this.SaveTransesWriteMode;

      return newObject;
   }

   public Atrans MakeDeepCopy()
   {
      return (Atrans)Clone();
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Atrans();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Atrans)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Atrans)vvDataRecord).currentData;
   }

   #endregion VvDataRecordFactory

}

