using System;
using System.Collections.Generic;
using System.Linq;


#region struct DevTecStruct

public struct DevTecStruct
{
   internal uint     _recID;
   internal DateTime _addTS;
   internal DateTime _modTS;
   internal string   _addUID;
   internal string   _modUID;

   internal uint     _lanSrvID;
   internal uint     _lanRecID;

   /* 05 */   internal uint     _dokNum;
   /* 06 */   internal DateTime _dokDate; // 'primjenjuje se od' 
   /* 07 */   internal string   _tt;      // HNB, ZABA, PBZ, ... cija je tecajna lista 
   /* 08 */   internal uint     _ttNum;
   /* 09 */   internal string   _napomena;
   /* 10 */   internal bool     _flagA;

   /* 11 */   internal DateTime _dateCreated; // 'datum formiranja' 
   /* 12 */   internal uint     _extDokNum;

}

#endregion struct DevTecStruct

public class DevTec2 : VvDocumentRecord
{
   #region Fildz

   // 12.05.2025. vidi opasku u ZXC.cs                                                                          
   //public const string recordName = "devTec";
   public const string recordName = "devTec2";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private DevTecStruct currentData;
   private DevTecStruct backupData;

   protected static System.Data.DataTable TheSchemaTable = ZXC.DevTecDao.TheSchemaTable;
   protected static DevTecDao.DevTecCI    CI             = ZXC.DevTecDao.CI;

   #endregion Fildz

   #region Constructors

   public DevTec2() : this(0)
   {
   }

   public DevTec2(uint ID) : base()
   {
      this.currentData = new DevTecStruct();

      Memset0(ID);
   }

   public override void Memset0(uint ID)
   {
      this.currentData._recID = ID;
      this.currentData._addTS = DateTime.MinValue;
      this.currentData._modTS = DateTime.MinValue;
      this.currentData._addUID = "";
      this.currentData._modUID = "";
      this.currentData._lanSrvID = 0;
      this.currentData._lanRecID = 0;

      // well, svi reference types (string, date, ...)

      /* 05 */      this.currentData._dokNum       = 0;
      /* 06 */      this.currentData._dokDate      = DateTime.MinValue;
      /* 07 */      this.currentData._tt           = "";
      /* 08 */      this.currentData._ttNum        = 0;
      /* 09 */      this.currentData._napomena     = "";
      /* 10 */      this.currentData._flagA        = false;
      /* 11 */      this.currentData._dateCreated  = DateTime.MinValue;
      /* 12 */      this.currentData._extDokNum    = 0;

      this.Transes = new List<Htrans2>();

   }

   #endregion Constructors

   #region Sorters

   public static VvSQL.RecordSorter sorterDokNum = new VvSQL.RecordSorter(DevTec2.recordName, DevTec2.recordNameArhiva, new VvSQL.IndexSegment[]  
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.dokNum]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Br Dok", VvSQL.SorterType.DokNum, false);

