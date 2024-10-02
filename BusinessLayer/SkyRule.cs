using System;
using System.Collections.Generic;

#region struct SkyRuleStruct

public struct SkyRuleStruct
{
   internal uint     _recID;
   internal DateTime _addTS;
   internal DateTime _modTS;
   internal string   _addUID;
   internal string   _modUID;

   internal uint     _lanSrvID;
   internal uint     _lanRecID;

 //internal uint     _skyServerID  ;
 //internal string   _skyServerName;
 //internal uint     _lanServerID  ;
 //internal string   _lanServerName;
   internal string   _record       ;
   internal string   _documTT      ;
 //internal string   _skladCD      ;
 //internal uint     _prjktID      ;
 //internal string   _prjktTick    ;
 //internal bool     _isExclusive  ;
 //internal uint     _frequency    ;
 //internal DateTime _timeOfDay    ;
 //internal ushort   _operation    ;

 //internal ZXC.LanSrvKind     _ruleFor     ;
   internal ZXC.LanSrvKind     _birthLoc    ;
   internal ZXC.SkySklKind     _skl1kind    ;
   internal ZXC.SkySklKind     _skl2kind    ;
   internal ZXC.SkyOperation   _centOPS     ;
   internal ZXC.SkyOperation   _shopOPS     ;
   internal ZXC.SkyReceiveKind _shopRCVkind ;
   internal bool               _centCanADD  ; 
   internal bool               _centCanRWT  ; 
   internal bool               _centCanDEL  ; 
   internal bool               _shopCanADD  ; 
   internal bool               _shopCanRWT  ; 
   internal bool               _shopCanDEL  ; 
   internal string             _opis        ;
   internal bool               _notBkgrndSND; 
   internal bool               _notSNDonExLd; 
   internal bool               _notRCVonLoad; 
}

#endregion struct SkyRuleStruct

// !!! Za sada su Prvlg i SkyRule jedini bussiness object koji nije niti VvSifrarRecord, niti VvDocumentRecord, niti VvTransRecord 
// nego diraktno VvDataRecord !!! 
public class SkyRule : VvDataRecord
{

   #region Fildz

   public const string recordName = "skyrule";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private SkyRuleStruct currentData;
   private SkyRuleStruct backupData;

   protected static System.Data.DataTable TheSchemaTable = ZXC.SkyRuleDao.TheSchemaTable;
   protected static SkyRuleDao.SkyRuleCI CI              = ZXC.SkyRuleDao.CI;

   #endregion Fildz

   #region Constructors

   public SkyRule() : this(0)
   {
   }

   public SkyRule(uint ID) : base()
   {
      this.currentData = new SkyRuleStruct();

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

      this.currentData._record        = ""   ;
      this.currentData._documTT       = ""   ;
      this.currentData._birthLoc      = ZXC.LanSrvKind   .NONE;
      this.currentData._skl1kind      = ZXC.SkySklKind.NONE;
      this.currentData._skl2kind      = ZXC.SkySklKind.NONE;
      this.currentData._centOPS       = ZXC.SkyOperation.NONE;
      this.currentData._shopOPS       = ZXC.SkyOperation.NONE;
      this.currentData._shopRCVkind   = ZXC.SkyReceiveKind.NONE;
      this.currentData._centCanADD    = false;
      this.currentData._centCanRWT    = false;
      this.currentData._centCanDEL    = false;
      this.currentData._shopCanADD    = false;
      this.currentData._shopCanRWT    = false;
      this.currentData._shopCanDEL    = false;
      this.currentData._opis          = "";
      this.currentData._notBkgrndSND  = false;
      this.currentData._notSNDonExLd  = false;
      this.currentData._notRCVonLoad  = false;

    //this.currentData._skyServerID   = 0    ;
    //this.currentData._skyServerName = ""   ;
    //this.currentData._lanServerID   = 0    ;
    //this.currentData._lanServerName = ""   ;
    //this.currentData._skladCD       = ""   ;
    //this.currentData._prjktID       = 0    ;
    //this.currentData._prjktTick     = ""   ;
    //this.currentData._isExclusive   = false;
    //this.currentData._frequency     = 0    ;
    //this.currentData._timeOfDay     = DateTime.MinValue;
   }

   #endregion Constructors

   #region ToString

   public override string ToString()
   {
      return  "[ " + Record + " " + DocumTT + " - bLoc: " + BirthLoc + " - Skl1: " + Skl1kind + " - Skl2: " + Skl2kind + " ]";
   }

   //public static string ToSifrarString(VvDataRecord vvDataRecord, VvSQL.SorterType sifrarType)
   //{
   //   SkyRule skyRule_rec = (SkyRule)vvDataRecord;

   //   switch(sifrarType)
   //   {
   //      case VvSQL.SorterType.Code: return skyRule_rec.UserName;
   //      case VvSQL.SorterType.Ticker: return skyRule_rec.PrjktTick;

   //      default: throw new Exception(sifrarType.ToString() + " NOT DEFINED in SkyRule.ToSifrarString(VvSQL.DokumentSorterType sifrarType)");
   //   }

   //}

   #endregion ToString

   #region propertiz

   internal SkyRuleStruct CurrentData // cijela SkyRuleStruct struct-ura 
   {
      get { return this.currentData; }
      set {        this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.SkyRuleDao; }
   }

