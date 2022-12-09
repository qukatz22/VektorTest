using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;


#region struct NalogStruct

public struct NalogStruct
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
   /* 13 */   internal string   _devName     ;
}
#endregion struct NalogStruct

public class Nalog : VvDocumentRecord
{

   #region Fildz

   public const string recordName       = "nalog";
   public const string recordNameArhiva =  recordName + VvDataRecord.ArhRecNameExstension;

   public const string PS_TT = "PS";
   public const string IZ_TT = "IZ";
   public const string KP_TT = "KP"; // kompenzacija 
   public const string KP_OK = "OK"; // korekcija lipa OTSa, ... 

   public const string TT_TCR = "TÈR";

   private NalogStruct currentData;
   private NalogStruct backupData;

   protected static System.Data.DataTable TheSchemaTable = ZXC.NalogDao.TheSchemaTable;
   protected static NalogDao.NalogCI CI                  = ZXC.NalogDao.CI;

   #endregion Fildz

   #region Constructors

   public Nalog() : this(0)
   {
   }

   public Nalog(uint ID) : base()
   {
      this.currentData = new NalogStruct();

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
      /* 13 */      this.currentData._devName      = "";

                    //this._transes.Clear();
                    //this.Transes = null;
                    // 21.4.2009: 
                    this.Transes = new List<Ftrans>();
   }

   #endregion Constructors

   #region Sorters

   public static VvSQL.RecordSorter sorterDokNum = new VvSQL.RecordSorter(Nalog.recordName, Nalog.recordNameArhiva, new VvSQL.IndexSegment[]  
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.dokNum]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Br Dok", VvSQL.SorterType.DokNum, false);

