using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;


#region struct AmortStruct

public struct AmortStruct
{
   internal uint     _recID;
   internal DateTime _addTS;
   internal DateTime _modTS;
   internal string   _addUID;
   internal string   _modUID;

   internal uint     _lanSrvID;
   internal uint     _lanRecID;

   /* 05 */   internal uint     _dokNum;
   /* 06 */   internal DateTime _dokDate;
   /* 07 */   internal string   _tt;
   /* 08 */   internal uint     _ttNum;
   /* 09 */   internal string   _napomena;
   /* 10 */   internal bool     _flagA;
   /* 11 */   internal decimal  _dug;
   /* 12 */   internal decimal  _pot;
}
#endregion struct AmortStruct

public class Amort : VvDocumentRecord
{

   #region Fildz

   public const string recordName       = "amort";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   public const string NABAVA_TT = "NB";
   public const string AMORT_TT  = "AM";
   public const string RASHOD_TT = "RS";
   public const string INVENT_TT = "IN";

   private AmortStruct currentData;
   private AmortStruct backupData;

   protected static System.Data.DataTable TheSchemaTable = ZXC.AmortDao.TheSchemaTable;
   protected static AmortDao.AmortCI      CI             = ZXC.AmortDao.CI;

   #endregion Fildz

   #region Constructors

   public Amort() : this(0)
   {
   }

   public Amort(uint ID) : base()
   {
      this.currentData = new AmortStruct();

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
      /* 11 */      this.currentData._dug          = decimal.Zero;
      /* 12 */      this.currentData._pot          = decimal.Zero;

                    //this._transes.Clear();
                    //this.Transes = null;
                    // 21.4.2009: 
                    this.Transes = new List<Atrans>();
   }

   #endregion Constructors

   #region Sorters

   public static VvSQL.RecordSorter sorterDokNum = new VvSQL.RecordSorter(Amort.recordName, Amort.recordNameArhiva, new VvSQL.IndexSegment[]  
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.dokNum]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Br Dok", VvSQL.SorterType.DokNum, false);

