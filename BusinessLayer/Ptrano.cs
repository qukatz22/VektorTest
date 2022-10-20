using System;

#region struct PtranoStruct

public struct PtranoStruct
{
   internal uint     _recID;

   /* 01 */  internal uint      _t_parentID  ;
   /* 02 */  internal uint      _t_dokNum    ;
   /* 03 */  internal ushort    _t_serial    ;
   /* 04 */  internal DateTime  _t_dokDate   ;
   /* 05 */  internal string    _t_tt        ;
   /* 06 */  internal uint      _t_ttNum     ;
   /* 07 */  internal uint      _t_personCD  ;
   /* 08 */  internal string    _t_ime       ;
   /* 09 */  internal string    _t_prezime   ;

   /* 10 */  internal DateTime  _t_dateStart;   
   /* 11 */  internal uint      _t_ukBrRata ;   
   /* 12 */  internal string    _t_opisOb   ;   
   /* 13 */  internal uint      _t_kupdob_cd;   
   /* 14 */  internal string    _t_kupdob_tk;
   /* 15 */  internal decimal   _t_iznosOb  ;   
   /* 16 */  internal bool      _t_isZbir   ;   
   /* 17 */  internal string    _t_partija  ;   

   /* 18 */  internal decimal   _t_izNetoaSt;   

   /* 19 */  internal uint      _t_rbrRate  ;   
 ///* 20 */  internal bool      _t_isZastRn ;   
   /* 20 */  internal ushort    _t_isZastRn ;   

}

#endregion struct PtranoStruct

public class Ptrano : VvTransRecord
{

   #region Fildz

   public const string recordName = "ptrano";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private PtranoStruct currentData;
   private PtranoStruct backupData;

   // spec za ptrano: 
   protected static System.Data.DataTable TheSchemaTable = ZXC.PtranoDao.TheSchemaTable;
   protected static PtranoDao.PtranoCI    CI             = ZXC.PtranoDao.CI;

   public enum SpecEnum
   {
      NOVOZAPOSL, UMIROV, NIJE
   }
   public enum InvalidEnum
   {
      HRVI, INVALID, NIJE
   }
   public enum RSmSpecifikumEnum
   {
      POROD51, POROD52, POROD60, NIJE
   }

   #endregion Fildz

   #region Constructors

   public Ptrano() : this(0)
   {
   }

   public Ptrano(uint ID) : base()
   {
      this.currentData = new PtranoStruct();

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

      /* 01 */   this.currentData._t_parentID  = 0;
      /* 02 */   this.currentData._t_dokNum    = 0;
      /* 03 */   this.currentData._t_serial    = 0;
      /* 04 */   this.currentData._t_dokDate   = DateTime.MinValue;
      /* 05 */   this.currentData._t_tt        = "";
      /* 06 */   this.currentData._t_ttNum     = 0;
      /* 07 */   this.currentData._t_personCD  = 0;
      /* 08 */   this.currentData._t_ime       = "";
      /* 09 */   this.currentData._t_prezime   = "";

      /* 10 */  this.currentData._t_dateStart = DateTime.MinValue;   
      /* 11 */  this.currentData._t_ukBrRata  = 0;   
      /* 12 */  this.currentData._t_opisOb    = "";   
      /* 13 */  this.currentData._t_kupdob_cd = 0;   
      /* 14 */  this.currentData._t_kupdob_tk = "";   
      /* 15 */  this.currentData._t_iznosOb   = decimal.Zero;
      /* 16 */  this.currentData._t_isZbir    = false;
      /* 17 */  this.currentData._t_partija   = "";   

      /* 18 */  this.currentData._t_izNetoaSt = decimal.Zero;
      /* 19 */  this.currentData._t_rbrRate   = 0;   
    ///* 20 */  this.currentData._t_isZastRn  = false;   
      /* 20 */  this.currentData._t_isZastRn  = 0;   

   }

   #endregion Constructors

   #region Sorters