   public static VvSQL.RecordSorter sorterDokDate = new VvSQL.RecordSorter(Nalog.recordName, Nalog.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.dokDate]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.dokNum]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Datum", VvSQL.SorterType.DokDate, false);

   public static VvSQL.RecordSorter sorterTtNum = new VvSQL.RecordSorter(Nalog.recordName, Nalog.recordNameArhiva, new VvSQL.IndexSegment[]
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
      get { return Nalog.sorterDokNum; }
   }

   #endregion Sorters

   #region propertiz

   internal NalogStruct CurrentData // cijela NalogStruct struct-ura 
   {
      get { return this.currentData; }
      set { this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.NalogDao; }
   }

   public override string VirtualRecordName
   {
      get { return Nalog.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Nalog.recordNameArhiva; }
   }

   public override string TransRecordName
   {
      get { return Ftrans.recordName; }
   }

   public override string TransRecordNameArhiva
   {
      get { return Ftrans.recordNameArhiva; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "na"; }
   }

   public override uint VirtualRecID
   {
      get { return this.RecID; }
      set {        this.RecID = value; }
   }

   public override DateTime VirtualAddTS  { get { return this.AddTS; } }
   public override DateTime VirtualModTS  { get { return this.ModTS; } }
   public override string   VirtualAddUID { get { return this.AddUID; } }
   public override string   VirtualModUID { get { return this.ModUID; } }

   public override uint VirtualLanSrvID { get { return this.LanSrvID; } set { this.LanSrvID = value; } }
   public override uint VirtualLanRecID { get { return this.LanRecID; } set { this.LanRecID = value; } }

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
   /// Gets or sets a list of ftrans (line items) for the nalog.
   /// </summary>
   public List<Ftrans> Transes { get; set; }

   /// <summary>
   /// PAZI!!! Ovdje a'o po jajima. Metode nemozes pozivati nego Invoke()... vidi dolje.
   /// get {};  vraca zapravo 'List<Ftrans> Transes' konvertiran u List<VvTransRecord>
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
      //get { return (((IList<VvDataRecord>)((IList<Ftrans>)Transes))); }

      get
      {
         //OBSOLETE / Sepsolete: if(this.Transes != null) virtualTranses = Transes.Convert_ListToVvList(FtransConverter, virtualTranses);
         
         //if(this.Transes != null) virtualTranses = Transes.ConvertAll(FtransConverter);
         if(this.Transes != null) return Transes.ConvertAll(ftr => ftr as VvTransRecord);
         else                     return null;

         //return virtualTranses;

         //return (VvList<VvTransRecord>) Transes.ConvertAll<VvTransRecord>(FtransConverter); // casting to more derived type ti bas i ne sljaka 
      }
      //set
      //{
      //   virtualTranses = value;
      //}
   }

   //private Converter<Ftrans, VvTransRecord> FtransConverter = 
   //   new Converter<Ftrans, VvTransRecord>(delegate(Ftrans ftrans) 
   //      { 
   //         return ftrans as VvTransRecord; 
   //      } );

   public override void InvokeTransClear()
   {
      if(this.Transes != null) this.Transes.Clear();
   }

   public override void InvokeTransRemove(VvTransRecord trans_rec)
   {
      this.Transes.Remove((Ftrans)trans_rec);
   }

   //private static Converter<Ftrans, VvTransRecord> FtransConverter = new Converter<Ftrans, VvTransRecord>(FtransToVvDataRecord);
   //private static VvTransRecord FtransToVvDataRecord(Ftrans ftrans)
   //{
   //   return ftrans as VvTransRecord;
   //}

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

   /* 05 */
   public uint DokNum
   {
      get { return this.currentData._dokNum; }
      set { this.currentData._dokNum = value; }
   }
   /* 06 */
   public DateTime DokDate
   {
      get { return this.currentData._dokDate; }
      set { this.currentData._dokDate = value; }
   }
   /* 07 */
   public string TT
   {
      get { return this.currentData._tt; }
      set { this.currentData._tt = value; }
   }
   /* 08 */
   public uint TtNum
   {
      get { return this.currentData._ttNum; }
      set { this.currentData._ttNum = value; }
   }
   /* 09 */
   public string Napomena
   {
      get { return this.currentData._napomena; }
      set { this.currentData._napomena = value; }
   }
   /* 10 */
   public bool FlagA
   {
      get { return this.currentData._flagA; }
      set { this.currentData._flagA = value; }
   }
   /* 11 */
   public decimal Dug
   {
      get { return this.currentData._dug; }
      set { this.currentData._dug = value; }
   }

   /* 12 */
   public decimal Pot
   {
      get { return this.currentData._pot; }
      set { this.currentData._pot = value; }
   }

   /* 13 */
   public string DevName
   {
      get { return this.currentData._devName; }
      set { this.currentData._devName = value; }
   }
   /**/

   public ZXC.ValutaNameEnum DevNameAsEnum
   {
      get
      {
         if(DevName.IsEmpty()) return ZXC.ValutaNameEnum.EMPTY;
         else                  return (ZXC.ValutaNameEnum)Enum.Parse(typeof(ZXC.ValutaNameEnum), DevName, true);
      }
   }

   public decimal DevTecaj { get { return ZXC.DevTecDao.GetHnbTecaj(this.DevNameAsEnum, this.DokDate); } }

   //private List<Ftrans> TransesNonDeleted
   /*private*/public Ftrans[] TransesNonDeleted
   {
      get
      {
         return this.Transes.Where(ftrn => ftrn.SaveTransesWriteMode != ZXC.WriteMode.Delete).ToArray();
      }
   }

   public decimal Sum_Dug
   {
      get
      {
         return this.TransesNonDeleted.Sum(ftrn => ftrn.T_dug);
      }
   }

   public decimal Sum_Pot
   {
      get
      {
         return this.TransesNonDeleted.Sum(ftrn => ftrn.T_pot);
      }
   }

   public decimal Saldo
   {
      get
      {
         return (Sum_Dug - Sum_Pot).Ron2();
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
      Generic_RestoreBackupData<NalogStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<NalogStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<NalogStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<NalogStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<NalogStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Nalog newObject = new Nalog();

      Generic_CloneData<NalogStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      return newObject;
   }

   public Nalog MakeDeepCopy()
   {
      return (Nalog)Clone();
   }

   //public override void TakeTransesFrom(VvDocumentRecord _vvPolyDocumRecord)
   //{
   //   if(_vvPolyDocumRecord.VirtualTranses == null) return;

   //   this.Transes = new List<Ftrans>(_vvPolyDocumRecord.VirtualTranses.ConvertAll(trans => trans as Ftrans));
   //}

   public override void TakeTheseTranses(List<VvTransRecord> transList)
   {
      if(transList.IsEmpty()) return;

      this.Transes = transList.ConvertAll(trans => trans as Ftrans);
   }

   public override void TakeTransesFrom(VvDocumentRecord _vvDocumentRecord)
   {
      if(_vvDocumentRecord.VirtualTranses == null) return;

      this.Transes = _vvDocumentRecord.CloneTranses().ConvertAll(trans => trans as Ftrans);
   }

   public override List<VvTransRecord> CloneTranses()
   {
      if(this.Transes == null) return null;

      List<Ftrans> newList = new List<Ftrans>(this.Transes.Count);

      foreach(Ftrans ftrans_rec in this.Transes)
      {
         newList.Add((Ftrans)ftrans_rec.Clone());
      }

      return (newList.ConvertAll(trans => trans as VvTransRecord));
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Nalog();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Nalog)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Nalog)vvDataRecord).currentData;
   }

   public override VvTransRecord VvTransRecordFactory()
   {
      return new Ftrans();
   }

   public override string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      return SaveSerialized_VvDataRecord_ToXmlFile_JOB<Nalog>(fileName, _isAutoCreat);
   }

   public override VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
      return DeserializeFromXmlFile<Nalog>(fileName);
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

   #region Ftrans Balance

   public decimal SetFtransesInBalance(decimal saldo)
   {
      return Nalog.SetFtransesInBalance(this.Transes, saldo);
   }

   public static decimal SetFtransesInBalance(List<Ftrans> ftransList, decimal theSaldo)
   {
      // usedAddition ne bi smio iznositi vise od 0.03kn po stavci jer 0.04kn je vec u riziku da EUR iznos promjeni za 0.01cent! 

      decimal usedAddition;

      decimal saldoABS     = Math.Abs(theSaldo);
      decimal saldoLeft    = theSaldo          ;
      int     rowCount     = ftransList.Count  ;

      bool saldoIsPositive = theSaldo.IsPositive();
      bool saldoIsNegative = theSaldo.IsNegative();

      bool canWeGoByOneLipaSteps = saldoABS / 0.01M <= rowCount;
      bool canWeGoByTwoLipaSteps = saldoABS / 0.02M <= rowCount;
      bool canWeGoByTriLipaSteps = saldoABS / 0.03M <= rowCount;

      if(canWeGoByOneLipaSteps == false &&
         canWeGoByTwoLipaSteps == false &&
         canWeGoByTriLipaSteps == false  )
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Ne mogu izbalansirati.");

         return 0M; // do nothin' 
      }

           if(canWeGoByOneLipaSteps) usedAddition = 0.01M;
      else if(canWeGoByTwoLipaSteps) usedAddition = 0.02M;
      else if(canWeGoByTriLipaSteps) usedAddition = 0.03M;
      else                           usedAddition = 0.00M;

      //if(saldoIsNegative) usedAddition *= -1M;

      foreach(var ftrans in ftransList.OrderByDescending(ftr => ftr.MaxAbsDugPot)) // lista sortirana desc po iznosu MaxAbsDugPot 
      {
         if(saldoIsPositive)
         { 
            ftransList.SingleOrDefault(ftr => ftr.T_serial == ftrans.T_serial).CorrectDugOrPot(usedAddition <= saldoLeft ? usedAddition : saldoLeft, true);
            saldoLeft -= usedAddition;
            if(saldoLeft.IsZeroOrNegative()) break;
         }
         else if(saldoIsNegative)
         {
            ftransList.SingleOrDefault(ftr => ftr.T_serial == ftrans.T_serial).CorrectDugOrPot(usedAddition <= -saldoLeft ? usedAddition : -saldoLeft, false);
            saldoLeft += usedAddition;
            if(saldoLeft.IsZeroOrPositive()) break;
         }
      }

      return usedAddition;
   }

   #endregion Ftrans Balance
}