   public static VvSQL.RecordSorter sorterDokDate = new VvSQL.RecordSorter(Amort.recordName, Amort.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.dokDate]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.dokNum]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Datum", VvSQL.SorterType.DokDate, false);

   public static VvSQL.RecordSorter sorterTtNum = new VvSQL.RecordSorter(Amort.recordName, Amort.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.tt]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.ttNum]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "TT Br", VvSQL.SorterType.TtNum, false);

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
         //case VvSQL.SorterType_Dokument.RecID:      return new object[] { this.RecID };
         case VvSQL.SorterType.DokNum : return new object[] { this.DokNum,               RecVer };
         case VvSQL.SorterType.DokDate: return new object[] { this.DokDate, this.DokNum, RecVer };
         case VvSQL.SorterType.TtNum  : return new object[] { this.TT,      this.TtNum,  RecVer };

         default: ZXC.aim_emsg(recordName + " Nema definiran sorter " + sortType.ToString());
            return null;
      }
   }

   public override VvSQL.RecordSorter DefaultSorter
   {
      get { return Amort.sorterDokNum; }
   }

   #endregion Sorters

   #region propertiz

   internal AmortStruct CurrentData // cijela AmortStruct struct-ura 
   {
      get { return this.currentData; }
      set {        this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.AmortDao; }
   }

   public override string VirtualRecordName
   {
      get { return Amort.recordName; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "ot"; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Amort.recordNameArhiva; }
   }

   public override string TransRecordName
   {
      get { return Atrans.recordName; }
   }

   public override string TransRecordNameArhiva
   {
      get { return Atrans.recordNameArhiva; }
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

   public override uint   VirtualLanSrvID{ get { return this.LanSrvID; } set { this.LanSrvID = value; } }
   public override uint   VirtualLanRecID{ get { return this.LanRecID; } set { this.LanRecID = value; } }

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

   /// <summary>
   /// Gets or sets a list of atrans (line items) for the amort.
   /// </summary>
   public List<Atrans> Transes { get; set; }

   /// <summary>
   /// PAZI!!! Ovdje a'o po jajima. Metode nemozes pozivati nego Invoke()... vidi dolje.
   /// get {};  vraca zapravo 'List<Atrans> Transes' konvertiran u List<VvTransRecord>
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
      this.Transes.Remove((Atrans)trans_rec);
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

   /* 05 */ public uint DokNum
   {
      get { return this.currentData._dokNum; }
      set {        this.currentData._dokNum = value; }
   }
   /* 06 */ public DateTime DokDate
   {
      get { return this.currentData._dokDate; }
      set {        this.currentData._dokDate = value; }
   }
   /* 07 */ public string TT
   {
      get { return this.currentData._tt; }
      set {        this.currentData._tt = value; }
   }
   /* 08 */ public uint TtNum
   {
      get { return this.currentData._ttNum; }
      set {        this.currentData._ttNum = value; }
   }
   /* 09 */ public string Napomena
   {
      get { return this.currentData._napomena; }
      set {        this.currentData._napomena = value; }
   }
   /* 10 */ public bool FlagA
   {
      get { return this.currentData._flagA; }
      set {        this.currentData._flagA = value; }
   }
   /* 11 */ public decimal Dug
   {
      get { return this.currentData._dug; }
      set {        this.currentData._dug = value; }
   }

   /* 12 */ public decimal Pot
   {
      get { return this.currentData._pot; }
      set {        this.currentData._pot = value; }
   }

   /**/

   //private List<Atrans> TransesNonDeleted
   private Atrans[] TransesNonDeleted
   {
      get
      {
         return this.Transes.Where(atrn => atrn.SaveTransesWriteMode != ZXC.WriteMode.Delete).ToArray();
      }
   }

   public decimal Sum_Kol
   {
      get
      {
         return this.TransesNonDeleted.Sum(atrn => atrn.T_kol);
      }
   }

   public decimal Sum_Dug
   {
      get
      {
         return this.TransesNonDeleted.Sum(atrn => atrn.T_dug);
      }
   }

   public decimal Sum_Pot
   {
      get
      {
         return this.TransesNonDeleted.Sum(atrn => atrn.T_pot);
      }
   }

   #endregion propertiz

   #region ToString

   public override string ToString()
   {
      return "dokNum: " + DokNum + " (" + DokDate.ToShortDateString() + ")" + " RecID: " + RecID;
   }

   #endregion ToString

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<AmortStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<AmortStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<AmortStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<AmortStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<AmortStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Amort newObject = new Amort();

      Generic_CloneData<AmortStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      return newObject;
   }

   public Amort MakeDeepCopy()
   {
      return (Amort)Clone();
   }

   public override void TakeTheseTranses(List<VvTransRecord> transList)
   {
      if(transList.IsEmpty()) return;

      this.Transes = transList.ConvertAll(trans => trans as Atrans);
   }

   public override void TakeTransesFrom(VvDocumentRecord _vvDocumentRecord)
   {
      if(_vvDocumentRecord.VirtualTranses == null) return;

      this.Transes = _vvDocumentRecord.CloneTranses().ConvertAll(trans => trans as Atrans);
   }

   public override List<VvTransRecord> CloneTranses()
   {
      if(this.Transes == null) return null;

      List<Atrans> newList = new List<Atrans>(this.Transes.Count);

      foreach(Atrans atrans_rec in this.Transes)
      {
         newList.Add((Atrans)atrans_rec.Clone());
      }

      return (newList.ConvertAll(trans => trans as VvTransRecord));
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Amort();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Amort)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Amort)vvDataRecord).currentData;
   }

   public override VvTransRecord VvTransRecordFactory()
   {
      return new Atrans();
   }

   public override string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      return SaveSerialized_VvDataRecord_ToXmlFile_JOB<Amort>(fileName, _isAutoCreat);
   }

   public override VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
      return DeserializeFromXmlFile<Amort>(fileName);
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

}
