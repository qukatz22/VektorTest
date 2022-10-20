using System;

#region struct PrvlgStruct

public struct PrvlgStruct
{
   internal uint     _recID;
   internal DateTime _addTS;
   internal DateTime _modTS;
   internal string   _addUID;
   internal string   _modUID;

   internal uint     _lanSrvID;
   internal uint     _lanRecID;

   internal uint   _prjktID;
   internal string _prjktTick;
   internal string _userName;
   internal string _prvlgScope;
   internal string _prvlgType;
   internal string _documType;
}

#endregion struct PrvlgStruct

// !!! Za sada jedini bussiness object koji nije niti VvSifrarRecord, niti VvDocumentRecord, niti VvTransRecord 
// nego diraktno VvDataRecord !!! 
public class Prvlg : VvDataRecord
{

   #region Fildz

   public const string recordName = "prvlg";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private PrvlgStruct currentData;
   private PrvlgStruct backupData;

   protected static System.Data.DataTable TheSchemaTable = ZXC.PrvlgDao.TheSchemaTable;
   protected static PrvlgDao.PrvlgCI CI                  = ZXC.PrvlgDao.CI;

   #endregion Fildz

   #region Constructors

   public Prvlg() : this(0)
   {
   }

   public Prvlg(uint ID) : base()
   {
      this.currentData = new PrvlgStruct();

      Memset0(ID);
   }

   public override void Memset0(uint ID)
   {
      this.currentData._recID  = ID;
      this.currentData._addTS  = DateTime.MinValue;
      this.currentData._modTS  = DateTime.MinValue;
      this.currentData._addUID = "";
      this.currentData._modUID = "";
      this.currentData._lanSrvID = 0;
      this.currentData._lanRecID = 0;

      this.currentData._prjktID    = 0;
      this.currentData._prjktTick  = "";
      this.currentData._userName   = "";
      this.currentData._prvlgScope = "";
      this.currentData._prvlgType  = "";
      this.currentData._documType  = "";
   }

   #endregion Constructors

   #region ToString

   public override string ToString()
   {
      return RecID + " (" + UserName + "/" + PrjktTick + ")";
   }

   public static string ToSifrarString(VvDataRecord vvDataRecord, VvSQL.SorterType sifrarType)
   {
      Prvlg prvlg_rec = (Prvlg)vvDataRecord;

      switch(sifrarType)
      {
         case VvSQL.SorterType.Person: return prvlg_rec.UserName;
         case VvSQL.SorterType.Ticker: return prvlg_rec.PrjktTick;

         default: throw new Exception(sifrarType.ToString() + " NOT DEFINED in Prvlg.ToSifrarString(VvSQL.DokumentSorterType sifrarType)");
      }

   }

   #endregion ToString

   #region propertiz

   internal PrvlgStruct CurrentData // cijela PrvlgStruct struct-ura 
   {
      get { return this.currentData; }
      set {        this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.PrvlgDao; }
   }

   public override string VirtualRecordName
   {
      get { return Prvlg.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Prvlg.recordNameArhiva; }
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

   public override uint VirtualLanSrvID { get { return this.LanSrvID; } set { this.LanSrvID = value; } }
   public override uint VirtualLanRecID { get { return this.LanRecID; } set { this.LanRecID = value; } }

   public override bool IsSifrar
   {
      get { return false; }
   }

   public override bool IsAutoSifra
   {
      get { return false; }
   }

   public override bool IsStringAutoSifra
   {
      get { return false; }
   }

   public override bool IsDocument
   {
      get { return false; }
   }

   public override bool IsDocumentLike
   {
      get { return false; }
   }

   public override bool IsPolyDocument
   {
      get { return false; }
   }

   public override bool IsTrans
   {
      get { return false; }
   }

   public override bool IsArhivable
   {
      get { return true; }
   }

   public override VvSQL.RecordSorter DefaultSorter
   {
      get { return Prvlg.sorterByPrjktTicker; }
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

   public override string VirtualIDstring { get { return ""; } }

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

   public uint PrjktID
   {
      get { return this.currentData._prjktID; }
      set { this.currentData._prjktID = value; }
   }

   public string PrjktTick
   {
      get { return this.currentData._prjktTick; }
      set { this.currentData._prjktTick = value; }
   }

   public string UserName
   {
      get { return this.currentData._userName; }
      set { this.currentData._userName = value; }
   }

   public string PrvlgScope
   {
      get { return this.currentData._prvlgScope; }
      set { this.currentData._prvlgScope = value; }
   }

   public string PrvlgType
   {
      get { return this.currentData._prvlgType; }
      set { this.currentData._prvlgType = value; }
   }

   public string DocumType
   {
      get { return this.currentData._documType; }
      set { this.currentData._documType = value; }
   }

   #endregion propertiz

   #region Implements IEditableObject

   #region Utils

   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<PrvlgStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<PrvlgStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<PrvlgStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<PrvlgStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<PrvlgStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Prvlg newObject = new Prvlg();

      Generic_CloneData<PrvlgStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      return newObject;
   }

   public Prvlg MakeDeepCopy()
   {
      return (Prvlg)Clone();
   }

   #endregion

   #region SorterCurrVal

   public static VvSQL.RecordSorter sorterByUserName = new VvSQL.RecordSorter(Prvlg.recordName, Prvlg.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.userName]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.prjktTick]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recID]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "ByUser", VvSQL.SorterType.Person, false);

   public static VvSQL.RecordSorter sorterByPrjktTicker = new VvSQL.RecordSorter(Prvlg.recordName, Prvlg.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.prjktTick]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.userName]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recID]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "ByPrjkt", VvSQL.SorterType.Ticker, false);

   private VvSQL.RecordSorter[] _sorters =
      new VvSQL.RecordSorter[]
      { 
         sorterByUserName, 
         sorterByPrjktTicker
      };

   public override VvSQL.RecordSorter[] Sorters
   {
      get { return _sorters; }
   }

   public override object[] SorterCurrVal(VvSQL.SorterType sortType)
   {
      switch(sortType)
      {
         case VvSQL.SorterType.Person: return new object[] { this.UserName,  this.PrjktTick, this.RecID, RecVer };
         case VvSQL.SorterType.Ticker: return new object[] { this.PrjktTick, this.UserName,  this.RecID, RecVer };

         default: ZXC.aim_emsg(recordName + " Nema definiran sorter " + sortType.ToString());
            return null;
      }
   }

   #endregion SorterCurrVal

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Prvlg();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Prvlg)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Prvlg)vvDataRecord).currentData;
   }

   public override string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      return SaveSerialized_VvDataRecord_ToXmlFile_JOB<Prvlg>(fileName, _isAutoCreat);
   }

   public override VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
      return DeserializeFromXmlFile<Prvlg>(fileName);
   }


   #endregion VvDataRecordFactory

}