   public static VvSQL.RecordSorter sorterDokDate = new VvSQL.RecordSorter(DevTec2.recordName, DevTec2.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.dokDate]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.dokNum]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "DatDok", VvSQL.SorterType.DokDate, false);

   public static VvSQL.RecordSorter sorterTtNum = new VvSQL.RecordSorter(DevTec2.recordName, DevTec2.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.tt]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.ttNum]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Banka", VvSQL.SorterType.TtNum, false);

   private VvSQL.RecordSorter[] _sorters =
      new VvSQL.RecordSorter[]
      {   
         //sorterRecID, 
         sorterDokNum,
         sorterDokDate,
         sorterTtNum
      };

   public override VvSQL.RecordSorter[] Sorters
   {
      get { return _sorters; }
   }


   public override object[] SorterCurrVal(VvSQL.SorterType sortType)
   {
      switch(sortType)
      {
         case VvSQL.SorterType.DokNum : return new object[] { this.DokNum,                               RecVer };
         case VvSQL.SorterType.DokDate: return new object[] { this.DokDate,    this.DokNum,              RecVer };
         case VvSQL.SorterType.TtNum  : return new object[] { this.TT,         this.TtNum,               RecVer };

         default: ZXC.aim_emsg(recordName + " Nema definiran sorter " + sortType.ToString());
            return null;
      }
   }

   public override VvSQL.RecordSorter DefaultSorter
   {
      get { return DevTec2.sorterDokDate; }
   }

   #endregion Sorters

   #region propertiz

   internal DevTecStruct CurrentData // cijela DevTecStruct struct-ura 
   {
      get { return this.currentData; }
      set {        this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.DevTecDao; }
   }

   public override string VirtualRecordName
   {
      get { return DevTec2.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return DevTec2.recordNameArhiva; }
   }

   public override string TransRecordName
   {
      get { return Htrans2.recordName; }
   }

   public override string TransRecordNameArhiva
   {
      get { return Htrans2.recordNameArhiva; }
   }

   public override uint VirtualRecID
   {
      get { return this.RecID; }
      set {        this.RecID = value; }
   }

   public override DateTime VirtualAddTS { get { return this.AddTS;  } }
   public override DateTime VirtualModTS { get { return this.ModTS;  } }
   public override string   VirtualAddUID{ get { return this.AddUID; } }
   public override string   VirtualModUID{ get { return this.ModUID; } }

   public override uint     VirtualLanSrvID { get { return this.LanSrvID; } set { this.LanSrvID = value; } }
   public override uint     VirtualLanRecID { get { return this.LanRecID; } set { this.LanRecID = value; } }

   public override uint VirtualDokNum
   {
      get { return this.DokNum; }
      set { this.DokNum = value; }
   }

   public override DateTime VirtualDokDate
   {
      get { return this.DokDate; }
   }

   public override uint VirtualTTnum
   {
      get { return this.TtNum; }
      set { this.TtNum = value; }
   }

   public override uint VirtualTTnum_Bkp
   {
      get { return this.backupData._ttNum; }
   }

   public override string VirtualTT
   {
      get { return this.TT; }
   }

   public override bool IsPrjkt_NonPUG_DataRecord { get { return (true); } }


   /// <summary>
   /// Gets or sets a list of htrans (line items) for the devTec.
   /// </summary>
   public List<Htrans2> Transes { get; set; }

   /// <summary>
   /// PAZI!!! Ovdje a'o po jajima. Metode nemozes pozivati nego Invoke()... vidi dolje.
   /// get {};  vraca zapravo 'List<Htrans> Transes' konvertiran u List<VvTransRecord>
   /// preko buffer varijable virtualTranses
   /// set {}; ne utjece Na Transes nego samo na buffer varijablu virtualTranses
   /// Zaguljeno indeed. (Googlaj Generics covariance) 
   /// 
   /// Comment added in Jan 2008:
   /// jebote, ovaj dole virtualTranses se ne salje kao reference type pa moras ili 'ref' ili da ga vrati
   /// bez toga ti se uprkos:       if(outputList == null) outputList = new VvList<TOutput>();
   /// u metodi Convert_ListToVvList, ovaj vrati kao null ako je po dolasku bio null.
   /// 
   /// </summary>
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public override List<VvTransRecord> VirtualTranses
   {
      get
      {
         if(this.Transes != null) return Transes.ConvertAll(ftr => ftr as VvTransRecord);
         else                     return null;
      }
   }

   public override void InvokeTransClear()
   {
      if(this.Transes != null) this.Transes.Clear();
   }

   public override void InvokeTransRemove(VvTransRecord trans_rec)
   {
      this.Transes.Remove((Htrans2)trans_rec);
   }

   public override bool IsSomeOfPossibleForeignKeyFieldsChanged
   {
      get
      {
         // notin' to do here. Ovo treba samo za VvDataRecord-e koji su sifrari a foreign key im nije RecID nego 
         // neki UC-u dostupan column. Ovo koristis za npr. 'Kplan', 'User', 
         return false;
      }
   }

   //===================================================================
   //===================================================================
   //===================================================================


   public uint RecID
   {
      get { return this.currentData._recID; }
      set { this.currentData._recID = value; }
   }

   public DateTime AddTS
   {
      get { return this.currentData._addTS; }
      set { this.currentData._addTS = value; }
   }

   public DateTime ModTS
   {
      get { return this.currentData._modTS; }
      set { this.currentData._modTS = value; }
   }

   public string AddUID
   {
      get { return this.currentData._addUID; }
      set { this.currentData._addUID = value; }
   }

   public string ModUID
   {
      get { return this.currentData._modUID; }
      set { this.currentData._modUID = value; }
   }

   public uint LanSrvID { get { return this.currentData._lanSrvID; } set { this.currentData._lanSrvID = value; } }
   public uint LanRecID { get { return this.currentData._lanRecID; } set { this.currentData._lanRecID = value; } }
   //===================================================================

   /**/

   /* 05 */ internal uint DokNum
   {
      get { return this.currentData._dokNum; }
      set {        this.currentData._dokNum = value; }
   }

   /* 06 */ 
   internal DateTime DokDate
   {
      get { return this.currentData._dokDate; }
      set {        this.currentData._dokDate = value; }
   }
   /* 07 */ internal string TT
   {
      get { return this.currentData._tt; }
      set {        this.currentData._tt = value; }
   }
   /* 08 */ internal uint TtNum
   {
      get { return this.currentData._ttNum; }
      set {        this.currentData._ttNum = value; }
   }
   /* 09 */ internal string Napomena
   {
      get { return this.currentData._napomena; }
      set {        this.currentData._napomena = value; }
   }
   /* 10 */ internal bool FlagA
   {
      get { return this.currentData._flagA; }
      set {        this.currentData._flagA = value; }
   }

   /* 11 */ internal DateTime DateCreated
   {
      get { return this.currentData._dateCreated; }
      set {        this.currentData._dateCreated = value; }
   }

   /* 12 */ internal uint ExtDokNum
   {
      get { return this.currentData._extDokNum; }
      set {        this.currentData._extDokNum = value; }
   }

   /**/

   private Htrans2[] TransesNonDeleted
   {
      get
      {
         return this.Transes.Where(dtrn => dtrn.SaveTransesWriteMode != ZXC.WriteMode.Delete).ToArray();
      }
   }

   #endregion propertiz

   #region ToString

   public override string ToString()
   {
      return "Banka: " + TT + " Za dan: " + DokDate;
   }

   #endregion ToString

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<DevTecStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<DevTecStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<DevTecStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<DevTecStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<DevTecStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      DevTec2 newObject = new DevTec2();

      Generic_CloneData<DevTecStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      return newObject;
   }

   public DevTec2 MakeDeepCopy()
   {
      return (DevTec2)Clone();
   }

   public override void TakeTheseTranses(List<VvTransRecord> transList)
   {
      if(transList.IsEmpty()) return;

      this.Transes = transList.ConvertAll(trans => trans as Htrans2);
   }

   public override void TakeTransesFrom(VvDocumentRecord _vvDocumentRecord)
   {
      if(_vvDocumentRecord.VirtualTranses == null) return;

      this.Transes = _vvDocumentRecord.CloneTranses().ConvertAll(trans => trans as Htrans2);
   }

   public override List<VvTransRecord> CloneTranses()
   {
      if(this.Transes == null) return null;

      List<Htrans2> newList = new List<Htrans2>(this.Transes.Count);

      foreach(Htrans2 htrans_rec in this.Transes)
      {
         newList.Add((Htrans2)htrans_rec.Clone());
      }

      return (newList.ConvertAll(trans => trans as VvTransRecord));
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new DevTec2();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((DevTec2)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((DevTec2)vvDataRecord).currentData;
   }

   public override VvTransRecord VvTransRecordFactory()
   {
      return new Htrans2();
   }

   public override string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      // ne zelimo DevTec cuvati 
    //return SaveSerialized_VvDataRecord_ToXmlFile_JOB<DevTec>(fileName, _isAutoCreat);
      return "";
   }

   public override VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
      return DeserializeFromXmlFile<DevTec2>(fileName);
   }


   #endregion VvDataRecordFactory

}
