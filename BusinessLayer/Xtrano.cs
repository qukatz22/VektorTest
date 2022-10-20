using System;

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
   /* */

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

}

