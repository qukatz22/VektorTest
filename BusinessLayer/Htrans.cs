using System;

#region struct HtransStruct

public struct HtransStruct
{
   internal uint     _recID;

   /* 01 */   internal uint      _t_parentID;
   /* 02 */   internal uint      _t_dokNum;
   /* 03 */   internal ushort    _t_serial;
   /* 04 */   internal DateTime  _t_dokDate;
   /* 05 */   internal string    _t_tt;
   /* 06 */   internal uint      _t_ttNum;

   /* 07 */   internal string   _t_valName ;
   /* 08 */   internal decimal  _t_kupovni ;
   /* 09 */   internal decimal  _t_srednji ;
   /* 10 */   internal decimal  _t_prodajni;

}

#endregion struct HtransStruct

public class Htrans : VvTransRecord
{

   #region Fildz

   public const string recordName = "htrans";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private HtransStruct currentData;
   private HtransStruct backupData;

   #endregion Fildz

   #region Constructors

   public Htrans() : this(0)
   {
   }

   public Htrans(uint ID) : base()
   {
      this.currentData = new HtransStruct();

      Memset0(ID);
   }

   public override void Memset0(uint ID)
   {
      this.currentData._recID = ID;

      /* 01 */      this.currentData._t_parentID= 0;
      /* 02 */      this.currentData._t_dokNum  = 0;
      /* 03 */      this.currentData._t_serial  = 0;
      /* 04 */      this.currentData._t_dokDate = DateTime.MinValue;
      /* 05 */      this.currentData._t_tt      = "";
      /* 06 */      this.currentData._t_ttNum   = 0;
      /* 07 */      this.currentData._t_valName = "";
      /* 08 */      this.currentData._t_kupovni = 0.00M;
      /* 09 */      this.currentData._t_srednji = 0.00M;
      /* 10 */      this.currentData._t_prodajni= 0.00M;
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
      //get { return Htrans.sorter_Person_DokDate_DokNum; }
      get 
      { 
         throw new Exception("Mislim da se ovo nema zasto ikada pozivati?!. not really sure yet."); 
         /*return new VvSQL.RecordSorter();*/ 
      }
   }

   #endregion Sorters

   #region propertiz

   internal HtransStruct CurrentData // cijela HtransStruct struct-ura 
   {
      get { return this.currentData; }
      set { this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.HtransDao; }
   }

   public override string VirtualRecordName
   {
      get { return Htrans.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Htrans.recordNameArhiva; }
   }

   public override string DocumentRecordName
   {
      get { return DevTec.recordName; }
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

   public override bool IsPrjkt_NonPUG_DataRecord { get { return (true); } }

   public static string DevTecForeignKey
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
      set { this.currentData._t_tt = value; }
   }
   /* 06 */
   public uint T_ttNum
   {
      get { return this.currentData._t_ttNum; }
      set { this.currentData._t_ttNum = value; }
   }
   /* 07 */
   public string T_ValName
   {
      get { return this.currentData._t_valName; }
      set {        this.currentData._t_valName = value; }
   }
   /* 08 */
   public decimal T_Kupovni
   {
      get { return this.currentData._t_kupovni; }
      set {        this.currentData._t_kupovni = value; }
   }
   /* 09 */ public decimal T_Srednji
   {
      get { return this.currentData._t_srednji; }
      set {        this.currentData._t_srednji = value; }
   }

   /* 10 */ public decimal T_Prodajni
   {
      get { return this.currentData._t_prodajni; }
      set {        this.currentData._t_prodajni = value; }
   }

   /* */

   public decimal T_TheTecaj { get { return this.T_Srednji; } }

   //||||||||||||||||||||||||||||||||||||||||||||||||| 

   #endregion propertiz

   #region ToString

   public override string ToString()
   {
      return " DevTec: " + T_TT + " (" + T_dokDate.ToShortDateString() + ")" + 
         "\n     Ser: " + T_serial +
         "\n  Valuta: " + T_ValName + 
         "\n SrTecaj: " + T_Srednji + "\n";
   }

   #endregion ToString

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<HtransStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<HtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<HtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<HtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<HtransStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Htrans newObject = new Htrans();

      Generic_CloneData<HtransStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      // 1.4.2011: !!! NOTA BENE for future; VvDataRecord.Clone() ti ne klonira eventualne property-e koji nisu u currentData strukturi, a zivi su podatak a ne izvedenice npr: 
      newObject.SaveTransesWriteMode = this.SaveTransesWriteMode;

      return newObject;
   }

   public Htrans MakeDeepCopy()
   {
      return (Htrans)Clone();
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Htrans();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Htrans)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Htrans)vvDataRecord).currentData;
   }


   #endregion VvDataRecordFactory

}