   // sluzi samo za GTEREC kod PersonDao.GetPersonDokumDataList(); 
   public static VvSQL.RecordSorter sorterDokDate = new VvSQL.RecordSorter(Ptrano.recordName, Ptrano.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_dokDate]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_dokNum]),
         //new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_recVer], true)
      }, "Datum", VvSQL.SorterType.DokDate, false);

   private VvSQL.RecordSorter[] _sorters = null;

   public override VvSQL.RecordSorter[] Sorters
   {
      get { return _sorters; }
   }


   public override object[] SorterCurrVal(VvSQL.SorterType sortType)
   {
      switch(sortType)
      {
         case VvSQL.SorterType.DokDate: return new object[] { this.T_personCD, this.T_dokDate, this.T_dokNum, RecVer };

         default: ZXC.aim_emsg(recordName + " Nema definiran sorter " + sortType.ToString());
            return null;
      }
   }

   public override VvSQL.RecordSorter DefaultSorter
   {
      //get { return Ptrano.sorter_Person_DokDate_DokNum; }
      get 
      { 
         throw new Exception("Mislim da se ovo nema zasto ikada pozivati?!. not really sure yet."); 
         /*return new VvSQL.RecordSorter();*/ 
      }
   }

   // sluzi samo za GTEREC kod PersonDao.GetPrevPtranoForPerson(); 
   public static VvSQL.RecordSorter sorter_Person_DokDate_DokNum = new VvSQL.RecordSorter(Ptrano.recordName, Ptrano.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_personCD]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_dokDate]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_dokNum]),
         //new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_recVer], true)
      }, "PrsnDokDateNum", VvSQL.SorterType.DokDate, false);


   #endregion Sorters

   #region propertiz

   internal PtranoStruct CurrentData // cijela PtranoStruct struct-ura 
   {
      get { return this.currentData; }
      set { this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.PtranoDao; }
   }

   public override string VirtualRecordName
   {
      get { return Ptrano.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Ptrano.recordNameArhiva; }
   }

   public override string DocumentRecordName
   {
      get { return Placa.recordName; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "po"; }
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

   public static string PersonForeignKey
   {
      get { return "t_personCD"; }
   }

   public static string PlacaForeignKey
   {
      get { return "t_parentID"; }
   }

   public static string KupdobForeignKey
   {
      get { return "t_kupdob_cd"; }
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
      get { return this.currentData._t_dokDate; }
      set {        this.currentData._t_dokDate = value; }
   }
   /* 05 */ public string T_TT
   {
      get { return this.currentData._t_tt; }
      set {        this.currentData._t_tt = value; }
   }
   /* 06 */
   public uint T_ttNum
   {
      get { return this.currentData._t_ttNum; }
      set {        this.currentData._t_ttNum = value; }
   }
   /* 07 */ public uint T_personCD
   {
      get { return this.currentData._t_personCD; }
      set {        this.currentData._t_personCD = value; }
   }
   /* 08 */ public string T_ime
   {
      get { return this.currentData._t_ime; }
      set {        this.currentData._t_ime = value; }
   }
   /* 09 */ public string T_prezime
   {
      get { return this.currentData._t_prezime; }
      set {        this.currentData._t_prezime = value; }
   }

   public string T_PrezimeIme
   {
      get { return Person.GetPrezimeIme(this.currentData._t_prezime, this.currentData._t_ime); }
   }

   /* 10 */public DateTime T_dateStart
   {
      get { return this.currentData._t_dateStart; }
      set {        this.currentData._t_dateStart = value; }
   }
   /* 11 */public uint T_ukBrRata
   {
      get { return this.currentData._t_ukBrRata; }
      set {        this.currentData._t_ukBrRata = value; }
   }
   /* 12 */public string T_opisOb
   {
      get { return this.currentData._t_opisOb; }
      set {        this.currentData._t_opisOb = value; }
   }
   /* 13 */public uint T_kupdob_cd
   {
      get { return this.currentData._t_kupdob_cd; }
      set {        this.currentData._t_kupdob_cd = value; }
   }
   /* 14 */public string T_kupdob_tk
   {
      get { return this.currentData._t_kupdob_tk; }
      set {        this.currentData._t_kupdob_tk = value; }
   }
   /* 15 */public decimal T_iznosOb
   {
      get { return this.currentData._t_iznosOb; }
      set {        this.currentData._t_iznosOb = value; }
   }

   /* 16 */public bool T_isZbir
   {
      get { return this.currentData._t_isZbir; }
      set {        this.currentData._t_isZbir = value; }
   }

   /* 17 */public string T_partija
   {
      get { return this.currentData._t_partija; }
      set {        this.currentData._t_partija = value; }
   }

   /* 18 */public decimal T_izNetoaSt
   {
      get { return this.currentData._t_izNetoaSt; }
      set {        this.currentData._t_izNetoaSt = value; }
   }

   /* 19 */public uint T_rbrRate
   {
      get { return this.currentData._t_rbrRate; }
      set {        this.currentData._t_rbrRate = value; }
   }

   ///* 20 */public bool T_isZastRn
   //{
   //   get { return this.currentData._t_isZastRn; }
   //   set {        this.currentData._t_isZastRn = value; }
   //}
   ///* */

   /* 20 */ public Ptrans.PtranoKind T_ptranoKind
   {
      get { return (Ptrans.PtranoKind)this.currentData._t_isZastRn; }
      set {                           this.currentData._t_isZastRn = (ushort)value; }
   }

   #endregion propertiz

   #region ToString

   public override string ToString()
   {
      return " Obust: " + T_dokNum + " (" + T_dokDate.ToShortDateString() + ")" + 
           "\n   Ser: " + T_serial + 
           "\n PerCd: " + T_personCD + 
           "\nPerson: " + T_PrezimeIme + "\n";
   }

   #endregion ToString

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<PtranoStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<PtranoStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<PtranoStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<PtranoStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<PtranoStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Ptrano newObject = new Ptrano();

      Generic_CloneData<PtranoStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      // 1.4.2011: !!! NOTA BENE for future; VvDataRecord.Clone() ti ne klonira eventualne property-e koji nisu u currentData strukturi, a zivi su podatak a ne izvedenice npr: 
      newObject.SaveTransesWriteMode = this.SaveTransesWriteMode;

      return newObject;
   }

   public Ptrano MakeDeepCopy()
   {
      return (Ptrano)Clone();
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Ptrano();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Ptrano)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Ptrano)vvDataRecord).currentData;
   }

   #endregion VvDataRecordFactory

}