   public override string VirtualRecordName
   {
      get { return SkyRule.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return SkyRule.recordNameArhiva; }
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
      get { return SkyRule.sorterByCode; }
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

   public override bool IsPrjkt_NonPUG_DataRecord { get { return (true); } }

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

   public string   Record                 { get { return this.currentData._record      ; } set { this.currentData._record       = value; } }
   public string   DocumTT                { get { return this.currentData._documTT     ; } set { this.currentData._documTT      = value; } }
   public short    DocumTTsort            { get { return ZXC.TtInfo(DocumTT).TtSort    ; }                                                 }
                                                                                                                                
   public ZXC.LanSrvKind     BirthLoc     { get { return this.currentData._birthLoc    ; } set { this.currentData._birthLoc     = value; } }
   public ZXC.SkySklKind     Skl1kind     { get { return this.currentData._skl1kind    ; } set { this.currentData._skl1kind     = value; } }
   public ZXC.SkySklKind     Skl2kind     { get { return this.currentData._skl2kind    ; } set { this.currentData._skl2kind     = value; } }
   public ZXC.SkyOperation   CentOPS      { get { return this.currentData._centOPS     ; } set { this.currentData._centOPS      = value; } }
   public ZXC.SkyOperation   ShopOPS      { get { return this.currentData._shopOPS     ; } set { this.currentData._shopOPS      = value; } }
   public ZXC.SkyReceiveKind ShopRCVkind  { get { return this.currentData._shopRCVkind ; } set { this.currentData._shopRCVkind  = value; } }
   public bool               CentCanADD   { get { return this.currentData._centCanADD  ; } set { this.currentData._centCanADD   = value; } }
   public bool               CentCanRWT   { get { return this.currentData._centCanRWT  ; } set { this.currentData._centCanRWT   = value; } }
   public bool               CentCanDEL   { get { return this.currentData._centCanDEL  ; } set { this.currentData._centCanDEL   = value; } }
   public bool               ShopCanADD   { get { return this.currentData._shopCanADD  ; } set { this.currentData._shopCanADD   = value; } }
   public bool               ShopCanRWT   { get { return this.currentData._shopCanRWT  ; } set { this.currentData._shopCanRWT   = value; } }
   public bool               ShopCanDEL   { get { return this.currentData._shopCanDEL  ; } set { this.currentData._shopCanDEL   = value; } }
   public string             Opis         { get { return this.currentData._opis        ; } set { this.currentData._opis         = value; } }
   public bool               NotBkgrndSND { get { return this.currentData._notBkgrndSND; } set { this.currentData._notBkgrndSND = value; } }
   public bool               NotSNDonExLd { get { return this.currentData._notSNDonExLd; } set { this.currentData._notSNDonExLd = value; } }
   public bool               NotRCVonLoad { get { return this.currentData._notRCVonLoad; } set { this.currentData._notRCVonLoad = value; } }

   public ZXC.SkyOperation   TheSkyOperation
   {
      get
      {
         if(ZXC.ThisLanServerKind == ZXC.LanSrvKind.CENT) return this.CentOPS;
         if(ZXC.ThisLanServerKind == ZXC.LanSrvKind.SHOP) return this.ShopOPS;

         throw new Exception("ZXC.ThisLanServerRuleKind UNDEFINED for TheSkyOperation");
      }
   }

   // =================================================================================== 

 //public List<VvSQL.VvSkyLogEntry> TheSkyLogList { get; set; }
 //public List<VvSQL.VvLanLogEntry> TheLanLogList { get; set; }

   // =================================================================================== 

   #endregion propertiz

   #region Implements IEditableObject

   #region Utils

   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<SkyRuleStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<SkyRuleStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<SkyRuleStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<SkyRuleStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<SkyRuleStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      SkyRule newObject = new SkyRule();

      Generic_CloneData<SkyRuleStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      return newObject;
   }

   public SkyRule MakeDeepCopy()
   {
      return (SkyRule)Clone();
   }

   #endregion

   #region SorterCurrVal

   public static VvSQL.RecordSorter sorterByCode = new VvSQL.RecordSorter(SkyRule.recordName, SkyRule.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.record    ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.documTT   ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.birthLoc  ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.skl1kind  ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.skl2kind  ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer    ], true)
      }, "ByCode", VvSQL.SorterType.Code, false);

   private VvSQL.RecordSorter[] _sorters =
      new VvSQL.RecordSorter[]
      { 
         sorterByCode
      };

   public override VvSQL.RecordSorter[] Sorters
   {
      get { return _sorters; }
   }

   public override object[] SorterCurrVal(VvSQL.SorterType sortType)
   {
      switch(sortType)
      {
         case VvSQL.SorterType.Code: return new object[] { this.Record, this.DocumTT, this.BirthLoc, this.Skl1kind, this.Skl2kind, RecVer };

         default: ZXC.aim_emsg(recordName + " Nema definiran sorter " + sortType.ToString());
            return null;
      }
   }

   #endregion SorterCurrVal

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new SkyRule();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((SkyRule)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((SkyRule)vvDataRecord).currentData;
   }

   public override string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      return SaveSerialized_VvDataRecord_ToXmlFile_JOB<SkyRule>(fileName, _isAutoCreat);
   }

   public override VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
      return DeserializeFromXmlFile<SkyRule>(fileName);
   }


   #endregion VvDataRecordFactory

}
